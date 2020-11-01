// 
// cil asmout.sml - write out an IL file (used also in the pushlogic compiler backend).
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
// NB: the source version of this file is shared between projects but not its .uo form.
//

module asmout

open cilgram
open yout
open moscow



(* Functions to convert cilgram.sml constructs to valid strings for ilasm parsing *)

let g_globalname = "-GLOBALS"  // What is this for ? Not used anymore?


exception IexpToStr;

let sclToStr = function
    | CV_formal n -> "formal-" + i2s n
    | CV_local n  -> "local-" + i2s n
    | CV_sfield   -> "sfield"
    | CV_heap    -> "field"
    | CV_none     -> "CV_none"
    | CV_object   -> "object"
    | CV_mutex    -> "mutex"
    | CV_const    -> "const"
    | CV_brand s  -> "brand+" + s


let cil_flagToStr = function
    | Cilflag_extern -> "extern"
    | Cilflag_auto -> "auto"
    | Cilflag_interface -> "interface"
    | Cilflag_virtual -> "virtual"
    | Cilflag_family -> "family"
    | Cilflag_final -> "final"
    | Cilflag_abstract -> "abstract"
    | Cilflag_private -> "private"
    | Cilflag_field -> "field"
    | Cilflag_public -> "public"
    | Cilflag_initonly -> "initonly"
    | Cilflag_cil -> "cil"
    | Cilflag_managed -> "managed"
    | Cilflag_hidebysig -> "hidebysig" (* Ignored by the runtime *)
    | Cilflag_specialname -> "specialname"
    | Cilflag_rtspecialname -> "rtspecialname"
    | Cilflag_instance -> "instance"
    | Cilflag_default -> "default"
    | Cilflag_ansi -> "ansi"
    | Cilflag_static -> "static" // static is aka Cilflag_class 
    | Cilflag_pinned -> "pinned"
    | Cilflag_enum -> "enum"    
    | Cilflag_valuetype -> "valuetype"
    | Cilflag_strict -> "strict"
    | Cilflag_literal -> "literal"
    | Cilflag_assembly -> "assembly"
    | Cilflag_sequential -> "sequential"
    | Cilflag_serializable -> "serializable"
    | Cilflag_sealed -> "sealed"
    | Cilflag_nested -> "nested"
    | Cilflag_partial -> "partial"
    | Cilflag_hprls -> "hprls"     // KiwiC-specific.
    | Cilflag_hpr s -> "hpr:" + s  // hpr library primitive
    | Cilflag_explicit -> "explicit"
    | Cilflag_beforefieldinit -> "beforefieldinit"
    | Cilflag_unsafe -> "unsafe"
    | Cilflag_runtime -> "runtime" 

    | other -> sprintf "flagToStr??%A" other 


let iexpToChar = function
    | Cil_number32 n when n >= 32 && n < 127 -> sprintf("%c") (System.Convert.ToChar n)
    | _ -> "."
    

let rec iexpToStr aA =
    match aA with 
    | Cil_id s -> s
    | Cil_string ss -> "\"" + ss + "\""
    | Cil_float(n, s) -> sprintf "Float%i:%s" n s
    | Cil_int_i(w, bn) -> sprintf "%A" bn
    | Cil_float32 f32 -> sprintf "%A" f32
    | Cil_float64 f64 -> sprintf "%A" f64
    | Cil_number32 n -> i20x n
    | Cil_tnumber(n, v) ->
            let hex3 n = (sprintf "%03x" n)
            let rec k = function
                | (Cil_tnumber(n, v)) -> k v + "_" + hex3 n
                | Cil_tnumber_end n -> if n then "-" else "+"
                | _ -> raise IexpToStr
            k aA
     | Cilexp_caster(t, e) -> cil_typeToStr t + "(" + iexpToStr e + ")"
     | Cil_dotpath(s, tag) -> iexpToStr s + "." + tag
     | Cil_explist l ->
         "[[" + foldr (fun(x,c) -> iexpToStr x + " " + c) "]]" l

     | Cil_blob lst ->
         "blob(" + List.foldBack (fun x c -> iexpToStr x + " " + c) lst ")" +
         "(\"" + List.foldBack (fun x c  -> iexpToChar x + c) lst "\")" + (sprintf " // size %i bytes\n" (length lst))
     | Cil_nativetypedata l -> "nativetypedata??"
     | other -> "??iexpToStr"



