// 
// Kiwi Scientific Acceleration: KiwiC compiler.
//
// kcode.fs: Definitions for elaboration data structures.
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

module kcode

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
open cecilfe
open protocols
open ksc

// Active pattern that can be used to assign values to symbols in a pattern
let (|Let|) value input = (value, input)


// Need to move over to having this enabled or its successor:

// Static memory map - nominal flat space before disambiguation under repack recipe stage.
//let smm_tnow = 140
let g_ssm_spillbase   = 1000L
let g_ssm_static_scalar_base  = 30000L
//t g_ssm_heapbase    = 400000L

let g_filesearch_vd = ref 3
let g_ataken_vd = ref   3 // Log-level for nemtok/aid/sc tracking. -1=off, 10=most verbose.

// A concrete class/struct/record is stored as a dtable entry in the following dictionary.
let g_record_table = Dictionary<string, typeinfo_t * int64 * int * (string * int64 option * int) list>() 

let regionToStr n = sprintf "(%i)" n

type linepoint5_t =
    {
        srcfile: string
        codept: (string list * int * int * string ref) option ref
        waypoint: string option ref
        callstring__: string list ref
        linepoint_stack: string list // better as a linepoint5 parent!
    }

type major_hls_mode_t = // TODO: these are not orthogonal yet
    | MM_root
    | MM_aux_cctor_style
    | MM_specific 
    | MM_remote // perhaps a subclass of classical infact
    | MM_classical_hls
    | MM_pipeline_accelerator_hls

let styleToStr hls_style =     
   if hls_style=MM_pipeline_accelerator_hls then "pipelined-accelerator-hls"
   else
       sprintf "%A" hls_style // ... for now

//
// Norm_t: Abstract syntax tree from PE/CIL input, post some normalisation.
//    
type Norm_t =
    | No_list of Norm_t list
    | No_knot of ciltype_t * Norm_t option ref   // For recursive data structures, where the type is a CT_knot
    | No_esc of cil_t
    | No_proxy of string
    | No_method of string * cilflag_t list * cil_callkind_t * (string list * string * string) * method_struct_t * cil_method_flags_t * cil_t list * cil_t list
    | No_class of string list * cilflag_t list * string list * ciltype_t * Norm_t list * (string * string) list
    | No_field of cilexp_t option * cilflag_t list * string * field_struct_t * cil_field_at_t * (string * ((string * hexp_t) list)) list

    | No_filler
    
let g_volat = "volatile"

// Unaddressable is no longer going to be applied to static fields.  Instead, we will have the concept of undecidabley addressed.


let uidToIdl(cid, widx) = (if widx=g_unaddressable then [] else [i2s64 widx; "%%"]) @ cid


//
// Although we generate Cil_cr_flag(Cilflag_valuetype, x)   in cecilfe, we no longer use the CT_valuetype wrapper around types - it defeats pattern matching and is unnecessary.
let gec_valuetype arg =
    match arg with 
    | CTL_net(volf, width, signed, flags) -> arg // Do not wrap a CTL_net with valuetype since always is.
    | x -> x // DISABLED NOW CT_valuetype x

type nemtok_class_id_t = int // This is essentially the same as a storeclass_t : it's the dereference of the second component.


type aid_leaf_attribute_t =
    { la_nemtok:          new_nemtok_t option
      la_literalstring:   bool
      la_romf:            bool
      la_arithx:          bool // This is the arith-dontcare SCX storage class arising from pointer arithmetic etc.
      la_regionf:         bool
  }


let aidaToStr la = sprintf "%s%s%s%s%s" (if la.la_literalstring then "LITERALSTRING~" else "") (if la.la_romf then "ROMF~" else "") (if la.la_arithx then "ARITHX~" else "") (if la.la_regionf then "REGIONF~" else "") (if nonep la.la_nemtok then "BLANK-NEMTOK" else valOf la.la_nemtok)

let g_no_escape_mark = "$NOESCAPE"

let g_purely_symbolic_under_reference = "purely-symbolic-under-reference"
let g_pointer_type = g_canned_i64
let g_pointer_size = 8L;     // A pointer takes 8 bytes since we have a 64-bit virtual machine. 


let g_null_aid =
    { la_nemtok=          None
      la_literalstring=   false
      la_romf=            false
      la_arithx=          false // This is the arith-dontcare SCX storage class arising from pointer arithmetic etc.
      la_regionf=         false
  }
    

let oapp ff = function
    | None -> None
    | Some x -> Some(ff x)

let rec aidToStr_quiet = function
    | other         -> aidToStr other
    
let rec aid_complexity arg = aid_depth_measure 0 arg

let aidlToStr = function
    | [] -> "BLANK-AIDL"
    | aidl -> sfold aidToStr aidl

let aidoToStr = function
    | None -> "NO-AID"
    | Some aid -> aidToStr aid


// One equality concept for aids is that all array locations are equivalent whereas all record tags are distinct. Hence we must eliminate
// any unique utag information in array subscription operators.    Constant subscriptions and other such patterns are later factored out in
// repack.fs    
// Alternatively: don't put a utag in when making an indtag in the first place!

//
// Indexing of memory space:  we need numeric values for equality checking during KiwiC compilation, but we want to output in symbolic
// form (using X_pairs to annotate the numeric values) so that repack can easily relocate items.
type idx_t =
    | O_tot of int64 // deprecated?
    | O_tagc  of aid_t option * int64 *(int * (string * string) * typeinfo_t * int64) * int64 // constant form: base address, tag details and total nominal address.
    | O_tagv  of aid_t option * hexp_t * (int * (string * string) * typeinfo_t * int64)   // variable form: base address and tag details
    | O_subsc of aid_t option * aid_t option * int64 * int64 * int64 * int64   // constant form: nemtok, base address, length, offset and total nominal address.
    | O_subsv of aid_t option * aid_t option * hexp_t * int64 * hexp_t   // variable form: nemtok, base address, len and offset expression
    | O_filler of string

let idx2s = function
    | O_tot n -> i2s64 n
    | O_tagc  (nt, baser, (tag_no, ptag_utag, dt, tag_idx), n) -> sprintf "O_tagc(%i; " baser + snd ptag_utag + sprintf "; %i)" n
    | O_tagv  (ntl, baser, (tag_no, ptag_utag, dt, tag_idx)) -> sprintf "O_tagv(%s; %s; %s)" (aidoToStr ntl) (xToStr baser) (snd ptag_utag)
    | O_subsc (aid_b, aid_o, baser, len, subs_idx, n) -> sprintf "O_subsc(%i..%i; %i; %i)" baser (baser+len-1L) subs_idx n
    | O_subsv (aid_b, aid_o, baser, len, subs_idx) -> sprintf "O_subsv(%s;%s;%s..+%i;%s)" (aidoToStr aid_b) (aidoToStr aid_o) (xToStr baser) len (xToStr subs_idx)
    | O_filler s -> muddy ("idx filler " + s)
    

type heap_block_alloc_record_t =
    { baser:  int64
      len:    int64
      types:  string //Don't need ttype string now aid is also present.
      aid:    new_nemtok_t // 
    }


type ce_fn_t =
    | CEF_bif of kiwi_bif_fn_t
    | CEF_mdt of method_struct_t

// Server call - net names and so on.
type scall_t = // Static definition of a server port (once only however many call sites).
    {
        idl:        stringl             // name of the port or binding between client and server - not yet used
        voidf:      bool
        a1:         bool * int
        rn:         (int * (string list * typeinfo_t * cil_eval_t) * hexp_t * cil_eval_t option) option
        hs_nets:    (int * (string list * typeinfo_t * cil_eval_t) * hexp_t * cil_eval_t option) list
        arg_nets:   (int * (string list * typeinfo_t * cil_eval_t) * hexp_t * cil_eval_t option) list
    }

and tannotation_map_t = Map<string list, ciltype_t>
and annotation_map_t = Map<string list, dropping_data_t>
(*
 * There are far too many forms here: please rationalise!
 *)
and cil_eval_t = 
    | CE_ternary of typeinfo_t * cil_eval_t * cil_eval_t * cil_eval_t
    | CE_struct of string * typeinfo_t * new_nemtok_t * (string(*ptag only*) * ciltype_t * cil_eval_t option) list // A structure valuetype has a list of components in it. This is optioned because not all elements may be assigned at some mid-elaboration points. But C# insists that ultimately they all should be.
    | CE_start  of bool * stringl * dropping_data_t list * cil_eval_t * cil_eval_t list * annotation_map_t (* newthreadf, callstring, types, action/delegate and thread args *)
    | CE_server_call of string * bool * callers_flags_t * cil_eval_t list * scall_t * cil_eval_t list
    | CE_apply  of ce_fn_t * callers_flags_t * cil_eval_t list
    | CE_alu    of typeinfo_t * x_diop_t * cil_eval_t * cil_eval_t * aid_t option
    | CE_scond  of x_bdiop_t * cil_eval_t * cil_eval_t     // Set on condition - predicate testers.
    | CE_conv   of typeinfo_t * cast_severity_t * cil_eval_t         // Convert (cast).
    
    | CE_typeonly of typeinfo_t
    //| CE_zombie of string  // Indicates bottom/nota-const.
    | CE_init of cil_t

    // Address-of or dereference operator (positive n is & and negative is *).
    // The cached aid field describes the result of the CE_star expression so will have (stars) A_ind layers added w.r.t. the enclosed ce.
    | CE_star of int * cil_eval_t * aid_t 


    //| CE_blk of cil_eval_t list  // A structure/record block of fields needed for struct assigns

    // Array subscription: base and index expressions are each tagged with a dataflow aid_t 
    | CE_subsc of typeinfo_t * cil_eval_t * cil_eval_t * aid_t option
    // Entries are the type of the array (not just its contents type), the array and the subscript and a pair of cached dataflow ids.
    // The cached aid fields describe the result of the CE_subsc expression so need incrementing (deleting an A_ind) to get back to the base and offset expression storage classes themselves.


    // Indexed literal (experimental for C++ input)
    | CE_ilit of cil_eval_t * cil_eval_t
    
    // Field tag: another form of subscripting. 
    | CE_dot of typeinfo_t * cil_eval_t * (int * field_struct_t option * (string * string) * typeinfo_t * int64) * aid_t option
    // Entries in dot are: record type, record pointer, one selected field of the record by (no, id, type and byte offset) and the naming token of what?  

    | CE_var of                   // Singleton registers (statics and locals with tid and calldepth baked in)
         int  *                   // Virtual register number
         typeinfo_t *             // Type
         string list *            // Singleton name
         field_struct_t option *
         (string * string) list   // Misc attribute catcherall

    | CE_x of typeinfo_t * hexp_t // An already lowered expression.

    |  CE_region of              // Region of memory.
        vimfo_t *                // Region primary descriptor, 
        typeinfo_t *             // Type. If the region is an array then dt=CT_arr, and if it is a record, then dt=CT_class or dt=CTL_record. So this is not the content type (which is meaningless for a record) it is the overall type of the region.  It perhaps does not have the leading & but this would be added on by the newarr and newobj calls.  
        (string * string) list * // Attributes,
        new_nemtok_t option *    // Storage class if not a complete wondarray.
                                 // DefinitiveS: This is the class of the base address. A scalar pointer to this region would be in the same storage class. If this is an array, its contents' class is SC_ind applied to this base.  On the other hand, the storage class for a scalar register is for what is stored in it, not its own address, so no SC_ind is applied to a register's class as data is moved in and out. 
        ciltype_t option         // Original src code - such as array initialisation data.

    // CE_handleArith for pointer arithmetic is now subsumed into CE_alu so please delete this
    | CE_handleArith of typeinfo_t * cil_eval_t * cil_eval_t // Pointer arithmetic (base, integer_offset)

    | CE_reflection of typeinfo_t * typeinfo_t option * string list option * int option * (string * string) list // Reflection/dynamic API token: cooercable to g_IntPtr when needed as a runtime value. The first typeinfo will be a CTL_reflection_handle always and the second is the type of what is being reflected if it is a type.

    | CE_filler

(*
 DefinitiveS: For either a .tag or a .[] subscription, the aid of the result (content type) is the  adi0 of the lhs decremented once.  Of course, for fields they may have their own disjoint dataflows and should best be kept as separate storage classes, but this is a separate question of when to conglomorated commonly  pointed-at classes.

 DefinitiveS: The .tag CE_dot subscript form contains both the record and the field type, whereas the .[] CE_subsc subscript form contains the type of the whole array.

 DefinitiveS: The aid cached in both the .tag subscript form and the .[] subscript form contains the aid of the result (ie deca_for_vassign already applied).  But the best nt to put in a subscripting operation itself needs to be the undecremented form (aka unquoted) since the repack subscript manager needs to look up the regions.


DefinitiveS: The blockref var clearly is in the same sc as the handle V_0.  We need to allocate a pair of classes for a data array and multiple classes (1 + no fieldds) for a record.

DefinitiveS: Storage classes are essentially types, branded types.

OLD NOTATION FOLLOWS:


DefinitiveS: 'scx points at scy' can be better read as 'scx contains addresses of variables that hold items from scy' or 'scx is the class of multiplexor control expressions that multiplex values in class scy'

DefinitiveS: In the new notation, d100 is a storage class for values.   [d100] is a storage class for the handle on or base address of an array containing items in d100.  d100->foo is the class for a field called foo in objects whose handle is in class d100.   

DefinitiveS: A variable and its address are in different storage classes.  The same goes for regions, which are either data arrays or records. For records, each field can be in a different class but the address of the record remains as one class, of course.  The storage class next to a heap constant is the address of a region.  Regions are primarily labled with their address storage class whereas scalars (about whose addresses we are seldom concerned) are labled with their content class.

DefinitiveS:  The storage class held in a region (or its naming token, nemtok) is that of its base address. A scalar pointer to that region would be in the same storage class since we speak of the content class for scalars. On the other hand, the storage class for a scalar register is for what is stored in it, not its own address, so no SC_ind is applied to a register's class as data is moved in and out. 

To help keep matters clear, we set a region flag in the aid_t of regions - indicating they are
  ... partial types 


...  NO NO NO TODO. Kiwi used to commonly put the content type designation inside an array subscript expression, clearly marked as such with a ^-1 or lack of quotes: then the inverse points at relation was needed by repack to get the base storage class which was also the subscript class.  There is an argument perhaps for the base and subscript classes to be kept separate, especially when both are non-constant expressions.  
*)



and ce_env_item_t =
    | CEE_dyn
    | CEE_i64 of int64
    | CEE_scalar of rez_idx_t * cil_eval_t
    | CEE_vector of rez_idx_t * ce_tree_env_t
    | CEE_zombie of string // Bottom-marked death not-a-const
    | CEE_method_set of stringl list // Sorted list of methods invoked from a dynamic call site

and rez_idx_t = int

and ce_tree_env_t =
    //| CEE_cell  of int64 * int64 * ce_tree_env_t 
    | CEE_tree  of ce_tree_env_t * int64 * ce_env_item_t * ce_tree_env_t // Tree of disjoint regions of memory, indexed lower bound and upper bounds, const idx form, plus contents, that may be a subtree sometimes. The first field is left sub-tree and final is right-subtree 
    | CEE_tend   of string

// Heapspace env
and ce_hs_env_t =
    | CEE_hs_tend of string
    | CEE_heapspace of (int64 * heap_block_alloc_record_t list) ref * int64 list * int64 list  // Virgin heap space base, block use plan/policy (global), in use block list, and discrepancy list.


// Main env : indexed by memtok not nemtok if we are precise, but both are strings.
type envmap_t = Map<new_nemtok_t, ce_env_item_t>

type env_t = { hs1: ce_hs_env_t; mainenv: envmap_t; }
 

let g_assign_once_cache_var =  "AssignOnceCacheVar"

let g_hsimple_servers:(stringl * scall_t) list ref = ref [] // These are the old HSIMPLE servers - we should not be using these any more, but test19 in the regression suite is still HSIMPLE.  Test67 is HFAST.

let kp2s md = mdToStr md

type kiwi_storeclass_t = memdesc_record_t

type proto_memdesc_record_t =
    {
        //p_region_flag:   bool           // never read infact
        p_idl:              string list
        p_aid:              aid_t
        p_nemtok:           new_nemtok_t   // Dataflow name nad primary key (not unique for heap-allocated)
        p_uid:              vimfo_t      
        mats:               att_att_t list
        p_labelled_literal: bool
    }


