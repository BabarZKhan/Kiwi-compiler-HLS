// (C) 2012-15 DJ Greaves.
// $Id: compose.fs $
//
// Combine separate FSMs together and reorganise ready for RTL generation.
//
// Communicating FSMs are a nice abstraction but potentially inefficient for implementation.  Here we spot composable
// machines and conglomorate them.
//
//   DJG has a draft paper about this - describing guiding metrics and deadlock avoidance checks.
//
   
module compose

let compose_banner = "d320 $Id: compose.fs,v 1.10 2013-07-08 08:32:14 djg11 Exp $ "


(*
 *
 * compose.fs : combine RTL blocks ? or FSMs ?
 *
 * (C) 2003-12 DJ Greaves. Cambridge. CBG.
 *
 * HPR L/S Core component.  
 *
 *)

open Microsoft.FSharp.Collections



open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open linepoint_hdr
open dpmap // for now





// Control for env_rpt
let g_trace_enable = ref true // normally off: in future settable with magic print statements and/or recipe.

let g_compose_loglevel = ref 3 // Verbosity:  Set this to 3 for normal operation.


type compose_elab_t = 
      {
        kK:              bevelab.bevelab_budgeter_t
      }



type compose_S1_t =
    {
        vd:              int
        bisimulate:      bool       // Share states that are observably equivalent
        msg:             string
        scheduler:       string
        ubudget:         int
        ce:              compose_elab_t
    }


// Callgraph
// opath_bevelab:
// |->naive_sequence_vliw or other sequencer or rebal 
//    |->bevToXrtl 
//       |->work/bevbev2a

    
(* ---------------------------------------------------------*)
// 
// This is a cv1-style routine -- is it ? : best avoid! - cv1 fails on assigns to outer guards 
//
// Call bevbev2a to combine in one clock cycle the effects of several quads.
// If from same thread, can perform 'project through next state function' ? No - they are in parallel within a block and you'd need two instances of that thread to do this... would halve the number of cycles though!
//
// Convert rv form? to xrtl
//
let bevToXrtl ww kK (bev:hbev_t) =  // old now - copied out in rendez below...
    let ww = WF 3  "bevToXrtl" ww "start"
    let vd = -1 // -1 for off
    //let (msg, control, c2) = K2

    let work e = function
          | Xskip -> e
          | Xcomment _ -> e
          | x -> bevelab.bevbev2a ww kK e (bevelab.nextgen()) x
    let ans = work (bevelab.em_gen (fun _ -> "bevToXrtl start") [] ([], [])) bev

    let cvt2rtl(es, ea, mm_, sef, costvec_) = // Convert result-vector form? to xrtl.
        let esf = function
              | (an, l, None, g, r, _, _) -> gen_XRTL_arc(None, g, l, r)
              | (an, l, _,    g, r, _, _) -> muddy "esf had bitinsert"

        let eaf1 = function
              | (an, l, Some s, g, r, _, _) -> gen_XRTL_arc(None, g, ix_asubsc l s, r)
              | (an, l, None,   g, r, _, _) -> muddy "eaf1 had no subscript"

        let eaf ((A, updates), c) = map eaf1 updates @ c

        if vd>=5 then vprintln 5 ("bevToXrtl: updates es=" + i2s(length es) + " ea=" + i2s(length ea) + " sef=" + i2s(length sef) + " items")
        let sef2rtl (tag, pli) = pli
        map esf es @ foldl eaf [] ea @ map sef2rtl sef 
    let ii = { id="from_bevToXtl" } : rtl_ctrl_t
    let code = SP_rtl(ii, cvt2rtl ans)
    let nets = [] // always empty!!!!!!!!!! could some have been made?
    (code, nets)