// (* TODO: This is the place to look at e and generate short form instructions ... *)
and iargToStr = iargToStr1 "." 

and iargToStr1 prefix = function    
    | Cil_argexp(Cil_number32 n) when prefix <> "." -> prefix + sprintf "%i" n
    | Cil_argexp e -> " " + iexpToStr e
    | (Cil_suffix_0) -> prefix + "0"
    | (Cil_suffix_1) -> prefix + "1"
    | (Cil_suffix_2) -> prefix + "2"
    | (Cil_suffix_3) -> prefix + "3"
    | (Cil_suffix_4) -> prefix + "4"
    | (Cil_suffix_5) -> prefix + "5"
    | (Cil_suffix_6) -> prefix + "6"
    | (Cil_suffix_7) -> prefix + "7"
    | (Cil_suffix_8) -> prefix + "8"
    | (Cil_suffix_m1) -> ".m1"
    | (Cil_suffix_M1) -> ".M1"
    | (_) -> "??iargToStr"


and cil_targToStr = function
    | Cil_tp_arg(methodf, n, s)   -> s + "/" + (if methodf then "!!" else "!") + i2s n
    | Cil_tp_type t  -> cil_typeToStr t

and cil_classrefToStr x = cil_typeToStr x



and cgr1ToStr (s, x) = s + "=" + sprintf "%A" x 

and dtToStr = typeinfoToStr // too many aliases - delete them..

and typeinfoToStr = cil_typeToStr

and cgrToStr classgenerics = 
    "$(" + sfold cgr1ToStr (Map.toList classgenerics) + ")$"

and typeinfoToStr0__ (rn, dt, pos, size, fap) = // unused - could make dtable files look nicer?
    sprintf "   %s at pos=%i size=%i " (rnToStr rn) pos size  +
    (":" + typeinfoToStr dt + (if nullp fap then "" else "<<" + sfold typeinfoToStr fap + ">>"))

and
   fieldtag_infoToStr1 crt = crt.ptag + " at pos=" + i2s64 crt.pos + " size=" + i2s64 crt.size_in_bytes + (":" + typeinfoToStr crt.dt + (if nullp crt.fap__ then "" else "<<" + sfold typeinfoToStr crt.fap__ + ">>"))


and drToStr = function
    | RN_monoty(ty, ats)     -> cil_typeToStr ty + (if nullp ats then "" else sprintf "ats=^%i" (length ats))
    | RN_duplex (a, b, ptag) -> sprintf "RN_duplex(%s, %s, %s)" (drToStr a) (drToStr b) ptag
    | RN_call(class_dt, rt, ga, args, [], []) -> sprintf "RN_call(^%i:%s->%s)" (length args) (sfold drToStr args) (drToStr rt)
    | RN_call(class_dt, rt, ga, args, cgr, mgr) -> sprintf "RN_call<%s><%s>(^%i:%s->%s)" (sfold drToStr cgr) (sfold drToStr mgr) (length args) (sfold drToStr args) (drToStr rt)    
    | other  -> sprintf "drToStr??%A" other

