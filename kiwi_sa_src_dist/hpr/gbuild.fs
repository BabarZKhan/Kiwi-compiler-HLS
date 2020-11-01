(*
 *  $Id: gbuild.fs,v 1.24 2013-05-25 00:14:35 djg11 Exp $
 *
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
 * (C) 2003-17 DJ Greaves. 
 *
 * NEW: This file contains synthesis assist for the VM2 run-time system, including start/stop control for virtual machines and fault/debug wiring.  It should make sure 'big-bang' start up code is complete before accepting commands and so on.
 *
 * This file implements the implicit semantics of the HPR VM2 when they need to be made explicit for RTL or SystemC or other output.
 
 * OLD: Gate builder for the hexp_t type.  Generates Verilog RTL type though, so no good for SystemC output as gate list: still pandex does all the work.
//
// (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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
 *
 *
 *)

module gbuild

open Microsoft.FSharp.Collections
open System.Collections.Generic

open hprls_hdr
open moscow
open yout
open verilog_hdr
open meox
open plot
open verilog_render
open abstract_hdr
open abstracte

// Active pattern that can be used to assign values to symbols in a pattern
let (|Let|) value input = (value, input)


type layout_zone_t = // Layout unit of real estate
    {     
          loid:        loid_id_t           // Layout area identifier
          parent:      layout_zone_t option
          m_sons:      layout_zone_t list ref
          m_units:     (verilog_unit_t * (int * int) option ref) list ref // Leaf cells with optional layout (rel within zone).
          area:        si_area_t option ref          // Total area, sons plus local unit instances.
          dims:        (int * int) option ref  // Dimensions of this zone.
          position:    (int * int) option ref  // When there is a concrete layout : rel within parent zone.

    }

let g_layout_regions = new OptionStore<loid_id_t, layout_zone_t>("g_layout_regions")


// Lowest common parent (LCP) layout estimation cache.
let g_lcp_cache = new Dictionary<(loid_id_t * loid_id_t), layout_zone_t option>()

type unitsd_t = ListStore<string, verilog_unit_t>


// Equation-to-logic dynamic programming cache:
type eql_dp_t = Dictionary<int * precision_t, v_exp_t>

// RTL layout out estimation: primary structures:
// component -> zone * abscissa * area * (net * absicissa) list 
// net -> input/src_component * static_time * length * net delay * receiving * component list


// Collection of nets that are read-only memories (ROMs) whose initialisation tables must be rendered.
type rom_inits_t = OptionStore<int, net_att_t * v_exp_t * int>  // v_exp_t is verilog-specific - redo please.

type directorate_synth_t =
    {
        clknet:                    edge_t option
        //rstnet:                    hexp_t option
        int_run_enable:            hexp_t option  // A local broadcast net for local abend detection. Only one regardless of clock domains? Will cause timing issues.
        domain_cen:                hbexp_t list   // Clock enable that includes those from the director and int_run_enable when present.

        handshake_links:           (hexp_t * hexp_t * hexp_t * hexp_t * hexp_t option * hexp_t option) option  // Start/stop connection to handshakes
    }

type gatelevel_mux_collate_t = ListStore<v_exp_t, (hbexp_t * directorate_synth_t option * hbexp_t * hbexp_t)>  // A list of (lhs, dir, guard, value) pairs for each net-level assignment. Can also be used for abend codes and so on...

type layout_prefs_t =
    | LAYOUT_lcp
    | LAYOUT_disable
    | LAYOUT_random
    | LAYOUT_constructive

type gbuild_prefs_t =
     {        
        keep_pli:          bool  // Do not discard PLI calls (such as $display)
        keep_waypoints:    bool  // Do not discard waypoints - the ASCII ones, not the manual ones.
        vnl_synthesis:     bool   // is used infact
        reset_mode_:       string  // Get straight from director now
        gatelib:           string option   // Generate a net-level circuit with no RTL if this is non-None.
        layout_enable:   layout_prefs_t    // Enable layout delay estimation based on an actual layout.

        // comb_delayo: int option; // Add a delay to combinational logic: the LCP estimator overrides this (check?). - Move to gbuild_prefs please??
     }


type built_directors_t = OptionStore<string, hexp_t list * hexp_t list>

type eqToL_t = // This is output language agnostic, like all code in gbuild.
    {  
                 name:                      string
                 dir:                       directorate_t
                 sdir:                      directorate_synth_t option
                 m_nets:                    hexp_t list ref 
                 m_resetgrds:               hbexp_t list ref
                 cen_factor:                bool  // Wheter to use clock-enable flip-flops.     
                 unitsd:                    unitsd_t // Searchable list used for oldgate subexp sharing.
                 kogge_stone_threshold:     int // Smaller than this use ripple carry.
                 netinfo_dir:               netinfo_dir_t
                 token:                     string
                 built_directors:           built_directors_t
                 comb_delayo:               int option
                 dp:                        eql_dp_t
                 prefs:                     gbuild_prefs_t
                 m_verilog_strings:         (string * (string * net_att_t option)) list ref
                 gatelevel_mux:             gatelevel_mux_collate_t // Collate all rhs writing expressions for a net (or regfile/RAM)
                 rom_inits:                 rom_inits_t
                 m_units: (verilog_unit_t * (int * int) option ref) list ref // Linear list (for those not allocated to a layout_zone, plus layout.
                 m_kpp_nets:                (hexp_t * hexp_t) option ref // Performance predictor waypoint outputs.
    }

let new_cvtToRtl_DP() = new eql_dp_t()

// cf dirToStr - this addresses the directorate_synth_t type.
let sdirToStr dir =
    (if nonep dir.clknet  then "NoClock" else abstracte.edgeToStr (valOf dir.clknet)) +
    "" // incomplete!
    
let get_sdirectorate_extra_nets sdir = // Nets beyond those from get_directorate_nets
    let n1 = 
        match sdir.handshake_links with
            | None -> []
            | Some (fsm_idle_link, fsm_ending_link, req, ack, reqrdy_o, ackrdy_o) ->
                [ fsm_idle_link; fsm_ending_link ]

    let gx = function
        | None     -> []
        | Some net -> [net]
    (gx sdir.int_run_enable) @ n1 // @ ... clock and reset -- currently fetched from core director
    
let netgut = function
    | X_bnet ff -> ff
    | other -> sf("netgut other " + sprintf "%A" other)
    
//let g_reset_vnet()  = gec_V_NET -1 (def(xi_blift(default_reset_f())))
let g_tnow_vnet(n)  = gec_V_NET -1 (netgut(vectornet_w("$time", 32)))
let g_undef_vnet  = V_X
let g_vfalse = V_NUM(1, false, "b", xi_num 0) // These are in verilog_hdr too
let vtrue()  = V_NUM(1, false, "b", xi_num 1)



let vnetToStr = function // Concise alternative?
    | V_NET(ff, _, -1) -> "net " + netToStr(X_bnet ff)
    | V_NET(ff, _, n) -> "NSEL" + netToStr(X_bnet ff) + "[" + i2s n + "]"
    | other -> sprintf "vnetToStr??? %A" other