let synch_rendez ww kK (i1: fsm_info_t, a1:vliw_arc_t) (i2: fsm_info_t, a2:vliw_arc_t) =
    let vd = !g_compose_loglevel
    let m0 = "synch_rendez " + xToStr i1.pc + " " + xToStr i2.pc
    let e0 = bevelab.em_gen (fun _ -> m0) [] ([], []) 

    let apply2 e (a:vliw_arc_t) = 
        let r2bev ng cmd c = // convert to bev. TODO: this is done in abstracte and diosim! so don't repeat here!
              match cmd with
               | XRTL(None, g, lst) -> // This is a pblock (parallel block)
                   let q = function
                       | Rarc(gc, l, r) -> gec_Xif1 (xi_and(xi_and(gc, ng), g)) (Xassign(l, r))
                       | Rpli(gc, (fgis, ord), args) -> gec_Xif1(xi_and(xi_and(gc, ng), g)) (gec_Xcall(fgis, (args)))
                       | Rnop s -> Xcomment s 
                   (map q lst) @ c
               | XIRTL_pli(None, g, (fgis, ord), args) -> gec_Xif1(xi_and(ng, g)) (gec_Xcall(fgis, args)) :: c
               | _ -> sf "r2bev other: nonsence to have xrtl here anyway"
        let parbev = gen_Xpblock (List.foldBack (r2bev X_true) a.cmds [])
        let work e = function
            | Xskip -> e
            | Xcomment _ -> e
            | x -> bevelab.bevbev2a ww kK e (bevelab.nextgen()) x
        work e parbev
    let e1 = apply2 e0 a1
    let e2 = apply2 e1 a2
    let cprint m (es, ea, mm_, sef, costvec) =
        if vd>=4 then vprintln 4 (m0 + m + costToStr costvec + " :-: updates es=" + i2s(length es) + " ea=" + i2s(length ea) + " sef=" + i2s(length sef))
        ()
    cprint "e0 had cost " e0
    cprint "e1 had cost " e1
    cprint "e2 had cost " e2
    let cvt2cmds(es, ea, mm_, sef, costvec_) = // Convert result-vector form? to xrtl.
        let esf = function
              | (an, l, None, g, r, _, _) ->
                  let i = get_init "compose" l
                  //if i<>None && constantp r && valOf i = xi_manifest_int "compose"  r then vprintln 0 ("Reset to start val " + xToStr l)
                  gen_XRTL_arc(None, g, l, r)
              | (an, l, _,    g, r, _, _) -> muddy "esf had bitinsert"

        let eaf1 = function
              | (an, l, Some s, g, r, _, _) -> 
                  let i = get_init "compose" l
                  if i<>None && constantp r && valOf i = xi_manifest_int "compose"  r then vprintln 0 ("Reset array to start val " + xToStr l)
                  gen_XRTL_arc(None, g, ix_asubsc l s, r)

              | (an, l, None,   g, r, _, _) -> muddy "eaf1 had no subscript"

        let eaf ((A, updates), c) = map eaf1 updates @ c

        vprintln 5 ("cvt2rtl: updates es=" + i2s(length es) + " ea=" + i2s(length ea) + " sef=" + i2s(length sef) + " items")
        let sef2rtl (tag, pli) = pli
        map esf es @ foldl eaf [] ea @ map sef2rtl sef 
    let ans = cvt2cmds e2
    ()

    
