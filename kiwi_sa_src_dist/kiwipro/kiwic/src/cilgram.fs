//  
//  Kiwi Scientific Acceleration Project - AST form for parsed dotnet bytecode.
//  Kiwi Project: KiwiC compiler.
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

module cilgram

open yout
open moscow
open hprls_hdr

type callsite2_id_t = string

type vt_style_t =
    | K_uint
    | K_int
    | K_float


type stringl = string list

// Provenience (for branding) and other additional cil info.
// We want to tag/brand each major storage class so that a name alias on one does not collide with another: this
// is a bit crude but is sufficient to make the existing testsuite pass while allowing statics to be referenced.
type scl_t =
       | CV_formal of int
       | CV_local of int  // aka stack
       | CV_sfield
       | CV_heap // aka heap?
       | CV_none // THESE ARE NEVER ADDRESS TAKEN - could use cez for them!
       | CV_object
       | CV_mutex
       | CV_const
       | CV_brand of string

type enum_t = string option * (int64 * string * string) list


type wasNorm_t = stringl

type clknets_t = edge_t list * dir_reset_t list * hexp_t list

type ctvar_t =
    {
        methodf: bool; // if clear then a class template var.
        id: string option;
        idx: int option;
    }

let g_def_CTVar = { id=None; idx=None; methodf=false; } // Better as idx:(int * bool) option since methodf applies only to the idx field

type cil_template_item_t =
      | Cil_tp_arg of bool * int * string
      | Cil_tp_type of ciltype_t


// The CTL_ forms are grounded types used both by the parser/front end and in intermediate code. Other Ciltype forms are mapped to CT_ forms in later processing.
and typeinfo_t = ciltype_t


and dropping_t = (string list * dropping_data_t)

and dropping_data_t =
    | RN_call of dropping_data_t * dropping_data_t * dropping_data_t list * dropping_data_t list * dropping_data_t list * dropping_data_t list // instance type (was none for C++ global method but now not optional), return type, method generics (cgr for .ctor), arg types, class generics, method generics.
    | RN_monoty of ciltype_t * (string * string) list
    | RN_duplex of dropping_data_t * dropping_data_t * (*ptag*) string
    | RN_tmp_id of string

        
and cref_struct_t = // A class reference - with type actuals.
    {
      name:         stringl
      cl_actuals:   dropping_data_t list    // TODO these actuals would be more convenient as types. Recode please.
      meth_actuals: dropping_data_t list  // You would expect this field is never non-null - a class reference does not have method actuals! But we do pop them in here at one point. In other places we pass them separately. Please tidy.
      volatilef:    bool
      ats:          (string * string) list // Attributes      
      crtno:        typeinfo_t             // This should always be the corresponding CT_class class_struct_t (containing, for instance, the generic formal names).  Not sure I like this owing to recursive walks needed to rehyd and recursive types and so on 
      //enumf: bool; // Is enumeration type.
    }

and class_struct_t = 
    {   name:              stringl
        structf:           bool  // True for a struct (which is a valuetype in C#), false for an object;
        ats:               (string * string) list // Attributes
        // ? what about implemented interfaces?
        parent:            ciltype_t option
        newformals:        string list
        whiteformals:      (string * ciltype_t) list // Mapping from old formals to new - needed for the CIL instruction code which is not rewritten.
        // Unannotated fields and methods are held in the 'firstpass' database
        // but the type specialisations for any callstring are stored as droppings here.
//        fields:   stored in the No_class form, not here.
        class_item_types:  (string list * dropping_data_t) list // i.e. dropping_t list
        class_tag_list:    (string * string) list   // Presentation (user-visible) and unique versions of each tagname. (ptag * utag) list.
        arity:             int        // Number of type formals.
        m_next_vt:         int ref    // Next number to allocate for this class tree - field only used when parent=None
        vtno:              int option // Our virtual table number (used for dynamic poly tags not for method pointers (at the moment))
        sealedf:           bool       // When sealed and not part of a hierarchy then no virtual table will be needed.
        remote_marked:     (string * hexp_t) list option    
    }

