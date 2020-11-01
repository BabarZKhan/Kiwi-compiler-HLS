(*
 * cvipgen - IP Block Generator for simple CV leaf components.
 *
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
 * (C) 2003-17 DJ Greaves.
 *
 * The core constructs of hprls are split between abstracte.sml and meox.sml.
 * This file contains the main imperative and functional programming constructs.
 * It stores all expressions in a 'normalish form' in a memoising heap expression data structure and performs a multititude of peep-hole optimisations..
 * Many other generic expression handling routines are in this file as well.
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

 *
 
 *
 * This module automatically generates simulation models and perhaps concrete implementations for various generic leaf IP Blocks such as adders and RAMs.
 *)

module cvipgen

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
open abstracte

open protocols

// These are constructors for the so-called pents
let f_comb (g, l, r)       = (0, g, l, r, X_undef)
let f_seq  (g, l, r)       = (1, g, l, r, X_undef)
let f_seq0 (pcpo, g, l, r) = (1, pcpo, g, l, r, X_undef)
let f_fut  (g, l, r)       = (-1, g, l, r, X_undef)

let posfind m = function
    //| []      -> sf ("clock needed for " + m)
    | E_pos clk -> (E_pos clk, clk)
    | E_neg clk -> sf ("TODO: negedge needed (not in IP library) for " + m)   
    | clkother -> sf ("too many clock domains or -ve edge clock for " + m + ": edges=" + edgeToStr clkother)            

let get_by_logical (ikey, kinds) pi_name lname contacts =
    let eqeq_o lname = function
        | None -> false
        | Some ln -> lname_eqeq lname ln
    let rec scan = function
        | (formal_name_, X_bnet ff)::tt ->
            let f2 = lookup_net2 ff.n
            if eqeq_o lname (at_assoc g_logical_name f2.ats) && (pi_name = "" || at_assoc g_port_instance_name f2.ats = Some pi_name) then X_bnet ff
            else scan tt
        | _::tt -> scan tt
        | [] ->
            let pos = "\nCandidates are " + sfoldcr_lite (fun (fid, net) -> sprintf "  %A %s" fid (netToStr net)) contacts
            sf(sprintf "get_by_logical: formal contact to module not found: instance_key=%s kind=%s pi_name=%s logical_name=%s" ikey kinds pi_name lname + pos)
    scan contacts

let db_netwrap_formal (logical_name_, formal) = DB_leaf(Some formal, None)    
let db_netwrap_assocp ((logical_name_, formal), net) = DB_leaf(Some formal, Some net)

let sq_prec (prec:precision_t) =
    if prec.signed = FloatingPoint then (if prec.widtho=Some 32 then "SP" else "DP") elif prec.signed=Unsigned then "U" else "I"




// Common leaf routine that marshalls the nets for a component definition or instance according to purpose.
let declsrez_formals stagename pi_name ctrl purpose externally_instantiated (formal_rides, actual_rides, formals, local_sim_nets) =

    let cpi = { g_null_db_metainfo with kind= "res2-contacts"; pi_name=pi_name; not_to_be_trimmed=true; purpose=purpose } 
    let local_cpi = cpi

    let a1_formal_rides =
        let cpi = { cpi with kind= "res2-rides"; form=DB_form_pramor }
        gec_DB_group(cpi, map db_netwrap_formal formal_rides)

    let a3_formals =
        let formals_ctrl = map fst ctrl // Include clock, clock_enable (cen) and reset where appropriate. This is the common clock form and not the domain crosser.
        //let cpi = { cpi with kind= sprintf "declrez-ctrl%i" (length ctrl);  }
        //gec_DB_group(cpi, map db_netwrap_assocp ctrl)

        let form0 = if externally_instantiated then DB_form_external else DB_form_local // This form0 field is probably not needed now diosim makes proper instances.
        let cpi = { cpi with kind= "res2-contacts"; form=form0 }
        gec_DB_group(local_cpi, map db_netwrap_formal (formals_ctrl @ formals))

    let a4_simnets =
        let cpi = { cpi with kind= "res2-sim-nets"; (*  norender=true; *)  } // Nets for behavioural model for diosim etc..  If we mark these as norender we get malformed custom ROMs.  The norender effect is now achieved through respecting the net hierarchy and the component definition not being rendered.
        gec_DB_group(cpi, map db_netwrap_null local_sim_nets)

    vprintln 3 (sprintf "declsrez_formals %i rides for %s" (length a1_formal_rides) pi_name)
    (a1_formal_rides @ a3_formals @ a4_simnets)


