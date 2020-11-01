//
// Kiwi Scientific Acceleration: KiwiC compiler.
//
// intcil.fs - This file interprets CIL instructions and rewrites them in kcode form:. the KiwiC stack removal process.
// 
// All rights reserved. (C) 2007-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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
//
   
module intcil


open Microsoft.FSharp.Collections
open System.Numerics
open System.Collections.Generic

open moscow
open yout
open meox
open hprls_hdr
open abstract_hdr
open abstracte
open kiwi_canned
open cilgram
open asmout
open linepoint_hdr
open kcode
open firstpass
open ksc

let g_kiwi_objectlock = "kiwi_objectlock"  // Pseudo field in every object at offset zero: serves as a mutex.

// let g_intcil_vd = ref 20 // 20 is off, default level.

type space_t = (stringl * typeinfo_t * cil_eval_t) option array
    

type methodbind_t = 
         { 
           argspace : space_t
         }


let saved_first_passes = new Dictionary<stringl, firstpass_t>()

    
type IB_t = (* instruction branch *)
    | IB_None
    | IB_LAB of string * string
    | IB_CLAB of hbexp_t * string * string
    | IB_TAINTFLAG of hbexp_t

type filler2_t = Unused_filler_env



// Sufficient to run one method in its stack frame.

type cil_methodenv_t = firstpass_t * methodbind_t option


let kcode_address_of msg = function
    | other -> sf (msg + sprintf ": kcode_address_of other form: " + ceToStr other)
    
(*
 * Scan to generate an a-list of labels and their basic block addresses and the inverse mapping.
 * _oi maps original CIL offset to block no
 * _is maps block no to label name.
 * _si maps label to block no
 *) 
let scan_sis mm (ia:ia_t) il1 =
    let rec scano cc blk_pc =
        if blk_pc=il1 then cc
        else
            let (_, insl) = ia.[blk_pc]
            //let _ = vprintln 0 (mm + sprintf ": scan_sis mon top %i/%i %A" blk_pc il1 insl)
            let scano_ff (cc, oi, cd, ed) = function
                | CIL_instruct(offset, _, _, Cili_kiwi_trystart(token, catchers, (fno, fcode))) ->
                    (cc, (offset, blk_pc)::oi, cd, (token, (token, catchers, (fno, fcode)))::ed)
                               
                | CIL_instruct(offset, _, _, Cili_lab s) ->
                    //let _ = vprintln 0 (mm + sprintf ": scan_sis mon %i/%i offset=%i" blk_pc il1 offset)
                    ((blk_pc, s)::cc, (offset, blk_pc)::oi, (s, blk_pc)::cd, ed)

                | CIL_instruct(offset, _, _, _) ->
                    //let _ = vprintln 0 (mm + sprintf ": scan_sis mon %i/%i offset=%i" blk_pc il1 offset)
                    (cc, (offset, blk_pc)::oi, cd, ed)

                | CIL_try(None, _, _) -> sf "preflat try remains"
                
                | _ -> (cc, oi, cd, ed)
// Should really check no lables occur after a first real instruction in the list. Hopefully no malformed code is generated, otherwise it needs flattening.
            let cc = List.fold scano_ff cc insl
            scano cc (blk_pc+1) 
    let (labs_is, labs_oi, labs_si, exn_handler_directory) = scano ([], [], [], []) 0
    (labs_is, labs_oi, labs_si, exn_handler_directory)


let rec redirect_ia (pc, ia:ia_t, labs_si, labs_is, il1) =
    if true then () // Disabled, perhaps since only looks at head
    elif pc=il1 then ()
    else muddy "disabled"
#if SPARE
    let (_, ins) = ia.[pc]
    let go1 = a_assoc "redirect: go1: missing lable in labs_si" labs_si
    let go2 = a_assoc "redirect: go2: missing lable in labs_is" labs_is
    let ins' = labscan2a (fun x -> go2(go1(x))) (hd ins)
    //  IS THIS ?FUNCTION OK OR USEFUL?  IT LOOKS ONLY AT hd TODO.
    let _ = if not_nullp ins' then Array.set ia pc (sf "Needed", [gec_ins(snd(hd ins'))])
    redirect_ia(pc+1, ia, labs_si, labs_is, il1)
#endif
    

let g_ce_zero = gec_yb(xmy_num 0, cil_type_n_vt Ciltype_int);

// Null is not the same thing as zero: null is a pointer type.
// This make a difference for overloaded method dispatch.
let g_ce_null = gec_yb(xmy_num 0, g_ctl_object_ref)
     //g_ce_zero //gec_CE_tv(g_ctl_object_ref, g_ce_zero)
    

// We need to find unsafe code where pointers are cast to a different pointer type, since this defeats the wondarray system.
// Sometimes the compiler will include a cast and sometimes it does not, so we need to support both situations.
// An example that is undecorated in the CIL is
//       int p1 = 0x12345678;  byte *src = (byte *)(&p1);
//        .locals (      [0] (valuetype [System]Int32) V_0  [1] (valuetype [System]Byte)* V_1)
//           6  : ldloca  V_0
//           8  : stloc  V_1
    
// Such unsafe pointers casts need to have their associated nemtok marked up and a decision made as to which wondarray will represent it.
// A wondarray of bytes is always the easiest, since unaligned word operations will all be naturally supported.
// The operations on another form that involve pack and unpack to bytes can commonly all disappear in peephole pattern matching where the pack and unpack are composed???
let spot_type_abuse msg indirectf lty rty =
    //let _ = vprintln 3 (msg + sprintf ": spot_type_abuse indirectf=%A dt_lhs=%s dt_rhs=%s " indirectf (cil_typeToStr lty) (cil_typeToStr rty))
    let abuse =
        match (lty, rty) with
            | (CT_star(n_l, ll), CT_star(n_r, rr)) when abs n_l = 1 && abs n_r = 1->
                //let _= dev_println "Spotting unsafe type casts: spaggot for now intcil"
                Some 22
                
            | _ ->
                let _ = vprintln 3 (msg + sprintf ": spot_type_abuse: indirectf=%A ignored pair dt_lhs=%s dt_rhs=%s " indirectf (cil_typeToStr lty) (cil_typeToStr rty))
                None
    abuse

let lower_type ww msg dtx = 
    let s = ct_signed ww dtx
    let bits = int32(ct_bitstar ww msg 0 dtx)
    //let _ = vprintln 0 (msg + sprintf ":   do_CE_conv lower type - cast bits=%i s=%A for dt=%s" bits s (cil_typeToStr dtx))
    { g_default_prec with widtho=Some bits; signed=s; }


// Generate a convert/cast operation - folding on constants for constant result.
let gec_CE_conv ww msg newprec cvtf rhs =
    let msg = "gec_CE_conv"
    let vd = !g_gtrace_vd
    let vdb = vd >= 4
    let newprec = follow_knot msg newprec
    let _ = if vdb then vprintln vd (sprintf "Applying S2 gec_CE_conv ce=%s to type=%s" (ceToStr rhs) (cil_typeToStr(get_ce_type msg rhs)))
    let _ = if vdb then vprintln vd (sprintf "         S2 gec_CE_conv to newprec=%s " (cil_typeToStr newprec))
    let resolved_region_dt existing_region_dt =
        let ans = 
            match (newprec, existing_region_dt) with
                | (CT_star(1, CTL_record(record_idl, cst_, lst, len, ats, binds, srco)), (CT_arr(CT_class cst, None))) when is_squirrel_base cst.name record_idl -> Some newprec

                | (CT_star(nn, CTL_record(record_idl, cst_, lst, len, ats, binds, srco)), (CT_star(mm, CTL_record(record_idl', _, _, _, _, _, _)))) when record_idl = record_idl' -> Some newprec

                | _ -> None
        let _ = if vdb then vprintln vd (sprintf "newprec resolved_region_dt of\n newprec=%s\n  existing_region_dt=%s\n   ans=%s" (cil_typeToStr newprec) (cil_typeToStr existing_region_dt) (if nonep ans then "None" else cil_typeToStr (valOf ans)))
        ans

    let tyeq msg a b =
        //let _ = vprintln 0 (sprintf "tyeq start")
        let mf = (fun ()->msg)
        let ans = generic_tyeq vd mf a b
        //let _ = vprintln vd (sprintf "msg=%s: tyeq done %s cf %s ans=%A" msg (cil_typeToStr a) (cil_typeToStr b) ans)
        ans

    // We can encounted up and down casts as well as valuetype converts and converts from abstract class references to concrete CTL_records.  Generally we want to avoid a convert away from a CTL_record unless an up/down cast since this will discard detail.
    let ans = 
      match rhs with
        | CE_conv(oldprec, cvtf1, ce') when  tyeq "conv telescope check" newprec oldprec -> // Telescope identical converts: Do not doubly apply that might remove all converts.
            if cvtf=cvtf1 then rhs
            else CE_conv(newprec, cvt_resolve cvtf cvtf1, ce')

        //| CE_conv(oldprec, cvtf1, ce') ->
        //    //let rp = precision_compose (*prec_left=*) true  l_newtype l_dt1
        //    CE_conv(newprec, cvt_resolve cvtf cvtf1, rhs) // 

        | CE_star(1, CE_struct(idtoken, dt, aid_, _), _) when tyeq "conv struct/record address" newprec (CT_star(1, dt)) ->
            let _ = vprintln 3 ("supressed gec_CE_conv of struct ref to self/similar")
            rhs //   Drop the convert of struct references when of that type already.

        | CE_region(uid, dt1, ats, nemtok, _) when tyeq "conv region" newprec dt1 -> rhs //   Drop the convert of regions when of that type already.

        | CE_region(uid, existing_region_dt, ats, nemtok, x) when resolved_region_dt existing_region_dt <> None -> // Apply the convert to a region if becoming more specific.
            CE_region(uid, valOf(resolved_region_dt existing_region_dt), ats, nemtok, x)

        | CE_x(orig_dt, hexp) ->
            let _ = if tyeq "conv CE_x" newprec orig_dt then vprintln 3 (sprintf "We do not skip conv to self CS=%A. For dt=%s" cvtf (typeinfoToStr newprec))
//          let enumf = dt_is_enum newprec
            let (l_newtype, l_orig_dt) = (lower_type ww msg newprec, lower_type ww msg orig_dt)
            let _ = if vdb then vprintln 0 (sprintf "   X1 gec_CE_conv 2: ce=%s" (ceToStr rhs))    
            let _ = if vdb then vprintln 0 (sprintf "Applying gec_CE_conv 4: to newprec=%s " (cil_typeToStr newprec))
            if l_newtype=l_orig_dt then
                let _ = if vdb then vprintln vd (sprintf "   X2 gec_CE_conv 5 nop on %s" (prec2str l_newtype))
                CE_x(newprec, hexp)
            else
                let ans = ix_casting l_orig_dt.widtho l_newtype cvtf hexp
                let _ = if vdb then vprintln vd (sprintf "    X3 gec_CE_conv to newprec=%s - to CE_x(%s, %s): composedcast=%s giving %s" (cil_typeToStr newprec) (cil_typeToStr orig_dt) (xToStr hexp) (prec2str l_newtype) (xToStr ans))
                CE_x(newprec, ans)
            
        | other -> CE_conv(newprec, cvtf, rhs) // Backstop leaves it symbolic for lower to process.
    let _ = if vdb then vprintln 0 (sprintf "Applied (L274) gec_CE_conv with newprec=%s finally gave %s\n" (cil_typeToStr newprec) (ceToStr ans))
    ans

let template_eval ww gbb dt = 
    if trivially_grounded_leaf dt then dt
    else
        //let _ = vprintln 0 ("Template eval of " + typeinfoToStr dt)
        let ww = (WN "template_eval" ww)
        let ans = cil_type_n ww gbb dt
        //let _ = vprintln 0 (sprintf "invoke template eval of %s with bindings=%A giving %s" (cil_typeToStr dt) gbb (cil_typeToStr ans))
        //let _ = vprintln 0 (sprintf "Finished template eval of %s giving %s" (cil_typeToStr dt) (cil_typeToStr ans))        
        ans



//   
let rec cil_oncesplitted_and_int_bind_cgr ww gb cr0 = // DEPRECATED ? use quite a lot still !  Now a NOP ! - - ni like as part for fr3 now - but for others please split in norm phase now - which is earlier than bind. We may need this back for generics in ldtoken etc.
    let ww = (WN "cil_oncesplitted_and_int_bind_cgr" ww)
    match cr0 with
    | Cil_cr_templater(cr, args) ->  // This match is now mostly/entirely replace with the one in cil_type_n_gb
        let ct__ = cil_type_n ww gb cr0 // Pass all to type_n - it will double up on fap processing a little. - Side effecting only?
        //vprintln 0 ("want to and_int_bind " + typeinfoToStr ct__)
        (args, cr)
    | cr -> ([], cr)


(*
 * fr3 'binds' in the sense that formal names are inserted where implied and actuals
 * are looked up in the current cgr0 (and mgr in future?) context as needed.
 * For static fields include the whole of cr in with the final path name. Now always include?
 *)
let rec fr33 ww fullpathname (cgr:annotation_map_t) (cr, id, gargs_) =
    let tycontrol  = { grounded=false; } // these looked up on first pass only - second pass should use droppings
    let (fap_, cr1) = cil_oncesplitted_and_int_bind_cgr ww (tycontrol, cgr) cr (* find and bind ? some user want fr2 I think  *)
    //let _ = vprintln 0 (sprintf "fr33 gargs=%A " gargs)
    let idl = if fullpathname then cil_classrefToIdl ww cgr cr1 else [] 
    (Some cr, id :: idl)
  
and fr3_fullpathname ww cgr fr = fr33 ww true cgr fr  (* set fullpathname for conservative behaviour *)


and cil_type_n_gb ww realdo_ tcn arg = 
    let ww = WN "cil_type_n_gb" ww

    let lift_gformals ww tcn lst =
        let fp_lift = function
            | Cil_tp_arg(methodf, idx, id) ->
                let _ = vprintln 3 (sprintf "fp_lift idx=%i id=%s" idx id)
                CTVar{ g_def_CTVar with idx=Some idx; id=Some id; methodf=methodf }
            | Cil_tp_type ctype -> cil_type_n (WN "type_n_gb" ww) tcn ctype
        map fp_lift lst

    //let _ = vprintln 0 (sprintf "n_gb %A" arg)
    let fap =
        match arg with
            | Cil_cr_templater(cr, fap) -> lift_gformals ww tcn fap
            | _ -> []
    let ans = template_eval ww tcn arg
    (ans, map (gec_RN_monoty ww) fap)




    
let gec_CE_alu ww (ct0, dop, lhs, rhs) =
    let (aidl, ct, lhs, rhs) = 
        if dop <> V_plus && dop <> V_minus then (None, ct0, lhs, rhs)
        else
            let msg = "gec_CE_alu: POINTER ARITH"
            // If we are doing add/sub and one just one arg is a pointer we return the aid of that arg, assuming pointer arithmetic.
            let lt = get_ce_type "pointer-l" lhs // We steer our initial search using types, but this will not work for heavily casted
            let rt = get_ce_type "pointer-r" rhs // cases in which case a deeper dive is needed - ultimately one arg will serve.
            let rec scorer = function
                | CT_star(sc, b) -> (abs sc) * 1000 + scorer b
                | CTL_net(volf, width, signed, flags) -> if not_nonep flags.nat_IntPtr then 1 else 0
                | _ -> 0
            let mf () = msg + sprintf ": RT=%A \n L=%s %A R=%s %A" rt (cil_typeToStr lt) (ceToStr lhs) (cil_typeToStr rt) (ceToStr rhs)
            // Do not need CE_handleArith now this code is here.
            let (ls, rs) = (scorer lt, scorer rt)
            let (aidl, ct99, indexed_ty, lrbar) = 
                match (ls, rs) with
                    | (0, 0) -> // straightforward arithmetic
                        (None, ct0, None, false) 
                    | (ls, rs) when ls > rs -> (ce_aid0 mf lhs, gec_CT_IntPtr lt, Some lt, false)
                    | (ls, rs) when ls < rs -> (ce_aid0 mf rhs, gec_CT_IntPtr rt, Some rt, true)

                    | (ls, rs) when ls + rs > 0 ->
                        // We dont know the pointer size to multiply by - this will fail unless happens to cancel out on both loads and stores and does not alias anything else!
                        let _ = vprintln 0 (sprintf "gec_CE_alu: +++ POINTER ARITH RESOLUTION FAILED - DIVE BOTH SIDES \n    ls=%i L=%s LT=%s\n    rs=%i R=%s    RT=%s" ls (cil_typeToStr lt) (ceToStr lhs) rs (cil_typeToStr rt) (ceToStr rhs))
                        let l_aidl = ce_aid0 mf lhs
                        let r_aidl = ce_aid0 mf rhs                    
                        (muddy "l_aidl @ r_aidl", g_canned_IntPtr, None, false) // Wrong to return no scale-by type.
            let scaler =
                let mx = "pointer arithmetic scale factor"
                let (scaler, my) =
                    match indexed_ty with
                        | None -> (1L, "no-ptr-dt")
                        | Some (CT_star(n, CT_arr(ct, _))) when abs n=1 -> // TODO don't need to have both these forms supported.
                            (ct_bytes1 ww mx ct, cil_typeToStr(valOf indexed_ty))
                        | Some (CT_arr(ct, _)) ->
                            (ct_bytes1 ww mx ct, cil_typeToStr(valOf indexed_ty))
                        | Some dt ->                        
                            let cdt = remove_CT_star msg dt
                            let vt = ct_is_valuetype cdt
                            let s = if vt then ct_bytes1 ww mx cdt else int64 g_pointer_size
                            (s, cil_typeToStr(valOf indexed_ty))

                vprintln 3 (msg + sprintf ": scaler=%i for an array containing %s. " scaler my)
                let _ = cassert(scaler <> 0L, "scalar <> 0")
                scaler

            vprintln 3 (sprintf "gec_CE_alu ls=%i rs=%i return %s instead of the perhaps different %s.  lhs=%s scaler=%i" ls rs (cil_typeToStr ct99) (cil_typeToStr ct0) (ceToStr lhs) scaler)
            let ce_times l r = CE_alu(g_pointer_type, V_times, l, r, None) 
            let lhs = if scaler > 1L && lrbar then ce_times lhs (ce_int64 scaler) else lhs
            let rhs = if scaler > 1L && not lrbar then ce_times rhs (ce_int64 scaler) else rhs
            (aidl, ct99, lhs, rhs)
    CE_alu(ct, dop, lhs, rhs, aidl)
    
// Use bitwise AND as a mask - a bit brutal ...
let ce_mask ww (width) nvn =
    let xmask = himask width
    let rt = CTL_net(false, width, Unsigned, g_native_net_at_flags)
    let mask = gec_yb(xi_bnum_n width xmask, rt)
    gec_CE_alu ww (rt, V_bitand, nvn, mask) 


let gec_unpacking_fun ww msg dt_struct skip_ptag =   // unpack all fields of a binary packed struct except perhaps one.
    let vecdata = 
        match destar dt_struct with
            | CTL_record(idl, cst, lst, len, ats, binds, _) ->
                let scant crf cc = (crf.pos*8L, ct_bitstar ww "all-field-unpacking-site" 0 crf.dt, crf.dt, crf.ptag)::cc
                List.foldBack scant lst []
            | other -> sf (sprintf "expected CTL_struct (CTL_record) for struct all-field-unpacking instead of %s" (cil_typeToStr other))
    //let master_dt = { widtho=Some(int(dt_width_bits dt_struct)); signed=Unsigned }
    let master_dt = CTL_net(false, int(dt_width_bits ww dt_struct), Unsigned, g_null_net_at_flags)
    let unpacker packed_data = 
        let bic (bit_offset, width, fdt, ptag) cc =
            let nv =
                if ptag=skip_ptag then None
                else
                    let nvn = CE_alu(master_dt, V_rshift Unsigned, packed_data, gen_ce_int64(bit_offset), None) // Field extract from struct.
                    let nvn = ce_mask ww (int width) nvn
                    let nvn = gec_CE_conv ww "lower: lhs-of-assign" fdt CS_maskcast nvn 

                    Some(gec_CE_conv ww msg fdt CS_maskcast nvn)
            (ptag, fdt, nv) :: cc
        //let master_width = ((xi_bnum_n (int32 master_width) 0I)
        List.foldBack bic vecdata []
    unpacker


let convert_to_uint x = x // for now




// Returns the HwWidth attribute which needs be used because netgen is now much later.
// Returns the storage width attribute if not same as custom width.
//
let non_native_width ww (dt, idl) = function
    | None -> []
    | Some(w) -> 
        let rec k = function
             | CTL_net(volf, width, signed, flags) -> Some width
             | CT_star(n, a) -> k a 
             | other -> None
        let w1 = k dt
        let ans = w1 <> None && valOf w1 <> w
        let _ = if ans then vprintln 3 (hptos idl + " has native width " + i2s(valOf w1) + " and custom width HwWidth " + i2s w)
        let hw = ("HwWidth", i2s(w))
        hw :: (if ans then [ ("storage", i2s(valOf w1)) ] else [])





//
// The code in field_def is needed elsewhere, e.g. for dyna's and  for cili_ldsfld so I copy it out
// in field_type_resolve: nicer/should call that copy from here
//
// This copy handles the cust attributes from a No_field definition whereas field_type_resolve operates on an idl useage.
//
//
// The width opt may disagree with the ctype native width at this point, but we flag
// an error later if this item is ever the target for width-changing arithmetic.
//
// 
//
let rec cil_svar_def ww cCBo m0 atakenf realdo llbf idl dt wopt iot wond ats io_group_opt envo fd_str_o =
    let vd = 4
    let sheap_id = hptos idl
    let msg = " static variable declare " + sheap_id
    vprintln 3 (sprintf "cil_svar_def: %s  wopt=%A iot=%A" msg wopt iot)
    let m0() = m0() + msg
    let dims = -1
    let io_atts =
        match iot with
            | INPUT   -> [("io_input", "true")]
            | RETVAL  -> [("io_input", "true")]            
            | OUTPUT  -> [("io_output", "true")]
            | _ -> []
    let cgr = Map.empty
    // hmm this is ignoring wopt now...  let net = netgen ww "cil_svar_def" (name, dt, wopt, [], ats)
    // But custom width args still work, as in  test9 that now has a 9-bit bus with parity.       
    let ats = lst_union io_atts (ats @ non_native_width ww (dt, idl) wopt)
    let (blobf, cez, vrnl_) = gec_singleton ww vd cCBo msg "cil_svar_def" ats realdo m0 "" idl dt io_group_opt atakenf fd_str_o
    (blobf, cez, envo)


// static_field_def
// got both uid0(aka idl) and F ... nasty 
//
// This returns ((bool, ans) option, env) where a1 is the normal answer, but if allow_indexable_literalf is set, the bool may hold indicating a labelled blob (perhaps a readonly or string literal) was found instead of a static variable.
//    
and static_field_def ww cCBo allow_indexable_literalf realdo m0 prefix mf cgr (envo:'a option) field =
    let cCC = valOf !g_CCobject
    let gb0 = cgr
    match field with
    | No_field(layout, flags, name, fdt, atclause, cargs) ->
        let idl = name :: prefix
        let prefix' = idl  // prefix' needed for structures with further fields inside them.
        let mm = ("static_field_def: " + hptos idl + " field definition " + noitemToStr true field)
        let msg = mf() + " " + mm
        let ww = WF 3 "static_field_def: " ww msg
        let dt = fdt.ct
        let bytes = ct_bytes1 ww msg dt
        let sheap_id = hptos idl
        let vtf = ct_is_valuetype fdt.ct
        let storemode = if vtf then STOREMODE_singleton_vector else STOREMODE_singleton_scalar
        let types = typeinfoToStr dt
        let alloc_ats = []
        let (hidx, envo) = obj_alloc_serf ww m0 types false storemode (Some sheap_id) envo (idl, alloc_ats, 0L)   // 1/2 - this should return correct base address in uid but not do the actual definition when on the heap.
        let uid = { f_name=idl; f_dtidl=ct2idl ww dt; baser=hidx; length=Some bytes; } 
        let ids = uidToStr uid
        vprintln 3 (sprintf "static_field_def: normed type is %s bytes=%i vtf=%A for idl=%s " types bytes vtf ids)

        (* Old way (framestore wide outputs: wide lost_ios2) Useful for bit-encoded outputs.  *)
        let oldportdesignation = false// CB.codefix <> [] && old_port_pred ats (hd(CB.codefix))
        let custom_fldatts (r0:(string * string) list) sp = 
            let dn = function
                | (_, X_num n) -> n
                | (fname, other) -> sf (sprintf "custom field attribute: bad format int expected for %s" fname)
            let ds = function
                | (_, W_string(ss, _, _)) -> ss
                | (fname, other) -> sf (sprintf "custom field attribute: bad format s expected string for %s not  " fname + xToStr other)

            let arrayat(key, args1, defv) = 
                let number = dn(select_nth 0 args1) // may be missing for auto
                [ (key, i2s number) ]

            let offchip(args1) =
                //  OutboardArray with a single arg of "-onchip-" denotes an onchip override regardless of size.
                if length args1 >= 1 && ds (hd args1) = "-onchip-" then  [ ("onchip", "true") ]
                else
                    let portname = ds(select_nth_or 0 args1 ("portname", xi_string "")) // may be missing for main DRAM default
                    let offset   = dn(select_nth_or 1 args1 ("offset", xi_num -1))    // may be missing for auto
                    let _ =  vprintln 3 ("Outboard/offchip var portname=" + portname + " offset=0x" + i2x offset  + " for " + ids)
                    [ ("offchip", "true") ] @ (if portname<>"" then [("portname", portname)] else []) @ (if offset >=0 then [ ("offset", i2s offset) ] else [])

            //let RomArray(args1) = [ ("RomArray", "true") ]
            //let subsume(args1) = [ ("forced_subsume", "true") ]
            //let elaborate(args1) = [ ("forced_elaborate", "true") ]

            let io_input(args1) = [ ("io_input", "true") ]
            let io_output(args1) = [ ("io_output", "true") ]
            let rtl_parameter = function
                | [(_, W_string(name, _, _))] -> [ (g_username, name);  ("init", "0"); ("rtl_parameter", "true") ] 
                | [(_, W_string(name, _, _)); (_, n)] when constantp n  ->
                      [ (g_username, name); ("rtl_parameter", "true"); ("init", xToStr n) ]
                | other ->
                    hpr_yikes("malformed RtlParameter attribute args")
                    [ ("rtl_parameter", "malformed") ]            // This is collaged to DB_form_pramor form at end of cilnorm.


            let rec scan rr (s:string, args1) = 
                let p k = isPrefix k s
                if (p "OutboardArray") then offchip(args1) @ rr
                elif (p "ThreadsPerPort") then arrayat("ThreadsPerPort", args1, 1) @ rr
                elif (p "PortsPerThread") then arrayat("PortsPerThread", args1, 1) @ rr
                elif (p "SynchSRAM") then arrayat("CombSRAM", args1, 1) @ rr
                elif (p "CombSRAM") then arrayat("SynchSRAM", [("latency", xi_num 0)], 1) @ rr                                 
                //elif (p "RomArray") then RomArray(args1) @ rr
                //elif (p "Elaborate") then elaborate(args1) @ rr // Kiwi.Elaborate() is no longer used. To be deleted
                elif (p "Input") then lst_union (io_input args1) rr
                elif (p "Output") then lst_union (io_output args1) rr
                elif (p "RtlParameter") then lst_union (rtl_parameter args1) rr
                elif (p "MultiPortRegfile") then ("MultiPortRegfile", "2")::rr // Dual port only for now.
                else rr
            scan r0 sp

        (* This is both here and in the fldats - should compute this from fldats in future *)
        let custom_io cc (ss, args1) = 
            //let _ = dev_println (sprintf " Checking I/O string >%s<" ss)
            let p k = isPrefix k ss
            if p "Input" then INPUT elif p "Output" then OUTPUT else cc
        let m_hintname = ref None

        // A 'username' is the name of a net manually set by the user using a C# attribute or otherwise.
        // A g_logical_name is a token from a protocol definition that designates one net or vector within the net-level port that carries the protocol.
        let custom_username cc (s, args1) = 
            let p k = isPrefix k s
            let rec jname = function
                | ((_, W_string(v, _, _)) :: _) when strlen v > 0 ->
                    let _ = m_hintname := Some v
                    (g_username, v) :: cc
                | _::tt ->  jname tt
                | _     -> cc
            if p "Input" ||  p "Output" then jname args1 else cc

        let custom_width r0 (s, args1) =
                if isPrefix "HwWidth" s
                then
                    let w = xdeval "custom width" (snd (select_nth 0 args1))
                    let _ = vprintln 3 ("Selected custom width " + i2s w)
                    if w=0 then None else Some w // Supress bad blob args for now
                elif length args1 >= 2 &&
                             (isPrefix "OutputWord" s ||
                              isPrefix "InputWord" s
                             )
                    then
                    let h = xdeval "Word Range" (snd (select_nth 0 args1))
                    let l = xdeval "Word Range" (snd (select_nth 1 args1))
                    let _ = if l<>0 then vprintln 0 (ids + " lower label non zero: both labels moved down")
                    let _ = vprintln 3 ("Selected custom width bit range lables " + i2s h + ":" + i2s l)
                    let _ = if h=l then vprintln 0 (ids + "+++ attribute HERE not used correctly h=l")
                    let w = abs(h-l)+1
                    if (h=0&&l=0) then None else Some w // Supress bad blob args for now
                else r0

        //let _ = dev_println (sprintf "attribute tags (cargs) for %s are " ids + sfold (fun (ss, x) -> ss + ":" + sfold (snd>>xToStr) x) cargs)
        let wopt = List.fold custom_width None cargs
        let tats = List.fold custom_username [] cargs
        let tats = List.fold custom_fldatts tats cargs
        let io_info = if oldportdesignation then OUTPUT else List.fold custom_io LOCAL cargs

        vprintln 3 (sprintf "I/O designation for %s is " ids + varmodeToStr io_info)
#if PRE_CECIL
        // This init code was for input via monodis, but now cecil collects the information for us.
        let get_init = function
                | Cil_field_at(Cil_id data_id) ->
                    let ov = op_assoc [data_id] cCC.dir
                    let _ = vprintln 3 ("At clause for " + ids + " returns " + (if ov=None then "None" else cil_classitemToStr "" (valOf ov) ""))
                    Some(CE_init(valOf ov))
                | Cil_field_lit(Cil_id ss) -> Some(gec_ce_string ss)
                | Cil_field_lit e -> Some(cilToCe (*was CCL but want cgr probably*)[] e)
                | Cil_field_none -> None
                | (_) -> sf("cil: Invalid at clause " + ids)

        let ginit = get_init atclause
#endif
        // Second call : 2/2 - it is not documented why we use this two-stage process? Could be before CT_knot was introduced. Probably not needed these days. Only the obj_alloc called twice?
        // The second call also has reallyf=false for a (singleton) struct since the individual fields will be allocated during sconing,
        let (_, envo) =
            let reallyf = not vtf
            obj_alloc_serf ww m0 types reallyf storemode (Some sheap_id) envo (idl, alloc_ats, bytes)  // Second call : 2/2

        let labelled_literal_blob = // GCC4CIL labelled literals - e.g. string constants
            match atclause with
                | Cil_field_none ->
                    let _ = vprintln 3 ("No data blob for field " + mm)
                    None
                | Cil_field_lit(Cil_blob data) ->
                    let tochar = function
                        | Cil_number32 n -> char n
                        | _ -> '.'
                    let ss = implode(map tochar data)
                    Some (ss)
                | _ -> None
        let llbf = allow_indexable_literalf && not_nonep labelled_literal_blob

        let (blobf, cez, envo) = cil_svar_def ww cCBo m0 None realdo llbf idl dt wopt io_info (Some uid) tats None envo (Some fdt) //This returns (blobf:bool, ce, _) so we can handle indexed literals (found in gcc4cil strings).
        if allow_indexable_literalf then // GCC4CIL NEW WAY
            let ans = 
                if not_nonep labelled_literal_blob then
                    let _ = vprintln 3 (sprintf "%s labelled blob makes string literal of %i bytes. %A" sheap_id bytes labelled_literal_blob)
                    gec_ce_string(valOf labelled_literal_blob) // TODO assumes only strings are such ... and ...
                else
                    let _ = dev_println "rezzed a CE_ilit but we dont like these really ... "
                    CE_ilit(cez, gen_ce_int64 0L)

            // Dont add this to the sheap for now ... it may never need looking up or the dp/cache may be too nailed ?
            //let _ =  cCC.heap.sheap.add sheap_id ((true, ans), ref None) // TODO dodgey cached/dp hexp field applies to all index values?
            //We do need to create a memdesc
            let ats_ = []
            let _ = record_memdesc_item sheap_id "labelled-string-literal" !m_hintname tats uid ans dt 
            (Some(true, ans), envo)
        else
            let ww = WF 3 "static_field_def: " ww (msg + " FINISHED")
            ((if vtf then Some(blobf, cez) else None), envo) // Static instance handles do not need to be returned.

    | No_knot(CT_knot(idl, whom, cto), nvo) ->
        if not_nonep !nvo then static_field_def ww cCBo allow_indexable_literalf realdo m0 prefix mf cgr envo (valOf !nvo)
        else sf (whom + " " + hptos idl + sprintf ": field_def knot untied") 

    | No_method (_) -> (None, envo)
    | other -> sf ("other form in static_field_def " + noitemToStr true other)




let cilgen_sex msg = function // Sign extend.
    | (v, CTL_net(volf, width, signed, flogs), width') ->
        //vprintln 4 (sprintf "%A -> %A:  sign extending %i to %i" signed signed width width')
        if width=width' then v
        else sf "cilgen sign extend 1"
    | (_) -> sf ("cilgen sign extend 2")


// Singleton vars - not found in arrays or non-static fields. Singletons which are structs are sconed out.
let singleton_locate ww cCB allow_indexable_literalf realdo m0 cgr (envo:'a option) idl =
    let cCC = valOf !g_CCobject
    let sheap_id = hptos idl
    let r0 = cCC.heap.sheap1.lookup sheap_id
    let on_demand() = // Define on demand if not found so far.
        let mf() = "on_demand static singleton_locate sheap_id=" + sheap_id
        let ww' = WN (mf()) ww
        let bindings = Map.empty
        let (r, nn_) = valOf_or_fail "L653" (normed_ast_item_lookup_wb_wfail ww' true bindings idl)
        let nn = length r
        vprintln 3 (mf() + sprintf " : %i suitable items found for on_demand static field definition." nn)
        let (_, envo) =
            if nn=1
            then static_field_def ww' (Some cCB) allow_indexable_literalf realdo m0 (tl idl) mf cgr envo (hd r)
            else (None, envo)
        let r1 = cCC.heap.sheap1.lookup sheap_id
        if nonep r1 then // Should now exist.
            vprintln 0 (sprintf "On demand generate of " + sheap_id + " failed: Please mark up the static class for conversion to hardware")
            reportx 0 "on demand static items found" (noitemToStr_untie true) r                    
            cassert(r1 <> None, "count=" + i2s nn + ": singleton_locate failed to establish definition of field " + sheap_id)
        let _ = unwhere ww
        r1

    let r1 = if nonep r0 then on_demand() else r0
    r1


    

(* Oldstyle - HSIMPLE RPC invocation.
 * Default approach: We generate one interface per remotely-called method, even if there are several such methods on a single remote object. See user manual for writing a monomethod shim to adapt other styles to this.
 *
 * The args and return value to an RPC stub method become external terminals to the HPR machine when externally-instantiated.
 * The method has a name. The busabstraction has a name.  The protocol has a name and associated handshake nets. For SoC Render to automate the interconnection we need to give a group name.
 * Sometimes we will want to override the defaults using C# attributes. 
 * 
 * TODO: process the [out] modifiers.
 *
 *
 *)
let squirrel_port_signature signat =
    let arity = length signat
    let rec spx = function
        | CTL_net(volf, width, signed, flags)::tt ->
            let ss = sprintf "%c%i" (if signed=Signed then 'S' elif signed=FloatingPoint then 'F' else 'U') width
            if nullp tt then ss
            else ss + "_" + spx tt
        | _::tt -> "X_" + spx tt
        | [] -> ""
    sprintf "SQ%i_%s" arity (if nullp signat then "UNIT" else spx signat)


//
// Create the contacts for inter-compilation method call.
// Originally there were three possibilities:
//      current:  They are the formal contacts/signature for the current compilation unit.
//      child:    They are local nets that connect to an internal instance of the peer.
//      external: They are additional formals for the current component, with reverse directions, that will connect to the peer via a parent.    
//
// Note that external instantiation makes no difference to kiwife behaviour - it generates an internal instance always and verilog_gen externalises it based on a passed down attribute.
//
// Note also that restructure also adds child components, such as ALUs, RAMs and memory busses,  but these are not seen by kiwife.
//
// Finally note, that RPC calls to child machines are now left as function applications in the hexp_t code generated by kiwife and these are
// then filled in as structural components by restructure, as per the ALUs and so on.
//    
let generate_rpc_cs_terms ww cCB kxr realdo cpio (idl0, signat, rtype, overload_suffix) client_direction fsems =
    let cCC = valOf !g_CCobject
    let _ = do_not_abbreviate idl0

    // We can probably always do this in here rather than in the caller.
    // let _ = if fsems.fs_overload_disam then "_" + squirrel_port_signature (map f3o4 signat) else ""

    let add_inhold = not fsems.fs_inhold // If the caller does not hold the inputs we do ... confused need reverse polarity from master v slave TODO.
    let m_assigns = ref []
    let bus_definition_name = hptos idl0 + overload_suffix
    let msg = bus_definition_name + " RPC_cs_terms being made for " + hptos idl0 + " bus_definition_name=" + bus_definition_name + " clientdir=" + boolToStr client_direction + " overload_suffix=" + (if overload_suffix = "" then "<none>" else overload_suffix)
    let ww = WF 3 "generate_rpc_cs_terms" ww msg
    let m0() = msg
    let _ = vprintln 3 msg
    let pi_key = hptos idl0 + overload_suffix
    let iogo = if nonep cpio then Some { g_null_db_metainfo with pi_name=pi_key; kind=bus_definition_name; not_to_be_trimmed=true } else cpio
    let parameters = [] // aka portmeta

    let inholder_suffix = if add_inhold then Some (funique "_inholder") else None // Create a net with internal name _iterm (that needs to be read in via t-latch bypass style) to hold input value under that style of operation. We call funique owing to multiple methods/overloads having the same formal name but could better key off the method uid.


    let genio idx tats id dt io_info suffixo =
        let msg = ("genio rpc: Start define arg id=" + id + " " + typeinfoToStr dt)
        let _ = vprintln 3 msg
        let m0() = m0() + msg
        let staticf = true

        let (idx, (idl, dt, ce), xnet, xnet_external) =
            let io_att = [((if io_info = INPUT then "io_input" else "io_output"), "true")]
            //let username__ = hd idl0 + "_" + apply_sq_overload id
            let tats1 = io_att @ [ (g_logical_name, id); (g_port_instance_name, pi_key) ] @ tats
            let sql = if overload_suffix = "" then [] else [ overload_suffix ]
            match suffixo with
                | Some suffix -> // If need inholder
                    // Declarare the internal svar as local and with suffix, leaving original name as the exposed formal for caller's associative instantiation.

                    let internal_idl = (id+suffix)  :: sql @ idl0
                    //let external_idl = (id)       :: sql @ idl0       // With component name
                    let external_idl = (id)         :: sql @ [ hd idl0 ]  // Without component name
                    let (blobf_, internal_ce, _) = cil_svar_def ww cCB m0 (Some false) realdo false internal_idl dt None(*wopt*) LOCAL None (tats) None None None
                    let xnet = lnetgen ww (Some pi_key) internal_ce None (* suffix now not ever used here : suffixo *) // TODO if structs are args we need to call scone here via gec_singleton
                    let external_ce =
                        match xnet with
                            | X_bnet ff -> // Create external net with I/O attribute
                                // It is added to the sheap1 by cil_svar_def which is sufficient for it to appear in the final netlist via heapdig.
                                let (blobf_, external_ce, _) = cil_svar_def ww cCB m0 (Some false) realdo false external_idl dt None(*wopt*) io_info None (tats1) cpio None None
                                let _not_needed_really_ = xgen_bnet(iogen_serf(hptos external_idl, ff.length, ff.h, ff.width, io_info, ff.signed, None, false, map (fun (a,b)->Nap(a,b)) tats1, []))
                                external_ce
                                
                            | other -> sf ("cannot add an inhold register (or otherwise clone) for net " + netToStr other)
                    let _ = mutadd cCC.m_additional_net_decls xnet
                    //let _ = mutadd m_assigns (Xassign(xnet, external_net))
                    let _ = mutadd m_assigns (K_as_sassign(kxr, internal_ce, external_ce, None))
                    let _ = vprintln 3 (sprintf "iterm form with suffix: xnet=%s external_net=%s" (netToStr xnet) (ceToStr external_ce))
                    (idx, (external_idl, dt, internal_ce), xnet, Some external_ce)
                | None ->
                    let idl = id :: sql @ idl0
                    //vprintln 3 (sprintf "iterm form without suffix: idl=%s" (hptos idl))
                    let (blobf_, ce, _) = cil_svar_def ww cCB m0 (Some false) realdo false idl dt None(*wopt*) io_info None tats1 iogo None None
                    let xnet = lnetgen ww (Some pi_key) ce suffixo
                    vprintln 3 (sprintf "iterm form without suffix: xnet=%s ce=%s" (netToStr xnet) (ceToStr ce))
                    (idx, (idl, dt, ce), xnet, None)
                
        let _ = vprintln 3 ("genio rpc: done define id=" + id + " ce=" + ceToStr ce + " x=" + xToStr xnet)
        (idx, (idl, dt, ce), xnet, xnet_external)

    let (ii, oo) = if client_direction then (INPUT, OUTPUT) else (OUTPUT, INPUT)
    
    let rec argf idx = function
        | (Some x, idx_, dt, id)::tt -> sf("genio rpc: bound RPC stub arg " + x + "\n")
        | (None,   idx_, dt, id)::tt ->
            let r = genio idx [] id dt oo inholder_suffix
            r :: (argf (idx+1) tt)
        | [] -> []
        //| _ -> sf ("rpc_cs_terms other:")
        
    let arg_nets = argf 0 signat

    
    let adapt (bindingo, (b:int option), dt, (name:string)) =
        let binding_ = valOf_or bindingo "nobinding-anon"
        let (prec, width) = get_prec_width ww dt
        (true, binding_, prec, width, name)
    (* Simply declaring this net on the heap is sufficient? *)


    let method_name = "dotheop" // for now
    let netlevel = "both"
              
    let (rv, rvl) =
        if rtype <> CTL_void then
            let tats = [ (g_port_instance_name, pi_key); (g_logical_name, "return") ]
            let r =  genio -1 tats "return" rtype ii None
            let _ = vprintln 3 ("rpc server : return net " + ceToStr (f3o3 (f2o4 r)))
            let (prec, width) = get_prec_width ww rtype
            (Some r, [(false, "binding_", prec, width, "return")])
        else (None, [])

    // These wires needed for HSIMPLE 4-phase handshake and for HFAST: but can be different for other protocols. Will be part of directorate intrinsically in the future.
    // Always-ready methods and some others do not need any/some of them.
    let tats_a = [ (g_port_instance_name, pi_key); (g_logical_name, "ack") ]
    let tats_r = [ (g_port_instance_name, pi_key); (g_logical_name, "req") ]    

    // A g_logical_name is a token from a protocol definition that designates one net or vector within the net-level port that carries the protocol. So it would be wrong for the sq_overload to be applied to a logical name.
    let handshakers =
        [ (genio -1 tats_a "ack" g_canned_bool ii None, (false, "binding_", g_bool_prec, 1, "ack"))
          (genio -1 tats_r "req" g_canned_bool oo None, (true, "binding_", g_bool_prec, 1, "req"))
        ]

     

    // For this RPC method we emit both a busDefinition and one or two (RTL and TLM) busAbstractions.
    // We only need to this on server generation, but we tended to do it on both sides generation.
    // We noe defer busDefinition render until after the code is generted, so we can do static analysis re EIS and set the fsems correctly.
    if (false) then
        let expected_latency = 1 // for now - some of these are per method and some per block TODO 
        let reinit_latency = 1 // for now - some of these are per method and some per block TODO 
        protocols.render_ip_xact_export_busDefinition ww protocols.g_autometa_prefix bus_definition_name netlevel (map snd handshakers @ rvl @ (map adapt signat)) parameters method_name (expected_latency, reinit_latency, fsems)

    let bus_abstraction_name = bus_definition_name + (if netlevel <> "TLM" then "_rtl" else "_tlm")
    let signals = (map snd handshakers @ rvl @ (map adapt signat))
    protocols.render_ip_xact_export_busAbstraction ww protocols.g_autometa_prefix bus_abstraction_name netlevel signals parameters method_name


    let busDef_handy_info = (bus_definition_name, netlevel, handshakers, method_name, signat, parameters, signals)
    (rv, map fst handshakers, arg_nets, !m_assigns, busDef_handy_info) // end of generate_rpc_cs_terms



//
// This function creates the names used for wondarrays.
//
(*
 * A wondarray is a 1-d array corresponding to a concrete type, indexed by an integer widx.
 * A widx is a wondtoken/wtoken or a wvar, depending on whether a constant value is found by adv4 or not. 
 * A wondtoken is a base index to a wondarray, held as a CE_region with a marker attribute.
 *
 * This is not a net with these dimensions! It is just a marker to be picked up by repack.
 * The init field should be used again: as an hexp.
 *
 *)
let uid_nemtokf (uid:vimfo_t) =
    if uid.baser = 0L then sf "No nemtok an no address!"
    else
    // Perhaps return None if the uid is a nonce.
    //[ uidToStr uid; "U$D" ]
    "U$D" + "/" + uidToStr uid


let g_rnsc_tag = "rnsc"

    
// gec_CE_region
let gec_CE_Region ww rnsc (aido, content_type, uid, ats0, leno) = 
    let ats = [ ("constant", "true"); (g_wondtoken_marker, "true"); (* ("cid", rdot_fold uid.f_dtidl) *) ] @ ats0
    let ats = if rnsc then (g_rnsc_tag, "true") :: ats else ats // Add a really no scale marker.
    let volf = ct_is_volatile ww content_type
    let ats = if volf then (g_volat, "true")::ats else ats
    let adt = CT_arr(content_type, leno) // The wondarray is a region of unconstrained size. It is an array. This is its type.
    // The CE_region itself also has this type in it, but also base and length information.  Region types are always individual records/classes or arrays of any type.
    // The nemtok (aido) is that of the static rez site in the elaborated kcode.
    //let _ = vprintln 0 (sprintf "gec_CE_Region: gec_CE_region uid=%s null_aido=%A aid=%A" (uidToStr uid) (nonep aido) aido2)
    CE_region(uid, adt, ats, aido, None)

let gec_CE_Region_0 ww (aido, dt, uid) = gec_CE_Region ww false (aido, dt, uid, [], None)


let rec stackdump vd = function
    | []   ->
        youtln vd ""
        //sf "Stackdump enabled!"
    | h::t -> (youtln vd ("    STK:" + ceToStr h); stackdump vd t)




// Find overhead in stack use terms where an instance 'this' argument is used.
let thisohead(virtualf_, ck) =
    let hasthis =
        match ck with
            | CK_vararg hasthis
            | CK_default hasthis
            | CK_generic  hasthis
            | CK_instance hasthis -> hasthis
    let ohead = if hasthis then 1 else 0
    (hasthis, ohead)


let cilmethod_flags(No_method(srcfile, flags, ck, (_, uid, _), mdt, flags1, instructions0, atts)) =
    let rec gotch = function
            | [] -> ""
            | CIL_custom(s, t, fr, args, vale)::tt -> (cil_fieldrefToStr fr) + (gotch tt)
            | _::tt -> gotch tt

    let madsquirrel = gotch atts
    //let _ = vprintln 0 (" qq gotch=" + madsquirrel)
    let is_hpr_native =
        if madsquirrel.Contains("HprPrimitiveFunction")
        then [Cilflag_hprls]
        else []
    is_hpr_native @ flags


//
// Please explain this routine - no sign is applied! ?
//
let cil_apply_sign_to_constant bindings tt vv =
    match (tt, vv) with
    | (CTL_net(volf, width, signed, flags), Cil_argexp(Cil_float(prec, ss))) when signed=FloatingPoint ->
        let xx = xi_fps width ss
        let _ = if width <> prec * 8 then sf (sprintf "Unsupported float precision %i cf %i" prec width)        
        CE_x(tt, X_bnet xx) // Floating point literals are held as hexp_t bnet's with a constant value.

       
    | (CTL_net(volf, width, signed, flags), _) when signed=Signed ->

        let deconv = function
            | Cil_argexp(Cil_number32 n)  -> (32, BigInteger n)
            | Cil_argexp(Cil_int_i(w, n)) -> (w, n)
            | Cil_suffix_0 -> (-1, BigInteger 0)
            | Cil_suffix_1 -> (-1, BigInteger 1)
            | Cil_suffix_2 -> (-1, BigInteger 2)
            | Cil_suffix_3 -> (-1, BigInteger 3)
            | Cil_suffix_4 -> (-1, BigInteger 4)
            | Cil_suffix_5 -> (-1, BigInteger 5)
            | Cil_suffix_6 -> (-1, BigInteger 6)
            | Cil_suffix_7 -> (-1, BigInteger 7)
            | Cil_suffix_8 -> (-1, BigInteger 8)
            | Cil_suffix_m1 
            | Cil_suffix_M1 -> (-1, BigInteger -1)

            | other -> sf(sprintf "other form in constant deconv %A" other)
        let (w, bn) = deconv vv
        let w = if w < 1 then 32 else w
        let v = Cil_argexp(Cil_int_i(w, bn))  // We need to use the with-width forms.
        let v = ciltoarg bindings v
        v
        
   // | ... missing unsigned cases?  ??

#if OLD2015
    | (CTL_net(volf, width, signed, flags), Cil_argexp(Cil_tnumber(lst, x))) ->
        let t = Cil_tnumber(lst, x)
        if not(signed=Signed && width > 0) then v
        else
            let rec unwrap = function
                | (Cil_tnumber_end false) -> 0I
                | (Cil_tnumber(n, tt)) -> (BigInteger n) + 4096I * (unwrap tt)
                | _ -> sf "unwrap"
            let rec wrap = function
                | []   -> Cil_tnumber_end false
                | h::t -> Cil_tnumber(h, wrap t)

            let lst' = unwrap (t)
            let mask = muddy "tnum_exp (width-1) [2]"
            let signbit = muddy "do_wide_logic(&&& perform_bitand 12, mask, lst')"
            let needflip = signbit <> 0

            let docomplementer() = muddy "docomplementer"
#if OLD2012
                let r = wide_addsub(false, lst', wide_times(mask, [2]))
                let _ = lprintln 10 (fun()->"complementer w=" + i2s width + " for " + iexpToStr t + " m=" + tnumToX(X_tnum mask) + " sign=" + tnumToX(X_tnum signbit) + " needflip=" + boolToStr needflip + " ans=" + tnumToX(X_tnum r) + " ans=" + xToStr(X_tnum r))
                wrap r
#endif
            let ans = if needflip then Cil_argexp(docomplementer()) else v
            ans
#endif
    | (tt, tv) ->
        sf (sprintf "missing constant input for %s %A " (cil_typeToStr tt) vv)




let heapf_printout(heap:heap_t, msg) =
     let f  (sheap_id, b) = vprintln 0 ("    cez: " + sheap_id)
     let _ = vprintln 0 ("Error:" + msg)
     let _ = vprintln 0 ("Available static items were:")
     let _ = for z in heap.sheap1 do f (z.Key, z.Value) done
     sf msg;

// tnow - simulation time or real time in clock cycles etc..
// tnow is defined as Unsigned uint64 in meox.fs and Kiwi.cs. 
let tnow_cez ww realdo m0 =
    let vd = 3
    let ethereal = false 
    let ((_, ans), _)  = gec_uav_core ww vd m0 ethereal (Some false) STOREMODE_singleton_scalar realdo false g_canned_u64 (g_tnow_string, [ ("mapto", "g_tnownet")]) 0 None None None
    ans



(*
 * Accessing the value referred to by a reference is called dereferencing it.
 * DefinitiveS: Dereferencing is a matter of logically 'following a pointer' and it reduces the number of asterisks in the return type, but increases the number of asterisks on an expression if just done symbolically as a wrapper around an expression.
 * Dereferencing a valuetype is illegal; it's referencing a valuetype that takes its address.
 * Referencing an object makes a pointer to it, here represented by incrementing the star count, which for expressions and types is really an ampersnd count, but we call it a star count owing to the stars persent in the lhs of a C++ declaration syntax: e.g int *p = &myint.
 *)
//let trapper = ref 0
let rec gec_CE_star sitemarker stars arg = 
    match arg with
    | CE_star(n1, ce, aid) -> gec_CE_star sitemarker (stars+n1) ce
    | CE_typeonly dt       -> CE_typeonly(gec_CT_star stars dt)
    | ce ->
        if stars=0 then ce
        else
            let aid_o = 
                match ce_aid0 (fun () -> sprintf ": gec_CE_star nstars=%i" stars) ce with
                    | None -> None
                    | Some aid ->
                        let dereference_aid aid = anon_dereference_aid_core "gec_CE_star" sitemarker aid
                        let rec aid_star nn arg =
                            if nn = 0 then arg
                            elif nn < 0 then dereference_aid (aid_star (nn+1) arg)
                            //se map (fun x-> A_tagged(x, g_indtag, sitemarker)) (aid_star (nn-1) arg)
                            else (fun x-> A_subsc(x, None)) (aid_star (nn-1) arg)
                        let aid = aid_star stars aid
                        vprintln 3 (sprintf "DO L975 ce_aid0: gec_CE_star clause:  stars=%i ce=%s giving %s" stars (ceToStr ce) (aidToStr aid))
                        Some aid
            let aid = if nonep aid_o then sf "L780-aid-null" else valOf aid_o // discards any others - TODO tidy
            CE_star(stars, ce, aid)

//
// To dereference an array we can return its content type, but for a record we need to know which field: unsafe and gcc4cil code will sometimes have the numeric offset of the record field.
//
let dereference_ct msg offset = function
    | CT_star(n, arg) when abs n = 1 -> arg
    | CT_star(n, arg) ->
        let sgn x = if x>0 then 1 elif x<0 then -1 else 0
        (CT_star(n - sgn n, arg))
    | CT_arr(ct, None) -> ct
    | CT_arr(ct, Some len) ->
        let _ = dev_println (msg + sprintf "deleted array length information")
        ct

    | other ->
        let ks = (msg + sprintf " +++ dereference_ct other form %s when offset=%s" (cil_typeToStr other) (ceToStr offset))
        let _ = vprintln 3 ks
        Ciltype_filler // for now
 
    
// Follow a pointer:
//    Regarding the type, it should have a positive number of stars and we can decrement it. E.g.  &(CTL_record(x)) becomes CTL_record(x)
//    Regarding an expression, we seek to decrement the CE_star count and always compose CE_star(n, CE_star(m, x)) as CE_star(n+m, x).
let dereference_ptr sitemarker msg ce =
    let sgn x = if x>0 then 1 elif x<0 then -1 else 0
    //let _ = vprintln 0 ("Temp debug: " + msg + ": dereference_ptr: arg is " + ceToStr ce)
    match ce with
    | CE_star(stars, ce, aid) ->
        //let _ = if 
        gec_CE_star sitemarker (stars-1) ce

    | CE_x _ ->
        // should perhaps complain here -  This occurs when loading chars from a constant string array: perhaps a string array should have a ref on it to start with. Perhaps this is now fixed (Dec 2016) with the gcc4cil string support.
        ce  

    | CE_typeonly(CT_arr(ct, len)) -> CE_typeonly(ct)
    
    | CE_typeonly(CT_star(n, x))   -> CE_typeonly(gec_CT_star (n - sgn n) x) // Both +ve and -ve type stars move towards origin

    | CE_typeonly(x)               ->
        let _ = dev_println (msg + ": dereference_ptr: other typeonly: " + cil_typeToStr x)
        CE_typeonly(gec_CT_star (-1) x)    

    | other  ->
        let _ = dev_println (msg + ": dereference_ptr: other exp: " + ceToStr other)
        gec_CE_star sitemarker (-1) other // Remove one star

(*
 * Referencing a struct or variable (the ampersand operator in C and C#) makes a pointer to it, here represented by incrementing the star count.
 * The polarity of stars in the CE_star operator is different (and indeed it is a poor encoding since there are cases that are not inverse operations) whereas for the type, the absolute value indicates the number of dereferences needed to get back to the wrapped value, with polarity distinguishing between, in C terms, ampersand and asterisk.
 * Referencing a valuetype is need for C# structs - we defer that operation until really needed, using a star
 * in the meantime, but it involves taking the uid address field and promoting that to a pointer.
 *)
let reference_ptr sitemarker = function
    | ce -> gec_CE_star sitemarker 1 ce  // +ve stars in an expression is an ampersand operator.  

// Return true if the reference operator cannot be lowered to hexp_t form, as for instance, when the address of a virtual register is passed by reference to a method or the address of a struct.  When inter-basic block the control flow cannot be resolved by bevelab since it does not see anything, so contant_fold must work  in such cases (such as posit repack final negate example).
let purely_symbolic_under_reference msg = function
    | CE_struct _ -> true
    | CE_var(vrn, dt, idl, fdto, ats) when not_nonep (op_assoc g_purely_symbolic_under_reference ats)-> true
//    | CE_var(PHYS hidx, dt, idl,  ats) ->
//        hidx.baser <= 0L  // For unaddressable items we return a symbolic answer
// post gcc4cil we have always false for all other net forms
    | _ -> false

let rec ce_is_symbolic_var_reference msg = function
    | CE_conv(_, _, ce) -> ce_is_symbolic_var_reference msg ce 
    | CE_star(starx, ce, aid) when starx=1 && purely_symbolic_under_reference msg ce -> Some ce
    | _ -> None


let vrn_is_singleton msg idl_ vrn =
    let cCC = valOf !g_CCobject        
    match cCC.m_vregs.lookup vrn with
        | None ->
            sf (sprintf "vrn_is_singleton check: %s: missing vrn V%i  %s" msg vrn (hptos idl_))
        | Some vreg_record ->
            match vreg_record.storemode with
                | STOREMODE_singleton_scalar
                | STOREMODE_singleton_vector -> true
                | _ -> false


let ce_is_singleton msg = function
    | CE_var(vrn, dt, idl, fdto, ats) when vrn_is_singleton msg idl vrn -> Some idl

    | CE_subsc _
    | CE_struct _ -> None
    | other ->
        dev_println (sprintf "ce_is_singleton: %s: Default to false for %s" msg (ceToStr other))
        None

// We convert a dynamic field to a static field when there is only one instance used or when the staticated override acts on the current class since it is the root for this KiwiC run.
// This checks the type and not the instance: TODO a design might have two instances of the top-level instance, one linked to another!
let ce_is_singletonptr cCB msg arg =
    let staticated =
        match cCB(*dynclo*) with
            //| None     -> []
            | ccl -> ccl.staticated
    //if not_nullp staticated then dev_println (sprintf "Dublin dynclo=%s" (hptos staticated))
    let dmon name =
        //dev_println (sprintf "Dublin cf %s" (hptos name))
        name
    match arg with
        | CE_var(_x, CT_cr crst, idl_, fdto, ats) when not_nullp staticated && dmon crst.name = staticated -> // The type of this expression is comapred - very silly - the ktop instance name is what we really want to check.
            vprintln 3 (sprintf "Convert dynamic field to static owing to staticated override of %s" (hptos crst.name))
            Some staticated
        | CE_star(nn, ce, _) when abs nn = 1 -> ce_is_singleton msg ce
        | _ -> None
    
let rec reference_valuetype sitemarker ce0 = // reference: Take the address
    match ce0 with

    | CE_conv(dt, _, ce) when dt = g_ctl_object_ref -> reference_valuetype sitemarker ce

    | CE_region(hidx, dt, ats, nemtok, _) ->
        let ttt = gec_CT_star 1 dt   // The result is a pointer to the original type.
        CE_x(ttt, xi_bnum (BigInteger hidx.baser))

    | CE_var(vrn, dt, idl, fdto, ats) ->
        let cCC = valOf !g_CCobject
        match cCC.heap.sheap2.lookup vrn with
            | Some she -> 
               if purely_symbolic_under_reference "reference_valuetype" ce0 then
                   // For unaddressable items we return a symbolic answer
                   let _ = dev_println (sprintf "Temp note: added reference symbolically to %s at sitemarker %s" (ceToStr ce0) sitemarker)
                   gec_CE_star sitemarker 1 ce0
               else
                   let ttt = gec_CT_star 1 dt   // The result is a pointer to the original type. (c.g. IntPtr).
                   CE_x(ttt, xi_bnum (BigInteger she.baser))
            | None -> sf (sprintf "reference_valuetype: missing she on lookup of V%i for %s" vrn (hptos idl))

    | CE_struct(idtoken, dt, aid, items) -> //  It is indeed reasonable to reference a struct (e.g. ldloca of a struct local var).
        let _ = vprintln 3 ("Reference_valuetype anon_struct - no special action but we avoid a warning print in the other clause: " + ceToStr ce0)
        //if nonep items then None else Some(map (reference_valuetype sitemarker) (valOf items))
        //let ans = CE_struct(idtoken, gec_CT_star 1 dt, muddy "another aid L1006", items') // Silly old distributive law was wrong
        let ans = gec_CE_star sitemarker 1 ce0
        ans

    | CE_subsc(CT_arr(content_dt, len), arg_ce, idx, aido) ->
        let _ = vprintln 3 ("+++ reference_valuetype subsc - no special action") // + ceToStr ce0
        let ans = gec_CE_star sitemarker 1 ce0
        ans

    | other  ->
        let _ = vprintln 3 ("Reference_valuetype other - cannot reference an arbitrary expression normally - no special action taken, just added the & symbolically: " + ceToStr other)
        let ans  = gec_CE_star sitemarker 1 ce0
        ans

//
// Constructor for the CE_subsc operator is called gec_idx instead of gec_CE_subs.
// Called both during intcil (with no aido) gtrace and as origx for keval - perhaps split?
//
// nt is inserted: this is the aid (nemtok+stars) of the ...
//
let rec gec_idx ww sitemarker mf arg_ce aid_o idx =
    let msg = "gec_idx"
    let rec get_content_dt arg =
        let (ct_, len_) = array_both_from_type ww arg
        match arg with 
        | CT_arr(content_dt, len) -> (content_dt, len)
        | other -> sf (mf() + sprintf ": gec_idx:  other form in local get_content_dt %s"  (typeinfoToStr other))
                    
    let dt = get_ce_type "gec_idx_L908" arg_ce
    let (content_dt, len) = get_content_dt (f1o3(ty_mark_strip (0, false) dt))
    //The cached aid_info fields in the CE_subsc describe the result of the CE_subsc expression so are incrementing (deleting an A_ind) to get back to the base and offset expression storage classes themselves.
    let aid_o =
        match aid_o with
            | None ->
                let (aid_bo, aid_oo) = (ce_aid0 mf arg_ce, ce_aid0 mf idx)
                if nonep aid_bo then None
                else Some(A_subsc(valOf aid_bo, aid_oo))
                //(oapp (anon_deca_for_vassign sitemarker) aid_b, oapp (anon_deca_for_vassign sitemarker) aid_o)
            | Some _ -> aid_o
    //dev_println (mf() + sprintf "gec_idx trace of %s type is %s" (ceToStr arg_ce) (cil_typeToStr(get_ce_type msg arg_ce)))
    //dev_println (sprintf  " gec_idx trace of aid_b=%s, aid_a=%s" (sfold aid2s aid_b) (sfold aid2s aid_o))
    CE_subsc(CT_arr(content_dt, len), arg_ce, idx, aid_o)

let rec gen_typeonly = function
    | (0, t) -> CE_typeonly(t) 
    | (n, t) -> gen_typeonly(0, gec_CT_star n t)


//
// Constructor for the CE_dot operator: gec_CE_dot.
//
// passed in dct is a pair with both pre or post type.
// When tago=None then return all fields for structure assigns.
//
// If the arg is a CE_struct we can directly extract the relevant field's value and for other structs (but not classes) we can consider extending the struct name.
// e.g. we convert  [| &(Test59.aa)->sdata1 |] the in the kcode to [|  Test59.aa.sdata1 |] in keval.
// We supply 'Some ptag' as tago (since utags are only for aid loop detection).
let gec_field_snd0 ww sitemarker msg dt_record tago ce = // aka gec_CE_dot
    let ww = WN "gec_field_snd0" ww
    let mf = fun()->(sprintf "gec_field_snd0 tago=%A dt_record=%s" tago (cil_typeToStr dt_record))
    //let _ = dev_println (mf () + sprintf " gec_field_snd0 ce=%s" (ceToStr ce))
    let gec_field1 ns vale =
        let (arglen, items) =
            match vale with
                | CE_struct(idtoken, _, aid, items) -> (length items, Some items)
                | _                        -> (-10000, None)
        let rec gtag1 record_idl = function // scan for one or all...
            | (crt:concrete_recfield_t)::tt when (nonep tago || valOf tago = crt.ptag) ->
                let base_aid_o = ce_aid0 (fun ()->mf()+":ce_aid dot gec_field_snd0") vale
                let aid_o =
                    if nonep tago then
                        vprintln 3 (sprintf "+++ null tag/dot, need base aidl" + aidoToStr base_aid_o)
                        oapp (anon_deca_for_vassign sitemarker) base_aid_o
                    else oapp (deca_for_dotassign (crt.ptag, crt.utag)) base_aid_o

                let a0 =
                    match items with
                        | Some items ->
                            let _ = vprintln 3 (sprintf "gec_field_snd0: %s: manual select from struct %s item no=%i %s" sitemarker (hptos record_idl) crt.no crt.ptag)
                            match select_nth crt.no items with
                                | (ptag, _, Some vale) -> vale
                                | (ptag, _, None)      -> cleanexit(sprintf "L1085 - uninstantiated struct field tag=%s" ptag)
                        | None ->
                            CE_dot(dt_record, vale, (crt.no, crt.fld_st_o, (crt.ptag, crt.utag), crt.dt, crt.pos), aid_o)// We could have just saved all of crt in the CE_dot!
                let ans = a0
                if nonep tago then ans::(gtag1 record_idl tt) 
                else [ans] // Can stop after first match.

            | _::tt -> gtag1 record_idl tt             

            | [] when tago=None -> []
            | [] -> sf(sprintf "gtag1: no such tag: %A " (valOf tago) + " in record " + hptos record_idl)

#if SPARE
        let rec gtag1v_ record_idl baser = function // create composite var ref - not used in this form
            | (crt:concrete_recfield_t)::tt when valOf tago = crt.idl ->
                let aidl =
                    if nonep tago then
                        let _ = dev_println (sprintf "temp rezed aidl=[] in dot=%A crtidl=%s" tago (hptos crt.idl))//TODO used? please explain
                        []
                    else list_once(map (deca_for_dotassign (valOf tago)) (ce_aid0 (fun ()->mf()+":ce_aid dot gec_field_snd0") vale))
                let composite = (valOf tago) @ record_idl
                let dtidl = ct2idl ww crt.dt
                // e.g. we convert  [| &(Test59/aa)->sdata1 |] to [|  Test59/aa/sdata1 |]
                let ans = CE_var(PHYS{ f_name=composite; f_dtidl=dtidl; baser=baser+(int64 crt.pos); length=None(*REALLY ALWAYS*) }, crt.dt, composite, None, [])
                    //dot(dt_record, vale, (crt.idl, crt.dt, crt.pos), aidl)) // could just save all of crt in the CE_dot!
                [ans]
            | _::tt -> gtag1v_ record_idl baser tt             
            | [] -> sf("gtag1v: no such tag: " + htos(valOf tago) + " in record " + hptos record_idl)
#endif

        let rec gtag0 stars arg =
            let _ = vprintln // 0 ("gtag0 of " + typeinfoToStr arg)
            match arg with
                // TODO use valuetype/star/brand discard

            | CT_star(ns, dt) -> gtag0 (stars+ns) dt

            | CTL_record(sq_idl, cst, lst, len, ats, binds, srco) -> // nicely have records and not classes on a 2nd pass
                let _ = vprintln // 0 "record"
                gtag1 sq_idl lst
           
            | CT_class cdt -> sf (msg + ": class should be a CTL_record for code generation: " + hptos cdt.name + ", not " + cil_typeToStr arg)
            
            | CTL_void -> sf (sprintf "gec_field_snd0: void pointers cannot be used to access fields. tagso=%A" tago)
            
            | arg when not_nonep tago && valOf tago = g_kiwi_objectlock ->
                // The lock field does not explicitly exist; instead we tacitly create it when referenced.
                let aid_o = oapp (deca_for_dotassign ("__object_lock", "__object_lock")) (ce_aid0 (fun ()->mf()+":ce_aid dot gec_field_snd0") vale)
                //let _ = dev_println(sprintf "aidl=%s for objectlock on arg=%A" (sfold aid2s aidl) arg)
                let dt_field = g_canned_bool
                [CE_dot(g_ctl_object_ref, vale, (-1, None, (g_kiwi_objectlock, g_kiwi_objectlock), dt_field, 0L), aid_o)]

            | dt when dt=g_ctl_object_ref && nonep tago ->
                muddy("gtag0 dt object: gec_field: error looking for all fields in an uninstantiated class (the objroot)\nPerhaps the defining class was has not been nominated for KiwiC compilation")

            | dt when dt=g_ctl_object_ref -> sf(sprintf "gtag0 dt object: gec_field: error looking for field '%s' in an uninstantiated class (the objroot)\nPerhaps the defining class was has not been nominated for KiwiC compilation" (sprintf "%A" (valOf tago)))

            | record_dt when nonep tago ->
                let items = [] // class is so-far unresolved (e.g. first pass) so do not return a list of typeonlys.
                let _ = vprintln 3 (sprintf "Return empty struct for so-far unresolved class. (ok for typecheck) " + cil_typeToStr record_dt)
                let idtoken = funique "SID"
                [CE_struct(idtoken, record_dt, idtoken, [])]

            | dt_other -> sf(msg + sprintf ": gtag0 dt other: gec_field: error looking for field '%A' in a %s" (valOf tago) (typeinfoToStr dt_other))

        let ans = gtag0 0 dt_record
        map (gec_CE_star sitemarker ns) ans

    match ce with
        | CE_star(ns, CE_struct(idtoken, dt, aid_, rlst), aid) when not_nonep tago && ns > 0 ->
            //let _ = vprintln 3 (sprintf "L996 Idiom1 code should be invoked here ... ns=%i %A rlst=%A" ns (valOf tago) rlst)            
            //muddy (sprintf "test30/test35 should invoke code here ...  ns=%i %A rlst=%A " ns (valOf tago) rlst)
            gec_field1 (ns-1) (CE_struct(idtoken, dt, aid_, rlst)) // TODO we discard a star here but it should not have been present - kludge for test35

        | CE_struct(idtoken, dt, aid, rlst) when not_nonep tago -> // Struct field extract operation.
            //let _ = dev_println (sprintf "L1001 Is there any idiom1 code to invoke here ... struct_field_extract ... nostar %A rlst=%A" (valOf tago) rlst)
            gec_field1 0 ce   
        | ce            -> gec_field1 0 ce   

    //| ce -> sf ("gec_field other: "  + ceToStr ce + "\ntag_idl=" + (if tago=None then "None" else htos(valOf tago)) + "\nPerhaps you are dereferencing an unassigned object handle.")




let gec_field_snd_pass ww sitemarker msg dt_record ce ptag = // Wierd ptag call convention for now! fix it.
    let ans = gec_field_snd0 ww sitemarker msg dt_record (if ptag="" then None else Some ptag) ce
    match ans with
        | [item] -> item
        | others -> sf (sprintf "Got %i. Not one field in gec_field with ptag=%A site=%s" (length others) ptag sitemarker)        // Appropriate when ptag=""


let bopbass = false // temporary debug print off

let log_tval ww msg dlist callstring lty =
    let a1vd = !g_firstpass_vd
    if a1vd>=4 then vprintln 4 (msg + sprintf ": log_tval prefix=%s dropping=%s" (hptos callstring) (drToStr lty))
    mutadd dlist (callstring, lty)

let log_pval ww dlist callstring ce =
    match ce with
        | CE_typeonly ty -> log_tval ww "log_pval" dlist callstring (gec_RN_monoty ww ty)
        | other -> sf (sprintf "log_pval other form %A" other)

//
// For instance fields, we can lookup the types from the instance on the stack OR use annotation in the instruction (cr0).
// For statics there is no instance on the stack.
//        
let field_type_check_and_drop ww msg thato dlist callstring gb ins_annotated_ast_dt fridl =
    let vdp = false
    let ww = (WN "ftc: field_type_check_and_drop" ww)
    let staticf = nonep thato // If no structptr then a static operation and ins_annotate_ast_dt may be referred to.
    //let _ = vprintln 0 (sprintf "field_type_check_and_drop: '%s': staticf=%A: looking for field '%s' of type %A" msg staticf (hptos fridl) (ins_annotated_ast_dt))
    if vdp then dev_println(sprintf "field_type_check_and_drop: '%s': staticf=%A: looking for field '%s' (of annotated type %s)" msg staticf (hptos fridl) (cil_typeToStr ins_annotated_ast_dt))    
    let ptag = hd fridl // public tag
    let tag_eq fridl idl =
        //let _ = if vd then dev_println(msg + sprintf ": tag idl compare  %A  cf  %A" fridl idl)
        fridl = idl

    let rec field_dt_get arg =
        //vprintln 0 (sprintf "field_dt_get: %s %s search in %A" msg (hptos callstring) arg)
        match arg with
        //| CT_brand(_, dt) | CT_valuetype(dt)        
        | CT_star(_, dt) -> field_dt_get dt        

        | CTL_record(rec_idl_, cst, crt_dt, _, _, binds, _) ->
            let field_select10 cc (crt:concrete_recfield_t) =
                if crt.ptag = ptag then crt::cc // looked up on public tag field
                else cc
            match List.fold field_select10 [] crt_dt with
                | [x] ->  [ gec_RN_monoty ww x.dt]
                | [] -> sf (sprintf "field %s wanted from %s but there is no such field (popas) %A" (hptos fridl) (hptos cst.name) arg)
                | _ -> sf (sprintf "more than one popas %A" arg)                

        | CT_cr crst ->
            let crt = 
                match crst.crtno with
                    | CTL_record(sqidl, cst, lst, len, _, binds, _) -> cst                    
                    | CT_class cst -> cst
                    | CT_knot(idl, whom, vv) when not(nonep !vv) -> 
                        match valOf !vv with
                            | CT_class cst -> cst
                            | CTL_record(sqidl, cst, lst, len, _, binds, _) -> cst
                            | other -> sf (sprintf "need to unpack other form 2/1 : fetched=%A" other)
                    | other -> sf (sprintf "class expected: need to unpack other form 1/2 fetched=%A" other)

            let gb1 =
                let binds = List.zip crt.newformals crst.cl_actuals
                //let _ = vprintln 0 (msg + sprintf ": inserting %i typeformals for surrounding class. %A" (length binds) (dest_super_ty))
                let gmap (m:annotation_map_t) (x, y) =
                    let _ = vprintln 3 (msg + sprintf ": field check_and_drop: surrounding class template bind fid=%s ty=%s" x (drToStr y))
                    m.Add([x], y)
                List.fold gmap (snd gb) binds 
                
            let field_select4 cc = function
                | (idl', d) when tag_eq fridl idl' ->
                    match d with
                        | RN_monoty(ty, ats) ->
                            let dcnew = cil_type_n ww ({ grounded=false; }, gb1) ty 
                            gec_RN_monoty_ats ww dcnew ats :: cc
                        | x -> sf(msg + sprintf ": _and_drop: field_select3 other %A" x)
                | _ -> cc

            List.fold field_select4 [] crt.class_item_types 
                
        | CT_class cdt -> // This will be lacking type actuals if polymorphic but will be rehydrated.
            let field_select3 cc = function
                | (idl', d) when tag_eq fridl idl' ->
                    match d with
                        | RN_monoty(ty, ats) ->
                            let _ = vprintln 0 ("+++ eval needed if poly?  No - rehydrate will rewrite this and do the eval.")
                            let dcnew = ty 
                            gec_RN_monoty_ats ww dcnew ats :: cc
                        | x -> sf(msg + sprintf ": _and_drop: field_select3 other %A" x)
                | _ -> cc

            List.fold field_select3 [] cdt.class_item_types 

        | CTL_net _  when staticf -> muddy " [gec_RN_monoty ww arg] " // This is a nonsense return that is not going to be used. TODO tidy.

        | other -> sf (sprintf " field_type_check_and_drop: '%s':  looking for field '%s' in other form %A" msg (hptos fridl) other)
            
    let (rec_dt, field_dt) = 
        if staticf then
            let field_dt =
                //let _ = vprintln 3  ("This potentially ignores the volatile attribute ")
                cil_type_n ww gb ins_annotated_ast_dt 
            let rec_dt = CTL_void // Use void as a filler for the class of a static field. We should not be interested in its nominal record name.
            if vdp then dev_println(sprintf "static field_type_check_and_drop: '%s': rec_dt=%s\nfield_dt=%s posout %A"  msg (cil_typeToStr rec_dt) (cil_typeToStr field_dt) ins_annotated_ast_dt)
            (rec_dt, field_dt)
        else
            let rec_dt = get_ce_type "get_idx_L1124" (valOf thato)
            //vprintln 0 (sprintf "struct thato=%s" (typeinfoToStr rec_ty))
            let field_dt =
                match field_dt_get rec_dt  with
                    | [] -> cleanexit (msg + sprintf ": field not found: looking for %s (annotated type  %s)" (hptos fridl) (cil_typeToStr ins_annotated_ast_dt))
                    | [RN_monoty(dt, _)] -> dt
                    | ans -> sf (sprintf "field_type_determine: multiple or other answers %A" ans)
            (rec_dt, field_dt)

#if NEEDED_PERHAPS        
    let dt =
                            | [] when tag_idl = [g_kiwi_objectlock] ->
                                let dt_field = g_canned_bool
                                gec_RN_monoty ww dt_field
#endif                
    if vdp then vprintln 3 (sprintf "field_type_check_and_drop: Drop %s callstring=%s dt=%s idl=%s" msg (hptos callstring ) (cil_typeToStr field_dt) (hptos fridl))
    log_tval ww msg dlist  ((*g_field_type_marker::*)callstring) (RN_duplex(gec_RN_monoty ww rec_dt, gec_RN_monoty ww field_dt, ptag)) // Save record type and field type.
    field_dt


// Droppings are already rehydrated for the codegen pass so this is a nop.
let concrate ww msg = function
    | RN_monoty(ty, ats) ->
        match ty with
            // A minor check that classes are no longer present (all should now be CTL_records ...)
            | CT_class cr -> sf (msg + ": concrate: A class should be replaced with a record if grounded or else an RN_cr. Not ty=" + cil_typeToStr ty)
            | ty -> (ty, ats)
    | other -> sf (sprintf "rn concrate other form %A " other)
    


// Make dropping - hoik out type actuals for pushdown manipulation. -- sufficient? i.e. are there any other use cases that will need pushdown mods
let qqx_a ww msg cl_actuals = function
    | CT_class crt ->
        let la = length cl_actuals
        let _ = vprintln 0 (sprintf "gec CT_cr inserted %i actuals for %s" la (hptos crt.name)) 
        let _ = if la <> crt.arity then sf ("wrong arity in class ref for " + hptos crt.name)
        //if (sprintf "inserted %i actuals for %s" (length cl_actuals) (hptos crt.name))  = "inserted 0 actuals for sonclass`2" then sf "hit it"
        gec_RN_monoty ww (CT_cr { g_blank_crefs with name=crt.name; cl_actuals=cl_actuals; crtno=CT_class crt; })
    
    | ty -> gec_RN_monoty ww ty



let call_type_retrieve ww msg (_, gbm:annotation_map_t) prefix =
    match gbm.TryFind prefix with
        | Some(RN_call(class_dto, rt, gactuals, signat, cgr, mgr)) -> (class_dto, rt, gactuals, signat, cgr, mgr)
        | Some other  -> sf (sprintf "ftrm other %A" other)
        | None ->
            let inscope = sfold (fun (x,y)->htos x) (Map.toList gbm)
            sf(sprintf "%s: prefix=%s call_type_retrieve_multi failed: inscope=%s" msg (hptos prefix) inscope)


let field_type_retrieve ww msg (_, gbm:annotation_map_t) prefix =
    match gbm.TryFind ((*g_field_type_marker::*)prefix) with
        | Some (RN_duplex(ty_rec, ty_fld, ptag))  ->
            let ((ty_rec, _), (ty_fld, _)) = (concrate ww msg ty_rec, concrate ww msg ty_fld)
            //let _ = vprintln 0 (sprintf "%s: prefix=%s duplex types retrieved rec=%s and fld=%s" msg (hptos prefix) (cil_typeToStr ty_rec) (cil_typeToStr ty_fld))
            (ty_rec, ty_fld, ptag)
        | Some other  -> sf (msg + sprintf ": field_type: non-duplex : other rn retrieved %A" other)
        | None ->
            let inscope = sfold (fun (x, y)->hptos x) (Map.toList gbm)
            sf(sprintf "%s: prefix=%s field_type_retrieve: inscope=%s" msg (hptos prefix) inscope)

let mono_ats_type_retrieve ww msg (_, gbm:annotation_map_t) prefix =
    match gbm.TryFind prefix with
        | Some rn ->
            let (ty, ats) = concrate ww msg rn        
            //let _ = vprintln 0 (sprintf "prefix=%s: retrieved mono dropping %A as %s" (hptos prefix) rn (cil_typeToStr rr))
            (ty, ats)
        | None ->
            let ov = sfold (fun (x,y)->htos x) (Map.toList gbm)
            sf(sprintf "%s: prefix=%s mono_type_retrieve missing dropping: inscope=%s" msg (hptos prefix) ov)


let mono_type_retrieve ww msg (x, gbm:annotation_map_t) prefix =
    let (ty, ats) = mono_ats_type_retrieve ww msg (x, gbm) prefix
    ty
    
let g_dummy_objectholder_ = "$$elab1_dummy_placeholder___" // no longer user

let nonvoid rt =
    let ans =
        match rt with
        | CTL_void -> false
        | Cil_namespace(_, Cil_cr "Void") -> false
        | _ -> true
    //let _ = vprintln 0 ("nonvoider ? " + cil_typeToStr rt + " " + boolToStr ans)
    ans


// Predicate to check for manifestly constant expressions
let ce_constantp arg = 
    let rec con ns = function
        | CE_struct(idtoken, dt, _, items) ->
            let spag = function
                | (tag, dt, None) -> false
                | (tag, dt, Some cez) -> con ns cez (*ns should probably be zero again for this call*)
            conjunctionate spag items
        | CE_reflection _ -> true
        | CE_conv(_, _, ce)  -> con ns ce
        // newarr: nasty old way : need to make array handles look constant for now ! which is not true in general --- old way ignores ns TODO fix
        //| CE_newarr _     -> true
        | CE_x(_, x) -> fconstantp x
        | CE_dot(_, x, _, aid) -> false// ce_constantp x
        | CE_region(uid, dt, ats, nemtok, cil) ->
            if ns=0 then (uid.baser = 0L || op_assoc "constant" ats <>None)
            else false
        | CE_var(hidx, vdt__, idl, fdto, ats) ->
            // The address of a scalar variable is constant or the variable itself may be attributed as a constant
            ns > 0 || op_assoc "constant" ats <>None
        | CE_star(a, ce, aid) -> con (ns+a) ce
        | CE_subsc(_, ce, idx, _) -> con (ns+1) ce && con ns idx

        // Reftrans built-ins.
        // TODO:  We should just check a 'referentially transparent' annotation within bif.reftrans and hence know that constant in means constant out!
        | CE_apply(bif, cf, [arg]) when bif.uname=g_bif_neg.uname || bif.uname=g_bif_not.uname -> con ns arg//TODO: do not pass on ns
        | CE_apply(bif, cf, [a1])    when hd bif.uname="Abs" -> con ns a1 // It is safe tp only check the last component of the function name for built-in functions since we know there are no aliases.
        | CE_apply(bif, cf, [a1;a2]) when hd bif.uname="Max" || hd bif.uname="Min" -> con ns a1 &&  con ns a2


        | CE_apply(other_fun, _, _) ->
            let _ = vprintln 3 (sprintf "conservatively assume that %s has non-constant result" other_fun.hpr_name)
            false
        | CE_alu(_, _, a1, a2, _)
        | CE_scond(_, a1, a2) ->
            let va = con ns a1 &&  con ns a2
            let _ = if va then vprintln 0 (sprintf "+++ should not have made a constant scond form %A" arg)
            va
            
        | other -> (vprintln 0 ("Please fix +++ ce_constantp other: " + ceToStr other); false)
    let ans = con 0 arg
    //let _ = vprintln 0 ("ce_constantp " + boolToStr ans + " for " + ceToStr arg)
    ans 


let gec_CE_scond(oo, a1, a2) =
    let m = "generate scond"
    let tailer () = CE_scond(oo, a1, a2)

    let signed_override = function // We may wish to apply converts to the args so that the rest of the tool chain does not need to be so strictly accurate about interpreting the signnedness and it may help fold converts to constant leaf expressions more quickly.
            | V_dled signed 
            | V_dltd signed -> Some signed
            | _ -> None
        // ... but not done currently ...
            
    if ce_constantp  a1 && ce_constantp  a2 then
        let rec ssm = function
            | (CE_x(_, l), CE_x(_, r)) ->
                let q = ix_bdiop oo [l; r] false
                gec_yb(xi_blift q, g_canned_bool)
                //in  sf (sprintf "resolved form constant scond %A (%A)  %A -> %A" a1 oo a2 q)
            | (CE_typeonly dt0, CE_typeonly dt1) when dt0=g_ctl_object_ref && dt1=g_ctl_object_ref ->
                let lr = xi_num 1
                let q = ix_bdiop oo [lr; lr] false
                gec_yb(xi_blift q, g_canned_bool)

            | _ -> sf (sprintf "other form constant scond %A cf (%A)  %A" a1 oo a2)
        ssm (a1, a2)

    else tailer()



let gec_CE_compare ww cmd ll rr =
    match cmd with
        | "Equals"    -> gec_CE_scond(V_deqd, ll, rr) // For struct references, since they are supposed to be valuetypes,  we need to do a deep equals, but nlower will implement this.

        | "CompareTo" -> gec_CE_alu ww (g_canned_i32, V_minus, ll, rr) 
            // returns -ve if ll preceeds rr in standard ICompare order

        | cmd -> muddy ("gec_CE_compare: other=" + cmd)

(*=================================================*)

let g_end_of_flow_delta  = -100
let g_leave_delta        = -101

let special_stack_depth_pred arg = arg=g_end_of_flow_delta || arg=g_leave_delta



(*
 * Give stack bump delta for each instruction.
 * For instructions that exit, return g_end_of_flow_delta.
 *)
let stackcounter vv = 
    let sdfun = function
        | Cili_throw -> g_end_of_flow_delta

        | Cili_isinst _ -> 0
        | Cili_kiwi_trystart _ -> 0
        | Cili_kiwi_special _ -> -1        

        | Cili_ldstr _   // Load instructions
        | Cili_ldnull
        | Cili_ldarg _
        | Cili_ldarga _
        | Cili_ldtoken _
        | Cili_ldftn _
        | Cili_ldloc _
        | Cili_ldloca _
        | Cili_ldc _
        | Cili_ldsfld _
        | Cili_ldsflda _ -> 1


        | Cili_ldfld _   -> 0
        | Cili_ldflda _  -> 0
        | Cili_ldlen     -> 0

        | Cili_cpblk       -> -3
        | Cili_initblk     -> -3

//       | Cili_refanyval

        // Prefixae
        | Cili_prefix _ -> 0

        | Cili_stfld _  -> -2

        | Cili_stsfld _
        | Cili_stloc _
        | Cili_starg _ -> -1

        | Cili_dup -> 1
        | Cili_pop -> -1
        
        | Cili_not -> 0
         
        | Cili_comment _
        | Cili_nop 
        | Cili_lab _ -> 0


        | Cili_leave _ -> g_leave_delta  // Leave is magic in that it zeros the stack depth always.

        | Cili_endfault
        | Cili_endfinally -> 0         

        | Cili_neg -> 0     

        | Cili_and
        | Cili_or
        | Cili_xor
        | Cili_mul _
        | Cili_add _
        | Cili_div _
        | Cili_rem _
        | Cili_sub _
        | Cili_shl _
        | Cili_shr _ -> -1
        
        | Cili_stelem _  -> -3
        | Cili_stind _   -> -2
        | Cili_ldind _   -> 0
        | Cili_ldelem _  -> -1
        | Cili_ldelema _ -> -1

        | Cili_ldobj _ -> 0
        | Cili_stobj _ -> -2
        
        |  Cili_box_any _
        |  Cili_unbox_any _ 
        |  Cili_box _ 
        |  Cili_unbox _ -> 0

        | Cili_conv _ -> 0
        | Cili_br _ -> 0

        | Cili_switch _
        | Cili_brtrue _
        | Cili_brfalse _ -> -1


        |   (Cili_bgt _) -> -2
        |   (Cili_bge _) -> -2
        |   (Cili_blt _) -> -2
        |   (Cili_ble _) -> -2

        |   (Cili_bgt_un _) -> -2
        |   (Cili_bge_un _) -> -2
        |   (Cili_blt_un _) -> -2
        |   (Cili_ble_un _) -> -2
        
        |   (Cili_bne _) -> -2
        |   (Cili_beq _) -> -2
        |   (Cili_cgt _) -> -1
        |   (Cili_cge _) -> -1
        |   (Cili_cne) -> -1
        |   (Cili_ceq) -> -1
        |   (Cili_clt _) -> -1
        |   (Cili_cle _) -> -1


        | Cili_newarr cr ->
            let argcnt = 1
            -argcnt + 1

        | Cili_newobj(flags, ck, tt, (cr, id, gargs), signat, site_) ->
             let argcnt = length signat
             //let noinstancef_ = callkind_static ck
             // NB: noinstance is ignored ... what does it mean in this context?
             -argcnt + 1

        | Cili_initobj _ -> -1

        | Cili_call(opcode, ck, rt, fr, signat, site_) -> 
             let argcnt = length signat
             let voidf = not(nonvoid rt)
             let  (instancef, ohead) = thisohead(opcode, ck) 
             let noinstancef = not instancef
             //let _ = vprintln 0 (sprintf "call accounting ck=%s voidf=%A noinstancef=%A" ck voidf noinstancef)
             -argcnt + (if voidf then 0 else 1) + (if noinstancef then 0 else -1)
                
        | Cili_castclass _ -> 0
        
        | other -> sf("stackcounter: unsupported PE/CIL instruction: " + ciliToStr other)

    let threadexit = function
        | Cili_ret   -> true
        | Cili_throw -> true // Not an exit when caught of course, but stack contents will be irrelevant except for top location that has the exception handle in it.
        | _          -> false
    if threadexit vv then g_end_of_flow_delta
    else
        let r = sdfun vv
        //let _ = vprintln 0 (sprintf "stack delta for %A is %i" (ciliToStr v) r)
        r

let stackcounter1 = function
    | CIL_instruct(_, _, _, v) ->
         (
           (*     if k=None then () else vprint(0, "label " + valOf k + "\n"); *)
           stackcounter v
         )
    | _ -> 0 




(*
 * Scan a basic block following the stack depth count.
 *)
let stackcounter1c vd c arg = 
     if special_stack_depth_pred c then c
     else
         let d = stackcounter1 arg
         let ans = if special_stack_depth_pred d then d else d + c
         if vd >= 4 then vprintln 4 (sprintf "stackcounter1c: delta=%i, sd=%i ::: %s" d ans (cil_classitemToStr "" arg ""))
         ans

let spilgen_xcheck (D:cil_methodenv_t) id instructions = (* useless xcheck *)
    let counts = map stackcounter1 instructions
    ()



let unconditional_jmp2 lst = disjunctionate (* aka any *) unconditional_jmp1 lst


// Kiwi.Remote() attribute parsing
let parse_kiwi_remote_protocol_attributes ww msg arg =
    let styles_etc =
        let destring arg cc =
            match arg with
                // | int -> i2s int
                | (_, W_string(ss, _, _)) -> ss::cc
                | _ -> cc
        List.foldBack destring arg []
    let ww = WF 3 "parse_kiwi_remote_protocol_attributes" ww ("start " + sfold id styles_etc)
    let toupper (ss:string) = String.map System.Char.ToUpper ss
    let prams = string_to_assoc_list (sfold_colons styles_etc) // Parse the string, getting key/value pairs from a colon or semicolon-separated list.
    let fsems =  // The user-attributed properties override those that are inferred.
        let override_fsem_attribute fsems (key, vale) =
            let key = toupper key
            if memberp key protocols.g_foldable_fsem_attribute_names then
                vprintln 3 (sprintf "Overriding inferred protocol attribute '%s' with '%s'" key vale)
                protocols.fsems_folder msg key vale fsems
            else fsems
        List.fold override_fsem_attribute g_default_fsems prams
        
    let externals = valOf_or (op_assoc "externally-instantiated" prams) "false"
    let searchbymethod = (valOf_or (op_assoc "searchbymethod" prams) "false") = "true" 
    let overloaded = (valOf_or (op_assoc "overloaded" prams) "false") = "true"   
    let stubonly_ = (valOf_or (op_assoc "stubonly" prams) "false") = "true"
    let externally_instantiated  = externals = "true"
    let protocol = valOf_or (op_assoc "protocol" prams) "HSIMPLE"
    let fsems =
       { fsems with
              fs_overload_disam=  overloaded    
       }
    if fsems.fs_asynch then muddy "Kiwi.Remote: async call" // ... for now ... please integrate with 
    (prams, protocol, searchbymethod, externally_instantiated, fsems)




// Old special code for pointer arithmetic - prior to pointer arithmetic being generaly supported (for C++ input).
let gec_CE_handleArith ww self baser offset =
    let msg = "gec_CE_handleArith" 
    if true then
        let ct = get_ce_type msg self  // Please explain why we do not simply take the type of baser
        gec_CE_alu ww (ct, V_plus, baser, offset)

    else 
    match get_ce_type msg self with
        | CT_cr cst ->
            match cst.cl_actuals @ cst.meth_actuals with
                | [RN_monoty(st, _)] -> CE_handleArith(st, baser, offset)
                | other ->  sf (sprintf "gec_CE_handleArith : other generic form %A" other)
    
        | self ->  sf (sprintf "gec_CE_handleArith : other form %A" self)

// Old vararg implementations
let builtin_v = function
        | ["WriteLine"; "Console"; "System"; "mscorlib"] -> Some "hpr_writeln"  (* Varargs ones *)
        | ["Write"; "Console"; "System"; "mscorlib"]     -> Some "hpr_write"
        | ["NeverReached"; "Kiwi" ]                      -> Some "hpr_neverreached"
        | (_) -> None


let rec ce_manifest_int64 m = function
    | CE_x(_, x) -> xi_manifest64 m x

    | CE_apply(bif, cf, [arg]) when bif.uname=g_bif_not.uname ->  // One's complement - bitwise complement.
        let a1 = ce_manifest_int64 m arg
        -1L - a1 

    | CE_apply(bif, cf, [arg]) when bif.uname=g_bif_neg.uname ->         // Subtract from zero - two's complement.
        let a1 = ce_manifest_int64 m arg
        0L - a1

    | ce -> sf ("Need constant expression: ce_manifest_int64 " + m + " other: " + ceToStr ce)


// Overcome an expression from not being referentially transparent - where is has side effects, evaluate it once and cache the result in the new variable.
let block_reftran_fastspill ww vd m0 cCBo realdo kxr aid arg_ats cg (arg_dt, arg) = // TODO always use this abstracted copy of this replicated code
    let msg = "block_refreftran_fastspill"
    let ident = funique msg
    let ptr_aid = aid + "." + ident // ident :: aid
    // This call to gec_uav simply creates a singleton to hold the result of the CE_newobj call.  The object storage is not itself allocated until intcil.
    let cez = 
        let (blobf_, cez, vrnl_) = gec_singleton ww vd cCBo msg "reftran_fastspill" arg_ats realdo m0 ident [] arg_dt None (Some false)  None
        cez
    let _ = cg msg (K_as_sassign(kxr, cez, arg, None))
    cez


// The Kiwi.ObjectHandler provides a mechanism to read off the size of an object.
// This is the .size() getter : KiwiSystem.Kiwi.ObjectHandler`1.size()
//   
let canned_ObjectHandler_size_implementation ww self =
    let msg = "ObjectHander.size"
    let rec destar = function
        | CT_star(_, ct) -> destar ct // removes all stars - too fierce  TODO
        | ct -> ct
    let object_handler_size st =
        let bytes = ct_bytes1 ww msg (destar st)
        let _ = vprintln 3 (sprintf "Builtin sizeof in ObjectHandler: returns size=%i bytes for %s" bytes (cil_typeToStr st))
        CE_x(g_canned_i32, xi_num64 bytes)

    match destar(get_ce_type msg self) with
        | CT_cr cst ->
           match cst.cl_actuals with
                | [RN_monoty(st, _)] -> object_handler_size st
                | other -> sf(sprintf "ObjectHandler: (2/2) unexpected arg type %s : other=%A" (cil_typeToStr (CT_cr cst)) other)
        | CTL_record(_, _, _, _, _, [(fid, st)], _) -> object_handler_size st
        | ot -> sf(sprintf "ObjectHandler: unexpected arg type %s" (cil_typeToStr ot))


let g_poly_dispatcher_store = new OptionStore<string list * int, Norm_t>("poly_dispatcher_store")
// Synthesise a shim routine that implements virtual method dispatch by selecting one of several static disptaches.        
// We generate CIL code for the shim routine which is then subject to spill-var and basic block partitioning as usual.
// There is a similar sort of dispatch done for delegates elsewhere.
let gen_poly_dispatcher ww handler_ctx mf n idl that usersubs =
    let my_idl = ("$polydispatch"::idl)
    let key = (my_idl, n)
    match g_poly_dispatcher_store.lookup key with
        | Some ans -> (idl, ans)
        | None ->
            let poly_uid = hptos my_idl
            let scl =
                let l_aidToStr cc = function
                    | A_loaf nemtok -> nemtok::cc
                    | other ->
                        dev_println (sprintf "gen_poly_dispatcher: cannot store aid in BCOND aid=%s" (aidToStr other))
                        cc
                match ce_aid0 mf that with
                    | Some aid -> l_aidToStr [] aid
                    | None     -> []
            let msg = sprintf "poly_dispatcher %s n=%i sc=%s" (hptos idl) n (sfold (fun x->x) scl)
            let ww = WF 3 "gen_poly_dispatcher" ww msg
            let (gid, signat, mdt, disamname) =
                let common cc = function
                    | ((idl, No_method(srcfile, flags, _, (gid, uid, disamname), mdt, flags1, _, atts)), idx) ->
                        // TODO method generics
                       let f2a (rpc_, idx_, dt, fid) = (Some fid, dt)
                       [(gid, map f2a mdt.arg_formals, mdt, disamname)]
                    | _ -> sf ("not a method in " + msg)
                match List.fold common [] usersubs with
                    | [item] -> item
                    | _ -> sf ("cannot create poly dispatcher " + msg)
            let arity = length signat


            let instructions =
                let exit_label = poly_uid + "_exit"
                let aload n = Cili_ldarg(Cil_argexp(Cil_number32 n))
                // Args are pushed on the CIL stack in normal order, starting with this, meaning they will pop in the reverse order.
                let argloads = map aload ([0..arity]) // Include the this pointer
                let glab nn = poly_uid + "_" + i2s nn
                let dispatchx((idl, No_method(srcfile, flags, _, (gid, uid, disamname), mdt, flags1, _, atts)), idx) =
                    let dt = match mdt.meth_parent_class with
                                | (idl, m_cr) -> Cil_cr_idl idl
                    let nextlab = if false && idx=n-1 then exit_label else glab (idx+1)
                    let chainer = [ Cili_lab(glab idx); aload 0; Cili_isinst(true, dt); Cili_brfalse(scl, nextlab) ]
                    let fr = (Cil_cr_idl(tl mdt.name), hd mdt.name, [])
                    let f2a (rpc_, _, dt, fid) = (Some fid, dt)
                    let callsite_token = sprintf "T%s.%i" (hptos idl) idx
                    let call = Cili_call(KM_call, CK_generic (true), mdt.rtype, fr, map f2a mdt.arg_formals, callsite_token) // Call kind will vary on whether a class or instance method 
                    chainer @ argloads @ [ call; Cili_br exit_label ]
                let filler = if mdt.rtype=CTL_void then [] else [ Cili_ldnull ]
                let i0 = list_flatten(map dispatchx usersubs) @ [ Cili_lab(glab n) ] @ filler @ [ Cili_lab exit_label]
                let wrap (i, n) = CIL_instruct(n, handler_ctx, [], i)
                macro_expand_cil_instructions ww None (map wrap (zipWithIndex i0))

            let listing_vd = 3
            let mdt1 =
                {
                  meth_parent_class= mdt.meth_parent_class
                  arg_formals=     mdt.arg_formals
                  name=            my_idl
                  uname=           poly_uid
                  rtype=           mdt.rtype
                  ty_formals=      mdt.ty_formals
                  ataken_formals=  []
                  ataken_locals=   []
                  has_macro=       false
                  has_macro_regs=  false
                  mutated_formals= []
                  gamma=           [] // Correct? If we have method tyformas?
                  is_ctor=               mdt.is_ctor
                  is_root_marked=        None
                  is_accelerator_marked= None
                  is_remote_marked=      None
                  is_fast_bypass=        None
                  is_static=             false
                }
            let flags = [Cilflag_virtual]
            let atts = []
            let flags1 =
                {
                    is_startup_code=false
                }
            let ans = No_method("$synthesised_poly_dispatch$", flags, CK_generic false(*ie static*), (gid, poly_uid, disamname), mdt1, flags1, instructions, atts)
            let ff ss = vprintln listing_vd ss
            ff msg
            ff(noitemToStr true ans)
            ff ("End of " + msg)
            g_poly_dispatcher_store.add key ans
            (idl, ans)


let rec dt_is_struct dt =
    let dt = grossly_simplify_type dt 
    match dt with
    | CTL_record(sq_idl, cst, crt_lst, len_bytes, ats, binds, srco) ->
        //let _ = dev_println (sprintf "dt_is_struct on CTL_record ans=%A"         cst.structf)
        cst.structf

    | other ->
        //let _ = dev_println (sprintf "dt_is_struct false on other =%s" (cil_typeToStr other))
        false

// Case switch between packed structs and unpacked singleton forms (static fields will be unpacked scalars, structs as dynamic class instances and all arrays will be packed).
// Structs are put in heap space as STOREMODE_singleton_vector allegedly to satisify simpleminded repack predicates but that may be fixed now.
let packed_struct_pred dt_record ce =
    if not(dt_is_struct dt_record) then false
    else
        let ii =
            match ce with
                | CE_subsc _                -> true
                | CE_star(_, CE_subsc _, _) -> true                
                | ce ->
                    isIndexable(get_base64 ce) // If indexable then packed.  This code is somewhat wonky - we should be able to get rid of the special vector address region shortly?
        let _ = vprintln 3 (sprintf "isIndexable ans=%A for %s" ii (ceToStr ce))
        ii

// See if a field operation is on a struct and if so collate releated information.
let is_struct_field_op msg ce4 = 
    let is_lhs_struct xtype =
        let ans =
            match xtype with
                | CTL_record (idl, cst, items, len, _, binds, _)             -> if cst.structf then Some 0 else None

// We are wrong to ignore the & in the field insert - it hopefully has a symbolic value and we need to manipulate what it points at instead
                | CT_star(nn, CTL_record (idl, cst, items, len, _, binds, _)) ->
                    //let _ = dev_println (sprintf "Register star nn=%i in field extract/insert code struct?=%s" nn (cil_typeToStr xtype))
                    if cst.structf then Some(nn) else None // Perhaps just remove one star, not all, but &(&(...)) is more advanced than we have tested...
                | _                                              -> None
        let _ = vprintln 3 (sprintf "is_struct_field_op ans=%A for ce=%s  type=%s" ans (ceToStr ce4) (cil_typeToStr xtype))
        ans

                           
    match ce4 with
        | CE_dot(dt_record, l0, (tag_no, fdt, (ptag, utag), dt_field, tag_idx), aid) when packed_struct_pred dt_record l0 ->
            let sfv = is_lhs_struct dt_record
            if not_nonep sfv then Some(true, valOf sfv, ptag, l0, dt_record) else None
        | CE_dot(dt_record, l0, (tag_no, fdt, (ptag, utag), dt_field, tag_idx), aid) when dt_is_struct dt_record -> // singleton/unpacked indirect assign (direct struct singleton field assigns can go via the class field assign code so do not need flagging here)
            let sfv = is_lhs_struct dt_record
            if not_nonep sfv then Some(false, valOf sfv, ptag, l0, dt_record) else None
        | _ ->
            //let _ = dev_println (sprintf "Determined not a field insert for lhs %s of type %s" (ceToStr lhs_ce) (cil_typeToStr(get_ce_type mx lhs_ce)))
            None


let gec_CE_reflection cCC msg (reflection_dt, typeo, idlo, ats) =
    let idl = if not_nonep idlo then valOf idlo else cil_typeToSquirrel (valOf_or_fail "L1835 no reflection type either" typeo) []
    match op_assoc idl !cCC.reflection_constants with
        | Some ov -> ov
        | None ->
            let n = !cCC.next_reflection_int
            let _ = cCC.next_reflection_int := n + 1
            let _ = vprintln 3 (sprintf "allocate next_reflection_int of %i for %s" n (hptos idl))
            let ans = CE_reflection(reflection_dt, typeo, idlo, Some n, ats)
            let _ = mutaddonce cCC.reflection_constants (idl, ans)
            ans

let gen_invoker_dispatcher_shim_method ww cCP cCB handler_ctx callstring callsite msg dis virtualf actionf mdt0 signat' targets =
    let arity = length signat'
    let n_forks = length targets
    let dispatching = n_forks >= 2 // When n_forks >= 2 we need a conditional branch structure.
    let dispo_uid = "SynthesisedInvokeDispatch_" + hptos callstring
    let disamname = dispo_uid
    let gid = [dispo_uid]

    let glab nn = dispo_uid + "_" + i2s nn
    let gps (no, idl) =
        let m_notice = ref None
        let handler = fun(nd:int)->(dis + "\n" + hptos idl + sprintf " : delegate callee not (uniquely) defined.  Number of defs=%i.\n" nd, 1)
        match id_lookup_fn ww cCP cCB handler m_notice virtualf callsite idl signat' with // Look up the actually invoked method via the delegate.
            | [] -> sf(sprintf "no delegate '%s'" (hptos idl))
            | [item] -> // TODO more than one might be ok for dynamic dispatch.
                 (no, idl, item)
    let trips = map gps targets
    let m_static_collate = ref []

    // Args are pushed on the CIL stack in normal order, starting with this, meaning they will pop in the reverse order.
    let that_dto ohead idl gid = // Will vary according to which is called!
        if ohead=0 then None
        else
            // Delete method name from hd of idl to get class name.
            // This record should now be to hand as a dropping etc.
            let that_ty =
                let class_idl = tl gid
                let oc = id_lookupp ww class_idl Map.empty (fun()->("class_idl not found", 1))
                cil_gen_dtable (WN ("class_find gen_dtable") ww) [] (RN_idl(class_idl), valOf_or_fail "class_idl" oc)
            let _ = vprintln 3 (sprintf "STOS downcast to %s" (cil_typeToStr that_ty))
            Some that_ty

    let aload ohead idl gid nn cc =
        let ld = Cili_ldarg(Cil_argexp(Cil_number32 nn))
        //let that = CE_conv(that_ty, that) // We need to downcast the CTL_object from the closure to the relevant callee class,
        // This will defeat mine cgrm - we probably need to here add the method generics in if the callee has any
        let cast = if ohead=1 && nn= 1 then [ Cili_conv(valOf_or_fail "L2459" (that_dto ohead idl gid), false) ] else []
        ld :: cast @ cc

    let gen_full () =
        let exit_label = dispo_uid + "_exit"
        let dispatchy((no, idl, (_, No_method(srcfile, flags, _, (gid, uid, disamname), mdt, flags1, _, atts))), idx) =
            let staticf = memberp Cilflag_static flags // We expect this to be consistent over all callees of course.
            let ohead = if staticf then 0 else 1
            let argloads = List.foldBack (aload ohead idl gid) ([1..arity+ohead]) [] // Include the this pointer but exclude the very first which is the discriminator.
            let _ = mutadd m_static_collate staticf
            let nextlab = if false && idx=n_forks-1 then exit_label else glab (idx+1)
            let load_discriminator = [ Cili_ldarg(Cil_suffix_0); ] // Load the which enumeration integer passed in as additional argument0
            let chainer = [ Cili_lab(glab idx); ] @  load_discriminator @ [ Cili_ldc(g_canned_i32, Cil_argexp(Cil_number32 no)); Cili_bne nextlab ]
            let fr = (Cil_cr_idl(tl mdt.name), hd mdt.name, [])
            let f2a (rpc_, _, dt, fid) = (Some fid, dt)
            let callsite_token = sprintf "T%s.%i" (hptos idl) no
            let call = Cili_call(KM_call, CK_generic(not staticf), mdt.rtype, fr, map f2a mdt.arg_formals, callsite_token)
            chainer @ argloads @ [ call; Cili_br exit_label ]
        let backfiller = if actionf then [] else [ Cili_ldc(g_canned_i32, Cil_argexp(Cil_number32 0x55)) ] // Backstop value should not ever be loaded in reality.
        list_flatten(map dispatchy (zipWithIndex trips)) @ [ Cili_lab(glab n_forks) ] @ backfiller @ [ Cili_lab exit_label ]

    let gen_simple () = // Only one target to call, so no dispatcher needed.
        let dispatch_s(no, idl, (_, No_method(srcfile, flags, _, (gid, uid, disamname), mdt, flags1, _, atts))) =
            let staticf = memberp Cilflag_static flags // We expect this to be consistent over all callees of course.
            let ohead = if staticf then 0 else 1
            let argloads = List.foldBack (aload ohead idl gid) ([1..arity+ohead]) [] // Include the this pointer but exclude the very first which is the discriminator.
            let _ = mutadd m_static_collate staticf
            let fr = (Cil_cr_idl(tl mdt.name), hd mdt.name, [])
            let f2a (rpc_, _, dt, fid) = (Some fid, dt)
            let callsite_token = sprintf "T%s.%i" (hptos idl) no
            let call = Cili_call(KM_call, CK_generic(not staticf), mdt.rtype, fr, map f2a mdt.arg_formals, callsite_token)
            argloads @ [ call ]
        dispatch_s (hd trips)



            
    let i0 = if n_forks > 1 then gen_full () else gen_simple ()
    
    let wrap (i, n) = CIL_instruct(n, handler_ctx, [], i)
    let instructions = macro_expand_cil_instructions ww None (map wrap (zipWithIndex i0))

    let ohead =
        match list_once !m_static_collate with
            | [true]  -> 0
            | [false] -> 1
            | _ -> sf (sprintf "mixed class and instance methods within the %i callee delegates of one Invoke callsite %s. Callees=" n_forks (hptos mdt0.name) + (sfold (snd>>hptos) targets))
    let listing_vd = 3 // Make a listing of this invoker
    let _ = vprintln 3 (sprintf "Synthesised_Invoker n_forks=%i listing_vd=%i" n_forks listing_vd)
    let mdt1 = 
        { mdt0 with
           name=            "Synthesised_Invoker" :: mdt0.name
           uname=           funique "Synthesised_Invoker"
           arg_formals=     (None, None, g_canned_i32, "KiwiCWhichMethod")::(if ohead=1 then [(None, None, g_ctl_object_ref, "that")] else []) @ (map (fun (_, _, dt, id) -> (None, None, dt, id)) mdt0.arg_formals)
           has_macro=       false
           has_macro_regs=  false
           mutated_formals= []
           gamma=           [] // Correct? If we have method tyformas?

        }

    let flags =  [Cilflag_static]
    let atts =   []
    let flags1 = { is_startup_code=false }
    let ans = No_method("$synthesised_invoker$", flags, CK_generic true, (gid, mdt1.uname, disamname), mdt1, flags1, instructions, atts) // was mdt0 !
    let ff ss = vprintln listing_vd ss
    let _ = ff(noitemToStr true ans)
    let _ = ff ("End of " + msg)
    (ohead, (gid, ans))



// Slightly different structs are used for built-in methods compared with user methods and this code converts one to the other, as needed for Invoke.
let bif2mdt (bif:kiwi_bif_fn_t) signat =
    {   name=            bif.uname
        uname=           hptos bif.uname
        rtype=           if nonep bif.rt then CTL_void else valOf bif.rt        // Return type
        arg_formals=     map (fun (dt, no) -> (None, None, dt, sprintf "ps%i" no)) (zipWithIndex signat)    // arg_formal_t list
        ty_formals=      []            // Method generics (mgr)
        meth_parent_class=([], ref None)   //    (stringl * class_struct_t option ref)
        gamma=            [] 
        ataken_formals=   [] // You might thing this needs to be conjunction of all callees dispatched to: but we are generating a shim around those callees which will then be processed on a per-callsite basis, so it is ok.
        ataken_locals=    [] // Ditto
        mutated_formals=  [] // Ditto
        has_macro=        false // Ditto
        has_macro_regs=   false // Ditto
        is_ctor=               constructor_name_pred (hd bif.uname)
        is_root_marked=        None
        is_accelerator_marked= None
        is_remote_marked=      None
        is_fast_bypass=        None
        is_static=             not bif.instancef
    }

    
// Built-in functions: The difference between make_bifcall and bif_dispatch is the former (make_bifcall) generates kcode containing a function application (that may be expanded inside cilnorm or passed on to HPR library) whereas the latter (bif_dispatch) expands the call during intcil processing.
// Built-in functions processed at the CIL stack level are dispatched here.
let bif_dispatch ww vd cCC cCB cCP cf mk_usercall_wrapper cg sp' kxr gb0 msg ncp callsite callstring sitemarker realdo idl textcl m0 dis virtualf bif cg_args mg_args voidf signat' args =
    let cCBo = Some cCB
    let handler_ctx = []
    let ids = hptos idl
    let msg = "bif_dispatch"
    let ww = WN msg ww
    let barrier_gen1 kxr args = function
        | [] ->
            //let _ = cg(K_easc(kxr, CE_apply(g_bif_newregion, []), false))
            let _ = cg msg (K_easc(kxr, CE_apply(g_bif_pause0, cf, []), true))            
            sp'
        | [flagvec] ->
            //let _ = cg(K_easc(kxr, CE_apply(g_bif_newregion, []), false))
            let _ = cg msg (K_easc(kxr, CE_apply(g_bif_pause1, cf, [hd args]), true))            
            sp' (* hard clock cycle count... ignored ... will be munged in restructure anyway *)


    let readin_pragma kxr args =
        let msg = "readin_pragma"
        let nf nn = gen_ce_int32 nn
        let bf = function
            | CE_x(_, mx) -> not(xi_iszero mx)
            | _ -> cleanexit(m0() + " readin_pragma: cannot parse fatal bool")
        let strf = function
            | CE_x(_, W_string(ss, _, _)) -> ss
            | _ -> cleanexit(m0() + " readin_pragma: cannot parse cmd string")
        let (fatalf, cmd, a0, a1) =
            match args with
                | [ fatalf; cmd; ] -> (bf fatalf, strf cmd, nf -1, nf -1)
                | [ fatalf; cmd; a0 ] -> (bf fatalf, strf cmd, a0, nf -1)
                | [ fatalf; cmd; a0; a1 ] -> (bf fatalf, strf cmd, a0, a1)
                | _ -> sf( sprintf "readin_pragma: poor shone %A" args)
        let _ = cg msg (K_pragma(kxr, fatalf, cmd, a0, a1))
        nf 0 // return 0 always for now

        // TODO this may be a struct return - merge in the register colouring branch here please


    // We have two forms of delegate dispatch with different implementations. Compare builtin_hybrid_bif_delegates (test17) with bif_invoke (test55).
    // Here is code Action and Func Invoker that is applied to a delegate whose generic types are the formal types of the callee. If the callee itself has method generics it would be more complex and I have not looked at that.
    let bif_invoke bif =
        let actionf = (bif.hpr_name = "KiwiC-ActionInvoke")
        let (class_dt, return_dt, _, signat, cgr, mgr) = call_type_retrieve ww msg gb0 ncp
        let delegate_type = (fatal_dx bif.hpr_name "L1897-closure" class_dt)
        let delegate_record = hd args
        let retreg =
                if actionf then None
                else
                    let retreg_idl = "return_vreg" :: callstring //
                    let dt:ciltype_t = fatal_dx bif.hpr_name "L2390c2" return_dt 
                    //let vrn = alloc_fresh_vreg ww cCB realdo "return_vreg" dt retreg_idl
                    let ats = []
                    let retreg = 
                        let (blobf_, cez, vrnl_) = gec_singleton ww vd cCBo msg "retreg" ats realdo m0 "" retreg_idl dt None (Some false) None
                        cez
                    Some retreg
                                            
        let that = gec_field_snd_pass ww sitemarker msg delegate_type delegate_record "closure"
        let targets = cCC.dyno_dispatch_methods.lookup callstring
        let arity = length signat'
        let kludge = ref None

// NEW
        let do_new_invdisp() =
          if not_nullp targets then
            let msg = "do_new_invdisp"
            let mdt = bif2mdt bif signat'
            let (ohead, invoker_dispatcher_shim_method) = gen_invoker_dispatcher_shim_method ww cCP cCB handler_ctx callstring callsite msg dis virtualf actionf mdt signat' targets
            let entry_point = gec_field_snd_pass ww sitemarker msg delegate_type delegate_record "entry_point"
            let _ = vprintln 3 (sprintf "RECONCIL entry_point=%s arity=%i" (ceToStr entry_point) arity)
            let extra_dt = g_canned_i32 // The discriminator
            let entry_point = gec_CE_conv ww msg extra_dt CS_typecast entry_point

                        // The delegate function has same return type and signature as its delegate, but a different 'this' ptr.
                
            let xstack_munged = // Modify the stack to suit the invoker dispatch shim.
                let xstack = rev args // CIL calling convention: right-most argument is top of stack. Hence need list reverse here.
                let rec ins_disc_arg n = function // Pull off the actual args and the delegate pointer, then add the entry_point and callee closure, then push them back.
                    | ssp when n=(arity) -> (if ohead=1 then [that] else []) @ entry_point :: (tl ssp)
                    | h::tt -> h :: (ins_disc_arg (n+1) tt)
                    | _ -> sf "L2017 too few args in ins_disc_arg"
                ins_disc_arg 0 xstack
            let ck = CK_generic false // Always a static (aka class) call to the dispatcher.
            let (delegates_stack, _) = mk_usercall_wrapper ww (extra_dt :: (if ohead=1 then [g_canned_object] else [])@signat') (cg_args, mg_args) (None(*shim is static*)) xstack_munged ck invoker_dispatcher_shim_method
            let _ = if actionf then ()
                                else
                                    let rv = match delegates_stack with
                                                | [item] -> item
                                                | a::b::z ->
                                                    let _ = vprintln 0 (msg + sprintf ": ++++more than one rv. hack chose fst no=%i\n%s" (2 + length z) (sfoldcr ceToStr (a::b::z)))
                                                    a // TODO a should be poped
                                                | items -> sf (msg + sprintf ": not one rv. no=%i" (length items))
                                    let _ = kludge := Some [rv]
                                    let rv = gec_yb(xmy_num 4444, g_canned_i32) // TODO explain or delete!
                                    cg msg (K_as_sassign(kxr, valOf retreg, rv, None))
            cg msg (K_remark(kxr, sprintf"dynamic method new dispatch: delegate end %s" (hptos idl)))
// END OF NEW

// TODO say whether this code is also used for  polymorphic dynamic method invoke .

// OLD
        let do_old_invdisp() =
             let usf2 (no, idl) =   // This dynodispatch code is split between intcil delegate invoke and gtrace CE_start procedures. This code used with test55.
                let m_notice = ref None
                let handler = fun(nd:int)->(dis + "\n" + hptos idl + sprintf " : delegate callee not (uniquely) defined.  Number of defs=%i.\n" nd, 1)
                match id_lookup_fn ww cCP cCB handler m_notice virtualf callsite idl signat' with // Look up the actually invoked method via the delegate.
                    | [] -> sf(sprintf "no delegate %s" (hptos idl))
                    | [item] ->
                        let ck = 
                            match item with
                                | (idl, No_method(srcfile, flags, ck, (_, uid, _), mdt, flags1, instructions0, atts)) ->ck
                                | other -> sf (sprintf "Invoke %s not a method but an %A" bif.hpr_name other)
                        let msg = bif.hpr_name + (sprintf ": found one delegate %s" (hptos idl))
                        let ww = WF 3 bif.hpr_name ww msg
                        // This dtable should already be to hand as a dropping etc, but we make it here again for no reason.
                        // TODO - this will defeat mine cgrm - we probably need to add the method generics in if the callee has any. Please check with a test.
                        let that_ty =
                            let class_idl = tl idl  // Delete method name from hd of idl to get class name.
                            let oc = id_lookupp ww class_idl Map.empty (fun()->("class_idl not found", 1))
                            cil_gen_dtable (WN ("class_find gen_dtable") ww) [] (RN_idl(class_idl), valOf_or_fail "class_idl" oc)
                            
                        let that = CE_conv(that_ty, CS_typecast, that) // We need to downcast the CTL_object from the closure to the relevant callee class

                        let _ = vprintln 3 (msg + sprintf ": delegate downcast. That ce=%s" (ceToStr that))
                        // The delegate function has same return type and signature as its delegate, but a different 'this' ptr.
                        let xstack = rev args // CIL calling convention: right-most argument is top of stack. Hence need list reverse here.
                        //vprintln 3 (msg + sprintf ": xstack has %i entries" (length xstack))
                        let (delegates_stack, _) = mk_usercall_wrapper ww  signat' (*mgr*)(cg_args, mg_args) (Some that) xstack ck item
                        let _ = cg msg (K_remark(kxr, sprintf "dynamic method %i dispatch: delegate start " no + hptos idl))
                        let _ = if actionf then ()
                                else
                                    let rv = match delegates_stack with
                                                | [item] -> item
                                                | a::b::z ->
                                                    let _ = vprintln 0 (msg + sprintf ": ++++more than one rv. hack chose fst no=%i\n%s" (2 + length z) (sfoldcr ceToStr (a::b::z)))
                                                    a // TODO a should be poped
                                                | items -> sf (msg + sprintf ": not one rv. no=%i" (length items))
                                    let _ = kludge := Some [rv]
                                    let rv = gec_yb(xmy_num 4444, g_canned_i32) // TODO explain or delete!
                                    cg msg (K_as_sassign(kxr, valOf retreg, rv, None))
                        cg msg (K_remark(kxr, sprintf"dynamic method %i dispatch: delegate end " no  + hptos idl))
                    | _ -> muddy(sprintf "TODO: multiple possible overrides for delegate callstring=%s overrides=%s" (hptos callstring) (hptos idl))
             app usf2 targets
// END OF OLD

        let _ = if true then do_new_invdisp() else do_old_invdisp()

        // The CE_start that is emitted here is a message to keval to augment the dyno_dispatch set at this point and when we come around again under kiterate we will make the dispatch.
        let _ = cg msg (K_easc(kxr, CE_start(false, callstring, class_dt::return_dt::signat, hd args, tl args, textcl.cm_generics), false))
        let _ = vprintln 3 (sprintf "intcil invoke %s  actionf=%A invoked on %i" bif.hpr_name actionf (length args))
        if actionf then sp'
        else
            if nonep !kludge then (valOf retreg)::sp'
            else valOf !kludge

    let ans =
        if idl = ["KiwiDynDispWrap"; "CallSite`1"; "CompilerServices"; "Runtime"; "System"] then
            let nn = 1234567 // For Now - please recode - the reflection token numer is now allocated inside cec_CE_reflection so we dont need silly number here.
            let arg =
                match hd args with
                    | CE_x(_, X_num n)             -> sprintf "DynDispWrap intcond ya int %i" n
                    | CE_apply(bif, cf, [binder_flags; expression_type; typer; operands])  when hd bif.uname= "BinaryOperation" -> sprintf "DynDispWrap binaryoperation f4 %s" (ceToStr operands)
                    | other -> sprintf "OTHER FORM DynDispWrap %A" other 
            let _ = vprintln 0 (sprintf "DynDisp FIX SHORTLY Alloc reflection icon %i for DynDispWrap of %A " nn arg)
            let ss = i2s nn // ce = gec_yb(xi_num nn, g_canned_i32) 
            let result = gec_CE_reflection cCC msg (valOf bif.rt, None, Some [ss], [])
            (result::sp')

        elif bif.hpr_name = "KiwiC-FInvoke" || bif.hpr_name = "KiwiC-ActionInvoke" then bif_invoke bif
          
        elif idl = ["KPragma"; "Kiwi"; "KiwiSystem"] then
            let result = readin_pragma kxr args
            (result::sp')
        elif idl = ["Dispose"; "IDisposable"; "System"] then
           let _ = vprintln 3 (hptos idl + " in Kiwi 2 we shall handle dispose")
           (sp')
        elif idl = ["Start"; "Thread"; "Threading"; "System"] then
            let (class_dt, return_dt, _, signat, cgr, mgr) = call_type_retrieve ww msg gb0 ncp
            let _ = cg msg (K_easc(kxr, CE_start(true, callstring, (class_dt)::return_dt::signat, hd args, [], textcl.cm_generics), true))
            (sp')                                    
        elif idl = ["Equals"; "Object"; "System"] || idl = ["CompareTo"; "Object"; "System"] then
            let result = gec_CE_compare ww (hd idl)  (hd args) (cadr args) 
            (result::sp')                                    
            
        elif idl = ["handleArith"; "ObjectHandler`1"; "Kiwi"; "KiwiSystem"] then
            let result = gec_CE_handleArith ww (hd args) (cadr args) (caddr args)
            (result::sp')                                    

        elif idl = ["size"; "ObjectHandler`1"; "Kiwi"; "KiwiSystem"] then
            let result = canned_ObjectHandler_size_implementation ww (hd args)
            (result::sp')                                    
        elif hd idl="get_ManagedThreadId" then 
            let result = gec_yb(xi_num cCP.ntid, g_canned_i32) // pop1, push1.
            result::sp'
        elif ids = "Microsoft.CSharp.RuntimeBinder.Binder.Convert" then
            let _ = vprintln 0 (sprintf "binder bif: identity on ids=%s args=%s" ids (sfold ceToStr args))
            (hd args) :: sp'

        elif idl = ["ToCharArray"; "String"; "System" ] then            
            // Strings in dotnet are unicode (virtually UTF-16) and chars are 16 bits.
            // Note, a second form with two further args takesa substring slice - not supported.
            let result = hd args // A nop actually. We implicitly convert from string form to pseudo char array when taken off the stack.
            let _ = vprintln 3 (sprintf "handle ToCharArray %i %i " (length args) (length sp')) 
            (result:: sp') 

        elif hd idl="op_Equality" then
            let result = gec_CE_scond(V_deqd, hd args, cadr args)
            (result::sp') 

        elif idl= [ "inHardware"; "Kiwi"; "KiwiSystem" ] then
            let res = xmy_num 1
            let _ = vprintln 3 (sprintf "Intercepted %s and return %s hardcoded." (hptos idl) (xToStr res))
            let yes = gec_yb(res, g_canned_i32)
            (yes :: sp') 
           
        elif idl=g_bif_pause0.uname then barrier_gen1 kxr args signat'

        else // Otherwise, bif_dispatch, non-hardcoded in intcil so pass on to cilnorm that makes the final front end traps for KiwiC or else maps on to an hpr primitive.
            //let _ = vprintln 3 (sprintf "bif2 passon via kcode to cilnorm voidf=%A eisf=%A uname=%s" voidf bif.eis (hptos bif.uname))
            if voidf then
                let _ = cg msg (K_easc(kxr, CE_apply(bif, cf, args), true)) // 
                (sp')
            elif bif.eis then
                let a0 = CE_apply(bif, cf, args)
                let ans = block_reftran_fastspill ww vd m0 cCBo realdo kxr (funique "WASANON") [] cg (valOf_or_fail "bif.rt" bif.rt, a0)
                (ans::sp')
                //sf (sprintf "non-eis bif call has a result - needs uniquify protection. bif=%s arity=%i" (hptos bif.uname) arity)
            else //let rt = if nonep bif.rt then sf (sprintf "bif call requires an argument %s arity=%i" (bif.uname) arity) else valOf bif.rt
                 //let _ = gec_yb(X_undef, rt)
                 (CE_apply(bif, cf, args)::sp')
            // KPragma note: intcil is currently not lazy called so must not flag the pragma here!

    ans

    

//
// 
// Step through some cil code.  Generate kcode using cg emit calls.  The CIL stack is removed.
// It would be helpful to re-evaluate the reachable basic block set as we go since some will be
// trimmed by inHardware() and other constant branch conditions that emerge.        
//
let rec elab_executable_ins ww linepoint_stack fvd realdo (env:annotation_map_t * int) (cCP:cilpass_t, cCB:cilbind_t, dD:cil_methodenv_t, cg) (textcl:cil_cl_textual_t, dynclo:cil_cl_dynamic_t option) linepoint5 hints (sp:cil_eval_t list) handler_ctx (kxr, callsite:callsite_t, ins) =
    let (bindings:(string list * (int * cil_eval_t)) list) = [] // TODO?
    let cCC = valOf !g_CCobject
    let cCBo = Some cCB
    let gb0 = ({ grounded=false; }, textcl.cm_generics) // only used on typecheck pass
    let callstring = (sprintf "CZ:%s:%i" (hptos (fst callsite)) (snd callsite)) :: textcl.cl_codefix
    let vd = if yout_active fvd then 3 else -1
    let ff ss = youtln fvd ss
    let dis = ciliToStr ins
    let msg = realDo realdo  
    let msg1 = msg + ":" + hptos callstring + ":"  + dis
    let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
    let m_final_handlers_invoked = ref None
    (* let _ = if (boring ins) then () else yout(vd, msg1 + "\n") *)
    let _ = WN (msg1) ww

    let (fstpass:firstpass_t, wmd:methodbind_t option) = dD
    let m0() =
        let linepoint51 = { waypoint=ref None;  callstring__ = ref []; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=linepoint_stack; } 
        "QX1581:" + kxrToStr (KXLP5 linepoint51) + " " + msg1 + ":" + hptos fstpass.idl + ":" + dis

    let g1() = if sp = [] then sf(dis + ": no arg on stack") else (hd sp, tl sp)

    let disc1() = if length sp < 1 then sf(dis + ":  not one arg on stack") 
                  else tl sp

    let g2() = if length sp < 2 then sf(dis + ": not two args on stack") 
               else (hd(tl sp), hd sp, tl(tl sp)) 

    let disc2() = if length sp < 2 then sf(dis + ":  not two args on stack") 
                  else tl(tl sp) 

    let disc3() = if length sp < 3 then sf(dis + ":  not three args on stack") 
                  else tl(tl(tl sp)) 

    let g3() = if length sp < 3 then sf(dis + ": not three args on stack") 
               else (hd(tl(tl sp)), hd(tl sp), hd sp, tl(tl(tl sp))) 

    let rec gn n sp c = // unstack n args from the dot net machine's stack.
       if n=0 then (sp, c)
       else
              (
                if (sp= []) then sf(dis + " gn: insufficient args on stack")
                gn (n-1) (tl sp) (hd sp :: c)
               )

    let gn_static   n sp  = gn (n) sp []    
    let gn_instance n sp  = gn (n+1) sp []  // The 'this' ptr is an extra argument.

    // Arith: lower each arg.
    let e2arith ins diop = 
        let (l, r, c') = g2() // Get two args from stack.

        // C#: We have an unsigned result only if both operands are unsigned (even for shifts): 
        // C++: We have an unsigned result if at least one operand is unsigned (even for shifts):       
        // Note C and C# differ :
        (* For the binary +, –, *, /, %, &, ^, |, ==, !=, >, <, >=, and <= operators, the operands are converted to type T, where T is
          the first of int, uint, long, and ulong that can fully represent all possible values of both operands. The operation
          is then performed using the precision of type T, and the type of the result is T (or bool for the relational operators). It
          is not permitted for one operand to be of type long and the other to be of type ulong with the binary operators.

          C# Unary negate applied to int gives long and it may not be applied to long.

          For the binary << and >> operators, the left operand is converted to type T, where T is the first of int, uint, long, and ulong
          that can fully represent all possible values of the operand.  The same goes for unary + and ~.
        *)
        // When doing arithmetic on a signed and an unsigned value, C# converts both to long since this type can hold both the Int32 and UInt32 range.
        // We promote all types narrower than 32 bits to 32 bits: integer_promotion. When signed and unsigned are combined we promote to long.
        // C# int  aka System.Int32 = from -2,147,483,648 to 2,147,483,647.
        // C# long aka System.Int64 = from -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807
        let diadic_promote_and_resolve = function
            | (CTL_net(volf_l, width_l, signed_l, flags_l), CTL_net(volf_r, width_r, signed_r, flags_r)) ->
                let width = if width_l > 32 || width_r > 32 || (width_l=32 && width_r=32 && signed_l=Signed && signed_r=Unsigned) || (width_l=32 && width_r=32 && signed_l=Unsigned && signed_r=Signed) then 64 else 32
                let signed = if signed_l=FloatingPoint || signed_r=FloatingPoint then FloatingPoint
                             elif signed_l=Unsigned || signed_r=Unsigned then Unsigned // Either operand unsigned gives unsigned answer (C++).
                             else Signed
                //let _ = vprintln 0 (sprintf "diadic_promote_and_resolove oo=%A  %i x %i -> %i" diop width_l width_r width)
                CTL_net(volf_l||volf_r, width, signed, nats_resolve flags_l flags_r)

            | (other_l, other_r) ->
                let _ = vprintln 0 (sprintf "+++ precision failure? ::: diadic_promote_and_resolve did not know what to do with %s %A %s" (cil_typeToStr other_l) diop (cil_typeToStr other_r))
                other_l

        let monadic_promote_and_resolve = function
            | CTL_net(volf_l, width_l, signed_l, flags_l) -> 
                let width = if width_l > 32 then 64 else 32
                let signed = if signed_l=FloatingPoint then
                                 let _ = cleanexit(sprintf "Unsupport logical operator %A applied to floating point expression" diop)
                                 signed_l
                             else signed_l
                CTL_net(volf_l, width, signed, flags_l)
            | other_l ->
                let _ = vprintln 3 (sprintf "monadic_promote_and_resolve did not know what to do with %s %A" (cil_typeToStr other_l) diop)
                other_l
              
            
        match realdo with
            | Rdo_tychk dlist ->
                let ct =
                    match ins with 
                        | Cili_mul s_
                        | Cili_add s_
                        | Cili_div s_
                        | Cili_rem s_
                        | Cili_sub s_ ->
                            diadic_promote_and_resolve (simplify_numeric_type(get_ce_type "arith main 1" l), simplify_numeric_type(get_ce_type "arith main r" r))
                        // C# is sensible and for the shifts, only the precision for the lhs operand matters (C++ takes both into account!).
                        // For the arith operators we have a manifest operation style, but its not clear it this is always the return type. This would depend on the C# compiler. We ignore s_ here. 
                        | Cili_shl s_
                        | Cili_shr s_  ->
                            //let _ = if s=Cil_unsigned then Unsigned else Signed
                            monadic_promote_and_resolve (get_ce_type "arith main 1" l)

                        | Cili_and 
                        | Cili_or  
                        | Cili_xor    -> 
                            diadic_promote_and_resolve (get_ce_type "e2arith bit 1" l, get_ce_type "e2arith bit r" r)
                log_tval ww msg dlist callstring (gec_RN_monoty ww ct)
                CE_typeonly(ct)::c'
            | _ ->
                let ct = mono_type_retrieve ww msg gb0 callstring 
                gec_CE_alu ww (ct, diop, l, r) :: c'



    // Replacement for HSIMPLE invoker - pass on to restructure to implement.
    let invoke_subsidiary_ip ww (protocol, externally_instantiated_call, fsems) block_kind sq instance_name method_name hpr_ids (rt:ciltype_t) args =  
        let arity = length args          // Overload on arg count for now but not types TODO.
        //let block_kind = [instance_name] // for now - no farming/mirroring here - see restructure for that.
        
        let _ = WF 3 "invoke_subsidiary_ip" ww (sprintf "(via CE_apply) Start method=%s block=%s arity=%i" method_name (hptos block_kind) arity)
        // Read in all the methods and select the overload
        let (fu_meld, (gis, ret_details, variable_latency, arg_details), externally_instantiated_callee, mirrorable_) = protocols.assoc_external_method ww msg block_kind method_name sq arity (Some externally_instantiated_call)

        let eee = externally_instantiated_call || externally_instantiated_callee

        let cf = { g_null_callers_flags with externally_instantiated_block= eee; loaded_ip_=true }

        let autobif =
            { g_baseline_bif with
                arity=       arity
                uname=       [method_name]
                rt=          if rt = CTL_void then None else Some rt // rt__
                hpr_name=    hpr_ids
                hpr_native=  Some gis // Don't need this and hpr_name etc...
//       nonref=false  // Non-referentially transparent: calling it more often than intended is wrong *)
//       yielding=false  // aka blocking: Values of volatile variables may be changed meantimes. *)
                //is_loaded=                 Some(block_kind, instance_name) // Function is provided by a structural resource (here loaded from an IP-XACT file).
            }

        let yielder (name, gis) = 
            let ans = CE_apply(autobif, cf, args)
            // Get gis details from imported block table (which has come from parsing IP-XACT)
            //let _ = dev_println (sprintf "yielder ee_call=%A ee_callee=%A %s"  externally_instantiated_call externally_instantiated_callee (ceToStr ans))
            ans
            
        vprintln 3 (sprintf "Invoke subsidiary IP block: block kind=%s nominal instance=%s method=%s args=%s overloaded_disam=%A\nInstance name may yet be be further refined by mirroring and load balancing." (hptos block_kind) (instance_name) method_name (sfold ceToStr args) fsems.fs_overload_disam)
        yielder (hpr_ids, gis)
        
    // Bypass methods are built-in functions that are denoted as such through being attributed, such as with FastBitConvert.
    let invoke_hpr_fast_bypass idl ids ids_wot rt arg =  
        let cf = g_null_callers_flags
        let ids = map snd ids_wot
        let yielder (name, gis) = 
            let autobif =
                { g_baseline_bif with
                   arity=     1
                   uname=     [name]
                   rt=        Some rt
                   hpr_name=  name
                }
            let _ = vprintln 3 (sprintf "Made a fast bypass of %s to %s" (sfold xToStr ids) name)
            CE_apply(autobif, cf, [arg])
        match (rt, get_ce_type "invoke_hpr_fast_bypass" arg) with
            | (CTL_net(_, 64, FloatingPoint, _), CTL_net(_, 64, Unsigned,_))
            | (CTL_net(_, 64, FloatingPoint, _), CTL_net(_, 64, Signed,_)) ->
                yielder g_hpr_bitsToDouble_fgis

            | (CTL_net(_, 64, Unsigned, _), CTL_net(_, 64, FloatingPoint,_))
            | (CTL_net(_, 64, Signed, _), CTL_net(_, 64, FloatingPoint,_)) ->
                yielder g_hpr_doubleToBits_fgis

            | (CTL_net(_, 32, FloatingPoint, _), CTL_net(_, 32, Unsigned,_))
            | (CTL_net(_, 32, FloatingPoint, _), CTL_net(_, 32, Signed,_)) ->
                yielder g_hpr_bitsToSingle_fgis

            | (CTL_net(_, 32, Unsigned, _), CTL_net(_, 32, FloatingPoint,_))
            | (CTL_net(_, 32, Signed, _), CTL_net(_, 32, FloatingPoint,_)) ->
                yielder g_hpr_singleToBits_fgis

            | (rt, argt) -> cleanexit(m0() + sprintf ": Cannot invoke an hpr_fast_bypass on %A: unsupported argument/return combination: return_type=%s  arg_type=%s  arg=%s" ids (cil_typeToStr rt) (cil_typeToStr argt) (ceToStr arg))

    let remark kxr s = (vprintln 3 ("emit remark " + s); cg msg (K_cmd(kxr, Xcomment s)))

    let add_labcontext id = id :: cCB.codefix //Localfix is unique for each callsite that is not on the stack more than once, but here we need the globally flat callsite name for when the code gets flattened into a VM dic. hence use codefix?  Also, when a finally block (that can contain  more than one basic block) is used more than once with different final destinations, owing to multiple exit routes from a try block, which is typical common case, we need a different labcontext for each inline of the finally block

    let branchcond swap diop = 
        let (l, r, c') = g2() // Get two args from stack.
        let ans = if swap then (diop, r, l) else (diop, l, r)
        (ans, c')

    let rec mymonkey pol = function
        //| CE_tv(t, v) -> mymonkey pol v
        | x when ce_constantp x ->
            let v = ce_manifest_int64 "mymonkey" x
            if v = 0L then Some pol
            else Some(not pol)

        | _ -> None


    // If transferring to a different (always higher) try nesting, we are longjumping and must likely interpose the code for some finally blocks.
    let emit_k_goto kxr gg lab =
        let _:handler_token_t list = handler_ctx
        let dx_lab = add_labcontext lab
        let redirecting =
            if nullp handler_ctx then false
            else
                match op_assoc lab fstpass.labs_si with
                    | None -> sf ("emit_K_goto_missing label lab " + lab)
                    | Some dx ->
                        let (dest_ctx, insl_) = fstpass.ia.[dx]
                        let finallys_tokens_needing_torun =
                                let remove_still_surrounding lst dest = lst_subtract lst [f1o3 dest]
                                List.fold remove_still_surrounding handler_ctx dest_ctx
                        let _ = vprintln 3 (sprintf "kxr=%A emit_K_goto longjump check dest=%s/%i in %A -  %A. Final blocks needing processing=%A" msg1 lab dx handler_ctx (sfold f1o3 dest_ctx) finallys_tokens_needing_torun)

                        if nullp finallys_tokens_needing_torun then false
                        else
                            let finallys_needing_torun =
                                let lookup_handler token = valOf_or_fail "missing finally handler" (op_assoc token fstpass.exn_handler_directory)
                                map lookup_handler finallys_tokens_needing_torun
                            m_final_handlers_invoked := Some(rev finallys_needing_torun, lab)  // We will need to the innermost one first.
                            true
        if not redirecting then cg msg (K_goto(kxr, gg, dx_lab, ref -1))  
        
    let emit_cbranch kxr lab cond =
        let tail gg = emit_k_goto kxr gg lab
        match cond with
        | None               -> tail None
        | Some(pol, ce, scl) ->
            match mymonkey pol ce with
                | None ->         tail (Some(BCOND_guarded(pol, ce, scl)))
                | Some true ->
                    let _ = remark kxr ("A conditional branch to " + lab + " is now an unconditional jump")
                    tail None // unconditional branch
                | Some false ->   remark kxr ("Site of a conditional branch to " + lab)

    let c1branch kxr scl negf lab = 
        let (v, c') = g1()
        // On tychk_pass_pred realdo we are in program order so doing nothing is fine  ?
        let _ = if not(tychk_pass_pred realdo) then emit_cbranch kxr lab (Some(negf, v, scl))
        (c', env)


    let scond kxr swap diop = 
        let (cond, c') = branchcond swap diop
        let ans = //if realdo=Rdo_true then gec_yb(xi_blift(branchcond_lower cond), g_canned_bool)
                  if tychk_pass_pred realdo then gec_yb(X_undef, g_canned_bool)
                  else gec_CE_scond cond
        (ans :: c', env)


    let emit_cbranch2 kxr lab cond = emit_k_goto kxr cond lab // please remove this nop shim!
                  
    let cbranch kxr swap diop lab = 
        let (cond, c') = branchcond swap diop
        let _ = emit_cbranch2 kxr lab (Some(BCOND_diadic cond))
        (c', env)


    let c_switch kxr lst = 
        let (v, c') = g1()

        let rec gen kxr q = function
            | [] -> ()
            | lab :: tt ->
                let ce = gec_CE_scond(V_deqd, v, gec_yb(xi_num q, g_canned_i32))
                emit_cbranch kxr lab (Some(false, ce, []))
                                      // let dest = add_labcontext lab
                                      // cg msg (K_bgoto(Some(false, ce), dest, ref -1))
                gen kxr (q+1) tt
                         
        // On tychk_pass_pred realdo we are in program order so doing nothing is fine  ?
        let _= if tychk_pass_pred realdo 
               then ()
               else gen kxr 0 lst
        (c', env)
                                 



// We scan forward on create array and newobj to collect the attributes
// of the field their handle is first stored in.... bit of a hack, but
// this is helpful for comprehensible naming and also vital for offchip and hardware servers and so on.
    let get_hintname def arg =
        let hvd = -1 // Set to -1 for debugging off.
        let get_hintname2 staticfieldf fr =
            let envo = None
            let (cr, idl) = fr33 ww true textcl.cm_generics fr
            if not staticfieldf then
                if hvd>=4 then vprintln 4 (sprintf "hintname too hard: using default %s" (fst def))
                def       //  Too hard .... really?
            else
                match singleton_locate ww cCB false realdo m0 textcl.cm_generics envo idl with // cgr is in CCL quite often with
                    | None ->
                        if hvd>=4 then vprintln 4 ("+++hintname: Field not defined: " + hptos idl)
                        ("noh" + fst def, snd def)
                    | Some she ->
                        if hvd>=4 then vprintln 4 ("hintname succeeded for " + hptos idl + " : Cez=" + ceToStr she.cez)
                        let ats = cez_ats she.cez
                        (hd idl + "_", ats)

        let rec get_hintname1 = function
            | (ins0, (CIL_instruct(_, _, _, Cili_stsfld(_, fr))::_)) -> (get_hintname2 true fr)
            | (ins0, (CIL_instruct(_, _, _, Cili_stfld(_, fr))::_))  -> (get_hintname2 false fr)
            | (ins0, (CIL_instruct(_, _, _, Cili_stloc _)::_))       ->
                if hvd>=4 then vprintln 4 (sprintf "hintname for local var fails: using default %s" (fst def))
                def (* no atts on a local, at least until we read int the aux meta file from the C# compiler. *)

            //newarr encountered in stream is next one, so must find it in ins0. Need to stop on newobj too! 
            | (Cili_newarr cr, CIL_instruct(_, _, _, Cili_newarr _)::t) -> (hd(rn_valOf "get_hintname1" (fst(cr2idl ww cr))), [])

            // Ditto - find a name hint for a new object from common CIL patterns
            | (Cili_newobj(flags, ck, tt, (cr, id, gargs), signat, site_), CIL_instruct(_, _, _, Cili_newobj _)::t) -> (hd(rn_valOf "get_hintname1a" (fst(cr2idl ww cr))), [])

            | (ins0, CIL_instruct(idx, _, _, Cili_lab _)::t) -> hn_scan(ins0, t)     
            | (ins0, _::t)                                   -> hn_scan(ins0, t)

        and hn_scan = function
            | (ins, []) -> ("-1stpass-NOHINT-", [])  // This happens on 1st pass: causes a problem ?
            | (ins, h::t) ->
                if hvd>=4 then vprintln 4 ("get hintname scan " + cil_classitemToStr "" h "")
                get_hintname1(ins, h::t)

        let ans = hn_scan arg
        if hvd>=4 then vprintln 4 (sprintf "get_hintname result is  %s" (fst ans))        
        ans

    let ra_end stack ans = 
            (mutadd stack (gec_yb(ans, g_canned_i32 (* for now *)));
             env
            )

    let romarray_read__ it = function
        | (stack, [a1; a2]) -> ra_end stack (xi_apply(hpr_native "romarray", muddy "you need to make this a system-romarray application: [ LL(f3o3 a1); LL(f3o3 a2)]"))
        | _ -> sf "rom array read wrong args2"                                 


    // Some OLD builtin vararg function handling...
    let obuf_end ans rt voidf sp' = if voidf then (muddy "cil_emit (Xeasc ans)"; sp')  else (gec_yb(ans, rt)):: sp' 
    let buf_v(idl, argcnt, (sp, args), rt, voidf) =
        if vd>=4 then vprintln 4 ("Calling builtin vararg function " + hptos idl)
        let ans = if tychk_pass_pred realdo then X_undef (* for now *) (*CE_typeold rt*)
                  elif idl=["NeverReached"; "Kiwi" ] then sf ("Kiwi.NeverReached was reached args=(" + sfold ceToStr args + ")\n")
                  else xi_apply(hpr_native (valOf(builtin_v idl)), muddy "style_sel args")
        (obuf_end ans rt voidf sp, env)                  



    // Here we avoid a full disposal of potentially side-effecting calls. 
    // Do we ? we emit raw calls but not if nested in expression for some reason.
    // We should check the gis flags...
    // Currently, side effecting calls with void result are handled elsewhere.  where ?*)
    let rec tidy_disposal kxr arg =
        if vd>=4 then vprintln 4 ("Doing tidy disposal of " + ceToStr arg)
        let cg_tidy e =
            if tychk_pass_pred realdo then () // only called for genuine gtrace forms
            else cg msg (e)
        match arg with
            | CE_start _ 
            | CE_apply _ -> cg_tidy (K_easc(kxr, arg, true))
            | CE_x(_, W_apply(a, cf, ar, m)) ->
                let cmd = Xeasc(W_apply(a, cf, ar, m))
                if tychk_pass_pred realdo then ()
                //elif realdo=Rdo_true then cil_emit cmd
                else cg msg (K_cmd(kxr, cmd))                

            | other -> ()

    let vd = !g_firstpass_vd



    let mk_spill_var2 ww (d_D:firstpass_t) n idl rhs =
        let mm = "mk_spill_var2"
        let dt = get_ce_type mm rhs
        let ff ss = vprintln (*a1*)vd ss
        let dtidl = ct2idl ww dt
        let is_string = dt_is_string dt
        //vprintln 0 (mm + sprintf "  s=%A c=%A" (is_string) (dt_is_char dt))
        let ats = [("compiler-temp", "true")] @ (if is_string then [("is-string", "true")] else [])
        //let dt = if is_string then g_string_prec else dt
        let ww = WF 3 mm ww (i2s n) 
        let cez = 
            match d_D.spillage.[n-g_spillidx] with
                | SP_none -> sf "spill zygote expected"
                | SP_zygote idl ->
                    let ids = hptos_net idl
                    let mm = "mk_spill_var2 " + ids
                    let uid = 
                       let length = ct_bytes1 ww mm dt
                       Some{ f_name=idl; f_dtidl=dtidl; baser=g_ssm_spillbase + int64 n * 10L; length=Some length; }
                    //let _ = if (*a1*)vd then ff(mm + sprintf ": Insert spill type %i for " n + typeinfoToStr dt)
                    let cez =
                        let fix = idl
                        let (blobf_, cez, vrnl) = gec_singleton ww (*a1*)vd cCBo mm "spillvar" ats realdo (fun _ -> "L2757") "" fix dt  None (Some false) None
                        cez
                    // Using the _full ceToStr here crashes mono 4.7
                    //let _ = vprintln 0 (sprintf "bad cez %A" cez)
                    let ss  = ceToStr_simple cez
                    //let _ = vprintln 0 (sprintf "bad cez formatted")                    
                    if vd>=4 then ff(mm + sprintf "Made spill %i " n + ss + " for " + hptos idl)
                    cez
        Array.set d_D.spillage (n-g_spillidx) (SP_full cez)
        cez

    // Some built-in functions (bifs) are handled during intcil and others in kcode form within cilnorm. Yet other are passed down the recipe in hpr_x form and are methods provided on external IP blocks.

    let ncf = g_null_callers_flags
    let instance_builtin0 stack voidf (env) cf (fid:kiwi_bif_fn_t, ct, self) =
        let ce =
            match fid.uname with
                | "ToString"::_              -> gec_ce_string (if true then ceToStr self else "<ToString-opaque>")
                | "ToCharArray"::"String"::_ -> self (*This is a NOP at the moment since we are using char arrays forst strings throughout and not unicode arrays. *)
                | "get_Length"::"String"::_  -> CE_apply(fid, ncf, [self]) // c.f. ldlen instruction


                                             
                | idl -> sf("unknown built-in function or property (arity 0 applied to instance) name=" + hptos idl) // Could pass on but perhaps meaningless since intrinsically non-OO beyond here.
        (mutadd stack ce; (env))

    let instance_builtin1 stack voidf (env) cf (fid, ct, self, a0) = // Pass on
        (mutadd stack (CE_apply(fid, cf, [a0])); (env))

    let instance_builtin2 stack voidf (env) cf (fid, ct, self, a0, a1) = // Pass on
        let _ = vprintln 3 (sprintf "builtin instance call arity2 %A %A %A %A "  self id a0 a1)
        (mutadd stack (CE_apply(fid, cf, [a0])); (env)) // all rather a NOP at this point!


    let s_builtin0 stack voidf (e) cf fid ct =
        (mutadd stack (CE_apply(fid, cf, [])); (env))

    let s_builtin1 stack voidf (e) cf fid ct a0 =
        (mutadd stack (CE_apply(fid, cf, [a0])); (env))

    let s_builtin2 stack voidf (e) cf fid ct (a, b) =
        (mutadd stack (CE_apply(fid, cf, [a; b])); (e))

    let s_builtin3 stack voidf (e) cf fid ct (a, b, c) =
        (mutadd stack (CE_apply(fid, cf, [a; b; c])); (e))        

    let s_builtin4 stack voidf (e) cf fid ct (a, b, c, d) =
        (mutadd stack (CE_apply(fid, cf, [a; b; c; d])); (e))

        

    // There are two forms of delegate dispatch with different implementations. Compare builtin_hybrid_bif_delegates (test17) with bif_invoke (test55).
    let builtin_hybrid_bif_delegates ww cCL' ncpo mname handler_ctx sitemarker actionf virtualf args signat' (cg_args, mg_args) mdt ck =
        let (ncp, mk_usercall_wrapper) = valOf_or_fail "missing ncp L2386" ncpo
        let (class_dr_dt, return_dt, _, signat, cgr, mgr) = call_type_retrieve ww msg gb0 ncp
        let delegate_type = fatal_dx mname "L2397-delegate-type" class_dr_dt
        let class_dt = fatal_dx mname "L2397a-delegate-type" class_dr_dt // TODO not the same?
        let arity = length signat'

        let delegate_record = valOf_or_fail "L2398-delegate invoke" cCL'.this
        if vd>=4 then vprintln 4 (msg  + sprintf ": STOS hybrid actionf=%A arity=%i delegate=%s" actionf arity (ceToStr delegate_record))
        //vprintln 4 (msg  + sprintf ": STOS signat' = %s" (sfold cil_typeToStr signat'))
        let retreg___ = // just side effecting?
            if actionf then None
            else
                let retreg_idl = "return_vreg" :: callstring // This return register may not be needed owing to spill vars being used now on exit confluence. Please check.
                let dt:ciltype_t = fatal_dx mname "L2390c1" return_dt
                let ats = []
                let retreg =
                    let (blobf_, cez, vrnl_) = gec_singleton ww (*a1*)vd cCBo msg "delegate_retreg" ats realdo m0 "" retreg_idl dt None (Some false)  None
                    cez
                Some retreg

        // The closure and entry_point fields are inherited from MulticastDelegate

        let that = gec_field_snd_pass ww sitemarker msg delegate_type delegate_record "closure"
        let targets = cCC.dyno_dispatch_methods.lookup callstring
        let m_kludge = ref(Some(gec_yb(xi_num 4444, g_canned_i32))) // This value 'returned' when no delegatees are so far collated at this site.


        if nullp targets then
            if vd>=4 then vprintln 4 (msg  + sprintf ": hybrid actionf=%A  no callable_methods_at_this_site_so_far" actionf)
            ()
        else
            let (ohead, invoker_dispatcher_shim_method) = gen_invoker_dispatcher_shim_method ww cCP cCB handler_ctx callstring callsite msg dis virtualf actionf mdt signat' targets
            let entry_point = gec_field_snd_pass ww sitemarker msg delegate_type delegate_record "entry_point"
            if vd>=4 then vprintln 4 (sprintf "BAGGOT entry_point=%s arity=%i" (ceToStr entry_point) arity)
            let extra_dt = g_canned_i32 // The discriminator
            let entry_point = gec_CE_conv ww msg extra_dt CS_typecast entry_point
                
            let xstack_munged = // Modify the stack to suit the invoker dispatch shim.
                let rec ins_disc_arg n = function // Pull off the actual args and the delegate pointer, then add the entry_point and callee closure, then push them back.
                    | ssp when n=(arity) -> (if ohead=1 then [that] else []) @ entry_point :: (tl ssp)
                    | h::tt -> h :: (ins_disc_arg (n+1) tt)
                    | _ -> sf "L2632 too few args in ins_disc_arg"
                ins_disc_arg 0 sp
            vprintln 3 (sprintf "BAGGOT ohead=%i munged stack is " ohead + sfoldcr_lite ceToStr xstack_munged)
            let ck = CK_generic false // Always a static (aka class) call to the dispatcher.
            let (delegates_stack, _) = mk_usercall_wrapper ww (extra_dt :: (if ohead=1 then [g_canned_object] else [])@signat') (cg_args, mg_args) (None(*shim is static*)) xstack_munged ck invoker_dispatcher_shim_method

            let _ =
                if actionf then ()
                else
                    let rv =
                        match delegates_stack with
                        | [item] -> item
                        | a::b::z ->
                            hpr_yikes (msg + sprintf ": ++++more than one rv for dispatched invoker. hack chose fst no=%i\n%s" (2 + length z) (sfoldcr ceToStr (a::b::z)))
                            a // TODO a should be popped? Or is delegates stack junk now?
                                            | [] -> sf (msg + sprintf ": no invoker rv when expected")
                    m_kludge := Some rv
                    ()

            ()
        // The CE_start that is emitted here is a message to keval to augment the dyno_dispatch set at this point and when we come around again under kiterate we will make the dispatch.
        cg msg (K_easc(kxr, CE_start(false, callstring, class_dr_dt::return_dt::signat, delegate_record, args, textcl.cm_generics), false))
        //if vd>=4 then vprintln 4 (sprintf "intcil FInvoke (as per test17) invoked on %A" args)
        if actionf then None
        else
            if not_nonep !m_kludge then !m_kludge
            else sf "L2452 - need a retreg answer"


    let make_hybrid_bifcall cCL' cCB idl ncpo item stack signat' actuals handler_ctx sitemarker virtualf (cg_args, mg_args) env = 
        let cf = g_null_callers_flags
        match item with
        | No_method(srcfile, flags, ck, (gid, uid, _), mdt, flags1, instructions0, atts) ->
            let voidf = (mdt.rtype = CTL_void)
            let staticf = memberp Cilflag_static flags
            let instancef = not staticf
            let mname = hd idl
            let ids = hptos idl
            let arity = length actuals
            let lookup_arity = if instancef then arity-1 else arity
            if vd>=4 then
                vprintln 4 ("intcil hybrid_bifcall " + ids + " rtype=" + typeinfoToStr mdt.rtype)
                vprintln 4 ("intcil hybrid_bifcall " + ids + " with " + i2s(length(actuals)) + sprintf " actuals, arity=%i staticf=%A" arity staticf)
                vprintln 4 ("intcil hybrid_bifcall " + ids + " with stack " + i2s(length(!stack)) + " items")
            let ans =
                match hd idl with
                    | "Invoke" ->
                    //| "KiwiC-ActionInvoke" // The KiwiC-prefix versions are created within kiwi_canned
                    //| "KiwiC-FInvoke"  ->
                        let actionf = voidf // (mname = "KiwiC-ActionInvoke")
                        builtin_hybrid_bif_delegates ww cCL' ncpo mname handler_ctx sitemarker actionf virtualf actuals signat' (cg_args, mg_args) mdt ck
                    | other -> sf (msg + sprintf ": L2474: other hybrid built-in method  unrecognised '%s'" other)
            let _ = if nonep ans then () else stack := [valOf ans]
            Some (env) // unmodified always - not used in intcil anymore?

    // Built-in functions: The difference between make_bifcall and bif_dispatch is the former (make_bifcall) generates kcode containing a function application (that may be expanded inside cilnorm or passed on to HPR library) whereas the latter (bif_dispatch) expands the call during intcil processing.
    let make_bifcall cCL' cCB cf idl (item, stack, actuals) =
        match item with
        | No_method(srcfile, flags, ck, (gid, uid, _), mdt, flags1, instructions0, atts) ->
            let voidf = (mdt.rtype = CTL_void)
            let staticf = memberp Cilflag_static flags
            let (instancef, cpred_) = (not staticf, constructor_name_pred (hd idl))
            let dt = mdt.rtype
            let prefix = idl @ [] (*  #prefix CN need class name *) 
            let ids = hptos idl
            let arity = length actuals
            // Constructors are not labled as static, 
            let lookup_arity = if instancef && mdt.is_ctor=0 then arity-1 else arity
            match g_kiwi_canned_bif.TryFind (idl, instancef, lookup_arity) with
                | None -> None

                | Some fid ->
                    let ans = 
                       let arity_ = length mdt.arg_formals
                       let a' = map (f3o3) actuals
                       if vd>=3 then
                           vprintln 4 ("intcil bif " + ids + " rtype=" + typeinfoToStr mdt.rtype)
                           vprintln 4 ("intcil bifl " + ids + " with " + i2s(length(actuals)) + " actuals, arity=" + i2s arity + " staticf=" + boolToStr staticf + " staticf=" + boolToStr staticf)
                           vprintln 4 ("intcil bif " + ids + " with stack " + i2s(length(!stack)) + " items")

                       if not staticf && cCL'.this = None then sf("no object for instance call:" + dis)
                       if staticf && arity=0   then s_builtin0 stack voidf (env) cf fid dt
                       elif staticf && arity=1 then s_builtin1 stack voidf (env) cf fid dt (hd a')
                       elif staticf && arity=2 then s_builtin2 stack voidf (env) cf fid dt (hd a', hd(tl a'))
                       elif staticf && arity=3 then s_builtin3 stack voidf (env) cf fid dt (hd a', hd(tl a'),  hd(tl(tl a')))
                       elif staticf && arity=4 then s_builtin4 stack voidf (env) cf fid dt (hd a', hd(tl a'),  hd(tl(tl a')), hd(tl(tl(tl a'))))
                       elif lookup_arity=0 then instance_builtin0 stack voidf (env) cf (fid, dt, valOf(cCL'.this))
                       elif lookup_arity=1 then instance_builtin1 stack voidf (env) cf (fid, dt, valOf(cCL'.this), hd a')
                       elif lookup_arity=2 then instance_builtin2 stack voidf (env) cf (fid, dt, valOf(cCL'.this), hd a', hd(tl a'))
                       else sf(sprintf "static=%A prefix=%s " staticf (hptos prefix) + ": bad KiwiC built-in/primitive method - arity no good " + i2s(arity))
                    Some ans

              
    let log_call_types ww m1 dlist callstring class_dt tyvars cgr mgr rt args =
        let msg = msg + ":" + m1
        let qqy = qqx_a ww msg tyvars
        let rt1 = qqy rt
        log_tval ww msg dlist callstring (RN_call(qqy class_dt, rt1, [], map (fun d ->qqy (get_ce_type "log_call_type" d)) args, cgr, mgr))
        rt1

    let mk_usercall ww signat_types ncpo realdo linepoint_stack (sp00, (newobjf, thato_ty_called_class, thato), voidf, virtualf, ck, return_ct, (idl, usersub), mgr, ats0) = 
        let cf = g_null_callers_flags
        //lprint 1000 (fun()->"Inlining method call to " + noitemToStr true usersub)
        let signat' = (signat_types:ciltype_t list) 
        let ids = hptos idl
        //let cgr_msg = "cgr=?" // missing?   cgr is in cst.cl_actuals of called_class
        let mgr_msg =
            if nullp mgr then " mgr=[]" else " mgr=<" + sfoldcr_lite (fun (ty, dr) -> cil_typeToStr ty + "    " + drToStr dr) mgr + "> "
        let msg = sprintf "mk_usercall to %s called_class=%s" ids (cil_typeToStr thato_ty_called_class) +  (if nonep thato then "" else sprintf " (using thato: that=%s as this)" (ceToStr ((valOf thato))) + mgr_msg)
        let linepoint_stack = msg::linepoint_stack
        if vd>=4 then vprintln 4 ("linepoint_stack augmented to "  + sfoldcr (fun x->x) linepoint_stack)
        let ww = WF 3 (if tychk_pass_pred realdo then "typecheck" else "gtrace") ww msg

        let m_parental_spr = ref sp00
        // Args are eval'd in and pushed on the CIL stack in normal order, meaning they will pop in the reverse order.
        let getarg() = if nullp !m_parental_spr then sf(hptos idl + ": getarg: too few args on stack")
                       else let rr = hd !m_parental_spr
                            m_parental_spr := tl !m_parental_spr
                            rr
        let (uid, disambname_, mdt) =
            match usersub with
                | No_method(srcfile, flags, ck, (gid, uid, disambname), mdt, flags1, instructions, atts) -> (uid, disambname, mdt)
                | _ -> sf "L3080"
        let (instancef, ohead) = thisohead(virtualf, ck)

        if vd>=4 then vprintln 4 (sprintf "mk_usercall: %s   is_ctor=%i Arity=%i instancef=%A" ids mdt.is_ctor (length signat_types) instancef + ", ohead=" + i2s ohead + sprintf " non-thatocall=%A" (nonep thato))

#if SPARE        
        let _ =
            if mdt.is_ctor=0 && ohead=0 && length idl > 1 then
                let classname = tl idl
                match muddy "op_assoc classname !cCC.m_classes_where_statics_constructed" with // Note: despite being static they can still have overloads on their template args.
                | None ->
                    if vd>=4 then vprintln 4 (sprintf "Request static_classes_constructed constructor needed for class=%s associated with usercall %A" (hptos classname) (hptos idl))
                    muddy " mutadd cCC.m_classes_where_statics_constructed (classname, (uid, 0, ref false))"
                | Some existing_ -> ()
#endif
        let actuals =
            let rec argzip = function
                    | [] -> []
                    | ca::tt -> 
                        let b = (ca, LOCAL, getarg())
                        if vd>=4 then vprintln 4 (" call formal=" + cil_typeToStr ca + ",  call actual=" + ceToStr(f3o3 b))
                        b::(argzip tt)
            rev(argzip (rev signat_types)) // Reverse since cons created reversed list. Reverse signat too, since we will pop args in reverse order.

        let rec rv k = function
            | [] -> []
            | h::t -> (xi_num k, snd h) :: rv (k+1) t

        let (called_class, dest_obj) =
            if not_nonep thato then (thato_ty_called_class, thato) 
            elif ohead=1 then (thato_ty_called_class, Some(getarg())) // pop 'this' ptr after actuals
            else
                //vprintln 0 (sprintf "silly_dx was dual none thato_ty_called_class=%s" (cil_typeToStr thato_ty_called_class))
                (thato_ty_called_class, None)

        //The printout of 'this' can be vast
        //if not_nonep dest_obj then vprintln 0 (sprintf "OO-poly  realdo=%s, called this=%s" (realDo realdo) (ceToStr(valOf dest_obj))) else vprintln 5  (" static call")
        let (prefix, codefix, localfix, previously) = methodbody_namemake callsite uid idl cCP
        if vd>=4 then vprintln 3 (sprintf "%s: Previous call count was %i " ids previously + sprintf ". Prefix=%s localfix=%s" (hptos codefix) (hptos localfix))
        mutadd (cCP.expanded_items_) idl

        let rlab = funique("returner")
//      youtln vd ("TRC1 this=" + (if cCL.this =None then "None" else ceToStr(valOf cCL.this)))

        let cCB'  = {  (* This is the second time this CB is made for a call, but would not have had a dest obj the first time if we are calling a constructor. *)
                         cCB with
                      //methodgenerics=mgr // why commented out ?
                          codefix=codefix
                          simple_localfix=localfix
                          fieldat=ref None 
                          cpoint= ref None
                          tailhangf= false
                          retlab=(false,  rlab)
                          local_vregs_used=  ref []
                   } 
        let (cm_generics, remote_marked) =
            let mgr_text =
                let mgr_bind (mm:annotation_map_t) = function
                    | (CTVar ctv, RN_monoty(dt, ats)) when not_nonep ctv.id -> mm.Add ([valOf ctv.id], RN_monoty(dt, ats)) // Bind named method generic.
                    | item ->
                        vprintln 0 (sprintf "+++ yikes - should not need to bind positional mgr? %A" item)
                        mm
                List.fold mgr_bind Map.empty mgr

            let (binds, remote_marked) = 
                match called_class with
                    | ct ->
                        match f1o3(ty_mark_strip (0, false) ct) with
                            //| CTL_object -> []
                            //| Cil_cr_idl _ -> [] // This temp came from delegates and the delegate code does not add generics currently.
                            | CTL_record(_, cst, _, _, ats, binds, _) ->
                                let binds = map (fun (f,a)->(f, RN_monoty(a, ats))) binds
                                (binds, cst.remote_marked)
                            | CT_arr(ct, _)  -> ([("bananas-content-type-fid-delete-me-pls", RN_monoty(ct, []))], None) // TODO.
                            | CT_cr cst ->
                                let pairs = function
                                    | CT_class crt ->
                                        let binds = 
                                            if length crt.newformals = length cst.cl_actuals then List.zip crt.newformals cst.cl_actuals
                                            else sf (msg + sprintf ": Cannot zip uneven class generics  nf=%i na=%i" (length crt.newformals) (length cst.cl_actuals))
                                        (binds, crt.remote_marked)
                                    | other -> sf (msg + sprintf ": no class generic formals in a %s" (typeinfoToStr other))

                                match cst.crtno with
                                    | CT_knot(idl, whom, v) when not(nonep !v) -> pairs(valOf !v)
                                    | CT_class crt -> pairs(CT_class crt)
                                    | CTL_record(sqidl, cst, lst, len, _, binds, _) -> ([], cst.remote_marked) // could return binds if regress to dropping_t form but should no longer need to be accessed by formal name.
                                    | other -> sf (sprintf "need to unpack other form in lookup idl=%s: other=%A" ids other)
                            | other -> sf (sprintf "intcil: mine cgrm other form %A" other)
            let _ = //if nonep called_class then vprintln 3 ("no dest_ty") else
                    vprintln 3 (msg + sprintf ": inserting %i typeformals for surrounding class. %A" (length binds) (cil_typeToStr called_class))
            // We could better save this closure for all methods of this instance.
            let gmap (m:annotation_map_t) (x, y) =
                vprintln 3 (msg + sprintf ": method call: method instance template bind fid=%s with %s" x (drToStr y))
                m.Add([x], y)
            (List.fold gmap mgr_text binds, remote_marked) // rehyd for method in gtrace but class standings ?


        let squirrel = 
            let ctor_squirrel x = dev_println (sprintf "Squirrel spog %A" x)
            let ctor_pred usersub =
                match usersub with
                | No_method(srcfile, flags, ck, (gid, uid, _), mdt, flags1, instructions, atts) -> mdt.is_ctor
                | _ -> 0
            if not_nonep remote_marked && ctor_pred usersub > 0 then map ctor_squirrel actuals else []

        let textcl =
            {
                cm_generics= cm_generics       // Class and method generics on a callstring basis.
                cl_prefix=   prefix
//              cl_codefix=  uid :: tl idl     // Full method name is its static/textual name, post overload disambiguation, for droppings and local var naming.
                cl_codefix=  idl               // Full method name is its static/textual name for droppings.
                this_dt=     Some called_class      // If dest_obj=None then None else (hd signat_types); // get_ce_content_type (valOf dest_obj));
            }

        let cCL' = {
                     text=            textcl
                     cl_localfix=     localfix
                     this=            dest_obj
                   }

        let flogs = ats0 (* FOR NOW: ats ? *)
        let stack = ref []
        
        let fakecall() = ()

        let cf = g_null_callers_flags

        let rec native_flag_scan = function
            | [] ->false
            | Cilflag_hprls::tt -> true
            | Cilflag_hpr(s)::tt -> true
            | Cilflag_runtime::tt -> true            
            | _::tt -> native_flag_scan tt
        let env' = 
            if dest_obj=Some g_ce_zero // temp hack for when reading object from uninit array loc: arity one int32 match.
                then (stack := [g_ce_zero]; (env))
            elif tychk_pass_pred realdo then (fakecall(); (env))
            else
                let bif_flag = native_flag_scan (cilmethod_flags usersub)
                let bif_ans =
                    if bif_flag && memberp (hd idl) g_hybrid_bif_method_names then
                        let sitemarker = hptos callstring
                        let args:cil_eval_t list = map f3o3 actuals
                        let (cg_args, mg_args) = ([], []) // TODO - although the delegate may have type variables we dont yet support the delegated method having generics - please finish cleanly.
                        make_hybrid_bifcall cCL' cCB' idl ncpo usersub stack signat' args handler_ctx sitemarker virtualf (cg_args, mg_args) env // This serves for invoke
                    elif bif_flag then
                        make_bifcall cCL' cCB' cf idl (usersub, stack, actuals)
                    else None
                match bif_ans with
                    | Some ans -> ans
                    | None ->
                        let usersub =
                            if bif_flag then
                                let class_parent_list = get_class_parent_list msg thato_ty_called_class 
                                let special_cases = list_intersection([[ "MulticastDelegate"; "System" ]], class_parent_list)
                                let special_casef = not_nullp special_cases
                                vprintln 0 (sprintf "+++ supposedly built-in function uname=%s instance=%A not in bif table parents=%s special_casef=%A cpred=%A (idl=%A)" ids instancef  (sfold hptos class_parent_list) special_casef mdt.is_ctor idl)
                                match special_cases with // Divert to alternative implementation of native method (e.g. for MulticastDelegate .ctor)
                                        | [ item ] ->
                                            let new_idl = hd idl :: item
                                            if vd>=4 then vprintln 0 (sprintf "second chance idl=%s" (hptos new_idl))
                                            let virtualf = true
                                            let m_notice = ref None                                            
                                            match id_lookup_fn ww cCP cCB (fun(nd:int)->(dis + sprintf ":spcial_case method not (uniquely) defined.   Number of defs=%i.\n" nd, 1)) m_notice virtualf callsite new_idl signat' with
                                            //match g_kiwi_canned_bif.TryFind (new_idl, instancef, lookup_arity) with
                                                | [item] ->
                                                    if vd>=4 then vprintln 4 (sprintf "special_case divert method found: instancef=%A divert to new_idl=%s" instancef (hptos new_idl))
                                                    (snd item)
                                                | multiple ->
                                                    sf (sprintf "special_case divert method not found: instancef=%A arity=%i new_idl=%s notice=%s no_found=%i" instancef (length signat') (hptos new_idl) (valOf_or !m_notice "blank" ) (length multiple))
                                        | _ ->
                                            usersub

                            else usersub
                        //vprintln 3 (sprintf "ghent debug actuals = %s" (sfold (f3o3>>ceToStr) actuals))
                        cil_walk_method kxr mgr None realdo ww linepoint_stack (cCP, cCL', cCB', stack, env) usersub actuals flogs 

        if vd>=4 then
            //yout(fvd, "Returning from " + (if tychk_pass_pred realdo then "firstpass faked " else "") + sprintf "usercall method %s with %i child and %i parent stack items. voidf=%A\n" ids (length !stack) (length !spr) voidf)
            yout fvd ("Returning from " + (if tychk_pass_pred realdo then "firstpass faked " else "") + sprintf "usercall method %s with %i child and %i parent stack items. voidf=%A\n" ids (length !stack) (length !m_parental_spr) voidf)
            stackdump fvd (!stack)
        let _ = if not_nonep thato && newobjf then mutadd m_parental_spr (valOf thato)
                elif voidf then app (tidy_disposal kxr) (!stack)
                elif tychk_pass_pred realdo then mutadd m_parental_spr (CE_typeonly(return_ct))
                elif !stack=[] (*&& stl > 0 *)then sf (sprintf "No return value from non-void function '%s' " ids + (if false (*count' = C_hangs*)then ": Function did not exit" else ": Function returned void")) 
                //else mutadd m_parental_spr (if stl > 0 then hd(!stack) else CE_null)
                else mutadd m_parental_spr (hd !stack)   (* Pop result to parent stack *)
                //lprint 300 (fun() -> " return env=" + envlist(env') + " env on return from usercall\n\n")
        (!m_parental_spr, env') // end of mk_usercall
                        
    
    let newobj_ctorcall classgens_ realdo ats it sp idl (signat_types) usersub =
        match usersub with
            | No_method(srcfile, flags, ck, (gid, uid, _), mdt, flags1, instructions, atts) ->
                let voidf, virtualf = false, false
                let mgr = []
                let mdt' = { mdt with rtype=CTL_void; }
                mk_usercall ww signat_types None realdo linepoint_stack (sp, (true, fst it, snd it), voidf, virtualf, ck, mdt.rtype, (idl, usersub), mgr, ats) 
       
            | other -> sf ("newobj_ctorcall other: " + noitemToStr false other)

    let obj_alloc_length msg aa = ct_bytes1 ww msg (get_ce_type msg aa)

    // Locate a singleton cez addressed by an idl or generate a dummy (eg when referencing a static I/O that is not marked as a compilation target).
    // This is invoked from ldsfld, ldsflda and stsfld.  But for static structs we can see
    // ldsflda/ldfld code being generated by csc, which has the semantics of ldsfld and so this code needs to be applied from there too.   Ditto stores.
    let idl2cez_singleton allow_indexable_literalf idl = 
        if hd(rev idl) = g_dummy_objectholder_
        then muddy " valOf_or_fail \"luf not defined\" (fst(lookup_uninstantiated_field ww (CC, CCL) ([hd idl], lookup_uninstantiated_object (CC, CCL) structptr)))"
        else
            let envo = None
            let jj = singleton_locate ww cCB allow_indexable_literalf realdo m0 textcl.cm_generics envo idl // TODO: cgr is in CCL quite often!
            let aa = if nonep jj then heapf_printout(cCC.heap, dis + "\nheap item id='" + hptos idl + "' is not defined ")
                     else valOf jj
            aa

    let tycheck_fcall ww msg fr bifo rt ohead signat thato = 
        let (cr2, cg_args, mg_args) =
            match fr with // replicated from fr33 
                | (Cil_cr_templater(cr, cg_args), _, mg_args) ->
                    //It is still used for array content type.
                    //let _ = dev_println ("The cr_templater approach for method generics was believed to be unused :" + ids)
                    (cr, map dex_fap cg_args, mg_args) // Two forms from parse tree - perhaps only one used now.
                | (cr, _, mg_args) -> (cr, [], mg_args)

        let (cg_args, mg_args) =
            let mpe ty = RN_monoty(fst(cil_type_n_gb ww realdo gb0 ty), [])
            (map mpe cg_args, map mpe mg_args)

        // Augment bindings temporarily with method generic bindings - copy 1/2 (typecheck and dropping make).
        // These may be class or method generics for the called method, but they are not our own class generics, they are just bindings for this one method call.
        let gb0_augmented = 
            let mg_augment_normenv methodf (cc:annotation_map_t) = function
                | (RN_monoty(ty, ats), idx) ->
                    let fid = sprintf (if methodf then "!!%i" else "!%i") idx
                    if (*a1*)vd>=4 then vprintln 4 (sprintf "L3508 method generic bind:  methodf=%A idx=%i  %s  with %s " methodf idx fid (cil_typeToStr ty))
                    cc.Add([fid],  RN_monoty(ty, ats))
            let ee = List.fold (mg_augment_normenv false) (snd gb0) (zipWithIndex cg_args)
            let ee = List.fold (mg_augment_normenv true) ee (zipWithIndex mg_args)                            
            (fst gb0, ee)

        let rec closed_tyeval = function
            | ty -> fst(cil_type_n_gb ww realdo gb0_augmented ty)

        let tycomp (so, ctype) = (so, closed_tyeval ctype)
        let signat' = map tycomp signat // NB: will later  use type values from stack instead. eg. args which we have now peeked so could use?
        // rt is eval'd in augmented gb because of the following example: T0 is a local to
        // the called object and may not be bound in the caller's generics.
        // sc=fake:1.3::: callvirtual instance !T0 [Kiwi]Channel2`1<[System]Int32>::Read()

        let rt =
            match bifo with
                | Some bif ->
                    if bif.identity_types then (if nullp signat' then sf "L3223bif" else snd(hd signat'))
                    else valOf_or bif.rt CTL_void
                | None -> rt
        let (rt, tyvars) = cil_type_n_gb ww realdo gb0_augmented rt
        // The class_dt for dynamic poly dispatch (C++ virtual calls) in typechecking will be the superclass.
        // If overrides exist, the appropriate one must be invoked by a gtrace conditional branch.
        let class_dt =
            if ohead=0 then
                let idl = cil_classrefToIdl ww (snd gb0_augmented) cr2 //
                let ids = hptos idl
                let ty_for_now = Cil_cr ("TODO-please lookup - should be a CT_class for " + ids)
                let dt_here = CT_cr { g_blank_crefs with name=idl; cl_actuals=cg_args; meth_actuals=mg_args; crtno=ty_for_now } // static methods can still have type generics, so need a CT_cr wrapper around nothing.
                //vprintln 3 (sprintf "Static class with generics: silly_dx mis save thing at %s  using %s "  ids (cil_typeToStr dt_here))
                RN_monoty(dt_here, [])
            else RN_monoty(get_ce_type "L2877" (valOf_or_fail ("L2734 thato") thato), [])

        //let _ = vprintln 0 (sprintf "class_dt now %A" class_dt)
        (class_dt, rt, tyvars, map snd signat', cg_args, mg_args)


    let ldsfld_readldo_intcil_core msg sitemarker field_dt idl =
        let singleton_she = idl2cez_singleton false idl // ty_rec and idl already combined in field_dt we trust ?  - Only a field is loaded (or multiple if that field is a struct).        let ce =
        let debug_ce = singleton_she.cez // Not used
        let ce =
            match get_struct_tag_listf msg debug_ce field_dt [] with  // ldfld and lfsfld expand structs to CE_struct. ldloc relies on its caller to have done this. ldloc had the expansion on local creation.
                | Some fields__ ->
                    //let _ = dev_println (msg + sprintf ": get_struct_tag_list fields are %A" (sfold (fun (tagid, dt) -> tagid + ":" + cil_typeToStr dt) fields))
                    let fields = struct_get_raw_items msg field_dt                                   
                    let idtoken = funique "LDSFLD_STRUCT"
                    CE_struct(idtoken, field_dt, idtoken, map (fun (ptag, dt, _) -> (ptag, dt, Some(gec_field_snd_pass ww sitemarker ("L2724" + msg) field_dt singleton_she.cez ptag))) fields)

                | None ->
                    // (msg + " was not a structure load")
                    singleton_she.cez
        //dev_println (msg + sprintf ":  loaded ce=%s ty=%s " (ceToStr ce) (cil_typeToStr field_dt))
        ce


    let stsfld_readldo_intcil_core msg sitemarker field_dt rty lhs_ats rhs idl =
        let abuse = spot_type_abuse msg false field_dt rty
        let atakenf = None
        let lhs =
            let (blobf, cez, vrn_) = gec_singleton ww (*a1*)vd cCBo msg "lhs" lhs_ats realdo m0 "" idl field_dt None None None
            cez
        let as_gen (l, r) =
            if is_rtl_pram_or_input l then
                vprintln 3 (sprintf "supressed assigment to static scalar %s that is an input or RTL parameter." (hptos idl))
                ()
            else cg msg (K_as_sassign(kxr, l, r, abuse))
        let full_structure_assign = full_struct_assign_pred msg lhs // could do once during typecheck
        //dev_println(sprintf "%s: lhs=%s\n    rhs=%s\n     field_dt=%s" msg (ceToStr lhs) (ceToStr rhs) (cil_typeToStr field_dt))
        if full_structure_assign then
            let msg = "Structure assign to static field " // TODO unify with code for dynamic field
            let llst = gec_field_snd0 ww sitemarker msg field_dt None lhs
            let rlst =
                match rhs with
                | CE_struct(idtoken, dt_, aid_, lst) ->
                    map (fun (tag, dt, vo) -> valOf_or_fail "L3589" vo) lst

                | other -> 
                    let _ = sf (sprintf "We expect the following work now to have been done for us during rhs load. Non_struct_ce=%s" (ceToStr other))
                    //let fields = get_struct_tag_listf msg rhs field_dt [] 
                    let fields = struct_get_raw_items msg field_dt
                    //dev_println (msg + sprintf ": get_struct_tag_list fields are %A" (sfold (fun (ptag, _, _) -> ": ptag=" + ptag) fields))
                    map (fun (ptag, dt_, _) -> gec_field_snd_pass ww sitemarker ("L2605" + msg) field_dt rhs ptag) fields
            if vd>=4 then vprintln 4 (msg + ceToStr lhs + "   " + i2s(length llst) + " " + i2s(length rlst))
            app as_gen (List.zip llst rlst)
        else as_gen(lhs, rhs)

    let rec cil_eint kxr handler_ctx eE =
        youtln fvd ("intcil: " + ciliToStr eE) 
        let sitemarker = hptos callstring
        match eE with
        | Cili_ret ->
                let msg = "Cili_ret"

            (* Owing to the way the code is inserted in the ia (instruction array), the only
               return should be at thread exit, and thats if there's no tailhang.  In-lined methods just fall off the end with any returned value copied to the parent's stack.
               Constructor calls from other root items are expected to be totally non-blocking, in which case they should form combinational style hardware. But if they do block, they will force system exit, so perhaps we need to apply tailhang to non lastone items, but this would make them a bit blocking and lead to non-combinational code...  perhaps suppress finish calls on bigbang sensitivities.
            *)
                let (keep_return, rc) = cCB.retlab
                let tailhang = cCB.tailhangf
                let _ = (if tychk_pass_pred realdo then vprintln 3 (sprintf "Return instruction: retlab=%A tailhang=%A" keep_return tailhang))
                let (ce, sp') = if nullp sp then (None, sp) else (Some(hd sp), tl sp) 
                let _ =
                    if keep_return && not tailhang
                    then cg msg (K_topret(kxr, ce))
                    else sf ("Subroutine returned when it should have been inlined in a parent")
                (sp', env)

        | Cili_throw -> 
            let msg = "throw (.NET throw exception instruction)"
            let (thrown_item, sp) = g1()
            let magic_etag = "code"
            let rec scrap_around_for_error_code_field msg = function
                | CT_star(_, ct) ->  scrap_around_for_error_code_field msg ct
                | CT_cr crst ->  scrap_around_for_error_code_field msg crst.crtno
                | CTL_record(sqidl, cst, lst, len, _, binds, _) ->
                    //let _ = dev_println (sprintf "scrapping in field list %A" lst)
                    let rec get_code_field = function
                        | concfield::tt when concfield.ptag.Contains magic_etag -> Some(sqidl, concfield.ptag)
                        | _::tt                                                 -> get_code_field tt
                        | [] -> None
                    get_code_field lst
                | other -> 
                    //muddy (sprintf "Perhaps need to find a suitable error code in here %A" (cil_typeToStr other))
                    None


            let ptag_o = scrap_around_for_error_code_field msg (get_ce_type msg thrown_item)
            //let _ = dev_println (sprintf "scrapped for %A" ptag_o)
            let _ =
                match realdo with
                    | Rdo_tychk dlist ->
                        match ptag_o with
                            | None -> () 
                            | Some(sqidl, ptag) ->
                                let dct = field_type_check_and_drop ww msg (Some thrown_item) dlist callstring gb0 CTL_void [ptag]
                                ()

                    | Rdo_gtrace _ ->
                        // Since we have elaborated all of the parent's stack frames, implementing a catch handler is not really that tricky, but for now it is missing.
                        let _ = dev_println (sprintf "Throw %A with handlers surrounding being %A"  (ceToStr thrown_item) handler_ctx)

                        let check_handler htoken =
                            match op_assoc htoken fstpass.exn_handler_directory with
                                | None -> sf ("missing handler " + htoken)
                                | Some(token, catchers, _) ->
                                    let check_catch (hty, pno, body) =
                                        let null_normenv = ({grounded=false}, Map.empty)
                                        let check_code = isinst_macro_render ww null_normenv handler_ctx (Cili_isinst(true(*for now, false really when TODO is done*), hty))
                                        muddy (sprintf "%s check code %A" htoken check_code) // TODO

                                    app check_catch catchers
                                    
                        let _ = app check_handler handler_ctx
                        let (ty_rec, field_dt, ptag_) = field_type_retrieve ww msg gb0 callstring


                        if nonep ptag_o then
                            cleanexit(sprintf "KiwiC requires all (uncaught) raised exceptions to contain an integer field where the field name contains the string '%s'.  Could not find one in %s" magic_etag (cil_typeToStr ty_rec))
                            
                        else    
                            let errorcode = gec_field_snd_pass ww sitemarker msg ty_rec thrown_item (snd (valOf ptag_o)) // Normal handle

                            let _ = vprintln 3 (sprintf "Thrown item currently ignored - but need to generate an appropriate call to hpr_sysexit")
                            let idl = [ "simplethrow" ]
                            let cf = g_null_callers_flags
                            cg msg (K_easc(KXR_dot(idl, kxr), CE_apply(g_bif_abend_or_sysexit, cf, [errorcode]), true))            
            (sp, env)

        | Cili_ldnull -> (g_ce_null::sp, env)

        | Cili_ldarga s ->  // Load address of formal parameter.
            let n1 = ciltoarg_symb bindings s
            let ans = 
                match realdo with
                    | Rdo_tychk _ ->
                        match  fstpass.argtypes.[n1] with
                            | None ->  sf (sprintf "ldarga: no argtype %i" n1)
                            | Some x -> CE_typeonly(gec_CT_star 1 x)
                    | Rdo_gtrace(revlst, thunk) ->
                        let really(space:space_t) = 
                            match space.[n1] with
                                | None -> sf("spaceread: ldarg: arg was not initialised: " + i2s n1)
                                | Some (_, ctype, vale) -> gec_CE_star sitemarker 1 vale
                        really (valOf_or_fail "ldarga: no argspace" wmd).argspace
            (ans::sp, env)

        | Cili_ldarg s ->  // Load formal parameter's actual value
            let n1 = ciltoarg_symb bindings s
            let msg = "ldarg"
            let ans = 
                match realdo with
                    | Rdo_tychk _ ->
                        match  fstpass.argtypes.[n1] with
                            | None ->  sf (sprintf "ldarg: no argtype %i" n1)
                            | Some x -> CE_typeonly x
                    | Rdo_gtrace(revlst, thunk) ->
                        let really(space:space_t) = 
                            match space.[n1] with
                                | None -> sf("spaceread: ldarg: arg was not initialised: " + i2s n1)
                                | Some (_, ctype, vale) -> vale
                        really (valOf_or_fail "ldarg: no argspace" wmd).argspace
            //let _ = vprintln 0 (msg + sprintf ": ans=%s ans_dt=%s" (ceToStr ans) (cil_typeToStr(get_ce_type msg ans)))
            (ans::sp, env)

        | Cili_starg formal_idx -> 
            let (rhs, c') = g1()
            //if not colouringf then cleanexit(m0() + ": starg KiwiC TODO do not update your formal parameters for now -------- TODO - coming shortly")
            let n1 = ciltoarg_symb bindings formal_idx
            let callstring_r = "starg-rhs-type" :: callstring
            let _ = 
                match realdo with
                | Rdo_tychk dlist -> 
                    let lty = match fstpass.argtypes.[n1] with
                                | Some ct -> ct 
                                | None    -> sf (sprintf "starg: missing first pass type no=%i" n1)
                    log_tval ww msg dlist callstring (gec_RN_monoty ww lty)
                    let rty = get_ce_type msg rhs
                    log_tval ww msg dlist callstring_r (gec_RN_monoty ww rty)
                    ()
                        
                | Rdo_gtrace _ ->
                    let ldt = mono_type_retrieve ww msg gb0 callstring 
                    let rty = mono_type_retrieve ww msg gb0 callstring_r
                    let abuse = spot_type_abuse msg false ldt rty
                    let as_gen (l, r) = cg msg (K_as_sassign(kxr, l, r, abuse))
                    let argspace = (valOf_or_fail "starg: no argspace" wmd).argspace
                    let (idl_, dt_, cez) = valOf_or_fail "starg: arg out of range" argspace.[n1]
                    let _ = vprintln 3 (sprintf "intcil: starg %i  lhs=%s rhs=%s" n1 (ceToStr cez) (ceToStr rhs))
                    let sap = full_struct_assign_pred msg cez
                    if sap then 
                        let msg = "Structure assign to formal parameter "
                        let rlst = gec_field_snd0 ww (sitemarker + "R") msg ldt None rhs 
                        let llst = gec_field_snd0 ww (sitemarker + "L") msg ldt None cez 
                        let q = List.zip llst rlst
                        let _ = vprintln 3 (msg + ceToStr cez + "  lengths= " + i2s(length llst) + ", " + i2s(length rlst))
                        app as_gen q
                    else as_gen(cez, rhs)


            (c', env)
               

        | Cili_mul s  -> (e2arith eE (V_times), env)
        | Cili_add s  -> (e2arith eE V_plus, env)
        | Cili_div s  -> (e2arith eE (V_divide), env)
        | Cili_rem s  -> (e2arith eE V_mod, env)
        | Cili_and    -> (e2arith eE V_bitand, env)
        | Cili_or     -> (e2arith eE V_bitor, env)
        | Cili_xor    -> (e2arith eE V_xor, env)
        | Cili_sub s  -> (e2arith eE V_minus, env)
        | Cili_shl s  -> (e2arith eE V_lshift(* Is s ever meaningful for left shift? here we ignore it. *), env)
        | Cili_shr s  -> (e2arith eE (V_rshift (if s=Cil_unsigned then Unsigned else Signed)), env)

        | Cili_switch lst       -> c_switch kxr lst
        | Cili_brtrue s         -> c1branch kxr []  false s
        | Cili_brfalse(sco, s)  -> c1branch kxr sco  true s

        | (Cili_bge s) -> cbranch kxr true (V_dled Signed) s
        | (Cili_bgt s) -> cbranch kxr true (V_dltd Signed) s
        | (Cili_ble s) -> cbranch kxr false (V_dled Signed) s
        | (Cili_blt s) -> cbranch kxr false (V_dltd Signed) s

        | (Cili_bge_un s) -> cbranch kxr true (V_dled Unsigned) s
        | (Cili_bgt_un s) -> cbranch kxr true (V_dltd Unsigned) s 
        | (Cili_ble_un s) -> cbranch kxr false (V_dled Unsigned) s
        | (Cili_blt_un s) -> cbranch kxr false (V_dltd Unsigned) s

        | (Cili_beq s) -> cbranch kxr false V_deqd s
        | (Cili_bne s) -> cbranch kxr false V_dned s

        | (Cili_cge s) -> scond kxr true (V_dled Signed)
        | (Cili_cgt s) -> scond kxr true (V_dltd Signed)
        | (Cili_cle s) -> scond kxr false (V_dled Signed)
        | (Cili_clt s) -> scond kxr false (V_dltd Signed)
        | (Cili_ceq) -> scond kxr false V_deqd
        | (Cili_cne) -> scond kxr false V_dned

        | Cili_ldftn(virtualf, ck, rt, fr, signat, site_) ->
            //According to docs, this just pushes an opaque integer function pointer (g_IntPtr) on the stack.
            //But our implementation pops one (typically the closure), and then pushes two values on stack: method and object.
            //If a static method mcs will have pushed null in advance. We modify that with dynclo because ...
            //IL_0038:  ldnull 
            //IL_0039:  ldftn int32 class Fpointers::pred(int32)
            //IL_003f:  newobj instance void class Fpointers/PointerToFunction::'.ctor'(object, native int)

            let (obj, sp') = g1() // This pop lies outside the CIL spec
            let msg = "ldftn"
            let obj' = if obj<>g_ce_null then obj // aka thato
                       elif nonep dynclo then g_ce_null // dynclo is passed in at the top specially for this instruction's use.
                       else valOf_or ((valOf dynclo).this) g_ce_null
            let (cr_, idl) = fr3_fullpathname ww textcl.cm_generics fr
            let arity = length signat
            // let _ = vprintln 3 (i2s arity + " arity")
            let (ohead, thato) = (0, None) // for now
            let bifo = None // TODO ltoken can be applied to built-ins
            let handle_dt = CTL_reflection_handle["method"]
            let ans =
                match realdo with
                | Rdo_tychk dlist ->
                    let (class_dt, rt, tyvars, signat', cg_args, mg_args) = tycheck_fcall ww msg fr bifo rt ohead signat thato
//                    let (rt, tyvars) = cil_type_n_gb ww realdo gb0 rt
//                    let tycomp (so, ctype) = (so, fst(cil_type_n_gb ww realdo gb0 ctype)) //TODO do we not want _gb here?
//                    let (cgr, mgr) = ([], []) // TODO get from _gb following the pattern elsewhere
//                    let class_dt = muddy "class_dt L2838"
//                    let signat' = map tycomp signat
                    log_tval ww msg dlist callstring (RN_call(class_dt, qqx_a ww msg tyvars rt, [], map (fun(ty) ->qqx_a msg ww tyvars ty) signat', cg_args, mg_args))
                    //Why is this slave routine not being used? it just runs qqx_a.
                    //let _ = log_call_types ww msg dlist callstring thiso rt signat'  
                    CE_typeonly(handle_dt) // g_IntPtr at runtime 

                | Rdo_gtrace _ ->
                    let (class_dto_, return_dt, _, signat', cgr1_, mgr1_) = call_type_retrieve ww msg gb0 callstring
                    let ((return_dt, _), signat') = (concrate ww msg return_dt, map (concrate ww msg >> fst) signat')
                    let m_notice = ref None
                    // This old uid will not distinguish between overloads.
                    let uid__ = { f_name=idl; f_dtidl=idl; baser= -3303L;  (* Arb hidx for a static function pointer. May need an object pointer too, as in the two args to a ThreadStart.  (TODO need also ldvirtftn code).  *)  length=None; }

                    match id_lookup_fn ww cCP cCB (fun(nd:int)->(dis + sprintf ":ldftn : item not (uniquely) defined.   Number of defs=%i.\n" nd, 1)) m_notice virtualf callsite idl signat' with

                        | [item_] ->
                            gec_CE_reflection cCC msg (handle_dt, None, Some idl, [])  // ldftn - we dont use the valo field for ldftn
                            // TODO use a CE_reflection(handle_dt, ... )
                            // All of our methods have a disam name - that's what we should return here as a lifted from of opaque integer.
                            // This has identified which overload to use, but overrides need work?
                            //gec_CE_Region ww true (Some(uid_nemtokf uid__), g_ctl_object_ref, uid__, [], None)
                        | _ ->
                            sf(msg + sprintf "function not found " + dis)
            (ans:: obj':: sp', env)

        | Cili_newobj(flags, ck, tt, (cr, id, gargs), signat, site_) ->
            (* tt is always 'void' and stands for a specialtype ? *)
            (* The id is the constructor name and is always '.ctor' or a disam version *)
            let arity = length signat
            let vd = 3
            let rec get_dtidl_and_sqidl = function
                | CT_star(_, a) -> get_dtidl_and_sqidl a
                | CT_cr cr -> (cr.name, cr.name)
                | CTL_record(sqidl, cst, lst, len, _, binds, _) -> (cst.name, sqidl)
                //| CTL_object -> (g_core_object_path, g_core_object_path)
                | other -> sf (sprintf "cili_newobj: other form dtidl: %s" (cil_typeToStr other))
            let ohead = 0 // No 'this' pointer yet!
            let bifo = None
            //let _ = vprintln vd (i2s arity + " is newobj arity")
            match realdo with
                | Rdo_tychk dlist ->
                    let (sp', args) = gn_static arity sp
                    let (class_dt, void_dt_, tyvars, signat', cg_args, mg_args) = tycheck_fcall ww msg (cr, id, gargs) bifo tt ohead signat None
//                    let signat' =
//                        let tycomp  (so, ctype) = (so, cil_type_n_gb ww realdo gb0 ctype)
//                        map tycomp signat
//                    let (dt, tyvars) = cil_type_n_gb ww realdo gb0 cr
//                    let (cgr, mgr) = ([], []) // TODO get from _gb following the pattern elsewhere

                    let dt = fatal_dx msg "L2976" class_dt
                    vprintln 3 (msg + sprintf ": 1st pass newobj a dt=%s" (cil_typeToStr dt))
                    let dt1_ = log_call_types ww "newobj" dlist callstring (dt) tyvars cg_args mg_args dt args
                    vprintln 3 (msg + sprintf ": 1st pass newobj b dt=%s" (drToStr dt1_))
                    //log_tval ww msg dlist callstring (RN_call(qqx_a ww msg actual_drops dt, actual_drops, map (fun (so, ty)->(so, qqx ww msg ty)) signat'))
                    ((CE_typeonly dt)::sp', env)

                | Rdo_gtrace _ ->
                    let (class_dto, dt, gactuals', signat', cgr1_, mgr1_) = call_type_retrieve ww "newobj" gb0 callstring // retrieve dropping.
                    let ((dt, _), signat') = (concrate ww msg dt, map (concrate ww msg >> fst) signat')
                    // Search for a hintname to make the output more human readable.
                    let (hintname, ats) = get_hintname ("NEWOBJ", []) (ins, hints)  (* This is not working if you see NEWOBJ in the o/p!*) 
                    let hintname_o = if hintname = "NEWOBK" then None else Some hintname
                    let ww = WF vd "newobj" ww (sprintf "hintname done: hintname=%A : %s" hintname_o (cez_ToStr_atts ats))
                    vprintln 3 (msg + sprintf ": 2nd pass newobj dt=%s" (cil_typeToStr dt))


                    // When we apply newobj to a valuetype, we will not allocate any heap space.  Instead, it is more like a 'positional new' in C++ and the constructor shall return a CE_struct immediate form.
                    let valuetype = ct_is_valuetype dt 
                    let ptr_dt = dt
                    let deref = // TODO abstract this replicated code but check on star modifications: it is nearly abstracted in block_reftran_fastspill.
                        let ident = funique "block_refxx_newobj"
                        let ptr_aid = ident :: callstring // Need a different aid since the types are different by one star
                        let (blobf_, cez, vrnl_) = gec_singleton ww (*a1*)vd cCBo msg "block_refxx_newobj" ats realdo m0 "" ptr_aid ptr_dt None (Some false) None
                        cez // newobj deref nemtok create

                    let item_aid =  htos(funique ("item") :: callstring)
                    let (root_dtidl, dtidl) = get_dtidl_and_sqidl dt
                    let stru =
                        match dt with
                            | dt when dt = g_ctl_object_ref || dtidl=g_core_object_path -> g_canned_object
                            //| CTL_object -> g_canned_object
                            | CT_star(_, dt) when dt= g_ctl_object_ref -> g_canned_object // TODO temporary miscoding re stars on CTL_object - please fix.
                            | dt -> 
                                //vprintln 3 (sprintf "newobj type names: dtidl=%A root_dtidl=%A" dtidl root_dtidl)
                                let stru =
                                    let (found, ov) = g_record_table.TryGetValue (hptos dtidl)
                                    if found then f1o4 ov
                                    else sf (dis + "\n" + hptos dtidl + " record should be defined in g_record_table [newobj]\n" + sprintf "%A" dt)
                                stru
                    let idtoken = funique "NEWSTRUCT"
                    let deref_obj = deref
                    if valuetype then
                        let av = CE_struct(idtoken, dt, idtoken, struct_get_raw_items "NEWSTRUCT" dt)
                        cg msg (K_as_sassign(kxr, deref_obj, av, None))
                        else
                            cg msg (K_new(kxr, deref_obj, item_aid, KN_newobj(dtidl, hintname_o, stru, dt, ats)))
                                    
                    let ctor_idl = id :: root_dtidl
                    let ctor_ans =                     // Copy 1/2 of constructor lookup code
                        let virtualf = false
                        let m_notice = ref None

                        match id_lookup_fn ww cCP cCB (fun(nd:int)->(dis + sprintf ": newobj: constructor not (uniquely) defined.  Number of defs=%i.\n" nd, 1)) m_notice virtualf callsite ctor_idl signat' with
                            | [] -> sf (sprintf "Missing constuctor for (no suitable overload?)  " + hptos ctor_idl)
                            | [ (idl_, ctor) ] ->
                                let bytes = obj_alloc_length "newobj get size" (deref_obj)
                                let msg = if valuetype then (sprintf "newobj of valuetype will return a CE_struct and not allocate heap space.") else (sprintf "CE_newobj rezzed for %i bytes" bytes)
                                let ww = WF vd "newobj" ww (sprintf "%s. Now calling the constructor." msg)
                                let ctor_ans = newobj_ctorcall gactuals' realdo ats (dt, Some deref_obj) sp ctor_idl (signat') ctor // this will call the inner ctors? C# compiler has inserted those calls in this. ... but we need to allocate the valuetypes and object handles in the current object.

                                //vprintln 0 (sprintf "remote_marked squirrel site L3717" )
                                //remark kxr ("post constructor call - squirrel ckpt " + hptos ctor_idl)
                                ctor_ans
                    ctor_ans

        | Cili_ldtoken(ct, fro) ->  // Used for array initialisation blobs and some higher-order programming.
            let msg = "ldtoken"
            // The ldtoken instruction pushes a RuntimeHandle for the specified metadata token. A RuntimeHandle can be a fieldref/fielddef, a methodref/methoddef, or a typeref/typedef.
            let handle_dt =
                match fro with
                    | None ->  CTL_reflection_handle ["type"] // Or g_IntPtr 
                    | Some fr -> CTL_reflection_handle [ "field-or-method"] // Or g_IntPtr 

            let ans = 
                match realdo with
                | Rdo_tychk dlist ->
                    log_tval ww msg dlist callstring (gec_RN_monoty ww ct) // Save type of underlying object
                    let eager_check_ =
                        match fro with
                            | None -> ()
                            | Some fr ->
                                let (_, idl) = fr3_fullpathname ww textcl.cm_generics fr
                                let oc = id_lookupp ww idl Map.empty (fun()->(dis + " : typecheck pass: item not defined\n", 1))
                                ()
                    CE_typeonly(handle_dt) // Return type of the handle.
                     
                | Rdo_gtrace _ ->
                    let dt = mono_type_retrieve ww msg gb0 callstring
                    let ats = []
                    match fro with
                        | Some fr ->
                            let (_, idl) = fr3_fullpathname ww textcl.cm_generics fr
                            //let uid = { f_name=idl; f_dtidl=idl; baser=0L; length=None; } (* TODO *)  //This should be an unmanaged pointer - g_IntPtr
                            gec_CE_reflection cCC msg (handle_dt, None, Some idl, ats)  // ldtoken
                        | None ->
                            //let idl = type_idl msg dt
                            gec_CE_reflection cCC msg (handle_dt, Some dt, None, ats)  // ldtoken
            (ans::sp, env)

        | Cili_neg    // Two's complement.
        | Cili_not -> // Bitwise one's complement.  Can also serve to complement a bool.  There is no dotnet NOR instruction as per the ! operator in C/C++.
            let (arg, sp') = g1() 
            let msg = "neg/not monadic complements"
            let ans = 
                match realdo with
                    | Rdo_tychk dlist ->
                        let dt = get_ce_type msg arg
                        log_tval ww msg dlist callstring (gec_RN_monoty ww dt)
                        CE_typeonly(dt)
                    | Rdo_gtrace _ ->
                        let dt = mono_type_retrieve ww msg gb0 callstring
                        let base_bif = (if eE = Cili_not then g_bif_not else g_bif_neg)
                        let bif = { base_bif with rt = Some dt; }
                        CE_apply(bif, g_null_callers_flags, [arg])
            (ans::sp', env)

        | Cili_nop
        | Cili_comment _ -> (sp, env) 

        | Cili_lab lab  ->
            let m = (" src lab: " + hptos fstpass.idl +  " : " +  lab)
            //let s = remark m
            let _ = cg msg (K_lab(kxr, add_labcontext lab))
            (sp, env)

        | Cili_endfinally   
        | Cili_endfault   -> //    Does nothing here - but parent shall make the the xfer for us.
            let _ = vprintln 3 "Endfinally or endfault encountered."
            (sp, env)

        | Cili_conv(ttt, overflow_trapf)  ->  // TODO generate overvflow exception when requested.
            let msg = "Cili_conv"
            let (v, sp') = g1()
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let (dt, _) = cil_type_n_gb ww realdo gb0 ttt
                        let dt =
                            match (dt, v) with
                                // Although we may have a A64 machine model, input dotnet code will commonly have w=32.
                                                       // TODO unify this code with the other pointer arrithmetic spotters in gec_CE_alu and newlf_star
                                | (CTL_net(_, w, _, _), CE_typeonly(CT_star(ns, ott))) when(w = 32 || w = int32(g_pointer_size*8L)) && ns <> 0 ->
                                    let _ = vprintln 3 (msg + sprintf ": %s infer convert to IntPtr from %s %s" (hptos callstring) (cil_typeToStr dt) (ceToStr v))
                                    gec_CT_IntPtr(CT_star(ns, ott))

                                | (dt, _) ->
                                    //let _ = dev_println (msg + sprintf ": %s did not infer convert to IntPtr from %s %s" (hptos callstring) (cil_typeToStr dt) (ceToStr v))
                                    dt
                        log_tval ww msg dlist callstring (gec_RN_monoty ww dt)
                        CE_typeonly(dt)
                    | Rdo_gtrace _ ->
                        let dt = mono_type_retrieve ww msg gb0 callstring
                        //let _ = dev_println (msg + sprintf ": %s recall dt %s %s" (hptos callstring) (cil_typeToStr dt) (ceToStr v))
                        gec_CE_conv ww msg dt CS_preserve_represented_value v // This is the stongest sort of cast that invokes int to float and its cousins and inverses.
            (ans:: sp', env)
                   
        | Cili_br lab ->
            let _ = emit_k_goto kxr None lab
            (sp, env)


        // The Cili_leave is like an unconditional branch, but first executes the body of any enclosing finally blocks.  The leave instruction is a long jump that resets stack pointer.  We can treat mainly as a jump here since we detect and invoke associated finally blocks using a different mechanism.
        | Cili_leave lab ->
            let _ = emit_k_goto kxr None lab
            ([], env)  // We treat leave the same as a branch because we check for leaving all branches.  But note the stack is zeroed for Cili_leave.

        | Cili_castclass _ -> (sp, env) // nop for now - but would affect the dynamic dispatch TODO

        | Cili_ldc(ct, v) -> 
            let (t, _) = cil_type_n_gb ww realdo gb0 ct
            let v = cil_apply_sign_to_constant bindings t v
            (v::sp, env)

        | Cili_ldstr s -> ((gec_ce_string s) :: sp, env) 

        | Cili_ldlen -> 
          let (v, c') = g1() in
          if tychk_pass_pred realdo then (CE_typeonly(valOf g_bif_ldlen.rt)::c', env)
          else
              let ans = CE_apply(g_bif_ldlen, g_null_callers_flags, [v])
              (ans::c', env)

        | Cili_stloc local_idx ->
            let msg = "stloc"
            let (rhs, c') = g1()
            let w' = ciltoarg_symb bindings local_idx
            let spillf = (w' >= g_spillidx)
            let spillget n =
                match fstpass.spillage.[n-g_spillidx] with
                    | SP_none -> sf (i2s w' + ": stloc: spill var not defined") 
                    | SP_zygote idl ->  mk_spill_var2 ww fstpass w' idl rhs
                    | SP_full sv -> sv
                    
            let lhs_cez = if spillf then spillget w'
                          else f3o5(valOf_or_failf (fun () -> i2s w' +  ": stloc: localvar not defined") fstpass.localnames.[w'])
            let callstring_r = "stloc-rhs-type" :: callstring
            match realdo with
                | Rdo_tychk dlist -> 
                    let lty = get_ce_type msg lhs_cez
                    log_tval ww msg dlist callstring (gec_RN_monoty ww lty)
                    let rty = get_ce_type msg rhs
                    log_tval ww msg dlist callstring_r (gec_RN_monoty ww rty)
                    ()
                        
                | Rdo_gtrace _ ->
                    let rty = mono_type_retrieve ww msg gb0 callstring_r
                    let ldt = mono_type_retrieve ww msg gb0 callstring 
                    let abuse = spot_type_abuse msg false ldt rty
                    let as_gen (l, r) = cg msg (K_as_sassign(kxr, l, r, abuse))
                    let sap = full_struct_assign_pred msg lhs_cez
                    if sap then 
                        if ce_iszero lhs_cez then
                            // The pinned paradigm adds a pinned flag to the variable declaration and clears it to null with a stloc to free for garbage collection.
                            // We do not need to generate code when a struct handle is set to zero
                            vprintln 3 (msg + sprintf "Ignore struct assign with handle as zero - an unpinning")
                        else
                            let msg = "Structure assign to local var "
                            let rlst = gec_field_snd0 ww (sitemarker + "R") msg ldt None rhs 
                            let llst = gec_field_snd0 ww (sitemarker + "L") msg ldt None lhs_cez 
                            let q = List.zip llst rlst
                            let _ = vprintln 3 (msg + ceToStr lhs_cez + "  lengths= " + i2s(length llst) + ", " + i2s(length rlst))
                            app as_gen q
                    else as_gen(lhs_cez, rhs)
            (c', env)

        | Cili_ldloc w ->
            let msg =  "Cili_ldloc"
            let (w', _) = (ciltoarg_symb bindings w, msg)
            let spillget n = match fstpass.spillage.[n-g_spillidx] with
                                | SP_full sv -> sv
                                | _ -> sf "spill var not defined" 
            let cez = if w' >= g_spillidx then spillget w'
                        else f3o5(valOf_or_failf (fun ()-> sprintf "ldloc: localvar V_%i not defined" w') fstpass.localnames.[w'])
            let dt = get_ce_type msg cez
            let sap = full_struct_assign_pred msg cez
            //let _ = vprintln 3 (msg + sprintf " cez=%s sap=%A dt=%s" (ceToStr cez) sap (cil_typeToStr dt))
            let ans = 
               match realdo with
                   | Rdo_tychk dlist ->
                       cez
                       
                   | Rdo_gtrace _ ->
                       if sap then
                           let rlst = gec_field_snd0 ww sitemarker msg (dt) None cez // all fields if a struct
                           let items = struct_get_raw_items msg dt
                           //let _ = vprintln 0 (sprintf "  backtop sap  p1 %i   p2 %i" (length items) (length rlst))
                           let idtoken = funique "LDLOCSTR"
                           CE_struct(idtoken, dt, idtoken, map (fun ((tag, dt, _), ce) -> (tag, dt, Some ce)) (List.zip items rlst))
                        else cez
            (ans::sp, env) 

        | Cili_ldloca w ->  // Load the address of a local variable. (Generally unindexable except via purely-symbolic means)
            let (w', msg) = (ciltoarg_symb bindings w, "Cili_ldloca")
            let ld = fstpass.localnames.[w']
            let _ = if nonep ld then sf(dis + sprintf ": local variable %A not defined" w)
            let ans0 =
                let cez = f3o5(valOf ld) // old way - works not for structs
                let _ = vprintln 3 ("ldloca (pre star add) cez=" + ceToStr cez)
                cez
            let ans1 =
                match ans0 with
                    | CE_struct(_, sdt, _, _) ->
                        let idl = f2o5(valOf ld)
                        vprintln 3 ("Create pseudo local var for ldloca of struct " + hptos idl)
                        CE_var(f5o5(valOf ld), sdt, idl, None, [(g_purely_symbolic_under_reference, "true")]) 
                    | CE_var _ -> ans0
                    | CE_typeonly _ -> ans0                        
                    | other -> sf (msg + sprintf ":L3603 other" + ceToStr other)
            let ans = gec_CE_star sitemarker 1 ans1 // We add one ampersand for ldloca and we subtract a star on ldind.                            
            let _ =  // nop
               match realdo with
                    | Rdo_gtrace _ ->
                        // This could be embodied in gec_CE_star +1 code
                        //let _ = cg msg (K_pointout(kxr, "array_declaration", A_ind(A_loaf nemtok_region), A_loaf nemtok_content))
                        ()
                    | _ ->  ()
            let _ = vprintln 3 ("ldloca (post star add) cez=" + ceToStr ans)
            (ans::sp, env)

        | Cili_stsfld(ins_annotated_ast_dt_, (ty, frid, gargs)) -> 
            let (cr, idl) = fr33 ww true textcl.cm_generics (ty, frid, gargs)
            let (rhs, c') = g1()
            let callstring_r = "stsfld-rhs-type" :: callstring
            let msg = "stsfld"
            let _ = 
                match realdo with
                    | Rdo_tychk dlist ->
                        // We get here both the lhs and rhs types.  The lhs will generally have the volatile flag and we must take note of that (until such time as we add a pass that deduces volatile requirements from a composite kcode pass)
                        // The ins_annotated_ast_dt was only used on statics - the cr passed in on dynamics is ignored and misleading and even for static it does not have the correct volatile flag and other attributes, so use for disassembly compatibility only please.
                        let she = idl2cez_singleton false idl // Need this call still for static autodef. Statics cannot have unbound type formals and so this will be grounded and final even on first pass.
                        let (lhs_ats, dt) =
                            match she.cez with
                                | CE_var(_, dt, idl, fdto, ats) ->
                                    //let _ = dev_println (msg + sprintf " she.cez=" + ceToStr she.cez + " type=" + cil_typeToStr dt)
                                    (ats, dt)
                                | other ->
                                    sf (msg + sprintf " unexpect static field form: %s" (ceToStr she.cez))                                                            
                        let dct = field_type_check_and_drop ww msg (*thato=*)None dlist callstring gb0 dt [frid]
                        let rty = get_ce_type msg rhs
                        log_tval ww msg dlist callstring_r (gec_RN_monoty_ats ww rty lhs_ats) // A second dropping uses the _r suffix so that they are distinguished, but odd mix of lhs and rhs.
                        ()
                    | Rdo_gtrace _ ->
                        let (rty, lhs_ats) = mono_ats_type_retrieve ww msg gb0 callstring_r 
                        let (dt_record, field_dt, ptag_) = field_type_retrieve ww msg gb0 callstring 
                        stsfld_readldo_intcil_core msg sitemarker field_dt rty lhs_ats rhs idl
            (c', env)

        | Cili_stobj ctype ->  (* Two stack pops: first is instance. *)
            let (lhs, rhs, c') = g2()
            // stobj is used in test30r2 - for a structure assign :         30  : stobj (valuetype [test30r2]memop)
            let msg = "stobj"
            let callstring_r = "stobj-rhs-type" :: callstring
            let _ =
                match realdo with
                    | Rdo_tychk dlist ->
                        let rty = get_ce_type msg rhs
                        log_tval ww msg dlist callstring_r (gec_RN_monoty ww rty)
                        ()
                    | Rdo_gtrace _ ->
                        match lhs with
                            | CE_star(stars, CE_subsc(fat, ce, idx, sco), aid) when false && stars >= 1 -> // old clause
                                // Essentially remove CE_subsc and go down path of remaking it.
                                // idx is already scaled so pass noscale=true.
                               let lhs = ce
                               muddy "cg msg (K_as_vassign(kxr, stars, lhs, rhs, idx, sco))"

                            | CE_star(stars, ce, aid) when stars >= 1 ->
                                let rty = mono_type_retrieve ww msg gb0 callstring_r
(*
                                let _ =
                                    match is_struct_field_op msg lhs with
                                        | Some(packedf, nn, ptag, l0, dt_struct) -> dev_println (msg + sprintf "foo foo  Struct and packed=%A" packedf) 
                                        | _ -> dev_println "foo foo + " + cil_typeToStr rty

*)
                                let abuse = spot_type_abuse msg false g_ctl_object_ref rty
                                let full_structure_assign = full_struct_assign_pred msg ce
                                let as_gen (l, r) = cg msg (K_as_sassign(kxr, l, r, abuse))
                                if full_structure_assign then
                                    let dt_field = rty
                                    let msg = "Structure assign to instance field "
                                    let rlst = gec_field_snd0 ww (sitemarker + "-struct-R") msg dt_field None rhs
                                    let llst = gec_field_snd0 ww (sitemarker + "-struct-L") msg dt_field None ce
                                    let q = List.zip llst rlst
                                    let _ = vprintln 3 (msg + sprintf " %s   len=%i len=%i" (ceToStr ce) (length llst) (length rlst))
                                    app as_gen q
                                else
                                    as_gen (ce, rhs)

                            | arg -> // dead clause?  cac_posit uses it
                                
                                // We do not always physically see a star here: if it is pass by reference we'll just have the formal pram whose value is not eval'd until gtrace.
                                let _ = dev_println  ("stobj CIL instruction: bad argument (expect an address type for indirect store)  " + ceToStr lhs)
                                let rty = mono_type_retrieve ww msg gb0 callstring_r
                                let abuse = spot_type_abuse msg false g_ctl_object_ref rty
                                let ce = dereference_ptr sitemarker msg arg // Symbolic deref needed
                                cg msg (K_as_sassign(kxr, ce, rhs, abuse))


            (c', env)
                
        | Cili_stfld(dtype_, (ty, ptag, gargs)) ->  // Two stack pops: first is pointer to class instance or CE_struct immediate. Second is value to store, aka rhs.
            let msg = "stfld"
            let (structptr, rhs, sp') = g2() 
            let callstring_r = "stfld-rhs-type" :: callstring
            match realdo with
                | Rdo_tychk dlist ->
                    //vprintln 0 (sprintf "prefer/want to get from %A" structptr)
                    let (Some cr, idl_) = fr33 ww false textcl.cm_generics (ty, ptag, gargs) // prefer to get ty from the stack structptr ... TODO
                    // want the type - either half (fr33 or gec_field_tyo) can eval cgr? choose which progably the latter...
                    //vprintln 0 (sprintf "stfld cr=%A structptr=%A" cr structptr)
                    let _ = field_type_check_and_drop ww msg (Some structptr) dlist callstring gb0 cr [ptag]
                    let rty = get_ce_type msg rhs
                    log_tval ww msg dlist callstring_r (gec_RN_monoty ww rty)
                    (sp', env)
                | Rdo_gtrace _ ->
                    let rty = mono_type_retrieve ww msg gb0 callstring_r 
                    let (record_dt, field_dt, ptag_) = field_type_retrieve ww msg gb0 callstring 
                    let abuse = spot_type_abuse msg false field_dt rty
                    let singletonptr_o = ce_is_singletonptr cCB msg structptr
                    if not_nonep singletonptr_o then
                        let lhs_ats = [] // for now
                        let idl = ptag ::  valOf singletonptr_o  //muddy (sprintf " poggle ptag=%A   ptr=%A" ptag singletonptr_o)
                        stsfld_readldo_intcil_core msg sitemarker field_dt rty lhs_ats rhs idl
                    else
                        //  stfld/ldfld cgr note:
                        //  Although the dtype gives a good hint at the cgr involved, the necessary info should always already be findable via the this pointer on the stack although this seems tricky with delegates in use. 
                        // The 'this' pointer will give any cgr for the class we are storing in, but these are just bindings to be used for free cgr tprams of the field we are storing in, which
                        //  can be parameterised differently.   Alternatively, the rhs value on the stack should be concrete with no free cgr but this does not exist for a ldfld. 
                        // Final option is the field we are storing in has typeinfo as well.
                        // Can skip all this dct at a store
                        //vprintln 3 ("stfld structptr=" + ceToStr structptr + "   stfld dct=" + cil_typeToStr dct)

                        // Regarding structs: stfld can be doing an all-field structure assign or a structure field insert of a single field.
                        let lhs_cez = gec_field_snd_pass ww sitemarker msg record_dt structptr ptag
#if SPARE
                        let _ =
                            match is_struct_field_op msg lhs_cez with // NO this is the whole obj not its field! do not use.
                                | Some(packedf, nn, ptag, l0, dt_struct) -> dev_println (msg + sprintf " Struct and packed=%A" packedf) 
                                | _ -> ()
#endif
                        //vprintln 3 ("stfld lhs=" + ceToStr lhs)
                        let as_gen (l, r) = cg msg (K_as_sassign(kxr, l, r, abuse))
                        let full_structure_assign = full_struct_assign_pred msg lhs_cez
                        if full_structure_assign then // We have a seemingly arbitrary choice between expanding a parallel struct assign at kcode generation or kcode interpretation.  But doing it here, at generation times, wherever possible, means the dataflow analysis of the kcode is finer-grained.
                            let msg = "Structure assign to instance field "
                            let rlst = gec_field_snd0 ww (sitemarker + "-struct-R") msg field_dt None rhs
                            let llst = gec_field_snd0 ww (sitemarker + "-struct-L") msg field_dt None lhs_cez
                            let q = List.zip llst rlst
                            //vprintln 3 (msg + sprintf " %s %i %i" (ceToStr lhs_cez) (length llst) (length rlst))
                            app as_gen q
    //                  elif ct_is_valuetype dt_record then
    //                      muddy (sprintf "stfld: denote a field insert stfld vt on %s ptag=%s" (cil_typeToStr dt_record) ptag)
                        else as_gen(lhs_cez, rhs)
                    (sp', env) 

        | Cili_ldfld(dtype, (ty, fridl, gargs)) ->
            let msg = "ldfld"
            let (structptr, sp') = g1() 
            let (Some cr, ptag) = fr33 ww false textcl.cm_generics (ty, fridl, gargs)
            let singletonptr_o = ce_is_singletonptr cCB msg structptr
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        if false && not_nonep singletonptr_o then  
                            let dct = cil_type_n_gb ww realdo (gb0) dtype
                            muddy "poggle" 
                        else
                            //let (Some cr, idl_) = fr33 ww false textcl.cgr (ty, ptag) // prefer to get ty from the stack structptr ... TODO
                            // want the type - either half (fr33 or gec_field_tyo) can eval cgr? choose which progably the latter...
                        //let _ = vprintln 0 (sprintf "ldfld cr=%A structptr=%A" cr structptr)
                            let dct = field_type_check_and_drop ww msg (Some structptr) dlist callstring gb0 cr ptag
                            CE_typeonly dct

                    | Rdo_gtrace _ ->
                        let (ty_rec, field_dt, ptag_) = field_type_retrieve ww msg gb0 callstring
                        let debug_ce = structptr
                        //dev_println (msg + sprintf ": Exams: structptr %s singletonptrf=%A %A" (ceToStr structptr) singletonptr_o structptr)
                        let ce =
                            if not_nonep singletonptr_o then // TODO replicate on ldflda
                                let idl = ptag @ valOf singletonptr_o
                                vprintln 3 (msg + ": operating on a singleton struct, so should be sconed")
                                ldsfld_readldo_intcil_core "singleton-ldfld" sitemarker field_dt idl
                            // We can be loading a field from a struct or a field that is a struct: don't get confused.
                            // We can be loading a static field since our 'that' pointer might be originated from a ldsflda for a static struct.
                            // We dont need a case split here once struct_tag_list flattens out . TODO.
                            else
                            match get_struct_tag_listf msg structptr field_dt [] with  // ldfld and lfsfld expand structs to CE_struct. ldloc relies on its caller to have done this. ldloc had the expansion on local creation.
                                | Some fields__ ->
                                    let the_field = gec_field_snd_pass ww sitemarker msg ty_rec structptr (hd ptag) // Whole thing, as per None clause
                                    //let _ = dev_println (msg + sprintf ": get_struct_tag_list fields are %A" (sfold (fun (tagid, dt) -> tagid + ":" + cil_typeToStr dt) fields))
                                    // let items = map (fun (tagid, dt_) -> gec_field_snd_pass ww sitemarker ("L2750" + msg) field_dt she.cez [tagid]) fields) - not relevant for non-singelton.
                                    let fields = struct_get_raw_items msg field_dt
                                    let idtoken = funique "WONTWOK"
                                    let ce = CE_struct(idtoken, field_dt, idtoken, map (fun (tagid, dt, _) -> (tagid, dt, Some(gec_field_snd_pass ww sitemarker ("L2724" + msg) field_dt the_field tagid))) fields)
                                   
                                    // Invoke the technique that should also work for ldsfld and result in simper code where we pass in no tagidl to get all tags.
                                    //let items = gec_field_snd_pass ww sitemarker ("L2977" + msg) field_dt the_field ""
                                    //let ce = items // already applied CE_struct (field_dt, Some items)

                                    let _ = vprintln 3 ("ldfld: loaded a struct from another struct/class. ans ce=" + ceToStr ce)
                                    ce
                                | None -> gec_field_snd_pass ww sitemarker msg ty_rec structptr (hd ptag) // Normal handle
                        //let _ = vprintln 3 ("ldfld: loaded ce=" + ceToStr ce)
                        ce
            (ans :: sp', env)

        | Cili_ldflda(dtype, (ty, ptag, gargs)) ->
            let (structptr, sp') = g1()
            let msg = "ldflda"
            let (_, idl) = fr33 ww false textcl.cm_generics (ty, ptag, gargs)
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let dct = field_type_check_and_drop ww msg (Some structptr) dlist callstring gb0 dtype [ptag]
                        gen_typeonly(1, dct) // One additional star dropped.
                        
                    | Rdo_gtrace _ ->
                        let (dt_record, _, _) = field_type_retrieve ww msg gb0 callstring 
                        let a0 = gec_field_snd_pass ww sitemarker msg dt_record structptr (hd idl)
                        //     let oc__ = id_lookup ww (idl, [], fun()->(dis + " : item not defined\n", 1)) 
                        //     else reference_ptr(sf " add_struct_tag CF (structptr, idl)) ") 
                        gec_CE_star sitemarker 1 a0

            (ans::sp', env)

        | Cili_ldsfld(dtype, (ty, ptag, gargs)) -> 
            let msg = "ldsfld"
            let (_, idl) = fr33 ww true textcl.cm_generics (ty, ptag, gargs)
            // Spot certain 'magic' static fields.
            // TODO these wont work for ldsflda and so on ... abstract and unify please.
            if idl = g_kiwi_tnow_path then // We, somewhat simplisticly,  convert KiwiSystem.Kiwi.tnow to the g_now_string which is "hpr_tnow" 
                let _ = vprintln 3 (sprintf "Special handling for %s invoked" (hptos idl))
                ((tnow_cez ww realdo m0)::sp, env) 
            elif idl = [ "Empty"; "String"; "System" ] then ((gec_ce_string "")::sp, env)             
            else
            //vprintln 3  ("ldsfld   idl=" + hptos idl)
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let dct = field_type_check_and_drop ww msg None dlist callstring gb0 dtype [ptag]
                        CE_typeonly dct                        
                    | Rdo_gtrace _ ->
                        let (ty_rec, field_dt, ptag_) = field_type_retrieve ww msg gb0 callstring
                        ldsfld_readldo_intcil_core msg sitemarker field_dt idl
            (ans :: sp, env)

        | Cili_ldsflda(dtype, (ty, ptag, gargs)) ->
            let msg = "ldsflda"
            let (_, idl) = fr33 ww true textcl.cm_generics (ty, ptag, gargs) //TODO ban textcl.cgr here
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let dct = field_type_check_and_drop ww msg None dlist callstring gb0 dtype [ptag] // Dropped type is for the field itself.
                        //let _ = vprintln 3 (msg + " first pass type pre star add is " + cil_typeToStr dct)
                        gen_typeonly(1, dct) // One additional star dropped.
                        // We may want to implement array decay here : loading an array base is the same as loading an array: they both decay to a pointer to the first element in C++ unless sizeof or address-of is directly applied.
                    | Rdo_gtrace _ ->
                        let (ty_rec, ty_fld_, _) = field_type_retrieve ww msg gb0 callstring
                        let static_she = idl2cez_singleton true idl
                        if static_she.labelled_literal then vprintln 3 (msg + " labelled_literal old/disabled bypass of gec +1 star")// else vprintln 3 ("did not bypass")
                        if false && static_she.labelled_literal then static_she.cez else gec_CE_star sitemarker 1 static_she.cez 
            //vprintln 0 (msg + sprintf ": ans=%s ans_dt=%s" (ceToStr ans) (cil_typeToStr(get_ce_type msg ans)))
            (ans::sp, env) 

        | Cili_ldelem ctype_ ->  
            // Where we have 'ldelem .ref' we clearly want to take the type from the left-hand arg rather than the type annotation in the instruction.
            let mf() = m0() + ":ldelem"
            let (array, subs, c') = g2() 
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let ce1 = gec_CE_star sitemarker -1 array                        
                        //Prev: totally different styles in ldelem and ldelema but now the same save a deref.
                        //let (dct, tyvars) = cil_type_n_gb ww realdo gb0 ctype_
                        //log_tval ww msg dlist callstring (qqx_a ww msg tyvars dct) // not retrieved at the moment.
                        //let _ = vprintln 3 ("first deref (remove star) gives " + ceToStr ce1)
                        let ce = dereference_ptr sitemarker "ldelem" ce1
//                      let _ = log_pval ww dlist callstring ce // not retrieved later
                        ce 

                    | Rdo_gtrace _ ->
                        let ce' = if true then array else dereference_ptr sitemarker "ldelem" array (* We use this because when ctype=.ref it is not very helpful *)
                        let ce2 = gec_idx ww sitemarker mf ce' None subs
                        //let _ = vprintln 3  (mf() + ":  loaded ce=" + ceToStr ce2)
                        ce2
            (ans::c', env)

        | Cili_ldelema ctype_ ->  
            let mf() = m0() + ":ldelema" // Load element address
            let (array, subs, c') = g2() 
            //let _ = vprintln 3 ("ldelema : base=" + ceToStr_full array)
            let ans =
                match realdo with
                    | Rdo_tychk dlist ->
                        let rec ldelema_ty_1 = function
                            | CT_star(_, dt) -> ldelema_ty_1 dt
                            | CT_arr(dt, _) -> dt
                            | other -> sf ("ldelema typecheck other type " + cil_typeToStr other)
                        let ce = 
                            match array with
                                | CE_typeonly dt ->
                                    let dt1 = ldelema_ty_1 dt
                                    CE_typeonly(gec_CT_star 1 dt1)
                                | other -> sf ("ldelema typecheck other ce form " + ceToStr other)
                        //let ce = gec_CE_star 1 array
                        let _ = log_pval ww dlist callstring ce // not retrieved later
                        ce
                    | Rdo_gtrace _ ->
                        let r = gec_idx ww sitemarker (mf) array None subs 
                        let ce = gec_CE_star sitemarker 1 r
                        //let _ = vprintln 3  (mf() + ":  loaded ce=" + ceToStr ce)  
                        ce
            (ans::c', env)

        | Cili_box_any cr 
        | Cili_box cr -> // Convert a valuetype to a corresponding class instance (e.g. int to Int).
            let (x, sp') = g1()
            let ans = // we implement this just as a cast - nothing actually happens.
                match realdo with
                    | Rdo_tychk dlist ->
                        let (dt, _) = cil_type_n_gb ww realdo gb0 cr // contents_type
                        log_tval ww msg dlist callstring (gec_RN_monoty ww dt) 
                        CE_typeonly dt

                    | Rdo_gtrace _ ->
                        let ct = mono_type_retrieve ww msg gb0 callstring 
                        match x with
                            | CE_region _
                            | CE_var _      -> x
                            | CE_x(old_type_, vale) -> CE_x(ct, vale)
                            | other ->
                                //The removal of the CE_tv form means that 'casting' is no longer generally representable.  Does not matter I think.
                                //let _ = vprintln 1 ("+++ could not box this " + ceToStr other)
                                other
            (ans::sp', env)

            // A suitable Kiwi semantics is that box does nothing (except cast)  and unbox does the opposite to ldind.
            // This unbox implementation is simply designed to amount to a NOP when followed by a ldind.
        | Cili_unbox_any cr
        | Cili_unbox cr ->  
            let (d, c') = g1()   (* Unbox actually adds the reference that was nominally done by box. *)
            let unboxf ce = gec_CE_star sitemarker 1 ce
            let ans = unboxf d
            (ans::c', env)

        | Cili_ldobj ctype -> 
            let (d, c') = g1() 
            let ans = dereference_ptr sitemarker "ldobj" d
            (ans::c', env)

        | Cili_ldind ctype -> 
            let (d, c') = g1() 
            let ans = dereference_ptr sitemarker "ldind" d
            (ans::c', env)

        | Cili_stind ctype_ ->  // Compared with ldelem and stelem, there is no subscript scaling for ldind and stind: a pointer dereference is all thats needed.
            let msg = "stind"
            let (lhs, rhs, c') = g2() 
            let mf () = m0() + ":" + kxrToStr kxr + ("stind, lhs=" + ceToStr lhs + ", rhs=" + ceToStr rhs)
            let callstring_r = "stind-rhs-type" :: callstring
            let _ = 
                match realdo with
                | Rdo_tychk dlist ->
                    let lty = get_ce_type msg lhs
                    log_tval ww msg dlist callstring (gec_RN_monoty ww lty)
                    let rty = get_ce_type msg rhs
                    log_tval ww msg dlist callstring_r (gec_RN_monoty ww rty)
                    ()
                | Rdo_gtrace _ ->
                    let fp_dt = mono_type_retrieve ww msg gb0 callstring 
                    // stind: v1: convert the pointer to a noscale point to the wondarray of the appropriate type. - but a problem with scaling ..?
                    // stind: v2: make an sassign to a star - its just a matter of who makes the wondtoken
                    let rty = mono_type_retrieve ww msg gb0 callstring_r                     
                    let abuse = spot_type_abuse msg true fp_dt rty
                    match lhs with // lhs 
                        | CE_star(stars, ce, aid) -> 
                            if stars >= 1 then // exactly one really being removed
                                cg msg (K_as_sassign(kxr, ce, rhs, abuse))
                            else sf (mf() + "\n" + dis + ": lhs not a pointer")


                        | arg when false -> // old - disabled - the only clause that used rnsc? - so that attribute can now be ignored?
                            // Idiomatic wondtoken multi-copied code - please tidy
                            let idl = [] // idl is always blank for a wondtoken! TODO tidy up
                            let uid = { f_name=idl; f_dtidl=idl; baser=0L; length=None; } 
                            let lhs_base = gec_CE_Region ww (*rnsc=*)true (None, remove_CT_star msg fp_dt, uid, [], None)
                            //vprintln 0 (msg + sprintf " clause 3")
                            let aid_bo = oapp (anon_dereference_aid_core sitemarker msg) (ce_aid0 mf lhs)
                            cg msg (K_as_vassign(kxr, lhs_base, rhs, lhs, aid_bo))

                        | lhs (* when not(isIndexable(get_base64 lhs)) *)  -> // new
                            let ce1 = gec_CE_star sitemarker -1 lhs
                            cg msg (K_as_sassign(kxr, ce1, rhs, abuse))
                            
  
            (c', env)

        | Cili_stelem ctype_  ->  // Three items on stack: Base, index and value to store (aka rhs).
        //  TODO : Need to spot_abuse here too, but vassign does not hold it.
            if tychk_pass_pred realdo then (disc3(), env)  else 
            let msg = dis
            let (lhs, subs, vale, c') = g3() 
            let mf () = m0() + ":" + (sprintf "intcil: stelem 8/4 lhs=%s subs=%s rhs=%s" (ceToStr lhs) (ceToStr subs) (ceToStr vale))
            let aid_oo = oapp (anon_deca_for_vassign (sitemarker + "O")) (ce_aid0 mf subs)
            let aid_o = oapp (fun aid -> A_subsc(aid, aid_oo)) (ce_aid0 mf lhs)
            //vprintln 0 (msg + sprintf ": aid_b=%s     aid_o=%s" (sfold aid2s aid_b) (sfold aid2s aid_o))
            cg msg (K_as_vassign(kxr, lhs, vale, subs, aid_o))
            (c', env)

        | Cili_pop ->
            if sp=[] then sf("pop empty stack " + dis)
            else 
              let (v, c') = g1()
              let _ = tidy_disposal kxr v
              (c', env)

        | Cili_dup ->
            let (arg, c') = g1()
              // The fastspill avoids the wrong answer in postfix plusplus of value types.
              // For vv++ the C# compiler loads vv and dups it but our symbolic load loads its address (essentially) on our stack and we need its value, not its address to be duplicated.
              // For foo[ee]++ we have loaded an address on the stack anyway, so do not make a fastspill.
(*
   int qq = sizz4[1] ++;
        IL_0007:  ldloc.1 
        IL_0008:  ldc.i4.1 
        IL_0009:  ldelema [mscorlib]System.Int32
        IL_000e:  dup 
        IL_000f:  ldind.i4 
        IL_0010:  dup 
        IL_0011:  stloc.3 
        IL_0012:  ldc.i4.1 
        IL_0013:  add 
        IL_0014:  stind.i4 
        IL_0015:  ldloc.3 
        IL_0016:  stloc.2 
*)
              // Overall, this is a bit of a kludge - what are the formal semantics - whenever a value is not popped immediately in the next instruction we should insert a fastspill and then trim them all with dataflow analysis at the end.
            let fastspill dt arg = // TODO use common code
                  match arg with
                  | CE_star(n, ce, aid) when n >= 1 ->
                      //let _ = vprintln 3 ("fastspill bypass dup owing to being a pointer")
                      (true, arg) // Bypass fastspill kludge when a pointer.
                  | ce ->
                      let ww = WN "fastspill_dup" ww
                      let idl = [ funique "fastspill_dup" ]
                      yout fvd ("fastspill for dup " + hptos idl + " dt=" + typeinfoToStr dt + " ce=" + ceToStr ce) 
                      //let _ = vprintln 0 ("fastspill for dup " + hptos idl + " dt=" + typeinfoToStr dt + " ce=" + ceToStr ce)
                      let _ = if dt=g_ctl_object_ref then dev_println("fastspill object for dup " + hptos idl + " dt=" + typeinfoToStr dt + " ce=" + ceToStr ce) // Clearly object references are sometimes duplicated but this may indicate a bug or lazy programming elsewhere.
                      let ats = [] //let dtidl = ct2idl ww dt
                      let storemode = STOREMODE_singleton_scalar
                      let vrn = alloc_fresh_vreg ww cCBo realdo "fastspill_dup" dt idl storemode 
                      let cez =
                          let (blobf, cez, vrnl_) = gec_singleton ww (*a1*)vd cCBo msg "fastspill_dup" ats realdo m0 "" idl dt None (Some false) None
                          cez

                      (false, cez)
            let v' = 
                match realdo with
                    | Rdo_tychk dlist ->
                        let dt = ce2ctype arg
                        log_tval ww msg dlist callstring (gec_RN_monoty ww dt) 
                        CE_typeonly dt
                        
                    | Rdo_gtrace _ ->    
                        let dt = mono_type_retrieve ww msg gb0 callstring 
                        let (bypassed, v') = fastspill dt arg
                        let _ = if not bypassed then cg msg (K_as_sassign(kxr, v', arg, None))
                        v'
            (v' :: v' :: c', env)

        | Cili_newarr cr ->
            let (len, c') = g1()
            let (ans, env) =
                match realdo with
                    | Rdo_tychk dlist ->
                        let (ct, tyvars_) = cil_type_n_gb ww realdo gb0 cr // contents_type
                        log_tval ww msg dlist callstring (gec_RN_monoty ww ct) 
                        // Replicated on codegen pass:
                        let adt = CT_arr(ct, None)  // unsized array - len may not be known during this pass
                        let ptr_dt = gec_CT_star 1 adt
                        let a = gen_typeonly(0, ptr_dt) // or (1, adt)
                        (a, env)

                    | Rdo_gtrace _ ->
                        let ct = mono_type_retrieve ww msg gb0 callstring 
                        let aid_root = [  callsiteToStr callsite ; "CS" ]
                        // We can make sc nemtoks pre or post static elaboration - post provides more diversity and this will reduce to the pre case if there are data paths between the elaborated call sites.  Each cg of a CE_newarr is a post elaboration callsite, so we do it here and make sure newobj borrows the nemtok created here.
                        let ident = funique ("aitem") 
                        let nemtok_region = hptos(ident :: aid_root)
                        let nemtok_content = nemtok_region + "_content"
                        let (hintname, ats) = get_hintname ("-NOHINT-", []) (ins, hints) // This is not working if you see -NOHINT- in the answer
                        let mx = "newarr instruction: dt=" + typeinfoToStr ct + ", hintname=" + hintname + ": ats=" + cez_ToStr_atts ats
                        let hintname = if hintname= "-NOHINT-" then None else Some hintname
                        youtln fvd mx
                        vprintln 3 mx
                        let adt = CT_arr(ct, None)  // unsized array - len may not be known during this pass
                        let ptr_dt = gec_CT_star 1 adt
                        let newarg = KN_newarr(ct, len,  nemtok_content, hintname, ats)
                        // msg (K_pointout(kxr, "array_declaration", A_tagged(A_loaf nemtok_region, g_indtag, ident), A_loaf nemtok_content))
                        cg msg (K_pointout(kxr, "array_declaration", A_subsc(A_loaf nemtok_region, None), A_loaf nemtok_content))                        
                        let unrv =  // (Nearly) Replicated reftran code - in this variant there is a one star difference to include
                            let reftran_aid =  funique ("refxxarray") :: aid_root  // Need a different aid since the types are different by one star
                            // This call to gec_uav simply creates a singleton to hold the result of the CE_newarr call.  The array storage is not itself allocated until gtrace.
                            let atakenf = Some false
                            let (blobf, cez, vrnl_) = gec_singleton ww vd cCBo msg "refxarray" ats realdo m0 "" reftran_aid ptr_dt None atakenf None
                            cez

                        //unrv=uniqify-non-ref-transparent-variable
                        //else store into holding reg for non-ref-trans operator.
                        cg msg (K_new(kxr, unrv, nemtok_region, newarg))
                        (unrv, env)
                        
            (ans::c', env)

        | Cili_call(opcode, ck, rt, fr, signat, callsite_token) -> 
            let virtualf = opcode=KM_virtual
            let argcnt = length signat
            let voidf = not(nonvoid rt)
            //Classgeneric actuals can be pulled out from fr because the C# compiler (or cecil) puts them in. TODO: dont recalc these later, what about resolution bindings in choosing a method?  what about genuinely polymorphic code?  These are no mgrs in the fifo one-place channel examples. Best make a real test...
            let (cr_, idl) = fr33 ww true textcl.cm_generics fr
            let ids = hptos idl
            let arity = length signat
            let ncp = callsite_token :: "of" :: callstring 
            let m = (if virtualf then "Cili_callvirtual " else "Cili_call ") + ids + " ncp=" + hptos ncp
            let ww = WF 3 (if tychk_pass_pred realdo then "typecheck" else "gtrace") ww m
            let (instancef, ohead) = thisohead(opcode, ck)
            let (bifo, idl) =
                let (usercall_override, usercall_class) = // This is a KiwiC override/redirection not an OO override. Perhaps rename.
                    // get_Length is 1-D array property getter
                    // GetLength(dim) returns length in a given dimension from multi-dim arrays but needs mapping down to primitives for 1-D arrays.
                    if instancef && (hd idl = "get_Length" || hd idl = "GetLength"|| hd idl = "get_Rank") then
                        let rec pokidl = function // TODO use generic backconvert from CTL_net to idl form ... c.f. ot_table
                            | CT_arr (CTL_net (_,16, Signed, nats), _) when nats.nat_char -> ["String"; "System"]
                            | CT_arr _ -> [ "Array"; "System" ]
                            | CT_cr crst -> crst.name
                            | CT_star(_, dt) -> pokidl dt
                            | CTL_record(record_idl, cst_, lst, len, ats, binds, srco) -> record_idl
                            | other ->
                                dev_println (sprintf "other form in bifo GetLength pokidl: %s" (cil_typeToStr other))
                                []
                        let (sp__, args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                        let dt = get_ce_type "bifo check" (hd args)
                        let dt_idl = pokidl dt
                        let mdnames = valOf(!g_kfec1_obj).multid_names
                        //let _ = vprintln 0 (sprintf "virtual mdnames: %A cf %A" dt_idl mdnames)
                        let usercall_override = list_intersection(mdnames, dt_idl)
                        //let _ = vprintln 0 (sprintf "virtual for %s:  user sub override on builtin bif=%A" ids usercall_override)
                        (not_nullp usercall_override, usercall_override)
                    else (false, [])
                if usercall_override then (None, [ hd idl; hd usercall_class]) 
                else (g_kiwi_canned_bif.TryFind (idl, instancef, arity), idl) //  built-in method lookup

            if idl = [ "get_Rank"; "Array"; "System"; ] && not_nonep bifo then
                // This was just tested above! Is this the best site for a canned function? There are others afterall.
                let ans = gec_yb(xi_num 1, g_canned_i32)
                //let _ = vprintln 3 (sprintf "One-dimensional array, get_Rank ans=%s" (ceToStr ans))
                let (sp, args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                (ans::sp, env)
                
            else
            let ids = hptos idl
            //vprintln 0 (sprintf "built-in function option (bifo) for %s is bif=%A ohead=%i" ids (not_nonep bifo) ohead)
            let (class_dt, rt, signat', cg_args, mg_args) =
                match realdo with
                    | Rdo_tychk dlist ->
                        // Peek args to get class_dt, but discard pop effect.
                        let (sp__, args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                        let (class_dt, rt, tyvars_, signat', cg_args, mg_args) = tycheck_fcall ww msg fr bifo rt ohead signat (if instancef then Some(hd args) else None)
                        (class_dt, rt, signat', cg_args, mg_args)
                    | Rdo_gtrace _ ->
                        let (class_dt, rt, _, signat', cg_args, mg_args) = call_type_retrieve ww "call" gb0 ncp
                        let __ =
                            if instancef then
                                let (sp__, args) = gn_instance argcnt sp
                                //let _ = vprintln 0 (sprintf "dispatch class_dto=%A but that=%A" ((valOf class_dto)) (hd args))
                                ()
                        (class_dt, fst(concrate ww msg rt), map (concrate ww msg >> fst) signat', cg_args, mg_args)

            let m_notice = ref None
            let usersubs = // Some Kludging here - multidarray classes provide 'item'
                          // We map Set to set_Item and get to get_Item
                          // In general, we should look at the IsSetter and IsGetter attributes, but here we munge
                if not_nonep bifo then []
                else
                    let handler = fun(nd:int)->(dis + "\n" + hptos idl + sprintf " : callee not (uniquely) defined.  Number of defs=%i.\n" nd, 1)
                    let usf idl handf =
                        match id_lookup_fn ww cCP cCB handf m_notice virtualf callsite idl signat' with
                            | [item] -> [item]
                            | items ->
                                vprintln 3 (sprintf "need to dyndispatch %s over %s " (hptos idl) (sfold (fst >> hptos) items))
                                items
                    let h1 = if hd idl = "Set" || hd idl = "Get" then fun (nd:int)->("", 1) else handler 
                    let ans = usf idl h1
                    if nullp ans && hd idl = "Set" then usf ("set_Item" ::(tl idl)) handler // Pop in alternatives if not found - TODO map this earlier in firstpass .. e.g. add ref cell annotation to Cili_call
                    elif nullp ans && hd idl = "Get" then usf ("get_Item" ::(tl idl)) handler
                    else ans // for now
            let get_mdt = function
                | No_method(srcfile, flags, ck, id, mdt, flags1, instructions, atts) -> mdt
                | _ -> sf "get_mdt usig other: method expected"

            // See if making a remote call to another IP block.
            let (client_stub, fast_bypass) =
                if length usersubs <> 1 then (None, None)
                else
                    let mdt = get_mdt(snd(hd usersubs))
                    (mdt.is_remote_marked, mdt.is_fast_bypass)

            let (instancef, ohead) = thisohead(opcode, ck)

            match realdo with
                | Rdo_tychk dlist ->
                    let push_return_ty sp = function // or use voidf
                        | CTL_void -> sp
                        | other    -> CE_typeonly(other)::sp
                    let tyvars = []   // for now - this code is not quite tidy! TODO delete this                        

                    let mine_class_dt_spog thato class_dt = // Give precedence to stacked items over cecilfe annotations.
                        match thato with 
                            | None ->
                                //vprintln 0 (sprintf "mine_class_dt_spog: no thato: class_dt=%s" (drToStr class_dt))
                                match class_dt with
                                    | RN_monoty(ty, _) -> ty
                                    | other -> muddy (m + sprintf ": mine_class_dt_spog other class_dt=%A" class_dt)
                            | Some(CE_typeonly ty) -> ty
                            | Some other ->
                                vprintln 3 (m + sprintf ": mine_class_dt_spog: from that: other : that=%A  class_dt=%A" other class_dt)
                                get_ce_type "mine_class_dt_spog" other
                                
                    match bifo with // Another strangely formed match statement
                        | Some bif ->
                            //vprintln 0 (sprintf "bif minirep voidf=%A %s hpr_name=%s instancef=%A arity=%i rt=%s" voidf (hptos bif.uname) bif.hpr_name bif.instancef arity (cil_typeToStr rt))
                            let (sp', args) = (if bif.instancef then gn_instance argcnt sp else gn_static argcnt sp)
                            let (thiso, args) = (if bif.instancef then (Some(hd args), tl args) else (None, args))
                            let _ = log_call_types ww "bif" dlist ncp (mine_class_dt_spog thiso class_dt) tyvars cg_args mg_args rt args
                            if idl = [ "KppMarkPrimitive"; "Kiwi"; "KiwiSystem" ] then setwaypoint ww realdo linepoint5 args
                            (push_return_ty sp' rt, env)

                        | None when not (nonep(builtin_v idl)) -> // varargs ones (old)
                            let (sp', args) = gn_static argcnt sp
                            let thato = None // These are all static
                            let _ = log_call_types ww "builtin_v" dlist ncp (mine_class_dt_spog thato class_dt) tyvars cg_args mg_args rt args
                            (push_return_ty sp' rt, env)

                        | None when not_nonep client_stub ->
                            let (sp', args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                            let (thato, args) =
                                if instancef then
                                    vprintln 0 (sprintf "CLient demultiplexed dispatch? %s arity=%i,%i" (hptos ncp) (length args) argcnt)
                                    (Some(hd args), tl args)
                                else (None, args)
                            //vprintln 0 (sprintf "debug strange drop %s arity=%i,%i" (hptos ncp) (length args) argcnt)
                            ignore(log_call_types ww "rpc_client" dlist ncp (mine_class_dt_spog thato class_dt) tyvars cg_args mg_args rt args)
                            (push_return_ty sp' rt, env)
                            
                        | None when not_nullp usersubs ->
                            //vprintln 0 (sprintf "usersub ohead=%i " ohead)
                            let (sp', args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                            let (thato, args) = (if instancef then (Some(hd args), tl args) else (None, args))
                            let rt1_ = log_call_types ww "usercall" dlist ncp (mine_class_dt_spog thato class_dt) tyvars cg_args mg_args rt args
                            //dev_println (sprintf "tyvars=%A mgr=%A " tyvars mg_args)
                            //dev_println (sprintf "Need to postproc rt=%s rt1=%s" (cil_typeToStr rt) (drToStr rt1_))
                            (push_return_ty sp' rt, env)

                        | other ->
                            //let ctt a = cil_typeToStr(get_ce_type "nsm-msg" a)
                            sf("typecheck method call of " +  ids + sprintf " not implemented (L3275) %s. Static_wanted=%A Actual arg types are %s" (valOf_or_ns !m_notice) (not instancef) (sfold cil_typeToStr signat'))

                | Rdo_gtrace _ ->
                    if not_nullp usersubs then ff(sprintf "Usersub %s voidf=%A instancef=%A\n" (hptos idl) voidf instancef)
                    vprintln 3 (sprintf "Marked as locally-called method in this compilation: method-name=%s (all overloads)" (hptos idl))
                    mutadd cCC.m_called_methods idl // Mark as locally-called (so cannot be a toplevel remote or root entry point).
                        
                    let rec mk_usercall_wrapper ww signat' (cgr, mgr) thato xstack ck (idl, us) =
                        let mgr_formals = (get_mdt us).ty_formals
                        //let _ = vprintln 0 (sprintf "us=%A ty_formals=%A" us mgr_formals)
                        let mgr_zipped =
                            if length mgr_formals <> length mgr then
                                let _ = vprintln 3 (m + sprintf ": not zipping mgr %i cf %i - ok for polypara array handling" (length mgr_formals) (length mgr))
                                []
                            else List.zip mgr_formals mgr  // Zip method generics
                        let class_dt = if nonep thato then (fatal_dx msg "" class_dt) else (get_ce_type "class_dt for mk_usercall" (valOf thato))
                        mk_usercall (WN m ww) signat' (Some(ncp, mk_usercall_wrapper)) realdo linepoint_stack (xstack, (false, class_dt, thato), voidf, virtualf, ck, rt, (idl, us), mgr_zipped, [])

                    let ans =
                        match bifo with // This is the bif2 match statement which is somewhat odd coding style.
                           | Some bif -> //TODO abstract this to make more readable and use the table info
                                //let _ = vprintln 0 (sprintf "bif2 id2=%s  uname=%s hpr_name=%s instancef=%A" (hd idl) (hptos bif.uname) bif.hpr_name bif.instancef)
                                let cf = g_null_callers_flags
                                let (sp', args) = (if bif.instancef then gn_instance argcnt sp else gn_static argcnt sp)
                                let _ = if idl = [ "KppMarkPrimitive"; "Kiwi"; "KiwiSystem" ] then setwaypoint ww realdo linepoint5 args
                                let ans = bif_dispatch ww vd cCC cCB cCP cf mk_usercall_wrapper cg sp' kxr gb0 msg ncp callsite callstring sitemarker realdo idl textcl m0 dis virtualf bif cg_args mg_args voidf signat' args
                                (ans, env) 
                                
                            | None when not_nonep fast_bypass ->
                                let instancef = false // Alternatives may exist in the future.
                                let (sp', args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                                let _ = vprintln 3 (sprintf "fast_bypass n_bodies=%i n_args=%i bool=%A" (length usersubs) arity instancef)
                                if length args=1 then
                                    let vfp = invoke_hpr_fast_bypass idl ids (valOf fast_bypass) rt (hd args)
                                    (vfp :: sp', env) 
                                else cleanexit(kxrToStr (KXLP5 linepoint5) + ": precisely one arg expected for " + ids)

                            | None when not_nonep client_stub -> // Call to an external IP block.
                                let aid = [ funique ("$dereftrc"); callsiteToStr callsite ; "CS" ]
                                let (class_dto_, return_dt, _, signat, cgr, mgr) = call_type_retrieve ww msg gb0 ncp
                                let (sp', args) = (if instancef then gn_instance argcnt sp else gn_static argcnt sp)
                                let arg_types =
                                    let dern_ty = function
                                        | RN_monoty(ty, _) -> ty
                                        | other -> sf "other signature type form L4568"
                                    map dern_ty signat

#if OLD
                                let arg_bindings =
                                    let silly_unbind (ty, arg) = arg // Odd we used to zip this and then discard again!
                                    let (l1, l2) = ((if instancef then 1 else 0) + length signat, length args)
                                    if l1 <> l2 then sf (m + sprintf ": Poor RPC arity: dropped=%i cf stub=%i, ncp callstring=%s, for %A" l1 l2 (hptos ncp) client_stub)
                                    let arg_types = if instancef then (valOf_or_fail "L4612" class_dto_)::arg_types else arg_types
                                    map silly_unbind (List.zip arg_types args)
#endif
                                let arg_bindings = args
                                let (prams, protocol, searchbymethod, externally_instantiated, fsems) = parse_kiwi_remote_protocol_attributes ww m (valOf client_stub)

                                // These to be taken from IP-XACT wrapper not here when calling pre-compiled unit.
                                let meta_version = valOf_or (op_assoc "version" prams) "0.1"
                                let meta_library = valOf_or (op_assoc "library" prams) "blank"                                
                                let method_name = hd idl
                                vprintln 3 (sprintf "subsidiary ip call: protocol=%s  externally-instantiated=%A  searchbymethod=%A" protocol externally_instantiated searchbymethod +
                                            sprintf " idl=%s hpr_name=%s instancef=%A. note=note"  (hptos idl) method_name instancef)
                                // instance name will be final for a static block
                                let block_kind =
                                    if instancef then (tl idl) // dynamic object block - will be later load balanced or demultiplexed so instance name is a placeholder
                                    else (tl idl)              // If a static block
                                let overload_suffix = if fsems.fs_overload_disam then "_" + squirrel_port_signature arg_types else ""
                                let result = 
                                    if protocol <> "HSIMPLE" then // Only calls to remote methods via the old HSIMPLE mechanism are implemented within kiwife - the new ones are handled in restructure.
                                       let instance_name = if searchbymethod then hptos idl else hptos(tl idl)
                                       //vprintln 3 (sprintf "new-style RPC call not using HSIMPLE protocol")
                                       let sq = if overload_suffix="" then None else Some overload_suffix
                                       let res = invoke_subsidiary_ip ww (protocol, externally_instantiated, fsems) block_kind sq instance_name method_name ids rt args // new style - Pass down recipe for restructure to instantiate. 
                                       let arg_ats = [] // always null TODO
                                       if voidf then None else Some(arg_ats, res)
                                    else
                                        vprintln 1 (* hpr_yikes *) (sprintf "old-style RPC call using HSIMPLE protocol")
                                        let meta = (voidf, length signat)
                                        let callflags = g_null_callers_flags
                                        let v = 
                                            match op_assoc idl !g_hsimple_servers with
                                                | Some v -> v
                                                | None ->
                                                    let usig = function
                                                        | No_method(srcfile, flags, ck, (gid, uid, _), mdt, flags1, instructions, atts) -> mdt.arg_formals
                                                        | _ -> sf "usig other: hsimple: method expected"
                                                    let (retnet, callnets, arg_nets, assigns_, _) = generate_rpc_cs_terms (WN "clientcall" ww) cCBo kxr realdo None (idl, (*meta,*) usig(snd(hd usersubs)), rt, overload_suffix) true fsems
                                                    let v:scall_t =
                                                        { a1=         meta
                                                          arg_nets=   arg_nets
                                                          hs_nets=    callnets
                                                          rn=         retnet
                                                          idl=        idl
                                                          voidf=      voidf
                                                        }
                                                    if v.a1 <> meta then sf("subsequent call has different signature")
                                                    mutadd g_hsimple_servers (idl, v)
                                                    v
                                        cg msg (K_easc(KXR_dot(idl, kxr), CE_server_call("system-servercall", instancef, callflags, arg_bindings, v, args), true)) // Put cf in here
                                        if voidf then None
                                        else
                                            let arg_ats =
                                                match v.rn with
                                                    | Some (_, _, X_bnet f, _) -> [] // Always null. TODO. Wants string*string list instead.
                                                    | None ->  sf "null form retnet"
                                                    | _    -> sf "other form retnet"
                                            let dd = f3o3(f2o4(valOf v.rn))
                                            Some (arg_ats, dd)

                                match result with
                                    | None -> (sp', env)
                                    | Some(arg_ats, dd) ->
                                        // All old HSIMPLE server calls were considered non-referentially transparent hence we save once here... TODO not always needed. look at fs_reftran
                                        let ans = block_reftran_fastspill ww vd m0 cCBo realdo kxr (hptos aid) arg_ats cg (rt, dd)
                                        (ans::sp', env) 



                            | None when not_nonep (builtin_v idl) -> buf_v(idl, argcnt, gn_static argcnt sp, rt, voidf)
                            | None when not_nullp usersubs ->
                                match length usersubs with
                                    | 0 -> sf "unreachable missing usersub zero"
                                    | 1 -> mk_usercall_wrapper ww signat' (cg_args, mg_args) None sp ck (hd usersubs)
//                                  | _ -> mk_usercall_wrapper ww signat' (cgr, mgr) None (hd usersubs)
                                    | n ->
                                        let that = hd (snd(gn_instance argcnt sp))
                                        mk_usercall_wrapper ww signat' (cg_args, mg_args) None sp ck (gen_poly_dispatcher ww handler_ctx m0 n idl that (zipWithIndex usersubs))
                                            


                            | other -> cleanexit("method call of " +  ids + " not implemented or no suitable overload (L3540) Arg types are " + (sfold cil_typeToStr signat'))
                    let _ = WN ("instruction call" + ids + ": done") ww
                    ans 

        | Cili_initobj _ -> 
            let (len, sp') = g1()
            // This is invoked for structs - see test30/35 uses it only on the struct. Classes have null constructors instead that may contain custom code.
            // We treat, at the moment, as a nop!
            (sp', env)                           

        // Prefix instructions: - they should not appear in this form here. They should be tucked into the CIL_instruct that they affect?
        | Cili_prefix pfix ->
            let _ = dev_println (sprintf "Poorly parsed prefix %A" pfix)
            (sp, env)  (* for now *)

        | Cili_kiwi_special("isinst", exactf, dt) -> // pop cond, pop arg: if cond fails replace with null.
            let (ptr, cond, sp') = g2()
            let ans = 
                match realdo with
                    | Rdo_tychk _ -> ptr
                    | Rdo_gtrace(bodylst, taillst) -> 
                        CE_ternary(g_ctl_object_ref, cond, ptr, g_ce_null)
            (ans::sp', env)

        | Cili_kiwi_trystart(token, catchers, (fno, fcode)) ->
            let _ = sf "Cili_kiwi_trystart should be handled by parent e1f routine, not here."
            (sp, env)

        | other -> sf ("KiwiC: PE/CIL instruction not supported: " + dis)


    let (sp, env) =
        let rec cil_eint_macros followon (sp, env) handler_ctx ins = 
            let (sp, env, followon) =
                match ins with
#if SPARE
                    | Cili_isinst _ ->
                        let (sp, env, new_instructions) = expand_cili_macro (sp, env) ins
                        (sp, env, new_instructions @ followon)
#endif
                    | no_macro ->
                        let spl = length sp
                        let (sp', env) = cil_eint kxr handler_ctx ins // Does D change? can it be a free var of eint?
                        let delta = length sp' - spl
                        let nominal_delta = stackcounter ins
                        let _ =
                            if not(special_stack_depth_pred nominal_delta) && delta <> nominal_delta then
                                vprintln 0 (sprintf "Pre stack contents (%i items)  = " spl + sfold ceToStr sp)                
                                vprintln 0 (sprintf "Post stack contents (%i items) = " (length sp') + sfold ceToStr sp')
                                sf(dis + sprintf ": elab: Stack depth accounting error: depths: pre=%i post=%i. Deltas: actual=%i, nominal=%i" spl (length sp') delta nominal_delta)
                        (sp', env, followon)
            if nullp followon then (sp, env)
            else cil_eint_macros (tl followon) (sp, env) handler_ctx (hd followon)

        cil_eint_macros [] (sp, env) handler_ctx ins
    (sp, env, !m_final_handlers_invoked)

and elab_lst ww linepoint_stack realdo fvd (env) sp (cCP, dD) cgr linepoint5 (cCB, kxr, pc, insl, enpo, expo) =
    let pcs = i2s pc
    let (fstpass, _) = dD
    let pcoTos = function
        | None -> "None"
        | Some(pc,k) -> "Some(" + pcs + "." + i2s k + ")"

    let real_mark = (if tychk_pass_pred realdo then "typecheck" else "codegen")

    let cg msg kk = // Code generate: side-effecting: emit a line of kcode. All branches are moved to end of the basic block.
        match realdo with
            | Rdo_tychk _ -> ()
            | Rdo_gtrace(bodylst, taillst) -> // code is stored inside the realdo
                // emit me on 
                if !g_gtrace_vd>=5 then vprintln 5 (msg + " kcode emit (y) " + kToStr true kk)
                match kk with
                    | K_goto _ -> mutadd taillst kk
                    | _        -> mutadd bodylst kk                    

    let kremark ss = cg "kremark" (K_remark(kxr, ss))

    let rec e1f cCB further_blocks lst (k, sp, envs) = // Do not use foldBack so we can scan hints etc.
        match lst with
        | [] -> ((k, sp, envs), further_blocks)

        | CIL_instruct(_, _, _, Cili_kiwi_trystart(handler_spec))::tt -> // Push handler here, and check for try block exit in parent routine.
            //vprintln 3 (sprintf "Nesting into try block " + f1o3 handler_spec)
            let cCB' = { cCB with exn_handler_stack_ = (f1o3 handler_spec, handler_spec)::cCB.exn_handler_stack_ }
            kremark ("Try/Finally block: Try Start for " + f1o3 handler_spec)
            let ans = e1f cCB' further_blocks tt (k+1, sp, envs)
            //vprintln 3 (sprintf "DeNesting from try block " + f1o3 handler_spec)
            ans
            
        | CIL_try(_, try_blk, handler_spec)::tt  -> sf "Try block should have been flattened to Cili_kiwi_trystart form"
            
        | h::tt -> 
            let hints = tt
            let callsite = ([pcs], k) // local callsite - need to prepend cl_codefix for the dropping.
            let mm = "\n>>" (* + contToStr count *) + " sc=" + realDo realdo + ":" + (pcs + "." + (i2s k)) + "::: " + cil_classitemToStr "" h "\n"
            yout fvd mm
            //let ww = WF 3 "intcil" ww m
            let _ = lprint 300 (fun()->" elab_lst, ins done: " + cil_classitemToStr "" h "\n\n")
            let (sp, envs, final_handlers_invoked) = elabf ww linepoint_stack (fun()->mm) fvd realdo envs sp (cCP, cCB, dD, cg) cgr linepoint5 (kxr, callsite, h, hints)
            let kfinal = k+1
            let add_labcontext id = id :: cCB.codefix
            let (envs, further_blocks) =
                match final_handlers_invoked with
                    | Some(handlers_invoked, final_dx_lab) ->
                        vprintln 3 (sprintf "Noting (perhaps queueing) %i interstitial handlers invoked before xfer to final_dx_lab=%s" (length handlers_invoked) final_dx_lab)
                        let rec dispatch_final_blocks (envs, further_blocks) (prior_dx_lab:string list) = function
                            | [] ->  (envs, further_blocks, prior_dx_lab)
                            // These need to be done in reverse order, each returning its entry point to the next one processed, which can jump to it at runtime.
                            | (handler_token, catchers_, (Some final_handler_pc, _))::fb_tt ->
                                let bpc = valOf_or (op_assoc final_handler_pc fstpass.labs_oi) -1
                                // Where there is more than one handler on the way out, the destination label (dx_lab) for an inner one is the entry point of the next outer one. 
                                let fhkey = (handler_token, prior_dx_lab) // Use the pair of ultimate dx and handler token name as a key to ensure we emit the code for that pair only once. Ditto catch blocks.
                                let fhkey_sq = fst fhkey + "_" + hptos(snd fhkey)
                                let handler_entry_point = [ "handler_entry_point"; fhkey_sq ] 
                                let _ = vprintln 3 (sprintf "Queuing intersitial handler %s from %s to %s" handler_token (hptos handler_entry_point) (hptos prior_dx_lab))
                                let cCB' = { cCB with codefix = fhkey_sq :: cCB.codefix } // Make new lab context for expanded handler
                                let ncc =
                                    if bpc < 0 then
                                        let annotation = sprintf "Try/finally block handler: longjump insert final block for %s at offset=%i bpc=%i -no-handler-" handler_token final_handler_pc bpc
                                        let _ = vprintln 3 annotation
                                        let _ = cg "" (K_remark(kxr, annotation + " NOT PROVIDED"))
                                        (envs, further_blocks)
                                    else
                                        match op_assoc bpc fstpass.control_flow_sets with
                                            | None -> sf(sprintf "missing control flow set for %s for bpc=%i" fhkey_sq bpc)
                                            | Some handler_set -> // Final handler is not always just one basic block, it is one control flow set instead.
                                                let nb = (fhkey, handler_entry_point, cCB', handler_set)
                                                (envs, nb::further_blocks)
                                dispatch_final_blocks ncc handler_entry_point fb_tt
                            | _ -> sf "dispatch_final_blocks L4548"
                        let (envs, further_blocks, first_dx_lab) = dispatch_final_blocks (envs, further_blocks) (add_labcontext final_dx_lab) (rev handlers_invoked)
                        let _ = cg "first_dx_lab" (K_goto(kxr, None, first_dx_lab, ref -1))
                        (envs, further_blocks)
                        
                    | None -> (envs, further_blocks)
            e1f cCB further_blocks tt (kfinal, sp, envs)

    let _ = if not_nonep enpo then cg "handler enpo" (K_lab(kxr, valOf enpo))
    let ((k_, sp, env), further_blocks) = e1f cCB [] insl (0, sp, env)
    let _ = if not_nonep expo then cg "handler expo" (K_goto(kxr, None, valOf expo, ref -1))
    let _ = lprint 30 (fun()->" elab_lst, e1f done\n")
    ((sp, env), further_blocks)

// Elaborate one basic block of CIL code.  This is called both from typecheck (firstpass) and from cil_gtrace.
and elab_block ww linepoint_stack (output_passage, tycheck_realdo) fvd (env) handlers_rendered (cCP, cCB, dD) cgr linepoint5 (kxr, block_pc)  = 
    let (fFP, MBO_) = dD

    
    
    let elab_block_serf (cCB, kxr, block_pc, enpo, expo) = 
        let sp = [] // Always empty on entry to one of our basic blocks owing to spill processing.
        // TODO should perhaps augment kxr at this point for better error reporting.
        let ids = hptos fFP.idl
        let (realdo, emit_flush) =
            match output_passage with
                | Some passage ->
                    let (bodylst, taillst) = (ref [], ref []) // Move branches and jumps to the end of the basic block by having two mutable lists for cg to operate on.                        
                    let emit_flusher () =
                        let cmds = (rev !bodylst) @ (rev !taillst)
                        //let _ = dev_println (sprintf "Gtrace gt0 code output channel flush performed for basic block bpc=%i l0=%i l1=%i " block_pc (length !bodylst) (length !taillst))
                        app (fun x -> mutadd passage x) cmds
                    (Rdo_gtrace(bodylst, taillst), emit_flusher)
                | None ->
                    (tycheck_realdo, fun _ -> ())   

        let (handler_ctxo, insl) = fFP.ia.[block_pc] // Fetch instruction list for this basic block.
        let msg = ids +  "--" + realDo realdo + "--------------------- pc=" + i2s block_pc
        youtln fvd msg
        let _ = WF 3 "elab_block" ww msg
        let _ = cassert(tychk_pass_pred realdo || nullp sp, "Stack not empty on entry to basic block: id =" + ids + " pc=" + i2s block_pc)

        let (ans, further_blocks) = elab_lst ww linepoint_stack realdo fvd (env) sp (cCP, dD) cgr linepoint5 (cCB, kxr, block_pc, insl, enpo, expo)


        let listing_gt0__ () =  // If reinstating, please use kcode_gt_print cos dic len lpl "gt0"
            //let _ = vprintln vd "-gt0-------------------------------------------------"
            //let _ = app (fun x -> vprintln vd ("   gt0:" + kToStr lpl x)) cmds
            //let _ = vprintln vd "-gt0-------------------------------------------end---"            
            ()
        yout fvd (ids + "--" + realDo realdo + "--------------------- pc=" + i2s block_pc + " BLOCK END\n");

        emit_flush()
        (ans, further_blocks)

    let _: OptionStore<string * string list, string> = handlers_rendered 
    let phase_string = if nonep output_passage then "typecheck" else "gtrace"
    let rec elab_block_iterate anso = function
        | [] -> valOf anso
        | hh::tt ->
            let _ = vprintln 3 (sprintf "De-queuing for %s bpc=%i" phase_string (f3o5 hh))
            let (ans, further_block_sets) = elab_block_serf hh
            let anso = if nonep anso then Some ans else anso // Keep only the first answer from i3
            let (further_basic_blocks) =
                let preprocess_final_handlerset ((handler_token, dx_lab), entry_point, nested_CB, block_nos) (cc) =
                    let hkey = (handler_token, dx_lab)
                    let _ = vprintln 3 (sprintf "Potentially queuing consequential for %s: handler block set %s, dx_lab=%s, blocks={ %s }" phase_string (handler_token) (hptos dx_lab) (sfold i2s block_nos))
                    if not_nonep(handlers_rendered.lookup hkey) then
                        let _ = vprintln 3 (sprintf "Skip re-queue of done-already block set %A" hkey)
                        (cc)
                    else
                        let _ = handlers_rendered.add hkey "done"
                        let genx bpc =
                            let enpo = if bpc = hd block_nos then Some entry_point else None
                            let expo = if bpc = hd (rev block_nos) then Some dx_lab else None
                            (nested_CB, kxr, bpc, enpo, expo)
                        (map genx block_nos @ cc)

                List.foldBack preprocess_final_handlerset further_block_sets ([])
            elab_block_iterate anso (tt @ further_basic_blocks)
    let ans = elab_block_iterate None [(cCB, kxr, block_pc, None, None)]
    ans

(*
 * elabf takes a CIL instruction or directive and returns a tuple:  1=new dotnet runtime stack, 2=(annotation_map_r, budget steps remaining), 3=(handlers_invoked:int list)
 *)
and elabf ww linepoint_stack m0 yd realdo (env:annotation_map_t, xsteps) sp qT (tcl, dclo) linepoint5 (kxr, callsite, i, hint) =
    let cCC = valOf !g_CCobject
    let (cCP, cCB, dD, cg) = qT
    let cCBo = Some cCB
    let (fP, MBO) = dD
    let vd = if yout_active yd then 3 else -1
    let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
    let _ = if xsteps <= 0 then cleanexit(kxrToStr (KXLP5 linepoint5) + sprintf ": elaborating '%s': ran out of CIL unwind budget - this may be a temporary restriction while constant meet alg is no longer doing recursive control flow (TODO). This may be set from commandline with -cil-uwind-budget=N" (m0()))
    let envs = (env, xsteps-1)
    let gb0 = tcl.cm_generics // We take these out of here and pass them on in parallel for now.
    let tcn_tycheck = ({ grounded=false; }, gb0)
    match i with
    | CIL_maxstack _ 
    | CIL_locals([], _)
    | CIL_entrypoint _ -> (sp, envs, None)

    | CIL_instruct(_, handler_ctx, _, ins) ->         
        let boring = function
           | Cili_lab _     -> true
           | Cili_comment _ -> true
           | other -> false
        let (sp, envs, final_handlers_invoked) = elab_executable_ins ww linepoint_stack yd realdo envs qT (tcl, dclo) linepoint5 hint sp handler_ctx (kxr, callsite, ins)
        if vd>=0 then (if boring ins then () elif nullp sp then yout yd "   (STK: no entries)\n" else stackdump yd sp)
        (sp, envs, final_handlers_invoked)


    | CIL_locals(locals_list, _) ->
        let m_loc_ctr = ref 0
        let ww = WF 3 "elabf: CIL_locals clause" ww  (sprintf " %i local vars to define" (length locals_list))
        let fix =
            if colouringf then cCB.simple_localfix  // no TODO
            else cCB.simple_localfix 
        let rec decl_local decls = function // The output of this 'function'  is via side effect: an entry in the localnames array.
            | P3(no, ctype, ident) ->
                if no <> None then m_loc_ctr := valOf no
                let ats = [(g_purely_symbolic_under_reference, "true")] 
                let wopt = None // Attributes cannot be applied to locals in C# so we do not have a custom width.
                let pos = !m_loc_ctr
                m_loc_ctr := !m_loc_ctr + 1
                let callstring = (sprintf "LV:%s:%i" (hptos (fst callsite)) pos) :: tcl.cl_codefix // Textual name for static type dropping
                let idl = ident::fix
                let msg = (sprintf "Defining local %i %s fullname=%s callstring=%s ctype=%s" pos ident (hptos idl) (hptos callstring) (cil_typeToStr ctype))
                let ww = WF 3 "elabf: CIL_locals clause" ww  msg 
                let atakenf = memberp pos fP.mdt.ataken_locals
                let (cez, dt, vrn) = 
                    match realdo with
                    | Rdo_tychk dlist ->
                       let (dt, _) = cil_type_n_gb ww realdo tcn_tycheck ctype
                       log_tval ww msg dlist callstring (gec_RN_monoty ww dt) 
                       (CE_typeonly dt, dt, -1)
                    | Rdo_gtrace _ ->
                        let dt = mono_type_retrieve ww msg tcn_tycheck callstring
                        let (blobf_, cez, vrn) = gec_singleton ww !g_firstpass_vd cCBo msg "localvar" ats realdo m0 ident fix dt None (Some atakenf) None
                        (cez, dt, vrn)
                        
                Array.set (fP.localnames) pos (Some(ident, idl, cez, dt, vrn))
                yout yd (sprintf "Defined local %i atakenf=%A idl=%s dt=%s\n\n" pos atakenf (hptos idl) (cil_typeToStr ctype) )
                //let n = { purpose= "localvar"; ident=[ident]; vrn=vrnl; frameoffset=Some pos; cez=cez; dt=dt }
                cez::decls

        let _ = List.fold decl_local [] locals_list // must define these in order since some are annotated positional.
        // Pre colouring, output was side-effecting, during gtrace, via gec_uav and xgen_bnet
        (sp, envs, None)

    | CIL_custom(id, t, fr, args, vale) -> 
        if vd>=4 then vprintln 4 ("CIL_custom encountered " + id)
        let (_, idl) = fr33 ww true (*consv value *) tcl.cm_generics fr
        let idl = if hd idl = ".ctor" then tl idl else idl
        let ss = if idl=[] then "" else hd idl
        vprintln 3 ("Custom method att encountered: " + hptos idl)
        let args1 = custat_argmarshall ww vd ss (args, vale)
        reportx 3 "Custom Attribute Marshalled Args" (fun (fn, x)-> fn + ":" + xToStr x) args1

        let clock_attribute = Nap(g_ip_xact_is_Clock_tag, "true")
        let reset_attribute = Nap(g_ip_xact_is_Reset_tag, "true")        

        // The kiwic.tex documentation says we can set the reset net this way but it seems not infact! And ClockDomNeg is now replaced with compositeAttributeString TODO
        let ParseKiwiClockDom args1 =
            let get_s mm  = function
                | (_, W_string(id, _, _)) -> id
                | (_, other) -> sf(sprintf "Parse Kiwi.ClockDom: %s - expect string not %s" mm (xToStr other))

            let (clk, resets, clk_enables) = 
                match args1 with
                    | [clockName; resetName; composite; clockEnableName ] ->

                        let prams = string_to_assoc_list (sfold_colons ([get_s "compositeAttributes" composite])) // Parse the string, getting key/value pairs from a colon or semicolon-separated list.
                        let clk_invf =
                            match op_assoc "clockPolarity" prams with
                                | None  -> false
                                | Some "pos" -> false
                                | Some "neg" -> true
                                | Some other -> cleanexit ("Kiwi.ClockDom expects clockPolarity to be pos or neg, not " + other)
                        let clock_id = get_s "clockName" clockName
                        let clocknet = ionet_w(clock_id, 1, INPUT, Unsigned, [clock_attribute])
                        let clk = if clk_invf then E_neg(clocknet) else E_pos(clocknet)   

                        let reset_id = get_s "resetName" resetName
                        let resets =
                            if reset_id = "" then (!cCB.director).resets
                            else
                                let reset = ionet_w(reset_id, 1, INPUT, Unsigned, [reset_attribute])
                                [reset]

                        let clockEnable_id = get_s "clockEnableName" clockEnableName
                        let clk_enables =
                            if clockEnable_id = "" then (!cCB.director).clk_enables // TODO add the option to augment instead of replace this list
                            else
                                let clk_enables = ionet_w(clockEnable_id, 1, INPUT, Unsigned, [])
                                [clk_enables]

                        (clk, resets, clk_enables)
                    | other -> sf(sprintf "Kiwi.ClockDom attribute needs four args, not %i" (length args))
            let _ = cCB.director := { !cCB.director with clocks=[clk]; resets=resets; clk_enables=clk_enables } // Note, This does not mutate all future ones owing to a new ref for each thread.
            ()
        let _ = if ss="ClockDom" && args1 <> [] then ParseKiwiClockDom args1

        //let _ = if ss="AssertCTL" then muddy("assert")
        (sp, envs, None)

    | CIL_param(n, eo) -> 
        let _ = vprintln 3("elab param: " + cil_classitemToStr "" (CIL_param(n, eo)) "")
        (sp, envs, None)

    | CIL_comment ss ->
        let _ = vprintln 3 (sprintf "CIL_comment(%s)" ss)
        (sp, envs, None)
        
    | other -> sf("elab realDo=" + realDo realdo + " other/notsupported form: " + cil_classitemToStr "" other "")



//
// We need a first pass for each method body instance (ie callstring).  Since cgr are unique/concrete at such a site, the first pass embodies the cgr as well.
// Some work is perhaps repeated for each callstring...
//    
//
//    First pass follows program textual flow, starting at entry point and initial      
//    conditions identified by pass 0. It covers only the reachable pc states
//    and does 'typeonly' evaluation of expressions.  It discovers the stack depth
//    at entry and exit to each basic block and so makes entries in the spillin array.

//    It also tracks the zpc flag, determining where the program counter is tainted by
//    a runtime value. Now not used...?
//    When it arrives at a site with tainted pc that was not before then it re-evals that
//    point onwards, which promulgates the taintedness.
//    It uses a coverage array that contains pairs: (stack depth * pc is tainted bool).

//
//   First pass also marks each branch as a fwd, back or cross edge - no longer needed.
//
//
//  Arg bindings for tyvars and method formal types are provided. Droppings in these terms are then logged.
//
and make_a_first_pass ww (cCP, (tcl:cil_cl_textual_t), cCB:cilbind_t) fvd (idl, mdt, instructions0, flogs, argtypes) cdt =
    let ubudget = 10012 // first pass of a method body does not follow control flow and has linear cost in terms of instruction length. Any reasonable value here should be fine.
    let ff ss = youtln fvd ss
    let xids = hptos idl
    let mm = "First pass of " + xids
    ff mm
    ff (sprintf "Number of template bindings=%i"  (length (Map.toList tcl.cm_generics)))
    let _ = WF 3 mm ww "Commence"
    let limit = g_spillidx
    let linepoint5 = { waypoint=ref None; callstring__= ref [ mm ]; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=[]; }
    let droppings_list = ref []
    let realdo = Rdo_tychk droppings_list
    let cCC = valOf !g_CCobject
    let env000 = (Map.empty, (valOf !g_kfec1_obj).cil_unwind_limit) // unwind limit will not be used on first pass.

    let tcl =
        let generic_bind (mm:annotation_map_t)= function
            | (ff, aa) ->
                vprintln 3 ("class generic bind (may get done again) " + ff + " as " + typeinfoToStr aa);
                mm.Add ([ff], gec_RN_monoty ww aa)
            //These have two forms so can be retrieved by name or number. Dont need the numbers explicit any more...
        if tychk_pass_pred realdo then
            let cgr = List.fold generic_bind tcl.cm_generics (cdt.whiteformals)
            { tcl with cm_generics=cgr }
        else tcl
    let cil_add_lab = function // a nop now
        | (other) -> other
    let instructions = map cil_add_lab instructions0
// mdt.is_ctor
    //ff(sprintf "constructor_name_pred=%A" is_ctor)
    let spillnext = ref g_spillidx // can use vrn now instead of spillidx
    let spill_nf() =
        let ans = (i2s(!spillnext) :: "_SPILL" :: (cCB.simple_localfix), !spillnext)
        let _ = mutinc spillnext 1
        ans

    let spillage = Array.create (g_spillidx) SP_none
    
// We need to reuse spill names on a per instance basis.
// Each spill var is given a number and can be accessed in the spillage array by that number-
// As spill vars hold stack items, they are all valuetypes or object handles (hence no dimensions).
    let (*a1*)vd = (if bopbass then 20 else !g_firstpass_vd)
    let mk_spill_var1 ww nf idtag =
        //    let ff s = yout(d_D.vd, s + "\n")
        let ff ss = vprintln (*a1*)vd ss
        let (idl, n) = nf()
        let idl = if idtag = "" then idl else idtag :: idl
        let _ = Array.set spillage (n-g_spillidx) (SP_zygote idl)
        n
            
    let bmo =
        if not mdt.has_macro_regs then None // If the method uses certain macros we define, currently, all the vars they might need.
        else
            let key = funique "BM"                
            let _ = vprintln 3 ("Creating macro expansion register set for " + key)
            let gen_macro_reg idtag =
                let (nn) = mk_spill_var1 ww spill_nf idtag
                let _ = vprintln 3 (sprintf "     macro reg %i   %s" nn (hptos []))
                (nn)
            Some
                   { 
                       ikey=                 key
                       dptr=                 gen_macro_reg "dptr"
                       count=                gen_macro_reg "count"
                       sptr_or_vale=         gen_macro_reg "sptr_or_vale"
                    }

    let (ia, il) = code_setup ww cCB mdt bmo xids instructions
    //vprintln 3 ("this=" + (if tcl.this =None then "None" else ceToStr(valOf(tcl.this))))
    let il1 = il+1 // one extra for ... please say why
    let xedge_count = Array.create (il1) 0 (* not not used ?*)
    let edge_style = Array.create (il1) []
    
    let fresh_env0(m) = ([], Sparer)

// Spill and local variables are nominally allocated on the stack and so dead when a method returns.
// Spill vars can be allocated completely virtually and later colour mapped to physical registers,
// or else we can partially map them here.  Things to consider are that a given method can be called
// by more than one thread and recursive calls are possible.
(*
   Recursion was previously unwound as we inlined method calls using intcil.  Stack net names were allocated per method by counting the number of live calls to the current method on the stack of the current thread and including the thread name in the register name.

    Now that recursion is unwound by emitting fresh kcode for each call, the call depth must be passed in
    by the kcode generator ...

*)

    let spillin = Array.create  (il+1) []
    let spillout = Array.create (il+1) None
    let coverage = Array.create (il+1) None
    let (labs_is, labs_oi, labs_si, exn_handler_directory) = scan_sis mm ia il1    
    // Already partitioned into basic blocks, so pc values here are basic block numbers from the ia. dd is stack depth.
    // We generate a pair, which is block pc and the sort of edge from the old pc (hd of cstack) to me. Not used now?
    // There is an entry point at the origin of the method and also for each finally and catch block. For each entry point we give the block pcs reachable from it. These should be disjoint and only joined up by try/catch/finally control flow which we model later.
    let method_controlflow_bcp_sets ww mm (coverage:(int * bool) option array) =
    
        let rec adv1_caller ww furtherwork ep =
            let dd = 0 // Always start with empty stack ? Not true for catchers perhaps?
            let (_, reachables, furtherwork) = adv1_serf [] ww [] furtherwork (ep, dd)
            ((ep, reachables), furtherwork)

        and adv1_serf reachables ww cstack furtherwork (pc, sd_in) =
            if pc >= il then ((pc, E_exit), reachables, furtherwork)
            else
                let opc = if cstack=[] then "EP" else sfold i2s cstack
                let (handler_ctx_lst, insl) = ia.[pc]
                let sd = (List.fold (stackcounter1c vd) sd_in insl)
                let delta = sd-sd_in 
                let ovo = coverage.[pc]
                let disf() = " bpc=" + i2s pc + ":::" + sprintf " sd=%i delta=%i " sd delta + " " + cil_classitemToStr "" (hd insl) ""
                let ww = WN ("advance1 " + i2s pc) ww
                let _ = if not(special_stack_depth_pred sd) && sd < 0 then sf (sprintf "  " + disf() + sprintf " --- Negative stack depth is not possible. sd=%i delta=%i sd'=%i" sd delta (sd+delta))
                if vd>=4 then vprintln 4 (sprintf "bpc=%i sd_in=%i adv1 progress " pc sd_in + disf())
                let myedge =
                    if special_stack_depth_pred sd then
                        E_exit
                    elif memberp pc cstack then 
                          (       
                             if vd<=10 then ff(xids + sprintf " bpc=%i opc=%s etype=%s " pc opc  " BACK EDGE to " + i2s pc)
                             E_back
                          )
                    elif not_nonep ovo then
                          (       
                              Array_add(xedge_count, pc, 1);
                              if vd<=10 then ff(xids + sprintf " bpc=%i opc=%s etype=%s " pc opc  " CROSS EDGE to " + i2s pc + " count=" + i2s(xedge_count.[pc]))
                              E_cross
                          )
                    else
                          (       
                              if vd<=10 then ff(xids + sprintf " bpc=%i opc=%s etype=%s " pc opc  " FWD EDGE to " + i2s pc)
                              E_fwd
                          )
                let unused_edge_info_ = (pc, myedge)
                let reachables = singly_add pc reachables
                match ovo with
                    | Some ov ->
                        if fst ov = sd then (unused_edge_info_, reachables, furtherwork) else sf(mm + sprintf ": adv1 More than one stack depth (again) at bpc=%i old=%i new=%i:  " pc (fst ov) sd +  disf() + "\n\n")
                    | None ->
                        let _ = WF (*a1*)vd "advance1a" ww (sprintf "continue with sd=%i at " sd + disf())
                        let _ = Array.set coverage pc (Some(sd, (not_nonep ovo && snd(valOf ovo))))
                        let spillregs = 
                            if sd_in <= 0 || not_nonep ovo then
                                vprintln (*a1*)vd (sprintf "bpc=%i: There are no entry spill regs to make." pc)
                                []
                            else
                                vprintln (*a1*)vd (sprintf "bpc=%i: Making %i entry spill regs." pc sd_in)
                                map (fun _ -> mk_spill_var1 ww spill_nf "") [0 .. sd_in-1]
                        Array.set spillin pc spillregs
                        // We need to separate out handlers to furtherwork - but not here based on the leave flag which is irrelevant.
                        let dests_a = list_flatten(map f1o3 (List.foldBack (labscan2 (fun x->x)) insl []))
                        let xfer_assoc =
                            let emsg = "adv1: xfer assoc: missing label " + xids                    
                            (fun lab -> a_assoc emsg labs_si lab)
                        let xfer_assoc_offset =
                            let emsg = "adv1: xfer_assoc_offset: missing label " + xids
                            (fun offset -> a_assoc emsg labs_oi offset)
                        let dests = map xfer_assoc dests_a
                        let stopp = unconditional_jmp2 insl // No transfer to successor for unconditional jumps.
                        let sd' = if special_stack_depth_pred sd then 0 else sd
                        let successor = if stopp then [] else [(pc+1, sd')]
                        let normal_dests = successor @ map (fun pc ->(pc, sd')) dests
                        if vd>=4 then ff("Advance1 bpc=" + i2s pc + " dests=" + sfold (fun (pc, sd) -> sprintf "pc=%i/sd=%i" pc sd) normal_dests + ".   stopp=" + boolToStr stopp + "\n")
                        app (fun (dpc, _)->Array.set spillout pc (Some(singly_add dpc (valOf_or_nil(spillout.[pc]))))) normal_dests

                        let handler_dests =
                            let ip2s (pc, sd) = sprintf "%i/sd=%i" pc sd // PC and stack depth pair.
                            let refone cc (mysac, catchers, (foo, _)) =
                                // We just need reachable scan for typecheck of all catcher and finally block code.
                                // The catch code for a never raised exception can be left out but we do not check for that here.
                                let pc_lst = []
                                // A catcher should always be elaborated with one typeonly arg on the stack.
                                // Comment out for now
                                //let pc_lst = List.fold (fun cd (dt,  offset_o, code_) -> if nonep offset_o then cd else (valOf offset_o, 1) :: cd) pc_lst catchers
                                let pc_lst = if nonep foo || valOf foo < 0 then pc_lst else (valOf foo, 0) :: pc_lst
                                let dests = list_once(map (fun (pc, sd) -> (xfer_assoc_offset pc, sd)) pc_lst)
                                //ff (sprintf "xfer out of try block  %s: src=%i handler_dests=%s via_db=%s" mysac pc (sfold ip2s pc_lst) (sfold ip2s dests))
                                dests @ cc 
                            List.fold refone [] handler_ctx_lst
                        let _ = vprintln 3 (sprintf "adv1 xfers: bpc=%i. To normal_dests=%s. To handler_dests=%s." pc (sfold (fst>>i2s) normal_dests) (sfold (fst>>i2s) handler_dests))
                        //ff (sprintf "bpc=%i: recursive scan start. %i normal dests" pc (length normal_dests))
                        let zz = map (adv1_serf [] ww (pc::cstack) []) (normal_dests) // Recurse
                        //ff (sprintf "bpc=%i: recursive scan return. Did %i normal dests" pc (length normal_dests))
                        let (edges, reachables2, furtherwork2) = List.unzip3 zz
                        // Conservative List once needs to keep the last copy - first added LIFO style - for program order.
                        let reachables = list_once(list_flatten reachables2 @ reachables)
                        //if vd<=10 then ff("Log edge record " + i2s pc + " with " + sfold (fun (a,b)->i2s a) edges)
                        let _ = Array.set edge_style pc edges // Only used by dominates which we do not currently use.
                        (unused_edge_info_, reachables, lst_union (map fst handler_dests) (furtherwork @ list_flatten furtherwork2))

        let initialwork = [ (0) ]
        let rec iterate done_already cc = function
            | [] -> cc
            | hh::tt when memberp (hh) done_already -> iterate done_already cc tt
            | hh::tt ->                    
                let (ans, furtherwork) = adv1_caller ww tt hh
                iterate (hh::done_already) (ans::cc) furtherwork
        iterate [] [] initialwork

    let control_flow_sets = method_controlflow_bcp_sets ww mm coverage
    let control_flow_sets = map (fun (ep, r) -> (ep, List.sort r)) control_flow_sets // Have ascending to make a little sense but also so CIL_locals, that will be in basic block 0, is done first.


    let typecheck_scan_advance1 ww mm fFP = 
        //let (ep, reachables) = adv1 fFP coverage [] (WN ("adv1_old_callsite " + m) ww) [] (0, 0)
        let _ =
            let qf (ep, reachables) = sprintf " ep=%i  bpcs=%s" ep (sfold i2s reachables)
            reportx 3 "reachable basic block pcvalues" qf control_flow_sets
        let taintedpc a = if a >= fFP.il || coverage.[a]=None then false else snd(valOf(coverage.[a]))// Last of a run.
        let kxr = KXREF("typecheck", idl)
        let splats = list_flatten(rev(map snd control_flow_sets))
        app (insert_spillage fFP) splats
        if cCC.settings.late_cil_dumpf then gen_post_spill_cil_listing ww cCC fFP
        let handlers_rendered = new OptionStore<string * string list, string>("handler_done-typecheck")
        let _ = 
            let tyadv1b pc =
                let dclo = None // No dynamic values when type checking.
                let _ = elab_block (WN "advance1 elab_block" ww) ["typecheck_linepoint_stack"] (None, realdo) fvd (env000) handlers_rendered (cCP, cCB, (fFP, None)) (tcl, dclo) linepoint5 (kxr, pc)
                ()
            app tyadv1b splats
        let rec rt_last s = if taintedpc s then rt_last(s+1) else s-1
        let rec rtt s = 
            let e = rt_last s
            rt(e+1)
        and rt s = if taintedpc s then rtt s elif s < fFP.il then rt (s+1)
        let _ = rt 0
        ()


    let stackin = Array.create (il1) None
    let _ = redirect_ia(0, ia, labs_si, labs_is, il1)
    reporty fvd  "Method lable table I->S" (fun(i,s) -> i2s(i) + " " + s) labs_is
    reporty fvd  "Method lable table S->I" (fun(s,i) -> i2s(i) + " " + s) labs_si


    let localnames = Array.create limit None
    let fFP: firstpass_t =
            {
                fvd=           fvd
                instcount=     length(instructions0)
                edge_style=    edge_style
                xedge_count=   xedge_count
                argtypes=      argtypes
                localnames=    localnames
                ia=ia
                il=il; labs_is=labs_is; labs_si=labs_si; labs_oi=labs_oi
                idl=           idl
                spillage=      spillage
                spillin=       spillin
                spillout=      spillout
                droppings=     []
                reachables=    []
                mdt=           mdt
                exn_handler_directory=exn_handler_directory
                control_flow_sets=control_flow_sets
            } 
    let _ = WF 3 mm ww "Firstpass spills done. Typecheck start."
    vprintln 3 (sprintf "First pass structure made for %s of length %i with %i CIL instructions" xids il fFP.instcount)
    typecheck_scan_advance1 ww mm fFP
    let _ = WF 3 mm ww "Firstpass and typecheck done."
    
    let fFP = { fFP with droppings=  !droppings_list; }
    let concourse_reachable =
        let dominate_reachscan start = 
            let successors(n) = if n<0 then [] else map (fst) (edge_style.[n])
            let rec k ans = function
                | [] -> ans
                | h::t -> if memberp h ans then k ans t
                          else k (h::ans) (successors(h) @ t)
            k [] [start] // We find which nodes are dominated by x by computing reachability without x and complementing the set
        rev (dominate_reachscan 0) // starting with 0 makes for readibility!

    let fFP = { fFP with reachables=concourse_reachable; }

    let inverted_map = // generate predecessor matrix from successor matrix
        let add c n = 
            let successors = map (fst) (edge_style.[n])
            let add1 (c:Map<int, int list>) v =
                let ov = c.TryFind v
                in c.Add(v, singly_add n (valOf_or_nil ov))
            List.fold add1 c successors
        List.fold add Map.empty concourse_reachable

    //let ff s = vprintln 0 s
    let (forward_map, exit_states) = // simple recast of the array
        let add (c:Map<int, int list>, es) n = 
            let successors = map (fst) (edge_style.[n])
            let _ = ff ("Control graph arc " + i2s n + " to " + sfold i2s successors)
            (c.Add(n, successors), (if successors=[] then n::es else es))
        List.fold add (Map.empty, []) concourse_reachable

    let _ = vprintln 2 (sprintf " Concourse reachable is " + sfold i2s concourse_reachable)
    let _ = vprintln 2 (sprintf " Exit states are " + sfold i2s exit_states)

#if DOMINATES_ON
    // Classical algorithm is: put all nodes in all sets and then iterate until closure the dominators of a node are self and the intersection of the dominators of the predecessors.
    let dodom msg mp exitnode = 
        let successors(n) = valOf_or_nil((mp:Map<int, int list>).TryFind n)
        let dominate_reachscan start leaveout = // Not the classical algorithm!
            let rec k ans = function
                    | [] -> ans
                    | (h::t) -> if h=leaveout || memberp h ans then k ans t
                                else k (h::ans) (successors(h) @ t)
            k [] [start] // We find which nodes are dominated by x by computing reachability without x and complementing the set
        
        let dominated n =
            let ans = list_subtract(concourse_reachable, dominate_reachscan (valOf_or exitnode 0) n) 
            let _ = ff("Node " + i2s n + " " + msg + ":" + sfold i2s ans)
            ans


        let predoms = map dominated concourse_reachable
        predoms


    let _ = dodom "dominates" forward_map None
    let _ = if (length exit_states = 1)
            then dodom "post-dominates" inverted_map (Some(hd exit_states))
            else []
#endif
    let _ = WF 3 mm ww "End of first pass"
    fFP

//
//   
and cil_gtrace ww kxr (Rdo_gtrace(parent_bodylst, xx)) linepoint_stack (cCP, dcl, cCB, dD) M (arg_bindings, cre, flogs, id, stack00) =
    // cil_gtrace: return a kcode generator for each basic block that emits the instructions for that block in textual order making a kcode trace at a supplied stack depth.
    // The resulting kcode contains conditional and unconditional gotos.
    // The insertion of finally blocks where a try block is exited, by whatever means, is done here.
    // Note that the returned generator function can be recursive when the user's program is recursive.
    let vd = !g_gtrace_vd
    let mm = "gtrace pass of method " + id
    let ww' = WF vd mm ww "commence"
    let (fP, _) = dD
    let logger = YOVD vd
    let gbm_ = Map.empty // Not needed post dropping rewrite. delete me.
    let envs = (gbm_, snd cre) // env part not used? - has freespace in it.
    let linepoint5 = { waypoint=ref None; callstring__= ref [ mm ]; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=linepoint_stack; }
    let handlers_rendered = new OptionStore<string * string list, string>("handler_done-gtrace")
    let gtrace_basic_block stack09 block_pc =
        if not(memberp block_pc fP.reachables) then
            vprintln 3 (sprintf "bpc=%i unreachable" block_pc)
            snd cre // If unreachable, ignore.
        else
            let _ = vprintln vd ("gt " + i2s block_pc + "  " )
            let (s', (env_, steps')) = elab_block ww' linepoint_stack (Some parent_bodylst, Rdo_gtrace(parent_bodylst, xx)) logger (envs) handlers_rendered (cCP, cCB, dD) (dcl.text, Some dcl) linepoint5 (kxr, block_pc)
            // Want to know if pc is live on exit (control flow out of block).
            let _ = if block_pc=fP.il-1 && not_nullp s' then mutadd stack00 (hd s') // Final return value.
            steps'


    let rendering_fns__ = map (fun block_pc sdepth -> (block_pc, fun () -> gtrace_basic_block stack00 block_pc)) [0 .. fP.il - 1]
    // Currently we seem to run the function instead of returning the rendering_fns_ thunks.  This will not work for recursion unwind anymore? Please add a test to the regression suite.
    let ans = map (gtrace_basic_block stack00)  [0 .. fP.il - 1]  // Need to just do those in reachables.  Control flow is ignored here but K_goto's are generated and control flow interpretation is left to the kcode vm.


    let lowest = 
        let rec lowate  = function
            | [] -> snd cre
            | [x] -> x
            | h::t -> min (h) (lowate t)
        lowate ans
    let ww' = WF vd mm ww "finished making thunks"

    let _ = vprintln 3 (id + sprintf ": rsteps at end %i" lowest)
    let _ = if lowest < 0 then cleanexit(kxrToStr (KXLP5 linepoint5) + sprintf ": elaborating '%s': ran out of initial elaboration steps - this is temporary while constant meet alg is no longer doing recursive control flow (TODO)" id)
    lowest

// Recursive walk of a method - how is bounded recursion handled if we are doing insufficient
// symbolic evaluation at this stage?   We return a function for each basic block that is applied to a representation of the callstack.
and cil_walk_method parent_kxr mgr topargs realdo ww linepoint_stack (cCP, dcl:cil_cl_dynamic_t, cCB:cilbind_t, stack, cre) mM actuals flogs = 
    //let _:((int * (string list * ciltype_t * cil_eval_t) * hexp_t * cil_eval_t option) list * decl_binder_t) option = topargs
    let cCC = valOf !g_CCobject
    match (mM) with
    | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1, instructions0, atts) -> 
        let vd = !g_gtrace_vd
        let idsx = hptos idl
        let _ = WF vd ("cil_walk_method (L3619)") ww (idsx + " uid=" + uid)
        let prefix = idl @ [] (*  #prefix CN need class name *) 
        if memberp Cilflag_hprls flags // rename as Cilflag_KiwiC
        then sf(hptos prefix + ": should be trapped as builtin eariler")
        else
        let staticf = memberp Cilflag_static flags      
        let tcl = dcl.text

        let tycontrol =
            match realdo with
                | Rdo_gtrace _ -> { grounded=true; }
                | _ -> { grounded=false; }
    
        let kxr = KXR_dot(idl, parent_kxr)
        

        let msg = "Method " + idsx + " uid=" + uid + " staticf=" + boolToStr staticf + " being converted to kcode."
        vprintln vd msg
        let m0() =
            let linepoint51 = { waypoint=ref None;  callstring__ = ref []; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=linepoint_stack; } 
            "QX3705:" + kxrToStr (KXLP5 linepoint51) + " " + msg
        let argsize = (if staticf then 0 else 1) + length mdt.arg_formals
        let argspace = Array.create (argsize) None
        let argtypes = Array.create (argsize) None

        let cdt = // Defining class info.
            match mdt.meth_parent_class with
                | (idl, k) when nonep !k -> // Used for C/C++ inputs and so on.
                    vprintln 3 ("global method (without class binding) " + hptos (uid :: idl))
                    {
                        name= []
                        structf=false
                        ats=[]
                        parent=None
                        newformals=       []
                        whiteformals=     []
                        class_item_types= []
                        class_tag_list=   []
                        arity=            0
                        vtno=             None
                        m_next_vt=        ref 1
                        sealedf=          false
                        remote_marked=    None//TODO scan for this
                    } // class_struct_t 
                | (idl, k) -> valOf !k
                
        let gb0 = tcl.cm_generics 
        let _ =
            let gToStr(idl, dropping) = "  " + hptos idl + "   " + drToStr dropping
            reportx vd ("class and method generic bindings for walk method:" + idsx) gToStr (Map.toList gb0)

        
        //TODO : Mark as a final value for read only.
        let thato =
            if staticf then None
            else
               let thisget = function
                   | None -> sf ("no this pointer for instance method call to " + hptos idl) // CIL calls static method calls 'class' calls.
                   | Some k -> k
               // Insert the 'this' pointer
               let that = thisget(dcl.this)
               Array.set argtypes 0 tcl.this_dt
               Array.set argspace 0 (Some(["this"], valOf tcl.this_dt, that))
               vprintln 3 (sprintf "Set arg0 (aka 'this') of type %s to vale %s" (cil_typeToStr (valOf tcl.this_dt)) (ceToStr that))
               Some that

        let as_gen cgo (l, r) =
            match cgo with
                | Some cg ->
                    //let _ = vprintln 3 ("emit assign to replicated actual")
                    cg "as_gen" (K_as_sassign(kxr, l, r, None)) // TODO abstract to support structures
                | _ -> ()
        //The CIL VM is call-by-value always. Call by reference in C# is handled in the the C# compiler. The formal is bound without prefix in bindings
        // If the callee uses the stloc instruction we need to freshen the passed-in expression to achieve call by value where the parent is an unmodified l-value or an expression.
        //Call-by-reference is handled symbolically within Kiwife using the CE_star within ldloca: repack is not involved (e.g. with address computations) for that.
        //For C# structures, that are call-by-value, we copy out each field rather than optimising as we do for scalars, where a scalar arg is only copied out if we detect a mutated_formal owing to a starg instruction present in the body of the callee.
        let insert_either_args cgo gb01 =
            let vd = if nonep cgo then (if bopbass then 3 else !g_firstpass_vd) else !g_gtrace_vd
            let msg = if nonep cgo then "insert_args-typecheck" else "insert_args-gtrace"
            let rec insert_arg pos (cc, cd) = function
                | ([], []) -> (cc, cd)
                | ((_, _, formal_dt, ident)::tt, (actual_dt, varm, vale)::att) -> 
                      let wopt = None
                      let dct = template_eval ww gb01 actual_dt // We are using the actual type during a call, but the formal types earlier in type checking.
                      let dct = array_decay_to_pointer_override formal_dt dct
                      Array.set argtypes pos (Some dct)
                      let (atakenf, mutated) = (memberp pos mdt.ataken_formals, memberp pos mdt.mutated_formals) 
                      let replicate = atakenf || mutated || full_struct_assign_pred msg vale
                      let (cezo, rhs) =
                          if replicate then
                              let (blobf_, cez, vrnl_) = gec_singleton ww vd (Some cCB) msg "formalvar" [] realdo m0 ident prefix dct None (Some atakenf) None
                              //let ((_, cez), _) = gec_uav ww m0 STOREMODE_singleton_scalar realdo false dct (hptos (ident::prefix), []) vrn None
                              as_gen cgo (cez, vale)  // Only need vreg when a mutated formal
                              (Some cez, cez)
                          else (None, vale)
                      Array.set argspace pos (Some(ident::prefix, dct, rhs))
                      let cd =
                          if mdt.is_ctor>0 then
                              if not_nonep thato then
                                  //>H
                                  //dev_println (sprintf "ctor squirrel remote-spog %i that=%s %s" pos (ceToStr (valOf thato)) (ceToStr rhs))
                                  (valOf thato, (pos, dct, rhs))::cd // create a note of constructor arguments in the the m_remote_ctor_arg_db
                              else cd
                          else cd
                      if vd>=4 then vprintln 4 (*vd*) (msg + sprintf " Setup method formal in argspace. Argno=%i  potentially_mutated=%A  replicate=%A  atakenf=%A  f_dt=%s a_dt=%s rhs_ce=%s" pos mutated replicate atakenf (typeinfoToStr formal_dt) (typeinfoToStr dct) (ceToStr rhs) + (if mutated then sprintf " copied in %s" (ceToStr vale) else ""))
                      let n = (pos, ([ident], cezo))
                      insert_arg (pos+1) (n::cc, cd) (tt, att)

            //vprintln 3 (sprintf "debug actuals 2/2 = %s" (sfold (f3o3>>ceToStr) actuals))

            let staticatedf = not_nullp cCB.staticated
                      
            // Insert into the execution env either the local caller's args or top-level args passed into this compilation.
            let rec insert_toparg offset (cc, cd)  = function
                | [] -> (cc, cd)
                | (idx, ite, xnet_, external_net_o_)::tt when offset+idx <  0  -> insert_toparg offset (cc, cd) tt
                | (idx, ite, xnet_, external_net_o_)::tt when offset+idx >= 0 ->
                    // TODO check mutation supported for topargs.
                    let nv = Some ite
                    vprintln vd (sprintf "Insert toparg idx=%i+%i %s " offset idx (ceToStr (f3o3 ite)))
                    dev_println (sprintf "squirrel Insert toparg idx=%i+%i %s " offset idx (ceToStr (f3o3 ite)))                    
                    Array.set argspace (offset+idx) nv
                    Array.set argtypes (offset+idx) (Some(f2o3 ite))
                    insert_toparg offset ((offset+idx, (f1o3 ite, Some(f3o3 ite)))::cc, cd) tt
            let act0ToStr  (dt, vm, ce) = sprintf "%s:%s" (cil_typeToStr dt) (ceToStr ce)
            let act1_4ToStr (idx, vm, hexp, ce_) = sprintf "%i:%s" idx (xToStr hexp)
            let formal_count = length mdt.arg_formals
            let (arg_bindings, ctor_squirrel) =
                if vd>=4 then vprintln 4 (msg + sprintf ": Start insert formals. formal_count=%i  method arguments (staticf=%A staticated=%A) topargs=%A" formal_count staticf cCB.staticated (not_nonep topargs))
                match topargs with
                    | None ->
                        let al = length actuals
                        let _ =
                            if al<>formal_count then
                                vprintln 0 (sprintf "+++measured arg count mismatch actuals=" + sfold act0ToStr actuals)
                                cleanexit(m0() + sprintf ": wrong no of actual args %i provided. Expected %i" al formal_count)
                        //if argvd then vprintln 3 (sprintf "formals are " + sfold act0ToStr mutated_formals)
                        insert_arg (if staticf then 0 else 1) ([], []) (mdt.arg_formals, actuals)
                    | Some(port1, port2_) ->
                        //dev_println (sprintf "toparg actuals=" + sfoldcr_lite act1_4ToStr port1)
                        insert_toparg (if staticatedf then 1 else 0) ([], []) port1

            if vd>=3 then vprintln 3 (msg + sprintf ": Inserted %i method arguments" formal_count)
            (arg_bindings, ctor_squirrel)
            
        //Find or create the first pass digest for a method.
        let find_first_pass(cf, uid) = 
            let fp_id = uid :: cf // Create a unique name as a key to index saved first passes.
            let fp_ids = hptos fp_id
            let (found, ov) = saved_first_passes.TryGetValue fp_id
            if found then (vprintln 3 ("Retrieved previous firstpass of " + fp_ids); ov)
            else
                  let (*a1*)vd = !g_firstpass_vd 
                  let _ = insert_either_args None ({grounded=false; }, gb0)
                  let cos = if false then YO_null //
                            elif true then YOVD (*a1*)vd
                            else yout_divert_log("obj/" + mysanitize [] (hptos fp_id) + ".firstpass")
                  let ww' = WF (*a1*)vd "typecheck method body (first pass)" ww fp_ids
                  let ans = make_a_first_pass ww' (cCP, tcl, cCB) cos (fp_id, mdt, instructions0, flogs, argtypes) cdt
                  yout_close cos
                  let ww' = WF (*a1*)vd "make first pass done" ww fp_ids                  
                  if vd>=4 then vprintln 4 (sprintf "1st pass found %s modified/mutated formals are %s" (hptos fp_id) (sfold i2s ans.mdt.mutated_formals))
                  saved_first_passes.Add(fp_id, ans)
                  ans

        let dD = (find_first_pass(cCB.simple_localfix, uid), Some{argspace=argspace}) 
        let gbx = ({grounded=true; }, dcl.text.cm_generics)

        let gb0n = // rehydrate the first pass for code generation pass - cf concrate called on the droppings retrieve - both do the same //.... concrate is the final operation whereas for instantiating a class within a class we can rewrite with further type vars in places ... nonetheless both routines are the same and the could be located at either site
            let ww = WF 3 "rehydrate droppings" ww ("Start for " + idsx)
            let dropss = fst(dD).droppings
            let make_dropping (m:annotation_map_t) (x, y) =
                let rehyd_eval_ty gb ty =
                    if trivially_grounded_leaf ty then ty
                    else
                        //let ww = WF 3 "rehyd" ww "not trivially grounded"
                        // This WF can print too much:
                        //let ww = WF 0 "rehyd" ww (sprintf "L3889 rehyd type (xpoint=%s) pre_it=%s " (hptos x) (cil_typeToStr ty))
                        // We need passed in bindings, not textual bindings for this type eval.
                        let ty1 = cil_type_n ww gb ty
                        //let _ = vprintln 0 (sprintf " L3889 rehydrated dropping: post_it=%s " (typeinfoToStr ty1))
                        ty1
                    
                let rec rehyd gb = function // TODO - abstract this - it is fnorm_dropapp but with support for RN_call too.
                    | RN_call(class_dt, rt, ga, args, cgr, mgr) ->
                        //Augment gb here - again - somewhat wierd - some ! got through earlier stage wrongly?
                        //Augment bindings temporarily with method generic bindings - copy 2/2
                        //let _ = vprintln 0 (sprintf "rehyd monor idsx=%s class_dto=%A  %i %i" idsx (drToStr class_dt) (length cgr) (length mgr))
                        let class_dt =
                            let gb = 
                                let aug4 methodf (cc:annotation_map_t) = function
                                    | (RN_monoty(ty, ats), idx) ->
                                        let fm = (if methodf then "!!" else "!") + i2s idx
                                        let _ = vprintln 3 (sprintf "L4644  methodf=%A generic bind idx=%i   %s  with %s " methodf idx fm (cil_typeToStr ty))
                                        cc.Add([fm],  RN_monoty(ty, ats))
                                    | other -> sf (sprintf "aug4 other %A" other)
                                let gbm = List.fold (aug4 false) (snd gb) (zipWithIndex cgr)
                                let gbm = List.fold (aug4 true) (gbm) (zipWithIndex mgr)
                                (fst gb, gbm)
                            (rehyd gb class_dt)
                        RN_call(class_dt, rehyd gb rt, map (rehyd gb) ga, map (rehyd gb) args, map (rehyd gb) cgr, map (rehyd gb) mgr)
                    | RN_duplex(t1, t2, ptag)   -> RN_duplex(rehyd gb t1, rehyd gb t2, ptag)
                    | RN_monoty(ty, ats) when ty=g_ctl_object_ref -> RN_monoty(ty, ats)
                    | RN_monoty(ty, ats)        ->
                        match (rehyd_eval_ty gb ty) with //TODO always expect a CTL_record now ? depends on content.
                            | CT_class crt when not_nullp crt.whiteformals ->
                                let cl_actuals' = sf (sprintf "want a class ref not a class without type actuals when rehydrate type %A" ty)
                                gec_RN_monoty_ats ww (CT_cr { g_blank_crefs with name=crt.name; cl_actuals=cl_actuals'; crtno=ty; }) ats
                            | other -> gec_RN_monoty_ats ww other ats// should always be a nop 
                    | rn -> sf (sprintf "rehdyrate: d=%s : other form %A" (hptos x) (rn)) 
                //and rehyd_a (so, d) = (so, rehyd d)
                m.Add(x, rehyd gbx y)
            List.fold make_dropping tcl.cm_generics dropss // TODO save time by starting a new mapping just for type annotates and then save this separately so that it can be used for each walk of the meth/class thing with this callstring.

        let ww = WF 3 "rehydrate droppings" ww ("Done for " + idsx)

        let tcl = { tcl with cm_generics= gb0n  }
        let dcl = { dcl with text=tcl;  } // bit messy
            
        let ans =
            match realdo with
                | Rdo_gtrace(parent_bodylst, _) ->
                    let cg msg kk =
                        if !g_gtrace_vd>=4 then vprintln 4 (msg + " kcode emit (x) " + kToStr true kk)
                        mutadd parent_bodylst kk
                    let (arg_bindings, ctor_squirrels) = insert_either_args (Some cg) (tycontrol, gb0n)
                    if not_nullp ctor_squirrels then // We save this whether remote or not at the moment ... only need to save remote_marked items.
                        let class_idl = tl idl // Remove the .ctor name
                        //dev_println(sprintf "Remote-spog lst consider %A" cCB)
                        let that = fst(hd ctor_squirrels)
                        let actuals = map snd ctor_squirrels
                        vprintln 3 (sprintf "NOT: Remote-spog add to db class=%s that=%s args^=%i" (hptos class_idl) (ceToStr that) (length actuals))
                        
                        //cCC.m_remote_ctor_arg_db.add class_idl (that, actuals)
                        ()
                    let ans = cil_gtrace ww kxr realdo linepoint_stack (cCP, (dcl), cCB, dD) mM (arg_bindings, cre, flogs, uid, stack)
                    let emit_kill_locals lst =
                        let get_vrn kvl = kvl.vrn
                        //cg msg (K_kill(kxr, map get_vrn lst))
                        ()
                    emit_kill_locals !cCB.local_vregs_used // Does nothing now
                    ans
                | _ -> sf "L3494"
        // "We must store analyse before reducing to phy"
        (Map.empty, ans)
        
    | x -> sf("method walk other: " + noitemToStr false x) 



// eof
        
