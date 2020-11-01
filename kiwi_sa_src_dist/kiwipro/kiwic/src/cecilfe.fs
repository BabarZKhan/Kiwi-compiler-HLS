//
// Kiwi Scientific Acceleration: KiwiC compiler.
// All rights reserved. (C) 2007-15, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// cecilfe.fs - This uses the cecil library to read in .net PE files to our internal AST.
// There is an alternative to this file that uses yacc to parse the output of monodis.
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


module cecilfe


//-------------
open Mono.Cecil
open Mono.Cecil.Cil
open Mono.Cecil.Metadata
open System.Collections.Generic
open System.Numerics
 
open yout
open moscow
open cilgram
open opath_hdr
open asmout

let g_gcc_kludge = ref false

type fe_scope_t =
    {
        srcfile:          string
        lazy_pe_reading:  int
        AssemblyName:     string
        stack:            string list
    }


// Debug routine    
let printProperties msg x =
    let t = x.GetType()
    let properties = t.GetProperties()
    printfn ("----------- ")
    printfn "%s %s" msg t.FullName
    properties |> Array.iter (fun prop ->
                              if prop.CanRead then
                                  let value = prop.GetValue(x, null)
                                  printfn "%s: %O" prop.Name value
                              else printfn "%s: ?" prop.Name)



// To avoid loosing equality type we store the generator functions in this ancillary dictionary
let g_lazybones_factory = new OptionStore<string, WW_t->cil_t list>("lazybones_factory")

let g_module_esc = "<Module>"

let gec_CIL_lazybones(myname, sx, genf) =
    let key = funique "lazykey"
    g_lazybones_factory.add key genf
    CIL_lazybones(myname, sx, key, ref None)

let cil_lazybones_apply ww = function
    | CIL_lazybones(myname, sx, key, answer) when not_nonep !answer -> valOf !answer

    | CIL_lazybones(myname, sx, key, answer) ->
        let ww = WF 3 "cil_lazybones_apply" ww (sprintf "lazybones ceceilfe parse of  " + myname + " : " + sx)
        let ans = match g_lazybones_factory.lookup key with
                   | None -> sf (sprintf "lazybones key not registered for " + myname + " : " + sx)
                   | Some genf ->
                       match genf ww with
                           | [ans] -> ans
                           | other -> sf (sprintf "lazybones produced %i answers for " (length other) + myname + " : " + sx)
        answer := Some ans
        ans
    | other -> other
    

let g_ast_rendered = ref []

let g_fevd = ref 3 // Normally 3. This is the logging level for details: make higher for more output or else adjust kiwife-loglevel on command line or recipe.


let g_startup_names = [  "COBJ?init" ]

let cil_cr_dot(a, b) = Cil_lib(a, Cil_cr b)

type xe_t =
     {
       hasthis:      bool
       formals:      cil_formal_t list
       fe_scope:  fe_scope_t
     }

let cil_ins_database = Dictionary<string, xe_t -> Instruction -> cilinst_t>()


let define_ins(a, b) =
    let (found, ov) = cil_ins_database.TryGetValue(a)
    if found then vprintln 0 ("Redefined " + a)
             else cil_ins_database.Add(a, b)

let g_cecilfe_srcfiletoken = ref None

let disam name =
    // This magic identifier needs disambiguation over its multiple definitions in different src files.
    let myname = if name = "<PrivateImplementationDetails>" then (valOf !g_cecilfe_srcfiletoken) + "---" + name else name
    myname
    
let gen_cil_cr_bin = function
    | x -> x


let decode_i4oh xe ohead (x: Instruction) =
    let vd = !g_fevd
    let w = 32
    let ot = x.OpCode.OperandType
    let oty = x.Operand.GetType().ToString()
    let ots = x.Operand.ToString()
    if vd >=4 then vprintln 4 ("operand " + ots + " has type >" + oty + "<")

    if ot=OperandType.ShortInlineParam
    //|| oty = "Mono.Cecil.Cil.ParameterDefinition"
    then 
        let v = x.Operand :?> Mono.Cecil.ParameterDefinition
        if ohead > 0 then if vd >= 4 then vprintln 4 ("Adding 'hasthis' ohead")
        let idx = v.Method.Parameters.IndexOf(v) + ohead
        //if xvd>=4 then vprintln 0 ("Now got " + v.ToString() + " " + i2s v.IndexOf)
        Cil_argexp(Cil_number32(idx))
    elif oty = "Mono.Cecil.Cil.VariableDefinition" then
        let idx = (x.Operand :?> Mono.Cecil.Cil.VariableReference).Index
        Cil_argexp(Cil_number32(idx))
    else
        let _ = assertf(ot=OperandType.ShortInlineI || ot=OperandType.InlineI || ot=OperandType.InlineParam) (fun () -> sprintf "cecilfe: decode not an i4 - had %A" ot)
        let s = x.Operand.ToString() // nasty but dont know a better way
        let v = if s.[0] = '-' || System.Char.IsDigit(s.[0])
                then atoi s 
                else
                    let rec find_formal = function
                        | [] -> None
                        | Cil_fp(_, Some idx, _, id)::_ when id=s -> Some idx
                        | _::tt -> find_formal tt
                    match find_formal xe.formals with
                        | Some n -> BigInteger n
                        | None ->
                            dev_println (sprintf "%s: decode_i40h: ot=%A not a digit at >%s< ots=%s operand=%A" (x.OpCode.ToString()) ot s ots x.Operand)
                            BigInteger -12345678 // for now
        Cil_argexp(Cil_int_i(w, v))


let decode_i4_argref (xe:xe_t) (x: Instruction) = 
    let ohead = if xe.hasthis then 1 else 0
    decode_i4oh xe ohead x


let decode_i4 xe (x: Instruction) = decode_i4oh xe 0 x

let decode_i8 (x: Instruction) =
    let w = 64
    let ot = x.OpCode.OperandType
    let vd = !g_fevd
    if vd>=5 then vprintln 5 ("operand " + x.Operand.ToString() + " has type " + ot.ToString())
    let _ = cassert(ot = OperandType.ShortInlineI || ot = OperandType.InlineI || ot = OperandType.InlineI8, "decode not an i8")
    let v = atoi(x.Operand.ToString()) // nasty but dont know a better way
    Cil_argexp(Cil_int_i(w, v))

let decode_r4 xe (x:Instruction) = Cil_argexp(Cil_float(4, x.Operand.ToString()))

let decode_r8 xe (x:Instruction) = Cil_argexp(Cil_float(8, x.Operand.ToString()))



let decode_str (x: Instruction) =
    let ot = x.OpCode.OperandType
    let _ = cassert(ot = OperandType.InlineString, "decode not a string")
    let rr = (x.Operand :?> string)
    rr


let ce_dedot s =
    let lst = split_string_at ['.'] s
    let aqf x = "'" + x + "'"
    //if xvd>=4 then vprintln 0 ("Split " + s + " at dot: " + sfold aqf lst)
    let rec kla = function
            | [] -> sf "dedot"
            | [item] -> Cil_cr(disam item)
            | a::b::t -> Cil_namespace(Cil_cr(disam a), kla (b::t))
    kla lst

let decode_switch (x: Instruction) =
    let ot = x.OpCode.OperandType
    let _ = cassert(ot = OperandType.InlineSwitch, "decode not a switch")
    let targets = x.Operand :?> Instruction []
    let l = targets.Length
    map (fun x -> "LL" + i2s(targets.[x].Offset)) [ 0 .. l-1 ]
    
let decode_branch (x: Instruction) =
    let ot = x.OpCode.OperandType
    let _ = cassert(ot = OperandType.InlineBrTarget || ot = OperandType.ShortInlineBrTarget, "decode not a branch")
    let target = (x.Operand :?> Instruction)
    let r = "LL" + i2s target.Offset
    r



// This vt form is simply to facilitate asmout nicer forms.  The CT_vt form should not be used elsewhere.
let gec_vt = function
    | (K_int, 32)   -> CTL_vt(K_int,   32, g_canned_i32)
    | (K_int, 64)   -> CTL_vt(K_int,   64, g_canned_i64)
    | (K_float, 32) -> CTL_vt(K_float, 32, g_canned_float)
    | (K_float, 64) -> CTL_vt(K_float, 64, g_canned_double)
    | other -> sf (sprintf "gec_vt other form %A" other)


let rec cecil_decode_GenericInstanceType settings ans1 (git:Mono.Cecil.GenericInstanceType) =
    let vd = !g_fevd
    let gargs = git.GenericArguments
    let titems = ref []
    let _ = for pty:Mono.Cecil.TypeReference in gargs do
                        let name = pty.(*ParameterDefinition.*)Name
                        let ty = decode_TypeReference settings pty
                        mutadd titems (Cil_tp_type ty)
    //let namespace1 = decode_TypeReference settings(git.GetOriginalType())
    let ans = if nullp !titems then ans1 else Cil_cr_templater(ans1, rev(!titems))
    if vd>=5 then vprintln 5 ("Final decode_typeref templated=" + cil_typeToStr ans)
    ans