type proto_memdesc_store_t = ListStore<new_nemtok_t, proto_memdesc_record_t> // Explain why more than one is possible under a single nemtok.

(* Heap is a splaymap of string list to pairs of type and current symbolic/actual value. 
 *
 * Since we are doing static allocation of all elab variables, our heap contains
 * items that one might think should be on the stack, like method locals: we ultimately have no stack
 * in an HPR machine.
 *
  The singleton/static heap can use nemtoks as the retrieval sheap_id key directly since there is one only of each static variable.  When lowered to hexp_t the option ref is populated as a d/p cache. The bool indicates the result is labelled literal data instead of a variable. 
 *)
type vrn_t = int

type sheap_entry_t =
    {
        vrn:              vrn_t   // Virtual register number, primary key, 0 is not allowed, -ve numbers are artificial keys were the singleton should be displayed in its idl/nemtok form instead.
        //ids:              string // 
        s_nemtok:         new_nemtok_t // This nemtok is hptos of idl, should be an alternative key to the sheap, whereas cez with have hptos_net of that idl.
        s_aid:            aid_t
        idl:              string list
        labelled_literal: bool                // A string for C# but other forms too for C++
        ethereal:         bool                // Holds when just a place holder, such as for a struct that has been sconed.
        cez:              cil_eval_t
        ever_addressed:   bool ref
        hexp:             hexp_t option ref   // DP cache
        baser:            int64               // Address in wond space which will not be needed if never addressed.
        len:              int64
        atakenf:          bool option         // Knowledge of whether this entry ever has its address taken (and hence liable to be remotely overwritten).
        io_group:         decl_binder_metainfo_t option
        dt:               ciltype_t
        tlm_propertyf:    bool option         // Denotes that this variable is held in a component and reads/writes need to do message passing (TLM calls) for access.
    }

type sheap1_t = OptionStore<new_nemtok_t, sheap_entry_t>
type sheap2_t = OptionStore<vrn_t, sheap_entry_t>

type heap_t =
    { 
      sheap1:                sheap1_t        // Singleton heap indexed by nemtok      
      sheap2:                sheap2_t        // Singleton heap indexed by vrn
      memdescs:              proto_memdesc_store_t
      scalar_singleton_sbrk: int64 ref
      vector_singleton_sbrk: int64 ref      
    }



type gpregs_scoreboard_t =
    {
        cez:          cil_eval_t option ref  // We create the gpreg net as late as possible once we know all its uses
        usecount:     int ref
        inuse:        bool ref
        idls:         stringl list ref  // We support multiple idl names in case it is shared used and then give some final indicative name
        vrns:         int list ref      // Likewise, there will then be multiple virtual register numbers mapped to one physical register.
    }

type storemode_t =
    | STOREMODE_singleton_scalar
    | STOREMODE_singleton_vector
    | STOREMODE_compiletime_heap
    | STOREMODE_runtime_heap        
    | STOREMODE_sri of bool option * string * ciltype_t * string list * fun_semantic_t option
    
type virtual_reg_info_t =
    {
        //frameoffset: int
        ident_:       string list
//        virt_cez:    cil_eval_t option ref         // Set when created (or shortly after)
        phyreg:      gpregs_scoreboard_t option ref  // Set when mapped to a physical register
        allocated:   bool ref
        vrn:         int
        dt:          ciltype_t
        purpose:     string
        storemode:   storemode_t
    }

type vregs_database_t = OptionStore<int, virtual_reg_info_t>

    
// Scoreboard and affinity needed to reuse
type phyregs_database_t = ListStore<string list, gpregs_scoreboard_t>


(*
   There are two envisionable designs :
  1. where all classes are target classes but they are linked in a DAG : this would imply that all pointers that can point to class x are in a common class, whereas they could be in several classes that do not intercommunicate. E.g. some pointers to X range over three instances whereas others range over six plus null and there is some overlap in the pointed at sets.

    2. so instead each target class has a list of specific nemtoks it points at which is independet of which target classes those nemtoks are in.  target classes for non-pointer types will not point at anything of course.
*)

let g_literalstring_flags = new OptionStore<string, bool>("literalstring_flags")

type kcode_globals_t = // // Storage class tracking
    {
         nemtok_details:   OptionStore<new_nemtok_t, aid_leaf_attribute_t>
        // There's also heap.memdescs that is logically associated - tidy perhaps.
         // There seems to be no real purpose to having this info split over two collections... its a pain really.
        // and ataken should be in here TODO
         pointstyles:      ListStore<string, (string * string)> // Maps sc to tags and inds built off it (aka seen)
         ksc:              ksc_classes_db_t
         aid_derefs:       OptionStore<aid_t, aid_t>
         aid_refs:         ListStore<aid_t, aid_t>         
    }
     

type dyno_dispatch_t = ListStore<stringl, int * stringl>  // Method names dynamically dispatch from a call site


type already_initialised_vars_t = OptionStore<stringl, int>

type thread_id_t = string

type kcode_idx_t = string

type reflection_icon_dbase_t = OptionStore<int, int>

type specific_t =
    | S_kickoff_collate
    | S_startup_code
    | S_spawned_method
    | S_root_method
    | S_root_class 


type tid3_t = // Attributed thread identifier.
    {
        no:         int
        id:         thread_id_t
        style:      major_hls_mode_t
        specific:   specific_t
    }

type codesite_t = (tid3_t * kcode_idx_t)

type constant_or_volatile_marker_rd_t = (tid3_t * bool) list

type constant_or_volatile_marker_wr_t =
    | VOC_wr_none
    | VOC_monoassign of codesite_t 
    | VOC_multi_value_assign of codesite_t list
    | VOC_multi_thread_assign of codesite_t list 

type constant_or_volatile_marker_t = constant_or_volatile_marker_rd_t * constant_or_volatile_marker_wr_t

type constant_or_volatile_dbase_t = OptionStore<new_nemtok_t, constant_or_volatile_marker_t>

type toparg_t = (int * (stringl * typeinfo_t * cil_eval_t) * hexp_t * hexp_t option)

type m_remote_ctor_arg_db_t = ListStore<string list, cil_eval_t * (int * typeinfo_t * cil_eval_t) list>  // A record, indexed by class name, of which constructors were called with what args, in case they are needed for separate/RPC compilations.

type raw_cil_dir_t = (stringl * (bool * string * cil_t)) list ref // (is_exe, filename, ast)

type library_substitutions_t = OptionStore<string list, string list>

type kfec_settings_2_t =
    {
        stagename:                 string  // O/Path name of this recipe stage - "kiwife"
        dynpoly:                   bool    // Enable dynamic method dispatch
        // CIL can be written out immediately post parsing or after some other procesing steps.
        reset_mode:                string  // Synch or asynch resets or none for the final RTL.
        zerolength_arrays:         bool    // Allow empty arrays
        postgen_optimise:          bool    // Post-generation optimisations
        directorate_attributes:    (string * string) list // Default director
        directorate_style:         directorate_style_t  // Clocks, clock enables and so on.
        supress_zero_struct_inits: bool    // Discard clear-to-zero in constructor code (needed while gbuild does not respect big_bang for remote servers).

        // Logging level diorosity settings
        hgen_loglevel:             int     // Hardware generation logging level
        constvol_loglevel:         int        
        early_cil_dumpf:           bool    // Write intermediate CIL to a dump file early on
        //mid_cil_dumpf:           bool  
        late_cil_dumpf:            bool    // Write intermediate CIL to a dump file
        cil_dump_separately:       bool    // Write CIL assemblies to separate dump files
        cil_dump_combined:         bool    // Write CIL assemblies to one big file
        kcode_dump:                int     // Write intermediate kcode to a dump file
        
    }


type m_classes_possibly_used_t = OptionStore<string list, bool> // used as a set for now


type cilconst_t = // Not actually constant, but a singleton concourse class for a KiwiC recipe stage run.
    {
        toolname:                  string  // Normally "kiwife"
        settings:                  kfec_settings_2_t
        dyno_dispatch_updated:     bool ref
        dyno_dispatch_methods:     dyno_dispatch_t   // Method names dynamically dispatch from a call site for higher-order programs.
        //dyno_dispatch_closures:    dyno_dispatch_t   // ... dont need since done by repack for us
        already_initialised_vars:  already_initialised_vars_t
        next_reflection_int:       int ref
        reflection_constants:      (string list * cil_eval_t) list ref
        kcode_globals:             kcode_globals_t
        rpt:                       logger_t
        m_vregs:                   vregs_database_t // Virtual registers names and types
        kiwicid:                   string

        // We may not need to store these headargs/topargs now: they are being mined from the heap declarations using the DB_ approach.
        m_old_headargs_:           (toparg_t list * toparg_t option) option ref // Top entry point signature  - Not used anymore
        m_additional_static_cil:   cil_t list list ref
        m_additional_net_decls:    hexp_t list ref
        control_options:           control_t
        directory:                 raw_cil_dir_t  // Directory of declared/defined CIL ast components 
        heap:                      heap_t



        m_remote_ctor_arg_db:      m_remote_ctor_arg_db_t
        //m_called_methods:          string list list ref  // Methods called (so discarded as remote entry roots)
        //m_classes_where_statics_constructed: (string list * (string * int * bool ref)) list ref
        m_classes_possibly_used:   m_classes_possibly_used_t
        next_virtual_reg_no:       int ref // +ve vregs are first-class ones and these go in cCB
        next_negative_reg_no:      int ref // Count seperately -ve ones where the number serves for lookup of a singleton but has nothing to do with a virtual register.

        reflection_icons:          reflection_icon_dbase_t

        allow_hpr_alloc:           bool

        bondouts:                  ((bondout_address_space_t * bondout_port_t list) * bondout_memory_map_manager_t) list    
        constant_or_volatile:      constant_or_volatile_dbase_t
        library_substitutions:     library_substitutions_t
    }

// Ideally would use some oo programming here for the cilconst -  this is the frontend object.
let g_CCobject:cilconst_t option ref = ref None
    

let cez_ToStr_atts = function
    | [] -> ""
    | (l) -> "{" + foldl (fun ((a,b), c)-> a + "=" + b + (if c="}" then c else ", " + c)) "}" l


let dimsToStr l = "[" + foldl (fun(a,b) -> i2s a + (if b = "]" then b else "," + b)) "]" l

let uidToStr (x:vimfo_t) = hptos x.f_name + "%" + hptos x.f_dtidl + "%" + i2s64 x.baser + "%" + (if x.length=None then "None" else i2s64(valOf x.length))

//+ "%" + sfold hptos x.sc

let tid3ToStr tid3 = (sprintf "%s %A %A" tid3.id tid3.style tid3.specific)

let rec discard_ce_conv msg = function
    | CE_conv(_, _, arg) -> discard_ce_conv msg arg
    | other -> other

let rec wanton_tidy msg = function
    | CE_conv(_, _, ce) -> wanton_tidy msg ce
    | CE_star(_, ce, _) -> wanton_tidy msg ce
    | other -> other


// Aka gec_CE_x
let gec_yb(x, dt) = CE_x(dt, x) // Generate (aka rez) an output expression 

let gec_ce_string ss = gec_yb(xi_string ss, f_canned_c16_str(String.length ss))

let ce_iszero = function
    | CE_x(_, x) -> xi_iszero x
    | _ -> false

let ce_is_hllreset = function
    | CE_x(CTL_net(_, _, _, atts), _) -> atts.nat_hllreset
    | _ -> false


let xmy_num n = xi_num n
let gen_ce_int32 n = gec_yb(xmy_num n, g_canned_i32)
let gen_ce_int64 n = gec_yb(xi_int64 n, g_canned_i64) 
let g_ce_null      =  gen_ce_int64 0L
let g_ce_resetval  = gec_yb(xi_int64 0L, g_canned_i64rst) 
let ce_isnull = ce_iszero

let rec scan_vinv = function
    | [] -> false
    | ("rtl_parameter", _)::tt
    | ("io_input", _)::tt -> true
    | _::tt -> scan_vinv tt

let is_rtl_pram_or_input ce =
     match wanton_tidy "is_rtl_pram_or_input" ce with
        | CE_var(_, dt_, idl, fdto, ats) -> scan_vinv ats
        | _ -> false

let aidtag2s (regf, ns, tag) = hptos tag

let vreg2s vrn = sprintf "V%i" vrn

    
let rec list_some_updates n v =
    if n=0 || nullp v then ""
    else (ceToStr(snd(hd v)) + "/[" + xToStr(fst(hd v)) + "]") + (if nullp v then "" else ",") + (list_some_updates (n-1) (tl v))


and cez_valeToStr(l) = if l < 0 then "" else "[" + i2s l + "]"


and ceToStr_simple = function
    | CE_x(_, v) -> xToStr v
    | CE_alu(ct, dop, a1, a2, aidl)    ->
        //let aids = sfold_quiet aidToStr_quiet aid
        sprintf "DIOP(%s, %s, %s)" (ceToStr_simple a1)  (f1o3(xToStr_dop dop)) (ceToStr_simple a2) // (if aids="" then "" else sprintf "{ALUAID=%s}" aids)
    | CE_var(vrn, dt, idl, fdto, ats) ->
        let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
        if vrn >= 0 && colouringf then vreg2s vrn else hptos idl
    | CE_region(uid, dt, ats, nemtok, srccode) -> sprintf "CE_region(%s)" (hptos uid.f_name)
    | CE_dot(ttt, ce, (tag_no, fdt, (ptag, utag), dt_field, n), aid_o) -> sprintf "%s->%s" (ceToStr_simple ce) ptag
    //| CE_newarr(ct, a1, (nemtok_region, nemtok_content), hintname, ats) -> sprintf "CE_newarr[%s]" (ceToStr_simple a1)
    | other -> ceToStrx true other
    
and ceToStr:(cil_eval_t -> string) = ceToStrx false