//
//
let naive_sequence_vliw ww cS1 (directorate, vliws) = 
    let vd = 3     
    let oldsched = cS1.scheduler = "oldsched"
    let what = if oldsched then "oldscheduler" else "naive seqeuence scheduler"
    let ww = WF 3  what ww "start"
    let machines = length vliws
    let tidreg = vectornet_w("kiwitid", 32) // bound_log2(System.Numerics.BigInteger machines)
    let max = xgen_num(machines - 1)
    //let (msg, control, c2) = K2

   
    let reshed = Xassign(tidreg, if (machines = 1) then ix_minus (xi_num 1) tidreg else ix_query (xgen_deqd(tidreg, max)) (X_num 0) (ix_plus tidreg (xi_num 1)))
    let rec kk n = function
          | [] -> []
          | (SP_fsm(info, actions))::tt -> 
             let pc=info.pc
             let rst=info.start_state
             let m = "naive_seq: pc=" + xToStr pc + " resumes=" + i2s(length info.resumes) + " actions=" + i2s(length actions)
             let _ = WF 3 "naive_seq" ww m

             let pguard g =
                 let econ = enum_add "COMP1772" tidreg "COMP1772" n 
                 xi_and(g, xi_deqd(tidreg, econ)) //use enum for tidreg too!

             let bi_action (resumept, hardf) (qq:vliw_arc_t) =
                 let andtid ng = ix_and qq.gtop (pguard ng)
                 let pp = Some(qq.pc, qq.resume) 
                 let lcm_atoi = xi_manifest
                 let adv = if lcm_atoi "adv" qq.dest < 0 (*|| pc=dest *)then [] else [ gen_XRTL_arc(pp, andtid X_true, qq.pc, qq.dest) ]
                 let reg = function // regenerate will simplify based on full knowledge of pc state space
                     | XRTL(None, ga, lst) -> XRTL(pp, ga, lst)
                     | XIRTL_pli(None, g, fgis, args) -> gen_XRTL_pli(pp, andtid g, fgis, args)
                     |_ -> sf "reg other"
                 let nn=if resumept=qq.resume then (adv @ map reg qq.cmds) else []
                 //reportx 0 "bi_actions" xrtlToStr nn
                 nn

             let install_enum (pt, _) =
                 let dest_lab = enum_add "COMP1788" pc "COMP1788" (xi_manifest "COMP1788" pt) 
                 ()
             app install_enum (info.start_state :: info.resumes)
             let _ = close_enum "bevelab-L1796" pc
             
             let onethrd(resumept, cc) =
                  let pops = map (bi_action resumept) actions (* These are put in 'some' static priority, on the basis their guards are disjoint: but perhaps...  *)
                  list_flatten pops @ cc

             let wholethread = foldl onethrd [] info.resumes
             let other_threads = kk (n+1) tt

             let r00 = wholethread @ other_threads
             //vprintln g_fsmvd ("Ans so far is " + hbevToStr r00)
             r00

    let solo_machine = function
        | SP_fsm(info, actions) ->
            let m = "solo machine pc = " + xToStr info.pc
            let _ = WF 3 "bevelab" ww m
            let install_enum (pt, _) =
                 let dest_lab = enum_add "COMP1811" info.pc "COMP1811" (xi_manifest "COMP1811" pt)                 
                 ()
            app install_enum (info.start_state :: info.resumes)

            let onethrd (resumept, hardf) =
                 let bi_action (q:vliw_arc_t) =
                     let pp = Some(q.pc, q.resume)
                     let topgrd ng = ix_and q.gtop ng 
                     let lcm_atoi = xi_manifest "adv"
                     let adv =
                               if q.dest=X_undef || lcm_atoi q.dest < 0 || q.dest = q.resume
                               then []
                               else [ gen_XRTL_arc(pp, topgrd X_true, q.pc, q.dest) ]
                     let xc = function
                         | XRTL_nop ss -> XRTL_nop ss
                         | XRTL(None, g, lst) -> XRTL(pp, topgrd g, lst)
                         | XIRTL_pli(None, g, fgis, args) -> gen_XRTL_pli(pp, topgrd g, fgis, args)

                         //| XIRTL_buf 
                         | other -> sf (sprintf "xc other form: (nonsense to have xrtl here anyway?) other=%A " other)
                     if resumept=q.resume then adv @ map xc q.cmds else []
                 let pops = list_flatten(map bi_action actions)
                 let _ = reportx 3 ("solo_machine (pc=" + xToStr info.pc + " resumept=" + xToStr resumept + "): bi_action pops") xrtlToStr pops
                 pops 
            let rtl = list_flatten(map onethrd info.resumes)
            (rtl, Xskip)

    let naive() = // just cycle them at runtime!
        let (rtl, ans) =
            if machines=1 then solo_machine (hd vliws)
            elif machines=0 then ([], Xskip)
            else (kk 0 vliws,   gec_Xblock [ reshed;  ])
        if vd>=5 then vprintln 5 (what + " returns: " + hbevToStr ans)
        (rtl, ans)
        
    let nets0 = (if machines < 2 || oldsched then [] else [tidreg ]) @ map (fun((SP_fsm(info, arcs)))->info.pc) vliws
    let (code, nets) =
        let ii = { id="from_compose" } : rtl_ctrl_t
        if oldsched then
            let rec ko = function
                | [] -> ([], [])
                | h::t -> // Apply solo_mc to each machine in turn and put all in lockstep...
                   let (code0, bev0) = (solo_machine h) 
                   let (code, nets) = bevToXrtl ww cS1.ce.kK bev0
                   let (codets, netst) = ko t
                   let ccc = [ code ; SP_rtl(ii, code0) ]
                   ( ccc  @ codets, nets @ netst)
            let (c, n) = ko vliws
            (gen_SP_lockstep c, n)
        elif true then
            let (code0, bev0) = (naive())
            let (code1, nets) = bevToXrtl ww cS1.ce.kK bev0
            (gen_SP_lockstep [(SP_rtl(ii, code0)); code1], nets)
            
        else (SP_comment "", [])

    vprintln 3 ("Nets=" + i2s(length nets) + " and ans is " + hprSPSummaryToStr code)
    let ww = WF 3  what ww "end"
    ((directorate, code), nets @ nets0)


