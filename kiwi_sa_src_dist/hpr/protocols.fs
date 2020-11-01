//
//
// CBG Orangepath. HPR Logic Synthesis and Formal Codesign System.
//
// protocols.fs - Inter-IP block protocol support.
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

module protocols

open System.Collections.Generic
open System.Numerics
open Microsoft.FSharp.Collections

open yout
open hprls_hdr
open meox
open abstract_hdr
open moscow
open opath_hdr
open hprxml
open linprog
open linepoint_hdr


let g_ip_xact_file_suffix = ".xml" // Boring sufix

    
let vlnvToStr (vlnv:ip_xact_vlnv_t) = hptos vlnv.kind + "/" + vlnv.version // short form

let vlnvToStr_full (vlnv:ip_xact_vlnv_t) =  vlnv.vendor + "/" + vlnv.library + "/" + hptos vlnv.kind + "/" + vlnv.version // long form

type xact_markup_t =
    | XACT_prams of (string * string) list list


type loaded_fu_summary_t = (bool * bool * ip_xact_vlnv_t * string * string list)  // For functions that are to be replaced with structural components (functional units): is_loaded, is_cv_canned, kind, pi_name and component instance name(s) (pre load balancing)


// Certain functional units are canned (hardcoded inside HPR L/S) and others are loaded dynamically from IP_XACT files.
// The global loaded and canned database: Indexed by function name (string) or by the squirrel string if overloaded.
let g_isloaded_db = new OptionStore<string, loaded_fu_summary_t>("g_isloaded_db")


// Add this prefix to machine-generated IP_XACT files (to avoid overwriting library or user files of the same name).
// Users should manually delete this prefix from file names and the file lists in parent IP_XACT files when committing to libraries for future use.
let g_autometa_prefix = "AUTOMETA_"
    
// A load/store port may be simplex or half_duplex.  
// When using AXI for off-chip memory connection, the load and store channels are independent, so using simplex ports
// that are either load only or store only is preferable.  The typical structure is for a number of in-order simplex ports
// to be multiplexed onto a single out-of-order AXI bus.  


type port_duplex_t = // TODO this is the data aspect, not the hs protocol - please rework
    | PD_halfduplex
    | PD_storeport
    | PD_loadport

    
type ipblk_protocol_t = // This is not a good datatype - please refactor!
    | IPB_AXI3 of bool * port_duplex_t // if bool holds then streaming (no address port)
    | IPB_FPFL     // Fully-pipelined, fixed-latency (no handshakes needed)
    | IPB_HFAST of hfast_prams_t * port_duplex_t // duplexity is a dataplane aspect and should not be in here
    | IPB_HSIMPLE  // Fully deprecated
    | IPB_BVCI     // no longer needed...

type bondout_address_space_t =
   {        
       logicalName:     string
       m_userNames:     string list ref
       wordsAvailable:  int64       
       wordWidth:       int
   }


type bondout_port_dims_t =
    {
        addrSize:   int // Address bus width
        no_lanes:   int
        laneWidth:  int
        bytesAvailable: int64  // Just a replication of address_space_space information.
    }    

//
// Whereis generic inter-IP block protocol support? In restructure perhaps.
//
type bondout_port_t =
    {
        ls_key:            string // aka port instance name
        protocol:          ipblk_protocol_t
        dims:              bondout_port_dims_t
        bondout_space:     bondout_address_space_t
        clock_domain:      string
    }


type offchip_static_region_t = // managed offchip memory region : vague name TODO rename - the ikey is restructure specific
    {
        memory:    hexp_t // ikey
        baser:     BigInteger // Memory map layout. - Base address in units of one lane (commonly one byte).
        endpt:     BigInteger
        n_items:   BigInteger        
        space:     bondout_address_space_t
        port:      bondout_port_t  // The binding to a given port is not really needed here is it?  Several posts to the same logical space could balance the load.
        layout:    layout_t
    }


let ipbToStr(ipb:ipblk_protocol_t) =
    match ipb with
        | IPB_FPFL  -> "IPB_FPFL"
        | IPB_HFAST(protocol_prams, duplexity) ->
            let a1 = if protocol_prams.reqrdy_present then "_RR1" else ""
            let a2 = if protocol_prams.ackrdy_present then "_AR1" else ""            
            sprintf "HFAST%i(%A)%s%s" protocol_prams.max_outstanding duplexity a1 a2
        | ipb -> sprintf "OTHER? %A" ipb

// Attribute for REN net present on ROM or RAM
let g_romram_with_ren = "romram-with-ren"


// Parse handshake subschema.
// TODO - max outstanding and always ready cannot be directly specified at this level. Easy to add.
// maxoutstanding is in the field before this rax subschema: eg HFAST2-RR1-AR1
let rax_pcol_subschema_core msg dsimple subschema = 
    let default_protocol_prams = { max_outstanding=1; req_present=true; ack_present=true; reqrdy_present=not dsimple; ackrdy_present=not dsimple; posted_writes_only=false  } // ackrdy_present and so on are programmable via recipe bondout schema string and, in Kiwi, attribute strings.

    let default_clk_domain = xToStr g_clknet 
    let rez_clk_net clkname =
        let _ = bnetgen g_ip_xact_is_Clock_tag clkname
        clkname // This is string-form clock name is later looked up by associating in the directors in scope.
    let items = split_string_at [ '-' ] subschema
    let rec bas (protocol_prams, clk_domain) = function
            | [] -> (protocol_prams, clk_domain)
            | "" :: tt -> bas (protocol_prams, clk_domain) tt
            | "RR1" :: tt -> bas ({protocol_prams with reqrdy_present=true },  clk_domain) tt
            | "RR0" :: tt -> bas ({protocol_prams with reqrdy_present=false }, clk_domain) tt
            | "AR1" :: tt -> bas ({protocol_prams with ackrdy_present=true },  clk_domain) tt
            | "AR0" :: tt -> bas ({protocol_prams with ackrdy_present=false }, clk_domain) tt
            | ss :: tt when strlen ss > 4 && ss.[0..4] = "clk=" -> 
                let clk_domain = rez_clk_net ss.[5..]
                bas (protocol_prams, clk_domain) tt
            | _ -> sf (msg + sprintf ":subschema: unrecognised protocol detail token: parse failed for subschema '%s'  exploded to '%s'" subschema (sfold id items))
    let (protocol_prams, clk_domain) = bas (default_protocol_prams, default_clk_domain) items
    (protocol_prams, clk_domain)


// Duplexity should not be inside IPB really?
let rax_pcol_subschema msg subschema duplexity = 
    let (protocol_prams, clk_domain) = rax_pcol_subschema_core msg false subschema
    (IPB_HFAST(protocol_prams, duplexity), clk_domain) 


let parse_protocol_hangmode ww msg directorate_ats hang_mode_str =
    match hang_mode_str with
        | "hang"                       -> Some DHM_hang 
        | "finish"                     -> Some DHM_finish
        | "auto-restart"               -> Some DHM_auto_restart
        | "pipelined-no-handshake"     -> Some DHM_pipelined_no_handshake
        | ss ->
            match has_str_prefix "pipelined-stream-handshake-" ss with
                | Some subschema_string ->
                    let (protocol_prams, clk_domain_) = rax_pcol_subschema_core msg false subschema_string
                    Some (DHM_pipelined_stream_handshake(subschema_string, protocol_prams))
                | None ->
                    vprintln 1 (sprintf "parse_protocol_hangmode: unsupported hangmode %s.  Expected one of: hang, finish, auto-restart, pipelined-no-handshake or pipelined-stream-handshake-SUBSCHEMA" hang_mode_str)
                    None


//
//   just part of the space manager? not called elsewhere ?
let layout_pack_to_lanes_or_words msg name no_lanes lane_width n_dwidth =
    let rec roundup lanes =
        if lanes * lane_width >= n_dwidth then lanes else roundup (lanes * 2)
    let ulanes = roundup 1
    let r_dwidth = lane_width * ulanes // Round the natural width up to binding number of lanes - power of 2 !
    let dbusWidth = no_lanes * lane_width
    let _ = vprintln 3 (sprintf "layout: round_to_words/lanes: %s: Port h/w parameters no_lanes=%i lane_width=%i  dbusWidth=%i.  User wanted_width=%i. Rounded to power of 2 lanes width=%i" name  no_lanes lane_width  dbusWidth n_dwidth r_dwidth)
    // round_to_words no_lanes lane_width  wanted_width 

    if r_dwidth = dbusWidth then // exact fit - packed is moot.
        let _ = vprintln 3 (msg + sprintf ": exact fit to external data bus width")
        { packed=false; n_dwidth=n_dwidth; r_dwidth=r_dwidth; words_item_ratio=1 }

    elif r_dwidth < dbusWidth then // can perhaps pack if small items
        let rec pfit lanes =
                if lanes * lane_width >= r_dwidth then lanes
                else pfit (lanes * 2)
        let lanes = pfit 1
        let _ = vprintln 3 (msg + sprintf ": User's word requires %i lanes.  Can pack %i items per word."  lanes (no_lanes/lanes))
        { packed=true; n_dwidth=n_dwidth; r_dwidth=r_dwidth; words_item_ratio=no_lanes/lanes }
    else
        let need = (dbusWidth - 1 + r_dwidth)/dbusWidth
        let words = bound_log2 (BigInteger need)
        let _ = vprintln 3 (msg + sprintf ": User's item requires %i words.  Round up to %i words per item."  need words)
        { packed=false; n_dwidth=n_dwidth; r_dwidth=r_dwidth; words_item_ratio=words }





// ==============================================================

type axi_rd_addr_channel_t = 
    {
        port:     bondout_port_t
        araddr:   hexp_t
        arprot:   hexp_t
        arready:  hexp_t
        arvalid:  hexp_t
    }

type axi_rd_data_channel_t = 
    {
        port:     bondout_port_t
        rdata:    hexp_t
        rready:   hexp_t
        rresp:    hexp_t
        rvalid:   hexp_t
    }

type axi_wr_addr_channel_t = 
    {
        port:    bondout_port_t
        awaddr:  hexp_t
        awprot:  hexp_t
        awready: hexp_t
        awvalid: hexp_t
    }

type axi_wr_resp_channel_t = 
    {
        port:    bondout_port_t
        bready:  hexp_t
        bresp:   hexp_t
        bvalid:  hexp_t
    }


type axi_wr_data_channel_t = 
    {
        port:    bondout_port_t
        wdata:   hexp_t
        wready:  hexp_t
        wstrb:   hexp_t option
        wvalid:  hexp_t
    }

type axi_rd_channel_t =
    {
        raddr:   axi_rd_addr_channel_t
        resp:    axi_rd_data_channel_t
    }

type axi_wr_channel_t =
    {
        waddr:  axi_wr_addr_channel_t            
        resp:   axi_wr_resp_channel_t
        wdata:  axi_wr_data_channel_t
    }
    
type aix_netrec_t =
    {
        ikey:            string
        axi_rd_channel:  axi_rd_channel_t option
        axi_wr_channel:  axi_wr_channel_t option
    }




let lname_eqeq a b =
    a=b                      || // Allow external blocks to have req and ack and so on in either case
    b="REQ" && a = "req"     ||
    a="REQ" && b = "req"     ||
    b="ACK" && a="ack"       ||
    a="ACK" && b="ack"       ||    
    b="RETURN" && a="return" ||
    a="RETURN" && b="return"      


let meta_fsem_ats msg fsems =
    let tf b = if b then "true" else "false"
    [
        ("NONREF",        tf fsems.fs_nonref)
        ("EIS",           tf fsems.fs_eis)
        ("YIELDING",      tf fsems.fs_yielding)
        ("MIRRORABLE",    tf fsems.fs_mirrorable)
        ("INHOLD",        tf fsems.fs_inhold)
        ("OUTHOLD",       tf fsems.fs_outhold)
        ("ASYNCH",       tf fsems.fs_asynch)
        //("",       tf fsems.fs_)
    ]


// The attributes Kiwi.Remote Kiwi.Stub and Kiwi.Server have composite arg strings that are unpacked here.
let fsems_folder msg attribute nv ov =
    let tolower (ss:string) = String.map System.Char.ToLower ss
    let tf =
        match tolower nv with
            | "true"  -> true
            | "false" -> false
            | other -> cleanexit(msg + sprintf ": bad attibute setting '%s' for %s: expected 'true' or 'false'" other attribute)

    match toupper attribute with
        | "NONREF"     ->    { ov with fs_nonref=tf }
        | "EIS"        ->    { ov with fs_eis=tf }
        | "YIELDING"   ->    { ov with fs_yielding=tf }
        | "MIRRORABLE" ->    { ov with fs_mirrorable=tf }
        | "INHOLD"     ->    { ov with fs_inhold=tf }
        | "OUTHOLD"    ->    { ov with fs_outhold=tf }
        | "ASYNCH"     ->    { ov with fs_asynch=tf }
        | other        ->
            hpr_yikes(msg + sprintf ": cannot parse method semantic attibute %s so ignored." other)
            ov
// MIRRORABLE is really a block attribute: all invoked methods must be flagged MIRRORABLE for the block property to be inferred.
let g_foldable_fsem_attribute_names = [ "NONREF";  "EIS"; "YIELDING"; "MIRRORABLE";  "INHOLD"; "OUTHOLD"; "ASYNCH" ]


let port_eval_int msg msg1 portmeta (arg:string) =
    let w0 = 
        if System.Char.IsLetter arg.[0] then
                    match op_assoc arg portmeta with
                        | None   ->
                            let _ = hpr_yikes(msg + sprintf ": port_eval_int: +++ Missing metapram '%s' in bus port build for %s" arg msg1)
                            32L // For now default to this
                        | Some v -> atoi64 v
                else atoi64 arg
    w0






let g_hfast_handshake_structure =  // This is just a handshake protocol, so there is no address or data field here. Data nets run alongside as needed.
 [ // The order of listed the nets here may be relied on below.
  { g_null_netport_pattern_schema with lname="REQ";      width="1";           sdir="i";    idleval="0"; ats=[(g_control_net_marked, "true")] };
  { g_null_netport_pattern_schema with lname="ACK";      width="1";           sdir="o";    idleval="0"; ats=[(g_control_net_marked, "true")] };
  { g_null_netport_pattern_schema with lname="REQRDY";   width="1";           sdir="o";    idleval="0"; ats=[(g_control_net_marked, "true")] };
  { g_null_netport_pattern_schema with lname="ACKRDY";   width="1";           sdir="i";    idleval="0"; ats=[(g_control_net_marked, "true")] };
 ]

// This is not called by restructure... ? phys_methods needs one!
// cf gec_hs_net_schema_and_melsh_protocol
// compare with ackrez and other 'mine' code.

type vlat_t = // A generic handshake net holder. This is at the schema level.
    {
        protocol:       ipblk_protocol_t
        // (actual pi_name, data prec, schema, mapped_lname) - the schema contains the formal pi_name
        // The mapped pi_name is for IP_XACT port mapping which we are not really using
        vlat_req:       string * precision_t * netport_pattern_schema_t * string
        vlat_ack:       string * precision_t * netport_pattern_schema_t * string
        vlat_reqrdy_o: (string * precision_t * netport_pattern_schema_t * string) option
        vlat_ackrdy_o: (string * precision_t * netport_pattern_schema_t * string) option
    }

let gec_hs_net_schema_vlat_and_melsh ww protocol = // cf grock_protocol_vlat ... some replication of functionality
    let (hs_net_schema, hs_t0_control, hs_t1_control) =
        match protocol with
            | IPB_FPFL -> ([], [], [])
            | IPB_HFAST(protocol_prams, duplexity_) ->
                let xpred arg =
                    if   lname_eqeq arg.lname  "REQ" then true
                    elif lname_eqeq arg.lname  "ACK" then true                
                    elif lname_eqeq arg.lname  "REQRDY" && protocol_prams.reqrdy_present then true
                    elif lname_eqeq arg.lname  "ACKRDY" && protocol_prams.ackrdy_present then true
                    else false
                let t0 = [ ("REQ", "1" ) ]
                let t1 = if protocol_prams.ackrdy_present then [ ("ACKRDY", "1") ] else []
                (List.filter xpred g_hfast_handshake_structure, t0, t1)
            | _ -> sf "bad protocol L313"


    let vlat =
        if nullp hs_net_schema then None
        else
            let simple_schema_mine defo search_item hs_net_schema =
                let rec scanner = function
                    | [] -> defo
                    | nps::tt when lname_eqeq nps.lname search_item -> Some ("", g_bool_prec, nps, "mapped+")
                    | _::tt -> scanner tt
                scanner hs_net_schema
            let req = valOf_or_fail "L353" (simple_schema_mine None "req" hs_net_schema)
            let ack = valOf_or_fail "L354" (simple_schema_mine None "ack" hs_net_schema)
            let reqrdy_o = simple_schema_mine None "reqrdy" hs_net_schema
            let ackrdy_o = simple_schema_mine None "ackrdy" hs_net_schema
            Some { protocol=protocol; vlat_req=req; vlat_ack=ack; vlat_reqrdy_o=reqrdy_o; vlat_ackrdy_o=ackrdy_o }

    (hs_net_schema, vlat, hs_t0_control, hs_t1_control)



// This creates a vlat by scanning hs_nets but, unless portMaps are used and so on, this does nothing significantly
// different gec_hs_vlat_net_schema_and_melsh protocol which does not need an input list.    
//
let grock_protocol_vlat ww msg pi_name hs_protocol portMaps portmeta hs_nets =
    let report_possibilitites() =
        vprintln 0 ("formal name possibilities include: " + sfold (fun (x:netport_pattern_schema_t) -> x.lname) hs_nets)
    match hs_protocol with
        | IPB_FPFL -> None
        | IPB_HFAST(protocol_prams, duplexity_) ->
            if nullp hs_nets then sf (msg + sprintf "Expected there to be some hs_nets in an interface that has protocol %s" (ipbToStr hs_protocol))
            else
                //dev_println (sprintf "need to gricket for pi_name=%s from hs_nets=%A. " pi_name hs_nets)

                let rec schema_mine defo searchitem = function
                    | [] ->
                        let msg = (sprintf "Bus/interface %s has missing handshake net %s for protocol %s" (if pi_name="" then "(no pi_name)" else sprintf "called '%s'" pi_name) (searchitem) (ipbToStr hs_protocol))
                        if nonep defo then
                            report_possibilitites()
                            cleanexit msg
                        else
                            vprintln 1 msg
                            None
                    | h::_ when lname_eqeq h.lname searchitem ->
                        let def = if pi_name="" then h.lname elif isDigit pi_name.[0] then h.lname + pi_name else  pi_name + "_" + h.lname // rule-based default 
                        let mapped = // We currently do not use these mapped forms, where pi_name and lname are combined  - instead we use the two fields and combine them ourselves as needed (e.g. in verilog render, with an underscore between, which hopefully comes out the same according to shared conventions).
                            match op_assoc h.lname portMaps with
                                | Some netname ->
                                    //dev_println (sprintf " h.lname=%s mapped to %s" h.lname netname)
                                    netname
                                | None ->
                                    hpr_yikes(sprintf "Bus %s has missing portMap entry for %s.  Using default %s" pi_name h.lname def)
                                    def 
                        let width = port_eval_int def "width field" portmeta h.width
                        let prec = { g_default_prec with widtho=Some(int32 width); signed=g_bool_prec.signed }
                        Some (pi_name, prec, h, mapped)
                    | _::tt -> schema_mine defo searchitem tt

                let req = valOf(schema_mine None "req" hs_nets)
                let ack = valOf(schema_mine None "ack" hs_nets)
                let reqrdy_o = if protocol_prams.reqrdy_present then schema_mine (Some "X_true")  "reqrdy" hs_nets else None
                let ackrdy_o = if protocol_prams.ackrdy_present then schema_mine (Some "X_true")  "ackrdy" hs_nets else None
                Some { protocol=hs_protocol; vlat_req=req; vlat_ack=ack; vlat_reqrdy_o=reqrdy_o; vlat_ackrdy_o=ackrdy_o }



        
