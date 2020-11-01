//
//
// CBG Orangepath. HPR Logic Synthesis and Formal Codesign System.
//
// abstracte.sml - Defines a virtual machine that executes parallel hbev_t programs of various forms.
// 
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


module abstracte

let g_report_each_step             = ref true            // Why not in opath.fs ?
let g_rtl_finish_render_enable     = ref true //// OLD backdoor  - should go down the directorate path now

open System.Collections.Generic
open System.Numerics
open Microsoft.FSharp.Collections
open yout
open hprls_hdr
open meox
open abstract_hdr
open moscow
open opath_hdr
open hprxml
open linprog
open linepoint_hdr
open protocols


let g_async_designation = Some(X_undef, X_num 0)



let g_unbound = "unbound continue/break statement"


let g_null_vliw_arc =
    {
        old_resume= None
        pc=         X_undef
        resume=     X_undef
        iresume=    -1
        dest=       X_undef
        idest=     -1
        gtop=         X_true
        cmds=         []
        hard=         g_default_visit_budget
        eno=          -1
        visit_ratio=  None
    }

let g_null_ast_ctrl =
    {
        id=               "anon"
    }:ast_ctrl_t

let g_null_dic_ctrl =
    {
        ast_ctrl=         g_null_ast_ctrl
    }:dic_ctrl_t

    
(* Cannot store ML functions in tables and then do a table lookup since they are non-equality types, even without free vars. 
 * Should they all be in this table too ? Not this one  - it is deprecated and replaced with hpr_native_table. Please remove this or else explain that it contains ML function implementations.
 *)
let xg_hpr_canned_table =
    [ 
      ("hpr_arrayop", true);
    ] 

let edgeToStr = function
    | E_pos x -> "posedge(" + xToStr x + ")"
    | E_anystar -> "*"
    | E_neg x -> "negedge(" + xToStr x + ")"
    | E_any x -> "anyedge(" + xToStr x + ")"


let fatal_marked vm =
    let rec fm_scan arg cc =
        match arg with 
            | (ii, Some(HPR_VM2(minfo, decls, sons, execs, asserts))) -> minfo.fatal_flaws @ List.foldBack fm_scan sons cc
            | _ -> cc
    fm_scan vm []


let visitToStr visit_budget =
        sprintf "VT=%i H=%A" visit_budget.visit_time visit_budget.hardf

let timespecToStr performance_target =
    (if performance_target.hardness > 0.0 then sprintf " hardness=%1.2f" performance_target.hardness else "") +
    (if performance_target.latency_target >= 0 then sprintf " latency_target=%i" performance_target.latency_target else "") +
    (if performance_target.ii_target >= 0 then sprintf " ii_target=%i" performance_target.ii_target else "")


let g_directorate_burner = ref 10

let next_duid() = funique "DDX"


let hangmodeToStr = function
    | DHM_pipelined_stream_handshake(subschema_string, protocol_prams) -> "pipelined_stream_handshake:" + subschema_string
    | other -> sprintf "%A" other
    
let dirToStr include_timespecf dir =
    if dir.duid = g_null_directorate.duid then " -duid unset- "
    //let msg = (sprintf "Director duid is unset")
    //            hpr_yikes msg
    //msg
    else
    let style = sprintf "%A " dir.style
    let hangmode = hangmodeToStr dir.hang_mode
    let pc_export = if dir.pc_export then "PCEXPORT " else ""
    let performance_target =
        match dir.performance_target with
            | Some performance_target when include_timespecf -> timespecToStr performance_target
            | _ -> ""
    
    let performance_target =
        if performance_target <> "" then sprintf " performance target is " + performance_target else ""

    let resetToStr(is_pos, is_asynch, net) =
        let ans = xToStr net
        let ans = if is_pos then ans else "!" + ans
        let ans = if is_asynch then "ASYNC-" + ans else ans
        ans

    let cenToStr(is_pos, net) =
        let ans = xToStr net
        let ans = if is_pos then ans else "!" + ans
        ans

    let clock_details =
        (if nullp dir.clocks then " NoClock" else " Clocks=" + sfold edgeToStr dir.clocks) +
        (if nullp dir.resets then " NoReset" else " Resets=" + sfold resetToStr  dir.resets) +
        (if nullp dir.clk_enables then "" else " ClockEnables=" + sfold xToStr  dir.clk_enables)
    sprintf "%s:" dir.duid  + pc_export + style + hangmode + clock_details + performance_target

            
let clkmine cc (H2BLK(dir, _)) = lst_union dir.clocks cc

let nemtokToStr = sfold id

let mdescs_sort lst =
    let pred (aid, a:memdesc_record_t) = function
        | (bid, Memdesc0 b) ->
            match a with
                | Memdesc0 a -> if a.uid.baser = b.uid.baser then 0 elif a.uid.baser > b.uid.baser then 1 else -1
                | _ -> -1
        | _ -> 0
    List.sortWith pred lst
    

let rec ii_fold = function // This reverses since the innermost tag is at the head of the list.
    | [] -> "NIL"
    | [item] -> item.iname + "//" + vlnvToStr_full item.vlnv  // We render as iname//componentRef with a double-slash separator.
    | h::tt -> ii_fold tt + "." + h.iname

let ii_summary ii = // VM2 Instance info summary
    (if ii.definitionf then "DEFINITION " else "INSTANCE ") +
    (if ii.nested_extensionf then "NESTED-EXTENSION " else "") +
    (if ii.externally_provided then "EXTERNALLY_PROVIDED " else "") + 
    (if ii.external_instance then "EXTERNAL_INSTANCE " else "") +    
    (if ii.preserve_instance then "PRESERVE_INSTANCE " else "") +    
    (if ii.in_same_file then "IN_SAME_FILE " else "") +
    "ii " +
    (if ii.iname="" then "" else sprintf " iname='%s'" ii.iname)

   
let pausemodeFlagToStr arg =
    (if arg &&& hpr_PauseFlags.NoUnroll > 0 then  " NoUnroll" else "") +
    (if arg &&& hpr_PauseFlags.SoftPause > 0 then " SoftPause" else "") +
    (if arg &&& hpr_PauseFlags.HardPause > 0 then " HardPause" else "") +
    (if arg &&& hpr_PauseFlags.CurrentModePause > 0 then " CurrentModePause" else "") +
    (if arg &&& hpr_PauseFlags.EndOfElaborate > 0 then " EndOfElaborate" else "") 


let vm_att_assoc tag (ids, mo) =
    match mo with
        | Some vm ->
            let minfo = hpr_minfo vm
            at_assoc tag minfo.atts
        | None -> None

// A component has three levels of formal: metaprams, overrides and hexp_t contacts.
// skip_params=Some true -->  return only nets
// skip_params=Some false -->  return only overrides
let rec db_flatten_generic (skip_params, depth_limit) depth ml arg cc =
    let vd = false
    match arg with
    | DB_group(meta, lst) ->
        if vd then dev_println (sprintf "Consider depth=%i DB_group %s" depth meta.pi_name)
        if vd then dev_println (sprintf "Consider DB_group si far %i" (length cc))
        let is_pram = (meta.form=DB_form_meta_pram || meta.form=DB_form_pramor)
        let skip = meta.norender || // norender is used for internal nets in behavioural internal model of library cell, such as RAM.
                   (is_pram && skip_params=Some true) ||
                   (depth_limit >= 0 && depth >= depth_limit) ||
                   (not is_pram && skip_params=Some false)
        if skip then
            if vd then dev_println (sprintf "Skipping DB_group %s" meta.pi_name) 
            cc
        else
            List.foldBack (db_flatten_generic (skip_params, depth_limit) (depth+1) (meta::ml)) lst cc
    | DB_leaf(Some formal, ao) ->
        if vd then dev_println (sprintf "db_flatten  formal=%s" (xToStr formal))
        if vd && not_nonep ao then hpr_yikes(sprintf "skip flatten of binding for f=%s a=%s" (netToStr formal) (xToStr (valOf ao)))
        (ml, formal) :: cc
    | DB_leaf(None, Some actual) ->
        if vd then dev_println (sprintf "db_flatten  actual=%s" (xToStr actual))
        (ml, actual) :: cc

let rec db_flatten ml arg cc = db_flatten_generic (None, -1) 0 ml arg cc

let gec_DB_group(meta, lst) = if nullp lst then [] else [DB_group(meta, lst)] // Used when we do not want to create these with empty args


