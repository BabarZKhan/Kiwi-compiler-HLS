(*
 * (C) 2003-8 DJ Greaves. Cambridge. CBG.
 * HPR L/S Project. 2009/10.
 * $Id: systolic.fs,v 1.2 2013-07-08 08:32:14 djg11 Exp $  
 *
 * systolic.fs
 *
 *
 *
 *
 *
 *)
module systolic

open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr

let g_cr_vd = 5  (* Set this verbosity to 20 unless debugging *)

type systolic_control_t =
    {
        vd:           int
        control: control_t;

    }



(* ---------------------------------------------------------*)

let systolic_top ww K2 vm = vm

// Systolic recurrence equations for Bubble Sort.
let g_kk     = xgen_bnet(netgen_serf_ff("kk", [], 0I, 32, Signed))   
let g_kk_old = ix_minus g_kk (xi_num 1)
let g_bubble_sort_canned_recurrence_relations =
    let aa = xgen_bnet(netgen_serf_ff("aa", [32L], 0I, 32, Signed))
    let xx = xgen_bnet(netgen_serf_ff("xx", [32L], 0I, 32, Signed))    
    let qq = xgen_bnet(netgen_serf_ff("qq", [], 0I, 32, Signed))
    let kk = g_kk
    let initmode = xgen_bnet (iogen_serf_ff("initmode", [], 0I, 32, INPUT, Signed, None, false, [], []))
    let fmin (e, f) = xi_apply(g_hpr_min_fgis, [e; f])
    let fmax (e, f) = xi_apply(g_hpr_max_fgis, [e; f])    
    let ix_asubsc2 array aa bb = ix_asubsc (ix_asubsc array aa) bb
    let a1 = ix_asubsc2 xx qq g_kk_old
    let a2 = ix_asubsc2 aa (ix_minus qq (xi_num 1)) kk

    let ptap = ix_asubsc xx (ix_minus qq (xi_num 1))
    let recurrence_eqns = 
      [
        Xassign(ix_asubsc2 xx qq kk, ix_query (xi_orred initmode) ptap (fmin(a1, a2)))
        Xassign(ix_asubsc2 aa qq kk, fmax(a1, a2))        
      ]
    let decls = [ aa; xx; qq; kk; initmode ]
    let projection = qq 
    ("bubbler1", xx, decls, recurrence_eqns, projection)


let rez_hprsystol_ii i_name kind  =
    let external_instance = false
    let _ = vprintln 3 (sprintf "rez_hprsystol_ii %s %s external_instance=%A" i_name (hptos kind) external_instance)
    { g_null_vm2_iinfo with
          generated_by=         "hprsystol"
          vlnv= { vendor= "Hprsystol"; library= "custom"; kind=kind; version= "1.0" }
          iname=                i_name
          externally_provided=  false // false, since these should not be separate modules
          external_instance=    external_instance
          preserve_instance=    true  //deprecated flag or ignored even? 
          nested_extensionf=    true
    }



let systol_gen ww  recurrence_eqns one_off_eqns projection range =

    let gsys0 (_, mm) arg = 

        let gsys_ee = xi_rewrite_exp mm
        match arg with
        | Xassign(ll, rr) ->
            gen_XRTL_arc(None, X_true, gsys_ee ll, gsys_ee rr)
        | other -> sf (sprintf "other form in gsys0")


    let fq qit =
        let sigma = [ (projection, xi_num qit) ]
        let gen_ARGM(ll, rr) = ARGM(ll, X_true, rr, false)
        let mm = makemap1 (map gen_ARGM sigma)
        let pred arg =
            match arg with 
            | X_bnet ff when arg=projection -> true  // Should not need this rule since in mapping
            | W_asubsc(baser, idx, _) when idx = g_kk || idx = g_kk_old -> true
            | W_asubsc(baser, idx, _) when constantp idx && lc_atoi (* "systolic-array-idx-negative" *) idx < 0I -> true 
            | _ -> false

        let pred a =
            let ans = pred a
            vprintln 0 (sprintf "Predd at %i for %s is %A" qit (xToStr a) ans)
            ans
        let mapper strict_ arg =
            match arg with
            | X_bnet ff when arg=projection -> (xi_num qit)  // Should not need this rule ...
            | W_asubsc(baser, idx, _) when constantp idx && lc_atoi idx < 0I -> xi_num 0
            | W_asubsc(baser, idx, _) when idx = g_kk || idx = g_kk_old -> baser
            | other -> other

        let mapper s a =
            let ans = mapper s a
            vprintln 0 (sprintf "Mapper at %i for %s is %A" qit (xToStr a) (xToStr ans))
            ans

        let mm = hexp_agent_add mm (pred, mapper, (true, true))
        let gP = ("sigma", mm)
        map (gsys0 gP) recurrence_eqns

    let oners =
        let mm = makemap1 []
        map (gsys0 ("[]", mm)) one_off_eqns 
    list_flatten(map fq range) @ oners



let gec_systolic_vm2 ww0 =
    let ww = WF 1 "SystolicElab:" ww0 "Start"  
    let (iname, xx, decls, recurrence_eqns, projection) = (g_bubble_sort_canned_recurrence_relations)

    let dout = xgen_bnet (iogen_serf_ff("dout", [], 0I, 32, OUTPUT, Signed, None, false, [], []))
    
    let decls =
        let cpi = { g_null_db_metainfo with kind= funique "systol_gen-topbinds"; pi_name=iname }

        let wrap net = DB_leaf(None, Some net)
        [ DB_group(cpi, map wrap (dout::decls)) ]

    let range = [0..31]

    let one_off_eqns = 
      [
        Xassign(dout, ix_asubsc xx (xi_num(hd(rev range))))
      ]

    let rtl = systol_gen ww recurrence_eqns one_off_eqns projection range

    let director00 =  // A default directorate settings.
            { g_null_directorate with
                  //style=           directorate_style
                  clocks=          [E_pos g_clknet]
                  resets=          [(true, false, g_resetnet)]
                  duid=            next_duid()
            }

    let ii = { id="from_systolic" } : rtl_ctrl_t
    let execs = [H2BLK(director00, SP_rtl(ii, rtl))]

    let assertions = []
    let ii = { rez_hprsystol_ii iname [ iname; "custom-hls-subsystem" ] with
                 definitionf= true
             }

    let minfo = { g_null_minfo with name=ii.vlnv; }
    let ans = (ii, Some(HPR_VM2(minfo, decls, [], execs, assertions)))
    let ww = WF 1 "SystolicElab:" ww0 "Complete"  
    ans 



let systolic_args =
        [
//          Arg_string_list("cone-refine-keep", "List of net names for cone refine to retain. Terminate with --");

          Arg_enum_defaulting("systolic", ["enable"; "disable"], "enable", "Enable control for this operation");




        ]


//
//
//
let opath_systolic ww op_args vm =
    let c3 = op_args.c3
    
    let enable = (control_get_s op_args.stagename c3 op_args.stagename None) = "enable"
    //let manual_keeps = control_get_strings c3 "cone-refine-keep" 
    //let asa = (control_get_s op_args.stagename c3 "array-scalarise" None) = "true"
    let K2:systolic_control_t
           = {
               vd=           g_cr_vd
               control=      c3
             }
             
    if enable then () else vprint 1 "Systolic is Off\n"
    let ans = if (enable) then systolic_top ww K2 vm else vm
    ans 




let install_systolic() =
    let stagename = "systolic"
    let _ = install_operator (stagename,  "Systolic Layout", opath_systolic, [], [], systolic_args)
    0


(* eof *)