and field_struct_t = 
    {   ptag:             string // Presentation or public name
        utag:             string // Unique name
        fld_parent_class: (stringl * class_struct_t option ref)
        gamma:            dropping_t list
        volf:             bool
        static_cache_f:   bool
        ct:               ciltype_t
    }


and arg_formal_t =           string option * int option * typeinfo_t * string (* Fields are: RPC binding, Position/index, type, name *)
and cil_formal_t = Cil_fp of string option * int option * ciltype_t * string // Same thing.


and method_struct_t = 
    {   name:            stringl                   // Full hierarchic name
        uname:           string                    // Unique local name (for this compilation)
        rtype:           ciltype_t                 // Return type
        arg_formals:     arg_formal_t list
        ty_formals:      ciltype_t list            // Method generics (mgr)

        // ? what about implemented interfaces?
        meth_parent_class:    (stringl * class_struct_t option ref)
        // Unannotated fields and methods are held in the 'firstpass' database
        // but the type specialisations for any callstring are stored as droppings here.
//        fields:  
//        methods:
        gamma:            dropping_t list
        ataken_formals:   int list
        ataken_locals:    int list
        mutated_formals:  int list
        has_macro:        bool
        has_macro_regs:          bool
        is_static:               bool
        is_ctor:                 int        // Is non-zero for constructor methods
        is_remote_marked:        ((string * hexp_t) list * (hfast_prams_t * string) * bool * bool * fun_semantic_t) option    
        is_accelerator_marked:   (string * hexp_t) list option
        is_root_marked:          (string * hexp_t) list option
        is_fast_bypass:          (string * hexp_t) list option
        is_hpr_prim:             bool           // Whether hardcoded somewhere in the compiler.
        sri_arg:                 string option  // Whether one of the arguments becomes an shared resource. 
        fu_arg:                  string list option  // Whether maps to an IP block - if so this is the fu_kind.
        fsems:                   fun_semantic_t option
        clknets:                 clknets_t     // The clocks and resets markup (for an entry point only)
    }



and concrete_recfield_t =
    {
        no:              int                  // Record no, ordering from zero, in a CE_struct representation.
        ptag:            string               // Original/presentation tag/field name.
        utag:            string               // So loops in datastructures can be found easily, we uniquify each user tag
        dt:              typeinfo_t
        pos:             int64                // byte offset
        size_in_bytes:   int64
        fap__:           typeinfo_t list      // Not used.
        fld_st_o:        field_struct_t option
    }


and net_at_flags_t =
    {
        nat_enum:    enum_t option
        nat_char     :bool               // A char is a special variation on a short (int16).
        nat_string   :bool
        nat_IntPtr   :ciltype_t option   // An IntPtr is a special variation on an integer that masks an underlying type
        nat_native   :bool               // A type that can be converted directly to hpr hexp_t form? Or a member of System that can be unboxed (object and string are the tricky ones)
        nat_hllreset: bool              // An indication of a default clear-to-zero value that is distinct from an explicit zero.
        //xnet_io:   varmode_t          // Input/output net designation
    }


// Types.
//   Those starting Cil_ are encountered in input parse trees from the PE assembly code.
//   They are converted to the CT_ or CTL_ primary forms.  The CTL_ prefix indicates a grounded type: that is, a type with no generic or size parameters.     
and ciltype_t =
    | CT_knot of string list * string * ciltype_t option ref



