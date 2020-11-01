(*  $Id: dpmap.fs,v 1.15 2013-03-26 10:08:43 djg11 Exp $
 * Orangepath: datapath-mapper.
 *
 *
 *)

module dpmap

open meox
open yout
open moscow
open hprls_hdr
open abstract_hdr
open abstracte
open opath_hdr

(*
 * Rather than generate the datapath in-line with the controller, we can map
 * it into DPATH-style tiles where greater sharing of resources between
 * states is likely to be possible, compared with the straightforward subexpression sharing we
 * otherwise resort to.
 *)
let log_dp_operation dpath seq guard lhs value = 
    match seq with
        | None -> sf ("attempted use of datapath generator without a sequencer!")
        | (Some(pc, point))  ->
            let ops = (dpath:Dpath_t).ops
            let _ = mutadd ops (pc, point, guard, lhs, value)
            ()


(*
 * Certain operators, such as multiplexors, ones-comp and shifts, are considered to have no cost
 * in the DPATH generation, and so stored not as DPATH_funcs but as wrappers
 * around the the outputs of other dpath_nodes.
 * Simple boolean logic gates can might sometimes be thought of in this way: depening on the power of the 'synapses' in the target technology.
 * All reads to a given array share the same node, although this could be split later
 * for a dual-ported array.
 *) 
type dp_point_t = (hexp_t * hexp_t)                                    // makes links/cycles ?


type dp_func_t =
| DP_func_diop of precision_t * x_diop_t                                              // ALU diadic operator 
| DP_func_fcall of (string *native_fun_signature_t)                     // Function calls to library units (not valid tiles)


type dpath_node_t = 
| DPATH_func   of dp_point_t * dp_func_t * dpath_link_t list           // Main computation node for arithmetic and function calls

| DPATH_abfunc of dp_point_t * x_abdiop_t * dpath_link_t list * bool   // Computation node for associative boolean operators
| DPATH_bfunc  of dp_point_t * x_bdiop_t * dpath_link_t list * bool    // Computation node for comparison predicates (normally always <).

| DPATH_sink of dp_point_t * dpath_link_t * hexp_t * dpath_link_t      // Consumer of an expression
| DPATH_src of dp_point_t * hexp_t                                     // Source of a value (e.g. variable or array location)
| DPATH_leaf of hexp_t                                                 // Constant value
| DPATH_bleaf of hbexp_t                                               // Constant boolean value

and dpath_link_t =
  DPATH_wrap of x_diop_t * dpath_link_t list                          // Wrapper for 'zero-cost' function, such as a bit shift.
| DPATH_id of int                                                     // Numbering system of some sort
| DPATH_mux of dpath_link_t * dpath_link_t * dpath_link_t             // Straightforward mux a?b:c - or Structural fanout primitive ? Get's introduced as we start to fold ?



let rec dpathToStr = function // Summary form.
    | DPATH_func(pp, dop, args) -> "DPATH_func()"
    | DPATH_bfunc(pp, oo, _, invf) -> "DPATH_bfunc(pp, " + f1o4(xbToStr_dop oo) + ", ..., " + boolToStr invf + ")"
    | DPATH_abfunc(pp, oo, _, invf) -> "DPATH_abfunc(pp, " + f1o3(xabToStr_dop oo) + ", ..., " + boolToStr invf + ")"
    | DPATH_src _ -> "DPATH_src()"
    | DPATH_sink _ -> "DPATH_sink()"
    | DPATH_leaf d -> "DPATH_leaf(" + xToStr d + ")"
    | DPATH_bleaf d -> "DPATH_bleaf(" + xbToStr d + ")"
    //|  _ -> "dpathToStr??"

and dlinkToStr = function
    | DPATH_mux(g, l, r) -> "DPATH_mux(" + dlinkToStr g + ", ...)"
    | _ -> "dlinkToStr??"




// could use a .net native hash for this ?

let rec dpath_link_hash = function
    | DPATH_id(n) -> n * 32
    | DPATH_wrap(_, lst)  -> foldl (fun(a,b)->hash_digest(dpath_link_hash a, b)) 0 lst
    | DPATH_mux(a,b,c)    -> foldl (fun(a,b)->hash_digest(dpath_link_hash a, b)) 0 [a;b;c]
   // | (_) -> sf "match dpath_link_hash"
    