//
// hmm, called parallel - but actually put each machine or atomic block in some sequence.
//
let parallel_sequence_vliw ww kK (dir, vliw) =
    let ww = WF 3  "parallel seqeuence (each on its own)" ww "start"
    //let (msg, control, c2) = S1
    let rec kk n = function
        | [] -> []
        | SP_fsm(info, actions)::tt ->
           let pc=info.pc
           let rst=info.start_state
           let resumes=info.resumes
           let lcm_atoi = xi_manifest "adv"
           let adv ng d = if lcm_atoi  d < 0 then [] else [ gec_Xif1 ng (Xassign(pc, d)) ]
           let install_enum (pt, _) =
                 let dest_lab = enum_add "COMP1882" pc "COMP1882" (xi_manifest "COMP1882" pt)
                 ()
           app install_enum (info.start_state :: info.resumes)

           let r2bev ng cmd c = // convert to bev. TODO: this is done in abstracte and diosim! so don't repeat here!
              match cmd with
               | XRTL(None, g, lst) ->
                   let q = function
                       | Rarc(gc, l, r) -> gec_Xif1 (xi_and(xi_and(gc, ng), g)) (Xassign(l, r))
                       | Rpli(gc, (fgis, ord), args) -> gec_Xif1(xi_and(xi_and(gc, ng), g)) (gec_Xcall(fgis, (args)))
                       | Rnop s -> Xcomment s 
                   (map q lst) @ c
               | XIRTL_pli(None, g, (fgis, ord), args) -> gec_Xif1(xi_and(ng, g)) (gec_Xcall(fgis, (args))) :: c
               | _ -> sf "r2bev other: nonsence to have xrtl here anyway"
               
           let bi_action (resumept, hardf) (q:vliw_arc_t) c =
                let ng = ix_and  q.gtop (xi_deqd(q.pc, resumept))
                if resumept=q.resume then List.foldBack (fun cmd c->r2bev ng cmd c)  q.cmds ((adv ng q.dest)@c)
                else c

           let onethrd resumept =
                let pops = List.foldBack (bi_action resumept) actions []
                pops 

           let wholethread = gen_Xpblock (list_flatten(map onethrd resumes))
           vprintln 0 ("Parallel_sequence thread->" + hbevToStr wholethread)
           let other_fsms = kk (n+1) tt
           [wholethread; Xcomment "---------------------------"] @ other_fsms
        | _ -> sf "L332"
    let ast_ctrl = { g_null_ast_ctrl with id=funique "compose" } // Better to retain name via minfo and otehrs named exec forms
    let ans = gec_Xblock (kk 0 vliw) // Sequential composition of each participant with the next
    let nets0 = map (fun(SP_fsm(info, _))->info.pc) vliw
    let (code, nets) = if true then bevToXrtl ww  kK ans else (SP_l(ast_ctrl, ans), [])
    ((dir, code), nets @ nets0)