let g_null_ats = [] : (string * string) list

// Transactor - performs net-level operations to invoke a remote IP block method call.
//type meld_xactor_t = (string * string option * int) * (string * (string * (string * string) list) list * (string * string) list) // An a-list (methodName/cmd, squirrel, ,arity) -> (pi_name, cams, ats)

type meld_xactor_t = (string * (string * string) list) list // Just the cams : when and what to do at that time.


type proto_meld_busDefinition_t = // As read in from IP-XACT
    { vlvn:             ip_xact_vlnv_t
      directConnection: bool
      broadcast:        bool
      maxMasters:       int
      maxSlaves:        int64 option
      parameters:       (string * string) list
      meld_fsems0:      fun_semantic_t
    }


// A method group (mgr) corresponds to a physical bus or port. An FU may have several such.
// Owing to the sharing of nets, only one method in a method group can be invoked at once, such as the read or write methods of one port of a RAM. But multiple outstanding transactions can exist for pipelined implementations (when II < latency) and different method groups on the same FU always operate independently.
type method_meld_abstraction_t = // One method of a method group (aka of an mgr) of an IP block (aka FU).
     {    method_name:      string
          method_sq_name:   string option  // Overload disambiguation
          voidf:            bool
          arity:            int            // Number of args
          arg_details:      netport_pattern_schema_t list
          meld_xactor:      meld_xactor_t
          meld_fsems1:      fun_semantic_t // 
          return_lname:     string
          // Could put the whole gis in here? No, the data widths and so on are abstract at this point. The arg_details has only default widths in them.
          // Not currently used?
          sim_model:        hblock_t list // for now
          sim_nets:         hexp_t list
     }

type mgr_meld_abstraction_t = // All methods of a method group (aka of an mgr).
        { mgr_name:         string
          contact_schema:   netport_pattern_schema_t list       // Formal contacts
          method_melds:     method_meld_abstraction_t list      // Methods of this  method group
          hs_protocol:      ipblk_protocol_t
          hs_vlat:          vlat_t option

          is_hpr_cv:        bool        // When is_hpr_cv holds, this is a so-called 'canned' library that is part of the HPR L/S runtime system. We are then free to hardwire certain aspects (such as knowledge of existence of the parameter structure, such as for custom bit widths).  See also is_canned which is/was more-or-less the same thing?
        }


type block_meld_abstraction_t = // All method groups on an IP block (aka FU).
        { kinds:            string
          parameters:       (string * string) list              // aka portmeta
          formals_rides:    (string * hexp_t) list              // Parameter overrides - formal schema (with defaults?)
          contact_schema:   netport_pattern_schema_t list       // Formal contacts that are not part of any method group (directorate nets typically).
          mgr_melds:        mgr_meld_abstraction_t list         // Methods in their method groups
          mirrorable:       bool                                // If any methods are non-mirrorable, the whole FU is non mirrorable (additional instances cannot be freely made).
        }

// TODO - in this release the general means of describing which methods are in which mgr is not fully implemented.  It works as follows:
// Canned IP blocks, such as the SSRAMs, can have multiple methods per method group. These have been hand-crafted.
// Imported and exported IP blocks have exactly one method per methodgroup.        

let g_null_block_meld_abstraction__ = 
            { kinds=            "anon-meld-abstraction"
              parameters=       []
              formals_rides=    []
              contact_schema=   []
              mgr_melds=        []
              mirrorable=       true
            }


type proto_mgr_meld_abstraction_t = (netport_pattern_schema_t list * proto_meld_busDefinition_t)

let g_external_bus_definitions_database = new OptionStore<string, proto_meld_busDefinition_t>("bus_definitions_database")

let g_external_bus_abstractions_database1 = new OptionStore<string, proto_mgr_meld_abstraction_t>("mgr_bus_abstractions_database1")

let g_external_bus_abstractions_database2 = new OptionStore<string, block_meld_abstraction_t>("block_abstractions_database2") // TODO bus and mgr are complete synonyms here?

let candidate_filenames idl =
    let first_pref = hptos idl // Try first the full name
    let second_pref = if length idl < 2 then [] else [ hd idl ]
    first_pref :: second_pref


let meld_save a b =
    g_external_bus_abstractions_database2.add a b

// HFAST is our principle vari-latency protocol (adapted to AXI-S by SoC Render instantiated adaptors that are mostly combinational and hence have no logic synthesis overhead)
// An HFAST component has an expected latency but that is not manifest in the protocol.
// It has simplex forms _RONLY and _WONLY.
// Any net-level instance will be be master or slave.
// Where multiple transactors are provided, we have more than one method per port
// -----
// It can cope with various numbers of maximum outstanding transactions, but we mainly/only use 1 here and we instead use the ALU protocol for fully-pipelined, fixed latency operations that have multiple outstanding.
// A server can be always ready, meaning no REQRDY signal.
// A client can also be always ready to accept the reply, meaning there is no ACKRDY signal.
// It cab be posted, meaning no ACK and result signals.    
// Also, it can pack items into lanes or can have multiple simulataneous arg and result busses.



// TODO explain how to enforce dynamic heap on BRAM without manual instantiation of BRAM on a load/store port.

// We have some number of so-called bondout (previously offchip) memory banks, each logical bank has a bank name and its own address space with a memory map manager.
// They are byte-addressed but byte write lanes are not always available.
// Each bank can have more than one load/store port and these may be simplex (ie. load or store only) or duplex.
// HPR SystemIntegrator can deploy bank aggregators to turn several physical memory banks into one logical memory bank.

let loadstore_portName n = sprintf "bondbank%i" n // Was called drambank and offchip bank in the past.



// This is a pseduo for a bondout dynamic heap space.
// Restructure will map its operations to a load/store bondout port.
// We may have a username or a bondout space name at this point.
let retrieve_named_bondout_bank_pseudo ww _ m_dynamic_heaps bankname =

    let ids = "bonk" + bankname

    dev_println (sprintf "Bondout ids=%s" ids)
    match lookup_net_by_string ids with
        | Some(X_bnet ff) ->
            (X_bnet ff, ff.id, ff, lookup_net2 ff.n)
        | None ->
            let ff =
                let width = 64// for now
                let signed = Unsigned
                let arb = g_unspecified_array
                let (nn, ov) = netsetup_start ids
                let wondtoken_ats = [ (g_named_bondout_bank_pseudo, bankname)]
                match ov with
                    | Some vale -> vale
                    | None ->
                        let atsa = map (fun (a,b) -> Nap(a,b)) wondtoken_ats
                        let ff =
                            { 
                                id=        ids
                                width=     int32 width
                                n=         nn
                                rh=         0I
                                rl=         0I
                                constval=  []
                                is_array=  true
                                signed=    signed
                            }
                        let f2 =
                            {
                                length=    [arb]
                                dir=       false
                                pol=       false
                                xnet_io=   LOCAL 
                                vtype=     V_VALUE
                                ats=       atsa
                            } 
                        netsetup_log (ff, f2)
            let bonk = X_bnet ff
            //snets.add ff.id bonk                          
            mutadd m_dynamic_heaps bonk
            (bonk, ff.id, ff, lookup_net2 ff.n)


// Bondout memory banks can have user names allocated by the user and logical names allocated by the recipe.
// A bondout memory bank is accessed via a loadstore port and there can generally be multiple loadstore ports per bank.
// These are mapped to physical banks by substrate wiring or HPR intergrator and so on.
type bondout_memory_map_manager_t(ctor_arg: bondout_address_space_t * bondout_port_t list) = class

    let (space, ports) = ctor_arg
    let vd = 3
    let m_offchipped:offchip_static_region_t list ref = ref []             // Offchip memory map - the part decided at compile time.
    let _ = 
        vprintln 2 (sprintf "Create bondout_memory_map_manager %s with %i load/store ports." space.logicalName (length ports))

    let tableReports: string list list ref = ref []

// TODO also explain how to map multiple arrays to a single BRAM - that should be possible too: but can be done manually by substrate wiring.
//
// Find where a large static is allocated in a bondout memory map.
// There are multiple loadstore ports in general to a given memory space/bank for bandwidth reasons.
//
    let pas (* port_alloc_scan *) memory spaceName = // Allocate word aligned in word terms.  All addresses are in units of one lane (and hence often one byte).
        let rec scan_alloc sofar = function
            | [] -> (None, sofar)
            | (a:offchip_static_region_t)::tt when a.memory = memory-> (Some a, sofar)

            | (a)::tt when a.space.logicalName = spaceName -> //
                let nv = if sofar=None then a.endpt else max a.endpt (valOf sofar)
                scan_alloc (Some nv) tt
            | _ ::tt -> scan_alloc sofar tt 
        let (existing, hwm) = scan_alloc None (!m_offchipped) //TODO might want to round up to some alignment unit here? Yes, please round lanes to words.
        (existing, hwm)

    let offchipToTab (v:offchip_static_region_t) =
        let items = array_lenfun (int64 v.n_items)
        [ xToStr v.memory; i2s v.layout.n_dwidth; i2s v.layout.r_dwidth;  sprintf "%A" v.baser;  items; sprintf "%A" v.endpt;  v.port.ls_key ]

    // Allocate address in off-chip address space. Static region.
    // This must be disjoint from the dynamic region used by any HPR_HEAPMANGER or similar.
    // TODO explain how kept disjoint ...
    let log_offchip_inject_ram (ff:net_att_t) (pp:bondout_port_t) =
        let memory = X_bnet ff
        let (existing, hwm) = pas memory pp.bondout_space.logicalName

        if not (nonep existing) then valOf existing
            elif not ff.is_array then
                sf (sprintf "log_offchip_inject_ram: no length to RAM %s" ff.id)
        else
            let f2 = lookup_net2 ff.n
            let dwidth = encoding_width memory
            if vd >= 5 then vprintln 5 (ff.id + " injected to port " + pp.ls_key)
            let bondout_banko = at_assoc g_named_bondout_bank_pseudo f2.ats   // Runtime heap indication        
            let total_words_available = pp.bondout_space.wordsAvailable
            let n_items = hd f2.length
            if n_items = g_unspecified_array then hpr_yikes (sprintf "Attempt to insert unspecified length into array %s" ff.id)
            let msg = "offchip layout for " + xToStr memory
            let layout = layout_pack_to_lanes_or_words msg pp.ls_key pp.dims.no_lanes pp.dims.laneWidth dwidth
            let v = BigInteger layout.words_item_ratio
            let strathe = if layout.packed then (BigInteger n_items+v-1I)/v else BigInteger n_items * v
                //
                // See if already have a mapping (base address and layout) for this item in a bondout memory space.
            let offset =
                let cmd = at_assoc "offset" f2.ats  // Check for an explicit offset - what if it clashes - TODO raise alias warning. TODO say what units the offset is in.
                if existing <> None then (valOf existing).baser
                elif cmd <> None then atoi(valOf cmd)
                elif hwm <> None then valOf hwm     // Append at hwm
                else 0I

            let t1 = msg + sprintf ": layout report:  %A items,  packed=%A ratio=%i\n\n" n_items layout.packed layout.words_item_ratio
            mutadd tableReports [t1] // temporary log

            let oc_record =
                { space=         pp.bondout_space
                  port=          pp
                  memory=        X_bnet ff
                  endpt=         offset+strathe
                  n_items=       BigInteger n_items
                  baser=         offset
                  layout=        layout
                }

            if nonep bondout_banko && n_items = g_unspecified_array then sf (sprintf "unspecified offchip array length encountered in %A" oc_record)
            vprintln 2 (sprintf "Mapped %s to offchip RAM/DRAM %s. Total words=%i (%i bytes). " ff.id pp.bondout_space.logicalName  total_words_available (total_words_available * int64(pp.dims.no_lanes * pp.dims.laneWidth/8)) + sfold (fun x->x) (offchipToTab oc_record) ) // This gives no col headings - rather unreadable.
            mutadd m_offchipped oc_record
            //vprintln 3 ("Mapped bondout/offchip RAM/DRAM " + item + " " + xToStr memory + " found existing=" + optToStr existing + " len=" + i2s(length !m_offchipped))
            oc_record


    member x.inject_ram ff pp = log_offchip_inject_ram (ff:net_att_t) (pp:bondout_port_t)

    member x.create_bondout_report m_tableReports =
        let t = tableprinter.create_table("Bondout/Offchip Memory Map - Lane addressed.", ["Resource"; "N-Width"; "R-Width"; "Base Address";"No Items"; "End+1"; "Portname"], map offchipToTab !m_offchipped)
        let t1 = "Memory Map Offchip\n":: t
        mutadd m_tableReports t1
        aux_report_log 2 "bondout memory map manager" t1
        //let _ = app (vprintln 2) t1
        ()


    member x.space_availablep (sizeRequest:int64) =
        let (_, hwm) = pas X_undef space.logicalName
        let hwm = int64 (valOf_or hwm 0I)
        let bankCapacity = space.wordsAvailable * int64 space.wordWidth / 8L
        let ans = bankCapacity - hwm <= sizeRequest
        vprintln 2 (sprintf "check space available in bank %s  %i - %i <= %i answer=%A" space.logicalName bankCapacity hwm sizeRequest ans)
        ans
        
    member x.port_alloc_scan memory spaceName = pas memory spaceName
#if OLD
        { stagename=           stagename        
          tableReports=        m_table_reports          
          vd=                  vd
          m_offchipped=        m_offchipped
          offchip_disable=     false
          bondout_ports=       bondout_port_prams
        }
#endif
end


//
//  Bondout port stucture is defined by a coded string.
//   
let parse_logical_port_schema ww c1 (bspace_id, schema) =
    let ww = WF 2 "parse_logical_port_schema" ww (sprintf "Start id=%s  schema=%s" bspace_id schema)
    let items = split_string_at [ ',' ] schema
      
    let msg = "bondout specification"
    let rec rax ports args =
        match args with
        | ss::tt when strlen ss >= 3 && ss.[0..2] = "H1H" -> rax ((rax_pcol_subschema msg ss.[3..] PD_halfduplex) :: ports) tt
        | ss::tt when strlen ss >= 3 && ss.[0..2] = "H1L" -> rax ((rax_pcol_subschema msg ss.[3..] PD_loadport)   :: ports) tt
        | ss::tt when strlen ss >= 3 && ss.[0..2] = "H1S" -> rax ((rax_pcol_subschema msg ss.[3..] PD_storeport)  :: ports) tt
        | others ->
            if length others <> 3 then cleanexit(sprintf "Cannot parse bondout schema '%s'  schema=%s. Expected 3 numeric fields at the end but encountered: '%s'" bspace_id schema (sfold id others))
            let addressableSize = atoi64(hd others) // In Words - This is the actual size 
            let no_lanes = atoi32(cadr others)      // Number of lanes
            let laneWidth = atoi32(caddr others)    // Width in bits of each lane
            let bytes_size = addressableSize * int64((no_lanes * laneWidth)/8)
            let pigs = zipWithIndex (rev ports)
            // A lane is normally 8 bits wide.  But whatever width a lane is, we have a 'lane-addressed' address bus.  The bottom (log2 no_lanes) address bits will then always be ignored (and stripped by logic synthesiser tool).
            let lane_addr_size = control_get_i c1   "bondout-loadstore-lane-addr-size" 24
            let wordsize = sprintf "One word is %i lanes of %i bits" no_lanes laneWidth
            
            let max_addressable_words = two_to_the lane_addr_size
            if max_addressable_words < BigInteger addressableSize then
                let msg = (sprintf "Problem with bondout space address bus width: specified size is not addressable with this width address bus: Size=%i words (%i bytes) address_bus_width=%i max_addressable_words=%A.  Wordsize='%s'"  addressableSize bytes_size lane_addr_size max_addressable_words wordsize)
                cleanexit msg

                
            vprintln 2 (sprintf "Parsed bondout memory space: name=%s size=%i words (%i bytes), lane_addr_size=%i no_lanes=%i, laneWidth=%i, boundout arity (no of ports) %i." bspace_id addressableSize bytes_size lane_addr_size no_lanes laneWidth (length ports))
            let space =
                {
                    logicalName=      bspace_id
                    m_userNames=      ref []
                    wordsAvailable=   addressableSize
                    wordWidth=        no_lanes*laneWidth
                }
            let dims =
                {
                    addrSize=        lane_addr_size 
                    no_lanes=        no_lanes
                    laneWidth=       laneWidth
                    bytesAvailable=  addressableSize*int64(no_lanes*laneWidth/8)
                }
            let gec_loadstore_port ((protocol, clk_domain), idx_no) =
               {
                   ls_key=         sprintf "%s_%i" bspace_id idx_no
                   bondout_space=  space
                   protocol=       protocol
                   dims=           dims
                   clock_domain=   clk_domain
              }            

            (space, map gec_loadstore_port pigs)
    let bondout_spaces_and_ports_from_schema_string = rax [] items

    bondout_spaces_and_ports_from_schema_string


