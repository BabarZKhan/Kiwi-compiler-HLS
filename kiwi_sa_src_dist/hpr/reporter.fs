// CBG Orangepath.  HPR L/S Formal Codesign System.
//
// reporter.fs
//
// Generate predictor and dot output file entries. - only the dot code moved in here from res2 so far
//
// All rights reserved. (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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


module reporter

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open hprls_hdr
open abstract_hdr
open abstracte
open moscow
open yout
open meox
open dotreport


let g_shading_col = ref 0

let get_threadcolor() =
    let threadcolor = g_pastels.[!g_shading_col % List.length g_pastels ]
    mutinc g_shading_col 1
    threadcolor

// Convert FSM control flow to dot viz.
let fsm_dot_gen_serf ww threadname nodework vliw_arcs =
    let add p n = if  n="" then p else p + "\n" + n
    let ww = WF 3 "fsm_dot_gen_serf" ww ("start threadname=" + threadname)
    let pcLabName pcv = sprintf "%s:STX%i" threadname pcv
    let threadcolor = get_threadcolor()
    let fully_sanitise s = string_fold_and_sanitize_no_leading_digit 55 g_okl2 s

    let edgeName vliw_arc_idx = sprintf "edgeblock%i" vliw_arc_idx

    let getArc npcv eno =
        // search new eno in vliw_arc_t
        let rec gra2 = function
            | [] ->
                dev_println (sprintf "gra2 no arc %i,%i" npcv eno)
                None
            | (h:vliw_arc_t)::_ when h.iresume = npcv && h.eno = eno -> Some h
            | _ :: tt -> gra2 tt
        gra2 vliw_arcs

    let resource_use_arcs = []
#if NOTYET
// This code to be resurrected here from res2.
    let resource_use_arcs =
        let gen_resource_use_arc cc (exec_pcv, pge_lst) =
            let grua cc = function
                | PGE_wi (CBG_track(npcv, eno), shen, newop) ->
                    match newop.str with
                        | Some sc2 ->
                            let name = resName sc2.ikey
                            //let _ = vprintln 3 (sprintf " resource use arc %s te=%s" name (shenToStr shen))
                            let (mwhen0, mwhen1) = house2a "L654" newop 
                            let label =  sc2.kkey
                            let ats = [ ("shape", "hexagon"); ("label", label); ("color", "blue") ]
                            let addBlockUseArcs cc (HK_abs(vl, _)) =


                                let addBlockUseArcs_serf cc v =
                                        if newop.cmd = "scalarw" then cc
                                        else
                                        let gubbins = "cmd=" + newop.cmd + "\n" + sfoldcr_lite (fun x->fully_sanitise (xToStr x)) newop.args
                                        let nl =
                                            if anal.dot_plot_detailed then
                                                [DARC(DNODE(edgeName v), DNODE(name), [  ("color", "blue"); ("label", gubbins) ])]
                                            else
                                                let adon_name = funique "adon_name" // Do not make an arc to the resource unless detailed since causes a confused drawing.  Instead have an add-on local node.
                                                [
                                                    DNODE_DEF(adon_name, [("label", gubbins)])
                                                    DARC(DNODE(edgeName v), DNODE(adon_name), [  ("color", "blue");  ])
                                                ]

                                        let resp =
                                            if newop.postf || nonep newop.str then []
                                            else
                                                let dests rv =
                                                    let n2 = DARC( DNODE(name), DNODE(pcLabName rv), [  ("style", "dashed"); ("color", "blue"); ("label", "*result*") ])
                                                    (XML_WS " ", [n2])
                                                map dests mwhen1 
                                        (x, nl) :: resp @ cc
                                List.fold addBlockUseArcs_serf cc vl
                            let cc = List.fold addBlockUseArcs cc shen.old_pcvs // can use mwhen0 instead
                            let n = DNODE_DEF(name, ats)
                            ([n]) :: cc
                        | _ -> cc

                | PGE_wp(CBG_track(npcv, eno), waypoints) ->
                    let ag pp g =
                        let s1 = match pp with
                                   | None -> ""
                                   | Some (pc, pcv) -> xToStr pcv
                        let s2 = if g=X_true then "&&" else " && " + xbToStr g
                        s1+s2

                    let mao = getArc npcv eno

                    // If we were smarter we would illustrate the control flow arc going via the waypoint shape!
                    let make_wp_shape cc wpf conds msg =
                        let k_int = exec_pcv
                        let wname = funique (sprintf "%s_waypoint_icon%i" threadname k_int)
                        //let _ = vprintln 3 (sprintf " graph/plot waypoint %s %s " wname msg)
                        let label = fully_sanitise msg
                        let ats = (if wpf then [("fillcolor", "orange"); ("shape", "octagon")] else[("shape", "note")]) @ [ ("label", label); ("color", "red"); ("style", "filled");  ]  
                        // We want to connect to the edge symbol from the execpc on this edge
                        let from =
                            match mao with
                                | None    -> pcLabName exec_pcv // TODO this is root pcv for a control edge structure access
                                | Some ma -> edgeName ma.eno
                        let n0 = DARC(DNODE from, DNODE wname, [  ("color", "orange"); ("label", fully_sanitise conds) ])
                        let n1 = DNODE_DEF(wname, ats)
                        (x0, [n0])::(x1, [n1])::cc
                    let waypoint_tog cc = function
                        | WP_wp(pp, g, nn, msg)            -> make_wp_shape cc true  (ag pp g) msg
                        | WP_other_string (pp, g, fn, msg) -> make_wp_shape cc false (ag pp g) (fn + ":" + msg)

                    List.fold waypoint_tog cc waypoints
                    
            List.fold grua cc pge_lst
        List.fold gen_resource_use_arc [] (list_flatten predictor_graph_work) // Perhaps ignore this and plot from the vliw_arcs?
