           let ctr_0 = [ RV_leaf(start) ]
            let ctr_1 =
                let gw (gg, d) = RV_goto(RV_leaf -1, gg, d) // This does tend to copy out the basiswork twice.
                map gw succs0


            let ctr_0 = [ RV_leaf(start) ]
            let ctr_1 =
                let gw (gg, d) = RV_goto(RV_leaf -1, gg, d) // This does tend to copy out the basiswork twice.
                map gw succs0

            let tbasis = start+10000
            let a0 = { from_=None; basis=start; succs=[tbasis]; predecs=real_predecs; work=work; ctr=ctr_0 }
            let a1 = { from_=None; basis=tbasis; succs=succs;   predecs=[tbasis];     work=[];   ctr=ctr_1 }                

        let xformer = function
            | V_NETDECL(nN, delo, f, width, signed, amax, netdir) when netdir=VNT_WIRE || netdir=VNT_REG ->
                let regdeclf = Set.contains f.id regslist // regslist contains non-sanitised net names
                let netdir' = if regdeclf then VNT_REG else VNT_WIRE
                let ans =
                    V_NETDECL(nN, delo, f, width, signed, amax, netdir')
                //let _ = vprintln 0 (sprintf "xformer regdeclf=%b so to %A ::  %A" regdeclf netdir' f.id)
                in ans
            | V_NETDECL(nN, delo, f, width, signed, amax, netdir) -> V_NETDECL(nN, delo, f, width, signed, amax, netdir)
            | other ->
                let _ = hpr_warn(sprintf "xformer other %A" other)
                in other
        //let termnets' = map xformer termnets

//
// Why would we call this ever, in preference to the sorting one ? -- this is for bdd nodes not queries!
//  NO- there is one in assoc exp!
let rec xi_bmux(vv, ff, tt)  = 
    // Assert ff and tt are always either true/false or another bmux.  Could have used a new fsharp type for that ?
    if bconstantp vv then
        match vv with
            | X_true -> tt
            | X_false -> ff
            | other -> sf ("xi_bmux other constant" + xbToStr vv)
    elif x_beq(ff, tt) then ff
    else
        let h = hexp_bhash vv
        let rec scan ov =
            match ov with
                | [] -> None
                | (W_bmux(v, f, t, _)::ts) -> if (vv=v && tt=t && ff=f) then Some(hd ov) else scan ts
                | (_::t) -> scan t

        let skill_l x =
            match x with
                | (W_bmux(q, ff, tt, _)) -> if q=vv then ff else x
                | x -> x
        let skill_r x =
            match x with
                | (W_bmux(q, ff, tt, _)) -> if q=vv then tt else x
                |  x -> x
        let ff = skill_l ff
        let tt = skill_r tt

        let ovl = mdig<hbexp_t> (wb_SPLAY) h
        let ov = scan ovl
        in if ov<>None then (mutinc nodes_reused 1; valOf ov)
           else
            let nn = nxt_it()
            let ans = W_bmux(vv, ff, tt, { hash=h; n=nn; sortorder=0 }) (* Sortorder is given by vv *)
            //let _ = vprintln 0 ("write bmux " + i2s h + " ovl =" + sfold xbToStr ovl)
            let _ = if ovl<>[] then ignore(wb_SPLAY.Remove h)
            let _ = wb_SPLAY.Add(h, Djg.WeakReference(ans::ovl))
            in ans
;


(*
 * Since true and false have sortorder zero, we arrange our bmux trees in descending
 * sort order.
 *)
and bmux_and(argL, argR) =
    // Local copy:
    let new_known c efo x = // could simplify if x is now true or false.
        if efo=Enum_not then (c, c)
        else ((enum_negate efo)::c, (efo)::c)

    let rec band known (argL0, argR0) =
        let argL = simplifyb_ass known argL0
        let argR = simplifyb_ass known argR0   

        if islinp argL && islinp argR && (linp_var argL) = (linp_var argR) then
           let (v, nv) =
               match (argL, argR) with
               | (W_linp(lv, linl, _), W_linp(rv, linr, _)) when lv=rv -> (lv, linrange_conj(linl,linr))
           in xi_linp(v, nv)
        else
        match (argL, argR) with
            | (_, X_false)  -> X_false
            | (X_false, _)  -> X_false
            | (X_true,  arg)
            | (arg, X_true) -> arg

            | (W_bmux(gl, ll, lr, _), W_bmux(gr, rl, rr, _)) ->
                let gln = xb2order gl
                let grn = xb2order gr                
                let ans =
                    if gln > grn then
                        let efo_l = determine_efo gl
                        let (kff, ktt) = new_known known efo_l gl
                        in xi_bmux(gl, band kff (ll, argR), band ktt (lr, argR))
                    elif gln < grn || (gln = grn && gl<>gr) then
                        let efo_r = determine_efo gr
                        let (kff, ktt) = new_known known efo_r gr
                        in xi_bmux(gr, band kff (argL, rl), band ktt (argL, rr))
                    else
                        let efo_r = determine_efo gr // gl=gr
                        let (kff, ktt) = new_known known efo_r gr
                        in xi_bmux(gr, band kff (ll, rl), band ktt (lr, rr))
                        // We should never get negated guards!
                        //let (abs_l, flip_l) = w_babs gl
                        //let (abs_r, flip_r) = w_babs gr  
                        //let _ = assertf(gl=gr, fun()->i2s(gln) + " " + i2s(grn) + ": bmux parity 'and'" + xbToStr gl + " cf " + xbToStr gr)

                in ans

            |   (ol, orr) -> sf ("and: malformed bmux tree:" + xbToStr ol + " -o- " + xbToStr orr)
    in band [] (argL, argR)

and bmux_or(argL, argR) =
    let new_known c efo x = // could simplify if x is now true or false.
        if efo=Enum_not then (c, c)
        else ((enum_negate efo)::c, (efo)::c)

    let rec bor known (argL0, argR0) =
        let argL = simplifyb_ass known argL0
        let argR = simplifyb_ass known argR0   
#if XI_QUERY
        let _ = vprintln 4 ("bmux or 0/2 L=" + xbToStr argL0 + "\n In R=" + xbToStr argR0)
        let _ = vprintln 4 ("bmux or 1/2 L=" + xbToStr argL + "\n In R=" + xbToStr argR)
#endif
        if islinp argL && islinp argR && (linp_var argL) = (linp_var argR) then
           let (v, nv) =
               match (argL, argR) with
               | (W_linp(lv, linl, _), W_linp(rv, linr, _)) when lv=rv -> (lv, linrange_disj(linl,linr))
           in xi_linp(v, nv)
        else
        match (argL, argR) with
            | (_, X_true)
            | (X_true, _)   -> X_true
            | (arg,  X_false) 
            | (X_false, arg)  -> arg
            | (W_bmux(gl, ll, lr, _), W_bmux(gr, rl, rr, rmeo)) ->
#if XI_QUERY
                let _ = vprintln 4 ("bmux or " + i2s(xb2order argL) + " " + i2s(xb2order argR))
                let _ = vprintln 4 ("bmux or 2/2 L=" + xbToStr argL + "\n In R=" + xbToStr argR)
#endif
                let gln = xb2order gl
                let grn = xb2order gr                
                let ans =
                    if gln > grn then
                        let efo_l = determine_efo gl
                        let (kff, ktt) = new_known known efo_l gl
                        in xi_bmux(gl, bor kff (ll, argR), bor ktt (lr, argR))
                    elif gln < grn || (gln = grn && gl<>gr) then
                        let efo_r = determine_efo gr
                        let (kff, ktt) = new_known known efo_r gr
                        in xi_bmux(gr, bor kff (argL, rl), bor ktt (argL, rr))
                    else // same order but not necessarily the same (ie could be complements).
                        let efo_r = determine_efo gr
                        let (kff, ktt) = new_known known efo_r gr
                        in xi_bmux(gr, bor kff (ll, rl), bor ktt (lr, rr))      
                    //let _ = (assertf(gl=gr, fun()->"bmux parity:" + xbToStr gl + " cf " + xbToStr gr) 

                in ans

            | (ol, orr) -> sf ("or: malformed bmux tree:" + xbToStr ol + " -o- " + xbToStr orr)
    in bor [] (argL, argR)



    | W_linp(vv, LIN(pol, rng), _) ->
        let efo = determine_efo arg
             // let _ = vprintln 0 ("simplifyb_ass other " + xbToStr other + " efo=" + knwToStr efo)
        let gg = simplify_assuming_efos known arg efo   // pass in new efo ?
        in gg
             
    | W_bmux(gg0, ff, tt, _) ->
        let efo = determine_efo gg0
        let gg = simplifyb_ass known (simplify_assuming_efos known gg0 efo)
#if SASSUMING
        let _ = vprintln 4 ("bmux clause: simplifyb_ass arg =" + xbToStr arg)
        let _ = vprintln 4 ("bmux clause: simplifyb_ass arg gg0 =" + xbToStr gg0)
        let _ = vprintln 4 ("bmux clause: simplifyb_ass arg tt =" + xbToStr tt)
        let _ = vprintln 4 ("bmux clause: simplifyb_ass arg ff =" + xbToStr ff)
        let _ = vprintln 4 ("bmux clause: simplifyb_ass " + xbToStr gg0 + " gave " + xbToStr gg)
        
        let _ = if gg=X_true then vprintln 4 ("True clause inserted is " + xbToStr tt)
                elif gg=X_false then vprintln 4 ("False clause inserted is " + xbToStr ff)
                else ()
#endif        
        in if gg=X_true then simplifyb_ass known tt
           elif gg=X_false then simplifyb_ass known ff
           else xi_bmux(gg, simplifyb_ass known ff, simplifyb_ass known tt)
        

$Id: spare.fs,v 1.4 2011-08-29 13:16:32 djg11 Exp $


        let pair2 g c = function // Consider all pairs on this var. Deqd would not need a three way comp.
            |((X, X_num x, v1, false), (Y, X_num y, v2, false)) -> (*x<a,y<a*) Jimp(Y,X)::c
            |((X, v1, X_num x, false), (Y, X_num y, v2, false)) -> (*a<x,y<a*) Jex(X,Y)::c
            |((X, X_num x, v1, false), (Y, v2, X_num y, false)) -> (*x<a,a<y:gap2*) if g=0 then Jex(X,Y)::c else if g=1 then Jeq(v1, x+1)::c else c
            |((X, v1, X_num x, false), (Y, v2, X_num y, false)) -> (*a<x,a<y*) Jimp(X,Y)::c

            |((X, X_num x, v1, true), (Y, X_num y, v2, false)) -> (*x>=a,y<a*) Jex(X,Y)::c
            |((X, v1, X_num x, true), (Y, X_num y, v2, false)) -> (*a>=x,y<a*) c
            |((X, X_num x, v1, true), (Y, v2, X_num y, false)) -> (*x>=a,a<y*) c
            |((X, v1, X_num x, true), (Y, v2, X_num y, false)) -> (*a>=x,a<y,gap1*) if g=0 then Jeq(v1,x)::c else c 

            |((X, X_num x, v1, false), (Y, X_num y, v2, true)) -> (*x<a,y>=a,gap4*) if g=0 then Jeq(v1,y)::c  else c
            |((X, v1, X_num x, false), (Y, X_num y, v2, true)) -> (*a<x,y>=a*) c
            |((X, X_num x, v1, false), (Y, v2, X_num y, true)) -> (*x<a,a>=y*) c
            |((X, v1, X_num x, false), (Y, v2, X_num y, true)) -> (*a<x,a>=y*) Jex(X,Y)::c

            |((X, X_num x, v1, true), (Y, X_num y, v2, true)) -> (*x>=a,y>=a*) Jimp(X,Y)::c
            |((X, v1, X_num x, true), (Y, X_num y, v2, true)) -> (*a>=x,y>=a,gap3*) c
            |((X, X_num x, v1, true), (Y, v2, X_num y, true)) -> (*x>=a,a>=y*) Jex(X,Y)::c
            |((X, v1, X_num x, true), (Y, v2, X_num y, true)) -> (*a>=x,a>=y*) Jimp(Y,X)::c

            |(_, _) -> c


and xint_assoc_bmux skolem e n w =
        let rec do_assoc_operation = function
            | W_bmux(gg, ff, tt, _) ->
               let gg' = xint_assoc_bexp_w skolem e n gg
               let tt' = do_assoc_operation tt
               let ff' = do_assoc_operation ff
               let (absv, flip) = w_babs(gg')
               in xi_bmux(absv, (if flip then tt' else ff'), (if flip then ff' else tt')) 
            | X_true  -> X_true
            | X_false -> X_false
            | other   -> sf ("other in bmux assoc do_assoc_operation")
        let raw = do_assoc_operation w
#if XI_ASSOC
        let _ = vprintln 3 ("assoc bmux do_assoc_operation: " + xbToStr w + " ===> " + xbToStr raw)
#endif
        if raw = w then w
        else  // Do not reorder if no change made.
        // Want to sort into descending sort order because assoc rewrites may have put things out of order.
        // Why not do this sorting in the xi_bmux constructor ? Because that is called in proper order normally by the xi_and and xi_or functions.
        // We make a single insertion sort operation and the 'rstore' makes a recursive call, giving an insertion sort-like N^2 sorting cost.
        let rec reorder (known:known_t list) = function //
            | X_true  -> X_true
            | X_false -> X_false
            | W_bmux(gg0, ff, tt, _) ->
                let efo = determine_efo gg0
                let gg = simplify_assuming_efos known gg0 efo
#if XI_ASSOC
                let _ = vprintln 4 ("bmux reorder knw=" + knwToStr efo  + " gg0=" + xbToStr gg0  + " gg=" + xbToStr gg)
#endif
                in
                if   gg=X_true  then reorder known tt //
                elif gg=X_false then reorder known ff //
                else
                let go = xb2order gg
                let gff = xb2order ff
                let gtt = xb2order tt
                let level = function
                    | W_bmux(g1, ff, tt, _) -> g1
                    | X_true -> X_true
                    | X_false -> X_false
                    | other -> sf ("level of non bdd item " + xbToStr other)
                let llet arg =
                    match arg with
                    | W_bmux(gg, ff, tt, _) -> ff
                    | X_true | X_false -> arg
                    | other -> sf("llet other " + xbToStr other)
                let rlet arg = 
                    match arg with
                    | W_bmux(gg, ff, tt, _) -> tt
                    | X_true | X_false -> arg
                    | other -> sf("rlet other " + xbToStr other)

                // Sort so highest at top                
                in if go >= gff && go >= gtt then
                    let (known_ff, known_tt) = new_known known efo gg
                    in xi_bmux(gg, reorder known_ff ff, reorder known_tt tt)    // In order ok.

                   else if gtt > go && gtt > gff then
                       let efo = determine_efo tt
                       let (known_ff, known_tt) = new_known known efo tt
                       let rstore_ff x = reorder known_ff (xi_bmux x) 
                       let rstore_tt x = reorder known_tt (xi_bmux x) 
                       in xi_bmux(level tt,  rstore_ff(gg, llet tt, ff), rstore_tt(gg, rlet tt, ff)) // If tt is the largest put above

                   else
                       let efo = determine_efo ff
                       let (known_ff, known_tt) = new_known known efo ff
                       let rstore_ff x = reorder known_ff (xi_bmux x) 
                       let rstore_tt x = reorder known_tt (xi_bmux x) 
                       in xi_bmux(level ff,  rstore_ff(gg, tt, llet ff), rstore_tt(gg, tt, rlet ff))   // all other cases.

        in reorder [] raw   // It would be nice todo a dynamic sift/reorder of tree AND dp HASH CASHE.
    // We find a ROM or ROM region in an array if a subscript is assigned only one constant value: we are therefore ignoring behaviour where there is an initial read of a default value before the first write. TODO: perhaps flag up or fix, but currently this is not a used paradigm.
    let rom_determine its l (ncf, n_subs_val, n_rhs_val) =
        let (found, ov) = vinfo.rominfo.TryGetValue l
        let ov = if found then ov else (true, new rominfo_updates_t("rominfo_updates"))
        //let _ = vprintln 0 (sprintf "rom_determine %s  subs=%A" its n_subs_val)
        let nv =
            match ov with
            | (false, updates)               -> (false, updates) // Already false, leave as.
            | (true, updates) when (not ncf) -> (false, updates) // New update is not const, becomes false.
            | (true, updates)when ncf  ->  // Else, see if location has an alternate update
                let oi = updates.lookup n_subs_val
                if oi = None then
                    let _ = updates.add n_subs_val n_rhs_val
                    (true, updates)
                elif valOf oi = n_rhs_val then (true, updates)    
                else (false, updates) // New value is different - clearly not a ROM afterall.
                
        let _ = if found then ignore(vinfo.rominfo.Remove l)
        let _ = vprintln 0 (sprintf "rom_determine %s  subs=%A nv=%A" its n_subs_val nv)        
        vinfo.rominfo.Add(l, nv)


                let romf =
                        match romcheck_retrieve m vinfo lnn  with
                            | None -> ()
                            | Some updates ->
                                let _ = vprintln 2 (sprintf "romcheck_retrieve: All of %s is a ROM (%i items)" m (length lst0))
    
//
// romcheck_retrieve does a simple lookup of information written by romcheck
//    
let romcheck_retrieve m (vinfo:vinfo_t) nn =
    let _ = vprintln 3 (sprintf "romcheck_retrieve nn=%i" nn) 
    match vinfo.rominfo.lookup nn with
        | None -> None
        | Some(false, updates) -> None
        | Some(true, updates)  -> Some updates// all a ROM
<<<<<<< HEAD
    let ans = ref false
    let unitfn arg =
        match arg with
            | W_apply((f, gis), args, _) -> // walker recurses on args before or after body.
                //let _ = vprintln 3 (sprintf "eis_rhs encounters %s eis=%A"  f gis.eis)
                let _ = if gis.eis then ans := true
                ()
            | _ -> ()
    let lfun strict clkinfo arg rhs = ()
    let null_sonchange _ _ nn (a,b) = b
    let opfun arg N bo xo a b = a 
    let (_, sWALK) = new_walker(true, opfun, (fun (_) -> ()), lfun, unitfn, null_sonchange, null_sonchange)
    let _ = mya_walk_it sWALK g_null_directorate x
    //let _ = vprintln 3 (sprintf "eis_rhs result %A for %s"  (!ans) (xToStr x))
    !ans
=======
        let rec gsys_ee_  = function
            | W_apply(fgis, _, args, _)  -> xi_apply(fgis, map gsys_ee_ args)
            | W_asubsc(baser, idx, _) ->
                if idx = g_kk_old || idx = g_kk then gsys_ee_ baser
                else ix_asubsc (gsys_ee_ baser) (gsys_ee_ idx)

            | X_bnet(ff) ->
                match op_assoc (X_bnet ff) sigma with
                    | None -> X_bnet ff
                    | Some a -> a
            | o -> o
                    

let is_eis msg arg = // use disjunction under walker please... TODO
    let rec is_eis_scan = function1
        | W_apply((fname, gis), cf, args, _) ->
            gis.fsems.fs_eis
        | other ->
            dev_println ("URGENT deep look needed")
            false // not a very deep look at the moment !
    is_eis_scan arg