and ceToStrx simpleflag arg =    
    let wt = true
    let rec is = function
        | 0 -> ""
        | n -> "  " + is (n-1)

    let rec ceToS indent arg =
        match arg with
        | CE_x _
        | CE_alu _
        | CE_var _
        | CE_region _
        //| CE_newarr _
        | CE_dot _  when simpleflag -> ceToStr_simple arg


        | CE_typeonly dt -> "CE_typeonly(" + typeinfoToStr dt + ")"

        | CE_reflection(hdt, None, Some idl, valo, ats) -> sprintf "CE_reflection(idl=%s)" (hptos idl)
        | CE_reflection(hdt, Some dt, None, valo, ats) -> sprintf "CE_reflection(dt=%s)" (cil_typeToStr dt)        
        | CE_handleArith(dt, l, r) -> sprintf "handleArith(%s, %s,%s)" (typeinfoToStr dt) (ceToS (indent+1) l) (ceToS (indent+1) r)
        //| CE_newobj(nemtok, idl, hintname_o, stru, _, ats) -> sprintf "CE_newobj(%s, %s, ...,%s, ...)" nemtok (hptos idl) (valOf_or hintname_o "-NOHINT-") 
        //| CE_newarr(ct, a1, (nemtok_region, nemtok_content), hintname_o, ats) -> sprintf "CE_newarr(%s,%s" (typeinfoToStr ct) (ceToS (indent+1) a1) + ", " + nemtok_region + ":" + nemtok_content + "," + (valOf_or hintname_o "-NOHINT-") + ")" + cez_ToStr_atts ats
        | CE_server_call(id, instancef, cf, sargs, v, args) -> "CE_server_call(" + id + ", " + sfold (ceToS (indent+1)) args + ")"
        | CE_apply(ff, cf, args) ->
            let name = 
                match ff with
                    | CEF_bif fid -> hptos fid.uname + "=" + fid.hpr_name
                    | CEF_mdt mdt -> mdt.uname
            let rec p n = function
                | []   -> ""
                | h::t -> sprintf "\n    %s   Arg%i = %s" (is indent) n (ceToS (indent+1) h) + p (n+1) t
            sprintf "CE_apply%s(%s, %s)" (cfToStr cf) name (p 1 args)
                
        | CE_conv(dt, severity, arg) -> sprintf "CE_conv(%s, %A, %s)" (typeinfoToStr dt) severity (ceToS (indent+1) arg)
       
        | CE_scond(dop, a1, a2)        -> sprintf "CE_scond(" + ceToS (indent+1) a1  + f1o4(xbToStr_dop dop) +  ceToS (indent+1) a2 + ")"

        | CE_ternary(ct, g1, a1, a2)    -> sprintf "CE_ternary<%s>(%s, %s, %s)" (if wt then cil_typeToStr ct else "..") (ceToS (indent+1) g1) (ceToS (indent+1) a1)  (ceToS (indent+1) a2)
        | CE_alu(ct, dop, a1, a2, aido) -> sprintf "CE_alu<%s>(%s, %s, %s, {%s})" (if wt then cil_typeToStr ct else "..") (ceToS (indent+1) a1) (f1o3(xToStr_dop dop)) (ceToS (indent+1) a2) (if nonep aido then "" else aidToStr_quiet(valOf aido))
        | CE_struct(idtoken, ty, aid, items) ->
            let cesToS indent (ptag, field_dt, vale_opt) =
                sprintf "%s/%s" (if nonep vale_opt then "_!_" else "(" + ceToS indent (valOf vale_opt) + ")") ptag
            sprintf "CE_struct_%s<%s>^%i(%s)"  idtoken (if wt then cil_typeToStr ty else "..") (length items) (sfold (cesToS (indent+1)) items)
        | CE_start(threadf, csl, _, delegater, args, cgr)   -> sprintf "CE_start%s(%s, %s)" (if threadf then "thread" else "call") (hptos csl) (ceToS (indent+1) delegater)
        //| CE_blk(lst)               -> "CE_blk(" + sfold (ceToS (indent+1)) lst + ")"   

        // We can print CE_dot as DOT or as -> field access.  The concise form uses the latter. 
        | CE_dot(ttt, ce, (tag_no, fdt, (ptag, utag), dt_field, n), aid_o) -> sprintf "DOT<%s>(%s, field=%s" (if wt then typeinfoToStr ttt else "..") (ceToS (indent+1) ce) (ptag) + "/" + typeinfoToStr dt_field + "/" + i2s64 n + ", " + aidoToStr aid_o + ")"    

        | CE_x(_, v) -> "CE_x(" + xkey v + " "  + xToStr v + ")"

        | CE_star(stars, ce, aid) ->  // +ve stars in CE_star expressions are printed as ampersands since you take the address of something to get a pointer type.
            let rec st = function
                | 0 -> ""
                | n -> if n > 0 then "&" + st(n-1) else  "*" + st(n+1)
            st stars + (if simpleflag then sprintf "(%s)"  (ceToS (indent+1) ce) else sprintf "(%s, {%s})" (ceToS (indent+1) ce) (aidToStr aid))
            

        | CE_ilit(baser, idx) -> sprintf "CE_ilit(%s, %s)" (ceToS (indent+1) baser) (ceToS (indent+1) idx)
        | CE_subsc(adt, ce, idx, aid_o) ->
            //The cached aid_info fields describe the result of the CE_subsc expression so need incrementing (deleting an A_ind) to get back to the base and offset expression storage classes themselves.
            let aid_info = if nonep aid_o then "" else aidToStr(valOf aid_o)
            sprintf "CE_subsc<%s>(%s, %s%s)" (if wt then typeinfoToStr adt else "..") (ceToS (indent+1) ce) (ceToS (indent+1) idx) (aid_info)
        
        | CE_var(vrn, dt, idl, fdto, ats) ->
            let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
            (if vrn > 0 && colouringf then sprintf "V%i/" vrn else "") + hptos idl//  + typeinfoToStr dt + cez_ToStr_atts ats 

        
        | CE_init a      -> "CE_init(" + cil_classitemToStr " " a ")"

        //| CE_zombie ss   -> sprintf "CE_zombie(%s)" ss

        | CE_region(uid, dt, ats, nemtok, srccode) -> sprintf "CE_region<%s>(%s, nemtok=%s, ats=%s)" (if wt then cil_typeToStr dt else "..") (uidToStr uid) (valOf_or_blank nemtok) (cez_ToStr_atts ats)

        | other -> sf (sprintf "ceToStr other %A" other)

    ceToS 0 arg

and env_itemToStr n = function
    | CEE_dyn              -> "DYNAMIC"
    | CEE_i64 v            -> sprintf "CEE_int(%i)" v
    | CEE_scalar(ri, v)    -> ceToStr v
    | CEE_zombie ss        -> sprintf "CEE_zombie(%s)" ss
    | CEE_vector(ri, tree) -> ceTreeToStr n tree
    | CEE_method_set idls  -> sprintf "CEE_method_set(%s)" (sfold hptos idls)
    
and ceTreeToStr n = function
    //| CEE_cell(lidx, hidx,  vale) -> sprintf "cell[%i:%i,%s]" lidx hidx (ceTreeToStr (n-1)  vale)
    //| CEE_tree(ll, lidx, ridx, CEE_cell(lidx', ridx', vale), rr) when lidx=lidx' && ridx=ridx' -> 
    //if n <= 0 then "..."
    //        else sprintf "tree[%s,%s,%s]" (ceTreeToStr (n-1) ll) (ceTreeToStr (n-1) vale) (ceTreeToStr (n-1) rr)
    | CEE_tree(ll, idx, vale, rr) -> 
        if n <= 0 then "..."
        else sprintf "tree[%s,%i:%s,%s]" (ceTreeToStr (n-1) ll) idx (env_itemToStr (n-1) vale)  (ceTreeToStr (n-1) rr)
    | CEE_tend s ->  "/tend:" + s

    //| other ->  (sprintf "CEETREE?? %A:" other)

and ce_hs_envToStr n__ = function
    | CEE_hs_tend ss -> sprintf "CEE_hs_tend(%s)"  ss
    | CEE_heapspace(allocating_info, inuse, discrepancies) ->
        let heapblkTos (blk:heap_block_alloc_record_t) = sprintf "(%i,%i,%s)" blk.baser blk.len blk.types
        let (virgin_limit, blks) = !allocating_info
        sprintf "CEE_heapspace(%i,^%i=%s,inuse=%s, discreps=%s)"  virgin_limit (length blks) (sfold heapblkTos blks)  (sfold i2s64 inuse) (sfold i2s64 discrepancies)


and ceToStr_full other = ceToStr other


let ce2ctype ce =
    match ce with
        | CE_typeonly ct -> ct
        | CE_var(vrn, dt, idl, fdto, ats) -> dt
        | CE_region(uid, dt, ats, nemtok, _) -> dt
        | CE_alu(dt, _, _, _, aidl)  -> dt
        | CE_star(n, CE_var(vrn, dt, idl, fdto, ats), aid) -> gec_CT_star n dt    
    
        | other ->
            let _ = dev_println ("ce2ctype brutal object poorman's path ce=" + ceToStr other)
            g_ctl_object_ref

let ce_vrn = function
    | CE_var(vrn, dt, idl, fdto, ats) -> Some (int64 vrn)
    | CE_region(uid, dt, ats, nemtok, srccode) -> Some uid.baser


    | CE_dot _ -> None

    | other ->
        dev_println(sprintf "Other ce_vrn form %s." (ceToStr other))
        None


let rec ct_signed ww = function
    | CT_knot(idl, whom, vv) when not_nonep !vv -> ct_signed ww (valOf !vv)
    | CTL_net(_ , _, signed, _) -> signed
    | CTL_record _ | CT_class _ | CT_cr _ | CTL_void -> Unsigned
    //| CT_brand(_, cs)    | CT_valuetype(cs)    
    | CT_star(_, cs)
    | CT_arr(cs, _) -> ct_signed ww cs
    | CTL_vt(_, _, cs) -> ct_signed ww cs

    | CTL_reflection_handle _ | CTL_void -> Unsigned
    | Ciltype_filler -> Signed // Only for debug.
    | other -> sf(sprintf "ct_signed other %s\n%A" (typeinfoToStr other) other)






// Collect supporting variable numbers for an expression in one of several modes. 
// The return type is int64 option list
// For DFA we want topmode which lists variables whose value may be directly returned by an expression, as opposed to those needed from computing an expression.    
let ce_support mode cc ce =

    let topmode = true // The only mode needed at the moment - inclusion of subexpressed aids is not normally wanted, so just return the topmost aid.

    let rec ces cc arg =
        match arg with
            | CE_region(uid, dt, ats, nemtoko, a)       -> singly_add (ce_vrn arg) cc
            | CE_var(vrn, dt, idl, fdto, ats)           -> singly_add (ce_vrn arg) cc

            | CE_ternary(_, g, aat, aaf) ->
                let cc = ces (ces cc aat) aaf
                if topmode then cc else ces cc g
            | CE_start(_, callstring, signat_etc, objptr, args, cgr0) -> if topmode then cc else List.fold ces (ces cc objptr) args
            | CE_typeonly _                                           -> cc
            | CE_init _                                               -> cc
            | CE_star(_, ce, _)                                       -> if topmode then cc else ces cc ce
            | CE_apply(g, cf, args)                     -> if topmode then cc else List.fold ces cc args
            | CE_x _                                    -> cc
            | CE_scond _                                -> cc
            | CE_reflection(_, None, Some idl, valo, _) -> cc
            | CE_subsc(_, lhs, subscript, _)                          -> if topmode then cc else ces (ces cc subscript) lhs
            | CE_dot(dt_record, l0, (tag_no, fdt, ptag_utag, dt_field, tag_idx), aid) -> if topmode then cc else ces cc l0
            | CE_conv(ct, cvtf, ce) -> ces cc ce
            | CE_alu(result_dt, diop, a1, a2, aidl)                   -> if topmode then cc else ces (ces cc a2) a1
            | CE_struct(idtoken, dt, aid, lst)                        -> if topmode then cc else List.fold ces cc (map (f3o3>>(valOf_or_fail "L843")) lst)
            | other ->
                dev_println (sprintf "ce_support other form ignored " + ceToStr other)
                cc
                
    let ans = ces cc ce
    ans



//
// All CTL_net forms and several others are valuetypes, but here we are just after structs ?
//   cf dt_is_struct
let rec ct_is_valuetype dt =
    match dt with
    | CT_knot(idl, whom, vv) when not(nonep !vv) -> ct_is_valuetype (valOf !vv)
    | CT_knot(idl, whom, vv) when (nonep !vv)    -> false // Only records will encounter an untied knot.
    | CTL_reflection_handle _
    | Cil_cr "$method_name_handle$" -> true  // TODO hardcode this as CTL_method_name?  This special string no longer generated anywhere?

    | CT_cr cst      -> ct_is_valuetype cst.crtno // Otherwise a classref not a valuetype (in our sense)
    | CT_class cst   -> cst.structf


    //| CT_valuetype(_) -> true
    | CTL_net(_) -> true // The correct answer (* ie not a wondidx ! *)
    | CT_star(n, ct) ->
        let _ = vprintln //2 ("+++ ct_valuetype applied to pointer: " + typeinfoToStr dt)
        n = 0 && ct_is_valuetype ct // Both +ve and -ve stars indicate a pointer. (Changed Sept 2017).

    | CTL_record(idl, cst, items, len, _, binds, _) -> cst.structf // Structures are valuetypes but objects are not.

    | CTL_void -> false
    | CT_arr _     -> false
    | Cil_cr_idl _ -> false

    //| CT_brand(_, ct)
    | CTL_vt(_, _, ct) -> ct_is_valuetype ct

    | CT_cr _
    | CT_method _ // A CT_method may have unbound type formals.
    | CT_class _-> false    
    | Ciltype_filler -> false // Only for debug
    | other -> sf (sprintf "%A" other +  ": ct_is_valuetype: other form " + typeinfoToStr other)

let rn_valOf m = function
    | RN_idl idl ->  idl
    | other -> sf(m + "rn_valOf: unexpected form: " + rnToStr other)

let rn_rev = function
    | (RN_idl idl) -> RN_idl(rev idl)
    | other -> other

// rnapp operator overload
let (++) a b =
    match(a, b) with
    | (RN_idl a, RN_idl b) -> RN_idl(a@b)
    | _ -> sf "rnapp other"




//---------------------------------------------------------------------------
// This is the k-code intermediate language definition:

type kcode_src_xref_t =
    | KXREF of string * string list // for now
    | KXR_dot of string list * kcode_src_xref_t
    | KXLP5 of linepoint5_t

type bcond_t =
    | BCOND_diadic of x_bdiop_t * cil_eval_t * cil_eval_t
    | BCOND_guarded of bool * cil_eval_t * string list

// Where strong typing is abused, as in casting from byte array to struct, we need a note to disable certain normal assumptions when looking up in envs.
type type_abuse_note_t = int // for now

type assign_attributes_t =
    { abuse:         type_abuse_note_t option
      reset_code:    bool  // Holds when following dotnet clear-to-zero initial.
    }

type kcode_alloc_form_t = 
    | KN_newobj of string list * string option * ciltype_t * typeinfo_t * (string * string) list
    | KN_newarr of typeinfo_t * cil_eval_t * new_nemtok_t * string option * (string * string) list // Typeinfo is the contents type. nemtok is the content aid, since the handle aid is in the K_new construct 

type kcode_t =
    | K_remark     of kcode_src_xref_t * string
    | K_lab        of kcode_src_xref_t * stringl
    | K_cmd        of kcode_src_xref_t * hbev_t
    | K_pragma     of kcode_src_xref_t * bool * string * cil_eval_t * cil_eval_t
    | K_easc       of kcode_src_xref_t * cil_eval_t * bool // Expression as statement = easc.

    | K_goto       of kcode_src_xref_t * bcond_t option * stringl * int ref
    | K_as_idl     of kcode_src_xref_t * stringl * cil_eval_t
    | K_as_vassign of kcode_src_xref_t * cil_eval_t * cil_eval_t * cil_eval_t * aid_t option * assign_attributes_t
    | K_new        of kcode_src_xref_t * cil_eval_t * new_nemtok_t * kcode_alloc_form_t
    | K_as_sassign of kcode_src_xref_t * cil_eval_t * cil_eval_t * assign_attributes_t
    | K_pointout   of kcode_src_xref_t * string * aid_t * aid_t
    | K_topret     of kcode_src_xref_t * cil_eval_t option // Return statement (top level only)

type gt_dic_line_t =
    {
        kinstruction:      kcode_t
        regionchange:      bool
    }

type gt_dic = // Directly-indexed code
    | Gt_dic of string * int * gt_dic_line_t array

    
// Linepoint to string - a user-friendly cross-reference to this C# source file, as best we can.
// It would be great if C# compilers preserved source-file line numbers - can we use an mdb/pdb file ?
let cil_lpToStr lp5 =
    //let cil_lpToStr1 = function 
    //| None -> i2s bpc + ">>000" + i2s bpc + ":" + i2s region + ":" + dis
    //| Some(sl, bpc, region, _) -> sprintf ">>000%i:%s\n>>000%i:%i:%s" bpc (hptos sl) bpc region dis

    let s1 = 
        match !lp5.codept with
            | None -> "<no codepoint>"
            | Some(sl, bpc, region, disref) -> sprintf ">>000%i:%s\n>>000%i:%i:%s:" bpc (hptos sl) bpc region !disref

    let s2 = if nonep !lp5.waypoint then "" else valOf !lp5.waypoint + ":"
    let s3 = if nullp !lp5.callstring__ then "" else "callstring__=" + hptos !lp5.callstring__ + ":"
    let stacktrace = "SrcStack=" + sfoldcr (fun x->x) lp5.linepoint_stack
    lp5.srcfile + ":" + s1 + s2 + s3 + stacktrace

// Different forms of linepoint apply at different points during the compilation/elaboration.
let rec kxrToStr = function
    | KXREF(origin, sl)   -> origin + "//" + hptos sl
    | KXR_dot(idl, path) ->  kxrToStr path + "/" + hptos idl // hptos uses the dot separator.
    | KXLP5 lp5 -> cil_lpToStr lp5
    
let kxr_sf kxr msg = sf(kxrToStr kxr + ":" + msg)

let kxr_cleanexit kxr msg = cleanexit(kxrToStr kxr + ":" + msg)



let get_kxr = function
    | K_remark(kxr, _) 
    | K_goto(kxr, _, _, _)
    | K_new(kxr, _, _, _)     
    | K_cmd(kxr, _)
    | K_pragma(kxr, _, _, _, _)     
    | K_easc(kxr, _, _)
    | K_topret(kxr, _)
    //| K_kill(kxr, _)         
    | K_as_sassign(kxr, _,  _, _) 
    | K_as_vassign(kxr, _, _, _, _, _)
    | K_pointout(kxr, _, _, _)    
    | K_lab(kxr, _) -> kxr
    | other -> sf (sprintf "other get_kxr %A" other)
                   