and cil_typeToStr = function
    | Cil_cr s -> s // + ".p"
    | Cil_cr_idl l         -> hptos l // + ".q"
    | Cil_cr_flag(flag, t) -> "(" + cil_flagToStr flag + " " + cil_typeToStr t + ")"
    | Cil_lib(assembly_name, cr)   -> sprintf "[%s]" (cil_classrefToStr assembly_name) + cil_classrefToStr cr
    | Cil_namespace(s,cr) -> "[" + cil_classrefToStr s + "]" + cil_classrefToStr cr// + ".s"
    | Cil_cr_dot(cr, e)   -> cil_classrefToStr cr + "." + e
    | Cil_cr_slash(cr, e) -> cil_classrefToStr cr + "/" + e
    | Cil_cr_templater(cr, args) -> cil_classrefToStr cr + (if nullp args then "" else "<"  + sfold cil_targToStr args + ">")
    | Cil_cr_global      -> g_globalname


    | CTL_vt(K_int, ww, _)   -> sprintf "int%i" ww
    | CTL_vt(K_uint, ww, _)  -> sprintf "unsigned int%i" ww
    | CTL_vt(K_float, ww, _) -> sprintf "float%i" ww
 
    | CTL_reflection_handle sl -> sprintf "CTL_handle(%s)" (sfold id sl)
    | CTL_void   -> "CTL_void"  // CTL forms are final forms in the main IR.
    
    | Ciltype_ellipsis -> "..."
    | Ciltype_int      -> "int"
    | Ciltype_decimal  -> "decimal"    
    | Ciltype_bool     -> "bool"
    | Ciltype_char     -> "char"
    | Ciltype_string   -> "string"

   
    | Cil_suffix_i -> ".i"
    | Cil_suffix_ref -> ".ref"
    | (Cil_suffix_i1) -> ".i1"
    | (Cil_suffix_i2) -> ".i2"
    | (Cil_suffix_i4) -> ".i4"
    | (Cil_suffix_i8) -> ".i8"
    
    
    | (Cil_suffix_r4) -> ".r4"
    | (Cil_suffix_r8) -> ".r8"
    
    | (Cil_suffix_u) -> ".u"
    | (Cil_suffix_u1) -> ".u1"
    | (Cil_suffix_u2) -> ".u2"
    | (Cil_suffix_u4) -> ".u4"
    | (Cil_suffix_u8) -> ".u8"



    | Ciltype_modopt(a, b) -> cil_typeToStr a + " modopt " + cil_classrefToStr b
    | Ciltype_modreq(a, b) -> cil_typeToStr a + " modreq " + cil_classrefToStr b
    

    | Ciltype_array(a, None)    -> cil_typeToStr a + "[]"
    | Ciltype_array(a, Some n)  -> cil_typeToStr a + "[" + i2s n + "]"
    | Ciltype_star a            -> cil_typeToStr a + "*"
    | Ciltype_ref a             -> cil_typeToStr a + "&"
    | CTL_record(idl, cst_, l, len, ats, binds, srccode_o) -> 
        let token = if cst_.structf then "CTL_struct" else "CTL_record"
        let bindToStr (fid, ty) = fid + ":" + typeinfoToStr ty
        let lb = length binds
        if lb > 0 then sprintf "%s(%s, %s, binds^%i=%s)" token  (hptos idl) (sfold fieldtag_infoToStr1 l) lb (sfold bindToStr binds)
        else sprintf "%s(%s, ...)" token (hptos idl) 

    | CT_class cdt -> sprintf "Class(%s, ...)" (hptos cdt.name)

    | CT_cr cr ->
        let fodet =
            match cr.crtno with
                | CT_class cst -> if cst.structf then "structsite" else "cr" // A class reference or the actual site of a structure.
                | _ -> "structsite_or_cr"
        if nullp cr.cl_actuals && nullp cr.meth_actuals then
            sprintf "CT_%s(%s)" fodet (hptos cr.name)
        else
           sprintf "CT_%s(%s, <%s><%s>)" fodet (hptos cr.name) (sfold drToStr cr.cl_actuals) (sfold drToStr cr.meth_actuals)
    
    | CT_arr(dct, None)    -> sprintf "CT_arr(%s, <unspec>)" (typeinfoToStr dct)
    | CT_arr(dct, Some n)  -> sprintf "CT_arr(%s, %i)" (typeinfoToStr dct) n
    //| CT_valuetype dt      -> "valuetype:" + typeinfoToStr dt
    //| CT_brand(scl, dt)    -> sclToStr(scl) + "+" + typeinfoToStr dt
    | CT_star(n, a) ->
        if n = 0 then sprintf "$$STAR0(%s)" (typeinfoToStr a)
        else
        // Positive stars in types are here denoted with ampersands.  This follows the printing convention of expressions (CE_star) but 
        // it may be more sensible to invert the polarity for types following the C 'cancelling' convention where,
        // for example, * applied to an int * yields an int.
            let rec j n = if n=0 then "" elif n > 0 then "&" + j(n-1) else "*" + j(n+1) 
            (j n) + "(" + typeinfoToStr a + ")"

    | CT_knot(idl, whom, vv) -> sprintf "CT_knot(%s, %s, %s)" (hptos idl) (whom) (if nonep !vv then "<untied>" else "tied ...")
        
    | CTL_net(volf, w, signed, ats) ->
        let astr =
            (if ats.nat_char then ", CHAR" else "") +
            (if ats.nat_string then ", STRIN" else "") +
            (if ats.nat_native then ", native" else "") +
            //(if ats.xnet_io <> hprls_hdr.LOCAL then sprintf ", %A" ats.xnet_io else "") +
            (if ats.nat_hllreset then ", HLL_RESET" else "") +            
            (if not_nonep ats.nat_enum then sprintf ", ENUM" else "") +
            (if not_nonep ats.nat_IntPtr then sprintf ", IntPtr(%s)" (cil_typeToStr (valOf ats.nat_IntPtr)) else "")            
        "CTL_net(" + boolToStr volf + ", " + i2s w + ", " + meox.sftToStr signed + astr + ")"

    | CTVar vdata ->
        let a = if vdata.methodf then "!!" else "!"
        let b = if nonep vdata.id then "" else valOf vdata.id
        let c = if nonep vdata.idx then "" else i2s(valOf vdata.idx)
        let f = if b<>"" && c <>"" then "_" else ""
        a + b + f + c

    | CT_list lst -> sprintf "CT_list^%i(%s)" (length lst) (sfold typeinfoToStr lst)

    | CT_method mst ->
        if nullp mst.ty_formals then "Method:" + hptos mst.name
        else sprintf "%s<%s>" (hptos mst.name) (sfold cil_typeToStr mst.ty_formals)