//
let checkpos ww kK (i, clk, SP_fsm(ii, ia)) (j, clk', SP_fsm(ji, ja)) =
    vprintln 3 ("Check pos " + i2s i + " cf " + i2s j)
    if j<>i // Need to check both ways round but not with self
    then
        let sqq (iarc:vliw_arc_t) jarc =
            vprintln 3 ("checkpos " + xToStr iarc.pc + "=" + xToStr iarc.resume + " with " + xToStr jarc.pc + "=" + xToStr jarc.resume)
            let _ = synch_rendez ww kK (ii, iarc) (ji, jarc)
            ()
        let sq (iarc:vliw_arc_t) = map (sqq iarc) ja        
        let _ = map sq ia
        ()


// Rebalance - find work ...
let rebal ww0 kK acode anets1 =
    let ww = WN "compose.rebal" ww0
    let nfsm = length acode
    g_next_procid := 0 // for now -- array must start 0
    let acode = map (fun (clk, fsm) -> (fresh_procid(), clk, fsm)) acode // Tag each fsm : use zipWithIndex neater.
    let procs = Array.ofList acode
    vprintln 2 ("Found " + i2s nfsm + " FSMs to compose\npcs=" + sfold xToStr anets1)

    let guardvars = new ListStore<hexp_t, int>("guardvars")
        // Find full support of guard conditions : these will be driven by actions.
    let shv_find1 c = function
            | (procid, dir__, SP_fsm(info, arcs)) -> // Deletes director info!
                let req c (q:vliw_arc_t) =
                    let support = xi_bsupport [] (q.gtop)
                    app (fun (id, _) -> guardvars.add id procid) support
                    sd_union(support, c) // not needed...
                List.fold req c arcs
    let shared_vars = List.fold shv_find1 [] acode
    reportx 3 "Shared variables used for guard conditions" (fun (a,b)->xToStr a) shared_vars

    let shared_vars = map (support_to_sd_form ww) shared_vars

    // We can find the interconnection matrix between machines via shared variables by scanning
    // We need to find the action on guard matrix: we find those actions which negate a guard.
    let shv_scon1 c = function
            | (procid, clko, SP_fsm(info, arcs)) ->
                let req c (q:vliw_arc_t) =
                    let squog_cmd (d, cde, s, eis) =
                        let q = sd_intersection_pred (map (support_to_sd_form ww) d) shared_vars
                        let interconnects = List.fold (fun c (d,_) -> guardvars.lookup d @ c) [] d 
                        let interconnects = list_subtract(interconnects, [procid])
                        //if interconnects<>[] then vprintln 0 ("procid=" + i2s procid + ": Squog " + xrtlToStr cde +  " life altering?=" + boolToStr q + "   " + sfold i2s interconnects)
                        let _ = map (fun jj -> checkpos (WN "scan" ww0) kK (procid, clko, SP_fsm(info, arcs)) procs.[jj]) interconnects
                        
                        ()
                    app squog_cmd (List.fold (tripgen_x 4 true) [] q.cmds)
                    c
                List.fold req c arcs
    let _ = List.fold shv_scon1 [] acode
    acode // Return arg - a nop for now


//
// Combine separate FSMs together. Provided dir.duid matches.
//
let opath_compose_core ww0 cS1 kK1 mc =
    match mc with
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls_, sons_, execs_, assertions))) ->
        let phasename = "compose"
        //establish_log false phasename
        let ww = WF 2 phasename ww0 "Commence"

        let (nets, execs, serts) =
            let rec flatten (decs, execs, serts) = function
                | (ii, None) -> (decs, execs, serts)
                | (uu, Some(HPR_VM2(minfo, decs', sons, execs', ass'))) ->
                    List.fold flatten (decs' @ decs, execs' @ execs, ass' @ serts) sons
            flatten ([], [], []) mc

        let ww = WF 2 "Compose:" ww0 "Flattened"

        let ((acode, otherexecs_), anets1) =
            let m_pcs = ref []
            let rec scanfsm0 dir arg (cc, dd) =
                match arg with
                | SP_fsm(info, a) ->  // We handle here only a parallel composition of FSMs.
                    mutadd m_pcs info.pc
                    ((dir, arg)::cc, dd)
                | SP_par(_, l) -> List.foldBack (scanfsm0 dir) l (cc, dd)
                | other ->
                    vprintln 2 ("Not applying the compose projection to input exec of form " + hprSPSummaryToStr other)
                    (cc, H2BLK(dir, other)::dd)

            let scanfsm1 arg cc =
                match arg with
                    | H2BLK(dir, rtl) -> (*List.foldBack*) (scanfsm0 dir) rtl cc
            (List.foldBack scanfsm1 execs ([],[]), !m_pcs)

        let _ = rebal (WN "rebal" ww0) kK1 acode anets1

        let nosched = cS1.scheduler = "none"
        let newseq = false // S1.scheduler = "parallelnew"

        //let acode = map (fun (procid, clko, fsm) -> (clko, fsm)) acode // convert back to this form for now

        let acode = if cS1.bisimulate then map (bisimulate (WN "bisimulate" ww)) acode else acode
        let ww = WF 2 "Compose:" ww0 (" Bisimulate " + if cS1.bisimulate then "Complete" else "Skipped")
        
        let (code, bnets1) =
            let rec par_elide sofar = function  // We probably want to collate here, rather than elide adjacent.
                | [] ->
                    match sofar with
                        | None -> []
                        | Some(dir0, code0) -> [(dir0, code0)]

                | (dir1, code1)::tt ->
                    match sofar with
                        | None -> par_elide (Some(dir1, [code1])) tt
                        | Some(dir0, code0) ->
                            if dir0.duid = dir1.duid then par_elide (Some(dir1, code0 @ [code1])) tt
                            else (dir0, code0) :: par_elide (Some(dir1, [code1])) tt
            let elided = (par_elide None acode)

            let (pcode, nets) =
                if nosched then (map (fun (dir, x) -> (dir, gen_SP_lockstep x)) elided, [])
                elif newseq then
                    let (b, n) = List.unzip (map (parallel_sequence_vliw ww kK1) elided)
                    (b, list_flatten n)
                else
                    let (b, n) = List.unzip (map (naive_sequence_vliw ww cS1) elided)
                    (b, list_flatten n)
            let ag (dir, code) = H2BLK(dir, code)
            (map ag pcode, nets)


        let pcnets =
            let cpi = { g_null_db_metainfo with kind= "compose-pcnets" }
            //reportx 3 "pc regs needed " xToStr pcnets
            let pcnets = lst_union anets1 bnets1
            reportx 3 "pc regs needed " xToStr pcnets
            gec_DB_group(cpi, map db_netwrap_null pcnets)

#if SPARE
        let clks =
            let cpi = { g_null_db_metainfo with kind= "compose-clks" }
            //reportx 3 "pc regs needed " xToStr pcnets
            gec_DB_group(cpi, map db_netwrap_null (map de_edge dir.clocks))
#endif            
        let clks = [] // clocks now added in rendering stages
        let decls = nets @ pcnets @ clks
        //reportx 3 "gdecs this machine " xToStr gdecs
        let ii =
            { ii with
                  generated_by= phasename
              //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
            }
        let decls = db_duplicates_trim ww decls
        let ans = (ii, Some(HPR_VM2(minfo, decls, [], execs (* @ otherexecs *), assertions)))
        let ww = WF 2 "Compose:" ww0 " Core Complete"  
        ans 


let opath_compose ww op_args vms =
    let c1:control_t = op_args.c3
    let stagename = op_args.stagename
    let disabled = 1= cmdline_flagset_validate stagename ["enable"; "disable" ] 0 c1
    vprintln 2 (compose_banner)
    if disabled
    then
        vprintln 2 "Stage is disabled";
        vms
    else                
        let zz = cmdline_flagset_validate "skip-propagate" ["false"; "true" ] 0 c1

        let skip = zz=1
        let vd = 3 // Cannot set this from recipe yet!
        let ubudget = control_get_i c1 "compose-ubudget" 10000
        let redirect =  0=cmdline_flagset_validate "compose-redirect" ["enable"; "disable" ] 0 c1  // Better to steal bevelab setting!?
        g_compose_loglevel := vd
        let kK1:bevelab.bevelab_budgeter_t = // repeated code - fix me - bevctrl needs split in two once we start using compose
                {
                    default_pause_mode= bevelab.pmos_lexate(control_get_s "compose" c1 "compose-default-pause-mode" None) // Better to steal bevelab setting!?
                    big_bangp=      false // for now
                    systolic4=      "disable"
                    revast_enable=  false
                    ubudget=        ubudget
                    morenets=       None
                    retnet=         None // WRONG determine_retnet (ldecls @ gdecls) 
                    cost_queries=   { g_null_queries with logic_costs=Some(logic_cost_walk_set_gen ww vd "opath_compose" false) }
                    redirect=       redirect
                    soft_pause_threshold= int64(control_get_i c1 "compose-soft-pause-threshold" 1000)
                 }

        let ce=
                {
                    kK=kK1
                }


        let cS1:compose_S1_t =
                 {
                   vd=                 vd
                   ce=                 ce
                   scheduler=          control_get_s "compose" c1 "scheduler" None
                   ubudget=            ubudget
                   msg=                op_args.banner_msg
                   bisimulate=         0=cmdline_flagset_validate "compose-bisim-reduction" ["enable"; "disable" ] 1 c1

                 }

        vprintln 2 "Starting compose 1749";    

        let vms = 
            let mfun vm = 
                let _ = WF 2 stagename ww "Starting 1749";    
                let vm = opath_compose_core (WN "compose_core" ww) cS1 kK1 vm
                if true || skip then vm else opath_propagate_serf (WN "bevelab_propagate" ww) op_args vm
            map mfun vms

        vms


let install_compose() =
    let bev_argpattern =
        [
          Arg_enum_defaulting("compose", ["enable"; "disable"; ], "enable", "Disable this stage");
          
//        Arg_enum_defaulting("compose-tracing", ["enable"; "disable"; ], "disable", "Initial state for trace enable flag");

          Arg_enum_defaulting("compose-redirect", ["enable"; "disable"; ], "disable", "Redirect goto statements to share Pause calls (and other shareable code).");

          Arg_enum_defaulting("compose-bisim-reduction", ["enable"; "disable"; ], "disable", "Share states that are observably equivalent.");          

//          Arg_enum_defaulting("compose-autobarrier-namealias", ["enable"; "disable"; ], "enable", "Enable automatic pause insertion to disambiguate name alias undecidable (array index not comparable)");


          //Arg_enum_defaulting("compose-autobarrier-stuckinloop", ["enable"; "disable"; ], "enable", "Enable automatic pause insertion on a loop that will not unwind");

          Arg_enum_defaulting("compose-detailed-trace", ["enable"; "disable"; ], "disable", "Enable further print statements");

          //Arg_enum_defaulting("compose-generate-nondet-monitors", ["enable"; "disable"; ], "enable", "Enable embedding of runtime monitors for non-deterministic updates");
          

          Arg_enum_defaulting("compose-default-pause-mode", ["auto"; "hard"; "soft"; "maximal"; "bblock"], "auto", "Which mode to start a thread, before pause control is set.");
          
          Arg_enum_defaulting("scheduler", ["oldsched"; "naivenew"; "parallelnew"; "none" ], "oldsched", "Select th
                              read scheduler");
          Arg_int_defaulting("compose-soft-pause-threshold", 1000, "Nominal working cost of work per clock cycle");

          Arg_int_defaulting("compose-ubudget", 10000, "Steps to unwind before considering unwindable");

        ]

    let _ = install_operator ("compose",  "Behavioural code to FSM Generator", opath_compose, [], [], bev_argpattern)

    ()


// eof