let declsrez_actuals stagename pi_name ctrl purpose externally_instantiated (formal_rides, actual_rides, formals, actuals, local_sim_nets) =
    let cpi = { g_null_db_metainfo with kind= "res2-contacts"; pi_name=pi_name; not_to_be_trimmed=true; purpose=purpose }             

    let db_netwrap_contact ((f_logical_name, formal), (_, actual)) = DB_leaf(Some formal, Some actual)

    let a1_contacts_rides =
        if nullp actual_rides then []
        else
            let cpi = { cpi with kind= "declsrez-rides"; form=DB_form_pramor }
            //let _ = dev_println (sprintf "declsrez parameter overrides: %i cf %i" (length formal_rides) (length actual_rides))
            let cx = List.zip formal_rides actual_rides 
            gec_DB_group(cpi, map db_netwrap_assocp cx)

    let a2_contacts_ctrl =
        let cpi = { cpi with kind= sprintf "declsrez-ctrl%i" (length ctrl);  }
        gec_DB_group(cpi, map db_netwrap_assocp ctrl)



    let a3_actuals_decls =
        let form0 = if externally_instantiated then DB_form_external else DB_form_local // This form0 field is probably not needed now.
        let cpi = { cpi  with form=form0 }
        let db_netwrap_actual (_, net) = DB_leaf(None, Some net)
        gec_DB_group(cpi, map db_netwrap_actual actuals)


    let a3_contacts =
        if nullp actuals then []
        else
            let form0 = if externally_instantiated then DB_form_external else DB_form_local // This form field is probably not needed now.
            let cpi = { cpi  with form=form0 }
            //dev_println (sprintf "declsrez contact zip: %i cf %i" (length formals) (length actuals))
            let cx = List.zip formals actuals
            let db_netwrap_contact ((_, fnet), (_, anet)) = DB_leaf(Some fnet, Some anet)
            gec_DB_group(cpi, map db_netwrap_contact cx)

    vprintln 3 (sprintf "declsrez_actuals  %s, pi_name=%i contacts=%i" pi_name (length a2_contacts_ctrl) (length a3_contacts))
    (a3_actuals_decls, a1_contacts_rides @ a2_contacts_ctrl @ a3_contacts)


let rec string_list_flatten = function
    | [ item ] -> (item:string)
    | h::tt    -> h + "_" + string_list_flatten tt

let sqfold = string_list_flatten




// When a component is instantiated more than once by diosim with different parameter overrides or precisions, a given net can have more than one width or other attribute.
// We use a unique id for all parameterisations of a logical nets. This is needed by the memoizing heap system. The variation is hidden by output since stages like RTL render that use instead the logical name for a contact, such as 'arg0' or 'NN'.
// For our id, we could catentate all the string fields, but this gets a bit long, so we make up a short alias tokens here. Typical names are cvalu10 and cvalu12
let g_definitional_ikey_dbase = new OptionStore<string * string list * string, string>("definitional_ikey_database")

let definitional_ikey_alloc ww generic_kind (kind:ip_xact_vlnv_t) sq =

    let key = (generic_kind, kind.kind, sq)
    match g_definitional_ikey_dbase.lookup key with
        | Some ov -> ov
        | None ->
            let ans = funique generic_kind
            g_definitional_ikey_dbase.add key ans
            vprintln 2 (sprintf "Setup short key (definitional key) %s for  %s %s %s" ans generic_kind (vlnvToStr kind) sq)
            ans
            
let rez_definition_ii externally_instantiated kkey =
    { g_null_vm2_iinfo with
          vlnv=                 { g_canned_default_iplib with kind=kkey }
          externally_provided=  true
          external_instance=    externally_instantiated 
          preserve_instance=    true
          definitionf=          true
          generated_by=         "cvipgen"
      }


let gec_ctrl () =   // This should perhaps accept a +ve/-ve edge triggered arg and so on.
    // The directorate render should now convey these nets through the design - we dont need to include them in the decls net list. But it does need to be in the contact map for the component.  But listing the same net more than once under various DB groups is allowed and that is what happens.
    let ctrl = [ (("clk", g_clknet), g_clknet); (("reset", g_resetnet), g_resetnet) ]  // (formal, actual) pairs owing to historical nonsense - actuals are meaningless here
    let new_director = { g_null_directorate with clocks=[E_pos g_clknet]; resets=[(true, false, g_resetnet)]; duid=next_duid() } // This reference to global clock  and reset etc (and cen in future) is misleading - these are inputs that can be wired on a per-instance basis.
    (ctrl, new_director)


let lookup_i64 ww msg rides pram = 
    match op_assoc pram rides with
        | None ->
            let pbind (k,v) = sprintf "k=%s / v=%s " k v
            vprintln 0 (sprintf "Override bindings were " + (if nullp rides then "NONE PROVIDED" else sfoldcr_lite pbind rides))
            cleanexit (msg + sprintf ": no portmeta field found for '%s'" pram)
        | Some ss -> atoi ss
        

