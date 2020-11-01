// 
// Kiwi Scientific Acceleration: Kiwic Compiler.
//
// firstpass.fs: Perform first pass and other boring code set up functions.
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


module firstpass

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open moscow
open yout
open meox
open hprls_hdr
open abstract_hdr
open abstracte
open tableprinter
open protocols
open ksc

// Kiwi-specific:
open kiwi_canned
open cilgram
open asmout
open linepoint_hdr
open kcode


let hptos_net = htos_net // for now

let g_unif_loglevel = ref -1 // -1 for off. Can set via recipe -kiwife-overloads-loglevel=0.

type tycontrol_t =
    {
        grounded: bool;
    }

let g_kludgecgr = false


let g_object_overhead = 8 // Every object contains a lock mutex at offset zero so this must be nonzero. Other housekeeping could be added.

let g_reflection_token_prec = { signed=Signed; widtho=Some 64 } // TODO a differing definition of this exists?


let g_firstpass_vd = ref -1 // Normally -1 for off else a veru low amount of firstpass logging.
let g_gtrace_vd =    ref -1
let g_kiwi_autodispose_enabled = ref false
let g_spillidx = 256


let g_array_boll = // canned partial 
            { name=       ["Array";" System"]
              structf=    false
              arity=      0
              ats=        []
              parent=     None
              class_item_types=[] // All were hardcoded. Now IndexOf and some other are in Kiwic.dll
              class_tag_list=[]
              newformals= ["array_content_type"]
              whiteformals= [ ("array_content_type", CTVar{ g_def_CTVar with id=Some "array_content_type"})]  
              vtno= None;
              m_next_vt=      ref 1
              sealedf=        true
              remote_marked=  None
            }

type Sparer_t = Sparer

type spawned_thread_t =
    |  USER_THREAD1 of string * cil_eval_t * cil_eval_t * string * stringl * cil_eval_t list * env_t // src file, object reference, method reference, spawntoken, mystery?, args and environment
    |  ROOT_THREAD1 of stringl * stringl option * string

type cilpass_t =
     {
          tid3:                    tid3_t                      // Thread id 
          m_spawned_threads:       spawned_thread_t list ref   //
          marked_end_of_elab:      int option ref              // User's annotation of end of elaboration
          expanded_items_:         string list list ref        // not used once colouring is used
          m_shared_var_fails:      (string * string) list ref  // Singletons/variables that have been assumed not-volatile and now found to be volatile - list of threads needing recompile on modified assumptions.
          sptok:                   int ref                     // Static token for next spawned thread.
     }

type aedge_t = E_fwd | E_back | E_cross  | E_exit


let edgeToStr = function
    | E_fwd ->"FWD"
    | E_back ->"BACK"
    | E_cross ->"CROSS"
    | E_exit ->"EXIT"


type spillvar_wrapper_t =
    | SP_none
    | SP_zygote of string list
    | SP_full of cil_eval_t


// Instruction array type: the type of a basic block.
type ia_t = (exn_handler_desc_t list * cil_t list) array

// Information about a method on its first pass 
type control_flow_set_t = int * int list // Subgraph of basic blocks in one final or catch block.

type firstpass_t =  
     { 
         idl:                   string list
         fvd:                   logger_t
         instcount:             int
         localnames:            (string * stringl * cil_eval_t * typeinfo_t * int) option array // the local vars to a method
         argtypes:         typeinfo_t option array // the arg types for this method
         ia:               ia_t

         il : int
         labs_si:          (string * int) list    // Label directory, string to int
         labs_oi:          (int * int) list       // Label directory, offset int to block int
         labs_is:          (int * string) list    // Label inverse directory: integer to string 

         spillage:         spillvar_wrapper_t array
         spillin:          int list array         // List of spill registers to load to the stack on entry to basic block.
         spillout:         int list option array  // List of block numbers whose spill regs we must save the stack in on block exit.
         edge_style:       (int * aedge_t) list array
         xedge_count:           int array 
         droppings:             (string list * dropping_data_t) list
         reachables:            int list              // Reachable PC states
         mdt:                   method_struct_t       // Raw method data
         exn_handler_directory: (string * exn_handler_desc_t) list           
         control_flow_sets:     control_flow_set_t list
     }




// Bindings for current class.
// We want different fields active for type checking versus code generation.
type cil_cl_textual_t = // static chain
     {
       cl_prefix:    stringl  // cl_prefix : Implied nesting namespace/scope for interpreting names.
       cl_codefix:   stringl  // cl_codefix is a static chain; textual name nesting. The main callstring.
       cm_generics:  annotation_map_t // Combined class and method generics
       this_dt:      typeinfo_t option
     }


type cil_cl_dynamic_t = // dynamic chain for a method.
     {
       text:         cil_cl_textual_t
       cl_localfix:  stringl   // Localfix wass a dynamic chain, stack depth nesting. This was the basis of the old, simple register colouring.
       this:         cil_eval_t option
     }



type director_spec_t = string option * performance_target_t option * directorate_style_t * (string * string) list * directorate_hangmode_t

// Bindings for current method or try/catch/finally region thereof.
type cilbind_t =
        { 
           fieldat:            cil_t option ref
           retlab:             (bool * string) (* top/main flag and return/jmp label.  *)
           prefix:             string list 
           codefix:            string list 
           simple_localfix:    string list                 // Old register colouring mechanism

           director_ref:       director_spec_t

           //tailhangf:          bool                        // Whether to hang forever on thread end - can go inside the director please
           cpoint:             (int * int) option  ref     // Interpreting checkpoint in current method: block and offset: null stack.
           srcfile:            string                      // Current source file name for error messages. Should be in top of linepoint_stack now.
           local_vregs_used:   virtual_reg_info_t list ref // locals includes spills
           exn_handler_stack_:  (string * exn_handler_desc_t) list // Unused.

           staticated:         stringl                     // Set to a class name when we are compiling a dynamic definition (class/field) as the top-level compilation root
         }


// This is the table of mappings allowed by box/unbox.  CTL_vt is essentially the same as CTL_net and it is silly to continue with both forms.
let boxing_table = [
 (CT_star(1, CTL_void),                 [ "System"; "IntPtr"]); 
 // The following definition may be preferable.
 //(g_canned_IntPtr,                      [ "System"; "IntPtr"]);
 (CTL_vt(K_int, 8, g_canned_i8),        [ "System"; "SByte"]); 
 (CTL_vt(K_int, 16, g_canned_i16),      [ "System"; "Int16"]);
 (CTL_vt(K_int, 32, g_canned_i32),      [ "System"; "Int32"]);
 (CTL_vt(K_int, 64, g_canned_i64),      [ "System"; "Int64"]);
 (Ciltype_int,                          [ "System"; "Int32"]);
 (Ciltype_decimal,                      [ "System"; "Decimal"]); 
 (CTL_vt(K_uint, 8, g_canned_u8),       [ "System"; "Byte"]);
 (CTL_vt(K_uint, 16, g_canned_u16),     [ "System"; "UInt16"]);
 (CTL_vt(K_uint, 32, g_canned_u32),     [ "System"; "UInt32"]);
 (CTL_vt(K_uint, 64, g_canned_u64),     [ "System"; "UInt64"]);
 (Ciltype_bool,                         [ "System"; "Boolean"]);
 (Ciltype_char,                         [ "System"; "Char"]);
 (CTL_vt(K_float, 32, g_canned_float),  [ "System"; "Single"]);
 (CTL_vt(K_float, 64, g_canned_double), [ "System"; "Double"]);
]


let type_alias_table =
 boxing_table
 @
 [
     
     //(CTL_object, [ "System"; "Object" ]); // String lists are not reversed in these tables, 
     (CTL_void,   [ "System"; "Void"   ]); // but are everywhere else.
     (Ciltype_string, [ "System"; "String" ]);          
     (Cil_suffix_i,   [ "System"; "Int32"  ]);
     (Cil_suffix_i1,  [ "System"; "Int32"  ]); 
     (Cil_suffix_i2,  [ "System"; "Int16"  ]);
     (Cil_suffix_i4,  [ "System"; "Int32"  ]);
     (Cil_suffix_i8,  [ "System"; "Int64"  ]);
     (Cil_suffix_u,   [ "System"; "UInt"   ]);
     (Cil_suffix_u1,  [ "System"; "UInt8"  ]);
     (Cil_suffix_u2,  [ "System"; "UInt16" ]);
     (Cil_suffix_u4,  [ "System"; "UInt32" ]);
     (Cil_suffix_u8,  [ "System"; "UInt64" ]);
     (Cil_suffix_r4,  [ "System"; "Single"]);
     (Cil_suffix_r8,  [ "System"; "Double"]);
 ]


// Should go in a table ?
// (Ciltype_ellipsis, true, "", 0);
// (Cil_suffix_ref, true, "", 0);

 
// Basic valuetype definitions.
let vt_table = 
 [
    ( (Signed, g_native_net_at_flags, 8),  ["Int8"; "System"]);
    ( (Signed, g_native_net_at_flags, 8),  ["SByte"; "System"]); 
    ( (Signed, g_native_net_at_flags, 16), ["Int16"; "System"]);
    ( (Signed, g_native_net_at_flags, 32), ["Int32"; "System"]);
    ( (Signed, g_native_net_at_flags, 64), ["Int64"; "System"]);
    ( (Signed, g_native_net_at_flags, 32), ["Int32"; "System"]);

    ( (Signed, { g_native_net_at_flags with nat_char=true }, 16), ["Char"; "System"]);

    // String is not needed here most probably? Since it is in g_ot_table.  Please explain distinction!
    //( (Signed, { g_native_net_at_flags with nat_string=true }, 16), ["String"; "System"]);

    ( (Unsigned, g_native_net_at_flags, 1),  ["Boolean"; "System"]);    
    ( (Unsigned, g_native_net_at_flags, 8),  ["Byte"; "System"]); 
    ( (Unsigned, g_native_net_at_flags, 8),  ["UInt8"; "System"]); 
    ( (Unsigned, g_native_net_at_flags, 16), ["UInt16"; "System"]);
    ( (Unsigned, g_native_net_at_flags, 32), ["UInt32"; "System"]);
    ( (Unsigned, g_native_net_at_flags, 64), ["UInt64"; "System"]);
    ( (Unsigned, g_native_net_at_flags, 32), ["UInt"; "System"]);

    ( (FloatingPoint , g_native_net_at_flags, 64), ["Double"; "System"]);
    ( (FloatingPoint , g_native_net_at_flags, 32), ["Single"; "System"]);
 ]

// The so-called other table of built-in types.
let g_ot_table = [ (["Void"; "System"],                                                       (CTL_void, 0));
                   (["String"; "System"],                                                     (f_canned_c16_str 0, 0));
                   (["Object"; "System"],                                                     (g_ctl_object_ref, 0));
                   (["CSharpBinderFlags"; "RuntimeBinder"; "CSharp"; "Microsoft"],            (g_canned_i32, 0));
                   (["CSharpArgumentInfo"; "RuntimeBinder"; "CSharp"; "Microsoft"],           (g_canned_i32, 0));
                   (["CSharpArgumentInfoFlags"; "RuntimeBinder"; "CSharp"; "Microsoft"],      (g_canned_i32, 0));                                      
                   (["RuntimeTypeHandle"; "System"],                                          (CTL_reflection_handle ["type"], 0)); 
                 ]





// Syntactic definitions from the front end, such as classes and interfaces, are stored after some inital processing in g_normbase.
// To rapidly find methods that need dynamic dispatch we also keep the mutable collections g_method_downcasts
let g_normbase = Dictionary<stringl, (Norm_t list * int)>()

let g_method_downcasts = new ListStore<string, stringl * stringl> ("method_downcasts")

type typebase_t = Map<stringl, (ciltype_t * int)>


let dt_is_enum arg =
    let rec dt_is_enum = function    
         //| CT_valuetype dt -> dt_is_enum dt
        | CTL_record(["Enum"; "System"], _, _, _, _, binds, _) -> true
        | CT_cr crst -> dt_is_enum crst.crtno
        | CT_class cst -> cst.name = ["Enum"; "System"] || (not(nonep cst.parent) && dt_is_enum (valOf cst.parent))    
        | CT_star _ 
        | CTL_record _
        | CTL_net _ -> false
        | other ->
            dev_println (sprintf "TODO backstop is_enum_class other %A" other)
            false
    let ans = dt_is_enum arg
    //vprintln 0 (sprintf "is_enum on %A returns %A" arg ans)
    ans
        
let no_is_enum arg =
    let ans =
        match arg with
                //| No_method(flags, ck, id, mdt, flags1, instructions, atts) ->
            | No_class(srcfiles, flags, idl, CT_class cst, items, prats)::_ ->  dt_is_enum (CT_class cst)
            | _ -> false
    //vprintln 0 (sprintf "isenum %A -> %A" arg ans)
    ans

// We can be making a full-field structure assign or a single field structure insert.
let struct_assign_dt_pred msg = function
    // OLD comments:
    // We rely on there not being CT_valuetype annotations around trivial valuetypes such as CTL_net.
    //| CT_valuetype(CTL_net _) -> false // enum type most likely, but all CTL_nets are valuetypes. 
    //| CT_valuetype cr ->               // Specifically denoted valuetype
    //    let enum = dt_is_enum cr
    //    not enum           // cf ct_is_valuetype - duplicated code?
    // The CTL_record/struct has a struct field in it, but we do not test it here. The convention is that these are valuetypes unless wrapped with a CT_star.
        // Note1: in CIL all valuetypes are technically structs but we treat them differently so we can do finer-grain dataflow analysis.
    // | CTL_record _ ->  true
    // | CTL_star -> false     
    | CTL_record(idl, cst, lst, len, ats, binds, _) -> cst.structf
    | dt ->
        //vprintln 3 (sprintf "struct_assign_dt_pred %s on %s" msg (cil_typeToStr dt))
        false
    //| _ -> false

// A predicate to check that all fields of a struct are to be parallel assigned.
// For a packed structure, no special action is normally needed.        
let full_struct_assign_pred msg arg = struct_assign_dt_pred msg (get_ce_type msg arg)

// Setters and Getters - need some name munging.  This matches what the C# compiler does.
// We map Set to set_Item and get to get_Item and so on
let setget_munge = function
    | "Set" :: tt -> "set_Item" :: tt
    | "Get" :: tt -> "get_Item" :: tt
    | other -> other

let vt_insert () = // Start-up code to create valuetype table.
    let m0 =
        let vti (m:typebase_t) ((signed, atts, wid), idl) =
            let volf = false // No value types are volatile by default but this flag may be put in later.   at_assoc g_volat atts<> None
            let e = CTL_net(volf, wid, signed, atts)
            m.Add(idl, (e, 0))
        List.fold vti Map.empty vt_table

    let m1 =
        let oti (m:typebase_t) (idl, xt) = m.Add(idl, xt)
        List.fold oti m0 g_ot_table // Why do we have this extra table? Historical legacy? 
    (m1:typebase_t)

let g_basetype_map = vt_insert () // Start-up code run.

let rec vt_scan idl =
    let (m:typebase_t) = g_basetype_map
    match m.TryFind idl with
        | Some (x, arity) -> Some x
        | None -> None


let constructor_name_pred = function
    | ".ctor"  -> 1
    | ".cctor" -> 1
    //| "COBJ?init" -> 3 
    | ss -> 
         (*  if String.length s > 5 && s.[size s - 5..]) = ".ctor"  *)
         //if vd>=5 then vprintln 5 ("Not a constructor: " + ss)
         0


let rec cgr_unambig = function
    | [item] -> true
    | []     -> false
    | (a::b::tt) -> snd a = snd b && cgr_unambig(b::tt)


let cgr_error__ msg cgr id =
    let listthem () = reportx 0 "[0] Class generics in scope at error." cgr1ToStr cgr
    if length cgr = 1 then
         (
             listthem();
             vprintln 0 ("+++ " + msg + ": inscope items=" + i2s(length cgr) + ": defaulting generic arg id='" + id + "'");
             Some(snd(hd cgr))
         )
         
    elif not g_kludgecgr then
         (
             listthem();
             sf (msg + ": inscope items=" + i2s(length cgr) + ": unbound generic arg id='" + id + "'")
         )
    else 
     (
      vprintln 0 ("+++" + msg + ": inscope items=" + i2s(length cgr) + ": kludge unbound generic arg id=" + id);
      Some(CTL_net(false, 13, Signed, g_null_net_at_flags))
     ) 

// Enumerations either have explict code points or not.  
// MSDN says the approved types for an enum are byte, sbyte, short, ushort, int, uint, long, or ulong, but Kiwi uses a suitable custom width of any number of bits.
// TODO perhaps support xnet_io modes on an enum in future.
let gec_enum_type ww idl lst = 
    let cCC = valOf !g_CCobject
    let ids = hptos idl
    let used_values =
        let trawl cc = function
            | (_, None, _) -> cc
            | (_, Some vv, _) -> singly_add vv cc
        List.fold trawl [] lst
    let rec succ v = // succ avoids user's defined numbers even if the user has them out of ascending order, which C# may not allow.
        if memberp v used_values then succ (v+1L) else v

    let m_report_lines = ref []
    let rec encode (p:int64) = function
        | [] -> []
        | (ss, Some manual_codepoint, _)::tt ->
            let _:int64 = manual_codepoint
            //vprintln 3 (sprintf "   %s enum member %i %s  manual_codepoint=%A" ids p ss manual_codepoint)

            mutadd m_report_lines [ss; i2s64 manual_codepoint; i2s64 p ]
            //if manual_codepoint < p then cleanexit(sprintf "For entry %s, cannot reallocate codepoint %i after %i  within enum %s" ss manual_codepoint p ids)
            (manual_codepoint, ss, ids) :: (encode (succ(manual_codepoint+1L)) tt)
        | (s, None, _)::tt ->
            let _ = vprintln 3 (sprintf "   %s enum member %i %s" ids p s)
            (p, s, ids) :: (encode (succ (p+1L)) tt)
    let encodings = encode (succ 0L) lst
    let maxvalue = List.fold (fun c (cp, _, _) -> max c cp) 0L encodings
    let so = Some ids
    let bitwidth = bound_log2 (BigInteger maxvalue)
    let _ = // Report enumeration coding
        let headings = ["Token"; "Code point";  "p" ]
        let t1 = create_table("Enumeration codepoints for " + ids, headings, rev !m_report_lines)
        aux_report_log 1 cCC.settings.stagename  t1
        app (vprintln 2) t1
        //mutadd settings.tableReports t1 

    let ans = CTL_net(false, bitwidth, Unsigned, {g_null_net_at_flags with nat_enum=Some(so, encodings) })
    vprintln 3 (sprintf "Created enum type %s: %s:  arity %i entries,  maxvalue=%i, field bitwidth=%i." (cil_typeToStr ans) ids (length lst) maxvalue bitwidth)
    ans


// modreq dig  - find storage class modifier in classref leaf component.
let rec mr_dig = function
    | Cil_namespace(_, a)               
    | Cil_cr_flag(_, a)
    | Cil_lib(_, a)       -> mr_dig a
    | Cil_cr_slash(_, a) 
    | Cil_cr_dot(_, a)    -> a
    | Cil_cr s            -> s
    | other -> sf ("modreq dig other 1/2" + cil_typeToStr other)  


//---------------------------------------------------------------


let noitem_name = function
    | No_class(srcfile, flags, idl, cst, items, prats) -> hptos idl
    | No_field(abound, flags, name, _, atclause, attributes) -> name
    | No_method(srcfile, flags, ck,  (idl, uid, disamname), mdt, flags1, instructions, atts) ->hptos idl// cil_typeToStr id
    | _ -> "anon"

let fapToStr x = cil_targToStr x


let follow_no_knot = function
    | No_knot(ct_, no_knot) when not_nonep !no_knot -> valOf !no_knot
    | other -> other
    
let rec noitemToStr_untie ff = function // cf follow_knot follow_no_knot and rationalise please
    | No_knot(CT_knot(idl, whom, v), _) ->
        match !v with
            | Some x -> typeinfoToStr x
            | None -> muddy "     No_knot(notmono)"
    
and noitemToStr fullflag no = // TODO check for another copy of this in asmout etc..
    match no with
    | No_knot(CT_knot(idl, whom, v), kno) ->
        match !v with
            | None   -> sprintf "No_knot(%s, <untied>)" (hptos idl)
            | Some x -> sprintf "No_knot(%s, <tied>)" (hptos idl)

    | No_esc cil -> classitemToStr "" fullflag cil    
    
    | No_class(srcfile, flags, idl, CT_class cst, items, prats) ->
        let id =  hptos idl + (if nullp cst.newformals then "" else "<<" + sfold (fun x->x(*fapToStr *)) cst.newformals + ">>")
        let ext = (if nonep cst.parent then "" else " extends " + cil_classrefToStr(valOf cst.parent))
        (
          (if cst.structf then "Normd_struct " else  "Normd_class ") + " " + cil_flagsToStr flags + " " + id + ext + "\n" +
          (if fullflag then "{\n" + foldr (fun(x,c)->"  " + noitemToStr false x + "\n" + c) "" (items) + "} // end of " + id + "\n\n"
           else "")
        )
        
    | No_class(srcfile, flags, idl, CTL_record _, items, prats) ->
        let id =  hptos idl
        let ext = "" //(if nonep dt.parent then "" else " extends " + cil_classrefToStr(valOf dt.parent))
        (
          "Normd_class " + " " + cil_flagsToStr flags + " " + id + ext + "\n" +
          (if fullflag then "{\n" + foldr (fun(x,c)->"  " + noitemToStr false x + "\n" + c) "" (items) + "} // end of " + id + "\n\n"
           else "")
        )
        
    | No_field(abound, flags, name, fdt, atclause, attributes) ->
        let id = cil_typeToStr fdt.ct
        let atos (id, args) = id + "(" + sfold (fun (fn, x) -> fn + xToStr x) args + ")"
        let ans =
              "Normd_field " + 
              (if abound=None then "" else "[" + iexpToStr(valOf abound) + "] ") +
              cil_flagsToStr flags + " " + id + " " + name +
              cil_field_atToStr atclause +
              (if fullflag then "\n" else "") 
        ans + " ats=(" + (sfold atos attributes) + ")"
        
    | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1, instructions, atts) -> 
        let ss = sprintf "Normd_method^%i(%s, %s, rt=%s, name=%s, formals=%s, %s)" (length mdt.arg_formals) (cil_flagsToStr flags) (cil_ckToStr ck) (typeinfoToStr mdt.rtype) (hptos idl + " uid=" + uid) (no_formalsToStr mdt.arg_formals) (cil_method_flagsToStr flags1) +
                        (if fullflag then "\n{\n" + List.foldBack (cil_classitemToStr "") instructions "}\n\n" else "")
        ss
    | No_proxy s -> "No proxy " + s

    | other -> sprintf "noitem other??%A" other
    

and get_src_ref_string = function
    | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1, instructions, atts) ->  srcfile
    | No_class(srcfiles, flags, idl, CTL_record _, items, prats) -> sfold id srcfiles
    | _ -> "<nosrcfile>"
    
and no_formalsToStr l =
    let rec f  = function
        | [] -> ""
        | (rpc_bindo, idxo, dt, s)::tt ->
            (if rpc_bindo=None then "" else sprintf "%s " (valOf rpc_bindo)) +
            (if idxo=None then "" else sprintf "[%i] " (valOf idxo)) +  
            typeinfoToStr dt + " " + s + (if tt<>[] then ", " else "") + f tt
    "(" + f l + ")"



//--------------------------------------------


(*
 * Print out a directory for debug/error purposes.
 *)
let report_dir_contents1 xvd msg fn dir = 
    vprintln xvd msg
    let cos = yout_open_out fn
    youtln cos msg 
    youtln cos ("Items in the CIL directory (before normalise)")   
    let summary v =
        let additional =
            match v with
                | CIL_method(srcfile, (cl_cr, cl_fap, meth_fap), flags, ck, rtype, (id, unique_id), arg_formals, flags1, instructions) ->
                    let staticf = memberp Cilflag_static flags
                    sprintf " arity=%i staticf=%A" (length arg_formals) staticf
                | _ -> ""
        let s = classitemToStr "" false v
        let s = if String.length s <= 130 then s else s.[0..129] + "..."
        s + additional
    let kk (idl, (is_exe, srcfilename, ast)) = youtln cos ("  Dir item " + hptos idl + "  " + summary ast)
    let k1 (idl, v) = vprintln 0 ("  Dir item " + hptos idl + "  " + summary v)    
    app kk dir
    //let _ = app k1 dir
    yout_close cos
    ()

let report_dir_contents (dir) =
    let fn = !g_log_dir + "/inscope.txt"
    let msg = "For your assistance, the " + i2s(length dir) + " items in scope at the error point have been written to '" + fn + "'"
    let vd = 0
    report_dir_contents1 vd msg fn dir


let report_nodir_contents1 xvd msg fn dir =
    vprintln xvd msg
    let cos = yout_open_out fn
    youtln cos msg
    youtln cos "Items in the CIL directory (after normalise)"   
    let summary v = 
        let s = noitemToStr false v
        if String.length s <= 45 then s else s.[0..44] + "..."
    let kk (idl, v) = youtln cos ("  Dir item " + hptos idl + "  " + summary v)
    app kk dir
    yout_close cos
    ()


let report_nodir_contents dir =
    let fn = path_combine(!g_log_dir, "inscope.txt")
    let msg = "For your assistance, the " + i2s (length dir) + " items in scope at the error point have been written to '" + fn + "'"
    let vd = 0
    report_nodir_contents1 vd msg fn dir