//
// Read from recipe or command line the number of bondout load/store ports to allow.
//   
let really_get_bondout_port_prams ww c1 stagename log_setting_for_report =
    let ww = WF 2 "really_get_bondout_port_prams" ww "Start"

    let lane_addr_size = control_get_i c1   "bondout-loadstore-lane-addr-size" 24
    log_setting_for_report "bondout-loadstore-lane_addr-size" 24L (int64 lane_addr_size) ""

    // This setting is part of the old manifest, but if it has been manually set to zero we null the new schema too.
    let loadstore_port_count= control_get_i c1 "bondout-loadstore-port-count" 1        
    log_setting_for_report "bondout-loadstore-port-count" 1L (int64 loadstore_port_count) ""

    let protocol = // Default bondout protocol
        match control_get_s stagename c1 "bondout-protocol" None with
            | "HFAST1" -> "HFAST1"
            | ss        -> ss

    let old_bondout_port_manifest() = 
        // These are the old command line settings:
        let simplex_ports__ = control_get_s stagename c1 "bondout-loadstore-simplex-ports" None = "enable"
        let clock_domain = xToStr g_clknet // This needs to be user-controlled in the new manfiest at least.
        // We prefer to have a fully explicit specification in a boundout-schema

        // The bondout ports need to be allocated to logical banks.  We encode all the information in a string normally.
        // bondout port codes H1H denotes bidirectional (half duplex) H1L denotes simplex load, H1SS denoes simplex store
        // A semicolon-separated list of bondout names and codes. name=code. Items are repeated to make more than one port for a logical bank.


        let bytesAvailable= control_get_i64 c1 "bondout-dram-available-size-bytes" (int64 (1<<<20)) // This and some other info should be shared over multiple loadstore ports.
        let no_lanes=       control_get_i c1   "bondout-loadstore-port-lanes" 8
        let laneWidth=      control_get_i c1   "bondout-loadstore-lane-width" 8

        let bspace =
            {
                logicalName=     "bondspace0"
                wordWidth=       (no_lanes*laneWidth)
                wordsAvailable = bytesAvailable / int64(no_lanes*laneWidth/8)
                m_userNames=     ref []
            }

        let dims =
            {
                addrSize=            lane_addr_size
                no_lanes=            no_lanes
                laneWidth=           laneWidth
                bytesAvailable=      bytesAvailable //addressableSize*int64(no_lanes*laneWidth/8)
            }
        let duplex = PD_halfduplex  // Only this way for now - we'll need to refine this based on simplex_ports soon.
        let protocol_prams = { max_outstanding=1; req_present=true; ack_present=true; reqrdy_present=true; ackrdy_present=false;  posted_writes_only=false }
        let goc n =
                  {
                    ls_key=         loadstore_portName n
                    dims=           dims
                    //duplex=       duplex
                    protocol=       IPB_HFAST(protocol_prams, duplex)
                    bondout_space=  bspace
                    clock_domain=   clock_domain
                  }

        [(bspace, map goc [0..loadstore_port_count-1])]

    // New approach uses a schema string.  A JSON or XML spec for these so that they can be diverse would be better?
    let bondouts =     // An example schema string "bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8"
                       // We need to be able to allocate clock domains on a per port basis, even when shared over spaces. We do this by putting minus signs between key/value pairs, but before the numeric fields. e.g. H1H-clk=myclk 
    
        if loadstore_port_count = 0 then []
        else
        match control_get_s stagename c1 "bondout-schema" None with
            | "disable"   -> [] 
            | "oldway"    -> old_bondout_port_manifest()
            | sx ->
                let prams = string_to_assoc_list sx
                let logical_ports = map (parse_logical_port_schema ww c1) prams
                logical_ports

    let rez_memory_map_manager bondout_bank = (bondout_bank, new bondout_memory_map_manager_t(bondout_bank))
    map rez_memory_map_manager bondouts
    

let g_bondout_ports = ref None

let get_bondout_port_prams ww c1 stagename log_setting_for_report =
    match !g_bondout_ports with
        | Some ans -> ans
        | None ->
            let ans = really_get_bondout_port_prams ww c1 stagename log_setting_for_report
            g_bondout_ports := Some ans
            ans


let g_bondout_arg_patterns =
    [ 
        // Used always
        Arg_int_defaulting("bondout-loadstore-lane-addr-size", 22, "LOADSTORE address bus width in bits. In other words, Log2(words*lanes) of off-chip RAM."); 


        // Part of pre-schema string system
        Arg_enum_defaulting("bondout-loadstore-simplex-ports", ["enable"; "disable"; ], "disable", "Generate simplex load/store ports instead of halfduplex: simplex is better for AXI bond out.");            
        Arg_int_defaulting("bondout-loadstore-port-count", 1,   "Number of loadstore ports per thread for automatic off-chipping of large arrays and dynamic storage allocation (new() calls).");
        Arg_int_defaulting("bondout-loadstore-port-lanes", 32, "Loadstore ports - number of write lanes.");
        Arg_int_defaulting("bondout-loadstore-lane-width", 8, "Loadstore lane width - total width = bits per lane * number of lanes1.");
        Arg_defaulting("bondout-protocol", "HFAST1", "Default Offchip/Bondout protocol.");


        // Schema string system (new)
        Arg_defaulting("bondout-schema", "bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8", "Offchip memory schema");
    ]

//



// Virtually Spare?
let rez_phys_methods__ ww msg mkey kinds portmeta data_prec fu_meld latency_pair = 
    vprintln 2 (sprintf "Rezdown: gec_phys_methods: mkey=%s kinds=%s portmeta=%s" mkey kinds (sfold (fun (k, v)->sprintf "%s/%s" v k) portmeta))
    dev_println ("pi_name needed in rezdown")
    let phys_methods =
        let gec_mgr_methods mgr_meld =
            let pi_name = mgr_meld.mgr_name
            let gec_method (method_meld:method_meld_abstraction_t)  =
                let arg_details =
                    let baz nps =
                        let width = int(port_eval_int "rez_phys_methods" kinds portmeta nps.width)
                        ({ signed=nps.signed; widtho=Some width }, (pi_name, nps.lname, "mapped+"))
                    map baz method_meld.arg_details
                // Traditionally we do not set the .is_fu field for 'canned' CV/HPR FUs.
                let (rv, ret_details) =
                    if method_meld.voidf then (g_void_prec, None) else (data_prec, Some(pi_name, method_meld.return_lname, "mapped+"))
                let gis = { g_default_native_fun_sig with overload_suffix=method_meld.method_sq_name; fsems=method_meld.meld_fsems1; args=map fst arg_details; rv=rv; latency=latency_pair }
                ((method_meld.method_name, method_meld.method_sq_name, method_meld.arity), (gis, ret_details, latency_pair, arg_details))
            map gec_method mgr_meld.method_melds
        list_flatten (map gec_mgr_methods fu_meld.mgr_melds)
    phys_methods



// The heap manager can be written in C# and compiled separately.  It is then a loaded module. Its schema should then not be canned here.
// Canned heapmanager - perhaps not  for use in future - use loadable one, but it's handy to have this one for simple diosim runs.
// hpr_alloc accepts an address space number or name.  This is mapped to the structural instance that manages that space. The structural components do not have a space argument.
// For a monolithic design, the heap manager could be in-lined with the main component HLS compile.
// But even with a monolithic design, there may be multiple threads currently making access to the heap manager.
// For an incremental design, where more than one component allocates heap items,  the heap manager needs to be callable by each component.
let rez_canned_heapmanager ww model pi_name = 
    vprintln 2 (sprintf "Created HPR_HEAPMANAGER schema model=%s" model)
    let rv_prec = g_pointer_prec
    let kind_vlnv = { g_canned_default_iplib with kind=["HPR_HEAPMANGER_" + model ]; }
    let size = { g_null_netport_pattern_schema with lname="SIZE_IN_BYTES";   width="64";  sdir="i";    ats=[] }
    let port_schema = 
      [
          size
          { g_null_netport_pattern_schema with lname="RESULT";          width=i2s(valOf rv_prec.widtho);  sdir="o";  signed=rv_prec.signed;  ats=[(g_ip_xact_is_Data_tag, "true")]  };
          { g_null_netport_pattern_schema with lname="FAIL";            width="1";   sdir="o";    idleval="0"; ats=[] };  // A one-bit abend code
      ]

    let protocol = IPB_HFAST({ max_outstanding=1; req_present=true; ack_present=true; reqrdy_present=true; ackrdy_present=false; posted_writes_only=false;  }, PD_halfduplex)
    let (hs_net_schema, vlat, hs_t0_control, hs_t1_control) = gec_hs_net_schema_vlat_and_melsh ww protocol
      
    let gop_xactor:meld_xactor_t = 
                   [ ("t0", [ ("SIZE_IN_BYTES", "ARG0"); ("REQ", "G") ] @ hs_t0_control)
                     ("t1", [ ("RESULT", "RV") ] @ hs_t1_control)
                   ]
    let method_meld =
                       { method_name=      "hpr_alloc"
                         method_sq_name=   None
                         return_lname=     "RESULT"
                         arity=            2
                         voidf=            false
                         arg_details=      [size]
                         meld_xactor=      gop_xactor
                         meld_fsems1=      { g_default_fsems with fs_nonref=true; fs_mirrorable=false } //Heap Manager
                         // Not currently used?
                         sim_model=        []
                         sim_nets=         []
                       }
    let mgr_meld =
        {
            mgr_name=         ""
            method_melds=     [ method_meld ]
            contact_schema=   hs_net_schema @ port_schema // netport_pattern_schema_t list       // Formal contacts
            hs_protocol=      protocol
            hs_vlat=          vlat
            is_hpr_cv=        true
        }

    dev_println (sprintf "rez_canned_heapmanger  hs_net_schema  is %A" hs_net_schema )

    let fu_meld = 
      { kinds=            hptos kind_vlnv.kind
        parameters=       [] // (string * string) list              // aka portmeta
        formals_rides=    [] // (string * hexp_t) list              // Parameter overrides - formal schema (with defaults?)
        contact_schema=   [] // Additional beyond mgr nets - clocks and reset please - or else explain where they are added
        mgr_melds=        [ mgr_meld ]
        mirrorable=       false
      }
    let externally_instantiated = false // Make programmable? TODO - need external when shared over separate compilations.
    let variable_latency = true
    (kind_vlnv, fu_meld, mgr_meld, rv_prec, externally_instantiated, variable_latency, mgr_meld.contact_schema)

let xactor_name_to_str = function
    | ((name, None,    arity), _) -> sprintf "  (%s, <NoOverloadDisam>, %i)" name arity
    | ((name, Some sq, arity), _) -> sprintf "  (%s, %s, %i)" name sq arity                 


// Further Temporary code for the TEMP HEAPMANAGER !
let gec_temp_heapmanager_meld ww block_kind msg port_schema mgr_meld fname arity  rv_prec fu_meld sq externally_instantiated =
            let pi_name = ""
            let structure =
                let gest schema_entry =
                    let formal = schema_entry.lname
                    (pi_name, formal, schema_entry)
                map gest port_schema
            let portMaps = [] 
            let portmeta = [] // (string * string) list
            let vlat = grock_protocol_vlat ww msg pi_name mgr_meld.hs_protocol portMaps portmeta port_schema
            let latency_pair = Some (1, 1)

            let kinds = hptos block_kind
            let mkey = kinds
            let phys_methods__ = rez_phys_methods__ ww msg mkey kinds portmeta rv_prec fu_meld latency_pair // Should call/use this
            let area = Some 100.0  // Arbitrary
            let phys_methods = [ ((fname, None, arity), (snd g_hpr_alloc_gis, Some(pi_name, "RESULT", "64"), vlat, [ (rv_prec, (pi_name, "SIZE_IN_BYTES", "mapped+"))])) ] // :    ((string * string option * int) * (native_fun_signature_t * (string * string * string) option * vlat_t option * (precision_t * (string * string * string)) list)) list

#if SPARE
            let block__ = // aka FU or AFU. This is hardly used now - please delete it?
                  {
                      //ip_vlnv=    kind
                      kind=       hptos kind.kind
                      phys_methods=    phys_methods
                      //typical_latency=     3 // crude
                      //typical_rei=         3
                      meta=       portmeta
                      phys_structure__=  structure//  (string * string * netport_pattern_schema_t) list // Formal pi_name, formal net name and basic nps pattern.
                      phys_area=  area
                      meld=       fu_meld
                  }
#endif
            // Should call assoc_external_method not copy it out here
            let methoder = 
                match op_assoc (fname, sq, arity) phys_methods with
                    | None ->
                        let available = sprintf "\nCanned methods available are ^%i: " (length phys_methods) + sfold xactor_name_to_str phys_methods
                        cleanexit(msg + sprintf ": Cannot find method %s,  sq=%A with arity %i in external IP block block_kind=%s" fname sq arity (hptos block_kind) + available)
                    | Some methoder -> methoder

            //let externally_instantiated = false // Not correct always for heap_manager
            (fu_meld, methoder, externally_instantiated, fu_meld.mirrorable)

            

let g_cv_sram_rides_schema = [ "DATA_WIDTH"; "ADDR_WIDTH"; "WORDS"; "LANE_WIDTH"; "trace_me" ]  // CV_SRAM meta info schema.    

// Static RAMs: example kinds are of the form CV_SP_SSRAM_FL0, CV_SP_SSRAM_FL1 and CV_2P_SSRAM_FL1.
let gen_cv_ssram_port_structure latency ports =  // Synchronous static RAMs - with multiple ports when needed.
    let kinds = (if ports < 2 then "CV_SP_SSRAM" else sprintf "CV_%iP_SSRAM" ports)
    let kinds = kinds + sprintf "_FL%i" latency
    // We add (g_control_net_marked, "true") to all nets at the moment, despite them being do_not_trim because conerefine will delete their drive on internal instatiations...

    let formals_rides = map (fun ss -> (ss, gec_X_net ss)) g_cv_sram_rides_schema
    let port_manifest = (if ports <= 1 then [ -1] else [ 0..ports-1])
    let portmeta = []

    let gen_mgr_meld port_no =
        let ps = if port_no < 0 || ports < 2 then "" else i2s port_no
        let lps = ""
        let pi_name = "" // using ps instead of this for now
        let addr =  { g_null_netport_pattern_schema with lname="addr" + lps;   chan="RW";  width="ADDR_WIDTH";  sdir="i";    pon="d"; ats=[(g_ip_xact_is_Address_tag, "true");(g_control_net_marked, "true")] };        
        let wdata = { g_null_netport_pattern_schema with lname="wdata" + lps;  chan="RW";  width="DATA_WIDTH";  sdir="i";    pon="d"; ats=[(g_ip_xact_is_Data_tag, "true");(g_control_net_marked, "true")] };
        let gen_contact_schema =
                [ // Positional order is commonly used in cvgates.v so keep this order please.
                  { g_null_netport_pattern_schema with lname="rdata" + lps;  chan="RW";  width="DATA_WIDTH";  sdir="o";    pon="d"; ats=[(g_ip_xact_is_Data_tag, "true")]  };
                  addr
                  { g_null_netport_pattern_schema with lname="wen" + lps;    chan="RW";  width="1";           sdir="i";    pon="d"; idleval="0"; ats=[(g_control_net_marked, "true")] };
                  { g_null_netport_pattern_schema with lname="ren" + lps;    chan="RW";  width="1";           sdir="i";    pon="d"; idleval="0"; ats=[(g_control_net_marked, "true")] };
                  wdata
                ]

        let sim_model = [] // for now - its canned inside restructure.fs - no its not - try cvipgen

        let gen_meld_xactor no cmd = // :meld_xactor_t 
            let lps = "" // OLD: if port_no < 0 || ports < 2  then "" else i2s port_no
            match cmd with
                | "write" ->
                   [ ("t0", [ ("addr" + lps, "ARG0"); ("wdata" + lps, "ARG1"); ("wen" + lps, "G"); ]) ]

                | "read" ->
                    [ ("t0", [ ("addr" + lps, "ARG0"); ("ren" + lps, "G"); ]);  
                      ("t1", [ ("rdata" + lps, "RV") ]);
                    ]

        let gec_method_meld cmd : method_meld_abstraction_t =
            let (voidf, arity, fsems, arg_details) =
                if cmd = "write" then (true, 2, { g_default_fsems with fs_nonref=true; fs_eis=true }, [ addr; wdata ])  // Write is EIS, read is not. 
                else (false, 1, { g_default_fsems with fs_nonref=true }, [ addr ])

            { method_name=      cmd
              method_sq_name=   None
              return_lname=     "rdata"
              voidf=            voidf
              arg_details=      arg_details
              arity=            arity
              meld_xactor=      gen_meld_xactor port_no cmd
              sim_model=        sim_model; sim_nets = []
              meld_fsems1=      fsems
            }

               
        in
        {
            mgr_name=         ps
            method_melds=     [ gec_method_meld "write"; gec_method_meld "read" ]
            contact_schema=   gen_contact_schema
            hs_protocol=      IPB_FPFL
            hs_vlat=          None
            is_hpr_cv=        true
        }

    let fu_meld:block_meld_abstraction_t =
        { kinds=          kinds
          contact_schema= []
          parameters=     portmeta
          formals_rides=  formals_rides
          mgr_melds=      map gen_mgr_meld port_manifest 
          mirrorable=     false
        }
    meld_save kinds fu_meld
    (fu_meld) // end of canned static RAM



    
    
// Synchronous static ROM - single ported (for further ports, please mirror the whole ROM)
let gen_cv_ssrom_port_structure withRen latency suffix = 
    let kinds = "AS_SSROM_" + suffix
    let pi_name = ""  // No port instance name - there is one anonymous port.

    cassert(latency=1, "Latency unity on synchronous ROM") // Others in future?
    
    let addr_schema_line = { g_null_netport_pattern_schema with lname="addr";   chan="RW";  width="ADDR_WIDTH";  sdir="i";    pon="d"; ats=[g_no_decl_alias; (g_ip_xact_is_Address_tag, "true")] };

    let ren_schema_line = { g_null_netport_pattern_schema with lname="ren";    chan="RW";  width="1";           sdir="i";    pon="d"; idleval="0"; ats=[g_no_decl_alias; (g_control_net_marked, "true")] };
      

    let contact_schema =     
       [  // Positional order is commonly used in cvgates.v so keep this order please.
         { g_null_netport_pattern_schema with lname="rdata";  chan="RW";  width="DATA_WIDTH";  sdir="o";    pon="d"; ats=[g_no_decl_alias; (g_ip_xact_is_Data_tag, "true")]  }; 
         addr_schema_line
       ] @ (if withRen then [ren_schema_line] else [])


    let xactor = // TLM methods defined for this component
        [ 
          ("t0", [ ("addr", "ARG0")] @ (if withRen then [("ren", "G")] else []));
          ("t1", [ ("rdata", "RV") ]);
        ]
    let sim_model = [] // for now
    let portmeta = []  // for now
    let method_meld:method_meld_abstraction_t =
        { method_name=    "read"
          method_sq_name= None
          return_lname=   "rdata"
          voidf=          false
          arity=          1
          arg_details=    [addr_schema_line]
          meld_xactor=    xactor
          sim_model=      sim_model; sim_nets = []
          meld_fsems1=    { g_default_fsems with fs_nonref=false; fs_mirrorable=true } // ROM
        }

    let mgr_meld =
        {
            mgr_name=         pi_name
            method_melds=     [ method_meld ]
            contact_schema=   contact_schema
            hs_protocol=      IPB_FPFL
            hs_vlat=          None
            is_hpr_cv=        true
        }

    let fu_meld:block_meld_abstraction_t =
        { kinds=          kinds
          contact_schema= []
          formals_rides=  [] // For ROM.
          mgr_melds=      [ mgr_meld ]
          parameters=     []
          mirrorable=     true
        }
    meld_save kinds fu_meld // ROM - not dual-port ROMs are possible in Virtex 7 ... these need supporting.
    vprintln 2 (sprintf "gen_cv_ssrom_port_structure kinds=%s withRen=%A  latency=%i"  kinds withRen latency)
    (fu_meld)



