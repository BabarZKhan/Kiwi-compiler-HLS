// CBG Orangepath HPR L/S.  HPR Logic Synthesis and Formal Codesign System.
// abstract_hdr.fs
//
// Contains HPR virtual machine framework definitions.
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


module abstract_hdr

open moscow

open hprls_hdr
open microcode_hdr



let g_null_class = 2
let g_anon_class = 4
let g_as_next = ref 10 // start higher than reserved values




let p2ss b =
    let i2s (x:int) = System.Convert.ToString x
    (if b=g_anon_class then "c-blank" else sprintf "d%i" b)

// Asterisk denotation - for +ve stars (number of dereferences made).
// Modern approach uses uparrow for negative stars with the convention that a string in quotes denotes 0 stars, -1 is unquoted.
let p2ss1 stars sci =
    if true then failwith "p2ss1: old style - DELETE ME TODO"
    else
    match stars with
    | 0 -> sprintf "\"c%i\"" sci // Best not to call xi_uqstring on this answer because that would put quotes inside an unquoted string - see code elsewhere (cilnorm.fs).
    | -1 -> p2ss sci
    | s when s > 0 ->
        let rec s n = if n=0 then "" elif n>0 then "s" + s(n-1) else  "*" + s(n+1)
        (s stars) + p2ss sci
    | s when s<0 -> sprintf "^%i" s + p2ss sci

let p2ss0 (stars, sci) = p2ss1 stars sci

let p2s ((a, stars, c):storeclass_t) =
    if false then a + ":" + p2ss1 stars c
    else p2ss1 stars c

type visit_ratio_t =
    {
        callrate:   double  // Visit ratio is estimated from control-graph balance equations or from profile-directed feedback.
    }

type resume2_t = //
    {
        hres:   hexp_t
        ires:   int // give with lc_atoi32 applied to hres
    }

type visit_budget_t =
    {

        visit_time: int  // Maximum or nominal time allowed to execute this state in clock cycles (postcursors may resonate afterwards, but we must move on to the next state in this time).
        hardf: bool // bool which holds for a 'hard state'. This is one where the visit time is precise, not nominal.
    }

let g_default_visit_budget =
    {

        visit_time=   0
        hardf=        true
    }

//
// A finite-state machine which is also a custom processor when an instruction ROM is present.
//
// Information about a finite state machine: paired with a list of vliw_arc_t records.
// States are tagged with a time budget
// be removed or stuttered owing to specific synchronous timing (e.g. at a net-level interface).    
type fsm_info_t =
  {
 // odd there is no id field or name?  please add one?
    pc            : hexp_t                      // PC variable that ranges over the resumes.
    resumes       : (hexp_t * visit_budget_t) list        // PC values - constants.
    exit_states   : (hexp_t * visit_budget_t) list        //
    start_state   : hexp_t * visit_budget_t               //
    controllerf   : bool                        // Holds if it is desirable to preserve this as an FSM (e.g. denote with a case statement in some output languages). Will be false for trivial FSMs with one state.
    inst_rom      : (int * string) array option // Opcode and disassembly.
    inst_set      : (int * xrtl_t list) array option // Opcode to behaviour mapping.
  }


// A finite state machine transition: If we are in the state where 'pc=resume' and 'gtop' holds, then transfer to state 'dest' and execute 'cmds'. 
and vliw_arc_t = 
  {
     pc:        hexp_t         // program counter name (resume and dest are enumeration constants ranged over by this pc)
     old_resume: hexp_t option // Earlier resume state for visualisation outputs
     resume:    hexp_t        // commence state
     iresume:   int
     dest:      hexp_t        // where to go after this state
     idest:     int
     gtop:      hbexp_t       // guard for whole transition
     cmds:      xrtl_t list   // commands to run in this state (beyond any in the instruction set)
     hard:      visit_budget_t// If hard, then no clock cycle stuttering allowed.
     eno:       int           // A global reference number for this arc.
     visit_ratio:  visit_ratio_t option
  }


// XRTL is a form of executable rule rather like register transfer level code.
// RTL transitions
and rtl_op_t =
    | Rarc of hbexp_t * hexp_t * hexp_t  // A sequential assignment: Fields are (pc, state) option, guard, lhs, rhs. Can also make combinational by inserting a negative X_x.
    | Rpli of hbexp_t * ((string * native_fun_signature_t) * vsfg_order_t option) * hexp_t list   // A primitive function call - with sort order constraints
    | Rnop of string

