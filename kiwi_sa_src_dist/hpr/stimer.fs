//
// CBG Orangepath.
// HPR L/S Formal Codesign System.
// (C) 2003/16 DJ Greaves. All rights reserved.
//
//
module stimer

let stimer_banner = "d320 $Id: stimer.fs $ "


(*
 *
 * stimer.sml - Static timing analyser
 *
 * (C) 2003-13 DJ Greaves. Cambridge. CBG.
 *
 * HPR L/S Core component.  
 *
 *)

open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open linepoint_hdr
open Microsoft.FSharp.Collections
open System.Collections.Generic

type static_times_t = Dictionary<xidx_t, walker_nodedata_t>

type seq_flag_t =
    | STA_seq
    | STA_clkinfo of edge_t list // Sequential but clock domain no yet stored.
    | STA_comb

type sequential_flags_t = Dictionary<xidx_t, seq_flag_t>

type st_t =
    {
        seqflags: sequential_flags_t;
        sta: static_times_t;
        maxtime:int;
    }


let opath_stimer_core ww0 vd title mc =
    let m = "opath_stimer_core " + title
    let ww = WF 3 m ww0 "start"
    let vdp = false
    let k1 =
        {
          maxtime= 1000;
          sta= new static_times_t();
          seqflags= new sequential_flags_t();          
        }

    let histogram = Array.create k1.maxtime 0

    let entries = ref 0
    let hwm = ref 0
    let log_stime = function
        | STIME_NODE(_, v) -> 
                if v >= k1.maxtime then sf (sprintf "maxtime static time exceeded %i" k1.maxtime)
                else
                let _ = Array.set histogram v (histogram.[v]+1)
                let _ = mutinc entries 1
                let _ = hwm := max !hwm v
                ()

    let sSS = 
        let null_unitfn arg =
            let _ = vprintln // 0 ("Unitfn " + xToStr arg)
            //let _ = rdytime arg
            ()


        let null_sonchange _ _ nn (a,b) =
            let _ = vprintln //0 ("Null_sonchange " + xToStr a + " -> " + qToStr b)
            b

        let null_b_sonchange _ _ nn (a,b) =
            let _ = vprintln //0 ("Null_b_sonchange " + xbToStr a)
            b

        let rec lfun_nettime_get m1 x =
            let n = x2nn x
            let oa =
                let (found, ov) = k1.sta.TryGetValue (n)
                if found then Some ov else None
            if oa <> None then
                let _ = vprintln 3 ("retrieved " + xToStr x + " result " + qToStr(valOf oa))
                valOf oa
            else
                match x with
                | X_pair(l, r, _) ->
                    let ans = lfun_nettime_get (fun () -> "pair") r // pairs have no nn for dp
                    let _ = vprintln 3 ("pair " + xToStr x + " result " + qToStr(ans))
                    ans
                | x when fconstantp x -> STIME_NODE(X_true, 0)
                | x -> sf (m1() + ": nettime_get not yet stored for " + xToStr x)

        let ensure_comb x n =
            let nn = abs n
            let (found, ov) = k1.seqflags.TryGetValue nn
            if found
                then match ov with
                        | STA_comb -> ()
                        | STA_clkinfo _
                        | STA_comb -> sf ("Net is already tagged sequentional " + xToStr x)

                else k1.seqflags.Add(nn, STA_comb)

        let ensure_seq x n =
            let nn = abs n
            let (found, ov) = k1.seqflags.TryGetValue nn
            if found
                then match ov with
                        | STA_seq -> ()
                        | STA_clkinfo _ -> ()
                        | STA_comb -> sf ("Net is already tagged combinational " + xToStr x)

                else k1.seqflags.Add(nn, STA_seq)


        let lfun (pp, g) (dir:directorate_t) lhs rhs =
            let m1() = ("lfun " + title + " lhs=" + xToStr lhs + " g=" + xbToStr g + " rhs=" + xToStr rhs)

            let recordx start_time  x =
                    match dir.clocks with
                        | [] ->
                            let _ = vprintln //0 (m1 + " comb logged: bnet: support ready at " + qToStr start_time)
