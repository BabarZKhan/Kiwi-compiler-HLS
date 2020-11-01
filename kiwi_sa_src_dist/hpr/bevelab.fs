module bevelab

let bevelab_banner = " $Id: bevelab.fs $ "

//
// bevelab.sml - Hardware compilation and fsm/automaton generation.
//
//
// HPR L/S core component.  Converts an imperative sequence, with or without existing pause markup, into an XRTL finite state machine.
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
open reporter
open protocols
open revast

let g_autob_loglev = ref 3 

let g_fsmvd = ref 3 // Verbosity:  Set this to 3 for normal quiet operation.


// The delete from env minus combine - disabled for now
let costvec_minus_combine (v:costvec_t) q = v // muddy "{  total=v.total - q.total; }"


let costvec_replace (oldv_: costvec_t) newv = newv //  { v with total= q; }

//let g_recomb_limit = ref 0

// A note on : Clock enables, Abend Register, Debug Resource and other Hardware Control/Debug mechanisms.
//
// This module introduces requires directorate when converting from DIC to XRTL. But the directorate is infact already present within the H2BLK that contains the DIC. 
// Whether the directorate should be observed (especially w.r.t. clock enables) by the interpreter of the H2BLK or the code that is inside it is an important decision.
// Either way, export routines such as cpp_gen and verilog_gen cannot export a DIC and must embed its functionality as they render their output, although this we be a null procedure if the contents of the H2BLK already observed the DIC.
//
// Following the pattern of the clock and reset nets, it is the projectors that need to process clock enables. They should also provide the abend register boilerplate that intercepts
// certain hpr_calls that write to them and halt the system or thread.  Finally, they need to manage the handshake wires that were interpreted by the inside user logic under the 
// HSIMPLE system.
//


// Callgraph
// opath_bevelab:
// |->opath_bevelab_process_one_vm:
//    |->opath_bevelab_w
//       |->bev_compile_exec
//          |->bev_compile_exec_1
//             |->bev_compile_exec_2
//                 |->sp_catenate
//                 |->bev_compile_exec_2_end
//             |->wrap

(* ---------------------------------------------------------*)
(*
 * HPR L/S State Machine Compiler/Generator
 * 
 *)

type standing_pause_mode_t =
    | SPM_auto
    | SPM_hard
    | SPM_soft
    | SPM_maximal
    | SPM_bblock    
    


type bevelab_budgeter_t = 
      {
        big_bangp:            bool // Holds if a rezzing, start of universe constructor.
        default_pause_mode:   standing_pause_mode_t
        retnet:               hexp_t option   // Return value from Xreturn when non void. Was the special RETVAL I/O type.
        ubudget:              int 
        morenets:             hexp_t list ref option
        cost_queries:         anal_queries_t
        revast_enable:        bool
        systolic4:            string
        redirect:             bool            // Redirect goto statements to share Pause calls (and other shareable code)
        soft_pause_threshold: int64
      }


type bevelab_settings_t =
    {
        recipe_recode_pc:     bool
        recipe_onehot_pc:     bool
//      bisimulate:           bool        // Share states that are observably equivalent
        msg:                  string
//      scheduler:            string
        recipe_ubudget:       int
        kK:                   bevelab_budgeter_t
        render_cfg_dotreport: bool // string option
        vd:                   int
    }


// Program counter is a pair:      
let pc2s (pc, phase) = i2s pc + "." + i2s phase



let rec is_tlm_call arg = // We just need to check for shallow calls owing to the UNR pre-processing.  We do run the blockreftran/UNR here code in the bevelab do we?
    //dev_println (sprintf "is_tlm_call: scan in %s" (xToStr arg))
    match arg with
        | W_apply((f, gis), cf, args, _) ->
            //dev_println (sprintf "Check TLM search on %s" (sfold xToStr args))
            if not_nonep cf.tlm_call then Some arg
            else None

        | W_node(old_cast, V_cast old_cvt_power, [arg1], _) -> is_tlm_call arg1

        | other ->
            //dev_println (sprintf "Stop TLM search at %s" (xToStr other))
            None


// DO NOT USE: use statename1 please.
let statename npc npcs p =
    if false then xi_num p
    //elif false then xi_stringx (XS_withval(xi_num p)) (i2s p + ":" + npcs)
    else enum_add "statename" npc npcs p



// Add PLI invokation to the elaboration environment.
// PLI is side-effecting and order needs to be maintained.    
// Abbreviation: "sef" is "side-effecting function".
// WaW elides where the rhs (or lhs support) has EIS component requires evict_to_sef.
let pliadd ww (kK:bevelab_budgeter_t) (es, ea, mm, sef, cost:costvec_t) nH = 
    //vprintln 0 ("pliadd (old length =" + i2s(length sef) + ") " + xrtlToStr H)
    let vdp = false
    let ii = { id="pliadd" }:rtl_ctrl_t // temporary
    let cv = bevelab_sp_logic_cost ww vdp kK.cost_queries (SP_rtl(ii, [snd nH]))
    (es, ea, mm, nH::sef, cv)  // NB sef is built up backwards

(*
 * Lookup value in env. For scalars, subs=None (except bit inserts).
 *)