// ---------------------------------------------------------
// Remove any duplicate formal net declarations within a DB_group tree.
let rec db_duplicates_trim ww decls =
    let ww = WF 3 "duplicates_trim" ww "start"

    let rec trimmer arg (cc, map:Map<xidx_t, bool>) =
        match arg with
            | DB_group(meta, lst) ->
                let (lst', map') = List.foldBack trimmer lst ([], map) 
                (DB_group(meta, lst')::cc, map')
            | DB_leaf(None, Some kk) ->
                let nn = x2nn kk
                if not_nonep(map.TryFind nn) then (cc, map) else (arg::cc, map.Add (nn, true))
            | DB_leaf(Some kk, ao) ->
                let nn = x2nn kk
                if not_nonep(map.TryFind nn) then (cc, map) else (arg::cc, map.Add (nn, true))
    let (decls, map_out_) = List.foldBack trimmer decls ([], Map.empty)
    decls


// TODO: Next function is more-or-less the same ?

// Trim or delete nets from a decls structure without otherwise altering its shape and properties.
// If flip holds we delete those specified by the first operand, otherwise we retain only those.
// When keepio holds we retain all I/O nets regardless of predicate.
let decls_trim flip (* keepio *) nets_to_add_or_take decls =
    let retained_map =
        let gen (m:Map<int, bool>) v = m.Add (x2nn v, true)
        List.fold gen Map.empty nets_to_add_or_take
    let rec filter arg cc =
        match arg with
            | DB_group(meta, lst) ->
                if meta.not_to_be_trimmed then arg :: cc
                //elif meta.is_param then arg :: cc   // keep all parameters  conerefine was discarding them but now we mark them not_to_be_trimmed=true
                else
                    let lst' = List.foldBack filter lst []  
                    if nullp lst' then cc
                    else DB_group(meta, lst')::cc

            | DB_leaf(fo, ao) ->
                let discard_pred = function
                    | None -> false
                    | Some kk ->
                        match retained_map.TryFind (x2nn kk) with
                            | Some true -> flip
                            | _         -> not flip
                if discard_pred fo || discard_pred ao then cc else arg::cc // A little bit loose and flexible here ... perhaps better for the user to nominate fo v ao.

    List.foldBack filter decls []

// ---------------------------------------------------------
// Lookup a primitive function by name
let hpr_native id = 
    match builtin_fungis id with
        | None ->
            sf("+++ function/method is missing from hpr_native_table: " + id)
        | Some ov -> (id, ov)

    
let gen_pause_call args = // If no arg, then make call in current pause mode for the thread at that point.
    let args = if nullp args then [xi_num hpr_PauseFlags.CurrentModePause ] else args
    xi_apply(hpr_native "hpr_pause", args)

let gen_hard_pause_call () = gen_pause_call [ xi_num hpr_PauseFlags.HardPause ] // Alternatively [ xi_string "hard" ] should work just as well

let gen_postdom_call (args) = xi_apply(hpr_native "hpr_postdom", args)


let rec hbev_is_decoration = function //cf commenty function
    | Xlinepoint(lp, cmd) -> hbev_is_decoration cmd
    | Xskip
    | Xcomment _         
    | Xlabel _    -> true
    | _ -> false



let gen_SP_seq = function
    | []     -> SP_comment "no items"
    | [item] -> item
    | t      -> SP_seq(t)


let gen_SP_par pstyle = function // Ideally want to flatten a tree of these constructors.
    | []     -> SP_comment "no items"
    | [item] -> item
    | t      -> SP_par(pstyle, t)


let gen_SP_lockstep items = gen_SP_par PS_lockstep items

let ppToStr A =
    match A with
        | None -> "():"
        | (Some(pc, lab)) -> 
               if A = g_async_designation then "*ASYNCHD*"
               else "(" + xToStr pc + "==" + xToStr lab + "):"


let g_order_tag = ref 1
let next_tag() =
    let rr = !g_order_tag
    g_order_tag := rr+1
    rr

// vsfg ordering for fences and side effects.    
let orderToStr = function
    | (domain, idl, n, m) -> domain + "{" + sfold (fun x->x) idl + i2s n + ":" + i2s m + "}"

let orderToStr_opt = function
    | None -> ""
    | Some vsfg_order -> orderToStr vsfg_order



let rec arcToStr fullf (si:vliw_arc_t) =
    let fsm_arcwork = if fullf then sfold (fun x ->  "\n" + xrtlToStr x ) si.cmds else ""
    //in "\nFSM_arc: " + xToStr si.resume + "      g=" + xbToStr si.gtop + "  goto ->" + xToStr si.dest + fsm_arcwork
    "FSM_arc: " + xbToStr si.gtop + " " + xToStr si.pc + " " + xToStr si.resume + "->" + xToStr si.dest + fsm_arcwork

and fgisOrdToStr ((f, gis), ord) =  fgisToStr(f, gis) + ":" + orderToStr_opt ord 

and xb1ToStr g = if g=X_true then "" else xbToStr g

and xrtlToStr rtl =
    match rtl with
        | XRTL(pp, g0, lst) ->
            let rtlopToStr = function
                | Rarc(gg, l, r) when pp<>None && fst(valOf pp)=l ->
                    if xi_istrue gg then " GOTO " + xToStr r
                    else (sprintf "if (%s) then goto %s " (xb1ToStr gg) (xToStr r))

                | Rarc(gg, l, r)       -> "(" + xb1ToStr gg + "):" + (if pp<>None && l=fst(valOf pp) then " goto " else "") + xToStr l + " <= " + xToStr r                
                | Rpli(gg, fgisord, args) -> "(" + xb1ToStr gg + "):" + fgisOrdToStr fgisord + "(" + sfold xToStr args + ")"
                | Rnop s -> "nop(" + s + ")"
            ppToStr pp + "(" + xb1ToStr g0 + ") --> {" + sfold rtlopToStr lst + "}" 

        | XRTL_nop(s) -> " XRTL_nop(" + s + ")"

        | XIRTL_buf(X_true, l, r) -> " XIRTL_buf " + xToStr l + " := " + xToStr r

        | XIRTL_buf(g, l, r) -> " XIRTL_bufif g=" + xb1ToStr g + " "  + xToStr l + " := " + xToStr r

        | XIRTL_pli(pp, g, fgis, args) ->
                ppToStr pp + " --> " + (if xi_istrue g then "" else " if " + xbToStr g + " then ") + fgisOrdToStr fgis + "(" + sfold xToStr args + ")"


let arc_rp__ vd m (qq:vliw_arc_t) =  // not used
           (
             if vd>=4 then vprintln 4 (m + "Report arc_rp :pc=" + xToStr qq.pc + "\n    resume point=" + xToStr qq.resume + ",\n    g=" + xbToStr qq.gtop + ",\n    suspend point=" + xToStr qq.dest + " " + sprintf "%A" qq.hard);
             if vd>=4 then vprintln 4 "Env:";
             app (fun x -> if vd>=4 then vprintln 4 ("   " + xrtlToStr x)) (qq.cmds); 
             if vd>=4 then vprintln 4 "\n";
             ()
           )




let hprSPSummaryToStr = function
    | SP_l(ast_ctrl, b)  -> sprintf "SP_l(%s, ...)" ast_ctrl.id
    | SP_asm(ar, len, _, _, _) -> "SP_asm len->" + (i2s len)
    | SP_dic(dic, ctrl)        -> "SP_dic id=" + ctrl.ast_ctrl.id + " lendic=" + (i2s dic.Length)
    
    | SP_par(md,  l) -> sprintf "SP_par(%A, <%i items>)" md (length l)
    | SP_seq(l)     -> sprintf "SP_seq(<%i items>)" (length l)

    | SP_rtl(i, lst) ->  sprintf "SP_rtl(<%s, |%i| RTL arcs>)" i.id (length lst)
    | SP_fsm(i, trans) -> "SP_fsm ..." + xToStr i.pc + " transitions=" + i2s(length trans)
    | SP_constdata d -> "SP_constdata ..."
    | SP_comment s -> "// " + s + " //" 
    //| x -> "??hprSPSummaryToStr"


let de_edge = function
    | E_pos x
    | E_neg x
    | E_any x -> x

let execSummaryToStr = function
    | H2BLK(dir, v) ->  dirToStr true dir + " directs " + hprSPSummaryToStr v



//
// Catenate serial blocks into a larger DIC.
//
let rec sp_catenate dir00 spl = 

    let (flt) =
        let rec seq_flat = function
            | SP_seq(lst)::tt -> seq_flat (lst @ tt)
            | other           -> (other)
        seq_flat spl
    let diclen = function
        | SP_dic(ar, id) -> ar.Length
        | _ -> 0
    let ohead = 5 // room for a few comments.
    let total = List.fold (fun c x -> c + ohead + diclen x) 0 flt
    let dic = Array.create total g_dic_hwm_marker
    let m_ptr = ref 0
    let insert x =
        if x<>g_dic_hwm_marker then
            let _ = Array.set dic (!m_ptr) x
            m_ptr := !m_ptr + 1
    let mid = ref "*unset*"
    let idc = function
        | SP_dic(dic, ctrl) ->
            mid := ctrl.ast_ctrl.id
            insert (Xcomment ("Code catenated from dic id=" + ctrl.ast_ctrl.id))
            app (fun x -> insert (dic.[x])) [0..(dic.Length-1)]
            insert (Xcomment ("End of dic id=" + ctrl.ast_ctrl.id))
            ()
    app idc flt
    let dic = SP_dic(dic, { g_null_dic_ctrl with ast_ctrl={ g_null_ast_ctrl with id="extended-from-" + !mid}} ) // Ctrl aspects will be lost.
    (dir00, dic)

//
// Ideally, RTL generation uses only straightforward combinational and sequential assigments, with the sequential assignments being non-blocking in Verilog terms, so the order of listing of either is unimportant.
// In the past, we have sometimes used the blocking assignment form for sequential logic, but this is deprecated in most scenarios.
//
// This function finds sequential and combinational assignments within hbev code on the assumption that the X_x is on the lhs.  (Negated values of n might denote the same information on the rhs but we assume these are not being used.) 
//
// We need to use always_comb forms instead of XIRTL_buf with Verilog output in certain cases, such as when we want it to be associated with an fsm state or rendered as a V_SWITCH.
// Combination expression arises when a non-blocking assigns are hoisted above all readers within the same always block if only read in that block,
// or they may be split into their own fully supported blocks to let the EDS/Verilog scheduler implement the ordering.
//
let find_blka l0 =
    match l0 with
        | X_x(l, -1, _) -> (l, true)
        | X_x(l, n, _) when n <> 0 -> sf ("XIRTL_arc illegal lhs (X^n when n!=0 && n != 1): " + xToStr l0)
        | X_x(l, 0, _)
        | l ->             (l, false)

// Summary form
let fsm_arcToStr si = sprintf "FSM_arc E%i: " si.eno + xToStr si.pc + " ===  " + xToStr si.resume + " && " + xbToStr si.gtop +  "--->" + xToStr si.dest

// Full form
let fsm_arcToStr_full si = sprintf "FSM_arc E%i: " si.eno + xToStr si.pc + " ===  " + xToStr si.resume + " && " + xbToStr si.gtop +  "--->" + xToStr si.dest + " WORK=" + sfold xrtlToStr si.cmds


type anal_queries_t =
    {
        yd:              logger_t
        full_listing:    bool
        concise_listing: bool
        logic_costs:     walkctl_t option
        title:           string
        m_eisf:          bool ref option
    }

let g_null_queries = // Owing to ref cells, need to regen each call.
  {
    yd=                YO_null
    full_listing=      false
    concise_listing=   false
    logic_costs=       None
    title=             "untitled"
    m_eisf=            None
  }

        



let sdToStr (arg:sd_t) = // Subscripted list
    match arg with 
    | (x, []) -> "ALL(" + xToStr x + ")"
    | (x, lst)  -> "ITEMS(" + xToStr x + ": " + sfold i2s64 lst + ")"

let isdToStr = function // Subscripted list
    | (x, []) -> "ALL(" + xToStr x + ")"
    | (x, lst)  -> "ITEMS(" + xToStr x + ": " + sfold i2s64 lst + ")"

let rec sd_mentioned n = function
    | [] -> false
    | (DRIVE(k, g))::t -> (sd_lookup n k)<>None || sd_mentioned n t
    | (SUPPORT k)::t ->   (sd_lookup n k)<>None || sd_mentioned n t

(*
// This is the crumby old routine. sd_Union and sd_intersection are better ... Still used by cilnorm.fs
*)
let rec sd_oldmethod_support =  function
    | [] -> []
    | (SUPPORT k)::tt -> sd_union(k, sd_oldmethod_support tt)
    | _ :: tt         -> sd_oldmethod_support tt

    
let gec_Xcall(fgis, args) = Xeasc(xi_apply(fgis, args)) // Note: this uses default callflags (cf)


let augment_xrtl_guard msg gg = function
    | XRTL(pp, g, lst)               -> XRTL(pp, ix_and g gg, lst)

    | XIRTL_buf(g, lhs, rhs)         -> XIRTL_buf(ix_and g gg, lhs, rhs)

    | XIRTL_pli(pp, g, fx, args)     -> XIRTL_pli(pp, ix_and g gg, fx, args)
    
    | XRTL_nop ss                    -> XRTL_nop ss
    // _ -> sf (msg + ": augment_xrtl_guard other")
    
(*  
 * Convert xrtl to hbev (what does diosim use?).
 * Note - this will cause bugs if the a variable is both read and written in the RTL list since the hbev is an imperative program (order dependent) not a parallel (non blocking in Verilog terms) set of assignments.
 *)
let xrtl2hbev_1 arg =
    let qq gl (c0, c1, c2) = function // Reset mining is missing : should call let resets = xrtl_resets ww cmds
        // Or? return g in c0
        | Rarc(g, l, r) -> (c0, gec_Xif1 g (Xassign(l, r (* if l=pc then xi_bnum(lc_atoi r) else r*) ))::c1, c2)
        | Rnop s -> (c0, c1, c2)
        | Rpli(g, (fgis, _), args) -> 
               (c0, gec_Xif1 g (gec_Xcall(fgis, args))::c1, c2)

    let c0, c1,c2 = ([], [], []) // (guards, commands, resets)
    match arg with
        | XRTL(None, g, lst) ->
            let gl = [g]
            List.fold (qq gl) (gl, [], []) lst
        | XRTL(Some(pc, W_string(a, b, cd)), g, lst) -> // This should use  withvals !! TODO
            let gl = [xi_deqd(pc, xi_bnum(lc_atoi(W_string(a, b, cd)))); g]
            List.fold (qq gl) (gl, [], []) lst

        | XIRTL_buf(g, l, r) -> (g::c0, Xassign(l, r)::c1, c2)

        | XIRTL_pli(Some(pc, W_string(a, b, cd)), g, (fgis, _), args) -> 
               ([xi_deqd(pc, xi_bnum(lc_atoi(W_string(a, b, cd)))); g] @ c0, gec_Xcall(fgis, args)::c1, c2)

        | XIRTL_pli(Some(pc, a), g, (fgis, _), args) -> ([xi_deqd(pc, a); g]@c0, gec_Xcall(fgis, args)::c1, c2)

        | XIRTL_pli(None, g, (fgis, _), args) -> (g::c0, gec_Xcall(fgis, args)::c1, c2)
    
        | _ -> sf "xrtl2hbev other"


let xrtl_resets ww xrtl =
    let m_reset_vals = ref []
    let qor = function
        // Or? return g in c0
        | Rarc(_, lhs, rhs) ->
            match resetval_o lhs with
                | Some v ->
                    mutaddonce m_reset_vals (lhs, v)
                | None -> ()
        | _ -> ()

    let do_rr = function
        | XRTL(_, g, lst) -> app qor lst
        | _ -> ()
    app do_rr xrtl
    !m_reset_vals



let xrtl2hbev arg = 
    let (gds, cmds, reset_) = xrtl2hbev_1 arg  // Do not fold this over more than one arg! Murges guards and bodies!
    let g' = ix_andl gds
    gec_Xif1 g' (gec_Xblock cmds)



let gen_XRTL_pli =
    function
        | (pp, X_false, fgis, args) -> XRTL_nop "1p"
        | (pp, g, fgis, args)       ->
             let g1 = if pp=None then [] else [ xi_deqd(fst(valOf pp), snd(valOf pp))]
             let g = if g1=[] then g else simplifyb_ass_b g1 g_maxdepth g
             if g=X_false then XRTL_nop "2p"
             else 
             let gl = g::g1
             let args' = map(simplify_ass_b gl g_maxdepth) args
             XIRTL_pli(pp, g, fgis, args')

// X_n on the lhs and rhs alters whether combinational, sequential, or multiple sequential delays are required.
//    
let degen_XRTL_arc = function
    | (pp, X_false, l, r) -> XRTL_nop "3p"
    | (pp, g, l, r0) ->
             let w = xi_width_or_fail "gen_XRTL_arc" l
             let g1 = if pp=None then [] else [ xi_deqd(fst(valOf pp), snd(valOf pp))]
             let g = if g1=[] then g else simplifyb_ass_b g1 g_maxdepth g
             if xi_isfalse g then XRTL_nop "4p"
             else 

             let gl = g::g1
             let r = simplify_ass_b gl g_maxdepth r0
             // ditto lhs simplify_lmode
             if false then
                      (
                       vprintln 0 ("+++ XIRTL_arc gen " + xToStr l + " with g=" + sfold xbToStr gl + "; r=" + netToStr r + " w=" + i2s w);
                       reportx 0 "clauses"  xbToStr (xi_valOf(xi_clauses (Xist []) g))
                      )

             XRTL(pp, X_true, [Rarc(g, l, r)])

// Please explain difference beteen gen and degen !
             
let gen_XRTL_arc(pp, g, ll, rr) = if xi_isfalse g then XRTL_nop("gen_XRTL_arc false " + xToStr ll) else XRTL(pp, g, [Rarc(X_true, ll, rr)])


(*
 * This walker does the subshare tallying, area estimation, delay estimation and does conerefine tallies too: generic walker.
 *) 
let rec walk_sp ww vdp aux dir xX =
    let (m, sWALK, cost_tallies, wlk_plifun) = aux
    let costlog (cost:walker_nodedata_t) =
        match cost_tallies with
            | None -> cost
            | Some m_costlist ->
                mutadd m_costlist (m, cost)
                cost

    let pli_log0 soninfo arg = wlk_plifun soninfo arg

    let el (pp, g) l r = ignore(costlog(f1o3(mya_lwalk aux (pp, g) dir l r)))
    let e0 x  = costlog(mya_walk_it aux g_null_directorate x)     // We pass in null directorate
    let e x  = ignore(e0 x)
    let eb x = ignore(costlog(mya_bwalk_it aux g_null_directorate x))    // at these two sites because we are not expecting sequential logic inside a valOf block. Insufficient reason!

    let wlk4 (pp, g0) arg =
        let _ = eb g0 // This callback was missing until March 2017!
        match arg with
            | Rpli(g, fgis, args) -> (eb g; pli_log0 (map e0 args) (XIRTL_pli(pp, ix_and g g0, fgis, args)))
            | Rnop s -> ()
            | Rarc(g, l, r)       -> (eb g; e r; el (pp, ix_and g g0) l r)  // Do rhs before lhs.

    match xX with
    | SP_rtl(rtlctl, xrtl) ->
           let vd = 3
           let walk_xrtl aA =
                //dev_println (sprintf "walk Xrtl  %A" aA)
                let ans = 
                    match aA with
                        | XRTL(pp, g0, lst)            -> app (wlk4 (pp, g0)) lst
                        | XIRTL_buf(g, l, r)           -> (eb g; e r; el (None, g) l r) // Do rhs before lhs.
                        | XIRTL_pli(pp, g, fgis, args) -> (eb g; pli_log0 (map e0 args) aA)
                        | XRTL_nop _ -> ()
                        //| other -> sf("walk_sp xrtl walk_xrtl other: " + xrtlToStr other)
                //dev_println (sprintf "walked Xrtl  %A" aA)
                ans
           app (fun x->(walk_xrtl x; ())) xrtl


    | SP_fsm(i, states) ->
        let walk_xrtl = function
            | XRTL(pp, g0, lst)            -> app (wlk4 (pp, g0)) lst  
            | XIRTL_buf(g, l, r)           -> (eb g; el (None, g) l r; e r)
            | XIRTL_pli(pp, g, fgis, args) -> (eb g; app e args)
            | XRTL_nop _ -> ()
            //| other -> sf("walk_sp fsm walk_xrtl other:" + xrtlToStr other)

        let fsm_state_walk (si:vliw_arc_t) =
            if vdp then vprintln 3 (sprintf "SP_fsm walk state %s" (xToStr si.resume))
            app walk_xrtl si.cmds
            ()

        app fsm_state_walk states
        
    | SP_dic(dic, ctrl) -> 
        let ww' = WN ("SP_dic " + ctrl.ast_ctrl.id) ww 
        let j p = walk_bev aux dir dic.[p]
        let _ = mya_bwalk_it aux dir // { clkinfo with ctrl.clken }
        // Modified dic contents are not kept! Use rewrite_h2sp instead if you want the nas
        let rec kdic pc =
            if pc=dic.Length then ()
            else
                ignore(j pc)
                if vdp then vprintln 3 (sprintf "SP_dic walk line %i" pc)
                kdic (pc+1)
        kdic 0
        ()

    | SP_comment l -> ()
    | SP_l(ast_ctl, cmd) -> ignore(walk_bev aux dir cmd)


    | SP_seq(l)         -> (app (walk_sp ww vdp aux dir) l; ())
    | SP_par(pstyle, l) -> (app (walk_sp ww vdp aux dir) l; ())
    | other -> sf("other walk_sp: " + hprSPSummaryToStr other)


let walk_exec ww vdp aux = function
    | (H2BLK(dir, arg)) -> ((walk_sp ww vdp aux dir arg):unit)

let rec walk_vm ww vdp (aux) = function
    | (idl, None) -> ()

    | (idl, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->        
             let (m, sWalk, cost_tallies, wlk_plifun) = aux
             let m1 = m + vlnvToStr minfo.name
             let ww' = WF 3 "walk_vm" ww m1
             ignore(app (walk_exec ww' vdp aux) execs)
             app (walk_vm ww' vdp aux) sons
             ()

// logic_cost expressions. Please rename
// bevelab use only... should be general and accesible via anal which it is
// >H
let bevelab_xi_cost cost_queries exp =
    match cost_queries.logic_costs with
        | None -> sf "sp_logic_cost_c: no cost set"
        | Some logic_cost_set ->
            let m_costup = ref []
            let wlk_plifun = ref []            
            let aux = ("bevelab_xi_cost", logic_cost_set, Some m_costup, wlk_plifun)
            match mya_walk_it aux g_null_directorate exp with
                | ACOST cv        -> cv
                | COST_NODE(v, _) -> CV_single(int64 v)
                | tother -> sf (sprintf "tother bevelab_xi_cost %A" tother)

let g_null_pliwalk_fun _ _ = ()


// logic cost for SP blocks: Please rename 
let bevelab_sp_logic_cost ww vdp cost_queries arg =
    let m = "bevelab.pli_cost"
    let ans:costvec_t =
        match cost_queries.logic_costs with
            | None -> sf "sp_logic_cost_c: no cost set"
            | Some logic_cost_set ->
                let m_cost_tallies = ref []
                let _ = walk_sp ww vdp (m, logic_cost_set, Some m_cost_tallies, g_null_pliwalk_fun)  g_null_directorate arg
                let costup cc (m, cv) =
                    match cv with
                    | ACOST cv         -> costvec_parallel_combine cc cv
                    | COST_NODE(v, _)  -> costvec_parallel_combine cc (CV_single(int64 v))
                    | other -> sf(sprintf  "costvec _costup ombine other form%A" other)
                List.fold costup (CV_single 0L) !m_cost_tallies
    ans





//-----------------------------------------------------------------------------------------------
(*
// Symbolically handle one form of parallelism  propagating constants between threads : iterate ?
// TODO: Compare with compose.fs
*)
let rec opath_propagate_serf ww op_args = function
    | (sl, None) -> (sl, None)
        
    | (sl, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
           let _ = muddy "I was not in use?"
           let ww' = WN "serf" ww

           let k2 x = x
           let k1 (H2BLK(ck, v)) = H2BLK(ck, k2 v)

           let execs' = map k1 execs
           let sons' = map (opath_propagate_serf ww' op_args) sons

           (sl, Some(HPR_VM2(minfo, decls, sons', execs', assertions)))


(*
 * Inter-thread constant propagate.
 *)
let opath_propagate ww op_args vms = 
    //let (msg, control, c2) = op_args
    let ans = map (opath_propagate_serf ww op_args) vms
    ans


let _ = install_operator ("propagate","Inter-thread constant optimiser", opath_propagate, [], [], [])

let g_metastate = 10000000 //  Marker value - added to the index of a DIC as an embedded flag in some Xwaituntil behaviours etc..

// Return all succesors pc values for a state in assembled DIC
let rec dic_successors p = function
    | Xgoto(_, k)        -> [!k]  
    | Xbeq(X_true, _, k) -> [p+1]
    | Xbeq(_, _, k)      -> [p+1; !k]        
    | Xfork(_, k)        -> [p+1; !k]
    | Xwaituntil(_)      -> [p;  p+1; p+g_metastate]
    | Xannotate(_, s)    -> dic_successors p s
    | (_) -> [p+1]


(*
 * Collate the lables in the hbev_t Xcode.
 *)
let collate_labels (dic:hbev_t array) len =
    let fbd cc = function
        | Xlabel ss -> if memberp ss cc then sf("collate_labels: label defined mto" + ss) else singly_add ss cc
        | _ -> cc
    let rec walk pos c = if pos=len then c else walk (pos+1) (fbd c dic.[pos])
    walk 0 []


(*
 * Collate the DIC transition matrix.
 *)
let collate_dic_xition_function (dic:hbev_t array) =
    let len = dic.Length
    let rec mine_for_xition cc pc =
        if pc < 0 || pc >= len then cc else
        let cc = 
            let yielder go destref = 
                if !destref = -1 then (pc, -1, go)::cc // An explicit branch to -1 may be worth noting, but is not used I think.
                elif !destref < 0 then cc // unreachable code is not assembled and will have more negative labels
                    //sf (sprintf "pc=%i collate_dic_xition: not assembled - L696: lab=%s dest=%i" pc lab !destref)
                else (pc, !destref, go) :: cc // tag with false since not unconditional jump.

            match dic.[pc] with 
                | Xreturn arg_->
                    (pc, len, None) :: cc // Xreturn nominally branches to exit label (at len) where further code may be concatenate in the future.
                | Xgoto(lab, destref)   -> yielder (None)   destref
                | Xbeq(g, lab, destref) -> yielder (Some(xi_not g)) destref // Negate since this is a branch on false.

                | _ -> cc // Xgoto/Xbeq are not found inside Xannotate we assert.
        mine_for_xition cc (pc+1)
    let raw_xitions = mine_for_xition [] 0
    raw_xitions


// Splitting DIC code into basic blocks requires first an analysis of each type of line, discarding whitspace and labels unused.
// Lines 
type bblock_inst_t =
    | BBC_label of int * int                       // first and last PC indecies
    | BBC_branch of int * int * bool               // first and last PC indecies and whether a fallthrough
    | BBC_action of int * int                      // first and last PC indecies
    | BBC_labs of int * int

//   
(*
 * Collate the basic block start points, transition matrix and its inverse for a DIC
 *)
let collate_dic_blocks ww vd richf msg (dic:hbev_t array) =
    let _ = WF 2 ("Collate_dic_blocks: " + msg) ww (sprintf "Start %i instructions. Enable rich blocks=%A" dic.Length richf)
    let raw_xitions = collate_dic_xition_function dic 
    let dests_in_use = List.fold (fun (m:Map<int,int>) (s, d, go) -> m.Add(d, d)) Map.empty raw_xitions
    let len = dic.Length
    let classed =
        let rec classify pc =
            if pc = len || dic.[pc] = g_dic_hwm_marker then []
            else
                let cc = classify (pc+1)
                let cmd =  dic.[pc]
                //let _ = vprintln 0 (sprintf " collate_dic_blocks %i   %s" pc (hbevToStr cmd))
                match cmd with
                    | Xlabel _ when not_nonep(dests_in_use.TryFind pc) -> BBC_labs(pc, pc) :: cc
                    | Xlabel _ 
                    | Xskip 
                    | Xcomment _ 
                    | Xlinepoint _ -> cc // discard whitespace for clarification
                    | Xreturn _
                    | Xgoto _      -> BBC_branch(pc, pc, false) :: cc
                    | Xbeq(g, _, _)-> BBC_branch(pc, pc, true)  :: cc            // Branch if false, so negate guard.
                    | other        -> BBC_action(pc, pc)        :: cc
        classify 0

    let classed =
        let rec blockup = function // Conglomorate adjacent similar pairs.
            | BBC_labs(pc1, pc2) :: BBC_labs(pc3, pc4) :: tt     -> blockup(BBC_labs(pc1, pc4)::tt)
            | BBC_action(pc1, pc2) :: BBC_action(pc3, pc4) :: tt -> blockup(BBC_action(pc1, pc4)::tt)
        // We need to respect priority when eliding branches: the latter's guard must be augmented with the negation of the formers.
        // But here, when forming the classed abstraction, we neglect guards entirely for block boundary find.
            | BBC_branch(pc1, pc2, ft1) :: BBC_branch(pc3, pc4, ft2) :: tt -> blockup(BBC_branch(pc1, pc4, ft1&&ft2)::tt)
            | h::tt -> h :: blockup tt
            | [] -> []
        blockup classed
    let _ =
        if vd >= 5 then
            let displayer item =
                let ss = sprintf "  %A " item
                if strlen ss > 100 then ss.[0..99] else ss
            reportx 5 (msg + ": ProtoBasicBlock Classifications") displayer classed

    // A basic block starts at an aligned branch/jmp destination and at the instruction after a branch (but not a jump) unless that instruction is another branch or jmp in which case we fwd_scan.
    // A rich basic block is considered to contain a pool of any number of branch instructions at the end which logically consist of a single, multi-way branch, not as separate basic blocks.
    // A fall-through transition arises from a basic block that terminates with no unconditional transfers (jumps).
    // A poor basic block either contains work and one jump or else contains no work and a branch of two (or more directions)
    let add_poor = (richf = Some false) //Default is rich blocks, but in poor mode we have these additional points where action is followed by branch.
    let bblock_starts =
        let rec st arg cc =
            match arg with
                | BBC_labs(pc1, pc2)             -> singly_add pc1 cc 
                | BBC_branch(pc1, pc2, true)     -> singly_add (pc2+1) cc
                | _ -> cc                
        List.sort(List.foldBack st classed [len])

    let fallthroughs = // The times 10 is a sort key spacer.  There is no guard on 'poor' xfer from action to branch.  TODO dont make a poor split when there's only one dest anyway.
        let rec st cc = function
            | [] -> cc
            | BBC_action(pc1, pc2)::BBC_branch(pc1', pc2', ff_flag)::tt when add_poor (* && ff_flag *) -> st ((pc1 * 10, pc1, pc1', None)::cc) (BBC_branch(pc1', pc2', ff_flag)::tt)  
            | BBC_action(pc1, pc2)::BBC_labs(pc3, pc4)::tt -> st ((pc1 * 10, pc1, pc3, None)::cc) (BBC_labs(pc3, pc4)::tt)
            | BBC_branch(pc1, pc2, true)::tt -> // Make the fallthrough sort after the xition from the same starting pc by adding 5.
                //dev_println (sprintf "rez fallthrough %i -> %i" pc2 (pc2+1))
                st ((pc1 * 10 + 5, pc1, pc2+1, None)::cc) tt // The guard for the fallthrough is unconditional and this gets reguarded below in the guards sofar mechanism.
            | _::tt -> st cc tt                
        List.sort(st [] classed)


    let bblock_starts =
        let rec find_poor_start = function
            | [] -> []
            | BBC_action(pc1, pc2)::BBC_branch(pc3, pc4, ff_flag)::tt -> pc3 :: find_poor_start tt
            | _::tt -> find_poor_start tt                
        if add_poor then List.sort(find_poor_start classed @ bblock_starts) else bblock_starts

    let _ =
        if vd >= 5 then
            let displayer (sortkey, fpc, dest, go__) = sprintf "  %i -> %i" fpc dest
            reportx 5 (msg + ": FallThroughs ") displayer fallthroughs
            vprintln 5 (msg + sprintf ": diclen=%i.   Basic block starts are " len + sfold i2s bblock_starts)

    // Number all statements with this basic block number (which will be 1-to-1 for the first instruction in a basic block).

    let numbering_lst =
        let rec genner pos current = function
            | h::tt when pos < h -> (pos, current) :: genner (pos+1) current (h::tt)
            | h::tt when pos=h   -> (pos, pos) :: genner (pos+1) pos tt
            | [] -> []
        genner 0 0 bblock_starts
    let _ =
        if vd >= 5 then
            let displayer (pc, blockno) = sprintf "  %i is in block no %i" pc blockno
            reportx 5 (msg + ": Numbering") displayer numbering_lst

    let numbering = List.fold (fun (m:Map<int, int>) (pc, bno) -> m.Add(pc, bno)) Map.empty numbering_lst

    let bblock_starts = if nullp bblock_starts || hd bblock_starts <> 0 then 0::bblock_starts else bblock_starts
    
    let collated_xitions =
        let rec collate4 lst = function
            | [] -> ([], lst)
            | h::tt ->
                let blk s = valOf_or_fail "no-src-L857" (numbering.TryFind s)                
                let (sel, lst) = groom2 (fun (sortkey, f, t, g) ->(blk f = h)) lst
                let (tx, lst) = collate4 lst tt
                ((h, List.sortWith (fun a b -> f1o4 a - f1o4 b) sel)::tx, lst)
        let (collated, left_over) =
            let add_sort_field (pc, destg, go) = (pc+10, pc, destg, go) 
            collate4 (fallthroughs @ map add_sort_field raw_xitions) bblock_starts
        if not_nullp left_over then sf (sprintf "left over xitions %A" left_over)
        collated
        
    let (fwd, back) = // Compiled forward and reverse transition functions.
        let gmap_outer ((fwd:Map<int, (hbexp_t * int) list>), (back:Map<int, int list>)) (blk, sels) =
            let gmap_inner ((fwd:Map<int, (hbexp_t * int) list>), (back:Map<int, int list>), gsofar) (sortkey, pc, dest, go) =
                // All xitions, including fall throughs, need to be reguarded by the lack of any raw_xition sofar taken (since not done earlier in collate_dic_xition_function)
                let g0 = valOf_or go X_true
                //dev_println  (sprintf "pc=%i gmap g0=%s  gsofar=%s" pc (xbToStr g0) (xbToStr gsofar))
                let g1     = ix_and g0 gsofar
                let gsofar = ix_and (xi_not g0) gsofar
                let _ = if xi_isfalse g1 then vprintln 2 (sprintf "Arc guard never taken %s /\\ %s  %i (%i) --> %i" (xbToStr g0) (xbToStr gsofar) blk pc dest)
                let _ = vprintln 3 (sprintf "gmap %s    %i (%i) ---> %i" (xbToStr g1) blk pc dest)
                let dest =
                    if dest >= len then len
                    else valOf_or_failf (fun () -> sprintf "no-dst %A" dest) (numbering.TryFind dest)
                let predget x = valOf_or_nil(back.TryFind x)
                let sucget x = valOf_or_nil(fwd.TryFind x)
                (fwd.Add(blk, singly_add (g1, dest) (sucget blk)), back.Add(dest, singly_add blk (predget dest)), gsofar)
            let (fwd, back, _) = List.fold gmap_inner (fwd, back, X_true) sels
            (fwd, back)
        List.fold gmap_outer (Map.empty, Map.empty) collated_xitions

    let _ = WF 2 ("Collate_dic_blocks: " + msg) ww (sprintf "Finished")
    (bblock_starts, numbering, fwd, back)


// The postdominator of the exit node is itself.
// For all other nodes, set all nodes as the postdoms, then
// repeat until close, for each pc except for exit, PostDom(pc) = [pc] @ list_Intersection(PostDom(p) for all p in succ pc)
let find_cfg_postdoms ww vd msg (dic:hbev_t array) =
    let len = dic.Length
    let richf = None
    let (bblock_starts, numbering, fwd, back) = collate_dic_blocks ww vd richf msg dic
    let succget pc = map snd (valOf_or_nil(fwd.TryFind pc))
    let postdoms = Array.create (len+1) bblock_starts
    Array.set postdoms len [len]
    let changed = ref true
    let gpredom pc =
        if pc >= len then ()
        else
            //let _ = vprintln 3 (sprintf "compute gpredom %i" pc)
            let succdom succ = postdoms.[succ]
            let nv = List.sort(pc :: list_Intersection(map succdom (succget pc)))
            if nv <> postdoms.[pc] then
                changed := true
                Array.set postdoms pc nv
                ()

    let rec iterate_until_closure cycle_no =
        if vd>=4 then vprintln 4 (msg + sprintf ": postdoms: Iterate cycle %i" cycle_no)
        changed := false
        app gpredom bblock_starts
        if !changed then iterate_until_closure (cycle_no+1)
    iterate_until_closure 0

    let backedge_nodes =
        let m_bens = ref []
        let rec depthfst stack bno =
            //let _ = vprintln 3 (sprintf "backedge scan %i stack=%s" bno (sfold i2s stack))
            if memberp bno stack then
                mutaddonce m_bens bno
                ()
            else
                let sucs = succget bno
                app (depthfst (bno::stack)) sucs
                ()
        depthfst [] 0
        !m_bens
        
    let _ =
        if vd >= 3 then
            let serf n = (n, valOf_or_nil(fwd.TryFind n))
            let displayer (p, lst) =
                let bd (g, d) = (if g=X_true then "" else xbToStr g + "/") + i2s d
                i2s p + " :: " + sfold bd lst
            reportx 3 (msg + ": Successor Block Report") displayer (map serf bblock_starts)
            let serf n = (n, if n<0 || n>len then [] else postdoms.[n])
            let displayer (p, lst) = i2s p + " :: " + sfold i2s lst
            reportx vd (msg + sprintf ": Postdominates Report. diclen=%i" len) displayer (map serf bblock_starts)
            let displayer p = i2s p
            reportx 3 (msg + sprintf ": Backedge Report. diclen=%i" len) displayer backedge_nodes

    (bblock_starts, numbering, fwd, back, postdoms, backedge_nodes)


// for fsemsToStr you can also use
//    let toks = meta_fsem_ats "res2" thread_fsems
//    (sfold (fun (a,b) -> sprintf "%s:%A"  a b) toks))
let fsemsToStr fsems =
    let a1 = if fsems.fs_eis then "EIS " else ""
    let a2 = if fsems.fs_yielding then "YIELDING " else ""
    let a3 = if fsems.fs_inhold then "INHOLD " else ""
    let a4 = if fsems.fs_outhold then "OUTHOLD " else ""
    let a5 = if fsems.fs_mirrorable then "MIRRORABLE " else ""
    let a6 = if fsems.fs_asynch then "ASYNCH " else ""    
    a1 + a2 + a3 + a4 + a5 + a6

let signatureToStr saz =
    sprintf " has_hls_signature is_fu=%A fsems=%s" saz.is_fu (fsemsToStr saz.fsems)

let signature_oToStr = function
    | None -> ""
    | Some saz -> signatureToStr saz


(*----------------------------------*)
(*
 * Analyser and reporter:
 * Analyse: does printing and records a driven/support digest in sh and can compute logic costs.
 *
 * (We have three VM walkers in this file, which is a bit much: anal, rewrite and walk).
 *)
let anal_dic_factory ww queries yd m_support_ledger addo pline support_note =


    let anal_dic_line concisef pos mm cmd = 
        let driven = if m_support_ledger=None then [] (* yuck*) else xi_driven_bev [] cmd
          //vprintln 0 (gc_stats() + " anal " + i2s pos) // vd=0 means Flush to log
        let driven_note j = 
              if j = [] then ()
              else
                  // yout(vd, (sfold (fun (N, sk)->xToStr N) j) + " is a DRIVEN node wrote\n");  
                  addo false (DRIVE(j, mm))

        driven_note driven 
        let support = if nonep m_support_ledger then [] (* YUCK*) else xi_support_bev [] cmd
        support_note support
        let markerf = function
            | Xgoto(lab, dr) when !dr = pos+1 -> "--" // Flag simple goto's to next line
            | _                               -> "  "
              
        let commenty = function // Skip certain lines in concise mode
            | Xcomment _   -> true
            | Xlinepoint _ -> true
            | Xskip        -> true
            | other -> false
        let label = function
            | Xlabel _     -> true
            | other        -> false
        let printme = yout_active yd && ((not concisef) || (not (commenty cmd)))


        match cmd with
            | Xassign _ ->  // TODO: collect EIS on qualified severities as elsewhere.
                            // TODO: ditto EIS note for XRTL
                            // TODO: check eis apply made in r-value expressions
                            // TODO: check all the other fsem properties.
                  if not_nonep queries.m_eisf then (valOf queries.m_eisf) := true
            | _ -> ()
                  
        if printme then yout yd ( i2s pos + ":" + (if label cmd then "" else "      ") + hbevToStr_cleaned  (markerf cmd) cmd + (if label cmd then "\n" else "\n"))
        //vprintln 30 (gc_stats() + " anal " + i2s pos + " " + i2s(length driven) + "/" + i2s(length support) + " done")
        ()

    let rec anal_dic0 concisef id mm (ar:hbev_t array) pos =
        if pos >= ar.Length
        then pline("----------------------- end of " + id)
        else
            (
                if pos=0 then pline("SP_DIC listing: id=" + id);
                anal_dic_line concisef pos mm (ar.[pos]);
                anal_dic0 concisef id mm ar (pos+1)
            ) 
    let anal_hbev ww n_ mm  line = anal_dic_line false mm line 

    let anal_dic ww mm dic =
        match dic with
            | SP_dic(ar, ctrl) ->
                if queries.full_listing then anal_dic0 false ctrl.ast_ctrl.id mm ar 0 // full listing
                if yout_active yd then
                    if queries.concise_listing then yout yd ( "Concise listing:\n")
                if queries.concise_listing then anal_dic0 true ctrl.ast_ctrl.id mm ar 0
                ()
    (anal_hbev, anal_dic)


let anal_block ww m_support_ledger queries block =
    let yd = queries.yd
    let vdp = false
    let pline ss = youtln yd ss
    let addo rtl_style d =
        match m_support_ledger with
            | None -> ()
            | Some m_a -> mutadd m_a (rtl_style, d)
    let support_note j = 
        if j=[] then ()
        else
            //vprintln 0 ("Support note " + sfold (fun(N,sk)->xToStr N) j + " is read")
            // yout vd ( sfold (fun(N,sk)->boolToStr (xi_constantp N)) j + " is const ?\n"); 
// TODO: Please rewrite these to use sd_union and then implement sd_union as a map itself.
            addo false (SUPPORT j)  // RTL style marker does not matter for r-values

    cassert(queries.full_listing || queries.concise_listing, "anal (currently) needs at least one listing mode")

    // RTL arcs have an implied X_x on the lhs that we must make explict here.
    let rec xf3 v = (v, []) (* all bit lanes *)


    and xrtl_drive_anal arc = // This code needs combining with walk_sp code to avoid redundancy.
        //dev_println ("  xrtl_anal " + xrtlToStr arc + "\n")
        if yout_active yd then yout yd ( "      " + xrtlToStr arc + "\n")
        let arcs = function
            | Rarc(g, l, r) -> (addo true (DRIVE([xf3(gec_X_x(l, 1))], None)); support_note (xi_lsupport (xi_bsupport [] g) l); support_note(xi_support [] r))
            | Rnop s -> ()
            | Rpli(g, fgis, lst) -> support_note (List.fold xi_support (xi_bsupport [] g) lst)
        match arc with
               | XIRTL_pli(None, g, fgis, lst) ->  support_note(List.fold xi_support (xi_bsupport [] g) lst)

               | XIRTL_pli(Some(pc, s), g, fgis, lst) -> support_note (List.fold xi_support (sd_add (sd_add  (xi_bsupport [] g) (xf3 s)) (xf3 pc)) lst)

               | XIRTL_buf(g, l ,r) -> (addo true (DRIVE([xf3(gec_X_x(l, 1))], None)); support_note (xi_lsupport (xi_support (xi_bsupport [] g) r) l))

               | XRTL(None, g, lst) -> (support_note(xi_bsupport [] g); app arcs lst)
               | XRTL(Some(pc, v), g, lst) -> (support_note(xi_bsupport (List.fold xi_support [] [pc; v]) g); app arcs lst)                

//               | XIRTL_arc(Some(pc, s), g, l ,r) -> (addo true (DRIVE([xf3(xi_x(l, 1))], None)); support_note (sd_add(xf3 pc, sd_add(xf3 s, xi_lsupport (xi_support (xi_bsupport [] g) r) l))))

               | XRTL_nop _ -> ()
//             | (_) -> sf "xrtl_anal other: should be using xi"

    let (anal_hbev, anal_dic) = anal_dic_factory ww queries yd m_support_ledger addo pline support_note

    let support_note_do = function
        | dir ->
            support_note(List.fold xi_support [] (map de_edge dir.clocks)) // 
            support_note(List.fold xi_support [] (map f3o3 dir.resets))
            support_note(List.fold xi_support [] dir.clk_enables)
            //support_note(List.fold xi_support [] dir.handshakes)

    let rec anal_sp_2 mm director level = function
        | SP_l(ast_ctl, l) ->
            //if yout_active yd && not_nonep director then yout yd (sprintf "SP_l (hbev code).  DIR=%s\n" (dirToStr true (valOf director)))
            anal_hbev ww false 0 mm l
        | SP_dic(ar, ctrl) ->
            //if yout_active yd && not_nonep director then yout yd (sprintf "SP_dic  DIR=%s\n" (dirToStr true (valOf director)))
            anal_dic ww mm (SP_dic(ar, ctrl))
        | SP_rtl(rtlclt, lst) ->
            let ll = length lst
            // Preferably to elide RTL arcs for this director printing operation.
            //if ll >= 1 && yout_active yd && not_nonep director then yout yd ( sprintf "SP_rtl %i RTL statements.  Director=%s\n" ll (dirToStr true (valOf director)))
            app xrtl_drive_anal lst

        | SP_comment(ss) -> pline ss

        | SP_fsm(i:fsm_info_t, arcs) -> 
            // TODO (map xi_bsupport (dir.clk_enables @ dir.resets)  ordered sd list?
            addo true (DRIVE([xf3 i.pc], None))
            if yout_active yd then
                yout yd (sprintf "\nStart listing FSM of %i states and %i edges\n" (length i.resumes) (length arcs))
                yout yd ("pc=" + xToStr i.pc + ", controllerf=" + boolToStr i.controllerf + "\n")
            let fsm_arc_yd (si:vliw_arc_t) =
                if yout_active yd then pline(fsm_arcToStr si)
                app xrtl_drive_anal si.cmds
                ()
            app fsm_arc_yd arcs
            if yout_active yd then yout yd ( "End listing FSM of " + i2s(length arcs) + " arcs\n")
            ()

        | SP_asm(ar, len, p, q, obj) -> muddy "anal_asm false (ar, len, p, q)"
        | (_) -> sf("anal_sp_2 other SP form")

    let rec anal_sp_1 mm director level = function                      
        | SP_seq(lst) ->
            if yout_active yd then yout yd (sprintf "SP_seq at level %i of %i items\n" level (length lst))
            app (anal_sp_1 mm director (level+1)) lst
        | SP_par(pstyle, lst) ->
            if yout_active yd then yout yd (sprintf "SP_par at level %i of %i items\n" level (length lst))
            if yout_active yd then yout yd ( "SP_par of " + i2s(length lst) + " items\n")
            app (anal_sp_1 mm director (level+1)) lst
        | other ->
            anal_sp_2 mm director level other
    

    let anal_sp_0 mm director sp =
        let wlk_plifun_ _ _ = ()
        if yout_active yd then yout yd (sprintf "Execs: DIR=%s: " (dirToStr true director))
        support_note_do director
        let ans = anal_sp_1 mm director 0 sp
        match queries.logic_costs with
            | None -> ()
            | Some logic_cost_set ->
                    let m_cost_tallies = ref []
                    let _ = walk_sp ww vdp (mm, logic_cost_set, Some m_cost_tallies, wlk_plifun_) g_null_directorate sp
                    let msg = if nonep mm then "blank" else xToStr(valOf mm)
                    let costup cc (m, cv) =
                        let _ =
                            match cv with
                                | COST_NODE(i, sfg) when i=0 && msg = "blank" ->  () // Supress totally blank costs.
                                | _ ->
                                    //if yout_active yd then pline(sprintf "Cost at %s is %s" msg (qToStr cv))
                                    ()
                        match cv with
                        | ACOST cv         -> costvec_parallel_combine cc cv
                        | COST_NODE(v, _)  -> costvec_parallel_combine cc (CV_single(int64 v))
                        | other -> sf(msg + sprintf  ": costvec costup combine other form %A" other)
                    let ans = List.fold costup (CV_single 0L) !m_cost_tallies
                    if yout_active yd then pline(msg + ": LOGIC_COST: subtotal is " + costToStr ans)
                    // This cost is only being reported to the log and not returned as needed - hence we have bevelab_xi_cost for now.
                    ()
        ans
    let anal_g = function
        | H2BLK(dir2, v) -> 
            //let ss = dirToStr true dir
            let mm = None
            anal_sp_0 mm (dir2) v
            ()
    anal_g block

let indent pos =
            let rec indent x = if x>=pos then "" else ("-- ") + (indent(x+1))
            indent 0


// Heirarchy display in summary form from the top:
let little_hdisp yd pline machines = 

    let keyer iinfo = (if iinfo.definitionf then "D" else "I") + (if iinfo.nested_extensionf then "-e " else "   ")
    let rec hdisp pos prefix = function
        | (iinfo:vm2_iinfo_t, None) ->
            let id = sprintf "prefix=%s  kind=%s definitionf=%A" (hptos prefix) (vlnvToStr iinfo.vlnv) iinfo.definitionf
            if yout_active yd then pline(keyer iinfo + indent pos + id + ": No implementation of this machine provided here." + ii_summary iinfo)
            () // for now
        | (iinfo, Some vm2) -> hdisp_mc pos (iinfo.iname :: prefix) (iinfo, vm2)

    and hdisp_mc pos prefix = function
        | (iinfo, HPR_VM2(minfo, decls, children, execs, assertions)) ->
            //let k_ l = "/" + i2s(length l)
            if yout_active yd then
                // The prefix is not relevant for a machine definition (only for an instance) so perhaps don't print it.
                let id = sprintf "prefix=%s  name=%s" (hptos prefix) (vlnvToStr minfo.name) + (if minfo.squirrelled<>"" then " squirrel=" + minfo.squirrelled else "")
                pline(keyer iinfo + indent pos + id + sprintf " no of (children,decs,execs,asserts)=(%i,%i,%i,%i)" (length children) (length decls) (length execs) (length assertions))
            app (hdisp (pos+1) prefix) children


    if nullp machines then pline("No VM2 machines present.")
    elif yout_active yd then 
        pline("\nVM2 analyse hierarchical summary:")
        pline(sprintf "No of machines at top level is %i." (length machines))
        app (hdisp 0 []) machines
        pline ""
        pline ""        


let anal_decls_print pline msg decls =
    let rec netp vvl pi_name arg =
        let suffix =
                    match vvl with
                        | m::_ -> //(if m.is_param            then "   PARAMETER" else "") +
                                  (if m.norender            then "   NORENDER" else "") +
                                  (if m.not_to_be_trimmed   then "   NOTRIM" else "") 
                                  //(if m.h_level <> g_null_db_metainfo.h_level then sprintf "  H_LEVEL=%i" m.h_level else "")
                        | []   -> ""
        let suffix = if pi_name = [] || pi_name = [ "" ] then suffix else suffix + "   pi_name=" + hptos pi_name
                
        match arg with
            | DB_group(meta, lst) ->
                let metaprams = sfold (fun (k, v) -> sprintf "%s=%s" k v) meta.metaprams
                pline(sprintf "\nPort Group %s. Form=%A metaprams=%s" meta.kind meta.form metaprams)
                app (netp (meta::vvl) (meta.pi_name::pi_name)) lst

            | DB_leaf(Some formal, Some actual) -> pline(sprintf " %s binding .%s(%s) " msg (netToStr formal) (xToStr actual) + suffix)
            | DB_leaf(_, Some k) -> pline(sprintf "  %s "  msg + "  " + (netToStr k) + suffix) // actual or local
            | DB_leaf(Some k, _) -> pline(sprintf "  %s formal  "  msg + "  " + (netToStr k) + suffix)
            

    app (netp [] []) decls
    ()
        
let rec analyse0 ww norecurse topf title queries prefix design =
    let check ivlnv mvlnv = if ivlnv <> mvlnv then hpr_warn(sprintf "differing vm2 names:  %s cf %s" (vlnvToStr_full ivlnv) (vlnvToStr_full mvlnv))
    let pline ss = youtln queries.yd ss
    pline (sprintf "Analyse0 of %s" queries.title)
    match design with
        | (ii:vm2_iinfo_t, Some(HPR_VM2(minfo, decls, childmachines, execs, assertions))) ->
            let prefix = ii :: prefix
            let _ = check ii.vlnv minfo.name
            analyse1 ww norecurse topf title queries prefix (Some ii) (HPR_VM2(minfo, decls, childmachines, execs, assertions))
        | (ii, None) ->
            pline (ii_summary ii + " No Machine Instance or Declaration Present Here")
            let prefix = ii :: prefix
            vprintln 3 (sprintf "analyse0 - machine reference " + ii_fold prefix)
            []
        
and analyse1 ww norecurse topf title queries prefix iio vm = // Entry point - calls analyse0


    let pline ss = youtln queries.yd ss
    match vm with
        | HPR_VM2(minfo, decls, childmachines, execs, assertions) ->
            let p = []
            let m_support_ledger = ref ([]:(bool * anal_t) list)
            let id = sprintf "prefix=%s  name=%s %s" (ii_fold prefix) (vlnvToStr minfo.name) (signature_oToStr minfo.hls_signature)
            let ww = WN ("analyse1 " + id) ww
            let nodes =

                let rec nadd (cc:Set<hexp_t>) = function
                    | DB_leaf(Some formal, _) -> 
                        if Set.contains formal cc then
                            vprintln 1 (sprintf "+++ formal net %s is declared more than once (dmto) in %s." (netToStr formal) id)
                            cc
                        else cc.Add formal

                    | DB_leaf(None, Some nv) ->
                        if constantp nv then cc // Parameter overrides appear (or used to?) in gdelcs list and we avoid recording them as dmto with this little filter.
                        elif Set.contains nv cc then
                            vprintln 1 (sprintf "+++ net %s is declared more than once (dmto) in %s." (netToStr nv) id)
                            cc
                        else cc.Add nv
                    | DB_group(_, lst) -> List.fold nadd cc lst
                List.fold nadd Set.empty decls

                        
            if yout_active queries.yd then pline("\n\n\nVM2 analyse:\ntitle=" + id + (if minfo.squirrelled<>"" then " squirrel=" + minfo.squirrelled else ""))
            
            match iio with
                | Some ii ->
                    if yout_active queries.yd then pline (ii_summary ii)
                    vprintln 3 (sprintf "ii_prefix=" + ii_fold prefix)
                | None -> ()
            if yout_active queries.yd then
                let mdesc_report (aid, md) = pline(hptos aid + ": " +  mdToStr md + "\n")
                pline("Memory region descriptions (memdecs) no=" + i2s (length minfo.memdescs))
                app mdesc_report minfo.memdescs            


                let attout a = pline("Machine attribute:" +  atToStr a)
                app attout (minfo.atts)
                pline(id + ": Machine  " + (i2s(length execs))  + " rules")
                app (fun x -> pline("      " + execSummaryToStr x)) execs
            
                pline(id + ":          " + (i2s(length decls))  + " top-level terminal groups")
                pline(id + ":          " + (i2s(length childmachines))  + " children")

                anal_decls_print pline " machine decl "  decls
//            let rec dcheck = function
//                | []   -> ()
//                | h::t -> (if memberp h t then
//            let _ = dcheck nodes

            let yd' = if false then (yout queries.yd ("Skipping anal listing.\n"); YO_null) else queries.yd
            if nullp execs then pline "No execs to report" else app (anal_block ww (Some m_support_ledger) queries) execs
            let kc = if norecurse then [] else map (analyse0 ww norecurse false title { queries with yd=yd' } prefix) childmachines
            !m_support_ledger @ list_flatten kc

let analyse ww norecurse title logger concisef costsf report_areaf  machine =
    let queries =
        { g_null_queries with
            yd=logger
            concise_listing=concisef
            full_listing=not concisef
            logic_costs= (if costsf then Some(logic_cost_walk_set_gen ww 3 "analyse" report_areaf) else None)
        }
    analyse1 ww norecurse true title queries [] None (machine)


let hblockToStr ww (block) = 
    let m_support_ledger = None (* ref ([]:anal_t list) *)
    let yd = YOSL("hblockToStr", ref [])
    let queries =
        { g_null_queries with
            full_listing=    true
            concise_listing= true
            yd=              yd
        }
    anal_block ww m_support_ledger queries block
    "H2BLK(" + yoslToStr yd + ")"


let spToStr ww x =
    let m_support_ledger = None (* ref ([]:anal_t list) *)
    let yd = YOSL("spToStr", ref [])
    let queries =
        { g_null_queries with
            full_listing=     true
            concise_listing=  true
            yd=               yd
        }

    let _ = anal_block ww m_support_ledger queries (H2BLK(g_null_directorate, x)) // A nasty way to do it - why?
    yoslToStr yd

(*----------------------------------*)
(*
 * Absract machine compiler
 *)

// Directly indexed code (DIC) form.
type grecord_t = // A section of assembler/fortran-like imperative code.  This would better be an object with an emit method.
    {
        id:       string
        idx:      int ref
        limit:    int ref
        DIC:      hbev_t array ref
        labs:     OptionStore<string,int>    
        labprefix:string
    }

let new_gdic id labprefix =
    let starting_size = 256
    let DIC = Array.create starting_size g_dic_hwm_marker
    { limit=ref starting_size; labs=new OptionStore<string, int>("labs2"); idx=ref 0; DIC=ref DIC; id=id; labprefix=labprefix}


// Store an hbev command in the a DIC
let G_idx (G:grecord_t) = i2s(!G.idx) 

let G_emit (G:grecord_t) x = 
    let m_idx = G.idx 
    let _ =
        if !m_idx = !G.limit then
                G.limit := 2 * !G.limit
                let dest = Array.create !G.limit g_dic_hwm_marker
                let _ = for i in 0 .. !m_idx-1 do Array.set dest i ((!G.DIC).[i]) done
                G.DIC := dest
    //let _ = assertf(!idx < G.limit) (fun () -> (G.id) + ": Too much VM code generated: limit reached: " + i2s G.limit)
    Array.set !G.DIC !m_idx x
    //dev_println (sprintf "G_emit at %i . EOM=%A" !m_idx (x=g_dic_hwm_marker))
    m_idx := !m_idx + 1


let G_emitan (G:grecord_t) an x = G_emit G (if nullan an then x else Xannotate(an, x))


(* Goto will create a goto if the label is a backwards ref or else
 * make a tentative goto for a label still to be defined and flag it 
 * as work to do when the label is defined.
 *)
let Wwait G c = G_emit G (Xwaituntil c)
 

let gec_Ggoto G lab = G_emit G (Xgoto(lab, ref -43))
 

let gec_Gbranch G cond lab = G_emit G (Xif(cond, Xgoto(lab, ref -42), Xskip))


let gec_Gbne G cond lab = G_emit G (Xbeq(xi_not cond, lab, ref -41))



let gec_Glabel (gG:grecord_t) lab = 
    let idx = ! gG.idx
    let _ = gG.labs.add lab idx
    //dev_println (sprintf "Emit gec_Glabel %s at %i" lab idx)
    G_emit gG (Xlabel lab)


(*
 * Important optimisation here:   for boolean a : bufif(a&e, a, 0)  means a is always zero: not if set one elsewhere that does not get trimmed out by a PRIOR propagation : order of applications matters ?
 *                                for boolean a : bufif(~a, a, 1)  means a is always one
 *
 * Clauses...
 *)
let gen_XRTL_buf = function
    | (X_true, l, r) -> XIRTL_buf(X_true, l, r)

    | (g, l, r) -> 
        let nominal = XIRTL_buf(g, l, r)
        let w = ewidth "gen_XRTL_buf" l
        let mr = xi_monkey(r)
        let clauses = [g] (* FOR NOW xi_valOf(xi_clauses (xi_nil) g) *)

        let rec l_is_member_pos = function
            | (g::t) -> x_eq (xi_blift g) l || l_is_member_pos t
            | [] -> false

        let rec l_is_member_neg = function
            | (g::t) -> x_eq (xi_blift(xi_not g)) l || l_is_member_neg t
            | [] -> false

        if (* THIS NBEEDS TO BE IN TERMS NOT CLAUSES *)
              w=Some 1 && l_is_member_pos clauses && mr=Some false
              then 
               (
                vprint 3 ("Holder trim: Replacing " + xrtlToStr nominal + " with constant assign of false.\n");
                XIRTL_buf(X_true, l, xgen_num 0)
               )
              elif w=Some 1 && l_is_member_neg clauses && mr=Some true then
               (
                vprint 3 ("Holder trim: Replacing " + xrtlToStr nominal + " with constant assign of true.\n");
                XIRTL_buf(X_true, l, xgen_num 1)
               )
              else nominal 
           



let xi_rewrite_pp e = function
    | None -> None
    | Some(a, b) -> Some(xi_rewrite_exp e a, xi_rewrite_exp e b)


//
// Apply edits (assocs) to a section of RTL.
// If we supply other than X_true as additional_g we gate the RTL.
//
let rewrite_rtl ww vd additional_g (e:meo_rw_t) xrtl c =
    let remove_undef_assigns = e.remove_undef_assigns
    //let (M, dpft, MB, dbpft, agents, rwc) = 
    if vd>=4 then vprintln 4 (sprintf "Rewrite RTL (remove_undef_assigns=%A) " remove_undef_assigns + xrtlToStr xrtl)

    let booldiv nn dd =
        match ix_bdivide nn dd with
            | Some g' ->
                //vprintln 0 (sprintf "    rewrite_rtl nn=%s  dd=%s  --> %s " (xbToStr nn) (xbToStr dd) (xbToStr g'))
                g'
            | None    ->
                //vprintln 0 (sprintf "    rewrite_rtl nn=%s  dd=%s  --> won't go " (xbToStr nn) (xbToStr dd))
                nn

    match xrtl with
    | XRTL(pp, g1, lst) ->
        let g1 = xi_assoc_bexp { g_sk_null with strict=(pp, additional_g) } e g1 // We supply strict guard of additional_g so that a 'simplify assuming' reduction can take place.
        let g1 = ix_and additional_g g1
        //let pp' = xi_rewrite_pp e pp ??? want this ?
        let sk1 = { g_sk_null with strict=(pp, g1) }

        let qf arc cc =
            match arc with
            | Rpli(g2, fgis, lst) -> 
                let g2 = xi_assoc_bexp sk1 e g2
                let sk3 = {  sk1 with strict=(pp, ix_and g1 g2) }
                let lst' = map (xi_assoc_exp sk3 e) lst
                // Agent_pli - this form is missing but we always track waypoint settings.
                if fst (fst fgis) = "hpr_KppMark" then e.waypoint := Some (lst, lst')
                let g2 = booldiv g2 g1
                Rpli(g2, fgis, lst')::cc

            | Rarc(g2_raw, l, r) ->
               let g2 = xi_assoc_bexp sk1 e g2_raw
               let sk3 = {  sk1 with strict=(pp, ix_and g1 g2) }
               let rec syndicate = function
                   | [] -> // no agents matched.
                       let l' = xi_rewrite_lmode sk3.strict e l
                       //xdev_println (sprintf "post rewrite len %A" l')
                       let r' = xi_assoc_exp sk3 e r
                       let jarc cc ((_, g3), l) =
                           let g4 = booldiv g3 g1
                           if vd>=5 then vprintln 5 (sprintf " +++ rewrite rtl  Rarc g2_raw=%s   g3=%s Final=%s" (xbToStr g2_raw) (xbToStr g3) (xbToStr g4))
                           if xi_isfalse g4 then
                               
                               cc
                           else Rarc(g4, l, r')::cc
                       List.fold jarc cc l'

                   | Agent_rtl(arcpred, premap, (premunge, postmunge_))::tt -> // why is postmunge not implemented? Not needed ever so far.
                       if arcpred (pp, g1, (g2_raw, l, r)) then  // other arc agents in tt ignored on first accepting agent.
                           let ((l1, r1), pmf) = premap sk3.strict (l, r) // premap is being called now post the map of the guards.
                           let r2 = if premunge then xi_assoc_exp sk3 e r1 else r1
                           let postr = pmf(pp, g1, (g2, l1, r2))
                           let deagent cc (g, l, r) = if xi_isfalse g || (remove_undef_assigns && xi_isundef l) then cc else Rarc(g, l, r) :: cc
                           List.fold deagent cc postr
                       else syndicate tt
                   | _::tt -> syndicate tt
               syndicate e.fagents
        let arcs = List.foldBack qf lst []
        if nullp arcs then c else XRTL(pp, g1, arcs)::c


    | XIRTL_buf(g1, l, r) -> 
        let g1 = xi_assoc_bexp { g_sk_null with strict=(None, additional_g) } e g1
        let g1 = ix_and g1 additional_g
        let sk1 = { g_sk_null with strict=(None, g1) }
        if vd>=5 then vprintln 5 ("Rewrite rtl buffer " + xrtlToStr xrtl + " with g1=" + xbToStr g1)
        let l' = xi_rewrite_lmode sk1.strict  e l
        let r' = xi_assoc_exp sk1 e r
        let j ((_, g3), l) c = if xi_isfalse g3 then c else gen_XRTL_buf(g3, l, r')::c
        let ans = List.foldBack j l' c
        //if vd>=5 then reportx 5 "Rewritten buffer(s)" xrtlToStr ans
        ans

    | XIRTL_pli(pp, g1, fgis, lst) -> // todo delete this variant of XIRTL_pli
        let g1 = xi_assoc_bexp { g_sk_null with strict=(pp, additional_g) } e g1
        let g1 = ix_and g1 additional_g
        let sk1 = { g_sk_null with strict=(pp, g1) }
        XIRTL_pli(xi_rewrite_pp e pp, g1, fgis, map (xi_assoc_exp sk1 e) lst)::c

    | XRTL_nop _ -> xrtl::c
    //| other -> sf "other in rewrite_rtl"


//let _ = xi_brewrite (makemap []) X_true

let rewrite_edge ww e = function
    | E_anystar -> E_anystar
    | E_pos x -> E_pos(xi_rewrite_exp e x)

let rewrite_reset ww e (is_pos, is_asynch, x) = (is_pos, is_asynch, xi_rewrite_exp e x)

let mapo ff = function
    | None -> None
    | Some x -> Some(ff x)
    
let rewrite_dir ww e d =
    { d with
        handshakes=      map (xi_assoc_exp g_sk_null e) d.handshakes
        clk_enables=     map (xi_assoc_exp g_sk_null e) d.clk_enables
        resets=          map (rewrite_reset ww e) d.resets
        clocks=          map (rewrite_edge ww e) d.clocks
        abend_register=  (if nonep d.abend_register then None else Some(xi_assoc_exp g_sk_null e (valOf d.abend_register)))
    }


let rec rewrite_h2sp ww vd dir g0 e arg =
    match arg with
    | SP_l(ast_ctl, b)       -> SP_l(ast_ctl, gec_Xif1 g0 (xbev_rewrite ww e b))
    | SP_par(pstyle, l)      -> SP_par(pstyle, map (rewrite_h2sp ww vd dir g0 e) l)
    | SP_seq(l)              -> SP_seq(map (rewrite_h2sp ww vd dir g0 e) l)
    
    | SP_rtl(ii, rtl)        -> SP_rtl(ii, List.foldBack (rewrite_rtl ww vd g0 e) rtl [])
    
    | SP_asm _ -> arg
    | SP_dic(dic, ctrl) -> 
        let dic' = Array.create dic.Length (Xskip)
        let ww' = WN ("SP_dic " + ctrl.ast_ctrl.id) ww 
        cassert(g0=X_true, "Guarding sp_dic not implemented")
        let m b = 
            //vprint 20 (fun()->"dic rewrite " + hbevToStr b + "\n")
            let ans = xbev_rewrite ww' e b
            ans
        let _ =
            let jrewrite p = Array.set dic' p (m dic.[p])
            app jrewrite [0..dic.Length-1]
        SP_dic(dic', ctrl)

    | SP_comment _ -> arg
    | other  -> muddy("rewrite_h2sp other form:" + hprSPSummaryToStr other)



let rewrite_option ww e = function
    | None -> None
    | Some v -> Some(xi_rewrite_exp e v)


let xruleToStr = function
    | O_ctl_AG(O_state(v, clkinfo), msg) -> "always(" + xToStr v + " msg={" + sfold (fun x->x) msg + "}, clkinfo);"
    | (O_INITIAL v) -> "initial(" + xToStr v + ");"
    | (O_IDLE v) -> "idle(" + xToStr v + ");"
    | other -> sf("xruleToStr??")


(*
 *
 *)
              

let rewrite_xrule ww e = function
    | (O_ctl_AG(O_state(v, clkinfo), msg)) ->
        let biz = function
            | None -> None
            | Some ov -> Some(rewrite_edge ww e ov)
        O_ctl_AG(O_state(xi_rewrite_exp e v, biz clkinfo), msg)

    | v -> muddy("rewrite xrule other form: " + xruleToStr v + "\n")



let rewrite_exec ww vd g0 e (H2BLK(dir, v)) = H2BLK(rewrite_dir ww e dir, rewrite_h2sp ww vd dir g0 e v)


let rec rewrite_machine ww vd e = function
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls, children, execs, assertions)))->
        let ww' = WN ("rewrite " + ii.iname) ww 
        let g0 = X_true
        let execs' = map (rewrite_exec ww' vd g0 e) execs
        let assertions' = map (rewrite_xrule ww' e) assertions 
        let children' = map (rewrite_machine ww vd e) children
        (ii, Some(HPR_VM2(minfo, decls, children', execs', assertions')))





(*
 * Routine to ensure non-referentially transparent functions are only called once.
 * We extract the call and assign it to a temporary net.
 *)
let uniquify_non_reftransp ww vd m_morenets sp =
    let _ = WF 3 "uniquify non ref transparent" ww "start"

    let temp_net prec id = // The precision here should be the result width from gis
        let width = valOf_or prec.widtho 32
        let rr = X_bnet(iogen_serf(id, [], 0I, width, LOCAL, prec.signed, None, false, [], []))
        mutadd m_morenets rr
        vprintln 3 ("UNR: Added " + netToStr rr + " to morenets")
        rr
    let m_counter = ref 0
    let MM = (makemap [])        
    //let tieup = ref MM
    let nd = ref []

    let xmapper strict x = 
         match x with
             | W_apply((f, gis), cf, args, _) -> // recurse on args already done for us? no.
                 if vd >= 5 then vprintln 5 (sprintf "UNR: W_apply operating for f=%s   void=%A" (xToStr x) (gis.rv = g_void_prec))
                 cassert(gis.fsems.fs_nonref, " xmapper UNR invoked on " + f + " which is not reftrans")
                 if gis.rv = g_void_prec then
                     let _ = mutadd nd (Xeasc(xi_apply_cf cf ((f, gis), (*map kf *)args)))
                     xi_num 0
                 else
                    let tn = temp_net gis.rv (funique(f + "_res"))
                    //let kf x = xi_assoc_exp !tieup x
                    mutadd nd (Xassign(tn, xi_apply_cf cf ((f, gis), (*map kf *)args)))
                    tn
             | _ -> sf "UNR: xmapper other"
             
    let xpred = function
        | W_apply((f, gis), _, _, _) -> (gis.fsems.fs_nonref)
        | _ -> false

    let bpred b =
        let ans =
            match b with
            | Xeasc(W_apply((f, gis), _, _, _)) -> (gis.fsems.fs_nonref)
            | Xbeq(starter, d, n) -> true
            | Xassign(l, r) -> true
            | _ -> false
        // let _ = vprintln 0 ("Decide  " + hbevToStr b + " procwrap " + boolToStr ans)
        ans
        
    let wrap b =
        //vprintln 0 ("Have " + i2s(length !nd) + " items to wrap on " + hbevToStr b)
        let r = !nd
        nd := []
        let ans = gec_Xblock(r @ [b])
        mutinc m_counter 1
        //dev_println ("UNR: Ans post wrap is " + hbevToStr ans)
        ans


    let postmap g N =
        match N with
        | Xbeq(t, d, n) -> wrap(Xbeq(t, d, n)) // We need to reassemble if use gen_Xbeq so dont

        | other -> wrap(other)

    let premap g b = (b, postmap)
 
    let MM = hexp_agent_add MM (xpred, xmapper, (true, false))
    let MM = hbev_agent_add MM (bpred, premap)        
    //tieup := MM                         
    let g0 = X_true
    let sp' = rewrite_exec ww vd g0 MM sp
    let _ = WF 3 "uniquify non ref transparent" ww (sprintf "end. xformation count = %i" !m_counter)
    sp'



let report_decl f v =
    match v with
    | X_bnet ff -> 
        let f2 = lookup_net2 ff.n
        (
          youtln f ("Declared " + (netToStr v))
          youtln f ("          Dir=" + boolToStr f2.dir)
          youtln f ("          Type=" + vartypeToStr f2.vtype)
          if (f2.length <> []) then youtln f ( "  Array length=" + i2s64(asize f2.length))
          youtln f ""
       )
    | X_net(s, _) -> yout f ("Simple net " + s + " declared\n")
    | k           -> yout f ("Other net: " + xToStr k + "\n")


let rec xb_serialise fd (N) =
    let gel (s, l, pol) = XML_ELEMENT(s, (if not pol then [] else [("pol", "neg")]), l)
    let gel_op(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o3(xToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)
    let gel_bop(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o4(xbToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)
    let gel_aop(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o3(xabToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)
    
    let gel_lp (LIN(k, lst)) = XML_ELEMENT("LIN", [ ("start", boolToStr k)], map (fun a->XML_INT (int64 a)) lst)
    let ser = xi_serialise fd
    let rec serb x =
        match x with
        | (W_bdiop(k, lst, inv, _)) -> gel_bop("BDIOP", k, map ser lst, inv)
        | (W_bnode(k, l, inv, _))   ->  gel_aop("NODE", k, map serb l, inv)
        | (W_linp(k, l, _))    -> gel("LINP", [ser k; gel_lp l], false)
        | (W_bsubsc(l, r, inv, _)) -> gel("BSUBSC", [ser l; ser r], inv)
        | (W_bmux(gg, ff, tt, _)) -> gel("BMUX", [serb gg; serb ff; serb tt], false)
        | (W_bitsel(l, n, inv, _)) -> gel("BITSEL", [ser l; XML_INT(int64 n)], inv)
        | (X_true) -> XML_ATOM (xbToStr x)
        | (X_false) -> XML_ATOM (xbToStr x)
        | (X_dontcare) -> XML_ATOM (xbToStr x)
        //| (other) -> sf ("serialise b other:" + xbkey other + " " + xbToStr other)
    serb N

and xi_serialise fd hexp =
    let serb = xb_serialise fd
    let gel (s, l, pol) = XML_ELEMENT(s, (if not pol then [] else [("pol", "neg")]), l)
    let gel_op(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o3(xToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)
    let gel_bop(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o4(xbToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)
    let gel_aop(s, oo, l, pol) = XML_ELEMENT(s, ("op", f3o3(xabToStr_dop oo)) :: (if not pol then [] else [("pol", "neg")]), l)

    let gel_lp (LIN(k, lst)) = XML_ELEMENT("LIN", [ ("start", boolToStr k)], map (fun a->XML_INT(int64 a)) lst)
    let rec ser = function
        | X_bnet ff -> gel("BNET", [XML_WS ff.id], false)
        | X_net(s, _)  -> gel("NET",  [XML_WS s], false)
        | X_undef  -> gel("UNDEF", [], false)
        | W_string(s, XS_unquoted n, _)   -> gel("STRINGSQ", [XML_WS (i2s n); XML_WS s], false)
        | W_string(s, XS_fill n, _)       -> gel("STRING", [XML_WS (i2s n); XML_WS s], false)          
        | W_string(s, XS_withval x, _)    -> gel("STRINGWV", [XML_WS s; ser x], false)
        | W_string(s, _, _) -> gel("STRING", [XML_WS s], false)          
        | X_num n -> gel("NUM", [XML_INT(int64 n)], false)
        | X_bnum(w_, l, _)         -> gel("BNUM", [XML_WS (l.ToString())], false)
        | W_asubsc(l, r, _)        -> gel("ASUBSC", [ser l; ser r], false)
        | W_query(g, l, r, _)      -> gel("Q", [serb g; ser l; ser r], false)
        | W_node(prec, k, l, _)    -> gel_op("NODE", k, map ser l, false)
        | W_valof(e, _)            -> gel("VALOF", map (serialise_bev fd) e, false)
        |  X_pair(l, r, _)         -> gel("PAIR", [ ser l; ser r], false)
        | X_x(l, n, _)             -> XML_ELEMENT("X_X", [ ("index", i2s n) ], [ ser l ])
        |  (X_blift b)             -> gel("BLIFT", [ serb b], false)
        
        |  (W_apply((f, gis), _, args, _)) -> gel("APPLY", gel("NAME", [XML_ATOM f], false) :: map ser args, false)
        |  (other) -> sf ("serialise other:" + xkey other + " " + xToStr other)
    ser hexp


and serialise_bev fd = function
    | Xassign(l, r) -> XML_ELEMENT("Xassign", [], [ xi_serialise fd l; xi_serialise fd r])
    | Xif(g, t, f) -> XML_ELEMENT("Xif", [], [ xb_serialise fd g; serialise_bev fd t; serialise_bev fd f])
    | Xbreak -> XML_ELEMENT("Xbreak", [], [])
    | Xcontinue -> XML_ELEMENT("Xcontinue", [], [])
    | Xskip -> XML_ELEMENT("Xskip", [], [])
    | Xannotate(a_, t) -> XML_ELEMENT("Xannotate", [], [serialise_bev fd t])
    | Xdo_while(g, t) -> XML_ELEMENT("Xwhile", [], [ xb_serialise fd g; serialise_bev fd t])
    | Xwhile(g, t)    -> XML_ELEMENT("Xdo_while", [], [ xb_serialise fd g; serialise_bev fd t])
    | Xwaitabs e -> XML_ELEMENT("Xwaitabs", [], [ xi_serialise fd e ])
    | Xwaitrel e -> XML_ELEMENT("Xwaitrel", [], [ xi_serialise fd e ])
    | Xwaituntil e -> XML_ELEMENT("Xwaituntil", [], [ xb_serialise fd e ])
    | Xreturn e -> XML_ELEMENT("Xreturn", [], [ xi_serialise fd e ])
    | Xblock l -> XML_ELEMENT("Xblock", [], map (serialise_bev fd) l)
    | Xpblock l -> XML_ELEMENT("Xpblock", [], map (serialise_bev fd) l)
    | Xgoto(s, _) -> XML_ELEMENT("Xgoto", [], [XML_ATOM s])
    //| Xcall((f, gis), args) -> XML_ELEMENT("Xcall", [], XML_ELEMENT("NAME", [], [XML_ATOM f]) :: map (xi_serialise fd) args)
    | Xbeq(g, s, _) -> XML_ELEMENT("Xbeq", [], [xb_serialise fd g; XML_WS s])
    | Xdominate(s, _) -> XML_ELEMENT("Xdominate", [], [XML_ATOM s])
    | Xlabel ss   -> XML_ELEMENT("Xlabel", [], [XML_WS ss])
    | Xcomment ss -> XML_ELEMENT("Xcomment", [], [XML_WS ss])
    | Xlinepoint(LP(a, b), s) -> XML_ELEMENT("Xlinepoint", [], [ XML_WS a; XML_WS(i2s b); serialise_bev fd s])
    | Xeasc s -> XML_ELEMENT("Xeasc", [], [xi_serialise fd s])
    | other -> sf("serialise_bev other " + hbevToStr other)


let ser_xrtl fd A c =
    let q (ap, ep) arc c =
        match arc with
            | Rarc(g, l, r) ->   XML_ELEMENT("XARC", ap, ep @ [ xb_serialise fd g; xi_serialise fd l; xi_serialise fd r]) :: c
            | Rpli(g, fgis, args) -> XML_ELEMENT("XPLI", ap, XML_ELEMENT("NAME", [], [XML_WS(fst(fst fgis))]):: ep @ xb_serialise fd g  :: (map (xi_serialise fd) args)) :: c
            | Rnop s -> XML_ELEMENT("XNOP", [], []) :: c

    match A with
    | XRTL(None, g, lst) ->
        let preq = ([], [ xb_serialise fd g ])
        List.foldBack (q preq) lst c

    | XRTL(Some(a, b), g, lst) ->
        let preq = ([ ("pc", xToStr a); ("vale", xToStr b) ], [ xb_serialise fd g ])
        in List.foldBack (q preq) lst c

    | XIRTL_buf(X_true, l, r) -> XML_ELEMENT("XBUF", [], [ xi_serialise fd l; xi_serialise fd r ]) :: c

    | XIRTL_buf(g, l, r) -> XML_ELEMENT("XBUFIF", [], [ xb_serialise fd g; xi_serialise fd l; xi_serialise fd r ]) :: c

    | (XIRTL_pli(None, g, fgis, args)) -> XML_ELEMENT("XPLI", [], XML_ELEMENT("NAME", [], [XML_WS(fst(fst fgis))]):: xb_serialise fd g::(map (xi_serialise fd) args)) :: c

    | XIRTL_pli(Some(a, b), g, fgis, args) -> XML_ELEMENT("XPLI", [ ("pc", xToStr a); ("vale", xToStr b) ], XML_ELEMENT("NAME", [], [XML_WS (fst (fst fgis))])::xb_serialise fd g::(map (xi_serialise fd) args)) :: c

    | XRTL_nop _ -> XML_ELEMENT("XNOP", [], []) :: c

    //| _ -> sf "serialise_sp xrtl ser_xrtl other"


let serialise_directorate ww fd dir =
    let serialise_reset (is_pos, is_asynch, x) = [  XML_WS(boolToStr is_pos); XML_WS(boolToStr is_asynch); xi_serialise fd x ]
    XML_ELEMENT("DIRECTORATE", [],
                [
                   XML_ELEMENT("CLOCKS", [],  map (xi_serialise fd) (map de_edge dir.clocks))
                   XML_ELEMENT("CLOCK_ENABLES", [],  map (xi_serialise fd) dir.clk_enables)
                   XML_ELEMENT("RESETS", [],  list_flatten (map (serialise_reset) dir.resets))
                ])



let rec serialise_sp ww fd = function
    | SP_rtl(ii, xrtl) -> XML_ELEMENT("SP_xrtl", [], List.foldBack (ser_xrtl fd) xrtl [])

    | SP_fsm(i, fsm) ->
        let ats = [ ("controllerf", boolToStr i.controllerf); ("start", xToStr(fst i.start_state)); ("pc", xToStr i.pc)  ]
        let xstates = XML_ELEMENT("FSM_statelist", [], map (fun (a, hardf) -> XML_WS(xToStr a)) i.resumes)
        let serialise_fsm_state si =
            let b = List.foldBack (ser_xrtl fd) si.cmds []
            XML_ELEMENT("FSM_state", [], XML_ELEMENT("NAME", [], [xi_serialise fd si.resume]) :: b) 
        XML_ELEMENT("SP_fsm", ats, xstates :: map serialise_fsm_state fsm)

    | SP_comment l -> XML_ELEMENT("SP_comment", [], [XML_WS l ])
    | SP_l(_,  b) -> XML_ELEMENT("SP_l", [], [serialise_bev fd b])

    // TODO serialise directorate
    | SP_seq(l)         -> XML_ELEMENT("SP_seq", [], map (serialise_sp ww fd) l)
    | SP_par(pstyle, l) -> XML_ELEMENT("SP_par", [ ("pstyle", pstyle.ToString()) ], map (serialise_sp ww fd) l)

    | SP_dic(dic, ctrl) ->
         let rec k p = if p>= dic.Length then []
                       else XML_ELEMENT("STEP", [ ("pc", i2s(p)) ], [ serialise_bev fd (dic.[p])]) :: (k (p+1))
         let items = k 0
         //  ("clken", xbToStr ctrl.clken); etc missing.
         XML_ELEMENT("SP_dic", [ ("id", ctrl.ast_ctrl.id); ("idx", i2s(dic.Length)) ], items)
         
    | other -> sf ("other serialise_sp: " + hprSPSummaryToStr other)



let serialise_blk ww fd = function
    | H2BLK(dir, b) -> XML_ELEMENT("H2BLK", [], (serialise_directorate ww fd dir) :: [serialise_sp ww fd b])


let rec serialise_vm ww fdx = function // TODO this code has rotted in recent years - not being used.
    | (_, None) ->  XML_ELEMENT("VM2", [], [])
    
    | (_, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) -> 

    let deat = function
        | Napnode(s, v) -> (s, "<node>")
        | (Nap(a, b)) -> (a, b)
    let gel s v = XML_ELEMENT(s, [], [XML_WS v])
    let geli s n = XML_ELEMENT(s, [], [XML_INT n])
    let rec netfn = function // can use IP-XACT schema here, largely!
        | DB_group(meta, lst) ->  XML_ELEMENT("spirit:signals", [], list_once(map netfn lst))
        | DB_leaf(_, Some (X_net(id, _))) ->
            XML_ELEMENT("bnet", [], [ gel "ID" id ])
        | DB_leaf(Some(X_bnet ff), Some _) -> muddy "formal+actual"
        | DB_leaf(Some(X_bnet ff), None)
        | DB_leaf(None, Some (X_bnet ff)) ->                
            let f2 = lookup_net2 ff.n
            XML_ELEMENT("bnet", list_once(map deat f2.ats),
                        [ 
                          gel "ID" ff.id
                          geli "WIDTH" (int64 ff.width)
                          gel "SIGNED" (sftToStr ff.signed)
                          gel "VTYPE" (vartypeToStr f2.vtype)
                          gel "VARMODE" (varmodeToStr f2.xnet_io)
                        ] @ 
                        (if (ff.rh > 0I) then [ gel "H" (sprintf "%A" ff.rh); gel "L" (sprintf "%A" ff.rl)] else []) @
                        (if f2.length=[] then [] else [geli "LENGTH" (asize f2.length)]))

        | DB_leaf(_, Some X_undef) -> XML_ELEMENT("undef", [], [])
        | DB_leaf(_, Some v) -> sf("xml netfn " + xToStr v)
    let dagen = function
        | Nap(a, b) -> (a, b)
        | Napnode(s, v) -> (s, "<node>")
    let ats = [ ("minfo_name", vlnvToStr minfo.name) ] @ (map dagen minfo.atts)
    let l = map netfn decls
    let s = map (serialise_vm ww fdx) sons
    let e = map (serialise_blk ww fdx) execs
    //val a = map (serialise_blk ww fdx) assertions.
    XML_ELEMENT("VM2", ats,
        [
        XML_ELEMENT("DECLS", [], l);
        XML_ELEMENT("SONS", [], s);
        XML_ELEMENT("EXECS", [], e)
        (* XML_ELEMENT("ASSERTIONSS", [], a), *)
        ])


(*
 *  Report on a machine, using analyse for execs and returning the anal list.
 *)
let rec vmreport ww xmlfix vd m_fatal_marks concisef costsf report_areaf design = 
    let id = filename_sanitize [ ] (ii_fold([fst design]))
    let ww' = WF 2 ("Writing vmreport of " + id) ww "start"
    let _ = vmreport1 (WN ("vmreport :" + id) ww') vd m_fatal_marks concisef costsf report_areaf [] design
    let _ =
        if true || xmlfix="" then ()
        else
        let filename = System.IO.Path.Combine(!g_log_dir, id + xmlfix)
        let ww' = WF 2 ("Writing xml version of " + id + " for file " + filename) ww "serialise"
        let xml = serialise_vm ww' Sparer design
        let _ = WF 2 ("Writing xml version of " + id + " to file " + filename) ww "commence"
        let _ = hpr_xmlout (WN ("xml render " + id) ww) filename true xml
        let _ = WF 2 ("Writing xml version of " + id + " to file " + filename) ww "complete"
        ()
    let _ = WF 2 ("Writing vmreport of " + id) ww "complete"
    ()


and vmreport1 ww vd m_fatal_marks concisef costsf report_areaf prefix vM =
    let pline ss = youtln vd ss
    match vM with
    | (ii, None) ->
        let prefix = ii::prefix
        let ids = ii_fold prefix
        pline("Reference to machine " + ids)
        []
        
    | (ii, Some (HPR_VM2(minfo, decls, childmachines, execs, assertions))) -> // anal seems to recurse for us
        let zerop = nullp decls && execs=[] && assertions=[]
        let prefix = ii::prefix
        let ids = ii_fold prefix
        let banner = "HPR VM2 Abstract Machine Report (AMR).  Name="  + ids + (if zerop then "(NULL MACHINE)" else "")
        let banner = if not_nullp minfo.fatal_flaws then ( mutapp m_fatal_marks minfo.fatal_flaws; banner + " FATALLY FLAWED " + sfold id minfo.fatal_flaws) else banner
        pline(banner)
        pline(ii_summary ii)
        vprintln 3 banner
        //yout vd ( "Parentname="  + parent + "\n\n")

        let _ =
            if false then // Spare declarations printing code. Mirrored in analyse.
                let rec netp indent m v = function
                    | DB_group(meta, lst) ->
                        let _ = pline(sprintf "  Portgroup=%s  kind=%s "  meta.pi_name meta.kind)
                        app (netp (indent + "...") (m + " " + meta.pi_name) (Some meta)) lst
                    | DB_leaf(Some k, _) -> pline(indent + sprintf "  " + m + "  " + (netToStr k))
                pline("Declarations:")
                app (netp "  " "" None) decls
                yout vd ("\n\n")
                ()
(*      let a = list_flatten(map (fn (iname, x) -> vmreport1 ww vd concisef (((rdot_fold iname) + ":" + rdot_fold(minfo.name)), x)) childmachines)
*)
        let norecurse = false 
        let anal = analyse ww norecurse (ids + " analysis") vd concisef costsf report_areaf (valOf(snd vM))
#if SPARE
        if report_areaf then
            vprintln 0 (sprintf "anal area %A" anal)
            pline (sprintf "anal area %A" anal)            
#endif
        pline(ids + " report end.\n\n")
        anal


(*
 * Write a report
 *)
let vmreport_top ww (stage, banner_msg, control, c2, m_fatal_marks, report_areaf) vms = 
    let costsf = true // For now
    let filename = hd(arg_assoc_or stage ("report-filename", c2, ["report"]))
    let suffix = hd(arg_assoc_or stage ("report-suffix", c2, [".txt"]))
    //let shorter =  control_get_s stage c2 "issue-shorter-reports" None = "enable"
    let shorter = hd(arg_assoc_or stage ("reporter-issue-shorter", c2, ["disable"])) = "enable"
    let report_areaf = report_areaf || hd(arg_assoc_or stage ("reporter-issue-area", c2, ["disable"])) = "enable"
    vprintln 3 (sprintf "Writing report '%s' shorter-mode=%A" filename shorter)

    let write_report concisef = 
        let f1 = filename_sanitize [ '.'; '_'; ] filename + (if concisef then "" else "-full")
        try
            let yd = yout_open_log (f1)
            let pline ss = youtln yd ss
            pline stage
            pline banner_msg  
            little_hdisp yd pline vms  
            let anal = map (vmreport ww suffix yd m_fatal_marks concisef costsf report_areaf) vms

            yout_close yd
            ()
        with _ -> vprintln 0 (sprintf "+++ could not write vmreport >" + f1 + "< suffix=" + suffix)
    write_report shorter
    ()



let opath_vmreport ww (op_args:op_invoker_t) designs = 
    let disabled = 1= cmdline_flagset_validate "report" ["enable"; "disable" ] 0 op_args.c3
    let stage = "explict recipe report directive"
    let m_fatal_marks = ref []
    if !g_report_each_step then () // If reporting each step then make an explicit report step null. This interlock is also in opath.
    elif disabled then ()
    else vmreport_top ww (stage, op_args.banner_msg, op_args.command, op_args.c3, m_fatal_marks, false) designs
    vprintln 2 (sprintf "End vmreport on %i VMs" (length designs))
    designs


let report_argpattern = // These need to be different from earlyarg reporting flags
    [
        Arg_enum_defaulting("report", ["enable"; "disable"; ], "enable", "Disable this stage (reporting stage)");
        Arg_defaulting("report-filename", "report", "Name of the file where report is to be put");
        Arg_defaulting("report-suffix", ".xml", "Suffix of the file where report is to be put");
        Arg_enum_defaulting("reporter-issue-shorter", ["enable"; "disable"; ], "disable", "Remove full details in report files"); 
        Arg_enum_defaulting("reporter-issue-area", ["enable"; "disable"; ], "disable", "Remove full details in report files");       

    ]


let install_abstracte() =
    install_operator ("report",   "Report generator", opath_vmreport, [], [], report_argpattern);
    ()






let rec is_branch = function
    | Xlinepoint(_, s) -> is_branch s
    | Xannotate(_, s) -> is_branch s    
    | Xgoto _
    | Xbeq _ 
    | Xfork _ -> true
    | _ -> false


let is_uncond_goto = function
    | Xgoto(s, b) -> Some(!b)
    | _ -> None

// We wish to redirect goto's so that the number of distinct pause statements is reduced.
// Other benefits might exist (e.g. chain of goto's eliminated).
//
// For each candidate goto we trace backwards on it and its destination making a match. The candidate can be redirected to its destination when the destination
// has exactly the same subsequent trajectory.
// The assembler has a simple goto redirector that we normally use
let bireduce_dic ww msg (dic:hbev_t array) =
    let len = dic.Length
    let points = [ 0 .. len - 1]
    let goto_pairs =
        let  kk c n =
            let v = is_uncond_goto(dic.[n])
            let _ = cassert(v=None || valOf v >= 0, "bireduce: not assembled")
            if v =None then c else (n, valOf v) :: c
        List.fold kk [] points


    // scan backwards:
    let rec scanner (p, q) =
        let p1 = p-1
        let q1 = q-1
        if (p1 < 0 || q1 < 0) then (p, q)
        elif hbev_is_decoration(dic.[p1]) then scanner (p1, q)
        elif hbev_is_decoration(dic.[q1]) then scanner (p, q1)   
        elif is_branch(dic.[q1]) ||  is_branch(dic.[p1]) then (p, q)
        elif dic.[p1] = dic.[q1] then scanner (p1, q1)
        else (p, q)

    let p2s(p,q) = "(" + i2s p + "," + i2s q + ")"
    let scan_top (p, q) =
        if p=q then ()
        else 
        let (p1, q1) = scanner(p, q)
        let _ =
            if p1=q1 || p1=p || q1=q then ()
            else 
                let _ = vprintln 0 (msg +  ":" + p2s(p,q) + " redirect scanned to " + p2s(p1, q1))
                let _ = Array.set dic p1 (Xgoto("$red-scanned", ref q1))
                ()
        ()
    app scan_top goto_pairs 

    
    let _ = WF 3 "bireduce" ww ("Completed " + i2s len + " dic locations")
    ()


// dic_flatten: A simple flatten of any Xblocks embedded in a DIC - for blockstructured input please use 'compile'
let dic_flatten ww msg (in_dic:hbev_t array) =
    let rec flat_idx n q cc =
        match q with
            | h::tt ->
                //let _ = vprintln 0 ("junis " + hbevToStr h)
                if h=g_dic_hwm_marker then
                    vprintln 3 (msg + sprintf ": dic_flatten encounter hwm at %i" n)
                    g_dic_hwm_marker::cc
                else
                    match h with
                        | Xblock lst -> flat_idx n (lst @ tt) cc
                        | cmd -> flat_idx n (tt) (cmd::cc)
            | [] -> 
                if n >= in_dic.Length then
                    vprintln 3 (msg + sprintf ": dic_flatten end of input DIC at %i" n)
                    g_dic_hwm_marker::cc
                else flat_idx (n+1) [in_dic.[n]] cc
    let lst = flat_idx 0 [] []
    rev lst

    
(*
 * Fill in the labels in the Xgoto statements.
 * Also, we first flatten Xblocks.
 * Under optimisef control, we implement a goto redirection optimisier and replace unreachable code with a commented out version of itself.
 * 
 * Mutates the old DIC and returns a new DIC plus further details.
 *)
let assembler_vm ww vd (ctrl:dic_ctrl_t) optimisef (in_dic:hbev_t array) =
    let _ = WF 2 ("Assemble DIC VM: " + ctrl.ast_ctrl.id) ww (sprintf "start optimisef=%A" optimisef)
    let dic_flattened = List.toArray (dic_flatten ww ctrl.ast_ctrl.id in_dic)
    let labs = new OptionStore<string, int>("labs")
    let lookuplab ef s =
        match labs.lookup s with
            | None ->
                //if strlen s > 4 && s.[0..3] = "END." then g_exitpc
                ef("abstract vm: Missing label " + s)
            | Some v -> v

    let len = dic_flattened.Length
    let gG:grecord_t = // use new_gdic please instead
            {
                limit=ref len; labs=labs; idx=ref len; DIC=ref dic_flattened; id=ctrl.ast_ctrl.id; labprefix= "LL"
            }
    let queries =
        { g_null_queries with
            full_listing=      true
            concise_listing=   true
            yd=                YOVD 3
        }
    let errorfun() = anal_block ww None queries (H2BLK(g_null_directorate, SP_dic(!gG.DIC, ctrl)))
    let ef msg = (errorfun(); sf msg)
            
    let assembly_first_pass p =
            match dic_flattened.[p] with
                | Xlabel s ->                
                    if vd>=4 then vprintln 4 (sprintf "Reassemble_dic:  log symbol table entry %s %i" s p)
                    labs.add s p
                | _ -> ()
    let iota = [0..len-1]
    app assembly_first_pass iota
    let _ = WF 3 "reassemble_dic" ww ("Symbol tab formed for " + i2s len + " DIC locations")

    let _ =
        if optimisef then
            // goto redirector - skip over a jump to basic block that is a nop.
            // There is no point jumping to the middle of a basic block where the prefix does nothing since bevelab does that.
            let m_ops = ref 0
            let limit00 = 1000
            let rec redirect n = 
                let rec rd2 limit best p =
                    if limit < 0 then
                        let _ = vprintln 3 ("ran out of redirect steps - infinite loop? could check")
                        best
                    elif p>=len then best else rd3 limit best (p, (!gG.DIC).[p])
                and rd3 limit best = function
                    | (p, Xgoto(l, _)) -> rd1 limit (Some l) l
                    | (p, Xcomment _)
                    | (p, Xbeq(X_true, _, _)) // Ignore never-taken branches 
                    | (p, Xlabel _) -> rd2 limit best (p+1)
                    | (p, _) -> best
                and rd1 limit best l = rd2 (limit - 1) best (lookuplab ef l)

                let rdd = function
                    | Xgoto(l, _) when l.[0] <> '$' ->
                        let nd = rd1 limit00 None l
                        if nd <> None then 
                              (vprintln 3 ("Redirect Xgoto " + l + " to " + valOf nd);
                               Array.set !gG.DIC n (Xgoto(valOf nd, ref -1));
                               mutinc m_ops 1
                              )
                    | Xbeq(X_true, l, _) -> () // Ignore never-taken branches 
                    | Xbeq(g, l, _) -> 
                         let nd = rd1 limit00 None l
                         if nd <> None then
                              (vprintln 3 ("Redirect Xbeq " + l + " to " + valOf nd);
                               Array.set !gG.DIC  n (Xbeq(g, valOf nd, ref -1));
                               mutinc m_ops 1
                              )

                    | _ -> ()
                rdd (!gG.DIC).[n]
                ()
            app redirect [0..len-1]            
            vprintln 2 (sprintf  "redirect optimised %i/%i" !m_ops len)
            ()


    // Reachability scanner
    let reachable_locations = Array.create len false
    let rec rs2 p = function
        | Xgoto(l, n) when l.[0] = '$' && !n >= 0  -> rs1(!n)
        | Xgoto(l, n) when l.[0] = '$' && !n < 0   -> sf ("$red label lost")
        | Xgoto(l, _)        -> rs1(lookuplab ef l)
        | Xfork(l, _)        -> (rs1(lookuplab ef l); rs1(p+1))
        | Xbeq(X_true, l, _) -> rs1(p+1) // Never taken branch.
        | Xbeq(g, l, _)      -> (rs1(lookuplab ef l); rs1(p+1))  
        | Xannotate(_, s)  
        | Xlinepoint(_, s)   -> rs2 p s
        | Xreturn _
        | Xbreak 
        | Xcontinue          -> ()
        | Xif(g, t, f)       -> (rs2 p t; rs2 p f)
        | Xwhile(g, t) 
        | Xdo_while(g, t)    -> rs2 p t        
        | Xblock(lst)        -> app (rs2 p) lst
        | (_)                -> rs1(p+1) (* All other forms allow control to flow to next statement *)
    and rs1 h =
        if h >=len || reachable_locations.[h] then () 
        else
            Array.set reachable_locations h true
            if vd>=4 then vprintln 4 (sprintf "DIC index %i is reachable" h)
            rs2 h (!gG.DIC).[h]

    let _ = rs1 0 // Tree search from entry point location 0.

    let _ = WF 3 "reassemble_dic" ww ("Reachable analysis complete")

    let is_ngoto pos = function
        | Xgoto(lab, dr) when !dr = pos+1 -> true
        | _                               -> false

    let is_comment = function
        | Xcomment _ -> true
        | _ -> false
        
    let _ = // comment out etc
        let rs_rewriter p = // Rewrite the DIC (directly indexed code) with unreachable code (except comments) commented out.
            let xpatch1 nv =
                //dev_println (sprintf "xpatched1 reachable not: at %i to %s" p (hbevToStr nv))
                Array.set !gG.DIC p nv
            let cmd = (!gG.DIC).[p]
            if is_comment cmd then () 
            elif not(reachable_locations.[p]) then xpatch1 (Xcomment("/// L1 " + hbevToStr_cleaned "" cmd))
        app rs_rewriter iota


    (* Finally, do the second pass of the assemble by patching goto links *)
    let _ =
        let xpatch2 pos nv =
            if vd>=4 then vprintln 4 (sprintf "xpatched2 at %i to %s" pos (hbevToStr_cleaned "" nv))
            Array.set !gG.DIC pos nv

        let rec dic_Gpatch ef pos cmd =
            match cmd with
            | Xblock lst -> app (dic_Gpatch ef -1000) lst
            | Xgoto(s, p) when s.[0] = '$' -> ()
            | Xgoto(s, p) -> // Remove goto to immediate successor.
               let _ = p := lookuplab ef s
               if !p = pos+1 && optimisef then xpatch2 pos (Xcomment(hbevToStr_cleaned (if true then "/// " else sprintf "/// control xfer to immediate DIC successor %i->%i " pos !p) cmd))
               ()
            | Xbeq(X_true, s, p) -> ()
            | Xgoto(s, p)   
            | Xbeq(_, s, p) 
            | Xfork(s, p) ->
                p := lookuplab ef s
                if vd>=4 then vprintln 4 (sprintf "Patched branch/jump dest at %i %s to %i" pos s !p)
            | (_) -> ()
        
        let patcher n =
            if reachable_locations.[n] then dic_Gpatch ef n (!gG.DIC).[n]
        app patcher iota
    let _ = WF 2 ("Assemble VM: " + ctrl.ast_ctrl.id) ww (sprintf "finished %i instructions. gc=" len + gc_stats())
    let optimised = SP_dic(!gG.DIC, ctrl)
    (optimised, gG)


let post_assembly_report ww filename0 msg designs =
    let filename = filename_sanitize [ '+'; '.'; '_'; ] filename0
    if false then vprintln 3 ("Not writing post assembly report " + filename)
    else try
           vprintln 2 ("Writing post assembly report " + filename)
           let yd = yout_open_log filename
           let queries =
               { g_null_queries with
                   full_listing=      true
                   concise_listing=   true
                   yd=                yd 
               }
           yout yd ("Post assembly report\n")
           youtln yd ("msg=" + msg)
           let _ = map (anal_block ww None queries) designs
           yout yd ("EOF\n")
           yout_close yd
           ()
         with _ -> vprintln 0 ("Could not open log file " + filename)
        

(*
 * The x_valof return value is not known to start with, so it is made an ref to an x_net
 * that is changed on first assign, or finally deleted it no resultis expression is needed.
 *)
let vof_bind s = 
    let k = funique s
    let rr = (k + "lab", ref(gec_X_net(k + "val")))
    rr 

(*
 * If the rhs width is greater than the register width l && lw is not the storage
 * width then flag an error.
 *)
let check_assign_wrapconditions (l, r) =
    match l with
    | X_bnet ff ->
        let f2 = lookup_net2 ff.n
        let a = at_assoc "storage" f2.ats
        let rw = ewidth ("wrapconditions: check width r " + ff.id) r
        let lw = ewidth ("wrapconditions: check width l " + ff.id) l
        if a=None || rw=None || lw=None then ()
        else 
        let sw = atoi(valOf a) 
        let j = function
            | None -> "None"
            | Some v -> i2s v
        let m = netToStr l + " := " + xToStr r + ": assignment may wrap differently: rhs/w=" + j  rw + ", lhs/w=" + j lw + sprintf ", store/w=%A" sw
        let _ = vprintln 3 m
        if BigInteger(valOf lw) <> sw && (valOf rw > valOf lw || BigInteger(valOf rw) > sw) then vprintln 0 ("+++ wrap check warning: " + m)


    |   l -> ()


(*
 * Compile the rich set of hbev down to a psudo-code using a core set of hbev.  Conditional flow
 * control is implement only with Xbeq.
 *
 * Every point where a thread blocks
 * must have a G label so the scheduler can resume from there.
 *)
let rec bcompile_e ww G arg =
    match arg with
    | X_dontcare 
    | X_false
    | X_true 
    | W_linp _ -> arg 
    | W_bdiop(V_orred, [e], i, _) -> xi_orredi i (compile_e ww G e)
    | W_bnode(oo, lst, inv, _)    -> xi_bnode(oo, map (bcompile_e ww G) lst, inv)
    
    | W_bdiop(oo, lst, inv, _) -> 
        let lst' = map (compile_e ww G) lst
        ix_bdiop oo lst' inv

    | W_bmux(gg, ff, tt, _) -> xi_bmux(bcompile_e ww G gg, bcompile_e ww G ff, bcompile_e ww G tt)

    // Perhaps code this using the changed flag style of bassoc
    | W_cover(lst, _) -> xi_cover(map (fun x-> map (fun y -> xb2nn(bcompile_e ww G (deblit y))) x) lst)
        
    | W_bitsel(l, n, i, _) ->
        let l' = compile_e ww G l
        xi_bitseli i (l', n)

    | other -> sf("unsupported form in abstract bcompile_e: " + xbkey other + ": " + (xbToStr other) + "\n")


and compile_e ww G arg = // This compile for expressions is so close to a NOP one could just use the walker.
    match arg with
        | X_bnet _
        | X_net _
        | X_undef
        //| X_mask _
        | X_num _
        | X_bnum _
        | W_string _ -> arg

        | X_blift e -> xgen_blift(bcompile_e ww G e)

        | W_query(gg, tt, ff, _) -> xi_query(bcompile_e ww G  gg, compile_e ww G tt, compile_e ww G ff)
        | X_pair(tt, ff, _) -> ix_pair (compile_e ww G tt) (compile_e ww G ff)

        | W_node(prec, V_cast cvtf, [arg], _) -> ix_casting None prec cvtf arg

        | W_node(prec, V_onesc, [e], _) -> xi_onesc prec (compile_e ww G e)

            // V_exp missing ?
        | W_node(prec, V_neg, [e], _) ->
            let rr = xi_neg prec (compile_e ww G e)
            rr
            
        | W_node(prec, oo, lst, _) -> 
            let lst' = map (compile_e ww G) lst
            ix_op prec oo lst'

        | W_valof(l, _) ->
            let b  = g_unbound
            let c  = g_unbound
            let r = vof_bind "foval"
            let _ = compile_hbev ww G g_null_an (b, c, r) (gec_Xblock l)
            let _ = compile_hbev ww G g_null_an (b, c, r) (Xlabel(fst r))
            let xx = function
                | X_net(f, _) -> xgen_blift X_true
                | X_bnet v    -> X_bnet v
                | other       -> sf (sprintf "strange form in x_valof result expression %A" (xToStr other))
            xx(!(snd r))

        | W_asubsc(l, r, _) -> ix_asubsc (compile_e ww G l) (compile_e ww G r)

        | W_apply(fgis, cf, args, _) -> xi_apply_cf cf (fgis, map (compile_e ww G) args)

        | other -> sf("HPR vm compile_e other: " + xToStr other)


// For an l-mode compile, we have essentially the identity function, except for an r-mode compile of subscript expressions.
and compile_lmode ww gG nN =
    let jk = function
        | X_net _  
        | X_bnet _ 
        | X_x _    
        | X_undef           -> nN
        | W_asubsc(l, r, _) -> ix_asubsc l (compile_e ww gG r)
        | other -> sf("compile_lmode other form: " + xkey nN + "  " + xToStr nN)

    let ans = jk nN
    //let _ = vprintln 3 ("compile_lmode  " + xintToStr(Array.sub(XINT_MYA_AR, k)) + " is " + sfold (xToStr) (xi_valOf r))
    //let _ = vprintln 3 ("compile_lmode  ans=" + sfold xToStr (xi_valOf ans))
    ans

//
// compile_hbev accepts standard block-structured code and generates DIC code that uses gotos and so on.
//    
and compile_hbev ww G an (b, c, r) cmd =
    let labprefix = G.labprefix
    let selfcall = compile_hbev_ntr ww G an (b, c, r)
    //vprintln 0 ("compile_hbev " + hbevToStr cmd)
    match cmd with
    | Xassign(a,b)    ->
        let l = compile_lmode ww G a
        let r = compile_e ww G b
        let _ = check_assign_wrapconditions(l, r)
        G_emitan G an (Xassign(l, r))

    | Xblock l    -> app (selfcall) l

    | Xskip      -> G_emitan G an cmd
    // | (Xeasc(x_valof l))-> app (compile_hbev_ntr ww G an (g_unbound, g_unbound, vof_bind "easc_valof")) l
    | Xeasc e -> G_emitan G an (Xeasc(compile_e ww G e))
#if SPARE
    | Xcall((f, gis), args) -> 
        let fgis = (f, gis)
        let l' = map (compile_e ww G) args
        let canned = op_assoc f xg_hpr_canned_table // deprecated table
        let f' =
            if canned<>None then 
                (vprintln 3 ("hpr_compile: Expand native hpr function " + hbevToStr cmd);
                 if f = "hpr_arrayop" then protocols.wrapped_hpr_arrayop ww () else sf "hpr_canned_table overplete"
                )
            else (fun x->(Xskip, []))
        if canned=None then G_emitan G an (Xcall(fgis, l'))
        else compile_hbev ww G an (g_unbound, g_unbound, (g_unbound, ref X_undef)) (fst(f' l'))
#endif
    | Xgoto(s, _)     -> G_emitan G an (Xgoto(s, ref -1))
    | Xfork(s, _)     -> G_emitan G an (Xfork(s, ref -1))
    | Xlabel s        -> gec_Glabel G s 
    | Xbreak          -> if b=g_unbound then sf g_unbound else compile_hbev ww G an (b, c, r) (Xgoto (b, ref -1))

    | Xjoin _         -> G_emitan G an cmd

    | Xreturn arg     -> 
        let m r = function
            | X_bnet f     -> !r
            | X_net(id, _) -> (r := autotypenet arg id; !r)
        let arg' = compile_e ww G arg
        if fst r = g_unbound then G_emitan G an (Xreturn arg')
        else (
              compile_hbev ww G an (b, c, r) (Xassign(m (snd r) (!(snd r)), arg'));
              compile_hbev ww G an (b, c, r) (Xgoto (fst r, ref -1))
             )

    | Xcontinue ->
        if c=g_unbound then sf g_unbound 
        else compile_hbev ww G an (b, c, r) (Xgoto (c, ref -1))

    | Xwaitabs g         -> G_emitan G an (Xwaitabs(compile_e ww G g))
    | Xwaitrel g         -> G_emitan G an (Xwaitrel(compile_e ww G g))
    | Xwaituntil g       ->
        if true then selfcall (gen_Xwhile(xi_not g, Xeasc(gen_pause_call [])))
        else G_emitan G an (Xwaituntil(bcompile_e ww G g))
    | Xannotate(an1, arg) ->
        let an' = { comment= (if an1.comment <> None then an1.comment else an.comment);
                    palias= (if an1.palias <> None then an1.palias else an.palias);
                    atts = foldr atts_merge (an.atts) (an1.atts);
                  }
        compile_hbev ww G an' (b, c, r) (arg)

    | Xlinepoint(lp, g) -> 
        (G_emitan G an (Xlinepoint(lp, Xskip));
         if g <> Xskip then compile_hbev ww G an (b, c, r) g
        )

        // Xfor missing and Xbloop are missing
        
    | Xdo_while(g, s) -> 
        let topper = funique(labprefix + "T")
        let breaker = funique(labprefix + "B")
        let continuer = funique(labprefix + "C")
        let l1 = (
                  Xlabel topper
                 )
        compile_hbev_ntr ww G an (breaker, continuer, r) (l1)
        compile_hbev_ntr ww G an (breaker, continuer, r) (s)
        let g' = bcompile_e ww G (xi_not g)
        let l3 = [
                  Xlabel continuer;
                  Xbeq(g', topper, ref -1);
                  Xlabel breaker
                 ] 
        compile_hbev_ntr ww G an (breaker, continuer, r) (gec_Xblock l3) 

        
    | Xwhile(g, s) -> 
        let breaker = funique(labprefix + "B")
        let continuer = funique(labprefix + "C")
        let g' = bcompile_e ww G g
        let l = [ Xlabel continuer;
                  Xbeq(g', breaker, ref -1);
                  s;
                  Xgoto(continuer, ref -1);
                  Xlabel breaker
                ] 
        compile_hbev_ntr ww G an (breaker, continuer, r) (Xblock l)

    | Xif(g, t, Xskip) -> 
        let l1 = funique(labprefix + "I")
        let g' = bcompile_e ww G g
        let l = [ Xbeq(g', l1, ref -1); t; Xlabel l1 ] 
        compile_hbev_ntr ww G an (b, c, r) (Xblock l)

    | Xcomment _ -> G_emitan G an cmd

    | Xbeq(X_true, t, _) -> G_emitan G an (Xcomment ("Never taken: " + hbevToStr cmd))
    | Xbeq(g, t, _) -> G_emitan G an (Xbeq(bcompile_e ww G g, t, ref -1))
    | Xif(g, t, f) -> 
        let l1 = funique(labprefix + "IA")
        let l2 = funique(labprefix + "IB")
        let g' = bcompile_e ww G g
        let l = [ Xbeq(g', l1, ref -1); t; Xgoto (l2, ref -1); Xlabel l1; f; Xlabel l2 ] 
        in compile_hbev_ntr ww G an (b, c, r) (Xblock l)

    | other -> sf("unsupported form in abstract compile_hbev: " + (hbevToStr other) + "\n")

and compile_hbev_ntr ww (G:grecord_t) an (b, c, r) l = 
    let vd = -1
    if vd>=4 then vprint 4 ("compile_hbev " + (hbevToStr l) + "\n") 
    let ans = compile_hbev ww G an (b, c, r) l
    if vd>=4 then vprint 4 ("compile_hbev done\n\n") 
    ans

(*
 * Compile VM takes an SP_l block-structures (behavioural) imperative command AST and generates an SP_dic VM assembly block.
 *
 * If a clk is given, we wait once for that clock at head and then use it for
 * embedded PSL event guards.  ... ?  Hmmm comment out for now (2Jul08).  TODO.
 *)
let compile_hbev_to_lst ww vd (ast_ctrl:ast_ctrl_t) clk msg (minfo:minfo_t) labprefix lst =
    let limit = 200000
    let id = vlnvToStr minfo.name  + ast_ctrl.id
    let ww = WF 3 "VM compile hbev to list " ww ("Start " + id)
    //vprint(1, gc_stats() + "FL0\n") (* Flush to log *)
    //let _ = anal_block  None (YO_null, true, false) (H2BLK(clk, SP_l(gec_Xblock lst)))
    //let _ = WF 3 "VM compile lst " ww ("Anal done " + id)
    //vprintln 1 ("Flush:" + gc_stats() + "FL1\n") (* Flush to log *)
    let gG  = new_gdic id labprefix
    //let start = funique "start"
    //let _ = Glabel G start
    let b  = g_unbound
    let c  = g_unbound
    let r  = (g_unbound, ref X_undef)

    //if not_nonep clk then (Wwait gG (xi_not(valOf clk)); Wwait gG (valOf clk)) (* TODO use rose *)
    app (compile_hbev_ntr ww gG g_null_an (b, c, r)) lst // Main Compile Complete
    //Ggoto gG start // Make loop forever - this is now, instead, intrinsic to some directorate settings...
    let id = funique "vm"
    let minfo' = { minfo with memdescs=[]; name=  { minfo.name with kind = id :: minfo.name.kind }}
    let final_id = vlnvToStr(minfo'.name)
    // We are creating a new dic from an AST, so a fresh dic_ctrl is required.
    let dic_ctrl = { g_null_dic_ctrl with ast_ctrl={ g_null_ast_ctrl with id=id } }
    let _ = WF 3 "VM compile hbev to list" ww (sprintf "Finished SP_l %s compiled to VM by %s with %i commands.\n" final_id msg !gG.idx)
    let (rr, rG) = assembler_vm ww vd dic_ctrl true !gG.DIC
    let r1 = H2BLK(clk, rr)
    let dont_skip_op = true
    //let vvd = (YO_null, true, false)
    //let _ = anal_block  None vvd (r1)
    let s = final_id + ".vm"
    if dont_skip_op then post_assembly_report ww (s) msg [r1]
    else vprintln 3 ("Skipping post assembly report for " + s) 
    rr // (minfo', clk, r)

// This is a dic->dic recompile into simpler form (ie. no Xif, Xfor and so on) that accepts a few other input forms too.
let rec compile_sp_to ww vd clk msg (minfo:minfo_t) arg =
    let rec comp2 arg = 
        match arg with
        | SP_seq(l)         -> SP_seq(map comp2 l)
        | SP_par(a, l)      -> SP_par(a, map comp2 l) 
        | SP_asm _          -> arg
        | SP_l(ast_ctl, l)  -> compile_hbev_to_lst ww vd ast_ctl clk msg minfo "LSP" [l]
        | SP_dic(dic, dic_ctrl) -> // When already in DIC form, treat as a list of commands. This may contain Xgoto but these are symbolic Xlabel and local PC values are ignored.
            let rec zz pos = if pos >= dic.Length then [] else dic.[pos]::zz(pos+1)
            let lst = zz 0
            compile_hbev_to_lst ww vd dic_ctrl.ast_ctrl clk msg minfo "LDIC" lst

        | SP_comment _
        | SP_rtl _         -> arg
        //| other -> sf(vlnvToStr minfo.name + ":" + msg + sprintf " cannot be compiled: wrong form %A" other)
        | other -> sf(sprintf  "VM %s cannot be compiled: %s: wrong form." (vlnvToStr minfo.name) msg)        
    let ans = comp2 arg
    (minfo, clk, ans)

let compile_exec_to ww vd msg (minfo:minfo_t) = function
        | H2BLK(clkinfo, sp) ->
            let (_,  _, a) = compile_sp_to ww vd clkinfo msg minfo sp
            H2BLK(clkinfo, a)
            

//
// Flatten away the structure of an FSM.
// This performs the opposite to project_arcs_to_fsm.
// The FSM form can vary between having one edge/transition to a given destination with predicated work on it to having multiple edges with more complex guards to that same destination, each with less qualified work.  We could normalise that here by collating edges to a given destination.

// This function does not do a shannon decomposition by binary division: it should perhaps so do.  Instead it just looks at the pp fields of the arcs.
let fsm_flatten_to_rtl ww mach_id = function
    | SP_fsm(i:fsm_info_t, transitions) -> 
        let do_tran cc (v:vliw_arc_t, arcno) = // This is the form where pp is inserted: another version of this function puts pp as None and we insert that functionality in gqual
            let code_track_point = (mach_id + ":FSM_transition:" + xToStr i.pc + ":" + xToStr v.resume, Some arcno)
            cassert(v.pc=i.pc, "PC different")
            let pp = Some(i.pc, v.resume)
            let gqual g = ix_and v.gtop g
            let xc xrtl cc =
                match xrtl with
                | XRTL(None, g, lst) -> (code_track_point, XRTL(pp, gqual g, lst)) :: cc
                | XRTL(Some(pc, ss), g, lst) ->
                    if pc=i.pc && ss=v.resume then (code_track_point, XRTL(pp, gqual g, lst)) :: cc else sf ("fsm had wrong pp field:" + xrtlToStr xrtl) 

                | XIRTL_pli(None, g, fgis, args) ->  (code_track_point, XIRTL_pli(pp, gqual g, fgis, args)) :: cc

                | XIRTL_pli(Some(pc, ss), g, fgis, args) ->
                    if pc=i.pc && ss=v.resume then (code_track_point, XIRTL_pli(Some(pc, ss), gqual g, fgis, args)) :: cc else sf ("fsm had wrong pp field:" + xrtlToStr xrtl) 
                | XRTL_nop ss -> (code_track_point, xrtl) :: cc

                | other -> sf ("flatten_fsm_to_rtl other xrtl " + xrtlToStr other)
            let goto = XRTL(pp, gqual X_true, [Rarc(X_true, v.pc, v.dest)]) 
            let cmds = List.foldBack xc v.cmds []
            (code_track_point, goto) :: cmds @ cc
        let cmds = List.fold do_tran [] (zipWithIndex transitions)
        let resets = [ XRTL(None, X_true, [ Rarc(X_true, i.pc, fst i.start_state)]) ]
        (cmds, resets)

let g_eis_walker = ref None


// See if an expression contains an EIS subexpression (i.e. valuable side effecting content.)
let eis_rhs msg x =
    let vd = -1 // Off
    let vdp = false
    let msg = "eis_rhs"
    let walker =
        match !g_eis_walker with
            | Some ov -> ov
            | None ->
                let dyn_prog_cache = new Dictionary<xidx_t, walker_nodedata_t>()
                let unitfn arg = ()
                let lfun strict clkinfo arg rhs = ()
                let null_sonchange _ _ nn (a,b) = b

                // Operator function:
                let opfun arg n bo xo _ soninfo2 =
                    let s() = if xo=None then xbToStr(valOf bo) else xToStr(valOf xo)
                    //vprintln 0 ("Opfun " + s() + " acting on " +  sfold qToStr soninfo2)
                    let nn = abs n
                    let (found, ov) = dyn_prog_cache.TryGetValue nn
                    if found then ov
                    else
                        let yes =
                            match xo with
                                | Some(W_apply((f, gis), _, args, _)) -> // walker recurses on args before or after body.
                                   //vprintln 3 (sprintf "eis_rhs encounters %s eis=%A"  f gis.fsems.fs_eis)
                                   (gis.fsems.fs_eis)
                                | _ -> false
                        let sonscan cc = function 
                            | EIS_NODE eisf -> cc || eisf
                            | _ -> cc
                        let nc = yes || List.fold sonscan false soninfo2
                        let rr = EIS_NODE(nc)
                        dyn_prog_cache.Add(nn, rr)
                        rr

                let (_, sWALK) = new_walker vd vdp (true, opfun, (fun (_) -> ()), lfun, unitfn, null_sonchange, null_sonchange)
                g_eis_walker := Some sWALK
                sWALK

    let aux = (msg, walker, None, ref [])
    let p = mya_walk_it aux g_null_directorate x
    match p with
        | EIS_NODE ans ->
            //dev_println (sprintf "eis_rhs result %A for %s"  ans (xToStr x))
            ans
        | _ -> sf "L2381"

(*
 * Perform fsem analysis of RTL form of code.  Just EIS considered here. EIS is compositional by natural disjunction.
 * Nonreftran can also be conservatively deduced, in the future, from cyclic dataflows, but is non compositional.
 * Array writes should probably be considered EIS.
 * RTL scalar writes are currently not considered EIS and hpr_writeln is sometimes not - we need a finer severity level concept please.
 * An end in itself (EIS).
 * An arc that should be kept for its own sake (e.g. useful side-effects).
 *)
let eis_arc x = function
    | XRTL(Some(pc, state), g, lst) ->
        let q c a = c || match a with
                            | Rarc(g, l, rhs)             ->  (g_keep_goto_arcs && pc=l) || eis_rhs "eis_arc" rhs
                            | Rpli(g, ((_, gis), _), _)   -> gis.fsems.fs_eis // todo or eis of any args?
                            | _ -> false
        List.fold q false lst            

    | XRTL(None, _, lst) -> // We currently do not scan for eis sub-expression function calls in guards  ...
        let q c a = c || match a with
                            | Rpli(g, ((_, gis), _), _)   -> gis.fsems.fs_eis
                            | Rarc(g, l, rhs) -> eis_rhs "eis_arc" rhs
                            | _ -> false
        List.fold q false lst            

    | XIRTL_pli(f, _, ((_, gis), _), _)   -> gis.fsems.fs_eis // TODO or eis of any args?

    | _ -> false 



    
// PLI calls can be combined if their ordering and args etc are the same.
let sef_gate g = function
    | XIRTL_pli(a, g0, fgis, args) -> XIRTL_pli(a, xi_and(g, g0), fgis, args)
    | XRTL(a, g0, lst)             -> XRTL(a, xi_and(g, g0), lst)
    | other -> sf("sef sef_gate g other " + xrtlToStr other)

let sef_g = function
    | XIRTL_pli(a, g, fgis, args) -> g
    | XRTL(a, g, lst)             -> g
    | _ -> sf("sef sef_g other")

let sef_combine = function
    | (XIRTL_pli(a, g, fgis, args), XIRTL_pli(a', g', fgis', args')) when fgis=fgis' && args=args' ->
         if a=a' then XIRTL_pli(a, xi_or(g, g'), fgis, args)
         else muddy "// sef_combine from different activations"
    | (XRTL(a, g, lst), XRTL(a', g', lst')) when lst=lst' ->
         if a=a' then XRTL(a, xi_or(g, g'), lst)
         else muddy "// sef_combine arcs from different activations"
    | (a, b) -> sf("sef sef_combine other:\n A= " + xrtlToStr a + "\n B= " + xrtlToStr b)

let sef_combine_noguards = function
    | (XIRTL_pli(a, g, fgis, args), XIRTL_pli(a', g', fgis', args')) when fgis=fgis' && args=args' ->
         if a=a' then XIRTL_pli(a, X_true, fgis, args)
         else muddy "// sef_combine_noguards from different activations"
    | _ -> sf("sef sef_combine other")


let tripToStr (d,c,s,eis) = "TRIP( D=" + sfold (sdToStr) d + ";;\n  C=" + xrtlToStr c + ";;\n S=" + sfold (sdToStr) s + "\n  ;;EIS=" + boolToStr eis + ")"  

//let int_tripToStr (d, c, s, eis) = "TRIP( D=" + sfold (isdToStr) d + ";;\n  C=" + xrtlToStr c + ";;\n S=" + sfold (isdToStr) s + "\n  ;;EIS=" + boolToStr eis + ")"  

// In cone-refine (fmode=true) we are interested in either sequential or combinational dependencies, but when sorting
// blocking assigns we must not neglect the implied X_X on the lhs of an XIRTL_arc (fmode=false). 
let tripgen_x vd fmode c0 code =
    match code with
        | XIRTL_buf(g, l, r) ->
            if vd >= 5 then vprint vd (xrtlToStr code + "\n")
            let d = xi_driven [] l
            let s0 = xi_lsupport (xi_support (xi_bsupport [] g) r) l
            (d, code, s0, eis_arc false code) :: c0

        | XRTL(pp, g, lst) ->
            let s0 = xi_bsupport [] g
            let s0 = match pp with
                     | None -> s0
                     | Some(pc, v) -> xi_support (xi_support s0 pc) v
                      
            let qx (cd, cs, eisf) = function
                | Rarc(g, l, r) ->
                    if vd>=5 then vprintln 5 (xrtlToStr code)
                    let l' = if fmode then l else xi_X1 l
                    let d = xi_driven [] l'
                    let s = xi_lsupport (xi_support (xi_bsupport [] g) r) l'
                    let eisf = eisf || eis_arc false code
                    (sd_union(cd, d), sd_union(cs, s), eisf)
                | Rnop s -> (cd, cs, eisf)
                | Rpli(g, fgis, lst) -> 
                    let s =  List.fold xi_support (xi_bsupport [] g) lst
                    let eisf = eisf || eis_arc false code
                    (cd, sd_union(cs, s), eisf)
            let (d1, s1, eisf) = List.fold qx ([], [], false) lst
            (d1, code, sd_union(s1, s0), eisf) :: c0
       
        | XIRTL_pli(None, g, fgis, lst) -> 
            ([], code, List.fold xi_support (xi_bsupport [] g) lst, eis_arc false code) :: c0


        | XIRTL_pli(Some((pc, v)), g, fgis, lst) -> 
            ([], code, List.fold xi_support (xi_bsupport [(pc, [])] g) lst, eis_arc false code) :: c0

        | XRTL_nop _ -> ([], code, [], false)  :: c0
        //| _ -> sf("tripgen other: unsupported xrtl")


    
//Trip Sorter (topological sorter with Khan's Algorithm)
//L ← Empty list that will contain the sorted elements
//S ← Set of all nodes with no incoming edges
//while S is non-empty do
//   remove a node n from S
//       insert n into L
//       for each node m with an edge e from n to m do
//             remove edge e from the graph
//             if m has no other incoming edges then
//                 insert m into S
//                 if graph has edges then output error message (graph has at least one cycle)
//                 else output message (proposed topologically sorted order: L)
type trip_t = (sd_t list * xrtl_t * sd_t list * bool)

//type sd_x_t = (hexp_t * hexp_t list)
//type trip_x_t = (sd_x_t list * xrtl_t * sd_x_t list * bool)        


// Convert driven or support list to sd form where the second arg, the subscripts used list, is an i64 list for constants, or null for variables, denoting any subscripts or bit extract/inserts.

let support_to_sd_form ww (d, v) =
    match v with
        | lst when not(conjunctionate constantp lst) -> (d, [])
        | lst ->
            let git64 v = xi_manifest64 "support_to_sd_form" v // We use int64 for constant subscripts to an array. 32 bits would surely be enough for any feasible bus width. But we cannot express a constant subscript and a constant bit with this form (like silly old Verilog).
            (d, map git64 lst)


// A trip_t uses sd_form, whereas support functions return sd_x_t.
let trip_x_to_sd_form ww (d, cmd, s, flags) =  (map (support_to_sd_form ww) d, cmd, map (support_to_sd_form ww) s, flags)

//let int_tripgen_ ww vd fmode c0 code = map (trip_to_sd_form ww) (tripgen vd fmode c0 code)

    
let trip_sorter ww (trips: trip_t list) =
    let rec itpass c set = 
        let dg c (d, cd, s, flags) = sd_union(c, d)
        let all_driven = List.fold dg [] set
        // Pull out any whose support is not driven in the currently remaining set.
        let pred (driven, cmd, s, flags) = (sd_intersection_example s all_driven)=[]
        let (tops, rem') = groom2 pred set
        let c' = tops :: c
        let ans = if rem' = [] then (rev c')
                  elif tops=[] then
                      let onetrippred (d, cd, s, flags) = sd_intersection_pred d s
                      let solo = List.fold (fun c trip -> if c<>None then c else if onetrippred trip then Some trip else None) None rem'
                      if solo<>None then
                          dev_println ("Mandrake trip pulled " + tripToStr (valOf solo))
                          itpass ([valOf solo] :: c) (list_subtract(rem', [valOf solo]))
                      else
                          app (fun x -> vprintln 0 ("Trip = " + tripToStr x + "; pred=" + boolToStr (pred x) + "; example=" + sfold xToStr (sd_intersection_example (f3o4 x) all_driven))) rem'
                          vprintln 0 ("all_driven = " + sfold isdToStr all_driven)
                          sf ("sorter: (level-sensitive latch?) strongly connected clique includes some of: length=" + i2s(length rem') + "\n" + sfoldcr tripToStr rem')
                  else itpass c' rem' 
        ans
        
    //let ans = list_flatten(itpass [] (map (trip_to_sd_form ww) trips))
    let ans = list_flatten(itpass [] trips)

    let spred (d0, c0, s0, flags) (d1, c1, s1, flags1) =
        let a = sd_intersection_example d0 s1
        let b = sd_intersection_example d1 s0
        let v = (if a<>[] then 1 else 0) - (if b<>[] then 1 else 0)
        vprintln 0 ("\n\n" + xrtlToStr c0 + "\n" + xrtlToStr c1 + "\nSorting pred " + sfold (fun (a,b)->xToStr a) d0 + " cf " +  sfold (fun (a,b)->xToStr a) d1 + " --> " + i2s v)
        //vprintln 0 ("       " + sfold (fun (a,b)->xToStr a) s0 + " cf " +  sfold (fun (a,b)->xToStr a) s1 + " --> " + i2s v)        
        v
    //let ans = List.sortWith spred trips // not khan!
    ans


// Generate PC state names. Need a new PC as well.
// Want to embed 
let statename11 npc (_, hardf) pcinfos p =
    if false then xi_num p
    //elif false then xi_stringx (XS_withval(xi_unum p)) strobeo (i2s p + ":" + pcinfos + xToStr npc)
    elif false then enum_add "statename11" npc pcinfos p
    else
        let xa = pcinfos + ":" + (if p=0 then "start" else "") + i2s p
        //vprintln 0 (sprintf "statename11 >%s< ---  do not really need all this" xa)
        enum_add "statename11" npc xa p


// Predicate indicates when fsm is in idle/start state.
let fsm_idle_pred msg (SP_fsm(info, arcs)) = ix_deqd info.pc (fst info.start_state)

        
// Predicate indicates when fsm is transferring into idle/start state.    
let fsm_stopping_pred msg (SP_fsm(info, arcs)) = // aka ending_pred
    let disjax cc arc =
        let term = if arc.dest = X_undef then arc.gtop else ix_andl [ ix_deqd arc.dest (fst info.start_state); ix_deqd arc.pc arc.resume; arc.gtop ]
        //dev_println(sprintf "disjax resume=%s:  dest=%s cf start=%s with gg=%s yields %s" (xToStr arc.resume) (xToStr arc.dest) (xToStr (fst info.start_state)) (xbToStr arc.gtop) (xbToStr term))
        ix_or cc term
    List.fold disjax X_false arcs // or ix_orl(map disjax arcs)
    

//
    
// Make a recoding table and repack an SP_fsm with a different program counter encoding.
// mpcXX typically becomes xpcYY.
//
// Note: the existing PC may likely be the king variable of a closed unary strobe group. So it is important to rename the PC for the repacked form.
//   
let fsm_recode ww pcroot mx onehot_pc_flag arg =
    let ww = WF 3 "fsm_recode" ww (sprintf "Start onehot=%A pcroot=%s" onehot_pc_flag pcroot)
    let gen_reps (npc, npcs) (res:hexp_t * visit_budget_t) old_pcvs =
        // Want to preserve hardness of the state and the old reset state should be the new reset state.
        //let _ = vprintln 0 (sprintf "Start make initial state for npc=%s "  npcs)
        let x0 = statename11 npc (0, snd res) npcs 0 // ensure added to enum
        //reset state forwarded thru would tend to conflict on unary strobes?
        vprintln 3 (sprintf "Initial/reset state for npc=%s is %s nn=%A" npcs (xToStr x0) (x2nn x0))
        //let _ = vprintln 3 ("old_pcvs = " + sfoldcr (fun (a, hardf) -> sprintf  "   %s  hardf=%A" (xToStr a) hardf) old_pcvs)
        let rec binary_recode p = function
            | [] -> []
            | (X_undef, hardf)::tt -> ((X_undef, hardf), (X_undef, hardf)) :: binary_recode (p) tt
            | (a, hardf)::tt -> ((a, hardf), (statename11 npc (a, hardf) npcs p, hardf)) :: binary_recode (p+1) tt
        let rec onehot p = function
            | [] -> []
            | (X_undef, hardf)::tt -> ((X_undef, hardf), (X_undef, hardf)) :: onehot (p) tt            
            | (a, hardf)::tt -> ((a, hardf), (statename11 npc (a, hardf) npcs p, hardf)) :: onehot (p*2) tt

        // let l0 = List.filter (fun (a, hardf) -> a = res) old_pcvs
        //if list_subtract (l0, [res]) <> [] || list_subtract([res], l0) <> [] then dev_println "FSM_recode: reset list difference"
        let l0 = [res]
        let l0' = map (fun (x, h) -> (statename11 npc (x, h) npcs (xi_manifest "repatch" x), h)) l0
        let r' = list_subtract(old_pcvs, [res])
        let _ = cassert(length r' = length old_pcvs - 1, "reset was missing from old pcvs " + xToStr(fst res))
        let v =
            if onehot_pc_flag && length old_pcvs <= 30 then onehot 1 r'
            else binary_recode 1 r'
        let ans = (List.zip l0 l0') @ v
        //reportx 0 "FSM_recode old/new pc values" (fun (a,b) -> xToStr (fst a) + " ---> " + xToStr(fst b)) ans
        ans
        
    match arg with
        | (clko, SP_fsm(info, arcs)) ->
            //app (arc_rp 0 "Recode_in arcs") arcs
            let oldpcname =
                match arcs with
                    | a::_ -> xToStr a.pc
                    | _ -> "<no-arcs>"
            let npc = //create_enum_net
                let ats = []
                if true (* enable *) then
                    let npc = xgen_bnet(iogen_serf_ff(funique pcroot, [], 0I, 32, LOCAL, Unsigned, None, false, ats, [])) // for now a 32 bit reg - do not use one-hot for more states than 32 then? The encoding width for a closed enum overrides its declaration width at this point.
                    vprintln 3 (sprintf "fsm_recode: %s: Old PCname=%s,   New PC name is %s" mx oldpcname (netToStr npc))
                    npc
                else info.pc
                   
            let npcs = xToStr npc
            let old_resumes =
                let rec hard_flag_retrieve w = function // TODO use a Map
                    | [] when w=X_undef -> g_default_visit_budget
                    | [] ->
                        let _ = hpr_yikes (xToStr npc + ":  abstracte hard_flag_retrieve: no resume for " + xToStr w)
                        g_default_visit_budget // Should not be used but is under maximal sometimes - TODO debug
                    | h::tt when h.resume=w -> h.hard
                    | _::tt                 -> hard_flag_retrieve w tt

                let trawl c = function
                    // Thread exit point(s) is not listed as a resume - so dig it out here if present - NOW DONE EARLIER::: see exits
                    // Reset point is not always a resume.
                    | (si:vliw_arc_t) -> singly_add (si.dest,  hard_flag_retrieve si.dest arcs) c
                List.fold trawl (singly_add info.start_state info.resumes) arcs
            let trovial = length old_resumes < 2 // includes entry and exit
            vprintln 3 (sprintf "npcs=%s, reset=%s : Too trivial a controller to recode = %A" npcs (xToStr(fst info.start_state)) trovial)
            if trovial then arg
            else
                let reps = gen_reps (npc, npcs) info.start_state old_resumes
                let m1 = "recoding for " + npcs
                reportx 3 m1 (fun ((a, hardf), (b, _))-> (sprintf "%A" hardf) + " " + xToStr a + " --> " + xToStr b) reps
                let _ = close_enum m1 npc
                let recode v =
                    match op_assoc v reps with
                        | None -> sf ("missing recode of opc=" + xToStr (fst v))
                        | Some nv -> nv
                let reps' = List.fold (fun (m:Map<hexp_t, hexp_t * visit_budget_t>) ((a, _), b) -> m.Add(a, b)) Map.empty reps
                let recode' v =
                    match reps'.TryFind v with
                        | None -> sf ("needed recode' of opc=" + xToStr v)
                        | Some(a, _) -> a
                let df (si:vliw_arc_t) =
                    let r = recode' si.resume
                    let d = recode' si.dest
                    let si' =  { si with
                                  old_resume= Some si.resume
                                  resume=     r
                                  iresume=    (if r=X_undef then -1 else xi_manifest "iresume" r)
                                  dest=       d
                                  idest=      (if d=X_undef then -1 else xi_manifest "idest" d)
                                  pc=         npc
                               }
                    si'
                let arcs' = map df arcs
                let info' = { info with
                                    pc=           npc
                                    start_state=  recode info.start_state
                                    resumes=      map recode info.resumes
                            }
                //app (arc_rp 0 "Recode_out arcs") arcs'
                (clko, SP_fsm(info', arcs'))
        | other ->
            let _ = vprintln 2 ("fsm_recode: other form input not recodeed!")
            other



//
// Bisimulation reduction: find a pair of FSM states that have identical arcs leading from them and
// rewrite the machine to only use the former.    
//
//  This is a crude algorithm what will never notice forms where a->b->c->d->a might be reduced to a->b->a
//
let bisimulate ww (director, orig) =
    let earlier a b = xi_order_pred a b < 0

    let rec insert_sort (a:vliw_arc_t) = function // sort in ascending dest
          | [] -> [a]
          | h::tt when h.dest=a.dest && h.cmds=a.cmds -> { a with gtop=xi_or(h.gtop, a.gtop) } :: tt // ignores one hard flag(oh dear)
          | h::tt when h.dest=a.dest || earlier h.dest  a.dest -> a :: h :: tt
          | h::tt -> h::insert_sort a tt

    let rec resort = function
        | []   -> []
        | h::t -> insert_sort h (resort t)
        

    // Do some crude norming so equality of states is given by textual equality.
    let norm_arc (a:vliw_arc_t) =
        let is_pli = function
            | XIRTL_pli _ -> true
            | _ -> false
        let (pli, misc) = groom2 (is_pli) a.cmds
        let pred (a:xrtl_t) (b:xrtl_t) = a.GetHashCode() - b.GetHashCode()
        let cmds' = (pli) @ List.sortWith pred misc // was reversed by groom2 but also by the collator so cancel out.
        { a with resume=X_undef; cmds=cmds' }   // hide resume point.


    match orig with
    | SP_fsm(info, arcs) ->
        let insert (c:Map<hexp_t, vliw_arc_t list>) arc = c.Add(arc.resume, insert_sort (norm_arc arc) (valOf_or_nil(c.TryFind arc.resume)))
        let points = Map.toList(List.fold insert Map.empty arcs)
        // TODO: inserting in this table looses order predication

        let match1 (resume, arcs) lst =
            let rec scan = function
               | [] -> None
               | (r', arcs')::t when arcs' = arcs -> Some r'
               | _::t -> scan t
            let found = scan lst
            if found <> None then vprintln 3 ("Found bisim resume pair: " + xToStr resume + " equiv to " + xToStr(valOf found))
            if found = None then None
            elif lc_atoi resume > lc_atoi(valOf found) then Some(valOf found, resume)
            elif lc_atoi resume = lc_atoi(valOf found) then sf "identical resumes"
            else Some(resume, valOf found)

        let rec matcher = function
            | [] -> None
            | h::tt ->
                let v = match1 h tt
                in if v<>None then v else matcher tt

        let rec iterate points =
            let v = matcher points
            if v=None then points
            else
                   let (retained, deleted) = valOf v
                   let rewrite c (res, arcs:vliw_arc_t list) =
                       let rew1 (a:vliw_arc_t) = { a with dest=(if a.dest=deleted then retained else a.dest) }
                       if res=deleted then c
                       else (res, resort(map rew1 arcs)) :: c 
                   let points' = List.fold rewrite [] points
                   in iterate points'

        let points' = iterate points
        let reb c (r, v:vliw_arc_t list) = (map (fun x -> { x with resume=r }) v) @ c // Put resume info back.
        let info' = { info with
                        controllerf= length points' > 1;
                        resumes=     map (fun (x, arcs) -> (x, valOf(op_assoc x info.resumes))) points';
                    }
        vprintln 3 ("Bisim " + xToStr info.pc + " reduction attempt: " + i2s(length info.resumes) + " states changed to " + i2s(length info'.resumes))
        (director, SP_fsm(info', List.fold reb [] points'))


    | other -> (director, other)



// Elide adjacent constructs (preserving order) to give rtl_once and ifshare more to work on.
let sp_elider lst =
    let rec elide = function
        | SP_rtl(ii, k)::SP_rtl(ii_, m)::tt -> elide(SP_rtl(ii, k@m)::tt) // rtl_ctrl discarded
        | SP_par(kf, k)::SP_par(mf, m)::tt -> 
            if kf=mf then elide(SP_par(kf, k@m)::tt)
            else (SP_par(kf, k))::elide(SP_par(mf, m)::tt) 
        | (SP_seq k)::(SP_seq m)::tt -> elide(SP_seq(k@m)::tt)
        | other -> other
        // reportx 0 "elider rtl" spToStr lst
    if true then elide lst else lst 



let sef_eq ag = function // when ag holds, activations need to be equal.
    | XIRTL_pli(a, g, fgis, args), XIRTL_pli(a', g', fgis', args') -> fgis=fgis' && args=args' && (a=a' || (not ag))
    | XRTL(a, g, lst), XRTL(a', g', lst') when a=a' || (not ag) -> lst=lst' // todo take common intersection
            
            
            //| Rarc(l, r) -> XIRTL_arc(a', g', l', r') -> l=l' && r=r' 
    | _ -> false
        //| (aa, _) -> sf("sef sef_eq other " + xrtlToStr aa)


// For repeated commands that are idempotent or insensitive to order, form disjunction of their guards.
let rtl_once lst =
    let rec ks = function
        | a::b::tt when sef_eq true (a,b) ->
            let r = sef_combine(a, b)
            ks (r::tt)
        | other :: tt -> other :: ks tt
        | [] -> []
    ks lst

let guarded_rtl_once lst =
    let lov g0 gl = List.fold (fun c a -> xi_and(c, a)) g0 gl    
    let rec ks = function
        | (gla, a, rsta)::(glb, b, rstb)::tt when sef_eq true (a,b) && rsta=[] && rstb=[]->
            let r = ([xi_or(lov (sef_g a) gla, lov (sef_g b) glb)], sef_combine_noguards(a, b), [])
            in ks (r::tt)
        | other :: tt -> other :: ks tt
        | [] -> []
    ks lst
   

//
// Get lhs of RTL assignment and also report whether the whole statement is eis.
//
let rtl_drivengen cde =
    let (driven, eisf) =
        match cde with
            | XRTL(_, g, lst) ->
                let scn (sofar) = function 
                    | Rarc(g1, l, r) -> xi_driven sofar l
                    | Rpli(_, (fgis, ordering), args) -> sofar // todo pli may have out parameters.
                    | _ -> sofar 
                (List.fold scn [] lst, eis_arc false cde)
            | XIRTL_buf(g, l, r)    -> (xi_driven [] l, eis_arc false cde)
            | XIRTL_pli(_, g, fgis, lst) -> ([], eis_arc false cde) 
            | XRTL_nop _ -> ([], false)
            //| other      -> sf ("rtl_drivegen other: " + xrtlToStr other)
    (driven, eisf)

    
// Tarjan: finds strongly-connected components in a directed graph
// pof=print out function.
let tarjan1<'t when 't:equality> msg pof vd (nodes:'t list, edges) =
//  input: graph G = (V, E)
//  output: set of strongly-connected components (sets of vertices)
//  Another output would be the articulation points. See tarjan2 code.
    let _:ListStore<'t, 't> = edges
    let m_ans = ref []
    let nindex = ref 0
    let index = new Dictionary<'t, int> ()
    let ll = length nodes
    let lowindex = Array.create ll 0
    let stack = ref []
    let min (a,b) = if a<b then a else b
    let pe = match pof with
                  | Some f -> true
                  | None -> false
    let rec strongconnect(v:'t) = 
        let i = !nindex
        if pe then vprintln 0 (msg + ": node " + (valOf pof) v + "/" + i2s i)
        nindex := i+1   // Dont need index numbers if nodes already numbered from zero anyway.
        index.Add(v, i) // Set the depth index for v to the smallest unused index
        Array.set lowindex i i
        stack := v :: !stack // push v
        let pop() = let r = hd !stack in (stack := tl !stack; r)
        // Consider successors of v
        let consider dest = 
           if pe then vprintln 0 (msg + ":  edge " + (valOf pof) v  + " -> " + (valOf pof) dest)  
           if memberp dest nodes then
               let (found, idx) = index.TryGetValue dest
               if not found then //  if (w.index is undefined) then Successor w has not yet been visited; recurse on it
                   let nw = trial dest in Array.set lowindex i (min(lowindex.[i], lowindex.[nw]))
                elif memberp dest !stack
                   then Array.set lowindex i (min(lowindex.[i], idx))
            else
                // boondocks
                ()
        app consider (edges.lookup v)
        // If v is a root node, pop the stack and generate an SCC
        if (lowindex.[i] = i) then
            let scc = ref [] // start a new strongly-connected component
            let mutable continueLooping = true
            while continueLooping do
                let w = pop()
                scc := w :: !scc // add w to current strongly connected component
                if w = v then continueLooping <- false;
            if length !scc > 1 then
                if pe then vprintln 0 ("scc: " + msg + " " + sfold (valOf pof) (!scc))             // output the current strongly connected component
                mutadd m_ans !scc
        i
    and trial v =
        let (found, ov) = index.TryGetValue v
        if not found then strongconnect v else ov
    let _ = map trial nodes
    !m_ans



// OLD Articulation points finder.
let tarjan2_<'t when 't:equality> (nodes:'t list) (edgemap:ListStore<'t, 't>) =
    let vd = 4
    let mutable m_nextime = 0 
    let m_apoint_lst = ref []
    let lowvals = new OptionStore<'t, int>("lowvals")
    let visited = new OptionStore<'t, int>("visited")
    let parents = new OptionStore<'t, 't>("parents")
    let yielder msg uu =
        if vd>=4 then vprintln 4 (sprintf "AP AT %s root %A" msg uu)
        mutadd m_apoint_lst uu
    let min a b = if a < b then a else b
    let rec visit u =
        if not_nonep(visited.lookup u) then ()
        else
        let mutable count = 0
        let vtime = m_nextime + 1
        m_nextime <- vtime
        visited.add u vtime
        lowvals.add u vtime
        if vd>=4 then vprintln 4 (sprintf "tarjan2: Visit %A at time %i" u vtime)
        let parent_o = parents.lookup u
        let sonfun dest =
            if dest = u then
                if vd>=4 then vprintln 4 (sprintf "tarjan2: Ignore reflexive edge on node %A" dest)
            else
            match visited.lookup dest with
                | None ->
                    count <- count + 1
                    parents.add dest u // Could affect parent_o for reflexive edges.
                    visit dest
                    let lv = min (valOf_or_fail "L3241a" (lowvals.lookup u)) (valOf_or_fail "L3242b" (lowvals.lookup dest))
                    lowvals.add u lv 
                    match parent_o with
                        | None -> // When u is (a) root with two or more children. 
                            if count > 1 then yielder "no parent" u
                        | Some pp ->
                            // When u is non-root and lowval of a child is greater than discovery time of u. 
                            let dtime = valOf_or_fail "L3256a" (lowvals.lookup dest)
                            if dtime > vtime then yielder (sprintf "%A->%A  pp=%A  dtime %i > vtime %i" u dest pp dtime vtime) u
                    //lv
                | Some disctime ->
                    match parent_o with
                        | Some p when p = dest -> ()
                        //| _ -> lowvals.add u (min (valOf_or_fail "L3242a" (lowvals.lookup u)) (valOf_or_fail "L3242b" (lowvals.lookup dest)))
                        | _ -> lowvals.add u (min (valOf_or_fail "L3242a" (lowvals.lookup u)) (valOf_or_fail "L3242b" (visited.lookup dest)))                        
                    //lv
                        
        app sonfun (edgemap.lookup u)
    app visit nodes 
    if vd>=4 then vprintln 4 (sprintf "tarjan2: Found %i articulation points." (length !m_apoint_lst))
    !m_apoint_lst


// Articulation points finder.
let tarjan2<'t when 't:equality> (nodes:'t list) (edgemap:ListStore<'t, 't>) =
    let vd = 4
    let mutable m_nextime = 0 
    let m_apoint_lst = ref []
    let lowvals = new OptionStore<'t, int>("lowvals")
    let visited = new OptionStore<'t, int>("visited")
    let yielder msg uu =
        if vd>=4 then vprintln 4 (sprintf "AP AT %s root %A" msg uu)
        mutadd m_apoint_lst uu
    let min a b = if a < b then a else b

    let rec visit pp v =
        if not_nonep(visited.lookup v) then ()
        else
        let mutable count = 0
        let vtime = m_nextime + 1
        m_nextime <- vtime
        visited.add v vtime
        lowvals.add v vtime
        if vd>=4 then vprintln 4 (sprintf "tarjan2: Visit %A at time %i" v vtime)
        let sonfun dest =
            if dest = v then
                if vd>=4 then vprintln 4 (sprintf "tarjan2: Ignore reflexive edge on node %A" dest)
            else
                match visited.lookup dest with
                    | Some disctime ->
                        let lv = min (valOf_or_fail "L3241a" (lowvals.lookup v)) (valOf_or_fail "L3242b" (lowvals.lookup dest))
                        lowvals.add v lv 
                    
                    | None ->
                        visit (Some v) dest
                        let lv = min (valOf_or_fail "L3241a" (lowvals.lookup v)) (valOf_or_fail "L3242b" (lowvals.lookup dest))
                        lowvals.add v lv 
                        // When v is non-root and lowval of a child is greater than discovery time of u. 
                        let dtime = valOf_or_fail "L3256a" (lowvals.lookup dest)
                        let vtime = valOf_or_fail "L3256a" (visited.lookup v) 
                        if dtime >= vtime && not_nonep pp then yielder (sprintf "%A->%A  pp=%A  dtime %i > vtime %i" v dest pp dtime vtime) v
                        count <- count + 1
                      
        app sonfun (edgemap.lookup v)
        if nonep pp && count > 1 then yielder "solo" v

    app (visit None) nodes 
    if vd>=4 then vprintln 4 (sprintf "tarjan2: Found %i articulation points." (length !m_apoint_lst))
    !m_apoint_lst

(*
   https://cp-algorithms.com/graph/cutpoints.html
    void dfs(int v, int p = -1) {
        visited[v] = true;
        tin[v] = low[v] = timer++;
        int children=0;
        for (int to : adj[v]) { if (to == p) continue;
          if (visited[to]) { low[v] = min(low[v], tin[to]); }
          else {
                  dfs(to, v);
                  low[v] = min(low[v], low[to]);
                  if (low[to] >= tin[v] && p!=-1) IS_CUTPOINT(v);
                  ++children;
                }
          }
        if(p == -1 && children > 1)  IS_CUTPOINT(v);
        }
    *)

let tarjan1_test () =
    let edges0 = [ (0,1); (1,2); (2,3); (3,4); (4,5); (5,6); (6,7); (7,8); (8,9);   (5,3); (6,3); (9,8); ]
    //let edges = map (fun (a,b) -> ("n" + i2s a, "n" + i2s b)) edges0
    let pof = Some i2s
    // Fold this over edges to get node list:
    let (nodes, edges) =
        let edges = new ListStore<int, int>("test-edges")
        let install (f, tl) = app (fun x-> edges.add f x) (map snd tl)
        let col = (generic_collate fst edges0)
        app install col
        (map fst col, edges)
    let tarjan_nodes c (a,b) = singly_add a (singly_add b c)
    vprintln 0 ("tarjan_test1: " + sfold (fun x-> i2s x) nodes)
    tarjan1<int> "tarjan_test" pof 4 (nodes, edges)





let g_next_procid = ref 1
let fresh_procid() =
    let r = !g_next_procid
    g_next_procid := r+1
    r


type waypoint_t =
    | WP_wp of stutter_t * hbexp_t * int * string      // A real waypoint
    | WP_other_string of stutter_t * hbexp_t * string * string  // Another string of interest

let wpToStr = function
    | WP_wp(pp, g, nn, ss)        -> sprintf "WP:nn=%i ss=%s " nn ss
    | WP_other_string(pp, g, a,b) -> a + ":" + b
        
let find_waypoint_or_interesting_pli ww vd local_pli = // User phase change or waypoints
    let wp_scan (pp, g) (fname, gis) cc args =
        let rec togate cc = function
            | W_string(gg, _, _)::b::c when gg=s_autoformat -> togate cc (b::c)

            | W_string(ss, _, _)::_ ->
                let nv = if fname = "hpr_KppMark" then WP_wp (pp, g, -1, ss) else WP_other_string(pp, g, fname, ss)
                singly_add nv cc

            | [] ->
                //let _ = vprintln 10 (sprintf "+++ fname=%s Failed to find notable PLI or waypoint string in " fname + sfold xToStr args)
                singly_add (WP_other_string(pp, g, fname, "GSAI:" + fname)) cc

            | h::tt -> togate cc tt
        togate cc args
        
    let scan_for_waypoint cc = function  
            | XIRTL_pli(pp, g, (fgis, ordering), args) ->
                //let _ = if vd then vprintln 0 (sprintf "local_pli/waypoint 2/2 is %A" args)
                wp_scan (pp, g) fgis cc args

            | XRTL(pp, g, lst) ->
                let scn cc = function
                    | Rpli(g1, (fgis, ordering), args) ->  
                        //let _ = if vd then vprintln 0 (sprintf "local_pli/waypoint 1/2 is %A" args)
                        wp_scan (pp, ix_and g g1) fgis cc args
                    | _ -> cc
                List.fold scn cc lst
    let notables = List.fold scan_for_waypoint [] local_pli
    let _ =
        if vd then
            vprintln 0 ("From " + sfold xrtlToStr local_pli)
            reportx 0 "GSAI found" wpToStr notables
    (notables:waypoint_t list)


//
// Explore a finite state machine: prints lists of reachable and unreachable states. 
// Makes entries in the arc_database collection.
let explore_report ww vd thread reset_states (arc_database:ListStore<int, hbexp_t * int>) pc_xstates =
    let ww = WF 2 "explore_report" ww "finished"
    let pc_states = map lc_atoi32 pc_xstates
    let kprint n = i2s n + " -er--> " + sfold (fun (g, d) -> i2s d) (arc_database.lookup n)
    
    let rec explore doner = function
        | [] ->
            vprintln 2 (xToStr thread + sprintf ": states explored were: " + sfold i2s doner)
            vprintln 2 "-"
            //dev_println "crlf-flush"
            doner
        | opc::t when memberp opc doner -> explore doner t
        | opc::t ->
            if vd >= 4 then vprint 4 ("Explore: old pc=" + i2s opc + " ")
            let n =
                match arc_database.lookup opc with
                    | [] ->
                        vprintln 3 (xToStr thread + sprintf " explore: successor pc values for pc=%i not found (will exit)"  opc)
                        []
                    | transitions ->
                        let n = map snd transitions
                        if vd>=4 then vprintln 4 ("   Successors: " + sfold (fun (g, d) -> i2s d) transitions)
                        let spotout (g, d) = sprintf "\n       ->  %i/%s" d (xbToStr_concise g)
                        let _ =
                            if contains_repetitions (map snd transitions) then
                                let dests = list_once (map snd transitions)
                                let z =
                                    let sel_dest d =
                                        let sed cc (g, d1) = if d=d1 then ix_or g cc else cc
                                        (List.fold sed X_false transitions, d)
                                    map sel_dest dests
                                if vd >= 4 then vprintln 4 (sprintf "It is fine that there is more than one transition from %A to same dest: %s" opc ("   Successors: " + sfold spotout transitions + " disjunctions are " + sfold spotout z))
                        n
            explore (opc::doner)  (n @ t)

    let reachable = explore [] reset_states
    // let states = map fst (System.Collections.Generic.Dictionary.toList arc_database)
    if vd >= 3 then
        let unreachable = list_subtract(pc_states, reachable)
        vprintln 3 (sprintf "%i unreachable states" (length unreachable))
        reportx 3 "Unreachable states in input machine" kprint unreachable
    let ww = WF 2 "explore_report" ww "finished"
    ()


let m_arc_idx_counter = ref 800
let next_global_arc_no () =
    let r = !m_arc_idx_counter
    mutinc m_arc_idx_counter 1
    r

// Project/Convert a list of RTL statements to SP_fsm form.  cf fsm_recode

// This code could potentially use any variable as the 'program counter' - it will be semantically correct.
// Formally we are finding the Shannon co-factors with respect to the nominated PCs values.
// This performs the opposite to fsm_flatten_to_rtl.
let project_arcs_to_fsm ww msg vd avoid_dontcare_stall thread hardf arcs_top =
    let tids = xToStr thread
    let ww = WF 3 "project_arcs_to_fsm" ww (msg + sprintf ": Start thread = %s, with %i arc pool." tids (length arcs_top))

    let is_this_machine = function // note a thread is a machine.
        | XRTL(Some(pc, state'), _, _)
        | XIRTL_pli(Some(pc, state'), _, _, _)  -> thread=pc
        | _ -> false
    let (arcs_here, arcs_other_machines) = List.partition is_this_machine arcs_top

    let ww = WF 3 "pack_arcs_to_fsm" ww (sprintf "selected  %i arcs for this machine/thread." (length arcs_here))

    let is_old_pc p = 
        let f = (p=thread)
        f

    let stateNames = new OptionStore<int, hexp_t>("statenames")
    let pwh (state) = (state, hardf)
    let (old_trajectory, workarcs) =
        let old_trajectory = new ListStore<int, hbexp_t * int>("old_trajectory")
        let collect_old_nsf rtl cc = //nsf=next-state function.
            match rtl with
                | XRTL_nop ss                                -> rtl :: cc
                | XIRTL_buf _                                -> rtl :: cc                
                | XIRTL_pli(Some(pc, state'), g, fgis, args) -> rtl :: cc
                | XRTL(Some(pc, pcv), ga, lst) ->
                    let scn rop cc =
                        match rop with
                        | Rnop ss               -> rop :: cc
                        | Rpli (gb, fgis, args) -> rop :: cc                    
                        | Rarc(gb, lhs, rhs) ->
                            if is_old_pc lhs then
                                let l = lc_atoi32 pcv
                                let _ = stateNames.add l pcv
                                let r = match rhs with
                                            | X_undef -> -1 // Transfer to X_undef (aka boondocks) denotes halt.
                                            | r -> lc_atoi32 r
                                //let _ = dev_println (sprintf "nsf_add: Saving old/goto transition from %i %s    to    %i %s" l (xToStr pcv) r (xToStr rhs))
                                let _ = stateNames.add r rhs
                                old_trajectory.add l (ix_and ga gb, r)
                                cc
                            else
                                //dev_println (sprintf " not a goto for lhs=" + netToStr l)
                                rop :: cc
                    match List.foldBack scn lst [] with
                        | [] -> cc
                        | items -> XRTL(Some(pc, pcv), ga, items) :: cc
        let workarcs = List.foldBack collect_old_nsf arcs_here []
        (old_trajectory, workarcs) // Arcs without gotos are left.

    let find_states pc_states = function
        | XIRTL_pli(Some(pc, state), _, _, _)
        | XRTL(Some(pc, state), _, _)           ->  singly_add state pc_states        

    let pc_states = List.fold find_states [] arcs_here    // Src states
    let all_states =
        let dest_states = [ for x in stateNames -> x.Value ]  // Dest states
        lst_union pc_states dest_states
    for sn in pc_states do stateNames.add (lc_atoi32 sn) sn done
    
    if vd>=4 then reportx 4 (msg + sprintf ": project_arcs_to_fsm: tids=%s Input thread PC States" tids) xToStr all_states
    
    explore_report (WN "explorer" ww) vd thread [0] old_trajectory pc_states    

    // It is correct to delete the pp (i.e. edge_guard) from the RTL in the FSM branches.
    // We also wish to avoid repeating guard factors in the increasingly nested guard expressions.
    let booldiv nn dd = // Use alternatively simplify_assuming
        match ix_bdivide nn dd with
            | Some g' ->
                //vprintln 0 (sprintf "fsm_project nn=%s  dd=%s  --> %s " (xbToStr nn) (xbToStr dd) (xbToStr g'))
                g'
            | None    ->
                //vprintln 0 (sprintf "fsm_project nn=%s  dd=%s  --> won't go " (xbToStr nn) (xbToStr dd))
                nn
        
    let de_pp gxition = function
        | XIRTL_pli(Some(pc, state'), g, fgis, args) ->
            let g'= booldiv g gxition
            //dev_println (sprintf " de_pp g' is " + xbToStr g')
            XIRTL_pli(None, g', fgis, args)
        | XRTL(pp, g, lst) ->
            let g1 = booldiv g gxition
            let g2 = ix_and g g1
            //dev_println (sprintf " de_pp g1=%s g2=%s " (xbToStr g1) (xbToStr g2))
            let refine_grds = function
                | Rarc(g, l, r)       -> Rarc(booldiv g g2, l, r)
                | Rpli(g, fgis, args) -> Rpli(booldiv g g2, fgis, args)
                | Rnop s -> Rnop s
            let ans = XRTL(None, g1, map refine_grds lst)
            //vprintln 0 (sprintf "ans fsm_project is " + xrtlToStr ans)
            ans


    let dest_report() = sfoldcr_lite netToStr all_states

    let xprocess_input_mc_state workpool state =
        let is_this_state = function
            | XIRTL_pli(Some(pc, state'), _, _, _)
            | XRTL(Some(pc, state'), _, _) -> thread=pc && state'=state
        let whence = lc_atoi32 state
        let (arcs_here, arcs_otherstates) = List.partition is_this_state workpool

        let work_here = map (fun x -> (ref false, x)) arcs_here

        let garc2 dest g work =
                { g_null_vliw_arc with
                    old_resume= None
                    pc=thread
                    resume=state
                    iresume= xi_manifest "iresume" state  // State names have pretty forms as well as an underlying integer.
                    dest=dest
                    idest= (if dest=X_undef then -1 else xi_manifest "idest-L3157" dest)
                    gtop=g
                    cmds=work
                    eno= next_global_arc_no()
                }

        // collate_work1: find work whose guard is not disjoint with this transition guard and include it as Mealy work.
        let collate_work1 g_xition =
            let collate_work (bf, rtl) cc =
                let pull_pred g1  =
                    match ix_and g_xition g1 with
                        | X_false ->
                            //let _ = vprintln 0 (sprintf "   disc  %s  /\  %s" (xbToStr_concise g_xition) (xbToStr_concise g1))
                            false
                        | x ->
                            //let _ = vprintln 0 (sprintf "   keep  %s  /\  %s  => %s " (xbToStr_concise g_xition) (xbToStr_concise g1) (xbToStr x))
                            true
                match rtl with
                    | XIRTL_pli(Some(pc, state'), g, fgis, args) -> if pull_pred g then (bf := true; de_pp g_xition rtl :: cc) else cc
                    | XRTL(pp, g, lst)                           -> if pull_pred g then (bf := true; de_pp g_xition rtl :: cc) else cc
                    | XRTL_nop ss                                -> (bf := true; rtl :: cc)

            List.foldBack collate_work work_here []


        let rec fsm_lift (g_xition, dest) ccb =
            // Some work might be independent of our pc and so is ancillary global RTL: we filtered out arcs_other_machines earlier based on pp but there is potentially further to be found algebraically. We should perhaps find it.
            // Some work will be replicated over a few but not all arcs unless it is the ancillary mentioned in previous sentence.
            // The paired bool ref is to flag any that does not get picked up at least once. But we dont look at it now.
            // If we make all transitions dependent on +ve guard conditions then, under RTL_SIM, we stall on don't care owing to RTL semantics.  So we support a avoid_dontcare_stall mode where one guard is replaced with the NOR of the remainder.
            let work = collate_work1 g_xition
            //reportx 0 ("pulled de_pp work for state " + xToStr state) xrtlToStr work
            if dest = -1 then garc2 X_undef g_xition work :: ccb
            else
            match stateNames.lookup dest with
                | Some naz -> garc2 naz g_xition work :: ccb

                | None when nullp work ->
                    vprintln 3 (sprintf "fsm_lift: g_xition=%s branch destination %i missing but no work on that arc" (xbToStr g_xition) dest)
                    ccb
                | None ->
                        
                    sf(sprintf "fsm_lift: g_xition=%s branch destination missing. dest wanted=%i  dests available=%s" (xbToStr g_xition) dest  (dest_report()))                         

        let aces = old_trajectory.lookup whence
        let non_move_condition = List.fold (fun cc (g_xition, _) -> ix_and (xi_not g_xition) cc) X_true aces
        let has_no_non_move_condition = xi_isfalse non_move_condition
        if vd >= 4 then vprintln 4 (sprintf "tids=%sL: has_no_non_move_condition=%A avoid_dontcare_stall=%A" tids has_no_non_move_condition avoid_dontcare_stall)
        let aces =
            if avoid_dontcare_stall && has_no_non_move_condition && length aces >= 2 then
                // Here we hopefully introduces a negated branch condition, which is treated as true in RTL_RENDER when applied to dontcare, but this could be somewhat fragile and destoryed in subsequent recipe stages.  If there is a no_move_condition that should serve, so we don't do it.
                let (chosen, remainder) = (hd aces, tl aces) // Choose one at 'random' to progress on.
                let chosen_condition = List.fold (fun cc (g_xition, _) -> ix_and (xi_not g_xition) cc) X_true remainder
                let zapped = (chosen_condition, snd chosen)
                zapped::remainder
            else aces
        let move_arcs = List.foldBack fsm_lift aces []
        let reflexive_arcs = 
        // let non_move_work = List.filter (fun (bf, rtl) -> !bf = false) work_here
            if has_no_non_move_condition then []
            else
                let non_move_work = collate_work1 non_move_condition
                //if vd>=4 then vprintln 4 (sprintf "non-mover vliw_arc for state %s : g=%s items=%s" (xToStr state) (xbToStr non_move_condition) (sfoldcr xrtlToStr non_move_work))
                [garc2 state non_move_condition (* (map (de_pp non_move_condition) *) non_move_work]

        (reflexive_arcs @ move_arcs, arcs_otherstates)
        
    let vliw_arcs =
        let rec folder work cc = function
            | [] ->
                if not_nullp work then sf "project_arcs_to_fsm: work left over"
                cc
            | state::states ->
                //vprintln 0 (sprintf "Iterate " + xToStr state)
                let (arcs, work') = xprocess_input_mc_state work state
                folder work' (arcs @ cc) states
        folder workarcs [] pc_states


    if vd>=4 then reportx 4 (msg + ": project_arcs_to_fsm: Rebuilt FSM (work not listed)") fsm_arcToStr vliw_arcs

    //reportx 0 "project_acs_to_fsm: Rebuilt FSM" fsm_arcToStr_full vliw_arcs
    // All arcs in a major state should have disjoint guards, but we statically prioritise them here anyway.

    let fsm =
        if nullp all_states then
            let _ = WF 3 "project_arcs_to_fsm" ww (sprintf "Finish thread = %s, no states and hence no FSM created." (xToStr thread))
            []
        else
            let start_state_no =
                match stateNames.lookup 0 with
                    | Some v -> v
                    | None ->
                        sf("project_arcs_to_fsm: No start state found in states=" + dest_report())
            let fsm_info = 
               { g_default_fsm_info with
                   pc=            thread
                   exit_states=   [] // Could put here those with no successor, ie list_subtract pc_states dest_states 
                   controllerf=   length all_states > 1
                   start_state=   (start_state_no, hardf)
                   resumes=       map pwh all_states // Boondocks state is included now
                   inst_rom=      None
                   inst_set=      None
               }
            let fsm = SP_fsm(fsm_info, vliw_arcs)
            // Perhaps print out back-projected FSM
            if vd >=4 then vprintln 4 ("Reprojected FSM IS=\n" + hblockToStr ww (H2BLK(g_null_directorate, fsm)))
            let _ = WF 3 "project_arcs_to_fsm" ww (sprintf "Finish thread = %s, with %i resumes and %i total arcs." (xToStr thread) (length all_states) (length vliw_arcs))
            [fsm]
            
    (fsm, arcs_other_machines, old_trajectory, stateNames)


// Compute a reverse transition matrix for an HPR SP_fsm finite-state machine.
// Returns the forward one too.  These are both in terms of resumes. Similar datastructures exist for the arcs, as returned by ...
let fsm_compute_inverted_xition_mapping ww vd msg fsm =
    let ww = WF 3 "compute_fsm_inverted_xition_mapping" ww (msg+": Start")
    match fsm with
        | SP_fsm(info, arcs) ->
            //let pc_states = map (fun (x, _) -> lc_atoi32 x) info.resumes 
            let fwd_resume_database = new ListStore<int, int>("forward resume index")
            let rev_resume_database = new ListStore<int, int>("reverse resume index")
            let arcs_from_pc = new ListStore<int, int>("arcs_from_pc")
            let arcs_to_pc = new ListStore<int, int>("arcs_to_pc")                        
            let m_iresumes = ref []
            let fait0 arc =
                if arc.dest = X_undef then ()
                else
                    let from_pc = lc_atoi32 arc.resume
                    let to_pc = lc_atoi32 arc.dest
                    mutaddonce m_iresumes from_pc
                    mutaddonce m_iresumes to_pc                
                    fwd_resume_database.addflexi (singly_add) from_pc to_pc
                    rev_resume_database.addflexi (singly_add) to_pc from_pc
                    arcs_from_pc.addflexi (singly_add) from_pc arc.eno
                    arcs_to_pc.addflexi (singly_add)   to_pc   arc.eno
            app fait0 arcs
            let ww = WF 3 "compute_fsm_inverted_xition_mapping" ww (msg+": Done")


            let earlier_arc_database = new ListStore<int, int>("earlier arc index")
            let later_arc_database = new ListStore<int, int>("later arc index")

            let poggle pc =
                let pairs = cartesian_pairs (arcs_from_pc.lookup pc) (arcs_to_pc.lookup pc)
                //let _ = dev_println (sprintf "poggle/piggle %i  %i" pc (length pairs))
                let piggle (a0, a1) =
                    earlier_arc_database.addflexi (singly_add) a0 a1
                    later_arc_database.addflexi (singly_add) a1 a0
                app piggle pairs
            app poggle !m_iresumes

            let _ =
                if vd > 10 then vprintln 2 (sprintf "Arc Database Information report skipped")
                else
                    let yy ss = if vd>=4 then vprintln 4 ss
                    yy "Arc Database Information"
                    let pf k v = yy(sprintf "  resume fwd  E%i -> %s" k (sfold i2s v))
                    for x in (fwd_resume_database:ListStore<int, int>) do pf x.Key x.Value done

                    let pf k v = yy(sprintf "  resume rev  E%i -> %s" k (sfold i2s v))
                    for x in (rev_resume_database:ListStore<int, int>) do pf x.Key x.Value done

                    let pf k v = yy(sprintf "  earlier (preceeding) arc  E%i -> %s" k (sfold i2s v))
                    for x in (earlier_arc_database:ListStore<int, int>) do pf x.Key x.Value done

                    let pf k v = yy(sprintf "  later (following) arc fwd  E%i -> %s" k (sfold i2s v))
                    for x in (later_arc_database:ListStore<int, int>) do pf x.Key x.Value done
                    yy "Arc Database Information End"         
            
            (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database)

// Collate fsm edges indexed by resume.
// Note that an edge containing hpr_finish() call is really a transition to a nominated exit site and is two edges if the call has a guard that may or may not hold.
// Although real hardware does not exit, many of our tests for RTL_SIM do exit.
let fsm_edges_by_resume fsm =
    match fsm with
        | SP_fsm(info, arcs) ->
            let rec edge_by_resume arcs = function
                | [] -> ([], arcs)
                | (X_undef, _)::tt -> // X_undef as a resume is elsewhere called boondocks
                    vprintln 3 (sprintf "SP_fsm has an X_undef listed as a resume (better to leave them off the info.resumes list). PC=%s" (xToStr info.pc))
                    edge_by_resume arcs tt
                | (hh, hardf)::tt ->
                    let r = lc_atoi32 hh
                    let (our_arcs, other_arcs) = List.partition (fun ma -> lc_atoi32 ma.resume = r) arcs
                    let (cc, other_arcs) = edge_by_resume other_arcs tt
                    ((hh, our_arcs)::cc, other_arcs)                           
            let (ans, leftover) = edge_by_resume arcs info.resumes
            cassert(nullp leftover, "fsm_edges_by_resume: arcs left over")
            rev ans


// Slow run time on complex control flow balance equations is largely avoided by solving only for the fanin nodes.
// We greatly reduce the number of nodes (resumes) by redirecting each arc that only has one successor to that successor.
let abstract_core_fsm ww vd graph arity =   
    let vdp = false
    let ww = WF 3 "simplify FSM" ww "Start"
    let passing_nodes =
        let monodest_filter (mm:Map<int, int>) src dests =
            // We can get twice as many nodes to solve if we include the one-src condition.
            // Better to get solutions for fan out points and forwards substitute from them to the others: that step has linear cost instead of cubic.
            let onesrc = true // length(valOf_or_nil(inverted_index.TryFind src)) < 2
            if onesrc && length dests = 1 then mm.Add(src, hd dests) else mm
        Map.fold monodest_filter Map.empty graph

        // An infinite loop may have no conditional branches in it.  This is a degenerate case where
        // a process of redirecting a node to its successor will not terminate. So if a node has becomes its own dest we do no rewrite that arc.
    let timeout = (arity + 1) * 2 // Arity+2 should be enough?
    let rec iterate cnt graph = 
        vprintln 3 (sprintf "Iterative graph reduction step %i." cnt)
        if cnt > timeout then
            hpr_yikes (sprintf "Timeout at iterative graph reduction step %i." cnt)
            graph
        else
        let mutable donesome = false
        let reduced_graph =
            let reduce (mm:Map<int, int list>) src dests =
                let reduce1 dest cc =
                    if dest = src then singly_add dest cc
                    else
                        match passing_nodes.TryFind dest with
                            | None          -> singly_add dest cc
                            | Some redirect -> singly_add redirect cc
                let newdests = List.foldBack reduce1 dests []
                if newdests <> dests then donesome <- true
                //vprintln 0 (sprintf "Newdests src=%i: %s" src (sfold i2s newdests))
                mm.Add(src, newdests)
            Map.fold reduce Map.empty graph
        if donesome then iterate (cnt+1)  reduced_graph else reduced_graph


    let graph = iterate 0 graph
    if vdp then dev_println (sprintf "Now find reachable core")
                 
    let reachable_states =
        let rec reachscan (set:Set<int>) = function
            | [] -> set
            | h::tt ->
                if Set.contains h set then reachscan set tt
                else
                    let dests = valOf_or_nil (graph.TryFind h)
                    let set = set.Add h
                    reachscan set (dests @ tt)
        reachscan Set.empty [0]

    let arity = Set.count reachable_states
    if vdp then dev_println (sprintf "Now find reachable core arity=%i" arity)        
    let refined_graph = // Leave only the reachable part of the graph.
        let refiner (mm:Map<int, int list>) src dests =
            if Set.contains src reachable_states then mm.Add(src, dests) else mm
        Map.fold refiner Map.empty graph
        
    (refined_graph, arity)




//
// Solve balance equations for visit ratios.
// We essentially need to find x in x=Ax where A is the state transition probability matrix.
//
let visit_ratio_balancer vd ww arity graph = 
    let coefs =
        let lossfactor = 0.95 // By allowing a little loss we get a stable solution even if there are infinite loops, since these will trail off exponentially.
        let coefs = Array2D.create arity arity 0.0
        let pog state src dests =
            if not_nullp dests then
                let ila = length dests
                let branch_ratio = lossfactor / (double ila) // Assuming equal branching probability is highly inaccurate in itself but ends up ranking the inner loops highest nonetheless.
                let ff = src
                let pig dest =
                    if vd >= 4 then vprintln 4 (sprintf "   abstracted arc    %i -> %i  prob %f " src dest branch_ratio)
                    Array2D.set coefs dest ff (branch_ratio + coefs.[dest, ff]) // Sum in case of more than once arc from ff to tt.
                app pig dests
        Map.fold pog () graph
        for e in 0 .. arity-1 do Array2D.set coefs e (e) (coefs.[e,e] - 1.0) done // Subtract identity matrix.  
        coefs
    let rhs = Array.create arity 0.0
    Array.set rhs 0 -1.0 // Transition once into entry block, but this is put on rhs hence -ve.
    if vd >= 5 then vprintln 5 (sprintf "Balance Coefs are %A" coefs)
    let occupancy_ratios = fitters.SimuSolve vd coefs rhs  // Simusolve solves the equations.
    //dev_println (sprintf "Occupancy ratios determined.")        
    (occupancy_ratios)
    




// Profile-directed feedback will make this much more accurate, but rank order might seldom be affected.
// We solve Bx=r where B is derived from A by subtracting an identity matrix and r is a 0 column vector with -1.0 at the top for system entry.
// Real hardware, which is what we are interested in, does not have an exit arc, hence we will normally have infinite visits to the arcs of a multistage loop.
let balance_based_performance_prediction ww vd fsm_info edges_by_resume =
    let ww = WF 3 "balance_based_performance_prediction" ww "Start"
    let vdp = false
    let resumes:hexp_t list = map fst edges_by_resume
    let old_arity = (length edges_by_resume)
    vprintln 3 (sprintf "Input arity=%i" old_arity)    
    let (index, hwm) =
        let make_index (mm:Map<int, hexp_t * vliw_arc_t list>, hwm) (resume, arcs) =
            let pc = lc_atoi32x resume
            (mm.Add(pc, (resume, arcs)), max pc hwm)
        List.fold make_index (Map.empty, 0) edges_by_resume

    let graph =
        let graph_make (mm:Map<int, int list>) (resume, arcs) =
            let src = lc_atoi32x resume
            let dests =
                let gdest arc cc =
                    if arc.dest = X_undef then cc
                    else singly_add (lc_atoi32x arc.dest) cc
                List.foldBack gdest arcs []
            mm.Add(src, dests)
        List.fold graph_make Map.empty edges_by_resume 

    let inverted_index =
        let make_iindex mm src dests =
            let ii (mm:Map<int, int list>) dest =
                let ol = valOf_or_nil(mm.TryFind dest)
                mm.Add(dest, src::ol)
            List.fold ii mm dests
        Map.fold make_iindex Map.empty graph
            
    let (refined_graph, arity) = abstract_core_fsm ww vd graph old_arity
    // The ires field in resume2 is not necessarily a consecutive set of integers.


    let start_state = hd resumes // By convention.
    vprintln 3 (sprintf "balance_based_performance_prediction Arity=%i.  Entry block is %s" arity (xToStr start_state))
     //    vprintln 3 (sprintf "Resumes are " + sfold xToStr resumes)

//    let branch_ratios = new OptionStore<int, double>("branch_probs")



    let idxof = function
        | X_undef -> sf "idxof of undef"
        | resume  -> lc_atoi32x resume

#if SPARE
    let idxof resume = 
        match op_assoc r enumeration with
            | Some ans -> ans
            | None ->

                vprintln 0 (sprintf "+++ Available resumes were " + sfold (fst>>xToStr) enumeration)
                sf (sprintf "performance predictor: idxof fail for %A" r)

    let backarcs = // For reporting only infact
        let m_backarcs = ref []
        let rec barc_explore0 stack pc =
            match graph.TryFind pc with
                | None -> ()
                | Some lst -> app (barc_explore1 (pc::stack)) lst
        and barc_explore1 stack dest =
            if memberp dest stack then
                mutaddonce m_backarcs arc.eno
            else barc_explore0 stack dest
        barc_explore0 [] [0] // start_state
        !m_backarcs
#endif
    let branch_ratios =
        let branch_ratios = new OptionStore<int, double>("branch_probs") // Probability a branch will be taken. Indexed by edge no. 
        let pog = function
            | (X_undef, _) -> ()
            | (resume, []) -> () //     '(hexp_t * vliw_arc_t list) list'    
            | (resume, arcs) ->
                let ila = length arcs
                let lossfactor = 0.95 // By allowing a little loss we get a stable solution even if there are infinite loops, since these will trail off exponentially.
                let branch_ratio = lossfactor / (double ila) // Assuming equal branching probability is highly inaccurate in itself but ends up ranking the inner loops highest nonetheless.
                let ff = idxof resume
                let pig (arc:vliw_arc_t) =
                    if arc.dest = X_undef then ()
                    else
                        branch_ratios.add arc.eno branch_ratio
                        //let tt = idxof arc.dest
                        //let backarcf = false // memberp arc.eno backarcs
                        //if vd >= 4 then vprintln 4 (sprintf "    arc %i   %i -> %i  prob %f  %s" arc.eno ff tt branch_ratio (if backarcf then "BACKARC" else ""))
                for arc in arcs do pig arc done
        for x in edges_by_resume do pog x done
        branch_ratios

      
    let occupancy_ratios = // Find visit ratio with pack and unpack around a simusolve.
        let (packed_graph, pidxof, unpacker) =
            let m_next = ref 0
            let alloc() =
                let rr = !m_next
                m_next := rr + 1
                rr
            let (pack_map, unpack_map) =
                let gmap (pm:Map<int, int>, um:Map<int, int>) key vale_ =
                    match pm.TryFind key with
                        | Some _ -> (pm, um)
                        | None ->
                            let n = alloc()
                            //dev_println(sprintf "  pindxof %i -> %i" key n)
                            (pm.Add(key, n), um.Add(n, key))
                Map.fold gmap (Map.empty, Map.empty) refined_graph

            let pidxof_or_fault d =
                match pack_map.TryFind d with
                    | Some d' -> d'
                    | None -> sf (sprintf "missing pack map entry d=%i" d)

            let packed_graph =
                let grmap (mm:Map<int, int list>) key vales = mm.Add(pidxof_or_fault key, map pidxof_or_fault vales)
                Map.fold grmap Map.empty refined_graph

            let unpacker_ d =
                match unpack_map.TryFind d with
                    | Some d' -> d'
                    | None -> sf ("missing unpack map entry")

            let pidxof d = pack_map.TryFind d

            (packed_graph, pidxof, unpacker_)

        let abstract_occupancy_ratios = visit_ratio_balancer vd ww arity packed_graph
        let a:float[] = abstract_occupancy_ratios
        let occupancy_ratios = Array.create (hwm+1) None  
        let rec unpack resume =
            let p0 = idxof resume
            ignore (unpack1 [] p0)
        and unpack1 stack p0 =
            match occupancy_ratios.[p0] with
                | Some ratio -> (p0, ratio)
                | None ->
                    let ratio =
                        match pidxof p0 with
                            | Some p1 ->
                                abstract_occupancy_ratios.[p1]
                            | None ->
                                let ratio = searchback stack p0
                                if vdp then dev_println (sprintf "Recorded ratio %f for %i" ratio p0)
                                ratio
                    Array.set occupancy_ratios p0 (Some ratio)
                    (p0, ratio)
            
        and searchback stack pp =
            if memberp pp stack then sf "Unresolved searchback loop encountered"
            match inverted_index.TryFind pp with
                | Some []
                | None ->
                    vprintln 0 (sprintf "searchback: No antecedants to %i !" pp)
                    0.0
                | Some parents ->                    
                    let pairs = map (unpack1 (pp::stack)) parents
                    let sum_flux sofar (parent, occupancy) =
                        let (resume, arcs) = valOf_or_fail "L3918" (index.TryFind parent)
                        let adder sofar arc =
                            if arc.dest=X_undef || lc_atoi32 arc.dest <> pp then sofar
                            else
                                let prob = valOf_or (branch_ratios.lookup arc.eno) 0.0
                                prob * occupancy + sofar
                        List.fold adder sofar arcs
                    List.fold sum_flux 0.0 pairs
 
        app unpack resumes
        occupancy_ratios

        
    let ww = WF 3 "balance_based_performance_prediction" ww "Equations solved"        
    // The occupancy_ratios are based on the entry arc being taken once.  To get occupancy propabilities you would need to divide throughout by a scalar factor such that the matrix contents sum to unity.
    //if vd >= 5 then vprintln 5 (sprintf "Balance solution is %A" occupancy_ratios)

    let ww = WF 3 "balance_based_performance_prediction" ww "Done"        

    // To get the relative frequency of use of each arc we multiply arc branching probability with the starting state occupancy ratio
    // We found the balance for the reduced graph. For passing_nodes, whose value is not in the reduced graph, the frequency will ...
    let arc_freqs =
        let arcf1 ff (cc:Map<int,float>) (arc:vliw_arc_t) =
            if arc.dest = X_undef then cc else
                let tt = idxof arc.dest
                match branch_ratios.lookup arc.eno with
                    | None -> sf "L3380 branch_ratio"
                    | Some ratio ->
                        if ff=tt && (int ratio)=1 then cc.Add(arc.eno, -1.0)
                        else 
                            let arc_freq = ratio * valOf_or occupancy_ratios.[ff] 0.0
                            if vd>=4 then vprintln 4 (sprintf "    arc %i   %i -> %i  arc execution freq estimate is %f" arc.eno ff tt arc_freq)
                            cc.Add(arc.eno, arc_freq)

        let arcf2 cc (resume, arcs) = if resume=X_undef then cc else List.fold (arcf1 (idxof resume)) cc arcs
        List.fold arcf2 Map.empty edges_by_resume
    vprintln 2 (sprintf "balance_based_performance_prediction finished and reported.")
    // To find total_runtime metric we must multiply the arc frequency with its execution time. We do not have times yet, so cannot do it here.
    //let total_runtime =

    (idxof, occupancy_ratios, arc_freqs)






// Canterbury abend registers: We support 3 variants: Nothing, a simple FAIL output and the 8-bit abend syndrome.
// These directorate defines need sharing over all VM execution/render paths, such as diosim, cpp, RTL and perhaps the model checkers.
// 255 is the reset value for the syndrome register, meaning nothing so far recorded.
// The int_run_enable is disabled when the ext_run_enable is asserted or the abend register contains a code other than 255.
let g_directorate_abend  = ("hpr_abend_syndrome", {signed=Unsigned; widtho=Some 8}, OUTPUT, Some 255)
let g_dir_ext_run_enable = ("hpr_ext_run_enable", g_bool_prec, INPUT, None)
let g_dir_int_run_enable = ("hpr_int_run_enable", g_bool_prec, LOCAL, Some 1)

        
// Return the nets needs always needed for an implementation of a given director.
// There's also the sdir nets, returned for various output technologies and detailed implementations.
let get_directorate_nets (dir:directorate_t) =
    let d = (if nonep dir.abend_register then [] else [valOf dir.abend_register])    
    let unary_leds = if nonep dir.unary_leds then [] else [ valOf dir.unary_leds ]
    let ans = list_once(map de_edge dir.clocks @ map f3o3 dir.resets @ dir.clk_enables @ d @ unary_leds)
    lst_subtract ans [  g_construction_big_bang ]     // We remove construction big-bang which is just a pseudo.

let parse_directorate_style ww = function
    | "minimal" ->     DS_minimal
    | "basic" ->       DS_basic
    | "normal" ->      DS_normal
    | "advanced" ->    DS_advanced
    | other ->
        hpr_yikes (sprintf "Ignored illegal directorate style '%s'" other)
        DS_normal


//
//
let install_directorate_parsing_option_defaults ww stagename =
    let ins1 (basis, options, description, mut) =
        let id1 = "directorate-" + basis
        let defaultv = hd options
        mut := defaultv
        let id2 = sprintf "%s-%s" stagename id1
        (Arg_enum_defaulting(id2, options, defaultv, description), (id1, id2))

    let m_startmode  = ref "" // unused
    let m_endmode    = ref ""
    let m_ready_flag = ref ""
    let m_pc_export_flag = ref ""


    let flag_templates =  // Default is listed first. These are deprecated w.r.t. inference from hangmode enumeration now.
   // Why is style not listed here?
   // These attributes are (should be?) used both in the recipe/commandline and, for Kiwi etc, in attribute strings, such as Kiwi.Remote and Kiwi.HardwareEntryPoint.
        [
            ("startmode",   [ "self-start"; "wait-start" ], "Whether the component waits for a req input before reading arguments and starting", m_startmode)
            ("endmode",     [ "auto-restart"; "hang"; "finish" ], "What the component does when it has reach the end of its behaviour. Include simulator (RTL/SystemC) exit using $finish on return from main thread or not.", m_endmode)
            //("ready-flag",  [ "absent"; "present" ], "Whether an ack net is asserted for the last clock cycle of processing", m_ready_flag) // Should be in the protocol?
            ("pc-export",   [ "enable"; "disable" ], "Whether to generate program counter monitoring terminals for simple visibility/debug", m_pc_export_flag)
        ]
    let (settings, tags) = List.unzip(map ins1 flag_templates)

// We replaced kiwic-finish with -kiwife-directorate-endmode

//Examples:
//  -kiwife-directorate-style=minimal
//  -kiwife-directorate-startmode=wait-start
//  -kiwife-directorate-endmode=auto-restart
    let checker ww c3 = 
        let pairs = map (fun (id1, id2) -> (id1, control_get_s stagename c3 id2 None)) tags
        g_rtl_finish_render_enable := op_assoc ("directorate-endmode") pairs = Some "finish" // OLD global way has become a backdoor  - should go taken from the directorate path now.
        pairs
        
    (settings, checker)



(*
 * VMs are instantiated in a typical HDL tree structure.
 * VM definitions are also presented in the same tree structure, with one being defined inside another, but the structure has little or no meaning for definitions except for certain free vars (TODO explain)
 * vm2_definitions_flatten_and_index
 *)
let rec vm2_definitions_flatten_and_index ww arg cc =
    
    match arg with
        | (iinfo, None)  -> cc:Map<string list, hpr_machine_t>

        // We do not ignore non-definitions since the instance typically has a useful simulation model for diosim.
        //| (iinfo, _)  when not iinfo.definitionf -> cc        

        | (iinfo, Some(HPR_VM2(minfo, decls, children, execs, assertions))) -> 
            let body = HPR_VM2(minfo, decls, children, execs, assertions)
            if not iinfo.definitionf then cc
            else
                match cc.TryFind minfo.name.kind with
                    | Some ov ->
                        let _ = hpr_yikes(sprintf "vm2_definitions_flatten_and_index: %s defined more than once. Prior is %s" (vlnvToStr_full minfo.name) (vlnvToStr_full (hpr_minfo ov).name))
                        cc
                    | None ->
                        let cc = cc.Add(minfo.name.kind, body)
                        let _ = vprintln 2 (sprintf "vm2_definitions_flatten_and_index: assimilated %s" (vlnvToStr_full minfo.name))
                        List.foldBack (vm2_definitions_flatten_and_index ww) children cc

// In the same way that X_bnet defines a global flat space (in most respects outside diosim at least), the pipate_cache could/should similarily be global/flat.
type pipate_cache_t = OptionStore<int * int, hexp_t * hexp_t>
type pipate_controller_t = pipate_cache_t * ((hexp_t * hexp_t) list ref)

// Pipeline stage create from X_x markup.
let rez_pipate m_morenets controller = 
    let (pipate_cache, m_pipate_nets) = controller
    let rec pipate = function
        | (X_x(arg, power, _)) as arg00 ->
            if power > 0 then
                dev_println (sprintf "Pipeline stage generate: non-causal pipate +ve power=%i requested for %s" power (xToStr arg00))
                arg00
            else
                let arg = pipate arg
                let nn = x2nn arg
                if power = 0 then arg
                else
                    match (pipate_cache:pipate_cache_t).lookup(nn, power) with
                        | Some (ov, in_) -> ov
                        | None ->
                            let rhs = pipate (xi_X (power+1) arg)
                            let prec = mine_prec g_bounda rhs
                            let nv = newnet_with_prec prec (funique "pipe")
                            pipate_cache.add (nn, power) (nv, rhs)
                            mutadd m_pipate_nets (nv, rhs)
                            mutadd m_morenets nv
                            nv
        | X_blift boolexp -> xi_blift(bpipate boolexp)
        | other -> other

    and bpipate rhs =
        let ans = apply_pip_bx rhs
        //dev_println (sprintf "debug pip_bx   %s -> %s" (xbToStr rhs) (xbToStr ans))
        ans
        
    and apply_pip_bx = function // Fold inside boolean logic for convenience.
        | W_bdiop(V_orred, [(X_x(_, _, _))as arg], pol, _) -> xi_orred (pipate arg)

        | W_cover(cube, meo) -> // Fold inside booleans
            let pip_literal n =   xb2nn(bpipate(deblit n))
            let pip_cover terms = map pip_literal terms
            xi_cover(map pip_cover cube)

        | rhs -> rhs // xi_orred(pipate (xi_blift rhs))

        // let apply_pip_bx rhs = xi_orred(apply_pipate settings perthread (xi_blift rhs))
    (bpipate, pipate)


//
// Reorganise the strange VM2 tree structure into a flat definition list. We return a list of definitions that may contain nested_extensionf and instances.      
// 
let vm_separate ww msg lst =

    let partof_pred = function
        | (ii, None) -> false
        | (ii, Some(HPR_VM2(minfo, decls, sons_, execs, assertions))) -> ii.nested_extensionf  || not ii.definitionf

    let rec split worklist cc =
        if nullp worklist then rev cc
        else
            let mch = hd worklist
            let worklist = tl worklist
            
            match mch with
                | (ii, None) ->
                    // We do not rez via cvipgen and then render. We assume System Integrator and/or other mechanisms (such as vendor tools loading from cvgates.v) will serve for that.
                    // If the mcho field is null, we have neither a worthwhile definition or instance. We have a declaration at best, which we here ignore.
                    split worklist cc
                    
                | (ii, Some(HPR_VM2(minfo, decls, sons_, execs, assertions))) ->
                    let m1 = vlnvToStr minfo.name

                    // let keepf = not_nonep(at_assoc "preserveinstance" minfo.atts) || ii.preserve_instance                   // TODO - in future, just  take these propertiess from ii not from the ats            
                    // let keepasinstance = not topflag && (ii.preserve_instance || (* or old way *) keepf)
                    let instancef = not ii.definitionf
                    if instancef then
                        hpr_yikes (msg + sprintf "vm_separator: Instance of vm outside of any definiton is ignored. iname=" + vlnvToStr ii.vlnv)
                        split worklist cc

                    else
                        let (ans, worklist) = cutout mch worklist
                        let cc = if nonep ans then cc else (valOf ans)::cc
                        split worklist cc
                        
    and cutout mch worklist =

        let rec cutout_lst lst worklist sofar =
            if nullp lst then (rev sofar, worklist)
            else
                let hh = hd lst
                if partof_pred hh then
                    let (h1, worklist) = cutout hh worklist
                    cutout_lst (tl lst) (worklist) (if nonep h1 then sofar else (valOf h1)::sofar)
                else cutout_lst (tl lst) (hh::worklist) (sofar) 

        match mch with
            | (ii, None) ->
                // We do not rez via cvipgen and then render. We assume System Integrator and/or other mechanisms (such as vendor tools loading from cvgates.v) will serve for that.
                // If the mcho field is null, we have neither a worthwhile definition or instance. We have a declaration at best, which we here ignore.
                    (None, worklist)
                    
            | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
                let m1 = vlnvToStr minfo.name
                let (sons', worklist) = cutout_lst sons worklist []
                let n = (ii, Some(HPR_VM2(minfo, decls, sons', execs, assertions)))
                (Some n, worklist)

    split lst []

// See also abstracte.mine_decl_aliases. They do (exactly?) the same thing.
let formaltag_handler with_iname assoctag =
    let ans =
        match assoctag with
            | None -> None
            | Some (X_bnet ff) ->
                let f2 = lookup_net2 ff.n
                if at_assoc (fst g_no_decl_alias) f2.ats = Some "true" then Some ff.id
                else
                match at_assoc g_logical_name f2.ats with
                    | Some ss ->  // The pi_prefix is non blank when there are multiple instances of a port on a component. The logical_name needs thereby combined with the port_instance_name.
                        let (pi_prefix, pi_suffix) =  // 
                            match at_assoc g_port_instance_name f2.ats with
                                | Some pp when isDigit pp.[0] -> ("", pp)
                                | Some pp                     -> (pp + "_", "")
                                | None                        -> ("", "")
                        //dev_println (sprintf "formaltag_handler  ONE ss=%s  prefix=%s  suffix=%s" ss pi_prefix pi_suffix)
                        let (pi_prefix, pi_suffix) =  // 
                            match at_assoc g_fu_instance_name f2.ats with
                                | Some ikey when with_iname   -> (ikey + "_" + pi_prefix, pi_suffix)
                                | _                           -> (pi_prefix, pi_suffix)
                        //dev_println (sprintf "formaltag_handler TWO  ss=%s  prefix=%s  suffix=%s" ss pi_prefix pi_suffix)
                        Some (pi_prefix + ss + pi_suffix)
                    | _       -> Some ff.id
                                
            | Some (X_net(ss, _)) -> Some ss
    //if not_nonep assoctag then dev_println (sprintf "formaltag handler return %s from %s" (valOf_or_ns ans) (netToStr (valOf assoctag)))
    ans


// Render the alias name for a formal contact instead of its meo name.
let aliasfun san aliases (ff:net_att_t) =
    match op_assoc ff.id aliases with
        | None                            ->  san ff.id
        | Some(orig, fu_name_o, pi_name_o, lname) ->
            let (pi_prefix, pi_suffix) =  // 
                match pi_name_o with
                    | Some (pp:string) when isDigit pp.[0] -> ("", pp)
                    | Some pp                              -> (pp + "_", "")
                    | None                                 -> ("", "")
            let (pi_prefix, pi_suffix) =  // 
                 match fu_name_o with
                     | Some ikey                   -> (ikey + "_" + pi_prefix, pi_suffix)
                     | None                        -> (pi_prefix, pi_suffix)
            
            (pi_prefix + lname + pi_suffix)



// Set up aliases so that we render in RTL using the logical names for our formals where they exist
// The meo system allows a net with a given name to only have one set of attributes, so sub-components need fresh hames if instances are manifested with different parameter overrides, and many lnames, such as rdata, occur on different components where the precision may be different.  When rendering RTL and SystemC we convert to the lname form, discarding any discriminating prefix such as CVRAM10_.
// The rules are that a component instance ...            
let mine_decl_aliases ww with_iname decls =
    let rec miner cc = function
        | DB_group(_, lst) -> List.fold miner cc lst
        | DB_leaf(Some(X_bnet ff), _)         
        | DB_leaf(None, Some(X_bnet ff)) when hexpt_is_io (X_bnet ff) ->
            let f2 = lookup_net2 ff.n
            if at_assoc (fst g_no_decl_alias) f2.ats = Some "true" then cc
            else
                let fu_name = at_assoc g_fu_instance_name f2.ats 
                let pi_name = if with_iname then at_assoc g_port_instance_name f2.ats else None
                let lname = at_assoc g_logical_name f2.ats            
                if nonep lname then cc else (ff.id, (X_bnet ff, fu_name, pi_name, valOf lname))::cc
        | _ -> cc
    let lst = List.fold miner [] decls

    app (fun (id, (x, fu_name, pi_name, lname)) -> vprintln 3 (sprintf " using formal alias mapping  %s ->  %s %s %s"  id (valOf_or fu_name "fu=None  ") (valOf_or pi_name "pi=None  ") lname)) lst
    lst

let collate_defs_from_vm2_tree ww vms =
    
    let rec scan_serf topf cc = function
        | (iinfo, None) -> cc
        | (iinfo, Some(HPR_VM2(minfo, decls, children, execs, assertions))) when iinfo.definitionf ->
            let cc = (hptos minfo.name.kind, (iinfo, HPR_VM2(minfo, decls, children, execs, assertions)))::cc
            List.fold (scan_serf false) cc children
        | (iinfo, Some(HPR_VM2(minfo, decls, children, execs, assertions))) ->
            List.fold (scan_serf false) cc children
    let ans = List.fold (scan_serf true) [] vms
    vprintln 2 (sprintf "Found %i VMs definitions in input list." (length ans))
    ans


// Get the PLI fence ordering constraint from some XRTL. This is used to keep side effects in order.
let get_pli_fence msg arg =
    let addf cc = function
        | Some fence_order -> singly_add fence_order cc // for now
        | None -> cc
    let serf cc = function
        | XRTL(pp, g0, lst) ->
            let rtlopToStr cc = function
                | Rnop ss            -> cc
                | Rarc(gg, lhs, rhs) -> cc
                | Rpli(gg, (fgis, fence_order), args) -> addf cc fence_order
            List.fold rtlopToStr cc lst

        | XRTL_nop(ss)            -> cc
        | XIRTL_buf(gg, lhs, rhs) -> cc
        | XIRTL_pli(pp, gg, (fgis, fence_order), args) -> addf cc fence_order
        //| other -> sf (sprintf "get_pli_fence other form %A" other)
    let ans = serf [] arg
    //dev_println(sprintf "Fence ordering is %s" (sfold orderToStr ans))
    ans


// Lookup a clock domain by name
let director_assoc ww msg directors name =

    let pos() =
        let xf dir = sfold (edgeToStr) dir.clocks
        sfold xf directors
        
    let rec scan = function
        | [] ->
            sf(msg + sprintf ": There is no clock domain called '%s'.   Possibilities are %s" name (pos()))
        | dir::tt->
            if length dir.clocks = 1 && xToStr(de_edge(hd dir.clocks)) = name then dir
            else scan tt
    scan directors

(* eof *)
