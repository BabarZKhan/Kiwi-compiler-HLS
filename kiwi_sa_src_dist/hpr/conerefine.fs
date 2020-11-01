(*
 *
 * conerefine.sml - Simplify by removing un-read variables and their supporting logic.
 *
// CBG Orangepath. HPR Logic Synthesis and Formal Codesign System.
// (C) 2007-17, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met: redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer;
// redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution;
// neither the name of the copyright holders nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 * It uses a 'cone of influence' algorithm to determine what must be kept to drive outputs and other side-effecting (EIS) operations.
 * A manual keep list enables other nets of interest to be added as targets so they are not trimmed.
 * 
 * Also this stage can potentially do inter-vm constant propagation.
 *
 * For parallel machines, we might assume that private nets of one machine cannot be read by others, and hence their usefulness is locally determinable.  But when this is run after compose in a recipe, there is only one machine anyway.
 *
 *
 * (C) 2003-16 DJ Greaves. Cambridge. CBG.
 * HPR L/S Project.
 *
 *
 *)

module conerefine

open Microsoft.FSharp.Collections
open System.Collections.Generic

open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open protocols


type coneref_control_t =
    {
        stagename:     string // May vary according to recipe instantiation
        toolname:      string // Always 'conerefine' 
        cr_verbosef:   bool   //
        manual_keeps:  string list
        rcs:           int option 
        control:       control_t
        vd:            int
    }


(*
 * Conerefine: remove redundant logic that does not drive any useful output.
 * pass one: generate trips and collect information in cr_info, such as subscript use.
 *
 * Triples (driven, code, support) can be closed by the dataflow functions. 
 *)
let trip_collect_xrtl ww msg msc cc xrtl_in =
    let _ = WF 3 "Cone refine" ww ("cone refine xrtl " + msg)
    let vd = - 1
    let tripgen1 a cc = 
        let rr = tripgen_x vd true [] a
        //vprintln (!cr_vd) (fun ()->sfoldcr tripToStr r)
        rr @ cc
    List.foldBack tripgen1 xrtl_in cc


(*
 * See if a buffer is generating a constant LHS.
 *)
let cc_consider cmd =
    match cmd with
        | (XIRTL_buf(X_true, l, rhs)) -> if constantp rhs then Some (l, rhs) else None
        | (cmd) -> None (* "Constant prop cannot yet consider " + xrtlToStr C + "\n") *)

        
(*
 * Cone refine snd pass xrtl: remap and remove redundant logic that does not drive any useful output.
 * pass two: removal and rewrite under constant mapping - although the constant prop found here is not used in conerefine.
 *)
let cone_refine_two_xrtl ww (K2:coneref_control_t) msg (targs, driven, cr_info, mm) rtl =
    let vd = K2.vd
    let ww = WF 2 "cone_refine_two_xrtl" ww "start batch" 
    let (driven, eis) = rtl_drivengen rtl
    let driven = map (support_to_sd_form ww) driven
    let ww = WF 2 "cone_refine_two_xrtl" ww "drivegen batch done" 
    let rtl_kept = eis || sd_intersection_pred driven targs
    if vd>=4 then vprintln 4 (sprintf "cone_refine_two_xrtl: keep=%A : %s" rtl_kept (xrtlToStr rtl))
    if rtl_kept then
        let ans = rewrite_rtl ww vd X_true mm rtl []
        if vd>=4 then vprintln 4 ("CR2 Rewritten arcs: " + sfold xrtlToStr ans)
        Some ans    
    else
        //if not rtl_kept then vprintln 0 (sprintf "dropped - driven=%A" driven)
        None



(*
 * Cone refine for arcs in FSM form:  (hexp_t * xrtl_t list) list 
 * To form our output we get the targets from the linearised form cone and then filter the
 * fsm form input.
 *)
let cone_refine_one_fsm ww m m_MSC cc (si:vliw_arc_t) = // Not used at the moment - fsm flattening is brutally applied.
    let _ = WF 3 "Cone refine fsm" ww ("start: " + m)
    let goto = muddy "goto"
    let flattened = goto :: si.cmds
    let trips = trip_collect_xrtl ww m m_MSC cc flattened
    // let _ = WF 3 "Cone refine fsm" ww ("targets=" + i2s(length(xi_valOf targets_final)))
    trips