// Generate IP block declaration and simple simulation models for on-chip RAMs and ROMs. 
// For RAMs, these will be replaced with a hardware macro (from cvgates.v) for real implementation.
// For ROMs, these contain initial statements and are design-specific and the model generated here is used, perhaps with several mirrors.
//    
let gec_cv_romram_ip_block ww stagename kkey descr no_ports portmeta_s signed constval rom_id_o latency props =
    let msg = "cv_romram_ip_block"
    let vd = 2
    let kind = [kkey]
    // It is a little odd that 'signed' is passed into a RAM generator that should generate the same RAM regardless of what is to be stored in it.
    // The reason is perhaps to help default presentation in display and debugging mechanisms.
    
    let ww = WF 3 msg ww "start"
    let externally_instantiatedf = false // This component definition is the same regardless of how we set this here.
    //dev_println (sprintf "ROM/RAM portmeta_s=%A" portmeta_s)
    let actuals_rides = portmeta_s
    // latency and no of ports are already inside the kkey but the other portmeta parameters new squirrelling. 
    let kind_vlnv = { g_canned_default_iplib with kind=kind }
    let romf = not_nullp constval
    let block_meld =
        if romf then
            let roms_have_ren = false // We should get this from a portmeta attribute.
            //let roms_have_ren = not_nonep(op_assoc g_romram_with_ren actuals_rides) ... unfinished
            //let suffix = sf "gec_cv_rom - suffix and contents cannot be conjured up!"
            gen_cv_ssrom_port_structure roms_have_ren latency (valOf_or_fail "no rom_id" rom_id_o)
        else
            gen_cv_ssram_port_structure latency no_ports


    let w = int32(lookup_i64 ww msg actuals_rides "DATA_WIDTH")
    let aw = int32(lookup_i64 ww msg actuals_rides "ADDR_WIDTH")
    let words = int64(lookup_i64 ww msg actuals_rides "WORDS")
    let lane_width = int32(lookup_i64 ww msg actuals_rides "LANE_WIDTH") // Must be same as DATA_WIDTH for now

    let prec = { signed=signed; widtho=Some w }
    // Port number to string - used as an extension of logical net names for multi-port devices.
    let portno_tagon baser pno =
        let ans =  if pno < 0 then baser else sprintf "%s_P%i" baser pno
        //vprintln 0 (sprintf "Portno tagon %i giving %s" pno ans)
        ans

    let actuals_rides =
        let digger (ss, default_perhaps__) =
            match op_assoc ss portmeta_s with
            | Some v -> v
            | None   -> "0"
        map digger block_meld.formals_rides

    let portmeta_h =
        let harden ss = xi_num64(int64(atoi ss)) // Yuck!
        map harden actuals_rides

    // Currently we have only one director for all ports - not the future case for domain crossing RAMs and FIFOs.
    let (ctrl, director) = gec_ctrl () // This should perhaps accept a +ve/-ve edge triggered arg and so on.
    let sq = if nullp portmeta_s then "" else sqfold (map snd portmeta_s) + sq_prec prec
    let definitional_ikey = definitional_ikey_alloc ww (if romf then "ASROM" else "CVRAM") kind_vlnv sq
    let ram_core_id = if romf then funique "RomData" else "RAMDATA_" + definitional_ikey
    let ii = { id=ram_core_id }:rtl_ctrl_t

    let postproc ppo ((clkoo, clknet)) lst =
        let apply_pip_bx x = x
        let apply_pip_x x = x        // No X_x markup likely so identity suffices
        let pp = if ppo=None then None else Some(valOf ppo, valOf ppo) // This wierd looking form is used to denote a collated FSM that can be factorised again by dividing by the states of the PC (as is done in nfsate below)
        let dclk = { director with clocks=[clkoo] }
        let seq8fun  (g0, l, r, def) = H2BLK(dclk,      SP_rtl(ii, [gen_XRTL_arc(pp, apply_pip_bx g0, l, apply_pip_x r)]))
        let comb8fun (g0, l, r, def) = H2BLK(director,  SP_rtl(ii, [XIRTL_buf(apply_pip_bx g0, l, apply_pip_x r)])) // Or use a lhs X_x
        let groom_seqcomb (s, c) = function
            | (1, g, l, d, defval)  -> ((g, l, d, defval)::s, c)
            | (0, g, l, d, defval)  -> (s, (g, l, d, defval)::c)
        let (seqs, combs) = List.fold groom_seqcomb ([], []) lst
        (map seq8fun seqs) @ (map comb8fun combs)



     
    let net_schema_formal = // actualized
        let flip = Some false //externally_instantiatedf
        let tops =  List.foldBack (buildportnet ww vd flip (kkey:string) definitional_ikey portmeta_s "" "" (Some prec)) block_meld.contact_schema []
        let gec_contacts (mgr_meld:mgr_meld_abstraction_t) cc =
            List.foldBack (buildportnet ww vd flip (kkey:string) definitional_ikey portmeta_s mgr_meld.mgr_name "" (Some prec)) mgr_meld.contact_schema cc
        List.foldBack gec_contacts block_meld.mgr_melds tops
        //tops
    let ram_data_array =
        let ats = [] // Pass on from orig instead here ?
        //dev_println (sprintf "ram_core_id=%s" ram_core_id)
        let it = xgen_bnet(iogen_serf_ff(ram_core_id, [words], 0I, valOf prec.widtho, LOCAL, prec.signed, None, false, ats, constval))
        it


        
    let (the_component, formals) =
        let formals_decls = net_schema_formal
        // pns must come first now owing to mgr tie up
        let gen_port_model (mgr_meld, pno) (gubbins_bev0, gubbins_nets0) = 
            vprintln 3 (sprintf "Making SROM/SRAM nets and simulation gubbins for : " + descr + " port " + i2s pno)
            let meld = mgr_meld
            let pno = if no_ports < 2 then -1 else pno  // Use port -1 for a single-ported device to indicate the one-and-only port.
            let pns = "" 
            let pi_name = if no_ports < 2 then "" else i2s pno
            let rdbus   = get_by_logical (definitional_ikey, block_meld.kinds) pi_name ("rdata" + pns) net_schema_formal  // For sim model internals
            let wrbus() = get_by_logical (definitional_ikey, block_meld.kinds) pi_name  ("wdata" + pns) net_schema_formal
            let adbus   = get_by_logical (definitional_ikey, block_meld.kinds) pi_name ("addr" + pns) net_schema_formal
            let wen() = get_by_logical (definitional_ikey, block_meld.kinds)   pi_name ("wen" + pns) net_schema_formal 
            let ren() = get_by_logical (definitional_ikey, block_meld.kinds)   pi_name ("ren" + pns) net_schema_formal 

            //let _ = vprintln 3 (sprintf "The SRAM nets for port %i  "  pn + descr +  " are : " + sfold netToStr contacts1)
            let rd0bus = if latency < 2 then None else Some(newnet_with_prec prec (portno_tagon (definitional_ikey + "_RDR") pno)) // Read pipeline reg.

            // Generate a (behavioural) simulation model - this will not be emitted in the RTL but is needed for diosim to work. Really this should take another route for a standard RAM but this is the correct root for custom ROMs.
            let rdops cc =
                match latency with
                    | 0 -> f_comb (X_true, rdbus, ix_asubsc ram_data_array adbus) :: cc // REN not factored in.  roms_have_ren should be considered too.
                    | 1 -> f_seq  (X_true, rdbus, ix_asubsc ram_data_array adbus) :: cc
                    | 2 -> f_seq  (X_true, valOf rd0bus, ix_asubsc ram_data_array adbus) ::
                           f_seq  (X_true, rdbus, valOf rd0bus) :: cc

            // Write op is always synchronous for a RAM but not present for a ROM.
            let wrop = if romf then [] else [f_seq(xi_orred(wen()), ix_asubsc ram_data_array adbus, wrbus())] 
            let clkp = posfind "L0284" (once "L0284clk" director.clocks) // TODO - needs be inside gen_portnets for future domain-crossing variants.
            let gubbins_bev1 = postproc None clkp (rdops wrop)
            let gubbins_nets1 = if nonep rd0bus then [] else [ valOf rd0bus ]
            let (glue_nets1, glue_bev1) = ([], [])
            (gubbins_bev1 @ gubbins_bev0, gubbins_nets1 @ gubbins_nets0)

        let descr = H2BLK(director, SP_comment descr) 
        let (gubbins_bev2, gubbins_nets2) = List.foldBack gen_port_model (zipWithIndex block_meld.mgr_melds) ([descr], [])

        let atts = [  Nap("delv", i2s latency); Nap("n_ports", i2s no_ports)] @ map (fun (a,b)->Nap(a,b))  portmeta_s

        let definition_ii = // Not an instance but a definition. We may generate several of these for a mirrored ROM but only one needs noting.
            { rez_definition_ii externally_instantiatedf kind with
                externally_provided= not romf
                in_same_file=        romf
            }

        let (formals_and_gubbins) = declsrez_formals "cvipgen" definitional_ikey ctrl "definition" externally_instantiatedf (block_meld.formals_rides, [], net_schema_formal, ram_data_array::gubbins_nets2)  
        let the_component = (definition_ii, Some(HPR_VM2({ g_null_minfo with name=kind_vlnv; atts=atts; squirrelled=sq }, formals_and_gubbins, [], gubbins_bev2, [])))
        (the_component, net_schema_formal)

    (kind_vlnv, portmeta_h, actuals_rides, the_component, net_schema_formal, block_meld)