and decode_TypeReference settings (x:TypeReference) =
    if (x=null) then sf "decode_TypeReference null"
    else
    //vprintln 0 ("decode_TypeReference start")
    let vd = !g_fevd
    let m = x.Module
    let ox = x.GetOriginalType()
    if vd>=5 then vprintln 5 ("in typeref mn=" + boolToStr (m=null))    
    let vt = x.IsValueType
    if vd>=5 then vprintln 5 ("Decode_TypeReference  " + (if m=null then "null" else m.ToString()) + " vt=" + boolToStr vt  + " Namespace=" + x.Namespace +  " Name=" + x.Name + " Fullname=" + x.FullName)



    let ll = strlen "?string_type?" // as used in gcc4cil
    if strlen ox.Name > ll && ox.Name.[0..ll-1] = "?string_type?" then
        let string_length = atoi32(ox.Name.[ll..])
        // Under gcc4cil ldslfa will typically add one star back. Take one off here when we have one boiled into f_canned_c8 ...
        let ans = gec_CT_star 0 (f_canned_c8_str string_length) // This is a byte/UTF-8 string, not a unicode string.
        //if v>=4 then vprintln 4 (sprintf "decode_TypeReference gcc4cil string style from %s as %s" ox.Name (cil_typeToStr ans)) 
        ans
    else
    let ll = strlen "array?[" // as used in gcc4cil
    let arg = ox.Name
    if strlen arg > ll && arg.[0..ll-1] = "array?[" then
        let msg = "gcc4cil literal array" // parse tokens like "array?[100]I8"
        let sb = string_burner_t(arg, ll)
        let array_length = sb.Atoi()
        let rbar_ = sb.NextCh()
        let signed = if sb.Ch() = 'U' then (sb.Adv(); hprls_hdr.Unsigned) else hprls_hdr.Signed // note: no FP so far
        let bflag = sb.NextCh()
        let radix = sb.Atoi()
        if vd>=5 then vprintln 5 (msg + sprintf  ": parsed >%s< %A bflag=%A length=%i radix=%i" arg signed bflag array_length radix)
        let ct = CTL_net(false, int radix, signed, g_null_net_at_flags)
        CT_arr(ct, Some array_length)
    else

    let wf1 x = if vt then Cil_cr_flag(Cilflag_valuetype, x) else x

    let ans1 =
        if ox.DeclaringType=null then Cil_cr(disam ox.Name)
        else cil_cr_dot((decode_TypeReference settings ox.DeclaringType), disam ox.Name)

        // Declaring type takes precedence over namespace - do not apply both!

    if vd>=5 then vprintln 5 ("Declaring type=" + boolToStr(x.DeclaringType <> null) + ", Namespace=" + boolToStr(x.Namespace <> null) + ", IsNested=" + boolToStr(x.IsNested))

    let ans1 =
        if ox.Namespace <> null && ox.Namespace <> ""
        then Cil_namespace(ce_dedot ox.Namespace, ans1)
        else ans1

    let disid = x.GetType().ToString()
    if vd>=5 then vprintln 5 ("Disid=" + disid)

    let ans2 = 
       match disid with
        | "Mono.Cecil.GenericParameter" -> // These may be of the form "!n" or "!!n" for class and method generics respectively.
            let id:string = x.Name
            let (methodf, idx) =
                match x with
                    | :? GenericParameter as gp ->
                        let methodf = // If the owner is a type then this is a class generic, else a method generic. Or call gp.Type=GenericParameterType.Method in future.
                            match gp.Owner with
                                | :? TypeReference -> false
                                | _ -> true

                        (methodf, Some (gp.Position))
                    | _ ->
                        if vd>=5 then vprintln 5 (sprintf "Disid=%s has no Position property" disid)                        
                        (false, None)
            //TODO - toom many routes here
            //if xvd>=4 then vprintln 0 (sprintf "decode_TypeReference genericParameter %s idx=%A" id idx)
            //if xvd>=4 then vprintln 0 ("GP name only for now" + x.Name)
            if id.[0] = '!' then
               if id.[1] = '!' then CTVar{ g_def_CTVar with methodf=true; idx=Some(atoi32(id.[2..])); }
               else CTVar{ g_def_CTVar with idx=Some(atoi32(id.[1..])); }
            else CTVar{ g_def_CTVar with id=Some(id); idx=idx; methodf=methodf }
           
        | "Mono.Cecil.ArrayType" ->
            let at:Mono.Cecil.ArrayType = x :?> Mono.Cecil.ArrayType
            let rank = at.Rank
            let dims = at.Dimensions
            let d0:Mono.Cecil.ArrayDimension = dims.[0]
            let v = at.IsSizedArray
            let ty = decode_TypeReference settings at.ElementType
            let ub = dims.[0].UpperBound
            let lb = dims.[0].LowerBound
            let _ = cassert(lb=0, "Lower bound non zero")
            let kfec = valOf(!g_kfec1_obj)        
            let multid_name = function
                | 2 -> kfec.d2name
                | 3 -> kfec.d3name
                | 4 -> kfec.d4name
            if rank=1 then Ciltype_array(ty, (if v then Some ub else None))
            elif rank>=2 && rank<=4 then Cil_cr_templater(Cil_cr (multid_name rank + "`1"), [ Cil_tp_type ty ])
            else sf("array rank not 1..4")

            //sf ("ARRAY " + boolToStr v + " rank=" + i2s rank + " v0=" + i2s(ub) + " "  + i2s(lb))

        | "Mono.Cecil.GenericInstanceType" ->
            let git:Mono.Cecil.GenericInstanceType = x :?> Mono.Cecil.GenericInstanceType
            let ans = cecil_decode_GenericInstanceType settings ans1 (git:Mono.Cecil.GenericInstanceType)
            ans

        | "Mono.Cecil.ModifierRequired" ->
            // .field bool modreq ([mscorlib]System.Runtime.CompilerServices.IsVolatile) empty
            let modtype = (x :?> Mono.Cecil.ModifierRequired).ModifierType
            let moder = decode_TypeReference settings modtype
            let ans1 = Ciltype_modreq(ans1, moder)
            if vd>=5 then vprintln 5 ("Final decode_typeref modtype=" + cil_typeToStr ans1)
            ans1

        | "Mono.Cecil.ReferenceType" ->
            let at:Mono.Cecil.ReferenceType = x :?> Mono.Cecil.ReferenceType
            //if xvd>=4 then vprintln 0 (sprintf "ReferenceTyper %A at=%A" ans1 at)
            //if xvd>=4 then vprintln 0 (sprintf "ReferenceTyYper element=%A" at.ElementType)
            Ciltype_ref(decode_TypeReference settings at.ElementType)

        | "Mono.Cecil.PointerType" ->
            let at:Mono.Cecil.PointerType = x :?> Mono.Cecil.PointerType
            Ciltype_star(decode_TypeReference settings at.ElementType)


        | "Mono.Cecil.TypeReference"
        | "Mono.Cecil.TypeDefinition" ->
            if vd>=5 then vprintln 5 ("in typeref xn=" + boolToStr (x=null))
            let m = x.Module
            if vd>=5 then vprintln 5 ("in typeref mn=" + boolToStr (m=null))    
            //if vd>=5 then vprintln 5 ("TypeReference is (other) " + x.GetType().ToString())
            // wf1 behaviour just in here for now - makes test30 work
            let ans1 = wf1 ans1
            if vd>=5 then vprintln 5 ("Final decode_typeref other=" + cil_typeToStr ans1)
            ans1 // e.g. Ciltype_int    

        | "Mono.Cecil.PinnedType" ->
            let id:string = x.Name
            let ox = x.GetOriginalType()
            let disidx = disam x.FullName
            let disidox = x.GetType().ToString()
            let pt  = false
(*            let pt:PointerType = x :?> PointerType
                 match x with
                    | :?> TypeSpecification as ts ->
                        match ts with
                            | :?> PointerType as pt -> true
                            | _ -> false
                    | :?> PointerType as pt -> true
                    | _ -> false
*)
            let bt = decode_TypeReference settings ox
// e.g.       bt=Cil_cr_flag(Cilflag_valuetype,Cil_lib (Cil_cr "PinnedStructs",Cil_cr "datapacket"))
//            ct=PinnedStructs/datapacket*
//            d1=PinnedStructs/datapacket*
//            d2=Mono.Cecil.PinnedType

            let bt = Cil_cr_flag(Cilflag_pinned, bt)
            //if xvd>=4 then vprintln 3 (sprintf "PinnedType parsed as %s pt=%A ox=%A bt=%A ct=%A d1=%s d2=%s" id pt ox bt x disidx disidox)
            bt

        | "Mono.Cecil.ModifierOptional" ->
            let id:string = x.Name
            let ox = x.GetOriginalType()
            let bt = decode_TypeReference settings ox
            //if xvd>=4 then vprintln 3 (sprintf "ModifierOptional for %A for %s giving %s" "Mono.Cecil.ModifierOptional" id  (cil_typeToStr bt))
            bt

            
        | other -> sf ("cecilfe: Other type form " + other)

    if vd>=5 then vprintln 5 ("Final final decode_typeref " + cil_typeToStr ans2 + "\n-----------------\n")
    //if xvd>=4 then vprintln 0 ("decode_TypeReference finish")
    ans2



and decode_methodref settings (x:Mono.Cecil.MethodReference) =
    let vd = !g_fevd
    let rt = decode_TypeReference settings (x.ReturnType.ReturnType)
    let (m_gprams, m_prams) = (ref [], ref [])
    let _ =
        for pd:Mono.Cecil.ParameterDefinition in x.Parameters do
        let name = disam pd.(*ParameterDefinition.*)Name
        //let idx = pd.Index
        let ty = decode_TypeReference settings (pd.(*ParameterDefinition.*)ParameterType)
        let _ =
            for ca:CustomAttribute in pd.CustomAttributes do
                if vd>=5 then vprintln 5 ("+++ ignore custom at")
                ()
        let n = if pd.IsIn then "in," else ""
        let o = if pd.IsOut then "out," else ""
        let _ = pd.HasDefault        
        mutadd m_prams ((Some(n + o + name), ty))

    let _ =
        match x with
            | :? GenericInstanceMethod as x1 ->
                //if xvd>=4 then vprintln 0 (sprintf "methodname=%s hasit=%A" x.Name true)
                 for gp:Mono.Cecil.TypeReference in x1.GenericArguments do
                     let ty = decode_TypeReference settings gp
                     let name = disam gp.Name
                     let fullname = disam gp.FullName
                     let _ = mutadd m_gprams ty
                     //if xvd>=4 then vprintln 3 (sprintf "fetched gp %s ty=%s - should add to method prams " name (cil_typeToStr ty) + " fullname=" + fullname)
                     ()
            | _ -> ()
    (disam x.Name, rt, rev(!m_prams), rev(!m_gprams))