and xrtl_t =
    | XIRTL_pli of stutter_t * hbexp_t * ((string * native_fun_signature_t) * vsfg_order_t option) * hexp_t list   // A primitive function call (deprecated form)
    | XRTL of stutter_t * hbexp_t * rtl_op_t list // A guarded atomic block: 
    | XIRTL_buf of hbexp_t * hexp_t * hexp_t   //  A continuous/combinational assignment (like Verilog's bufif1 buffer): Fields are guard, lhs and rhs.
    | XRTL_nop of string

let g_default_fsm_info = 
                 {
                   pc=            X_undef
                   exit_states=   [] 
                   controllerf=   false
                   start_state=   (X_undef, g_default_visit_budget)
                   resumes=       []
                   inst_rom=      None
                   inst_set=      None
                 }


type pstyle_t =
     PS_lockstep   (* Clock events between lockstep parallel sections are aligned just like synchronous hardware. *)
   | PS_joining    (* In a joining parallel construct, all threads block waiting for the last to arrive *)
   | PS_eureka     (* In a eureka parallel construct, the first-exiting thread aborts the others spawned by the same construct. *) 
   | PS_unspec


type ast_ctrl_t = // 
    {

      id:          string
    }

type rtl_ctrl_t = // 
    {

      id:          string
    }

type dic_ctrl_t = // Directly-indexed code control structure
    {

      ast_ctrl:    ast_ctrl_t
    }
    
(*
 * HPR_SP is a serial/parallel form of rule arrangement
 *)
type HPR_SP =
    |  SP_l of ast_ctrl_t * hbev_t            // Named abstract syntax tree of a behavioural program or single assigment statement. (for/while/break/continue/assign/if etc..
    |  SP_comment of string 
    |  SP_rtl of rtl_ctrl_t * xrtl_t list         // Named block of flat register transfer-level code - a set of parallel assignments to be executed on an event.
    |  SP_fsm of fsm_info_t * vliw_arc_t list // Finite state machine form  - like flat RTL but collated into disjoint sets that can be annotated with visit ratios.
    |  SP_constdata of hexp_t        // not yet used, constant data 
    |  SP_dic of hbev_t array * dic_ctrl_t  // Directly-indexed array: Imperative program stored in an array of icode indexed by a pc, with name in dic_ctrl. int is length (maxidx + 1).  *)
    |  SP_asm of icode_t array * int * (int64 * hexp_t) * (string * int64) list * (int * int) list // Assembler for a local family of microprocessors


    |  SP_par of pstyle_t  * HPR_SP list // Parallel composition of blocks - in lockstep of asynchronously

    |  SP_seq of HPR_SP list             // Sequential composition of blocks - makes no sense for some forms of block


type hblock_t = H2BLK of directorate_t * HPR_SP



type minfo_t = // Machine information - and meta information.
 { 
     name:       ip_xact_vlnv_t
     atts:       att_att_t list
     squirrelled:         string          // Opaque identifier representing any parameter override non-default values in the atts.
     // Having memdescs associated with a machine index is a little odd, since we have a flat address space shared by all machines until we refine our opath reference semantics a little better.
     memdescs:   (string list * memdesc_record_t) list  // String list is just a nameing tag for tracking
     //bevmodel: HPR_SP; // Behavioural model (used for simulation of ALUs and so on) - we can use the execs field for now.
     fatal_flaws:      string list // Empty if no flaws.  We generate with flaws so we can report the full flawed output before abend.
     hls_signature:    native_fun_signature_t option // Present when the machine is an implementation of a function.
 }


(*
 * The CTL (combinational tree logic) quantifiers have a string list that
 * contains error messages and so on.
 * Need to unify with the psl forms in hexp_t.
 *)
type xrule_t =  (* Need to delete some of these unused forms: only need one form .... *)
    | O_ctl_AG of xrule_t * string list
    | O_ctl_AX of xrule_t * string list
    | O_ctl_AF of xrule_t * string list
    | O_ctl_EG of xrule_t * string list
    | O_ctl_EX of xrule_t * string list
    | O_ctl_EF of xrule_t * string list
    | O_state of hexp_t  * edge_t option (* State predicate and observation guard (clkinfo) *)
    | O_BEV of hbev_t list
    | O_INITIAL of hexp_t
    | O_IDLE of hexp_t
    | O_DEFINE of hexp_t * hexp_t
    | O_FILLER


