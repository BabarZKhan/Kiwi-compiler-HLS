// 
// Kiwi Scientific Acceleration: KiwiC compiler.
//
// ksc.fs: Storage class manager
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

module ksc

open System.Collections.Generic

open moscow
open yout
open meox
open hprls_hdr
open abstract_hdr


// A mutable dictionary that uses None/Some paradigm and supports mutable keys.
type SimpleMutKeyOptionStore<'k_t, 'v_t when 'k_t : equality>(id:string) = class

    let m_data:(KeyValuePair<'k_t, 'v_t>) list ref = ref []

    let wrap k v = new KeyValuePair<'k_t, 'v_t>(k, v) // TODO do not wrap on every read!
    let gen_enum() = 
        let started = ref false
        let state = ref []
        { new IEnumerator<_> with 
              member self.Current = if !started && not_nullp !state then (hd !state) else sf (sprintf "ksc: No Current L1/2 %A %A" started !state)
          interface System.Collections.IEnumerator with
              member self.Current = box(if !started && not_nullp !state then (hd !state) else sf "ksc: No Current L12/2")
              member self.MoveNext() =
                 if not !started then
                     started := true
                     state := !m_data
                     not_nullp !state
                 elif nullp !state then false
                 else (state := tl !state; not_nullp !state)
              member self.Reset() = started := false
          interface System.IDisposable with 
              member self.Dispose() = ()
        }

    
    interface IEnumerable<KeyValuePair<'k_t, 'v_t>>
       with
           member x.GetEnumerator() = gen_enum() :> IEnumerator<KeyValuePair<'k_t, 'v_t>>
           member x.GetEnumerator() = gen_enum() :> System.Collections.IEnumerator

    member this.NumElements = length !m_data
    member this.GetElement = muddy "png-png"
    member this.ToEnumerable() = Seq.init this.NumElements this.GetElement

    member x.remove kk =
        let rec scand = function
            | [] -> []
            | (aa:KeyValuePair<'k_t, 'v_t>)::tt when aa.Key=kk -> tt
            | aa::tt -> aa :: scand tt
        m_data := scand !m_data

    member x.add kk vv =
        let rec scanw = function
            | [] -> [wrap kk vv]
            | (aa:KeyValuePair<'k_t, 'v_t>)::tt when aa.Key=kk -> (wrap kk vv)::tt
            | aa::tt -> aa :: scanw tt
        m_data := scanw !m_data

    member x.lookup kk =
        let rec scan = function
            | [] -> None
            | (aa:KeyValuePair<'k_t, 'v_t>)::tt when aa.Key=kk -> Some aa.Value
            | _::tt -> scan tt
        scan !m_data

    // Where two keys have become the same, we are always told about this we return the value that is left and
    // any other values    
    member x.mutkey okey nkey =
        let mv = ref None
        let mo = ref []
        let rec scanu = function
            | [] -> []
            | (aa:KeyValuePair<'k_t, 'v_t>)::tt when aa.Key=okey ->
                match !mv with
                    | None ->
                        mv := Some aa.Value
                        (wrap nkey aa.Value) :: scanu tt
                    | Some ev ->
                        mutadd mo aa.Value
                        scanu tt 
            | aa::tt -> aa::scanu tt
        m_data := scanu !m_data
        (!mv, !mo)
        
end



type ksc_point_t = int ref
            
type new_aidx_t =
    | A2_base of int
    | A2_leaf of string
    | A2_tag of string * string    
    | A2_subsc of int option
    

type ksc_point_map_t = SimpleMutKeyOptionStore<new_aidx_t, ksc_point_t>

type ksc_clskey_t = int

type new_nemtok_t = string // This string is the hptos of the old nemtok_t since we do not need the stringl structure anymore


// An aid_t is a souped-up nemtok_t used in collating the points-at info and dataflow naming information.
// This has algebraic properties for reference and dereference.
type aid_t =
    | A_loaf of new_nemtok_t              // Basis nemtok
    | A_tagged of aid_t * string * string // fields ptag and utag (ptag may be a string list for enumerated subindecies?)
    | A_subsc of aid_t * aid_t option
    | A_metainfo of int
    