#endif
    let getRepresentativeArc n =
        let rec gra = function
            | [] -> sf "no representative arc"
            | h::_ when h.iresume = n -> h
            | _ :: tt -> gra tt
        gra vliw_arcs

    let start_name = threadname + "_START"
    let startNode  =
        let label = "START"
        let ats = [ ("shape", "box"); ("label", label) ]
        let n = DNODE_DEF(start_name, ats) 
        ([n])


    let startEdge =
        let n = DARC(DNODE(start_name,""), DNODE(pcLabName 0,""), [ ("color", "red") ]) // unconditional nodes in red
        [n]
        
    let gen_controlFlowNode sti =
        let arc = getRepresentativeArc sti // just for old resume name
        let name = pcLabName sti
        let oldname = if nonep arc.old_resume then "" else xToStr(valOf arc.old_resume) + "\n"
        let additional =
            match op_assoc sti nodework with
                | None -> ""
                | Some items -> "\n" + sfoldcr_lite (fun x -> x) items
        let label = oldname + name + additional            
        let ats = [ ("shape", "box"); ("label", label); ("style", "filled"); ("fillcolor", threadcolor) ]
        let n = DNODE_DEF(name, ats) 
        //vprintln 3 ("dot_report control flow node " + name)
        [n]
    
    let gen_controlFlowEdge  (ma:vliw_arc_t) =  // Unless unconditional with no actions, a control flow edge is rendered as a dot node with a dot edge in and out of it
        //let pc = xi_manifest "L419" ma.resume
        //let dest = if ma.dest=X_undef then -1 else xi_manifest "L420 ma.dest" ma.dest
        let gs1 = "" // sprintf "A%i" ma.eno
        if ma.gtop = X_true && nullp ma.cmds then
            let arc = DARC(DNODE(pcLabName ma.iresume,""), DNODE(pcLabName ma.idest,""), [ ("color", "red"); ("label", gs1) ])
            [arc]
        else
            // lable "{ top | bottom }"
            let gs2 = sprintf "E%i\n\"%s\"" ma.eno (fully_sanitise (xbToStr_concise ma.gtop))        
            //vprintln 0 (sprintf "reporter_fsm debug %i -> %i" ma.iresume ma.idest)
            let colour = if ma.gtop = X_false then "pink" else "black"
            let e_in = DARC(DNODE(pcLabName ma.iresume,""), DNODE(edgeName ma.eno, ""), [ ("color", colour); ("label", gs1) ])
            let en =
                let label = gs2
                let label =
                    if nullp ma.cmds then label
                    else
                        let arcwork =
                            let aw cc = function
                               | XRTL(_, _, wlst) ->
                                    let ax cc = function
                                        | Rarc(g, l, r) -> add (xToStr l) cc
                                        | _-> cc
                                    List.fold ax cc wlst
                               | _-> cc                               
                            List.fold aw "" ma.cmds
                        sprintf "{ %s | %s }" label arcwork
                let label = string_escape_generic [ '|'; '{'; '}'; '>'; '<' ] label // cannot have braces and angle brackets in a record label since they control vert/horiz and ports. Move san to dotreport.
                let ats = [ ("label", label);  ("color", threadcolor); ("shape", "record") ]
                DNODE_DEF(edgeName ma.eno, ats) 
            let e_out = DARC(DNODE(edgeName ma.eno, ""), DNODE(pcLabName ma.idest,""), [ ("color", colour); ("label", gs1) ])
            [en; e_in; e_out]



    let blockNodeIdxs = List.fold (fun cc ma->singly_add ma.iresume cc) [] vliw_arcs
    let nodes = startNode :: map gen_controlFlowNode blockNodeIdxs
    let edges = startEdge :: (map gen_controlFlowEdge vliw_arcs)
    (nodes, edges @ resource_use_arcs)




