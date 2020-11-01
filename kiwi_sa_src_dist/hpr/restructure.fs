(*
 * Restructure - open source (simple) version. 
 *
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
 * (C) 2003-17 DJ Greaves.
 *
 *
 * All rights reserved, except as licensed by LGPL below. (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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

 *
 
 *
 * This module implements scheduling of reads and writes to memory arrays, re-pipelineing and structural hazard avoidance.
 *
 *
 * Restructure RTL or bev.  This can be called as restructure1 and restructure2 - currently these are the same code, but run before and after bevelab in a typical recipe.  These will likely have different coding forms with before using the DIC code array and the after being XRTL parallel assignments or FSMs.
 
 * Now only res2 (for RTL) is used. The bev mode may have stopped/rotted.
 *
 *
 * This operation either supports machines that are essentially in RTL form: ie XRTL?
 * and in a different way handles untimed bev code...
 *
 * (NB: Where a Kiwi remote call is used, those operations are implemented in the kiwic front end, not here).
 *
 * Restructure1: For outboard resources (kiwi or otherwise), the hpr_arrayops are expanded here and the extra I/O
 * terminals are added. Really just used for hsimple and other future behavioural macros
 *
 * Restructure2: ALU and RAM instances are created and structural hazards resolved.  A static schedule for each thread is constructed.
 * Res2 is the main pipelining engine as found in classical HLS, working out loop reinitiation intervals and avoiding RaW hazards between simultaneously-active iterations.
 *
 *)


(*
 * Restructure provides assistance with floating point ALUs, off-chip memories, inter-compilation procedure calls and other instances used as shared resources.
 * NB: To use Xilinx Virtex block RAM inference we need a single register on the address bus.
 *)


module restructure

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open hprls_hdr
open abstract_hdr
open abstracte
open moscow
open yout
open meox
open hprxml
open opath_hdr
open opath_interface
open tableprinter
open protocols
open cvipgen

open planhdr
open planoptimiser

open dotreport



// let g_res2_debug = true //false // We have this set to false for normal use.

let g_max_enum_len = ref 25



let g_primpc_F = "F"     // A special denotation of pmode - a code point of the thread's new PC.


(*
 * Restructure instantiates ALUs, ROMs, RAMs, loadstore ports and so on. It forms and implements a time-domain schedule for necessary pipeline and hazard avoidance.
 *
 * The restructure approach for XRTL is generally different from restucture of imperative code.  The 
 * former needs explicit stalls on top whereas the latter can dally at the bottom since
 * it has no hard real-time constraints.
 *
 *)


// Restructure2 operation: stages of processing flow (needs renumbering!):
// Overall, It rewrites RTL with additional microarcs/states to avoid hazards in any one microarc.
//
// 0. The execs are collated per clock domain and then per thread. Each thread may be in FSM RTL form or flattened RTL form. We would need to recreate FSM form if flattened.  Pure bev form is only handled by restructure 1.

// 1. We map res2_arcate_thread_old or else res2_arcate_thread_new over each thread. This is the main scheduller. For each thread it makes a number of attempts under the guidance of planopt.  It results in a schedule for each edge with commoned root parts for edges eminating from the same resume. The schedule is a sorted list of newopx_t records for each FU.
// 1.1 For each thread, arcate calls minor_times (old or new) for each resume and then for each edge of that resume. This binds to FU instances and forms a partial schedule, allocating relatives times from the start of that resume for each FU operation of each edge from that resume. 
// 1.2 The res2_make_index_x or res2_make_index_new does an assembly allocating an absolute npc value for each relative time.
// 1.3 The res2_apply_index applies the new values by rewriting the housekeeping and gotos.  The result is the scheduled_edge_t

//      (res2_minor_times both creates the shedule and the walker needed to recode the RTL)



// 2b. allocator2_domain is called to allocate memory ports to threads or clock domains (althogh this binding must have tacitly been assumed during schedulling).

// res2_render_resource_instance is called


// 3 res2_build_thread  and res2_form_cam_ops are called for each major state to create the output RTL. The output RTL contains input RTL that is a PLI call or a scalar assign, but where expression in it have been rewritten to refer to the outputs of new FUs here instantiated, and control logic for the inputs to the new FUs, and gotos for the controlling microsequencer.

// OLD: 4. nsfate is called - removes any x_X applied to rhs expressions since these are not supported for most output generators (e.g. RTL or SystemC).


// Some Call Graph Details
// -----------------------  
//  ->opath_restructure_vm2
//  ---->res2_trawl
//  ---->res2_arcate_thread{old/new}
//  ------->res2_minor_times
//  ------->res2_make_index
//  ---->allocator2_domain
//  ---->res2_render_resource_instance
//  ---->res2_build_thread
//  ------>res2_gen_xitions3
//  ------>res2_form_cam_ops



 // TODO rename spog as clever_tappet
 // spog-form output - i.e. output for a control net (cam shaft waveform) that understands and resolves stalls, disjunctions, defaults, dont-cares and both combinational and sequential nets.

type rtl_stall_mode_t = 
    | RAC_unstallable
    | RAC_trigger
    | RAC_stallable    
    | RAC_spog

type abort_mode_t =
    | A_unrequired // The operation has no side effects and can be abandoned without consequence
    | A_safe_until of int // The operation can be safely abandoned before cycle n
    | A_cannot_abort   // ...


// We use relative while schedulling and convert to absolute time (pc vlaues) afterwards.
// Relative time is offset from the start of a schedule.  There are separate read and write schedules for each major state with the first write state being the last read state, also known as the exec state.
// The first part of the read schedule is called the control or root schedule and is shared over all arcs of that major state because it resolves which arc is to be used.
// Absolute time relates to the run-time pc value, which can be stalled as well, so is not true time.
      
type schedule_temporal_extent_t = 
    {
      //exec_cycle: int; // exec cycle - for major state
      
      // A schedulled entry has a start time, an expected input busy time, an expected result time and an abort mode.
      rootf: bool

      // PCVS will not always be consecutive owing to one or more control states followed by edge-specific branch states

      old_pcvs: (int * housekeeping1_t) list // PC values in use by this - only for plotting.

      // deprecate ss and ee since non-consecutive in practice
      ss: (string * int) list; // start 
      //ee: (string * int) list; // end: not used
    }

type stallpoints_t = OptionStore<string * int, string> // used as a mutable se

type filx_t = Filx

let p2s (id, nn) = sprintf "%s%i" id nn
let shenToStr shen = sprintf "te:%s" (sfold p2s shen.ss)
        
type restr_subs_t =
  {
    wrval:  hexp_t option;               // Written value (missing for read)
    rdnet:  (hexp_t * hexp_t option (*unused?*) * hexp_t) option;    // Read bus/holding register/tlatch (missing for a write operation)
    id:     string;                      // hazard/hint id
    enables: hbexp_t list ref;           // enabling terms: more about when. disjunction denotes in use.
  }


let subsToStr (v:restr_subs_t) = "wrval=" + xoToStr v.wrval + " rdnet=" + (if v.rdnet=None then "None" else xToStr(f1o3(valOf v.rdnet))) + " id=" + v.id

// Additional FU info field - used ad/hoc variously for different forms of FU
// See also aux_generic_fu_info_t 
type fu_aux_info2_t =
    | INFO_layout of  bondout_port_t * offchip_static_region_t
    | INFO_no_of_ports of int
    | INFO_xref of (string * string) list
        
type temp_db_wt =
    | DB_PHY of hexp_t
    | DB_LOGICAL of string * string * string * string * (string * string) list * netport_pattern_schema_t * bool //  ikey, pi_name, logical net name and kinds, schema and  externally instantiated flag : We use this closure to refer to a port net that has perhaps not been rezzed yet.  

let gec_phy x = DB_PHY(x)

let pToStr = function
    | DB_PHY x      -> xToStr x
    | DB_LOGICAL(ikey, pi_name, ss, kind, meta, nps, externally_instantiated) -> ss

let pcpToStr = function // or pcpo ?
    | Some(a, b) -> "(" + xToStr a + "," + xToStr b + ")"
    | None -> "()"
    
    
type pc_statemap_t = Dictionary<hexp_t, hexp_t list>
type nsfmap_t      = Dictionary<hexp_t, (stutter_t * hbexp_t * hexp_t) list>

type rest_op_key_t =
    {
        subs:hexp_t;                   // The operation (address part)
        pp:pcp_t option;               // When needed.
        clko: edge_t list;             // Clock net option
    }

let keyToStr (k:rest_op_key_t) = "subs=" + xToStr k.subs + " pp=" + ppToStr k.pp + " clko=" + sfold edgeToStr k.clko

let waypoint_f = function
    | None -> "-"
    | Some(nn, ss) when nn >= 0 -> sprintf "%i:%s" nn ss
    | Some(nn, ss) -> ss


// Already held collated by the structural resource, which is fine.
// Idea is to partion the what and when, but key has resource name and when, whereas subs_t has detailed names of holding regs. Can we make them virtual holding registers.
type rest_op_t =  rest_op_key_t  *  restr_subs_t

   
type thread_idx_t = hexp_t

// Parameter overrides are first defined formally with rides1fun and then prepended to the actual contacts with rides2fun.
// Attributes contain nominal default values.  Actual values are placed at the head of the contacts list.
let rides0fun (key, value) = Nap(key, value)
let rides2fun (key, value) = xi_num value

type starttime_t = housekeeping0_t * hbexp_t * bool * housekeeping1_t // Printed by meox.vl2s3 : (startarcf, guard/readynet seemingly not always used even for v/l component ?, Write/Read_N, hk)

type min_scheduled_t = OptionStore<int, starttime_t>


type auxix_t = // Auxiliary FU information - an early form of meld before precise details are known, such as how many ports on an SRAM.
    {
      rei_interval:               int         // Reissue delay before next command issued (1 for fully pipelined)
      result_latency:             int         // Result delay: aka rd_latency, fixed latency or average prediction if variable
      raises_data_hazards:        bool
      scalar_rtl_hazard_semantic: bool  // This flag holds when a concurrent read and write within an mgr is alloweable and the read will get the old value, as in standard synchrnous logic and RTL scalars.
     
    }

type aux_generic_fu_info_t =
    {
        fname:       string
        arity:       int
        gis:         native_fun_signature_t
    }
// The following enumeration covers the RAM/Register/Memory Technologies are supported and automatically selected
type memtech_t =
    | T_PortlessRegFile of auxix_t // Any number of concurrent reads and writes are possible without worry over structural hazard) - combinational read.
    | T_Comb_RAM         // Comb_RAM is a native RTL array that is not restructured here - ie same as None. - Wrong - this would have a read port hazard so not same as None - TODO clarify - Best not use and instead use T_Onchip_SRAM with 0 latency
    | T_Onchip_SRAM of auxix_t * string option  // Single or multi-ported RAM (or named ROM) with specificed read latency.  Can be BRAM, LUT RAM or a synchronous register file.  Can be externally instantiated, which makes it a little like a bondout memory.  But we do not need to maintaing sequential consistency here.

    | T_Offchip_RAM of auxix_t * string   // spacename   This makes use of a bondout load/store port or ports for access to remote/offchip memory banks.

let memtechToStr = function
    | T_PortlessRegFile(auxix_t)    -> "T_PortlessRegFile"
    | T_Comb_RAM                    -> "T_Comb_RAM"
    | T_Onchip_SRAM(_, None)        -> "T_Onchip_SRAM"
    | T_Onchip_SRAM(_, Some rom_id) -> sprintf  "T_Onchip_SSROM(%s)" rom_id    
    | T_Offchip_RAM(_, space)       -> sprintf "T_Offchip_RAM(%s)" space


type internal_ip_record_t = (vm2_iinfo_t * hpr_machine_t option)


type alu_diop_t =
     { mkey:             string
       kind:             ip_xact_vlnv_t
       oo:               x_diop_t
       precision:        precision_t //int
       latency:          int  // no of pipeline cycles (average for vl, actual constant for fixed latency)
       auxix:            auxix_t
       variable_latencyf: bool
       //variable_latency: hexp_t list // [ rdy; req; ] handshake nets when variable latency.
       adt_actuals_rides_:   (string * int) list // Not really used, but for completeness for now.
       fu_meld:              protocols.block_meld_abstraction_t
     }


type fu_block_record_t = external_ip_block_record_t // Same thing


type miniblock_t =  // Please explain
    {
        fu_meld:   block_meld_abstraction_t
        area_o:    double option
    }
    
type str_form_variant_t = // structural resource, FU or AFU.

    | SR_generic_external of ip_xact_vlnv_t (*containing kkey*) * string option(*sq*) * bool(*externally_instantiated*) * precision_t option * aux_generic_fu_info_t option * (*mirrorable*) bool

    | SR_ssram  of string * net_att_t * memtech_t * precision_t * (int * int64) * int(*unused*) * bool * att_att_t list // Structural RAM : technology, (wid,len), no of ports, bool=romflag, NO stored in...
    | SR_alu    of alu_diop_t // operation performed (constant function, +-*/ etc)

type str_form_t = // structural resource, FU or AFU.
    | SR_fu of string * auxix_t * str_form_variant_t * miniblock_t option // Whether the string is an mkey or an ikey is not clear and we do not use that field
    | SR_scalar of hexp_t  // Scalar register write
    | SR_pliwork
    | SR_filler
    

    
//The entry in a schedule for an operation on an FU (aka structural resource).
//Mono crashes when builtin equality is used.
[<CustomEquality;CustomComparison>] 
type newopx_t =
    {
        serial:      int   // Unique id
        mkey:        string
        method_o:    method_str2_t option
        cmd:         string
        sq:          string option // Override disambiguation, needed where 'cmd' and arity paired are not unique.
        //action_grd:  hbexp_t       
        postf:       bool    // If postf holds, this is a posted operation with no separate result phase (writes generally).
        oldnx:       hexp_t  // Original expression.
        // The action (aka work) guard: these are further conditions beyond the edge being taken to qualify this operation (or at least the side-effecting parts of it). We need to watch out for its support having been updated.
        args_raw:    hbexp_t * hexp_t list // action_grd and arguments - pre buildout.
        g_args_bo:   (hbexp_t * hexp_t list) option ref // Modified action guard and args with build outs as needed from when in the schedule their values were live.
        unaltered:   xrtl_t list // PLI code and array assignments that are not being replaced with bus ops (present when nonep str).
        housekeeping:    (int * housekeeping1_t) list // Ancillary work times (e.g. acknowledge a write post)
        support_times:   starttime_t list     // Earliest possible start time
        writef:          bool                   // For data hazard checks
        plimeta:         ((string * native_fun_signature_t) * vsfg_order_t option) option
        customer_edges: int list
        decoderfo:       (hexp_t ->hexp_t) option
        fence_ordering:  vsfg_order_t list
    }
    with
        interface System.IComparable with
            member this.CompareTo obj =
                match obj with
                    | :? newopx_t as y ->  this.serial - y.serial
              
        override x.Equals (y: obj) =
           match y with
               | :? newopx_t as y -> x.serial = y.serial
               | _ -> false


        override this.GetHashCode () = this.serial

and newop_t = (bool * int) * int * int  * newopx_t  // Bool holds if used in the root schedule. The int paired with the bool is the ??.  The next two ints are the start and end busy times for this entry (which will be equal for a fully-pipelined resource).


// For dual-ported RAMs we allocate one port per clock domain by default.
// User attributes can override: TODO check this.
// Fully-pipelined components do not need their inputs holding externally by definition. For variable latency operations the contract is specified in fs_inhold and fs_outhold.

and str_instance_t = // aka dinstance_t  This record is a structural resource (or FU) instance. We have the concept of multiple ports onto the same structural resource. A port will typically support various methods within its associated methodgroup.
    {
      mkey:       string             // Instance name at mapping stage: primary key before mirroring  
      ikey:       string             // Instance name after binding, mirroring and load balancing.  
      kkey:       string             // kind key - the library name of the FU
      clklsts:    directorate_t list // One component can have two or more of these lists when it spans clock domains.
      actuals_rides:    hexp_t list
      item:             str_form_t         // aka sr: RAM, ALU or other ...
      area:             double
      propso:           net_att_t option   // base net (for an instantiated RAM/ROM)
      fu_meldo:         block_meld_abstraction_t option // Might as well have the meld here and then pi_name is perhaps meld.mgr_name
    }



and str_methodgroup_t = // Each method group on an FU has as least one of these records - there may be several methods inside the method group, such as read and write, but they all share the same handshake nets, static parameters (such as bus widths and expected latency) and, critically, are mutually exclusive in the time domain.  Note: the two ports of a dual-port RAM are concurrently useable and so will have separate method groups. 
    {
      mg_uid:               string             // The mg_uid is globally unique and is typically a composite of FU iname and pi_name.- it is non null even when pi_name is empty.
      mgr_meldo:            mgr_meld_abstraction_t option // Might as well have the meld here and then pi_name is perhaps meld.mgr_name
      pi_name:              string             // Either blank when  there is no port_instance name in the formal naming sheme, or else (normally) the same as mgkey.
      dinstance:            str_instance_t
      max_outstanding:      int                // Number of concurrent outstanding requests (will be equal to result_delay for fully-pipelined, fixed-latency).
      auxix:                auxix_t
      oc_record:            offchip_static_region_t option   // Heap base address and portmap etc
      //rdycond:              temp_rdy_t option  // Ready indicator: None if fixed latency (or always output ready) component without handshakes.

      //inputs_inhold_external: bool   // Anything non-fully pipelined may request its inputs to be externally held while it is busy. Ditto outhold ... TODO this is now in fs_inhold ...


      methodgroup_methods:  method_str2_t list  // The individual methods of the methodgroup.

    }// We don't need phys_methods now we have this ?

and method_str2_t =
    {
      fname:             string
      overload_suffix:   string option
      precision2:        precision_t
      holders:           hr_scoreflop_t list ref list  // Preferred holding registers per clock domain.
      method_meld:       method_meld_abstraction_t option
      m_mgro:            str_methodgroup_t option ref
      latency:           int
    }


    //  A scoreboard is either a read or a write schedule (for one FU?) for an edge. In pipelined mode, there is only the read schedule.

and cmdstring_t = string    
    
and scoreboard_t =
    { //adv_guards:   (int * hbexp_t) list ref; // Stall if guard (rdy||vld) does not hold in a given state.
      scboard:      (cmdstring_t * readat_t * scorex_t) list ref                       // The schedule. A scorex_t may be used by several scoreboard_t instances owing to cloning.
      live_hrs:     (((string * string) * xidx_t * string) * hr_scoreflop_t) list ref  // Ancillary control structures and holding registers used
      //assist        : (hr_scoreflop_t * stutter_t * hbexp_t * hexp_t) list ref;
    }

// A readat_t holds information about when the support of an operation is needed ready for approach 2.
and readat_t =
    { // nominal_rdy_time_: int // When ready to be used. Negative -1 if always ready.  DELETE ME - we now use scorex.readyat
      sux_times:       starttime_t list
      atcmd:           string
    }

// A scorex is a final wrapper around an operation on an FU or copying some of its info .... TODO make better. 
and scorex_t =
    { old:     hexp_t                 // Original expression
      oldx_n:           xidx_t        // Original expression's expression number
      opcmd:            string
      overload_suffix:  string option // Which method of identically named opcmds.
      opargs:           hexp_t list
      newx:    hexp_t                 // Replacement expression - typically the output from an FU (or via rd_smart when holding registers present)
      startat: (string * int) list
      readyat: (string * int)         // Nominal abs ready time.
//      trigger: hbexp_t;
      s_mgr:   str_methodgroup_t
   }

and temp_rdy_t = (string (*iname*) * vlat_t) // Please rename
    
and hr_scoreflop_t =
    | HR_grdten of hexp_t * int * int
    | HR_argreg of hexp_t * hexp_t
    | HR_vl of string * hexp_t * temp_rdy_t * hexp_t * hexp_t      // Holding register for the result from a variable-latency component. With primed and valid flags.
    | HR_shot of string * hexp_t * hexp_t list // Holding register for a fully-pipelined, fixed-latency component. A shotgun shift register runs alongside it, never stalled, to capture the result in the case that the main logic is stallled and wants the result later.

 




    
type memtech_mapping_t = Dictionary<int, memtech_t * (string * string * str_form_t * bool) option>

type cbg_track_t =
    | CBG_track of string * int * int // (mc, npvc, eno)
    | CBG_track_filler
    
// Predictor graph entry
type PGE_d_t =
    | PGE_wp of cbg_track_t * waypoint_t list 
    | PGE_wi of cbg_track_t * schedule_temporal_extent_t * newopx_t 


type fu_quota_key_t = string


type per_thread_static_fu_limit_t = OptionStore<fu_quota_key_t, int>

type structure_instance_counter_t =
    {
      quota_key:   fu_quota_key_t
      rs_limit:    int // This is deployment bound: the maximum number of physical instances
      so_far_made: int ref // How many used - when this execeeds the limit the one in use is allocated cyclically with a mod operator although it would be better to be more intelligent about this, perhaps using spacial or structural affinities.
    }


// An assoc of containing maximum number to allocate per thread. The quota is decremented on each allocate by the binder.   
// We have one of these for each thread, the thread_fu_counter.  Items shared over threads, like DPRAMs count as what?
// We also have one for each binding step of that thread perhaps?
type per_thread_structure_counter_t = OptionStore<fu_quota_key_t, structure_instance_counter_t>


type per_thread_structure_count_limit_store_t = OptionStore<fu_quota_key_t, structure_instance_counter_t>


type asi_minor_shed_t =
    {
        minor_shed_hwm:  int
        xminor_shed:     (string -> newop_t list ref)  // TODO probably better to delete the ref here to avoid unexpected mutations.
   }

type saved_arc_schedule_t = // aka asi 
    {
        eno:             int
        edge:            vliw_arc_t
        control_length:  int
        post_pad:        int
        root_get_shed:   bool -> string -> newop_t list ref

        m_asiso:         asi_minor_shed_t option ref
    }


// We do not need both 'saved_arc_schedule_t'  'proto_scheduled_edge_t' in the future.
type proto_scheduled_edge_t = // There is not much 'proto' about these - they are hard/final absolute pc values.
    {
        resume:          hexp_t
        iresume:         int
        edge:            vliw_arc_t
        eno:             int          // edge reference number - in edge anyway

        maj_root_pc:     int
        dend:            int

        priv_start_pc:   int
        priv_last_pc:    int
        assocs:          (int * bool * string * int) list       // (oldv, writef, modechar, newv)
        dest_pc:         int

        // New
        asio:            saved_arc_schedule_t option
        reverb_state_names: (int * hexp_t) list
        // Old
        priv_exec_pc:    int
    }


type shed_style_t =
    | SS_old of proto_scheduled_edge_t option ref * int * int
    | SS_new of proto_scheduled_edge_t option ref * saved_arc_schedule_t
    | SS_abort of string
    
type scheduled_edge_t =
    {
        pse:             proto_scheduled_edge_t
        edge:            vliw_arc_t

// Don't need to copy all this out from pse now
        maj_root_pc:     int
        priv_start_pc:   int
        dend:            int
        priv_last_pc:    int
        priv_exec_pc:    int

        work:            (int * (cbg_track_t * schedule_temporal_extent_t * newopx_t) list) list
        waypoint:        (int * string) option // For reference and debugging.

        //pli:             ((hbexp_t * bool * int) * xrtl_t) list      // new only
    } 

type resume_t = hexp_t
    
type mapping_needs_t              = OptionStore<string, string * resume_t * int option * str_form_t * xidx_t list>  

type local_resources_collection_t = ListStore<string, str_methodgroup_t * int> // The int is the use count (on this edge)


type mapping_log_for_resume_t = OptionStore<resume_t, mapping_needs_t>
type mapping_log_for_edge_t   = OptionStore<int, mapping_needs_t>
// Allocate a fresh one of these for each major state
// Also, clone to a fresh one of these for each edge (but note pooling of root section over edges)

type rpool_t = 
  {
        mapping_needs:              mapping_needs_t option               // FU forms requiring binding used by this edge or major state root.
        resource_pool:              local_resources_collection_t  // FU concrete instances to be 
        edge_no_o:                  int option
  }

type vm2_id_t = ip_xact_vlnv_t
 
type perthread_t = // Allocate a fresh one of these for each thread processed.
    {
        thread:                     hexp_t // Thread name taken from input PC name.
        thread_str:                 string
        director:                   directorate_t
        vm2_ids:                    vm2_id_t list
        thread_fsems:               fun_semantic_t

        mapping_log_for_resume:     mapping_log_for_resume_t
        mapping_log_for_edge:       mapping_log_for_edge_t

        edge_resources:             rpool_t option ref
        maj_resources:              rpool_t option ref
        
        item_no_dbase:              OptionStore<string, int>

        // Code geneation phase only:
        pipate:                     pipate_controller_t
        m_max_state_time:           int ref
        m_next_stritem_no:          int ref
        m_thread_fu_summary_report:     (string * str_methodgroup_t) list ref // Just for reporting in dot and table forms.

        // Bind phase only:
        //itembudget:                 per_thread_structure_count_limit_store_t
        //thread_fu_counters:         per_thread_structure_counter_t
    }

type anal_manager_t =
    {
      reprint: bool        // Print out reworked input FSM
              // Report files for printing at the end
      dot_plots: (string * ((xml_t * dotitem_t list) list * (xml_t * dotitem_t list) list)) list ref;  // Aux plotting outputs - accumulated here if plotted separately.
      dot_plot_combined: bool
      dot_plot_detailed: bool      
      dot_plot_separately: bool
    }


type restruct_manager1_t =
    {
      filler1:          string
      stagename:        string 
      offchips:         (bondout_address_space_t * bondout_port_t list * bondout_memory_map_manager_t) list
      vd:               int                                                // verbal diorosity
      m_morenets:       hexp_t list ref                              // New nets and holding registers
      tableReports:     string list list ref
      stall_nets:       (string * hexp_t) list ref                 // Per-clock-domain stall net - used now only to count clock domains
    }

type alu_tech_params_t =
    {
        varilat:        bool // Whether variable latency.
        baseline:       int  // Baseline value for the latency.
        flash_limit:    int  // Number of results bits below which a combinational/flash implementation should be used.
        perbit:         int  // Augmentation of latency per bit of result width.
        rei_interval:   int  // Re-issue delay (1=fully pipelined)
//      description:    string
    }


let g_alu_latency_int_div =
    {
        varilat=       true    // We always use variable-latency dividers
        baseline=      15      // Override this with a width-base formula
        flash_limit =  3
        perbit=        3000
        rei_interval=     1       // This will be a max-outstanding field instead for varilat FUs.
    }


let g_alu_latency_int_addsub =
    {
        varilat=  true
        baseline=   -1   // Use -ve baseline to show that we should not instantiated these as FUs and instead leave to the backend logic synthesisers to do combinationally.
        flash_limit = 10000000
        perbit=     -1   // Ditto
        rei_interval=   0
    }



type portmeta_t = (string * string) list

type contact_map_t = Map<string, decl_binder_t list * ((string * hexp_t) * netport_pattern_schema_t) list * portmeta_t * str_instance_t option>

type bound_resources_ledger_t =
    {
       xition_summaries: (int * (string * int)) list ref        // Transitions of output FSM
       global_resource_thread_use:  ListStore<string, string>   // Resources and the thread that uses them.
       global_resources:  ListStore<string, str_instance_t>     // FU resources such as ALUs, RAMs and offchip ports.
       global_mgrs:       ListStore<string, str_methodgroup_t>  // Separately schedullable ports on FUs.
    }
    
type restruct_manager2_t =
    {
//    filler2: string
      stagename:string

      offchips:        (bondout_address_space_t * bondout_port_t list * bondout_memory_map_manager_t) list // Bondout ports
      static_manifest:             (int * (str_instance_t * str_methodgroup_t * portmeta_t)) list ref // All static FUs, including bondouts.
      
//    dotplot: dotplot_control_t
      anal:                       anal_manager_t
      externally_flagged_kinds:   string list                // FUs to override natural instantiation mode (internal/external)
      tableReports:               string list list ref
      ppgen_enable:               bool                       // Performance graph plot

      stall_nets: (string * hexp_t) list ref                 // Per-clock-domain stall net - used now only to count clock domains
      regen_sequencer:            bool                       // Refactorize results for a good-looking (readable) and factorisable case statement.

      vd:                         int                        // verbal diorosity
      avoid_dontcare_stall:       bool                       // Munge edge guard conditions to be friendly to don'tcares in RTL_SIM.
      asubsc_rmode_merge_enable:  bool                       // Enable sharing of read data between sites. restructure. This should normally be set true but at least one regression test fails when enabled,
      m_morenets:                 hexp_t list ref            // New nets and holding registers      

      regfile_threshold:       int64                         //
      combrom_threshold:       int64        //
      combram_threshold:       int64        //
      offchip_threshold:       int64        //
      include_bondout_ports_when_not_even_used: bool               // Included I/O ports for DRAM and offchip SRAM even when unused
      extend_for_pli_mode:     string        

      memtech_mapping:         memtech_mapping_t
      m_old_nets_for_removal:  hexp_t list ref

      alu_latency_fp_sp_mul: alu_tech_params_t   // ALU floating point, single-precision latency value for multiply.
      alu_latency_fp_sp_add: alu_tech_params_t   // ALU floating point, single-precision latency value for add/sub
      alu_latency_fp_sp_div: alu_tech_params_t   // ALU floating point, single-precision latency value for divide.

      alu_latency_fp_dp_mul: alu_tech_params_t   //  ALU floating point, double-precision latency value for multiply.
      alu_latency_fp_dp_add: alu_tech_params_t   //  ALU floating point, double-precision latency value for add/sub
      alu_latency_fp_dp_div: alu_tech_params_t   //  ALU floating point, double-precision latency value for divide.

      alu_latency_int_mul:   alu_tech_params_t   // "Fixed-latency integer ALU integer latency scale factor for multiply."
      alu_latency_int_div:   alu_tech_params_t   // "Integer ALU integer latency value for divide."
      alu_latency_int_addsub:alu_tech_params_t   // "Integer ALU integer latency value for adders and subtractors."

      //alu_iproduct_limit:     int                // "Structural ALU threshold for integer multiply." 

      ram_max_data_packing:       int
      rom_mirrors:                int // should be under per_thread_static_fu_limit_t

      per_thread_static_fu_limits:      per_thread_static_fu_limit_t

      roms_have_ren:             bool   // Set when a ROM has a read-enable input

      //grdtens: ((int * int) * hr_scoreflop_t) list ref
      next_eno: int ref
      scalar_hr_db:              OptionStore<string, hexp_t * hr_scoreflop_t> // Global/scalar
      pipelining:                bool // Pipeline over major state boundaries - of course, within an arc, many operators are always pipelined.
      extra_post_pad:            int  // Additional post pad requested for debuging visibility and/or RaW hazard debugging.
      schedule_attempts:         int
    }


let g_null_str2 =
    {
        mkey=                     "blankikey"
        ikey=                     "blankikey"
        kkey=                     "CV_FP_BLANK"
        clklsts=                  []        
        actuals_rides=            [] // not cur
        area=                     -0.001
        item=                     SR_filler
        propso=                   None
        fu_meldo=                 None
    }

let g_default_auxix =
        {  
           rei_interval=                1
           result_latency=              1
           raises_data_hazards=         false
           scalar_rtl_hazard_semantic=  false
        }

let g_null_methodgroup =
    {
        dinstance=                g_null_str2
        mgr_meldo=                None
        mg_uid=                   ""
        pi_name=                  ""
        //overload_suffix=          None
        max_outstanding=          1
        methodgroup_methods=      []
        //holders=                  [] // NB: Ensure you make a new ref cell each time you import this, rather than having a shared one in g_null_str2.
        //rdycond=                  None
        oc_record=                None
        //inputs_inhold_external=   false
        auxix=                    g_default_auxix
    }


let g_null_newopx =
    {
        serial=       -1
        mkey=         "anon"
        support_times=       []
        cmd=                 "g_null_newopx"
        sq=                  None
        //action_grd=          X_true
        args_raw=            (X_true, [])
        g_args_bo=           ref None // Never dirty this ref cell in the null_newopx instance - please make it constant some how!
        customer_edges=      []
        method_o=            None
        unaltered=           []
        oldnx=                X_undef
        postf=               false
        housekeeping=        []
        writef=              false
        plimeta=             None
        decoderfo=           None
        fence_ordering=      []
    } 



//-----------------------------------

let mirrorable_sr = function

//  | SR_fu(_, _, SR_ssram(kind, ff, tech, prec, (wanted_width, l), portref, is_rom, atts)) -> is_rom
//  | SR_fu(_, _, SR_generic_external(_, sq, block, ei_, prec, auxi_)) -> block.meld.mirrorable
    | SR_fu(_, _, SR_alu _, _)   -> true
    | SR_fu(_, _, SR_ssram(kind, ff, tech, prec, (wanted_width, l), _, is_rom, atts), _) -> is_rom
    | SR_fu(_, _, SR_generic_external(kinds, sq, ei_, prec, auxio, mirrorable), _) -> mirrorable
    | other -> sf(sprintf "Other form in mirrorable_sr: %A" other)


            
//-----------------------------------
let mgrTextualName = function // Show textual name or other id for a methodgroup. 
    | None -> "anon"
    | Some str_method2 ->
        match !str_method2.m_mgro with
            | None -> "missing-mgr." + str_method2.fname
            | Some mgr ->
                str_method2.fname + "/" + mgr.mg_uid + "/" + mgr.dinstance.ikey

let g_quota_key_manifest =         
    [
      ("max-no-int-divs",   "Maximum number of int dividers to instantiate per thread.", 2)
      ("max-no-fp-divs",    "Maximum number of f/p dividers to instantiate per thread.", 2)                   
      ("max-no-int-muls",   "Maximum number of int multipliers to instantiate per thread.", 3)
      ("max-no-fp-muls",    "Maximum number of f/p multipliers or dividers to instantiate per thread.", 6)
      ("max-no-fp-addsubs", "Maximum number of adders and subtractors (or combos) to instantiate per thread.", 6)
    ]
                
// Need to recreate these instance counters for each thread. The instances are reused over major states within each thread but freshly instantiated per thread.
let rez_structure_count_limits (rms:restruct_manager2_t) = 
    let thread_fu_counters = new per_thread_structure_counter_t("thread_fu_counters")
    let mk_counter (key, desc_, defv_)  =
        match rms.per_thread_static_fu_limits.lookup key with
            | Some limit ->
                let nb = { quota_key=key; rs_limit=limit; so_far_made=ref 0; }
                thread_fu_counters.add key nb
            | None ->
                hpr_yikes(sprintf "Quote key %s missing in receipe quota manifest" key)
    app mk_counter g_quota_key_manifest
    thread_fu_counters

let item_no perthread ikey =
    let rec item_no = function
        | W_asubsc(x, _, _) -> item_no x
        | other -> xToStr other

    let no = ikey // item_no ikey
    match perthread.item_no_dbase.lookup ikey with // Possibly this should be per-mgr, not per-ikey, but only used for debug visualisation  at the moment perhaps.
        | Some ov -> ov
        | None ->
            let vv = !perthread.m_next_stritem_no
            perthread.m_next_stritem_no := vv + 1
            perthread.item_no_dbase.add ikey vv  //
            vprintln 3 (sprintf "Allocated item number (col no) %i for ikey %s" vv ikey)
            vv

    
let rec depair = function
    | X_pair(l, r, _) -> depair r
    | other -> other



//Say whether a memory if offchip, in which case it needs an entry in a memory map instead of being an on-chip structural resource.
let offchipp = function
    | T_Comb_RAM         -> false // Comb_RAM is a native RTL array that is not restructured here - ie same as None.
    | T_PortlessRegFile _-> false // Any number of concurrent reads and writes are possible without worry over structural hazard)
    | T_Onchip_SRAM _    -> false // Single-ported RAM with specificed read latency.  Can be BRAM, LUT RAM or a synchronous register file.
    | T_Offchip_RAM(auxix, spacename)   -> true  // spacename * portno


let unaltered_pred = function
    | T_Comb_RAM          -> true // Comb_RAM is a native RTL array that is not restructured here - ie same as None.
    | T_PortlessRegFile _ -> true // Any number of concurrent reads and writes are possible without worry over structural hazard)
    | _ -> false
    
let scorexToStr scorex = sprintf "{rdytime=%s, ikey=%s, oldx=%s, result=%s}" (p2s scorex.readyat) scorex.s_mgr.mg_uid (xToStr scorex.old) (xToStr scorex.newx)




let domain_no (str:str_instance_t) d =
    let rec sc n = function
        | [] -> sf (sprintf "No such clock on component. d=%A" d)
        | h::t when h.clocks=d -> n
        | _::t -> sc (n+1) t
    sc 0 str.clklsts

    
let g_exec_marker = (0, gec_X_net "$exec", xi_one, xi_zero)  // A dummy net used by the restructucture naive scheduller to indicate the execution cycle.



let tuple_rtlToStr = function
    | (nx, Some(pc, pcv), g1, l, r, defval) -> i2s nx + " " + xToStr pc + "=" + xToStr pcv + " g=" + xbToStr g1 + " l=" + xToStr l + " r=" + xToStr r
    | (nx, None, g1, l, r, defval) -> i2s nx + " NoPP g=" + xbToStr g1 + " l=" + xToStr l + " r=" + xToStr r    


let tuple0_rtlToStr = function
    | (nx, l, r, defval) -> i2s nx + " " +  " l=" + xToStr l + " r=" + xToStr r


let tuple1_rtlToStr = function
    | (sl, nx, en, l, r, defval, rdyGrd, spog) ->
        sfold p2s sl + " nx=" + i2s nx + " g=" + xbToStr en +  " l=" + pToStr l + " r=" + xToStr r + " rdyGrd=" + xbToStr rdyGrd

    
// A cam net is a control input to an instantiated structural component.
// A cam net has the control_net_marked attribute set so that conerefine will keep it.
#if OLD
let rez_camnet__old__ ikey pi_name logical_name = // We need the logical names for the IP-XACT port maps.
    let port_instance_name = if pi_name = "" then [] else [ Nap(g_port_instance_name, pi_name) ]
    let id = ikey + "_" + (if pi_name = "" then "" else pi_name + "_") + logical_name
    let net = xgen_bnet(fst(iogen_serf(id, [], 0I, 1, LOCAL, Unsigned, None, false, port_instance_name @ [Nap(g_control_net_marked, "true");  Nap(g_logical_name, logical_name)], []))
    //dev_println (sprintf "rezzed camnet %s" (netToStr net))
    net
#endif


type microarc_t =
    { ma_idx:    (string * int)    // A microarc number - not the same as eno since there are commonly multiple microarcs per input edge.
      exec_flag: bool              // Holds if this state is the main state that corresponds to the original oldstate.
      oldstate:  string            //
      gpc:       hexp_t * hexp_t   // Pair: (new_pc var, new_pc label)
      npcv:      int               // new_pc label as int
      mag:       hbexp_t           // transition guard
      dest:      hexp_t            // next state.
      adv_guard: hbexp_t option    // Chance of stalling
      eno:       int               // Originating input arc
    }


let g_shading_col = ref 0

// Find port_structure entry for a given formal name.
//
let meld_assoc_formal pi needle pattern_lst =
    let matcher cc (pi_name, formal_name, sp) = if (pi="" || pi_name="" || pi=pi_name) && formal_name=needle then Some (pi_name, formal_name, sp) else cc
    List.fold matcher None pattern_lst


// We do not need to provide more than one definition for mirrored ROMs and so on, so scan for existing def.
let scan_for_existing_fu_definition newdef sonlist =
    match newdef with
        | Some(HPR_VM2(minfo, decls, sons, input_execs, assertions)) ->
            let rec scan = function
                | [] -> false
                | (ii, Some vm2)::tt when ii.definitionf && (hpr_minfo vm2).name=minfo.name && (hpr_minfo vm2).squirrelled = minfo.squirrelled -> true
                | _::tt -> scan tt
            scan sonlist
        | None -> false


// Meld/melsh is a datastructure containing several forms of datasheets and interpretable code for an IP block: we need to know how to drive its nets from TLM form and an high-level simulation model is preferably needed for diosim or should be loadable too ...
// Some older components are still hardcoded below, but we are moving to the meta/melsh form for all future FUs blocks, where they themselves are synthesised, handcoded, library, third party etc..
let get_mgr_meld block_meld mgro fname = 
    //let mgr_melds = List.filter (fun mgr_meld -> true)
    //dev_println (sprintf "portno ps is '%s'"  cmd_ps)
    //muddy (sprintf "fofxxcc want %A from %A"  mgro block_meld.mgr_melds)
    let by_fname() =
        let rec scan mgr_meld cc = 
            let ya (method_meld:method_meld_abstraction_t) = method_meld.method_name = fname
            if disjunctionate ya  mgr_meld.method_melds then mgr_meld::cc else cc

        once "L889" (List.foldBack scan block_meld.mgr_melds []) // TODO this code assumes all overloads of fname are in the same mgr.


    match mgro with
        | Some mgr ->
            match mgr.mgr_meldo with
                | None -> by_fname()
                | Some mgr_meld -> mgr_meld
        | None -> by_fname()

let get_method_meld block_meld ikey (mgr_meld:mgr_meld_abstraction_t) (fname, sq, arity) = 
    let yesf (meld:method_meld_abstraction_t)  = meld.method_name=fname && meld.method_sq_name=sq && meld.arity=arity 
    match List.filter yesf mgr_meld.method_melds with // Look up in method table.
        | [] ->
            let xactor_name_to_str2 (meld:method_meld_abstraction_t) = sprintf " %i   %s " meld.arity meld.method_name
            let pos = "\nCandidates are " + sfold xactor_name_to_str2 mgr_meld.method_melds
            sf (sprintf "res2: Component %s mgkey=%s supports no transaction called '%s' sq=%A that takes %i arguments" ikey mgr_meld.mgr_name fname sq arity + pos)
        | [item] -> item
        | _ -> sf ("More than one matching method meld defined for " + fname)    


// Lookup the net connected to a component instance by the name and method group of the formal.
let get_actual contact_maps msg iname pi_name logical_name =
    let attribute_match_pred tag sv (ff:net_att_t)  =
        let ans = 
            if tag = g_port_instance_name && sv = "" then true
            else
                let f2 = lookup_net2 ff.n
                match at_assoc tag f2.ats with
                    | Some a when lname_eqeq a sv -> true 
                    | Some x -> false
                    | None ->
                        false
        //dev_println(sprintf "attribute_match_pred: %s cf %s -> %A" sv ff.id ans)
        ans

    let ans =
        match (contact_maps:contact_map_t).TryFind iname with
            | None -> sf (msg + ": get_actual:missing: no contact map stored for " + iname)
            | Some(paz, phys_contacts, portmeta_, str2_) ->
                let rec get_actual_assoc cc = function
                    | DB_group(ii_, lst) ->
                        //dev_println (sprintf "Paz assoc for %s on %A" ii_.kind lst)
                        List.fold get_actual_assoc cc lst
                    | DB_leaf(_, Some(X_bnet ff)) when attribute_match_pred g_logical_name logical_name ff && attribute_match_pred g_port_instance_name pi_name ff -> Some(X_bnet ff) 
                    //| DB_leaf(Some f, Some actual) when net_id f=formal_name -> Some actual
                    | _ -> cc

                let pos () =
                    let rec paz_scan cc = function
                        | DB_group(ii_, lst) -> List.fold paz_scan cc lst
                        | DB_leaf(Some f, ans) -> netToStr f :: cc
                        | DB_leaf(None, Some ans) -> ("fid=anonymous/actual=" + netToStr ans) :: cc
                    "Possibilities were: " + sfoldcr id (List.fold paz_scan [] paz) + " "

                match List.fold get_actual_assoc None paz with
                    | None -> sf (msg + sprintf ": get_actual:missing: L4921: No matching formal net found for iname=%s  pi_name=%s  logical_name=%s.  " iname pi_name logical_name + pos())
                    | Some a -> a
    //dev_println (sprintf "get_actual: iname=%s pi_name=%s logical_name=%s  net=%s" iname pi_name logical_name  (netToStr ans))
    ans



let retrieve_handshake_inputs contact_maps = function 
    | (iname, vlat) ->
    //TRD1(iname, fname, (pi_name, prec, formal_name, _, actual_id)) ->
    //| TRD2(net) -> net
        let utemp1 (pi_name, prec_, nps, _) =
            get_actual contact_maps "ee" iname pi_name nps.lname
        let req_rdy_o = if nonep vlat.vlat_reqrdy_o then None else Some(utemp1 (valOf vlat.vlat_reqrdy_o))
        let ack_net = utemp1 vlat.vlat_ack
        (req_rdy_o, ack_net)


    //| _ -> sf "L4292 retrieve_handshake_inputs"
        


//
let get_melsh ww kinds op logical externally_instantiated fu_meld mkey method_str2 mwhen0 portmeta =
    let arity = length (snd op.args_raw)
    let mgr = valOf_or_fail "L932" !method_str2.m_mgro
    //let mgr_meld = valOf_method_str2 // get_mgr_meld fu_meld op.mgro op.cmd
    //dev_println (sprintf "tracker kinds=%s  mgkey=%s " (hptos (kinds:ip_xact_vlnv_t).kind) mgr.mg_uid)
    let method_meld = valOf_or_fail "L934" method_str2.method_meld // get_method_meld fu_meld mkey  mgr_meld (op.cmd, op.sq, arity)
    let mgr =         valOf_or_fail "L973" !method_str2.m_mgro
    let mgr_meld =    valOf_or_fail "L974" mgr.mgr_meldo
    //let (meld_kinds, meld_structure, meld_xactors, sim_model) = meld

    let (en, args) = valOf_or !op.g_args_bo op.args_raw
    let gwen = xi_blift en
    //match !op.g_args_bo with
    //   | None          ->  dev_println (sprintf "URGENT args_bo are None")
    //   | Some(g, args) ->  dev_println (sprintf "URGENT args_bo gg=Some %s" (xbToStr g))
    let phys_structure = // Late rez site
        let strfab pi_name panda = (pi_name, panda.lname, panda) // not using portmap here - use it?
        let mgr_structure (mgr_meld:mgr_meld_abstraction_t) =
            map (strfab mgr_meld.mgr_name) mgr_meld.contact_schema
        map (strfab "") fu_meld.contact_schema @ list_flatten(map mgr_structure fu_meld.mgr_melds)

    // The pi_name on the caller and the callee need not matchin in certain designs (e.g. port 2 of switch 1 connected to port 0 of switch 2)
    // The pi_name, when not the empty string, is prefixed to the logical name of a formal, except when pi_name is an integer when we instead suffix the integer on the logical name.
    let pi_name = mgr.pi_name
    let cams = method_meld.meld_xactor
    //dev_println (sprintf "URGENT: OX%i 1/3 start get melsh for cmd=%s pi_name=%s  gwen=%s mwhen0=%A" op.serial op.cmd pi_name (xToStr gwen) mwhen0)
    let camops = 
        //dev_println (sprintf "URGENT: 2/3  Want to see if camop support is built out correctly for argregs and so on. bo=%A." (not_nonep !op.g_args_bo))
        //dev_println (sprintf "get_melsh: 3/3 cmd=%s. logical=%A. arity=%i  Raw cams are %A" op.cmd logical arity cams)
        let scen_gen (twhen, bips) cc =
            if twhen = "t1" then
                vprintln 2 ("Ignored t1 in melsh for now") // no explict acknowledgement for the result needs driving, but ackrdy may be used soon ...
                cc // for now
            else
                let whener =
                    if twhen = "t0" then mwhen0 // for burst xfers there will be more ...
                    elif twhen = "t1" then muddy "twhen1 t1 ..."
                    else sf (sprintf "res2: Component %s %s has unknown twhen of '%s' in its xactor" mkey (vlnvToStr kinds) twhen)

                let scen_gen1 (name, expr) cc =
                    let net = name 
                    let (formal_name, biz, defv) =
                        if logical then 
                            let logical_name = name
                            match meld_assoc logical_name phys_structure with //This looks up the logical name of a net with an IP-XACT port, which is what we normally want.
                                | None ->
                                    let pos = "\nCandidate logical nets are " + sfoldcr_lite (fun (pi_name, formal_name, panda) -> sprintf "pi_name=%s  formal_name=%s   lname=%s " pi_name formal_name panda.lname) phys_structure
                                    sf (sprintf "res2: meld_assoc_formal: logical mode: Component %s mkey=%s mgkey=%s supports no logical net called '%s' referred to in its %s xactor" mkey "mgkey" (vlnvToStr kinds) net op.cmd + pos)
                                | Some (pi_name__, formal_name, netport_pattern) ->
                                    let defv = (if netport_pattern.idleval = "0" then xi_zero elif netport_pattern.idleval = "1" then xi_one else X_undef)
                                    (formal_name, netport_pattern, defv)
                        else // We have different assoc
                            // The formal_name is the tag string needed for an associative instantiation.
                            let formal_name = name // Formal name is logical name when there is no method group (aka pi_name) extension.

                            match meld_assoc_formal pi_name formal_name phys_structure with
                                | None ->
                                    // let pos = "\nCandidate nets are " + sfoldcr (fun (pi_name, formal_name, panda) -> sprintf " %s %s %s " pi_name formal_name panda.lname) phys_structure
                                    let pos = "\nCandidate formal nets are " + sfoldcr_lite (fun (pi_name, formal_name, panda) -> sprintf "pi_name=%s  formal_name=%s   lname=%s " pi_name formal_name panda.lname) phys_structure
                                    sf (sprintf "res2: meld_assoc_formal: Component %s %s sports no formal port called '%s' (pi_name=%s) referred to in its %s xactor" mkey (vlnvToStr kinds) net pi_name op.cmd + pos)
                                | Some (pi_name, formal_name, netport_pattern) ->
                                    let defv = (if netport_pattern.idleval = "0" then xi_zero elif netport_pattern.idleval = "1" then xi_one else X_undef)
                                    (formal_name, netport_pattern, defv)
                    let defv = if memberp biz.lname [ "req"; "REQ" ] then xi_zero else defv // Temporary override until read in  - also want to do control-net marked perhaps
                    let vale =
                        if expr = "0" then xi_zero
                        elif expr = "1" then xi_one
                        elif expr = "G" then gwen
                        elif expr.Length > 3 && expr.[0..2] = "ARG" then
                            let ano = sscanf "ARG%i" expr
                            if ano < 0 || ano >= length (snd op.args_raw) then sf(sprintf "res2: Melsh arg out of range expr=%s cmd=%s args=%i^%s" expr op.cmd (length (snd op.args_raw)) (sfold xToStr (snd op.args_raw)))
                            let arg = select_nth ano args
                            //vprintln 0 (sprintf "res2: Melsh monitor: cmd=%s (net=%s := expr=%s)    ano=%i   vale=%s" op.cmd net expr ano (xToStr arg))
                            arg
                            
                        else sf  (sprintf "res2: Component %s %s: for logical net %s cannot parse argument macro '%s'" mkey (vlnvToStr kinds) net expr)

                    let rdycond =
                        if nonep mgr_meld.hs_vlat then []
                        else [ (mgr.dinstance.ikey, valOf mgr_meld.hs_vlat) ] // The correct req or ack guard should be selected later depending on whener.
                    //dev_println  (sprintf "URGENT: melsh per-arg marshal: formal name is %s  expr=%s vale=%s " formal_name (expr) (xToStr vale))
                    let camop = (whener, 0, en, DB_LOGICAL(mkey, pi_name, formal_name, (vlnvToStr kinds), portmeta, biz, externally_instantiated), depair vale, defv, rdycond, RAC_spog)
                    camop::cc

                List.foldBack scen_gen1 bips cc
        List.foldBack scen_gen cams []  
    //vprintln 2 (sprintf "get_melsh: return")
    camops // end of get_melsh



//--------------------------------------------------------------
// Cross-reference to a local number space for concise output reporting.
let g_enumbers = new Dictionary<hexp_t, int option>()

let next_enumber = ref 1

let fresh_enumber() =
    let ans = !next_enumber
    next_enumber := ans + 1
    ans

let enumber2s n = sprintf "E%i" n    

let xeToStr x =
    let (found, ov) = g_enumbers.TryGetValue x
    if found && not (nonep ov) then enumber2s (valOf ov) + " " + xToStr_concise x else xToStr_concise x

let rec xbToE x = xToE (xi_blift x)

and xToE x = // Concise expression numbers for table output
    let ss = xToStr_concise x
    let (found, ov) = g_enumbers.TryGetValue x
    if found then if nonep ov then ss else enumber2s (valOf ov)
    else
        let ov = (if strlen ss < !g_max_enum_len then None else Some(fresh_enumber()))
        g_enumbers.Add(x, ov)
        if nonep ov then ss else enumber2s (valOf ov)


// Expressions are given short aliases called enumbers in printed tables for conciseness.    
let report_enumbers vd mlist =
    let m_p = ref []  
    youtln vd ("Concise expression alias report.")
    let count = ref 0
    let prt x =
        function
            | None -> ()
            | Some n ->
                let line = ("  " + enumber2s n + " =.= " + xToStr x)
                mutadd m_p (line + "\n")
                mutinc count 1
                youtln vd (line)
    for z in g_enumbers do prt z.Key z.Value done 
    if !count = 0 then youtln vd ("  -- No expression aliases to report")
    mutadd mlist !m_p
    ()

    
// Generate predictor XML and dot visual output file entries.
// Please move me to a freestanding recipe stage.
let ppg_gen_serf ww anal resName thread microarcs resources predictor_graph_work =
    let ww = WF 3 "dotout ppg_gen" ww "start"
    let threadname =  xToStr thread
    let pcLabName (id, pcv) = sprintf "%s:STX_%s_%i" threadname id pcv
    let threadcolor = g_pastels.[!g_shading_col % List.length g_pastels ]
    mutinc g_shading_col 1

    let edgeName (id, nn) = sprintf "edgebloc_%s_%i" id nn

    let house2a msg eno op =
        let selex (e, item) cc = if e<0 || e=eno then singly_add item cc else cc
        match List.foldBack selex op.housekeeping [] with
            | [HK_abs(mwhen0, _)] -> (mwhen0, [])
            | [HK_abs(mwhen0, _); HK_abs(mwhen1, _)] ->
                //vprintln 0 (sprintf "hk2 mwhen=%s  and  %s" (sfold i2s mwhen1) (sfold i2s mwhen2))
                (mwhen0, mwhen1)
            | items -> sf (sprintf ": %s: house2a L884 missing housekeeping: items=%A" msg items)

    let getArc npcv eno =
        // search new eno in microarc_t
        let rec gra2 = function
            | [] ->
                //dev_println (sprintf "getArc: gra2 no arc %i,%i" npcv eno)
                None
            | (h:microarc_t)::_ when h.npcv = npcv && h.eno = eno -> Some h
            | _ :: tt -> gra2 tt
        gra2 microarcs


    let resource_use_arcs =
        let gen_resource_use_arc cc (exec_pcv, pge_lst) =
            let grua cc = function
                | PGE_wi (CBG_track(mc_, npcv_, eno), shen, newop) ->
                    match newop.method_o with
                        | Some method_str2 ->
                            let sc2 (* aka mgr *) = valOf_or_fail "L1103" !method_str2.m_mgro
                            let name = resName sc2.mg_uid
                            //vprintln 3 (sprintf " resource use arc %s te=%s" name (shenToStr shen))
                            let (mwhen0, mwhen1) = house2a "L654" eno newop 
                            let label =  sc2.dinstance.kkey
                            let ats = [ ("shape", "hexagon"); ("label", label); ("color", "blue") ]
                            let addBlockUseArcs cc (eno_, HK_abs(vl, _)) =


                                let addBlockUseArcs_serf cc (v:(string * int)) =
                                        if newop.cmd = "write" then cc // TODO generalise command name for posted ops using the protocol flag posted_writes_only.
                                        else
                                        let gubbins = "cmd=" + newop.cmd + "\n" + sfoldcr_lite (fun x->fully_sanitise (xToStr x)) (snd newop.args_raw)

                                        let nl =
                                            if anal.dot_plot_detailed then
                                                [DARC(DNODE(edgeName v,""), DNODE(name,""), [  ("color", "blue"); ("label", gubbins) ])]
                                            else
                                                let adon_name = funique "adon_name" // Do not make an arc to the resource unless detailed since causes a confused drawing.  Instead have an add-on local node.
                                                [
                                                    DNODE_DEF(adon_name, [("label", gubbins)])
                                                    DARC(DNODE(edgeName v, ""), DNODE(adon_name,""), [  ("color", "blue");  ])
                                                ]

                                        let x = XML_ELEMENT("ru1", [],
                                                            [
                                                              XML_ELEMENT("label", [], [ XML_ATOM label]);
                                                              XML_ELEMENT("cmd", [], [ XML_ATOM newop.cmd]);
                                                              XML_ELEMENT("name", [], [ XML_ATOM name]);
                                                            ])
                                        let resp =
                                            if newop.postf || nonep newop.method_o then []
                                            else
                                                let dests rv =
                                                    let n2 = DARC(DNODE(name,""), DNODE(pcLabName rv,""), [  ("style", "dashed"); ("color", "blue"); ("label", "*result*") ])
                                                    (XML_WS " ", [n2])
                                                map dests mwhen1 
                                        (x, nl) :: resp @ cc
                                List.fold addBlockUseArcs_serf cc vl
                            let cc = List.fold addBlockUseArcs cc shen.old_pcvs // can use mwhen0 instead
                            let n = DNODE_DEF(name, ats)
                            let x = XML_ELEMENT("ru", [],
                                                [
                                                  XML_ELEMENT("label", [], [ XML_ATOM label]);
                                                  XML_ELEMENT("name", [], [ XML_ATOM name]);
                                                  XML_WS "\n"
                                                ])
                            (x, [n]) :: cc
                        | _ -> cc

                | PGE_wp(CBG_track(mc_, npcv, eno), waypoints) ->
                    let ag pp g =
                        let s1 = match pp with
                                   | None -> ""
                                   | Some (pc, pcv) -> xToStr pcv
                        let s2 = if g=X_true then "&&" else " && " + xbToStr g
                        s1+s2

                    let mao = getArc (* mc *) npcv eno

                    // The reverb states are not shown - these should be tails hanging off the midpoint of an edge.

                    // If we were smarter we would illustrate the control flow arc going via the waypoint shape!
                    let make_wp_shape cc wpf conds msg =
                        let k_int = exec_pcv
                        let wname = funique (sprintf "%s_waypoint_icon%i" threadname k_int)
                        //vprintln 3 (sprintf " graph/plot waypoint %s %s " wname msg)
                        let label = fully_sanitise msg
                        let ats = (if wpf then [("fillcolor", "orange"); ("shape", "octagon")] else[("shape", "note")]) @ [ ("label", label); ("color", "red"); ("style", "filled");  ]  
                        // We want to connect to the edge symbol from the execpc on this edge
                        let from =
                            match mao with
                                | None    -> pcLabName (g_primpc_F, exec_pcv) // TODO this is root pcv for a control edge structure access
                                | Some ma -> edgeName ma.ma_idx
                        let n0 = DARC(DNODE(from,""), DNODE(wname,""), [  ("color", "orange"); ("label", fully_sanitise conds) ])
                        let x0 = XML_ELEMENT("ru1", [],
                                                    [
                                                      XML_ELEMENT("label", [], [ XML_ATOM label]);
                                                      //XML_ELEMENT("cmd", [], [ XML_ATOM newop.cmd]);
                                                      XML_ELEMENT("name", [], [ XML_ATOM wname]);
                                                    ])                                
                        let n1 = DNODE_DEF(wname, ats)
                        let x1 = XML_ELEMENT("wP_", [],
                                                [
                                                  XML_ELEMENT("label", [], [ XML_ATOM label]);
                                                  XML_ELEMENT("name", [], [ XML_ATOM wname]);
                                                  XML_WS "\n"
                                                ])

                        (x0, [n0])::(x1, [n1])::cc
                    let waypoint_tog cc = function
                        | WP_wp(pp, g, nn, msg)            -> make_wp_shape cc true  (ag pp g) msg
                        | WP_other_string (pp, g, fn, msg) -> make_wp_shape cc false (ag pp g) (fn + ":" + msg)

                    List.fold waypoint_tog cc waypoints
                    
            List.fold grua cc pge_lst
        List.fold gen_resource_use_arc [] (list_flatten predictor_graph_work) // Perhaps ignore this and plot from the microarcs?

    let getRepresentativeArc n =
        let rec gra = function
            | [] -> None
            | h::_ when h.npcv = n -> Some h
            | _ :: tt -> gra tt
        gra microarcs

    let start_name = threadname + "_START"
    let startNode  =
        let label = "START"
        let ats = [ ("shape", "box"); ("label", label) ]
        let n = DNODE_DEF(start_name, ats) 
        let x = XML_ELEMENT("node", [],
                            [
                              XML_ELEMENT("name", [], [ XML_ATOM start_name]);
                              XML_ELEMENT("label", [], [ XML_ATOM label]);
                            ])
        (x, [n])

    let startEdge =
        let n = DARC(DNODE(start_name,""), DNODE(pcLabName (g_primpc_F, 0), ""), [ ("label", "true") ])
        let x = XML_ELEMENT("startnode", [], [])
        (x, [n])
        
    let gen_controlFlowNode sti =
        let name = pcLabName (g_primpc_F, sti)
        let arco = getRepresentativeArc sti // just for microarc oldPc
        let oldPc = if nonep arco then "--" else (valOf arco).oldstate
        let ea = if nonep arco then "" else (if (valOf arco).exec_flag then "\nEXEC" else "")
        let label =  sprintf "ST%i\n" sti + oldPc + "\n" + ea
        let ats = [ ("label", label); ("style", "filled"); ("fillcolor", threadcolor) ]
        let n = DNODE_DEF(name, ats) 

        let x = XML_ELEMENT("poss", [],
                            [
                              XML_ELEMENT("label", [], [ XML_ATOM label]);                
                              XML_ELEMENT("name", [], [ XML_ATOM name]);
                              XML_WS "\n"
                            ])
        (x, [n])


    let gen_controlFlowEdge  (ma:microarc_t) =  // A control flow edge is rendered as a dot node with a dot edge in and out of it
        let pc = xi_manifest "L419" (snd ma.gpc)
        let dest = xi_manifest "L420" ma.dest
        let gs1 = sprintf "A%s" (p2s ma.ma_idx)
        let gs2 = sprintf "A%s:\nE%i\n\"%s\"" (p2s ma.ma_idx) ma.eno (fully_sanitise (xbToStr_concise ma.mag))        
        
        let e_in = DARC(DNODE(pcLabName(g_primpc_F, pc), ""), DNODE(edgeName ma.ma_idx,""), [ ("color", "black"); ("label", gs1) ])

        let en =
            let label = gs2
            let ats = [ ("label", label);  ("color", threadcolor) ]
            DNODE_DEF(edgeName ma.ma_idx, ats) 

        let e_out = DARC(DNODE(edgeName ma.ma_idx, ""), DNODE(pcLabName (g_primpc_F, dest), ""), [ ("color", "black"); ("label", gs1) ])


        let x = XML_ELEMENT("xition", [],
                            [
                              XML_ELEMENT("src", [], [ XML_ATOM (i2s pc)]);                
                              XML_ELEMENT("dest", [], [ XML_ATOM (i2s dest)]);
                              XML_ELEMENT("guard", [], [ XML_ATOM gs2]);                              
                              XML_WS "\n"
                            ])
        (x, [en; e_in; e_out])


    let blockNodeIdxs = List.fold (fun cc ma->singly_add (xi_manifest "L410" (snd ma.gpc)) cc) [] microarcs       
    let nodes = startNode :: map gen_controlFlowNode blockNodeIdxs
    let edges = startEdge :: (map gen_controlFlowEdge microarcs)

    (nodes, edges @ resource_use_arcs)

// Write dot and control flow files. Separately after each thread or all combined at the end.    
let writeout_dot_plot ww keyname lst =
    let ww = WF 3 "writeout_dot_plot" ww "start"
    let filename = "controlflow_" + keyname + ".dot"

    let rec dezip = function
        | [] -> ([], [])
        //| [(key_, (priv, shared))] ->
        //    ([XML_ELEMENT("kthread", [], map fst priv @ map fst shared)], map snd priv @ map snd shared) // One copy of shared resources only is kept.
        | (key_, (priv, shared))::tt ->
            let (xc, dc) = dezip tt
            (XML_ELEMENT("kthread", [], map fst priv)::xc, map snd priv @ lst_union (map snd shared) dc) 
    let (xml_tree0, dot0) = dezip lst
    let xml_tree = XML_ELEMENT("kprg", [],  [XML_ELEMENT("kprg", [], xml_tree0)])
    hpr_xmlout ww ("profiler_cf_" + keyname + ".xml") false xml_tree

    
    let fd = yout_open_out filename
    yout fd (report_banner_toStr "// ")
    dotout fd (DOT_DIGRAPH("profiler_cf_" + keyname, list_flatten dot0))
    yout_close fd
    let ww = WF 3 "writeout_dot_plot" ww "done"
    ()


// Write Performance Predictor Graph and XML file
// For multiple threads we can write them combined or separately.
// It would be neater if this were a freestanding recipe stage (a generic HPR V/M report writer) instead of being part of restructure2.
let ppg_gen ww anal thread microarcs resources predictor_graph_work =
    let ww = WF 3 "dotprint" ww "start"
    let m_offchip_arcs = ref []
    let m_offchip_resources = ref [] // Currently find these as used but an explicit list passed in might be better
    //let anal = settings.anal
    
    let report_offchip_resource (oc_record:offchip_static_region_t) =
        let label = oc_record.port.ls_key // must tie up with gen_ for the offchip node
        mutaddonce m_offchip_resources oc_record
        label 

    let threadname =  xToStr thread


    let resName x = "RES_" +  x    

    let resource_nodes =
        let gen_resource_node (ks_, sc2) =
            let name = resName sc2.mg_uid
            let label =  sc2.dinstance.kkey
            let ats = [ ("shape", "trapezium"); ("label", label + "\n" + name) ]
            let n = DNODE_DEF(name, ats) 
            let x = XML_ELEMENT("resource", [],
                                [
                                  XML_ELEMENT("label", [], [ XML_ATOM label ]); 
                                  XML_ELEMENT("name", [], [ XML_ATOM name]);
                                  XML_WS "\n"
                                  ])
#if OLD
            let _ =
                match sc2.oc_record with
                    | None -> ()
                    | Some oc_record -> 
                        let v = report_offchip_resource oc_record
                        let n = (DARC(DNODE(name,""), DNODE(v,""), [ ("color", "red"); ]))
                        let x = XML_ELEMENT("ocRecord", [],
                                            [ XML_ELEMENT("port", [], [ XML_ATOM v;
                                                                        XML_WS "\n";
                                                                      ])])
                        mutadd m_offchip_arcs (x, [n])
#endif                        
            (x, [n])
        map gen_resource_node resources

    let (nodes, edges) = ppg_gen_serf ww anal resName thread microarcs resources predictor_graph_work

    let gen_offchipResourceNode (oc_record:offchip_static_region_t) =
        let label = oc_record.port.ls_key
        let ats = [ ("label", label); ("shape", "square"); ("color", "brown") ]
        let n = DNODE_DEF(label, ats) 
        let x = XML_ELEMENT("offchipResource", [],
                            [ XML_ELEMENT("name", [], [XML_ATOM label]);
                              XML_WS "\n"
                            ])
        (x, [n])

        // hmm not all resource nodes are shared! 
    let graph = (nodes @ edges @ !m_offchip_arcs, resource_nodes  @ (map gen_offchipResourceNode !m_offchip_resources))

    let _ =
        if anal.dot_plot_separately then
            writeout_dot_plot ww threadname [ (threadname, graph)]
        else
            mutadd anal.dot_plots (threadname, graph)

    let ww = WF 3 "dotprint" ww "done"

    ()






//======================================================================
// Work out the kind of ALU needed and also return the recipe settings for such an ALU

// This was called structured_alu_info
let alu_static_info ww settings msg prec oo =              //let sx = perthread.thread_fu_counters /// we want the static info only please now
    let vd = settings.vd
    let ww = WN "alu_static_info" ww
    let dp = prec.widtho = Some 64
    let fp = (prec.signed=FloatingPoint) 
    let (kindkey_root) =
        match oo with
            | V_divide _ -> ("DIVIDER")
            | V_plus     -> ("ADDER")
            | V_times _  -> ("MULTIPLIER")
            | V_mod      -> ("MODULUS")
            | other      -> sf (sprintf "other ALU component operator key %A" other)
    let (atp) =
        match oo with
            | V_mod  
            | V_divide _ -> (if not fp then settings.alu_latency_int_div    elif dp then settings.alu_latency_fp_dp_div else settings.alu_latency_fp_sp_div)
            | V_plus     -> (if not fp then settings.alu_latency_int_addsub elif dp then settings.alu_latency_fp_dp_add else settings.alu_latency_fp_sp_add)
            | V_times _  -> (if not fp then settings.alu_latency_int_mul    elif dp then settings.alu_latency_fp_dp_mul else settings.alu_latency_fp_sp_mul)

    let prec_key =
        let z = if prec.signed=FloatingPoint then "ALUF" elif prec.signed=Signed then "ALUS" else "ALUU"
        let w = i2s(valOf prec.widtho)
        z+w    


    let mkey = kindkey_root + "_" + prec_key

    let int_alu_info atp =
        let wid = valOf_or_fail "No int alu width" prec.widtho
        let uns = prec.signed=Unsigned
        let wid =  if not uns then wid
                   elif wid>32 then wid  // The purpose of these round-ups is perhaps to encourage sharing of multipliers rather than having a lot of different precisions.
                   elif wid>24 then 32        
                   elif wid>16 then 24
                   elif wid>8 then 16
                   else 8
        let aluform = sprintf "int_alu_info w=%i signed=%A" wid prec.signed
       
        let (use_fu, varilat, del, rei_interval) =
                match oo with // 2*wid + 2 // for now - rdy guards not being processed?
                | V_mod      
                | V_divide   -> (true, true, 2+(2000*wid)/5, atp.rei_interval) // TODO: get from settings.alu_latency_int_div - the varilatency divider takes worst case time twice the word width but generally, given numerator and denominator are often similar in magnitude, it takes often about 1/4 the word width.
                | V_plus     -> (false, false, 0, 0)// sf "Integer ADDER is never currently an instantiated FU"
                | V_times    ->
                    let del = atp.baseline + atp.perbit * wid
                    if vd>=4 then vprintln 4 (sprintf "Integer multiplier latency computation. vl=%A   baseline=%i/1000 perbit=%i/1000 wid=%i answer del=%i/1000 cycles." atp.varilat atp.baseline atp.perbit wid del)
                    (true, atp.varilat, del, atp.rei_interval)

        let del = del / 1000
        let del = if del < 1 then 1 else del
        // Integer ALUs (notibly flash multipliers) of result width less than atp.flash_limit should be left to the logic synthesiser (vendor FPGA tool). Such units will not be instantiated by arcate and hence the apt.flash_limit does not need to be checked here.
        let auxix =
                { g_default_auxix with
                    result_latency=  del
                    rei_interval=    atp.rei_interval
                }
        let kinds = "CV_INT_" + (if varilat then "VL_" else sprintf "FL%i_" del) + kindkey_root + (if uns then "_US" else "_S") 
        let rides = //  TODO - delete me - old way 
            [ ("rwidth", wid); ("nwidth", wid); ("dwidth", wid); ("trace_me", 0) ] // Give port widths for integer ALU unit parameters - result, a0, a1 - all the same currently.

        (use_fu, mkey, auxix, kinds, rides, del, atp)

    let fp_alu_info atp = 
        let wid = valOf_or_fail "No ALU width" prec.widtho
        //let (root_key, mkey, atp, no) = structured_alu_info ww settings prec oo // get again for no reason
        let dp = if wid=64 then true
                 elif wid=32 then false
                 else sf "Only single and double-precision FP is supported"
        let prec = { widtho=Some wid; signed=FloatingPoint; }

        let del = atp.baseline // No degrade per result bit for floating-point FUs - just single/double precision switch.
        let auxix =
                { g_default_auxix with
                    result_latency=  del
                    rei_interval=    atp.rei_interval
                }
        let kinds = "CV_FP_" + (if atp.varilat then "VL_" else sprintf "FL%i_" del) + kindkey_root + (if dp then "_DP" else "_SP") 
        let rides = [] // no overrides for floating-point FUs; it's all in the kind/name.
        (true, mkey, auxix, kinds, rides, del, atp)

    if prec.signed=FloatingPoint then fp_alu_info atp else int_alu_info atp
    // End of alu_static_info



            
//======================================================================
// Instance creator 
//
//       
let resource_add_use mkey (str, n) = sf "L989"

let newopxToStr ca = ca.cmd + "(g=" + xbToStr (fst ca.args_raw) + ", " + sfold xToE (snd ca.args_raw) + ")"

//
// rezzer_fu: create an instance of a functional unit.
// Note: the net-level contacts are not formed until res2_render_resource_instances.
//                                
let rezzer_fu ww settings callsite msg domain oraclef fresh_inamef sr iinfo =
    cassert(not_nullp domain.clocks, "null clocks L1708")

    let make_a_new_alu_or_similar_common_tail mkey adt sr iname del area = // This should divert to generic code?
        let portMaps = []
        let portmeta = [] // TODO this is needed for som FUs
        let str2 =
            { g_null_str2 with
                mkey=              mkey   // Floating-point ALU -again this code should be removed and we should redirect to the generic code
                ikey=              iname
                kkey=              hptos adt.kind.kind
              //actuals_contacts=  contacts
                clklsts=           [domain]
                item=              SR_fu(mkey, adt.auxix, sr, None)
                area=              area
            }

        let m_mgro = ref None
        let gec_method_str2 method_meld = 
            {
                fname=           method_meld.method_name // "compute"
                precision2=      adt.precision
                holders=         [ref[]] // Start with one domain.
                method_meld=     Some method_meld
                overload_suffix= None
                m_mgro=          m_mgro
                latency=         del
            }

        let mgr =
            let mgr_meld = hd adt.fu_meld.mgr_melds
            { g_null_methodgroup with
                mg_uid=           str2.ikey + "_" + mgr_meld.mgr_name
                pi_name=          mgr_meld.mgr_name
                dinstance=        str2
                mgr_meldo=        Some mgr_meld
                max_outstanding=  (if adt.variable_latencyf then 1 else del)
                auxix=            adt.auxix
                //rdycond=          ackrez ww portMaps portmeta iname mgr_meld
                methodgroup_methods = map gec_method_str2 mgr_meld.method_melds
              }
        m_mgro := Some mgr
        (str2, [mgr])

                
    let rez_a_fresh_alu mkey sr =
        match sr with

            | SR_alu adt when adt.precision.signed=FloatingPoint -> // { mkey=mkey; meld=meld; adt_actuals_rides_=rides; latency=del; oo=oo; kind={ g_canned_default_iplib with kind=kinds };  precision=prec; variable_latencyf=atp.varilat }
                let iname = funique ("if_" + mkey) // fresh_inamef implicit
                // kind and kkey are two versions of the same identifier for components in the top-level global HPR / CV library.
                vprintln 2 (sprintf "ALU latency lookup. vl=%A  prec=%s answer del=%i cycles kind=%s" adt.variable_latencyf (prec2str adt.precision) adt.latency (vlnvToStr adt.kind))
                let bits = valOf_or_fail "L1727" adt.precision.widtho
                let area = double(bits) * 10.0 // techcost.ialu_area_per_bit 
                make_a_new_alu_or_similar_common_tail mkey adt sr iname adt.latency area

        //box_alu_common_tail mkey prec oo sr make_a_new_one_fp alloca_freshp

            | SR_alu adt when adt.precision.signed<>FloatingPoint  -> // { mkey=mkey; meld=meld; adt_actuals_rides_=rides; latency=del; oo=oo; kind={ g_canned_default_iplib with kind=kinds };  precision=prec; variable_latencyf=atp.varilat }
                let uns = adt.precision.signed=Unsigned
                let iname = funique ((if uns then "iu_" else "is_") + adt.mkey)
                //dev_println (sprintf "URGENT Rez SR_ALU %s for nn=%i" iname (x2nn oldx))
                //if iname = "isMULTIPLIERALUS32_24" then sf (sprintf "hit git it callsite=%s" callsite)
                let bits = valOf_or_fail "L1727" adt.precision.widtho
                let area = double(bits) * 24.0 // techcost.fpalu_area_per_bit
                make_a_new_alu_or_similar_common_tail mkey adt sr iname adt.latency area
            
    
    let make_new_generic_fu msg sr  = 
        let externally_instantiated_o = None //Always passed in as None.  We can delete passing this to assoc since recomputed at rez/bind time?
        match sr with
            | SR_fu(mkey, auxix, SR_generic_external(kind, sq, externally_instantiated, data_prec_, auxinfo_o, _), miniblocko) ->
                // Internally instantiated FUs must not have the same iname as kind (both SystemC and Verilog do not like it).
                let ikey = if fresh_inamef then funique mkey elif not externally_instantiated && mkey = hptos kind.kind then "i_" + mkey else mkey
                let (area, fu_meld) = 
                    match miniblocko with
                        | None ->
                            let (block, fu_meld, phys_methods_, externally_instantiated_info) = assoc_external_block ww msg kind.kind externally_instantiated_o // IP-XACT lookup
                            let area =
                                match block.phys_area with
                                    | None ->
                                        hpr_yikes(sprintf "res2: No area information available for FU/AFU %s" (vlnvToStr kind))
                                        0.0
                                    | Some area -> area
                            (area, fu_meld)
                        | Some miniblock ->
                            let area =
                                match miniblock.area_o with
                                    | Some area -> area
                                    | None ->
                                        hpr_yikes(sprintf "res2: No area information available for canned/library FU %s" (vlnvToStr kind))
                                        0.0
                            (area, miniblock.fu_meld)


                let str2 =
                  { g_null_str2 with
                      item=             sr
                    //actuals_contacts= contacts
                      mkey=             mkey
                      ikey=             ikey 
                      kkey=             hptos kind.kind // kkey not is not really needed
                      clklsts=          [domain]
                      area=             area
                      fu_meldo=         Some fu_meld
                  }


                let mgrs =
                    let rez_mgr (mgr_meld:mgr_meld_abstraction_t) = 
                        let portMaps = []
                        let portmeta = [] // Wrong?
                        let m_mgro = ref None
                        let gec_method_str2_method (method_meld:method_meld_abstraction_t) =

                            let gis = 
                                match auxinfo_o with
                                    | Some auxinfo when mgr_meld.is_hpr_cv -> // We can only parameter override those that we know allow it. We cannot yet grock exoitic parameter semantics from IP_XACT.
                                        auxinfo.gis

                                    | _ -> 
                                        let (fu_meld_, (gis, result_details_, variable_latency_, arg_details_), externally_instantiated_, mirrorable_) = assoc_fu_method ww msg kind.kind method_meld.method_name method_meld.method_sq_name method_meld.arity externally_instantiated_o
                                        //dev_println (sprintf "assoc gix auxinfo=%A" auxinfo_o + sprintf " - assoc gix route rv=%A" gis.rv)
                                        gis
                            //dev_println (sprintf "gec_method_str2_method:LATEAM %s: method %s  method_meld.method_sq_name=%A  gis.overload=%A " mkey method_meld.method_name  method_meld.method_sq_name gis.overload_suffix)
                            let latency =
                                match gis.latency with
                                    | None -> 1
                                    | Some (result_latency, reint_interval) -> result_latency
                            in
                            {
                                fname=           method_meld.method_name
                                precision2=      gis.rv
                                holders=         [ref[]] // Start with one domain.
                                method_meld=     Some method_meld
                                overload_suffix= gis.overload_suffix
                                m_mgro=          m_mgro
                                latency=         latency
                            }


                        let mgr =
                            { g_null_methodgroup with
                                  pi_name=             mgr_meld.mgr_name
                                  mg_uid=              str2.ikey + "_" + mgr_meld.mgr_name
                                  dinstance=           str2
                                  mgr_meldo=           Some mgr_meld
                                  //rdycond=           ackrez ww portMaps portmeta ikey mgr_meld
                                  auxix=               auxix // per-method ?
                                  methodgroup_methods= map gec_method_str2_method mgr_meld.method_melds
                            }
                        m_mgro := Some mgr
                        //vprintln 3 (sprintf "Added global generic methodgroup mgkey=%s  varlat=%A" mgr.mg_uid  variable_latency)
                        mgr
                    map rez_mgr fu_meld.mgr_melds
                vprintln 3 (sprintf "Created instance of FU mkey=%s  fresh_inamef=%A  ikey=%s  kind=%s  %i method groups." mkey fresh_inamef ikey (vlnvToStr kind) (length mgrs))
                vprintln 3 (sprintf "Added global generic resource ikey=%s,%s  sq=%A" mkey ikey sq)
                (str2, mgrs)


    let make_new_ram_or_rom mkey sr =
        match sr with
            | SR_fu(_, auxix, SR_ssram(kind, ff, tech, prec, (wanted_width, locations), unused_, is_rom, atts), None) ->
                let no_ports =
                    match iinfo with
                        | Some(INFO_no_of_ports nn) -> nn
                        | _ -> 1
                let latency = 1 // FOR NOW  TODO - get this info from tech etc ... surely its there?
                //dev_println (sprintf "need latency from %A" tech)
                let fu_meld =
                    if is_rom then
                        let withRen = (at_assoc g_romram_with_ren atts = Some "true")
                        gen_cv_ssrom_port_structure withRen latency ff.id
                    else gen_cv_ssram_port_structure latency no_ports  // Synchronous static RAMs - with multiple ports when needed.
                let bits = int64 wanted_width * locations
                let area = double(bits) * 1.0 // techcost.ssram_area_per_bit
                let (ikey, ff1, f21) =
                    if is_rom || fresh_inamef then
                        let newname = funique (mkey)
                        let (nn, ov_) = netsetup_start newname
                        let ff1 =  { ff with id=newname; n=nn; is_array=true }
                        let f2 = lookup_net2 ff.n
                        let f21 =  { f2 with length=[locations] }
                        let ff1 = netsetup_log(ff1, f21)
                        mutadd settings.m_morenets (X_bnet ff1)
                        (newname, ff1, "f21") 
                     else (mkey, ff, "f2")
                let str2 =
                    { g_null_str2 with
                       mkey=             mkey
                       ikey=             ikey
                       kkey=             kind
                       item=             sr
                       propso=           Some ff1
                      //actuals_contacts= contacts
                       fu_meldo=         Some fu_meld
                       area=             area
                       clklsts=          [domain]
                     }
                vprintln 3 (sprintf "Created on-chip %s RAM/ROM FU mkey=%s  ikey=%s  kkey=%s" (if is_rom then "ROM" else "RAM") mkey ikey kind)
#if SPARE
                let auxix = // Second copy - TODO delete me after checking
                          { g_default_auxix with
                              raises_data_hazards=   not is_rom  // No data hazards on a ROM but yes on a register file or RAM.
                              result_latency=      latency//(valOf_or_fail "L2158" block_meld).typical_latency // Latency and precision are not the same for all methods in mgr and 
                              rei_interval=        1//(valOf_or_fail "L2158" block_meld).typical_rei
                          }
#endif
                let gec_mgr (mgr_meld, pno) = 
                     let m_mgro = ref None
                     let gec_method_str2 (method_meld:method_meld_abstraction_t) =
                         {
                             fname=           method_meld.method_name
                             precision2=      if method_meld.method_name = "write" then g_void_prec else  { signed=ff1.signed; widtho=Some wanted_width }
                             holders=         if method_meld.method_name = "write" then [] else [ref[]] // Start with one domain.
                             method_meld=     Some method_meld
                             overload_suffix= None
                             m_mgro=          m_mgro
                             latency=         auxix.result_latency
                         }

                     let mgr =
                         {  g_null_methodgroup  with
                                pi_name=           mgr_meld.mgr_name
                                dinstance=         str2
                                mgr_meldo=         Some mgr_meld
                                mg_uid=            str2.ikey + "_" + mgr_meld.mgr_name
                                methodgroup_methods = map gec_method_str2 mgr_meld.method_melds
                                auxix=           auxix
                                max_outstanding= 1//rd_latency        // Idiomatic for fully-pipelined 
                            //inputs_inhold_external=false       // Fully pipelined so this is n/a
                         }
                     m_mgro := Some mgr                            
                     mgr
                let mgrs = map gec_mgr (zipWithIndex fu_meld.mgr_melds)
                (str2, mgrs) // onchip RAM or ROM.  
                            
    match sr with
        | SR_fu(mkey, auxix, SR_generic_external _, meldo) -> make_new_generic_fu msg sr 

        | SR_fu(mkey, auxix, SR_ssram _, meldo) ->  make_new_ram_or_rom mkey sr

        | SR_fu(mkey, auxix, SR_alu adt, meldo) -> rez_a_fresh_alu mkey (SR_alu adt)

        | other -> sf (sprintf "rezzer_fu: other form %A" other)

    

// End of FU instance rezzing code
//====================================================================================

                                                                               
//
// Write out a structural FU (aka a resource) and any associated glue (was? called twice : we take a different part of the answer on each call).
//
let res2_render_resource_instances ww settings bounders (nets9, sons9, rtl9, contact_maps9) (pipate, director, str2) =

    //vprintln 2 (sprintf "FUs for this thread number %i" (length functional_units_this_thread))
    let vd = settings.vd
    let m_fx_set = ref [] // Freshly nulled for each thread. This means we can render a different port on the item for each thread (perhaps)?

    let static_manifest_instance_names =
        let get_iname (arg:str_instance_t) = arg.ikey
        map (snd>>f1o3>>get_iname) !settings.static_manifest
       
    let rez_gec_instance1 (nets9, sons9, rtl9, (contact_maps9:contact_map_t)) (str2:str_instance_t) = 
        let ikey = str2.ikey
        let iname = str2.ikey
            
        let (apply_pip_bx, apply_pip_x) = rez_pipate settings.m_morenets pipate
        vprintln 2 (sprintf "render_resource_instance mkey=%s  ikey=%s " ikey str2.ikey)

        let rez_opath1_ii stagename externally_instantiatedf i_name kind_vlnv =
            { g_null_vm2_iinfo with
                  vlnv=                 kind_vlnv // { g_canned_default_iplib  with kind=kkey }
                  iname=                i_name
                  externally_provided=  true
                  external_instance=    externally_instantiatedf 
                  preserve_instance=    true
                  generated_by=         stagename
              }


        match op_assoc iname !m_fx_set with
            | Some cmap ->
                //(nets9, sons9, rtl9, cmap::contact_maps9)
                (nets9, sons9, rtl9, contact_maps9)                
            | None ->
                mutadd m_fx_set (ikey, "contact_map") // ikey=iname
                let clkp = posfind "L1351" (once "L1351clk" director.clocks) // TODO - needs be inside gen_portnets for domain crossers

                //let clkscan (ro:rest_op_t) = if (fst ro).clko<>[] then clko := Some((fst ro).clko) // TODO - check operation of dual ports in different clock domains
                //app clkscan (!f.clko)
                if length str2.clklsts <> 1 then
                    sf (sprintf "Only one clock fx/directorate needed - domain crossing?  clocklsts=%s   kind=%s  iname=%s"  (sfold (dirToStr false) str2.clklsts)  str2.ikey iname)


                let ctrl = // The directorate render should now convey these nets through the design - we dont need to include them in the decls net list. But it does need to be in the contact map for the component.  But listing the same net more than once under various DB groups is allowed and that is what happens.
                    //sf (sprintf "dir is %A" perthread.director)
                    let (clk, clk_net) = clkp
                    let reset_net = if nullp director.resets then xi_blift(default_reset_f()) else f3o3(hd director.resets)
                    // TODO need to cope with reset and clocks not agreeing on polarity - currently we assume posedge clk and synchronous +ve reset.
                    // TODO perhaps use a private clock and reset net for these formals
                    [ (("clk", g_clknet), clk_net); (("reset", g_resetnet), reset_net) ]  // (formal, actual) pairs


                match str2.item with
                    

                | SR_fu(_, auxix, SR_generic_external(kind_vlnv, sq, externally_instantiatedf, data_prec, auxi_, _), miniblock_o) -> // res2_render_resource_instances clause
                    let fu_meld =
                        match miniblock_o with
                            | Some miniblock -> miniblock.fu_meld
                            | None -> sf ("l22 gec_fu_meld for " + vlnvToStr kind_vlnv)
                   // When externally_instantiated there is no instance to render and the net directions are different, but the contact map still needs augmenting.

                    let execs =
                        let only_meld mgr cc =
                            let boz2 method_meld cc =
                                method_meld.sim_model @ cc
                            List.foldBack boz2 mgr.method_melds cc
                            //sf (sprintf "only_meld: expected precisely one method group (and precisely one method in it) for '%s'" (vlnvToStr kind_vlnv))
                        List.foldBack only_meld fu_meld.mgr_melds []
                    let glue_bev = []
                    let portmeta = fu_meld.parameters // This would be the schema only! TODO, we will need  the actuals at some point please.

                    let phys_structure = // We should use the one from external_ip_block_record_t when we have it !  Rather than re-rez here?
                        let strfab pi_name panda = (pi_name, panda.lname, panda)
                        let mgr_structure (mgr_meld:mgr_meld_abstraction_t) =
                            map (strfab mgr_meld.mgr_name) mgr_meld.contact_schema
                        map (strfab "") fu_meld.contact_schema @ list_flatten(map mgr_structure fu_meld.mgr_melds)

                    //dev_println (sprintf "generic_external FU (IP block) instantiated: kind=%s: iname=%s  generic_external need melda" (vlnvToStr kind_vlnv) str2.ikey)
// for now - TODO use meld values instead please soon
                    //dev_println ("Making formals for " + iname)

                    let definitional_ikey = definitional_ikey_alloc ww "dkey" kind_vlnv (valOf_or sq "")  // We need a unique key for all nets owing to the memoizing heap system, but the RTL render will use instead the logical name for a contact, such as 'arg0' or 'NN'.
                    
                    let formal_contact_list = // Ideally we only make the formal contacts when rendering a definition of the child FU..
                        let null_ikey = ""
                        let flip = Some false // The component's contact direction remains the same regardless of where instantiated.
                        let formal_gen (pi_name, formal_name, nps) cc =
                            let nets = buildportnet ww vd flip (hptos kind_vlnv.kind) definitional_ikey portmeta pi_name formal_name data_prec nps cc
                            //dev_println  (sprintf "%s:%s  formal contact: generic_external externally_instantiatedf=%A rez net %s %s" (hptos kind_vlnv.kind) iname externally_instantiatedf nps.lname (sfold (snd>>netToStr) nets))
                            nets
                        List.foldBack formal_gen (phys_structure) []

                    //dev_println ("Making actuals for " + iname)
                    let (actual_contact_list, phys_contacts) =
                        let flip = if externally_instantiatedf then Some true else None// Declare the actuals (aka tappets) as locals if internally-instantiated, or flipped formals if external (up and above).
                        let actual_gen (pi_name, formal_name, nps) (cc, cd) =
                            let nets = buildportnet ww vd flip (hptos kind_vlnv.kind) iname portmeta pi_name formal_name None nps []
                            if nullp nets then (cc, cd)
                            else 
                                //dev_println (sprintf "%s:%s  actual_contact: generic_external externally_instantiatedf=%A rez net melda %s %s" (hptos kind_vlnv.kind) iname externally_instantiatedf nps.lname (sfold (snd>>netToStr) nets))
                                (hd nets :: cc, (hd nets, nps)::cd)
                        List.foldBack actual_gen phys_structure ([], [])
                    //dev_println ("Actuals made " + iname)

                    let actualssch =
                        let contact_gen (id, net) = ((id, net), net) // If, in future, we are passing expressions in we need net_id to generate a dummy net of the same precision.
                        map contact_gen actual_contact_list

                    let (formals_and_gubbins) =
                        let pi_name = iname // Pass in the iname as the pi_name here since all of the method group nets are atomically rezzed.
                        declsrez_formals settings.stagename pi_name ctrl "primary"  externally_instantiatedf (fu_meld.formals_rides, str2.actuals_rides, formal_contact_list, [])

                    let (actuals_decls, contacts) =
                        let pi_name = iname // Pass in the iname as the pi_name here since all of the method group nets are atomically rezzed.
                        declsrez_actuals settings.stagename pi_name ctrl "primary"  externally_instantiatedf (fu_meld.formals_rides, str2.actuals_rides, formal_contact_list, actualssch, [])

                    // For a custom AFU, such as a ROM, we render a definition whereas for CV library parts, like an ALU, FP convertor or BRAM, we do not.
                    // Note ROMs are under the SSRAM clause at the moment anyway.
                    let ii = rez_opath1_ii settings.stagename externally_instantiatedf iname kind_vlnv
                    let fu_definition = 
                            (ii, Some(HPR_VM2({g_null_minfo with name=ii.vlnv; atts=[ Napnode("parameters", map rides0fun  portmeta) ]; }, formals_and_gubbins, [], execs, []))) 

                    let fu_instances =
                        if externally_instantiatedf then []
                        else
                            let minfo = hpr_minfo(valOf_or_fail "L1475" (snd fu_definition))
                            [ (ii, Some(HPR_VM2(minfo, contacts, [], [], []))) ]


                    (actuals_decls @ nets9, fu_instances @ sons9, glue_bev @ rtl9, contact_maps9.Add(iname, (contacts, phys_contacts, portmeta, Some str2))) // end generic_external clause

                | SR_fu(kkey, auxix, SR_alu alu, _) -> // Rez Instance site
                    let externally_instantiatedf = false // Hardcoded inside but could easily be external if the need arises.
                    let glue_bev = []
                    let kind_vlnv = alu.kind
                    let (portmeta_s, actuals_rides, fu_definition, formal_contact_list, fu_meld) = gec_cv_alu_ip_block ww alu.oo kind_vlnv alu.latency alu.precision (alu.variable_latencyf)
                    let actuals_rides = if false then sf "old: map rides2fun alu.actuals_rides_" else actuals_rides
                    let (actual_contact_list, phys_contacts) = // This is the actual actual nets, not their schema.
                        let flip = if externally_instantiatedf then Some true else None// Declare the actuals (aka tappets) as locals if internally-instantiated, or flipped formals if external (up and above).
                        let actual_gen nps (cc, cd) =
                            let nets = buildportnet ww vd flip kkey ikey portmeta_s "" "" (Some alu.precision) nps []
                            if nullp nets then (cc, cd) else (hd nets :: cc, (hd nets, nps)::cd)
                        let mgr_rez (mgr_meld:mgr_meld_abstraction_t) (cc, cd) =
                            List.foldBack actual_gen mgr_meld.contact_schema (cc, cd)
                        List.foldBack actual_gen fu_meld.contact_schema (List.foldBack mgr_rez fu_meld.mgr_melds ([], []))
                    //dev_println (sprintf "ALU rez : net schema formal is "  + sfold (fun (f,a)->f + "/ " + xToStr a) formal_contact_list)
                    //dev_println (sprintf "ALU rez actual contacts_all are ^%i " (length actual_contact_list) + sfold (fun (f,a)->f + "/ " + xToStr a) actual_contact_list)
                    //dev_println (sprintf "ALU rez phys contacts_all are ^%i " (length phys_contacts) + sfold (fun ((f,a), nps)->f + "/ " + netToStr a) phys_contacts)                        

                    // For an AFU we render a definition whereas for CV library parts, like an ALU, FP convertor or BRAM, we do not because the library has them.
                    //map (fun xx -> (xx, xx)) (alu.variable_latency @ alu.alunets) // for now
                    let (actuals_decls, contacts) = declsrez_actuals settings.stagename (hptos alu.kind.kind) ctrl "primary" externally_instantiatedf (fu_meld.formals_rides, actuals_rides, formal_contact_list, actual_contact_list, [])

                    let ii = rez_opath1_ii settings.stagename false iname kind_vlnv
                    let fu_instance =
                        if externally_instantiatedf then []
                        else
                            let minfo = hpr_minfo(valOf_or_fail "L1475" (snd fu_definition))
                            [ (ii, Some(HPR_VM2(minfo, contacts, [], [], []))) ]
                    let cd = if ii.externally_provided then [] else [ fu_definition ]
                    (actuals_decls @ nets9, fu_instance @ cd @ sons9, glue_bev @ rtl9, contact_maps9.Add(iname, (contacts, phys_contacts, portmeta_s, Some str2))) // end ALU clause

                | SR_fu(_, auxix, SR_ssram(base_kind, ff, tech, prec, (w, l), _, romf, atts), _) when offchipp tech -> // offchip RAM needs no on-chip structural resource.
                    // An offchip RAM will use the contacts of a bondout port and these are already added.
                    dev_println "Contact mapp not augmented for offchip RAM"
                    (nets9, sons9, rtl9, contact_maps9)

                | SR_fu(_, auxix, SR_ssram(base_kind, ff, tech, prec, (w, l), portref_, romf, atts), _) -> // Onchip SSRAM is always fully pipelined but offchip HFAST can be a source of stalls
                // ROMs tend to get mirrored whereas RAMs can get multi-ported.  We do not mirror RAMs at the moment (which would mean broadcasting writes).
                // OLD Note: ROMs were rendered in the output of repack as straightforward X_bnet form vector with an initialisation list (not as structural components).
                // Restructure generates the structural instance and perhaps mirrors it as needed. System Integrator will also mirror ROMs or else share them with arbitration logic.
                    let delv = at_assoc "synch" atts
                    let rd_latency =
                        if delv=None || delv=Some "0" then 0
                        elif delv=Some "1" then 1
                        elif delv=Some "2" then 2                    
                        else sf ("synch ssram (ad) unsupported pipeline delay:" + valOf delv)

                    let externally_instantiatedf = false // RAMs and ROMs are always like this for now.
                    let threads = bounders.global_resource_thread_use.lookup ikey
                    let no_ports = length threads
                    if no_ports > 1 then vprintln 3 (sprintf "Memory instance %s is shared between (or operated on by) %i threads (prior to mirroring). Threads=%s" ikey (length threads) (sfold id threads))
                    let kkey =
                        match no_ports with
                            | 1 -> if romf then base_kind else "CV_SP_" + base_kind // SP denotes 'single-ported'
                            | n when n > 1 -> sprintf "CV_%iP_" n + base_kind                    
                            | n -> cleanexit(sprintf "Cannot find an SROM or SRAM with %i ports in technology library for ikey=%s." no_ports ikey)
                    cassert(no_ports>0, "ports>0")
                    let props = (valOf str2.propso)
                    let ram_data_array = X_bnet props
                    vprintln 3 (sprintf "Consider removal of RTL net from ROM/RAM %s    romf=%A" (xToStr ram_data_array) romf) 
                    let _ =
                        if not romf then // Only ROMs that are rendered inline in the current module (do not have "preserveinstance")  do not need their old net declaration removing. Currently all of our ROMs are of that nature, so we do not need to check that attribute.
                            mutadd settings.m_old_nets_for_removal ram_data_array
                    let iname = ikey
                    let descr = sprintf "Resource=%s iname=" (if romf then "SROM" else "SRAM") + iname + " " + i2s64 l + "x" + i2s w + " clk=" + edgeToStr (fst clkp) + " synchronous/pipeline=" + osToStr delv + sprintf " no_ports=%i " no_ports + sfold atToStr (lookup_net2 (valOf str2.propso).n).ats + " used by threads " + sfold (fun x->x) threads
                    let rom_id = if romf then Some ff.id else None
                    let aw = bound_log2 (BigInteger(l-1L))
                    let portmeta_s =
                        let lw = w 
                        let digger ss =
                            match ss with
                                | "DATA_WIDTH" ->  i2s w
                                | "ADDR_WIDTH" ->  i2s aw
                                | "WORDS"      ->  i2s64 l
                                | "LANE_WIDTH" ->  i2s lw // FOR NOW ONLY ONE LANE ALWAYS - NOT GOOD FOR SOME DESIGNS WHETHER OFF OR ON CHIP - TODO
                                | "trace_me"   ->  "0"
                                | _            ->  "0"
                        let schema = g_cv_sram_rides_schema
                        List.zip schema (map digger schema)

                    let atts = [ Nap("signed", sprintf "%A" prec.signed); Nap("ports", i2s no_ports)] @ atts
                    //dev_println (sprintf "props.constval=%A" props.constval)
                    let (kind_vlnv, portmeta_h, actuals_rides, fu_definition, formal_contact_list, meld) = gec_cv_romram_ip_block ww settings.stagename kkey descr no_ports portmeta_s prec.signed props.constval rom_id rd_latency props
                    let (actual_contact_list, phys_contacts) = // This is the actual actual nets, not their schema.
                        let flip = if externally_instantiatedf then Some true else None// Declare the actuals (aka tappets) as locals if internally-instantiated, or flipped formals if external (up and above).
                        let actual_gen pi_name nps (cc, cd) =
                            let nets = buildportnet ww vd flip kkey ikey portmeta_s pi_name "" (Some prec) nps []
                            if nullp nets then (cc, cd) else (hd nets :: cc, (hd nets, nps)::cd)
                        let mgr_rez (mgr_meld:mgr_meld_abstraction_t) (cc, cd) =
                            List.foldBack (actual_gen mgr_meld.mgr_name) mgr_meld.contact_schema (cc, cd)
                        List.foldBack (actual_gen "") meld.contact_schema (List.foldBack mgr_rez meld.mgr_melds ([], []))
#if OLD
                    let actual_contact_list =
                        let flip = if externally_instantiatedf then Some true else None// Declare the actuals (aka tappets) as locals if internally-instantiated, or flipped formals if external (up and above).
                        let mgr_rez (mgr_meld:mgr_meld_abstraction_t) cc =
                            List.foldBack (buildportnet ww vd flip kkey ikey portmeta_s mgr_meld.mgr_name "" (Some prec)) mgr_meld.contact_schema cc
                        List.foldBack (buildportnet ww vd flip kkey ikey portmeta_s "" "" (Some prec)) meld.contact_schema (List.foldBack mgr_rez meld.mgr_melds [])
#endif
                    //vprintln 0 (sprintf "SSRAM rez contacts_all are ^%i " (length contacts_all) + sfold (fun (f,a)->f + "/ " + xToStr a) formal_contact_list)
                    let externally_provided = not romf // Rather simplistic for going forward! But works for now.
                    let def_needed = not externally_provided
                    
                    let ii = rez_opath1_ii settings.stagename false ikey kind_vlnv
                    let ii = // If we clear  ii.preserve_instance, we can render ROMs inline, not as a separate module in the vnl file (they are custom on a per-design basis).
                             // But rendering them as modules is nice since it makes structural hazards more visible.
                            if romf then
                                 { ii with 
                                       externally_provided= externally_provided
                                       in_same_file=        true  //Not relevant for instance 
                                       generated_by=        settings.stagename
                                 }
                            else ii
                        // Render ROMs inline, not as a separate module in the vnl file since contact directions are not sorted out yet. Also supress generic rides (parameters) for ROMs.

                    let _: hpr_machine_t option = snd fu_definition
                    let fu_definitions =
                        if def_needed then
                            let fu_definition_already_provided = scan_for_existing_fu_definition (snd fu_definition) sons9
                            if fu_definition_already_provided then
                                vprintln 2 (sprintf "Not providing a further definition of identical custom FU " + (vlnvToStr (fst fu_definition).vlnv))
                                []
                            else [ fu_definition ]
                        else []

                    // Now create an instance of RAM or ROM.
                    // TODO - explain the subtle differrence between an externally-instantiated on-chip RAM and a bondout memory.
                    vprintln 3 (sprintf "ROM/RAM rez: romf=%A  base_kind=%s  full_kind=%s  ikey=%s " romf base_kind (vlnvToStr kind_vlnv) ikey)

                    let (actuals_decls, contacts) = declsrez_actuals settings.stagename (hptos kind_vlnv.kind) ctrl "definition" externally_instantiatedf (meld.formals_rides, portmeta_h, formal_contact_list, actual_contact_list, [])  

                    let fu_instances =
                        if externally_instantiatedf then [] // For external instances, we should still emit the component for diosim to use, but not an instance of it here. System integrator should create a system for diosim to simulate ...  DIOSIM can collect its own RAM models from cv_auto_ipgen but not ROMs 
                        else
                            let minfo = hpr_minfo(valOf_or_fail "L1475" (snd fu_definition))
                            [ (ii, Some(HPR_VM2( { minfo with atts=atts }, contacts, [], [], [])))]
                    //dev_println (sprintf "Adding contact map for " + iname)
                    (actuals_decls @ nets9, fu_definitions @ fu_instances @ sons9, rtl9, contact_maps9.Add(iname, (contacts, phys_contacts, portmeta_s, Some str2))) // end ROM/RAM clause (onchip)

                | SR_pliwork ->  // No FU resource to render for fabbed PLI call.
                    (nets9, sons9, rtl9, contact_maps9)

                | SR_filler ->
                    hpr_yikes("camops filler used")
                    (nets9, sons9, rtl9, contact_maps9) // Should not really be used.
                    
                | other     ->  sf ("L1552 other structural resource.")

    if memberp str2.ikey static_manifest_instance_names then
        vprintln 2 (sprintf "Not rendering iname=%s, kind=%s, since this instance is a member of the static manifest" str2.ikey str2.kkey)
        (nets9, sons9, rtl9, contact_maps9)
    else
        List.fold rez_gec_instance1 (nets9, sons9, rtl9, contact_maps9) [str2] // End of res2_render_resource_instances




let is_fp_convert (prec:precision_t) = prec.signed = FloatingPoint

// Now unused. Neat ep into compile_hbev for HSIMPLE transactor calls and so on. 
let friendly_hbev_wrapper__ ww cmds =
    let ww = WN "friendly_hbev_wrapper" ww
    let b  = g_unbound
    let c  = g_unbound
    let r  = (g_unbound, ref X_undef)
    let gG  = new_gdic (funique "iii")  "FHB" 
    let _ = compile_hbev ww gG g_null_an (b, c, r) (gec_Xblock cmds)
    let ans1 = gec_Xblock(dic_flatten ww gG.id !gG.DIC)
    //let id = "temp_minfo"
    //let temp_minfo = { name= [ id ]; atts=[]; } 
    //let (_, _, ans1) = compile_hbev_to ww (None) id temp_minfo (SP_l(gec_Xblock cmds)) 
    //vprintln 0 ("Restr: gave = " + hprSPSummaryToStr ans1)
    //vprintln 0 ("Restr: gave = " + hbevToStr ans1)
    ans1


//
// mostly old code - offchip only.
//
(*
 * Restr1_t is an extendable alist indexed by array or entity name and used by restructure.fs.
 *
 * Each entry in restructurings is a further extendable alist, but indexed by 
 * a pair that is the subscript and optional int that indicates rwbar read/write. why int not bool?
 *
 *)
type str_component_t = // older code - please delete me
    { 
       porto_:      bondout_port_t option   // aka ikey? really?
       item_:       str_form_t              // RAM, ALU or other ...
       props_:      net_att_t option        // base net (for an inferred RAM)
       ops_:        rest_op_t list ref      // 
     }

 
type restr1_t =
      string            // Structural resource assoc tag
    * str_component_t
//type rwstr_t =  (hexp_t option(*rwbar*) * restr_t list ref)

let res1_collate (resops:restr1_t list, pc_states: pc_statemap_t) =
    let flatten c (srat, foo) = List.fold (fun c kv -> (srat, foo, kv)::c) c (!foo.ops_)
    ()
    let flattened =  (List.fold flatten [] resops)
    let all_pp_lst = List.fold (fun c (srat, foo, (k, v)) -> singly_add (k.pp) c) [] flattened
    let vd = 3
    // now collate pp by thread
    let thrd_scan  c = function
        | Some(pc, state) -> singly_add pc c // repeated work: should be in pc_statemap.
        | None -> c
    let threads = List.fold thrd_scan [] all_pp_lst
    vprintln 3 (i2s (length threads) + " threads found for schedulling: " + sfold xToStr threads)

    let all_ikeys = List.fold (fun c (ikey, foo, (k, v)) -> singly_add ikey c) [] flattened
    vprintln 3 (i2s (length all_ikeys) + " structural resources found for schedulling: " + sfold (fun x->x) all_ikeys)

    // Need to see which structural resources are shared over more than one thread (they will need compiletime or runtime arbiters):
    let count_threads ikey =
        let zf = function
            | None -> gec_X_net "$noscheduler"
            | Some(pc, state) -> pc
        let users = List.fold (fun c (ikey', foo, (k, v)) -> if ikey=ikey' then singly_add (zf k.pp) c else c) [] flattened
        (ikey, users)
    let use_counts = map count_threads all_ikeys 
    reportx 3 "Old use counts (which threads use which resources)" (fun (id, threads) -> id +  ": " + sfold xToStr threads) use_counts
    

    let shed_thread thread =
        let pp_flt = function
            | Some(pc, pp) -> pc=thread // nb: can also use pc_states structure.
            | None -> false
        let pp_lst = List.filter pp_flt all_pp_lst
        let tables = muddy "map (shed thread) pp_lst" // collate on pp
        (thread, tables)
        
    let tables = map shed_thread threads // collate on pp
    (resops, tables)
    

// Determine what form of implementation technology to use for an array.    
let array_reprocess2_pred (settings:restruct_manager2_t) (ff:net_att_t) =
    let f2 = lookup_net2 ff.n
    let len = if not ff.is_array then -1L else hd f2.length
    let onchip_flag = at_assoc "onchip" f2.ats = Some "true"
    let aportno =
        match at_assoc "portname" f2.ats with // TODO aportno is ingored
            | None -> -1
            | Some v -> atoi32 v
    let romflag = not_nullp ff.constval && ff.is_array
    let rom_id = if romflag then Some ff.id else None
    let synch_sram_mark = at_assoc "SynchSRAM" f2.ats
    let latency = if not_nonep synch_sram_mark then atoi32(valOf synch_sram_mark) else 1 // BLOCK RAM latency in cycles
    let auxix_portless = // also for distributed RAM
        {
          g_default_auxix with
            rei_interval=1          // Reissue delay before next command issued (1 for fully pipelined)
            result_latency=0  
            raises_data_hazards=true
        }
    let auxix_rom =
        {
          g_default_auxix with
            rei_interval=1          // Reissue delay before next command issued (1 for fully pipelined)
            result_latency=latency  // Result delay: aka rd_latency, fixed latency or average prediction if variable
        }
    let auxix = { auxix_rom with
                     raises_data_hazards=true
                     scalar_rtl_hazard_semantic=  false  // TODO set this field, but need interlock on subscript match.  
                }

    let ans = 
      match settings.stagename with
        | "restructure2" -> 
            let f2 = lookup_net2 ff.n
            let bondout_banko = at_assoc g_named_bondout_bank_pseudo f2.ats
            if len = g_unspecified_array && nonep bondout_banko then sf (sprintf "cannot restructure: (perhaps repack missed this one?) unspecified length for array %s" (netToStr (X_bnet ff))) // Heap space can have unspecified length
            //let threads_per_port (ff:net_att_t) = at_assoc "ThreadsPerPort" ff.ats <> None
            //let ports_per_thread (ff:net_att_t) = at_assoc "PortsPerThread" ff.ats <> None
            //let multiport = not_nonep(at_assoc "MultiPortRegfile" ff.ats)
            //vprintln 3 (sprintf "multiport=%A portno=%i" multiport)
            let ans = 
                if not_nonep synch_sram_mark then T_Onchip_SRAM(auxix, rom_id) // latency in cycles
                elif not onchip_flag && not_nonep bondout_banko then T_Offchip_RAM(auxix, valOf bondout_banko)
                elif not onchip_flag && not_nonep (at_assoc "offchip" f2.ats) then T_Offchip_RAM(auxix, valOf(at_assoc "offchip" f2.ats)) // Not used anymore?
                elif not onchip_flag && len >= settings.offchip_threshold then
                    let spacename = "static-segment"
                    T_Offchip_RAM(auxix, spacename)
                     // This does not distinguish between combram and regfile currently. The FPGA tools do this automatically.
                elif romflag then
                    if len >= settings.combrom_threshold then T_Onchip_SRAM(auxix_rom, rom_id)
                    else T_PortlessRegFile auxix_rom
                elif len >= settings.regfile_threshold then T_Onchip_SRAM(auxix, None)
                else T_PortlessRegFile auxix_portless
            ans
        | othername -> sf (sprintf "bad stage name %s" othername)

    vprintln 3 (sprintf "Reprocess decision for array for %s of len %s (ROM=%A) is tech=%s " ff.id (array_lenfun len) romflag (memtechToStr ans))
    ans


let g_bondout_ram_del_rei = (1, 1)  // Use latency of HFAST1 for now - TODO allow to set more realistically ?


// Find by name the method details and accompanying method group.
// Overload by arity is intrinsic to squirrel sq_o form.
let find_method_str2 msg cmd_name lst sq_o =
    //As a legacy/kludge, we support two mgkey naming conventions.
    let mgkey1 = cmd_name + (if nonep sq_o then "" else valOf sq_o)     
    let mgkey0 = (if nonep sq_o then "" else valOf sq_o)    

    let possibilities () =
        let print_mgr_pos_methods cc mgr =
            let print_method_name cc (arg:method_str2_t) =
                let ss =
                    if mgr.pi_name = "" then arg.fname 
                    else arg.fname  + "(mgr.pi_name=" + mgr.pi_name + ")"
                //(ss + sprintf "arity %i"  (length arg.method_meld args)) :: cc 
                //let ss = ss + sprintf "prec2=%A" arg.precision2
                ss :: cc
            List.fold print_method_name cc mgr.methodgroup_methods

        List.fold print_mgr_pos_methods [] lst
    let rec search mgkey  = function
        | [] ->
            None
        | mgr::tt when nonep sq_o || mgkey = mgr.pi_name || mgr.pi_name = valOf sq_o -> // We should only need one of these final two terms.
            let rec find_method2 = function
                | [] -> search mgkey tt
                | method_str2::tt when method_str2.fname = cmd_name && (nonep sq_o || sq_o = method_str2.overload_suffix || mgr.pi_name=mgkey)->
                    Some (method_str2, mgr)
                | _::tt -> find_method2 tt
            find_method2 mgr.methodgroup_methods
        | _::tt -> search mgkey tt


    let a0 = search mgkey0 lst
    if not_nonep a0 then valOf a0
    else
        let a1 = search mgkey1 lst
        if not_nonep a1 then valOf a1
        else
            let error_gen mgkey = 
                let portmsg = if nonep sq_o then "<BLANK MGR NAME>" else sprintf "Port number/name of '%s' and mgkey=%s" (valOf sq_o) mgkey
            //let kind = (valOf !mgr.mgro).dinstance.ikey
                sprintf " :FU has no method '%s' %s in component '%s'. Searching %s." cmd_name (if nonep sq_o then "without a pi/mgr name" else "with sq=" +  valOf sq_o + " or mgkey=" + mgkey) msg portmsg 

            let e0 = error_gen mgkey0
            let e1 = error_gen mgkey1            
            let ma = sfold id (possibilities())
            cleanexit(msg +  e0 + e1 + " available methods are " + ma)




//
// Select appropriate memory technology for an array.  This decide4 needs be reftran, so no call to funique or similar in it please.
//
// Map phase then bind phase.   
let memory_decide4 (settings:restruct_manager2_t) bindpasso (ff:net_att_t) =
    let yn x = if x then "yes" else "no"
    let (found, ov) = settings.memtech_mapping.TryGetValue ff.n // Will return same one each time
    if found then
        //vprintln 3 (sprintf "decide4: (RexRex) %s using existing mkey decision." ff.id)
        (true, ov)
    elif not ff.is_array then
        sf (sprintf "decide4: no length to RAM %s" ff.id)
    else
        let prec = { widtho=Some(netwidth ff); signed=ff.signed; }
        let tech = array_reprocess2_pred settings ff
        let ans = 
            let mkey = ff.id
            let f2 = lookup_net2 ff.n
            let len = hd f2.length
            let wid = encoding_width (X_bnet ff)        
            let ans =
                if unaltered_pred tech then None
                else
                match tech with
                    | T_Comb_RAM        
                    | T_PortlessRegFile _ -> sf "unreachable at the moment - use rd_latency=0"

                    | T_Onchip_SRAM(auxix, None) ->  // fully-pipelined is implied
                        let fp = true
                        let vl = false
                        let kind = sprintf "SSRAM_FL%i" auxix.result_latency  // the CV_SP or CV_DP prefix is added latter according to need at a later refinement. Fixed-latency is 0, 1 or 2 for read. Fully pipelined.
                        let sr = SR_fu(mkey, auxix, SR_ssram(kind, ff, tech, prec, (wid, len),  0, false, [Nap("synch", i2s auxix.result_latency); Nap("fully-pipelined", yn fp); Nap("variable-latency", yn vl)]), None)
                        Some(mkey, kind, sr, false)

                    | T_Onchip_SRAM(auxix, Some suffix) ->  // ROM: fully pipelined when auxix.rei=1 which it normally is
                        let fp = true
                        let vl = false // For a ROM we need the unique contents to be part of its kind, so insert ff.id here
                        //let kind = sprintf "SROM_%s_FL%i" ff.id auxix.result_latency  // ROM latency is normally 1, fully-pipelined.
                        let block_meld = gen_cv_ssrom_port_structure settings.roms_have_ren auxix.result_latency suffix
                        let kind = block_meld.kinds
                        let sr = SR_fu(mkey, auxix, SR_ssram(kind, ff, tech, prec, (wid, len), 0, true, [Nap("synch", i2s auxix.result_latency); Nap("fully-pipelined", yn fp); Nap("variable-latency", yn vl)]), None)
                        Some(mkey, kind, sr, false)

                    | T_Offchip_RAM(auxix, spacename) -> 
                        let vl = true
                        // BVCI/HFAST0 (0 denotes infinity in terms of max outstanding) is variable latency but also is pipelined - you might say it is not fully-pipelined, but it can stream commands as long as order is respected. 
                        let (rd_latency, sr) = g_bondout_ram_del_rei  // Use latency of HFAST1 for now

                        // You might thing we should return an SR_generic here, but a mapping and packing adapator sits between the RAM model and the bondout port.
                        let sr = SR_fu(mkey, auxix, SR_ssram("*offchipkind*", ff, tech, prec, (wid, len), 0, false, [Nap("synch", i2s rd_latency);  Nap("variable-latency", yn vl)]), None)
                        Some(mkey, "*anon*", sr, true)
                    //| other -> sf (sprintf "other form of array tech %A" other)
            ans
        settings.memtech_mapping.Add(ff.n, (tech, ans)) // oddly use ff.n instead of ff.id as mkey.
        vprintln 3 (sprintf "decide4: Mapping phase: made technology decision for %s" ff.id)
        (false, (tech, ans))

let score_clone score =
    //vprintln 0 (sprintf "score_clone cloning %A" !score.scboard)
    {
        scboard=    ref !score.scboard
        live_hrs=   ref !score.live_hrs
    }


type minor_shed_t =
    {
      rd_shed:  newop_t list ref;        // Schedule per major state
      wr_shed:  newop_t list ref;        // Ditto writes
    }

// Housekeeping pair to string.
let hkpToStr (eno, hk) = i2s eno + ":" + hkToStr "" hk


let g_sox_serial = ref 100
let fresh_opx_serial() =
    let vv = !g_sox_serial
    g_sox_serial := vv + 1
    vv

// Combine two uses of an operation (on the same FU and mgr). Please explain uses? E.g. writes to the same scalar or reads from the same RAM location.
// The uses may be within the same schedule or installed in the root/control schedule by one edge and used in the data schedule of another edge from the same resume. 
let opx_merge settings mgkey (a0:newopx_t) ppx_startt (a1:newopx_t) =
    let vd = settings.vd
    let gs = ix_or (fst a0.args_raw) (fst a1.args_raw)
    let rhs =
        //let fc (ll, rr) = ix_query a0.action_grd ll rr
        let fc (ll, rr) = ix_query (fst a0.args_raw) ll rr        
        map fc (List.zip (snd a0.args_raw) (snd a1.args_raw))
    let _ =
        let rx = function
            | None -> "None"
            | Some(g, alst) -> sfold xToStr alst
        if vd>=4 then vprintln 4 (sprintf "opx_merge mgkey=%s: Merge OX%i and OX%i:  %s \\/  %s ==> %s.  Args=%s and %s. Args_bo=%s and %s    Commands=%s and %s" mgkey a0.serial a1.serial (xbToStr (fst a0.args_raw)) (xbToStr (fst a1.args_raw)) (xbToStr gs) (sfold xToStr (snd a0.args_raw)) (sfold xToStr (snd a1.args_raw)) (rx !a0.g_args_bo) (rx !a1.g_args_bo) a0.cmd a1.cmd)
    if vd>=5 then
        vprintln 5 (sprintf "opx_merge mgkey=%s: Merge hexp_t orig=%s  and  %s" mgkey (xToStr a0.oldnx) (xToStr a1.oldnx))
        vprintln 5 (sprintf "opx_merge mgkey=%s: Merge nn orig=%i and %i" mgkey (x2nn a0.oldnx) (x2nn a1.oldnx))
        vprintln 5 (sprintf "opx_merge mgkey=%s: Customer edges are %s  and  %s" mgkey (sfold i2s a0.customer_edges) (sfold i2s a1.customer_edges))
        vprintln 5 (sprintf "opx_merge mgkey=%s: Housekeeping sets were %s  and  %s" mgkey (sfold hkpToStr a0.housekeeping) (sfold hkpToStr a1.housekeeping))

    //cassert (a0.method_o=a1.method_o, "a0.s_mgr=a1.s_mgr") // causes segfault on mono.
    cassert (a0.cmd=a1.cmd, "a0.cmd=a1.cmd")
    cassert (a0.sq=a1.sq, "a0.sq=a1.sq")        

    let nodef_for_opx_merge = function
        | (eno, HK_prerel(pcv, tag)) ->
            //dev_println (sprintf "opx_merge: parasite hk %i: %i" eno (pcv+ppx_startt))
            (eno, HK_rel(pcv+ppx_startt, tag))
        | _ -> sf "L1839"
        
    let ans =
        { a0 with // This is not a commutative merge operator - the main aspects from a0 are used with only a few features of a1 added to it.
             //action_grd=gs
             args_raw=  (gs, rhs)
             housekeeping= lst_union a0.housekeeping  (map nodef_for_opx_merge a1.housekeeping) // lst_union must maintain order of both lists so we can continue to use simple lookup
             unaltered= a0.unaltered @ a1.unaltered          
             customer_edges= lst_union a0.customer_edges a1.customer_edges
             postf=     a0.postf && a1.postf;
             serial=    fresh_opx_serial() // a0.serial + "-and-" + a1.serial
        }
    //dev_println ("opx merge completed")
    ans

    
let report_work_summary m0 work =
    let opxSum newopx = newopx.cmd  + ":" +  xToStr newopx.oldnx
    let w2s (eno, lst) =
        let w22s (pcv_track, te, opx) = sprintf "track=%A eno=E%i, %s, opx=%s, hk at=%s"  pcv_track eno (shenToStr te) (opxSum opx) (sfold (fun (eno, hk) -> i2s eno + ":" + hkToStr "" hk) opx.housekeeping)
        map w22s lst
    reportx 3 (m0 + ": Work summary") (fun x->x)  (list_flatten(map w2s work))
    ()


let vl_max_times2 writeshed times2 =
    match times2 with
        | [] ->
            let hkeep0 = { startarcf=false; writeshed=writeshed; baseidx=0; } // A total dummy but this code route is not used...
            // "need origin start_times2"
            (hkeep0, X_true, writeshed, 0)

        | (hkeep0_r, vr0, sd0, HK_rel(tr0, _))::tt ->
            let vl_max (vl, tl) = function
               | (hkeep0_, vr, sdr, HK_rel(tr, _)) ->
                    if sd0<>sdr then sf (sprintf "vl_max: different schedules: srl=%A sdr=%A" sd0 sdr)
                    if tl=tr then (ix_and vl vr, tl) elif tl>tr then (vl, tl) else (vr, tr) // idiom
               | _ -> sf "L2484 other"
            let (vl, tl) = List.fold vl_max (vr0, tr0) tt
            (hkeep0_r, vl, sd0, tl)

        | _ -> sf "L2491 other"
                                          
let vl_max_i (startarcf, vl, sdl, tl) (startarcf_r, vr, sdr, tr) =
    if sdl<>sdr then sf "vl_max_i: different schedules"
    if startarcf<>startarcf_r  then sf "vl_max_i: different startarc_f"
    if (tl=tr) then (startarcf, ix_and vl vr, sdl, tl) elif tl>tr then (startarcf, vl, sdl, tl) else (startarcf, vr, sdl, tr) // idiom

let vl_inc (startarcf, vl, ss, tl) d = if d=0 then (startarcf, vl, ss, tl) else (startarcf, X_true, ss, tl+d) // idiom        

// Non-associates flattening node store.
type diadic_t =
    | DD_L of (hexp_t * starttime_t)
    | DD_C of bool * (hexp_t * starttime_t) * diadic_t
    | DD_P of bool * diadic_t * diadic_t    






// Remove polyadic form: Make associative operators degenerate to _balanced_ nest of diadic (binary) operand instances
// EG: We cannot compute (a * b * c * d) with diadic operators and we do not want a * (b * (c * d)) either since it take 50% longer than (a * b) * (c * d).
// The topf flag holds for the element at the top of the tree that will replace the original polyadic node.
let rec diadicise orig prec oo (rdy_zipped:(hexp_t * starttime_t) list) =
    let n_items = length rdy_zipped 
    if n_items > 4 then vprintln 3 (sprintf "diadicise/dido top length in is %i: " n_items) // + xToStr(W_node(prec, oo, lst, m)))
    let rec dido_unbal topf = function
        | [a;b] -> DD_C(topf, a, DD_L b)
        | h::t  -> DD_C(topf, h, dido_unbal false t)
        | other -> sf(sprintf "diadicise: dido too short (%i items) in %s old=" (length other) (sfold (fst>>xToStr) other)  + xToStr orig)

    let rec dido_unbal topf = function
        | [a;b] -> DD_C(topf, a, DD_L b)
        | h::t  -> DD_C(topf, h, dido_unbal false t)
        | other -> sf(sprintf "diadicise: dido too short (%i items) in %s old=" (length other) (sfold (fst>>xToStr) other)  + xToStr orig)

    let rec dido_bal topf len lst =
        if len >= 4 then
            let (aa, bb) = list_split (len/2) lst
            DD_P(topf, dido_balm false aa, dido_balm false bb) 
        else dido_unbal topf lst
    and dido_balm topf lst = dido_bal topf (length lst) lst

    if false then // Can make an oracle decision please.
        dev_println ("Please use dido balanced instead.")
        dido_unbal true rdy_zipped
    else dido_balm true rdy_zipped


let de_rel_hk = function
    | (startarcf_, _, _, HK_rel(start, _)) -> start
    | other -> sf "de_rel_hk: cannot start"


type rel_stall_corrector_dict_t = Dictionary<int, hexp_t>

let deton arg cc =
    match arg with
        | TIMEOF_NODE(startarcf, grd, ssf, reltime) -> (startarcf, grd, ssf, reltime)::cc
        | _ ->
            //dev_println ("other support_times2 ignored")
            cc



// We need to build out in the time domain the control and data inputs to an FU so they are all present together at the opstart clock cycle.
// Values from outputs-held FUs that have not been re-used do not need buildout and ditto for many scalar registers. 
// We insert X_x pipeline stages, and a later processing step will only render those that are needed owing to the pipeline input being non-held.
// There are no stall nets considered here, since this is invoked during static schedulling which is based on expected completion times with stall logic being post inserted.            
// The vl_max operator has already been computed to give us pos0 (but we could check for violation).

let buildout settings serial_ thread_fsems (gs, g_ready_at) (hkeep0, gig_, writef, pos0) lst =

    let startarcf = hkeep0.startarcf

    let divert_inholder hexp =
        if thread_fsems.fs_inhold then hexp // If fs_inhold is specified, the caller will maintain the input values throughout the operation, but the default is that the input data is likely to be removed straight after the input handshake (fs_inhold=false).
        else
            match hexp with
                | X_bnet ff -> ()
                | _ -> sf (sprintf "res2: inholder mechanism intended only for input nets, not %s" (netToStr hexp))
            let res_key = xToStr hexp
            match settings.scalar_hr_db.lookup res_key with
                | None ->
                    let clone = clonenet "" "_argread" hexp
                    //dev_println (sprintf "ROSIE buildout: rez argreg net %s for %s" (netToStr clone) (xToStr hexp))
                    let rr = HR_argreg(clone, hexp)
                    settings.scalar_hr_db.add res_key (clone, rr)
                    mutaddonce settings.m_morenets clone
                    clone
                | Some(clone, hr) -> clone
    if writef (*  not settings.pipelining *) then
        let gs2 = // buildout of the action_grd
            if true then gs
            else
                // just do the guard for now ... ... work in progress...
                let delta = (g_ready_at-pos0-hkeep0.baseidx) // Should be zero or -ve.
                let gs2 = xi_orred(xi_X delta (xi_blift gs))  // dont need for constants...
                vprintln 3 (sprintf "buildout serial=OX%i delta=%i: buildout of control guard from %s to %s" serial_ delta (xbToStr gs) (xbToStr gs2))
                dev_println(sprintf "URGENT OX%i writef no FORCE buildout %s  hkeep pos0=%i offset=%i" serial_ (xbToStr gs2) pos0 hkeep0.baseidx)
                gs2
        (gs2, map fst lst) 
    else
        let rbo (hexp, ready_at) =
            if constantp hexp then hexp // Constant expressions can safely skip being pipelined always.
            else
                let inputf =
                    match hexp with
                        | X_bnet ff ->
                            let f2 = lookup_net2 ff.n
                            isio f2.xnet_io 
                        | _ -> false
                if inputf then
                    vprintln 3 (sprintf "buildout: argreg consider %s startarcf=%A at time %A thread_fsems.inhold=%A" (xToStr hexp) startarcf (gig_, writef, pos0) thread_fsems.fs_inhold)
                    if (pos0=0 && startarcf) then hexp  else divert_inholder hexp
                else
                match f4o4 ready_at with
                    | HK_rel(nn, tag) ->
                        // pos0 will be later than nn in causal situations.  The arg to xi_X is -ve to delay its argument that number of cycles.
                        let delta = (nn-pos0) // Should be zero or -ve.
                        if delta > 0 then sf(sprintf "non-causal buildout. writef=%A   hexp=%s from %A to %A.  delta=%i" writef (xToStr hexp) ready_at pos0 delta) 
                        if delta < 0 then
                            vprintln 3 (sprintf "buildout hexp %s from %A to %A.  delta=%i" (xToStr hexp) ready_at pos0 delta)
                        xi_X delta hexp
                    | other -> sf(sprintf "Build out other ready time %A" other)

        let gs2 = // buildout of the action_grd
            let delta = (g_ready_at-pos0) // Should be zero or -ve.
            let gs2 = xi_orred(xi_X delta (xi_blift gs))  // dont need for constants...
            vprintln 3 (sprintf "buildout serial=OX%i delta=%i: buildout of control guard from %s to %s" serial_ delta (xbToStr gs) (xbToStr gs2))
            gs2
        (gs2, map rbo lst)


let op3ToStr (eno_, shen, opxx:newopx_t) = 
    let v = if nonep opxx.method_o then "-" else (valOf !(valOf opxx.method_o).m_mgro).mg_uid
    //let spare = " te=" + shenToStr shen + " "
    v + opxx.cmd + "(" + sfold xToE (snd opxx.args_raw) + ")"



let g_scalar_auxix =
                  { g_default_auxix with
                      raises_data_hazards= true
                      scalar_rtl_hazard_semantic=true
                      result_latency=    0
                      rei_interval=       1
                  }

// Move these two type definitions. And explain them.
type binding_private_info_t = (string * (string * str_instance_t * str_methodgroup_t list * fu_aux_info2_t option * (string * string) list)) list

type binding_mirrorable_info_t = ((string * int) * (string * str_instance_t * str_methodgroup_t list * fu_aux_info2_t option * (string * string) list)) list                      


let bindmapreport_kernel ww wr title tinfo =
    wr (sprintf "-------------------------------------------------------------------")
    wr (sprintf "Thread name           %s" (xToStr tinfo.thread))        
    wr (sprintf "     Director         %s" (dirToStr false tinfo.director))
    if not_nonep tinfo.director.performance_target then wr (sprintf "     Timespec         %s" (timespecToStr (valOf tinfo.director.performance_target))) 

    let concise_pred = function
        | SR_fu _ -> false
    let get_mkey = function
        | SR_fu(mkey, _, _, _)  -> mkey
    let report_resume resume  (mapping_needs:mapping_needs_t) =
        let details_for_report =
            let m_m = ref []
            let report_need kk (mkey_, resume, eno_o, sr, knnlst) =
                if concise_pred sr then ()
                else mutadd m_m (sprintf "root use %s knnlst=%s" (get_mkey sr) (sfold i2s knnlst))
            for vv in mapping_needs do report_need vv.Key vv.Value done
            !m_m
        if not_nullp details_for_report then wr (sprintf "       rex_resume  %s  FUs=" (xToStr resume)  + (sfold id details_for_report)) 

    let report_edge edge_no (mapping_needs:mapping_needs_t) =
        let details_for_report =
            let m_m = ref []
            let report_need kk (report_, resume, eno_o, sr, knnlst) =
                        if concise_pred sr then ()
                        else mutadd m_m (sprintf "edge use %s nnlst=%s" (get_mkey sr) (sfold i2s knnlst))
            for vv in mapping_needs do report_need vv.Key vv.Value done
            !m_m
        if not_nullp details_for_report then wr (sprintf "       rex_edge E%i  FUs=" edge_no + (sfold id details_for_report)) 
    for kv in tinfo.mapping_log_for_resume do report_resume kv.Key kv.Value done
    for kv in tinfo.mapping_log_for_edge do report_edge kv.Key kv.Value done            
    wr (sprintf "     Mapped FUs       %i" (length !tinfo. m_thread_fu_summary_report))
    wr (sprintf "     Mapped FU mkeys  %s" (sfold fst !tinfo. m_thread_fu_summary_report))
    ()

    
let bindmapreport_r33 ww title r33 =
    let wr ss = vprintln 1 ss
    wr title
    let fu_tad_report ((_, _, _, tinfo:perthread_t, arc_freqs_), _) = bindmapreport_kernel ww wr title tinfo
    app fu_tad_report r33
    ()
    
        
let rec find_binding ww (binding_private:binding_private_info_t, binding_mirrorable:binding_mirrorable_info_t) (mkey, resume, edge_o, sr, oldnn)  =
    let mirrorable = mirrorable_sr sr
    //vprintln 0 (sprintf "find_binding mkey=%s  resume=%s  edge_o=%A  qi=%i" mkey (xToStr resume) edge_o oldnn)
    let rr = if mirrorable then op_assoc (mkey, oldnn) binding_mirrorable else op_assoc mkey binding_private
    match rr  with
        | None ->
            let wr ss = vprintln 0 ss
            let title = "FU Thread and Domain Report - Bound FUs at error point" 
            //bindmapreport_kernel ww wr title bindpasso
            vprintln 0 (sprintf "The private mkeys bound when error encounted are " + sfoldcr_lite (fun (key, (_, str, _, _, _))-> "  private " + key + " "  + str.ikey) binding_private)
            vprintln 0 (sprintf "The mirrorable mkeys bound when error encounted are " + sfoldcr_lite (fun ((key, qi), (_, str, _, _, _))-> "  mirrorable " + key + " " + i2s qi + " " + str.ikey) binding_mirrorable)
            sf (sprintf "find_binding: FU Binding not found.  mkey=%s  nn=%i mirrorable=%A resume=%s  edge_o=%A " mkey oldnn mirrorable (xToStr resume) edge_o)
            // This should not happen. Kludge: Divert to no 0 (assuming there is at least one provided)
            find_binding ww (binding_private, binding_mirrorable) (mkey, resume, edge_o, sr, oldnn)
        | Some ov -> ov
    
// The function gen_walker_gen creates a schedule-forming walker that can be applied to various expressions within the edges. It is greedy
// so the order of application is important.  Different orders are used under different trial so planopt can find a good design.
// Entries are made in the schedule on the fly as we rewrite the XRTL.  The order of the execs will affect the outcome, as with all greedy algorithms.
// The greedy scheduller currently makes a poor decision in test10 where 3 multipliers are used, one twice, whereas they have a serial dependancy and it would be better to use one instance serially.
let gen_walker_gen ww0 (settings:restruct_manager2_t) bindpasso bounders (thread, perthread, domain) resume msg title oraclef m_fatal_marks =
    let vd = settings.vd
    let devp = vd >= 5
    //let tid_ = perthread.thread_str + ":" + xToStr state
    let mm = "minor_times: " + title
    let ww = WF 3 (mm) ww0 ("Start at Timestamp: " + timestamp true)

    let startarcf = (lc_atoi32 resume = 0)

    //match resume with
    //| other -> sf (sprintf "Need to find starting arc flag from %A" other)
    let phase = if nonep bindpasso then "MapScan" else "Schedule"

    let get_poolctr() =
        let ans = if nonep !perthread.edge_resources then valOf_or_fail "L2476" !perthread.maj_resources else valOf !perthread.edge_resources
        //if nonep ans then sf "get_poolctr NONE"
        ans

    let mappingledger_add_use mkey resume (oldx, str) =
        // OLD: Need to enter in ledger both on mapping explore and schedule, for consistency of occurence numbers. NEW: just use xidx_t nn.
        let ss = if phase="Schedule" then "Mapped FU use correlation in ledger" else "Mapping need being entered in ledger"
        let knn = x2nn oldx
        if vd>=4 then vprintln 4 (sprintf "%s: phase=%s, a use of FU mkey=%s  nn=%i" ss phase mkey knn)
        let bkey = 
            match get_poolctr().mapping_needs with
                | Some log ->
                    let edge_o = get_poolctr().edge_no_o
                    if vd>=4 then vprintln 4 (sprintf "%s: phase=%s, a use of FU mkey=%s  resume=%s edge_o=%A" ss phase mkey (xToStr resume) edge_o)
                    match log.lookup mkey with
                        | None ->
                            let use_idx = (mkey, resume, edge_o, str, [knn])
                            log.add mkey use_idx
                            (mkey, resume, edge_o, str, knn)//use_idx
                        | Some (mkey_, resume_, edge_o_, str_, oldxs) ->
                            let use_idx = (mkey, resume, edge_o, str, singly_add knn oldxs)
                            log.add mkey use_idx
                            (mkey, resume, edge_o, str, knn)//use_idx
                | None ->
                    sf (sprintf "No mapping_needs ledger in place to log use of %s" mkey)
        if vd>=4 then vprintln 4 (sprintf "Mapping need was entered in ledger: phase=%s, a use of FU mkey=%s. knn=%i" phase mkey knn)

        bkey

    let resource_add_use mkey (str, n) =

//      vprintln 2 (sprintf "Added, phase=%s, a use of FU mkey=%s  mgrkey=%s  instance.kkey=%s     use freq=%i" phase mkey str.mgkey str.dinstance.kkey n)
        vprintln 2 (sprintf "Added, phase=%s, a use of FU mkey=%s  mgrkey=%s  instance.kkey=%s     use count=%i" phase mkey str.mg_uid str.dinstance.kkey n)
        //get_poolctr().resource_pool.add mkey (str, n) // not needed?
        ()

      
    let mkstr_scalar (X_bnet ff) =
        match bounders.global_mgrs.lookup ff.id with
            | item::_ -> hd (item.methodgroup_methods)
            | [] ->
                let bits = ff.width
                let area = double(bits) * 10.0 // techcost.scalar_register_area_per_bit

                let str2 = { g_null_str2 with
                               mkey=           ff.id
                               ikey=           ff.id                         
                               kkey=           ff.id
                               item=           SR_scalar(X_bnet ff)
                               clklsts=        [domain]         
                               propso=         Some ff
                               area=           area
                           }
                let m_mgro = ref None
                let method_str2 = 
                  {
                    fname=           "general"       // Not used.
                    precision2=      { signed=ff.signed; widtho = Some(encoding_width (X_bnet ff)) }
                    holders=         [ref[]] // Start with one domain.
                    method_meld=     None
                    overload_suffix= None
                    m_mgro=          m_mgro
                    latency=         0
                  }

                let mgr =
                   { g_null_methodgroup with
                         pi_name=         ""
                         mg_uid=            ff.id 
                         dinstance=       str2
                         methodgroup_methods = [method_str2]
                         mgr_meldo=       None//Some mgr_meld
                         max_outstanding= 1
                     //inputs_inhold_external=false
                         auxix=           g_scalar_auxix 
                     }
                m_mgro := Some mgr                   
                if vd>=4 then vprintln 4 (sprintf "Added global resource for scalar " + str2.ikey)
                bounders.global_resources.add str2.ikey str2
                if mgr.mg_uid = "" then sf "splat-L2714" else bounders.global_mgrs.add mgr.mg_uid mgr
                bounders.global_resource_thread_use.add str2.ikey perthread.thread_str
                method_str2

    let (root_rd_hwm, root_wr_hwm) = (ref 0, ref 0)
                
    let root_minor_sheds_by_ikey = new OptionStore<string, minor_shed_t>("root_minor_sheds_by_ikey")
    let root_scheduled = new OptionStore<xidx_t, starttime_t>("root_scheduled") // Here we have the args ready vl pairs and the start time for the operation as a vl pair. 

    // Checker whether it is wrong to have earlier before later.
    let fence_hopper_pred msg earlier later  =
        if nullp earlier || nullp later then false
        else
            dev_println (sprintf "fence_hopper_pred earlier=%s, later=%s" (sfold orderToStr earlier) (sfold orderToStr later))
            let fence_domain_pivotf (dom, n0, n1, _) = dom
            let earlier = generic_collate fence_domain_pivotf earlier
            let later = generic_collate fence_domain_pivotf later
            let clashable = list_intersection (map fst earlier, map fst later)
            // Only need to compare on common state edges.
            let ans =
                if nullp clashable then false
                else
                    let ans = false
                    dev_println (sprintf "fence_hopper_pred: clashable=%A" (clashable))
                    ans
            dev_println (sprintf "ans=%A " ans)

            ans

        
    let walker_ss_gen ((rootf, eno) as rootd_in:bool * int) vinfo ghosting_set noclone bindpasso = 
        // We maintain schedules indexed by ikey
        //vprintln 0 (sprintf "walker_ss_gen: start noclone=%A for %A" noclone rootd_in)
        let (min_scheduled, minor_sheds_by_ikey, rd_hwm, wr_hwm) =
            if noclone then (root_scheduled, root_minor_sheds_by_ikey, root_rd_hwm, root_wr_hwm)
            else
                let scheduled = new OptionStore<xidx_t, starttime_t>("user-scheduled", Some root_scheduled) // Clone root schedule as basis.
                let msbi = new OptionStore<string, minor_shed_t>("msbi")
                let _ =
                    let spclone ikey vv =
                        let already = msbi.lookup ikey
                        //vprintln 0 (sprintf "walker_ss_gen:  clone root ikey=%s already=%A"  ikey already)
                        msbi.add ikey { rd_shed=ref !vv.rd_shed; wr_shed=ref !vv.wr_shed; } // Is this ref clone deep enough? Yes, it is a fresh mutable list with identical contents.
                    for kv in root_minor_sheds_by_ikey do spclone kv.Key kv.Value done
                let _ = cassert (!root_wr_hwm = 0, "writes occurred in root section")
                (scheduled, msbi, ref !root_rd_hwm, ref !root_wr_hwm)
        // Create and keep a read and a write schedule for each structural resource (indexed by ikey) for each minor state.
        // The read schedule is initialised to contain a copy of any operations from the control (aka root) schedule of the major state.
        let get_shed writef ikey =
            let a = 
                match minor_sheds_by_ikey.lookup ikey with
                | Some ov -> ov
                | None ->
                    //vprintln 0 (sprintf "walker_ss_gen: rez new sched pair for ikey=%s" ikey)
                    let nv = { wr_shed=ref[]; rd_shed=ref[]; } // If not found then not present in control schedule so can simply start afresh.
                    minor_sheds_by_ikey.add ikey nv
                    nv
            if writef then a.wr_shed else a.rd_shed

            
        let copyback() =
            // We copy back into the root/control schedule the work done by an edge for the steps up to the root hwm since this temporal real estate should best be used but we do not want structural hazards between the different edges before the control guard is established.  The oracle will/should schedule the most commonly used edges first to claim priority on use of this temporal real estate.
            // Writes must be guarded by the edge guard and we should avoid the whole sharing if edge guards are slow, off-chip results.
            // Possible refinements are 1. a non-commital (i.e. cancellable) part of an FU op, such as issuing a read can be started without control guard delay. And 2. where the control guard is manifestly constant or available at the outset...
            //dev_println ("Copyback start")
            // The copied back operation may already have been in the root, but now it will probably have an augmented customer_edges list.
            let spcopy ikey vv =
                let fromshed = get_shed false ikey // Copy back only the work from the read schedule
                let toshed =
                    match root_minor_sheds_by_ikey.lookup ikey with
                        | None ->
                            let nv = { wr_shed=ref[]; rd_shed=ref[] }
                            //dev_println ("copyback: Create a root shed and add to sheds_by_ikey " + ikey)
                            root_minor_sheds_by_ikey.add ikey nv
                            nv.rd_shed
                        | Some ov -> ov.rd_shed
                let eq (bb, n1, n2, op1) (bb', n1', n2', op1') = bb=bb' && n1=n1' && n2=n2' && op1.serial = op1'.serial
                let copyback_op (bb, n1, n2, op1) =
                    // +++ devx:    copyback cmd=read serial=SXAR22 oldnx=@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0[0]
                    //dev_println (sprintf "   copyback cmd=%s serial=OX%i oldnx=%s toshed=%A" op1.cmd op1.serial (xToStr op1.oldnx) toshed)
                    let newv = (bb, n1, n2, op1)
                    let rec sinsert arg =
                        //dev_println "sinsert step"
                        match arg with
                        | [] -> [ newv ]
                        | (bb', n1', n2', op1')::tt -> // Insertion sort, maintaining n1 ascending order.
                            if n1' = n1 && op1.serial = op1'.serial then (bb', n1', n2', op1')::tt // Replace old with new, possibly augmented.
                            elif n1' >= n1 then newv :: (bb', n1', n2', op1')::tt
                            else (bb', n1', n2', op1')::sinsert tt 
                    if n1 > !root_rd_hwm then
                        //dev_println "outside root"
                        () // Don't copy back work outside root region.
                    elif memberp newv !toshed then
                        //dev_println "already present"
                        () // If identically present then no change to old shed.
                    else
                        //dev_println "sinsert needed"
                        let ans = sinsert !toshed
                        toshed := ans
                //dev_println (sprintf "Copyback %i items ikey=%s" (length !fromshed) ikey)
                app copyback_op !fromshed
                ()
            for kv in minor_sheds_by_ikey do spcopy kv.Key kv.Value done            
            ()
        // log_op greedily enters operations in either the read or the write schedule using ASAP in the first instance.
        // Note general terminology: ASAP schedulling next places any node whose support is already placed and it places it straight after the last support item is ready, whereas
        // ALAP starts with a temporal extent and places a node whose consumers are already placed as late as possible while still serving the earliest consumer.
        // ASAP gives the minimum execution time but once that is known, some work can be moved later within the slack to reduce component count.
        // Items read twice are assumed to be readable once and the result shared. A holding register may not be needed for the first use but be needed for the subsequent reads.
        // returns a pair (rdy_guard, quad)

        let rec log_op mergef writef (auxinfo:auxix_t) (mgro:str_methodgroup_t option) opx ((vls:hbexp_t), startt) =

            let opx:newopx_t = { opx with writef=writef }
            let (qkey, rei_interval, scalar_rtl_hazard_semantic, raises_data_hazards) = // qkey is an mkey for mapping collate and an mgr key for schedule generation.
                match mgro with // During mapping scan major phase we do not have a concrete mgr to hand.
                    | Some mgr -> (mgr.mg_uid, mgr.auxix.rei_interval, mgr.auxix.scalar_rtl_hazard_semantic, mgr.auxix.raises_data_hazards)
                    | None ->     (opx.mkey,  auxinfo.rei_interval, auxinfo.scalar_rtl_hazard_semantic, auxinfo.raises_data_hazards)
            //dev_println (sprintf "res2: log_op start qkey=%s writef=%A"  qkey writef)

            let gec_hkeep0() =
                let baseidx =
                    if not writef then 0
                    else
                        //dev_println ("URGENT need hwm over all qkey")
                        let shed = get_shed false qkey // This is not correct - need hwm over all 
                        length !shed
                { startarcf=startarcf; writeshed=writef; baseidx=baseidx }


#if SPARE
            let (readop_, writeop_) = // opxn just use writef passed in.
                if not str.raises_data_hazards then (false, false)
                else match opx.cmd with
                        | "read" -> (true, false)
                        | "write" -> (false, true)
                        | other -> sf (sprintf "Expected read or write operations only on item with %s data hazards. cmd=%s" qkey opx.cmd)
#endif

            // If disjoint conditions then can re-use space in schedule, up to successor state computation: TODO.
            let dur = rei_interval - 1 // rei_interval=1 for full pipelined, so duration is from s to s+0.  Need complete strathe if not fully-pipelined.
            let max2 a b = if a>b then a else b
            let nodef ppx = // Once inserted, insert determined relative start time.
                let offset_prerel = function
                    | (eno, HK_prerel(pcv, tag)) -> (eno, HK_rel(pcv+ppx, tag))
                let opx = { opx with housekeeping=map offset_prerel opx.housekeeping }
                (rootd_in, ppx, ppx+dur, opx) // This s+dur is the end of the expected input blocked period, not the answer time, which could potentially be some time later.

                
            let this_shed =
                let s_writef = if settings.pipelining then false else writef // Which schedule to use (there is only the so-called 'read' one in the new approach).
                get_shed s_writef qkey

            //dev_println (sprintf "log_op got shed ikey=%s writef=%A"  qkey writef)
            let no_ghost_conflict ff tt = // 
                if not settings.pipelining then true
                else
                let struct_haz_checker cc (offset, (asi:saved_arc_schedule_t), getter) =
                    let shed = !(getter qkey)
                    let rec scan_struct_hazard = function
                        | [] -> cc
                        | (bb, ss1, ss2, op1)::htt ->
                            let ss1 = ss1 + offset
                            let ss2 = ss2 + offset
                            let struct_clear =
                                if scalar_rtl_hazard_semantic then 
                                    // The normal RTL data hazard rule for synchronous RAMs and scalars is that write and read at the same time are allowed provided the overall effect of read-before-write is desired.
                                    // This is different from the normal FU rule where any sort of concurrent operation (within an mgr) is disallowed as a structural hazard.
                                    true // We can just return no-hazard here and let the data hazard handler check it, since it is not a structural hazard on a scalar.
                                else
                                    (ss2 < ff) || (ss1 > tt) // Normal FU rule: Structurally clear if one finishes before the other starts, or starts after it finishes.

                            let foul = not struct_clear
                            if foul then vprintln 3 (sprintf "dad struct hazard check offset=%i, ghost_eno=E%i key=%s  len=%i items,  ff=(%A %A)   (%A  %A) foul=%A" offset asi.eno  qkey (length shed) ss1 ss2 ff tt foul) // ghosting_f3 tt
                            
                            foul || scan_struct_hazard htt
                    scan_struct_hazard shed
                let struct_haz = List.fold struct_haz_checker false (ghosting_set)
                vprintln 0 (sprintf "ghost structural_conflict eno=E%i key=%s struct_haz=%A ff=%i tt=%i (data_haz_possibility=%A)" eno qkey  struct_haz ff tt raises_data_hazards)
                if struct_haz then false
                else
                    let dhaz_checker cc (offset, (asi:saved_arc_schedule_t), getter) =
                        let shed = !(getter qkey)
                        let rec scan_data_hazard_write = function // We are doing a write: all later reads must indeed be later, earlier writes must indeed be earlier and later writes must indeed be later too.
                            | [] -> cc
                            | (bb, ss1, ss2, op1)::htt -> // Reads assumed ss1 and writes happen on ss2
                                let ss1 = ss1 + offset
                                let ss2 = ss2 + offset
                                let foul_earier_write_not_earlier = (offset < 0 && op1.writef && ss1 >= ff)
                                let foul_later_write_not_later = (offset > 0 && op1.writef && ss1 <= ff)
                                let foul_later_read_not_later = (offset > 0 && not op1.writef && ss1 <= tt)
                                // A later foul cannot be fixed by extending the current schedule, only by writing the current datum earlier or starting again with a greater postpad.
                                let foul = (foul_later_read_not_later || foul_earier_write_not_earlier || foul_later_write_not_later)
                                if foul then vprintln 3 (sprintf "dad data_write hazard check offset=%i, ghost_eno=E%i key=%s  len=%i items,  ff=(%A %A)   (%A  %A) foul=%A" offset eno  qkey (length shed) ss1 ss2 ff tt foul) // ghosting_f3 tt
                                let _ = if foul then vprintln 3 (sprintf "  write foul vector: foul_later_read_not_later=%A foul_earier_write_not_earlier=%A foul_later_write_not_later=%A" foul_later_read_not_later foul_earier_write_not_earlier foul_later_write_not_later)
                                if foul_later_read_not_later  || foul_later_write_not_later then
                                    let fatal_msg = (sprintf "write fatal_msg=I am eno=E%i. I say please extend my post pad from %i." eno asi.post_pad)
                                    let _ = vprintln 2 fatal_msg
                                    let _ = mutadd m_fatal_marks (eno, msg, "EP", 1)  // Can inform it how much if you like, or just try with one more.
                                    false // return not fouled because we want this attempt to come to a failed result fairly quickly.
                                else 
                                let _:saved_arc_schedule_t = asi
                                foul || scan_data_hazard_write htt
                        let rec scan_data_hazard_read = function // We are doing a read: all earlier writes must have committed and no later writes must have started.
                            | [] -> cc
                            | (bb, ss1, ss2, op1)::htt -> // Reads assumed on ss1/ff and writes commit on ss2/tt - TODO be clear
                                let ss1 = ss1 + offset
                                let ss2 = ss2 + offset
                                let foul_earier_write_not_earlier = (offset < 0 && op1.writef && ss2 >= ff)
                                let foul_later_write_not_later = (offset > 0 && op1.writef && ss2 <= ff)
                                let foul = (foul_earier_write_not_earlier || foul_later_write_not_later)
                                if foul then vprintln 3 (sprintf "dad data read hazard check offset=%i, ghost_eno=E%i key=%s  len=%i items,  ff=(%A %A)   (%A  %A) foul=%A" offset asi.eno  qkey (length shed) ss1 ss2 ff tt foul) // ghosting_f3 tt
                                let _ = if foul then vprintln 3 (sprintf "  read foul vector: foul_earier_write_not_earlier=%A foul_later_write_not_later=%A" foul_earier_write_not_earlier foul_later_write_not_later)
                                if foul_later_write_not_later then
                                    let fatal_msg = (sprintf "read fatal_msg=I am eno=E%i. I say please extend my post pad from %i." eno asi.post_pad)
                                    let _ = vprintln 2 fatal_msg
                                    let _ = mutadd m_fatal_marks (eno, msg, "EP", 1)  // Can inform it how much if you like, or just try with one more.
                                    false // return not fouled because we want this attempt to come to a failed result fairly quickly.
                                else foul || scan_data_hazard_read htt
                        (if writef then scan_data_hazard_write else scan_data_hazard_read) shed
                    let dhaz = if raises_data_hazards then List.fold dhaz_checker false (ghosting_set) else false    
                    vprintln 0 (sprintf "ghost structural_conflict eno=E%i key=%s struct_haz=%A dhaz=%A" eno qkey struct_haz dhaz)
                    if dhaz then true
                    elif nullp opx.fence_ordering then false
                    else
                        let rec fence_haz_check = function
                            | [] -> false
                            | (offset, (asi:saved_arc_schedule_t), getter)::tt ->
                                let shed = !(getter qkey)
                                let rec fence_violation_check = function
                                    | [] ->false
                                    | (bb, ss, ee, op1)::tt ->
                                        let earlier = opx.fence_ordering
                                        let later = op1.fence_ordering
                                        if fence_hopper_pred msg earlier later then true
                                        else fence_violation_check tt
                                let fence_foul = fence_violation_check shed
                                if fence_foul then vprintln 3 (sprintf "fences will be violated offset=%i fences=%s" offset (sfold orderToStr opx.fence_ordering)) 
                                fence_foul || fence_haz_check tt
                        fence_haz_check ghosting_set
                   
            // Structural avoidance: needs to start at or after earliest start and in a gap in the schedule or after end of previous (prev) operation, thereby extending the schedule. Also must observe max_outstanding limit when latency>II.
            //Fully-pipelined resources only use up one step in the structural scoreboard.
            let scaninsert shedref =
                //dev_println (sprintf "log_op ikey=%s cmd=%s (writef=%A) scaninsert start"  qkey opx.cmd writef)
                let rec scaninsert_asap prev = function
                    | [] ->
                        let rec extend ppx =
                            if no_ghost_conflict ppx ppx then (ppx, [nodef ppx]) // TODO no_ghost also needed for gap inserts.
                            elif not_nullp !m_fatal_marks then
                                vprintln 3 ("Abort scaninsert")
                                (ppx, [])
                            elif ppx >= 512 then sf (sprintf "scaninsert_asap: scoreboard feasible length exceeded at %i. (Reduce pause interval in bevelab)." ppx) // Hardwired limit - TODO make recipe.
                            else extend (ppx+1)
                        if devp then dev_println "scaninsert:  Insert/append on end, extending schedule length."
                        extend (max2 prev startt) 

                    | (rootd, s, e, ca)::tt when scalar_rtl_hazard_semantic && (max2 prev startt) = s ->
                        // Read old and write new concurrently is the basic synchronous logic semantic (applies to master/slave registers and RAMs that do not forward from write to read port).
                        if devp then dev_println "scan_insert: rtl_semantic: insert on top."
                        (max2 prev startt, nodef(max2 prev startt) :: (rootd, s, e, ca)::tt) // Insert on top
                        
                    | (rootd, s, e, ca)::tt when (max2 prev startt)+dur < s ->
                        if devp then dev_println "scaninsert: insert before."
                        (max2 prev startt, nodef(max2 prev startt) :: (rootd, s, e, ca)::tt) // Insert before

                    // Insert in gap : could be improved! TODO  check better
                    | (rootd, s, e, ca)::(rootd', s', e', ca')::tt when startt > e  && startt+dur < s' && no_ghost_conflict (e+1) (e+1+dur) ->
                        if devp then dev_println "scaninsert: insert in gap."
                        let ppx = e+1 
                        let nl = (rootd, s, e, ca) :: nodef(ppx) :: (rootd', s', e', ca') :: tt
                        (ppx, nl)

                    // Chain down
                    | (rootd, s, e, ca)::tt ->
                        let (ppx, nl) = scaninsert_asap (e+1) tt
                        (ppx, (rootd, s, e, ca) :: nl)
                let ans = scaninsert_asap 0 shedref
                if devp then dev_println (sprintf "log_op ikey=%s cmd=%s writef=%A scaninsert done. ans=%A"  qkey opx.cmd writef (fst ans))
                ans
                
            //dev_println (sprintf "log_op got shed ikey=%s writef=%A"  qkey writef)

            let (ppx, nl) =
                if mergef && not_nullp !this_shed then
                    if devp then dev_println ("log_op check for a merge opportunity where we can share the result of a previously-scheduled, identical, non-EIS operation (or perhaps augment an existing operation to deliver a further result one day).")
                    let rec mergescan = function
                        | (rootd, ss, ee, ca')::tt when (opx.cmd = ca'.cmd && opx.args_raw = ca'.args_raw) -> // As well as the command matching, the args may match, but that is handled inside opx_merge that may combined two potential operations into one.  Why does this not match on abs value of xidx_t/meo.n field? The schedule being considered is just for one FU.
                            let _ =
                                if ss<>startt then hpr_yikes (sprintf "%i<>%i \"ss<>start\" for key=%s merge %s with %s" ss startt (valOf_or_fail "L2944"  !(valOf_or_fail "L2571x-method_o" opx.method_o).m_mgro).mg_uid (newopxToStr opx) (newopxToStr ca'))
                            if devp then dev_println (sprintf "Return a merge with a prior op at %i" startt)
                            (ss (*bug was startt!*), (rootd, ss, ee, opx_merge settings qkey ca' ss (*startt*) opx)::tt) // Note: opx_merge could be less silent about WaW hazards.
                        | (rootd, ss, ee, ca')::tt ->
                            if devp then dev_println (sprintf "Could not merge cmd=%s args=%s with prior op at %i %i cmd=%s args=%s  scalar_rtl_hazard_semantic=%A" opx.cmd (sfold xToStr (snd opx.args_raw)) ss ee ca'.cmd (sfold xToStr (snd ca'.args_raw)) scalar_rtl_hazard_semantic)
                            mergescan tt 
                        | [] ->
                            if devp then dev_println (sprintf "log_op: did not merge: so insert freshly")
                            scaninsert !this_shed
                    mergescan !this_shed 
                else scaninsert !this_shed // Otherwise, no merge/sharing 
            this_shed := nl
            if vd >= 5 then vprintln 5 (sprintf "log_op: Schedule for %s writef=%A: ^%i items: input dur=%i start_at=%i" qkey writef (length nl) dur ppx)
            (gec_hkeep0(), (if ppx=startt then vls else X_true), writef, ppx) // End of log_op.

        and get_rdytime msg support_times2 (starttime:starttime_t) nn xoo = // Return time when this expression should be ready. The ssf/writef bool in starttime_t unused?
            //if nonep xoo then dev_println (sprintf "get_rdytime for xo=null nn=%i" nn) else dev_println (sprintf "get_rdytime for %s" (netToStr (valOf xoo)))
            let baseop = { g_null_newopx with customer_edges=[eno]; support_times=support_times2 }
            let varying_arg x = // Test whether a structure operand is a compile-time constant.
                let q = constantp x
                //dev_println (sprintf "get_rdytime rdy time %s is %A" (xToStr x) q)
                not q
            let guard_rdy = !root_rd_hwm  // Wrong - edge guard versus work guard. TODO?
            let startt = de_rel_hk starttime


            let yield_alu_or_similar_fu ww mkey sr result_latency auxix eno baseop xo oldx args cmd starttime =
                //let vlnv_kind = { g_canned_default_iplib with kind=[kinds] }
                let bkey = mappingledger_add_use mkey resume (oldx, sr) 
                let l_startt = de_rel_hk starttime
                //if vd>=6 then vprintln 6 (msg + sprintf ": nn=%s: collated structural ALU op %A prec=%i signed=%A" (x2nn_str oldx) oo wid prec.signed)
                let (methodish, mgro) =                            
                     if nonep bindpasso then (None, None)
                     else 
                         let (iname, str2, mgrs, info_, xref) = find_binding ww (valOf bindpasso) bkey
                         let (methodish, mgr) = find_method_str2 mkey cmd mgrs None
                         (Some methodish, Some mgr)

                let house = [ (eno, HK_prerel(0, "cmd-alu")); (eno, HK_prerel(result_latency, "rd-alu")) ]
                let opx = { baseop with mkey=mkey; cmd=cmd; args_raw=(X_true, map fst args); g_args_bo=ref None; method_o=methodish; oldnx=oldx; housekeeping=house; }
                let mergef = true // Enable sharing of read data for multiple purposes.
                let qq = log_op mergef false auxix mgro opx (f2o4 starttime, l_startt)
                opx.g_args_bo := Some(buildout settings opx.serial perthread.thread_fsems (fst opx.args_raw, guard_rdy) qq args)
                let ans_time = f4o4 qq + result_latency
                //vprintln 0 (sprintf "   yield_alu: Support ready times at (pre-diadic) %s. ans_time=%i." (sfold vl2s (map snd [xll;xrr])) ans_time)
                if vd>=5 && ans_time > l_startt then vprintln 5 (sprintf "structure traffic ALU waited from %i to %i to schedule %s " startt ans_time (xToStr(valOf xo)))
                (oldx, (f1o4(**) starttime, X_true,  f3o4 qq, HK_rel(ans_time, "ALU-result"))) // idiom

            let rec form_ans mirroring_policy xo = 
                match xo with
                | Some((X_bnet ff) as arg) -> // Scalar read.
                    let f2 = lookup_net2 ff.n
                    if not ff.is_array && settings.pipelining then // Scalar reads must be logged when pipelining owing to RaW hazard avoidance and need to come after all writes in antecedant ghosts.
                        let mkey = ff.id
                        // If we did this for OLD pipelining=false we would be in the write schedule. Scalars are always ready at time 0. perhaps.
                        let support_times = [] // A scalar has no support - but it might be late written in a ghost so the read op may get delayed on that
                        let baseop = { g_null_newopx with serial= fresh_opx_serial(); mkey=mkey; customer_edges=[eno]; support_times=support_times }
                        let methodish = mkstr_scalar arg
                        let mgro = !methodish.m_mgro
                        let qq = log_op true false g_scalar_auxix mgro { baseop with mkey=mkey; cmd="read"; oldnx=arg; method_o=Some methodish; postf=true } (f2o4 starttime, startt)
                        let ans_time = f4o4 qq
                        if devp then dev_println (mm + sprintf ": logged: scalar read serial=OX%i to %s    ans_time=%i" baseop.serial ff.id ans_time)
                        (f1o4(**) starttime, X_true, f3o4 qq, HK_rel(ans_time, "scalar-read")) 

                    elif isio f2.xnet_io then // Read of an input net.
                        let mkey = ff.id
                        let baseop = { g_null_newopx with serial= fresh_opx_serial(); mkey=mkey; customer_edges=[eno]; g_args_bo=ref None; support_times=[] } // Repeated code from just above
                        let methodish = mkstr_scalar arg
                        let mgro = !methodish.m_mgro
                        let qq = log_op true false g_scalar_auxix mgro { baseop with mkey=mkey; cmd="argread"; oldnx=arg; method_o=Some methodish; postf=true } (f2o4 starttime, startt)
                        let ans_time = f4o4 qq
                        // In pipelined mode surely we rely on normal padding?
                        // Sometimes 'inholder' registers have earlier been inserted at source, e.g. in kiwife where args are mutated.  Bevelab may cut through  such registers under symbolic evaluation so they do not help.  A buildout in res2 is needed, but ...
                        baseop.g_args_bo := Some(buildout settings baseop.serial perthread.thread_fsems (X_true, 0) qq [(arg, (startarcf, X_true, false, HK_rel(0, "input-arg")))])
                        //dev_println (sprintf "URGENT scalar consider i/o %s" (netToStr arg))
                        if devp then dev_println (mm + sprintf ": logged: scalar argread serial=OX%i to %s    ans_time=%i" baseop.serial ff.id ans_time)
                        //dev_println (sprintf "argread inputs-held for input %s. serial=%A" (netToStr arg) baseop.serial)
                        (f1o4(**) starttime, X_true, f3o4 qq, HK_rel(ans_time, "scalar-read")) 

                    else
                        starttime

                | Some(W_asubsc(W_string _, subsc, _)) -> starttime // No delay for constant char string indexing (uness subscript itself gets delayed).

                | Some((W_apply((fname, gis), cf, args, _)) as oldx) when not_nonep gis.is_fu -> // Generic FU transaction invoke (now including hpr_alloc and floating point conversions and TLM style where the FU instance is named by the first argument.
                    //We can only issue one per mgr per clock cycle and the max_outstanding limit must also be respected where latency>II.
                    //dev_println("W_apply " + fname)
                    let serial = fresh_opx_serial()
                    let tlmf = not_nonep cf.tlm_call
                    let sq_o (* aka overload_suffix *) = if gis.fsems.fs_overload_disam then gis.overload_suffix else None
                    let (kind, pi_name) =
                        let method_lookup_key = if gis.fsems.fs_overload_disam then fname + "+" + valOf_or_fail "LX3063" gis.overload_suffix else fname
                        let block_kind = fst(valOf gis.is_fu)
                        let q__ = assoc_external_block ww msg block_kind.kind None
                        let q = g_isloaded_db.lookup method_lookup_key
                        match q with
                            | Some (is_loaded, is_canned, kind, pi_name, instance) ->
                                //dev_println (sprintf "L3055 pi_name=%s   method_lookup_key=%s" pi_name method_lookup_key)
                                (kind, pi_name)
                            | None ->
                                sf (sprintf "Loaded metainfo for FU kind=%s. Supporting for function/method '%s' is missing." (vlnvToStr block_kind) fname)

                    let (result_delay, rei_interval) = valOf_or gis.latency (0, 1) // TODO explain
                    let (args, support_times2) =
                        if tlmf && not_nullp args && not_nullp support_times2 then (tl args, tl support_times2) // Discard the 'this' argument for a TLM call since that identifies which FU instance.
                        elif tlmf then sf (sprintf "TLM call requires a 'this' argument but no args provided:'%s' kind=%s" fname  (vlnvToStr kind))
                        else (args, support_times2)
                    let arity = length args

                    if vd>=3 then vprintln 3 (sprintf "External IP method lookup %s (TLM call=%A) overload_disam=%A  sq=%A pi_name=%s" fname tlmf gis.fsems.fs_overload_disam sq_o pi_name)
                    let (fu_meld, (gis, result_details, variable_latencyf, arg_details), externally_instantiated, mirrorable_) = assoc_fu_method ww msg kind.kind fname sq_o arity None

                    let externally_instantiated = // Old determination site? This gets recomputed in rez?
                        let kinds = hptos kind.kind
                        if memberp kinds settings.externally_flagged_kinds then
                            vprintln 1 (sprintf "Making an instance of '%s' externally instantiated owing to membership of override list (recipe of command line)." kinds)
                            true
                        else externally_instantiated

                    let raises_data_hazards = gis.fsems.fs_eis // Weakly Deduce from EIS flag for now.
                    let auxix =
                        { g_default_auxix with
                            raises_data_hazards=  raises_data_hazards
                            result_latency=       result_delay
                        }
                    let mirrorable = gis.fsems.fs_mirrorable
                    let mkey = hptos kind.kind
                    //dev_println (sprintf "FU %s: op serial=%A fname=%s mirrorable=%A or_=%A" serial mkey fname mirrorable mirrorable_)
                    let data_prec = Some gis.rv
                    let auxi = { fname=fname; gis=gis; arity=arity }                    

                    let sr = SR_fu(mkey, auxix, SR_generic_external(kind, sq_o, externally_instantiated, data_prec, Some auxi, mirrorable), Some { area_o=None; fu_meld=fu_meld }) // We are making several such generic_externals potentially for a single external IP block that supports more than one method call.  The instance name (ikey) will be the same for these
                    let use_idx = mappingledger_add_use mkey resume (oldx, sr) 
                    let (methodish, mgro) = 
                        if nonep bindpasso then (None, None)
                        else
                            let (iname, instance, mgrs, infoo, xref) = find_binding ww (valOf bindpasso) use_idx
                            let (methodish, mgr) = find_method_str2 mkey fname mgrs sq_o
                            (Some methodish, Some mgr) // Generic FU

                    //dev_println(sprintf "L3114: prec=%A sq_o=%A methodish=%A" (data_prec) sq_o methodish)

                    // Housekeeping pre-rel times are relative to when the op is finally logged, that we do not know until log_op returns, not start_time.
                    let house0 = [(eno, HK_prerel(0, "0-generic"))]
                    let result_delay =
                        match methodish with
                            | None -> result_delay
                            | Some method_str2 -> method_str2.latency

                    let house1 = if nonep gis.latency then [] else [(eno, HK_prerel(result_delay, "1-generic1") )] // 

                    let opx = { baseop with mkey=mkey; serial=serial; cmd=fname; sq=gis.overload_suffix; args_raw=(X_true, args); g_args_bo=ref None;  method_o=methodish; oldnx=valOf xo; housekeeping=house0@house1 }
                    let mergef = not gis.fsems.fs_eis && not gis.fsems.fs_nonref // Enable sharing of read data for multiple purposes unless notref or EIS apply.
                    let qq = log_op mergef false auxix mgro opx (f2o4 starttime, startt)
                    if false then
                        let mgs = if nonep mgro then "-" else sprintf "mgr=(%s %s)" (valOf mgro).mg_uid (valOf mgro).dinstance.kkey
                        dev_println (sprintf "URGENT structure traffic: S%i invoke operation '%s' on subsidiary/generic IP eis=%A mergef=%A %s (%i cf %i)" serial fname gis.fsems.fs_eis mergef mgs (length (snd opx.args_raw)) (length support_times2))
                    opx.g_args_bo := Some(buildout settings serial perthread.thread_fsems ((fst opx.args_raw), guard_rdy) qq (List.zip (snd opx.args_raw) support_times2))
                    let ans_time = f4o4 qq + result_delay
                    //if vd > 6 && ans_time > startt then vprintln 6 (sprintf "structure traffic: loaded subsidiary/generic IP %s %s: read waited from %s to ans_time=%i " mgr.mg_uid mgr.dinstance.kkey (vl2s starttime) ans_time + " to schedule " + xToStr(valOf xo))
                    (f1o4(**) starttime, X_true, f3o4 qq, HK_rel(ans_time, "generic-loaded-ip")) //


                | Some((W_asubsc(X_bnet ff, subsc, _)) as oldx)   -> // Generic array r-mode subscription (RAM/ROM read).
                    let (existed, xd) = memory_decide4 settings bindpasso ff // This decides basic technology. No of ports will be added later.
                    if vd >=6 then vprintln 6 (mm+sprintf ": res2: nn=%i walk asubsc-rmode subscription prop %s: decided=%s" nn (xToStr (valOf xo)) (memtechToStr(fst xd)))
                    let mkey = ff.id
                    let cmd = "read"
                    match xd with
                        | (tech, None) ->
                            let ans = starttime
                            if vd >= 5 then vprintln 5 (sprintf "rdytime: asubsc-rmode - memory tech for %s is a no-penalty, infinitely-ported flip-flop array - %s" ff.id (vl2s ans))
                            ans

                        | (tech, sixer) -> // memtech_t * (string * string * str_form_t * block_meld_t * bool) option    
                            let use_idx = mappingledger_add_use mkey resume (oldx, f3o4 (valOf_or_fail "L3043" sixer)) 
                            let (methodish, mgro, subsc, mkey, decoderfo) =
                                if nonep bindpasso then (None, None, subsc, mkey, None)
                                else
                                    let (iname, instance, mgrs, infoo, xref) = find_binding ww (valOf bindpasso) use_idx
                                    let sq_o = op_assoc perthread.thread_str xref
                                    let (methodish, mgr) = find_method_str2 mkey cmd mgrs sq_o
                                    let (subsc, decoderfo) =
                                        match infoo with
                                            | Some(INFO_layout (port, oc_record)) ->
                                                //let (mask, subcs) = packing_scale layout subsc
                                                let (lanesPerPWord, word_xtract, replicate, decidex, pwidth, packing_arity) = gen_portItemPack msg port.dims.laneWidth (port.dims.no_lanes * port.dims.laneWidth) oc_record.layout.r_dwidth
                                                //t lane_factor = xi_num port.dims.no_lanes
                                                let scale = xi_num oc_record.layout.words_item_ratio
                                                let subsc2 = ix_plus (depair subsc) (ix_times (xi_bnum oc_record.baser) scale)
                                                let subsc  = ix_divide subsc2 scale
                                                let offset = ix_mod    subsc2 scale
                                                let (lanesData, cycles) = decidex offset
                                                if cycles > 1 then muddy (sprintf "multicycle bondout burst  cycles=%i" cycles)
                                                (subsc, Some(word_xtract offset))
                                                
                                            | _ -> (subsc, None)
                                    (Some methodish, Some mgr, subsc, mkey, decoderfo) // Generic Array Read  -

                            let auxix =                                        
                                match mgro with
                                    | None ->
                                        match tech with // TODO abstract replicated code
                                        | T_PortlessRegFile auxix          -> auxix
                                        | T_Onchip_SRAM(auxix, romf)       -> auxix
                                        | T_Offchip_RAM(auxix, spacename_) -> auxix
                                        | other ->
                                            hpr_yikes(sprintf "TODO: Need latency figure for RAM mkey=%s: tech=%A" mkey tech)
                                            g_default_auxix
                                    | Some mgr -> mgr.auxix
                            let result_delay = auxix.result_latency
                            let serial = fresh_opx_serial()
                            let house = [ (eno, HK_prerel(0, "asubsc-rhs-addr")); (eno, HK_prerel(result_delay, "asubsc-rhs-data")) ] // Times are relative to when the op is finally logged, that we do not know until log_op returns, not start_time.
                            let opx = { baseop with mkey=mkey; serial=serial; cmd=cmd; args_raw=(X_true, [subsc]);  g_args_bo=ref None; method_o=methodish; oldnx=valOf xo; housekeeping=house; decoderfo=decoderfo }

                            let qq = log_op settings.asubsc_rmode_merge_enable false auxix mgro opx (f2o4 starttime, startt)
                            let support_times2 =
                                    if length support_times2 = 2 then tl support_times2 // We here want to just remove the X_bnet ff sensitivity here. 
                                    else
                                        dev_println "Yuck either arg could be a scalar read so fix better please"
                                        let discard_scalar_read = function
                                            | (startarcf_, _, _, HK_rel(_, "scalar-read")) -> false
                                            | _ -> true
                                        List.filter discard_scalar_read support_times2
                            //vprintln 3 (sprintf "zip matcher %i %i" (length opx.args_raw) (length support_times2))
                            if length support_times2 <> 1 then sf (sprintf "scalar-read trim failed for asubsc support_times %A" support_times2)
                            opx.g_args_bo := Some(buildout settings opx.serial perthread.thread_fsems ((fst opx.args_raw), guard_rdy) qq (List.zip (snd opx.args_raw) support_times2))
                            let ans_time = f4o4 qq + result_delay
                            if vd >= 6 (* && ans_time > startt*) then vprintln 6 (sprintf "structure traffic: asubsc-rmode waited from %s to ans_time=%i " (vl2s starttime) ans_time + " to schedule " + xToStr(valOf xo))
                            (f1o4(**) starttime, X_true, f3o4 qq, HK_rel(ans_time, "asubsc-rhs-data1")) //


                | Some((W_node(pto, V_cast CS_preserve_represented_value, [arg0], _)) as oldx) -> // Converting casts need structural units, whereas typecasts result in no additional components.
                    let pfrom = mine_prec g_bounda arg0
                    //vprintln 0 (sprintf "cvt mine prec in %A" arg0)
                    if vd >= 5 then vprintln 5 (sprintf "res2: Consider structural instantiate format convertor from %s to %s" (prec2str pfrom) (prec2str pto))
                    if pfrom = pto then starttime
                    elif pto.signed=FloatingPoint && pfrom.signed=FloatingPoint then
                        // Do not structurally instantiate float-to-float format convertors: these are combinational in implemented as RTL functions such as hpr_dbl2flt.
                        //  CV_FP_CVT_FL0_F32_F64  // Float 32 from float 64 (FL=0 implies combinational)                                        
                        //  CV_FP_CVT_FL0_F64_F32  // Float 32 from float 64 (FL=0 implies combinational)                                        
                        if vd>=6 then vprintln 6 (sprintf "skip structural instantiate an fp_convertor from %s to %s" (prec2str pfrom) (prec2str pto))
                        starttime 
                    elif pto.signed<>FloatingPoint && pfrom.signed<>FloatingPoint then
                        // Do not structurally instantiate integer format convertors: these are combination masks and sign extends.
                        if vd>=6 then vprintln 6 (sprintf "skip structural instantiate an int to int convertor from %s to %s" (prec2str pfrom) (prec2str pto))
                        starttime 
                    else
                        let (kind_vlnv, fname, gis) = gec_cv_fpcvt_name_and_gis pto pfrom None
                        let mkey = hptos kind_vlnv.kind // The mkey for a mirrorable FU is its kind.
                        let result_latency = 2 // DO NOT HARDWIRE - SET IN RECIPE PLEASE
                        let area = 22.2        // DO NOT HARDWIRE - TEMP ARBITRARY VALUE TODO
                        let (fu_meld, mgr_meld, method_meld (* , area*)) = gen_cv_fpcvt_port_structure ww pto pfrom (Some result_latency)
                        let auxix = { g_default_auxix with result_latency=result_latency }
                        let mirrorable = true
                        let auxinfo_o =
                            let gis = { g_default_native_fun_sig with overload_suffix=method_meld.method_sq_name; fsems=method_meld.meld_fsems1; args=[pfrom]; rv=pto; latency=Some(result_latency, 1) }
                            Some { gis=gis; fname=method_meld.method_name; arity=1 }
                        let sr1 = SR_generic_external(kind_vlnv, None, false(*externally_instantiatedf*), Some pto, auxinfo_o, mirrorable)
                        let sr = SR_fu(mkey, auxix, sr1, Some { fu_meld=fu_meld; area_o=Some area })
                        let (_, endtime) = yield_alu_or_similar_fu ww mkey sr result_latency auxix eno baseop xo oldx [(arg0, starttime)] method_meld.method_name starttime
                        endtime

                | Some((W_node(prec, oo, lst, _)) as orig) when length lst > 1 && // Rhs ALU use.
                                  (is_times oo ||
                                   oo=V_plus ||
                                   oo=V_minus ||
                                   is_divide oo ||
                                   (oo=V_mod && varying_arg (cadr lst))) ->
                    if length lst <> 2 && (oo=V_minus || is_divide oo || oo=V_mod) then sf ("polyadic non-associative operator in " + xToStr (valOf xo))
                    let rdy_zipped = 
                        let (l1, l2) = (length lst, length support_times2)
                        if l1 > 2 then vprintln 3 (sprintf "dido: polyadic to diadic reduction needed for %s" (xToStr orig))
                        if l1 <> l2 then sf(sprintf "zip matcher args^=%i times^=%i     args=%s" l1 l2 (sfold xToStr lst))
                        List.zip lst support_times2
                    //dev_println (sprintf "URGENT consider ALU %i %s" (x2nn orig) (xkey orig))
                    let diadic_form = diadicise orig prec oo rdy_zipped //If no ALUs are yielded then we can leave as was, but the diadic form is needed as a replacement structure if any ALUs are instantiated.
                    // Here we check the number of results bits and see if below combinational/flash implementation threshold, in which case arcate/restructure needs do nothing.
                    let need_alu_test quadraticf aggregate_metric_width_limit lst =
                        //dev_println (sprintf "oo=%A hit-it ALU instance consider %s %A" oo (xToStr(valOf xo)) (map varying_arg lst))
                        let max_arg_widths cc = function
                            | prec when not (nonep prec.widtho) -> max cc (valOf prec.widtho)
                            | _ -> cc // This ignores unsized inputs, which should not really occur anyway.
                        let product_arg_widths cc = function
                            | prec when not (nonep prec.widtho) -> cc * valOf prec.widtho
                            | _ -> cc // This ignores unsized inputs, which should not really occur anyway.
                        let dged v limit =
                            if vd>=6 then vprintln 6 (sprintf "checking structural ALU complexity threshold for operator %A  %i >= %i ?" oo v limit)
                            v >= limit
                        let prax = map (mine_precision g_bounda g_default_prec) lst
                        let metric =
                            if quadraticf then List.fold product_arg_widths 1 prax
                            else List.fold max_arg_widths 0 prax
                            // Use conjunction since we assume that when one arg is a constant then verilog_gen etc will implement an efficient custom circuit. (Is this true for constant-numerator division?)
                        conjunctionate varying_arg lst && dged metric aggregate_metric_width_limit


                    let wid =
                        match prec.widtho with
                            | Some wid -> wid
                            | None -> sf (sprintf "collate ALU op %A - no precision width indicators found %s" oo (sfold xToStr lst))

                    let (use_fu, mkey, auxix, kinds, rides, result_latency, atp) = alu_static_info ww settings msg prec oo

                    let noyield_alu (oldx, starttime) msg = 
                        if vd>=6 then vprintln 6 (sprintf "noyield_alu: %s for op %A" msg oo)
                        (oldx, starttime) // 


                    //vprintln 0 (sprintf "kiss alu_tailer ALU topf=%A oo=%A starttime=%s   x_ll=%s rdy_ll=%s   x_rr=%s rdy_rr=%s " topf oo (qq starttime) (xToStr x_ll) (qq rdy_ll) (xToStr x_rr) (qq rdy_rr))


                    let yield_alu (oldx, starttime) msg [xll;xrr] =
                        let meld = gen_cv_ALU_port_structure ww oo mkey prec result_latency atp.varilat
                        let sr = SR_fu(mkey, auxix, SR_alu { mkey=mkey; auxix=auxix; fu_meld=meld; adt_actuals_rides_=rides; latency=result_latency; oo=oo; kind={ g_canned_default_iplib with kind=[kinds] };  precision=prec; variable_latencyf=atp.varilat }, None)
                        let args = [xll; xrr]
                        yield_alu_or_similar_fu ww mkey sr result_latency auxix eno baseop xo oldx args "compute" starttime
                    let rec alu_spawn diform =
                        match diform with
                        | DD_P(topf, l, r) -> // pair
                            let (x_ll, rdy_ll) = alu_spawn l
                            let (x_rr, rdy_rr) = alu_spawn r 
                            alu_tailer topf (x_ll, rdy_ll) (x_rr, rdy_rr)
                            
                        | DD_C(topf, ll, r) -> // asymmetric
                            let (x_rr, rdy_rr) = alu_spawn r            
                            alu_tailer topf ll (x_rr, rdy_rr)

                        | DD_L(x, starttime) -> // leaf
                            // let _ = vprintln 0 (sprintf " tail hit %s" (xToStr x))
                            (x, starttime)

                    and alu_tailer topf (x_ll, rdy_ll) (x_rr, rdy_rr) =
                        let olx =
                            let starttime = vl_max_i rdy_ll rdy_rr // An explit call to vl_max is needed here owing to local de-polyadic.
                            if topf then (orig, starttime) else
                            // We need to create a dummy intermediator operator node for the rewriter to key off when making the connections.
                                let x = xi_node_nonassoc prec oo [x_ll; x_rr]
                                (x, starttime) 
                            
                        let _ = // sanity check
                            match olx with
                                | (W_node(prec, _, lst, _), _) -> if not topf && length lst > 2 then sf (sprintf "still polyadic for ll=%s rr=%s ans=%s" (xToStr x_ll) (xToStr x_rr) (xToStr (fst olx)))
                        let qq p = i2s(de_rel_hk p) 
                        

                        if not use_fu (*prec.signed <> FloatingPoint && (oo=V_plus || oo=V_minus) *) then noyield_alu olx "No structure or delay for integer add/isub"

                        elif prec.signed <> FloatingPoint && (is_times oo) then 
                             if (need_alu_test true atp.flash_limit [x_ll; x_rr]) then yield_alu olx "integer multiplier" [(x_ll, rdy_ll); (x_rr, rdy_rr)]
                             else noyield_alu olx "integer multiplier"

                        elif prec.signed <> FloatingPoint && (oo=V_mod || is_divide oo) then
                            // Div and mod are instances whenever denominator is variable. Verilog_gen can fabricate all integer constant-denominator dividers and modulus units.
                            let msg = "integer div/mod"
                            if varying_arg x_rr then yield_alu olx msg  [(x_ll, rdy_ll); (x_rr, rdy_rr)]
                            else noyield_alu olx msg

                        else yield_alu olx "misc" [(x_ll, rdy_ll); (x_rr, rdy_rr)]
                    let (_, starttime) = alu_spawn diadic_form
                    starttime

                //| Some(W_apply((fname, gis), cf, args, _)) when nonep gis.is_fu -> // Generic FU transaction invoke (now including hpr_alloc and floating point conversions and TLM style where the FU instance is named by the first argument.                
                    //dev_println(sprintf "BDAY: miss on W_apply %s %A" fname gis)
                    //starttime
                    // Detect Pause Mode and Waypoint change.?

                | Some x ->
                    let ans = starttime
                    if vd>=6 then vprintln 6 ("rdytime: other expression form: default handling (not mapped to FU) for a low-delay item " + xkey x + " " + vl2s ans)
                    ans
                    
                | None ->
                    //vprintln 5 ("rdytime: other low delay item  None - bool")
                    starttime

            let ans = form_ans None xoo
            min_scheduled.add nn ans
            let endd = de_rel_hk ans
            //dev_println  (sprintf "Recorded read rdytime %i for nn=%i" endd nn)
            if !rd_hwm < endd then rd_hwm := endd // Upgrade high water mark if exceeded (r-mode).
            ans

        and null_unitfn arg =
            let _ = vprintln // 0 ("Unitfn " + xToStr arg)
            //let _ = rdytime arg
            ()

        let res2_sonchange _ _ nn (aa, bb) =
            //dev_println (sprintf "res2_sonchange %s" (xToStr aa))
            match aa with
                | W_apply((fname, gis), cf, args, _) when fname = "hpr_pause_control"  && length args > 0 ->
                    let pm = xToStr(hd args)
                    vprintln 3 (sprintf "Ignored pause mode change to %s" pm)
                    bb

                | other ->
                    //vprintln 5 ("res2_sonchange other case: " + xToStr aa + " -> " + qToStr bb)
                    bb

        let null_b_sonchange _ _ nn (aa, bb) =
            //vprintln 0 ("Null_b_sonchange " + xbToStr aa)
            bb

        let rec x2nn_diver msg arg = // 
            match x2nn arg with
                //| None -> sf (msg + sprintf ": no nn in %s  %s" (xkey arg) (xToStr arg))
                | snn -> snn

        let lfun_rdy_time msg hkeep0 arg =
            if fconstantp arg then (hkeep0, X_true, false, HK_abs([], HK_rel(-1, "backstop")))
            else
                let snn = x2nn_diver msg arg
                match min_scheduled.lookup (abs snn) with
                    | None ->  sf (msg + sprintf ": missing ready time for snn=%i key=%s  x=%s" snn (xkey arg) (xToStr arg))
                    | Some (hkeep0, vlr, ssf, tn) ->
                        // The subscript ready time was in the read reltime schedule but will be relocated later.
                        (hkeep0, vlr, ssf, tn)

        let wvl2s(gg, nd)      = sprintf "(gg=%s,D=%i)" (xbToStr gg) nd
        let vl2s3(startarcf_, gg, bb, nd)  = sprintf "(gg=%s,b=%A,D=%i)" (xbToStr gg) bb nd
        
        let lfun_minor_sched (pp, comp_gs) clkinfo arg rhs = // Store an RTL assign in the initial relative xschedule.
            let hwm = if settings.pipelining then rd_hwm else wr_hwm // In the pipelined system, all operations go in the read schedule.
            let action_grd_rdy = !root_rd_hwm //<< ------------------------------------------- edge guard and action guard can have different ready times TODO
            let m1() = ("lfun " + title + " lhs=" + xToStr arg + " action_grd=" + xbToStr comp_gs + " rhs=" + xToStr rhs)
            //dev_println ("Using edge guard time as action guard time!" + m1() + " edge guard " + (if nonep pp then "None" else xToStr (snd(valOf pp))))

            let gec_hkeep0_lmode() = // same as rmode
                let baseidx = if not settings.pipelining then !rd_hwm else 0
                { startarcf=startarcf; writeshed=true; baseidx=baseidx }

            let hkeep0 = gec_hkeep0_lmode()

            let (wstart, support_times_o) =
                if not settings.pipelining then (0, None) // Start at zero in the write part of the schedule
                else
                    let support_times =
                        match arg with
                            | W_asubsc(_, subsc, _) -> [ lfun_rdy_time "asubsc-assign-subscript" hkeep0 subsc; lfun_rdy_time "asubsc-assign-rhs" hkeep0 rhs]
                            | other                 -> [ lfun_rdy_time "other lfun rhs"          hkeep0 rhs]
                    let rec new_asap_min cc (startarcf, g_, bool_, n1) = // Find latest support component. cf vl_max please.  And use that TODO.
                        match n1 with
                            | HK_abs(_, HK_rel(-1, _)) -> cc // Backstop
                            | HK_rel(nn, _)
                            | HK_abs(_, HK_rel(nn, _)) -> if nonep cc then Some(nn) else Some(max nn (valOf cc)) 
                            | other -> sf (sprintf "L2917 other form %A" other)
                    let scanned = List.fold new_asap_min None support_times
                    (valOf_or scanned 0, Some support_times)
            let write_starttime = (hkeep0, X_true, true, wstart)
            
            let ans =
                // Our main principle is that all value are nominally in the architectural registers at the C0 phase of any arc. This always holds for the support of arc control predicates.
                // Outside the control support, late writes must not foul with any data reads for already schedulled arcs. If this is unavoidable we fail and request our post_pad be extended appropriately in the next attempt at the whole thread.  Late writes for yet unschedulled threads are handled by those threads retarding their reads when they do get schedulled.
                // PLI calls, memory writes and anything that receives multiple input arguments in parallel, need them all building out to the same temporal point.  We nominally build out from the C0 state of the arc, but later trim off unnecessary parts (or all of) a build out where it is clear the value being read will not be dirtied. 
                // An extra channel for build out is the predicating guard, g of a work action.
                // 
                match arg with
                    | W_asubsc(X_bnet ff, subsc, _) -> // l-mode subscription is an array assign (RAM write).
                        let mkey = ff.id
                        let (existed, xd) = memory_decide4 settings bindpasso ff
                        if vd >= 5 then vprintln 5 (sprintf "mapledger/schedule lhs subscription %s " (xToStr arg))
                        // The subscript and rhs delay for conventional RAMs will be built out to the same delay (although AXI protocol provides this feature if we supported it for generic lower units).
                        // (Although can sometimes  be computed off induction variable in classical HLS).
                        // Lanes writes that are temporily dilated to wide memories also need corralling and this is done in op_merge.
                        let cmd = "write"
                        match xd with
                            | (tech, None) ->
                                if vd >= 3 then vprintln 3 (m1() + sprintf " did not treat array write (no additional structure for register file or combinational write RAM.): " + xToStr arg)
                                
                                //let _ = muddy ("need to insert args (no additional structure) for " + xToStr arg)
                                0 
                            | (tech, Some sixer) ->
                                let serial = fresh_opx_serial()
                                let use_idx = mappingledger_add_use mkey resume (arg, f3o4 sixer) 
                                if vd >= 3 then vprintln 3 (sprintf "asubsc-write: key %A   subsc=%s" (xkey subsc) (xToStr subsc))
                                if vd >= 3 then vprintln 3 (sprintf "asubsc-write: Getting asubsc write subscript ready time for n=%s sn=%s" (x2nn_str arg) (x2nn_str subsc))


                                let (methodish, mgro, args) =
                                    match bindpasso with 
                                        | None -> (None, None, [subsc; rhs])
                                        | Some _ ->
                                            let (iname, str2, mgrs, infoo, xref) = find_binding ww (valOf bindpasso) use_idx
                                            let args =
                                                match infoo with
                                                    | Some(INFO_layout (port, oc_record)) ->
                                                        //let (mask, subcs) = packing_scale layout subsc
                                                        let (lanesPerPWord, word_xtract, replicate, decidex, pwidth, packing_arity) = gen_portItemPack msg port.dims.laneWidth (port.dims.no_lanes * port.dims.laneWidth) oc_record.layout.r_dwidth
                                                        //t lane_factor = xi_num port.dims.no_lanes
                                                        let scale = xi_num oc_record.layout.words_item_ratio
                                                        let subsc2 = ix_plus (depair subsc) (ix_times (xi_bnum oc_record.baser) scale)
                                                        let subsc  = ix_divide subsc2 scale
                                                        let offset = ix_mod    subsc2 scale
                                                        let (lanesData, cycles) = decidex offset
                                                        if cycles > 1 then muddy (sprintf "multicycle bondout burst  cycles=%i" cycles)
                                                        //let hfast_rd2 = word_xtract offset port.rdata // Result comes back on this path
                                                        dev_println (sprintf "Replicating data and lanesData=%s" (xToStr lanesData))
                                                        if port.dims.no_lanes >= 2 then
                                                            [subsc; replicate rhs; lanesData]
                                                        else
                                                            [subsc; rhs]
                                                    | _ -> [subsc; rhs]
                                            let sq_o = op_assoc perthread.thread_str xref
                                            let (methodish, mgr) = find_method_str2 mkey cmd mgrs sq_o
                                            (Some methodish, Some mgr, args)

                                let support_times =
                                    match support_times_o with // this needs to magically have two or three entries
                                        | Some st -> st
                                        | None ->
                                            let ardy = lfun_rdy_time "asubsc-assign-subscript" hkeep0 subsc
                                            let lanes_st = if length args > 2 then [ ardy ] else []
                                            [ ardy; lfun_rdy_time "asubsc-assign-rhs" hkeep0 rhs] @ lanes_st
                                if vd >= 5 then vprintln 5 (sprintf "asubsc-write: retrieved subscript ready time for n=%s sn=%s at=%s" (x2nn_str arg) (x2nn_str subsc) (sfold vl2s support_times))
                                let auxix =                                        
                                    match mgro with
                                        | None ->
                                            match tech with // TODO abstract replicated code ?
                                            | T_PortlessRegFile auxix -> auxix
                                            | T_Onchip_SRAM(auxix, romf) -> auxix
                                            | T_Offchip_RAM(auxix, spacename_) -> auxix
                                            | other ->
                                                hpr_yikes(sprintf "TODO: Need latency figure for RAM mkey=%s: tech=%A" mkey tech)
                                                g_default_auxix
                                        | Some mgr -> mgr.auxix

                                let house = [ (eno, HK_prerel(wstart, "asubsc-assign-subscript")) ]
                                let baseop = { g_null_newopx with customer_edges=[eno]; g_args_bo=ref None; support_times=support_times }
                                let opx = { baseop with mkey=mkey;serial=serial; cmd=cmd; (* action_grd=comp_gs; *) args_raw=(comp_gs, args); g_args_bo=ref None; method_o=methodish; oldnx=arg; postf=true; housekeeping=house; }
                                let mergef = false // EIS writes should not be elided. We do not elide any writes at the moment. TODO - needs consideration, e.g. for lane aggregtion, this can be done inside the load/store FU rather than inside restrucuture here but better to do it early, preserving WaW write order.
                                let qq = log_op mergef true auxix mgro opx (f2o4 write_starttime, wstart)
                                opx.g_args_bo := Some(buildout settings serial perthread.thread_fsems (fst opx.args_raw, action_grd_rdy) qq (List.zip args support_times))
                                // TODO this build out channel is coupled in determination
                                let gtime = (X_true, false, (eno, HK_prerel(0, "controlhwm")))  // TODO this is the hwm of the control schedule (for this arc if they differ on this resume)
                                let posted_time = f4o4(vl_inc qq auxix.result_latency)
                                // Writes are posted and typically have an rei time of 1 for a synchronous RAM but perhaps larger for a generic off-chip output port.
                                if f4o4 qq > wstart then
                                    vprintln 3 (sprintf "structure traffic: asubsc write waited from %i to %i to schedule %s" wstart posted_time (xToStr arg))
                                if vd >= 3 then vprintln 3 (m1() + sprintf " logged: asubsc-write: support ready at %s. Write starts at ss=%s" (vl2s3 write_starttime) (vl2s3 qq))
                                posted_time // completion time. -  snd is end of completion time - we may be adding a spare write cycle at the end of every sequence.
                    | W_asubsc(l, r, _) -> sf ("minor_sched lhs malfored asubsc " + xToStr arg)
                    // | X_blift(W_bsubsc(l, subsc, _, _)) // Dynamic bit insert to scalar requires no special structural action.
                    | X_blift(W_bsubsc(W_asubsc(l, r, _), r2, _, _)) -> muddy "schedulling of bitinsert to array location (hardware style programming)"
                    | X_bnet ff -> // scalar write
                        let serial = fresh_opx_serial()
                        let mkey = ff.id
                        let support_times = [lfun_rdy_time "scalar assign rhs" hkeep0 rhs]  // Only pipelining needs scalar support times since with separate r and w schedules, wstart=0 always.
                        let baseop = { g_null_newopx with customer_edges=[eno];  g_args_bo=ref None; support_times=support_times }
                        let methodish = mkstr_scalar arg
                        let mgro = !methodish.m_mgro
                        if devp then dev_println (sprintf "scalar_write comp_grd=%s" (xbToStr comp_gs))
                        let opx = { baseop with mkey=mkey; cmd="write"; serial=serial; (*action_grd=comp_gs;*) oldnx=arg; args_raw=(comp_gs, [rhs]); method_o=Some methodish; postf=true; }
                        let qq = log_op true true g_scalar_auxix mgro opx (f2o4 write_starttime, wstart)
                        opx.g_args_bo := Some(buildout settings serial perthread.thread_fsems ((fst opx.args_raw), action_grd_rdy) qq (List.zip (snd opx.args_raw) support_times))

                        // Printing - make conditional please
                        let posted_time =
                            if settings.pipelining then f4o4(vl_inc qq (g_scalar_auxix.rei_interval))
                            else 0 // offset in write schedule. Scalars are always at zero with pipelining disabled (aka OLD).
                        if false (*devp*)  then dev_println (m1() + sprintf " logged: scalar write/bnet serial=OX%i to %s. Support ready at %s. wstart=%i. scheduled at %s." serial ff.id (sfold vl2s support_times) wstart (vl2s3 qq))
                        if f4o4 qq > wstart then vprintln 3 (sprintf "structure traffic: scalar/bnet write waited from %i to %i to schedule %s" wstart posted_time (xToStr arg))
                        posted_time

                    | X_undef -> // Side effecting Xcall's are now encoded as X_undef := fcall(...) so we see X_undef on the lhs.
                        //(X_1undef, false, write_starttime)
                        f4o4 write_starttime
                    | other -> sf ("minor_sched: RTL lhs has other form: key=" + xkey arg + " " + xToStr arg)
            if !hwm < ans then hwm := ans  // Upgrade high water mark if exceeded (l-mode).
            ()

        
        // Operator function:
        let opfun arg n bo xo _ support_times2 =
            let s() = if xo=None then xbToStr(valOf bo) else xToStr(valOf xo)
            let nn = abs n
//            match xo with
//                | Some(X_bnef ff) when not_nullp ff.length ->
            //dev_println (sprintf "opfun %s support_times^=%i" (s()) (length support_times2))                   
            let support_times2 = List.foldBack deton support_times2 []
            let all_args_rdy = vl_max_times2 false support_times2
            //dev_println (sprintf "URGENT Opfun n=%i nn=%i %s acting on " n nn (s())) //  +  sfold qToStr support_times2)
            let ans = 
                match min_scheduled.lookup nn with  // The read_at can vary for different reads of the same operation.
                | Some ov ->
                    //let _ = vprintln 3 (msg + sprintf ": Retrieved prior ready 2/2 time for nn=%i" nn)
                    TIMEOF_NODE(ov)

                | None ->
                    // The individual arg ready times as well as their maximum is required.  Those which are below the maximum may need to be read from holding registers and the operation can start once all are ready, which is the maximum rdytime over them.
                    let pres = "" // sfold qToStr support_times2 
                    //vprintln 3 ("sons=" + pres + sprintf "\nn=%i: Args ready for %s for minor_phase %i " nn (s()) (f3o3 args_rdy))
    // pass read or write info in
                    let all_args_rdy = (f1o4 all_args_rdy, f2o4 all_args_rdy, f3o4 all_args_rdy, HK_rel(f4o4 all_args_rdy, "son-max"))
                    let (hkeep0, vlr, ssf, r) = get_rdytime msg support_times2 all_args_rdy nn xo // start and end anal possible here - to flag waiting time 
                    //vprintln 3 (sprintf "Scheduled n=%i nn=%i %s result rdy for minor_phase " n nn (s()) + vl2s(vlr, ssf, r))
                    min_scheduled.add nn (hkeep0, vlr, ssf, r)
                    TIMEOF_NODE(hkeep0, vlr, ssf, r)
            ans
        let vdp = false
        let (_, walkerSS) = new_walker vd vdp (true, opfun, (fun _ -> ()), lfun_minor_sched, null_unitfn, null_b_sonchange, res2_sonchange)
        (min_scheduled, walkerSS, get_shed, copyback, rd_hwm, wr_hwm)
    (walker_ss_gen) // end of gen_walker_gen



// Get unaltered would be a better name
let get_local_unaltered_and_pli settings bindpasso combram_only arcsa =
    let vd = settings.vd
    let unaltered_array_assign_pred arg =
            match arg with
            | W_asubsc(X_bnet ff, subsc, _)   -> // Generic memory/array write.
                if vd >= 5 then vprintln 5 (sprintf "unaltered pred : subscription %s " (xToStr arg))
                match memory_decide4 settings bindpasso ff with
                    | (_, (tech, _)) -> unaltered_pred tech // Combinational writes go via the 'unaltered' route.

            // Assigns to scalar X_bnet forms can be handled here or left to go via the scheduler
            | X_undef ->false
            | X_bnet ff -> false // Returning false means they do go via the schedule
            | other -> sf (sprintf "unaltered_array_assign_pred: other form lhs " + xToStr other)

    let sco arg unaltered =
            match arg with
                | XRTL(pp_, ga, lst) -> // Can ignore nested pp owing to SP_fsm input form only allowed. ? Comment is now incorrect?
                    let sc cde unaltered =
                        match cde with
                            | Rpli(gb, fgis, args) when not combram_only ->
                                //let _ = waypoint_check ww fgis args
                                XRTL(None, ga, [ cde ]) :: unaltered
                            | Rarc(gb, l, r) when unaltered_array_assign_pred l -> XRTL(None, ga, [ cde ]) :: unaltered
                            | _ -> unaltered
                    List.foldBack sc lst unaltered
                | XIRTL_pli(pp_, g, fgis, args) when not combram_only ->
                    //let _ = waypoint_check ww fgis args
                    XRTL(None, g, [ Rpli(X_true, fgis, args)])::unaltered
                | _ ->  unaltered      // discard all else
    List.foldBack sco arcsa []


//Recall or install a fresh mapping ledger for the start of a new resume.
//Mapping ledgers are used both in the mapping and the schedulling phases.
//Since the oracle requests replay mode for the schedulling pahse, the same sequential trajectory of indexes should arise each time. Between the two, binding has taken place.    
let clone2_for_resume perthread bindpasso resume =
    perthread.edge_resources := None
    
    let mapping_needs_o =
        let mapping_needs =
            match perthread.mapping_log_for_resume.lookup resume with
                | Some ov -> ov
                | None    -> new mapping_needs_t("resume mapping needs")
        if nonep bindpasso then perthread.mapping_log_for_resume.add resume mapping_needs
        Some mapping_needs

    let np = { mapping_needs=mapping_needs_o; resource_pool=new local_resources_collection_t("major resource_pool"); edge_no_o=None } // FUs (ALUs, RAMs and so on) allocated so far on this resume.
    perthread.maj_resources := Some np
    np


//Recall or install a fresh edge ledger ready for a new edge of the current resume. The resume ledger is shared over all edges of that resume.
let clone2_for_edge perthread bindpasso eno maj_resctl =
    let mapping_needs_o =
        let mapping_needs =
            match perthread.mapping_log_for_edge.lookup eno with
                | Some ov -> ov
                | None    -> new mapping_needs_t("edge mapping needs")

        if nonep bindpasso then perthread.mapping_log_for_edge.add eno mapping_needs
        Some mapping_needs

    let op = maj_resctl
    // could clone from maj_resctl_ instead - nicer coding?
    perthread.edge_resources := Some { mapping_needs=mapping_needs_o; resource_pool=op.resource_pool.clone("clone2_for_edge"); edge_no_o=Some eno }
    ()


// res2_minor_times both creates the shedule and the walker needed to recode the RTL for a set of edges originating from a common resume.
// Schedule every subexpression, adding a time for computation in the minor schedule for an edge.
// The schedule may stall at run-time when resources vary their number of clock cycles before completion.
//
// OLD: We scheduled all reads before any writes.  A cycle at the boundary between reads and writes is the execute cycle where the first write is made and PLI calls get executed.  Some reads, the control reads, must be early and shared over multiple arcs since they serve the branch guard.
// We return newopx_t structures.
let res2_minor_times_old ww0 settings bindpasso msg perthread title (walker_ss_gen, maj_resctl) edges = 
    let ww = WF 3 msg ww0 "Start"      
    let vd = settings.vd
    let ghosting_ff = [] // Simplistic old scheme had no ghost considerations.
    let (root_min_scheduled, walkerS_root, root_get_shed, _, root_rd_hwm, _) = walker_ss_gen (true, -1) msg ghosting_ff true bindpasso  // Use root schedule for xition control
    //app (fun (edge:vliw_arc_t, eno) -> vprintln 0 (sprintf "control guard %i is %s" eno (xbToStr edge.gtop))) edges
    let clkinfo = g_null_directorate // not being used here.    
    let _ =
        let gtop_shed (edge:vliw_arc_t) =
            clone2_for_edge perthread bindpasso -1 maj_resctl
            if vd >= 5 then vprintln 5 (sprintf "root_hwm before schedulling eno=E%i is %i.  gtop=%s" edge.eno !root_rd_hwm (xbToStr edge.gtop))
            let _ = mya_bwalk_it ("root", walkerS_root, None, ref []) clkinfo edge.gtop
            if vd >= 5 then vprintln 5 (sprintf "root_hwm after schedulling eno=E%i is %i" edge.eno !root_rd_hwm)
            ()
        app gtop_shed edges // Schedule resources used in all xition guards

    // Now we know the re-initiation interval for this resume.
    let root_hwm =
        if vd >= 5 then vprintln 5 (title + sprintf ": Scheduled root/control logic for this resume with %i edges. root_hwm=%i" (length edges) !root_rd_hwm)
        !root_rd_hwm

    let schedule_edge (edge:vliw_arc_t) =
        let (min_scheduled, walkerS2, get_shed, copyback, rd_hwm, wr_hwm) = walker_ss_gen (false, edge.eno) msg ghosting_ff false bindpasso  // Clone for each edge
        clone2_for_edge perthread bindpasso edge.eno maj_resctl
        if vd >= 5 then vprintln 5 (msg + sprintf ": schedule/walk work edge start (old) eno=E%i. cmd count=%i" edge.eno (length edge.cmds))
        let ii = { id="temporary-rewrap" } : rtl_ctrl_t
        let _ = walk_sp ww (vd>=5) (sprintf "m_%i" edge.eno, walkerS2, None, g_null_pliwalk_fun) g_null_directorate (SP_rtl(ii, edge.cmds))
        if vd >= 5 then vprintln 5 ("now copyback")
        copyback()
        if vd >= 5 then vprintln 5 (msg + sprintf ": schedule/walk edge done (old) eno=E%i" edge.eno)
        (None, min_scheduled, edge, get_shed, SS_old(ref None, !rd_hwm, !wr_hwm))
    let sheds = map schedule_edge edges

    let ww = WF 3 msg ww0 "Done"      
    (root_min_scheduled, sheds, root_get_shed, root_hwm) // res2_minor_times_old


let assocToStr (ov, writef, modestr, npc) = sprintf "   assoc   ov=%i/%c -->  nv=%s %i" ov (if writef then 'w' else 'r')  modestr npc




    
// Allocate new pcvalues for the work of each edge.
// The arcs for a major state of a thread are here given pc values in a new 'reindexed' FSM.
let res2_make_index_x ww0 settings bounders thread resume npc_in new_pc_code_point title cc (root_hwm, sheds, root_get_shed) = 
    let m = "res2_make_index: " + title
    let ww = WF 3 m ww0 (sprintf "Start.")

    // Letter F denotes as specified by the main PC.
    let root_items = map (fun x -> (x, false, g_primpc_F, (new_pc_code_point true "root"))) [0 .. root_hwm]

    //reportx 3 (title + " Root Assoc Table") assocToStr (root_items)
    
    let make_index_for_edge_old cc (shed_style_info_, min_scheduled, edge, get_shed, sty) =
        match sty with
            | SS_old(pser, rd_hwm, wr_hwm) ->
                let textual_sum = sprintf "hwm=%i.%i.%i  " root_hwm rd_hwm wr_hwm
                let _:vliw_arc_t = edge
                let m00 = title + sprintf ": mkindex for edge no %i: (root.read.write) " edge.eno + textual_sum
                let len = rd_hwm + wr_hwm + 1 // Last read overlaps first write.
                mutadd bounders.xition_summaries (edge.eno, (textual_sum, len))
                let ww = WF 3 (m) ww0 m00

                // Reads upto root_hwm have shared npc values over all vliw edges in this resume/major state.
                let dend = rd_hwm + wr_hwm
                let priv_start_pc = if dend <= root_hwm then -1 else (new_pc_code_point false "")
                let priv_last_pc  = if dend <= root_hwm then -1 else priv_start_pc + dend - root_hwm - 1
                let (assocs, exec_pc) = 
                    let new_pt msg =  new_pc_code_point true msg
                    let lpc() = f4o4(hd(rev root_items))

                    // Create arc-specific assoc starting at root_hwm since we may have to at least provide the W0 entry at that offset.
                    // Note: The generated ov/writef values span two ranges: 0..rd_hwm/false and then 0..wr_hwm/true with one overlapping npc which is the exec cycle.
                    let rec augment_assocs_normal pos =
                        if pos > dend then []
                        elif pos > rd_hwm then                         (pos-rd_hwm, true, g_primpc_F, new_pt (sprintf "WN %i" pos)) :: augment_assocs_normal (pos+1)
                        elif pos = rd_hwm && pos = root_hwm then       (0, true, g_primpc_F, lpc()) :: augment_assocs_normal (pos+1)
                        elif pos = rd_hwm then
                            let v = new_pt (sprintf "RWN %i" pos)
                            (pos, false, g_primpc_F, v) :: (0, true, g_primpc_F, v) :: augment_assocs_normal (pos+1)
                        elif pos = root_hwm then augment_assocs_normal (pos+1)
                        else (pos, false, g_primpc_F, new_pt (sprintf "RN %i" pos)) :: augment_assocs_normal (pos+1)

                    let assocs = augment_assocs_normal root_hwm 
                    reportx 8 ("Assoc Table for " + title) assocToStr (root_items @ assocs)
                    let exec_pc =
                        let rec scan = function
                            | (0, true, _, nv)::_ -> nv
                            | _::tt -> scan tt
                            | _ -> sf "no exec_pc"
                        scan assocs
                    (assocs, exec_pc)

                let npc_in2 =
                    let rec sc2 = function
                        //| (true, _, _, _)::tt -> sc2 tt
                        | (_, _, _, npc)::_ -> npc


                    sc2 (root_items @ assocs)

                let pse =
                    {
                        edge=           edge
                        eno=            edge.eno
                        resume=         resume
                        iresume=        lc_atoi32 resume

                        // These next few fields are patched with values in the 'assembler' second pass.
                        dest_pc=        -1


                        assocs=         assocs
                        maj_root_pc=    npc_in2
                        priv_exec_pc=   exec_pc
                        dend=           dend
                        priv_start_pc=  priv_start_pc
                        priv_last_pc=   priv_last_pc

                        // Final field are used by new.
                        asio=           None 
                        reverb_state_names= [] 
                    }
                pser := Some pse
                pse::cc

    let codings = List.fold make_index_for_edge_old cc sheds
    (root_items, codings) // end of res2_make_index_x (non pipelined)


// NEW


// Allocate new pcvalues for the work of each edge.
// The arcs for a major state of a thread are now given pc values in a new 'reindexed' FSM.
// The can_go_back flag holds if the first read/control work can be inserted on the predeceding control arcs.
let res2_make_index_new ww0 settings bounders thread cc (asi_board, min_e, max_e) enos_by_resume = 
    let m = "res2_make_index_new for thread " + xToStr thread
    let ww = WF 3 m ww0 (sprintf "Start")
    let _ :saved_arc_schedule_t array = asi_board

    let make_index_for_resume_new (xlats, cc, npc_in) (resume, enos) = 
        let whence = lc_atoi32 resume
        if nullp enos then
            let _ = vprintln 3 (m + sprintf ": whence=%i npc_in %i. No enos" whence npc_in)
            (xlats, cc, npc_in)
        else
        // All edges on this resume share the same control code points, they differ in their post_pad only.
        let control_length = 
            let bag eno = 
                let asi = asi_board.[eno-min_e]
                //let root_items = [0 .. asi.root_hwm]
                let _ = vprintln 3 (m + sprintf ": npc_in=%i eno=E%i control_length=%i" npc_in eno asi.control_length)
                asi.control_length
            once (sprintf "L3032 control lengths differ") (list_once(map bag enos))

        let maj_root_pc = npc_in
        let npc_in = npc_in + control_length
        let root_assocs = map (fun x -> (x, false, g_primpc_F, x+maj_root_pc)) [ 0..control_length-1]

        let make_index_for_edge_new (cc, npc_in) eno =
            let asi = asi_board.[eno-min_e]
            let asis = valOf_or_fail ("no asis") !asi.m_asiso
            let total_len = asi.control_length + asi.post_pad
            // , min_scheduled, edge, get_shed, rd_hwm, wr_hwm) =
            let textual_sum = sprintf "hwm=%i.%i.pad=%i.len=%i  " (asi.control_length-1) asis.minor_shed_hwm asi.post_pad total_len
            let m00 = m + sprintf ": mkindex for edge no %i: (root.work.pad) " eno + textual_sum
            mutadd bounders.xition_summaries (eno, (textual_sum, asi.control_length))
            let ww = WF 3 (m) ww0 m00

            // We first have the control and then we have the post pad.
            // If the work length is greater than the control and post pad lengths we have additional reverb states numbering the excess. The self re-initiation interval is the control+post pad length (we rely on schedule formation order to effectively provided a pre-pad to non-self successors).

            let priv_start_pc = if asi.post_pad=0 then -1 else npc_in
            let priv_last_pc  = if asi.post_pad=0 then -1 else npc_in+asi.post_pad-1
            let priv_len = asi.post_pad
            let npc_in = npc_in + asi.post_pad
            let work_len = if asis.minor_shed_hwm < 0 then 0 else asis.minor_shed_hwm + 1
            let number_of_reverb_states = max (work_len - (asi.control_length + priv_len)) 0
            //dev_println (sum + sprintf " %i reverb states" number_of_reverb_states)
            let (reverb_state_names, reverb_assocs) =
                let pmode = sprintf "VE%iv" eno
                let rev_state_name eno_ nn = pmode + i2s nn
                if number_of_reverb_states > 0 then
                    let gen_reverb_state nn =
                        let rnet = newnet_with_prec g_bool_prec (rev_state_name eno nn)
                        //dev_println (sprintf "created reverb state %i   %s" nn (netToStr rnet))
                        mutadd settings.m_morenets rnet
                        let assoc = (asi.control_length + priv_len + nn, false, pmode, nn)
                        ((nn, rnet), assoc)
                    List.unzip (map gen_reverb_state [ 0 .. number_of_reverb_states-1 ])
                else ([], [])

            let private_assocs =
                if priv_len = 0 then []
                else
                    let pmode = g_primpc_F // "F" 
                    map (fun x -> (x+asi.control_length, false, pmode, x + priv_start_pc)) [ 0..priv_len-1 ]

            let assocs =
                let assocs = root_assocs @ private_assocs @ reverb_assocs
                if settings.vd >= 4 then reportx 4 ("Assoc Table for " + m) assocToStr assocs
                reportx 0 ("ADDITIONAL Assoc Table for " + m) assocToStr assocs
                assocs


            let pse =
                {  
                    resume=         resume
                    iresume=        lc_atoi32 resume
                    edge=           asi.edge
                    eno=            eno

                    // This fields is patched with values in the 'assembler' second pass.
                    dest_pc=        -1 // Put in later during 'assembly'


                    priv_exec_pc=   -100// exec concept is not used when pipelined (aka new )- PLI are run as prescribed by dataflow - need fences now.
                    
                    dend=           (asi.post_pad+control_length-1)

                    assocs=         assocs
                    maj_root_pc=    maj_root_pc
                    priv_start_pc=  priv_start_pc // Private PC values are only used when post_pad is non-zero.
                    priv_last_pc=   priv_last_pc

                    asio=           Some asi
                    reverb_state_names= reverb_state_names
                }
            (pse::cc, npc_in)
        let (codings, npc_out) = List.fold make_index_for_edge_new (cc, npc_in) enos
        let pc_hwm = npc_out - 1
        let new_xlat = (whence, (maj_root_pc, pc_hwm, root_assocs, None)) // Old to new root PC translation and control state info.
        (new_xlat::xlats, codings, npc_out) 
    let (xlats, codings, npc_arity) = List.fold make_index_for_resume_new cc enos_by_resume
    (xlats, codings, npc_arity) // end of res2_make_index_new (normal pipeline)

    
// Rewrite the edge schedules for a thread to use the new PC values.
// This is called once per resume with  different v5?
let res2_apply_index ww0 settings bindpasso bounders thread title v5 codings stallpoints (root_min_scheduled, root_hwm, sheds, root_get_shed) = 
    let _:proto_scheduled_edge_t list = codings 
    let vd = settings.vd
    let debugm = vd >= 5 
    let m = "res2_apply_index: " + title
    let (maj_root_pc, pc_hwm, root_items, ant_pcs) = v5

    let ant_pcs = 
        let flatteno = function
            | None -> None
            | Some l -> Some(list_flatten l)
        flatteno ant_pcs
        
    let ww = WF 3 (m) ww0 (sprintf "Start")

    let wrap_unaltered eno min_scheduled cmd xx =
        let support =
            let get_support cc (d, cmd, s, _) = sd_union (cc, s)
            List.fold get_support [] (List.fold (tripgen_x -1 false) [] xx)
        //dev_println (sprintf "support is " + sfold (fun (x,_) -> xToStr x) support + " for unaltered RTL " + sfold xrtlToStr x)
        let support_nn =
            let dex cc (arg, lanes_) =
                if constantp arg then cc else x2nn arg :: cc
            List.fold dex [] support // OLD: The idea is it is all ready on the exec cycle but we need to know which items need reading from holding registers.  Scalars and combinational functions of them do not, but comb functions of restructured do  etc ..   NEW: All done during arcate.
        let support_times =
            let sonrdy nn =
                match (min_scheduled:min_scheduled_t).lookup nn with
                    | None ->
                        dev_println (sprintf "support_times: missing ready time: need %i for " nn + sfold i2s support_nn + " unaltered RTL=" + sfold xrtlToStr xx)
                        let hkeep0 = { startarcf=false; writeshed=false; baseidx=0; } // A total dummy but this code route is not used...
                        (hkeep0, X_true, true, HK_rel(0, "spare-tag"))
                    | Some ans -> ans
            map sonrdy support_nn
        let baseop = { g_null_newopx with serial=fresh_opx_serial(); customer_edges=[eno];  g_args_bo=ref None; support_times=support_times }
        { baseop with  mkey="OldPLI"; method_o=None; cmd=cmd; unaltered=xx; postf=true } // There is no housekeeping popped in here: exec cycle is implied when pipelining is disabled and we instead have fabbed PLI when pipelining.

    let gen_predictor_entry items = map (fun (track, shen, newop) -> (track, shen, newop)) items
       
    let reindex_minor_step min_scheduled get_shed eo eno all_assocs__ (tn, writef, mc, npcv) =
        let execf = not settings.pipelining && writef && tn=0
        if vd>=4 then vprintln 4 (sprintf "reindex_minor_step root_pc=%s eo=%s eno=E%i (tn=%i, writef=%A, npcv=%i) execf=%A" (i2s maj_root_pc) (if nonep eo then "None" else "Some(...)") eno tn writef npcv execf)
        let m_any_stallable = ref false // Various interlocks can be bypassed at compile time if there are no stallable operations to upset the static schedule.
        let codings_for_this_edge eno = List.filter (fun (pse:proto_scheduled_edge_t)->pse.eno=eno) codings // TODO use the pse map please instead of filter
        let tabline = ref []
        let m_work = ref []
        //if vd >= 5 then vprintln 5 (sprintf "Dublin: reindex_minor_step: root_hwm=%i, assocs are=" root_hwm  + sfoldcr_lite assocToStr all_assocs__)
        let assoc_g wf msg eno ov = // Assoc_g is a lookup function for relative (rpcv) PC values that gives the absolute PC
            //dev_println (sprintf "assoc_g eno=E%i root_items=%s" eno (sfold assocToStr root_items))
            let assocs = 
                if eno < 0 && not wf then root_items // For control state, where there is no specific edge, we use eno<0.
                else
                    let pse:proto_scheduled_edge_t =
                        once (sprintf "L3054 no pc encoding found for eno=E%i" eno) (codings_for_this_edge eno)
                    //dev_println (sprintf "assoc_g eno=E%i pse.assocs=%s root_items=%s" eno (sfold assocToStr pse.assocs) (sfold assocToStr root_items))
                    lst_union root_items pse.assocs
            match List.filter (fun q3 -> f1o4 q3 = ov && f2o4 q3 = wf) assocs with
                | [] ->
                    //vprintln 0 (msg + sprintf ", eno=E%i: assoc_g lookups %i/%A yielding nothing!" eno ov writef)
                    //reportx 0 (title + " Assocs In Scope") assocToStr assocs
                    []
                | others ->
                    if debugm then vprintln 3 (msg + sprintf ", eno=E%i: assoc_g lookups %i/%A yielding  xnumber=%i" eno ov wf (length others))
                    map (fun (_, _, mc, pc) -> (mc, pc)) others

#if PROBLEM_WITH_GETSHED_NOT_BEING_ENUMERABLE
        let items =
            let m_r = ref []
            let git k v =
                let contents = !v
                vprintln 0 (sprintf "git %s" k)
            for x in (get_shed writef) do git x.Key x.Value done
            !m_r
#endif

        let assoc_current msg eno opcv = assoc_g writef msg eno opcv
        let _ =
            let rez_structural_action key (vl:str_methodgroup_t list) = // Applied to all FUs.
                //vprintln 3 (sprintf "rez_structural_action: key=%s tn=%i vl list is %s" key tn (sfold (fun v->v.mg_uid) vl))
                let shed0 = list_flatten (map (fun v-> !(get_shed writef v.mg_uid)) vl) 

                let (shed_lst, remnants) = groom2 (fun ((rootf, s_eno), s, e, newop) -> s=tn) shed0 // Yuck - please get rid of the outer tn loop soon - can index over the shed contents instead.
                let (shed_lst, remnants) =
                    let posh_grooms ((rootf, s_eno), s, e, newop) (yes_list, remnants) =
                        if s=tn then
                            let multi = true// for now - TODO explain
                            if multi then
                                (((rootf, s_eno), s, e, newop)::yes_list, ((rootf, s_eno), s, e, newop)::remnants)
                            else (((rootf, s_eno), s, e, newop)::yes_list, remnants)

                        else (yes_list, ((rootf, s_eno), s, e, newop)::remnants)

                    List.foldBack posh_grooms shed0 ([], [])// Yuck - please get rid of the outer tn loop soon - can index over the shed contents instead.

                // remnants not checked for null or only multi used root entries
                //if vd >= 5 then vprintln 5 (sprintf "reindex %s: eno=E%i %s tn=%i doing now %i/%i work items" (sfold (f4o4 >> (fun x->x.mgro) >> mgrTextualName) shed_lst) eno "" tn (length shed_lst) (length shed0))

                let reindex_sc ((rootf, s_eno), sss, eee, newop:newopx_t) cc = // The old sss/eee form assumes consecutive states but these are not in general so delete it please
                    if eno < 0 && not rootf then
                        //dev_println (sprintf "sKIPCASE-A2 Skip opxx oldx=%s for shen_rootf=%A eno=E%i customer_edges=%s" (xToStr newop.oldnx) rootf eno (sfold i2s newop.customer_edges))
                        cc  // Do just the root work if in control/root phase.
                    elif eno >= 0 && not (memberp eno newop.customer_edges) then // <----- here!
                        //dev_println (sprintf "sKIPCASE-B2 Skip newop oldx=%s for shen_rootf=%A eno=E%i customer_edges=%s" (xToStr newop.oldnx) rootf eno (sfold i2s newop.customer_edges))
                        cc // Just do this edge's work when we have a specific edge no.
#if TEMPORARY_ERROR_BYPASS_HK
                    elif disjunctionate temporary_invalid_hk_check newop.housekeeping then
                        let temporary_invalid_hk_check = function // Should not be needed. TODO delete.
                            | (_, HK_prerel _) -> true
                            | _                -> false
                        //dev_println (sprintf "HK_prerel %A should have been converted by log_op oldx=%s for shen_rootf=%A eno=E%i customer_edges=%s" newop.housekeeping (xToStr newop.oldnx) rootf eno (sfold i2s newop.customer_edges))
                        cc
#endif
                    else

                    let reindex_hk wflag eno hk0 =
                        let reindex_hk1 hk_tag rpcv =
                            let ans = assoc_g wflag "reindex_hk1" eno rpcv
                            let _ =
                                let debugf (rpcv:int) =
                                    let mgkey = (if nonep newop.method_o then "no instance" else (valOf !(valOf newop.method_o).m_mgro).mg_uid)
                                    vprintln 3 (sprintf "reindex_hk %s rpcv=%i  --> %A     tags=%A,%A " (sfold (fun vl->vl.mg_uid) vl) rpcv (sfold p2s ans) mgkey hk_tag)
                                if debugm then debugf rpcv
                            if rpcv >= 0 && nullp ans then sf (sprintf "reindex_hk1 error, no lookup result when finding HK_abs.  serial=OX%i tag=%s  hk0=%A  for nn=%s eno=E%i rpcv=%i/%A" newop.serial hk_tag (hkToStr "" hk0) (x2nn_str newop.oldnx) eno rpcv wflag)
                            ans

                        match hk0 with
                            | HK_prerel _ -> sf  ("reindex_hk: HK_prerel should have been converted by log_op")
                            
                            | HK_rel(rpcv, tag) ->
                                //dev_println ("HK_rel clause reindex")
                                let ans = reindex_hk1 tag rpcv
                                HK_abs(ans, HK_rel(rpcv, tag))
                            | HK_abs(already, HK_rel(rpcv, tag)) -> 
                                //let ans = reindex_hk1 wf tag rpcv // Why reindex something already allocated? hk-1 can be edge-specific..
                                //dev_println ("HK_abs clause reindex")                   
                                let ans = []
                                HK_abs(lst_union already ans, HK_rel(rpcv, tag))

                    let reindex_hk_pair wflag (eno, hk0) = (eno, reindex_hk wflag eno hk0)

                    let get_relevant_eno () =
                        let relevant_eno = eno
                        relevant_eno
                        
                    let reindex_hk_any_relevant_edge wflag hk0 =
                        let relevant_eno = get_relevant_eno()
                        reindex_hk wflag relevant_eno hk0
                        
                    let create_hk wflag rpcv =
                        if eee <> sss then muddy ("trapped exotic eee use for " + (mgrTextualName newop.method_o))
                        let relevant_eno = get_relevant_eno()                        
                        (relevant_eno, reindex_hk wflag relevant_eno (HK_rel(rpcv, "hk-create")))

                    let hk =
                        if length newop.housekeeping > 0 then
                            //let sl:int list = [sss..sss+length newop.housekeeping-1] // Consecutive relative times are appropriate.  H/K needs to be marked as rel/abs.  A rel remaining at render time is a bug.
                            map (reindex_hk_pair writef) newop.housekeeping
                        else
                            //dev_println ("create_hk still used for "  + (mgrTextualName newop.method_o)) // This route taken for all simple scalar vars and constants.
                            map (create_hk writef) [sss..eee]                                // Again assumes consecutive? DELETE ME
                    let _ =
                        let has_hs = // Stall is possible on both issue of args and collection of result.
                            match newop.method_o with
                                | None -> false
                                | Some method_str2 ->
                                    let mgr = valOf_or_fail "L3828" !method_str2.m_mgro
                                    match mgr.mgr_meldo with
                                        | None -> false // scalar most likely
                                        | Some mgr_meld -> not_nonep mgr_meld.hs_vlat

                        if has_hs then
                            m_any_stallable := true
                            let add_stallpoint = function
                                | (_, HK_abs(times, _)) ->  app (fun t -> (stallpoints:stallpoints_t).add  t "true") times
                                | _ -> sf ("add_stallpoint")
                            app add_stallpoint hk

                    let support_times = // we can use any example eno for reindex of support times since the support of a shared op is consistent over all sharing edges.
                        let reindex_support_times = function
                            | (startarcf, vlnet, false, hk) -> (startarcf, vlnet, false, reindex_hk_any_relevant_edge false hk)
                            | other -> sf (sprintf "reindex_support_times %A " other)
                        map reindex_support_times newop.support_times
                    let newop = { newop with housekeeping=hk; support_times=support_times }
                    if vd>=5 then vprintln 5 (sprintf "rootf=%A,writef=%A,tn=%i: reindexed op OX%i: %s" rootf writef tn newop.serial newop.cmd)
                    (CBG_track(mc, npcv, eno), { rootf=rootf; old_pcvs=hk; ss=assoc_current "sss" eno sss }, newop) :: cc
                let shed_lst = List.foldBack reindex_sc shed_lst []
                if not_nullp shed_lst then app (fun newop -> mutaddonce tabline (op3ToStr newop)) shed_lst
                mutadd m_work shed_lst // append would be more meaningful but flattened on deref.
                ()
            for z in bounders.global_mgrs do rez_structural_action z.Key z.Value done

        let extra =
            if execf then
                match eo with
                    | None -> []
                    | Some(waypoints, local_unaltered) ->
                        let report_waypoint = function
                            | WP_wp(_, _, nn, ss)             ->  mutaddonce tabline (" W/P:" + string_clip 20 ss) 
                            | WP_other_string(_, _, fname, ss) -> mutaddonce tabline (" PLI:" + string_clip 20 ss)  
                        app report_waypoint waypoints
                        //OLD: 0th microstate of write schedule is the exec cycle. 
                        let wf = not settings.pipelining
                        let gec_shen v = { old_pcvs=[(eno, HK_abs(assoc_g wf "pli-exec" eno 0, HK_rel(0, "pli-exec")))]; ss=assoc_g wf "pli-ss" eno 0; rootf=false }
                        // Scalar assigns can go through the pli/unaltered route or become ops - lfun has chosen which.
                        let pli_work = (CBG_track(mc, npcv, eno), gec_shen 0, wrap_unaltered eno min_scheduled "*misc_exec*" local_unaltered)
                        [pli_work]
            else []

        let chr = if writef then "W" else "R"
        let xm = if nonep eo then " CTRL" else " DATA"
        let spaceadd cc str = if cc="" then str else str + " " + cc // This reverses  the list back again.
        let stallable = not_nonep(stallpoints.lookup (mc, npcv))
        let tabdata = [ ("") + p2s (mc, npcv) + (if execf then "+E" else "") + (if stallable then "+S" else ""); (if eno < 0 then "-" else sprintf "E%i" eno); chr + i2s tn + xm; List.fold spaceadd "" !tabline]
        if vd>=5 then vprintln 5 ("tabdata = " + sfolds tabdata)
        let statework = (eno, extra @ list_flatten !m_work)
        let predictor_graph_entries = map PGE_wi (list_flatten(map gen_predictor_entry !m_work))
        (tabdata, statework, predictor_graph_entries)


    let (root_tabdata, rootwork, root_pge) = // The last root/control state overlaps with the first rd/data state and so we here only generate those before the last one.  The (ro) work for the last one is cloned into each arc's first rd/data state. Really?
        let (root_tabdata, rootwork, pge) = List.unzip3(map (reindex_minor_step root_min_scheduled root_get_shed None -1 []) root_items)
        (root_tabdata, (maj_root_pc, root_hwm, rootwork), pge)

    let keymsgs:string list ref = ref []

    let process_edgeshed (shed_style_info_, min_scheduled, edge, get_shed, sty) = // Interpret the schedule stored for each FU. Write a report. Convert to se form.
        let eno = (edge:vliw_arc_t).eno
        match sty with 
           | SS_new(pser, asi) -> 
                let pse:proto_scheduled_edge_t = once (sprintf "L3166 no pc encoding found for eno=E%i" eno) (List.filter (fun pse->pse.eno=eno) codings)
                pser := Some pse
                let arc_hwm = (valOf_or_fail "L3266 asiso" !asi.m_asiso).minor_shed_hwm
                let m00 = (title + sprintf ": process_shed for edge no %i:  hwm=%i.%i  (root.arc.write)" eno root_hwm arc_hwm)
                let ww = WF 3 (m) ww0 m00
                let len = arc_hwm + 1 
                // reads upto root_hwm have shared npc values over all vliw edges in this resume/major state.
                if vd >= 5 then mutadd keymsgs (sprintf "  Absolute key numbers for scheduled edge %s %i :  major_start_pcl=%s   edge_private_start/end=%i/%i exec=%i (dend=%i)" title eno (i2s pse.maj_root_pc)  pse.priv_start_pc pse.priv_last_pc pse.priv_exec_pc pse.dend)
                let local_unaltered = get_local_unaltered_and_pli settings  bindpasso true edge.cmds 
                //dev_println (sprintf "Local_unaltered new was %s" (sfoldcr_lite xrtlToStr local_unaltered))
                let waypoints = find_waypoint_or_interesting_pli ww false local_unaltered // sits guarded by being exec minor phase.
                let wp =
                    let gwp cc = function
                        | WP_wp(pp, g, nn, ss) -> Some(nn, ss)
                        | _ -> cc
                    List.fold gwp None waypoints
                let (tabdata, camops_work, predictor_graph) = List.unzip3(map (reindex_minor_step min_scheduled get_shed (Some(waypoints, local_unaltered)) eno pse.assocs) ((* for nonpipelined restore talisman root_items @*) pse.assocs))
                let predictor_graph = (pse.priv_exec_pc, (PGE_wp(CBG_track(g_primpc_F, pse.priv_exec_pc, eno), waypoints))::list_flatten predictor_graph) 
                //let _ = report_work_summary ("saveit:" + m00) camops_work

                let scheduled_edge = // This adds little over the pse!
                    { dend=            pse.dend;
                      maj_root_pc=     pse.maj_root_pc
                      priv_start_pc=   pse.priv_start_pc
                      priv_exec_pc=    pse.priv_exec_pc
                      priv_last_pc=    pse.priv_last_pc
                      edge=            edge
                      work=            camops_work
                      waypoint=        wp
                      pse=             pse
                    }
                (tabdata, scheduled_edge, predictor_graph)

           | SS_old(pse, rd_hwm, wr_hwm) ->
                let m00 = (title + sprintf ": process_shed for edge E%i:  hwm=%i.%i.%i  (root.read.write)" eno root_hwm rd_hwm wr_hwm)
                let ww = WF 3 (m) ww0 m00
                let len = rd_hwm + wr_hwm + 1 // Last read overlaps first write
                // reads upto root_hwm have shared npc values over all vliw edges in this resume/major state.
                let pse:proto_scheduled_edge_t = once (sprintf "L3166 no pc encoding found for eno=E%i" eno) (List.filter (fun pse->pse.eno=eno) codings)        
                if vd >= 5 then mutadd keymsgs (sprintf "  Absolute key numbers for scheduled edge %s %i :  major_start_pcl=%s   edge_private_start/end=%i/%i exec=%i (dend=%i)" title eno (i2s pse.maj_root_pc)  pse.priv_start_pc pse.priv_last_pc pse.priv_exec_pc pse.dend)
                let local_unaltered = get_local_unaltered_and_pli settings  bindpasso false edge.cmds // Retain input PLI but input RTL statements are converted to bus ops.  One reason for handling separately is we don't want to schedule a debug/display read to a structural RAM for simulation printout only.
                let waypoints = find_waypoint_or_interesting_pli ww false local_unaltered // sits guarded by being exec minor phase.
                let wp =
                    let gwp cc = function
                        | WP_wp(pp, g, nn, ss) -> Some(nn, ss)
                        | _ -> cc
                    List.fold gwp None waypoints
                let (tabdata, work, predictor_graph) = List.unzip3(map (reindex_minor_step min_scheduled get_shed (Some(waypoints, local_unaltered)) eno pse.assocs) (root_items@pse.assocs))
                let predictor_graph = (pse.priv_exec_pc, (PGE_wp(CBG_track(g_primpc_F, pse.priv_exec_pc, eno), waypoints))::list_flatten predictor_graph) 
                //let _ = report_work_summary ("saveit:" + m00) !m_work
                let scheduled_edge = // OLD:  This adds little over the pse!
                    { dend=            pse.dend;
                      maj_root_pc=     pse.maj_root_pc
                      priv_start_pc=   pse.priv_start_pc
                      priv_exec_pc=    pse.priv_exec_pc
                      priv_last_pc=    pse.priv_last_pc
                      edge=            edge
                      work=            work
                      waypoint=        wp
                      pse=             pse
                    }
                (tabdata, scheduled_edge, predictor_graph)
        
    let (tabdata, sw3, predictor_graph) = List.unzip3(map process_edgeshed sheds) //OLD: Do reads of operands before writes of results.
    let banner = title + (if settings.pipelining then sprintf ": " else "") 
    let table = create_table(banner, [ "pc"; "eno"; "Phaser"; "Work" ], root_tabdata @ list_flatten tabdata)
    let t1 = !keymsgs @ ("Schedule for " + title) :: table   
    mutadd settings.tableReports t1
    aux_report_log 2 "restructure2" t1
    //let _ = app (vprintln 2) t1
    (rootwork, sw3, predictor_graph)    // end of res2_apply_index


// Do run-length encoding of sid identifiers since otherwise can be very long.
// Also reverse, since held with deepest cut listed first.
let sidToStr arg =
    let i2s_lzp n = if n=0 then "" else i2s n
    let rec ss sf = function
        | []   when sf=None -> ""
        | []                -> i2s_lzp(snd(valOf sf))+fst(valOf(sf))
        | h::t when sf=None         -> ss (Some(h,1)) t
        | h::t when fst(valOf sf)=h -> ss (Some(h, snd(valOf sf)+1)) t
        | h::t                      -> ss (Some(h,1)) t + i2s_lzp(snd(valOf sf))+fst(valOf(sf))
    ss None arg
    



//
// Stall corrector for a thread - keeps track of whether a major state has stalled, given that it contains some variable latency operations.
// Some major states have no variable latency operations and cannot stall.  Others do not need stall correction information since they
// have no fixed-latency operators that span a variable-latency stall point.
//
type rel_stall_corrector_t (thread) = class // constructor - need one per thread.
   let entries = new rel_stall_corrector_dict_t();
   let hwm= ref -1;
   member x.lookup v = 
        if !hwm < v then
            let install_tracker n =
                    let trk = simplenet(xToStr thread + sprintf "_trk%i" n)
                    let _ = entries.Add(n, trk)
                    ()
            let _ = app install_tracker [!hwm + 1 .. v]
            hwm := v
            ()
        let (found, ov) = entries.TryGetValue v
        let _ = cassert(found, "found")
        ov

    member x.wireup (extra_nets, m_extra_spog) stall_net clear_net npc start_n_lst =
        let stall_bar = xi_not(xi_orred stall_net)
        let clear_bar = xi_not(xi_orred clear_net)            
        let wireit n =
            let (found, ov) = entries.TryGetValue n
            let _ = cassert(found, "found")
            let writeit_majstate cc start_n =
                // Set a stall noted flop 
                ix_or cc (ix_and stall_bar (ix_deqd (ix_minus npc (xi_num start_n)) (xi_num n)))
            let trkit = List.fold writeit_majstate X_false start_n_lst
            let _ = mutadd m_extra_spog (false, ov, [], xi_blift (ix_and clear_bar trkit))
            let _ = mutadd extra_nets ov
            ()
        let _ =
            if !hwm >= 0 then
                let clearer =
                    let clearge cc start_n = ix_or cc (ix_deqd npc (xi_num start_n)) // not if sticking with old pc TODO fix
                    List.fold clearge X_false start_n_lst
                let _ = mutadd m_extra_spog (true, clear_net, [], xi_blift clearer)
                app wireit [0..!hwm]
        ()
        
end


//
// Estimate execution time based on edge length and visit ratio.
//
let res2_executiontime_metric ww settings bounders thread arc_freqs codings =
    let estimate cc (pse:proto_scheduled_edge_t) =
        let freq =
            match (arc_freqs:Map<int, float>).TryFind pse.eno with
                | None->
                    hpr_yikes(sprintf "No edge visit ratio estimate stored for E%i" pse.eno)
                    1.0
                | Some freq -> freq
            
        let (textual_hwm, len) =
            match op_assoc pse.eno !bounders.xition_summaries with
                | None -> sf ("lost summary for edge " + i2s pse.eno)
                | Some high -> high
        let pad = if settings.pipelining then (if nonep pse.asio then 0 else (valOf pse.asio).post_pad) else 0
        //vprintln 3 (sprintf "res2_executiontime_estimate: High pad=%i  len=%i freq=%f %A" pad len freq textual_hwm)
        cc + double(len+pad) * freq
    let estimate = List.fold estimate 0.0 codings
    estimate



// Create a report table for the new PC values.
let report_table2 ww settings bounders thread codings =
    let hdr = [ "gb-flag/Pause"; "eno";  "Root Pc"; "hwm"; (if settings.pipelining then "PostPad" else "Exec"); "Reverb"; "Start"; "End"; "Next" ]
    let ant_pc_tos = function
        | None -> "---"
        | Some lst -> sfold i2s lst
    let coding_report (pse:proto_scheduled_edge_t) =
        let (textual_hwm, len) = valOf(op_assoc pse.eno !bounders.xition_summaries)
        let reverb = if nullp pse.reverb_state_names then "" else i2s(length pse.reverb_state_names)
        [ xToStr pse.resume;
          i2sm pse.eno; 
          i2sm pse.maj_root_pc;
          textual_hwm;
          (if settings.pipelining then (if nonep pse.asio then "" else i2s (valOf pse.asio).post_pad) else i2sm pse.priv_exec_pc);
          reverb;
          i2sm pse.priv_start_pc; i2sm pse.priv_last_pc; i2sm pse.dest_pc
        ]
    let sp ((a:proto_scheduled_edge_t)) ((b:proto_scheduled_edge_t)) =  a.maj_root_pc - b.maj_root_pc // Sort for a nice table.
    let table = create_table (sprintf "PC codings points for %s " (xToStr thread),  hdr, map coding_report (List.sortWith sp codings))
    aux_report_log 2 settings.stagename table
    mutadd settings.tableReports table
    ()
    //app (vprintln 2) table


//
// Polyhedral address mapper place-holder code - does nothing so far. Instead we will use the Xbloop constructs for bounded loops in bevelab or systolic.
//
let res2_poly_project_thread ww (settings:restruct_manager2_t) threads =
    let toolname = settings.stagename
    let vd = settings.vd
    let bof (thread, (director, arcs_revd)) cc =
        cassert(thread <> gec_X_net "", "non sequential logic")
        let tid = xToStr thread
        let ww = WF vd "res2_poly_project_thread" ww ("thread=" + tid)
        let arcs3 = (*map backfill *)(rev arcs_revd)
        let hardf = { g_default_visit_budget with hardf=false  }// A rebuilt FSM will not have tight/hard-real-time behaviour.  This flatten and Shannon project creates longer schedules. Poor really.
        let (fsm, arcs_other_machines__, old_trajectory_, stateNames) = project_arcs_to_fsm ww "polyh" vd settings.avoid_dontcare_stall thread hardf arcs3
        // TODO Keep arcs_other_machines to increase next step performance ...
        cc
    List.foldBack bof threads []

type pse_map_t = Map<int, proto_scheduled_edge_t>

//
// arcate_thread: schedule and resolve hazards for all arcs of one thread.
// Returns a newopx_t list for that arc.
//    
let res2_arcate_thread_old ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, (inverted_index:ListStore<int,int>), isFromStartState, all_work_this_thread) oraclef arc_freqs =
    let toolname = settings.stagename
    let vd = settings.vd
    let tid = xToStr thread
    let ww = WF vd "res2_arcate_thread" ww ("thread=" + tid)
    let _ = cassert(thread <> gec_X_net "", "no sequencer name")

    let m_fatal_marks_ = ref [] // Old way (pipelined=disable) never fails
    
    let perthread =
            {
                thread=                 thread
                vm2_ids=                vm2_ids
                thread_fsems=            thread_fsems
                thread_str=             xToStr thread
                //itembudget=             new per_thread_structure_count_limit_store_t("itembudget")
                mapping_log_for_resume= new mapping_log_for_resume_t("mapping_log_for_resume")
                mapping_log_for_edge=   new mapping_log_for_edge_t("mapping_log_for_edge") 
                director=               director


                m_thread_fu_summary_report= ref []
                maj_resources=          ref None //  new local_resources_collection_t("resource_pool") // ALUs, RAMs and so on allocated so far on this edge
                edge_resources=         ref None // 
                m_max_state_time=       ref -1 // not used in old
                m_next_stritem_no=      ref 1
                item_no_dbase=          new OptionStore<string, int>("item_no_dbase")
                pipate=                 (new pipate_cache_t("pipate_cache"), ref []) 
            }


    let all_minor_times = // OLD
        let scon1 (resume, edges) = 
            let whence = lc_atoi32 resume
            let title = ("res2: scon1: nopipeline: Thread=" + tid + " state=" + xToStr resume)
            if vd >= 5 then vprintln 5 (title + " start")
            let msg = title
            let maj_resctl = clone2_for_resume perthread bindpasso resume
            let walker_ss_gen = gen_walker_gen ww settings bindpasso bounders (thread, perthread, director) resume msg title oraclef m_fatal_marks_
            if vd >= 5 then vprintln 5 (title + " walker gen")
            let (root_min_scheduled, sheds, root_get_shed, root_hwm) = res2_minor_times_old ww settings bindpasso msg perthread title (walker_ss_gen, maj_resctl) edges
            if vd >= 5 then vprintln 5 (title + " minor times done")
            (lc_atoi32 resume, (resume, root_min_scheduled, title, edges, whence, root_get_shed, root_hwm, sheds))
        map scon1 all_work_this_thread

    if vd >= 5 then vprintln 5 (tid + " all scon1 done")
#if OLD
    let total_executiontime =
>>>>>>> f505a6e0e938d0795a62a44d69837bc30b2b2fcd
        let performget cc (rno, (resume, root_min_scheduled, title, idxd_edges, whence, root_get_shed, root_hwm, sheds)) =
            let pg1 cc (shed_style_info_, min_scheduled, edge, get_shed, sty) =
                match sty with
                    | SS_old _ -> 0.0
                    | SS_new(pse_, asi) ->
                        let _:vliw_arc_t = edge
                        let b2:int = asi.control_length + asi.post_pad 
                        let freq =
                            match (arc_freqs:Map<int,float>).TryFind edge.eno with
                                | None -> sf "L3329 arc freqs failed for schedulling priority baseline"
                                | Some freq -> freq
                        let del = b2 // rd_hwm + wr_hwm + 1
                        let timer = float(del) * freq
                        vprintln 0 (sprintf "  old perfget rno=%i eno=E%i del=%i freq=%f time=%f" rno edge.eno del freq timer)
                        // To find total_executiontime metric we must multiply the arc frequency with its execution time.
                        cc + timer
            List.fold pg1 cc sheds

        let total_executiontime = List.fold performget 0.0 all_minor_times
        if settings.pipelining then dev_println (sprintf "performget total_executiontime=%f" total_executiontime)
        total_executiontime
#endif
    let old_cost_ = 0.0
    (None, all_minor_times, perthread, old_cost_) // end of res2_arcate_thread_old 

//--------------------------------------------------------------------------------------------
// Make ASCII art plot of the schedule.  Two characters per row describe the operations on an FU.
let ascii_art_plot ww (perthread, root_get_shed, sheds, title, root_hwm) =
    let control_length = root_hwm+1
    let max_x = 180
    let max_y = 180
    let blank_slot_string = ". "
    let plot_1 = Array2D.create max_y max_x blank_slot_string
    let plot_2 = Array.create max_y (111, "-") // Uninitialised slots display as edge no 111 and mode - 
    let (m_ordinate, m_offgrid, m_index) = (ref 0, ref false, ref [])
            
            // // for y in 0 .. 0+root_hwm do Array.set plot_2 y (100, "R") done  // Letter R and number 100 denote the control guard/root region.

    let ppoint y x ch =
                if x >= max_x || y>= max_y then
                    vprintln 3 (sprintf "ppoint plot off the grid %i %i" x y)
                    m_offgrid := true
                else
                    let ov = plot_1.[y, x]
                    let ns = // Resolution function for more than one operation on an FU at a time step.
                        if ov.[0] = '.' then ch + " "
                        elif ov.[1] <> ' ' then sprintf "%s!" ch
                        else ov.[0..0] + ch
                    Array2D.set plot_1 y x ns
                
    let plot_edge_d2 (stringo, min_Scheduled, (edge:vliw_arc_t), get_shed, shed_style) =
                let (post_pad, plotting_length) =
                    match shed_style with
                    | SS_old(_, rd_hwm, _) -> (0, rd_hwm+1) // Write schedule for unpipelined is not plotted - we are no longer interested in that mode but could add the code here.
                    | SS_new(_, asi)       -> (asi.post_pad, max ((valOf !asi.m_asiso).minor_shed_hwm + 1) (control_length+asi.post_pad))
                    | SS_abort ss          -> sf "Attempt to print out an aborted schedule."
                let rbase = !m_ordinate

                // We plot the root for each edge, so it gets repeated that number of times, but only the control entries in the root will be generally be shared over each root plot.
                let padchar y = // Describe the nature of a time step: control, post pad or reverb
                    if y  < rbase+control_length then "c" elif y < rbase+control_length+post_pad then "p" else "r"
                for y in rbase .. rbase+plotting_length - 1 do Array.set plot_2 y (edge.eno, padchar y) done

                let d3 ikey lst =
                    let x = item_no perthread ikey
                    let d4work (_, ff, tt, _) =
                        let _ = ppoint (rbase+ff) x  "G" // Start (go)
                        let _ = ppoint (rbase+tt) x  "H" // End   (halt)           
                        ()
                    let _ = app d4work !lst
                    ()

                let draw_mgr (ikey, str_methodgroup) =
                    let rootshed = !(root_get_shed false ikey)
                    let x = item_no perthread ikey
                    let _ =
                        if nonep(op_assoc x !m_index) then
                            mutadd m_index (x, (ikey, sprintf "Item no %i is %s" x ikey))  // This is as close we get to reporting the item_no_database at the moment
                    let draw_hk = function
                        | (eno_, HK_rel(rpc, tag)) ->
                            let _ = ppoint (rbase+rpc) x (tag.[0..0]) //"H"
                            ()
                        //| HK_abs(blst, rel_) -> ()
                        | other -> sf (sprintf "draw_hk: Did not expect hk other %A" other)

                    let d4root (_, ff, tt, newopx) =
                        let _ = ppoint (rbase+ff) x  "x"
                        let _ = ppoint (rbase+tt) x  "y"                    

                        let _ = app draw_hk newopx.housekeeping
                        ()

                    app d4root rootshed
                    ()

                // We plot each resource twice and there are two columns per resource in the plot.
                app draw_mgr !perthread.m_thread_fu_summary_report //for vv in perthread.(():OptionStore<string, minor_shed_t>) do draw vv.Key !vv.Value done
                app (fun (ikey, smg_)->d3 ikey (get_shed false ikey)) !perthread.m_thread_fu_summary_report
                m_ordinate := rbase + plotting_length + 2 // spacer of 2 between edge plots - should be virgin
                ()
                
    let _ = app plot_edge_d2 sheds
    let index = List.sortWith (fun a b -> fst a - fst b) !m_index

    let edict ss = vprintln 1 ss
    
    let printout() = // Render the tableaux
        let limit_x = !perthread.m_next_stritem_no
        if limit_x > max_x then
            edict (sprintf "Two wide to print %i/%i" limit_x max_x)
        else    
        let key_lines =
                    let indexed_index = zipWithIndex index
                    let last_one = snd(hd(rev indexed_index))
                    let gen_key_line y =
                        let keygen ((x, (short_key, long_key)), tab_index) cc =
                            (if strlen short_key <= y then "  " else sprintf "%c " short_key.[y]) + (if tab_index%5=4 && tab_index<>last_one then "  " else "") + cc
                        List.foldBack keygen indexed_index ""
                    map gen_key_line [0..12] // Print first 13 chars of ikey with vertical tabulation.
        let _ = edict ("ASCII art schedule plotout for " + title + (if !m_offgrid then " Note: SOME ENTRIES NOT SHOWN SINCE OUTSIDE PLOT SPACE !" else ""))


        let bang_y y =
            if y>=max_y then
                edict("... ... ... ... clipped ... ... ...")
            else
                    let line_is_blank = fst plot_2.[y] = 111 && conjunctionate (fun x -> plot_1.[y, x] = blank_slot_string) [0..limit_x-1] // All lines marked 111 shouuld be blank, but we check anyway in case of bugs.
                    let bang_x cc x =
                        if x >= max_x || y>= max_y then
                            vprintln 3 (sprintf "printout ppoint plot off the grid %i %i" x y)
                            m_offgrid := true
                            "... " + cc
                        else
                            //dev_println (sprintf " %i %i point" x y)
                            let c = if line_is_blank then "  " else plot_1.[y, x] // replace blank line with all spaces...
                            cc + c + (if x%5=4 && x<>limit_x-1 then "  " else "") 
                    let line = List.fold bang_x "" [0..limit_x-1]
                    // The format of a line is the edge number, the mode char for that cycle and the FU operation map.
                    edict(sprintf "%3i %s "  (fst plot_2.[y]) (snd plot_2.[y]) + "|" + line + "|")
                    ()
        for y in 0 .. !m_ordinate-1 do bang_y y done
        app (fun keyline -> edict(sprintf "key   |%s|" keyline)) key_lines
        edict(sfoldcr_lite (fun (n, (short_key, long_key))->long_key) index)
    if nullp index then edict("No FUs in minor schedule for ascii-art plot " + title)
    else printout()
    ()
                
                
//--------------------------------------------------------------------------------------------
// NEW (aka pipelined!=disable)

// All writes within a single arc must be after all reads of that resource. This is the basic RTL parallel/synchronous assignment paradigm.
// With respect to earlier ghost arcs that are still running or later ones that overlap our current arc: Our read must, of course, be after all earlier ghost writes or forwarded by piggybacking on their write sites.     
// And a write must be before a subsequent ghost's read, which we can always locally ensure by extending our post_pad
// or we can allow it to be simultaneous if we ensure that ghosts are modified to instead use a value we forward to it.
// Owing to our constructive approach, we only need worry about arcs that are already schedulled within this thread's global attempt.



    
     
let res2_minor_times_new_roots ww0 settings bindpasso msg title walker_ss_gen post_pad_map edges = 
    let ww = WF 3 msg ww0 ("Start (new roots) " + title)
    let vd = settings.vd
    let n_edges = length edges
    let null_ghosting_ff = [] // For root there are no ghost considerations
    let (root_min_scheduled, walkerS_root, root_get_shed, _, root_rd_hwm, _) = walker_ss_gen (true, -1) msg null_ghosting_ff true bindpasso // Use root schedule for xition control
    //let _ = app (fun (edge:vliw_arc_t, eno) -> vprintln 0 (sprintf "control guard %i is %s" eno (xbToStr edge.gtop))) edges
    let clkinfo = g_null_directorate // not being used here.    
    let scoreboard_starting_entries =
        let gtop_shed (edge:vliw_arc_t) =
            let post_pad =
                match (post_pad_map:Map<int,int>).TryFind edge.eno with
                    | None -> settings.extra_post_pad
                    | Some v ->
                        let _ = vprintln 3 (sprintf "Schedule attempt with post_pad for %i as %i" edge.eno v)
                        v

            if vd >= 5 then vprintln 5 (sprintf "root_hwm before schedulling eno=E%i is %i.  gtop=%s" edge.eno !root_rd_hwm (xbToStr edge.gtop))
            let _ = mya_bwalk_it ("root", walkerS_root, None, ref []) clkinfo edge.gtop
            if vd >= 5 then vprintln 5 (sprintf "root_hwm after schedulling eno=E%i is %i" edge.eno !root_rd_hwm)
            {
                eno=             edge.eno
                edge=            edge
                control_length=  -1
                root_get_shed=   root_get_shed
                //total_length=    -1
                m_asiso=         ref None
                post_pad=        post_pad
            }

        map gtop_shed edges // Schedule resources used in all xition guards

    // Now we know the re-initiation interval for this major state.

    let root_hwm = // All edges on this resume have the same control_length in the current approach
        if vd>=5 then vprintln 5 (title + sprintf ": Scheduled root/control logic for this resume with %i edges. root_hwm=%i" (length edges) !root_rd_hwm)
        !root_rd_hwm

    let scoreboard_starting_entries =
        let fx asi = { asi with control_length= root_hwm+1 }
        map fx scoreboard_starting_entries

    let ww = WF 3 msg ww0 "Done"      
    (scoreboard_starting_entries, root_min_scheduled, root_get_shed, root_hwm) // res2_minor_times_new_roots

    
let res2_minor_times_new_arcwork ww0 settings bindpasso bounders msg title perthread (walker_ss_gen, maj_resctl)  (asi_board, min_e, max_e) ghosting_f guard_rdy m_fatal_marks edges = 
    let ww = WF 3 msg ww0 ("Start (new arcwork) " + title)      
    let vd = settings.vd
    // We invoke the planopt oracle to help search for a good design.
    let dummy_min_shed = new OptionStore<xidx_t,starttime_t>("dummy-do-not-use")
    let dummy_get_shed _ _ = ref []

    let schedule_edge (edge:vliw_arc_t) =

        if not_nullp !m_fatal_marks then
            // Short-cut for the rest of the iteration.
            vprintln 3 (sprintf "Skip %i on earlier abort" edge.eno)
            (Some "new_arcwork skipped/aborted", dummy_min_shed, edge, dummy_get_shed, SS_abort "new_arcwork skipped/aborted")

        else
        clone2_for_edge perthread bindpasso edge.eno maj_resctl
        let m_pli_lst = ref []
        let pliwalk_fun support_times = function
            | XIRTL_pli(pp, g, fgis, args_raw) ->
                // There are no structual count limits on PLI calls, so we can rez as many as we like.  We do need to pad out their args however.
                let support_times2 = List.foldBack deton support_times []
                let qq = vl_max_times2 false support_times2
                let (g_bo, args_bo) = buildout settings 0 perthread.thread_fsems (g, guard_rdy) qq (List.zip args_raw support_times2)
                if vd >= 5 then vprintln 5 (sprintf "Shove PLI rdytimes=%A  fgis=%A args=%A" support_times2 fgis args_bo)
                mutadd m_pli_lst (qq, XIRTL_pli(pp, g_bo, fgis, args_bo))
                ()

        let ghosting_set = ghosting_f edge.eno
        let _ =
            let print_ghosting_set (offset, (asi:saved_arc_schedule_t), get_shed) = vprintln 3 (sprintf "  ghosts are : offset=%i eno=E%i" offset asi.edge.eno)
            app print_ghosting_set ghosting_set
        let asi = (asi_board:saved_arc_schedule_t array).[edge.eno-min_e]
        let edge = asi.edge// have it already in fact, for now.
        let (min_scheduled, walkerS2, get_shed, copyback, rd_hwm, wr_hwm_) = walker_ss_gen (false, edge.eno) msg ghosting_set false bindpasso  // Clone for each edge
        if vd >=5 then vprintln 5 (msg + sprintf ": schedule/walk work edge start eno=E%i. cmd count=%i" edge.eno (length edge.cmds))
        if vd >= 5 then vprintln 5 (msg + sprintf ": schedule/walk work edge start eno=E%i. cmd count=%i" edge.eno (length edge.cmds))
        dev_println("about to walk edges " + sfoldcr_lite xrtlToStr (edge.cmds))
        let ii = { id="temporary-rewrap" } : rtl_ctrl_t
        let _ = walk_sp ww (vd>=5) (sprintf "m_%i" edge.eno, walkerS2, None, pliwalk_fun) g_null_directorate (SP_rtl(ii, edge.cmds))
        copyback()
        if vd >= 5 then vprintln 5 (msg + sprintf ": schedule/walk edge done eno=E%i" edge.eno)

        asi.m_asiso := Some{  minor_shed_hwm= !rd_hwm; xminor_shed=get_shed false }
        // We need to record the maximum state time to know how back in time to scan for ghostings that may structurally interfere if not avoided by newly shedulled work.
        perthread.m_max_state_time := max !perthread.m_max_state_time (asi.control_length + !rd_hwm + 1)

        let fab_pli_opx2 (((startarcf_, edge_grd, _, nn), plitem), sidx) =
            let fence_ordering = if settings.extend_for_pli_mode <> "disable" then get_pli_fence "res2" plitem else [] // TODO finer control needed here...
            // The pli_ikey needs to be the state edge ordering domain where there is one.
            dev_println(sprintf "Fence ordering in use is %s" (sfold orderToStr fence_ordering))
            let pli_ikey =
                match fence_ordering with
                    | [] -> sprintf "pli%i_%i" edge.eno sidx // was called fabbed pli in the past - a pseudo ikey for a PLI call.
                    | [(domain, _, _, _)] -> domain
                    | doms -> sf ("Two or more fence ordering domains apply at once." + sfold orderToStr doms)
            let str2 =
                        { g_null_str2 with
                                mkey=                pli_ikey
                                ikey=                pli_ikey
                                item=                SR_pliwork
                                kkey=                "SR_pliwork_kkey"
                                clklsts=             [perthread.director]
                                area=                0.0 // Assumed to be fully stripped away in logic synthesis for now
                        }
            let mg = { g_null_methodgroup with mg_uid=pli_ikey; dinstance=str2 }
            mutadd perthread.m_thread_fu_summary_report (pli_ikey, mg)
            if pli_ikey = "" then sf "splat-L4617" else bounders.global_mgrs.add pli_ikey mg                    
            bounders.global_resource_thread_use.add str2.ikey (xToStr perthread.thread)
            dev_println (sprintf "eno=E%i  edge_grd=%s:  fences=%s:   About to merge edge guard in fabbed PLI opx %s" edge.eno (xbToStr edge_grd) (sfold orderToStr fence_ordering) (xrtlToStr plitem))

            let opx =
                match plitem with
                    | XIRTL_pli(pp, action_grd0, ((fname, gis), ordering), args) ->
                        vprintln 3 (sprintf "Add (shove/fabbed) pli item ikey=%s  %s(%s)  edge_grd=%s  action_grd0=%s  at rel %i fences=%s" pli_ikey fname (sfold xToStr args) (xbToStr edge_grd) (xbToStr action_grd0) nn (sfold orderToStr fence_ordering))
                        let action_grd = ix_and edge_grd action_grd0 // This AND better as spog or andl
                        {  g_null_newopx with
                             mkey=            "XXPLI"
                             cmd=             fname
                             plimeta=         Some((fname, gis), ordering)
                             fence_ordering=  fence_ordering

                           // TODO dont merge guards here?
                             //action_grd=   action_grd
                           // we only need the edge guard if inside the root region and these are fairly TODO
                             serial=       fresh_opx_serial()
                             args_raw=     (action_grd, args)
                             g_args_bo=    ref None
                             housekeeping= [ (edge.eno, HK_rel(nn, "0-pli"))]
                             customer_edges=[edge.eno]
                             method_o=     None
                        }
                    | other -> sf(sprintf "L5286 other")                    
            let item:newop_t = ((false, edge.eno), nn, nn, opx)
            let shed = get_shed false pli_ikey
            mutadd shed item
            ()
            
        app fab_pli_opx2 (zipWithIndex (rev !m_pli_lst))
        
        (Some "newarcwork", min_scheduled, edge, get_shed, SS_new(ref None, asi)) // can return the asi if you like
    let sheds = map schedule_edge edges
    let aborted = not_nullp !m_fatal_marks
    let ww = WF 3 msg ww0 (if aborted then "Attempt aborted" else "Done")
    (sheds) // res2_minor_times_new_arcwork

// arcate_thread: schedule and resolve hazards for all arcs of one thread.
// Returns a newopx_t list for that arc.
let res2_arcate_thread_new_attempt ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread) oraclef arc_freqs post_pad_map =
    let toolname = settings.stagename
    let vd = settings.vd
    let tid = xToStr thread
    let ww = WF 3 "res2_arcate_thread_new" ww ("Start. thread=" + tid)
    cassert(thread <> gec_X_net "", "no sequencer/thread name")

    let _:ListStore<int,int> = earlier_arc_database
    let _:ListStore<int,int> = later_arc_database
    let m_fatal_marks = ref []
    let perthread =
            {
                thread=               thread
                vm2_ids=              vm2_ids                
                thread_fsems=         thread_fsems
                thread_str=           xToStr thread
                //itembudget=           new per_thread_structure_count_limit_store_t("itembudget")
                director=             director
                mapping_log_for_resume= new mapping_log_for_resume_t("mapping_log_for_resume")
                mapping_log_for_edge=  new mapping_log_for_edge_t("mapping_log_for_edge")                

                //thread_fu_counters=   rez_structure_count_limits settings // Allocate fresh thread resource limits
                m_thread_fu_summary_report= ref []
                maj_resources=        ref None //  new local_resources_collection_t("resource_pool") // ALUs, RAMs and so on allocated so far on this edge
                edge_resources=       ref None
                m_max_state_time=     ref -1
                m_next_stritem_no=    ref 1
                item_no_dbase=        new OptionStore<string, int>("item_no_dbase")
                pipate=               (new pipate_cache_t("pipate_cache"), ref []) 
            }


    // All roots are schedulled first and owing to control hazard aspects, they have no ghostings to worry about and they are read-only so have no writeback aftereffects.
    let (asi_lst_lst, ctrl_minor_times) =
        let new_scon1_roots (resume, edges) = 
            let whence = lc_atoi32 resume
            let title = ("res2: Thread=" + tid + " new rootwork state=" + xToStr resume)
            let msg = "new-rootwork"
            let walker_ss_gen = gen_walker_gen ww settings bindpasso bounders (thread, perthread, director) resume msg title oraclef m_fatal_marks
            let _ = clone2_for_resume perthread bindpasso resume // This should be rolled into gen_walker_gen
            let (asi_lst, root_min_scheduled, root_get_shed, root_hwm) = res2_minor_times_new_roots ww settings bindpasso msg title walker_ss_gen post_pad_map edges
            (asi_lst, (lc_atoi32 resume, resume, (walker_ss_gen, valOf !perthread.maj_resources), root_min_scheduled, title, edges, whence, root_get_shed, root_hwm))
        List.unzip(map new_scon1_roots all_work_this_thread)

    let ww = WF 3 "res2_arcate_thread_new" ww ("All controlflow (aka roots) complete for thread=" + tid)

    let (asi_board, min_e, max_e) = // Here we sort so array can be indexed on eno (assumed consecutive)
                    // Later we should sort according to use frequency with minor perurbations specified by the oracle. TODO
        let sel (asi:saved_arc_schedule_t) = asi.eno
        let sorted = List.sortWith (fun a b->sel a - sel b) (list_flatten asi_lst_lst)
        let min_e = List.fold (fun sofar asi -> min sofar (sel asi)) (sel(hd sorted)) (tl sorted)
        let max_e = List.fold (fun sofar asi -> max sofar (sel asi)) (sel(hd sorted)) (tl sorted)
        let max_e2 = length sorted + min_e - 1
        vprintln 3 (sprintf "Edge range is %i to %i (%i)" min_e max_e max_e2)
        if max_e <> max_e2 then sf (sprintf "Edge numbers not consecutive for array indexing")
        (List.toArray sorted, min_e, max_e)

    let _ = vprintln 0 (sprintf "Start gecc ghosting_f")

    let ghosting_f eno00 =  // The offset is a relative microstate offset from the majorstate baseline
        // We need to check for structural and data hazards against ourselves, and ghostings.
        // Ghostings are time-shifted postcursors of previous and future arc schedules (that will often include ourself), but we only need
        // to check against already schedulled operations, since future decisions will avoid whatever is already done.
        // Since the ghosting complexity can be exponential, we compute it lazily rather than projecting too far into the future.

        let clip_at = !perthread.m_max_state_time // TODO - make sure this is the dynamic live value, not a value from a closure.

        let add_ghost_once (eno, offset, asi, asiso) lst =
            let rec not_present_already_check = function
                | [] -> true
                | (e, o, _, _)::_ when e=eno && o=offset -> true
                | _::tt -> not_present_already_check tt
            if not_present_already_check lst then (eno, offset, asi, asiso)::lst else lst

        let ghost_f eno00  =
            // Earlier ghosts depend on the resume number, but that has been pre-mapped into earlier_arc_database.
            // An earlier or later one may or may not already have been schedulled, but we need to consider its predecessors and successors regardless.
            let earlier_ghosts eno00 cc =
                let rcheck = new OptionStore<int * int, bool>("rcheck")
                let rec eg1 backset cc eno0 = List.fold (eg2 eno0 backset) cc (earlier_arc_database.lookup eno0)
                and eg2 eno0 backset cc eno =
                    match rcheck.lookup (eno, backset) with
                        | Some _ -> cc
                        | None ->
                            let _ = rcheck.add (eno, backset) true
                            let asi = asi_board.[eno-min_e]
                            let backset = backset - asi.control_length - asi.post_pad 
                            let _ = vprintln 3 (sprintf "  For eno=E%i, consider eno=E%i with backset %i" eno0 eno backset)
                            let cc = if eno=eno00 then cc else add_ghost_once (eno, backset, asi, !asi.m_asiso) cc

                            // The post_pad needs to be known before an arc is schedulled to get spacing of its predecessors.
                            if backset < -clip_at then // <---- WRONG NEED TO CONSIDER EARLIER ONE STILL WHOSE REVERB IS WITHIN THIS LIMIT?
                                let _ = vprintln 3 (sprintf "  For %i, consider eno=E%i with backset %i: too far back in history" eno0 eno backset)
                                cc
                            else 
                                //recurse on it too
                                eg1 backset cc eno
                let ans = eg1 0 cc eno00 
                ans
            // Later ghosts depend on the edge number (whereas earlier ghosts depend on the resume number) but this has already been considered in the later/earlier arc database.
            let rec later_ghosts eno00 cc = 
                let rcheck = new OptionStore<int * int, bool>("rcheck")
                let rec lg1 fwdset cc eno =
                    if fwdset > clip_at then
                        //let _ = vprintln 3 (sprintf "consider eno=E%i with fwdset %i: too far in future" eno fwdset)
                        cc
                    else match rcheck.lookup (eno, fwdset) with
                            | Some _ -> cc
                            | None ->
                                let _ = rcheck.add (eno, fwdset) true
                                //let _ = vprintln 3 (sprintf "consider eno=E%i with fwdset %i" eno fwdset)
                                let asi = asi_board.[eno-min_e]
                                if asi.control_length < 0 then sf ("All control states should currently be schedulled before we need ghostings. lg2")
                                let cc = if eno=eno00 then cc else add_ghost_once (eno, fwdset, asi, !asi.m_asiso) cc
                                let fwdset = fwdset + asi.control_length + asi.post_pad
                                let later_arcs = later_arc_database.lookup eno
                                List.fold (lg1 fwdset) cc later_arcs  //recurse on it too
                let our_edge  = asi_board.[eno00-min_e]
                lg1 (our_edge.control_length + our_edge.post_pad) cc eno00

            let ghosts = earlier_ghosts eno00 (later_ghosts eno00 [])
            let rec select1 cc = function
                | (eno_, offset, asi, Some asis) ->
                    let cc = (offset, asi, asi.root_get_shed false)::cc
                    //!(asi.root_get_shed false ikey)
                    let cc = (offset, asi, asis.xminor_shed)::cc // Contains root anyway!
                    // !(asi.root_get_shed false ikeyasi.root_get_shed false ikey)
                    cc
                | (eno_, offset, asi, None) ->
                    let cc = (offset, asi, asi.root_get_shed false)::cc //!(asi.root_get_shed false ikey)
                    cc
            let better_ans = List.fold select1 [] ghosts
            better_ans
        // We return a function that given an eno and an ikey gives ghost schedules
        let _:(int -> (int * saved_arc_schedule_t * (string -> newop_t list ref)) list) = ghost_f // Prefer not to have the ref in this and need it paired with offsets. Or better  lazy function of offset.
        let _ = vprintln 0 (sprintf "Start gecc ghosting_f 2")
        let ans = ghost_f eno00 
        let _ = vprintln 3 (sprintf "Ghosting schedule (clip_at=%i) at eno %i is ^%i" clip_at eno00 (length ans))
        ans

// TODO - classical HLS loop forwarding from ghosts etc..

    let bixo = Some(asi_board, min_e, max_e)

    // Prefer not to have the ref in this and need it paired with offsets. Or better a lazy function of offset.

    let _:(int -> (int *  saved_arc_schedule_t * (string -> newop_t list ref)) list) = ghosting_f // put a token to skip duplicates in too please.
    
    let all_minor_times =
        let new_scon1_roots (resume_i, resume, (walker_ss_gen, maj_resctl), root_min_scheduled, title, edges, whence, root_get_shed, root_hwm) =
            //let whence = lc_atoi32 resume // TODO do not need to recompute whence when we have resume_i as well?
            let title = ("res2: Thread=" + tid + " new arcwork state=" + xToStr resume)

            if not_nullp !m_fatal_marks then
                // Short-cut 1 for the rest of the iteration.
                (resume_i, (resume, root_min_scheduled, title, edges, whence, root_get_shed, root_hwm, [](*SS_abort "new_scon1_roots"*)))
            else
            let guard_rdy = (root_hwm)
            let msg = "new-arcwork" // aka edgework



            let sheds = res2_minor_times_new_arcwork ww settings bindpasso bounders msg title perthread (walker_ss_gen, maj_resctl) (asi_board, min_e, max_e) ghosting_f guard_rdy  m_fatal_marks edges

            if not_nullp !m_fatal_marks then
                // Short-cut 2 for the rest of the iteration.
                (resume_i, (resume, root_min_scheduled, title, edges, whence, root_get_shed, root_hwm, [](*SS_abort "new_scon1_roots"*)))
            else
// START OF ASCII ART PRINT
            let _ = ascii_art_plot ww (perthread, root_get_shed, sheds, title, root_hwm)
// END OF PRINT
            // e.g. (Some "newarcwork", min_scheduled, edge, get_shed, SS_new(ref None, asi)) // can return the asi if you like
            
            //let _ = vprintln 3 (title + sprintf " so far max_state_time=%i (clip_at)" !perthread.m_max_state_time)
            (resume_i, (resume, root_min_scheduled, title, edges, whence, root_get_shed, root_hwm, sheds))

        map new_scon1_roots ctrl_minor_times 
    let aborted = not_nullp !m_fatal_marks
    vprintln 2 (sprintf " Thread %s max_state_time=%i (clip_at)" tid !perthread.m_max_state_time)

    let ww = WF 3 "res2_arcate_thread_new" ww (sprintf "All arcwork complete for thread=%s. Aborted=%A" tid aborted)
    let old_cost_ = 0.0 // no longer used?
    if aborted then vprintln 3 ("Aborted res2_arcate attempt")
    (!m_fatal_marks, bixo, all_minor_times, perthread, old_cost_) // end of res2_arcate_thread_new 


let res2_arcate_thread_new ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread) oraclef arc_freqs =

    let rec iterate_post_pads post_pad_map no = 
        let _ = vprintln 1 (sprintf "Thread=%s. Post pad iteration %i start." (xToStr thread) no)
        let (fatal_marks, bixo, all_minor_times, perthread, cost) = res2_arcate_thread_new_attempt ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread) oraclef arc_freqs post_pad_map

        if nullp fatal_marks then
            let _ = vprintln 1(sprintf "Post pad iteration %i is a success." no)
            (bixo, all_minor_times, perthread, cost)
        elif no  > 100 then sf ("Design abort: Infeasible number of fatal_marks")
        else
            let _ = vprintln 1 (sprintf "Post pad iteration %i failed." no)
            let rec elide_EP_duplicates cc = function
                | [] -> cc
                | (eno, msg, cmd, inc)::tt when cmd="EP" ->
                    let rec elide_serf messages maxinc = function
                        | [] -> (messages, maxinc, [])
                        | (e, m, p, i)::tt when e=eno && p="EP" -> elide_serf (singly_add m messages) (max i maxinc)  tt
                        | (e, m, p, i)::tt ->
                            let (messages, maxinc, tt) = elide_serf messages maxinc tt
                            (messages, maxinc, (e, m, p, i) :: tt)
                    let (messages, maxinc, tt) = elide_serf [msg] inc tt
                    (eno, messages, cmd, maxinc) :: elide_EP_duplicates cc tt
                | (eno, msg, cmd, inc)::tt -> elide_EP_duplicates ((eno, [msg], cmd, inc)::cc) tt

            let augment_arcate_settings post_pad_map = function
                | (eno, messages__, "EP", inc) ->
                    let ov =
                        match (post_pad_map:Map<int,int>).TryFind eno with
                            | None -> settings.extra_post_pad
                            | Some ov -> ov
                    let nv = ov+inc
                    let _ = vprintln 3 (sprintf "Will augment post_pad for eno %i from %i to %i" eno ov nv)
                    post_pad_map.Add(eno, nv)
                | (eno, messages, other_cmd_, inc) -> sf (sprintf "Other modification to scheduller settings not expected/supported: " + sfold (fun x->x) messages)
            let post_pad_map = List.fold augment_arcate_settings post_pad_map (elide_EP_duplicates [] fatal_marks)
            iterate_post_pads post_pad_map (no+1)

    let (bixo, all_minor_times, perthread, cost) =
        let initial_post_pad_map = Map.empty
        iterate_post_pads initial_post_pad_map 1


    (bixo, all_minor_times, perthread, cost)

// End Of New (aka pipelined!=disable)
//--------------------------------------------------------------------------------------------


//
//       
let res2_arcate_prepare ww (settings:restruct_manager2_t) (nom_thread__, (director, controllerf, vm2_ids, arcs_revd, hls_signatures)) =
    let thread_name = xToStr nom_thread__
    let ww = WF 2 "res2_arcate_prepare" ww ("Start thread " + thread_name)
    let vd = settings.vd

    let hardf = { g_default_visit_budget with hardf=false } // A rebuilt FSM will not have tight/hard-real-time behaviour.  This flatten and Shannon project creates longer schedules. Poor really.
    let arcs3 = (*map backfill *)(rev arcs_revd)
    let (fsml, arcs_other, old_trajectory_, stateNames) = project_arcs_to_fsm ww "res2_arcate_prepare" vd settings.avoid_dontcare_stall nom_thread__ hardf arcs3


    //dev_println(sprintf "Temor duid=%i %A" director.duid (length arcs_other))

    // Print out back-projected FSM
    if settings.anal.reprint && length fsml = 1 then vprintln 0 ("Global clock-domain FSM to RESTRUCTURE IS (reprint)=\n" + hblockToStr ww (H2BLK(director, hd fsml)))

    let (thread, controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread, arc_freqs) =
        let rec bix = function
            | [SP_fsm(fsm_info, arcs) as fsm] ->
                let edges_by_resume = fsm_edges_by_resume fsm
                let tid = xToStr fsm_info.pc
                let (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database) = fsm_compute_inverted_xition_mapping ww vd tid fsm
                let istart_no = lc_atoi32 (fst fsm_info.start_state)
                let isFromStartState (resume, edges) = (lc_atoi32 resume = istart_no) // Do start state first so its pc starts with zero.
                //dev_println (sprintf "res2: Partition start thread=%s" tid)
                let (starting_grp, others) = List.partition isFromStartState edges_by_resume
                //dev_println "Partition done"
                let _ = 
                    if length starting_grp <> 1 then
                        for (resume, edges) in starting_grp do vprintln 0 (sprintf " +++ arc " + sfold fsm_arcToStr edges) done
                        vprintln 0 (sprintf "not one start arc - no=%i" (length starting_grp))

                // Globally-unique edge numbers are added by abstracte and they number upwards form the distinctive origin number 800.
                let all_edges_this_thread = starting_grp @ others
                //dev_println (sprintf "Balance computation start thread=%s" tid)
                let (resume_idxof_, occupancy_ratios, arc_freqs) = balance_based_performance_prediction ww vd fsm_info all_edges_this_thread
                (fsm_info.pc, fsm_info.controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_edges_this_thread, arc_freqs)
            | other ->
                let tid = director.duid
                //dev_println(sprintf "Temor2 %A" other)                
                let handle_misc_rtl arg cc =
                    match arg with
                        | SP_rtl(ii_, lst) -> lst @ cc// Miscellaneous RTL: put it temporarily in a one-state FSM so that it has an edge no.
                        | other ->
                            hpr_yikes "Non-RTL being discarded during restructure: other SP form to restructure - not an FSM even after fsm_project."
                            cc
                let rtl = List.foldBack handle_misc_rtl other arcs_other
                vprintln 2 (sprintf "Temporary project misc RTL tid=%s of %i cmds to degenerate (controllerf=false) FSM" tid (length rtl))
                let the_resume = xi_num 0
                let hardf = g_default_visit_budget
                let fsm_info = { g_default_fsm_info with  pc=gec_X_net tid; resumes=[(the_resume, hardf)]; start_state=(the_resume, hardf); controllerf=false }
                let arc = { g_null_vliw_arc with cmds=rtl; hard=hardf; pc=fsm_info.pc; resume=the_resume; iresume=0; eno= next_global_arc_no() }
                let fsm = SP_fsm(fsm_info, [arc])
                bix [fsm]

        bix fsml

    // We sort the edges by frequency of use so a greedy algorithm can schedule the important edges first (these get first bite at the allowable silicon area or FU budgets).

    let sorted_arcs = 
        let arc_list = arc_freqs |> Map.toSeq |> List.ofSeq
        let sorted = List.sortWith (fun (a,b) (c,d) -> if b>d then 1 else -1) arc_list
        // ...
        ()

    let thread_fsems =
        match hls_signatures with
            | [] -> g_default_native_fun_sig.fsems
            | gis::tt ->
                if not_nullp tt then hpr_yikes (sprintf "discard second and further hls_signatures %s" (sfoldcr_lite signatureToStr hls_signatures))
                gis.fsems
    //dev_println  (sprintf "URGENT Thread fsems are %A" thread_fsems)  // protocols can print these
    vprintln 2 (sprintf "Thread fsems for %s are %s" thread_name (fsemsToStr thread_fsems))
    let ww = WF 2 "res2_arcate_prepare" ww "Finished"
    (thread, vm2_ids, thread_fsems, director, controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread, arc_freqs) // end of res2_arcate_prepare

//
//
let res2_arcate ww (settings:restruct_manager2_t) oraclef bindpasso bounders prepared_thread =
    let vd = settings.vd

    let (thread, vm2_ids, thread_fsems, director, controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread, arc_freqs) = prepared_thread
    let threadname = xToStr thread        
    let tid        = threadname
    let msg = sprintf "Search best design for " + tid

    let (bixo, all_minor_times, perthread, cost) =
        if settings.pipelining then
            res2_arcate_thread_new ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread) oraclef arc_freqs        
        else res2_arcate_thread_old ww (settings:restruct_manager2_t) bindpasso bounders thread vm2_ids thread_fsems director (controllerf, edges_by_resume, rev_resume_database, isFromStartState, all_work_this_thread) oraclef arc_freqs

    if nonep bindpasso then // We stop at this point when simply collating the resources that need binding.
        vprintln 3 (sprintf "arcate Early stop during bind explore pass tid=%s" tid)
        let ans = ((director, thread, controllerf, perthread, arc_freqs), None)
        ans

    else
        let enos_by_resume = map (fun (resume, edges) -> (resume, map (fun (e:vliw_arc_t)->e.eno) edges)) edges_by_resume
 
        let (xlats, codings, npc_arity) =  // Allocate npc numbers for each read and write microseq where xlat is the mapping from relative to absolute time.
                    let (xlats, codings, npc_arity) =
                        if settings.pipelining then
                            let (asi_board, min_e, max_e) = valOf bixo
                            let (xlats, codings, npc_arity) = res2_make_index_new ww settings bounders thread ([], [], 0) (asi_board, min_e, max_e) enos_by_resume
                            (xlats, codings, npc_arity)
                        else
                            let m_next_pc = ref 0
                            let new_pc_code_point allocf msg =
                                let rr = !m_next_pc 
                                if allocf then
                                    mutinc m_next_pc 1
                                    vprintln 3 (sprintf "new_pc_code_point %i for %s" rr msg)
                                rr

                            let rec scon2a (xlats, cc) = function // Forms xlats (translations from minor offsets to new pc code points).
                                | (rno, (resume, root_min_scheduled, title, edges, whence, root_get_shed, root_hwm, sheds)) ->
                                    let maj_root_pc = !m_next_pc
                                    let (root_items, codings) = res2_make_index_x ww settings bounders thread resume maj_root_pc new_pc_code_point title cc (root_hwm, sheds, root_get_shed)
                                    let xlats = (whence, (maj_root_pc, !m_next_pc-1, root_items, None))::xlats //Old to new root PC translation and control state info.
                                    (xlats, codings)

                            let (xlats, coding) = List.fold scon2a ([], []) all_minor_times
                            (xlats, coding, !m_next_pc)


                    let codings =  // Assembly step. 
                        let scon2b (pse:proto_scheduled_edge_t) =
                            let dest =
                                if pse.edge.dest=X_undef then -1
                                else
                                    let (maj_root_pc, pc_hwm, root_items, _) = valOf_or_fail "no xlat L3763" (op_assoc (lc_atoi32x pse.edge.dest) xlats)
                                    maj_root_pc
                            { pse with dest_pc=dest }
                        map scon2b codings

                    report_table2 ww settings bounders thread codings
                    let performance_metric = res2_executiontime_metric ww settings bounders thread arc_freqs codings
                    //dev_println (sprintf "executiontime_performance_metric=%f" performance_metric)
                    vprintln 3 (sprintf "Reindexed thread %s using %i new PC code points" (xToStr thread) npc_arity)
                    (xlats, codings, npc_arity)

        let stallpoints = new stallpoints_t("stallpoints") //value half of the pair is not used - used as a mutable set   
        let rec scon3 sofar pg_cc = function  // Apply npc numbers for each read and write microseq 
            | []   -> (sofar, pg_cc)
            | (rno, (resume, root_min_scheduled, title, idxd_edges, whence, root_get_shed, root_hwm, sheds))::tt ->
                //let can_go_back = valOf(op_assoc resume can_go_backs)
                let v5 = valOf_or_fail "no root_items" (op_assoc (lc_atoi32 resume) xlats)

                let (maj_root_pc, pc_hwm, _, _) = v5
                let (rootwork, se_work, predictor_graph) = res2_apply_index ww settings bindpasso bounders thread title v5 codings stallpoints (root_min_scheduled, root_hwm, sheds, root_get_shed) 
                let xc3 =
                    // It would be better just to include the se here rather than extract and render the fields positionally...
                    let gen_xition_summary (se:scheduled_edge_t) = (Filx, resume, waypoint_f se.waypoint, se.edge.eno, se.edge.gtop, lc_atoi32x se.edge.dest, (se.priv_start_pc, se.priv_exec_pc, se.dend))
                    (rootwork, map gen_xition_summary se_work)
                scon3 ((resume, se_work, xc3, maj_root_pc, pc_hwm)::sofar) (predictor_graph::pg_cc) tt
        let (majstates, predictor_graph) = scon3 [] [] all_minor_times

        let _ = 
        // report
            let majorstat_line = sprintf "Reindexed thread %s with %i minor control states" threadname npc_arity
            vprintln 2 majorstat_line
            majorstat_line_log "restructure2" majorstat_line
            ()

        let ans = ((director, thread, controllerf, perthread, arc_freqs), Some(npc_arity, majstates, predictor_graph, xlats, stallpoints))  // end of res2_arcate
        ans
    

//
// Collate a stall-correction index
//
let res2_check_stalls ww (settings:restruct_manager2_t) site (dir, thread, state) work =
    let vd = settings.vd
    let stall_correct_info = // This is the early check - but we need to spot inter-thread contention before stall correcting needs are fully complete, which we rely on being known at arcate time with no post-arcate sharing decisions..
        let mine1 start cc (eno_, shen, opxx:newopx_t) =
            match opxx.method_o with
                | None ->
                    //vprintln 0 (sprintf " not variable-latency 0  %s cmd=%s at  %i   te=%s" ("NONE") x.cmd start (shenToStr shen))
                    cc
                | Some method_str2 ->
                    let mgr =      valOf_or_fail "L4882" !method_str2.m_mgro
                    match mgr.mgr_meldo with
                        | None -> cc // Scalar most likely
                        | Some mgr_meld ->
                            if nonep mgr_meld.hs_vlat then
                               //vprintln 0 (sprintf " not variable-latency 1 on  %s cmd=%s at  %i   te=%s" (valOf opxx.s_mgr).ikey opxx.cmd start (shenToStr shen))
                               cc
                            else
                        // Both writes and reads can block on the start but only non-posted operations such as reads (and ALU results) also block on the end.
                                let times =
                                    let fx2 arg cc =
                                        match arg with
                                        | (eno_, HK_rel(rpcv, tag)) -> sf (sprintf "hk was not reindexed for " + sfold (fun (eno, hk) -> i2s eno + ":" + hkToStr "" hk) opxx.housekeeping)
                                        | (eno_, HK_abs(times, _))  -> times :: cc // Mine over all edges
                                    List.foldBack fx2 opxx.housekeeping []
                                let (cc, e_msg) =
                                    if opxx.postf then (cc, "") // no end interlock to stall on BUT CAN STILL STALL AT START -- TODO TODO - and also we need to watchout for max-outstanding overload.
                                    else
                                        let end_check_times = hd(rev times) // i.e. snd or last
                                        (lst_union end_check_times cc, sprintf " End_check_time=%s" (sfold p2s end_check_times))
                                if vd>=4 then vprintln 4 (sprintf " variable-latency yes stall on %s cmd=%s at  %i   te=%s. %s" mgr.mg_uid opxx.cmd start (shenToStr shen) e_msg)
                                lst_union cc (list_flatten times)
                                //singly_add shen.ss (singly_add shen.ee cc)
        let mine2_stall_states cc = function
            | (start, lst) -> List.fold (mine1 start) cc lst
        List.sort (List.fold mine2_stall_states [] work)

    // new site
    let nostalls = nullp stall_correct_info
    if vd>=4 then vprintln 4 (sprintf "Early variable-latency operation check for thread %s in state %s: result=" (xToStr thread) (xToStr state) + (if nostalls then "No stalls can happen in this state" else " It can stall in npcs=%s" + (sfold p2s (stall_correct_info))))
    stall_correct_info





// Compare pc against a state name or guard on a reverb flop.
// Two fairly similar functions: gen1 and gen2 - please tidy up.
let pc_deqd_gen1 snfo thread state npc (mc, pcv) =
        if mc <> g_primpc_F then
            let reverb_onehot_net = newnet_with_prec g_bool_prec (sprintf "%s%i" mc pcv) // TODO abstract and share
            xi_orred(reverb_onehot_net)
        elif nonep snfo then ix_deqd thread state else ix_deqd npc ((valOf snfo) (mc, pcv)) // Old pc compare or New pc compare, accordingly.



      
//
// Form camshaft waveforms (bus ops). Called once for the root of a major state and called again for each edge from that major state.
// Work is either rootwork or the work of an edge.
// This also rewrites the main RTL to connect to the structural components and creates (cam shaft) control waveforms for the structural resources.
//
let res2_form_cam_ops ww (settings:restruct_manager2_t) m_assist contact_maps (domain, thread, (rel_sto:((string * int) list * rel_stall_corrector_t) option), m_score) (f, edge_guard, state, start_pc, npco, (exec_cycle, priv_start_pc, priv_last_pc), snfo) pre_pairs seo camops_work =
    let _:filx_t = f
    let vd = settings.vd
    let devp = vd >= 5
    let abs_start_n:int = start_pc
    let _:(int * (cbg_track_t * schedule_temporal_extent_t * newopx_t) list) list = camops_work
    
//    if nullp work then ([], [], [], [], [], makemap [])
//    else
    let m0 = "res2_form_cam_ops: " + sfold edgeToStr domain.clocks + ":" + xToStr thread + ":" + xToStr state + ":?"
    let eno =
        match seo:scheduled_edge_t option with
            | Some se ->
                let _ = vprintln 3 (sprintf "se.edge.no=%i" se.edge.eno)
                se.edge.eno
            | None    ->  (-12345678)

    report_work_summary m0 camops_work
    
    let m0 = m0 + ":" + i2s eno

    let pp_gen(mc, pcv) =
        if mc <> g_primpc_F then sf (sprintf "expected firm (not reverb) state at L3860, not %s %i" mc pcv)
        elif nonep npco then None
        elif not_nonep snfo then Some(valOf npco, (valOf snfo) (mc, pcv))
        else Some(thread, state)

    let pc_deqd_pgen (mc, pcv) =
        if nonep npco then X_true
        elif mc <> g_primpc_F then
            if nonep snfo then sf ("Should not recode without a statename function.")
            else xi_orred ((valOf snfo) (mc, pcv))
        else
            if nonep snfo then ix_deqd thread state
            else ix_deqd (valOf npco) ((valOf snfo) (mc, pcv))

    let stutter aa = if nonep npco then None else Some(if nonep snfo then (thread, state) else (valOf npco, (valOf snfo) aa)) // same sas pp_gen?
    let m_pli_newrtl = ref []
    let now = if nonep snfo || nonep npco then state else valOf npco 


    let hr_inuse = new ListStore<string * string, hr_scoreflop_t>("hr_inuse") // Those in use his major cycle.
    // Allocate a holding register and scoreflop support that is not already in use this major cycle
    // Also FPGA is rich in registers so discuss merits. TODO.
    // These are seq or comb and per clock domain. TODO would like affinity to a port and not just a resource within a domain.
    let alloc_hr domain res_key (mgr:str_methodgroup_t, method_str2) = // Approach one - antistall/persistence controlled autonmously instead of approach two where we do context-sensitive reads.
        let method_meld = valOf_or_fail "L4965" method_str2.method_meld
        let key = method_meld.return_lname
        let key1 = (mgr.mg_uid + key)
        //dev_println (sprintf "Tracker: alloc_hr: %s %s" key1  method_str2.fname)
        let prec = method_str2.precision2
        let dn = domain_no mgr.dinstance domain
        let m_lst =
            if dn >= length method_str2.holders then
                dev_println ("Tacitly create gully to store spare holding register!") // TODO
                ref []
            else
                select_nth dn method_str2.holders // All holders for this methodgroup are indexed here.
        let av = list_subtract(!m_lst, hr_inuse.lookup res_key)
        let mgr =      valOf_or_fail "L5055" !method_str2.m_mgro
        let mgr_meld = valOf_or_fail "L5056" mgr.mgr_meldo

        let rr = 
        //TODO this stall corrector does not consider FUs with clock enable inputs which would provide a simpler mechanism.
            if av<>[] then hd av
            elif nonep mgr_meld.hs_vlat then // TODO exploit fs_outhold self-holders in future - ie units that register their output themselves and are not recycled within this major state.
                let id = funique (key1 + "_h")
                let hr = newnet_with_prec prec (id + "hold")
                vprintln 3 (sprintf "hholder F/L alloc_hr  %s  %s  %s  prec=%s" mgr.mg_uid key id (prec2str prec))
                let shotgun = map (fun n -> simplenet(id + sprintf "shot%i" n)) [0..mgr.auxix.result_latency-1]
                // A shotgun is a shift register tracking data down the pipelined component that does not stall with this thread.
                // Given no stalls, the output data is ready when expected and the last stage of the shotgun holds at that time.
                // If we have stalled, the data will have come out in the meantime and we read it from a holding register that was enabled by the shotgun.
                // Therfore, shotgun logic does not appear in the stallnet disjunction.
                if nullp shotgun then sf (sprintf  "no shotgun length %i" mgr.auxix.result_latency)
                let hr_scoreflop = HR_shot(id, hr, shotgun)
                mutapp settings.m_morenets (hr::shotgun)            
                mutadd m_lst hr_scoreflop
                hr_scoreflop
            else
                let id = funique (key1 + "_h")     // A tagged with valid holding register - dataflow!
                vprintln 3 (sprintf "hholder V/L alloc_hr  %s  %s  %s  prec=%s" mgr.mg_uid key id (prec2str prec))  
                let hr = newnet_with_prec prec (id + "hold")
                let primed_flag  = simplenet(id + "primed")
                let trn = (mgr.dinstance.ikey, valOf mgr_meld.hs_vlat)                

                let valid_flag  = simplenet(id + "vld")
                mutadd settings.m_morenets hr 
                mutadd settings.m_morenets valid_flag           
                mutadd settings.m_morenets primed_flag 
                let hr_scoreflop = HR_vl(id, hr, trn, primed_flag, valid_flag)
                mutadd m_lst hr_scoreflop
                hr_scoreflop
        hr_inuse.add res_key rr
        rr

    let stall_correct_info = if nonep rel_sto then [] else fst(valOf rel_sto) // Can move earlier? scon - but we need to spot inter-thread contention before stall correcting needs are fully complete.

    let nostalls = nullp stall_correct_info
    vprintln 3 (sprintf "Secondary (soon to be redundant) variable-latency operation check for thread %s in state %s: result=" (xToStr thread) (xToStr state) + (if nostalls then "No stalls can happen in this state" else " It can stall in npcs=%s" + (sfold p2s (stall_correct_info))))


    let new_bus_ops pass (starty_:cbg_track_t, shen__, opxx:newopx_t) (busops_sofar, pre_pairs, maps0) = // Folded over each operation on an FU
        // shen__ not needed now ? since op contains absolute housekeeping. Given a double__ suffix to denote we are removing it.
        // When pass=1 we enter operations in the scoreboard (collect rewrites) and when pass=2 we return the rewriting pairs.
        let m1 = sprintf "new_bus_ops: Pass=%i: eno=E%i serial=OX%i cmd=%s oldx=%s shen=%s" pass eno opxx.serial opxx.cmd (xToStr opxx.oldnx) (shenToStr shen__)
        let ww = WF 3 "restructure2" ww ("Form camops for " + m0+m1)

        let house1 ee = 
            match opxx.housekeeping with
                | (eno_, HK_abs(mwhen1, _))::_ -> // Start time is the same in all sharings over edges so can ignore eno_ here.
                    //vprintln 0 (sprintf "hk1 mwhen=%i" mwhen1)
                    mwhen1
                | items -> sf (sprintf "%s: no house1 items=%A" (ee()) items)

        let need_stall_corrector housekeeping = // Return a stall corrector signal if needed.  
            // An answer will have passed out un-noticed if we stall in any state beyond its first.
            let relevant_hk = if nullp housekeeping then [] else tl housekeeping // This will discard any gobacks too.
            let pcvs =
                let fx cc = function
                    | (_, HK_rel(rpcv, tag)) -> sf ("hk was not reindexed for " + hkToStr "" (HK_rel(rpcv, tag)))
                    | (_, HK_abs(times, _))  -> lst_union cc times
                List.fold fx [] relevant_hk
            //let latency = (valOf stro).result_latency 
            let rec need_searcher = function
                | [] -> false
                | h::_ when memberp h pcvs -> true
                | _::tt -> need_searcher tt
            let needed = need_searcher stall_correct_info
            if vd>=4 then vprintln 4 (sprintf "startn=%i: check need to make stall corrector for pipelined component   f/t=%s     needed=%A" abs_start_n (sfold p2s pcvs)  needed)
            if needed then
                let gterm (mc_, n) =
                    match rel_sto with
                        | Some(sites, db) -> xi_orred(db.lookup (n - abs_start_n)) 
                        | None -> sf ("stallcorrect was seemingly needed L3207")
                Some(ix_orl (map gterm pcvs))
            else None

        let (post_pairs, xmm) = // We need lots of mappings to support diversity - where the same subexpression is executed on FUs (hopefully in parallel to gain performance). Currently they are all composed into one.
            // rd_smart will directly read an expression if it is ready at the correct time or else read it from a holding register if ready earlier and not registered or forward it from its src if not yet committed.
            // We normally need rd_smart to deliver an expression before its new value in the current major state is committed. But when handling a go_back from a successor state we need to read the committed value, which will not be in the register/resource if only written in the last write state so it will then need forwarding from the source of that write. Such go_back reads can always complete in one cycle since they were scheduled in one minor cycle of the sucessor they belong to - the only possible exception being a combinational read of a structural RAM that is port limited ... TODO explain.
            // Loop forwarding of values committed in the precursor ...

            let rd_smart cc (cmd, rdat, scorex) =
                if vd>=5 then vprintln 5 (sprintf "rd_smart: cmd=%s old=%s" cmd (xToStr scorex.old))
                if cmd = "write" then cc // We need to avoid holding registers for void-returning FU operations TODO - generalise this rule for other method names.
                else
                let rdyat = scorex.readyat
                let is_scalar = // simple kludge
                    match scorex.s_mgr.dinstance.item with
                        | SR_scalar _ -> true
                        | _ -> false
                if is_scalar && scorex.newx= X_undef then cc
                elif snd rdyat < 0 then
                    vprintln 3 (sprintf "do not forward written value (-ve rdyat of %s) guts=%A" (p2s rdyat) (xToStr scorex.old))
                    cc
                else
                let gentime = scorex.readyat // Aproach 1 - create a translatch effect that is open at gentime.
                if vd>=5 then vprintln 5 (sprintf "rd_smart: approach 1 - for nn=%i,   cmd=%s,  generated at time=%A,  rdyat=%s/%i,   mgkey=%s reading value computed at  - needed for support to oldx=%s sux=%A" scorex.oldx_n cmd gentime (fst rdyat) (snd rdyat) (scorex.s_mgr.mg_uid) (xToStr scorex.old) rdat.sux_times)
                let isnow = pc_deqd_pgen rdyat
                let scxr = need_stall_corrector opxx.housekeeping
                // A holding register can hold the output from a structural component or else a computed function of it and perhaps others.
                // We tie them to computed result here and leave migration to subsequent recipe stages or FPGA/ASIC tools.
                let get_holding_reg res =
                    let hr_key = (res.dinstance.ikey, res.mg_uid) // Assuming FU has only one output per mg (method group) then no additional field needed here.
                    //let uid = res.mg_uid 
                    match op_assoc (hr_key, scorex.oldx_n, rdat.atcmd) !m_score.live_hrs with
                        | Some ov ->
                            if vd>=5 then vprintln 5 (sprintf "using existing live subexpression %A %s" hr_key scorex.opcmd)
                            ov // Use existing if already live with the required subexpression.
                        | _  ->
                            let rec hgolder_mine = function
                                | [] -> sf (sprintf "hgoldermine for cmd='%s' - command has no method" cmd)
                                | method_str2::tt when method_str2.fname = cmd && method_str2.overload_suffix=scorex.overload_suffix -> method_str2
                                | _::tt -> hgolder_mine tt
                            //dev_println (sprintf "mine for cmd %s x2 overload=%A" cmd scorex.overload_suffix)
                            let holder = alloc_hr domain.clocks hr_key (res, hgolder_mine res.methodgroup_methods)
                            mutadd m_score.live_hrs ((hr_key, scorex.oldx_n, scorex.opcmd), holder) 
                            let _ =
                                match holder with
                                    | HR_vl(id, hr, ack_net, primedflag, valid_flag) ->
                                        let vl_ = mutaddonce m_assist (holder, None, X_dontcare, scorex.newx) // now pulled elsewhere ...
                                        ()
                                    | HR_shot(id, hr, shotgun) ->                                        
                                        let shotgun_exit = hd(rev shotgun)
                                        if vd>=4 then vprintln 4 (sprintf "Assigned shotgun id=%s exit=%s for %s" id (xToStr shotgun_exit) (xToStr scorex.old))
                                        mutaddonce m_assist (holder, None, (* No pc guard on a shotgun *)(xi_orred shotgun_exit), scorex.newx)
                                        ()
                            // rewait - for f/l the h/r is written on shotgun barrel exit. For v/l it is written on resource ack.
                            // Neither is a scheduled bus op now.
                            holder
                let prior = op_assoc (rdat.atcmd, scorex.old) cc // TODO cmd match too please.
                //if not_nonep prior then dev_println ("not none prior")
                if false && not_nonep prior then
                    // Double entry now arises from root control entries
                    // or subexpressions that have been stored and are now being loaded back - we can forward these to avoid an array read.
                    vprintln 3 (sprintf "pass=%i double computation in schedule for cmd=%s oldx=%A %s. For non-EIS we should reuse old read or avoid WaW code.\nPrevious entry=%s"  pass rdat.atcmd scorex.oldx_n (scorexToStr scorex) (xToStr(f1o3(valOf prior)))) // What if read then write of synchronous RAM in same cycle - please say why that does not raise this trap.
                    cc

                //First special simple fast route - available now, fixed latency and  no stall correct applies.
                //Unfortunately we need to have schedulled all (edges?) before we know this for sure ... we can get this from a prescan actually - one of the scon passes.
                // elif false && (fpf) && (rewait_ = X_true) && scorex.rdytime=now && needs_no_hr then (scorex.old, scorex.newx)::cc

                //Second special simple fast route - variable latency but nothing else is going on so simple stall the whole thread until ready.
                //elif (not fpf) && false then // TODO: the further required checks are not implemented yet.
                //    (scorex.old, scorex.newx)::cc

                //elif nonep scorex.s_mgr.rdycond && now >=0 && scorex.readyat>abs_start_n+now then sf ("+++ " + xToStr bad_msg) // this is fatal
                else
                //vprintln 0 (sprintf "rd_smart  (readyat=%i) cmd=%s now=%i exec_cycle=%i te=%s"  scorex.readyat scorex.opcmd now exec_cycle (shenToStr shen__))
                // We can naively generate latency correctors for all structural resources, but some are easily optimised to a fixed or semi-fixed forwarding pattern without per-use-context processing (e.g. those only used once, which is the majority).

                let smartpair () = 
                    let hrego =
                        let prec = mine_prec g_bounda scorex.old
                        if scorex.s_mgr.auxix.result_latency < 1 then None elif nonep prec.widtho then None else Some(get_holding_reg scorex.s_mgr)

                    //TODO no holder needed for integral_hr self holdings - ie for a ready flagging (non-pipelined device).
                    let key = (rdat.atcmd, scorex.old)
                    match valOf hrego with // some components are self-holding - but instantiate a holding register anyway FOR NOW .... 
                        | HR_vl(id, hr, ack_net, primed_flag, valid_flag) ->
                            let nv = 
                                if vd>=4 then vprintln 4 (sprintf "varilatency read approach1 - looks after itself")
                                ix_query (xi_orred valid_flag) hr scorex.newx
                            (key, (nv, hrego, rdyat))

                        | HR_shot(id, hr, shotgun) ->
                            //if vd>=4 then vprintln 4 (sprintf "fully-pipelined, fixed latency read - start at %i, ready at %i  - stall_corrector_inuse=%A" abs_start_n (p2s scorex.readyat)  (not_nonep scxr))
                            if true then
                                // Optimise: Read from h/r always if all reads are statically post and not self-holding etc..
                                // Most general condition - read directly if isnow and no stalls and no shot ...
                                // If shot tag is valid then read from the h/r of course (unless self-holding)
                                let g0 = xi_not isnow
                                // let g0 = if nonep scxr then scorex.newx else valOf scxr
                                let gfinal = g0
                                let nv = ix_query gfinal hr scorex.newx
                                (key, (nv, hrego, rdyat))
    // All scoretag flops are cleared at the start of a major cycle.
    // For a V/L device, the stall divert is controlled by the scoretag flop.  If the device has integral output hold and not resused before exec we do not need an external holding reg but that's for future optim.
    // For a F/P device, the shotgun shift register, containing a reg no, triggers capture into that holding register on output.  Data is read from the holding reg if the output is schedulled before exec or a stall has been noted by the relevant stall collector.                    
                            else
                            let stall_divert ss_ rdytime_ = // Commented out for now TODO delete or fix
                                if nostalls then None
                                else
                                    let barrel_no_ = -1
                                    let pre_execf = false // true is in the if above
                                    Some (barrel_no_, pre_execf)  // pre_execf indicates whether to always read from hr or else only on stall correct.
                            let shotgun_exit = hd (rev shotgun)
                            let nv =
                                let _ : int = abs_start_n
                                match stall_divert shen__.ss (abs_start_n + snd scorex.readyat) with // Here IS A USE OF shen__  - ideally we want the rootf only from shen
                                    | None -> scorex.newx
                                    | Some (shotbarrel_no_, pre_execf) -> // pre_execf indicates whether to always read from hr or else only on stall correct.
                                        if pre_execf then muddy "above handled this"
                                        else ix_query (xi_orred shotgun_exit) scorex.newx hr
                            (key, (nv, hrego, rdyat))

                let approach2 = false
                if approach2 then cc
                else
                    let hrego =
                        let prec = mine_prec g_bounda scorex.old
                        if scorex.s_mgr.auxix.result_latency < 1 then None elif nonep prec.widtho then None else Some(get_holding_reg scorex.s_mgr)

                    let nv =        
                        if nonep hrego then ((rdat.atcmd, scorex.old), (scorex.newx, None, rdyat)) // <---- baseline assign pair case
                        else smartpair() // Old pairs - approach 1
                    //dev_println (sprintf "nv pair = %s -> %s" (xToStr (snd (fst nv))) (xToStr(f1o3 (snd nv))))
                    nv::cc


            let post_pairs = List.fold rd_smart pre_pairs !m_score.scboard // Convert scoreboard into rewriting pairs for the RTL - but just for one ca (operation on an FU) since different occurences of a subexpression may need different rewrites if read at different times.  But the o/p from one is the i/p to another.
            let _ =
                if vd>=6 then
                    if nullp post_pairs then
                        vprintln 6 (m0 + sprintf ": xmm: rewriting pairs (post_pairs) are te=%s  %i scoreboard entries turned into no rewrites."  (shenToStr shen__) (length !m_score.scboard))
                    else dev_println (m0 + sprintf ": xmm: rewriting pairs (post_pairs) are te=%s - some present:  ^%i -> ^%i = " (shenToStr shen__) (length !m_score.scboard) (length post_pairs) + sfoldcr (fun ((cmd, a),(b, _, _)) -> cmd + ":" + xToStr a + "--->" + xToStr b) post_pairs)
            let xmm = makemap (map (fun ((cmd, oldx), (newx, _, _)) -> (oldx,newx)) post_pairs)
            (post_pairs, xmm)

        let rewrite_pli guard =
            let ans = rewrite_rtl ww vd guard xmm
            ans
           
        let rewrite_pli_verbose__ guard =
            let fx g q =
                let ans = rewrite_rtl ww vd guard xmm g q
                if vd>=4 then vprintln 4 (sprintf "rewrite_pli: new guard=%s: Rewritten ans PLI is %s" (xbToStr guard) (sfold xrtlToStr ans))
                ans
            fx
            
        // The old RTL is rewritten as new RTL using mapping xmm. - This is the main rewrite, but the edge guard has been rewritten by our caller after ctrl call.  The boolean rewrites just below here are for RTL control guards (aka action guards).
        let rewrite_newop (opx:newopx_t) =
            //let sk = { g_sk_null with timebase= house1 (fun () -> "timebase") }
            let sk = g_sk_null
            let xi_assoc_bexp_vd sk xmm arg =
                let ans = xi_assoc_bexp sk xmm arg
                if vd>=5 then vprintln 5 (sprintf "xi_assoc_bexp (verbose) for npc=%s " (sfold p2s shen__.ss) + xbToStr arg + " becomes " + xbToStr ans)
                ans
            // Merge the edge guard and action guard, but not for a reverb or post pad state please
            let gs' =
                let action_grd = xi_assoc_bexp_vd sk xmm (fst opx.args_raw)
                let is_private_state =
                    match opx.housekeeping with
                        | (_, HK_abs(("F", nn)::_,_))::_ -> // This is an overly-sensitive pattern?
                            if devp then dev_println(sprintf "is_private: checking %i within %i .. %i"  nn  priv_start_pc priv_last_pc)
                            if priv_start_pc < 0 then false else priv_start_pc <= nn && nn <= priv_last_pc

                        | [] when opx.oldnx = g_null_newopx.oldnx && not_nullp opx.unaltered ->
                            vprintln 3 (sprintf "L5053: missing housekeeping acceptable for old-style non-fabbed PLI serial=OX%i" opx.serial) // (sfold xrtlToStr opx.unaltered))
                            false
                        | other ->
                            //dev_println (sprintf "Default private for hk=%A  %s  %s pli=%i" other opx.serial (xToStr opx.oldnx) (length opx.unaltered))
                            true
                if devp then dev_println (sprintf "state=%s is_private_state=%A" (xToStr state) is_private_state)
                if is_private_state then
                    //dev_println (sprintf "FRIDAY edge_guard is %s   action_guard is %s " (xbToStr edge_guard) (xbToStr action_grd))
                    action_grd
                else
                    ix_and edge_guard action_grd // Action guard rewrite - also conjuncts in the edge guard but that may be divided out again when rendered finally as SP_fsm. TODO edge guard needs build out too?
            //vprintln 0 (m1 + sprintf " action xition guard rewrite from %s to %s" (xbToStr (fst opx.args_raw)) (xbToStr gs'))
            let opx' = { opx with 
                           //action_grd=gs' // Action guard Or composite TODO - tidy names
                           args_raw=
                              let args_raw = map (xi_assoc_exp sk xmm) (snd opx.args_raw)
                              //dev_println (sprintf "Rewritten args_raw are " + (sfold xToStr args_raw))
                              (gs', args_raw)
                           g_args_bo=
                                match !opxx.g_args_bo with
                                 | None -> ref None
                                 | Some(g8, args) ->
                                      let args = map (xi_assoc_exp sk xmm) args
                                      //dev_println (sprintf "Rewritten args_bo are " + (sfold xToStr args))
                                      let g9 = 
                                          match g8 with
                                              | W_bdiop(V_orred, [X_x(_, power, _)], _, _) -> xi_orred(xi_X power (xi_blift gs'))
                                              | other -> gs' // Assume no build out needed
                                      ref(Some(g9, args))
                           unaltered=  List.foldBack (rewrite_pli gs') opx.unaltered [] // Now rewrite (alter) the unaltered - becomes a misnomer, but its structure is unaltered.
                      }
            opx'
        // Combinational links between blocks require value create/read sorting. So do twice. Always sufficient ? TODO answer.

        // For speed, do no rewrite if there are no mappings to transform. Disabled since further work is achieved anyway?
        let opxx = if false && nullp post_pairs then opxx else rewrite_newop opxx

// OLD (ie without major state pipeline enabled) new goes through so-called fabbed route.
        let include_pli ss = function
            | XRTL(_, g, zl) ->
                //dev_println ("Old route: PLI include")
                if snd(hd ss) <> exec_cycle then dev_println (sprintf "mismatched shen exec_cycle %i %s" exec_cycle (p2s (hd ss)))
                if length ss=1 then XRTL(pp_gen (hd ss), g, zl)
                else sf "did not expect multicast of pli"
        if pass=2 (*&& not_nullp shen__.ss *) then mutadd m_pli_newrtl (map (include_pli shen__.ss) opxx.unaltered) // Another USE of shen__ - exec cycle will serve instead here we believe
// END OLD

        let notpres q (cmd_, now, scorex) = scorex.old<>q // Predicate to check that an operation is not present earlier in the schedule from where it could be shared.

        let selected_housekeeping = 
            let selex (e, item) cc = if e<0 || e=eno then singly_add item cc else cc
            let ans = List.foldBack selex opxx.housekeeping []
            //dev_println (m1 + sprintf ": selex eno=E%i selected %s out of %s" eno (sfold (hkToStr "") ans) (sfold hkpToStr opxx.housekeeping))
            ans
            
        let house2 ee =
            match selected_housekeeping with
                | [HK_abs(mwhen1, _); HK_abs(mwhen2, _)] ->
                    //let _ = vprintln 0 (sprintf "hk2 mwhen=%s  and  %s" (sfold i2s mwhen1) (sfold i2s mwhen2))
                    (mwhen1, mwhen2)
                | [HK_abs(mwhen1, _) ] -> // Register files are currently processed as scalars and only have one hk entry, so replicate here
                    //let _ = vprintln 0 (sprintf "hk2 mwhen=%s  (auto replicated)" (sfold i2s mwhen1))
                    (mwhen1, mwhen1)
                | items -> sf (m1 + sprintf ": %s: house2 L4413 missing housekeeping pair: items=%A concourse=%s" (ee()) items (sfold (fun (eno, hk) -> i2s eno + ":" + hkToStr "" hk) opxx.housekeeping))

        let house2_solo_peek_absent () = 
            let absentf = 
                match selected_housekeeping with
                    | HK_abs(mwhen1, _)::HK_abs(mwhen2, _)::_ -> length mwhen2 = 0
                    | _ -> false
            let _ = if absentf then vprintln 3 (sprintf "ALU result_rd not present yet/ or not used on this arc eno=E%i" eno)
            absentf
            
        let house2_solo ee =  // Return second time, or copy of first if second missing.
            match selected_housekeeping with
                | [] -> sf (m1 + sprintf ": %s: house2_solo expect 2 items. Items=nil" (ee()))
                | [ HK_abs(mwhen1, _) ] ->
                    match mwhen1 with // Auto-replicate
                        | [solo_time] -> solo_time
                        | items -> sf (m1 + sprintf "%s: house2_solo auto-replicate not solo items=%A" (ee()) items)                        
                | HK_abs(mwhen1, _)::HK_abs(mwhen2, _)::_ ->
                    //let _ = vprintln 0 (sprintf "hk2_solo mwhen=%s  and  %s" (sfold i2s mwhen1) (sfold i2s mwhen2))
                    match mwhen2 with
                        | [solo_time] -> solo_time
                        | items -> sf (m1 + sprintf "%s: house2_solo not solo items=%A" (ee()) items)                        


        let ee() = xToStr opxx.oldnx + "instance=" + (if nonep opxx.method_o then "no instance" else (valOf !(valOf opxx.method_o).m_mgro).mg_uid)

        let newops =
            //if pass < 2 then [] // Dont bother to generate all the ops on the first pass.
            // Read ops may start in the control/root section and complete in an edge-specific schedule. We may trigger them more than once if done in edge and cannot complete  them if done in root.  We would still need to write/duplicate them to read shed of each edge if in root shed from somewhere else. So we do not generate them on root schedules and regenerate them on each edge schedule and post-elide multiple triggers occurring upto the root_hwm.  
            if shen__.rootf && eno >= 0 then
                //dev_println (sprintf "sKIPCASE-A Skip opxx oldx=%s for shen_rootf=%A eno=E%i customer_edges=%s" (xToStr opxx.oldnx) shen__.rootf eno (sfold i2s opxx.customer_edges))
                []      // Do not issue root work when we are collating for an arc.
            elif not shen__.rootf && eno < 0 then
                //dev_println (sprintf "sKIPCASE-B Skip opxx oldx=%s for shen_rootf=%A eno=E%i customer_edges=%s" (xToStr opxx.oldnx) shen__.rootf eno (sfold i2s opxx.customer_edges))
                [] // Do not issue work that is private to arcs when we are collating for the root.
            else
            let mwhen0 = shen__.ss // A use of shen__ - PLEASE USE house1 instead
            let ww = WF 3 "form_cam_ops" ww (sprintf "form newops for state=%s  eno=E%i pass=%i; mwhen0=%s; nn=%s cmd=%s oldx=%s ikey=%s" (xToStr state) eno pass (sfold p2s mwhen0) (x2nn_str opxx.oldnx) opxx.cmd (xToStr opxx.oldnx) (if nonep opxx.method_o then "-" else (valOf !(valOf opxx.method_o).m_mgro).mg_uid))

            let get_rd_bus ikey1 pi_name meld externally_instantiated portmeta formal_name = 
                //dev_println (sprintf "get_rd_bus pi_name=%s meld=%A" pi_name meld)
                let lhs = get_actual contact_maps "get_rd_bus" ikey1 pi_name formal_name
                //dev_println (sprintf "pno=%i Retrieved cmap for %s      lhs_net=%s" (pno()) formal_name (xToStr lhs))
                lhs

            match opxx.method_o with
              
            | None -> []
            | Some method_str2  ->
                let mgr = valOf(!method_str2.m_mgro)
                let ikey1 = mgr.dinstance.ikey
                let mgkey = mgr.mg_uid                
                let oldx_n = x2nn opxx.oldnx
                //dev_println ("work on " + opxx.serial)
                let (en, args) = valOf_or !opxx.g_args_bo opxx.args_raw
                let hr_key = (ikey1, mgr.mg_uid) 
                let triggers = // To store data into holding registers when needed. On pass2 when there from first time.
                
                    match op_assoc (hr_key, oldx_n, opxx.cmd) !m_score.live_hrs with
                        | Some (HR_shot(id, hr, shotgun)) ->
                            //vprintln 0 (sprintf "ikey=%s: pulled shotgun trigger %s for %s. head=%s" ikey (netToStr hr) (xToStr opxx.oldnx) (xToStr (hd shotgun)))
                            [(mwhen0, 1, en, gec_phy (hd shotgun), xi_one, xi_zero, [], RAC_trigger)]
                        | Some (HR_vl(id, hr, ack_net, primed_flag, valid_flag)) ->
                            //vprintln 0 (sprintf "ikey=%s: primed vr holding structure %s for oldx=%s" ikey (netToStr hr) (xToStr opxx.oldnx))
                            [(mwhen0, 1, en, gec_phy primed_flag, xi_one, X_undef, [], RAC_trigger)]

                        | Some _ ->
                            if vd>=5 then vprintln 5 ("shotgun trigger not required for " + xToStr opxx.oldnx)
                            []
                        | None ->
                            if vd>=5 then vprintln 5 (sprintf "nstf: mgkey=%s eno=E%i no shotgun trigger found for %s" mgkey eno (xToStr opxx.oldnx)) // TODO say why this is good or bad
                            [] 
                let base_rdat = { atcmd=opxx.cmd;  (* nominal_rdy_time_= -123456;  TODO dont use *)  sux_times=opxx.support_times; }
                
                let base_scorex =
                    let mgr = valOf !(valOf_or_fail "L5350" opxx.method_o).m_mgro
                    //dev_println (sprintf "base_scorex: method_str2.overload_suffix=%A" method_str2.overload_suffix)
                    { oldx_n=oldx_n; opcmd=opxx.cmd; overload_suffix=method_str2.overload_suffix; startat=mwhen0; old=opxx.oldnx; s_mgr=mgr; newx=X_undef;  readyat=("XX", -123457); opargs=args }
                let rec generate_camops dinstance =
                    match dinstance.item with
                        | SR_pliwork ->
                            // TODO rename spog as smart_tappet
                            // TODO better if spog-form output - i.e. output that understands and resolves disjunctions, defaults, dont-cares and both combinational and sequential nets.
                            if pass = 2 then
                                let hk = hd mwhen0
                                let nn = snd hk
                                let edge_grd = if nonep npco then X_true else pc_deqd_gen1 snfo thread state (valOf npco) hk
                                let (action_grd, args) = valOf_or !opxx.g_args_bo opxx.args_raw
                                let ggg = ix_and edge_grd action_grd
                                let pli = XIRTL_pli(None, ggg, valOf opxx.plimeta, args)
                                if devp then dev_println(sprintf "Final PLI shove OX%i edge_grd=%s  action_grd=%s  hk=%A %s %s ggg=%s" opxx.serial (xbToStr edge_grd) (xbToStr action_grd) hk  opxx.cmd (sfold xToStr (snd opxx.args_raw)) (xbToStr ggg))
                                mutadd m_pli_newrtl [pli]
                            []

                        | SR_scalar xnet ->
                            match opxx.cmd with
                                | "argread" ->
                                    //dev_println(sprintf "URGENT eno=E%i process scalar argread i/o '%s'" eno (netToStr xnet)) // Need to read from arg register (when not externally-held).
                                    let rdat = base_rdat // { base_rdat with nominal_rdy_time_= -120086; } 
                                    mutadd m_score.scboard (opxx.cmd, rdat, { base_scorex with readyat=("XX", -1234568);   })
                                    []
                                    
                                | "read"  -> []


                                | "write" -> // Can be a register file write, which is represented as an SR_scalar for now.
                                    //in scope already let (en, args) = valOf_or !opxx.g_args_bo (opxx.action_grd, opxx.args_raw)
                                    let rhs = hd args
                                    let wri = [(mwhen0, 1, en, gec_phy xnet, rhs, X_undef, [], RAC_spog)]
                                    let rdat = base_rdat // { base_rdat with nominal_rdy_time_= -120086; } 
                                    mutadd m_score.scboard (opxx.cmd, rdat, { base_scorex with readyat=("XX", -1234568);   })
                                    if vd >= 5 then vprintln 5 (sprintf "Scalar reg assign OX%i npc=%A  action_grd=%s  lhs=%s rhs=%s" opxx.serial (mwhen0) (xbToStr (fst opxx.args_raw)) (xToStr xnet) (xToStr rhs))
                                    if devp then dev_println(sprintf "Scalar reg assign OX%i npc=%A  action_grd=%s  lhs=%s rhs=%s" opxx.serial (mwhen0) (xbToStr (fst opxx.args_raw)) (xToStr xnet) (xToStr rhs))
                                    [ (wri@triggers, ["Sc: " + xToStr xnet]) ]
                                | other -> sf (sprintf "other scalar cmd '%s' on %s" other (netToStr opxx.oldnx))


                        | SR_fu(_, auxix, SR_alu alu, _) -> // generate_camops clause - convert to SR_generic_external(  ) FU
                            let externally_instantiated = false                          
                            let mkey = alu.mkey
                            let kind_vlnv = alu.kind
                            let portmeta = [] // ALUs do not have any
                            let kinds = hptos kind_vlnv.kind
                            let data_prec = alu.precision
                            let fu_meld = alu.fu_meld
                            let pi_name = "" // Only one method group on an ALU.
                            if vd>=4 then vprintln 4 (sprintf "Rezdown: ALU form: mkey=%s kinds=%s portmeta=%s" mkey (hptos kind_vlnv.kind) (sfold (fun (k, v)->sprintf "%s/%s" v k) portmeta))
                           
                            //let (block, phys_structure, phys_methods) = gec_block_phys ww fu_meld ikey1 mkey kind_vlnv data_prec contact_maps
                            let mirrorable = true
                            let str3 = SR_generic_external(kind_vlnv, None, externally_instantiated, Some data_prec, None(*Some auxi*), mirrorable)  // camops clause. iname is in foo.ikey anyway
                            // We need to direct to a specific method group for the selected port and go round again.
                            let dinstance' = { dinstance with item=(SR_fu(dinstance.mkey, auxix, str3, Some { fu_meld=fu_meld; area_o=None})) } // Go round again
                            generate_camops dinstance' // end of rezdown copy 2

                        // This is the only generate_camops basis clause. All others are mapped to it.  It works on all FUs not just 'external' ones so rename.
                        | SR_fu(mkey, auxix, SR_generic_external(kind_vlnv, sq, externally_instantiated, data_prec, auxi_, _), miniblock_o) ->  // camops clause. iname is in foo.ikey anyway
                            let msg = "restructure " + opxx.cmd
                            let rdyat = house2_solo ee
                            // mgr.rdycond=Some variable_latency was already inserted at mgr rez time.
                            let rdat = base_rdat // { base_rdat with nominal_rdy_time_=rdyat;  }  // Is it here where we add on mgr.auxix.result_latency please TODO
                            let mgr = valOf_or_fail "L5495" !method_str2.m_mgro
                            //dev_println (sprintf "Start preferred pi_name=%s  mgr=%A" mgr.pi_name mgr)
                            let miniblock = valOf_or_fail "L5530-blocko" miniblock_o

                            let portmeta = miniblock.fu_meld.parameters // This is the schema only? Are there cases where we want actuals?
                            let kinds = hptos kind_vlnv.kind
                            let method_name = opxx.cmd
                            let method_meld = valOf_or_fail "L5498" method_str2.method_meld
                            let rd_bus =
                                if method_meld.voidf then X_undef
                                else get_rd_bus ikey1 mgr.pi_name miniblock.fu_meld externally_instantiated portmeta method_meld.return_lname
                            let rd_bus = if nonep opxx.decoderfo then rd_bus else (valOf opxx.decoderfo) rd_bus // NOTE the free variables of decoderf must remain live until this time in alias analysis: TODO verify
                            //dev_println (sprintf "rd_bus is %s for structural resource %s sq=%A" (netToStr rd_bus) mkey "foo.overload_suffix")

                            if conjunctionate (notpres opxx.oldnx) (!m_score.scboard) then mutadd m_score.scboard (opxx.cmd, rdat, { base_scorex with newx=rd_bus; readyat=rdyat })
                            if snd rdyat < 0 then sf "generic opx -ve rdy at L3853"
                            //vprintln 0 (sprintf "generic rdi structure is %A" block.phys_structure)

                            let rdi = get_melsh ww kind_vlnv opxx false externally_instantiated miniblock.fu_meld ikey1 method_str2 mwhen0 portmeta  // only call site
                            //reportx vd (m0 + ": generic meld tuple_arcs pp=" + sfold p2s mwhen0 + " en=" + sfold agToStr (fst opxx.args_raw)) tuple1_rtlToStr rdi
                            let ans = (rdi @ triggers, (opxx.cmd + "+" + (valOf_or_fail "L5463" opxx.method_o).fname + "/" + (if nullp (snd opxx.args_raw) then "" else xToStr (hd (snd opxx.args_raw))))::[])
                            [ans]

                                
                        | SR_fu(mkey, auxix, SR_ssram(base_kind, ff_, tech, data_prec, (ww, ll), unused_, romflag, atts), _) -> // generate camops
                        // 1-port synch1-ram: first phase - cannot rd or wr a location requiring input support
                        // 1-port synch1-ram: exec phase - ideally read and write same idx - TODO error if not...
                            if house2_solo_peek_absent() then [(triggers, [])] // house guards not actually used here.  Please explain...
                            else
                        // SRAM - compared with a generic FU we want to take advantage of simultaneous read and write of a location... ? HFAST allows this for generics where a new command is issued at  the same time as receipt of old one.  Please write up clearly!
                            let latency =
                                match tech with // Replicated code
                                    | T_PortlessRegFile auxix     -> auxix.result_latency
                                    | T_Onchip_SRAM(auxix, romf)  -> auxix.result_latency
                                    | other ->
                                         hpr_yikes(sprintf "TODO: Need latency figure for RAM basekind=%s %A" (base_kind) tech)
                                         g_default_auxix.result_latency
                            let kinds = base_kind // TODO add suffix ... - dual port and latency - this works somehow else.
                            let kind_vlnv = { g_canned_default_iplib with kind= [ kinds ] }
                            let externally_instantiated = false // always internal (but could easily make customisable from a user attribute)
                            let mgr = valOf_or_fail "L5484" !method_str2.m_mgro
                            let fu_meld = valOf_or_fail "L5485" mgr.dinstance.fu_meldo
                                // let (block, phys_structure, phys_methods) = gec_block_phys ww fu_meld ikey1 mkey kind_vlnv data_prec contact_maps
                            let mirrorable = false
                            let str3 = SR_generic_external(kind_vlnv, None, externally_instantiated, Some data_prec, None, mirrorable)  // camops clause. iname is in foo.ikey anyway
                            // We need to direct to a specific method group for the selected port and go round again.
                            let dinstance' = { dinstance with item=(SR_fu(dinstance.mkey, auxix, str3, Some{ fu_meld=fu_meld; area_o=None })) } // Go round again
                            generate_camops dinstance' // end of rezdown

                        | other -> sf (sprintf "+++ new_bus_ops other form %A" other)

                generate_camops mgr.dinstance
        (newops @ busops_sofar, post_pairs, xmm::maps0) // End of new_bus_ops.

    let bopToStr (ops, strings) =
        let opToStr (wsl, delay, g, lhs, rhs, defval, rdyGrd, spog) = sprintf "   busop %s    %s := (%i) %s" (sfold (fun (ws, wx) -> sprintf "%s/%i" ws wx) wsl) (pToStr lhs) delay (xToStr rhs)
        sfoldcr opToStr ops + sfolds strings

    // First pass - used only to determine the need for stall correction.  Can do this in a previous scon now? No, it collects all the work for this edge.
    let (bus_ops, post_tups, map1) =
        let new_bus_ops_caller pass cc (eno_, worklst) =
            //vprintln 3 ("new_bus_ops1 " + i2s s)
            List.foldBack (new_bus_ops pass) worklst cc

        let (lo_, _, _) = List.fold (new_bus_ops_caller 1) ([], pre_pairs, []) camops_work // First pass - answer ignored. Used only to determine the need for stall correction. Can do in a scon pass now. TODO  ?  No, this  also writes all the work to the scboard for inter-op communication in second pass, since the output of one op affects the input to another.
        //vprintln 0 (sprintf "lo_ contents (len=%i) = " (length lo_) + sfoldcr bopToStr lo_)        

        let (li0, post_tups, maps) = List.fold (new_bus_ops_caller 2) ([], pre_pairs, []) camops_work // Second pass

        //vprintln 0 (sprintf "li contents (len=%i) = " (length li0) + sfoldcr bopToStr li0)
        (list_flatten(map (fun(qlst, desc)->qlst) li0), post_tups, (state, npco, rewrite_Compose maps)) // Composed map needed for the xition guard rewrite. We can optimise since we only need compose it when eno<0. TODO.

    // Some/most nets are comb-assigned by one FSM only, so can ultimately use Verilog blocking assigns to later-listed datapath.
    // However, comb-looking loops can be made in the support of these assigns
    // Indeed, ALL nets at this level are single-FSM since arbiters will have per-thread inputs.
    // Implement wired-or disjunction for each lhs with a defval (mux for vectors)

    let camops_lhs_lst =
        // Unused FUs may be present, such as bondout ports that have no work to do. These do not feature in the camops_work. Also perhaps some unused outputs in the a used mgr may be found.
        let (lhs_lst, lhs_set) =
            // We here ignore nets whose defVal is undef - they will not need tieoffs if unused, but surely we want to apply smart_tappet/spog to them?
            List.fold (fun (cc, (lhs_set:Set<temp_db_wt>)) (s, nx, g, lhs, r, defVal, rdyGrd, spog) -> if defVal=X_undef || Set.contains lhs lhs_set then (cc, lhs_set) else ((lhs, defVal)::cc, lhs_set.Add lhs)) ([], Set.empty) bus_ops // collate on assigned nets
        lhs_lst

    //dev_println(sprintf "camops_lhs_list is " + sfoldcr_lite (snd>>netToStr) camops_lhs_lst)
    // Partition on lhs into those that do and do not assign a default value?    
    // Verilog output now does this. Hmmm does it - the defVal is ignored henceforwards.  Diosim does not! Need to move this to diosim. TODO. done? fully_supported is ignored! 
    let (fully_supported_, others) = if false then ([], bus_ops) else List.partition (fun (s, nx, g, lhs, r, defVal, rdyGrd, spog)->memberp (lhs, defVal) camops_lhs_lst) bus_ops

    //if not (nullp fully_supported_) then dev_println ("+++ some fully_supported work ignored in restructure")

    let gen4(spog, (mc, pcv), g, lhs, rhs) =
        if mc = g_primpc_F then (spog, pp_gen (mc, pcv), g, lhs, rhs) // The pmode value of "F" is special and denotes 'controlled by primary new PC'
        else
            let reverb_shifthot_net = newnet_with_prec g_bool_prec (sprintf "%s%i" mc pcv) // TODO abstract and share this code - eg fold inside pp_gen
            (spog, None, ix_and (xi_orred reverb_shifthot_net) g, lhs, rhs)   // or else controlled by shift-hot reverb daisychain.

    let thread_busop_tappets arg cc =
        match arg with
        | (sl, _, g, DB_PHY(X_undef), r, defVal, rdyGrd, spog) ->
            sf ("discarded assign to undefined lhs 1/2")
            cc

        | (sl, nx, g, DB_LOGICAL(ikey1, pi_name, formal_name, kind, portmeta, nps, externally_instantiated), rhs, defVal, rdyGrd, spog) ->
            let lhs = get_actual contact_maps "ee1" ikey1 pi_name formal_name
            //dev_println (sprintf "pno=%i Retrieved cmap 1/3 for %s    lhs_net= %s" pno formal_name (netToStr lhs))
            //dev_println (sprintf "thread_busop_tappets pno is %i ikey=%s" pno ikey)
            let l1 = if nx=1 then lhs elif nx=0 then xi_Xm1 lhs else sf "bad nx latency (not 0 or 1)"
            let thread_busop_tappets_serf cc s = gen4(spog, s, g, l1, rhs)::cc
            List.fold thread_busop_tappets_serf cc sl

        | (sl, nx, g, DB_PHY(lhs), rhs, defVal, rdyGrd, spog) -> // Old route, still used for input RTL.
            //dev_println (sprintf "camops old route DB_PHY lhs=%s g=%s" (xToStr lhs) (xbToStr g))
            //if not (xi_istrue rdyGrd) then mutadd m_score.adv_guards (s, rdyGrd) // another trawl ? copy out TODO
            let l1 = if nx=1 then lhs elif nx=0 then xi_Xm1 lhs else sf "bad nx latency (not 0 or 1)"
            let thread_busop_tappets_serf cc s = gen4(spog, s, g, l1, rhs)::cc
            List.fold thread_busop_tappets_serf cc sl


    let start_rdyguards =  // This is where we check the FU with handshake nets is ready to receive a command and or that is does not stall us on resuly return.
        let m_adv_guards = ref []
        let disj = function
            | (sl, nx, g, l, r, _, rdyGrds, spog) ->
                let baz cc trn  =
                    let (req_rdy_o, ack_net) = retrieve_handshake_inputs contact_maps trn
                    if nonep req_rdy_o then cc else (xi_orred(valOf req_rdy_o)) :: cc
                let rdyGrds' = List.fold baz [] rdyGrds
                let guardx (sv, rdyGrd) = mutadd m_adv_guards (sv, rdyGrd)
                app guardx (cartesian_pairs sl rdyGrds')
        app disj bus_ops
        !m_adv_guards
        
    let thread_busop_rtl = List.foldBack thread_busop_tappets others []

    let stall_guards =
            let gsg cc (oldx, (newx, hrego, rdyat)) =
                if snd rdyat < 0 || nonep hrego then cc
                else
                    match valOf hrego with
                        | HR_shot(id, hr, shotgun) -> // Shotguns do not cause stalls. Just read diverts.
                            cc // xi_orred shotgun_exit

                        | HR_vl(id, hr, trn, primed_flag, valid_flag) ->
                            let (req_rdy_o_, ack_net) = retrieve_handshake_inputs contact_maps trn
                            let hrok = ix_or (xi_orred ack_net) (xi_orred valid_flag) 
                            (rdyat, hrok) :: cc
                        | other -> sf  (sprintf "stall guards other %A"  other)
            List.fold gsg [] post_tups
    let stall_guards = stall_guards @ start_rdyguards 
    //vprintln 0 ("The full list of stall guards is " + sfold (fun (s, rdy, vld) -> sprintf "%i:%s:%s" s (xbToStr rdy) (xbToStr vld)) stall_guards)
    (bus_ops, stall_guards, thread_busop_rtl (* @ fulls1*), list_flatten(rev !m_pli_newrtl), post_tups, f3o3 map1) // end of res2_form_cam_ops


    
// Create output microsequencer for a thread.
// Or remove microsequencer if there was only a single input resume and the result is fully-pipelined or does not need pipelining. 
let res2_gen_xitions3 ww settings (arc_guards, stall_guards, dir, controllerf, statenamef, next_aidx, npc, xlats) (state, statework, (xc3a, xc3b), (start_n:int), npc__) = 
    let vd = settings.vd
    let m0 = xToStr state
    let (spc, root_hwm, rootwork_) = xc3a

    let _:scheduled_edge_t list = statework

    let (m_arcs, m_reverbs) = (ref [], ref [])
    let m_nomove_condition = ref X_true  // Go back to first microstate if no transition out of major state.
    let mags = // rewritten control flow predicates as a list subscriptable by local_eno.
        match op_assoc state arc_guards with
            | None -> sf (sprintf "recoded arc guard missing")
            | Some mags -> mags

    let mk_xition msg eno mag exec_cycle newpcv dest = // Render a transition in microarc form.
        let adv_guard =
            let rec st_scan2 = function // TODO; this is the same as the stallnet generate ... combine please ... but this is selective on which guards it takes.
                | [] -> X_true
                | ((g_primpc_F, npc), gg)::tt when npc=newpcv -> ix_and gg (st_scan2 tt) 
                | _::tt ->                    st_scan2 tt
            let ans = st_scan2 stall_guards //TODO check these are in final terms
            if xi_istrue ans then None else Some ans

        m_nomove_condition := ix_and (xi_not mag) !m_nomove_condition
        let xdesc = if not settings.pipelining && newpcv=exec_cycle then " EXEC" else ""
        vprintln 3 (sprintf "mk_xition:  (%i, %i)%s" newpcv dest xdesc  + m0 + ": " + msg + ": from=" + xToStr(statenamef("F?", newpcv)) + " g=" + xbToStr mag + " --> " + xToStr(statenamef("F>", dest)) + (if adv_guard<>None then " stall=" + xbToStr (valOf adv_guard) else ""))

        let quad_xition (mag, dest) =
            let ma = { ma_idx=      next_aidx()
                       eno=         eno
                       oldstate=    m0
                       exec_flag=   (exec_cycle = newpcv)
                       gpc=         (npc, statenamef(g_primpc_F, newpcv))
                       mag=         mag                                    // microarc guard.
                       dest=        statenamef dest                        // successor state
                       npcv=        newpcv
                       adv_guard=   adv_guard                              // Advance guard (local resources unstalled) held separately for visualisation only.
                    }
            mutadd m_arcs ma // emit the arc
        quad_xition (mag, (g_primpc_F, dest))



    let xlate f = // Translate old pc to new pc
        let ans = 
            if f = -1 then -1
            else
                match List.filter (fun (f', d) -> f=f') xlats with
                    | [] -> sf (sprintf "no translation for old pc value %i" f)
                    | [(_, (npcv, pc_hwm, _, _))] ->
                        npcv
                    | multiple -> muddy(sprintf "xlate: multiple xitions %A" (map snd multiple))
        //dev_println(sprintf " xlate %i --> %i" f ans)
        ans

    // Create root traversal arcs - same for all edges sharing that resume
    if root_hwm > 0 then app (fun d -> mk_xition "control" -1 X_true -1 d (d+1)) [start_n .. start_n + root_hwm - 1] 

    // Create work edges aka work arcs.
    let _ =
        let mk_microarc ((fil:filx_t, w, waypoint_str, eno, gg_old_, dest, (priv_start_pc, exec_cycle, dend)), local_eno) =
            let gg = (mags:hbexp_t list).[local_eno] // The transition guard, restructured.
           

            let se = // Get sheduled_edge (se) from statework - all rather longwinded owing to history of edits!
                match List.filter (fun (x:scheduled_edge_t) -> x.edge.eno = eno) statework with
                    | [se] -> se
                    | _ -> sf "L4651-se-once"

            match se.pse.asio with
                | Some asi when not_nonep !asi.m_asiso -> // Make one-hot-style state reverb chain. It can, in fact, be more than one hot (re-entrant) for heavily pipelined systems, so we call it shift-hot.
                    let asi_ms = valOf(!asi.m_asiso)
                    // We trigger the reverb shift-hot chain on the last F state, which is the end of the root section or the end of a the post-pad where present.
                    let trig0 = ix_and gg (xi_deqd(npc, statenamef(g_primpc_F, if se.pse.priv_last_pc >= 0 then se.pse.priv_last_pc else start_n+root_hwm))) // i.e. priv_last_pc if it is present or else end of root.
                    let connect_reverb_state (nn, rnet) =
                        let (_, rev_oh_state) = se.pse.reverb_state_names.[nn]
                        let trig = if nn=0 then xi_blift trig0  else snd (se.pse.reverb_state_names.[nn-1] )
                        vprintln 3 (sprintf "mk_xition REVERB %i:  %s -> %s" nn (xToStr trig) (xToStr rev_oh_state))
                        mutadd m_reverbs (gen_XRTL_arc(None, X_true, rev_oh_state, trig))
                    app connect_reverb_state se.pse.reverb_state_names

                | _ -> ()

                
            if vd>=4 then
                let exec_info = if not settings.pipelining then sprintf "exec=%i" exec_cycle else ""
                vprintln 4 (sprintf "  key nos for new microstates for %s eno,local_eno=E%i,%i : ctrstart=%s root_hwm=%i start_n=%i priv_start_pc=%i %s dend=%i dest=%i/%i" m0 eno local_eno (i2sm spc) root_hwm start_n priv_start_pc exec_info dend dest (xlate dest))

            //vprintln 3 (sprintf "  post gg=" + xbToStr gg)
            let local_len =  dend - root_hwm // dend includes root: it is rd_hwm + wr_hwm and rd_hwm includes the root/control span.

            let issue_xitions dest' =
                if local_len = 0 then // If local_len is 0 then we transfer from here straight to ultimate dest
                // Add on post_pad here ? Dally states before going to successor.
                    mk_xition "sedge" eno gg (start_n + exec_cycle) (start_n + root_hwm) dest'
                elif local_len = 1 then  // If local_len is 1 then we have one intermediate state before ultimate dest
                    mk_xition "disp_1" eno gg (start_n + exec_cycle) (start_n + root_hwm) priv_start_pc
                    mk_xition "ledge_1" eno X_true exec_cycle priv_start_pc dest'
                else  // Otherwise we have a number of states in a chain.
                    mk_xition "disp_mt1" eno gg (start_n + exec_cycle) (start_n + root_hwm) priv_start_pc
                    app (fun d -> mk_xition "ledge_mt1" eno X_true (priv_start_pc + exec_cycle - root_hwm) d (d+1)) [priv_start_pc .. priv_start_pc + local_len - 2]
                    mk_xition "ledge_mt1x" eno X_true (priv_start_pc + exec_cycle - root_hwm) (priv_start_pc + local_len - 1) dest'

            // When no controller and restarting our dest is ourselves. When not restarting we will need a pseudo dest to go and hang in.
            if not controllerf && dest < 0 then
                dev_println (sprintf "URGENT: controllerless local_len=%i hang_mode=%A dest=%i" local_len dir.hang_mode dest)
                dev_println ("Revert temporary FSM back to raw RTL for pseudo state" + m0)
                match dir.hang_mode with
                    | DHM_hang
                    | DHM_finish       -> issue_xitions -1
                    | DHM_auto_restart 
                    | _                ->
                        if local_len > 1 then issue_xitions 0 // will have no xitions if local_len=1
                // Will have no xitions if II=1. Truely controllerless.
            else
                let dest' = xlate dest
                issue_xitions dest'
            ()
        app mk_microarc (zipWithIndex xc3b)


    let _ = // Create self-resume
        if vd>=4 then vprintln 4 (sprintf "Restart superstate (nomove) condition =" + xbToStr !m_nomove_condition)
        if controllerf && not (xi_isfalse !m_nomove_condition) then mk_xition "nomove" -100 !m_nomove_condition (-1) (start_n + root_hwm) start_n
        ()
    (!m_arcs, !m_reverbs)

    
//
// res2_build_thread --- new microsequencer for the logic of a reimplemented thread.
//   
let res2_build_thread ww (settings:restruct_manager2_t) m_assist contact_maps ((dir, thread, controllerf, perthread2, arc_freqs_), xon) =
    //dev_println (sprintf "res2_build_thread dir.duid=%i" dir.duid)
    let (npc_arity, majstates, predictor_graph, xlats, stallpoints) = valOf_or_fail "L5866 missed bind stage" xon
    
    let _: ((directorate_t * hexp_t * bool * perthread_t) * int * (hexp_t * (*----->*)scheduled_edge_t list * ((int * int * (int * (cbg_track_t * schedule_temporal_extent_t * newopx_t) list) list) * (filx_t * hexp_t * string * int * hbexp_t * int * (int * int * int)) list) * int * int) list * (int * PGE_d_t list) list list * (int32 * (int * int * (int * bool * string * int) list * 'b list list option)) list * stallpoints_t) = ((dir, thread, controllerf, perthread2), npc_arity, majstates, predictor_graph, xlats, stallpoints) 

    //let _:((directorate_t * hexp_t * perthread_t) * int * (hexp_t * scheduled_edge_t list * ((int * int * (int * (cbg_track_t * schedule_temporal_extent_t * newopx_t) list) list) * (hexp_t * string * int * hbexp_t * int * int * int * int) list) * int * int) list * (int * PGE_d_t list) list list * (int32 * (int * int * (int * bool * int) list * int list list option)) list * stallpoints_t)    =  ((dir, thread, perthread), npc_arity, majstates, predictor_graph, xlats, stallpoints) 
    let (apply_pip_bx, apply_pip_x) = rez_pipate settings.m_morenets perthread2.pipate
    let pip_lmode = function
        | W_asubsc(lhs, subsc, _) -> ix_asubsc lhs (apply_pip_x subsc)
        | other -> other


    let threadgen_XRTL_arc(pp, gl, lhs, rhs) = // First shuffle LHS next-value operators to the other side?
    //X_n on lhs is used to denote combinational assign rather than using the older XRTL_buf construct.
        gen_XRTL_arc(pp, ix_andl(map apply_pip_bx gl), pip_lmode lhs, apply_pip_x rhs) 


    let new_encoding_needed  =
        let always_force = true // disable with restructure=disable in recipe or cmd/line
        let a = ref false
        let new_encoding_check (state, work_, xc3, start_n, end_n) = if start_n <> end_n then a := true
        app new_encoding_check majstates
        if !a then
            vprintln 2 ("New pc encoding needed for " + xToStr thread)
            true
        elif always_force then
            vprintln 3 ("Forcing new encoding to be used though not needed owing to no state renumbering on thread " + xToStr thread)
            true
        else
            vprintln 2 ("No structural hazards found on this thread. No new pc encoding needed for " + xToStr thread)    
            false

    let no_output_pc = not controllerf && npc_arity < 2

    if new_encoding_needed then mutadd settings.m_old_nets_for_removal thread

    let tid = xToStr thread

    let (npco, npcs, statenamef) =
        if no_output_pc then
            vprintln 2 (sprintf "No output PC required: needed for %s with arity %i states" (xToStr thread) npc_arity)
            (None, "NO-OUTPUT-PC", fun _ -> xi_string "NO-OUTPUT-PC")
        elif new_encoding_needed then 
            let vals = [0..npc_arity] // include npc_arity itself as -ve/exit code point.
            let npc =
                let g n = (n, i2s n, "")
                create_enum_net(tid + "nz", (Some (tid + "nz" + "_t"), map g vals)) // new program counter for new states : See XPT below.
            let npcs = xToStr npc
            mutadd settings.m_morenets npc
            vprintln 2 (sprintf "New PC encoding npcs=%s needed for %s using %s with arity %i states" npcs (xToStr thread) (xToStr npc) npc_arity)
            let proto_statenamef p = enum_add "proto_statenamef" npc npcs p
            let preferred_lst = map proto_statenamef vals // side-effecting - keep this even if not printed.
            let exit_point = hd (rev preferred_lst)
            //vprintln 0 (sprintf "pc states enum is " + sfold xToStr preferred_lst)
            // Close the enum now all codepoints have been installed.
            let _ = close_enum "new pc created by restructure" npc
            let statenamef (mc, p) =
                if p < 0 then exit_point // Divert negative ones to the exit point.
                elif mc = g_primpc_F then preferred_lst.[p]
                else simplenet(sprintf "%s%i" mc p)
            (Some npc, npcs, statenamef)
        else
            let statenamef (mc, p) =
                if mc = g_primpc_F then xi_unum p // TODO use proper function for enum
                else simplenet(sprintf "%s%i" mc p)
            (Some thread, xToStr thread, statenamef)


    let snfo = Some statenamef // TODO delete
    //vprintln 3 ("build_thread waypt L3808 " + xToStr thread)

    let stall_guards =
        let qstate0 (state, se_work, xc3, start_n, end_n) cc =

            let _ :(hexp_t * scheduled_edge_t list * ((int * int * (int * (cbg_track_t * schedule_temporal_extent_t * newopx_t) list) list) * (filx_t * hexp_t * string * int * hbexp_t * int * (int * int * int)) list) * int * int) = (state, se_work, xc3, start_n, end_n)
            let (spcv, root_hwm, rootwork) = fst xc3            
            // Initial call: form cam_ops for ctrl/rootwork for this resume.
            let r_adv_guards = res2_check_stalls ww settings "site-root" (dir, thread, state) rootwork 
            // Call for each edge
            let qedge0 se cc =
                let adv_guards = res2_check_stalls ww settings "site-edge" (dir, thread, state) se.work 
                adv_guards @ cc
            let cc = List.foldBack qedge0 se_work (r_adv_guards@cc)
            cc
        List.foldBack qstate0 majstates []
    let nostalls_thread = nullp stall_guards
    vprintln 2 (sprintf "Early variable-latency operation check for whole thread %s: result=%s" (xToStr thread) (if nostalls_thread then "No stalls can happen in this thread" else " It can stall in npcs=%s" + (sfold p2s (stall_guards))))


    // Make the stall corrector for a thread only when determined that it is needed, but this will be after all inter-thread contention has been determined.
    // An alternative plan to next consider is to check where all FUs accept a clock enable input and hence stall correctors are not needed since EVERYTHING on the thread stops at once.  This suits AXI interfaces better.
    let rel_stall_corrector =
        if nostalls_thread then None
        else
            let _ = vprintln 3 (sprintf "Making stall corrector for " + xToStr thread)
            Some(stall_guards, new rel_stall_corrector_t(thread))


    let (late_collate, arc_guards, stall_guards, newrtl2, newrtl_for_stall) =
        let qstate1 (state, se_work, xc3, start_n, end_n) (lc, arc_guards, c, d, d1) =
            let (spcv, root_hwm, rootwork) = fst xc3            
            let m_score_for_root = // cloned for each edge
                {
                    scboard=  ref []
                    live_hrs= ref [] 
                }

            // Initial call: form cam_ops for ctrl/rootwork for this resume.
            let (r_bus_ops, r_adv_guards, r_assist_rtl, r_newrtl_pli, root_map_pairs, root_map) = res2_form_cam_ops (WN "res2_form_cam_ops:root/control" ww) settings m_assist contact_maps (dir, thread, rel_stall_corrector, m_score_for_root) (Filx, X_true, state, start_n, npco, (-1, -1, -1), snfo) [] None rootwork

            // Call for each edge
            let qedge1 (se:scheduled_edge_t) (lc, agc, c, d, d1) =
                let recoded_guard = xi_assoc_bexp g_sk_null root_map se.edge.gtop // Rewrite edge guard using the root map.

                // RTL guards within the edge are done by ?...  pli_guard rewrite is done by res2_form_cam_ops
                vprintln 3 (sprintf "recoded edge guard from %s to %s " (xbToStr se.edge.gtop) (xbToStr recoded_guard))
                
                let (bus_ops, adv_guards, assist_rtl, newrtl_pli, _, _) = res2_form_cam_ops (WN "res2_form_cam_ops:data" ww) settings m_assist contact_maps (dir, thread, rel_stall_corrector, score_clone m_score_for_root) (Filx, recoded_guard, state, start_n, npco, (se.priv_exec_pc, se.priv_start_pc, se.priv_last_pc), snfo) root_map_pairs (Some se) se.work
                ((state, bus_ops)::lc, recoded_guard::agc, adv_guards @ c, assist_rtl @ d, newrtl_pli @ d1)
            let (lc, ag, c, d, d1) = List.foldBack qedge1 se_work ((state, r_bus_ops)::lc, [], r_adv_guards@c, r_assist_rtl@d, r_newrtl_pli@d1)
            (lc, (state, ag)::arc_guards, c, d, d1)
        List.foldBack qstate1 majstates ([], [], [], [], [])


    let m_extra_spog = ref []

    let (stall_net, clear_net) =
        let gocheck = List.fold (fun cc (_, g) -> ix_and g cc) X_true stall_guards
        let needed = not_nonep npco && not(xi_istrue gocheck)
        vprintln 3 (sprintf "stallnets: stall gocheck=%s, needed=%A" (xbToStr gocheck) needed)
        let start_pcl_lst = map (fun (state, statework, xc3, start_pc, end_n) -> start_pc) majstates // We only need include here the major states that require stall correction. Currently we have all major states. TODO perhaps.
        let start_n_lst = start_pcl_lst

        if needed then
            let stall_net = simplenet(xToStr thread + "_stall")
            let clear_net = simplenet(xToStr thread + "_clear")
            vprintln 3 ("Variable-latency operations in thread " + xToStr thread + " stall=" + xToStr stall_net)
            mutapp settings.m_morenets [ stall_net; clear_net]
            (snd(valOf rel_stall_corrector)).wireup (settings.m_morenets, m_extra_spog) stall_net clear_net (valOf npco) start_n_lst 
            (stall_net, clear_net)
        else (X_undef, X_undef)

    let pc_deqd_gen2 pcv = // Compare pc against a state name (or elsewhere guard on a reverb flop) - we have a gen2 and a gen1 - why both?
        if nonep npco  then X_true
        else
            let pcvp = if nonep snfo then xi_num pcv else (valOf snfo) (g_primpc_F, pcv)
            ix_deqd (valOf npco) pcvp

    let assist_rtl0 = 
        let m_r = ref [] // cf m_extra_spog?
        let create_assist_rtl0 = function
            | (_, HR_argreg(holding_reg, argnet)) ->
                //dev_println "ROSIE check fs_inhold please"
                mutadd m_r (RAC_spog, None, pc_deqd_gen2 0, holding_reg, argnet)
            | _ -> sf "L6142 assist-rtl0"
        for xa in settings.scalar_hr_db do create_assist_rtl0 xa.Value done
        !m_r

    let assist_rtl1 = // new site
        let gen x5 =
            let (style, ppo, guard, lhs, rhs) = x5
            x5 // no functionality needed
        let create_assist_rtl1 (ccn, ccl) (hr_style, ppo_, gax, rr) =
            let (nn, once, multiple)=
                match hr_style with
                | HR_vl(id, hr, temp_rdy_net, primed_flag, valid_flag) ->
                    let (req_rdy_o_, ack_net) = retrieve_handshake_inputs contact_maps temp_rdy_net                    
                    // A vl component may have several holding registers, each permanently associated with it, but reused over major states.
                    // Primed is set when the arguments are inserted and valid is set once the result is ready in the holding register.  rd_smart may
                    // forward the output directly from the unit when it needs it in the rdy_net cycle immediately before it is installed in the holding register.

                    // Now done by trigger puller
                    //let multiple0 = gen(RAC_unstallable, None, gax, primed_flag, xi_one) // Set on exec if guard holds

                    let alpha = ix_and (xi_orred ack_net) (xi_orred primed_flag) // When complete, clear primed and set valid
                    //vprintln 0 (sprintf "hr=%s  : unused load g=%s" (xToStr hr) (xbToStr gax))
                    let arc0 = gen(RAC_unstallable, None, alpha,              primed_flag, xi_zero)
                    let arc1 = gen(RAC_unstallable, None, alpha,              valid_flag, xi_one)  // Set when resource flags its result :: TODO - only if primed! This will set all holders on that resource
                    let arc2 = gen(RAC_spog,        None, xi_orred clear_net, valid_flag, xi_zero) // Clear on new superstate
                    let arc3 = gen(RAC_unstallable, None, alpha, hr, rr) // Load holding register with data.  rr is the read bus from the FU.
                    (x2nn valid_flag, [arc0; arc1; arc2; arc3], [])
                    
                | HR_grdten(guardholder_net, exec_cycle, last_cycle) -> // Control guard holding flag.
                    //let multiple0 = gen(RAC_unstallable, stutter exec_cycle, gax, guardholder_net, xi_one) // Set on exec if guard holds
                    let multiple0 = gen(RAC_unstallable, None, ix_and gax (pc_deqd_gen2 exec_cycle), guardholder_net, xi_one) // Set on exec if guard holds - delete me for line above please.
                    let once0     = gen(RAC_spog,        None, pc_deqd_gen2 last_cycle, guardholder_net, xi_zero) // Clear on last minor state of this arc
                    (x2nn guardholder_net, [once0], [multiple0])

                | HR_shot(id, hr, shotgun) -> // Shotgun shiftregister for fixed latency. - Can avoid in future with clock-enabled FUs. TODO.
                    let multiple0 = gen(RAC_unstallable, None, gax, hr, rr) // doubled up on nstf code ? TODO.
                    let rec make_shift_shot = function // Make shift register for shotguns.
                        | a::b::tt ->
                            let arc = gen(RAC_unstallable, None, X_true, b, a)
                            arc :: make_shift_shot (b::tt)
                        | _ -> []
                    (x2nn hr, make_shift_shot shotgun, [multiple0])
                | _ -> sf "L4466"
            let already = memberp nn ccn
            ((if already then ccn else nn::ccn), multiple @ (if already then [] else once) @ ccl)
        snd(List.fold create_assist_rtl1 ([], []) !m_assist)

    let newrtl2 = newrtl2 @ assist_rtl0 @ assist_rtl1  // spog form

    let gen_stall_corrector_ from_pc to_pc =
        //stall_logged = !stall_net  
        // TODO we can optimise this since we know when stalls cannot happen and so need not be corrected for.
        // This is a flipflop - cleared on start of super state.
        let gsc cc n = ix_or cc (ix_and (ix_deqd (valOf npco)  (xi_num n)) (xi_orred stall_net))
        List.fold gsc X_false [from_pc..to_pc]
 
    let newrtl_for_stall = // TODO get rid of this and use clever_tappet (aka spog) forms
        if stall_net = X_undef then (newrtl_for_stall)
        else
            let gs = xi_not(xi_orred stall_net)
            let addit = function // A reguard - surely we have a standard function for that?
                | XRTL(q, work_grd, zl) ->
                    //dev_println (sprintf "Stall re-guard XRTL work_grd=%s" (xbToStr work_grd))
                    XRTL(q, ix_and gs work_grd, zl) // bit messy here!
                | XIRTL_pli(pp, work_grd, fgis, args) ->
                    //dev_println (sprintf "Stall re-guard PLI work_grd=%s" (xbToStr work_grd))
                    XIRTL_pli(pp, ix_and gs work_grd, fgis, args)
                // | XIRTL_buf // TODO
                | other -> sf (sprintf "other RTL form in addit %A" (xrtlToStr other)) // will get pli
            //vprintln 0 (sprintf "RTL IS %A" newrtl)
            map addit newrtl_for_stall

    let aidx = ref 0 // can number freshly for each machine (aids only for log output anyway).
    let next_aidx() =
        let r = !aidx
        aidx := r+1
        ("YY", r)

    let (microarcs, reverbs) =
        if nonep npco then ([], [])
        else
            let gf3 = res2_gen_xitions3 ww settings (arc_guards, stall_guards, dir, controllerf, statenamef, next_aidx, valOf npco, xlats) 
            let (microarcs, reverbs) =
                if new_encoding_needed then 
                    let (ma, rv) = List.unzip(map gf3 majstates)
                    (list_flatten ma, list_flatten rv)
                else ([], [])

            let microarc_table =
                let mat (ma:microarc_t) = [xToStr(snd ma.gpc);
                                           p2s ma.ma_idx; ma.oldstate 
                                           xbToE ma.mag ; xToStr ma.dest ; (if nonep ma.adv_guard then "" else xbToE (valOf ma.adv_guard))
                                          ]
                // These tables will be exported also in XML for debug visualisation in the IDE.
                // We wont have one if no hazards ... trivial designs anyway but would be nice for those to give a plot too.
                create_table("Controller Arcs (revealing old/new PC relationship)", [ "New state name"; "Arc"; "Old state name"; "Guard"; "Dest state"; "VL Guard"], map mat (rev microarcs))
            app (vprintln 2) (microarc_table)
            mutadd settings.tableReports microarc_table

            (microarcs, reverbs)

            
//                                ppg_gen ww settings.anal thread microarcs !perthread2.m_thread_resource_list predictor_graph
    if settings.ppgen_enable then ppg_gen ww settings.anal thread microarcs !perthread2.m_thread_fu_summary_report predictor_graph

    let (fulls2, driven_tapets) = // cam shaft: collate/marshall FU structure control waveforms.
        let (lhs_lst, driven_tapets) =
            let all_bus_ops = list_flatten (map snd late_collate) // nv fst (state) ignored here.
            let gexful only_nonundefsf cc = function
                | (s, nx, g, DB_LOGICAL(ikey1, pi_name, formal_name, meld_kinds, portmeta, nps, externally_instantiated), r, defVal, rdyGrd, spog) ->
                    let lhs = get_actual contact_maps "ee" ikey1 pi_name formal_name
                    //dev_println (sprintf "Retrieved cmap 2/3 for pi_name=%s lname=%s  lhs_net=%s" pi_name formal_name (netToStr lhs))
                    if (only_nonundefsf && defVal=X_undef) || not_nonep(op_assoc lhs cc) then cc else singly_add (lhs, defVal) cc
                    
                | (s, nx, g, DB_PHY lhs, r, defVal, rdyGrd, spog) ->                    
                    if (only_nonundefsf && defVal=X_undef) || not_nonep(op_assoc lhs cc) then cc else singly_add (lhs, defVal) cc
            let lhs_list = List.fold (gexful true) [] all_bus_ops       // collect all assigned nets that do not default to X_undef
            let driven_tapets = List.fold (gexful false) [] all_bus_ops // collect all assigned nets.
            (lhs_list, driven_tapets)
        //dev_println(sprintf "Driven_tapets are " + sfold (fst>>netToStr) driven_tapets)
        let disj_with_defVal2 cc (lhs, defVal) = // Form a disjunction of events that drive this lhs control net away from its default value.
            if lhs = X_undef then cc
            else
            let spb (state, bus_ops) (rhs2, nxo2, spogo2) =
                let rec dij (cc, nxo, spogo) = function
                    | (sl, nx, g, DB_LOGICAL(ikey1, pi_name, formal_name, meld_kinds, portmeta, nps, externally_instantiated), r, a_, rdyGrd, spog) ->
                        let lhs = get_actual contact_maps "ee" ikey1 pi_name formal_name
                        //dev_println (sprintf "2/2 Retrieved cmap checking %s in %A" ikey1 (sfold fst contact_maps))
                        //dev_println (sprintf "Retrieved cmap 3/3 for pi_name=%s lname=%s  lhs_net=%s" pi_name formal_name (netToStr lhs))
//
                        dij (cc, nxo, spogo) (sl, nx, g, DB_PHY lhs, r, a_, rdyGrd, spog) // Convert logical port name to DB_PHY
                        
                    | (sl, nx, g, DB_PHY ll, r, _, rdyGrd, spog) when ll<>lhs -> (cc, nxo, spogo) // TODO we go around again and again on the buildportnet now - even worse than before!
                    | (sl, nx, g, DB_PHY ll, r, _, rdyGrd, spog) ->
                        let emit_cam (cc, nxo, spogo) s =
                            //vprintln 0 (sprintf "dijj lhs=%s oldstate=%s check correct comparison %s cf %s nx=%i" (xToStr lhs) (xToStr state) (xToStr(valOf npco)) (i2s s) nx)
                            if nx<(-1) || nx>1 then sf (i2s nx + "bad nx value")
                            let nx' = 0 // if nx = -1 then -1 else 0
                            let term = if nonep npco then cc else ix_query (ix_and (pc_deqd_gen1 snfo thread state (valOf npco) ((*xi_X nx' *) s)) g) r cc
                            //let _ = if not(xi_istrue rdyGrd) then mutadd m_score.adv_guards (s, rdyGrd) 
                            (term, grab nxo nx, grab spogo spog)
                        List.fold emit_cam (cc, nxo, spogo) sl
                List.fold dij (rhs2, nxo2, spogo2) bus_ops
            let (rhs, nxo, spogo) = List.foldBack spb late_collate (defVal, None, None)
            let nx = valOf_or nxo 0  // unused control net will have no nx (seq/comb assign determiner)
            let l1 = if nx=1 then lhs elif nx=0 then xi_Xm1 lhs else sf "bad nx"
            let g = X_true
            (valOf_or spogo RAC_spog, None, g, l1, rhs)::cc
        let fulls2 = List.fold disj_with_defVal2 [] lhs_lst
        (fulls2, driven_tapets)

    let (xition_rtl, advgrd_conds) =         // Generate the sequencer next state function.  These were generated in microarc form by xitions3, but here we convert to hpr l/s form.
        let gnsf (c, gcc) (ma:microarc_t) = // 
            let (gl, gcc') =
                if ma.adv_guard=None then ([ma.mag], gcc)
                else
                    let gl = [ma.mag ; valOf ma.adv_guard ]
                    (gl, ma :: gcc)
            let arc = threadgen_XRTL_arc(Some(ma.gpc), gl, valOf_or_fail "L6199" npco, ma.dest) // Emit goto next micro state 
            (arc::c, gcc')
        List.fold gnsf ([], []) microarcs

    let stall_net_assign =
        if nullp stall_guards then []
        else
            if stall_net = X_undef || nonep npco then
                vprintln 0 ("+++ stall net was null when indeed needed")
                []
            else
                let stall_mux =
                    let gsnterm2 ((mc__, pcv), g) = ix_and (xi_not g) (ix_deqd (valOf npco) (xi_num pcv)) // TODO use pc_deqd and do not ignore mc__
                    ix_orl (map gsnterm2 stall_guards)
                //vprintln 0 ("stall_mux is " + xbToStr stall_mux)
                [XIRTL_buf(X_true, stall_net, xi_blift stall_mux)] // stall_net is combinational.

    let _ =
        let movToStr (ma:microarc_t) = "   "  + xToStr(snd ma.gpc) + ", " +  xbToStr ma.mag + " --mam--> " + xToStr ma.dest + (if nonep ma.adv_guard then "" else " stall=" + xbToStr (valOf ma.adv_guard))
        reportx 3 ("new PC movements for " + tid) movToStr microarcs


    let arcgen(cf, lhs, gl, rhs) = if cf then XIRTL_buf(ix_andl(map apply_pip_bx gl), pip_lmode lhs, apply_pip_x rhs) else threadgen_XRTL_arc(None, gl, lhs, rhs)

    // spog-form output: clever_tappet:  output processing for RTL nets and cam shaft waveforms that understands and resolves stalls, disjunctions, defaults, dont-cares and both combinational and sequential nets.
    // see all verilog_gen.inti_to_default
    let spog_Arc_Gen (spog, pp, g, lhs, rhs) =
        match spog with
            | RAC_unstallable -> threadgen_XRTL_arc(pp, [g], lhs, rhs)
            | RAC_trigger -> // The xition 0 to 1 is stallable but the 1 to 0 is not
                let gs = xi_not(xi_orred stall_net)
                let r2 = ix_and gs (xi_orred rhs)
                threadgen_XRTL_arc(pp, [], lhs, xi_blift r2)

            | RAC_spog // Already stallanded ...
            | RAC_stallable ->
                if stall_net = X_undef then threadgen_XRTL_arc(pp, [g], lhs, rhs)
                else 
                    let gs = xi_not(xi_orred stall_net)
                    threadgen_XRTL_arc(pp, [g; gs], lhs, rhs)

    let newrtl_for_stall = // misnamed since has PLI in it too for the moment
        let pli_pipate = function
            | XIRTL_pli(pp, gg, fgis, args) ->
                //vprintln 3 (sprintf "shove pipate final call %s %s" (fst(fst fgis)) (sfold xToStr args))
                XIRTL_pli(pp, apply_pip_bx gg, fgis, map apply_pip_x args)
            | XRTL(pp, gg, lst) ->
                let pip2 = function
                    | Rarc(gb, lhs, rhs)   -> Rarc(apply_pip_bx gb, pip_lmode lhs, apply_pip_x rhs)
                    | Rpli(gb, fgis, args) -> Rpli(apply_pip_bx gb, fgis, map apply_pip_x args)
                    | Rnop ss              -> Rnop ss
                XRTL(pp, apply_pip_bx gg, map pip2 lst)
            | XIRTL_buf(pp, lhs, rhs) ->  XIRTL_buf(pp, pip_lmode lhs, apply_pip_x rhs)
            
            | XRTL_nop ss -> XRTL_nop ss

        map pli_pipate newrtl_for_stall


    let p2 = (map spog_Arc_Gen (newrtl2 @ fulls2)) @ newrtl_for_stall


    // OLD: The restructure code does not always write out an FSM at the moment, so the proper place for pc_export, in the render functions, is replicated here.
    // OLD: This has the advantage that it ports to C++/SystemC output automatically, but gbuild should make that seamless.
    // OLD: This should now go in gbuild.fs to be called both from SystemC and RTL output.
    // OLD: We should only need to list the PC if it is not implicit in an SP_fsm output form.
    let (pc_export_nets, pc_export_logic) =
        if not dir.pc_export || nonep npco then ([], [])
        else
            // The pc_export is a monitor function for the PC of the current thread.
            let buffer_reg = cloneio "" "_pc_export" (Some OUTPUT) (valOf npco)
            vprintln 3 (sprintf "Making PC export %s" (xToStr buffer_reg))
            let logic = gen_XRTL_arc(None, X_true, buffer_reg, (valOf npco))
            ([buffer_reg], [logic])


    let pipate_rtl = // Pipeline stages r
        let gen_pipate_arc (lhs, rhs) = gen_XRTL_arc(None, X_true, lhs, rhs)
        map gen_pipate_arc !(snd perthread2.pipate)

    let all_rtl = pc_export_logic @ pipate_rtl @ reverbs @ stall_net_assign @ p2 @ map arcgen !m_extra_spog @ xition_rtl 
    let answer =  
        let ii = { id="from_restructure" } : rtl_ctrl_t
        if settings.regen_sequencer && not_nonep npco then
            let vd = settings.vd
            let ww = WF 3 "res2_regen_sequencer_project" ww "start"
            let hardf = { g_default_visit_budget with hardf=true } // Hard clock boundaries
            // Convert to FSM form.
            let (fsml, further_rtl, old_trajectory_, stateNames) = project_arcs_to_fsm ww "restructure: postrender" vd settings.avoid_dontcare_stall (valOf npco) hardf all_rtl
            vprintln 2 ("Using FSM projection at end of res2_build_thread")
            let ww = WF 3 "res2_regen_sequencer_project" ww "done"
            let frtl = if nullp further_rtl then [] else [SP_rtl(ii, further_rtl)]  // RTL output and FSM output where possible.
            H2BLK(dir, gen_SP_par PS_lockstep (frtl @ fsml))
        else
            vprintln 2 (sprintf "No FSM projection at end of res2_build_thread: settings.regen_sequencer=%A npco=%A" settings.regen_sequencer (if nonep npco then "None" else xToStr(valOf npco)))
            H2BLK(dir, SP_rtl(ii, all_rtl))

    (pc_export_nets, (perthread2.vm2_ids, answer), driven_tapets) // end of res2_build_thread.
    

   
// We call the control nets to FUs the tappets since they follow a 'camshaft' waveform.
// Here we tie off those nets that are not otherwise used. Combinational assigns are needed.
// All camnets, being commanders, default to zero, whereas operand/address busses default to X
let gec_unused_tappets_tieoffs ww settings contact_maps driven_tapets = 
    let vd = settings.vd
    let msg = "gec_unused_tappets_tieoffs"
    let ww = WF 2 msg ww "Start"
    let driven_set =
        let form_driven_set (cc:Set<hexp_t>) (lhs_net, defVal_) =
            cc.Add lhs_net
        List.fold form_driven_set Set.empty (list_flatten driven_tapets)

    let dangling_set =
            let bax3 cc ((_, ll), nps) = 
                    if Set.contains ll driven_set then
                        //dev_println (sprintf "Driven cam net " + netToStr ll)
                        cc
                    else
                        match ll with
                            | X_bnet ff when nps.sdir = "i" -> // We need to know if it is an input to the FU - the net itself may be a LOCAL or output.
                                if true || vd>=4 then vprintln 4 (sprintf "Tie off otherwise-unused (dangling) FU-input cam net " + netToStr ll)
                                let defVal = get_init msg ll
                                (xi_X -1 ll, defVal) :: cc // Make a combinational/continous assign wuth X^-1 on lhs.
                            | other ->
                                //vprintln 5 (sprintf "Other (not an output) dangling cam net " + netToStr ll)                                
                                cc

            let bax1 cc kk (contacts:decl_binder_t list, phys_contacts, portmeta_, str2) = List.fold bax3 cc phys_contacts
            Map.fold bax1 [] contact_maps        


    let unused_tappets_tieoffs =
        let lift (lhs, ival) cc = // TODO perhaps generate in spog form? Why? Allows mix of seq and comb. And also the reset value will be automatically applied, which is all we need. Actually RTL render should auto reset and this is then not needed here at all! TODO: write up the semantics of rtl render w.r.t reset and other directorate behaviours (abend code, clock enable).
            if nonep ival then cc else gen_XRTL_arc(None, X_true, lhs, xi_bnum(valOf ival))::cc
        List.foldBack lift dangling_set []

    let ww = WF 2 msg ww (sprintf "Finished. Created %i" (length unused_tappets_tieoffs))
    let combinational_dir = { g_null_directorate with duid=next_duid() }

    if nullp unused_tappets_tieoffs then []
    else
        let ii = { id="from_restructure:unused_tappets_tieoffs" } : rtl_ctrl_t
        [ H2BLK(combinational_dir, SP_rtl(ii, unused_tappets_tieoffs)) ]




//
// Setup controlling paramters from recipe or command line.
//
let baseline_settings ww c1 vd stagename =
    let m_offchipped = ref []
    let toolname = stagename
    let m_settings_report = ref []

    let log_setting_for_report key def vale description = mutadd m_settings_report (key, vale, description)
        
    let get_alu_setting mode (key, description) = // Create recipe keys for tech prams and look them up in the recipe (or command line if overridden)
        let def = 5
        let (varilat, flash_limit, baseline, perbit) =
            if mode = 2 then // Floating-point units
                let perbit = -1
                let ss = control_get_s stagename c1 (key) (Some "5")
                let (varilat, baseline) = if strlen ss > 1 && ss.[0]='V' then (true, atoi32(ss.[1..])) else (false, atoi32 ss) //Specifiy varilatency with a leading V
                let flash_limit = -1
                (varilat, flash_limit, baseline, perbit)
            else // Integer units  - use the per-bit derating.
                let perbit = control_get_i c1 (key + "-perbit") 1
                let ss = control_get_s stagename c1 (key + "-baseline") (Some "1000")
                let (varilat, baseline) = if strlen ss > 1 && ss.[0]='V' then (true, atoi32(ss.[1..])) else (false, atoi32 ss) //Specifiy varilatency with a leading V
                let flash_limit = control_get_i c1 (key + "-flash") 10
                (varilat, flash_limit, baseline, perbit)

        log_setting_for_report key def (int64 baseline) description // Only part of the story?

            
        let rei_interval = if varilat then baseline else 1
        { varilat=      varilat
          flash_limit=  flash_limit
          baseline=     baseline
          perbit=       perbit
          rei_interval=    rei_interval           
        }
        
    let get_alu_limit(key, desc) =
        let def = 8
        let vale = control_get_i c1 key def    
        log_setting_for_report key def (int64 vale) desc
        //vprintln 2 (sprintf "    baseline setting %s = %i   // %s " key vale desc)
        vale

    let get_threshold_setting tag def =
        let vale = control_get_i64 c1 tag def
        log_setting_for_report tag def vale ""
        vale


    // dot plotting should be a separate analysis recipe stage - not part of restructure.fs
    // There is a dot output generator in reporter.fs - it might have less info than we have here.
    let anal_manager:anal_manager_t =
        {
               reprint=  control_get_s stagename c1 "dotplot-reprint" None <> "disable";
               dot_plots= ref [];
               dot_plot_combined=   control_get_s stagename c1 "dotplot-plot" None = "combined";
               dot_plot_separately= control_get_s stagename c1 "dotplot-plot" None = "separately"
               dot_plot_detailed=  control_get_s stagename c1 "dotplot-detailed" None = "enable"
        }

    let tool2 = "res2"
    let externally_flagged_kinds =
        let ss = arg_assoc_or "" (tool2 + "-external-afus", c1, [])
        split_string_at [ ';'; ':'  ] (sfold_colons ss)

    let m_table_reports = ref []
    let m_morenets = ref []
    let m_stall_nets = ref []

    let bondouts =
        let px x = x //(x, ref false)
        let pair_with_usedflag ((space, ports), manager) = (space, map px ports, manager)
        map pair_with_usedflag (get_bondout_port_prams ww c1 stagename log_setting_for_report)



    let settings:restruct_manager1_t =
        { filler1=             "filler1"
          stagename=           stagename
          vd=                  vd
          offchips=            bondouts
          //anal=                anal_manager          
          tableReports=        m_table_reports          
          m_morenets=          m_morenets //ref []
          stall_nets=          m_stall_nets // no longer used? Just denotes number of clock nets in use.
        }

    let (pipelining, extra_post_pad) = 
        let ctrls = control_get_s stagename c1 "res2-killpipelining" None 
        let pipelining = ctrls <> "oldstyle"
        let extra_post_pad =
             match ctrls with
                | "pad1" -> 1
                | "pad4" -> 4
                | "pad10" -> 10                                
                | "pad20" -> 20                
                | _ -> 0
        (pipelining, extra_post_pad)

    let settings:restruct_manager2_t =
        {      stagename=                    stagename
               offchips=                     bondouts // { offchips with offchip_disable=true }
               static_manifest=              ref []
               m_old_nets_for_removal=       ref []
               scalar_hr_db=                 new OptionStore<string, hexp_t * hr_scoreflop_t>("scalar_hr_db")
               externally_flagged_kinds=     externally_flagged_kinds
               regfile_threshold=            get_threshold_setting "res2-regfile-threshold" 8L
               combram_threshold=            get_threshold_setting "res2-combram-threshold" 32L
               combrom_threshold=            get_threshold_setting "res2-combrom-threshold" 64L  
               offchip_threshold=            get_threshold_setting "res2-offchip-threshold" 32000L  
               anal=                         anal_manager
               pipelining=                   pipelining
               extra_post_pad=               extra_post_pad
               schedule_attempts=            control_get_i c1 "res2-schedule-attempts" 4
               asubsc_rmode_merge_enable=    control_get_s toolname c1 "res2-share-array-reads" None = "enable"
               regen_sequencer=              control_get_s toolname c1 "res2-regen-sequencer" None = "enable"
               ppgen_enable=                 false                          // Performance graph plot - todo set from recipe
               include_bondout_ports_when_not_even_used= control_get_s toolname c1 "always-include-loadstore-ports" None <> "disabled"

               extend_for_pli_mode= control_get_s toolname c1 "res2-extend-schedules-to-keep-pli-order" (Some "all-but-console")

               tableReports=                 m_table_reports
               memtech_mapping=              new memtech_mapping_t()
               vd=                           vd
               avoid_dontcare_stall=         true
               stall_nets=                   m_stall_nets // no longer used? Just denotes number of clock nets in use.
               m_morenets=                   ref []

               alu_latency_fp_sp_mul= get_alu_setting 2 ("fp-fl-sp-mul", "Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.");
               alu_latency_fp_sp_add= get_alu_setting 2 ("fp-fl-sp-add", "Fixed-latency ALU floating-point, single-precision floating-point latency value for add/sub.");
               alu_latency_fp_sp_div= get_alu_setting 2 ("fp-fl-sp-div", "Fixed-latency ALU floating-point, single-precision floating-point floating-point latency value for divide.");
               alu_latency_fp_dp_mul= get_alu_setting 2 ("fp-fl-dp-mul", "Fixed-latency ALU floating-point, double-precision floating-point latency value for multiply.");
               alu_latency_fp_dp_add= get_alu_setting 2 ("fp-fl-dp-add", "Fixed-latency ALU floating-point, double-precision floating-point latency value for add/sub.");
               alu_latency_fp_dp_div= get_alu_setting 2 ("fp-fl-dp-div", "Fixed-latency ALU floating-point, double-precision floating-point latency value for divide.");

               // The following, somewhat crude. static limits will be replaced with a generalised heuristic-based searcher soon ...
               // Deployment limits.
               ram_max_data_packing= get_alu_limit("max-ram-data_packing", "Maximum number of user words to pack into one RAM/loadstore word line.")
               rom_mirrors= get_alu_limit("max-no-rom-mirrors", "Maximum number of times to mirror a ROM per thread.")

               per_thread_static_fu_limits =
                   let limits= new per_thread_static_fu_limit_t("limits")
                   let set_limit (key, desc, defv_) =
                      let vale =  get_alu_limit(key, desc)
                      limits.add key vale
                   app set_limit g_quota_key_manifest
                   limits

               alu_latency_int_mul=  get_alu_setting 1 ("int-flr-mul", "Fixed-latency integer ALU integer latency scaling value for multiply.")
               alu_latency_int_div=  g_alu_latency_int_div
               alu_latency_int_addsub=  g_alu_latency_int_addsub               
               //alu_iproduct_limit=  control_get_i c1 "int_fl_limit_mul" 15 - see flash limit in atp.flash_limit
               roms_have_ren=                      false
               next_eno= ref 900 // A distinctive starting number.
               //grdtens= ref []
             }
    let _ =
        // Custom interconnect between separate compilations also shows up here in the port list ??
        if nullp settings.offchips then vprintln 2 (sprintf "Compilation uses no offchip/bondout ports")
        else bondout_port_report ww true toolname "Res2 Preliminary" settings.tableReports settings.offchips 

    (settings, !m_settings_report)


let gen_res_report settings items =
    let headings = [ "Key";"Value"; "Description" ]
    let repfun (key, (vale:int64), desc) = [ key; i2s64 vale; desc ]
    let t1 = create_table("Restructure Technology Settings", headings, map repfun items)
    vprintln 2 "Printed Technology Setting Summary"
    aux_report_log 1 settings.stagename  t1
    mutadd settings.tableReports t1 
    ()


//
// Walk input VM tree and collate per clock domain and per FSM
// Although we flatten, using vm2_id, we keep track of which VM the code came from to put back accordingly afterwards.
//
let res2_trawl ww toolname vm0 =
    let _ = WF 3 "Restructure2: processor" ww "res_trawl"
    let rec collater (vm0, execs0) = function
        | (ii, None) -> (vm0, execs0)

        | (ii, Some(HPR_VM2(minfo, decls, sons, input_execs, assertions))) ->
            vprintln 2 (sprintf "Collating work from VM name=%s" (vlnvToStr minfo.name))
            let vm2_id = minfo.name
            let openr_tmp (sp0, cc) arg =
                let add (cc:Map<directorate_t * bool * hexp_t, vm2_id_t list * xrtl_t list * native_fun_signature_t list>) key vale =
                    let (ov0, ov1, ov2) = 
                        match cc.TryFind key with
                            | None -> ([], [], [])
                            | Some(ov0, ov1, ov2) -> (ov0, ov1, ov2)
                    let nv0 = singly_add vm2_id ov0
                    let nv1 = vale::ov1  // NB all reversed in this collation.
                    let nv2 = valOf_or_null minfo.hls_signature @ ov2
                    cc.Add(key, (nv0, nv1, nv2))

                let openr2 dir (co, cc) arg =
                    //cassert(not_nullp dir.clocks, "null clocks L4449")
                    let tok = gec_X_net (dir.duid) // Form a pseudo fsm name from the director id.
                    match arg with
                        | XIRTL_buf(g, l, r)                -> (co, add cc (dir, false, tok) arg)
                        | XIRTL_pli(Some(pc, psc), _, _, _)
                        | XRTL(Some(pc, psc), _, _)         -> (co, add cc (dir, false, pc) arg)

                        | XRTL(None, _, _) 
                        | XIRTL_pli(None, _, _, _)          -> (co, add cc (dir, false, tok) arg)

                let rec openr1 dir (co, cc) = function
                    | SP_comment l -> (co, cc)
                    | SP_par(pform, lst) -> List.fold (openr1 dir) (co, cc) lst // pform ignored !
                    | SP_rtl(ii_, lst) ->
                        let (co, cc) = List.fold (openr2 dir) (co, cc) lst
                        (co, cc)
                    | SP_fsm(info, arcs) ->
                        if true then // Destroy FSM factorisation (this seems a little brutal) and refactorise. It might be better to allow the preceeding stage to retain more control since we generate one schedule per edge and there are potentially multiple edges between a given resume and destination in the provided factorisation.  By conglomorating edges we create longer schedules since all possible reads and writes are included in the schedule despite it being more qualified.
                            let (cmds, resets_) = fsm_flatten_to_rtl ww ("anon_mach_id") (SP_fsm(info, arcs))
                            (co, List.fold (fun cc->add cc (dir, info.controllerf, info.pc)) cc (map snd cmds))
                        else sf "unused (SP_fsm(info, arcs)::cm, co, cc)"
                    | other -> (other :: co, cc)
                match arg with
                    | H2BLK(dir, v) ->
                        let (co, cc) = openr1 dir ([], cc) v
                        ((map (fun x->H2BLK(dir, x)) co) @ sp0, cc)

            let (spare_execs, execs1) = List.fold openr_tmp ([], execs0) input_execs
            let (sons', execs2) = List.fold collater ([], execs1) sons
            // Gut the VMs, but leaving spare (non FSM) execs behind.
            let ii =
                { ii with
                      generated_by= toolname
                  //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
                }
            let vm' = (ii, Some(HPR_VM2(minfo, decls, sons', spare_execs, assertions)))
            (vm' :: vm0, execs2)


           
    let (vmf, collated2) = collater ([], Map.empty) vm0
    let threads0 = (Map.toList collated2) 
    let _ =
        let tidToStr ((dir, controllerf, tid), (rtl, vm_ids, sigs_)) = (if nullp dir.clocks then "no clock" else sfold edgeToStr dir.clocks) + sprintf " controllerf=%A " controllerf + " tid=" + xToStr tid + sprintf " arcs=%i" (length rtl)
        reportx 3 "Restructure2 - RTL threads in use" tidToStr threads0

    let resolve_sigs a b =
            muddy (sprintf "resolve sigs ROSIE %A  cf %A" a b)

    // Now collate by thread id to check we only have one directorate per thread.
    let reorg =
        let pivotf ((dir, controllerf, thread), (vm2_keys, arcs_revd, sigs)) = thread
        let raw = generic_collate pivotf threads0
        let reorganise (thread, lst) = 
            let rec reorg = function
                | [] -> muddy "no guts"
                | [((dir, cf, _), (vm2_ids, arcs_revd, sigs))] -> (dir, cf, vm2_ids, arcs_revd, sigs)
                | ((dir0, cf, _), (vm2_ids0, arcs_revd0, sigs0))::((dir1, _,  _), (vm2_ids1, arcs_revd1, sigs1))::tt ->
                    vprintln 3 (sprintf "Condensing more than one directorate setting for thread %s" (xToStr thread))
                    reorg (((directorate_resolve dir0 dir1, cf, thread), (lst_union vm2_ids0 vm2_ids1, arcs_revd0 @ arcs_revd1, resolve_sigs sigs0 sigs1))::tt)
            (thread, reorg lst)
        map reorganise raw

    // reorg: (hexp_t * (directorate_t * bool * xrtl_t list * native_fun_signature_t list)) list    
    (vmf, reorg) // end of res2_trawl


    
// One all threads are mapped, we collate the threads per clock domain or per mgr or per 
// Inter-thread, inter-domain port allocator... (dual-ported) DPRAMs only; we do not share ALUs and other on-chip FUs across threads at the moment.
let res2_rezbind ww settings bounders r33 = 
    let ww = WF 2 "res2_rezbind" ww "Start"
    let vd = settings.vd
    let wr ss = vprintln 3 ss
    let concise_pred = function
        | SR_fu _ -> false

    let get_mkey = function
        | SR_fu(mkey, _, _, _)  -> mkey


    let thread_lookup tid =
        let rec tlookup_scan = function
            | [] -> sf ("Lost thread " + xToStr tid)
            | ((_, _, _, tinfo:perthread_t, arc_freqs_), _)::tt when tinfo.thread = tid -> tinfo
            | _::tt -> tlookup_scan tt
        tlookup_scan r33
            
    // Items that are in a root need to be copied into each edge of that resume.
    // Has that copying been done already?  No, we only collate from the root hwm upwards on the edge?
    // Where they can be shared, opx_merge has already acted.
                
    let collated_work =
        let m_collated = ref []
        let fu_tad_collate ((_, _, controllerf_, tinfo:perthread_t, arc_freqs_), _) = 
            wr ("Report for thread " + (xToStr tinfo.thread))
            let collate_for_resume resume (mapping_needs:mapping_needs_t) =
                let details_for_report =
                    let m_m = ref []
                    let collate_for_need kk (mkey_, resume, eno_o, sr, knnlst) =
                        if concise_pred sr then ()
                        else
                            mutadd m_collated ((get_mkey sr), (sr, tinfo.thread, tinfo.director, resume, -1, knnlst))
                            mutadd m_m (sprintf "rezbind %s knnlst=%s" (get_mkey sr) (sfold i2s knnlst))
                    for vv in mapping_needs do collate_for_need vv.Key vv.Value done
                    !m_m
                wr (sprintf "   collate_for_resume  %s  " (xToStr resume) + (sfold id details_for_report)) 
            let collate_for_edge edge_no (mapping_needs:mapping_needs_t) =
                let details_for_report =
                    let m_m = ref []
                    let collate_for__need kk (mkey_, resume, eno_o, sr, knnlst) =
                        if concise_pred sr then ()
                        else
                            mutadd m_collated ((get_mkey sr), (sr, tinfo.thread, tinfo.director, resume, edge_no, knnlst))
                            mutadd m_m (sprintf "rezbind %s %s" (get_mkey sr) (sfold i2s knnlst))
                    for vv in mapping_needs do collate_for__need vv.Key vv.Value done
                    !m_m
                wr (sprintf "    collate_for_edge E%i  " edge_no + (sfold id details_for_report)) 
            for kv in tinfo.mapping_log_for_resume do collate_for_resume kv.Key kv.Value done
            for kv in tinfo.mapping_log_for_edge do collate_for_edge kv.Key kv.Value done            
            wr (sprintf "     Mapped FUs       %i" (length !tinfo. m_thread_fu_summary_report))
            wr (sprintf "     Mapped FU mkeys  %s" (sfold fst !tinfo. m_thread_fu_summary_report))
            ()
        app fu_tad_collate (r33)
        !m_collated
    vprintln 2 (sprintf "resbind: %i mgrs need binding" (length collated_work))


    // We coallesc the root and edge use counts ... TODO how?
    // The key to an SR_fu (its first field) is the iname when non-mirrorable and a kkey when mirrorable.  (Or else ensure one mgr is used for all ports where port binding is fluid?)
        
    // Find non-mirrorable mgr uses that are shared over clock domains
    let domain_collated =
        let domain_pivotf (key, (sr, thread, director, resume, edge_no, knn)) =
            if mirrorable_sr sr then "X_undef" else key
        generic_collate domain_pivotf collated_work

    vprintln 2 (sprintf "rezbind: %i mgr domains (including mirrorable dummy)" (length domain_collated))    

    //We check the number of threads and clock domains in each non-mirrorable mgr to plan the partition.
    let check_interthread (cc, cd, ce) = function
        | ("X_undef", mirrorables) -> (cc, cd, mirrorables @ ce)
        | (key, items) ->
            let tdom_pivotf (key, (sr, thread, director, resume, edge_no, knn)) = (thread, director)
            let pairs = generic_collate tdom_pivotf items
            if length pairs > 1 then (key::cc, cd, ce) else (cc, key::cd, ce)
    let (shared, privates, mirrorables) = List.fold check_interthread ([], [], []) domain_collated
    vprintln 2 (sprintf "rezbind: Global FU summary: private_mgrs=^%i shared mgrs=^%i.  mirrorables^=%i Shared=%s" (length privates) (length shared) (length mirrorables) (sfold id shared))


    let m_all_resources = ref []

    let  (m_bindings_private, m_bindings_mirrorable) = (ref [], ref [])
  
    let rez_head callsite msg threads director key sr infoo fresh_inamef =
        let oraclef _ _  = muddy "need oracle"

        let (iname, str2, mgrs, infoo) =
            match sr with
                | SR_fu(_, auxix, SR_ssram(base_kind, ff, tech, prec, (w, l), _, romf, atts), _) when offchipp tech ->
                    // bondouts are shared quite often - TODO
                    // assume private here
                    match tech with
                        | T_Offchip_RAM(auxix, spacename) ->
                            let portno = 0 // for now TODO
                            let portflattener (space, ports, manager) cc = map (fun port -> (space, port, manager)) ports @ cc
                            let pflat = List.foldBack portflattener (settings.offchips) []
                            let (space, pp, manager) = select_nth portno pflat
                            //m_used_flag := true
                            // TODO allocate user space name and load balance and capacity balance
                            if vd >= 5 then vprintln 5 (sprintf "T_Offchip_RAM: We want space %s and port %i out of %i" spacename portno (length !settings.static_manifest))
                            let (str2, mgr, portmeta_) = valOf_or_fail "bondout port is not on static manifest" (op_assoc portno !settings.static_manifest)

                        // The user's wanted data width is either some integer multiple number of external words, or some fraction of a word where the fraction is rounded up to a bounding power of 2 number of lanes.
                            let oc_record = manager.inject_ram (ff) pp
                            let layout = oc_record.layout
                            dev_println (sprintf "Ignore layout %A" layout)
                            let iname = pp.ls_key
                            if not_nonep infoo then sf ("infoo already present where layout noting needed")
                            let infoo = Some(INFO_layout(pp, oc_record))
                            (iname, str2, [mgr], infoo)

                | sr ->
                    let (str2, mgrs) = rezzer_fu ww settings callsite msg director oraclef fresh_inamef sr infoo
                    let iname = str2.ikey
                    //dev_println (sprintf "rez_head %s  nn=%A key=%s iname=%s mgkeys=%s" msg (if nonep oldx_o then 0 else x2nn(valOf oldx_o)) key iname (sfold (fun mgr->mgr.mg_uid) mgrs))
                    (iname, str2, mgrs, infoo)

        let reza thread =
            let perthread = thread_lookup thread
            app (fun mgr -> mutadd perthread.m_thread_fu_summary_report (mgr.mg_uid, mgr)) mgrs
            vprintln 3 (sprintf "   Adding FU to thread %s" (xToStr thread))
            bounders.global_resource_thread_use.add iname (xToStr thread)
        app reza threads
        bounders.global_resources.add key str2

        let perthread_pipate = (new pipate_cache_t("pipate_cache"), ref [])  //TODO this is rezzed more times than is correct?  Please explain ....
        // TODO do not need all these pipates and certainly dont need one for an offchip
        app (fun mgr -> vprintln 2 (sprintf "bounders.global_mgrs.add mgr.mg_uid=%s" mgr.mg_uid)) mgrs
        let _ = 
            let addx mgr = // Only add once to avoid equality test hang.
                if mgr.mg_uid="" then sf ("splat-L6667")
                match bounders.global_mgrs.lookup mgr.mg_uid with
                    | [] -> bounders.global_mgrs.add mgr.mg_uid mgr
                    | _  -> ()
            app addx mgrs


        mutadd m_all_resources (perthread_pipate, director, str2)
        //dev_println (sprintf "   Exit rez %s" "(xToStr thread)")
        (iname, str2, mgrs, infoo)


    let rez_shared shared_fu = // An FU that is shared between threads and possibly between clock domains.
        let uses =
            let scanx (key, (sr, thread, director, resume, edge_no, knn)) cc =
                if key = shared_fu then (key, (sr, thread, director, resume, edge_no, knn))::cc else cc
            List.foldBack scanx collated_work []
        let sr = once "L6563" (list_once(map (fun(key, (sr, thread, director, resume, edge_no, knn)) -> sr) uses))
            
        let domain_collated_uses =
            let domain_pivotf (key, (sr, thread, director, resume, edge_no, knn)) = director.duid
            generic_collate domain_pivotf uses

        let clk_domain_collated_uses =
            let domain_pivotf (key, (sr, thread, director, resume, edge_no, knn)) = director.clocks
            generic_collate domain_pivotf uses

        let thread_collated_uses =
            let domain_pivotf (key, (sr, thread, director, resume, edge_no, count)) = thread
            generic_collate domain_pivotf uses

        match sr with

            | SR_fu(_, auxix, SR_ssram(base_kind, ff, tech, prec, (w, l), _, romf, atts), _) when offchipp tech -> muddy "L6536"

            | SR_fu(key, auxix, SR_ssram(kind, ff, tech, prec, (wanted_width, l), unused_, is_rom, atts), _) when length clk_domain_collated_uses = 1 && length thread_collated_uses = 2 ->
                vprintln 2 (sprintf "Dual-port SRAM rez. %i uses. %i domains. %i threads. item=%s" (length uses) (length clk_domain_collated_uses) (length thread_collated_uses)  shared_fu)
                let director = (hd>>snd>>f3o6) uses // It should not matter which director is being used, since, for now at least, we are only looking at the clock net which we are sure is the same.
                let (iname, str, mgrs, infoo) = rez_head "shared" "DPRAM REZ" (map fst thread_collated_uses) director key sr (Some(INFO_no_of_ports 2)) false
                let xref = map (fun ((tid, uses), idx) -> (xToStr tid, i2s idx)) (zipWithIndex thread_collated_uses) // Could combine this with infoo really
                let info_xref = INFO_xref xref
                mutadd m_bindings_private (key, (iname, str, mgrs, Some info_xref, xref(*dont need this xref field now in infoo*)))
                ()

            | other_sr ->
                let repf (id, (str, thread, dir, resume, eno, count)) = sprintf " use=%s  domain=%s  thread=%s" id (sfold edgeToStr dir.clocks) (xToStr thread)
                reportx 0 "Use report" repf uses
                sf (sprintf "Component shared between threads or clock domains cannot be shared: FU rez. %i uses. %i domains.  Automatic instancing of clk-domain bridge is not implemented (would not be hard to provide). %i threads. sr=%A" (length uses) (length clk_domain_collated_uses) (length thread_collated_uses) other_sr) // such as dual-port RAM

    app rez_shared shared 

    let mirrorbase_fu = new OptionStore<thread_idx_t * string * int, xidx_t * (str_instance_t * str_methodgroup_t list * fu_aux_info2_t option * (string * string) list)>("mirrorbase_fu")
    let mirrorbase_a = new OptionStore<thread_idx_t * string, int>("mirrorbase_h")
    //let mirrorbase_h = new OptionStore<thread_idx_t * string, int>("mirrorbase_h")        


    let mirrorbase_allocated key =
        let ans = valOf_or (mirrorbase_a.lookup key) 0 // Number allocated for this thread (should be 0 to allowance)
        ans
    //let mirrorbase_hwm key = valOf_or (mirrorbase_h.lookup key) -1

    let m_mirrorable_affinities = ref []     // For mirrorables, we prefer to bind to an FU that has at least one arg in common, where commutative, to reduce multiplexor generation.


    let rez_mirrorable_real msg thread director key sr sans_iname knn = // An FU that can be instantiated freely and whose instances are interchangable.  The binder will create some number of instances.  The (squirrelled?) mkey will be the same for all instances and the binder also creates a mapping from resume/edge to a given instance.
        let affine_search = op_assoc // for now
        match affine_search knn !m_mirrorable_affinities with
            | Some (iname, str, mgrs, iinfo) ->
                //dev_println (sprintf "reuse exact affine %s  nn=%A key=%s iname=%s mgkeys=%s" msg knn key iname (sfold (fun mgr->mgr.mg_uid) mgrs))
                (str, mgrs, iinfo)
            | None ->
                let (iname, str, mgrs, infoo) = rez_head "mirrorable" msg [thread] director key sr None (not sans_iname)
                mutadd m_mirrorable_affinities (knn, (iname, str, mgrs, infoo))
                //dev_println (sprintf "rez_head %s  nn=%A key=%s iname=%s mgkeys=%s" msg knn key iname (sfold (fun mgr->mgr.mg_uid) mgrs))
                (str, mgrs, infoo)

    let binding_budgets = new OptionStore<thread_idx_t, per_thread_structure_counter_t>("binding_budgets")
    let get_thread_fu_counters thread =
        match binding_budgets.lookup thread with
            | Some thread_fu_counters -> thread_fu_counters
            | None ->
                let thread_fu_counters = rez_structure_count_limits settings // Allocate fresh thread resource limits
                binding_budgets.add thread thread_fu_counters
                thread_fu_counters                

    let get_m_alu_allowance thread fp oo =
        let thread_fu_counters = get_thread_fu_counters thread
        let getg key =
            match thread_fu_counters.lookup key with
                | Some vale -> Some vale
                | None ->
                    vprintln 2 (sprintf "restructure: getg quota_key=%s: has no quota." key)
                    None
                    
        match oo with
            | V_mod  
            | V_divide _ -> (if fp then getg "max-no-fp-divs" else getg "max-no-int-divs")
            | V_plus     -> (if fp then getg "max-no-fp-addsubs" else None(*unlimited *))
            | V_times _  -> (if fp then getg "max-no-fp-muls" else getg "max-no-int-muls")
            | other      -> sf (sprintf "other FU (float or integer ALU) component key %A" other)


    let rezbind_mirrorable msg (key, (sr, thread, director, resume, edge_no, (knnlst: xidx_t list))) = 
        // Find so-far-rezzed count  interpolate to current use_index if below per-thread limit ...
        // There may be a many-to-one mapping for squirrelled mgkey to quota_key.
        // If we have used the quota and the new request is for a variant we have not rezzed (e.g. different precision) then we exceed the allowance.  Planopt can penalize such wantonness.
        let allowance:int option =
            match sr with
                | SR_fu(_, auxix, SR_alu adt, _) ->
                    let (fp, oo) = (adt.precision.signed=FloatingPoint, adt.oo)
                    match get_m_alu_allowance thread fp oo with
                        | None           -> None
                        | Some allowance -> Some allowance.rs_limit
                | _ -> None

        // modulate allowance with planopt

        let externally_instantiatedf =
            match sr with
                | SR_fu(_, auxix, SR_generic_external(kind_vlnv, sq, externally_instantiatedf, data_prec, auxi_, _), miniblock_o) -> externally_instantiatedf
                | _ -> false
        //let last_use_hwm = mirrorbase_hwm (thread, key)
        //When in short supply, select by planopt modulated operand affinity or round robin for level load balancing.
        if true then
            let rezbind_mir knn =
                // planopt selects a permutation
                let item_no = mirrorbase_allocated (thread, key)
                let (item_no, knn_cuck, (str, mgrs, infoo, ats)) =
                    if nonep allowance || item_no < valOf allowance then
                        let sans_iname = (* (idx=0) && *) externally_instantiatedf
                        //dev_println (sprintf "URGENT Rezbind_mirrorable: instantiate FU: key=%s tid=%s  domain=%s  msg=%s  item_no=%i/%A sans_iname=%A" msg (xToStr thread) "director.dir_name" key item_no allowance sans_iname)
                        let (str, mgrs, infoo) = rez_mirrorable_real msg thread director key sr sans_iname knn
                        let ats = []
                        mirrorbase_fu.add (thread, key, item_no) (knn, (str, mgrs, infoo, ats))
                        let fu_handle0 = (str, mgrs, infoo, [])
                        mirrorbase_a.add (thread, key) (item_no+1)
                        (item_no, knn, fu_handle0)
                    else
                        let base_item_no = item_no % (valOf allowance) // TODO use operand wiring affinity
                        let (knn_cuck, fu_handle0) = valOf_or_fail "fu-retrieve L6762" (mirrorbase_fu.lookup (thread, key, base_item_no))
                        //dev_println (sprintf "URGENT Rezbind_mirrorable: recycle FU: key=%s tid=%s  domain=%s  msg=%s  item_no=%i allowance=%A" msg (xToStr thread) "director.dir_name" key item_no allowance)
                        (item_no, knn_cuck, fu_handle0)
                let fu_handle = (str.ikey, str, mgrs, infoo, ats)  // Dont need the first field in tuple here (ikey) since it is in str!
                mutadd m_bindings_mirrorable ((key, knn), fu_handle)
                vprintln 2 (sprintf "rezbind_mirrorable: bind FU: key=%s   resume=%s  edge_no=%i  msg=%s knn=%i (cuckoo knn=%i) bound to item_no=%i iname=%s" key (xToStr resume) edge_no msg knn knn_cuck  item_no str.ikey)
                ()
            app rezbind_mir knnlst
            //mirrorbase_h.add (thread, key) (use_index-1)
        ()

    let wanton_rez_private msg key = // An FU where operations must be directed to a particular instance (e.g. a RAM that contains state).  The mkey in these cases uniquely identifies the instance in question.
        //dev_println (sprintf "   rez_private top %s" key)
        match op_assoc key collated_work with
            | None -> sf  (sprintf "Wanton rez2 missing %s %s" msg key)
            | Some(sr, thread, director, resume, edge_no, knn) ->
                match op_assoc key !m_bindings_private with
                    | Some (iname, str, mgrs, infoo, xref) -> () // iname // unmirrorable route - never create another one
                    | None ->
                //if mirrorable_sr sr then "X_undef" else key        
                        let (iname, str, mgrs, infoo) = rez_head "private" msg [thread] director key sr None false
                        mutadd m_bindings_private (key, (iname, str, mgrs, infoo, []))
                        //vprintln 0 (sprintf "URGENT note_binding key=%s  resume=%s  edge_no=%i" key (xToStr resume) edge_no)
                        //dev_println (sprintf "Wanton_rez_private %s tid=%s  domain=%s  msg=%s  use_index=%i     msg=%A" msg (xToStr thread) "director.dir_name" key use_index msg)
                        () // iname
    app (rezbind_mirrorable "mirrorables") mirrorables 
    app (wanton_rez_private "privates") privates
    let ww = WF 2 "res2_rezbind" ww "Finished"
    (!m_all_resources, !m_bindings_private, !m_bindings_mirrorable)


// Create the static bondout port stuctures.
// We have one instance per logical space. We have one mgr per port in that space.  The ports of a space may be in different clock domains in future.
let create_indexed_bondout_ports ww settings directors (contact_maps:contact_map_t) =
    let ww = WF 2 "create_indexed_bondout_ports" ww "start"
    let vd = settings.vd

    let (last_idx_, pformals, contact_maps, port_netrecs_) =

        let spaceform (space, ports, manager) (port_idx, cc, contact_maps, cd) =

            let m_mgro = ref None
            let portMaps = []
            let fu_meld = gec_bondout_fu_meld ww ports // Details of all but first are ignored!  TODO

            let kind_vlnv = { vendor= "HPRLS"; library= "nolibrary"; kind=[fu_meld.kinds]; version= "1.0" }
            let auxix =
                          { g_default_auxix with
                              raises_data_hazards= true
                              result_latency=      1//(valOf_or_fail "L2158" block_meld).typical_latency // Latency and precision are not the same for all methods in mgr and 
                              rei_interval=        1//(valOf_or_fail "L2158" block_meld).typical_rei
                          }
            let externally_instantiatedf = true

            let dims = (hd ports).dims // All must be same dims
            let domain = director_assoc ww "allocate clock domain for bondout port" directors (hd ports).clock_domain  // Sadly the MGR only has one domain and so all ports for a space are in the same domain with this design .... TODO broaden please.
            let dbits = dims.no_lanes * dims.laneWidth

            let data_precision = { signed=Unsigned; widtho=Some dbits }
            vprintln 2 (sprintf "Creating bondout port space=%s, port_idx=%i, dwidth=%i" space.logicalName port_idx dbits)
            let kinds = hptos kind_vlnv.kind
            let kkey = kinds
            let ikey = space.logicalName // ocp.ls_key
            let mkey = ikey //space.logicalName
            let portmeta = fu_meld.parameters

            let (actual_contact_list, phys_contacts) = // This is the actual actual nets, not their schema.
                let flip = if externally_instantiatedf then Some true else None// Declare the actuals (aka tappets) as locals if internally-instantiated, or flipped formals if external (up and above).
                let actual_gen pi_name nps (cc, cd) =
                    let nets = buildportnet ww vd flip kkey ikey portmeta pi_name "" (Some data_precision) nps []
                    if nullp nets then (cc, cd) else (hd nets :: cc, (hd nets, nps)::cd)
                let mgr_rez (mgr_meld:mgr_meld_abstraction_t) (cc, cd) =
                    List.foldBack (actual_gen mgr_meld.mgr_name) mgr_meld.contact_schema (cc, cd)
                List.foldBack (actual_gen "") fu_meld.contact_schema (List.foldBack mgr_rez fu_meld.mgr_melds ([], []))

#if OLD
            let actual_contact_list = // This is the actual actual nets, not their schema.
                    let mgr_rez (mgr_meld:mgr_meld_abstraction_t) cc =
                        // We pass in ikey as the pi_name - do we ?
                        List.foldBack (buildportnet ww vd flip kkey ikey portmeta mgr_meld.mgr_name "" (Some data_precision)) mgr_meld.contact_schema cc
                    List.foldBack (buildportnet ww vd flip kkey ikey portmeta "" "" (Some data_precision)) fu_meld.contact_schema (List.foldBack mgr_rez fu_meld.mgr_melds [])
#endif
            vprintln 2 (sprintf "bondout rez contacts_all are ^%i " (length actual_contact_list) + sfold (fun (f,a)->f + "/ " + netToStr a) actual_contact_list)

            let contacts =
                    let cpi = { g_null_db_metainfo with kind=sprintf "bondout%i" port_idx; pi_name=mkey; not_to_be_trimmed=true;  } 
                    //let nets = netrec.vlat_nets @ netrec.data_nets
                    let db_netwrap_actual (_, net) = DB_leaf(None, Some net)
                    gec_DB_group(cpi, map db_netwrap_actual actual_contact_list)

            let contact_maps = (contact_maps:contact_map_t).Add(ikey, (contacts, phys_contacts, portmeta, None)) // First time

            //let (block, phys_structure, phys_methods) = gec_block_phys ww fu_meld ikey mkey kind_vlnv data_precision contact_maps // DO NOT CALL ME PLEASE
            let mirrorable = false // The mirrorable property is not applicable for a static resource anyway.
            let sr = SR_fu(mkey, auxix, SR_generic_external(kind_vlnv, None, externally_instantiatedf, Some data_precision, None, mirrorable), Some{ fu_meld=fu_meld; area_o=Some 0.0; })
            let str2 =
              { g_null_str2 with
                  mkey=                 mkey
                  ikey=                 ikey
                  kkey=                 "notset-bondout-kkey" // why? bondouts memories do not have a 'kind'
                //actuals_contacts=     contacts - see the block field within final sr
                  item=                 sr
                  area=                 0.0                  // Bondout ports are part of the static manifest and do not count in dynamic area use.
                  propso=               None
                  clklsts=              [domain]
              } // offchip bondout RAM
                // block should contain the str2 as a field 

            let contact_maps = (contact_maps:contact_map_t).Add(ikey, (contacts, phys_contacts, portmeta, Some str2)) // Second time
            let gec_method_str2 (method_meld:method_meld_abstraction_t) = 
                            {
                                fname=           method_meld.method_name
                                overload_suffix= None
                                precision2=      if method_meld.method_name="write" then g_void_prec else data_precision
                                latency=         auxix.result_latency                            
                                holders=         if method_meld.method_name="write" then [] else [ ref [] ]
                                method_meld=     Some method_meld
                                m_mgro=          m_mgro
                            }

            let gec_mgr mgr_meld =
                let mgr = 
                            { g_null_methodgroup with
                                    dinstance=           str2
                                    mg_uid=              str2.ikey + "_" + mgr_meld.mgr_name
                                    mgr_meldo=           Some mgr_meld
                                    methodgroup_methods = map gec_method_str2 mgr_meld.method_melds
                                    //oc_record=Some       oc_record
                                    max_outstanding= 1 // Only one supported in this release (ok for hfast connected to cache that serves in one logic clock cycle).
                                    auxix=               auxix
                              }
                //dev_println (sprintf "REZ mgr=%A\n\nmgr_meld=%A" mgr mgr_meld)
                m_mgro := Some mgr                        
                mgr
            let mgrs = map gec_mgr fu_meld.mgr_melds
            
            let contact_maps = contact_maps.Add(ikey, (contacts, phys_contacts, portmeta, Some str2)) // Second time we add it! This time with instance present. A bit silly.

            let _ =
                let emit (mgr, cx) =
                    mutadd settings.static_manifest (port_idx+cx, (str2, mgr, portmeta)) // TODO these used flags need resetting on new attempts
                app emit (zipWithIndex mgrs)
            (port_idx+length mgrs, contacts @ cc, contact_maps, () :: cd)
            //List.foldBack pxform_lo (zipWithIndex ports) (port_idx, cc, contact_maps, cd)
        List.foldBack spaceform settings.offchips (0, [], contact_maps, [])

    let indexed_ports = // flatten again!
        let portflattener (space, ports, manager) cc = map (fun port -> (space, port)) ports @ cc
        zipWithIndex (List.foldBack portflattener (settings.offchips) [])


        
    (indexed_ports, contact_maps, pformals) // end of create_indexed_bondout_ports



let compute_area_cost ww0 cc (pip_, director, dinstance) =
    let area = dinstance.area
    if area < 0.0 then hpr_yikes(sprintf "No silicon area associated with mkey=%s kkey=%s" dinstance.mkey dinstance.kkey)
#if OLD
        match dinstance.item with
            | SR_fu(mkey, auxi, srv, _) -> areacost1 srv
            | SR_scalar _ -> 0.0 // random logic cost ignored at the moment
            | _ -> 0.0    
#endif
    area + cc

let bsearch ww0 settings threads = 
    let ww1 = WF 1 "Restructure2: bsearch" ww0 "Start"
    //
    // let _ = res2_poly_project_thread ww settings threads

    let explore_operatex ww0  bmsg oraclef = 

        let bounders =
            {
                   xition_summaries= ref [] // Not really a setting per se!
                   global_mgrs=                new ListStore<string, str_methodgroup_t>("global_methodgroups")
                   global_resources=           new ListStore<string, str_instance_t>("global_resources")
                   global_resource_thread_use= new ListStore<string, string>("global_resource_thread_use")                       
            }

        let perform_major_phase ww no bindpasso =
            let title = if no=0 then "Mapping Major Phase" elif no=1 then "Schedulling Major Phase" else sf "Bad Major Phase"
            let ww = WF 1 "Restructure2: processor" ww0 title
            let r33 = map (res2_arcate (WN "res3_arcate_thread" ww) settings oraclef bindpasso bounders) threads
            let ww = WF 1 "Restructure2: processor" ww0 (title + " finished")
            r33

        let r33_0 = perform_major_phase ww0 0 None
        let ww0 = WF 2 "Restructure2: processor" ww0 "mapping pass completed"


        bindmapreport_r33 ww0 "FU Thread and Domain Report - Binding Phase Input" r33_0

        let ww1 = WF 1 "Restructure2: processor" ww0 "Binding Major Phase Start"
        let (all_resources, binding_private, binding_mirrorable) = res2_rezbind ww1 settings bounders r33_0 
        let ww0 = WF 1 "Restructure2: processor" ww0 "Binding Major Phase Finished"
        bindmapreport_r33 ww0 "FU Thread and Domain Report - Binding Phase Output" r33_0
        let r33 = perform_major_phase ww0 1 (Some(binding_private, binding_mirrorable))
        let area = List.fold (compute_area_cost ww0) 0.0 all_resources 
        vprintln 2 (sprintf "Area used %f" area)
        (r33, all_resources, bounders, 1111.1)


    let bsearch_merit_fn ww (r33, all_resources, bounders, merit) = merit
    let pprams_sys = // Planop search args
        {
            attempt_limit=     settings.schedule_attempts
            acceptable_merit=  1e6
        }

    let pprams_u =
        {
            operatex=          explore_operatex
            merit_fn=          bsearch_merit_fn
        }
    let seed = 1

    let ((r33, all_resources, bounders, merit), _, _)  = planopt_run ww1 "bsearch-main" pprams_sys pprams_u seed 2

    let ww1 = WF 1 "Restructure2: bsearch" ww0 "Start"
    (r33, all_resources, bounders)

//
// This vm2 is the main use and entry point for restructure - (the vm1 code was for HSIMPLE transactors before bevelab and has now been deleted?).
//
let opath_restructure_vm2 ww0 op_args vm0 =
    let toolname = "restructure2"
    let _ = WF 3 "Restructure2: processor" ww0 "Start"
    let c3 = op_args.c3
    let vd = max !g_global_vd (control_get_i op_args.c3 "res2-loglevel" 1)
    vprintln 1 (sprintf "vd logging level for res2 is vd=%i" vd)

    let (settings, report_items) = baseline_settings ww0 c3 vd toolname

    gen_res_report settings report_items

    let (vmf, threads) = res2_trawl ww0 toolname vm0

    let directors = list_once(map (snd>>f1o5) threads) // Should do this on duid basis?
    vprintln 2 (sprintf "There are %i directors and/or clock domains in this design" (length directors))
    let (_, contact_maps, pformals) =
        if nullp directors then ([], Map.empty, [])
        else create_indexed_bondout_ports ww0 settings directors Map.empty
    
    let ww0 = WF 2 "Restructure2: processor" ww0 "collated. stage=3"

    let prepared_threads =
        let preparer thread =
            let (nom_thread__, (director, controllerf, vm2_ids, arcs_revd, hls_sigs)) = thread
            let prepared_thread = res2_arcate_prepare ww0 (settings:restruct_manager2_t) thread
            let (director, vm2_ids, hls_signature, thread, controllerf, edges_by_resume, (fwd_resume_database, rev_resume_database, earlier_arc_database, later_arc_database), isFromStartState, all_work_this_thread, arc_freqs) = prepared_thread
            prepared_thread
        map preparer threads

    let (r33, all_resources, bounders) = bsearch ww0 settings prepared_threads

    let ww0 = WF 2 "Restructure2: processor" ww0 "generation pass completed"

    // Different directors here means different domains - not truely different clock domains but we regard similalry different.  Singly add can be done on duid basis if equality fails.
    let domains = List.fold (fun cc (tid, (clko, _, _, rtl, _)) -> singly_add clko cc) [] threads

    let all_resources:(pipate_controller_t * directorate_t * str_instance_t) list = all_resources

    let ww1 = WF 2 "Restructure2: processor" ww0 (sprintf "Start render_resource_instances: %i FUs" (length all_resources))
    let (nets_f, sons_f, tuple_rtl, contact_maps) = List.fold (res2_render_resource_instances ww1 settings bounders) ([], [], [], contact_maps) all_resources
    let ww0 = WF 2 "Restructure2: processor" ww0 (sprintf "render_resource_instances completed for: %i FUs" (length all_resources))
    
    let ww1 = WF 2 "Restructure2: processor" ww0 "Start res2_build_thread"
    let m_assist = ref []// Global scoreboarding RTL: (hr_scoreflop_t * stutter_t *  hbexp_t * hexp_t) list ref
    let (pc_export_nets, thread_rtl, driven_tapets) = List.unzip3(map (res2_build_thread ww1 settings m_assist contact_maps) r33)
    let ww0 = WF 2 "Restructure2: processor" ww0 (sprintf "res2 build thread complete for all threads")

    let unused_tappets_tieoffs = gec_unused_tappets_tieoffs ww1 settings contact_maps driven_tapets
    let morenets =
        let cpi = { g_null_db_metainfo with kind= "res2-morenets";  }
        gec_DB_group(cpi, map db_netwrap_null (!settings.m_morenets))

    let pc_export_nets =
        let cpi = { g_null_db_metainfo with kind= "res2-directornets";  }
        gec_DB_group(cpi, map db_netwrap_null (list_flatten pc_export_nets))

    let nets12 = pc_export_nets @ nets_f @ morenets 


    let ans =
        let rec insert_rtl_in_sons rtl = function
            | [] -> ([], rtl)
            | (ii, Some(HPR_VM2(minfo, decls, sons, ef, assertions)))::tt ->
                let vm2_id = minfo.name
                let (local_rtl, rtl) =
                    let rec find_local = function
                        | [] -> ([], [])
                        | (vm2_ids, lrtl)::tt when memberp vm2_id vm2_ids -> ([lrtl], tt)
                        | other::tt ->
                            let (ans, rtl) = find_local tt
                            (ans, other::rtl)
                    find_local rtl
                let (sons, rtl) = insert_rtl_in_sons rtl sons
                let nv_h = (ii, Some(HPR_VM2(minfo, decls, sons, local_rtl @ ef, assertions)))
                let (nv_t, rtl) =  insert_rtl_in_sons rtl tt
                (nv_h::nv_t, rtl)
                
        match vmf with
        | [(ii, Some(HPR_VM2(minfo, decls, sons, ef, assertions)))] ->
            let decls' = decls_trim (*delete=*)true (!settings.m_old_nets_for_removal) (decls@nets12)
            let minfo_out = minfo

            let (sons, remaining_thread_rtl) =
                if true then insert_rtl_in_sons thread_rtl sons
                else (sons, thread_rtl)
            let ii =
                { ii with
                    generated_by= toolname
              //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
                }

            let rtl12 = map snd remaining_thread_rtl @ tuple_rtl @ unused_tappets_tieoffs
            (ii, Some(HPR_VM2(minfo_out, decls' @ pformals, sons @ sons_f, ef @ rtl12, assertions)))

        | _ -> sf "not one top-level VM2 machine"

    let _ =
        // Custom interconnect between separate compilations also shows up here in the port list ?
        if nullp settings.offchips then vprintln 2 (sprintf "Compilation uses no offchip/bondout ports")
        else bondout_port_report ww0 false toolname "Res2 Final" settings.tableReports settings.offchips 
        // Two styles of report here!
        app (fun (space_, ports_, (mm:bondout_memory_map_manager_t)) -> mm.create_bondout_report settings.tableReports) settings.offchips


    let _ = 
        let fn = logdir_path (filename_sanitize g_okl (ii_fold [fst ans]) + ".restructureReport." + toolname + ".txt")
        let fd = yout_open_out fn
        yout fd (report_banner_toStr "REPORT :")
        app (fun x->(app (yout fd) x)) (rev !settings.tableReports)
        yout_close fd            
        vprintln 2 ("Wrote report file " + fn)
    let anal = settings.anal
    let _ =
        if anal.dot_plot_combined then
            let ww = WF 3 "dot writeouts" ww0 (sprintf "combined graph with %i threads" (length !anal.dot_plots))
            writeout_dot_plot ww "combined" !anal.dot_plots
            let ww = WF 3 "dot writeouts" ww (sprintf "combined graph written")
            ()

    let _ = // Concise expression number report - res2
        report_enumbers (YOVD 3) settings.tableReports
        let fd = yout_open_report "res2 enumbers" 
        report_enumbers fd settings.tableReports
        yout_close fd


    if true then ans else vm0 // end of restructure_vm2




let opath_restructure_vm toolname ww op_args vms =
    let dis = control_get_s toolname op_args.c3 "restructure" None // both 1 and 2
    if dis = "disable" then
        vprintln 2 ("Restructure: " + toolname + " stag is disabled.")
        vms
    else
        let dis = control_get_s toolname op_args.c3 toolname None
        if dis = "disable" then
            vprintln 2 ("Restructure " + toolname + " stage is disabled.")
            vms
        else
            //let mode = control_get_s toolname op_args.c3 "restructure-mode" None
            if false then
                let ax = 3
                set_console_verbose ax
                dev_println ("Console verbose enable for res2 to " + i2s ax)
            if toolname = "restructure2" && dis = "enable" then
                map (opath_restructure_vm2 ww op_args) vms
            elif toolname = "restructure1" then  // old one - just used for hsimple and other future behavioural macros
                vprintln 1 ("Old recipe invokes restructure1 - no longer used")
                //opath_restructure_vm1 toolname ww op_args vm
                vms
            else sf (sprintf "restructure: bad toolname %s: " toolname)
       



// This file once registered two stages/tools.  Toolname is always either "restructure1" or "restructure2"      
// The recipe's stagename may be different entirely, especially if one of the two tools is called more than once.
let install_restructure () =


    let res1_arg_patterns =
        let toolname = "restructure1"
        [
          Arg_enum_defaulting("restructure1", ["enable"; "disable"; ], "enable", "Enable control for this operation");                        
          Arg_enum_defaulting("restructure", ["enable"; "disable"; ], "enable", "Enable control for this operation");            
        ]

        
    let dot_arg_patterns = // TODO move out to their own stage
        [          
          Arg_enum_defaulting("dotplot-detailed", ["enable"; "disable"], "disable", "Show structure-resource use in dot plots.");
          Arg_enum_defaulting("dotplot-plot", ["separately"; "combined"; "disable"], "disable", "Write dot plots for each thread to separate files or one larger file.");
          Arg_enum_defaulting("dotplot-reprint", ["enable"; "disable"], "disable", "Refactor and reprint input machine.");
        ]

    let res2_arg_patterns =
        let toolname = "restructure2"

        let dr =
            let drf (key, description, defval) = Arg_int_defaulting(key, defval, description)
            map drf g_quota_key_manifest
        [

          Arg_enum_defaulting(toolname, ["enable"; "disable"; ], "enable", "Enable control for this operation")      //
          Arg_enum_defaulting("restructure", ["enable"; "disable"; ], "enable", "Enable control for this operation") // Two names?

          Arg_defaulting("res2-external-afus", "", "Specifiy a list of AFU (application-specific functional unit) kinds that should be instantiated externally. This can also be specified on an instance attribute.") 

          Arg_int_defaulting("res2-loglevel", 1, "Verbosity level for restructure2 (higher is more verbose)")
          Arg_enum_defaulting("res2-killpipelining", ["oldstyle"; "normal"; "pad1"; "pad4"; "pad10"; "pad20"], "oldstyle", "Pipeline operations over major state boundaries. Only disable this for debug purposes. It defaults to oldstyle on a temporary basis at the moment (Arpil2016)")

          Arg_enum_defaulting("res2-share-array-reads", ["enable"; "disable"], "enable", "This was turned off at one point while a bug was chased down.") 
          Arg_enum_defaulting("res2-regen-sequencer", ["enable"; "disable"], "enable", "After restructure, refactorise to reassemble FSM") // This was off for a few years but enabled is probably preferable and is needed for fsm_link to handshakes to work.
          Arg_enum_defaulting("always-include-loadstore-ports", ["enable"; "disable"; ], "enable", "Included all loadstore I/O signals even when unused")

          //Arg_enum_defaulting("res2-keep-oldpc", ["true"; "false"], "true", "Retain and update input-side program counters to as a debug aid - but they are not referenced");

          Arg_enum_defaulting("res2-extend-schedules-to-keep-pli-order", ["full"; "all-but-console"; "disable"], "all-but-console", "Ensure console PLI $display and/or other operations are maintained in order even if static schedule extension is required");          
          //Arg_int_defaulting("res2-bram-capacity", 8, "The capacity of the on-FPGA block RAM in bits");

          Arg_int_defaulting("res2-regfile-threshold", 8, "Threshold in terms of number of locations below which to not instantiate a structural SRAM");

          Arg_int_defaulting("res2-combrom-threshold", 64, "Threshold in terms of number of locations at which to instantiate synchronous, latency=1 ROM instead of a combinational (random logic or distribured-RAM), structural ROM - read only memory");          
          Arg_int_defaulting("res2-combram-threshold", 32, "Threshold in terms of number of locations at which to instantiate synchronous, latency=1, structural SRAM");
          Arg_int_defaulting("res2-combrom-threshold", 64, "Threshold in terms of number of locations at which to instantiate synchronous, latency=1, structural ROM");
          Arg_int_defaulting("res2-offchip-threshold", 1000*1000, "Threshold in terms of number of locations at which to map to an external bondout memory bank (e.g. DRAM)");

          Arg_int_defaulting("res2-schedule-attempts", 5, "Maximal number of schedules to construct before selecting best design.");          

          Arg_int_defaulting("fp-fl-sp-mul-flash", 5, "Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.");

          Arg_int_defaulting("fp-fl-sp-mul", 5, "Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.");
          Arg_int_defaulting("fp-fl-sp-add", 4, "Fixed-latency ALU floating-point, single-precision floating-point latency value for add/sub.");
          Arg_int_defaulting("fp-fl-sp-div", 15, "Fixed-latency ALU floating-point, single-precision floating-point floating-point latency value for divide.");

          // 3 is likely unrealistic! Our current S/P divider in hpr_ipblocks is 15 (although its not an SRT one which would be a lot better.)
          Arg_int_defaulting("fp-fl-dp-mul", 3, "Fixed-latency ALU floating-point, double-precision floating-point latency value for multiply.");

          Arg_int_defaulting("fp-fl-dp-add", 4, "Fixed-latency ALU floating-point, double-precision floating-point latency value for add/sub.");

          // 5 is likely unrealistic too!
          Arg_int_defaulting("fp-fl-dp-div", 5, "Fixed-latency ALU floating-point, double-precision floating-point  latency value for divide.");

          // Use latency of 3 for now for 32 bits and 5 for 64 meaning we need a gradient of 2/32 latency of 1 for 24, meaning ratio is 
          // Note we assume 32-bit output form 32-bit inputs following typical (nonsense) HLL integer promotion rules.
          // We do not have these for add/sub since they have a hardwired infinte flash limit
          // And we do not have it for divide because we currently use V/L divides.
          Arg_int_defaulting("int-flr-mul-perbit",       63, "Fixed-latency integer ALU latency-per-product-bit ratio for multiply (x1000). 0.0625=1/16th clock per bit.");
          Arg_int_defaulting("int-flr-mul-baseline",     1000, "Fixed-latency integer ALU latency intercept (baseline) for fictional 1-bit FU (x1000).");          
          Arg_int_defaulting("int-flr-mul-flash",       10, "Integer ALU (so-called flash) limit below which combinational logic will be left to be synthesised by back-end tools.");          


          //Divide is always variable latency or else converted to the equivalent multiply by reciprocal.
          //Arg_int_defaulting("int-flr-div", 500, "Fixed-latency integer ALU integer latency scaling per bit value for divide (x1000).");


          Arg_int_defaulting("max-ram-data-packing", 16, "Maximum number of user words to pack into one RAM/loadstore word line.");
          Arg_int_defaulting("max-no-rom-mirrors", 8, "Maximum number of times to mirror a ROM.");
          
          Arg_int_defaulting("int_fl_limit_mul", 20, "Structural ALU summed input bit width threshold for integer multiply.");

          Arg_enum_defaulting("resstructure-mode", ["0"; "1"; "2"], "0", "Mode 1=add RPC nets(old). Mode 2=instantiate and schedule structures.");
        ] @ dr


    let _ =
        install_operator ("restructure1", "Add RPC Terminals etc", opath_restructure_vm "restructure1", [], [], dot_arg_patterns @ res1_arg_patterns @ g_bondout_arg_patterns)

        install_operator ("restructure2", "Reorganise operations on structural resources to avoid contention hazards", opath_restructure_vm "restructure2", [], [], dot_arg_patterns @ res2_arg_patterns @ g_bondout_arg_patterns)

    // This tool/stage, called restructure, has two fairly disjoint functions, the first of which is shortly to be discarded with functionality pushed into the basic VM type for directibility (as Nik puts its).
    ()
    

(* DELETE THIS
Old issues pre Aug 2017:
   We have only a simple greedy algorithm in the released version. Now being fixed using planopt.

   We currently use an ALU structural resource at most once in a major state and do not exploit its pipeline. - That's the naive read before write approach!. Fixed now.

   DRAM bank load/store - other resource control limits - does not completely tie up with documentation.
   
   The blocking assigns are a pain and do they work for gatelevel output etc? - TODO does this still hold?
   Issue them as separate assigns or do the whole as fully-supported with subsequent nsf assigns?

   Behavioural style code with comb RAM lookup ? 

   Old and new pcs - the old is kept for reference in the output but is commonly not used.     Later cone trimming will likely drop it.
    - But it is used if the exec phase is not the last microstate and there are successors conditional on pre-exec support.
    - If execf has happened at end of major state, the old thread pc has been updated and tells us where to go, otherwise we do a more normal (direct) transition since support of g has not been updated.

   Only thing post exec is assumed to be holding register writebacks.



*)



(* eof *)