// Parse-tree forms
    | Cil_cr_flag of  cilflag_t * ciltype_t
    | Cil_cr of string
    | Cil_cr_idl of string list // string list is reversed components of hierarchic name, as rendered correctly by hptos
    | Cil_lib of ciltype_t * ciltype_t
    | Cil_namespace of ciltype_t * ciltype_t       
    | Cil_cr_dot of ciltype_t * string
    | Cil_cr_slash of ciltype_t * string

    | Cil_cr_templater of ciltype_t * cil_template_item_t list
    | Ciltype_array of ciltype_t * int option

    | Cil_cr_global // Used when there is no surrounding class for C++ input.


    // These four or five are just parse-tree aliases for System.Blah and should perhaps be left out entirely but they do make the asmout output more readable.
    | Ciltype_int 
    | Ciltype_bool
    | Ciltype_char
    | Ciltype_string
    | Ciltype_decimal    


    | Ciltype_ellipsis
    

    | Ciltype_star of ciltype_t
    | Ciltype_ref of ciltype_t
    | Ciltype_instance of ciltype_t
    | Ciltype_modopt of ciltype_t * ciltype_t
    | Ciltype_modreq of ciltype_t * ciltype_t
    

    | Cil_suffix_ref
    | Cil_suffix_i
    | Cil_suffix_i1     | Cil_suffix_i2     | Cil_suffix_i4     | Cil_suffix_i8
    | Cil_suffix_u
    | Cil_suffix_u1     | Cil_suffix_u2     | Cil_suffix_u4     | Cil_suffix_u8
    
    | Cil_suffix_r4     | Cil_suffix_r8

    | CT_arr of typeinfo_t * int64 option  // Array: content type and length of its 1-D dimension (length=None if unconstrained). A star is needed for an array handle.  The parents of an array are implicitly in g_CT_arr_parents

    | CTVar of ctvar_t                     // Generic type variable
    | CT_cr of cref_struct_t               // A class instance with bindings and modifier flags. No star needed. Or else a structure site.
    | CT_class of class_struct_t           // A class without bindings. Generally just part of a No_class but a star is added sometimes when used ... please clarify.
    | CT_method of method_struct_t // This method may have unbound generics whereas as CTL_method_handle does not.
    | CT_list of ciltype_t list
    | CT_star of int * typeinfo_t   // Pointer and reference handles around a type. Positive n is printed as & and negative as *.  We don't support pointers to references etc. in this simple approach.
    
// Grounded types
    | CTL_vt of vt_style_t * int * ciltype_t                // CTL_vt is an older form for CTL_net that we can remove in future.
    | CTL_net of bool * int * netst_t * net_at_flags_t  // primitive valuetypes are stored using CTL_net. Components are volatile, width, signed, atts.  We should be able to work out volatility for ourselves in later processing but currently rely on source annotations.
    | CTL_void
    | CTL_reflection_handle of string list  // A fancy alias that converts to CIL's IntPtr when needed at runtime



    //| CTL_object        // An object reference. No star needed. TODO yet one added in gen_dtable in two places!  We don't really need this - could just be a star applied to a CTL_record with null contents? Its also an IntPtr and a (void *) so please unify.
    // Now replaced with g_ctl_object_ref 


    | CTL_record of     // CTL_record/CTL_struct is the structural form for a bound class or struct. Named as CTL_record since always grounded.  A star is needed for a record (instance of a class) handle.
         stringl *                     // Name - will the same as cst.name when no generics, but can be extended as squirrelled idl in general.
         class_struct_t *              // cst - class struct
         concrete_recfield_t list *
         int64 *                       // total size in bytes
         (string * string) list *      // ats
         (string * ciltype_t) list *   // binds needed for method processing.
         wasNorm_t option              // Source code

//  | CTL_wond of string // Deprecated: to CT_arr(type, None) but still used for Cil_suffix_ref which should pehaps there be called CT_typeany
    

// Type modifiers
    //| CT_brand of scl_t * typeinfo_t
    //| CT_valuetype of typeinfo_t // Wrapper to denote passing by value of classes (i.e. structs).

    
    | Ciltype_filler  // only for debug

    // We sometimes want to force ciltype_t to be incomparable since mono can easily get stack overflow where we use built-in equality on them.
    //| Ciltype_non_comp of (int -> int)