and dpath_hash = function
    | DPATH_func(_, _, lst)
    | DPATH_bfunc(_, _, lst, _)
    | DPATH_abfunc(_, _, lst, _) ->  foldl (fun(a,b)->hash_digest(dpath_link_hash a, b)) 0 lst
    | DPATH_src(_, v) -> x2nn v
    | DPATH_sink(pp, l, _, _) -> dpath_link_hash l
    | DPATH_leaf(d)           ->  x2nn d
    | DPATH_bleaf(d)          -> xb2nn d
    //|  other -> sf("dpath_hash other: " + dpathToStr other)




let collate_ops c A =
    match A with
        | DPATH_func(_, _, l::t)   
        | DPATH_bfunc(_, _, l::t, _)
        | DPATH_abfunc(_, _, l::t, _) -> A::c

        (* Following are not ops. *)
        | DPATH_src(_, v) -> c
        | DPATH_sink(pp, l, _, _) -> c
        | DPATH_leaf(d) ->  c
        | DPATH_bleaf(d) -> c
        | other -> sf("collate_ops other: " + dpathToStr other)



let rec dig = function
    | (DPATH_wrap(_, l)) -> list_once(list_flatten(map dig l))
    | (DPATH_mux(a, b, c)) -> list_once(list_flatten(map dig [a;b;c]))
    | (DPATH_id n) -> [n]
    //| (_) -> sf ("dig other")


(*
 * Read out the material from the Dpath structure into a mesh, optimise it and then emit in hbev/xrtl form.
 *)