type sup_t = hexp_t * hexp_t list  (* A list of sup_t items should be sorted always, but the info lists on each item are not sorted. *)


(*
 * Notes made by anal of a block...
 *)
type anal_t = // Structure for partition signals into ports 
    | DRIVE of sup_t list * hexp_t option  (* Driven net and ce??? *)
    | SUPPORT of sup_t list  (* sorted *)

type hpr_portschema_t =
    {
        kind:            string

    }

// The netport_pattern_schema_t is one line of a formal spec of then net-level form of an inter-module (inter-VM) port.
type netport_pattern_schema_t = 
    {
      lname: string // Logical net name
      chan:  string // Channel, when so-called channelised like AXI or BVCI
      width: string // Net width
      sdir:  string // Direction on slave (aka component instance) (master will be the opposite) i/o/x
      pon:   string // Variant on which found - one letter per variant
      idleval: string // If empty then X_undef, else typically "0" for an active high control/guard net.
      ats:   (string * string) list
      signed: netst_t
    }

let g_null_netport_pattern_schema = 
    {
      lname=   "" // Logical net name
      chan=    "" // Channel, when so-called channelised like AXI or BVCI
      width=   "" // Net width
      sdir=    "" // Direction on slave (eg on the component definition for a RAM) master i/o/x
      pon=     "" // Variant on which found - one letter per variant
      idleval= ""
      ats=     []
      signed=  Unsigned
    }


type decl_binder_form_t =
    // A component has three levels of formal: metaprams, overrides and hexp_t contacts.        
    | DB_form_meta_pram       // Most abstract (higher than RTL parameters)
    | DB_form_pramor          // As in RTL parameters (aka overrides) to be instantiated post HPR synthesis     - also denoted with net att "rtl_parameter" - we dont need both mechanisms! Please delete the att approach.
    | DB_form_external        // RTL net-level contacts (or TLM ports) as the main inter-component connection mechanism.
    | DB_form_local           // For internal net grouping - .e.g. for connection to an internally-instantiated module or genuine working nets.

   //     | DB_form_


type decl_binder_metainfo_t = // Blank port description
    {
       purpose:           string // Standard purposes are primary subsidiary support directive debug ... 
       pi_name:           string // Port instance name - for when one component has multiple ports of the same kind.
       kind:              string // Protocol name in IP repositorary, same as portschema kind string when present
       version:           string

       //Temporary fix to design hierarchy
       //h_level:           int     // If an I/O which level below the top in the design heirarchy (1=main level, 0=externally_instantiated)

       not_to_be_trimmed: bool
       norender:          bool // Holds when local to a behavioural model
       form:              decl_binder_form_t //  enum { param, local, global, TLM etc.. }

       // A component has three levels of formal: metaprams, overrides and hexp_t contacts.
       metaprams:         (string * string) list     // Some of the same information in pramors rides is also held in ths higher meta form for IP-XACT render and can be held here.
       portschema:        netport_pattern_schema_t list option
    }

let g_null_db_metainfo =
    {
        purpose=       "primary"
        pi_name=       ""
        kind=          "nokind"
        portschema=    None
        version=       "1.0"
        form=          DB_form_local //is_param=      false
        not_to_be_trimmed= false
        norender=      false
        //h_level=       1
        metaprams=     [] // Much of the same information is also held in this form for IP-XACT render
    }



type filler_t = Filler

let g_fu_instance_name = "fu-instance-name"
let g_port_instance_name = "port-instance-name"      // Attribute used for IP-XACT port maps - name of a port instance (where a port is present more than once on a component)
// A g_logical_name is a token from a protocol definition that designates one net or vector within the net-level port that carries the protocol.
let g_logical_name = "logical-name"                  // Attribute used for IP-XACT port maps - name of a formal net within a port instance
let g_preferred_name = "preferred-name"              // Attribute used on a module name and so on.
// A g_username is the name of a net manually set by the user using a C# attribute or otherwise.
let g_username = "username"                          // Attribute used on a net - should not be different from g_preferred name - TODO