let cone_refine_two_fsm ww (targets_final) m fsm = 
    let ww = WF 3 "Cone refine two fsm" ww ("start: " + m)
    let driven l = map (support_to_sd_form ww) (xi_driven [] l)
    let keep0_pred = function
        | XIRTL_pli _     -> true
        | XRTL(_, g, lst) ->
            let scn x = function
                | Rarc(g, l, r) -> x || sd_intersection_pred (driven l) targets_final 
                | Rpli _    -> true
                | _ -> sf "Cone Refine2_x other 0"
            List.fold scn false lst
        | _ -> sf "Cone Refine2_x other 1"
    let fsm' = map (fun (a,l) -> (a, List.filter keep0_pred l)) fsm
    fsm'


(*
 * This needs really to be split out on a per-parallel block basis
 *)
let tripgen_for_local_execs ww m (K2:coneref_control_t) m_MSC cc exec =
    let rec kq cc arg =
        match arg with
        | SP_fsm(fsm_info, arcs) -> // Note, this brutally discards the FSM factorisation, whereas it would be neater to keep it at this stage, even if the next recipe step might discard it.
            let code_track_point = (m + ":SP_fsm" + xToStr fsm_info.pc)
            let (xrtl, resets) = fsm_flatten_to_rtl ww code_track_point arg
            //vprintln 0 ("current impl is " + sfold xrtlToStr xrtl)
            // let _ = List.fold (fun c (x:vliw_arc_t) -> trip_collect_xrtl ww m m_MSC (x.cmds, c)) c arcs
            // (cone_refine_one_fsm ww m MSC) c states
            trip_collect_xrtl ww (m + " SP_fsm") m_MSC cc (resets @ (map snd xrtl))
        | SP_rtl(ii, rtl_list)   -> trip_collect_xrtl ww (m + sprintf " SP_rtl %i xfers" (length rtl_list)) m_MSC cc rtl_list
        | SP_comment x      -> cc
        | SP_seq(l)         -> List.fold kq cc l
        | SP_par(_, l )     -> List.fold kq cc l        
        | SP_dic(dic, ids)  ->
            (
                vprintln 1 "+++ conerefine: SP_dic ignored for tripgen - dont need trips if refine only";
                sf "conerefine: malformed recipe: cannot ignore DIC: try '-conerefine=disable' option";
                cc
            )
        | SP_l(ast_ctl, cmd) -> (hpr_yikes "+++conerefine: SP_l effect on cone is ignored at the moment."; cc) // SP_l is not cone refined at the moment - TODO extend conerefine to work not just with XRTL
        | other -> sf("cone_refine_one other form: " + hprSPSummaryToStr other)

    let jj cc = function
        | H2BLK(clkinfo, s) -> kq cc s
        //| other -> sf ("cone_refine_one j other")

    jj cc exec


let mdpToStr (aid, md) =  htos aid + ": " + mdToStr md

// Scan a decls tree making a list of those flagged as notrim.
let notrim_decls_flatten cc db =
    let rec notrim_scan flag cc arg =
        match arg with
            | DB_group(meta, lst) ->
                let flag = flag || meta.not_to_be_trimmed
                List.fold (notrim_scan flag) cc lst
            | DB_leaf(Some kk, _)
            | DB_leaf(_, Some kk) ->
                match kk with // Parameter overrides with numeric values do not need their constant values to be noted.
                    | X_num _
                    | X_bnum _
                    | X_undef -> cc
                    | kk ->
                        //if flag then dev_println (sprintf "WOX adding notrim leaf %A" kk)
                        if flag then kk :: cc else cc
    List.fold (notrim_scan false) cc db


(*
 * First pass for a tree of VMs. Get flat list of trips and minfo ats.
 *)