// Here is the .ToString() function for k-code.
let kToStr lpl = function
    | K_lab(kxr, sl)     -> (if lpl then kxrToStr kxr + " " else "") + hptos sl + ":"
    | K_remark(kxr, s)   -> (if lpl then kxrToStr kxr + " " else "") + "Rem: " + s
    //| K_linepoint(kxr)-> (if lpl then kxrToStr kxr + " " else "") + "LP: " + kxrToStr kxr    
    | K_goto(kxr, Some(BCOND_diadic(dop, l, r)), sl, _) -> (if lpl then kxrToStr kxr + " " else "") + "K_goto(" + ceToStr l + " " + f1o4(xbToStr_dop dop) + " " + ceToStr r + ", " + hptos sl + ")"

    | K_goto(kxr, None, sl, _)           -> (if lpl then kxrToStr kxr + " " else "") + "K_goto(UNCOND, " + hptos sl + ")"
    | K_goto(kxr, Some(BCOND_guarded(negf, ge, scl)), sl, _) -> (if lpl then kxrToStr kxr + " " else "") + "K_goto(" + (if negf then "ISZERO " else "ISNZERO ") + ceToStr ge + (if nullp scl then "" else " scl=" + sfold (fun x->x) scl + " ") + ", " + hptos sl + ")"        
    | K_pragma(kxr, fatalf, cmd, a0, a1)  -> (if lpl then kxrToStr kxr + " " else "") + sprintf "K_pragma(%A,%s,%s,%s)" fatalf cmd (ceToStr a0) (ceToStr a1)
    //| K_kill(kxr, reglist)     -> (if lpl then kxrToStr kxr + " " else "") + "K_kill " + sfold vreg2s reglist
    | K_cmd(kxr, e)            -> (if lpl then kxrToStr kxr + " " else "") + "K_cmd " + hbevToStr e
    | K_topret(kxr, None)      -> (if lpl then kxrToStr kxr + " " else "") + "K_topret(NONE)"
    | K_topret(kxr, Some e)    -> (if lpl then kxrToStr kxr + " " else "") + "K_topret(" + ceToStr e + ")"    
    | K_easc(kxr, e, ef)       -> (if lpl then kxrToStr kxr + " " else "") + "K_easc(" + ceToStr e + (if ef then " EMIT)" else ")")    
    | K_as_idl(kxr, idl, rhs)  -> (if lpl then kxrToStr kxr + " " else "") + "K_as_idl assign:" + hptos idl + " := " + ceToStr rhs 
    | K_pointout(kxr, msg, l_aid, r_aid) -> (if lpl then kxrToStr kxr + " " else "") + sprintf "K_pointout(%s, %s, %s)" msg (aidToStr l_aid) (aidToStr r_aid)

    | K_new(kxr, lhs, nemtok, KN_newobj(idl, hintname_o, stru, _, ats)) -> sprintf "K_new(%s, %s, KN_newobj(%s, ...,%s, ...)" (ceToStr lhs) nemtok (hptos idl) (valOf_or hintname_o "-NOHINT-") 
    | K_new(kxr, lhs, nemtok_region, KN_newarr(ct, a1, nemtok_content, hintname_o, ats)) -> sprintf "KN_new(%s, %s, K_newarr(%s,%s" (ceToStr lhs) nemtok_region (typeinfoToStr ct) (ceToStr a1) + ", " + nemtok_content + "," + (valOf_or hintname_o "-NOHINT-") + "))" + cez_ToStr_atts ats

    | K_as_sassign(kxr, lhs, rhs, aso)  ->
        sprintf " sassign %s := %s\n" (ceToStr_simple lhs) (ceToStr_simple rhs) +
        (if aso.reset_code then " <reset-code>" else "") +
        (if lpl then kxrToStr kxr + " " else "") +
            let stars = 0 in "       sassign: " + (if stars=0 then "" else "Stars=" + i2s stars + " ") + " lhs="  + ceToStr lhs + "\n       sassign rhs=" + ceToStr rhs + "\n"

    | K_as_vassign(kxr, lhs, rhs, idx, aid_o, aso)  ->
        sprintf " vassign %s[%s] := %s // aid_b=%s\n" (ceToStr_simple lhs) (ceToStr_simple idx) (ceToStr_simple rhs) (aidoToStr aid_o) +
        (if aso.reset_code then " <reset-code>" else "") +
           let aido2s aido = (if nonep aido then "-!!" else aidToStr(valOf aido))
           (if lpl then kxrToStr kxr + " " else "") + "       K_as_vassign: lhs=" + ceToStr lhs + "\n       vassign subs=" + ceToStr idx + "\n       vassign rhs=" + ceToStr rhs + "\n       vassign aid=" + aidoToStr aid_o


let kcode_gt_print cos dic len lpl msg =
    let _ : gt_dic_line_t array = dic
    let println ss = youtln cos ss
    let _ = println(sprintf "-------%s---kcode-listing-start-------------------------- %i commands" msg len)
    let list p =
        let h = dic.[p]
        println(sprintf "   000%i:" p + kToStr lpl h.kinstruction)
    app list [0..len-1]
    println(sprintf "-------%s---kcode-listing-end---------------------------- %i commands" msg len)        
    ()


// Resolve a gtrace branch destination within a DIC (directly-indexed code) array of K code.
let resolve_gt_goto_late v pp = function
    | Gt_dic (name, len, dic) ->
        let rec scan p =
            if p>=len then
                let cos = YOVD -1
                kcode_gt_print cos dic len false "assembler-dump"
                sf (name + ": K-code assembler: resolve_gt_goto lab not found: " + hptos v)
            else match dic.[p].kinstruction with
                    | K_lab(kxr, sl) when sl=v -> p
                    | _ -> scan (p+1)
        let aa = scan 0
        let _ = pp := aa
        aa

let gen_gt_label_map = function
    | Gt_dic (name, len, dic) ->
        let rec scan (cc:Map<string list, int>) p =
            if p>=len then cc
            else
                match dic.[p].kinstruction with
                    | K_lab(kxr, sl) ->
                        let cc =
                            match cc.TryFind sl with
                                | Some ov ->
                                    let cos = YOVD -1
                                    kcode_gt_print cos dic len false "assembler-dump"
                                    sf (sprintf "gen_gt_label_map: label defined more than once (DMTO) %s  %i %i" (hptos sl) p ov)
                                | None -> cc.Add(sl, p)
                        scan cc (p+1)
                    | _ -> scan cc (p+1)
        scan Map.empty 0



type kce_walker_t<'WE, 'WC> =
    {
       expf: (cil_eval_t * 'WE list) -> 'WE;
       cmdf: 'WC -> ((int * kcode_t) * 'WE list) -> 'WC;
    }



// Take the max (strongest) of two severities.
let cvt_resolve cvtf = function
    | CS_preserve_represented_value -> CS_preserve_represented_value 
    | CS_maskcast                   -> if cvtf=CS_preserve_represented_value then cvtf else CS_maskcast
    | CS_typecast                   -> if cvtf=CS_preserve_represented_value || cvtf=CS_maskcast then cvtf else CS_typecast