and cilflag_t = 
       | Cilflag_enum
       | Cilflag_extern
       | Cilflag_valuetype 
       | Cilflag_newslot
       | Cilflag_auto
       | Cilflag_interface
       | Cilflag_runtime
       | Cilflag_synchronized
       | Cilflag_family
       | Cilflag_final
       | Cilflag_virtual
       | Cilflag_abstract
       | Cilflag_private
       | Cilflag_public
       | Cilflag_initonly
       | Cilflag_cil
       | Cilflag_managed
       | Cilflag_hidebysig
       | Cilflag_specialname
       | Cilflag_rtspecialname
       | Cilflag_instance
       | Cilflag_default
       | Cilflag_ansi
       | Cilflag_static // Aka Cilflag_class - dotnet calls these 'class methods'
       | Cilflag_pinned
       | Cilflag_strict
       | Cilflag_literal
       | Cilflag_field
       | Cilflag_partial
       | Cilflag_assembly
       | Cilflag_nested
       | Cilflag_explicit
       | Cilflag_sealed
       | Cilflag_sequential
       | Cilflag_serializable
       | Cilflag_beforefieldinit
       | Cilflag_unsafe       

       | Cilflag_autocons of int (* indicates to substitute a nop function instead of a constructor  *)
       | Cilflag_hprls         // KiwiC  built-in type or method - please rename as Cilflag_KiwiC.
       | Cilflag_hpr of string // HPR library built-in type or method or other marker.      



type cilexp_t =
      | Cil_explist of cilexp_t list
      | Cil_blob of cilexp_t list      
      | Cil_float of int * string
      | Cil_id of string
      | Cil_dotpath of cilexp_t * string
      | Cil_float32 of float
      | Cil_float64 of double
      | Cil_number32 of int
      | Cil_int_i of int * System.Numerics.BigInteger    
      | Cil_tnumber_end of bool
      | Cil_tnumber of int * cilexp_t
      | Cilexp_caster of ciltype_t * cilexp_t
      | Cil_string of string
      | Cil_nativetypedata of ciltype_t

type cilarg_t =
    | Cil_argexp of cilexp_t
    | Cil_suffix_0
    | Cil_suffix_1
    | Cil_suffix_2
    | Cil_suffix_3
    | Cil_suffix_4
    | Cil_suffix_5
    | Cil_suffix_6
    | Cil_suffix_7
    | Cil_suffix_8
    | Cil_suffix_m1
    | Cil_suffix_M1


type ct_rec_name_t = 
    | RN_idl of string list
    // TODO these two are the same now and dropping_t has a silly wrapper that this needs flatten into
    | RN_type of typeinfo_t
    | RN_filler


type cil_unsigned_t = Cil_signed | Cil_unsigned


type cil_versionno_t = Cil_version_no of int * int * int * int


type cil_fieldref_t = ciltype_t * string * ciltype_t list // Three fields are: class type, field name, generic fap.


type cil_callarg_t = string option * ciltype_t

// CIL has four method calling styles
//   Static Method:    call       void foo.boz::callee()
//   Instance Method:  call       instance void foo.boz::callee()
//   Virtual Method:   callvirt   instance void foo.boz::callee()
//   Global Function:  call       void foo.callee()

// We have a different set of four here!
type cil_callkind_t =
    | CK_vararg   of bool
    | CK_default  of bool  // The bool on each of these holds for an instance method (where arg0 is the this pointer) and is clear for a class (aka static) method.
    | CK_generic  of bool
    | CK_instance of bool


let cil_ckToStr kind = (sprintf "%A" kind).[3..]    



type cil_callmode_t =  // Static or dynamic method lookup determiner (for instance calls only of course).  Probably only C++ lets the HLL user chose between these?
    | KM_virtual
    | KM_call


type cilinst_prefix_t =
       | Cilip_unaligned // Prefix: Subsequent pointer instruction might be unaligned.
       | Cilip_volatile // Prefix: Subsequent pointer reference is volatile.
       | Cilip_readonly // Prefix: Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer.
       | Cilip_tail     // Prefix: tail call
       | Cilip_constrained of ciltype_t * cil_fieldref_t // Prefix: Call a virtual method on a type constrained to be type T. fieldref is spurious.

type cil_field_at_t =
    | Cil_field_none
    | Cil_field_at of cilexp_t 
    | Cil_field_lit of cilexp_t 
    | Cil_field_filler

type cil_method_flags_t =
    {
        is_startup_code:bool
    }


type handler_token_t = string



type exn_handler_desc_t = handler_token_t * (ciltype_t * int option * cil_t list) list * (int option * cil_t list)
    