and decode_fieldref settings (x: Instruction) =
    let vd = !g_fevd
    let ot = x.OpCode.OperandType
    if vd>=5 then vprintln 5 (" fieldref ot is 1/2 " + ot.ToString())
    if ot = OperandType.InlineField then
        let md = (x.Operand :?> FieldReference)
        //if vd>=5 then vprintln 5 (" fieldref ot is 2/2 " + ot.ToString())
        let fieldtype:TypeReference = md.FieldType
        let declaringtype:TypeReference = md.DeclaringType
        if vd>=5 then vprintln 5 ( "md0=" + fieldtype.ToString())
        if vd>=5 then vprintln 5 ( "md1=" + md.Name)
        //    if vd>=5 then vprintln 5 ("md2=" + md.FullName())
        let md2aa = decode_TypeReference settings declaringtype
        if vd>=5 then vprintln 5 ( "md2t=" + declaringtype.GetType().ToString())
        let fieldtype = decode_TypeReference settings fieldtype
        (fieldtype, (md2aa, disam md.Name, []))
    elif ot=OperandType.InlineType then
        let mt = (x.Operand :?> TypeReference)        
        let fieldtype = decode_TypeReference settings mt
        (fieldtype, (g_ctl_object_ref, "spare-redundant-string-field1", []))
    elif ot=OperandType.InlineTok then
        match x.Operand with
            | :? TypeDefinition as td ->
                let dt = decode_TypeReference settings td
                //if xvd>=4 then vprintln 0 "01 hit td"
                //let t =  fe_type settings td []
                (dt, (g_ctl_object_ref, "spare-redundant-string-field2", []))

//            =ERROR=-=-=->Muddy bog: a part of the compiler tool is missing: other InlineTok operand Mono.Cecil.TypeDefinition op=My.MyProject/MyWebServices
            | :? FieldReference as md ->
                let fieldtype:TypeReference = md.FieldType
                let declaringtype:TypeReference = md.DeclaringType
                if vd>=5 then vprintln 5 ( "md0=" + fieldtype.ToString())
                if vd>=5 then vprintln 5 ( "md1=" + md.Name)
                //    if vd>=5 then vprintln 5 ("md2=" + md.FullName())
                let md2aa = decode_TypeReference settings declaringtype
                if vd>=5 then vprintln 5 ( "md2t=" + declaringtype.GetType().ToString())
                let fieldtype = decode_TypeReference settings fieldtype
                (fieldtype, (md2aa, disam md.Name, []))

            | :? GenericParameter as gp ->
                let idx = gp.Position
                let name = gp.Name
                let fullname = gp.FullName
                let methodf = // If the owner is a type then this is a class generic, else a method generic. Or call gp.Type=GenericParameterType.Method in future.
                    match gp.Owner with
                        | :? TypeReference -> false
                        | _ -> true
                if vd>=5 then vprintln 5 (sprintf "  methodf=%A formal generic idx=%i name=%s: " methodf idx name + " fullname=" + fullname)
                let dt = CTVar{ g_def_CTVar with idx=Some idx; id=Some name; methodf=methodf }
                // There is no surrounding class at this time so use this value as a filler. Or else CT_up(dt) would serve.
                // Open (aka unbound) method generic: CIL did not have a syntax for this prior to .NET 4.0 - but now a "[1]" seems to be used.
                (dt, (dt, "", []))  // Return empty string for field name and tidytoc shall delete it.

            | :? GenericInstanceType as git ->
                let name = git.Name
                let ans1 =
                    if git.DeclaringType=null then Cil_cr(disam name)
                    else cil_cr_dot((decode_TypeReference settings git.DeclaringType), disam name)
                //printProperties "L505" git
                let fullname = git.FullName // Printable version - would have to be parsed
                //if vd>=5 then vprintln 5 (sprintf "  method formal generic name=%s: " name + " fullname=" + fullname)
                let dt = cecil_decode_GenericInstanceType settings ans1 (git:Mono.Cecil.GenericInstanceType)
                (dt, (dt, "", [])) // Return empty string for field name and tidytoc shall delete it.
                
            | operand ->
                let ss = operand.GetType().ToString()
                if vd>=5 then vprintln 5 (" fieldref ot is 2/2 " + ot.ToString())
                muddy (sprintf "other InlineTok operand %s op=%A" ss operand)
    else sf (sprintf "decode not a fieldref: ot=%A " ot)


and tidytok_decode_fieldref settings ins =
    match decode_fieldref settings ins with
        | (dt, (_, "", _)) -> (dt, None)
        | (dt, fr)         -> (dt, Some fr)

and decode_classref settings (x: Instruction) =
    let vd = !g_fevd
    let ot = x.OpCode.OperandType
    if vd>=5 then vprintln 5 ("tto " + x.OpCode.OperandType.ToString())
    let _ = cassert(ot = OperandType.InlineType, "decode not a classref ")
    let xtt = decode_TypeReference settings (x.Operand :?> TypeReference)
    xtt

and fe_call settings moder (x:Instruction) =
    let vd = !g_fevd
    let ot = x.OpCode.OperandType
    if vd>=5 then vprintln 5 ("operand " + x.Operand.ToString() + " has type " + x.Operand.GetType().ToString())
    let methref = (x.Operand :?> MethodReference)
    let (name, rt, actuals, gargs) = decode_methodref settings methref
    let hasthis = methref.HasThis
    let ck =
        match methref.CallingConvention with
            | Mono.Cecil.MethodCallingConvention.VarArg   -> CK_vararg hasthis
            | Mono.Cecil.MethodCallingConvention.ThisCall -> CK_instance hasthis
            | Mono.Cecil.MethodCallingConvention.Default  -> CK_default hasthis // aka static
            | Mono.Cecil.MethodCallingConvention.Generic  -> CK_generic hasthis
            
            | other -> muddy("instance? " + methref.CallingConvention.ToString())
            
    //let rt = decode_type(x.ReturnType)
    let md2aa = decode_TypeReference settings methref.DeclaringType
    Cili_call(moder, ck, rt, (md2aa, name, gargs), actuals, new_callsite name)

and fe_ldftn settings (x:Instruction) =
    let vd = !g_fevd
    let ot = x.OpCode.OperandType
    if vd>=5 then vprintln 5 ("operand " + x.Operand.ToString() + " has type " + x.Operand.GetType().ToString())
    let methref = (x.Operand :?> MethodReference)
    let (name, rt, actuals, gargs) = decode_methodref settings methref
    let hasthis = methref.HasThis

    let ck =
        if hasthis then "instance" else // repeated code
        match methref.CallingConvention with
            | Mono.Cecil.MethodCallingConvention.VarArg -> "vararg"
            | Mono.Cecil.MethodCallingConvention.ThisCall -> "instance"
            | Mono.Cecil.MethodCallingConvention.Default -> "class"            
            | other -> muddy("instance? " + methref.CallingConvention.ToString())
            
    //let rt = decode_type(x.ReturnType)
    let md2aa = decode_TypeReference settings methref.DeclaringType
    Cili_ldftn((*virtflag=*)false, "instance", rt, (md2aa, name, gargs), actuals, new_callsite name)

and fe_newobj settings (x:Instruction) =
    let vd = !g_fevd
    let ot = x.OpCode.OperandType
    if vd>=5 then vprintln 5 ("operand " + x.Operand.ToString() + " has type " + x.Operand.GetType().ToString())
    let methref = (x.Operand :?> MethodReference)
    let (name, rt, actuals, gargs) = decode_methodref settings methref
    let hasthis = methref.HasThis

    let modflags = ref []
    //if methref.IsPublic then mutadd modflags Cilflag_public
//    if methref.IsStatic then mutadd modflags Cilflag_static
  //  if methref.IsFinal then mutadd modflags Cilflag_final
    //if methref.IsVirtual then mutadd modflags Cilflag_virtual

    let ck =
        if hasthis then "instance" else // repeated code 3 times
        match methref.CallingConvention with
            | Mono.Cecil.MethodCallingConvention.VarArg -> "vararg"
            | Mono.Cecil.MethodCallingConvention.ThisCall -> "instance"
            | Mono.Cecil.MethodCallingConvention.Default -> "class"            
            | other -> muddy("instance? " + methref.CallingConvention.ToString())
            
    //let rt = decode_type(x.ReturnType)
    let md2aa = decode_TypeReference settings methref.DeclaringType
    Cili_newobj(!modflags, ck, rt, (md2aa, name, gargs), actuals, new_callsite name)