//
// Generate IP block declaration and simple simulation models for the instantiated ALUs. This is put in the execs field as a high-level simulation model
// that will be replaced with a hardware macro for real implementation.
//    
let gec_cv_alu_ip_block ww oo kind_vlnv latency (prec:precision_t) variable_latencyf =
    let vd = 2
    let fp = prec.signed=FloatingPoint
    let kkey = hptos (kind_vlnv:ip_xact_vlnv_t).kind
    let wid = valOf_or prec.widtho 32 // Better to get this from meld.portmeta  but it is the same
    //let prec = { prec with widtho=Some wid } // Make default manifest
    let m = sprintf "start gen_alu_sim_model for ALU %A kind=%s fp=%A wid=%i latency=%i prec=%A" oo kkey fp wid latency (prec2str prec) 
    let _ = if nonep prec.widtho then sf(m + " null precision not allowed")
    let ii = { id=kkey }:rtl_ctrl_t
    let ww = WF 3 "gen_alu_sim_model" ww m
    let externally_instantiatedf = false // This component definition is the same regardless
    let (ctrl, director) = gec_ctrl () // This should perhaps accept a +ve/-ve edge triggered arg and so on.
    let block_meld = gen_cv_ALU_port_structure ww oo kkey prec latency variable_latencyf // Lookup via retrieve_abstractionDefinition instead of rezzing each time. (todo some day)
    let pi_name = ""
    let actuals_rides =
        let digger (ss, _) =
            match ss with
                | "RWIDTH"
                | "NWIDTH"
                | "DWIDTH"
                | "A0WIDTH" // All the same for now
                | "A1WIDTH" ->  xi_num wid
                | "trace_me"
                | _            ->  xi_num 0 
        map digger block_meld.formals_rides 
           
    let portmeta = block_meld.parameters // aka meld.portmeta
    let sq = if nullp portmeta then "" else sqfold (map snd portmeta) // portmeta squirrelling for easy lookup 
    let definitional_ikey = definitional_ikey_alloc ww "dkey" kind_vlnv sq  // We need a unique key for all nets owing to the memoizing heap system, but the RTL render will use instead the logical name for a contact, such as 'arg0' or 'NN'.
    //let _ = if Signed = FloatingPoint then muddy (m  + " FP sim model needed? ")     // TODO - Strange? These do seem to work.
    // Note that Signed is ignored here! This should be ok for a sim model since diosim will infer as needed (in most situations?).

    let net_schema_formal =
        let flattened_schema =
            let rmeld_contacts (mgr:mgr_meld_abstraction_t) cc =
                let boz8 method_meld cc =
                    cc
                mgr.contact_schema @ List.foldBack boz8 mgr.method_melds cc
                //sf (sprintf "only_meld: expected precisely one method group (and precisely one method in it) for '%s'" (vlnvToStr kind_vlnv))
            block_meld.contact_schema @ List.foldBack rmeld_contacts block_meld.mgr_melds []

        let flip = Some false // externally_instantiatedf
        List.foldBack (buildportnet ww vd flip (kkey:string) definitional_ikey portmeta "" "" (Some prec)) flattened_schema []

    let (sim_nets, sim_arcs)  =
        let (a0, a1) = if oo=V_divide || oo=V_mod then ("NN", "DD") else ("XX", "YY")     // All a bit long-winded!
        let rr = get_by_logical (definitional_ikey, block_meld.kinds) pi_name "RR" net_schema_formal 
        let a00 = get_by_logical (definitional_ikey, block_meld.kinds)  pi_name a0 net_schema_formal 
        let da01 = get_by_logical (definitional_ikey, block_meld.kinds)  pi_name a1 net_schema_formal
        let fail = get_by_logical (definitional_ikey, block_meld.kinds)  pi_name "FAIL" net_schema_formal
        let ans =
            match oo with
                | V_times   -> ix_times  a00 da01
                | V_mod     -> ix_mod    a00 da01
                | V_divide  -> ix_divide a00 da01
                | V_plus    -> ix_plus   a00 da01
                | other     -> sf (sprintf "gen_alu_sim_model unsupported ALU op=%A" oo)

        match variable_latencyf with
            | false -> 
                let gen_pipeline_reg pos = newnet_with_prec prec (definitional_ikey + sprintf "X%i_" pos + funique ("PR"))
                let indecies = [0..latency-2]
                let pipeline = map gen_pipeline_reg indecies
                let gen_pipeline_assign (n, reg) =
                    let lhs = reg
                    let rhs = if n=0 then ans else pipeline.[n-1]
                    gen_XRTL_arc(None, X_true, lhs, rhs)

                let outfall = if latency=1 then ans else hd(rev pipeline)
                let gubbins =
                    [
                        gen_XRTL_arc(None, X_true, fail, xi_zero);            
                        gen_XRTL_arc(None, X_true, rr, outfall);
                    ]
                let gubbins = map (gen_pipeline_assign) (List.zip indecies pipeline) @ gubbins
                (pipeline, gubbins)

            | true -> // Multiply and add are commonly FL so will not come this way.
                let req = get_by_logical (definitional_ikey, block_meld.kinds) pi_name "req" net_schema_formal
                let ack = get_by_logical (definitional_ikey, block_meld.kinds) pi_name "ack" net_schema_formal
                let nets = [] // [dd1; busy]
                let arcs =
                    [ gen_XRTL_arc(None, X_true, rr, ans);
                      gen_XRTL_arc(None, X_true, ack, req);   // A fixed delay of 1 is too simplistic a model.
                    ]
                // We could simulate the time needed for VL long division by shifting left until d>n and then right again. 

                (nets, arcs)

    let gubbins_bev2 = [ H2BLK(director, SP_rtl(ii, sim_arcs)) ]

    let definition_ii = // Not an instance but a definition. We may generate several of these for a mirrored ROM but only one needs noting.
        rez_definition_ii externally_instantiatedf kind_vlnv.kind

    let atts = [ Nap("delv", i2s latency); ] @ map (fun (a,b)->Nap(a,b))  portmeta // kind is not needed here!
    
    let (formals_and_gubbins) = declsrez_formals "cvipgen" kkey ctrl "definition" externally_instantiatedf (block_meld.formals_rides, actuals_rides, net_schema_formal, sim_nets)  
    let the_component = (definition_ii, Some(HPR_VM2({ g_null_minfo with name=kind_vlnv; atts=atts; squirrelled=sq }, formals_and_gubbins, [], gubbins_bev2, [])))
    (portmeta, actuals_rides, the_component, net_schema_formal, block_meld)