and cilinst_t = 
       | Cili_prefix of cilinst_prefix_t 
       | Cili_nop
       

       | Cili_comment of string
       | Cili_switch of string list
       | Cili_ldnull
       | Cili_dup
       | Cili_pop
       | Cili_ret
       | Cili_mul of cil_unsigned_t
       | Cili_sub of cil_unsigned_t
       | Cili_add of cil_unsigned_t
       | Cili_div of cil_unsigned_t
       | Cili_rem of cil_unsigned_t
       | Cili_neg // 0x65 negate - subtract from zero
       | Cili_and
       | Cili_or 
       | Cili_xor
       | Cili_not // 0x66 bitwise one's complement
       | Cili_shl of cil_unsigned_t
       | Cili_shr of cil_unsigned_t

       | Cili_br of string
       | Cili_brfalse of (string list * string) // The first string list is a Kiwi extension here so that nullcheck on a storeclass not containing null, but where repack may use zero as a value, can be supressed.
       | Cili_brtrue of string
       | Cili_leave of string
       | Cili_endfinally
       | Cili_endfault

       | Cili_beq of string
       | Cili_bne of string

       | Cili_ble of string
       | Cili_bge of string
       | Cili_blt of string
       | Cili_bgt of string

       | Cili_ble_un of string
       | Cili_bge_un of string
       | Cili_blt_un of string
       | Cili_bgt_un of string

       | Cili_ceq 
       | Cili_cne 
       | Cili_cle of cil_unsigned_t
       | Cili_cge of cil_unsigned_t
       | Cili_clt of cil_unsigned_t
       | Cili_cgt of cil_unsigned_t

       | Cili_ldobj of ciltype_t
       | Cili_stobj of ciltype_t
       | Cili_throw



       | Cili_isinst of bool * ciltype_t // If the Kiwi-extension bool holds we need an exact match, not a cast to type. 
       | Cili_box_any of ciltype_t
       | Cili_unbox_any of ciltype_t
       | Cili_box of ciltype_t
       | Cili_unbox of ciltype_t
       | Cili_newarr of ciltype_t
       | Cili_refanyval of ciltype_t
       | Cili_initobj of ciltype_t
       | Cili_newobj of cilflag_t list * string * ciltype_t * cil_fieldref_t * cil_callarg_t list * callsite2_id_t
       | Cili_castclass of ciltype_t
       | Cili_ldftn of bool (* holds for ldvirtftn *) * string * ciltype_t * cil_fieldref_t * cil_callarg_t list * callsite2_id_t
       | Cili_ldtoken of ciltype_t * cil_fieldref_t option // It is  type, field or method reference. We use fieldref for the methodref at the moment



       | Cili_arglist
       | Cili_initblk  // unsafe copy: three args on stack: memset(dest, val, length in bytes).
       | Cili_cpblk    // unsafe copy: three args on stack: memcpy(dest, src, length in bytes).
       | Cili_ldc of ciltype_t * cilarg_t
       | Cili_ldind of ciltype_t
       | Cili_ldstr of string
       | Cili_ldarg of cilarg_t
       | Cili_ldloc of cilarg_t
       | Cili_ldelema of ciltype_t
       | Cili_ldelem of ciltype_t
       | Cili_stelem of ciltype_t
       | Cili_ldlen
       | Cili_conv of ciltype_t * bool // Bool set if overflow_trapping form


       | Cili_ldarga of cilarg_t
       | Cili_ldloca of cilarg_t

       | Cili_stc of cilexp_t
       | Cili_stind of ciltype_t

       | Cili_ldfld of ciltype_t * cil_fieldref_t
       | Cili_ldflda of ciltype_t * cil_fieldref_t
       | Cili_ldsfld of ciltype_t * cil_fieldref_t
       | Cili_ldsflda of ciltype_t * cil_fieldref_t
       | Cili_stsfld  of ciltype_t * cil_fieldref_t
       | Cili_stfld  of ciltype_t * cil_fieldref_t

       | Cili_ststr of string
       | Cili_starg of cilarg_t
       | Cili_stloc of cilarg_t

       | Cili_call of cil_callmode_t * cil_callkind_t * ciltype_t * cil_fieldref_t * cil_callarg_t list * callsite2_id_t

       | Cili_lab of string

       | Cili_kiwi_trystart of exn_handler_desc_t
       | Cili_kiwi_special of string * bool * ciltype_t