and fe_custom settings (ca:CustomAttribute) =
    let vd = !g_fevd
    let m_aargs = ref []
    let _ =
        for x in ca.ConstructorParameters do
        let aarg = match x.GetType().ToString() with
                   | "System.String"  -> Cil_id (x :?> System.String)
                   | "System.Boolean" -> Cil_number32(if (x :?> System.Boolean) then 1 else 0)
                   | "System.Int32"   -> Cil_number32 (x :?> System.Int32)
                   
                   | _ -> sf ("Custom pram type not supported " + x.GetType().ToString())
        mutadd m_aargs aarg
        
    // Type Value Constructor
    let uctor:Mono.Cecil.MethodReference = ca.Constructor

    // if vd>=5 then vprintln 5 ("Custom ctor=" + uctor.ToString())
    let (name, ty, args, gargs) = decode_methodref settings uctor
    let _ =
        for cana (* :Mono.Cecil.CustomAttributeNamedArgument  *) in ca.Fields do
            if vd>=5 then vprintln 5 ("ca has fld/cana " + cana.ToString())
            ()
    let _ =
        for cana (* :Mono.Cecil.CustomAttributeNamedArgument *) in ca.Properties do
            if vd>=5 then vprintln 5 ("ca has prop " + cana.ToString())
            ()
    //        let vale = ca.Value
    let cr = decode_TypeReference settings (uctor.DeclaringType)
    let fr = (cr, name, gargs)

    let _ = for pd:Mono.Cecil.ParameterDefinition in uctor.Parameters do
                let name = pd.(*ParameterDefinition.*)Name
                ()

    let read_blob =
        if ca.Blob <> null then
            let m_blobval = ref []
            //if xvd>=4 then vprintln 0 (sprintf "decoding blob %s A=%A" name blob)        
            let _ = for x:byte in ca.Blob do mutadd m_blobval (Cil_number32(System.Convert.ToInt32 x)) done
            rev !m_blobval
        else []
    if vd>=5 then vprintln 5 (if ca.Blob=null then "Null blob" else i2s(length read_blob) + " item blob list")

    let arga =
        if not_nullp read_blob && not_nullp !m_aargs then sf "both types of arg init provided!"
        elif not_nullp !m_aargs then Cil_explist(rev !m_aargs)
        else Cil_blob read_blob
    CIL_custom(name, ty, fr, args, arga)


and fe_method fe_scope cr cr_fap (meth:Mono.Cecil.MethodDefinition) =
    let vd = !g_fevd
    //let _ = cassert(meth <> null, "meth")
    let id = meth.Name // "noname"
    let id = // Discard first encountered method of this name by renaming it with discarded prefix.
        if id = "myputhex" && not !g_gcc_kludge then
            let _ = g_gcc_kludge := true
            "discarded_" + id
        else id
    let unique_id = funique id
    let is_startup_code = memberp id g_startup_names
    if vd>=5 then vprintln 5 (sprintf "fe_method 1 name=%s is_startup_code=%A" id is_startup_code)
    if fe_scope.lazy_pe_reading > 0 then
        gec_CIL_lazybones(id, unique_id, (fun ww -> [fe_method {fe_scope with lazy_pe_reading=fe_scope.lazy_pe_reading-1} cr cr_fap meth]))
    else

    let flags = ref []
    //if meth.EntryPoint then mutadd rans CIL_entrypoint
    let specialname = meth.IsSpecialName
    let specialname = meth.IsRuntimeSpecialName    

    if meth.IsSpecialName then mutadd flags Cilflag_specialname
    if meth.IsRuntimeSpecialName then mutadd flags Cilflag_rtspecialname
    if meth.IsRuntime then mutadd flags Cilflag_runtime
    if meth.IsPrivate then mutadd flags Cilflag_private
    if meth.IsPublic then mutadd flags Cilflag_public
    if meth.IsStatic then mutadd flags Cilflag_static
    if meth.IsFinal then mutadd flags Cilflag_final
    if meth.IsVirtual then mutadd flags Cilflag_virtual
// Other flags are: HideBySig IsSpecialName IsRuntimeSpecialName IsNative IsManaged IsUnmanaged IsForwardRef IsPreserveSig IsInternalCall IsSynchronised IsSetter IsGetter
    let hasthis = meth.HasThis // These are not orthogonal!! TODO
    let ck =
        match meth.CallingConvention with
            | Mono.Cecil.MethodCallingConvention.Generic  -> CK_generic hasthis 
            | Mono.Cecil.MethodCallingConvention.VarArg   -> CK_vararg hasthis
            | Mono.Cecil.MethodCallingConvention.ThisCall -> CK_default hasthis
            | Mono.Cecil.MethodCallingConvention.Default  -> CK_default hasthis
            | other -> muddy("instance?? " + meth.CallingConvention.ToString())
    

    let _ = cassert(meth.ReturnType <> null, "meth.ReturnType")
    let ty = decode_TypeReference fe_scope (meth.ReturnType.ReturnType)
    let _ = cassert(meth.DeclaringType <> null, "meth.DeclaringType")    
    let cr = decode_TypeReference fe_scope (meth.DeclaringType)



//    if xvd>=4 then vprintln 0 ("fe_method 2 name=" + id)

    let formals =
        let m_rr = ref []
        let m_idx = ref 0
        for p:ParameterDefinition in meth.Parameters do
            let id = p.Name
            //let _ = p.IsOptional
            if vd>=5 then vprintln 5 ("Parameter " + id)
            let ty = decode_TypeReference fe_scope p.ParameterType
            mutadd m_rr (Cil_fp(None, Some !m_idx, ty, id))
            mutinc m_idx 1
            ()
        rev !m_rr
        
    let m_mgr = ref []
    let _ =
        for gp:GenericParameter in meth.GenericParameters do
            let idx = gp.Position
            let name = gp.Name
            let methodf = // If the owner is a type then this is a class generic, else a method generic. Or call gp.Type=GenericParameterType.Method in future. TODO check further over this.
                    match gp.Owner with
                        | :? TypeReference -> false
                        | _ -> true

            let fullname = gp.FullName
            if vd>=5 then vprintln 5 (sprintf "  method formal generic idx=%i name=%s: " idx name + " fullname=" + fullname)
            let formal = CTVar{ g_def_CTVar with idx=Some idx; id=Some name; methodf=methodf; }
            mutadd m_mgr formal


    let prats =
        let m_rans = ref []
        for gp:CustomAttribute in meth.CustomAttributes do mutadd m_rans (fe_custom fe_scope gp) done
        !m_rans
               
    //vprintln 0 ("fe_method 3 name=" + id)

    let variables = // locals
        let m_vans = ref []
        let _ =
            if meth.Body = null || meth.Body.Variables = null then ()
            else
            for vv in meth.Body.Variables do
                let name = vv.ToString() // vv.VariableReference.name
                //if xvd>=4 then vprintln 1 ("fe_method var 0  name=" + id + " var =" + name)
                let idx = vv.Index
                if vd>=5 then vprintln 5 ("fe_method local var " + i2s idx  + "  name=" + id + " var =" + name)                
                let _ = cassert(vv.VariableType <> null, "vv.VariableType")
                let ty = decode_TypeReference fe_scope vv.VariableType
                if vd>=5 then vprintln 5 (sprintf "Parsed method local var %s idx=%i" name idx)
                mutadd m_vans (P3((if idx < 0 then None else Some idx), ty, name))

        rev !m_vans
    let initf = false // for now
    let locals =  CIL_locals(variables, initf) 


