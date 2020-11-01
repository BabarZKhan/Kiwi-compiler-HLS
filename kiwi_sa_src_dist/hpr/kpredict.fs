(* 
 * CBG Orangepath HPR/LS.
 * HPR Logic Synthesis and  Formal Codesign System.
 * (C) 2003-15 DJ Greaves. All rights reserved.  See license file with this distro for use and disclaimers.
 *
 *
 * Contains:
 *  Performance predictor prototype.
 *
 *
 *
 *)

module kpredict

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open opath_hdr
open fitters
open hprxml
open moscow
open yout

let kpredict_banner_msg = "KiwiPerformancePredictor-Alpha"

type vdict_t = OptionStore<int, int>
type vdict_tallied_t = ListStore<int, int>


type profile_record_t =
    {
        cost: double;
        index:int;
        visits_kprg: int list;
        visits: vdict_t * vdict_tallied_t;
    }
      
let g_null_profile_rec = { index= -1; visits_kprg=[]; visits=(new vdict_t("nullstarter"), new vdict_tallied_t("nullstarter")); cost=0.0; }

let tally (rawData:vdict_t) =
    let ans = new vdict_tallied_t("tallied")
    let summer p q = ans.add q p
    let _ = for z in rawData do summer z.Key z.Value done
    ans

type pd_t =
    | PD_profile of profile_record_t
    | PD_nill
    