and cilp3_t = P3 of int option * ciltype_t * string


and cil_t =
    | CIL_lazybones of string * string * string * cil_t option ref
    | CIL_method of string * (ciltype_t * cil_template_item_t list * ciltype_t list) * cilflag_t list * cil_callkind_t * ciltype_t * (ciltype_t * string) * cil_formal_t list * cil_method_flags_t * cil_t list
    | CIL_amethod of string * cilflag_t list * string * (ciltype_t * string) * cil_fieldref_t * cil_formal_t list
    | CIL_field of cilexp_t option * cilflag_t list * ciltype_t * ciltype_t * cil_field_at_t * cil_t list
    | CIL_comment of string
    | CIL_maxstack of cilexp_t
    | CIL_param of int * cilexp_t option
    | CIL_instruct of int * handler_token_t list * cilinst_prefix_t list * cilinst_t
    | CIL_entrypoint of string
    | CIL_locals of cilp3_t list * bool
    | CIL_event of cilflag_t list * ciltype_t * string * cil_t list
    | CIL_namespace of cilflag_t list * ciltype_t * cil_t list
    | CIL_assembly of cilflag_t list * ciltype_t * cil_t list
    | CIL_module of string * cilflag_t list * cil_t list
    | CIL_class of string * cilflag_t list * ciltype_t * cil_template_item_t list * ciltype_t option * cil_t list
    | CIL_imagebase of cilexp_t
    | CIL_file of cilexp_t * cilexp_t
    | CIL_override of cil_fieldref_t
    | CIL_stackreserve of cilexp_t
    | CIL_corflags of cilexp_t
    | CIL_subsystem of cilexp_t
    | CIL_publickeytoken of cilexp_t
    | CIL_ver of cil_versionno_t
    | CIL_data of cilexp_t * cilexp_t
    | CIL_custom of string * ciltype_t * cil_fieldref_t * cil_callarg_t list * cilexp_t
    | CIL_hash of cilexp_t
    | CIL_pack of cilexp_t
    | CIL_size of cilexp_t
    | CIL_mresource of cilflag_t list * ciltype_t * cil_t list
    | CIL_property of string * ciltype_t * ciltype_t * cilexp_t list * cil_t list
    | CIL_setget of string * string * cilflag_t list * ciltype_t * cil_fieldref_t * cil_formal_t list
    | CIL_try of int option * cil_t list * exn_handler_desc_t



exception Cilgram of string;

let gen_cil_template_item = function
    | t -> Cil_tp_type t
    //| (other) -> raise (Cilgram "gen_cil_template_item")


type metainfo_t = MIA of string * string


let gec_cil_crl ridl = Cil_cr_idl (rev ridl)

let new_callsite token = funique ("ZZ" + token)


let g_core_object_lib = Cil_cr_dot(Cil_cr "System", "Object")
let g_core_object_path = [ "Object"; "System" ]

let g_canned_object_struct =
    {
        structf=          false
        name=             g_core_object_path
        ats=              []
        parent=           None
        newformals=       []
        whiteformals=     []
        class_item_types= []
        class_tag_list=   []
        arity=            0
        vtno=             None
        m_next_vt=        ref 0
        sealedf=          false
        remote_marked=    None
    }



let g_blank_crefs =
        { name=         []
          ats=          []
          volatilef=    false
          cl_actuals=   []
          meth_actuals= []
          crtno=        CT_class g_canned_object_struct // This CT_class record should be implicit.
        }

let g_ctl_object_ref = CT_cr { g_blank_crefs with name=g_core_object_path; crtno=CT_class g_canned_object_struct; }

type kfec_settings_1_t = // Recipe constants
    {
        d2name: string
        d3name: string
        d4name: string
        reg_colouring: string
        cil_unwind_limit: int
        multid_names: string list               // Multi-dimensional array names
        default_dynamic_heapalloc_bytes: int64  // 
    }

