(*
 * $Id: diosim.fs,v 1.75 2013-07-21 14:22:03 djg11 Exp $  
 *
 * HPR L/S simulator: all internal forms can be simulated with this simulator.
 *
 * diosim.sml - HPR L/S machine simulator and plotter. Creates vcds too.
 *
 * (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
 * Fsharp version (C) 2010 onwards.
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

As well as providing simulation output in VCD and console form, diosim can collect statistics and
help with profile generating.  However, it is fairly slow and it is best to collect profiles from
faster execution engines, such as via Verilator.
 


The statistics that diosim can collect range from net-level switching activity to higher-level statistics like
imperative DIC instructions executed, RTL sequential and combinational assignment counts.
 *
 *)
module diosim


//
//  Understanding sim trace levels: 
//    1 normal
//    4 Assigns are printed 
//    8 expression details are traced
//    10 everything


let g_exitpc = -100



open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics
open System.Text.RegularExpressions


open microcode_hdr
open protocols
open hprls_hdr
open abstract_hdr
open abstracte
open moscow
open yout
open plot
open meox
open opath_hdr
open opath_interface
open linprog
open hprxml


let m_exitflag = ref true;

let ansi_techno col = // Write coloured text to the console.
    let escape = System.Convert.ToChar 27
    let r = if col < 0
            then sprintf "%c[0m" escape 
            else sprintf "%c[01;3%im" escape (col % 7)
    r
    

(*
 * Diosim uses this boxing to denote simulation values on nets and in registers and RAMs etc..
 *)
type simval_t =
    | DN of int64   // integers (for faster than BigInteger common cases)
    | DS of string  // strings
    | DL of precision_t * BigInteger * meo_t // large integers with precision.
    | DX // dont know
    | DF of double // A float - not needed now DL has a precision_t annotation.


type vcd_netref_t = string



let dnToStr = function
    | DX   -> "X"
    | DS s -> s
    | DN n64 -> sprintf "%i" n64
    | DF flt -> sprintf "%A" flt
    | DL(w, bn, _) -> bn.ToString() // sprintf "%A" bn - printf adds a capital I giving "I33" etc.


let dnToStrx = function // debug output only.
    | DX     -> "X"
    | DS s   -> s
    | DN n64 -> sprintf "0x%x" n64
    | DF flt -> sprintf "%A" flt
    | DL(w, bn, _) -> "0x" + bix bn


let dio_lognot = function
    | (DX) -> DX
    | (DN n) -> DN(if n=0L then 1L else 0L)
    | _ -> sf "dio lognot"

type dio_instance_t = string list


type dio_edge_t = De_pos  | De_neg | De_any


type edge_det_t = dio_edge_t * simrec_t * simval_t ref

and binder_t = string -> simrec_t option

and pmim_t = dio_instance_t * minfo_t * vm2_iinfo_t * binder_t * string

    
//[<XmlAttribute("trocs")>]    
// A troc is a tracing event to be displayed in a plot or vcd.
and troc_t = {  v:      simval_t
                tsrc:   simrec_t
                timer:  int
                subs:   int64 
                //twidth: int option
              }

//[<XmlRoot("trocs")>]
and trocs_t = troc_t list

and simrec_t = // Simulation net meta info and current state record.
    {
        prefix:             dio_instance_t
        current:            simval_t ref 
        array:              simval_t ref array option                // Packed array form: uses refs for compatiblity with sparse dictionary form.
        sdict:              Dictionary<int64, simval_t ref> option   // Sparse form
        flags:              net_att_t option
        length:             int64 list // Dimension for an array
        comb:               (pmim_t * edge_det_t list * hexp_t) option ref  //  Edge detector list and driver for a combinatorial net. 
        //generation: int ref;  // Last comb update time for efficiency.
        onstack:            bool ref
        it:                 hexp_t
        nn:                 xidx_t
        profiling_database: profiler_t option
        encoding_cvt:       precision_t
        cll:                vcd_netref_t

    }


(*
 * A virtual machine does not have an address space or specific architecture, its just a s/w AST or h/w netlist.
 *)
and diosim_virtual_machine_t =
    {
        dir:            directorate_t
        stack:          int list ref
        pc:             int ref
        pcnet:          hexp_t
        pc_simrec:      simrec_t
        code:           hbev_t array
        len:            int            // Length of code array.
        minfo:          minfo_t
        prefix:         dio_instance_t
        serial:         string
    }

(*
 * A real machine is like a microprocessor, but it does not work on the binary
 * version of the machine code by default: it works on the icode_t assembly.
 *)
and real_machine_t =
    {
        symbols:        (string * int64) list
        reg_count:      int64
        regs:           simval_t ref array
        mem:            simval_t ref array
        sp:             int ref
        pc:             int ref
        pc_simrec:      simrec_t        
        pcnet:          hexp_t
        code:           icode_t array
        minfo:          minfo_t
        returner:       icode_t list ref
        regsbase:       hexp_t
        prefix:         dio_instance_t        
    }




and simulator_t = 
  | S_VM of diosim_virtual_machine_t   (* A dic form: just has a pc and a stack. *)
  | S_RM of real_machine_t             (* An asm that also has main memory, iospace, regs, etc. *)
  | S_FILLER

and profiler_t(id:string) = class

    let lastval: simval_t option ref = ref None
    let xitions = new OptionStore<simval_t * simval_t, int>("xitions")

    let dump_xition_report__ () =
        let _ = vprintln 3 (sprintf "profiler_t for %s" id)
        let repx (pre, post) count =
            vprintln 3 (sprintf "profile %s: xition density pre+post=%A count=%i " id (pre, post) count)
        for p in xitions do repx p.Key p.Value done

    member x.xml_report () =
        //let _ = vprintln 3 (sprintf "profiler_t reporting for %s" id)
        let ans = ref []
        let repx (pre, post) count =
            let dos v = XML_ATOM(sprintf "%s " (dnToStr v))
            let v = XML_ELEMENT("xition", [], [ dos pre; dos post; XML_ATOM(sprintf "%i " count) ])
            let _ = mutadd ans v
            vprintln 3 (sprintf "profile %s: xition density from/to= %s/%s count=%i " id (dnToStr pre) (dnToStr post) count)
        let _ = for p in xitions do repx p.Key p.Value done
        XML_ELEMENT("netflow", [("netid", id)], !ans)

    member x.update(v) =
        match !lastval with
            | None -> ()
            | Some ov ->
               match xitions.lookup(ov, v) with // Count number of transitions between each old and new value.
                    | None       -> xitions.add (ov, v) 1
                    | Some count -> xitions.add (ov, v) (count+1)
        lastval := Some v

end


let codepoint2Str = function
    | (mach_id, None) -> mach_id
    | (mach_id, Some n) -> sprintf "%s:%i" mach_id n



    
type simrecs_t   = Dictionary<dio_instance_t * xidx_t, simrec_t>

type standing_troc_t = Dictionary<vcd_netref_t, troc_t>

type selections_t = Dictionary<string, (int * int) list ref>

type diosim_stats_t =
    {
        static_rtl_statement_count:     int64 ref
        vm_instructions_run:            int64 ref
        rtl_instructions_run:           int64 ref
        rtl_cont_instructions_run:      int64 ref                
    }

type update_t =
    {
        subs:            int64                // Which location in an array *)
        done_already:    bool    
        loc:             simval_t ref         // Location  to assign
        update:          simval_t          // New value  to assign
        mask:            int64             // Mask value for bit inserts to DN - otherwise all of it
        simrec:          simrec_t option
        src_hexp:        hexp_t            // Assigned expression (for bit-width precision mining perhaps?)
        cll:             vcd_netref_t
    }

type heap_ptrs_t = Dictionary<string, int64 ref>

type diosim_env_t =
    {
        mvd:             int ref                 // Tracing level - can change dynamically.
        gully_simrec:    simrec_t option ref
        iospace:         simrec_t option []
        timescale:       int
        standing_troc:   standing_troc_t // Holds last value of a net.  What about arrays? They are not traced.
        simrecs:         simrecs_t
        techno:          bool
        plot:            string option
        check_db:        string option
        save_db: string option
        vcd: string option  
        title:          string 
        trace_regexs:   string list
        //loosevals: loosevals_t;
        outcos:         logger_t option
        gpmim:           pmim_t
        m_global_nets:   (string list * hexp_t) list ref
        sim_cycle_limit: int
        granularity:     int
        selections:      selections_t
        heap_ptrs:       heap_ptrs_t
        stats:           diosim_stats_t
        m_exit_report:   string ref
        //default_reset_simrec: simrec_t
        m_cll:          char list ref
        m_pendings:     update_t list ref // m_pendings - updates ready to be committed at end of delta cycle.
        m_e_trocs:        troc_t list ref // Earlier and later tracing events
        m_l_trocs:        troc_t list ref 

    }


// Constructor 1/2 for a troc (tracing event).
// troc = tracing event for plotting/vcd output.
let misc_trocgen (dD1:diosim_env_t) ef (timer, tsrc, vv) =  // tuppled since on a list
    let cll =
        match  tsrc:simrec_t with
            | simrec -> simrec.cll
            //| _ -> sf "diosim L305"
    let (found, ov) = dD1.standing_troc.TryGetValue cll
    if found && ov.v = vv then () // same as prev value - no transition to record
    else 
        let (troc:troc_t) =
            { timer=       timer
              v=           vv
              tsrc=        tsrc
              subs=        -1L
              //twidth =     Some(encoding_width net) // TODO compute once!
            }
        //dev_println (sprintf "New troc %A on " cll + (xToStr tsrc.it) + " new value=" + dnToStr vv)
        if found then ignore (dD1.standing_troc.Remove cll)
        dD1.standing_troc.Add(cll, troc)
        mutadd (if ef then dD1.m_e_trocs else dD1.m_l_trocs) troc



let update_trocgen dD1 ef timer (update:update_t) = 
    if update.subs < 0L then misc_trocgen dD1 ef (timer, valOf update.simrec, !update.loc)
    else
        let troc = // Supscripted troc (not for vcd output?)
                { timer=  timer
                  v=      !update.loc
                  //twidth= (if update.flo <> None then Some(encoding_width (X_bnet (valOf update.flo))) else None) // can do this lazy when
                  tsrc=   (if not_nonep update.simrec then valOf update.simrec else muddy (sprintf "whak for %A" update + " update. (if k.flo <> None then X_bnet (valOf update.flo) else gec_X_net "))
                  subs=   update.subs
                }
            //vprintln 0 ("update_trocgen New troc " + xToStr t.net + " value " + dnToStr t.v)
        mutadd (if ef then dD1.m_e_trocs else dD1.m_l_trocs) troc


type disp_t = { yofn:string; xs:int; ys:int; hscale:int; x0 :int; starttime:int; }

let black = "000000"

let g_diosim_global_prefix = [ "diosim_globals" ]

let rec dispmap (d:plotter_t) = function
    | ([], x, (wide, y0, hscale, yscale), VV, _, k) -> 
        let _ = d.text (x+2) (y0+(yscale / 3)) ("EOT " + dnToStr k) black
        ()
        
    | ((troc:troc_t)::t, x, (wide, y0, hscale, yscale), VV:disp_t, y', k) ->
        let M = (wide, y0, hscale, yscale)
        let k' = troc.v
        let time = troc.timer
        let subs = troc.subs
        let x' = VV.x0 + (time - VV.starttime) * VV.hscale 
        let Xs = VV.xs
        let hh =
            match k with
                | DX                   -> X_dontcare
                | DN 0L                -> X_false
                | DL(_, bn, _) when bn.IsZero -> X_false
                //         // TODO floating point trocs
                | _                    -> X_true
        if x > Xs then ()
        else
        let y1 = y0 + yscale / 2
        let y = 
                (* if (h=x_unit) then y' else  *)
                y0  + (if (hh=X_true) then yscale / 2 elif (hh=X_false) then 0 else yscale/4)
        let dc = if   hh=X_true     then "FF0000"
                 elif hh=X_false    then "00FF00" 
                 elif hh=X_dontcare then "100010" else "404040"

        let (riser, faller) =
                if subs >= 0L then (true, true)
                elif wide then (k<>k', k<>k') else (y>y', y<y')
        let _ = if riser
                then (* risers *)
                  (       
                        d.move x y0;
                        d.draw x (y0+1) "FF00FF";
                        d.draw (x+1) (y1-1) "FF00FF";
                        d.draw (x+2) y1 "FF00FF";
                        ()
                  )

        let _ = if (faller)
                then (* faller *)
                  (       
                        d.move x y1;
                        d.draw x (y1-1) "FF44FF";
                        d.draw (x+1) (y0+1) "FF44FF";
                        d.draw (x+2) y0 "FF44FF";
                        ()
                  )

        let _ = if wide && hh<>X_dontcare then (* Treads: x to x' *)
                        (d.move (x+2) y0; d.draw x' y0 dc;
                         d.move (x+2) y1; d.draw x' y1 dc;
                         ()
                        )
                elif subs < 0L then ignore(d.draw x' y dc)

        let _ = if x>Xs-15 then () else
                if subs >= 0L then ignore(d.text (x+2) (y0+(yscale / 3)) (dnToStr k + "/" + i2s64 subs) black)
                elif wide then ignore(d.text (x+2) (y0+(yscale / 3)) (dnToStr k) black)

(*      let _ = if (h=X_undef) then () 
                        else (d.move(x+2, y-1; d.draw(x', y-1, dc)
*)
        dispmap d (t, x', M, VV, y, k')


// Plot file: write lhs signal name key.
let rec render_signal_names(tracers, VV, pos, d:plotter_t) =
    let vd = -1
    if (pos >= length(tracers)) then ()
    else 
        let dd = (length(tracers)+1)
        let spacing = (VV.ys-30) / (if dd = 0 then 1 else dd)
        let (it:net_att_t, legend, trocs) = select_nth pos tracers
        let wide = not it.is_array && (it.rh <> 1I || it.rl <> 0I)
        let v = String.length(legend) * 8
        let y0 = 20 + pos*spacing
        if y0 > VV.ys-20 then sf("diosim off screen")
        (*      let _ = vprint 0((i2s pos) + " trace render\n") *)
        let x0 = VV.x0
        d.text (x0-v) (y0+10) legend "102210"
        let yscale = spacing
        d.move x0 y0
        if vd>=4 then vprintln 4 (i2s(length trocs) + " trocs for " + legend)
        dispmap d (trocs, x0, (wide, y0, VV.hscale, yscale), VV, y0, DX)
        render_signal_names(tracers, VV, pos+1, d) 





let dl64 lst =
    let rec s  = function
        | [] -> 0L
        | (h:int)::tt -> (System.Convert.ToInt64 h) + (4096L * s tt)
    let ans = s lst
    //let _ = vprintln 0 ("dl64 " + sfold i2s lst + " gave " + i2s64 ans)
    ans

let id_to_simreco (mapping:simrecs_t) (prefix, id) = // lookup net by id and prefix
    match lookup_net_by_string id with // First get xidx from ascii name.
    | None -> None
    | Some (X_bnet ff) ->
        let (found, ov) = mapping.TryGetValue((prefix, ff.n)) // Then the real simrec lookup
        if found then Some ov else None
        // OLD: if symbolic_check (explode id) then xi_bnum(atoi id) else


let id_to_simrec (mapping:simrecs_t) (prefix, id) = // lookup net by id and prefix
    match id_to_simreco mapping (prefix, id) with 
    | None -> sf (sprintf "diosim:No such net as prefix=%s  id=%s" (hptos prefix) id)
    | Some net -> net


(*
 * VCD - Verilog change dump: write out.
 *)
type vcd_writer (title:string) = class
    
    member x.dump ww dD1 traces trocs opfn =
        let opfn = if opfn = "" then "diosim.vcd" else opfn
        let outcos = yout_open_out opfn
        let _ = vprintln 1 ("Writing vcd simulation (verilog change dump file) filename=" + opfn)
        let chr c = System.Convert.ToChar(c:int)
        let ord c =
            let ba = System.BitConverter.GetBytes (c:char)
            System.Convert.ToInt32(ba.[0])


        let bx_array = Array.create 2049 "" // Canned strings of binary don't know vectors.
        let _ = Array.set bx_array 1 "bx"
        let bx_gen n =
            let rec ensure n =
                if bx_array.[n] <> "" then ()
                else (ensure (n-1); Array.set bx_array n (bx_array.[n-1] + "x"))
            if n > 2048 then sf "too wide vcd" else (ensure n; bx_array.[n])


        let rR ss = yout outcos ss
        rR("$comment written by HPR DIOSIM.  " + title + " $end\n")
        rR("$date " + timestamp(true) + " $end\n")     
        rR("$timescale 1ps $end\n")

        // Now we render more than one scope/upscope following the design hierarchy.
        let scopes_decls (prefix, contents) =
            rR(sprintf "$scope module %s $end\n" (hptos prefix)) // Whole prefix is listed linearly here rather than tree/nesting --- viewer tools may vary in needs?
            let rec netnames lst =
                match lst with
                | [] -> ()
                | (X_bnet f)::tt -> 
                    let h = hd lst
                    let simrec = id_to_simrec dD1.simrecs (prefix, f.id)
                    let width = encoding_width h
                    rR("$var wire " + i2s width + " " + simrec.cll + " " + vsanitize (xToStr h) + " $end\n")
                    netnames tt

            netnames contents
            rR("$upscope $end\n")
            ()
            
        app scopes_decls traces
        rR("$enddefinitions $end\n")
        rR("$dumpvars\n")

        let rec bn2sbin w bn = // .ToByteArray may be quicker!
            if w=0 then ""
            else
                let rem = ref 0I
                let q = BigInteger.DivRem(bn, 2I, rem)
                let s = if (!rem).IsZero then "0" else "1"
                let tl = bn2sbin (w-1) (q)
                tl + s
        let xnq w bn = if w>1 then ("b" + bn2sbin w bn +  " ") else (if bn.IsZero then "0" else "1")

        let rec i2sbin w (k:uint64) = if w=0 then "" else (i2sbin (w-1) (k / 2uL)) + (if k % 2uL = 0uL then "0" else "1")
        let vcdlog_as_bin w n = if w>1 then ("b" + i2sbin w (uint64 n) +  " ") else i2s64(n % 2L)    
        let rec binner w b = // Render string in binary with first character first.
            let odd v = (v % 2) = 1
            if w < 0 then ""
            else binner (w-1) (b/2) + (if odd b then "1" else "0")
        let vcd_rhs_encode w = function
            | DX -> if w=1 then "x" else bx_gen w + " "
            | DS ss  ->
                let rec qq = function
                    | [] -> ""
                    | h::tt -> binner 8 (int h) + qq tt
                "b" + qq (explode ss)

            | DL(_, bn, _) -> xnq w bn
            | DN n  -> vcdlog_as_bin w n
            | DF fv ->
                let float32_to_bits (f32:float32) =
                    let bytes = System.BitConverter.GetBytes f32
                    int64(System.BitConverter.ToUInt32(bytes, 0))
                let float64_to_bits (f64:double) =
                    let bytes = System.BitConverter.GetBytes f64
                    int64(System.BitConverter.ToUInt64(bytes, 0))
                if w=32 then vcdlog_as_bin w (float32_to_bits(float32 fv))
                elif w=64 then vcdlog_as_bin w (float64_to_bits fv)
                else sprintf "%f" fv // The best we can do!
    //      | (_) ->  sf("diosim: vcd_dump: Cannot vcd_rhs_encode further form.")



        let rec dump tnow = function
            | [] -> ()
            | h::tt -> 
                let tnow' = (h:troc_t).timer
                let (ticket, twidth) =
                    match h.tsrc with
                        | simrec ->
                            (simrec.cll, encoding_width simrec.it)
                        //| TS_net net ->                            (op_assoc net !splaymap, encoding_width net)
                        //| _ -> muddy "L521 other"
                let _ =
                    if tnow' > tnow then rR("#" + i2s tnow' + "\n")
                    elif tnow' < tnow then dev_println (sprintf "diosim: out-of-order VCD trocs %i %i" tnow' tnow)
                let _ = if ticket <> "" then rR(vcd_rhs_encode (twidth) (h.v) + (ticket) + "\n")
                dump tnow' tt

        dump 0 trocs
        yout_close outcos
        ()
end



// Plot format output
let diodisp title (id, H, tracers, VV:disp_t, diops) = 
    let outcos = yout_open_out(VV.yofn)
    let edict ss = yout outcos ss
    let d = new plotter_t(edict, "diosim")
    let _ = d.preamble(VV.xs, VV.ys)


    let graticule () =   // Draw vertical timing markers.
        let Xs = VV.xs
        let col = "dddddd"      
        let black = "000000"
        let gline x t = (d.move x 20; d.draw x  (VV.ys-20) col;
                     d.text (x-10) 17 (i2s t) black)
        let rec glines t =
            let x = VV.x0 + VV.hscale * t
            in if x>=Xs-20 then () else (gline x t; glines(t+5))
            in glines (VV.starttime)
    graticule ()
    
    let timenow = "H2 DIOSIM" (* Time.toString(Time.now()) *)
    d.text 15 (VV.ys-18) (title + ":" + VV.yofn + ":" + timenow) "101010" 
    render_signal_names(tracers, VV, 0, d)
    d.postamble()
    yout_close(outcos)
    //let s = Unix.execute("/homes/djg/d410/diogif/diogif", diops @ [ "-o", "t", VV.yofn]) 
    //let _ = (Unix.reap s)
    vprintln 3 ("Diogif plot has run 1")
    ()







(*-----------------------------------------------------------------------------
 * This is a real mutable simulator. It uses 'driver_for' to get values and a mutable environment.
 * NB: there is another symeval routine in hprls.sml and there is the romsey one in bisim.
 *
 *
 * Env is a 3ple..  The hardware is in the first list. 
 * Values that are evaluated are added into the second list - assumes no oscillating items.
 * Futures are placed in the 3rd list.
 *)


let myexp v n = 
    let rec e (v,n) = if n=0L then 1L else v*e(v, n-1L)
    e(v,n)
    // handle Overflow => raise sfault((i2s v) ^ " ^ " ^ (i2s n) ^ " my exp overflow")

let dn_masks = Array.ofList (map (fun x -> myexp 2L x) [0L..63L])

let dn_inv_masks = Array.ofList (map (fun x -> -1L - (myexp 2L x)) [0L..63L])


let g_diosim_clear_arrays = ref false
let g_diosim_clear_scalars = ref false



(*
 * Create and Initialise an array.  Associative dictionaries are used when length is unspecified.
 *
 * Some front ends generate zero length arrays and store in them.  This is silently ignored, only flagging an error
 * if the user tries to read from such.
 *
 *)
let dxarray_ctor m dims inits = 
    let vd = -1
    let n = int32(asize dims)
    let n_inits = length inits 
    let v0f p =
        if p >= n_inits then DN 0L  // Initial value: better as DX 
        else inits.[p]
    if n<0 then sf("negative array length: " + m)
    let n = if n=0 then ((if vd>=4 then vprintln 4 ("diosim: +++zero array length: " + m)); 1) else n
    let ival = if !g_diosim_clear_arrays then DN 0L else DX
    let rr = Array.create (int32 n) (ref ival) // Surely intrinsically mutable without a ref? All locations contain the same ref?
    let rec k p = if p<n then (Array.set rr p (ref (v0f p)); k(p+1))
    k 0
    rr

let gec_DL(prec, vv) = // if outside 64 bit two's complement then use BigInteger.
    if vv > 9223372036854775807I || vv < 9223372036854775808I then
        let meo =
            match gec_X_bnum(prec, vv) with
                | X_bnum(_, _, meo) -> meo
        DL(prec, vv, meo)
    else DN (System.Convert.ToInt64 vv) // prec lost, hmm!


type combinations_t  = Map<dio_instance_t * xidx_t, pmim_t * hexp_t list * hexp_t>

// ROMs have a list of constants, whereas scalars have perhaps one.
let diosim_constval (ff:net_att_t) =
    let inibaz = function
        | XC_bnum(w, bn, m) -> DL(w, bn, m)
        | XC_float32 fv     -> DF(double fv)
        | XC_double dv      -> DF dv
        | XC_string s       ->
            let _ = vprintln 3 (sprintf "diosim ignored string init on %s: other=%A" ff.id s)
            DX
        //| other -> sf(sprintf "poor constval form for %s: other=%A" ff.id other)
    map inibaz ff.constval

let char_list_inc dD1 =
    let chr c = System.Convert.ToChar(c:int)
    let ord c =
        let ba = System.BitConverter.GetBytes (c:char)
        System.Convert.ToInt32(ba.[0])

    let suc x = chr(ord(x) + 1)
    let rec clinc (h::t) = if h = 'z' && t = [] then [ 'a'; 'a'; ] elif h='z' then 'a' :: (clinc t) else (suc h) :: t
    let aa = !dD1.m_cll
    let _ = dD1.m_cll := clinc aa
    implode aa


(*
 * Create a record for holding mutable values of variables during simulation/interpreter runs.
 * An init given as an arg takes precedence over #init field in the net.
 *) 
let newsimrec (dD1:diosim_env_t) newinit prefix it =
    //vprintln 3 (sprintf "make simrec for " + netToStr it)

    let profile_pred (id:string) =
        let ans = id.Contains "pc"
        //vprintln 3 (sprintf "profile for net %s ?  ans=%A" id ans)
        ans
        
    match it with
    | X_net(id, _) ->
        let bl = 
           {
             encoding_cvt=       g_default_prec // Used for undeclared nets and perhaps time and so on.
             prefix=             prefix
             profiling_database= (if profile_pred id then Some(new profiler_t(id)) else None)
             sdict=              None
             array=None
             it=                 it
             nn=                 x2nn it
             current= ref (if !g_diosim_clear_scalars then DN 0L else DX)
             flags= None
             comb=ref None; // Needs be ref owing to circular datastructure
             //generation= ref 0
             onstack= ref false          
             length=             []
             cll=                char_list_inc dD1
           }
        bl
        
    | X_bnet ff ->
        let vd = 3 
        let id = ff.id
        let f2 = lookup_net2 ff.n
        let initer =
            match diosim_constval ff with
                | [] ->
                    match newinit with
                        | None ->
                            match at_assoc "init" f2.ats with
                                | None -> []
                                | Some i ->
                                    let tobn = BigInteger.Parse // TODO - constant inits larger than 64 bits
                                    [DN (atoi64 i)]
                        | Some x ->
                            if vd>=4 then vprintln 4 (id + ": new init =" + xToStr x)
                            [gec_DL({widtho=Some(encoding_width it); signed=Signed}, xi_manifest_int "diosim newinit" x)]
                | items -> items 

        let (aa, sparsed) =
            if not ff.is_array then (None,None)
            else
                if f2.length<>[g_unspecified_array] then (Some(dxarray_ctor id f2.length initer), None)
                else (None, Some (new Dictionary<int64, simval_t ref>()))
                

        //vprintln vd ((netToStr it) + " e newsimrec\n")
        vprintln 2 (sprintf "prefix=%s net=%s simrec created. Initial value for %s " (hptos prefix) (netToStr it) ff.id + " is " + sfold dnToStr initer) 
        let bl = 
           {
             cll=                char_list_inc dD1
             encoding_cvt=       snd(net_att_to_precision ff)
             prefix=             prefix
             profiling_database= (if profile_pred id then Some(new profiler_t(ff.id)) else None)
             sdict=   sparsed
             array=   aa
             it=      it
             nn=      x2nn it
             current= ref(if nullp initer then DX else hd initer)
             comb=    ref None
             flags=   Some ff
             onstack= ref false          
             length=  f2.length
           }

        if not_nullp initer then misc_trocgen dD1 false (0, bl, hd initer) // Trace initial value
        bl

let g_blank_update =
    {
        simrec=      None
        subs=        0L
        loc=         ref  DX
        update=      DX
        mask=        0L
        done_already=  true
        src_hexp=        X_undef
        cll=         ""
    }:update_t



let up64 (x:int) = System.Convert.ToInt64 x
let down64 (x:int64) = System.Convert.ToInt32 x

let rec gen_srec dD1 inits prefix net =
    // _ = vprintln 0 (sprintf "Generating simrec pass %i %s %s" pass (hptos prefix) (netToStr net))
    match net with  // Match all valid l-mode forms here.
        | W_asubsc(b, _, _) -> gen_srec dD1 inits prefix b
        | X_bnet ff ->
            let bl =
                let (found, ov) = dD1.simrecs.TryGetValue((prefix, ff.n))
                if found then ov
                else
                    //vprintln 4 (netToStr net + " generating simrec")
                    let init = op_assoc ff.id inits
                    let nv = newsimrec dD1 init prefix net
                    dD1.simrecs.Add((prefix, ff.n), nv)
                    //vprintln 3 (netToStr net + " generated simrec")
                    nv 
            bl

        | X_x(v, _, _) 
        | X_blift(W_bitsel(v, _, false, _)) -> gen_srec dD1 inits prefix v

        | X_net(id, meo) ->
            let bl =
                let (found, ov) = dD1.simrecs.TryGetValue((prefix, meo.n))
                if found then ov
                else
                    //vprintln 4 (netToStr net + " generating simrec")
                    let init = op_assoc id inits
                    let nv = newsimrec dD1 init prefix net
                    dD1.simrecs.Add((prefix, meo.n), nv)
                    //vprintln 3 (netToStr net + " generated simrec")
                    nv 
            bl
        | other -> (vprintln 1 ("gen_srec other ignored :" + xToStr other); valOf !dD1.gully_simrec)




(*
 * Return an lvalue for assignment or that can be deref'd as an rvalue and also the simrec.
 * And an option ...
 *)
let dioset1 (dD1:diosim_env_t) pmim (pos, machine:real_machine_t option) arg =
    if pos < 0L then (ref DX, None)
    else
        let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
        //let _ = vprintln 0 "dioset1 start"
        let resolve = function
            | X_bnet ff, _ ->
                let f2 = lookup_net2 ff.n
                //dev_println (sprintf " resolve " + netToStr (X_bnet ff))
                (Some ff, ff.n, ff.id, X_bnet ff, f2.length  = g_wondarray_marker_dims, f2)
            | (_, id)        -> muddy "(None, id, gec_X_net id) L726"
        let (d, nn, id, net, wondf, f2) = resolve arg
        //vprintln 0 "dioset1 mid"
        let pos' = try Some(down64 pos)
                   with _ -> None
        let iobase = g_iobase ""

        if not_nonep d && not_nonep pos' && f2.vtype = V_REGISTER && not_nonep machine // microcode machine - old - not in mainstream use.
                && pos >= 0L
             then ((valOf machine).regs.[valOf pos'], None)

        elif id = "hpr_mem" && not_nonep machine && pos' <> None then  // hardcoded memory - old - not in mainstream use.
                (
                vprintln 0 (i2s64 pos + " iospace write 1");
                if valOf pos' >= iobase && valOf pos' < iobase+400+6 
                        then (((valOf_or_fail "invalid i/o address" dD1.iospace.[valOf pos'-iobase]).current), None)
                        else ((valOf machine).mem.[valOf pos'], None)
                )
        else
            let sreco =
                if wondf then Some(gen_srec dD1 [] g_diosim_global_prefix net)             //  ("wondf wondarray autodefine " + netToStr net)
                else my_binder id
            match sreco with
                | None -> sf(sprintf "dioset1: lookup net for simulation failed. net=%s raw=%A key=%s" (netToStr net) (netToStr (fst arg)) (xkey (fst arg)))

                    // We need to autocreate wondarrays where used - e.g. for mutex bools in high-level simulation. KiwiC assumes there is one for each type in global existance, shared over all VMs.

                    //sf (sprintf "need to create %A" b)
                | Some simrec ->
                    match simrec.sdict with // sparse dictionary
                        | Some dict ->
                            let sro = Some simrec
                            let (found, ov) = dict.TryGetValue(pos)
                            //let _ = vprintln 0 (sprintf "dioset1 sparse dict_id=%s pos=%A existing=%A %s" id  pos found (if found then sprintf "ov=%A" ov else ""))
                            if found then (ov, sro)
                            else
                                let ans = ref (if !g_diosim_clear_arrays then DN 0L else DX)
                                let _ = dict.Add(pos, ans)
                                (ans, sro)
                        | None -> // else a proper array or scalar
                            match simrec.array with
                                | None -> // A scalar
                                    if pos=0L then
                                        //dev_println  (sprintf "resolve id to a scalar (not an array) id=%s  simrec=%s: " id (netToStr simrec.it))
                                        (simrec.current, Some simrec)
                                    else sf((netToStr(X_bnet(valOf d))) + ": dioset subscripted item is not an array")
                                | Some aa ->
                                     let s = asize (simrec.length)
                                     if pos < 0L || pos >= s || pos'=None 
                                     then (if s=0L then ()
                                           else vprintln 0 ("diosim: +++ Subscript " + i2s64 pos + " is out of range for " + id + " (0.." + i2s64(s-1L) + ")");
                                           (ref DX, None)
                                          )
                                     else
                                         let rr = (aa.[valOf pos'], Some simrec)
                                         //vprintln 5 ("arraysubs " + id +  " gave " + dnToStr !(fst rr))
                                         rr


let rez_update dD1 simreco (subs:int64) (loc:simval_t ref) v lane doner hexp =
    match simreco with
        | Some (simrec:simrec_t) ->
            let cll = simrec.cll
            let up = { simrec=simreco; cll=cll; subs=subs; loc=loc; update=v; mask= lane; done_already=doner; src_hexp=hexp; }
            up
        | None -> sf ("diosim: rez_update - simrec missing L910")
(*
 * Create a pending update to be made at the clock edge: (evaluate/commit) signal assignment semantics.
 * Remove any existing updates to this var in this lane, before adding new.
 *)



let setpending dD1 simreco (subs:int64) (loc:simval_t ref) v lane doner hexp =
    if nonep simreco then ()
    else
        let up = rez_update dD1 simreco (subs:int64) (loc:simval_t ref) v lane doner hexp
        //vprintln 0 "setpending start"
        let cll = up.cll
        let rec remove_any_already_there = function
            | [] -> []
            | h::tt ->
                if cll<>h.cll || subs<>h.subs then h :: remove_any_already_there tt
                elif lane= -1L || h.mask = lane then remove_any_already_there tt
                elif h.mask = -1L then sf "idiosim bitinsert merge needed"
                else h :: remove_any_already_there tt
        dD1.m_pendings := up :: (remove_any_already_there !dD1.m_pendings)
        //dev_println (sprintf "setpending done for " + (xToStr hexp))
        ()
        


let diosim_logicnum prec = function
    | []     -> DN 0L
    | [item] -> DN (System.Convert.ToInt64 (item:int32))
    | lst    ->
        let rec kk = function
            | []   -> 0I
            | (h:int)::t -> BigInteger(h) + 4096I * kk t
        gec_DL (prec, kk lst)
    

let logical_op_on_fp_get_bits prec = function
    | DF f         -> if prec.widtho = Some 64 then get_f64_bits f else get_f32_bits (float32 f)
    | DN p         -> BigInteger p
    | DL(_, bn, _) -> bn
    | _ -> sf ("logical_op_on_fp_get_bits other numeric form")


let x_calc_bitand prec (l,r) =
    let bnand a b =
        match prec.widtho with
            | None -> bnum_bitand_unspec_width a b
            | Some width -> bnum_bitand_with_width (prec.signed=Signed) width a b

    match (l,r) with
    | (DN 0L, _) 
    | (_, DN 0L) -> DN 0L

    | (DX, _)
    | (_, DX) -> DX

    | (DN a, DN b) ->
        //let _ = vprintln 0 ("using DN &&& DN clause")
        DN (a &&& b)
    
    | (DN a, DL(_, b, _))        -> gec_DL(prec, bnand (BigInteger a) b)
    | (DL(_, a, _), DN b)        -> gec_DL(prec, bnand a (BigInteger b))
    | (DL(_, a, _), DL(_, b, _)) -> gec_DL(prec, bnand a b)

    | (_, DF _)
    | (DF _, _) ->
        gec_DL(prec, bnand (logical_op_on_fp_get_bits prec l) (logical_op_on_fp_get_bits prec r))

    
    | (_, _) -> sf "x_calc_bitand other"

//let x_calc_bitand_ arg =
//    let c = x_calc_bitand_core arg
//    vprintln 0 (sprintf "xcalc_bitand %A gives %A" arg c)
//    c

(*
 *  Arithmetic compute.
 *)
let x_calc_onesc = function
    | DX           -> DX
    | DN n         -> DN(-1L - n)
    | DL(w, bn, _) -> gec_DL(w, -1I - bn) 
    | other        -> sf (sprintf "diosim: x_calc_onesc other form %A" other)


// Unary negate - this is different for floating and integer numbers of course.   
let x_calc_neg = function
    | DX           -> DX
    | DN n         -> DN(-n)
    | DL(w, bn, _) -> gec_DL(w, 0I - bn)
    | DF flt       -> DF (-flt)
    | other     -> sf (sprintf "diosim: x_calc_neg other form %A" other)



let x_calc_bitor prec (l, r) =
    match (l, r) with
    | (DX, _) -> DX
    | (_, DX) -> DX
    | (DN a, DN b) -> DN (a ||| b)
    
    | (DN a, DL(_, b, _))         -> gec_DL(prec, bnum_bitor (BigInteger a) b)
    | (DL(_, a, _), DN b)         -> gec_DL(prec, bnum_bitor a (BigInteger b))
    | (DL(_, a, _), DL(_, b, _))  -> gec_DL(prec, bnum_bitor a  b)

    | (_, DF _)
    | (DF _, _) ->
        gec_DL(prec, bnum_bitor (logical_op_on_fp_get_bits prec l) (logical_op_on_fp_get_bits prec r))

    | (_, _) -> sf "x_calc_bitor other"


let x_calc0 prec f g (l, r) = // generic X progpagation - eg. for xor.
    match (l, r) with 
    | (DX, _) -> DX
    | (_, DX) -> DX
    | (DN p, DN q)               -> DN(f p q)
    | (DL(_, p, _), DN    q)     -> gec_DL(prec, g p (BigInteger q))
    | (DN p, DL(_, q, _))        -> gec_DL(prec, g (BigInteger p) q)
    | (DL(_, p, _), DL(_, q, _)) -> gec_DL(prec, g p q)
    | (_, DF _)
    | (DF _, _) ->      gec_DL(prec, g (logical_op_on_fp_get_bits prec l) (logical_op_on_fp_get_bits prec r))

    | (_, _) -> sf "x_calc0 f other"


let diosim_to_double = function
    | DN v        -> (double v)
    | DL(_, v, _) -> (double v)
    | DF fv       -> fv
    | other -> sf (sprintf "diosim_to_double other %A" other)

//  Bitwise predic
let x_calcb prec f g d = function
    |(DX, _) -> DX
    |(_, DX) -> DX
    |(DN p, DN q)               -> diosim_logicnum prec (f p q)
    |(DL(_, p, _), DN q)        -> diosim_logicnum prec (g(p, BigInteger q))
    |(DN p, DL(_, q, _))        -> diosim_logicnum prec (g(BigInteger p, q))
    |(DL(_, p, _), DL(_, q, _)) -> diosim_logicnum prec (g(p, q))

    |(DF ll, rr) ->     DN(if d ll (diosim_to_double rr) then 1L else 0L)
    |(ll, DF rr) ->     DN(if d (diosim_to_double ll) rr then 1L else 0L)

    |(ll, rr) -> sf (sprintf "x_calcb f other %A oo %A" ll rr)


let x_calc_growing prec g = function
    |(DX, _) -> DX
    |(_, DX) -> DX
    |(DN p, DN q)               -> diosim_logicnum prec (g (BigInteger p) (BigInteger q))
    |(DL(_, p, _), DN q)        -> diosim_logicnum prec (g p (BigInteger q))
    |(DN p, DL(_, q, _))        -> diosim_logicnum prec (g (BigInteger p) q)
    |(DL(_, p, _), DL(_, q, _)) -> diosim_logicnum prec (g p q)
    |(ll, rr) -> sf (sprintf "x_calc_growing other %A oo %A" ll rr)



let rec x_calc_fp_arith g l r =
    if l=DX || r=DX then DX
    else
        DF(g (diosim_to_double l) (diosim_to_double r))


let x_calc_int_arith prec g (l, r)  =
    match (l, r) with
    | (DX, _) -> DX
    | (_, DX) -> DX

    | (DN p, DN q)               -> gec_DL(prec, g (BigInteger p) (BigInteger q))
    | (DL(_, p, _), DN q)        -> gec_DL(prec, g p (BigInteger q))
    | (DN p, DL(_, q, _))        -> gec_DL(prec, g (BigInteger p) q)
    | (DL(_, p, _), DL(_, q, _)) -> gec_DL(prec, g p q)

    | (_, DF _)
    | (DF _, _) ->
        gec_DL(prec, g (logical_op_on_fp_get_bits prec l) (logical_op_on_fp_get_bits prec r))

        
    | (_, _) -> sf "x_calc_int_arith f other"


let ds_dx = function
    | DX -> true
    | _ -> false

let ds_bi = function
    | DN n -> BigInteger n
    | DL(w, bn, _) -> bn
    | other -> sf (sprintf "Apply ds_bi to %s" (dnToStr other))

    
let ds_monkey arg = // Convert to bool for IF guards and so on.
    let r =
        match arg with
            | DX           -> false
            | DN n         -> n <> 0L
            | DL(w, bn, _) -> not(bn.IsZero)
            | _ -> sf "ds_monkey other - float ?  "
    //let _ = vprintln 0 ("Monkey " + dnToStr arg + " gives " + boolToStr r)
    r

let x_monkey arg =
    let r =
        match arg with
            | DX           -> false
            | DN n         -> n <> 0L
            | DL(w, bn, _) -> not(bn.IsZero)
            | _ -> sf "x_monkey other - float ?  "
    //let _ = vprintln 0 ("Monkey " + dnToStr arg + " gives " + boolToStr r)
    r


//
//
//
let perform_bitinsert = function
    | (DN ov, bit, DN nv) when nv = 0L  ->  DN(ov &&& (dn_inv_masks.[int32 bit]))
    | (DN ov, bit, DN nv) when nv <> 0L ->  DN(ov ||| (dn_masks.[int32 bit]))    

    | (_, bit, DN nv) -> 
        if nv=0L then DN 0L
        else
            let m = dn_masks.[int32 bit]
            DN (nv*m)

    | (_, bit, DL _) -> sf "dl bitinsert"
    | (ov, bit, DX) -> ov


    | (ov, bit, _) -> sf "perform bitinsert other"





let troc_collate (l:troc_t list) = // Collate tracing events by cll - is this a NOP ?
    let colf v =
        match v.tsrc with
            | simrec -> simrec.cll

    let col cc (v:troc_t) = singly_add (colf v) cc
    let ids = List.fold col [] l 
        (*      let _ = app (fun x -> vprintln 0 (x + " troc collate")) ids *)
    let rec gg k = function
            | [] -> sf "troc collate"
            | (h:troc_t)::tt -> if colf h = k then h else gg k tt
    map (fun x ->(gg x l)) ids

  
//
// This implements casts to different precisions, including to and from floating point (f/p).
// The f/p casts potentially have two forms, one that preserves bit patterns and one that preserves represented number (when possible).
// The semantics in C is that values are preserved. In Verilog, everything outside the PLI is in bits.
let rec diosim_appmask iprec caster cvtf nv =
    let ans = diosim_appmask_ntr iprec caster cvtf nv
    //if vd>=4 then vprintln vd (sprintf "diosim appmask %s to %s gives %s" (prec2str caster) (dnToStrx nv) (dnToStrx ans))
    ans
    
and diosim_appmask_ntr iprec caster cvtf nv =
    let tailer_int bn =
        match bn_masker iprec caster bn with
            | X_bnum(pp, bn, _) -> gec_DL(pp, bn)
            | X_bnet ff when not_nullp ff.constval ->
                //dev_println (sprintf "tailer_int returning " + xToStr (X_bnet ff))
                match ff.constval with
                    | XC_double dd ::_   -> DF dd // Need to return int here! TODO
                    | XC_float32  ss ::_ -> DF (double ss)
                    | other -> sf (sprintf "diosim appmask const tailer cast=%A other=%A" caster other)
            | other -> sf (sprintf "diosim appmask tailer  L956: cast=%A key=%s other=%A" caster (xkey other) other)
            //if vd>=4 then vprintln vd (sprintf "diosim_appmask bnum tailer in_sign=%i out_sign=%i width=%A on %A giving %A" bn.Sign ans.Sign caster (bix bn) (bix ans))

    let fp_cvt_not_cast_mode = cvtf=CS_preserve_represented_value // We used to always cvt but struct unpack etc needs pure cast.

    let tailer_fp (bn:BigInteger) =  
        vprintln 3 (sprintf "fp convert fp_cvt_not_cast_mode=%A" fp_cvt_not_cast_mode)
        if fp_cvt_not_cast_mode then              // This variant changes the bits (fp_cvt_not_cast_mode holds) yet which maintains the represented value.
            if caster.widtho = Some 32 then
                DF (double bn) // TODO copy in code that checks 9223372036854775807.0 and 3.4028 etc.. since otherwise the value cannot be represented in the new form afterall.
            else DF (double bn)
        else
            // This variant is a cast that leaves the bit pattern logically unchanged and hence the value represented changes.
            if caster.widtho = Some 64 then
                //let bytes = System.BitConverter.GetBytes(int64 bn) <- this can raise overflow 
                let bytes = bn.ToByteArray()
                let bytes =
                    if bytes.Length < 8 then Array.append [| 0uy; 0uy; 0uy; 0uy; 0uy; 0uy; 0uy |] bytes
                    else bytes
                let f64 = System.BitConverter.ToDouble(bytes, 0)
                DF f64
            else
                let bytes = bn.ToByteArray()
                let bytes =
                    if bytes.Length < 4 then Array.append [| 0uy; 0uy; 0uy; 0uy |] bytes
                    else bytes
                let f32 = System.BitConverter.ToSingle(bytes, 0)
                DF (double f32)

    let tailer = if caster.signed=FloatingPoint then tailer_fp else tailer_int

    match nv with
    | nv when nonep caster.widtho ->
        vprintln 3 (sprintf "diosim_appmask - no width - leaving %A alone" nv)
        nv

    | DX -> DX
    
    | DF v ->
        if caster.signed=FloatingPoint then
            if caster.widtho = Some 32 then
                if v >= 3.402823e38 then DF System.Double.PositiveInfinity
                elif v <= -3.402823e38 then DF -System.Double.PositiveInfinity
                else DF v
            else DF v
        else
            if v > 9223372036854775807.0 || v < -9223372036854775808.0 then
                    cleanexit (sprintf "diosim : out of range double to int (%A) convert arg=%A" caster v)      // Mono does not seem to have BigInteger(double) in its library although msdn documents it.
            else DN(int64 v)  // You might think this changes the bits to preserve the value, but diosim is essentially an RTL simulator and the roles of DN are for presentation and for invoking the correct arithmetic overload of the standard for operators.  So is this correct?

    | DS ss ->
        let _ = if caster <> g_string_prec then vprintln 3 (sprintf "diosim: cannot apply mask %s to string %A - expect StringHandle" (prec2str caster) ss)
        DS ss

    | DN v ->
        let w = valOf caster.widtho
        let ans = 
            if w < 12 then DN(int64(lowmask w) &&& v) // wierd - pre native BigInteger legacy policy -  please clarify/refine TODO
                      else tailer(BigInteger v)
        //let _ = vprintln 0 (sprintf "diosim_appmask apply narrow mask caster=%s on %A giving %s" (prec2str caster) v (dnToStr ans))
        ans

    | DL(w, bn, _) ->
        let ans = tailer bn
        //let _ = vprintln 0 (sprintf "diosim_appmask bnum mask/caster=%s on signed=%i %A giving finally %A" (prec2str caster) bn.Sign (bix bn) (dnToStr ans))
        ans

let tsToStr__ = function
    | (sr:simrec_t) ->  hptos sr.prefix + ":" + xToStr sr.it

(*
 * Write pending assignments to slaves.
 *)
let committer dD1 ef time (update:update_t) = 
    let nv = update.update
    let (name, scalarf)  =
        match update.simrec with
            | Some simrec ->
                match simrec.it with
                    | X_bnet ff     -> (ff.id, not ff.is_array)
                    | X_net(id, _)  -> (id, true)
                    | _             -> ("-voider-", true)
            | _ -> ("-missing-", true)

    let simval = update.loc
    let nv1 = if update.mask < 0L then nv
              else perform_bitinsert(!simval, update.mask, nv)
    if !dD1.mvd > 6 then vprintln 6 (i2s time  + (if update.done_already then sprintf ": Log change to %s" name else sprintf ": Commit update to %s" name) + (if scalarf then "" else sprintf " storesubs=[%i]" update.subs) + (if update.mask<0L then "" else "{b=" + i2s64 update.mask + "}") + " to value " + dnToStr nv1  + " (" + (dnToStrx nv1) + ")")
    
    if update.done_already then update_trocgen dD1 ef time update (* One troc per bit-lane changed *)
    else 
        let d = !simval <> nv1
        //vprintln 0 (sprintf "diosim: committer hexp=%s nv=%s" (xToStr update.src_hexp) (dnToStr nv1))
        let nv2 =
            if nonep update.simrec then nv1 // A simrec needs be stored for the commit to be clip masked.
            else
                //vprintln 0 (sprintf "diosim committer: Unmasked new value for %s is %s mask=%A" update.id (dnToStr nv1) (valOf update.simrec).encoding_cvt)
                // We need here both the src and dest precisions.
                diosim_appmask (mine_prec g_bounda update.src_hexp).widtho (valOf update.simrec).encoding_cvt CS_maskcast nv1
        if d then simval := nv2
        update_trocgen dD1 ef time update


let tnowfn x = (i2s x) + ":"


let chr c = System.Convert.ToChar(c:int)

let printf_numeric_arg field_width modechar arg = // somewhat crude
    let ans = 
      match modechar with
        | 'h' // Verilog uses h for hex print.
        | 'x' ->
            //let _ = vprintln 0 (sprintf "diosim hex print %A" arg)
            match arg with
                | DN n  -> sprintf "%*x" field_width n
                | DL(w, bn, _) -> tnumToX_lc true (gec_X_bnum(w, bn))
                | other -> dnToStr arg 
        | 'H'
        | 'X' ->
            match arg with
                | DN n         -> sprintf "%*X" field_width n
                | DL(w, bn, _) -> tnumToX_lc false (gec_X_bnum(w, bn))
                | other -> dnToStr arg 
        | 'd' | 'u' -> // We ignore which ... hmm 
            dnToStr arg
        | 'f' ->
            dnToStr arg
        | other -> // decimal 'D' or 'U' 
            let _ = dev_println (sprintf "Ignore other modechar '%c' for %s" other (dnToStr arg))
            dnToStr arg
    //let _ = vprintln 0 (sprintf "diosim print field=%i,  modechar=%c, ans=%A" field_width modechar arg)
    ans


(*

let wide_sign_extend_to (new_bit_len:int) arg =  // This assumes arg is negative and needs extension.
    let (_, old_msb) = wide_find_msb arg
    let rec extend p = function
        | h::tt when p+12 < old_msb -> h :: extend (p+12) tt
        | h::tt ->
            let trim12 x = if x > 12 then 12 else x
            let q1 = (1<<<(old_msb - p + 1)) - 1
            let q2 = (1<<<(trim12(new_bit_len - p))) - 1
            let _ = vprintln 0 (sprintf "h=%i q1=%i q2=%i" h q1 q2)
            (h + q2 - q1) :: extend (p+12) [] // old tt is zeros and does not need to be passed
        | [] when p+12 < new_bit_len -> 4095 :: extend (p+12) []
        | [] -> if new_bit_len <= p then [] else [(1<<<(new_bit_len - p))-1]
    if old_msb+1 >= new_bit_len then arg else extend 0 arg



let wide_sign_extend_to_unit_tests () =
    let utest n =
        let tdata = [ 15; 16; 17 ]
        let d = n*5 + 20
        let ans = wide_sign_extend_to d tdata
        let _ = vprintln 0 (sprintf "wide sign extend unit test %A to %i is %A" (wideToX tdata) d (wideToX ans))
// wide sign extend unit test "[00f, 010, 011]" to 20 is "[00f, 010, 011]"
// wide sign extend unit test "[00f, 010, 011]" to 25 is "[00f, 010, 011]"
// wide sign extend unit test "[00f, 010, 011]" to 30 is "[00f, 010, 031]"
// wide sign extend unit test "[00f, 010, 011]" to 35 is "[00f, 010, 7f1]"
// wide sign extend unit test "[00f, 010, 011]" to 40 is "[00f, 010, ff1, 00f]"
        ()
    app utest [0..4]

*)

let g_ut_done = ref false

let bnum_logic_unit_tests () =
    let p = bix
    let rec utest = function
        | a::b::tt ->
            let _ = utest (b::tt)
            let _ = vprintln 0 (sprintf "\n\nutest %A with %A" a b)
            let xx = bnum_bitxor a b
            let aa = bnum_bitand_unspec_width a b
            let oo = bnum_bitor a b                        
            let _ = vprintln 0 (sprintf "   utest wide xor %s with %s gives %s     (%s)" (p a) (p b) (p xx) (p -xx))
            let _ = vprintln 0 (sprintf "   utest wide and %s with %s gives %s     (%s)" (p a) (p b) (p aa) (p -aa))
            let _ = vprintln 0 (sprintf "   utest wide  or %s with %s gives %s     (%s)" (p a) (p b) (p oo) (p -oo))
            ()
        | _ -> ()

    utest [ -1I; 1221123I; 1045I; 5555I; -11232I; -1I; 1231321231I; 0I; -123I ]

let yy(x) = if x then [1] else [0] // Positive demonkey
let yn(x) = if x then [0] else [1] // Negated variant.

    
// Big number less than compare - demonkeyed.
let bn_dltd prec signed (a, b) =
    // Use width from prec field, but ignore prec.signed.
    yy(const_bn_dltd prec.widtho signed a b)

// Big number less than or equal compare - made by negating and arg swap on dltd.
let bn_dled prec signed (a, b) =
    // Use width from prec field, but ignore prec.signed.
    yn(const_bn_dltd prec.widtho signed b a)

// TODO: We get a diosim/Verilog mismatch when 2'sd3 is compared with a two-bit signed register with 3 stored in it.  It would be better for that register to have been unsigned infact, since it held an enum (aka strobe group) generated by repack.fs (and that is now changed), but we do want to avoid diosim mismatches in the future.  Diosim represents the stored 2 and 3 as -1 and -2 and fails to match when compared with 2 and 3.  Whereas Verilog matches ok when the two-bit signed register is read back and compared with 2'sd2 or 2'sd3.  Can we make diosim compare look at width on equality?

let x_bdiocalc prec oo d e =
    //dev_println (sprintf "x_bdiocalc prec oo=%A d=%A e=%A" oo d e)
    match oo with 
    | V_dltd signed -> x_calcb prec (fun a b->yy(const_int64_dltd prec.widtho signed a b)) (bn_dltd prec signed) (fun (p:double) q -> p<q) (d, e)
    | V_dled signed -> x_calcb prec (fun a b->yn(const_int64_dltd prec.widtho signed b a)) (bn_dled prec signed) (fun (p:double) q -> p<=q) (d, e)
    | V_deqd        -> x_calcb prec (fun a b->yy(a=b))                                     (fun (a: BigInteger, b) -> yy(a=b)) (fun (p:double) q -> p=q)(d, e)
    | V_dned        -> x_calcb prec (fun a b->yn(a=b))                                     (fun (a: BigInteger, b) -> yn(a=b)) (fun (p:double) q -> p<>q) (d, e)
    | oo -> sf(f3o4(xbToStr_dop oo) + ": x_bdiocalc")

//
// x_diocalc is diosim's main ALU implementation.
//
let rec x_diocalc dD1 prec oo d e =
    let ans = x_diocalc_ntr dD1 prec oo d e
    //if !dD1.mvd >= 10 then vprintln 10 (sprintf "    x_diocalc  %s %A %s --> %s" (dnToStr d) oo (dnToStrx e) (dnToStrx ans))
    ans
    
and x_diocalc_ntr dD1 prec oo d e =
    let isf = function
        | DF _ -> true
        | _ -> false

    let is_logical = function // This function is repeated elsewhere - please use library copy
        | V_bitor | V_bitand | V_lshift _ | V_rshift _ | V_xor -> true
        | _ -> false

        
    if (isf d || isf e) && not (is_logical oo) then
        match oo with
    
        | V_minus  -> 
            let ans = x_calc_fp_arith (fun (l:double) r-> l-r) d e
            (* let _ = vprint(3, "subtract " + dnToStr d + " and " + dnToStr e + " gave " + dnToStr ans + "\n")
            *)
            ans
        | V_plus   ->
            let ans = x_calc_fp_arith (fun (l:double) r->l+r) d e
            // (*      let _ = lprint (!old_sim_vd) (fun()->"add " + dnToStr d + " and " + dnToStr e + " gave " + dnToStr ans + "\n")  *)
            ans

        | V_divide  -> x_calc_fp_arith (fun (l:double) r->l/r) d e
        | V_mod     -> x_calc_fp_arith (fun (l:double) r->l%r) d e

        | V_times -> 
            let ans = x_calc_fp_arith (fun (l:double) r->l*r) d e
            ans 

        // Logical operators on floating point numbers treat them as bit fields in other branch of this code now (see is_logical).
        //| V_lshift -> x_calc_fp_arith (fun (l:double) r->BigInteger.Pow(2I, int32(*System.Convert.ToInt32*) r) * l) d e
        //| V_rshift  -> x_calc_fp_arith (fun (l:double) r->l / BigInteger.Pow(2I, int32(*System.Convert.ToInt32*)r)) d e

        | oo -> sf(sprintf ": x_diocalc other f/p operator %A" oo)


    else
        match oo with
        | V_bitor   -> x_calc_bitor prec (d, e)  (* Preserve this for identity propagation of dontcare DX *)
        | V_bitand  ->
            let vperform_bitand a b =
                //let xp = printf_arg 8 'x' 
                //let _ = vprintln 0 (sprintf "perform bit and %s and %s" (xp l) (xp r))
                let ans = x_calc_bitand prec (a, b)
                ans
            if (false) then vperform_bitand d e else x_calc_bitand prec (d, e) // Preserve this for identity propagation of dontcare DX 
        | V_xor     ->
            let nar_xor a b =
                let ans = a ^^^ b
                //let _ = vprintln 0 (sprintf "kat narrow xor %x %x -> %x" a b ans)
                ans
            let bnum_bitxor_verbose a b =
                let ans = bnum_bitxor a b
                //let _ = vprintln 0 (sprintf "temp debug: bnum bitxor prec=%A 0x%s ^ 0x%s ----> 0x%s" prec (bix a) (bix b) (bix ans))
                ans
            x_calc0 prec nar_xor bnum_bitxor_verbose (d, e)

        | V_minus  -> 
            //vprintln (!old_sim_vd) "sub in"
            let ans = x_calc_int_arith prec (fun (l:BigInteger) r-> l-r) (d, e)
            (* let _ = vprint(3, "subtract " + dnToStr d + " and " + dnToStr e + " gave " + dnToStr ans + "\n")
            *)
            ans
        | V_plus   ->
            let ans = x_calc_int_arith prec (fun (l:BigInteger) r->l+r) (d, e)
            //vprintln 5 ("add " + dnToStr d + " and " + dnToStr e + " gave " + dnToStr ans)
            ans

        | V_divide ->
            if ds_dx d || ds_dx e then DX
            elif ds_bi e = 0I then
                vprintln 2 (sprintf "divide %s by zero" (dnToStr d))
                DX
            else
                let divider (l:BigInteger) r = l/r
                x_calc_int_arith prec divider (d, e)

        | V_mod      ->
            if ds_dx d || ds_dx e then DX
            elif ds_bi e = 0I then
                let _ = vprintln 2 (sprintf "mod %s by zero" (dnToStr d))
                DX
            else
            x_calc_int_arith prec (fun (l:BigInteger) r->l%r) (d, e)

        | V_lshift ->
            let wide64_lshift num amount = 
                let ans = BigInteger.Pow(2I, int32 amount) * num
                //let _ = vprintln 0 (sprintf "temp debug: wide lshift prec=%A 0x%s <<  %A ----> 0x%s" prec (bix num) amount (bix ans))
                ans
            x_calc_int_arith prec wide64_lshift (d, e)

        | V_times -> 
            let ans = x_calc_int_arith prec (fun (l:BigInteger) r->l*r) (d, e)
            //vprintln 5 ("diosim mult " + dnToStr d + " and " + dnToStr e + " gave " + dnToStr ans)
            //vprintln 5 (sprintf "diosim: prec=%A mult 0x%s * 0x%s --> 0x%s" prec (dnToStrx d) (dnToStrx e) (dnToStrx ans))
            ans 

        | V_rshift ss ->
            x_calc_int_arith prec (bnum_rshift ss prec (* { prec with signed=ss; }*)) (d, e)
        | (oo) -> sf(f3o3(xToStr_dop oo) + ": x_diocalc int")

//
//   Leaf console/printing output routine.
let xsim_print (dD1:diosim_env_t) (x:string) = 
   let col_on = ansi_techno 1
   let col_off = ansi_techno -1
   let x' = if dD1.techno then col_on + x + col_off else x
   in
   (
    vprint 0 x'
    if (dD1.outcos <> None) then yout (valOf dD1.outcos) x;
    //if !sim_tl < 6 then vprint(0, x) else ();
    // Trap some magic strings that turn on and off detailed logging.
    let sim_vd = dD1.mvd
    if x = "diosim:traceon" then sim_vd := 10
    elif x = "diosim:traceoff" then sim_vd := 3
    elif x = "diosim:exit" then (vprint 0 "Exit magic\n"; m_exitflag := true)
    else ()
   )



// diosim implementation of printf
let rec builtin_printf = function
    | ([], _) -> ""
    | (h::t, argfun) -> 
        let rec char_fun = function
            | DN n  when n > -128L && n < 256L -> implode [chr (int n)]
            | DN n  -> sprintf "*inv_char*^%i_" n
            | DL(_, bn, _) when bn > -128I && bn < 256I -> char_fun(DN (int64 bn))
            | other -> sprintf "*inv_char*%A" other
        let printf_str_fun = function
            | DS ss -> ss
            | other -> sprintf "*L1295-inv_string*" + dnToStr other
        let (n, t') =
            let rec readfw sofar = function
                | fc::tt when System.Char.IsDigit fc -> readfw (sofar*10 + int fc - int '0') tt
                | other -> (sofar, other)
            if h = '%' && not_nullp t then
                let (field_width, t) = readfw 0 t 
                match hd t with
                    | 'c' -> (char_fun(argfun()), tl t) //
                    | '%' -> ("%", tl t)
                    | 's' -> (printf_str_fun(argfun()), tl t)
                    | _   -> (printf_numeric_arg field_width (hd t) (argfun()), tl t)
            elif h = '\\' && not_nullp t && hd t = 'n' then ("\n", tl t)
            else (implode[h], t)
        n + (builtin_printf(t', argfun))


let dio_print (dD1:diosim_env_t) l newlinef = 
    if !dD1.mvd >= 8 then vprintln 8 ("Starting dio_print")
    let autoformat = not (nullp l) && hd l = DS s_autoformat
    let from_ds = function
        | DX        -> X_undef
        | DL(w, bn, m) -> X_bnum(w, bn, m)
        | DN n  -> xi_int64 n
        | other -> xi_string(dnToStr other)

    let to_ds = function
        | X_bnum(w, bn, m) -> DL(w, bn, m)
        | X_num n   -> DN (int64 n)
        | X_undef   -> DX
        | W_string(s, _, _) -> DS s
        | other ->
            let dsr = xToStr other
            vprintln 3 (sprintf "to_ds other form %s" dsr)
            DS dsr
    let l' = if autoformat then map to_ds (replace_autoformat_string true (remove_concats (map from_ds l))) else l
    //  ("In dio_print af=" + boolToStr autoformat)
    // let ptr = new string_burner(if nullp l' then [] else tl l', 0)
    let ptr = ref(if l'= [] then [] else tl l')
    let argfun() = if nullp !ptr then DX else let r = hd(!ptr) in (ptr := tl(!ptr); r) // TODO use string_burner
    let kpp = function
        | [] -> ()        
        | DS(s)::ttt -> xsim_print dD1 (builtin_printf(explode s, argfun))
        | v -> app (fun x->xsim_print dD1 (dnToStr x)) l'
    let _ = kpp l'
    if newlinef then xsim_print dD1 ("\n")
    ()

// Commit all changes to event list and find if any changed.
let is_triggered sensitivity_lst =
    let ans = ref false
    let trig_scan = function
        | (De_any, (simrec:simrec_t), oldval) ->
            let _ = if !simrec.current <> !oldval then (ans := true; oldval := !simrec.current)
            ()
    let _ = map trig_scan sensitivity_lst
    !ans

// We want comb changes to be rendered with some logic delay and to appear earlier than when it is computed on demand at next clock edge.
let comb_delay_plot_helper dD1 gen  =
    if true then (gen-1)*dD1.timescale +  17 // Experimental comb delay plot helper - can produce out-of-order VCD - please investigate.

    else gen*dD1.timescale


let rec diosim_interlocked_get_lval gG pmim debug_pc msg location =     // TODO need to generate a TROC on updates to this l-val.
    let (minfo, stack, HC, VM, gen, dD1) = gG
    let net =
        match location with
            | X_bnet net ->
                let (net, simrec_) = dioset1 dD1 pmim (0L, VM) (location, "interlocked-location")
                net
            | W_asubsc(lhs, subs, _) ->
                let ival64 = function // 
                    | DN subs        -> subs
                    | DL(w, bn, _)   -> int64 bn
                    | DX -> sf("diosim: interlocked-location-array: uncertain subscript arg to testandset in " + xToStr location)
                let pos = ival64 (xsim gG (pmim) debug_pc subs)
                let (net, simrec_) = dioset1 dD1 pmim (pos, VM) (lhs, "interlocked-location-array")
                net

            | X_net(sri_key, _) ->
                let simrec = gen_srec dD1 [] ["SRI_BOAT"] location
                simrec.current

            | other -> sf (sprintf "diosim: %s primitive requires l-value operand, not " msg + xToStr other)
    net

// Atomic test and set implementation.
and implement_testandset gG (pmim) debug_pc (mutex, new_vale) = 
    let (minfo, stack, HC, VM, gen, dD1) = gG
    let vd = !dD1.mvd
    let net = diosim_interlocked_get_lval gG pmim  debug_pc "hpr_test_and_set" mutex 
    let ov = !net
    // if !loc = nv then false else (setpending(simrec, id, Some d, subs, loc, nv, -1L, false); loc := nv; true)
    net := new_vale
    let ans = if not(ds_monkey new_vale) then new_vale else ov // Strange semantic on clearing: we return the ov only when setting really matters which is what we return
    if vd>=4 then vprintln 4 (xToStr mutex + ": testandset: new=" + dnToStr new_vale + ", result=" + dnToStr ans)
    ans


and read_register dD1 gG (pmim) debug_pc id nn nameN =
    let vd = !dD1.mvd
    let (minfo, stack, HC, PM:real_machine_t option, gen, dD1) = gG
    let (prefix, minfo, iinfo, my_binder, mach_id) = pmim

    //if vd>=7 then vrintln 7 ("read_register " + id)
    if id=s_autoformat then DS id
    elif id = g_tnow_string then DN (up64 gen)
    elif id = "hpr_sp" then DN(up64(!((valOf PM).sp))) (* Need to watch out for RTL with these special net names - they should be a little more distinctive perhaps or disabled by default ! *)
    elif id = "hpr_pc" then DN(up64(!((valOf PM).pc)))
    elif id = "hpr_mem" then sf("mem here")
    else
        let k = if nonep PM then None else op_assoc id ((valOf PM).symbols)
        if k <> None then
            if vd>=5 then vprintln 5 ("Was in symbols " + id)
            DN(valOf k)
        elif symbolic_check (explode id) then
            if vd>=5 then vprintln 5 ("Was symbolic " + id)
            DN(atoi64 id)
        else
            match my_binder id with
                | None ->  sf(sprintf "Cannot read_register: no simrec. id=%s net=%s" id (netToStr nameN))
                | Some simrec->
                    match !simrec.comb with
                        | None ->
                            if vd>=5 then vprintln 5 (sprintf "read_register: no comb: id=%s  srec_id=%s  current=%s" id (xToStr simrec.it) (dnToStr !simrec.current))
                            !simrec.current
                        | Some (pmim', sensitivity_lst, hexpp) ->
                            if !simrec.onstack then
                                vprintln 3 ("Combinational loop in " + id)
                                //let _ = simrec.generation := gen
                                !simrec.current
                            elif is_triggered sensitivity_lst then
                                simrec.onstack := true
                                // Lazy evaluation of combinational support:
                                if vd>=5 then vprintln 5 ("Needing comb value of " + id)
                                let nv = xsim gG pmim' debug_pc hexpp
                                simrec.current := nv
                                simrec.onstack := false                          
                                //let _ = simrec.generation := gen
                                // >H
                                let up = rez_update dD1 (Some simrec) (*was None*) (* simrec.flags *) -1L simrec.current nv -1L true hexpp
                                let _  = committer dD1 true (comb_delay_plot_helper dD1 gen) up
                                if vd>=5 then vprintln 5 ("Comb next value fetched for " + id + " is " + dnToStr nv)
                                nv
                             else !simrec.current

//
// Leaf routine for net updating.
//   
and xsim_assign10 gG (pmim) debug_pc hexpp aa = 
    let (minfo, stack, HC, PM, gen, dD1) = gG
    let _ =
        if not (!g_ut_done) then
            //let _ = bnum_logic_unit_tests ()
            g_ut_done := true


    match aa with
    | (n, X_bnet ff, mf, subs, bit, nv) -> 
        let it = X_bnet ff
        let (net, simrec) = dioset1 dD1 pmim ((if subs<0L then 0L else subs), PM) (it, "")
        if !dD1.mvd >=4 then vprintln 4 (((tnowfn gen) + (if n=1 then "Seq assign " else "Comb assign") + sprintf " prefix=%s next state for %s is %s cll=%A (xsim_assign10)" (hptos(f1o5 pmim)) ff.id (dnToStr nv) (if nonep simrec then "" else (valOf simrec).cll)))
        // Combinational assigns are actually just ignored at this point. How odd?  We want a troc from them, but why are they not updated right now?  Can cause a problem?
        if n=1 then setpending dD1 simrec  subs net nv bit false hexpp
        // In other words, nv is dropped when n <> 1.
        if !dD1.mvd>=4 then vprintln 4 ((tnowfn gen) + mf() + " of " + ff.id + " is (readback - commit still pending if seq) " + dnToStr !net)
        true


//
// Make a net assignment.
// The returned value holds if anything changed.
//
and xsim_assign gG (pmim) debug_pc n0 lhs0 erhs = 
    let (minfo, stack, HC, VM, gen, dD1) = gG
    let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
    //vprintln 0 (sprintf "xsim_assign: lhs=%s erhs=%s" (xToStr lhs0) (xToStr erhs))
    let nv = //evaluate rhs
        match lhs0 with
            | X_net(id,_) -> (xsim_ss gG (pmim) debug_pc erhs) // Special eval for string expressions.
            | _ ->        (xsim gG (pmim) debug_pc erhs)
            //with sfault(x) -> sf(id + " eval fault rethrow:" + x)

   // if tl >= 3 then vprintln 3 (sprintf "xsim_assign: lhs=%s rhs=%s" (xToStr lhs0) (dnToStr nv))

     //xsim_assign sequential code: the returned value holds if anything changed.
     // nx better be 1

    let xsim_assign_seq nx it =
        //if vd >= 4 then vprintln 4 (sprintf "xsim_assign_seq to %s" (netToStr it))
        match it with
        | X_bnet ff -> 
            let mf = (fun()-> ff.id)
            xsim_assign10 gG (pmim) debug_pc erhs (nx, it, mf, -1L, -1L, nv) 

        // SEQ: constant bitinsert
        | X_blift(W_bitsel(X_bnet f, bit, false, _)) -> 
            let mf = (fun()->"bit " + (i2s bit))
            xsim_assign10 gG (pmim) debug_pc erhs (nx, X_bnet f, mf, -1L, up64 bit, nv) 
            
        // SEQ: 2d : array/bitinsert 
        | X_blift(W_bsubsc(W_asubsc(X_bnet f, subs, _), bit, i, _)) ->
            let it = X_bnet f
            let _ = cassert(not i, "lhs negated bitinsert")
            let D m = function
                | DN v         -> Some v
                | DL(w, bn, _) -> Some(int64 bn)
                | (_) -> (vprintln 0 ((tnowfn gen) + m + " for write to " + f.id); None) 
            let b' = D "bit subs undertain" (xsim gG (pmim) debug_pc bit)
            let subs' = D "array subs uncertain" (xsim gG (pmim) debug_pc subs)
            if b'<> None && subs' <> None then
               let mf = fun()->"array bit " + i2s64(valOf b') + " location " + i2s64(valOf subs')
               xsim_assign10 gG (pmim) debug_pc erhs (nx, it, mf, valOf subs', valOf b', nv)
            else false


        //  SEQ: 1d : array OR bitinsert 
        | W_asubsc(X_bnet ff, subs, _) -> 
            let it = X_bnet ff 
            let fD m = function
                | DN v         -> v
                | DL(w, bn, _) -> int64 bn // TODO handle out of range
                                            
                | (_) -> (vprintln 1 ((tnowfn gen) + m + " uncertain for write to " + ff.id); -1L) 
            let subs' = xsim gG (pmim) debug_pc subs
            let bit = if ff.is_array then -1L else fD "bit subs undertain" subs'
            let subscript = if ff.is_array then fD "array subs uncertain" subs' else -1L
            let mf =
                if ff.is_array then fun()->"array location set " + i2s64 subscript
                else fun()->"Seq next state for bit " + i2s64 bit
            xsim_assign10 gG (pmim) debug_pc erhs (nx, it, mf, subscript, bit, nv)

        // Seq degenerate net
        | X_net(id,_) ->
            let (loc, simrec) = dioset1 dD1 pmim (0L, VM) (X_undef, id)
            muddy ("seq degenerate net assign - not expected to be used.  id=" + id)

        | X_undef -> false

        | d -> sf (xkey d + ": xsim assign_seq lhs other:" + xToStr d)


    let xsim_assign_comb nx it =
        //let _ = vprintln 0 (sprintf "xsim_assign_comb to %s" (netToStr it))
        match it with
        // COMB subsc of X_net: repeated ? NEED TO MAKE EIHER A OR B.
        | X_subsc(X_net(nets, m), subs) -> 
               let left = X_net(nets, m)
               let n' = (xsim gG (pmim) debug_pc subs) // with sfault(x) -> sf(id + " eval fault rethrow:" + x)
               if !dD1.mvd >=4 then vprintln 4 (tnowfn gen + "Comb next state for array " + nets + " subs=" + dnToStr n' + " is " + dnToStr(nv))
               let iv1 subs = 
                   let (loc, simreco) = dioset1 dD1 pmim (subs, VM) (X_undef, nets)
                   if !loc = nv then false
                   else (setpending dD1 simreco subs loc nv -1L false erhs; (* We both set straightaway and call setpending: to get a troc.*) loc := nv; true)

               let rec subsc_ival = function
                   | DL(w, bn, _)-> iv1(int64 bn)
                   | DN subs  -> iv1 subs
                   | DX -> false
               let ans =
                   try subsc_ival n'
                   with _ ->
                       let debug_pc = mach_id
                       vprintln 3 (debug_pc + sprintf ": subsc_ival - subscript to array %s out of range %A in %s" nets n' (xToStr it))
                       false

               ans

        | W_asubsc(X_bnet d, n, _) -> 
            let left = X_bnet d
            let id = d.id
            let n' = (xsim gG (pmim) debug_pc n) //with sfault(x) -> sf(id + " eval fault rethrow:" + x)
            if !dD1.mvd>=4 then vprintln 4 ("Comb next state for array " + id + " is " + dnToStr nv)
            if !dD1.mvd>=4 then vprintln 4 ((tnowfn gen) + "Assign (array) " + dnToStr nv + " to " + id + " subs=" + dnToStr n')
            let iv1 subs =
                let (loc, simrec) = dioset1 dD1 pmim (subs, VM) (left,"")
                //vprintln 0 "dioset done"
                if !loc = nv then false else (setpending dD1 simrec subs loc nv -1L false erhs; loc := nv; true)

            let rec bool_ival = function
                | DN subs   -> iv1 subs
                | DL(_, bn, _) -> iv1(int64 bn)
                | DX -> false
            let ans = try bool_ival n'
                      with _ ->
                        vprintln 3 (debug_pc + sprintf ": bool_ival - subscript to array %s out of range %A in %s" d.id n' (xToStr it))
                        false
            //let _ = vprintln 0 ( "j=" + boolToStr j)
            ans

        // COMB: subsc of bnet: a lhs dynamic bit insert - not a common form.
        | X_blift(W_bsubsc(X_bnet d, n, false, _)) -> 
            let id = d.id
            let left = X_bnet d
            let n' = (xsim gG (pmim) debug_pc n) //with sfault(x) -> sf(id + " eval fault rethrow:" + x)
            if !dD1.mvd >=9 then vprintln 9 ( "Comb next state for bit " + id + " is " + dnToStr nv + "\n")
            if !dD1.mvd >= 10 then vprintln 10 ((tnowfn gen) + "Assign dba " + dnToStr nv + " to " + id + " subs=" + dnToStr n' + "\n")
            let ival1 bitidx =
                let (loc, simrec) = dioset1 dD1 pmim (bitidx, VM) (left,"")
                if !loc = nv then false
                else (setpending dD1 simrec 0L loc nv bitidx true erhs; loc := nv; true)

            let rec lifted_ival = function
                | DN bitidx -> ival1 bitidx
                | DL(w, bn, _) -> ival1(int64 bn)
                | DX -> false
            let ans = try lifted_ival n'
                      with _ ->
                          vprintln 3 (debug_pc + sprintf ": lifted_ival - subscript to array %s out of range %A in %s" d.id n' (xToStr it))
                          false
            ans

        // COMB: constant bit position bitinsert.
        | X_blift(W_bitsel(X_bnet d, n, false, _)) -> 
           let it = X_bnet d
           let (loc, simrec) = dioset1 dD1 pmim (0L, VM) (it,"")
           let ov = !loc
           let nv1 = perform_bitinsert(ov, up64 n, nv)
           //if !dD1.mvd >= 4 then vprintln (tnowfn gen + (dnToStr ov) + ": Next state for bit" + i2s n + " of " + d.id + " is " + dnToStr(nv) + " giving " + dnToStr nv1)
           if nv1 = ov then false
           else
               setpending dD1 simrec -1L loc nv (up64 n) true erhs
               loc := nv1
               true

        // Combinational BNET basic scalar assign.
        | X_bnet d ->
            let id = d.id
            let (loc, simreco) = dioset1 dD1 pmim (0L, VM) (it, "")
            if !dD1.mvd >=4 then vprintln 4 ((tnowfn gen) + "Unmasked new value for " + id + " is " + dnToStr nv)// ( + sprintf " mask=" + oiToStr w)
            let nv =
                match simreco with
                    | Some sr->
                        //vprintln 0 ((tnowfn gen) + sprintf "diosim comb assign: Unmasked new value for %s is %s " id (dnToStr nv) + sprintf " mask=%A" sr.encoding_cvt)            
                        diosim_appmask (mine_prec g_bounda erhs).widtho sr.encoding_cvt CS_maskcast nv
                    | None -> nv
            if !dD1.mvd>=4 then vprintln 4 ((tnowfn gen) + "Comb new value for " + id + " is " + dnToStr nv)
            let ov = !loc
            if ov=nv then false
            else 
                let up = rez_update dD1 simreco -1L loc nv -1L true erhs
                committer dD1 true  (comb_delay_plot_helper dD1 gen) up
                loc := nv 
                true


        // Assign to old style loose nets. 
        | X_net(id, _) -> 
            if !dD1.mvd >=4 then vprintln 4 ((tnowfn gen) + "Comb new value (net) " + (dnToStr nv) + " to " + id)
            let loose loc = 
                let di = function
                    | DN v   -> down64 v
                    | DL(w, bn, _)  -> int32(*System.Convert.ToInt32*) bn
                    | _ -> sf "di"

                if nv=DX || (di nv)=(!loc) then false
                else (
                          //.. setpendin(ga, -1, loc, nv, -1, false); builtin reg monitoring not done
                          loc := di nv;
                          //D1.loosevals.add id ref nv;
                          true
                        )
            if id = "sp" && not_nonep VM then loose (valOf VM).sp 
            elif id ="pc" && not_nonep VM then loose (valOf VM).pc
            else 
                let (loc, simreco) = dioset1 dD1 pmim (0L, VM) (X_undef, id)
                (
                        setpending dD1 simreco -1L loc nv -1L true erhs;
                        loc := nv; 
                        true
                )

        | W_asubsc(X_net _, _, _) -> false (* FOR now : wond net *)

        | X_undef -> false

        | d -> sf(xToStr d + ": xsim assign lhs other")


    let (nx, it) = 
        let rec ass1 nx lhs =
            if !dD1.mvd>=4 then vprintln 4 (sprintf"xsim_assign: ass1 nx=%i lhs=%s rhs=%s" nx (netToStr lhs) (xToStr erhs))
            match lhs with
                // SEQ - better not to generate this X_x for each operation just to pattern match and discard it. And in parent!
                | X_x(it, n', _) -> ass1 (nx+n') it 
                | it -> (nx, it)
        ass1 n0 lhs0
    //let _ = vprintln 0 (sprintf "xsim_assign nx=%i to %s" nx (netToStr it))
    let ans =
        match nx with
            | 0 -> xsim_assign_comb nx it 
            | 1 -> xsim_assign_seq nx it
            | nx -> sf (sprintf "xsim: seq assign other next-state index nx=%i for it=%s" nx (xToStr it))

    if !dD1.mvd>=4 then vprintln 4 (sprintf "xsim_assign complete, n=%i to %s" nx (netToStr lhs0))
    ans


and xbsim gG (pmim) debug_pc x = 
    let (minfo, stack, HC, VM, gen, dD1) = gG
    match x with
    | X_false -> DN 0L
    | X_true  -> DN 1L
    | X_dontcare -> DX

    | W_cover(lst, _) ->
        let rec sums = function
            | [] -> DN 1L
            | [item] -> xbsim gG (pmim) debug_pc (deblit item)
            | h::t ->
                let b = xbsim gG (pmim) debug_pc (deblit h)
                if ds_monkey b then sums t else DN 0L
        let rec prods = function
            | [] -> DN 0L
            | [item] -> sums item
            | h::t ->
                let b = sums h
                if ds_monkey b then b else prods t
                
        prods lst

    | W_linp(e, LIN(polz, lst), _) ->
        let rec k1 vv pol = function
            | [] -> if (pol) then DN 1L else DN 0L
            | h::tt -> if vv < h then (if pol then DN 1L else DN 0L) else k1 vv (not pol) tt
        let ival1 vv =  k1 vv polz lst
        let k = function
           | DX           -> DX
           | DL(w, bn, _) -> ival1 (int32(*System.Convert.ToInt32*) bn)
           | DN vv        -> ival1 (down64 vv)
        let ans = k(xsim gG (pmim) debug_pc e)
        //let _ = vprintln 0 ("xbsim linp " + xbToStr x + " gave " + dnToStr ans)
        ans


    | W_bdiop(V_orred, [e], inv, _) ->
        let k = function
           | DX           -> DX
           | DL(w, bn, _) -> DN(if (bn.IsZero)=inv then 1L else 0L)
           | DN k         -> DN(if (k<>0L)<>inv then 1L else 0L)  (* call monkey *)
        k(xsim gG (pmim) debug_pc e)

    | W_bmux(gg, ff, tt, _) ->  xbsim gG (pmim) debug_pc (if (ds_monkey(xbsim gG (pmim) debug_pc gg)) then tt else ff)

    | W_bnode(V_band, lst, i, _) -> 
        let rec k = function
            | [] -> true
            | (h::t) -> ds_monkey(xbsim gG (pmim) debug_pc h) && k t
        let ans = k lst
        if ans<>i then DN 1L else DN 0L

    | W_bnode(V_bor, lst, i, _) -> 
        let rec k = function
            | [] -> false
            | (h::t) -> ds_monkey(xbsim gG (pmim) debug_pc h) || k t
        let ans = k lst
        if ans<>i then DN 1L else DN 0L

    | W_bnode(V_bxor, lst, i, _) ->
        let rec k = function
            | [] -> false
            | h::t -> ds_monkey(xbsim gG (pmim) debug_pc h) <> k t
        let ans = k lst
        if ans<>i then DN 1L else DN 0L

    | W_bdiop(oo, [d; e], inv, _) ->
        let prec = infer_prec g_bounda d e // could put this in all?
        let r0 = x_bdiocalc prec oo (xsim gG (pmim) debug_pc d) (xsim gG (pmim) debug_pc e)
        //dev_println (sprintf "x_bdiocalc result=%A,  inv=%A" r0 inv)
        let rr = if inv then dio_lognot r0 else r0
        //vprintln 0 ("diosim: W_bdiop eval of " + xbToStr x + " gave " + dnToStr rr)
        rr

    | W_bsubsc(X_bnet d, n, false, _) -> muddy "dynamic bitextract"

    | W_bitsel(e, n, inv, _) -> // static bitextract
        let e'= xsim gG (pmim) debug_pc e
        let rec bs1 (v, n) = if n = 0 then v else bs1(v / 2L, n-1)
        let q x = if inv then DN(1L-x) else DN x
        let bitsel = function
            | DX ->
                //vprintln 0 ("diosim: Undefined bitsel to " + xToStr e)
                DX
            | DN v      -> q (bs1(v, n) % 2L)
            | DL(w, bn, _) -> q (if bnum_bitsel n bn then 1L else 0L)
        bitsel e'

    | other -> sf("diosim xbsim other: " + xbkey other + " " + xbToStr other + "\n")

and xsim gG (pmim) debug_pc a =
    let (minfo, stack, HC, VM, gen, dD1) = gG
    if !dD1.mvd >= 10 then vprintln 10 ("       xsim expression start " + xkey a + " " + xToStr a)
    let dd = xsim_ntr gG (pmim) debug_pc a
    if !dD1.mvd >= 10 then vprintln 10 ("   xsim expression " + xkey a + " " + xToStr a + " returns=" + dnToStrx dd)
    dd

and xsim_ss gG (pmim) debug_pc = function  // Xsim eval for strings regardless of any 'withvalue' qualifier. 
       | W_string(s, _, _) -> DS s
       | other -> xsim gG (pmim) debug_pc other
    
and xsim_ntr gG (pmim) debug_pc ax =
    let (minfo, stack, HC, VM, gen, dD1) = gG
    match ax with
        | X_undef -> DX
        | X_num n -> (* vprint(1, (if n < 0 then "-ve" else "+ve") + "x_ num " + i2s n + ". ") *) DN (up64 n)
        | X_bnum(w, bn, m) -> DL(w, bn, m)

        | W_string(s, XS_withval x, _) -> xsim gG (pmim) debug_pc x
        | W_string(ss, _, _) ->
            if ss = "" || not(System.Char.IsDigit ss.[0]) then DS ss
            else
                let v =
                    try (atoi64 ss)
                        with _ -> 0L
                DN v
        | X_blift e -> xbsim gG (pmim) debug_pc e
        | X_pair(d, e, _) ->
            ignore (xsim gG (pmim) debug_pc d)
            let rr = xsim gG (pmim) debug_pc e
            if !dD1.mvd>=10 then vprintln 10 ("pair gave " + dnToStr rr)
            rr


        | W_node(prec, V_interm, [l; r], _) -> (ignore(xsim_assign gG (pmim) debug_pc 0 l r); DX)

        | X_bnet ff -> 
            if not_nullp ff.constval then hd(diosim_constval ff)
            else
                let rr = read_register dD1 gG (pmim) debug_pc ff.id ff.n ax
                if !dD1.mvd >= 10 then vprintln 10 ("Read reg " + ff.id + "  " + dnToStr rr + "\n")
                rr

        | X_net(id, meo) -> read_register dD1 gG (pmim) debug_pc id meo.n ax

        | W_asubsc(X_net(id, m), v, _) ->
            //vprintln 0 "Start xnet l eval"
            let l' = xsim gG (pmim) debug_pc (X_net(id, m))
            //let _ = vprintln 0 "Finish xnet l eval"
            let v' = xsim gG (pmim) debug_pc v
            let q n = function
                | DS s ->
                   let ll = up64(String.length s)
                   let i = down64 n
                   //let _ = vprintln 0 ("String subscript " + s + " at " + i2s i)
                   if n<0L || n>=ll then DX // out of range
                   else DN (up64((int)(s.[i])))
                | DX -> DX
                
            let best_ival = function
                | DN subs   -> q subs l'
                | DL(w, bn, _) -> q (int64 bn) l'
                | DX -> DX
            best_ival v'

        | W_asubsc(W_string(s, _, _), v, _) ->
            let v' = xsim gG (pmim) debug_pc v 
            let ll = up64(String.length s)
            let ival1 n =
                let i = down64 n
                //let _ = vprintln 0 ("String subscript " + s + " at " + i2s i)
                if n<0L || n>=ll then DX // out of range
                else DN (up64((int)(s.[i])))
            let rec yand_ival = function
                | DN subs      -> ival1 subs
                | DL(w, bn, _) -> ival1 (int64(*System.Convert.ToInt64*) bn)
                | DX -> DX
            yand_ival v'
        | W_query(c, tt, ff, _) -> if ds_monkey(xbsim gG (pmim) debug_pc c) then (xsim gG (pmim) debug_pc tt) else (xsim gG (pmim) debug_pc ff)



        | W_asubsc(X_bnet d, e, _) ->
            let it = X_bnet d
            let e'= xsim gG (pmim) debug_pc e
            if !dD1.mvd >= 10 then vprintln 10 ("subcs in " + dnToStr e' + "\n")
            let ival1 (i:int64) =
                let r = !(fst(dioset1 dD1 pmim (i, VM) (it,"")))
                if !dD1.mvd >= 10 then vprintln 10 (sprintf "Read array %s[%i], value=%s" d.id i (dnToStrx r))
                r

            let rec asubs_ival = function
                | DN i         -> ival1 i
                | DL(w, bn, _) ->
                    if bn < 0I || bn > 1000000000000I then
                        let _ = vprintln 1 (sprintf "diosim: +++Unreasonable array subscript to %s at %s "d.id (xToStr ax))
                        DX
                    else ival1 (int64(*System.Convert.ToInt64*) bn)
                | DX -> (vprintln 1 (sprintf "diosim: +++Undefined array subscript to %s at %s " d.id (xToStr ax));  DX)
                | other -> sf ("ival:  subscript malformed or out of range 2/2: " + dnToStr other)
            let r = asubs_ival e'
            r

        | X_subsc(X_subsc(X_bnet id, subs), e) ->
            let it = X_bnet id
            let e'= xsim gG (pmim) debug_pc e
            let subs'= xsim gG (pmim) debug_pc subs
            if e' =DX || subs'=DX then DX
            else
                let rec s2_ival = function
                   | (DN i) -> !(fst(dioset1 dD1 pmim (i, VM) (it, "")))
                   | DX -> (vprintln 0 ("diosim: +++Undefined subscript for bitsel of " + id.id);  DX)
                let rec bs1 (v, n) = if n = 0L then v else bs1(v / 2L, n-1L)
                let bitsel = function
                    | (DN v, DN n) -> DN (bs1(v, n) % 2L)
                    | (_, _) -> (vprintln 0 ("Undefined bitsel and subscript to " + id.id);  DX)
                bitsel (s2_ival subs', e')

        | W_node(prec, V_neg, [e], _) -> x_calc_neg(xsim gG (pmim) debug_pc e)        
        | W_node(prec, V_onesc, [e], _) -> x_calc_onesc(xsim gG (pmim) debug_pc e)

        | W_node(prec, V_cast cvtf, [arg],  _) ->
            let v0 = xsim gG (pmim) debug_pc arg
            diosim_appmask (mine_prec g_bounda arg).widtho prec cvtf v0

        | W_node(prec, oo, [d; e], _) -> 
            let rr = x_diocalc dD1 prec oo (xsim gG (pmim) debug_pc d) (xsim gG (pmim) debug_pc e)
            //vprintln 4  (xToStr ax + " gave " + dnToStr rr + "\n")
            rr

        | W_node(prec, oo, h::lst, _) when oo=V_plus || is_times oo ||  oo=V_bitand || oo=V_bitor || oo=V_xor  ->  // all other comassoc operators
            let rr = List.fold (fun c x -> x_diocalc dD1 prec oo (xsim gG (pmim) debug_pc x) c) (xsim gG (pmim) debug_pc h) lst
            rr

        | W_node(prec, V_interm, [X_x(X_net(d, _), n, _); e], _) ->  sf("intermediate net should have been converted to an assign for xsim")

        | W_node(prec, a, _, _) -> sf(f3o3(xToStr_dop a) + ": diosim diadic other")   

        // never matched? TODO.
        | W_node(prec, ao, _, _) -> sf(f3o3(xToStr_dop ao) + ": diosim diadic other:" + xToStr ax)


        | X_x(X_bnet f, n, _) -> 
            if n=0 then xsim gG (pmim) debug_pc (X_bnet f)
            elif n>1 then (vprintln 1 (i2s n + " higher values of next state function X_x not supported in diosim: " + xToStr ax); DX) 
            else (muddy "+++ diosim X_x not done yet"; DX)


        | W_apply((fname, gis), cf, args, _) ->
            let m_pcstop = ref None
            let rr = diosim_xcall dD1 gG m_pcstop debug_pc pmim gen fname gis cf args        
            rr
            
        | other -> sf("diosim xsim:  unsupported expression: " + xkey other + " " + xToStr other)

and diosim_xcall dD1 gG m_pcstop debug_pc pmim gen fname fis cf args =
    //dev_println (sprintf "diosim xcall %s  args=%A" fname args)
    match (fname, args) with

        | ("hpr_select", token :: items) ->
            let tok = match token with
                       | X_bnet f -> f.id
                       | other -> "not-a-net"
            let (found, sel) = dD1.selections.TryGetValue tok
            let sel = if not found then
                          let sel = ref []
                          let _ = dD1.selections.Add(tok, sel)
                          sel
                      else sel
                      
            let rec zsel pos = function
                | [] -> DN (System.Convert.ToInt64 0)
                | h::tt ->
                    let v = xsim gG (pmim) debug_pc h
                    if ds_monkey v then
                        vprintln 0 (sprintf "selected %A item %i" tok pos)
                        mutadd sel (gen, pos)
                        DN(System.Convert.ToInt64 pos)
                    else zsel (pos + 1) tt
            zsel 0 items

        | ("hpr_exchange", [location; vale; comparand; conditionalf]) ->
            let new_vale = xsim gG (pmim) debug_pc vale
            let net = diosim_interlocked_get_lval gG pmim debug_pc "hpr_exchange" location
            let ov = !net
            if ds_monkey(xsim gG (pmim) debug_pc conditionalf) then
                let comparand = xsim gG (pmim) debug_pc comparand
                let doit = ds_monkey(x_bdiocalc g_bool_prec V_deqd ov comparand)
                if doit then net := xsim gG (pmim) debug_pc vale
                if !dD1.mvd>=4 then vprintln 4 (xToStr location + ": hpr_compare_exchange: new=" + dnToStr new_vale + sprintf ", doit=%A" doit)
            else
                net := new_vale
                if !dD1.mvd>=4 then vprintln 4 (xToStr location + ": hpr_exchange: new=" + dnToStr new_vale)
            ov

        | ("hpr_atomic_add", [location; amount]) -> 
            let amount = xsim gG (pmim) debug_pc amount                
            let net = diosim_interlocked_get_lval gG pmim debug_pc  "hpr_atomic_add" location 
            let ov = !net
            let new_vale = x_diocalc dD1 (mine_prec g_bounda location) V_plus ov amount
            dev_println (xToStr location + sprintf ": hpr_atomic_add %s := %s + %s" (dnToStr new_vale) (dnToStr amount) (dnToStr ov))
            net := new_vale
            if !dD1.mvd>=4 then vprintln 4 (xToStr location + ": hpr_atomic_add new=" + dnToStr new_vale)
            new_vale

        | ("hpr_testandset", [a1; a2]) -> 
            let rr = implement_testandset gG (pmim) debug_pc (a1, xsim gG (pmim) debug_pc a2)
            rr


        | ("hpr_strlen" as fname, [a1]) ->
            let rec perform_strlen = function
                | DX     -> DX
                | DN 0L  -> DN 0L // Null string, return 0
                | DS str -> DN(int64(strlen str))
                | other -> cleanexit(sprintf "diosim: primitive function %s expected string arg, not %A" fname other)

            perform_strlen (xsim gG (pmim) debug_pc a1)
        | ("hpr_abs" as fname,  [a1])
        | ("hpr_fabs" as fname, [a1]) ->            
            let rec perform_abs = function
                | DX -> DX
                | DL(w, bn, m) ->
                    if bn < 0I then gec_DL(w, -bn) else DL(w, bn, m)
                | DN iff -> if iff < 0L then DN(-iff) else DN iff // Ignored minint overflow - TODO
                | DF dff -> if dff < 0.0 then DF -dff else DF dff
                // TODO include debug_pc in error
                | other -> cleanexit(sprintf "diosim: primitive function %s expected floating point arg, not %A" fname other)
                
            perform_abs (xsim gG (pmim) debug_pc a1)

            
        | ("hpr_bitsToSingle" as fname, [a1])
        | ("hpr_bitsToDouble" as fname, [a1]) ->
            let ss = fname="hpr_bitsToSingle"
            let yielder n = // We return a DF(double) for both calls but we check the range for the single.
                if ss && n > 3.402823E+38 then DF +infinity
                elif ss && n < -3.402823E+38 then DF -infinity
                else DF n

            let from_bits64 (i64:int64) = 
               let bytes = System.BitConverter.GetBytes i64
               yielder (System.BitConverter.ToDouble(bytes, 0))
            let from_bits32 (i32:int32) = 
               let bytes = System.BitConverter.GetBytes i32
               yielder (double(System.BitConverter.ToSingle(bytes, 0)))
            let rec to_float = function
                | DX -> DX
                | DL(w, bn, _) ->
                    if ss then
                        try from_bits32(int32 bn)
                        with _ -> DX
                    else
                        try from_bits64(int64 bn)
                        with _ -> DX
                | DN i64    ->
                    if ss then
                        try from_bits32(int32 i64)
                        with _ -> DX
                    else
                        try from_bits64(int64 i64)
                        with _ -> DX

                    // TODO include debug_pc in error
                | other -> cleanexit(sprintf "diosim: primitive function %s expected floating point arg, not %A" fname other)
                
            to_float (xsim gG (pmim) debug_pc a1)


        | ("hpr_singleToBits" as fname, [a1])
        | ("hpr_doubleToBits" as fname, [a1]) ->
            let dd = fname="hpr_doubleToBits"
            let to_bits (dff:double) =
                let ans =
                    if dd then DN(System.BitConverter.ToInt64(System.BitConverter.GetBytes dff, 0))
                    else
                        try
                            let bytes= System.BitConverter.GetBytes(float32 dff)
                            //let _ = vprintln 0 (sprintf "diosim: to_bits: %s:  %A -> %i %i %i %i" fname dff bytes.[0] bytes.[1] bytes.[2] bytes.[3])
                            DN((int64) (System.BitConverter.ToInt32(bytes, 0)))
                        with _ -> DX
                //let _ = vprintln 0 (sprintf "diosim: to_bits: %s   %A -> %A" fname dff ans)
                ans
                    
            let rec from_float = function
                | DX -> DX
                | DN n -> to_bits(double n) // Do not really expect an integer arg here ... Verilog-like semantics needed. Please define.
                | DF dff -> to_bits dff
                    // TODO include debug_pc in error
                | other -> cleanexit(sprintf "diosim: primitive function %s expected floating point arg, not %A" fname other)
            from_float (xsim gG (pmim) debug_pc a1)


        | ("hpr_KppMark", args) ->
            let _ = "this is here now a nop until we collect profiles from diosim"
            DX

        | ("hpr_alloc", [bytecount; address_space]) ->
            let xsim_vm_malloc space bytes =
                let ival64_alloc = function // 
                    | DN subs      -> subs
                    | DL(w, bn, _) -> int64 bn
                    | DX -> sf("diosim: hpr_alloc: uncertain size or address space ... ")  //xToStr ax
                    // Currently we ignore address space here and allocate everything 
                let bytes = heap_alloc_roundup (ival64_alloc bytes)
                let address_space =
                    match address_space with
                        | _ ->
                            dev_println (sprintf "diosim:  hpr_alloc missing feature: Address_space=%A " address_space)
                            "SPACE0"
                let heapmanager =
                    let (found, ov) = dD1.heap_ptrs.TryGetValue address_space
                    if found then ov
                    else 
                        let rr = ref g_runtime_heap_vm_base
                        dD1.heap_ptrs.Add(address_space, rr)
                        rr
                let ans = !heapmanager
                heapmanager := !heapmanager  + bytes
                vprintln 3 (sprintf "heap alloc %i bytes from space %s at address %i" bytes address_space ans)
                DN ans
            xsim_vm_malloc (xsim gG (pmim) debug_pc address_space) (xsim gG (pmim) debug_pc bytecount)
            


        | ("hpr_concat", [a1; a2]) ->
            let kk_concat = function
                | (W_string(s, _, _)) -> s
                | other -> dnToStr(xsim gG (pmim) debug_pc other)
            DS(kk_concat a1 + kk_concat a2)


        | ("hpr_printf", l)  
        | ("hpr_write", l)   ->
            //if !dD1.mvd >=9 then vprintln 9 ( i2s pc + ":Xcall printf/write will be executed\n")
            dio_print dD1 (map (xsim_l gG (pmim) debug_pc) l) false
            DX

        | ("hpr_writeln", l) ->
            //if !dD1.mvd >=9 then vprintln 9 ( i2s pc + ":Xcall writeln will be executed\n")        
            dio_print dD1 (map (xsim_l gG (pmim) debug_pc ) l) true
            DX
            

        | ("hpr_pause", l) -> 
            let l' = map (xsim_l gG (pmim) debug_pc) l  (* Do not advance pc *)
            m_pcstop := Some -1
            DX
            
        | ("hpr_barrier", args) ->   (* barrier and pause the same here, but different in fsmgen *)
            let l1_ = map (xsim_l gG (pmim) debug_pc) args
            m_pcstop := Some -1 (* Do not advance pc *)
            DX
            
        | ("hpr_sysexit", l) -> 
            let l' = map (xsim_l gG (pmim) debug_pc) l
            vprint 1 (if l <> [] then dnToStr(hd l') else "Diosim exit on sysexit")
            m_exitflag := true
            m_pcstop := Some g_exitpc
            DX
            

        | (fname, l) ->
            //location(gG, pc)
            vprintln 0 (debug_pc + ": Diosim: +++for now skip Xcall to " + fname)
            DX

//
// Some strings would eval to their xs value etc, so a special eval is used here..
//    
and xsim_l gG (pmim) debug_pc aA =
    match aA with
        | W_string(s, _, _) -> DS s
        | e -> 
            // if vd>=5 then vprint 5 (xkey e + " " + xToStr e + " xsim_l\n") 
            let r = xsim gG (pmim) debug_pc e
            r


let simenvdump vd (a:simrec_t) = if vd>=3 then vprintln 3 ("  SEV  " + dnToStr(!(a.current)) + "/" + (xToStr a.it) + "\n")

let bsenvdump(n, b) = vprint 2 (i2s(n) + "/" + b + "  ")

let mc_disassemble other = "disassembly not ported from mosml"



let ucode_printf dD1 (PM:real_machine_t) =
    let ds = function
        | (Idefs(n, _))-> n
        | (other) -> sf((mc_disassemble other) + " in printf string") 
    let skip = function
            | Ilabel _ -> true
            | _ -> false
    let rec s (n, c) = 
            if (n >=0 && n<65536) then
                let d = PM.code.[n]
                if skip(d) then s (n+1, c)
                else
                    let j = ds d
                    if j = 0 then rev c else s(n+1, (chr j)::c)
      
            else sf ("printf pointer error")

    let k (n) = xgen_num n
    let ptr = ref (!(PM.sp)+4)
    let nextptr() =
        let rr = !ptr
        ptr := (!ptr) + 2
        rr
    let argfun() = !(PM.mem.[nextptr()])
    let s0 = function
            | DX   -> explode("ucode_printf: Uncertain vararg from stack " + i2s(!ptr-2))
            | DN i -> s(down64 i, [])
    let s1 = s0(argfun())
    let _ = xsim_print dD1 (builtin_printf(s1, argfun))
    ()


let location ((minfo:minfo_t, stack, HC, _, _, D1), pc) =
    let htos1 jchar idl = foldr (fun(a,c)->if c="" then a else a + jchar + c) "" (rev idl)
    let htos idl = htos1 "/" idl
    vlnvToStr minfo.name + ":" + i2s pc



(*
 * Main simulator or interpreter for an orange model.
 * Returns next pc value or -1 for thread halt/exit.
 *)
let rec vm_sim gG pmim code_point pc x =
    let debug_pc = sprintf "vm_sim:%s" code_point
    let (minfo, stack, HC, PM, gen, dD1) = gG
    let vd = !dD1.mvd
    //vprintln 0 (sprintf "vm_sim cmd=%A" x)
    let code_track_point_ = (minfo, pc)
    mutinc64 dD1.stats.vm_instructions_run 1L
    let ans = 
        match x with
        | Xassign(a,b) -> (ignore(xsim_assign gG (pmim) debug_pc 0 a b); pc+1)
        | Xskip -> pc+1
        
        | Xannotate(_, s)  -> vm_sim gG pmim code_point pc s
        | Xlinepoint(_, s) -> vm_sim gG pmim code_point pc s
        | Xlabel _ -> pc+1
        (* | vm_sim G pc  (Xeasc(W_apply((f, gis), cf, l))) -> vm_sim G pc (Xcall((f, gis), l)) *)

        //| Xeasc(W_apply(f, _, l, _)) -> vm_sim gG pmim code_point pc (Xcall(f, l))

        | Xeasc X_undef -> pc+1

        | Xeasc(W_apply((fname, gis), cf, args, _)) ->
            let m_pcstop = ref None
            let _ = diosim_xcall dD1 gG m_pcstop debug_pc pmim gen fname gis cf args
            match !m_pcstop with
                | None -> pc + 1
                | Some v -> v

        | Xeasc e ->        
            if constantp e then pc+1
            else sf("diosim other easc: " + xToStr e) 

            
        | Xgoto(_, i) -> !i

        | Xwaitrel dt ->
            let f = function
                | DX -> (vprintln 1 ("uncertain value in Xwaitrel"); -1)
                | DN n -> down64 n
            let a = gen + f(xsim gG (pmim) debug_pc dt)
            if a = gen+1 then -1 else sf "diosim xwaitrel: not one - useless sim"

        | Xwaituntil g ->
            let c = ds_monkey(xbsim gG (pmim) debug_pc g)
            if c then pc+1 else pc (* Wait *)

        | Xif(g, t, f) -> 
            let g' = xbsim gG (pmim) debug_pc g
            let c = ds_monkey g'
            if vd >=9 then vprintln 9 (i2s pc + ":Xif branch condition evald to " + dnToStr g' + " monkeyed to " + boolToStr c + "\n")
            vm_sim gG pmim code_point pc (if c then t else f)

        | Xbeq(g, t, i) -> 
            let g' = xbsim gG (pmim) debug_pc g
            let c = not(ds_monkey g')  // Branch if the g expression was zero.
            let _ = cassert(!i <> -1, "diosim: beq: program was not assembled prior to simulation")
            if vd>=5 then vprintln 5 (i2s pc + ":Xbeq: branch if " + (dnToStr g')  + " is zero: taken=" + boolToStr c)
            if c then !i else pc+1

        | Xreturn e -> 
            let e' = xsim gG (pmim) debug_pc e
            //let _ = lprint 3 (fun ()->vlnvToStr minfo.name + " Xreturn " + xToStr e + " value=" + dnToStr e' + "\n")
            if stack<>None then
                let sp = valOf stack
                let pc' = if !sp = []
                          then 
                                (vprintln 3 (vlnvToStr minfo.name + ": diosim thread exit instead of return time stack empty"); g_exitpc)
                          else
                              let ans = hd (!sp)
                              let _ = sp := tl (!sp)
                              ans
                pc'
            else sf(vlnvToStr minfo.name + ": Xreturn on RM not supported")


        | Xcomment _ -> pc+1

        | other -> sf((hbevToStr other) + ": other form in vm_sim")
    //let _ = vprintln 0 (sprintf "vm_sim cmd done. ans=%A" ans)
    ans


let ureg (VM:real_machine_t) k =
    if (k < 0 || int64 k > VM.reg_count) then (vprint 1 ("diosim: +++Register out of range"); VM.regs.[0])
    else VM.regs.[k]


let amodeToS x = x.ToString() // for now

let usim_eval (G, VM) a = function
    | ABSS l       -> muddy "read_register dD1 gG (pmim) debug_pc l"
    | IMMED k      -> DN (up64 k)
    | IMMEDS(k, j) -> DN (if !j<> None then up64(valOf(!j)) else atoi64 k)
    | VREG k -> !(ureg VM k)
    | INDEXEDS(offset, basereg) -> muddy"read_register dD1 gG (pmim) debug_pc offset" (* TODO basereg.. *)
    | other    -> sf ((amodeToS other) + " unsupported simulator addressing mode")



let usim gG (pmim) debug_pc a = function
    | Icomment _ -> ()
    | Ilabel _ -> ()
    | Icompiled(ins, x) ->
        (
          //if vd>=5 then vprintln 5 ((i2s a) + ":" + (mc_disassemble ins) + " : " + xToStr x);
          ignore(xsim gG (pmim) debug_pc x);
          ()
        )
    | other ->  sf ((i2s a) + ":" + (mc_disassemble other) + ": unsupported simulator opcode")



// Return a bool: true if anything changed (not natural end).
// How much to simulate for ? Until block, get to zero, or time-slice granularity.
let xsim_vm ww (dD1:diosim_env_t) HC gen cc (pmim, arg) =
    let vd = !dD1.mvd
    match arg with 
    | S_VM vm -> // Virtual machine
        let m_pc = vm.pc
        //dev_println (sprintf "VM hashcode=%s"  (vm.serial))
        let code_point = sprintf "xsim_vm %s:%i" (vlnvToStr vm.minfo.name) !m_pc
        let idf() = xToStr vm.pcnet + " " + code_point
        if !m_pc < 0 then cc // exited: exit_status
        else
            //dev_println (sprintf "resume " + code_point)
            let rec step cycles pc = 
                let cleanp p' =
                    if   p' =  g_exitpc then p'  // Explicit exit.
                    elif p' =  -1       then pc+1 
                    elif p' >= 0        then p'
                    else sf "vm_sim" (* -1 is a yield that advances the pc *)
                    // (* thread blocked itself by returning -1 *)

                if vd>=10 then vprintln 10 (idf() + sprintf ": VM step from pc=%i budget=%i" pc cycles)
                let ucode = vm.code.[pc]
                if vd>=10 then vprintln 10 (i2s pc + ":" + hbevToStr ucode)
                let p' = vm_sim (vm.minfo, Some vm.stack, HC, None, gen, dD1) pmim code_point pc ucode
                //vprintln 0 (idf() + sprintf ": VM did step from %i budget=%i" pc  cycles)
                if cycles <= 0 then
                    if vd>=4 then vprintln 4 (sprintf "diosim: Thread %s did not block within timeslice. p'=%A" (idf()) p')
                    cleanp p'
                elif p' = vm.len then 0 // off the end: loop to top and yield.
                elif p' = 0 then p' // explict branch back to location zero is always defined as a yield. 
                elif p' = pc then p' // Not advancing the pc is also a yield.
                elif p' > 0 then step (cycles - 1) p' 
                else cleanp p'
            m_pc := step dD1.granularity !m_pc
            //dev_println (sprintf "Post pc is %i" !m_pc)
            true

    | S_RM pm -> // Real machine
        let m_pc = pm.pc
        if !m_pc < 0 then cc
        else
            let a = !m_pc
            let minfo = pm.minfo
            let jbase = 12 * 4096
            let ucode =
                if a >= jbase then 
                  (
                    if a = jbase+1 then ucode_printf dD1 pm
                    hd(!(pm.returner))
                  )
                else pm.code.[!m_pc]
            if vd>=4 then vprintln 4 (vlnvToStr minfo.name + ":" + "sp=" + i2s !pm.sp + ":" + i2s a + "::" + (mc_disassemble ucode)) 
            m_pc := a+1
            let debug_pc = sprintf "S_RM:%i" a
            usim (minfo, None, HC, Some pm, gen, dD1) pmim debug_pc (a) ucode
            true

    | (_) -> sf "diosim: unknown machine type"

let g_prbs_v = ref 3256L


let bound_log2_64 (n:int64) =
    if muddy "n >= (1UL <<< 62L)" then 63
    else
        let rec k r = muddy "if (two_to_the r) >= n+1 then r else k (r+1)"
        in k 0
                                    

let prbs topval a =
    let p = g_prbs_v;
    let bits = 1 + bound_log2_64 topval

    let nbit() =
        let rr = (!p / 32768L) ^^^ (!p/(32768L/2L))
        let nv = (rr % 2L) + !p * 2L;
        let nv = if (nv > 32768L) then nv-32768L else nv
        p := (if nv = 0L then 12345L else nv)
        rr
        
    let rec c n = if n=0 then 0L else nbit()+2L*c(n-1)
    let r = c bits 
    //let _ = print((i2s r) + " for prbs of " + (i2s bits) + " bits, topval=" ^(i2s topval) + "\n")
    if r > topval then r % topval else r


//
// hc: was lazy-like on-demand form for continuous assignments. It now has a sensitivity list.
// rtl_buf forms are added to hc
//
let xsim_hc gG j (pmim, support, g, lhs, rhs) =
    let (minfo, stack, HC, PM, gen, dD1) = gG
    // TODO : use is_triggered?
    let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
    let debug_pc = mach_id
    let g' = xbsim gG (pmim) debug_pc g
    let c = ds_monkey g'
    if c then
        let _ = mutinc64 dD1.stats.rtl_cont_instructions_run 1L
        xsim_assign gG (pmim) debug_pc 0 lhs rhs || j
    else j


// RTL assign: implied eval/commit semantics.
let xsim_rtl ww dD1 gG j rtl =
    let (minfo, stack, HC, VM, gen, dD1) = gG
    let egf pmim arc_info g =
        let g' = xbsim gG pmim arc_info g
        let c = ds_monkey g'
        //let _ = vprintln 0 (sprintf "egf %s -> %A" (xbToStr g) c)
        c

    let xsim1 pmim (arc_info:(string)) v =
        match xsim gG pmim arc_info v with
        | DN n -> n
        | DL(w, bn, _) -> int64(bn)
        | DX -> 0L
        | other -> sf (sprintf "xsim1 other lower %A" other)

    let _ = mutinc64 dD1.stats.rtl_instructions_run 1L

    //let _ = dev_println (sprintf "diosim: xsim_rtl : RTL code_trace_point=%s arc=%s" "ctp" (xrtlToStr (f3o3 rtl)))
    match rtl with
        | (pmim, dir, XIRTL_buf(g, l, r)) when nullp dir.clocks -> // Combinational assignments via buffers.
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let arc_info = mach_id
            let gc = egf pmim arc_info g
            //let _ = dev_println (sprintf "diosim: xsim_rtl XRTL_buf : egf gc=%A" gc)
            if gc then xsim_assign gG pmim arc_info 0 l r || j else j

        | (pmim, dir, XRTL(pp, ga, lst)) -> // CLOCK IS IGNORED IN THESE CLAUSES (but xi_X uses some clock or other!) !
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let arc_info = mach_id
            let lcm_atoi = xi_manifest
            let gc = egf pmim arc_info ga && match pp with
                                                | None -> true
                                                | Some(pc, pcs) ->
                                                    let v' = down64(xsim1 pmim arc_info pc)
                                                    lcm_atoi "xsim_rtl" pcs = v'
            //dev_println (sprintf "diosim: xsim_rtl XRTL : egf gc=%A" gc)
            //if gc then vprintln 0 ("Rpli ga holds " + xbToStr ga)
            let sq j = function 
                | Rarc(gb, l, r) ->
                    if egf pmim arc_info gb then
                        xsim_assign gG pmim arc_info 1 l r || j else j // This is the main RTL sequential logic assignment statement.
                        
                | Rnop _  -> j
                | Rpli(gb, (fgis, ord), args) ->
                     if egf pmim arc_info gb
                        then let _ = vm_sim gG pmim arc_info 0 (gec_Xcall(fgis, args))
                             //vprintln 0 ("Rpli gb holds " + xbToStr gb)
                             true
                        else j
            if gc then List.fold sq j lst else j

        | (pmim, dir, XIRTL_pli(Some(pc, pcs), g, (fgis, ord), args)) ->
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let arc_info = mach_id
            let v' = down64(xsim1 pmim arc_info pc)
            let lcm_atoi = xi_manifest
            let gc = lcm_atoi "xsim_rtl" pcs = v' && egf pmim arc_info g
            if gc then
                //vprintln 0 ("XIRTL_pli g holds " + xbToStr g)
                ignore(vm_sim gG pmim arc_info -1 (gec_Xcall(fgis, args)))
                true
            else j

        | (pmim, dir, XIRTL_pli(None, g, (fgis, ord), args)) ->
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let arc_info = mach_id
            let gc = egf pmim arc_info g
            if gc then
                //vprintln 0 ("XIRTL_pli None/g holds " + xbToStr g)
                ignore(vm_sim gG pmim arc_info -1 (gec_Xcall(fgis, args)))
                true
            else j

        | (pmim, dir_, rtl) ->
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let arc_info = mach_id
            sf (arc_info + ": unsupported other form of xrtl in xsim_hc: " + xrtlToStr rtl)
            

//
let genProfileReportEntries gen commits =
    vprintln 3 (sprintf "synch profile %i + ^%i" gen (length commits))
    let xer update =
        match update.simrec with
            | Some simrec when not (nonep simrec.profiling_database) ->
                //vprintln 3 (sprintf "synch profile run %s %i" simrec.id gen)    
                valOf(simrec.profiling_database).update update.update
                ()
            //| None -> vprintln 0 ("+++ no simrec in it " + update.id + dnToStr update.update)
            | _ -> ()
    app xer commits
    ()

(*
 * This simulator uses a global time called tnow and a pending update (eval/commit) model
 * that handles X(v) assigns.  History could be accessed using X(v, -1) and so on if we wanted.
 * All threads get a chance to run each delta cycle and any time all have had a go, the commit occurs.
 * If none still want to run we have a 'natural end'.
 *
 *
 * Threads can yield or be pre-empted.  Preemption occurs when their budget is exceeded, causing a commit if that was the last thread!  Nasty non-det behaviour therefore arises, as in real systems.
 *)
let hc_pseudo = { vendor="HPRLS"; library="custom"; kind=["hc psudo"]; version="1.0" }



    
let gec_diosim_global_pmim (simrec_dict:simrecs_t) =
    let m_glob_nets = ref []

    let diosim_global_binder ss = Some(id_to_simrec simrec_dict (g_diosim_global_prefix, ss))

    let diosim_global_binder__ ss =
        match List.filter (fun (_, x) -> net_id x = ss) !m_glob_nets with
            | [(prefix, vv)]  ->
                let (found, ov) = simrec_dict.TryGetValue((g_diosim_global_prefix, x2nn vv))
                if found then Some ov else None
            | other -> muddy (sprintf "diosim_global_binder: undefined %s" (ss))


    (m_glob_nets, (g_diosim_global_prefix, g_null_minfo, g_null_vm2_iinfo, diosim_global_binder, "diosim-global-environment"))


let rec diosim_deltacycle ww (hC, vVMs, hRTL, (dD1:diosim_env_t), undriven, tnow) (gen, rr) =
    let vd = !dD1.mvd
    //vprintln 0 (sprintf "diosim: Starting cycle %i/%i at tnow=%s" gen D1.sim_cycle_limit (dnToStr !tnow.current))
    let random = function
        | X_bnet ff -> 
            let it = X_bnet ff
            let id = ff.id
            let v  = DN (prbs (int64 ff.rh) id)
            let (w, _) = dioset1 dD1 dD1.gpmim (0L, None) (it, "")
            w := v 

        |  other -> sf((xToStr other) + ": random diosim_deltacycle")
    //app random undriven // Random starting values for undriven nets.
    if vd >= 8 then for a in dD1.simrecs do simenvdump 8 a.Value done
    (tnow:simrec_t).current := DN(up64 gen)
    let hc_minfo = { g_null_minfo with name=hc_pseudo; }
    let j0 = List.fold (xsim_hc (hc_minfo, None, hC, None, gen, dD1)) false hC
    //vprintln 0 (sprintf "diosim: hc done")
    let j1 = List.fold (xsim_rtl ww dD1 (hc_minfo, None, hC, None, gen, dD1)) j0 hRTL
    //vprintln 0 (sprintf "diosim: rtl done")
    let j2 = List.fold (xsim_vm ww dD1 hC gen) j1 vVMs
    //vprintln 0 (sprintf "diosim: vms done")
    //vprintln 0 (boolToStr j1 + " " + boolToStr j2)
    //      let _ = foldl (fun (x:simrec_t, cc) ->
    //            if (x.array)<> None then g_blank_update::cc else
    //            if (x.comb)=None || (!(x.generation))=gen then
    //     {id= x.id; loc= x.current; net= x.it; subs= -1; mask=0; width=width; update= !(x.current); doner=true ; }::cc 
    //            else cc) [] nets
    let tracing = dD1.plot <> None || dD1.vcd <> None
    let o_pendings  = !dD1.m_pendings
    dD1.m_pendings := []
    app (committer dD1 false (gen*dD1.timescale)) o_pendings
    //vprintln 0 (sprintf "diosim: Commits done at %A" tnow)

    let trace_pc_values (pmim, aa) =
        let log_dic_pc simrec vale =
            match simrec.profiling_database with
                | Some db -> db.update vale
                | None -> ()
            misc_trocgen dD1 false (gen * dD1.timescale, simrec, vale)
        match aa with// Add pc values to the traces always.
            | S_VM vm -> log_dic_pc vm.pc_simrec (DN(up64(!vm.pc)))
            | S_RM rm -> log_dic_pc rm.pc_simrec (DN(up64(!rm.pc)))
            | _ -> sf "other trace_pc_values"


    // Clock generation: fixed rate clock at 100 timesteps per cycle.  Another factor of dD1.timescale is multiplied in during plotting.
    if gen % 50=0 then 
                let minfo = { g_null_minfo with name=hc_pseudo; }:minfo_t
                let G = (minfo, None, hC, None, gen, dD1)
                let ck = (gen / 50) % 2
                let _ = xsim_assign G dD1.gpmim (*pc=*)"g_clknet system clock assign" 0 g_clknet (xgen_num ck)
                //let _ = if ck = 0 then genProfileReportEntries gen o_pendings
                ()
    //vprintln 0 (sprintf "diosim: clock done")

    app trace_pc_values vVMs
    let trocs = !dD1.m_e_trocs @ !dD1.m_l_trocs // Not Backwards, so put earlier first.
    let _ = (dD1.m_e_trocs := []; dD1.m_l_trocs := [])
    let rr = if tracing then (troc_collate trocs) @ rr else rr   //  Build up all tracing events in a list.
    if j2 then () else vprintln 1 ("Simulation natural end (nothing happening anymore) at " + i2s gen)
    if gen>=dD1.sim_cycle_limit then vprintln 0 (sprintf "+++ diosim: Simulation cycle limit reached at %i" gen)

    let _ = genProfileReportEntries gen o_pendings                // use this call for asynchronous designs - ie where simulation is ignoring the clock net.
    let limited = gen >= dD1.sim_cycle_limit
    let _ =
        if limited then
            let msg = "Number of simulation cycles limit reached"
            dD1.m_exit_report := msg
            vprintln 1 msg
    if (!m_exitflag || limited || not j2) then (rev rr) 
    else diosim_deltacycle ww (hC, vVMs, hRTL, dD1, undriven, tnow) (gen+1, rr) // Tailcall is main simulation process loop or iteration.


(*
 * Collect contents of a number of H2 machines for simulation.  Return hC, hS and hRTL lists.
 * Blocking assigns are put on hC list.
 *
 *  hC = comb (info, clk, SP_l ...) list  : combinational when clk=None
 *  hS = seq ...
 *  hRTL : sequential RTL
 *)
let rec xsim_tosim_c ww dD1 dir (prefix, minfo, iinfo, my_binder, mach_id) arg c0 =
    let vd = -1
    let augment_c0 (arc, arcno) (aHC, hHS, ahRTL) =
        let augment_comb code_trace_point gtot lhs rhs =
            let sup = xi_support (xi_bsupport [] gtot) rhs // TODO add lmode support for comb regfile if asynchronous write (yuck) to arrays was ever used.
            ((prefix, minfo, iinfo, my_binder, code_trace_point), map fst sup, gtot, lhs, rhs)

        let augment_seq code_trace_point arc = ((prefix, minfo, iinfo, my_binder, code_trace_point), dir, arc) 

        let code_trace_point = (mach_id + sprintf ":SP_RTL-A%i" arcno)
            //let _ = dev_println (sprintf "diosim: xsim_to_sim: RTL code_trace_point=%s arc=%s" code_trace_point (xrtlToStr arc))
        match arc with
        // Sort order of PC in bmux2 guards may lift it and make other guards eval'd non short-circuit!
        // Hence we use the double-nested Xif to preserve strictness.
            | XRTL_nop msg -> (aHC, hHS, ahRTL)
            | XIRTL_pli _  -> (aHC, hHS, ((prefix, minfo, iinfo, my_binder, code_trace_point), dir, arc)::ahRTL)
            | XRTL(pp, ga, lst) ->
                let gg = (if pp=None then ga else xi_and(ga, xi_deqd(fst(valOf pp), snd(valOf pp))))
                let sq cde (aHC, hHS, ahRTL) =
                    match cde with
                    | Rnop _  -> (aHC, hHS, ahRTL) 
                    | Rpli(gb, fgis, args) ->
                        (aHC, hHS, (augment_seq code_trace_point (XIRTL_pli(pp, ix_and gg gb, fgis, args)))::ahRTL)
                    | Rarc(gb, l0, r) ->
                        let (l, buffer) = find_blka l0
                        if buffer then ((augment_comb code_trace_point (ix_and gg gb) l r)::aHC, hHS, ahRTL)
                        else
                            let newarc = XRTL(pp, ga, [cde])
                            (aHC, hHS,  (augment_seq code_trace_point newarc)::ahRTL)

                List.foldBack sq lst (aHC, hHS, ahRTL)
            | XIRTL_buf(g, l, r) ->
                //if clk<>[] then vprintln 0 ("diosim: +++clocked XIRTL_buf? Domain ignored: " + xrtlToStr arc)
                ((augment_comb code_trace_point g l r)::aHC, hHS, ahRTL)
            //| other -> sf (sprintf "xsim_to_sim: other RTL arc form %A" other)

    match arg with
    | SP_rtl(ii_, arcs) ->
        mutinc64 dD1.stats.static_rtl_statement_count (int64(length arcs))
        let ans = List.foldBack augment_c0 (zipWithIndex arcs) c0
        ans 
    // Pathological example (cv1):
    // if (p && q) { p := false; r := r+1; } is converted to
    // begin if (p && q) { p := false; } 
    //       if (p && q) { r := r+1; } 
    //       end
    // If the assignment to p is blocking in the output formalism, then r fails to be incremented.
    | SP_l(ast_ctrl, bev) when nullp dir.clocks -> // no clock: continuous assignment
        let code_trace_point = (mach_id + ":SP_l_continuous")
        let m_cs = ref []
        let rec scan g cmd =
            match cmd with
            | [] -> ()   // This is a 'cv1' routine. BUG! TODO.
            | Xcomment _ :: tt 
            | Xskip :: tt         -> scan g tt
            | (Xblock v)::tt      -> scan g (v @ tt)
            | Xassign(l, r)::tt   -> (mutadd m_cs (g, hd cmd); scan g tt)
            | Xif(g1, t, f)::tt   -> (scan (ix_and g g1) [t]; scan (ix_and g (xi_not g1)) [f]; scan g tt)
            | other::tt -> sf ("diosim: cannot sim unclocked behavioural command " + hbevToStr other)
        let _ = scan X_true [bev]

        let dodo (hHC, hHS, hRTL) (g, a) =
            match a with
                | Xassign(X_x(a, n, _), r) when n = 1 ->  // Sequential assign - standard RTL semantic inside a software-style behavioural block..
                     (hHC, hHS, ((prefix, minfo, iinfo, my_binder, code_trace_point), dir, gen_XRTL_arc(None, g, a, r))::hRTL) 

                | Xassign(X_x(lhs, 0, _), rhs)            // Combinational or immediate assign - the normal variable assignment in imperative software.
                | Xassign(lhs, rhs) -> 
                    //let _ = vprintln 0 ("diosim has a combinational driver for " + xToStr lhs)
                    let sup = xi_support (xi_bsupport [] g) rhs
                    dev_println ("diosim, lhs=" + xToStr lhs + " assign may not be supported? rhs=" + xToStr rhs)
                    (((prefix, minfo, iinfo, my_binder, code_trace_point), map fst sup, g, lhs, rhs)::hHC, hHS, hRTL) // Assign is added to HC list, but need to check not to X_x !
        List.fold dodo c0 !m_cs

    | SP_l(ast_ctrl, g) when not_nullp dir.clocks -> // cf unclocked clause  above 
        // TODO use dir.clk_enables and dir.clknet etc.. DIOSIM failes to observe the director at the moment.
        let ww' = WN "SP_l" ww
        let msg = mach_id + ":" + ast_ctrl.id
        let (minfo, dir, ans) = compile_sp_to ww' vd dir msg minfo arg
        let (hHC, hHS, hRTL) = c0        
        (hHC, ((prefix, minfo, iinfo, my_binder, mach_id), dir, ans)::hHS, hRTL)  // Other clocked SP_l forms added to HS sync list !

    | SP_fsm _ ->
        let (fsm_rtl, resets) = fsm_flatten_to_rtl ww mach_id arg
        let _ = cassert(not_nullp dir.clocks, "had a non-clocked FSM");
        let resets =
            let code_trace_point = (mach_id + ":SP_fsm_reset")
            map (fun x -> ((prefix, minfo, iinfo, my_binder, code_trace_point), {g_null_directorate with clocks=[E_any g_construction_big_bang]; duid=next_duid()}, x)) resets
        mutinc64 dD1.stats.static_rtl_statement_count (int64(length fsm_rtl))
        let redecorate ((id, nn), rtl) = (rtl, valOf_or nn -9999) // TODO this discards some code trace point info!
        let (hHC, hHS, hRTL) = List.foldBack augment_c0 (map redecorate fsm_rtl) c0
        (hHC, hHS, resets @ hRTL)

    // Add clk to each HS list item so we can prioritise them? 
    | SP_dic(dic, ctrl) ->
        let (hHC, hHS, hRTL) = c0        
        let (improved, _) = assembler_vm ww vd ctrl false dic // Ensure DIC is assembled but we'd like to ignore optimisations also returned and implemented by the assembler. However, the Xblock removal.flattening _is_ needed by diosim at the moment.
        let nn = if true then improved else SP_dic(dic, ctrl)
        (hHC, ((prefix, minfo, iinfo, my_binder, mach_id), dir, nn)::hHS, hRTL)


    | SP_par(_, lst) ->
        let code_trace_point = (mach_id + ":SP_par")
        List.foldBack (xsim_tosim_c (WN "SP_par" ww) dD1 dir (prefix, minfo, iinfo, my_binder, mach_id)) lst c0

    | SP_seq(lst) ->
        let (dir99, total) = sp_catenate dir lst
        let code_trace_point = (mach_id + ":SP_seq")        
        xsim_tosim_c ww dD1 dir99 (prefix, minfo, iinfo, my_binder, mach_id) total c0
        
    | SP_asm _ ->
        let (hHC, hHS, hRTL) = c0        
        (hHC, ((prefix, {g_null_minfo with name=hc_pseudo }, iinfo, my_binder, mach_id), dir, arg)::hHS, hRTL)

    | SP_comment k -> c0 (* comment discarded here *)

    | other -> 
      //let (pc, states, startstate, controllerf) = K2
      sf("xsim_tosim_c other: " + hprSPSummaryToStr other)


// A database of transitions
type transition_db_t(prefix:dio_instance_t, net:hexp_t) = class

    let history = ref []
    let val_history = ref []    
    let m = netToStr net

    member this.log (x:troc_t) =
        mutadd history x
        if nullp !val_history || x.v <> hd !val_history then mutadd val_history x.v
        ()


    member this.getnet() = net
    member this.hist() = rev !history
    member this.val_hist() = rev !val_history
    

    member this.compare_db (other:transition_db_t) cc =
        let mergerun pflag =
            let s str = if pflag then vprintln 2 str
            let fails = ref false
            let rec merger = function
                | ([], []) -> ()
                | (aas, [])  -> s (m + sprintf " other ran out of events before self")
                | ([], oos)  -> s (m + sprintf " ran out of events before the other other")
                | ((a:troc_t)::aas, (o:troc_t)::oos)  ->
                    let _ = if a.subs <> o.subs then muddy "subscripts used"
                    let _ = if a.v <> o.v then (fails := true; s (m + sprintf " diff at t=%i %s  cf t=%i %s" a.timer (dnToStr a.v) o.timer (dnToStr o.v))) else s (m + sprintf " same at t=%i %s" a.timer (dnToStr a.v))
                    if a.timer = o.timer then merger (aas, oos)
                    elif a.timer < o.timer then merger (aas, o::oos)
                    else merger (a::aas, oos)
            let _ = merger (this.hist(), other.hist())
            !fails

        let _ = cassert(other.getnet() = net, "Compared databases have different nets.")
        let fails = mergerun false
        let _ = if this.val_hist() <> other.val_hist() then vprintln 0 (m + " value history different")
        let _ = if fails then vprintln 1 (m + " db check failed")
        let _ = if fails then ignore(mergerun true)
        if fails then (prefix, net_id net) :: cc else cc

end

(*
 * 
 *)
let xsim_tosim ww dD1 (prefix, minfo, iinfo, my_binder, mach_id) (hHC, hHS, hhRTL) arg =
    match arg with
    | H2BLK(dir, sp_item) when nullp dir.clocks -> // Ignore reset and clk_enables if no clocks
        let msg = "diosim: continuous/unclocked"
        let ww' = WF 3 msg ww ("prepare len=" + hprSPSummaryToStr sp_item)
        let (hHC', hHS', hhRTL') = xsim_tosim_c ww' dD1 dir (prefix, minfo, iinfo, my_binder, mach_id) sp_item (hHC, hHS, hhRTL)
        let _ = unwhere ww
        (hHC', hHS', hhRTL')

    | H2BLK(dir, sp_item) ->
        let msg = "diosim: clock on " + (sfold edgeToStr dir.clocks)
        let ww' = WF 3 msg ww ("prepare len=" + hprSPSummaryToStr sp_item)        
        let (hHC', hHS', hhRTL') = xsim_tosim_c ww' dD1 dir (prefix, minfo, iinfo, my_binder, mach_id) sp_item (hHC, hHS, hhRTL)
        let _ = unwhere ww
        (hHC', hHS', hhRTL')

   // | _ -> sf("xsim_tosim")




let id_to_net (mapping:simrecs_t) (prefix, id) =
        (id_to_simrec mapping (prefix, id)).it
    
let diosim_gentrace dD1 m trocs prefix net =      //  Filter trocs collating for one target net.
    let tig (h, l) = if (h=1I && l=0I) then "" else sprintf "[%A:%A]"  h l

    let simrec = id_to_simrec dD1.simrecs (prefix,  net_id net)
    let target_cll = simrec.cll
    let rr =
        let diosim_gentrace_w l =
            let ypred (x:troc_t) =
                match x.tsrc with
                    | simrec -> simrec.cll = target_cll
            List.filter ypred l
    
        diosim_gentrace_w trocs 


    let jf f2 = varmodeToStr(f2.xnet_io)
    let kf = function
        | X_bnet ff::_   ->
            let f2 = lookup_net2 ff.n
            (ff.id, tig(ff.rh, ff.rl) + "/" + jf f2, ff)
        | X_net(ss,_)::_ -> (ss, "<>", g_null_net_att)
        | []             -> ("??", "<>", g_null_net_att)
    let kf = function
        | []      -> kf []
        | troc::_ -> kf [troc.tsrc.it]
    let (id, handles, ff) = kf rr
    let legend = id + handles
    vprintln 3 ("Diosim " + m + " : " + legend)
    //vprintln 4  ("Diosim trace done of : " + legend + "\n")
    (ff, legend, rr)

let is_alevel it =
    match it with
        | X_bnet ff ->
            let f2 = lookup_net2 ff.n            
            f2.vtype = V_VALUE && not ff.is_array // A value and not an array.
        |  _ -> sf("is_alevel other")

(*
 * Can only display level variables with this viewer at the moment
 *)
let write_diosim_vcd ww dD1 (fdn, id, title, endtime, decls, (trocs:troc_t list), tracenet_names) =  
    //let _:(dio_instance_t * string list) list = tracenet_names
    let vcdl = new vcd_writer(title)
    let tracenets =
        let prex (prefix, net_name) cc =
            match id_to_simreco dD1.simrecs (prefix, net_name) with
                | None ->
                    let _ = vprintln 3 (sprintf "Cannot find tracing %s  %s" (hptos prefix) net_name)
                    cc
                | Some simrec -> simrec.it :: cc
        map (fun (prefix, net_names) -> (prefix, List.foldBack prex net_names [])) tracenet_names
    let ww = WN "diosim_vcd" ww
    let _ = vprintln 3 ("Diosim_vcd " + i2s(length tracenets) + " nets to trace. Total trocs=" + i2s(length trocs))
    let tracenets2 = map (fun (prefix, nets) -> (prefix, List.filter is_alevel nets)) tracenets
    let _ = vprintln 3 ("Diosim_vcd " + i2s(length tracenets) + " fluent nets to trace. ")
    //let _ = dev_println (sprintf "Tracenets2 is %A" tracenets2)
    vcdl.dump ww dD1 tracenets2 trocs fdn


type db_t = Map<dio_instance_t * string, transition_db_t>

let g_saved_databases = new Dictionary<string, db_t>() 

let diosim_save_db ww dD1 (name, id, endtime, (trocs:troc_t list), tracenet_names) =  
#if UNUSED
    use writer = System.Xml.XmlWriter.Create  "foo1.xml"
    let xs = System.Xml.Serialization.XmlSerializer typeof<trocs_t >
    let _ = xs.Serialize (writer, trocs)
#endif
    let tracenets = map (fun (prefix, name) -> (prefix, id_to_net dD1.simrecs (prefix, name))) tracenet_names
    let m = ("Diosim_save db " + i2s(length tracenets) + " nets to trace. Total trocs=" + i2s(length trocs))
    let ww = WF 3 "diosim_save_db" ww m
    let (found, _) = g_saved_databases.TryGetValue name
    let _ = if found then cleanexit(m + " there is already a saved db")
    let tracenets2 = tracenets // List.filter is_alevel tracenets
    let gen (prefix, net) =
        let db = new transition_db_t(prefix, net)
        let (f, legend, trocs) = (diosim_gentrace dD1 m trocs prefix net)
        let _ = app (fun e -> db.log e) trocs
        db

        
    let dbs = List.fold (fun (cc:db_t) ((prefix, net), v) -> cc.Add ((prefix, net_id net), v)) Map.empty [ for x in tracenets2 -> (x, gen x) ]
    g_saved_databases.Add(name, dbs)
    vprintln 1 (sprintf "Saved diosim event database under tag '%s'" name)
    ()

let apToStr (prefix, id) = hptos prefix + ":" + id 

//
// Diff : Read a database file to compare with the current simulation against.
//
let diosim_check_against_db ww dD1 (name, id, endtime, (trocs:troc_t list), tracenet_names) =  
    let tracenets = map (fun (prefix, name) -> (prefix, id_to_net dD1.simrecs (prefix, name))) tracenet_names
    let m = sprintf "Check diosim result against tag '%s'" name
    let ww = WF 3 "diosim_check_against_db" ww m
    let (found, old_dbs) = g_saved_databases.TryGetValue name
    let _ = if not found then
                let  _ = vprintln 0 (m + " diosim: +++ cannot find saved db " + name)
                cleanexit(m + " cannot find saved db " + name)
    
    let tracenets2 = tracenets // List.filter is_alevel tracenets
    let gen (prefix, net) =
        let db = new transition_db_t(prefix, net)
        let (f, legend, trocs) = (diosim_gentrace dD1 m trocs prefix net)
        let _ = app (fun e -> db.log e) trocs
        db
        
    let new_dbs = List.fold (fun (cc:db_t) ((prefix, net), v) -> cc.Add ((prefix, net_id net), v)) Map.empty [ for x in tracenets2 -> (x, gen x) ]

    let compare c x =
        match (old_dbs.TryFind x, new_dbs.TryFind x) with
            | (None, None)     -> (vprintln 1 (m + sprintf ": %s was not in old or new" (apToStr x)); c)
            | (None, Some n)   -> (vprintln 1 (m + sprintf ": %s was not in old db" (apToStr x)); x::c)
            | (Some o, None)   -> (vprintln 1 (m + sprintf ": %s was not in new db" (apToStr x)); x::c)
            | (Some o, Some n) -> o.compare_db n c
    let allsame = List.fold compare [] tracenet_names
    let _ = vprintln 0 (m + sprintf " dissimilar=%A" (map apToStr allsame))
    ()



//
//        Old unix-plot style output
//
let diosim_plot_display ww dD1 (fdn, id, title, endtime, trocs, diops) tracenet_names =  
    let m = "diosim_plot_display"
    let tracenets =
        let prex_filter (prefix, net_name) cc =
            match lookup_net_by_string net_name with // Delete any not known - yuck silly filter - please recode so input is not in net name form to start with.
                | None ->
                    vprintln 3 (sprintf "Cannot find tracing %s  %s" (hptos prefix) net_name)
                    cc
                | Some net -> (prefix, net) :: cc
        List.foldBack prex_filter tracenet_names []

    let _ = vprintln 3 ("Diosim plot " + i2s(length tracenets) + " nets to trace")
    let tracenets = tracenets // List.filter is_alevel tracenets
    let traces = map (fun (prefix, net) -> diosim_gentrace dD1 m trocs prefix net) tracenets
    let traceno = length traces
    let _ = vprintln 3 (i2s traceno + " simulation traces collated.")
    let starttime = 0
    let x0 = 230 // TODO:  read plot pixel size from xml recipe !
    let xs = 810
    let span = min (endtime-starttime) 100
    let VV = {
                yofn=       fdn 
                xs=         xs 
                ys=         460 + (if traceno > 15 then 4 *(traceno-15) else 0) 
                x0=         x0
                hscale=     (xs-x0) / span
                starttime=  starttime
             }
    diodisp title (id, 1, traces, VV, diops); 
    vprintln 2 ("Simulation traces in old unix plot format written to file " + fdn)
    ()


let rec lname_op_assoc search lst =
    let lname =
        match search with
            | X_bnet ff -> // Match also on lname - provided pi_name is null
                let f2 = lookup_net2 ff.n
                if nonep (at_assoc g_port_instance_name f2.ats) then at_assoc g_logical_name f2.ats            
                else None

            | _         -> None
    let sid = net_id search
    let eq kk =
        let ans = kk = sid || (not_nonep lname && kk=valOf lname)
        //dev_println (sprintf "lname_op_assoc %s cf %s     %A" (kk) (netToStr search) ans)
        ans
    let rec scan = function
        | [] ->  None
        | (kk, vv)::tt -> if eq kk then Some vv else scan tt
    scan lst


(*
 * 
 *)
let rec diosim_prepare ww hh_level stack prefix dD1 vm_flat_index m_anets parent_bindo parent_non_executable topf_ coc name iinfo atts contacts =
    let prefix = iinfo.iname :: prefix                    

    let portmeta =
        let desa arg cc =
            match arg with 
            | Nap(k, v) -> (k, v) :: cc
            | _         -> cc
        List.foldBack desa atts []
    if memberp name stack then cleanexit(sprintf "diosim: recursive hardware VM2 structure through " + sfold vlnvToStr stack)
    else

    let (rides, actuals_portmap) =
        let rec bazzer ridef arg (cc, cd) =
            match arg with
                | DB_group(meta, lst) -> List.foldBack (bazzer (meta.form=DB_form_meta_pram || meta.form=DB_form_pramor)) lst (cc, cd)
                | DB_leaf(Some formal, Some actual) ->
                    if ridef then ((net_id formal, actual)::cc, cd)
                    else (cc, (net_id formal, (actual, ref false))::cd) 
                | _ -> (cc, cd)
        List.foldBack (bazzer false) contacts ([], [])

    vprintln 3 (sprintf "diosim bind: %s with %i parameter overrides and %i actual contacts" (hptos prefix) (length rides) (length contacts))
    let _:Map<string list, hpr_machine_t> = vm_flat_index 
    let instantiated_vm2 = 
        match vm_flat_index.TryFind name.kind with
            | None ->
                let failfun _ = ""
                cvipgen.dyno_rez_ipblock ww failfun name portmeta (map (fun (k,v)->(k, xToStr v)) rides)
            | Some vm2 -> vm2

    match instantiated_vm2 with
        | HPR_VM2(minfo, decls, children, execs, assertions) ->
            let stack = name::stack
            let mach_id = hptos prefix // rdot_fold(if nullp iname then minfo.name else iname)
            let (nets, hHC, hHS, hhRTL, inits) = coc
            let null_executable = parent_non_executable && nullp execs && nullp assertions // Detect that this is just a binder level of the VM hierarchy
            let m_scopenets = ref []
            let m_unbound_formals = ref []
            mutadd m_anets (prefix, m_scopenets)
            let rec bindate pramor__ arg (cc:Map<string, simrec_t>) =
                //dev_println (sprintf "bindate bet_id=%s arg=%A" arg)
                match arg with
                | DB_group(meta, lst) ->
                    let is_pram = (meta.form=DB_form_pramor || meta.form=DB_form_meta_pram)
                    if is_pram then
                        List.foldBack (bindate true) lst cc
                    else List.foldBack (bindate false) lst cc

                | DB_leaf(Some formal, None) ->
                    match lname_op_assoc (formal) actuals_portmap with
                        | Some (actual, used_flag) ->
                            //dev_println (sprintf "  diosim bind:  binding made:  prefix=%s formal=%s  actual=%s" (hptos prefix) (xToStr formal) (xToStr actual))
                            used_flag := true
                            let bf:(string -> simrec_t option) = valOf_or_fail "L3175" parent_bindo // Look up the actual in the parent scope and bind its srec to our formal.
                            match bf (net_id actual) with
                                | Some (vv:simrec_t) ->
                                    //vprintln 0 (sprintf "Parent has binding to " + xToStr vv.it)
                                    //vprintln 0 (sprintf "diosim bind: at %s formal=%A  actual=%s  ultimate=%s" (hptos prefix) (xToStr formal) (xToStr actual) (xToStr vv.it))
                                    cc.Add(net_id formal, vv)
                                | None ->
                                    vprintln 3 (sprintf "Parent offers no binding to this i/o contact: %A" (netToStr formal))
                                    mutadd m_unbound_formals formal
                                    //vprintln 0 (sprintf "diosim bind: at %s formal=%A  actual=%s noultimate" (hptos prefix) (xToStr formal) (xToStr actual))
                                    let net = actual
                                    let vv = gen_srec dD1 inits prefix net
                                    mutadd m_scopenets (prefix, net)
                                    cc.Add(net_id formal, vv)

                        | None ->
                            let net = formal
                            let simrec = gen_srec dD1 inits prefix net
                            vprintln 1 (sprintf "diosim bind: log local net prefix=%s id=%s. No binding for formal %s" (hptos prefix) (netToStr net) (net_id formal))
                            mutadd m_unbound_formals formal
                            mutaddonce m_scopenets (prefix, net)
                            cc.Add(net_id net, simrec)

                | DB_leaf(None, Some local) ->
                    let net = local
                    let simrec = gen_srec dD1 inits prefix net
                    vprintln 3 (sprintf "diosim bind: log local net prefix=%s id=%s" (hptos prefix) (netToStr net))
                    mutaddonce m_scopenets (prefix, net)
                    cc.Add(net_id net, simrec)
                        
                | DB_leaf(formal_o, actual_o) ->
                    let count = (if nonep formal_o then 0 else 1) + (if nonep actual_o then 0 else 1)

                    match count with
                        | 0 ->
                            hpr_yikes (sprintf " empty DB_leaf encountered")
                            cc

                        | 2 ->
                           dev_println (sprintf "diosim: prefix=%s: bound formal in definition not expected now.  count=%i formal=%s actual=%s" (hptos prefix) count (xToStr (valOf formal_o)) (xToStr (valOf actual_o)))
                            //let formal = net_id net
                            // if hexpt_is_io net then
                           cc
                        | _ ->
                            dev_println (sprintf "diosim: prefix=%s: bound formal in definition not expected now.  count=%i " (hptos prefix) count)
                            let net = if nonep actual_o then valOf formal_o else valOf actual_o
                            let simrec = gen_srec dD1 inits prefix net
                            vprintln 3 (sprintf "  diosim log local net prefix=%s id=%s" (hptos prefix) (netToStr net))
                            mutaddonce m_scopenets (prefix, net)
                            cc.Add(net_id net, simrec)
                   
            let local_bindings = List.foldBack (bindate false) decls Map.empty

            let _ =
                let m_xerror = ref false
                let check_was_used (formal, (actual, used_flag)) =
                    if not !used_flag then
                        m_xerror := true
                        hpr_yikes(sprintf "diosim: +++ prefix=%s:  component has no formal named %s that was meant to be bound to %s" (hptos prefix) (formal) (net_id actual))
                app check_was_used actuals_portmap
                if !m_xerror then
                    let pline ss = vprintln 0 ss
                    pline (sprintf "Unbound formals available were " + sfold netToStr !m_unbound_formals)
                    pline (sprintf "Actuals available were " + sfold (fst) actuals_portmap)
                    anal_decls_print pline "formal(s) available include "  decls
            // Name scoping: We should inherit from parents when a definition is within a definition, but not when instanced).
            let inheriting = true  // TODO this is left as true always for the moment.
            
            let my_binder id =
                match local_bindings.TryFind id with
                    | None ->
                        if id = "reset" || id = "clk" then None // dD1.default_reset_simrec
                        elif inheriting && not_nonep parent_bindo then (valOf parent_bindo) id
                        else
                            let report =
                                let rex key vale = vprintln 0 (sprintf "  Scope contains binding for %A " key) 
                                Map.iter rex local_bindings
                                ()
                            sf (sprintf "diosim: no net binding in scope (out of %i) at prefix=%s for net named '%s'" local_bindings.Count (hptos prefix) id)
                    | Some bix -> Some bix

            let anals() =
                let norecurse = true 
                let xx = analyse ww norecurse "Simulate analysis:" (YO_null) (*concisef*)true (*costsf*)false false (HPR_VM2(minfo, decls, children, execs, assertions))
                xx

            let initials =
                let initialp ar cc =
                    match ar with
                        | O_INITIAL a -> a::cc
                        | _ -> cc
                List.foldBack initialp assertions [] 
            vprintln 3 (mach_id + sprintf ": Machine simulation: %i rules" (length execs))
            app (fun x->vprint 3 (execSummaryToStr x + " hblck\n")) execs
            vprintln 3 (mach_id + ":                     " + i2s(length initials)  + " initialisations")

            let inits = unify inits initials // Find starting settings that meet all initial conditions.
            
#if SPARE
            let m k = vprintln 3 ("  machine net " + netToStr k)
            let nodes = map snd nodes
            let _ = app m nodes
            let _ = app (mutaddonce anets) nodes
#endif 
            let (hHC, hHS, hhRTL) = List.fold (xsim_tosim (WN "xsim_tosim" (WN ("vm=" + mach_id) ww)) dD1 (prefix, minfo, iinfo, my_binder, mach_id)) (hHC, hHS, hhRTL) execs
            let _ = WN ("vm=" + mach_id + " done") ww

            let coc = (nets, hHC, hHS, hhRTL, inits)
            let diosim_prepare_child ww arg cc =
                match arg with 
                | (iinfo, None) -> cc

                | (iinfo, Some vm2) when not parent_non_executable && iinfo.definitionf ->
                    let _ = vprintln 3 (sprintf "Skip simulation of non-root definition (%A, %A) of %s" parent_non_executable topf_ (vlnvToStr_full iinfo.vlnv))
                    cc // This is our autodetect simulation root mechanism: Do not simulate VM2 definitions that have executable code above them. We could instead use and explicit nomination.

                | (iinfo, Some(HPR_VM2(minfo, decls, _, _, _))) -> 
                    let ww = WF 3 "diosim: prepare tree:" ww (sprintf "Start bindings for kind=%s  iname=%s" (vlnvToStr minfo.name) (hptos(iinfo.iname::prefix)))
                    // let _:int -> simrec_t option = my_binder
                    diosim_prepare ww (hh_level+1) stack prefix dD1 vm_flat_index m_anets (Some my_binder) null_executable false cc minfo.name iinfo minfo.atts decls

            List.foldBack (diosim_prepare_child ww) children coc



let lifted_compile1 (a:real_machine_t) ins = muddy "needs microcode.dll: lifted_compile(myset, a.regsbase, a.symbols) ins"


let faultersf x = ()

let generic_vm_constructor dD1 pmim dir (pc_net, pc_simrec) arg =
    let (prefix, (minfo:minfo_t), iinfo_, my_binder, mach_id) = pmim
    match arg with
        | SP_dic(dDIC, id) ->
            if not_nullp dir.clocks then faultersf "diosim - clock sens ignored"
            let vm = 
                   { dir=       dir // TODO halt on abend code set in director
                     pc=        ref 0
                     stack=     ref []
                     code=dDIC; minfo=minfo
                     pcnet=     pc_net
                     pc_simrec= pc_simrec // newsimrec dD1 None prefix pc_net
                     len=       dDIC.Length
                     prefix=    prefix
                     serial=    funique "dictate"
                   }
            vprintln 3 (sprintf "diosim: rezzed S_VM %s  dic_length=%i" vm.serial vm.len)
            (pmim, S_VM vm)
          
        | SP_l(ast_ctr, cmd) ->
            let dDIC = Array.create 2 (Xskip)
            if not_nullp dir.clocks then faultersf "diosim - clock sens ignored"
            Array.set dDIC 0 cmd
            Array.set dDIC 1 (Xgoto(funique "diosimstart", ref 0))
            let vm =
                     { dir=       dir
                       pc=        ref 0
                       stack=ref []; code=dDIC; minfo=minfo
                       pcnet=     pc_net
                       pc_simrec= newsimrec dD1 None prefix pc_net
                       prefix=    prefix
                       len=       2
                       serial=    funique "mini-dictate"
                       }
            vprintln 3 (sprintf "diosim: rezzed S_VM %s  dic_length=%i" vm.serial vm.len)    
            (pmim, S_VM vm)
        | _ -> sf "generic_vm_constructor: other  "


(*
 * Main simulator - everything is elsewhere converted back to an H2 abstract machine, including
 * RTL and microcode, before simulation.
 *)
let diosimulate ww (sim, machines, dD1:diosim_env_t) = 
    let vd = !dD1.mvd
    let design_id = "anonit" // for now
    vprintln 1 (sprintf "Diosim simulation starting. %s.  %i top-level machines." design_id (length machines))

    let tnownet = X_bnet(register16("tnow16", [])) (* Hmmm; there's another of these elsewhere ! *)
    let makenative id = X_bnet(register16(id, []))

    vprintln 1 "diosim: ANOTHER NATIVE FUNCTION TABLE!: Please conglomorate. This one contains EIS-useful procedures. TODO."

// old
//  let natives = g_clknet::(xi_blift(reset()))::tnownet::(map makenative [ "hpr_barrier"; "hpr_sysexit"; "hpr_write"; "hpr_writeln"; "hpr_pause" ])

    let fg1 cc (nN, sk) = singly_add nN cc 

    let m_anets = ref []

    
    let (simname, iinfo, rootname) =
        match machines with
            | [(iinfo, Some(HPR_VM2(minfo, decls, children, execs, assertions)))] -> (iinfo.iname, iinfo, minfo.name)
            | _ -> muddy "diosim: multiple-machines"
               
    let clears = []
    
    let endtime = dD1.sim_cycle_limit

    let vm_flat_index =
        if true then List.foldBack (vm2_definitions_flatten_and_index ww) machines Map.empty
        else Map.empty

    let hh_level = 1
    let (nets__, hHC, hHS, hRTL, inits) = (diosim_prepare ww hh_level [] [] dD1 vm_flat_index m_anets None) true true ([], [], [], [], []) rootname iinfo [] []

    let m_glob_nets = dD1.m_global_nets
    mutadd m_anets (g_diosim_global_prefix, m_glob_nets)

    let net_spawn (m_nets, pmim, net) = 
       let (prefix, (minfo:minfo_t), iinfo_, my_binder, mach_id) = pmim    
       let simrec = gen_srec dD1 inits prefix net
       vprintln 3 (sprintf "  diosim log net h_level=%i prefix=%s id=%s" hh_level (hptos prefix) (netToStr net))
       mutaddonce m_nets (prefix, net)
       simrec

    let tnow = net_spawn (m_glob_nets, dD1.gpmim, tnownet)
    let _ = map net_spawn [ (m_glob_nets, dD1.gpmim, g_clknet); (m_glob_nets, dD1.gpmim, g_resetnet) ]


    let ww = WF 3 "diosim_prepare" ww (simname + "done          " + i2s(length hHC) + " HC nodes")

    let _ =
        let queries =
            { g_null_queries with
                concise_listing=   true
                yd=                YOVD 3
            }
        let rephs ((prefix, minfo, iinfo, my_binder, mach_id), dir, blk) = anal_block ww None queries (H2BLK(dir, blk))
        app rephs hHS

    let traceme = function
        | _ (*V_LEVEL*) -> true
        //| (_) -> false 


    let regs_array(n, names) = 
        let rr = dxarray_ctor "regs" [n] []
        rr


    let hcgen (cc:combinations_t) = function // Mux up all updates to a combinational net and form the union of its support.
        | (pmim, ed, g, l, r) ->
            let (prefix, minfo, iinfo, my_binder, code_trace_point) = pmim
            let nn = x2nn l
            let ov = cc.TryFind((prefix, nn))
            let nv =
                match ov with
                    | Some(pmim_, ed0, r0) -> (pmim, lst_union ed ed0, xi_query(g, r, r0)) // nb we have special structures for lists of support but not used here.
                    | None when g=X_true -> (pmim, ed, r)
                    | None ->                        
                        let sv = get_init "diosim" l
                        let _ = if sv=None then vprintln 1 ("diosim: +++No init value for conditional comb assign to " + xToStr l)
                        let r1 = xi_query(g, r, (if sv=None then X_undef else xi_bnum(valOf sv)))
                        (pmim, ed, r1)
            cc.Add((prefix, nn), nv)


    let hcmap = List.fold hcgen Map.empty hHC

    let install_hc prefix (simrec:simrec_t) = 
        match hcmap.TryFind((simrec.prefix, x2nn simrec.it)) with
            | None -> ()
            | Some(pmim, sup, comb_driver) ->
                match comb_driver with
                    | W_string(s, _, _) ->
                        vprintln 3 ("Giving special protection to assignment of " + simname + " with string " + s + "(assuming do not want it eval'd later)")
                        simrec.current := DS s
                        simrec.comb := None
                        ()
                    | other when constantp other -> 
                        //if vd>=6 then vprintln 6 ("Comb source driver for " + xToStr it + " set up " + optToStr hc)
                        assertf(sup=[]) (fun()-> simname + sprintf " expect null support for constant assign of %s (perhaps more than one combinational assign) sup=[] failed: sup=%s" (xToStr other) (sfold xToStr sup) + " lhs=" + xToStr simrec.it + " rhs=" + xToStr comb_driver)
                        let _ = simrec.current := gec_DL(simrec.encoding_cvt, xi_manifest_int "dio2" other)
                        ()

                    | X_bnet ff -> 
                        //let _ = vprintln 10 ("Comb source driver for " + xToStr it + " set up " + optToStr hc)
                        let ss = ff.id
                        let gen_sens x =
                            let (found, simrec1) = dD1.simrecs.TryGetValue((prefix, x2nn x))
                            assertf found (fun () -> sprintf "Saved combinational sources for X_bnet '%s' not found. Should be there from first pass. " ss)
                            (De_any, simrec1, ref DX)
                        simrec.comb := Some(pmim, map gen_sens sup, comb_driver)

                    | other -> 
                        //if vd>=6 then vprintln 6 ("Comb source driver for " + xToStr it + " set up " + optToStr hc)
                        let gen_sens xx =
                            let (found, simrec1) = dD1.simrecs.TryGetValue((prefix, x2nn xx))
                            let _ =  assertf found (fun () -> sprintf "Saved combinational source for key=%s '%s' as support of %s not found. Should be there from first pass." (xkey xx) (xToStr xx) (xToStr other))
                            (De_any, simrec1, ref DX)
                        simrec.comb := Some(pmim, map gen_sens sup, comb_driver)
 
#if SPARE
    let total =
        let fg cc = function
            | DRIVE(lst, _)
            | SUPPORT lst ->  List.fold fg1 cc lst
            //| (_, cc) -> cc
        List.fold fg natives anal
#endif 

    let _ =  // We use two passes so that all combination driver records are created before wiring up.
        let wireup_pass2 (prefix, nn) simrec = //let i = (id_to_net (total @ !anets) id)
            //..let _ = gen_srec dD1 install_hc inits prefix 0 i
            install_hc prefix simrec
            ()
        for kk in dD1.simrecs do wireup_pass2 kk.Key kk.Value done

    let late_id_to_simrec prefix id = // We use two passes so that all combination driver records are created before wiring up.
        let simrec = gen_srec dD1 inits prefix id
        install_hc prefix simrec
        simrec

        
    let _ = WF 3 "diosim" ww "simrecs created (first batch)"

    let install_io_devices(netname, addr) = 
        let addr = down64 addr
        let net = vectornet_w(netname, 32) (* TODO: width set *)
        let iobase = g_iobase ""
        if addr >= iobase then
            let p = late_id_to_simrec [ "DS_IOSPACE"] net
            Array.set dD1.iospace (addr - iobase) (Some p)

    let pcnet pmim =  // for virtual machines only - real machines have an explicit pc?
        let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
        let a:ip_xact_vlnv_t = minfo.name 
        let net_name = funique("pc_" + hptos a.kind) // better to use iname surely?
        let prec = { signed=Unsigned; widtho=Some 32 } 
        let reset_val = 0
        let rr = xgen_gend None (net_name, prec, LOCAL, Some reset_val)
        vprintln 2 (sprintf "pcnet for vm %s is %s" (hptos prefix) (netToStr rr))
        let simrec = newsimrec dD1 (Some(xi_num reset_val)) prefix rr // Add in to simrecs database
        (rr, simrec)

        (* A number of VMs run concurrently in the simulator. There are several forms: main ones dic and asm, and also SP_l for reactive rules. *)

    let gen_vm arg =
        match arg with
        | (pmim, H2BLK(dir, SP_dic(dDIC, id))) -> generic_vm_constructor dD1 pmim dir (pcnet pmim) (SP_dic(dDIC, id))
        | (pmim, H2BLK(dir, SP_l(ast_ctrl, x))) -> generic_vm_constructor dD1 pmim dir (pcnet pmim) (SP_l(ast_ctrl, x))
        | (pmim, H2BLK(dir, SP_asm(dDIC, diclen, regs, symbols, code))) -> 
            let (prefix, minfo, iinfo, my_binder, mach_id) = pmim
            let mem = dxarray_ctor "memory" [65536L] []
            let (pc_net, pc_simrec) = pcnet pmim
            let a = { prefix=     prefix
                      regsbase=   snd regs
                      regs=       regs_array(regs)
                      reg_count=  fst regs
                      pc=         ref 0
                      sp=         ref(12*4096-2)
                      code=dDIC; minfo=minfo; symbols=symbols;
                      mem=        mem
                      returner=   ref []
                      pcnet=      pc_net
                      pc_simrec=  pc_simrec // newsimrec dD1 None prefix pc_net
                    }
            app install_io_devices symbols
            let rec im addr = function
                | (Idefs(n, _)) -> mem.[addr] := DN (up64 n)
                | (_) -> ()
            let rec initialise_memory pos =
                if pos=diclen then ()
                else 
                    (
                        im pos dDIC.[pos];
                        initialise_memory (pos+1)
                    )

            initialise_memory 0
            a.returner := [ lifted_compile1 a (Ij(i_rts, NAM, NAM, NAM, ref 0)) ]
            (pmim, S_RM a)

        |  (pmim, H2BLK(clko, other)) -> sf ("gen_vm: diosim: other form " + hprSPSummaryToStr other)


    let vsort lst  = // Sort so that the nonblocking VMs are stepped first, with big_bang first of all, and we refactor the directorate too.
        let rec fsort vm (bb, e, l) =
            match vm with
                | (pmim, dir, b)::tt when nullp dir.clocks ->  fsort tt (bb, (pmim, H2BLK(dir, b))::e, l)
                | (pmim, dir, b)::tt ->
                    (vprintln 3 (sfold edgeToStr dir.clocks + " was a vsort sensitivity");
                     if de_edge(hd dir.clocks) = g_construction_big_bang
                     then fsort tt ((pmim, H2BLK(g_null_directorate, b))::bb, e, l)
                     else fsort tt (bb, e, (pmim, H2BLK(dir, b))::l)
                    )

                | [] -> (rev bb) @ (rev e) @ (rev l)
        fsort lst ([], [], [])

    let hHS = vsort hHS
    let _ = WF 3 "diosim" ww "sort done"
    if vd>=3 then reportx 3 "Sorted VMs for diosim" (fun ((prefix, (minfo:minfo_t), iinfo_, my_binder, mach_id), x) -> "diosim HS: " + vlnvToStr minfo.name + " " + execSummaryToStr x + "\n") hHS
    let vVMs = map gen_vm hHS
    let _ = WF 3 "diosim" ww "VMs generated"

    (* ignored for now              let _ = app simenvdump 8 initials *)
    //let _ = map (gen_srec dD1 install_hc inits prefix 1) total
    let _ = WF 3 "diosim" ww "simrecs created (second batch)"

    let undriven = [] // Old pseudo-random auto stimuls mechanism - currently disabled
#if AUTOSTIM
    let driven =
        let dg cc = function
            | DRIVE(x, _)  -> List.fold fg1 cc x
            | _            -> cc
        List.fold dg natives anal
    let undriven = list_subtract (total, tnownet::driven)
    if vd>=5 then
        vprintln 5 (i2s(length undriven) + " undriven out of "  + i2s(length total))
        let netmapping_list = (total @ !anets)
        app (fun x -> vprintln 5 ("  net " + netToStr x + " declared (t)")) total
        app (fun x -> vprintln 5 ("  net " + netToStr x + " declared (a)")) (!anets)
        app (fun x -> vprintln 5 ("  net " + netToStr x + " driven")) driven
        app (fun x -> vprintln 5 ("  net " + netToStr x + " undriven")) undriven
#endif

    let netmapping = dD1.simrecs

    m_exitflag := false
    let _ = WF 3 "diosim" ww "Starting simulation"

    let initial_trocs = !dD1.m_l_trocs @ !dD1.m_e_trocs
    (dD1.m_e_trocs := []; dD1.m_l_trocs := [])
    
    let trocs = initial_trocs @  diosim_deltacycle ww (hHC, vVMs, hRTL, dD1, undriven, tnow) (1, [])
            // with sfault(s) -> (vprintln 0 ("Simulation diosim error: " + s); [])

    let _ = WF 3 "diosim" ww ("Simulation completed at endtime=" + i2s endtime)
    
    let tracepf cc (prefix, xx) =
        match xx with
        | X_net(s, _) -> (prefix, s)::cc
        | X_bnet ff   ->
            let f2 = lookup_net2 ff.n
            if traceme f2.vtype then (prefix, ff.id)::cc else cc
        | X_undef     -> cc
        | other       ->
            vprintln 3 ("+++diosim: tracepf other " + (xToStr other))
            cc
    let isBeingTraced (prefix_, netid) = // Trace all that textually match, regardless of where (ie regardless of prefix)
        (nullp dD1.trace_regexs) || conjunctionate (fun regex->System.Text.RegularExpressions.Regex.IsMatch(netid, regex)) dD1.trace_regexs
    vprintln 3 ("Trace regexps are " + sfold (fun x->x) dD1.trace_regexs)

    let tracenets =
        let filterize (pref, items) = (pref, List.filter (fun netid -> isBeingTraced netid) (List.fold tracepf [] !items))
        map filterize !m_anets

    if not_nonep dD1.vcd then write_diosim_vcd ww dD1 (valOf dD1.vcd, id, dD1.title, endtime, netmapping, trocs, tracenets)

    let flat_tracenets = list_flatten (map snd tracenets)
    let _ =
        if dD1.save_db <> None then
            let _ = diosim_save_db ww dD1 (valOf dD1.save_db, id, endtime, trocs, flat_tracenets)    
            ()

    let _ =
        if dD1.check_db <> None then
            let _ = diosim_check_against_db ww dD1 (valOf dD1.check_db, id, endtime, trocs, flat_tracenets)    
            ()
        
    let _ =
        if dD1.plot<> None then
            let _ = diosim_plot_display ww dD1 (valOf dD1.plot, id, dD1.title, endtime, trocs, ["-MNRZI 1"]) flat_tracenets
            ()
 
    let _ =
        let xmlfix = ".xml" // enabled by default ... TODO control in recipe
        if xmlfix <> "" then
            let ans = ref []
            let profile_report = function
                | None -> ()
                | Some (p:profiler_t) -> mutadd ans (p.xml_report())
            for v in dD1.simrecs do profile_report v.Value.profiling_database done

            match !ans with
                | [] -> ()
                | xml ->
                    let com1 = XML_COMMENT "diosim dynamic execution profile"
                    let title1 = XML_ELEMENT("title", [], [ XML_ATOM dD1.title ])
                    let xmld = XML_ELEMENT("diosimprofile", [], com1::title1::xml)
                    let filename = if abs_file_path_pred simname then simname + xmlfix else path_combine(!g_log_dir, simname + xmlfix) 
                    let ww' = WF 2 ("Writing xml simulation profile of " + simname + " to file " + filename) ww "serialise"
                    try
                        hpr_xmlout (WN ("xml profile render " + simname) ww) filename true xmld
                        let _ = WF 2 ("Writing xml simulation profile of " + simname + " to file " + filename) ww "complete"
                        ()
                    with _ -> vprintln 0 (sprintf "+++ could not write xml simulation profile to %s" filename)


    let diosim_exit_report vd =
        youtln vd (sprintf "Diosim simulation completed. Title='%s'" dD1.title)
        youtln vd (sprintf "      complete at time             %i" endtime)
        youtln vd (sprintf "      simulation cycle limit       %i" dD1.sim_cycle_limit)
        youtln vd (sprintf "      static_rtl_statement_count   %i" !dD1.stats.static_rtl_statement_count)
        youtln vd (sprintf "      vm_instructions_run          %i" !dD1.stats.vm_instructions_run)
        youtln vd (sprintf "      rtl_instructions_run         %i" !dD1.stats.rtl_instructions_run)
        youtln vd (sprintf "      rtl_cont_instructions_run    %i" !dD1.stats.rtl_cont_instructions_run)
        ()

    let vd = YOSL("Diosim Report", ref [])
    diosim_exit_report (vd)
    yout_close vd
    ()



(* 
 * Simulate using builtin diosim.
 *)
let diosim ww0 op_args vms =
    let msg = op_args.banner_msg
    let stagename = "diosim"
    let c2 = op_args.c3
    let ww = WF 2 "opath_diosim_vm" ww0 msg

    let seed = 3256L;
    let _ = g_prbs_v := seed

    let _ = if control_get_s "diosim" c2 "diosim-clear-arrays" (Some "disable") = "enable" then g_diosim_clear_arrays := true
    let _ = if control_get_s "diosim" c2 "diosim-clear-scalars" (Some "disable") = "enable" then g_diosim_clear_scalars := true    
    let techno     = control_get_s "diosim" c2 "diosim-techno" (Some "disable") = "enable"
    let save_db    = arg_assoc_or "diosim" ("diosim-save-db", c2, [])
    let check_db   = arg_assoc_or "diosim" ("diosim-check-db", c2, [])    
    let title      = arg_assoc_or "diosim" ("diosim-title", c2, [])
    let traces     = arg_assoc_or "diosim" ("diosim-traces", c2, [])
    let spool      = arg_assoc_or "diosim" ("diosim-spool", c2, [])    
    let sim_cycle_limit = control_get_i c2 "sim" 250
    let vd = control_get_i c2 "diosim-loglevel" -1 // previously called diosim-tl
    let kk x = if x=[] || x =[""] then None else Some(hd x)
    let k0 x defaultv = if x=[] || x =[""] then defaultv else hd x    
    let gully_simrec = ref None;
    let outcos = if nullp spool || hd spool = "-" then None else Some(yout_open_out (hd spool))
    let gec_diosim_stat_record () =
        {
            static_rtl_statement_count=   ref 0L
            vm_instructions_run=          ref 0L
            rtl_instructions_run=         ref 0L
            rtl_cont_instructions_run=    ref 0L            
        }
    let simrec_dict = new simrecs_t()
    let (m_global_nets, gpmim) = gec_diosim_global_pmim simrec_dict
    let dD1 =
        {
            mvd=             ref vd
            m_exit_report =  ref "Diosim done."
            selections=      new selections_t()
            gully_simrec=    gully_simrec  
            iospace=         Array.create 65536 None            
            save_db=kk save_db
            check_db=kk check_db            
            timescale=               100
            standing_troc=           new standing_troc_t()
            techno=techno
            m_pendings=              ref [] // m_pendings - updates ready to be committed at end of delta cycle.
            m_e_trocs=               ref [] // Tracing events early ones
            m_l_trocs=               ref [] // Tracing events later ones 
            simrecs=                 simrec_dict
            plot=kk (arg_assoc_or "diosim" ("diosim-plot", c2, []))
            vcd=kk (arg_assoc_or "diosim" ("diosim-vcd", c2, []))
            title=                   k0 title "$no-title$"
            trace_regexs=traces
            m_global_nets=           m_global_nets
            outcos=                  outcos
            heap_ptrs=               new heap_ptrs_t()
            granularity=             control_get_i c2 "diosim-granularity" 20
            sim_cycle_limit=         sim_cycle_limit
            stats=                   gec_diosim_stat_record()
            gpmim=                   gpmim
            m_cll=                   ref  ['a'; 'a'; 'a'; ]
        }
    let _ = gully_simrec := Some (newsimrec dD1 None g_diosim_global_prefix (X_bnet(register16("$gully", []))))

    
    //We do not really want to see this anal output, since the code gets munged by sp_catenate.
    // The report after that is included, which is more useful.
    
    let _ = try diosimulate (WN "diosim final" ww) ("simulate", vms, dD1)
            with je when true ->
                let m = ("*** exception in simulate " + je.ToString() + "\n") 
                vprintln 3 m
                if outcos <> None then yout (valOf outcos) m
                ()
    let _ = xsim_print dD1 (!dD1.m_exit_report + "\n")
    let _ = if not_nonep outcos then yout_close(valOf outcos)
    let selections =
        let selise k v = Napnode(k,  map (fun (a,b)-> Nap(i2s a, i2s b)) (rev v))
        Napnode("selections", [ for KeyValue(k, v) in dD1.selections -> selise k !v ])

    //  We (can) rewrite the machine with the simulation endvals .... not sure who wants this. Profile-directed operators?
    let endvals =
        let selrec (v:simrec_t) = Nap(net_id v.it,  dnToStr !v.current)
        Napnode("endvals", [ for KeyValue(k, v) in dD1.simrecs -> selrec v ])    

    let recode design =  // All the information is added to all the vms - please collate/refine this !
        match design with
            | (ii, None) -> (ii, None)
            | (ii, Some(HPR_VM2(minfo, decls, sons, execs, asserts))) ->
                let ii =
                    { ii with
                        generated_by= stagename
                      //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps
                    }
                let minfo' =  { minfo with atts=selections :: endvals :: minfo.atts; }
                (ii,  Some(HPR_VM2(minfo', decls, sons, execs, asserts)))
    map recode vms

(* 
 * Simulate using builtin diosim.
 *)
let opath_diosim_vm ww0 op_args vms =
    let msg = op_args.banner_msg
    let stagename = "diosim"
    let c2 = op_args.c3
    let ww = WF 2 "opath_diosim_vm" ww0 msg
    // You can also disable via opath using -diosim=disable or something
    let enable = (control_get_s "diosim" c2 "diosim" None) = "enable"
    let vms =
        if nullp vms then
            vprintln 2 (sprintf "Diosim: no input machine(s)")
            []
        elif enable then diosim ww op_args vms
        else
            vprintln 2 (sprintf "Diosim is disabled")
            vms
    vms


let install_diosim () =
    let sim_argpattern =
        [
            Arg_defaulting("diosim-save-db", "", "Write a database file recording traced net events");
            Arg_defaulting("diosim-check-db", "", "Read a database file to compare with the current simulation against");    
            Arg_defaulting("diosim-vcd", "diosim.vcd", "VCD (verilog change dump) file name (use gtkwave or modelsim's vcd2wlf command to view)");
            Arg_defaulting("diosim-plot", "", "Plot file name (use 'diogif -x plot.plt' to view)");
            Arg_defaulting("diosim-title", "diosim plot", "Plot and VCD file title");
            Arg_defaulting("diosim-spool", "diosim.out", "Spool file for console output generated during simulation");

            Arg_enum_defaulting("diosim-clear-scalars", ["enable"; "disable"], "enable", "Clear scalars to zero rather than uncertain X value");
            Arg_enum_defaulting("diosim-clear-arrays", ["enable"; "disable"], "enable", "Clear arrays to zero rather than uncertain X value");
            Arg_enum_defaulting("diosim", ["enable"; "disable"], "enable", "Disable simulator run");
            Arg_enum_defaulting("diosim-techno", ["enable"; "disable"], "disable", "Enable technicolor output (ANSI color escapes)");
            
            Arg_string_list("diosim-traces", "Regular expression for net names to trace");
            Arg_int_defaulting("sim", 10000, "Number of cycles/ticks to simulate for");
            Arg_int_defaulting("diosim-granularity", 100, "Number of DIC instructions (timeslice size) before pre-emption");
            Arg_int_defaulting("diosim-loglevel", -1, "Trace level -1 to 10: higher is more tracing: 1=new default (5=default), 6=see all assigns");
            Arg_int_defaulting("diosim-plot-width", 1000, "Plot pixel width");
            Arg_int_defaulting("diosim-plot-height", 1000, "Plot pixel width");
            //Arg_int_defaulting("starttime", 0, "Start time for plot (ticks)");              
        ]
    install_operator ("diosim", "Diosim simulator", opath_diosim_vm, [], [], sim_argpattern)
    ()
  


// eof