//  | CT_structsite

    | other -> sprintf "cil_type??%A" other



and safe_typeinfoToStr = function
    | other -> typeinfoToStr other

and cil_safe_typeToStr T =
    match T with
        | (Ciltype_modreq _) -> sf("unsafe typeToStr : " + cil_typeToStr T )
        | other -> cil_typeToStr other

and cil_safe_classrefToStr = function
    | x->x


and rnToStr = function
    | RN_idl idl -> hptos idl
    | RN_type ct -> "RN_type(" + safe_typeinfoToStr ct + ")"
    | (_) -> "rntoStr??"





let rec cil_flagsToStr = function
    | [] -> ""
    | [item] -> cil_flagToStr item
    | (h::t) -> cil_flagToStr h + " " + cil_flagsToStr t


let cil_fieldrefToStr = function
    | (cr, id, []) ->  sprintf "%s::%s" (cil_classrefToStr cr) id
    | (cr, id, generics) ->  sprintf "%s::%s%s" (cil_classrefToStr cr) id (if nullp generics then "" else "<" + sfold cil_typeToStr generics + ">")



let cilipToStr = function
    // Instruction Prefixes:
    | Cilip_unaligned -> "unaligned. "
    | Cilip_volatile -> "volatile. "
    | Cilip_readonly -> "readonly. "
    | Cilip_tail -> "tail. "
    | Cilip_constrained(ct, fr_) ->"constrained " + cil_typeToStr ct