// Floating point convertor IP block 'generator'  - they are canned in most respects actually.
let gec_cv_fpcvt_ip_block ww pto pfrom latency =
    let vd = 2
    let ww = WF 3 "gen_cv_fpcvt_ip_block" ww "start"
    let externally_instantiatedf = false // This component definition is the same regardless
    let kind_vlnv = gec_cv_fpcvt_name pto pfrom latency
    vprintln 2 (sprintf "gec_cv_fpcvt_ip_block kind=%s pto=%s  pfrom=%s  latency=%i" (vlnvToStr kind_vlnv) (prec2str pto) (prec2str pfrom) latency)
    let (fu_meld, mgr_meld, method_meld) = gen_cv_fpcvt_port_structure ww pto pfrom None
    let pi_name = mgr_meld.mgr_name
    let portmeta = fu_meld.parameters
    //let sq = if nullp portmeta then "" else sqfold (map snd portmeta) // portmeta squirrlling for easy lookup and unique net names
    let sq = "" // We do not need any squirrels for these at the moment (this must match restructure policy for diosim to rez correctly)
    let definitional_ikey = definitional_ikey_alloc ww "dkey" kind_vlnv sq  // We need a unique key for all nets owing to the memoizing heap system, but the RTL render will use instead the logical name for a contact, such as 'arg0' or 'NN'.
    let kkey = definitional_ikey

    let flat_meld_contact_schema =
        let gec_flat_contact_schema (mgr_meld:mgr_meld_abstraction_t) cc = mgr_meld.contact_schema @ cc
        List.foldBack gec_flat_contact_schema fu_meld.mgr_melds fu_meld.contact_schema

    let flat_net_schema_formal =
        let flip = Some false // externally_instantiatedf
        List.foldBack (buildportnet ww vd flip (kkey:string) definitional_ikey portmeta "" "" None) flat_meld_contact_schema []

    let result = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "result" (* method_meld.result_lname*) flat_net_schema_formal 
    let arg = get_by_logical (definitional_ikey, fu_meld.kinds)  pi_name "arg" flat_net_schema_formal
    let fail = get_by_logical (definitional_ikey, fu_meld.kinds)  pi_name "FAIL" flat_net_schema_formal
    let midpt = newnet_with_prec pto  (definitional_ikey + "midpt" (* + i2s (valOf pto.widtho) *))
    let ii = { id=definitional_ikey }:rtl_ctrl_t
    //let midpt2 = newnet_with_prec pto  (definitional_ikey + "midpt2")
    let sim_nets = [  midpt ]
    let (ctrl, director) = gec_ctrl () // This should perhaps accept a +ve/-ve edge triggered arg and so on.    
    let sim_arcs =   // Simulation model
        [
            gen_XRTL_arc(None, X_true, midpt, ix_casting None pto CS_preserve_represented_value arg);
            //gen_XRTL_arc(None, X_true, midpt2, midpt);
            gen_XRTL_arc(None, X_true, result, midpt);
            gen_XRTL_arc(None, X_true, fail,   xi_num 0); 
        ]
    let gubbins_bev2 = [ H2BLK(director, SP_rtl(ii, sim_arcs)) ]
    let atts = [ Nap("delv", i2s latency); ] @ map (fun (a,b)->Nap(a,b)) portmeta // kind is not needed here!
    let definition_ii = // Not an instance but a definition. We may generate several of these for a mirrored ROM but only one needs noting.
        rez_definition_ii externally_instantiatedf kind_vlnv.kind
    let (formals_and_gubbins) = declsrez_formals "cvipgen" kkey ctrl "definition" externally_instantiatedf (fu_meld.formals_rides, [], flat_net_schema_formal, sim_nets)  
    let the_component = (definition_ii, Some(HPR_VM2({ g_null_minfo with name=kind_vlnv; atts=atts; squirrelled=sq }, formals_and_gubbins, [], gubbins_bev2, [])))
    (kind_vlnv, portmeta, the_component, flat_net_schema_formal, fu_meld)  