let hpr_sp_l_dot_report ww (ast_ctrl:ast_ctrl_t, bev) cc =
    let ww = WF 3 "hpr_sp_l_dot_report" ww ("start")

    let m_emissions = ref []
    let emit x = mutadd m_emissions x
    let threadcolor = get_threadcolor()

    //let n =
    //               
    let nnode ats key = 
        let ss = funique key
        let ats =  ("label", ss) :: ats
        emit(DNODE_DEF(ss, ats))
        ss

    let leafer2 predec_ ats sx =
        ("unused", string_autoinsert_newlines 55 sx)

    let leafer predec_ ats sx =
        let (_, sx) = leafer2 predec_ ats sx
        let ss = nnode [ ("shape", "box"); ("label", sx)] "LEAFER"
        ss

    let rec leafx = function // These commands are rendered into a macroblock if occurring adjacently
        | Xcomment ss    -> Some(leafer2 "predec_" [] ("Comment " + ss))
        | Xreturn ee     -> Some(leafer2 "predec_" [("shape", "cds")] ("return " + xToStr ee))
        | Xeasc ee       -> Some(leafer2 "predec_" [] (xToStr ee))
        | Xassign(ll, rr)-> Some(leafer2 "predec_" [] (xToStr ll + ":=" + xToStr rr))
        | Xbreak         -> Some(leafer2 "predec_" [] ("Xbreak"))
        | Xcontinue      -> Some(leafer2 "predec_" [] ("Xcontinue"))
        | Xskip          -> Some(leafer2 "predec_" [] ("Xskip"))
        | Xlinepoint(_, ss)   -> leafx ss // Not rendered
        | _ -> None

    let rec walkx predec_ arg =
        match arg with
        | Xif(gg, Xskip, ff) ->
            let qs = nnode [("label", sprintf "Xif-ff(%s)" (xbToStr gg)); ("shape", "diamond") ] "XIFNOT"
            let sf = walkx qs ff
            emit(DARC(DNODE(qs, ""), DNODE(sf, ""), [ ("penwidth", "2");  ("color", "black") ]))             
            qs

        | Xif(gg, tt, Xskip) ->
            let qs = nnode [("label", sprintf "Xif-tt(%s)" (xbToStr gg)); ("shape", "diamond") ] "XIFONEHAND"
            let st = walkx qs tt
            emit(DARC(DNODE(qs, ""), DNODE(st, ""), [  ("penwidth", "2"); ("color", "green") ]))
            qs

        | Xif(gg, tt, ff) -> // 2-handed IF is a triangle instead of a diamond.
            let qs = nnode [("label", sprintf "Xif-tt-ff(%s)" (xbToStr gg)); ("shape", "triangle") ] "XIF"
            let st = walkx qs tt
            let sf = walkx qs ff
            //emit(DARC(DNODE(predec, ""), DNODE(qs,""), [ ("color", "red");  ]))
            emit(DARC(DNODE(qs, ""), DNODE(st, ""), [  ("penwidth", "2"); ("color", "green") ]))
            emit(DARC(DNODE(qs, ""), DNODE(sf, ""), [  ("penwidth", "2"); ("color", "black") ]))             
            qs

        | Xwhile(gg, body) ->
            let qs = nnode [("shape", "septagon"); ("fillcolor", "orange"); ("label", sprintf "Xwhile(%s)" (xbToStr gg)) ] "XWHILE"
            let st = walkx qs body
            //emit(DARC(DNODE(predec, ""), DNODE(qs,""), [ ("color", "red") ]))
            emit(DARC(DNODE(qs, ""), DNODE(st,""), [  ("penwidth", "4"); ("color", "orange") ]))             
            qs
        | Xdo_while(gg, body) ->
            let qs = nnode [("shape", "septagon"); ("fillcolor", "orange"); ("label", sprintf "Xdowhile(%s)" (xbToStr gg)) ] "XDOWHILE"
            let st = walkx qs body
            //emit(DARC(DNODE(predec, ""), DNODE(qs,""), [ ("color", "red") ]))
            emit(DARC(DNODE(qs, ""), DNODE(st,""), [  ("penwidth", "4"); ("color", "orange") ]))             
            qs

        | Xpblock lst
        | Xblock lst ->
            let rec slst predec_ = function
                | [] -> leafer predec_ [] "Implied-end-of-block-skip" // Shold not be encounted while we have Xskip instead of Xblock []
                | [item] -> walkx predec_ item
                | lst ->
                    let rec get_aggs sofar = function
                        | [] ->   (rev sofar, [])
                        | hh::tt ->
                            //dev_println(sprintf "%i sofar " (length sofar))
                            match leafx hh with
                                | Some(labs, xs) -> get_aggs ((hh, labs, xs)::sofar) tt
                                | None    -> (rev sofar, hh::tt)
                    let (aggs, tt) = get_aggs [] lst
                    //dev_println(sprintf "%i aggs %i tt " (length aggs) (length tt))            
                    if nullp aggs then
                        let (hh, tt) = (hd lst, tl lst)
                        let here = walkx predec_ hh
                        let follows = slst predec_ tt                    
                        emit(DARC(DNODE(here, ""), DNODE(follows,""), [  ("penwidth", "2"); ("color", threadcolor) ])) 
                        here
                    else 
                        let rec trim_boring_unless_only_cmd = function
                            | [] -> []
                            | [item] -> [item]
                            | (Xeasc(v), _, _) :: tt when xi_iszero v -> trim_boring_unless_only_cmd tt
                            | (Xskip, _, _)    :: tt -> trim_boring_unless_only_cmd tt
                            | hh::tt                 -> hh :: trim_boring_unless_only_cmd tt
                                
                        let here = leafer predec_ [] (sfoldcr_lite (fun (_, unused_, x) -> x) (trim_boring_unless_only_cmd aggs))
                        let follows = slst predec_ tt                    
                        emit(DARC(DNODE(here, ""), DNODE(follows,""), [ ("penwidth", "2"); ("color", threadcolor) ])) 
                        here
                    
            slst predec_ lst

        | Xlabel ss      -> leafer predec_ [] (ss + ":")
        | Xgoto(ss, _)   -> leafer predec_ [("fillcolor", "orange")] ("Xgoto " + ss)
        | Xbeq(gg, lab, _)    -> leafer predec_ [("fillcolor", "orange")] ("Xbeq " + xbToStr gg + " " + lab)
        | Xlinepoint(_, ss)   -> walkx predec_ ss // Not rendered