let kce_walk (wlk:kce_walker_t<'WE, 'WC>) (cc0:'WC) cmd =
    let cCC = valOf !g_CCobject
    let rec vexp ce =
        match ce with
        | CE_struct(idtoken, dt, aid, lst)   ->
            let vexp_fb (ptag, dt, ceo) cc = if nonep ceo then cc else (vexp (valOf ceo))::cc
            wlk.expf(ce, List.foldBack vexp_fb lst [])
        | CE_region _ 
        | CE_var _

        | CE_reflection _
        | CE_x _                             -> wlk.expf(ce, [])
        | CE_start(_, csl, _, ce1, args, _)  -> wlk.expf(ce, map vexp (ce1::args)) 
        | CE_conv(_, _, l)
        | CE_dot(_, l, _, _)                     -> wlk.expf(ce, [vexp l])
        | CE_ilit(baser, idx)                    -> wlk.expf(ce, [vexp baser; vexp idx])
        | CE_subsc(_, l, r, _)
        | CE_scond(_, l, r)
        | CE_handleArith(_, l, r)        
        | CE_alu(_, _, l, r, _)                  -> wlk.expf(ce, [vexp l; vexp r])
        | CE_ternary(_, g, l, r)                 -> wlk.expf(ce, [vexp g; vexp l; vexp r])        
        | CE_server_call(id, _, _, sargs, v, args) ->  wlk.expf(ce, map vexp (sargs @ args))
        | CE_apply(_, _, lst)                    -> wlk.expf(ce, map vexp lst)
        | CE_star(_, ce', _) -> vexp ce' 
        | other -> sf ("kce_walker other exp:" + ceToStr other)


    and vcmd cc (n, cmd) =
        match cmd with
        | K_topret(_, None)
        | K_lab _
        //| K_linepoint _
        | K_new _  
        | K_cmd _  
        | K_pragma _         
        | K_goto(_, None, _, _)
        | K_remark _ -> wlk.cmdf cc ((n, cmd), [])
        | K_goto(kxr, Some(BCOND_diadic(dop, l, r)), sl, _) -> wlk.cmdf cc ((n, cmd), [vexp l; vexp r])
        | K_goto(kxr, Some(BCOND_guarded(_, ce, _)), _, _)
        | K_topret(kxr, Some ce)
        | K_easc(kxr, ce, _)              ->  wlk.cmdf cc ((n, cmd), [vexp ce])

        // NB stars is ignored again:
        //| K_as_sassign(kxr, CE_subsc _, rhs)   // This poor form would have potential lmode confusion but we sort it out in cmdf

        | K_pointout(kxr, msg_, lhs, rhs)     ->  wlk.cmdf cc ((n, cmd), [])        
        | K_as_sassign(kxr, lhs, rhs, _)      ->  wlk.cmdf cc ((n, cmd), [vexp rhs])
            
        | K_as_vassign(kxr, lhs, rhs, idx, _, _) ->  wlk.cmdf cc ((n, cmd), [vexp rhs; vexp idx])

        | other -> sf ("kce_walker other kcode command:" + kToStr true other)
    vcmd cc0 cmd

    


//
// addressable (dereferencable) predicate - deprecate?
//
// Separate av from uav for env evolve and lookup purposes.  In type-unsafe code the distinction is blurred, so ... please explain.
// I think our nemtoks are sufficiently fine for this to be abandond now.
let rec ava_scan_ = function
    | CE_conv(_,  _, v)      -> ava_scan_ v
    | CE_dot(ttt, v, _, aid) -> ava_scan_ v
    | CE_subsc(_, v, _, _)   -> ava_scan_ v                
    | CE_region _            -> Some true
    | CE_var _               -> Some false // On the basis we do not take addresses of variables! (except for full-symbolic pass-by-reference mechanisms).
    | other -> (vprintln 0 ("ava_scan other: " + ceToStr other); None)


// Generic ce to type mandrake miner.  cf get_ce_content_type
let rec get_ce_type2 m1 m2 arg =
    let rec get_ce_type_serf stars = function 
        | CE_apply(CEF_bif fid, cf, args)            -> valOf_or_fail ("no fid type for " + fid.hpr_name) fid.rt
        | CE_apply(CEF_mdt mdt, cf, args)            -> (* ("no mdt type for " + mdt.uname) *)mdt.rtype 
        | CE_reflection(dt, _, _, valo, ats) -> dt
        | CE_handleArith(dt, l, offset)      -> gec_CT_star stars dt // or Object infact, pre user cast.
        | CE_alu(dt, oo, _, _, aidl)         -> gec_CT_star stars dt
        | CE_typeonly dt                     -> gec_CT_star stars dt
        | CE_ternary(dt, _, _, _)
        | CE_x(dt, _)                        -> gec_CT_star stars dt        
        | CE_var(vrn, dt, idl, fdto, ats)    -> gec_CT_star stars dt
        | CE_region(uid, dt, _, _, _)        -> gec_CT_star stars dt
        | CE_conv(dt, _, expr_)              -> gec_CT_star stars dt
        | CE_star(n, ce, _)                  -> get_ce_type_serf (stars+n) ce
        | CE_subsc(array_dt, _, _, _)        -> get_content_type_from_type m2 array_dt    
        | CE_dot(record_dt, ce, (_, fdt, _, field_dt, _), aid) -> gec_CT_star stars field_dt
        | CE_struct(idtoken, dt, _, _)                -> dt
        | CE_ilit(baser, _)                  -> get_ce_type_serf (-1 + stars) baser
        | CE_scond _                         -> g_canned_bool
        //W_string(s, a, b)) -> f_canned_string (String.length s)
        | other -> sf (sprintf "%s: %s: get_ce_type other form: other=%A" m1 m2 (ceToStr other))
    get_ce_type_serf 0 arg


// Container type to content type mapper. 
and get_content_type_from_type msg arg =
    match arg with
    | CT_arr(ct, _) -> ct
    | CT_star(_, atype) -> get_content_type_from_type msg atype // Just discard stars before dereferencing.
    | CT_cr crst ->
        match crst.crtno with
            | record -> get_content_type_from_type msg record
            //| None -> muddy (msg + sprintf ": need to look in crst")
    | CTL_record(idl, cst_, items, len, _, binds, _) ->
        match binds with // TODO why not get from items .... do we really need binds ever ?
            | (v, dt)::_ -> dt
            | _ -> sf (msg + sprintf ": bad record binds %A " arg)
    | other -> sf (msg + sprintf ": get_content_type_from_type: no content type found in other=%A" other)


let get_ce_type msg = get_ce_type2 msg ""
// Basic points-to/address-taken analiser.
(*

Basic points-to analysis: We have pairs of classes, pointers and targets (pp, tt) of aid
   strings : each item in tt is pointed to by an expression in pp a
   pointing expression typically points at more than one item, tt
   contains all items possibly pointed at by anything in pp.  No aid
   is in more than one pair's pp set or in more than one pair's tt set
   and pairs must be conglomorated when such a situation is
   encountered.

   The pp and tt classification are both partitions of the aid space, but they are rarely the same
   as each other.  For instance, all variables pointed at by a particular class do not
   generally themselves point to a common class.

   Variables that are in no target class are called 'nonataken' variables.  Regions are never such.

   For each of member of pp we decide whether every pointing operation it makes is strictly decidable
   at compile time. If so, then we do not have to restrict the symbolic elaboration when it happens..

   Volatile ... ?

   Aims:
    1. indicate/decide how to represent a var (region or var) (vars in no target class can be CE_var)
    2. to stop kill of limit int when store to some other int array
    3. We can also simplify the memory region anal as needed by repack.

   Aim 2 is when we store a varidx we must kill all env entries in same target class.
   We store the env during cilnorm partitioned by target class.


    Could perhaps work on digrams or other nemtok forms

    can you make a pathological example here where nemtok is not sufficient and control flow could usefully be taken into account ...

    : a class and its field is a sort of digram

     Pointer classification uses equivalence classes and { e | p(e) <-> p(e') => blah }

     Each expression is also in a target class.
        
     If | p(e)) |  > 1 then we do a class merge so that only one result exits. Each expression leads to at most one pointer class (e.g. if p(x?y:z) occurs ever then it implies that p(y)=p(z) )

    Case of reference taken "V0 := &V1"  p(V0) .=. t(V1) ??? NO
    Case of dereference taken "V0 := *ei"  p(V0) .=. t(ei)    

    Case of monadic pointer deref:
     This occurs in C# using stind, so it makes sense to write "*ei := er" in pseudocode.
     Here the t(eo) .=. p(er)

     How does this work for invalidation ?  Well if el is not a compile-time constant we do not
     know what is addressed, so all items x where t(x)=t(el) are clobbered (all in same target class).
    
     When a pointer is dereferenced inc C# (ei.field := V1) then we have two associations p(field)<->p(V1) and t(V0) .=. p(ei) ?? 

    
     "(el)[es] := er"    el will be evaled to, e1, and we expect p(el)<->p(e1) owing to the way V0 was assigned.  Equally, t(el) = t(e1) .=. p(er).  We only make range assertions about es.  
     
     "vl := (ea)[es] + ef"   if arith is allowed p(vl) = p(ef) = t(ea), 


   A static field certainly has an nemtok based on its static name idl but if it is referenced we instead use its dtidl/hidx form for code generation.

   A static field that is part of a static instance of a complete class (or struct) can be referenced if that class is referenced.

   A static field can be referenced and hence may be subject to undecidable arith

   an instance field is typically named by its type and hidx, but does it always have an aid ?  
   
   If we define a UAV as something that is never address taken then it will not serve for static fields in general.

   I think we need a complete symbol table ... uav's are just names for its entries but nearly everything needs an address...

   We'd like to work only in terms of symbolic names, but future support of C++ means that a full machine image is worth preserving.
   Therefore we have full machine addresses as the underlying mechanism but use symbolic names as much as possible.

*)

let rec cez_ats = function
    | CE_var(_, dt, _, _, ats) 
    | CE_region(_, dt, ats, _, _) -> ats
    //| CE_tv(t, v) -> cez_ats v
    | other -> sf ("cez_ats other " + ceToStr other)


// This is the arith-dontcare SCX storage class.
// This entry can be present in multiple storage classes without them becoming conglomorated.
let g_aid_arith_scx = { g_null_aid with  la_arithx=true }
let g_aid_anon = g_aid_arith_scx



// Return the naming token for the dataflow returned by an expression (regardless of instance, but possibly uniqfied by callsite).
// Really this is the aid for an indexing operation or an indexed object: we can be as precise or slack as we like but must be based on textual context?  If we are a bit imprecise then we will have larger name alias sets, reducing what can be disambiguated.
// There is at most one lhs in the typical assignment but there can be serveral rhs subscription operations that may have different names.  
// Now that intcil does not use an elaboration env the aid in the ce forms is fairly redundant.  It could all be inferred from the concrete kcode, prior to register colouring and symbolic eval.  But the stored aid's in the CE_subsc and CE_dot forms do speed up the store0/store1 dataflow extraction pass called 'populate ataken'.
// Returns aid_t : (regf, via, stars, nemtoks)
// Return region flag boolean: regf holds for all non-scalar forms.

// Wierd coding: offsets input stars rather than modifies on return...


let nemtok_define (msg:string) giveclass freshtok = // The lowest leaf add
    let cCC = valOf !g_CCobject
    let ans = A_loaf freshtok
    let _ = cCC.kcode_globals.nemtok_details.add freshtok  { g_null_aid with la_nemtok=Some freshtok } 
    let cls = cCC.kcode_globals.ksc.classOf ans
    (ans, cls)

// Enter a pair of aids in a common class. If both already in different classes then merge those classes.
// A nemtok_t is a string list.
let nemtok_add_pair msg (n1, n2) =
    let cCC = valOf !g_CCobject
    let gl = cCC.kcode_globals
    let vd = !g_ataken_vd
    let msg = "nemtok_add_pair"
    let sci = cCC.kcode_globals.ksc.conglom msg [n1; n2]
    let scs = cCC.kcode_globals.ksc.sciToStr sci
    scs:string


// 
// Algebraic system of nemtoks: Deca moves down from control to data
let anon_deca_for_vassign sitemarker aid =
    //A_tagged(aid, g_indtag, sitemarker)
    vprintln 3 "Anon deca used" // Dont like the anon ones.
    A_subsc(aid, None)
    
let anon_dereference_aid_core msg_ = anon_deca_for_vassign 
    
let deca_for_dotassign (ptag, utag) x = A_tagged(x, ptag, utag)


    
//  Algebraic system of aids:
//  Move upwards from data to control by removing [] and -> wrappers 
let inca_for_vassign msg sitemarker = function
    | A_tagged(v, _, _) -> v
    | A_subsc(v, _)     -> v
    | other ->     // Or add a new nemtok and peer A_ind(freshtok) with our arg
        let cCC = valOf !g_CCobject
        let gl = cCC.kcode_globals
        match cCC.kcode_globals.aid_derefs.lookup other with
            | Some ov -> ov
            | None ->
                let freshtok = funique "nemstar"
                let (ans, _) = nemtok_define msg false freshtok
                //let ians = A_tagged(ans, g_indtag, sitemarker)
                let ians = A_subsc(ans, None) // When used for a dot tag this is a little odd...
                dev_println "Anon deca rezzed." // Dont like the anon ones.
                let msg = msg + sprintf ": Logging extra aid pair (pointout) gec_CE_star clause: newly peered aids are:\n    aid_arg=%s\n    aid_fresh=%s" (aidToStr other) (aidToStr ians)
                let scs = nemtok_add_pair msg (ians, other)
                vprintln 3 (sprintf "%s in class %s" msg scs)
                gl.aid_derefs.add other ans
                gl.aid_refs.add ans other // add_pair should have done this update
                ans
                


let ce_aid0 (mf:unit->string) arg =
    let cCC = valOf !g_CCobject
    let vd = !g_ataken_vd
    let saveaid nemtok arg =
        cCC.kcode_globals.nemtok_details.add nemtok arg
        Some(A_loaf nemtok)

    let rec ce_aid site__ arg =
        match arg with
        | CE_handleArith(dt, baser, offset_) -> ce_aid site__ baser
        | CE_ilit(baser, idx)                -> ce_aid site__ baser        
        | CE_conv(dt_, _, casted_arg)        -> ce_aid site__ casted_arg


        | CE_struct(idtoken, dt, aggregate_aid, items)  -> Some(A_loaf aggregate_aid)  // Nominally take first. TODO discuss: we really want the keep a tagged list and keep things parallel all the way to the consumer ... that would track the dataflow properly. We instead use the aggregate_aid

        // Address-of and also dereference operator (positive n is & and negative is *).
        // When we take the address of 
        | CE_star(1, ce, aid)     -> Some aid //TODO this
        | CE_star(stars, ce, aid) ->
            dev_println (sprintf "ice_aid0: ignored stars %i on %s for ce_aid0" stars (ceToStr ce))
            Some aid



        | CE_reflection(_, Some dt_, None, _, _)            -> None // Just a type token.
        | CE_reflection(_, None, Some idl, _, _)            ->
            let nemtok0 = hptos (idl @ [ "KiwiC-Reflection" ])
            saveaid nemtok0 { g_null_aid with la_nemtok=Some nemtok0; la_regionf=false } 

        // K_pointout ("array_declaration", A_ind(A_loaf nemtok_region), A_loaf nemtok_content)

        // The nemtoks held in a CE_region, and its partner CE_newobj, are for the storage and the individual contents are give flow identifiers using this and the A_tagged wrapper.
        // These two have an implied reference operator and hence add one. - But we ignored the star operator. TODO.
        //| CE_newobj(nemtok0, idl, hintname, stru, mgr1, ats) -> saveaid nemtok0 { g_null_aid with la_nemtok=Some nemtok0; la_regionf=true } 
        // An array could get a nemtok for its storage via the immediate nonreftrans holding register applied to it. 
        //| CE_newarr(ct, len, (nemtok_region, nemtok_content), _, ats) -> saveaid nemtok_region { g_null_aid with la_nemtok=Some nemtok_region }

       
        | CE_region(uid, dt, ats, nemtok_option, srccode) ->
            match nemtok_option with
                | Some nemtok0 -> saveaid nemtok0 { g_null_aid with la_regionf=true; la_nemtok=Some nemtok0; } 
                | None -> None 
        
        | CE_var(vrn, dt, idl, fdto, ats) ->
            //vprintln 3 (sprintf "vrn V%i aid yielded as leaf %s" vrn (hptos idl))
            let nemtok0 = hptos idl
            saveaid nemtok0 { g_null_aid with la_nemtok=Some nemtok0 }


        | CE_x(ty_, W_string(s, a, b)) ->
            let nemtok = s
            saveaid nemtok { g_null_aid with la_regionf=true; (* la_string=true *) la_literalstring=true; la_romf=true }

        | CE_x(_, X_pair(W_string(s, a, b), r, _)) ->
            //vprintln 0 ("aid other: string " + xToStr(W_string(s, a, b))) // TODO why different from W_string above ...?
            None

        | CE_x(_, X_net(ss, _)) ->
            if strlen ss>4 && ss.[0..3] = "SRI_" then Some(A_loaf (ss.[4..]))
            else
                sf (sprintf "Cannot remove SRI_ from " + ss)


        | CE_x(_, other) ->
            //vprintln 0 ("aid other: lowered x " + xToStr other)
            None

        // For Kiwi-2 We need to track nemtok through 'unsafe' pointer arithmetic - add and subtract.
        // This is also needed for gcc4cil input where arithmetic on array bases or literal string pointers is performed.
        | CE_alu(rt, oo, lhs, rhs, aido) -> aido


        | CE_apply(_, cf, sri_arg::_) when not_nonep cf.tlm_call ->
            ce_aid site__ sri_arg

        
        | CE_apply(CEF_bif gis, cf, args) when gis=g_bif_hpr_alloc -> // CE_newobj or CE_newarray might mask this form at kcode stage
            muddy ("ce_aid0: need to note the offchip aid for hpr_alloc: if you are not expecting to use any run-time heap then ... check your static allocations are in in lasso stem for ... rest of message missing " + ceToStr arg + mf())
            // unfinished - there is an aid in the alloc potentially ...
  
                
        // The following forms return the arith-dontcare SCX storage class.
        | CE_start _
        | CE_apply _ -> None // A_loaf{ g_null_aid with la_arithx=true}]// This is the arith-dontcare SCX storage class. 

        | CE_scond(_, l, r) ->
            let la = ce_aid (fun ()->mf()+":leftop") l
            let lr = ce_aid (fun()->mf()+":rightop") r // Conditional sub-expressions - multiplexer. Should take list union not append.
            match (la, lr) with
                | (None, lr) -> lr
                | (la, None) -> la
                | (Some la, Some lr) ->
                    let _ = nemtok_add_pair "CE_scond" (la, lr)
                    Some la

        // OLD: Object records are inserted by intcil giving rise to the following form
        // that cannot be accessed for nemtok pre eval.  So we had a special rule:
        //| CE_dot(CT_star(1, CTL_record(idl, _, _, _, _, binds, _)), CE_region(uid, dt, ats, nemtok, srccode), (_, tag_id, _, _), aid_) when false -> // TODO delete me
        //vprintln 3 ("Invoking region object rule for " + hptos nemtok)
        //sf "[([], 1, nemtok)]"

        | CE_dot(CT_star(1, CTL_record(idl, _, _, _, _, binds, _)), ll, (_, fdt, tag_id, _, _), aid_) // Note the aid_ here is ignored. Please explain. We use it in the CE_subsc case. TODO.

        | CE_dot(CTL_record(idl, _, _, _, _, binds, _), ll, (_, fdt, tag_id, _, _), aid_) // We are ambivalent about the star in the expression but we track accurately the stars in the ns 
            ->  if true then // TODO this ignores the aid_ cached field ... please use
                // DefinitiveS: we remove one star at a dereference if we recalc from l.
                    match ce_aid (fun ()->mf()+":ce_aid dot rec-call") ll with
                        | None -> None
                        | Some aid -> Some(deca_for_dotassign tag_id aid)
                else muddy "aid_do clause " // its not the field_aid_t type, its the silly branded     (bool * int * string list) list    

        // Note that aid of A[B].C can be kept separate from that for A[B.C]
        | CE_subsc(_, l, idx, aid_o) -> // aid is already present - just return it.
            // These have been deca'd on rezing of the CE_subsc. 
            // DefinitiveS: we remove one star at a dereference if we recalc from l or idx.
            aid_o



        | other -> sf ("ce_aid0 other " + ceToStr other)
    let ans = ce_aid mf arg
    if vd >= 4 then vprintln 4 ("ce_aid0 '" + mf() +  "' applied to " + ceToStr arg + "\n   ce_aid0 returned=" + aidoToStr ans)
    ans


// Was used for stelem and stind: Find a nemtok using some hunting perhaps ?  DELETE ME?
let no_aid_legacy mf ce =
    let s_aid = ce_aid0 mf ce  // we want the 'subscript' aid before eval of lhs - that's all done now via the DFA between kcode generate and kcode eval.
    let aid =
        if nonep s_aid then
            vprintln 0 (sprintf "full arg ce=%A" ce)
            sf(mf() +  sprintf ": no_aid: not precisely one nemtok for ptr base ce=%s aids=" (ceToStr ce) + aidoToStr s_aid)
        else valOf s_aid
    //vprintln 0 ("no_aid used:" + mf())
    aid

let ce_aid_lst mf ce =
    match ce_aid0 mf ce with
        | None     -> []
        | Some aid -> [aid]


let rec is_structsite msg = function
    | CT_cr cst -> is_structsite_1 cst.crtno
    | _ -> false
and is_structsite_1 = function
    | CT_class cst -> cst.structf
    | _ -> false

let rec dt_is_struct msg = function
    | CT_cr cst      -> dt_is_struct msg cst.crtno
    | CT_class cst   -> cst.structf
    | CTL_record(idl, cst, items, len, _, binds, _) -> cst.structf
    | _ -> false



let follow_knot msg = function
    | CT_knot(idl, whom, vv) when not_nonep !vv ->
        valOf !vv
    | other -> other



let dt_is_structptr msg dt =
    match follow_knot msg dt with
        | CT_star(n, dt) when abs n = 1 && dt_is_struct "dt_is_structptr" dt -> true
        | _ -> false
            

let ce_int64 (i64:int64) = CE_x(g_canned_i64, xi_int64 i64) // 




let isone_failf thunk = function
    | [item] -> item
    | _ -> sf(thunk())



// KiwiC compiler algorithm for deciding the number of bits to use in a pointer variable.  This also serves
// to define an upper bound on the number of RAMs and/or register banks and the minimum size of each.
//
// Basic data types are scalar variables and arrays and structures thereof.  Each variable is either a reference type or a valuetype (int, char etc.).

// Expression forms are
//      v       : read a scalar (statically allocated in the source code)
//     e[e']    : read from a 1D vector (e must evaluate to an array)
//     e.id     : read from field id of structure pointed at by e (id is a constant string)
//     &e       : take address of an expression which should be a scalar v or of the form e?v:v'
//     e?e'?e'' : conditional expression
//     new O    : new object of type O
//     new A e  : new array of length e (e is compile time constant)
//     e op e'  : function application and arithmetic operators (no pointer arithmetic however).
//     e == e'  : comparison of pointers
//     null     : a special pointer value

// Command forms are:
//    v := e    : Assign to scalar
//   *e := e'   : Assign via a pointer
//   e.id := e' : Assign to record field

// Control flow is totally neglected for this analysis, so there are no control flow commands listed above, but the expressions in conditional branches are considered as though they were right-hand-sides of assignments to a fresh dummy scalar.
// Including control flow could clearly give a different answer when unreachable code is present, but other cases would be interesting to discuss.

// Every static scalar is allocated a unique, opaque naming token (nemtok)
// Every array or record returned by new is nemtok tagged with the call site of that new invocation.

//
// Address taken storage classes are used. These are equivalence classes.
// The purpose of storage classes is to track what possible values of array subscript might be used.

// Each nemtok is initially in its own storage class. And each class initially points to no others.
// We scan the assignments and all subexpressions on right-hand-sides and in lhs subscript expressions a number of times merging the classes and setting up points-to relationships between them. Order is not important. The number of scans required depends on the maximum level of indirection in the program.

// On the first, pass we merge (identify on) direct comparisons, multiplexors and assignments.
// All nemtoks that appear on alternate hands of a given conditional expression or each side of an equality comparison
// are merged to a common class. For example v0 := (v1==v2) ? v3: v4 will merge  the classes of v1 and v2 and merge the classes of v3 and v4 and then this will be merged with v0.

// A dereference operation defines a pointed at relationship or merges existing pointed at relationships:
// For example   v1 := &v2  combines the storage class of v2 with the pointed at class of v1.
// Also          *v3 := v4  or v4 := *v3 combine the storage class of v4 with the pointed at class of v3.

// I need to write up the rules about field access and array subscription... TODO.
//
//
//   Example
//     v0 := {"c55", 401010}           // An object heap address returned by newobj
//     v1 := @BOOL[v0 + 51]            // A boolean field in an object instance - this v0 should keep its class


// On the second pass, all singly indirect exchanges by assignment or equality comparison merge the refered-to classes.  For instance *v1 := *v2 or *v1 == *v2 will merge the classes pointed at by v1 or v2

// On the third pass we handle **v1 = **v2 and so on.


// At the end of the procedure we know the arity of each pointer (i.e. how many different things it might point to at run time and if this is only one then it is a constant that can be removed during the compilation and the pointed at object does not need to be placed in a RAM or equivalent multiplexed register structure.




// Dataflow and storage class tracking. Local form.
let setup_kcode_globals () =
        {
            ksc=            new ksc_classes_db_t("kiwife-classstore", 'c')
            pointstyles=    new ListStore<string, (string *string)>("pointstyles") 
            nemtok_details= new OptionStore<new_nemtok_t, aid_leaf_attribute_t>("nemtok_details")
            aid_derefs=     new OptionStore<aid_t, aid_t>("aid_derefs")   //
            aid_refs=       new ListStore<aid_t, aid_t>("aid_refs")       // Points-at
        }



//
// Rebuild an aid using our prefered representation, pairing intermediate forms as we go
//
let aid_logger ranges_over_nullf is_static_cachef aid000 =
    let vd = !g_ataken_vd
    let cCC = valOf !g_CCobject
    let gl = cCC.kcode_globals
    let msg = "aid_logger"
    let sci = cCC.kcode_globals.ksc.classOf_i msg aid000
    if ranges_over_nullf then
        //vprintln 0 (sprintf "Ranges_over_null true %A" aid000)
        cCC.kcode_globals.ksc.setNullFlag msg aid000
    if is_static_cachef then
        cCC.kcode_globals.ksc.attribute_set sci g_assign_once_cache_var "true"
    cCC.kcode_globals.ksc.sciToStr sci
    
// encounter a potentially new aid - assign a class for it if does not have one.
let aid_add_solo msg aid = 
    let vd = !g_ataken_vd
    let scs = aid_logger false false aid
    if vd>=4 then vprintln 4 (msg + sprintf " aid_add_solo : %s:  scs=%s  aid=%s" msg (scs) (aidToStr aid))
    scs


let nemtok_mark_ranges_over_null_ msg aid = 
    let vd = !g_ataken_vd
    let msg = "nemtok_mark_ranges_over_nul"
    let scs = aid_logger true false aid
    if vd>=4 then vprintln 4 (msg + sprintf " mark_ranges_over_null : %s:  scs=%s  aid=%s is_null" msg (scs) (aidToStr aid))
    ()




//
//  See if a nemtok ever has its address taken (i.e. is in a target class).  If never, return true.
//  If not defined yet, make a decision based on its default.
//
type ataken_flag_t = { nontaken: bool option; regf: bool; literalstringf: bool }

let g_ataken_base = new OptionStore<new_nemtok_t, ataken_flag_t>("g_ataken_base") // TODO: this is global over all HPR VMs so put in meox.fs please.
let g_ataken_deflt = { nontaken=None; regf=false; literalstringf=false; } // TODO enclose the aid_leaf here directly

// Copy the regionflag from A_loaf into the ataken_base.
// Probably this code is no longer needed since we get LA_REGION correct to start with.
let insert_regionf aid_ats =
    ()
#if SPARE    
    let regionf = aid_ats.la_regionf
    let aid = aid_ats.la_nemtoks
    let d1 = if aid_ats.la_literalstring then { g_ataken_deflt with literalstringf= true} else g_ataken_deflt
    let biz nemtok =
            match g_ataken_base.lookup nemtok with
                | Some _ -> ()
                | None ->
                    let vv = if aid_ats.la_regionf then { d1 with nontaken=Some true; regf=true } else d1
                    g_ataken_base.add nemtok vv
    app biz aid_ats.la_nemtoks 
#endif

// Return true if singleton (only one of these exists) and it never has its address taken.
let nonatakef nemtok baser =
    let vd = -1
    if baser=g_unaddressable then true // no longer used? TODO. - indeed all of this ataken database is no longer really used?
    else
        let ov = 
            match g_ataken_base.lookup nemtok with // g_ataken_base to be moved tin kcode_globals
                | Some ov -> ov
                | None ->
                    let hs = isHeap baser               // TODO - deprecated?
                    if vd>=4 then vprintln 4 (sprintf "aid was not yet inserted in ataken base: aid=" + nemtok + "  baser=" + i2s64 baser + "  heapspace=" + boolToStr hs)
                    // For an unknown item, if it is on the heap we default to 'region' status so it is 'ataken'
                    { g_ataken_deflt with regf=hs }
        match ov.nontaken with
            | Some ans -> ans
            | None -> // not decided yet
                let cCC = valOf !g_CCobject
                let ksc = cCC.kcode_globals.ksc
                let aid = A_loaf nemtok
                let onc = ksc.classOf_q aid
                let (ans, m) =
                    if nonep onc then (not ov.regf, Memdesc_sc 0) // Not registered as a target, non-ataken if not a region (so far. Make decision persist).
                    else (false, Memdesc_scs(ksc.sciToStr (valOf onc)))
                g_ataken_base.add nemtok { ov with nontaken=Some ans }
                if vd>=4 then vprintln 4 ("nonataken oracle:  aid=" + aidToStr aid + "   md=" + mdToStr m + "  nonataken=" + boolToStr ans)
                ans


let f_nonataken (uid:vimfo_t) = nonatakef (hptos uid.f_name) uid.baser


// We need all the static class constructors to be considered the same thread.
// For volatile considerations, two threads are considered the same if they are both cctor threads or one is and the other is a main thread, or indeed they are the same.
// For recompile considerations two threads are the same if they _are_the same.
let samethread volatile_considerationsf (a:tid3_t) (b:tid3_t) =
    //if a.no <> b.no then dev_println (sprintf "samethread?  %A  cf %A"  (tid3ToStr a) (tid3ToStr b))
    let ans =
        a.no = b.no ||
        (volatile_considerationsf  &&
         (
            a.specific=S_kickoff_collate && b.specific=S_kickoff_collate||
            a.specific=S_kickoff_collate && b.specific=S_root_method||
            a.specific=S_root_method     && b.specific=S_kickoff_collate))
    ans


let is_static_cache = function
    | CE_dot(ttt, ce, (tag_no, Some fdt, (ptag, utag), dt_field, n), aid_o) when fdt.static_cache_f -> true
    | CE_var(vrn, dt, idl, Some fdt, ats) when fdt.static_cache_f -> true    
    | _ -> false


// Collect l and r mode static support. Note, in kcode, all variables (CE_var form) are static.
// Not used?
let rez_kcode_static_support_sets_walker () =
    let static_lhs_set = new OptionStore<stringl, cil_eval_t>("static_lhs_set")
    let static_rhs_set = new OptionStore<stringl, cil_eval_t>("static_rhs_set")

    let cmdf auxarg_ ((_, cmd), gens1) =
        match cmd with
            | K_as_sassign(kxr, ((CE_var(vrn, dt, idl, fdto, ats)) as ce), rhs, abuse) ->
                static_lhs_set.add idl ce
                //vprintln 0 (sprintf "lcode_collect_static_support lhs %s" (hptos idl))
                auxarg_
                
            | _ -> auxarg_

    let expf (ce, sons_) =
        //dev_println(sprintf "kcode walk look for hit hatty: " + ceToStr ce)
        match ce with
            //| CE_apply(CEF_mdt mdt, cf, args) when not_nonep mdt.fu_arg -> 
            | CE_var(vrn, dt, idl, fdto, ats) ->
                static_rhs_set.add idl ce
                //vprintln 0 (sprintf "lcode_collect_static_support rhs %s" (hptos idl))
                ()

            | _ -> ()
            
    let wlk =
        {
            cmdf= cmdf
            expf= expf
        }
#if SPARE
    match kcode with
        | Gt_dic(name, len, array) ->
            // FSharp fold over array - there is nicer syntax of course...
            let biz n = kce_walk wlk () (n, array.[n])
            app biz [0..len-1]
#endif
    (static_lhs_set, static_rhs_set, wlk)


// Get the support (set of free variables) for a kcode command.
let kcode_collect_static_support ww msg cmds =
    let ww = WF 3 "kcode_collect_static_support" ww "start"
    let (static_lhs_set, static_rhs_set, wlk) = rez_kcode_static_support_sets_walker ()   
    app (kce_walk wlk ()) cmds
    (static_lhs_set, static_rhs_set)


let rec thread_memberp volf (item, f) lst =
    let rec scan = function
        | (h, _)::tt when samethread volf h item -> true
        | _::tt                             -> scan  tt
        | []                                -> false
    scan lst


// For all A_loaf nemtoks we report them in terms of their storage class.  For higher members we rewrite them in terms of leaf storage classes.

// We maintain lists of reading and writing threads.
// When we add a writer where a reader has relied on it being non-volatile, then that thread needs recompiling.
// When we add a store of a new value where threads have relied on a monoassign value, then the relying threads need recompiling.
// We could avoid recompiles by scanning all of the kcode first, but we generate the kcode on-the-fly.        
let rhs_constvol_noter2 m_shared_var_fails thrd_id nemtok debug_info =
    let cCC = valOf !g_CCobject    
    let vd = cCC.settings.constvol_loglevel
    let dbase = cCC.constant_or_volatile
    let volf = true
    match cCC.constant_or_volatile.lookup nemtok with
        | None ->
            if vd>=5 then vprintln 5 (sprintf "constvol_tracker: r-mode first noted for read of nemtok=%A  site=%A" nemtok  thrd_id)
            let tok = (thrd_id, true(*We note we have relied on it being non-volatile*))
            dbase.add nemtok ([tok], VOC_wr_none) 
            false // It is not volatile.

        | Some (readers, VOC_wr_none) ->
            let tok = (thrd_id, true)
            if thread_memberp volf tok readers then ()
            else
                if vd>=5 then vprintln 5 (sprintf "constvol_tracker: r-mode another reader but no writes yet for nemtok=%A  site=%A" nemtok  thrd_id)
                dbase.add nemtok (tok::readers, VOC_wr_none)
            false

        | Some (readers, VOC_monoassign site) when samethread volf thrd_id (fst site)->
            if vd>=5 then vprintln 5 (sprintf "constvol_tracker: r-mode subsequent intra-thread read of nemtok=%A site=%A" nemtok  thrd_id)
            let tok = (thrd_id, true)
            if thread_memberp volf tok readers then ()
            else dbase.add nemtok (tok::readers, VOC_monoassign site)
            false

        | Some (readers, VOC_monoassign site) when not (samethread volf thrd_id (fst site)) ->
            if vd>=5 then vprintln 5 (sprintf "constvol_tracker: r-mode inter-thread nemtok=%A  site=%A" nemtok  thrd_id) 
            true

        | Some(readers, VOC_multi_thread_assign _) -> true

        | Some(readers, VOC_multi_value_assign sites) -> // If updated by another thread then it is volatile. 
            let rec scanner = function
               | []                                               -> false
               | (thrd_id', line_no)::tt when not (samethread volf thrd_id thrd_id') -> true
               | _ :: tt -> scanner tt
            let volatilef = scanner sites

            if not volatilef then
                let tok = (thrd_id, true)
                if thread_memberp volf tok readers then ()
                else dbase.add nemtok (tok::readers, VOC_multi_value_assign sites)
            volatilef
//               [("TTX403", "K0004:"); ("TTX403", "K00014:")],false))

        | ov -> sf(sprintf  "pingk voc other %A" ov)



let generic_constvol_tracker ww m_shared_var_fails site tid vtag volatilef full_correction_neededf left_nemtok = // Track which (scalar) variables are assigned only once and/or shared between threads.
        let cCC = valOf !g_CCobject
        let vd = cCC.settings.constvol_loglevel
        let one_thread_only sites = length (list_once(map fst sites)) < 2
        if vd>=4 then vprintln 4 (sprintf "constvol_tracker: generic/lhs volaltilef=%A write %s  vd=%i" volatilef left_nemtok vd)
        let txToStr (tid_3, vidx) = tid_3.id

        //dev_println "volf/volatilef"
        let volf = true // not being used now .. . FOR NOW - this is the comparison basis not the external assertion that it is volatile

        let codesite = (tid, vtag)
        if vd>=5 then vprintln 5 (sprintf "constvol_tracker: lmode codesite=%A lhs_nemtok=%s" (fst codesite).id left_nemtok)
        let kill killed_tid =
            let token = (killed_tid.id, "kill")
            if memberp token !m_shared_var_fails then ()
            else
                if vd>=2 then vprintln 2 (sprintf "constvol_tracker: recording recompile (kill) needed for tid=%s" (killed_tid.id))
                mutadd m_shared_var_fails token



        let apply_qkill sites = // This should be all reader and writes sites
            let qkill (tid_3, vidx) =
                (*if tid_3.no <> tid.no then*) kill tid_3
            app qkill sites

        match cCC.constant_or_volatile.lookup left_nemtok with
            | None ->
                if vd>=5 then vprintln 5 (sprintf "constvol_tracker: l-mode first noted for write to nemtok=%A  site=%A" left_nemtok codesite)
                cCC.constant_or_volatile.add left_nemtok ([], VOC_monoassign codesite)

            | Some(readers, VOC_wr_none) ->
                let lkill cc (thread, reliedf) =
                    if samethread volf thread tid then cc
                    elif reliedf then 
                        kill thread
                        cc
                    else (thread, reliedf)::cc
                let readers = List.fold lkill [] readers
                // If another thread relied on this being non-volatile, then flag to go back and recompile earlier thread.
                if vd>=5 then vprintln 5 (sprintf "constvol_tracker: first write noted for nemtok=%A  site=%A" left_nemtok (site))
                cCC.constant_or_volatile.add left_nemtok (readers, VOC_monoassign codesite)
                    
            | Some(readers, VOC_monoassign codesite') ->
                let rxToStr (tid_3, reliedf) = sprintf "%s:%A" tid_3.id reliedf
                let samef = (codesite = codesite') // Assigned in one place is not the same as assigned only one value!  TODO fix and then start using the constant inferences.
                if samef then ()
                else
                       //dev_println ("ataken dbase tie in please") // TODO
                       let sites = [codesite; codesite']
                       let onethreadf = one_thread_only sites
                       if vd>=5 then vprintln 5 (sprintf "constvol_tracker: multi assign sites noted for nemtok=%A  onethreadf=%A readers=%s" left_nemtok onethreadf (sfold rxToStr readers))
                       if onethreadf then
                           if vd>=5 then vprintln 5 (sprintf "constvol_tracker: multiassign noted for nemtok=%A  sites=%A" left_nemtok (sfold txToStr sites))
                           cCC.constant_or_volatile.add left_nemtok (readers, VOC_multi_value_assign sites)
                           if full_correction_neededf then apply_qkill sites
                       else 
                            let lkill cc (thread, reliedf) =
                                if samethread volf thread tid then
                                    if vd>=5 then vprintln 5 (sprintf "constvol_tracker: nemtok=%A.  Thread=%A. Local nop." left_nemtok (thread)) 
                                    if full_correction_neededf then apply_qkill sites
                                    cc
                                elif reliedf then 
                                   if vd>=5 then vprintln 5 (sprintf "constvol_tracker: nemtok=%A.  Thread=%A. Did rely. Must recompile." left_nemtok (thread)) 
                                   if full_correction_neededf then apply_qkill sites
                                   kill thread
                                   cc
                                else
                                    if vd>=5 then vprintln 5 (sprintf "constvol_tracker: nemtok=%A.  Previous %A did not rely on non-volatile nature." left_nemtok (thread)) 
                                    if full_correction_neededf then apply_qkill sites
                                    (thread, reliedf)::cc
                            let readers = List.fold lkill [] readers
                            // If another thread relied on this being non-volatile, then flag to go back and recompile earlier thread(s).
                            if vd>=5 then vprintln 5 (sprintf "constvol_tracker: interthread_shared noted for nemtok=%A  sites=%A" left_nemtok (sfold txToStr sites))
                            if full_correction_neededf then apply_qkill sites
                            cCC.constant_or_volatile.add left_nemtok (readers, VOC_multi_thread_assign sites)

            | Some(readers, VOC_multi_value_assign sites)
            | Some(readers, VOC_multi_thread_assign sites) ->
                if memberp codesite sites then ()
                else
                    let sites = codesite::sites
                    let onethreadf = one_thread_only sites
                    if vd>=5 then vprintln 5 (sprintf "constvol_tracker: multi/shared assign sites noted for nemtok=%A  onethreadf=%A" left_nemtok onethreadf)
                    if onethreadf then
                        cCC.constant_or_volatile.add left_nemtok (readers, VOC_multi_value_assign sites)
                    else 
                        // When reliedf then go back and recompile earlier thread
                        let lkill cc (thread, reliedf) =
                                if samethread volf thread tid then cc
                                elif reliedf then 
                                   kill thread
                                   cc
                                else (thread, reliedf)::cc
                        let readers = List.fold lkill [] readers

                        if vd>=5 then vprintln 5 (sprintf "constvol_tracker: note interthread-shared for %A  onethreadf=%A" left_nemtok onethreadf)
                        cCC.constant_or_volatile.add left_nemtok (readers, VOC_multi_thread_assign sites)
                

// The purpose of storage classes is to track what possible values of array subscript might be used.
// Populate ataken database by making a static analysis of the kcode.  - Repack looks at hexp_t code not kcode, so please explain here quite why we need to do it on the kcode: the answers are, the keval env operates on a storage class basis and the hexp_t code does not contain sufficient annotations and has additional complexities or optimisations applied that obscure the main tracking behaviour.
// We need to do it on virtual registers before colouring, since the surjection to physical registers introduces false dependencies.
// Roughly following (perhaps?) "Strictly declarative specification of sophisticated points to analyses",  Bravenboer.

// Aid's should be preserved through a small subset of operations: apart from assignment, there is multiplexing, pointer arithmetic and field pack/unpacking.  kcode should represent field packing using the CE_anon_struct form.  The unpack operator is sometimes via the CE_dot of a valuetype mechanism.

// We ignore control flow in this analysis.  We can make sc nemtoks pre or post static elaboration - post provides more diversity and this will reduce to the pre case if there are data paths between the elaborated call sites.

// populate_ataken is a currently a little of a misnomer - really it is capturing dataflow information on pointers for escape and disambiguation analysis.
// In general, we need to know which singletons have their address taken in case we encounted a stind, stobj or stelem instruction where we have failed to track the dataflow id (aka nemtok) because, then, at least we can preserve knowledge over all singletons that are never address taken.
let populate_ataken ww m_shared_var_fails site (tid:tid3_t) kcode =
    let cCC = valOf !g_CCobject
    let nemtok_details = cCC.kcode_globals.nemtok_details
    let vd = !g_ataken_vd
    let msg = sprintf "populate_ataken   site=%s   tid=%s   %i kcode statements" site (tid.id) (length kcode)
    let ww = WF 3 msg ww "Start"
    let mf() = msg
    let lpl = false

    //dev_println msg 
    

    let lhs_constvol_tracker_aid vtag = function
        | A_loaf nemtok ->
            generic_constvol_tracker ww m_shared_var_fails site tid vtag false false nemtok
        | _ -> ()

    let rhs_constvol_tracker_aid vtag = function
        | Some(A_loaf nemtok) ->
            let debug_info = [] // for now.
            ignore(rhs_constvol_noter2 m_shared_var_fails tid nemtok debug_info)
            ()
        | _ -> ()

    let not_arx_pred = function // Address arithmetic checker.
        | A_loaf nemtok ->
            match nemtok_details.lookup nemtok with
                | Some la -> not la.la_arithx
                | None ->
                    vprintln 3 (sprintf "+++ not_arx_pred: missing nemtok_details for %s" nemtok)
                    false
        | _ -> true


    let m_escaping_handles = ref []
    let comprehend_support ce =
        let support = ce_support "mode=topmode" !m_escaping_handles ce
        let axe arg = sprintf "%A" arg
        m_escaping_handles := support
        //dev_println (sprintf "comprehend ce_support is %s" (sfold axe support))
        ()

        //let (static_lhs_set__, static_rhs_set, kcode_static_wlk) = rez_kcode_static_support_sets_walker ()
        
    let cmdf kcc ((n, cmd), son_exps) = 
        let vtag = "K000" + i2s (n+0) + ":" // listing has/had one greater line numbers. // Use K now for kcode, since V00 can look like a virtual register number.
        let mon xvd ss = vprintln xvd ("populate_ataken dataflow analysis " + tid.id + ":" + vtag + " " + kToStr lpl cmd + "\n" + ss + "\n\n")


        let _ =
            let constvol_noter vale =
                //dev_println(sprintf "constvol noter " + ceToStr vale)
                rhs_constvol_tracker_aid vtag (ce_aid0 mf vale) // Note the aids of the son expressions are not the same as the aid of the rhs itself in general.
            app constvol_noter son_exps

        match cmd with
            // We should not need to make pairs between items appearing on one side only since if they are paired there must be an assignment somewhere or else it is logged by a pointout.

            | K_easc(kxr, CE_start(true, callstring_, signat_etc_, obj, args, cgr0), _) -> (app comprehend_support args; kcc)
            | K_easc(kxr, CE_apply(bif, cf, args), _)                                   -> (app comprehend_support args; kcc)

            | K_as_sassign(kxr, lhs, rhs, abuse) ->
                let mm = "populate_ataken K_as_sassign"
                let lhs_items = ce_aid_lst mf lhs
                comprehend_support rhs
                app (lhs_constvol_tracker_aid vtag) lhs_items
                let is_static_cachef =
                    if is_static_cache lhs then
                        vprintln 3  (sprintf "Noting static cache attribute for %s  class=%s" (ceToStr lhs) (sfold aidToStr lhs_items))
                        true
                    else false
                let is_explicit_nullf = ce_isnull rhs && not (ce_is_hllreset rhs)
                //if is_explicit_nullf then vprintln 0 (vtag + sprintf ": Hit null assign " + msg + " on " + ceToStr rhs)

                if is_explicit_nullf || is_static_cachef then ignore (map (aid_logger is_explicit_nullf is_static_cachef) lhs_items)
                let addl kcc lv_ause = 
                    let rv_ause_lst = list_subtract(List.filter not_arx_pred (ce_aid_lst (fun()->mf()+":rhs") rhs), [lv_ause]) // It's pointless listing a reflexive xfer so subtract it out and also delete arithmetic results.
                    if vd>=4 then if not_nullp rv_ause_lst then mon 4 (sprintf " pat sassign: %s %s := (is_explicit_null=%A) %s" vtag (aidToStr lv_ause) (is_explicit_nullf) (aidlToStr rv_ause_lst))
                    //else mon 4 (vtag + ": pat sassign GOT NOTHING")
                    //vprintln 0 (sprintf "lv_ause is %A" lv_ause)
                    singly_add (lv_ause, map (fun a->(vtag, a)) rv_ause_lst) kcc
                List.fold addl kcc lhs_items 

            | K_as_vassign(kxr, lhs, rhs, idx_, lhs_aid_o, _) ->
                let mm = "populate_ataken K_as_vassign"
                let is_explicit_nullf = ce_isnull rhs && not (ce_is_hllreset rhs)
                comprehend_support rhs
                let rhs_aids = List.filter not_arx_pred (ce_aid_lst (fun()->mf()+":rhs") rhs)
                // The left aids are stored in the assign, but we dig in the rhs expression for its aids.
                if is_explicit_nullf then ignore(oapp (aid_logger is_explicit_nullf false) lhs_aid_o)

                // OLD NOTE: If either is null we get no product and lose the aid information present in one side or the other.
                // This is not relevant to this being a vassign.  The null aid relates to the data moved and what is important is that the subscript contains aid tags.
                // These tags are needed in repack and may be needed for constant propagation and keval.
                //if nonep lhs_aid_o || nullp rhs_aids then
                    //vprintln 0 (sprintf "lhs_aid_o=%A" lhs_aid_o)
                    //vprintln 0 (sprintf "rhs_aids (%i) =%A" (length rhs_aids) rhs_aids)
                    //mon 0 ("troublesome kcode line")
                    //hpr_yikes(sprintf "vassign: if either aid is null we get no product and lose the information.")

#if OLD
                if nullp lhs_aids || nullp rhs_aids then

                    if vd>=4 then mon vd (sprintf " pat vassign: %s no result" vtag)
                    match lst_union lhs_aids rhs_aids with
                        | [] -> kcc
                        | [item] -> kcc
                        | h::tt ->
                            let bad0 cc aid = singly_add (h, [(vtag, aid)]) cc // yuck
                            List.fold bad0 kcc tt
                else
                    let badd kcc laid =
                    // The deca is now done on create of the kcode so none needed here.
                        let bad1 kcc raid = 
                            if laid = raid then kcc
                            else
                                if vd>=4 then mon 4 (sprintf " pat vassign: %s %s := (is_explicit_null=%A) %s" vtag (aidToStr laid) (is_explicit_null) (aidToStr raid))
                                singly_add (laid, [(vtag, raid)]) kcc
                        List.fold bad1 kcc rhs_aids
                    List.fold badd kcc lhs_aids
#endif
                if nonep lhs_aid_o then kcc 
                else
                    lhs_constvol_tracker_aid vtag (valOf lhs_aid_o)
                    map (fun rhs_aid -> (valOf lhs_aid_o, [(vtag, rhs_aid)])) rhs_aids @ kcc

            | K_new(kxr, lhs, nemtok, newarg) ->
                generic_constvol_tracker ww m_shared_var_fails site tid vtag false false nemtok
                let lhs_items = ce_aid_lst mf lhs
                let rv_ause  = A_loaf nemtok
                let addl kcc luse = (luse, [(vtag, rv_ause)]) :: kcc 
                List.fold addl kcc lhs_items

            | K_pointout(kxr, msg, lhs, rhs) ->
                (lhs, [(vtag, rhs)]) :: kcc


            | other -> kcc
    let raw_ans =            
        let expf (ce, sons) = // Any sons can be ignored here since they will have just had expf applied to them in turn already.
            //if vd>=4 then dev_println(sprintf "kcode walk look for hit hat: " + ceToStr ce)
            match ce with
                //| CE_apply(CEF_mdt mdt, cf, args) when not_nonep mdt.fu_arg -> muddy "dont need to trap this now since done in insert_topargs"
                | other -> ce

        let wlk =
            {
                cmdf= cmdf
                expf= expf
            }
        List.fold (kce_walk wlk) [] kcode

    let ww = WF 3 msg ww "Pass2 start."
    let pass2 =            
        let vd = if cCC.settings.kcode_dump > 0 then 4 else -1
        let escaping_vrnis =
            let shox cc arg =
                match arg with // Long-winded!
                    | None      -> cc
                    | Some vrni -> vrni::cc
            List.fold shox [] !m_escaping_handles
        let pass2_escape_scan (vtag, cmd) =
            match cmd with
                | K_new(kxr, lhs, nemtok, newarg) ->
                    let vrni = ce_vrn lhs
                    if vd>=4 then dev_println (sprintf "escape_scan: examined K_new %s  vrni=%A" (ceToStr lhs) vrni)
                    let escaped =
                        not_nonep vrni && memberp (valOf vrni) escaping_vrnis
                    if vd>=4 then dev_println (sprintf "K_new arg: comprehend escaped=%A " escaped + ceToStr lhs)
                    if not escaped then
                        let sci = cCC.kcode_globals.ksc.classOf_i msg (A_loaf nemtok)
                        cCC.kcode_globals.ksc.attribute_set sci g_no_escape_mark "true"
                        if vd>=4 then dev_println (sprintf "K_new escape note: nemtok=%s sci=c%i escaped=%A for %s" nemtok sci escaped (ceToStr lhs))
                        ()
                | _ -> ()
          
        app pass2_escape_scan kcode
    let ww = WF 3 msg ww "Pass2 finished."

    let _ =
        let rawf (lv, rv_ause_lsts) = "ataken arc: " + aidToStr lv + " := " + sfold (fun (vtag, rv_ause_lst)->vtag + ":" + aidToStr rv_ause_lst) rv_ause_lsts
        if vd>=4 then reportx 4 "Ataken raw data for this thread" rawf raw_ans

    let lhs_list = List.fold (fun c a -> singly_add (fst a) c) [] raw_ans

    let collated =
        let collate v = // collate on lhs.
            let col c (l, r) = if l=v then lst_union c r else c
            (v, List.fold col [] raw_ans)
        map collate lhs_list

    let _ =
        let collf (lv, rv_ause_lsts) = sprintf "  collated ataken arc: " + aidToStr lv + " := " + sfold (fun (vtag, rv_ause_lst)-> sprintf "%s: %s" vtag (aidToStr rv_ause_lst)) rv_ause_lsts
        if vd>=4 then reportx 4 ("Ataken collated data for thread " + tid.id) collf collated

    // now uncollate on lhs aid - and commute according to complexity - perhaps rather roundabout! And better to collate on lower-complexity items.
    let uncollated =
        let uncol cc (lhs_aid, rhs_lst) =
            let lcost = aid_complexity lhs_aid
            let genpair cc (vtag, rhs_aid) =
                let rcost = aid_complexity rhs_aid
                let nv = if lcost > rcost then (vtag, rhs_aid, lhs_aid) else (vtag, lhs_aid, rhs_aid)
                let fx (vtag, l, r) = sprintf "%s %s   l=%s r=%s"  (tid.id) vtag (aidToStr l) (aidToStr r)
                if vd>=4 then vprintln 4 ("uncollated dataflow pair formed " + fx nv)
                nv::cc
            List.fold genpair cc rhs_lst
        List.fold uncol [] collated

        
// Do store0 first to conglomorate as many basic nemtoks together into a common class, before building higher parts on them.
    let store0 = function // targets
        | (vtag, ((A_loaf left_nemtok) as left_aid), rhs) -> // vtag was just the kcode line number for debugging but now used for assign-once checking in constvol.

        //constvol_tracker vtag left_nemtok
            match rhs with
                | (A_loaf right_nemtok) as right_aid ->
                    if vd>=5 then vprintln 5 (sprintf "  store0 leaf %s %s lhs_ntoks=%s rhs_ntoks=%s" (tid.id) vtag (left_nemtok) (right_nemtok))
                    // We may freely ignore regionf when equating nemtoks owing to the convention of registers and region aids (aid of register is its contents whereas for region it is its base address).
                    
                    let lnc1 = nemtok_add_pair "store00" (left_aid, right_aid)
                    //let _ = if lsf then sett lnc1
                    if vd>=4 then vprintln 4 (sprintf "  store00 pair tid=%s lineno=%s cart pair l=%s   and    r=%s entered in class lnc=%s" (tid.id) vtag (aidToStr left_aid) (aidToStr right_aid) lnc1)
                    ()
                    
                | right_aid_ -> // clause almost never run now we groom - only on A_loaf(None)?
                    let lnc = aid_add_solo "store0-popleft" left_aid
                    //let _ = if lsf then app sett lnc
                    if vd>=4 then vprintln 4 (sprintf "  store0 solo (tid.id)=%s lineno=%s   left_aid=%s   right_aid=%s    entered in class lnc=%s" (tid.id) vtag (aidToStr left_aid)  (aidToStr right_aid_) lnc)
                    ()

        | (vtag, _, _) ->
            hpr_yikes (sprintf "store0: silent other clause lineno=%s" vtag)
            ()
            
    let store1 (vtag, lhs_aid, rhs_aid) = 
        if vd>=4 then vprintln 4 (sprintf  "    store1 yap %s: lhs=%s rhs=%s" vtag (aidToStr lhs_aid) (aidToStr rhs_aid))
        let (l, r) = (lhs_aid, rhs_aid)
        if l=r then
               let sc = aid_add_solo "store10-solo" l
               if vd>=4 then vprintln 4 (sprintf "store1 %s : reflexive lsh=rhs=%s in class %s" vtag (aidToStr r) (sc))
               ()
        else
            let scs = nemtok_add_pair "store1" (l, r)
            if vd>=5 then vprintln 5 (sprintf "store1 make ties %s:  lhs=%s  rhs=%s in class %s" vtag (aidToStr l) (aidToStr r) (scs))
            ()

    let (uncollated1_r, uncollated0_r) = // Groom delivers first those for which the predicate holds.
        let is_store_one (line_no_, a, b) =
            let s1f = function
                | A_loaf(_) -> false
                | _         -> true
            s1f a || s1f b
        groom2 is_store_one (rev uncollated) // Reverse for a less confusing listing.
    app store0 uncollated0_r
    app store1 uncollated1_r    



    let ww = WF 3 msg ww (sprintf "Finished.  %i items collated" (length collated))
    ()



    
let gec_RN_monoty ww = function
    | CT_class cst  -> sf ("we expect class references and records to be stored, not classes. Hit bad route with " + hptos cst.name)
    | other -> RN_monoty(other, [])

let gec_RN_monoty_ats ww dt ats = RN_monoty(dt, ats)

// deqd operator: Perform equality comparison of compile-time constants.
// We expect constants to be reduced to CE_x form already.
let rec ce_deqd l r =
    //let _ = vprintln 0 (sprintf "ce_deqd L=%s\n        cf R=%s" (ceToStr l) (ceToStr r))    
    let ans = 
        match l with
        | CE_x(_, l) ->
            match r with
                | CE_x(_, r) -> xbmonkey(ix_deqd l r) = Some true
                | _ -> false

        | CE_var(vrn, dt, idl, fdto, ats) -> false

        | CE_region(luid, _, _, _, _) ->
            let rec rcomp = function
                | CE_conv(r_dt_, _, r_ce) -> rcomp r_ce // Could generalise over all clauses 
                | CE_region(ruid, dt', ats', _, _) -> luid.baser = ruid.baser 
                | _ -> false
            rcomp r
        | CE_conv(dt, _, ce) -> ce_deqd ce r // Should apply the conv of course!

        | other  ->
            let _ = vprintln 0 (sprintf "+++ no ce_deqd main clause for " + ceToStr other)
            false // Conservative result.

    //let _ = vprintln 0 (sprintf "  ce_deqd L=%s\n       cf R=%s ans=%A" (ceToStr l) (ceToStr r) ans)
    ans



                
// aids are cached here
let gec_CE_subsc (dt, ce, offset, aid_o) =
    if nonep aid_o then sf("CE_susbc: aid is missing for " + ceToStr ce)
    CE_subsc(dt, ce, offset, aid_o) 


let get_ce_string = function
    | CE_x(_, W_string(str, _, _)) -> str
    | _ -> ""

let get_ce_number msg = function
    | CE_x(_, nn) -> xdeval msg nn
    | other ->
        let _ = cleanexit (sprintf "%s: expected a number, not '%s'" msg (ceToStr other))
        -1


type realdo_t = 
    | Rdo_tychk of (string list * dropping_data_t) list ref
    | Rdo_gtrace of (kcode_t list ref * kcode_t list ref)  // Emitted code in main and postamble, reversed, lists.


let realDo = function
    | Rdo_tychk _  -> "typecheck"    
    | Rdo_gtrace _ -> "gtrace"    

let tychk_pass_pred = function
    | Rdo_gtrace _ -> false
    | _            -> true


//
// Setwaypoint KppMark - this has no innate multi-threaded capabilities and so should generally be
// set by an application's master/controlling thread, assuming it has one.
//
// Revised Jan 2018 so that first arg is a manualWaypointNumber, but will revise to re-automate
let setwaypoint ww realdo lp5 args = 
    let (no, str) =
        match args with
            | []              -> (0, "")
            | no :: name :: _ ->
                if tychk_pass_pred realdo then (-10, "undef")
                else
                    (get_ce_number "manual waypoint no" no, get_ce_string name)
            | other           -> cleanexit(sprintf "As of Jan 2018, KppMark has two overloads only: (manualNo:int, wayPointName:string) and (manualNo:int, wayPointName:string, subsequentPhaseName:string). You have to manually create the waypoint no for now. Automatic service can again be provided in the future.")
    let manual_wpn = no
    let _ = vprintln 3 (sprintf "set waypoint: oldwp=%s newwp=%s manual_wpn=%i" (cil_lpToStr lp5) str manual_wpn)
    if str <> "" then lp5.waypoint :=  Some str 


//
let is_squirrel_base potential_base query =
    //let _ = vprintln 3 (sprintf "is squirrel base? potential_base=%s  query=%s" (hptos potential_base) (hptos query))
    let rec sc = function
        | ([], _) -> true
        | (b::bs, q::qs) -> b=q && sc (bs, qs)
        | _ -> false
    sc (rev potential_base, rev query)

let rec knotflatten arg cc =
    match arg with
        | No_list lst -> List.foldBack knotflatten lst cc
        | No_knot(CT_knot(idl, w, v), kno) when not(nonep !kno) -> knotflatten (valOf !kno) cc
        | other -> other :: cc

// The disambiguation name identifies various overloads: it is the squirrel string.
let get_disamname = function
    | No_method(srcfile, flags, ck, (id, unique_id, disamname), mdt, flags1, instructions, atts) -> disamname
    | other ->
        match knotflatten other [] with
            | [No_method(srcfile, flags, ck, (id, unique_id, disamname), mdt, flags1, instructions, atts)] -> disamname
            | others -> sf (sprintf "get_disamname others %A" others)


// Import an AST expression from CIL to CE form.
let rec cilToCe bindings lst =
    match lst with
    | Cil_float(n, fps)   -> gec_yb(X_bnet(xi_fps 64 fps), g_canned_double)
    | Cil_number32 n      -> gec_yb(xmy_num n, g_canned_i32)
    | Cil_int_i(w, n)     ->
        if w = 32 then gec_yb(xi_bnum_n w n, g_canned_i32)
        elif w = 64 then gec_yb(xi_bnum_n w n, g_canned_i64)        
        else sf(sprintf "unsupported Cil_int_i width %i" w)
    | Cil_blob lst      ->
        let chr c = System.Convert.ToChar(int c)
        let k = function
            | Cil_number32 n -> chr n
            | other -> sf ("blob contents not a number:" )
        let vs = implode(map k lst)
        let ans = gec_ce_string vs //yb(xi_string vs, f_canned_str(length lst))
        ans

#if OLD2016        
    | Cil_tnumber _ ->
        let rec signer = function
            | (Cil_tnumber_end sg) -> sg
            | (Cil_tnumber(a, v)) -> signer v
        let rec k = function
             | (Cil_tnumber_end _) -> 0I
             | (Cil_tnumber(n, tt)) -> BigInteger n + 4096I * (k tt)
        let a = k lst
        let a = if signer lst then 0I - a else a
        gec_yb(xi_bnum_n wid a, g_canned_i32)
#endif
    | Cilexp_caster(t, v) -> cilToCe bindings v

//  | Cil_string ss ->
    | Cil_id id ->
        let ov = sf(id + ": code path nolonger used. legacy binding code called ?")
        (* id_lookup ww (bindings, fun()->("cil: no binding for " + hptos [id], 1))
        let (ct, _, r) = valOf ov
        (!r) *)
        ov
   
    | other  -> sf("cilToCe other " + iexpToStr other)


// Perform dataflow analysis for register colouring.
// It's always a little counterintuitive that an assign 'kills' its lhs, but this reflects that any precomputed subexpressions dependent on that variable are invalidated and also means that a read 'gens' its argument.
let collect_gen_kill_null_sets ln code =

    let cmdf (kills, gens) ((_, cmd), gens1) =
        match cmd with
            | K_as_sassign(kxr, CE_var(vrn, dt, idl, fdto, ats), rhs, abuse) ->
                //let _ = vprintln 0 (sprintf "vregdfa: Kill V%i at %i" vrn ln)
                (ascending_int_sort_add vrn kills, union_ascending_int_sort gens gens1)
                
            | _ -> (kills, union_ascending_int_sort gens gens1)

    let expf (ce, sons) =
        let ss = Union_ascending_int_sort sons
        match ce with
            | CE_var(vrn, dt, idl, fdto, ats) ->
                //let _ = vprintln 0 (sprintf "vregdfa: gen V%i at %i" vrn ln)
                ascending_int_sort_add vrn ss
            | _ -> ss
            
    let wlk =
        {
            cmdf= cmdf
            expf= expf
        }
    let (kills, gens) = kce_walk wlk ([], []) code
    (kills, Union_ascending_int_sort gens)

#if SPARE
let ce_aid1_ mf lhs =
    let s_aid = ce_aid0 mf lhs  // we want the 'subscript' aid in some sense (pre eval of lhs)
    vprintln 3 ( "transpose s_aid=" + aidlToStr s_aid )
    if length s_aid <> 1 then sf(mf() + ": not one nemtok for ptr base 3/4:" + aidlToStr s_aid)
    hd s_aid
#endif
    
// Get core (CTL_net) numeric type from within a more complex form
let rec simplify_numeric_type arg =
    match arg with
    | CTL_net(volf, width, signed, flags) -> CTL_net(volf, width, signed, flags)
    | CT_star(1, CT_arr(ct, _)) -> simplify_numeric_type ct // TODO <------- if we never make these we would not have to remove them!
    | CT_knot(idl, whom, vv) when not(nonep !vv) -> simplify_numeric_type (valOf !vv)

    //| CT_valuetype ct     | CT_brand(_, ct) -> simplify_numeric_type ct    
    | other ->
        dev_println (sprintf "other type in simplify_numeric_type " + cil_typeToStr arg)
        other 

let rec grossly_simplify_type = function
    //| CT_valuetype ct     | CT_brand(_, ct) 
    | CT_star(_, ct) -> grossly_simplify_type ct
    | CT_knot(idl, whom, vv) when not(nonep !vv) -> grossly_simplify_type (valOf !vv)
    | other -> other

// Structs are valuetypes. They are collections of primitive types and possibly further structs.
// Flatten all valuetypes out into ordered list of primitive types.
// Return no contents for an object handle, just the handle. For valuetypes we need to recurse to flatten. TODO
let rec get_struct_tag_listf msg earg_ arg cc = 

    let rec get_struct_tag_list2__ arg cc =
        match arg with
            | ((ptag, utag), RN_monoty(dt_, _)) -> ((ptag, utag), dt_) :: cc
            | (_,            RN_call _)         -> cc

    and gs1 vt arg cc =
        match arg with
            //| CT_valuetype xx -> gs1 true xx cc
            | CT_star(_, dt) -> gs1 vt dt cc // TODO - ignoring stars here is not correct - please fix once other bugs are fixed
            | CTL_record(sqidl, cst, lst, len, _, binds, _) when vt -> Some(cst.class_tag_list)
                //let _ = vprintln 0 (sprintf "get field tags from instance of " + hptos sqidl)
                //Some(List.foldBack get_struct_tag_list2 cst.class_item_types cc)
            | other ->
                None
                //dev_println (msg + ": cannot use the following type in a structure assign: type=" + cil_typeToStr other + " expression=" + ceToStr earg_)

    gs1 false arg cc


// Return basic (ptag, dt, None) tuples as the templated for an empty CE_anon_struct body.
let struct_get_raw_items msg arg =
    let rec gs1a arg =
        match arg with
            | CT_class dct ->
                let bop (ptag, utag) = (ptag, CTL_void, None)
                //let _ = vprintln 3 (sprintf "Return empty struct for so-far unresolved class. (ok for typecheck) " + cil_typeToStr record_dt)
                //let _ = vprintln 3 (sprintf "struct_get_raw_items fields for %s are " (cil_typeToStr arg) + sfold fst dct.class_tag_list)
                map bop dct.class_tag_list

            | CT_cr crst -> gs1a crst.crtno
            //| CT_valuetype xx -> gs1a xx
            | CT_star(_, dt) -> gs1a dt // TODO - ignoring stars here is not correct - please fix once other bugs are fixed
            | CTL_record(sqidl, cst, lst, len, _, binds, _) ->
                let boz concrete = (concrete.ptag, concrete.dt, None)
                map boz lst
            | other  -> sf (sprintf "struct_get_raw_items other form " + cil_typeToStr other)

    gs1a arg

    

let m_next_rez_idx:rez_idx_t ref = ref 1000

let gec_cee_scalar arg =
    let nv = !m_next_rez_idx
    let _ = m_next_rez_idx := nv+1
    CEE_scalar(nv, arg)

let gec_cee_vector arg =
    let nv = !m_next_rez_idx
    let _ = m_next_rez_idx := nv+1
    CEE_vector(nv, arg)


let nats_resolve l r =
    {
        nat_enum=     if not_nonep l.nat_enum then l.nat_enum else r.nat_enum
        nat_char=     l.nat_char || r.nat_char
        nat_string=   l.nat_string || r.nat_string
        nat_IntPtr=   if not_nonep l.nat_IntPtr then l.nat_IntPtr else r.nat_IntPtr
        nat_native=   l.nat_native || r.nat_native // Arbitrary: there is no clear answer on this field.
        nat_hllreset= l.nat_hllreset && r.nat_hllreset
        //xnet_io=    if l.xnet_io<>LOCAL then l.xnet_io else r.xnet_io
    }


let rec destar = function
    | CT_star(_, ct) -> destar ct // removes all stars
    | ct -> ct


let dex_fap = function
    | Cil_tp_type ty -> ty
    | other -> sf (sprintf "dex_fap: generic other %A" other)

let rex_fap ty =  Cil_tp_type ty 


let fatal_dx msg1 msg2 = function
    | RN_monoty(ty, _) -> ty
    | other -> sf (msg1 + sprintf ": fatal_dx: missing return type dropping for %s %A " (msg2) other)



let get_class_parent_list msg arg =
    let rec get_parents_of_type = function
                | CT_star(_, dt) -> get_parents_of_type dt
                | CTL_record(sqidl, cst, lst, len, _, binds, _) ->
                    let rec get_parents = function
                        | None -> []
                        | Some ty -> get_parents_of_type ty
                    cst.name :: (get_parents cst.parent)
                | CT_cr cst -> [ cst.name ] // TODO should search in its parens but this will be sufficient to pick up immediate MulticastDelegate parents needed for native constructor invoke.
                | other -> sf(msg + sprintf ": other class_parent_search L2543 " + cil_typeToStr other)
    get_parents_of_type arg



// See which allocation region or base address an item hase been allocated in.
let rec get_base64 = function
    | CE_var _                 -> 0L // Not indexable/purely-symbolic 
    | CE_star(_, arg, _)       -> get_base64 arg
    | CE_conv(_, _, arg)       -> get_base64 arg    
    | CE_subsc(_, arr, _, _)   -> get_base64 arr
    | CE_region(hidx, dt, ats, _, cil_) -> hidx.baser
    | other -> sf (sprintf "base64 other " + ceToStr other)


let struct_filled_field_audit items =
    let counter (ff, tf) = function
        | (_, _, None) -> (ff, tf+1)
        | (_, _, Some _) -> (ff+1, tf+1)
    let (filled_fields, total_fields) = List.fold counter (0, 0) items
    (filled_fields, total_fields)


//
// Combine token with a library path to get a full path name 
//
let support_lib_dir cc tok =
    let hpr_path = opath.get_hpr_path true // We only look in this one place for these two libraries.  
    if tok = "" then cc
    elif (tok.[0] = '/' || tok.[0] = '\\') then tok :: cc
    else path_combine(hpr_path, path_combine("..", path_combine("support", tok))) :: cc 




let cil_writeout ww ast filename = 
    let msg = "KiwiC CIL/PE disassembly dump " + filename + "\n"
    let ww = WF 2 "cil_writeout" ww msg
    let fd = yout_open_out filename
    vprintln 3 ("Writing " + msg)
    youtln fd ("//Output from KiwiC: " + filename)
    //print(msg + timestamp(true) + "\n" + !g_argstring + "\n")
//get branch dests
    app (cil_out fd) ast
    yout fd ("//End of " + msg)
    yout_close fd
    ()
    

let read_dotnetfile_and_dump ww settings (fname, root, suf) cc =
    let readone (fn, root, suf) =
        // We delete any linker-style command prefix on the CIL input file names for user convenience.
        let fn =
            if strlen fn > 4 then match fn.[0..2] with
                                           | "/r:"
                                           | "-r:" -> fn.[3..]
                                           | _ -> fn
            else fn
        let is_exe = suf = "exe"
        if suf <> "exe" && suf <> "dll" then vprintln 0 (fn + " had suffix '" + suf + "', expecting exe or dll")
        let msg = "reading cil file " + fn
        vprintln !g_filesearch_vd msg
        kiwi_readin_exe_or_dll (WN msg ww) is_exe fn 

    if fname = "" then
        vprintln 1 (sprintf "KiwiC: ignore file with name '%s'" fname)
        cc
    else
        let ast = readone (fname, root, suf)
        //vprintln 0 (sprintf "Joining test : we get %s" (path_combine("foo", "bar")))
        if settings.cil_dump_separately then cil_writeout ww (f3o3 ast) (path_combine(!g_log_dir, (*filename_sanitize [] *)root + ".ast.cil"))
        ast::cc



// eof