let varval_a m v subs e =
    let xToStro x = if x=None then "None" else xToStr (valOf x)
    let rec scan = function
        | [] -> None
        | (an, l, subs', g, r, true, cst)::t -> scan t
        | (an, l, subs', g, r, false, cst)::t when v=l && subsmatch_o m (subs, subs') ->
            //let _ = vprintln 0 (" varval cf " + xToStr v + " cf " + xToStr l + " " + boolToStr(v=l))
            Some(an, l, subs', g, r, false, cst)
        | _::t -> scan t
    scan e


let varval m v subs elst = 
    let ov = varval_a m v subs elst
    let  rval(an, l, subs, g, r, false, cst) = r 
    let rvalg(an, l, subs, g, r, false, cst) = g 
    if ov = None then (X_false, None) else (rvalg(valOf ov), Some(rval(valOf ov)))


// Same as varval but returns annotation as well
let varval2 m v subs elst = 
    let ov = varval_a m v subs elst
    let   rval(an, l, subs, g, r, false, cst) = r
    let rvalan(an, l, subs, g, r, false, cst) = an
    let rvalg(an, l, subs, g, r, false, cst) = g 
    if ov = None then (X_false, None, None) else (rvalg(valOf ov), Some (rval(valOf ov)), Some(rvalan(valOf ov)))



(*
 * Scalar vardel : must cope with bit inserts that overlap or not. 
 * Array behaviour is very similar: the difference is that the array will not see a block assignment
 * to all bits: the subscripts will always be present. So can use the same code for both cases. 
 *
 * A delete of an EIS expression needs it to be transferred to sef or made into a TLM call using evict_to_sef.
 *)
let rec vardel_s mf v subsc (m_evicted, m_deleted_cost) lst =
    let vd = !g_fsmvd

    let rec serf = function
        | [] -> []

        // All sites:  inserted is not subscripted, delete all previous entries.
        | ((an_, id, _, g, rhs, vf, cost) as hh)::tt when nonep subsc && id=v->
            if eis_rhs "vardel_s" rhs then // TODO also on arrays please
                mutadd m_evicted (g, rhs)
            else mutadd m_deleted_cost cost // Its cost is not recovered if preserved as sef/TLM.
            serf tt

        // One site inserted: delete that entry and fail if need to merge ...
        | ((_, id, Some vs', g, rhs, vf, cost)as hh)::tt when not_nonep subsc ->
            if id=v && subsmatch (fun() -> "vardel_s " + mf()) (valOf subsc, vs') then serf tt
            else hh :: serf tt

        | hh::tt -> hh:: serf tt

    serf lst



(*
 * vardel_a the datastructure is a list of lists: each array has its own list of updates.
 * Select item and then user scalar function.
 *)
let vardel_a m id subs m_deleted_cost ea =
     let rec filter = function
         | ([], c) -> (rev c, [])
         | ((v, updates)::t, c) -> 
            if id=v then ((rev c)@t, vardel_s m id subs m_deleted_cost updates)
             else filter(t, (v, updates)::c) 

     let (other_arrays, other_updates) = filter(ea, [])
     (other_arrays, other_updates)


(* 
 * Delete entry for array id at subs s.  Can be undecidable.
 *)
let vardel_a_at mf arr s ea =
    let rec filter cc = function
        | [] -> (rev cc, [])
        | (v, updates)::tt -> 
              if arr=v then ((rev cc)@tt, updates)
              else filter ((v, updates)::cc) tt

    let (other_arrays, updates) = filter [] ea  // Partition out assigns to other arrays from the ea env.
    let it = ref None
    (* vprint(1, i2s(length updates) + " concourse updates\n") *)
    let del ei c =
        match ei with
        | (an, _, Some a, g, rhs, vf, cst) ->
             let d=subsm1 (a, s) // "vardel_a" (a, s) // x_udeq(a, s)
             let msg () = ("bevelab: name alias hazard: subsm1 cannot compare array subscripts (fatal in hard pause mode)\n" + mf() + "\n     a=" + xToStr s + "\n cf  b=" + xToStr a + "\n gives->" + (if d=None then "NONE" else boolToStr(valOf d)) + "\n")
             if nonep d then
                 let mm = msg()
                 let mm1 = ("vardel: undecidable and functional array not generated yet for subscripting " + xToStr arr + " with " + mm) // We expect this to be trapped and retried with another pause inserted. Not in hard pause mode.
                 vprintln 1 mm1
                 raise (Name_alias mm1)
                elif d=Some true then (it := Some(g, rhs); c)
                else ei::c
        | (an, l, s, g, r, _, cst) -> sf "array has 'None' as a subscript"

    let other_updates = List.foldBack del updates []
    //vprint(1, i2s(length other_updates) + " resultant updates\n")
    (!it, other_arrays, other_updates)




// Scalar env entry:
// Also used for individual locations of an array.
type es_entry_t =
    { anno:       annotate_t option
      lhs:        hexp_t
      index:      hexp_t option
      guard:      hbexp_t
      rhs:        hexp_t
      volatilef:  bool
      cost:       costvec_t
    } // TODO - use me
    

// tuple form of the above:
type es_t = (annotate_t option * hexp_t * hexp_t option * hbexp_t * hexp_t * bool * costvec_t) list


let esToStr = function
    | (an, l, None, g, r, vf, cost) -> (annToStr an) + "g=(" + xbToStr g + ") " + (xToStr l) +  " := " + (xToStr r)
    | (an, l, subs, g, r, vf, cost) -> (annToStr an) + "g=(" + xbToStr g + ") " + (xToStr l) +  "[///] := " + (xToStr r)


type tid_t =
     {
         tidid:     string list
         pausemode: standing_pause_mode_t
     }


type env_t = 
     es_t *                    // Scalars
     (hexp_t * es_t) list *    // Vectors
     meo_rw_t *                            // Scalars and Vectors in mapping form.
     (vsfg_order_t option * xrtl_t) list * // sef: side-effecting function calls: tagged native/pli calls.
     costvec_t



// Another, unused, copy of env_rpt - lazy one. - checks pred
// cf activation_print
let lenv_rpt__ vd msg (es, ea, _, sef, costvec) =
    if not (lprint_pred vd) then ()
    else
        if vd>=4 then vprintln 4 ("levnrpt: " + msg)
        let er1 es = if vd>=4 then vprintln 4 (esToStr es)
        let env_rpt l = (vprint vd (msg + " "); app er1 l; if vd>=4 then vprintln 4 "\n")
        env_rpt es
        app (fun (a,b) -> env_rpt b) (ea)
        app (fun (tag, pli) -> if vd>=4 then vprintln 4 ("sef item: " + xrtlToStr pli)) (sef)
        //if vd>=4 then vprintln 4 ("levnrpt: " + msg + " done")
        ()



 // hpr_testandset, hpr_interlocked_add and hpr_exchange require pass-by-ref on their first argument since it updated. These methods are side-effecting.

 // The most general way to treat these is to map every shared scalar to a shared_scalar_fu.  Ditto for arrays.
 // The cunning aspect is that the shared_scalar_fu either contains the scalar or else is just a tree node in a larger tree that routes updates to the actual storage location.      

 // For Kiwi use, each call to this has been uniquified by UNR. (UNR=uniquify non-referentially transparent in Kiwi Front End.)
 // These functions make non-deterministic updates to variables shared between threads.
        // There are three basic scenarios:
        //  1. All the threads that act on the shared variable are compiled in one bevelab run.
        //  2. All the threads that act on the shared variable are ultimately implemented in one RTL file (compiled with one HPR/KiwiC run).
        //  3. Different RTL files need to act on the shared variable.
        

//  It is the responsiblity of the scheduler to put the resulting operations into some order (e.g. by 'evaluating' the XRTL in
 // some order.  But this is true of all composed RTL blocks and shared variables: we adopt some serialisation?

 // We carry the updates from a single thread in the env during bevelab. We assume 0 if no value in env we do not read the h/w reg?  
 // Want to carry these values from one thread to another in reality ?

 // hpr_test_and_set(mutex, newvalue): returns the old value of the mutex.

 // We might think that the begin/end critical region markers are a lower
 // level way of doing this: indeed they would be...

  // Notes: every elaborated block in hard pause mode _is_ implicitly handled as an atomic/critical region, hence there is no
  // special code needed and atomics such as hpr_interlocked_add and hpr_test_and_set does not need to be built in as long as it
  // has had the UNR treatment... ?  But this argument neglects the time dilation of a basic block in restructure so is wrong!


let g_use_pending_read = false // This needs to be enabled with the corresponding support in compose enabled as well.
 
let hpr_testandset gg (mutex, subs, vale, ov)  = 
    //let (og_, ov) = varval (fun ()->"mutex") mutex subs e
    vprintln 3 (sprintf "hpr_testandset on mutex %s  guard=%s  " (xToStr mutex) (xbToStr gg)  + (if nonep ov then " ov=None" else " ov= Some " + xToStr(valOf ov)))
    // We wish to get blocking/combinational semantics between threads for mutex chains generating a static-priority arbiter in order of which is elaborated first.
    // The hpr_pending_read primitive is like the X_x primitive, in that it can read the current pending update to a register, but it defaults to the value of the register instead of failing if no update is pending.
    // In RTL terms  "aa<=e1; bb<=aa; cc<=pending_read(aa)" will result in bb taking on the old value of aa as normal but cc getting e1.
    // The use of pending_read enables values to be conveyed combinationally from one thread to another.
    // But this does not work for separate compilations -  a dedicated FU then needs instantiating. TODO.
    let gov =
        match ov with
            | None ->
                if g_use_pending_read then
                    xi_apply(g_hpr_pending_read_fgis, [mutex])
                else
                    xi_blift X_false
            | Some ov -> ov

    let vf = false // volatile flag: See note above re at_assoc.
    let nenv = (None, mutex, subs, gg, vale, vf, CV_single(int64 g_default_op_area_cost))
    if vale <> xi_one  && vale <> xi_zero then hpr_yikes ("+++hpr_testandset: bad arg value, expect 1 or 0. Not " + xToStr vale)
    (gov, nenv)



// See which arg positions of a function call modify their arguments.
let has_outargs site (fname, gis) =
    let argpos = []
    dev_println (sprintf "has_outargs. site=%s for %s outargs=%s returns %s"  site fname gis.outargs (sfold i2s argpos))
    argpos



(* em_gen: generate env mapping.
 * We use a quad: (es, ea, em, pli, cost) : scalars, arrays and a mapping digest of them and pli/eis list.  
 *
 * For arrays: we insert twice: once with subscript and once as a map.
 * em_update must behave the same way as this function, but it is incremental side-effecting and can only
 * be used if we know the old em is not shared.
 *)
let em_gen mf sef (es, ea) =
    //vprintln 0 ("em_gen " + i2s(length es) + " " + i2s(length ea))
    let esgen cc = function
        | (an, l, None,      g, r, vf, cst) -> ARGM(l, g, r, vf)::cc
        | (an, l, Some subs, g, r, vf, cst) -> 
            let v = safe_xi_asubsc "em_gen" (l, subs)
            ARGM(v, g, r, vf)::cc

    let epgen vf arg cc =
        match arg with
        | (an, l, None,      g, r, vf, cst)    -> sf (mf() + "epgen: missing subscript")
        | (an, l, Some subs, g, r, true, cst)  -> (vf:=true; cc)
        | (an, l, Some subs, g, r, false, cst) -> 
            ARG1(subs, g, r) :: cc

    let eagen cc (id, updates) = 
        let vf = ref false
        let pairmap = List.foldBack (epgen vf) updates []
        if !vf then cc
        else
            //vprintln 30 ("Making mapping for " + xToStr id + " with " + i2s(length updates) + " updates.")
            let eshortcut arg cc =
                match arg with
                | (_, l, None,      g, r, vf, cst) -> sf(mf() + "eagen: no subscript")
                | (_, l, Some subs, g, r, vf, cst) -> 
                    let v = safe_xi_asubsc "eagen" (l, subs)
                    ARGM(v, g, r, vf) :: cc 
            let efull cc (l, pairmaps) = ARL(l, pairmaps, false) :: cc
            let maps = List.foldBack eshortcut updates (efull cc (id, pairmap))
            maps
    let costsum c (an, l, _, g, r, vf, cst) =
        //vprintln 0 ("cost of assign " + xToStr l + " "  + costToStr cst)
        costvec_parallel_combine c cst 
    let costvec = List.fold costsum (CV_single 0L) es // Add up again
    let costvec = List.fold (fun cc (arr, items) -> List.fold costsum cc items) costvec ea
    let l = List.fold esgen (List.fold eagen [] ea) es
    //vprintln 5 (fun()->"em_gen: " + mf() + " makemap1: items=" + i2s(length l) + " sef=" + i2s(length sef))
    let em = makemap1 l
    //vprintln 0 (mf() + ": total costs of env "  + costToStr costvec)    
    (es, ea, em, sef, costvec):env_t


let xgen_query1 m  g t f =
    let ans = ix_query g t f
    //vprintln 0 (m + sprintf ":  xgen_query1 tracing %s of %s : %s is %s" (xbToStr g) (xToStr t) (xToStr f) (xToStr ans))
    ans



        
// Join a pair of envs predicated by g.
// Note each env entry also has its own qualifying g.
let env_mux kK gtop (est, eat, rw_, seft, cost_t) (esf, eaf, _, seff, cost_f) =
    let vd = -1 // Normally -1
    let gtop_b = xi_not gtop
    let m = fun()->"env_mux left={ est=" + i2s(length est) + "eat=" + i2s(length eat) + " st=" + i2s(length seft) + " } right = { esf=" + i2s(length esf) + "eaf=" + i2s(length eaf) + " sf=" + i2s(length seff) + " } g=" + xbToStr gtop
    let rec emux_scalars_finish = function
        | ([], dones) -> []
        | ((an, l, s, gf, rf, vf, cst)::t, dones) ->
                if memberp (l, s) dones then emux_scalars_finish(t, dones)
                else
                    let gg = ix_and gtop_b gf
                    let rr = rf // xgen_query1 "emf" gtop l rf
                    let _ =
                        if vd >=6 then
                            vprintln 6 ("em emf rf: " +  i2s(x2nn rf) + ":" + xToStr rf)
                            vprintln 6 ("   emf g:  " + xbToStr gtop)
                            vprintln 6 ("   emf gf: " + xbToStr gf)
                            vprintln 6 (sprintf "emf: Envmux_scalars_finish generated: gg=%s  rr=%s" (xbToStr gg) (xToStr rr)) //  + i2s(x2nn rans) + ":" + xToStr rans)
                    (an, l, s, gg, rr, vf, cst)::emux_scalars_finish(t, dones) // gg cost not in cst. TODO?

    let rec emux_scalars = function
        | ([], dones) -> emux_scalars_finish(esf, dones)
        | ((an, l, s, ng, nv, vf, (cst:costvec_t))::t, dones) ->
              let (og, ov) = varval m l s esf
              let _ =
                  if vd >= 6 then
                      //vprintln 6 ("em ov: " + (if nullp ov then "None" else (i2s(x2nn(valOf ov)) + ":" + xToStr(valOf ov))))
                      vprintln 6 ("em em nv: " +  i2s(x2nn nv) + ":" + xToStr nv)
                      vprintln 6 ("   em gtop:  " + xbToStr gtop)
                      vprintln 6 ("   em ng: " + xbToStr ng)
                      vprintln 6 ("   em og: " + xbToStr og)                      
              let (ransg, rans, cst') =
                  match ov with
                      | None ->
                          let gnn = ix_and gtop ng // This was just ng !
                          if vd>=5 then vprintln 5 (sprintf "em: Envmux none-f generated g=%s ans=%s:" (xbToStr gnn) (xToStr nv))
                          (gnn, nv, cst) 
                      | Some rvv ->
                          let rans = xgen_query1 ("emux_scalars") gtop nv rvv // This was ng instead of gtop - an ancient bug - amazingly never manifested?
                          let ransg = ix_bquery gtop ng og
                          if vd>=5 then vprintln 5 (sprintf "em: Envmux generated: ransg=%s rans=%s" (xbToStr ransg) (xToStr rans))
                          (ransg, rans, bevelab_xi_cost kK.cost_queries rans) // yes, includes all cost
              (an, l, s, ransg, rans, vf, cst')::emux_scalars(t, (l, s)::dones)

    let rec combine_array_updates = function
        | ([], []) -> []
        | ([], (an, A, s, gf, vf, volf, cst)::t) -> 
            let disable_aupdate vv =
                let gg = ix_and gtop_b gf 
                (an, A, s, gg, vv, volf, cst)
            (disable_aupdate vf)::combine_array_updates ([], t) 
   
        | ((an, A, s, gt, vt, volf, cst)::t, flist) -> 
            let ov = varval_a m A s flist
            let rval(an, l, subs, g, r, false, cst) = r 
            let rvalg(an, l, subs, g, r, false, cst) = g
            let rval_cst(an, l, subs, g, r, false, cst) = cst            
            let mux(t, f) = ix_or (ix_and gtop t) (ix_and gtop_b f)
            let (aa, gg, vv, cstcst) =
                if ov<>None
                then
                    let an' = an(*!!! not always correct*)
                    let g' = mux(gt, rvalg (valOf ov))
                    let v' = xgen_query1 "cag" gtop vt (rval(valOf ov))
                    let cst' = costvec_replace cst (bevelab_xi_cost kK.cost_queries v')
                    (an', g', v', cst')
                else (an, gt, vt, cst)
            let flist' = if ov=None then flist else remove_item(flist, valOf ov)
            // let vf = if ov=None then xgen_asubsc(A, valOf_or_fail "cau" s) else rval(valOf ov)
            let n = (aa, A, s, gg, vv, volf, cstcst)
            n::combine_array_updates (t, flist')


    let rec emux_array_finish dones = function
        | ([]) -> []
        | ((A, updates)::t) -> 
            let disable_aupdate (an, A, s, gf, vf, volf, cst) = (an, A, s, ix_and gtop_b gf, vf, volf, (cst:costvec_t))
            if memberp A dones then emux_array_finish dones t
            else (A, map disable_aupdate updates)::emux_array_finish dones t

    let rec emuxa = function
        | ([], dones) -> emux_array_finish dones eaf
        | ((A, updates)::t, dones) -> 
              let fv = op_assoc A eaf
              let enable_aupdate(an, A, s, gt, vt, volf, cst) =
                  let gg = ix_and gtop gt 
                  let vv = vt
                  (an, A, s, gg, vv, volf, cst)
              let r = if fv=None then map enable_aupdate updates else combine_array_updates (updates, valOf fv)
              (A, r)::emuxa(t, A::dones)

    let es = emux_scalars (est, [])
    let ea = emuxa (eat, [])

    let rec sefmerge a = // sef=side-effecting functions
        // PLI calls can be combined under disjunction of guards when their ordering and args etc are the same.
        // Private copies with extra tags... delete them soon
        // TODO : cost of sef? - mostly deleted for logc synthesis so can be neglected.
        let sef_gate g arg cc =
            match arg with 
            | (tag, XIRTL_pli(a, g0, fgis, args)) -> (tag, XIRTL_pli(a, ix_and g g0, fgis, args)) :: cc
            | (tag, (XRTL(a, ga, lst) as ax)) -> 
               if true then (tag, (XRTL(a, ix_and g ga, lst))) :: cc
               else
               let sq ox cc =
                   match ox with
                   | Rpli(gb, fgis, args) -> (tag, XIRTL_pli(a, xi_and(g, xi_and(ga, gb)), fgis, args)) :: cc // This converts form
                   | other -> sf("sef sef_gate op other + " + xrtlToStr ax)
               List.foldBack sq lst cc
            | (tag, other) -> sf("sef sef_gate g other + " + xrtlToStr other) 

        // Matchup checks for sef equality but is happy if their guards differ since these will be disjuncted anyway.
        let sef_matchup = function
            | ((tag, XIRTL_pli(a, g, fgis, args)), (tag', XIRTL_pli(a', g', fgis', args'))) -> tag=tag' && fgis=fgis' && args=args'

            | ((tag, XRTL(a1, g1, l1)), (tag', XRTL(a2, g2, l2))) ->
                let rec matching = function
                   | ([], []) -> true
                   | (Rpli(g00, (fgis, order_tag), args)::tt1, Rpli(g00', (fgis', order_tag'), args')::tt2) -> fgis=fgis' && order_tag=order_tag' && args=args' && matching (tt1, tt2)
                   | (tag, other) ->
                        let _ = dev_println (sprintf "sef_matchup XRTL other form treated as different %s" (xrtlToStr (XRTL(a1, g1, l1))))
                        false
                           
                tag=tag' && a1=a2 && matching (l1, l2)
               
            | ((tag, other), (tag', rhs)) ->
                // other forms missing? - being lazy?:  Rarc and Nop do are not relevant here since not entered in env anyway.
                let _ = dev_println (sprintf "sef_matchup other pairwise form treated as different %s" (xrtlToStr other))
                false

        let sef_combine = function // form a disjunction of guards
            | ((tag, XIRTL_pli(a, g, fgis, args)), (tag', XIRTL_pli(a', g', fgis', args'))) when tag=tag' && fgis=fgis' && args=args' ->
                        if a=a' then (tag, XIRTL_pli(a, ix_or g g', fgis, args))
                        else muddy "// sef_combine bevelab from different activations"

            | ((tag, XRTL(a1, g1, l1)), (tag', XRTL(a2, g2, l2))) when tag=tag' && a1=a2 ->
                let rec disjunction = function
                    | ([],[]) -> []
                    | (Rpli(g00, (fgis, order_tag), args)::tt1, Rpli(g00', (fgis', order_tag'), args')::tt2) when fgis=fgis' && order_tag=order_tag' && args=args' ->
                        let nv = Rpli(ix_or g00 g00', (fgis, order_tag), args)
                        nv::disjunction (tt1, tt2)

                    // other forms missing? - being lazy?
                    | (_, _) ->
                         sf (sprintf "sef_combine XRTL other forms")

                           
                (tag, XRTL(a1, g1, disjunction (l1, l2)))

            | ((tl, oll), (tr, orr)) -> sf(sprintf "sef_combine other  tl=%A tr=%A\n ol=%A\n or=%A" tl tr oll orr)

        match a with
        | ([], []) -> []
        | ([], ff) -> List.foldBack (sef_gate gtop_b) ff []
        | (tt, []) -> List.foldBack (sef_gate gtop)  tt []
        | (tt::ts, ff::fs) when sef_matchup(tt, ff) -> sef_combine(tt, ff) :: sefmerge(ts, fs)
        | (tt::ts, ff) -> tt :: sefmerge(ts, ff)        

    let sef = sefmerge(seft, seff)
    if vd >= 5 then vprintln 5 (sprintf "Env_mux result: es=%i ea=%i sf=%i" (length es) (length ea) (length sef))
    let ans = em_gen m sef (es, ea)  // emgen will recompute all costs.
    ans:env_t


let env_print vd msg (ev, ea, _, sef, cv) =
    if vd >= 0 then
        let scalar_env_print vd x = vprintln vd ("  " +  esToStr x)
        let  array_env_print vd x = vprintln vd ("   "+  esToStr x)
        app (scalar_env_print vd) (ev)
        app (array_env_print vd) (list_flatten (map snd (ea)))
        app (fun (tag, pli) -> if vd>=4 then vprintln 4 ("    sef item: " + xrtlToStr pli)) sef
        //dev_println (sprintf "bevelab: env_print active %i" vd)

        
// cf env_rpt
let activation_print vd msg (pos, tid, pp, g, env, an) = 
   if vd >=0 then
       if vd>=4 then vprintln 4 (msg + "  activation_print start")
       if vd>=4 then vprintln 4 (msg + " " + xToStr(snd pp) + "->" + pc2s pos + ":" + annToStr an + ": g=" + xbToStr g) 
       env_print vd msg env
       if vd>=4 then vprintln 4 "  activation_print end"
       //dev_println ("bevelab: activation_print active")



//
//  Find a number of activations that share the same pp and pos and combine them.
//  What happens about g being different .... TODO explain why ok ... differences in g will reflect the control flow guards that led to the recombination site and a disjunction of them should make them cancel or else we should use the g of their immediate dominator.
//
let recomb_fun sS1 lst0 =
    let kK = sS1.kK
    let vd = sS1.vd
    let msg = "recomb_fun"
    let nulenv = em_gen (fun _ -> "null start") [] ([], [])
    let recomb1 lst = 
        if nullp lst then sf "nothing to recombine"
        else
        let ((e_pos, e_tid, e_pp, _, _, e_an), stack) = hd lst
        let m_selected = ref []
        let rec groom lst = // maintain order of remainder list.
            match lst with
            | [] -> []
            | ((pos, tid, pp:pcp_t, g, e:env_t, an), stack')::tt when pos=e_pos && pp=e_pp ->
                let _ = mutadd m_selected (hd lst)
                groom tt
            | other::tt -> other :: groom tt    
        let remainder = groom lst
        match !m_selected with
            | [] -> sf "L582"
            | [item] ->
                let _ = if vd>=4 then vprintln 4 (msg + ": One item recomb list " + pc2s e_pos)
                (item, remainder)
            | items ->
                if vd>=3 then vprintln 3 (msg + sprintf ": recombining %i activations for site pos=%s pp=%s" (length items) (pc2s e_pos) (xToStr (snd e_pp)))

                let sq (g0, e0) ((pos, tid, pp:pcp_t, g, e:env_t, an), stack') =
                    cassert(pos = e_pos, "pos = e_pos")
                    cassert(tid = e_tid, "tid = e_tid")
                    cassert(pp = e_pp, "pp = e_pp")
                    cassert(an = e_an, "an = e_an")        
                    let _ =
                        if vd >= 6 then
                            vprintln 6 (sprintf " g-new=%s         g-0=%s"  (xbToStr g)  (xbToStr g0))
                            vprintln 6 (sprintf "---- e-new")
                            env_print 6 msg e
                            vprintln 6 (sprintf "---- e-0")
                            env_print 6 msg e0
                            vprintln 6 (sprintf "---- ")
                    (ix_or g g0, env_mux kK g e e0) // Don't want full meo make on each fold - a bit costly: TODO
                let (gf, ef) = List.fold sq (X_false, nulenv) items
                let nx = (e_pos, e_tid, e_pp, gf, ef, e_an)
                if vd>=5 then activation_print 5 msg nx
                let newstack = [] // discard old stack (perhaps keep common part of it and insert an ellipsis?)
                ((nx, newstack), remainder)
    let rec iterate cc items =
        if vd>=4 then vprintln 4 (sprintf "recomb iterate: %i items, %i results" (length items) (length cc))
        match items with
            | [] -> cc
            | items ->
                let (ans, remainder) = recomb1 items
                iterate (ans :: cc) remainder
      
    if (true) then
        let xs = iterate [] lst0
        (xs, [])
    else
        let (x, remainder) = recomb1 lst0 
        ([x], remainder)

// Note: side effects here dropped.
// The problem with using rewrite_exp for the semantic evaluator of functions is that side effects are not captured.
// Using the walker is a better idea for the future. TODO.        
let rewrite_exp1 (es, ea, em, sef, _) x  = xi_rewrite (em) x
let rewrite_bexp1 (es, ea, em, sef, _) x = xi_brewrite em x


// This is old code - soon to be replaced with an FU call as per hpr_atomic_add.
let bevelab_testandset ww mm e mutex vale =
    let m_deleted_cost = ref [] // TODO why local instance of this?  Needs passing on?
// TODO add hpr_pending_read
    let rec mutex_scan grg ee mutex =
        match mutex with
            | W_query(g0, tt, ff, _) -> // We certainly need non-strict semantics for the ternary operator here.
                let g0 = rewrite_bexp1 ee g0 // TODO? side effects here dropped
                let gt = ix_and grg g0
                let gf = ix_and grg (xi_not g0)
                let (rt, ee) = mutex_scan gt ee tt
                let (rf, ee) = mutex_scan gf ee ff                    
                let r999 = ix_query g0 rt rf
                (r999, ee)
                                
            | X_bnet ff -> // Scalar mutex
                let subs = None
                let (og_, old_value_o) = varval (fun ()->"mutex") mutex subs (f1o5 ee)  
                let (r', newv) = hpr_testandset grg (mutex, subs, vale, old_value_o)
                let oenv = vardel_s (fun()->"hpr_testandset") mutex subs (ref [],  m_deleted_cost) (f1o5 ee)
                (r', em_gen mm (f4o5 ee) (newv::oenv, f2o5 ee))

            | W_asubsc(X_bnet ff, subs, _) -> // Array of mutexes
                let subs = rewrite_exp1 ee subs
                let mutex_array = X_bnet ff
                let (it, other_arrays, other_updates) = vardel_a_at mm mutex_array (subs) (f2o5 ee)
                let old_value_o =
                        match it with
                            | None -> None
                            | Some(gg, rhs) -> Some(ix_query gg rhs (ix_asubsc mutex_array subs))
                let (r', e1) = hpr_testandset grg (mutex_array, Some subs, vale, old_value_o)
                (r', em_gen mm (f4o5 ee) (f1o5 ee, (mutex_array, e1::other_updates)::other_arrays))

            | other -> 
                hpr_yikes (mm() + ": bevelab: test and set: tas other expression/form used as mutex not supported 2/2: " + xkey other + " " + xToStr other)
                (xi_zero, ee)
    mutex_scan X_true e mutex
        


(*
 * e2 elaborate: take an hbev imperative statement and make a symbolic calculation
 * of its effect on environment e.
 *
 * pc is a virtual machine program counter
 * g is an activation guard
 * an is an annotation.
 *
 * Return a list of new active symbolic environments, with closed results being
 * placed on the r list.
 * Return also a blk flag that is 1 if the command may 'block' in some sense.

 * 
 *)
let g_logdisable = false;
let logadd db closure = if g_logdisable then () else mutadd db closure;

let g_return_pc = -10

let activation_e (pos, tid, pp, g, e, an) = e



// We want to see effects of assignemnts to volatiles
// within an atomic sequence of statements but will need
// to drop volatile updates over pre-emption/yield points.

let vol(f:net_att_t) = false   // at_assoc "volatile" f.ats = Some "true" 
  // The volatile attribute really refers to the kiwi front end
  // and at this point, everything is 'volatile' at a pause or yield


let vol_net = function
    | X_bnet ff -> vol ff
    | _ -> sf "vol_net"


let i2so(b) = if b=None then "NONi" else i2s(valOf b)


//
// Compose in parallel a pair of scalar envs: hopefully disjoint:
// Also used for two sets of updates for the same array.
//
let env_par_scalar_or_subs m nsef (esl, esr) =
    let dones = ref []
    let vd = !g_fsmvd

    let geng (x:annotate_t option) =
        match x with
        | None   -> 0I // sf "geng: no annotation to resolve non-det join"
        | Some x -> atoi(valOf_or_fail "generation-attribute" (op_assoc "generation" (x.atts)))
    let rec k lst =
        match lst with
        | [] -> []  
        | (an, l, s, g, r, vf, cst)::tt ->     
            let (og, ov, oan) = varval2 (fun()-> m + "env_par_scalar") l s esr
            let m1() = (m + " : non-deterministic updating of net:" + xToStr l + (if s=None then "" else " s=" + xToStr(valOf s)) +
                             "\n  gl=" + xbToStr g + " ll=" + annToStr an + " " + i2s(x2nn r) + ":" +  xToStr r +
                             "\n  gr=" + xbToStr og + " rr=" +  annToStr(valOf oan) + " " + i2s(x2nn(valOf ov)) + ":" + xToStr(valOf ov)
                             + "\n")

            if ov=None || x_eq (valOf ov) r then (mutadd dones (l, s); (hd lst)::(k tt))

            elif true then                   
                     let gg = xi_or(g, og)
                     let nodet = xi_and(g, og)
                     let rr = xi_query(g, r, valOf ov) // makes a resolution here if non-det
                     let tag = None
                     let nH =(tag, XIRTL_pli(None, nodet, (hpr_native "hpr_writeln", tag), [xi_string(m1())]))
                     if nodet <> X_false then mutadd nsef nH
                     (an, l, s, gg, rr, vf, cst):: (k tt)
            else    
                   let gg = xi_or(g, og) // NOT USED 
                   let gen_l = geng an
                   let gen_r = geng (valOf oan)
                   if vd>=4 then vprintln 4 (m1())
                   let (r', an') =
                       if gen_l > gen_r then (r, an)
                       elif gen_r > gen_l then (valOf ov, valOf oan)
                       elif x_eq r (valOf ov) then
                           vprintln 3 ("\nThey assigned the same values")
                           (r, an)
                       else
                           // The patch approach rewrites the guard and not just the backstop val...
                          let patch:meo_rw_t = makemap [(l, r)]
                          let r' = xi_rewrite_exp patch (valOf ov)
                          vprintln 3 ("Equal rankings : random selection - hope disjoint - could generate a checker")
                          (r', an)
                   mutadd dones (l, s)  (* Hmmm is it really done! then  ? TODO *)
                   (an', l, s, gg, r', vf, cst):: (k tt)
              
    let n = k esl
    let rval (an, l, s, g, r, vf, cst) = not(memberp (l,s) (!dones))  (* TODO - is memberp really good enough?  should use subs_match *)
    n @ (List.filter rval esr)



(*
 * Compose in parallel a pair of 1-D array envs:
 *)
let env_par_array nsef (esl, esr) = 
    let adone = ref []
    let zz (A, updates) = 
        let _ = mutadd adone A
        let ov = op_assoc A esr
        if ov=None then (A, updates)
        else (A, env_par_scalar_or_subs "subs" nsef (updates, valOf ov))
      
    let n = map zz esl
    let rval (A, _) = not(memberp A (!adone))
    n @ (List.filter rval esr)



(*
 * Compose a pair of updates in parallel: hopefully deterministic (i.e. no clashes).
 *)
let env_par_comb m ((esl, eal, _, sefl, cost_l), (esr, ear, _, sefr, cost_r)) =  
    let nsef = ref []
    let es = env_par_scalar_or_subs m nsef (esl, esr)
    let ea = env_par_array nsef (eal, ear)
    em_gen (fun _ -> "par comp") (!nsef @ sefl @ sefr) (es, ea)  // very expensive in cost_recompute from scratch TODO



//
//
// Can get non det on parallel joins...
//
let rec env_parallel_combine m = function
    | [item] ->
        //let er1 x = if vd>=5 then vprintln 5 (esToStr x)
        //| (an, l, None,   g, r, vf, cst) -> lprint vd (fun()->"    fo " + (annToStr an) + (xToStr l) +  " := " + (xToStr r) + "\n")
        //| (an, l, Some s, g, r, vf, cst) -> lprint vd (fun()->"    fo " + (annToStr an) + (xToStr l) +  "[.o.] := " + (xToStr r) + "\n")
        //let env_rpt(es, ea, m, sef, cost) = (lprint vd (fun()->(" First off env_parallel_combine \n")); app er1 es; vprint vd ("\n\n"))
        //let _ = env_rpt item
        item:env_t

    | (a::b::tt) -> env_parallel_combine m (env_par_comb m (a, b)::tt)


// This enumeration needs to be nailed to string values owing to the use of strings in the Kiwi.cs attribute definitions.
let pmos_lexate = function
    | "auto" -> SPM_auto
    | "hard" -> SPM_hard
    | "soft" -> SPM_soft
    | "maximal" -> SPM_maximal
    | "bblock" -> SPM_bblock   
    | s -> cleanexit("pmos: pause mode invalid: " + s)

// Hard pauses mean what they say and are used to precisely count out clock cycles.
// A pair of adjacent soft pauses may be elided.
// An auto pause is inserted to avoid unresolvable name aliases.
// A soft pause may be a hint for where a pause could usefully be placed or is a pause inserted by logic. 

type pause_point_t =
  |   PS_hard
  |   PS_soft
  |   PS_auto



type pause_points_t = pause_point_t option array

let psToStr = function // can just use sprintf
    | PS_hard    -> "PS_hard"
    | PS_soft    -> "PS_soft"
    | PS_auto    -> "PS_auto"    

        
let is_hard1 vd pmode_str = // Convert standing pause mode to pause_point_t
    let style = match pmode_str with
                | SPM_auto     -> PS_soft
                | SPM_hard     -> PS_hard
                | SPM_soft     -> PS_soft
                | SPM_maximal  -> PS_hard
                | SPM_bblock   -> PS_soft
//                | SPM_postdom  -> PS_hard // For now                
//                | ss -> sf (sprintf "pmos is_hard1: pause mode invalid: %A" ss)
    if vd>=4 then vprintln 3 (sprintf "Pause arg mode=%A so using %A"  pmode_str style) // not much use without a line no
    style



let is_hard style = style= PS_hard

let pf_mine m k =
    if not (constantp k) then sf (m + " was set to non-compiletime-constant value " + xToStr k)
    else
        let v = xi_manifest m k
        if (v < 0 || v >= hpr_PauseFlags.NextToUse_) then  sf (m + " was set to unrecognised constant value " + xToStr k + " flagsvec=" + pausemodeFlagToStr v)
        muddy ("numeric pf_mine forms temporarily disabled " + xToStr k)


let iarg_mine m max k =
    if not (constantp k) then sf (m + " was set to non-compiletime-constant value " + xToStr k)
    else
        let v = xi_manifest m k
        if (v < 0 || v > max) then  sf (m + " was set to unrecognised constant value " + xToStr k)
        v

let genno = ref 100
let nextgen () =
    let r = !genno;
    genno := r + 1
    vprintln 3 ("Allocated generation number " + i2s r)
    r
   

// Handler for (experimental) dynamic setting current pausemode mid thread.
// These calls need to be retained in the user's program at least until restructure.
// The idea is that you can change it locally within various parts of a thread's control flow graph.
let pause_control_handlex kK (("hpr_pause_control", gis), vale) =
    //let kk = iarg_mine "pause_control mode" 4 vale
    let pausemode =
        match vale with
               | W_string(ss, _, _)::_ -> pmos_lexate ss
               | arg::_ -> pf_mine "hpr_pause/barrier argument" arg
               | [] -> cleanexit("missing pause/barrier argument")
    vprintln 3 (sprintf "hpr_pause_control: Dynamic set pause mode to %A" pausemode)
    pausemode

       
//
// Handler for hpr_pause and pausemode
//
let pause_handlex (K:bevelab_budgeter_t) current_style (fgis, args) = 
    let m = "pause_handlex"
    let style =
        match args with
            | [] -> Some current_style
            | W_string(ss, _, _)::_ -> Some (pmos_lexate ss)
            
            | arg::_ when constantp arg -> 
                let vv = xi_manifest m arg
                let failf() =  sf (m + " was set to unrecognised constant value " + xToStr arg + " flagsvec=" + pausemodeFlagToStr vv) 
                let _ = if vv < 0 || vv >= hpr_PauseFlags.NextToUse_ then failf()
                if vv &&& hpr_PauseFlags.HardPause > 0 then Some SPM_hard
                elif vv &&& hpr_PauseFlags.NoUnroll > 0 then  Some SPM_soft // for now: TODO
                elif vv &&& hpr_PauseFlags.SoftPause > 0 then Some SPM_soft 

                elif vv &&& hpr_PauseFlags.CurrentModePause > 0 then Some current_style // This should be current not standing? TODO
                elif vv &&& hpr_PauseFlags.EndOfElaborate > 0 then
                    None // ("EndOfElaborate checking is done in KiwiC or other front end.")
                else failf()

            | [] -> cleanexit("missing pause/barrier argument")

    style


//
// An expression with side effects is moved from value path to a pure side-effecting site.       
//
let evict_to_sef_or_tlm ww kK tid pos (g00, rhs) env =
    let rec e2s rhs = 
        match rhs with
            | W_node(prec, V_cast cvt_power, [vv], _) -> e2s vv
            | W_apply((fname, gis), cf, args, _) ->
                dev_println ("URGENT - evict_to_sef of " + xToStr rhs)
                let order_tag = Some("tid", tid.tidid, fst pos, next_tag())
                let arc =
                    if true then
                        gen_XRTL_arc(None, g00, X_undef, xi_apply_cf cf ((fname, gis), args)) // Assign to X_undef for an EIS side-effecting arc.  Order tags not used for arcs ...
                    else
                        XRTL(None, X_true, [Rpli(g00, ((fname, gis), order_tag), args)])
                pliadd ww kK env                 (order_tag, arc)

            | other ->
                hpr_yikes ("evict_to_sef other form :"  + xToStr other)
                env
    e2s rhs


                
(*
 * A second route into bevbev that is used without a pc on AST forms only (so no Xbeq  or Xgoto expected!)
 * hence Xblock and Xpblock are supported by bevbev (not here).
 *
 *)
let rec bevbev2a ww kK e gen x = // Also used for Xpblock forms
    let vd = !g_fsmvd
    let db = ref []
    let morenets = ref []
    let post_K =
        {
          kK with
            big_bangp=           false
            default_pause_mode=  SPM_soft
            ubudget=             kK.ubudget // Start off/reset budget to start value for post
            morenets=            Some morenets
          //reset=               xi_blift(reset()) (*Clock and reset domain ...  TODO get from atts ? NOT USED ANYWAY - see directorate_t now. *)
            retnet=              None
        } : bevelab_budgeter_t
    let rv = None
    let pos = (-1, 1)
    let tid:tid_t = { tidid=[]; pausemode= (kK.default_pause_mode); }
    vprintln 3 (sprintf "bevbev2a: starting pause mode for this thread is %A" tid.pausemode)
    let pp = (X_undef, X_undef)
    let g = X_true
    let n = (pos, tid, pp, g, e, Some { atts=[("generation", i2s gen)]; palias=None; comment=None; })
    let ww'  = WF vd "bevbev2a" ww "start bevbev2"
    let (blk, ans) = bevbev2 ww' (post_K, rv, db) n x
    let _ = WF vd "bevbev2a" ww "end bevbev2"
    cassert(length ans = 1, "forked in bevbev2a")
    activation_e(hd ans)


//
// Behavioural elaborator: bevbev2 handles one hbev command.
//   
and bevbev2 ww (kK, r, db) alpha cmd =
    let vd = !g_fsmvd
    let (pos, tid, pp:pcp_t, g00, e:env_t, an) = alpha
    if vd >= 5 then vprintln 5 ("bevbev2: base=" + xToStr (fst pp) + "=" + xToStr(snd pp) + " interpret pc="+ pc2s pos + " cmd=" + hbevToStr cmd)
    let successor(pc, phase) =
        let pc = if pc > g_metastate then pc-g_metastate else pc
        (pc+1, 0)

    //dev_println (sprintf " bevelb cmd=%A" cmd)


    let bevbev2_easc_call application =
        let easc_bstop = function // It becomes either PLI or a an ARC dependng on whether has an FU.
            | W_apply((fname, gis), cf, args, _) ->
                let args' = remove_concats(map(rewrite_exp1 e) args)
                if vd>=3 then vprintln 3 (sprintf "Default processing for Xcall %s.  is_fu=%A" (hbevToStr cmd) gis.is_fu + " args'=" + sfold xToStr args')
                //lenvrpt 0 "Xcall debug " e

                let yielding = gis.fsems.fs_yielding
                // Tag is a src code textual point. We later elide pli calls with identical tags. BUT IF THEY WERE ON DIFFERENT UNWINDS WE WANT TO KEEP THEM DISTINCT... If args are same we later elide, disregarding the final tag, but tie break on it when sorting.

                match gis.is_fu with
                    | Some fu ->
                        let order_tag = Some("tid", tid.tidid, fst pos, next_tag())
                        let arc = gen_XRTL_arc(None, g00, X_undef, xi_apply_cf cf ((fname, gis), args')) // Assign to X_undef for an EIS side-effecting arc.
                        let nH = (order_tag, arc)
                        let e = pliadd ww kK e nH
                        let barrierf = if yielding then Some PS_soft else None
                        if not_nonep barrierf then vprintln 3 ("bevelab: Treating xcall " + fname + " as a pause barrier at " + pc2s pos)
                        (barrierf, [(successor pos, tid, pp, g00, e, None)])

#if SPARE
                        let vf = vol f
                        if vd >= 5 then vprintln 5 ("Making update to " + xToStr lhsL + " new value is " + xToStr r' + " with annotation " + annToStr an)
                        let cst_new = costvec_sequential_add (bevelab_xi_cost kK.cost_queries r')  (0L,0L)//No cost for a const
                        let es' = ((an, lhsL, None, g, r', vf, cst_new)::(vardel_s mm lhsL None (m_evicted, m_deleted_cost) es), ea, em)
                        let cst_ded = List.fold costvec_minus_combine (costvec_parallel_combine costvec cst_new) !m_deleted_cost 
                    // em_update : used only for scalar assigns.
                        let em_update ((es, ea, em), k, v) = (es, ea, makemap1_add em (ARGM(k, g, r', vf)), sef, cst_ded)
                        em_update (es', lhsL, r')
#endif
                    | None ->
                        let order_tag = Some("tid", tid.tidid, fst pos, next_tag())
                        let nH = (order_tag, XRTL(None, X_true, [Rpli(g00, ((fname, gis), order_tag), args')]))
                        let e = pliadd ww kK e nH
                        let barrierf = if yielding then Some PS_soft else None
                        if not_nonep barrierf then vprintln 3 ("bevelab: Treating xcall " + fname + " as a pause barrier at " + pc2s pos)
                        (barrierf, [(successor pos, tid, pp, g00, e, None)])

        //dev_println (sprintf "check legacy application %s" (xToStr application))
        match application with
        //  | W_apply(("hpr_KppMark", gis), cf, args, _) ->     // This is not encountered since it is EIS and converted to an assignment not an Xeasc
            | W_apply(("hpr_postdom", gis), cf, args, _) ->
                //sf "postdom bev"
                let blk = None // pause_handlex (K) (("hpr_pause", gis), args') // Detected in find_pause_points
                let r1 = (successor pos, tid, pp, g00, e, None)
                (blk, [r1])

            | W_apply(("hpr_pause", gis), cf, args, _) ->
                let args' = map (rewrite_exp1 e) args
                let blk = None // pause_handlex (K) (("hpr_pause", gis), args')  // This is now pre-detected in find_pause_points.
                let r1 = (successor pos, tid, pp, g00, e, None)
                (blk, [r1])


            | W_apply(("hpr_pause_control", gis), cf, [v], _) ->
                let vale = rewrite_exp1 e (v)
                let pausemode = pause_control_handlex kK (("hpr_pause_control", gis), [vale])
                let tid' = { tid with pausemode=pausemode; }
                (None, [(successor pos, tid', pp, g00, e, an)])


            | W_apply(("hpr_atomic_add", gis), cf, [location; amount], _) ->
                let _ = has_outargs "site Xcall" ("hpr_atomic_add", gis)  // Certain calls, such as the atomic ops, are side-effecting as descriped by gis.outargs. Bevelab does not currently support these, apart from some old (pre atomics) support for hpr_test_and_set.
                easc_bstop application 
                // muddy "bevelab: atomic_add"

            | W_apply(("hpr_exchange", gis), cf, [location; vale; comparand; conditionalf], _) ->             
                easc_bstop application 
                //muddy "bevelab: exchange"

            // We need to spot testandset at several places since it is side-effecting (and pass-by-reference on first operand) using logic_assoc_exp as the main expression evaluator drops the side effect. We can look for the gis.outargs annotations please ...
            | W_apply(("hpr_testandset", gis), cf, [mutex; vale0], _) -> 
                dev_println "legacy testandset"
                let _ = has_outargs "site Xcall" ("hpr_testandset", gis) 
                //(fname, gis) =
                let mm = (fun()-> "Xcall " + hbevToStr cmd + " args'=" + sfold xToStr [mutex; vale0])
                let ww = WN ("hpr_testandset call") ww
                //vprintln 3 (mm())
                let vale = rewrite_exp1 e (vale0)
                let vale = rewrite_exp1 e vale // 
                let (r, ee) = bevelab_testandset ww mm e mutex vale
                //let e' = em_gen (fun _ -> "testandset") (f4o5 e) (tas mutex vale)
                (None, [(successor pos, tid, pp, g00, ee, an)])


            | W_apply(fgis, cf, args, _) -> // other hpr native functions - hpr_sysexit comes through here.
                easc_bstop application

            | _ -> sf "L1105"

    match cmd with
    | Xif(g1, tt, ff) -> // A block-structured if statement: not generally found in DIC-style code.  Who uses this? 
        let g2 = rewrite_bexp1 e g1
        // TODO: we should short cut here by applying (xbmonkey g2) - but Xif not normally found in DIC-style code.
        match xbmonkey g2 with
            | Some _ -> hpr_yikes ("bevelab: monkey Xif missing")
            | _      -> ()
        let n0 = ((-1, -1), tid, pp, X_true, e, None)
        let eef n = (cassert(length(snd n)=1, "Xif bevbev2 had other than one activation"); activation_e(hd(snd n)))
        let tt1:env_t = eef(bevbev2 ww (kK, r, db) n0 tt)
        let ff1:env_t = eef(bevbev2 ww (kK, r, db) n0 ff)
        let ef = env_mux kK g2 tt1 ff1
        (None, [((-1, -1), tid, pp, g00, ef, an)])

    | Xpblock lst ->
        let gen = nextgen()
        let remove_sef(es, ea, M, sef, cost) = ((es, ea, M, [], cost), sef, cost) // cost not decreased! TODO
        let reinsert_sef sef cost (es, ea, M, sef1, cost1) = (es, ea, M, sef @ sef1, costvec_parallel_combine cost cost1)
        let (e', sef, cost) = remove_sef e
        let ef = env_parallel_combine "Xpblock" (map (bevbev2a (WN "Xpblock next" ww) kK e' gen) lst)
        let ef' = reinsert_sef sef cost ef
        (None, [((-1, -1), tid, pp, g00, ef', an)])

    | Xbeq(g1, v, k) ->
        let g2 = rewrite_bexp1 e g1
        (* let _ = app (fun (_,v,e,_)->lprint (vd+10) (fun()=>"  " + xToStr e + "/"+ xToStr v + "\n")) e *)

        let log dbv = (logadd db (pos, dbv))//   lprint vd dbv)
        log (fun()->"Conditional branch Xbeq " + xbToStr g1 + "\n")
        //let _ = log (fun()->"Rewritten condition (take branch if false) is " + xbToStr g2 + "\n")
        //Beq branches if its guard exp is false or zero.

        if vd>=7 then
            vprintln 7 ("  Xbeq before=" + xbToStr g1)
            vprintln 7 ("   Xbeq after=" + xbToStr g2)

        let atrim cc (k, tid, pp, g, e, an) =
            if xi_isfalse g then cc else (k, tid, pp, g, e, an)::cc

        //dev_println(sprintf "bevelab: Xbeq condition %s is %s" (xbToStr g1) (xbToStr g2))
        let beq g2 =
            match xbmonkey g2 with
                | Some false -> (log(fun()->"  beq: taken ->" + i2s(!k) + "\n"); [ ((!k, 0), tid, pp, g00, e, an) ])
                | Some true ->  (log(fun()->"  beq: fall-thru ->" + pc2s(successor pos) + "\n"); [ (successor pos, tid, pp, g00, e, an) ] )                
                | None ->
                    //dev_println(sprintf "bevelab: Xbeq monkey: manifest condition not found for %s" (xbToStr g2))
                    let r0 = ((!k, 0), tid, pp, xi_and(g00,  xi_not g2), e, an)
                    let r1 = (successor pos, tid, pp, xi_and(g00, g2), e, an)
                    log(fun ()-> "Forked both ways, g= " + xbToStr(f4o6 r0) + "\n")

                    let ans = (*foldl atrim [] *)[r1; r0]
                    let ss (k, tid, pp , g, e, an) = if vd>=6 then vprint 6 ("Forked s, pc=" + pc2s k + ", g=" + xbToStr g + "\n")
                    //app ss ans
                    ans (* conditional branch forks the env *)
        (None, beq g2)

    | Xannotate(m, x) -> bevbev2 ww (kK, r, db) (pos, tid, pp, g00, e, Some m) x

    | Xskip ->
        let r1 = (successor pos, tid, pp, g00, e, None)
        (None, [r1])

    | Xgoto(ss, k) ->
        if !k < 0 then sf("bevbev2: missing Xgoto label " + ss)
        else (None, [ ((!k, 0), tid, pp, g00, e, an) ])

    | Xfork(s, k) ->
        let lab = snd pp
        let forkid = funique "fork"
        let subsid_pc = vectornet_m(forkid, (* points*) 1000I)
        let tid_l = { tid with tidid= "L" :: forkid :: tid.tidid }
        let tid_r = { tid with tidid= "R" :: forkid :: tid.tidid }
        vprintln 3 (" Xfork on thread '" + rdot_fold tid.tidid + "',  allocated id " + forkid)
        let g' = xi_and(g00, xi_deqd(fst pp, snd pp))
        if !k < 0 then sf(s + ": bevbev2 missing label")
        else (None, [ (successor pos, tid_l, pp, g00, em_gen (fun _ -> "fork") (f4o5 e) ([], []), an); 
                           ((!k, 0), tid_r, (subsid_pc, lab), g', e, None) ])


// Experimental: On a postdominate point (the second of a predom/postdom pair of points), all activations will reach it, by definition.  When all have got here, we can envmux them up
// and resume.  This avoids exponential growth in the number of control-flow paths between hard pauses.
// TODO explore futher ... and automate.

(*
 * For a level-sensitive wait, there are various implementations, such as one which always commits pending work and consumes a clock cycle. A succession of those would consume
 * successive clock cycles even when all guards hold, and the guards would have to hold in a temporarily-matching order. But our implemented here is better: it has no overhead
 * when the guard holds. When the guard does not hold, behaviour statically depends on where there is any work to commit. If there is not, it adds a conjunct to the edge guard.
 * If there is work and the guard does not hold, it commits the work and transfers to an additional, so-called, meta state,  where there is no further work to commit and that behaves
 * then like the empty env case.
 *
 * TODO: add the simpler form as well?
 *
 * Summary: An Xwaituntil that blocks with a non-null e must flush e on the first cycle and then enter a metastate where it sits until the guard holds.
 * 
 *)
    | Xwaituntil g1 -> // Level-sensitive wait.
        let ms = fst pos >= g_metastate // test - is this a metastate
        let g2 = rewrite_bexp1 e g1

        match xbmonkey g2 with
            | Some true ->
                if vd>=4 then vprintln 4 (sprintf "Xwaituntil guard manifestly always holds - no blocking needed")
                let r_continue = (successor pos, tid, pp, g00, e, an)
                (None, [r_continue])

            | Some false ->
                if vd>=4 then vprintln 4 (sprintf "Xwaituntil guard manifestly false.  Thread hangs indefinitely.")                    
                (Some PS_hard, [])
                
            | None ->
                let gt = ix_and g00 g2
                let gf = ix_and g00 (xi_not g2)
                let is_empty (a, b, _, sef, costvec) = a=[] && b=[] && nullp sef
                let zilch = is_empty e
                vprintln 3 (pc2s pos + ": Xwaituntil g2=" + xbToStr g2 + sprintf ", zilch=%A" zilch + ", enter metastate if not zilch on negation of guard which is gf=" + xbToStr gf)
                match (ms, zilch) with
                    | (true, false) -> sf ("Expected null env in a metastate entry") 
                    | (true, true) -> 
                        let r_continue = (successor pos, tid, pp, gt, e, an)
                        if vd>5 then vprintln 5 ("Ms continue:  metastate after committing the env writes");
                        (None, [r_continue]) // TODO check: this returns None for pause info but it does indeed block!

                        // Another version of this will not fermer hard on the gt branch...
                    | (false, false) ->  
                        let r_stay = ((fst pos+g_metastate, snd pos), tid, pp, gf, e, an) 
                        let r_move = (successor pos, tid, pp, gt, e, an)
                        (Some PS_hard, [r_move; r_stay]) // Applies to all continuations: get distributed by a little map later.

                    | (false, true) ->                 
                        // Don't need a holding state if there's no work in e and no sef (side-effecting functions) or PLI.
                        let r_slip = (successor pos, tid, pp, gt, e, an)
                        if vd>=5 then vprintln 5 ("Slip through on null env: no pause needed");
                        (None, [r_slip])



    | Xeasc(W_apply(fgis, cf, args, meo)) -> bevbev2_easc_call (W_apply(fgis, cf, args, meo))

    | Xeasc(W_valof(v, _)) -> bevbev2 (WN "Xeasc valof" ww) (kK, r, db) alpha (gec_Xblock v)

    | Xeasc(X_net _)     | Xeasc(X_bnet _)
    | Xeasc(X_bnum _)    | Xeasc(X_undef)    
    | Xeasc(X_num _) -> (None, [(successor pos, tid, pp, g00, e, an)])    

    | Xeasc(v) -> sf(pc2s pos + ": bevbev2: unsupported easc (side-effecting expression): " + xToStr v + "\n") 

    | Xassign(lhsL, rhs) -> // UNR should have organised side-effecting function calls to be in this simple "v := f()" form.
        let mm = fun() -> (pc2s pos + ": Xassign to " + xToStr lhsL)

        // Where a function call is non-referentially-transparent we expect it to have been pre-processed to be a free-standing assing of "temp := f()" form.
        // This will have been done by uniquify_non_reftransp ww (morenets) sp0
        // Example nonreftran functions are hpr_testandset(), hpr_alloc() and hpr_read().
        let (rhs', e) =
            let rhs = 
                match rhs with
                    // Old testandset killer - TODO remove this code.
                    // Beyond being nonreftran, hpr_testandset is call-by-reference on first operand and we use a special handler here.
                    // We need to spot hpr_testandset at several places since it mutates its first operand and using logic_assoc_exp as the main expression evaluator drops the side effect.

                    | W_node(old_cast, V_cast old_cvt_power, [ W_apply((f, gis), cf, args, _)], _) when f="hpr_testandset" -> xi_num 0
                    | W_apply((f, gis), cf, args, _) when f="hpr_testandset" -> xi_num 0
                    | rhs -> rhs
            //if not_nonep cf.tlm_call then Some arg
            match is_tlm_call rhs with
                | None ->
                    let rhs' = rewrite_exp1 e rhs
                    (rhs', e)
                | Some(W_apply((f, gis), cf, args, _)) ->
                    // TODO - here we need to see if TLM call is blocking or nonblocking.
                    let rhs' = rewrite_exp1 e rhs
                    (rhs', e)

        let (es, ea, em, sef, costvec) = e

        let pos3 (_, _, v) = if v < 0 then sf "pos3" else v
        let rhs' =
            match mine_prec g_bounda lhsL with
                //| arg -> rhs' //for now
                | prec ->
                    //dev_println (sprintf "Start ix_casting %s" (xToStr rhs'))
                    let ans = ix_casting (None) prec CS_maskcast rhs'
                    //dev_println (sprintf "Finished ix_casting %s" (xToStr rhs'))
                    ans

        let g = X_true
        let m_deleted_cost = ref []
        let m_evicted = ref []
        let jk_lhs = function
            | X_bnet ff ->
                //let f2 = lookup_net2 ff.n
                let vf = vol ff
                if vd >= 5 then vprintln 5 ("Making update to " + xToStr lhsL + " new value is " + xToStr rhs' + " with annotation " + annToStr an)
                let cst_new = costvec_sequential_add (bevelab_xi_cost kK.cost_queries rhs')  (0L,0L,None)//No cost for a const
                let es' = ((an, lhsL, None, g, rhs', vf, cst_new)::(vardel_s mm lhsL None (m_evicted, m_deleted_cost) es), ea, em)
                let cst_ded = List.fold costvec_minus_combine (costvec_parallel_combine costvec cst_new) !m_deleted_cost 
                // em_update : used only for scalar assigns.
                let em_update ((es, ea, em), k, v) = (es, ea, makemap1_add em (ARGM(k, g, rhs', vf)), sef, cst_ded)
                em_update (es', lhsL, rhs')


            | X_blift(W_bitsel(X_bnet ff, N, false, _)) -> // Bit insert assignment : cost augments - but should sub off old cost of that bit lane! todo
                //let f2 = lookup_net2 ff.n
                let n = X_bnet ff
                let subs = Some(xi_num N) 
                let cst_new = costvec_sequential_add (bevelab_xi_cost kK.cost_queries rhs') (0L,0L,None) // Bitselect operator is free - it is 'just' wiring.
                let ans = ((an, n, subs, g, rhs', vol ff, cst_new)::(vardel_s mm n subs (m_evicted, m_deleted_cost) es), ea)
                //let cst_ded = List.fold costvec_minus_combine (costvec_parallel_combine costvec cst_new) !m_deleted_cost 
                em_gen (fun _ -> "bitupdate") sef ans

            | X_blift(W_bsubsc(X_bnet ff, s, false, _)) ->  // Bit insert assignment : cost augments - but should sub off old cost of that bit lane! todo
                //let f2 = lookup_net2 ff.n
                let n = X_bnet ff
                let s' = rewrite_exp1 e s 
                let vf = vol ff
                let cst_new = costvec_parallel_combine (bevelab_xi_cost kK.cost_queries rhs') (costvec_sequential_add (bevelab_xi_cost kK.cost_queries s')  (1L,1L,None))
                let ans =
                    if constantp s' then  
                        let subs = s'
                        let ans1 = ((an, n, Some subs, g, rhs', vf, cst_new)::(vardel_s mm lhsL (Some subs) (m_evicted, m_deleted_cost) es), ea) // vardel should return old cost! or recomp...
                        ans1
                    else ((an, lhsL, Some s', g, rhs', vf, cst_new)::es, ea)
                em_gen (fun _ -> "bitupdate dynamic") sef ans

// Array subs are always whole location, so we dont need to rebuild the ftree, but we can get an undec error or else generate deep queries: TODO dp.
            | W_asubsc(X_bnet ff, subs0, _) -> // Array assignment
                //let f2 = lookup_net2 ff.n
                let net = X_bnet ff
                let s' = rewrite_exp1 e subs0 
                if vd >= 5 then vprintln 5 ("Subs' on lhs of array assign is " + xToStr s')
                let cst_new = costvec_parallel_combine (bevelab_xi_cost kK.cost_queries rhs') (costvec_sequential_add (bevelab_xi_cost kK.cost_queries s')  (1L, 1L,None))
                let vf = vol ff
                if false && constantp s' then  
                    let subs = s' (* (X_num(x_manifest_int(s', "bevbev2 jk_lhsa"))) *)
                    let (other_arrays, other_updates) = vardel_a mm net (Some subs) (m_evicted, m_deleted_cost) ea
                    let ea' = (net, (an, net, Some subs, g, rhs', vf, cst_new)::other_updates) :: other_arrays
                    // em_update not working for arrays any more
                    let em_update ((es, ea, em), k, v) = (es, ea, makemap1_add em (ARGM(k, g, rhs', vf)), sef, muddy "cv")
                    let ans = em_update ((es, ea', em), X_subsc(net, s'), rhs')
                    ans
                else 
                    let (_, other_arrays, other_updates) = vardel_a_at mm net s' ea
                    let ans = em_gen (fun _ -> "subscripted array write") sef (es, (net, (an, net, Some s', g, rhs', vf, cst_new)::other_updates)::other_arrays)
                    ans
            | other -> sf("hwe2: assign other lhs: " + xToStr other)
        let env = jk_lhs lhsL
        
        let er1_ x = lprintln vd (fun () -> esToStr x) //function
            //| (an, l, None,   g, r, vf, cst) -> lprint vd (fun()->"    " + (annToStr an) + (xToStr l) +  " := " + (xToStr r) + "\n")
            //| (an, l, Some s, g, r, vf, cst) -> lprint vd (fun()->"    " + (annToStr an) + (xToStr l) +  "[.o.] := " + (xToStr r) + "\n")
        let env_rpt(es, ea, m, pcg) = ()  // if vd>=5 then vprint 5 ((" Post assign scalars\n")); app er1 es; vprint 5 ("\n\n"))
        //env_rpt env
        let env = List.foldBack (evict_to_sef_or_tlm ww kK tid pos)  !m_evicted env // Order must be preserved should there be more than one.  Sef is backwards, but so might be m_evicted in the future.
        let log dbv = (logadd db (pos, dbv)) // lprint (!g_fsmvd) dbv)
        //log(fun()->  "  assign (nb unexpanded lhs)" + xToStr lhsL + " := " + xToStr rhs' + "\n")
    
        (None, [ (successor pos, tid, pp, g00, env, an) ])

    | Xlabel(l) -> (None, [(successor pos, tid, pp, g00, e, an)])

    | Xblock(l) -> // Sequential composition
        let rec jp = function
            | ([a], []) -> (None, [a])
            | ([a], h::t) -> 
                let er1 = function
                    | (an, l, None, r, vf, cst) -> lprint vd (fun()->"     " + (annToStr an) + (xToStr l) +  " := " + (xToStr r) + "\n")
                    | (an, l, Some s, r, vf, cst) -> lprint vd (fun()->"   " + (annToStr an) + (xToStr l) +  "[.o.] := " + (xToStr r) + "\n")
                //let env_rpt1(es, ea, m, pcg) = if vd>=5 then vprintln 5 ((" Pre step in seq block: " + h.ToString() + " \n")); app er1 es; vprint vd ("\n\n"))
                //let env_rpt(es, ea, m, pcg) = if vd>=5 then vprintln 5 ((" Post step in seq block:\n")); app er1 es; vprint vd ("\n\n"))                
                //let _ = env_rpt1 (f5o6 a)
                let (blk, a') =
                    try bevbev2 (WN "Xblock" ww) (kK, r, db) a h
                    with
                        | Name_alias x ->
                            if vd>=5 then vprintln 5 ("Caught mid-block name alias " + x)
                            raise (Name_alias ("rethrow mid-block name alias (should be caught above):" + x))

                //let _ = app (fun a -> env_rpt (f5o6 a)) a'

                // Make all statements of this sub AST appear to start at the initial position pc.
                let reset_pos(pos_, tid, _, g, e, an) = (f1o6 a, tid, f3o6 a, g, e, an)
                if conjunctionate is_nop t then (blk, a')
                elif not_nonep blk then sf ("mid seq block blocking cmd ignored (please compile this block-structured code before presenting to bevelab): " + hbevToStr cmd + "\nblocking stm =" + hbevToStr h + "\n" + t.ToString())
                else jp (map reset_pos a', t)

            | _ -> sf("bevbev2: branch inside basic block:" + hbevToStr cmd)
        jp ([alpha], l)

    | Xreturn rval -> 
        let rv = rewrite_exp1 e rval
        let l = kK.retnet
        let g = X_true
        let scalar_update m (es, ea, em, sef, costvec) (l, r) =
            // TODO: abstract this function
            let m_deleted_cost = ref []
            let m_evicted_ = ref [] // First assign to return is normally final, so don't need to worry about consecutive reassigns.
            let vf = vol_net l
            let cst_new = bevelab_xi_cost kK.cost_queries r
            let es' = (an, l, None, g, r, vf, cst_new)::(vardel_s m l None (m_evicted_, m_deleted_cost) es)
            let em' = makemap1_add em (ARGM(l, X_true, r, vf))
            let cst_ded = List.fold costvec_minus_combine (costvec_parallel_combine costvec cst_new) !m_deleted_cost 
            (es', ea, em', sef, costvec_parallel_combine costvec cst_ded)

        let e' =
            if nonep kK.retnet then
                if kK.big_bangp
                     then
                         vprintln 3 ("bevelab: pos=" + pc2s pos + ", No return net allocated for Xreturn (big_bang so not mapped to hpr_sysexit).")
                         e
                     else
                         vprintln 3 ("bevelab: pos=" + pc2s pos + ", No return net allocated for Xreturn. Mapped to hpr_sysexit instead.")
                         // Finish should perhaps be demoted orderwise to always be the last item?
                         let order_tag = Some("tid", tid.tidid, fst pos, next_tag())
                         pliadd ww kK e (None, XRTL(None, g00, [Rpli(X_true, (hpr_native "hpr_sysexit", order_tag), [rv])]))

            else
                if vd>=6 then vprintln 6 ("bevelab: pos=" + pc2s pos + ", Return net is allocated for Xreturn so assigning to it.")
                scalar_update (fun ()->"Xreturn bevbev2" + xToStr (valOf l)) e (valOf l, rv)

        //vprintln 5 ("return value rv=" + xToStr rv)
        (None, [((g_return_pc, 0), tid, pp, g00, e', an)])


    | Xcomment s -> 
        let log dbv = (logadd db (pos, dbv))//  lprint (!g_fsmvd) dbv)
        //log(fun()->"// " + s +  "\n")
        (None, [(successor pos, tid, pp, g00, e, an)])

    | Xlinepoint(lp, v) -> 
        let log dbv = (logadd db (pos, dbv))//  lprint (!g_fsmvd) dbv)
        let sss = lpToStr lp
        //log(fun()->sss +  "\n")
        ignore(pliadd ww kK e (None, XRTL_nop sss))
        (log_linepoint lp; bevbev2 (WN "Xlinepoint" ww) (kK, r, db) alpha v)


    | other -> sf((hbevToStr other) + ": Match bevbev2: form should have been removed by earlier operators.")


let metastate_imap i =
    match i with
        | Xwaituntil(g) -> i
        | (other) -> sf("metastate_imap: other instruction: " + hbevToStr other)


(*
 * Return vm labels that are branch targets where code joins.  An Xjoin and an Xlabel are typical
 * such places, but we find such places with explict searching instead of examining the instruction.
 *
 * For each forked thread, we need an idle state that it can rest in while it is not active.  The join
 * state is used for that, so the thread starts life at its exit point and branches to its start point
 * whenever it is activated.
 *
 *)
let find_pause_points ww (kK:bevelab_budgeter_t) msg (dicar:hbev_t array) =
    let len = dicar.Length
    let vd = !g_fsmvd
    let pause_points  = Array.create len None
    let visited = Array.create dicar.Length false
    let ww = WF 3 "find_pause_points" ww (msg + " start")
    // First scan: Will not always be accurate if variables are used in pause control arguments but nobody is using it that way at the moment.
    let m_count = ref 0
    //    dev_println ("Find pause points start")
    let rec pause_scan n (style:standing_pause_mode_t) cmd =
        match cmd with
            | Xblock lst -> List.fold (pause_scan n) style lst // Last one in list will dominate WaW.

            | Xeasc(W_apply(("hpr_pause", gis), _, args, _)) ->
                mutinc m_count 1
                match pause_handlex kK style (("hpr_pause", gis), args) with
                        | Some spm ->
                            let r = is_hard1 vd spm
                            if vd >= 5 then vprintln 5 (sprintf "Pause type %A %A at dicidx=%i" spm r n)
                            if pause_points.[n] <> None && valOf pause_points.[n] <> r then vprintln 1 ("+++ incident control flow arcs with different pause modes converge at " + i2s n)
                            Array.set pause_points n (Some r)
                        | None -> ()
                style

            | Xeasc(W_apply(("hpr_pause_control", gis), cf_, args, _)) ->
                let rr = pause_control_handlex kK (("hpr_pause_control", gis), args)
                rr 

            | other -> style

    let rec walk style pos =
        if pos < 0 || pos >= dicar.Length then ()
        else
            let cmd = dicar.[pos]
            ignore(pause_scan pos style cmd)
            let style' = style // for now
            if visited.[pos] then 
              (
                //
              )
            else 
             (
                 if vd>=8 then vprintln 8 ("pause_scan " + i2s pos);
                 Array.set visited pos true;
                 app (walk style') (dic_successors pos cmd)
             )
    let opening_pausemode = kK.default_pause_mode
    let _ = walk opening_pausemode 0
    //dev_println ("Find pause points end")
#if FFF_ // unused approach
    let rec walk pos =
        if pos < 0 || pos >= diclen then ()
        else 
            pause_scan pos (dicar.[pos]);
            walk (pos+1)
    walk 0
#endif
    ((pause_points:pause_points_t), !m_count)


let add_pausepoint pause_points n m t =
    vprintln 3 (m + sprintf ": adding new pausepoint at %i form=%A" n t)
    Array.set pause_points n (Some t)
    ()


// Old code for join points not needed now we use postdom scan?
// Also tracks visited code
let find_joinpoints ww sS1 (kK:bevelab_budgeter_t) msg (dicar:hbev_t array) =
    let diclen = dicar.Length
    let vd = sS1.vd
    let jpoints = Array.create diclen 0
    let visited = Array.create diclen false
    let ww = WF 3 "find_joinpoints" ww (msg + " start")
    let m_jp = ref []
    let rec walk pos =
        if pos < 0 || pos >= diclen then ()
        else 
            Array.set jpoints  pos (jpoints.[pos]+1)
            if visited.[pos] then 
              (
                if vd>=5 then vprintln 5 (sprintf "find_joinpoints: joinpoint at %i " pos)
                mutaddonce m_jp pos // A joint point is any place we can get to more than one way in a depth-first search (a back or cross edge).
              )
            else 
             (
                if vd>=9 then vprintln 9 ("find_joinpoints " + i2s pos)
                Array.set visited pos true
                app walk (dic_successors pos dicar.[pos])
             )
    walk 0
    let ans_old = rev !m_jp
#if SPARE
    (* Update ep: counts vm labels that have a flow into them: not used: code above replaces. *)
    let rec fep p = function
        | Xgoto(_, k) -> ()
        | Xannotate(_, s) -> fep p s
        | (_) -> Array.set jpoints  (p+1) (jpoints.[p+1]+1)
    let rec epwalk pos = if pos >= 0 && pos < diclen then (fep pos (dicar.[pos]); epwalk (pos+1))
#endif
    let rec mto pos = if pos >= diclen then []
                      elif jpoints.[pos] > 1 then pos::(mto (pos+1)) else (mto (pos+1))

    //let ans_new = mto 0
    let _ =
        if vd >= 4 then
            reportx 4 "Join Points Old" i2s ans_old
            //reportx 5 "Join Points New" i2s ans_new
    ans_old



(*
 * Generate a synchronous finite state machine from intermediate code in DIC form (imperative program fragment).


The old procedure used active and closed sets of activations.  The
active set initially contains a member for each thread entry point and
the closed set is null.  The program is considered to be a flow chart,
annotated with barrier points.  The initial activation for a thread
follows the flow chart and forks into a pair of active activations at
the first conditional branch.  This behaviour is then applied to the
new applications until the active set is empty, at which point the
procedure has finished and the result is the closed set.

In basic operation, at a join point in the flow chart, where
activations might converge, the arriving activations are committed to the
closed set and a new active activation is created that represents the
departure of the union of all arriving activations.

When an activation encounters an assignment or PLI call, this operation
is added to the body of the activation, building a symbolic environment.

Where an activation encounters a barrier, it is closed and a fresh
active activation created to model execution beyond the barrier.
Activations arriving at the exit point of the program are also closed
at that point.

Symbolic evaluation enables certain conditional branches to be
determined as unconditional, and hence an activation can follow the
branch without forking.  This implements loop unwinding and tends to
reduce the number of closed activations in exchange for increasing the
complexity of those that do close.  Such loop unwinding can be done
in a prior step if desired, as can the contra operation of combining
an alternate pair of basic blocks by predicating their operations.

It is not always necessary to close an activation at a joinpoint as
the separately arriving activations can be allowed to continue onwards
separately. However, if no restrictions are placed on this, the number
of active activations grows exponentially (or even becomes unbounded?) and the procedure will not
return.  On the other hand, if every loop contains a barrier, the
number of activations becomes bounded but no compile-time loop
unwinding is possible.


We could detect infinite loops by ... ?

The set of closed activations must be race compatible.  Race compatible means
that that no two closed activations contain a pair of assignments to
the same variable under the same conditions that disagree in value.
Duplicate assignments of the same value at the same time are
discarded.  This checking cannot always be complete where values
depend on run-time values, with array subscript comparision being a
common source of ambiguity.  Where incompatiblity is detected, an
error is flagged.  When not detected, the resulting system can be
non-deterministic.

Visited is an array indexed by program counter that containins the
activations that have already visited that point, enabling duplicate
execution trajectories to be discarded more quickly, rather than
waiting until duplicates are deleted when attempting to join the
closed set.

*)


let autobarrier msg fermer nx =
    let n' =
        match fermer with
            | None ->  sf ("no fermer for auto barrier (perhaps name alias handler)  : " + msg)
            | Some ff -> ff "autobarrier" (Some PS_auto) nx // was soft on the basis that a soft is only deleted on an empty env, which could not contain a name alias, but no longer worked...
    cassert(length n' = 1, "More than one activation after autobarrier")
    hd n'



// Try executing a command, but if it raises a name alias, implement an autobarrier by running it again after
// executing fermer on the input activation.
let rec bevbev20 ww (kK, fermer, db) n x =
    let ans =
        bevbev2 (WN "bevbev20" ww) (kK, fermer, db) n x
#if OLD22
        with Name_alias ss when false -> // Disabled: because we need to catch lower down to get pause into dones list
            let (pos, tid, pp, g, e, an) = n
            let mm = " autobarrier to overcome ambiguous name alias " + pc2s pos + " " + ss
            vprintln (!g_autob_loglev) ("Inserting " + m)
            let n' = autobarrier mm fermer n
            vprintln (!g_autob_loglev) ("Inserted " + m)            
            in try bevbev20 ww (kK, fermer, db) (n') x
               with Name_alias ss -> sf ("name alias not caught on second try " + ss)
#endif
    ans


let not_boring = function
    | Xlinepoint _
    | Xlabel _ 
    | Xcomment _ -> false
    | _ -> true



            
//
// 
//
let bev_compile_exec_2_end ww0 sS0 mt (minfo:minfo_t) iinfo sp5_in =
    let ww = WF 3 "bev_compile_exec_2_end" ww0 "start"
    let kK0 = sS0.kK
    let morenets = valOf_or_fail "No 'morenets' collector installed" (kK0.morenets)
    let vd = sS0.vd
    match sp5_in with
    | H2BLK(director, SP_dic(ar0_in, ctrl0_in)) ->
        let tname = ctrl0_in.ast_ctrl.id
        if kK0.redirect then bireduce_dic ww tname ar0_in // Clean up API do bireduce please Perhaps do in a cloned copy instead of mutating input ?  - we do clone in assmbler_vm afterall.  
        let _ = WF 3 "bev_compile_exec_2_end" ww0 "uniquify done"
        let tid0:tid_t = { tidid= [ mt ]; pausemode=kK0.default_pause_mode; }
        let msg = tname
        let start_point_is_pause_point =
            match director.hang_mode with
                | DHM_auto_restart                  
                | DHM_pipelined_stream_handshake _ -> true
                | _ -> false
        vprintln 3 (sprintf "h2blk: SP_dic: starting pause mode for thread %s is '%A'.  start_point_is_pause_point=%A" tname tid0.pausemode start_point_is_pause_point)

        let assemble_vd = -1 // Off. Don't need a listing.
        let (improved, gdic) = assembler_vm ww assemble_vd ctrl0_in true ar0_in
        let sp5 = H2BLK(director, improved)
        let ctrl0 = ctrl0_in // for now?
        let queries =
            { g_null_queries with
                concise_listing=   true
                yd=                YOVD 3
                title=             "post-revast print, and post all catenate calls"
            }
        let _ = anal_block ww None queries sp5 // printed once now - after all catentate calls

        let ww = WF 3 "bev_compile_exec_2_end" ww0 "reported recoded machine"
        let diclen = (!gdic.DIC).Length
        let clkinfo_string = "clk=" + (if nullp director.clocks then "NONE" else sfold edgeToStr director.clocks)
        let _ = WF 1 "bevelab H/W elaboration" ww (mt + ":" + ctrl0.ast_ctrl.id + ": clkinfo=" + clkinfo_string + sprintf ", diclen=%i decimal" diclen)
        let labels = collate_labels !gdic.DIC diclen
        let (pause_points, count) = find_pause_points ww kK0 msg !gdic.DIC
        if start_point_is_pause_point then Array.set pause_points 0 (Some PS_hard)

        vprintln 1 (sprintf "Thread %s: User-annotated pause count = %i pauses " tname count)
        let _ =
            if count > 0 then
                let pp_rpt q = if vd >= 5 && not_nonep q then vprintln 5 (sprintf "Initial pause points %A" q)
                for x in pause_points do pp_rpt x done
        let joinpoints = find_joinpoints ww sS0 kK0 msg !gdic.DIC

        let _ =
            if false then // A backedge pause stops any loop being unwound
                vprintln 3 ("Adding backedge pauses") 
                //Join points are sufficient for now.
                let (bblock_starts, numbering, fwd, back, postdoms, backedge_nodes) = find_cfg_postdoms ww vd "bev_compile_exec_2_end" !gdic.DIC
                let mben x =
                    Array.set pause_points x (Some PS_soft) 
                    vprintln 3 (sprintf "Thread %s: Added backedge pause at %i" tname x)
                    ()
                app mben backedge_nodes
        // These fag nets are actually symbolic constants to be rewritten later.
        // Best to insert them all into the enumeration before using them since this helps with disjoint condition testing.
         
        //vprintln 3 ("Join points are :" + sfold i2s joinpoints)

        //let pc = X_bnet(flags_sortout(funique g_pcnet, [Onq_dot_range(Onum points, Onum 0), Onq_init(Onum 0)]))
        (* Use an integer for raw/unpacked PCs since they have to with metastates too *)
        (* The master_pc starts execution but may fork subsids *)

        let master_pc = intnet(funique "mpc", []) // This will become an enum. Needs to be changed in name in repack, compose and restructure as these are different enums yet the meox name space if flat and persistent.

        // Todo: use statename1 in abstracte and ensure enumeration gets closed
        let fag (n, phase) =
            if n < 0 then sf ("+++ adding exit location (-ve) to pc enumeration!!!")
            enum_add "fag" master_pc ((xToStr master_pc) + ":" + (if n=0 then "start" else "") + i2s n) n

        let startstate = fag (0, 0)
        let nulenv = em_gen (fun _ -> "null start") [] ([], [])

        let kK = { kK0 with big_bangp= false } // Start of universe ? 
        let set000 = ((0,1), tid0, (master_pc, startstate), xi_true, nulenv, None)
        //vprintln ("Set000  guard g=" + xToStr xi_true + "\n")

        //let er1 x = if vd>=4 then vprintln 4 (esToStr x)

        let posnorm pos = if pos >= g_metastate then pos-g_metastate else pos

        let insget (pos) =
            if (pos >= g_metastate) 
            then metastate_imap (!gdic.DIC).[pos-g_metastate]
            else (!gdic.DIC).[pos]
        let dis pos = hbevToStr (insget pos)

        let bevbev4 (fermer, db) nx =
            let (pos, tid, pp, g, e, an) = nx
            let (es, ea, em, sef, cv) = e
            if xi_isfalse g then 
               if vd>=4 then vprintln 4 "Discard null activation"
               (None, [])
            elif fst pos >= diclen then
                vprintln 3 (sprintf "Reached end of control flow graph at %A/%i" pos diclen)
                let n' = autobarrier "end-of-flow fermer apply" fermer nx
                vprintln 3 (sprintf "End of control flow graph: post autofermer %A/%i" pos diclen)
                (None, [])
            else
                let x = insget (fst pos)
                (* let _ = p "hbev: " n *)
                if vd >= 5 then mya_stats 5
                if vd >= 5 then vprintln 5 ("bevbev4 comp: " + "g=" + xbToStr g + " " + xToStr(snd pp) + " pc=" + pc2s pos + ": " + hbevToStr x)
                let (blko, ans) = bevbev20 (WN "bevbev4" ww) (kK, fermer, db) nx x
                if vd>=4 then vprintln 4 ("bevbev4 comp done: " + xToStr(snd pp) + " pc=" + pc2s pos + ": blk=" + optToStr blko)
                if vd >= 5 && not_boring x then app (activation_print vd "gave: ") ans
                //vprintln 5 ("blko=" + optToStr blko + "\n\n")
                (blko, ans)

        let isjp (pos, tid, pp, g, e, an) = 
            let ans = memberp (fst pos) joinpoints
            //vprintln 0 ("isjp ? " + pc2s pos + " ans=" + boolToStr ans)
            ans


        (* We make a new unwind resultvec for an exploratory unwind: link to prev r,
                and link to previous visited array. *)
        let mu x = if x < g_metastate then x else x-g_metastate+diclen

        let subsid_threads = ref []
        let log_subsid_thread forkers_tid (pc, startpos) = mutaddonce subsid_threads (pc, (statename pc (xToStr pc) startpos, is_hard (is_hard1 vd forkers_tid.pausemode)))
        (*
         * bevbev3 routine steps through a basic block and does not consume ubudget within
         * such blocks.  The e form is converted to em form at entry to block and em is mutable within the block.
         *)
        let exit_pred (pos, tid, pp, g, e, an) = fst pos < 0 
        //let srpt1 m (pos, tid, pp, g, e, an) = if vd>=4 then vprintln 4 (m + " " + i2s(length (f1o5 e @ list_flatten(map snd (f2o5 e)))) + " lts items, base=" + xToStr(snd pp) + " pc=" + (pc2s pos) + " g=" + xbToStr g + ", env="); app er1 (f1o5 e @ (list_flatten (map snd (f2o5 e))))
                
        let gen_rv_closure() =
            let m_rv = ref [] (* new_resultvec(ref [], []) *)
            let is_empty (a, b, _, sef, _) = nullp a && nullp b && nullp sef
            let get_sef (a, b, c, sef, _) =
                let ans = rev sef // Reverse sef - built up backwards
                // sef = side-effecting functions with tags
                let sefToStr(tag, b) = xrtlToStr b
                //vprintln 0 ("fermer sef plicall:  " + sfold (sefToStr) sef)
                map snd ans  // discard tags here for now


            let fermer_env = function
                | (_, l, None,      g, r, _, _) -> gen_XRTL_arc(None, g, l, r) 
                | (_, l, Some subs, g, r, _, _) -> gen_XRTL_arc(None, g, safe_xi_asubsc "bevelab fermer" (l, subs), r)

            let fermer1 mm m_rv (style) = function
                | (pos, tid, pp, X_false, e, an) -> ()
                | (pos, tid, pp, g, e, an) -> 
                    let cmds = (get_sef e) @ map fermer_env ((f1o5 e) @ (list_flatten (map snd (f2o5 e))))
                    let rvi = (pp, g, (if fst pos < 0 then None else Some(fag pos)), cmds, style)
                    mutadd m_rv rvi
                    if vd >= 5 then vprintln 5 ("F:Goto (unused?)" + (annToStr an) + (pc2s pos) + " on " + xbToStr g)
                    ()

            let fermer mm style_str n0 = 
                let (pos, tid, (pc, pcv), g, e, an) = n0
                let style1 = if style_str<>None then valOf style_str elif fst pos < 0 then PS_hard elif pause_points.[fst pos]<>None then valOf (pause_points.[fst pos]) else (ignore(Array.set pause_points (fst pos) (Some PS_hard)); PS_hard)
                let style = (*is_hard1*) style1
                let zilch = is_empty e
                if vd>=4 then vprintln 4 (mm + " " + xToStr pcv + "->" + pc2s pos + sprintf ": Fermer: zempty=%A style=%A"  zilch  style + " cost=" + costToStr(f5o5 e))
// TEMP: we must close zilch because need resume point?
                if false && style=PS_soft && zilch && (pcv<>startstate)// dont discard start state since we do not redirect to an alternative yet && false // for now
                then
                    if vd>=4 then vprintln 4 ("Fermer: soft and empty - fermer skipped letting run. Also starting fresh from " + pc2s pos)
                    // need to start the n1 activation in case it is a join point that is branched to otherwhere ...
                    let n1 = if fst pos < 0 then [] else [(pos, tid, (pc, fag pos), X_true, nulenv, None)]
                    let ans = n1 @ [ n0 ]
                    //app (srpt1 "softset") ans
                    ans
                else
                    fermer1 mm m_rv style n0
                    if (fst pos < 0) then [] else [ (pos, tid, (pc, fag pos), X_true, nulenv, None) ]
            (m_rv, fermer)

        let maximal = kK.default_pause_mode = SPM_maximal
        let bblock  = kK.default_pause_mode = SPM_bblock        
        let add_bblock_pause n = add_pausepoint pause_points n "basic-block join point" (is_hard1 vd SPM_bblock)
        if bblock then app add_bblock_pause joinpoints
            
        // bevbev3 advances in basic block till fork, end, joinpoint or blocking call and then lets bev0 do the detailed control.
        // This may still change activation owing to name alias barriers in bevbev20
        let (m_rv, fermer) = gen_rv_closure() // TODO pass in whole closure
        let about = vprintln (!g_autob_loglev) 
        let retry (pos, tid, pp, g, e, an) = ((fst pos, 0), tid, pp, g, e, an) // reset phase to zero to restart this state.


        // m_recombs is a mutable list where arrived recombs are placed.
        // We also return an indication if any have been added.
        // stack is used to insert autobarriers
        let rec bevbev3 (db) stack ms n =
            let (pc, phase) = (f1o6 n)
            let m0 = "bevbev3 " + ms  + " elab from base=" + xToStr(fst(f3o6 n)) + "/" + xToStr(snd(f3o6 n)) + " pc=" + pc2s(f1o6 n)
            (if phase=0 then bevbev3_zero else bevbev3_one) (db) stack ms n m0

        and bevbev3_zero (db) stack ms nx m0 =
            let (pos, tid:tid_t, pp, g, ee, an) = nx
            if vd >= 5 then vprintln 5 m0
            let eq_key = (pos, f1o5 ee, f2o5 ee) // discard mapping since no (or jolly expensive in FSharp) equality test

            let autopause_time =
                let lcpred = function
                    | CV_single n     -> n >= kK.soft_pause_threshold
                    | CV_stranded cve -> cve.maxa >= kK.soft_pause_threshold
                tid.pausemode = SPM_soft && lcpred (f5o5 ee)
            //vprintln 3 (sprintf "Checking pause point %i of %i" (fst pos) pause_points.Length)
            let pause = if autopause_time then Some PS_soft elif fst pos < pause_points.Length && not_nonep pause_points.[fst pos] then pause_points.[fst pos] elif maximal then Some PS_hard else None

            
            let nx = ((fst pos, snd pos + 1), tid, pp, g, ee, an) // Increment phase - i.e. support more than one pc name at a single input DIC pc address.
            match pause with
                | Some style ->
                    //cassert(!recombs = [], "recombs = []");
                    let mm = sprintf "deploy pause style=%A encountered at " style + pc2s pos
                    vprintln 2 m0
                    vprintln 2 mm                    
                    let nx = fermer mm (pause) nx
                    (m_rv, [], true, nx, stack) // arb
                                                                                
                | None ->
                    let stuck_wontunwind = member_twicep eq_key stack
                    if stuck_wontunwind
                    then
                        app (fun n -> about("    traceback " + pc2s(f1o3 n) + ":" + dis(fst(f1o3 n)))) (rev stack)
                        let m1 = (mt + ": Autobarrier needed. Same configuration noticed three times in succession: unwindable non-blocking loop (manually add pause to body to supress this warning)")
                        about m1
                        add_pausepoint pause_points (fst pos) "wontunwind" PS_auto
                        (m_rv, [], true, [retry nx], stack) // arb
                    else bevbev3 (db) (eq_key::stack) ms nx // No pause, go round again for phase 1

        and bevbev3_one (db) stack ms n m =
            let (pc, phase) = (f1o6 n)
            if vd>=4 then vprintln 4 m
            if vd>=5 then activation_print 5 "bevbev3 start: " n
            
            let (blk, nlst) =
                try
                    let (blko_, nlst) = bevbev4 (Some fermer, db) n
                    (false, nlst) // Ignore old pause blko from now on ... rely on pause_points list ... but can reinstate for genuine restarters TODO tidy
                    
                with Name_alias ss ->
                    add_pausepoint pause_points (pc) "bevbev4 name alias detected" PS_auto
                    (true, [retry n])

               // We have slightly different reasons for stopping, reporting blocking to parent and calling fermer:
               // 1. If we have bifurcated, we do not need to fermer, but we return two pending activations.
               // 2. If we have joined on control flow we wish to fermer if we are in basic block mode - but this can now be done by premarking with pause points
               // 3. NEW: If we have joined on control flow we will wish to park until all postdoms arrive, which can be ensured by maintaining FIFO order on the activations.
               // If we have joined on threads we need compatible activations and must let our parent elide them in the viz array, but we do not need to fermer here.
               // Name alias, struct hazard or running cost leads to a fermer of existing followed by a restart, but that fermer must make it into the dones list otherwise test21 etc does not halt.
               // 3. If the code has a hard pause we certainly need to fermer (but we could continue locally No: does not get included in the dones list. ?) and this is marked in the pause_points array.
            // We can consider each pc value has a pre/post pause version embodying a pre-fermer site (is this the phase?).
            // ? problem with other back edges to joinpoint sometimes means it is missing from generated program
            // Those activations that have exited we hand to fermer, those that have freshly reached a join point we park and those that have split we return to parent: all others proceed serially!
            let trimp nx (parked, ready) =
                if exit_pred nx then
                    if vd >= 5 then vprintln 5 ("Activation exited at " + pc2s(f1o6 nx))
                    let  _ = fermer "trimp-exited" None nx // Pass exiting activation to fermer with no further ado.
                    (parked, ready)
                elif isjp nx then
                    (nx::parked, ready)  // Activations reaching joinpoint are placed on parked list. A joinpoint is not necessarily a pause point.
                else (parked, nx::ready)
            let (parked, nlst) = List.foldBack trimp nlst ([], [])
            match (parked, nlst) with
                | ([], [singleton]) when not blk ->
                    if vd >= 5 then vprintln 5 ("Continue processing in bevbev3.")
                    bevbev3 (db) stack "continue" singleton
                | (_, _) ->
                    if vd >= 5 then vprintln 5 ("Stop processing in bevbev3.") 
                    let activation_paused = nullp nlst || blk
                    (m_rv, parked, activation_paused, nlst, stack) // arb

        (*
         // At the bevbev0 level we proceed on all control bifurcations of an activation until they block. 
         // At the bevbev? level we proceed on all threads that have been forked.
         // At the iterate level we proceed on all pause-to-pause activations.
         * Bevbev0 needs to advance,  on all forks of a route until they block.
       
         * Can we better explain why iterate and bevbev0 BOTH iterate breadth-like ? Yes, the greaves simple algorithm evaluates every reachable pause-to-pause path, where many such paths start out on top of each other and then go their various ways at conditional branches.
         *)


        // Maintain FIFO order by reversing recomb_queue and using it as the new item set when item set is empty.
        let rec bevbev0 recomb_queue paused_activations items =
            if nullp items && nullp recomb_queue then 
                if vd>=5 then vprintln 5 ("Stop processing in bevbev0. All work done.")
                paused_activations
            elif nullp items then
                if vd>=5 then vprintln 5 ("No items in queue. Need next recomb to proceed.")
                //let _ = mutinc g_recomb_limit 1
                //if !g_recomb_limit > 100 then sf "Temp stop on recomb infinite loop"
                // We take a head activation off main work list and recombine it with similar ones present on that list.
                // TODO preserve FIFO order - do all of them and reverse
                // Why is this called twice?
                let _ =                  recomb_fun sS0 recomb_queue
                let (hs, recomb_queue) = recomb_fun sS0 recomb_queue
                bevbev0 recomb_queue paused_activations hs
            else
            match items with
              | (h, stack)::tt ->
                let mm = "RESUME"
                if vd>=5 then vprintln 5 (pc2s(f1o6 h) + " " + mm + sprintf ": number to go %i,  recomps=%i" (length tt) (length recomb_queue))
                let (m_rv, parked, activation_paused, lst, stack') = bevbev3 (ref []) stack mm h
                let augment_with_antecedant_trajectory_stack x = (x, stack')
                let recomb_queue = (map augment_with_antecedant_trajectory_stack parked) @ recomb_queue
                // We do want to localising the loop finding stack for error reports.

                if activation_paused then
                    if vd>=5 then vprintln 5 ("Activation paused. Continue processing another resume in bevbev0")
                    bevbev0 recomb_queue (lst @ paused_activations) tt
                else
                    if vd>=5 then vprintln 5 ("Continue processing, having noted " + i2s (length lst) + " additional branch points in bevbev0")
                    bevbev0 recomb_queue paused_activations ((map augment_with_antecedant_trajectory_stack lst) @ tt)
                       
        let combine_budgets = function
            | ([], l) -> l
            | (a::t, l) -> (a :: t) @ l (* Does this really combine anything ? It's just an append *)

        let aToS (pos, tid, pp, g, e, an) = ("Resume/entry point pc=" + pc2s pos + " g=" + xbToStr g)
        (* Iterate over all activations, until there are none left. *)
        let rec iterate credit dones activations =
            if vd>=5 then vprintln 5 ("P-Iterate: " + i2s(length activations) + " activations")
            match activations with 
            | [] ->
                if vd>=5 then vprintln 5 ("P-Iterate all activations finished")
                ()
            | h::tt ->
            // Skip those processed already: what sources of equality failure might there be ?
            // This is the simple all pairs comparison: it's the stuck_wontunwind detection that does equality on environments.
            // Only looking at the resume and place fields anyway in the he_key.
            let mkcomp a = (f1o6 a, f3o6 a)
            let he_key = mkcomp h
            let a12s (pos, pp) = xToStr(snd pp) + "->" + pc2s pos
            if vd>=5 then vprintln 5 ("Consider done already " + a12s  he_key + " in dones = " + sfold a12s dones)
            if memberp he_key dones then
                if vd>=5 then vprintln 5 ("Discard done already alpha=" + a12s he_key)
                iterate credit dones tt
            else
                if vd>=5 then vprintln 5 ("-----\n\n\nResume/entry point " + a12s he_key)
                let set' = bevbev0 [] [] [(h, [])]
                if vd>=5 then vprintln 5 ("P-Iterate " + aToS h)
                //app (srpt1 "postset") set'
                if credit<0 then
                           // TODO need a cleaner message here - uses's will encounter this
                           sf("bevelab: iteration limit exceeded: thread never blocks or spawns new ones infinitely ?")
                let dones = he_key::dones
                if vd>=5 then vprintln 5 ("Adding resume/suspend pair " + a12s he_key + " to dones:  lst=" + sfold a12s dones)   
                iterate (credit-1) dones (combine_budgets(set', tt))


        iterate kK.ubudget [] [set000]

        let pcinfo = (master_pc, (startstate,  is_hard(is_hard1 vd tid0.pausemode)))::(!subsid_threads)
        if vd>=4 then vprintln 4 (mt + ": bevelab H/W elaboration finished. Threads=" + i2s(length pcinfo) + "\n")
        let ans = !m_rv
        //reportx 3 "Final rv contents" xrtlToStr ans ... call wrap here?
        (ans, pcinfo, director)




let bev_compile_exec_2_mid ww0 sS0 m (minfo:minfo_t) iinfo catenates_performed sp0 =
    let vd = sS0.vd
    let ww = WF 3 "bev_compile_exec_2_mid" ww0 "start"
    let kK0 = sS0.kK
    let morenets = valOf_or_fail "No 'morenets' collector installed" (kK0.morenets)
    let sp2 = uniquify_non_reftransp ww vd (morenets) sp0

    let dotrep sp2 keyname = 
        let richf = Some false
        if sS0.render_cfg_dotreport then
            let keyname = filename_sanitize ['_'; '.'] keyname
            let title = keyname
            let dot_reports = hpr_sp_dot_report ww title richf [sp2]
            //vprintln 0 (sprintf "dotcode %A" code)
            let ww = WF 2 "bevelab" ww (sprintf "Writing a dot cfg report.  keyname=%s" keyname)
            render_dot_plot ww keyname dot_reports
            ()


    match sp2 with
    | H2BLK(director, SP_dic(ar0, ctrl0)) ->
        if kK0.redirect then bireduce_dic ww ctrl0.ast_ctrl.id ar0 // Perhaps do in a cloned copy instead of mutating input ?  - we do clone in assmbler_vm afterall. 
        let _ = WF 3 "bev_compile_exec_2_mid" ww0 "uniquify done"
        let tname = ctrl0.ast_ctrl.id
        dotrep sp2 tname
        let tid0:tid_t = { tidid= [ m ]; pausemode=kK0.default_pause_mode; }
        vprintln 3 (sprintf "h2blk: SP_dic: starting pause mode for thread %s is '%A'" tname tid0.pausemode)
        // (vlnvToStr minfo.name)
        let (improved, _) = assembler_vm ww vd ctrl0 true ar0 // Where/when  to assemble?
        let sp2 = H2BLK(director, improved)
        if vd >= 4 || catenates_performed then

            let queries =
                { g_null_queries with
                    full_listing=      true
                    yd=                YOVD 3
                    title=             "pre-revast print, but post all catenate calls"
                }
            let _ = anal_block ww None queries sp2 // A pre-revast print, post all catenate calls
            ()
        else vprintln 3 ("No input code listing logged since no recompile of " + hptos tid0.tidid)
        let sp3 =
            if kK0.revast_enable then

                if true then
                    let ctrl0 = ctrl0 // for now?
                    let queries =
                        { g_null_queries with
                            concise_listing=   true
                            yd=                YOVD 3
                            title=             "pre-revast print, but post all catenate calls"
                        }
                    let _ = anal_block ww None queries sp2 // This print can go as an option in revast
                    ()
                ast_reveng_find ww vd tname minfo sp2
            else sp2

        if true then
            bev_compile_exec_2_end ww0 sS0 m (minfo:minfo_t) iinfo sp3
        else 
            muddy "(sp3, pcinfo, director)" // Want to just invoke revast, round trip or not

//
// Here we collate and catenate various exec forms prior to the main compile.
//   
let rec bev_compile_exec_2 ww0 sS0 m minfo iinfo catenates_performed sp0 =
    let ww = WF 3 "bev_compile_exec_2" ww0 "start"
    let vd = sS0.vd 
    let morenets = valOf_or_fail "No 'morenets' collector installed" (sS0.kK.morenets)
    let sp2 = compile_exec_to ww vd m minfo sp0

    match sp2 with
        | H2BLK(dir__, SP_dic(ar, id)) ->
            let catenates_performed = false
            let (ans, pcinfo, director) = bev_compile_exec_2_mid ww sS0 m minfo iinfo catenates_performed sp2
            //let (ans, pcinfo, director) = bev_compile_exec_2_mid ww sS0 m minfo iinfo sp2
            (ans, pcinfo, director, [])                

        | H2BLK(dir, SP_asm _)       -> muddy "bevelab: compile SP_asm to FSM not implemented - please lift to hbev form first."

        | H2BLK(dir, SP_seq lst) ->
            vprintln 3 (sprintf "sp_catenate: sequential list of %i sp blocks" (length lst))
            let m_catenates_performed = (length lst > 1)
            let (dir, combined_sp) = sp_catenate dir lst
            bev_compile_exec_2 ww0 sS0 m minfo iinfo catenates_performed (H2BLK(dir, combined_sp))

        | H2BLK(dir, SP_par(pstyle, h::t)) -> 
            let (a, b, c, untouched) = bev_compile_exec_2 ww sS0 m minfo iinfo catenates_performed (H2BLK(dir, h))
            muddy "parblock synthesis needs minor work - fork/join synthesis is already supported"
            //bev_compile_exec_2 ww0 sS0 m minfo iinfo (H2BLK(dir, combined_sp))


        | H2BLK(dir, SP_comment ss) ->  ([], [], dir, [sp2]) // Comments etc are untouched

        | H2BLK(dir, SP_rtl(_, v))  ->  ([], [], dir, [sp2])

        
        // These non-trivial SP_l clauses look broken ...  all is ignored?
        | H2BLK(dir, SP_l(ast_ctrl, Xassign(l, r))) ->   // TODO do not ignore clk_enables here ...
        (* A strange way to represent no pc : but putting none means datapath synch with no fsm-state owner *)
            ([(*  HWDG(None, g_async_designation, xi_true, l, r)*) ], [], dir, [])


    //    | (H2BLK(None, SP_l b)) -> hbevToStr b

        | H2BLK(clkinfo, SP_l(ast_ctrl, b)) -> muddy("bevelab soster " + hbevToStr b)    

        | other -> sf("bev_compile_exec_2 elaborate other SP form: " + execSummaryToStr other)



//
//  Wrap up the answer in FSM form - which is basically a shannon projection onto the program counter.  (Although it is mostly in that form already!)       
//
let wrap_ans_as_fsm ww sS1 (director, (pc, rst)) mid style_get rv =
    let vd = !g_fsmvd

    let arcwrap((pc, pv), gg, d, cmds, style) = // Convert to output FSM form.
        //reportx 0 ("Wrap results " + xToStr pc + "=" +  xToStr pv + " g=" + xbToStr gg + " d=" + (if d=None then "None" else xToStr(valOf d))) (xrtlToStr) cmds
        let si =
            { g_null_vliw_arc with
               pc=            pc
               iresume=       xi_manifest "iresume" pv
               resume=        pv
               dest=          (if nonep d then X_undef else valOf d)
               idest=         (if nonep d then -1 else xi_manifest "idest" (valOf d))
               gtop=          gg
               cmds=          cmds
               hard=          style_get pv
               eno=           next_global_arc_no()
            }
        si:vliw_arc_t
    let arcs = map arcwrap rv    

    let resumes =
        let mine_res cc (si:vliw_arc_t) = singly_add (si.resume, si.hard) cc
        List.fold mine_res [] arcs
    vprintln 3 (sprintf "Resumes were " + sfold (fun (b, a) -> visitToStr a + xToStr b) resumes)
    let n_resumes = length resumes
    vprintln 3 (sprintf "There are %i resumes for scheduling" n_resumes)

    let alldests = 
        let rec hf w = function  // This is in abstracte as hard_flag_retrieve - we do not need this copy of this code. TODO DELETE
            | [] when w=X_undef -> g_default_visit_budget
            | [] ->
                hpr_yikes (xToStr pc + ":hf: no resume for " + xToStr w)
                g_default_visit_budget  // Should not be used but is under maximal sometimes - TODO use the copy in abstracte.
            | (h, hardf)::tt when h=w -> hardf
            | _::tt              -> hf w tt
        let trawl cc  = function
            // Thread exit point(s) is not listed as a resume - so dig it out here if present - yuk - TODO move routine to abstracte
            // Reset point is not always a resume.
            | (si:vliw_arc_t) -> singly_add (si.dest, hf si.dest resumes) cc
        List.fold trawl resumes arcs
    if vd >= 5 then vprintln 5 (xToStr pc + ": All dests are " + sfold (fun (b,a) -> visitToStr a + xToStr b) alldests)
    let threadname = xToStr pc
    let majorstat_line = sprintf "Thread %s has %i bevelab control states (pauses)" threadname n_resumes
    vprintln 2 majorstat_line
    majorstat_line_log "npc" majorstat_line

#if SPARE
        // Consider this code going fwd ...
        let _ = // Write resource use to main report file.
            let f2 = yout_open_report "bevelab"
            let ff2 str = yout f2 str
            report_resource_use ff2 "" modname sr
            yout_close f2
#endif
    let fsm_info:fsm_info_t =
        {
            inst_set=     None
            inst_rom=     None
            exit_states=  list_subtract(alldests, resumes)
            start_state=  rst
            pc=           pc
            resumes=      resumes
            controllerf=  n_resumes > 1
        }

    let ans = [ (director, SP_fsm(fsm_info, arcs)) ]
    let ans =
        if sS1.recipe_recode_pc then
            let pc_root = mid + "_PC" // Seed for new PC name.
            vprintln 3 (sprintf "bevelab: recode output FSM using PC root name %s" pc_root)
            map (fsm_recode (WN "fsm_recode" ww) pc_root mid sS1.recipe_onehot_pc) ans
        else ans
    ans


//
//  Wrap up the answer in FSM form - which is basically a shannon projection onto the program counter.  (Although it is mostly in that form already!)       
//
let wrap_ans_as_spar ww sS1 (director, (pc, rst)) mid style_get rv =
    let mm = mid
    let vd = !g_fsmvd

    vprintln 3 ("bev elaborated name='" + mm + "' with " + xToStr pc + " as pc")
    // if length (list_once(map fst resumes)) <> length resumes then sf (sprintf "bevelab: hard/soft pause mode counts messed up")
    let n_arcs = length rv    
    //app (arc_rp 0 "L1830") rat_arcs
    vprintln 3 (sprintf "Thread elab '%s' finished: n_arcs=%i" mm n_arcs)

    if n_arcs = 1 && (f5o5(hd rv)) = PS_auto then

        let idss = mid + " " + xToStr pc
        vprintln 2 (idss + ": Thread contains just a single autobarrier only - make RTL block instead of FSM")

        let ((pc, pv), gg, dest, cmds, style) = hd rv
        if xi_isfalse gg then
            vprintln 2 (idss + ": Thread contains just a single autobarrier whose guard never holds.")
            []
        elif nullp cmds then
            vprintln 2 (idss + ": Thread contains just a single autobarrier and no executable RTL.")
            []
        else
            let a0 = if xi_istrue gg then cmds else map (augment_xrtl_guard "bevelab-L2222" gg)  cmds
            dev_println ("Some hangmodes only. Consider dest too")
            let ii = { id="from_bevelab " + idss  }:rtl_ctrl_t
            [ (director, SP_rtl(ii, a0)) ]
            
    else wrap_ans_as_fsm ww sS1 (director, (pc, rst)) mid style_get rv

// bev_compile_exec_1 : the wrap function : keeps the arcs of one machine in lockstep.
//   1. compile bev to rv form
//   2. wrap the quads as FSM_state form (perhaps make a different type from mainstream xrtl !)
//   3. Rename and re-pack the sequencer states. Conditional on userhs's repack_pc flag (THIS STEP NOT YET PORTED FROM SML? )
// Return info comment is at end of function.
//
let bev_compile_exec_1 ww sS0 m minfo iinfo xx =
    let vd = sS0.vd
    let m_arc_idx_counter = ref 5000
    let next_no () =
        let rr = !m_arc_idx_counter
        mutinc m_arc_idx_counter 1
        rr
    let catenates_performed = false
    let (rv, pcrstl, director, untouched) = bev_compile_exec_2 ww sS0 m minfo iinfo catenates_performed xx

    if nullp rv then
        (None, untouched)
    else

    let er1__ = function
        | (an, l, None, r, vf, cst) -> vprintln 0 ("    " + (annToStr an) + (xToStr l) +  " := " + xToStr r)
        | (an, l, subs, r, vf, cst) -> vprintln 0 ("    " + (annToStr an) + (xToStr l) +  "[///] := " + xToStr r)

    //dev_println (sprintf "pcrstl %A xx=%A" pcrstl xx)
    let _ = cassert(length pcrstl = 1, "pcinfo not 1 - more than one clock ?")

    let style_map =
        let add (c:Map<hexp_t, pause_point_t>) ((pc, pv), g, d, cmds, style) =
            let ov = c.TryFind(pv)
            if ov=Some style || (ov=Some PS_hard) then c
            else c.Add(pv, style)
        List.fold add (Map.empty) rv


    let style_get pv =
        match style_map.TryFind pv with
            | Some PS_hard -> { g_default_visit_budget with hardf=true }
            | _            ->  g_default_visit_budget // Also hard for now! TODO
    // 

    let pcrstl' = map (fun (pc, (a, b)) -> (pc, (a, style_get a))) pcrstl
    let quad_rpt vd ((pc, pv), g, d, cmds, style) = 
        if vd >= 5 then
           (
             vprintln 5 ("Report on a quad :pc=" + xToStr pc + " style=" + psToStr style + "\n    resume point=" + xToStr pv + ",\n    g=" + xbToStr g + ",\n    suspend point=" + (if d=None then "None" else xToStr(valOf d)));
             vprintln 5 "Env:";
             app (fun x -> vprintln 5 ("   " + xrtlToStr x)) (rev cmds); // they are stored backwards
             vprintln 5 "\n";
             ()
           )

    vprintln 3 ("Thread elab finished: no quads=" + i2s(length rv))
    if lprint_pred vd then app (quad_rpt vd) rv

    (Some((hd pcrstl', rv), director, style_get), untouched) 
 



(*
 * XRTL compile hblocks.
 *
 * There are several compilation minor variations here, such as whether to have a controlling sequencer or not and
 * whether the output reflects it as a collated FSM or whether we union over sequencer states.
 *  - controllerf holds if there is more than one pc state for a thread.
 *  - preserve_sequencer holds if we wish to show the pc states in a match statement RTL form (shannon decompose over a PC).
 *
 * The pipelined accelerator mode will need us to avoid sequencers to keep the initiation interval high...x
 *) 
let bev_compile_exec ww sS0 (vlnv) (kK__:bevelab_budgeter_t, minfo, iinfo, mid) arg (nets0, xrtl0, acode0, untouched) =
    let vd = sS0.vd
    let mm = vlnvToStr vlnv
    let ww' = WF 3  "bev_compile_exec" ww mm
    let (rv_o, further_untouched) = bev_compile_exec_1 ww' sS0 mm minfo iinfo arg
    let untouched = further_untouched @ untouched

    let rr = 
        match rv_o with
            | None -> []
            | Some((pcrst, rat_arcs), director, style_get) ->
                let rr = wrap_ans_as_spar ww sS0 (director, pcrst) mid style_get rat_arcs
                rr // [(director, rat_arcs)]

    //let rr= if S1.bisimulate then map (bisimulate (WN "bisimulate" ww)) rr else rr

    //let a0code = map (wrap_ans_as_fsm ww) vliw
    let ww' = WF 3  "bev_compile_exec" ww  ("DONE bev elaborate name='" + mm + "' \n======================================================\n\n\n")

    (nets0, xrtl0, rr @ acode0, untouched)








#if OLDFLATTENER
(* 
 * Function to compile a VM to XRTL form, invoking the main bevelab generator on the global schedule over
 * all of its executable components.
 *
 *
 * Recurses into children as well and composes the results in ??? form.
 *
 * 
 *)

 OLD CODE
 // Somewhat stange behaviour here - we preserve the VM hierarchy but pull out all the nets of vms we compile and pass them up to the top.
 // Similar behaviour occurs for vliw. Hardly any (none?) execs are left where they were.       // Also the top-returned VM is ignored so we separately return the untouched for inclusion below.

let rec opath_bevelab_w ww0 sS0 mm (a0code0, nets9) =
    match mm with

    | (iinfo, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) when iinfo.definitionf ->
        let vlnv = iinfo.vlnv   // or minfo.vlnv perhaps is better
        let ids = vlnvToStr vlnv
        let mid = hptos minfo.name.kind
        let phasename = "opath-bevelab-1"
        establish_log false (phasename + "+"  +  filename_sanitize [ '.'; '_'; '/'; ] ids)
        let ww = WF 2 "opath_bevelab" ww0 ("Start processing for id=" + ids)

        let rec determine_retnet = function
            | [] -> None
            | (_, X_bnet ff)::t when (lookup_net2 ff.n).xnet_io = RETVAL -> Some(X_bnet ff)
            | (_ :: t) -> determine_retnet t

        let m_morenets = ref []
        let fsm_kk =
            {
              sS0.kK with
                morenets=             Some m_morenets
                retnet=               determine_retnet (List.foldBack (db_flatten []) decls [])
            } : bevelab_budgeter_t

        // Main compile step
        let sS1 = { sS0 with kK=fsm_kk }    
        let (nets2, xrtl, a0code0, untouched)  = List.foldBack (bev_compile_exec ww sS1 (vlnv) (fsm_kk, minfo, iinfo, mid)) execs ([], [], a0code0, [])

        vprintln 2 (sprintf "Bevelab: %s: Found %i RTL execs (outside of any dpath). Found %i RTL entries to be passed on untouched."  ids (length xrtl) (length untouched))
        let execs = untouched @ execs
        let ww = WF 2 "opath_bevelab" ww0 ("Exec gen complete for id=" + ids)

        let rec sonfun (sons0, a0code0, nets0) = function // why not a foldBack ?
            | [] -> (rev sons0, a0code0, nets0)
            | h::tt ->
                let ww' = WN "opath_bevelab_son machine" ww
                let (son', untouched_, a0code0, nets') = opath_bevelab_w ww' sS0 h (a0code0, nets0)
                sonfun (son'::sons0, a0code0, nets') tt

        let morenets =
            let cpi = { g_null_db_metainfo with kind= "bevelab-morenets" }
            gec_DB_group(cpi, map db_netwrap_null !m_morenets)

        let nets3 = morenets @ decls @ nets2 @ nets9
        //reportx 3 (ids + " nets3") (x >> xToStr) nets3
        let (sons, a0code0, nets) = sonfun ([], a0code0, nets3) sons 
        // Recurse on children.
   
        let iinfo =
            { iinfo with
                  generated_by= phasename
              //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
            }

        let mc' = (iinfo, Some(HPR_VM2(minfo, [], sons, execs, assertions)))
        let ww = WF 2 "opath_bevelab" ww0 ("New VM created for id=" + ids)
        (mc', untouched, a0code0, nets)


    | (iinfo, _) when not iinfo.definitionf ->
        vprintln 3 (sprintf "Encountered vm2 instance %s.  passing on raw." iinfo.iname)
        (mm, [], a0code0, nets9)        

//
//
let opath_bevelab_process_one_vm ww0 sS1 kK1 mc =
    match mc with
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls_, sons_, execs_, assertions))) ->
        let phasename = "opath-bevelab-2"
        let vm_name = hptos minfo.name.kind
        let ww = WF 1 "Bevelab:" ww0 (sprintf " Start processing VM2 %s" vm_name)
        let (ans_, untouched, a0code, nets) = opath_bevelab_w ww0 sS1 mc ([], []) 

        //let ww = WF 1 "Bevelab:" ww0 (" Bisimulate " + if S1.bisimulate then "Complete" else "Skipped")


        let pcs =
            let m_pcs = ref []
            let rec dog = function
                | SP_fsm(fsm_info, arcs) -> mutaddonce m_pcs fsm_info.pc
                | SP_par(_, lst)         -> app dog lst
                | SP_rtl _               -> ()
                | other ->
                    hpr_yikes(sprintf "bevelab: dog other sp form ignored %A" other)
                    ()
            let mine_pc_name = function
                | (_, splst) -> dog splst
                //| other ->
                    //let mid = vlnvToStr minfo.name
                    //sf (mid + sprintf ": other form in mine_pc_name")
            app mine_pc_name a0code
            !m_pcs
            
        let ww = WF 1 "Bevelab:" ww0 (" Recode " + if sS1.recipe_recode_pc then "Complete" else "Skipped")

        let execs =
            let rec par_elide sofar = function
                | [] ->
                    match sofar with
                        | None -> []
                        | Some(dir0, code0) -> [H2BLK(dir0, code0)]

                | (dir1, code1)::tt ->
                    match sofar with
                        | None -> par_elide (Some(dir1, code1)) tt
                        | Some(dir0, code0) ->
                            if dir0.duid = dir1.duid then par_elide (Some(dir1, gen_SP_lockstep [code0; code1])) tt
                            else H2BLK(dir0, code0) :: par_elide (Some(dir1, code1)) tt
            par_elide None a0code

        let execs = execs @ untouched

        let pcnets =
            let cpi = { g_null_db_metainfo with kind= "bevelab-pc-nets" }
            reportx 3 "PC regs needed " xToStr pcs
            gec_DB_group(cpi, map db_netwrap_null pcs)

        let clks_etc = // Including director nets in the output decls is relatively optional - they are implied in the output VM by the presence of the director. We do normally include the PC of an FSM however.
            let directors =
                let ignore_big_bang cc dir =
                    if memberp g_construction_big_bang (map de_edge dir.clocks) then cc
                    else dir :: cc
                List.fold ignore_big_bang [] (map fst a0code)
            let directors = 
                match list_once directors with // can use duid for identity
                    | [] ->
                        let mid = vm_name
                        vprintln 3 ("Create directorate for mid=" + mid)
                        [{ g_null_directorate with clocks=[E_pos g_clknet]; duid=next_duid() }]
                    | dirs -> dirs
 
            let ng director =
                let clks_etc = get_directorate_nets director
                let cpi = { g_null_db_metainfo with kind= "bevelab-control-nets" }
                gec_DB_group(cpi, map db_netwrap_null clks_etc)
            list_once(list_flatten(map ng directors))

        let decls = nets @ pcnets
        let decls = lst_union clks_etc decls // Repeats now not supressed here. They should be supressed where important, such as C++ and RTL gen.
        let ii = { ii with generated_by=phasename }
        let ans = (ii, Some(HPR_VM2(minfo, decls, [], execs, assertions)))
        let ww = WF 1 "Bevelab:" ww0 (sprintf " Core processing complete for VM2 %s" vm_name)
        ans 
#endif

        
let rec opath_bevelab_process_vm_tree ww0 sS0 kK1 mc =
    match mc with
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
        let vm_name = hptos minfo.name.kind
        let ww = WF 1 "Bevelab:" ww0 (sprintf " Core processing start for VM2 %s" vm_name)
        let phasename = "opath-bevelab-1"
        establish_log false (phasename + "+"  +  filename_sanitize [ '.'; '_'; '/'; ] vm_name)
        let ww = WF 2 "opath_bevelab" ww0 ("Start processing for id=" + vm_name)

        let rec determine_retnet = function
            | [] -> None
            | (_, X_bnet ff)::t when (lookup_net2 ff.n).xnet_io = RETVAL -> Some(X_bnet ff)
            | (_ :: t) -> determine_retnet t
        let iinfo = ii
        let m_morenets = ref []
        let fsm_kk =
            {
              sS0.kK with
                morenets=             Some m_morenets
                retnet=               determine_retnet (List.foldBack (db_flatten []) decls [])
            } : bevelab_budgeter_t

        // Main compile step
        let sS1 = { sS0 with kK=fsm_kk }    
        let vlnv = minfo.name
        let (nets2, xrtl, a0code, untouched)  = List.foldBack (bev_compile_exec ww sS1 (vlnv) (fsm_kk, minfo, iinfo, vm_name)) execs ([], [], [], [])

        if not_nullp xrtl then hpr_yikes (sprintf "ROSIE xrtl ignored %i" (length xrtl))
        
        let new_execs =
            let rec par_elide sofar = function
                | [] ->
                    match sofar with
                        | None -> []
                        | Some(dir0, code0) -> [H2BLK(dir0, code0)]

                | (dir1, code1)::tt ->
                    match sofar with
                        | None -> par_elide (Some(dir1, code1)) tt
                        | Some(dir0, code0) ->
                            if dir0.duid = dir1.duid then par_elide (Some(dir1, gen_SP_lockstep [code0; code1])) tt
                            else H2BLK(dir0, code0) :: par_elide (Some(dir1, code1)) tt
            par_elide None a0code

        vprintln 2 (sprintf "Bevelab: %s: Found %i RTL execs (outside of any dpath). Found %i RTL entries to be passed on untouched."  vm_name (length xrtl) (length untouched))
        let execs = untouched @ new_execs
        let ww = WF 2 "opath_bevelab" ww0 ("Exec gen complete for id=" + vm_name)

        let morenets =
            let cpi = { g_null_db_metainfo with kind= "bevelab-morenets" + vm_name }
            gec_DB_group(cpi, map db_netwrap_null !m_morenets)



        let clks_etc = // Including director nets in the output decls is relatively optional - they are implied in the output VM by the presence of the director. We do normally include the PC of an FSM however.
            let directors =
                let ignore_big_bang cc dir =
                    if memberp g_construction_big_bang (map de_edge dir.clocks) then cc
                    else dir :: cc
                List.fold ignore_big_bang [] (map fst a0code)
            let directors = 
                match list_once directors with // can use duid for identity
                    | [] ->
                        let mid = vm_name
                        vprintln 3 ("Create directorate for mid=" + mid)
                        [{ g_null_directorate with clocks=[E_pos g_clknet]; duid=next_duid() }]
                    | dirs -> dirs
 
            let ng director =
                let clks_etc = get_directorate_nets director
                let cpi = { g_null_db_metainfo with kind= "bevelab-control-nets" }
                gec_DB_group(cpi, map db_netwrap_null clks_etc)
            list_once(list_flatten(map ng directors))

        let pc_decls =
            let pcs =
                let m_pcs = ref []
                let rec dog = function
                    | SP_fsm(fsm_info, arcs) -> mutaddonce m_pcs fsm_info.pc
                    | SP_par(_, lst)         -> app dog lst
                    | SP_rtl _               -> ()
                    | other ->
                        hpr_yikes(sprintf "bevelab: dog other sp form ignored %A" other)
                        ()
                let mine_pc_name = function
                    | (_, splst) -> dog splst
                    //| other ->
                        //let mid = vlnvToStr minfo.name
                        //sf (mid + sprintf ": other form in mine_pc_name")
                app mine_pc_name a0code
                !m_pcs

            let cpi = { g_null_db_metainfo with kind= "bevelab-pc-nets" }
            reportx 3 "PC regs needed " xToStr pcs
            gec_DB_group(cpi, map db_netwrap_null pcs)

        let decls = list_Union [clks_etc; nets2; pc_decls; decls; morenets] // Repeats need not now be spressed here. They should be supressed where important, such as C++ and RTL gen.  But it is neater to do it here.


        //reportx 3 (vm_name + " nets3") (x >> xToStr) nets3


        let ww = WF 1 "Bevelab:" ww0 (sprintf " Core processing complete for VM2 %s" vm_name)
        let sons = map (opath_bevelab_process_vm_tree ww0 sS0 kK1) sons
        let ans = (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions)))

        
        ans 




let opath_bevelab ww op_args vms =
    let c1:control_t = op_args.c3
    let stagename = op_args.stagename
    let disabled = 1= cmdline_flagset_validate stagename ["enable"; "disable" ] 0 c1
    //vprintln 2 (bevelab_banner)
    if disabled
    then
        vprintln 1 "bevelab: Stage is disabled"
        vms
    else                
        let zz = cmdline_flagset_validate "skip-propagate" ["false"; "true" ] 0 c1
        let skip_propagate = zz=1
        let vd = max !g_global_vd (control_get_i c1 "bevelab-loglevel" 3)
        g_fsmvd := vd
        let ubudget = control_get_i c1 "bevelab-ubudget" 10000
        let redirect = 0=cmdline_flagset_validate "bevelab-redirect" ["enable"; "disable" ] 0 c1
        let default_pause_mode = control_get_s stagename c1 "bevelab-default-pause-mode" None
        let revast_enable = (control_get_s stagename c1 "bevelab-revast-enable" None) <> "disable"
        let systolic4 = ""//control_get_s stagename c1 "systolic4" None                
        let cost_queries =
            let report_areaf = false
            { g_null_queries with
                logic_costs = Some(logic_cost_walk_set_gen ww vd "opath_bevelab" report_areaf)
            }

        let kK1 = // repeated code - fix me
                {
                    cost_queries=        cost_queries
                    big_bangp=           false // for now
                    revast_enable=       revast_enable
                    systolic4=           systolic4
                    default_pause_mode=  pmos_lexate default_pause_mode
                    ubudget=             ubudget
                    morenets=            None
                    //reset=               xi_blift(reset()) // TODO get from atts ?
                    retnet=              None // WRONG determine_retnet (ldecls @ gdecls) 
                    redirect=            redirect
                    soft_pause_threshold=control_get_i64 c1 "bevelab-soft-pause-threshold" 1000L
                } : bevelab_budgeter_t


        let sS1 ={ //scheduler=  control_get_s c1 "scheduler" None;
                   vd=                      vd
                   recipe_recode_pc=        control_get_s stagename c1 "bevelab-recode-pc" None = "enable"
                   recipe_onehot_pc=        control_get_s stagename c1 "bevelab-onehot-pc" None = "enable"

                   recipe_ubudget=          ubudget
                   msg=                     op_args.banner_msg

                   //bisimulate=            0=cmdline_flagset_validate "bevelab-bisim-reduction" ["enable"; "disable" ] 1 c1
                   kK=                      kK1

                   render_cfg_dotreport=    control_get_s stagename c1 "bevelab-cfg-dotreport" None <> "disable" //Even though cfg plot is part of opath report we perform sp_catenate locally and get different block reports, so write here please. 
                 }


        let vms =
            let mfun vm = 
                let _ = WF 2 stagename ww "Starting 1749";    
                //let vm = opath_bevelab_process_one_vm (WN "bevelab_process_one_vm" ww) sS1 kK1 vm
                let vm = opath_bevelab_process_vm_tree (WN "bevelab_process_one_vm" ww) sS1 kK1 vm                
                if true || skip_propagate then vm
                else opath_propagate_serf (WN "bevelab_propagate" ww) op_args vm
            map mfun vms
        vms


let install_bevelab() =
    let stagename = "bevelab"
    let bev_argpattern =
        [
          Arg_enum_defaulting(stagename, ["enable"; "disable"; ], "enable", "Disable this stage");
          
//        Arg_enum_defaulting("bevelab-tracing", ["enable"; "disable"; ], "disable", "Initial state for trace enable flag");

          Arg_int_defaulting("bevelab-loglevel", 3, "Trace level: lower is more tracing:  (10 or so is default)");


          Arg_enum_defaulting("bevelab-cfg-dotreport", ["enable"; "disable"; ], "disable", "Write a dot report of the elaborated control flow.");
          
          Arg_enum_defaulting("bevelab-redirect", ["enable"; "disable"; ], "disable", "Redirect goto statements to share Pause calls (and other shareable code).");

//          Arg_enum_defaulting("bevelab-bisim-reduction", ["enable"; "disable"; ], "enable", "Share states that are observably equivalent.");          

          // Seems no longer implemented? Or on always?
          Arg_enum_defaulting("bevelab-autobarrier-namealias", ["enable"; "disable"; ], "enable", "Enable automatic pause insertion to disambiguate name alias undecidable (array index not comparable)");

        //  Arg_int_defaulting("bevelab-autobarrier-loglevel", 1, "Logging level for autobarrier insertion");
          //Arg_enum_defaulting("bevelab-autobarrier-stuckinloop", ["enable"; "disable"; ], "enable", "Enable automatic pause insertion on a loop that will not unwind");
          Arg_enum_defaulting("bevelab-recode-pc", ["enable"; "disable"; ], "enable", "Pack PC encodings rather than retain SP_dic pc values");

          Arg_enum_defaulting("bevelab-detailed-trace", ["enable"; "disable"; ], "disable", "Enable further print statements");

//        Arg_enum_defaulting("generate-nondet-monitors", ["enable"; "disable"; ], "enable", "Enable embedding of runtime monitors for non-deterministic updates");


          Arg_enum_defaulting("bevelab-onehot-pc", ["enable"; "disable"; ], "enable", "Make PC encoding one-hot (unary) if recoded rather than binary");

          Arg_enum_defaulting("bevelab-revast-enable", ["enable"; "disable"], "disable", "Find natural loops in CFG before allocating soft pause points.");

//        Arg_enum_defaulting("systolic4", ["enable"; "disable"], "disable", "Temporary systolic divert mode."); // Work in progress          


          Arg_enum_defaulting("bevelab-default-pause-mode", ["auto"; "hard"; "soft"; "maximal"; "bblock"; "tin" ], "bblock", "Which mode to start a thread, before pause control is set."); // tin=target-initiation-interval
          
//          Arg_enum_defaulting("scheduler", ["oldsched"; "naivenew"; "parallelnew"; "none" ], "oldsched", "Select thread scheduler");
          Arg_int_defaulting("bevelab-soft-pause-threshold", 1000, "Nominal working cost of work per clock cycle");

          Arg_int_defaulting("bevelab-ubudget", 10000, "Steps to unwind before considering unwindable");

        ]

    install_operator (stagename,  "Behavioural code to FSM Generator", opath_bevelab, [], [], bev_argpattern)

    let prop_argpattern =
        [
          //Arg_required("srcfile", -1, "Input file (dll/exe) for kiwic conversion", "");
        ]
    let _ = install_operator ("propagate",  "Inter-VM constant propagate", opath_propagate, [], [], prop_argpattern);

    ()


(* eof *)