// Create a simulation model for a static RAM.
// These are used as callbacks from diosim to custom create FUs.
//    
let dyno_rez_cv_ram ww (name:ip_xact_vlnv_t) rides signer no_ports latency  =

    let stagename = "diosim"
    let signed =
        match signer with // A RAM will ultimately not care about its content type which could be mixed.
        | "Signed"        -> Signed
        | "Unsigned"      -> Unsigned
        | "FloatingPoint" -> FloatingPoint
    let kkey = hptos name.kind
    let portmeta_s = rides
    let descr = "CV_RAM-autoip"
    let externally_instantiatedf = false
    let props = []
    let (kind_vlnv, portmeta_h, actuals_rides, the_component, net_schema_formal, meld) = gec_cv_romram_ip_block ww stagename kkey descr no_ports portmeta_s signed [] None latency props
    valOf(snd the_component) // Discard meaningless iinfo and optioning.


// Create a simulation model for a structural heap manager 
let dyno_rez_hpr_heapmanager ww rides =
    let vd = 2
    let ww = WF 2 "dyno_rez_hpr_heapmanager" ww "Start"
    let pi_name = ""
    let model = "T0" // for now
    let no = 0
    let (kind_vlnv, fu_meld, mgr_meld_, rv_prec, externally_instantiatedf, variable_latency_, port_schema) = rez_canned_heapmanager ww model pi_name
    let portmeta_s = rides
    let sq = if nullp portmeta_s then "" else sqfold (map snd portmeta_s) + sq_prec rv_prec
    let kkey = hptos (kind_vlnv:ip_xact_vlnv_t).kind
    let definitional_ikey = definitional_ikey_alloc ww kkey kind_vlnv sq

    let flat_meld_contact_schema =
        let gec_flat_contact_schema (mgr_meld:mgr_meld_abstraction_t) cc = mgr_meld.contact_schema @ cc
        List.foldBack gec_flat_contact_schema fu_meld.mgr_melds fu_meld.contact_schema

    let flat_net_schema_formal =
        let flip = Some externally_instantiatedf
        List.foldBack (buildportnet ww vd flip (kkey:string) definitional_ikey portmeta_s pi_name "" (Some rv_prec)) flat_meld_contact_schema []

    let (sim_nets, sim_arcs) =
        let arg0 = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name  "SIZE_IN_BYTES" flat_net_schema_formal
        let rr      = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "RESULT" flat_net_schema_formal 
        let reqrdy  = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "REQRDY" flat_net_schema_formal
        let req     = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "REQ" flat_net_schema_formal
        let ack     = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "ACK" flat_net_schema_formal
        let fail    = get_by_logical (definitional_ikey, fu_meld.kinds) pi_name "FAIL" flat_net_schema_formal  
        let arg1 = xi_string g_module_name_reflection
        dev_println (sprintf "HPR_HEAPMANGER  gubbins  %s := hpr_alloc(%s, %s)" (netToStr rr) (netToStr arg0) (netToStr arg1))
        if (true) then
            let arcs =
                [ gen_XRTL_arc(None, xi_orred req, rr, xi_apply (g_hpr_alloc_gis, [arg0; arg1]))
                  gen_XRTL_arc(None, X_true, ack, req)
                  gen_XRTL_arc(None, X_true, fail, xi_num 0)
                  gen_XRTL_arc(None, X_true, reqrdy, xi_num 1)
                ]
            ([], arcs)
        else
            let baser = muddy "HPR_HEAPMANGER_T0"   // - it could just call hpr_alloc actually as per above clause
            let nv = ix_plus baser (ix_lshift (ix_rshift Unsigned (ix_plus (xi_num 7) arg0) (xi_num 8)) (xi_num 8)) 
            let arcs =
                [ gen_XRTL_arc(None, xi_orred req, rr, baser)
                  gen_XRTL_arc(None, xi_orred req, baser, nv)  
                  gen_XRTL_arc(None, X_true, ack, req)  
                  ]
            ([baser], arcs)
    let ii = { id="hpr_alloc_model" }:rtl_ctrl_t
    let (ctrl, director) = gec_ctrl () // This should perhaps accept a +ve/-ve edge triggered arg and so on.        
    let gubbins_bev2 = [ H2BLK(director, SP_rtl(ii, sim_arcs)) ]
    let atts = []

    let definition_ii = // Not an instance but a definition. We may generate several of these for a mirrored ROM but only one needs noting.
        rez_definition_ii externally_instantiatedf kind_vlnv.kind
    let (formals_and_gubbins) = declsrez_formals "cvipgen" kkey ctrl "definition" externally_instantiatedf (fu_meld.formals_rides, [], flat_net_schema_formal, sim_nets)  
    let vm2 = (HPR_VM2({ g_null_minfo with name=kind_vlnv; atts=atts; squirrelled=sq }, formals_and_gubbins, [], gubbins_bev2, []))
    let the_component = (definition_ii, Some vm2)
    //(kind_vlnv, portmeta_s, the_component, net_schema_formal, meld)  
    vm2 // end of dyno_rez_hpr_heapmanager

