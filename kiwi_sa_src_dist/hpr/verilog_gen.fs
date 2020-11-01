(* $Id: verilog_gen.fs $
 *
 * HPR L/S core component - Convert a tree of VM2 machines into a flat Verilog module, sharing ifs and so on.  The tree's must have suitable forms - e.g. use mainly SP_rtl.
 *
 *
 * All rights reserved. (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met: redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer;
 * redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution;
 * neither the name of the copyright holders nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *)


(*
 * An on-heap RTL AST created here and then written to a file by verilog_render and also converted back to hexp_t form if roundtrip is enabled.   
 * Parts of the tree (eg those with "preserveinstance") can be placed in different verilog modules in the same output file, in which case 
 * instances of them will be present in their parents.
 *
 * TODO - please discuss internal and external instantiation here.  External instantiation is when a component that could logically be an instance within the current module is instead instantiated outside the current module and the current module thereby gets additional I/O nets for connecting to the external instance.  Those nets would normally just be local to the current module.
 * 
 * Those marked as 'externally-provided' are not rendered on the basis that the current output will be combined with implementations in a later part of the toolchain.
 *
 * Those marked as 'preserveinstance-assoc' are instantiated with associative syntax.
 *
 * We do not automtically spot shared subtrees at the moment, however. ?? Really?
*)

module verilog_gen

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open verilog_hdr
open hprls_hdr
open protocols
open moscow
open meox
open abstract_hdr
open abstracte
open verilog_render
open yout
open gbuild
open opath_hdr
open opath_interface
open bevctrl
open linprog

type filler_t = Filler

let gg_unspec_width = { signed=Signed; widtho=None; }

let g_vnl_loglevel = ref 1 // Higher settings turn on more logging and debugging

let floatToBits (fv:float32) =
    let bytes = System.BitConverter.GetBytes fv
    let tobits (v:byte) cc = (cc * 256I) + BigInteger(int32 v)    
    let ival = Array.foldBack tobits bytes 0I
    //let _ = vprintln 0 (sprintf "verilog_gen floatToBits fv=%f  ival=%A   bytes=%A" fv ival bytes)
    gec_X_bnum(g_s32, ival)


let promote_to_fp ww arg = // TODO - should just use the generic casting code and make sure it handles this case
    match arg with
        | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | (XC_string ss)::_ -> muddy ("string as fp constant:" + ss)
                | XC_bnum(prec, bn, _)::_ ->
                    if ww = 32 then xi_float32((float32) bn)
                    elif ww = 64 then xi_double (double bn)
                    else sf "L90"
                | XC_float32(f32)::_ ->
                    if ww = 32 then arg
                    elif ww = 64 then xi_double (double f32)
                    else sf "L94"
                | XC_double(d)::_ ->
                    if ww = 64 then arg
                    else sf "LL97"

                | [] -> arg  // Non-constant will take multiple runtime cycles to convert and cannot be done here (except perhaps single to double if that's escaped being cast earlier in the tool chain)
                
        | arg ->
            // TODO can convert non-consts
            arg



// For example, little endian
// dv [|0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 240uy; 63uy|] bytes
//    
let doubleToBits (dv:double) =
    let bytes = System.BitConverter.GetBytes dv
    let tobits (v:byte) cc = (cc * 256I) + BigInteger(int32 v)
    let ival = Array.foldBack tobits bytes 0I
    //vprintln 0 (sprintf "verilog_gen doubleToBits fv=%f  ival=%A bytes=%A" dv ival bytes)
    gec_X_bnum(g_s64, ival)


type FV_t =
    {
        prefs:                     gbuild_prefs_t
        timescale:                 string         // typically contains "`timescale 1ns/10ps"
        vd:                        int            // Debug level
        pagewidth:                 int            // Page column width for pretty printer
        comb_delayo:               int option     // Add a delay to combinational logic: the LCP estimator overrides this (check?). - Move to gbuild_prefs please
        ddctrl:                    ddctrl_t       // RTL style/compatibility options
        add_aux_reports:           bool           // Append report files (so far) to generated .vnl file.
        //m_topused:               bool ref
        preserve_sequencer:        bool // Structure RTL as a state machine with a CASE/SWITCH statement where appropriate.
//        m_resetgrds:             hbexp_t list ref
        separate_files:            bool           // Write one module per file.
        subex_sharecost_threshold: int
        disable_subexpression_sharing:                 bool
        output_file_suffix:        string
        ip_xact_filestyle:         string
    }

type offspring_t = (vm2_iinfo_t * hpr_machine_t option) // A spawned child.

let omax arg0 = function
    | (Some a, None)
    | (None, Some a)   -> a
    | (Some a, Some b) -> if a > b then a else b
    | (lw, rw) ->
        sf (sprintf "verilog_gen: cannot render a floating point comparison between widths %A and %A in %s" lw rw (xbToStr arg0))



let pram_groom a (cf, cp) = // Get parameters (generics) from attributes. - old way of doing it?
    match a with 
        | Napnode ("parameters", pairs) ->
            let pfun arg (cf, cp) =
                match arg with
                    | Nap(fid, "") ->
                        let ov = None
                        (fid::cf, (V_PRAMDEF(V_PARAM(fid, None), ov), ref None)::cp)
                    | Nap(fid, nomval) ->
                        let ov = Some(V_NUM(-1, false, "d", xi_bnum (atoi nomval)))
                        (fid::cf, (V_PRAMDEF(V_PARAM(fid, None), ov), ref None)::cp)

            List.foldBack pfun pairs (cf, cp)
        | _ -> (cf, cp)


let rec v_isconst = function
    | V_NET(ff, _, _)     -> constantp(X_bnet ff)
    | V_NUM(_, _, _, x)  -> constantp x
    | V_MASK _ -> true

    // Further non-constant forms are not expected ...
    | V_DIADIC(prec, oo, l, r) -> v_isconst l && v_isconst r // Not expected
    | V_LOGNOT v -> v_isconst v
    | _ -> false

let isnum = function
    | V_NUM _ -> true
    | _ -> false

let getnum = function
    | V_NUM(w, signed, baser, c) -> (w, signed, baser, c)
    | _ -> sf "getnum"

let gen_V_IF = function
    | (V_NUM(_, _, _, n), t) -> if xi_monkey n <> Some false then t else V_BLOCK []
    | (g, V_BLOCK []) -> V_BLOCK []
    | (g, t) -> V_IF(g, t)


let rec gen_V_IFE = function
    | (V_NUM(_, _, _, n), t, f) -> if xi_monkey n <> Some false then t else f
    | (g, t, V_BLOCK []) -> gen_V_IF(g, t)
    | (g, V_BLOCK [], f) -> gen_V_IF(V_LOGNOT g, f)
    | (g, t, f) -> if t=f then t else V_IFE(g, t, f)


// Record that a net is assigned with always_comb (fully-supported always block)
let note_is_combreg regslist site lhs =
    let ids = net_id lhs
    let nv = 
        match (regslist:regslist_t).lookup ids with
            | None ->       { is_reg=true; is_combreg=true }
            | Some entry -> { entry with is_reg=true; is_combreg=true } // that's all of it anyway for now.
    regslist.add ids nv
    ()

                
// Apply a signed or unsigned cast, $signed/$unsigned, discarding any immediately-enclosed identical or opposite cast.
// Also, do not apply to a net that already clearly has a sign.
let gen_verilog_signed_cast arg =
    match arg with
    | V_NET(ff, _, -1) when ff.signed = Signed -> arg
    | V_CALL(cf, ((fn, _), _), [ax]) when (fn=fst g_verilog_unsigned_cast || fn=fst g_verilog_signed_cast) -> V_CALL(cf, (g_verilog_signed_cast, None), [ax])
    | ax -> V_CALL(g_null_callers_flags, (g_verilog_signed_cast, None), [ax])    

let gen_verilog_unsigned_cast arg =
    match arg with
    | V_NET(ff, _, -1) when ff.signed = Unsigned -> arg        
    | V_CALL(cf, ((fn, _), _), [ax]) when (fn=fst g_verilog_unsigned_cast || fn=fst g_verilog_signed_cast) -> V_CALL(cf, (g_verilog_signed_cast, None), [ax])
    | ax -> V_CALL(g_null_callers_flags, (g_verilog_unsigned_cast, None), [ax])    


(*
 * Pulling finish inside an IF block is not good in general: we want it to stay last.
 * Recent ifshare code respects eis hpr call ordering though so the problem of premature exit is now fixed in that way ...
 *)
let finish_pred = function
    | V_EASC(V_CALL(_, (("$finish", gis), _), _)) -> true
    | other -> false

let rec gec_V_BA (l, r) =
    match (l, r) with
        | (V_BITSEL(lhs, bh, bl), V_DIADIC(prec, V_BITAND,V_MASK (hh,ll), rr)) when bh=hh && bl=ll -> gec_V_BA(l, rr)

        | (l, r) ->
            //vprintln 0 (sprintf "+++ BA other %A r=%A" l r)
            V_BA(l, r)

let rec gec_V_NBA (l, r) =
    match(l, r) with
        | (V_BITSEL(lhs, bh, bl), V_DIADIC(prec, V_BITAND,V_MASK (hh,ll), rr)) when bh=hh && bl=ll -> gec_V_NBA(l, rr)

        | (l, r) ->
            //vprintln 0 (sprintf "+++ NBA other %A r= %A" l r)
            V_NBA(l, r)


(*
 * 'ifshare' - if we have two, successive IF's with the same guard we can share the guard over the bodies.
 *
 * This is an old or 'trivial' ifshare function called when generating Verilog AST.  There are heavyweight implementations elsewhere (meox has hbev_ifshare, used by cpp, then there's new_ifshare below) .
 * For best-looking output we need to up and down it, espresso like: we want to share guards over cmds and cmds over guards.

 * This applies to conjunctions of clauses as well, so we pre-process to clause and cmd form...? 
 * This one works on VERILOG.
 *)
let rec old_v_ifshare msg arg =
    match arg with
    | [] -> []
    | V_IF(g, v)::V_IF(g', v')::tt -> 
        let a1 = V_IF(g, v)
        let b1 = V_IF(g', v')
        // use built-in equality - this can be very slow, seemingly even when not matching - please justify
        if g=g' (* && not(finish_pred v) *)
        then old_v_ifshare msg (gen_V_IF(g, gen_V_BLOCK[v; v'])::tt)
        else
            //dev_println (sprintf "old_v_ifshare: %s did not if_share between guards\n   G1=%s and\n   G2=%s" msg (ecToStr1 g) (vToStr g'))
            a1::old_v_ifshare msg (b1::tt)

    | other::tt -> other :: old_v_ifshare msg tt


let xi_ggen tieoffs x g =
    match x with
        | None -> xi_brewrite tieoffs g
        | Some pp ->
            let ppg = xi_deqd(fst pp, snd pp)
            //vprintln 3 ("xi_ggen ppg= " + xbToStr ppg)
            xi_brewrite tieoffs (ix_and ppg g)




let vgen_STRING s = V_STRING(xi_string s, s)

let edd_op_trap = ref V_bitor


// Factor for net declearation wrapping.
let gec_cx site cx1 = 
    let cxleft (ao, x) =
        let vnet = cx1 x
        if hexpt_is_io x then
            if not_nonep ao then vprintln 3 (sprintf "wrap actual: cxleft:non null ao: site=%s ao=%A on %s" site ao (netToStr x)) // string option
            VDB_formal(vnet)
        else VDB_actual(ao, vnet)
    (cxleft, (fun (a,b) -> gec_VDB_group(a, b)))


let gec_cx_always_actual site cx1 = 
    let cxleft (ao, x) =
        let vnet = cx1 x
        VDB_actual(ao, vnet)
    (cxleft, (fun (a,b) -> gec_VDB_group(a, b)))


(*
 * This is the gate builder routine for expressions.
 * All variables must be one bit wide for correct processing here.
 *)


// String table: emitted as parameters since then it is valid to subscript them as and when needed. 
// Or alternatively emitted as initialised read only registers: the subscript then needs multiplying by 8 to an 8 bit range.
let g_emit_verilog_strings_as_constant_regs = true



let g_default_string_netinfo id slen n = // TODO messy - we either dont want this for strings or else want to tidy it up
    let ff =
        { 
            n=           n
            rh=         -1I
            rl=         -1I
            id=         id
            width= -1 // There's a convention: strings w=-1 and void is -2. Or could put slen * 8 in here.
            constval=   []
            signed=     Signed
            is_array =  false
        }
    let f2 =
        {
            length=     [] 
            dir=        false
            pol=        false
            ats=        []
            xnet_io=    LOCAL 
            vtype=      V_VALUE
        }
    (ff, f2)
        
let verilog_subexps pe =
    let escapestr = insert_string_escapes true
    let alias = ""
    let emit_as_pram (a, (b, netinfo)) = (V_PRAMDEF(V_PARAM(b, netinfo), Some(vgen_STRING(escapestr a))), ref None)
    let emit_as_const_reg (a, (b, netinfo)) =
        let x1 = escapestr a
        let init_str = vgen_STRING x1
        let slen = strlen x1
        (V_NETDECL(gec_X_net b, None, valOf_or netinfo (fst (g_default_string_netinfo b slen 0)), (slen * 8), alias, -1L, VNT_REG, Some init_str), ref None)
        

    let tostr s = sprintf "\"%s\"" s
    let rr = map (if g_emit_verilog_strings_as_constant_regs then emit_as_const_reg else emit_as_pram)  !pe.m_verilog_strings
    pe.m_verilog_strings := []
    rr

let abvi_mapping =
    [
        (V_band, V_LOGAND);
        (V_bor, V_LOGOR)
    ]

let bvi_mapping = 
    [
        ( V_dltd Signed,        V_DLTD);
        ( V_dltd Unsigned,      V_DLTD);
        ( V_dltd FloatingPoint, V_DLTD);                
        ( V_deqd, V_DEQD);

        ( V_dled Signed, V_DLED)
        ( V_dled Unsigned, V_DLED)
        ( V_dled FloatingPoint, V_DLED)                
    ]

let vi_mapping = 
    [( V_minus, V_MINUS);
     ( V_mod, V_DMOD);
     ( V_plus, V_PLUS);
     ( V_bitand, V_BITAND);
     ( V_bitor, V_BITOR);
     ( V_xor, V_XOR);
     ( V_divide, V_DIVIDE);
     ( V_times, V_TIMES);
     ( V_lshift, V_LSHIFT);
     ( V_rshift Signed, V_ArithRSHIFT);     // Preserves sign bit.
     ( V_rshift Unsigned, V_LogicalRSHIFT)  // Shifts in zeros at the top.
     ]

// V_MASK: Constant number with bit positions (h..l) set to ones. For example, V_MASK(3,1) == 14
// TODO Consider using xi_bnum_n
let presim_mask = function
    | V_MASK(h, l) -> xi_bnum(himask (h+1) - (if l=0 then 0I else himask l))    


let vdiTox k = 
    let rec j = function
        | [] -> None (* sf("vdi operator not listed: " + vdopToS k) *)
        | (a,b)::t -> if b=k then Some a else j t
    (j vi_mapping, j bvi_mapping, j abvi_mapping)



let xToVdi__ k =  // unused
    let rec j = function
        | [] -> sf("xToVdi diadic other: " + f1o3(xToStr_dop k))
        | (a,b)::t -> if a=k then b else j t
    j vi_mapping


let xbToVdi k = 
    let rec j = function
        | [] -> sf("xbToVdi diadic other: " + f1o4(xbToStr_dop k))
        | ((a,b)::t) -> if a=k then b else j t
    j bvi_mapping



//
// Generate sequential logic gate-level flip-flops.
//
let vgen_seq (clknet, (rstnet:v_exp_t option), (reset_expr_, is_asynch, vreset_o), (fv1, rvu, (p:eqToL_t), aliases)) (l, d, cen) = // Generate a D-type flip-flop
    let undef_vnet = V_X 1
    let ats = []

    let gec_V_EVC_POS clk = V_EVC_POS clk

    let v_evc_gen (pp:eqToL_t) reset_expr clk = // dont need both
        if is_asynch && not_nonep vreset_o then
            V_EVC_OR(gec_V_EVC_POS clk, gec_V_EVC_POS (valOf vreset_o)) // Assume clock net has already been inverted and reset_expr, if asynch, is a single net that was or-reduced.
        else
            gec_V_EVC_POS clk

    let vreset = valOf_or vreset_o g_vfalse

    let wrap x =
        //dev_println (sprintf "wrap actual 0/2 %A" x)
        VDB_actual(None, x) // Nicer to put net names in here
    match l with
        | V_NET(lhs, _, n) ->
            let i = funique("itn")
            let xg = V_INSTANCE(rvu.loid, g_CVDFF, i, [], map wrap [l; d; clknet; cen; vreset; g_vfalse  ], ats);
            mutadd rvu.m_units (xg, ref None)
            //let _ = vprintln 0 ("Recording dff driver use " + vnetToStr l)
            record_net_use true (rvu, p) xg (wrap l) 0
            record_net_use false (rvu, p) xg (wrap cen) 4 // Note: clock and resets are not logged here
            record_net_use false (rvu, p) xg (wrap d) 2                       
            (undef_vnet:v_exp_t)

        | V_BITSEL(lhs, _,  _) ->
            let i = funique("itb")
            let xg = V_INSTANCE(rvu.loid, g_CVDFF, i, [], map wrap [l; d; clknet; cen; vreset; g_vfalse  ], ats);
            mutadd (rvu.m_units) (xg, ref None)
            record_net_use true (rvu, p) xg (wrap l) 0
            record_net_use false (rvu, p) xg (wrap cen) 4  // Note: clock and resets are not logged here
            record_net_use false (rvu, p) xg (wrap d) 2  
            undef_vnet

        | V_SUBSC(a, r) -> 
            let xg = V_ALWAYS(V_BLOCK[V_EVC(v_evc_gen p rstnet clknet); gen_V_IF(cen, gec_V_NBA(l, d))])
            record_net_use true (rvu, p) xg (wrap l) 0 /// hmmm record will not work for a V_SUBSC but these are not used in netlists.
            record_net_use false (rvu, p) xg (wrap cen) 4
            record_net_use false (rvu, p) xg (wrap d) 2
            mutadd (rvu.m_units) (xg, ref None)
            undef_vnet

        | V_NUM(a, signed, b, c) when isnum d ->
            if (a, signed, b, c) = getnum d then undef_vnet else sf ("attempt num write")

        | l -> 
            let xg = V_ALWAYS(V_BLOCK[V_EVC(v_evc_gen p rstnet clknet); gen_V_IF(cen, gec_V_NBA(l, d))])
            record_net_use true (rvu, p) xg (wrap l) 0
            record_net_use false (rvu, p) xg (wrap cen) 4
            record_net_use false (rvu, p) xg (wrap d) 2
            mutadd (rvu.m_units) (xg, ref None)
            verror("vgen seq other: " + (vToStr d) + "\n")
            undef_vnet



//
// Continuous assigns with taskcalls in the rhs sometimes cause problems further down the
// toolchain, so we need to render them as fully-supported @( * ) blocks.
// On the other hand, if the rhs is a constant the @( * ) may not be triggered for simulation and hence we don't want that.
//
let gec_V_CONT (fv1:FV_t) as_real_cont_assign = function
    | (l, V_X _) ->
        //let _ = vprintln 0 (sprintf "gec_V_CONT l=%A  ans=>%s<" l (vToStr l))
        (V_SCOMMENT (sprintf " %s := *X_undef*;" (vToStr l)), ref None)

    | (l, r) ->
        let delo = fv1.comb_delayo
        let as_real_cont_assgin = as_real_cont_assign || v_isconst r  // Constant assign must be rendered as real continuous assigns since RTL simulators object to null full support.
        //let _ = vprintln 0 (sprintf "gec_V_CONT +++ l=%s  rconst=%A as_real_cont_assign=%A r=%s " (vToStr l) (v_isconst r) as_real_cont_assign (vToStr r))
        // Note, calls to pause mode setter do return compile-time constants but they look like applies here?
        (V_CONT ((delo, as_real_cont_assign), l, r), ref None)

let gec_V_CONT_g (fv1:FV_t) as_real_cont_assign (g, l, r) =
    if g=vtrue() then gec_V_CONT fv1 as_real_cont_assign (l, r)
    else
        //let _ = vprintln 0 (sprintf "gec_V_CONT_g +++ l=%s  rconst=%A r=%s " (vToStr l) (v_isconst r) (vToStr r))        
        let rhs = V_QUERY(g, r, l)
        gec_V_CONT fv1 as_real_cont_assign (l, rhs)


(*
 *
 *)
let vgen_buf_leaf msg g0 (fv1, lono:layout_zone_t option, p:eqToL_t, aliases) two_ (l, d) =
    match lono with
//        | None -> gec_
        | Some rvu ->
            let ats = []
            lprintln fv1.vd (fun () -> msg + ": vgen_buf_leaf l=" + vToStr l + "   guard=" + vToStr g0)
            let xg =
                match l with
                    | V_NET(lhs, _, n) ->
                        let i = funique("vbuf")
                        let xg = if g0=vtrue() then V_INSTANCE(rvu.loid, g_CVBUF, i, [], map wrap [ l;  d ], ats) // todo include comb_delayo as a ride
                                               else V_INSTANCE(rvu.loid, g_CVBUFIF1, i, [], map wrap [ l;  d; g0 ], ats)
                        let _ = record_net_use true (rvu, p) xg (wrap l)
                        let _ = record_net_use false (rvu, p) xg (wrap g0)
                        let _ = record_net_use false (rvu, p) xg (wrap d)
                        xg
                    | V_BITSEL(lhs, _,  _) ->
                        let i = funique("vbus")
                        let xg = if g0=vtrue() then V_INSTANCE(rvu.loid, g_CVBUF, i, [], map wrap [ l;  d ], ats)
                                               else V_INSTANCE(rvu.loid, g_CVBUFIF1, i, [],  map wrap [ l;  d; g0 ], ats)
                        let _ = record_net_use true (rvu, p) xg (wrap l)
                        let _ = record_net_use false (rvu, p) xg (wrap g0)
                        let _ = record_net_use false (rvu, p) xg (wrap d)
                        xg

                    | (oo) -> sf(vToStr oo + ": vgen_buf")
            mutadd rvu.m_units (xg, ref None) 
            V_X 1



//
//
let vgen_diadic (fv1, rvu:layout_zone_t option, (pp:eqToL_t), aliases) prec (oo, l, r) = 
    //if oo=V_BITAND then vprintln 0 (sprintf "Generating bitand from l=%s r=%s" (vToStr l) (vToStr r))
    let gatef = not_nonep pp.prefs.gatelib && rvu <> None
    if gatef then
        match oo with
            | V_LOGAND | V_BITAND -> gbuild_and (valOf rvu, pp) (l, r)
            | V_LOGOR  | V_BITOR  -> gbuild_or2 (valOf rvu, pp) (l, r)
            | V_XOR               -> gbuild_xor (valOf rvu, pp) (l, r)
            //| V_MINUS  | V_PLUS   -> gbuild_arith oo (fv1, valOf rvu, p) (l, r)
            | V_DEQD -> gbuild_deqd  oo (fv1, valOf rvu, pp) (l, r)            
            | oo ->
                let _ = vprintln 0 (sprintf "+++ Gatelib does not contain a %s gate: l=%s r=%s" (vdopToS oo) (vToStr l) (vToStr r))
                V_DIADIC(prec, oo, l, r)  // so leave as expression in output RTL!
    else 
       (
         V_DIADIC(prec, oo, l, r) 
       )


// Bitwise AND. No longer normally used for masking (we now aim to use bn_masker in meox - certainly for constants).
let vgen_bitand prec pp (l, r) =
    match (l, r) with
        | (V_NUM(rr, signed, rb, orx), V_MASK(hh, ll))
        | (V_MASK(hh, ll), V_NUM(rr, signed, rb, orx)) ->
            // 
            let bn = xi_manifest_int "vgen_bitand constant mask" orx
            //let bn = himask(abs(hh-ll)+1) * bn_unary (min hh ll)
            let ans = bn_masker (mine_prec g_bounda orx).widtho prec bn
            //let msk = presim_mask(V_MASK(hh, ll))
            //let _ = dev_println (sprintf "verilog_gen: unexpected mask int constant: prec=%A   l=%A   r=%A  ans=%A" prec l r ans)
            // Consider using xi_bnum_n in the next line
            V_NUM(rr, signed, rb, ans)
            
        | (l, r) ->
            //let _ = vprintln 0 (sprintf "other fu %A %A" l r)
            if l=r then l else vgen_diadic pp prec (V_BITAND, l, r)

let vgen_bitor prec pp (l, r) = if l=r then l else vgen_diadic pp prec (V_BITOR, l, r)
let vgen_plus  prec pp (l, r) = vgen_diadic pp prec (V_PLUS, l, r)
let vgen_minus prec pp (l, r) = vgen_diadic pp prec (V_MINUS, l, r)
let vgen_xor   prec pp (l, r) = vgen_diadic pp prec (V_XOR, l, r)


let vgen_lshift prec p two (l, r) =
// Verilog RTL and C will drop bits off the left hand side as they shift out.
// There was a problem on the convert a float to a double ok.
    match r with
         | V_NUM(_, signed, _, x) when false && constantp x-> // A simple extend by cat does not preserve the precision... DO NOT USE unless we also do a bit extract on the upshifted bits.  
             let i = xi_manifest "lshift RTL" x
             let _ = vprintln 0 (sprintf " constant amount lshift by %i prec=%A" i prec)
             let tw = match two.widtho with
                        | None -> -1
                        | Some n -> n-i
             V_CAT [ (tw, l); (i, V_NUM(i, signed, "b", xi_zero)) ]
         | r -> vgen_diadic p prec (V_LSHIFT, l, r);


// Has an unsigned override, like elsewhere.
let vgen_rshift prec unsignedf p two (l, r) = vgen_diadic p prec ((if unsignedf then V_LogicalRSHIFT else V_ArithRSHIFT), l, r)

let vgen_times prec pp (l, r) = vgen_diadic pp prec (V_TIMES, l, r)

// Logical ops follow:
let vgen_bxor   prec p (l, r) = vgen_diadic p prec (V_XOR, l, r)
let vgen_logand prec p (l, r) = if l=r then l elif l=g_vtrue then r elif r=g_vtrue then l else vgen_diadic p prec (V_LOGAND, l, r);
let vgen_deqd   prec p (l, r) = if l=r then vtrue() else vgen_diadic p prec (V_DEQD, l, r);
let vgen_logor  prec p (l, r) = if l=r then l elif l=g_vfalse then r elif r=g_vfalse then l else vgen_diadic p prec (V_LOGOR, l, r);
let vgen_logorl prec p two_ lst = gbuild_orl p lst


// prec is the argument precision - the range is a bool obviously.
let vgen_dltd prec p (l, r) = vgen_diadic p prec (V_DLTD, l, r);
let vgen_dled prec p (l, r) = vgen_diadic p prec (V_DLED, l, r);
let vgen_dned prec p (l, r) = vgen_diadic p prec (V_DNED, l, r);
let vgen_dged prec p (l, r) = vgen_diadic p prec (V_DGED, l, r);


//
//  Generate a divide or mod operator, but only if not a special case.
//  Special cases are 1. rhs is a number
//                    2. there is no 2 yet.
let vgen_moddiv fv1 prec uns modf (rvu, (pp:eqToL_t), aliases) two (ll, rr) =
    let tailer _ = vgen_diadic (fv1, rvu, pp, aliases) prec ((if modf then V_DMOD else V_DIVIDE), ll, rr)
    let signed = prec.signed=Signed
    if isnum rr && pp.prefs.vnl_synthesis && (not modf) then         // RHS is a number.
        let mknum p = V_NUM(-1, signed, "d", xi_num p)
        let lw = valOf_or two 32
        let (a, signed, b, x) = getnum rr // Divide by constant: do reciprocal etc.. 
        let rr = xi_manifest "vgen_divide" x // 32 bit only ... hmmm!
        if rr=1 then
            if modf then V_NUM(-1, signed, "d", xi_zero) 
            else ll
        else

        let _ = cassert(rr<>0, "Generated program would contain a divide by zero")
        if (rr < 0) then tailer() // Only +ve denominators actually.
        else
        let rec binrecip pos xf =
            if pos >= lw || xf = 0.0 then []
            else
                let (v, xf) = if (xf >= 1.0) then (1, 2.0 * (xf-1.0)) else (0, 2.0 * xf)
                v :: (binrecip (pos+1) xf)
        let recip = binrecip 0 (1.0/((float)(rr)))
        vprintln 3 ("Binary reciprocal of " + xToStr x + " in " + i2s lw + " bits is " + sfold i2s recip + " (lsb first)")

        let rec doj pos  = function
            | 0::t -> doj (pos+1) t
            | 1::t ->
                let h = vgen_rshift prec uns (fv1, rvu, pp, aliases) two (ll, mknum pos)
                h:: (doj (pos+1) t)
            | [] -> []
        let k = doj 0 recip
        let rec dom = function
            | [] -> sf ("no dom")
            | [item] -> item;
            | h::t -> vgen_plus prec (fv1, rvu, pp, aliases) (h, dom t)
        let ans = dom k 
        ans // vgen_diadic p (V_DIVIDE, l, r);
    else tailer()

//=======
//        let (a, b, x) = getnum r
//        let po2 = pwr_of_two_pred x
        //let _ = vprintln 2 ("vgen_mod constant " +  xToStr x)
//        in vgen_diadic rvup (V_DMOD, l, r);
//    else vgen_diadic rvup (V_DMOD, l, r);

//let vgen_divide  p (l, r) = vgen_diadic p (V_DIVIDE, l, r);




let vgen_query (fv1, rvu, (pp:eqToL_t), aliases) = function
    | (V_NUM(_, signed_, _, n), t, f) ->
        if xi_monkey n=Some false then f elif xi_monkey n = Some true then t else sf "VGEN_QUERY"
    | (g, t, f) ->
        if t=f then t
        elif pp.prefs.gatelib=None || rvu=None then V_QUERY(g, t, f) else gbuild_mux2 (valOf rvu, pp) (g, t, f)

// RTL's logical not (aka !) is first an or-reduce to boolean and then a boolean complement.
let vgen_lognot (fv1, rvu, (pp:eqToL_t), aliases) = function
    | V_NUM(_, signed_, _, n) -> if xi_monkey n=Some false then vtrue() elif xi_monkey n = Some true then g_vfalse else sf "VGEN_LOGNOT"
    | x -> if rvu=None || pp.prefs.gatelib=None then V_LOGNOT x else gbuild_not (valOf rvu, pp) x


(*
 * Two logical nots do not cancel out on an operand of width greater than one bit - they or-reduce, but three or more cancel down to one or two.
 *)
let vgen_not p = function
    | V_LOGNOT(V_LOGNOT x) -> V_LOGNOT(x)
    | x -> vgen_lognot p x

// Apply peephole based on identity ~~(X) === X that holds regardless of width of X.
let vgen_onesc _ = function
    | V_1sCOMPLEMENT x -> x
    | x -> V_1sCOMPLEMENT x



let deconstv w ff = function
    | XC_double dv   -> V_NUM(64, false, "h", doubleToBits dv)

    | XC_float32 fv  -> V_NUM(32, false, "h", floatToBits fv)

    | XC_bnum(prec, bn, _) ->
        let w = valOf_or prec.widtho -1
        let _ =
            if w < 0 then
                let determined_width = bound_log2 bn // Computed and then ignored ... ? Re-checked later?
                //if determined_width > 32 then dev_println (sprintf "deconst: out-of-range bnum generated: width requested is %i but %A needs width %i" w bn determined_width)
                ()
        V_NUM(w, false, "d", gec_X_bnum(prec, bn))

    | other -> sf(sprintf "constant (marker) symbol %s does not have a supported value: has=%A" (netToStr(X_bnet ff)) other)


let debib_rez bufferf (lx, rx) = if bufferf then gec_V_BA(lx, rx) else gec_V_NBA(lx, rx)

//
// debib makes assignments: either combinational buffers or sequential registers with separate register resets when appropriate.
// 
let debib (eqtol:eqToL_t) bufferf gl l0o (w, ll, l', r') (buffers, seq3s) =
    //vprintln 0 (sprintf "resetval setting=%s mode=%s" eqtol.pp.reset_mode eqtol.pp.fv_resets)
    let (reset_expr, is_asynch, reset_nets) = // Repeated reset code. Does not handle mixed synch and asynch resets so far, but could be added.
         match eqtol.dir.resets with
             | [] -> (X_false, true, [])
             | (true,  is_asynch, resetnet)::ignored_ -> (ix_orl(map greset eqtol.dir.resets), is_asynch, map f3o3 eqtol.dir.resets)
             | (false, is_asynch, resetnet)::ignored_ -> (ix_orl(map greset eqtol.dir.resets), is_asynch, map f3o3 eqtol.dir.resets) 
    let bib = debib_rez bufferf (l',r')
    let rstcode resetval = debib_rez bufferf (l', V_NUM(w, false, "d", resetval))
    // dev_vprintln (m + sprintf ": w=%i assigns to " w + netToStr ll)
    if bufferf then ((gl, bib, [])::buffers, seq3s)
    else
        let reset = 
           if not (xi_isfalse reset_expr) then
               let rstval = resetval_o (if nonep l0o then ll else valOf l0o)
               //if not_nonep rstval then dev_println (sprintf "verilog_gen: gec reset, Site A, async_resetf=%A for %s" async_resetf (xToStr ll))
               if nonep rstval then [] else [ rstcode (valOf rstval) ]
            else []
        //dev_println (sprintf "debib: for %s created reset" (xToStr ll) + sfold verilog_render_bev reset)
        (buffers, (gl, bib, reset)::seq3s)




//
// Partition a cover (list of cubes) by v into negative, ambivalent and positive lists.
let cofactorate v cubes =
    let pos = ref []
    let amb = ref []
    let neg = ref []
    let sc1 cube = 
        let rec sc sofar = function
            | h::tt when h = v  -> mutadd pos ((rev sofar) @ tt)
            | h::tt when h = -v -> mutadd neg ((rev sofar) @ tt)
            | h::tt when abs h < abs v -> sc (h::sofar) tt
            | h::tt when abs h > abs v -> mutadd amb ((rev sofar) @ (h::tt))
            | []                       -> mutadd amb cube
        sc [] cube
    app sc1 cubes
    (!neg, !amb, !pos)

let gec_v_num_autowidth two arg =
    // Note: an X_num has implied width of 32.  Application of encoding width is just a double-check to ensure value is within bit-width range.
    // So, we do not emit constants larger than their bit field.
    let abase = encoding_width arg
    let width = 
            match two.widtho with
                | Some aa -> max aa abase 
                | None -> 32
    let signed = two.signed = Signed
        //let _ = vprintln 0 (sprintf "V_NUM width for bn %A is %i based on two=%A" (bn) width two)
    V_NUM(width, signed, "d", arg)

//
// Bubble sort, largest returned first, deleting items with arity below theshold.
//
let trimming_bubble_sort thresh lst = 
    let rec pass = function
        | (a,a1)::tt when a1 < thresh -> pass tt
        | (a,a1)::(b,b1)::tt when a1<b1 -> (b,b1)::pass ((a,a1)::tt)
        | x::tt-> x::pass(tt)
        | other -> other
    let rec iterate lst =
        let lst' = pass lst
        if lst=lst' then lst'
        else iterate lst'
    iterate lst


// Verilog's parameterised width constant sytnax, where Nbits is a parameter: example {{(Nbits-4){1'b0}}, 4'd10};
// Sign extender: eg convert from 4 to 8 bits with " { (4){arg[3]}, arg[3:0] }"
// To apply bit extracts we need 'arg' to be a net or function input.    



// Issue a width and/or signedness change to an integral expression.
let gen_rtl_cvt_fgis smaller_notlarger (domain:precision_t) (range:precision_t) =

    let fname =
        if smaller_notlarger then
            if range.signed = Signed then "rtl_signed_bitextract" else "rtl_unsigned_bitextract"
        else
            if range.signed = Signed then "rtl_sign_extend" else "rtl_unsigned_extend"
    //dev_println (sprintf "gen_rtl_cvt_fgis: smaller_notlarger=%A domain=%A  range=%A" smaller_notlarger (prec2str domain) (prec2str range))
    let validp = function
        | Some k when k > 0 -> true
        | _                 -> false
    let _ = assertf (validp domain.widtho && validp range.widtho) (fun () -> "gen_rtl_cvt_fgis: unspecified domain or range " + sprintf "gen_rtl_cvt_fgis: smaller_notlarger=%A domain=%A  range=%A" smaller_notlarger (prec2str domain) (prec2str range))
    (fname, { g_default_native_fun_sig with rv=range; args=[domain] })


let vgen_fixed_point_cast (fv1, rvu, pp, aliases) caster inner cvtf arg =
    if cvtf=CS_typecast then inner
    else
    let range = caster
    let domain_prec = mine_prec false arg
    let equal_widths =
        not_nonep range.widtho && not_nonep domain_prec.widtho && valOf range.widtho = valOf domain_prec.widtho
    let smaller_notlarger = // For equal precision but different signedness we use the smaller-mode code.
        not_nonep range.widtho && not_nonep domain_prec.widtho && valOf range.widtho <= valOf domain_prec.widtho
    //vprintln 0 (sprintf "fixed-precision cast smaller/notlarger=%A range.signed=%A" smaller_notlarger range.signed)
    let masking_ans() =
        let mask = V_MASK(valOf caster.widtho-1, 0)
        vgen_bitand caster (fv1, rvu, pp, aliases) (mask, inner) // Masking/bitand only works properly for making wider without sign extension and we only use it when we do not have both precisions.  We could possibly make a signed larger result without sign extension using a bitand, but that's never needed.  Making smaller with a bitand works numerically but does not return an expression of smaller width in Verilog terms, owing to Verilog's width-determining rules.

    let fcall_ans () = // fcall needs both precisions.
        if equal_widths || nonep domain_prec.widtho then
            (if domain_prec.signed=Signed then gen_verilog_signed_cast else gen_verilog_unsigned_cast) inner
        else
            let fgis = gen_rtl_cvt_fgis smaller_notlarger domain_prec caster
            V_CALL(g_null_callers_flags, (fgis, None), [inner])
    let ans =
        match (domain_prec.widtho, range.widtho, range.signed) with
            | (Some _, Some _, _)     -> fcall_ans()
            | _ ->
                vprintln 3 ("used backstop clause")
                masking_ans()
    //vprintln 0 (sprintf "verilog_gen: compile fixed_point_cast integer of %s to prec=%A  giving %A" (xToStr arg) caster ans)
    // Note that, as well as sign extension being needed when expanding the width of a signed number, sign non-extensions needs to be enforced on negative sign+magnitude bignumbers that are being cast to unsigned. 
    // TODO we need to sign extend sometimes
    ans


(*
 * Convert from x to v.  Generate gates if gatelib<>None.
 *
 * Semantics of bit-widths in Verilog expressions are a little bit unusual.  Expressions are either self-determining in width or context-determined.


 * Jonathan Bromley (Dulous) writes: All operands are widened to the same width as the widest - including
 * the left-hand side.  Then, do the arithmetic in that width. Then assign the result to the left-hand side,
 * dropping more-significant bits if necessary.
 * 
 * If ALL the right-hand side operands are signed, then widening is performed by sign extension
 * rather than by zero-fill.  If ANY right-hand side operand is unsigned, then ALL widening and arithmetic is unsigned.
 * This behaviour is NOT affected by whether the left-hand side is signed or unsigned.
 *
 * Self-determined expressions are those inside a cat (hence a+b will typically drop the carry inside a cat), on the rhs of shifts and the operands to comparison predicates and unary reductions where the context is a bool and would be meaningless.

   A problem can arise with  "UInt32 value = ((UInt32)key << 24);" where key is a byte.
       Unlike C/C++, Verilog does not have integer promotion and the rhs operand to a shift, being 32 bits wide does not make the expression compute in 32 bits.  The left shifting a byte by 24 bits makes it zero in Verilog semantics.

       A related problem is demonstrated with operators that can cause 'carries' such as addition, multiplication, subtraction and negate.
 *)

// Active pattern that can be used to assign values to symbols in a pattern
let (|Let|) value input = (value, input)



// Convert an hpr native call to either a gate instance or a Verilog PLI call or some inline code written to gax.
    // PLI calls are handled 3 ways:
    //  1. They are actually structural instances
    //  2. They are 'finessed' to alternative logic, like sysexit's write to the abend syndrome register.
    //  3. They are geninue PLI calls for rendering in the output RTL.
    // RTL PLI calls are never compiled to gates, but actual args to other structural instances should be. Some
// PLI calls are never compiled to gates, whether there args are is somewhat moot for synthesis, but actual args passed to other structural instances should always be.
//
let rec rtl_pli_or_leafcell ww (fv1, rvu, (pp:eqToL_t), aliases) ((f, gis), ordering) gg (bb, cc) args ags = 
    let cf = g_null_callers_flags
    let kpp_waypoint_rtl_finesse ww (f, gis) args =
        if not pp.prefs.keep_waypoints then []
        else
            let msg = "kpp_waypoint_rtl_finesse"
            let kpp_nets = ensure_kpp_nets pp
            let shin n =
                if n >= length args then []
                else   
                    let net = if n = 0 then fst kpp_nets else snd kpp_nets
                    let kpp_net = gec_V_NET -1 (netgut net)
                    let arg = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width args.[n]
                    let w = valOf_or (ewidth msg net) -1
                    [(w, net, kpp_net, arg)] // a tuple for debib
                    // let tid = TODO we need to record waypoints by thread
            //vprintln 0  (sprintf "trap Convert hpr_KppMark to directorate output assigns %A" args')
            (shin 0) @ (shin 1)

    let (bb, cc, f', ags) =
        match f with
            | "hpr_KppMark" ->
                let db = kpp_waypoint_rtl_finesse ww (f, gis) args
                let (bb, cc) = List.foldBack (debib pp false gg None) db (bb, cc)
                let ss = "<subsumed>"
                (bb, cc, ss, ags)

            | "hpr_select" -> // Used in toy BSV compiler.
                let ss = sprintf "hpr_unary_encode %i" (length ags - 1) // Discard select token from the args. But we want it. 
                (bb, cc, ss, tl ags)

            | "hpr_sysexit" ->
                let syndrome = if nullp args then xi_num 0 else hd args
                let (bb, cc)  =
                    if nonep pp.dir.abend_register then
                        if not(xi_iszero syndrome) then hpr_yikes(sprintf "Thread makes abend with syndrome code %s but no abend register in directorate style %A. Add -directorate-style=normal" (sfold xToStr args) pp.dir.style)
                        (bb, cc)
                    else
                        let ba_ = Xassign(valOf pp.dir.abend_register, syndrome)
                        // This is an OLD backdoor  - should go down the directorate path now. Please explain!
                        vprintln 3 (sprintf "hpr_exit RTL render rtl_finish_render_enable=%A  abend_syndome=%s" !g_rtl_finish_render_enable (xToStr syndrome))
                        let syndrome = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width syndrome
                        let abend_code_net = valOf_or_fail "no_abend_net" pp.dir.abend_register
                        let db = (32, abend_code_net, gec_V_NET -1 (netgut abend_code_net), syndrome) // a tuple for debib
                        let (bb, cc) = debib pp false gg (Some abend_code_net) db (bb, cc)
                        (bb, cc)
                let ss =
                    if !g_rtl_finish_render_enable then verilog_plimap f // We want it in the output code as well (generally).  What does this mean?
                    else "<subsumed>"
                (bb, cc, ss, ags)

            | "hpr_sysleds" ->
                let argument = if nullp args then xi_num 0 else hd args
                let (bb, cc)  =
                    if nonep pp.dir.unary_leds then
                        hpr_yikes(sprintf "sysleds: Thread writes unary_leds register in directorate style %A. Add -directorate-style=normal" pp.dir.style)
                        (bb, cc)
                    else
                        let ba_ = Xassign(valOf pp.dir.unary_leds, argument)
                        // This is an OLD backdoor  - should go down the directorate path now. Please explain.       
                        let argument = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width argument
                        let net = valOf_or_fail "no_led bus " pp.dir.unary_leds
                        let db = (32, net, gec_V_NET -1 (netgut net), argument) // a tuple for debib
                        let (bb, cc) = debib pp false gg (Some net) db (bb, cc)
                        (bb, cc)
                let ss = "<subsumed>"
                (bb, cc, ss, ags)
            | f -> (bb, cc, verilog_plimap f, ags)

    let nativep = f'.[0] = '$'    // Those starting with dollars sign are PLI natives to appear in the rendered RTL.
    let cannedp = is_hpr_rtl_canned f
    let g_rtl_zero = V_NUM(0, false, "", xi_num 0)
    if fv1.vd>=5 then vprintln 5 ("Convert PLI or instance to: cannedp=" + boolToStr cannedp + " native=" + boolToStr nativep + " f=" + f' + " for "  + hbevToStr (gec_Xcall((f, gis), args)))
    let ans =
        if f' = "<subsumed>" then None
        else
            let ans =
                if nativep || cannedp || f <> f' || rvu=None then // NO NO want instances sometimes even when rvu is none: TODO must add them to p.units instead of to rvu!
                    let (f, gis) =
                        if f = "hpr_abs" || f = "hpr_max" || f = "hpr_min" then // Why does this list not include items like single2double ?
                            let precx = map (mine_prec g_bounda) args
                            let rt = hd precx
                            let f = if rt.signed=FloatingPoint then "hpr_f" + f.[4..] else f // Create hpr_fabs, hpr_fmax, hpr_fmin etc that remain as function calls in rendered RTL.
                            (f, { gis with rv=rt; args=precx } )
                        else (f', gis) 
                    V_CALL(cf, ((f, gis), ordering), ags) // We do not need to guard here since guarded in parent at xref-whereguard. (Clearly instances cannot be guarded!)
                else
                    let rides = [] // for now
                    newgate rides (valOf rvu, pp) ({base_cell() with name=f}, map wrap ags) // Otherwise a structural instance.
            Some ans
    (bb, cc, ans)

and eqnToL_ss fv1 ww (rvu, pe: eqToL_t, aliases) reb aA =
    // Want to implement DP here? Not for rare_ss version.
    match aA with
        | W_string(ss, XS_fill n, _)
        | W_string(ss, XS_unquoted n, _)
        | (Let 0 (n, W_string(ss, _, _))) ->
            let j = baseup_string n ss
            if not reb
            then
                let ans = vgen_STRING(insert_string_escapes true j)
                //vprintln 3 ("String, not reb, but with escapes in eqnToL_ss gave " + vToStr ans)
                ans
            else
                let ov = op_assoc j !pe.m_verilog_strings 
                if not_nonep ov then V_PARAM(valOf ov)
                else
                    let id = funique "str"
                    let nv = (j, id)
                    vprintln 3 ("New verilog string logged as para m" + xToStr aA + " --> " + snd nv) 
                        //let net = arraynet_w (id, [strlen j + 1], 8); // Assume ASCII not unicode
                    let net = gec_X_net id // The above array form would need an initial value too (ROM).
                    let deb = function
                        | X_bnet ff -> Some ff
                        | _ -> None
                    mutadd pe.m_verilog_strings (j, (id, deb net))
                        // The net is not added as a net here, since that would output in .v file. But added in presim.
                    V_PARAM(id, deb net)

        | s -> eqnToL fv1 ww (rvu, pe, aliases) { signed=Signed; widtho=None } s



and v_evc_gen1 ww (fv1, rvu, (pp:eqToL_t), aliases) ox x =
    //let async_resetsf = pp.prefs.reset_mode = "asynchronous" - this is now passed down via the director.
    let (is_pos, asynch_resetsf, rst_net) =
        match pp.dir.resets with
            | (is_pos, is_asynch, resetnet)::tt ->
                if not_nullp tt then hpr_yikes("More than one reset. Only first used for now...")
                (is_pos, is_asynch, Some resetnet) // We look at only one for now.
            | _ -> (false, false, None)
    //
    //dev_println  (sprintf "verilog_gen: v_evc_gen1: using %s as reset rst_net.  p.sdir.rstnet=%A" (netToStr rst_net) (not_nonep p.sdir.rstnet))
    
    if not_nullp pp.dir.resets then
        mutaddonce pp.m_resetgrds (ix_orl (map greset pp.dir.resets))
    match ox with
        | E_pos _ when nonep rst_net -> V_EVC_POS x
        | E_neg _ when nonep rst_net -> V_EVC_NEG x
        | E_pos _ -> if asynch_resetsf
                     then V_EVC_OR(V_EVC_POS x, V_EVC_POS(eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec (valOf rst_net)))
                     else V_EVC_POS x
        | E_neg _ -> if asynch_resetsf
                     then V_EVC_OR(V_EVC_NEG x, V_EVC_POS(eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec (valOf rst_net)))
                     else V_EVC_NEG x

// Boolean equation to Verilog logic.
and beqnToL (fv1:FV_t) ww (rvu:layout_zone_t option, pp, aliases) known arg0 = 
    let nn = xb2nn arg0
    let dp_key = (nn, g_bool_prec)
    let (found, ov) = pp.dp.TryGetValue dp_key // Note, these can be negative.
    if found then ov // Added to different rvu ?
    else
    let btc = beqnToL fv1 ww (rvu, pp, aliases) known
    //vprintln 0 ("exp call " + xbToStr arg0)
    let arg = simplifyb_ass (known:known_t) 1 arg0 
    //vprintln 0 ("exp return " + xbToStr arg)
    let precb = g_default_prec // suitable for boolean args
    let ans = 
        match arg with 
        | X_dontcare -> g_vfalse // hmm why not RTL dontcare?
        | X_false    -> g_vfalse
        | X_true     -> vtrue()

        | W_cover(lst, _) ->
            let tally_term (m:Map<int, int>) v0 =
                let v = abs v0
                let ov = m.TryFind v
                let nv = if ov=None then 1 else 1 + valOf ov
                m.Add(v, nv)
            let tally_cube m cube = List.fold tally_term m cube

            let tallies = trimming_bubble_sort 2 (Map.toList((List.fold tally_cube Map.empty lst)))
            //vprintln 0 ("terms=" + i2s(length lst) + " tallies " + sfold (fun (a,b) -> i2s a + ":" + i2s b) tallies)


            let rec conj lst = if rvu<>None && pp.prefs.gatelib<>None then gbuild_andl (fv1, valOf rvu, pp) lst
                               else match lst with
                                       | [] -> vtrue()
                                       | [item] -> item
                                       | h::t when h = g_vtrue -> conj t
                                       | h::t when h = g_vfalse -> h
                                       | h::t -> vgen_logand precb (fv1, rvu, pp, aliases) (h, conj t)

            let rec disj lst = if rvu<>None && pp.prefs.gatelib<>None then gbuild_orl (fv1, valOf rvu, pp) lst
                               else match lst with
                                       | [] -> g_vfalse
                                       | [item] -> item
                                       | h::t when h = g_vtrue -> h
                                       | h::t when h = g_vfalse -> disj t
                                       | h::t -> vgen_logor precb (fv1, rvu, pp, aliases) (h, disj t)

            let rec prods lst =
                if rvu<>None && pp.prefs.gatelib<>None then gbuild_andl (fv1, valOf rvu, pp) (map (fun h->btc(deblit h)) lst)
                else
                match lst with
                | [] -> vtrue() // unused.
                | [item] -> btc(deblit item)
                | h::t -> vgen_logand precb (fv1, rvu, pp, aliases) (btc(deblit h), prods t)

                
            let rec sums lst = // accepts a list list
                if rvu<>None && pp.prefs.gatelib<>None then gbuild_orl (fv1, valOf rvu, pp) (map prods lst)
                else
                match lst with
                | [] -> g_vfalse // unused.
                | [item] -> prods item
                | h::t -> vgen_logor precb (fv1, rvu, pp, aliases) (prods h, sums t)

            let rec spoc lst = function
                | [] -> sums lst
                | (v, cnt)::tt ->
                    //vprintln 0 (i2s cnt + " spoc " + xbToStr(deblit v))
                    let v' = btc(deblit v)
                    let (nnn, aaa, ppp) = cofactorate v lst

                    let aa1 = if aaa<>[] then [spoc aaa tt] else []
                    if nnn<>[] && ppp<>[] then
                        let nnx = spoc nnn tt
                        let ppx = spoc ppp tt
                        let q = vgen_query (fv1, rvu, pp, aliases) (v', ppx, nnx) 
                        disj (q::aa1)
                    else
                        let prod inv x =
                            let g = if inv then vgen_not (fv1, rvu, pp, aliases) v' else v'
                            conj [x; g] // need this order since x is often vtrue yet conj cannot delete last.

                        let nn1 = if nnn<>[] then [prod true (spoc nnn tt)] else []
                        let pp1 = if ppp<>[] then [prod false (spoc ppp tt)] else []

                        disj (nn1 @ aa1 @ pp1)
                    //((if nn<>[] then [prod true nn1] else []) @ (if aa=[] then [] else [aa1]) @ (if pp<>[] then [prod false pp1] else []))
            let ans = spoc lst tallies
            ans

#if NOTUSEDNOW
        | W_linp(v, LIN(pol, lst), _) -> 
            let signed = Signed // implied linp mode (for now)
            let v' = eqnToL fv1 ww (rvu, pp, aliases) None v
            let w = valOf_or (ewidth "linp-v" v) -1
            let tc = eqnToL fv1 ww (rvu, pp, aliases) (Some w)
            let rec k pol = function
                 | []  -> btc (if pol then X_true else X_false)
                 | [a] -> (if pol then vgen_dltd else vgen_dged) g_default_prec (rvu, pp, aliases) (v', tc(xi_num a))

                 | [a;b] when b=a+1 -> (if pol then vgen_dned else vgen_deqd) g_default_prec (rvu, pp, aliases) (v', tc (xi_num a))

                 | a::tt when pol ->
                     let c0 = k (not pol) tt
                     vgen_logor (rvu, pp, aliases) (vgen_dltd g_default_prec (rvu, pp, aliases) (v', tc(xi_num a)), c0)


                 | a::b::tt when (not pol) && b=a+1 -> // deqd disjunct
                     let c0 = k pol tt
                     // never dned
                     let h = (if pol then vgen_dned else vgen_deqd) (rvu, pp, aliases) (v', tc (xi_num a))
                     vgen_logor (rvu, pp, aliases) (h, c0)

                 | a::b::tt when (not pol) -> // generic range disjunction
                     let c0 = k pol tt
                     let h = vgen_logand (rvu, pp, aliases) (vgen_dged (rvu, pp, aliases) (v', tc (xi_num a)), vgen_dltd g_default_prec (rvu, pp, aliases) (v', tc (xi_num b)))
                     vgen_logor (rvu, pp, aliases) (h, c0)

            k pol lst
        //| W_linp(v, LINP(false, [i]), _) ->  vgen_dged (tc v, tc (xi_num i))
        //| W_linp(v, LINP(false, [i;j]), _) when j = i+1->  vgen_deqd (tc v, tc (xi_num i))
        //| W_linp(v, LINP(true, [i;j]), _)  when j = i+1->  vgen_dned (tc v, tc (xi_num i))
#endif

        | W_bdiop(V_orred, [x], i, _) ->
            let wo = ewidth "beqn orred" x
            let tc = eqnToL fv1 ww (rvu, pp, aliases) { signed=Signed; widtho=None }
            let a0 =
                if wo = Some 1 then tc x
                else vgen_not (fv1, rvu, pp, aliases) (vgen_not (fv1, rvu, pp, aliases) (eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec x)) 
            if i then vgen_not (fv1, rvu, pp, aliases) a0 else a0

#if USING_BMUX
        | W_bmux(gg, X_true, X_false, _) -> vgen_not (fv1, rvu, pp, aliases) (btc gg)
        | W_bmux(gg, X_false, X_true, _) -> (btc gg)

        | W_bmux(gg0, X_false, conj, _) ->
            let efo = determine_efo (new_known_dp []) gg0
            let gg = simplify_assuming_efos known gg0 efo
            let (known_ff, known_tt) = new_known known efo 
            vgen_logand (rvu, pp, aliases) (beqnToL fv1 ww (rvu, pp, aliases) known gg, beqnToL fv1 ww (rvu, pp, aliases) known_tt conj)
        | W_bmux(gg0, disj, X_true, _)  -> 
            let efo = determine_efo [] gg0
            let gg = simplify_assuming_efos known gg0 efo
            let (known_ff, known_tt) = new_known known efo
            vgen_logor (rvu, pp, aliases) (beqnToL fv1 ww (rvu, pp, aliases) known gg, beqnToL fv1 ww (rvu, pp, aliases) known_ff disj)

        | W_bmux(gg0, X_true, cp, _)  ->
            let efo = determine_efo [] gg0
            let gg = simplify_assuming_efos known gg0 efo
            let (known_ff, known_tt) = new_known known efo
            vgen_logor (rvu, pp, aliases) (vgen_not (rvu, pp, aliases) (beqnToL fv1 ww (rvu, pp, aliases) known gg), beqnToL fv1 ww (rvu, pp, aliases) known_tt cp)

        | W_bmux(gg0, cp, X_false, _) ->
            let efo = determine_efo [] gg0
            let gg = simplify_assuming_efos known gg0 efo
            let (known_ff, known_tt) = new_known known efo
            vgen_logand (rvu, pp, aliases) (vgen_not (rvu, pp, aliases) (beqnToL fv1 ww (rvu, pp, aliases) known gg), beqnToL fv1 ww (rvu, pp, aliases) known_ff cp)


        | W_bmux(gg0, ff, tt, _) ->
            let efo = determine_efo [] gg0
            let gg = simplify_assuming_efos known gg0 efo
            let gg' = beqnToL fv1 ww (rvu, pp, aliases) known gg // HMM can augment known here TODO
            let ff' = beqnToL fv1 ww (rvu, pp, aliases) known ff
            let tt' = beqnToL fv1 ww (rvu, pp, aliases) known tt
            vgen_query (rvu, pp, aliases) (gg', tt', ff') (* sic: Note order of args is swapped *)
#endif

        | W_bnode(oo, lst, i, _) ->
              let lst' = map btc lst
              let ff = if oo=V_band then vgen_logand
                       elif oo=V_bor then vgen_logor
                       elif oo=V_bxor then vgen_bxor
                       else sf "W_bnode nonop"
              let rec k = function
                  | [] -> sf "need identity"
                  | [item] -> item
                  | h::t -> ff g_default_prec (fv1, rvu, pp, aliases) (h, k t)
              let a0 = k lst'
              // We don't encounter this form very much owing to the use of bmux trees.
              let _ = vprintln // 0 ("W_bnode verilog_gen - NEEDS known ---> " + vToStr a0)
              if i then vgen_not (fv1, rvu, pp, aliases) a0 else a0


        | W_bdiop(oo, [d0; e0], inv, _) ->
            //Regarding signedness, dotnet byte code has an explicit denotation in the operator and that is reflected in our AST encoding in our hexp_t bidops.  This is clear.  When converting to an x_bdiop_t (from a non-dotnet source) we must put in the correct designation and when converting out, as in cpp_render or verilog_gen, we must generate casts when needed.

            //if oo=V_deqd then dev_println(sprintf "Monitor of %s" (xbToStr arg))
            let (prec_d, prec_e) = (mine_prec g_bounda d0, mine_prec g_bounda e0)
            let (fp, width) =
                if oo=V_deqd || oo=V_dned then (false, omax arg0 (prec_d.widtho, prec_e.widtho))  // Use integer comparison for floating point equality checks
                else
                    match (prec_d.signed, prec_e.signed) with
                        | (FloatingPoint, FloatingPoint) -> (true, omax arg0 (prec_d.widtho, prec_e.widtho))
                        | (FloatingPoint, _)             -> (true, valOf_or_fail "omax1a" prec_d.widtho)
                        | (_, FloatingPoint)             -> (true, valOf_or_fail "omax1b" prec_e.widtho)
                        | (_, _)                         -> (false, 0)

            //vprintln 0 (sprintf "gen bdiop fp=%A arg=%s" fp (xbToStr arg))
            let cf = g_null_callers_flags
            let(a0, inv) =
                if fp then // Need to promote from integers to FP - can only do constant promotions from integer here (others must have been handled in restructure).  We can promote from single to double here though, since that is just wiring.
//x                    let l = 
                    let d0 = promote_to_fp width d0
                    let e0 = promote_to_fp width e0
                    let (l, r) = (eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width d0, eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width e0)
                    let (l, r, inv) =
                        match oo with
                            | V_dltd _ -> (l, r, inv)
                            | V_dgtd _ -> (r, l, inv)                            
                            | V_dled _ -> (r, l, not inv)
                            | V_dged _ -> (l, r, not inv)                            
                            | _ -> sf "L1012"
                    let width = omax arg0 (prec_d.widtho, prec_e.widtho)
                    (V_CALL(cf, (f_hpr_fp_dltd width, None), [l;r]), inv)

                else // Otherwise an integer comparison or equality of floats.
                    let signed_override = match oo with
                                              | V_dged signed 
                                              | V_dgtd signed 
                                              | V_dled signed 
                                              | V_dltd signed -> Some signed
                                              | _ -> None
                    let (d, e) = unsigned_correct_bdiop(d0, e0)
                    //dev_println(sprintf "comparison render of %s and %s  signed_override=%A" (xToStr d) (xToStr e) signed_override)
                    let (d, e) =
                        let prec = if not_nonep signed_override then { gg_unspec_width with signed=valOf signed_override } else gg_unspec_width
                        (eqnToL fv1 ww (rvu, pp, aliases) prec d, eqnToL fv1 ww (rvu, pp, aliases) prec e)

                    // dev_println (sprintf "pre-override oo=%A  ll=%s  rr=%s" oo (vToStr d) (vToStr e))
                    // To get signed behaviour we need both args to be signed.  Unsigned arises if either arg is unsigned.
                    let (d, e) =
                        if signed_override = Some Signed then
                            //let _ = vprintln 3 (sprintf "signed_override comparison oo=%A l=%A R=%A   l=%s r=%s" oo (xi_is_signed d0) (xi_is_signed e0) (xToStr d0) (xToStr e0))
                            match (prec_d.signed=Signed, prec_e.signed=Signed) with
                                | (false, false) -> (gen_verilog_signed_cast d, gen_verilog_signed_cast e)
                                | (false, true)  -> (gen_verilog_signed_cast d, e)
                                | (true, false)  -> (d, gen_verilog_signed_cast e)
                                | (true, true)   -> (d, e)

                        elif signed_override = Some Unsigned then
                            match (prec_d.signed=Signed, prec_e.signed=Signed) with
                                | (false, false)
                                | (false, true) 
                                | (true, false)  -> (d, e)
                                | (true, true)   -> (gen_verilog_unsigned_cast d, gen_verilog_unsigned_cast e)
                        else (d, e)
                    let fx =
                        match oo with
                            | V_dled signed -> vgen_dled g_default_prec
                            | V_dltd signed -> vgen_dltd g_default_prec
                            | V_deqd -> vgen_deqd  g_default_prec
                            | V_dned -> sf "not nf"
                            | _ -> sf "bad W_bdiop"


                    (fx (fv1, rvu, pp, aliases) (d, e), inv)
            if inv then vgen_not (fv1, rvu, pp, aliases) a0 else a0

        | W_bitsel(X_bnet ff, n, i, _) -> 
            //let f2 = lookup_net2 ff.n
            if ff.id = g_tnow_string then (g_tnow_vnet n) (* Wrong place for this too !*)
            else
               let k = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width (X_bnet ff)
               let do_subscript = function
                   | V_NET(ff, id, -1)   -> V_NET(ff, id, n)
                   //| VD_BUS(ff, _) -> V_NET(ff, n)
                   //| VD_ARRAY _ -> muddy "fix me: bit select of array location"
                   | other -> sf (sprintf "invalid bitsel generated - Verilog only allows nets to be subscripted, not %A" other)
               let a0 = do_subscript k (* Wrong place for this TODO *)
               if i then vgen_not (fv1, rvu, pp, aliases) a0 else a0

            // Bitsel of an array location.
        | W_bitsel(W_asubsc(X_bnet f, a, b), n, i, _) -> 
            let A = W_asubsc(X_bnet f, a, b)
            let a0 = V_SUBSC(eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width A, V_NUM(-1, false, "d", xi_num n))
            if i then vgen_not (fv1, rvu, pp, aliases) a0 else a0

        | W_bitsel(other, n, i, _) ->
            //sf("eqnToL bitsel other: " + xkey other + " " + xToStr other)
            // standard dispatch: move to meo . todo.
            let rec power b n = if n<=1 then b else b * power b (n-1)
            let a0 = xi_orred(ix_bitand other (xi_bnum(power 2I n)))
            let a1 = if i then xi_not a0 else a0
            btc a1
            
        | other -> sf("beqnToL other " + xbToStr other)
    //vprintln vd ("beqnToL (nn=" +  i2s nn + ") " + xbToStr arg0 + " --> " + xbToStr arg + " gave " + vToStr ans)
    pp.dp.Add(dp_key, ans)
    ans

    
and eqnToL fv1 (ww:WW_t) (rvu, pp, aliases) (two:precision_t) x =
    let vd = !g_vnl_loglevel
    match x2nn x with
        | nn ->
            let dp_key = (nn, two)
            let (found, ov) = pp.dp.TryGetValue dp_key
            if found then
                //vprintln 0 ("eqnToL (nn=" +  i2s nn + ") dp recalled ") //  + vToStr ov
                ov
            else
                //vprintln 0 ("eqnToL (nn=" +  i2s nn + ") not dp recalled ")
                let ans = eqnToL_ntr fv1 ww (rvu, pp, aliases) two x
                if vd >= 5 then vprintln 5 (sprintf "eqnToL (nn=%i key=%s prec=%s) x=%s gave " nn (xkey x) (prec2str two) (xToStr x) + sprintf " 1/2  >%s< %A" (vToStr ans) x)
                let (found, _) = pp.dp.TryGetValue dp_key
                if not found then pp.dp.Add(dp_key, ans) // May have been inserted already by beqnToL!
                if vd >= 5 then vprintln 5 ("eqnToL (nn=" +  i2s nn + ") " + xToStr x + " gave " + vToStr ans)
                //dev_println ("eqnToL (nn=" +  i2s nn + ") " + xToStr x + " gave " + vToStr ans)                
                ans

// Make a note that a ROM has contents that will need to be rendered (in initial statements or for readmemh in future).
and note_rom_init_needed ww pp (net_att:net_att_t) vnet len =
    match pp.rom_inits.lookup net_att.n with
        | None ->
            let _ = vprintln 2 ("Noted a ROM init that will provide contents for " + net_att.id)
            pp.rom_inits.add net_att.n (net_att, vnet, len)
        | Some ov -> ()
        
// Equation to logic: two=target precision option for context determined expressions.  Our input is hexp_t, not RTL, so why would we need this: Providing widths to specifically unsized integers and when bit-blasting with pandex.
and eqnToL_ntr fv1 ww (rvu, pp, aliases) (two:precision_t) arg =
    let k0() = new_known_dp []

    // We must be careful with signed numbers:
    // For example
    //          $display("dispatch 4: TKWr1_5_V_1=%1d    <32'd0 = %1d     or     < 0 = %1d", TKWr1_5_V_1, TKWr1_5_V_1<32'd0, (TKWr1_5_V_1<0))
    //    Produces output dispatch 4: TKWr1_5_V_1=-1    <32'd0 = 0     or     < 0 = 1
    // -1 is less than 0 but it is not less than 32'd0  
                      

    let integer_promote (do_left, do_right) two (ll, rr) =
        let cf = g_null_callers_flags
        let ww = WN "integer_promote" ww
        let (lp, rp) = (vprec ww ll, vprec ww rr)
        let _ =
            if true || ((lp=32 && rp=32) || (lp=64 && rp=64)) then () // temp supress printing
            else vprintln 3  (sprintf "integer_promote (%A,%A) two=%A  l=%i r=%i" do_left do_right two.widtho lp rp)
        let tw =
            let tw = max lp rp
            if nonep two.widtho then tw else max tw (valOf two.widtho)
        let range_prec = { widtho=(if tw=0 then None else Some tw); signed=two.signed; }
        let extend domw arg =
            let dom_prec = { widtho=(if domw=0 then None else Some domw); signed=two.signed(*not really*); }
            if dom_prec.widtho = None then
                vprintln 3 (sprintf "Skip extend of %s owing to null domain width" (vToStr arg))
                arg
            else
                let fgis = gen_rtl_cvt_fgis false dom_prec range_prec
                V_CALL(cf, (fgis, None), [arg])
        let ll' = if do_left && lp < tw then extend lp ll else ll
        let rr' = if do_right && rp < tw then extend rp rr else rr            
        (range_prec, (ll', rr'))


    let gec_v_num prec bn arg =
        let abase_check = encoding_width arg  // Some belt-and-braces here but it was a troublesome feature.
        let width = if not_nonep prec.widtho then valOf prec.widtho else valOf_or two.widtho 0  // None denotes no explicit size. -1 denotes string handle.
        if abase_check > width && width > 0 then
            hpr_yikes (sprintf "gec_v_num: Insufficient two specified need %i but two=%s  arg=%s  %s" abase_check (prec2str two) (xkey arg) (xToStr arg))
        let signed = (prec.signed=Signed)
        let width =
            if width > 0 then width
            else 
                //if (signed && (bn < -2147483648I || bn > 2147483647I))||(not signed && bn < 0I || bn >= 2147483648I * 2I) then
                let determined_width = bound_log2 bn
                let width = if determined_width > 32 then determined_width else 32
                vprintln 3 (sprintf "+++ gec_v_num: determined width for %A as %i" bn width)
                width
        if abase_check > width then dev_println (sprintf "gec_v_num: Insufficient two specified two=%s  user_prec=%s arg=%s  %s" (prec2str two) (prec2str prec) (xkey arg) (xToStr arg))
        V_NUM(width, signed, "d", arg)


        
    match arg with
        | X_num n              -> gec_v_num_autowidth two arg
        | X_bnum(prec, bn, _)  -> gec_v_num prec bn arg
        | W_string(ss, XS_withval x, _) ->
            let ew = { signed=Signed; widtho=enum_coding_width arg}
            //vprintln 0 (sprintf "Encoding width for %s is %A" (xToStr arg) ew)
            V_ECOMMENT(ss, eqnToL fv1 ww (rvu, pp, aliases) ew x)
        | W_string(ss, _, _) -> eqnToL_ss fv1 ww (rvu, pp, aliases) false arg
        | X_blift s -> beqnToL fv1 ww (rvu, pp, aliases) (k0()) s

        | W_node(caster, V_cast cvtf, [arg], _) ->
            // This is different cast code from in meox - ideally we should share.
            let two_inner = if not_nonep caster.widtho then caster else two // Somewhat wierd semantics here: needs explanation.
            let cf = g_null_callers_flags
            let inner = eqnToL fv1 ww (rvu, pp, aliases) two_inner arg
            let iconstantp x = classed_constantp x = Constant_int
            let op_prec = mine_prec g_bounda arg
            let ans =
                if nonep caster.widtho then
                    vprintln 0 (sprintf "verilog_gen: skipping cast %A" caster)
                    inner
                elif iconstantp arg then
                    // If a constant arg we might normally expect this cast to have been processed earlier, but process it here anyway. 
                    // This clause ignores cvtf for no good reason!
                    let width = valOf caster.widtho
                    let bn = xi_manifest_int "vgen constant mask" arg
                    match bn_masker (mine_prec g_bounda arg).widtho caster bn with
                        | X_bnum(prec, bn, mm) when not_nonep prec.widtho ->
                            dev_println (sprintf "verilog_gen: unexpected late const cast/mask int constant: prec=%A, arg=%A" caster arg)
                            V_NUM(valOf prec.widtho, caster.signed=prec.signed, "10", X_bnum(prec, bn, mm))

                        | other ->
                            sf (sprintf "verilog_gen: mask int constant: prec=%A   arg=%A ans=%A " caster arg other)
                            //V_NUM(width, caster.signed=Signed, "10", xi_bnum_n width ans)

                elif caster.signed = FloatingPoint then
                    //let _ = vprintln 3 (sprintf "verilog_gen: compile cast of %s to prec=%A, op=%A" (xToStr arg) caster op_prec)
                    if cvtf <> CS_preserve_represented_value then inner
                    elif op_prec.signed=FloatingPoint then
                        match (caster.widtho, op_prec.widtho) with
                            | (Some 32, Some 32) -> inner
                            | (Some 64, Some 64) -> inner
                            | (Some 64, Some 32) -> V_CALL(cf, (g_hpr_flt2dbl_fgis, None), [inner])
                            | (Some 32, Some 64) -> V_CALL(cf, (g_hpr_dbl2flt_fgis, None), [inner])
                            | other -> sf (sprintf "verilog_gen: compile f/p  cast on %s, but strange precisions %A" (xToStr arg) other)
                    else
                        if op_prec.widtho=caster.widtho then inner
                        else
                            let ans = muddy (sprintf "verilog_gen: compile cast of %s to prec=%A from %A - restructure should have instantiated a CV_FP_CVT convertor" (xToStr arg) (prec2str caster) (prec2str op_prec))
                            inner
                elif op_prec.signed = FloatingPoint then // convert F/P to integer: we'd only expect this when restructure2 is disabled. (res2 instantiates multi0-cycle convertors in general)
                    if cvtf <> CS_preserve_represented_value then inner
                    else
                    match (caster.widtho, op_prec.widtho) with
                        | (Some 32, Some 32) -> V_CALL(cf, (g_hpr_flt2int32_fgis, None), [inner])
                        | (Some 32, Some 64) -> V_CALL(cf, (g_hpr_dbl2int32_fgis, None), [inner])
                        | (Some 64, Some 32) -> V_CALL(cf, (g_hpr_flt2int64_fgis, None), [inner])
                        | (Some 64, Some 64) -> V_CALL(cf, (g_hpr_dbl2int64_fgis, None), [inner])

                        | other -> sf(sprintf "verilog_gen: unsupported int width %A in convert from %A" other op_prec)
                else vgen_fixed_point_cast (fv1, rvu, pp, aliases) caster inner cvtf arg
            //vprintln 0 (sprintf "verilog_gen: compile cast of %s prec=%s to prec=%s  giving %A" (xToStr arg) (prec2str op_prec) (prec2str caster) ans)
            ans

        // One's complement is not a logical not, it is V_1sCOMPLEMENT. One's complement flips all bits, and is denoted with ~.
        | W_node(prec, V_onesc, [d], _) -> vgen_onesc (fv1, rvu, pp, aliases) (eqnToL_ntr fv1 ww (rvu, pp, aliases) two d)

        // Monadic negate
        | W_node(prec, V_neg, [d], _) ->
            //vprintln 0 (sprintf "Making monadic negate on %s prec=%s" (xToStr d) (prec2str prec))
            let tc = eqnToL_ntr fv1 ww (rvu, pp, aliases) two
            if prec.signed = FloatingPoint then // Floating monadic negate is a cheap operation - flip the msb.
                let sign_bit = tc (if prec.widtho=Some 32 then xi_bnum_n 32 (bn_unary 31) else xi_bnum_n 64 (bn_unary 63))
                vgen_xor prec (fv1, rvu, pp, aliases) (sign_bit, tc d)
            else
                //vprintln 0 ("Making minus of " + xToStr d)
                vgen_minus prec (fv1, rvu, pp, aliases) (V_NUM(-1, true, "d", xi_num 0), tc d) // Hmmm, we do not use V_2sCOMPLEMENT here.

        | W_node(prec, oo, [l;r], _) when oo = V_mod || is_divide oo -> 
              let lw = ewidth "W_node mod/div" l
              let uns = xi_is_unsigned l
              let ff = vgen_moddiv fv1 prec uns (oo=V_mod)
              let ll = eqnToL fv1 ww (rvu, pp, aliases) prec l
              let rr = eqnToL fv1 ww (rvu, pp, aliases) prec r 
              //vprintln 0 (sprintf " rr done for div/mod %s -> %A" (xToStr r) rr)
              ff (rvu, pp, aliases) lw (ll, rr)
          
        | W_node(prec, oo, lst, _) ->
            //vprintln 3 (sprintf "W_node operator %A" oo)
            let op_nonassoc a ff =
                   let _ = if length lst <> 2 then sf ("non-associative diadic operator expected two operands" + xToStr arg)    
                   // Note: no target width operand passed to rhs processing. Please say why!
                   let ap = (eqnToL fv1 ww (rvu, pp, aliases) two (hd lst), eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width (cadr lst))
                   let (prec_, ap) = integer_promote a two ap
                   ff (fv1, rvu, pp, aliases) two ap
            match oo with
                | V_interm    -> op_nonassoc (false, true) (vgen_buf_leaf "W_node" (vtrue()))
                | V_lshift    -> op_nonassoc (true, false) (vgen_lshift prec)
                | V_rshift ss -> op_nonassoc (true, false) (vgen_rshift prec (ss=Unsigned || prec.signed=Unsigned))
                | oo -> // other associatives
                    let lst' = map (eqnToL fv1 ww (rvu, pp, aliases) two) lst
                    let ff n ap = if oo=V_bitand    then vgen_bitand prec n ap
                                  elif oo=V_bitor   then vgen_bitor prec n ap
                                  elif oo=V_xor     then vgen_xor prec n ap
                                  elif oo=V_plus    then
                                      let (prec, ap) = integer_promote (true, true) two ap
                                      vgen_plus prec n ap
                                  elif oo=V_minus   then
                                      let (prec, ap) = integer_promote (true, true) two ap
                                      vgen_minus prec n ap
                                  elif is_times oo  then
                                      let (prec, ap) = integer_promote (true, true) two ap
                                      vgen_times prec n ap
                                  else sf ("eqnToL bad W_node op: " + f1o3(xToStr_dop oo))
                    let rec kfold = function
                        | [] -> sf "null zil" 
                        | [item] -> item
                        | h::t -> ff (fv1, rvu, pp, aliases) (h, kfold t) // Fold list on diadic comassoc operator.
                    kfold lst'

        | X_pair(l, r, _) ->
            let l' = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width l
            let r' = eqnToL fv1 ww (rvu, pp, aliases) two  r
            r'

        | W_query(g, d, e, _) -> vgen_query (fv1, rvu, pp, aliases) (beqnToL fv1 ww (rvu, pp, aliases) (k0()) g, eqnToL_ntr fv1 ww (rvu, pp, aliases) two d, eqnToL_ntr fv1 ww (rvu, pp, aliases) two e)

        | X_net(v, _) -> gec_V_NET -1 ((fun (X_bnet ff) -> ff)(simplenet v))

        | W_asubsc(W_string(a,b,c), n, _) ->
            let lhs = eqnToL_ss fv1 ww (rvu, pp, aliases) true (W_string(a,b,c))
            let subs = eqnToL_ntr fv1 ww (rvu, pp, aliases) gg_unspec_width n
            if g_emit_verilog_strings_as_constant_regs then
                let slen = strlen a // Must index from the other end so subtract index from length-1 before multiply by 8.
                let prec = g_default_prec
                let ll = V_DIADIC(prec, V_LSHIFT, V_DIADIC(prec, V_MINUS, V_NUM(-1, false, "", xi_num(slen-1)), subs), V_NUM(-1,false,  "", xi_num 3))
                V_SLICE(lhs, ll, 8)
            else 
                V_SUBSC(lhs, subs)

        | W_asubsc(a, n, _) -> V_SUBSC(eqnToL_ntr fv1 ww (rvu, pp, aliases) gg_unspec_width a, eqnToL_ntr fv1 ww (rvu, pp, aliases) two n)

        | X_bnet ff -> 
            //let f2 = lookup_net2 ff.n
            let tailer() =
                let alias = aliasfun vsanitize aliases ff
                V_NET(ff, alias, -1)

            match length ff.constval with
                | 0 -> tailer()
                | n when n > 1 ->
                    let vnet = tailer()
                    // TODO - abstract to gbuild so SystemC output etc can use it.
                    // A ROM is inferred from an X_bnet with a list of initial values.
                    let _ = note_rom_init_needed ww pp ff vnet n
                    vnet
                | 1 ->
                    match hd ff.constval with // 
                    | XC_string s ->
                        dev_println (sprintf "ignore XC_string init now %A" ff.constval)
                        tailer()
                    | other ->
                        let w = encoding_width (X_bnet ff)
                        deconstv w ff other

        | X_undef -> V_X (valOf_or two.widtho 1)

        | X_iodecl(id, hi,lo, w) -> V_X 1

        | W_apply((fname, gis), cf_, args, _) ->
                 // This site only support pure functions and we cannot return side effects in bb or cc
                 // RPC calls on child components should have been compiled to net-level operations by restructure.
                 // Function applies remaining here are converted to component instances.
                 // Except for:
                 //  1. rtl native     - things available in Verilog itself, such as $bitstoreal
                 //  2. hpr_canned     - like testandset,: we provide our own inline implementations.
                 //  3. hpr_rtl_canned - like  toChar, min, max, abs, flt2dbl: we provide our own inline implementations.            
                 // General PLI calls currently always come in as XRTL but could be here as well in future ?
            let args = if gis.needs_printf_formatting then replace_autoformat_string true (remove_concats args) else args
            let args' = map (eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width) args
            if fname.[0] = '$' || is_hpr_rtl_canned fname then
                let ord = None
                let (bb, cc, ans) = rtl_pli_or_leafcell ww (fv1, rvu, pp, aliases) ((fname, gis), ord) ((*cens*)[]) ([], []) args args'
                if not_nullp cc || not_nullp bb then hpr_yikes("side effects not captured from RTL render of " + fgisToStr(fname, gis))
                valOf_or_fail "pure RTL function needed in this context" ans
            elif gis.is_identity_fn_in_hw_terms then
                if length args = 1 then hd args'
                else sf (sprintf "verilog_gen: identity functions should be monadic (take one argument) " + fname)
            else
                vprintln 2 (sprintf "verilog_gen W_apply not_rlt_canned trap for %s" fname)
                match fname with
                    | "hpr_alloc" // alloc should not longer be hardcoded here in any way.
                    | "hpr_KppMark" ->
                        // The waypoint mechanism is part of an execution environment, like the directorate, so processing of KppMark here in verilog_gen is appropriate, so I don't like this message or code.
                        vprintln 0 (sprintf "+++ %s should have been replaced before Verilog RTL generation : args="  (fgisToStr (fname, gis)) + sfold xToStr args)
                        g_vfalse
                    | other ->
                        vprintln 0 (sprintf "+++ Ignoring unrecognised PLI/primitive %s: Omitting it from generated RTL"  (fgisToStr (fname, gis)))
                        g_vfalse

        | X_x(e, power, _) ->
            // Rmode X_x: we can synthesise delayed versions of any signal using flip-flops for any -ve power.
            // Lmode X_x with power -1 converts sequential to combinational logic: "Rarc(X_x(Q, -1), D)" or "Rarc(Q, X_x(D, 1))". These denote "assign q = d" in Verilog terms.
            // A value of -n in RMODE means the same as a value of +n in LMODE
            // Golden law: "X(Q)=X_x(Q, 1)=d" : ie, the next value of the Q output of a D-type is given by its D input. 

            // THIS CODE IS REPLICATION OF ABSTRACTE.REZ_PIPATE...  TODO RATIONALISE.

            let sdir = valOf_or_fail "L9821-sdir" pp.sdir
            let clknet = match sdir.clknet with
                                  | Some(E_pos x)
                                  | Some(E_neg x) -> valOf sdir.clknet
                                  | other -> sf(sprintf "not one clock net for X_x compilation: other=%A" other)
            let m_place = if rvu=None then pp.m_units else (valOf rvu).m_units
            let ck = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec (de_edge clknet) // Find an existing NBA for this net (few in general, so linear search ok).
            let ec = v_evc_gen1 ww (fv1, rvu, pp, aliases) clknet ck
            let rec expand_pipeline power e =
                if power > 0 then sf(sprintf "eqnToL other: X_x causality problem: +ve power=%i cannot occur on rhs in " power + xToStr arg)
                elif power = 0 then eqnToL fv1 ww (rvu, pp, aliases) two e
                else
                    vprintln 3 (sprintf "eqnToL expand X_x (we prefer other recipe stages to have handled this so as verilog_gen does not to make these...) power=%i of %s. rvu_present=%A" power (xToStr arg) (if nonep rvu then "none" else hptos (valOf rvu).loid))
                    let inner = expand_pipeline (power+1) e
                    let rec finder = function
                        | [] -> None
                        | (V_ALWAYS(V_BLOCK[V_EVC(ec'); V_NBA(l, d')]), _)::_ when ec'=ec && d'=inner -> Some l
                        | _::tt -> finder tt

                    match finder !m_place with
                        | Some oldgate -> oldgate
                        | None -> 
                            // Create a local copy of a register, delayed by one clock cycle.
                            let prec = mine_prec g_bounda e
                            let (ns, rr, net) = funique_net pp prec ("hpr_xx")
                            let xg =  V_ALWAYS(V_BLOCK[V_EVC ec; gec_V_NBA(rr, inner)])
                            let netinfo =
                                  {
                                      net=         rr
                                      driver=      ref (Some(xg, (if rvu=None then [] else (valOf rvu).loid)))
                                      net_static_time= ref None
                                      phy_data=    ref None
                                      uses=        ref []
                                  }
                              //record_net_use true (rvu, p) xg r // ::  want to use this instead!
                            pp.netinfo_dir.Add(ns, netinfo)
                            mutadd m_place (xg, ref None)
                            rr

            expand_pipeline power e
            
        | other -> sf("eqnTtoL other " + xToStr other)


(*
 * Pandex: Convert RTL to binary form (no vector nets) and thence to a netlist.
 * Pandex should be (or is already aswell?) in gbuild to be share over other forms, such as SMV and Promula output.
 *)
let pand_vgen_synch ww (fv1, aP, rvu, (pp:eqToL_t), aliases) (grd0, l, r) =
    let vd = 0
    let sdir = valOf_or_fail "L223" pp.sdir
    let m0 = "pand_vgen_synch" 
    let kK mm = () // vprintln 0 ("  m0 " + mm)
    //let clknet = match p.sdir.clknet with
    //                | [E_pos x] -> hd p.sdir.clknet
    //                | [E_neg x] -> hd p.sdir.clknet
    //                | other -> sf("not one clock net for "  + m0 +  ":" + sfold edgeToStr other)
    let ww' = WF 4 "pand_vgen_synch" ww  ("L=" + xToStr l)
    let ll = pandex ww' None aP l
    let ww' = WF 4 "pand_vgen_synch" ww  ("L done, " + i2s (length ll) + " bits. Now grd0=" + xbToStr grd0)
    let grd = bpandex ww' aP grd0
        //vprintln 3 ("pand_vgen_synch grd = " + xbToStr grd)
    let ww' = WF 4 "pand_vgen_synch" ww  ("grd done. Now R=" + xToStr r)
    let rr = pandex ww' (Some(length ll)) aP r
    kK("  pandex R=" + xToStr r + " gave " + sfold xbToStr rr)
        //let rstnet' = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec (valOf_or_fail "no reset net PGS" (p.sdir.rstnet))
        //let clknet' = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec (de_edge clknet) // Ideally do this cvt later...
    kK("  pandex of args complete")
(*
        let _ = print((xToStr clknet) + " was clocknet\n")
        let _ = print((verilog_render_exp clknet') + " was clocknet\n")
*)
    let xgen_dff l r = 
           if l = xi_false || l=xi_true then ()
           else let k0() = new_known_dp []
                let msg = "xgen bit dff: " + xbToStr l 
                let ww' = WF 3 msg ww "lhs"
                let l' = beqnToL fv1 ww' (rvu, pp, aliases) (k0()) l
                kK(" l dff done\n")
                if bconstantp r then
                    //let r' = beqnToL ww' (rvu, pp, aliases) (k0()) r
                    //let g' = beqnToL ww' (rvu, pp, aliases) (k0()) grd
                    pp.gatelevel_mux.add l' (l, Some sdir, grd, r)
                    //let _ = vgen_buf_leaf (rvu, pp, aliases) "dff-replaced-with-const" (g', l', r') // delete me
                    ()
                else
                let ww' = WF 3 msg ww "rhs"
                let (r1, cen) =
                    if pp.cen_factor then // cen-factorise both here on gen and later on emit
                        let (r1, cen) = lr_factor l r
                        kK("g cen dff mid \n")
                        let cen = bpandex ww aP cen
                        (r1, ix_and grd cen)
                    else (r, grd) // There's still a guard above!

                kK("lr factor done\n")
                //let r' = beqnToL ww' (rvu, pp, aliases) (k0()) r1
                //let cen' = beqnToL fv1 ww (rvu, pp, aliases) (k0()) cen
                pp.gatelevel_mux.add l' (l, Some sdir, cen, r1)
                kK("saved\n")
                // Do not emit until collated all assigns to this bit.
                //let _ = vgen_seq (clknet', rstnet', (fv1, valOf rvu, p)) (l', r', cen')  // HERE
                let _ = unwhere ww
                ()

    let rec gen =
            function
                | ([], _)        -> ()
                | (h::t, [])     -> (xgen_dff h xi_false; gen(t, [])) // xgen buf here please!
                | (h::t, r::rt)  -> (xgen_dff h r; gen(t, rt))
    //let _ = lprint vd (fun() -> (xToStr l) + " pand_vgen_synch generation done.\n")
    let ans = gen (ll, rr)
    kK("HERE 3\n")
    ans


// naively calls leaf vgen_buf_leaf above        
// No: need to gate_collatef over all assigns to a LHS and generate mux tree.
let pand_vgen_buf ww (fv1, aP, (pp:eqToL_t), aliases) rvu msg (g, l, r) =
        let vd = fv1.vd
        let rvu = new_rvu "vgenbuf" rvu
        let K m = lprintln  fv1.vd (fun()->"  pand_vgen_buf: " + m)
        let _ = K("HERE pand_vgen_buf top\n")
        let ww' = WF 4 "pand_vgen_buf" ww  ("L=" + xToStr l)
        let ll = pandex ww' None aP l
        let ww' = WF 4 "pand_vgen_buf" ww  ("R")
        let rr = pandex ww' (Some(length ll)) aP r
        let _ = K("mid1 g start\n")

        let k0() = new_known_dp [] // TODO - dp good or bad semantically ?
        //let g' = beqnToL ww' (rvu, pp, aliases) (k0()) g                
        //let _ = K("g' done mid2 \n")

        let msg' = msg + ":" + "xgen_buf"

        let xgen_buf(l, r) = 
           if l = xi_false || l=xi_true then ()
           else
                let msg = "xgen buf: " + xbToStr l 
                let ww' = WF 3 msg ww "lhs"
                let l' = beqnToL fv1 ww' (rvu, pp, aliases) (k0()) l
                let _ = K(" l bit done\n")
                let ww' = WF 3 msg ww "rhs"
                //let r' = beqnToL ww' (rvu, pp, aliases) (k0()) r
                //let _ = K("r bit done\n")

                pp.gatelevel_mux.add l' (l, None, g, r)
                //let ans = vgen_buf_leaf (rvu, pp, aliases) msg' (g', l', r') 
                let _ = K("buf done\n")
                let _ = unwhere ww
                ()

        let rec gen =
            function
                | ([], _)        -> ()
                | (h::t, [])     -> (xgen_buf(h, xi_false); gen(t, []))
                | (h::t, r::rt)  -> (xgen_buf(h, r); gen(t, rt))
        let _ = lprint vd (fun() -> (xToStr l) + " pand_vgen_buf generation done.\n")
        let ans = gen (ll, rr)
        let _ = K(" 3 done\n")
        ans

(* 
 * osensitivep : is a verilog statement order sensitive within the context of a synth always block (ie is it not pure RTL)
 * Note: we should also implement a consistency model like total-write-ordering here.
 * TODO: should check for pli calls in expressions I suppose.
 *) 
let rec osensitivep = function
    | V_BA (_)  -> true  // blocking assignments are order sensitive
    | V_NBA (_) -> false // non-blocking assignments are not order sensitive    

    | V_EASC(_)    -> true  (* pli calls normally have side effects *)
    | V_COMMENT(_) -> false
    | V_BLOCK l         -> List.fold (fun b bev -> b || osensitivep bev) false l
    | V_SWITCH(_, l, _) -> List.fold (fun b (tag, bev) -> b || osensitivep bev) false l
    | V_IF(g, ct)      -> osensitivep ct
    | V_IFE(g, ct, cf) -> osensitivep ct || osensitivep cf
    | other -> (vprintln 1 ("+++osensitivep other. evc?\n"); true)


            
//
//  'ifshare' - if we have two, successive IF's with the same guard we can share the guard over the bodies.
//  This is shim around ifshare_poly that can count frequency of guard terms and collate on the most frequent.
//  Generally the pc is promoted in apparent frequency to make it the main discriminant. 
//
let new_ifshare_v ww (control:control_t) (fv1, rvu, pp, aliases) cens lst =
    let vd = fv1.vd
    let zz = cmdline_flagset_validate "vnl-ifshare" ["none"; "simple"; "on" ] 0 control    
    let sharep_none = (zz=0)
    if sharep_none then vprintln 1 ("vnl-ifshare set to none - pretty formatting of IF statements disabled.")
    let k0() = new_known_dp [] // for now!
    let cvt x = eqnToL fv1 ww (rvu, pp, aliases) gg_unspec_width(*crude unspec_width here*) x 
    let bcvt x = beqnToL fv1 ww (rvu, pp, aliases) (k0()) x // (xi_simplif X_undef "verilog_gen bcvt" x)
    let gen_ifl (g, ttlst, fflst) = gen_V_IFE(bcvt(deblit g), gen_V_BLOCK ttlst, gen_V_BLOCK fflst) 

    //reportx 3 "new_ifshare_v work yes" (fun (gl, cmd, rst) -> sfold xbToStr gl + " : " + verilog_render_bev cmd) lst

    let finish_with_conjunction (on_, guards, cmd, reset) = gen_V_IF(bcvt (List.fold ix_and X_true (map deblit guards)), cmd)
    let rec finish complainf = function
        | (on, [], cmd, rst) -> cmd
        | (on, h::tt, cmd, rst) ->
            let h = deblit h
            if complainf then vprintln 0 ("+++ new_ifshare: finish work had repeated guard " + xbToStr h)
            gen_V_IF(bcvt h, finish complainf (on, tt, cmd, rst)) 

    //if vd <=10 then reportx vd "Reset guards for ifshare poly" xbToStr !pp.m_resetgrds
    let case_flags = { full_case=false; parallel_case=false; }
    let makecase (dispatch_exp, lst) = V_SWITCH(cvt dispatch_exp, map (fun (a, b) -> (map cvt a, gen_V_BLOCK b)) lst, case_flags)

    //vprintln 1 ("Timestamp: L1534 new_ifshare " + timestamp true)
    let ans = 
        if sharep_none then
            let k0() = new_known_dp []
            let fwc (gl, cmd, reset) = gen_V_IF(beqnToL fv1 ww (rvu, pp, aliases) (k0()) (List.fold (fun c a ->ix_and c a) X_true gl), cmd) // can use ix_andl here.
            //app (fun (g, cmd, reset)-> vprintln vd ("Ordering at ifshare:" + sfold "do not use slow sfold here" (verilog_render_bev cmd))) lst
            map fwc lst
        else 
            let ww1 = WF 4 "new_ifshare" ww (sprintf "start mode=%i (0=none,1=simple,2=on)" zz)
            let sharep_simple = zz=1
            let ans = new_ifshare_poly ww1 (sharep_simple, finish, finish_with_conjunction, osensitivep, gen_ifl, vnl_skip, makecase, cens, !pp.m_resetgrds) lst
            let ww1 = WF 4 "new_ifshare" ww (sprintf "done")
            ans 
    //vprintln 1 ("Timestamp: L1547 new_ifshare done " + timestamp true)
    ans



//
// Sort mixture of blocking and nonblocking RTL so that blocking comes first and behaves combinationally (or is ultimately placed in a separate comb block)
// and is sorted into combinational order, as far as possible - which will always be possible unless there are level-senstive parasitic latch loops.
let ba_sort ww fv1 regslist seqs =
    let vd = fv1.vd
    let groom_blocking lst =
        let grm arg (b, nb) =
            match arg with 
                | XRTL(pp, g, lst) ->
                    let q arg (b, nb) =
                        match arg with
                        | Rnop _ ->
                           let a = XRTL(pp, g, [arg])
                           if false then (a::b, nb) else (b, a::nb)

                        | Rpli(g1, fgis, args) ->
                           let a = XRTL(pp, g, [arg]) 
                           if false then (a::b, nb) else (b, a::nb)
                            
                        | Rarc(g1, l0, r) ->
                           let (l_, buffer) = find_blka l0                
                           if buffer then note_is_combreg regslist "L1847" l_
                           let a = XRTL(pp, g, [arg]) 
                           if buffer then (a::b, nb) else (b, a::nb)
                    List.foldBack q lst (b, nb)
                | other -> (b, arg::nb) // others on nb list
        let (b, nb) = List.foldBack grm lst ([], [])
        (b, nb)

    let (blocking, non_blocking) = groom_blocking seqs
    let trips:trip_t list = map (trip_x_to_sd_form ww) (List.fold (tripgen_x vd false) [] blocking) // Trips reverse again here!
    //reportx vd "Blocking pred trips" tripToStr trips
    vprintln vd ("len blocking=" + i2s(length blocking) + ", len non_blocking=" + i2s(length non_blocking))
    let sorted_blocking = trip_sorter ww trips
    //if sorted_blocking<>[] then reportx vd "Blocking pred trips sorted" int_tripToStr sorted_blocking
    if true then (map f2o4 sorted_blocking) @ non_blocking
    else seqs 

// Avoid latch inference by setting all blocking assigns used for comb logic to their default values.
let inti_to_default eqtol unconds cmds =
    
   let rec lsh_collate c = function
       | V_SWITCH(_, lst, _) -> List.fold lsh_collate c (map snd lst)
       | V_BLOCK lst         -> List.fold lsh_collate c lst       
       | V_BA(lhs, _) 
       | V_NBA(lhs, _)       -> singly_add lhs c
       | V_IF(g, ct)         -> lsh_collate c ct
       | V_IFE(g, ct, cf )   -> lsh_collate (lsh_collate c cf) ct       
       | other               -> sf ("other form " + other.ToString())
   let lhs_lst = List.fold lsh_collate [] cmds
   let clearme cc arg =
       match arg with
       | V_NET(ff, id, _) ->
           if memberp ff.n unconds then
               //dev_println (sprintf "ROSIE: suppress inti_to_default of %s since main control is unconditional" ff.id)
               cc
           else
               let f2 = lookup_net2 ff.n
               let net = V_NET(ff, id, -1)
               let tobn = BigInteger.Parse
               let ival = xi_bnum(tobn(valOf_or_failf (fun()->"no value for init/default in blocking assign for " + ff.id) (at_assoc "init" f2.ats)))
               //dev_println (sprintf "ROSIE: inti_to_default of %s to %s" ff.id (xToStr ival))
               V_BA(arg, eqtol (gg_unspec_width(*its a constant so no need for target width*)) ival)::cc
       | other -> sf ("other form " + other.ToString())       
   List.fold clearme [] lhs_lst

   
// For repeated commands that are idempotent or insensitive to order, form disjunction of their guards.
// now its work is already done by rtl_once
let rec once_cmda = function  
    | [] -> [] 
    | [item] -> [item]
    | (g, cmd, rst_info)::t ->
                     //vprintln 0 (verilog_render_bev(cmd) + " cmda ?")
                     let vcmd = function
                         | V_BA (_)
                         | V_NBA (_) -> true // Idempotent cmd: repeated instances of it are not needed
                         //| V_EASC(V_CALL(fgo, args)) -> muddy "call sef_eq .. "
                         |  _ -> false

                     let lov gl = List.fold (fun c a -> xi_and(c, a)) X_true gl
                     let rec scan = function
                         | [] -> None
                         | (g', cmd', rst_info')::tt when cmd=cmd' && rst_info=rst_info' -> Some (g')
                         | _ :: tt -> scan tt
                     let another = if vcmd cmd then scan t else None
                     if another=None then (g, cmd, rst_info)::(once_cmda t)
                     else (
                              //vprintln 0 (verilog_render_bev(cmd) + " was combined " + xbToStr g + " v " + xbToStr(valOf another));
                              once_cmda(([ix_or (lov(valOf another)) (lov g)], cmd, rst_info)::list_subtract(t, [(valOf another, cmd, rst_info)]))
                          )


let once_cmd x =   // FSM state contents: 
    let rr = once_cmda x
    //let _ = reportx 0 "before once" (fun (a,b,c)->verilog_render_bev b) x
    //let _ = reportx 0 "after once" (fun (a,b,c)->verilog_render_bev b) rr
    rr


(*
 * Convert xrtl to rtl (ie to Verilog) or gatelevel.  
 * Flag controllerf holds if there is more than one pc state for a thread.
 * Flag preserve_sequencer holds if we wish to show the pc states in a match statement RTL form.
 *
 * There are three main routes:
 *    1. fsm rtl output with sequencer (unless only one state).
 *    2. flat rtl output (no sequencer)
 *    3. gate output (no sequencer)
 *
 * We expect SP_fsm input for 1 and SP_rtl input clauses for 2 and 3.
 * Another output form is needed for the s-expressions.
 * 
 * Return triple/quad: ((module units, ant(*totally unused*), new nets, bevs))
 * Also returns a bool: whether a controller is needed. 
 *)
    // PLI calls are handled 3 ways:
    //  1. They are actually structural instances
    //  2. They are 'finessed' to alternative logic, like sysexit's write to the abend syndrome register.
    //  3. They are geninue PLI calls for rendering in the output RTL.
    // RTL PLI calls are never compiled to gates, but actual args to other structural instances should be. Some
let exec_flatten_pli ww (fv1, (pp:eqToL_t), tieoffs, aliases) rvu (bb, cc) gl_cens ((f, gis), order) args = 
    let msg = "kbev_pli"
    let p' = { pp with prefs={ pp.prefs with gatelib=None }}
    let rvu = new_rvu msg rvu
    let jval  r =
            // Use _ss version here? USE.
            let r' = xi_rewrite tieoffs r
            let ans = eqnToL fv1 ww (rvu, p', aliases) gg_unspec_width r'
            //vprintln 0 ("jval with p' for disable . " + xkey r + " :" + xToStr r + " --> " + xToStr r' + " --> " + vToStr ans)
            ans
    //let gl = [g0] @ (if pp=None then [] else [ix_deqd (fst(valOf pp)) (snd(valOf pp))])
    //lprint 10 (fun()->"kbev: pli call guard is " + (xbToStr g) + "\n")
    let args = if gis.needs_printf_formatting then replace_autoformat_string true (remove_concats args) else args
    let args' = map jval args
    if fv1.vd>=4 then vprintln 4 ("verilog_gen: kbev_pli: rvu=" + optToStr rvu + " preserve PLI: " + f + "(" + sfold xToStr args + ")")
    let (bb, cc, call0) = rtl_pli_or_leafcell ww (fv1, rvu, p', aliases) ((f, gis), order) gl_cens (bb, cc) args args'
    let call = if nonep call0 then None else Some(gl_cens, V_EASC(valOf call0), []) // Here is where gl guarding occurs. xref-whereguard
    (bb, cc, call)


let rec get_extra_p site pp =
    let rr = (!pp.m_units, !pp.m_nets)
    (pp.m_units := []; pp.m_nets := []) 
    vprintln 3 (sprintf "%s: %i extra nets, %i extra bev/gate/FU units." site (length (snd rr)) (length (fst rr)))
    rr

let exec_flatten_gbuild_vgen ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens arg (g0, n0, bb0, cc0) = // TODO not called from SP_fsm route?
    let _:(string * hexp_t list) list = n0
    let (reset_expr, is_asynch, reset_nets) = // Repeated reset code. Does not handle mixed synch and asynch resets so far, but could be added.
         match pp.dir.resets with
             | [] -> (X_false, true, [])
             | (true,  is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pp.dir.resets), is_asynch, map f3o3 pp.dir.resets)
             | (false, is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pp.dir.resets), is_asynch, map f3o3 pp.dir.resets) 

    //let use_resetsf = fv1.prefs.reset_mode="synchronous" || fv1.prefs.reset_mode="asynchronous"
    let sdir = valOf_or_fail "L121" pp.sdir
    match arg with
        | XIRTL_buf(ga, l, r) -> // SP_rtl:
            let _ = pand_vgen_buf ww (fv1, pep, pp, aliases) rvu "XIRTL_buf" (ga, l, r)
            (g0, n0, bb0, cc0)
        | XRTL(pps, ga, lst) ->  // gbuild SP_rtl:
            let q arg cc =
                match arg with
                // Note: FSM code uses find_blka instead. We should use the same techniques in either case.
                | Rarc(gb, X_x(l, -1, _), r) ->
                    // Lmode X_x here converts sequential to combinational logic.
                    let rvu = new_rvu "XRTL" rvu // New region for each component.
                    //vprintln 0 (sprintf " gbuild_vgen buffer assign to %s" (xToStr l))
                    note_is_combreg regslist "L2000" l
                    let _ = pand_vgen_buf ww (fv1, pep, pp, aliases) rvu "gbuild_vgen XRTL x^-1" (xi_ggen tieoffs pps (ix_and ga gb), l, xi_rewrite tieoffs r)
                    cc

                | Rarc(gb, l0, rr) ->
                    let rvu = new_rvu "XRTL" rvu // New region for each component.
                    let (l, buffer) = find_blka l0                
                    if buffer then note_is_combreg regslist "L2007" l
                    let ww' = WN ("XIRTL_arc " + xToStr l) ww
                    let g9 = xi_ggen tieoffs pps (ix_and ga gb) // TODO <----------- reset in here too please
                    let _ =
                        if buffer then pand_vgen_buf ww' (fv1, pep, pp, aliases) rvu "gbuild_vgen XRTL 0" (g9, l, rr)
                        else
                            let rr =
                                if not(xi_isfalse reset_expr) then
                                    ix_query reset_expr (xi_resetval l) rr
                                else rr
                            let _ = pand_vgen_synch ww' (fv1, pep, rvu, pp, aliases) (g9, l, xi_rewrite tieoffs rr)
                            // with sfault(s) -> sf(s + " pandex rethrow for " + xToStr l + "<=" + xToStr rr) 
                            ()
                    cc
                | Rpli(gb, fgis, args) when fv1.prefs.keep_pli ->
                    let (bb_, cc_, cmd) = exec_flatten_pli ww (fv1, pp, tieoffs, aliases) None ([], []) cens fgis args  // gbuild site. TODO: Reset not in here?  
                    //let (bb_, cc_, cmd) = kbev_pli None ([], []) (pp, ix_and ga gb, fgis, args)  // gbuild site. TODO: Reset not in here?  
                    if nonep cmd then cc else (valOf cmd) :: cc // bb_ and cc_ are discarded TODO - they need to be iteratively further gbuilt.
                | _ -> cc
            let cc' = List.foldBack q lst cc0
            let (gates, nets) = get_extra_p "exec_flatten_gbuild" pp
            vprintln 1 (mm + ": SP_rtl exec complete  threads/gates=" + i2s(length gates))
            (gates@g0, ("gbuild", nets)::n0, bb0, cc')
        | _ -> muddy "bguild case non rtl arc"


let exec_flatten_kbev_rtl ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens = function  // generic
    | XRTL(pps, ga, lst) ->
        let sq gg = if xi_istrue gg then [] else [gg]
        let qq arg (bb, cc) =
            let rvu = new_rvu "KBEV_XRTL" rvu // New layout region for each component.
            let glf gb = cens @ sq ga @ sq gb @ (if nonep pps then [] else [ xi_deqd(fst(valOf pps), snd(valOf pps))])
              
            match arg with
            | Rpli(g1, ((f, gis), so), args) when fv1.prefs.keep_pli ->
                  let (bb, cc, n) = exec_flatten_pli ww (fv1, pp, tieoffs, aliases) None (bb, cc) (glf g1 @ cens) ((f, gis), so) args
                  //let (bb, cc, n) = kbev_pli None (bb, cc) (pp, ix_and ga g1, ((f, gis), so), args)                  
                  (bb, (if nonep n then cc else (valOf n)::cc))

            | Rarc(gb, l0, rr) -> // generic  - no reset code mandrake.   There is at least 3 copies of this code in this file - we need to abstract to one that normalises X_x forms all to the lhs and then applies find_blka.  Recent edits have moved us closer to this being an easy rewrite.
                let mm = "exec_flatten_kbev_rtl-Rarc" 
                let rr = xi_rewrite tieoffs rr
                //vprintln 0 (sprintf "arc12/2 rhs=%A" (xToStr rr))
                let (l0, buffer) = find_blka l0
                if buffer then note_is_combreg regslist "L2051" l0
                let prec = mine_prec g_bounda l0
                let l' = eqnToL fv1 ww (rvu, pp, aliases) prec l0
                let assign_pairs =
                    match rr with  // EASC was historically here matched - a better cvt to PLI convention would help. Now done.
                        //| W_apply((f, gis), args, _) when memberp f g_rtl_finessed_pli -> rtl_finesse_pli ww (f, gis) args
                        | _ -> 
                            let w = valOf_or prec.widtho -1
                            [(w, l0, l', eqnToL fv1 ww (rvu, pp, aliases) prec rr)]
                List.foldBack (debib pp buffer (glf gb) (Some l0)) assign_pairs (bb, cc) 
            | _ -> (bb, cc) // buffers and xtrop  versus (bufferp, v) - seq 3 suitable for if-share?
        List.foldBack qq lst ([], [])
    | _ -> sf ("other kbev_rtl1")



let exec_flatten_kbev_v0 ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens k0 arg (ic, nc, bb0, bevc0) =  // generic/SP_rtl: convert to Verilog RTL or gates. 
    let vd = fv1.vd
        // ic     - gate instances
        // nc     - net groups
        // bb     - buffers            - (g, bib, reset) list
        // bevc   - sequential, aka cc - (g, bib, reset) list
    let _ = WF 2 "kbev_0" ww "start"
    if vd>=5 then vprintln 5 ("   start rtl/gate build for " + xrtlToStr arg)
    let ans = 
            match arg with
                | XRTL _ -> // send to generic handler
                    if not_nonep fv1.prefs.gatelib then List.foldBack (exec_flatten_gbuild_vgen ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens) [arg] (ic, nc, bb0, bevc0) 
                    else
                        let (bb, seq) = exec_flatten_kbev_rtl ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens arg
                        //vprintln 3 ("middle "  + sfold verilog_render_bev (map f2o3 ans) + "\nendmiddle\n")
                        (ic, nc, bb@bb0, seq @ bevc0)

                | XIRTL_pli(pps, g0, (f, gis), args) -> //send to kbev_pli which is generic 
                    let sq gg =
                        let gg = xi_brewrite tieoffs gg
                        if xi_istrue gg then [] else [gg]
                    let gl = sq g0 @ (if nonep pps then [] else [ xi_deqd(fst(valOf pps), snd(valOf pps))])
                    let (bb, cc, call) = exec_flatten_pli ww (fv1, pp, tieoffs, aliases) None (bb0, bevc0) (gl @ cens) (f, gis) args
                    ///let (bb, cc, call) = kbev_pli None (bb0, bevc0) (pp, g0, (f, gis), args)                    
                    (ic, nc, bb, (if nonep call then cc else (valOf call)::cc))

                | XIRTL_buf(X_true, l, r) -> // generic buffer
                    if not_nonep fv1.prefs.gatelib then List.foldBack (exec_flatten_gbuild_vgen  ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens) [arg] (ic, nc, bb0, bevc0)
                    else
                        let m = "kbev_v0_buf1"
                        let rvu = new_rvu m rvu
                        let cp = constantp r
                        let prec = mine_prec g_bounda l
                        let l' = eqnToL fv1 ww (rvu, pp, aliases) prec l
                        //vprintln 0 (sprintf "%s: w=%i rtl buf to V " m w + xrtlToStr arg)
                        let r' = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs r)
                        let n = gec_V_CONT fv1 cp (l', r') // Generates a cont instead of an entry on nc?
                        (n :: ic, nc, bb0, bevc0)

                | XIRTL_buf(g, l, r) -> // generic. Make continuous assignments (or always_comb) from buffers.
                    if not_nonep fv1.prefs.gatelib then List.foldBack (exec_flatten_gbuild_vgen ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens) [arg] (ic, nc, bb0, bevc0)
                    else
                        let m = "kbev_v0-lhs-buf"
                        let rvu = new_rvu m rvu
                        let prec = mine_prec g_bounda l
                        let l' = eqnToL fv1 ww (rvu, pp, aliases) prec l
                        //vprintln 0 (sprintf "%s: w=%i rtl buf to V " m w + xrtlToStr arg)
                        let r' = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs r) (* TODO : why some local variants of jval ? *)
                        let jbval r = beqnToL fv1 ww (rvu, pp, aliases) (k0()) (xi_brewrite tieoffs r)
                        let cp = constantp r && bconstantp g
                        let rhs = V_QUERY(jbval g, r', l')
                        let n = gec_V_CONT fv1 cp (l', rhs)
                        (n::ic, nc, bb0, bevc0)
                | XRTL_nop s ->
                    let n = (V_SCOMMENT s, ref None) 
                    (n::ic, nc, bb0, bevc0)
    let _ = WF 2 "kbev_0" ww "end"
    ans
        
//
//
//           
let rec exec_flatten ww0 (mm, fv1, control:control_t, aliases, tieoffs, regslist) (pep, pp) rvu work = 
    let vd = fv1.vd
    let ww = WF 3 "exec_flatten" ww0 ("Pt0 for DIR=" + pp.dir.duid)
    let sdir = valOf_or_fail "L2081" pp.sdir

    let (reset_expr, is_asynch, reset_nets) = // Repeated reset code. Does not handle mixed synch and asynch resets so far, but could be added.
         match pp.dir.resets with
             | [] -> (X_false, true, [])
             | (true,  is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pp.dir.resets), is_asynch, map f3o3 pp.dir.resets)
             | (false, is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pp.dir.resets), is_asynch, map f3o3 pp.dir.resets) 

//    let (is_pos, is_asynch, _) = reset_info
//    let use_resetf = not_nullp pp.dir.resets && (fv1.prefs.reset_mode="synchronous" || fv1.prefs.reset_mode="asynchronous")
    let reset_exp =
        if nullp pp.dir.resets then None
        else
            let reset_exp = ix_orl(map greset pp.dir.resets)
            mutaddonce pp.m_resetgrds reset_exp
            Some reset_exp


    let k0() = new_known_dp [] // TODO - sort me out! Should not need keep making? hprpin is broken?

    let cens = sdir.domain_cen
    
    let domain_clken_guard body =
        if nullp cens then body
        else
            let dp = new_known_dp []
            let cen = beqnToL fv1 (WN "domain_cen" ww) (rvu, pp, aliases) dp (ix_andl cens) // These are conjuncted.
            gen_V_IF(cen, body)

    let pas = (rvu, pp, aliases)

    let (directorate_nets, dir_rcode, dir_bb, dir_cc) =
        match pp.built_directors.lookup pp.dir.duid with
            | None ->
                let (dir_rcode, dir_bb, dir_cc) = gbuild_director ww (debib, beqnToL fv1, eqnToL fv1, pas) pp // sets up initial (bb,cc)
                pp.built_directors.add pp.dir.duid ([], [])

                let directorate_nets = get_directorate_nets pp.dir
                //dev_println (sprintf "exec_flatten: directorate nets are " + sfold netToStr directorate_nets + " for " + dirToStr false pp.dir)
                let directorate_nets = directorate_nets @ get_sdirectorate_extra_nets sdir
                vprintln 3 (sprintf "exec_flatten: pe=%s directorate_nets=" pp.name + sfold netToStr directorate_nets)
                (directorate_nets, dir_rcode, dir_bb, dir_cc)
            | Some (_, _) ->     // Although we do not consult what was stored, this ensures we create the director nets once for each distinct director.
                ([], [], [], []) // We want to include these only once please
    //dev_println(sprintf "ROSIE: dir_rcode=%A" dir_rcode)
                    
    let down_convert_rtl ww xrtl (instances, nets, bb_cc) = 
        let vd = fv1.vd
        let gate_collatef_ = fv1.prefs.gatelib<>None //Need different form of collated form for gates : exactly one assign to each lhs. 
        vprintln vd (mm + ": SP_rtl convert to Verilog: preserve-sequencer=" + boolToStr fv1.preserve_sequencer + ", gatelib=" + (valOf_or fv1.prefs.gatelib "None Specified") + " xrtl arcs=" + i2s(length xrtl) + "\n")
        let xrtl_sorted = ba_sort ww fv1 regslist (rtl_once xrtl)
        let (instances, more_nets, bib_comb, bib_seq) = List.foldBack (exec_flatten_kbev_v0 ww (fv1, pep, pp, aliases, tieoffs, regslist) rvu mm cens k0) xrtl_sorted (instances, [], [], [])  // Generic invoke
        let p' = { pp with prefs={pp.prefs with gatelib=None}}
        //vprintln 3 ("pregot "  + sfold (fun (_, x, _) -> verilog_render_bev x) bev)
        let comb_body = new_ifshare_v ww control (fv1, rvu, p', aliases) [] bib_comb
        let seq_body = new_ifshare_v ww control (fv1, rvu, p', aliases) cens bib_seq    
#if SPARE_DEBUG
        let _ = 
            dev_println ("exec_flaggen: yield ")
            app (fun ss -> dev_println ("   bev item1 " + sfoldcr_lite id ss)) (map verilog_render_bev seq_body)
            dev_println ("exec_flaggen: comb yield ")
            app (fun ss -> dev_println ("   comb item1 " + sfoldcr_lite id ss)) (map verilog_render_bev comb_body)
#endif 

        let ww = WF 3 "down_convert" ww0 "Pt3"
        let (comb_body, seq_body) =
            if not(xi_isfalse reset_expr) then
                let rvu = new_rvu "body1_reset" rvu
                let jbval r = beqnToL fv1 ww (rvu, pp, aliases) (k0()) (xi_brewrite tieoffs r)
                let rst_code =
                    let dig_reset (g, cmd, rst) cc =
                        //dev_println (sprintf "verilog_gen: ROSIE gec reset, Site B, for %A rst=%A"  (cmd) rst)
                        lst_union rst cc 
                    List.foldBack dig_reset bib_seq []
                let seq = gen_V_IFE(jbval(valOf_or_fail "L2154-reset" reset_exp), gen_V_BLOCK rst_code, gen_V_BLOCK seq_body)
                (comb_body, [ seq ])
            else (comb_body, seq_body)
        let _ =
            if false then
                //app (fun (cmd)->vprintln 0 ("Ordering :" + verilog_render_bev cmd)) (seq_body)
                //vprintln 0 "Was ordering for SP_rtl block\n"
                ()

        let ans = (instances, more_nets @ nets, (comb_body @ fst bb_cc, seq_body @ snd bb_cc))
        let ww = WF 3 "down_convert" ww0 "Pt-exit"
        ans
        
    let eg2 (b, a) (t, f) = if b then (a::t, f) else (t, a::f)
    let partition_eg4 (b, a1, a2, a3) (t, f) = if b then ((a1,a2,a3)::t, f) else (t, (a1,a2,a3)::f) 

    let rec has_fsm_pred = function // can go in gbuild/abstracte
        | SP_seq lst 
        | SP_par(_, lst) -> disjunctionate has_fsm_pred lst
        | SP_fsm _ -> true
        | _        -> false
    let has_fsm = has_fsm_pred work
    
    let handshake_liaison_sans_fsm msg (ic, nets, bb_cc) =  // This code can/should be lifted and then go in gbuild.fs
        if has_fsm then (ic, nets, bb_cc)
        else
            let dc = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec 
            vprintln 3 (sprintf "Exec has no FSM. Tieoff idle/ending nets using handshake_liaison_sans_fsm for %s" msg)
            match sdir.handshake_links with
            | None -> (ic, nets, bb_cc)
            | Some (fsm_idle_link, fsm_ending_link, req, ack, reqrdy_o, ackrdy_o) ->
                let (fsm_idle, fsm_ending) = (xi_one, xi_one) // Set both to one when there is no controlling FSM.
                let fsm_wireup (ic, nets, (bb, cc)) (lhs, rhs) =
                    let nb = V_BA(dc lhs, dc rhs) // Blocking assign makes combinational logic in always_comb.
                    (ic, nets, (nb::bb, cc))
                List.fold fsm_wireup (ic, nets, bb_cc) [ (fsm_idle_link, fsm_idle); (fsm_ending_link,  fsm_ending) ]

    let rst =
        if xi_isfalse reset_expr then None
        else Some(beqnToL fv1 ww (rvu, pp, aliases) (k0()) reset_expr)

    let rec down_convert xr (ic, nets, bb_cc) =
        match xr with
        | SP_rtl(ii, xrtl) -> // We have duplication of code here between SP_rtl and SP_fsm, both contain XIRTL_pli.
            let (ic, nets, bb_cc) = handshake_liaison_sans_fsm ("SP_rtl " + ii.id) (ic, nets, bb_cc)

            down_convert_rtl ww xrtl (ic, nets, bb_cc) // Only want to do this once please

        | SP_par(_, lst) ->
            let ii = { id= "from_SP_par" }:rtl_ctrl_t
            let rec doit_fold aa (ic, nets, bb_cc) =
                 match aa with 
                 | [] -> (ic, nets, bb_cc)
                 | SP_rtl(ii, k)::SP_rtl(ii_, m)::tt ->  doit_fold (SP_rtl(ii, k@m)::tt)  (ic, nets, bb_cc) // One ii ignored!
                 | hh::tt ->
                     vprintln 3 (sprintf "doit_fold par invoke")
                     let (ic, nets, bb_cc) = down_convert hh (ic, nets, bb_cc)
                     doit_fold tt (ic, nets, bb_cc)
            let ans = doit_fold lst (ic, nets, bb_cc)
            ans

        | SP_fsm(ii, edges) ->  // Contains XIRTL_pli
            let kvd = 3
            if not_nonep fv1.prefs.gatelib then muddy("sequencer FSM form requested to be compiled to gates (need to add compose to recipe or complete this gappy code?)")
            let gate_collatef_ = not_nonep fv1.prefs.gatelib //Need different form of collated form for gates : exactly one assign to each lhs. 
            let msg = ("commence:" + mm + sprintf ": preserve-sequencer=%A  controllerf=%A" fv1.preserve_sequencer ii.controllerf + sprintf ", states=%i, edges=%i" (length ii.resumes) (length edges))
            let ww = WF 2 "verilog_gen: FSM to Verilog conversion" ww msg
            let _ = cassert(nonep fv1.prefs.gatelib, "FSM form present: gatelib given, but not collated.\n")

            if nullp edges then
                 let ss = (sprintf "Discard empty FSM '%s'"  (xToStr ii.pc))
                 vprintln 2 ss
                 // yield a comment here in the output to show where the FSM was
                 let n = (true, V_COMMENT ss)
                 (ic, nets, bb_cc)
            else
            let k0() = new_known_dp []
             // Switcher form: present output looking like an FSM with a switch/case statement.
             // The switcher form has a reset outside the match statement. Without switcher we put reset code in the arcs.


            let (ic, nets, bb_cc) = 
                let (fsm_idle, fsm_ending) = (xi_blift(fsm_idle_pred msg xr), xi_blift(fsm_stopping_pred msg xr))
                //dev_println(sprintf "FSM liaison predicates idle=%s ending=%s" (xToStr fsm_idle) (xToStr fsm_ending))
                match sdir.handshake_links with // TODO this needs copying to SystemC output and mirroring in diosim.
                    | None -> (ic, nets, bb_cc)
                    | Some (fsm_idle_link, fsm_ending_link, req, ack, reqrdy_o, ackrdy_o) ->

                        let fsm_wireup (ic, nets, (bb, cc)) (lhs, rhs) =
                            let dc = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec 
                            let nb = V_BA(dc lhs, dc rhs) 
                            (ic, nets, (nb::bb, cc))
                        List.fold fsm_wireup (ic, nets, bb_cc) [ (fsm_idle_link, fsm_idle); (fsm_ending_link,  fsm_ending) ]

            let switcher = ii.controllerf && fv1.preserve_sequencer
            //dev_println (msg + sprintf ": verilog_gen: FSM: switcher=%A  ii.controllerf=%A  fv1.preserve_sequencer=%A" switcher ii.controllerf fv1.preserve_sequencer)

            let switcher =
                if switcher then switcher
                else
                    if length edges > 1 then
                         vprintln 1 (sprintf "verilog_gen: override no switcher request since have multiple edges: we should better invoke FSM flatten fsm_flatten_to_rtl...")
                         true
                    else false

            let edge_actions_collate garc arg (rcode, bb, cc, cmds, pli) = // This needs to be called by foldBack to avoid reversing side-effecing PLI invokation order.
                 // The cmds for a state are collated so they can be given the 'if-share' treatment.
                 // Apart from converting to RTL, operations include rewrite, delete goto, resetval extraction, fsm preservation, if-shareing, ...
                 // but gate rendering is lost
                 // (bb,cc) provides compatibility with generic routines and we want to move the other forms to use these please. ie. down_convert_rtl should be commoned up TODO 
                 // rcode is BA/NBA list
                 // cmds is (bufferp, vbev stmt) list
                 // pli is (bool, xb guard, V_EASC, reset 'a list)
                let sq gg =
                    let gg = xi_brewrite tieoffs gg
                    if xi_istrue gg then [] else [gg]

                let ans = 
                    match arg with
                        | XIRTL_pli(pp__, gg, ((f, gis), so), args)  ->  // fsm copy - can safely ignore pp but we include garc instead.
                            //dev_println(sprintf "XYz XIRTL_pli clause")
                            let rvu = new_rvu "SP_fsm_pli" rvu
                            let jvalx prec r = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs r)
                            let _ = lprintln kvd (fun()->"edge_actions_collate: xirtl pli call guard is " + xbToStr gg)
                            let gl = garc :: sq gg
                            let (bb, cc, call0) = exec_flatten_pli ww  (fv1, pp, tieoffs, aliases) None (bb, cc) (cens@gl) ((f, gis), so) args
                            let call_old = None
                            (rcode, bb, (if nonep call0 then cc else (valOf call0)::cc), cmds, (if nonep call_old then pli else (valOf call_old)::pli))

                        | XRTL(pp__, ga, lst) -> // FSM state contents:  // fsm copy - can safely ignore pp but use it as a tag for identifying gotos.
                            //dev_println(sprintf "XYz XRTL clause")
                            let qqf cde (rcode, bb, cc, cmds, pli) =
                                let rvu = new_rvu "SP_fsm" rvu
                                let jvalx prec r = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs r) // tail call
                                match cde with
                                | Rpli(gb, ((f, gis), so), args) when fv1.prefs.keep_pli ->
                                     //if kvd>=5 then vprintln 5 ("edge_actions_collate: xirtl pli call guard is " + xbToStr g)
                                     //dev_println(sprintf "XYz XRTL Rpli clause")
                                     let gl = garc :: sq ga @ sq gb
                                     let (bb, cc, call0) = exec_flatten_pli ww (fv1, pp, tieoffs, aliases) None (bb, cc) (cens@gl) ((f, gis), so) args
                                     let call_old = None // = (jbvaln g, V_EASC(V_CALL((plimap f, gis), map jvalx args)), [])
                                     (rcode, bb, (if nonep call0 then cc else (valOf call0)::cc), cmds, (if nonep call_old then pli else (valOf call_old)::pli))


                                | Rarc (gb, l0, v) ->   // FSM state contents:  -> please send to generic code soon - but the generic has no debib
                                     //dev_println(sprintf "XYz XRTL Rarc clause %s  switcher=%A" (xToStr l0) switcher)
                                     if (ii.controllerf=false && l0=ii.pc) then (rcode, bb, cc, cmds, pli) // Delete goto transitions when no controller
                                     elif (l0=ii.pc && not_nonep pp__ && snd(valOf pp__) = v) then (rcode, bb, cc, cmds, pli) //Delete arcs to current state as they are implied by NOR of exits.
                                     else
                                         //vprintln 0 (sprintf "arc 2/2 rhs=%A" v)
                                         let (l, buffer) = find_blka l0 // Should perhaps xfer X_x on rhs with negated polarity to the lhs first... and in general.
                                         if buffer then note_is_combreg regslist "L2356" l
                                         let prec = mine_prec true l
                                         let is_combreg =
                                             match (regslist:regslist_t).lookup (xToStr l) with
                                                 | Some entry -> entry.is_combreg
                                                 | _ -> false
                                        //dev_println (sprintf "ROSIE lhs=%s resetval=%A entry=%A" (netToStr l) (xi_resetval l) is_combreg)
                                         let l' = eqnToL fv1 ww (rvu, pp, aliases) prec l
                                         let r' = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs v)
                                         let rst_code =
                                             let width = valOf_or prec.widtho  -1
                                             debib_rez buffer (l', V_NUM(width, false, "d", xi_resetval l))
                                         let gg = ix_andl [garc; ga; gb]
                                         if switcher then
                                             let mcode = (buffer, garc :: sq ga @ sq gb, debib_rez buffer (l', r'), [rst_code]) // No if-share this branch
                                             let rcode =
                                                 if is_combreg then rcode else singly_add rst_code rcode
                                             (rcode, bb, cc, cmds, mcode::pli)   // Add to pli (misnomer) when sequencer-style render
                                         else
                                             let mcode = (garc :: sq ga @ sq gb, debib_rez buffer (l', r'), (if is_combreg then [] else [rst_code]))
                                             let stmt =
                                                 let cmd = gen_V_BLOCK(new_ifshare_v ww control (fv1, rvu, pp, aliases) cens [mcode])
                                                 if is_combreg then cmd
                                                 else 
                                                     if nonep rst then sf "L2300-rst"
                                                     else gen_V_IFE(valOf rst, rst_code, cmd)
                                             (rcode, bb, cc, (buffer, stmt)::cmds, pli) // Add to cmds if not sequencer-style render
                                | _ ->
                                     //dev_println(sprintf "XYz XRTL other/nop clause")
                                     (rcode, bb, cc, cmds, pli)
                            List.foldBack qqf lst (rcode, bb, cc, cmds, pli)

                        | XIRTL_buf(grd, lhs, rhs) ->
                            //dev_println(sprintf "XYz XRTL_buf clause")
                            //We could write it out as hoisted blocking assignment if only used locally but would need to factorise the guard. And for most generality should check for X_x on either side first and normalise, xferring all to the lhs and then invoking find_blka.
                            let prec = mine_prec true lhs
                            let l' = eqnToL fv1 ww (rvu, pp, aliases) prec lhs
                            let w = valOf_or prec.widtho  -1
                            let r' = eqnToL fv1 ww (rvu, pp, aliases) prec (xi_rewrite tieoffs rhs)
                            let buffer = true
                            let mcode = (buffer, garc :: sq grd, debib_rez buffer (l', r'), [])
                            (rcode, bb, cc, cmds,  mcode::pli) // pli is a misnomer here since the buffers are added in!

                        | XRTL_nop s ->
                            //dev_println(sprintf "XYz XRTL_nop clause")
                            let n = (true, V_COMMENT s)
                            (rcode, bb, cc, n::cmds, pli)
                        //| other -> (rcode, bb, cc, cmds, pli)
                ans
            let (fsm_comb, fsm_seq) = 
                         let iresumes = map (fun (res, hf) -> (res, lc_atoi32 res)) ii.resumes
                         if switcher then // Factorise as an FSM again.
                             let (resets, comb_clauses, seq_clauses) =
                                 let fsm_state_to_vnl (resets0, comb_clauses0, seq_clauses0) (resume, iresume) =
                                     let fsm_e1_to_vnl edge (resets, bb, cc, comb_clauses, seq_clauses) =
                                         if lc_atoi32 edge.resume = iresume then
                                             let goto = XRTL(Some(edge.pc, edge.resume), edge.gtop, [Rarc(X_true, edge.pc, edge.dest)])
                                             let work0 = rtl_once(goto :: edge.cmds)  
                                             let (resets, bb, cc, comb_clauses, seq_clauses) = List.foldBack (edge_actions_collate edge.gtop) work0 (resets, bb, cc, comb_clauses, seq_clauses) 
                                             vprintln fv1.vd ("Mid a state 1 " + xToStr resume)
                                             (resets, bb, cc, comb_clauses, seq_clauses) 
                                         else (resets, bb, cc, comb_clauses, seq_clauses)
                                     let (resets1, bb1, cc1, comb_clauses1, seq_clauses1) = List.foldBack fsm_e1_to_vnl edges ([], [], [], [], [])
                                     let ppf = eqnToL fv1 ww (rvu, pp, aliases) g_bool_prec resume
                                     let (bb, seq) = List.foldBack eg2 comb_clauses1 ([], [])
                                     let (pli_bb, pli_seq) = List.foldBack partition_eg4 seq_clauses1 ([], [])
             // fsm bb:   v_bev_t list    
             // bb1 and pli_bb:    (hbexp_t list * v_bev_t * 'a list) list    
             // pli_seq && cc1:    (hbexp_t list * v_bev_t * v_bev_t list) list
                                     let pli_seq = pli_seq @ cc1
                                     let pli_bb = pli_bb @ bb1
                                     let a20 = if bb <> [] then old_v_ifshare "bb" bb else []
                                     let a21 = new_ifshare_v ww control (fv1, rvu, pp, aliases) [] ((*once_cmd*) pli_bb) // perhaps use one if share for all a2 ?
                                     //Let's try to use the new one ...
                                     //let a20_ = new_ifshare_v ww control (fv1, rvu, pp, aliases) ((*once_cmd*) bb)
                                     let a2 = a20 @ a21
                                     let a30 = if seq <> [] then old_v_ifshare "seq" seq else []
                                     let a31 = new_ifshare_v ww control (fv1, rvu, pp, aliases) cens ((*once_cmd*)pli_seq) // use one if share for all a3 ?
                                     let a3 = a30 @ a31
                                     let comb = if a2=[] then [] else [([ppf], gen_V_BLOCK(a2))]
                                     let seq =  if a3=[] then [] else [([ppf], gen_V_BLOCK(a3))]
                                     //muddy (m0 + "([eqnToL fv1 ww (rvu, pp, aliases) s], gen_V_BLOCK(v_ifshare m' @ new_ifshare_v ww control (p) reset_exp (once_cmd sh))) ")
                                     vprintln fv1.vd ("End a state " + xToStr resume)
                                     //dev_println(sprintf "ROSIE: 2452 rcode0=%A" resets0)
                                     //dev_println(sprintf "ROSIE: 2452 rcode1=%A" resets1)

                                     (lst_union resets0 resets1, comb @ comb_clauses0, seq @ seq_clauses0)
                                 List.fold (fsm_state_to_vnl) ([], [], []) iresumes 
                             let pc_prec = { widtho=Some(bound_log2_arity(length seq_clauses)); signed=Unsigned; }
                             let pc' = eqnToL fv1 ww (rvu, pp, aliases) pc_prec ii.pc
                             let case_flags = { full_case=false; parallel_case=false; }
                             let comb_code = if nullp comb_clauses then [] else [ V_SWITCH(pc', comb_clauses, case_flags) ]
                             let main_code = domain_clken_guard(V_SWITCH(pc', seq_clauses, case_flags))
                             // Where is the pc set to the entry point (0) ? This is comment out in the next line 
                             let rcode = resets (* V_NBA(pc', V_NUM (resetval pc)) *) (* only one ! *) 
                             //dev_println(sprintf "ROSIE: 2452 rcode=%A" rcode)
                             vprintln 3 (sprintf "Verilog_gen:  FSM.  pc=%s resets^=%i states=^%i" (xToStr ii.pc) (length resets) (length seq_clauses))
                             let seq_code = [ (if not(xi_isfalse reset_expr) then gen_V_IFE(valOf_or_fail "L2372-reset" rst, gen_V_BLOCK rcode, main_code) else main_code) ]
                             (comb_code, seq_code)
                         else
                             match length edges with
                                | 1 ->
                                    let soloarc = hd edges
                                    let (reset_code, bb1, cc1, mm, sh) = List.foldBack (edge_actions_collate soloarc.gtop) soloarc.cmds ([], [], [], [], []) 
                                    let (bb_, seq_) = List.foldBack eg2  mm ([], []) // spare line
                                    let (pli_bb_, pli_seq_) = List.foldBack partition_eg4 sh ([], []) // delete this spare line please
                                    let pli_bb = pli_bb_ @ bb1
                                    let pli_seq = pli_seq_ @ cc1                       
                                    let m20 = new_ifshare_v ww control (fv1, rvu, pp, aliases) [] (once_cmd pli_bb)
                                    let m1 = new_ifshare_v ww control (fv1, rvu, pp, aliases) cens (once_cmd pli_seq)
                                    //dev_println(sprintf "ROSIE: 2385 dir_rcode=%A" reset_code)
                                    let seq = if not(xi_isfalse reset_expr) then [ gen_V_IFE(valOf_or_fail "L2385-reset" rst, gen_V_BLOCK reset_code, domain_clken_guard(gen_V_BLOCK(m1  @ seq_))) ] else m1 @ seq_
                                    (m20 @ bb_, seq)

                                | ll ->
                                    sf(sprintf  "verilog_gen:controllerf is set yet input has %i edges (no switcher)" ll)
                
            let _ = WF 2 "FSM to Verilog conversion" ww ("finished:" + mm)
            (ic, nets, (fsm_comb @ fst bb_cc, fsm_seq @ snd bb_cc))


        | SP_comment l -> ((V_SCOMMENT l, ref None)::ic, nets, bb_cc)

        | SP_seq(l) -> sf ("verilog_gen: A sequential compostion of finite-state machines in exec_flatten does not make sense: " + hprSPSummaryToStr xr)

        | SP_l(ast_ctrl, bev) -> // Convert to SP_rtl and go round again.
             // Here we cope with some very simple bev code, but all richer forms need converting to RTL using bevelab or similar HLS stage.
             let eflt n = sf (sprintf "verilog_gen: nox=%i other form in exec_flatten SP_l cannot simply convert to RTL (perhaps use alternative recipe structure):" n + hbevToStr bev)
             let m_cs = ref []
             let rec simple_bev_compile nox g cmd =
                 match cmd with
                 | [] -> nox
                 | Xif(g1, ct, cf) :: tt ->
                     let n0 = simple_bev_compile nox (ix_and g1 g) [ct]
                     let n1 = simple_bev_compile nox (ix_and (xi_not g1) g) [cf]
                     let max2(a,b) = if a>b then a else b
                     simple_bev_compile (max2(n0, n1)) g tt; 
                 | Xcomment _ :: tt 
                 | Xskip :: tt     -> simple_bev_compile nox g tt
                 | Xblock(v) :: tt -> simple_bev_compile nox g (v @ tt)
                 // Use counter nox to ensure only one assignment in any control flow, otherwise blocking and non-blocking resolution will be needed.
                 // Xeasc clause missing ::  should be converted to PLI here
                 | Xassign(l, r)::tt ->
                     if nox=0 then (mutadd m_cs (Rarc(g, l, r)); simple_bev_compile (nox+1) g tt)
                     else
                         hpr_yikes(sprintf "Verilog_gen: More than one assignment in SP_l control flow path.")
                         eflt nox
                 | other::tt ->
                     vprintln 0 ("Verilog_gen: cannot effect a simple conversion of behavioural command (need an HLS expansion please) " + hbevToStr other)
                     eflt(nox)
             let _ = simple_bev_compile 0 X_true [bev]
             if nullp !m_cs then (ic, nets, bb_cc)
             else
                 let ii = { id=ast_ctrl.id }:rtl_ctrl_t
                 let n = SP_rtl(ii, [ XRTL(None, X_true, !m_cs) ])
                 vprintln 3 (sprintf "SP_l bev code par invoke: go round again.")
                 down_convert n (ic, nets, bb_cc)

        | _ -> sf ("verilog_gen: other form in exec_flatten cannot directly convert to RTL (perhaps use alternative recipe structure):" + hprSPSummaryToStr xr)

    let (dir_bb, dir_cc) =
        let m20 = new_ifshare_v ww control (fv1, rvu, pp, aliases) [] dir_bb
        let m1 = new_ifshare_v ww control (fv1, rvu, pp, aliases) cens dir_cc
        let seq =
            if not(xi_isfalse reset_expr) then
                let rst = beqnToL fv1 ww (rvu, pp, aliases) (k0()) reset_expr
                [ gen_V_IFE(rst, gen_V_BLOCK dir_rcode, domain_clken_guard(gen_V_BLOCK(m1))) ]
            else m1
        (m20, seq)

        
    let ans = down_convert work ([], [("directorate-nets", directorate_nets)], (dir_bb, dir_cc))
    let ww = WF 3 "exec_flatten" ww0 ("Finished for DIR=" + pp.dir.duid)    
    ans

// Convert really-simple assertions to xrtl.  This should not be done here - should be in another receipe stage so output is shared in other forms!
let g2_assertion ww fv1 rvu pe1 aliases = function
    | O_ctl_AG(O_state(v, clkinfo), m::t) -> 
        let sdir = valOf_or_fail "L9282" pe1.sdir
        let cf = g_null_callers_flags
        let m0 = "O_state"
        let ck = match clkinfo with
                                  | Some(E_pos x)
                                  | Some(E_neg x) -> valOf clkinfo
                                  | Some other -> sf("not one clock net for "  + m0 +  ":" + edgeToStr other)
                                  | None -> sf("no clock net for "  + m0)
        let ck1 = eqnToL fv1 (WN m0 ww) (rvu, pe1, aliases) g_bool_prec (de_edge ck)
        let clker = [ V_EVC(v_evc_gen1 ww (fv1, rvu, pe1, aliases) ck ck1) ]
        let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[] }
        let pe8 = { pe1 with sdir=Some{ sdir with clknet=Some ck}; token="cvt_hhh"; } // A pe refine site
        let k0() = new_known_dp []
        let ans = V_ALWAYS(gen_V_BLOCK(clker @ [gen_V_IF(beqnToL fv1 (WN "g2 assertion: always" ww) (rvu, pe8, aliases) (k0()) (xi_not(xgen_orred v)), V_EASC(V_CALL(cf, (("$display", gis), None),[ vgen_STRING m ])))]))
        (ans, ref None) // no layout yet.

    | (_) -> sf("assertion cannot be compiled to monitoring machine")



// Simple printout for debug assistance sometimes.
let rec debug_vdbToStr arg cc =

    let simpleToStr = function
        | V_NET(ff, id, nn_) -> if id <> ff.id then sprintf "  aliased %s -> %s" (netToStr (X_bnet ff)) id else netToStr(X_bnet ff)
        | other -> sprintf "otherother%A" other

    match arg with
        | VDB_group(mm, lst)   -> mm.kind + "//" + List.foldBack debug_vdbToStr lst cc
        | VDB_formal v   
        | VDB_actual(None, v)    -> simpleToStr v + cc
        | VDB_actual(Some formal, v) -> sprintf ".%s(%s)" formal (simpleToStr v) + cc        


// Delete me
let refactor_old_rtl_pram_annotations__ ww decls = 
    // Convert attribute-marked overrides to DB_form and also move default value of either style into formal position. Yuck. Anyway, most likely this is not needed now that pramdef code scans for init field and rtl_parameter is converted to DB_form_pramor inside kiwife.

    let treat_blank_as_none = function
        | Some  ss -> Some (xi_string ss)
        | other -> None

    let rec expose_latent_rtl_parameters porf arg cc = // Nets that are flagged to become parameters in the RTL form - must not be assigned of course. Should be marked now with DB_form_pramor
        let is_rtl_param = function
            | DB_leaf(Some(X_bnet ff), _) -> at_assoc "rtl_parameter" (lookup_net2 ff.n).ats 
            | _ ->  None

        match arg with
          | DB_group(meta, lst) ->
              let porf = porf || meta.form=DB_form_pramor
              let (exposed, remaining) = groom2 (is_rtl_param>>not_nonep) lst
              let exposed =
                  let insert_vale arg =
                      match arg with
                          | DB_leaf(_, Some(X_bnet ff)) -> DB_leaf(treat_blank_as_none(at_assoc "init" (lookup_net2 ff.n).ats), Some(X_bnet ff)) // For a parameter, we use the formal field as a constant string initialiser.  Perhaps make it a pair option?
                          | _ -> arg
                      
                  map insert_vale exposed 

              let remaining = List.foldBack (expose_latent_rtl_parameters porf) remaining []
              vprintln 3 (sprintf "refactor_remaining_rtl %i %i" (length exposed) (length remaining))
              let nv = if nullp exposed then [] else [DB_group({meta with form=DB_form_pramor }, exposed)]
              //dev_println (sprintf "scan RTL args %A" nv)
              let ov = if nullp remaining then [] else [ DB_group(meta, remaining) ]
              nv @ ov @ cc
          | DB_leaf(pp, k) ->
              DB_leaf(pp,  k) :: cc

    let decls = List.foldBack (expose_latent_rtl_parameters false) decls []
    decls  // end of unused function.
 

let rec add_once = function // cf lst_union ?   --- delete this - it does nothing now we are using DB groups for ports.
    | ([], lx) ->
        //vprintln 0 (sprintf "add_once returns %A" l)
        lx
    | (h::t, lx) -> if memberp h lx then add_once(t, lx) else add_once(t, h::lx)


//
// Convert executable code to RTL.
//    
let run_d_exec ww fv1 pep control tieoffs regslist output_file_root pe00 rvu mm aliases m1 pe1 execs =
    let binder (dir:directorate_t) =
        let name = dir.duid
        let _ = WF 3 "run_d_exec" ww ("binder and gbuild_synth_director for " + dir.duid)
        let dir_synth = gbuild_synth_director "run_d_exec" dir // first rez
        let pe4 = { pe00 with
                      name=    name
                      dir=     dir
                      sdir=    Some dir_synth
                      // dp=DP
                  }
        pe4


    let ggg3 pe3 (gates0, nets0, (combs0, seqs0)) args =
        let rvu = new_rvu "ggg3" rvu
        let (gates, nets, combs_and_seqs) = exec_flatten ww (mm, fv1, control, aliases, tieoffs, regslist) (pep, pe3) rvu args // exec_flatten does the actual convert
        // de_edge is used to add the clocks to the clock list.
        (gates @ gates0, add_once(nets, nets0), (fst combs_and_seqs @ combs0, snd combs_and_seqs @ seqs0))


    // Top and tail the runs of the same clockinfo/directorate.
    let d_exec ww execs = 
        let collated =
            let col_pred (H2BLK(dir, _)) = dir.duid
            generic_collate col_pred execs
        
        vprintln 3 (sprintf "%i different directors for clock domain; being %s" (length collated) (sfold fst collated))
        //let elided = dir_elider0 execs []
        let aggregated =
            let caller (dir, items) = 
                let binded = binder ((fun (H2BLK(dir, work)) -> dir) (hd items))
                let items = sp_elider (map (fun (H2BLK(dir, work)) -> work) items)
                let nn = (binded, List.fold (ggg3 binded) ([], [], ([],[])) items) 
                nn
            map caller collated

        let eqtol = eqnToL fv1 (WN "eqtol" ww) (rvu, pe1, aliases) 
        let ddf arg (gates0, nets0, bevb0) =
           match arg with
               | (pe5, (gates, nets, ([], [])))  -> 
                    (add_once(gates, gates0), add_once(nets, nets0), bevb0)
               | (pe5, (gates, nets, (comba, seqa))) -> 
                    let s_starter =  V_COMMENT("Start structure " + m1)
                    let s_keystone = V_COMMENT("KEYSTONE KEYB0 ")
                    let s_ender =    V_COMMENT("End structure " + m1 + "\n\n") 
                    let combs =
                        let comb_updates = old_v_ifshare "L1811" comba // Want init_to_default before changes from non-default.
                        //dev_println (sprintf "comb_updates is %A" comb_updates)
                        let check_uncond cc = function // Nice not to init those that are straightaway unconditional assigned with an update.
                            | V_BA(V_NET(ff, _, _), V_NUM(_)) -> ff.n :: cc
                            | V_COMMENT _ 
                            | V_IF _
                            | V_IFE _
                            | V_BA(_, V_QUERY _)
                            | V_BA(_, V_DIADIC _)                            
                            | V_SWITCH _ -> cc
                            | other ->
                                //vprintln 5 (sprintf "ignore other inti/init supressor  %A" other)
                                cc
                        let unconds = List.fold check_uncond [] comb_updates
                        let inits = inti_to_default eqtol unconds comba
                        inits @ comb_updates
                    let seqs = old_v_ifshare "L1812-seqs" seqa
                    // This defaults to a fully-supported form - needed when function calls are embedded in rhs expressions.
                    let mycomb = if nullp combs then [] else [ (Some E_anystar, combs) ]
                    //Comb markup comments are getting removed from meaningful positions by sorter so now commented out.
                    //let mycomb =  if combs=[] then [] else [ ([E_anystar], [ s_starter; s_keystone ] @ combs @ [ s_ender ]) ]                    
                    let sdir = valOf_or_fail "L2595" pe5.sdir
                    let mybev = if nullp seqs then [] else [ (sdir.clknet, [ s_starter ] @ seqs @ [ s_ender ]) ]
                    vprintln 2 (sprintf "unit inventory at d_exec:  %i seq and %i cont" (length mycomb) (length mybev))
                    (add_once(gates, gates0),  add_once(nets, nets0), mycomb@mybev@bevb0)
        List.foldBack ddf aggregated ([], [], [])

    let ww = WF 2 "apply d_exec" ww (sprintf "Start on %i execs" (length execs))
    let ans = d_exec ww execs // functional return
    let _ = WF 2 "apply d_exec" ww "Finished"
    ans



// Mutable datastructures rez for an ultimate RTL module.
let gec_pe ww fv1 =
    let netinfo_dir = new netinfo_dir_t() 
    let m_units = ref []
    let m_nets = ref []
    let m_resetgrds = ref []
    let dDP = new_cvtToRtl_DP()

    let pe1:eqToL_t =
        {
            name=             "pe1"
            dir=              g_null_directorate
            sdir=             None //gbuild_synth_director "gec_pe" "pe00" g_null_directorate  // second rez
            built_directors=  new built_directors_t("built_directors")
            m_nets=           m_nets
            m_resetgrds=      m_resetgrds
            cen_factor=       true // move to prefs?
            unitsd=           new unitsd_t("unitsd")
            kogge_stone_threshold= 6 // recipe please - or delete !            
            netinfo_dir=      netinfo_dir
            token=            "cvt"            
            comb_delayo=      fv1.comb_delayo
            dp=               dDP
            prefs=            fv1.prefs  
            m_verilog_strings= ref []
            gatelevel_mux=    new gatelevel_mux_collate_t("")
            rom_inits=        new rom_inits_t("rom_inits")
            m_units=          m_units
            m_kpp_nets=       ref None
        }
    pe1



//
//
// Keep a VM2 instance as an explicit instance. (Alternatives would be to inline it or to make it an external instance).
//                   
let buztop_keep_instance ww mm pe00 fv1 rvu aliases mch = 
    match mch with
        | (ii, Some(HPR_VM2(minfo, decls, sons_, execs, assertions))) ->
          let m1 = mm + vlnvToStr minfo.name
          let (rtl_fids, rtl_prams) = List.foldBack pram_groom minfo.atts ([], [])
          let n_generics = length rtl_prams // Attributes contain default values.  OLD: Actual values are in the contacts list and need to be split off.
          vprintln 3 (m1 + sprintf ": Component has %i parameters (generics)." n_generics)
          let pe1 = pe00 // A pe refine site: This should be programmed via minfo

          // TODO - in future take these properties from ii not from the ats
          let keep = at_assoc "preserveinstance" minfo.atts // || ii.preserve_instance

          let assocf = (* old way: new way is inside DB_leaf *)  at_assoc "preserveinstance-assoc" minfo.atts <> None
          // The valOf keep used to be iname but now a separate attribute is used and the keep field indicates whether an import is needed
          //let decls = refactor_old_rtl_pram_annotations ww decls // This call now redundant since done inside cilnorm etc..
         
          let cx1 = eqnToL fv1 ww (rvu, pe1, aliases) gg_unspec_width
          let cx = gec_cx_always_actual "buzbot-instance" cx1
          //dev_println(sprintf "WEDS: instance dispose mapping? %A" decls)
          let actuals0  = cvt_flatten (Some true, 1)  cx None decls 
          let overrides = cvt_flatten (Some false, 1) cx None decls
          let f_overrides = List.foldBack (vdb_flatten) overrides []
          let n_overrides = length f_overrides

          if n_overrides <> n_generics then vprintln 3 (sprintf "Note: not all overrides supplied to %s %s   groups=%i f_overides=%i  wanted=%i" ii.iname (hptos minfo.name.kind) (length overrides) (length f_overrides) n_generics)
          let _ =
              if false then
                 let rec aToStrSerf lev_ n cc =
                     match n with 
                     | VDB_actual(_, v) -> vnetToStr v + " " + cc
                     | VDB_group(_, lst) -> List.foldBack (aToStrSerf (lev_ + 1)) lst cc
                 let aToStr x = aToStrSerf 0 x ""
                 vprintln 0 (sprintf "gdecls on son scan " + sfold (aToStr) actuals0)
                 vprintln 0 (sprintf "all decls on son scan %A" decls)
                 ()

          let actuals_assoc() =
              let already =
                  let find_existing = function
                      | (ii, Some(HPR_VM2(minfo', decls', sons', execs', assertions'))) -> minfo.name = minfo'.name && minfo.squirrelled = minfo'.squirrelled
                  List.filter find_existing [] // If we want to check for more than one instance with same iname, this is the wrong code.  Disable by passing in [].

              let pairs = 
                  match already with
                      | [] ->
                          let formals = List.foldBack (db_flatten_generic (*skip_params=*)(Some true, 1) 0 []) decls []
                          vprintln 3 (sprintf "Already rendered %s = not found" (vlnvToStr minfo.name))
                          List.zip (map (snd >> xToStr) formals) actuals0                              

                      | [ (ii, Some(HPR_VM2(minfo', decls', sons', execs', assertions'))) ] ->
                          vprintln 3 (sprintf "Already rendered %s = Yes, found- can use formals for assoc instancing." (vlnvToStr minfo.name))
                          //(sprintf "found an existing instance - must use its formal names %A " p + rdot_fold minfo.name)
                          let formals = List.foldBack (db_flatten_generic (*skip_params=*)(Some true, 1) 0 []) decls' []
                          List.zip (map (snd >> xToStr) formals) actuals0


                      | _ -> sf ("found several existing instances of " + vlnvToStr minfo.name)    
              let insert_formal = function
                  | (fname, VDB_actual(_, e)) ->
                      //dev_println (sprintf "WEDS: wrap actual Insert formal %s for actual %s" fname (vToStr e))
                      VDB_actual(Some fname, e) // Leaf field is here used for formal name (in an instance).
                  | _ -> sf "L2403"
              map insert_formal pairs
          let contacts = if assocf then actuals_assoc() else actuals0
          // termnets: The actuals passed to a son need to be declared as local nets in the Verilog version. In the past they may not have been declared
          // already and hence a final list_once is used in case they are being replicated in this step.
          let the_instance = (V_INSTANCE([], { base_cell() with name=vsanitize (hptos minfo.name.kind)}, vsanitize ii.iname, overrides, contacts, minfo.atts), ref None)

          // OLD: instances were previously combined with definitions, hence added both to the offspring and to the gates list.
          //let _ = if not externalf && nullp already then mutadd m_offspring (mch)

          vprintln 1 (sprintf "Instance preserved structurally: iname=%s,  kind=%s,  assocf=%A,  n_generics=%i,  n_overrides=%i,  ats=" ii.iname (hptos minfo.name.kind) assocf n_generics n_overrides)
          the_instance



//
//
// Flatten/inline a nest of VM2 machines.
//       
let rec buzbot_nest_flatten ww mm pep control tieoffs regslist fv1 rvu externalf (m_root_name_used, output_file_root) pe1 arg (aliases, (prams00, decls00), gates0, seqs0) = 
    match arg with
        | (ii, None) -> sf "verilog_gen: L2580"
        | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
          let m1 = mm + vlnvToStr minfo.name
          let ww = WN m1 ww
          let (rtl_fids, rtl_prams) = List.foldBack pram_groom minfo.atts ([], [])
          let n_generics = length rtl_prams // Attributes contain default values.  OLD: Actual values are in the contacts list and need to be split off.
          vprintln 2 (m1 + sprintf ": vm2 component iname=%s (with %i generics) being inline converted to Verilog. kind=%s"  ii.iname n_generics (vlnvToStr (minfo.name)))

          let aliases = (mine_decl_aliases ww true decls) @ aliases

          //let d_exec = gec_d_exec ww fv1 pep control tieoffs output_file_root pe1 rvu mm aliases 
          //let decls = refactor_old_rtl_pram_annotations ww decls // This call now redundant since done inside cilnorm etc..
          let (gates, nets, bevb) = run_d_exec ww fv1 pep control tieoffs regslist output_file_root pe1 rvu mm aliases m1 pe1 execs
          let hw_monitors = map (g2_assertion ww fv1 rvu pe1 aliases) assertions               // This should not be affected by the keep 'IF'.
          //reportx 3 (m1 + " net batch") netToStr n
          let skip_params = None // for now
          let cx1 = eqnToL fv1 (WN (m1 + " net batch") ww) (rvu, pe1, aliases) gg_unspec_width // This is the conversion function for net delcarations.
          let cx = gec_cx "buzbot-2686" cx1
                  
          let gdecls = cvt_flatten (skip_params, 1) cx (Some true)  decls
          let ldecls = cvt_flatten (Some true, 1)   cx (Some false) decls  // We dont want this flattening I think? Looses information.
          let lprams = cvt_flatten (Some false, 1)  cx (Some false) decls
          let _ =
              if fv1.vd>=5 then // debug net scanner

                 vprintln 5 (sprintf "gdecls-boat = %s" (List.foldBack debug_vdbToStr gdecls ""))
                 vprintln 5 (sprintf "ldecls-boat = %s" (List.foldBack debug_vdbToStr ldecls ""))
                 vprintln 5 (sprintf "lprams-boat = %s" (List.foldBack debug_vdbToStr lprams ""))

                 // Another debug scanner
                 let rec aToStrSerf n cc =
                     match n with 
                     | VDB_actual(_, v)  -> vnetToStr v + " " + cc
                     | VDB_group(_, lst) -> List.foldBack aToStrSerf lst cc
                 let aToStr x = aToStrSerf x ""
                 if fv1.vd >= 4 then
                     vprintln 4 (sprintf "gdecls on debug net scanner " + sfold (aToStr) gdecls)
                     vprintln 4(sprintf "ldecls on debug net scanner " + sfold (aToStr) ldecls)
                 ()


          let new_decls = 
              let wrap x =
                  let vnet = cx1 x
                  //dev_println (sprintf "wrap actual 1/2 " + netToStr x)
                  if hexpt_is_io x then VDB_formal(vnet) else VDB_actual(None, vnet)

              let genn (id, nets_for_group) cc = 
                  if nullp nets_for_group then cc
                  else
                      let pis = gec_VDB_group({g_null_db_metainfo with kind="L2590-vg"; pi_name=funique ("net2batch" + id) }, map wrap nets_for_group)
                      //vprintln 0 (sprintf "boat-pis = %s" (List.foldBack vdbToStr pis ""))
                      pis@cc
              List.foldBack genn nets [] 
          //dev_println(sprintf "%i gdecls,  %i ldecls, %i newdecls, %i old_decls" (length gdecls) (length ldecls) (length new_decls) (length decls00))
          let decls = gdecls @ ldecls @ new_decls

          let newpramdefs = // Convert to 'verilog_unit_t' from 'v_decl_binder_t'
              let conv ss =  if ss<> "" && isDigit ss.[0] then gec_v_num_autowidth g_default_prec (xi_num64 (atoi64 ss)) else vgen_STRING ss
              let rec flatten arg cc =
                  match arg with
                      | VDB_group(_, lst)      -> List.foldBack flatten lst cc
                      | VDB_actual(None, v)    ->
                          let defv = 
                              match v with
                                  | V_NET(ff, _, _) ->
                                      let f2 = lookup_net2 ff.n
                                      match at_assoc "init" f2.ats  with
                                          | Some ss -> Some(conv ss)
                                          | None    -> None
                                  | _ -> None
                          V_PRAMDEF(v, defv) :: cc
                      | VDB_actual(Some ss, v) ->
                          V_PRAMDEF(v, Some(conv ss)) :: cc
              List.foldBack flatten lprams []
          //dev_println (sprintf "newpramdefs=%A" newpramdefs)
          
          // Add_once reverses 1st arg: prefer to keep order here
          // It will not meet its goals in the presence of VDB_groups - fix or remove.
          let c1 = (aliases, (add_once(newpramdefs, prams00), decls@decls00), rtl_prams @ hw_monitors @ gates @ gates0, bevb @ seqs0)
          // Recurse on sons if being rendered in line (as opposed to just examine contacts of sons in the alternate hand).
          // let buzbot_nest_flatten ww mm pe00 fv1 rvu m_offspring externalf topflag mch output_file_root d_exec ((newprams0, gnets0, lnets0), gates0, seqs0) = 
          List.foldBack (machine_walk_ccf ww (fv1, tieoffs, regslist, control, pep, rvu, (m_root_name_used, output_file_root), mm, pe1))  sons c1


              
//
//
//       
and machine_walk_ccf ww (fv1, tieoffs, regslist, control, pep, rvu, (m_root_name_used, output_file_root), mm, pe1) (ii:vm2_iinfo_t, mcho) (aliases0, (prams00, decls00), gates0, seqs0) =
    // OLD:Some son instances need rendering inline as part of this RTL and others need to be rendered as instances of separate RTL modules.
    // NEW: We no longer tend to render inline since we no longer have the flat namespace concept. TODO redocument.
    let instancef = not ii.definitionf
    let _ : v_decl_binder_t list = decls00
    match mcho with
        | None ->
            // We do not rez via cvipgen and then render. We assume System Integrator and/or other mechanisms (such as vendor tools loading from cvgates.v) will serve for that.
            // If the mcho field is null, we have neither a worthwhile definition or instance. We have a declaration at best, which we here ignore.
            vprintln 1 (sprintf "Missing machine not rendered ! (definitionf=%A) kind=" ii.definitionf + vlnvToStr ii.vlnv)
            (aliases0, (prams00, decls00), gates0, seqs0)

        | Some(HPR_VM2(minfo, decls, sons, execs, assertions)) ->
            let m1 = mm + vlnvToStr minfo.name

            // let keepf = not_nonep(at_assoc "preserveinstance" minfo.atts) || ii.preserve_instance                   // TODO - in future, just  take these propertiess from ii not from the ats            
            // let keepasinstance = not topflag && (ii.preserve_instance || (* or old way *) keepf)

            if instancef && ii.preserve_instance then
                // Keep as an explicit instance.
                vprintln 2 (sprintf "Encountered an instance of component " + vlnvToStr ii.vlnv)
                let the_instance = buztop_keep_instance ww mm pe1 fv1 rvu aliases0 (ii, mcho)
                (aliases0, (prams00, decls00), the_instance::gates0, seqs0)

            else 
                let externalf = ii.externally_provided //  || (* or old way *) keep = Some("externally-provided")  // When externally-provided, we do not render the component here. cf "in-same-file" and also cf external instantiatons

#if SPARE_OLD
                // A VM that is a definition that will be externally provided does not need rendering, since the external one will be used in FPGA tools.  Its only purpose was a simulation model for diosim pre auto-rez.
                let already =
                    let find_existing = function
                        | (ii, Some(HPR_VM2(minfo', decls', sons', execs', assertions'))) -> minfo.name = minfo'.name && minfo.squirrelled = minfo'.squirrelled
                    List.filter find_existing !m_offspring
                    
                if runningf && not ii.nested_extensionf  then
                    mutadd m_offspring (ii, mcho)
                    vprintln 2 (sprintf "Deferring for a separate RTL module the rendering of component definiton " + vlnvToStr ii.vlnv)
                    (peo, aliases0, (newprams0, nets0), gates0, seqs0)

                elif not_nullp already then
                    vprintln 2 (sprintf "Discarding already accumulated component definiton for " + vlnvToStr ii.vlnv)
                    (peo, aliases0, (newprams0, nets0), gates0, seqs0)

                else
#endif
                let (rtl_fids, rtl_prams) = List.foldBack pram_groom minfo.atts ([], [])
                //let decls = refactor_old_rtl_pram_annotations ww decls // This call now redundant since done inside cilnorm etc..
                //vprintln 2 (sprintf "Consider future for %s.  keepasinstance=%A" m1 keepasinstance)
                buzbot_nest_flatten ww mm pep control tieoffs regslist fv1 rvu  externalf (m_root_name_used, output_file_root) pe1 (ii, mcho) (aliases0, (prams00, decls00), gates0, seqs0) 

//
//  D-type flip-flop emitter for gate-level output.                 
//
let dff_emitter ww fv1 pcont pep aliases mod_rvu =
    let kk0 = new_known_dp []
    // Emit the flip-flops that we have gate_collated if compiling to gates. - Such gate-level compilation should be in a different recipe stage such as gbuild!

    let (reset_expr, is_asynch, reset_nets) = // Repeated reset code. Does not handle mixed synch and asynch resets so far, but could be added.
         match pcont.dir.resets with
             | [] -> (X_false, true, [])
             | (true,  is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pcont.dir.resets), is_asynch, map f3o3 pcont.dir.resets)
             | (false, is_asynch, resetnet)::ignored_ -> (ix_orl(map greset pcont.dir.resets), is_asynch, map f3o3 pcont.dir.resets) 

    let emit_dff lhs updates =
        let msg = "Genflop" + vToStr lhs
        let ww' = WN msg ww
        let m_dir:directorate_synth_t option ref = ref None
        let mx c (l, diro, grd, r) =
            match (!m_dir, diro) with
                | (_,    None)                  -> () // An error really 
                | (None, Some dir)              ->  m_dir := Some dir
                | (Some ov, Some nv) when ov=nv -> ()
                | (Some ov, Some nv)            -> sf ("verilog_gen: Cannot render flip-flop " + vToStr lhs + " which straddles two different clock domains or directorates: " + sdirToStr ov + " cf " + sdirToStr nv)
            //vprintln 3 ("mx for " + vToStr lhs + " g=" + xToStr grd + " r= " + xToStr r)
            ix_query grd (xi_blift r) c
        let l = f1o4(hd updates)

        //let sdir = valOf pcont.sdr // Which is correct?
        let sdir:directorate_synth_t =
            match !m_dir with
                | None -> sf ("Cannot render flip-flop " + vToStr lhs + " which has no clock/directorate domain")
                | Some sdir -> sdir
        let sdir = valOf_or_fail "L2890" pcont.sdir
        let domain_cen = if nonep sdir.int_run_enable then X_true else xi_orred(valOf sdir.int_run_enable) // Two copies of this.  Need to factor in external clock enable in advanced
        let r = List.fold mx (xi_blift l) updates 
        //let cen = X_true //this could be disjunction of the guards above.
        let (rhs, cen) =
            if pcont.cen_factor then // Factor for second time
                 let (r1, cen1) = lr_factor l (xi_orred r)
                 let cen = bpandex ww pep (cen1) // pandex of this already done in fact
                 (r1, ix_and cen1 cen)
            else (xi_orred r, domain_cen)

        let ad = (new_rvu msg (Some mod_rvu), pcont, aliases)
        let ado = (fv1, valOf (f1o3 ad), f2o3 ad, f3o3 ad)
        let rstnet =
            if nullp pcont.dir.resets then None
            else Some(beqnToL fv1 ww' ad kk0 (ix_orl(map greset pcont.dir.resets)))
        let clknet = eqnToL fv1 ww' ad g_bool_prec (if nonep sdir.clknet then g_clknet else de_edge (valOf sdir.clknet))
        let rhs = beqnToL fv1 ww' ad kk0 rhs
        let vreset =
            if xi_isfalse reset_expr then None
            else Some(beqnToL fv1 ww' ad kk0 reset_expr)
        let cen = beqnToL fv1 ww' ad kk0 cen
        let _ = vgen_seq (clknet, rstnet, (reset_expr, is_asynch, vreset), ado) (lhs, rhs, cen) 
        ()
    for z in pcont.gatelevel_mux do emit_dff z.Key z.Value done
    ()
    

  
(*
 * Walker function for subexp sharing.
 * Function to compute the cost of a subtree.
 *)
let gen_subexp_walker ww fv1 =
    let vd = fv1.vd
    let vdp = false
    let share_opfun (costs:pina_t) dN bo_ xo nodeinfo lst =  // bo is not looked at since we use sum-of-products form (at the moment bmux if is off)
        let neverShareMe = // TODO nevershare should be an attribute in gis please, not spotted ad hom here.
            match xo with
                | None -> false
                // The KppMark cannot be shared - firstly it is side-effecting and must be called at the right time - secondly, once shared, it is not pattern matched properly and the calls to it get lost.
                | Some(W_apply(("hpr_KppMark", gis), _, args, _)) -> true
                | Some _ -> false

        let rec never_shares = function // use disjunctionate (aka any) please.
            | []                -> false
            | (NEVER_SHARE)::tt -> true
            | _::tt             -> never_shares tt
        // We sum the costs of our children.
        // Nodes above our cost threshold are marked as good for sharing.
        let kk = function
            | (SHARE_NODE _, (c, k)) -> (c, k) // should perhaps return zero here since sharing brings cost back to zero.  <-- TODO clarify
            | (COST_NODE(v, f), (c, SFH_none)) -> (v+c, f)
            | (COST_NODE(v, SFH_none), (c, f)) -> (v+c, f)
            | (COST_NODE(v, f), (c, f1))       -> if f<>f1 then sf "share_opfun: son should have been changed to SHARE_NODE" else (v+c, f)
            | (_, (c, k)) -> sf "shareop other"
        // NB we are ONLY DOING xo and not bo (booleans) TODO?
        if not_nonep xo then costs.add dN (valOf xo)
        if neverShareMe || never_shares lst then NEVER_SHARE
        else
            let (cost, k) = foldl kk (0, SFH_none) (nodeinfo::lst)
            if cost >= fv1.subex_sharecost_threshold then SHARE_NODE k else COST_NODE(cost, k)

    (*
     * Function to change a disagreeable son to be a separate sharing node.
     * A 'disagreement' means they cannot be shared since they need to be in different forms: continuous assignment versus fully-supported (@(*) form) always.
     *)
    let share_sonchange (costs:walker_costs_t) (COST_NODE(c, k)) nn = function
        | (n, (SHARE_NODE x)) -> SHARE_NODE x
        | (n, (COST_NODE(c', k'))) ->
            //vprintln 0 (sprintf "share consider " + xToStr n)
            if k=SFH_none || k'=SFH_none || k=k' then COST_NODE(c', k')
            else
                let aa' = SHARE_NODE(k')
                costs.add (x2nn n) aa'
                aa'

        // | _ -> sf "share_sonchange"

    let share_bsonchange (costs:walker_costs_t) (COST_NODE(c, k)) nn = function
        | (n, (SHARE_NODE x)) -> SHARE_NODE x
        | (n, (COST_NODE(c', k'))) ->
            if k=SFH_none || k'=SFH_none || k=k' then COST_NODE(c', k')
            else
                let aA' = SHARE_NODE(k')
                costs.add (abs (xb2nn n)) aA'
                aA'

        // | _ -> sf "share_sonchange"
    let (pina, sSS) = new_walker vd vdp (true, share_opfun, (fun _ -> ()), (fun _ _ _ _ ->()), (fun _ ->()), share_bsonchange, share_sonchange)
    (pina, sSS)


(*
 * Based on costlevel, pin shared expressions (uses count > 1) with that cost or more...
 * This is based on whole mya_array and not a local scope ...
 *
 * Return a list of tuples: (type of event control, the expression pinned, pin net).
 *)
let mya_findshares22 vd (ss:walkctl_t) (pina:pina_t) =
    let rec kk cc pos =
        if pos >= !mya_splay_nn then cc
        else 
            let netg prec id =
                let width = if nonep prec.widtho then 32 else valOf prec.widtho
                let signed = prec.signed
                X_bnet(iogen_serf(id, [], 0I, width, LOCAL, signed, None, false, [], []))
            let pinner n it =
                let prec = mine_prec true it
                netg prec (funique (sprintf "hpr_pin%ix" n))

            let isshare = function
                | (SHARE_NODE t) -> Some t
                | _ -> None
            let xi_width_or_32 v = 
                    let r = ewidth "xi_width_or_32" v
                    if r<>None && r <> Some 0 then valOf r else 32 
            let ito = pina.lookup pos
            let ov = ss.costs.lookup pos
            let cc = 
                if nonep ov || nonep ito || nonep (isshare(valOf ov)) then cc
                else
                    match ss.usecounts.lookup pos with
                        | None -> cc
                        | Some uc ->
                            if vd>=3 then vprintln 3 (sprintf "Use count for %i is %i (cost %A)" pos uc (ss.costs.lookup pos))
                            let it = valOf ito
                            let ecmode = valOf(isshare(valOf ov))
                            let np = pinner pos it
                            //dev_println ("Share node " + i2s pos + " " + ig2s pos + " w=" + i2s pw + " as  " + xToStr np)
                            (ecmode<>SFH_fcall, it, np)::cc 
            kk cc (pos+1)
    let ans = kk [] 4  // Iterate over all expressions - not just ones of interest!
    ans

//
// cvtTo2 -
//  1. recursive tree walk of VM2 hierarchy
//  2. find subexpressions to share.       
//  3. ...
let cvtTo2 ww (msg, fv1, control, (m_root_name_used, result_ii, output_file_root, filename)) designx =        
    let vd = fv1.vd
    let vdp = false
    let (ii, minfo, decls_in) =
        match designx with
            | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) -> (ii, minfo, decls)
            | _ -> sf "L2957"

    let regslist = new regslist_t("reglist") // This instance uses only the is_combreg flag.
    let tieoffs = [] // This mechamism no-longer used here.
    let mm = msg
    let tieoffs_rw = makemap tieoffs  // This is null, but is needed for the composer to accept a per-net skip.
    // It would be sensible to do subexpressions within a locality of a large design.  We do have locality information to hand but dont parition so far.
    let (pina, sSS) = gen_subexp_walker ww fv1
    let _ = walk_vm ww vdp (mm, sSS, None, g_null_pliwalk_fun) designx
    let shares =
        if fv1.disable_subexpression_sharing  // Global subexpression finder for sharing.
        then 
            vprintln 1 "Subexp sharing disabled."
            []
        else mya_findshares22 vd sSS pina
    vprintln 2 (sprintf "%i subexpressions selected for sharing" (length shares))
     
    let shares_rw =
        let shg (ecmode, exp, pin) = // Subexpression 'exp' becomes replaced with a pin net.
            //vprintln 0 (sprintf "Share for xidx_t %A   %s   with pin=%s and prec=%A" (x2nn exp) (xToStr exp) (xToStr pin) (mine_prec true exp))
            let ans = (* if xi_arithp(l) then*) gen_AR(exp, pin) (* else AR(l, xi_orred r) *)
            ans
        makemap1 (map shg shares) 
    let pep = new_pandex_dp(control) // One of these per run of this recipe stage. ? 
    let mod_rvu = Some(create_rvu "MODUAL" None)
    let pe1 = gec_pe ww fv1


    let mapfunc = rewrite_compose [] tieoffs_rw shares_rw
    //vprintln 1 ("Timestamp: L2389 " + timestamp true)                
    let (aliases, (newprams, decls), gates, bevb) = machine_walk_ccf ww (fv1, mapfunc, regslist, control, pep, mod_rvu, (m_root_name_used, output_file_root), mm, pe1) designx ([], ([], []), [], [])

    //vprintln 1 ("Timestamp: L2408 " + timestamp true)                
    let pcont00 = pe1
    dff_emitter ww fv1 pcont00 pep aliases (valOf mod_rvu)
    
    (* Don't need pins that are not in the final support *)
    (* Create the pin assigns, having rewritten their rhs with no rewrite for itself. *)
    let pin_rw skip x = 
        let rr = xi_rewrite (rewrite_compose [skip] tieoffs_rw shares_rw) x
        //vprintln 0 ("  " + xToStr skip + " pin_rw -> " + xToStr rr)
        rr

    let msa_share cc (oldsort, r, l) = // hprpin net generation
        let prec = mine_prec true l
        //dev_println (sprintf "pin prec=%A" prec)
        let l' = eqnToL fv1 ww (mod_rvu, pcont00, aliases) prec l //transclose ?
        let w = valOf_or prec.widtho -1        
        let r' = eqnToL fv1 ww (mod_rvu, pcont00, aliases) prec (pin_rw r r) 
        let ans = gec_V_CONT fv1 oldsort (l', r') (* TODO?: if building gatelevel, we need to pandex this ! and collect its offspring*)
        ans :: cc
    vprintln 1 ("Timestamp: L2464 " + timestamp true)
    let share_assigns = List.fold msa_share [] shares
    let share_nets =
        let gfa = function
            | (V_CONT(_, l, _), _) -> l
            | _ -> sf "gfa other"
        let wrap x =
            //dev_println (sprintf "wrap actual 2/2 %A" x)
            VDB_actual(None, x)
        gec_VDB_group({g_null_db_metainfo with kind="share-nets"; pi_name=funique "share-A-nets"}, map (gfa>>wrap) share_assigns)

    let pcont_nets =
        let cx = (eqnToL fv1 ww (mod_rvu, pcont00, aliases) gg_unspec_width)  // need to iterate on this line ! - TODO?
        let wrap x =
            //dev_println (sprintf "wrap actual 3/2 " + netToStr x)
            VDB_actual(None, cx x)
        gec_VDB_group({g_null_db_metainfo with kind= "share-nets"; pi_name= funique "share-B-nets"}, map wrap !pcont00.m_nets)

    vprintln 1 ("Timestamp: L2474 " + timestamp true)
    let gatef = not_nonep fv1.prefs.gatelib
    let mod_units = ref []
    if gatef then
        let _ = WF 3 "cvtToVerilog"  ww "Computing area of each region"
        let total = rvu_tally mod_units (valOf_or_fail "L3087" mod_rvu)
        mutadd mod_units (V_SCOMMENT("Total area " + i2s64 total), ref None) // This mutably copies the instances to mod_units, so call last.
        let _ = WF 3 "cvtToVerilog" ww ("Computed total area = " + i2s64 total)
        ()
            
    // Once areas are computed, wire lengths can be computed (using LCP approach: abscissa approach computes wiring swell area afterwards!)

    let domains = List.foldBack (fun (clk, _) cc -> singly_add clk cc) bevb  []
    let pe000 = pe1

    //vprintln 2 (sprintf "gbev will be applied to %i clock domains (counting combinational logic as a domain)." (length domains))
    let gbev ww clkarg cc =
        let domain_collator clk (clk', b) cc = if clk=clk' then b@cc else cc//Can make this more efficient and trap stragglers using leftovers paradigm, or use generic_collate
        match clkarg with
          | None -> 
               let m0 = "gbev-combinational"
               vprintln 2 (sprintf "Now handle " + m0)
               let domain_logic = List.foldBack (domain_collator clkarg) bevb []
               let gcomb arg cc =
                   match arg with
                   | V_NBA(l, r)   -> gec_V_CONT fv1 false (l, r)::cc
                   | V_COMMENT s   -> (V_SCOMMENT s, ref None)::cc
                   | V_BLOCK []    -> cc   // disregard null block
                   | other -> 
                       vprintln 0 (sprintf "verilog_gen: other %A" other)
                       hpr_yikes  "+++ verilog_gen: other combinational form"
                       (V_SCOMMENT("+++ verilog_gen: other combinational form:"), ref None)::(V_ALWAYS other, ref None)::cc
               List.foldBack gcomb domain_logic cc
          | Some clk ->
            let m0 = "gbev-synch " + edgeToStr clk
            let domain_logic = List.foldBack (domain_collator clkarg) bevb []

            let (ec, combf) =
                  match clk with
                    | (E_pos x)
                    | (E_neg x) ->
                        let ck1 = eqnToL fv1 (WN m0 ww) (mod_rvu, pe000, aliases) g_bool_prec (de_edge clk)
                        (V_EVC(v_evc_gen1 ww (fv1, mod_rvu, pe000, aliases) clk ck1), false)
                    | E_anystar -> (V_EVC(V_EVC_STAR), true)
                    //| other -> sf("not one clock net for "  + m0 +  ":" + sfold edgeToStr other)
            //dev_println (sprintf "pre hope ec=%A" ec)
            // Constant combs get rendered as continuous assigns since RTL simulators object to null full support.
            let parter = function
                | V_COMMENT _ -> false
                | V_SWITCH _
                | V_IF _
                | V_IFE _ -> false                                
                | V_BA(l, r) 
                | V_NBA(l, r) -> v_isconst r  // Perhaps also need lhs not to be an array - but we can continuously assign an array location.
                | other ->
                    false
            let (constconts, others) = (if combf && false then List.partition parter domain_logic else ([], domain_logic)) // This code not needed - the real work is in gec_V_CONT
            let lp1 = if nullp others then [] else [(V_ALWAYS(gen_V_BLOCK(ec :: others)), ref None)]
            let lp2 = // Always null given above 'false' 
                let tocont = function
                    | V_BA(l, r) 
                    | V_NBA(l, r) -> (V_CONT((None, true), l, r), ref None)
                map tocont constconts
            vprintln 2 (sprintf "gbev unit inventory for clk=%s:  %i seq and %i cont" (edgeToStr clk) (length lp1) (length lp2))
            lp1 @ lp2 @ cc



    //vprintln 1 ("Timestamp: L2266 " + timestamp true)
    let ww = WF 2 "cvtToVerilog"  ww (sprintf "Start apply gbev to %i domains (counting combinational logic as a domain)." (length domains))
    let bevunits = List.foldBack (gbev ww) domains []
    let ww = WF 2 "cvtToVerilog"  ww (sprintf "Finished gbev application.")
    //vprintln 1 ("Timestamp: L2268 " + timestamp true)
    let (llnets, lunits) = (map (eqnToL fv1 (WN "lnets" ww) (mod_rvu, pe000, aliases) gg_unspec_width) !pe000.m_nets, !pe000.m_units)
    let llnets =
        let wrap x =
            //dev_println (sprintf "wrap actual 4/2 %A" x)
            VDB_actual(None, x)
        gec_VDB_group({g_null_db_metainfo with kind="llnets-2694"; pi_name= funique "llnets"}, map (wrap) llnets)

    let rom_setups =
        // A ROM looks like a RAM in the RTL, except the only writes are from Verilog's initial statements.
        let m_rom_setups = ref []
        let render_rom_setup (z:net_att_t, vnet, len) =
            let w = encoding_width (X_bnet z)
            let gen_init_assign (d, idx) = V_BA(V_SUBSC(vnet, V_NUM(-1, z.signed=Signed, "d", xi_num idx)), deconstv w z d) 
            let leader = V_COMMENT(sprintf "ROM data table: %i words of %i bits." len w) 
            let ans = V_INITIAL (gen_V_BLOCK(leader :: map gen_init_assign (zipWithIndex z.constval)))
            vprintln 3 ("Rendered ROM setup for " + z.id)
            mutadd m_rom_setups (ans, ref None)
        for z in pe000.rom_inits do render_rom_setup z.Value done
        !m_rom_setups

    //vprintln 1 ("Timestamp: L2285 " + timestamp true)
    let subx_decls = verilog_subexps pe1
    //vprintln 1 ("Timestamp: L2285 " + timestamp true)
    let extra_units = 
        match !pe000.m_units with
            | [] -> []
            | items -> muddy (sprintf "Ignored %i extra units\n" (length items))
    let (extra_units, _) = 
        if nonep mod_rvu then ([], [])
        else 
            let rec rvu_scan site (rvu:layout_zone_t) (cc, cd) =
                let cc = 
                    match !rvu.m_units with
                        | [] -> cc
                        | items ->
                            vprintln 3 (sprintf "Retrieved %i extra units in mod_rvu\n%A" (length items) items)
                            items @ cc
                let cd = cd 
                List.foldBack (rvu_scan "recurse") !rvu.m_sons (cc, cd)
            rvu_scan "mod_rvu" (valOf mod_rvu) ([], [])
    let bevunits = extra_units @ bevunits
    let kppIos = // Waypoint nets - part of directorate
        match !pe000.m_kpp_nets with
            | None -> []
            | Some (a, b) ->
                let kppx_to_v net = gec_V_NET -1 (netgut net)
                let wrap x =
                    //dev_println (sprintf "wrap waypoint contact 5/2 %A " x)
                    VDB_formal(x)
                gec_VDB_group({ g_null_db_metainfo with kind="waypoint_nets";  pi_name= funique "kppIos"; form=DB_form_external }, map (wrap) [kppx_to_v a; kppx_to_v b])
    //dev_println (sprintf "kpp nets are %A" kppIos)
    let decls = kppIos @ decls @ llnets
    //reportx 3 "Convert to Verilog nets" vnetToStr nets1
    let d = mm + sprintf ": Finished convert to Verilog: gates=%i, net groups=%i, ROMs=%i" (length gates) (length decls) (length rom_setups) + " at timestamp: L2296 " + timestamp true
    vprintln 1 d
    let _ = WF 3 "cvtToVerilog"  ww d

    let (tag, iname, kind) =
        let _ : vm2_iinfo_t = result_ii
        let topflag = not !m_root_name_used
        if topflag then
            m_root_name_used := true        
            let tag = if topflag then "" else result_ii.iname
            vprintln 2 (sprintf "Creating output name/kind (used already topflag=%A) tag/name=%s" topflag tag) 
            (tag, result_ii.iname, result_ii.vlnv)
        else
    // TODO - in future take these properties from ii not from the ats
            let iname = if ii.iname="" then hptos minfo.name.kind else ii.iname // valOf_or (at_assoc "iname" minfo.atts) (hptos minfo.name.kind)
            //let kind = valOf_or (at_assoc "kind" minfo.atts) minfo.name.kind // kind attribute / override is no longer used
            (iname, iname, minfo.name)
    let rid = { ii with iname=iname; vlnv=kind } 
    let hh_level = 1 // Nominal starting value for the component under design.        
    vprintln 2 (sprintf "Unit inventory: kind=%s %i bevunits,  %i gates/FUs, %i share_assigns, %i mod_units, %i rom_setups, %i subx_decls." (hptos kind.kind) (length bevunits) (length gates) (length share_assigns) (length !mod_units) (length rom_setups) (length subx_decls))

    let (vgdecls, vldecls) =
        let regroom arg (gcc, lcc) = // A bit messy, but a final split back into contacts and locals.
            match arg with
            | VDB_group(pi, items) ->
                let external_pred = function
                    | VDB_formal _ -> true
                    | VDB_actual _ -> false
                    | arg -> sf (sprintf "external_pred arg=%A" arg)
                let (g, l) = groom2 external_pred items
                let a = if not_nullp g then [VDB_group({pi with form=DB_form_external}, g)] else []
                let b = if not_nullp l then [VDB_group({pi with form=DB_form_local}, l)] else [] 
                (a@gcc, b@lcc)
            | _ -> sf "verilog_gen: regroom other layout"
        List.foldBack regroom (decls @ share_nets @ pcont_nets) ([], [])

    //dev_println (sprintf "vldecls are %A" vldecls)
    //dev_println (sprintf "vgdecls are %A" vgdecls)        
    let vmodule = (rid, filename, hh_level, (aliases, newprams, vgdecls, vldecls), bevunits @ gates @ share_assigns @ !mod_units @ rom_setups @ subx_decls)
    let netinfo_dir = pe000.netinfo_dir

    match fv1.prefs.layout_enable with
        | LAYOUT_disable ->
            ()
        | LAYOUT_random
        | LAYOUT_constructive ->
            let msg = "Computing complete layout for wire lengths instead of using efficient LCP estimator"
            let _ = WF 3 "cvtToVerilog" ww msg
            make_nominal_layout ww vd fv1.prefs msg (valOf mod_rvu) netinfo_dir
            plot_layout (valOf mod_rvu, "layout", netinfo_dir)
            ()
        | LAYOUT_lcp ->
            let _ = WF 3 "cvtToVerilog" ww ("Computing wire lengths: LCP estimator.")
            for z in netinfo_dir do lcp_compute_wire_len (z.Value) done
            plot_layout (valOf mod_rvu, "non-layout", netinfo_dir)
            ()


    let _ = // Write IP_XACT component definition.  This should be independent of RTL output since equally valid for SystemC output. TODO

          let reported_area = 1                     // for now
          let reported_heapuses = [ ("port0", 0L) ] // for now
#if OLD
          // These are per-method and so should be in the busDefinition from fsems, not here.
          let expected_latency = 1 // for now - some of these are per method and some per block TODO 
          let reinit_latency = 1 // for now - some of these are per method and some per block TODO                 
          let eisf = false                          // for now
          let inhold = false                        // for now
          let outhold = false                       // for now
#endif
          match fv1.ip_xact_filestyle with
                  | "disable" -> ()
                  | style_ ->
                      // IP-XACT output - probably move to own recipe stage please.
                      let depram arg cc =
                            match arg with 
                            //| (DB_leaf(Some v, arg), nn) -> (v, arg)::cc
                            | (DB_leaf(_, Some arg), nn)   -> (sprintf "P%i" nn, xToStr arg)::cc // TODO pram names in here instead of just ordinals.
                            | _ ->
                                dev_println ("other parameter tree ignored")
                                cc
                      let (parameters, ports) = // The top level, metaprams, should not appear at all in RTL (except as embedded comments).
                            let groom arg (parameters, ports) =
                                match arg with
                                    | DB_group(meta, items) when meta.form=DB_form_pramor ->
                                        let portmeta = meta.metaprams // metaprams and RTL pramors are mixed freely here for now.
                                        (portmeta @ List.foldBack depram (zipWithIndex items) parameters, ports)
                                    | _ -> (parameters, arg::ports)
                            List.foldBack groom decls_in ([], [])
                      let files = [ output_file_root + tag +  fv1.output_file_suffix ]
                      // TODO place in folder designated by -obj-dir-name= which defaults to '.'
                      let xml_file_name = output_file_root + tag  // for now
                      protocols.ip_xact_export_component ww xml_file_name parameters files ports (reported_area, reported_heapuses) // fsems
                      ()

    (designx, pe000, vmodule)// end of cvtTo2



//    
// Convert one HPR machine to RTL.
// We need to look at the I/O connections to our son machines (which may be rendered as externally instantiated) for net declarations to wire to them, but we do not process their bodies unless being rendered in line.
// Those that are to be emitted as separate RTL modules (not rendered in line) will be handled by a seprate call to cvtTo1 from y_iterate or similar.
//    
let cvtTo1 ww (msg, fv1, control, (m_root_name_used, result_ii, output_file_root, filename)) designs  =
    let mm = ""
    let ww = WF 1 msg ww (mm + " start")

    // let rvu = new_rvu (mm) rvu // TODO define later
    // Some instances are normal, internal instances, and others are external instances whos connections will be added, with reverse directions, to the signature of the currently being rendered module.
    // Some separate RTL modules defintions will be provided externally and others will be defined in the same .v file that we are making.
    let defs = vm_separate ww msg designs
    let _ =
        let sqf minfo = if minfo.squirrelled="" then "" else " sq=" + minfo.squirrelled
        let rliner = function
            | (ii, Some vm2) -> ii.iname + "  " + vlnvToStr (hpr_minfo vm2).name + sqf (hpr_minfo vm2)
            | (ii, None) -> ii.iname            
            
        reportx 2 "Definitions found for RTL render" rliner defs
      
    map (cvtTo2 ww (msg, fv1, control, (m_root_name_used, result_ii, output_file_root, filename))) defs
        


// 
// Delay/layout estimator is run at the end of this function, but nothing is written to output file.
//
// The layout estimator should be a generic opath plugin not inside this module.  To run it on gate-level components, these should be exported back to VM2 using the presim code.       
//
let cvtToVerilog ww (mm, fv1, control) (m_root_name_used, result_ii, output_file_root, filename) topdesign =

    // TODO - we want area/layout estimates over multiple components, so perhaps move that analysis into this shim-like function.
    
    let mm = "cvtToVerilog"
    let vmodules = cvtTo1 ww (mm, fv1, control, (m_root_name_used, result_ii, output_file_root, filename)) [ topdesign ] // A silly extra layer owing to map of serf below
    vprintln 1 ("Timestamp: L2503 " + timestamp true)
    vmodules // end of cvtToVerilog



(*---------------------------------------------------------------------*)
(* RTL output bits *)


(*
 * Generate RTL I/O declarations from a list of nets.  OLD DELETE ME ?
 * Currently just used for the microcontroller output? use rtloutput1 instead.
 *)
let equations_to_ios ww (width, (ddctrl:ddctrl_t)) regslist flip (ml_, net) cc =
    let alias = ""
    match net with
        | (X_bnet ff) ->
            let f2 = lookup_net2 ff.n
            let regdeclf =  // regslist contains both sanitised and non-sanitised net names
                match (regslist:regslist_t).lookup ff.id with
                    | Some entry -> entry.is_reg
                    | _ -> false
            let delo = None
            let kk = function
                    | INPUT -> if flip then V_OUT else V_IN
                    | OUTPUT -> if flip then V_IN else V_OUT
                    | RETVAL -> if flip then V_IN else V_OUT (* Returned value is through an extra formal *)
                    | _ -> if f2.length <> [] then VNT_REG  // VNT_INT was used before verilog has signed registers . can delete now I think.

                           elif ff.width = 32 && ff.signed=Signed && ddctrl.use_integers then VNT_INT else VNT_WIRE
            vprintln 3 ("   equations_to_ios: " + netToStr net + sprintf " reg=%A" regdeclf)
            let vtype = f2.vtype
            let m = f2.xnet_io
            let w = encoding_width net
            let redec = if regdeclf && ddctrl.kandr then [V_NETDECL(net, delo, ff, w, alias, asize f2.length - 1L, VNT_REG, None)] else []
            if vtype <> V_VALUE || m=LOCAL then cc
            else V_NETDECL(net, delo, ff, w, alias, asize f2.length - 1L, kk m, None)::redec@cc
        | (_) -> cc
 

(*
 * Declare local nets to a Verilog module:: OLD DELETE TODO...
 *  used only by ... microcontroller outputter....
 * This code now replicated in verilog_render.fs - this copy is deprecated.
 *)
let equations_to_decls ww bag regslist (sr:statereport_t) (ml_, net) cc =
    let alias = ""
    match net with
        | X_bnet ff ->  (* This is replicated in verilog render!*)
            let f2 = lookup_net2 ff.n
            let delo = None
            let regdeclf =  // regslist contains both sanitised and non-sanitised net names
                match (regslist:regslist_t).lookup ff.id with
                    | Some entry -> entry.is_reg
                    | _ -> false

            let scalarf = (ff.width = 32 && ff.signed=Signed)
            let kk _  = if f2.length <> [] then VNT_REG 
                            else 
                            if scalarf && bag.use_integers then VNT_INT
                            elif regdeclf then VNT_REG // VNT_INT was used before verilog has signed registers . can delete now I think.
                                else VNT_WIRE
            let vtype = f2.vtype
            let m = f2.xnet_io
            let w = encoding_width net
            vprintln 3 ("   equations_to_decls: " + netToStr net + sprintf " reg=%A width=%i" regdeclf w)
            let elab = function
                | V_ELAB x -> true
                | other -> false
            let amax = asize f2.length - 1L
            let dummy = (amax = g_unspecified_array-1L) && (not !g_repack_disable)

            if dummy then vprintln 3 ("Ignored dummy array declaration " + ff.id)
            if (elab vtype) || dummy  then cc
            else
                let sparseinc (asl:array_size_log_t) w delta = asl.add w (delta + valOf_or (asl.lookup w) 0L)
                if (ff.is_array && amax+1L < 0L) then sf("bad String.length-B: " + netToStr net)
                let bits = (int64 w * (if ff.is_array then amax+1L else 1L))
                let _ = 
                    if not regdeclf then mutincu64 (sr.continuous) bits
                    //elif w > sr.maxwidth then mutincu64 (sr.otherbits) bits
                    elif scalarf then mutincu64 (sr.scalars) (int64 w)
                    elif ff.is_array then sparseinc sr.arrays w (amax + 1L)
                    else sparseinc sr.vectors w 1L
                V_NETDECL(net, delo, ff, w, alias, amax, kk m, None)::cc

        | (_) -> cc



(* This code is copied out verbatim from orangemain: 
   TODO one copy only please. Currently just used for the microcontroller output? use rtloutput1 please instead.
*)
let rtl_output_auxold ww bag vd fd (id, pcs, decls, units) =
    let prettyprint_width = 132 // for now
    let ddctrl = g_null_ddctrl
    let ww = WN "rtl_output_auxold" ww
    let vvf = (prettyprint_width, ddctrl)
    let regslist =
        let regslist = new regslist_t("regslist")// Set.empty
        List.fold rtl_reglist regslist units
    let sr = statereport_ctor true
    let flat = List.foldBack (db_flatten []) decls [] // need a depth limit and to add on @ pcs 
    let (gdecls, ldecls) = groom2 (snd >> hexpt_is_io) flat 

    let f_decls = List.foldBack (equations_to_ios ww vvf regslist true) gdecls []
    let l_decls = List.foldBack (equations_to_decls ww bag regslist sr) ldecls []
    let modulea = V_MOD(id, [], f_decls, l_decls, units)
    vprintln 2 ("Writing Verilog module " + id + " with " + i2s(length units) + " gates/units")
    verilog_render_module ww vvf sr (yout fd) regslist modulea
    vprintln 2 ("Wrote Verilog module " + id + " with " + i2s(length units) + " gates/units")
    ()



(*
 * Presim converts/lifts an RTL design back into x for diosim or other purposes.
 *)
type presim_t =
     { prefix : string;
     }

let rec presim_bexp (R:presim_t) arg =
    match arg with
    | V_NET(ff, _, -1) -> xi_orred(X_bnet ff)
    | V_DIADIC(prec, oo, l, r) -> 
        let _ = 0
        if oo=V_LOGAND then ix_and(presim_bexp R l) (presim_bexp R r)
        elif oo=V_LOGOR then ix_or (presim_bexp R l) (presim_bexp R r)
        elif oo=V_DEQD then xi_deqd(presim_exp R l, presim_exp R r)
        elif oo=V_DLTD then xi_dltd(presim_exp R l, presim_exp R r)
        elif oo=V_DLED then xi_dled(presim_exp R l, presim_exp R r)
        elif oo=V_DGED then xi_dged(presim_exp R l, presim_exp R r)
        elif oo=V_DGTD then xi_dgtd(presim_exp R l, presim_exp R r)
        else 
            let (a, b, c) = vdiTox oo
            if a<>None then xi_orred(ix_diop prec (valOf a) (presim_exp R l) (presim_exp R r))
            elif b<>None then ix_bdiop (valOf b) [presim_exp R l; presim_exp R r] false
            else sf("presimb: vbdi operator not listed: " + vdopToS oo)

    | (V_LOGNOT n) -> xi_not(presim_bexp R n)
    | (V_ECOMMENT(s, x)) -> presim_bexp R x
    | other -> xi_orred(presim_exp R other)

and presim_exp (preR:presim_t) = function // convert an RTL/Verilog expression back to hexp_t form.
    | V_NET(ff, _, -1) -> X_bnet ff
    | V_LOGNOT n    -> xi_blift(xi_not(presim_bexp preR n))
    | V_NUM(_, _, _, n) -> n
    | V_PARAM(s, Some ff) -> X_bnet ff
    | V_PARAM(s, None) -> gec_X_net s    
    | V_X _ -> X_undef
    | V_NET(ff, _, a) -> xi_blift(xi_bitsel(X_bnet ff, a))
    | V_2sCOMPLEMENT n -> 
        let v = presim_exp preR n
        xi_neg (mine_prec false v) v
    | V_CALL(cf, ((f, gis), ord), n) -> xi_apply_cf cf ((verilog_un_plimap f, gis), map (presim_exp preR) n)

    | V_SLICE(lhs, subs, n) ->
        ix_mask Unsigned n (ix_rshift_both Unsigned (presim_exp preR lhs) (presim_exp preR subs)) // Bit range extract - Verilog defaults to unsigned.

    | V_SUBSC(s, n) -> ix_asubsc (presim_exp preR s) (presim_exp preR n)
    | V_STRING(N, s) -> N
    | V_QUERY(g, l, r) -> xi_query(presim_bexp preR g, presim_exp preR l, presim_exp preR r)
    | V_ECOMMENT(s, x) -> presim_exp preR x
    | V_DIADIC(prec, oo, l, r) -> 
        let (a, b, c) = vdiTox oo
        if a<> None then ix_diop prec (valOf a) (presim_exp preR l) (presim_exp preR r)
        elif b<>None then xi_blift(ix_bdiop (valOf b) [presim_exp preR l; presim_exp preR r] false)
        elif c<>None then xi_blift(xi_bnode(valOf c, [presim_bexp preR l; presim_bexp preR r], false))
        else sf("presim: vdi operator not listed: " + vdopToS oo)

    | V_CAT lst ->
        let l1 = map (fun (w, e) -> (w, presim_exp preR e)) lst // could set w inside preR - eg to mask carries and left-shift shift outs.
        let catter sofar (w, ee) = ix_bitor ee (ix_lshift sofar (xi_num w))
        let r = List.fold catter (xi_num 0) l1
        r

//    | VD_BUS(n0, f, l, 0) -> n0
//    | VD_ARRAY(n0, f, len, l) -> n0 

    | V_MASK(h, l) -> presim_mask(V_MASK(h, l))
    | other -> sf(sprintf "presim_exp: other form %s detail=%A" (vToStr other) other)


let presim_gate R = function
    | ("CVDFF", [q; d; clk; cen; rst; greset]) -> 
        let _ = 1
        (* TODO clk *)
        let cen' = presim_bexp R cen
        let rst' = presim_exp R rst
        let q' = presim_exp R q
        let k = gec_Xif cen' (Xassign(gec_X_x(presim_exp R q, 1), presim_exp R d)) Xskip
        (*      let k = Xassign(X_x(q', 1), xgen_query(cen', presim_exp R d, q')) *)
        let _ = lprint 100 (fun()->(hbevToStr k) + " presimed\n")
        (k, Some[E_pos(presim_exp R clk)])

    | ("CVMUX2", [y; g; t; f]) -> 
        (Xassign(presim_exp R y, xi_query(presim_bexp R g, presim_exp R t, presim_exp R f)), None)

    | ("CVINV", [y; a]) -> 
        (Xassign(presim_exp R y, xi_blift(xi_not(presim_bexp R a))), None)

    | ("CVOR2", [y; a; b]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_or (presim_bexp R a) (presim_bexp R b))), None)
    | ("CVOR3", [y; a; b; c]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_or (ix_or (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c))), None)
    | ("CVOR4", [y; a; b; c; d]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_or (ix_or (ix_or (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c)) (presim_bexp R d))), None)
    | ("CVOR5", [y; a; b; c; d; e]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_or (ix_or (ix_or (ix_or (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c)) (presim_bexp R d)) (presim_bexp R e))), None)

    | ("CVBUF", [y; a]) -> 
        (Xassign(presim_exp R y, presim_exp R a), None)

    | ("CVXOR2", [y; a; b]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_xor (presim_bexp R a) (presim_bexp R b))), None)

    | ("CVAND2", [y; a; b]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_and (presim_bexp R a) (presim_bexp R b))), None)

    | ("CVAND3", [y; a; b; c]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_and (ix_and (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c))), None)

    | ("CVAND4", [y; a; b; c; d]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_and (ix_and (ix_and (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c)) (presim_bexp R d))), None)

    | ("CVAND5", [y; a; b; c; d; e]) -> 
        (Xassign(presim_exp R y, xi_blift(ix_and (ix_and (ix_and (ix_and (presim_bexp R a) (presim_bexp R b)) (presim_bexp R c)) (presim_bexp R d)) (presim_bexp R e))), None)

// TODO add further RTL functions like max, min etc..
    | ("hpr_abs", a) ->
        let fgis = builtin_fungis "hpr_abs"
        (Xeasc(xi_apply(("hpr_abs", valOf fgis), map (presim_exp R) a)), None)


    | (s, args) -> sf(s + " unrecognised gate for presim (roundtrip back to hexp_t from RTL form can be disabled with -vnl-roundtrip=disable): arity=" + i2s(length args))


let presim_gate1 R arg =
    let bax1 = function
        | VDB_actual(None, v) -> presim_exp R v // Need's positional syntax currently
        | _ -> sf "L2985z"
    let bax2 = function
        | VDB_actual(None, v) -> v
        | _ -> sf "L2985a (please add -vnl-roundtrip=disable)"
    match arg with
    | ("MPX", [ yw; aw; bw], contacts) ->  // Multiplier - TODO this code is now replaced with the behavioural models inserted at rez time by restructure ... I think.
        let rec f = function
            | ([], 0, c) -> (rev c, [])
            | ([], _, c) -> sf ("MPX too few actuals:" + i2s(length contacts))
            | (h::t, 0, c) -> (rev c, t)
            | (h::t, k, c) -> f (t, k-1, h::c)
        let nets = map bax1 contacts
        let af n = xi_manifest  "MPX port width" (presim_exp R (bax2 n))
        let (y, nets) = f(nets, af yw, [])
        let (a, nets) = f(nets, af aw, [])
        let (b, nets) = f(nets, af bw, [])

        let rec boil = function
            | [] -> xi_num 0
            | (h::t) -> xgen_blift(ix_or (xgen_orred h) (xgen_orred(ix_times (xi_num 2) (boil t))))
        (Xassign(boil y, ix_times (boil a) (boil b)), None)


    | (s, rides_, contacts) -> presim_gate R (s, map bax2 contacts)

(*
 *  Some of this code folds the 'g' guard in a bit, and other does not. todo: Why?
 *)
let rec presim_bev_ntr R g = function
    | V_IF(g1, s)     -> presim_bev R (xi_and(g, presim_bexp R g1)) s
    | V_IFE(g1, t, f) -> gec_Xif (presim_bexp R g1) (presim_bev R g t) (presim_bev R g f)
    | V_EASC s        -> gec_Xif g (Xeasc(presim_exp R s)) Xskip
    | V_NBA(l, r)     -> gec_Xif g (Xassign(gec_X_x(presim_exp R l, 1), presim_exp R r)) Xskip
    | V_BA(l, r)      -> gec_Xif g (Xassign(presim_exp R l, presim_exp R r)) Xskip
    | V_BLOCK l       -> gec_Xblock(map (presim_bev R g) l)
    | V_COMMENT(s)    -> Xskip
    | V_SWITCH(e, l, _)  ->
        let e' = presim_exp R e
        let gg a c  = ix_or (xi_deqd(e', presim_exp R a)) c
        let ff(x, v) = gec_Xif1 (List.foldBack gg x xi_false) (presim_bev R g v)
        gec_Xblock(map ff l)

    | (_) -> sf "presim_bev_ntr R other"

and presim_bev R g b =
   let vd = 0
   //vprintln vd ("presim bev " + verilog_render_bev b))
   let ans = presim_bev_ntr R g b 
   ans

#if MICROCODE_SML_INUSE
(*
 * This converts a simple subset of hbev to rtl.  You must use vm_compile for more
 * complex forms, but then you will loose the structure of the input psudo 'rtl'.
 *
 *    Please use eqnToL wherever possible instead.... why do we have this one ? Used by microcode.sml. TODO remove it.
 *
 * The guarded form of rewrite is used because .... ?  perhaps restucture was once considerd here rather than as a separate, eariler phase ? YES: this code also now appears in restucture so delete from here!
 
 *)
let rec xrtl_to_rtl__ ww fv1 = function // NEVER CALLED DEAD CODE?
        | (H2BLK(clkmode, SP_l(ast_ctrl, l)), c) ->
            let _ = sf "Not used?"
            let p = { fv1 with gatelib= None; clknet=clkmode; nets= ref []; units=ref []; rstnet=None; token="xrtl_to_rtl"; dp=new_cvtToRtl_DP() }
            let rvu = Some{ units=p.m_units; lon=1; loid="fornow"; }
            //let p1b g rwbar x = eqnToL (*xiToRtl*) (WN "xrtl_to_rtl" ww) (rvu, pp, aliases) (xi_rewrite_lmode g nulmap x)
            //   let p1 g rwbar x = eqnToL (*xiToRtl*) (WN "xrtl_to_rtl" ww) (rvu, pp, aliases) (xi_rewrite_lmode g nulmap x)
            let px  g x = eqnToL (*xiToRtl*) (WN "xrtl_to_rtl" ww) (rvu, pp, aliases) x
            let k0 = new_known_dp [] 
            let p1b g x = beqnToL (WN "xrtl_to_rtl" ww) (rvu, pp, aliases) k0 x
            let p1 g x = eqnToL (WN "xrtl_to_rtl" ww) (rvu, pp, aliases) x

            let pp g x = p1 g x
            let pb g x = p1b g x
            let pw g r x = p1 g  x
            let pwx g r x = px g  x
            let rec kb g = function
                | (Xif(gg, b, e), c) -> 
                     let gt = xi_and(g, gg)
                     let gf = xi_and(g, xi_not gg)
                     gen_V_IFE(p1b g gg, gen_V_BLOCK(kb gt (b, [])), gen_V_BLOCK(kb gf (e, [])))::c

                | (Xassign(l, r), c) -> let r' = px g r in gec_V_NBA(pwx g r' l, r')::c
                | (Xblock l, c) -> foldr (kb g) c l
                //    | kb g (Xeasc(x_valof l), c) -> foldr (kb g) c l
                | (Xeasc(e), c) -> kb1 g (e, c)
                | (Xskip, c) -> vnl_skip::c
                | (Xannotate(a, s), c) -> V_COMMENT(valOf_or_ns(a.comment)) :: kb g (s, c)
                |  (other, c) -> sf(hbevToStr other + " in xrtl_to_rtl other")
            and kb1 g = function
                // | (X_pair(l, r), c) -> kb1 g (l, kb1 g (r, c))
                | (W_node(prec, V_interm, [l; r], _), c) -> (* blocking really ?*)
                      let r' = pp g r in gec_V_NBA(pw g r' l, r')::c
                | (X_num 0, c) -> c
                | (other, c) -> sf(xToStr other + " in xrtl_to_rtl other 1")

            let body = foldr (kb xi_true) [] [l]
            let rec kcont = function
                | (V_NBA(l, r),c) -> gec_V_CONT fv1 false (l, r)::c
                | (V_BLOCK l, c)  -> foldr kcont c l
                | (V_COMMENT l, c) -> V_SCOMMENT(l)::c
                | (V_IF(_), c)     -> sf("kcont other 2 'if'")
                | (V_IFE(_), c)    -> sf("kcont other 3 'ife'")
                | (other, c) -> sf("kcont other form in continuous block")
         

            let rr = if body=[] then cc 
                    else match clkmode with
                           | Some clk -> V_ALWAYS(V_BLOCK(V_EVC(v_evc_gen1 ww (rvu, pp, aliases) (pp X_true clk))::body))::cc
                           | None     -> (foldr kcont cc body)
            rr


        | (H2BLK(clkmode, SP_par(_, l)), c) -> 
               foldr (fun (x,c) -> xrtl_to_rtl  (WN "H2BLK" ww) fv1 (H2BLK(clkmode, x), c)) c l

        | (_, c) -> sf "394error" // end of not used code


// Render a VM as a structural Verilog component.  Generics (parameter overrides) come from attributes in ats.
let h2_as_rtl ww (iname, HPR_VM2(minfo, decls, [], execs, assertions)) =
    let p = { comb_delayo=None; vnl_synthesis=true; m_resetgrds=ref[]; resets="none"; gatelib=None; units=ref [];
              unitsd=new unitsd_t("unitsd");
              nets=ref []; clknet=None; rstnet=None; token="h2_as_rtl"; dp=new_cvtToRtl_DP() }
    let rvu = Some{ units=p.m_units; lon=1; loid="fornow"; }
    let modname = underscore_fold (minfo.name)
    let iname' = underscore_fold iname
    let contacts = map (eqnToL (WN "contacts" ww) (rvu, pp, aliases)) formals
    vprintln  3 ("Son HPR machine " + modname + " is structurally rendered.")
    let rides =
        let mineride aa cc =
            match aa with
                | Napnode("parameters", lst) -> sf "parameter rides present"
                | _ -> cc
        List.foldBack mineride minto.ats []
    V_INSTANCE(0, modname, iname', rides,  V_IA_positional contacts, minfo.atts)

#endif




(* 
 * Back-convert RTL to vbev for feeding into diosim simulator.
 *)
let rtl_presim ww (vlnv, l) (nets0, execs0) =
    let vd = 30
    let width = 132
    let jtrace kf = if true then () else ignore(WF 2 "rel_presim" ww (kf()))
    let ids = vlnvToStr vlnv
    let _ = WF 2 "Presim" ww (ids + " started")
    let R = { prefix="rtl_" }
        
    let jpb a c = (presim_bev R xi_true a)::c

    let m_enets = ref []
    let render_unit20 dD (aA, layout) cc =
        match aA with
            | V_INSTANCE(_, kind, iname, generics, ports, ats) -> 
                let keep = at_assoc "preserveinstance" ats
                let external = keep = Some("externally-provided") // cf "in-same-file"
                // If externally provided, we cannot roundtrip it from here, but we may have its original VM to shortcut tack back to the result.
                //jtrace (fun()->verilog_render_unit_x width dD aA + sprintf " presim start instance externally_provided=%A\n" external);
                let r = presim_gate1 R (kind.name, generics, ports)
                r::cc

            | V_ALWAYS(V_BLOCK((V_EVC ec)::lst)) -> 
                //jtrace (fun()->verilog_render_unit_x width dD aA + " presim start always\n")
                let rec s c = function
                   | V_EVC_OR(a, b) -> s (s c a) b
                   | V_EVC_POS clk -> E_pos(presim_exp R clk)::c
                   | V_EVC_NEG clk -> E_neg(presim_exp R clk)::c
                   | V_EVC_ANY clk -> E_any(presim_exp R clk)::c
                   | V_EVC_STAR    -> E_anystar::c
                let q = match s [] ec with
                         | E_anystar::_ -> None
                         | other -> Some other
                (gec_Xblock(List.foldBack jpb lst []), q)::cc

            | V_CONT(_, l, r) -> // delay ignored! todo
                //jtrace (fun()->verilog_render_unit_x width dD aA + " presim start cont\n")
                // this adds an X_x! : presim_bev R xi_true (V_NBA(l, r)), None)::c
                let ans = (Xassign(presim_exp R l, presim_exp R r), None)
                ans :: cc

            | V_PRAMDEF(V_PARAM(id, netatt) as l, None) -> cc

            | V_NETDECL(lhs, _, _, _, _, _, _, Some rhs) ->
                //jtrace (fun()->verilog_render_unit_x width dD aA + " presim start initialised reg def\n");
                let ans = (Xassign(lhs, presim_exp R rhs), None)
                //let _ = if netatt<>None then mutadd m_enets (X_bnet (valOf netatt)) -- correct?
                ans::cc
                

            | V_PRAMDEF(V_PARAM(id, netatt) as l, Some r) ->
                //jtrace (fun()->verilog_render_unit_x width dD aA + " presim start pramdef\n");
                let ans = (Xassign(presim_exp R l, presim_exp R r), None)
                // let ans = (presim_bev R xi_true (V_NBA(l, r)), None)
                if netatt<>None then mutadd m_enets (X_bnet (valOf netatt))
                ans::cc

            | V_ALWAYS(V_BLOCK l) ->             sf "presim: non-clocked RTL form: other2" 

            | V_ALWAYS(item) -> 
                let debugs = verilog_render_bev item 
                vprintln 0 (sprintf "Problem code region is:")
                app (vprintln 0) debugs
                sf("presim: non-clocked RTL form: item A ")

                 // A ROM is an RTL array filled by initial statements.
                 // A synchronous ROM will have a registered output or input.
            | V_INITIAL(item)  -> sf "presim: non-clocked RTL form: initial forms not done yet" 
            | V_SCOMMENT(item) -> cc // Why discard the comments?

            | other ->
                let debugs = verilog_render_unit_x width (g_null_ddd()) other
                vprintln 0 (sprintf "Problem code region is:")
                app (vprintln 0) debugs
                sf("presim: unexpected RTL form: " +  sprintf "%A" other)

    let j dD x =
        let rr = render_unit20 dD x
        jtrace (fun()->"finished")
        rr
            
    let tx = List.foldBack (j (g_null_ddd())) l [] // Collate keys
    let domains =
        let ds = List.fold (fun cc (s, clk) ->singly_add clk cc) [] tx
        ds
        
    let _:edge_t list option list = domains
    let rec collate clk = function
        | [] -> []
        | (s,c)::tt ->
            if c=clk then
                let ast_ctrl = { g_null_ast_ctrl with id=funique "cpp_ifshare" }
                (SP_l(ast_ctrl, s))::(collate clk tt)
            else collate clk tt

    reportx 3 "presim clock domains found"
                     (fun x -> if x=None then "None"
                               else sfold edgeToStr (valOf x)
                     ) domains
    let r = map (fun dom -> (dom, collate dom tx)) domains
    let execs = map (fun(clks, l) ->
        let dir = {g_null_directorate with clocks=valOf_or_nil clks; duid=next_duid() }
        H2BLK(dir, gen_SP_par PS_lockstep l)) r
    let assertions = []
    let decls =
        let cpi = { g_null_db_metainfo with kind= "rtl-presim-nets" }
        [DB_group(cpi, map db_netwrap_null !m_enets)]
    if vd>=5 then app (fun m -> vprintln 5 ("presim machine item=" + hblockToStr ww m)) execs
    let _ = WF 2 ("Presim of " + ids) ww "finished"
    let _ = unwhere ww
    (decls@nets0, execs@execs0) // end of rtl_presim


// vnl means Verilog net list. But we write out behavioural code too!
let vnl_writer_grp ww fv1 rvu asrf filename vtrips =

    let aux0 = [ sprintf "Layout wiring length estimation mode is %A." fv1.prefs.layout_enable ]

    let aux1 = [ " " ]
    let auxlist = aux1 @ aux0
    let fvv = (fv1.pagewidth, fv1.ddctrl)

    let vmodules = map f3o3 vtrips

    //let reset_net = (g_bool_prec, reset_net()) // Auto-adding this here requires matching actions in other output drivers and diosim, so is a little unpleasant.
        // The reset style (synch/asynch) is controlled by a flag to vnl render, so having a reset auto-added here makes a little sense.
        // Also, many CV_ blocks in our library want a reset input.
        // Add a set of directorate nets, according to use preferences to scale, to every output.

    let directorate_nets pp =  
        let aliases = []
        let ww = WN "eqnToL-directorate-net421" ww
        //dev_println (sprintf "verilog_gen: directorate-vg-dir: using %s as reset rst_net." (netToStr (snd reset_net)))
        let hnets = [ ] // reset_net autoadd
        let biz (prec, net) =
            //dev_println (sprintf "wrap actual 6/2 " + netToStr net)
            VDB_actual(None, eqnToL fv1 ww (rvu, pp, aliases) prec net)
        gec_VDB_group({g_null_db_metainfo with kind="directorate-vg-dir"; pi_name=funique "directorate" }, map biz hnets)

    let write_rtl_file (_, pp, ((rid:vm2_iinfo_t), filename, hh_level, (aliases, newpramdefs, gv_nets, lv_nets), items)) =
        let ww' = (WN "HPR L/S write_rtl_file" ww)
        let name = rid.vlnv.kind
        let gv_nets = list_once (gv_nets @ directorate_nets pp) // reset added here  - painful site 2      
        vprintln 2 (sprintf "Write RTL module named %s to file %s" (hptos rid.vlnv.kind) filename)
        rtl_output2 ww' fv1.ddctrl filename fv1.timescale auxlist fv1.add_aux_reports [(pp.netinfo_dir, asrf, hptos name, fvv, hh_level, (aliases, newpramdefs, gv_nets, lv_nets), items)]
        () 

    let todecl__ arg cc =
        match arg with
            | V_NET (ff, id, n) -> V_NET(ff, id, n)::cc

            | V_NUM _ ->
                //vprintln 3 (sprintf "A comment elsewhere explains why these arise V_NUM LHS (from ROMs)")
                cc
            | other -> sf (sprintf "todecl other form %A" other)

    let todecl a cc = a::cc

    let _ = WF 2 "compile_to_xrtl_or_rtl" ww (sprintf "write RTL to file starting filename=%s  " filename +  " at Timestamp: " + timestamp true)
    if fv1.separate_files // if each as own file
    then app write_rtl_file vtrips
    elif length vmodules = 0 then ()
    else
        let filename = f2o5(hd vmodules)
        let ww' = (WN "verilog_gen: write_rtl" ww)                

        let kickq(_, pp, ((rid:vm2_iinfo_t), filename, hh_level, (aliases, newpramdefs, gv_nets, lv_nets), items)) =
                        let gv_nets = list_once (gv_nets @ directorate_nets pp) // reset added here - painful site 1
                        let lv_nets = list_once (lv_nets)
                        vprintln 2 (sprintf "Write RTL module named %s as part of file %s" (hptos rid.vlnv.kind) filename )
                        //vprintln 0 (sprintf "michstart lv_nets = %s" (List.foldBack vdbToStr lv_nets ""))
                        //vprintln 0 (sprintf "lv_nets are " + sfoldcr_lite (fun x -> x.ToString()) lv_nets)
                        (pp.netinfo_dir, asrf, hptos rid.vlnv.kind, fvv, hh_level, (aliases, newpramdefs, List.foldBack todecl gv_nets [], List.foldBack todecl lv_nets []), items)
        rtl_output2 ww' fv1.ddctrl filename fv1.timescale  auxlist fv1.add_aux_reports (map kickq vtrips)
        ()
    ()


let backconvert ww fv1 roundtrip_bypass filename (orig_design, pp, vmodule) = 
        // Now presim - convert back to hexp form.
    if roundtrip_bypass
    then
        vprintln 2 ("Lift (convert) back to VM from RTL disabled (presim bypassed)")
        orig_design
    else
            let phasename = "rtl-presim"
            let _ = establish_log false ("rtl-presim")
            let ww_p = WF 2 phasename ww "starting"
            let fnets = []

            let gdecls =
                let cpi = { g_null_db_metainfo with kind= "rtl-control-nets" }
                [DB_group(cpi, map db_netwrap_null (singly_add g_clknet fnets))]               // Add clocknet back into lifted design - but twice?

            let ldecls =
                let cpi = { g_null_db_metainfo with kind= "rtl-synth-nets" }
                [DB_group(cpi, map db_netwrap_null !pp.m_nets)]
            let id1 = [ filename ]
            let ii = { fst orig_design with generated_by=phasename }
            let minfo' = { g_null_minfo with name=ii.vlnv }
            let (nets, rtl_design) = List.foldBack (fun (rid, filename, _, _, items)-> rtl_presim ww_p (minfo'.name, items)) [vmodule] ([], [])
            let _ = WF 2 "RTL presim" ww "complete"

            //We could preserve VM hierarchy in Verilog future, converting each machine to an RTL module, but currently cvtToVerilog (mostly) flattens for us.
            let children' = []

            //reportx 0 "fnets" netToStr fnets
            let ans = (ii, Some(HPR_VM2(minfo', gdecls@ldecls@nets, children', rtl_design, [])))
            ans

(*
 * Write out as RTL (allowable forms only)
 * This will build to gates too, so not a pure output generator under current code partitioning.
 *
 * The VM's tagged with preserve instance are output as separate RTL modules or instances of externally-provided RTL modules.  Instances can be internal or external. Those without preserve instance are all coallesced and output according to recipe settings or preffered name attributes.
 *)
let opath_rtl_write_vm ww op_args vms = // aka opath_verilog_gen aka vnl
    let control = op_args.c3
    let enable = (control_get_s op_args.stagename control op_args.stagename (Some "disable")) = "enable"
    if not enable then vms
    else
        let shortname = "vnl"
        let roundtrip_bypass = hd(arg_assoc_or_fail "verilog-gen:opath_rtl_writer" ("vnl-roundtrip", control))="disable"
        let separate_files = (control_get_s op_args.stagename control "vnl-separate-files" (Some "enable")) = "enable"
        let suffix = ".v"
        let who = "verilog-gen:opath_rtl_writer"

        let filename0 = arg_assoc "vnl" who control
        let filename00 = arg_assoc "vnl-default-name" who control
        let root0 = arg_assoc "vnl-rootmodname" who control

        g_vnl_loglevel := max !g_global_vd (control_get_i control "vnl-loglevel" 1)
        let gatelib = hd(arg_assoc_or_fail who ("vnl-gatelib", control) )
        let gatelib = if gatelib = "" || gatelib = "NONE" then None else Some gatelib
        let ps = (control_get_s op_args.stagename control "vnl-preserve-sequencer" (Some "false")) = "true"
        let ddctrl =
            { g_null_ddctrl with
                  uniquify_all_functions=control_get_s op_args.stagename control "vnl-uniquify_all_functions" (Some "enable") = "enable"

              // TODO delete old k and r syntax (pre Verilog 2001).
                  kandr=               control_get_s op_args.stagename control "vnl-kandr" (Some "enable") = "enable"
            }

        let gbuild_prefs = get_gbuild_recipe_prefs op_args.stagename shortname control

        let disable_subexpression_sharing = (1 = (cmdline_flagset_validate "vnl-subexpression-sharing" ["enable"; "disable"] 0 control))        
        let subex_sharecost_threshold = control_get_i control "vnl-subexpression-share-threshold" 20

        vprintln 2 (sprintf "Subexpression sharing: enabled=%A    cost=%i" (not disable_subexpression_sharing)               subex_sharecost_threshold)
        let fv1:FV_t =
            {
              ddctrl =             ddctrl 

              //m_topused=           ref false
              add_aux_reports=     control_get_s op_args.stagename control "vnl-add-aux-reports" (Some "disable") = "enable"          
              pagewidth=           control_get_i control "pagewidth" 132
              prefs=               gbuild_prefs

              separate_files=      separate_files // A global setting here conflicts with the attribute ii.in_same_file - TODO reconcile.
              vd=                  !g_vnl_loglevel
              preserve_sequencer=  ps
              comb_delayo=         control_get_io control "vnl-comb-assignment-delay"
              timescale=           control_get_s op_args.stagename control "vnl-timescale" (Some g_default_timescale)

              subex_sharecost_threshold=               subex_sharecost_threshold
              disable_subexpression_sharing=           disable_subexpression_sharing

              //output_file_root=    output_file_root
              output_file_suffix=  suffix
              ip_xact_filestyle=   control_get_s op_args.stagename control "vnl-ip-xact-style" (Some "component") 
            }

        let asrf = None // not used: if resets="asynchronous" then Some(xi_blift(reset())) else None


        let rvu = Some (create_rvu "TOP" None) // Shared over all?
        
        let vnl_serf design =
            let preferred_name = vm_att_assoc g_preferred_name design // This is not needed now vlnv is passed in
            vprintln 1 ("Any preferred name from VM attributes? " + (if preferred_name = None then "None" else valOf preferred_name))


            let san = filename_sanitize [ '+'; '-'; '_'; ]


            let filename1 =
                if filename0 <> [] && filename0 <> [ "" ] then hd filename0
                elif not_nonep preferred_name then san(valOf preferred_name)
                elif filename00 <> [] then san(hd filename00)
                else "DUT"

            let output_file_root = fst(strip_suffix filename1)

            let filename = output_file_root  + suffix

            let filename =
                let where = !g_obj_dir
                if where <> "." then path_combine(where, filename) else filename


            let (input_ii, topdesign) =
                match design with
                    | (ii, vm2) -> ((ii:vm2_iinfo_t), (ii, vm2)) // Here we did previously insert an hh_level of 0

            let root_vlnv =
                let rootname = if root0 <> [] && root0 <> [ "" ] then hd root0 else output_file_root
                { input_ii.vlnv with kind=[rootname]; version="1.0" }

            vprintln 3 ("Output filename         = " + filename)
            vprintln 3 ("Output root module name = " + vlnvToStr_full root_vlnv)    

            let result_ii =
                { input_ii with // g_null_vm2_iinfo with
                    definitionf= true
                    vlnv=root_vlnv
                    iname=hptos root_vlnv.kind // iname not relevant for a definition.
                }
            let m_root_name_used = ref false
            let ans = cvtToVerilog (WN "orangemain: compile_to_xrtl_or_rtl: cvtToVerilog" ww) ("HPR ", fv1, control) (m_root_name_used, result_ii, output_file_root, filename) topdesign
            vnl_writer_grp ww fv1 rvu asrf filename (ans)
            let _ = WF 2 "compile_to_xrtl_or_rtl" ww ("write RTL to file finished")

            (map (backconvert ww fv1 roundtrip_bypass filename) ans)


        vprintln 2 (sprintf "Processed %i top-level VMs" (length vms))
        let new_ans = list_flatten(map vnl_serf vms)
        if roundtrip_bypass then
            vprintln 2 (sprintf "Return %i vms at top level, exactly as per input to recipe stage." (length vms))
            vms
        else
            vprintln 2 (sprintf "Did have %i VMs. Now return %i VMs at top level as new answer." (length vms) (length new_ans))
            new_ans

let install_verilog() =

    let stagename = "verilog-gen"
    let shortname = "vnl"
    let argpattern =
        [
          Arg_defaulting("vnl-rootmodname", "", "Root module name in output RTL file");
          Arg_defaulting("vnl", "", "Output file name for Verilog RTL");
          Arg_defaulting("vnl-timescale", g_default_timescale, "RTL timescale declaration string to included in RTL files.");
          Arg_defaulting("vnl-gatelib", "None", "Gate library (set to \"NONE\" for technology-independent RTL output)");
          Arg_enum_defaulting("verilog-gen", ["enable"; "disable"], "enable", "Enable control for this operation");


          Arg_enum_defaulting("vnl-subexpression-sharing", ["enable"; "disable"], "enable", "Subexpression sharing (via additional continuously assigned nets.");
          Arg_enum_defaulting("vnl-synthesis", ["enable"; "disable"], "enable", "Enable synthesisable xforms in output (e.g. div as rshift strength reduction"); // not here anymore?
          Arg_enum_defaulting("vnl-add-aux-reports", ["enable"; "disable"], "enable", "Insert report file as comment into bottom of Verilog file");

          Arg_enum_defaulting("vnl-ip-xact-style", ["disable"; "component"], "component", "Write IP-XACT description of generated module(s).");                    

          Arg_enum_defaulting("vnl-ifshare", [ "none"; "simple"; "on" ], "on", "Whether to gather up 'if' statement bodies (when possible)");
          Arg_enum_defaulting("vnl-preserve-sequencer", [ "true"; "false"; ], "true", "Whether to emit control sequencers in a case statement style (when possible)");
          
          Arg_int_defaulting("vnl-loglevel", 1, "Trace level: higher enables further tracing:  (10 or is maximum. Default is 1.)");

          Arg_enum_defaulting("vnl-uniquify_all_functions", ["enable"; "disable"], "disable", "RTL functions can generally be reused if they are stateless.  In new Verilog the automatic keyword should be used to make functions that appear stateless to be stateless.  Some tools may still confuse separate calls to a common function so this flag is provided to force all functions to be used only once textually.");
          Arg_enum_defaulting("vnl-kandr", ["enable"; "disable"], "disable", "Enable Kernighan and Ritchie style (pre-2001) Verilog port syntax");

          
          Arg_enum_defaulting("vnl-roundtrip", ["enable"; "disable"], "enable", "Convert generated Verilog back to internal VM form for further processing");

          Arg_int_defaulting("pagewidth", 132, "Page column width for pretty printer");
          Arg_int_defaulting("vnl-comb-assignment-delay", -1, "Combinational delay to insert in each assignment (rather than use zero delay or estimated models)");
          Arg_int_defaulting("vnl-subexpression-share-threshold", 20, "Logic cost of a replicated subexpression to extract via a continuously-assigned pin.");                              

          Arg_enum_defaulting("vnl-separate-files", ["enable"; "disable"], "disable", "Write each Verilog modules to its own new file");
        ] @ rez_gbuild_recipe_prefs stagename "vnl"

    let _ = install_operator ("verilog-gen", "Generate Verilog (allowable forms only)", opath_rtl_write_vm, [], [], argpattern)
    ()

// Make appear as a loadable plugin
type Verilog_gen() =
    interface IOpathPlugin with
        member this.OpathInstaller() = install_verilog()
        member this.MethodName = "install_verilog"

(* eof *)