//    if xvd>=4 then vprintln 0 ("fe_method 4 name=" + id)

    let raw_ins_list =
        if meth.Body = null || meth.Body.Instructions = null then []
        else
            let m_raw_ins = ref []
            let _ = for ins0 in meth.Body.Instructions do mutadd m_raw_ins ins0 done
            rev !m_raw_ins

    let handler_info =
        if meth.Body = null then
            //if xvd>=4 then vprintln 3 (sprintf "Ignoring a null body for " + unique_id)
            []
        else
            let m_hi = ref []
            let proc_ex (ex:ExceptionHandler) =
                //if xvd>=4 then vprintln 0 (sprintf "Starting a handler groc " + unique_id)                        
                let ty = ex.Type // An enum: ExceptionHandlerType: it can be Catch, Filter, Finally or Fault.
                let tr = if ex.CatchType=null then None else Some(decode_TypeReference fe_scope ex.CatchType)
                let tf = if ex.FilterStart=null then None else Some(ex.FilterStart.Offset)
                if ex.TryEnd=null || ex.TryStart=null then vprintln 0 (sprintf "%A Null try" ty)
                elif ex.HandlerStart=null || ex.HandlerEnd=null then vprintln 0 (sprintf "%A Null handler" ty)
                else
                    //if xvd>=4 then vprintln 3 (sprintf "Exception handler cecilfe: method=%s: ex=%A ty=%A try=%i..%i, handler=%i..%i %s" unique_id ex ty ex.TryStart.Offset  ex.TryEnd.Offset   ex.HandlerStart.Offset ex.HandlerEnd.Offset (if nonep tr then "" else "tr=" + cil_typeToStr (valOf tr)))
                    mutadd m_hi (ex.TryStart.Offset, ex.TryEnd.Offset, ty, ex.HandlerStart.Offset, ex.HandlerEnd.Offset, tr)
                    ()
                ()
            for ex in meth.Body.ExceptionHandlers do proc_ex ex done
            //vprintln 3 (sprintf "Method %s: %i handlers" unique_id (length !m_hi))
            rev !m_hi

    let assimilate_raw_ins m_prefixes ins0 =
        let local_try_nesting = [] // This is re-inserted afterwards.
        let m_ans0 = ref []
        let ins:Instruction = ins0
        let opc:OpCode = ins.OpCode
        let code = ins.OpCode.Value

        let arg = ins.Operand
        let tt = opc.OperandType
        let tt1 = opc.OpCodeType
        let vale = opc.Value
        let ast = if arg=null then "" else arg.ToString()
        let opcs = opc.ToString()
        let (found, ov) = cil_ins_database.TryGetValue(opcs)
        let _ = if true then ()
                else
                match opc.OperandType with
                | OperandType.InlineString -> if vd >=5 then vprintln 5 "inline"
                | OperandType.InlineType -> if vd >=5 then vprintln 5 "inlinetype"
                | OperandType.ShortInlineVar -> if vd >=5 then vprintln 5 "shilnv inlinetype"
                
                | OperandType.InlineMethod -> if vd >=5 then vprintln 5 "method  inlinetype"

                | OperandType.InlineBrTarget -> if vd >=5 then vprintln 5 "brtarg inetype"
                | OperandType.InlineField -> if vd >=5 then vprintln 5 "brtarg inetype"
                | OperandType.InlineNone -> if vd >=5 then vprintln 5 "none"
                | other -> if vd >=5 then vprintln 5 ("+++ OTHER " + other.ToString())
        let offsets = i2s ins.Offset
        if vd>=5 then vprintln 5 ("ins "  + offsets + ": " + opcs + " ast=" + ast  + "//" + tt.ToString() + " " + found.ToString() + " " + tt1.ToString () + " opcode=" + vale.ToString())//ins.StackBehaviourPop )
        let xe =
            {
              formals=     formals
              fe_scope= fe_scope
              hasthis=     hasthis
            }
        let _ = if found
                then
                    match ov xe (ins) with
                        | Cili_prefix p ->
                            mutadd m_prefixes (ins.Offset, p)
                            ()
                        | v ->
                            if vd >=5 then vprintln 5 (" dis=" + ciliToStr v)
                            let ir = rev !m_prefixes
                            m_prefixes := [] // This requires this method is called in program order!
                            //let _ =  if not_nonep ir then dev_println (sprintf "prefixes on  dis=%s are %A" (ciliToStr v) ir)

                            let baseaddr = if nullp ir then ins.Offset else fst(hd ir)
                            mutadd m_ans0 (CIL_instruct(baseaddr, local_try_nesting, map snd ir, v)) 
                else sf ("cecilfe: +++CIL instruction not implemented yet : " + opcs)
        if vd>=5 then vprintln 5 "\n"
        rev !m_ans0

    let add_nesting_info nester cil_item =
        let rec addn = function
            | CIL_instruct(baseaddr, prof, ir, v) ->  CIL_instruct(baseaddr, nester::prof, ir, v)
            | CIL_try(nto, try_blk, (token, catchers, (nfo, finaly_blk)))  ->
                let af (p, co, lst) = (p, co, map addn lst)
                CIL_try(nto, map addn try_blk, (token, map af catchers, (nfo, map addn finaly_blk)))
            | other -> sf (sprintf "add_nesting_info other form %A" (cil_classitemsToStr "" [other ] ""))
        addn cil_item

            
    let try_block_starts pos = List.filter (fun x -> f1o6 x = pos) handler_info

    let advance_to pos lst =
        let rec scan = function
            | [] -> [] // Silent fail
            | (offset, h)::tt when offset = pos -> (offset, h)::tt
            | _:: tt -> scan tt
        scan lst
            
    let body =
        let m_r = ref []
        if nullp handler_info then list_flatten (map (assimilate_raw_ins m_r) raw_ins_list)
        else
            let converted_instructions = map (fun (ins:Instruction) -> (ins.Offset, assimilate_raw_ins m_r ins)) raw_ins_list
            let df lst = list_flatten (map snd lst) // Remove the markup and flatten.
            //if xvd>=4 then vprintln 3 (sprintf "method %s: %i instructions and %i handlers" unique_id (length converted_instructions) (length handler_info))
            let rec getsome nester stopat lookf work =
                match work with
                    | []    -> ([], [])
                    | (offset, (h:cil_t list))::tt ->
                        //if xvd>=4 then vprintln 0 (sprintf "Key offset=%i" offset)
                        if offset = stopat then ([], work)
                        else
                            let handlers_due = if lookf then try_block_starts offset else []
                            //if xvd>=4 then vprintln 0 (sprintf "Key offset=%i handlers=%i" offset (length handlers_due))
                            let _ =
                                let rec check_ordering = function
                                    | a::b::tt -> // Smallest one first is needed
                                        let _ = if f2o6 a > f2o6 b then sf (sprintf "Try blocks starting at %i need sorting" offset)
                                        check_ordering (b::tt)
                                    | _ -> ()
                                let _ = check_ordering handlers_due
                                ()
                            let rec apply_any_handlers work = function
                                | [] ->
                                    match work with
                                        | [] -> ([], [])
                                        | h1::tt1 ->
                                            let (b, leftover) = getsome nester stopat true tt1
                                            (h1::b, leftover)
                                | trier::tt ->
                                    let (try_code, leftover_) = getsome nester (f2o6 trier) false work
                                    let token = funique "ehandler"
                                    //let nester = token :: nester_ // not used
                                    // We fail to add nester code to handler and finally blocks at the moment - support for exceptions in those themselves can be considered at some future date.
                                    let try_code = map (add_nesting_info token) (df try_code)
                                    let (handler_code, leftover) = getsome nester (f5o6 trier) true (advance_to (f4o6 trier) work)
                                    let (catchers, final_code) =
                                        match f6o6 trier with
                                            | None    -> ([], df handler_code)
                                            | Some tr -> ([ (tr, None, df handler_code) ], [])

                                    let work = (offset, [CIL_try(None, try_code, (token, catchers, (None, final_code)))]) :: leftover
                                    apply_any_handlers work tt
                            apply_any_handlers work handlers_due
            let (ans, leftovers) = getsome [] -1 true converted_instructions
            df ans
    //if xvd>=4 then vprintln 0 ("fe_method 99 name=" + id)
    let flags1 =
        {
            is_startup_code=is_startup_code
        }
    CIL_method(fe_scope.srcfile, (cr,  cr_fap, rev !m_mgr), !flags, ck, ty, (Cil_cr id, unique_id), formals, flags1, prats @ locals :: body)
    



//-----------------------------------

and fe_field_definition fe_scope (field:Mono.Cecil.FieldDefinition) =
    let vd = !g_fevd
    let cr0 = Cil_cr (field.Name)
    
    let const_inito =
        if field.HasConstant then
            //if xvd>=4 then vprintln 3 (sprintf "cecilfe: field %A has constant %A" cr0 field.Constant)
            match field.Constant with
                | :? System.Int64   as constant_int -> Some(Cil_int_i(64, BigInteger constant_int))
                | :? System.UInt64  as constant_int -> Some(Cil_int_i(64, BigInteger constant_int))
                | :? System.Int32   as constant_int -> Some(Cil_number32(int constant_int))
                | :? System.UInt32  as constant_int -> Some(Cil_number32(int constant_int)) // unsigned?
                | :? System.Int16   as constant_int -> Some(Cil_number32(int constant_int))
                | :? System.UInt16  as constant_int -> Some(Cil_number32(int constant_int)) // unsigned?
                | :? System.Byte    as constant_int -> Some(Cil_number32(int constant_int))
                | :? System.SByte   as constant_int -> Some(Cil_number32(int constant_int)) // signed

                | :? System.Double  as constant_init -> Some(Cil_float64(constant_init)) 
                | :? System.Single  as constant_init -> Some(Cil_float32(float constant_init)) 

                | :? System.Boolean as bb           -> Some(Cil_number32(if bb then 1 else 0))
                | :? System.String  as ss           -> Some(Cil_string ss)
 

                | other -> sf (sprintf "field=%s: unsupported field constant type %s" field.Name (other.GetType().ToString()))
        else None
                    
    // The declaring type is not put on a field declaration: just its name.
    let cr = if true || field.DeclaringType=null
                  then cr0
                  else cil_cr_dot(decode_TypeReference fe_scope field.DeclaringType, field.Name)

    //let offset_ = Cil_field_at(field.Offset)
    let bytear = field.InitialValue
    //dev_println (sprintf "field %s blob find  const=%A initialValue=%A hasLayoutInfo=%A offset=%A" field.Name (not_nonep const_inito) bytear  field.HasLayoutInfo offset)

    let ty = decode_TypeReference fe_scope field.FieldType
    let ats = []
    let flags = ref []

    let init =
        if not_nonep const_inito then Cil_field_lit (valOf const_inito) 
        elif bytear= null || bytear.Length = 0 then Cil_field_none
        else
            let zl = ref []
            for x:byte in bytear do mutadd zl (Cil_number32(System.Convert.ToInt32 x)) done
            Cil_field_lit (Cil_blob(rev !zl)) 



    //if field.IsRuntime then mutadd flags Cilflag_runtime
    if field.IsPrivate then mutadd flags Cilflag_private
    if field.IsPublic then mutadd flags Cilflag_public
    if field.IsStatic then mutadd flags Cilflag_static
    if field.IsInitOnly then mutadd flags Cilflag_initonly
    if field.IsLiteral then mutadd flags Cilflag_literal
    if field.IsSpecialName then mutadd flags Cilflag_specialname
    if field.IsRuntimeSpecialName then mutadd flags Cilflag_rtspecialname

    // For fields, monodis put the ca before the field, but with cecil we can put it inside.
    let m_attributes = ref []
    let _ =
        for ca:CustomAttribute in field.CustomAttributes do
        if vd>=5 then vprintln 5 ("field has custom " + ca.ToString())
        let r = fe_custom fe_scope ca
        
        mutadd m_attributes r
    //vprintln 0 (sprintf "Field name ast %A" cr)
    [ CIL_field(None, !flags, ty, cr, init, rev !m_attributes) ]