//                            if ... if treated as zero delay then need another pass
                            let _ = ensure_comb x
                            ()

                        | elist ->
                            let _ = vprintln //0 (m1 + " seq logged: bnet: support ready at " + qToStr start_time)
                            let _ = log_stime start_time
                            let _ = ensure_seq x //<---------here we can log the clock domain in future but currently all logic is simply assumed to be in a zero skew single domain.
                            // Need to eval rdy time for each elist member
                            ()

            let vl_max = function
                | (STIME_NODE(vl, tl), STIME_NODE(vr, tr)) ->if (tl=tr) then STIME_NODE(ix_and vl vr, tl) elif tl>tr then STIME_NODE(vl, tl) else STIME_NODE(vr, tr) // idiom

            match lhs with
                | W_asubsc(X_bnet f, subsc, _) ->
                    let start_time = lfun_nettime_get m1 rhs
                    let subsc_time = lfun_nettime_get m1 subsc
                    let start_time = vl_max (start_time, subsc_time)
                    recordx start_time (X_bnet f)
                | W_asubsc(l, ss, _) -> sf ("stimer unsupported lhs: " + xToStr lhs)
                | X_bnet f ->
                    let start_time = lfun_nettime_get m1 rhs
                    recordx start_time lhs
                    
                | other -> sf ("static time anal lhs other " + xkey lhs + " " + xToStr lhs)
                    
        // Operator function:
        let opfun arg n bo xo _ soninfo2 =
            let s() = if xo=None then xbToStr(valOf bo) else xToStr(valOf xo)
            let _ = vprintln //0 ("Opfun " + s() + " acting on " +  sfold qToStr soninfo2)
            let nn = abs n
            let (found, ov) = k1.sta.TryGetValue nn
            if found then ov
            else
                let sonscan c = function 
                    | STIME_NODE(p,q) -> max c q
                    | _ -> c
                let _ =
                    if nullp soninfo2
                    then
                        match bo, xo with
                        | (None, Some (X_bnet f)) ->
                                       let _ = vprintln 3 ("Leaf sequential net: " + s()) // assume sequential on first pass
                                       let _ = ensure_seq (valOf xo) (x2nn(valOf xo))
                                       () //let _ = 
                        | (None, Some other) when fconstantp other->  ()
                        | (None, Some other) ->  vprintln 0 ("ignored s node with no sons " + s())                                   
                        | (Some other, None) when bconstantp other ->  ()
                        | (Some other, None) ->  vprintln 0 ("ignored b node with no sons " + s())                                                                      

                let args_rdy = List.fold sonscan (0) soninfo2
                let _ = vprintln //0 ("sons=" + sfold qToStr soninfo2 + "\nArgs ready for " + s() + sprintf "  %i " args_rdy)
// pass read or write info in
                let output_rdy = args_rdy + 1 // unit delay model
                vprintln 3 ("Timed " + s() + sprintf " result rdy %i " output_rdy)
                let rr = STIME_NODE(X_false, output_rdy) // The timeof_nodes first arg is a complete dummy when used by this static timer.
                let _ = k1.sta.Add(nn, rr)
                rr
        let (_, sSS) = new_walker vd vdp (true, opfun, (fun _ -> ()), lfun, null_unitfn, null_b_sonchange, null_sonchange)
        sSS

    let _ = walk_vm ww vdp (m, sSS, None, g_null_pliwalk_fun) mc

    let anal() =
            let timing_histogram() =
                if !entries = 0 then sf "no entries"
                else
                let _ = vprintln 0 (sprintf "Maximum static time %i out of %i" !hwm !entries)
                let m0 = List.fold (fun c s -> histogram.[s]+c) 0 [0..!hwm]
                let m1 = List.fold (fun c s -> s*histogram.[s]+c) 0 [0..!hwm]       
                let fmean = float(m1) / System.Convert.ToDouble(m0)
                let mean = m1/m0
                let _ = vprintln 0 (sprintf "Mean static time %f out of %i" fmean m0)
                let mx0 = List.fold (fun c s -> histogram.[s]+c) 0 [mean+1..!hwm]
                let mx1 = List.fold (fun c s -> s*histogram.[s]+c) 0 [mean+1..!hwm]       
                let fmeanx =
                    if mx0 > 0 then
                        let fmeanx = float(mx1)/float(mx0)
                        let _ = vprintln 0 (sprintf "Mean excess static time %f out of %i" fmeanx mx0)
                        fmeanx
                    else 0.0
                let _ = vprintln 0 (sprintf "CSV max,mean,excess,%i,%f,%f" !hwm fmean fmeanx)
                ()
            timing_histogram()

            vprintln 3 ("Static time anal done")
            ()
    let _ = anal()
    let ww = WF 3 m ww0 "done"
    mc 




let opath_stimer_vm ww op_args vms =
    let disabled = 1= cmdline_flagset_validate op_args.stagename ["enable"; "disable" ] 0 op_args.c3
    let _ = vprintln 1 (stimer_banner)
    let _ = 
        if disabled then vprintln 1 "Stage is disabled"
        else
            let vd = 3
            vprintln 1 "Starting stimer 1749";    
            let _ = map (opath_stimer_core (WN "stimer_core" ww) vd "plugin") vms
            ()
    vms // Output machine(s) same as input


let install_stimer() =
    let bev_argpattern =
        [
          Arg_enum_defaulting("stimer", ["enable"; "disable"; ], "enable", "Disable this stage");

        ]

    let _ = install_operator ("stimer",  "Static Timing Analyser", opath_stimer_vm, [], [], bev_argpattern)

    ()


(* eof *)