// Generate meld (metainfo) for a canned (CV) ALU
let gen_cv_ALU_port_structure ww oo kkey prec latency variable_latencyf =  // 
    let pi_name = "" // ALU has no port groups.
    let rw = valOf_or prec.widtho 32 // Both the same
    let dw = valOf_or prec.widtho 32

    let g_cv_int_div_schema_rides = [ "RWIDTH"; "NWIDTH"; "DWIDTH"; "trace_me" ]
    let g_cv_int_alu_schema_rides = [ "RWIDTH"; "A0WIDTH"; "A1WIDTH"; "trace_me" ]


    let fp = prec.signed=FloatingPoint
    let formals_rides = if fp then [] elif oo=V_divide || oo=V_mod then  g_cv_int_div_schema_rides else g_cv_int_alu_schema_rides
    let formals_rides = map (fun ss -> (ss, gec_X_net ss)) formals_rides
    let (a0, a1) = if oo=V_divide || oo=V_mod then ("NN", "DD") else ("XX", "YY")

    let portmeta = [ ("RANGE_WIDTH", i2s rw); ("DOMAIN_WIDTH", i2s dw) ]
     
    let arg0 = { g_null_netport_pattern_schema with lname=a0;     chan="RW";  width="DOMAIN_WIDTH";  sdir="i";    pon="d"; signed=prec.signed  };
    let arg1 = { g_null_netport_pattern_schema with lname=a1;     chan="RW";  width="DOMAIN_WIDTH";  sdir="i";    pon="d"; signed=prec.signed  };          
    let contact_schema =     
       [  // Positional order is commonly used in cvgates.v so keep this order please.
         { g_null_netport_pattern_schema with lname="RR";   chan="RW";  width="RANGE_WIDTH";   sdir="o";    pon="d"; signed=prec.signed  };
         arg0
         arg1
         { g_null_netport_pattern_schema with lname="FAIL"; chan="RW";  width="1";             sdir="o";    pon="d"; idleval="0" };
       ]

    let protocol = 
        match variable_latencyf with
            | true ->  IPB_HFAST({ max_outstanding=1; req_present=true; ack_present=true; reqrdy_present=false; ackrdy_present=false; posted_writes_only=false }, PD_halfduplex)
            | false -> IPB_FPFL
    let (hs_net_schema, vlat, hs_t0_control, hs_t1_control) = gec_hs_net_schema_vlat_and_melsh ww protocol

    let compute_xactor = // TLM method defined for this component
        [ 
          ("t0", [ (a0, "ARG0"); (a1, "ARG1"); ] @ hs_t0_control)
          ("t1", [ ("RR", "RV") ]  @ hs_t1_control)
        ]

            
    let sim_model = [] // for now- created in cvip at the moment

    let method_meld:method_meld_abstraction_t =
        { method_name=    "compute"
          method_sq_name= None
          return_lname=   "RR"
          voidf=          false
          arity=          2
          arg_details=    [arg0; arg1]
          meld_xactor=    compute_xactor
          meld_fsems1=    { g_default_fsems with fs_nonref=false; fs_mirrorable=true } // ALU
          sim_model=      sim_model
          sim_nets =      []
        }

    let mgr_meld =
        {
            mgr_name=         pi_name
            method_melds=     [ method_meld ]
            contact_schema=   hs_net_schema @ contact_schema
            hs_protocol=      protocol
            hs_vlat=          vlat
            is_hpr_cv=        true
        }

    let fu_meld:block_meld_abstraction_t =
        { kinds=          kkey
          parameters=     portmeta
          formals_rides=  formals_rides
          contact_schema= []
          mgr_melds=      [ mgr_meld ]
          mirrorable=     true
        }
    meld_save kkey fu_meld
    fu_meld // end of gen_cv_ALU_port_structure


let gec_cv_fpcvt_name_and_gis pto pfrom latency_o =
    let latency_o = valOf_or latency_o 2
    let kind_vlnv = gec_cv_fpcvt_name pto pfrom latency_o

    let (fname, gis) = 
        match (pto.signed, pto.widtho, pfrom.signed, pfrom.widtho) with
            | (Signed, Some 32,    FloatingPoint, Some 32) -> g_hpr_flt2int32_fgis
            | (Signed, Some 64,    FloatingPoint, Some 32) -> g_hpr_flt2int64_fgis            
            | (Signed, Some 32,    FloatingPoint, Some 64) -> g_hpr_dbl2int32_fgis
            | (Signed, Some 64,    FloatingPoint, Some 64) -> g_hpr_dbl2int64_fgis            

            | (FloatingPoint, Some 32,    Signed, Some 32) -> g_hpr_flt_from_int32_fgis
            | (FloatingPoint, Some 32,    Signed, Some 64) -> g_hpr_flt_from_int64_fgis
            | (FloatingPoint, Some 64,    Signed, Some 32) -> g_hpr_dbl_from_int32_fgis
            | (FloatingPoint, Some 64,    Signed, Some 64) -> g_hpr_dbl_from_int64_fgis
            | _ -> sf(sprintf "No canned F/P convertor to %s from %s FU available" (prec2str pto) (prec2str pfrom))

    (kind_vlnv, fname, gis)


// Floating point to/from integer convertors
let gen_cv_fpcvt_port_structure ww pto pfrom latency_o =  
    let latency = valOf_or latency_o 2
    let (kind_vlnv, fname, gis) = gec_cv_fpcvt_name_and_gis pto pfrom None
    let kkey = hptos kind_vlnv.kind // This is also the mkey since these are mirrorable.
    let pi_name = ""      // No port groups on these.
    let rw = valOf_or pto.widtho 32
    let dw = valOf_or pfrom.widtho 32
    let portmeta = [ ("RANGE_WIDTH", i2s rw); ("DOMAIN_WIDTH", i2s dw) ]

    let arg0 = { g_null_netport_pattern_schema with lname="arg";      chan="RW";  width="DOMAIN_WIDTH";  sdir="i";    pon="d"; signed=pfrom.signed };
    let contact_schema =     
       [  // Positional order is commonly used in cvgates.v so keep this order please.
         { g_null_netport_pattern_schema with lname="result";   chan="RW";  width="RANGE_WIDTH";   sdir="o";    pon="d"; signed=pto.signed  };
         arg0
         { g_null_netport_pattern_schema with lname="FAIL"; chan="RW";  width="1";             sdir="o";    pon="d"; idleval="0" };
       ]
    let protocol = IPB_FPFL
    let (hs_net_schema, vlat, hs_t0_control, hs_t1_control) = gec_hs_net_schema_vlat_and_melsh ww protocol
       
    let compute_xactor = // TLM methods defined for this component
        [ 
         ("t0", [ ("arg", "ARG0")  ] @ hs_t0_control)
         ("t1", [ ("result", "RV") ] @ hs_t1_control)
        ]

    let sim_model = [] // for now- created in cvipgen at the moment

    let method_meld:method_meld_abstraction_t =
        { method_name=    fname
          method_sq_name= None
          return_lname=   "result"
          voidf=          false
          arity=          1
          arg_details=    [arg0]
          meld_xactor=    compute_xactor
          meld_fsems1=    { g_default_fsems with fs_nonref=false; fs_mirrorable=true } // FPCVT
          sim_model=      sim_model
          sim_nets =      []
        }

    let mgr_meld =
        {
            mgr_name=         pi_name
            method_melds=     [ method_meld ]
            contact_schema=   hs_net_schema @ contact_schema
            hs_protocol=      protocol
            hs_vlat=          vlat
            is_hpr_cv=        true
        }

    let fu_meld:block_meld_abstraction_t =
        { kinds=          kkey
          contact_schema= []
          formals_rides=  []
          parameters=     portmeta
          mgr_melds=      [ mgr_meld ]
          mirrorable=     true
        }
    meld_save kkey fu_meld
    (fu_meld, mgr_meld, method_meld) //   FPCVT



let g_axi_port_structure = 
 [
// s=streaming h=Heavy and l=light signal options. AXI3 and 4.
 { g_null_netport_pattern_schema with lname="AWVALID";  chan="AW";  width="1";       sdir="i";     pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="AWREADY";  chan="AW";  width="1";       sdir="o";     pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="AWADDR";   chan="AW";  width="ADDR_WIDTH";  sdir="i"; pon="34hl"; ats=[(g_ip_xact_is_Address_tag, "true")] };
 { g_null_netport_pattern_schema with lname="AWPROT";   chan="AW";  width="3";       sdir="i";     pon="34hl" }; // {instr,non-secure,priveleged}

 
 { g_null_netport_pattern_schema with lname="WDATA";    chan="W";  width="DATA_WIDTH";        sdir="i";    pon="34hls";  ats=[(g_ip_xact_is_Data_tag, "true")] };
 { g_null_netport_pattern_schema with lname="WSTRB";    chan="W";  width="NUMBER_OF_LANES";   sdir="i";    pon="34hls" };
 { g_null_netport_pattern_schema with lname="WVALID";   chan="W";  width="1";        sdir="i";    pon="34hls"; idleval="0" };
 { g_null_netport_pattern_schema with lname="WREADY";   chan="W";  width="1";        sdir="o";    pon="34hls"; idleval="0" };

 { g_null_netport_pattern_schema with lname="BRESP";    chan="B";  width="2";        sdir="o";    pon="34hl" }; // 00=ok/exfail,01=exok,10=slverr,11=decerr.
 { g_null_netport_pattern_schema with lname="BVALID";   chan="B";  width="1";        sdir="o";    pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="BREADY";   chan="B";  width="1";        sdir="i";    pon="34hl"; idleval="0" };

 { g_null_netport_pattern_schema with lname="ARADDR";   chan="AR";  width="ADDR_WIDTH";  sdir="i";    pon="34hl"; ats=[(g_ip_xact_is_Address_tag, "true")] };
 { g_null_netport_pattern_schema with lname="ARVALID";  chan="AR";  width="1";       sdir="i";    pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="ARREADY";  chan="AR";  width="1";       sdir="o";    pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="ARPROT";   chan="AR";  width="3";       sdir="i";    pon="34hl" }; // {instr,non-secure,priveleged}

 { g_null_netport_pattern_schema with lname="RDATA";    chan="R";  width="DATA_WIDTH";   sdir="o";    pon="34hl";  ats=[(g_ip_xact_is_Data_tag, "true")] };
 { g_null_netport_pattern_schema with lname="RRESP";    chan="R";  width="2";        sdir="o";    pon="34hl" };
 { g_null_netport_pattern_schema with lname="RVALID";   chan="R";  width="1";        sdir="o";    pon="34hl"; idleval="0" };
 { g_null_netport_pattern_schema with lname="RREADY";   chan="R";  width="1";        sdir="i";    pon="34hl"; idleval="0" };

 { g_null_netport_pattern_schema with lname="ACLK";     chan="D";  width="1";        sdir="x";    pon="34hls" };
 { g_null_netport_pattern_schema with lname="ARESETn";  chan="D";  width="1";        sdir="x";    pon="34hls" };
 { g_null_netport_pattern_schema with lname="INTERRUPT";chan="D";  width="1";        sdir="o";    pon="34hlO" }; // Optional

// Heavy only signals
 { g_null_netport_pattern_schema with lname="AWID";     chan="AW";  width="ID_WIDTH"; sdir="i";   pon="34h" };
 { g_null_netport_pattern_schema with lname="AWLEN";    chan="AW";  width="8";       sdir="i";    pon="34h" }; // Burst length minus 1
 { g_null_netport_pattern_schema with lname="AWSIZE";   chan="AW";  width="3";       sdir="i";    pon="34h" }; // Transfers per beat
 { g_null_netport_pattern_schema with lname="AWBURST";  chan="AW";  width="2";       sdir="i";    pon="34h" }; // 00=Fixed,01=Inc,10=Wrap
 { g_null_netport_pattern_schema with lname="AWLOCK";   chan="AW";  width="2";       sdir="i";    pon="3h" };
 { g_null_netport_pattern_schema with lname="AWLOCK";   chan="AW";  width="1";       sdir="i";    pon="4h" };  // Set for store-conditional
 { g_null_netport_pattern_schema with lname="AWCACHE";  chan="AW";  width="4";       sdir="i";    pon="34h" }; // 0-3=cache-bypass, 4-F=cacheable
 { g_null_netport_pattern_schema with lname="AWREGION"; chan="AW";  width="4";       sdir="i";    pon="34hO" }; // Optional
 { g_null_netport_pattern_schema with lname="AWQOS";   chan="AW";   width="4";       sdir="i";    pon="34hO" }; //
 { g_null_netport_pattern_schema with lname="AWUSER";   chan="AW";  width="0";       sdir="i";    pon="34hO" };


 { g_null_netport_pattern_schema with lname="WID";      chan="W";  width="ID_WIDTH";  sdir="i";   pon="3h" };   // No WID in AXI4 - use AWID instead.
 { g_null_netport_pattern_schema with lname="WLAST";    chan="W";  width="1";        sdir="i";    pon="34h" };
 { g_null_netport_pattern_schema with lname="WUSER";    chan="W";  width="0";        sdir="i";    pon="34hO" }; // Optional


 { g_null_netport_pattern_schema with lname="BID";     chan="B";  width="ID_WIDTH";   sdir="o";   pon="34h" };
 { g_null_netport_pattern_schema with lname="BUSER";   chan="B";  width="1";         sdir="o";    pon="34hO" }; // Optional


 { g_null_netport_pattern_schema with lname="ARID";    chan="AR";  width="ID_WIDTH";   sdir="i";  pon="34h" };
 { g_null_netport_pattern_schema with lname="ARLEN";   chan="AR";  width="8";        sdir="i";    pon="34h" }; // Burst length minus 1
 { g_null_netport_pattern_schema with lname="ARSIZE";  chan="AR";  width="3";        sdir="i";    pon="34h" }; // Transfers per beat: 1,2,8,...128. 010=4 for 32 but bus

 { g_null_netport_pattern_schema with lname="ARBURST"; chan="AR";  width="2";        sdir="i";    pon="34h" }; // 00=Fixed,01=Inc,10=Wrap
 { g_null_netport_pattern_schema with lname="ARLOCK";  chan="AR";  width="2";        sdir="i";    pon="3h" };  // Two-bit on AXI3 
 { g_null_netport_pattern_schema with lname="ARLOCK";  chan="AR";  width="1";        sdir="i";    pon="4h" };  // Set of load-linked
 { g_null_netport_pattern_schema with lname="ARCACHE"; chan="AR";  width="4";        sdir="i";    pon="34h" }
 { g_null_netport_pattern_schema with lname="ARREGION"; chan="AR"; width="4";        sdir="i";    pon="34hO" }; // Optional
 { g_null_netport_pattern_schema with lname="ARQOS";   chan="AR";  width="1";        sdir="i";    pon="34hO" }; // Optional
 { g_null_netport_pattern_schema with lname="ARUSER";  chan="AR";  width="0";        sdir="i";    pon="34hO" }; // Optional


 { g_null_netport_pattern_schema with lname="RID";     chan="R";  width="ID_WIDTH";    sdir="o";  pon="34h" };
 { g_null_netport_pattern_schema with lname="RLAST";   chan="R";  width="1";         sdir="o";    pon="34h" };
 { g_null_netport_pattern_schema with lname="RUSER";   chan="R";  width="0";         sdir="o";    pon="34hO" }; // Optional
 ]


let g_spirit_ns =
    {
        prefix=        "spirit"
        namespaceName= "http://www.spiritconsortium.org/XMLSchema/SPIRIT/1685-2009"
        // or   "http://www.accellera.org/XMLSchema/IPXACT/1685-2014"
    }

let g_hprls_ns =
    { prefix=         "hprls"
      namespaceName=  "http://www.hprls.com/XMLSchema/hprlsZero"
    }
    
let spiritElement (name, ats, contents) = XML_ELEM2(g_spirit_ns, name, ats, contents)

let spiritElement_o (name, ats, contents) =
    if nullp contents && nullp ats then [] else [spiritElement (name, ats, contents)]