//
// Foldback style   
//  ... CIL_class reader - assumes no global methods or global fields ... not correct for C++
//    
and fe_type fe_scope (classa: Mono.Cecil.TypeDefinition) cc = 
    let vd = !g_fevd

    let ns = classa.Namespace 
    let myname = disam classa.Name

    let my_cr =
        if classa.DeclaringType <> null then
            let p = decode_TypeReference fe_scope classa.DeclaringType
            cil_cr_dot(p, myname)
        elif ns <> null && ns <> "" then Cil_namespace(ce_dedot ns, Cil_cr myname)
        else Cil_cr(myname)

    if vd >= 4 then vprintln 4 (sprintf "fe_type: srcfile=%s Class name ast %s.  HasNestedTypes=%A  MethodArity=%i  Stack=[%s]" fe_scope.srcfile myname classa.HasNestedTypes  classa.Methods.Count (sfold id fe_scope.stack))

    let my_cr =
        match my_cr with
            | Cil_cr "<Module>" when false ->
                if vd>=3 then vprintln 3 (sprintf "cr is special global flag %A. Using assembly name %s instead " my_cr fe_scope.AssemblyName)
                Cil_cr fe_scope.AssemblyName
            | other -> other

    let render_key = cil_typeToStr my_cr
    let fe_scope = { fe_scope with stack= render_key :: fe_scope.stack }
    if render_key <> "<Module>" && memberp render_key !g_ast_rendered then
        if vd>=5 then vprintln 5 (sprintf "Already rendered key=%s myname=%s" render_key myname)
        cc
    elif fe_scope.lazy_pe_reading > 0 then
        if vd>=5 then vprintln 5 ("Lazybones lazy render for  " + myname)
        gec_CIL_lazybones(myname, render_key, (fun ww -> fe_type { fe_scope with lazy_pe_reading=fe_scope.lazy_pe_reading-1} classa []))::cc
    else
    let m_classans = ref []
    let flags = ref []

    if classa.IsAbstract then mutadd flags Cilflag_abstract
    if classa.IsSealed then mutadd flags Cilflag_sealed
    if classa.IsEnum then mutadd flags Cilflag_enum
    if classa.IsValueType then mutadd flags Cilflag_valuetype
    if classa.IsRuntimeSpecialName then mutadd flags Cilflag_rtspecialname
    if classa.IsSpecialName then mutadd flags Cilflag_specialname
    if classa.IsAnsiClass then mutadd flags Cilflag_ansi
    if classa.IsAutoClass then mutadd flags Cilflag_auto
    if classa.IsBeforeFieldInit then mutadd flags Cilflag_beforefieldinit
    // IsNestedFamilyAndAssembly IsClass  IsImport IsUnicodeClass 
    if classa.IsNestedPublic then mutapp flags [ Cilflag_nested ; Cilflag_public ]
    if classa.IsNestedPrivate then mutapp flags [ Cilflag_nested ; Cilflag_private ]
    if classa.IsNestedFamily then mutapp flags [ Cilflag_nested ; Cilflag_family ]
    if classa.IsNestedAssembly then mutapp flags [ Cilflag_nested ; Cilflag_assembly ]

    //if classa.IsPartial then mutadd flags Cilflag_partial
    //if classa.IsRuntime then mutadd flags Cilflag_runtime
    //if classa.IsPrivate then mutadd flags Cilflag_private
    if classa.IsPublic then mutadd flags Cilflag_public
    if classa.IsInterface then mutadd flags Cilflag_interface
    if classa.IsSerializable then mutadd flags Cilflag_serializable
    //if classa.Is then mutadd flags Cilflag_


    let extendso =
        if classa.BaseType=null
        then None
        else
            let toz_dt = classa.BaseType.DeclaringType
            let toz = "classa.NameSpace"
            if vd>=5 then vprintln 5 ("parent's full name " + (if toz=null then "NulL" else "toz.FullName"))
            match decode_TypeReference fe_scope classa.BaseType with
                | Cil_namespace(Cil_cr "System", Cil_cr "Object") -> None
                | x -> Some x                

    let avd = -1  // 
    //lprintln avd (fun() -> "Fields of " + classa.ToString())
    for x in classa.Fields do mutapptail m_classans (fe_field_definition fe_scope x) done
    //if avd>=4 then vprintln 4 "Customs"
    for att in classa.CustomAttributes do mutaddtail m_classans (fe_custom fe_scope att) done
    //if avd>=4 then vprintln 4 "Methods"

    let cr_fap =
        let titems = ref []
        let _ =
            for gp:GenericParameter in classa.GenericParameters do
                let methodf = // If the owner is a type then this is a class generic, else a method generic. Or call gp.Type=GenericParameterType.Method in future.
                    match gp.Owner with
                        | :? TypeReference -> false
                        | _ -> true

                let idx = gp.Position
                let name = gp.Name
                let aa = Cil_tp_arg(methodf, idx, name)
                mutadd titems aa
                let fullname = gp.FullName
                if vd>=5 then vprintln 5 ("  gp idx=" + i2s idx + ", name=" + name + ", fullname=" + fullname)
                ()
        !titems

    if vd>=5 then vprintln 5 (sprintf "fe_type: recap srcfile=%s Class name ast %s.  HasNestedTypes=%A MethodArity=%i" fe_scope.srcfile myname classa.HasNestedTypes  classa.Methods.Count)
    //vprintln avd "Assimilating Methods"
    let _ =
        let smeth meth =
            //if avd>=4 then vprintln 4 (sprintf "invoke fe_method")
            (fe_method fe_scope my_cr cr_fap meth)
        for meth in classa.Methods do mutaddtail m_classans (smeth meth) done


    if avd>=3 then vprintln 3 "Assimilating Constructors"
    for meth in classa.Constructors do mutaddtail m_classans (fe_method fe_scope my_cr cr_fap meth) done
    if avd>=3 then vprintln 3 "Assimilating Nested Types"
    for nt in classa.NestedTypes do m_classans := fe_type fe_scope nt !m_classans done
    mutadd g_ast_rendered render_key
    if vd>=4 then vprintln 4 (sprintf "Marking as rendered key=%s myname=%s" render_key myname)
    CIL_class(fe_scope.srcfile, !flags, my_cr, cr_fap, extendso, !m_classans)::cc


    
let kiwi_readin_exe_or_dll ww is_exe name =
    let vd = !g_fevd
    let msg = "KiwiC front end : processing CIL PE filename=" + name
    let ww = WF 2 "cecilfe" ww ("starting parse/readin of " + msg)
    if vd>=5 then vprintln 5 msg
    let srcfile = name // Could wrap this name up with further meta info for srcfile xreferencing.
    let fe_scope = { srcfile=srcfile; lazy_pe_reading= -10000; AssemblyName="anon"; stack=[]; }
    g_cecilfe_srcfiletoken := Some (funique (filename_sanitize [] srcfile))



    // No sign of pathopen here - perhaps use incdir for dll files as well as IP-XACT.
    // Full pathname still needed
    // Use this:
    // let name = pathopen searchpath vd delims file (suffix:string)

    let ef = existsFile name
    if not ef then
        dev_println (sprintf "dll/exe file exists %s %A" name ef)
        cleanexit(sprintf "KiwiC: fatal error: cannot find .exe or .dll called '%s'" name)
    else        
        let assembly = Mono.Cecil.AssemblyFactory.GetAssembly name
        let m_topans = ref [ CIL_comment ("END PARSE TREE of "  + name)]
        let ep_name__ = // TODO perhaps use
            if assembly.EntryPoint = null then None
            else
                vprintln 3 (sprintf "Assembly '%s' EntryPoint name is %A" assembly.Name.Name assembly.EntryPoint.Name)
                mutadd m_topans (CIL_entrypoint assembly.EntryPoint.Name)
                Some(assembly.EntryPoint.Name)
        let ww = WF 2 "cecilfe" ww ("assimilate assembly " + assembly.Name.Name)
        let settings' = { fe_scope with AssemblyName=assembly.Name.Name }
        vprintln 3 (sprintf "fe_type invoking %i times" assembly.MainModule.Types.Count)
        for classa:Mono.Cecil.TypeDefinition in assembly.MainModule.Types do m_topans := fe_type settings' classa !m_topans    
        let ww = WF 2 "cecilfe" ww ("finished parse/readin of " + msg)
        mutadd m_topans (CIL_comment ("START PARSE TREE of "  + name))
        (is_exe, name, !m_topans)


// Not rendered:
//  CIL_module
//  CIL_maxstack etc ...

    
define_ins("nop",     fun xe (x) -> Cili_nop)
//define_ins("break", fun xe (x) -> Cili_break)
define_ins("ldarg.0", fun xe (x) -> Cili_ldarg(Cil_suffix_0))
define_ins("ldarg.1", fun xe (x) -> Cili_ldarg(Cil_suffix_1))
define_ins("ldarg.2", fun xe (x) -> Cili_ldarg(Cil_suffix_2))
define_ins("ldarg.3", fun xe (x) -> Cili_ldarg(Cil_suffix_3))
define_ins("ldloc.0", fun xe (x) -> Cili_ldloc(Cil_suffix_0))
define_ins("ldloc.1", fun xe (x) -> Cili_ldloc(Cil_suffix_1))
define_ins("ldloc.2", fun xe (x) -> Cili_ldloc(Cil_suffix_2))
define_ins("ldloc.3", fun xe (x) -> Cili_ldloc(Cil_suffix_3))
define_ins("stloc.0", fun xe (x) -> Cili_stloc(Cil_suffix_0))
define_ins("stloc.1", fun xe (x) -> Cili_stloc(Cil_suffix_1))
define_ins("stloc.2", fun xe (x) -> Cili_stloc(Cil_suffix_2))
define_ins("stloc.3", fun xe (x) -> Cili_stloc(Cil_suffix_3))

define_ins("ldarg.s",  fun xe (x) -> Cili_ldarg(*.s*)  (decode_i4_argref xe x))
define_ins("ldarga.s", fun xe (x) -> Cili_ldarga(*.s*) (decode_i4_argref xe x))
define_ins("starg.s",  fun xe (x) -> Cili_starg(*.s*)  (decode_i4_argref xe x))