let dyno_rez_cv_alu ww name rides varilatf latency signer operator widtho = // Also does RAMS etc, so a misnomer.
    let operator =
        match operator with
            | "DIVIDER"    -> V_divide
            | "ADDER"      -> V_plus
            | "MULTIPLIER" -> V_times 
            | "MODULUS"    -> V_mod      
            |  other       -> cleanexit(sprintf "Cannot autorez an ALU for operator '%s' in %s" other (vlnvToStr name))
            
    let prec = { signed=signer; widtho= widtho }
    let (portmeta, actuals_rides, the_component, net_schema_formal, meld) = gec_cv_alu_ip_block ww operator name latency prec varilatf
    valOf(snd the_component) // Discard meaningless iinfo and optioning.


// This FU lookup only copes with HPR native units.  diosim does not work for incremental compilations using AFUs (application-specific functional units).
    
let dyno_rez_ipblock ww failfun (name:ip_xact_vlnv_t) portmeta rides =
    let ww = WF 3 "dyno_rez_ipblock" ww (hptos name.kind)

    let signer() =
        match op_assoc "signed" portmeta with // Get from portmeta or kind name, but not from rides, normally... there's no really hard distinction here.
            | Some ss -> ss
            | None ->
                let _ = hpr_yikes(sprintf ":dyno_rez_ipblock: we prefer to have signed specified in the portmeta for a %s" (vlnvToStr name))
                "Unsigned"

    let failer(ss) = cleanexit (failfun() + sprintf ": cvipgen: failed to find definition of IP block %s.  Reason=%s" (vlnvToStr name) ss)
    //dev_println (sprintf "dynorez %A" name)
    let ans = 
        match hd name.kind with
            | "CV_SP_SSRAM_FL0" -> dyno_rez_cv_ram ww name rides (signer()) 1 0 
            | "CV_SP_SSRAM_FL1" -> dyno_rez_cv_ram ww name rides (signer()) 1 1 
            | "CV_SP_SSRAM_FL2" -> dyno_rez_cv_ram ww name rides (signer()) 1 2 
            | "CV_SP_SSRAM_FL3" -> dyno_rez_cv_ram ww name rides (signer()) 1 3 

            | "CV_2P_SSRAM_FL0" -> dyno_rez_cv_ram ww name rides (signer()) 2 0 
            | "CV_2P_SSRAM_FL1" -> dyno_rez_cv_ram ww name rides (signer()) 2 1 
            | "CV_2P_SSRAM_FL2" -> dyno_rez_cv_ram ww name rides (signer()) 2 2 
            | "CV_2P_SSRAM_FL3" -> dyno_rez_cv_ram ww name rides (signer()) 2 3 

            | "HPR_HEAPMANGER_T0" -> dyno_rez_hpr_heapmanager ww rides 

            | token when strlen token > 3 && token.[0..9] = "CV_FP_CVT_" -> // For example CV_FP_CVT_FL2_F64_I32
                let sl = split_string_at [ '_';  ] token.[10..]
                let prec_parse (ss:string) =
                    let w = atoi32 ss.[1..]
                    let signed = if ss.[0] = 'F' then FloatingPoint elif ss.[0] = 'U' then Unsigned else Signed
                    { widtho=Some w; signed=signed }
                match sl with
                    | [ latency ; pto ; pfrom ] ->
                        let latency = sscanf "FL%i" latency
                        let pto = prec_parse pto
                        let pfrom = prec_parse pfrom
                        let ww = WN "dyno_rez fpcvt block" ww
                        let (kind_vlnv, portmeta, the_component, net_schema_formal, meld) = gec_cv_fpcvt_ip_block ww pto pfrom latency 
                        valOf(snd the_component)


                    | other -> failer ("Bad floating point convert block name")
            | token when strlen token > 3 && token.[0..2] = "CV_" -> // For example [ "INT"; "FL1"; "MULTIPLIER"; "S"]
                let sl = split_string_at [ '_';  ] token.[3..]

                let ride_width =
                    match (op_assoc "RWIDTH" rides) with 
                        | None    -> None
                        | Some ss -> Some(int(atoi ss))
                let _ = vprintln 3 (sprintf "dyno_rez rides=%A portmeta=%A sl=%A" rides portmeta sl)
                match sl with
                    | "FP" :: latency :: operator :: signer :: []
                    | "INT" :: latency :: operator :: signer :: [] -> 
                        let width = if not_nonep ride_width then ride_width elif signer = "DP" then Some 64 elif signer = "SP" then Some 32 elif signer = "US" || signer = "S" then None else failer ("other widther")
                        let (varilatf, latency) =
                            if latency = "VL" then (true, 1)
                            else (false, sscanf "FL%i" latency)
                        let sign_er = if sl.[0] = "FP" then FloatingPoint elif signer = "S" then Signed elif signer = "US" then Unsigned else failer("bad sign/unsigned marker")
                        dyno_rez_cv_alu ww name rides varilatf latency sign_er operator width
                    | _ -> failer("Unrecognised or malformed CV_IP block name")

            | other -> failer("unknown/unrecognised IP block name")
                //            let fl = sscanf "CV_INT_FL%i_MULTIPLIER_S"

    let rep_enabled = true
    let _ =
        if rep_enabled then
            let stagename = "cvipgen"
            let m_fatal_marks = ref []
            let banner_msg = "cvipgen"
            let concisef = false
            let costsf = false
            let report_areaf = false
            if rep_enabled then
                let ii = { g_null_vm2_iinfo with definitionf=true; vlnv=name }
                let vd = (YOVD 2)
                youtln vd ("\n\ncvipgen-generated ipblock report: " + vlnvToStr name)
                vmreport ww "" vd m_fatal_marks concisef costsf report_areaf (ii, Some ans)
                ()

        
    ans