// Type compare - see if one is compatible with another, and type reduce - find highest-common supertype.
let tycomp vd msgf (ff, aa) =

    // Temporary interface matching code.
    let array_ifc (crst:cref_struct_t) = 
    // This handles the IEnumerable and other interfaces supported by an array on a temporary basis
        let check_array_interface cc = function // These Cil_cr_idl forms should be converted into CT_cr ot of the mainstream CT form before this point.
            | Cil_cr_idl idl when idl = crst.name -> Some 20 
            | _ -> cc
        let ans = List.fold check_array_interface None g_CT_arr_parents
        if nonep ans then None
        else
            //vprintln 3 (sprintf "tr0 array interface match: success: rv=%i" (valOf ans))
            ans


    let rec tc0_ntr disp (ff, aa) = // disp is star disparity left minus right
        // For type resolution we ignore the sign of star count - since a reference to is essentially a pointer to.  (For expressions we must, however, respect the difference between a reference and a dereference.)
        //if ff=aa then 20 would be the low-hanging fast path!  But using built-in equality tester blows the stack in F# (owing to maps stored in types?).
        //dev_println (sprintf "tc0_ntr  %s cf %s" (cil_typeToStr ff) (cil_typeToStr aa))
        match (ff, aa) with
        //| (ff, CT_valuetype aa) -> tc0 disp (ff, aa)
        //| (CT_valuetype ff, aa) -> tc0 disp (ff, aa)


        // Knots could be identified via their idl but we should check that these always include generic squirrels before making that optimisation.
        | (ff, CT_knot(idl, whom, v)) when not_nonep !v-> tc0 disp (ff, valOf !v)
        | (CT_knot(idl, whom, v), aa) when not_nonep !v-> tc0 disp (valOf !v, aa)


        | (CT_star(n, CT_arr(fft, _)), CT_star(m, CT_arr(act, _))) when disp=0 && abs n = abs m -> tc0 0 (fft, act) // Bypass the following decay clause for precise matches
        | (CT_star(n, ff), CT_star(m, CT_arr(act, _))) when abs n <> 0 && m>0 -> // Implement the classical type decay for arrays, where the address of an array unifies with the address of the first element
            if vd>=5 then vprintln 5 (sprintf "tc0 implementing type decay resolution")
            let sgn x = if x>0 then 1 elif x<0 then -1 else 0
            let n' = n - sgn n
            let m' = m-1 // m being +2 would be an address of an address that is nonsense normally, so m' is 0.
            tc0 (disp + n' - m')  (ff, act)
             
        // For other star forms we accumulate the disparity
        | (ff, CT_star(n, aa)) -> tc0 (disp - abs n) (ff, aa)
        | (CT_star(n, ff), aa) -> tc0 (disp + abs n) (ff, aa) 

        | (CT_arr(fct, _), CT_arr(act, _)) when disp=0 -> tc0 disp (fct, act)

        | (CT_arr(act, _), CT_cr crst) // Check for match to one of the interfaces of the built-in array.
        | (CT_cr crst, CT_arr(act, _)) when disp=0 && not_nonep(array_ifc crst) -> valOf(array_ifc crst)

            
        // For unsafe C# and C++ we need to identifty -ve stars (pointers) with an array dimension. Infact we take the absolute value of the input stars.
        | (CT_arr(fact, leno), aa) when disp < 0 -> tc0 (disp+1) (fact, aa)
        | (ff, CT_arr(aact, leno)) when disp > 0 -> tc0 (disp-1) (ff, aact)

       
        | (CT_cr crst, CT_cr crst') when disp=0 ->
            if crst.name = crst'.name then 16  // why was this only 10? It stopped telescope of gec_CE_conv on CT_cr
            else
                match crst'.crtno with
                    | CTL_record(idl, cst, lst, len, ats, binds, _) when not_nonep cst.parent -> tc0 disp (ff, valOf cst.parent)
                    | other ->
                        // let _ = vprintln 3 (sprintf "not expecting  %s" (cil_typeToStr other))
                        0

        | (CT_class cst, CT_class cst') when disp=0 -> if cst.name = cst'.name then 10 else 0
            //cleanexit(m + sprintf ": Used class %s when %s expected." (hptos cst.name) (hptos cst'.name))
        | (CT_class cst, CT_arr(act, _)) when disp=0 && cst.name = [ "Array"; "System" ] -> 10
        

        | (CTL_void, aa) ->
            match aa with
                | dt when dt=g_ctl_object_ref && abs disp = 1 -> 20  // Unify Object with void* for C++ reasons.  TODO also IntPtr please.  Perhaps make all the same in terms of internal representation.
                | CTL_void   -> 20
                | _ -> 1 
        
        | (dt, aa) when dt=g_ctl_object_ref ->    
            match aa with
                | CTL_void when abs disp = 1 -> 20 // Unify Object with void*
                | dt when dt= g_ctl_object_ref -> 20
                | _ -> 1 // really when other arg is a reference type only? // BOTH FORMS 2/3 !! messy

        | (_, dt) when dt=g_ctl_object_ref -> 1 // TODO this conclusion should be based on following up the inheritance tree


        | (CTVar _, _) -> 5 // Code is structured so we cannot lookup callee gbr at the moment ... so just loosely match for now ... (could at least unify?) need to get callee gbr from its this pointer.

        | (CTL_reflection_handle _, CTL_net _) // Reflection handles match IntPtrs ok.
        | (CTL_net _, CTL_reflection_handle _) when disp=0 -> 10        

        | (CT_cr crst, CTL_reflection_handle toks) when disp=0 && hd crst.name = "Type" && hd toks = "type" -> 20

        | (CT_cr crst, CTL_reflection_handle _) when disp=0 && hd crst.name =  "IntPtr" -> 10
        | (CTL_reflection_handle s1, CTL_reflection_handle s2) -> if hd s1 = hd s2 then 20 else 0

        | (CTL_record(f_idl, f_cst_, _, _, _, f_binds, _),  CTL_record(a_idl, a_cst_, _, _, _, a_binds, _)) when disp=0 ->
            let score =
                if f_idl<>a_idl then 0 // nominal match and then check binds
                else
                    let rec genm cc = function
                        | ([], [])                   -> cc
                        | ((_, f)::ftt, (_, a)::att) ->
                            let cc = cc + tc0 0 (f, a)
                            genm cc (ftt, att)
                        | _ -> sf "genm L603"
                    let q = genm 0 (f_binds, a_binds)
                    if q > 15 then 15 else q // We loosely check the degree of generic matching by clipping - TODO explain why.
            if vd>=5 then vprintln 5 (sprintf "tycomp records score=%i" score)
            score

        | (CTL_net(_, wid, signed, atts), CTL_net(_, wid', signed', atts')) when disp=0 -> (if wid=wid' then 20 else 5) + (if signed=signed' then 2 else 0) + (if atts=atts' then 1 else 0) // We prefer to match a char against a char rather than against a short.


        | (CT_class cst, CTL_record(idl, cst_, lst, len, ats, binds, _)) when disp=0 ->
            if idl=cst.name then 20 // prefer grounded record.
            elif cst.name=g_array_boll.name && hd (rev idl) = valOf(!g_kfec1_obj).d2name + "`1" then
                vprintln 3 (sprintf "Unified variant array nominals %s and %s (%A)" (hptos cst.name) (hptos idl) idl) // This is playing fast and loose over the blah/CT parameter. TODO.
                15
            else
                vprintln 0 (sprintf "+++ Try to unify disjoint nominal types %s and %s" (hptos cst.name) (hptos idl)) // Hmm input src code presumably passed typechecking in C# compiler!
                0

        | (CTL_record(idl, cst, lst, len, ats, binds, _), CT_cr crst)
        | (CT_cr crst, CTL_record(idl, cst, lst, len, ats, binds, _)) ->
            let _ = if disp<>0 then vprintln 2 (sprintf "Woefully disregard star disparity %i on %s" disp (hptos idl))
            // Could see if  is_structsite msg (CT_crst) == cst.structf
            let chok idl' = 
                // We don't bother to check we have the same actuals for the bindings... the overload on generic issue.  But the generic arity is at least squirrelled into the class name with a backtick.
                if idl = idl' then 20
                elif tl idl = idl' then
                    // partial match - TODO // Where we have squirreled generics we need a better matcher - for now kludge
                    vprintln 3 (sprintf "Ignored (for now) ct in unify disjoint types %s and %s" (hptos idl') (hptos idl)) // Hmm input src code presumably passed typechecking in C# compiler!
                    16
                elif not_nonep cst.parent then
                    let pscore = tc0 disp (ff, valOf cst.parent) // TODO this needs to scan up the full inheritance chain, not just one level.
                    if vd>=5 then vprintln 5 (sprintf "tc0 parent score %i" pscore)
                    pscore
                else
                    let _ = vprintln 3 (sprintf "Try again to unify disjoint types %s and %s" (hptos idl') (hptos idl)) // Hmm input src code presumably passed typechecking in C# compiler!
                    0

            match crst.crtno with
                | CT_knot(idl', whom, v) -> chok  idl'
                | CT_class cst -> chok cst.name
                | CTL_record(idl', _, _, _, _, _, _) -> chok idl'
                | other -> sf ("L510 other: " + typeinfoToStr other)
                        
        | (CTL_net _, CT_cr crst)
        | (CT_cr crst, CTL_net _) when disp=0 && dt_is_enum crst.crtno -> 10

        | (CTL_net _, CT_cr crst)
        | (CT_cr crst, CTL_net _) when crst.name = [ "ValueType"; "System" ] -> 10 // Why does this have a special entry but not, say, System.Exception?

        | (CT_class cst, CTL_net _) 
        | (CTL_net _, CT_class cst) when disp=0 && dt_is_enum (CT_class cst) -> 10
        
        | _ ->
            let _ = vprintln (min vd 3) (msgf () + sprintf ": disp=%i Cannot type unify f=%s with a=%s" disp (typeinfoToStr ff) (typeinfoToStr aa))
            0 // This type failure is commonly encountered when checking the wrong method overload.
            
    and tc0 disp (f, a) =
        if vd>=5 then vprintln 5 (sprintf "star disparity=%i    tc0 formal=%s" disp (typeinfoToStr f) + " against act=" + typeinfoToStr a)
        tc0_ntr disp (f, a)

    tc0 0 (ff, aa)

let generic_tyeq vd mf t1 t2 =
    let n = tycomp vd mf (t1, t2)
    n > 15
    
(*
 * Match a signature for a function call. To support slightly loose matching we use a scoring function rather than a hard match on disamname.
 * Also called a second time on the successful candidate to associate the method generics.
 *)
let sigmatch actual_signat desired_arity secondf (mM, s) =
    let vd = !g_unif_loglevel
    if vd>=4 then vprintln 4 (sprintf "starting a signature match second=%A" secondf)    
    let msgf () = 
        match mM with
            | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1, instructions, atts) -> "method " + hptos idl + ":uid=" + uid
            | other -> sprintf "not a method %A" other 

    let tr00 = function
        | (f, other) -> sf("tr00 other f=" + cil_typeToStr f + " a=" + typeinfoToStr other)

    let isobj idl =      // a simple hardwired version of the inheritance tree!! for now.
            idl = [ "Object"; "System" ]

    let opms = if secondf then "arg associate" else "type reduce"
    let trx = function
        | ((_, _, p, id), actual) ->
             let r = tycomp vd msgf (p, actual)
             lprint vd (fun()->opms + " formal=" + typeinfoToStr p + "/" + id + " against act=" + typeinfoToStr actual + " ans=" + i2s r + "\n")
             r

    let rec trf1 = function
         | ([], [], sofar) -> sofar      (* (Formals, Actuals, Weight) *)
         | (formal::t, (actual)::bt, sofar) -> 
              let k = trx(formal, actual)
              if k=0 then 0 else trf1(t, bt, sofar+k)
    let hpr_bias (No_method(srcfile, flags, ck, id, mdt, flags1, instructions, atts)) = if memberp Cilflag_hprls flags then 1 else 0
    let trf (mM, s) = 
         if vd>=5 then vprintln 5 (opms + sprintf " with formal sig=%s.  Length f=%i and length a=%i" (no_formalsToStr s) (length s) desired_arity)
         let weight = 
             if nullp s && actual_signat=[] then 10
             elif length s <> desired_arity then 0 else trf1(s, actual_signat, hpr_bias mM)
         vprintln vd ("Weight=" + i2s weight)
         (mM, weight)

    let ans = if secondf then sf "second time failure" else trf (mM, s)
    ans





// We make a codefix and a localfix: prefixes to make static some CIL dynamic storage.
//
// We need to create the same identifiers on the different passes of the code.
// We certainly need to make new identifiers for different calling depths of a recursive function and for different thread invokations of the same function.
// Now that we support virtual registers later mapped to physical registers, there's less need to re-cycle register names here at initial code generation time.
// But we still maintain the code that re-uses static vars that cannot be live at the same time owing to the stack having deallocated them.
// For large recursions we could move over to arrays for local vars of course. 
//
// codefix is expanded depending on how many times the current component has been expanded before.
//
// localfix is expanded on how many identical parents on further up the stack.
//
// When an object is created more than once and is reentrant ...  its locals need to be named off its root
// this...

type callsite_t = stringl * int


let callsiteToStr (cs:callsite_t) = hptos (fst cs) + "." + i2s(snd cs)


//
// We need fancy names for things that are called/instantiated from various places - especially if reentrant ... please explain more...
//
//
// When we concat a callsite with a hierarchic name we commonly get repeated identifiers in the resulting list.
// As all callsites are statically unique and have static template actuals we do not need to add anything beyond the callsite except the recursion level (for dynamic register names) - but it is cosmetically much nicer to add at least the last component of the called method's name.  To assist with this we can invoke
// funique on each last component so that these are statically unique.
// localfix is for simple register colouring
let methodbody_namemake cs uid idl0 (cCP:cilpass_t) =
    let vd = !g_gtrace_vd
    let callsite = callsiteToStr cs
    let idl1 = if uid="" then idl0 else uid :: tl idl0 // Use uid after overload disambiguation.
    let idl1 = idl0 // for now - needed to make more of test45 work
    //if uid <> "" then dev_println  (sprintf " tracking uid use " + hptos idl1)
    let stackme = 0 // FOR NOW: occurences of myself further up elab stack. TODO-4. Please recursion check test48 ...
    let previously = membercount(idl1, !(cCP.expanded_items_))
    let prefix = (if previously<1 then [] else [ cbg_ordinal(previously+1)]) @ idl1
    let codefix = (if previously<1 then [] else [ cbg_ordinal(previously+1)]) @ idl1 @  [ cCP.tid3.id ]
    // callsite in simple_localfix was wrong - making far too many registers (while phy=virt) - but we are removing that entirely now
    //let localfix = [ cbg_ordinal(stackme+1)] @ idl1 @ [ cCP.tid ]
    let simple_localfix = (if stackme < 1 then [ callsite ] else [ callsite; cbg_ordinal(stackme+1)]) @ idl1 @ [ cCP.tid3.id ]
    if vd>=4 then vprintln 4 (sprintf "namemake: uid=%s prefix=%s stackme=%i previously=%i idl1=%s" uid (hptos prefix) stackme previously (hptos idl1))
    (prefix, codefix, simple_localfix, previously)


// Another split routine ! Misnamed too. cf fwd_flatten : TODO conglomorate these ...
// OR rename this one type_idl_serf
let safe_type_idl arg =
    let aap a b =
        if nonep a || nonep b then None
        else Some(valOf a @ valOf b)
    let rec strip_generics = function
        | Cil_cr_flag(_, a)   -> strip_generics a
        | Cil_cr "" ->  sf "nullstring namespace"
        | Cil_lib(a, b)        
        | Cil_namespace(a, b) -> aap (strip_generics a) (strip_generics b) 
        | Cil_cr_dot(a, b) 
        | Cil_cr_slash(a, b)  -> aap (strip_generics a) (Some [b])
        | Cil_cr a -> Some [a]
        | Cil_cr_templater(cr, args) -> strip_generics cr
        | Cil_cr_idl idl             -> Some idl
        | CTL_record(record_idl, cst, lst, len, ats, binds, srco) -> Some(rev record_idl)

        | CT_cr cr -> Some(rev cr.name)
        | other  ->
            None

    strip_generics arg

let type_idl msg arg =
    match safe_type_idl arg with
        | Some ans -> rev ans
        | None -> sf(msg + sprintf ": type/idl strip generics other " + cil_classrefToStr arg)
            
// Returns an hierarchic identifier list. A reversed list of strings. Our normal form for 'idl'.
// We normally want the reverse of this except for printing where it is swapped by hptos.      
let rec fwd_flatten_cc arg cc =
    match arg with
    | Cil_cr_flag(_, b)   ->  fwd_flatten_cc b cc // TODO make this cr naming thing all a subtype - it was once!
    | Cil_namespace(a, b)
    | Cil_lib(a, b)       ->  fwd_flatten_cc a (fwd_flatten_cc b cc)
    | Cil_cr_dot(a, b)
    | Cil_cr_slash(a, b)  -> fwd_flatten_cc a (b :: cc)
    | Ciltype_star(a)     -> fwd_flatten_cc a ("$star$" :: cc)    
    | Cil_cr a            -> a :: cc
    | Cil_cr_idl idl      -> (rev idl) @ cc
    | Cil_cr_global       -> cc
    | other ->
        let _ = dev_println (sprintf "fwd_flatten other form %A" other)
        []
    
let rev_flatten ast_type = // Returns an hierarchic identifier list. A reversed list of strings. Our normal form for 'idl'.
    match op_assoc ast_type type_alias_table with
        | None     ->
            match ast_type with
                | Ciltype_star(ty) ->
                    match rev(fwd_flatten_cc ty []) with
                        | ["Byte"; "System"] 
                        | ["SByte"; "System"] ->  [ "String"; "System" ]
                        | idl -> sf (sprintf "other star %A" ast_type)
                | ty -> rev(fwd_flatten_cc ty [])
        | Some idl -> rev idl


let trivially_grounded_leaf = function
    | CTL_reflection_handle _
    | CTL_net _  // Q. Why is this different from CTL_vt ? A CTL_vt is for initial table load only.
    | CTL_record _ 
    | CTL_void        -> true
    | _ -> false

// Process simple types into normalised forms. Return 'None' on abstract types whether grounded or not.
let cil_type_n_vto = function
    | CTL_vt(_, _, ct) -> Some ct
    | ty ->
        let ty' =
            match op_assoc ty type_alias_table with
                | None -> ty
                | Some alias ->
                    let rec kf = function
                        | [] -> sf ("null class name in cil_type_n_vt"  + cil_typeToStr ty)
                        | [item] -> Cil_cr item
                        | h::t -> Cil_namespace(Cil_cr h, kf t)

                    kf alias
        match fwd_flatten_cc ty' [] with
            | []   -> None
            | rlst ->
                let idl0 = rev rlst
                vt_scan idl0

let cil_type_n_vt ty = 
    match cil_type_n_vto ty with
        | Some ans -> ans
        | None -> sf(sprintf "cil_type_n_vt other valuetype %s A=%A\nTable=%A" (cil_typeToStr ty) ty type_alias_table)
        

// Mark a net as volatile.
// We will probably add the volatile indication to the flags vector in future and use first field for other purposes or delete it.        
// The volatile nature would be better inferred from global (interthread) analysis in compose.fs rather than being 
let rec add_vol_flag_1 = function 
    | CTL_net(_, w, s, flags) -> CTL_net(true, w, s, flags)
    //CT_brand(b, t) -> CT_brand(b, add_v_flag t)
    | CTL_record(a, cst, lst, len, ats, binds, srco) -> CTL_record(a, cst,  map (fun crt -> {crt with dt=add_vol_flag_1 crt.dt; }) lst, len, ats, binds, srco)
    | other -> sf ("add_vol_flag_1 other:" + typeinfoToStr other)


let rec add_vol_flag_2 = function // Why do we have second copy of this ? Delete me.
    | CTL_net(_, w, s, a) -> CTL_net(true, w, s, a)
    | CTL_record(a, cst, lst, len, ats, binds, srco) -> CTL_record(a, cst,  map (fun crt -> {crt with dt=add_vol_flag_2 crt.dt }) lst, len, ats, binds, srco)
    | other ->
        vprintln 0 ("+++ ignored add_vol_flag_2 other:" + typeinfoToStr other)
        other




(* Certain net names contain troublesome characters or are Verilog keywords: munge them here *)
let san = function
    |  ('?') -> '_'
    |  ('$') -> '_'
    |  ('`') -> '_'
    |  ('.') -> '_'
    |  (':') -> '_'
    |  ('!') -> '_'
    |  ('{') -> '_'
    |  ('}') -> '_'
    |  ('-') -> '_'
    |  ('%') -> '_'
    |  (' ') -> '_'
    |  ('<') -> '_'
    |  ('>') -> '_'
    |  (',') -> '_'
    |  ('/') -> '_'
    |  ('(') -> '_'
    |  (')') -> '_'
    |  ('[') -> '_'
    |  (']') -> '_'
    |  other -> other


let mysanitize kill l =
    let rec no_double_underscores = function
        | [] -> []
        | [item] -> [ item ]
        | (('_')::('_')::t) -> no_double_underscores(('_')::t)
        | (h::t) ->
            let r = if memberp h kill then '_' else h
            r::no_double_underscores(t)
    implode(no_double_underscores(map san (explode l)))

let restrict_name md a = md + ":" + mysanitize [] (hptos a)

let defap_cr ww cr = // compare with strip_generics
    let rec dig_fap = function
        | Cil_cr_templater(cr, args) -> 
            let (a1, a2) = dig_fap cr
            (a1, a2 @ args)
        | cr -> (cr, [])
    dig_fap cr


let cr_flatten_or_error = function
    // the whacky array code could come in here too for regular use I think
    | CT_cr crst -> crst.name
    | CT_class cst -> cst.name
    | CTL_record(idl, _, _, _, _, binds, _) -> idl
    | CT_knot(idl, whom, v) -> idl
    | arg ->
        match rev_flatten arg with
            | [] -> sf (sprintf "null answer in fwd_flatten_or_error of %A" arg)
            | idl -> idl

let cr2idl_nr ww cr =
    //let _ = dev_println (sprintf "fp start on %s" (cil_typeToStr cr))
    let rec dig_fap = function
        | Cil_cr_templater(cr, args) ->  // compare with strip_generics
            let (a1, a2) = dig_fap cr
            (a1, a2 @ args)
        | cr -> (cr, [])
    let (cr, fap) = dig_fap cr

    let idl = cr_flatten_or_error cr
    //let _ = vprintln 0 ("cr2idl_nr gets " + hptos idl)
    (idl, fap)

let cr2idl ww ty = // good to get modreq stuff in here too
    let (a, fap) = defap_cr ww ty
    let rn =
        match a with
           | CTVar v -> RN_type(CTVar v)
           | cr -> 
              let (a, nomorefap__) = cr2idl_nr ww cr
              RN_idl a
    (rn, fap)


let rec printDtable vd m aA =
    let ff ss = youtln vd ss
    match destar aA with
       | CTL_record(idl, cst_, lst, len, ats, binds, srccode_o) ->
           let token = if cst_.structf then "CTL_struct" else "CTL_record"
           let rec rprt (crst:concrete_recfield_t) = ff(sprintf "    member no=%i   ptag=%s  utag=%s  type=%s  size_in_bytes=%i byte_offset=%i" crst.no crst.ptag crst.utag (cil_typeToStr crst.dt)  crst.size_in_bytes crst.pos)
           (ff(m + sprintf ": Dtable Report is %s %s : %i bytes" token (hptos idl) len);
            app rprt lst
            app (fun (f, a) -> ff(sprintf "    generic formal %s \\ %s" f (cil_typeToStr a))) binds
           )

        | other -> ff(m + ": Dtable is non-record: " + typeinfoToStr other)





let lift_gformal2 = function
    | Cil_tp_arg(_, _, id) ->  id
    | other -> sf ("lift_gformal2 other:" + fapToStr other)


(*
   Doing an alpha rename at definition time for all typevariable names helps debugging no end but does not help with two instances
   of the same class in scope with different bindings.

   C# allows nested scopes for typevariables and masking then applies.  But this is implemented inside the C# compiler and explicit
   typevars are passed down to the nested class constructors for their free type variables.
 *)
let whiten2 ww m (env:annotation_map_t) ss =
    let rec scan = function
        //| [] ->
            //vprintln 0 ("In scope type identifiers are " + sfold (fun (a,b) -> a) env)
            //cleanexit(m + sprintf ": Unbound type formal %A" ss)
        //| (a, b) ::t when a=ss -> b
        | _::tt -> scan tt
    //scan env
    match env.TryFind ss with
        | None ->             cleanexit(m + sprintf ": Unbound type formal %A" ss)
        | Some(RN_monoty(ty, _)) -> ty
        
// Deterministically generate a unique string for any concrete type - so strings can be matched instead of type expressions and so on.
// argpairs only for top call recurse  ...
let cil_typeToSquirrel ty argpairs =
    //let _ = vprintln 0 (sprintf "Squirrel of %A" ty)
    let rec doit arg =
        match arg with
        | CT_knot(idl, whom, v) when not_nonep !v -> doit (valOf !v)
        | CT_knot(idl, whom, v) -> idl // not squirrelled - but only in debug messages.
        | Cil_cr_idl idl -> idl
        | Cil_cr s -> [s] // missing clauses Cil_cr ...need to rev flatten all similar forms

        | CTVar vv when not (nonep vv.id)  -> [ valOf vv.id ]
        | CTVar vv when not (nonep vv.idx) -> [ sprintf "TEXAN%i" (valOf vv.idx) ]        

        //| CT_valuetype x -> doit x
        
        | CTL_net(volf, width, signed, flags) ->
            let basen =  [(*"SX"*)] 
            let vf = if true then "" else (if volf then "V" else "")
            // Best disable this V-based rendering aspect for now - seems mad.
            let rppt =
                if width=1 then [ vf + "BOOL" ]
                elif signed=FloatingPoint then [ (if width=32 then "FPS" else "FPD") ]             // SINT = [ "Int32"; "System" ] - brevity
                elif width=32 then  [ vf + (if signed=Signed then "SINT" else "UINT") ]
                else [ vf + (if signed=Signed then "SS" else "US"); i2s width ]
            rppt @ basen

        | CTL_reflection_handle _ -> [ "Opaque"; "IntPtr"; "KiwiC" ] 
        | CTL_void                -> [ "Void"; "KiwiC" ]  // TODO cf g_canned definition
        | CTL_record(idl, cst_, l, len, ats, binds, srccode_o) -> idl

        | CT_star(n, ct)   -> doit ct @ [ sprintf "$star%i$" n ] // n should never be zero.
        | CT_arr(ct, len_) -> doit ct @ [ "@" ]

        | CT_class crt ->
            let doit1 (f, a) = hptos(doit a) + "." + f
            (map doit1 argpairs) @ crt.name 
            
        | CT_cr cst ->
            let _ = if not (nullp argpairs) then sf "two lots of args"
            let doit2 = function
                | RN_monoty(a, ats) -> hptos(doit a)  + "//"
            (map doit2 cst.cl_actuals) @ (map doit2 cst.meth_actuals) @ cst.name
        | Ciltype_filler -> ["filler-squirrel"] // Only for debug
        | other -> sf (sprintf "other form in typeToSquirrel: type=" + cil_typeToStr other)
    doit ty


let print_listbindings b =
    let lb (idl, (n, ce)) = vprintln 0 (i2s n + " Binding of " + hptos idl)
    if b=[] then vprintln 0 ("No bindings were in scope") else app lb b



//
//
let ciltoarg bindings = function
    | Cil_argexp e -> cilToCe bindings e
    | Cil_suffix_0 -> gec_yb(xmy_num 0, g_canned_i32)
    | Cil_suffix_1 -> gec_yb(xmy_num 1, g_canned_i32)
    | Cil_suffix_2 -> gec_yb(xmy_num 2, g_canned_i32)
    | Cil_suffix_3 -> gec_yb(xmy_num 3, g_canned_i32)
    | Cil_suffix_4 -> gec_yb(xmy_num 4, g_canned_i32)
    | Cil_suffix_5 -> gec_yb(xmy_num 5, g_canned_i32)
    | Cil_suffix_6 -> gec_yb(xmy_num 6, g_canned_i32)
    | Cil_suffix_7 -> gec_yb(xmy_num 7, g_canned_i32)
    | Cil_suffix_8 -> gec_yb(xmy_num 8, g_canned_i32)
    | Cil_suffix_m1 -> gec_yb(xmy_num -1, g_canned_i32)
    | Cil_suffix_M1 -> gec_yb(xmy_num -1, g_canned_i32)
    //| (_) -> sf ("ciltoarg other")

let ciltoarg_symb bindings = function
    | Cil_argexp(Cil_number32 n) -> n
    | Cil_argexp(Cil_int_i(w_, n)) ->
        try int32 n
        with _ -> sf(sprintf "Overflow in %A" (Cil_int_i(w_, n)))
    | Cil_argexp(Cil_id v) ->
        let ov = op_assoc [v] bindings
        let _ = if ov=None then (print_listbindings bindings; sf("no binding for " + v))
        let jj = function
            | (n, ce) -> n
        jj (valOf ov)

    | Cil_suffix_0 -> (* b *)0
    | Cil_suffix_1 -> (* b *)1
    | Cil_suffix_2 -> (* b *)2
    | Cil_suffix_3 -> (* b *)3
    | Cil_suffix_4 -> (* b *)4
    | Cil_suffix_5 -> (* b *)5
    | Cil_suffix_6 -> (* b *)6
    | Cil_suffix_7 -> (* b *)7
    | Cil_suffix_8 -> (* b *)8

    | other ->
       let k = (ciltoarg bindings other)
       let rec fof = function
           | CE_x(_, x)   -> x
           | other -> sf("other in citoarg_symb: " + ceToStr other)
       //in fof k
       muddy (sprintf "ciltoarg_symb: other  %s ast=%A" (iargToStr other) other)

// Spot a call to the inHardware macro
let inHardware_check arg =
    match arg with
        | CIL_instruct(_, _, _, Cili_call(cm, ck, rt, (tyidl, "inHardware", _), _, _)) ->
            let idl = rev(fwd_flatten_cc tyidl [])
            let ans = idl = [ "Kiwi"; "KiwiSystem" ]
            //vprintln 3 (sprintf " inHardware check %A for %A idl=%s" ans tyidl (hptos idl))
            ans
        | _ -> false



 (*
  * Although atts are put before the item to be declared in C#, they appear afterwards
  * in the monodis form, so this collected trailing customs after next class item.
  * We'll do this in cecil now...
  *
  * The customs for a method are popped in the top.
  *)
let rec foldl_withatts f e = function
    | [] -> e
    | lst ->
        let p = function
            | CIL_custom _ -> true
            | _ -> false
        let rec sep = function
             | ([], c) -> ([], rev c)
             | (h::t, c) -> if (p h) then sep (t, h::c) else (h::t, rev c)
        let (t', peeks) = (tl lst, []) // cecilfe does it differently, so disabled here : sep(tl lst, [])
        let e' = f(hd lst, peeks, e)
        foldl_withatts f  e' t'


let g_default_accelerator_timing_target =
            {
              latency_target=  4  // Get from recipe please 
              ii_target=       1
              hardness=        0.5
            }


type cd_t = { prefix: stringl; localfix: stringl; codefix: stringl }

let g_cd0 = { prefix=[]; localfix=[]; codefix=[] } : cd_t
(*
 * Compute an idl for each directoryable item in the input cil and pair them as an association list.
 * For inherited method definitions, we add them in a post-processing step.
 * Also scan of items marked up with the H/W attribute.
 * Notice also any static fields so they can be later, easily auto-included on demand.
 *)
let rec install_in_cil_directory ww (cD:cd_t) srcfilename is_exe directory (directorate_style, timing_target, as_alias) (arg, peeks, (marked, rootdata)) =
    let vd = 4
    let _:raw_cil_dir_t = directory
    let _ = WF 3 "install_in_cil_directory" ww ("top add item " + "codefix=" + htos cD.codefix + " " + classitemToStr "" false arg)


    let as_aliasToStr (fromlib_rev, to_lib) =
        if nullp fromlib_rev then ""
        else sprintf "(as alias:%s->%s)" (hptos(rev fromlib_rev)) (hptos to_lib)

    let apply_alias as_alias arg =
        if nullp(fst as_alias) then arg
        else
            let rec renameq = function
                | (rak,    [])     ->
                    let ans = (rev rak) @ (snd as_alias)
                    vprintln 2 (sprintf "Entered in directory renamed %s as %s" (hptos arg) (hptos ans))
                    ans
                | (r::rtt, q::qtt) ->
                    //dev_println (sprintf "renameq %s cf %s" r q)
                    if r=q then renameq (rtt, qtt)
                    else
                        if vd>=5 then vprintln 5 ("Did not rename " + hptos arg)
                        arg // Did not rename. 
            renameq (rev arg, fst as_alias)

    let save2 tag ast =
        let savee = (is_exe, srcfilename, ast)
        if vd>=4 then vprintln 4 (sprintf "Install in CIL directory %s" (hptos tag))
        mutadd directory (tag, savee)                          // Save under real name
        let aname = apply_alias as_alias tag
        if aname <> tag then
            if vd>=4 then vprintln 4 (sprintf "Install also in CIL directory %s" (hptos aname))
            mutadd directory (aname, savee)  // Save also under its alias if it has one.

    let scan_atts token cc = function
        | CIL_custom(_, t, fr, args, vale) ->
            //let idl = id :: rev(fwd_flatten_cc fr_raw [])
            let rec dig = function // use fwd_flatten instead
                | Cil_lib(_, a) 
                | Cil_cr_flag(_, a)
                | Cil_namespace(_, a) -> dig a
                
                | Cil_cr_dot(_, a)
                | Cil_cr_slash(_, a) -> a
                | Cil_cr s -> s
                | other -> sf ("scan dig other 3/2: " + cil_typeToStr other)  (* REPEATED CODE *)
            let dig2 (a, id_, gargs_) = dig a
            let what = dig2 fr
            let found = isPrefix token what
            //lprintln 20 (fun ()-> "scan_atts: what=" + what + " " + boolToStr found)
            if found then 
                vprintln 3 ("Found attribute marked-up target for h/w compilation " + classitemToStr "" false arg)
                arg :: cc
            else cc

        | _ -> cc

    match arg with
    | CIL_assembly(flags, cr, flags1) -> (marked, rootdata) // ignore

    | CIL_mresource(flags, cr, items) -> (marked, rootdata) // ignore

    | CIL_module(id, flags, items) ->
        //let idl = rn_valOf "install_in_cil_directory namespace" rn  // BETTER WAY
        let ww = WF 3 "install_in_cil_directory" ww (" module " +  id + as_aliasToStr as_alias)
        save2 (id :: cD.codefix) arg
        (marked, rootdata)
        
    | CIL_class(srcfile, flags, cr, gformals_, extends_, items) -> 
        let (rn, _) = cr2idl ww cr
        let class_idl = rn_valOf "install_in_cil_directory class" rn
        let prefix' = class_idl
        let cD' = { prefix=prefix'; codefix=prefix';  localfix=class_idl @ cD.localfix }
        let ww = WF 3 "install_in_cil_directory" ww ("class " + htos cD'.codefix + as_aliasToStr as_alias)
        //dev_println (sprintf "CIL_class localfix=%s for class called '%s'" (htos cD'.localfix) (htos class_idl))
        let (marked, flagged) = foldl_withatts (install_in_cil_directory ww cD' srcfilename is_exe  directory (directorate_style, timing_target, as_alias)) (marked, []) items
        save2 prefix' arg
        let ids = hptos class_idl
        let already_marked = op_assoc class_idl marked
        let flagged1 = not_nullp flagged && nonep already_marked
        //reportx 0 "marked" (fun (a,b) -> hptos a) (marked)
        // Please explain: if contents have been flagged then put the contents on the marked list, augmented with the defining class name.
//      if flagged1 then vprintln 1 ("Class '" + ids + "' has root/ep method(s) flagged for h/w compilation: " + sfold (fun (a,b) -> sprintf "%A %s" a (htos b)) flagged)
        ((if flagged1 then map (fun x->(class_idl, (x, arg))) flagged else []) @ marked, rootdata)

    | CIL_namespace(flags, cr, items) -> 
        let (rn, _) = cr2idl ww cr
        let idl = rn_valOf "install_in_cil_directory namespace" rn
        let ww = WF 3 "install_in_cil_directory" ww ("namespace " + hptos idl)
        let cD' = { prefix= idl@ cD.prefix; codefix=idl @ cD.codefix;  localfix=idl @ cD.localfix }
        let (marked', _) = foldl_withatts (install_in_cil_directory ww cD' srcfilename is_exe directory (directorate_style, timing_target, as_alias)) ([], []) items
        (marked' @  marked, rootdata)
  
    | CIL_setget(opcode, ck, flags, tt, fr, signat) ->  // No longer used with cecil front end.
#if UNDEF
          let textcl__ =
            { //cl_codefix= cD.codefix;
              cgr=          []
              cl_localfix=  cD.localfix
              cl_prefix=    cD.prefix
              this_dt=      None
            }
#endif
        (marked, rootdata)
        
    | CIL_property(tt, ck, cr, signat, items) -> 
        let (rn, _) = cr2idl ww cr
        let idl = rn_valOf "install_in_cil_directory property" rn
        let cD' = { prefix=idl @ cD.prefix; codefix=idl @ cD.codefix;  localfix=idl @ cD.localfix }
        let (marked', _) = foldl_withatts (install_in_cil_directory ww cD' srcfilename is_exe directory (directorate_style, timing_target, as_alias)) ([], []) items
        save2 (idl @ cD.codefix) arg
        (marked' @ marked, rootdata)
        
    | CIL_method(srcfile, cr_, flags, kind, rtype, (gid, unique_id), signat, flags1_, instructions) -> 
        let found_accelerator = List.fold (scan_atts "PipelinedAccelerator") [] instructions
        let found_root = List.fold (scan_atts "HardwareEntryPoint") [] instructions
        let found_remote = List.fold (scan_atts "Remote") [] instructions       
        let id = (type_idl "L957" gid) @ cD.codefix
        let mm = sprintf " Method %s, method uid=%s added to directory %s" (hptos id) unique_id (as_aliasToStr as_alias)
        let ww = WF 3 "install_in_cil_directory" ww mm

        let arity = length signat
        match op_assoc id !directory with
            | Some x -> vprintln 2 (sprintf "Method %s (arity=%i) (uid=%s) defined more than once - perhaps overloaded." (hptos id) arity unique_id)
            | None   -> ()
        save2 id arg                
        let timing_target =
            if nonep timing_target && not_nullp found_accelerator then Some g_default_accelerator_timing_target else timing_target
        let dirtim = (directorate_style, timing_target) // TODO prefer to parse attribute overides here please.
        let rootdata =
            if not_nullp found_root then (MM_root, id, unique_id, dirtim) :: rootdata
            elif not_nullp found_accelerator then (MM_pipeline_accelerator_hls, id, unique_id, dirtim) :: rootdata
            elif not_nullp found_remote then (MM_remote, id, unique_id, dirtim) :: rootdata
            else rootdata
        (marked, rootdata)
        
    | CIL_field(layout, flags, ctype, cr, atclause, custs) ->
        let (rn, _) = cr2idl ww cr
        let idl = rn_valOf "install_in_cil_directory method" rn
       //if memberp Cilflag_static flags then vprintln 0 ("CIL STATIC FIELD entered in directory idl=" + hptos idl +  " codefix=" + hptos cD.codefix)

       //vprintln 0 ("HW_scan " + sfold (fun (b)-> classitemToStr true b) custs) 
        let found = List.fold (scan_atts "HardwareAttribute") [] custs
        if found<>[] then cleanexit ("From this release onwards: Please use HarwareEntryPoint applied to a method instead of Hardware applied to a field")
        let arg1 = CIL_field(layout, flags, ctype, cr, atclause, custs @ peeks)
        let idxname =idl @ cD.codefix
        vprintln 3 (sprintf "insert_field: Created idxname %s for %s %s" (hptos idxname) (hptos idxname) (as_aliasToStr as_alias))
        save2 idxname arg1
        (marked, rootdata)  


    | CIL_data (l, r) ->
       save2 (iexpToStr l:: cD.codefix) arg
       (marked, rootdata)

    // The following are not installed in the directory
    | CIL_entrypoint _ 
    | CIL_custom _ 
    | CIL_imagebase _
    | CIL_corflags _
    | CIL_file _ 
    | CIL_subsystem _
    | CIL_size _
    | CIL_pack _
    | CIL_comment _ 
    | CIL_stackreserve _ ->
        (marked, rootdata)
        
    | other ->
        vprintln 2 (cil_classitemToStr "" other ": install_in_cil_directory other form ignored")
        (marked, rootdata)  



let further_static_work ww ((*flag_, srcfile_name, *)ast) cc =

    let srcfile_name = "Z4-missing"
    let scan arg cc =
        match arg with
            | CIL_class _
            | CIL_data _
            | CIL_entrypoint _ 
            | CIL_custom _ 
            | CIL_imagebase _
            | CIL_corflags _
            | CIL_file _ 
            | CIL_subsystem _
            | CIL_size _
            | CIL_pack _
            | CIL_comment _ 
            | CIL_stackreserve _ ->
                cc

            | CIL_method(srcfile, cr_, flags, kind, rtype, (gid, unique_id), signat, flags1_, instructions) -> 
                let idl = (type_idl "L1413" gid) 
                if hd idl = ".cctor" then
                    vprintln 3 ("further_static_work: CIL static class constructor ready for directory idl=" + hptos idl + " src=" + srcfile_name)
#if SPARE
    let cctor_idl = ".cctor" :: idl // Auto-compile the constructor
    let specificf = S_kickoff_collate
    let tid = get_next_tid true (g_main_tid_prefix + "_CTOR")
    let item = Root_cc(tid, MM_aux_cctor_style, specificf, ROOT_THREAD1(idl, Some cctor_idl, uid), (kfec2.directorate_style, kfec2.directorate_attributes), None)

#endif
                    arg::cc
                else cc
            
            | CIL_field(layout, flags, ctype, cr, atclause, custs) ->
                let (rn, _) = cr2idl ww cr
                let idl = rn_valOf "install_in_cil_directory method" rn
                if memberp Cilflag_static flags then
                    vprintln 3 ("further_static_work: CIL static field ready for directory idl=" + hptos idl + " src=" + srcfile_name)
                    arg::cc
                else cc

            |  other ->
                vprintln 2 (sprintf "further_static_work: other form ignored in %s: other=%A " (srcfile_name) other)
                cc
    List.foldBack scan ast cc


// TODO: Recipe settings need to be provided to alter this default, instead of relying only on Kiwi.ClockDom. ... 
let g_default_clknets = ([E_pos g_clknet], [(true, false, g_resetnet)], [])


// Kiwi.ClockDom() attribute parsing
// The kiwic.tex documentation says we can set the reset net this way but it seems not infact! And ClockDomNeg is now replaced with compositeAttributeString TODO
let parseKiwiClockDom ww vd msg args =
    let cCC = valOf !g_CCobject
    let clock_attribute = Nap(g_ip_xact_is_Clock_tag, "true")
    let reset_attribute = Nap(g_ip_xact_is_Reset_tag, "true")        
    
    // reportx 3 "Custom Attribute Marshalled Args" (fun (fn, x)-> fn + ":" + xToStr x) args
    let get_s mm  = function
        | (_, W_string(id, _, _)) -> id
        | (_, other) -> sf(sprintf "Parse Kiwi.ClockDom: %s - expect string not %s" mm (xToStr other))

    let default_ans = g_default_clknets


    let (clk, resets, clk_enables) = 
        match args with
            | None -> default_ans
            | Some [clockName; resetName; composite; clockEnableName ] ->
                let prams = string_to_assoc_list (sfold_colons ([get_s "compositeAttributes" composite])) // Parse the string, getting key/value pairs from a colon or semicolon-separated list.
                let (combf, clk_invf) =
                    match op_assoc "clockPolarity" prams with
                        | None        -> (false, false)
                        | Some "comb" -> (true, true)
                        | Some "pos"  -> (false, false)
                        | Some "neg"  ->  (true, true)
                        | Some other -> cleanexit (msg + ": Kiwi.ClockDom expects clockPolarity to be pos, comb or neg, not " + other)
                let clock_id = get_s "clockName" clockName
                let clocknet = ionet_w(clock_id, 1, INPUT, Unsigned, [clock_attribute])
                let clks =
                    if combf then []
                    else [(if clk_invf then E_neg clocknet else E_pos clocknet)]

                let reset_id = get_s "resetName" resetName
                let resets =
                    if reset_id = "" then f2o3 default_ans
                    else
                        let reset = ionet_w(reset_id, 1, INPUT, Unsigned, [reset_attribute])
                        let is_pos = true
                        let is_asynch = cCC.settings.reset_mode = "asynchronous" // should check for 'non' too?
                        [(is_pos, is_asynch, reset)]

                let clockEnable_id = get_s "clockEnableName" clockEnableName
                let clk_enables =
                            if clockEnable_id = "" then f3o3 default_ans
                            else
                                let clk_enables = ionet_w(clockEnable_id, 1, INPUT, Unsigned, [])
                                [clk_enables]

                (clks, resets, clk_enables)                                
            | _ ->

                cleanexit(msg + ": Kiwi.ClockDom should have four arguments including default args.")
    (clk, resets, clk_enables)



// Kiwi.HprPrimitiveFunction() attribute parsing
let parse_kiwi_primitive_function_attributes ww msg arg =
    let styles_etc =
        let destring arg cc =
            match arg with
                // | int -> i2s int
                | (_, W_string(ss, _, _)) -> ss::cc
                | _ -> cc
        List.foldBack destring arg []
    let ww = WF 3 "parse_kiwi_primitive_function_protocol_attributes" ww ("start " + sfold id styles_etc)
    let prams = string_to_assoc_list (sfold_colons styles_etc) // Parse the string, getting key/value pairs from a colon or semicolon-separated list.
    //dev_println (sprintf "BDAY: primitive_function_attributes=%A" prams)
    let fsems =  // The user-attributed properties override those that are inferred. An example is INHOLD=false
        let override_fsem_attribute fsems (key, vale) =
            let key = toupper key
            if memberp key protocols.g_foldable_fsem_attribute_names then
                vprintln 3 (sprintf "Overriding inferred protocol attribute '%s' with '%s'" key vale)
                protocols.fsems_folder msg key vale fsems
            else fsems
        List.fold override_fsem_attribute g_default_fsems prams

    let sri_arg = (op_assoc "SRI" prams) 
    let fu_arg = (op_assoc "FU" prams)
    //dev_println (msg + sprintf ": BDAY: primitive_function_fsems=%A" fsems)    
    (sri_arg, fu_arg, fsems)


let rax_hfast_mainschema ww vd msg protocol_str =
    let sl = strlen protocol_str // This should be a generic protocol parser in protocols.fs and not part of Kiwife.
    let sog max_outstanding (protocol, clk_domain_markup_here_unused_) = ({ protocol with max_outstanding=max_outstanding }, protocol_str)
    if vd>=4 then vprintln 4 (sprintf "rax_hfast_mainschema protocol_str=%s" protocol_str)
    let dsimple = true // Set dsimple to default to assumed to be ready.
    match protocol_str with
        | _ when sl>=6 && protocol_str.[0..5] = "HFAST1" ->
            let protocol_prams = rax_pcol_subschema_core msg dsimple protocol_str.[6..]
            sog 1 protocol_prams
                
        | _ when sl>=6 && protocol_str.[0..5] = "HFAST2" ->
            let protocol_prams = rax_pcol_subschema_core msg dsimple protocol_str.[6..]
            sog 2 protocol_prams

        | _ when sl>=6 && protocol_str.[0..5] = "HFAST3" ->
            let protocol_prams = rax_pcol_subschema_core msg dsimple protocol_str.[6..]
            sog 3 protocol_prams

        | _ when sl>=5 && protocol_str.[0..4] = "HFAST" ->
            let protocol_prams = rax_pcol_subschema_core msg dsimple protocol_str.[5..]
            sog 1 protocol_prams

        | other -> sf (msg + sprintf ": Server entry point with protocol '%s' not supported" protocol_str)


// Kiwi.Remote() attribute parsing
let parse_kiwi_remote_protocol_attributes ww vd msg arg =
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

    //dev_println (sprintf "firstpass: fsems=%A" fsems)
        // inputs held is via fsems INHOLD=false
    let externals = valOf_or (op_assoc "externally-instantiated" prams) "false"
    let searchbymethod = (valOf_or (op_assoc "searchbymethod" prams) "false") = "true" 
    let overloaded = (valOf_or (op_assoc "overloaded" prams) "false") = "true"   
    let stubonly_ = (valOf_or (op_assoc "stubonly" prams) "false") = "true"
    let externally_instantiated  = externals = "true"

    //dev_println (sprintf "Kiwi.Remote() assocs are %A" prams)
    let protocol =
        match op_assoc "protocol" prams with
            | Some protocol_str ->
                rax_hfast_mainschema ww vd msg protocol_str
            | None -> sf (msg + ": no protocol attribute for RPC entry point.\nPerhaps use [Kiwi.Remote(\"protocol=HFAST1;reftran=true;mirrorable=true;inhold=false;overloaded=true\")]")
    let fsems =
       { fsems with
              fs_overload_disam=  overloaded    
       }
    if fsems.fs_asynch then muddy "Kiwi.Remote: async call" // ... for now ... please integrate with ...
    let dash (key, vale) = (key, xi_string vale)
    //dev_println (sprintf "Git return %A" protocol)
    (map dash prams, protocol, searchbymethod, externally_instantiated, fsems)

    
 

(*
 * Serialised binary initialiser
 * Custom attributes have initialisers expressed as a byte list.  This function
 * parses such a blob according to a list of types.
 *)
let rec custat_argmarshall ww vd msg (formal_args, args1) =
    //dev_println (sprintf "custat_argmarshall vd=%i" vd)
    let m_reported = ref false
    let report() =
        if not !m_reported then
            vprintln 0 ("Args wanted were " + sfold cil_callargToStr formal_args);
            m_reported := true

    let d = ref [] // The burner
    let burner_peek () = hd !d
    let burner_nxt() =
        if nullp !d then (vprintln 0 (msg + "+++ CIL parse problem: no more blob"); report(); Cil_number32 0)
        else
            let rr = hd !d 
            d := tl !d
            rr

    let peek_id () =
        match burner_peek() with
            | Cil_id _ -> true
            | _ -> false
    let marsh_g burner_nxt (formal_name, ty) =
        let formal_name = valOf_or formal_name "_anon_"
        let sval idl = function
            | Cil_id ss -> ss
            | other ->sf (sprintf "Bad form constructor argument expression for formal_name=%s %s when wanting a string. arg=%s" formal_name (hptos idl) (iexpToStr other))
        let ival = function
            | Cil_number32 nn ->
                vprintln 3 (sprintf "Blob read byte L993 %i" nn)
                nn
            | other -> sf (sprintf "Bad form constructor argument expression for formal_name=%s when wanting an int. arg=" formal_name + iexpToStr other)
        let g16 () = // little-endian read two bytes from blob
            let lo = ival(burner_nxt()) in lo + 256 * ival (burner_nxt())
        let g32 () = // little-endian read four bytes
            let lo = g16() in lo + 65536 * g16()
        let rec rds n = if n = 0 then [] else ord_chr(ival(burner_nxt())) :: rds(n-1) 
        let rr =
            let idl = rev_flatten ty
            if nullp idl then sf "marsh_g nillp L512"
            else
                match hd idl with
                    | "String" ->
                        let ss = if peek_id() then sval idl (burner_nxt()) else implode(rds(ival(burner_nxt())))
                        xi_string ss
                    | "UInt32" -> xi_num(ival(burner_nxt()))
                    | "Int32"  -> xi_num(ival(burner_nxt()))                    
                    | "AttributeTargets" ->
                        X_undef 
                    | ss ->
                        vprintln 0 (msg + sprintf "+++ Other type '%s' ignored in field init from marsh explist:" (hptos idl) + cil_typeToStr ty)
                        X_undef
        (formal_name, rr)


    match args1 with
    | Cil_explist tx ->
        d := tx
        let ans = map (marsh_g burner_nxt) formal_args 
        if vd>=4 then vprintln 4 (sfold (fun (fn, x)-> "Attribute argument marshal:" + msg + ": " + fn + ": " + xToStr x) ans)
        ans

    | Cil_blob tr ->
        //vprintln 0 (sprintf "Spaggle: print binary blob %A" tr)
        d := tr
        if not_nullp formal_args then (ignore(burner_nxt()); ignore(burner_nxt()))// dont want the no more blob message if none needed.  Discard leader.
        let ans = map (marsh_g burner_nxt) formal_args 
        if vd>=4 then vprintln 4 (sfold (fun (fn, x) -> "Arg marshal:" + msg + ":" + fn + ": " + xToStr x) ans)
        ans

    | _ ->
        report()
        sf (msg + ": Bad forms in arg marshal3")


(*
 * Determine whether this method is marked up with an attribute prefixed by any of the supplied tokens (e.g. used for server, remote...)
 * The attributes are tucked inside the method at the start of the instruction list.
 *)
and checkfor_tokmarked_method ww vd toklst instructions =
    //vprintln -1 (sfold (fun x->x) toklst + " method scan under " + hptos idl)
    let rec scan = function
             | CIL_custom(s_, t, (fr_raw, id, gargs), args, vale)::tt -> 
                 let idl = id :: rev(fwd_flatten_cc fr_raw [])
                 let idl = if hd idl = ".ctor" then tl idl else idl
                 let s = if idl=[] then "" else hd idl
                 //vprintln 0 ("tokmarked_method: Scan of " + s)
                 let ya = disjunctionate (fun v -> isPrefix v s) toklst
                 if ya then
                     Some(custat_argmarshall ww vd s (args, vale))
                 else scan tt // This ignores any potential further matches after first matching one.

             | (_) -> None
    let rr = scan instructions
    rr





and scan_cust_args ww vdp idl0 cc arg = 
    let vd = if vdp then 4 else 3
    match arg with
    | CIL_custom(_, t, (fr_raw, id, gargs), args, vale) -> 
        let idl = id :: rev(fwd_flatten_cc fr_raw [])
        let idl = if hd idl = ".ctor" then tl idl else idl
        //vprintln 3 (sprintf "Examining custom attribute of %s which are %s in %s"  (hptos idl0) (hptos idl) (classitemToStr "" true arg))
        let ss = if idl=[] then "NoNameField" else hd idl // Discard defining attribute class - so thereby assume a flat namespace of all attributes.
        let args1 = custat_argmarshall ww vd ss (args, vale)    // Serialised binary initialiser
        (ss, args1)::cc

    | other ->
        if vdp then hpr_warn ("Ignoring custom field attribute of " + hptos idl0 + " which is " + classitemToStr "" true other)
        cc

   
and lift_fap_values ww normenv msg arg = 
    let lift = function
        | Cil_tp_arg(methodf, n, ss) -> CTVar{ g_def_CTVar with idx=Some n; id=Some ss; methodf=methodf }
        | Cil_tp_type ctype -> cil_type_n (WN "template_eeval" ww) normenv ctype
    let dt = lift arg
    gec_RN_monoty ww dt

//
// Make the initial conversion from AST CIL_xxx forms to NO_xxx forms.
// Create the ungrounded (parameterised) types for each class and field.
// normenv is provided for a little assistance and scope checking, but type substitution will be done later as pushdowns on the gamma fields.    
and apply_norm000 ww bindings idl pco items =
    // We prefer to do fields first so methods can refer to them.
    let first_results = List.foldBack (norm000 ww 1 bindings idl pco) items []
    let second_results = List.foldBack (norm000 ww 2 bindings idl pco) items first_results
    second_results

and norm000 ww passno (normenv:annotation_map_t) class_idl pcref cil cc3 =
    let vd = 3 // Need to be able to recipe set this please
    let tycontrol =
        {
            grounded=false
        }
            
    match cil with
        | CIL_class(srcfile, flags, cr, gformals, extends, items) ->
            if passno=2 then cc3
            else
                let mf = "norm No_class from CIL_class " + hptos class_idl
                let ww = WF 3 "norm000" ww mf
                let eidl = if nonep extends then [] else cil_classrefToIdl ww Map.empty (valOf extends)
                let idl = class_idl
                let cargs = List.fold (scan_cust_args ww false class_idl) [] items
                let remote_marked = op_assoc "RemoteClass" cargs
                let sealedf = memberp Cilflag_sealed flags
                let structf = eidl = [ "ValueType"; "System" ] || memberp Cilflag_valuetype flags
                let mm = if structf then "cil_directory struct" else "cil_directory class"
                let (rn_, always_nil_) = cr2idl_formals ww cr // unused?
                let ww = WF 3 "norm000" ww (mf + ": " + mm + " " + hptos idl  + sprintf " with %i type formals" (length gformals))
                if not (nullp always_nil_) then sf "rhdhfd - array has special stuff..."
                let (found, ov) = g_normbase.TryGetValue(idl) // Try merge with previous partial class.
                // TODO stop if found ... muddy?
                let lower_fancy_tyfid = function
                    | Cil_tp_arg(_, _, tfid) -> tfid
                    | other -> sf (sprintf "other form fancy tfid %A" other)
                let gformals = map lower_fancy_tyfid gformals
                let whiten x = whiten2 ww mf normenv x
                let wflist =
                    let whiten tfid =
                        let white_name = tfid // funique(tfid + "_w") - now a nop
                        CTVar{ g_def_CTVar with id=Some white_name; }
                    List.zip gformals (map whiten gformals)

                //mutadd cCC.m_additional_static_cil
                    // TODO do this again
                //dev_println (sprintf ">H check already_constructed statics?")
                // compile_auxiliary_static_cctors cc00 (rev !cCC.m_classes_where_statics_constructed)
                //Not being used yet:
                //let _ = (List.foldBack (further_static_work ww) [items] [])

                let normenv' =
                    let augment_normenv (cc:annotation_map_t) (f, a) =
                        vprintln 3 ("  norm000 bind template black/white " + f + " to " + cil_typeToStr a)
                        cc.Add([f],  RN_monoty(a, []))
                    List.fold augment_normenv normenv wflist  
                let extendso = match extends with
                                | None -> None
                                | Some pt ->
                                    match cil_type_n ww (tycontrol, normenv') pt with
                                        | dt when dt=g_ctl_object_ref -> None
                                        | ty         ->
                                            //let _ = vprintln 3 (sprintf "extends %s" (cil_typeToStr ty))
                                            Some ty

                let rec setup_vt = function
                    | CT_cr crst ->
                        match crst.crtno with
                            | CTL_record(idl, cst, lst, len, ats, binds, _) ->
                                if not_nonep cst.parent then setup_vt (valOf cst.parent)
                                else // We use the counter at the top of the class hierarchy, excluding object.
                                    let rr = !cst.m_next_vt
                                    let _ = mutinc cst.m_next_vt 1
                                    rr
                            | other ->
                                let _ = vprintln 3 (sprintf "setup_vt not expecting  %s" (cil_typeToStr other))
                                0


                    | other -> sf (sprintf "setup_vt_other " + cil_typeToStr other)

                let has_vtable = not sealedf
                let cdt =
                    {
                        // What about implemented interfaces? We assume the C# compiler has ensured they are indeed implemented and they are, currently, not further noted here.
                        structf=           structf
                        ats=               []
                        parent=            extendso
                        name=              idl
                        newformals=        gformals
                        whiteformals=      wflist
                        arity=             length wflist
                        class_item_types=  []
                        class_tag_list=    []
                        vtno=              (if not has_vtable then None else Some(if nonep extendso then 0 else setup_vt (valOf extendso)))
                        m_next_vt=         ref 1
                        sealedf=           sealedf
                        remote_marked=     remote_marked
                    }

                if not_nonep cdt.vtno then vprintln 3 (sprintf "allocated vtno %A for %s with parent=%A" cdt.vtno (hptos idl) (not_nonep extendso)) // not needed for a struct
                let cdtref = ref (Some cdt)
                let (new_items, gamma, class_tag_list, prats) = unzip4(apply_norm000 ww normenv' idl cdtref items)
                let gamma = list_flatten gamma
                let cst = { cdt
                              with class_item_types=  gamma
                                   class_tag_list=    list_flatten class_tag_list
                          }
                cdtref := Some cst // Repatch parent now fully-defined.
                let bottom_drops = gamma
                (No_class([srcfile], flags, idl, CT_class cst, new_items, list_flatten prats), bottom_drops, [], [])::cc3 // dt does not need to be put in onward gamma since it is a 'root' item.

        | CIL_field(layout, flags, starter, cr_name, att, attributes) ->
            if passno=2 then cc3
            else
                let class_ids = hptos class_idl
                let field_ids = cil_typeToStr cr_name
                let mm0 =  sprintf "norm field %s from class %s" field_ids class_ids
                let ww = WF 3 "norm000" ww mm0
                let ptag =
                    match cr_name with
                        | Cil_cr id -> id
                        | other -> sf ("other complex field name: " + mm0)

                let demodreq = function // We extract volatile information here rather than taking note of Cilip_volatile: please give an example and say why.
                    | Ciltype_modreq(t, needed_cr) ->
                        let what = mr_dig needed_cr
                        if what="IsVolatile" then (t, true)
                        else
                            vprintln 0 (sprintf "+++ ignored storage class '%s' in classref %s" what (cil_typeToStr needed_cr))
                            (t, false)
                    | other -> (other, false)
                let (cr1, volf) = demodreq starter


                let is_static = memberp Cilflag_static flags
                let (newflags, static_cache_flag) =
                    if is_static && field_ids.IndexOf "mg$cache" > -1 then                 // Find Microsoft-generated static cache fields.
                        vprintln 2 (sprintf "This field heuristically detected as a C# assigned-once cache '%s'." field_ids)
                        ([Cilflag_hpr g_assign_once_cache_var], true)
                    else ([], false)


                // This does something that needs explaining  with 1-D arrays
                let ct = cil_type_n ww (tycontrol, normenv) cr1        // A field just has a type - no formals! So no split off is needed.
#if SPARE
                let rec add_vol_flag_2_ = function // Why do we have second copy of this locally?
                    | CTL_net(_, w, s, a) -> CTL_net(true, w, s, a)
                    | CTL_record(a, cst, lst, len, ats, binds, srco) -> CTL_record(a, cst,  map (fun crt -> {crt with dt=add_vol_flag_2_ crt.dt; }) lst, len, ats, binds, srco)
                    | other ->
                        let _ = vprintln 0 ("+++ ignored add_vol_flag_2 other:" + typeinfoToStr other)
                        other
#endif
                // CSharp does not allow the volatile keyword on ulong's and perhaps others, so here we support a secondary attribute flag.
                let idl = ptag :: class_idl
                let cargs = List.fold (scan_cust_args ww false idl) [] attributes
                let utag = funique ptag  // Uniquify for easy loop detection without ambibuity, e.g. in linked-list and tree-based applications.        
                let additional_volf =
                    let custom_volf (ss, args1) = isPrefix "Volatile" ss
                    disjunctionate custom_volf cargs 
                vprintln 3 (sprintf "Normalised field %s utag=%s " mm0 utag + sprintf " volf=%A additional_volf=%A" volf additional_volf + sprintf " length of ats=%i or %i" (length attributes) (length cargs))

                let volf = volf || additional_volf
                let ct = if volf then add_vol_flag_2 ct else ct


                let fl_idl = [ptag]
                let drop = (fl_idl, RN_monoty(ct, [])) 
                let fld_st =
                    {
                      ptag=             ptag
                      utag=             utag
                      volf=             volf
                      fld_parent_class= (fl_idl, pcref)
                      ct=               ct
                      static_cache_f=   static_cache_flag
                      gamma=            [] // for now, until typechecking .... overkiller
                    }
                (No_field(layout, newflags@flags, ptag, fld_st, att, cargs), [drop], [(ptag, utag)], []) :: cc3


        | CIL_custom(s, ty, (fr_raw, id, gargs), args, vale) ->
            if passno=2 then cc3
            else
                // TODO norm the ty here
                //let drop = ([s], RN_tmp2)
                let idl = id :: rev(fwd_flatten_cc fr_raw [])
                let idl = if hd idl = ".ctor" then tl idl else idl
                let get_ids = function
                    | Cil_explist((Cil_id id)::_) -> id
                    | other -> sf(sprintf "Parse Kiwi class %s custom attribute %s- expect Cil_id string not %A" (hptos class_idl) (hptos idl) other)
//Examining class custom attribute: KiwiSystem.Kiwi.MemSpace..ctor in Cil_explist [Cil_id "space2"]
                //vprintln 3 (sprintf "Examining class custom attribute: %s in %A"  (hptos idl) (vale))
                let prats = 
                    match idl with
                        | [ "MemSpace"; "Kiwi"; "KiwiSystem" ] when length args >= 1 ->
                            let space = get_ids vale
                            let size =
                                let default_size = (valOf !g_kfec1_obj).default_dynamic_heapalloc_bytes
                                // If allowed to override by attribute here, then first one needs to set the size.
                                default_size
                            vprintln 3 (sprintf "Parse (for %s) KiwiSystem class custom attribute %s: noting attribute arg '%s'" (hptos class_idl) (hptos idl) space)
                            [ (g_bondout_heapspace_name, space); (g_bondout_heapspace_size, i2s64 size) ]

                        | _ ->
                            match vale with
                                | Cil_blob _ ->
                                    vprintln 3 (sprintf "Ignored custom_attribute blob for %s "  (hptos idl))                
                                    []
                                | _ ->
                                    vprintln 3 (sprintf "Ignored class custom attribute: %s with args %A"  (hptos idl) (vale))
                                    []

                (No_esc cil, [], [], prats)  :: cc3

        | CIL_method(srcfile, (cl_cr, cl_fap, meth_fap), flags, ck, rtype, (id, unique_id), arg_formals, flags1, instructions) ->
            if passno=1 then cc3
            else
                let id = cil_typeToStr id
                let mm0 = sprintf ": norm method %s from class %s   uid=%s" id (hptos class_idl) unique_id
                let is_static = memberp Cilflag_static flags
                let ww = WF 3 "norm000" ww  mm0
                let cl_idl = cr_flatten_or_error cl_cr
                let idl = id :: cl_idl
                let n_faps = length cl_fap        
                let class_generics = map (lift_fap_values ww (tycontrol, normenv) "No_method cgr") (cl_fap)  // TODO call a formal norm here...
                let method_generics = map (lift_fap_values ww (tycontrol, normenv) "No_method mgr") (map rex_fap meth_fap)  // TODO call a formal norm here...        

                let normenv' =
                    let auga methodf0 (mm:annotation_map_t) = function
                        | RN_monoty (CTVar vv, ats) when nonep vv.idx ->
                            if vv.methodf<>methodf0 then sf (sprintf "Xa methodf mismatch  L1335 methodf=%A vv.methodf0=%A vv=%A" methodf0 vv.methodf vv)
                            let fid = valOf vv.id
                            //vprintln 0 ("   norm000 binds fid " + fid)
                            mm.Add([fid], RN_monoty(CTVar vv, ats))
                        | RN_monoty (CTVar vv, ats) when not_nonep vv.idx ->
                            if vv.methodf<>methodf0 then sf (sprintf "Xb methodf mismatch  L1335 vv=%A" vv)
                            let fid = sprintf "TEX%i" (valOf vv.idx)
                            //vprintln 0 ("   norm000 binds fid " + fid)
                            mm.Add([fid], RN_monoty(CTVar vv, ats))
                        | arg -> sf(sprintf "other auga %A" arg)
                    let normenv = List.fold (auga false) normenv class_generics
                    List.fold (auga true) normenv method_generics            

                let signat' =
                    let norm_meth_formal ww2 = function
                        | Cil_fp(rpc_bindo, po, CTVar vv, id) when nonep vv.id ->
                            let n = valOf vv.idx
                            if n < 0 || n>= n_faps then sf (mm0 + sprintf ": method generic formal index %i out of range 0..%i" n (n_faps-1)) // TODO these found on calls only, not definitions?
                            (rpc_bindo, po, CTVar vv, id)
                        | Cil_fp(rpc_bindo, po, rawty, id) ->                
                            //vprintln 0 (m + sprintf ": Norm method formal %A" id)
                            let ty = cil_type_n ww2 (tycontrol, normenv') rawty
                            (rpc_bindo, po, ty, id) 
                    map (norm_meth_formal (WN "method formal" ww)) arg_formals

                let disamname =
                    let argpairs = [] // Do not disambiguate overloads on method generics (for now).
                    let sq (_, _, ty, id) = cil_typeToSquirrel ty argpairs
                    hptos (list_flatten(map sq signat') @  [ id ])
                // We do not overload/override on return type so we include only the method name and the formal types in the disambiguation name
                if vd>=4 then vprintln 4 (sprintf "disamname=%s" disamname)
                let rtype' = cil_type_n (WN "method return type" ww) (tycontrol, normenv') rtype

                let cl_type = CT_cr { g_blank_crefs with name=cl_idl; cl_actuals=class_generics; meth_actuals=method_generics; }  // Q. shouldn't this be a class with formals, not a cr with actuals? Want to use a nominal reference here to save norming the parent class.. a bit silly.
                let drop = ([unique_id], RN_call(RN_monoty(cl_type, []), RN_monoty(rtype', []), [],  [], class_generics, method_generics))   // TODO arg types here etc.. ?

                let atts =
                    let custom_pred = function
                        | CIL_custom(s, t, fr, args, vale) -> true
                        | _ -> false
                    List.filter custom_pred instructions

                let (has_cpy, has_macro) =
                    let (m_has_cpy, m_has_macro) = (ref false, ref false)
                    let rec scn arg =
                        match arg with
                        | CIL_instruct(_, _, _, Cili_isinst _)   -> m_has_macro := true
                        | CIL_instruct(_, _, _, Cili_initblk)         // unsafe copy: three args on stack: memset(dest, val, length in bytes).
                        | CIL_instruct(_, _, _, Cili_cpblk)      -> m_has_cpy := true 
                        | CIL_instruct(_, _, _, Cili_call _) when inHardware_check arg -> m_has_cpy := true
                        | _  -> ()
                    app scn instructions
                    (!m_has_cpy, !m_has_macro || !m_has_cpy)
                let potentially_mutated_formals = // Owing to call-by-value, formals updated with stloc must be freshened. Also, those that are ataken will (generally) be updated with a stind, and likewise are considered mutated.  This applies to fields inside structs too.  Since such fields are modifiable by setters, considerable digging is needed to find whether they are modified, and we do not spot this here, instead making explict copies of all passed-by-value structs - the user should be aware that structure passing is potentially inefficient.
                    let gather1 cc arg =
                        match arg with
                        | CIL_instruct(_, _, _, Cili_ldarga idx)
                        | CIL_instruct(_, _, _, Cili_starg idx) ->
                            //vprintln 3 (sprintf "logging-mutated_formals YES for %s" (cil_classitemToStr "" arg ""))
                            singly_add (ciltoarg_symb [] idx) cc
                        | other ->
                            //vprintln 3 (sprintf "logging-mutated_formals NO for %s" (cil_classitemToStr "" arg ""))                    
                            cc
                    List.fold gather1 [] instructions 

                // In general, we need to know which singletons have their address taken in case we encounted a stind, stobj or stelem instruction where we have failed to track the dataflow id (aka nemtok) because, then, at least we can preserve knowledge over all singletons that are never address taken.
                let ataken_locals = 
                    let gather2 cc = function
                        | CIL_instruct(_, _, _, Cili_ldloca idx) -> singly_add (ciltoarg_symb [] idx) cc
                        | _ -> cc
                    List.fold gather2 [] instructions 

                let ataken_formals = 
                    let gather3 cc = function
                        | CIL_instruct(_, _, _, Cili_ldarga idx) -> singly_add (ciltoarg_symb [] idx) cc
                        | _ -> cc
                    List.fold gather3 [] instructions 
                let is_ctor = constructor_name_pred (hd idl)
                let is_accelerator_marked = checkfor_tokmarked_method ww vd [ "PipelinedAccelerator" ] instructions
                let is_root_marked        = checkfor_tokmarked_method ww vd [ "HardwareEntryPoint" ] instructions
                let is_clock_marked       = checkfor_tokmarked_method ww vd [ "ClockDom" ] instructions                
                let is_fast_bypass        = checkfor_tokmarked_method ww vd [ "FastBitConvert"] instructions  // This is the only bypass method at the moment, but table lookup needed in future.

                let clknets = parseKiwiClockDom ww vd mm0 is_clock_marked 
                let (is_remote_marked, fsems) =
                    match checkfor_tokmarked_method ww vd [ "Remote" ] instructions with
                        | None -> (None, None)
                        | Some client_stub ->
                            let (prams, protocol, searchbymethod, externally_instantiated, fsems) = parse_kiwi_remote_protocol_attributes ww vd mm0 client_stub
                            (Some(prams, protocol, searchbymethod, externally_instantiated, fsems), Some fsems)
                    
                let (is_hpr_prim, sri_arg, fu_kind, fsems) =
                    match checkfor_tokmarked_method ww vd [ "HprPrimitiveFunction" ] instructions with
                        | Some arg ->
                            let (sri_arg, fu_arg, fsems) = parse_kiwi_primitive_function_attributes ww mm0 arg
                            //dev_println(sprintf "FRI hpr primitive: %s: fu=%A sri_arg=%A" (hptos idl) fu_arg sri_arg)
                            (true, sri_arg, (if nonep fu_arg then None else Some [valOf fu_arg]), Some fsems)
                        | None -> (false, None, None, fsems)

                vprintln 3 (sprintf "log method %s: potentially_mutated_formals=%A has_macro=%A has_cpy=%A" (hptos idl)  potentially_mutated_formals has_macro has_cpy)
                let mdt' =
                    {
                        meth_parent_class=(cl_idl, pcref)
                        gamma=                   []
                        rtype=                   rtype'
                        arg_formals=             signat'
                        ty_formals=              meth_fap
                        name=                    idl
                        uname=                   unique_id
                        ataken_formals=          ataken_formals
                        ataken_locals=           ataken_locals
                        mutated_formals=         potentially_mutated_formals
                        has_macro=               has_macro
                        has_macro_regs=          has_cpy
                        is_ctor=                 is_ctor
                        is_root_marked=          is_root_marked
                        is_remote_marked=        is_remote_marked
                        is_accelerator_marked=   is_accelerator_marked
                        is_fast_bypass=          is_fast_bypass
                        is_static=               is_static
                        is_hpr_prim=             is_hpr_prim
                        sri_arg=                 sri_arg
                        fu_arg=                  fu_kind
                        fsems=                   fsems
                        clknets=                 clknets
                    }
                (No_method(srcfile, flags, ck, (idl, unique_id, disamname), mdt', flags1, instructions, atts), [drop], [], []) :: cc3

        | other ->
            hpr_yikes (sprintf "Cil norm000 item from %s: other form ignored: " (hptos class_idl) + cil_classitemToStr "" other "")
            cc3



// Lookup AST item in normbase and if not already there add it by searching in the raw AST item directory.
// Bindings is just passed to recursive calls of norm000.
and normed_ast_item_lookup00 ww idl = normed_ast_item_lookup_wb ww Map.empty idl

and normed_ast_item_lookup_wb ww bindings idl0 =
    if idl0 = ["Array"; "System"] then ([No_proxy "System-Array"], 1) // an unusual anomally - owing to lack of template
    else
    let vd = !g_unif_loglevel
    let idl = setget_munge idl0
    //vprintln 0 (sprintf "normed_ast_item_lookup ^%i %s aka %s" (length idl) (hptos idl) (hptos idl0))
    let rec idl_match = function // why not use built-in equality - cannot insert print debugs.
        | ([], []) ->
            if vd>=8 then vprintln 8 ("   ast item lookup match successful")
            true
        | (_, []) 
        | ([], _) ->
            if vd>=8 then vprintln 8 ("   ast item lookup match fail on length mistmatch")            
            false
        | (p::ps, q::qs) ->
            let ok = p=q
            if vd>=10 then vprintln 10 (sprintf "   ast item lookup: cf %s with %s --> %A" p q ok) // A lot of output - so only enable for high vd
            ok && idl_match(ps, qs)

    // See if normed already and return it: recursive types need this behaviour.
    // All parts of partial classes must be found at once by the kfilter call.
    // Hence bindings must be a cat of all defs from all input files.
    let (found, ov) = g_normbase.TryGetValue idl
    if found then ov
    else
    let cCC = valOf !g_CCobject
    let ids = hptos idl
    let ww = WF 3 "normed_ast_item_lookup" ww (sprintf "start %s (toks=%i)" ids (length idl))
    let r0 =                  
        let kfilter c (a, vale) =  // Collate all those with this prefix (assemble a partial class). TODO use a cheaper tree scan, not list.
            if vd>8 then vprintln 8 (sprintf "normed_ast_item_lookup cf %s with %s " ids (hptos a))
            if idl_match(idl, a) then singly_add vale c else c
        List.fold kfilter [] !cCC.directory

    let rec pclass_flatten = function
        | No_class(srcfile0, flags0, idl0, CT_class cst0, items0, prats0)::No_class(srcfile1, flags, idl, CT_class dt, items, prats1)::tt ->
            let items' = items0 @ items
            // could check similar formals, structf and flags etc ... here.
            let _ = vprintln 3 ("Now have " + i2s(length items') + " items in partial class " + hptos idl + " was " + hptos idl0)
            //cassert(idl=idl0, "idl=idl0")
            //vprintln 0 (sprintf "gformals=%A" dt.whiteformals)
            //vprintln 0 (sprintf "gformals0=%A" cst0.whiteformals)
            //cassert(dt.whiteformals=cst0.whiteformals, "gformals=gformals0") // assert fails owing to whiten silly - TODO reorg
            //cassert(extends=extends0, "extends=extends0")
            let dt1 = { dt with
                          class_item_types= cst0.class_item_types @ dt.class_item_types
                          class_tag_list=   cst0.class_tag_list   @ dt.class_tag_list
                        }
            pclass_flatten(No_class(lst_union srcfile0 srcfile1, lst_union flags0 flags, idl, CT_class dt1, items', prats0@prats1)::tt)

        | other -> other
    let count = function
        | (_, _, CIL_class _) :: tt_ -> 1 // There is only one class of a given name, even if composed of partials.
        | other -> length other
    let (tyknot, noknot) = (ref None, ref None)
    let c = count r0

    if c=0 then ([], 0)
    else
    let ans = ([No_knot(CT_knot(idl, "FP1116", tyknot), noknot)], c)
    g_normbase.Add(idl, ans)

    let ww = WF 3 "normed_ast_item_lookup" ww (sprintf "Knot entered for %s (ready for norm000 call)" ids)
    let pco = // If singleton methods or fields are nominated for synthesis best get their parent class here.
        let m_pco = ref None
        let parents =
            let mine_parents cc = function
                | CIL_field _
                | CIL_class _ -> cc
                | CIL_method(srcfile_, cr, flags, ck, rtype, (id, unique_id), arg_formals, flags1, insns) -> singly_add cr cc
                | other ->
                    let _ = vprintln 0 (sprintf "No class normed other top (arity=%i) for %s: %A" (length cc) (hptos idl) other)
                    cc
            List.fold mine_parents [] (map f3o3 r0)
        
        match parents with
            | cr::crs ->
                let _ =
                    if not (nullp crs) then // multiple inheritance not supported
                        vprintln 3 (sprintf "Norm_class: multiple inheritance: multiple parent names - ignoring all but first (arity=%i) for %s: %A" (length parents) (hptos idl) parents)
                        ()
                let (parent_idl, fap) = (cr_flatten_or_error(f1o3 cr), f2o3 cr) // TODO generate this way to start with
                let pis = hptos parent_idl
                let _ = vprintln 3 (sprintf "parent idl=%s for %s" (pis) (hptos idl))
                if pis = "<Module>" then m_pco // here discard <Module>
                else
                let _ =  // We look up the parent to eagerly get it normalised and stored.
                    match normed_ast_item_lookup_wb ww bindings parent_idl with
                    | ([No_knot(CT_knot(idl_, w, kv), kno)], nn) when not(nonep !kv) -> () // ([valOf !kv], -1)
                    | ([No_proxy(sadid)], nn) -> ()
                    | x ->
                        dev_println (sprintf "other form parent: other=%A" x)
                //vprintln 0 ("TOP22ss  " + hptos idl)
                let (r, nn_) = normed_ast_item_lookup_wb (WN "parent mine" ww) bindings parent_idl
                //vprintln 0 ("TOP22ee  " + hptos idl)
                match r with
                    | ([No_knot(CT_knot(idl, whom, vv), kno)]) when not(nonep !kno) ->
                        match valOf !vv with
                            | CT_class cst -> m_pco := Some cst
                            | CTL_record (_, cst, items, fap_, _, binds, _) -> m_pco := Some cst //A waste of detail.
                            | other -> sf (sprintf "xoo2 %A" other)
                        m_pco
                    | (No_proxy "System-Array") :: _ ->
                        m_pco := Some g_array_boll
                        //let _ = dev_println("Failed to tie parent knot for " + hptos idl)
                        m_pco
                    | other ->
                        // Some canned libraries have parents wrongly given? They are just nested, not extending.
                        dev_println (sprintf "xoo: other parent form for %s. form=%A" (hptos parent_idl) other)
                        m_pco

            | [] ->
                let _ = vprintln 3 (sprintf "Norm class normed: no parents for %s" (hptos idl))
                m_pco
    let (r1, gamma, class_tag_list, prats__) =
        // Main (non-recursive) call to norm000
        unzip4 (apply_norm000 ww bindings idl pco (map f3o3 r0))
    let r1 = pclass_flatten r1 

    // There are sometimes method generic tyvars so the env for a method is not the same as a class.
    let gamma = list_flatten gamma
    let insert_starting_gamma = function
        | No_class(srcfile, flags, idl, CT_class cst, items, prats) ->
            let cst' = { cst with class_item_types=gamma; class_tag_list=list_flatten class_tag_list }
            (CT_class cst', No_class(srcfile, flags, idl, CT_class cst', items, []))

        | No_method(srcfile, flags, ck, id, mdt, flags1, insns, ats) ->
            let mdt' = { mdt with gamma=gamma; }
            (CT_method mdt', No_method(srcfile, flags, ck, id, mdt', flags1, insns, ats))

        | No_field(layout, flags, name, fdt, att, attributes) ->
            let fdt' = { fdt with gamma=gamma; }
            (fdt'.ct, No_field(layout, flags, name, fdt', att, attributes))
            
        | other -> sf (hptos idl + sprintf ": insert gamma other %A" other)
    let (dt1, nitems1) = List.unzip (map insert_starting_gamma r1)

    let ww =
        if length dt1 = 1 then
            noknot := Some (hd nitems1) // Tie knot.    
            tyknot := Some (hd dt1) // Tie knot.
            WF 3 "normed_ast_item_lookup" ww (sprintf "Knot %s tied" ids)
        else
            // Both overloads and partial classes can lead to multiple items with the same name.  I think we join up partial classes here but overloads must be kept separately.
            //vprintln 0 (sprintf "multiple l1=%A\nl2=%A" dt1 nitems1)
            noknot := Some (No_list nitems1) // Tie knot.    
            tyknot := Some (CT_list dt1) // Tie knot.
            WF 3 "normed_ast_item_lookup" ww (sprintf "Knots %i %s tied" (length dt1) ids)

    vprintln 3 ("Installed normbase " + hptos idl + sprintf " with lr1=%i " (length r1) + " lr0=" + i2s(length r0) + " c=" + i2s c)

    // If concrete (no templates) then we can generate the dtable now. (Makes later log files easier to read.)
    let gen_dtable_eager =
        match dt1 with
            | [CT_class cdt] when nullp cdt.whiteformals -> true
            | others -> // e.g. CT_method etc ...
                //let _ = vprintln 3 (sprintf "No eager dtable gen for %s since has form ^%i %s" (hptos idl) (length others)  (sfold cil_typeToStr others))
                false
    let enumf = no_is_enum r1
    let _ = 
        if enumf || gen_dtable_eager then
            let ww = WF 3 "normed_ast_item_lookup" ww (sprintf "Eager generate dtable for %s " ids)
            let (fct:typeinfo_t) = cil_gen_dtable (WN ("class_find gen_dtable " + ids) ww) [] (RN_idl idl, hd r1)
            //vprintln 0 (sprintf "enumf=%A dtable supposedly grounded is %A" enumf gen_dtable_eager)
            match fct with
                | CT_star(n, ((CTL_record _) as fct1)) ->
                    let insert_grounded_type = function
                        | No_class(srcfile, flags, idl, cdt, items, prats) -> cdt
                        | other  -> sf ("insert new in igt other")
                    //let _ = if length r1 = 1 then tyknot := Some(insert_grounded_type (hd r1)) 
                    let _ =
                        noknot :=
                          match valOf !noknot with
                            | No_class(srcfile, flags, idl, _, items, prats) -> Some(No_class(srcfile, flags, idl, fct1, items, prats))
                            // Why has this not discarded the body_gamma? Preserved through gen_dtable perhaps...
                            | other -> sf (sprintf "other L1216 %A" other)

                    tyknot := Some fct1 // Retie with lowered form.
                    ()
                | other ->
                    vprintln 3 (ids + sprintf ": Did/could not unclass other %A" (cil_typeToStr other))
                    ()
        elif conjunctionate trivially_grounded_leaf dt1 then ()
        else vprintln 3 (sprintf "enumf=%A No eager dtable make for " enumf + sfold cil_typeToStr dt1)
    let ww = WF 3 "normed_ast_item_lookup" ww ("finished " + ids)
    ans




//
// id_lookup invokes the core lookup and has a handler on failure
// Look on search path and dynamically load if not already loaded.  
//
and normed_ast_item_lookup_wb_wfail ww aof bindings idl00 =
    let cCC = valOf !g_CCobject
    let vd = !g_filesearch_vd
    let m_alt_name = ref None
    let lookup2 idl = 
       let (ans, nn) = normed_ast_item_lookup_wb ww bindings idl
       //dev_println (sprintf "no of items is nn=%i for %s" nn (hptos idl))
       if nn=0 then None else Some(ans, nn)

    let lookup1 tryloadf as_alias idl =
            if tryloadf=0 then lookup2 idl
            else
                let ids = hptos idl
                let suf = "dll"
                let is_exe = false
                let ids_dll = ids + "." + suf

                let rec library_search = function
                    | [] -> None
                    | search_place::tt->
                        let tentative = path_combine(search_place, ids_dll)
                        let ef = existsFile tentative
                        if vd>=4 then vprintln 4 (sprintf "Dynamic load: searching for ids=%s in place=%s.    existsFile=%A" ids tentative ef)
                        if ef then Some tentative else library_search tt

                let hpr_path = path_combine(opath.get_hpr_path true, path_combine("..", "support")) // The poorly named userlib becomes 'support' when in the kdistro.
                match library_search (hpr_path :: !opath_hdr.g_ip_incdir) with
                    | None -> None
                    | Some tentative ->
                        // Need to set dump separately for late-loading dlls
                        // This demand-driven loading does not work for static class constructors at the moment .... hence we rely on cil_digest.
                        let ast = read_dotnetfile_and_dump ww { cCC.settings with cil_dump_separately=true } (tentative, hptos idl, suf) []
                        let srcfilename = tentative
                        let install_ast cc (is_exe_, filename_, ast) = foldl_withatts (install_in_cil_directory ww g_cd0 srcfilename is_exe cCC.directory ((cCC.settings.directorate_style, cCC.settings.directorate_attributes), None, as_alias)) cc ast
                        let (_,_) = List.fold install_ast ([], []) ast
                        //omutadd cCC.m_additional_static_cil (List.foldBack (further_static_work ww) ast [])
                        lookup2 idl00     // We try to dynamically load the alternate name, but it will be installed under the original name. Hence use idl00 here.

    let lookup0 alternate_namef tryloadf idl = 
        let ids = hptos idl
        if alternate_namef>0 then
            match cCC.library_substitutions.lookup idl with
                    | None ->
                        vprintln 2 (sprintf "KiwiC Library Substitutions: No alternate (Kiwi-specific) implementation of library %s found." ids)
                        m_alt_name := Some (sprintf "(And no Kiwi alias was listed for this in library_substitutions.)")
                        None
                    | Some alt_name ->
                        m_alt_name := Some (sprintf "(Also looked up under the Kiwi library_substitutions alias %s, but this seemingly did not contain a relevant definition.)" (hptos alt_name))
                        vprintln 2 (sprintf "KiwiC Library Substitutions: Using alternative (Kiwi-specific) name %s for library %s." (hptos alt_name) ids)
                        // We need an alias mapping establised to rename the substituted defintions with the conventional name as they are entered in the ast directory.
                        let as_alias = (rev alt_name, idl) // We break convention having the from not in reversed order for ease of prefix lookup.
                        lookup1 tryloadf as_alias alt_name
        else lookup1 tryloadf ([], []) idl

    let ans = lookup0 0 0 idl00
    if not_nonep ans then ans 
    else
    let ans = lookup0 0 1 idl00
    if not_nonep ans then ans
    else
    let ans = lookup0 1 0 idl00
    if not_nonep ans then ans
    else
    let ans = lookup0 1 1 idl00
    if not_nonep ans then ans
    else
        if aof then // abend on fail holds?
            let list_them (a, b) = sprintf "  ^%i:%s"  (length a) (hptos a)
            vprintln 0 ("items in scope were :" + sfold list_them !cCC.directory)
            let m1 = if nonep !m_alt_name then "" else valOf !m_alt_name
            cleanexit(sprintf  "Class, field or method (tokens=^%i) '%s' not defined (L1027). %s" (length idl00) (hptos idl00) m1)
        else None


//
// Another id_lookup function: this invokes the core lookup and has a handler on failure
//   
and id_lookupp ww idl bindings handler =
    let (r, lr) = normed_ast_item_lookup_wb ww bindings idl  // cf _wfail routine that doubles up. TODO.
    if lr=1 then Some(follow_no_knot(hd r)) // <-- following knot may not be a good idea during definition?
    //if lr=1 then Some(hd r)    
    else
        let (m, ret_code_) = handler()
        if m = "" then None
        else 
            let cCC = valOf !g_CCobject
            vprintln 0 "+++ Available bound identifiers are being written to a file."
            report_dir_contents !cCC.directory
            if lr = 0 then vprintln 0 ("No match for identifier '" + hptos idl + "'")
            let mm = if lr > 1 then ("Ambiguous (L1189): '" + hptos idl + "' : More than one match for identifier.") else m
            vprintln 0 (sprintf " they are %A" r)
            sf mm
            None



(*
 * Lookup overloaded method/function by signature of the actuals.
 * Both cr and idl are passed in.  They should be de-templated for this lookup and the template filled out again by the caller on the returned result.  We can de-template anyway, for the caller, as in the 2d newobj ctor.
 *)
and id_lookup_fn ww  (cCP_:cilpass_t) (cCB_:cilbind_t) handler m_notice virtualf callsite idl actual_signat =
    let ww = WN "id_lookup_fn" ww
    let vd = !g_unif_loglevel
    let (r0, a_count) = normed_ast_item_lookup00 ww idl
    let r0 = List.foldBack knotflatten r0  []
    let no_of_named = length r0
    if vd>=4 then vprintln 4 (hptos idl +  sprintf ": methods of that name: count=%i" no_of_named)
    if vd>=4 then
        //reportx 4 "id_lookup_fn mq" (fun(a, ctype)->typeinfoToStr a + ": " + typeinfoToStr ctype) mqbind
        vprintln 4 (hptos idl + sprintf ": Scanning %i function alternatives in scope." a_count)

    let rec meth_arity mM =
        match mM with
        | No_method(srcfile, flags, ck, (gid, uid, disamname), mdt, flags1, instructions, atts) -> (mM, mdt.arg_formals)
        | other ->
             (
                 sf ("+++" + noitemToStr false other + ": called function is not a method: " + hptos idl)
             )
    let desired_arity = length actual_signat
    let r0 = map meth_arity r0
    let wq = map (sigmatch actual_signat desired_arity false) r0 (* Generate a weighted list of candidate callees. *)

    let rec hf v = function
         | [] -> v
         | (item, weight)::tt -> hf (if weight>v then weight else v) tt
    let highest = if no_of_named = 0 then 0 else hf (snd(hd wq)) (tl wq) // A fold

    let alst2 = if highest=0 then [] else List.filter (fun (item, weight) -> weight=highest) wq
    //report_nodir_contents(map (fun x->([noitem_name x], x)) r)
    let ok_in_arity() =
        let ok_in_arity_f (mM, s) = length s = desired_arity
        length (List.filter ok_in_arity_f r0)
    let no_of_pos = length alst2
    let _ =
        if no_of_named=0 then m_notice := Some(sprintf "There are no method definitions of that name found at all.")
        elif no_of_pos=0 then m_notice := Some(sprintf "Of %i named method definitions found. %i have arity %i and %i have appropriate type signature with arity." no_of_named (ok_in_arity()) desired_arity no_of_pos)

    let ans = 
        if length r0=0 then
            let (m, rc) = handler(0)
            let cCC = valOf(!g_CCobject) // OLD WAY still used for statics
            if m <> "" then (vprintln 0 (m + ": Function or method '" + hptos idl + "' not in scope."); report_dir_contents !cCC.directory)
            []
                
        elif no_of_pos=0 then
                ( report_nodir_contents(map (fun (x, _)->([noitem_name x], x)) r0);
                  vprintln 0 (hptos idl + sprintf ": Overload resolution problem. No suitable signature defined for called method/function out of %i available signatures" (length wq)); []
                )
        elif no_of_pos=1 then
            let ans0 = fst(hd alst2)
            if virtualf then // Find overrides and parents if a virtual call
                let methodname = hd idl
                let entry_class = tl idl
                let disamname = get_disamname ans0
                let get_updown_methods cc (x, y) =
                    if x=entry_class then singly_add y cc 
                    elif y=entry_class then singly_add x cc
                    else cc                        
                let overrides = entry_class :: (List.fold get_updown_methods [] (g_method_downcasts.lookup disamname))
                // use disamname now to select up/down variants in the inheritance tree
                match overrides with
                    | [] -> [ (idl, ans0) ]
                    | overrides ->
                        let get_all_inheriting cc class_idl =
                            let idl1 = methodname :: class_idl
                            let (rx, a_count_) = normed_ast_item_lookup00 ww idl1
                            let rx = List.foldBack knotflatten rx  []
                            match List.filter (fun x-> get_disamname x = disamname) rx with
                                | [] -> sf (sprintf "at least one declaration of a method called '%s' expected in class %s" methodname (hptos class_idl))
                                | [item] -> (idl1, item)::cc
                                | items ->
                                    // Should this go via the handler?
                                    //let _ = if no_of_pos=0 then  m_notice := Some(sprintf "%i named method definitions found. %i have suitable arity." no_of_named no_of_pos)
                                    sf (sprintf "more than one method '%s' declaration encountered in %s" methodname (hptos class_idl))
                        List.fold get_all_inheriting [] overrides

                        
            else [ (idl, ans0) ] // need to enumerate them

        else
            m_notice := Some(sprintf "%i named method definitions found. %i have suitable arity." no_of_named no_of_pos)            
            let (mm, rc) = handler(no_of_pos)
            if no_of_pos=0 then  m_notice := Some(sprintf "%i named method definitions found. %i have suitable arity." no_of_named no_of_pos)            
            vprint 0 "Available signatures were:"
            app (fun (x, v) -> vprintln 0 (get_src_ref_string x + "  " + noitemToStr true x)) alst2
            vprintln 0 ""
            vprintln 0 (hptos idl + sprintf ": (L1262) More than one defined method matches (count=%i)." no_of_pos)
            sf mm
            []
    ans





(*
 * This function pre-processes the canned libraries for insertion into dir.
 * The library is indexed with a reversed-order heirarchic name.
 *
 * We have to insert .ctor methods for classes as well as their class,
 * owing to current dir design.
 *)
and fillout_canned_dir ww prefix aA cc =
    let vd = -4
    match aA with

    | CIL_class(srcfile, flags, Cil_cr id, gformals_, extends_, items) -> // Casually-named class
        if vd>=4 then vprintln 4 ("Start adding casually-named canned class to library " + id) 
        let r = 2
        let p' = id :: prefix

        let auto_cons_argcount =
            let rec g = function
                | (Cilflag_autocons nargs :: t) -> Some nargs
                | _::t -> g t
                | [] -> None
            g flags
            
        let rec argfun = function
            | None   -> []
            | Some n -> if n=0 then [] else Cil_fp(None, None, g_ctl_object_ref, "ARG" + i2s n) :: argfun(Some(n-1))

        let ctor =
            if nonep auto_cons_argcount then []
            else
                if vd>=4 then vprintln 4 (sprintf "Adding synthetic .ctor arity=%i to library for class %s" (valOf auto_cons_argcount) id)
                //vprintln 0 (sprintf "+++Adding synthetic .ctor arity=%i to library for class %s" (valOf auto_cons_argcount) id)                
                let args = argfun auto_cons_argcount   // Automatically synth a ctor (constructor) when autocons requested.
                let ctor_type = g_ctl_object_ref
                let ck = CK_default true
                let flags1 =
                    {
                        is_startup_code=false 
                    } 
                [ CIL_method(srcfile, (Cil_cr id, [], []), flags, ck, ctor_type, (Cil_cr ".ctor", funique ".ctor"), args, flags1, []) ]
        let cc = List.foldBack (fillout_canned_dir ww p') (ctor @ items) cc
        if vd>=4 then vprintln 4 ("Finished adding casually-named canned class to library " + id) 
        (p', aA)::cc


    | CIL_class(srcfile, flags, cr, gformals_, extends_, items) ->
        // We do not synthesise a .ctor for these ones ... why not ?
        let idl = cil_classrefToIdl ww Map.empty cr
        if vd>=4 then vprintln 4 ("Start adding canned class to library " + (hptos idl)) 
        let cc = List.foldBack (fillout_canned_dir ww idl) items cc
        if vd>=4 then vprintln 4 ("Finished adding canned class to library " + (hptos idl))
        (idl, aA)::cc


    | CIL_method(srcfile, cr, flags, _, _ , (Cil_cr id, _), _, _, _) -> 
        let r= 2
        let p' = id :: prefix
        (p', aA)::cc

    | CIL_field(layout, flags, ctype, cr, att, _) -> cc (* For now - deleted ! how does array2d work then ? - well the whole class is in the dir and we never lookup individual fields in the dir... *)

    | other -> sf("fillout_canned_dir: other " +  cil_classitemToStr "" other "")




    
// 1-D array special processing.... kludge?
// Differences between formals/actuals versions: for an array the formal and actual are different
//  the lookup functionality in gb might also differ ?    we dont need gb for both
and cr2idl_formals ww cr =
    match cr with
        // CTVar{ g_def_CTVar with idx=Some 0; }
        | Ciltype_array(content_cr, dims) ->  (RN_idl [ "CBG-ARRAY1D"], [ Cil_tp_arg(false, 0, "CT0") ])
        | other -> cr2idl ww other

and cr2idl_actuals ww cr =
    match cr with
        | Ciltype_array(content_cr, dims) ->  (RN_idl [ "CBG-ARRAY1D"], [ Cil_tp_type content_cr ])
        
        | other -> cr2idl ww other


and cil_classrefToIdl ww cgr_ cr =
    let (idl, _) = cr2idl_nr ww cr  (* DEPRECATED =- USE cr2idl_nr directly *)    
    idl 
                      //muddy (sprintf "hereerere %A" other
                      //rn_valOf "classrefToIdl" (fst(





(*
  * CIL: Only support 1-D arrays at the lowest level, but some 2-d array forms in the type system!.
  * 
  * 
  * type_nf implements a table of attributes for the supported, builtin leaf types.
  * Some of the builtin types, such as System.Boolean are not as builtin as others, such as CTL_vt(K_int, 32, [], []).
  *
*)


//
// For the valuetypes, we might want the boxed or unboxed forms as a result of typen.
// We compromise by returning a CTL_net structure if we can and we assume all such CTL_net's inherit directly from System.Object
//
and cil_type_n ww tcn tT =
    let cCC = valOf !g_CCobject
    let (tycontrol, normenv) = tcn
    let vd = !g_unif_loglevel
    match tT with
        | (Cil_suffix_u1) 
        | (Cil_suffix_u2)
        | (Cil_suffix_u4)
        | (Cil_suffix_u8)
        
        | (Cil_suffix_i1)
        | (Cil_suffix_i2)
        | (Cil_suffix_i4)
        | (Cil_suffix_i8)
        
        | (Cil_suffix_i) 
        | (Cil_suffix_u) ->  cil_type_n_vt tT
        
        
        | CTL_vt _                   
        | Ciltype_int       
        | Ciltype_decimal
        | Ciltype_char      
        | Ciltype_bool    ->  cil_type_n_vt tT
        | Ciltype_string  ->  f_canned_c16_str 0

        // Type constants that need no work.
        | tT when trivially_grounded_leaf tT -> tT

        | CTVar vv ->
            let prefix = if vv.methodf then "!!" else "!"
            let lookup key = 
                match normenv.TryFind([key]) with
                    | Some(RN_tmp_id new_id) ->
                        vprintln 3 (sprintf "freshen  found '%s' for " new_id  + cil_typeToStr tT)
                        Some(CTVar{ g_def_CTVar with id=Some new_id; idx=vv.idx })
                    | Some(RN_monoty(ty, _)) -> Some ty
                    | None                   -> None
                    | other -> sf (sprintf "other form normenv %A" other)

            let ans = if nonep vv.idx then None else lookup (prefix + i2s(valOf vv.idx))  // Give priorty to indexed lookup over named lookup
            let ans = if nonep ans && not_nonep vv.id then lookup (valOf vv.id) else ans
            match ans with
                | Some ty -> ty
                | None ->
                    if tycontrol.grounded then
                        //let _ = report_env ()
                        cleanexit(sprintf "Dangling type var %s encountered when supposedly grounded." (cil_typeToStr tT)) // TODO say where !
                    else tT

            
        | CT_arr(ty, l) -> CT_arr(cil_type_n ww tcn ty, l)

        | CT_star(n, ty) -> gec_CT_star n (cil_type_n ww tcn ty)

        | CT_class dct ->
            let _ = vprintln 0 ("+++ Please dont type_n a class !! " + hptos dct.name)
            CT_class dct // Perhaps ensure this is never used

        | Ciltype_ref a -> // Translates simply to an &
            let a' = cil_type_n ww tcn a
            gec_CT_star (1) a'  // +ve stars are ampersands

        | Ciltype_star a -> // Translates simply to an asterisk
            let a' = cil_type_n ww tcn a
            gec_CT_star (-1) a' // ove stars are asterisks

        //| CT_valuetype a  ->
        //    let a' = cil_type_n ww tcn a
        //    CT_valuetype a'

        | Ciltype_array(a, dims) ->
            let a' = cil_type_n ww tcn a

            // Dotnet uses the convention that arrays and objects are passed by reference by default unless wrapped as valuetype annotation.
            // We make this more explicit, as does C++, by including a star in the signatures in ce expressions and kcode.
            // 3. All forms of Ciltype_array in the source code are autoref converted to &CT_arr(...).
            let autoref aat =  gec_CT_star 1 aat

            let gec_CT_ray ct =
                match ct with
                    | CT_arr _ ->
                        //dev_println "clause 0 - array of arrays without pointers - perhaps not used in C#?"
                        let ans = CT_arr(gec_CT_star 1 ct, None) // Why add a ref inside?
                        //vprintln 0 ("clause 0 ans = " + typeinfoToStr ans)
                        autoref ans 
                    | CTL_net _ ->
                        //vprintln 0 "clause 1"
                        autoref(CT_arr(ct, None))

                    | CT_cr _ 
                    | CTVar _ ->
                        if vd>=4 then vprintln 4 ("can't add autoref if dont know what it is: must do on depram step")
                        autoref(CT_arr(ct, None))           

                    | other ->
                        let ans = autoref(CT_arr(other, None))
                        //dev_println (sprintf "array gec_CT_ray: other=%s ans=%s "  (typeinfoToStr other) (typeinfoToStr ans))
                        // Typically yields: please fix: other=&(CT_arr(CTL_net(false, 32, Signed,[native]), <unspec>)) ans=&(CT_arr(&(CT_arr(CTL_net(false, 32, Signed,[native]), <unspec>)), <unspec>)) 
                        ans

            gec_CT_ray a' 

        | Cil_cr_flag(Cilflag_valuetype, ct) ->
           let k0 = cil_type_n ww tcn ct
           gec_valuetype k0

        | Cil_suffix_ref -> muddy "CTL_wond suffix_ref"
           
        | Ciltype_modreq(ct, needed_cr) ->
            let k0 = cil_type_n ww tcn ct
            let what = mr_dig needed_cr
            if what="Is_volatile" || what = "IsVolatile" then add_vol_flag_2 k0
            else
                vprintln 3 (sprintf "+++ unsupported form of modreq: %s applied to " what + cil_typeToStr ct)
                k0
                
        | CT_cr crst -> // cil_type_n clause.
            // TODO make and then use dropapp
            let ids = hptos crst.name
            let fxx = (cil_type_n ww tcn)
            let fnorm_dropapp = function
                | RN_monoty(dt, ats) ->  RN_monoty(fxx dt, ats)
                | other -> sf(sprintf "dropapp other form: " + drToStr other)
            let cl_actuals' =   map fnorm_dropapp crst.cl_actuals
            let meth_actuals' = map fnorm_dropapp crst.meth_actuals

            if tycontrol.grounded then // requesting a grounded form.
                //vprintln 3 (sprintf "Typenorm classref %s requesting grounded" ids)
                let rec bindfind arg =
                    match (*follow_no_knot*) arg with
                        | No_knot(CT_knot(idl, w, kv), noknot) when not_nonep !kv -> (valOf !kv, "arg")
                        | No_knot(CT_knot(idl, w, kv), noknot)                    -> (CT_knot(idl, w, kv), "noarg202")
                        | No_class(srcfile, flags, idl, dt, items, prats)         -> (dt, "arg")
                        | other -> sf (sprintf "L1386 other form %A" other)
                let (cdto, oc) =
                    // This is now nearly in crst (as field ? but not No_class)  if you want to avoid lookup ... for now.
                    // We still find things despite normenv=Map.empty, we just have no additional, local bindings (?).
                    match (id_lookupp ww crst.name (*normenv*)Map.empty (fun()->("cil_type_n grounded classref: class not defined: cr=" + ids, 1))) with
                        | Some v ->
                            match fst (bindfind v) with
                                | CT_knot(idl, w, kv) when nonep !kv             -> (None, v)
                                | CTL_record(a, cst, lst, len, ats, binds, srco) -> (Some cst, v) 
                                | CT_class cst                                   -> (Some cst, v)
                                | other                                          -> sf (sprintf "L1527 other %A" other) 
                        | None    -> sf  ("identifier not defined or in scope (L1389): " + ids)

                let pairs = // Do not bind any meth_actuals at this stage since there are no correspondng gfodmals and they are not needed for dtable which only has fields.
                    if nullp cl_actuals' then []
                    elif nonep cdto then muddy(sprintf "classref %s has generic types but has not been tied yet: cl=%A meth=%A" ids cl_actuals' meth_actuals')
                    else
                        let ww = WN "cil_type_n delit pairs" ww
                        let delit = function
                            | (fid, RN_monoty(aty, _)) ->
                                let _ = WF 3 "delit" ww ("cil_type_n delit: bound formal fid=" + fid + " to " + cil_typeToStr aty)
                                (fid, aty)
                        map delit (List.zip (valOf cdto).newformals cl_actuals')
                if vd>=4 then vprintln 4 (sprintf "type_n: bound formals giving pairs=%A" pairs)
                let ans = cil_gen_dtable (WN ("cil_type_n call gen_dtable " + ids) ww) pairs (RN_idl crst.name, oc)
                ans
                // Classref becomes a reference to a record but that ref pointer is added inside gen_dtable now.
                // For a struct we do not want the ref pointer - it should be a valuetype instead.
            else
                if ids <> "System.Object" then vprintln 3 (sprintf "Typenorm %s ungrounded" ids)                
                CT_cr { crst with cl_actuals=cl_actuals'; meth_actuals=meth_actuals'; } // The droppings will not get rehydrated until this is converted to a CTL_record ... but we stick with it as a CT_cr for now to avoid infinite record trees.
                     


        // We have two situations:
        //   Instantiation, such as :  .field class [itest1]catter1`1<[mscorlib]System/UInt32> fieldname
        //   Dtable generation, such as :  .field class [itest1]catter1`1<!T> fieldname
        // This depends on whether gbo is None or not

        | cr -> // User types should reach this point.
            let (idl, cl_fap) = cr2idl_nr ww cr
            let ids = hptos idl
            //let _ = vprintln 3 (ids + sprintf "cil_type_n cr masquerader: " cil_typeToStr cr)
            match vt_scan idl with
                | Some vto ->
                    //vprintln 3 ("Class '" + hptos idl + "'+ : masq: is in vt_table? " + boolToStr(vto<>None))
                    if not_nullp cl_fap then sf (sprintf "actuals should not be present on a valuetype: " + cil_typeToStr cr)
                    vto
                | None ->
                    let (r, a_count) = valOf_or_fail "L2598" (normed_ast_item_lookup_wb_wfail ww true normenv idl)
                    //vprintln 0 (sprintf "no of items for '%s' is %i" (hptos idl) (length r))
                    let rec no_to_type = function
                        | [] ->
                            let list_them (a, b) = hptos a
                            vprintln 0 ("items in scope were :" + sfold list_them !cCC.directory)
                            cleanexit (sprintf "cil_type_n: Class '%s' not defined (L1435)" (hptos idl))
                        | [item] ->
                            match (* follow_no_knot please here *) item with                          
                                | No_proxy "System-Array" -> CT_class g_array_boll
                                | No_class(srcfile, flags, idl, cdt, items, prats) ->
                                    match cl_fap with
                                        | [] -> cdt
                                        | lst ->
                                            match cdt with
                                                | CT_class cst ->
                                                    CT_cr { g_blank_crefs with name=idl; cl_actuals=map (lift_fap_values ww tcn "No_class") cl_fap; crtno=CT_class cst }
                                                | other -> sf (sprintf "expected a class form to bind fap %A, not %A" cl_fap other)

                                | No_knot(CT_knot(idl, whom, k), noknot) when nonep !k ->
                                    match cl_fap with
                                        //| [] -> Cil_cr_idl idl
                                        | _ -> CT_cr { g_blank_crefs with name=idl; cl_actuals=map (lift_fap_values ww tcn "knot") cl_fap;  crtno=CT_knot(idl, whom, k); }
                                            
                                | No_knot(CT_knot(idl_, w, k), noknot) ->
                                    CT_cr { g_blank_crefs with name=idl; cl_actuals=map (lift_fap_values ww tcn "knot") cl_fap;  crtno= valOf !k; }
                                    
                                | other -> sf (sprintf "other No_item or other  in lookup %s: %A" (hptos idl) other)
                        | multiple -> sf (sprintf "multiple in lookup %s" (hptos idl))
                    no_to_type r


//------------------------------------------------
(*
 * Convert an IL class into the type of an hexp variable.
 * Constant literals, as found in enums, must be tallied to give the encoding range. 
 * 
 * We use a nominal layout sized using bytes to allocated a widx to each addressable item.
 * This might be strictly necessary for compiling cil code from C/C++ with genuine pointer
 * arithmetic.  
 *
 * We build up a symbol table for each structure or class consisting of a reversed linked
 * list of its contents. 
 *
 * If we want to make the symbol table only once and then reload it on instances then the generics
 * will be unbound. BUT THEN HOW DO WE ALLOCATE SPACE FOR VALUETYPES WE DONT KNOW SIZE OF ? ACCURATESIZES DO NOT MATTER FOR KIWI...  CANT DO DIS_VALUETYPE FOR TEMPLATES EITHER.
 *
 * For recursive types we use a Knot containing a ref field to generate an infinite structure.  Cant we just use a class name and gactuals?
 *
 * We pass in the real_rn/idl because the idl in the AST does not contain namespace prefix info.
 *
 *
 * Where type generic parameters have been passed in we need to augment the record name to reflect the specific rehydration - we squirrel the types to strings.
 *)


and cil_gen_dtable ww argpairs (real_rn, x) = // real_rn is not used. fap are bound in gb0 with any luck
    let cCC = valOf !g_CCobject
    let vd = 3
    let gtvd = -1
    let ww = WN "cil_gen_dtable" ww 
    let tycontrol =
        {
            grounded=true;
        }
    match (* follow_no_knot here please *) x with
    | No_knot(CT_knot(idl, whom, v), kno) ->
        if idl = [ "Object"; "System" ] then // These should never be made.
            if vd>=3 then vprintln 3 ("Please do not even make System.Object object knots!")
            g_canned_object // aka CTL_record(idl, g_canned_object_struct, [], g_object_overhead, [], [], None) // We should trap System.Object earlier and not try to make this dtable for it.
        else
        if not_nonep !kno then cil_gen_dtable ww argpairs (real_rn, valOf !kno)
        else
            vprintln 3 (whom + ": gen_dtable: knot was untied for dtable (mutual recursion): " + rnToStr real_rn)
            //CTL_void //(void, 0, 0, [])
            CT_knot(idl, whom, v)
            //sf ("gen_dtable: " + rnToStr real_rn + " big tied knot " +  noitemToStr false x + " length = " + i2s ll)

    | No_proxy "System-Array" ->  gec_CT_star 1 (CT_class g_array_boll)

    | No_class(srcfile, flags, idl, CTL_record (a,b,c,d,e,binds, f), items, prats) -> gec_CT_star 1 (CTL_record (a,b,c,d,e,binds,f))
    
    | No_class(srcfile, recflags, idl, CT_class cst, items, prats) ->
        let ats = prats
        let add_class_ref dt = if cst.structf then dt else gec_CT_star 1 dt
        if cst.name = [ "Object"; "System"; ] then
            //vprintln 3 ("Please do not even make System.Object objects! - comes from canned?")
            g_ctl_object_ref // add_class_ref CTL_object
        else
        let ids = hptos idl
        let alsoneeded = ref []
        //let ww = WF 3 "cil_gen_dtable" ww ((if cst.structf then "start on struct " else "start on class ") + noitemToStr false x)
        (* let _ = reportx 300 "cgr in" cgr1ToStr cgr0 *)
        let grounded = nullp argpairs
        let rec gen_record_tag ww dt =
            match dt with
                | CT_knot(idl, w, vv)  when not(nonep !vv) -> gen_record_tag ww (valOf !vv)
                | CT_knot(idl, w, vv)  when (nonep !vv)    -> dt
                | CT_cr cr   -> dt
                | CTVar x when nonep x.idx   ->
                    let rec scan n = function
                        | [] -> cleanexit(sprintf ": Unbound type formal %A" x)
                        | (id, ty)::tt when id=valOf x.id -> ty
                        | _ :: tt -> scan (n+1) tt
                    scan 0 argpairs

                | CT_class _ -> dt
                | dt when trivially_grounded_leaf dt -> dt                                
                | CT_star(_, _) ->
                    let _ = vprintln 3 ("+++gen_dtable: gen_record_tag star : " + typeinfoToStr dt)
                    dt
                | other ->
                    let _ = vprintln 0 ("+++gen_dtable: gen_record_tag other: " + typeinfoToStr dt)
                    dt
        let sqname = cil_typeToSquirrel (CT_class cst) argpairs
        let (idl, ids__) = (rn_valOf "gen_record_tag-b" real_rn, rnToStr real_rn)  // TODO tidy me
        let idsq = hptos sqname
        let (found, ov) = g_record_table.TryGetValue(idsq)  

        let unkf dt = // without generic bindings must simply allocate 8 bytes
            match dt with
                | CTVar _  -> true
                | other    ->false

        if found then
            if gtvd>=4 then vprintln 4 ("Found already with idsq=" + idsq)
            f1o4 ov
        else
            let ww = WF 3 "cil_gen_dtable" ww (sprintf "start table pairs=%i for real_rn=%A" (length argpairs) real_rn) 
            let ww = WF 3 "cil_gen_dtable" ww ("start for " + ids + " idsq=" + idsq)
            let dtable_gb =
                  let finbind (mm:annotation_map_t) = function
                      | (f, dt) ->
                          let _ = vprintln 3 (sprintf " cil_gen_dtable: bind actual to class %s f=%s a=%s" ids (f) (cil_typeToStr dt))
                          mm.Add([f], RN_monoty(dt, []))

                  List.fold finbind Map.empty argpairs

            let dtable_eval ty =
                let ans = cil_type_n (WN "template_eval for dtable" ww) (tycontrol, dtable_gb) ty
                vprintln 3 (sprintf "dtable_eval ans is %s"  (typeinfoToStr ans))
                ans
            // Need to add a holder in here, in case the type is recursive. -- really? - it will recurse by name through a CT_cr form now ...
            //vprintln 0 ("Insert dtable holder for " + ids)
            let knot = ref None
            let placeholder = CT_knot(idl, "KL1709", knot)
            g_record_table.Add(idsq, (placeholder, 0L, 0, [])) // since always grounded can use a global store.
            let (inherited, inherited_noms) =
                let rec upper_class_items height (cc, cd) = function
                    | None -> (cc, cd)
                    | Some parent ->
                        let (rn, additional_fap) = cr2idl ww parent
                        if not (nullp additional_fap) then muddy "parent class additional fap TODO!"
                        let idl = rn_valOf "dtable inherited" rn
                        let ids' = hptos idl
                        let ww'  = WF 2 "cil_gen_dtable" ww (sprintf "recurse on parent class '%s'" ids' )
                        let oc = id_lookupp ww' idl Map.empty (fun()->(hptos idl + ": parent class not defined (L1588): cr->" + ids', 1))

                        let rec class_valOf aA =
                            match aA with
                                | Some(No_knot(CT_knot(idl, w, v), kno)) when not(nonep !kno) -> class_valOf !kno
                                | Some(No_class(srcfile, _, _, CT_class cst_upper, items, prats)) -> (valOf aA, cst_upper.parent)
                                | Some(No_class(srcfile, _, _, _, items, prats)) -> (valOf aA, None)
                                | Some other -> sf ("parent class not defined or knot untied: class_valof other: " + noitemToStr false other)
                                | None -> sf("class_valof None for " + ids')
                        let (inherited_nom, grandparent) = class_valOf oc
                        let (cc, cd) = upper_class_items height (cc, cd) grandparent
                        let cat = cil_gen_dtable ww' argpairs (rn, inherited_nom) // recurse on parent class - TODO will it have some of the same gb generic binds? TEST please.
                        let rec record_valOf = function
                             | CT_star(ns, arg) -> record_valOf arg
                             | CTL_record(_, cst_, items, fap_, _, binds, _) -> items                         
                             | other -> sf("record valOf other in parent class elab: " + typeinfoToStr other)
                        (record_valOf cat @ cc, (idl, inherited_nom)::cd)
                upper_class_items 1 ([], []) cst.parent 
            if gtvd>=4 then vprintln gtvd  ""
            let ww = WF 3 "cil_gen_dtable" ww ("finished inherited parents for " + ids)
            if gtvd>=4 then vprintln 4 (whereAmI_concise ww)
            let m_pack = ref 1
            
            let m_total = ref(int64 (if cst.structf then 0 else g_object_overhead)) // Add overhead here instead of inheriting object properly.
            let m_enums = ref []

            let (m_dtable, m_e_no) = (ref [], ref 0)
            let append_dtable_entry(bytes, ptag, utag, dt, fap, fld_st) =
                  // the fap arg is the further dropping now ? so not needed here?
                let no = !m_e_no
                mutinc m_e_no 1
                (mutadd m_dtable (bytes, ptag, utag, !m_total, dt, fap, no, fld_st); mutinc64 m_total bytes; ())

            // We put the vtable field in the top-of-class and the children inherit this field but initialise it differently. Structs no longer not have overheads of vtable and lock field under KiwiC. So virtual method dispatch on structs is not supported.
            if cCC.settings.dynpoly && not_nonep cst.vtno && nonep cst.parent && not cst.structf then append_dtable_entry(4L, RN_idl ["_kv_table"], "_kv_table", g_canned_i32, [], None)
                  
            let insert_inherited (crt:concrete_recfield_t) (*rn, dt, base_, bytes, fap*) = append_dtable_entry(int64 crt.size_in_bytes, RN_idl [crt.ptag], crt.utag, crt.dt, crt.fap__, None)

            app insert_inherited inherited

            let att = function
                  | Cil_field_lit(Cil_number32 v)   -> Some(int64 v)
                  | Cil_field_lit(Cil_int_i(_, bn)) -> Some(int64 bn)
                  | other ->
                      vprintln 3 (sprintf "other enum manual codepoint at %A" other)
                      None

              
            let kk_rec_fields argA = // re_de a record item: casesplit.
                match argA with
                | No_esc(CIL_pack(Cil_number32 v)) -> m_pack := v

                | No_class(srcfile, flags, idl, dt, items_, prats) -> () // Presence of a nested class makes no difference to layout of this current class.

                | No_field(layout, flags, name, fld_st, atclause, cust_ats) ->
                    let fld_st_ct = dtable_eval fld_st.ct
                    if memberp Cilflag_literal flags then // or Cil_at_enum?
                        m_pack := -1 (* not an array ? why not ? *);
                        mutadd m_enums (name, att atclause, (*spare*)-1);
                    else
                        let (idl, ids, utag) = ([name], name, fld_st.utag)
                        if gtvd>=4 then vprintln 4 ("gen_dtable field " + ids)// + " fap=" + sfold typeinfoToStr fld_st.fap)
                        let vt = not(unkf fld_st_ct) && ct_is_valuetype fld_st_ct
                        let is_array = function
                              | (CT_arr _) -> true
                              | other -> 
                                  if vd>=4 then vprintln 4 ("This class is seemingly not an array " + typeinfoToStr other)
                                  false

                        let is_ref = function
                              | CT_cr _                  -> true
                              | CT_star(n, _) when n > 0 -> true
                              | other -> 
                                  //if vd>=4 then vprintln 4 ("This class is seemingly not an ref " + typeinfoToStr other)
                                  false

                        let isa = is_array fld_st_ct

                          // TODO this code is replicated and does not check elaborate flag !
                        let dovol = (if fld_st.volf then add_vol_flag_1 else fun x->x)
                        let ww'  = WF 3 "gen_dtable" ww ("gen_dtable of field: " + noitemToStr false argA + " ids='" + ids + "'  vt=" + boolToStr vt + " is_array=" + boolToStr isa + sprintf " volf=%A" fld_st.volf) //  + ", fap items=" + i2s(length fap))
                        // Cannot naively always call type_n on ctype owing to recursive types.
                        if is_ref fld_st_ct then// Instance of a class: has pointer size
                           if fst(g_record_table.TryGetValue idsq) = false then mutadd alsoneeded fld_st_ct
                           append_dtable_entry(g_pointer_size, RN_idl idl, utag, (gen_record_tag ww fld_st_ct), [], Some fld_st)
                        elif vt then
                            let dt = dovol fld_st_ct
                            let frad:int64 = dt_width_bits ww dt
                            let bytes = (frad + 7L) / 8L
                            if gtvd>=4 then vprintln 4 ("   " + i2s64 bytes + " byte valuetype")
                            (* Can always call type_n on leaf valuetypes *)
                            append_dtable_entry(bytes, RN_idl idl, utag, dt, [], Some fld_st)
                        elif unkf fld_st_ct then
                            // Unknown - allocate 8 since that's the largest valuetype
                            append_dtable_entry(8L, RN_idl idl, utag, (gen_record_tag ww fld_st_ct), [], Some fld_st)

                        elif not isa then// Instance of a class: has pointer size
                           if fst(g_record_table.TryGetValue idsq) = false then mutadd alsoneeded fld_st_ct
                           append_dtable_entry(g_pointer_size, RN_idl idl, utag, (gen_record_tag ww fld_st_ct), [], Some fld_st)
                        else 
                            if gtvd>=4 then vprintln 4 ("Find out more about array " + typeinfoToStr fld_st_ct + " before adding to dtable")
                            let dtable_ct = function
                                     | CT_arr(CTVar vv, _)  -> (true, 1, (CTVar vv), None)
                                     | CT_arr(ct, d) -> (false, 1, ct, Some d)
                                     | other -> sf("dtable_ct other: " + typeinfoToStr other)
     
                            let detailed_ct = 
                                    if vt then sf "still a valuetype!"
                                    else
                                        let (pramd, n, ct, d) = dtable_ct fld_st_ct
                                        let vt1() = not(unkf fld_st_ct) && ct_is_valuetype ct
                                        //vprintln 3 "ZZ0"
                                        let a2d = control_get_s "kiwi-fe" cCC.control_options "array-2d-name" None
                                        //dev_println ("a2d=" + a2d + " DO WE NEED TO ADD AUTOREF TO CT HERE ? ")
                                        if n=2 then (muddy "gen_ub_cref_deprec") "L1700" ("ARRAY2D", ["ARRAY2D"], muddy "2d", muddy "2d")  // THIS CODE IS MUDDY!
                                        elif pramd then CT_arr(ct, valOf_or_fail "L1923" d)
                                        elif vt1() then CT_arr(ct, valOf_or_fail "L1924" d)
                                        else
                                               //let (rn, fap) = cr2idl gbo cr   
                                               //let (idl, ids) = (rn_valOf "bobob", rnToStr rn)
                                               muddy "... call ctype'? splitter"
                                               //let ids = hptos idl
                                               //let clsa = class_find ww' idl ids
                                               //let _ = vprint(1, "ZZ0 end\n")
                                               //in dovol  clsa

                            // end : IF BRANDING ENABLED
                            // Branding, if enabled, enables the repacker to further disambiguate memory regions.
                            //let detailed_ct = CT_brand(CV_brand(funique "B"), detailed_ct)

                            let gb1() =
                                let frad = dt_width_bits ww detailed_ct
                                let bytes = (frad + 7L) / 8L
                                bytes
                            let bytes = if vt then gb1() else g_pointer_size
                            if gtvd>=4 then vprintln 4 ("   " + i2s64 bytes + " byte")
                            append_dtable_entry(bytes, RN_idl idl, utag, detailed_ct, [], Some fld_st)

                | No_esc(CIL_size(Cil_number32 v)) -> append_dtable_entry(int64 v, RN_idl [], "", CT_arr(CTL_void, Some(int64 v)), [], None)

                | No_esc(CIL_custom(s, t, fr, args, vale)) -> ()
                | No_esc(CIL_setget(opcode, ck, flags, tt, fr, signat)) -> ()

                | No_method(srcfile, flags0, ck, (idl, uid, disamname), mdt, flags1, instructions, atts) -> // A method does not increase the record length, but we make notes of overrides
                    let scan_up idl2  = function
                        | No_method(srcfile2, flags02, ck2, (gid2, uid2, disamname2), mdt2, flags12, instructions2, atts2) ->
                              if disamname2 = disamname then
                                  let _ = vprintln 3 (sprintf "found override for %s %s with parent=%s child=%s" (hptos idl) disamname (hptos idl2) ids)
                                  let _ = g_method_downcasts.add (disamname) (idl, idl2)
                                  ()
                        | _ -> ()

                    let scan_parents = function
                        | (idl2, No_class(srcfile, flags, idl, _, items, prats)) ->
                            app (scan_up idl2) items
                        | other -> sf (sprintf "other form inherited nomo %A" other)
                    app scan_parents inherited_noms 


                //| CIL_property _ -> ()  (* A property does not have a String.length: it just defines two methods. !*)       
                | other -> sf("cil itemsize kk_rec_fields other: " + noitemToStr false other)

            let kk1 u = (  if gtvd>=4 then vprintln 4 ("  dtable layout: " + noitemToStr false u + " at offset " + i2s64(!m_total));
                           kk_rec_fields u
                        )
            app kk1 items
            if gtvd>=4 then vprintln 4 (sprintf "cil itemsize for %s " ids + " : total*pack = " + i2s64(!m_total) + " * " + i2s !m_pack + "  enums=" + i2s(length !m_enums))

            let de_nz = function
                  | (s, RN_idl[ptag], utag, pos, t, fap, no, fld_st_o) -> { ptag=ptag; utag=utag; dt=t; pos=pos; size_in_bytes=s; fap__=fap; no=no; fld_st_o=fld_st_o }
                  | _ -> sf ("de_nz")
            let final_dt =
                  match !m_enums with
                      | [] ->
                          let size_bytes = !m_total
                          if gtvd>=4 then vprintln 4 (sprintf "Record/class instance %s made: non-enum.  Size=%i bytes" (hptos sqname) size_bytes)
                          add_class_ref (CTL_record(sqname, cst, map de_nz (rev !m_dtable), !m_total, ats, argpairs, None))

                      | lst when memberp Cilflag_enum recflags-> // Could also check whether extends System.Enum.
                          if gtvd>=4 then vprintln 4 (hptos idl + sprintf "flags for this enum list were %A" recflags)
                          gec_enum_type ww idl (rev lst) 

                      | lst__ ->
                          let size_bytes = !m_total
                          if gtvd>=4 then vprintln 4 (sprintf "Record/class instance %s made: Size %i bytes" (hptos sqname) size_bytes)
                          add_class_ref (CTL_record(sqname, cst, map de_nz (rev !m_dtable), size_bytes, ats, argpairs, None))
            knot := Some final_dt // Cannot have recursive structs - only recursive classes, but tie it anyway.
            let dtable_filename =
                if !m_total > 0L then
                    let ids2 = if grounded then ids else idsq
                    let fn = path_combine(!g_log_dir, mysanitize [] ids2 + ".dtable")
                    let outcos = yout_open_out fn
                    youtln outcos (sprintf "unique tag idsq=%s  structf=%A" idsq cst.structf)
                    youtln outcos ("parent(s) inherited from are = " + sfold (fst>>hptos) inherited_noms)
                    youtln outcos ("attributes are " + sfold (fun (a,b) -> sprintf "  %s=%s" a b) ats)                                        
                    printDtable outcos "gen_dtable record format adopted:" final_dt
                    yout_close outcos
                    fn
                else "no-output-file"

            if idsq = "System.ValueType" && not cst.structf then sf "System.ValueType should be have structf flag holding."
            let ww = WF 3 "cil_gen_dtable" ww (sprintf "Finished generating dtable for real_rn=%A structf=%A. Written to %s." real_rn cst.structf dtable_filename) 
            //if dtable_filename = "obj/WideWordDemoUsingStructs_widenet_1_16_SS_T1.dtable" then sf "TRAP TO EXAMS"
            let ans = (final_dt, !m_total, !m_pack, !m_enums)
            let _ =
                  // shortcut the place holder - it would be better for testing missing cases if this step were not done.
                  let _ = g_record_table.Remove(idsq)
                  let _ = g_record_table.Add(idsq, ans)
                  ()
            if vd>=4 then vprintln 4 (sprintf "Dtype for %s was " ids + typeinfoToStr final_dt)
            let ww = WF 3 "gen_dtable" ww (sprintf "Complete dtype table saved for %s. Others also needed=%i" ids (length(!alsoneeded)))

            let do_other = function
                  | dt when dt=g_ctl_object_ref -> ()
                  | dt ->
                      let _ = vprintln 3 ("do other class ignored for now: " + typeinfoToStr dt)
                      ()
                      
            app do_other (!alsoneeded)
            let ww = WF 3 "gen_dtable" ww ("any others now done")
            final_dt 

    | other -> sf("cil gend_table (itemsize) other: " + noitemToStr false other)


//--------------------------------------------
// Size functions.  
//
// These are leaf functions, calling on an abstract type is an error as the leaf function cannot sort this out itself.
//
// de = dereference number: when 0 we return the size of the arg in bits
//                          when <> 0 we return the pointer size, specifically we do not autoderef references.
//
// ct_bitstar returns the number bit needed to store a type.
//    
and ct_bitstar ww msg de arg =
    
    match arg with
    | CT_knot(idl, whom, v) when not(nonep !v) -> ct_bitstar ww msg de (valOf !v)
    //| CT_knot _ -> when untied

    | CT_star(n, dt) -> ct_bitstar ww msg (n+de) dt
    | _ when de <> 0 ->  (g_pointer_size * 8L)


    | CT_class _
    | CT_cr _ when de=0 && dt_is_enum arg ->
        let tycontrol =
                {
                    grounded=true
                }
        let tcn = (tycontrol, Map.empty)
        let dt = cil_type_n ww tcn arg
        vprintln 3 (sprintf "ct_bitstar: converted enum to net for width measurement: %s -> %s " (cil_typeToStr arg) (cil_typeToStr dt))
        ct_bitstar ww msg de dt


    | CT_class _
    | CT_cr _ ->
        let ww = WN "ct_bitstar" ww
        if de=0 && is_structsite msg arg then
            let tycontrol =
                {
                    grounded=true
                }
            let tcn = (tycontrol, Map.empty)
            let dt = cil_type_n ww tcn arg
            dev_println (msg + sprintf " ct_bitstar on ungrounded/un-normed class %s" (cil_typeToStr arg))
            match dt with
                | CTL_record(idl, cst, lst, len, ats, binds, _) -> ct_bitstar ww msg de dt // Now grounded, go round again.
                | other ->
                    sf (msg + sprintf " ungrounded struct  size neededn %s" (cil_typeToStr other))
        else (g_pointer_size * 8L)

        
    | CTL_net(volf, w, signedf, _) when de=0 -> int64 w

    | CTL_reflection_handle _          -> (g_pointer_size * 8L)
 
    // gcc4cil puts string and perhaps other constant literals as labelled blobs in static data segment.
    // Static blobs of data, such as initialisation values for arrays and string literals generated by gcc4cil are labelled.  We use &(Var(...)) for labels.  A label has no size (or perhaps IntPtr size) but the blob it lables does have a size. 
    // We encounter de=-1 for a lable, denoted as &Var - the address of a variable.

    | CT_arr(ct, Some len) when de <= 0 ->
        len * ct_bitstar ww msg 0 ct
        //| CT_arr(ct, None) -> sf "ct_bitsar -ve de and no length"

    | CT_cr _ when de > 0 -> (g_pointer_size * 8L) // surely never matched?

#if DDD
    | CT_cr crt ->
        let ans1= ct_bitstar ww msg de (f1o3(valOf(!lzy)))
        let (found, ov) = g_record_table.TryGetValue(ids)//is this unique over all deabstractions?
        let _ = cassert(found, "ct_bitstar: ct_bound_cref: not in typeinfo table: " + ids)
        let ans2 = 8 * f2o4 ov
        let _ =
            if ans1 <> ans2 then vprintln 0 (msg + " +++ " + sprintf "ct_bitstar ww size in bytes : ans1=%A ans2=%A for %A" ans1 ans2 ids)
        ans1
#endif

     //| (CT_valuetype(CTL_record(ttt, cst, crt_lst, len, ats, binds, _))
     | CTL_record(ttt, cst, crt_lst, len, ats, binds, _) when de <= 0 ->
        List.fold (fun c (rdt:concrete_recfield_t) -> c + int64(rdt.size_in_bytes) * 8L) 0L crt_lst
  
    | n -> sf(msg + sprintf ": ct_bitstar other form de=%i,   dt=" de + typeinfoToStr n)


//
// Return data type size in bytes.
//
and ct_bytes1 ww msg z =
    let wb = ct_bitstar ww msg 0 z
    //let _ = vprintln 0 (sprintf "ct_bytes1: %s has size %i bits" (cil_typeToStr z) wb)
    (int64 wb + 7L)/8L  // Rounds up - alignment from bits to complete bytes is required.

and dt_width_bits ww dt = ct_bitstar ww "dt_width_bits" 0 dt

let get_prec_width ww dt =
    let signed = ct_signed ww dt
    let width = dt_width_bits ww dt
    let width = int width
    ({ g_default_prec with  widtho=Some width; signed=signed }, width)



//
// This is a poor-man's routine - need to fix for the paper.
//
let rec ct_is_volatile ww arg =
    match arg with
    | CT_knot(idl, whom, vv) when not(nonep !vv) -> ct_is_volatile ww (valOf !vv)
    | CT_knot(idl, whom, vv) when (nonep !vv)    -> true // Conservative temp result
    //| CT_valuetype ct -> ct_is_volatile ww ct
    //| CT_brand(_, ct) -> ct_is_volatile ww ct
    | CT_star _ -> false // All address takes are non-volatile!  TODO <------- yuck, like the whole volatile issue.

    | CT_arr(contents_dt, _) -> ct_is_volatile ww contents_dt // SEMANTICS CLEAN YUCK TODO
    
    | CTL_net(volf, width, signed, ats) -> volf



    | CT_cr _
    | CT_class _
    | CTL_record _ ->
        // let volf = assoc lookup in flags op_assoc g_volat flags = Some "true"
        //let _ = vprintln 3 "record is perhaps volatile? %A " volf
        false   (* could be volatile I suppose? check ats? *)

    | CTL_reflection_handle _ -> false
    | dt when dt=g_ctl_object_ref -> false

    | other ->
        vprintln 0 ("+++ct_is_volatile other " + typeinfoToStr other)
        false

let rec is_volatile ww = function
    | CE_x _ -> false // Of course, this could be a shared variable and hence change.
    | CE_region(_, dt, ats, _, _)     -> ct_is_volatile ww dt
    | CE_var(vrn, dt, idl, fdto, ats) -> ct_is_volatile ww dt
    | CE_subsc(_, l, r, _)  -> is_volatile ww l || is_volatile ww r
    | CE_conv(_, _, r)      -> is_volatile ww r
    | CE_dot(_, l, _, aid)  -> is_volatile ww l
    | CE_star(s, e, aid)    -> if s=0 then is_volatile ww e else false

    | CE_struct(_, _, _, items) -> // Use disjunctionate its members
        let siv = function
            | (_, _, Some xv) -> is_volatile ww xv
            | _ -> false
        disjunctionate siv items
    | CE_reflection _       -> false
    | other                 -> (vprintln 0 ("+++ ce is_volatile other " + ceToStr other); false)


let rec ct2idl ww v =
    //vprintln 3 ("ct2idl input=" + typeinfoToStr v)
    let ans = cil_typeToSquirrel v []
    //let ans = ct2idl_lst ww 0 v
    vprintln 3 (sprintf "ct2idl %s ans= %s" (cil_typeToStr v) (hptos ans))
    ans

// At end end of static elaboration, heap allocs must be done by the run-time allocator, not the obj_alloc symbolic heap allocator.
// This flags specifies which to use.    
type post_static_elab_t =
    | PSE_pre   // use compile-time symbolic alloc
    | PSE_post  // use run-time malloc



//
// Retrieve the cgr bindings from a classref etc..
//
let rec dt_class_gens_ ww dt =
    match dt with
    //| CT_valuetype(dt)     | CT_brand(_, dt)
    | CT_arr(dt, _)  // CORRECT ? THERE IS ONE  IN THE ARRAY!

    | CT_star(_, dt) -> dt_class_gens_ ww dt    

    | CTL_record _ 
    | CTL_net _    
    | CTL_void        -> []
   

    | CT_class cdt -> [] //  Wrong - could do something with this cdt.formals - but better TODO delete this routine!
    | dt -> sf("dt_class_gens other: " + typeinfoToStr dt)

    
// Most of the following description is now wrong:
// Initially an element is given an nemtok and a tag.  Where an element is instatiated multiple times it will always have the
// same nemtok but the tags are unique. A tag is a (function of?) a uid.
// Nemtoks are assigned unique target classes but these get conglomorated in equivalence classes both here and during later repack phases.
// We also once kept a nemtok class reference id, but these are/were just for debugging.
let record_memdesc_item nemtok msg_or_site hintname_o ats (uid:vimfo_t) ce__ dt =
    let cCC = valOf !g_CCobject
    let ats = if nonep hintname_o then ats else ("hintname", valOf hintname_o)::ats
    let nr:proto_memdesc_record_t =
            {
                //p_region_flag=  true //flag never needed?
                p_labelled_literal = false // always so ... for now
                p_aid=          A_loaf nemtok
                p_nemtok=       nemtok
                p_idl=          uid.f_name // unclean
                p_uid=          uid
                mats=map (fun (a,b) -> Nap(a,b)) ats
            }
    //vprintln 3 (sprintf "Record_memdesc_item: hintname_o=%A regf=%A msg=%s nemtok=%s  vuid=%A" hintname_o regf msg_or_site nemtok (* mdToStr nc *) (vuid))
    cCC.heap.memdescs.add nemtok nr
    nr


    
let is_teend = function
    | CEE_tend _ -> true
    | _ -> false


//
// Main env is held as a tree functional array of disjoint byte-addressed memory regions .
// Walk an env tree, looking for an entry at address m.
//
let rec ce_tree_walk1 bb = function
    | CEE_tend _ -> None
    | CEE_tree(l, idx, vv, r) -> 
        //vprintln // 0 ("Tree assoc walk compare: " + i2s64 m + " cf " + i2s64 n)
        if idx = bb then Some vv
        elif bb < idx then ce_tree_walk1 bb l
        else ce_tree_walk1 bb r

    //| other -> sf("ce_tree_walk1 other:" + ceTreeToStr 3 other + ", idx=" + i2s64 bb)

// binary region tree: Find a nemtok region of the env
// let rec env_peek env bb = ce_tree_walk1 bb env.mainenv


let vector_env_evolve env tok (vvl, vale) = 
    let rec treei = function
        | CEE_tend x -> CEE_tree(CEE_tend x, vvl, vale, CEE_tend x)
        | CEE_tree(l, idx, ov, r) -> 
           if   vvl < idx then CEE_tree(treei l, idx, ov, r)
           elif vvl > idx then CEE_tree(l, idx, ov, treei r)           
           else CEE_tree(l, idx,  vale, r)

    //let _ = vprintln 3 (sprintf "treei:  [%i:%i]" vvl vvh + " := " + ceTreeToStr 3 vale)
    let nv = treei env
    //let _ = vprintln 0  (sprintf "vector_env_evolve: Tree post evolve: %s new value=" tok + ceTreeToStr 3 nv)
    nv


// Death concept: storing Zombie marks an entry as not-a-constant and it stays in the env as lattice bottom for constant meet.
// Store with a varindex will make the whole array as bottom.    
let env_insert_vector (e00:env_t) tok idx = function
        | CEE_zombie ss ->  // bottom of lattice - death - denotes not a constant.
            let vale = CEE_zombie ss
            { e00 with mainenv=e00.mainenv.Add(tok, vale)  }

        | vale -> 
            let ov = e00.mainenv.TryFind tok
            match ov with
                | Some(CEE_zombie _) -> e00 // This array has been fully deferred to runtime
                | _ ->
                    let ex =
                        match ov with
                            | None                    -> CEE_tend "genesis"
                            | Some(CEE_vector(_,  v)) -> v
                            | other -> sf (sprintf "other form env_insert_vector %A" other)
                    { e00 with mainenv=e00.mainenv.Add(tok, gec_cee_vector(vector_env_evolve ex tok (idx, vale))) }

let env_insert_scalar (e00:env_t) arg =
    match arg with
        | (tok, _, CEE_zombie ss) ->  // death - mark not a constant
            let vale = CEE_zombie ss
            { e00 with mainenv=e00.mainenv.Add(tok, vale)  }

        | (tok, _, vale) -> 
            { e00 with mainenv=e00.mainenv.Add(tok, vale) }





// Basic algorithm: Two heaps are identical if the blocks in use match. Free'd blocks take no part in the comparison.
// Enhanced algorithm: two heaps are reported as comparing ok even when the blocks in use differ, but the discrepancies are instead reported via advanced_ptr.
// 
let heapspace_compare advanced_ptr msg = function
    | (CEE_heapspace(plan, inuse, discreps__), CEE_heapspace(_, inuse', _)) ->
        let vd = 3
        match advanced_ptr with
            | Some m_discrep when !g_kiwi_autodispose_enabled ->
                let m_commons = ref []
                let discreps = 
                    let rec discrep_rally = function
                        | ([], []) -> []
                        | (x, []) -> x
                        | ([], y) -> y
                        | (x::xs, y::ys) when x=y -> (mutadd m_commons x; discrep_rally(xs, ys))
                        | (x::xs, y::ys) when x<y -> x::discrep_rally(xs, y::ys)
                        | (x::xs, y::ys) when x>y -> y::discrep_rally(x::xs, ys)                        
                    discrep_rally(inuse, inuse')
                if not_nullp discreps then m_discrep := Some(CEE_heapspace(plan, rev !m_commons, discreps))
                let ans = true
                vprintln 3 (sprintf "heaper: autodispose %s heapspace_compare: (%i,^%i)   heap shapes: %s cf %s ---> %A" msg (fst !plan) (length (snd !plan)) (sfold i2s64 inuse) (sfold i2s64 inuse') ans)
                ans
            | _ ->
                let basic_ans = inuse = inuse'
                if vd>=5 then vprintln 5 (sprintf "heaper: basic %s heapspace_compare: (%i,^%i)   heap shapes: %s cf %s ---> %A" msg (fst !plan) (length (snd !plan)) (sfold i2s64 inuse) (sfold i2s64 inuse') basic_ans)
                basic_ans

    
let obj_dispose ww env item =
    let freeblock =
        match item with
            | CE_region(uid, dt, ats, nemtok, _) -> (uid.baser, uid.length)
            | other -> sf ("other form dispose " + ceToStr other)

    if nonep(snd freeblock) then env
    else
        let (b, len) = (fst freeblock, valOf(snd freeblock))
        let _ = vprintln 3 (sprintf "obj_dispose: block=%i length=%i" b len)
        let ov = env.hs1
        //let _ = vprintln 3 ("obj_dispose: Heap pointer was "  + ceTreeToStr 1 ov)
        let hidx =
            match ov with
                //| CEE_tend msg ->
                //    let _ = vprintln 3 (sprintf "heaper: dispose ignored owing to CEE_tend(%s)" msg)
                //    ov
                | CEE_heapspace(allocating_info, inuse, discreps) ->
                    let rec disposer = function
                        | [] ->
                            vprintln 0 ("KiwiC: +++ Ignored attempt to dispose a block that was not ever allocated " + ceToStr item)
                            []
                        | blk::blks when blk=b ->
                            //if not blk.inuse then (*yikes*)vprintln 0 ("KiwiC: +++ Ignored attempt to dispose a block that was not in use " + ceToStr item)
                            blks
                        | other::blks -> other :: disposer blks
                    //let _ = vprintln 0 (sprintf "disposer invoked while heap has %i items" (length inuse))
                    //let _ = vprintln 0 (sprintf "disposer invoked while heap is %s" (sfoldcr i2s64 inuse))                    
                    let nv = disposer inuse
                    vprintln 3 (sprintf "heaper: obj_dispose %i giving %s in use" b (sfold i2s64 nv))
                    CEE_heapspace(allocating_info, nv, discreps)
                //| other -> sf (sprintf "obj_dispose: other form heap record %A" other)
        { env with hs1=hidx }


// Note that a memory region is offchip.
// Which memory bank may be specified by C# attribute mark up or dynamically load balanced (by restructure?).
let record_offchip_aid m0 nemtok bankinfo =
    // Each callsite to 'new()' should define a fresh nemtok.  These will later coalesc into some number of disjoint dynamic heap spaces.
    // Alternatively, the user can manually specify a heapspace with some markup of the 'new()' call but C# attributes for that are not yet defined ...?
    // Equally, a class or struct type can have a preferred space attribute, which will given about the same level of control in general - a user can extend a class just to give it a preferred memory space attribute...
    vprintln 3 (sprintf "Noting that nemtok %s is offchip,  bankinfo=%A" nemtok  bankinfo)
    let cCC = valOf !g_CCobject
    let sci = cCC.kcode_globals.ksc.classOf_i "record_offchip_aid" (A_loaf nemtok)
    cCC.kcode_globals.ksc.attribute_set sci "bank" bankinfo
    ()



// Three allocation styles: staticmd forms: 0=static, Kiwi 1 dynamic that becomes static during elaboration, and Kiwi 2 dynamic that uses a run-time allocator.
// We do not reallocate memory for different types or regions since this confuses repack, so we save all dealloc information, but the constant_meets needs to ignore that information in comparisons.
// Which of several simiar-typed memory regions to realloc will affect wiring length and mux complexity in the final hardware, but a ...

let g_zal_experiment = false // This zal feature was highly temporary code, now turned off again.
let g_zal_temp_savings = ref []
//
// The really flag is only deasserted for static field allocs.   
// obj_alloc_core is called both by kcode interpeter and by eint intcil interpeter. But the intcil calls should only be for singletons that use sheap not hs1
// We preferably need to record hintname in the allocated object since this enables repack to generate more comprehensible net names.
let rec obj_alloc_core m0 types reallyf storemode aido env aspace hintname bytes = // If flag reallyf does not hold, then return base answer but do not bump sbrk etc..
    let vd = -1 // off
    let rec insert bb = function // Insert in ascending list.
        | [] -> [bb]
        | h::t when h < bb -> h :: (insert bb t)
        | other -> bb :: other

    let heapspace_manage size0 = function // This manages the "static heapspace" which sounds like an oxymoron, but refers to KiwiC converting dynamic C# allocations into static allocations.  This contrasts with the use of hpr_alloc at run time.
        | CEE_heapspace(allocating_info, inuse, discrepancies) ->
            let (virgin_limit, block_policy) = !allocating_info
            let size = heap_alloc_roundup size0
            let aid:new_nemtok_t = valOf_or_fail "no aid in obj_alloc_core" aido
            let rec find_existing_block cc = function
                | [] -> (cc, None)
                | blk::tt when blk.aid=aid && blk.len=size && not (memberp blk.baser inuse) -> (cc, Some blk)
                | blk::tt -> find_existing_block (blk::cc) tt
            match find_existing_block [] block_policy with
                | (no_, Some blk)   ->
                    if vd>=5 then vprintln 5 (sprintf "heapspace_manage: Found suitable old block to reuse at %i" blk.baser)
                    ((blk.baser, blk.len), CEE_heapspace(allocating_info, insert blk.baser inuse, list_take blk.baser discrepancies))
                | (no_, None)       ->
                    if vd>=5 then vprintln 5 (sprintf "heapspace_manage: No suitable old block - allocate fresh store at %i for nemtok=%s hintname=%A size=%i (orig size=%i)" virgin_limit aid hintname size size0)
                    let nb = { baser= virgin_limit; len=size; types=types; aid=aid }
                    if g_zal_experiment then dev_println (sprintf "test39 Need to save heapspace zal %s -> %A" aid nb.baser)
                    if g_zal_experiment then mutadd g_zal_temp_savings (aid, nb)
                    allocating_info := (virgin_limit + size, nb :: block_policy)
                    ((nb.baser, nb.len), CEE_heapspace(allocating_info, insert nb.baser inuse, discrepancies))

    let wrap64 n = CE_x(g_canned_i64, xi_num64 n) 
    let cCC = valOf !g_CCobject
    let (hidxo, ce, nv, ov) =
        match storemode with
            | STOREMODE_singleton_scalar ->
                    // The old way, with sbrk in g_CCobject, is still used for statics.
                    let hidx = !cCC.heap.scalar_singleton_sbrk
                    if reallyf then cCC.heap.scalar_singleton_sbrk := !cCC.heap.scalar_singleton_sbrk + heap_alloc_roundup bytes // nasty muting
                    (Some hidx, wrap64 hidx, None, None) // and functional - hmm which is really used?
            | STOREMODE_singleton_vector ->
                    let hidx = !cCC.heap.vector_singleton_sbrk
                    let _ = if reallyf then cCC.heap.vector_singleton_sbrk := !cCC.heap.vector_singleton_sbrk + heap_alloc_roundup bytes // nasty muting
                    (Some hidx, wrap64 hidx, None, None) // and functional - hmm which is really used?

            | STOREMODE_compiletime_heap
            | STOREMODE_runtime_heap  ->
                let a0 = 
                  match aido with
                    | Some nemtok ->
                        match cCC.constant_or_volatile.lookup nemtok with
                            | Some(readers, VOC_monoassign site)  ->
                                //dev_println (sprintf "constvol: lookup nemtok=%s %A " nemtok site)
                                match op_assoc nemtok !g_zal_temp_savings with
                                    | Some ov when g_zal_experiment -> // Zal disabled
                                        dev_println (sprintf "heap tempory kludge zal recalled %A" ov.baser)
                                        Some(Some ov.baser, wrap64 ov.baser, None, None)
                                    | _ -> None
                            | None ->
                                dev_println (sprintf "no zal info for nemtok %A" nemtok)
                                None

                    | None ->
                        dev_println (sprintf "zal: no nemtok present")
                        None

                if not_nonep a0 then valOf a0
                else
                if not_nonep env then
                       //let tag = g_heap_brk
                       let ov = (valOf env).hs1
                       vprintln 3 (sprintf "obj_alloc_core for %s: reallyf=%A Heap pointer was %s" types reallyf (ce_hs_envToStr 2 ov))
                       let (hidxo, ce, hwm) =
                           match ov with
                               //| CEE_i64 n -> sf "old form heap (Some n, wrap64 n)"
                               | CEE_heapspace _ when storemode = STOREMODE_compiletime_heap ->
                                   let ((b, l), nv) = heapspace_manage bytes ov
                                   (Some b, wrap64 b, nv)
                               | _ when (*storemode = STOREMODE_runtime_heap && *) cCC.allow_hpr_alloc ->
                                   // KiwiC 2/staticmd 2 allows alloc at run-time.
                                   let aid:new_nemtok_t = valOf_or_fail "no aid for hpr_alloc (run-time heap) in obj_alloc_core" aido
                                   record_offchip_aid m0 aid g_undecided_bankinfo
                                   vprintln 3 (sprintf "heaper: use storemode=2  hpr_alloc run-time allocator for object %s %i bytes aid=%s" (hptos hintname) bytes aid)

                                   // TODO - throw this runtime abend on zero return: \item Abend code {\bf 0x91 --- Abend on Heap Memory Exceeded }
                                   (None, CE_apply(CEF_bif g_bif_hpr_alloc, g_null_callers_flags, [ wrap64 bytes; wrap64 aspace]), ov)


                               | other -> cleanexit (m0() + sprintf ": Bad form heap pointer for obj_alloc of type %s post end of elaboration point (or have already allocated a runtime variable sized object ?). Check whether -kiwic-autodispose=enable. Then, unless you are geninuely making a dynamic linked list or tree, this can generally be fixed using a manual call to Kiwi.Dispose() in your source code at the point where your allocation could be safely garbage collected.  storemode=%A, sbrk=" types storemode + ce_hs_envToStr 2 ov)
                       (hidxo, ce, Some hwm, Some ov)
                else sf (m0() + sprintf ": no env provided to lookup heap descriptors. storemode=%A reallyf=%A" storemode reallyf)

            | _ -> sf "storemode other form L3453"

    vprintln 3 (sprintf "obj_alloc_core: storemode=%A reallyf=%A hintname=%s bytes=%i bytes at %s.  ce=%s" storemode reallyf (hptos hintname) bytes (if nonep hidxo then "None " else i2s64(valOf hidxo)) (ceToStr ce))

    let _ =
        if reallyf then
            //if bytes = 0L then sf (sprintf "ZERO LENGTH OBJECT ALLOC %s reallyf=%A" (hptos hintname) reallyf)
            ()
    let env' =
        if reallyf && storemode = STOREMODE_runtime_heap then
            // Even in md=2 we need to bump the heap pointer here (or otherwise flag non-constant) to stop the constant meet converging.
            let nv =
                if nonep hidxo then CEE_hs_tend "obj_alloc_core - storemode=2"
                else
                    match ov with
                        | None ->  CEE_hs_tend "obj_alloc_core - ov storemode=2"
                        | Some(CEE_heapspace(allocator_info, inuse, discreps)) ->
                          //let nvi = valOf hidxo + heap_alloc_roundup bytes
                          //let _ = hwm := nvi // both forms store (for now)
                          //vprintln 0 (sprintf "Val of heap sbrk post storemode=2 alloc = %i" nvi)
                            CEE_heapspace(allocator_info, insert (valOf hidxo) inuse, discreps)
                        | oo -> sf (sprintf "spare posit %A" oo)
            let env = if nonep env then env else Some { valOf env with hs1=nv }
            if vd>=5 then vprintln 5 (sprintf "new_obj_core: L2681 runtimeheap sbrk %s " (if nonep env then "NONE" else ce_hs_envToStr 10 (valOf env).hs1))
            env
        elif reallyf then
            //let nv = valOf hidxo + heap_alloc_roundup bytes
            //let _ = hwm := nv // both forms store (for now)
            let e1 = if nonep env || nonep nv then env else Some { valOf env with hs1=valOf nv }
            //if storemode <> 0 then vprintln 3 (sprintf "heaper: Val of heap sbrk post alloc is %s" (ceTreeToStr 1 nv))
            if vd>=4 then vprintln 4 (sprintf "new_obj_core: L2685 heap sbrk %s " (if nonep e1 then "NONE" else ce_hs_envToStr 10 (valOf e1).hs1))
            e1
        else env
    (hidxo, ce, env') // end of obj_alloc_core


// If not reallyf then return base answer but do not bump sbrk etc..        
// No runtime_heap (storemode=2) supported via this wrapper.
let alloc_fresh_vreg ww cCBo realdo purpose dt ident storemode =
    let cCC = valOf !g_CCobject
    let vrn = !cCC.next_virtual_reg_no // +ve vregs go in cCB
    mutinc cCC.next_virtual_reg_no 1
    let vreg_record = { allocated=ref true; phyreg=ref None; vrn=vrn; ident_=ident; purpose=purpose; dt=dt; storemode=storemode }
    let _ = cCC.m_vregs.add vrn vreg_record
    if not_nonep cCBo then mutadd (valOf cCBo).local_vregs_used vreg_record
    vrn


let kk_ats (a, b) = a + "=" + b

//
//
let user_or_logical_memspace_lookup ww m0 (hintname:string list) ats =
    let aspace =
        match op_assoc g_bondout_heapspace_name ats with
            | None -> -1L
            | Some spaceName ->
                let cCC = valOf !g_CCobject
                let bondouts = cCC.bondouts
                if nullp bondouts then cleanexit(sprintf "Cannot lookup or allocate bondout memory bank with name '%s' for %s since no bondout ports have been defined (check bondout-schema setting)" spaceName (hptos hintname))
                let size = atoi64(valOf_or_fail "L3055: no g_bondout_heapspace_size" (op_assoc g_bondout_heapspace_size ats))
                let rec scan_aspace pass = function
                    | [] -> None
                    | ((space:bondout_address_space_t, ports), (manager:bondout_memory_map_manager_t)) :: tt -> // Lookup logical or user name
                        if space.logicalName = spaceName then Some space
                        elif memberp spaceName !space.m_userNames then Some space
                        elif pass=1 && manager.space_availablep size then // Greedy insert in first space
                            let m_dynamic_heaps = cCC.m_additional_net_decls// For forwarding on to the resultant netlist.
                            let (bonk, ids, ff, f2) = protocols.retrieve_named_bondout_bank_pseudo ww 0 m_dynamic_heaps spaceName
                            let oc_record_ = manager.inject_ram (ff) (hd ports) // TODO this always uses the first port  - need to load balance
                            mutadd space.m_userNames spaceName
                            Some space
                        else scan_aspace pass tt
                let ans =
                    match scan_aspace 0 bondouts with
                        | Some ov -> ov
                        | None ->
                            vprintln 2 (sprintf "Establish user bondout space called %s" spaceName)
                            let ans1 = scan_aspace 1 bondouts
                            valOf_or_fail "L0376" ans1
                    
                dev_println (sprintf "aspace lookup needed: spaceName=%s defaultsize=%i found=%A ats=%A  hintname=%s" spaceName size ans ats (hptos hintname))
                0L

    aspace


// We have one heap manager per logicalSpace
let rec obj_alloc_serf ww m0 types reallyf storemode aido env (hintname, ats, bytes) = 
    vprintln 3 (sprintf "obj_alloc_serf: reallyf=%A storemode=%A" reallyf storemode)
    let aspace = user_or_logical_memspace_lookup ww m0 hintname ats
    let (hidxo, ce_, env') = obj_alloc_core m0 types reallyf storemode aido env aspace hintname bytes //other callsites in cilnorm
    (valOf hidxo, env')


//
// Generate an unadressable/addresable scalar variable. Now renamed as singleton since some are addressable.  And always invoke through gec_singleton.
//
and gec_uav_core ww vd m0 ethereal atakenf storemode realdo llbf vdt (nemtok, ats) vrn io_group_opt envo str_fd_o =
    // staticf here relates to the allocation style in the hpr output, not the c# input. Many dynamic variables in the C# become static within this front end.
    let (tlm_propertyf, sri_token, sri_dt, fu_kind, fsems) =
        match storemode with
            | STOREMODE_sri(tlm_propertyf, sri, sri_dt, fu_kind, fsems) -> (tlm_propertyf, sri, sri_dt, Some fu_kind, fsems)
            | _ ->                                                         (None, "", CTL_void, None, None)
    let staticf = storemode = STOREMODE_singleton_scalar || storemode = STOREMODE_singleton_vector
    let doit = not(tychk_pass_pred realdo) || staticf
    let cCC = valOf !g_CCobject
    let sheap_id = nemtok:new_nemtok_t  // same as ce_aid please since unadressable (makes things easier).
    let (vrn, sheo) =
        if vrn <> 0 then (vrn, cCC.heap.sheap2.lookup vrn)
        elif not doit then sf (sprintf "cannot autoalloc vreg no when doit unset for %s" sheap_id)
        // elif not have_vreg then (0, None)
        else
            match cCC.heap.sheap1.lookup sheap_id with
                | Some she -> (she.vrn, Some she)
                | None ->
                   let vrn = - !cCC.next_negative_reg_no // Allocate a -ve vreg no for lookup purposes, but do not put in cCB (-ve helps distinguishing when debugging.)
                   mutinc cCC.next_negative_reg_no 1
                   //dev_println (sprintf "autoalloc vreg doit=%A for %s as V%i" doit sheap_id vrn)
                   let vreg_record = { allocated=ref true; phyreg=ref None; vrn=vrn; ident_=[sheap_id]; purpose="*negative*"; dt=vdt; storemode=storemode }
                   cCC.m_vregs.add vrn vreg_record // -ve vregs do not go in cCB and have global persistence
                   (vrn, None)
            
    match sheo with
        | Some she when she.tlm_propertyf = tlm_propertyf ->  // Can be redefined as volatile/sri etc ...
            let vrn = she.vrn
            if vd>=4 then
                vprintln 4 (sprintf "gec_uav: %s V%i idl=%s static=%A doit=%A exists=%A" (m0()) vrn sheap_id staticf doit true)
                vprintln 4 (sprintf "named reg/sheap item existed. blobf=%A sheap %s" she.labelled_literal (ceToStr she.cez))
            let _ =
                if vrn <> 0 then
                    match cCC.m_vregs.lookup vrn with
                        | None -> sf (sprintf "gec_uav: existed: virtual register V%i is unknown" vrn)
                        | Some ov ->
                            if vd>=4 then vprintln 4 (sprintf "get_uav: cf old and new V%i cf V%i\nov=%A" vrn ov.vrn ov)

        //    let checksame(CEZ(idl1, cr_, dt1, wopt1, uid, st1, iot1, _, _, dims1)) =
        //        idl1=idl && dt1=dt && wopt1=wopt && dims1=dims && st1=st && iot1=iot
            ((she.labelled_literal, she.cez), envo) // Need to check cgr and ats!!!!!!!!!!!!!!! TODO are still same
        | _ ->
            let regionf = false
            let dtidl = ct2idl ww vdt
            let name =
                let username = op_assoc "username" ats
                if username<>None && String.length(valOf username) > 0 then valOf username
                elif nonatakef sheap_id 0L then hptos_net [sheap_id] // TODO - only 2 remaining uses of ataken dbase - can remove I think.
                else hptos_net dtidl // needs subsc of wond

            let vdtf = ct_is_valuetype vdt
            let l = if vdtf then ct_bytes1 ww "gec_uav:L2633" vdt else g_pointer_size
            if vd>=4 then vprintln 4 (sprintf "gec_uav: central: %s idl=%s VRN=V%i vdtf=%A dt=%s static=%A atakenf=%A ats=%s was not yet on singleton heap doit=%A (user)name=%s len=%i" (m0()) sheap_id vrn vdtf (cil_typeToStr vdt) staticf atakenf (sfold kk_ats ats) doit name l)

            let (baser, envo, cez) = 
                if not_nonep tlm_propertyf  then                     // blocking read TLM copy1/2
                    let cf = { g_null_callers_flags with tlm_call=tlm_propertyf }
                    let (prec, _) = get_prec_width ww vdt
                    //dev_println(sprintf "BDAY fixed now gec_uav using %A as fu_arg when it should be the kind" sri_token)
                    let mdt_gis =
                        {
                            name=                    ["read/write"]
                            meth_parent_class=       ([sri_token], ref None)
                            gamma=                   []
                            rtype=                   vdt
                            arg_formals=             []
                            ty_formals=              []
                            uname=                   funique "read/write"
                            ataken_formals=          [] // int list
                            ataken_locals=           []
                            mutated_formals=         [1]
                            has_macro=               false
                            has_macro_regs=          false
                            is_ctor=                 -1
                            is_root_marked=          None
                            is_remote_marked=        None
                            is_accelerator_marked=   None
                            is_fast_bypass=          None
                            is_static=               false    // It is TLM
                            is_hpr_prim=             false
                            sri_arg=                 Some "1" // TODO use same numbering as ataken_formals
                            fu_arg=                  fu_kind  // The sri_token is instance name.
                            fsems=                   fsems
                            clknets=                 g_default_clknets
                        }
                        //eis=         gis.fsems.fs_eis
                        //    is_loaded=   true//gis.is_loaded
                        //    hpr_native=  Some gis
                    let aobj = gec_yb (gec_X_net sri_token, sri_dt)
                    let cez = CE_apply(CEF_mdt mdt_gis, cf, [aobj])
                    (-1L, envo, cez)
                else
                    let (baser, envo) = obj_alloc_serf ww m0 name true storemode (Some nemtok) envo ([sheap_id], ats, l)
                    let cez = CE_var(vrn, vdt, [sheap_id], str_fd_o, ats)
                    (baser, envo, cez)

            let she =
               {
                   atakenf=          atakenf
                   vrn=              vrn
                   labelled_literal= llbf
                   s_aid=            A_loaf nemtok
                   s_nemtok=         nemtok
                   idl=              [sheap_id] // for now - not really correct
                   len=              l
                   baser=            baser
                   cez=              cez // The answer
                   ever_addressed=   ref false
                   ethereal=         ethereal
                   hexp=             ref None
                   io_group=         io_group_opt
                   dt=               vdt
                   tlm_propertyf=    tlm_propertyf
                   //username=         username
               }
            if doit then
                vprintln 2 (sprintf "Defined sheap entry %s ethereal=%A  V%i  dt=%s  (redefinition=%A)" sheap_id ethereal vrn  (cil_typeToStr vdt) (not_nonep sheo))
                //if vrn=5007 then sf "TRAP CAT"
                cCC.heap.sheap1.add sheap_id she  
                cCC.heap.sheap2.add vrn she
                ()
            ((she.labelled_literal, she.cez), envo) // TODO: Preferably should check cgr and ats are still the same
            

//
// Typically a shared variable (currently only scalars) is being converted to a structural FU with method invokations on it.
//
let record_shared_resource_info ww cCP fu_name dt fsems arg =
    let cCC = valOf !g_CCobject
    let vd = 3
    let msg = "record_shared_resource_aid"
    let mf = (fun() ->  msg + " fu_name=" + hptos fu_name)
    dev_println (mf())
    let sri_dt = CTL_reflection_handle fu_name

    match arg with
        | CE_x(_, (X_net _)) -> arg // Already an SRI token.
        | arg ->

            let note_volatilef full_correction_neededf nemtok =
                let tid = cCP.tid3
                let vtag = msg
                generic_constvol_tracker ww cCP.m_shared_var_fails msg tid vtag true full_correction_neededf nemtok

            let sri_token = 
                match ce_aid0 mf arg with
                | Some aid ->
                    let sci = cCC.kcode_globals.ksc.classOf_i msg (aid)
                    match aid with
                        // Star also expected?
                        | (A_subsc(A_loaf nemtok,_)) -> // A_subsc form owing to pass by reference of interlock 'location' 
                            let ats = []
                            let sri = sri_digest([nemtok], ats)

                            cCC.kcode_globals.ksc.attribute_set sci "sri" sri // Bruteforce - need to laise with constvol?
                            let sri = "SRI_" + nemtok
                            vprintln 3 (sprintf "Noted shared resource identifier (sri) %s at '%s' for " nemtok msg + aidToStr aid)
                            let gen full_correction_neededf =
                                // let (fname_, gis) = gec_generic_sri_tlm_fgis prec sri_token fu_name mdt.fsems lmodef 
                                note_volatilef full_correction_neededf nemtok
                                let tlm_propertyf = Some true // Blocking TLM
                                let realdo = Rdo_gtrace(ref [], ref[])   // Not relevant
                                let ethereal = false                     // Not relevant
                                let vrn = 0                              // Not relevant
                                let vdt = dt
                                let atakenf = Some true
                                let io_group_opt = None
                                let m0 = fun () -> msg
                                let op_group_opt = None
                                let llbf = false
                                let envo = None
                                let str_fd_o = None
                                let storemode = STOREMODE_sri(tlm_propertyf, sri, sri_dt, fu_name, fsems)
                                let ((llf_, cez), _) = gec_uav_core ww vd m0 ethereal atakenf storemode realdo llbf vdt (nemtok, ats) vrn io_group_opt envo str_fd_o
                                ()
                            match cCC.heap.sheap1.lookup nemtok with
                                | None ->
                                    vprintln 3 (sprintf "sri %s not known on sheap - need to add it" sri)
                                    gen false

                                | Some cez ->
                                    if nonep cez.tlm_propertyf then
                                        vprintln 3 (sprintf "sri %s not STOREMODE_sri as entered on sheap - need to correct it" sri)
                                        gen true // need to correct sheap entry
                            sri
                        | other ->
                            sf (msg + sprintf ": 1/2 Failed to note other=%A  mf=" other + mf())
                | _ ->
                    sf (msg + sprintf ": 2/2 Failed to note " + mf() + " arg=" + ceToStr arg)
            gec_yb (gec_X_net sri_token, sri_dt)




let rec dt_is_char = function 
    | CTL_net(_, 16, _, nats) -> nats.nat_char
    | CT_cr crst -> crst.name =  [ "System"; "Char" ]
    | _ -> false
    
let rec dt_is_string = function
    | CTL_net(_, 16, _, nats) -> nats.nat_string // This form not used. An array of chars is used always
    | CT_arr (act, _) -> dt_is_char act
    | CT_cr crst -> crst.name = ["String"; "System" ]
    | CT_star(_, dt) -> dt_is_string dt
    | _ -> false



(*
 * Define an X_bnet hexp_t form from a CIL type.
 * We called this netgen but it is used for all variables that are converted to hexp_t form at any point and many will not make it to becoming hardware nets owing to conerefine and other removal mechanisms.
 * If a wopt width option it present, it takes precedence over anything implied from ctype.
 *

 * Arrays are created by newarr but the offchip attributes are held against the field that they are
 * first pointed to by.  This is implemented via the hintname mechanism.
 * We take array length either from ctype or length attribute.
 *
 *
 * A warray (wondarray), like all hexp_t arrays, is a 1-D array indexed by an integer, but the subscript is called a widx and the length is nominally 2**g_pointer_size, with repack slicing it to a number of reaonable-sized actual arrays.
 * A widx is a wvar or a wtoken, depending on whether a constant value is found by adv4 or not. 
 *)
let netgen_kernel ww mx (id, dt, wopt_lostio_oldway, ats) = 
    let len' = []
    let CC = valOf !g_CCobject
    let vd = 3 // 
    let is_string = dt_is_string dt || not_nonep(op_assoc "is-string" ats)
    let wopt1 = op_assoc "HwWidth" ats
    let wopt = if wopt1 <> None then Some(atoi32(valOf wopt1)) else wopt_lostio_oldway
    let volf = ct_is_volatile ww dt //op_assoc g_volat ats = Some "true"
    let ats = if volf then (g_volat, "true")::ats else ats
    let io = if op_assoc "io_input" ats <>None then INPUT
             elif op_assoc "io_output" ats <>None then OUTPUT
             elif op_assoc "io_retval" ats <>None then RETVAL // no longer used.
             else LOCAL
    let use_integer = nonep wopt && is_i32 dt
    let constantf = op_assoc "constant" ats = Some "true"
    //let vd = 0 // for now
    if vd >=4 then
        vprintln 4 (sprintf "  NETGEN_KERNEL mx=%s io_output=%A ats=%s" mx ((op_assoc "io_output" ats)<>None) (sfold kk_ats ats))
        vprintln 4 (sprintf "  NETGEN_KERNEL id=%s dt=%s io=%s is_string=%A  use_integer=%A constantf=%A" id (cil_typeToStr dt) (varmodeToStr io) is_string use_integer constantf)
        ()
    let states nats = nats.nat_enum

    let kats_oride c nats = // Preserve certain attributes.
         (if nats.nat_char   then [Nap("presentation", "char")]   else []) @
         (if nats.nat_string then [Nap("presentation", "string")] else []) @ c         
    let atsa = map (fun (a,b)->Nap(a,b)) ats

    let io_oride io nats =
        //if nats.xnet_io <> LOCAL then nats.xnet_io else io
        io  
    // Insert string handles for variables that range over strings let repack sort them out.
    let width_oride width = if is_string then -1 elif not_nonep wopt then valOf wopt else width

    let rec netgen_kernel_kot ns arg =
        //dev_println (sprintf "netgen_kernel_kot nstars=%i of %s" ns (typeinfoToStr arg))
        match follow_knot "netgen kot" arg with
            | CT_star(n', a) -> netgen_kernel_kot (ns+n') a 

            // Enumeration types are wond's sometimes! Please explore.
            | dt when dt=g_ctl_object_ref -> (* "wondtwo" gets used for spills : read return in parfir tap ?*)
                  if vd>=5 then vprintln 5 ("  NETGEN WOND/OBJ")
                  xgen_bnet(iogen_serf_ff(id, [], 0I, int(g_pointer_size*8L), io, Unsigned, None, false, atsa, []))
            | CT_cr _ -> 
                  if vd>=5 then vprintln 5 ("  NETGEN WOND/OBJ")
                  xgen_bnet(iogen_serf_ff(id, [], 0I, int(g_pointer_size*8L), io, Unsigned, None, false, atsa, []))

            | ((CT_class _) as dt) ->
                    let tycontrol =
                        {
                            grounded=true
                        }
                    let tcn = (tycontrol, Map.empty)
                    let dt = cil_type_n ww tcn dt
                    match dt with
                        | CTL_record(idl, cst, lst, len, ats, binds, _) ->  netgen_kernel_kot ns dt  // Grounded, go round again.
                        | other ->
                            sf (sprintf " netgen_kernel_kot: intangible class used as net type %s" (cil_typeToStr other))

                            //netgen_kernel_kot ns (muddy (sprintf "netgen_kernel_kot: CTL_wond wox %s"  (cil_typeToStr dt)))

            | tangible ->
                if ns <> 0 then // New coding - check ns once for all forms here: some of the checks of ns below in the tangible dispatch are now redundant.
                    if vd>=4 then vprintln 4 ("  NETGEN GENERIC PTR")
                    xgen_bnet(iogen_serf_ff(id, [], 0I, int(g_pointer_size*8L), io, Unsigned, None, false, atsa, []))
                else
                    match tangible with
                        | CTL_void when abs ns = 1 -> netgen_kernel_kot 0 g_canned_IntPtr // no longer used
 
                        | CTL_reflection_handle _ -> netgen_kernel_kot ns g_IntPtr 

                        | CTL_net(volf, width, signed, flags) -> 
                           if vd>=4 then vprintln 4 ("  NETGEN NET")
                           if constantf then
                               let q = atoi(valOf_or_fail "no value for constant" (op_assoc "init" ats))
                               xi_named_big_constant q None id // for now
                           else xgen_bnet(iogen_serf_ff(id, (if len' <> [] then len' else []), 0I, width_oride width, io_oride io flags, signed, states flags, use_integer, kats_oride atsa flags, []))

                        | CTL_record _ ->
                            let width0 = int(dt_width_bits ww tangible)
                            let width = width_oride width0
                            if vd>=4 then vprintln 4 (sprintf "  NETGEN RECORD width=%i then %i" width0 width)
                            let ans = xgen_bnet(iogen_serf_ff(id, [], 0I, width, io, Unsigned, None, false, atsa, []))
                            ans

                        | CT_arr(CTL_net(volf_, width0, signed, flags), alen) -> // Only 1D arrays are native. - TODO old coding: should call width function on array content type instead of matching for CTL_net here
                            let width = int width0
                            if vd>=4 then vprintln 4 (sprintf "  NETGEN L2662 ARR/NET nstars=%i widths=%i then %i" ns width0 width)
                            let dims_ = if ns=0 then [int64(valOf_or_fail "array length (alen) missing" alen)] else []
                            // This does not generate a simple array of the appropriate length (that is left to repack). Instead, here we return the wondarray.
                            let dims = g_wondarray_marker_dims
                            xgen_bnet(iogen_serf_ff(id, dims, 0I, width_oride width, io_oride io flags, signed, states flags, use_integer, atsa, []))

                        | CT_arr(dt, alen) ->
                            if vd>=4 then vprintln 4 (sprintf "  NETGEN L2668 ARR nstars=%i io=%s" ns (varmodeToStr io))
                            let dims_ = if ns=0 then [int64(valOf_or_fail "no alen" alen)] else []
                            // This does not generate a simple array of the appropriate length (that is left to repack). Instead, here we return the wondarray.
                            let dims = g_wondarray_marker_dims
                            xgen_bnet(iogen_serf_ff(id, dims, 0I, width_oride (int(dt_width_bits ww dt)), io, ct_signed ww dt, None, use_integer, atsa, []))


                        | other -> sf(mx + ":" + id + sprintf ": netgen k: nstars=%i " ns + "; inappropriate type/form for conversion to hpr net (or RTL net): " + typeinfoToStr other)
    let ans = netgen_kernel_kot 0 dt
    if vd>=3 then vprintln 3 (mx + ": NETGEN dt=" + typeinfoToStr dt + ", use_integer=" + boolToStr use_integer + ", dims=" + (sfold (fun x -> "[" + i2s64 x + "]") len') + "\nnetgen ans=" + netToStr ans)
    ans


let islab = function
    | CIL_instruct(_, _, _, Cili_lab _) -> true
    | (_) -> false

(*
 * Insert spillage ensures there is nothing on the PE stack at a basic block boundary.  It also macro expands some instructions.
 * We insert loads after the leading label(s).
 * We could insert saves before any trailing jumps or branches, but then the branch conditions would be wrong.  Instead, the elaborator knows
 * to defer the actual branch to the end of the BB, so the saves are placed last but not executed last.
 *)
let insert_spillage (fFP: firstpass_t) block_pc =
    let ff ss = youtln fFP.fvd ss
    let cCC = valOf !g_CCobject
    let kspill pc =
        let (handler_ctxo, code) = fFP.ia.[pc]
        let loadlist = fFP.spillin.[pc]  // A list of registers.
        let savelist = valOf_or_nil fFP.spillout.[pc] // A list of basic block numbers (bpc list).
        ff(sprintf "Spill Load/Save stack.  bpc=%i  with |loadlist|=%i   |savelist|=^%i:%s " pc (length loadlist) (length savelist) (sfold i2s savelist))

        let loaders() = map (fun n -> gec_ins(Cili_ldloc(Cil_argexp(Cil_number32 n)))) loadlist
        let savers() = 
                let rec k1 c = function
                    | []    -> c
                    | n::tt ->
                          let a =  gec_ins(Cili_stloc(Cil_argexp(Cil_number32 n)))
                          let b =  gec_ins(Cili_dup)
                          if tt=[] then a::c else k1 (a::b::c) tt
                let rec k0 = function
                   | []   -> []
                   | h::t -> k1 (k0 t) h
                (* We should !! reverse here because load and save orders are reversed on a stack m/c *)
                let jj dpc = (* rev *)(fFP.spillin.[dpc])
                rev(k0(transpose(map jj savelist))) (* final rev since list built up backwards *)

        let rec zz = function
            | (true, []) -> loaders()
            | (false, []) -> []
            | (ls, h::tt) -> 
                let c = not(islab h)
                (if c && ls then loaders() @ [h] else [h]) @ (zz (ls && (not c), tt))

        Array.set fFP.ia pc (handler_ctxo, (zz(true, code)) @ savers())
        ()
    kspill block_pc
    ()

let gen_post_spill_cil_listing ww cCC fFP =
    let rec gen_post_spill_cil_listingy pos =
        let ff ss = vprintln 0 ss // yout(0, s + "\n")
        ff ("Post_spill_cil_listing")
        let rec disp1 pc p = function
            | []     -> ()
            | ins::t -> (ff("  " + i2s pc + "." + i2s p + "::: " + cil_classitemToStr "" ins ""); disp1 pc (p+1) t)

        if pos >= fFP.il
        then ff (hptos fFP.idl + "--B--late-------------------- " + i2s pos + " END")
        else 
          (
           ff((if pos=0 then "\nPost spill var CIL Listing\n" else "") + hptos fFP.idl + "--B---------------------- pc=" + i2s pos);
           //ff("loadlist: reg nos are = " + sfold i2s (fFP.spillin.[pos]));
           //ff("savelist: dests are = {" + sfold i2s (valOf_or_nil(fFP.spillout.[pos])) + "}");
           disp1 pos 0 (snd fFP.ia.[pos]);
           gen_post_spill_cil_listingy (pos+1)
          )
    gen_post_spill_cil_listingy 0
    ()





// labscan - find control flow branches, jumps and labels used in CIL instructions and apply f to them as found.
// Labels are in string list form but are always just simple strings?
// Control flow in and out of exception handlers and finally blocks needs separating off since we treat finally blocks as subgraphs that are replicated per subsequent destination.    
let labscan00 f arg cc =
    match arg with
        | Cili_brtrue s        -> ([s], Cili_brtrue(f s), false) :: cc
        | Cili_brfalse(sco, s) -> ([s], Cili_brfalse(sco, f s), false) :: cc

        | Cili_switch lablist -> (lablist, Cili_switch(map f lablist), false) :: cc

        | Cili_bge s -> ([s], Cili_bge(f s), false) :: cc
        | Cili_bgt s -> ([s], Cili_bgt(f s), false) :: cc
        | Cili_ble s -> ([s], Cili_ble(f s), false) :: cc
        | Cili_blt s -> ([s], Cili_blt(f s), false) :: cc

        | (Cili_bge_un s) -> ([s], Cili_bge_un(f s), false) :: cc
        | (Cili_bgt_un s) -> ([s], Cili_bgt_un(f s), false) :: cc
        | (Cili_ble_un s) -> ([s], Cili_ble_un(f s), false) :: cc
        | (Cili_blt_un s) -> ([s], Cili_blt_un(f s), false) :: cc


        | (Cili_beq s) -> ([s], Cili_beq(f s), false) :: cc
        | (Cili_bne s) -> ([s], Cili_bne(f s), false) :: cc
        | (Cili_br s) -> ([s], Cili_br(f s), false) :: cc
        | (Cili_leave s) -> ([s], Cili_leave(f s), true) :: cc // Set flag on xfers in and out of finally  blocks (Cili_leave is the 'out of' direction).
        | _ -> cc


let rec labscan2 f arg cc =
    match arg with
        | CIL_instruct(_, _, _, v) -> labscan00 f v cc
        | CIL_try(nto, try_blk, (token, catchers, (nfo, finally_blk))) ->
            let cc = List.foldBack (labscan2 f) try_blk cc
            let cc = List.foldBack (labscan2 f) (list_flatten(map f3o3 catchers)) cc
            List.foldBack (labscan2 f) finally_blk cc          
        | _ -> cc

let labscan2a f arg = labscan2 f arg []

        
let isinst_macro_render ww normenv handler_ctx ins =
    let cCC = valOf !g_CCobject
    match ins with
        | Cili_isinst(exactf, ctype) -> // Check where object handle is an instance of the given type returning null if not and returning the up/down cast if so (as per C++ dynamic_cast<T>(arg)).
            let msg = "isinst"
            let ww = WN msg ww
            if not cCC.settings.dynpoly then cleanexit(sprintf "Cannot use %s with dynpoly=false" msg)
            let (new_instructions) = 
                    let dt = cil_type_n ww normenv ctype
                    let rec get_vt_constant = function
                        | CT_cr crst -> get_vt_constant (destar crst.crtno)
                        | CTL_record(record_idl, cst, lst, len, ats, binds, srco) ->
                            if nonep cst.vtno then sf (msg + ": There is no virtual table for an " + hptos(record_idl))
                            valOf cst.vtno
                        | other ->  sf (msg + sprintf ": get_vt_constant not expecting a %s : %A" (cil_typeToStr other) other)
                    let vt_constant = get_vt_constant(destar dt)
                    let ct1 = type_idl msg (destar dt)
                    let dt1 = type_idl msg (destar dt)
                    let _ = vprintln 3 (msg + sprintf ": ty=%s isinst check with %s vt_constant=%i " (cil_typeToStr dt) (cil_typeToStr ctype) vt_constant)

                    if exactf then
                        let aload n = Cili_ldarg(Cil_argexp(Cil_number32 n))
                        let new_instructions =
                            [ Cili_dup
                              Cili_ldfld(g_canned_i32, (dt, "_kv_table", []))
                              Cili_ldc(g_canned_i32, Cil_argexp(Cil_number32 vt_constant))
                              Cili_ceq
                              Cili_kiwi_special("isinst", exactf, dt)
                            ]
                        // This pushs null or the cast.

                        (new_instructions)
                    else muddy msg // upcast match not implemented yet - TODO


            map (freshwrap handler_ctx) new_instructions

        | other ->
            let dis = ciliToStr ins
            sf ("KiwiC: PE/CIL instruction was not a cili_macro: " + dis)


//
//  Expand instructions in macro form
//
let macro_expand_cil_instructions ww block_move_rego instructions =
    let bmo = block_move_rego
    let normenv = ({grounded=false}, Map.empty) // for now - some generic bindings needed perhaps in the future

    let cbranch = function
        | CIL_instruct(_, _, _, Cili_brtrue ss)      -> Some (false, ss) 
        | CIL_instruct(_, _, _, Cili_brfalse(_, ss)) -> Some (true, ss)
        | _ -> None
    let rec macro_expand_ins = function
        | CIL_try(tbo, tryblk, (token, catchers, (fbo, finalblk)))::tt -> 
            let cf (v, cno, lst) = (v, cno, macro_expand_ins lst)
            CIL_try(tbo, macro_expand_ins tryblk, (token, map cf catchers, (fbo, macro_expand_ins finalblk))) :: macro_expand_ins tt
        | CIL_instruct(_, handler_ctx, _, Cili_isinst(a, b))::tt -> isinst_macro_render ww normenv handler_ctx (Cili_isinst(a, b)) @ macro_expand_ins tt
        | CIL_instruct(_, handler_ctx, _, Cili_cpblk)::tt   -> cpblk_initblk_render (valOf bmo) "cpblk" @  macro_expand_ins tt
        | CIL_instruct(_, handler_ctx, _, Cili_initblk)::tt -> cpblk_initblk_render (valOf bmo) "initblk" @  macro_expand_ins tt
        | i1::i2 ::tt when inHardware_check i1 && not_nonep(cbranch i2) ->
                match cbranch i2 with
                    | Some (falsef, dest) ->
                        let _ = vprintln 3 (sprintf "Peephole redirect of inHardware control flow: falsef=%A dest=%s" falsef dest)
                        let nv = if falsef then CIL_comment ("Site of branch if not inHardware to " + dest) else gec_ins(Cili_br dest)
                        nv :: macro_expand_ins tt
        | other :: tt -> other :: macro_expand_ins tt
        | [] -> []
    macro_expand_ins instructions 

(*
 * Perform code setup for a method body by ...
 *    macro expanding some instructions
 *    putting the instructions in an array
 *    scanning for the lables used removing those that are not.
 *    making exit via a single return statement (repeated also at the intcil level so redundant here?).
 *)
let code_setup ww (cCB:cilbind_t) mdt bmo id instructions =
    let fd = YOVD !g_firstpass_vd
    let cCC = valOf !g_CCobject
    let ff s = vprintln 3 s
    let lpref = "LL"
    let rec codelen = function
        | [] -> 0
        | CIL_try(tbo, tryblk, (_, catchers, (fbo, final_blk)))::tt -> codelen tryblk + List.fold (fun cc (a, cbo, b) -> length b + cc) 0 catchers + codelen final_blk + codelen tt + 1
        | h::tt -> 1 + codelen tt

    // We insert an explicit Cili_lab before an instruction that is a branch target. We dont say why. But they get renumbered by macro expand.  We only put them where there is a branch target since additional ones will defeat peephole rewrites pattern matching.

    let bdests0 =
        let bdests0 = new OptionStore<string, bool>("bdests0")
        let ff ss =
            //let _ = dev_println (sprintf "bdests0: assimilate " + ss)
            (bdests0.add ss true; ss)
        let _ = app ((labscan2a ff)>>ignore) instructions
        bdests0

    let rec cilcode_flatten lst =
        if nullp lst then []
        else
            match hd lst with
                | CIL_try(tbo_, try_blk, (token, catchers, (fbo_, final_blk))) ->
                    // Here we move the code out of the try block and place it all flattly with the block itself changied to a Cili_kiwi_trystart
                    // We don't put the numbers in originally in case they get changed anywhere before here (but I dont think they do get changed)
                    let rec biz_code_idx = function
                        | []                               -> -1
                        | CIL_instruct(offset, _, _, _)::_ -> offset
                        | CIL_try(_, try_blk, _)::_        -> biz_code_idx try_blk
                        | other -> sf (sprintf "biz_code_idx other form %A" other)
                    let yax (pattern, _, code) = (pattern, Some (biz_code_idx code), [])
                    //let nv = CIL_try(Some(biz_code_idx try_blk), [], (token, map yax catchers, (Some(biz_code_idx final_blk), [])))
                    let nv = Cili_kiwi_trystart(token, map yax catchers, (Some(biz_code_idx final_blk), []))
                    gec_ins nv :: cilcode_flatten try_blk @ list_flatten(map cilcode_flatten (map f3o3 catchers)) @ cilcode_flatten final_blk @ cilcode_flatten(tl lst)
                | x -> x :: cilcode_flatten(tl lst)


    let rec add_src_labs = function
        | CIL_instruct(off, handler_ctx, prefix, i1)::tt ->
            let plab = lpref+i2s off
            if nonep (bdests0.lookup plab) then CIL_instruct(off, handler_ctx, prefix, i1) :: (add_src_labs tt)
            else gec_ins(Cili_lab plab) :: CIL_instruct(off, handler_ctx, prefix, i1) :: (add_src_labs tt)

        | CIL_try(tbo, try_blk, (token, catchers, (fbo, final_blk)))::tt -> // clause not useful post cilcode_flatten
            let daf (pp, cbo, lst) = (pp, cbo, add_src_labs lst)
            let _ = muddy "now unused"
            CIL_try(tbo, add_src_labs try_blk, (token, map daf catchers, (fbo, add_src_labs final_blk))) :: (add_src_labs tt)
        | h::tt -> h::add_src_labs tt
        | [] -> []

    let instructions = add_src_labs (cilcode_flatten instructions)

    let instructions =
        if not mdt.has_macro then instructions
        else macro_expand_cil_instructions ww bmo instructions 

    let keep_return = fst(cCB.retlab) && mdt.is_ctor=0 // Constructors do not have returns or hangs.

    // We should no longer need to generate the tailhang since it is now defined to be part of the director semantics and implemented by receiving hbev recipe stages instead of generators such as kiwife.
    //let tailhangf = cCB.tailhangf && mdt.is_ctor=0
    let tailhangf = f5o5 cCB.director_ref = DHM_hang && mdt.is_ctor=0
    let margin = 100 // A few instructions such as those for the tailhang are inserted by myself. (Tailhang to become intrinsic in the director at some point)
    let ssize = margin + codelen instructions
    ff(sprintf "code_setup: keep_return=%A" keep_return + sprintf ", tailhangf=%A,  sequence of %i instructions" tailhangf ssize)
    // Remove unused labels.  Note we just did this with bdest0.  Perhaps this copy is now redundant.
    let instructions = 
        let labsused =
            let m_labsused = ref []
            let _ = List.foldBack (labscan2 (fun x->(mutadd m_labsused x;x))) instructions []
            !m_labsused
        if yout_active fd then reporty fd "Dest labs really used" (fun x->x) labsused
        let neededlab = function
            | CIL_instruct(_, _, _, Cili_lab s) -> memberp s labsused
            | _ -> true
        List.filter neededlab instructions 

    let rlab = funique (if tailhangf then "cbgtailhang" else "cbgrlab")
              
    //See if there are returns in the body and if so map them to the single return ins at the rlab label. 
    let redirect_returns (instructions) = 
        youtln fd ("Redirecting returns to " + rlab)
        let rec zz = function
            | [] -> [ gec_ins(Cili_lab rlab); gec_ins Cili_ret ]
            | CIL_instruct(_, _, _, Cili_ret)::t -> gec_ins(Cili_br rlab)::(zz t)
            | other::t -> other::(zz t)
        zz instructions

    let is_ret = function
        | CIL_instruct(_, _, _, Cili_ret) -> true
        | _ -> false

    let instructions =
        if not (nullp instructions) && (tailhangf || disjunctionate is_ret (tl(rev instructions)))
        then redirect_returns(instructions)
        else instructions

    let swaplast l v = rev((rev v) @ tl(rev l))
    let remove_trailing_return = function
          | [] -> []
          | l -> if is_ret(hd(rev l))
                 then rev(tl(rev l)) 
                 else l
    let instructions =
          if tailhangf && not(nullp instructions)
          then swaplast instructions 
                            [ (* done by redirect .. gec_ins(Cili_lab rlab), *)
                              gec_ins(Cili_call(KM_call, CK_default false, CTL_void, (Cil_cr_idl[ "Kiwi"; "KiwiSystem" ], "Pause", []), [], funique "ZZtailhang"));
                              gec_ins(Cili_br rlab)
                            ]
          elif keep_return then instructions
          else remove_trailing_return instructions


    let is_eobb = function // Miscellaneous end-of-basic-block instructions. cf unconditional_jmp_pred
        | CIL_instruct(_, _, _, Cili_ret)
        | CIL_instruct(_, _, _, Cili_throw)
        | CIL_instruct(_, _, _, Cili_leave _)
        | CIL_instruct(_, _, _, Cili_endfinally) 
        | CIL_instruct(_, _, _, Cili_endfault)    -> true                        
        | arg                                     -> not_nullp(labscan2 (fun x->x) arg [])

    let ia:ia_t = Array.create (ssize+1) ([], [])
    let m_pos = ref 0
    let inc() = (mutinc m_pos 1; ())
    // Insert linear code into basic block array, tagged with handler contexts.
    // Insert instructions in array, starting a new block when a label or try block is encountered.
    // Note fall through a conditional branch also starts a new block without needing a label.
    // Where do we make the explict jump that replaces a fallthrough? Do we just rely on ascending order of bpc?
    let _ =
        let m_local_handler_info:(string * exn_handler_desc_t) list ref = ref []
        let from_tok tok =
            match op_assoc tok !m_local_handler_info with
                | None ->  sf (sprintf "firstpass: No exception handler details to hand for tok %s" tok)
                | Some handler -> handler
        let insert1 handler_toks_lst x =
            let _ = vprintln // 0 ("Insert ins " + cil_classitemToStr "" x (" at " + i2s !pos))
            let (handlers, insl) = ia.[!m_pos]
            let handlers =
                if nullp handler_toks_lst then handlers
                elif nullp handlers then map from_tok handler_toks_lst
                elif map f1o3 handlers <> handler_toks_lst then sf ("handlers in scope changed mid basic block")
                else handlers
            Array.set ia (!m_pos)  (handlers, insl @ [x])        

        let rec insert cg = function
            | h::tt ->              
                let (handler_toks_lst, handler_defo) =
                    match h with
                        | CIL_instruct(_, handler_toks_lst, _, Cili_kiwi_trystart handler) -> (handler_toks_lst, Some handler)
                        | CIL_instruct(_, handler_toks_lst, _, _ )                         -> (handler_toks_lst, None)                        
                        | ins                                                              -> ([], None)
                        //| ins                                                            -> muddy ("L3408 " + cil_classitemToStr "" ins "")
                let labf = islab h || not_nonep handler_defo
                let _ = if not_nonep handler_defo then mutadd m_local_handler_info (f1o3 (valOf handler_defo), valOf handler_defo)
                (if labf && cg then inc();
                 insert1 handler_toks_lst h; 
                 if is_eobb h then (inc(); insert false tt) else insert (cg || not labf) tt
                )
            | [] -> if not_nullp (snd ia.[!m_pos]) then inc()
        insert false instructions

    let il = !m_pos
    if il=0 then vprintln 3 (sprintf "*** Warning, method %s function has no instructions in its body" id)

    let _ =
        if cCC.settings.early_cil_dumpf
        then
            let avd = 0 (* We dont need to print this normally since printed again post spillage insert: 'late' mode. *)
            let disp1 ins = vprint avd (" ::: " + cil_classitemToStr "" ins "")
            let rec gen_post_spill_listingx(pos) =
                if pos >= il then vprint avd (id + "--A--early------------------- END\n")
                else
                    vprintln avd (id + "--A---------------------" + i2s pos)
                    app disp1 (snd ia.[pos])
                    vprintln avd ""
                    gen_post_spill_listingx(pos+1)
            gen_post_spill_listingx 0             
    (ia, il)


//
// Usual wrapper for netgen - create an hpr net from a ce net and store it in the singleton heap.
// TODO explain why we need the core and the wrapper separately.
// lnetget takes a CE_var and returns an hexp_t equivalent.
// Why is this not the same as ceToNet?  Seems a bit odd?    
let lnetgen ww io_group_ ce id_suffix_option =
    let cCC = valOf !g_CCobject
    match ce with

    | CE_var(vrn, dt, idl, fdto, ats) ->
        let mx = "lnetgen"
        match cCC.heap.sheap2.lookup vrn with
            | Some she -> 
                //vprintln 0 (sprintf "lnetgen she.ats are %A" (sfold kk_ats she.ats))
                match !she.hexp with
                    | Some net -> net
                    | None ->
                        let username = op_assoc "username" ats
                        let ids =
                            if username<>None && String.length(valOf username) > 0 then valOf username else hptos_net idl
                        // We do not envisage a username and a suffix being needed ... perhaps support.
                        let ids = if nonep id_suffix_option then ids else ids + valOf id_suffix_option
                        let rec ton_dims arg =
                            match arg with
                            | CTL_net _ | CTL_void -> None
                            | CT_arr(dt, len) -> len
                            //| CT_brand(_, dt)
                            | CT_star(_, dt) ->
                                let _ = vprintln 3 (mx + " star dropped!")
                                ton_dims dt
                            | CTL_record _ -> None
                            | dt ->
                                let _ = sf ("+++ton_dims other (lnetgen) " + typeinfoToStr dt)
                                None
                        let net = netgen_kernel ww mx (ids, dt, None, ats)
                        she.hexp := Some net
                        net
            | None -> sf (sprintf "lnetgen: missing net under lookup by vrn on " + ceToStr ce)

    | other -> sf ("lnetgen other form: " + ceToStr other)



#if SPARE
(* OLD CODE DELETE
 * Scan generate list of labels that are branch sources, cond or uncond jumps.
 *) 
let rec scan_srcs_ (pc, ia:ia_t, il) c =
    if pc=il then c
    else
        let ins = ia.[pc]
        let dest = List.foldBack (labscan00 (fun x->x)) ins []
        let c' = if dest<>[] then pc::c else c 
        scan_srcs_ (pc+1, ia, il) c'

(* OLD CODE DELETE
 * Scan generate list of labels that are destinations. 
 *) 
let rec scan_dests_ mapping (pc, ia:ia_t, il, c) =
    if pc=il then c 
    else
    let ins = ia.[pc]
    let newv = map (a_assoc "scandests missing lable" mapping) (List.foldBack labscan1c ins []) 
    scan_dests_ mapping (pc+1, ia, il, newv @ c) // Please code as fold
#endif


let unconditional_jmp_pred = function
    | Cili_leave _
    | Cili_endfinally _
    | Cili_endfault _        
    | Cili_br _
    | Cili_ret -> true

    //| Cili_leave s -> true // We used to treat leave blocks by assuming there is only one and falling into it, which meant treating this instruction as a nop, but not any longer
    | _ -> false



let unconditional_jmp1 = function
    | CIL_instruct(_, _, _, v) -> unconditional_jmp_pred v
    | _                     -> false


let rec get_ce_content_type msg = function
    // We support a few special cases here - perhaps they do not need special treatment
    | CE_region(uid, dt, ats, nemtok, _) -> dt // LOOKS WRONG: This should be CT_arr(adt, ...) or CT_record(...). It should not be adt.  
    | CE_subsc(adt, ce, idx, aid) ->
        let rec desubsc = function
            | CT_star(_, dt) -> desubsc dt // ignore stars
            | CT_arr(content_type, _) -> content_type // Dereference once only to get content type.
        desubsc adt

    | CE_dot(ttt, ce, (_, fdt, s, dt_field, n), aid) -> dt_field // One indirection - perhaps trim stars here too.
    
   
    | arg -> // general case
        let dt = get_ce_type2 msg "get_ce_content_type" arg
        let content_type = get_content_type_from_type msg dt
        content_type



// Decompose an array type, giving its conent type and length option.
// Returned units are number of locations.
let rec array_both_from_type ww a0 =
    match a0 with
        | CTL_record(idl, cst_, items, len, _, binds, _) ->
            match items with
                //| [(fid, ty)] -> 
                | aux_ -> sf ("array_both_from_type other record format contents:" + cil_typeToStr a0)
        | CT_star(_, dt) -> array_both_from_type ww dt
        | CT_arr(ct, len) -> (ct, len)
        | other -> sf ("array_both_from_type other:" + cil_typeToStr other)

let array_item_size_in_bytes ww msg dt  =
    let (ct, len) = array_both_from_type ww dt
    let bits = ct_bitstar ww msg 0 ct
    (bits + 7L)/ 8L


//
// Array helper, returns both type and length of an array.
// The returned units are number of locations.
//
let rec array_both_from_ce ww ce = 
    let xm ss = () //  vprintln 0 ss
    //xm (sprintf "array_both_from_ce of %s start" (ceToStr ce))
    match ce with
        | CE_conv(ty, cvtf_, arg) ->
            let (old_type_, len) = array_both_from_ce ww arg // Dive down and get the real length from underneath.
            // Here we get it from the type but getting it from ce makes more sense... TODO
            let (ct, len2_) = array_both_from_type ww ty
            if nonep len then
                vprintln 3 (sprintf "Outer len %A ignored despite inner len is None" len2_)
            else
                vprintln 3 (sprintf "Ignore outer len in conv %A since inner len present and is %i" len2_ (valOf len))
            let ct = get_ce_content_type "array_both_from_ce" ce
            (ct, len) 

        | CE_x(ty, W_string(s, _, _))  ->
            xm "array_both_from_ce: get from string constant length"
            (ty, Some(int64 (strlen s)))
        | CE_x(ty, X_bnet ff) when ff.is_array ->
            xm "array_both_from_ce: get from bnet attributes"
            let f2 = lookup_net2 ff.n
            (ty, Some(hd f2.length))
        | CE_x(ty, vale)        -> sf ("ce_arraylen yb other:" + ceToStr ce + " " + xToStr vale)
        | CE_star(_, ce, aid)   ->
            xm "discard stars"
            array_both_from_ce ww ce 

        | general_case ->
            let dt = get_ce_type "array_both_from_ce" ce
            xm "array_both_from_ce: default action: get from type"
            array_both_from_type ww dt

//
// 1-D native array - get length
//   
let ce_arraylen ww ce =
    let (ct_, len) = array_both_from_ce ww ce
    match len with
        | None ->
            vprintln 3 ("builtin ce_arraylen: other form for arg: return symbolic: arg=" + ceToStr ce)
            CE_apply(CEF_bif g_bif_strlen, g_null_callers_flags, [ce]) // Note: this calls strlen which may not be appropriate for all arrays.
            
        | Some len -> gec_yb(xi_int64 len, valOf g_bif_strlen.rt)


let rec ty_mark_strip (ns, nb) = function                     // discard stars and valuetypes here. cf grossly etc..
    | CT_star(n, t)   -> ty_mark_strip(ns+n, nb) t
    //| CT_valuetype(t) -> ty_mark_strip(ns, true) t
    | ot -> (ot, ns, nb)


// Array decay - an array without subscript used as a value decays ...
//   Keep the variable as a pointer when encountering the following resolution
//      f_dt=    *(CTL_net(false, 8, Signed,[native]))
//     a_dt=CT_arr(CTL_net(false, 8, Signed,[native]), 7)
// or  a_dt=CT_star(1, CT_arr(CTL_net(false, 8, Signed,[native]), 7))
let array_decay_to_pointer_override formal_dt actual_dt =
    match (formal_dt, actual_dt) with
        | (CT_star(ns, ct), CT_star(1, CT_arr(act, len))) when ns < 0 ->
            let _ = vprintln 3 (sprintf "using array_decay_to_pointer_override as pointer 1/2 " + (cil_typeToStr (CT_star(ns, ct)))) // formal_dt 
            CT_star(ns, ct)

        | (CT_star(ns, ct), CT_arr(act, len)) when ns < 0 ->
            let _ = vprintln 3 (sprintf "using array_decay_to_pointer_override as pointer 2/2 " + (cil_typeToStr (CT_star(ns, ct)))) // formal_dt 
            CT_star(ns, ct)
        | _ ->

            actual_dt


// Implement a singleton variable, such as a field from a class, a reftran register, a stack actual, spill or local variable.
// We prefer to do this lazily, on demand, to avoid littering the output file with every static var defined in every dotnet and Kiwi library that is accessed.
// This performs scone expanding of structures since singleton valuetypes are not held packed.
// Needs to be called for more than just stack vars - needed for all sconed structs.
let gec_singleton ww vd cCBo msg purpose ats realdo m0 tag_or_nullstring fix dt io_group_opt atakenf str_fd_o =
    let cCC = valOf !g_CCobject
    let idl = if tag_or_nullstring = "" then fix else tag_or_nullstring::fix 
    let buz_stackvar ats ethereal part_of_structf dt idl =
        let storemode =   // Structs are put in heap space to satisify simpleminded repack predicates - perhaps this is an old comment - Singleton structs should just be collections of scalar owing to scone processing.
                          // 
            if part_of_structf then STOREMODE_singleton_vector
            else match dt with // We declare as statics but we rename them to handle reentrant to static depth.
                    | CTL_record(record_idl, cst, lst, len, ats, binds, srco) when cst.structf -> STOREMODE_singleton_vector
                    //| CT_valuetype(CTL_record(record_idl, cst, lst, len, ats, binds, srco)) -> STOREMODE_singleton_vector
                    | _ ->  STOREMODE_singleton_scalar // we call this a singleton even in recursive subroutines since the nesting depth is baked into the sheap_id name.

        let sheap_id = hptos idl
        let (vrn, blobf, cez) =
            match cCC.heap.sheap1.lookup sheap_id with
                | Some she -> (she.vrn, she.labelled_literal, she.cez)
                | None ->
                    let vrn = alloc_fresh_vreg ww cCBo realdo purpose dt [tag_or_nullstring] storemode
                    let blobf = false
                    let ((blobf, cez), _) = gec_uav_core ww vd m0 ethereal atakenf storemode realdo blobf dt (sheap_id, ats) vrn io_group_opt (*envo=*)None str_fd_o
                    (vrn, blobf, cez)
        let msg = msg + sprintf ": gec_singleton def: vrn=V%i ce=%s idl=%s dtype=%s storemode=%A atakenf=%A" vrn (ceToStr cez) (hptos idl) (cil_typeToStr dt) storemode atakenf
        vprintln 3 msg
        (blobf, cez, (hd idl, dt, Some cez), vrn)


    let ats_update_username tag ats = // Add user tag into username for a struct.
        let rec scan = function
            | [] -> []
            | (k,v)::tt when k=g_username -> (k, v + "_" + tag)::tt
            | other::tt -> other :: scan tt
        scan ats

    let rec perhaps_struct_declare ats part_of_structf full_idl dt = 
        //vprintln 3 (sprintf "perhaps_struct_declare: full_idl=%s dt=%s" (hptos full_idl) (cil_typeToStr dt))
        let rec raz dt = 
            match dt with
                | CT_knot _ -> raz (follow_knot msg dt) 
                | CT_cr crst when is_structsite msg dt && not(dt_is_enum dt) -> // The dig out of unwind of CT_cr should be done by a follow_know variant...
                    let tycontrol =
                        {
                            grounded=true
                        }
                    let tcn = (tycontrol, Map.empty)
                    vprintln 3  (msg + sprintf " ungrounded/un-normed class used as singleton %s. cil_type_n needed." (cil_typeToStr dt))
                    let dt = cil_type_n ww tcn dt
                    match dt with
                        | CTL_record(idl, cst, lst, len, ats, binds, _) -> raz dt // Now grounded, go round again.
                        | other ->
                            sf (msg + sprintf " ungrounded class used as singleton %s" (cil_typeToStr other))

                | CTL_record(record_idl, cst, lst, len, ats_ignored__, binds, srco) when cst.structf -> 

                    let (blobf_, cez_, (idx_, dt_, cezo_), vrn) = buz_stackvar ats true part_of_structf dt full_idl
                    let struct_declare1 (crt:concrete_recfield_t) =
                        let ats = ats_update_username crt.ptag ats
                        perhaps_struct_declare ats true (crt.ptag :: full_idl) crt.dt 
                    let (_, _, struct_items, vrns) = unzip4(map struct_declare1 lst)
                    let idtoken = funique "SID"
                    //let _ = vprintln 0 (msg + sprintf "Struct scone items idtoken=%s to declare are %A" idtoken struct_items)
                    let cez = CE_struct(idtoken, dt, idtoken, struct_items)
                    // This is correctly recursive - structs containing structs are processed in the self call to perhaps_struct_declare.
                    (false, cez, (hd full_idl, dt, Some cez), vrn) // Return item in two different ways and caller can choose which to use.

                | dt1 ->
                    //dev_println (sprintf "gec_singleton: L3691-exams %A" part_of_structf)
                    buz_stackvar ats false part_of_structf dt1 full_idl
        raz dt 
    let (blobf, cez, _, vrnl) = perhaps_struct_declare ats false idl dt
    (blobf, cez, vrnl)



    
// eof