define_ins("ldloc.s",  fun xe (x) -> Cili_ldloc(*.s*)  (decode_i4 xe x))
define_ins("ldloca.s", fun xe (x) -> Cili_ldloca(*.s*) (decode_i4 xe x))
define_ins("stloc.s",  fun xe (x) -> Cili_stloc(*.s*)  (decode_i4 xe x))
define_ins("ldnull",   fun xe  (x) -> Cili_ldnull)
define_ins("ldc.i4.m1", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_m1))
define_ins("ldc.i4.0", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_0))
define_ins("ldc.i4.1", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_1))
define_ins("ldc.i4.2", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_2))
define_ins("ldc.i4.3", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_3))
define_ins("ldc.i4.4", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_4))
define_ins("ldc.i4.5", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_5))
define_ins("ldc.i4.6", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_6))
define_ins("ldc.i4.7", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_7))
define_ins("ldc.i4.8", fun xe (x) -> Cili_ldc (* .i4 *)(gec_vt(K_int, 32), Cil_suffix_8))
define_ins("ldc.i4.s", fun xe (x) -> Cili_ldc (* .i4 *)(*.s*) (gec_vt(K_int, 32), decode_i4 xe x))
define_ins("ldc.i4", fun xe (x) -> Cili_ldc(gec_vt(K_int, 32), decode_i4 xe x))
define_ins("ldc.i8", fun xe (x) -> Cili_ldc(gec_vt(K_int, 64), decode_i8 x))
define_ins("ldc.r4", fun xe (x) -> Cili_ldc(gec_vt(K_float, 32), decode_r4 xe x))
define_ins("ldc.r8", fun xe (x) -> Cili_ldc(gec_vt(K_float, 64), decode_r8 xe x))
define_ins("dup", fun xe (x) -> Cili_dup)
define_ins("pop", fun xe (x) -> Cili_pop)
//define_ins("jmp", fun xe (x) -> Cili_jmp)


define_ins("callvirt", fun xe (x) -> fe_call xe.fe_scope KM_virtual x)
define_ins("call", fun xe (x)     -> fe_call xe.fe_scope KM_call x)
           
//define_ins("calli", fun xe (x) -> Cili_calli)
define_ins("ret", fun xe (x) -> Cili_ret)
define_ins("br.s", fun xe (x) -> Cili_br(decode_branch x))

define_ins("brfalse.s", fun xe (x)  -> Cili_brfalse(*.s*) ([], decode_branch x))
define_ins("brtrue.s", fun xe (x)   -> Cili_brtrue(*.s*) (decode_branch x))

define_ins("beq.s", fun xe (x) -> Cili_beq(*.s*)(decode_branch x))
define_ins("bge.s", fun xe (x) -> Cili_bge(*.s*)(decode_branch x))
define_ins("bgt.s", fun xe (x) -> Cili_bgt(*.s*)(decode_branch x))
define_ins("ble.s", fun xe (x) -> Cili_ble(*.s*)(decode_branch x))
define_ins("blt.s", fun xe (x) -> Cili_blt(*.s*)(decode_branch x))


define_ins("bge.un.s", fun xe (x) -> Cili_bge_un(decode_branch x))
define_ins("bgt.un.s", fun xe (x) -> Cili_bgt_un(decode_branch x))
define_ins("ble.un.s", fun xe (x) -> Cili_ble_un(decode_branch x))
define_ins("blt.un.s", fun xe (x) -> Cili_blt_un (decode_branch x))

define_ins("bge.un", fun xe (x) -> Cili_bge_un(decode_branch x))
define_ins("bgt.un", fun xe (x) -> Cili_bgt_un(decode_branch x))
define_ins("ble.un", fun xe (x) -> Cili_ble_un(decode_branch x))
define_ins("blt.un", fun xe (x) -> Cili_blt_un (decode_branch x))

define_ins("br", fun xe (x)        -> Cili_br(decode_branch x))
define_ins("brfalse", fun xe (x)   -> Cili_brfalse([], decode_branch x))
define_ins("brtrue", fun xe (x)    -> Cili_brtrue(decode_branch x))

define_ins("beq", fun xe (x) -> Cili_beq(decode_branch x))
define_ins("bge", fun xe (x) -> Cili_bge(decode_branch x))
define_ins("bgt", fun xe (x) -> Cili_bgt(decode_branch x))
define_ins("ble", fun xe (x) -> Cili_ble(decode_branch x))
define_ins("blt", fun xe (x) -> Cili_blt(decode_branch x))

// Separate signed and unsigned versions of bne and beq make no sense for integer types but have, ideally, slightly different behaviour for floating point comparisons between the two compare operators.  We ignore the differences here. For floating-point number, ceq will return 0 if the numbers are unordered (either or both are NaN).
define_ins("bne.un", fun xe (x)   -> Cili_bne(decode_branch x))
define_ins("beq.un", fun xe (x)   -> Cili_beq(decode_branch x))
define_ins("bne.un.s", fun xe (x) -> Cili_bne(decode_branch x))
define_ins("beq.un.s", fun xe (x) -> Cili_beq(decode_branch x))


define_ins("switch",   fun xe (x) -> Cili_switch(decode_switch x))
define_ins("ldind.i1", fun xe (x) -> Cili_ldind(Cil_suffix_i1))
define_ins("ldind.u1", fun xe (x) -> Cili_ldind(Cil_suffix_u1))
define_ins("ldind.i2", fun xe (x) -> Cili_ldind(Cil_suffix_i2))
define_ins("ldind.u2", fun xe (x) -> Cili_ldind(Cil_suffix_u2))
define_ins("ldind.i4", fun xe (x) -> Cili_ldind(Cil_suffix_i4))
define_ins("ldind.u4", fun xe (x) -> Cili_ldind(Cil_suffix_u4))
define_ins("ldind.i8", fun xe (x) -> Cili_ldind(Cil_suffix_i8))
define_ins("ldind.i", fun xe (x)  -> Cili_ldind(Cil_suffix_i))
define_ins("ldind.r4", fun xe (x) -> Cili_ldind(Cil_suffix_r4))
define_ins("ldind.r8", fun xe (x) -> Cili_ldind(Cil_suffix_r8))
define_ins("ldind.ref", fun xe (x) -> Cili_ldind(Cil_suffix_ref))
define_ins("stind.ref", fun xe (x) -> Cili_stind(Cil_suffix_ref))
define_ins("stind.i1", fun xe (x) -> Cili_stind(Cil_suffix_i1))
define_ins("stind.i2", fun xe (x) -> Cili_stind(Cil_suffix_i2))
define_ins("stind.i4", fun xe (x) -> Cili_stind(Cil_suffix_i4))
define_ins("stind.i8", fun xe (x) -> Cili_stind(Cil_suffix_i8))
define_ins("stind.r4", fun xe (x) -> Cili_stind(Cil_suffix_r4))
define_ins("stind.r8", fun xe (x) -> Cili_stind(Cil_suffix_r8))
define_ins("stind.i", fun xe (x)  -> Cili_stind(Cil_suffix_i))

define_ins("add.ovf",    fun xe (x) -> Cili_add Cil_signed); // overflow ignored for now
define_ins("add.ovf.un", fun xe (x) -> Cili_add Cil_unsigned)
define_ins("mul.ovf",    fun xe (x) -> Cili_mul Cil_signed)
define_ins("mul.ovf.un", fun xe (x) -> Cili_mul Cil_unsigned)
define_ins("sub.ovf",    fun xe (x) -> Cili_sub Cil_signed)
define_ins("sub.ovf.un", fun xe (x) -> Cili_sub Cil_unsigned)

define_ins("add", fun xe (x) -> Cili_add Cil_signed)
define_ins("sub", fun xe (x) -> Cili_sub Cil_signed)
define_ins("mul", fun xe (x) -> Cili_mul Cil_signed)
define_ins("div",    fun xe (x) -> Cili_div Cil_signed)
define_ins("div.un", fun xe (x) -> Cili_div Cil_unsigned)
define_ins("rem", fun xe (x) -> Cili_rem Cil_signed)
define_ins("rem.un", fun xe (x) -> Cili_rem Cil_unsigned)
define_ins("and", fun xe (x) -> Cili_and)
define_ins("or", fun xe (x) -> Cili_or)
define_ins("xor", fun xe (x) -> Cili_xor)
define_ins("shl", fun xe (x) -> Cili_shl Cil_signed)
define_ins("shr", fun xe (x) -> Cili_shr Cil_signed)
define_ins("shr.un", fun xe (x) -> Cili_shr Cil_unsigned)
define_ins("neg", fun xe (x) -> Cili_neg)
define_ins("not", fun xe (x) -> Cili_not)

define_ins("conv.u2", fun xe (x) -> Cili_conv(Cil_suffix_u2, false))
define_ins("conv.u1", fun xe (x) -> Cili_conv(Cil_suffix_u1, false))
define_ins("conv.i", fun xe (x) -> Cili_conv(Cil_suffix_i4, false))
define_ins("conv.u", fun xe (x) -> Cili_conv(Cil_suffix_u4, false))

define_ins("conv.ovf.i", fun xe (x) -> Cili_conv(Cil_suffix_i4, true))
define_ins("conv.ovf.u", fun xe (x) -> Cili_conv(Cil_suffix_u4, true))

//define_ins("conv.ovf.i1.un", fun xe (x) -> Cili_conv.ovf.i1.un)
//define_ins("conv.ovf.i2.un", fun xe (x) -> Cili_conv.ovf.i2.un)
//define_ins("conv.ovf.i4.un", fun xe (x) -> Cili_conv.ovf.i4.un)
//define_ins("conv.ovf.i8.un", fun xe (x) -> Cili_conv.ovf.i8.un)
//define_ins("conv.ovf.u1.un", fun xe (x) -> Cili_conv.ovf.u1.un)
//define_ins("conv.ovf.u2.un", fun xe (x) -> Cili_conv.ovf.u2.un)
//define_ins("conv.ovf.u4.un", fun xe (x) -> Cili_conv.ovf.u4.un)
//define_ins("conv.ovf.u8.un", fun xe (x) -> Cili_conv.ovf.u8.un)

// The difference between these two is ... different overflow conditions ?
// ...Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow
define_ins("conv.ovf.i.un", fun xe (x) -> Cili_conv(Cil_suffix_i4, true))
define_ins("conv.ovf.u.un", fun xe (x) -> Cili_conv(Cil_suffix_i4, true))