let install_canned_FU_metainfo() =
    let workitems =
        [
            g_hpr_alloc_gis
            g_hpr_flt2int32_fgis // Convert integer forms to/from floating point forms.
            g_hpr_flt2int64_fgis
            g_hpr_dbl2int32_fgis
            g_hpr_dbl2int64_fgis

            g_hpr_flt_from_int32_fgis // Further convert integer forms to/from floating point forms.
            g_hpr_flt_from_int64_fgis
            g_hpr_dbl_from_int32_fgis
            g_hpr_dbl_from_int64_fgis
        ]

    let install_canned_FU (fname, gis) =
        let is_loaded = false
        let is_cv_canned = true 
        let kind_vlnv =
            match gis.is_fu with
                | Some(vlnv, _) -> vlnv
                | None -> { vendor="Orangepath"; library="HPRLS_CANNED"; kind=[fname];  version="0.0"; }
        let pi_name = ""
        g_isloaded_db.add fname (is_loaded, is_cv_canned, kind_vlnv, pi_name, [])
        ()
    app install_canned_FU workitems
    ()


    

// Start-up code - was not being run so now called from opath manually. 
(* let _ = 
    vprintln 2 "Start install_canned_FU_metainfo"
    install_canned_FU_metainfo()
    vprintln 2 "Finished install_canned_FU_metainfo"
    ()
*) 


(* eof *)