let g_null_aid = A_loaf "KSC_NULL_PTR"
let g_anon_aid = A_loaf ""


let rec aidToStr = function
    | A_metainfo mci             -> sprintf "MCI%i" mci
    | A_loaf ss                  -> if ss = "" then "ANON-AID" else ss
    | A_tagged(v, ptag, utag)    -> sprintf "%s->%s" (aidToStr v) utag
    | A_subsc(b_aid, None)       -> sprintf "%s[-]" (aidToStr b_aid)
    | A_subsc(b_aid, Some o_aid) -> sprintf "%s[%s]" (aidToStr b_aid) (aidToStr o_aid)    

let rec san_tags = function
    | A_subsc(a, Some _) -> A_subsc(a, None)
    //| A_tagged(aid, ptag, utag) when ptag=g_indtag -> A_tagged(san_tags aid, ptag, ptag)
    | other -> other





let rec aid_depth_measure cc = function
    | A_loaf _ -> cc
    | A_subsc(x, None)        -> aid_depth_measure (cc+1) x
    | A_subsc(x, Some y     ) -> cc + 10 + max (aid_depth_measure 0 x) (aid_depth_measure 0 y)
    | A_tagged(x, ptag, utag) -> aid_depth_measure (cc+1) x

//
// Beyond a certain aid complexity we give up trying to disambiguate and just regard
// aids with matching suffixes as identical.
let san_depth identity_threshold arg =

    let rec depth_trim budget arg =
        if budget = 0 then arg
        else
            match arg with
                | A_loaf _ -> sf "depth_trim struck the bottom!"
                | A_subsc(x, None)   -> A_subsc(depth_trim (budget-1) x, None)

                | A_subsc(x, Some y) -> A_subsc(depth_trim (budget-1) x, Some(depth_trim (budget-1) y))


                | A_tagged(x, ptag, utag) -> depth_trim (budget-1) x
    let depth = aid_depth_measure 0 arg
    if true || depth <= identity_threshold then arg
    else
        //dev_println (sprintf "    ksc long_one - trimmed suffix %i to %i for %s"  depth identity_threshold (aidToStr arg))
        depth_trim (depth-identity_threshold) arg


// Resolving a pair of attribute lists is a generic operation.  Conjunction, Disjunction and MustAgree/Precedence are potential policy axes to flexibily support in the future.  Here we use disjunction with alphabetic precedence.

let g_undecided_bankinfo = "!UndecideD" // We start this with a pling to make it sort before any properly named bank.
        
let auxinfo_resolve_bank oldv newv =
    match (oldv, newv) with
        | (u, _) when u=g_undecided_bankinfo -> newv
        | (_, u) when u=g_undecided_bankinfo -> oldv
        | (ox, nx) ->
            let ans = ox
            hpr_yikes(sprintf "A dataset cannot be in bank %s and %s; just using %s. To interleave banks for a single set, do this manually (in RTL or SystemIntegrator etc)  beneath the load/store port." ox nx ans)
            ans