define_ins("conv.i1", fun xe (x) -> Cili_conv(Cil_suffix_i1, false))
define_ins("conv.i2", fun xe (x) -> Cili_conv(Cil_suffix_i2, false))
define_ins("conv.i4", fun xe (x) -> Cili_conv(Cil_suffix_i4, false))
define_ins("conv.i8", fun xe (x) -> Cili_conv(Cil_suffix_i8, false))
define_ins("conv.r4", fun xe (x) -> Cili_conv(Cil_suffix_r4, false))
define_ins("conv.r8", fun xe (x) -> Cili_conv(Cil_suffix_r8, false))
define_ins("conv.u4", fun xe (x) -> Cili_conv(Cil_suffix_u4, false))
define_ins("conv.u8", fun xe (x) -> Cili_conv(Cil_suffix_u8, false))

define_ins("conv.ovf.i1", fun xe (x) -> Cili_conv(Cil_suffix_i1, true))
define_ins("conv.ovf.i2", fun xe (x) -> Cili_conv(Cil_suffix_i2, true))
define_ins("conv.ovf.i4", fun xe (x) -> Cili_conv(Cil_suffix_i4, true))
define_ins("conv.ovf.i8", fun xe (x) -> Cili_conv(Cil_suffix_i8, true))
define_ins("conv.ovf.r4", fun xe (x) -> Cili_conv(Cil_suffix_r4, true))
define_ins("conv.ovf.r8", fun xe (x) -> Cili_conv(Cil_suffix_r8, true))
define_ins("conv.ovf.u4", fun xe (x) -> Cili_conv(Cil_suffix_u4, true))
define_ins("conv.ovf.u8", fun xe (x) -> Cili_conv(Cil_suffix_u8, true))

define_ins("conv.r.un", fun xe (x) -> Cili_conv(Cil_suffix_r4, false)) // Same as conv.r4  - why is this a different instruction ?  TODO.

//define_ins("cpobj", fun xe (x) -> Cili_cpobj)
define_ins("ldobj", fun xe (x) -> Cili_ldobj(decode_classref xe.fe_scope x))
define_ins("ldstr", fun xe (x) -> Cili_ldstr(decode_str x))
define_ins("newobj", fun xe (x) -> (fe_newobj xe.fe_scope x))
define_ins("castclass", fun xe (x) -> Cili_castclass(decode_classref xe.fe_scope x))
define_ins("isinst", fun xe (x) -> Cili_isinst(false, decode_classref xe.fe_scope x))

define_ins("unbox", fun xe (x) -> Cili_unbox(decode_classref xe.fe_scope x))
define_ins("throw", fun xe (x) -> Cili_throw)
define_ins("ldfld", fun xe (x) -> Cili_ldfld(decode_fieldref xe.fe_scope x))
define_ins("ldflda", fun xe (x) -> Cili_ldflda(decode_fieldref xe.fe_scope x))
define_ins("stfld", fun xe (x) -> Cili_stfld(decode_fieldref xe.fe_scope x))
define_ins("ldsfld", fun xe (x) -> Cili_ldsfld(decode_fieldref xe.fe_scope x))
define_ins("ldsflda", fun xe (x) -> Cili_ldsflda(decode_fieldref xe.fe_scope x))
define_ins("stsfld", fun xe (x) -> Cili_stsfld(decode_fieldref xe.fe_scope x))
define_ins("stobj", fun xe (x) -> Cili_stobj(decode_classref xe.fe_scope x))
define_ins("box", fun xe (x) -> Cili_box(decode_classref xe.fe_scope x))
define_ins("newarr", fun xe (x) -> Cili_newarr(decode_classref xe.fe_scope x))
define_ins("ldlen", fun xe (x) -> Cili_ldlen)
define_ins("ldelema", fun xe (x) -> Cili_ldelema(decode_classref xe.fe_scope x))
define_ins("ldelem.i1", fun xe (x) -> Cili_ldelem(Cil_suffix_i1))
define_ins("ldelem.u1", fun xe (x) -> Cili_ldelem(Cil_suffix_u1))
define_ins("ldelem.i2", fun xe (x) -> Cili_ldelem(Cil_suffix_i2))
define_ins("ldelem.u2", fun xe (x) -> Cili_ldelem(Cil_suffix_u2))
define_ins("ldelem.i4", fun xe (x) -> Cili_ldelem(Cil_suffix_i4))
define_ins("ldelem.u4", fun xe (x) -> Cili_ldelem(Cil_suffix_u4))
define_ins("ldelem.i8", fun xe (x) -> Cili_ldelem(Cil_suffix_i8))

define_ins("ldelem.r4", fun xe (x) -> Cili_ldelem(Cil_suffix_r4))
define_ins("ldelem.r8", fun xe (x) -> Cili_ldelem(Cil_suffix_r8))

define_ins("ldelem.any", fun xe (x) -> Cili_ldelem(decode_classref xe.fe_scope x))
define_ins("stelem.any", fun xe (x) -> Cili_stelem(decode_classref xe.fe_scope x))
define_ins("ldelem.ref", fun xe (x) -> Cili_ldelem(Cil_suffix_ref))
define_ins("stelem.ref", fun xe (x) -> Cili_stelem(Cil_suffix_ref))

define_ins("ldelem.i", fun xe (x) -> Cili_ldelem(Cil_suffix_i4))
define_ins("stelem.i", fun xe (x) -> Cili_stelem(Cil_suffix_i4))


define_ins("stelem.i1", fun xe (x) -> Cili_stelem(Cil_suffix_i1))
define_ins("stelem.i2", fun xe (x) -> Cili_stelem(Cil_suffix_i2))
define_ins("stelem.i4", fun xe (x) -> Cili_stelem(Cil_suffix_i4))
define_ins("stelem.i8", fun xe (x) -> Cili_stelem(Cil_suffix_i8))
define_ins("stelem.r4", fun xe (x) -> Cili_stelem(Cil_suffix_r4))
define_ins("stelem.r8", fun xe (x) -> Cili_stelem(Cil_suffix_r8))

//define_ins("ldelem.any", fun xe (x) -> Cili_ldelem_any)
//define_ins("stelem.any", fun xe (x) -> Cili_stelem_any)
define_ins("unbox.any", fun xe (x) -> Cili_unbox_any(decode_classref xe.fe_scope x))



//define_ins("ckfinite", fun xe (x) -> Cili_ckfinite)

// This set of 3 is not yet full implemented but do not present any problems - the CE_tv pair could be use again.
//mkrefany    - converts ToS to a typeRef
//refanytype  - deconstruct a refany to its type part
//refanyval   - deconstruct a refany to its value type
//define_ins("mkrefany", fun xe (x)   -> Cili_mkrefany)
//define_ins("mkrefanytype", fun xe (x) -> Cili_mkrefanytype)
define_ins("refanyval", fun xe (x)  -> Cili_refanyval(decode_classref xe.fe_scope x))    


define_ins("ldtoken", fun xe (x)     -> Cili_ldtoken(tidytok_decode_fieldref xe.fe_scope x))
define_ins("endfinally", fun xe (x)  -> Cili_endfinally)
define_ins("leave", fun xe (x)       -> Cili_leave(decode_branch x))
define_ins("leave.s", fun xe (x)     -> Cili_leave(*.s*) (decode_branch x))


define_ins("ceq", fun xe (x) -> Cili_ceq)
define_ins("cgt", fun xe (x) -> Cili_cgt Cil_signed)
define_ins("cgt.un", fun xe (x) -> Cili_cgt Cil_unsigned)
define_ins("clt", fun xe (x) -> Cili_clt Cil_signed)
define_ins("clt.un", fun xe (x) -> Cili_clt Cil_unsigned)
define_ins("ldftn", fun xe (x) -> fe_ldftn xe.fe_scope x)
//define_ins("ldvirtftn", fun xe (x) -> Cili_ldvirtftn)

define_ins("ldarg", fun xe (x) -> Cili_ldarg(decode_i4 xe x))
define_ins("ldarga", fun xe (x) -> Cili_ldarga(decode_i4 xe x))
define_ins("starg", fun xe (x) -> Cili_starg(decode_i4 xe x))

define_ins("ldloc", fun xe (x) -> Cili_ldloc(decode_i4 xe x))
define_ins("ldloca", fun xe (x) -> Cili_ldloca(decode_i4 xe x))
define_ins("stloc", fun xe (x) -> Cili_stloc(decode_i4 xe x))

// Prefixes/Prefixae. Ending with a dot.
let define_ins_prefix(ss, fn) = define_ins(ss, fn)
                                            
define_ins_prefix("volatile.", fun xe (x)    -> Cili_prefix Cilip_volatile)
define_ins_prefix("readonly.", fun xe (x)    -> Cili_prefix Cilip_readonly)
define_ins_prefix("constrained.", fun xe (x) -> Cili_prefix(Cilip_constrained(decode_fieldref xe.fe_scope x)))
define_ins_prefix("unaligned.", fun xe (x)   -> Cili_prefix Cilip_unaligned)

// End of Prefixae.


define_ins("arglist", fun xe (x) -> Cili_arglist)

//define_ins("localloc", fun xe (x) -> Cili_localloc)
//define_ins("endfilter", fun xe (x) -> Cili_endfilter)



//define_ins("tail.", fun xe (x) -> Cili_tail)
define_ins("initobj", fun xe (x) -> Cili_initobj(decode_classref xe.fe_scope x))

// C++ only
define_ins("cpblk", fun xe (x)   -> Cili_cpblk)      // aka memcpy(dest, src, length_in_bytes)
define_ins("initblk", fun xe (x) -> Cili_initblk)  // aka memset(dest, value, length_in_bytes)




//define_ins("rethrow", fun xe (x) -> Cili_rethrow)
//define_ins("sizeof", fun xe (x) -> Cili_sizeof)




// eof