let create_datapath_netlist ww (dPath:Dpath_t) =
    let vd = 3
    let _ = WF 3 "create_datapath_netlist" ww "Start"
    let ops = !(dPath.ops)
    let op_point(pc, point, g, l, r) = point
    let op_pc(pc, point, g, l, r) = pc
    let pcs = foldl (fun (oo, c)-> singly_add (op_pc oo) c) [] ops
    let pcno = length pcs
    let justarc(pc, point, g, l, r) = gen_XRTL_arc(Some(pc, point), g, l, r)
    in if pcno=0 then (map justarc ops, [])
       else 
        let _ = cassert(length pcs = 1, "More than one PC in single datapath")
        let points = foldl (fun (oo, c)-> singly_add (op_point oo) c) [] ops
        let collate_by_point pt = (pt, List.filter (fun oo->op_point oo=pt) ops)
        let collated = map collate_by_point  points
        let m = "create_datapath_netlist pc=" + xToStr(hd pcs)
        let _ = WF 3 m ww ("points=" + i2s(length points))
        let max = 100000
        let idx = ref 0
        let nextidx() =
            let _ = cassert(!idx<max, "Too many dpath nodes");
            let rr = !idx
            idx := (!idx)+1
            rr
        let NODES = Array.create max []
        let NODES_idx = System.Collections.Generic.Dictionary<int, dpath_link_t>()
        let Redirects = Array.create max None (* Redirection of one node number to another *)
        let rec redirect n = let ov=Redirects.[n] in if ov=None then n else redirect(valOf ov)

        let insleaf x = 
            let h = dpath_hash x
            let (found, ov) = NODES_idx.TryGetValue(h)
            if found then ov
            else
                 let n = nextidx()
                 Array.set NODES  n [x]
                 let _ = lprint vd (fun()->"Create build node" + i2s n + ":" + dpathToStr x + "\n")     
                 let ans = (DPATH_id n)
                 // kills it 
                 NODES_idx.Add(h, ans) /// hmmm ? converted wrongly?
                 ans

        let rec insert_e pp x =
            match x with
                | (X_bnet f) -> (* memoise of src?*) insleaf (DPATH_src(pp, x))

                | W_asubsc(l, s, _) -> insleaf (DPATH_leaf x) (* for now *)

                | W_node(prec, oo, lst, _) -> 
                        let lst' = map (insert_e pp) lst
                        if oo=V_onesc || (oo=V_lshift && constantp(hd(tl lst)))
                             || (is_rshift oo && constantp(hd(tl lst)))
                        then 
                             (DPATH_wrap(oo, lst'))
                        else insleaf (DPATH_func(pp, DP_func_diop(prec, oo), lst'))


                | W_apply(gis, _, alst, _) -> 
                    let lst' = map (insert_e pp) alst
                    insleaf (DPATH_func(pp, DP_func_fcall gis, lst'))
                        


                | (X_blift e) -> binsert_e pp e



                | W_query(g, tt, ff, _) -> 
                     let (g', tt', ff') = (binsert_e pp g, insert_e pp tt, insert_e pp ff)
                     in DPATH_mux(g', tt', ff')

        
                | e -> if fconstantp e then insleaf (DPATH_leaf e)
                       else sf("other in dpath insert_e:" + xToStr e)    

        and binsert_e pp = function
            | W_bnode(oo, lst, invf, _) ->
                         let lst' = map (binsert_e pp) lst
                         in insleaf(DPATH_abfunc(pp, oo, lst', invf))

            | W_bdiop(oo, lst, invf, _) -> 
                         let lst' = map (insert_e pp) lst
                         in insleaf(DPATH_bfunc(pp, oo, lst', invf))

            | e -> if bconstantp e then insleaf (DPATH_bleaf e)
                   else sf("other in dpath binsert_e:" + xbToStr e) 


        let insert_op(pc, point, g, l, r) = 
            let pp = (pc, point)
            let r' = insert_e pp r
            let g' = binsert_e pp g
            let n = nextidx()
            vprint vd ("Create sink node" + i2s n + ":" + xToStr l + "\n")      
            let a = DPATH_sink(pp, g', l, r')
            Array.set NODES n [a]
            n

        let targets = map insert_op ops
        let _ = WF 3 m ww ("insert complete")



        let rec retrieve_opnodes n =
            if n >= (!idx) then []
            else
                let k = NODES.[n]
                let is_opnode = function
                    | (DPATH_func _) -> true
                    | _ -> false
                in if foldl (fun(a,c)->c || is_opnode a) false k then n::(retrieve_opnodes(n+1)) else retrieve_opnodes(n+1)

            
        let point_valOf = function
            | DPATH_func(pp, _, _)    -> pp
            | DPATH_sink(pp, g, l, r) -> pp
            | (other) -> sf("point_valOf " + dpathToStr other)

        let op_valOf = function
            | DPATH_func(pp, oo, _) -> oo
            | (other) -> sf("op_valOf " + dpathToStr other)

        let sup_valOf = function
            | DPATH_func(pp, oo, lst) -> list_once(list_flatten(map dig lst))  (*Hmmm dont we want the rbus of this func ? *)

            // |   sup_valOf(DPATH_mux(g, l, r)) -> list_once(list_flatten(map dig [l, r]))
             (* Dont include g since likely to be control not data. For now. *)

            | DPATH_sink(pp, g, l, r) -> dig r  (* Dont include g since likely to be control not data. For now. *)
            | (other) -> sf("sup_valOf " + dpathToStr other)

        let points n = list_once(map point_valOf (NODES.[redirect n]))
        let ops n = list_once(map op_valOf (NODES.[redirect n]))
        let sup n = list_once(list_flatten(map sup_valOf (NODES.[redirect n])))

        (* Do n+2 search of all pairs *)
        let rec search = function
            | ([], sofar) -> sofar
            | (h::t, sofar) -> 
                let h = redirect(h)
                let opsh = ops h
                let rec s1 = function
                    | ([], sofar) -> sofar
                    | (h1::t1, sofar0) -> 
                        let sofar = s1(t1, sofar0)
                        let h1 = redirect(h1)
                        in if list_intersection_pred(points h1, points h) then sofar (* If same point, cannot reuse *)
                           else
                              let src_score = length(list_intersection(sup h1, sup h))
                              let op_score = if list_intersection_pred(ops h1, opsh) then 2 else 0
                              let score = src_score + op_score
                              let sofar' = if sofar=None || score > fst(valOf sofar) then Some(score, (h, h1)) else sofar
                              in sofar'
                in search(t, s1(t, sofar))
               


        let rec iterate(opnodes) = 
            let donesome = ref false
            let a = search (opnodes, None)
            in if (a=None) then ()
                else
                let (score, (p, q)) = valOf a           
                    in if (score < 2) then ()
                       else
                           vprint 3 ("Dpath share of " + i2s p + " and " + i2s q + " with score " + i2s score + "\n")
                           Array.set Redirects p (Some q)
                           Array.set NODES q (lst_union NODES.[p] NODES.[q])
                           Array.set NODES p []
                           iterate(list_subtract(opnodes, [p]))
                   

        let _ = iterate(retrieve_opnodes 0)

        let _ = WF 3 m ww ("placement complete")



        let hw = ref []
        let RBUS = Array.create max None (* Resultant output bus from each node *)

        let rec build_e logicf = function
            | (DPATH_mux(g, l, r)) when logicf=false -> 
                let ans = xi_query(build_b g, build_a l, build_a r)
                ans

            | (DPATH_id n0) ->
                let n = redirect n0
                let k = NODES.[n]
                let rbus = RBUS.[n]
                in
                if rbus<> None then valOf rbus
                else

                let rec muxg = function
                    | [] -> (sf("dpath build: no funcs in node\n"); xi_num 12345677) 

                    | (DPATH_leaf(c)::tt) -> (cassert(tt=[], "const has tail"); c) (* not muxed at the moment *)


                    | (DPATH_src(pp, vale)::tt) -> 
                         let g = xi_deqd(fst pp, snd pp)
                         let nr = if tt=[] then vale else xi_query(g, vale, muxg tt) 
                         nr

                    | (DPATH_func(pp, oo, lst)::tt)  ->
                         let g = xi_deqd(fst pp, snd pp)
                         let lst' = map build_a lst 
                         let _ = cassert(length lst' = 2, "not a diadic operator")
                         let k = function
                             | DP_func_diop(prec, oo) -> ix_op prec oo lst'
                             | DP_func_fcall gis      -> xi_apply(gis, lst')
                             //| (other) -> sf ("DPATH_func k other")

                         let ans = k oo 
                         let nr = if tt=[] then ans else ix_query g ans (muxg tt) 
                         nr



                    | (other::tt)  -> 
                        let needs_lift = function
                            | (DPATH_bleaf _) -> true
                            | (DPATH_bfunc _) -> true
                            | (DPATH_abfunc _) -> true
                            | (_) -> false
                        in if (needs_lift other) then xi_blift(bmuxg(other::tt))
                           else sf("muxg other: " + dpathToStr other)
                   

                and bmuxg = function
                    | ([(DPATH_bleaf c)]) -> c (* not muxed at the moment *)
                    
                    | ([])  -> sf "dpath build: [] bools"

                    | (DPATH_bfunc(pp, oo, lst, inv)::tt) ->
                         let g = xi_deqd(fst pp, snd pp)
                         let lst' = map build_a lst 
                         let ans = ix_bdiop oo lst' inv 
                         let nr = if tt=[] then ans else xi_orred(xi_query(g, xi_blift ans, xi_blift(bmuxg tt)))
                         nr

                    | (DPATH_abfunc(pp, oo, lst, inv)::tt) ->
                         let g = xi_deqd(fst pp, snd pp)
                         let lst' = map build_b lst 
                         let ans = xi_bnode(oo, lst', inv) 
                         let nr = if tt=[] then ans else xi_orred(xi_query(g, xi_blift ans, xi_blift(bmuxg tt)))
                         nr

                    | (other :: tt) ->
                        let needs_red = function
                            | (DPATH_leaf _) -> true
                            | (DPATH_src _) -> true
                            | (DPATH_func _) -> true
                            | (_) -> false
                        in if (needs_red other) then xi_orred(muxg(other::tt))
                           else sf("bmuxg other: " + dpathToStr other)
                   

                let aops = List.fold collate_ops [] k

                let r00 = if logicf then xi_blift(bmuxg k) else muxg k 
                let ans = if aops<>[]
                          then 
                             let rbus = vectornet_w(funique "rbus", 32)  (* signed? TODO set width *)
                             vprintln 3 ("Set up " + xToStr rbus + " for ops " + sfold dpathToStr aops)
                             let hw_assign(l, r) = XIRTL_buf(X_true, l, r)
                             mutadd hw (hw_assign(rbus, r00)) 
                             Array.set RBUS  n (Some rbus)
                             rbus

                          else r00

                ans

            | (other) -> sf ("build_e")

        and build_a (* arith *) e = build_e false e
        and build_b (* bools *) e = xi_orred(build_e true e)


        let rept xi = (vprintln 0 ("Gen xi " + xrtlToStr xi); xi)
        let build n =
            let k = NODES.[n]
            let _ = lprint vd (fun()->"Start build " + i2s n + "\n")        
            let b1 = function
                | DPATH_sink(pp, g, l, r) ->
                    let g' = build_b g
                    let r' = build_a r

                    let hw_nba(pp, g, l, r) = gen_XRTL_arc(Some pp, g, l, r)
                    mutadd hw ((*rept*)(hw_nba(pp, g', l, r')) )
                    
                    if vd >= 5 then vprintln 5 ("End build " + i2s n)
                    ()
                | other -> sf "b1 other"

            let _ = app b1 k
            in 0

        let _ = map build targets

        let rec netget(p, c) = if p >= (!idx) then c
                               else netget(p+1, if RBUS.[p]<>None then valOf(RBUS.[p])::c else c)

        let nets = netget(0, [])
        let _ = WF 3 m ww ("build complete. edges=" + i2s(length(!hw))) 

        (!hw, nets)


(* {\tt -dpmap=disable} disables the DPATH code generation phase.
 *This pahse allocates the operators of a thread  to
 *physical resources (RTL operators) that are temporarily re-used as the
 *thread performs different operations.  Without the DPATH code generator, the
 *data path operations are not split out from the sequencer for the thread.
 *The DPATH phase has no effect when a thread does not needs a sequencer.
 *)

let dpath_modes = ["none"; "simple"; "full" ]; // not used ?
          

(*
 * Data path generator: takes a VM and generates a datapath (for each thread?) that has a sequencer. 
 * The datapath shares structural resources between thread states.
 *)
type K3_t =
    {
       m: string;

    }


let rec opath_dpath_w ww0 (K3:K3_t) arg = 
    match arg with 
        |(ii, None) -> (ii, None)
        |(ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
            let kvd = 3
            let ww = WN ("dpath_exec " + ii_fold [ii]) ww0
            let sons' = map (opath_dpath_w ww K3) sons
            let fsmk dp pc resume (rcode, cmds, sh) call  =
                match call with
                    | XIRTL_pli(pp__, g, (f, gis), args) ->
                        let _ = lprintln kvd (fun()->"dpath: xirtl pli call guard is " + xbToStr g)
                        // PLI is ignored for now - shame
                        in (rcode, cmds, call::sh)

                    | XRTL(pp_, g, lst) -> //TODO pass in just the lst here to avoid pp_ having to be ignored
                        let q arg (rcode, cmds, sh) =
                            match arg with
                            | Rarc(g1, l, v) -> 
                               let pp = Some(pc, resume)
                               if (l=pc && pp<>None && snd(valOf pp) = v) then (rcode, cmds, sh) (*Delete arcs to current state*)
                               //  Brutal convert to Verilog here as member of switch or not. 
                               // Or else put datapath arcs into datapath generator. 
                               elif l<>pc then (log_dp_operation dp pp (xi_and(g, g1)) l v; (rcode, cmds, sh))
                               else (rcode, cmds, call::sh)
                            | Rpli(g, (f, gis), args) ->
                                let _ = lprintln kvd (fun()->"dpath: xirtl pli call guard is " + xbToStr g)
                                // PLI is ignored for now - shame
                                in (rcode, cmds, call::sh)

                        in List.foldBack q lst (rcode, cmds, sh)
                        
            let vcmd = function
                | XRTL(pp, g, lst) -> lst <> []// Idempotent cmd: repeated instances of it are not needed.
                | _ -> false // TODO LOOK AT GIS PROPS OF A PLI ?
                
            let rec once_cmda = function
                    | [] -> [] (* For repeated commands that are idempotent/order, form disjunction of their guards *)
                    | [item] -> [item]
                    | ((g, cmd)::t) ->
                        let rec scan = function
                            | [] -> None
                            | ((g', cmd')::tt) -> if cmd=cmd' then Some g' else scan tt
                        let others = if vcmd cmd then scan t else None
                        if others=None then (g, cmd)::(once_cmda t)
                        else (
                                           // vprint(0, verilog_render_bev(cmd) + " was combined " + xbToStr g + " v " + xbToStr(valOf others) + "\n");
                                           once_cmda((xi_or(valOf others, g), cmd)::list_subtract(t, [(valOf others, cmd)]))
                                 )
            


            let once_cmd x = 
                let rr = once_cmda x
                //reportx 0 "before once" (fun (a,b)->verilog_render_bev b) x
                //reportx 0 "after once" (fun (a,b)->verilog_render_bev b) rr
                rr

            // Insert a list of states into a dp
            let rec fsm_state_dp dp (i:fsm_info_t) = function
                | ([], r, states_sofar) -> (r, rev states_sofar)

                | ((si:vliw_arc_t)::tt, r, states_sofar)  -> 
                    let _ = vprintln 3 "Start a dpath state"
                    let (r', m', sh) = List.fold (fsmk dp i.pc si.resume) ([], [], []) si.cmds (* foldl/fsmk combination does not reverse pli call ordering ? *)
                    let newclause = (si.resume, m' @ (*once_cmd*) sh)
                    let si' =
                        { si with
                          //cmds=actions;
                          //gtop=muddy "gtop guard missing";
                          //resume=muddy "r missing";
                          //dest=muddy "dest missing";
                             pc=si.pc;
                        }

                    vprintln 3 "End a dpath state\n"
                    fsm_state_dp dp i (tt, lst_union r r', si' :: states_sofar)


            let rec dpath_sp A =
                    // We currently handle only the following forms of VM executable code
                match A with
                    | SP_fsm(i, statelist) ->
                        // First read it in to dpform
                        let create_dpath_recorder m = { ops=ref []; id=m; }
                        let dp = create_dpath_recorder K3.m
                        let (resets, states') = fsm_state_dp dp i (statelist, [], [])
                        vprintln 3 ("Processing SP_fsm resets=" + i2s(length resets) + " clauses=" + i2s(length states'))
                                
                                // Output as dot here

                                // ... Here we manipulate it as we want
                                // ... missing!
                                
                                // Now write out again
                        let (dpath_arcs, dpath_nets) = create_datapath_netlist (WN (K3.m + ": create_dpath_netlist") ww) dp
                        let ans =
                            let rtlctrl = { id="dpath_rezzed" }:rtl_ctrl_t
                            [ SP_fsm(i, states'); SP_rtl(rtlctrl, dpath_arcs) ]
                        (ans, dpath_nets)

                    | SP_rtl(ii, x) ->
                        let _ = vprintln 3 "Passing through an RTL statement unchanged"
                        ([A], [])
                    | SP_par(q, lst) ->
                        let rec fld (ans, nets0) = function
                            | [] -> (ans, nets0)
                            | h::t ->
                                    let (a, nets) = dpath_sp h
                                    fld (a @ ans, nets @ nets0) t
                        fld ([], []) lst
                    | SP_comment _ -> ([A], [])
                    | sp ->
                        vprintln 0 ("+++ dpath_vm other sp form not processed: " + hprSPSummaryToStr sp)
                        ([A], [])
                                
            let dpath_exec (cc, cd) = function
                | H2BLK(clkinfo, sp) -> 
                    let (ex, nets) = dpath_sp sp
                    (H2BLK(clkinfo, gen_SP_lockstep ex)::cc, nets @ cd)

            let (execs', newnets) = List.fold dpath_exec ([], []) execs
            let m = { g_null_db_metainfo with kind= "dpmap-glue" }
            (ii, Some(HPR_VM2(minfo, DB_group(m, map db_netwrap_null newnets) :: decls , sons', execs', assertions))
   ) 

// Entry point for this stage of the recipe
let opath_dpath ww0 (op_args:op_invoker_t) vms =
    let disabled = 1= cmdline_flagset_validate "dpgen" ["enable"; "disable" ] 0 op_args.c3
    if disabled then vms
    else
        let kK3:K3_t =
            {
                m= "mmmm";
            }
        map (opath_dpath_w ww0 kK3) vms


let dpgen_argpattern = 
  [
     Arg_enum_defaulting("dpgen", ["enable"; "disable"; ], "enable", "Disable this stage");
     Arg_enum_defaulting("dpgen-mode", dpath_modes, "full", "Select DP mode");
  ]


type asip_ins_t =
    {
        precond: hbexp_t;
        boolf:   (hexp_t * bool) list; // Opcode fields and default values
    }
//
// See if we can factorise a command into an existing instuction.

let newfield_b (cc:asip_ins_t) defv =
    let newfield = vectornet_w(funique "OF", 1)
    let cc' = { cc with boolf= (newfield, defv) :: cc.boolf }
    in (xi_orred(newfield), cc')

let rec unif_bexp cc (cmd, ins) = (ins, cc) // for now

and unif_exp cc (cmd, ins) =
    if cmd=ins then (ins, cc)
    
    else
    let _ = vprintln 3 ("unif_exp  cmd=" + xToStr cmd + " ins=" + xToStr ins)
    match (cmd, ins) with
        | (W_query(cg, ctt, cff, _), W_query(ig, itt, iff, _)) ->
           let (tt, cc) = unif_exp cc (ctt, itt)
           let (ff, cc) = unif_exp cc (cff, iff)
           let (gg, cc) = unif_bexp cc (cg, ig)
           let newi = xi_query(gg, tt, ff) // Hopefully this is commonly the same as ins.
           in (newi, cc)

        | (cother, W_query(ig, itt, iff, _)) when itt=cother || iff=cother-> // clause where instruction is more general
           let ig1 = if iff=cother then xi_not ig else ig
           let cc' = { cc with precond=xi_and(ig1, cc.precond) } // so can use this instruction but constrain the field.
           in (ins, cc')
           
        | (W_query(cg, ctt, cff, _), iother) when ctt=iother || cff=iother-> // instruction needs generalising ? todo if top level might become new ins?
           let (nf, cc') = newfield_b cc (ctt=iother)
           let newi = xi_query(nf, ctt, cff)
           in unif_exp cc' (cmd, newi)

        | (W_query(cg, ctt, cff, _), iother) ->        
           let (nf, cc') = newfield_b cc false
           let newi = xi_query(nf, iother, ins)
           in unif_exp cc' (cmd, newi)

        | (X_bnet _, ins)
        | (W_asubsc _, ins) ->
        // Do a load for all subscript expressions in soft pause mode?
            let (nf, cc') = newfield_b cc false
            let newi = xi_query(nf, cmd, ins)
            in unif_exp cc' (cmd, newi)

        | (_, ins) when fconstantp cmd ->
            let (nf, cc') = newfield_b cc false
            let newi = xi_query(nf, cmd, ins)
            in unif_exp cc' (cmd, newi)
                

        | (cother, ins) ->
            sf ("dpmap: no support for " + xkey cother + ": cother=" + xToStr cother)


let g_set = ref []

let unif_test exp =
    let _ = vprintln 0 ("auto asip generation " + xToStr exp)
    let update c old_ins = 
        let cc0 = { boolf=[]; precond=X_true; }
        let (ins, cc1) = unif_exp cc0 (exp, old_ins)
        let _ = vprintln 0 ("New ins= " + xToStr ins + " g=" + xbToStr cc1.precond)
        ins::c

    let _ = if !g_set = [] then g_set := [ exp ] else g_set := List.fold update [] (!g_set)

    let _ = reportx 0 "asip_set" xToStr (!g_set)
    ()
    
let install_dpath _ = install_operator ("dpgen",    "Data Path Generator", opath_dpath, [], [], dpgen_argpattern)


(* eof *)