let rec cone_refine_one_vm ww K2 m_MSC (cr_info) cc0 = function
    | (ii, None) -> cc0
    | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
        let m = (" id=" + vlnvToStr ii.vlnv)
        let ww' = WF 3 "cone_refine_one_vm" ww m 
        let (c', a', notrims) = List.fold (cone_refine_one_vm ww' K2 m_MSC cr_info) cc0 sons
        let trips = List.fold (tripgen_for_local_execs ww' m K2 m_MSC) c' execs
        let _ = unwhere ww
        let notrims = notrim_decls_flatten notrims decls
        (trips, minfo.atts @ a', notrims)
    


(*
 * In the second pass we rewrite each VM according to the mapping arrived at in the first pass.   
 * Second pass (sometimes used on its own without first pass): filter, inter-thread const propagate.
 *)
let cone_refine_two_dic ww K2 msg (targs, driven, cr_info, mm) dir dic =
    let vd = 2
    let ww = WF 3 "cone_refine_two_dic" ww "foo" 
    let rr = rewrite_h2sp (WN "rewrite dic" ww) vd dir X_true mm dic
    rr


let rec cone_refine_two_vm ww K2 info retained_nets vm =
    match vm with
        | (ii, None) -> (ii, None)
        | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
            let vm_name = vlnvToStr ii.vlnv
            let m = " id=" + vm_name
            let ww = WF 3 "cone_refine_two_vm" ww m 
            let cr2 x = cone_refine_two_xrtl ww K2 m info x

            let rec filter f = function // todo use List.filter
                | [] -> []
                | h::t -> let r = f h in if r=None then filter f t else (valOf r)@(filter f t)

            let rec kr dir arg = // TODO should use the sp_walker for this now! (todo)
                match arg with
                | SP_fsm(fsm_info, states) ->
                    let statetrim (si:vliw_arc_t) =
                        { si with
                            cmds = filter cr2 si.cmds
                        }
                    SP_fsm(fsm_info, map statetrim states)
                | SP_l(ast_ctl, cmd) -> SP_l (ast_ctl, xbev_rewrite (WN "SP_l" ww) (f4o4 info) cmd)
                | SP_rtl(ii, x) -> 
                    let pre_len = length x
                    let r = delete_nones(map (cone_refine_two_xrtl ww K2 m info) x)
                    let post_len = length x
                    vprintln 3 (sprintf "   SP_rtl %s: assignment count/length: old=%i, new=%i" ii.id pre_len post_len)
                    if r<>[] then gen_SP_lockstep (map (fun x->SP_rtl(ii, x)) r) 
                             else SP_comment "" 
                | SP_comment s -> arg
                | SP_seq(l)          -> SP_seq (map (kr dir) l)
                | SP_par(pstyle, l)  -> gen_SP_par pstyle (map (kr dir) l)        
                | SP_dic _           -> cone_refine_two_dic ww K2 m info dir arg
                | other -> sf("cone_refine_two SP other: " + hprSPSummaryToStr other)


            let execs' =
                let jj = function
                    | (H2BLK(dir, s)) -> H2BLK(dir, kr dir s)
                    //| other -> sf ("cone_refine_two j other")
                map jj execs

            let ww' = WN "cone_refine_two_vm" ww
            let sons' = map (cone_refine_two_vm ww' K2 info retained_nets) sons // recursive call on sons.

            let av = map snd (List.foldBack (db_flatten []) decls [])
            let ldecls_new() = xi_valOf(xi_intersection(xiSort av, retained_nets))  

            let decls' = decls_trim false (xi_valOf retained_nets) decls

            let _ =
                if K2.cr_verbosef then
                    (   //reportx 1 ("local nets pre filter for VM " + vm_name)            netToStr ldecls
                        //reportx 1 ("local nets post filter for VM " + vm_name)           netToStr (ldecls_new())
                        reportx 1 ("lhs of intersection (local set) for VM " + vm_name)  netToStr av
                        reportx 1 ("rhs of intersection (retained) for VM " + vm_name)   netToStr (xi_valOf retained_nets)
                    )

            // TODO: this is not finished! ? Really - its worked for years!
            //let ldecls'' = list_intersection(av, xi_valOf retained_nets)  
            //if K2.cr_verbosef then reportx 1 (sprintf "local nets post filter2 for VM %s" vm_name) (fun x-> xToStr x + ":" + xToStr x) ldecls''

            //reportx 0 "Gdecls cr2" xToStr gdecls
            //reportx 0 "Ldecls cr2" xToStr ldecls' 
            let minfo' = { minfo with memdescs=[] }
            (ii, Some(HPR_VM2(minfo', decls', sons', execs', assertions)))
(*
 * Third pass: does inter-thread constant prop: TODO catch new constants from inside dic
 *)
let cone_refine_three_dic ww K2 msg (targs, driven, cr_info) SS dic =
    let (consts, dmmo) = SS 
    let (consts', r) = muddy "eval_h2sp (WN \"eval_h2sp dic\" ww) consts dic"
    let SS' = (consts', if consts' = consts then dmmo else None)
    (SS', r)


let cone_refine_three_xrtl ww K2 msg (targs, driven, cr_info) (consts, dmmo) a =
        let vd = 3
        let dmm =
            match dmmo with
            | None -> makemap consts
            | _ -> valOf dmmo
        let ans = rewrite_rtl ww vd X_true dmm a []
        let co = cc_consider a
        if co = None then ((consts, Some dmm), ans)
        else (((valOf co)::delete_assoc (fst(valOf co)) consts, None), ans)


(*
 * Apply constant propagation based on the output of a previous thread in the static schedule
 *)
let rec cone_refine_three_vm ww  K2 m_MSC info SS vM =
    match vM with
    | (_, None) -> (vM, SS)
    | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
        let m = (" id=" + vlnvToStr ii.vlnv)
        let ww = WF 3 "cone_refine_three_vm" ww m 
        let consts = ref []
(* TODO: whether consts is fresh or shared depends on the type of parallel composition of behaviours: we are following a unicore static sequence for now... *)
        let cr3 SS x = cone_refine_three_xrtl ww K2 m info SS x

        let rec fil f SS sf = function
            | [] -> (rev sf) (* very silly not needed fun *)
            | (h::t) -> 
                let (SS', r) = f h 
                in fil f SS' (r@sf) t

        // No new updates in SS in fsm - but we need to delete changed entries !
        let rec k SS sarg =
            match sarg with
            | SP_fsm(fsm_info, states) -> 
                  let statefix (si:vliw_arc_t) =
                      { si with
                          cmds = fil (cr3 SS) SS [] si.cmds
                      } 
                  (SS, SP_fsm(fsm_info, map statefix states))

            | SP_rtl(ii, lst) -> 
                  let rec q SS sf = function
                      | [] -> (SS, rev sf)
                      | (h::t) -> 
                          let (SS', h') = cone_refine_three_xrtl ww K2 m info SS h
                          q SS' (h' :: sf) t
                  let (SS', ans)  = q SS [] lst
                  let rr = if ans<>[] then gen_SP_lockstep (map (fun x->SP_rtl(ii, x)) ans) else SP_comment "" 
                  (SS', rr)

            | SP_comment s -> (SS, sarg)

            | SP_seq(lst) -> 
                let rec q SS sf = function
                    | [] -> (SS, rev sf)
                    | (h::t) -> let (SS', h') = k SS h in q SS' (h'::sf) t
                let (SS', ans) = q SS [] lst
                (SS', SP_seq ans)

            | SP_par(pstyle, lst) -> 
                let rec q SS sf = function
                    | [] -> (SS, rev sf)
                    | (h::t) -> let (SS', h') = k SS h in q SS' (h'::sf) t
                let (SS', ans) = q SS [] lst
                (SS', gen_SP_par pstyle ans)

            | SP_dic _ -> cone_refine_three_dic ww K2 m info SS sarg

            | other -> sf("cone_refine_two SP other: " + hprSPSummaryToStr other)

        let j SS = function
            | (H2BLK(clkinfo, s)) -> 
                let (SS', ans) = k SS s 
                (SS', H2BLK(clkinfo, ans))
            //| other -> sf ("cone_refine_two j other")

        let (SS', execs') = 
            let rec q SS sf = function
                | [] -> (SS, rev sf)
                | (h::t) ->
                     let (SS', z) = j SS h
                     q SS' (z::sf) t
            q SS [] execs

        let trips = List.fold (tripgen_for_local_execs (WN "const prop trip get" ww) m K2 m_MSC) [] execs'
        let trips = map (trip_x_to_sd_form ww) trips
        let ww' = WN "cone_refine_three_vm" ww
        // We can retain the constants from the previous thread that were not updated by this thread.
        let rec canuse_old x = function
            | [] -> true
            | ((d, c, s, eis)::t) -> if sd_intersection_pred x d then false else canuse_old x t
        let retained = List.filter (fun (l, r) -> canuse_old [(l, [])] trips) (fst SS)
        vprintln 3 (sprintf "Retained %i out of %i " (length retained) (length(fst SS)) + " constants and have " + i2s(length(!consts)) + " new ones.")

        let consts1 = retained @ (!consts)
        if consts1 = [] then () else reportx 0 ("Inter-thread constant propagate: constants now active") (fun (l,r)->xToStr l + ":= " + xToStr r) (consts1)

        let rec walksons = function
            | ([], ss, cc) -> (cc, ss)
            | (h::tt, ss, cc) -> 
                let (vm, ss) = cone_refine_three_vm ww' K2 m_MSC info ss h
                walksons(tt, ss, vm::cc)

        let (sons', SS_out) = walksons(sons, SS', []) 
        let vm' = (ii, Some(HPR_VM2(minfo, decls, sons', execs', assertions)))
        (vm', SS_out)



// We process a trip. If it has no support then the value it generates forms a new constant pair,
// passing it through to the remainder list otherwise.
// oscs is untouched.
let constant_scan trip (pairs, oscs, remainder) =
    match trip with
        | (driven, cmd, [], false) ->  // A command with no support that is not EIS is a candidate.
            let a = cc_consider cmd
            if nonep a then (pairs, oscs, trip::remainder)
            else
                match op_assoc (fst(valOf a)) pairs with
                    | None -> ((valOf a)::pairs, oscs, remainder)
                    | Some ov ->
                         if ov = snd(valOf a) then (pairs, oscs, remainder) // Delete duplicate 
                         else muddy "found an osc : No need to see if driven to another value by ANY trip" 
        | (D, C, S, eis) -> (pairs, oscs, trip::remainder)

(*
 * Trip format=DCSe = (hexp_t * hexp_t list) list * xrtl_t * (hexp_t * hexp_t list) list * bool)
 *
 * A trip with no support generates a constant output.
 *
 * Some trips are denoted as EIS = end-in-self.  These are side effecting and must intrinsically be kept.
 *
 * Operations on program counter variables may also be classed as EIS by some code for ease of debugging and to preserve FSM-looking output format.
 *
 * The first pass involves flattening and global propagation.
 * The second pass rewrites the initial input while preserving its structure.
 * Hence we do not return the rewritten execs from this pass.
 *
 * Certain trips have constant outputs, and we can propage this to the inputs of others, perhaps making
 * them constant.  Repeat until closure.  DOING THIS ACROSS THREADS MAY REVEAL THEY ARE NOT CONSTANT : eg inter-thread handshaking variables (busy/full flags) TODO example please.  The compose recipe stage should handle this properly instead of doing it here.
 *)
let const_propagate_one ww K2 (input:trip_t list) = 
    let vd = 3
    let _:((hexp_t * int64 list) list * xrtl_t * (hexp_t * int64 list) list * bool) list = input // Input is a trip_t list
    let ww = WF 3 "Cone refine constant propagate" ww ("starting")
    let q(ss, _) = if vd>=3 then vprintln 3 (sprintf "Const prop %s const=%A" (xToStr ss) (constantp ss))

    // We need to iterate to progress constants through layers of logic.
    let rec iterate pass raw const_pairs = 
        let (new_pairs, oscs_, remainder) = List.foldBack constant_scan raw ([], [], []) (* FoldBack preserves order *)
        if vd>=5 then reportx 5 ("Constant propagate: new constants in pass " + i2s pass) (fun (l,r)->xToStr l + ":= " + xToStr r) new_pairs
        let total_pairs = new_pairs @ const_pairs
        let mm = makemap total_pairs
        let decon (driven, cmd, support, eisf) = cmd
        let rewritten = List.foldBack (rewrite_rtl ww vd X_true mm) (map decon remainder) []
        let l1 = length remainder
        let l2 = length rewritten
        if l1<>l2 then vprintln 3 (sprintf "Constant propgate reduced number of statements from %i to %i with %i total constants." l1 l2 (length total_pairs))
        if nullp new_pairs then total_pairs
        else
            let trips = List.fold (tripgen_x 0 true) [] rewritten
            iterate (pass+1) (map (trip_x_to_sd_form ww) trips) total_pairs

    let const_pairs = iterate 0 input []
    let _  = WF 3 "Cone refine constant propagate" ww ("now finished")
    const_pairs





(*
 * Form the cone of influence and return the nets needed to implement it.
 * There are multiple cones infact, one for each target, although some may be totally inside others.
 *)
let cone_form ww K2 notrims concourse_trips = 
    let vd = K2.vd
    let manual_keeps = K2.manual_keeps
    vprintln 2 ("Cone refine manual retain for " + (if nullp manual_keeps then "no nets" else sfold (fun x->x) manual_keeps))

    if vd>=4 then vprintln 4 ("Cone refine notrim tips are " + (if nullp notrims then "no nets" else sfold (xToStr) notrims))
    let manual_keeps =
        let xToStr_mean = function
            | X_bnet ff -> ff.id
            | X_net(id, _) -> id
            | other ->
                dev_println (sprintf "Other form notrim net in conerefine key=%s it=%s" (xkey other) (netToStr other))
                xToStr other
        map xToStr_mean notrims @ manual_keeps
    let rec cone_form_sweep = function
        | (targets, [], r, leftover) -> (targets, r, leftover)
        | (targets, trip::tt, r, leftover) -> 
            let (d,c,s, eis) = trip
            // Basic rule: if this trip drives anything in our target set then keep the trip and augment our target set with its support.
            let needed = if eis then [gec_X_net "EIS"] else sd_intersection_example d targets
            let already = memberp trip r 
            if needed<>[] && (not already) then
                if vd>=4 then
                             (
                              vprintln 4 ("Need trip that drives '" + sfold isdToStr d + "',  because='" + (if eis then "EIS" else " of net " + netToStr(hd needed)) + "' already=" + boolToStr already);
                              vprintln 4 ("It's support is " + sfold isdToStr (s) + "\n")
                             )
                            elif vd>=4 then vprintln 4 ("Don't need trip : " + tripToStr trip + " already=" + boolToStr already)
              
                cone_form_sweep(sd_union(s, targets), tt, trip::r, leftover)
            else cone_form_sweep(targets, tt, r, trip::leftover)
 

    // Iterative cone form.
    let rec rtl_cone_form(targets, sofar, conc) = 
        let _ = WF 3 "Cone refine" ww ("pre iterate   : "+ i2s(length targets) + ":" + i2s(length sofar) + ":" + i2s (length conc))
        let (targets', sofar', leftover) = cone_form_sweep(targets, conc, sofar, [])
        let _ = WF 3 "Cone refine" ww ("post iterate  : "+ i2s(length targets')  + ":" + i2s(length sofar') + ":" + i2s (length leftover))
        if length(targets') = length (targets) && length sofar' = length sofar
            then (targets', sofar')
            else rtl_cone_form(targets' , sofar', leftover)

    let driven = sd_Union(map f1o4 concourse_trips)
    let _ = WF 2 "Cone refine" ww (sprintf "driven done, dnet count=%i" (length driven))

    let full_sd(it, lanes) = (it, []) // Discard lane/subscript infomation.

    let vmemberp f a keeps =
        let r = memberp a keeps 
        if vd>=5 then vprintln 5 ("Manual keep " + a +  " nn=" + i2s(x2nn f) + " nn=" + i2s(x2nn f) + " was " + boolToStr r)
        r 

    let targets =
        let tpred1 = function
            | X_bnet ff ->
                let f2 = lookup_net2 ff.n
                isio f2.xnet_io 
                              || at_assoc g_control_net_marked f2.ats <> None   // This attribute marks an output --- do not trim  - please improve definition and infer from do_not_trim.
                              || at_assoc "io_input" f2.ats <> None             // Also inputs - but will not find these normally in driven list.
                              || vmemberp (X_bnet ff) ff.id manual_keeps
            | other -> (lprintln vd (fun()->"do not need target " + netToStr other); false)  
        let tpred_sd(x, lanes_) =
            let ans = tpred1 x
            if vd>=5 then vprintln 5 (sprintf "keep target predicate on %s returns %A" (netToStr x) ans)
            ans
        map full_sd (List.filter tpred_sd driven)

    //    (* Trips for eis calls are retained anyway, so we do not need them in targets. *)
    if nullp targets then vprintln 1 ("+++ Cone refine has no targets: entire design may be trimmed away (unless contains EIS-designated items).") else reportx 3 "Target nets at tips of cones" isdToStr targets
    let _ = WF 2 "Cone refine" ww ("cone form starting")
    let (targets_final, cone)  = rtl_cone_form(sdSort targets, [], concourse_trips)


    // These pairs are actually tie-offs from one thread's output, seen by another, and but now not changed again since we detect oscs.
    // Constant propagation should be in another recipe stage, such as compose: please delete from here.
    let const_pairs = const_propagate_one (WN "const propagate first pass" ww) K2 cone

    let _ = WF 2 "Cone refine" ww (sprintf "cr1: cone form done: target_final count=%i" (length(targets_final)) + " const_pairs=" + i2s(length(const_pairs)))
    //reportx 0 "final cone" tripToStr cone
    let tvl = map (fun (d,c,s,eis)->d) (list_once(list_subtract(concourse_trips, cone)))
    let ret = map (fun (d,c,s,eis)->sd_union(d,s)) cone

    let de_sd arg cc =
        match arg with
            | (it, [])     -> it::cc 
            | (it, lanes)  -> if isarray it then List.foldBack (fun i c -> (ix_asubsc it (xi_num64 i))::c) lanes (it::cc) else it::cc
    //reportx 0 "Retained parts" (fun x->sfold sdToStr  x) ret 
    let retained_nets = List.foldBack de_sd (sd_Union ret) []
    let trimmed_nets = sd_Union tvl
    let _ =
        reportx 3 "Trimmed Nets/Variables" isdToStr trimmed_nets
        reportx 3 "Retained Nets/Variables" xToStr retained_nets
        ()

    let f = (fun (l,r)->xToStr l + ":= " + xToStr r)
    reportx 3 "Constant Pairs" f const_pairs
    (retained_nets, targets_final, const_pairs)



(*
 * Implement two (main) passes. First collects info. Then global analysis. Second rewrites just retaining needed logic.
 *
 * Now, the scalarised versions of the array locations are not updated in the original trip list
 * so how do we know which ones we need in the final net list ?  We apply xi_rewrite to the original list.
 *
 * Also, we want to know the
 * index range for the ones that are kept as arrays...
 *
 *)


//
//   
let cr_walker_gen vd (cr_info) = 
    let vdp = false
    let isoffset a = strlen(a) >= 7 && a.[0..6] = "$offset" // copy 1/2
    let isfield  a = if strlen(a) >= 8 && a.[0..7] = "$offset+" then Some(a.[8..]) else None // copy 1/2
    let unitfn arg = ()
    (*
        match arg with
            | X_pair(W_string(s, _, _), r, _) when constantp r ->
                ()
            | W_asubsc(l, r, _) -> (ignore(array_op_logger cr_info arg); ())
            | _ -> ()
*)
    let lfun strict clkinfo arg rhs =
        match arg with
            //| W_asubsc(l, r, _) -> (ignore(array_op_logger cr_info arg); ())
            | _ -> ()

    let null_sonchange _ _ nn (a,b) = b
    let opfun arg N bo xo a b = a 
    let (_, sWALK) = new_walker vd vdp (true, opfun, (fun (_) -> ()), lfun, unitfn, null_sonchange, null_sonchange)
    sWALK
   

(*
 * Cone refine. Delete logic that does not contribute to a backwards cone originating from required outputs.
 *)
let cone_refine_top ww0 (k2:coneref_control_t) vm =
    let vd = k2.vd
    let crm = k2.stagename
    //let memdesc_map = new memdesc_map_t()
    let cr_info = "unused"
    let m_MSC = ("conerefiner", cr_walker_gen vd cr_info)

    //-------------------------FIRST PASS--------------------
    let ww = WF 2 crm ww0 ("Start first pass.   T/S " + timestamp true)
    let cc0 =  ([], [], [])
    let (concourse_trips, flatats, notrims) = cone_refine_one_vm ww k2 m_MSC cr_info cc0 vm
    let ww = WF 2 crm ww0 ("First pass done.   T/S " + timestamp true)
    let ww = WF 2 crm ww0 (sprintf "concourse trip count=%i" (length concourse_trips))
    let concourse_trips = map (trip_x_to_sd_form ww) concourse_trips
    if k2.cr_verbosef then reportx 1 "Concourse trips" tripToStr concourse_trips

    let (retained_nets, targets_final, const_pairs) = cone_form ww k2 notrims concourse_trips
    let mm0 = makemap1 []
    let const_pairs = [] // for now ... constant and volatile are mixed up
    let augment_mapping c (L, R) = makemap1_add c (gen_AR(L, R))
    let mfinal = List.fold augment_mapping mm0 const_pairs
    let mm = map_transclose(mfinal)
    let _ = WF 2  crm ww0 ("finished transclose.   T/S  "  + timestamp true)
    let retained_nets' = List.fold (xi_net_regen mm) [] retained_nets
    reportx 3 "Retained Variables Recoded" xToStr (retained_nets')


    //-------------------------SECOND PASS--------------------
    // In the second pass we rewrite each VM according to the mapping arrived at in the first pass.
    let ww = WF 2 crm ww0 ("Start second pass.   T/S  "  + timestamp true)
    let ans = cone_refine_two_vm ww k2 (targets_final, concourse_trips, cr_info, mm) (List.fold ix_add (Xist []) retained_nets') vm
    let ww = WF 2 crm ww0 ("Finish second pass.   T/S  "  + timestamp true)

    //-------------------------THIRD (currently unused) PASS--------------------        
    
    let do3__ ans = // currently unused
        let info3 = (targets_final, concourse_trips, cr_info)
        let rec closure3 n (ans, c) =
            if n=0 then ans
            else closure3 (n-1) (cone_refine_three_vm ww  k2 m_MSC info3 c ans)
        closure3 3 (ans, ([], None))

    let ans' = 
        let _ = WF 2  crm ww0 ("completed pass 2, (no pass 3 requested).   T/S  " + timestamp true) |> ignore;
        ans
    let _ = WF 2 crm ww0 "completed all"
    ans'

let cone_args =
        [
          Arg_string_list("conerefine-keep", "List of net names for cone refine to retain. Terminate with --");
          Arg_int_defaulting("conerefine-loglevel", 3, "Log file verbosity setting (0..10, higher value for more output)");
          Arg_enum_defaulting("conerefine", ["enable"; "disable"], "enable", "Enable control for this operation");
        ]


//
// This is the callback invoked by the orangepath recipe.
//
let opath_conerefine ww op_args vms =
    let c3 = op_args.c3
    let toolname = "conerefine"
    let manual_keeps = control_get_strings  c3 "conerefine-keep" 
    let refine =      (control_get_s toolname c3 "conerefine" None) = "enable" // replace this with a generic opath callback that checks both stagename and toolname are enabled. please. TODO.
    let vd = max !g_global_vd (control_get_i c3 "conerefine-loglevel" 3)
    let cr_verbosef = vd >= 6
    vprintln 2 (sprintf "conerefine: vd=%i   verbosef=%A" vd cr_verbosef)
    let k2:coneref_control_t =
             {
               toolname=      toolname
               stagename=     op_args.stagename
               vd=            vd
               cr_verbosef=   cr_verbosef
               manual_keeps=  manual_keeps
               control=       c3
               rcs=           None
             }

    if not refine then vprintln 1 "Cone Refine is disabled."
    if (refine) then map (cone_refine_top ww k2) vms else vms



let install_coneref() =
    install_operator ("conerefine",  "Cone Refine", opath_conerefine, [], [], cone_args)
    0


(* eof *)