let rec ciliToStr = function
    // Instruction Prefixes:
    | Cili_prefix pfix -> cilipToStr pfix
    
    | Cili_lab s -> s + ":"
    | Cili_add s -> "add"  + (if s=Cil_unsigned then ".un " else " ") // signed is only on overflow form
    | Cili_ret -> "ret "


    | Cili_nop -> "nop "
    | Cili_switch args -> "switch(" + (foldr (fun (x,c)->x + (if c="" then "" else ", ") + c) "" args)   + ")"
    | (Cili_comment s) -> "// " + s
    | Cili_sub s -> "sub"  + (if s=Cil_unsigned then ".un " else " ") // signed is only on overflow form
    | Cili_mul s -> "mul"  + (if s=Cil_unsigned then ".un " else " ")  // signed is only on overflow form
    | Cili_div s -> "div"  + (if s=Cil_unsigned then ".un " else " ")
    | Cili_rem s -> "rem"  + (if s=Cil_unsigned then ".un " else " ")
    | (Cili_dup) -> "dup "
    | (Cili_pop) -> "pop "
    | (Cili_and) -> "and "
    | (Cili_or)  -> "or "
    | (Cili_xor) -> "xor "
    | Cili_not     -> "not "
    | Cili_shl s   -> "shl" + (if s=Cil_unsigned then ".un " else " ")
    | Cili_shr s   -> "shr" + (if s=Cil_unsigned then ".un " else " ")
    | Cili_neg     -> "neg "
    | Cili_throw   -> "throw "

    | Cili_kiwi_trystart(token, catchers, (fbo, _)) -> sprintf "Cili_kiwi_trystart(%s, ^%i, %s)" token (length catchers) (if nonep fbo then "None" else i2s(valOf fbo))
    | Cili_kiwi_special(cmd, exactf, dt) -> sprintf "KiwiSpecial %s %A %s" cmd exactf (cil_typeToStr dt)
    | Cili_isinst(false, cr) -> "isinst "  + cil_classrefToStr cr
    | Cili_isinst(true, cr) -> "isinst.exact "  + cil_classrefToStr cr       
    | (Cili_ldobj cr) -> "ldobj "  + cil_classrefToStr cr
    | (Cili_stobj cr) -> "stobj "  + cil_classrefToStr cr
    | (Cili_box cr) -> "box "  + cil_classrefToStr cr
    | (Cili_unbox cr) -> "unbox "  + cil_classrefToStr cr
    | (Cili_box_any cr) -> "box.any "  + cil_classrefToStr cr
    | (Cili_unbox_any cr) -> "unbox.any "  + cil_classrefToStr cr
    
    | (Cili_ceq) -> "ceq "
    | (Cili_cne) -> "cne "
    | (Cili_cle s) -> "cle" + (if s=Cil_unsigned then ".un " else " ")
    | (Cili_cge s) -> "cge" + (if s=Cil_unsigned then ".un " else " ")
    | (Cili_clt s) -> "clt" + (if s=Cil_unsigned then ".un " else " ")
    | (Cili_cgt s) -> "cgt" + (if s=Cil_unsigned then ".un " else " ")
    
    | (Cili_endfinally) -> "endfinally"
    | (Cili_endfault) -> "endfault"
    | (Cili_leave l) -> "leave " + l 

    | Cili_brfalse([], l) -> "brfalse " + l
    | Cili_brfalse(scl, l) ->  sprintf "brfalse.sc=%s destlab=%s" (sfold (fun x->x) scl) l

    | (Cili_brtrue l) -> "brtrue " + l 
    | (Cili_beq l) -> "beq " + l 
    | (Cili_bne l) -> "bne " + l 
    
    | (Cili_ble l) -> "ble " + l 
    | (Cili_bge l) -> "bge " + l 
    | (Cili_blt l) -> "blt " + l 
    | (Cili_bgt l) -> "bgt " + l 
    
    | (Cili_ble_un l) -> "ble_un " + l 
    | (Cili_bge_un l) -> "bge_un " + l 
    | (Cili_blt_un l) -> "blt_un " + l 
    | (Cili_bgt_un l) -> "bgt_un " + l 
    | Cili_br l       -> "br " + l 
    
    | Cili_stind t    -> "stind " + (cil_typeToStr t) 
    | Cili_ldind t    -> "ldind " +  (cil_typeToStr t)
    | Cili_ldloc v    -> "ldloc " +  (iargToStr1 " V_" v)
    | Cili_ldloca v   -> "ldloca " +  (iargToStr1 " V_" v)
    | Cili_ldarga v   -> "ldarga " +  (iargToStr v)
    
    | Cili_ldfld(dtype, fr)   -> "ldfld " +  cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    | Cili_ldflda(dtype, fr)  -> "ldflda " +  cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    | Cili_ldsfld(dtype, fr)  -> "ldsfld " +  cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    | Cili_ldsflda(dtype, fr) -> "ldsflda " +  cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    | Cili_stsfld(dtype, fr)  -> "stsfld " + cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    | Cili_stfld(dtype, fr)   -> "stfld " + cil_typeToStr dtype + " " + cil_fieldrefToStr fr
    
    
    | Cili_ldstr ss   -> "ldstr \"" + ss + "\""
    | Cili_call(callmode, kind, rtype, fr, signat, site) ->
        let opcode = if callmode = KM_virtual then "callvirtual" else "call"
        opcode + " " + cil_ckToStr kind + " " +  cil_typeToStr rtype + " " + cil_fieldrefToStr fr  + cil_signatureToStr signat
    
    
    
    | Cili_stloc  v  -> "stloc " + (iargToStr1 " V_" v) 
    | Cili_stelem t  -> "stelem " + (cil_typeToStr t) 
    | Cili_ldarg v   -> "ldarg " + (iargToStr v) 
    | Cili_starg v   -> "starg " + (iargToStr v) 

    | Cili_cpblk     -> "cpblk"
    | Cili_arglist   -> "arglist"    
    
    | Cili_ldc(t, v) -> "ldc " + cil_typeToStr t + " " + iargToStr v 

    | Cili_castclass cr -> "castclass " + cil_classrefToStr cr

    | Cili_newobj(flags, ck, t, fr, signat, site) ->
         "newobj " + cil_flagsToStr flags + " " + ck + " " + cil_typeToStr t + " " + (cil_fieldrefToStr fr) + cil_signatureToStr signat
    | Cili_newarr cr  -> "newarr " + (cil_classrefToStr cr)
        
    | Cili_ldftn(virt_flag, ck, tt, fr, signat, site) ->
        let code = if virt_flag then "ldvirtftn " else "ldftn "
        code + ck + " " + (cil_typeToStr tt) + " " + cil_fieldrefToStr fr + cil_signatureToStr signat

    | Cili_ldnull       -> "ldnull"

    | Cili_refanyval cr -> "refanyval " + cil_classrefToStr cr
    | Cili_ldtoken(tt, None) -> "ldtoken type/class  " + cil_typeToStr tt
    | Cili_ldtoken(tt, Some fr) -> "ldtoken " + (cil_typeToStr tt) + " " + cil_fieldrefToStr fr    
    
    | Cili_initobj cr   -> "initobj " + cil_classrefToStr cr 
    
    | Cili_ldlen        -> "ldlen"
    | Cili_ldelem t     -> "ldelem " + cil_typeToStr t 
    | (Cili_ldelema t)  -> "ldelema " + cil_classrefToStr t 
    
    | Cili_conv(t, false)      -> "conv" + (cil_typeToStr t) 
    | Cili_conv(t, true)       -> "conv.ovf" + (cil_typeToStr t) 


    | other -> sprintf "??ciliToStr=%A" other