let vdeclToStr = function //  Hier form here please
    | V_NET(ff, _, _) -> "net " + netToStr(X_bnet ff)
    //| VD_ARRAY(ff, length, width) -> "array " + netToStr(X_bnet ff) + sprintf "[%i:0]" (length-1L) + sprintf "[%i:0]" (width-1)
    //| VD_NET(ff, _, n) -> "NSEL" + netToStr(X_bnet ff) + "[" + i2s n + "]"
    //| VD_BUS(ff, w) -> "bus " + netToStr(X_bnet ff) + "[" + i2s (w-1) + ":0]"
    //| (_) -> "vnetToStr???"



let funique_net (p:eqToL_t) prec ss =
    let gid = funique ss
    let net = newnet_with_prec prec gid
    let ff =
        match net with
            | X_bnet ff -> ff
            | _ -> sf "L161 netgut"
    //let n = netgen_serf(g, [], 1I, 1, Unsigned)
    //let net = X_bnet nn
    mutadd p.m_nets net // parent must also add me to p.netinfo_dir.
    let r = V_NET(ff, ff.id, -1)
    (gid, r, net)

let log_net_w (p:eqToL_t) (id, w, range) =
    mutadd p.m_nets id (* (V_NETDECL(id, w, range, VNT_WIRE)) *)
    ()