// Port group binder - gives hierarchy to otherwise flat nets in Verilog RTL. Useful for managing the nets within a logical port, as reported in the IP-XACT output files.

// A collection of I/O declarations assembled into a port
type decl_binder_t =
    | DB_group of decl_binder_metainfo_t * decl_binder_t list
    | DB_leaf of hexp_t option * hexp_t option // The two fields are normally the formal and actual parameter, although a variant use for the logical name (within a port) and the concrete name is also used?

let db_netwrap_null net = DB_leaf(None, Some net)

let db_netwrap_assoc fid net = DB_leaf(Some fid, Some net)


type vm2_iinfo_t = // vm2 name reference and child instance information
    {
        vlnv:                ip_xact_vlnv_t  // Kind name.
        iname:               string   // Instance name. There does not need to be an instance name for a definition so this field is blank when definitionf holds.
        definitionf:         bool     // When definitionf holds, this is a module definition, not an instance of it.
        nested_extensionf:   bool     // This used to implicitly hold but now is explicit where used.  Indicates that a VM definition is just a partitioned-off part of its parent and shares the same name space etc..

        
        externally_provided: bool     // Holds when we should not render the component since the real implementation comes from an external library. But diosim may exploit the local definition as a simulation model.
        external_instance:   bool     // Holds when the instance should be outside the parent component, connected using additional nets extending the signature of the parent.
        preserve_instance:   bool     // Not set when the vm should be inlined inside its parent. Old form of nested_extension denotation with contrary polarity that can now probably be deleted.
        generated_by:        string
        in_same_file:        bool     // Holds when a component implementation should be listed inside the same RTL/C++ file as its parent's implementation.
    }

let g_null_vm2_iinfo =
    {
       generated_by=         ""
       vlnv= {vendor= "HPRLS"; library= "-nolib-"; kind= ["-noname-"];  version= "1.0" }
       iname=                "" // We must be clear if this is the iname w.r.t to parent's iname or total flat iname.  It is the former, a relative iname.
       externally_provided=  false
       nested_extensionf=    false
       external_instance=    false
       preserve_instance=    false
       in_same_file=         false
       definitionf=          false
    }
    
