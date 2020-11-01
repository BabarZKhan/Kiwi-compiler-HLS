// Kiwi Scientific Acceleration: KiwiC compiler.
// 
// cilnorm.fs - This is the main file of KiwiC compiler front end.  It calls intcil to first convert CIL to kcode.
// 
// (C) 2007-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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


module cilnorm

open Microsoft.FSharp.Collections
open System.Collections.Generic

open moscow
open yout
open meox
open hprls_hdr
open abstract_hdr
open abstracte
open linepoint_hdr
open dotreport

open kiwi_canned
open cilgram
open asmout
open protocols
open kcode
open firstpass
open intcil
open cecilfe
open ksc

let kiwic_banner = "Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019"

let cilnorm_sitemarker = "cilnorm_sitemark"

let sitemarkerf aid = "SFA:" + aidToStr aid



// let g_temp_count = ref 500 // An old backstop bug catcher clips output file size.

let g_kiwife_topname_o = ref None

let g_old_hfast_support = false

let result_dt_to_prec ww msg rt = 
    let bits = ct_bitstar ww msg 0 rt
    let prec =
        { g_default_prec with
            widtho=Some(int32 bits);
            signed=ct_signed ww rt;
        }
    prec

type xition_t = Map<int, int list>

// Active pattern that can be used to assign values to symbols in a pattern
//let (|Let|) value input = (value, input)


type server_t = hexp_t * (string * hexp_t list * hexp_t option * string)

type gdp_t =
    {
       psef:       post_static_elab_t    // Post static elaboration flag.
       gen:        grecord_t option      // Code writer.
       //servers__:  server_t list ref
       m_tthreads: spawned_thread_t list ref
       vd:         int                   // Verbal diorosity loglevel.
    }



let g_hardcoded_library_substitutions =
    // This substitution list needs to be far more comprehensive.
    // This substitution list needs to be augmentable from the recipe please.

    [  // From/to mapping, plus flags.
       (["System"; "Random"],                       ["KiwiSystem"; "Random"],                       [])
       (["System"; "Threading"; "Barrier" ],        ["KiwiSystem"; "Threading"; "Barrier" ],        [])
       (["System"; "Threading"; "Interlocked" ],    ["KiwiSystem"; "Threading"; "Interlocked" ],    [])       

        // For Sqrt, Sin, Cos, Tan, Log, Exp etc.. See kickoff library stuff?
       (["System"; "Math"  ],                       ["KiwiSystem"; "Math"],                         [])        

        // Filesystem - KiwiFilesystemStubs.cs

    ]

let g_next_thread_id = ref 400

let get_next_tid style specific prefux =  // Next thread name/number allocator
    let ntid = !g_next_thread_id
    mutinc g_next_thread_id 1
    { style=style; specific=specific; no=ntid; id=prefux + i2s ntid }


// Create a name for a thread, somewhat resembling its src name.
let rez_tid_prefix ids =
    let ids = toupper ids
    let ll = strlen ids
    if ll > 8 then
        ids.[0..3] + ids.[ll-4..ll-1]
    elif ll > 3 then ids
    else "TMAIN"


type kiwic_root_t =
|   Root_s of stringl            // A compilation root specified by a cmd-line string
|   Root_cc of tid3_t * spawned_thread_t * director_spec_t * performance_target_t option // A compilation root specified by Kiwi.Hardware() attribute.

let item_tid = function
    | Root_cc(tid3, dir, _, _) -> tid3
    | Root_s idl -> sf "L6437"

let item_specific = function
    | Root_cc(tid3, dir, _, _) -> tid3.specific
    | Root_s idl -> sf "L114"

let item_name = function
    | Root_cc(tid3, ROOT_THREAD1(idl, idl_o, uid), _, _) -> valOf_or idl_o idl
    | Root_cc(tid3, USER_THREAD1 _, _, _)           -> [tid3.id]
    | Root_s idl -> idl



#if SPARE
let nullp = function
    | []    -> true
    | other -> false

let non_nullp = function
    | []    -> false
    | other -> true
#endif

let rec stringl_order = function
    | ([], []) -> sf "stringl_order same"
    | ([], _) -> true
    | (_, []) -> false
    | (h::t, p::q) ->
        if h=p then stringl_order(t, q)
        else
            let l = (h:string).GetHashCode()
            let r = (p:string).GetHashCode()            
            in l < r

// let env_comparer = LanguagePrimitives.FastGenericComparer<env_key_t>.Compare


let n64_lift n = CE_x(g_canned_i64, xi_int64 n)
let n64_liftx x   = CE_x(g_canned_i64, x)


// Printout the environment
let printenv xvd depthlimit msg env = 
    let yo s = vprintln xvd s
    yo ("-- printenv " +  msg + " --")
    yo ("    " + ce_hs_envToStr depthlimit env.hs1)    
    let kesToStr x = x
    let  _ =
        let penv1 kes r = (yo(sprintf "    %s  --> %s " (kesToStr kes) (env_itemToStr depthlimit r)); true)
        Map.forall penv1 env.mainenv
    yo "-- printenv end -- \n\n"
    ()

    
let splus a b  = if a=int64 g_unaddressable then b elif b=int64 g_unaddressable then a else a+b
let stimes a b = if a=int64 g_unaddressable then b elif b=int64 g_unaddressable then a else a*b

//
// Increment an array subscript (with or without a range present) but if
// either operand is unadressable then return the other... somewhat strange ... why is this correct?
//    
let rec wt_plus (a,b) =
    //let _ = vprintln 0 ("wt_plus a=" + xToStr a + " b=" + xToStr b)
    match (a,b) with
    | (X_num a, X_num b) -> if a=int32 g_unaddressable then xgen_num b elif b= int32 g_unaddressable then xmy_num a else xmy_num(a+b)
    | (X_pair(rng, a, _), b) -> ix_pair rng (wt_plus(a, b))
    | (a, X_pair(_, _, _)) -> sf "wrong format splus"
    
    | (X_num a, b) -> if a=int32 g_unaddressable then b else ix_plus (X_num a) b
    | (a, X_num b) -> if b=int32 g_unaddressable then a else ix_plus a (X_num b)
    | (a, b) -> ix_plus a b

let rec gec_O_subs m = function
    | (aid, X_pair(_, base_ptr, _), len, offset) ->  gec_O_subs m (aid, base_ptr, len, offset)
    | (aid_o, base_ptr, len, offset) when constantp offset && constantp base_ptr ->
        let ibase = xi_manifest64 "gec_O_subs_ibase" base_ptr
        let a = xi_manifest64 "gec_O_subs_offset" offset        
        let (aid_bo, aid_oo) =
            match aid_o with
                | None                         -> (None, None)
                | Some(A_subsc(aid_b, aid_oo)) -> (Some aid_b, aid_oo)
        O_subsc(aid_bo, aid_oo, ibase, a, len, ibase + a)    
    | (aid_o, other, len, offset) ->
        let (aid_bo, aid_oo) =
            match aid_o with
                | None                         -> (None, None)                
                | Some(A_subsc(aid_b, aid_oo)) -> (Some aid_b, aid_oo)
        let ans = O_subsv(aid_bo, aid_oo, other, len, offset)    
        ans
        
let rec gec_O_tag m = function
    | (aid, X_pair(_, base_ptr, _), (tag_no, tag_idl, dt_field, (tag_idx:int64))) ->  gec_O_tag m (aid, base_ptr, (tag_no, tag_idl, dt_field, tag_idx))

    | (aid, base_ptr, (tag_no, tag_idl, dt_field, tag_idx)) when constantp base_ptr ->
        let ibase = xi_manifest64 "gec_O_tag" base_ptr
        let ans = O_tagc(aid, ibase, (tag_no, tag_idl, dt_field, tag_idx), ibase + tag_idx)
        ans
       
    | (aid, other, (tag_no, tag_idl, dt_field, tag_idx)) ->
        // ("other base form in gec_0_tag for " + htos tag_idl + "  base=" + xToStr other)
        let ans = O_tagv(aid, other, (tag_no, tag_idl, dt_field, tag_idx))
        ans

//         
let insert_string_handle dt ats = dt
//    if is_string dt || not_nullp(op_assoc "is-string" ats) then g_string_prec else dt


// TODO use generic mine_atts
// TODO - actually, we do not set the rnsc marker anymore I think, so this whole function can be deleted.
// find a : rnsc = really no scale marker
let rec mine_rnsc = function 
    | CE_region(uid, dt, ats, nemtoko, _) ->
        let v = op_assoc g_rnsc_tag ats
        not_nonep v
    | CE_dot _ -> false                 // Dot subscripts do not require scaling so we can be ambivalent here.
    | CE_conv(_, _, arg) -> mine_rnsc arg  // atts could be in the casted-to?

    //| CE_subsc _ -> false             // The CE_subsc of a CE_subsc needs further study... TODO.

    | CE_x _ -> false
    | CE_var(shared_vrn, dt, idl, fdto, ats) ->
        let v = op_assoc g_rnsc_tag ats
        not_nonep v

    | other ->
        vprintln 3 (sprintf "mine rnsc other form ignored: other=%s" (ceToStr other))
        false

//
//
let additional_create_uav_1 ww shared dt idl ats = // This has one call site. TODO remove - shared gp regs only - never addressable
    let sheap_id = hptos idl
    let regionf = false
    let dtidl = ct2idl ww dt
    let cCC = valOf !g_CCobject
    let shared_vrn = -1 // For now. In the future we might want to supply a cross-referenceable no instead and store in sheap etc...
    //vprintln 0 (sprintf "additional_create_uav: %s ats are %s" sheap_id (sfold kk_ats ats))
    CE_var(shared_vrn, dt, idl, None, ats)

#if SPARE    
    match cCC.heap.sheap1.lookup sheap_id with
        | Some she -> she.cez
        | None -> muddy (sprintf "SHAR T2 %s" sheap_id) // TODO
        
    //let _ = // we do not expect it to be found in future - just an evolutionary transient case.        let skey = cil_typeToSquirrel dt []
    let uid = { f_name=idl; f_dtidl=dtidl; baser=g_unaddressable; length=None; }
    //vprintln 0 (sprintf "gpreg rez %s %s  foundold=%A" (htos skey) (htos idl) found)
            let dt = insert_string_handle dt ats
            let cez = muddy "second call CE_ var(PHYS uid, dt, idl, ats) // dont want firstuse vreg idl"
            let _ = cCC.heap.sheap.add sheap_id ((false, cez), ref None)
            let _ =
                if not shared then
                    let aid = idl
                    // aid and idl are identical for a single use scalar but you should not take the aid of a multiuse GP reg.
                    ignore(record_memdesc_item sheap_id "uav_additional_create_kernel" (*dtidl*) regionf ats (uid) cez dt)
            cez
#endif
    
type vreg_scoreboard_t =
    {
        n_pregs: int ref;           // Number of physical resources allocated
        phyregs: phyregs_database_t // Physical regs used by virtuals
    }

// Create GP registers using the original name or else reporting the shared used of a hybrid name.
let vreg_uav_complete ww msg scoreboard =
    let cCC = valOf !g_CCobject
    let m_ans = ref []
    let complete_uav shared idl vrn ats1 = 
        match cCC.heap.sheap2.lookup vrn with
            | None -> sf (sprintf "vreg_uav_complete: %s: virtual register V%i is unknown L234" msg vrn)
            | Some she ->
                match she.cez with // take dt and ats from an example one - somewhat crudely for now
                    | CE_var(vrn, dt, idl_, fdto, ats) -> additional_create_uav_1 ww shared dt idl (ats1@ats)
                    | other  -> sf (sprintf "L216 other %s" (ceToStr other))

    let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
    
    let rez scoreitem =
        let shared = colouringf && !scoreitem.usecount > 1
        let idl =
            if shared then
                let gp_reg_name = "GP" :: (hd !scoreitem.idls)
                let report_sharing orig =
                    let rline = sprintf "Register sharing: general %s used for %s" (htos gp_reg_name) (htos orig)
                    //let _ = vprintln 2 rline - dont need to log this explicitly since the report logs it
                    youtln cCC.rpt rline
                    ()
                app report_sharing !scoreitem.idls
                gp_reg_name // General purpose register with one sign of origin.
            else hd !scoreitem.idls 
        let ats1 = if shared then [ ("shared_gp", "true")] else [] // Suppress further spurious name aliases that do not occur in reality owing to disjoint control flow.
        let cez = complete_uav shared idl (hd !scoreitem.vrns) ats1
        let _ = scoreitem.cez := Some cez
        mutadd m_ans cez
        
    let complete skey phyregs =
        vprintln 3 (sprintf "complete uav rez for %s  %i items" (htos skey) (length phyregs))
        app rez phyregs
    for k in scoreboard.phyregs do complete k.Key k.Value done
    !m_ans


    
// Allocate a free physical register in place of a vreg (using affinity information in the future)
// The mappings are stored in m_vregs mutable map.
let vreg_alloc_phy ww m_vreg_use_report scoreboard msg vrn =
    let m1 = "vreg_alloc_phy"
    let ww = WN m1 ww
    let cCC = valOf !g_CCobject

    let vreg_uav_unaddressable_kernel skey vdt idl ats =
        let reuse =
            let rec scan_reuse = function // Select best based on affinity please (or perhaps create new if nothing nearby)
                | [] -> None
                | scitem::tt when not !scitem.inuse -> Some scitem
                | _::tt -> scan_reuse tt
            scan_reuse (scoreboard.phyregs.lookup skey)
        match reuse with
            | Some ov ->
                mutadd ov.vrns vrn
                mutadd ov.idls idl
                ov
            | None ->
                let scoreitem = { cez=ref None; idls=ref [idl]; usecount=ref 0; inuse=ref false; vrns= ref [vrn] } 
                let _ = mutinc scoreboard.n_pregs 1
                let _ = scoreboard.phyregs.add skey scoreitem
                scoreitem

    match cCC.m_vregs.lookup vrn with
        | None -> sf (sprintf "vreg_alloc_phy: %s virtual register V%i is unknown L293" msg vrn)

        | Some ov ->
            match !ov.phyreg with
            | Some scoreitem ->
                //They are all unallocated when finally used, since the colouring ran to completion before any use.
                //if not !scoreitem.inuse then dev_println (msg + sprintf ": vreg V%i seemingly used when dealloc'd but not really" vrn) 
                scoreitem // a resource is already allocated 

            | None -> 
                match cCC.heap.sheap2.lookup vrn with // Some duplication of lookup here. 
                    | Some she ->
                        match she.cez with
                           | CE_var(vrn, vdt, idl, fdto, ats) ->
                               let skey = cil_typeToSquirrel vdt []
                               let scoreitem = vreg_uav_unaddressable_kernel skey vdt idl ats // Create a new scoreitem to manage this register
                               mutaddonce scoreitem.idls idl
                               mutinc scoreitem.usecount 1
                               scoreitem.inuse := true
                               let report_line = (sprintf "Allocate phy reg purpose=%s msg=%s for V%i dt=%s usecount=%i" ov.purpose msg vrn (htos skey) !scoreitem.usecount)
                               vprintln 3 report_line
                               ov.phyreg := Some scoreitem // Also store in she.hexp TODO if not present
                               //let _ = if nonep !she.hexp then she.hexp := Some scoreitem else sf (sprintf "redone hexp setup for V%i" vrn)
                               mutadd m_vreg_use_report (report_line + "\n")
                               scoreitem
                           | other  -> sf (sprintf "vreg_alloc_phy: missing vreg V%i other=%A" vrn (ceToStr she.cez))

                    | None -> sf (sprintf "vreg_alloc_phy: missing sheap2 vreg V%i"  vrn)



let rec map_vrn_to_phy ww m_vreg_use_report msg ce = // Last minute phy regs should not be needed since all should be properly coloured during dfa. But this gets called a lot!
    let scoreboard = { n_pregs= ref 0; phyregs=new phyregs_database_t("phyregs backstop") }
    //dev_println (msg + ": Last minute alloc for " + ceToStr ce)
    match ce with
        | CE_var(vrn, dt, idl, fdto, ats) ->
            let scoreitem = vreg_alloc_phy ww m_vreg_use_report scoreboard msg vrn 
            match !scoreitem.cez with
                | Some cez -> cez
                | None ->
                    let cezl = vreg_uav_complete ww msg scoreboard
                    if nullp cezl then sf (msg + sprintf " L291 error on V%i" vrn) else hd cezl
        | other -> other


let vreg_release_phy ww scoreboard msg vrn =
    let ww = WN "vreg_free_phy" ww
    let cCC = valOf !g_CCobject
    match cCC.m_vregs.lookup vrn with
        | None -> sf (sprintf "vreg_release_phy: virtual register V%i is unknown L337" vrn)
        | Some ov ->
        match !ov.phyreg with
            | None ->  dev_println(sprintf "vreg_release_phy: vreg not bound when dealloc'd V%i" vrn) // shortly to be an error
            | Some scoreitem -> 
                if not !scoreitem.inuse then dev_println(sprintf "vreg_freeup: vreg free when dealloc'd V%i" vrn) // this too will shortly become an error
                scoreitem.inuse := false
                ()

    
let cr_yield (nc1, nc2, stars) =
    let k2 b = if b=g_anon_class then "c-blank" else "c" + i2s b
    if stars > 0 then
        let rec s n = if n=0 then "" elif n>0 then "s" + s(n-1) else  "*" + s(n+1)
        xi_uqstring(s stars + k2 !nc2)
    else
        match stars with
        | 0  -> xi_uqstring(k2 !nc2)
        //| -1 -> xi_uqstring(k2 !nc2)
        | n  -> xi_uqstring (sprintf "^%i%s" n (k2 !nc2))

        






let report_all_sc_mappings msg xvd =
    let cCC = valOf !g_CCobject
    let wl ss = vprintln xvd ss
    wl (msg + ": Report of all sc mappings")
#if OLD
    let estore = cCC.kcode_globals.ksc.classes()
    let report_mapping key (_, vale) = wl(key + " contains " + sfoldcr_lite aidToStr vale)
    for kv in estore do report_mapping kv.Key kv.Value done
#endif
    wl "Report of all sc mappings finished"

// Storing these under their nemtok class will not work since the classes are only establised after kcode static analysis.
// We must store them by aid afterall!
// Get current storage class to use in env for a memtok.
// We must avoid synonyms when we have  sc0={ a->b->c } and sc1={ f->c, a }
let get_current_sc msg (aid000:aid_t)  =
    let cCC = valOf !g_CCobject
    let ans = cCC.kcode_globals.ksc.classOf_yes aid000
    //vprintln 3 (sprintf "get_current_sc: %s: lookup %s yields--> %s " msg (aidToStr aid000) (aidToStr ans))                
    ans
    
let nemtok_to_sc msg (nemtok:new_nemtok_t) =
    get_current_sc msg (A_loaf nemtok)
    //let cCC = valOf !g_CCobject
    //let ans = Memdesc_scs(cCC.kcode_globals.ksc.classOf_yes aid)
    //vprintln 3 (sprintf "nemtok_to_sc %s -> %A" (aidToStr aid) ans)
    //ans

let get_memtok msg (aid000:aid_t)  =    
    match get_current_sc msg aid000 with
        //| A_loaf sc ->
        //dev_println (sprintf "get_nemtok: %A -> %A" aid000 sc)
        //    sc
        | (ss:string) -> "MM" + ss
        //sf ("memtok was not base  " + aidToStr other)
        

// For singletons we can use that nemtok=memtok until we have address-taken singletons for which aliases may exist.
// For indexed items we have an alias-spotting memtok function.
let idx_aid_to_mem ww msg nidx_ aid_o = 
    let msg = "idx_aid_to_mem"
    let memtok =
        match aid_o with
            | Some aid ->    get_memtok msg aid

            | None -> sf (msg + sprintf ": idx_aid_to_mem: no nemtoks nidx=" + i2s64 nidx_)
    memtok


let ce_to_scl msg ce =  // Dont like get_memtok in here!
    let aid_o = ce_aid0 (fun () -> "ce_to_sc " + msg) ce
    //vprintln 0 (sprintf "Group hof %A  aid for grp is " aidlst)
    let memtoks = if nonep aid_o then [] else [ get_memtok msg (valOf aid_o) ]
    memtoks


//
// general_subscription_lowerer
// This needs to generate some memdesc/class information to be passed along to help the disambiguator in repack stage.
//TODO reintroduce bounds_check, where possible, in general_subscription_lowerer.

// Note if we emit hbev code on a thread-by-thread basis we need to keep track and ties of sc values used in case they get renamed, but if we generate all kcode first and ensure sc classes are locked we are ok. TODO say which is used.
    
let general_subscription_lowerer ww m (a_ce, a_x, otag) =
    let tr = false
    //vprintln 0 (sprintf "general_subscription_lowerer: \n  a_ce=%s\n  a_x=%s\n  otag=%s" (ceToStr a_ce) (xToStr a_x) (idx2s otag))
    let cCC = valOf !g_CCobject
    let ksc = cCC.kcode_globals.ksc
    let establish_cls site = function
        | None -> sf (m + ": " + site + sprintf ": MISSING AID CLASS ZEROCH!  Please check log_classref needed: L297 %s " (ceToStr a_ce))
        | Some aid ->
            let escs = sc_export ww ksc site true [] aid
            if nullp escs then sf "establish class L536"
            else xi_stringx (XS_sc escs) (sfold mdToStr escs)

        
    let ce1_abase_add abase = function
        | CE_region(uid, dt, ats, nemtoko, _) ->
            let site = "abase_add CE_region"
            if nonep nemtoko then abase
            else
                let rr =
                    let rst = Memdesc_scs(get_current_sc site (A_loaf(valOf nemtoko)))
                    xi_stringx (XS_sc [rst]) (mdToStr rst) // TODO abstract this rez copy 1/2
                vprintln 3 ("ce1_abase_add " + xToStr rr + " from region nemtok=" + (valOf nemtoko))
                ix_pair rr abase // NB. this ignores any stars in aid which there will not be currently.

        | ce ->
            //"otherx " + ceToStr ce
            abase
    let df ss = if tr then vprintln 0 (" general_subscription_lowerer mode " + ss)
    let ans = 
        match (a_ce, a_x, otag) with
        | (a_ce_, a_x, O_tagc(ntl, baser, (tag_no, ptag_utag, dt, tag_idx), tot_)) -> // Field offset from a constant base pointer.
            let site = "O_tagc : FORM 1 "
            //vprintln 0 (msg + ce1ToStr a_ce_)
            let a = ix_pair (establish_cls site ntl) (xi_int64 baser)
            let a = ce1_abase_add a a_ce_
            let b = ix_pair (xi_uqstring("$offset+" + snd ptag_utag)) (xi_num64 tag_idx)
            let ans = ix_asubsc a_x (ix_plus a b)
            if baser=0L then hpr_yikes (sprintf "ARRAY HANDLE IS NULL (not set up when used) IN: O_subsc(.., baser=%i,  ans=%s" baser  (xToStr ans))
            if tr then vprintln 3 (sprintf "O_subsc(.., baser=%i,  ans=%s" baser  (xToStr ans))
            ans
            
        | (a_ce_, a_x, O_tagv(ntl, baser, (tag_no, ptag_utag, dt, tag_idx))) -> // Field offset from a variable base pointer.
            let site = "O_tagv : FORM 1 "
            //vprintln 0  (site + ce1ToStr a_ce_)
            let ov = baser
            let a = ix_pair (establish_cls site ntl) ov
            let a = ce1_abase_add a a_ce_
            let b = ix_pair (xi_uqstring("$offset+" + snd ptag_utag)) (xi_num64 tag_idx)
            let ans = ix_asubsc a_x (ix_plus a b) 
            if xi_iszero baser then hpr_yikes (sprintf "ARRAY HANDLE IS NULL (not set up when used) lower_subsc: O_tagv(.., baser=%s) ans=%s" (xToStr baser) (xToStr ans))
            if tr then vprintln 3 (sprintf "lower_subsc: O_tagv(.., baser=%s) ans=%s" (xToStr baser) (xToStr ans))
            ans

        | (a_ce_, a_x, O_subsv(aid_b, aid_o, W_string(ss, m, n), len_, idx)) -> // Special case: char extract rom constant string.
            df "O_subsv string"
            let ans = ix_asubsc (W_string(ss, m, n)) (ix_divide idx (xi_num 2))
            if tr then vprintln 3 (sprintf "lower_subsc: string form: O_subsv(.., baser=%s, idx=%s) ans=%s" ss (xToStr idx) (xToStr ans))
            ans

        | (a_ce_, a_x, O_subsc(aid_b, aid_o__, baser, idx, totlen_, sum_)) ->
            let site = "O_subsc"
            df site
            let a = ix_pair (establish_cls site aid_b) (xi_int64 baser)
            let b = ix_pair (xi_uqstring "$offset") (xi_int64 idx)
            let ans = ix_asubsc a_x (ix_plus a b)
            if baser=0L then hpr_yikes (sprintf "ARRAY HANDLE IS NULL (not set up when used) O_subsc(.., baser=%i, idx=%i, totlen_=%i, sum_=%i) ans=%s" baser idx totlen_ sum_ (xToStr ans))
            if tr then vprintln 3 (sprintf "O_subsc(.., baser=%i, idx=%i, totlen_=%i, sum_=%i) ans=%s" baser idx totlen_ sum_ (xToStr ans))
            ans

        | (a_ce_, a_x, O_subsv(aid_b, aid_o__, baser, totlen_, idx)) ->
            let site = "O_subsv"
            df site
            let a = ix_pair (establish_cls site aid_b) baser // change $baser to storage class
            let b = ix_pair (xi_uqstring "$offset") idx
            let ans = ix_asubsc a_x (ix_plus a b)
            if xi_iszero baser then hpr_yikes (sprintf "ARRAY HANDLE IS NULL (not set up when used) O_subsv(.., baser=%s, idx=%s, totlen_=%i) ans=%s" (xToStr baser) (xToStr idx) totlen_ (xToStr ans))
            if tr then vprintln 3 (sprintf "O_subsv(.., baser=%s, idx=%s, totlen_=%i) ans=%s" (xToStr baser) (xToStr idx) totlen_ (xToStr ans))
            ans

        | (a_ce_, a_x, O_tot n) ->
            let _ = df "no metainfo"
            let ans = ix_asubsc a_x (xi_int64 n) // no metainfo here!
            if xi_iszero a_x then hpr_yikes (sprintf "ARRAY HANDLE IS NULL (not set up when used). lower_subsc: no meta info ans=%s" (xToStr ans))
            if tr then vprintln 3 (sprintf "lower_subsc: no meta info ans=%s" (xToStr ans))
            ans

        | (a_ce_, a_x, O_filler s) -> muddy ("general_subscription_lowerer filler " + s)

    //vprintln 0 "general_subscription_lowerer done L319"
    ans

    
let wt_times = function // TODO delete me - the unaddressable aspect is now handled elsewhere
    | (X_num a, X_num b) -> if a=int32 g_unaddressable then xi_num b elif b=int32 g_unaddressable then xi_num a else xi_int64(int64 a * int64 b)
    | (X_num a, b) -> if a=int32 g_unaddressable then b else ix_times (X_num a) b
    | (a, X_num b) -> if b=int32 g_unaddressable then a else ix_times a (X_num b)
    | (a, b) -> 
         let k = function
             | (X_num _) -> "is a num, "
             | (X_bnum _) -> "is a bnum, "
             | (_) -> "not num, "
         let rr = ix_times a b
         //vprintln 20 ((k a) + (k b) + " wt_times ans " + xToStr r + "\n")
         rr

let k x = (vprint 0 (x + " ins\n"); x)


type tonc_t = 
    | T_steps of int * int (* Budget, allowance *)
    | T_runout 

let toncToStr = function
    | T_steps(n, b) -> sprintf " %i steps out of %i" n b
    | T_runout -> "RUNOUT"

let outdatedp = function
//    | (T_outdated _) -> true
    | _ -> false


let rec outdatedl c =
    match c with
//        | (T_outdated l) -> Some c
        |  _   -> None



// Is this an unbox ? 

let classToType ww C2 arg = arg // for now
   // let a = classToType_opt ww C2 arg
   // in if a = None then sf ("no classToType (TODO:move me post asmout)")
   //    else valOf a











(*------------------------------------------*)


 (*

 CIL processing stage: How it works: 

 The KiwiC Orangepath recipe stage reads CIL bytecode and generates VM code for the HPR 
 virtual machine.  The CIL bytecode from monodis was parsed to an AST by a bison parser but
 now we use the cecil front end to read the bytecode directly.


 A variable is a static or dynamic object field, a top-level method
 formal, a local variable, or a stack location.  For each variable we
 decide whether to subsume it at the CIL processing stage. If not
 subsumed, it appears in the abstract VM code that is fed to the next
 stage (where it may then get subsumed for other reasons).
 Variables that are subsumed in this way tend to be object and
 array handles.  Such variables must contain compile-time constant
 values throughout the execution of the output code.

 We perform a symbolic execution of each thread at the CIL basic block
 level and emit VM code for each block.  CIL label names that are
 branch destinations define the basic block boundaries and these appear
 verbatim in the emitted VM code.

 Although CIL bytecode defines a stack machine, no stack operations are
 emitted from the CIL processing stage. Stack operations within a basic
 block are symbolically expanded and values on the stack at basic block
 boundaries are stored and loaded into stack variables on exit and
 entry to each basic block.  The stack variables are frequently
 subsumed, but can appear in the VM code and hence, from time-to-time,
 in the output RTL.

 A -root command line flag enables the user to select a number of
 methods or classes for compilation.  The argument is a list of
 hierarchic names, separated by semicolons. Other items present in the
 CIL input code are ignored, unless called from the root items.

 Where a class is selected as the root, its contents are converted to
 an RTL module with IO terminals consisting of various resets and
 clocks that are marked up in the CIL with custom attributes (see
 later, to be written).  The constructors of the class are interpreted
 at compile time and all assignments made by these constructors are
 interpreted as initial values for the RTL variables.  Where the
 values are not further changed at run time, the variables turn
 into compile-time constants and disappear from the object code.

 Where a method is given as a root component, its parameters are added
 to the formal parameter list of the RTL module created. Where the
 method code has a preamble before entering an infinite loop, the
 actions of the preamble are treated in the same way as constructors of
 a class, viz.\ interpreted at compile-time to give initial or reset
 values to variables.  Where a method exits and returns a non-void
 value, an extra parameter is added to the RTL module formal parameter
 list.


 The VM code structure for a thread consists of an array of
 instructions addressed by a virtual program counter.  Each instruction
 is an assignment, wait primitive, I/O primitive or conditional branch.
 The expressions occurring in various fields of the instructions may be
 arbitrarily complicated, containing any of the operators and
 referentially-transparent library calls present in the input language,
 but their evaluation must be non-blocking.  The VM code can be
 processed by the HPR tool in many ways, but of interest here is the
 'convert_to_fsm' operation that is activated by the '-vnl' command
 line option.


Overview of operation:

 cecil front end reads CIL bytecode of relevant files. These are stored and also
 a disassembly is written to ast.cil with -kiwic-cil-dump=combined or separately.


 The 'firstpass' code divides each method's body into basic blocks, works out
 the type of each expression on the stack and inserts loads and stores
 at basic block boundaries so that nothing is on the stack between basic blocks of the method.
 All returns are redirected to a special 'last' basic block which is the only exit from the method.

intcil: Removing the stack:  
   1. the net profit and loss on the stack for each basic is readily calculated
   2. we insist on the same stack depth at each entry to a given basic block
   3. the stack depth at the entry point to a method is zero
   4. combining the above information, we work out the number of spill vars active
at entry to each basic block. We also note their types.

 intcil: For each thread, a flat 'kcode' program is generated that inlines the bodies of all subroutines
 invoked by that thread. The kcode form is a bit like a FORTRAN IV + malloc program. It has no stack
 but does have a heap.

 A classical constant 'meet' algorithm is run on the kcode form of each thread so that we can remove (subsume) all
 compile-time constant variables - clearly we need to take remote thread writes into account in this.

 We find a lasso stem of kcode. The remainder of the thread (if any) is then the eternal code that turns into an FSM. 
 The FSM runtime entry point will be what remains, starting from the first control flow back edge that is not unwound.
 
 ERROR: the bounded recursion idea requires symbolic eval - if the bottoming out cannot be done adequately
 during intcil we need to do this lazily/dynamically too.
 
 TODO: process the inHardware pragma during intcil to extract only relevant parts to the kcode.


 The heap pointer must be come a constant under the meet algorithm ... otherwise we have unbounded
 dynamic storage allocation.

 The kcode is then emitted in the HPR VM DIC form for further processing by the HPR library.
 


Sybolic expression: CE forms:
  We do symb eval for pointer references and this is only on strongly-typed code. Casting c++
pointers between object types will not work.

  We allocate every object an offset in a nominal flat address space.  Later we make an
array (wondarray) for each object type but use the same offsets, so the arrays are sparse (until repacked in next recipe stage).

  For loads and dereferences, symbolic expressions are loaded on to
the stack and if the expression is bound in the env, that value from
the env is pushed instead.  This is the basis of the CIL loop unwind.

The main operators in the symbolic expression are the normal arithmetic and
logical operators, but we also have the field tag and array subscription abstractions.
 *)


let idx2i = function
    | O_tot n -> n
    | O_tagc(nt, baser, tagdata, n)        -> int64 n
    | O_subsc(_, _, baser, len, offset, n) -> int64 n    
    | O_tagv _
    | O_subsv _ -> sf "idx2i v"
    
    | O_filler s -> sf "idx2i filler"
    
let idx_nc = function
    | O_tagc(ntl, baser, tagdata, n)               -> ntl
    | O_subsc(ntl, aid_o__, baser, len, offset, n) -> ntl  
    | O_tagv(ntl, _, _)                            -> ntl
    | O_subsv(ntl, aid_o__, _,  _, _)              -> ntl   
    | O_filler s -> sf "filler"
    
let vari_idx_pred = function
    | O_tot n  -> false // not used currently

    | O_subsc _  | O_tagc _  -> false
    | O_tagv _   | O_subsv _ -> true
    | O_filler s -> sf ("vari_idx_pred filler " + s)



let i_tree_walk_ m t =
    if vari_idx_pred m then None
    else ce_tree_walk1 (idx2i m) t

let rec ce_tree_walk ce t =
    match ce with 
    | CE_x(_, X_num m)         -> ce_tree_walk1 (int64 m) t
    | CE_x(_, X_bnum(_, n, _)) -> ce_tree_walk1 (int64 n) t    
    | idx           -> sf("ce_tree walk: other index: " + ceToStr idx + "\n")

(*
 * Cez/env still needs idl form of a var.
 *)
let vuidToIdl(cid, cel) =
    let kx = function
        | (CE_x(_, a), cc) -> (xToStr a) ::  "%%" :: cc
        | (ce, cc) -> sf("vuidToIdl:" + ceToStr ce)
    kx(cel, cid);

let ceToStr_list lst = "(" + foldr (fun (a,b)->ceToStr a + (if b=")" then b else ", " + b)) ")" lst


 
// Non-constant values are replaced with CEE_tend zombie/bottom nonconst markers.
// (CE_null no longer used.)
//
// When marking death to a region, owing to undetermined index store, where it is a region of struct/object records, only one of the fields will be killed at a time and the remaining entries may be kept.
//
let death_rhs v =
    let vd = 2
    if ce_constantp v then gec_cee_scalar v
    else 
        if vd>=3 then vprintln 3 ("death: Values were killed from env since not a constant: " + ceToStr v)
        CEE_zombie "death"
        
let threadToStr = function
    | USER_THREAD1(srcfile, oo, mm, spawntoken, nn, args, env) -> "USER_THREAD1(" + ceToStr oo + ", " + ceToStr mm + ", " + ceToStr_list args + ")"
    //| REMOTE_THREAD(oo, mm) -> "REMOTE_THREAD(" + ceToStr oo + ", " + ceToStr mm + ")"


let cez_adupToStr(x, ce) = " [" + xToStr x + "]=" + ceToStr ce

                     

let eqfold = function
    | (h::t) -> h + "=" + (sfold (fun x->x) t)
    | x -> sfold (fun x->x) x


let is_a_handle ct = not(ct_is_valuetype ct)



//--------------------------------------------------------------------------------


(*
 * A 2-d array class has a special KiwiC canned template pram name for its content type: 2DCT
 *
 * Returns the template prams list and the templated root class.
 *) 
let cil_typeGenerics ww CF cgr other = 
    let _ = vprintln 0 ("+++ typeGenerics other :" + cil_typeToStr other)
    ([], (*Cil_cr_bin*) other)



(*
 * struct_access: Squirrel a struct token and a field name to obtain an idl for that field.
 *
 * On first pass, we have the type only of the struct, so must return something dummy...
 *)
//These names are string lists that lead with the finest (statically innermost on declaration) identifier.
let rec structg_name = function
    | CE_conv(dt, _, _) when dt=g_ctl_object_ref -> [ "$anon$" ]
    | CE_conv(_, _, CE_reflection(_, lto, idl_o, _, _))
    | CE_reflection(_, lto, idl_o, _, _) ->
        if not_nonep idl_o then valOf idl_o
        elif not_nonep lto then sf ("structg_name: found type not reflection name")
        else muddy "structg - nill reflection token encountered"
    | CE_var(hidx, vdt, idl, fdto, ats) -> idl
    | CE_region(uid, dt, ats, nemtok, cil) -> [ uidToStr uid ]     
    | CE_dot(ttt, ce, (_, _, (ptag, utag), dt, n), aid) ->   ptag :: (structg_name ce)
    | CE_x(_, X_num nn) -> [sprintf "%%W%i" nn] // These should only occur in cez names, not hexp net names. But also, thread dispatch of a static thread may have the number 0 in the object's name.
    | CE_x(_, X_bnum(_, bn, _)) -> [sprintf "%%W%A" bn] // These should only occur in cez names, not hexp net names. But also, thread dispatch of a static thread may have the number 0 in the object's name.    

    | other -> sf("structg_name other: " + ceToStr other)


// Create or retrieve an existing wondtoken for a type to serve as the base for a wondarray.
// A wondarray nominally holds all variables of a given type.
// We hope many of its subscripts are often exclusively constant so that repack can later partition or entirely remove the array. 
//
// We return as a second arg the lowered form of the wondarray: this is always used as the lhs arg of a subsc.
//
// Wondarrays seem to have a static nature? They are all nominally indexed from 0 in virtual space but no two wondarrays have data stored at overlapping addresses.
//
let retrieve_wondtoken ww ids dt = 
    let (ff, f2) =
        let width = if is_a_handle dt then (8L*g_pointer_size)
                    else ct_bitstar ww ids 0 dt
        let signed = ct_signed ww dt
        let arb = g_unspecified_array
        let (n, ov) = netsetup_start ids
        let wondtoken_ats = [ (g_wondtoken_marker, "true")]
        match ov with
            | Some ff ->
                (ff, lookup_net2 ff.n)
            | None ->
                let atsa = map (fun (a,b) -> Nap(a,b)) wondtoken_ats
                let ff = 
                    { 
                        n=         n
                        id=        ids
                        width=     int32 width
                        constval=  []
                        signed=    signed
                        rl=         0I
                        rh=         0I
                        is_array=  true
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
                let ff = netsetup_log(ff, f2)
                (ff, f2)
    xgen_bnet (ff, f2)


//
// The wondarray is all possible variables of a given type (excluding struct fields).  It has no nemtok until it is indexed.
// This routine, mk_wondarray, does not and should not accept an aid. cf gec_CE_Region.
// A wondtoken is a pseudo array base for all wondarrays of that type. 
// Where type abuse is spotted (casting as in gcc4cil) one wondtoken is used for all types with an equivalence group of wondarrays that are subjected to such casts.
//
let rec mk_wondarray ww mm content_type = 
    let volf = ct_is_volatile ww content_type
    let region_ats = if volf then [(g_volat, "true")] else [] // This gets applied to whole array but having it as part of the cid means disconnection of certain reads and writes : better as memdesc on patches... ? // TODO stored as a field in CT_NET but as an attribute in CE_region -- please unify
    let signed = ct_signed ww content_type
    //let CE = (f1o4 CF, f2o4 CF, f3o4 CF)
    match content_type with
        // Wondarrays of structs are perfectly sane, like any other valuetype.
        // But the fields of a struct should never be stored in their native wondarray: instead they should always have an arithmetic or CE_anon_struct packing into their parent.
        | _ ->
            let dt = CT_arr(content_type, None)
            let width = if is_a_handle content_type then 8L *g_pointer_size 
                        else ct_bitstar ww mm 0 content_type
            let dtidl = ct2idl ww dt
            let ids = hptos_net dtidl
            // The wondtoken is shared over all wondarrays of that type and sso the attributes for the CE_region, such as its length, do not go in the wondtoken.
            let wondtoken = retrieve_wondtoken ww ids content_type 
            let rnet__ = ref (Some wondtoken)
            let uid = { f_name=[ids]; f_dtidl=dtidl; baser=0L; length=Some(hd g_wondarray_marker_dims) }
            let cez = CE_region(uid, dt, region_ats, None, None)
            //if vd>=3 then vprintln 3 (mm + ": Made wondarray w=" + i2s64 width + " " + ceToStr cez)
            (cez, wondtoken)


// Shared resource TLM gis.
let gec_generic_sri_tlm_fgis prec sri_token fu_kind fsems_o lmodef =
    let ip_block_name = //for now
        { vendor= "HPRLS"; library= "misc"; kind=fu_kind; version= "1.0" }
    let fname = if lmodef then "write" else "read"
    let gis = if nonep fsems_o then g_default_native_fun_sig else { g_default_native_fun_sig with fsems = valOf fsems_o } 
    let gis = { gis with rv=prec; args=[g_reflection_token_prec]; is_fu=Some(ip_block_name, None);  outargs="b"  }
    let gis =
        if lmodef then { gis with fsems=l_eis_unmirrorable; rv=g_void_prec; args=[g_reflection_token_prec; prec]; outargs="b-" }
        else gis
    (fname,  gis)



// Net lowering code: addressable and unaddressable are handled differently.
// cf lnetgen  and netgen_kernel. This nethunk code should be elsewhere.
let rec ceToNet ww mm lmodef ce =
    let cCC = valOf !g_CCobject
    let vd = 3
    let ww = WN mm ww
    let m_vreg_use_report__ = ref [] // Not reported since not shared.

    let ensure_net netthunk she =
        match !she.hexp with
            | Some x -> x
            | None ->
                vprintln 3 ("Invoke netthunk")
                she.hexp := Some(netthunk())
                valOf !she.hexp

    match ce with
    | CE_region(uid, dt, ats, _, _) ->         
        let static_key = uid.f_name 
        let netthunk() =
            if uid.baser = 0L then (vprintln 3 ("the null pointer or HPR_top:" + ceToStr ce); xi_num 0)
            else
                let username = op_assoc "username" ats // The username is the only hexp_t attribute that is not manifest in the CE form.
                let ids =
                    if username<>None && String.length(valOf username) > 0 then valOf username
                    elif f_nonataken uid then hptos_net uid.f_name
                    else hptos_net uid.f_dtidl
                let rec ton_dims arg =
                    match arg with
                        | CTL_net _
                        | CTL_void        -> None
                        | CT_arr(dt, len) -> len
                        | CTL_record _    -> None
                        | dt ->
                            vprintln 3 ("ton_dims another " + typeinfoToStr dt)
                            None
                netgen_kernel ww mm (ids, dt, None, (* (if dims>=0 then [dims] else []) *) ats)
        let sheap_id = hptos static_key

        match cCC.heap.sheap1.lookup sheap_id with // named statics // indexable statics?? NO: wondarrays
           | Some ov ->
               // lprintln 10 (fun()->"ceToNet retrieved from heap: " + ceToStr (snd(fst ov)))
               ensure_net netthunk ov
           | None -> netthunk()

    | CE_var(vrn, dt, idl, fdto, ats) ->
        //TODO rationalise lnetgen and netgen_kernel and makes sure that username is processed on all branches
        match cCC.heap.sheap2.lookup vrn with // singleton heap lookup
           | Some she ->
               if vd>=4 then vprintln 4 (sprintf "ceToNet: V%i (FRI) for ce=%s  dt=%s" vrn (ceToStr ce) (cil_typeToStr dt))
               if she.ethereal then sf (sprintf "ethereal: An ethereal net has no hexp_t equivalent. Fail on V%i  idl=%s" vrn (hptos idl))
               //if ceToStr ce = "Test73r1.shared0" then dev_println(sprintf "hit it she=%A" she)
               if not_nonep she.tlm_propertyf then // TLM invoke - read/write method for now
                    let sri_dt = dt
                    let cf = { g_null_callers_flags with tlm_call=she.tlm_propertyf }
                    let (prec, _) = get_prec_width ww dt
                    let sri_token = "SRI_" + hptos idl
                    let fu_arg =
                        muddy(sprintf "ceToNet: need fu dt from %A" dt)
                        [ "bognor" ]
                    let (fname, gis) = muddy " gec_generic_sri_tlm_fgis prec sri_token fu_arg mdt.fsems lmodef "

                    let xobj = gec_X_net sri_token
                    xi_apply_cf cf ((fname, gis), [ xobj ])

               else
                   let netthunk() =
                       let username = op_assoc "username" ats // The username is the only hexp_t attribute that is not manifest in the CE form.
                       let ids =
                           if username<>None && String.length(valOf username) > 0 then valOf username
                           else hptos_net idl
                       netgen_kernel ww mm (ids, dt, None, (* (if dims>=0 then [dims] else []) *) ats)
                   ensure_net netthunk she
           | None -> sf (sprintf "ceToNet: missing vrn record for %s" (ceToStr ce))

    | ce -> sf(mm +  " other form in ceToNet: " + ceToStr ce)


(*
 * Create an hidx constant to return as an object pointer and also, for static instances, allocate
 * similar for each of the object's contents (which are valuetypes or object references).
 * mk_dyna_object is essentially malloc.
 *)
// gactuals are already evald.  they've been put as bindings in cgr but find_and_bind is going to do that again so dont need cgr passed in here? hmm now am using them...  all processed in the type checker now - so TODO edit this comment.
let rec mk_dyna_object_top11 ww m0 cCL aido ((dtidl, name_), idl, dt, cgr) staticmd strun env = // called from do_newobj from kcode interpeter.
    let vd = 2
    let dp x = if vd>=3 then vprintln 3 ("dyna:    " + x)
    //let (cC_:cilconst_t) = valOf !g_CCobject
    match strun with
    | CT_star(_, CTL_record(ttt, cst, crt_lst, len_bytes, ats, binds, srco))
    | CTL_record(ttt, cst, crt_lst, len_bytes, ats, binds, srco) ->
        let ids = hptos idl
        let mm = ": Creating class instance this/uid token=" + ids
        let m0() = m0() + mm
        let _ = WF 3 "mk_dyna_object_top" ww mm
        let lzy = ref None
        let dct = CT_class cst//one of a few rez sites
        let kfunique = funique // for now - may want to unroll on top-level iterate
        let bytes = int64 len_bytes // ct_bytes1 dct
        let types = ids
        let aspace = user_or_logical_memspace_lookup ww m0 [name_] ats
        let (hidxo, ce, envo) = obj_alloc_core m0 types true (staticmd idl) aido (Some env) aspace dtidl (int64 bytes)
        if vd>=4 then dp (sprintf "mk_dyna_object_top1l: L1170 heap sbrk %s " (ce_hs_envToStr 10 (valOf envo).hs1))
        
        let (obj, uid_final) =
            if nonep hidxo then (ce, None)
            else
                let uid_final = 
                  {   
                    length=    Some bytes
                    f_dtidl=   dtidl
                    baser=     valOf hidxo
                    f_name=    sprintf "%i" (valOf hidxo) :: idl
                  }
                let obj = gec_CE_Region_0 ww (aido, dct, uid_final) // An alternative mechanism to staticated.
                (obj, Some uid_final)

               
        vprintln 3 ("HFF 2a dct=" + typeinfoToStr dct + " wondtoken=" + ceToStr obj + " bytes=" + i2s64 bytes)
        //lprintln 2 (fun ()->"cil: instantiating class " + noitemToStr false strun + " with prefix=" + hptos (cCL.cl_codefix))
        let ww = WF 3 "dyna" ww ("instantiating class '" + ids + "'")

        let tcl' = {
                     cm_generics=   cgr       
                     this_dt=       Some dct
                     cl_prefix=     idl
                     cl_codefix=    idl
                   }

        let CCL' = {
                     text=         tcl'
                     this=         Some obj
                     cl_localfix=  idl
                   }


        dp "obj_alloc done"
        dp  (sprintf "mk_dyna_object_top1l: xheap sbrk %s " (ce_hs_envToStr 10 (valOf envo).hs1))                    
        // We dont need to allocate contents of referenced class instances. We do need to allocate local valuetypes, object handles and structs.
        // Don't need to do content details of referenced objects: 
#if SPARE
        // We need this code for structs... please explain ...
        let dyna_contents___ ats (base_idx, base_name) envo (crt:concrete_recfield_t) = //(idl, dct, b, l, members__)
                 // look up globally if really needed  if staticf then () // not in dyn object
                 // else
                let m = "dyna internal field"
                //dp ("No_field  4/4 " + name)                        
                let prefix = (if CCL'.text.cl_codefix = [g_globalname] then [] else CCL'.text.cl_codefix)
                let full_idl = crt.idl @ prefix
                let ww' = WF 3 m ww m
                let bytes = if ct_is_valuetype crt.dt
                            then 8L * int64(ct_bitstar ww 0 crt.dt)
                            else 8L * int64(ct_bitstar ww 1 crt.dt)
                //let ats = [] // todo extract from customs using abstract of static code
                //Here is where we might perform alignment.  Currently there is no rounding up for word alignment. TODO discuss.
                           
                let hidx' = base_idx + int64 crt.pos // obj_alloc Rdo_true false (full_idl, bytes)
                //let aid' = full_idl @ aid // backwards?
                let storeclass = STOREMODE_singleton_vector
                let dtidl = ct2idl ww crt.dt
                let (obj, env') = gec_uav ww' m0 storeclass false dt (full_idl, ats) vrn envo
                let uid = { baser=hidx'; length=Some bytes; dtidl=dtidl; name= kfunique "ij" :: base_name; }
                //record_memdesc_item aid' "dynamid" (*full_idl*) ats uid obj dt // now in gec_uav
                //vprintln 0 (sprintf " Ignored memdesc rez %A" (aid', "dynamid", full_idl, ats, uid, obj, dt))
                let _ = unwhere ww
                env'

        let envo =
            if false (* &&  strun *) then
                let uid_final = valOf uid_final
                let dyna_insert_in_env env = function
                    | (CTL_record(idl, cst, contents, reclen, ats_, binds, _), ats, cil) ->
                        List.fold (dyna_contents___ ats (uid_final.baser, uid_final.name)) env contents 
                    | (other, _, cil) -> sf ("dyna_insert_in_env other: " + typeinfoToStr other)

                dyna_insert_in_env envo (valOf(!lzy))
            else envo
#endif            
        //vprintln 0 (sprintf "mk_dyna_object_top1l: post op heap sbrk %s " (ce_hs_envToStr 10 (valOf envo).hs1))            
        (obj, uid_final, envo)

    | other ->
        sf (sprintf "mkdyna other %A" other)
        // for other fields? perhaps boxed ? 



(*
 * List of addresses that get no flow through from previous unconditional branch
 *)
let rec scan_ins (pc, ia:cil_t list array, il, c) =
    if pc=il then c
    else
        let ins = ia.[pc]
        let c' = if unconditional_jmp2 ins then (pc+1)::c else c                 
        scan_ins(pc+1, ia, il, c')





(*
 * Read expression list: return ml integer. Hope no overflow!  TODO initialised Int64?
 *
 * Bitradix is number of bits for each item.
 *)
let array_init_pack signed bitradix l = 
    let vd = 2
    let byteradix = bitradix / 8 // Byte radix - ignore remainder for now.
    if vd>=4 then vprintln 4 (sprintf "array_init_pack: bitradix=%i byteradix=%i" bitradix byteradix + ", " + i2s(length l) + " src items")

    let toByte = function
        | Cil_number32 n -> (byte) n
    let bytes = (map toByte l) |> List.toArray

    //let _ = reportx vd "Init_pack Items" (fun l -> iexpToStr(Cil_explist l)) d
    let negativep(Cil_number32 ms::_) = ms >= 128
    
    let zfun p =
        let offset = p * byteradix
        let ans = 
            if signed=Signed then
              match byteradix with
                | 1 -> gec_yb(xi_num(int (bytes.[offset])), g_canned_i32)
                | 2 -> gec_yb(xi_num(int (System.BitConverter.ToInt16(bytes, offset))), g_canned_i16)
                | 4 -> gec_yb(xi_num (System.BitConverter.ToInt32(bytes, offset)), g_canned_i32)
                | 8 -> gec_yb(xi_int64 (System.BitConverter.ToInt64(bytes, offset)), g_canned_i64)
                | _ -> muddy (sprintf "other signed init_pack byteradix of %i" byteradix)
            elif signed=Unsigned then
              match byteradix with
                | 1 -> gec_yb(xi_num(int (bytes.[offset])), g_canned_u32)
                | 2 -> gec_yb(xi_num(int (System.BitConverter.ToUInt16(bytes, offset))), g_canned_u16)
                | 4 -> gec_yb(xi_num(int (System.BitConverter.ToUInt32(bytes, offset))), g_canned_u32)
                | 8 -> gec_yb(xi_int64(int64 (System.BitConverter.ToUInt64(bytes, offset))), g_canned_u64)
                | _ -> muddy (sprintf "other unsigned init_pack byteradix of %i" byteradix)
            else
              match byteradix with
                | 4 ->
                    let f32 = System.BitConverter.ToSingle(bytes, offset)
                    let hexp_const = fps_core (f32.ToString()) 32 (XC_float32(float32 f32))
                    gec_yb(X_bnet hexp_const, g_canned_float)

                | 8 ->
                    let f64 = System.BitConverter.ToDouble(bytes, offset)
                    let hexp_const = fps_core (f64.ToString()) 64 (XC_double f64)
                    gec_yb(X_bnet hexp_const, g_canned_double)

                    
                | _ -> muddy (sprintf "other init_pack byteradix of %i - e.g. floating point !" byteradix)
        //let _ = vprintln 0 (sprintf "Array_init_pack signed=%A byteradix=%i  %A" signed byteradix ans)
        ans 
    let items = (length l) / byteradix
    let ans = try map zfun [0 .. items-1]
              with _ -> sf (sprintf "Integer overflow in array constant data: bytes=%i, lst=%A" byteradix l)
    ans

let lower_kiwi_pause_call ww args = gen_pause_call args



let m100 = "Undecidable subscript comparisons must be done at run time or in different clock cycles.  Insert additional pauses, of tf this array should be elaborated and ismarked as subsume or undec then add an elaborate attribute to it.\n";



let pflip(sid, l) = (sid, rev l)

(* Commonly a list of array array contents is consecutive heap items, so we convert that
 * to a subscription operation.
 *)
let rec find_patterns = function
    | ([], patterns) -> Some(map pflip patterns)
    | (CE_region(uid, dt, _, _, _)::t, []) -> find_patterns(t, [(uid.f_dtidl, [uid.baser])])
    | (CE_region(uid, dt, _, _, _)::t, (sid', idx)::ps) -> 
        if uid.f_dtidl=sid' then find_patterns(t, (uid.f_dtidl, uid.baser::idx)::ps)
        else find_patterns(t, (uid.f_dtidl, [uid.baser])::(sid', idx)::ps)

    | (other::t, ps) -> (vprint 3 ("give up on find_patterns: " + ceToStr other); None)


 
let patternToStr(sid, l) = "P " + htos sid + (sfold i2s l) + ")"

let rec incrementing = function
    | (a::b::t) -> b=a+1 && incrementing(b::t)
    | _ -> true


let reg_pattern(sid, lst) = 
    if incrementing lst then Some(hd lst, 1)
    else None


let assign_codegen_final gf (lhs, rhs) aso =
    let cCC = valOf !g_CCobject
    if lhs=rhs then vprintln 3 ("Skip assign lhs=rhs")
    else
        match lhs with
            // Supress initialisation of constants (they have their value by magic!)
        | X_bnet ff when at_assoc "constant" (lookup_net2 ff.n).ats  <> None -> ()
        | other ->
            if aso.reset_code && cCC.settings.supress_zero_struct_inits then
                // Discard clear-to-zero in constructor code (needed while gbuild does not respect big_bang for remote servers).
                vprintln 3 (sprintf "Discard reset assignment to %s" (xToStr lhs))
            else
                //dev_println (sprintf "No discard reset assignment to %s   reset_code=%A" (xToStr lhs) aso.reset_code)
            gf (Xassign(lhs, rhs))




let is_compiler_temp = function
    | X_bnet ff -> at_assoc "compiler-temp" (lookup_net2 ff.n).ats = Some "true"
    | _ -> false


// The assembler does some optimisations for us.
// (There's also the bisim code unused that could optimise more)
// Here we remove some other code not removed by the assembler ... vague please explain.
let kiwi_optimise_DIC ww vd dir arg =
    match arg with
    | SP_dic(dic, ctrl) ->
        let m_sh = ref []
        let block = H2BLK(dir, arg)
        let _ = WF 2 ("optimise_dic " + ctrl.ast_ctrl.id) ww "starting anal"
        let queries =
            { g_null_queries with
                concise_listing=   true
            }

        let _ = anal_block ww (Some m_sh) queries block
        let _ = WF vd ("optimise_dic " + ctrl.ast_ctrl.id) ww "done anal"
        let support = map fst (sd_oldmethod_support (map snd !m_sh))
        //dev_println ("kiwi_optimise_DIC: Support = " + sfold xToStr support)
        let kill n =
            let cmd = dic.[n]
            match cmd with
                | Xassign(l, r) when is_compiler_temp l && not(memberp l support) ->
                    let olds = hbevToStr cmd
                    if vd>=4 then vprintln 4 (i2s n + ": Killing (commenting out) assign to compiler temp: never read: " + olds)
                    Array.set dic n (Xcomment olds)
                | _ -> ()
        let _ = WF vd ("optimise_dic " + ctrl.ast_ctrl.id) ww "starting kill (commenting out) of spare lines" 
        app kill [0..dic.Length-1]
        let _ = WF 2 ("optimise_dic " + ctrl.ast_ctrl.id) ww "done kill of spare lines"
        ()

let hbev_emitter (gdp:gdp_t) k = 
   let vd = gdp.vd
   if gdp.gen=None then ()
   else
   let g = valOf gdp.gen
   let idx = !g.idx
   let emit1 = function
       | Xlabel s -> gec_Glabel g s (* We need to log the label using Glabel (todo: fix this api). *)
       | other -> G_emit g other
   (* let _ = dupflush CE GDP () *)
   if vd>=4 then vprintln 4 ("hpr emit at " + i2s idx + ":" + hbevToStr k + "\n\n")
   emit1 k


let lookup_lab (FP:firstpass_t) d = 
    let rec ll1 pos =
        if pos >= FP.il then sf("lookup_lab: Destination lab does not exist: " + d + "\n") else
            let (_, ins) = FP.ia.[pos]
            if memberp (gec_ins(Cili_lab d)) ins then pos else ll1 (pos+1)
    let ans = ll1 0
    //vprintln 30 ("  lookup_lab " + d + " --> " + i2s ans)
    ans

let map2seq (e:envmap_t) =
    if e.IsEmpty then Nil
    else
        let it = Map.toSeq e
        let it = it.GetEnumerator()
        let rec adv() =
            //vprintln 0 "Advance adv()"
            let aa = if it.MoveNext() then Cons(it.Current, adv) else Nil
            //vprintln 0 ("env2seq: " + (if mys_null a then "Nil" else penv_leaff(snd (snd (mys_hd a)))))
            aa
        let aa = adv()
        aa


// A node in the CEE_tree can be a further complete tree for non-atomic data, such as the results of bit-inserts or word updates in an array.
// Generate a sequence from our CEE_tree, but not (yet) sequencifying the contents of a node when that node is another such tree.
let vecmap2seq msg_ ee = // If a F# addict then we would use "seq { yield }"
    let rec ceeToSeq ee =
        //vprintln 0 ("vecmap2seq" + sprintf ": seqitem=%s" (ceTreeToStr 2 ee))
        match ee with
            | CEE_tree(ll, idx, vale, rr) -> // in-order lazy list - but node data is not sequenced until later.
                let a2 = if is_teend rr then mys_list1 (idx, vale) else (mys_cons (idx, vale) (ceeToSeq rr))
                if is_teend ll then a2 else mys_append (ceeToSeq ll) a2 
            | CEE_tend ss_ -> Nil
    let aa = ceeToSeq ee
    aa


    

// deqd operator: Perform equality comparison of compile-time constants.
// We expect constants to be reduced to CE_x and CE_anon_struct form already, but scalar variable addresses are also constant and ...
let rec ce_deqd l r =
    let vd = 2
    if vd>=4 then vprintln 4 (sprintf "ce_deqd L=%s\n        cf R=%s" (ceToStr l) (ceToStr r))

    let struct_addr l_token = function
        | CE_conv(_, _, CE_star(1, CE_struct(r_token, _, _, _), _))
        | CE_star(1, CE_struct(r_token, _, _, _), _)  ->
            if vd>=3 then vprintln 3 (sprintf "Compare struct tokens %s cf %s" l_token r_token)
            l_token = r_token
        | rhs ->
            //if vd>=3 then vprintln 3 (sprintf "Fail matcher on compare struct tokens %A cf %A" l_token rhs)
            false

    let ans = 
        match l with
        | CE_x(_, l) ->
            match r with
                | CE_x(_, r) -> xbmonkey(ix_deqd l r) = Some true
                | _ -> false

        // Trap here that the address of a variable matches itself, even if a variable itself is demed (owing to constant folding completeness) not to match itself in a lower clause.
        | CE_conv(_, _, CE_star(1, CE_var(hidx, dt, idl, fdto, ats), _))   // Symbolic address comparison - disregards the CE_conv to cope with casts.
        | CE_star(1, CE_var(hidx, dt, idl, fdto, ats), _)  ->  
            match r with
                | CE_conv(_, _, CE_star(1, CE_var(hidx', dt', idl', fdto', ats'), _))
                | CE_star(1, CE_var(hidx', dt', idl', fdto', ats'), _) ->
                    //let _ = vprintln 0 (sprintf "compare scalar addresses %A %A" l r)
                    hidx = hidx'
                | _ ->
                    //let _ = vprintln 0 (sprintf "compare scalar addresses otherwise %A %A" l r)
                    false

        // Trap here that the address of a struct matches itself, even if its contents are different.
        | CE_conv(_, _, CE_star(1, CE_struct(ltoken, _, _, _), _))    // Symbolic address comparison - disregards the CE_conv to cope with casts, but these are (mostly?) not made now owing to calling tyeq in gec_CE_conv.
        | CE_star(1, CE_struct(ltoken, _, _, _), _) when struct_addr ltoken r -> true


        | CE_star(n, arg, _)  ->  
            match r with
                | CE_star(m, arg_r, _) ->
                    n=m  && ce_deqd arg arg_r 
                | _ ->
                    false

        | CE_var(hidx, vdt, idl, fdto, ats) ->
            // We assume anything in a variable is not known at compile-time: otherwise it would be a constant not a variable.
            if vd>=5 then vprintln 5 (sprintf "cd_deqd: return false on variable " + hptos idl)
            false

        // | CE_alu ->  // If an uncomputed expression then a leaf must be non-constant owing to constant folding  completeness.

        | CE_struct(idtoken_l, dt, aid, items_l) ->
            match r with
                | CE_struct(idtoken_r, _, _, items_r) ->
                        let rec matchok = function
                            | [] -> true
                            | ((tl, _, Some l1), (tr, _, Some r1))::tt ->
                                let a1 = ce_deqd l1 r1
                                //dev_println (sprintf "rec deqd on anon struct item %s cf %s -> %A" tl tr a1)
                                a1 && matchok tt
                                //ce_deqd l1 r1 && matchok tt
                            | ((_, _, None), (_, _, None))::tt       -> matchok tt
                            | _ -> false
                        let ans = length items_l = length items_r && matchok (List.zip items_l items_r)
                        //dev_println (sprintf "ce_deqd anon_struct matched=%A" ans)
                        ans
                | _ ->
                    false

        | CE_region(luid, _, _, _, _) ->
            let rec rcomp = function
                | CE_conv(r_dt_, _, r_ce) -> rcomp r_ce // Could generalise over all clauses 
                | CE_region(ruid, dt', ats', _, _) -> luid.baser = ruid.baser 
                | _ -> false
            rcomp r

        | CE_conv(dt, _, l_ce) -> // Discard all other converts.
            let rec rcomp4 = function
                | CE_conv(r_dt_, _, r_ce) -> rcomp4 r_ce // Does this generalise over all clauses for us ? Yes, but is asymmetric in discarding rhs leading conv for no reason ... recode.
                | r -> ce_deqd l_ce r //
            rcomp4 r

        | CE_reflection(_, lo, ro, _, _) ->
            let rec rcomp3 = function
                | CE_reflection(_, lo', ro', _, _) -> lo = lo' && ro = ro'
                | _ -> false
            rcomp3 r


        | other  ->
            let _ = vprintln 0 (sprintf "+++ no ce_deqd main clause for " + ceToStr other)
            false // Conservative result.
    if vd>=5 then vprintln 5 (sprintf "  ce_deqd L=%s\n       cf R=%s ans=%A" (ceToStr l) (ceToStr r) ans)
    ans

// Check equality between a pair of environment values.
// Recording minor differences (currently just in method sets) in m_advanced if present.    
let rec cee_deqd (key:new_nemtok_t) m_advanced m_cgc mx l r =
    let vd = -1 // Normally -1 for debug off.
    let ans = 
      match l with
        | CEE_zombie _ ->
            match r with
                | CEE_zombie _ -> true
                | _ -> false
            
        | CEE_i64 l -> 
            match r with
                | CEE_i64 r -> l=r
                | _ -> false

        | CEE_scalar(l_rezno, l) ->
            match r with
                | CEE_scalar(r_rezno, r) -> l_rezno = r_rezno || ce_deqd l r
                | _ -> false

        | CEE_method_set llst ->
            let key = ["no-key-for-now"]
            let (ans, longer) = 
                match r with
                    | CEE_method_set rlst -> (llst = rlst, if length llst > length rlst then llst else rlst)
                    | _ -> (false, llst)
            let (m_changed, m_data) = m_cgc
            mutadd m_data (key, longer) 
            if not ans then
                vprintln 3 (sprintf "call graph changed at %s to %s - need rerun" (hptos key) (sfold htos longer))
                m_changed := true
                ()
            ans

        | CEE_vector(l_rezno, lvec) ->
            match r with
                | CEE_vector(r_rezno, rvec) -> l_rezno = r_rezno || compare_vectors vd key m_advanced m_cgc mx lvec rvec
                | CEE_zombie _ -> false
                | other -> muddy (sprintf "cee_deqd: compare_items other " + env_itemToStr 2 other)
             
        | other_l -> sf (sprintf "no cee_deqd tree clause for %A cf %A" other_l r)
    if vd>=8 then vprintln 8 (sprintf "  cee_deqd  key=%s %s cf %s  ans=%A" key (env_itemToStr 2 l) (env_itemToStr 2 r) ans)
    ans
    
and compare_vectors vd key m_advanced m_cgc mx lv rv =
    let msg = "compare_vectors"
    let lseq = vecmap2seq "ea0" lv
    let rseq = vecmap2seq "ea1" rv
    let rec tree_env_eq1 sofar lseq rseq =
        if mys_null lseq && mys_null rseq then sofar
        elif mys_null lseq || mys_null rseq then false
        else
            match mys_hd lseq with
                | (l_idx, l_v) ->
                    match  mys_hd rseq with
                        | (r_idx, r_v) ->
                            //let aa (m, q, idl) = htos idl
                            if vd>=5 then vprintln 4 (msg + sprintf ": key=%s (sofar=%A) compare_vectors lk=%i rk=%i  lv=%s rv=%s" key sofar l_idx r_idx (env_itemToStr 2 l_v) (env_itemToStr 2 r_v))
                            let sofar = 
                                if l_idx <> r_idx then
                                    if vd>=5 then vprintln 5 (sprintf "compare vectors key mismatch %i cd %i" l_idx r_idx)
                                    false
                                else sofar && (cee_deqd key m_advanced m_cgc mx l_v r_v)
                            if true && (not sofar) then sofar else // early stop if not debugging
                            tree_env_eq1 sofar (mys_tl lseq) (mys_tl rseq)

    let ans = tree_env_eq1 true lseq rseq
    if vd>=5 then vprintln 5 (sprintf "compare_vectors ans=%A" ans)
    ans

let rec cee_hs_deqd m_advanced m_cgc mx l r =
    match l with
        | CEE_heapspace _ ->
            let ans =
               match r with
                   | CEE_heapspace _ -> heapspace_compare m_advanced mx (l, r)
                   | _ -> false
            //vprintln 0 (sprintf "cee_hs_deqd heap %s cf %s deqd=%A" (ce_hs_envToStr 2 l) (ce_hs_envToStr 2 r) ans)
            ans
        | CEE_hs_tend _ ->
            let ans =
               match r with
                   | CEE_hs_tend _ -> true
                   | _ -> false
            //vprintln 0 (sprintf "cee_hs_deqd heap %s cf %s deqd=%A" (ce_hs_envToStr 2 l) (ce_hs_envToStr 2 r) ans)
            ans
            
        //| other_l -> sf (sprintf "no cee_hs_deqd tree clause for %A cf %A" other_l r)


// Check equality between a pair of environments.        
// Constant meeter deqd equality comparator. deqd is Fortran .EQ.
let cenv_deqd m_advanced m_cgc mx l r =
    let msg = "cenv_deqd"
    //let _ = vprintln 0 (sprintf "cenv_deqd start 1 l=%s r=%s" (ceToStr l) (ceToStr r))                                
    let c1_ans = cee_hs_deqd m_advanced m_cgc mx l.hs1 r.hs1
    //let early_stop = true
    let rec c2() =
        let lseq = map2seq l.mainenv  // we rely on these being sorted to a common order
        let rseq = map2seq r.mainenv
        let rec tree_env_eq2 lseq rseq =
            if mys_null lseq && mys_null rseq then true
            elif mys_null lseq || mys_null rseq then false
            else
                match (mys_hd lseq, mys_hd rseq) with
                    | ((l_tok, l_v), (r_tok, r_v)) ->
                          //vprintln 0 (msg + sprintf " (sofar=%A) step compare_vectors of  lv=%s rv=%s" sofar (penv_leaff l_v) (penv_leaff r_v))
                        if l_tok <> r_tok then false
                        elif cee_deqd l_tok m_advanced m_cgc mx l_v r_v then tree_env_eq2 (mys_tl lseq) (mys_tl rseq)
                        else false
        tree_env_eq2 lseq rseq
    let ans = c1_ans && c2()
    //vprintln 0 (sprintf "end 1 %A" ans)                                        
    ans

// Classical/Kildall73 constant latice meet algorithm needs a bottom flag for variables marked as not constant and mitre must preserve bottoms.
// We dentote a bottom with a CEE_zombie.
// See also "A Constant Propagation Algorithm for Explicitly Parallel Programs" by Lee, Midkiff and Padua.
let constant_meet_mitre msg m_cgc mx (ea0, ea1) = 
    let vdp = !g_gtrace_vd >= 4
    let hs_ans =
        let m_advanced = ref None
        let hs_mitre (lv, rv) =
            match lv with
                | CEE_heapspace _ ->
                    //vprintln 0 (sprintf "cee_deqd heap %s cf %s " (ceTreeToStr 2 lv) (ceTreeToStr 2 rv))
                    match rv with
                        | CEE_heapspace _ -> heapspace_compare (Some m_advanced) mx (lv, rv)
                        | _ -> false
                | CEE_hs_tend _ ->
                    match rv with
                        | CEE_hs_tend _ -> true
                        | _ -> false

        if hs_mitre (ea0.hs1, ea1.hs1) then
            let representative = if nonep !m_advanced then ea0.hs1 else valOf !m_advanced 
            representative
        else CEE_hs_tend "mitre-fail"
        
    let e0 = map2seq ea0.mainenv
    let e1 = map2seq ea1.mainenv    
    let normadd (cc:envmap_t) (k,v) = cc.Add(k,v)

    let isZombie = function
        | CEE_zombie _ -> true
        | _            -> false
    let zcode = function
        | CEE_zombie zz -> Some zz
        | _             -> None
    let rec iss cc (lseq, rseq) = // symmetric meet: items are flagged as not constant (at this point) if they conflict, otherwise it is a conjunction.
        if mys_null lseq && mys_null rseq then cc
        elif mys_null lseq then iss (normadd cc (mys_hd rseq)) (lseq, mys_tl rseq)
        elif mys_null rseq then iss (normadd cc (mys_hd lseq)) (mys_tl lseq, rseq)
        else
            match (mys_hd lseq, mys_hd rseq)  with
                | ((l_idx, l_v), (r_idx, r_v)) ->
                    let gmsg() = mx + sprintf ": const meet of\n   lk=%s" l_idx + "   lv=" + env_itemToStr 2 l_v + sprintf " and\n   rk=%s"  r_idx + "   rv=" + env_itemToStr 2 r_v
                    if vdp then dev_println (gmsg())
                    let m_advanced = ref None
                    let keymiss = l_idx <> r_idx 
                    if not keymiss && not(isZombie l_v) && not(isZombie r_v) && cee_deqd l_idx (Some m_advanced) m_cgc mx l_v r_v then 
                        let representative = if nonep !m_advanced then l_v else valOf !m_advanced
                        if vdp then dev_println (msg + " Noting a mitre keyhit and a valuehit on " + l_idx)                        
                        iss (normadd cc (l_idx, representative)) (mys_tl lseq, mys_tl rseq) // agree
                    elif not keymiss then
                        let token =
                            match zcode l_v with
                                | Some zz -> zz
                                | None ->
                                    match zcode r_v with
                                        | Some zz -> zz
                                        | None -> funique "nota_const" + " " + mx 
                        let nota_const c kk = normadd c (kk, CEE_zombie token)
                        if vdp then dev_println (msg + sprintf " Noting a mitre keyhit, %s, valuemiss zombie on " token + l_idx)
                        iss (nota_const cc l_idx) (mys_tl lseq, mys_tl rseq) // disagree
                    else
                        // disagreement items are bottom marked, but asymmetries are kept
                        if l_idx < r_idx // merge style advance on lower stream
                            then iss (normadd cc (l_idx, l_v)) (mys_tl lseq, rseq)
                            else iss (normadd cc (r_idx, r_v)) (lseq, mys_tl rseq)

    let ans = { hs1=hs_ans; mainenv=iss Map.empty (e0, e1); }
    if vdp then printenv 0 10 (mx + ": post-mitre meeter common ") ans
    ans


// TODO perhaps: keep compatible CEE_vectors that are not necessarily equal but which do not disagree on the mitre

     
(*
 * evolve: update env, deleteing any old value held, and storing a new if a manifest constant.
 * Must conservatively handle name alias for array subscripts, so a store to an undetermined index must delete all entries.
 *)
let rec evolve_idxd ww (ee:env_t) aid_o ats nidx vale0 =
    let msg = "evolve_idxd"
    let memtok = idx_aid_to_mem ww msg 0L aid_o
    let vari = vari_idx_pred nidx
    //vprintln 0 (msg + sprintf ": memtok=%s vari=%A nidx=%s rhs=%s" memtok vari (idx2s nidx) (ceToStr vale0))
    if ce_constantp vale0 && not vari then
        //if op_assoc "forced_elaborate" ats = Some "true" then (vprintln 3 "evolve: no evolve: forced elaborate"; ee) // No longer used
        if is_volatile ww vale0 then (vprintln 3 "evolve: no evolve: is_volatile rhs"; ee) // TODO - why check rhs for volatile ? Its a lhs property
        elif op_assoc g_volat ats = Some "true" 
        then (vprintln 3 "evolve: no evolve: is_volatile lhs"; ee) // these are not killing all entries but should not be present.
        else 
            let idx = int64(idx2i nidx)
            let vale = death_rhs vale0
            vprintln 3 (sprintf "evolve: vidx: memtok=%s idx=%s vari=%A rhs=%s" memtok (idx2s nidx) vari (ceToStr vale0))
            env_insert_vector ee memtok idx vale 
    else 
        // delete all entries in this indexed region
        vprintln 3 (sprintf "evolve: vidx: killing from env %A [%s] := %s" memtok (idx2s nidx) (ceToStr vale0))
        env_insert_vector ee memtok 0L (CEE_zombie "varindx")


// update a scalar (singleton) value in the keval semi-symbolic environment.
let evolve_scalar ww ee starx2 vrn ats vale = 
    let msg = "evolve_scalar"
    let vd = !g_gtrace_vd
    let cCC = valOf !g_CCobject
    let (idx, nemtok) =
        match cCC.heap.sheap2.lookup vrn with
            | None -> sf(sprintf "evolve_scalar L1694: vrn not defined v%i" vrn)
            | Some she -> (she.baser, she.s_nemtok)
    let memtok = nemtok
    let constant_rhs = ce_constantp vale
    let vv = if constant_rhs then gec_cee_scalar vale else CEE_zombie "evolve-scalar-rhs-nonconst"
    let ee = env_insert_scalar ee (memtok, idx, vv)
    if vd>=5 then vprintln 5 (sprintf "evolve_scalar: nona: constant_constant_rhs=%A nemtok=%s memtok=%s hidx=%i" constant_rhs nemtok memtok idx + " := " + ceToStr vale)
    ee


(*
 * client call to an 'rpc' server.
 * compile a call to a hardware server or an offchip (ie separately-compiled) rpc.
 * allocate the i/o busses if this is the first call to this target on this thread.
 *)
let rec scall_getnet s = function
    | [] -> sf ("kiwife: scall_getnet: protocol net "  + s + " is missing") 
    | (idx, (idl, dt, ce), xnet, external_net)::tt when s=hd idl ->
        vprintln 3 ("scall_getnet: net for i/o: " + hptos idl + " " + ceToStr ce + " " + xToStr xnet)
        xnet 
    | (idx, (idl, dt, ce), xnet, external_net)::tt -> scall_getnet s tt


// old-style HSIMPLE servers or stubs: calling side (drives req, monitors ack): 
// Now done for HFAST and so on in output component render based on director style being 
let make_hsimple_server_call ww kxr cf instancef (ll, xass, waitu) (v:scall_t, sargs, args) = 
    let vd = 3
    let ids = hptos v.idl
    reportx 3 "server call configuration" xToStr sargs
    //cassert(length sargs = 2, "wrong number of stub/server call config parameters")
    //lprint vd (fun()->"server call to " + cil_classitemToStr(usersub, ""))
    let msg = "generating a call to Kiwi auxiliary IP block " + ids // + " via " + xToStr id_ + sprintf "callflags=%A" cf 
    let ww = WF  3 ("inlining call ") ww msg

    let retnet = v.rn
    let req = scall_getnet "req" v.hs_nets
    let ack = scall_getnet "ack" v.hs_nets                  

    let args =
        if instancef then
            if nullp args then sf (sprintf "Missing this argument to RPC instance call")
            let that = hd args            // TODO dispatch on this to correct instance ...
            tl args
        else args
    let rec z_call = function
        | ([], []) -> []
        | (actual_ce::t, (_, (idl, dt, net_ce), net, external_net)::nets) -> 
            let _ = xass net_ce actual_ce // Store actual expressions into nets to slave IP block.
            z_call(t, nets)

        | ([], _)
        | (_, []) -> sf (sprintf "wrong number of args in RPC server call to %s. instancef=%A  %i cf %i" ids instancef (length args) (length v.arg_nets))

        //| (other::_, _) -> sf ("server call arg form other : " + ceToStr other)

    let _ = (waitu(xi_not(xgen_orred(ack))); // canned 4-phase h/s code.
             ignore(z_call(args, v.arg_nets)); 
             xass (gec_yb(req, g_canned_bool)) (gec_yb(xgen_blift X_true, g_canned_bool));
             waitu(xgen_orred(ack));
             xass (gec_yb(req, g_canned_bool))  (gec_yb(xgen_blift X_false, g_canned_bool));
             ()
            )
    ()

(*
 * convert the java-style {n}  printf args to a conventional %s, %h/%x, %f or %d form.
 *)
let printf_ansi_form mf = function
    | (_ :: types, (_, W_string(v, _, _)) :: tt) ->
        let vd = -1
        let re = ref []
        let rec present deflt = function
            | (_,  W_string(_, _, _)) -> "%s"
            | (oty, X_bnet ff) -> 
                let f2 = lookup_net2 ff.n
                if at_assoc "presentation" f2.ats = Some "char" then "%c"
                else if at_assoc "presentation" f2.ats = Some "string" then "%s" 
                else deflt
            | (oty, W_node(prec, _, [l; r], _)) -> present (present deflt (oty, r)) (oty, l)
            | (_, _) -> deflt

        let zz (argno, modestring) =
            let od = chr_ord argno
            if od >= 0 then
                let v = try select_nth od tt 
                        with _ -> cleanexit (mf() + sprintf ": printf/writeline arg index %i out of range %i (large arities passed as vectors not supported)" od (length tt))
                let usercoding = select_nth od types
                let (mode, fieldwidth) =
                    if nonep modestring then (None, "")
                    else
                        let l = strlen (valOf modestring)
                        (Some(valOf(modestring:string option).[0]), (if l > 1 then (valOf modestring).[1..] else ""))
                let coding =
                        if mode   = Some 'x' then "%" + fieldwidth + "x"
                        elif mode = Some 'X' then "%" + fieldwidth + "X"
                        elif mode = Some 'c' then "%c"  // is this char standard? should look at arg type really within present.
                        elif usercoding <> "" then usercoding
                        else present ("%" + fieldwidth + "d") v
                //dev_println (sprintf "modestring >%s<  coding>%s< for types " (valOf_or_ns modestring) coding + sfold id types)
                let _ = mutadd re v
                explode coding
            else explode("??badarg")
        let rec parse_format_specifier = function
          | [] -> []
          | '{' :: arg_no :: '}' :: tt -> zz(arg_no, None) @ parse_format_specifier tt

          | '{' :: arg_no :: ':' :: tt ->
              let rec scan_to_rbrace sofar = function
                | [] -> ([], []) // malformed
                | '}'::tt -> (tt, rev sofar)
                | h::tt -> scan_to_rbrace (h::sofar) tt
                       
              let (tt, modestring) = scan_to_rbrace [] tt
              zz(arg_no, Some (implode modestring)) @ parse_format_specifier tt

          
          | '%' :: tt -> '%' :: '%' ::parse_format_specifier tt // Escape any user percent signs by repeating them.
          | h::tt     -> h::parse_format_specifier tt
        let v' = parse_format_specifier (explode v)
        let ans = implode v'
        if vd>=5 then vprintln 5 ("ansi_form arg string is " + ans)
        (xi_string ans) :: (map snd (rev !re))
    | (types, l) -> (map snd l)

//

// When a region is passed from one thread to another via a threadStart there is no kcode store0 tie generated, so we track them numerically with this global database.
// We normally aim to use a pointout instead? The standard nemtok pairing should work. But this catches all other name aliases and does no harm.
// The memdesc database is global in the same way that the HPR VM2 name space is flat.  The definition of some of this database should therefore be inside hpr not KiwiC.
let g_numeric_ties = new ListStore<int64, memdesc_record_t>("numeric_ties") //

let tiegen_numeric_mirking baser rst =
    g_numeric_ties.add baser rst
    let vd = !g_ataken_vd
    if vd>=4 then vprintln 4 (sprintf "record a numeric_tie mirking %i for %s" baser (mdToStr rst))
    ()

let render_numeric_ties() =
    let rr = ref []
    let fol kk vv =
        //dev_println(sprintf "kill ties memdesc numeric tie %A" vv)
        if length vv > 1 then mutadd rr ([], Memdesc_tie vv)
        ()
    for z in g_numeric_ties do fol z.Key z.Value done
    !rr

// format strings: infer a printf escape code from a datatype.
let format_escape_mine ce =
    let dt = get_ce_type "format_escape_mine" ce
    //dev_println (sprintf "format_escape_mine: dt=%s %s" (cil_typeToStr dt) (ceToStr ce))
    let vvf deflt x =
        match x with
            // we did get arrays here string arrays here ... (test14b and parallel convolver) 
            | CT_arr(ct, _) when ct = g_canned_char -> "%s"
            | CTL_net(volf, width, signed, nats) ->
                  if nats.nat_char then "%c"
                  elif nats.nat_string then "%s"
                  elif signed=FloatingPoint then "%f"
                  elif signed=Signed then "%d" else "%u"

            | dt -> (vprintln 1 (sprintf "+++ other type in printf format character type determination: exp=%s type=%s" (ceToStr ce) (typeinfoToStr x)  + " using " + deflt); deflt)
    vvf "%d" dt

let remove_corresponding_star msg nx l0 =
    if nx=0 then l0
    else
        match l0 with
            | CE_star(ny, larg, _) ->
                  if nx=ny then larg
                  else
                      dev_println (sprintf "not removing correct number of stars %i %i" nx ny)
                      larg
            | l0 ->
                dev_println (sprintf "did not remove stars %i from %s" nx (ceToStr l0))
                l0




//
let rec arglower mf g ll lst =
    //if vd>=3 then vprintln 3 (sprintf "arglower invoker %A" g)
    let style_sel = function
        | [arg] ->  [ g_autoformat; ll arg]
        | hh::tt -> printf_ansi_form mf ("g_autoformat" :: (map format_escape_mine tt), List.zip (hh::tt) (map ll (hh::tt)))
    if hd g = "WriteLine" || hd g = "Write" // These are functions that may need autoformat escape
    then
        style_sel lst
    else 
        let lst' = map ll lst
        lst'

// A CE_conv can be applied to a constant and fully dispatched.  The constant will be in the CE_x form and the attached type in the result will be the type converted to.
// For variable expressions, certain CE_conv pairs can be telescoped.
// The following rules are not highly-relevant just here since precision_compose in HPR library will implement them.
//     A pair of unsigned conversions can be telescoped into one that is the minimal width in terms of data operations, but the result width is the returned type.
//    
//     Likewise, a pair of signed conversions can also be telescoped to one of the minimal width.
//     The above two are commutative since 'min' is.
//
//     A pair that convert between signed and unsigned and which are the same width can be telescoped to just the outer conversion.

//     A convert to unsigned applied to a convert to unsigned of a larger width is just the outer unsigned conversion.
//     All other converts between signed and unsigned cannot be telescoped.
        
//     A convert to double applied to a convert to single becomes just a convert to double.
//     A convert from an int type to a float and then back to an int type can 'reasonably' ignore the intermediate float since no sane design might rely on such behaviour.
and yan_ce_conv ww cCP mf0 dt cvtf ce =
    let vd = 2
    let msg = "yan_ce_conv: " + mf0()
    //let src_dt = get_ce_type mx ce
    let okptr = function
        | CTL_net(volf, width, signed, flags) -> true
        | other ->
            let _ = vprintln 0  ("+++ okptr ignored on " + typeinfoToStr other)
            false

    let mf () = sprintf "type cvt %s %s %s" (ceToStr ce) (cil_typeToStr dt) (mf0())
    let ll = nlower ww cCP "yan_ce_conv" mf
    
    let ans = 
      match (dt, ce) with
        | (_, CE_x(orig_dt, hexp)) ->
            let (l_newtype, l_orig_dt) = (lower_type ww msg dt, lower_type ww msg orig_dt)
            let ans = ix_casting l_orig_dt.widtho l_newtype cvtf hexp
            gec_yb(ans, dt)

        | (_, CE_conv(dt1, cvtf1, ce')) when cvtf=cvtf1 && dt=dt1 -> ce // nop convert to self.
        
        | (CTL_net(volf, width, signed, flags), CE_star(stars, ce', _)) ->
            if stars > 0 && okptr dt then ce
            elif stars > 0 then sf (typeinfoToStr dt + ": cannot convert a pointer type (yan_ce_conv)")
            else
                let hexp = ll ce
                let (l_newtype, l_orig_dt) = (lower_type ww msg dt, lower_type ww msg (get_ce_type msg ce))
                let ans = ix_casting l_orig_dt.widtho l_newtype cvtf hexp
                gec_yb(ans, dt)

                          
        | (CTL_net(volf, width, signed, flags), ce) ->  // so many special cases? they are not special. please simplify now.
            //gec_yb(conv_guts dt (ll ce), dt)
            let hexp = ll ce
            let (l_newtype, l_orig_dt) = (lower_type ww msg dt, lower_type ww msg (get_ce_type msg ce))
            let ans = ix_casting l_orig_dt.widtho l_newtype cvtf hexp
            gec_yb(ans, dt)


// inappropriate: exception: ce=CE_region(system/threading/thread/ii12%system/threading/thread%400012%8, CT_arr(class(system/threading/thread, ...), <unspec>), {cid=thread.threading.system, marker=wondtoken, constant=true}): inappropriate type cvt to: &(ctl_record(system/threading/5athread, entryp at pos=4 size=4:&(ctl_record(system/threading/threadstart, object at pos=4 size=4:ctl_object, method at pos=8 size=4:ctl_object, binds^0=<None>)), binds^0=<None>))

// +++ inappropriate type cvt: ce=&(CE_struct_SID12<CTL_struct(cac_posit, ...)>^2((T402.cac_posit.repack.5.16.V_10.prec)/prec, (T402.cac_posit.repack.5.16.V_10.vale)/vale), {[SID12]_{cac_posit.repack.CZ:35:4}}), dt=&(CT_structsite(cac_posit))


        | (CT_star(1, CT_cr crt), CE_star(nn, CE_struct(idtoken, dt, aid, items), _)) when abs nn = 1 (* && is_crt_struct crt *) ->  // Cast of a struct reference to a struct reference is ok provided crt is of structsite form

            if is_structsite "L2046" (CT_cr crt) then
                let equal_anyway = generic_tyeq (*vd*)20 mf dt (CT_cr crt)
                if equal_anyway then ce
                else muddy (sprintf "Unexpected struct type cast : PANK2048 eq=%A  %s" equal_anyway (mf()))
            else sf (sprintf "inappropriate type cvt L2048" + mf())

//      | (CT_star(1, CTL_record(idl, cst, lst, len, ats, binds, dd)), CE_star(nn, CE_struct(idtoken, dt, aid, items), _)) when cst.structf && abs nn = 1 ->  // Cast of a struct reference to a struct reference - is ok? upcast?

       
        | (CT_star(1, CTL_record(idl, cst, lst, len, ats, binds, dd)), CE_region(uid, dt__, ats', nemtok, a)) ->  // Cast of a region
            CE_region(uid, CTL_record(idl, cst, lst, len, ats, binds, dd), ats', nemtok, a)

        | (other, ce) ->
            let _ = vprintln 0 (sprintf " +++ inappropriate type cvt: ce=%s, dt=" (ceToStr ce) + typeinfoToStr other)
            //gec_yb(conv_guts dt (ll ce), dt)
            let hexp = ll ce
            let (l_newtype, l_orig_dt) = (lower_type ww msg dt, lower_type ww msg (get_ce_type msg ce))
            let ans = ix_casting l_orig_dt.widtho l_newtype cvtf hexp
            gec_yb(ans, dt)
    //let _ = vprintln 0 (sprintf "yan_ce_conv: %s  ---conv---> %s" (ceToStr ce) (ceToStr ans))
    ans
     
and nlower ww (*here*)cCP  site (mf:unit -> string) (x:cil_eval_t) =
    let _:cilpass_t = (*here*)cCP 
    let vd = 2
    let ll = nlower ww (*here*)cCP  site mf

    let hpr_native_s (bif:kiwi_bif_fn_t) arity = function
        | _ when not_nonep bif.hpr_native -> (hd bif.uname, valOf bif.hpr_native)
        | "<hardcoded>" -> sf(sprintf "A built-in method, arity %i, should have been trapped within KiwiC front end (not passed to later recipe stages): uname=%s" arity (htos bif.uname))
        | ss -> hpr_native ss


    let hpr_native0 bif = hpr_native_s bif 0 bif.hpr_name

    let hpr_native1 (bif:kiwi_bif_fn_t) =
        match bif.uname with
            //| "<hardcoded-kppmark>"    -> hpr_native_s "hpr_kppmark"
               
            | other -> hpr_native_s bif 1 bif.hpr_name


    let hpr_native2 (bif:kiwi_bif_fn_t) = hpr_native_s bif 2 bif.hpr_name
    let hpr_native3 bif = hpr_native_s bif 3 bif.hpr_name
    let hpr_native4 bif = hpr_native_s bif 4 bif.hpr_name
    let hpr_native5 bif = hpr_native_s bif 5 bif.hpr_name
    let hpr_native6 bif = hpr_native_s bif 6 bif.hpr_name    

    let rec lower_appcall lmodef = function
        // The eval of args is done in keval. Here we translate calls that are mapped into hexp code as calls.
        | CE_apply(CEF_bif g, cf, []) -> xi_apply_cf cf (hpr_native0 g, []) 

        | CE_apply(CEF_bif g, cf, [a1]) ->
            match g.uname with
                | [ ".ctor"; "Object"; "System" ] -> X_undef // no work to be done
                | [ "get_Name"; "MemberInfo"; "Reflection"; "System"; ] ->
                    let ss = cil_typeToStr (get_ce_type (hptos g.uname) a1)
                    if vd>=3 then vprintln 3 (sprintf "Reflection API get_Name %s" ss)
                    xi_string ss

                | uname ->
                     if hd uname = "abs" then ix_abs(hd(arglower mf g.uname (lower1 "appcall-abs") [a1]))
                     else xi_apply_cf cf (hpr_native1 g, arglower mf g.uname (lower1 "appcall") [a1])

        | CE_apply(CEF_bif g, _, [a1; a2]) when hd g.uname = "max" || hd g.uname = "min"  -> 
             let args' = arglower mf g.uname (lower1 "max/min") [a1; a2]
             (if hd g.uname = "max" then xi_max else xi_min) (hd args', hd (tl args'))

        | CE_apply(CEF_mdt mdt, cf, iobj::args) when not_nonep cf.tlm_call ->
            let args' = arglower mf [mdt.uname] (lower1 mdt.uname) args
            let fu_name =
                match mdt.fu_arg with
                    | Some fu_name -> fu_name
                    | _ -> sf ("L1998 missing fu_name" + mdt.uname)
            let dt = mdt.rtype
            let sri_token = valOf_or_fail "L2000" mdt.fu_arg 
            let prec = lower_type ww mdt.uname dt
            //dev_println (sprintf "BDAY: TLM call %s fu_name=%A fsems=%A" (mdt.uname) fu_name mdt.fsems)
            let (fname_, gis) = gec_generic_sri_tlm_fgis prec sri_token fu_name mdt.fsems lmodef // ... not very clean
            let gis = { gis with rv=prec; args=[g_reflection_token_prec; (*....*)]; outargs="b-" }
            //let aobj_ = record_shared_resource_info ww (*here*)cCP  [fu_name] dt iobj
            let cf = {cf with tlm_call=Some true}
            let fname = hd mdt.name
            let fname = if fname = "read/write" then "read" else fname
            let xobj = gec_X_net (hptos sri_token)
            xi_apply_cf cf ((fname, gis), xobj::args')

        | CE_apply(CEF_bif g, cf, [a1; a2])             -> xi_apply_cf cf (hpr_native2 g, arglower mf g.uname (lower1 "generic-apply3") [a1; a2])

        | CE_apply(CEF_bif g, cf, [a1; a2; a3])         -> xi_apply_cf cf (hpr_native3 g, arglower mf g.uname (lower1 "generic-apply3") [a1; a2; a3])

        | CE_apply(CEF_bif g, cf, [a1; a2; a3; a4])     -> xi_apply_cf cf (hpr_native4 g, arglower mf g.uname (lower1 "generic-apply4") [a1; a2; a3; a4])
        
        | CE_apply(CEF_bif g, cf, [a1; a2; a3; a4; a5]) -> xi_apply_cf cf (hpr_native5 g, arglower mf g.uname (lower1 "generic-apply5") [a1; a2; a3; a4; a5])       

        | CE_apply(CEF_bif g, cf, [a1; a2; a3; a4; a5; a6]) -> xi_apply_cf cf (hpr_native5 g, arglower mf g.uname (lower1 "generic-apply6") [a1; a2; a3; a4; a5; a6])       

        
        | other -> sf("cil appcall lower1 other: (fcall max arg arity is 6)" + ceToStr other)

 
    and lower1 site ce =
        if vd>=5 then vprintln 5 (sprintf "lower1 start " + ceToStr ce)
        match ce with
       
        | CE_struct(idtoken, dt, aid, items) -> // Lower the whole immediate (anon) struct by packing it to a large bitfield.
            let fields = 
                match grossly_simplify_type dt with // grossly cf wantonly
                    | CTL_record(idl, cst, lst, len, ats, binds, _) ->
                        let _ = if length lst <> length items then sf (sprintf "struct field pack - wrong number of fields %i cf %i" (length lst) (length items))
                        map (fun crf -> (crf.pos*8L, ct_bitstar ww "dot-struct-lower-site" 0 crf.dt)) lst
                    | other -> sf (sprintf "expected CTL_struct/CTL_record for struct field pack instead of %s" (cil_typeToStr other))
            let pairs = List.zip items fields
            //let (filled_fields, total_fields) = struct_filled_field_audit items
            let ans =
                let castop width x = ix_casting None ({g_default_prec with signed=Unsigned; widtho=Some width })  CS_typecast x                
                let master_width = dt_width_bits ww dt
                if vd>=3 then vprintln 3 (sprintf "Assemble struct %s width %i" (cil_typeToStr dt) (master_width))
                let assemble_struct cc = function
                    | ((tag, dt_, Some ce), (baser, width)) ->
                        let hx = lower1 (site + "-stuctitem") ce                    
                        ix_bitor cc (ix_lshift (castop (int32 master_width) hx) (xi_num64 baser)) // Lower1 - Pack fields for structure.
                    | ((tag, dt_, None), (baser, width)) ->
                        // This is a real error/warning on final render pass but ok during constant fold.
                        if vd>=3 then vprintln 3 (sprintf "lower1: cannot fully assemble struct %s owing to tag %s not having a value set up." (cil_typeToStr dt) tag)
                        cc
                List.fold assemble_struct (xi_bnum_n (int32 master_width) 0I) pairs

            ans

        | CE_x(_, v) -> v // lower1

        | CE_dot(dt_record, CE_star(1, lhs, _), (tag_no, fdt, ptag_utag, dt, tag_idx), aidl) ->
            lower_indexod ww cCP mf (CE_dot((dt_record, lhs, (tag_no, fdt, ptag_utag, dt, tag_idx), aidl)))

        | CE_ilit _ // these three lower the same in lmode and rmode
        | CE_subsc _
        | CE_dot _ -> lower_indexod ww cCP mf ce  

        | CE_star(starx, arg_ce, aid_b) -> // reference/dereference operator - lower1
            if starx = 0 then dev_println (sprintf "zero stars should never be generated. applied to %s" (ceToStr ce))
            // Note a star and a subscription (or vassign) of a wondarray are two forms for the same thing and intcil makes both at the moment.  There may be differences in aid tracking?
            let rec star_newlf ss ll =
                //dev_println (sprintf "lower1:star_newlf: star_newlf %i on %s" ss (ceToStr ll))
                // we must eval lhs using deval and also apply reference operators: the order is important
                //let _ = mutinc g_temp_count -1 // delete me
                //vprintln 3 (sprintf "g_temp_count consume %i" !g_temp_count)
                if ss = 0 then ll // return when all stars done.
                elif false (* !g_temp_count <= 0 *) then
                    //dev_println (site + sprintf ": lower1: star_newlf %i on %s" ss (ceToStr ll))
                    // TODO delete this error trap when cause is fixed
                    muddy ("KiwiC has entered a seemingly-infinite internal loop on the nlower star_newlf code (to do with references) " +   ceToStr ce)
                elif ss > 0 then // +ve ss is address-of operator
                    if purely_symbolic_under_reference "reference_valuetype" ll then sf (mf() + sprintf ": purely symbolic form should not be lowered stars=%i, top_arg=" ss + ceToStr ce)
                    match ll with
                        | CE_struct _ when false -> // No, ldloca of structs can mean spillvars have struct references in them and these need to be respected.
                            if vd>=3 then vprintln 3 (sprintf "Kludge for now: suppress further reference (%i) of Cstruct form (that likely should not have been rezzed)" ss + ceToStr ll)
                            ll
                        | ll -> 
                            let ll' = reference_valuetype (sitemarkerf aid_b) ll
                            if vd>=3 then vprintln 3 (site + sprintf ": lower1: reference star_newlf +ve result is %s" (ceToStr ll'))
                            star_newlf (ss-1) ll'
                else // normal dereference operator (asterisk in C), denoted with a negative star count. 
                    let msg = site + ": lower1: star operator: dereference "  
                    let ce2 = 
                       match get_ce_type msg ll with
                           | ((CT_arr _) as dt)->  // asterisk applied to an array (gcc4cil), gets zeroth location.
                               gec_CE_subsc(dt, ll, gec_yb(xmy_num 0, g_canned_i32), (Some aid_b)) 
                           | _ -> // The general clause below should work just as well - but this has a more-precise array length?
                               match arg_ce with
                                   | CE_alu(result_dt, V_plus, a_l, a_r, aidl) -> // Reverse engineer pointer arithmetic back into a sanitary array subscription. - This code is elsehwere - rationalise please.
                                        let find_a_base msg = function
                                            | CE_conv(_, _, CE_star(1, vale, _))
                                            | CE_star(1, vale, _)                   -> Some (get_ce_type msg vale)
                                            | _                                     -> None
                                        let lto = find_a_base "reveng-star-l" a_l 
                                        let rto = find_a_base "reveng-star-r" a_r 
                                        let rec scorer = function
                                            | CT_star(sc, b) -> (abs sc) * 1000 + scorer b
                                            | CTL_net(volf, width, signed, nats) -> if not_nonep nats.nat_IntPtr then 1 else 0
                                            | _ -> 0
                                        let msg = site + ": lower1:reveng-star"

                                        let ans =
                                            match (lto, rto) with
                                                // NOTE this may be discarding length when we could pass it in
                                                | (Some bt, None) -> gec_CE_subsc(CT_arr(bt, None), a_l, a_r, Some aid_b)
                                                | (None, Some bt) -> gec_CE_subsc(CT_arr(bt, None), a_r, a_l, Some aid_b)
                                                
                                                | (None, None) ->
                                                    let lt = get_ce_type "reveng-star-l" a_l
                                                    let rt = get_ce_type "reveng-star-r" a_r
                                                    match (scorer lt, scorer rt) with
                                                        | (ls, rs) when ls > rs ->
                                                            let adt = CT_arr(dereference_ct "reveng-star-gl" a_r lt, None)
                                                            gec_CE_subsc(adt, a_l, a_r, Some aid_b)
                                                        | (ls, rs) when ls < rs ->
                                                            let adt = CT_arr(dereference_ct "reveng-star-gr" a_l rt, None)
                                                            gec_CE_subsc(adt, a_r, a_l, Some aid_b)
                                                        | (ls, rs) ->  sf (msg + sprintf  " dive no sides ls=%i rs=%i %s %s" ls rs (ceToStr a_l) (ceToStr a_r))
                                                | (_, _) -> sf (msg + ": dived both sides")
                                                //(msg + sprintf ": DIVE BOTH SIDES \n    ls=%i L=%s LT=%s\n    rs=%i R=%s    RT=%s" ls (cil_typeToStr lt) (ceToStr a_l) rs (cil_typeToStr rt) (ceToStr a_r))

                                        //muddy (sprintf "trap %s     %s  " (cil_typeToStr(get_ce_type "reveng-subsc-l" a_l)) (cil_typeToStr(get_ce_type "reveng-subsc-r" a_r)))
                                        if vd>=3 then vprintln 3 (msg + sprintf ": form2 subsc-revenge:  ss=%i - l0=%s ans=%s" ss (ceToStr ce) (ceToStr ans))
                                        ans
                                   | arg_ce -> // Cannot reveng adequately
                                       let ce' = gec_CE_star cilnorm_sitemarker (ss+1) ll
                                       let dt = remove_CT_star msg (get_ce_type "star_newlf-lower1-dereference-l2054" ce')
                                       // idiomatic wondtoken multi-copied code - please tidy - TODO edited since copied with the remove_CT_star
                                       let uid = { f_name=[]; f_dtidl=[]; baser=0L; length=None; } // this uid is meaningless and we do not have a nemtok yet either. all that is known is the type. repack will sort it out.
                                       // We stop it being scaled with rnsc attribute.
                                       let lhs = gec_CE_Region ww (*rnsc=*)true (None, dt, uid, [], None)
                                       let ce2 = gec_CE_subsc(CT_arr(dt, None), lhs, ce', Some aid_b)
                                       vprintln 3 (msg + sprintf ": form3 dt=%s ss=%i - l0=%s ce2=%s" (cil_typeToStr dt) ss (ceToStr ce) (ceToStr ce2))
                                       ce2
                    star_newlf (ss+1) ce2 // recurse in case of further stars (ss was less than -1)
                    //sf (mf() + sprintf ": lower1: negative stars: dt=%s ss=%i - l0=%s" (cil_typeToStr dt) ss (ceToStr ce))

            let arg_ce'= star_newlf starx arg_ce
            //dev_println (sprintf "start: star_newlf stars=%i  %s -> %s" starx (ceToStr ce) (ceToStr arg_ce'))
            lower1 (site + "-again;") arg_ce' // Go round again.

        | CE_region(uid, dt, ats, nemtoko, _) -> 
            let site = "lower1 region"
            if not_nonep (op_assoc g_wondtoken_marker ats)
            then 
                let l = op_assoc "bytes" ats
                let b = xi_int64 uid.baser
                if nonep nemtoko then b
                else
                    let mirking =
                        let rst = Memdesc_scs((get_current_sc site (A_loaf(valOf nemtoko))))
                        tiegen_numeric_mirking uid.baser rst
                        xi_stringx (XS_sc[rst]) (mdToStr rst) // TODO abstract this rez copy 2/2
                    ix_pair mirking b 
            else ceToNet ww "lower1 av" false ce

        | CE_var(hidx, dt, idl, fdto, ats)  ->
            if not_nonep (op_assoc g_wondtoken_marker ats) then sf "no hidx in a uav"
            else ceToNet ww "lower1 uav l2021" false ce



        | CE_scond(dop, a1, a2) ->
            let ll = lower1 "CE_scond"
            let ans = xi_blift(ix_bdiop dop [ll a1; ll a2] false)
            ans

        | CE_alu(result_dt, diop, a1, a2, aidl) ->
            let msg = "ce_alu:L2110"
            let ll = lower1 msg
            let e2fn_a diop l r =
                //let _ = vprintln 0 (sprintf "lower1 ce_alu oo=%A l=%s r=%s" diop (xToStr l) (xToStr r))                
                let rv =
                    if xi_iszero r && (diop=V_mod || diop=V_divide) then X_undef
                    else ix_diop (result_dt_to_prec ww msg result_dt) diop l r // Masking here for overflow is needed? And trapped forms too. Please test this.
                //let _ = vprintln 0 (sprintf "lower1 ce_alu ce=%s lower1 returns=%s" (ceToStr ce) (xToStr rv))
                rv
            e2fn_a diop (ll a1) (ll a2)

        // The eval of args is done in keval, but here we apply some of those that generate hexp code.
        | CE_apply(CEF_bif g, cf, args) -> // lower site - we need the gtrace site as the main one.
            match g.uname with
                |  "kpragma"::_ -> // this is now expected to be in k_pragma form
                    vprintln 0 (mf() + sprintf ": kpragma called in unexpected form. args=%A" args)
                    xi_num 0
                        
                | _ -> lower_appcall false ce

        | CE_apply(CEF_mdt g, cf, args) -> lower_appcall false ce

        | CE_conv(ct, cvtf_, ce) -> // lower1 site - conv becomes a mask here, but yan and do lower the conv to hexp w_convert form. TODO explain why different.
        
            let r0 = lower1 "conv" ce
// cf yan_ce_conv that also lowers
            match ct with
                | CTL_net(volf, bit_width, signed, flags) -> ix_mask signed bit_width r0 // no sex? todo crude.
                | ct when ct=g_ctl_object_ref -> r0 // ignore, for now, converts to object. (ditto other upcasts ?)
                | ct ->
                    if vd>=3 then vprintln 3 (sprintf "lower1: ignored ce_conv %A of %s to %s" cvtf_ (xToStr r0) (typeinfoToStr ct))
                    r0
        | CE_ternary(_, g, aat, aaf) ->
            let gg = (ll g)
            let ans = ix_query (xi_orred gg) (ll aat) (ll aaf)
            let isvar =
                match aat with
                    | CE_var _ -> true
                    | _ -> false
            if vd>=3 then vprintln 3 (sprintf "ternary var=%A larg=%s" isvar (ceToStr aat))
            if vd>=3 then vprintln 3 (sprintf "ternary lower g=%s to %s" (xToStr gg) (xToStr ans))
            ans

        | CE_reflection(_, Some dt, None, _, _)  -> xi_string ("CX_reflection_dt_" + cil_typeToStr dt)

        | CE_reflection(_, None, Some idl, valo, _) ->
            match valo with
                | Some nn ->
                    xi_stringx (XS_withval(xi_num nn)) ("CX_reflection_idl_" + hptos idl)        
                | None ->
                    //dev_println (sprintf "Lower reflection token without integer enumeration value. idl=" + hptos idl)
                    xi_string ("CX_reflection_idl_novalo_" + hptos idl)        

        | other -> sf("cil lower1 other: " + ceToStr other + "\n")

    if vd>=5 then vprintln 5 (site + ":  nlower1 trc " + ceToStr x + " start")
    let rr = lower1 site x
    if vd>=5 then vprintln 5 (site + ":  nlower1 trc " + ceToStr x + " trc-gave " + xToStr rr)
    rr

               
// all of the lowering functions convert from ce to hexp representation, discarding most kiwi-specific information, but some
// is retained and passed down the recipe as pair annotations or as memdesc records.    
//   
// lowering an array, class or other object pointer is here called indexing.
// there are several forms of indexing to be lowered :
//  1. dots:    record field lookup by tag, and 
//  2. subcs:   array subscription
//  3. pure-symbolics - where an address-of operator is later processed by a stind ot stobj instruction.
and lower_indexod ww (*here*)cCP  mf ce =
    let b = lower_indexed_ntr ww (*here*)cCP  (fun () -> mf() + " work call on ") ce
    //if vd>=5 then vprintln 5 (mf() + ": lower_indexed invoked " + ceToStr ce + " ---> " + xToStr b)
    b


and lower_indexed_ntr ww (*here*)cCP  (mf:unit->string) ce =
    let vd = -1
    //let ww = WF  3 "lower_indexed" ww (mf() + " of " + ceToStr ce)
    match ce with
    | CE_dot(dt_record, lhs, (tag_no, fdt, (ptag_utag), dt, tag_idx), aid_o) -> 
        let msg = sprintf "lower_indexed: .CE_dot tag=%A "  ptag_utag 
        match is_struct_field_op msg ce with
            | Some(packedf, nn, ptag, l0, dt_struct) -> // A field insert.
                if packedf then // If a packed struct only, then do packed field extract.
                    vprintln 3 (sprintf "SEVL lower_indexed packed struct yes")
                    //vprintln 3 ("SEVL we process a dot tag applied to a struct as a field extract, not as a subscription")
                    match lhs with
                        | CE_struct _ -> muddy "SELV shortcut possible (*here*)cCP " // TODO finish this code or else remove this clause
                        | other when nn=0 ->
                            vprintln 3 (sprintf "lower direct of nn=%i l0=%s" nn (ceToStr l0))
                            let lhs_x = nlower ww cCP "dot-struct-lower-direct-site" mf l0
                            let (bit_offset, width) =
                                match grossly_simplify_type dt_record with
                                    | CTL_record(idl, cst, lst, len, ats, binds, _) ->
                                        let ptag = fst ptag_utag
                                        let rec scans = function
                                            | crf :: tt when crf.ptag=ptag -> (crf.pos*8L, ct_bitstar ww "dot-struct-lower-site" 0 crf.dt)
                                            | crf :: tt -> scans tt
                                            | [] -> sf "field %s not found"

                                        scans lst
                                    | other -> sf (sprintf "expected CTL_record for struct field extract instead of %s" (cil_typeToStr other))
                            let ans = (ix_rshift Unsigned lhs_x (xi_num64 bit_offset)) // Field extract from struct.
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-direct lower of field %s %s extract from packed struct (%i, %i) yields pre mask %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            let ans = ix_mask Unsigned (int width) ans
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-direct lower of field %s %s extract from packed struct (%i, %i) yields %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            let ans = ix_casting (Some 32) (lower_type ww msg dt) CS_typecast ans
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-direct lower of field %s %s extract from packed struct (%i, %i) yields after cast %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            ans

                        | other when nn <> 0 ->
                            if vd>=3 then vprintln 3 (sprintf "lower of nn=%i l0=%s" nn (ceToStr l0))

                            let l0 = remove_corresponding_star msg nn l0
                            let lhs_x = nlower ww (*here*)cCP  "dot-struct-lower-indirect-site" mf l0 // (fatal newlf loop mistake to lower with star still present)
                            let (bit_offset, width) =
                                match grossly_simplify_type dt_record with
                                    | CTL_record(idl, cst, lst, len, ats, binds, _) ->
                                        let ptag = fst ptag_utag
                                        let rec scans = function
                                            | crf :: tt when crf.ptag=ptag -> (crf.pos*8L, ct_bitstar ww "dot-struct-lower-site" 0 crf.dt)
                                            | crf :: tt -> scans tt
                                            | [] -> sf "field %s not found"

                                        scans lst
                                    | other -> sf (sprintf "expected CTL_record for struct field extract instead of %s" (cil_typeToStr other))
                            let ans = (ix_rshift Unsigned lhs_x (xi_num64 bit_offset)) // Field extract from struct.
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-indirect lower of field %s %s extract from packed struct (%i, %i) yields pre mask %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            let ans = ix_mask Unsigned (int width) ans
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-indirect lower of field %s %s extract from packed struct (%i, %i) yields %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            let ans = ix_casting (Some 32) (lower_type ww msg dt) CS_typecast ans
                            //if vd>=3 then vprintln 3 (sprintf "SEVL-indirect lower of field %s %s extract from packed struct (%i, %i) yields after cast %s" (fst ptag_utag) (snd ptag_utag) bit_offset width (xToStr ans))
                            ans
                else muddy "lower1 struct unpacked SEVL"

            | None ->
                //if vd>=3 then vprintln 3 " not pasta L2390"
                //let ww = WF  3 "lower_indexed" ww msg
                    // Transpose clause for singleton forms 
                    let (a_ce, a_x) = mk_wondarray ww "lower_indexed dot" dt
                    // This is a sort of 'transpose' lower where an array of objects/records is lowered as
                    // a record of arrays.  This means valuetype arrays are indexed directy as follows
                    // for instance ap[x].t becomes wond(typeof(t))[ap + x*sizeof(contenttypeof(ap)) + offsetof(t)]

                    // Note that a transpose here should be matched with an l-mode equivalent if this code not used?  the kcode vassign form requires all such transposes take place infact.  a similar case arises for assigns to arrays of arrays infact.

                    // This is not the clause intended to be exercised by the array within test14x since the manifest array is a field itself, but because there are pointer twiddles in that test
                    // we essentially have an array of records where one field itself is also an array.

                    let ntl = oapp (inca_for_vassign "L2194" cilnorm_sitemarker) aid_o //pass the subscript aid to o_tag, not the field content aid.
                    let base_ptr = nlower ww (*here*)cCP  "general-subs" mf lhs
                    if vd>=4 then vprintln 4 (msg + sprintf ":  nc=%s idx=%i" (aidoToStr ntl) tag_idx)
                    let idx = gec_O_tag mf (ntl, base_ptr, (tag_no, ptag_utag, dt, tag_idx))
                    general_subscription_lowerer ww msg (a_ce, a_x, idx)

    // Special clauses for extracting a char from a lowered ASCII string. : used in test9.
    | CE_subsc(_,  CE_x(_, W_string(s, f, me)), idx, _) -> 
        let ww = WF  3 "lower_indexed" ww "CE_subsc c2: string special case: uses xi_asubsc for real work"
        let rng = None // ix_pair (xi_num g_unaddressable) (xi_num (string.length s))
        let idx' = nlower ww (*here*)cCP  "ce-subsc-string" mf idx
        vprintln 3 "+++ this form of string handling not being used anymore? yes it is in test9.cs"
        ix_asubsc (W_string(s, f, me)) idx'

    
    | CE_subsc(CT_star(1, CT_arr(cdt, len)), lhs, idx, aid_o) // TODO clean up this ambivalence over stars in the type.
    | CE_subsc(CT_arr(cdt, len), lhs, idx, aid_o) ->         
        let rnsc = mine_rnsc lhs // really_noscale
        let mm = sprintf "CE_subsc clause-3: general array: uses general_subscription_lowerer for real work. rnsc=%A" rnsc
        let ww = WF  3 "lower_indexed" ww mm
        let adt__ = CT_star(1, CT_arr(cdt, len)) // an as clause - inserted the star if missing - no longer used.
        let base_ptr = nlower (WN "subsie" ww) (*here*)cCP  mm mf lhs
        let (a_ce, a_x) = mk_wondarray (WN "lower_indexed subsc l1779" ww) "lower_indexed subsc" cdt
        let idx' = nlower ww (*here*)cCP  mm mf idx
        vprintln 3 (mm + sprintf " rnsc really noscale=%A" rnsc)
        let scaler =
            if rnsc then 1L
            else
                let vt = ct_is_valuetype cdt
                let scaler = if vt then ct_bytes1 ww mm cdt else int64 g_pointer_size 
                if vd>=4 then vprintln 4 ("lowed_indexed CE_subsc c3: scaler=" + i2s64 scaler +  " for an array containing " + (if vt then " valuetypes " else " reference types ") + typeinfoToStr cdt)
                cassert(scaler <> 0L, "scalar <> 0")
                scaler
        //Tthe cached aids describe the result of the CE_subsc expression so need incrementing (deleting an a_ind) for the base and offset expression storage classes.
        let aid_o1 = oapp (inca_for_vassign mm (cilnorm_sitemarker + "O2291")) aid_o
        //dev_println (sprintf "%s:  aid presl=%s  %s" m (sfold aidToStr aid_b) (sfold aidToStr aid_o))
        //dev_println (sprintf "%s:  aid final=%s  %s" m (sfold aidToStr aid_b1) (sfold aidToStr aid_o1))        

        let idx = gec_O_subs mf (aid_o, base_ptr, valOf_or len 0L, wt_times(idx', xi_int64 scaler))
        general_subscription_lowerer ww mm (a_ce, a_x, idx)
        
    | other -> sf("cil lower_indexod other: " + ceToStr other + "\n")


//
// envlook is the main env lookup, often called via keval_new.
// For strongly-typed input languages, like safe c#, we could rely on dtidl and nemtok to each partition the address space, but for more general code, such as unsafe c# where the address of a local variable is cast to an integer or unaligned access to words and structs, we need to fully respect the numerical address and ignore higher-level formalisms.  Hence we need to efficiently support memory as a sparse dictionary of bytes.
and envlook cCC m_shared_var_fails env thrd_callsite dt origx nemtok bb ats = 
    let thrd_id:tid3_t = fst thrd_callsite
    let vd = -1
    let rec scan_volatile = function // Net-level inputs, RTL parameters (really?) and remotely-written shared vars are volatile.
        | [] -> false
        | ("rtl_parameter", _)::tt
        | ("io_input", _)::tt -> true
        | (key, vale)::tt when key=g_volat ->
            if vd>=4 then vprintln 4 (sprintf "    volatile so no envlook " + ceToStr origx)
            true        
        | _::tt -> scan_volatile tt

    // do this first ?
    let nonvol_note () =  // Record inter-thread shared variable use.
        match origx with
            | CE_x(ce_, X_bnet ff) -> // Could go round again on the ce_ field. Cleaner.
                let nemtok_details = cCC.kcode_globals.nemtok_details
                rhs_constvol_noter2 m_shared_var_fails thrd_id nemtok [ff.id]

            | CE_var(vrn, dt, idl, fdto, ats) -> 
                rhs_constvol_noter2 m_shared_var_fails thrd_id nemtok idl

            | CE_subsc _  // Non-singleton forms
            | CE_dot _ ->
                false
                
            | other ->
                hpr_yikes ("envlook: g-ignored other form " + ceToStr other)
                false
                
    if scan_volatile ats then origx // Net-level inputs and RTL parameters are volatile.
    else 
        let memtok = nemtok
        let ov = env.mainenv.TryFind memtok
        let dt (idl, x) = if vd>=4 then vprintln 4 (hptos idl + " in env as " + ceTreeToStr 4 x)
        //if (fst env) = [] then vprintln 3 "empty env on that idl" else (app dt (fst env); vprintln 3 "was full tree on that idl")    
        if vd>=4 then vprintln 4 (sprintf "   envlook memtok=%s bb=%i env_value=" memtok bb + (if ov=None then "no-tree-stored" else env_itemToStr 3 (valOf ov)))
        let rec ovf vv =
            match vv with 
                | CEE_vector(_, vv) ->
                    let tree_ans = ce_tree_walk (n64_lift bb) vv
                    if vd<=10 then vprintln 3 ("    vector tree_ans=" + (if nonep tree_ans then "none" else env_itemToStr 1 (valOf tree_ans)))
                    match tree_ans with
                        | Some other -> ovf other //
                        | None -> None
                | CEE_scalar(_, x) ->
                    if vd>=4 then vprintln 4 (sprintf "    scalar tree_ans=%s" (ceToStr x))
                    Some x
                | CEE_zombie ss ->
                    // Ideally we need to return X_under or originx depending on whether constant meeting or code generating respectively.
                    // But returning origx will be non-constant and should serve accordingly.
                    if vd>=4 then vprintln 4 (sprintf "    zombie tree_ans=None %s was in env as dead value '%s'. Using origx" memtok ss)
                    None

                | CEE_dyn ->
                    if vd>=4 then vprintln 4 (sprintf "    dyn tree_ans=CEE_dyn")
                    None

                | other ->
                    if vd>=4 then printenv 4 10 "ee-ee" env
                    sf ("evalof: tag=" + memtok + " form unexpected. Not " + env_itemToStr 3 other + "\nce=" + ceToStr origx)

        match ov with
            | None ->
                if vd>=4 then vprintln 4 ("envlook-notfound - returning symbolic origx")
                origx
            | Some vv -> 
                match ovf vv with
                    | None -> origx
                    | Some vx ->
                        if nonvol_note () then
                            if vd>=4 then vprintln 4 (sprintf "Discard env value owing to volatile inference of %s.  Returning symbolic original." memtok)
                            origx  // We note that we relied on it being nonvolatile and also if it is so denoted we supress value from env anyway.
                        else vx


type kcode_eval_reason_t =
    | KER_constant_meet
    | KER_lasso_stem
    | KER_code_generation





let process_pragma ww reason mf site (cCP:cilpass_t) bpc count (fatalf, cmd, a0, a1) =
    let temp = false // reason = KER_constant_meet
    //Kiwi.KPragma with first argument as Boolean true can be used to conditionally abend elaboration or to log user progress messages during elaboration.
    //Kiwi.KPragma calls present in run-time loops can be emitted at runtime using the Console.WriteLine mechanisms.
    //Kiwi.KPragma calls with magic string values will be used to instruct the compiler, but no magic words are currently implemented.
    let msg = mf() + sprintf ": Kiwi KPragma: tid=%s reason=%A site=%s process_pragma. cmd=%A" cCP.tid3.id reason site cmd

    let action =
        match cmd with
            | "_EndOfElaborate" ->
                if not_nonep !cCP.marked_end_of_elab then vprintln 0 (msg + sprintf ": +++ EndOfElaborate marker encountered twice in thread in blocks %i and %i" bpc (valOf !cCP.marked_end_of_elab))
                if reason <> KER_lasso_stem then vprintln 0 (msg + sprintf ": +++ EndOfElaborate marker encountered beyond end of lasso stem in basic block %i" bpc)
                vprintln 3 (msg + sprintf ": Seting end_of_elab marker to %i" bpc)
                cCP.marked_end_of_elab := Some bpc
                0
            | _ ->
                vprintln (if temp then 3 else 0) msg // Just echo other pragmas to the log for now.
                0
    if fatalf then cleanexit("KPragma fatal: "  + msg)
    ()


let g_dummy_realdo =
    let (opl1, opl2) = (ref [], ref [])
    Rdo_gtrace(opl1, opl2) // Used in contexts where we do not expect any code to be created - please refine type so that it traps any code emitted in this context TODO


// Scone is our name for combining the fields of a struct with an unindexable/singleton instance of it to get a group of unpacked variable, one per field, as opposed to using a CE_struct form.
// We cons the ptag to the idl to get the scone label.
// The kcode could be designed to work on pre or post sconed form. We chose post-sconed form. That is, Kiwife implements the scone transform during cil to kcode conversion, This means stages that process kcode see the fields _of singletons_ as separate scalars.  These stages must still perform field insert and extract of structs held in arrays an so on.
let sconer ww cCC cCBo msg l0 ptag = 
    let vd = 2
    let (idl, dt, ats) = 
        match discard_ce_conv msg l0 with
            | CE_var(vrn, dt, idl, fdto, ats)                -> (idl, dt, ats)
            | CE_star(_, CE_var(vrn, dt, idl, fdto, ats), _) -> (idl, dt, ats)
            | other -> sf (msg + sprintf ": scone: get_singleton_heap_id:  other form " + ceToStr other)
    let sheap_scone_id = hptos(ptag :: idl) // new_nemtok_t
    match cCC.heap.sheap1.lookup sheap_scone_id with
        | None ->
            //reportx 0 "static items found" (noitemToStr_untie true) r
            if vd>=3 then vprintln 3 (msg + sprintf ": sconer: late declaration of sheap_scone_id named %s dt=%s" (sheap_scone_id) (cil_typeToStr dt))
            // We aim now for this late code to never run since we invoke gec_singleton where needed at all earlier declaration points.n
            let atakenf:bool option = None // Providing None here means we do not know at this point.
            let ans = gec_singleton ww !g_gtrace_vd cCBo msg "sconer" ats g_dummy_realdo (fun ()->msg) "" idl dt None atakenf
            match cCC.heap.sheap1.lookup sheap_scone_id with
                | Some she -> she                    
                | None ->
                    sf (msg + sprintf ": sconer: missing late declaration of sheap_scone_id named %s dtype=%s" (sheap_scone_id) (cil_typeToStr dt))
        | Some she -> she


//
// Main kcode to hbev_t conversion function: Used three times: constant folding, lasso stem eleaboration and code generation.
// So runs with and without kcode generation.
//               
let rec kcode_lower_block ww reason vd mmsg cre (cCP:cilpass_t, cCL:cil_cl_textual_t, cCB:cilbind_t, gdp:gdp_t) apflag compiled (linepoint5_in, callsite:(tid3_t * callsite_t), topret_mode, epc) =
    let cCC = valOf !g_CCobject
    let ksc = cCC.kcode_globals.ksc
    let cCBo = Some cCB
    let m_linepoint = ref (KXLP5 linepoint5_in)
    let reason_str = sprintf "%A" reason
    let m0() = "QT-klb:" + reason_str + ":" + kxrToStr (!m_linepoint)
    let here = cCP
    let m_shared_var_fails = cCP.m_shared_var_fails
    let boring = function
       | Cili_lab _ -> true
       | other -> false
    let (count, region, env) = cre

    let storemode idl =
        match gdp.psef with // pre or post end of static elaboration ?
        | PSE_pre  -> STOREMODE_compiletime_heap
        | PSE_post ->
            if apflag then STOREMODE_compiletime_heap // Allocations within articulation blocks may be safely on the compile-time heap since intrinsically finite (for finite thread spawning), but may be repacked to run-time heap if sharing a storage class.
            elif cCC.allow_hpr_alloc then STOREMODE_runtime_heap
            else cleanexit(sprintf "Program requires dynamic heapspace for %s. Please adjust code so the total number needed is statically determinable or else set command line flag -kiwife-allow-hpr-alloc=enable which will lead to a run-time heap manager being included in the design." (hptos idl))

    let cgr0 = cCL.cm_generics
    let callstring = "LOWERBLOCK" :: cCL.cl_codefix // TODO append something ?


    let zeng_threadstart msg eval_f (ce, args, env) = 
        let nload msg xus ce id = 
            let m1 = ("Eval thread start: " + id)
            let ww = WF 3 "zeng_threadstart" ww m1
            //vprintln 0 m
            // long winded - make these lookups once only... in the record dictionary for example.
            let oc = id_lookupp ww xus Map.empty (fun()->("xus not found", 1))
            // This record should now be to hand as a dropping etc.
            let dt_record = cil_gen_dtable (WN ("class_find gen_dtable") ww) [] (RN_idl xus, valOf oc)
            let dct = (dt_record, CTL_reflection_handle ["type"])
            let ce = gec_field_snd_pass ww cilnorm_sitemarker "zeng_threadstart" dt_record ce id
            let vd = 10 // for noew
            if vd >= 2 then
                vprintln 2 ("zeng_thread_start: gec field ret " + ceToStr ce)                
                vprintln 2 ("df-eval of thread input " + ceToStr ce)
            let ans = eval_f ce
            if vd >=2 then vprintln 2 (sprintf "df-eval %s of thread return " msg + ceToStr ans) 
            ans

            
        if gdp.gen <> None then
            // We need to evaluate the fields in the threadstart at log time since
            // the same delegate will sometimes be reused (e.g. inside a loop) to start multiple threads with different args.
            let xus = rev  [ "System"; "Threading"; "Thread" ]
            let entryp = nload "entryp" xus ce "entryp"
            let m1 =  (sprintf "Logging start thread entry point=%s user_arg_arity=%i" (ceToStr entryp) (length args))
            let xus = rev  [ "System"; "Threading"; "ThreadStart" ]
            let method1 = nload "method" xus entryp "method"
            let object1 = nload "object" xus entryp "object"
            vprintln 2 ("Spawn/static-queue thread delegate  object=" + ceToStr object1 + "\n  method=" + ceToStr method1)
            let srcfile = "TODO-lookup src file from delegate context"
            let nn =
                        let m = structg_name method1
                        let o = structg_name object1
                        m @ list_subtract(o, m)
            let spawntoken = (mutinc cCP.sptok 1; cCP.tid3.id + sprintf "SPAWN%i" !cCP.sptok)
            let tT = USER_THREAD1(srcfile, object1, method1, spawntoken, nn, args, env)
            let m2 = m1 + ": " + threadToStr tT
            //dev_println (m2 + sprintf ": Final args are " + (sfold ceToStr args))
            vprintln 2 m2
            youtln cCC.rpt m2
            //vprintln 3 ("Pre spawn queue length=" + i2s(length !cCP.m_spawned_threads))
            mutaddoncetail (gdp.m_tthreads) (tT)
        ()
               
    let stop_int(code, m) = ()

    let unimportant = function
        //   (Xeasc(X_num _)) = true
        // | (Xeasc(X_blift _)) = true (* any constant really ? what does printf return ?*)
        | (_) -> false

    let hbev_emit k = if unimportant k then () else hbev_emitter gdp k
               
    //Scan forward for name and attribute 'hints'. The attributes need to be correct really.
    // Need to get attributes from following field as well as name in format ... 

            
                             
    (*------------------------------------*)

    let is_array = function
        | X_bnet ff -> ff.is_array
        | (_) -> false


    (*
     * An assign is either to a variable, a field or an array.  vassigns are used for arrays since they provide the index expression.
     * 
     *)
    let rec ats_mine arg =
        match arg with
            | CT_knot(idl, whom, v) when not(nonep !v) -> ats_mine (valOf !v)
            | CTL_reflection_handle _
            | CTL_void -> []

            | CTL_record(idl, cst, lst, len, ats, binds, _) -> ats 

            //| CT_valuetype ct
            //| CT_brand(_, ct)

            | CT_arr(ct, _)
            | CT_star(_, ct)  -> ats_mine ct
            | CT_cr crt       -> crt.ats
            | CT_class cst    -> cst.ats
            | CTL_net _ -> [] ///hmm it has cil_at_flag_t style ats
            | other -> sf ("ats_mine other: " + typeinfoToStr other)

    let m_vreg_use_report__ = ref [] // Not reported since not shared.

    // Codegen2 is called from codegen 1 once the lhs array index has been scaled (or directly for scalars sic).
    // Codegen1 preprocesses complex lvalues.
    let assign_codegen2 ee (deval) starx2 mf ats lhs_ce rhs aso =
        let bounds_check(n) =
            if n < 0 then cleanexit(mf() + " array subscript is negative")
        let vd = -1 // normally -1 for off
        let vf ss = if vd>=5 then vprintln 5 ss
        let mx = "assign_codegen2"
        vf ("cg2 do rhs " + ceToStr rhs)     

        let ltype = // The type we need here get_ce_content_type for an indexed assign and get_ce_type for a scalar.
            match lhs_ce with 
                | CE_var(uid, vdt, idl, fdto, ats) -> vdt
                | CE_dot(dt_record, l0, (tag_no, fdt, ptag_utag, dt_field, tag_idx), aid) -> dt_field
                | CE_subsc(CT_star(1, CT_arr(cdt, len)), l, idx, nto) // TODO this willfully discards a star - need to keep track
                | CE_subsc(CT_arr(cdt, len), l, idx, nto) -> cdt
                | CE_x(dt, _) -> dt // hmm if this were a subsc?
                | CE_struct(idtoken, dt, _, _) -> dt
                | ce -> sf("assign_codegen2: other lhs_ce form: " + ceToStr ce)


        //vprintln 0 (sprintf "  assign codegen2 ltype %s   rval=%s rtype=%s" (cil_typeToStr ltype) (ceToStr rhs) (cil_typeToStr(get_ce_type mx rhs)))
        let rhs = gec_CE_conv ww "lower: lhs-of-assign" ltype CS_maskcast rhs // Assignment mask: convert lhs of assign to rhs form (sign extend or clip).
        //vprintln 0 (sprintf "  assign promoted to %s" (ceToStr rhs))
        if vd>=5 then vprintln 5 ("cg2 rhs conv done. " + mf())

        let is_purely_symbolic = not_nonep(ce_is_symbolic_var_reference "assign_codegen2" rhs) // used only to simplify/shortcut pass-by-reference references.
        //vprintln 3 ("assign_codegen2: start lower rhs")
        let rhs' =
            if is_purely_symbolic then
                vprintln 3 "rhs purely symbolic - no hbev command emit but keval env is updated"
                None
            else
                //vprintln 0 ("rhs not purely symbolic: " + ceToStr rhs)
                Some(nlower ww (*here*)cCP  mx mf rhs)
        //vprintln 3 ("assign_codegen2: finish lower rhs")
        if vd>=5 then vprintln 5 ("cg2 rhs lower done.")

        let rec lower_lmode (ee:env_t) lhs_ce rhs = // local function of assign_codegen2 that lowers the lhs and invokes env_evolve as well
            //vprintln 0 ("start lower_lmode " + ceToStr lhs_ce)
            let msg = "lower_lmode"
            match lhs_ce with
                | CE_x(_, x) -> (x, ee) // Simple-assign  
                    // ?Confused - vassign has already split out the base and idxo forms.  ...
                    
                | CE_subsc(CT_star(1, CT_arr(cdt, len)), lhs0, idx, aid_o) // TODO this willfully discards a star, but only in the type - need to keep clean this really.
                | CE_subsc(CT_arr(cdt, len), lhs0, idx, aid_o) ->  // lower_lmode
                    let mx = "lower_lmode: subsc form 33"
                    let rnsc = mine_rnsc lhs0 // really_noscale
                    let adt_ = CT_arr(cdt, len)
                    let idx' = nlower (WN "lower_lmode subsc nlower" ww) (*here*)cCP  msg mf idx
                    if vd>=5 then
                        vprintln 5 ("lower_lmode subsc of adt=" + typeinfoToStr adt_)
                        vprintln 5 ("                      ce=" + ceToStr lhs_ce)
                    let subsc' =
                        if rnsc then idx'
                        else
                            let scaling = ct_bytes1 ww "L2454" cdt
                            let _ = 
                                if scaling >= 1L then if vd>=5 then vprintln 5 ("                 scaling=" + i2s64 scaling)
                                else cassert(scaling >= 1L, sprintf "scaling >= 1 fail. scaling=%i dt=%s" scaling (cil_typeToStr cdt))
                            //vprintln 0 (msg + sprintf ": now i scale rnsc=%A" rnsc)
                            if scaling=1L then idx' else wt_times(idx', xi_int64 scaling)
                    // TODO do array bounds check trapping again.
                    //  bounds_check(x_manifest_int(subscript, "assign_codegen1"))
                    let (a_ce, a_x) = mk_wondarray (WN "lower_lmode subsc wa" ww) "lower_lmode subsc wa" (cdt)
                    //vprintln 3 (msg + " lhs=" + ceToStr lhs0)
                    //let _ = assertf(cdt=cdt1, fun() -> "cdt=cdt1" + typeinfoToStr cdt1 + " cf " + typeinfoToStr cdt)
                    let base_ptr = nlower (WN mx ww) (*here*)cCP  mx mf lhs0
                    //The cached aid_info fields describe the result of the CE_subsc expression so need incrementing (deleting an A_subcs) to get back to the base and offset expression storage classes themselves.  This is now inside gec_O_subs
                    //let aid_o1 = oapp (inca_for_vassign mx (cilnorm_sitemarker + "O2559")) aid_o
                    let subsc' = gec_O_subs mf (aid_o, base_ptr, valOf_or len 0L, subsc')
                    if vd >= 5 then vprintln 5 (sprintf "  lower_lmode subsc' =%s  aid=<<%s>> " (idx2s subsc') (aidoToStr aid_o))
                    //let aid_2o = muddy "aid2o : check was this locally incremented first!"
                    let ee = evolve_idxd (WN "evolve lower_lmode subsc" ww) ee aid_o ats subsc' rhs
                    let _ = unwhere ww
                    (general_subscription_lowerer ww mx (a_ce, a_x, subsc'), ee)
                    
                | CE_dot(dt_record, CE_star(1, cel, _), (tag_no, fdt, ptag_utag, dt_field, tag_idx), aidl) ->   // lower_lmode clause 1/2 - an assign to a field in a struct.
                    let _ = sf (sprintf "lower_lmode: OLD CODE idiom0 perhaps cons tag no=%i to idl L755 " tag_no + ceToStr lhs_ce)
                    let ans = 
                        match cel with // When lhs is a struct, we are doing a field insert, so we do not wish to evaluate it.
                        // We should here return an index on a wondarray of the struct type
                        // THIS IS THE WRONG CODE FOR LMODE

                        // In general we should here return an index on a wondarray of the struct type
                            | CE_subsc(CT_arr(cdt, len), lhs0, idx, aid_o) ->  //
                                let lhs1 = deval lhs0
                                let (a_ce, a_x) = mk_wondarray (WN mx ww) "lower_lmode dot" dt_record
                                sf (mx + sprintf " L2396 pants lower_lmode tag_idx=%i utag=%s  dot struct idiom0 subsc_lhs=%s lhs1=%s" tag_idx (snd ptag_utag) (ceToStr lhs0) (ceToStr lhs1))
                                cel
                                //Discard tag completely here since it will be processed as part of the field insert packing.

                        // And here we might make a non-ataken or purely-symbolic short cut around the wondarray.
                            | CE_struct(idtoken, dt_, _, items) ->
                                 if tag_no < 0 || tag_no >= length items then sf ("lower_lmode: out of range dot idiom1")
                                 match select_nth tag_no items with
                                    | (tag, dt, None) ->
                                        cleanexit(sprintf "Cannot access unset field %s in struct %s" tag (cil_typeToStr dt_record))
                                    | (tag, dt, Some vale) ->
                                        dev_println "Here we find the old vale extracted - but we want the rest of it instead!"
                                        vale


                            // Q. Explain how env works for structs please. A. There are two kinds: packed and unpacked. Packed is used for indexed and unpacked is used for singletons. The packed form will store a CE_struct form for each row and this will be backed to binary in hexp_t form.  Unpacked has ultimately an X_bnet form for each field since these are better for FPGA registers and are never stored in DRAM or BRAM.  Also, the unpacked form folds nicely when only some fields are constant.
                                 
                            // An array of structs is _not_ lowered as a separate array for each field (unlike how repack does this for arrays of class instances).
                            // Instead the struct is always binary packed as we emit hexp_t and it will be restructure that makes further split or aggregate pack decisions for RAMs.

                                
                            | other -> muddy (mx + sprintf " L2396 lower_lmode tag_no=%i utag=%s  dot struct idiom0 other form=" tag_no (snd ptag_utag) + ceToStr other)
                    vprintln 3 (sprintf "lower_lmode: idiom0 tagno=%i src=" tag_no + ceToStr lhs_ce)
                    vprintln 3 (sprintf "lower_lmode: idiom0 tagno=%i fieldans=" tag_no + ceToStr ans)
                    //vprintln 3 (sprintf "lower_lmode: fieldans=%A" ans)                    
                    lower_lmode ee ans rhs
                    
                | CE_dot(dt_record, l0, (tag_no, fdt, ptag_utag, dt_field, tag_idx), aid_o) ->   // lower_lmode clause 2/2
                    let mx =  "lower_lmode dot index"
                    // match is_struct_field_op has already been called, so these two CE_dot clauses are for non-struct fields and the 1/1 is now dead. 
                    let rec dot_nlower_fn ss ll =
                        // we must eval lhs using deval and also apply reference operators: the order is important
                        if ss = 0 then ll
                        elif ss > 0 then
                            let ll' = reference_valuetype cilnorm_sitemarker ll
                            let _ = sf ("this code is now unused I think")
                            dot_nlower_fn (ss-1) ll'
                        else sf (sprintf "starx2 %i - l0=%s" starx2 (ceToStr l0))
                    let newl = dot_nlower_fn starx2 (deval l0)
                    if vd>=5 then vprintln 5 ( "a00 newl=" + ceToStr newl)
                    let base_ptr = nlower (WN mx ww) (*here*)cCP  mx mf newl
                    if vd>=5 then vprintln 5 ("Here a01 newl=" + ceToStr newl)
                    let vol = ct_is_volatile ww dt_field
                    //lprintln vd (fun()->"Here a02  a volf=" + boolToStr vol)
                    let (a_ce, a_x) = mk_wondarray (WN mx ww) "lower_lmode dot" dt_field
                    (* There are now 4 dt's in scope ! *)
                    let idl = ct2idl ww dt_field 
                    let ntl = oapp (inca_for_vassign "L2534" cilnorm_sitemarker) aid_o //Pass the subscript aid to O_tag, not the field content aid.
                    let subsc' = gec_O_tag mf (ntl, base_ptr, (tag_no, ptag_utag, dt_field, tag_idx))
                    if vd>=4 then vprintln 4 (sprintf " lower_lmode tag subsc' =%s  aidl=%s " (idx2s subsc') (aidoToStr ntl))
                    let ee = if vol then (vprintln 3 ("No evolve, is_volatile."); ee) // thought this check was in evolve? TODO check.
                             else evolve_idxd (WN "evolve lower_lmode tag" ww) ee aid_o ats subsc' rhs
                    //let _ = lprintln 300 (fun() -> " post assign env=" + envlist ee + " was post assign\n\n")
                    (general_subscription_lowerer ww "l53" (a_ce, a_x, subsc'), ee)

                | CE_var(vrn, vdt, idl, fdto, ats) ->
                    let pgo = None
                    let net = lnetgen ww pgo lhs_ce None
                    let ee =
                        let volf = ct_is_volatile ww vdt                         
                        if volf then
                            // We would need to evolve on volatile struct assembly for sure.
                            if vd>=4 then vprintln 4 ("evolve: no evolve: volatile lhs") // could kill any existing env entry but there should be none when volatile ...
                            ee
                        else
                            //vprintln 0 (sprintf "evolve: CE_var not volatile. starx2=%i" starx2)
                            evolve_scalar (WN "evolve lower_lmode uav" ww) ee starx2 vrn ats rhs // TODO very messy
                    (net, ee)

                | CE_struct _ -> sf ("lower_lmode SEVL - CE_anon_struct assign should be trapped above in caller")

                | other -> sf (sprintf "lower_lmode: other ce form L2332 %s" (ceToStr other))

        match is_struct_field_op "lower_lmode main dispatcher" lhs_ce with 
            | None -> // A normal scalar or indexed assign.
                if starx2=0 then
                    //dev_println (sprintf "Non-struct scalar assign lhs_ce=" ceToStr lhs_ce)
                    let (lhs, ee) = lower_lmode ee lhs_ce rhs
                    if not_nonep rhs' then assign_codegen_final hbev_emit (lhs, valOf rhs') aso
                    ee
                else
                    let msg = (sprintf "Non-struct indirect scalar assign starx2_nn=%i lhs_ce=" starx2 + ceToStr lhs_ce)
                    let l1 = deval lhs_ce // deval of l0 corresponds to eval_lmode of original lhs.
                    let l1 = discard_ce_conv msg l1
                    let l1 = remove_corresponding_star msg -starx2 l1  // Only one should be removed
                    match l1 with
                        | CE_var _ ->
                            vprintln 3 (msg + sprintf ": indirect assign to l1=%s" (ceToStr l1))
                            let (lhs, ee) = lower_lmode ee l1 rhs 
                            if not_nonep rhs' then assign_codegen_final hbev_emit (lhs, valOf rhs') aso
                            ee
                        | l1 -> sf (sprintf "plop unsupported lhs for indirect scalar assign: arg=%s   l1=%s" (ceToStr lhs_ce) (ceToStr l1))


            | Some(packedf, nn, ptag, l0, dt_struct) -> // A field insert. (outer stars already relocated to starx2 that we ignore in this branch...)
                let msg = "structure field insert"
                let (l1to, ee) =
                    if nn=0 then
                        if packedf then
                            match discard_ce_conv msg l0 with
                                | CE_struct _ -> // packed struct true field insert
                                    let old_lhs = deval l0 
                                    (Some (l0, old_lhs), ee)
                                | other ->
                                    let unpacker = gec_unpacking_fun ww msg dt_struct ptag   // unpack all fields except the one we shall overwrite
                                    vprintln 3 (sprintf "pangd field insert: 1/2 no longer woeful disregard of previous non CE_anon_struct contents which were %s" (ceToStr other))
                                    let idtoken = funique "Post_DFA"
                                    vprintln 3 (sprintf "pangd field insert: packedf=%A 1/2 no longer woeful disregard of previous non CE_anon_struct contents which were %s" packedf (ceToStr l0))
                                    (Some (l0, CE_struct(idtoken, dt_struct, idtoken, unpacker other)), ee)
                        else 
                            let she = sconer ww cCC cCBo "structure field insert not packed" l0 ptag 
                            let (lhs, ee) = lower_lmode ee (she.cez) rhs // Lower lmode will do the env evolve
                            if vd>=5 then vprintln 5 ("cg2 baz a struct indirect unpacked struct singleton")
                            let rhs = nlower ww (*here*)cCP  mx mf rhs
                            assign_codegen_final hbev_emit (lhs, rhs) aso
                            vprintln 3 ("finished assign indirect to unpacked struct singleton_she = " + ceToStr she.cez)
                            (None, ee)
                    else if nn=1 then
                        //dev_println (msg + " pangd indirect field insert")
                        let l1 = deval l0 // deval of l0 corresponds to eval_lmode of original lhs.
                        let l1 = remove_corresponding_star msg nn l1
 
                        vprintln 3 (sprintf "pangd indirect field insert: ptag=%s  packedf=%A\n  l0=%s\n  l1=%s\n  " ptag packedf (ceToStr l0) (ceToStr l1))
                        if packedf then
                            let template = deval l1
                            //let lhs_primed = deval_lmode lhs_ce
                            vprintln 3 (sprintf "sshat indirect packed field insert: ptag=%s  packedf=%A\n  l0=%s\n  l1=%s\n  template=%s\n " ptag packedf (ceToStr l0) (ceToStr l1) (ceToStr template))
                            (Some(l1, template), ee)
                        else // If unpacked, then must be findable on the singleton heap
                            let she = sconer ww cCC cCBo "structure indirect field insert unpacked" l1 ptag 
                            let (lhs, ee) = lower_lmode ee (she.cez) rhs // Lower lmode will do the env evolve
                            if vd>=5 then vprintln 5 ("cg2 baz a struct indirect unpacked struct singleton")
                            let rhs = nlower ww (*here*)cCP  mx mf rhs
                            assign_codegen_final hbev_emit (lhs, rhs) aso
                            vprintln 3 ("assign indirect to unpacked struct singleton_she = " + ceToStr she.cez)
                            (None, ee)

                    else muddy ("unhandled indirect struct field insert " + ceToStr lhs_ce)

                let struct_update msg ov tag nv =
                    let rec su1 = function
                        | [] -> sf (sprintf "Could not find a tag named '%s' to update with struct %s" tag (cil_typeToStr dt_struct))
                        | (id, dt, _)::tt when id=tag -> (id, dt, Some rhs) :: tt
                        | other_entry::tt -> other_entry :: su1 tt
                    // packed field insert : hopefully in CE_struct form but we can also cope with packed binary forms. We load old, update field value, store back
                    match ov with
                        | CE_struct(idtoken, dt, aid, old_contents) -> CE_struct(idtoken, dt, aid, su1 old_contents)
                        | l20 -> // sf (sprintf "struct_update for tag '%s' not applied to a struct. Applied to %s instead " tag (ceToStr other))
                            let unpacker = gec_unpacking_fun ww msg dt_struct ptag   // unpack all fields except the one we shall overwrite
                            vprintln 3 (sprintf "+++ pangd field insert: 2/2 no longer woeful disregard of previous non CE_anon_struct contents which were %s" (ceToStr l20))
                            let idtoken = funique "Post_DFA"
                            CE_struct(idtoken, dt_struct, idtoken, su1(unpacker l20))
                        

            //muddy (sprintf "temp stop field insert ptag=%s giving=%s" ptag (ceToStr newv))

                match l1to with
                    | None -> ee
                    | Some(l1x, template) ->
                        let newv = struct_update msg template ptag rhs
                        let (lhs, ee) = lower_lmode ee (l1x)  newv // Lower lmode will do the env evolve
                        if vd>=5 then vprintln 5 ("cg2 a struct s")
                        let newv' = nlower ww (*here*)cCP  mx mf newv
                        if not_nonep rhs' then assign_codegen_final hbev_emit (lhs, newv') aso
                        if vd>=5 then vprintln 5 ("cg2 b struct done")     
                        ee

    //If noscale holds, as used by ldind/stind, we do not need to apply a scale factor as we add idxo here.
    //This works for stind but ldind also need writing... TODO check it ...
    //We dont support un-aligned stores : remainder on noscale / is discarded.
    // The putative reason for this code is some transposing of the dot and subscript operators... utimately repack does that for us now?
    // aidl as passed in is possibly wrong but it is NOT USED
    let rec assign_codegen1t kxr ee d2 starx1 mf (aid_o:aid_t option) lhs_ce idxo rhs aso =
        let noscale = true
        let mx = "assign_codegen1t"
        if vd >= 5 then vprintln 5 (mx + sprintf ": lhs=%s idx=%s rhs=%s" (ceToStr lhs_ce) (if nonep idxo then "None" else xToStr(valOf idxo)) (ceToStr rhs))
        let vf xf = () // Debug code: vprintln 0 ("PIX: " + xf())
        match (lhs_ce, (idxo:hexp_t option)) with
            | (CE_star(ss, ce_inside, aid1), idxo) ->
                //dev_println (sprintf "assign_codegen1: lhs is star: Ignored aid1=%s for aid_o=%s" (aidToStr aid1) (aidoToStr aid_o))
                assign_codegen1t kxr ee d2 (ss+starx1) mf aid_o ce_inside idxo rhs aso

            // All non-indexed cases should go through to cg2 ... 
            | (CE_dot(dt_record, _, (tag_no, fdt, ptag_utag, dt_field, idx), aid), None) // non-indexed field case
                                             -> assign_codegen2 ee d2 starx1 mf (ats_mine dt_field) lhs_ce rhs aso

            | (CE_var(_, dt, _, fdto, _), None)    -> assign_codegen2 ee d2 starx1 mf (ats_mine dt) lhs_ce rhs aso // non indexed scalar case: direct route to second stage
            // We don't see dots of uav's because these are elided in eval/scone ?

            | (CE_subsc(CT_arr(dt, _), _, _, _), None) ->
                // We freely accept sassign to a subsc instead of a vassign - indeed the vassign is converted using gec_idx and comes this way now, so no difference really and the code can be tidied.
                //   lhs=CE_subsc(CT_arr(CTL_net(false, 32, Signed, native), 9), CE_region($aD_SINT%$array-1d$/D/SINT%400000%36, {bytes=36, length=9, cid=SINT.D.$array-1d$, marker=wondtoken, constant=true}), CE_x(num 0),0^fastspilldup10)
                assign_codegen2 ee d2 starx1 mf (ats_mine dt) lhs_ce rhs aso // Pass base and idx separately to codegen2

            | (CE_struct(idtoken, dt_record, aggregate_nemtok_l, l_items), None) ->
                match rhs with
                    | CE_struct(idtoken, dt_, aggregate_nemtok_r_, r_items) -> // Simple code - general clause that now follows should have the same effect
                        vprintln 3 (sprintf "structure assign - deferred site: arity should match here: %i =?= %i" (length l_items) (length r_items))
                        let struct_deferred_assign ee = function
                            | ((ptag, _, Some lhs), (_, _, ro)) ->
                                if nonep ro then
                                    dev_println (sprintf "+++ need to check this is gtrace and not incomplete. TODO.  ptag=%s" ptag)
                                    ee
                                else
                                let rhs = valOf ro
                                //let aid_l = ce_aid0 (fun()->mf()+":deferred-struct-assign-lhs") lhs // TODO these are post-eval dataflow trackers - all that is needed is blank aids?
                                //let aid_r = ce_aid0 (fun()->mf()+":deferred-struct-assign-lrs") rhs
                                let ee = assign_codegen1t kxr ee d2 starx1 mf (*aid_l, aid_r*) None lhs None rhs aso
                                ee
                            | ((ptag, _, lo), (_, _, ro)) ->
                                let dx oo = if nonep oo then "None" else sprintf "Some(%s)" (ceToStr(valOf oo))
                                // Explain please how constant meet interacts with complete structs?
                                sf (kxrToStr !m_linepoint + sprintf ": L2773a - incomplete structure assign for %s:  tag %s missing. lo=%s ro=%s" (cil_typeToStr dt_record)  ptag (dx lo) (dx ro))
                        let ee = List.fold struct_deferred_assign ee (List.zip l_items r_items)
                        ee
                    | rhs ->
                        vprintln 3 ("SEVL delane-assign to CE_anon_struct")
                        let rec get_r_items = function
                            //| CT_valuetype(dt)
                            | CT_star(_, dt)   -> get_r_items dt
                            | CTL_record(idl, cst, items, len, _, binds, _) -> items
                            | other -> sf (sprintf "CE_anon_struct get_r_items other form %s" (cil_typeToStr other))
                        let pairs = List.zip (l_items) (get_r_items dt_record)
                        let struct_assigner ee ((tag_, dt_, l_opt), rx) =
                            let r2 = gec_field_snd0 ww cilnorm_sitemarker "delane-assign" dt_record (Some rx.ptag) rhs  // aka gec_CE_dot
                            let l = valOf_or_fail "L2878 - partial struct" l_opt
                            let ee = assign_codegen2 ee d2 starx1 mf (ats_mine dt_record) l (once "delane-assign" r2) aso
                            (ee)
                        let (ee) = List.fold struct_assigner ee pairs
                        ee

            | (CE_x(dt, _), None) -> assign_codegen2 ee d2 starx1 mf (ats_mine dt) lhs_ce rhs aso

            | (CE_apply(CEF_mdt mdt, cf, [arg]), None) ->  // A register store becomes a TLM application of 'write' method. 
                let msg = "TLM-write"
                let sri_dt = mdt.rtype
                let (prec, _) = get_prec_width ww sri_dt
                
                let fu_kind = valOf_or_fail "L2953" mdt.fu_arg 
                let g_reflection_token_prec = { signed=Signed; widtho=Some 64 }
                let prec = lower_type ww msg sri_dt
                let (fu_kind_, xobj) =
                    match arg with
                        | CE_x(CTL_reflection_handle fu_kind, xobj) -> (fu_kind, xobj)
                        | other ->sf(sprintf " L2954: need fu kind from %A" arg)
                let sri_token =
                    match xobj with
                        | X_net(sri_token, _) -> sri_token
                        | _ -> sf "L2964 TLM"
                let (fname_, gis) = gec_generic_sri_tlm_fgis prec sri_token fu_kind mdt.fsems true // untidy
                let fname =  "write"
                //let xobj = gec_X_net (hptos sri_token)
                //let aobj = gec_yb (xobj, sri_dt)
                //let cez = CE_apply(bif_gis, cf, [aobj])
                //(sprintf "TLM WRITE mdt=%s rhs=%s" (hptos mdt.idl) (ceToStr rhs))
                let rhs = nlower ww (*here*)cCP  mx mf rhs
                hbev_emit(Xeasc(xi_apply_cf cf ((fname, gis), [ xobj; rhs ])))
                ee

            | (lhs_ce, None) ->  kxr_sf kxr ("codegen1 assign other non-indexed lhs = temp stop - should just fold it on to cg2 lhs: lhs=" + ceToStr lhs_ce)

            // TODO pass in stars to ct_bytes1 ...?
            // TODO starx is being largely ignored - tidy
            | (lhs_ce, Some idx) -> 
                let dt = get_ce_content_type "assign_codegen1" lhs_ce
                if vd>=5 then vprintln 5 (sprintf "assign_codegen1 idx present: lhs_ce=%s idx=%s" (ceToStr lhs_ce) (xToStr idx))
                let idxo' =
                    vprintln 3 (mx + sprintf ": noscale=%A" noscale)
                    if noscale then n64_liftx idx
                    else
                        let scaler = ct_bytes1 ww "assign_codegen1 idx present: noscale=false. L2578" dt
                        n64_liftx(wt_times(idx, xi_int64 scaler)) // This multiply will be divided away again in repack recipe stage.
                let ce' = gec_idx ww cilnorm_sitemarker (fun()->mf()+":assign_codegen1 idxo") lhs_ce aid_o idxo'
                //vprintln 3 (sprintf "assign_codegen1: noscale=%A and  scale factor=%i giving " noscale scaler + ceToStr ce')
                //if vd>=5 then vprintln 5 ("assign_codegen1: added l-side subscript: " + ceToStr ce' + sprintf " aid=<<%s||%s>>" (aidoToStr (fst aid2)) (aidoToStr (snd aid2)))
                assign_codegen1t kxr ee d2 starx1 mf aid_o ce' None rhs aso // was noscale=false // Go round again with idxo=None

            //| (lhs_ce, idxo) -> sf ("assign_codegen1 other lhs: lhs=" + ceToStr lhs_ce + "\n idx=" + (if idxo=None then "None" else xToStr(valOf idxo)))

    let LL site = nlower ww (*here*)cCP  site

    // sassign0 and vassign0: These call codegen1 which calls codegen2.
    let vassign0t kxr ee d2 mf (lhs, rhs, (idx, aid_o), aso) = // TODO if this aid is to be used as an override then please note it is now disabled at the bottom in gec_idx. It makes sense to keep this associated with the idx expression we have to hand.  Then it might get used properly. But all aid operations are now done with static analysis of the kcode and so are not needed when interpreting the vassign and other keval operations... No they are not, the ones embedded as annotations are still handled by lower.
        match ce_is_symbolic_var_reference "vassign subscript" idx with // used only to simplify/shortcut pass-by-reference references.
            | Some scalar ->
                vprintln 3 (sprintf "divert vassign: making a (symbolic) indirect assign to unaddressable scalar " + ceToStr scalar)
                assign_codegen1t kxr (ee) d2 0 mf aid_o scalar None rhs aso
            | None ->
                let idx_lowered = LL "vassign0" mf idx
                assign_codegen1t kxr (ee) d2 0 mf aid_o lhs (Some idx_lowered) rhs aso

    let sassign0 kxr ee d2 m (aid_o, corelhs, r, aso) = assign_codegen1t kxr (ee) d2 0 m aid_o corelhs None r aso // was noscale=false but irrelevant

    let zero_padding_p arg = (ce_manifest_int64 "zero_padding_check" arg) = 0L

    let InitializeArray_items signed radix r =
        let rec gidl = function
            | CE_reflection(_, None, Some idl, _, _) -> idl
            | CE_region(hidx, dt, ats, _, cil_)   -> hidx.f_dtidl
            | other -> sf("gidl other " + ceToStr other)

        let rec retrieve_initiation_blob cc = function
            | No_class(srcfiles, flags, idl, _, items, prats) -> List.fold retrieve_initiation_blob cc items
                
            | No_knot(CT_knot(idl, whom, v), noknot) ->
                if not(nonep !noknot) then retrieve_initiation_blob cc (valOf !noknot)
                else sf ("retrieve_initiation_blob knot untied " + whom)
            
            | No_field(layout, flags, qq, name, Cil_field_at atclause, _) ->
                let where = [ iexpToStr atclause ]
                let r' = id_lookupp ww where Map.empty (fun()->("array init not found\n", 1))
                //vprintln 3 ("Initialising with " + noitemToStr true (valOf r'))
                let initer =
                    match (valOf r') with
                        | No_esc(CIL_data(l, Cil_explist r)) -> r
                        | k -> sf ("array init : other")
                initer :: cc

            | No_field(layout, flags, qq, name, Cil_field_lit(Cil_blob r), _) ->
               //vprintln 3 ("Initialising with blob " + iexpToStr(Cil_explist r))
               r :: cc

            | No_field(layout, flags, qq, name, Cil_field_lit(Cil_explist r), _) ->
               //vprintln 3 ("Initialising with " + iexpToStr(Cil_explist r))
               r :: cc

            | No_esc _ -> cc
            | other -> sf (sprintf "retrieve_initiation_blob other %A" other)

        let data =
            match (gidl r) with
            | [spog; where] ->
                let r' = id_lookupp ww [spog; where] Map.empty (fun()->("array init not found L2329\n", 1))
                vprintln 3 ("Initialising with " + noitemToStr true (valOf r'))
                match retrieve_initiation_blob [] (valOf r') with
                    | [blob] -> blob
                    | [] -> sf "no array init data blobs found"
                    | blobs  -> sf (sprintf "multiple init blobs for [%s, %s] found: %A" spog where blobs)
            | other -> sf(sprintf "retrieve_initiation_blob22 other form: %A" other)
        array_init_pack signed radix data

    // 2d and higher array inits are a little different from 1d owing to the 1d being a native type whereas the higher dimensions are constructed via library code with just a little hardwired help. 
    let InitializeArray_2d kxr e d2 dt mf (atype, dt_, ceA, uid, rhs) =
        let ids = uidToStr uid
        let msg = ids + ": InitializeArray2d."
        let ww = WF 3 msg ww "start"
        let contentType = get_content_type_from_type msg atype
        let msg = ids + ": InitializeArray2d.  Content type=" + typeinfoToStr contentType
        vprintln 3 msg
        let bitradix = 8 * int32(ct_bytes1 ww msg contentType)
        let signed = ct_signed ww contentType
        vprintln 3 (msg + ": Lifted 2d array init: bitradix=" + i2s bitradix + ".\n" + ids + " := " + ceToStr rhs)
        let data = InitializeArray_items signed bitradix rhs   
        let arraybase = gec_field_snd_pass ww cilnorm_sitemarker msg atype ceA "arraybase"
        vprintln 3 (sprintf "arraybase is %s" (ceToStr arraybase))
        let aid_bo = oapp (anon_deca_for_vassign cilnorm_sitemarker) (ce_aid0 (fun()->mf()+":2dinit aid-getter") arraybase)
        let aid2 = aid_bo // (aid_bo, None) // offsets are just undecorated integers, hence aid_o=[]
        let ce = (d2) arraybase // cg1 does not do this eval because such evals are (wierdly) done in gtrace kcode_stepper.
        let warn xx = hpr_warn xx
        vprintln 3  (msg + ": internal 1d actual array:\n        aid_b=" + aidoToStr aid_bo + "\n       class A=" + ceToStr ceA + "\n       arraybase=" + ceToStr arraybase + "\npost eval arraybase=" + ceToStr ce)
        let l =
            let cel = ce_arraylen ww ce
            if ce_constantp cel then ce_manifest_int64 "2d-arrraylen" cel
            else cleanexit(msg + ": Expected constant 2d-arraylen. Not " + ceToStr cel)
        if l < 0L then sf ("initialize2d: not an array:" + ceToStr ce)
        let setup2d_assign e d2 m p v = vassign0t kxr e d2 m (ce, v, (n64_lift p, aid2), { abuse=None; reset_code=false}) // In one sense it is reset code, but we should not discared user's explicit init intentions.  It will go in big_bang.
        let rec setup2d e d m p = function
            | [] -> if p < l-1L then (warn(ids + sprintf ": Not all 2-d array locations init: %i/%i" p l); e)
                    else e
            | h::t -> 
                if p >= l && not (zero_padding_p h) then (vprintln 0 (sprintf "Extra init item for 2-d array of length %i: %A" l h); e)
                elif p>=l then e
                else setup2d (setup2d_assign e d2 m p h) d2 m (p+1L) t
                
        let e' = setup2d e d2 mf 0L data
        (X_undef, e')
           

    // 1-d: this only deals with constant binary data.
    let InitializeArray_1d kxr e d2 mf (aid, atype_, dt, ceA, bytelen, r) =
        let msg = "InitializeArray_1d"
        let bytes = array_item_size_in_bytes ww msg dt
        let signed = ct_signed ww (get_content_type_from_type msg dt)
        let len = bytelen / (int64)bytes
        let _ = if len <= 0L then sf(mf() + ": length of array 1-D is zero")
                else vprintln 3 (sprintf "Making array init of length %i: dt=" len + typeinfoToStr dt)

        vprintln 3 (sprintf "Native 1-D array init: length=%i int bytes=%i: " len bytes + ":  " +  ceToStr ceA + " := " + ceToStr r + " aid_b=" + aidoToStr aid)
        // For constant data init, we need to ensure array is elabd.
        let data = InitializeArray_items signed (8 * int32 bytes) r   
        let warn(x) = hpr_warn(mf() + ":*** Warning: " + ceToStr ceA + ": " + x)

        let setup1x e (p, v) =
            //vprintln 0 (msg + sprintf ": InitializeArray: Setup1 p=%A  v=%A" p v)
            vassign0t kxr e d2 mf (ceA, v, (n64_lift p, aid), {abuse=None; reset_code=false})

        let rec setup1d env p = function
            | [] ->
                let _ = if p < len then warn(sprintf "Not all 1-D array locations init: %i/%i (blob bytes=%i)"  p len bytes)
                env
                
            | h::tt -> 
                if p >= len && not (zero_padding_p h) then
                    let  _ = warn(sprintf "Too many values in array init: %i\n" p)
                    env
                elif p >= len then env
                else setup1d (setup1x env (p, h)) (p+1L) tt
        let e' = setup1d e 0L data
        if vd>=4 then vprintln 4 (sprintf "Initialised array 1-D locations.")
        (X_undef, e')

               
    let rec InitializeArray kxr e d2 mf (atype, aid_for_1d, ceA, r) = // We support 1d and 2d array initialisations hardwired.
        match ceA with
        | CE_region(uid, dt, ats, nemtok, cil)  ->
            match  rev uid.f_dtidl with 
                | "@" :: _ ->
                    let bytelen = valOf_or_fail "array init length"  uid.length
                    InitializeArray_1d kxr e d2 mf (aid_for_1d, atype, dt, ceA, bytelen, r)
                |  dtidl when hd dtidl = valOf(!g_kfec1_obj).d2name + "`1" ->
                    InitializeArray_2d kxr e d2 dt mf (atype, dt, ceA, uid, r)
                |  dtidl ->
                    muddy (sprintf "3d or higher array init? uid=%A dtidl=%A\nArray-dt-names=%A" uid dtidl (!g_kfec1_obj))
        // Here we discard the star and it would be an error not to have it so we should really check that one was discarded overall.
        | CE_conv(ignored_, _, ce) -> InitializeArray kxr e d2 mf (atype, aid_for_1d, ce, r) 
        | CE_star(ignored_, ce, _) -> InitializeArray kxr e d2 mf (atype, aid_for_1d, ce, r) 
        | l ->  sf("init array: unrecognised array form: " + ceToStr l + " := " + ceToStr r + "\n" + "init array other\n")

          
    let style_sel mf = function
        | [arg]   ->  [ g_autoformat; LL "style_sel" mf arg]
        | hh::tt  ->  printf_ansi_form mf ("g_autoformat" :: (map format_escape_mine tt), List.zip (hh::tt) (map (LL "style_sel:args" (fun()->mf()+":style_sel")) (hh::tt)))

    //These built-in functions should really look at the whole class name, not just last word ! 

    // Every class object has an implied mutex inside it and here we create explicit definitions thereof.
    let rec mutexof d ce =
        match ce with
        | CE_subsc(_, A, _, _) -> mutexof d A  (* hmm! discards subsc  - need to check arrays of mutex vars! *)

        | CE_var(vrn, dt, _,  _, ats) when dt = g_ctl_object_ref -> // Some versions of gmcs convert mutex to object themselves.
            let uid = muddy "need PHYS uid"
            let idl = "mutex" :: uid.f_name
            // Use same hidx. - Maps on first field of the object: inherited from object.
            // let (obj, env') = gec_uav ww' m0 STOREMODE_ g_canned_bool (full_idl, ats) 0 envo
            let uid' = { uid with f_dtidl=rev ["System"; "Boolean" ];  length=Some 4L; f_name=idl }
            let cem =
                muddy "restore me cem"
                //CE_var(PHYS uid', g_canned_bool, idl, [(g_volat, "true"); ("mutex", "true"); ("init", "0")])
            vprintln 3 ("mutex var rez " + ceToStr ce + " --> " + ceToStr cem)
            cem

        // This is an eval l-mode/ref function - todo unifiy - its a form of eval that does not derefence an ultimate lvalue (perhaps could make keval always return its ultimate dereferee ?)
        | CE_dot(ttt, l0, (tag_no, fdt, ptag_utag, dt1, offset), aid) ->
            let l1 = d l0
            let sbs () = CE_dot(ttt, l1, (tag_no, fdt, ptag_utag, dt1, offset), aid) // symbolic result
            muddy ("ARRAYS CONTAINING MUTEXES: BROKEN IN THIS SPECIFIC RELEASE OF KIWIC mutex dot tag=" + snd ptag_utag + "\n  ce=" + ceToStr (sbs()))
 
        | CE_region(uid, dt, _, _, _) ->
            let idl = [ "mutex"; uidToStr uid ]
            let dt = g_canned_bool
            (* let init__ = gec_yb(xi_num 0, dt)  : TODO need to use init ? *)
            //vprintln 0 ("mutex av created " + hptos idl)
            let ats = []
            let sc = None
            ce
        | other -> sf ("mutex of other (every dot net object can be used as a mutex but this one cannot) other=" + ceToStr other)
            
    let cdec = function
        | T_steps(n, b) -> if n=0 then T_runout else T_steps(n-1, b)
        | other -> other

                                                           
    let remark ss = (vprint 3 ("emit remark " + ss); if gdp.gen<>None then G_emit (valOf gdp.gen) (Xcomment ss))
    // The keval/gt_step code will soon be moved out into a different file entirely
    // It's nested in here for historic reasons (still supporting operation without gtrace).

    let do_newobj ww mf env aid static_cache_f (KN_newobj(idl, hintname, stru, dt, ats)) =
        let msg = "do_newobj"
        let ww = WN msg ww
        let vd = 3
        let gb0 = Map.empty // for now - TODO OCT14 - old code ? all types are now grounded in the kcode surely?        
        let dt = template_eval ww ({grounded=false; }, gb0) dt
        let rec newobjfun dt_record =
            match dt_record with
            | CT_cr crst when crst.name = g_core_object_path -> newobjfun (g_canned_object)
            | CT_star(_, CTL_record(idl, cst, items, len, _, binds, _)) // support both forms for now. One is correct.
            | CTL_record(idl, cst, items, len, _, binds, _) ->
                let bytes = int64 len
                let ids = hptos idl
                // aid       = dataflow_id: textual name of the allocation site in this instance.
                // uid.name  = unique instance symbolic name -- allocated at time hidx is bumped - perhaps fold down into top11.
                // uid.dtidl = class type name  
                let rec namer__ = function // no longer used
                    | (".ctor" :: a:: t) -> namer__(a::t)
                    | idl -> if String.length(hptos idl) > 10 then (hptos idl).[String.length(hptos idl)-10..] else "newobj_uid"

                let cgr1 = Map.empty
                let storemode = if static_cache_f then fun _ -> STOREMODE_compiletime_heap else storemode
                let (obj, uido, envo) = mk_dyna_object_top11 ww mf cCL (Some aid) ((idl, hptos_net idl), idl, dt, cgr1) storemode stru env // This does all the allocation work. The ctor body is inlined in the kcode after this allocation point.
                if cCC.settings.dynpoly && not_nonep cst.vtno then
                    let lhs = gec_field_snd0 ww cilnorm_sitemarker msg dt_record (Some "_kv_table") obj // aka gec_CE_dot
                    hbev_emit(Xassign(LL msg mf (hd lhs), xi_num(valOf cst.vtno))) 
                    vprintln 3 (msg + sprintf ": Setup vtable field %s to %i" ids (valOf cst.vtno))
                    ()
                if not_nonep uido then ignore(record_memdesc_item aid "do_newobj" hintname ats (valOf uido) obj dt)
                if vd>=4 then vprintln 4 (sprintf "obj made, ctor not yet elaborated.  obj=" + ceToStr obj)
                //vprintln 0 (sprintf "do_newobj: post op heap sbrk %s " (ce_hs_envToStr 10 (valOf envo).hs1))            
                (obj, valOf envo)
            
            | other -> sf (sprintf "do_newobj other %s" (cil_typeToStr other))
        newobjfun dt
        
    let create_array ww env (mf, m) (ct, len, region_nemtok, hintname_o, ats) = 
        let vd = YOVD -1
        let create_array1 (contents_type0) = 
            let msg = "create_array1"
            let leng = ce_manifest_int64 "create_array length" len
            let ctt = contents_type0
            let cgr = [ (CTVar{ g_def_CTVar with idx=Some 0; }, ctt) ]
            let dt = CT_arr(ctt, Some leng)
            let bytes = int64 leng * ct_bytes1 ww msg ctt
            let _ = if bytes=0L && not cCC.settings.zerolength_arrays then cleanexit(mf() + ":" + msg + ":" + typeinfoToStr dt + sprintf ": +++ Zero length array formed containing %i locations of %s." leng (hptos(ct2idl ww dt)))
            let dtidl = ct2idl ww dt
            let types = cil_typeToStr dt
            let un = op_assoc "username" ats 
            let ids = if un<>None then valOf un // this better not be used since gec_uav does not mirror this...
                      else hptos_net dtidl // uidToStr  { name=(if un=None then [] else [valOf un]); dtidl=dtidl; baser=hidx; length=Some bytes }

            let aspace =
                let hintname_lst = if nonep hintname_o then [] else [ valOf hintname_o ]
                user_or_logical_memspace_lookup ww m0 hintname_lst ats
            let (hidxo, ce, envo) = obj_alloc_core mf types true (storemode [ids]) (Some region_nemtok) (Some env) aspace dtidl (int64 bytes)
            let ce =
                if nonep hidxo then ce
                else
                    let uid = { f_name=[ids]; f_dtidl=dtidl; baser=valOf hidxo; length=Some bytes;  }
                    let ce = gec_CE_Region ww false (Some region_nemtok, ctt, uid, ("length", i2s64 leng) :: ("bytes", i2s64 bytes) :: ats, Some leng)
                    //let regf = true
                    ignore(record_memdesc_item region_nemtok "newarr" hintname_o ats uid ce dt)
                    vprintln 3 (sprintf "Allocated array %s, %i bytes at %i.  region_nemtok=%s hintname=%A" (ceToStr ce) bytes (valOf hidxo) region_nemtok hintname_o)
                    ce
            (ce, valOf envo)
        match ct with
            | CT_arr(dt, None) when false -> 
                   (vprintln 3 "+++ special array making kludge used - need to fix - fixed sept2011";
                    (*      sf "kludgeon"; *)
                    create_array1 dt
                   )

            | contents_type -> create_array1 contents_type

                    

    let rec keval_lmode ww (mf:unit->string) env ce =
        //let ww = WN (m0()) ww // TODO store a better thunk in ww.
        match ce with
            // Do we need: match is_struct_field_op msg lhs_ce with ...
            | CE_dot(ttt, l0, (tag_no, fdt, ptag_utag, dt1, offset), aidl) -> 
                let m1 = "keval: main keval_lmode dot clause"
                //vprintln 3 (mf() + ": aid=" + aidToStr aid)
                let (l1, env) = keval_new ww (fun()->mf()+ m1) env l0
                //let volf = hw_vol ptag_utag
                let ans = CE_dot(ttt, l1, (tag_no, fdt, ptag_utag, dt1, offset), aidl)
                (ans, env)

            | CE_star(1, l0, aid) -> 
                let (l1, env) = keval_new ww (fun()->mf()+": keval_lmode: star1-base") env l0
                let ans = CE_star(1, l1, aid)
                (ans, env)
                
            // Subsc keval lmode proto clause
            | CE_subsc(CT_arr(content_ct, len), l0, idx, aid) when false ->
                let m = ": keval_lmode: CE_subsc "
                keval_lmode ww (fun()->mf()+m) env ce

            | other -> (other, env) // for now?   // End of keval_lmode


    and keval_new ww mf env ce =
        //vprintln 0 (sprintf "keval_new call %s call" (ceToStr ce))
        let (ans, env) = keval_new_ntr ww mf env ce
        if vd>=10 then vprintln 10 (sprintf "keval_new ret  %s returned %s" (ceToStr ce) (ceToStr ans))
        (ans, env)

    and keval_new_ntr ww mf env ce =
        //let ww = WN (mf()) ww // TODO faster to store thunks in ww.
        //vprintln 0 ("keval_new of " + ceToStr ce)
        match ce with
            | CE_x _ 
            | CE_region _ ->
                (ce, env)

            | CE_var(vrn, vdt, idl, fdto, ats) ->
                match cCC.heap.sheap2.lookup vrn with
                    | None -> sf (sprintf "keval: %s virtual register %s V%i is unknown L2859 "  (mf()) (hptos idl) vrn)
                    | Some she ->
                        //dev_println (sprintf "Using nemtok from she for %s" (hptos idl))
                        let ans = envlook cCC m_shared_var_fails env callsite vdt ce she.s_nemtok she.baser ats
                        (ans, env)

            // Subsc .[] keval_new clause
            | CE_subsc(((CT_star(_, CT_arr(content_ct, len0))) as vv), l0, idx0, aidp2)
            | CE_subsc(((CT_arr(content_ct, len0))) as vv, l0, idx0, aidp2)  -> 
                let subsc_array_type = gec_CT_star 1 vv // given that vv is the actual array, and reference to it will have a star.  
                let m1 = ":keval_new subsc .[]"
                let (l1, env) = keval_new ww (fun()->mf()+":keval_new subsc-base") env l0
                let (idx1, env) = keval_new ww (fun()->mf()+":keval_new subsc-idx") env idx0

                let scaling = ct_bytes1 ww m1 content_ct
                
                let origq() = //backstop
                    vprintln 3 (sprintf "origq2: l1b pre eval=%s \npost eval reapplied=%s" (ceToStr l0) (ceToStr l1)) 
                    gec_idx (WN m1 ww) cilnorm_sitemarker (fun()->mf()+":keval_new subsc origq") l1 aidp2 idx1

                let ans =
                    match l1 with
                    | CE_region(uid, dt, ats, Some nemtok, cil) when ce_constantp idx1 ->
                        let idx = ce_manifest_int64 "keval_new subsc constant idx" idx1
                        let uid' = { uid with baser=splus uid.baser (stimes (int64 scaling) idx) }
                        let memtok = idx_aid_to_mem ww m1 uid'.baser (Some(A_loaf nemtok))
                        envlook cCC m_shared_var_fails env callsite dt (origq()) memtok  uid'.baser ats

                    | CE_region(uid, dt, ats, nemtok, cil) (* when not ce_constantp idx1 *) -> origq()

                    | other ->
                        if vd>=4 then vprintln 4 (m1 + " subsc base form match other (do not eval but insert nt): " + ceToStr other)
                        origq()
                (ans, env)
                
            // Subscripting with a field dot tag: keval_new clause:
            | CE_dot(ttt, l00, (tag_no, fdt, ptag_utag, dt1, offset), aid_o) ->   // keval_new clause 2/2
                let msg = ":Main keval subsc .tag dot clause"
                vprintln 3 (mf() + ": dot keval_new aid=" + aidoToStr aid_o)


                match is_struct_field_op msg ce with
                    | Some(packedf, nn, ptag, l0, dt_struct) -> // A field extract
                        let msg = "keval structure field extract: concise" 
                        //let msg = sprintf "keval structure field extract: packedf=%A ptag=%s nn=%i l0=%s" packedf ptag nn (ceToStr l0)
                        //dev_println (msg)
                        if packedf then
                            if nn=0 then                          
                                let (l1, env) = keval_new ww (fun()->mf()+": keval: direct packed struct field extract") env l0
                                let ans = 
                                    match l1 with
                                        | CE_struct(idtoken, dt_record, aid, items) ->
                                            if tag_no < 0 || tag_no >= length items then sf ("keval: out of range dot direct packed field extract")
                                            match select_nth tag_no items with
                                                | (tag, dt, Some vale) -> vale
                                                | (tag, dt, None) -> cleanexit (sprintf "Tag field %s is not assigned in %s" tag (cil_typeToStr dt_record))

                                        | other ->
                                            //dev_println (sprintf "L3441 keval tag_no=%i utag=%s  dot direct struct packed field extract idiom0 other form=" tag_no (snd ptag_utag) + ceToStr other)
                                            let star_aid = g_anon_aid // lost but not needed post dataflow analysis.
                                            CE_dot(ttt, CE_star(1, l1, star_aid), (tag_no, fdt, ptag_utag, dt1, offset), aid_o)  // keval_new - struct symbolic result
                                vprintln 3 (sprintf "keval: field extract packed Pantaloon idiom0 tagno=%i field=" tag_no + ceToStr ans)
                                (ans, env)
                            elif nn=1 then // We have the same code either side of this owing to coding errors earlier?
                                let (l1, env) = keval_new ww (fun()->mf()+": keval: indirect packed struct field extract") env l0
                                //vprintln 3 (sprintf "keval: indirect field extract l0=%s l1=%s" (ceToStr l0) (ceToStr l1))
                                let ans = 
                                    match l1 with
                                        | CE_struct(idtoken, dt_record, aid, items) ->
                                            if tag_no < 0 || tag_no >= length items then sf ("keval: out of range dot idiom1 packed indirect field extract")
                                            match select_nth tag_no items with
                                                | (tag, dt, Some vale) -> vale
                                                | (tag, dt, None) -> cleanexit (sprintf "Tag field %s is not assigned in %s" tag (cil_typeToStr dt_record))

                                        | other ->
                                            // K-Means Demo and test30r2: array of structs.
                                            //dev_println (sprintf "L2980 keval tag_no=%i utag=%s  dot struct idiom0 other form=" tag_no (snd ptag_utag) + ceToStr other)
                                            let star_aid = g_anon_aid // lost but not needed post dataflow analysis.
                                            CE_dot(ttt, CE_star(1, l1, star_aid), (tag_no, fdt, ptag_utag, dt1, offset), aid_o)  // keval_new - struct symbolic result
                                //vprintln 3 (sprintf "keval: indirect packed field extract tagno=%i field=" tag_no + ceToStr ans)
                                (ans, env)
                            else 
                                 muddy (sprintf "keval: structure field extract indirect packed pasta nn=%i" nn)
                        else
                            if nn=0 then
                                let msg1 = "keval: indirect unpacked scone field extract scone"
                                let she = sconer ww cCC cCBo msg1 l0 ptag 
                                vprintln 3 (msg + sprintf ": indirect unpacked scone field extract scone to " + ceToStr she.cez)
                                keval_new ww mf env she.cez
                            //elif nn=1 then
                            else
                                let l1 = remove_corresponding_star msg nn l0
                                let msg1 = "keval: structure field extract indirect unpacked pesto"
                                vprintln 3 (sprintf "keval: field extract indirect unpacked pesto nn=%i ptag=%s\n   ce=%s\n  l0=%s\n  l1=%s" nn  ptag (ceToStr ce) (ceToStr l0) (ceToStr l1))
                                let she = sconer ww cCC cCBo msg1 l1 ptag 
                                keval_new ww (fun()->mf() + msg + ": " + msg1) env she.cez                                //muddy (sprintf "keval: field extract indirect unpacked pesto nn=%i ptag=%s\n   ce=%s\n  l0=%s\n  l1=%s" nn  ptag (ceToStr ce) (ceToStr l0) (ceToStr l1))


                    | None ->
                        let (l1, env) = keval_new ww (fun()->mf()+":keval (non-sruct) dot") env l00
                        let semi_symbolic_s () =  CE_dot(ttt, l1, (tag_no, fdt, ptag_utag, dt1, offset), aid_o) // semi-symbolic result, formed around l1.
                        //vprintln 3 ("CE_dot l1=" + ceToStr l1)
                        let ans =
                                let rec scone_av = function
                                    | CE_region(uid, dt, ats, Some nemtok, x) ->
                                        let dtidl = ct2idl ww dt1
                                        let vtno = -2 // Used here?
                                        let uid' = { f_name=[hptos_net dtidl]; f_dtidl=dtidl; baser=int64 offset + uid.baser; length=None }
                                        let origx = CE_dot(ttt, l1, (tag_no, fdt, ptag_utag, dt1, offset), aid_o) // as per semi_symbolic_s!
                                        //vprintln 0 (sprintf "%s region nemtok=%s ptag=%s utag=%s" msg nemtok (fst ptag_utag) (snd ptag_utag))
                                        let memtok = idx_aid_to_mem ww msg uid'.baser (Some(A_tagged(A_loaf nemtok, fst ptag_utag, snd ptag_utag)))
                                        envlook cCC m_shared_var_fails env callsite dt1 origx memtok uid'.baser ats

                                    | CE_conv(_, _, ce) -> scone_av ce

                                    | other ->
                                       vprintln 3 ("keval_dot scone av left arg, unexpected other form: (will default to semi_symbolic): " + ceToStr other)
                                       semi_symbolic_s()
                                //vprintln 0 ("Scone av") 
                                scone_av l1
                        (ans, env)

            | CE_scond(dop, a1, a2) ->
                let LL = nlower ww (*here*)cCP  "g-CE_scond" mf
                let (l, env) = keval_new ww (fun()->mf()+":CE_scond l") env a1
                let (r, env) = keval_new ww (fun()->mf()+":CE_scond r") env a2
                let ans = gec_yb(xi_blift(ix_bdiop dop [LL l; LL r] false), g_canned_bool)
                (ans, env)
                                                                                              
            | CE_alu(rt, diop, a1, a2, aidl) ->
                let LL = nlower ww (*here*)cCP  "g-CE_alu" mf
                let prec = result_dt_to_prec ww "CE_alu:L2992" rt
                let (l, env) = keval_new ww (fun()->mf()+":CE_alu l") env a1
                let (r, env) = keval_new ww (fun()->mf()+":CE_alu r") env a2
                let e2fn_b diop (l, r) = // ALU ops - integer_promote to 32 bit for smaller differing types etc..
                    let ll_r = LL r
                    let rv = if xi_iszero ll_r && (diop=V_mod || diop=V_divide) then X_undef else ix_diop prec diop (LL l) ll_r
                    let _ =
                        if false then
                            vprintln 0 (sprintf "CE_alu keval_new l=%s r=%s" (ceToStr r) (ceToStr l))                    
                            vprintln 0 (sprintf "CE_alu keval_new promote/clip result to rt = %s" (cil_typeToStr rt))
                            vprintln 0 (sprintf "CE_alu prec=%s keval_new rv=%s" (prec2str prec) (xToStr rv))
                            ()
                    gec_yb(rv, rt)


                let e2f (lhs, rhs) =
                    let dan_ptr_ (aid_n, aid_o) = Some (aid_n, None) // Remove subscript aid since probably not correct? - address arithmetic rationalise please.

                    let dan_ptr aid = muddy "reveng aid e2f"
                    
                    match (lhs, rhs) with
                        | (CE_x(t, l), CE_star(n, CE_subsc(_, b, r, aid), aid2)) -> // Pointer arithmetic
                            let aid2 = muddy "aid2 ptr arith 1/2"
                            CE_star(n, gec_idx ww cilnorm_sitemarker (fun()->mf()+":e2f-a") b (dan_ptr aid) (CE_x(t, ix_diop prec diop l  (LL r))), aid2)

                        | (CE_x(_, l), CE_subsc(_, b, r, aid)) -> e2fn_b diop (lhs, rhs)

                        | (CE_star(n, CE_subsc(_, b, l, aid), aid2), CE_x(_, r)) ->
                            let arith_lift rt v = CE_x(rt, v)
                            let aid2 = muddy "aid2 ptr arith 2/2"
                            CE_star(n, gec_idx ww cilnorm_sitemarker (fun()->mf()+":e2f-b") b (dan_ptr aid) (arith_lift rt (ix_diop prec diop  (LL l) r)), aid2)

                        | (CE_subsc(_, b, l, aid), CE_x(_, r)) -> e2fn_b diop (lhs, rhs)

                        | (lhs, rhs) -> e2fn_b diop (lhs, rhs)
                (e2f (l, r), env)



            | CE_apply(CEF_bif bif, cf, [ce]) when bif.uname = g_bif_neg.uname -> // Subtract from zero - two's complement.
                let mx = ":system-neg"
                let (ce', env) = keval_new ww (fun()->mf() + mx) env ce
                let LL = nlower ww (*here*)cCP  "g-CE_apply" mf
                let atype = get_ce_type mx ce  // Could get from bif.rt but may be poly 
                let prec = result_dt_to_prec ww "CE_apply:L3026" atype
                let rr = xi_neg prec (LL ce')
                (gec_yb(rr, atype), env)

            | CE_apply(CEF_bif bif, cf, [ce]) when bif.uname = g_bif_not.uname ->   // One's complement - bitwise complement.
                let mx = "ones_comp g_bif_not"
                let (ce', env) = keval_new ww (fun()->mf() + mx) env ce
                let LL = nlower ww (*here*)cCP  "g-CE_apply-not" mf
                //let logical_nor_ans = gec_yb(xgen_blift(xi_not(xgen_orred(LL ce'))), g_canned_bool)
                let atype = get_ce_type mx ce  // Could get from bif.rt but may be poly
                let prec = result_dt_to_prec ww mx atype                
                let ans = gec_yb(xi_onesc prec (LL ce'), atype)
                
                (ans, env)

            | CE_apply(CEF_bif bif, cf, [ce]) when hd bif.uname = "get_Length" || hd bif.uname = "ldlen" -> // The C# length property or ldlen instuction
                let msg = "get_Length/ldlen"
                let (argd, env) = keval_new ww (fun()->mf()+msg) env ce
                let xans =
                    if false then // We no longer need these preliminary tests.?
                        let rec x_get_length = function 
                            | W_node(_, V_cast cvtf, [x], _) -> x_get_length  x
                            | W_string(s, _, _)  -> gec_yb(xgen_num(String.length s), valOf g_bif_strlen.rt)
                            | (arg) -> sf("x_get_Length: bad x form for arg: must apply hpr_strlen" + xToStr arg)
                        let rec instance_get_length = function // new copy - where is old ?                
                            | CE_x(t, x) -> x_get_length x
                            | argd ->
                                // Can perhaps get from the type
                                vprintln 3 ("builtin instance get_Length/ldlen: other form for arg: " + ceToStr argd)
                                CE_apply(CEF_bif g_bif_strlen, cf, [argd])
                        instance_get_length argd
                    else
                        let xans = ce_arraylen ww argd
                        xans
                //dev_println (sprintf "ldlen local code commented out. Now yielding %s from %A" (ceToStr xans) argd)
                (xans, env)

            | CE_apply(CEF_bif bif, cf, [a0; a1]) when bif.hpr_name = "hpr_testandset" -> // The pass-by-reference first arg is now encoded in bif.gis.outargs
            // We do not really want to hardcode any special behaviour for this function inside KiwiC until we get to bevelab.
            // Buy occurences in the lasso stem need to be evaluated and
            // the first arg is an object pointer and we convert it to a zeroth offset boolean field l-value, which is often a nop but could involve some keval work in more advanced design patterns.  We certainly dont want to r-value fetch the bool flag since we need to write it as well as read.
                let mx = "g-CE_apply:testandset"
                let ww' = WN mx ww
                let a00 = a0 //gec_CE_star 1 a0 
                let (a0', env) = keval_lmode ww' (fun()->mf()+":hpr_testandset-a0") env a00
                let (a1', env) = keval_new ww' (fun()->mf()+":hpr_testandset-a1") env a1
                if vd>=4 then vprintln 4 ("testandset: first arg=" + ceToStr a0')
                let LL ce = nlower ww' (*here*)cCP  mx mf ce
                // The LL of the first arg is ok - it's not going to derefernce an l-value.
                let a0' = gec_field_snd_pass ww cilnorm_sitemarker "apply-testandset" g_ctl_object_ref a0' g_kiwi_objectlock
                let a = gec_yb(xi_apply(hpr_native "hpr_testandset", [LL a0'; LL a1' ]), g_canned_i32)
                (a, env)

            | CE_apply(CEF_bif bif, cf, args) -> 
                let rec fap e = function
                    | [] -> ([], e)
                    | h::t ->
                        let (r, e') = fap e t
                        let (d, e'') = keval_new ww (fun()->mf()+":"+bif.hpr_name) e' h
                        (d::r, e'')
                let (args', env') = fap env (args)
                (CE_apply(CEF_bif bif, cf, args'), env')


            | CE_apply(CEF_mdt mdt, cf, args) -> 
                let rec fap e = function
                    | [] -> ([], e)
                    | h::t ->
                        let (r, e') = fap e t
                        let (d, e'') = keval_new ww (fun()->mf()+":" + mdt.uname) e' h
                        (d::r, e'')
                let (args', env') = fap env (args)
                (CE_apply(CEF_mdt mdt, cf, args'), env')




            | CE_struct(idtoken, ct, aid, lst) -> // Structure keval - a new struct with each item eval'd. - Are these used? Structure assigns are converted to multiple scalar assigns in intcil.
                let rec faps ee = function
                    | [] -> ([], ee)
                    | (ptag, dt, vo)::tt ->
                        let (r, ee) = faps ee tt
                        let (d, ee) =
                            if nonep vo then (None, ee)
                            else
                                let (d, ee) = keval_new ww (fun()->mf()+": keval_CE_anon_struct") ee (valOf vo)
                                (Some d, ee)
                        ((ptag, dt, d)::r, ee)
                let (lst', env') = faps env lst
                (CE_struct(idtoken, ct, aid, lst'), env')
                
            | CE_conv(ct, cvtf, ce) -> // keval site
                let (ce', env) = keval_new ww (fun()->mf()+": keval_newCE_conv") env ce
                (yan_ce_conv ww cCP mf ct cvtf ce', env)


            | CE_star(nn, arg_ce, aid) ->
                if   nn > 0 then (ce, env) // The address of an expression (e.g. lvalue) is not the address of that expression eval'd!
                elif nn < 0 then
                    let msg = (sprintf "Indirect star read nn=%i" nn)
                    vprintln 3 msg
                    // Here we would handle the identity *(&p) === p, but this does not arise with the current star algorithm (its one good feature!) in the absence of CE_conv wrappers
                    // example needed ...
                    let (a0, env) = keval_new ww (fun()->mf()+msg) env (gec_CE_star msg (nn+1) arg_ce)
                    let ans = gec_CE_star msg -1 a0
                    (ans, env)
                else
                    dev_println ("Should not make 0 star in " + ceToStr ce)
                    keval_new ww (fun()->mf()+":CE_starzero") env arg_ce


            | CE_ilit(baser, idx) ->
                let (baser, env) = keval_new ww mf env baser
                let (idx, env)   = keval_new ww mf env idx
                (CE_ilit(baser, idx), env)

            | CE_handleArith(dt, baser0, offset0) -> // Address arithmetic is now supported by CE_alu so we do not need this?
                let msg = "CE_handleArith:L3119"
                let (baser, env) = keval_new ww (fun()->mf()+":CE_handleArith-l") env baser0
                let (offset, env) = keval_new ww (fun()->mf()+":CE_handleArith-r") env offset0
                let (dt1, dtidl) =
                    match dt with
                        | CT_cr crst -> (crst.crtno, crst.name)
                let itemsize = ct_bytes1 ww msg dt1
                // Mash up - the aid and base of the base address, the type of the casted.
                let ans = vprintln 0 (sprintf "+++ new code: handleArith itemsize=%i T=%A\n  baser=%A\n  offset=%A" itemsize dt baser0 offset)
                let (newuid, nt, ats, aid) = 
                    match baser with
                        | CE_region(uid, dt, ats, aid, cil_) ->
                            match uid.length with
                                | Some len ->
                                    let newlen = len/itemsize
                                    let newuid  = { uid with length=Some newlen; baser=uid.baser; f_name=funique "handleArith" :: dtidl ; f_dtidl=dtidl; }
                                    (newuid, CT_arr(dt1, Some newlen), ats, aid)
                                |  _ -> sf "L2811z"
                        |  _ -> sf "L2812x"
                let newregion = CE_region(newuid, nt, ats, aid, None)
                let base_aid_o = oapp (anon_deca_for_vassign cilnorm_sitemarker) (ce_aid0 mf baser)
                let ans = gec_CE_alu ww (gec_CT_star -1 dt1, V_plus, newregion, offset)
                vprintln 0 (sprintf "handleArith ans=" + ceToStr ans) // TODO this needs an extra valuetype or reference operator compared with a normal object array since this is essentially a struct array without the C# restrictions on structs.
                (ans, env)
            | CE_ternary(dt, g1, tt, ff) ->
                let (gg, env) = keval_new ww (fun()->mf()+":CE_ternary-gg") env g1
                let gg_const =
                    match gg with
                        | CE_x(_, rv) -> xi_monkey rv
                        | _ -> None
                match gg_const with
                    | None ->
                        let (tt, env) = keval_new ww (fun()->mf()+":CE_ternary-gg") env tt  
                        let (ff, env) = keval_new ww (fun()->mf()+":CE_ternary-gg") env ff
                        (CE_ternary(dt, gg, tt, ff), env)
                    | Some true  -> keval_new ww (fun()->mf()+":CE_ternary-l") env tt
                    | Some false -> keval_new ww (fun()->mf()+":CE_ternary-r") env ff

            | CE_reflection _ -> (ce, env) // These are constants
                
            | other -> sf("keval_new other: " + ceToStr other)


    let disref = ref ""

    let gt_step ww gcode cre0 bpc0 =
        let mf() = "QX33: gt_step"
        let cdec = function
            | T_steps(n, b) -> if n=0 then T_runout else T_steps(n-1, b)
            | other -> other
        let gmsg =  "gt_step " + mmsg
        let m_ib = ref None
        let gt_dic = function
            | Gt_dic (name, len, dic) -> dic
        let gt_len = function
            | Gt_dic (name, len, dic) -> len
        let len = gt_len(gcode)
        let dic = gt_dic(gcode)


        let hbev_emit_jmp_eomethod () = hbev_emit (Xcomment "replace return with branch to end of method")


        let detailed_trace (nemtok:string) =
            let ans = 
              match env.mainenv.TryFind nemtok with
                | None -> "missing"
                | Some vv -> env_itemToStr 3 vv
            dev_println (sprintf " detailed trace %s gives %s" nemtok ans)
                
        let kcode_stepper_instr (count, region, env) bpc =
            let region =
                let region_inc mm maj =
                    vprintln 3 (sprintf "Elaboration regions: region_inc from %i to %i for tid=%s  mm=%s" maj (maj+1) cCP.tid3.id mm)
                    maj+1

                if dic.[bpc].regionchange then region_inc "" region
                else region
            let ins = dic.[bpc].kinstruction
            let lpl = false
            let dis = sprintf "kcode000%i: %s" bpc (kToStr lpl ins)
            let ww = WN dis ww
            disref := dis
      
            let set_linepoint kxr = m_linepoint := kxr
            let mf() = "QX9: gt_step:" + reason_str + ":" + kxrToStr !m_linepoint + " " + dis

            //let _ = detailed_trace "T402.cac_posit.get_asInt.1.9.V_2"

            let _ =
                let vd = !g_gtrace_vd
                if vd>=4 then vprintln 4 (sprintf "\n\n\nkcode_stepper: %A: " reason + mf() + "\n" + dis)            

            // Uncomment to print env on each step
            //let vd = 0 in (vprintln 0 ("Trees pre ins:"); printenv vd 10 "Trees pre ins" env)

            if vd>=4 then vprintln 4 (sprintf "Heap sbrk %s " (ce_hs_envToStr 10 env.hs1))
            let set_linepoint kxr = m_linepoint := kxr
            if vd>=4 then vprintln 4 ( mf())
            let rec run_kbev env ins = 
                set_linepoint (get_kxr ins)
                match ins with
                    | K_pointout _
                    | K_remark _    -> (count, region, env)


                    | K_lab(kxr, _) ->
                        let _ = set_linepoint kxr
                        vprintln 3 (sprintf "set lp to %A" (kxrToStr kxr))
                        //let _ = linepoint5 := Some(s, bpc, region, disref)
                        (count, region, env)

                    | K_topret(kxr, None)  ->
                        let v = xmy_num 0
                        match topret_mode with
                            | (true, _)     -> ()
                            | (_, None)     -> hbev_emit (Xreturn v)
                            | (_, Some _)   -> hbev_emit_jmp_eomethod()
                        (count, -1001, env)
                        
                    | K_topret(kxr, Some ce)  ->
                        let (dd, env) = keval_new ww (fun()->mf()+":K_topret") env ce
                        let v = LL "g-K_topret" mf dd
                        //let _ = cCC.topret_val := Some v // old way in CC - dont like
                        let _ =
                            match topret_mode with
                                | (true, _)          -> ()
                                | (_, None)          ->  hbev_emit (Xreturn v)
                                | (_, Some ax) ->
                                    match ax with
                                        |  None      -> ()
                                        |  Some rvl  ->
                                            hbev_emit (Xassign(f3o4 rvl, v))
                                            if vd>=4 then vprintln 4 ("Logged topret return value " + xToStr v)
                                    hbev_emit_jmp_eomethod()

                        (count, -1001, env)

                    | K_easc(kxr, CE_server_call(_, instancef, cf, sargs, scv, args), ef) -> // This should not be used - instead restructure invokes servers as per other FUs
                        let ww = (WN "CE_server_call" ww)
                        let nort x = fst(keval_new ww (fun()->mf()+":CE_server_call") env x)
                        //let nort_lmode x = fst(keval_lmode ww (fun()->mf()+":CE_server_call") env x)
                        let d2 = (nort) //, nort_lmode)
                        let mm = "server call"
                        let simple_assign lhs rhs =
                            ignore(assign_codegen1t kxr env d2 0 mf None lhs None rhs { abuse=None; reset_code=false })
                            ()
                        let lX = LL "g-CE_server_call" mf
                        let gen_waituntil mf e =
                            vprintln 3 ("Waitu waituntil " + xbToStr e)
                            if gdp.gen<>None then G_emit (valOf gdp.gen) (Xwaituntil e)
                        make_hsimple_server_call ww kxr cf instancef (lX, simple_assign, gen_waituntil mm) (scv, map (lX) sargs, map nort args)
                        (cdec count, region, env)

                    | K_easc(kxr, CE_start(false, callstring, signat_etc, objptr, args, cgr0), _) -> // A note of higher-order or dynamic method dispatch.
                        let msg = "keval dynamic method dispatch (aka startcall) + " + hptos callstring
                        let keval_nort x =
                            //vprintln 0 (sprintf "  Start knort") 
                            let ans = fst(keval_new ww mf env x)
                            ans
                        let func_dt =
                            match signat_etc with
                                | RN_monoty(ty, _) :: _ -> ty
                                | other -> sf (sprintf "CE_start: mal dropping for %s %A " msg other)
                        //vprintln 0 (sprintf "Start entry_point_eval objptr=" + ceToStr objptr)
                        //vprintln 0 (sprintf "Start entry_point_eval")                        
                        let ce0 = keval_nort(gec_field_snd_pass ww (cilnorm_sitemarker + "EP") msg func_dt objptr "entry_point")
                        //vprintln 0 (sprintf "Finished entry_point eval ans=" + ceToStr ce0)
                        //vprintln 0 (sprintf "Start closure eval")
                        // This dynodispatch code is split between intcil delegate invoke and gtrace CE_start procedures.
                        // We need to enumerate both the called class instance and the called method names, assigning an integer value that potentially get handled at runtime, as in test17r2.
                        let that__ = keval_nort(gec_field_snd_pass ww (cilnorm_sitemarker + "CL") msg func_dt objptr "closure")  // The closure currently ignored, but please fold in to the dyno_dispatch closures list.
                        //vprintln 0 (sprintf "Finished closure_eval")
                        let rec fn_get = function
                            | CE_star(_, inner, _)                   -> fn_get inner
                            | CE_conv(_, _, inner)                   -> fn_get inner
                            // We now expect reflection token here rather than the old region kludger.
                            | CE_reflection(_, None, Some idl, _, _) -> [idl]
                            | CE_region(uid, dt, ats, _, cil)        -> [uid.f_name]

                            | other -> // If we cannot find a named ep constant here we get all entry points within the storage class and assume all might get called at this site,
                                let scl = ce_to_scl msg other
                                //vprintln 0 (sprintf "Group hof scl %A  " scl)
                                let candidates =
                                    let collate_targets_in_same_sc cc (idl, ce) =
                                        if list_intersection_pred(ce_to_scl msg ce, scl) then ce::cc else cc
                                    List.fold collate_targets_in_same_sc [] !cCC.reflection_constants

                                //vprintln 0 (sprintf "Group hof candidates are %s" (sfold ceToStr candidates))
                                list_once(list_flatten(map fn_get candidates))
                                //sf (msg + sprintf ": other form method name. Higher-order programming too complex for KiwiC?  Simple other=%s\nfull=%s\nother=%A" (ceToStrx true other) (ceToStr other) other)
                        let idls = fn_get ce0
                        let prev = cCC.dyno_dispatch_methods.lookup callstring
                        let p1 = map snd prev
                        let new_callees =
                            let not_already_known cc idl = if memberp idl p1 then cc else idl::cc 
                            List.fold not_already_known [] idls
                        let xhptos (no, name) = sprintf "  %i: %s" no (hptos name)
                        let _ = 
                            if not_nullp new_callees then
                                let baser = length prev
                                let newpops =
                                    let add_no idl =
                                        match op_assoc idl !cCC.reflection_constants with
                                            | Some ce ->
                                                match ce with
                                                    | CE_reflection(_, lo, Some idl, Some vo, _) -> (vo, idl)
                                                    | other -> sf (sprintf "add_no other form %A" other)
                                            | None -> sf (sprintf "add_no None")
                                    map add_no new_callees
                                app (fun n -> cCC.dyno_dispatch_methods.add callstring n) newpops
                                cCC.dyno_dispatch_updated := true
                                vprintln 3 (sprintf "Call graph not yet closed. dyno_dispatch updated at %s -> new_callees=%s old_callees=^%i %s" (hptos callstring) (sfold xhptos newpops) (length prev) (sfold xhptos prev))
                                ()
                            else
                                vprintln 3 (sprintf "Call graph is closed at %s ->  old_callees=^%i %s" (hptos callstring) (length prev) (sfold xhptos prev))
                                () // When all are already noted, no further action at this point.
                        (cdec count, region, env)


                    | K_easc(kxr, CE_start(true, callstring_, signat_etc_, obj, args, cgr0), _) -> // Start/spawn thread.
                        let msg = ":thread-Start"
                        let mf() = mf() + msg
                        let keval_nort x = fst(keval_new ww mf env x) // captures env prior to arg bind - any particular reason? Just poor?
                        let rec fap e = function
                            | [] -> ([], e)
                            | h::tt ->
                                let (r, e') = fap e tt
                                let (d, e'') = keval_new ww mf e' h
                                dev_println (sprintf "CE_start: Thread start arg " + ceToStr d)
                                (d::r, e'')
                        let (args, env') = fap env args

                        zeng_threadstart msg keval_nort (keval_nort obj, args, env')
                        (cdec count, region, env')

                    | K_easc(kxr, CE_apply(CEF_bif bif, cf, [a1]), ef) when bif.uname= [ "Dispose"; "Kiwi"; "KiwiSystem" ] -> // Both static and instance dispose have one arg.                
                        vprintln 3 (sprintf "Explicit dispose call to %s arg=%s" (hptos bif.uname) (ceToStr a1))
                        let a1 = fst(keval_new ww (fun()->mf()+":Dispose") env a1)
                        //vprintln 3 (sprintf "Explicit dispose call to %s env=%A" (hptos bif.uname) env)
                        let env' = obj_dispose ww env a1
                        (cdec count, region, env')
                    
                    | K_easc(kxr, CE_apply(CEF_bif bif, cf, args), ef) when hd bif.uname = "Pause" -> // TODO - process some hard/soft args ? TODO - split into separate tidy fn
                        let mm = "Pause() applied"
                        if ef then hbev_emit(Xeasc(lower_kiwi_pause_call ww (map (LL "g-CE_apply-Pause" mf) args)))
                        (cdec count, region, env) 

                    | K_easc(kxr, CE_apply(CEF_bif bif, cf, [a0; a1]), ef) when hd bif.uname="InitializeArray" -> 
                        let msg = "InitializeArray"
                        let atype = get_ce_type msg a0 
                        let nort ce = fst(keval_new ww (fun()->mf()+":InitializeArray") env ce)
                        //let nort_lmode ce = fst(keval_lmode ww (fun()->mf()+":InitializeArray") env ce)
                        let d2 = (nort) // , nort_lmode)
                        // We trap this array init here since the implementation is in here too at the moment, but should better be pushed into kevel_new standard handlers...
                        // There's a big difference between 1D and higher dim arrays: a0 is the array for 1D but is a class wrapper for highers
                        let mf() = mf()+":kcode_stepper_instr InitializeArray"
                        //vprintln 3 (msg + sprintf ": aid before deca " + sfold aidToStr aid)
                        let aid_for_1d = oapp (anon_deca_for_vassign cilnorm_sitemarker) (ce_aid0 mf a0) // Get pre-eval (on static analysis basis).
                        let (a0', env) = keval_new ww mf env a0 // a1 is not eval'd since we currently assume it is a table of constant data but this would/should do no harm.
                        let (xx, env') = InitializeArray kxr env d2 mf (atype, aid_for_1d, a0', a1)
                        (cdec count, region, env')

                    | K_easc(kxr, ce, emitf) ->
                        let (ce', env) = keval_new ww (fun()->mf()+":K_easc") env ce
                        vprintln 3 (sprintf "K_easc codegen " + ceToStr ce')
                        let _ =
                            match LL "K_easc codegen" mf ce' with
                                   | X_undef -> ()
                                   | xe -> if emitf then hbev_emit(Xeasc xe)
                        (cdec count, region, env)
                        
                    | K_goto(kxr, bcond, sl, pp) ->
                        let p = if !pp < 0 then resolve_gt_goto_late sl pp gcode else !pp
                        let d ce = fst(keval_new ww (fun()->mf()+":K_goto") env ce)
                        let lX = LL "K_goto" mf
                        //vprintln 3 ("goto l=" + xToStr ll + "   r=" + xToStr rr)

                        let nib_tailer cond = 
                            if bconstantp cond then 
                                if cond=X_true then
                                    (vprintln 3 (regionToStr region + ": kcode branch condition: unconditionally following conditional branch to " + hptos sl);
                                     (X_true, None, Some p)
                                    )
                                elif cond=X_false then
                                    ( vprintln 3 (regionToStr region + ": kcode branch condition: conditional branch not taken (" + xbToStr cond + ")");
                                      (X_false, Some(bpc+1), None)
                                    )
                                else sf("bool const is not true or false : (" + xbToStr cond +  ").\n")
                            else 
                                vprintln 3 (regionToStr region + ": save ib: bne taken=" + xbToStr cond)
                                (cond, Some (bpc+1), Some p)

                        let nib =
                            match bcond with
                            | None ->
                                vprintln 3 (regionToStr region + ": jump to " + hptos sl);
                                (X_true, None, Some p)
                                
                            | Some(BCOND_diadic(dop, l, r)) ->
                                let ll = lX (d l)
                                let rr = lX (d r)
                                let branch_cond = ix_bdiop dop [ll; rr] false
                                nib_tailer branch_cond

                            | Some(BCOND_guarded(negf, ge, nemtoks)) ->
                                let msg = "BCOND_guarded"
                                let overrider =
                                    if nullp nemtoks then None
                                    else
                                        // Note - this lookup must be done after all kcode is generated, which might not be the case for multi-threaded code: TODO.
                                        let has_null =
                                            let gotnull nemtok =
                                                let sci = ksc.classOf_i  "BCOND_guarded" (A_loaf nemtok)
                                                ksc.lookup_null_flag  sci
                                            disjunctionate gotnull nemtoks
                                        vprintln 3 (sprintf "BCOND_guarded negf=%A has_null for %s is %A" negf (sfold (fun x->x) nemtoks) has_null)
                                        if has_null then None
                                        else Some (if negf then X_true else X_false) // If we are testing a domain that does not contain null for being null, we report false.
                                        
                                let (ge', env) = keval_new ww (fun()->mf()+":K_bgoto") env ge
                                let lX = LL msg mf
                                let branch_cond =
                                    if false && not_nonep overrider then // For now
                                        vprintln 3 (sprintf "discard condition %s" (xToStr (lX ge')))
                                        //if negf then xi_not(xgen_orred(lX ge')) else xgen_orred(lX ge')
                                        valOf overrider
                                    elif negf then xi_not(xgen_orred(lX ge')) else xgen_orred(lX ge')
                                nib_tailer branch_cond
                        // When destacking the CILcode, branches were deferred until after the spillvar correction.  Here the branch/goto will either be interepreted or emitted at caller's choice.
                        m_ib := Some nib
                        //if vd>=4 then vprintln 4 (sprintf "save ib instruction branch information.")
                        (cdec count, region, env)


                    | K_cmd(kxr, k)      ->
                        // "K_cmd " + hbevToStr e
                        if unimportant k then () else hbev_emitter gdp k
                        (cdec count, region, env)

                    | K_new(kxr, lhs, nemtok, newarg) ->
                        let sci = ksc.classOf_i "K_new" (A_loaf nemtok)
                        let static_cache_f = ksc.attribute_get g_assign_once_cache_var sci
                        let no_escape_f = cCC.kcode_globals.ksc.attribute_get g_no_escape_mark sci
                        // We have 3 heuristics for diverting to compile-time heap past end of static elaboration:
                        // 1. When the basic block is visited only once in the control flow (cannot be re-executed)
                        // 2. When the static-assign-one heuristic matches on mg$cache fields
                        // 3. Where the argument to K_new is manifestly uncopied (does not escape) (happens for thread delegates).

                        vprintln 3 (sprintf "K_new: nemtok=%s: static_cache_f=%A  no_escape_f=%A" nemtok static_cache_f no_escape_f)
                        let (rhs, env) = 
                            match newarg with // These can alter the heap as a side effect
                                | KN_newobj _ -> do_newobj ww mf env nemtok (not_nonep no_escape_f || not_nonep static_cache_f) newarg
                                | KN_newarr(ct, len0, aid_content, hintname_o, ats) ->
                                    let m = ":keval CE_newarr"
                                    let deval_new e ce = keval_new ww (fun()->mf()+m) env ce
                                    let (len1, env) = deval_new env len0
                                    create_array ww env (mf, m) (ct, len1, nemtok, hintname_o, ats) 
                        let aso = { abuse=None; reset_code=false }
                        let expanded = K_as_sassign(kxr, lhs, rhs, aso)
                        run_kbev env expanded

                    | K_as_vassign(kxr, lhs, rhs, subs, aid_o, aso) ->
                        let lhs', env  = keval_new ww  (fun()->mf()+":K_as_vassign-the-lhs") env lhs
                        let rhs', env  = keval_new ww  (fun()->mf()+":K_as_vassign-the-rhs") env rhs
                        let subs', env = keval_new ww  (fun()->mf()+":K_as_vassign-the-subs") env subs
                        let d1 ce = fst(keval_new ww (fun()->mf()+":vassign d1") env ce)
                        //let d1_lmode ce = fst(keval_lmode ww (fun()->mf()+":vassign d1_lmode") env ce)
                        let env' = vassign0t kxr env (d1) mf (lhs', rhs', (subs', aid_o), aso) 
                        (cdec count, region, env')


                    | K_as_sassign(kxr, CE_subsc(_, lhs, subs, aid2), rhs, aso) 

                    // A scalar assign to a star is not a vassign owing to the need to scale subscripts on a vassign.
                    // A scalar assign to a subscription is ...?
                    | K_as_sassign(kxr, CE_star(1, CE_subsc(_, lhs, subs, aid2), _), rhs, aso)  ->
                        // TODO deprecate this form entirely!
                        let aid = no_aid_legacy (fun()->mf()+":K_as_vassign") lhs // TODO this should also best be a property of the subscript - might as well be none here? Although lhs itself can change with the eval here. 
                        let lhs', env =  keval_new ww (fun()->mf()+":K_as_latent_vassign-the-lhs") env lhs
                        let rhs', env =  keval_new ww (fun()->mf()+":K_as_latent_vassign-the-rhs") env rhs
                        let subs', env = keval_new ww (fun()->mf()+":K_as_latent_vassign-the-subs") env subs
                        let d1 ce = fst(keval_new ww  (fun()->mf()+":latent vassign d1") env ce) // TODO discards side effects
                        //let d1_lmode ce = fst(keval_lmode ww  (fun()->mf()+":latent vassign d1_lmode") env ce)
                        vprintln 3 ("   latent rhs'=" + ceToStr rhs')
                        vprintln 3 ("   latent lhs'=" + ceToStr lhs')
                        vprintln 3 ("   latent subs'=" + ceToStr subs')
                        let env' = vassign0t kxr env d1 mf (lhs', rhs', (subs', aid2), aso) 
                        (cdec count, region, env')
                        
                    | K_as_sassign(kxr, lhs, rhs, aso)  -> // Static or field assign.
                        let (rhs', env) = keval_new ww (fun()->mf()+":K_as_sassign-rhs") env rhs
                        let d1 ce = fst(keval_new ww (fun()->mf()+":K_as_sassign") env ce)
                        let env' = sassign0 kxr env (d1) mf (None, lhs, rhs', aso)
                        (cdec count, region, env')

                    | K_pragma(kxr, fatalf, cmd, a0, a1) ->
                        let _ = process_pragma ww reason mf ("kcode_stepper") cCP bpc count (fatalf, cmd, a0, a1)
                        (cdec count, region, env)
                    | other -> sf ("other kcode in kcode_stepper_instr: " + kToStr true other)
            run_kbev env ins


        let rec kcode_stepper_bblock cre pc =  // Iterate through a kcode basic block (until epc or ib branch occurs)
            if pc >= len then (m_ib := Some(X_dontcare, None, None); cre)
            else
            //vprintln 0 "--------------------------------- ----------------------------- ---------------------------------- ----"
            //printenv 0 10 "pre kcode_stepper_instr" (f3o3 cre)  // Debug print env - for finding where assign is lost.                
            let cre' = kcode_stepper_instr cre pc
            //dev_println (sprintf "kcode_lower_block: stepper_bblock: post step: pc=%i(epc=%i) Region'=%i" pc epc (f2o3 cre'))
            //printenv 0 ("post kcode_stepper_instr" (f3o3 cre')  // Debug print env - for finding where assign is lost.
            //dev_println (sprintf " Compare pc=%i with epc=%i" pc epc)
            if pc = epc then cre'
            elif nonep !m_ib then kcode_stepper_bblock cre' (pc+1)
            else cre'

        let ans = kcode_stepper_bblock cre0 bpc0
        (ans, !m_ib) 

    let bpc = snd(snd callsite)
    let ((count', region', env'), ib) = gt_step (WN "gt_step" ww) compiled cre bpc
    ((count', region', env'), ib) // end of kcode_lower_block

//
//




let discard_side_effects cCP (gdp:gdp_t) =
    gdp.m_tthreads := []
    ()
    
let commit_side_effects cCP (gdp:gdp_t) =
    cCP.m_spawned_threads := !gdp.m_tthreads @ !cCP.m_spawned_threads
    gdp.m_tthreads := []
    ()


//
// Perform interpret of the kcode to find first branch that is run-time conditional or first region change.
// This finds the lasso stem for a thread. The thread's runtime entry point will be what remains, starting from the first control flow back edge that is not unwound.
let kcode5_static_elaborate_stem ww linepoint_stack tT (cre, (tid3:tid3_t), topret_mode, basic_blocks, justify2) kcode =
    let vd = 3 // TODO from recipe
    let n_blocks = length basic_blocks
    let m = sprintf "kcode5_static_elaborate of tid=%s no_basic_blocks=%i" (tid3.id) n_blocks
    let _ = WF 3 m ww "commence"
    let (cCP, cCCL, (cCB:cilbind_t), gdp:gdp_t) = tT
    let cCC = valOf !g_CCobject
    let tid = tid3.id
    let mc(pc) = "PC" + tid + ":" + i2s pc // generate a restriction marker
    let gt_len = function
       | Gt_dic (name, len, dic) -> len

    let msg = sprintf "Make sure your program does not fully run at compile time by inserting a Kiwi.Pause or read of run-time data at your end-of-elaboration point.  You can increase the unwind budget in the recipe or using -cil-uwind-budget=N and please note this is not the same as the similar bevelab-budget (or compose-budget) in othe recipe stages.\nSee the obj/h2 logs for where we'd got to.\nValue was %i" (valOf !g_kfec1_obj).cil_unwind_limit

    let a5 m1 = vprintln 3  (m + ": " + m1)
    let len = gt_len kcode
    let visitlog = Array.create len None

    let linepoint5 = { waypoint=ref None;  callstring__= ref []; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=linepoint_stack; }
    
    let rec elab5_step ww (age, bpc, count, region, env:env_t) = // This is the compile-time control flow elaborator (ie interpreter) that yields the linear lasso hgen sequence.
        match count with
        | T_runout -> cleanexit(cil_lpToStr linepoint5 + ": KiwiC lasso stem unwind: ran out of steps\n" + msg)
        | T_steps(steps, budget) ->
            if steps=0 then cleanexit(cil_lpToStr linepoint5 + ": KiwiC lasso stem unwind error: ran out of steps\n" + msg)
            else
            linepoint5.codept := Some([], bpc, region, ref "")
            let m1 = "Block start. " + mc(bpc) + " region=" + regionToStr region + ")"       
            let ww' = WF 3 m ww m1
            let lp = LP(mc bpc, -1) 
            G_emit (valOf gdp.gen) (Xlinepoint(lp, Xskip)) 
            let gt_exec5 cre bpc = 
                let (bpc', epc) = justify2 bpc
                a5 ("elab5: Log visit to " + i2s bpc' + " at age=" + i2s age)
                Array.set visitlog  bpc' (Some(age, env, !(valOf gdp.gen).idx)) // Save starting env and code generation point in the array.
                let thrd_callsite = (tid3, ([tid3.id], bpc))
                let apflag = true
                let (cre', ib) = kcode_lower_block ww' KER_lasso_stem vd m cre tT apflag kcode (linepoint5, thrd_callsite, topret_mode, epc) // stem-find call site
                (cre', ib, epc)
            let ((count', region', env'), ib, epc) =
                if bpc >= len then 
                     ((count, -444, env), None, bpc)
                else gt_exec5 (count, region, env) bpc
            //dev_println (sprintf "Return lasso stem step: region'=%i" region')
            if region' < 0 then
                a5 (sprintf "elab5: Return or normal exit encountered in starting region during stem interpret.  age=%i. No code from this thread remains to run at run-time." age)
                commit_side_effects cCP (gdp)
                (false, "return", env') // Return true for end_of_elabf if successful completion of all blocks, hence no roll back needed.
                
            elif region' <> region then // If region has changed then we have found end of static elaboration.
                   a5 (sprintf "elab5: New region encountered: bpc=%i age=%i   old=%i new=%i" bpc age region region')
                   discard_side_effects cCP (gdp)
                   (true, "regionchange", env')

            else
                match ib with
                | None ->
                    commit_side_effects cCP (gdp)
                    a5 (sprintf "elab5: continue after no break: block pc=%i at age=%i" epc age)
                    elab5_step ww (age+1, epc+1, count', region', env')
                
                | Some(_, Some pc, None) // If branch direction is manifestly constant we continue stem lengthening.
                | Some(_, None,  Some pc) ->
                    commit_side_effects cCP (gdp)
                    a5 (sprintf "elab5: continue with naturally-chained basic block pc=%i at age=%i" pc age)
                    elab5_step ww (age+1, pc, count', region', env')
                | Some(cond, Some ff_pc, Some br_pc) -> 
                         // Do not commit side effects since this block will be re-encountered at runtime after projecting by render.
                         discard_side_effects cCP (gdp)
                         a5(sprintf "elab5: conditional branch from %i to %i at age %i" bpc br_pc age)
                         (true, "condbranch", env')
                | Some(_, None, None) ->
                         a5 (sprintf "elab5: Ran off end of thread code during stem interpret.  age=%i. No code from this thread remains to run at run-time." age)
                         commit_side_effects cCP (gdp)
                         (false, "ranoff", env')

    let (end_of_elabf, env') =
        let bpc = 0 // Entry point.
        let (count, region, env) = cre
        a5 ("elab5: Start generate lasso stem static elaborate")
        let (end_of_elabf, reason, env') = elab5_step (WN "elab5_step" ww) (0, bpc, count, region, env) 
        a5 (sprintf "elab5: Finished generating static elaborate trajectory rc=%s  end_of_elabf=%A" reason end_of_elabf)
        (end_of_elabf, env')

    // The output env we return will be that returned by the last block statically elaborated.
    // If the last block completed without an end-of-elab marker, we return its output env.  
    // The most-advanced is not numericall the highest pc, it is the one with the most highest vist time.
    // An array of all of them is not needed - we could keep the running highest finished block in a scalar.
    if end_of_elabf then
        let find_most_advanced (pold, c) p =
            vprintln 3 ("find_most_advanced p=" + i2s p)  
            match visitlog.[p] with
                | None  ->
                    //dev_println (sprintf "hwm: nothing stored for %i" p)
                    (pold, c)
                | Some y ->
                    //dev_println (sprintf "hwm: Consider bpc=%i with age %i gidx=%i"  p (f1o3 y) (f3o3 y))
                    if (f1o3 y > f1o3 c) then (p, y) else (pold, c)
        let ans = List.fold find_most_advanced (0, (-1, f3o3 cre, 0)) (map f1o4 basic_blocks)
        a5(sprintf "Most advanced basic block start was %i (total no blocks=%i)" (fst ans) n_blocks)
        (end_of_elabf, ans)
    else
        let aa = (end_of_elabf, (-1, (-1, env', -1))) // end of kcode5_static_elaborate_stem
        aa

let justifier2 basic_blocks xx = // Align a kcode address, such as a branch/jump dest, to basic block nominal start point.
    let rec jj = function
        | (a, epc, jpf_)::tt -> if xx <= a then (a, epc) elif xx <= epc then sf(sprintf "mid block justify xx=%i" xx) else jj tt
        | _ ->
            dev_println (sprintf "justify kcode block boundary - no block for %i" xx)
            (xx, xx)
    jj basic_blocks


// elab5_render performs an hgen render of one basic block in hbev_t form. Inter-block control flow is ...
// This creates the main output from KiwiC frontend.  
// Blocks which flow from one to another can be rendered in natural order to nicely manifest that behaviour or explicit linking gotos can be issued.
// When jpflag holds we issue jump to epc instead of ...
let elab5_render ww linepoint_stack tT justify (tid3, topret_mode, kcode, hwm) cre (spc, epc, jpflag, apflag) nexto =
    let m = "elab5_render " + (tid3.id)
    let _ = WF 3 m ww "commence"
    let (cCP, cCL, (cCB:cilbind_t), gdp) = tT
    let cCC = valOf !g_CCobject
    let vd = cCC.settings.hgen_loglevel
    let gG = valOf gdp.gen
    let mc(pc) = "PC" + (tid3.id) + ":" + i2s pc // generate a restriction marker
    let gt_len = function
       | Gt_dic (name, len, dic) -> len

    let a5 m1 = vprintln 3 (m + ": " + m1)
    let len = gt_len kcode

    let m1 = m + ": Block start. " + mc(spc)  
    let ww' = WF 3 m ww m1
    let blab nn = (tid3.id) + "_" + i2s nn

    let dlabel n =
        let x = blab n
        if vd>=4 then vprintln 4 ("hpr emit at " + G_idx gG + ": label " + x);
        gec_Glabel gG x

    dlabel spc

    let linepoint5 = { waypoint=ref None;  callstring__= ref [ m1 ]; codept= ref None; srcfile=cCB.srcfile; linepoint_stack=linepoint_stack;  }
    let natural_modef = false // breaking test4
    let rec elab5r_step ww (count, region, env:env_t) (bpc, epc, jpflag, apflag) = // hgen step
            let lp = LP(mc bpc, -1) 
            log_linepoint lp
            G_emit gG (Xlinepoint(lp, Xskip)) 
            let gt_exec cre bpc = 
                let callsite = (tid3, ([tid3.id], bpc))
                if vd>=4 then vprintln 4 (sprintf "hgen bpc=%i epc=%i nexto=%A" bpc epc nexto)
                //printenv 0 10 ("render to output") (f3o3 cre) 
                let (cre', ib) = kcode_lower_block ww' KER_code_generation vd m cre tT apflag kcode (linepoint5, callsite, topret_mode, if true || jpflag then epc else -1) // rendering to output
                (cre', ib)
            let ((count', region', env'), ib) = gt_exec (count, region, env) bpc

            let dgoto allow_fallthroughf dest =
                let lab = blab(justify dest)
                let supressf = allow_fallthroughf && (nonep nexto || f1o4(valOf nexto) <> dest)
                if supressf then
                    if vd>=4 then vprintln 4 (sprintf "dgoto: supress hpr emit of fallthrough to %i %s not available %A" dest lab nexto)
                else
                    if vd>=4 then vprintln 4 ("hpr emit at " + G_idx gG + ": goto " + lab)
                    gec_Ggoto gG lab

            let dbne g dest =
                    match g with
                        | X_true -> dgoto false dest
                        | g ->
                            let lab = blab(justify dest)                            
                            if vd>=4 then vprintln 4 ("hpr emit at " + G_idx gG +  ": bne " + xbToStr g + " " + lab)
                            gec_Gbne gG g lab

            if false && jpflag then dgoto false (epc+1) // When jpflag holds there is an implied transfer to the following code point which we here make explicit. No - it is an input from the previous flow.
            else match ib with
                    | None ->
                        a5("other stop - epc")
                        if not natural_modef && epc+1 < hwm then dgoto natural_modef (epc+1) // Stopped at epc
                        ()


                    | Some(_, Some pc, None)
                    | Some(_, None,  Some pc) ->
                        a5("jump from " + i2s bpc + " to " + i2s pc)
                        dgoto natural_modef pc
                        ()
                        
                    | Some(cond, Some ff_pc, Some br_pc) -> 
                        a5("conditional branch from " + i2s bpc + " to " + i2s br_pc)
                        dbne cond br_pc 
                        dgoto natural_modef ff_pc
                        ()

                    | Some(_, None, None) ->
                        a5("ran off end")
                        ()  // thread run off end of program

    let (count, region, env) = cre
    a5 (": Start hgen generate traj " + i2s spc + " .. " + i2s epc)
    let ans = elab5r_step (WN "elab5r_step" ww) (count, region, env)  (spc, epc, jpflag, apflag)
    commit_side_effects cCP (gdp)
    ()
    



(*
 * Create an elab context, introductin some initial bindings.
 *)
let gec_cil_blk binds env = (binds, [], [], [], [], [], env, [], [], []) // TODO use me more

(*
 * See whether this method is marked up as an assertion.
 *)
let assertionmethod ww cgr = function
    | No_method(srcfile, flags, ck, gid, mdt, flags1_, instructions, atts)  -> 
       let vd = 3
       let dekin = function
            | W_string(s, _, _) -> s 
            | (other) -> sf("assertionmethod dekin other " + xToStr other)
 
       let rec scan = function
            | CIL_custom(s, t, fr, args, vale)::tt -> 
                let (_, idl) = fr33 ww true cgr fr
                let idl = if hd idl = ".ctor" then tl idl else idl
                let ss = if idl=[] then "" else hd idl
                let ya = isPrefix "AssertCTL" ss
                if ya then Some(custat_argmarshall ww vd ss (args, vale)) else scan tt
            | (_) -> None
       let rr = scan instructions
       rr
    | (_) -> sf ("assertionmethod not a method")




let gec_preliminary_director ww timespec (directorate_style, directorate_ats) = 
    let msg = "gec_preliminary_director"
    let (duido, hangmode) =
        match op_assoc "directorate-endmode" directorate_ats with
            | Some hang_mode_str ->
                match parse_protocol_hangmode ww msg directorate_ats hang_mode_str with
                    | Some hang_mode -> (None, hang_mode)
                    | other -> sf (msg + sprintf ": other directorate-endmode not recognised: %A" other)
            | other -> sf (msg + sprintf "Mising directorate-endmode attribute")
    (duido, timespec, directorate_style, directorate_ats, hangmode)



let kiwife_rez_director ww clk_override clknets handshakes (duido, timespec, directorate_style, directorate_ats, hangmode) =  // This can be in gbuild really?
//let gec_preliminary_director_really ww timespec (directorate_style, directorate_ats) = 
    // handshakes are popped in later.
    let duid = if nonep duido then next_duid() else valOf(duido)
    let msg = sprintf "kiwife: Build directorate %s" duid
    vprintln 3 msg
    let (clocks, resets, clk_enables) = clknets

    // If user wants a clock enable they can request it via Kiwi.ClockDom or via future recipe settings that override the construction of g_default_clknets
    //let clk_enables = (if directorate_style=DS_advanced then [xgen_gend tag g_dir_ext_run_enable] else [])

    let tag = None // TODO need to sort out unary led autowiring!
    let unary_leds =
        if directorate_style=DS_minimal then None
        else
            let tag = Some(duid)
            let width = if directorate_style=DS_minimal then 1 else 8
            let unary_leds_net = xgen_gend tag ("hpr_unary_leds", {signed=Unsigned; widtho=Some width}, OUTPUT, Some 1)
            Some unary_leds_net
    { g_null_directorate with
                  pc_export=       not(op_assoc "directorate-pc-export" directorate_ats = Some "disable")
                  duid=            duid
                  style=           directorate_style
                  clocks=          if not_nullp clk_override then clk_override else clocks
                  resets=          resets
                  abend_register=  (if directorate_style=DS_normal || directorate_style=DS_advanced then Some(xgen_gend tag g_directorate_abend) else None)
                  clk_enables=     clk_enables
                  self_start =     op_assoc "directorate-startmode" directorate_ats = Some "self-start"
                  hang_mode=       hangmode
                  unary_leds=      unary_leds
                  ats=             directorate_ats
                  performance_target= timespec
                  handshakes=      handshakes
    }         


//
// Generate accelerator mode and RPC server-side entry point: connections and top and tail code. A top-level that polls the 'req' line under HSIMPLE and an extension to the directorate in HFAST.
// Such an entry point requires additional top arguments - there is no 'internal instantiation' for RPC entries (at the moment).
//   
let rpc_server_entryp ww cCB (director_spec_in) (idl, cgr, mdt) this_o =
    let cCC = valOf !g_CCobject
    let voidf = mdt.rtype=CTL_void
    let signat = mdt.arg_formals

    let (prams, (protocol_prams, protocol_str), searchbymethod, externally_instantiated, fsems) = valOf_or_fail "L4883" mdt.is_remote_marked
    let (duido, timespec_, director_style, director_ats, hangmode) = director_spec_in

    let overload_suffix = if fsems.fs_overload_disam then "_" + squirrel_port_signature (map f3o4 signat) else ""
    let ba_name = hd idl         // We want the name of the port, the pi_name, to be just the method name, not the top-level component it is a method of, hence hd here. This also discards namespaces. Finally, it make the bus abstraction name the same as the pi_name which is ok for AUTOMETAs but convergence with a member of a standard library protocol may be desirable in the future. HSIMPLE used the whole thing - might restore for ancient back compatibility?
    let abstraction_name = protocols.g_autometa_prefix + filename_sanitize [] (ba_name + overload_suffix)
    let msg = (sprintf "Starting, abstraction_name=%s voidf=%A add_inhold=%A overloaded_disam=%A" abstraction_name voidf fsems.fs_inhold fsems.fs_overload_disam)
    let ww = WF 3 ("rpc_serve_entryp idl=" + hptos idl) ww msg
    let ids = hptos idl
    // TODO do this cpi gen inside generate_rpc_cs_terms
    let (pi_name, cpi) =
        let sql = if overload_suffix = "" then [] else [ overload_suffix ]        
        let mp1 = if overload_suffix = "" then [] else [ ("overloadSuffix", overload_suffix) ]
        let mp2 = [ ("methodName", ba_name) ]
        //let pi_name = hptos(sql @ [ba_name])
        let pi_name = ba_name + overload_suffix
        (pi_name, { g_null_db_metainfo with kind=abstraction_name; pi_name=pi_name;  not_to_be_trimmed=true; form=DB_form_external; metaprams=mp1 @ mp2 })


#if SPARE            
        let dp =
            {     max_outstanding= -1
                  posted_writes_only= false
                  ack_present=        false
                  req_present=        false              
                  reqrdy_present=     false
                  ackrdy_present=     false
            }
              
        match protocol_str with
            | "IPB_FPFL" -> // Fully-pipelined, fixed latency.
                (dp, protocol_str) // Processed later stages ...

            | "HSIMPLE" ->
                (dp, protocol_str) // dp in this clause here gets ignored so any darn'd thing.

            | protocol_str ->
                let protocol = rax_hfast_mainschema ww msg protocol_str
                protocol
#endif
    //dev_println (sprintf "server protocol=%A" protocol)
                
    let (retnet, hs_nets, arg_nets, top_kcode, busDef_handy_info) =
        let root_kxr = KXREF("rpc_entrypoint_start", idl)
        let formal_port_idl = [ hd idl ]
        vprintln 2 (sprintf "Formal port %s is denoted just as %s" (hptos idl) (hptos formal_port_idl)) // We do not want the server kind name in its formal port nets.
        generate_rpc_cs_terms ww (Some cCB) root_kxr g_dummy_realdo (Some cpi) (formal_port_idl, signat, mdt.rtype, overload_suffix) (protocol_prams, protocol_str) (*client_direction=*)false fsems


    let (top_xcode, tail_xcode, handshakes) =

        match protocol_str with
            | "IPB_FPFL" -> 
                ([], [], []) 

            | "HSIMPLE" ->
                let req = scall_getnet "req" hs_nets
                let ack = scall_getnet "ack" hs_nets                
                
                let clockfence = Xeasc(gen_hard_pause_call ()) // A Hard Pause - wait for one clock cycle.
                let lxu = funique "server_dispatch"
                let s1code = [ Xlabel lxu; (*clockfence; *) Xwaituntil(xgen_orred req);]
                let s2code = [ Xassign(ack, xi_one); (* clockfence; *) Xwaituntil(xi_not(xgen_orred req)); ] 
                let s3code = [ Xassign(ack, xi_zero); Xgoto(lxu, ref -11) ]
                let handshakes = [ req; ack ]
                if voidf then (s1code @ s2code, s3code, handshakes) else (s1code, s2code @ s3code, handshakes)

            | protocol_str ->
                let a1 = if protocol_prams.req_present then [scall_getnet "req" hs_nets] else []
                let a2 = if protocol_prams.ack_present then [scall_getnet "ack" hs_nets] else []                
                let a3 = if protocol_prams.reqrdy_present then [scall_getnet "reqrdy" hs_nets] else []
                let a4 = if protocol_prams.ackrdy_present then [scall_getnet "ackrdy" hs_nets] else []                
                let start_lab = ids + "_restart"
                let headcode = [ Xlabel start_lab ]
                let tailcode =            // Self restart for next work set of args.
                    [Xgoto(start_lab , ref 0)]
                    //(Xblock[Xgoto(start_lab , ref 0); g_dic_hwm_marker])      // Old:?Make loop to start if run off end - makes eternal/process loop for h/w.

                (headcode, tailcode, a1 @ a2 @ a3 @ a4)
                    
    let objnet =
        if nullp cCB.staticated then [] // Add a dummy arg for the object pointer when our method is staticated.  This will not appear in the final hbev code.
        else
            let (top_this_dt, top_this) = valOf_or_fail "L4338 this_dt" this_o
            // We need to call the constructor to set up the env.
            //If there are constructor args, we can lookup saved constructor arg vectors.
            //We need to squirrel them where constant and run for each arg vector.  Where they are non-constant we should create RTL parameters.We need to find constructor args if the constructor has any. 
            let class_idl = tl mdt.name
#if SPARE
            dev_println(sprintf "Remote-spog constructor lookup on db. class=" + hptos class_idl)
            let saved_ctor_args = 
                match cCC.m_remote_ctor_arg_db.lookup class_idl with
                    | [] ->
                        dev_println (sprintf "no ctor args saved for " + ceToStr top_this)
                    | arg_vectors -> dev_println (sprintf "Kiwi.Remote entry point: retrieved %i vector(s) of consrtuctor args saved for class %s" (length arg_vectors)  (hptos class_idl))
#endif
            vprintln 2 (sprintf "Adding a dummy 'this' arg for the staticated object ref %s" (cil_typeToStr top_this_dt))
            [ (-1, ([], gec_CT_star 0 top_this_dt, top_this), X_undef, None) ] // The -1 index becomes 0 when a staticated offset of one is applied to all actual args.
    let arg_nets = objnet @ arg_nets
    let arg_nets = if not voidf then (valOf retnet) :: arg_nets else arg_nets

//>H
    let (old_duido_, a, b, c, nax) = director_spec_in
    let director_spec_out = (Some pi_name, a, b, c, nax) // Override the one passed in to kiwic_start_toplevel - could tidy please.
    vprintln 3 (sprintf "At term gen staticated=%A  : args=%A" (hptos cCB.staticated) (sfold (f3o4>>xToStr) arg_nets))
    let port1 = hs_nets @ arg_nets
    let cpi = { cpi with pi_name = "frankly-wasteofspace-" + cpi.pi_name }
    let port2 = DB_group(cpi, [] (* map db_netwrap_null (map (fun a -> valOf_or (f4o4 a) (f3o4 a)) port1)*)) // We want the external net (4th element) here if present, since that is really the I/O pad. TODO. - even if we do it gets ignored.? port2 is never used?
    // port2 is the DB form - ant not currently needed since we heap mine.
    (director_spec_out, Some retnet, Some(port1, port2), (* Some scall, *) top_xcode, top_kcode, tail_xcode, Some busDef_handy_info, handshakes) 



// The Kiwi-generated VM2s are not part of any library. They are bespoke for the current HLS run.
let rez_kiwi_ii i_name kind external_instance =
    vprintln 3 (sprintf "rez_kiwi_ii %s %s external_instance=%A" i_name (hptos kind) external_instance)
    { g_null_vm2_iinfo with
          generated_by=         "kiwife"
          vlnv= { vendor= "Kiwi"; library= "custom"; kind=kind; version= "1.0" }
          iname=                i_name
          externally_provided=  false // false, since these should not be separate modules
          external_instance=    external_instance
          preserve_instance=    true  //deprecated flag or ignored even? 
          nested_extensionf=    true
    }

(*
 * OLD: DELETE ME
 * Generate an instance of an HPR machine that will model a separately-compiled hardware machine.
 * Do we want this - generally the instances will be instantiated above/outside this compilation and not as an automatically embedded component - because we have no easy way of wiring up the other ports we dont know about on such units.
 * Yes, we do want it - we generate child instances that are stubs which may be flagged for external instantiation and verilog-gen makes them external. But we can run diosim with the stub in place this way.
 *)
let gen_server_instance ww (id, (basic, args, retnet, usersub, external_instance)) = 
    let id = xToStr id 
    let iname = funique("the" + id)
    let ww = WF 3 ("gen_server_instance of " + id) ww (sprintf "start. iname=%s" iname)
    let formals = (if retnet=None then [] else [ valOf retnet ]) @ args

    let atts = [ Nap("preserveinstance", iname) ] // Old flag - now in minfo.
    let kind = [id]
    let ii = 
       { rez_kiwi_ii iname kind external_instance with
            preserve_instance=    true // This _is_ a separate RTL module
            generated_by=         "kiwife-gen-server-instance"
        }

    (ii, Some(HPR_VM2({ g_null_minfo with name=ii.vlnv; atts=atts; hls_signature=None }, formals, [], [], [])))

let bb2str (spc, epc, flag) =  i2s spc + "-" + i2s epc + (if flag then "-T" else "-F")




// Make a dot plot for the kcode control flow.
let kcode_cfg_dot_report ww id justify transitions =
    let title = "kcode thread " +  id
    let ww = WF 3 "kcode_cfg_report" ww ("start " + title)
    let vd = -1
    let _:(int * (int * int * bool * bool) * bool * int list) list = transitions
    let nodes2 =
        let gknw ((src, (ss, ee, jpflag, apf_), apf, dests), srcno) =
            (srcno, src, (ss, ee, jpflag, apf_), apf, dests)
#if  SPARE
            let rec collect_work pc1 cc =
                let e = valOf_or_fail "gwn" (numbering.TryFind pc1)
                if e<>pc || pc1 >= dic.Length then cc
                else
                    let cc =
                        match dic.[pc1] with
                            | Xassign(l, r) -> singly_add (xToStr l + " :=") cc
                            | _ -> cc
                collect_work (pc1+1) cc
            let work = collect_work pc []
            (pc, work)
#endif                
        map gknw (zipWithIndex transitions)

    // With the DIC form, there is work in the nodes, whereas with the SP_fsm form, the work is on the edges.
    let arcs = // Create a temporary microarc set from kcode DIC just to use the plotter output code.
        let pc = gec_X_net id
        let ktempnode (srcno, src, (ss, ee, jpflag, apf_), apf, dests) cc =
            let ktemparc dd cc =
                let src = justify src
                let dd = justify dd
                { g_null_vliw_arc with 
                      pc=            pc
                      gtop=          X_true // for now
                      resume=        xi_num src
                      iresume=       src
                      dest=          xi_num dd
                      idest=         dd
                      eno=           next_global_arc_no()
                }::cc
            List.foldBack ktemparc dests cc
        List.foldBack ktempnode nodes2 []
    let nodework =
        let nw (srcno, src, (ss, ee, jpflag, apf_), apf, dests) = (justify src, [sprintf "KB%i%s" (justify src) (if apf then "-AP" else "")])
        map nw nodes2
    let (p1, p2) = reporter.fsm_dot_gen_serf ww id nodework arcs
    let lst = [(id, p1 @ p2)]
    let keyname = id // Code for more than one in a file here is vestigial.
    let filename = "kcontrolflow_" + keyname + ".dot"
    let fd = yout_open_log filename
    yout fd (report_banner_toStr "// ")
    let ww = WF 3 "render_dot_plot" ww (sprintf "start %i plots" (length lst))
    let rdp (tag, contents) =
        let ww = WF 3 "collate_dot_plot" ww (sprintf "tag=%s file=%s plots" tag filename)
        yout fd (sprintf "// + including tag=%s\n" tag)
        list_flatten contents
    let data = map rdp lst
    dotout fd (DOT_DIGRAPH("kcode_cfg_" + keyname, list_flatten data))
    yout_close fd
    let ww = WF 3 "kcode_cfg_report" ww ("finish " + title)
    ()


let generate_kcode ww m_shared_var_fails kiteration (tid3, uid) idsx top_kcode (cCC, cCP, cCL, cCB) topargs flogs mM actuals = // convert the trajectory of a thread to kcode DIC (directly indexed code).
    let vd = !g_gtrace_vd // Normally -1 for debugging off.
    let tid = tid3.id
    //dev_println(sprintf "Generate KCODE tid %s finger %s" tid uid)
    match topargs with
        | None -> ()
        | Some (port1, port2_declbinder_form) ->
            //dev_println (sprintf "generate_kcode: We have %i topargs %A" (length port1) (sfold (f3o4>>xToStr) port1))
            ()
    let cmds = 
        let ww = WF 3 "generate_kcode" ww (sprintf "tid=%s, finger=%s start iteration %i" tid uid kiteration)
        let root_kxr = KXREF("threadstart", [ tid ])
        let stack = ref [] // use own
        let linepoint_stack = [ "EntryPoint_" + tid ]
        let env = Map.empty//Unused_filler_env - all env processing now done on kcode after this step.
        let steplimit = (valOf !g_kfec1_obj).cil_unwind_limit
        let (opl1, opl2) = (ref [], ref [])
        let realdo = Rdo_gtrace(opl1, opl2)
        let mgr = []
        let old_now_unused_env_ = cil_walk_method root_kxr mgr topargs realdo ww linepoint_stack (cCP, cCL, cCB, stack, (env, steplimit)) mM actuals (flogs (* if you want flog ats then put in ccl or cb please*) )        
        let cmds = (rev !opl1) @ (rev !opl2)
        //vprintln xvd "-------gt1---kcode-listing---------------------------------"
        //app (fun x -> vprintln xvd ("   gt1:" + kToStr true x)) cmds
        cmds

    // Do not apply hll_reset to top-level formal parameters, whether they be logic inputs or RTL parameters..
    let _ = 
        let pram_nets =
            let reveng_top_kcode cc = function
                | K_as_sassign(_, lhs, rhs, _) -> // Any scalar that is assigned is picked up here.
                    match lhs with
                        | CE_var(uid, dt, idl, fdto, ats) -> idl :: cc
                        | other -> sf (sprintf "Other lhs in reveng %A" other)
                | _ -> cc
            List.fold reveng_top_kcode [] top_kcode
        //vprintln 3 (sprintf "disable hll_reset for " + sfold hptos pram_nets)
        app (fun idl->cCC.already_initialised_vars.add idl 1) pram_nets
       
    let (commit_hll_resets, cmds) = 
        // We make a note of the nets this thread or ctor reads so that they can be cleared (hll_reset applied) as per the .NET sementic if read before written.
        // We also make a note of the nets written by this thread so they are not cleared in the precursor to a subsequent thread or ctor.
        // We defer commiting them unitl this thread is successfully compiled, which may have taken a number of iterations.
        // Note that prepending these extends the 'live' range for a net, largely defeating the vreg system beneft ... TODO.
        let (m_hll_resets_done, m_resets) = (ref [], ref [])
        let commit_hll_resets () = // Commits to the aux datastructure - the various assigns have already been emitted.
            vprintln 3 (sprintf "commit_hll_resets: commit HLL resets for tid=%s count=%i" tid (length !m_hll_resets_done))
            let commitx idl =
                if vd >= 5 then vprintln 5 (sprintf "Committing note of already initialised net (no further HLL reset needed) during tid=%s idl=%s" tid (hptos idl))
                cCC.already_initialised_vars.add idl 1
            app commitx !m_hll_resets_done
        if true then // Clear all CIL variables to zero or null, which is the .NET semantic.
            // This currently only does static/singleton nets, not regions. 
            let msg = "Scan HLL reset/initial vlaues for tid=" + tid 
            let (lhs_support, rhs_support) = kcode_collect_static_support ww msg (map (fun x -> (-1, x)) cmds)
            let kxr = KXREF("starting/reset values", [tid])
            let emitter idl ce =
                match cCC.already_initialised_vars.lookup idl with // Do not reset those set by other threads.
                    | Some 1 ->
                        if vd >= 5 then vprintln 5 (sprintf "Skip HLL reset owing to earlier thread/.ctor write for tid=%s idl=%s" tid (hptos idl))
                        ()
                    | _      ->
                        match ce with // Do not reset input or parameter nets - these are a Kiwi extension and not part of standard C# semantic.
                            | CE_var(uid, dt, idl, fdto, atts) when (hd idl).[0] = '$' ->
                                if vd>=5 then vprintln 5 ("Not applying an HLL reset to PLI meta-variable " + hptos idl)
                                ()
                            | CE_var(uid, dt, idl, fdto, atts) when dt_is_struct dt ->
                                if vd>=5 then vprintln 5("Not applying an HLL reset to struct " + hptos idl)
                                ()
                            | CE_var(uid, dt, idl, fdto, atts) when scan_vinv atts ->
                                if vd>=5 then vprintln 5 ("Not applying an HLL reset to input/parameter net " + hptos idl)
                                ()
                            | other ->
                                let sap = full_struct_assign_pred msg ce
                                if vd>=5 then vprintln 5 (sprintf "Establish pending HLL reset during tid=%s idl=%s sap=%A ce=%s" tid (hptos idl) sap (ceToStr ce))
                                mutadd m_hll_resets_done idl
                                let aso = { abuse=None; reset_code=true }
                                mutadd m_resets (K_as_sassign(kxr, ce, g_ce_resetval, aso)) // If it is a struct ...

            let noter idl ce =
                if vd >= 5 then vprintln 5 (sprintf "Noting no HLL reset will now be needed for tid=%s idl=%s" tid (hptos idl))
                mutadd m_hll_resets_done idl
            for kk in rhs_support do emitter kk.Key kk.Value done
            for kk in lhs_support do noter kk.Key kk.Value done
            (commit_hll_resets, !m_resets @ cmds)
        else (commit_hll_resets, cmds)



    let (kcode, cmds) =
        let cmds =
            let ll = (length top_kcode)
            if ll > 0 && vd>=4 then vprintln 4 (sprintf "Prepend %i top_kcode cmds" ll)
            top_kcode @ cmds // Prepend prefix code.
        let len = length cmds
        let dic = Array.create len { regionchange=false; kinstruction=K_remark(KXREF("", []), "+++ uninitialised dic location")}
        let cmds =
            let rec ins p = function
                | [] -> []
                | h::t ->
                    let regionchangef =
                        match h with
                            | K_easc(kxr, CE_apply(CEF_bif bif, cf, args), ef) when hd bif.uname = "Pause" -> true
                            | K_easc(kxr, CE_server_call(_, _, cf, sargs, scv, args), ef) -> true
                            | _ -> false
                    let line = { kinstruction=h; regionchange=regionchangef }
                    Array.set dic p line
                    (p, h) :: (ins (p+1) t)
            ins 0 cmds

        let kcode = Gt_dic(idsx, len, dic)

        let _ =
            if cCC.settings.kcode_dump > 0 then // Write out an early kcode listing file for this thread.
               let lpl = false
               let fn = logdir_path( "kcode." + tid + "." + uid + ".gt3.txt")
               let fd = yout_open_out fn
               yout fd (report_banner_toStr "REPORT :")
               yout fd ("Early (gt3) listing.")
               kcode_gt_print fd dic len lpl (sprintf "gt3-early-thread=%s" tid)
               //let _ = gt3print fd
               yout_close fd            
               vprintln 2 (sprintf "Wrote early kcode listing file. Filename=%s. length=%i commands. " fn len)

        // Note also: kcode should be analysed for induction variables before register colouring.
        populate_ataken ww m_shared_var_fails "L3883a" tid3 cmds // Must do this before reg colouring to phy and before rebuild of tree env since store class references are changed.
        (kcode, cmds)


    // Of course the first-pass also divided everything into basic blocks but the interim intcil
    // code conglomoerated them again.
    let kcode_divide_into_basic_blocks entry_points kcode =
    // jpflag = joinpoint_flag
    // transtion_t = int * (int * int * bool) * int list  : from * (start, end, joinpoint_flag) * destinations
        let a5 m = vprintln 3 (tid + ": Elab5 kcode_divide_into_basic_blocks: " + m)
        match kcode with
        | Gt_dic(id, 0, dic) -> ([], [], None)
        | Gt_dic(id, len, dic) ->
            let jmp_mapping = gen_gt_label_map kcode
            let rec pointscan finished lst = // Find reachable states and join points and regionchange points that sub-divide an apparent basic block.
                let rec joinpoint_serf p =
                    if p <0 || p >= len then ([], [])
                    else 
                        //dev_println ("Joinpoint_serf " + i2s p + "/" + i2s len)
                        match dic.[p].kinstruction with
                            | K_goto(kxr, _, sl, pp)   when !pp < 0 ->
                                    match jmp_mapping.TryFind sl with
                                        | None ->
                                            let cos = YOVD 0 // Listing 
                                            kcode_gt_print cos dic len false "assembler-dump"
                                            sf (sprintf "kcode label '%s' not defined." (hptos sl))
                                        | Some d -> (pp:= d; joinpoint_serf p) // Go round again
                            | K_topret  _ -> ([], []) 
                            | K_goto(kxr, None, sl, d) -> ([!d], [!d])        // ucond jump
                            | K_goto(kxr, Some _, sl, d) -> ([!d], [!d; p+1]) // conditional branch
                            | other ->
                                if p >= len-1 then ([], [])
                                else joinpoint_serf (p+1)

                if lst=[] then []
                else
                    let n = hd lst
                    //dev_println(sprintf "joinpoint_serf: dequeue %i:  lst=%s" n (sfold i2s lst))
                    cassert(n>=0, "Non-assembled kcode.")
                    if n>=len || memberp n finished then pointscan finished (tl lst)
                    else 
                        let (blockstarts, successors) = joinpoint_serf n
                        lst_union blockstarts (pointscan (n::finished) (successors  @ tl lst))
            let joinpoints = pointscan [] entry_points
            let exit_block = ref None
            let m_transitions = ref []
            let rec blkscan finished lst =
                let rec scan1 p = // Return inclusive (start, end) pair for this basic block executable contents and list of successors.
                    let skipon = function
                        //| K_linepoint _
                        | K_remark _ | K_lab _ -> true
                        | _ -> false
                    //dev_println ("Scan1 p=" + i2s p)
                    if p >= len then (None, [])
                    else
                    let h = dic.[p].kinstruction
                    if skipon h && p<len-1 then scan1 (p+1)
                    else
                        let start = p
                        let rec scan2 p =
                                  //vprintln 0 ("Scan2 " + i2s p)  
                            let set_exit_block() =
                                   let eblk = (start, p, false)
                                   a5("Exit block is " + bb2str eblk)
                                   cassert(!exit_block = None, "At most one exit block")
                                   exit_block := Some eblk
                                   (Some eblk, [])
                            match dic.[p].kinstruction with
                                | K_topret  _ -> set_exit_block()
                                | K_goto(kxr, None, sl, d)   -> (Some(start, p, false), [!d])  // ucond jump
                                | K_goto(kxr, Some _, sl, d) -> (Some(start, p, false), [!d; p+1]) // conditional branch
                                | other ->
                                    if memberp p joinpoints then (Some(start, p-1, true), [p]) // For this block, joinpoint jpflag holds, meaning it can be invoked by control flowing out of the previous block.
                                    elif p < len-2 && dic.[p+1].regionchange then (Some(start, p, false), [p+1])
                                    elif p < len-1 then scan2 (p+1)
                                    else set_exit_block()
                        scan2 p
                if lst=[] then []
                else
                    let n = hd lst
                    cassert(n>=0, "Non-assembled k code")
                    if memberp n finished
                    then blkscan finished (tl lst)
                     else 
                         let (s1, sucs) = scan1 n
                         if not_nonep s1 then
                             mutadd m_transitions (n, valOf s1, sucs)
                             singly_add (valOf s1) (blkscan (n::finished) (sucs  @ tl lst))
                         else []
            let basic_blocks =
                let ascending a b = f1o3 a - f1o3 b
                List.sortWith ascending (blkscan [] entry_points)

            let majorstat_line = sprintf "Thread %s has %i CIL instructions in %i basic blocks" idsx len (length basic_blocks)
            vprintln 2 majorstat_line
            majorstat_line_log "cilnorm" majorstat_line

            let gt4print cos = // This is the post elaboration (and constant folded) print.
            // Why do we have so many kcode printers ? This variant has top/tail annotations
                let println ss = youtln cos ss
                println("KiwiC dump\n" + timestamp(true) + "\n" + !g_argstring + "\n")
                println("Basic blocks " + sfold bb2str basic_blocks)
                println(sprintf "-------gt4---kcode-listing-start-------thread=%s.%s------------------- %i commands" tid uid len)
                let (tops, tails, _) = List.unzip3 basic_blocks
                let idt = ref ""
                let lpl = false
                let plist p =
                    let h = dic.[p]
                    if memberp p tops then ( idt := "   "; println " $(") 
                    println(!idt + "   000" + i2s p + ":" + (if h.regionchange then "NEW-REGION " else "") + kToStr lpl h.kinstruction)
                    if memberp p tails then ( idt := ""; println " $)")            
                    ()
                app plist [0..len-1]
                println(sprintf "-------gt4---kcode-listing-end-------thread=%s.%s------------------- %i commands" tid uid len)
                ()
            if cCC.settings.kcode_dump > 0 then // Write out a kcode listing file for this thread.
                          let fn = logdir_path( "kcode." + tid + "." + uid + ".gt4.txt")
                          let fd = yout_open_out fn
                          yout fd (report_banner_toStr "REPORT :")
                          yout fd ("Post-fold listing.")
                          gt4print fd
                          yout_close fd            
                          vprintln 2 ("Wrote post-fold kcode listing file " + fn)
            (basic_blocks, !m_transitions, !exit_block)
    let (basic_blocks, transitions, exit_block) = kcode_divide_into_basic_blocks [0] kcode
    let transitions =
        let ascending2 a b = f1o3 a - f1o3 b
        List.sortWith ascending2 transitions



    let ww = WF 3 "generate_kcode" ww (sprintf "End iteration. generate_kcode done for tid=%s" tid)
    // In a grander approach, lasso stem and constant meet would not be separate phases, but while they are
    // we also take note of 
    (cmds, kcode, basic_blocks, transitions, exit_block, commit_hll_resets)

    

(*
 *  Start a thread at a method entry point.  The method might be an assertion or an RPC stub.
    Is env already in c ... duplication: does no harm?

Callgraph:  kiwic_strt->kiwic_start_thread->   kiwic_start_toplevel_method->  rpc_server_entryp

 *)
let kiwic_start_toplevel_method ww (director_spec:director_spec_t) staticated (cCL0:cil_cl_dynamic_t) tid3 (ctorcode) m_spawned_threads (ignore_returnf, keep_returnf) srcfile (ep, actuals_opt, mM, cxc) (uid, idl, toplevel_mdt) m_shared_var_fails  = 

    let msg = sprintf "KiwiC start_toplevel_method: thread (or entry point) uid=%s full_idl=%s" uid (hptos idl)
    let ww = WF 2 "kiwic_start_toplevel_method" ww msg
    //let No_method(srcfile, flags, callkind, (idl, uid, _), mdt, flags1_, instructions, atts) = mM
    let colouringf = (valOf !g_kfec1_obj).reg_colouring <> "disable"
    let cCC = valOf !g_CCobject
    let vd = cCC.settings.hgen_loglevel
    let tid = tid3.id
    let cgr0 = cCL0.text.cm_generics
    let asserty = assertionmethod ww cgr0 mM
   
    let has_args = not_nullp toplevel_mdt.arg_formals// Should perhaps consider rtype not void too?


    let (not_a_serverf, director_spec, externally_instantiated, fsems) =
        match toplevel_mdt.is_remote_marked with
            | None ->
                // If pipelined accelerator, should have come from accelerator attributes...
                //let protocol = parse_kiwi_accelerator_attributes ww msg args
                if has_args then // dev mode only
                    (true, director_spec, false, g_default_fsems) 
                else (true, director_spec, false, g_default_fsems)

            | Some(prams, (protocol_prams, protocol_str), searchbymethod, externally_instantiated, fsems) -> 
                // A whole remote thread?   TODO also infer this for every called method of a RemoteClass.
                let (duido, a, b, c, _) = director_spec
                let nax = DHM_pipelined_stream_handshake(protocol_str, protocol_prams)
                //vprintln 2 (sprintf "Using %s as RPC server method pi_name.  " pi_name)
                let director_spec = (duido, a, b, c, nax) // Override the one passed in to kiwic_start_toplevel - could tidy please.
                (false, director_spec, externally_instantiated, fsems) // odd that we drop protocol_prams here and regen inside entry pt? New hangmode.
           
    let msg = if asserty<>None then msg +  ": An assertion method" else msg
    let msg = if not_a_serverf then msg else msg + ": A remote-callable method"

    let ww = WF 3 msg ww "Commence" // ("Commence: env items=" + i2s(env_length(f6ox cxc)))
    let (decls, execs, non_ctor_, childs, rules, flogs, (env:env_t), f22, f33, t33) = cxc
    let idsx = hptos idl  + " uid=" + uid

    //printenv here

    //dev_println ("Premier")    printenv 0 10 ("Env at start of thread " + tid3.id) env
    let rlab = tid3.id + (if f5o5 director_spec (*.hang_mode*)=DHM_hang then "tzailhang_" else "retpoint_")

    youtln cCC.rpt msg
    let mkcre e =
        let steplimit = (valOf !g_kfec1_obj).cil_unwind_limit
        (T_steps(steplimit, steplimit), 0, e) // arbitrary env
    let bb2str (spc, epc, flag, apflag) =  i2s spc + "-" + (if apflag then "AP-" else "") + i2s epc + (if flag then "-T" else "-F")
    let linepoint_stack = [ "ThreadStart_" + uid ]
    let cpoint = ref None
    let ctl = { // textual aka static chain
                   cm_generics= Map.empty
                   this_dt=     if nonep ep then None else Some(get_ce_type msg (valOf ep))
                   cl_codefix=  idl @ [tid3.id] @ (cCL0.text.cl_codefix)
                   cl_prefix=   idl @ (cCL0.text.cl_prefix)
              }
    let cCL = { // dynamic chain information (method's stackframe).
                   text=           ctl
                   cl_localfix=    idl @ [tid3.id] @ (cCL0.cl_localfix)
                   this=           ep
              }
    //vprintln 0 ("Forming new prefix: extend " + hptos cCL0.cl_prefix + " by adding " + hptos idl)

    let cCB = { // Environment for a method's compilation.
                   srcfile=            srcfile
                   exn_handler_stack_= [] // Unused.
                   fieldat=            ref None
                   cpoint=             cpoint
                   director_ref=       director_spec
                   //tailhangf=        tailhangf // we now use dir.hang_mode=DHM_hang please!
                   prefix=             idl @ (cCL0.text.cl_prefix)
                   simple_localfix=    idl @ [tid3.id] @ (cCL0.cl_localfix)
                   codefix=            idl @ [tid3.id] @ (cCL0.text.cl_codefix) 
                   retlab=             (keep_returnf, rlab) // A return label and result register exist and hold for use with a thread's top-level method. Also used as the hang loop dest.
                   local_vregs_used=   ref []
                   staticated=         staticated
              }

            
    let cCP = {
                  m_spawned_threads=    m_spawned_threads
                  expanded_items_=      ref [] // not used
                  tid3=                 tid3
                  marked_end_of_elab=   ref None
                  m_shared_var_fails=   m_shared_var_fails
                  sptok=                ref 750 // Arbitrary starting number makes distinctive log/debug output.
            }



    let actuals =
        match actuals_opt with
            | Some actuals ->
                //dev_println (sprintf "Setting up actuals %A" actuals)
                actuals
            | None ->  // Establish bindings that make top-level inputs eval to their runtime values (never subsumed).  This is used for delegate/thread invocations and accelerators and rpc entry points.
            // For a staticated method, arg0 will be a dummy and ldfld/ldflda will ignore it.
                let zat = function
                    | (_, _, ctype, v)->(g_ctl_object_ref, INPUT, g_ce_null)      //For an argless static class there will be none. In other cases, the result here is ignored in the method start code.
                let ans = map zat toplevel_mdt.arg_formals
                //dev_println (sprintf "No externally provided or set up actuals: staticated=%A adding %A" (hptos staticated) ans)
                ans
                
    let idx = ref 0
    let labs=ref []
    let start_lab = tid3.id + "_anfang"
    let etarget = funique "etarget"
    vprintln 3 ("Elaboration etarget =" + etarget + " retlab=" + snd(cCB.retlab))

    let tailhangcode() = // The hang aspect should perhaps now be inserted by a render stage or inferred by a simulator stage. But the loop to start (for server process loops) is still required.
        if not not_a_serverf then [ Xgoto(etarget, ref -12); ]
        elif f5o5 director_spec <>DHM_hang then
            if ignore_returnf then []
            else [ Xreturn(xi_one) ] // TODO? Change to write zero to abend syndrome code on reaching normal exit? 
        else
            let id = snd cCB.retlab
            vprintln 3 ("Generating tail hang code for " + id)
            (* Label is already emitted at end of subroutine *)
            [ Xeasc(lower_kiwi_pause_call ww []); Xgoto(id, ref -13) ]

    // TODO get inferred fsems from static analysis before processing user's overrides.
    // fsems also come from Kiwi.Remote attributes
    let xname = hptos cCB.codefix

    let (director_spec, topret_loc, topargs, top_xcode, top_kcode, tailcode, busDef_handy_info, handshakes) =
        if has_args || not not_a_serverf then
            let conditionally_append p q =
                let rec already_present = function
                    | (a::at, b::bt) when a=b -> already_present(at, bt)
                    | ([], _) -> true
                    | _ -> false
                if already_present (p, q) then q else p @ q

            let this_o =
                match ctl.this_dt with
                    | None -> None
                    | Some dt -> Some(dt, valOf_or_fail "L4780" cCL.this) 
            rpc_server_entryp ww (cCB) (director_spec) (conditionally_append idl cCL0.text.cl_prefix, cgr0, toplevel_mdt) this_o
        else (director_spec, None, None, [], [], tailhangcode(), None, [])
    let topret_mode = (ignore_returnf, topret_loc)

#if OLD
    let decls =
        match topargs with
            | Some(port1, port2) ->
                let ag_ag (m, (idl, dt, ce), xnet, external_net_opt) = (idl, (m, ce), xnet, external_net_opt)
                let nb = (map ag_ag port1, port2)
                dev_println (sprintf "Temp ignore toparg binds") // We now dig them out from singleton heap later along with all other scalar nets.
                nb::decls
                decls
            | None -> decls
#endif

    let mm = "main KiwiC pass of " + idsx
    let ww = WF 3 mm ww "Commence"
    (* CP was reset for each main pass *)

    // TODO the high-level resets need down propagating since they hopelessly expand the live region of a register.
    let do_dfa kcode (xition:xition_t) basic_blocks  = // Dataflow analysis for live virtual registers on instruction level. (Const meet was on basic block level.)
        let dfa_vd = false  // normally false
        match kcode with
            | Gt_dic(id, len, dic) ->
            let live_out = Array.create len ([]:int list)
            let ins_successors = Array.create len ([]:int list)
            let allpoints =
                let detailer cc (spc, epc, flag, bbflag) =
                    if dfa_vd then  vprintln 0 (sprintf "detailer basic block %i..%i flag=%A (len=%i)" spc epc flag len)
                    app (fun n -> Array.set ins_successors n [n+1]) [spc..epc-1]
                    let successors = valOf_or_nil(xition.TryFind spc) // indexed by spc
                    if dfa_vd then vprintln 0 (sprintf " successors %i  ->  %s" epc (sfold i2s successors))
                    Array.set ins_successors epc successors
                    [spc..epc] @ cc
                List.fold detailer [] (*reachable_*) basic_blocks

            if colouringf then
                let ww = WF 3 "dfa" ww (tid3.id + sprintf ": Live variable analysis of %i points start." (length allpoints))

                let live_in = Array.create len ([]:int list)
                let live_gens = Array.create len ([]:int list)
                let live_kills = Array.create len ([]:int list)

                let bn_ = 0 
                let _ = 
                    let dfa kpc =
                        let code = dic.[kpc].kinstruction
                        let (kills, gens) = collect_gen_kill_null_sets kpc (bn_, code) 
                        if dfa_vd then vprintln 0 (sprintf "dfa kpc=%i   kills=%s gens=%s" kpc (sfold vreg2s kills) (sfold vreg2s gens))
                        (Array.set live_gens kpc gens;Array.set live_kills kpc kills)
                    app dfa allpoints

                let ww = WF 3 "dfa" ww (tid3.id + sprintf ": Gen and Kill sets computed.")
                let m_changed = ref false
                let rec liveness_iterate (count) =
                    m_changed := false
                    let livevar_step0 i =
                        //in[i] = gen[i] union (out[i] - kill[i])
                        Array.set live_in i (union_ascending_int_sort live_gens.[i] (ascending_int_subtract(live_out.[i], live_kills.[i])))
                    let livevar_step1 i =
                         //out[i] = union over all successors of their ins
                        let new_out_i = Union_ascending_int_sort(map (fun x->live_in.[x]) ins_successors.[i])
                        if new_out_i <> live_out.[i] then (m_changed:= true; Array.set live_out i new_out_i) 

                    app livevar_step0 allpoints
                    app livevar_step1 allpoints
                    if !m_changed then liveness_iterate(count+1) else count
                let count = liveness_iterate 0 

                let ww = WF 3 "dfa" ww (tid3.id + sprintf ": Live variable analysis of %i points completed in %i iterations." (length allpoints) count)
                let _ =
                    if dfa_vd then
                        let m_list = ref []
                        for k in [0..len-1] do mutadd m_list (k, live_out.[k]) done
                        reportx 3 ("dfa live in for " + tid3.id) (fun (kpc, lst) -> i2s kpc + "  " + (if nullp lst then "" else sfold vreg2s lst)) (rev !m_list) // (zipWithIndex (live_in |> Array.toList))

                let liveintervals = new OptionStore<int, int * int>("liveintervals")
                let _ =
                    let msg = "allocation for thread " + tid
                    // Linear Scan Register Allocation (POLETTO and SARKAR)
                    let determine_live_intervals scanpt =
                        let update_live_intervals vrn =
                            match liveintervals.lookup vrn with
                                | None -> liveintervals.add vrn (scanpt, scanpt)
                                | Some (start, _)-> liveintervals.add vrn (start, scanpt)
                        app update_live_intervals (live_out.[scanpt])
                    app determine_live_intervals [0..len-1]

                    let (epochs, vrn_lst) =
                        let m_vrn_lst = ref []
                        let mm = ref []
                        let mke vrn (start_pt, end_pt) =
                            mm := (start_pt, (vrn, start_pt, end_pt)) :: (end_pt, (vrn, start_pt, end_pt)) :: !mm
                            mutadd m_vrn_lst vrn
                        let _ = for x in liveintervals do mke x.Key x.Value done
                        (List.sortWith (fun a b -> fst a - fst b) !mm, !m_vrn_lst) 

                    let scoreboard = { n_pregs= ref 0; phyregs=new phyregs_database_t("phyregs") }

                    let m_vreg_use_report = ref [] // Register virt-to-phys mapping for report file
                    let linear_scan_engine (epoch, (vrn, start_pt, end_pt)) =
                        let _ = if dfa_vd then vprintln 0 (sprintf "epoch=%i: alloc V%i from %i to %i" epoch vrn start_pt end_pt)
                        if start_pt=end_pt then vprintln 3 (tid3.id + sprintf ": Solo use register - register most likely never read ?? V%i from %i to %i" vrn start_pt end_pt)
                        elif epoch = start_pt then ignore(vreg_alloc_phy ww m_vreg_use_report scoreboard msg vrn)
                        elif epoch = end_pt then vreg_release_phy ww scoreboard msg vrn
                        else sf "L3883"

                    app linear_scan_engine epochs // Nominally create and share the registers for this kcode thread.
                    let _ = vreg_uav_complete ww msg scoreboard // Finally create the actual registers.                    
                    let line2 = sprintf ": Linear scan colouring done for %i vregs using %i pregs" liveintervals.Count !scoreboard.n_pregs
                    let ww = WF 3 "dfa" ww "done" // (tid3.id + line2)
                    aux_report_log 2 (cCC.toolname + " virtual to physical register colouring/mapping for thread " + tid3.id) ((line2 + "\n") :: !m_vreg_use_report)

                    ()
                ()
                


    let m_cgc = (ref false, ref []) // Call graph has changed mutable var - TODO there's another in cCC now


    // Constant-folding meets algorithm. Largely the classical.  This does not require a variable to
    // be globally constant, just to have a common constant value at all control paths leading to the basic block.       

    // However, we extend it to support a simplistic form of garbage collection (or implicit object disposal).
    // Where a heap object is allocated inside a loop that is not compile-time, it will potentially consume fresh memory
    // on each iteration.  There are two basic senarios associated with such a condition: either the fresh memory is
    // useful, such as when adding items to a linked-list datastructure, or else it is not needed because the previous
    // allocation is no longer live and could be simply reused.  This second case can be fully served by a static allocation
    // at compile time.

    // Kiwi 1 did not support genuine dynamic storage allocation inside an execution-time loop.  Bit it provides two mechanisms to support dynamic to static reduction where dynamic store is not really needed.
    // The first uses an explicit dispose and the second uses an implicit dispose.  Either way, when the loop iterates, the active heap has
    // shrunk and KiwiC makes sure to reuse the previously allocated heap record at the allocation site (call to C\# new).
    
    // See test39 for Kiwi 2 genuine heap allocation. Also the linked list example ... http://www.cl.cam.ac.uk/research/srg/han/hprls/orangepath/kiwic-demos/linkedlists.html

    // The implicit dispose works by checking whether a heap block handle (reference to the object or any field or location within the object) (class, struct or array) is used in subsequent basic blocks.  In classic Fosdick+Osterweil terminology, a block reference is killed when all handles are overwritten with fresh data ...

(*

Both for virtual register reuse and heap block reuse, HLS may rely on lack of aliases beyond imperative code apparent lifetime.  For instance

   In the example:
              ax[1] = 3;
              int axt = ax[1];
              bx = new int [100];
              bx [1] = 4;
              return bx[1] + axt;
   where ax has been implicitly freed after its last apparent reference and the same memory reused for bx, an HLS synthesiser, like the one in bevelab, may pack all the work into one clock cycle, bypass the temporary holding in bx and ...
          
When a block is allocated it is given a uid (aka memtok). Our DFA store0/store1 analysis is run before constant folding and so has all the relevant nemtok tagging info.


A straightforward constant meet implementation fails on a CFG back edge to a point above where realloc is to take place: - the blk is free in the original entry and allocated on the backedge.  The essence of our approach is
  1/ not to fail at that point, just to record a discrepancy in the heap record,
  2/ when an alloc is encounted we see if there is a matching discrepancy for that alloc in the heap record and remove the discrepancy as part of an otherwise normal allocation: this gives consistency.
  3/ where the CFG makes a copy of a heap pointer or address of a field within the heap item, we also record this for escape analysis. 


Where a heap alloc occurs beyond the end of satic elaboration, it might often only ever be run once or in an otherwise static-transformable form.

*)


    
    let constant_fold_meets ww0 linepoint_stack tT (env0:env_t, entry_point) (xition:xition_t) basic_blocks justify kcode = 
        match kcode with
        | Gt_dic(id, len, dic) ->
        let mx = sprintf "constant_fold_meets tid=%s entry_point=%i" tid entry_point
        let linepoint5 = { waypoint=ref None;  callstring__= ref [mx]; codept=ref None; srcfile= cCB.srcfile; linepoint_stack=linepoint_stack; }

        let ww = WF 3 mm ww0 "start"
        let entry_point = justify entry_point
        let nblks = length basic_blocks
        let idxlst = map f1o4 basic_blocks
        cassert(memberp entry_point idxlst, mx + ": Entry point found")
        cassert(xition.TryFind(entry_point) <> None, mx + ": Entry point was at start of a basic block")
        let inverted_xition = // generate predecessor matrix from successor matrix
            let add c n = 
                let blk_successors = valOf_or_nil(xition.TryFind n)
                let add1 (c:Map<int, int list>) v =
                    let ov = c.TryFind v
                    //vprintln 3 ("Reverse xition " + i2s v + " <-- " + i2s n)
                    c.Add(v, singly_add n (valOf_or_nil ov))
                List.fold add1 c blk_successors
            List.fold add Map.empty idxlst

        let const_in = Array.create len None
        let const_out = Array.create len None // env_t option array.



        let reverse_postorder =
            let doner = ref []
            let rec scan  n =
                if memberp n (!doner) then []
                else
                let _ = mutadd doner n
                let sucs = valOf_or_nil(xition.TryFind n)
                let sons = map (scan) sucs
                (list_flatten sons) @ [n]
            rev (scan entry_point)
        vprintln 3 ("Reverse post-order ordering for constant fold is " + sfold i2s reverse_postorder)


        let m_meet_changes = ref true

        let meet_process_block (spc, epc, flag, apflag) = 
            let antecedants = valOf_or_nil(inverted_xition.TryFind spc)
            let mx = "Constant Meet: " +  bb2str (spc, epc, flag, apflag) + "-nA=" + i2s (length antecedants)
            let antecedants = List.fold (fun c x -> if const_out.[x]<>None then valOf(const_out.[x])::c else c) [] antecedants
            let mx = mx + ":" + i2s(length antecedants) // live antecedants
            let (antecedants, mx) = if (spc=entry_point) then (env0 :: antecedants, mx+"*EP") else (antecedants, mx)
            if nullp antecedants then (vprintln 3 (mx + ": No envs yet"); ())
            else
            let rec meetfold = function
                | [item] -> item
                | [] -> sf(mx + ": ep=" + i2s entry_point +  ": meetfold: no envs at all")
                | a::b::tt -> meetfold(constant_meet_mitre mx m_cgc mx (a,b) :: tt)

            let met = meetfold antecedants // We can eliminate this costly op when we know no antecedants changed their constant outs. TODO oneday after re-enabling dynamic dispatch and recursion.
            let m_advanced = Some(ref None)
            match const_in.[spc] with
                | Some ov when cenv_deqd m_advanced m_cgc mx ov met -> 
                    if vd>=4 then vprintln 4 (mx + " No change to constants in")
                | _ ->
                    if vd>=4 then vprintln 4 (mx + " Change to constants in")
                    Array.set const_in spc (Some met)
                    let gt_exec e bpc = 
                        let cre = mkcre e
                        let callsite = (tid3, ([tid3.id], bpc))
                        let (cre', ib) = kcode_lower_block ww KER_constant_meet vd mx cre tT apflag kcode (linepoint5, callsite, topret_mode, epc) // constant meet callsite
                        (cre', ib)
                    let (cre', _) = gt_exec met spc // NB: xitions can now change!! but do they ever get greater ? Yes, as we deepen out recursive src code it grows.
                    let e' = (f3o3 cre')
                    let diff =
                        match const_out.[spc] with
                            | None ->
                                vprintln 3 (sprintf "marking a diff (degenerate case) on %i" spc)
                                true
                            | Some ov ->
                                //vprintln 0 (sprintf "start 2")
                                let ans = not(cenv_deqd m_advanced m_cgc mx e' ov) // used only to detect closure? Seems a bit wasteful to run this and the mitre?
                                //vprintln 0 (sprintf "end 2 inv=%A" ans)
                                ans
                    let _ =
                        if diff then
                            m_meet_changes := true
                            Array.set const_out spc (Some e')
                            if vd>=4 then vprintln 4 (mx + ": Change to constants out")
                            //printenv 0 10 (mx + ": Changed env is ") e'
                            ()
                        else if vd>=4 then vprintln 4 (mx + ": No change to constants out")
                    ()
        let reachable_basic_blocks =
            let rec ff x = function
                | (spc, epc, jpflag, apflag)::tt when spc=x -> (spc, epc, jpflag, apflag)
                | _::tt->ff x tt
            map (fun x -> ff x basic_blocks) reverse_postorder

        let rec meet_iterate n =
            vprintln 3 (tid3.id + sprintf ": Constant_fold meet iteration number %i" n)
            m_meet_changes := false
            // How can this fail to terminate? If a constant assumption propagates on a very windy path between the basic blocks it will take a while, but a pathological upper bound is the number of variables times the number of basic blocks I think.
            if n > nblks * 100 then cleanexit (sprintf "Thread %s: Constant_fold meet did not terminate after blks*100=%i iterations" tid n)
            app meet_process_block reachable_basic_blocks
            if !m_meet_changes then meet_iterate(n+1)

        let ww = WF 3 mm ww0 "constant_fold: start iterate over basic blocks"
        meet_iterate (0)
        let ww = WF 3 mm ww0 "constant_fold: finished iterate over basic blocks"


        (const_in, const_out, reachable_basic_blocks)


    // Please explain why we do not determine all thread constants before proceeding - this will help greatly with the ugly volatile issue which needs user assertions over which vars are touched by other threads.  TODO track variables that were flagged as non-volatile or single assigners and recompile if later compromised.  See VOC_interthread_shared.
        
    // We need to build the kcode until complete - additional recursive calls, higher-order method calling and object polymorphism all enlarge the possible kcode control flow.
    let rec kcode_iterate kiteration =
        cCC.dyno_dispatch_updated := false
        let a5 mm = vprintln 3  ("Elab5 " + mm)
        let ww = WF 2 "kcode_iterate" ww (sprintf "Start iteration %i " kiteration + mm)
           // Self restart
          //(Xblock[Xgoto(start_lab , ref 0); g_dic_hwm_marker])      // Old:?Make loop to start if run off end - makes eternal/process loop for h/w.
        let gG  = new_gdic idsx "KIWIFE"

        let gdp00 = { gen=None; m_tthreads=ref[]; psef=PSE_pre; vd=vd }

        let tTt_step1_stemfind  = (cCP, cCL.text, cCB,  { gdp00 with gen=Some gG; })
        let tTt_step2_fold      = (cCP, cCL.text, cCB,  gdp00)
        let tTt_step3_codegen   = (cCP, cCL.text, cCB,  { gdp00 with gen=Some gG; psef=PSE_post })

        let (cmds, kcode, basic_blocks00, transitions, exit_block, commit_hll_resets) = generate_kcode ww m_shared_var_fails kiteration (tid3, uid) idsx top_kcode (cCC, cCP, cCL, cCB) topargs flogs mM actuals // Convert the trajectory of a thread to kcode DIC (directly indexed code)  // and here is the call -----------------------------------------------


        let justify x = fst(justifier2 basic_blocks00 x)
        let transitions = map (fun (a, (spc, epc, jpflag), dests) -> (a, (spc, epc, jpflag), map justify dests)) transitions
        let visited_once_points: int list =
            let msg = (sprintf "Finding visited-once points in thread %s CFG" (tid3.id))
            let ww = WF 3 msg ww "Operation"
            let nodes = basic_blocks00
            let edgemap = new ListStore<int, int>("edgemap")
            app (fun (_, (src, _, _), dests) -> edgemap.adda src dests) transitions
            let printout_function x = i2s x //vprintln 3 (sprintf "%A" x)
            let inodes = map (f1o3) nodes
            let sccs = tarjan1 "visited-onced" (if false then Some printout_function else None) 4 (inodes, edgemap)
            // A visited-once block is not in any SCC and also has no reflexive arcs (our tarjan implementation may ignore SCCs of size one).
            let loopset =
                let sadd (sx:Set<int>) n = sx.Add n
                List.fold sadd Set.empty (list_flatten sccs)
            let print_scc lst = vprintln 3 (sprintf "  ... %s scc " tid3.id + sfold i2s lst + " \n")
            app print_scc sccs
            let reflexivep n =
                match edgemap.lookup n with
                    | dests -> memberp n dests
                    //| _ -> sf "reflexivep L5289"
            let visited_once_points =
                let negf cc n = if loopset.Contains n || reflexivep n then cc else n::cc
                List.fold negf [] inodes
            vprintln 3 (sprintf "Found %i visited_once points in thread %s CFG" (length visited_once_points) (tid3.id))
            visited_once_points


        let transitions =
            let add_apf1 (src, (ss, ee, jpflag), dests) =
                let apf = memberp ss visited_once_points
                (src, (ss, ee, jpflag, apf), apf, dests)
            map add_apf1 transitions
        let basic_blocks =
            let add_apf2 (ss, ee, jpflag) = (ss, ee, jpflag, memberp ss visited_once_points)
            map add_apf2 basic_blocks00
        
        reportx 3 ("Transitions for " + tid3.id) (fun (src, bb, apf, dests) -> i2s src + "   " + bb2str bb + sprintf " %s --> " (if apf then "VONCE" else "  ") + sfold i2s dests) transitions
        let xition = List.fold (fun (c:Map<int, int list>) (a, (spc, epc, flag, apf), apf, dests) -> c.Add(spc, dests)) Map.empty transitions

        let render_kcode_cfg_as_dot = cCC.settings.kcode_dump > 0
        if render_kcode_cfg_as_dot then kcode_cfg_dot_report ww tid3.id justify transitions

        do_dfa kcode xition basic_blocks
        let ww = WF 2 "kcode_iterate" ww (sprintf "dataflow analysis do_daf complete")
        let hbev_top_emit k =
            let idx = !gG.idx
            if vd>=4 then vprintln 4 (sprintf "kiwife top/tail emit: " + i2s idx + ":" + hbevToStr k + "\n\n")
            hbev_emitter { gdp00 with gen=Some gG; } k

        if nullp basic_blocks then (env, gG) // longwinded way...
        else
            // Here we have the main three steps of the compiler front end: KiwiC1, KiwiC2, KiwiC3.
                
            // Step 0
            app hbev_top_emit top_xcode // The top xcode is the old HSIMPLE poller used while (soon to be before) the directorate supported such start/stop control.

            // Step 1 - lasso stem find by compile-time elaborate (interpreting).
            let _ = WF 3 "KiwiC-stages" ww ("KiwiC1 stem find start")
            let justify2 x = justifier2 basic_blocks00 x
            let (end_of_elabf, (runtime_entry_point, (age, env, fidx))) =
                let gen_cref() = 
                    let steplimit = (valOf !g_kfec1_obj).cil_unwind_limit // Use cil_unwind_limit for the lasso stem length - loops are unwound inside the stem.
                    (T_steps(steplimit, steplimit), 100, env)
                kcode5_static_elaborate_stem ww linepoint_stack tTt_step1_stemfind (gen_cref(), tid3, topret_mode, basic_blocks, justify2) kcode
            let remark s = (vprintln 3 ("emit remark: " + s); G_emit gG (Xcomment s))
            // TODO crosscheck coincidence with the user's denotation of Kiwi.EndOfElaboration when present.
            // Note, end of elaboration can be any point on the lasso stem, not always the end of the stem, and some apps exit without any eternal loop.

            let print_constants = !g_gtrace_vd>=5
            
            if not end_of_elabf then
                let mm = sprintf "KiwiC1 end of code encountered for tid %s without further code being required for run-time." tid
                remark mm
                vprintln 2 mm
                commit_hll_resets()
                app hbev_top_emit tailcode // This drives HSIMPLE ack (old way).
                // dev_println ("Premier: no end_of_elabf"); printenv 0 10 ("Env at end of operation " + tid3.id) env                    
                (env, gG)
            else
                let mm = sprintf "End of KiwiC1 end-of-static-elaboration. Simple stem find for tid %s. After all generate loops are unwound we arrive here: %i.   UsersEndOfElab=%s (normally the same basic block but a different code index. This needs aligning before a mismatch error/warn can be issued." tid runtime_entry_point (if nonep !cCP.marked_end_of_elab then "None so far" else i2s(valOf !cCP.marked_end_of_elab))
                remark mm
                vprintln 3 (sprintf "Rollback (fseek-like) hbev_t code writer pointer from %i to %i for (re-)elaboration of bpc=%i" !gG.idx fidx runtime_entry_point)
                gG.idx := fidx // Never a roll forward.
                let _ = WF 3 "KiwiC-stages" ww mm

            // Step 2 - recursion static elaborate and constant folding
                let _ = WF 3 "KiwiC-stages" ww (sprintf "tid=%s: Start of KiwiC2 constant folding" tid)            

                let (const_in, const_out, reachable_basic_blocks) = constant_fold_meets (WN "elab5 constant_fold" ww) linepoint_stack tTt_step2_fold (env, runtime_entry_point) xition basic_blocks justify kcode

                let _ = 
                    if print_constants then // report all constants determined
                        vprintln 3 (sprintf "\n\nConstants determined for thread %s: they are:" tid)
                        printenv 3 10 ("Env at start of thread " + tid) env
                        vprintln 3 (sprintf "Constants determined report end. tid=%s\n\n" tid)
                        ()

                
                let _ = WF 3 "KiwiC-stages" ww (sprintf "tid=%s: End of KiwiC2 constant folding" tid)
            
                if !cCC.dyno_dispatch_updated then
                    vprintln 1 (sprintf "\n\n\n\nNeed fresh kcode re-elaborate for tid=%s" tid)
                    kcode_iterate (kiteration+1)
                else
                    // cmds is kcode not yet inside its DIC
                    // Old call site for populate: please delete.
                    //populate_ataken ww "L4453" tid cmds // Must do this before rebuild of tree env since store class references are changed. No longer true I think but need to do it before colouring - hmmm the populate can see through the colouring anyway.

                    commit_hll_resets()
                    //Step 3 - hgen - render the remaining code as an HPR hbev DIC to the next recipe stage.



                    let _ = WF 3 "KiwiC-stages" ww (sprintf "tid=%s: Start of KiwiC3 hgen hbev VM2 code generation" tid)
                    let nb = length reachable_basic_blocks

                    let sorted_basic_blocks = // A sort into ascending order is not purely cosmetic: it also reflects the natural flow from one block to another and this will not be upset by missing unreachable blocks.
                        let bpp (a, _, _, _) (b, _, _, _) = a-b
                        List.sortWith bpp reachable_basic_blocks
                    let hwm = if nb > 0 then (rev>>hd>>f2o4) sorted_basic_blocks else -1
                    //dev_println(sprintf "hwm = %A" (hd sorted_basic_blocks))

                  
                    let sorted_basic_blocks =  //We cannot assume the runtime entry point bb is the lowest numbered, so move it first
                        dev_println "TODO: check no natural flow into runtime ep"
                        let qf cc arg = if f1o4 arg=runtime_entry_point then Some arg else cc
                        let ep = valOf_or_fail "missing-ep-block" (List.fold qf None sorted_basic_blocks)
                        ep :: lst_subtract sorted_basic_blocks [ep]
                    let rec elab5_render_control = function
                        | [] -> ()
                        | (bb, idx)::tt ->

                            let nexto = match tt with
                                           | (bb, _)::_ -> Some bb
                                           | _ -> None
                            if vd>=4 then vprintln 4 (sprintf "Rendering basic block %i/%i    %s" idx nb (bb2str bb))
                            let cre = mkcre(valOf_or_fail  ("no env stored " + bb2str bb) const_in.[f1o4 bb])
                            if print_constants then printenv vd 10 (bb2str bb + sprintf ": The constant env for block elaborate entry is:") (f3o3 cre)
                            elab5_render (WN "elab5_render_control" ww) linepoint_stack tTt_step3_codegen justify (tid3, topret_mode, kcode, hwm) cre bb nexto
                            if exit_block<>None && f1o3(valOf exit_block)=(f1o4 bb) then app hbev_top_emit tailcode
                            elab5_render_control tt

                    elab5_render_control (zipWithIndex sorted_basic_blocks)
                    let env' = if exit_block=None then env (*arbitrary!*) else valOf_or_fail "exit block" const_out.[f1o3(valOf exit_block)]
                    hbev_top_emit (Xcomment "End of DIC comment 1")
                    (env', gG)

    let (env', gG0) = kcode_iterate 0 //Iterate all 3 stages until dynamic dispatch all resolved.
    let dic_ctrl = { g_null_dic_ctrl with ast_ctrl= {g_null_ast_ctrl with id=idsx }}
    let (optimised, _) = assembler_vm (WN (mm + "Kiwi-4408-assembler_vm") ww) vd dic_ctrl true !gG0.DIC
    let _ = WF 3  ("Main iterate the 3 KiwiC stages" + hptos idl) ww "ending" // ("Ending: env items=" + i2s(length(fst env')))
    let dic =
        if true then optimised
        else SP_dic(!gG0.DIC, dic_ctrl) 


    //dev_println (sprintf "interim director spec %A" director_spec)

    let my_director =
        let ctor_flagging = constructor_name_pred (hd idl)
        let clk_override = if ctor_flagging<>0 then [E_pos g_construction_big_bang] else []
        kiwife_rez_director ww clk_override toplevel_mdt.clknets handshakes director_spec 

    if cCC.settings.postgen_optimise then kiwi_optimise_DIC (WN "kiwi_optimise_DIC" ww) vd my_director dic // mutates it
          
    //vprintln 1 (sprintf "recombine: not_a_serverf=%A ctorcode_length=%i" not_a_serverf  (length ctorcode))
    let (pre_execs, result_blk) =

        let ctorcode_h2 =
            match ctorcode with
                | hprsp -> hprsp
                //| CTOR_code_form_null -> []
                //| _ -> muddy "ctorcode L5341"

        if not_a_serverf then
            // When a server or pipeline accelerator stage, keep any ctor code outside of main body DIC.
            // Alternatively, for the primary app or main entry point, prepend ctor code to start of main body DIC.
            ([], H2BLK(my_director, gen_SP_seq(ctorcode_h2 @ [ dic ]))) // When not a server, prepend ctor code inside main body.
        else
            let ctorx = if nullp ctorcode_h2 then [] else [H2BLK(my_director, gen_SP_seq ctorcode_h2)]
            (ctorx, H2BLK(my_director, dic)) // When a server, keep ctor code outside of main body. 

   
    if not not_a_serverf then
        let (bus_definition_name, netlevel, handshakers, method_name, signat, parameters, signals, fsems__) = valOf busDef_handy_info
        vprintln 2 (sprintf "Can now do some static analysis on generated code for fsem and other metric descriptions.")
        let ww = WF 2 "Analyse remote-callable method body" ww "Start"
        let m_sh = ref []
        let m_eisf = ref false
        let report_areaf = false
        let queries =
          { g_null_queries with
                yd=              YO_null
                concise_listing= true
                full_listing=    false
                logic_costs=     Some(logic_cost_walk_set_gen ww vd "analyse-remote-method" report_areaf)
                m_eisf=          Some m_eisf
          }
        anal_block ww (Some m_sh) queries result_blk
        let ww = WF 2 "Analyse remote-callable method body" ww "Finished"

        let (expected_latency, reinit_latency) = (-1, 0) // This info needs to be put in later on down the recipe, at end of restructure. So we should not write the file here.
        let fsems = // TODO abstract this code to abstracte.fs and also check all the other fsems
            vprintln 2 (sprintf "Static analysis determined eisf=%A" !m_eisf)
            let fsems = if !m_eisf then { fsems with fs_eis= true} else fsems
            fsems
        protocols.render_ip_xact_export_busDefinition ww protocols.g_autometa_prefix bus_definition_name netlevel signals parameters method_name (expected_latency, reinit_latency, fsems)


    let execs = if not_nonep asserty then execs else result_blk::execs
    let servers = [] // OLD code - this was used before restructure load balanced over servers.
    let childs = (map (gen_server_instance ww) servers) @ childs
        
    let rules =
        if not_nonep asserty then
            let doar rules =
                let _ = WF 3 "Call xrule on assertion method" ww "start"
                let rules = if asserty<>None then muddy "(xrule_sp ww (valOf asserty) (H2BLK(my_director, r)))::rules" else rules
                let _ = WF 3 "Call xrule on assertion method" ww "finished"
                rules
            doar rules
        else rules

    ((decls, pre_execs, execs, childs, rules, flogs, env', f22, f33, t33), externally_instantiated) // end of kiwic_start_toplevel_method


(*
 *  Start a thread at a method entry point.  The method might be an assertion or an RPC stub.
    Is env already in c ... duplication: does no harm?
 *)
let rec kiwic_start_thread ww tid3 director staticated (cCL0:cil_cl_dynamic_t) ctorcode m_spawned_threads (ignore_returnf, keep_returnf) srcfile hls_style m_shared_var_fails cxc (ep, actuals_opt, mM) = 
    let cCC = valOf !g_CCobject

    let _ = 
        let (decls, pre_execs__, non_ctor, sons, rules, flogs, env, f22, f33, t33) = cxc
        //printenv 0 10 "kiwic_start_thread" env
        ()
        
    match mM with
    | No_knot(CT_knot(idl, whom, k), noknot) when not(nonep !noknot) ->
        let mM = valOf !noknot
        kiwic_start_thread ww tid3 director staticated cCL0 ctorcode m_spawned_threads (ignore_returnf, keep_returnf) srcfile hls_style m_shared_var_fails cxc (ep, actuals_opt, mM)

    | No_method (srcfile, flags, callkind, (idl, uid, disamname), mdt, flags1_, instructions, atts) ->

        vprintln 1 (sprintf "Start compile of thread %s  hls_style=%A" (hptos idl) hls_style)
        kiwic_start_toplevel_method ww director staticated (cCL0:cil_cl_dynamic_t) tid3 ctorcode m_spawned_threads (ignore_returnf, keep_returnf) srcfile (ep, actuals_opt, mM, cxc) (uid, idl, mdt) m_shared_var_fails

    | other -> sf (sprintf "kiwic_start_thread: form other %A" other)

type cil_orange_t = 
   | CIL_orange_machine1 of hpr_machine_t
   | CIL_orange_data of cilexp_t


let childer = function
    | (idl, CIL_orange_machine1 x) -> (idl, x)
    |  other -> sf (sprintf "childer: Bad form for child HPR machine at this point %A" other)


(*
 * Treewalk calls classwalk which calls kiwic_start for each thread.
 * When control returns to treewalk, it calls kiwic_end for the execs and decls collected.
 *
 * This function closes off a kiwic_start to create an HPR machine. It works on a class or a thread.
 * When the machine has more than one thread, for any reason, ... ?
 *
 * Returns an HPR VM and an env (which can be used as concourse env to eval subsequent threads.
 *)
let kiwic_end ww tid3 suffix_no ck idl external_instancef mdt dBRCAF =
    let cCC = valOf !g_CCobject
    let (decls, pre_execs, execs, sons, rules, flogs, env, spawned22, invalidates33, ties33) = dBRCAF

    let claim_additionals_nets() =
        let rr = !cCC.m_additional_net_decls
        cCC.m_additional_net_decls := []
        rr
        
    let mmode = styleToStr tid3.style
    let tid = [ tid3.id ]
    let _ = WF 3 "kiwic_end thread" ww (sprintf "tid=%s hls-style=%A" (hptos tid) tid3.style)

#if OLD
    // Create special net for the return value - pre heapdig
    let (rv, args) =
        if nonep rt || nonep !cCC.m_old_headargs_ then []
        else
            let (args, rv) = valOf !cCC.m_old_headargs_
            if nonep rv then [] else [ f3o4 (valOf rv) ]
        //_rpc && valOf(!cCC.topret_rpc) <> None then [f3o3 (valOf(valOf(!cCC.topret_rpc)))]
#endif
            
    let iname = hptos tid + (if suffix_no>0 then sprintf "_%i" suffix_no else "")
    
    let assertions = rules (* For now *)

    let hls_signature =
        match mdt.fsems with // Create a basic gis in case this synthesis unit is to be later used as an FU.
            | None -> None 
            | Some fsems ->
                let rv = lower_type ww "hls_signature_rv" mdt.rtype // Need to de-template if abstract, but we do not support abstract compilation roots yet.
                let f2a (rpc_, idx_, dt, fid) = lower_type ww "hls_signature" dt
                let args = map f2a mdt.arg_formals
                Some { g_default_native_fun_sig with fsems=fsems; rv=rv; args=args } // Just the basics/basis are/is put in at this stage.

    let decls =
        let fg = 
            let further = claim_additionals_nets()
            vprintln 3 (sprintf "%i further nets claimed" (length further)) // Somewhat crude
            if nullp further then []
            else
                let cpi = { g_null_db_metainfo with kind= "kiwic-decls"; pi_name=iname;  not_to_be_trimmed=false;  }
                gec_DB_group(cpi, map db_netwrap_null further) 
        fg @ decls

    let ii = { rez_kiwi_ii iname [ iname; "kiwi" ] external_instancef with
                 definitionf= true
             }

    let ctor_machines =
        if nullp pre_execs then []
        else
            let kname1 = { ii.vlnv with kind="ctor" :: ii.vlnv.kind }
            let ii = { ii with vlnv=kname1 }
            let minfo = { g_null_minfo with name=kname1; atts= [ Nap("comment-note", "pre-execs-ctor-machine")] } // memdescs are all put in the top one
            let ctor_machine = HPR_VM2(minfo, [], [], pre_execs, [])
            [(ii, Some ctor_machine)]
            
    let main_machine =
        vprintln 2 (sprintf "main VM2 made %s: %i sons, %i declarations, %i execs, %i assertions" (hptos ii.vlnv.kind) (length sons) (length decls) (length execs) (length assertions))
        if nullp sons && nullp decls && nullp execs && nullp assertions then
            vprintln 2 "Discarding empty machine"
            []
        else
            let minfo = { g_null_minfo with name=ii.vlnv; atts= [ Nap("MMODE", mmode)]; hls_signature=hls_signature } // memdescs are all put in the top one
            let decls = db_duplicates_trim ww decls
            [(ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions)))]
    let _ = WF 3 "kiwic_end" ww (sprintf "KiwiC: Created %i main VM2 machines, monica %s, and %i ctor machines." (length main_machine) (vlnvToStr ii.vlnv) (length ctor_machines))
    (ctor_machines @ main_machine, env, spawned22, invalidates33) // end of kiwic_end


//
// We collate (steal) the big_bang code from the various classes in use.
// This will be constructor code, ready for catenation. 
// Also, redirect mid-body Xreturns to the end point and remove any trailing Xreturn instruction at the end point.
// This will be placed as a prefix on the 'main' thread of an application.  Where we are compiling a server or pipeline stage then it will go in a freestanding vm.
//
let kill_return msg exec =
    let is_return = function
        | Xreturn _ -> true //
        | other     -> false

    let is_comment = function
        | Xcomment _ -> true//            vprintln 0 "is return yes"
        | other      -> false//           vprintln 0 (sprintf "not a return %A" other)
    let is_label = function
        | Xlabel   _ -> true
        | other      -> false

    let rec kill_return_core = function // Mutates the existing DIC
        | SP_dic(ar, id) ->
            let len = ar.Length
            let ndl =
                match ar.[len-1] with
                | Xlabel s -> s

                | Xcomment q ->
                    let ndl = funique "DESTWASCOMMENT"
                    Array.set ar (len-1) (Xlabel ndl)
                    ndl
                | Xreturn q ->
                    //dev_println(sprintf "patch Xreturn operating")
                    let ndl = funique "DESTWASRETURN"
                    Array.set ar (len-1) (Xlabel ndl)
                    ndl
                | other ->  sf (sprintf "Last command is not suitable for exit from dic block: must be a comment, label or return: was %A" other)
            let qkill q =
                if is_return ar.[q] then
                    vprintln 3 (sprintf "Jump to end at %i" q)
                    Array.set ar (q) (Xgoto (ndl, ref -14))
            app qkill [ 0 .. len - 2 ]
        | SP_seq lst
        | SP_par(_, lst) -> app kill_return_core lst
        | _ ->
            hpr_yikes ("Did not kill return on non DIC: " + msg)
            ()
    kill_return_core exec
    exec
    
let rip_exec_code1 msg (decls, pre_execs__, execs, sons, rules, flogs, env, f22, f33, t33) =
    let strip_dir_and_kill_ret = function
         | H2BLK(dir, h2sp) ->
             kill_return msg h2sp

    let (ctor_code, non_ctor) =
        let bbp arg =
            let ans = 
                match arg with
                    | H2BLK(dir, bod) -> nullp dir.clocks || de_edge(hd dir.clocks)=g_construction_big_bang
            //vprintln 0 (sprintf "bbp pred on %A -> %A" arg ans)
            ans
        groom2 bbp execs

    // Should/could just xfer into pre_execs .... TODO
    let na = (decls, pre_execs__, non_ctor, sons, rules, flogs, env, f22, f33, t33)
    vprintln 3 (sprintf "rip_exec_code: %s constructor block count=%i" msg (length ctor_code)  + ", others=" + i2s(length non_ctor))
    (map strip_dir_and_kill_ret ctor_code, na)


let rip_exec_code2 vd msg cxxc =
    let cCC = valOf !g_CCobject
    let rip_son arg (sons, c1c) =
        match arg with
            | (iinfo1, Some(HPR_VM2(minfo1, decls1, sons1, execs1, assertions1))) -> // Note: does not recurse on sons. Could do.
               let rip_exec exec (e1c, c1c) =
                   match exec with
                       | H2BLK(dir, h2sp) when  nullp dir.clocks || de_edge(hd dir.clocks)=g_construction_big_bang ->
                           //dev_println(sprintf "rip_exec_code2 operating")
                           let nv = kill_return msg h2sp
                           (e1c, nv :: c1c)
                       | other -> (other::e1c, c1c)
               let (execs, cn) = List.foldBack rip_exec execs1 ([], [])
               let c1c = cn @ c1c

               if nullp decls1 && nullp sons1 && nullp execs && nullp assertions1 then (sons, c1c)
               else ((iinfo1, Some(HPR_VM2(minfo1, decls1, sons1, execs, assertions1)))::sons, c1c)
            | other -> (other::sons, c1c)

    let (sons, env, ctorcode, s22, inv33, ties33) = cxxc
    //dev_println (sprintf "%i existing readout sets vd=%i" (length ties33) vd)
    let ties = cCC.kcode_globals.ksc.readOut vd

    let (sons, c1) =
        let rip_son1 sons (e1c, c1c) = 
            let (a, c1c) = List.foldBack rip_son sons ([], c1c)
            (a::e1c, c1c) // Preserve list^2 form for sons. Not sure why we have it?
        List.foldBack rip_son1 sons ([], [])
    let cxxc = (sons, env, c1@ctorcode, s22, inv33, ties::ties33)
    cxxc


type root_temp_t = // Nasty temporary datatype while code migrates to a cleaner state!
    | RTEMP_method of Norm_t * int * specific_t * bool // Bool ignore_returnf overrides nature of specificf_t (not tidy!)
    | RTEMP_root of kiwic_root_t
(*
 * Start processing a thread, and any it spawns.  Each must have its own VM
 * under a common parent, since there are per-thread atts, including clock and retval 
 * (although clocks could go in the SP_BLK sensitivity).
 *)
let kiwic_strt ww tid3 (director_spec:director_spec_t) timespec staticated  mm srcfile (cCL:cil_cl_dynamic_t) (ctorcode) cxc rtemp =
    let m_spawned_threads = ref []
    let m_shared_var_fails = ref []
    let cCC = valOf !g_CCobject
    let vd = cCC.settings.hgen_loglevel
    let hls_style = tid3.style
    let check_staticated_ctor ww ctorcode (env:env_t) that epM =
        let msg = "staticated remote constructor"
        let mdt =
            match knotflatten epM [] with
                | [No_method(srcfile, flags, ck, id, mdt, flags1, instructions, atts)] -> mdt
                | _ -> sf "L4778 : get_mdt other form: method expected"

        if not_nonep mdt.is_remote_marked && not mdt.is_static && false then
            let (ctorcode, env) = 
                        // find ctor
            // analyse args for constant
            // generate a squirrel string
            // complaine if more than one ctor vector?
            // Pass in to get prefixed on the intcil elaboration.
                let class_idl = tl mdt.name
                let ctor_idl = ".ctor" :: class_idl
                dev_println(sprintf "Remote-spog constructor lookup on db. class=" + hptos class_idl)
                let saved_ctor_args = 
                    match cCC.m_remote_ctor_arg_db.lookup class_idl with
                        | [] ->
                            dev_println (sprintf "Kiwi.Remote: no ctor args need saved for " + hptos mdt.name)
                            []
                        | arg_vectors ->
                            dev_println (sprintf "Kiwi.Remote: entry point: retrieved %i vector(s) of consructor args saved for class %s" (length arg_vectors) (hptos class_idl))
                            snd(hd arg_vectors) // Just do the first one for now.  TODO.

                let remote_ctor =
                    match normed_ast_item_lookup00 ww ctor_idl with
                    | ([item], _) -> item
                    | _ -> sf ("The constructore for a staticated remote method was not found. remote_idl=" + hptos ctor_idl)

#if SPARE
                let ctorcode =                     // Copy 2/2 of constructor lookup code - this code is nicer since it handles overloaded constructors...
                    let virtualf = false
                    let m_notice = ref None
                    let signat = map f2o3 args // We just need the types for overload disambiguation.
                    let callsite = (tid3, ([tid3.id], -101))
                    match id_lookup_fn ww cCP cCB (fun(nd:int)->(sprintf ": Kiwi.Remote: constructor not (uniquely) defined.  Number of defs=%i.\n" nd, 1)) m_notice virtualf callsite ctor_idl signat with
                        | [] ->
                            vprintln 2 (sprintf "Missing constuctor for (no suitable overload?)  " + hptos ctor_idl)
                            []
                        | [ (idl_, ctor_method) ] ->
                            vprintln 2 (sprintf "Located phantom constructor code for staticated remote method. " + hptos ctor_idl)
                            [CTOR_code_form_method(ctor_method, args)]
                        | _ -> muddy (sprintf "More than one constructor exists for staticated remote method (and overload disambiguation support for this is insufficient at the moment!)   " + hptos ctor_idl)
                ctorcode
#endif
                let actuals =
                    let reorg (pos, dt, vale) = (dt, LOCAL, vale)
                    map reorg saved_ctor_args
                let (tailhangf_, ignore_returnf, keep_returnf) = (false, false, false)
                let ww = WF 2 ("Staticated remote constructor elaborate") ww "Start"
                let (dbra, _) = kiwic_start_thread ww tid3 director_spec staticated cCL [] m_spawned_threads (ignore_returnf, keep_returnf) srcfile hls_style m_shared_var_fails (gec_cil_blk [] env) (that, Some actuals, remote_ctor)

                // TODO check what other kiwi_end functionality we need pasted/invoked here

// Do we want static class constructor code?  This is a matter of whether the invokeees are really totally separate compilations  ...
                let (ctorcode', dbra) = rip_exec_code1 msg dbra
                let ctorcode = ctorcode @ ctorcode'
                let (decls, pre_execs__, non_ctor, sons, rules, flogs, env, f22, f33, t33) = dbra // better fold please?
                let ww = WF 2 ("Staticated remote constructor elaborate") ww "Finished"
                (ctorcode, env)

            (ctorcode, mdt, env)
        else (ctorcode, mdt, env)
       //CTOR_code_form_hbev ctorcode :: nb


        
    let (cxc, mdt, externally_instantiated) =
        match rtemp with
            | RTEMP_method(epM, ctor_flag, specificf, ignore_returnf) ->
                vprintln 1 ("Starting elaboration of a CIL thread: RTEMP_method, method=" + get_disamname epM)
                let finish_aswellas_hang = f5o5 director_spec (*.hang_mode*)=DHM_finish
                let finish_not_hang = finish_aswellas_hang // a subtle difference here needs nailing

                // If a constructor or  start-up code we do not tailhang.  If a forked thread we do. If the primary thread then its set in the director via recipe/cmdline.
                let tailhangf = ctor_flag=0 && specificf <> S_startup_code && (if specificf=S_root_method then not(finish_not_hang) else true)
                let did = "TSTART " + mm + " this=" + (if cCL.this=None then "None" else ceToStr(valOf(cCL.this)))
                let ww' = WF 3 did ww (sprintf "START COMPILE OF THREAD/EP. ctor_flag=%i  tailhang=%A   specificf=%A" ctor_flag tailhangf specificf)
                let keep_returnf = specificf <> S_startup_code
                let ignore_returnf = not keep_returnf || ignore_returnf // Not tidy!
                
                let (cxc, mdt) =
                    let (decls, pre_execs__, non_ctor, sons, rules, flogs, env, f22, f33, t33) = cxc // better fold please
                    let (ctorcode, mdt, env) = check_staticated_ctor ww ctorcode env cCL.this epM 
                    (gec_cil_blk [] env, mdt) //nasty really TODO

                let (DBRAF, externally_instantiated) = kiwic_start_thread ww' (tid3) director_spec staticated cCL (ctorcode) m_spawned_threads (ignore_returnf, keep_returnf) srcfile hls_style m_shared_var_fails cxc (cCL.this, None, epM) 
                let _ = WF 3 did ww ("FINISHED COMPILE OF THREAD/EP")
                (DBRAF, mdt, externally_instantiated)

            | RTEMP_root(Root_cc(tid3, USER_THREAD1(srcfile, object1, method1, spawntoken, nn, args, env), dir_, timespec)) -> // Uses env in scope when thread closure made rather than latest cxc - possibly wrong given we are compiling an imperative language that might rely on constructor call ordering/rules.
                vprintln 1 ("Starting elaboration of a CIL thread: RTEMP_root, method=" + (ceToStr method1) + " args=(" + sfold ceToStr args + ")")
                if vd>=4 then vprintln 4 ("Thread delegate  object=" + ceToStr object1 + "\n  method=" + ceToStr method1)
                let m =  hptos nn
                let _ = WF 3 ("lstart_thread") ww m
                let rec form_callable_del0 = function
                    //| CE_star(_, ce) -> form_callable_del0 ce
                    //| CE_dot
                    | CE_conv(dt, _, a) when dt = g_ctl_object_ref -> form_callable_del0 a
                    | CE_region(uid, dt, ats, _, cil) -> (dt, uid.f_dtidl)             
                    | CE_reflection(_, lto, Some idl, _, _) -> (valOf_or lto CTL_void, idl)
                    | CE_conv(dt, _, arg) ->
                        let (_, uid) = form_callable_del0 arg
                        (dt, uid)
                    | other -> sf ("KiwiC: Method or delegate is not thread-callable:" + ceToStr other)

                let (method_dt_, remote_idl) = form_callable_del0 method1
                vprintln 2 (sprintf " method uid.idl=%s" (hptos remote_idl))

                let epM =
                    match normed_ast_item_lookup00 ww remote_idl with
                    | ([item], _) -> item
                    | _ -> sf ("The entry point method for a thread (threads_method) was not found. remote_idl=" + hptos remote_idl)

                let rec ep_class_mine = function
                    | CE_conv(dt, _, r) when dt=g_ctl_object_ref -> ep_class_mine r 
                    | CE_region(uid, dt, ats, nemtok, cil) ->  Some(CE_region(uid, dt, ats, nemtok, cil))
                    | CE_var(uid, dt, idl, fdto, ats) when not(nonep(op_assoc "constant" ats)) -> Some(CE_var(uid, dt, idl, fdto, ats))
                    | CE_x(_, X_num _)
                    | CE_x(_, X_bnum _) ->
                        vprintln 3 (sprintf "Unsurprisingly, cannot find static forked thread's class instance  (remote_idl=%s) from void" (hptos remote_idl))
                        None

                    | other -> sf (sprintf "Cannot find forked thread's class instance (remote_idl=%s) from %s" (hptos remote_idl) (ceToStr other))

                let that = ep_class_mine object1
                let (ctorcode, mdt, env) = check_staticated_ctor ww ctorcode env that epM 
                let actuals =
                    let wrap_up_arg a = ((g_ctl_object_ref), LOCAL, a)
                    map wrap_up_arg args
                let keep_returnf = true
                let ignore_returnf = false
                let (DBRAF, externally_instantiated) = kiwic_start_thread ww tid3 director_spec staticated cCL (ctorcode) m_spawned_threads (ignore_returnf, keep_returnf) srcfile hls_style m_shared_var_fails (gec_cil_blk [] env) (that, Some actuals, epM)
                (DBRAF, mdt, externally_instantiated)

           

    let cxc = 
        reportx 3 "Threads spawned this run" (fun x->"Thread pending: " + threadToStr x) !m_spawned_threads
        reportx 3 "Threads invalidated for run" (fun (thrd_id, op_) -> "Thread invalidated: " + thrd_id) !m_shared_var_fails
        let (decls, pre_execs, execs, new_vms, rules, flogs, env, threadsf22, invalidates33, ties33) = cxc
        let wrap_root_cc thread =
            // Q. How do I spawn a thread in a different clock domain using C# delegates?
            let dir_spec = director_spec //  Default to parent's template.
            let time_spec = timespec // Makes spawned have same timespec as parent? Seems unlikely?
            let specific = S_spawned_method
            let tid3 = get_next_tid hls_style specific "TSX" // TODO need replay consistency
            vprintln 1 (sprintf "Allocated thread name %s for spawned method %s" (tid3.id) (threadToStr thread))
            Root_cc(tid3, thread, dir_spec, time_spec)
        (decls, pre_execs, execs, new_vms, rules, flogs, env, (map wrap_root_cc !m_spawned_threads) @ threadsf22, !m_shared_var_fails @ invalidates33, ties33)

    (cxc, mdt) // end of kiwic_strt




type phase_t =
    | P_ctor
    | P_fields
    | P_final
    
(*
 * Walk through the elements of a class, entering the static declarations in the heap.
 * On a second walk, constructor/kickoff code is collated on its own list.
 * On the third walk we get (make note of?) the main app code.
 * NOT CURRENTLY: Assertions are created from methods that have the appropriate attributes.
 *)
let rec classwalk_item ww director_spec time_spec staticated (cCL:cil_cl_dynamic_t, (hls_style_o:major_hls_mode_t option), idl, rpt) cgr0 phase tid3 suffix_no (mth, p, cc) =
    let gb0 = (cgr0, None)
    let gb0o = Some gb0
    let (sofar, flds, bindings, newdecs, env, ctorcode) = cc

    let report_writeln ss = (youtln rpt ss; vprintln 3 ss)
    match mth with
    | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1_, instructions, atts) ->
        let ids = hptos idl
        let msg = sprintf "classwalk method %s hls_style=%A of " ids hls_style_o  + ceToStr (valOf_or_fail "no CCL.this field set" cCL.this)
        let ww = WF 2 msg ww  "Start"
        //dev_println (msg + sprintf ": consider method compile during phase=%A" phase)
        let ctor_flag = constructor_name_pred(hd idl)
        let gids = hptos idl

        let doit = 
            // Only compile a nonconst method from this class if it is the last one ... epscan
            // Don't compile class constructors with nonzero arity (unless someone gives us some actuals!).  
            if phase=P_ctor && ctor_flag<>0 && nullp mdt.arg_formals
            then (report_writeln("Performing root elaboration of constructor " + noitemToStr false mth); Some "constructor")
            elif phase=P_final && ctor_flag=0 then
                (report_writeln("Performing root elaboration of method " + gids);
                 Some "user root"
                )
            else None
        
        if nonep doit then
            vprintln 2 (msg + sprintf ": not compiling root method during phase=%A" phase)
            cc
        else
            let keep = phase=P_final && ctor_flag=0
            vprintln 3 (msg + sprintf ": phase=%A : compiling root method %s (ctor_flag=%A) (gids=%s) keep=%A " phase ids ctor_flag gids keep)
            // Crossover - we remove the execs generated by a constructor and insert them as a prefix for a main compilation.
            let (ctorcode_inserted, ctorcode_ongoing) = if keep then (ctorcode, []) else ([], ctorcode)

            let hls_style =
                match hls_style_o with
                    | Some hls_style -> hls_style
                    | None -> sf "L5344"
#if OLD
            let _ =
                if nullp mdt.arg_formals || hls_style = MM_remote then ()
                else
                    vprintln 0 msg
                    let msg1 = (sprintf "+++ Entry point method style=%A has formal parameters (2/2).  Method name =%s " hls_style ids + " " + gids) // This was banned in the past.
                    vprintln 0 msg1
                    cleanexit msg1 // 2/2
#endif
            let specificf2 = if phase=P_final then S_root_method else S_root_class
            let (dbra, mdt_) = kiwic_strt ww tid3 director_spec time_spec staticated ("classwalk method " + valOf doit + ":" + ids) srcfile cCL (ctorcode_inserted) (gec_cil_blk bindings env) (RTEMP_method(mth, ctor_flag, specificf2, false(*ignore_returnf*))) // classwalk_item No_method call site
            vprintln 3 (sprintf "phase=%A : Keep method as a root/ep ids=%s  gids=%s   keep=%A" phase ids gids keep)
            let (ctorcode', dbra) =
                if not keep
                then rip_exec_code1 msg dbra
                else ([], dbra)
            let (son, env, spawned22_, invlidated33_) = kiwic_end ww tid3 suffix_no flags idl false mdt dbra // root item
            //dev_println ("Premier"); printenv 0 10 ("Env at end of operation " + tid3.id) env
            (son::sofar, flds, bindings, newdecs, env, ctorcode' @ ctorcode_ongoing)  (* local decls do not enter subsequent scope, but TODO this method should now be added to bindings. *)

    | No_field(layout, flags, name, fdt, atclause, custs) ->
        if phase <> P_fields then cc
        else
        //let _ = cassert(custs= [], "custs already inserted [CW]")
        let fF = No_field(layout, flags, name, fdt, atclause, custs @ p)
        let staticf = memberp Cilflag_static flags
        let prefix = (if cCL.text.cl_codefix = [g_globalname] then [] else cCL.text.cl_codefix)
        let full_idl = name :: prefix
        let skip = not staticf && staticated <> prefix
        let m0() =
            if staticf then "Incorporating static"
            elif nullp staticated then "Skipping non-static"
            elif staticated = prefix then "Including staticated field of top-level component"
            else sprintf "Skipping (%s != %s) non-static" (hptos staticated) (hptos prefix)
        let ww1 = WF 3 ("classwalk:" + m0() + " field") ww (hptos full_idl)
        if skip then cc
        else 
            let uid = if staticf then None else cCL.this
            let (cezo, env1) = static_field_def (WN "classwalk field def" ww1) None false g_dummy_realdo m0 prefix m0 cgr0 (Some env) fF
            // A static field is added to the current bindings and to the declarations, and it and its components are put on the heap.
            let _ = unwhere ww
            match cezo with
                | Some(blobf_, cez) ->
                     (sofar, (full_idl, (-1, cez))::flds, bindings, (full_idl, cez)::newdecs, (valOf env1), ctorcode)
                | None -> (sofar, flds, bindings, newdecs, env, ctorcode) 


    | No_class(srcfiles, flags, idl, CT_class dt, items, prats) when not (nonep dt.parent) -> cc // ignore nested class at this point. It will be lifted and compiled separately if needed.

    | other -> 
        vprintln 3 (noitemToStr false other + ": ignored in classwalk pass\n\n")
        cc

//
// A root that is a Remote target that is also called from another Root becomes a stub. So a single attribute 'Remote' is sufficient to denote
// client and server operations, with the specific action depending on whether the method is called or not from another hardware entry point.
and root_classwalk ww tid3 suffix_no director_spec time_spec staticated (ccl, (hls_style_o:major_hls_mode_t option), specificf, class_idl, uid, rpt) cgr c0 cls_items = 
    let cCC = valOf !g_CCobject
    //let ccl = { ccl with this= Some(CE_typeonly(Cil_cr_idl class_idl)) } 
    let mm = sprintf "root_classwalk hls_style_o=%A " hls_style_o + sprintf ", Specificf=%A target='%s'" specificf (hptos class_idl) + "' items=" + i2s(length cls_items)
    let report_writeln ss = (youtln rpt ss; vprintln 3 ss)
    vprintln 3 "\n\n----------------------------------------------------"
    let ww' = WF 2 mm ww "Start"    

    // OLD: We no longer do this.
    //A root class will be static (pre staticated support) so add it to the static classes constructed and record them as constructed in the mutable list.
    //mutadd cCC.m_classes_where_statics_constructed (class_idl, (uid, 0, ref true)) // Use ids as uid since there can only be one class of this name.

    let new_epscan cc item = // Find potential entry point for hardware-attributed class (remote or root marked) (new way with HardwareEntryPoint attribute - non specific -root)
        match item with
        | No_method(srcfile, flags, ck, (method_idl, uid, _), mdt, flags1_, ins, ats) ->
            let staticf = memberp Cilflag_static flags
            let ctor_flag = constructor_name_pred(hd method_idl)
            let _ =
                if not_nonep mdt.is_root_marked then vprintln 1 (sprintf "Detected rootmarked method as an entrypoint for %s %s rootmarked=%A" (hptos class_idl) (hptos method_idl) (valOf mdt.is_root_marked))
                elif not_nonep mdt.is_accelerator_marked then vprintln 1 ("Detected method designated as a pipelined accelerator (body will be compiled only if the method is _not_ called by another entry point) class_id=" + hptos class_idl + " method_idl=" + hptos method_idl)
                elif not_nonep mdt.is_remote_marked then vprintln 1 ("Detected remote method as an entrypoint (body will be compiled only if the method is _not_ called by another entry point) class_id=" + hptos class_idl + " method_idl=" + hptos method_idl)

                //else vprintln 3 ("Did not detect method class_idl=" + hptos class_idl + " method_idl=" + hptos method_idl + sprintf " as a root or remote.")
            let staticf1 = staticf || staticated = class_idl
            //dev_println (sprintf " %s  accelerator_marked=%A  staticf=%A staticf1=%A" (hptos method_idl) accelerator_marked staticf staticf1)
            if staticf1 && ctor_flag=0 && not_nonep mdt.is_root_marked          then (method_idl, item, "Root",        valOf mdt.is_root_marked, uid) :: cc 
            elif staticf1 && ctor_flag=0 && not_nonep mdt.is_accelerator_marked then (method_idl, item, "Accelerator", valOf mdt.is_accelerator_marked, uid) :: cc
            elif staticf1 && ctor_flag=0 && not_nonep mdt.is_remote_marked      then (method_idl, item, "Remote",      f1o5(valOf mdt.is_remote_marked), uid) :: cc
            else cc            
        | _ -> cc

        
    // Will use the-last listed entry point (if a class is marked as the root).
    let entry_points =
        if specificf <> S_root_class then
            vprintln 2 (sprintf " Not considering attributed entry points since specificf=%A" specificf)
            []
        else
            let entry_points = List.fold new_epscan [] cls_items    
            reportx 1 "potential class (static) method compilation root entry points (The bodies of Kiwi.Remotes will only be compiled if the remote method is _not_ called by a compiled entry point.)" (fun (idl, item, style, args, uid) -> sprintf "%A: %s uid=%s args=%s" style (hptos idl) uid (sfold (fun (fn,x)->fn+":"+xToStr x) args)) entry_points
            entry_points

   
    let mkpass ww (hls_style_o:major_hls_mode_t option) phase cc aa = 
        let ww' = WF 3 mm ww (sprintf " start pass: style=%A phase=%A" hls_style_o phase)
        let ans = foldl_withatts (classwalk_item ww director_spec time_spec staticated (ccl, hls_style_o, class_idl, rpt) cgr phase tid3 suffix_no) cc aa
        let _ = unwhere ww
        ans

    let c1 = mkpass (WN "mkpass-fields" ww') None P_fields c0 cls_items

    let c2 =
        if true then
            //vprintln 3 ("Skipping ctor pass now we have the earlier imports scan.")
            c1
        else mkpass (WN "mkpass-ctor-pass" ww') hls_style_o P_ctor c1 cls_items
    
    let (sons, flds, bindings, newdecs, env, ctorcode) =
        let passer cc = function
            | (idl, cls, hls_style_ss, args, uid) ->
                // TODO get default pausemode from args? Currently we need only the major mode here and the remainder is in bevelab/restructure2.
                let ids = hptos idl
                let zx = false // supressed ... memberp idl !cCC.m_called_methods // Check whether has been called by a root entry point.o
                let major_hls_mode =
                    if hls_style_ss = "Remote" then MM_remote
                    else
                    match args with
                        | []             -> MM_classical_hls
                        | [(_, X_num 5)] -> MM_pipeline_accelerator_hls
                        | [(_, W_string(accelerator_name, _, _)); (_, W_string(arg_ss, _, _))] ->
                            let args = split_string_at ['.'] arg_ss
                            dev_println (sprintf "TODO Need to check hls_style=%s and parse accelerator mode args from %s"  hls_style_ss arg_ss)
                            MM_pipeline_accelerator_hls                            

                        | other -> sf (sprintf "other major HLS mode/style unrecognised : %A" other)

                vprintln 2 (sprintf "consider %s main pass: called already=%A " ids zx + ",  e/p args=" + sfold (fun (fn,x) -> fn+":"+xToStr x) args)
                vprintln 2 (sprintf "consider %s main pass: called already=%A major_hls_mode=%A" ids zx major_hls_mode)              
                //vprintln 2 (sprintf "consider %s main pass: called already=%A " ids zx + ",  called=" + sfold hptos !cCC.m_called_methods) 
                if zx && hls_style_ss = "Remote" then // TODO avoid string form of the style
                    report_writeln("Skipping entry point associated with remote method that has been called directly already. idl=" + ids)
                    cc
                else
                report_writeln ("Compiling (static) method as entry point: style=" + hls_style_ss + " idl=" + hptos idl)
                mkpass (WN "mkpass-final" ww') (Some major_hls_mode) P_final cc [ cls ]
            

        if specificf<>S_root_class then c2
           else List.fold passer c2 entry_points


    let ww' = WF 3 mm ww "End"
    (sons, flds, bindings, newdecs, env, ctorcode)



// root_walker:
 // This function is called with and without specific set of root items.
 // For a root class, all static fields are rendered as hardware variables with
 // those that have I/O attributes becoming terminals and the remainder being locals.

 // Nonspecific: For a class that has a Kiwi.Hardware attribute, the last static method is the entry point for
 // converting behaviour to hardware.
 //
 // A function will have its exit/return code trimmed off if it is a constructor.  Otherwise it
 // will perform a return or tailhang.
 //
 // If a specific class, no method from that class is used: instead the method must be specified separately as another specific root.
 //
 // Any static method can be denoted as an entry point using -root.  Careful not to specify it twice
 // both with -root and Kiwi.Hardware attribute.
 //
// Where further static classes are encounted during traversal beyond the roots, the user used to have to specify them on the command line,
// but now we call their constructors automatically.
let rec root_compiler ww director timespec cCN mgr tid3 suffix_no arg cxxc =
    let (hls_style_o, ccl:cil_cl_dynamic_t, specificf, ignore_returnf, rpt) = cCN
    let (c0, env0, ctorcode0, spawned0, invalidates0, ties0) = cxxc
    let rec has_nonstatic_fields = function
        | [] -> None
        | No_field(abound, flags, name, fdt, atclause, attributes)::tt ->
            let staticf = memberp Cilflag_static flags
            if staticf then has_nonstatic_fields tt else Some name
        | _::tt-> has_nonstatic_fields tt
    match arg with
    | No_knot(CT_knot(idl, whom, t), v) ->
        if not_nonep !v then root_compiler (ww) director timespec cCN mgr tid3 suffix_no (valOf !v) cxxc
        else sf (hptos idl + " root knot left untied by " + whom)

    | No_class(srcfiles, flags, idl, CT_class cdt, items, prats) ->
        sf(hptos idl + ": root not concrete? type=" + typeinfoToStr (CT_class cdt))
        
    | No_class(srcfiles, flags, idl, CTL_record(_, cst, cdt, _, _, binds, _), items, prats) ->         
        let class_idl = if idl=ccl.text.cl_codefix then idl else idl @ ccl.text.cl_codefix // Kludge double prefixing.
        //vprintln 3 ("root_compiler: class compile: forming new prefix: extend " + hptos ccl.cl_prefix + " by adding " + hptos idl)
        let ids = hptos class_idl
        let staticf = memberp Cilflag_static flags // should be a field in cst
        let possibly_bad = if staticf then None else has_nonstatic_fields items
        // A so-called 'staticated' top-level module is a dynamic method that is being treated as a static method for the duration of its compilation.
        // Note the alternative ktop mechanism produces a fresh top-level instance for each root - this can be prevent and then we would have what we want?
        let staticated =
            if not_nonep possibly_bad then
                vprintln 1  ("root_compiler: Top-level root class " + ids + " was not static or contains non-static fields. KiwiC will make a dummy, top-level static instance for compilation.  Instance field=" + valOf possibly_bad)
                class_idl
            else
                []
        let cgr1 =
            let gacts = [] // no top-level templates provided by user. Copy 2/2
            Map.empty
            
        let mm = ("elaborating class '" + ids + "'" + " tid=" + tid3.id)
        youtln rpt (sprintf "kiwic root_compiler: start %s with hls_style=%A staticated=%A" mm  hls_style_o staticated)
        let _ = WF 3 "root_compiler" ww mm
        youtln rpt mm
        let dcl' =
            {
                this_dt=        ccl.text.this_dt
                cl_prefix=      class_idl
                cl_codefix=     class_idl
                cm_generics=    cgr1

            }
        let ccl' = { ccl with
                         text=          dcl'
                         cl_localfix=   class_idl                     
                   }
        let (sons, flds_, bindings_, newdecs, env, ctorcode) = (root_classwalk ww tid3 suffix_no director timespec staticated (ccl', hls_style_o, specificf, class_idl, ids, rpt) cgr1) ([], [], [], [], env0, ctorcode0) (items)
        vprintln 3 ("root_compiler class done: " + ids)
        youtln rpt ("root_compiler class done: " + ids)        
        let _ = unwhere ww
        vprintln 3 (sprintf "Root class elaborated: specificf=%A" specificf + " leftover=" + i2s(length ctorcode))
        (sons @ c0, env, ctorcode, spawned0, invalidates0, ties0)

    | No_method(srcfile, flags, ck, (idl, uid, _), mdt, flags1_, instructions, atts) -> 
        let constp = constructor_name_pred(hd idl)
        let ids = hptos idl
        let staticf = memberp Cilflag_static flags
        let staticated =
            if staticf then []
            else tl idl
        let msg = "root_compiler: method compile: entry point. Method name=" + ids + sprintf " uid=%s staticf=%A staticated=%A" uid staticf staticated
        let ww' = WF 1 "root_compiler" ww msg
        youtln rpt msg
                         
        let hls_style =
            match hls_style_o with
                | Some hls_style -> hls_style
                | None -> muddy "po hls_style L5760"
            
        // Crossover switch: if specificf then insert the ctorcode so far into this run and have none going forward.
        let (ctorcode_insert, ctorcode_ongoing) =
            if specificf=S_root_method then (ctorcode0, []) else ([], ctorcode0)
        let (dbraf, mdt_) = kiwic_strt ww' tid3 director timespec staticated ("tree_walk specificf: " + ids) srcfile ccl ctorcode_insert (gec_cil_blk [] env0) (RTEMP_method(arg, constp, specificf, ignore_returnf))  // root_compiler No_method callsite
        let (ctorcode, dbra) =
            if specificf<>S_root_method
            then rip_exec_code1 msg dbraf
            else ([], dbraf)
        let msg1 = (sprintf "Root method elaborated/compiled: specificf=%A" specificf + " leftover=" + i2s(length ctorcode) + "/new + " + i2s(length ctorcode_ongoing) + "/prev")
        vprintln 1 msg1
        youtln rpt msg1
        let ww = WF 3 "root_compiler" ww ("method compile: Now elaborated/compiled medhod " + ids)
        let (r, env, spawned22, invalidates33) = kiwic_end ww' tid3 suffix_no flags idl false(*external_instance*) mdt dbra // toplevel
        let ww = WF 3 "root_compiler" ww ("method compile: Now finished method " + ids)
        (r::c0, env, ctorcode @ ctorcode_ongoing,  spawned0 @ spawned22, invalidates0 @ invalidates33, ties0)

    //| CIL_custom _ -> c 
    //| CIL_imagebase _ -> c
    //| CIL_corflags _ -> c
    //| CIL_file _ -> c
    //| CIL_subsystem _ -> c
    //| CIL_stackreserve _ -> c
    //| CIL_assembly(flags, cr, flags1) -> c
    //| CIL_module(flags, cr) -> c
    | No_esc(CIL_data (l, r)) -> 
        let id = iexpToStr l
        vprintln 3 ("defined data " + id)
        sf "([id], CIL_orange_data r)::c"
    | other -> sf("root_compiler other form:" + noitemToStr true other)




#if NOT_IN_USE
(* Wide I/O busses code.
 * lostios : Convert back from VHDL-style array coding into verilog packed bit vectors, for I/O ports only.
 * not currently used but under discussion for re-use for very wide input busses.
 *)
let pack_bit_vectors vM =
    match vM with
    | HPR_VM2(minfo, decls, sons, a, b) -> 
        let io_net = function
            | INPUT -> true   (* TODO: this function is defined in sev places *)
            | OUTPUT -> true
            | RETVAL -> true
            | _ -> false

        let repack = function
            | X_bnet f ->
                if f.length <> []  && io_net(f.xnet_io)
                then
                    let length' = []
                    let w = encoding_width (X_bnet f)
                    let width' = asize(f.length)
                    let states = None
                    let ats = []
                    vprintln 3 ("Repack array transpose of " + f.id + " with new")
                    if (w <> 1) then sf(f.id + ": Array of vectors cannot become an RTL I/O port")
                    xgen_bnet(iogen_serf(f.id, length', 0I, int32 width', f.xnet_io, f.signed, states, false, ats)) 
                else X_bnet f
            | other -> sf "repack"

        reportx 3 "Repacking vars" netToStr topports
        let _ = map repack topports
        let rec pack = function
            | (ii, Some(HPR_VM2(minfo, ports, sons, execs, b))) -> 
               let sons = map pack sons
               let ports = map repack ports
               reportx 3 (sfold (fun c->c) minfo.name + " so ports's ") xToStr ports
               reportx 3 (sfold (fun c->c) minfo.name + " so oldecls") xToStr decls
               (ii, Some(HPR_VM2(minfo, ports, sons, execs, b)))
        snd(pack([], M))
#endif          



(*
 * Count how many identifiers nested in this item's definition to get its context.
 * For a method we wish to drop all but its name.
 *)
let rec dcount = function
    | No_knot(CT_knot(idl, whom, v), kno) when not(nonep !kno)-> dcount (valOf !kno)
    | No_class(srcfiles, flags, idl, _, items, prats) -> length idl
    | No_method(srcfile, flags, ck, id, mdt, flags1_, items, atts) -> 1
    | other -> sf("dcount other:" + noitemToStr true other)




(*
 * Support for bit-vector I/O.  An array of bits gets packed to finally become an RTL vector. 
 * This somewhat strange routine takes a pair (L,R) and returns a new form of R
 * that takes on the I/O attributes of L.
 * The magic behind this is that the same subscription operator in hexp_t was once used for both
 * array subscription and bit insert, so we do not need to change the code: just the declaration of the var, but now ...
 *)
let is_a_lostio (ats, len) =
    (at_assoc "io_output" ats <>None || at_assoc "io_input" ats <>None)


//
//
// Add in memory heap record info to the atts
let io_refinder ww CF lost_ios ((ff0:net_att_t, idl, ct, len, iot, ats), cc) =
    vprintln 3 ("Checking to see if net " + ff0.id + " len=" + i2s len + " " + varmodeToStr iot + " is a bitwise (lost) I/O")
    let f20 = lookup_net2 ff0.n
    let net = xgen_bnet (ff0, { f20 with ats=ats })
    if is_a_handle ct && (iot=INPUT || iot=OUTPUT) then cc 
    elif is_a_lostio (ats, len) then 
        (* Take R to pieces to regenerate again! *)
        let vd = 3
        let un = at_assoc "username" ats 
        let id' = if un<>None then valOf un else hptos_net idl
        let dk = function
                    | Nap(a,b)->(a,b)
                    //| Nap _ -> sf "io_refinder not nap"
        let net' = netgen_kernel ww "lost_io_def" (id', ct, Some len, map dk ats)
        if vd>=4 then vprintln 4 (": Reformatting bitwise (lostio) I/O\n old form L=" + ff0.id +
                                "\n new net is=" + netToStr net')
        net' :: cc
    else singly_add net cc




let gen_canned_array_addresser ww cc dname =
    let dname = dname + "`1"
    let method_name = "Address"
    let objptr = g_ctl_object_ref
    let addresser =
        gec_CIL_method false (Cil_cr dname) ([Cilflag_instance], [], objptr, Cil_cr method_name, 
               [Cil_fp(None, None, g_canned_i32, "i1"); Cil_fp(None, None, g_canned_i32, "i2")],
               [], 
               [
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_1);
                        gec_ins(Cili_ldarg Cil_suffix_2);                        
                        gec_ins(gec_instance_call(g_canned_i32, (Cil_cr dname, "KiwiSpecialAddress", []), [(Some "d1", g_canned_i32); (Some "d2", g_canned_i32)]));
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        // TODO times by object size - work in progress...
                        gec_ins(Cili_ldfld(objptr,  (Cil_cr dname, "arraybase", [])));
                        gec_ins(Cili_add Cil_signed);
                        ])

    let nv = ([method_name; dname], addresser)
    nv :: cc


// Pull together the obvious equivalence classes denoted by the memdesc_ties.
// g_null_aid, g_anon_aid: The null marker and the anon marker should not attract.  But they do in this naive implementation so it is disabled.
let simple_memdesc_tidy ww msg memdescs =
    if true then memdescs // Disable path owing to faulty code.
    else
        let ident = function
(*            
            | Memdesc0 of memdesc0_t
            | Memdesc_sc of int
            | Memdesc_scs of string

            | Memdesc_ind of memdesc_record_t      // Data array or pointer indirect annotation 
            | Memdesc_via of memdesc_record_t * string list // Field access annotation
*)
           | _ -> true 
        let estore = new EquivClass<memdesc_record_t>("ident")
        let cb _ _ _ = ()
        let grix cc = function
            | (_, Memdesc_tie lst) -> (ignore(estore.AddFreshClass cb lst); cc)
            | other -> other :: cc 
        let rx = List.fold grix [] memdescs
        let m_readout = ref []
        let boz (kv:KeyValuePair<string,(int * memdesc_record_t list)>) = mutadd m_readout ([kv.Key], Memdesc_tie(snd kv.Value))
        for k in estore do boz k done
        rx @ !m_readout


// Let's be clear about C# semantics for static classes.
// 1. We can define a class as 'public static class blah ...'
// 2. We can mark all the fields and methods in a class with the static modifier.
// 3. We can have a static handle on a class 'static blah that = new blah()'
// 4. Then there's any variations to do with structs instead of classes.
//
// Adding the static term to the class makes it abstract/sealed modifiers and removes the constructor for it. It is then not allowed to have member fields that are not also marked static.
// Having a static class handle does not create an instance of that class.
//
// Making it a struct introduces the 'valuetype' modifier and makes the class extend valuetype
// but does not create a static instance.
//
type ties_t = (int * (int * aid_t) list) list

type redo_t =
    | RED_pending 
    | RED_do_again of string 
    | RED_ok_result of (vm2_iinfo_t * hpr_machine_t option) list list * HPR_SP list(* not used? *) * ties_t list 


type workqueue_item_t = kiwic_root_t * redo_t ref




let prelim_cil_digest ww cCC top_asts =
    let vd = !g_filesearch_vd
    let ww = WF 2 "cil_digest" ww "initial scan of CIL classes"
    // Get a call graph with a pre-scan.  We use this to find which static class constructors to run.
    // TODO: currently unseeded so uses all presented classes and all methods in them.
    let cil_digest cc (is_exe, filename, ast) =
        let rec digester ww cro mro cc = function
            | CIL_class(srcfile, flags, cr, gformals_, extends, items) ->
                match safe_type_idl cr with
                    | Some cr2 ->
                        let cr2 = rev cr2 // The full name is present in the child so no special action needed for nested classes.
                        //if not (nonep cro) then muddy (sprintf "digest of nested class not yet ... %A in %A" (cr2) (cro))
                        let ww = WN ("cil_digest consider class " + hptos cr2) ww
                        if vd>=5 then vprintln 5 (sprintf "cil_digest: cls=%A meth=%A    --: %s" cro mro (hptos cr2))
                        let cc = (cr2, "")::cc
                        List.fold (digester ww (Some cr2) None) cc items
                    | None -> cc
                        
            | CIL_method(srcfile, cr, flags, ck, rtype, (id, unique_id), signat, flags1, instructions) ->
                match safe_type_idl id with
                    | None -> cc
                    | Some cr2 ->
                        let cr2 = rev cr2
                        let staticf = memberp Cilflag_static flags
                        let idl = cr2
                        let cpred = firstpass.constructor_name_pred (htos idl)
                        let cc = List.fold (digester ww cro (Some idl)) cc instructions
                        if staticf && cpred<>0 then
                            vprintln 3 (sprintf "cil_digest: consider class %s method %s staticf=%A cpred=%A" (htos cr2) (if nonep cro then "no-cro" else hptos(valOf cro)) staticf cpred)
                            let idl1 = valOf_or_fail "method without class" cro
                            if vd>=5 then vprintln 5 (sprintf "cil_digest: cls=%A meth=%A    --: %s" cro mro (hptos cr2))
                    //TODO note_static_class m_static_classes_possibly_used idl1 id
                            (idl1, "")::cc 
                        else cc


            | CIL_instruct(z, _, prefix_, d) ->
                match d with
                    // If we call a static method then we potentially have a static class that potentially has a .cctor that should
                    // be invoked before we start the main compile from the entry point.
                    // Cili_newobj is another one to scan
                    | Cili_call(opcode, ck, rt, (cr, id, _), signat, callsite_token_) ->
                        let (instancef, ohead) = thisohead(opcode, ck) 
                        let staticf = not instancef
                        match safe_type_idl cr with
                            | None -> cc
                            | Some cr2 ->
                                let cr2 = rev cr2
                                //if staticf then // static members of dynamic classes should also be included.
                                //note_static_class m_static_classes_possibly_used idl id
                                if vd>=5 then vprintln 5 (sprintf "cil_digest: cls=%A meth=%A    --: z=%A %s" cro mro z (hptos cr2))
                                (cr2, id)::cc
                                //else cc
                    | other -> cc
            | CIL_locals(locals_list, _) ->
                let digest_local cc = function
                     | P3(no, ctype, ident) ->
                         match safe_type_idl ctype with
                             | Some cr2 ->
                                 if vd>=5 then vprintln 5 (sprintf "cil_digest: cls=%A meth=%A    --: %s" cro mro (hptos cr2))
                                 (rev cr2, ident)::cc
                             | None     -> cc
                List.fold digest_local cc locals_list
            | _ -> cc
        let cc = List.fold (digester ww None None) cc ast
        cc

    let m_classes_possibly_used = cCC.m_classes_possibly_used                           //Used as a set.
    let m_known_to_be_missing = new OptionStore<stringl, bool>("known_to_be_missing")  //Also used as a set.
    // Need to iterate until closure, finding all the classes in use.
    let v0 = List.fold (cil_digest) [] top_asts
    // TODO add v0 to the used OptionStore

    let rec classes_needed_explore count (items:stringl list) =
        let newones =
            let takenote cc idl =
                match m_classes_possibly_used.lookup idl with
                    | Some _ -> cc
                    | None ->
                        m_classes_possibly_used.add idl true
                        singly_add idl cc
            List.fold takenote [] items
        // add items to m_static_classes_possibly_used
        //let v1 = map (fun (classname, _) -> (classname, (hptos classname, 0, ref false))) items
        vprintln 2 ( sprintf "classes_possibly_needed_explore cycle=%i newones=%s" count (sfold_unlimited hptos newones))
        if nullp newones then ()
        else     
            let check idl cc =
                match m_known_to_be_missing.lookup idl with
                    | Some _ -> cc
                    | _ ->
                        let normed_ = normed_ast_item_lookup_wb_wfail ww false Map.empty idl
                        match op_assoc idl !cCC.directory with
                            | Some ast ->
                                let xform (is_exe, srcfilename, ast) =  (is_exe, srcfilename, [ast]) 
                                let v2 = (cil_digest) [] (xform ast)
                                (map fst v2) @ cc
                            | None ->
                                vprintln 3 (sprintf "cil_digest: normed item %s not found in ast form" (hptos idl))
                                m_known_to_be_missing.add idl true
                                cc
            let v3 = List.foldBack check newones []
            classes_needed_explore (count+1) v3

    let _ = classes_needed_explore 0 (map fst v0)
#if SPARE        
    let _ = 
        let aux_cctor_toStr (idl, (uid, priority, donef)) = sprintf " static class? %s priority=%i done=%A" (hptos idl) priority !donef
        reportx 3 "Auxilliary static classes" aux_cctor_toStr v1 //!m_static_classes_possibly_used

#endif
    let ww = WF 2 "cil_digest" ww "initial scan of CIL classes finished"        
    let m_static_classes_possibly_used = ref [] // for now
    m_static_classes_possibly_used


let kiwic_install_library_substitutions ww cCC mappings =
    let ww = WF 2 "kiwic_install_library_substitutions" ww "Start"
    let vd = 3
    let install (fromlib, tolib, flags_) = // These are read in using human-readable order, so need reversing for internal storage.
        let fromlib = rev fromlib
        let tolib   = rev tolib
        if vd>=3 then vprintln 3 (sprintf "Install library substitution %s to %s" (hptos fromlib) (hptos tolib))
        cCC.library_substitutions.add fromlib tolib
        ()
        
    app install mappings
    ()



let kiwic_main_factory (kfec1:kfec_settings_1_t) kfec2 =
    g_kfec1_obj := Some kfec1
    let stagename = "kiwife"
    let heap:heap_t =
        {
            sheap1 = new sheap1_t("sheap1")
            sheap2 = new sheap2_t("sheap2")
            memdescs = new proto_memdesc_store_t "proto_memdescs"
            scalar_singleton_sbrk= ref g_ssm_static_scalar_base
            vector_singleton_sbrk= ref g_ssm_static_vector_base            
        }

    // Wine is making mono return Windows NT etc as the O/S.  You should run KiwiC via mono and not via automatic invokation via wine on some linux variants.
    let ban = kiwic_banner + " " + get_os()
    vprintln 1 (ban + "\n" + timestamp(true) + "\n")
    g_version_string := ban
    //vprintln 0 ("OS is " + System.Environment.OSVersion.VersionString) //System.Diagnostics.IsWindows)




    
    // Readfun takes a number of asts and returns one vm2 that may be just a wrapper around multiple threads and server entry points.
    let readfun ww (sons0, topname, composite_root:string, c1:control_t) top_asts =
        if nonep !g_kiwife_topname_o then g_kiwife_topname_o := Some(hptos topname)
        //        let _ = g_current_log_verbosity := 100 // max out for now
        let cmdline_roots = map (fun x -> rev(split_string_at ['.'; ','] x)) (split_string_at [';'] composite_root)
        let cmdline_roots = list_subtract(cmdline_roots, [["$attributeroot"]])
        //vprintln 0 ("composite>" + composite_root + "<"); reportx 0 "cmdline root item split" (fun x->"'"+hptos x+"'") cmdline_roots

        let bin_libs = [] // inserted by hand in kiwi_canned now : was : map insert_bin_type bin_types

        // report will be re-opened and overwritten ? No readfun_serf is the one that iterates
        let _ =
            let summary_report_filename =
                if nonep !g_kiwife_topname_o then "KiwiC.rpt" else filename_sanitize [] (valOf !g_kiwife_topname_o) + ".rpt"
            set_opath_report_banner_and_name( 
                [  "Cmd line args: " + !g_argstring
                   timestamp(true) 
                   kiwic_banner 
                   "KiwiC compilation report"
                ], summary_report_filename)
                // List is printed in reverse, to allow postscripts to be added.
        let rpt = yout_open_report("KiwiC-fe.rpt")
        let cos = yout_divert_log(logdir_path("preambles.log"))
        yout cos (report_banner_toStr "REPORT :")
        vprintln 2 "Canned library processing : start"
        let canned = List.foldBack (fillout_canned_dir ww []) g_canned_libs bin_libs 
        let canned = List.fold (gen_canned_array_addresser ww) canned [ kfec1.d2name; kfec1.d3name; kfec1.d4name ] // How does this work for 1D arrays?
        vprintln 2 "Canned libraries now installed in KiwiC directory of available resources"


        let allow_hpr_alloc = (control_get_s stagename c1 (stagename + "-allow-hpr-alloc") None) = "enable"

        let default_timing_target =  // cf slack // Get default from recipe .... please
            {
              latency_target=  -1 
              ii_target=       -1
              hardness=        0.0
            }

        let directory =
            let xform (idl, ast) = (idl, (false, "<canned>", ast))
            ref (map xform canned) // Start the directory with the canned definitions.
        
        // Put PE/CIL code into a directory form, but norm it as items are needed by lookup.
        let (marked_roots, _) =
            let aliasp = ([], [])
            let install_ast cc (is_exe, srcfilename, ast) = foldl_withatts (install_in_cil_directory ww g_cd0 srcfilename is_exe directory ((kfec2.directorate_style, kfec2.directorate_attributes), None, aliasp)) cc ast
            List.fold install_ast ([], []) top_asts
        let marked_roots = rev marked_roots // Restore order of definition for compilation because constvol iterations will be reduced if writing methods (such as load_coefficients for a FIR) are compiled first.  

        let no_cmdline_roots = nullp cmdline_roots
        
        let marked_roots =
            let hmep_pred (cls_name, ((hls_mode, idl, uid, ((dirstyle, ats), timespec_o)), spog)) = (hls_mode = MM_root)
            let has_marked_entry_pt = disjunctionate hmep_pred marked_roots
            let refine (cls_name, ((hls_mode, idl, uid, ((dirstyle, ats), timespec_o)), spog)) =
                let skipped = has_marked_entry_pt && hls_mode = MM_remote
                (cls_name, ((hls_mode, idl, uid, ((dirstyle, ats), timespec_o)), spog), skipped)
            map refine marked_roots

            
        report_dir_contents1 3 "Final directory of CIL items"  (logdir_path "inscope.txt") !directory
        yout_close cos
        let _ = WF 3 ("KiwiC readfun") ww "entry"


        let marked_roots =
            if no_cmdline_roots then // Ignore the empty placemarker
                vprintln 1 (i2s (length marked_roots) + " attribute-marked roots (entry points) in dotnet dll/exe input files")
                let _ =
                    let banner = "Entry Points To Be Compiled:"
                    let dToStr(dirstyle, ats) = sprintf "%A " dirstyle + sfold (fun (k,v) -> sprintf "%s/%s" v k) ats
                    let gen_table_line (cls_name, ((hls_mode, idl, uid, ((dirstyle, ats), timespec_o)), _), supressed) = [ hptos cls_name; styleToStr hls_mode; dToStr(dirstyle, ats); (if nonep timespec_o then "" else timespecToStr(valOf timespec_o)); hptos idl; uid; sprintf "%A" supressed ]
                    let table = tableprinter.create_table(banner, [ "Class"; "Style"; "Dir Style"; "Timing Target"; "Method"; "UID"; "Skip" ], map gen_table_line marked_roots)
                    let t1 = table   
                    //mutadd settings.tableReports t1
                    aux_report_log 1 "kiwife" t1
                marked_roots
            else
                vprintln 1 (sprintf " Ignore %i attribute-marked roots (entry points) in input dotnet dll/exe files and using command-line root control instead." (length marked_roots))   
                []
                
        //vprintln 0 (sfold (fun (meth, cls) -> classitemToStr false cls) marked_roots + " yaya roots")


        let already_initialised_vars = new already_initialised_vars_t("already_initialised_vars")
        let _ =
            already_initialised_vars.add [g_tnow_string] 1 // This "tnow" mechanism needs to be left unbound so later recipe stages can perform their tricks.




        let setup_bondout_spaces ww c1 rptcos =
            let ww = WF 2 "setup_bondout_spaces" ww "start"
            let m_settings_report = ref []
            let log_setting_for_report key def vale desc = mutadd m_settings_report (key, vale, desc)
            
            let bondout_ports = protocols.get_bondout_port_prams ww c1 stagename log_setting_for_report 

            let hdr = [ "AddressSpace"; "Name"; "Protocol"; "No Words"; "Awidth"; "Dwidth"; "Lanes"; "LaneWidth" ]
            // Note: there is a copy of this in abstracte
            let port_report (space, (port:bondout_port_t), manager) =
                [ space.logicalName; port.ls_key;  ipbToStr port.protocol; i2s(two_to_the32 port.dims.addrSize); i2s port.dims.addrSize; i2s(port.dims.no_lanes * port.dims.laneWidth); i2s port.dims.no_lanes; i2s port.dims.laneWidth ]

            // Custom interconnect between separate compilations also shows up here in the port list ??
            let portflattener ((space, ports), manager) cc = map (fun port -> (space, port, manager)) ports @ cc
            let table = tableprinter.create_table ("Bondout Load/Store (and other) Ports", hdr, map port_report (List.foldBack portflattener bondout_ports []))
            aux_report_log 2 stagename table
            //mutadd settings.tableReports table

            let _ =
                let headings = [ "Key";"Value"; "Description" ]
                let repfun (key, (vale:int64), desc) = [ key; i2s64 vale; desc ]
                //app repfun !m_settings_report
                let t1 = tableprinter.create_table("Bondout Port Settings", headings, map repfun !m_settings_report)
                app (fun x-> youtln rptcos x; vprintln 2 x) t1
                vprintln 2 "Printed Bondout Setting Summary"
                ()
                    //mutadd settings.tableReports t1 
            bondout_ports
                    
        let bondouts = setup_bondout_spaces ww c1 rpt 



        let cCC:cilconst_t = {  directory=                  directory  // Directory of all CIL code read in.
                                toolname=                   stagename
                                settings=                   kfec2
                                kiwicid=                    "The global cilconst scope"
                                control_options=            c1
                                kcode_globals=              setup_kcode_globals() 
                                already_initialised_vars=   already_initialised_vars

                                // Constant recipe settings should be in kfec_settings_1_t not here!
                                heap=                       heap 
                                //lost_ios2=lost_ios2 // Lost_ios is/was useful for wider than 64-bit I/O port paradigms
                                dyno_dispatch_updated=      ref false
                                next_reflection_int =       ref 0
                                reflection_constants=       ref []
                                //dyno_dispatch_closures=     new dyno_dispatch_t("dyno_dispatch_closures")
                                dyno_dispatch_methods=      new dyno_dispatch_t("dyno_dispatch_methods")

                                m_additional_net_decls=     ref []
                                m_additional_static_cil=    ref []
                                m_vregs=                    new vregs_database_t "m_vregs"
                                m_old_headargs_=            ref None
                                m_classes_possibly_used=    new m_classes_possibly_used_t("m_classes_possibly_used")
                                //m_classes_where_statics_constructed= m_static_classes_possibly_used
                                //m_called_methods=           ref []
                                rpt=                        rpt
                                next_virtual_reg_no =       ref 5000 // Some noticable, arbitrary starting values
                                next_negative_reg_no =      ref 1000000 // never printed out
                                reflection_icons=           new  reflection_icon_dbase_t("reflection_icon_dbase")
                                allow_hpr_alloc=            allow_hpr_alloc

                                bondouts=                   bondouts
                                constant_or_volatile=       new constant_or_volatile_dbase_t("constant_or_volatile")
                                m_remote_ctor_arg_db=       new m_remote_ctor_arg_db_t("m_remote_ctor_arg_db")
                                library_substitutions=      new library_substitutions_t("library_substitutions")
                        }
        g_CCobject := Some cCC


        let _ =
            let doit = control_get_s stagename c1 "kiwic-library-redirects" None = "enable"
            if doit then kiwic_install_library_substitutions ww cCC g_hardcoded_library_substitutions
            ()

        let _ = prelim_cil_digest ww cCC top_asts
            
        let ksc = cCC.kcode_globals.ksc
        let heapspace1 = CEE_heapspace(ref(g_ssm_heap_base, []), [], [])
        let env00 = { hs1=heapspace1; mainenv=Map.empty; }
        // Load some initial types into the dtable to avoid later verbosity - done for each root item!
        let ww11 = WN "preload basic types" ww
        let preload ss =
            let idl = [ ss; "System" ]
            let gb0 = []
            let ids = hptos idl
            let ww = WF 3 "preload" ww11 ("class_find and load of " + ids)
            let oc = id_lookupp ww idl Map.empty (fun()->(ids + ": class_find: class not defined: cr=" + ids, 1))
            let t_ = cil_gen_dtable (WN ("class_find gen_dtable " + ids) ww11) gb0 (RN_idl idl, valOf oc)
            ()

        app preload [ "RuntimeFieldHandle"; "Array"; "Object"; "String"; "Char"; "Int32"; "UInt32" ] // Why not float / Math - these are done dynamically.
        let vd = cCC.settings.hgen_loglevel
        vprintln 2 "Preamble/preload complete"


        let m_lastroot = ref ["anonroot"]
        let m_donesome = ref false
        let m_deferred_cctors = ref []
        
        let rec compile9 ww cxc arg =
            let gb0 = Map.empty

            let get_sf_key = function 
                | No_knot(CT_knot(idl, whom, v), noknot)          ->
                    hpr_yikes("Knots should be untied by here " + hptos idl)
                    (hptos idl, idl)
                | No_class(srcfiles, flags, idl, _, items, prats) -> (hptos idl, idl)
                | No_method(srcfile, flags, ck, (id, unique_id, disamname), mdt, flags1_, instructions, atts)  ->  (unique_id, [ disamname ])
                | other -> sf (sprintf "compile9: other sf_key form: %A" other)



            match arg with
                | Root_cc(tid3, (USER_THREAD1(srcfile, epclass, meth, spawntoken, nn, args, env) as spawned), dirstyle, timespec10) ->
                    let mm = "spawned thread root"
                    let staticated = []

                    let director_spec = dirstyle //gec_preliminary_director ww timespec10 dirstyle
                    let top_dt = get_ce_type mm epclass
                    let tcl = // It does not matter too much what is passed in as the cCL0 since thread start code extends and fills in correctly.
                        { this_dt=        Some(gec_CT_star 0 top_dt)
                          cm_generics=    gb0         
                          cl_codefix=     tl nn 
                          cl_prefix=      tl nn

                        }
                    vprintln 2 (sprintf "kiwic: compile9 user thread compile")
                    let cCL0:cil_cl_dynamic_t =
                        { this=           Some(gec_CE_star "topthis-site" 0 epclass)
                          text=           tcl
                          cl_localfix=    tl nn
                        }

                    let (dbraf, mdt) = kiwic_strt ww tid3 director_spec timespec10 staticated mm srcfile cCL0 [] (gec_cil_blk [] env) (RTEMP_root arg)
                    let flags = []
                    let externally_instantiated = false
                    let (sons10, env, spawned10, invalidates10) = kiwic_end ww tid3 0 flags nn externally_instantiated mdt dbraf // user spawned thread1
                    let ties = cCC.kcode_globals.ksc.readOut vd
                    let (sons, execs, _, ctorcode, sofar_compiled, spawned22, invalidates33, ties33) = cxc
                    //dev_println (sprintf "%i existing readout sets " (length ties33))
                    (sons10 :: sons, execs, env, ctorcode, sofar_compiled, spawned10 @ spawned22, invalidates10 @ invalidates33, ties::ties33)
                    
                | Root_cc(tid3, ROOT_THREAD1(class_idl, method_idl_o, uid), dirstyle, timespec10) ->
                    let idl = valOf_or method_idl_o class_idl
                    let rec unknot arg cc =
                        match arg with
                            | No_knot(ct_, no_knot) when not_nonep !no_knot -> unknot (valOf !no_knot) cc
                            | No_list lst -> List.foldBack unknot lst cc
                            | other -> other::cc

                          
                       
                    let (named_items, n_named_items) =
                        let (items, _) =
                            if hd idl = g_globalname then
                                let all_ast = list_flatten(map f3o3 top_asts)
                                ([No_esc(CIL_class("", [], Cil_cr_global, [], None, all_ast))], 1)
                            else normed_ast_item_lookup00 ww idl
                        let untied = List.foldBack unknot items []
                        let untied =
                            if length untied > 1 then  // All very longwinded! Please tidy up this knot and refine code!
                                let refine_selection_pred arg = (fst (get_sf_key arg) = uid)
                                let untied1 = List.filter refine_selection_pred untied
                                if length untied1 = 1 then untied1 else untied
                            else untied
                        (untied, length untied)
                    if n_named_items=0 then
                        if list_intersection_pred (idl, g_startup_names) then cxc
                        else
                        let _ =
                            match idl with //Dont flag missing static class constructors - many of the canned ones are missing.
                                | ".cctor" :: _ -> ()
                                | _ -> hpr_yikes (sprintf "Top-level class or method not found " + hptos idl)
                        cxc
                    elif n_named_items > 1 then
                        vprintln 0 (sprintf "+++ no such entry point for compilation : %s - none found" (hptos idl))
                        let sf_keys = map get_sf_key named_items
                        sf (sprintf "multiple (no=%i) root definitions with name %s: uid=%s:  sf_keys=%s" n_named_items (hptos idl) uid (sfold fst  sf_keys))
                    else

                    let named_item = hd named_items
                    let sf_key = get_sf_key named_item // squirrel key (to avoid compiling the same component twice wrongly) -- no longer needed?
                    let (sons, execs, env, ctorcode, sofar_compiled, spawned22, invalidates33, ties33) = cxc
                    if memberp sf_key sofar_compiled then cxc
                    else

                    let director = dirstyle // gec_preliminary_director ww timespec10 dirstyle
                    let prepended =
                        match op_assoc class_idl !m_deferred_cctors with
                            | Some ctor_idl ->
                                let (items, n_items) = normed_ast_item_lookup00 ww ctor_idl
                                if n_items = 1 then
                                    [(true, hd items)]
                                else
                                    vprintln 0 (sprintf "deferred class constructor missing (%i): " n_items + hptos ctor_idl)
                                    []
                            | None -> []
                    //dev_println (sprintf "toptop weds: sf_key=%A prepended=%i items=%i" sf_key (length prepended) 1)
                    let items = prepended @ [(false, named_item)]

                    let top_dt = template_eval (WN "root static class type" ww) ({grounded=false; }, gb0) (Cil_cr_idl class_idl)                   
                    //let ids = hptos (snd sf_key)
                    let m0() = sprintf "Start compile9 of %s " (hptos class_idl) + "  " + cil_typeToStr (top_dt)
                    // Create a static instance record for this top-level class.

                    let ((_, topthis), env00_) =
                        let ethereal = true
                        gec_uav_core ww vd m0 ethereal (Some false) STOREMODE_singleton_scalar g_dummy_realdo false top_dt (funique "ktop", [("constant", "true")]) 0 None (Some env) None

                    let tcl =
                        { this_dt=        Some(gec_CT_star 0 top_dt)
                          cm_generics=    gb0         
                          cl_codefix=     class_idl
                          cl_prefix=      class_idl
                        }

                    vprintln 2 (sprintf "kiwic: compile9 root class dt=%s" (if nonep tcl.this_dt then "NONE" else cil_typeToStr (valOf tcl.this_dt)))
                    let cCL:cil_cl_dynamic_t =
                        { this=           Some(gec_CE_star "topthis-site" 0 topthis)
                          text=           tcl
                          cl_localfix=    class_idl 
                        }

                    // This must be folded (not foldback) so that the env generated by the cctor is set up for the entry pooint.  But the generated result in execs needs appending in the correct order.
                    let root_compiler_wrapper cxxc ((___, an_item), suffix_no) = 
                        let tokens = dcount an_item
                        let drop n l =
                                let err() =
                                    hpr_yikes ("*** DROP ERROR in " + hptos l)
                                    hpr_yikes ("*** Trying to drop " + i2s n + " path tokens from the " + i2s (length l) + " tokens in '" + sfold (fun x->x) l + "'")
                                    hpr_yikes ("*** This was the number of tokens in the prefix '" + (noitemToStr false an_item) + "'")
                                    sf "cannot drop prefix"
                                let rec d1 n l = if n=0 then l else if l=[] then err() else d1 (n-1) (tl l)
                                d1 n l

                        let idl' = drop tokens class_idl (* The id_lookup discards all but last-nested id *) // The staticated elaboration path.

                        //vprintln 0 (sprintf "Munich: idl' = '%s' for this root item called  '%s' " (hptos idl') (hptos idl))
                        let tcl' = { tcl with cl_codefix=idl' }
                        let cCL' = { cCL with
                                        cl_localfix=  idl'
                                        text=         tcl'
                                   }
                        let ignore_returnf = false // Do not need this now we have the rip_exec_code2 below
                        let cCN = (Some tid3.style, cCL', tid3.specific, ignore_returnf, rpt)
                        let msg = sprintf "KiwiC: front end input processing of class %s  wonky=%s igrf=%A" (hptos class_idl) (hptos idl') ignore_returnf
                        let ww' = WF 3 msg ww "start"
                        youtln rpt msg
                        let cxxc = root_compiler ww' director timespec10 cCN ({grounded=false }, tcl.cm_generics) tid3 suffix_no an_item cxxc
                        let cxxc = rip_exec_code2 vd msg cxxc
                        let ww' = WF 3 msg ww "done"
                        vprintln 2 (sprintf "%i ctorcode items at end of compile 9" (length ctorcode))
                        vprintln 3 ("\n\n\n\n\n")
                        cxxc
                    let (sons', env, ctorcode, spawned22, invalidates33, ties33) =  List.fold root_compiler_wrapper ([], env, ctorcode, spawned22, invalidates33, ties33) (zipWithIndex items)
                    if not_nullp sons' then m_donesome := true
                    (sons @ sons', execs, env, ctorcode, sf_key::sofar_compiled, spawned22, invalidates33, ties33) // end of compile9

                                
        let slack_time = 
            {
                latency_target=  -1 
                ii_target=       -1
                hardness=        0.0
            }

        let all_ast = list_flatten(map f3o3 top_asts)
        let ast_map_dir = Map.empty // now unused?

        let items1 =
            if not no_cmdline_roots then // If we have command-line roots then we ignore attribute-marked roots.
                let raw_items = (map (fun x -> Root_s x) cmdline_roots)
                let cook arg cc =
                    match arg with
                        | Root_s [] ->
                            hpr_warn "Null string present on root compilation list!"
                            cc

                        | Root_s idl when idl = [g_globalname] ->
                            //let ast1 = No_esc(CIL_class("", [], Cil_cr_global, [], None, all_ast))o
                            let tid = get_next_tid MM_specific S_root_method (rez_tid_prefix "RSG")
                            let timespec = None
                            let dir_spec = gec_preliminary_director ww timespec (kfec2.directorate_style, kfec2.directorate_attributes)
                            let item = Root_cc(tid, ROOT_THREAD1(idl, None, hptos idl), dir_spec, timespec)
                            item :: cc

                        | Root_s idl ->
                            let work_item = 
                                let ids = hptos idl
                                let k0 = "KiwiC: processing specific root cmdline item " + ids
                                let tid = get_next_tid MM_specific S_root_method (rez_tid_prefix ids)
                                let ww = WF 3 k0 ww "start"
//                                let rr = if idl = [g_globalname] then sf "globalname clause should be dispatched in parent."
//                                    else id_lookupp ww idl ast_map_dir (fun()->(ids + " :Root item not defined\n (NB:freely use dots or slashes to separate parts of a name)", 1))
                                let timespec = Some slack_time
                                let dir_spec = gec_preliminary_director ww timespec (kfec2.directorate_style, kfec2.directorate_attributes)
                                Root_cc(tid, ROOT_THREAD1(idl, None, ids), dir_spec, timespec)
                            work_item :: cc
                List.foldBack cook raw_items []

            else
                let lnorma (class_idl, ((hls_style, method_idl, uid, dir_time), cls_), supressed) cc =
                    if supressed then cc
                    else
                    let tid3 = get_next_tid hls_style S_root_method (rez_tid_prefix (hptos method_idl))
                    m_lastroot := class_idl // A use for the surrounding class name ?
                    let ww = WN "lnorm" ww
                    //dev_println(sprintf "Attribute scanned for director and timespec: Get %A" dir_time)
                    let timespec = snd dir_time
                    let dir_spec = gec_preliminary_director ww timespec (fst dir_time)

                    Root_cc(tid3, ROOT_THREAD1(class_idl, Some method_idl, uid), dir_spec, timespec)::cc 
                List.foldBack lnorma marked_roots []

        let brief_form_toproots =
            let bft cc = function
                | Root_cc(tid3, ROOT_THREAD1(class_idl, _, uid), dirspec_, timespec_)  -> class_idl::cc
                | other -> sf (sprintf "bft other %A" other)
            List.fold bft [] items1
        let compile_auxiliary_static_cctors (cxc, root_companions) idl bool_ = //(idl, (uid, ordering, m_donef)) = // These may need typically be called observing partial order constraints, having come from an import tree.
            let ids = hptos idl
            let uid = ids // Is this not sufficient? Classconstructs cannot be overloaded.
            let ww = WF 3 "auxiliary_static_cctors" ww ("toptop: compile9: start on " + ids)
            vprintln 2 (sprintf "toptop: compile9: Auxiliary static class construction : %s" ids)
            let cctor_idl = ".cctor" :: idl // Auto-compile the constructor
            if memberp idl brief_form_toproots then
                (cxc, (idl, cctor_idl)::root_companions) // Defer a root class constructor for prepending to the lasso stem of that root later on.
            else
                let specificf = S_kickoff_collate
                let style = MM_aux_cctor_style
                let tid = get_next_tid style specificf (rez_tid_prefix ids + "_CTOR")
                let timespec = None
                let dir_spec = gec_preliminary_director ww timespec (kfec2.directorate_style, kfec2.directorate_attributes)
                let item = Root_cc(tid, ROOT_THREAD1(idl, Some cctor_idl, uid), dir_spec, timespec)
                let cxc = compile9 ww cxc item
                (cxc, root_companions)

        let cxc00 = (sons0, [], env00, [], [], [], [], []) // TODO share with gec_cil_blk
                // The last one added was most likely the last one to need calling, so reverse the list here to mimimise iterations before closure.

        let (cxc, root_companions) = cCC.m_classes_possibly_used.fold compile_auxiliary_static_cctors (cxc00, [])  // Order of calling startup code should be preserved actually.
        m_deferred_cctors := root_companions
        //dev_println (sprintf "root companions are " + sfold (fst>>hptos) root_companions)
        let report_ctorcode_leftover ctorcode_leftover =
            let has_code = function
                | SP_dic(dDIC, ids) -> (dDIC.Length > 0)
                | other -> sf (sprintf "has_code other form %A" other)
            if disjunctionate has_code ctorcode_leftover then
                reportx 0 "+++some constructor code was left out: some things may not be initialised" (fun x-> hprSPSummaryToStr x) ctorcode_leftover
                reporty rpt "+++ctor code leftover: a class was not constructed?  Perhaps alter order of dll/exe on command line. " (fun x-> hprSPSummaryToStr x) ctorcode_leftover
                let queries =
                    { g_null_queries with
                        concise_listing=   true
                        yd=                YOVD -1
                    }
                //let dummy_director = gec_preliminary_director ww None (kfec2.directorate_style, kfec2.directorate_attributes)
                anal_block ww (None) queries (H2BLK(g_null_directorate, SP_seq(ctorcode_leftover))) // Use anal block to report it.
                cassertnf(nullp ctorcode_leftover, "construtor code (ctorcode) left over")


        let compile_startup_code cxc (class_idl, uid) =  // Not from C# but from gcc4cil and so on.
            let ids = hptos class_idl
            let ww = WF 3 "startup_code" ww ("toptop: compile9: start on " + ids)
            //let (sup_items, nn) = normed_ast_item_lookup00 ww (class_idl)
            vprintln 2 (sprintf "toptop: compile9: startup %s" ids)
            let tid = get_next_tid MM_aux_cctor_style S_startup_code (rez_tid_prefix ids + "_SX")
            let timespec = None
            let dir_spec = gec_preliminary_director ww timespec (kfec2.directorate_style, kfec2.directorate_attributes)
            let item = Root_cc(tid, ROOT_THREAD1(class_idl, None, uid), dir_spec, None)
            compile9 ww cxc item

        // gcc4cil generates a start routine that calls Startup, then main, then Shutdown.  We ignore that.
        // We do need to call COBJ?init first when present.
        let gcc4cil_startup_names = [ [  "COBJ?init" ; g_module_esc ] ]
        let cxc = List.fold compile_startup_code cxc (map (fun x->(x, hptos x)) gcc4cil_startup_names)

        let (_, _, env0, ctorcode_reaped, _, _, _, ties) = cxc
        let cycle_through_entry_points seeds =

            let rec analyse (pcount, docount, okcount) = function
                | [] -> (pcount, docount, okcount)
                | (item, status)::tt ->
                    let nn = 
                        match status with
                            | RED_pending     -> (pcount+1, docount, okcount)
                            | RED_do_again _  -> (pcount, docount+1, okcount)
                            | RED_ok_result _ -> (pcount, docount, okcount+1)
                    analyse nn tt

            let initial_queue = map (fun seed -> (seed, RED_pending)) seeds


            let rec redo_iteration count_ queue = 
                let (pcount, docount, okcount) = analyse (0, 0, 0) queue
                vprintln 2  (sprintf "NEW-CYCLER Work Queue Status: pcount=%i, docount=%i, okcount=%i." pcount docount okcount)
                let all_done = (docount+pcount)=0
                if all_done then queue
                else
                    let (m_spawned, m_invalidates) = (ref [], ref [])
                    
                    let rec process_work_list passon = function
                        | [] ->
                            let (env, ctorcode_leftover) = passon
                            vprintln 2 (sprintf "%i ctor items left over at end of scan" (length ctorcode_leftover))
                            report_ctorcode_leftover ctorcode_leftover
                            [] // passon is dropped ... could check reaped ctor code here
                        | (item, status)::further ->
                            let (newv, passon) = 
                                match status with
                                    | RED_pending
                                    | RED_do_again _ ->
                                        let ((sons, execs, ties33), passon, okf) =
                                            if vd>=2 then vprintln 2 (sprintf "NEW-CYCLER iteration_no=%i - START ITEM %s" count_  (hptos(item_name item)))
                                            let (env, ctorcode_reaped) = passon
                                            let cc00 = ([], [], env, ctorcode_reaped, [], [], [], [])
                                            let (sons, execs, env, ctorcode_reaped, sf_keys_, spawned22, invalidates33, ties33) = compile9 ww cc00 item
                                            if vd>=2 then vprintln 2 (sprintf "%i ctorcode items passed on to potential next scan stage" (length ctorcode_reaped))
                                            mutapp m_spawned spawned22
                                            mutapp m_invalidates invalidates33
                                            let passon = (env, ctorcode_reaped)
                                            ((sons, execs, ties33), passon, true)
                                        if not okf then sf "go round again? or fatally fail"
                                        else ((item, RED_ok_result(sons, execs, ties33)), passon)

                                    | RED_ok_result(_, _, _) ->  // We could/should save the passon in the ok_result so the subsequent work item sees the same input env regardless.  But the saved same would not see changes arising from earlier recompiles.
                                        let passon =
                                            if item_specific item =S_root_method || item_specific item=S_root_class then
                                                let (env, ctorcode_reaped) = passon
                                                (env, [])
                                            else passon
                                        ((item, status), passon)
                            newv :: process_work_list passon further
                    let queue = process_work_list (env0, ctorcode_reaped) queue
                    if vd>=2 then vprintln 2 (sprintf "NEW-CYCLER: noted %i thread invalidates" (length !m_invalidates))

                    let newthreads =
                        let newthreader cc arg =
                            match arg with
                            | Root_cc(tid3, USER_THREAD1(srcfile, oo, mm, spawntoken, nn, args, env), _, _) ->
                                let rec extant_check = function
                                    | [] -> false
                                    | (Root_cc(_, USER_THREAD1(srcfile, oo, mm, spawntoken1, nn, args, env), _, _), _)::tt when spawntoken1 = spawntoken -> true
                                    | _::tt -> extant_check tt
                                let is_extant = extant_check queue 
                                if is_extant then
                                    vprintln 3 (sprintf "Supress duplicate spawn of %s" spawntoken)
                                    cc 
                                else (arg, RED_pending)::cc
                            | other ->  sf (sprintf "newthread extant other %A" other)
                        List.fold newthreader [] !m_spawned
                    let queue =
                        let apply_invalidate queue (tid, command_) =
                            let aiq arg cc =
                                match arg with
                                    | (item, RED_ok_result(a, b, ties)) when tid = (item_tid item).id ->
                                        vprintln 1 (sprintf "Applying compiled thread invalidate %A cmd=%s to %A" tid command_ item)
                                        (item, RED_pending)::cc

                                    | (item, RED_ok_result(a, b, ties)) ->
                                        dev_println(sprintf "Missed compilation unit invalidate %A cf %A" tid  (item_tid item))
                                        arg :: cc
                                    | arg -> arg :: cc
                            List.foldBack aiq queue []
                        
                        List.fold apply_invalidate queue !m_invalidates @ newthreads
                    redo_iteration (count_ + 1) queue

            redo_iteration 0 initial_queue 

        let completed_design = cycle_through_entry_points items1


        let (son_vms, execs, ties) = // Reap the compilation results.
            let (son_vms, execs, _, _, _, _, _, ties) = cxc
            //dev_println (sprintf "Prior to reap: son vm count is %i. Exec count %i" (length son_vms) (length execs))
            let reaper (item, status) (cc, cd, ce) =
                match status with
                    | RED_ok_result(sons, execs, ties) -> (sons@cc, execs@cd, ties@ce) // Some sons are on final cxc and others reaped here - please say why.
                    | _ -> sf "BAD ALT - entry point compile not complete"
            let (son_vms, execs, ties) = List.foldBack reaper completed_design (son_vms, execs, ties)
            (son_vms, execs, ties)
            
        vprintln 2 (sprintf "Post reap: son vm count is %i. Exec count %i" (length son_vms) (length execs))
        vprintln 1 ("Kiwife: CIL/.net ---conversion by kiwife to HPR internal form complete---\n")

        let lower_decls =
            let rec collect_lowers cc = function
                | (ii, None) -> cc
                | (ii, Some(HPR_VM2(minfo, decls, sons, execs_, _))) ->
                   let cc = List.fold collect_lowers cc sons
                   lst_union decls cc
            List.foldBack (db_flatten []) (List.fold collect_lowers [] (list_flatten son_vms))  []

        if not !m_donesome then hpr_warn ("+++ No executable code encountered (perhaps change root command line flag or Kiwi.HardwareEntryPoint attributes) ?")

        // Lostio (bitwise) bit-wise and very wide I/O - disabled for now.
        let lost_ios =
            let m_lirr = ref []
            //let lost_io_dig (k, v) = if is_a_lostio(v.atts,  then mutadd lirr v else () )
            //for z in Heap.sheap do lost_io_dig (z.Key, z.Value) done
            !m_lirr

        // A second output from the compilation is all the attributes in minfo.
        // The memdesc is used by repack.
        // The user's name is a default name for vnl file name and root.
        let m_minfo_ats = ref (map (fun (k,v) -> Nap(k,v)) cCC.settings.directorate_attributes)

        let is_ionet = function
            | X_bnet ff -> isio (lookup_net2 ff.n).xnet_io 
            | _ -> false

        //Create a list of the static variables we have used by digging them out of static heap.
        let here_nets =  // List of nets collating.
            // capture too cCC.m_additional_net_decls=     ref []
            let cpi_io = { g_null_db_metainfo with kind= funique "kiwic-misc-io";  not_to_be_trimmed=true; form=DB_form_external }
            let cpi_local = { g_null_db_metainfo with kind= funique "kiwic-main-nets" }
            let cpi_pramor = { g_null_db_metainfo with kind= funique "kiwic-main-overrides"; form=DB_form_pramor }  
            let collated_by_io_group = new ListStore<decl_binder_metainfo_t, sheap_entry_t>("io_groups")

            let tobecome_rtl_param = function
                | Some(X_bnet ff) -> at_assoc "rtl_parameter" (lookup_net2 ff.n).ats 
                | _ -> None

            let _ =
                let gdsh kk cez =
                    if not_nonep cez.tlm_propertyf then ()
                    else
                        if nonep !cez.hexp then
                            let net = lnetgen ww cez.io_group cez.cez None
                            vprintln 3 (sprintf "late netlist generation rez %s for io_group %A.  net=%s" kk cez.io_group (netToStr net))
                            cez.hexp := Some net
                        let k1 =
                            if not_nonep cez.io_group then valOf cez.io_group
                            elif not_nonep(tobecome_rtl_param !cez.hexp) then cpi_pramor
                            elif is_ionet (valOf !cez.hexp) then cpi_io
                            else cpi_local
                        collated_by_io_group.add k1 cez
                for z in heap.sheap1 do gdsh z.Key z.Value done

            let heapdig (she:sheap_entry_t) cc =
                let napgen (a,b) = Nap(a,b)
                match she.cez with
                    | _ when she.ethereal ->
                        vprintln 3 (sprintf "No render for ethereal net %s" (hptos she.idl))
                        cc
                    | CE_var(vrn, CTL_void, idl, fdto, ats) -> cc // Discard 'void' variables.
                    | _   when !she.hexp <> None -> valOf(!she.hexp) :: cc
     
            let heapdefs_now =
                let m_hdr = ref []
                let gdsh (cpi:decl_binder_metainfo_t, items) =
                    vprintln 3 (sprintf "\n\nNetlist generation : pi_name='%s'  %i nets" cpi.pi_name (length items))
                    let group = List.foldBack heapdig items []          // Statics only ... the dynamics are trawled by the repack operation.
                    if not_nullp group then
                        //reportx 3 "PC regs needed " xToStr heapdefs_now
                        mutadd m_hdr (gec_DB_group(cpi, map db_netwrap_null group))
                for z in collated_by_io_group do gdsh (z.Key, z.Value) done
                !m_hdr
            let decls = list_flatten heapdefs_now
            decls 

        let memdescs =
            let redirects = []
            let mr = ref Map.empty
            let col_memdesc aid (v:proto_memdesc_record_t) =
                let cv = get_memtok "heap memdescs" v.p_aid
                //let targets_ = target_classes.Points_at(cv)
                let _ = mr := (!mr).Add(cv, v)
                ()
            for z in heap.memdescs do app (col_memdesc z.Key) z.Value done
                                                  
            let m_final_memdesc_list = ref(render_numeric_ties())
            //vprintln 0 (sprintf "newl: Yielding %i memdescs" (length newl))
            //vprintln 0 (sprintf "initially: Yielding %i memdescs" (length !m_final_memdesc_list))   
            let dig_memdesc aid (pmr:proto_memdesc_record_t) =
                let nc2 = get_current_sc "dig_memdesc" pmr.p_aid
                let yild (aid, md) =
                    //vprintln 0 (sprintf "Memdesc dig %s %s nemtok=%s is in class %s %s " "(vuidToStr pmr.vuid)" " " pmr.p_nemtok (mdToStr nc2) (mdToStr md))
                    mutaddonce m_final_memdesc_list ([aid], md)

                let make_md nc0 = // make memory descriptions
                    let (ncv, ncc) =
                        match nc0 with
                            | Memdesc_sc nc when nc <> 0 -> muddy "TODO nc" // not zero
                            | Memdesc_scs ss when ss.[0] = 'c'-> // This hardcoded char needs to match letter given to ksc when rezzed - can read it out please.
                                //dev_println  (sprintf "scanf start from %s" ss)
                                let si = sscanf "c%i" ss
                                //vprintln 0 (sprintf "scanf %i from %s" si ss)
                                (si, 'c')
                            | md -> sf (sprintf "other form md for report: %s  %A " (mdToStr md) md)
                    let (uid,labelled_literal) =
                        if true then (pmr.p_uid, pmr.p_labelled_literal)
                        else
                            let sheo = heap.sheap1.lookup pmr.p_nemtok
                                //muddy (sprintf "de_Vrn %s nemtok=%A" p_nemtok sheo)
                            match sheo with
                                | None -> sf (sprintf "make_md: tacit ignore nemtok=%s" pmr.p_nemtok)
                                | Some she ->
                                    let uid =
                                        match she.cez with // take dt and ats from an example one - somewhat crudely for now
                                        | CE_var(vrn, dt, idl_, fdto, ats) ->
                                            let dtidl = ct2idl ww dt
                                            { f_name=[sprintf "V%i" vrn]; f_dtidl=dtidl; baser=int64 g_unaddressable; length=None }
                                        | other -> sf (sprintf "make_md: other cez L5136 %s" (ceToStr other))
                                    (uid, she.labelled_literal)
                    let md0 =
                        { g_blank_memdesc0 with // Do not confuse blank and null for this structure. The blank has the has_null flag clear.
                            literalstring=  labelled_literal
                            f_sc_char=      ncc
                            f_sc=           ncv
                            uid=            uid
                            mats=           pmr.mats
                            vtno=           0 // for now
                        }
                    yild (aid, Memdesc0 md0)
                make_md (Memdesc_scs nc2)

            for z in heap.memdescs do app (dig_memdesc z.Key) z.Value done // First step: Add memdesc0 entries to final_memdesc_list from the heap while making nc2_list for the second step.

            if !g_ataken_vd < 10 then report_all_sc_mappings "cilnorm" !g_ataken_vd

            // For all A_loaf nemtoks we report them in terms of their storage class.  For higher members we rewrite them in terms of leaf storage classes.
            let _ =
                let md2_fun1 (sci, members) =
                    let self_key = ksc.sciToStr sci
                    let has_null = ksc.lookup_null_flag sci
                    //vprintln 0 (sprintf "+++raw md2_fun L6657 key=%s has_null=%A vals=%s" self_key has_null (sfold aidToStr members))
                    let sri_ties =
                        match ksc.attribute_get "sri" sci with
                            | None -> []
                            | Some sri ->
                                vprintln 2 (sprintf "Noting shared resource (sri) attribute for sc %s %A" self_key sri)
                                let sri = sri_hydrate(sri)
                                [ Memdesc0 { g_blank_memdesc0 with shared_resource_info=Some sri} ]

                    let offchip_ties =
                        // Find those that are designated as offchip and remove any nominal address since all will be decided at runtime.
                        match ksc.attribute_get "bank" sci with
                            | None -> []
                            | Some specified_bank ->
                                //let sc = nemtok_to_sc "md2_fun2" (A_loaf key)
                                let bank = if specified_bank=g_undecided_bankinfo then g_bondout_heapspace_name else specified_bank
                                vprintln 2 (sprintf "Noting manual bondout space name attribute for sc %s isbank=%A" self_key bank)
                                [ Memdesc0 { g_blank_memdesc0 with shared_resource_info=Some([bank], [])} ]

                    let sri_and_offchip_ties = sri_ties @ offchip_ties
                    let tied__ =
                        let isHigher = function
                            | A_loaf _ -> false
                            | _        -> true
                        List.filter isHigher members
                    let tied = members
                    let exports = List.fold (sc_export ww ksc "md2_fun1" false) [] tied
                    vprintln 2 (sprintf "memdesc export: %i offchip_ties and %i/%i ties for %s" (length sri_and_offchip_ties) (length exports) (length tied) self_key)
                    let tiers = sri_and_offchip_ties @  exports
                    let spare_key_ = self_key // This spare_key_ is just a meaningless decoration at the moment.
                    // Add selfkey in here
                    let tiers = singly_add (Memdesc_scs self_key) tiers
                    //dev_println (sprintf "kill ties %A" tiers)
                    if length tiers > 1 then mutaddonce m_final_memdesc_list ([spare_key_], Memdesc_tie tiers)
                    ()

                //let ties = cCC.kcode_globals.ksc.readOut vd
                let ties = list_flatten ties
                let ties = map (fun (pt, items) -> (pt, map snd items)) ties
                //dev_println (sprintf "%i memdesc ties to report" (length ties))
                app md2_fun1 ties
                
                //for z in cCC.kcode_globals.ksc.classes() do md2_fun1 z.Key z.Value done //  Second step: Add memdesc1 (points at) entries to final_memdesc_list.
                //dev_println "stage 2"
                // Now report those memdescs whose pointer ranges over null.  Ideally report this in the above.
                let md2_fun2_addNull sci flag_ =
                    let self_key = ksc.sciToStr sci
                    //let sc = nemtok_to_sc "md2_fun2" (A_loaf key)
                    let sc = Memdesc_scs self_key
                    vprintln 3 (sprintf "raw md2_fun2_addNull L5435 key=%s flag_=%A" self_key flag_)
                    let spare_key_ = self_key // This spare_key_ is just a meaningless decoration at the moment.
                    mutaddonce m_final_memdesc_list ([spare_key_], Memdesc_tie [ sc; Memdesc0 g_hasnull_memdesc0 ])
                for z in cCC.kcode_globals.ksc.nullSet() do md2_fun2_addNull z.Key z.Value done

            vprintln 3 (sprintf "finally: Yielding %i memdescs" (length !m_final_memdesc_list))
            
            !m_final_memdesc_list 

        let memdescs = simple_memdesc_tidy ww "cilnorm" memdescs
        // ----------------------------------------------------------------------------


        if not_nullp marked_roots then
            let prefname = hd(f1o3(hd (rev marked_roots))) // The last marked root is our preferred name for the compilation result, but this may well be overridden later in the recipe.
            vprintln 2 (sprintf "Setting preferred output name to '%s'" prefname)
            mutadd m_minfo_ats (Nap(g_preferred_name, prefname))
            
        //reportx 3 "KiwiC top-level machine netlist" netToStr nets
        let decls = decls_trim (*delete=*)true (map snd lower_decls) here_nets // reap decls from lower machines. A horrendous means to an end.

        // Done now already
        //let decls = refactor_old_rtl_pram_annotations ww decls // Should have collated them separately above just now instead of calling this!

        let vlnv = { vendor="Kiwi"; library="custom"; kind=topname; version="1.0"} // TODO these fields should all be settable via recipe.
        vprintln 2 (sprintf "Created VM2 called " + vlnvToStr vlnv)
        let minfo = { g_null_minfo with name=vlnv; atts= !m_minfo_ats; memdescs=memdescs;  hls_signature=None }
//        let _ = reportx 3 (sfold (fun c->c) minfo.name + " fin lowers ") xToStr lowers
//        let _ = reportx 3 (sfold (fun c->c) minfo.name + " fin nets") xToStr nets
//        let _ = reportx 3 (sfold (fun c->c) minfo.name + " fin gdecls ") xToStr gdecls
//        let _ = reportx 3 (sfold (fun c->c) minfo.name + " fin ldecls") xToStr ldecls


        let execs_as_hblocks =
            if nullp execs then []
            else sf ("need to magic up a director for these top-level execs")
        let ans = HPR_VM2(minfo, decls, list_flatten son_vms, execs_as_hblocks, []) // Final wrapper VM (has all the nets in it).
        opath.dump_all_recipe_settings_and_overrides rpt
        youtln rpt "END OF KIWIC REPORT FILE"
        yout_close rpt
        ((*pack_bit_vectors *)ans, !m_lastroot)
    readfun





(*-------------------------------------------------------------------
Simple referencing/dereferencing using a simple star counter - is not perfect!

The compile-time representation of a type or expression has a number of stars
associated with it which is the number of levels of indirection needed to get
to a valuetype. Stars can be +ve or -ve and the absolute value of either form represents the number of
dereferences needed.  

There are some complex subtleties: The C++ language uses the asterisk and ampersand operators to dentote stars but their meaning
in type declarations and expressions is not the same. The dot net system uses the 'valuetype' flag to override a default approach where 
scalars are passed by value and objects and arrays are passed by reference.  Moreover, the star approach might get some esotric
situations, such as a C++ pointer to reference, wrong, owing to representing everything as a simple integer.  But it should be
fine for all C# situations.


As operators, asterisk and ampersand denote dereference and reference respectively: they are opposites, with a dereference being applied
to a reference cancelling out to give the identity function, but a reference to a dereference is not the indentity.  As types, asterisk
and ampersand, confusingly, have the same 'polarity': they are both wrappers that generate a handle on an item and the handle is removed
with using a dereferencing operation.

Consider a scalar s and an array A.

  int s, A[32];

Stars in the type of an expression are allocated by a f operator according to the following semantics:

    f[| s |]  = (0, s, 0)
    f[|* e|]  = let (i, v, _) = f e in (i-(sgn i), v, 0)
    f[|& e|]  = let (i, v, _) = f e in (i+1, v, 0)

The middle field can either be used to designate the type of s (e.g. int) or s itself according to usage.

In executable code the dereferencing/asterisk operator REMOVES a star from the
star count in the type of its operand and the & operator adds one.  Here 'removes' means makes closer to zero by subtracting the sgn().
For types, CT_star(1, int) is printed as &int.

For a simple assignment between scalars
     s1 := s2 
both the lhs and rhs have zero stars.

If we take the address of a scalar, we have a reference to it that is removed when we read or write it using the asterisk operator in C:
    *&s = e;
    e = *&s;


Initialised pointer variables, such as [| int *p = &s |], which is the same as  [| int *p; p = &s |],
will set up p to have the type (1, s, 0) or (1, int, 0).  Thus indicating a single level of indirection (dereference)
is needed to access the real data. Note that the declartion [| int *p |] holds something with one positive star even though
the expression *p, when evaluated, would decrement the star count. This is the origin of a positive 'star' denoting
a reference type, which means that the ampersand operator in expressions generates, ironically, a positive star too whereas
applying the star operator to an expression (derferencing it with ldind for instance) removes one star between the domain and range
types of the ldind.

A pointer to a pointer to an integer will have stars=+2. A C++ reference type will have stars=-1, denoting automatic dereference is needed upon use.
We never have the address of an address, so stars is never less than -1, ignoring the following pathological, valid C++
 { int dd = 42;  int *dp = &dd;  const int &dr = dd; const int * const &drr = &dr; }.  Also, a reference to a dereference should not cancel to the identity!




Static blobs of data, such as initialisation values for arrays and string literals generated by gcc4cil are labelled.  We use &(Var(...)) for labels. 
For C++ declarations, both pointers and references are handles  that can be dereferenced to get at the datum, but the reference requires no explicit operator and could be implemented a different way from a pointer.
   int dd;
   int *dp = &dd;
   int &dr = &dd;


Consider now arrays:
  The dereferencing done by the [] operator also removes one star as it yields its resultant type.


   f[|  int *p = &(A[0]) |] defines a pointer p with value (1, A, 0)
   f[|  int *p = A |]       also defines the same thing (1, A, 0)
   f[| A |]                 as does this (1, A, 0)

The array subscription operator a[b] is sometimes defined
commutatively as *(a+b) but this is an obfuscating sugaring and we
require one of the operands to be an integer expression and the other
a pointer.  Lets call b the integer.  Therefore 
   f[|  A[b] |] = (0, A, b)

When generating hexp_t we need to introduce subcript operators on all
access to arrays but not scalars, so (0,A,0) generates subsc(A, 0)
whereas (0,s) converts to just s.  This relates to A being a partial
type, instead of ever being considered a reference type, and ignoring
full array (valuetype) assignments, it can be thought of as real data that needs a
special operator for access operations (ie array subscription).


The CIL ldloca, stind and ldind instructions are part of a family of instructions for making
indirect references to storage locations.

The CIL newarr, ldelem and stelem instructions are part of another family relating to the CIL primitive 1D
arrays. But things are more complicated than outlined above, since an
array is always accessed using a handle variable.
 
    int [] V_10 = new array [ 32 ]
    int k =  V_10[3]

        newarr 32, stloc .10, ldloc .10, ldc 3, ldelem, stfld k

Equating the [] symbols in the monodis to negative stars 

Therefore a correct implementation is
  1.  (not) to have stars on array body definitions

We do not load the symbolic value of an array on to the stack when we
access it's handle since we might be going to write to it: we just
lookup the width to find the underlying tmptem.

... this seems unfinished

What currently have is:


DefinitiveS:
  1. Newobj creates a class reference type (no added stars).
  2. A class ref is equivalent in type to a star applied to a record (&CTL_record(...) form).
  3. All forms of Ciltype_array in the source code are autoref converted to &CT_arr(...) in typenorm.
---------------

If we consider something like "valOf(!(snd v)+1" we see v must have type  int option ref pair where the int is at the leaf of the AST for the type expression, so to find the type of the lhs operand of the addition we must delve deeply inside its ast to find v and then inside the AST for the type of v to the same depth to find the int.  So it is much better to store associated type within the value expression AST and we do this for the CE_subsc and CE_dot operators.  But for the remaining operators, such as "+" in address arithmetic, we use the CE_tv form to tag the expression.

Are there two coding styles for storing symbolic operations on typed expressions?

In the composite approach, we compute the type of an expression when we need it,
delving down the value AST to get the core type then computing the expression type as we unwind.
But the separate approach stores types in the AST nodes when convenient to short cut the devlve and t/v pairs elsewhere.
 
   Suppose ca is the separate pair  (int array array, {{1,2}{3,3},{5,6}})
then ca[2] is (int array, {5,6}), which is achieved by removing the outermost
operator on both type and value expressions.  This method is fairly natural
but does essentially require that the stack is a stack of pairs.

--------------


The user will access a variable with some structural form, such as
  A[x].B[y].v1
but are other ways to get to the same variable, such as Q.v1
so we need a canonical form for saving and lookup in the environment.

The generated x code uses a canonical cuid form that uses 1-D arrays,
named by a string list with an hexp_t expression as subscript, so this
example is converted to

Absolute rubbish
       [ v1, B, A ][base+x*String.lengthof+y]

        // The idl for the wondarray is the type of the object, but for env is a canonical form of the access path: we can get errors from unknown  aliases sometimes!..   Should the idl used in the lhs of evolve have tag names in it ?  Well we can always convert them into hidx values, so thats ok ... 


Correct:
  Example type B_t = array (range y) of alpha
          type A = array (range x) of B_t

We note that the normal association of something like A[x][y] is (A[x])[y] where
the outer subsc has a non-array as its lhs arg (it has a subsc of another array as its lhs arg).
This is not allowed in hexp_t, where the lhs of all subsc operators must be an array.

Ultimately we index something of type alpha and we go further and assume all 
indexable items of type alpha are held in a common heap array called HA/alpha.  The array A
has a start address in heap array called HA/HA/alpha: ie an array of arrays of alpha.

  So (A[x])[y] becomes HA/alpha[HA/HA/alpha[hidx(A) + x * String.lengthof(HA/alpha)] + y * sizeof(alpha)]

In this syntax, an array of name HA/x has content type x and when indexed by an expression $e$
as in $A[e]$ then $e$ is treated as a byte address and must take on certain discrete values
only.  These values are some base address, returned originally by newarr, plus an integer multiple of
String.lengthof(x).

If we wanted to enable free pointer casting between all reference types, we would
instead use a heap array called HA/HA to hold all pointer variables. In the full system,
an initial scan of the program determines all types that are cast between, thereby generating
equivalence classes of types, and creates a heap array for each equivalence type.  Also, in the
full system, any variable that is clearly never referred to by reference is ...

So the procedure for compiling array and field references is

  let lower_indexed(subsc(l, r)) = 
         let (type, idx) = lower_array l
             let content_type = tail type
             (content_type, type[idx + lower r * String.lengthof(content_type)])
            
  |   lower_indexed(dot(l, (name, type, offset))) = 
         let 
             in (type, subsc(HA/type, lower l + offset))
            
  | lower_indexed(htoken(content_type, hptr)) = (content_type, hptr) 

  and lower (E as subsc(l, r)) = snd(lower_indexed E)
  |   lower (E as dot(l, (name, type, offset))) = snd(lower_indexed E)
  |   lower other = other

Where the function the infix operators [ ] plus and multiply generate nodes of 
the resultant abstract syntax tree.


The basis of the lower_indexod function is that it returns a pair composed
of the type of a subexpression and its heap index  NO - a full expression sometimes.  The pair is lowered
to an hexp by forming the subcript X_subsc(wondarray fst P, snd P).


| Worked example   Q[vs]
| inner call     lower_indexed(Q) gives (HA/alpha, hidx(Q))
| output call returns HA/alpha[hidx(A) + vs * String.lengthof(alpha)]

| Worked example   (A[x])[y]
| inner call     lower_indexed(A) gives (HA/HA/alpha, hidx(A))
| middle call of lower_indexed(A[x]) gives (HA/alpha, HA/HA/alpha[hidx(A) + x * String.lengthof(HA/alpha)])
| output call returns HA/alpha[HA/HA/alpha[hidx(A) + x * String.lengthof(HA/alpha)] + y * sizeof(alpha)]

With this basis, we now add record and object field access as a variant on array subscription.
The differences from an array are that each field has a constant offset known at
compile time but the fields do not all need to share a common type.

The user will access a variable with some structural form, such as

| Worked example     A[x].B[y].v1
| leaf call   on (A)    gives (HA/reca ... , hidx(A))
| inner call  on (A[x]) gives (reca ..., HA/reca ...[hidx(A) + x * String.lengthof(reca ...)])
| middle call on (A[x].B) gives (HA/recb ..., subsc(HA/HA/recb, HA/reca ...[hidx(A) + x * String.lengthof(reca ...)] + offset(B)))
| upper call on  (A[x].B[y]) gives 
| outermost on   (A[x].B[y].v1) gives 


but are other ways to get to the same variable, such as Q.v1

* We dont support un-aligned stores... 

 Wondtoken clauses: One of these two is wrong: one gives cdt and other  gives self dt !  TODO could just be a matter of how we NAME our wondarrays: for arrays its by content type wehereas for a record, there are no anon records and so we use the record definition name, ttt.  

There's an issue with array lengths:  when we allocate an array it has a length
property that can be read with the ldlen instruction.   When an array
is stored in a wondpointer it ...


Under gtrace we expect the number of object allocated and the lengths of arrays to, in general, vary
during elaboration time (elab44).

Each call to new or newarr is given a unique id, but if this call is inside a loop that is unwound during elaborate this is not sufficient...

Also, .cctor calls and so on are currently elaborated totally before proceeding to the next... and the heap pointer seems to be reset in test15 so objects are on top of each other...

*)

(* eof *)
    