(*//
//
//*)
let wrap5 f lst =
    if length lst <> 5 then sf "wrap5: not given five args" 
    else f(hd lst, hd(tl lst), hd(tl(tl lst)), hd(tl(tl(tl lst))), hd(tl(tl(tl(tl lst)))))


(*//
//
//*)
let wrap6 f lst =
    if length lst <> 6 then sf ("wrap6: not given six args: " + sfold xToStr lst) 
    else f(hd lst, hd(tl lst), hd(tl(tl lst)), hd(tl(tl(tl lst))), hd(tl(tl(tl(tl lst)))), hd(tl(tl(tl(tl(tl lst))))))

let wrap7 f lst =
    if length lst <> 7 then sf ("wrap7: not given seven args: " + sfold xToStr lst) 
    else f(hd lst, hd(tl lst), hd(tl(tl lst)),
           hd(tl(tl(tl lst))),
           hd(tl(tl(tl(tl lst)))),
           hd(tl(tl(tl(tl(tl lst))))),
           hd(tl(tl(tl(tl(tl(tl lst)))))))


//
// Compute factors of pack/unpack of data between or over word lanes ...
//
let gen_portItemPack msg0 laneWidth phys_port_width r_dwidth =
    let lanesPerPWord = phys_port_width / laneWidth
    let lanesPerUWord = r_dwidth/laneWidth        
    let packed = phys_port_width > r_dwidth
    let perfect = phys_port_width = r_dwidth    
    let packing_arity = lanesPerPWord / lanesPerUWord

    let msg() = (sprintf "gen_portItemPack:  %s packed=%A  laneWidth=%i  phys_port_width=%i  requested_data_width=%i  packing_arity=%i" msg0 packed laneWidth phys_port_width r_dwidth packing_arity)

    let decidex offset =  // Return number of bus cycles, or if packed, the lane mask needed. The offset arg is in units of one user word width.
        let mm = gec_X_bnum({widtho=Some lanesPerUWord; signed=Unsigned}, himask lanesPerUWord)
        let msg() = (sprintf " decidex   packed=%A r_dwidth=%i packing_arity=%i offset=%s" packed r_dwidth packing_arity (xToStr offset))
        //vprintln 0 (msg())
        let (lanes, cycles) =
                    if packed  then // packed: use selected lanes only.
                       (ix_lshift mm (ix_times (xi_num lanesPerUWord) offset), 1)
                    else (mm, (r_dwidth+phys_port_width-1)/phys_port_width) // All lanes
        (lanes, cycles)

    let word_xtract offset rd =
                if packed then
                    // TODO? could mask to users natural u_dwidth instead of power of 2 lanes.
                    ix_mask Unsigned r_dwidth (ix_rshift_both Unsigned rd (ix_times offset (xi_num(lanesPerUWord * laneWidth)))) 
                elif perfect then rd
                else muddy (sprintf "multicycle bondout port packing: " + msg())


    let replicate wd =
        let _ = vprintln 3 (sprintf "write needs %i replicated copies over the lanes" packing_arity)
        if packed then
            let wd = ix_mask Signed r_dwidth wd
            let copy_over_all_lanes cc n = ix_bitor cc (ix_lshift wd (xi_num (n * laneWidth * lanesPerUWord)))
            let ans = List.fold copy_over_all_lanes (xi_num 0) [0..packing_arity-1]
            //let _ = vprintln 0 (sprintf "replicated write word %s over %i lanes giving %s" (xToStr wd) packing_arity (xToStr ans))
            //let _ = if constantp ans then vprintln 0 (sprintf "Arg in hex is " + bigPrintHex (xi_manifest_int "replicate-wd" wd) +  " Answer in hex is " + bigPrintHex (xi_manifest_int "replicate" ans))
            ans
        else wd

            // For example: 32 bit uwords and 8 bit lanes need 4 lanes per uword. Copies is lanesPerPWord / lanesPerUWord 
    (lanesPerPWord, word_xtract, replicate, decidex, phys_port_width, packing_arity)



let gen_portItemPacking_bytewide xs = // An old routine.
    let laneWidth = 8
    dev_println (sprintf "lane width needed")
    match xs with
        | X_pair(X_num phys_port_width, X_num dwidth, _) -> gen_portItemPack "bytewide" laneWidth phys_port_width dwidth


(*
//
*)
let bvci_hpr_arrayop ww flip arg =
    let nom_out () = if flip then INPUT else OUTPUT
    let nom_inp () = if flip then OUTPUT else INPUT
    match arg with
    | (W_string(oOp, _, _), xs, W_string(id, _, _), X_pair(X_num addrSize, aAddr, _), dData, X_num layout_ratio) ->
        let xid = "bv_" + id
        let (lanesPerPWord, word_xtract, replicate, decx, pwidth, packing_arity) = gen_portItemPacking_bytewide xs

        let cmdval = ionet_w(xid +  "_" + "cmdval", 1, nom_out(), Unsigned, [])
        let cmdack = ionet_w(xid +  "_" + "cmdack", 1, nom_inp(), Unsigned, [])
        let plen   = ionet_w(xid +  "_" + "plen", 7, nom_out(), Unsigned, [])  (* No of bytes *)
        let cmd =    ionet_w(xid +  "_" + "cmd", 2, nom_out(), Unsigned, [])
        let eop =    ionet_w(xid +  "_" + "eop", 1, nom_out(), Unsigned, [])
        let wdata =  ionet_w(xid +  "_" + "wdata", pwidth, nom_out(), Unsigned, [])
        let addr =   ionet_w(xid +  "_" + "addr", addrSize, nom_out(), Unsigned, [])
        let be =     ionet_w(xid +  "_" + "be", pwidth / 8, nom_out(), Unsigned, [])
        let rspval = ionet_w(xid +  "_" + "rspval", 1, nom_inp(), Unsigned, [])
        let rspack = ionet_w(xid +  "_" + "rspack", 1, nom_out(), Unsigned, [])
        let reop =   ionet_w(xid +  "_" + "reop", 1, nom_inp(), Unsigned, [])
        let rerror = ionet_w(xid +  "_" + "rerror", 1, nom_inp(), Unsigned, [])
        let rdata =  ionet_w(xid +  "_" + "rdata", pwidth, nom_inp(), Unsigned, [])

        let aAddr2 = ix_divide aAddr (xi_num packing_arity)
        let offset = ix_mod aAddr (xi_num packing_arity)
        let (lanesData, cycles) = decx offset
        let _ = if cycles > 1 then muddy "BVCI multicycle"

        vprintln 3 ("Rehydrate BVCI arrayop " + id + " " + oOp)
        let nets = [ cmdack; cmdval; plen; cmd; eop; wdata; addr; be;
                             rspval; rspack; reop; rerror; rdata
                   ]
        if oOp<>"read" && oOp<>"write" then sf("Bad xtor canned bvci op=" + oOp)
            

        if cycles > 1 then muddy "BVCI multicycle"

        let xtor =
              [
                Xassign(rspack, xi_one);
                Xassign(cmd, xi_num((if oOp="read" then 1 else if oOp="write" then 2 else 0)));
                Xassign(addr, aAddr2);
                Xassign(cmdval, xi_one);
                (if oOp="write" then Xassign(wdata, replicate dData) else Xskip);
                Xassign(plen, xi_num cycles)
                Xassign(be, lanesData)
                Xassign(eop, xi_one);
                Xwaituntil(ix_and (xgen_orred cmdval) (xgen_orred cmdack));
                Xassign(cmdval, xi_zero);

                Xassign(eop, xi_zero); (* Don't need to deassert anything except for cmdval *)
                Xassign(addr, xi_zero);
                Xassign(wdata, xi_zero);

                Xwaituntil(ix_and (xgen_orred rspval) (xgen_orred rspack));
                (if oOp="read" then  Xassign(dData, word_xtract offset rdata) else Xskip);
              ] 
        (gec_Xblock xtor, nets) 

    | (_, _, _, _, _, _) -> sf (sprintf " bvci_hpr_arrayop: bad arg types in call %A" arg)



let hsimple_hpr_arrayop ww flip arg = // OLD do not use now
    let nom_out () = if flip then INPUT else OUTPUT
    let nom_inp () = if flip then OUTPUT else INPUT

    match arg with
    | (W_string(oOp, _, _), xs, W_string(id, _, _), X_pair(X_num addrSize, aAddr, _), data, X_num layout_ratio) ->
        let xid = "hs_" + id
        let io(ikey, logical_name, width, direction, ats) =
            ionet_w(ikey + "_" + logical_name, width, direction, Unsigned, Nap(g_logical_name, logical_name)::ats)

        let (lanesPerPWord, word_xtract, replicate, decx, pwidth, packing_arity) = gen_portItemPacking_bytewide xs
        let lanes_net = if lanesPerPWord <= 1 then [] else [ io(xid, "lanes", lanesPerPWord, nom_out(), []) ]
        let req    = io(xid, "REQ", 1, nom_out(), [])
        let ack    = io(xid, "ACK", 1, nom_inp(), [])
        let rwbar  = io(xid, "RWBAR", 1, nom_out(), [])
        let wdata =  io(xid, "WDATA", pwidth, nom_out(),  [Nap(g_ip_xact_is_Data_tag, "true")])
        let addr =   io(xid, "ADDR", addrSize, nom_out(),  [Nap(g_ip_xact_is_Address_tag, "true")])
        let rdata =  io(xid, "RDATA", pwidth, nom_inp(),  [Nap(g_ip_xact_is_Data_tag, "true")])

        let aAddr2 = ix_divide aAddr (xi_num packing_arity)
        let offset = ix_mod aAddr    (xi_num packing_arity)
        let (lanesData, cycles) = decx offset
        let _ = if cycles > 1 then muddy "HSIMPLE multicycle"

        let _ = vprintln 3 ("HSIMPLE Rehydrate arrayop " + id + " " + oOp)
        let nets = [ req; ack; rwbar; wdata; addr; rdata; ] @ lanes_net 
        let _ = if oOp<>"read" && oOp<>"write" then sf("Bad xtor canned arrayop=" + oOp)

        let xtor =
         (if nullp lanes_net then [] else [ Xassign(hd lanes_net, lanesData)]) @
         [
             
             Xassign(rwbar, xi_num((if oOp="read" then 1 else if oOp="write" then 0 else 0)));
             Xassign(addr, aAddr2);
             Xassign(req, xi_one);
             (if oOp="write" then Xassign(wdata, replicate data) else Xskip);
             Xwaituntil(xgen_orred ack);
             (if oOp="read" then  Xassign(data, word_xtract offset rdata) else Xskip);
             Xassign(req, xi_zero);
             Xassign(addr, xi_zero);

             Xwaituntil(xi_not(xgen_orred ack));
         ] 
        (gec_Xblock xtor, nets) 

    | (_, _, _, _, _, _) -> sf (sprintf "HSIMPLE hpr_arrayop: bad arg types in call %A" arg)

//
// An AXI port has 5 so-called channels (unless streaming which has one).
// Those 5 channels should be described as one spirit:businterface without further IP-XACT hierarchy.
let gen_axi_nets ww duplex flip latency (port:bondout_port_t) =
    let ikey = sprintf "hf%i_%s" latency port.ls_key
    let nom_out () = if flip then INPUT else OUTPUT
    let nom_inp () = if flip then OUTPUT else INPUT
    let port_instance_name = if port.ls_key = "" then [] else [ Nap(g_port_instance_name, port.ls_key) ]
    let datasize = port.dims.no_lanes * port.dims.laneWidth
    let io(ikey, logical_name, width, direction, ats) =
        ionet_w(ikey + "_" + logical_name, width, direction, Unsigned, port_instance_name @ Nap(g_logical_name, logical_name)::ats)

    let ip_xact_parameters =
        [
            [("name", "PROTOCOL");    ("value", "AXI4LITE")]
            [("name", "ADDR_WIDTH");  ("value", i2s port.dims.addrSize)]
            [("name", "DATA_WIDTH");  ("value", i2s datasize)]
        ]

    let axi_rd_addr_channel() = 
       {
          port=     port
          araddr=   io(ikey, "ARADDR",  port.dims.addrSize, nom_out(), [Nap(g_ip_xact_is_Address_tag, "true")])          
          arprot=   io(ikey, "ARPROT",  3,                  nom_out(), [])
          arready=  io(ikey, "ARREADY", 1,                  nom_inp(), [])
          arvalid=  io(ikey, "ARVALID", 1,                  nom_out(), [])
       }

    let axi_rd_addr_channel_nets(arg:axi_rd_addr_channel_t) =
        let cpi = { g_null_db_metainfo with kind= "axi-rd-addr-channel"; pi_name=port.ls_key; not_to_be_trimmed=true  } 
        [DB_group(cpi, map db_netwrap_null [ arg.araddr; arg.arprot; arg.arready; arg.arvalid ])]

    let axi_rd_data_channel() = 
       {
          port=     port
          rdata=    io(ikey, "RDATA",  datasize,                                 nom_inp(), [Nap(g_ip_xact_is_Data_tag, "true")])
          rready=   io(ikey, "RREADY", 1,                                        nom_out(), [])
          rresp=    io(ikey, "RRESP",  3,                                        nom_inp(), [])
          rvalid=   io(ikey, "RVALID", 1,                                        nom_inp(), [])          
       }
    let axi_rd_data_channel_nets(arg:axi_rd_data_channel_t) =
        let cpi = { g_null_db_metainfo with kind= "axi-rd-data-channel";  pi_name=port.ls_key; not_to_be_trimmed=true  }
        [DB_group(cpi, map db_netwrap_null [ arg.rdata; arg.rready; arg.rresp; arg.rvalid ])]

    let axi_wr_addr_channel() = 
       {
          port=    port
          awaddr=  io(ikey, "AADDR",  port.dims.addrSize, nom_out(), [Nap(g_ip_xact_is_Data_tag, "true")])
          awprot=  io(ikey, "AWPROT",  3,                 nom_out(), [])
          awready= io(ikey, "AWREADY", 1,                 nom_inp(), [])
          awvalid= io(ikey, "AWVALID", 1,                 nom_out(), [])

       }
    let axi_wr_addr_channel_nets(arg:axi_wr_addr_channel_t) =
        let cpi = { g_null_db_metainfo with kind= "axi-wr-addr-channel";  pi_name=port.ls_key; not_to_be_trimmed=true  }
        [DB_group(cpi, map db_netwrap_null [ arg.awaddr; arg.awprot; arg.awready; arg.awvalid ])]

    let axi_wr_resp_channel() = 
       {
          port=    port
          bready=  io(ikey, "BREADY",   1,           nom_out(), [])
          bresp=   io(ikey, "BRESP",    3,           nom_inp(), [])
          bvalid=  io(ikey, "BVALID",   1,           nom_inp(), [])

       }
    let axi_wr_resp_channel_nets(arg:axi_wr_resp_channel_t) =
        let cpi = { g_null_db_metainfo with kind= "axi-wr-resp-channel";  pi_name=port.ls_key; not_to_be_trimmed=true  }
        [DB_group(cpi, map db_netwrap_null [ arg.bready; arg.bresp;  arg.bvalid ])]

    let axi_wr_data_channel()=  
       {
          port=    port
          wdata=   io(ikey, "WDATA",   datasize,                                                          nom_out(), [Nap(g_ip_xact_is_Data_tag, "true")])
          wready=  io(ikey, "WREADY",  1,                                                                 nom_inp(), [])
          wstrb=   if port.dims.no_lanes < 2 then None else Some(io(ikey, "WSTRB", port.dims.no_lanes,    nom_out(), []))
          wvalid=  io(ikey, "WVALID",  1,                                                                 nom_out(), [])
       }
    let axi_wr_data_channel_nets (arg:axi_wr_data_channel_t) =
        let cpi = { g_null_db_metainfo with kind= "axi-wr-data-channel";  pi_name=port.ls_key; not_to_be_trimmed=true }
        [DB_group(cpi, map db_netwrap_null ([ arg.wdata; arg.wready; arg.wvalid ] @ (if nonep arg.wstrb then [] else [valOf arg.wstrb])))]
    
    let rd_channel() =
        {
            raddr=   axi_rd_addr_channel()
            resp=    axi_rd_data_channel()
        }
    let axi_rd_channel_nets = function
        | None -> []
        | Some (arg:axi_rd_channel_t) ->
            let cpi = { g_null_db_metainfo with kind= "axi-rd-channel";  pi_name=port.ls_key; not_to_be_trimmed=true} 
            [DB_group(cpi, axi_rd_addr_channel_nets arg.raddr @ axi_rd_data_channel_nets arg.resp)]
            

    let wr_channel() =
        {
            waddr=  axi_wr_addr_channel()            
            resp=   axi_wr_resp_channel()
            wdata=  axi_wr_data_channel()
        }
    let axi_wr_channel_nets = function
        | None -> []
        | Some (arg:axi_wr_channel_t) -> axi_wr_addr_channel_nets arg.waddr @ axi_wr_data_channel_nets arg.wdata @ axi_wr_resp_channel_nets arg.resp
    
    let netrec =
        {
            ikey=ikey
            axi_rd_channel=(if duplex=PD_storeport then None else Some (rd_channel()))
            axi_wr_channel=(if duplex=PD_loadport then None else Some (wr_channel()))
        }

    //let lanes_net = if port.no_lanes <= 1 then [] else [ valOf netrec.lanes ]
    let nets_list = axi_wr_channel_nets netrec.axi_wr_channel @ axi_rd_channel_nets netrec.axi_rd_channel
    (netrec, nets_list, XACT_prams ip_xact_parameters)

let not_undef_pred = function
    | X_undef -> false
    | _       -> true








(*
let hfast_hpr_port_gen port latency =
//    match arg with
//    | (W_string(oOp, _, _), xs, W_string(id, _, _), X_pair(X_num addrSize, aAddr, _), data, X_num layout_ratio) ->

    let nets = gen_hfast_nets ikey port
    let _ = vprintln 3 ("Rehydrate HFAST arrayop " + ikey)
    let (lanesPerPWord, word_xtract, replicate, decx, pwidth, packing_arity) = gen_portItemPacking_bytewide xs


    let xtor = []
    //let _ = if oOp <> "netsonly" then sf "xtor use for HFAST not supported - restructure2 should make pipelined requests and stall its static schedule instead"
    (port, nets) 
*)



let canned_hpr_arrayop ww arg = // OLD do not use
    match arg with
    | (W_string(s, _, _), a, b, c, d, e, layout) -> 
        if s="HSIMPLE" then hsimple_hpr_arrayop ww false (a, b, c, d, e, layout)
        //elif s="HFAST1" then hfast_hpr_arrayop 1 (a, b, c, d, e, layout)
        elif s="BVCI" then bvci_hpr_arrayop ww false (a, b, c, d, e, layout)
        else sf("unknown protocol for offchip array op: " + s)

    | (_, _, a, b, c, d, e) -> sf "canned_hpr_arrayop other"


let wrapped_hpr_arrayop ww () = wrap7(canned_hpr_arrayop ww)  // OLD do not use

let vlnv_protocol_ats kind version =
    [  ("vendor", "HPRLS");  ("library", "protocols") ; ("name", kind);   ("version", version) ]

let vlnvToXml vlnv =
    [  ("vendor", vlnv.vendor);  ("library", vlnv.library) ; ("name", hptos vlnv.kind);   ("version", vlnv.version) ]

let name_wrap name = spiritElement("name", [], [ XML_ATOM name ])
let value_wrap name = spiritElement("value", [], [ XML_ATOM name ])

let ip_xact_out ww compname filename x_ip_xact =
    let filename = filename + g_ip_xact_file_suffix
    //let filename = System.IO.Path.Combine(!g_log_dir, filename)
    let ww = WF 2 ("Writing IP-XACT version of " + compname + " for file " + filename) ww "serialise"
    let _ = hpr_xmlout ww filename true x_ip_xact
    let _ = WF 2 ("Wrote IP-XACT version of " + compname + " to file " + filename) ww "complete"
    ()

let fileWrap ww filename =
    let x_name = spiritElement("name", [], [XML_ATOM filename])
    spiritElement("fileSet", [], [x_name]) 

let deboiler (name, value, _, _) = XML_ELEMENT(name, [], [XML_ATOM(value)])


// Write out a bus definition.  This corresponds with a port type on a component and will be acompanied with an RTL or TLM busAbstraction file.
// We have one bus definition for each invokable method on a component and the nets (apart from clock/reset/cen(perhaps)) are disjoint for each one.
// cen is for further study.
let render_ip_xact_export_busDefinition ww fn_prefix bus_name netlevel_(*aka TLM/RTL*) ports parameters method_name__ (expected_latency, reinit_latency, fsems) =

    let ww = WF 3 "ip_xact_export_busDefinition" ww (sprintf "fn_prefix=%s bus_name=%s method_name=%s" fn_prefix bus_name method_name__)

    let top_ats = []
    
    let boilerplate_preamble =
        [
            ("vendor", "custom", 3, 4) // 4-part VLNV identifier
            ("library", "nolib", 3, 4)
            ("name", bus_name, 3, 4)
            ("version", "1.0", 3, 4)
        ]

    let x_directConnection = spiritElement_o("directConnection", [], [XML_ATOM "true" ])
    let x_broadcast = spiritElement_o("broadcast", [], [XML_ATOM "false" ])

    // here would be isAddressable
    // here would be extends
    let x_maxMasters        = spiritElement_o("maxMasters", [], [XML_ATOM "1" ])
    let x_maxSlaves        = spiritElement_o("maxSlaves", [], [XML_ATOM "1" ])    
    // here would be systemGroupNames
    // here would be description   
    let x_parameters =
        let gec_pram (key, vale) = spiritElement("parameter", [], [ name_wrap key; value_wrap vale ])
        if nullp parameters then [] else [spiritElement("parameters", [], map gec_pram parameters)]
    
    // here would be assertions  

    let fsem_ats = meta_fsem_ats "ip_xact_export_busDefinition" fsems // Recall that the abstraction is essentially an extension of the definition (more concrete infact) so these attributes go here in the definition and not in any (so-called) abstraction.
    let x_vendorExtensions =
        let x_lat1 = XML_ELEM2(g_hprls_ns, "expected-latency", [], [ XML_ATOM(sprintf "%i" expected_latency)])
        let x_lat2 = XML_ELEM2(g_hprls_ns, "reinit-interval", [], [ XML_ATOM(sprintf "%i" reinit_latency)])
        let  lift_bool_attribute (key, vale) = XML_ELEM2(g_hprls_ns, key, [], [ XML_ATOM vale])               
        spiritElement_o("vendorExtensions", [], map lift_bool_attribute fsem_ats @ [  x_lat1; x_lat2; ])

    let x_ip_xact = spiritElement("busDefinition", top_ats, (map deboiler boilerplate_preamble) @ x_directConnection @ x_broadcast @ x_maxMasters @ x_maxSlaves @ x_parameters @ x_vendorExtensions)
    let filename = fn_prefix + fn_sanitize bus_name
    let msg = (sprintf "Write IP-XACT busDefinition for %s to %s" bus_name filename)
    aux_report_log 2 "IP-XACT input/output" [msg]
    ip_xact_out ww bus_name filename x_ip_xact
    () // end of render_ip_xact_export_busDefinition



    
// Write out an busAbstraction: or in other words, a bus specification.  Typically a custom one for intercomponent connections in an incremental design.
// abstractionDefinition->ports->port->transactional instead of spirit:wire inside spirit:port
// We can write the abstraction both trabnsactionally and net-level with different suffixes.
let render_ip_xact_export_busAbstraction ww fn_prefix abstraction_name netlevel(*aka TLM/RTL*) ports parameters method_name =  

    let ww = WF 3 "ip_xact_export_abstractionDefinition" ww (sprintf "name=%s tlm_mode=%A" abstraction_name netlevel)
    let top_ats = []
    
    let boilerplate_preamble =
        [
            ("vendor", "custom", 3, 4) // 4-part VLNV identifier
            ("library", "nolib", 3, 4)
            ("name", abstraction_name, 3, 4)
            ("version", "1.0", 3, 4)
        ]

    // busType
    let x_busType = spiritElement_o("busType", [], [XML_ATOM method_name])
    // here would be extends
        
    let gport (out_on_init, b, prec, width, name) =
        let x_logicalName = spiritElement_o("logicalName", [], [XML_ATOM name])
        let x_displayName = spiritElement_o("displayName", [], [XML_ATOM name])        

        // next would come description
        
        let nl_item width xnet_io =
            //let x_name = spiritElement_o("name", [], [XML_ATOM name])        
            let x_qualifier = [] // isData, isAddress etc..
            // presence: onSystem onMaster onSlave
            // defaultValue 
            // requiresDriver defaultValue
            let df pol = if pol<>out_on_init then "input" else "output"
            let x_width  = spiritElement("bitWidth",  [], [XML_ATOM(i2s width)])
            let dir c = spiritElement("direction", [], [XML_ATOM(df c)])         // in out inout or phantom

            // Expect this in a wire: <spirit:driver> <spirit:defaultValue>0x0F</spirit:defaultValue> </spirit:driver>
            let x_master_props = spiritElement_o("onMaster", [], [ x_width; dir true]) 
            let x_slave_props  = spiritElement_o("onSlave",  [], [ x_width; dir false])
            x_qualifier @ x_master_props @ x_slave_props

        let tr_item requires _ =
            // presence: onSystem onMaster onSlave
            // qualifer=isData or isAddress only - too low level for our composite ports
            // transactional
            // protocol
            //
            let initiative = spiritElement_o("initiative", [], [ XML_ATOM (if requires then "requires" else "provides") ])
            initiative @ []

        let nl = if netlevel <> "TLM" then spiritElement_o("wire", [], nl_item width b) else []
        let tl = if netlevel <> "NET" then spiritElement_o("transactional", [], tr_item true b) else []                
        let x_vendorExtensions = []
        spiritElement("port", [], x_logicalName @ x_displayName @ nl @ tl @ x_vendorExtensions)

    let x_ports = spiritElement_o("ports", [], map gport ports)
    // here would be description

    let x_prams =
        let gec_pram (key, vale) = spiritElement("parameter", [], [ name_wrap key; value_wrap vale ])
        if nullp parameters then [] else [spiritElement("parameters", [], map gec_pram parameters)]
    // here would be assertions

    // here are our extensisons
    let x_vendorExtensions = [] 

    let x_ip_xact = spiritElement("abstractionDefinition", top_ats, (map deboiler boilerplate_preamble) @ x_busType  @ x_ports @ x_prams @ x_vendorExtensions)
    let filename = fn_prefix + fn_sanitize abstraction_name
    let msg = (sprintf "Write IP-XACT abstractionDefinition for %s to %s" abstraction_name filename)
    aux_report_log 2 "IP-XACT input/output" [msg]
    ip_xact_out ww abstraction_name filename x_ip_xact
    ()


//
// Write a component definition as an IP-XACT document.
// You may manually validate the output with xmllint --noout --schema ~/Dropbox/ip-xact/component.xsd foo.xml
// 
let ip_xact_export_component ww compname parameters files ports (area, reported_heapuses) = 

// eisf, expected_latency, reinit_latency, inhold, outhold

    let msg = (sprintf "Write IP-XACT component %s" compname)
    let ww = WF 3 "ip_xact_export_component" ww msg

    let top_ats =
        [
          //("xmlns:spirit",              g_spirit_ns.namespaceName) // These should now be issued instead via hpr_xml
          //("xmlns:xsi",                "http://www.w3.org/2001/XMLSchema-instance")
          ]       

    let boilerplate_preamble =
        [
            ("vendor", "HPRLS", 3, 4) // 4-part VLNV identifier
            ("library", "nolib", 3, 4)
            ("name", compname, 3, 4)
            ("version", "1.0", 3, 4)
        ]

    let x_prams =
        let gec_pram (key, vale) = spiritElement("parameter", [], [ name_wrap key; value_wrap vale ])
        if nullp parameters then [] else [spiritElement("parameters", [], map gec_pram parameters)]
      

    let gec_portmap = function // Map net-level name to formal name in the corresponding bus definition.
        | DB_group(_, signals) ->
            //dev_println (sprintf " Writing component %s portMap %i signals " compname (length signals))
            let gec_pair (DB_leaf(fo_, Some(X_bnet ff))) cc =
                let f2 = lookup_net2 ff.n
                match at_assoc g_logical_name f2.ats with
                    | Some logical_name ->
                        spiritElement("portMap", [],
                                    [ spiritElement("logicalPort", [], [name_wrap logical_name])
                                      spiritElement("physicalPort", [], [name_wrap (vsanitize ff.id)])
                                    ])::cc
                    | None ->
                        vprintln 1 (sprintf "+++ IP-XACT logical name missing for %s. But fo=%A" ff.id fo_)
                        cc
            let maps = List.foldBack gec_pair signals []
            if nullp maps then [] else [spiritElement("portMaps", [], maps)]
        | _ -> sf "L618"
        
    let spirit_signal(DB_leaf(_, Some(X_bnet ff))) =
        //let f2 = lookup_net2 ff.n
        //let width  = spiritElement("bitWidth",  [], [XML_ATOM(i2s ff.width)])
        //let dir    = spiritElement("direction", [], [XML_ATOM d])         // in out inout or phantom
        // isClock isData isReset isAddress - only on abstractionDefinition?
        spiritElement("signal", [], [ name_wrap (vsanitize ff.id) ])

//{http://www.accellera.org/XMLSchema/IPXACT/1685-2014}signal': This
//element is not expected. Expected is one of ( connectionRequired,
//bitsInLau, bitSteering, endianness, parameters or vendorExtensions ).

    let busInterfaceWrap port cc =
        match port with
            | DB_group(meta, nets) when meta.pi_name=g_null_db_metainfo.pi_name -> cc // Anonymous - do not report it. (perhaps local vars)
            | DB_group(meta, nets) when meta.form = DB_form_local -> cc // Do not report local busses by default
            | DB_group(meta, nets) ->
                let x_nets__ = map spirit_signal nets
                let x_pop = XML_ELEM2(g_hprls_ns, "purpose", [], [ XML_ATOM meta.purpose ])
                // 
                // let nameGroup = spiritElement("nameGroup", [], [ ... ]) // For multiple instances of the same bus, we give each an instance name, like the system_integrator.dp_name data convservation ... for use in HPR System Integrator for the domain/group name
                let x_bus_extensions = [ spiritElement("vendorExtensions", [], [ x_pop ]) ]                
                let busType = spiritElement("busType", vlnv_protocol_ats (meta.kind + "") meta.version, [])
                let absType = spiritElement("abstractionType", vlnv_protocol_ats (meta.kind + "_rtl") meta.version, [])

                let mode = spiritElement("master", [], []) // This should be an atom inside and interfaceMode? No it should perhaps contain <spirit:addressSpaceRef spirit:addressSpaceRef="main"/> and the slave variant should have a binding group 
                let cr = true
                let x_cr = spiritElement("connectionRequired", [],[ XML_ATOM(if cr then "true" else "false")])
                // Expected is one of ( {http://www.accellera.org/XMLSchema/IPXACT/1685-2014}abstractionTypes, 
                //     master,    slave,    system,    mirroredSlave,    mirroredMaster,  mirroredSystem or  monitor.
                let x_prams =
                    let gec_pram (key, vale) = spiritElement("parameter", [], [ name_wrap key; value_wrap vale ])
                    if nullp meta.metaprams then [] else [spiritElement("parameters", [], map gec_pram meta.metaprams)]
                                        
                let x_portmap = gec_portmap port
                spiritElement("busInterface", [], [name_wrap meta.pi_name; busType; absType; mode; x_cr] @ x_prams @ (* x_nets @ *) x_portmap @ x_bus_extensions) :: cc
            | _ -> sf "L612"
            
       // http://www.accellera.org/XMLSchema/IPXACT/1685-2014}portMaps:
       //    indirectInterfaces,   channels, remapStates, addressSpaces, memoryMaps, model,
       //    componentGenerators, choices, fileSets or whiteboxElements.


    let x_busInterfaces =
        let _ = vprintln 2 (sprintf "%i busInterfaces to render" (length ports))
        let pl = List.foldBack busInterfaceWrap ports []
        vprintln 2 (sprintf "%i busInterfaces finally" (length pl))
        if nullp pl then [] else [spiritElement("busInterfaces", [], pl)]
    
    let x_filesets = if nullp files then [] else [spiritElement("fileSets", [], map (fileWrap ww) files)]

    let x_heapspaces =
        let rh (spacename, (usebytes:int64)) =
            let n = name_wrap spacename
            let v = value_wrap(sprintf "%i" usebytes)
            XML_ELEM2(g_hprls_ns, "heapspace", [], [ n; v ])
        XML_ELEM2(g_hprls_ns, "heapspaces", [], map rh reported_heapuses)

    let x_area = XML_ELEM2(g_hprls_ns, "area", [], [ XML_ATOM(sprintf "%i" area)])
    let x_extensions = [ spiritElement("vendorExtensions", [], [ x_area; x_heapspaces ]) ]

    let x_ip_xact = spiritElement("component", top_ats, (map deboiler boilerplate_preamble) @ x_prams @ x_busInterfaces @ x_filesets @ x_extensions)
    let filename = fn_sanitize compname
    let msg = (sprintf "Write IP-XACT component file for %s to %s" compname filename)
    aux_report_log 2 "IP-XACT input/output" [msg]
    ip_xact_out ww compname filename x_ip_xact // End of component XML write.
    ()

type tmep_t =
    {
        iiname: string   // Name of component
        busref: string   // Name of port on component aka pi_name or mg_key
    }

let g_null_tmep =
    {
        busref= "*unset-busref*"
        iiname= "*unset-iiname*"
    }

// Validate the output with xmllint --noout --schema ~/Dropbox/ip-xact/design.xsd beanHPR\ test0.xml
let ip_xact_export_design ww desname parameters files sons lst = 
    let ww = WF 3 "ip_xact_export_design" ww ("name=" + desname)
    let x_prams =
        let gec_pram (key, vale) = spiritElement("parameter", [], [ name_wrap key; value_wrap vale ])
        if nullp parameters then [] else [spiritElement("parameters", [], map gec_pram parameters)]
    let top_ats = []
    
    let boilerplate_preamble =
        [
            ("vendor", "HPRLS", 3, 4) // 4-part VLNV identifier - rez should be lifted out of here
            ("library", "nolib", 3, 4)
            ("name", desname, 3, 4)
            ("version", "1.0", 3, 4)
        ]

    let deboiler (name, value, _, _) = XML_ELEMENT(name, [], [XML_ATOM(value)])

    let gec_instance ii =

        let items =
            [
                spiritElement("instanceName", [], [XML_ATOM ii.iname])
                spiritElement("componentRef", vlnvToXml ii.vlnv, [])
                spiritElement("configureableElementValues", [], [])                 
            ]
        spiritElement("componentInstance", [], items)

    let x_interconnections = 
        let gec_interconnection (name, e0, e1) =
            let name = name_wrap name
            let ep0 = spiritElement("activeInterface", [ ("componentRef", e0.iiname); ("busRef", e0.busref) ], [])
            let ep1 = spiritElement("activeInterface", [ ("componentRef", e1.iiname); ("busRef", e1.busref) ], [])
            spiritElement("interconnection", [], [name; ep0; ep1 ])
        spiritElement_o("interconnections", [], map gec_interconnection lst)
    let x_component_instances = spiritElement_o("componentInstances", [], map gec_instance sons) 
    let x_filesets = if nullp files then [] else [spiritElement("fileSets", [], map (fileWrap ww) files)]
    let x_ip_xact = spiritElement("design", top_ats, (map deboiler boilerplate_preamble) @ x_prams @ x_component_instances @ x_interconnections @ x_filesets)
    let filename = fn_sanitize desname // or fn_sanitize even
    let _ = ip_xact_out ww desname filename x_ip_xact // same function called for designs and files at the moment - need to refine
    ()


// Find port_structure entry for a given logical name.
//
let meld_assoc portnet_logical_name pattern_lst =
    let matcher cc (pi_name, formal_name, sp) = if sp.lname = portnet_logical_name then Some(pi_name, formal_name, sp) else cc
    List.fold matcher None pattern_lst





// Construct one net of a port following the plans in the (IP-XACT exportable) schema
// Used both for child actuals in restructure and for formals to definition synthesis runs (which contains child actuals in I/O form for externally instantiated children).
// The pi_name, when not the empty string, is prefixed to the logical name of a formal, except when pi_name is an integer when we instead suffix the integer on the logical name.
let buildportnet ww vd flip kind ikey portmeta pi_name formal_name (data_prec:precision_t option) nps cc =
    let fu_instance_name = if ikey = "" then [] else [ Nap(g_fu_instance_name, ikey) ]
    let port_instance_name = if pi_name = "" then [] else [ Nap(g_port_instance_name, pi_name) ]
    let formal_name1 = if formal_name = "" then nps.lname else formal_name
    let formal_name1 =
        //if at_assoc (fst g_no_decl_alias) ff.ats = Some "true" then cc  <- code missing ? why
        if pi_name = "" then formal_name1
        elif isDigit pi_name.[0] then formal_name1 + pi_name
        else pi_name + "_" + formal_name1
    let id = if ikey = "" then formal_name1 else  ikey + "_" + formal_name1
    //dev_println (sprintf "name composite %s  ++  %s    %s" ikey nps.lname    id)
    // We refine the precision of the data bus since it helps with $display style debugging of the final RTL.
    let (width, signed) =  if op_assoc g_ip_xact_is_Data_tag nps.ats = Some "true" && not_nonep data_prec then (i2s(valOf_or (valOf data_prec).widtho 32), (valOf data_prec).signed) else (nps.width, nps.signed)
    let width = 
        port_eval_int ikey kind portmeta width
    if width < 0L then sf (sprintf "build_port: bad bus width '%s' for %s of kind %s" nps.width id kind)
    if width = 0L then cc
    else
        let width = int32 width
        let io =
            match nps.sdir with
                | "x" -> INPUT 
                | "o" -> if nonep flip then LOCAL else if valOf flip then INPUT else OUTPUT
                | "i" -> if nonep flip then LOCAL else if valOf flip then OUTPUT else INPUT
                | _ -> sf "other mdir"

        let ats = port_instance_name @ fu_instance_name @ [Nap(g_logical_name, nps.lname)] @ (map (fun (k,v) -> Nap(k, v)) nps.ats)
        let hexp = ionet_w(id, width, io, signed, ats)
        if vd >= 5 then vprintln 5 (sprintf "buildportnet: flip=%A ikey=%A kind=%A width=%i pi_name=%s rezzed port net " flip ikey kind width pi_name + netToStr hexp)
        (formal_name1, hexp)::cc


type phys_methods_t = (string * string option * int) * (native_fun_signature_t * (string * string * string) option * vlat_t option * (precision_t * (string * string * string)) list)

type external_ip_block_record_t = // An AFU or FU - a slightly elaborated version of the block_meld.
    {
        ip_vlnv:    ip_xact_vlnv_t
        kind:       string // as per vlnv
                    //name  squirrel  arity   gis                      if_iname  ret lname ret formal_
        //typical_latency__: int// Put in meld? A crude illustrative latency for initial shedullng for all methods, regardless of mgr.
        //typical_rei__: int // Put in meld? A crude illustrative II for initial shedullng for all operations, regardless of mgr.        
        phys_methods:    phys_methods_t list
        phys_area:       double option
        meld:            block_meld_abstraction_t
        meta:            (string * string) list // Perhaps use meld.parameters (aka portmeta) directly since this is a copy?
        phys_structure__:  (string * string * netport_pattern_schema_t) list // Formal pi_name, formal net name and basic nps pattern.  

    }

let g_external_ip_blocks_database = new OptionStore<string list, external_ip_block_record_t * block_meld_abstraction_t * phys_methods_t list * bool option>("external_ip_blocks_database")


let readin_vlnv msg xml =
    let msg = msg + " vlnv read "
    { vendor=   xml_once_atom msg ((*"spirit:" + *) "vendor")  None xml
      library=  xml_once_atom msg ((*"spirit:" + *) "library") None xml
      kind=     [ xml_once_atom msg ((*"spirit:" + *) "name")    None xml ]
      version=  xml_once_atom msg ((*"spirit:" + *) "version") None xml
    }



// Each interface needs two IP-XACT files to describe it : a busDefinition and a abstractionDefinition
// This routine is the definition reader, which is the upper-level of the pair.
let retrieve_busDefinition ww busType =
    let vd = 3
    let msg = "IP-XACT read of busDefinition " + hptos busType
    aux_report_log 2 "IP-XACT input/output" [msg]

    let readin_busdef_vendor_extension _ xml =
        let fsems =  // The user-attributed properties override the default set.
            let collect_fsem_attribute fsems key =
                let vale =  xml_once_bool msg ("hprls:" + key) (Some false) xml // g_hprls_xml_namespace = "hprls:"
                vprintln 3 (sprintf "collected fsem protocol attribute '%s' with '%A'" key vale)
                let vale = if vale then "true" else "false"
                fsems_folder msg key vale fsems
            List.fold collect_fsem_attribute g_default_fsems g_foldable_fsem_attribute_names 

        let expected_latency = xml_once_int_option msg ("hprls:" + "expected-latency")  xml 
        let reinit_interval = xml_once_int_option msg ("hprls:" + "reinit-interval")  xml 
        (fsems, expected_latency, reinit_interval)


    match g_external_bus_definitions_database.lookup (hptos busType) with 
        | Some ov -> ov
        | None ->
            let path = sfold_colons !g_ip_incdir 
            match pathopen_any path vd [';'; ':'] (candidate_filenames busType) g_ip_xact_file_suffix with
                | (None, places_looked) ->
                    if path = "" then hpr_yikes(sprintf "IP Block search path was empty. Please set one with -ip-incdir=a:b:c (or semicolon on Windows)")
                    cleanexit(sprintf "L1182: Failed trying to load a missing busAbstraction IP-XACT document: search path=%s filename=%s suffix=%s. Places looked=%s " path (hptos busType) g_ip_xact_file_suffix (sfoldcr_lite id places_looked))

                | (Some filename, _) ->
                    let msg = sprintf "Read IP-XACT busDefinition %s" filename
                    let ww = WF 3 "retrieve/read_in busDefinition" ww msg
                    let xml = hpr_xmlin ww filename
                    
                    let readin_ipxact_busDefinition ats_ xml =
                        let vlnv =              readin_vlnv msg xml 
                        let directConnection =  xml_once_bool msg ("spirit:" + "directConnection") None xml
                        let broadcast =         xml_once_bool msg ("spirit:" + "broadcast")  None xml
(* Extends example:
          <spirit:extends spirit:vendor="amba.com" spirit:library="AMBA" spirit:name="AHBlite" spirit:name=”v1.0” /> *)
                        let extends =           xml_once_atom_option msg ("spirit:" + "extends") xml
                        let maxMasters =        xml_once_int msg ("spirit:" + "maxMasters") None xml
                        let maxSlaves =         xml_once_int_option msg ("spirit:" + "maxSlaves") xml



                        let (fsems, expected_latency, reinit_interval) = xml_once msg ("spirit:" + "vendorExtensions") readin_busdef_vendor_extension (Some(g_default_fsems, None, None)) xml

                        (vlnv, (directConnection, broadcast, maxMasters, maxSlaves, fsems))

                    let (vlnv, (directConnection, broadcast, maxMasters, maxSlaves, fsems)) = xml_once msg ("spirit:" + "busDefinition") readin_ipxact_busDefinition None [xml]

                    let portmeta = [] // for now

                    let busDef_meld:proto_meld_busDefinition_t =
                        { vlvn=              vlnv
                          directConnection=  directConnection
                          broadcast=         broadcast
                          maxMasters=        (int)maxMasters
                          maxSlaves=         maxSlaves
                          parameters=        portmeta
                          meld_fsems0=       fsems
                        }
                    g_external_bus_definitions_database.add (hptos busType) busDef_meld
                    busDef_meld
                    // end of retrieve_busDefinition

// Each interface needs two IP-XACT files to describe it : a busDefinition and a abstractionDefinition
// This routine is the abstraction reader, which is the lower-level of the pair.
// The name of this lower member, is normally the upper name with either "_tlm" or "_rtl" appended.
//This returns a 'proto_mgr_meld_abstraction_t' which is a pair where the second item is the proto_meld for the mgr_meld.

let retrieve_abstractionDefinition ww definitionMeld busType =
    let vd = 3
    let msg = "IP-XACT read of busAbstraction " + hptos busType
    aux_report_log 2 "IP-XACT input/output" [msg]
    // These canned matches no longer needed since the canned items are inserted in the database on creation.
    //if busType = "CV_SSRAM1" then g_cv_ssram_port_structure 
    //if   busType = "HFAST1_S"  then g_hfast1_meld "S"   // TODO insert in db and lookup
    //elif busType = "HFAST1_M"  then g_hfast1_meld "M"   // These are canned - also the simplex versions please.
    //else
    match g_external_bus_abstractions_database1.lookup (hptos busType) with
        | Some ov -> ov
        | None ->
            let path = sfold_colons !g_ip_incdir 
            match pathopen_any path vd [';'; ':'] (candidate_filenames busType) g_ip_xact_file_suffix with
                | (None, places_looked) ->
                    if path = "" then hpr_yikes(sprintf "IP Block search path was empty. Please set one with -ip-incdir=a:b:c (or semicolon on Windows)")
                    cleanexit(sprintf "L1182: Failed trying to load a missing abstractionDefinition IP-XACT document: search path=%s filename=%s suffix=%s. Places looked=%s" path (hptos busType) g_ip_xact_file_suffix (sfoldcr_lite id places_looked))
                | (Some filename, _) ->
                    let msg = sprintf "Read IP-XACT abstractionDefinition %s" filename
                    let ww = WF 3 "retrieve/read_in abstractionDefinition" ww msg
                    let xml = hpr_xmlin ww filename
                    let readin_ipxact_onon ms ats_ xml =
                        let direction = xml_once_atom msg ("spirit:" + "direction") None xml
                        let bitwidth = xml_once_int msg ("spirit:" + "bitWidth") None xml
                        (direction, bitwidth)

                    let readin_ipxact_qualifier ats_ xml =
                        let isClock =         xml_once_bool msg ("spirit:" + "isClock") (Some false) xml
                        let isReset =         xml_once_bool msg ("spirit:" + "isReset") (Some false) xml
                        let isAddress =       xml_once_bool msg ("spirit:" + "isAddress") (Some false) xml
                        let isData =          xml_once_bool msg ("spirit:" + "isData") (Some false) xml                        
                        (isClock, isReset, isAddress, isData)

                    let readin_ipxact_wire lname ats_ xml =
                        let qualifiers =
                            match xml_multi ("spirit:" + "qualifier") readin_ipxact_qualifier xml with
                                | [] -> None
                                | item::_ -> Some item

                        //let lname = xml_once_atom msg ("spirit:" + "logicalName") None xml
                        let masterf = xml_once msg ("spirit:" + "onMaster") (readin_ipxact_onon "M") None xml
                        let slavef = xml_once msg ("spirit:" + "onSlave") (readin_ipxact_onon "S") None xml  
                        //dev_println (sprintf " wire name read as >%s< master=%A slave=%A" lname masterf slavef)
                        let width = match (snd masterf, snd slavef) with
                                      | (n, m) when n=m -> n
                                      | (n, m) -> max n m // FOR now - perhaps warn or maintain distinction
                        let sdir = match (fst masterf, fst slavef) with
                                      | ("input", "output")  -> "o"
                                      | ("output", "input")  -> "i"
                                      | ("output", "output") -> "O"
                                      | ("input", "input")   -> "x"
                                      | other ->
                                          hpr_yikes(msg + " illegal input output dir combination on " + lname)
                                          "i"

                        (lname, width, sdir)

                    let readin_ipxact_tl lname ats_ xml =
                        let initiative = xml_once_atom msg ("spirit:" + "initiative") None xml
                        //let _ = dev_println (sprintf " name read as >%s< master=%A slave=%A" name masterf slavef)
                        (lname, initiative)

                    let readin_ipxact_port ats_ xml =
                        let lname = xml_once_atom msg ("spirit:" + "logicalName") None xml
                        let wire = xml_once msg ("spirit:" + "wire") (readin_ipxact_wire lname) None xml // we require this - not the standard.
                        let trans =
                            match xml_multi ("spirit:" + "transactional") (readin_ipxact_tl lname) xml with
                                | [] -> None
                                | item::_ -> Some item
                        //let _ = dev_println (sprintf " name read as >%s< master=%A slave=%A" name masterf slavef)
                        (wire, trans)

                    let readin_ipxact_ports ats_ xml =
                        let ans = xml_collate ("spirit:" + "port") readin_ipxact_port xml
                        ans            

                    let readin_ipxact_abstractionDefinition ats_ xml =
                        let vlnv = readin_vlnv msg xml 
                        let ports = xml_collate ("spirit:" + "ports") readin_ipxact_ports xml
                        (vlnv, ports)

                    let (vlnv__, ports) = xml_once msg ("spirit:" + "abstractionDefinition") readin_ipxact_abstractionDefinition None [xml]

                    //vprintln vd (sprintf "Read IP-XACT astractionDefinition %s : XML in scanfor %A" filename xml)

                    let contact_schema =
                        let gen_sch port_schema = 
                            let gen_sch1 ((lname, width, sdir), trans_pair_o_) =
                                { g_null_netport_pattern_schema with
                                    lname=   lname //"ADDR"
                                    width=   i2s64 width // "ADDR_WIDTH"      
                                    sdir=    sdir // Direction on slave
                                    //pon="hwr";  chan="RW" 
                                    ats=[] // thinks like : etc [(g_ip_xact_is_Address_tag, "true")]
                                }

                            (map gen_sch1 port_schema)
                        list_flatten(map gen_sch ports) // one xactor per port scheme
                        
                    let ans = (contact_schema, definitionMeld)
                    g_external_bus_abstractions_database1.add (hptos busType) ans
                    ans
                    // end of retrieve_busAbstraction

// Parse key/value string pairs
let readin_ipxact_parameters msg ats xml =
    let readin_ipxact_parameter msg ats xml =
        let key =  xml_once_atom msg ("spirit:" + "name") None xml
        let value =  xml_once_atom msg ("spirit:" + "value") None xml 
        (key, value)
    let ans = xml_collate ("spirit:" + "parameter") (readin_ipxact_parameter msg) xml
    ans


//
// Read IP-XACT component definition
//
let readin_ip_xact_subsblock ww block_kind =
    let path = sfold_colons !g_ip_incdir 
    let vd = 3
    let fn_kind = map fn_sanitize block_kind

    let readin_serf filename =
            let msg = sprintf "Read IP-XACT component definition %s from %s" (hptos block_kind) filename
            aux_report_log 2 "IP-XACT input/output" [msg]
            let xml = hpr_xmlin ww filename
            //vprintln 0 (msg + sprintf ": XML in scanfor %A" xml)
            let getname msg ofwhat ats_ arg =
                //| other -> sf(msg + sprintf ": expected %s name in %s.  not %A" ofwhat block_kind other)
                let name = xml_once_atom msg ("spirit:" + "name") None arg
                name
                
            let readin_ipxact_busType ats xml_ =
                let vendor = op_assoc "vendor" ats
                let library = op_assoc "library" ats
                let name = op_assoc "name" ats
                let version = op_assoc "version" ats
                match name with
                    | None -> sf (msg + ":bus abstraction missing name attribute")
                    | Some name -> name

            let readin_ipxact_portMap msg ats xml =
                //let _ = dev_println (sprintf " grack portMap from %A" xml)
                let logical  = xml_once msg ("spirit:" + "logicalPort")   (getname msg "logicalPort")  None xml
                let physical = xml_once msg ("spirit:" + "physicalPort")  (getname msg "physicalPort") None xml      
                (logical, physical)
                
            let readin_ipxact_portMaps msg ats xml =
                let ans = xml_collate ("spirit:" + "portMap") (readin_ipxact_portMap msg) xml
                ans

            let readin_ipxact_busInterface ats_ xml =
                let pi_name = xml_once_atom msg ("spirit:" + "name") None xml // aka nameGroup
                let msg = msg + ": Reading Interface pi_name=" + pi_name
                let busType = xml_once msg ("spirit:" + "busType") readin_ipxact_busType None xml  
                let busDef_meld = retrieve_busDefinition ww [busType] // Mostly ignored except for fsems.
                let busAbsDef = retrieve_abstractionDefinition ww busDef_meld [busType + "_rtl"]
                let parameters = xml_once msg ("spirit:" + "parameters") (readin_ipxact_parameters msg) None xml
                let portMaps = xml_once msg ("spirit:" + "portMaps") (readin_ipxact_portMaps msg) None xml
                let scx token =
                    let pred = function
                        | XML_ELEMENT(d, _, _) -> d=token
                        | _ -> false
                    not_nullp (List.filter pred xml)
                let masterf = scx ("spirit:" + "master")  
                let slavef = scx ("spirit:" + "slave")              
                //let _ = dev_println (sprintf "busInterface name read as >%s< master=%A slave=%A" pi_name masterf slavef)
                (pi_name, busType, busAbsDef, portMaps, masterf, slavef, parameters) 


            let readin_ipxact_busInterfaces ats_ xml =
                let ans = xml_collate ("spirit:" + "busInterface") readin_ipxact_busInterface xml
                ans            

            let readin_component_vendor_extension ats_ xml =
                let msg = "IP-XACT component vendor extension fields"
                let area =  xml_once_double_option msg ("hrpls:" + "area") xml
                let heapspaces _ xml =
                    let readin_heapspaces ats_ xml =
                        let spacename = muddy (sprintf "Need to parse space/bytes pairs from %A" xml)
                        let usebytes = xml_once_int_option msg ("hrpls:" + "heapspace") xml
                        (spacename, usebytes)
                    xml_once msg ("hprls:" + "heapspaces") readin_heapspaces (Some([], None)) xml
                (area, heapspaces)


            let readin_ipxact_component ats_ xml =
                let vlnv = readin_vlnv msg xml 
                let interfaces = xml_once msg ("spirit:" + "busInterfaces") readin_ipxact_busInterfaces None xml
                let (area, heapspaces) = xml_once msg ("spirit:" + "vendorExtensions") readin_component_vendor_extension None xml
                (vlnv, interfaces, area, heapspaces)
                
            let (vlnv, interfaces, area, heapspaces) = xml_once msg ("spirit:" + "component") readin_ipxact_component None [xml]

            // An interface corresponds directly to a method group. We currently use the 'one method per busInterface'. i.e. methodgroups with only one method. These are therefore non-exclusive.
            // And with the currently-supported, canned protocols, there is one method per methodgroup.
            // TODO implement the methodgroup and exclusivity matrix in the future and load it from an IP-XACT file extension.
            let zipped_methods =
                let grock_external_method_declaration (pi_name, busType, busAbsDef, portMaps, masterf_, slavef_, parameters) =

                    let busAbsDef:proto_mgr_meld_abstraction_t = busAbsDef
                    let (contact_schema, busDef) = busAbsDef
                    // let contact_schema = List.filter (pon_pred) g_hfast_port_structure // This now moved here to mgr schema?

                    let protocol_prams = { max_outstanding=1; req_present=true; ack_present=true; reqrdy_present=true; ackrdy_present=false;  posted_writes_only=false } // Make ackrdy_present programmable via recipe or schema string?  
                    let hs_protocol = IPB_HFAST(protocol_prams, PD_halfduplex) // TODO this should be readin from IP-XACT
                    let hs_lnames_canned = [ "req"; "ack"; "reqrdy"; "ackrdy"; "REQ"; "ACK"; "REQRDY"; "ACKRDY" ]
                    let portmeta = parameters

                    // For now - supports one instance of that interface on that subsidiary block only - we can use portmap alternatively.
                    let (hs_nets, rems) =  // hs_ is being ignored and recooked - default values needed etc for tieoffs.
                        let hs_pred schem = memberp schem.lname hs_lnames_canned
                        groom2 hs_pred contact_schema 

                    let (rv, args) =
                        let rvpred schem = schem.sdir = "o"
                        groom2 rvpred rems

                    let import_port_net_declaration schem =
                        let w = port_eval_int msg busType portmeta schem.width 
                        //let _ = vprintln 0 (msg + sprintf " import_port_net_declaration width: busAbs=%s lname %s width %s" busType schem.lname schem.width)
                        ({ g_default_prec with widtho=Some(int32 w); }, (pi_name, schem.lname, pi_name + "_" + schem.lname))
                    let arity = length args

                         
                    let portmeta = [] // for now
                    let sim_model = [] // for now - diosim should load the concrete IP or dont use diosim for complex (multi-block) designs.
                    let method_name =
                        match op_assoc "methodName" parameters with
                            | None            -> pi_name
                            | Some methodName -> methodName
                    let (sq_o, overloaded) =
                        match op_assoc "overloadSuffix" parameters with
                            | None -> (None, false)
                            | Some overloadSuffix -> (Some overloadSuffix, true)

                    // TODO signed and floating point marshalling?
                    let (rv, ret_details) =
                        match rv with
                            | [item] ->
                                let def = pi_name + "_" + item.lname
                                let mapped = op_assoc item.lname portMaps
                                //let _ = dev_println (msg + sprintf ": pi_name=%s compare ret_details def=%s with lookedup=%A" pi_name def mapped)
                                (fst(import_port_net_declaration item), Some(pi_name, item.lname, def)) // We can get physical names using the IP-XACT portMap or just by following a convention that we prefix the interface instance name and squirrel suffix when overloaded.
                            | [] -> (g_void_prec, None)
                            | multiple ->
                                let bish schem = sprintf "   w=%s  logicalName=%s" schem.width schem.lname
                                sf(msg + sprintf " cannot cope with multiple return values from subsidiary block call in method %s " pi_name + "\nValues are " + sfold bish multiple)

                    let vlat = grock_protocol_vlat ww msg pi_name hs_protocol portMaps portmeta hs_nets
                        
                    let imported_arg_details = map import_port_net_declaration args
                    let return_lname = if nonep ret_details then "return" else f2o3(valOf ret_details)
                    let meld_xactor =
                        let req =
                            match vlat with
                                | None -> []
                                | Some vlat ->
                                    // f3o4 returns the schema.  f4o4 returns the mapped formal name (which we reproduce ourselves according to convention currently).
                                    [ ((f3o4 vlat.vlat_req).lname, "1") ]  // en.gs (the work guard) needs popping in unless abortable

                        let sender = 
                            let argsend n = (f2o3(snd(select_nth n imported_arg_details)), sprintf "ARG%i" n)      
                            ("t0", (if arity=0 then [] else map argsend [0..arity-1]) @ req)

                        let getter =
                            if nonep ret_details then []
                            else
                                let rx = valOf_or_fail "ret_details" ret_details
                                //let _ = dev_println (sprintf "rdbus rx = %A" rx)
                                [ ("t1", [ (return_lname, "RV") ]) ]
                        sender :: getter

                    let method_meld:method_meld_abstraction_t =
                        { method_name=     method_name
                          method_sq_name=  sq_o
                          return_lname=    return_lname
                          arg_details=     args
                          voidf=           nonep ret_details
                          arity=           arity // includes protocol overhead
                          meld_xactor=     meld_xactor
                          sim_model=       sim_model; sim_nets = []
                          meld_fsems1=     busDef.meld_fsems0
                        }

                    dev_println (sprintf "Need to grock latency")
                    vprintln 0 (msg + sprintf ": L1557 grock_external_method_declaration read method name=%s arity=%i sq_o=%A" method_name arity sq_o)
                    let latency_pair = (1, 1)// TODO - should be read in from IP-XACT - it is written out in the busDefinition along with the fsems.

                    let fsems =
                        //sf (sprintf "Need fsems from %A" busAbs) 
                        //let arity = methodAbs.arity
                        { method_meld.meld_fsems1 with fs_overload_disam=overloaded } // >H

                    let gis_ip_import pi_name arity =
                        let pi_name_lst = pi_name // This could be a list if multiple alternate ports are available for work sharing, but it is not a list at the moment.
                        let method_lookup_key = if fsems.fs_overload_disam then method_name + "+" + valOf_or_fail "LY2517" sq_o else method_name
                        g_isloaded_db.add method_lookup_key (true, false, vlnv, pi_name_lst, [])
                        // pi_name maps one-to-one with mgr_name where the mgr is named
                        let fnameo = if method_name = pi_name then None else Some (pi_name) // Provide method name or pi_name on the FU if different from the name used in xi_apply.
                        //dev_println(sprintf "LATEAM: method_lookup_key=%A fnameo=%A" method_lookup_key fnameo)
                        { g_default_native_fun_sig with overload_suffix=sq_o; fsems=fsems; args=map fst imported_arg_details; rv=rv; latency=Some latency_pair; is_fu=Some(vlnv, fnameo) }
                    let gis_xml = gis_ip_import pi_name arity

                    let mgr_meld =
                      {
                          mgr_name=       pi_name
                          method_melds=   [ method_meld ] // One method per mgr is hardcoded here for FUs imported via IP-XACT - at the moment.
                          contact_schema= contact_schema // For this mgr.
                          hs_protocol=    hs_protocol
                          hs_vlat=        vlat
                          is_hpr_cv=      false
                      }

                    // Note we ignored masterf and slavef.  We are just assuming for now that the callee will be a slave/target and not an initiator. Perhaps trap this!
                    // The pi_name will consist of the method name and a squirrel if overload_disam holds.
                    (mgr_meld, ((method_name, sq_o, arity), (gis_xml, ret_details, vlat, imported_arg_details)))
                map grock_external_method_declaration interfaces

            let (mgr_melds, phys_methods) = List.unzip zipped_methods

            let mgr_mirrorable (mgr_meld:mgr_meld_abstraction_t) =
                let method_mirrorable method_meld =
                    let not_mirrorable = method_meld.meld_fsems1.fs_nonref || not method_meld.meld_fsems1.fs_mirrorable
                    not not_mirrorable
                conjunctionate method_mirrorable mgr_meld.method_melds 

            let portmeta = [] // for now please re-instate                 --- 

            let pon_pred _ = true
            
            let block_meld =
                { kinds=            hptos vlnv.kind
                  parameters=       portmeta // Formals please
                  formals_rides=    [] // Are there any?
                  contact_schema=   []
                  mgr_melds=        mgr_melds
                  mirrorable=       conjunctionate mgr_mirrorable mgr_melds
                }

            let phys_structure = 
                // Create a netport_pattern_schema over all interfaces on the block.
                //let grock_pattern_schema (pi_name, busType, mgrAbs, portMap__, _, _, parameters) cc =
                let grock_pattern_schema (mgr_meld:mgr_meld_abstraction_t) cc =                
                    let apply_portmap arg =
                        (mgr_meld.mgr_name, (* name + "_" +*) arg.lname, arg)
                    
                    let ans = map apply_portmap mgr_meld.contact_schema
                    ans @ cc
                    
                List.foldBack grock_pattern_schema block_meld.mgr_melds []

            let block = // This block def is not needed here - better to rez in restructure etc and just return block_meld and area etc here ?  We need the portmeta actuals here I think
                {
                  ip_vlnv=             vlnv
                  kind=                hptos block_kind // Keep as stringl would be better?
                  phys_methods=        phys_methods
                  phys_area=           area
                  //sim_model=           sim_model; sim_nets = []
                  meld=                block_meld
                  meta=                portmeta  // Bindings please
                  phys_structure__=    phys_structure // lname chan pon etc mdir
                }
            let _ = WF 3 msg ww (sprintf "Read in FU description for IP block %s. This has %i interfaces, %i methods. Area=%A.  Mirrorable=%A" (vlnvToStr vlnv) (length interfaces) (length phys_methods) area (block_meld.mirrorable))
            (block, phys_methods)


        
    match pathopen_any path vd [';'; ':'] (candidate_filenames fn_kind) g_ip_xact_file_suffix with
        | (Some filename, _) ->
            readin_serf filename 
        | (None, places_looked) ->
            let msg1 = (if path="" then "\nPlease set a search path using flag -ip-incdir=... or setting HPRLS_IP_INCDIR envvar." else "")
            cleanexit (sprintf "Cannot open IP file root=%s suffix=%s using path '%s'%s.  Places_looked=%s" (hptos fn_kind) g_ip_xact_file_suffix path msg1 (sfoldcr_lite id places_looked))




// Lookup an IP block in IP-XACT descriptions    
let assoc_external_block ww msg block_kind externally_instantiated_o =
    match g_external_ip_blocks_database.lookup block_kind with
        | None ->
            let (block, phys_methods) = readin_ip_xact_subsblock ww block_kind
            g_external_ip_blocks_database.add block_kind (block, block.meld, phys_methods, externally_instantiated_o)
            (block, block.meld, phys_methods, externally_instantiated_o)
            //cleanexit(sprintf "cannot find external IP block in database. kind=%s for method name %s" kind fname)
        | Some blockdetails -> blockdetails


// Lookup a method on external FU via its IP-XACT meta info.
let assoc_external_method ww msg (block_kind:string list) fname sq arity externally_instantiated_o =

    let (block, fu_meld, phys_methods, externally_instantiated_info) = assoc_external_block ww msg block_kind externally_instantiated_o // IP-XACT lookup


    let methoder = 
        match op_assoc (fname, sq, arity) phys_methods with
            | None ->
                let available = sprintf "\nExported methods available are ^%i: " (length phys_methods) + sfold xactor_name_to_str phys_methods
                cleanexit(msg + sprintf ": Cannot find method %s,  sq=%A with arity %i in external IP block block_kind=%s" fname sq arity (hptos block_kind) + available)
            | Some methoder -> methoder
    let externally_instantiated =
        match externally_instantiated_info with
            | Some true -> true
            | _         ->  false 
    (fu_meld, methoder, externally_instantiated, fu_meld.mirrorable)





//
// Get the FU kind details (but no instance details) for a function call such as hpr_alloc or fpcvt.
// These may be canned in here or else loaded from an IP-XACT file.
//
let assoc_fu_method ww msg (block_kind:string list) fname sq arity externally_instantiated_o =
    let method_lookup_key = if not_nonep sq then fname + "+" + valOf sq else fname
    //dev_println(sprintf "method_lookup_key=%s" method_lookup_key)
    match g_isloaded_db.lookup method_lookup_key with

        // Everything takes this route now and the rest of this match is rubbish?
        | Some (is_loaded, is_cv_canned, kind, pi_name, instances_) when is_loaded -> assoc_external_method ww msg (block_kind:string list) fname sq arity externally_instantiated_o

        | Some (is_loaded, is_cv_canned, kind, pi_name, instances_) when is_cv_canned -> // Dont like this clause!
            sf ("is_cv_canned trap - method names on CV library FUs should be hardwired. " + method_lookup_key)
        
        | Some (is_loaded, is_cv_canned, kind, pi_name, instances_) when not is_loaded -> // Special control flow for canned FUs.    But see the  install_canned_FU_metainfo() code too.
        
            // Currently we look this up in a preloaded map and then make a method dispatch with a case statement - cleaner (and higher order) to just store the factory method in the map.
            // We have a small amount of vestigial code in restructure for SRAM, ALU and FPCVT, but other canned/CV FUs in future, such as heap manager, are treated here instead as canned generics.
            match fname with
                | "hpr_alloc" ->  // Only one here for now ... ? Seems odd. Try to follow ALU and FPCVT paradigm for canned CV components in future please...
                    let model = "T0" // for now
                    let (kind, fu_meld, mgr_meld, rv_prec, externally_instantiated, variable_latency, port_schema) = rez_canned_heapmanager ww model pi_name
                    gec_temp_heapmanager_meld ww block_kind msg port_schema mgr_meld fname arity rv_prec fu_meld sq externally_instantiated
                | other -> sf (sprintf "assoc_fu_method: canned function or method '%s' not matched afterall!" fname)

        | Some _ -> muddy (sprintf "method fname=%s,  disambiguation key=%s :Not canned or loaded already or still to be loaded." fname  method_lookup_key)
        | None ->
            let report_possibility key = vprintln 0 (sprintf "   available method names at fail point includes: %s" key)
            for x in g_isloaded_db do report_possibility x.Key done
            cleanexit(sprintf "L2641 Method not found:  method fname=%s,  disambiguation key=%s :Not canned or loaded already or still to be loaded." fname  method_lookup_key)



// Write a report on memory bondouts.  We list each port and each address space.  Every port belongs to an address space.
let bondout_port_report ww preliminaryf toolname title tableReports bondouts = 

    let m_capacities_reported = ref []
    let space_report1 (space, (ppl:bondout_port_t list), mm_manager) =
        let _:bondout_memory_map_manager_t = mm_manager
        let (_, hwm) = mm_manager.port_alloc_scan  X_undef space.logicalName
        let used = not (nonep hwm)
        let hwm2 = if nonep hwm then "--not-used--" else sprintf "%A" (valOf hwm)
        let m1 = (sprintf "Highest off-chip SRAM/DRAM location in use in logical memory space %s is %A (%s) bytes=%i" space.logicalName hwm hwm2 space.wordsAvailable)
        aux_report_log 2 toolname [ title; m1]
        //let _ = vprintln 1 m1
        ()

    if not preliminaryf then  app space_report1 bondouts


    let _ =
        let hdr = [ "AddressSpace"; "Name"; "Protocol"; "Bytes"; "Addressable Words"; "Awidth"; "Dwidth"; "Lanes"; "LaneWidth"; "ClockDom" ]
        let port_report2 (space, (port:bondout_port_t), mgr_) =
            let bytes = i2s64 port.dims.bytesAvailable
            let bytes =
                if memberp space.logicalName !m_capacities_reported then sprintf "(%s)" bytes
                else
                    mutadd m_capacities_reported space.logicalName
                    bytes
            let clockDom = port.clock_domain
            [ space.logicalName; port.ls_key; ipbToStr port.protocol; bytes; i2s(two_to_the32 port.dims.addrSize); i2s port.dims.addrSize; i2s(port.dims.no_lanes * port.dims.laneWidth); i2s port.dims.no_lanes; i2s port.dims.laneWidth; clockDom ]

        
        let portflattener (space, ports, manager) cc = map (fun port -> (space, port, manager)) ports @ cc
        let table = tableprinter.create_table (sprintf "Bondout Load/Store (and other) Ports '%s'" title, hdr, map port_report2 (List.foldBack portflattener bondouts []))
        aux_report_log 2 toolname table
        mutadd tableReports table
        () //app (vprintln 2) table
    ()





// Create meeld for a bondout port. This provides access to an 'offchip' memory bank.
let gec_bondout_fu_meld ww ports =

    let (no_ports_on_space:int) = length ports

    let dims = once "Differing dimensions within bondout port group not currently supported" (list_once(map (fun port -> port.dims) ports))
    
    let lanes_schema =
        if dims.no_lanes < 2 then []
        else
          [
             { g_null_netport_pattern_schema with lname="LANES";  chan="W";  width= i2s dims.no_lanes;  sdir="i";    pon="hw";  ats=[(g_ip_xact_is_Address_tag, "true")]  };
          ]

    let bondout_port_structure =  // This has read-only and write-only simplex variants and a half-duplex variant.
     [
      { g_null_netport_pattern_schema with lname="ADDR";   chan="RW";  width="ADDR_WIDTH";  sdir="i";    pon="hwr"; ats=[(g_ip_xact_is_Address_tag, "true")] };
      { g_null_netport_pattern_schema with lname="RWBAR";  chan="RW";  width="1";           sdir="i";    pon="h";   ats=[(g_control_net_marked, "true")] };
      { g_null_netport_pattern_schema with lname="WDATA";  chan="RW";  width="DATA_WIDTH";  sdir="i";    pon="hw";  ats=[(g_ip_xact_is_Data_tag, "true")]  };
      { g_null_netport_pattern_schema with lname="RDATA";  chan="RW";  width="DATA_WIDTH";  sdir="o";    pon="hr";  ats=[(g_ip_xact_is_Data_tag, "true")]  };


     ] @ lanes_schema

    let wanted_pred protocol arg =
        match protocol with
            | IPB_HFAST(protocol_prams, pd) ->
                let cc =
                    match pd with
                        | PD_halfduplex -> 'h'
                        | PD_loadport   -> 'r'
                        | PD_storeport  -> 'w'
                memberp cc (explode arg.pon)
            | other -> sf (sprintf "unsported bondout protocol" )
            

    let abits = dims.addrSize
    let dbits = dims.no_lanes * dims.laneWidth

    let portmeta = // all dims must be the same since portmeta is shared over the instance at the moment.
            [
              ("ADDR_WIDTH", i2s abits)
              ("DATA_WIDTH", i2s dbits)
            ]

    let pxform_hi (port:bondout_port_t, idx) =
        // Create one instance per port with just one method group, despite the ports sharing spaces at a higher level.
        //if ocp = IPB_FPFL then (cc, cd) // not sensible and highly unlikely
        //else

        let (hs_net_schema, vlat, hs_t0_control, hs_t1_control) = gec_hs_net_schema_vlat_and_melsh ww port.protocol
        let contact_schema = hs_net_schema  @ List.filter (wanted_pred port.protocol) bondout_port_structure

        let addr_pattern_schema = once "bondout_addr" (List.filter (fun x->x.lname="ADDR") contact_schema)
        let data_pattern_schema = once "bondout_data" (List.filter (fun x->x.lname="WDATA") contact_schema)       

        let write_xactor =
            let lane_control =
                if port.dims.no_lanes < 2 then [] 
                else [("LANES", "ARG2")]
            [ ("t0", [ ("ADDR", "ARG0"); ("WDATA", "ARG1"); ("RWBAR", "0"); ] @ lane_control @ hs_t0_control)
            ]

        let read_xactor =
            [ ("t0", [ ("ADDR", "ARG0");  ("RWBAR", "1") ] @ hs_t0_control);
              ("t1", [ ("RDATA", "RV") ] @ hs_t1_control);
            ]

        let gec_method_meld fname = 
                   { method_name=      fname
                     method_sq_name=   None
                     return_lname=     "RDATA"
                     arity=            if fname="read" then 1 elif port.dims.no_lanes >= 2 then 3 else 2
                     voidf=            (fname="write")
                     arg_details=      if fname="write" then [ addr_pattern_schema; data_pattern_schema ] else [ addr_pattern_schema ]
                     meld_xactor=      if fname="write" then write_xactor else read_xactor
                     meld_fsems1=      { g_default_fsems with fs_nonref=true; fs_eis=true; fs_mirrorable=false } // Bondout port
                     // Not currently used?
                     sim_model=        []
                     sim_nets=         []
                   }

        let mgr_meld =
            {
                      mgr_name=       i2s idx // Not : port.ls_key
                      method_melds=   map gec_method_meld [ "read"; "write" ] // If half-duplex only have one
                      contact_schema= contact_schema // For this mgr.
                      hs_protocol=    port.protocol
                      hs_vlat=        vlat
                      is_hpr_cv=      true
            }
        mgr_meld

    let mgr_melds = map pxform_hi (zipWithIndex ports)
    let fu_meld = 
            {   kinds=            sprintf "bondout_port_%i_2**%ix%i" no_ports_on_space abits dbits
                parameters=       portmeta
                formals_rides=    [] // (string * hexp_t) list              // Parameter overrides - formal schema (with defaults?)
                contact_schema=   [] // Additional beyond mgr nets - clocks and reset please
                mgr_melds=        mgr_melds
                mirrorable=       false
            }

    fu_meld


// Shared resource identifers.
// We make this global across recipe stages for now.

type sri_digest_t = string // for now!
let g_sri_store = new OptionStore<sri_digest_t, shared_resource_info_t>("sri_store")
 

let sri_digest (idl, ats) =
    let info = (idl, ats)
    let key = hptos idl
    g_sri_store.add key info
    key

let sri_hydrate key =  // Inverse of sri_digest
    match g_sri_store.lookup key with
        | None ->
            sf(sprintf "sri_hydrate: nothing stored under " + key)
        | Some info
            -> info
// eof