#if SPARE
    | Xannotate(a_, t) -> XML_ELEMENT("Xannotate", [], [serialise_bev fd t])
    | Xwaitabs e -> XML_ELEMENT("Xwaitabs", [], [ xi_serialise fd e ])
    | Xwaitrel e -> XML_ELEMENT("Xwaitrel", [], [ xi_serialise fd e ])
    | Xwaituntil e -> XML_ELEMENT("Xwaituntil", [], [ xb_serialise fd e ])
    | Xcall((f, gis), args) -> XML_ELEMENT("Xcall", [], XML_ELEMENT("NAME", [], [XML_ATOM f]) :: map (xi_serialise fd) args)
    | Xdominate(s, _) -> XML_ELEMENT("Xdominate", [], [XML_ATOM s])


#endif
        | arg ->
            match leafx arg with
                | None -> sf("hpr_sp_l_dot_report " + hbevToStr arg)
                | Some(labs, ans) -> ans
                
    let epnode = nnode [("label", "START " + ast_ctrl.id); ("shape", "house")] "START"
    let ans = walkx epnode bev
    emit(DARC(DNODE(epnode, ""), DNODE(ans, ""), [ ("color", threadcolor) ])) 
    (ast_ctrl.id, rev [!m_emissions]) :: cc // end of hpr_sp_l_dot_report
    