and cil_callargToStr = function
    | ((None, tt))           -> cil_typeToStr tt
    | ((Some pramname, tt))  -> cil_typeToStr tt + sprintf " \"%s\"" pramname

and cil_signatureToStr l =

    let rec f = function
        | []     -> ""
        | [item] -> cil_callargToStr item        
        | h::t   -> cil_callargToStr h + ", " + f t
    "(" + f l + ")"

and cil_formalsToStr l =
    let rec f  = function
        | [] -> ""
        | Cil_fp(rpc_bind, idxo, dt, s)::tt ->
            (if rpc_bind=None then "" else sprintf "%s " (valOf rpc_bind)) +
            (if idxo=None then "" else sprintf "[%i] " (valOf idxo)) +                 
            cil_typeToStr dt + " " + s + 
            (if not_nullp tt then ", " else "") + f tt
    "(" + f l + ")"


let cil_actualsToStr l = 
    let rec f = function
        | [] -> ""
        | a::tt -> (iexpToStr a) + (if not(nullp tt) then ", " else "") + f tt
    "(" + f l + ")"




let cil_field_atToStr = function
    | Cil_field_none -> ""
    | Cil_field_at e ->  " at " + iexpToStr e
    | Cil_field_lit e ->  " = " + iexpToStr e

let cil_method_flagsToStr flags1 =
           (if flags1.is_startup_code then ", STARTUP" else "") 