let read_profile ww xml =

    let ff2 a = muddy ("ssosl " + xmlToStr "" a)

    let ff_visits (cdic:vdict_t) = function
        | XML_ELEMENT("visit", ats, XML_ATOM site :: XML_ATOM count :: _) ->
            let count = atoi32 count
            let site = atoi32 site
            match cdic.lookup site with
                | None ->
                    cdic.add site count
                | Some ov -> sf ("counts recorded more than once for " + sprintf " count is %i %i" site count)

        | XML_ATOM sp -> ()
        | other -> sf ("Unrecognised ff_visits xml structure: " + xmlToStr "" other)

    let ff_dram = function
        | XML_ELEMENT("name", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ELEMENT("activates", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ELEMENT("reads4", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ELEMENT("writes4", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ELEMENT("reads8", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ELEMENT("writes8", ats, XML_ATOM name_ :: _) -> 2        
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_dram xml structure: " + xmlToStr "" other)

    
    let ff_prof arg cc =
        match arg with
        | XML_ELEMENT("creator", ats, _) -> cc
        | XML_ELEMENT("clocksUsed", ats, XML_ATOM clocks:: _) ->
            let cost = atoi32 clocks
            { cc with cost=double cost; }

        | XML_ELEMENT("dramStats", ats, lst) ->
            let _ = map ff_dram lst
            cc // for now
        | XML_ELEMENT("kprg", ats, lst) -> { cc with visits_kprg=map ff2 lst; }
        | XML_ELEMENT("index", ats, XML_ATOM idx :: _) -> cc
        | XML_ELEMENT("pcVisits", ats, lst) ->
            let cdic = new vdict_t("cdic")
            let _ = app (ff_visits cdic) lst
            let tallied = tally cdic
            { cc with visits=(cdic, tallied); }

        | XML_ATOM sp_ -> cc
        | other ->
            let _ = vprintln 1 ("Ignored unrecognised ff_profile xml structure: " + xmlToStr "" other)
            cc
            
    let ff_res = function
        | XML_ELEMENT("label", ats, [XML_ATOM lab_ ]) -> 1
        | XML_ELEMENT("name", ats, [XML_ATOM name_ ]) -> 2        
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_res xml structure: " + xmlToStr "" other)

    let ff_ru = function
        | XML_ELEMENT("label", ats, [XML_ATOM lab_ ]) -> 1
        | XML_ELEMENT("name", ats, [XML_ATOM name_ ]) -> 2
        | XML_ELEMENT("cmd", ats, [XML_ATOM name_ ]) -> 3
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_ru xml structure: " + xmlToStr "" other)

    let ff_node = function
        | XML_ELEMENT("label", ats, [XML_ATOM lab_ ]) -> 1
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_node xml structure: " + xmlToStr "" other)

    let ff_poss = function
        | XML_ELEMENT("label", ats, XML_ATOM lab_ :: _) -> 1
        | XML_ELEMENT("name", ats, XML_ATOM lab_ :: _) -> 1        
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_poss xml structure: " + xmlToStr "" other)

    let ff_xition = function
        | XML_ELEMENT("src", ats, XML_ATOM src :: _) -> 1
        | XML_ELEMENT("dest", ats, XML_ATOM dest :: _) -> 1        
        | XML_ELEMENT("guard", ats, XML_ATOM lab_ :: _) -> 1        
        | XML_ATOM sp_ -> -1
        | other -> sf ("Unrecognised ff_xition xml structure: " + xmlToStr "" other)

    let ff_kprg = function
        | XML_ELEMENT("resource", ats, lst) -> map ff_res lst
        | XML_ELEMENT("ru", ats, lst) -> map ff_ru lst
        | XML_ELEMENT("ru1", ats, lst) -> map ff_ru lst
        | XML_ELEMENT("node", ats, lst) -> map ff_ru lst
        | XML_ELEMENT("xition", ats, lst) -> map ff_xition lst        
        | XML_ELEMENT("startnode", ats, lst) -> []
        | XML_ELEMENT("poss", ats, lst) -> map ff_poss lst                                
        | other -> sf ("Unrecognised ff_kprg xml structure: " + xmlToStr "" other)


    let int_el_assoc tag whence items =
        let rec scan = function
            | [] -> None
            | XML_ELEMENT(t, _, XML_ATOM idx :: _)::_ when t=tag -> Some (atoi32 idx)
            | _::tt -> scan tt
        match scan items with
            | Some idx -> idx
            | None -> cleanexit(sprintf "Missing tag %s. Not found in " tag + xmlToStr "" whence)
            
    let ff0 arg =
        match arg with
        | XML_ELEMENT("kprg", ats, lst) ->
            let _ = map ff_kprg lst
            PD_nill
        | XML_ELEMENT("kiwiProfile", ats, lst) ->
            let idx = int_el_assoc "index" arg lst
            PD_profile(List.foldBack ff_prof lst { g_null_profile_rec with index=idx; })
        | other -> sf ("Unrecognised profile top-level xml structure: " + xmlToStr "" other)


    ff0 xml
    
let opath_kiwi_pp ww op_args vm =
    let rdin filename cc = 
        let _ = vprintln 0 ("read file " + filename)
        let xml = hpr_xmlin ww filename
        //let _ = vprintln 0 (sprintf "Kpredict log %A" xml)
        match read_profile ww xml with
            | PD_nill -> cc
            | arg -> arg :: cc
          
        
    let names = rev(control_get_strings op_args.c3 "srcfile") // Reverse since returned in wrong order 
    let data = List.foldBack rdin names []

    let visitcountEquivs = new EquivClass<int>("vce")

    let anal pd = 
        match pd with
            | PD_profile prec ->
                let _ = vprintln 0 (sprintf "classes %A" prec.visits)
                match prec.visits with
                    | (raw, tallied) ->
                        let _ = for x in tallied do ignore(visitcountEquivs.AddFreshClass (fun _ _ _ -> ()) x.Value) done
                        let _ = vprintln 0 (sprintf "ff %i %A" prec.index tallied)
                        (prec.index, prec.cost, raw)
                    
            | _ -> sf "profile other form"
    let anals = map anal data

    let statemodels =
        let statemodels = ref []
        let fit (nk_, states) =
            let _ = vprintln 0 ("fit for states " + sfold i2s states)
            let representative = hd states
            let shape = map (fun (idx, cost_, (raw:vdict_t)) -> (idx, valOf_or_fail "L349" (raw.lookup representative))) anals
            let _ = vprintln 0 ("data = " + sfoldcr (fun (a,b) -> i2s a + "," + i2s b) shape)
            let best =
                match run_regression shape with
                    | Some best -> best
                    | None -> muddy "regression failed"
            let groupSize = length states
            mutadd statemodels (representative, double groupSize, best)

        for z in visitcountEquivs do fit z.Value done
        !statemodels



    let targets = [ 3000; 6000; 10000; 50000; 100000; 200000]
    let _ = vprintln 0 ("Based on profiles at " + sfold (f1o3>>i2s)  anals)

    let xml_dati =
        let f2 (idx, cost, bux) =
            XML_ELEMENT("postulate", [],
                        [ XML_ELEMENT("index", [], [XML_ATOM(i2s idx)]);
                          XML_ELEMENT("cost", [], [XML_ATOM(sprintf "%f" cost)]);
                          XML_WS "\n"
                        ])
        map f2 anals
    let performance_extrapolate target =
        let target = double target
        let computeCost (m0:double, m1) (rep, size, (style, mse, mf)) =
            let pred = size * mf target
            (pred + m0, size * mse * pred + m1)
        let (cost, variance) = List.fold computeCost (0.0, 0.0) statemodels
        let spread = sqrt variance
        let _ = vprintln 0 (sprintf "Extraploate to %f, estimated cost is %f +/- %f" target cost spread)
        XML_ELEMENT("prediction", [],
                    [ XML_ELEMENT("index", [], [ XML_ATOM(sprintf "%f" target) ])
                      XML_ELEMENT("estimatedCost", [], [ XML_ATOM(sprintf "%f" cost) ])
                      XML_ELEMENT("estimatedError", [], [ XML_ATOM(sprintf "%f" spread) ])                      
                      XML_WS "\n"
                    ])
    let predict_xml = map performance_extrapolate targets

    let toViewer = XML_ELEMENT("KiwiPredictionReport", [], xml_dati @ predict_xml)

    let _ = hpr_xmlout ww "toViewer.xml" false toViewer
    let _ = vprintln 0 ("Kpredict done ")

    vm

[<EntryPoint>]
let main (argv : string array)  =
    let ww = WW_end
    let ww = WF 1 "kpredict" ww "start"
    let _ = vprintln 0 ("Kpredict ")

    let argpattern =
            [
                Arg_required("srcfile", -1, "xml profile file or files", "");
            ]

    let _ = opath_hdr.install_operator ("kpredict",   "Kiwi Performance Predictor", opath_kiwi_pp, [], [], argpattern);
    let rc = opath.opath_main ("kpredict", argv)
    rc


(*

Main principle:
 we assume normally one independent variable that corresponds to the main order parameter of the problem to be solve, such as the size of input design dataset.
 We can extend this to multiple independent variables in future work.
 
 All blocks are assumed to have a polynmonial relationship to the independent variable. With three profile runs of different sizes we can fit a quadratic or n log n
 curve to the available points.  Generally, higher-order algorithms are not used but a fourth run not fitting serves as a warning.

 We need to split off visit counts arising from stalls from those arising from work doing self transitions.
 
 We first try least squares fitting of the visit counts to the independent variable(s).  Blocks are placed in equivalence classes based on visit counts.

  We first validate the control-flow of each run against the compiler-generated model - any disparity is a toolchain bug. Then we compare the control flow graph of
 different runs against each other and place runs in classes that have the same control flow graph (although visit counts will be different). Generally we
 expect short runs to perhaps not ilustrate all relevant control flows and we should only extrapolate on the control flow graph of the class of
that contains the longest run. We expect this to include a consecutive set of run sizes. 

      
 Then for each class we fit it to the independent variable.

statically determined as control-flow equivalent have different counts
   *)


// eof