let g_kfec1_obj:kfec_settings_1_t option ref = ref None // Recipe settings


let g_null_net_at_flags =
    {
        nat_enum=     None
        nat_char=     false      // A char is a special variation on a short (int16).
        nat_string=   false
        nat_IntPtr=   None       // An IntPtr is a special variation on an integer that masks an underlying type
        nat_native=   false      // A type that can be converted directly to hpr hexp_t form? Or a member of System that can be unboxed (object and string are the tricky ones)
        nat_hllreset= false      // An indication of a default clear-to-zero value that is distinct from an explicit zero.
        //xnet_io=      LOCAL
    }

let g_native_net_at_flags =   { g_null_net_at_flags with nat_native=true }

     
let g_canned_u64    = CTL_net(false, 64, Unsigned, g_native_net_at_flags)
let g_canned_u32    = CTL_net(false, 32, Unsigned, g_native_net_at_flags)
let g_canned_u16    = CTL_net(false, 16, Unsigned, g_native_net_at_flags)
let g_canned_u8     = CTL_net(false, 8,  Signed, g_native_net_at_flags)

let g_canned_i64    = CTL_net(false, 64, Signed, g_native_net_at_flags)
let g_canned_i64rst = CTL_net(false, 64, Signed, { g_native_net_at_flags with nat_hllreset=true })
let g_canned_i32    = CTL_net(false, 32, Signed, g_native_net_at_flags)
let g_canned_i16    = CTL_net(false, 16, Signed, g_native_net_at_flags)
let g_canned_i8     = CTL_net(false, 8,  Signed, g_native_net_at_flags)

let g_canned_bool   = CTL_net(false, 1, Unsigned, g_native_net_at_flags)
let g_canned_float  = CTL_net(false, 32, FloatingPoint, g_native_net_at_flags)
let g_canned_double = CTL_net(false, 64, FloatingPoint, g_native_net_at_flags)
let g_canned_char   = CTL_net(false, 16, Signed, { g_native_net_at_flags with nat_char=true })


let g_canned_pointer_type = g_canned_u64

(* Should we define a string as a pointer to a char or perhaps a pointer to a
char array ?  Neither, we define it as a char (or unicode) array, which when used
in an expression decays to a pointer to the first element. Except for
address-of (&)  or sizeof expressions where the return type
is different from that operator applied to the decayed value.
*)

// Canned string types:
let f_canned_c16_str n       = CT_star(1, CT_arr(g_canned_char, (if n=0 then None else Some(int64 n)))) // NB: n=0 for unspecified length strings.

// The c8_str does not have a star at the moment - this is only a native type for gcc4cil input ... which we are working on.
let f_canned_c8_str n = CT_arr(g_canned_i8, (if n=0 then None else Some(int64 n))) // NB: n=0 for unspecified length strings.


// IntPtr is a general pointer that behaves like an integer - non poly version
let g_IntPtr = Cil_cr_idl [ "IntPtr"; "System"; ]

let rec gec_CT_IntPtr = function
    | CTL_net(_, _, _, flags) when not_nonep flags.nat_IntPtr -> gec_CT_IntPtr(valOf flags.nat_IntPtr)
    | dt -> CTL_net(false, 64, Signed, { g_native_net_at_flags with nat_IntPtr=Some dt })

let g_canned_IntPtr = gec_CT_IntPtr CTL_void
                                                   
// This simple summation of stars is not good - they should sum on an absolute basis     
let gec_CT_star x = function
    | CT_star(y, t) ->
        let r = x+y
        if r=0 then t else CT_star(r, t) 
    | t -> if x=0 then t else CT_star(x, t)

let sgn x = if x>0 then 1 elif x<0 then -1 else 0

// Remove one will take us closer to zero stars, whether originally +ve or -ve
let remove_CT_star msg = function
    | CT_star(n0, dt) ->
        let n = n0 - sgn n0
        let _ = vprintln 3 (msg + sprintf ": Removed one star from %i to %i" n0 n)
        if n = 0 then dt else CT_star(n, dt)
    | other -> sf (msg + sprintf ": cannot remove_CT_star")

// eof