//
//        Write a control flow report in DOT form. 
// Use -cfg-plot-each-step to get dot reports.
let hpr_sp_dot_report ww title richf execs =
    let ww = WF 3 "hpr_sp_dot_report" ww ("start " + title)
    let rec miner cc = function
        | SP_dic(dic, ctrl)  -> 
            let vd = -1
            let id = ctrl.ast_ctrl.id
            let ww = WF 3 "hpr_sp_dot_report" ww (sprintf "hpr_sp_dot_report: cfg dot report title=%s DIC %s richf=%A" title id richf)
            let dic_record__ = assembler_vm ww vd ctrl  false dic
            let (bblock_starts, numbering, fwd, back) = collate_dic_blocks ww vd richf "hpr_fsm_dot" dic

            let nodework =
                let gnw pc =
                    let rec collect_work pc1 cc =
                        let e = valOf_or_fail "gwn" (numbering.TryFind pc1)
                        if e<>pc || pc1 >= dic.Length then cc
                        else
                            let cc =
                                match dic.[pc1] with
                                    | Xassign(l, r) -> singly_add (xToStr l + " :=") cc
                                    | _ -> cc
                            collect_work (pc1+1) cc
                    let work = collect_work pc []
                    (pc, work)

                
                map gnw bblock_starts
            // With the DIC form, there is work in the nodes, whereas with the SP_fsm form, the work is on the edges.
            let arcs = // Create a temporary microarc set from a DIC just to use the plotter output code.
                let pc = gec_X_net id
                let temparc ((f, (g, t)), e) =
                    { g_null_vliw_arc with 
                            pc=            pc
                            gtop=          g
                            resume=        xi_num f
                            iresume=       f
                            dest=          xi_num t
                            idest=         t
                            eno=           next_global_arc_no()
                    }
                let arcs = list_flatten(map (fun s -> map (fun t ->(s, t)) (valOf_or_nil (fwd.TryFind s))) bblock_starts)
                map temparc (zipWithIndex arcs)
            let (p1, p2) = fsm_dot_gen_serf ww id nodework arcs
            (id, p1 @ p2)::cc
            
        | SP_fsm(info, arcs) ->
            let thread = info.pc
            let threadname = xToStr thread
            let ww = WF 3 "hpr_sp_dot_report" ww ("cfg dot report SP_fsm " + threadname)
            let (p1, p2) = fsm_dot_gen_serf ww threadname [] arcs
            (threadname, p1 @ p2)::cc 
        | SP_par(_, lst) -> List.fold miner cc lst
        | SP_seq(lst) -> List.fold miner cc lst        
        | SP_l(ast_ctrl, bev) ->
            hpr_sp_l_dot_report ww (ast_ctrl, bev) cc
        | _ -> cc

    let miner1 cc = function
        | H2BLK(dir_, sps) -> miner cc sps

    List.fold miner1 [] execs

    
// Write dot and control flow files. Separately after each thread or all combined at the end.    
// Generally enabled with the -cfg-plot-each-step
// View with something like dot < controlflow_cfg_plot_stage1.dot -T > ~/png.png
let render_dot_plot ww keyname lst =
    if nullp lst then
        vprintln 2 ("no dot plots to print for " + keyname)
        ()
    else
    let filename = "controlflow_" + keyname + ".dot"
    let fd = yout_open_log filename
    yout fd (report_banner_toStr "// ")
    let ww = WF 3 "render_dot_plot" ww (sprintf "start %i plots" (length lst))
    let rdp (tag, contents) =
        let ww = WF 3 "collate_dot_plot" ww (sprintf "tag=%s file=%s plots" tag filename)
        yout fd (sprintf "// + including tag=%s\n" tag)
        list_flatten contents
    let data = map rdp lst
    dotout fd (DOT_DIGRAPH("profiler_cf_" + keyname, list_flatten data))
    yout_close fd
    let ww = WF 3 "render_dot_plot" ww "done"
    ()



// eof