let auxinfo_resolve oldv newv =
    let rec lexographic_disjunction dones = function
        | [] -> []
        | (kk, vv)::tt when memberp kk dones -> lexographic_disjunction dones tt
        | (kk, vv)::tt ->
            let rec precedence_scan v0 = function
                | [] -> v0
                | (kk', vv')::tt when kk <> kk' -> precedence_scan v0 tt
                | (kk', vv')::tt ->
                    precedence_scan (max v0 vv') tt
            let nv = precedence_scan vv tt 
            (kk, nv) :: lexographic_disjunction (kk::dones) tt
    lexographic_disjunction [] (oldv @ newv)


let a2ToStr = function
    | A2_leaf ss         -> ss
    | A2_base sci        -> sprintf "sc%i" sci    
    | A2_tag(ptag, utag) -> "->" + utag
    | A2_subsc(None)     -> "[-]"
    | A2_subsc(Some idx) -> sprintf "[sc%i]" idx


// We identify as equal any pair of linearised aids containg a common repeated substring.
// We do this by removing all repeated substrings.
// This is to handle recursive store classes, such as occur with linked lists and trees.    
// Examples   A.B.B.C becomes A.B.C and A.B.C.D.E.C.D.E.C.D.E.F becomes A.B.C.D.E.F.
// It is possible that there is more than one repeated string, so iterate until fixed point is needed.  If there exist problems with more than one solution all we need is deterministic behaviour.
let strim_chains arg =

    let prefix_repeat_find arg =
        let len = length arg
        if len < 2 then None
        else
            let rec trier pos =
                if pos > len/2 then None
                elif arg.[0..pos-1] = arg.[pos..2*pos-1] then Some pos
                else trier (pos+1)
            trier 1
        
    let rec strim1 = function
        | [] -> []
        | lst ->
            match prefix_repeat_find lst with
                | None -> (hd lst)::strim1(tl lst)
                | Some v -> strim1 (lst.[v..])
    strim1 arg
                

let is_named = function
    | A2_subsc(Some _) -> true
    | _                -> false
let is_wild = function
    | A2_subsc(None)   -> true
    | _                -> false


let has_wild (mm:ksc_point_map_t) =
    let m_p = ref false
    for zz in mm do (if is_wild zz.Key then m_p := true) done
    (!m_p)

// Memory points-at equivalance classes    
// Copied here from repack.fs - please abstract and use once.
type ksc_classes_db_t(ident:string, class_char: char) = class
    let vd = -1 // This vd is normally -1 for debug logging off

    // Integer sci classes are turned into strings by prepending the class_char.
    let scToStr sci = sprintf "%c%i" class_char sci


    // Null_pointers are represented by the shared value Memdesc0 g_hasnull_memdesc0
    let contains_null = new OptionStore<ksc_clskey_t, bool>(ident)           // Which classes range over the null pointer.
    let auxiliary_designations = new ListStore<ksc_clskey_t, string * string>("bankinfo") // Various attributes in key/value pair form, such as which classes have an offchip designation.
    
    // bankinfo is set to g_undecided_bankinfo if a mapping is requested but has not taken place (late binding for load balancing).

    let m_locked = ref None
    let m_all_pms = ref []
    let m_retired_sci_list = ref []
    let m_all_points:ksc_point_t list ref = ref []
    let m_inverted_index = ref None
    let ksc_array_size = 10123
    let m_next_ksc_point = ref 1

    let clear_inverted_index() = m_inverted_index := None
    let ksc_table = Array.create ksc_array_size (new ksc_point_map_t("dummyfiller"))

    let identity_threshold = 40000 // not in use.


    let san x =
        san_depth identity_threshold (san_tags x)

    let set_null_flag cls =
        if vd>=4 then vprintln 4 (sprintf "Noting/logging the has_null attribute for c%i" cls)
        contains_null.add cls true // We only record 'Some true': None implies false.



    let check_null_flag sci =
        match contains_null.lookup sci with
            | Some true -> true
            | _         -> false

        
    let conglom_divert oc nc =
        let conmap ksc_point = if !ksc_point = oc then ksc_point := nc
        app conmap !m_all_points

    let conglom_auxwork ocls ncls = // This consequential conglomoration of points styles is not present in repack copy - TODO rationalise - it should be there, but we want to export this and have just one master copy in abstracte or somewhere.
    //Sonseen and contains null properties need to move over from ocls to ncls
        if vd >= 5 then vprintln 5 (sprintf "ksc conglom auxwork oc=c%i nc=c%i" ocls ncls)
        let old_ranges_over_null = check_null_flag ocls
            //let _ = if vd >= 5 then vprintln 5 (sprintf "Move ranges_over null info:check_null=%A.  %i movers.  +++ being=%s" old_ranges_over_null (length movers) (sfold snd movers))
        if old_ranges_over_null then set_null_flag ncls

        match (auxiliary_designations.lookup ocls, auxiliary_designations.lookup ncls) with
            | (oldv, newv) ->
                auxiliary_designations.clear ncls
                auxiliary_designations.adda ncls (auxinfo_resolve oldv newv)
        ()



    let newcell () =
        let pp = ref !m_next_ksc_point
        //dev_println (sprintf "Allocate node %i/%i" !pp ksc_array_size)
        mutinc m_next_ksc_point 1
        mutadd m_all_points pp
        if not_nonep !m_locked then hpr_yikes(sprintf "Operate on ksc when locked by %s" (valOf !m_locked))
        pp

    let new_pointmap() =
        let pm = new ksc_point_map_t("link")
        mutadd m_all_pms pm
        pm
        
    let newcell2 vv = 
        let pp = ref vv
        mutadd m_all_points pp
        pp


    let pairup2_top m_further_work sc_oc sc_nc =
        let rec pair_up2a depth sc_oc sc_nc =
            //dev_println (sprintf "ksc: conglom: pair_up2a %i with %i" sc_oc sc_nc)
            if sc_oc<>sc_nc then
                            let (boc, bnc) = (ksc_table.[sc_oc], ksc_table.[sc_nc])
                            let pup2 kk vv =
                                match bnc.lookup kk with
                                    | None -> bnc.add kk vv
                                    | Some nv when !nv = !vv -> ()
                                    | Some nv when !nv <> !vv ->
                                        //dev_println (sprintf "ksc: conglom: pair_up2 ksc %s (depth=%i) kk=%A c%i c%i:  %A cf %A"  ("") depth kk sc_oc sc_nc nv vv)
                                        let _ = pair_up2a (depth+1) !vv !nv
                                        ()
                            for xx in boc do pup2 xx.Key xx.Value done
                            conglom_divert sc_oc sc_nc
                            let _ = 
                                let (ov, nv) = (A2_subsc(Some sc_oc), A2_subsc(Some sc_nc)) 
                                let dij = function
                                    | ksc_point -> !ksc_point
                                let idx_divert (pm:ksc_point_map_t) =
                                    let (knocks, evicted_vales) = pm.mutkey ov nv
                                    match knocks with
                                        | None -> ()
                                        | Some xa ->
                                            let x = dij xa
                                            vprintln 3 (sprintf "%i items of further work generated" (length evicted_vales))
                                            app (fun y -> mutaddonce m_further_work (x, dij y)) evicted_vales
                                app idx_divert !m_all_pms
                                
                            conglom_auxwork sc_oc sc_nc
                            mutadd m_retired_sci_list sc_oc
            sc_nc
        pair_up2a 0 sc_oc sc_nc
                            
    let iterate_further_work m_further_work =
        dev_println("ksc: start further work")
        let rec knockons depth =
            match !m_further_work with
                | [] -> ()
                | (scx, scy)::tt ->
                    m_further_work := tt
                    if vd >= 3 then vprintln 3 (sprintf "ksc: congloms: knockon iteration %i start: scx=%i scy=%i" scx scy depth)                                    
                    let _ = pairup2_top m_further_work scx scy
                    vprintln 3 (sprintf "ksc: congloms: knockon iteration %i done" depth)
                    knockons (depth+1)
        knockons 0

    let rec ins_serf m_further_work mutf pt arg = // The main insert and lookup function.
        let mm = ksc_table.[pt]
        match arg with
            | (A2_base sci)::tt ->
                if vd >=4 then vprintln 4 (sprintf "ksc: Jump to base sc%i" sci)
                ins_serf m_further_work mutf sci tt
            | kk::tt ->
                // If inserting a wild card and there are named ones present already, these need converting to wild.
                let wild_partner_scan dd = // Find complementary partner to a subscription since must unify on wild cards.
                    if is_wild kk then
                        let pigout key vale =
                            mutadd m_further_work (!vale, dd)
                            mm.remove key
                        for zz in mm do (if is_named zz.Key then pigout zz.Key zz.Value) done
                            

                            
                match mm.lookup kk with
                    | None when mutf ->
                        // If inserting a named A2_subsc and a wild is present, then just follow the wild path and ignore the name.    
                        if is_named kk && has_wild mm then
                            let dx = valOf_or_fail "L388" (mm.lookup(A2_subsc None))
                            if vd>=4 then vprintln 4 (sprintf "ksc: Follow existing wild link for: mut=%A  (%s)    %i->%i" mutf (a2ToStr kk) pt !dx)
                            ins_serf m_further_work mutf !dx tt
                        else
                            let mx = new_pointmap()
                            let newtok = newcell()
                            mm.add kk newtok
                            if vd>=4 then vprintln 4 (sprintf "ksc: Establish link: mut=%A  (%s)    %i->%i" mutf (a2ToStr kk) pt !newtok)
                            Array.set ksc_table !newtok mx
                            if is_wild kk then wild_partner_scan !newtok
                            ins_serf m_further_work mutf !newtok tt
                    | None ->
                        if is_named kk && has_wild mm then
                            let dx = valOf_or_fail "L388" (mm.lookup(A2_subsc None))
                            if vd>=4 then vprintln 4 (sprintf "ksc: Follow existing wild link for: mut=%A  (%s)    %i->%i" mutf (a2ToStr kk) pt !dx)
                            ins_serf m_further_work mutf !dx tt
                        else None
                    | Some ov ->
                        if vd>=4 then vprintln 4 (sprintf "ksc: Follow link: mut=%A  (%s)    %i->%i" mutf (a2ToStr kk) pt !ov)
                        ins_serf m_further_work mutf !ov tt
            | [] -> Some pt

    and linearise aid = 
        let rec linserf = function
            | A_loaf ss ->
                if strlen ss >= 2 && ss.[0] = class_char && isDigit ss.[1] && atoi32(ss.[1..]) > 0
                then
                    //vprintln 3 (sprintf "ksc: parsed A2_base during linearise of %s" (aidToStr aid))
                    [ A2_base (atoi32(ss.[1..])) ]
                else [A2_leaf(ss)]
            | A_subsc(parent, None)       ->  A2_subsc(None) :: linserf parent 
            | A_subsc(parent, Some o_aid) ->
                let cell_o =
                    match insert_serf (linearise(san o_aid)) with
                        | Some sc -> Some(sc)
                        | None -> None
                A2_subsc(cell_o) :: linserf parent           
            | A_tagged(parent, s1, s2) ->  A2_tag(s1,s2) :: linserf parent
        let aid = san aid
        let ans = rev(linserf aid)
        let ans = strim_chains ans
        ans

    and insert_serf arg =
        clear_inverted_index()
        let m_further_work = ref []
        let sci = ins_serf m_further_work true 0 arg

        if nullp !m_further_work then sci
        else
            iterate_further_work m_further_work
            ins_serf m_further_work true 0 arg  // Should be no new work on second attempt.

    let insert arg = 
        let sci_o = insert_serf arg
        valOf_or_fail "sci_o" sci_o

    let lookup arg = ins_serf (ref []) false 0 arg

    let cToStr = aidToStr

    let m_long_ones_monitor = ref 4 // There may be a bug with infinite tag strings, so we report long ones here and trim them with the ksc budget.

    let monitor_long_one depth b =
        if depth > !m_long_ones_monitor then
            m_long_ones_monitor := depth
            dev_println (sprintf "    ksc long_one length=%i, %s" depth (aidToStr b))
        
    let same_class a b =
        let c1 = insert (linearise a)
        let c2 = insert (linearise b)
        (c1=c2)



    let classOf_ii aid000 = 
        let c1 = linearise (san aid000)
        let sci = insert c1
        sci

    let classOf_y aid000 = 
        let sci = classOf_ii aid000 
        scToStr sci
        //vprintln 0 (ident + sprintf "  ksc classOf_y create %s for %s" sc (aidToStr aid000))



    let rec rebuild = function
        | [ A2_leaf ss ]        -> A_loaf ss
        | [ A2_base sci ]       -> A_loaf (scToStr sci)
        | [ A2_tag(s1, s2) ]    -> A_loaf s1
        | A2_subsc(None)::tt    -> A_subsc(rebuild tt, None)
        | A2_subsc(Some sc)::tt ->
            let aid_o_o =
                dev_println (sprintf "Rebuild A_subsc with sc %i" sc)
                Some (A_loaf(scToStr sc))
            A_subsc(rebuild tt, aid_o_o)
        | A2_tag(s1, s2)::tt -> A_tagged(rebuild tt, s1, s2)


    // Add implied equivs between sons, but avoid looping.
    // Example,  when a is combined with b then b->bog is added to any class that contained a->bog.
    // Be careful though, a->foo does not get merged with {a->bog, b->bog}.
    // Precisely, for each son of a any member of the merged set we add that son 
    // When pairing up children, where there are cyclic data structures (e.g. parent has collection of children each with link to parent) going up to the 'top' could loop indefinately, so we need to overlay a tree and be aware of back edges and cross edges.
    // We assume a degree of strong-typedness in that an array can never contain itself as an entry, directly or through nested arrays.


    let do_readout vd =
        let m_ans = ref []
        let rec rdrd dones = function
            | [] -> ()
            | (pt, prefix)::tt ->
                if memberp pt dones then rdrd dones tt
                else
                    let dones = pt :: dones
                    let mx = ksc_table.[pt]
                    let uncollated = 
                        let m_ans = ref []
                        let pup1 kk vv = mutadd m_ans (kk::prefix, !vv)
                        for xx in mx do pup1 xx.Key xx.Value done
                        !m_ans
                    let collated = generic_collate snd uncollated 
                    let boh (pt1, items) =
                        if vd >= 5 then vprintln 5 (sprintf " readout %i %i %s" pt pt1 (sfold (fun (aidpath, yn) -> sprintf "(%A,%c%i)" (map a2ToStr (rev aidpath)) class_char yn) items))
                    app boh collated
                    let save (pt', items) = // For memdecs, will later collate on all ariving arcs at a dest point.
                        let save1 (item, dest_) = mutadd m_ans (pt', rebuild item) 
                        app save1 items
                    app save collated // can just do save1 on uncollated
                            
                    let further =
                        let best_example (pt', items) =
                            let shortest lst =
                                if nullp lst then sf "No shortest aid"
                                else
                                    let rec scan_shortest sofar = function
                                        | []            -> sofar
                                        | (aidl, _)::tt -> scan_shortest (if length aidl < length sofar then aidl else sofar) tt
                                    scan_shortest (fst(hd lst)) (tl lst)
                            //(pt', shortest items)
                            (pt', [A2_base pt'])
                        map best_example collated
                    rdrd dones (further @ tt) 


        rdrd [] [(0, [])]

        let collated_memdescs = generic_collate fst !m_ans
        if vd>=4 then vprintln 4 (sprintf "Readout done.  %i raw items. Reducing to %i when collated." (length !m_ans) (length collated_memdescs))
        collated_memdescs

    let filter_boring collated_memdescs =
        let report (pt', items) =
            if length items >= 2 then
                if vd>=5 then vprintln 5 (sprintf "ksc: Saving %i ties for ...->%i"   (length items)  pt')
        app report collated_memdescs
        // Can/should filter out the boring ones
        List.filter (fun (pt, items) -> length items >= 2) collated_memdescs

    let raw_readout vd =
        let collated_memdescs =
            match !m_inverted_index with
                | Some ov -> ov
                | None ->
                    let readout = do_readout vd
                    m_inverted_index := Some readout
                    readout
        collated_memdescs


// TODO remove anon_aid and null_aid from conglom action.
    // TODO check san
    member x.setNullFlag msg_ aid = set_null_flag (classOf_ii aid)

    member x.nullSet() = contains_null // Access to the enumerable set.

    member x.classes() =
        list_once (map (fun x -> !x) !m_all_points)

    member x.lookup_null_flag sci =
        check_null_flag sci

    member x.classOf_i msg_ aid000 = classOf_ii aid000

    member x.attribute_set sci key vale = auxiliary_designations.add sci (key, vale)

    member x.attribute_get key aid =
        match auxiliary_designations.lookup aid with
            | ats -> op_assoc key ats

    member x.classOf_yes aid =
        classOf_y aid
    
    member x.classOf arg = scToStr(insert(linearise(san arg)))

    member x.classOf_q arg = lookup(linearise(san arg))

    member x.Lock whom = 
        m_locked := Some whom // TODO locked should stop existing ones disappering during conglom, so becomes more of a checkpoint ? Only an issue if kcode env is forwarded in tree form. And the discarded points can be kept as aliases explicitly easily enough...

    member x.seen__ msg aid = // same as other API members
        classOf_y aid

        
    member x.readOut vd =
        let collated_memdescs = raw_readout vd
        let ans = filter_boring collated_memdescs
        ans

    member x.conglom msg arg =
        let sci =
            match arg with
                | []  ->             sf (msg + " gsc_conglom fewer than twoss")
                // TODO check san
                | [item] ->
                    clear_inverted_index()
                    insert(linearise(san item)) // One argument is also a nop, but kindly return its class.
                | items ->
                    clear_inverted_index()
                    let items = map san items
                    let lin_items = (map linearise items)
                    let m_further_work = ref []
                    let pair_up2 depth oc nc =
                        let sc_oc = insert oc
                        let sc_nc = insert nc
                        let ans = pairup2_top m_further_work sc_oc sc_nc
                        ans:int

                    let rec pair_up1 = function
                        | [aa;bb] -> pair_up2 0 aa bb
                        | aa::bb::tt ->
                            let cc = pair_up2 0 aa bb // Note bb is the new one
                            pair_up1 (bb::tt)
                        | []
                        | [_] -> sf "L365"

                    let sci = pair_up1 lin_items
                    if nullp !m_further_work then sci
                    else
                        iterate_further_work m_further_work
                        insert(hd lin_items)

        sci


    // This call should not be needed now since we do the wildcard handling during insert.
    member x.resolveWildcards msg =
        clear_inverted_index()
        let m_work = ref []
        let wildcard_divert (pm:ksc_point_map_t) =
            let m_wild_dests = ref []
            let wd1 kk vv =
                match kk with
                | A2_subsc(None) -> mutaddonce m_wild_dests vv
                | _ -> ()

            // TODO worth flagging up presence of more than one wildcard transition?
            let wd2 kk vv =
                match kk with
                | A2_subsc(Some jf) -> app (fun alt_vv->mutadd m_work (!vv, !alt_vv)) !m_wild_dests
                | _ -> ()

            for vv in pm do wd1 vv.Key vv.Value
            if not_nullp !m_wild_dests then for vv in pm do wd2 vv.Key vv.Value                
        app wildcard_divert !m_all_pms
        vprintln 2 (msg + sprintf ": ksc: resolveWildcards: %i work items" (length !m_work))

        let pairup_wild_dests lst =
            let m_further_work = ref []
            let rec pair_up1q = function
                | [] -> ()
                | (aa, bb)::tt ->
                    if aa<>bb then ignore(pairup2_top m_further_work aa bb)
                    pair_up1q tt
            pair_up1q lst
            if not_nullp !m_further_work then iterate_further_work m_further_work
        pairup_wild_dests !m_work 
        ()

    member x.sciToStr sci = scToStr sci

    member x.members vd sci = 
        let collated_memdescs = raw_readout vd
        match op_assoc sci collated_memdescs with
            | None ->
                if memberp sci !m_retired_sci_list then hpr_yikes(sprintf "Attempt to retrieve by retired sci %i" sci)
                []
            | Some items -> items

end

let g_unbound_sc = Memdesc_scs "$unbound$"

// ksc storage class export in memdesc format.
let sc_export ww (ksc:ksc_classes_db_t) msg bindnew cc aid00 =

    let aid_add_solo msg aid = 
        let sci = ksc.classOf_i msg aid
        ksc.sciToStr sci // There's a method that does this anyway?


    let rec ex m_discarder arg =
        match arg with
            | A_loaf nemtok ->
                match ksc.classOf_q arg with
                    | None when not bindnew ->
                        vprintln 3 (msg + sprintf ": get_e_cls - not in a class so discard (parent=%s) local=%s"  (aidToStr aid00) (aidToStr arg))
                        m_discarder := true
                        g_unbound_sc

                    | None when bindnew ->
                        let msg = "L401"
                        let cls = aid_add_solo msg arg 
                        //dev_println (msg + sprintf ": get_e_cls - not in a class so bindnew  using %s for %s" cls (aidToStr aid00)
                        Memdesc_scs cls

                    | Some sci -> Memdesc_scs(ksc.sciToStr sci)
            | A_subsc(v, None)        -> Memdesc_ind(ex m_discarder v, None)
            | A_subsc(v, Some aid_o)  -> Memdesc_ind(ex m_discarder v, Some(ex (ref false) aid_o))
            | A_tagged(v, ptag, utag) -> Memdesc_via(ex m_discarder v, [utag])

    let m_discarder = ref false
    let tx = ex m_discarder aid00
    if !m_discarder then cc else singly_add tx cc


// eof