type hpr_machine_t =
   HPR_VM2 of 
        minfo_t                 (* Machine info *)
     *  decl_binder_t list      // Declarations of nets and interfaces and local variables etc.
     *  (vm2_iinfo_t * hpr_machine_t option) list // Child machine instance or instance refs.
     *  hblock_t list           (* Executable rule blocks - mainly Xassign's *)
     *  xrule_t list            (* Other and non-executable rules - initial/safe/live *)


let g_null_minfo = {name=g_null_vm2_iinfo.vlnv; (*bevmodel=SP_comment "no bev model";*) atts=[]; memdescs=[]; fatal_flaws=[]; squirrelled= ""; hls_signature= None }

let null_hpr_machine s =
    let x:hpr_machine_t option = None
    ({g_null_vm2_iinfo with definitionf=true; iname=s}, x)


let builtin_var x = 
    match x with
       | ss when ss = g_tnow_string -> Some(ss, 64)
       | (_) -> None


(* Deconstructor/access functions *)
let hpr_name x = match x with (HPR_VM2(minfo, decls, sons, execs, asserts)) -> (minfo.name) 


let hpr_minfo x = match x with (HPR_VM2(minfo, decls, sons, execs, asserts)) -> minfo


let hpr_sons x = match x with (HPR_VM2(minfo, decls, sons, execs, asserts)) -> sons

let l_eis_mirrorable = { g_default_fsems with fs_yielding=true; fs_nonref=true; fs_eis=true }
let l_eis_unmirrorable = { g_default_fsems with fs_yielding=true; fs_nonref=true; fs_eis=true; fs_mirrorable=false }

let l_reftran = { g_default_fsems with fs_reftran=true }

let g_hpr_alloc_gis = add_ip_block_name ("hpr_alloc", { g_default_native_fun_sig with  biosnumber=12; fsems={ g_default_fsems with fs_nonref=true; fs_eis=true }; rv=g_pointer_prec; args=[g_default_prec; g_string_prec]})


let g_signed_prec = { widtho=None; signed=Signed }                        

#if NOT_REMOVED
// These are supported via attributes only now
let g_hpr_atomic_add_fgis = add_ip_block_name ("hpr_atomic_add",  { g_default_native_fun_sig with  biosnumber=33; fsems=l_eis; rv=g_signed_prec; args=[g_signed_prec; g_signed_prec]; outargs="b-" })

let g_hpr_exchange_fgis = add_ip_block_name ("hpr_exchange", { g_default_native_fun_sig with  biosnumber=34; fsems=l_eis; rv=g_signed_prec; args=[g_signed_prec; g_signed_prec; g_signed_prec; g_signed_prec]; outargs="b---" })
#endif

// Write to unary leds on blade/card (perhaps virtualised through substrate).
// This should be implemented in leaf recipe stages such as verilog_gen and cpp_render.
let g_hpr_sysleds_fgis = ("hpr_sysleds", { g_default_native_fun_sig with  biosnumber=35; fsems=l_eis_unmirrorable; rv=g_void_prec; args=[g_default_prec]})

// cf let g_pointer_size = 8L     // A pointer takes 8 bytes since we have a 64-bit virtual machine. 

let g_hpr_sysexit_fgis = ("hpr_sysexit", { g_default_native_fun_sig with  biosnumber=3; fsems=l_eis_unmirrorable; rv=g_void_prec; args=[g_default_prec]});

// Built-in primitive functions:
let hpr_native_table:(int * string * native_fun_signature_t) list =
  [
      //
    (0, "hpr_pause",      { g_default_native_fun_sig with  biosnumber=0; rv=g_void_prec; args=[g_default_prec]; fsems=l_eis_mirrorable } );
    (1, "hpr_testandset", { g_default_native_fun_sig with  biosnumber=1; rv=g_bool_prec; args=[g_bool_prec; g_bool_prec]; fsems=l_eis_unmirrorable; outargs="b-" } ); // Prefer to use CompareExchange instead in future...
    (2, fst g_hpr_abs_fgis,       snd g_hpr_abs_fgis)
    (3, fst g_hpr_sysexit_fgis,    snd g_hpr_sysexit_fgis)
    (4, "hpr_writeln", { g_default_native_fun_sig with  biosnumber=4; fsems={ g_default_fsems with fs_nonref=true; fs_eis=true }; needs_printf_formatting=true; rv=g_void_prec; args=[]});
    (6, "hpr_write",   { g_default_native_fun_sig with  biosnumber=6; fsems={ g_default_fsems with fs_nonref=true; fs_eis=true }; needs_printf_formatting=true; rv=g_void_prec; args=[]});
    (7, "hpr_concat",  { g_default_native_fun_sig with  biosnumber=7; rv=g_string_prec; args=[g_string_prec; g_string_prec]});

    // Note: some of these table entries are canned in meox too.
    (8, fst g_hpr_max_fgis, snd g_hpr_max_fgis);
    (9, fst g_hpr_min_fgis, snd g_hpr_min_fgis);

    (10, "hpr_select",  { g_default_native_fun_sig with  biosnumber=10; fsems={ g_default_fsems with fs_nonref=true; }; rv=g_absmm_prec; args=[g_int_prec]});    

    (11, "hpr_KppMark", { g_default_native_fun_sig with  biosnumber=11; fsems={ g_default_fsems with fs_nonref=true; fs_eis=true }; rv={g_default_prec with widtho=None; }; args=[g_bool_prec]}) 
    (12,  fst g_hpr_alloc_gis, snd g_hpr_alloc_gis)

    (13, "hpr_pause_control",  { g_default_native_fun_sig with  biosnumber=13; fsems={ g_default_fsems with fs_nonref=true; fs_eis=true }; rv={g_default_prec with widtho=Some 32; }; args=[g_default_prec]});        

   //  ("romarray", { g_default_native_fun_sig with  biosnumber=9; rv= g_default_prec, args=[g_default_prec; g_default_prec]})  (* Currently a bodge for SW *) 

    (14, fst g_hpr_flt2dbl_fgis, snd g_hpr_flt2dbl_fgis);  // Convert between floating point precisions
    (15, fst g_hpr_dbl2flt_fgis, snd g_hpr_dbl2flt_fgis);

    (16, fst g_hpr_flt2int32_fgis, snd g_hpr_flt2int32_fgis);  // Convert integer forms to/from floating point forms.
    (17, fst g_hpr_flt2int64_fgis, snd g_hpr_flt2int64_fgis);
    (18, fst g_hpr_dbl2int32_fgis, snd g_hpr_dbl2int32_fgis);
    (19, fst g_hpr_dbl2int64_fgis, snd g_hpr_dbl2int64_fgis);

    (20, fst g_hpr_flt_from_int32_fgis, snd g_hpr_flt_from_int32_fgis); // Further convert integer forms to/from floating point forms.
    (21, fst g_hpr_flt_from_int64_fgis, snd g_hpr_flt_from_int64_fgis);
    (22, fst g_hpr_dbl_from_int32_fgis, snd g_hpr_dbl_from_int32_fgis);
    (23, fst g_hpr_dbl_from_int64_fgis, snd g_hpr_dbl_from_int64_fgis);
    (24, fst g_hpr_toChar_fgis, snd g_hpr_toChar_fgis);

    (25, fst g_hpr_bitsToDouble_fgis, snd g_hpr_bitsToDouble_fgis); // Get access to floating point as raw bits.
    (26, fst g_hpr_bitsToSingle_fgis, snd g_hpr_bitsToSingle_fgis);
    (27, fst g_hpr_doubleToBits_fgis, snd g_hpr_doubleToBits_fgis);
    (28, fst g_hpr_singleToBits_fgis, snd g_hpr_singleToBits_fgis);

    (29, fst g_hpr_strlen_fgis, snd g_hpr_strlen_fgis);

    (30, fst g_hpr_sqrt_dp_fgis, snd g_hpr_sqrt_dp_fgis)   // Square roots - being implemented - why are these hardwired and not the rest of the maths library ?  Being fixed 4Q18.  Also, don't like overloads being supported by listing twice?
    (31, fst g_hpr_sqrt_sp_fgis, snd g_hpr_sqrt_sp_fgis)
    // Use hpr_sysexit instead please
    // (, "hpr_abend", { g_default_native_fun_sig with  biosnumber=; fsems=l_eis; rv=g_void_prec; args=[g_default_prec]})  

    (32, fst g_hpr_pending_read_fgis, snd g_hpr_pending_read_fgis)
#if NOT_REMOVED
    (33, fst g_hpr_atomic_add_fgis,   snd g_hpr_atomic_add_fgis)
    (34, fst g_hpr_exchange_fgis,     snd g_hpr_exchange_fgis)
#endif
    (35, fst g_hpr_sysleds_fgis,      snd g_hpr_sysleds_fgis)
    
  ]

// Other potentially builtin functions are the 10 cv_fp_cvt functions, one of which doubles up on the lightweight hpr_flt2dbl.
  

let builtin_function(s) = List.fold (fun c (n, x, gis) -> c || s=x) false hpr_native_table

let builtin_fungis s =
    let ans = List.fold (fun c (n, x, gis) -> if s=x then Some gis else c) None hpr_native_table // Stupid linear dictionary search!
    ans
    
let builtin_id x = builtin_function x || builtin_var x <> None


let g_jbase (_) = 12*4096


let g_iobase (_) = 12*4096


// Layout of user items in a memory.
// The wanted data width is either some integer multiple number of external words, or some fraction of a word where the fraction is rounded up to a bounding power of 2 number of lanes.
type layout_t =
    {
        n_dwidth: int;            // Intrinsic width of underlying content date (e.g. 1 for a Bool).
        r_dwidth: int;            // Real width - rounded up to a multiple of lane width.
        packed: bool;             // Several items are stored per word.  
        words_item_ratio: int;    // If packed this is the numer of items per word. Otherwise it is the number of words per item.
    }

let g_runtime_heap_vm_base = 1L * 1000L * 1000L * 1000L

// First arg to hpr_pause is a bit vector with the following encodings.  This also can serve
// as read and write fences in the future...
module hpr_PauseFlags =
    [<Literal>]
    let NoUnroll = 1
    let SoftPause = 2
    let HardPause = 4
    let CurrentModePause = 8
    let EndOfElaborate = 16
    let NextToUse_ = 32
    // To be added next:
    //let WriteFence = 32
    //let ReadFence = 64



        
(* eof *)