// Linear lookup ?  Quadratic run time!
let oldgate (qP:eqToL_t) (kind:verilog_cell_kind_t, conts) = // Look up in flat gate list and reuse, regardless of how far away (for now)
    let lst = qP.unitsd.lookup kind.name
    let rec oldg = function
        | [] -> None
        | V_INSTANCE(_, kind', iname, _, VDB_group(_, VDB_actual(_, d)::q)::_, ats)::_ when (q=conts) -> Some d
        //| V_INSTANCE(_, kind', iname, _, V_IA_associative cts, ats)::tt -> muddy "assoc lookup of o/p net"
        | _::tt -> oldg tt
    oldg lst


// Net use noter for layout wiring length estimation only.
// For an area estimate we can use the logic_cost costvec_entry_t mechanisms.
let record_net_use driverf (rvu:layout_zone_t, p:eqToL_t) xg net contact_no =
    match net with
    | VDB_formal(V_NUM _)      -> ()
    | VDB_formal(V_NET(ff, _, n))  ->
        let id = if n<0 then ff.id else ff.id + "." + i2s n // TODO abstract repeated code
        let (found, netinfo) = p.netinfo_dir.TryGetValue(id)
        //vprintln 0 ("Rec use " + id + " found=" + boolToStr found)
        let netinfo =
            if found then netinfo
            else
                let netinfo =
                    {
                        net=         V_NET(ff, ff.id, n)
                        driver=      ref None
                        net_static_time= ref None
                        phy_data=    ref None
                        uses=        ref []
                    }
                p.netinfo_dir.Add(id, netinfo)
                netinfo
        //if vd>=4 then vprintln 4 ("Record net use driverf=" + boolToStr driverf + " for " + id)
        if driverf then netinfo.driver := Some(xg, rvu.loid)
                   else mutadd netinfo.uses (xg, rvu.loid, contact_no)

    | other ->
        let _ = vprintln 3 (sprintf "Ignored in record_net_use: %A" other)
        () // other net use ignored (only suitable for net-level RTL estimation...


// The rvu is a hierachic layout area approximator, based on summing the area of the children.
let create_rvu id parent =
    let ans = 
      {
       parent=    parent
       dims=      ref None
       position=  ref None
       area=      ref None // Not computed yet
       m_units=   ref []
       loid=      [funique id]
       m_sons=    ref []
      }
    g_layout_regions.add ans.loid ans
    ans


// Nest a new layout region under a parent region.
let new_rvu id = function
    | None -> None
    | Some parent ->
        let ans = create_rvu (htos (id::parent.loid)) (Some parent)
        mutadd parent.m_sons ans
        Some ans

let tokenhint (p:eqToL_t) arg t0 t1 =
    match arg with
        | _ ->
           {
               p with token=t0+t1;
           }


let base_cell() =
    let ans =
        { name= "base_cell";
          dims= (0, 0);
          delays= [];
        }:verilog_cell_kind_t
    in ans

let g_CVINV = { base_cell() with
                  name = "CVINV";
                  dims=(5,10);
                  delays=[(1,[2])];
              }
let g_CVBUF = { base_cell() with
                  name = "CVBUF";
                  dims=(5,10);
                  delays=[(1,[2])];
              }
let g_CVBUFIF1 = { base_cell() with
                     name = "CVBUF";
                     dims=(7,10);
                     delays=[(1,[2;3])];
                 }
let g_CVDFF = { base_cell() with // see cvgates.v for reference.
                  name = "CVDFF";
                  dims=(25,10);
                  delays=[(1,[2;5])]; // q depends on clk and ar
              }

let g_CVAND2 = { base_cell() with
                   name = "CVAND2";
                   dims=(10,10);
                   delays=[(1,[2;3])];                
               }
let g_CVAND3 = { base_cell() with
                   name = "CVAND3";
                   dims=(13,10);
                   delays=[(1,[2;3;4])];                
               }
let g_CVAND4 = { base_cell() with
                   name = "CVAND4";
                   dims=(16,10);
                   delays=[(1,[2;3;4;5])];                
               }
let g_CVAND5 = { base_cell() with
                   name = "CVAND5";
                   dims=(19,10);
                   delays=[(1,[2;3;4;5;6])];                
               }
let g_CVXOR2 = { base_cell() with
                   name = "CVXOR2";
                   dims=(10,10);
                   delays=[(1,[2;3])];                
               }
let g_CVOR2 = { base_cell() with
                  name = "CVOR2";
                  dims=(10,10);
                  delays=[(1,[2;3])];                
               }
let g_CVOR3 = { base_cell() with
                  name = "CVOR3";
                  dims=(13,10);
                  delays=[(1,[2;3;4])];                
               }
let g_CVOR4 = { base_cell() with
                  name = "CVOR4";
                  dims=(16,10);
                  delays=[(1,[2;3;4;5])];                
               }
let g_CVOR5 = { base_cell() with
                  name = "CVOR5";
                  dims=(19,10);
                  delays=[(1,[2;3;4;5;6])];                
               }
let g_CVMUX2 = { base_cell() with
                   name = "CVMUX2";
                   dims=(20,10);
                   delays=[(1,[2;3;4])];                
               }



let newgate rides (rvu:layout_zone_t, (pp:eqToL_t)) (kind:verilog_cell_kind_t, conts) =
    let og = oldgate pp (kind, conts) // generic DP should do this now ? DP
    if not_nonep og then valOf og
    else
        let (ns, r, net) = funique_net pp g_bool_prec (pp.token)
        let wrap nn = VDB_actual(None, nn)
        let rides = map wrap rides
        let r = wrap r        
        let rvu = if pp.prefs.layout_enable <> LAYOUT_disable then valOf (new_rvu ns (Some rvu)) else rvu 
        let i = "i" + ns

        let m = { g_null_db_metainfo with kind="gbuild-newgate"; pi_name=i }  // On a local declaration, the pi_name can reasonably be the instance name of the component the nets connect to. But on formal declarations, the pi_name is only needed when the component has multiple ports of the same kind.
        let xg = V_INSTANCE(rvu.loid, kind, i, rides, [VDB_group(m, r::conts)], [])
        //if vd>=5 then vprintln 5 ("New gate " + kind.name + " " + i + " : " + sfold vToStr conts) 
        mutadd rvu.m_units (xg, ref None) 
        pp.unitsd.add kind.name xg
        record_net_use true (rvu, pp) xg r 0
        let rec rnu contact_no = function
            | [] -> ()
            | h::t ->
                let _ = if kind=g_CVDFF && (contact_no=3 || contact_no>=5) then () // Do not record CLK or RESET wiring (assume separate metal layer).
                        else record_net_use false (rvu, pp) xg h contact_no
                rnu (contact_no+1) t
        let _ = rnu 2 conts
        let wrap = function
            | X_bnet ff ->
                //let f2 = lookup_net2 ff.n
                VDB_actual(None, V_NET(ff, ff.id, -1))
            | _ -> sf "L330"
            
        muddy "V_NET(net, -1)"

let wrap vnet = VDB_actual(None, vnet)
    
let gbuild_not (rvu, p) aA =
    match aA with 
    | V_NUM(_, _, _, n) ->
        let vn = function
            | (None) -> sf("vgen not constant: " + xToStr n)
            | (Some true) -> xi_num 0
            | (Some false) -> xi_num 1
        V_NUM(1, false, "b", vn(xi_monkey n))

    | V_NET(id, _, n) -> newgate [] (rvu, tokenhint p aA (id.id+(if n>=0 then "_" + i2s n else "")) "bar") (g_CVINV, [ wrap aA ])
    | other -> newgate [] (rvu, p) (g_CVINV, [ wrap other ])


let gbuild_and p = function
    | (V_NUM(_, _, _, n), x) -> if (xi_monkey n = Some false) then g_vfalse else x
    | (x, V_NUM(_, _, _, n)) -> if (xi_monkey n = Some false) then g_vfalse else x
    | (x, y) -> if x=y then x else newgate [] p (g_CVAND2, map wrap [x; y])



let gbuild_mux2 p = function
    | (V_NUM(_, _, _, n), t, f) ->
        if xi_monkey n=Some false then f 
        elif xi_monkey n = Some true then t
        else sf("gbuild_mux2: " + xToStr n)
    | (g, t, f) -> newgate [] p (g_CVMUX2, map wrap [g; t; f])
 


let gbuild_or2 p = function
    | (V_NUM(_, _, _, n), x) -> if (xi_monkey n = Some true) then vtrue() else x
    | (x, V_NUM(_, _, _, n)) -> if (xi_monkey n = Some true) then vtrue() else x
    | (x, y) -> if x=y then x else newgate [] p (g_CVOR2, map wrap [x; y])

let gbuild_andl (fv2, rvu, p) lst = // Make multi-input wide gate with maximum fanin.
    let invf = ref false
    let trim arg c =
        match arg with
        | V_NUM(_, _, _, n) -> if (xi_monkey n = Some false) then (invf := true; [g_vfalse]) else c
        | x -> singly_add x c
    let lst' = List.foldBack trim lst [] 
    if !invf then g_vfalse
    else
    let edict lst =
        let l = length lst
        let xg = if l=2 then g_CVAND2
                 elif l=3 then g_CVAND3
                 elif l=4 then g_CVAND4
                 elif l=5 then g_CVAND5
                 else sf "gate fanin exceeded"
        newgate [] (rvu, p) (xg, map wrap lst)
        
    let max_fanin = 5 // Also the max in reimporter.
    let l = length lst'
    if l = 0 then vtrue()
    elif l = 1 then hd lst'
    else
        let rec split p c = function // Split into wide gates: naive linear form: balanced tree would be better of course but output will be to a logic synthesiser when we are not running our own local citical path heuristic estimates.
            | [item] when p = max_fanin-1 -> edict (rev(item::c))
            | h::t when p = max_fanin-1 ->
                let tail = split 0 [] (h::t)
                edict (rev(tail::c))
            | h::t -> split (p+1) (h::c) t           
            | [] -> edict (rev c)
        split 0 [] lst'


let gbuild_orl (fv2, rvu, p) lst = // Make multi-input wide gate with maximum fanin.
    let taut = ref false
    let trim arg c =
        match arg with
        | V_NUM(_, _, _, n) -> if (xi_monkey n = Some true) then (taut := true; [vtrue()]) else c
        | x -> singly_add x c
    let lst' = List.foldBack trim lst [] 
    if !taut then vtrue()
    else
    let edict lst =
        let l = length lst
        let xg = if l=2 then g_CVOR2
                 elif l=3 then g_CVOR3
                 elif l=4 then g_CVOR4
                 elif l=5 then g_CVOR5
                 else sf "gate fanin exceeded"
        newgate [] (rvu, p) (xg, map wrap lst)
        
    let max_fanin = 5 // Also the max in reimporter.
    let l = length lst'
    if l = 0 then g_vfalse
    elif l = 1 then hd lst'
    else
        let rec split p c = function // Split into wide gates: naive linear form: balanced tree would be better.
            | [item] when p = max_fanin-1 -> edict (rev(item::c))
            | h::t when p = max_fanin-1 ->
                let tail = split 0 [] (h::t)
                edict (rev(tail::c))
            | h::t -> split (p+1) (h::c) t           
            | [] -> edict (rev c)
        split 0 [] lst'


let vwidth = function
    | V_NET(ff, _, n)-> ewidth "vwidth" (X_bnet ff) // net was always one bit originally !  TODO
    | other ->
        let _ = vprintln 0 ("+++ vwidth other " + vnetToStr   other)
        in None


//let kogge_stone alst blst =
//    let 
   
     
let gbuild_deqd oo (fv2, rvu, p) (l, r) = // Make multi-input wide gate with maximum fanin.
    let _ = lprintln 3 (fun ()->"gbuild deqd " + vdopToS oo + " gate: l=" + vnetToStr l + ", r=" + vnetToStr r)
    let wl = vwidth l
    let wr = vwidth r    
    let m = match (wl, wr) with
               | (None, None)   -> 32 // Default (bad case really)
               | (Some x, None) ->  x
               | (None, Some x) ->  x
               | (Some x, Some y) -> (if x>y then x else y)

    let x,y =  l,r
    let g_CVDEQD = { base_cell() with name = "CVDEQD"; dims = (5*m, 10) } // need to define the delays=
    let ans = newgate [ V_NUM(m, false, "d", xi_num m) ] (rvu, p) (g_CVDEQD, map wrap [x; y])               
    ans
    
let vgen_xor_bin p = function
    | (V_NUM(aw, signed,  _, a), V_NUM(bw, _, _, b)) -> V_NUM(max aw bw, signed, "d", ix_bitxor a b)
    | (x, y) -> newgate [] p (g_CVXOR2, map wrap [x; y])

let gbuild_xor p = function
    | (V_NUM(_, _, _, n), x) -> if (xi_monkey n = Some true) then gbuild_not p x else x
    | (x, V_NUM(_, _, _, n)) -> if (xi_monkey n = Some true) then gbuild_not p x else x
    | (a, b) -> vgen_xor_bin p (a, b)

let lowest_common_parent l r =
    let key = (l.loid, r.loid)
    let (found, ov) = g_lcp_cache.TryGetValue key
    if found then ov
    else
        let rec scanup2 v f =
            if v.loid=f.loid then Some f
            elif f.parent=None then None
            else scanup2 v (valOf f.parent)
        let rec scanup1 v =
            let ans = scanup2 v r
            in if ans<>None then ans
               elif v.parent=None then None
               else scanup1 (valOf v.parent)
        let ans = scanup1 l
        let _ = g_lcp_cache.Add(key, ans)
        ans



let len_to_delay length = (length) * 1 // For now


let lcp_compute_wire_len (ni:netinfo_t) =
    let m = "Estimate  net length using LCP for " + vnetToStr ni.net
    let _ = vprintln 3 (m)
    let regions = if nonep !ni.driver then [] else [snd(valOf !ni.driver)]
    let regions = regions @ map f2o3 (!ni.uses)
    let regions =
        let spis x cc =
            match g_layout_regions.lookup x with
                | None -> cc
                | Some v -> v::cc
        List.foldBack spis regions [] // FSharp has a ToList we should use for this?
    let rec reduce = function
        | [] -> None 
        | [item]  -> Some item
        | a::b::c ->
            let q = lowest_common_parent a b
            reduce (if q=None then b::c else (valOf q)::c)
    match reduce regions with
        | None -> vprintln 3 (m + " no common region!")
        | Some rvu ->
            match !rvu.area with
                | None ->
                    let _ = vprintln 3 ("wire_length: no area set")
                    ()
                | Some area ->
                    let length = System.Convert.ToInt32(0.2 * sqrt(System.Convert.ToDouble area))
                    let delay = len_to_delay length 
                    let _ = vprintln 3 (m + " length=" + i2s length + " delay=" + i2s delay)
                    let _ = ni.phy_data := Some(length, delay) 
                    ()



// Simple Verilog area tally function: a quick run, sufficient for LCP estimator, used before doing any detailed layout. 
let rec rvu_tally units (rvu:layout_zone_t) =    
    let a = map (rvu_tally units) !rvu.m_sons
    let leaf_area (cc:si_area_t) (xg, phy) =
        match xg with
        | V_INSTANCE(loid, kind, i, rides, contacts_, ats_) ->
            let a = fst kind.dims * snd kind.dims
            int64 a + cc
        | _ -> cc

    let simple_area = List.fold leaf_area 0L (!rvu.m_units) // unit area for each leaf!
    units := !rvu.m_units @ !units
    let tot = List.fold (fun c n -> n+c) simple_area a
    rvu.area := Some tot
    vprintln 3 ("rvu area tally " + sfold (fun x->x) rvu.loid + " sons=" + i2s(length !rvu.m_sons) + ", units=" + i2s(length !rvu.m_units) + ", excess area=" + i2s64 simple_area + ", total_area=" + i2s64 tot)
    tot


// constructive layout: insertion sort tally of neighbour zones
let find_best_neighbour (netinfo_dir:netinfo_dir_t) contacts =

    let tally_contact_loid tallies (u, loid, contact_no) =
        let rec scan_tally tl = function
            | (loid', count)::t when loid=loid' -> (rev tl, count+1, t)
            | h::t -> scan_tally (h::tl) t
            | [] -> (rev tl, 1, [])
            
        let (pre, count, post) = scan_tally [] tallies
        let rec insert = function // Keep in decending count
            | (l, c)::tt when c > count -> (l,c)::(insert tt)
            | other -> (loid,count)::other
        insert pre@post



    let findbest tallies contact =
        match contact with
            | V_NET(f, _, n) ->
                let key = if n<0 then f.id else f.id + "." + i2s n // TODO abstract me
                let (found, ni) = netinfo_dir.TryGetValue(key)
                if not found then tallies
                else List.fold tally_contact_loid tallies !ni.uses        
            | _ -> tallies
    let tallies = List.fold findbest [] contacts

    let rec find_unplaced = function
        | [] -> []
        | (loid, count)::tt ->
            match g_layout_regions.lookup loid with
                | None -> find_unplaced tt
                | Some rvu ->
                    let sons = !rvu.m_units // dealing only with leav lvu's we think
                    let unplaced_sons = List.filter (fun (x, phy) -> !phy=None) sons
                    if unplaced_sons=[] then find_unplaced tt else (unplaced_sons)

    let rec find_pref (xs, ys, n) (loid, count) =
        match g_layout_regions.lookup loid with
            | None -> (xs, ys, n)
            | Some rvu ->
                let sons = !rvu.m_units // dealing only with leav lvu's we think
                let ave (xs, ys, n) (x, phy) =
                    match !phy with
                        | None -> (xs, ys, n)
                        | Some(x, y) -> (xs+x, ys+y, n+1)                     
                List.fold ave (xs, ys, n) sons

    let next = find_unplaced tallies
    let (prefx, prefy, n) = List.fold find_pref (0, 0, 0) tallies
    let hx = (if n>0 then prefx/n else 0) 
    let hy = (if n>0 then prefy/n else 50*16)  // 50 is half the ordinate space - use approx/2
    let hy = (hy/16)*16
    in map (fun x -> (x,(hx,hy))) next

let vchspace = 10
let hchspace = 4


let rec v_netsof arg cc =
    match arg with
        | VDB_formal n
        | VDB_actual(_, n)  -> n::cc
        | VDB_group(_, lst) -> List.foldBack v_netsof lst cc


//
// Do constructive placement.
//
#if SPARE_COPY
let cons_layout_v0_ ww vd msg (top_rvu:layout_zone_t, netinfo_dir) =

    match  !top_rvu.area with
        | None ->
            vprintln 1 (msg + ": cons_layout: no area set")
            ()
        | Some area ->
            let approx = System.Convert.ToInt32(1.2 * sqrt(System.Convert.ToDouble(area)))
            let pos = ref (100, 100, 0) // x, y, maxy

            let ymax = (2048*16)
            let abscissas = Array.create ymax 0 // Fill depth in each cell row

            let rec place (rvu:layout_zone_t) =
                app (fun x -> place2 [] [(x, (50,50*16))]) (!rvu.m_units)
                app place (!rvu.m_sons)


            and place1 revwork work = // place1 ignores preferences
                match work with
                    | [] -> if revwork<>[] then place1 [] (rev revwork)
                    | ((m:verilog_unit_t, phy), prefy)::t when !phy<>None -> place1 revwork t
                    | ((m:verilog_unit_t, phy), prefy)::t ->
                        let (x,y,maxy) = !pos
                        phy := Some(x+1, y+2)
                        //let _ = placement.Add(m, (x+1,y+2))
                        match m with
                            | V_INSTANCE(loid, kind, iname, rides, contacts, ats_) ->
                                let x' = fst kind.dims + x + hchspace
                                let maxy' = if snd kind.dims > maxy then snd kind.dims else maxy
                                let _ = vprintln 4 ("Placed1 " + iname + " at " + i2s x + "," + i2s y)
                                let _ = if (x' > approx)
                                        then pos := (0, y+maxy + vchspace, 0)
                                        else pos := (x', y, maxy')                            
                                    // let _ = d.oblong (x) (y) (x+fst kind.dims) (y+snd kind.dims) 0

                                let nw = find_best_neighbour netinfo_dir (List.foldBack v_netsof contacts [])
                                place1 (nw @ revwork) t

            and place2 revwork work =
                match work with
                    | [] -> if revwork<>[] then place2 [] (rev revwork)
                    | ((m:verilog_unit_t, phy), prefy)::t when !phy<>None -> place2 revwork t
                    | ((m:verilog_unit_t, phy), (prefx, prefy))::t ->
                        match m with
                            | V_INSTANCE(loid, kind, iname, rides, contacts, ats_) ->
                                let rec zigzag pol delta last =
                                    let y = if pol then prefy + delta else prefy - delta
                                    let x = if y <0 || y >= ymax then 100 * approx else abscissas.[y]
                                    let err = abs(x-prefx) + abs(y-prefy)
                                    //let _ = vprintln 3 ("pref is " + i2s prefx + "," + i2s prefy + " x=" + i2s x + " test y=" + i2s y + " err=" + i2s err)
                                    let next n = if pol then zigzag false (delta+16) n else zigzag true delta n
        //                            if (y > 200*16) then 
                                    if (x <= approx && y >= 0)
                                    then match last with
                                          | None -> next(Some(x, y, err))
                                          | Some (xp, yp, errp) when err < errp -> next(Some(x, y, err))
                                          | Some (xp, yp, errp) -> // Go back and use last if error has grown.
                                              let _ = phy := Some(xp+1, yp+2)                    
                                              let x' = fst kind.dims + xp + hchspace
                                              Array.set abscissas yp x'
                                              (xp, yp)
                                    else next last
                                let (x,y) = zigzag true 0 None
                                if vd>=5 then vprintln 5 ("Placed2 " + iname + " at " + i2s x + "," + i2s y)
                                // let _ = d.oblong (x) (y) (x+fst kind.dims) (y+snd kind.dims) 0
                                let nw = find_best_neighbour netinfo_dir (List.foldBack v_netsof contacts [])
                                place2 (nw @ revwork) t
            place top_rvu
#endif


// Constructive placer
// Why do we have a v1 and v2 version?
//            
let make_cons_layout_v1 ww vd (top_rvu:layout_zone_t) netinfo_dir =

    if nonep !top_rvu.area then
        vprintln 1 (sprintf "make_cons_layour: No top-level area set")
        ()
    else
    let approx = System.Convert.ToInt32(1.2 * sqrt(System.Convert.ToDouble(valOf_or_fail "no top-level area set" (!top_rvu.area))))
    let pos = ref (100, 100, 0) // x, y, maxy

    let ymax = (2048*16)
    let abscissas = Array.create ymax 0 // Fill depth in each cell row
    
    let rec place (rvu:layout_zone_t) =
        app (fun x -> place2 [] [(x, (50,50*16))]) (!rvu.m_units)
        app place (!rvu.m_sons)

        
    and place1 revwork work = // place1 ignores preferences
        match work with
            | [] -> if revwork<>[] then place1 [] (rev revwork)
            | ((m:verilog_unit_t, phy), prefy)::t when !phy<>None -> place1 revwork t
            | ((m:verilog_unit_t, phy), prefy)::t ->
                let (x,y,maxy) = !pos
                phy := Some(x+1, y+2)
                //let _ = placement.Add(m, (x+1,y+2))
                match m with
                    | V_INSTANCE(loid, kind, iname, rides, contacts, ats_) ->
                        let x' = fst kind.dims + x + hchspace
                        let maxy' = if snd kind.dims > maxy then snd kind.dims else maxy
                        if vd>=5 then vprintln 5 ("Placed1 " + iname + " at " + i2s x + "," + i2s y)
                        let _ = if (x' > approx)
                                then pos := (0, y+maxy + vchspace, 0)
                                else pos := (x', y, maxy')                            
                            // let _ = d.oblong (x) (y) (x+fst kind.dims) (y+snd kind.dims) 0

                        let nw = find_best_neighbour netinfo_dir (List.foldBack v_netsof contacts [])
                        place1 (nw @ revwork) t

    and place2 revwork work =
        match work with
            | [] -> if revwork<>[] then place2 [] (rev revwork)
            | ((m:verilog_unit_t, phy), prefy)::t when !phy<>None -> place2 revwork t
            | ((m:verilog_unit_t, phy), (prefx, prefy))::t ->
                match m with
                    | V_INSTANCE(loid, kind, iname, rides, contacts, ats_) ->
                        let rec zigzag pol delta last =
                            let y = if pol then prefy + delta else prefy - delta
                            let x = if y <0 || y >= ymax then 100 * approx else abscissas.[y]
                            let err = abs(x-prefx) + abs(y-prefy)
                            //vprintln 3 ("pref is " + i2s prefx + "," + i2s prefy + " x=" + i2s x + " test y=" + i2s y + " err=" + i2s err)
                            let next n = if pol then zigzag false (delta+16) n else zigzag true delta n
//                            if (y > 200*16) then 
                            if (x <= approx && y >= 0)
                            then match last with
                                  | None -> next(Some(x, y, err))
                                  | Some (xp, yp, errp) when err < errp -> next(Some(x, y, err))
                                  | Some (xp, yp, errp) -> // Go back and use last if error has grown.
                                      phy := Some(xp+1, yp+2)                    
                                      let x' = fst kind.dims + xp + hchspace
                                      Array.set abscissas yp x'
                                      (xp, yp)
                            else next last
                        let (x,y) = zigzag true 0 None
                        if vd>=5 then vprintln 5 ("Placed2 " + iname + " at " + i2s x + "," + i2s y)
                        // let _ = d.oblong (x) (y) (x+fst kind.dims) (y+snd kind.dims) 0
                        let nw = find_best_neighbour netinfo_dir (List.foldBack v_netsof contacts [])
                        place2 (nw @ revwork) t
    place top_rvu
    ()



// Create a random layout within region - this is crazy of course.
let make_random_layout ww (rvu:layout_zone_t) netinfo_dir =    

    if nonep !rvu.area then
        vprintln 1 (sprintf "make_cons_layour: No rvu area set")
        ()
    else

    let rec place rvu = 
        let a = map place !rvu.m_sons
        match !rvu.area with
            | None ->
                hpr_yikes (sprintf "layout-based area estimate: no area set in %A" rvu.loid) 
                ()
            | Some area ->
                let approx = System.Convert.ToInt32(1.2 * sqrt(System.Convert.ToDouble(area)))
                
                let rec rastergen (x, y, maxx, maxy) = function
                    | ((xdim, ydim), phy)::tt ->
                        let _ = phy := Some(x+2, y+2) // Offset within region
                        let x' = x+hchspace+xdim
                        let maxx' = if x'>maxx then x' else maxx
                        let maxy' = if ydim>maxy then ydim else maxy
                        if x' > approx 
                        then rastergen (0, y+maxy' + vchspace, maxx', 0) tt  // start new line
                        else rastergen (x', y, maxx', maxy') tt   // continue on this line
                    | [] -> (maxx, y+maxy)
                let qzone (a:layout_zone_t) =
                    let dims = if nonep !a.dims then sf ("make_random_layout: zone dimension not set (crazy):" + htos a.loid) else valOf !a.dims
                    let phy = a.position
                    (dims, phy)

                let qu = function
                    | (V_INSTANCE(loid, kind, i, rides, contacts, ats_), phy) -> (kind.dims, phy)
                let (x, y) = rastergen (0, 0, 0, 0) (map qu (!rvu.m_units) @ map qzone !rvu.m_sons)
                let _ = rvu.dims := Some(x, y)
                ()
    place rvu
    ()



let make_nominal_layout ww vd gprefs msg (rvu:layout_zone_t) netinfo_dir =    
    if gprefs.layout_enable = LAYOUT_random then make_cons_layout_v1 ww vd rvu netinfo_dir
    else make_random_layout ww rvu netinfo_dir


//
// We do not need a geometric layout to apply Rent's wiring estimates.
// We just need to know area of each part of the hierarchy.
let plot_layout(top_rvu:layout_zone_t, id, netinfo_dir) =
    let outcos = yout_open_out(id + ".eps")
    let edict ss = yout outcos ss
    let d = new cbg_ps_plotter_t(edict, id)
    let _ = d.preamble(1000,1000)

    let placement = new Dictionary<verilog_unit_t, (int * int)>()
    
    let brown = (44,44,0)
    let black = (0, 0, 0)
    let red = (255, 0, 0)
    let green = (0, 255, 0)
    let blue = (0, 0, 255)    

    let _ = d.text 20 20 ("HPR L/S plot: " + id) black

    let rec plot (x, y) (rvu:layout_zone_t) =
        let position = if !rvu.position=None then (0, 0) (*"zone offset not set :" + htos rvu.loid *) else valOf(!rvu.position)        
        let (x, y) = (x + fst position, y + snd position)
        let _ =
            match !rvu.dims with
                | None -> vprintln 3 ("zone dimension not set in plot:" + htos rvu.loid)
                | Some dims ->
                    let _ = d.oblong (x+2) (y+2) (x+fst dims) (y+snd dims) brown
                    ()
        //let _ = d.text x y ("Hello" + sfold (fun x->x) rvu.loid) black
        let _ = app (plotleaf (x, y)) (!rvu.m_units)
        let _ = app (plot (x, y)) (!rvu.m_sons)        
        ()
    and plotleaf (x, y) (m:verilog_unit_t, phy) =
        let (x,y) =
            match !phy with
                | None -> (x,y) // Will plot ontop without layout!
                | Some(xo,yo) -> (x+xo, y+yo)
        let _ =
            match m with
                | V_INSTANCE(loid, kind, iname, rides, contacts, ats_) ->
                    d.oblong (x) (y) (x+fst kind.dims) (y+snd kind.dims) black
                    if kind = g_CVDFF then d.oblong (x+2) (y+2) (x+8) (y+5)  black // Decorate the D-types
                    placement.Add(m, (x,y))
                    //let _ = find_best_neighbour netinfo_dir contacts
                    //let _ = d.text x y iname 0
                    ()

                | _ ->
                    d.oblong (x) (y) (x+2) (y+3) 0
                    ()
        ()
       
    let _ = plot (150, 100) (top_rvu) // Plot cell outlines

    let input_term_idx = ref 0 // We'll place some input pads around the edge (the old way!).

    let next_input_term(id) =
        let r = !input_term_idx
        let _ = input_term_idx:= r+1
        let x = 50
        let y = 80+r*40
        d.text 8 y id blue
        d.oblong (x-15) (y-15) (x+15) (y+15) green
        ((x, y), 1) // pad co-ordinates and abscissa (absolute)


    let tot = ref 0    
    let plot_net (ni:netinfo_t) = // also compute it's length. AND UPDATE netinfo when LCP not in use.

        let find_contacts cc (u, loid, contact_no) =
            let (f1, ov1) = placement.TryGetValue u            
            if f1 then (ov1, contact_no)::cc else cc

        let rec plot ((x0, y0), contact_no0) = function
            | ((x1, y1), contact_no1)::tt ->
                let contact_ordinate = 4
                let contact_abscissa n = 4 + 2 * n                
                let _ = d.line (contact_abscissa contact_no0 + x0)  (y0+contact_ordinate) (contact_abscissa contact_no1 + x1) (y1+contact_ordinate) red
                let rest = plot ((x1, y1), contact_no1) tt
                in rest + abs(x0-x1) + abs(y0-y1) // Manhatten distance.
            | [] -> 0

        let m = vnetToStr ni.net
        let a =
            if !ni.driver = None then [next_input_term(m)] // assume an input pad.
            else
                let (f1, driver_xg) = placement.TryGetValue (fst(valOf (!ni.driver)))
                in if f1 then [(driver_xg, 0)] else []
        let a = List.fold find_contacts a !ni.uses        
        let f ((x0,y0),_) ((x1,y1),_) = if x0=x1 then y0-y1 else x0-x1 // NOT A GOOD SORT FOR THIS
        // Want to scan along and find nearest neighbour from this starting point.
        let routing_sort lst = List.sortWith f lst
        let a = routing_sort a
        let net_len = if length a > 1 then plot (hd a) (tl a) else 0

        let delay = len_to_delay net_len 
        vprintln 3 (m + " length=" + i2s net_len + " delay=" + i2s delay) // overwrites LCP - todo compare values!
        ni.phy_data := Some(net_len, delay) 

        mutinc tot net_len
        ()
        
    for z in (netinfo_dir:netinfo_dir_t) do plot_net (z.Value) done        
    vprintln 2 ("Total wiring length " + i2s (!tot))
    let _ = d.postamble()
    yout_close(outcos)
    ()



#if TEMP
let kogge_stone xlst0 ylst0 =
     let (xlst, ylst) = zeropad xlst0 ylst0 // make both same length - need sign extend instead if signed.
     let l = length xlst

     let stf w = if w>=l then w/2 else stf (w*2) // Get logarithm and exp
     let strathe = stf 1 

     let rec ks_stage strath (x, y) = 
         if strath=1 then
	     let fcu (x, y)
	         let g = and(x, y)
		 let p = xor(x, y) // does OR work just as well here? - OR on lsb for carry in?
		 (g, p)
	     let ans = map fcu (zip2(x, y))
	     ans
         else 
             let (xl, xh) =  takedrop strath x
             let (yl, yh) =  takedrop strath y
	     let lower = ks_stage (strath/2) xl yl
	     let upper = ks_stage (strath/2) xh yh
	     let fcu ((gl, pl), (gh, ph)) =  
	         let g = or(and(gl, ph), gh)
		 let p = and(pl, ph)
		 (g, p)
	     let ans = map fcu (zip2(lower, upper)
	     ans @ lower

    let carries = ks_stage strathe (xlst, ylst)

    let sums = zip 

#endif


let dir_get_by_logical pi_name lname requiredf contacts =
    let eqeq_o lname = function
        | None    -> false
        | Some ln -> protocols.lname_eqeq lname ln
    let rec scan = function
        | (X_bnet ff)::tt ->
            let f2 = lookup_net2 ff.n
            if eqeq_o lname (at_assoc g_logical_name f2.ats) && (pi_name = "" || at_assoc g_port_instance_name f2.ats = Some pi_name) then Some(X_bnet ff)
            else scan tt
        | _::tt -> scan tt
        | [] ->
            if requiredf then
                let pos = "\nCandidates are " + sfoldcr_lite (fun (net) -> sprintf "  %s" (netToStr net)) contacts
                sf(sprintf "get_by_logical: formal director contact to module not found: pi_name=%s logical_name=%s"  pi_name lname + pos)
            else None
    scan contacts


// Form and process the net declarations and any controlling logic associated with a director.
             

// TODO: Call me from cpp_render and other output writers please
let gbuild_synth_director site (dir:directorate_t) =
    vprintln 3 (sprintf "gbuild_synth_director for %s:,  dir.style=%A" dir.duid dir.style)
    //If there are any external clk_enables. All must hold.
    let tag = sprintf "%s" dir.duid
    let (req_run_wanted, handshake_links) =
        match dir.hang_mode with
            | DHM_pipelined_stream_handshake(subschema_string, protocol_prams) ->
                let req = dir_get_by_logical tag "req" true dir.handshakes
                let ack = dir_get_by_logical tag "ack" true dir.handshakes
                //dev_println (sprintf "WANV: ack=%s" (netToStr (valOf ack)))
                let reqrdy = dir_get_by_logical tag "reqrdy" false dir.handshakes
                let ackrdy = dir_get_by_logical tag "ackrdy" false dir.handshakes
                let g_idle = ("hpr_fsm_idle_" + tag, g_bool_prec, LOCAL, Some 1)
                let g_ending = ("hpr_fsm_ending_" + tag, g_bool_prec, LOCAL, Some 0)
                let t = None
                (true, Some (xgen_gend t g_idle, xgen_gend t g_ending, valOf req, valOf ack, reqrdy, ackrdy))

            | _ -> (false, None)

    let int_run_enable = if dir.style=DS_normal || dir.style=DS_advanced || req_run_wanted then Some (xgen_gend (Some tag) g_dir_int_run_enable) else None
    //dev_println (sprintf "Int_run_enable site=%s name__=%s delaration for %A is %A"  site name__ dir.style int_run_enable)

    let dir_synth =
            {
                int_run_enable= int_run_enable
                domain_cen=    map xi_orred (dir.clk_enables @ (if nonep int_run_enable then [] else [ valOf int_run_enable]))
                clknet=        (if nullp dir.clocks then Some(E_pos g_clknet) else Some(hd dir.clocks)) // not really needed here since adds nothing to director
                handshake_links = handshake_links
            }
    dir_synth


let gbuild_director ww (debib, bxfun, xfun, pas) pp = 
    let dp = new_known_dp []
    let bdf barg = bxfun (WN "dir_resets" ww) pas dp barg
    let xdf arg  = xfun (WN "dir_resets" ww) pas (*(fv1, rvu, pp, aliases)*) (mine_prec g_bounda arg) arg
    let vnetx net = gec_V_NET -1 (netgut net)
    let (bb, cc) =  ([], [])    
    let sdir = valOf_or_fail "L1069" pp.sdir
    let (bb, cc, server_run_enable) =  
         match sdir.handshake_links with // Does this block start and stop according to handshake nets?
             | None ->                  (bb, cc, [])
             | Some (fsm_idle_link, fsm_ending_link, req, ack, reqrdy_o, ackrdy_o) ->
                 let server_run_condition = [ ix_or (xi_orred req) (xi_not (xi_orred fsm_idle_link))]
                 let arg = xi_blift(xi_orred fsm_ending_link)
                 let (bb, cc) = debib pp false [] None (1, ack, vnetx ack, xdf arg) (bb, cc) // Sequential assign for ack
                 //[ V_NBA(xdf ack, xdf arg)                 ]
                 (bb, cc, server_run_condition)
    let (dir_resetcode, arun_enable, bb, cc) =
        //dev_println (sprintf "dir_resets YES pp.name=%s dir.style=%A abend_register=%A" pp.name pp.dir.style  pp.dir.abend_register)
        //if not firstflag then  ([], [], []) // Crude way to supress repeating the directorate in a par block
        //else
        match pp.dir.abend_register with // TODO this code will disregard external clock enables when there is no abend_register, but that's no problem with the current DS_ ordering.
           | None -> ([], [], bb, cc)
           | Some abend_reg ->  // This functionality needs exporting and sharing over other output forms, like SystemC.
               let arun_enable = [ix_deqd abend_reg (xi_num 255)]  // Need to deassert clock enable on any fault syndrome being logged, including 0 the good exit code. The idle value is 255.

               // Use debib with an invalid arg to ensure reset code for abend syndrome register is created even if no abend sources exist in the design/domain.
               // Does not work, since resetcode is factored off on clause basis and does not go through if_share. Also if_share with a guard X_false wrongly, tacitly negates and then deletes it?
               //let (bb, cc) = debib fv1 p false [X_false] None (32, abend_reg, vnetx abend_reg, V_X 32) (bb, cc)                    
               let rval = xi_resetval abend_reg
               vprintln 3 (sprintf "Reset code for abend %A from net %s" rval (netToStr abend_reg))
               let resetcode = [ V_NBA(vnetx abend_reg, xdf rval) ]
               (resetcode, arun_enable, bb, cc)                    

    let run_enable_wanted = not_nullp server_run_enable || not_nonep pp.dir.abend_register || not_nullp pp.dir.clk_enables

    let (bb, cc) =
        if run_enable_wanted then
            let int_run_net = valOf_or_fail "int_run_enable" (sdir.int_run_enable) // This should exist when needed.
            let ext_enable = ix_andl (arun_enable @ server_run_enable @ map xi_orred pp.dir.clk_enables) // Wire in (form conjunction with) any external clk_enables. All must hold.
            debib pp true [] None (1, int_run_net, vnetx int_run_net, bdf ext_enable) (bb, cc)
        else (bb, cc)

    (dir_resetcode, bb, cc) // end of gbuild_director



            
let ensure_kpp_nets pex =
    // Create the performance predictor and progress monitoring nets if not already so done.
    match !pex.m_kpp_nets with
        | Some nets -> nets
        | None ->
            let wid = 8 * 80 // 80 character field
            let net0 = ionet_w("KppWaypoint0", 12, OUTPUT, Unsigned, [])
            let net1 = ionet_w("KppWaypoint1", wid, OUTPUT, Unsigned, [])  
            vprintln 3 (sprintf "Rezzed kpp waypoint net 0  " + netToStr net0)
            vprintln 3 (sprintf "Rezzed kpp waypoint net 1  " + netToStr net1)            
            pex.m_kpp_nets := Some(net0, net1)
            (net0, net1)


let get_gbuild_recipe_prefs stagename shortname c2 =
    let layout_enable =
        match control_get_s stagename c2 (shortname + "-layout-delay-estimate") (Some "disable") with
            | "disable"      -> LAYOUT_disable
            | "lcp"          -> LAYOUT_lcp
            | "constructive" -> LAYOUT_constructive
            | "random"       -> LAYOUT_random
            | other -> cleanexit("unexpected layout enable " + other)
    in
        {  
            keep_waypoints=   control_get_s stagename c2 (shortname + "-keep-waypoints") (Some "enable") = "enable"
            gatelib=          None // Not supported for SystemC I dont think.
            layout_enable=    layout_enable
            reset_mode_=      control_get_s stagename c2 (shortname + "-resets") (Some "none")
            vnl_synthesis=    control_get_s stagename c2 (shortname + "-synthesis") (Some "enable") = "enable"
            keep_pli=         control_get_s stagename c2 (shortname + "-keep-pli") (Some "enable") = "enable"
        }    

let rez_gbuild_recipe_prefs stagename shortname =
    [
        Arg_enum_defaulting(shortname + "-layout-delay-estimate", ["random"; "constructive"; "lcp"; "disable"], "lcp", "Insert net delay estimates using a layout method.");
        Arg_enum_defaulting(shortname + "-resets", [ "none"; "synchronous"; "asynchronous" ], "none", "What style of reset to insert in generated RTL");        
        Arg_enum_defaulting(shortname + "-keep-pli", ["enable"; "disable"], "enable", "Do not discard PLI calls (such as $display).");
        Arg_enum_defaulting(shortname + "-keep-waypoints", ["enable"; "disable"], "enable", "Export waypoint information at the top level.");
    ]



                        
// Process a tree of declarations
// This varies from db_flatten since we respect the logical names of ports and so on, letting these override the hpr internal net name.
let rec cvt_flatten (skip_params, max_depth) (mk_leaf, mk_group) io_select_ctrl lst =
    //dev_println (sprintf "cvt_flatten input %A" lst)
    let rec cvt depth ml arg cc =
        match arg with
        | DB_group(meta, lst) ->
            let is_pram = (meta.form=DB_form_pramor) // Those where meta.form=DB_form_meta_pram are not RTL rendered (but could usefully put in comments)
            let skip = meta.norender || // norender is used for internal nets in behavioural internal model of library cell, such as RAM.
                       (depth >= max_depth) ||
                       (is_pram && skip_params=Some true) ||
                       (not is_pram && skip_params=Some false)
            if skip then cc
            else
                let b = List.foldBack (cvt (depth+1) (meta::ml)) lst []
                mk_group(meta, b) @ cc


        | (Let None (assoctag, DB_leaf(Some kk, None))) // Formal with no assoc tag - None is bound to identifier assoctag with this 'Let' construct.
        | DB_leaf(assoctag, Some kk) -> // Formal/actual pair.  
            //dev_println (sprintf "cvt flatten encounter %s" (netToStr kk))
            if not_nonep io_select_ctrl && hexpt_is_io kk <> valOf io_select_ctrl then cc // Skip some decls if we just want locals or just want i/o nets.
            else
                let ftag = formaltag_handler false assoctag
                mk_leaf(ftag, kk) :: cc
    List.foldBack (cvt 0 []) lst []


// For pipelined accelerator mode, we need transformations that reduce the initiation interval.
let repipeline ww xarg =

    let vd = 0
    // Canned example
    let biquad_ex0 =
        let yy = vectornet_w1 "yy" 32 []
        let xx = vectornet_w1 "xx" 32 []

        let aa = vectornet_w1 "AA" 32 [Nap("coef", "true")]
        let bb = vectornet_w1 "BB" 32 [Nap("coef", "true")]        
        let eq0 = (yy, ix_plusl [xx; ix_times aa (xi_X 1 yy); ix_times bb (xi_X 2 yy)])

        [eq0]

    let msg = "repipeline"


    // An expression is schedullable when all the operators present have permissible fanin and latency.
    // Our algorithm uses a time datum of zero to represent the time the outputs are delivered and assumes that external inputs are available at any point in the past. Hence there is no limit to negative time subscripts on external inputs. Once schedulled, the overall latency is the absolute value of the most negative subscript used.  
    // For a causal rule, the time subscript of the output must be greater than or equal to any of its support. Where it is equal to that of a supporting expression, this denotes a combinational path.

    // Rewrite rules
    //  Push back rule: a negative integer is added to all time subscripts in the equation.
    //  Substituition rule
    //  Fanin rule:  an operator with too many inputs is replaced with a (possibly unbalanced) tree of operator nodes. 
    //  Modulo rule: all times are multiplied by a factor of two

    // Feedforward flows can always be trivially schedulled since support can always be made available earlier.  But there are still many valid designs that vary in latency and silicon use.
    // Feedback flows are the main challenge, since the latency of the available FUS may prohibit a recursive loop from being directly implementable.

    // A simple greedy algorithm operates, for one output variable, by applying the substitution rule on the  
    let mpx_latency = 2       // Available FU
    let adder_latency = 1

  
    let opfun arg nx bo xo orig child_node_data_lst = orig

    let lfun(pp, comp_gs) clkinfo arg rhs =
        ()

    let null_unitfn arg =
        //vprintln 0 ("Unitfn " + xToStr arg)
        ()

    let null_b_sonchange _ _ nn (aa, bb) =
        //vprintln 0 ("Null_b_sonchange " + xbToStr aa)
        bb

    let x_sonchange _ _ nn (aa, bb) = bb

    let (_, walker_ss) = new_walker vd false (true, opfun, (fun _ -> ()), lfun, null_unitfn, null_b_sonchange, x_sonchange)
    let tallies_o = None
    let wlk_plifun _ = ()
    
    let aux = (msg, walker_ss, tallies_o, wlk_plifun)
    let e0 x  = (mya_walk_it aux g_null_directorate xarg)     // We pass in null directorate
    ()



let greset (is_pos, is_asynch_, net) = if is_pos then xi_orred net else xi_not(xi_orred net)
(* eof *)