// The field attributes are not inserted by the parser, but collated and inserted in a later rewrite.
// The ff bool states whether to print the contents of items or just their declarations.
let rec classitemToStr pad ff item =
    match item with
    | CIL_field(abound, flags, ctype,  cr, atclause, attributes) ->
        let ans =
              pad + ".field " + 
              (if nonep abound then "" else "[" + iexpToStr(valOf abound) + "] ") +
              cil_flagsToStr flags + " " + cil_typeToStr ctype + " " + cil_classrefToStr cr +
              cil_field_atToStr atclause 
        let ans = (if not_nullp attributes then ans + " ATTS=(" + (List.foldBack (cil_classitemToStr pad) attributes "") + ")" else ans)
        (if ff then ans + "\n" else "") 
        
    | CIL_mresource(flags, cr, items) -> pad + ".mresource " + cil_flagsToStr flags + " " + cil_classrefToStr cr + "\n" + "{\n" + foldr (fun(x,c)->"  " + cil_classitemToStr "" x "" + "\n" + c) "" items + "}\n\n"


    | CIL_publickeytoken e -> pad + ".publickeytoken " + iexpToStr e

    | CIL_ver(Cil_version_no(a,b,d,e))  -> pad + ".ver " + i2s a + ":"  + i2s b +  ":" + i2s d + ":"  + i2s e

    | CIL_custom(ck0, t, fr, signat, vale) -> 
           pad  + ".custom " +  ck0 + " " + cil_typeToStr t + " " + cil_fieldrefToStr fr +
           cil_signatureToStr signat + (if ff then " " + iexpToStr vale + "\n" else "")

    | CIL_setget(opcode, ck, flags, tt, fr, signat) -> 
           pad + opcode +  " " + ck + " " + cil_flagsToStr flags  + " " + cil_typeToStr tt + " " + cil_fieldrefToStr fr + 
           cil_formalsToStr signat + "\n"         

    | CIL_hash e  -> pad + ".hash algorithm " + iexpToStr e

    //| CIL_instruction(s)    -> pad +  "      " + ciliToStr s + "\n"
    | CIL_instruct(n, try_nesting, prefixes, i)    ->
        let tries = if nullp try_nesting then "" else "tries=" + sfold id try_nesting + "   "
        tries + pad + sprintf "    %s %s : %s\n" (if n >=0 then i2s n else " ") (if nullp prefixes then "" else sfold cilipToStr prefixes) (ciliToStr i)
     

    | CIL_comment s         -> pad + "// " + s + "\n"
    | CIL_maxstack e        -> pad + ".maxstack " + iexpToStr e + "\n"
    | CIL_param(n, None)    -> pad + ".param [" + i2s n + "]\n"
    | CIL_param(n, Some e)  -> pad + ".param [" + i2s n + "] " + iexpToStr e + "\n"
    | CIL_size e            -> pad + ".size " + iexpToStr e + "\n"
    | CIL_entrypoint ss     -> pad + ".entrypoint " + ss + "\n"
    
    | CIL_locals(lst, initf) ->
          let kl arg cc =
              match arg with
              | P3(no, a, b) -> pad + "  " + (if no=None then "" else "[" + i2s(valOf no) + "] ") + cil_typeToStr a + " " + b + (if cc="" then "" else "\n" + cc)
          pad + ".locals " + (if initf then "init " else "") + "(" +  List.foldBack kl lst "" +  pad + ")\n"


    | CIL_method(srcfile_, (_, _, mgr), flags, ck, rtype, (id, uid), signat, flags1, instructions) -> 
           let ids = cil_classrefToStr id
           pad + ".method " + cil_flagsToStr flags + " " + cil_ckToStr ck + " " + cil_typeToStr rtype + " " + 
           ids + 
           (if nullp mgr then "" else "<" + sfold cil_typeToStr mgr + ">") +
           cil_formalsToStr signat + " " +
           cil_method_flagsToStr flags1 +
           " %uid=" + uid +
           (if ff then "\n" + pad + "{\n" + List.foldBack (cil_classitemToStr (pad + "  ")) instructions "" + pad + "} end of method " + ids + "\n\n"
            else ""
           )

    | CIL_pack e -> pad + ".pack " + iexpToStr e + "\n"

    | CIL_data(l, r) -> pad + ".data " + iexpToStr l + "->" + iexpToStr r + "\n"


    | CIL_assembly(flags, cr, items) ->
         (
            pad + ".assembly " + cil_flagsToStr flags + " " + cil_classrefToStr cr + "\n{\n" +
            List.foldBack (cil_classitemToStr (pad + "  ")) items "" +
            "}\n\n"
         )

    //| CIL_module(flags, cr) -> pad + ".module " + cil_classrefToStr cr + "\n"
    | CIL_module(id, flags, items) ->
        // Module contents do not have a { } bracketing.
        pad + sprintf ".module %s \n" id +
          (if ff then pad + "{\n" + List.foldBack (cil_classitemToStr (pad + "  ")) items "" + pad + " // end of module " + id + "\n\n"
           else "")


    
    | CIL_class(srcfile_, flags, cr, generics, extends, items) ->
        let id =  cil_classrefToStr cr 
        let tplates = if nullp generics then "" else "<"  + sfold cil_targToStr generics + ">"
        let ext = (if nonep extends then "" else " extends " + cil_classrefToStr(valOf extends))
        (
          pad + ".class " + " " + cil_flagsToStr flags + " " + id + tplates + ext + "\n" +
          (if ff then pad + "{\n" + List.foldBack (cil_classitemToStr (pad + "  ")) items "" + pad + "} // end of class " + id + "\n\n"
           else "")
        )

    | CIL_property(ck, tt, cr, signat, items) -> 
         pad + ".property " + ck + " " + cil_typeToStr tt + " " + cil_classrefToStr cr +
         cil_actualsToStr signat +
         (if ff then "{\n" + List.foldBack (cil_classitemToStr "") items "" + "}\n\n" else "")


    | CIL_imagebase e -> pad + ".imagebase " + iexpToStr e + "\n"
    | CIL_file(e, f) ->  pad + ".file " + iexpToStr e + " " + iexpToStr f + "\n"
    | CIL_stackreserve e -> pad + ".stackreserve " + iexpToStr e + "\n"
    | CIL_corflags e     -> pad + ".corflags " + iexpToStr e + "\n"
    | CIL_subsystem e    -> pad + ".subsystem " + iexpToStr e + "\n"

    | CIL_try(Some nt, tryblk, (_, catchers, (Some nf, finallyblk))) -> // Render numerically when numbers are present
        let ax = function
            | (dt, Some nc, _) -> cil_typeToStr dt + "->" + i2s nc
            | _ -> ">>?"
        sprintf "CIL_try(%i, %s, %i)" nt (sfold ax catchers) nf
    | CIL_try(None,   tryblk, (_, catchers, (no_, finallyblk))) ->
        let aa = pad + ".try {\n" + cil_classitemsToStr (pad + "  ") tryblk "" + pad + "}\n"
        let crender (ep, no_, lst) =
            let tx = cil_typeToStr ep
            pad + "catch " + tx + "\n" + pad + "{\n" + cil_classitemsToStr (pad + "  ") lst "\n" + pad + "}\n"
        let cc = if nullp finallyblk then "" else pad + "finally\n" + pad +  "{ \n" + cil_classitemsToStr (pad + "  ") finallyblk "" + pad + "}\n"
        aa + sfoldcr_lite crender catchers + cc

    | CIL_namespace(flags, cr, items) ->
        let id = cil_classrefToStr cr
        in
          (
           pad + ".namespace " + " " + cil_flagsToStr flags + " " + id + "\n{\n" +
           List.foldBack (cil_classitemToStr (pad + "  ")) items "" + 
           "} // end of " + id + "\n\n"
          )
    | CIL_lazybones(id, unique_id, key, answer) when nonep !answer -> sprintf "CIL_lazybones(%s, %s, ... missing)" id unique_id
    | CIL_lazybones(id, unique_id, key, answer) -> classitemToStr pad ff (valOf !answer)
    | other -> pad + ("?? .cil classitem other\n " + other.ToString())

and cil_classitemToStr pad a c = classitemToStr pad true a + c

and cil_classitemsToStr pad a c = List.foldBack (cil_classitemToStr pad) a c 


let cil_out vd l = yout vd (cil_classitemToStr "" l "")


let cilld_int32(n) = Cili_ldc(Ciltype_int, Cil_argexp(Cil_number32 n))



(* End of asmout.fs and other toStr functions *)
