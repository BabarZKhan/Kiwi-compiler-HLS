//
// revast.fs - Reverse Engineer a DIC back to an AST - finding loops and induction variables.  This is needed since the bounded FOR loops of typical vector applications may have lost their structure in the front end, such as with Kiwi C# input.
//
//
// All rights reserved. (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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

module revast


open Microsoft.FSharp.Collections

open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open linepoint_hdr
open reporter
open protocols



// Representation of a partially reverse-engineered AST: a hybrid from with index integers and block structures.
type revast_t =
    | RA_leaf of bool * int       // command block: an index to the original DIC augmented with is_decoration bool
    | RA_break
    | RA_continue
    | RA_comment of string
    | RA_if2 of hbexp_t * revast_t list * revast_t list   // A 2-handed IF statement    
    | RA_dowhile of revast_t list * hbexp_t               // A WHILE loop
    | RA_while of hbexp_t * revast_t list                 // A DO loop

let revast_is_decoration = function
    | RA_comment _ -> true
    | RA_leaf(is_decoration, nn) -> is_decoration
    | RA_break
    | RA_continue
    | RA_if2 _
    | RA_while _
    | RA_dowhile _ -> false
    //other -> sf (sprintf "revast_is_decoration other %A" other)


let m_s = ref 100
    
let next_serial() =
    let p = !m_s
    m_s := p+1
    p
    
type softblock_t =
    {
        basis:     int
        predecs:   int list
        work:      revast_t list  // Proto abstract syntax tree fragement
        branches:  (hbexp_t * int) list
        //serial:    int
    }

let rec rvToStr = function
    | RA_leaf(_, nn) -> sprintf "LF%i" nn
    // ...
    | other -> sprintf "?%A" other


let rec branchToStr = function
    | (gg, dd) ->  sprintf " (%s) -> sb%i" (xbToStr gg) dd


let rec scan_common = function
    | (tt::ts, ff::fs) when tt=ff -> // use native equality.
        let (common_prefixes, ts2, fs2) = scan_common (ts, fs)
        (tt::common_prefixes, ts2, fs2)

    | (tt, fs) -> ([], tt, fs)

let scan_common2 (tt, ff) =
    let (common_suffixes, tt, ff) = scan_common (rev tt, rev ff)
    (rev common_suffixes, rev tt, rev ff)


let gec_RA_if2 (gg, tt, ff) =
    if nullp tt && nullp ff then []
    else
    match xbmonkey gg with // Could perfer to rez ff as RA_skip here by negation guard?
        | Some true   -> tt
        | Some false  -> ff
        | None        -> 
            let l_gec_RA_if2 (gg, tt, ff) = if nullp tt then RA_if2(xi_not gg, ff, []) else RA_if2(gg, tt, ff)
            let (common_prefixes, tt, ff) = scan_common (tt, ff)
            let (common_suffixes, tt, ff) = scan_common2 (tt, ff)
            let node = if not_nullp tt || not_nullp ff then [ l_gec_RA_if2(gg, tt, ff) ] else []
            common_prefixes @ node @ common_suffixes

let gec_RA_if1 (gg, cmds) =
    if nullp cmds then []
    else
        match xbmonkey gg with
            | Some true   -> cmds
            | Some false  -> []
            | None        -> [RA_if2(gg, cmds, [])]


// For a basic block sequence of commands, we flatten nested sequences, remove RA_skips and we elide if blocks, making an if2 wherever possible.
// Also, successive if1s may have trailing or leading prefix matches in their bodies, where the replicated code needs hoisiting outside with a disjunction of the guards.

let rec gec_RA_seq lst =
    let rec elider = function
        | [] -> []

        | RA_if2(_, [], _)::_ -> sf "gec_RA_seq - if2 was not in normal form"

        | RA_if2(g1, tt1, ff1) :: RA_if2(g2, tt2, ff2) :: cc when abs (xb2nn g1) = abs (xb2nn g2) ->
            match (xb2nn g1, xb2nn g2) with
                | (n1, n2) when n1 =  n2  -> elider (gec_RA_if2(g1, gec_RA_seq(tt1 @ tt2), gec_RA_seq(ff1 @ ff2)) @ cc)
                | (n1, n2) when n1 = -n2  -> elider (gec_RA_if2(g1, gec_RA_seq(tt1 @ ff2), gec_RA_seq(ff1 @ tt2)) @ cc)
                | _                       ->
                    sf "revast: cf should not happen"
                    RA_if2(g1, tt1, ff1) :: elider(RA_if2(g2, tt2, ff2) :: cc)

        | RA_if2(g1, b1, []) :: RA_if2(g2, b2, []) :: cc when false ->   // This is not a valid transformation - disabled!
            let (common_prefixes, b1p, b2p) = scan_common (b1, b2)
            if not_nullp common_prefixes then
                let nv = gec_RA_seq (gec_RA_if1(ix_or g1 g2, gec_RA_seq common_prefixes) @  gec_RA_if1(g1, b1p) @  gec_RA_if1(g2, b2p) @ cc)
                elider nv
            else
                let (common_suffixes, b1s, b2s) = scan_common2 (b1, b2)            
                if not_nullp common_suffixes then
                    let nv = gec_RA_seq (gec_RA_if1(g1, b1p) @  gec_RA_if1(g2, b2p) @ gec_RA_if1(ix_or g1 g2, gec_RA_seq common_suffixes) @ cc)
                    elider nv

                else RA_if2(g1, b1, []) :: (elider (RA_if2(g2, b2, []) :: cc))

        //| RA_if2(g1, _, []) :: RA_if2(g2, tt2, ff2) :: cc ->  muddy "patteon2" // Eliding if1 body with if2 not implemented ... ?
        //| RA_if2(g1, _, _)  :: RA_if2(g2, tt2, []) :: cc ->  muddy "patteon3"
                

        | other :: tt -> other :: elider tt
    elider lst


// Print and/or convert to hbev code.
// TODO add Xfor form instead of Xwhile?
let render0 ww _ vd msg (ast, leafworko) =
    let newast msg1 ast =
        let treeprint (node:softblock_t) =
            if vd>=5 then vprintln 5 (msg + sprintf ": revast treeprint start. softblock: Node %i" node.basis)
            let rec xprint ind aa =
                let ind = if ind > 55 then 55 else ind // if vd >= 5 then vprintln 5 (indf 0 + "... suppressed")
                let rec indf n = if n>=ind then "" else "  " + indf(n+1)
                match aa with
                    | RA_comment ss ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf "// %s " ss)
                        Xcomment ss

                    | RA_leaf(is_decorationf, nn) ->
                        //let _ = if not_nonep !pref then xprint (ind+1) (valOf !pref)
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " Leaf block %i" nn)
                        let cmds = if nonep leafworko then [] else (valOf leafworko) nn
                        let cmds =
                            let noEOMpred x = x <> g_dic_hwm_marker
                            List.filter noEOMpred cmds
                        app (fun b -> if vd >= 5 then vprint 5 (indf 0 + hbevToStr_cleaned "" b)) cmds
                        gec_Xblock cmds
                        
                    | RA_break ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " break ")
                        Xbreak

                    | RA_continue ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " continue ")
                        Xcontinue

                    | RA_if2(gg, work, []) ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " if1(%s, " (xbToStr gg))
                        let work' = map (xprint (ind+2)) work
                        //if vd>=5 then vprintln 5 (indf 0 + sprintf " )")
                        gec_Xif1 gg (gec_Xblock work')

                    | RA_if2(gg, [], work) ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " ifnot(%s, " (xbToStr gg))
                        let work' = map (xprint (ind+2)) work
                        //if vd>=5 then vprintln 5 (indf 0 + sprintf " )")
                        gec_Xif1 (xi_not gg) (gec_Xblock work')

                    | RA_if2(gg, tt, ff) ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " if2(%s, " (xbToStr gg))
                        let tt' = map (xprint (ind+2)) tt
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " ) else ")
                        let ff' = map (xprint (ind+2)) ff
                        //if vd>=5 then vprintln 5 (indf 0 + sprintf " )")
                        gec_Xif gg (gec_Xblock tt') (gec_Xblock ff')

                    | RA_while(gg, work) ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " While(%s, " (xbToStr gg))
                        let work' = map(xprint (ind+2)) work
                        //if vd>=5 then vprintln 5 (indf 0 + sprintf " )")
                        Xwhile(gg, gec_Xblock work')
#if SPARE_FOR_NOW                        
                        let blp =
                            {
                                parf=  false
                                start= 0L
                                endv=  0L
                                step=  0L
                            }
                        Xbloop(X_undef, work', blp)
#endif
                    | RA_dowhile(work, gg) ->
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " do(")
                        let work' = map(xprint (ind+2)) work
                        if vd>=5 then vprintln 5 (indf 0 + sprintf " while (%s)" (xbToStr gg))
                        Xdo_while(gg, gec_Xblock work')
            map (xprint 0) node.work
        treeprint ast

    match ast with
        | [node] ->
            let msg1 = msg + (sprintf ": revast: render0 singleton. %s node.basis=%i summary_onlyf=%A" msg node.basis (nonep leafworko))
            vprintln 2 msg1
            let mc = Xcomment msg1
            mc :: newast msg1 node
        | asts ->
            let msg1 = msg + sprintf ": revast: render0: %s +++ node count %i" msg (length asts)
            vprintln 0 msg
            let bp x =
                let _ = newast msg1 x
                ()
            app bp asts
            sf (sprintf "revast: render0: %s not applied to one block. arity=%i sl=%s" msg1 (length asts) (sfold (fun x ->i2s x.basis) asts))
    // end



//
// scombine: Aho natural loop operator: combine cand with its one ant and see what sort of AST production we get.
//    
let scombine ww vd (mm:Map<int, softblock_t>) nodes__ (cand:softblock_t) =
    let ant =
        match mm.TryFind (once "revast: scombine" cand.predecs) with
           | None -> sf "There will be only one ant by definition."
           | Some ant -> ant
        //if vd>=5 then vprintln 5 (sprintf "revast: found %i %i   and   %i %i " (length ants1) (length others1b) (length ants2) (length others2b))
//  vprintln 0 (sprintf "Found ant %A" ant)
//  let alts = List.filter (fun sb->sb.basis=hd cand.predecs) nodes__
//  vprintln 0 (sprintf "Alternative ants=%A" alts)
//  let ant = once "L378" alts
    let ans =
        vprintln 3 (sprintf "revast: try combine %i with %i" ant.basis cand.basis)
        //We retain the basis id of the ant, so others continue to forward refer to it, but we need to update the predecs lists of cand's successors.
        //The ant replacement takes on the successors of the cand, excluding itself, which is when we likely find natural loop.
        let cand_succs = lst_subtract (list_once (map snd cand.branches)) [ant.basis]
        let (ant_yy, ant_nn) =
            let xp (gfwd, dd) = dd=cand.basis // This is the goto between ant and cand that is always eaten per iteration.
            groom2 xp ant.branches
        let (preference, ctr, workf) =
            match (ant_yy, ant_nn) with // Generally, yy is deleted and nn is augmented with cand.ctr
                | ((gfwd, dd)::aux, ant_nn) -> // Check 1-handed IF since we dont check in aux here. The 2-handed is mined from the rez routines.

                    let (cand_yy, cand_nn) =
                        let yp (gfwd, dd1) = dd1=ant.basis // Filter to find cand_yy which are the backedge gotos between cand and the ant that may be processed.
                        groom2 yp cand.branches

                    match cand_yy with
                        | (gback, cand_dest)::aux2 when cand_dest=ant.basis ->  // Backarc: A 2-loop found: the candidate has a successor which is the anterior.
                            let deep_gfwd = ix_orl (map fst (List.filter (fun (b,d)->d=cand.basis) ant.branches))
                            let msg() = (sprintf "revast: patch to dowhile/while gback=%s gfwd=%s deep_gfwd=%s ant.basis=%i cand.basis=%i  |cand.succs|=%i |cand_succs|=%i |aux|=%i |aux2|=%i |cand_nn|=%i" (xbToStr gback) (xbToStr gfwd) (xbToStr deep_gfwd) ant.basis cand.basis (length cand.branches) (length cand_succs) (length aux) (length aux2) (length cand_nn))
                            let gfwd = ix_and gfwd deep_gfwd
                            vprintln 3 (msg())

                            // Need  to watch out for two successive loops from same continue point - can loose track of their order ... hmm
                            // All gback's need to be X_true for a while loop
                            if xi_istrue gback && length cand_succs < 2 then // TODO : too tight - we are happy with any number of back edges - separate whiles or else a while with continue stmt.
                                dev_println ("fst: two is too tight TODO")
                                let new_cand_workf () =
                                    vprintln 3 (sprintf "Rez/Seed while %i %i" ant.basis cand.basis)
                                    muddy "pog3"
                                    //patch_ra None cand_dest (fun x -> gec_RA_seq(x @ [RA_comment(sprintf "w/d was goto %i" cand.basis)])) cand.work
                                let workf = fun () ->
                                    if not_nullp cand_nn then sf(sprintf "Expected null cand_nn")
                                    
                                    if conjunctionate (revast_is_decoration) ant.work then
                                        vprintln 3 (sprintf "patch to simple RA_while.")
                                        [RA_while(gfwd, gec_RA_seq(ant.work @ cand.work))]
                                         
                                        
                                    else
                                        vprintln 3 (sprintf "patch to RA_while(true)/RA_break")
                                        let bdy = gec_RA_seq(ant.work @ gec_RA_if2(xi_not gfwd, [RA_break], []) @ cand.work)
                                        [RA_while(X_true, bdy)]
                                    //patch_ra (Some(xb2nn gfwd)) dd (fun x -> [RA_while(gfwd, gec_RA_seq(x @ new_cand_workf()))]) ant.work // A modified ant work now contains this and both gotos eaten.
                                // The ant body will/should be just the condition test at the point of patch - the IF needs ideally replacing since it will become just an extra guard on while loop entry
                                let ctr = (aux @ ant_nn @ aux2 @ cand_nn)
                                (80, ctr, workf) // Prefer whiles over do's.
                            elif xi_istrue gfwd && length cand.branches = 2 then // One back edge and one loop exit edge needed for a do loop.
                          // How do we know how much of ant is inside the while: we need to patch at a label, so labels are needed <------------
                             // The gback at dd needs to be just removed and the dowhile needs to be inserted ahead of the ant block
                            // The forward patch will be the whole existing work when applied in the correct order
                                                    //let work = patch_ra None dd (fun x -> [RA_dowhile(x @ new_cand_work, gback)]) ant.work // A modified ant work now contains this and both control flow ops (the jump and the branch) eaten.
                                dev_println ("snd: two is too tight TODO")
                                
                                let workf () =
                                    if nullp ant_nn then
                                        vprintln 3 (sprintf "Rez/Seed dowhile %i %i grd=%s"  ant.basis cand.basis (xbToStr gback))
                                        //let new_cand_work = patch_ra None cand_dest (fun x -> x @ [RA_comment(sprintf "d/w was goto %i" cand.basis)]) cand.work
                                        [RA_dowhile(gec_RA_seq(ant.work @ cand.work), gback)]
                                    else
                                        muddy "revast: ant_nn present"
                                let ctr = aux @ ant_nn @ aux2 @ cand_nn
                                (70, ctr, workf)
                            else
                                dev_println (sprintf "No loop detect on cx %A  cand.branches=%A cand_succs=%A" (xi_istrue gfwd) (sfold branchToStr cand.branches) (sfold i2s cand_succs))
                                dev_println (msg() + ": Aho revast requires natural loops only. cand.sucss=" + sfold branchToStr cand.branches)
                                (-1, [], fun () -> [])

                        | _ ->
                            //(cand.branches @ aux @ nn, patch_ra (Some(xb2nn gg)) dd (gec_RA_if2(gg, cand.work)) ant.work) // OLD: Remove a guard and add back the same! Relies on withg mechanism
                            let ant_succs = length ant.branches
                            let preference = if ant_succs=1 then 101 else 50 
                            vprintln 3 (sprintf "revast: Rez Pair Other. combine %i with %i   gfwd=%s  |ant.branches|=%i |cand.branches|=%i" ant.basis cand.basis (xbToStr gfwd) ant_succs (length cand.branches))
                            //vprintln 0 (sprintf "cand.branches=%A  aux=%A ant_nn=%A" cand.branches  aux  ant_nn)
                            //let elider_thunk = fun () -> patch_ra None dd (fun x -> gec_RA_seq(x @ cand.work)) ant.work
                            let elider_thunk = fun () -> gec_RA_seq(ant.work @ cand.work)
                            
                            (preference, (cand.branches @ aux @ ant_nn), elider_thunk) // Simple forward patch using existing IF

#if SPARE
// Why is this considered spare?
                        | ([(gfwd, dd)], []) -> // Single successor without back edge: the trivial case. 
                            vprintln 3 (sprintf "revast: Rez trivial:  ant.basis=%i cand.basis=%i no back edge found. gfwd=%s  |cand.branches|=%i" ant.basis cand.basis (xbToStr gfwd) (length cand.branches))
                            (99, cand.branches @ ant_nn, fun () -> patch_ra None dd (fun x -> gec_RA_seq(x @ cand.work)) ant.work)

                        | ([RV_goto(pp, gg, dd); RV_goto(pp', gg', dd')], []) when pp=pp' && xb2nn gg = 0 - xb2nn gg' -> // This clause for 2-handed IF should never match
                                       sf "two iff"
#endif
                | (ant_yy, ant_nn) ->
                    hpr_yikes (sprintf "revast: unexpected configuration: L185 ant=%i,  cand=%i, other. yy=%s   nn=%s" ant.basis cand.basis (sfold branchToStr ant_yy) (sfold branchToStr ant_nn))
                    (-1, [], (fun () -> []))
                                        
        let newabsf () =
            let remaining_branches =
                let remate (gg, dd) = dd <> cand.basis && dd <> ant.basis
                List.filter remate (ant.branches @ cand.branches)

            { ant with
                  branches=   remaining_branches
                  work=       workf()
              //serial=  next_serial()
            }
        Some(cand, preference, newabsf, ant.basis)
    ans
                
//
// fwd_combiner finds forward joins in the control flow graph arising typically after an IF statement.  
//   
let fwd_combiner ww vd nexter limitval nodes =

    let rec fwd_iterate limit nodes =
        if limit < 0 then sf "revast: ran out of fwd_iterate steps: try -revast_enable=disable or similar."
        vprintln 3 (sprintf "revast: fwd_iterate limit=%i" limit)
        let mm =
            let addx (mm:Map<int, softblock_t>) sb = mm.Add(sb.basis, sb)
            List.fold addx Map.empty nodes

        let one_ant tf nn =
            let sb = valOf_or_fail "L520" (mm.TryFind nn)
            if tf then vprintln 3 (sprintf "   one_ant sb %i predecs=%s" sb.basis (sfold i2s sb.predecs))
            (length sb.predecs = 1)


        let recombine sb dest nodes =
#if OLD
            let rec collate_to_dest limit expect_uncond (gg, xnn) =
                if expect_uncond && not(xi_istrue gg) then [] //sf (sprintf "revast: expected uncond")
                elif limit < 0 then sf "revast: limit failed"
                elif xnn = dest then [[]]
                else
                    let sb = valOf_or_fail "l635" (mm.TryFind xnn)
                    if one_ant sb.basis then
                       let dl = map (collate_to_dest (limit-1) true) sb.branches
                       map (fun dli -> xnn :: dli) (list_flatten dl)
                    else []

            let paths = list_flatten(map (collate_to_dest limitval false) sb.branches )
#endif
            let tf = sb.basis=71

            let new_branch_info =
                let collate_to_dest (gg, ddd) =
                    let rec collate_to_dest limit dd =
                        if dd=dest then (true, [])
                        else
                            let sb = valOf_or_fail "l635" (mm.TryFind dd)
                            if one_ant tf sb.basis then
                                match sb.branches with
                                    | [(gg, dx)] when xi_istrue gg ->
                                        let (foundf, path) = collate_to_dest (limit-1) dx
                                        (foundf, sb::path)
                                    | _ -> (false, [])
                            else (false, [])
                    let (foundf, intermediates) = collate_to_dest limitval ddd
                    (gg, ddd, foundf, intermediates)
                map collate_to_dest sb.branches

            // The groom discards precedence amongst the guards so we want them to be properly disjoint : TODO check formed that way please.
            
            let (old_branches, nowork_branches, work_branches) =
                let groomer (gg, ddd, foundf, intermediates) (c1, c2, c3) =
                    if not foundf then ((gg, ddd)::c1, c2, c3)
                    elif nullp intermediates then (c1, gg::c2, c3)
                    else (c1, c2, (gg, intermediates)::c3)

                List.foldBack groomer new_branch_info ([], [], [])

            vprintln 3 (sprintf "revast: recomb %i -> %i old_branches^=%i, nowork_branches^=%i, work_branches^=%i" sb.basis dest (length old_branches) (length nowork_branches) (length work_branches))

#if SPARE
            let nowork_xitions =
                let collated = generic_collate snd nowork_branches
                let gdisj (ddd, arcs) = (ix_orl (map fst arcs), ddd)  // If two arcs to same dest, just make disjunction of guards.
                map gdisj collated
#endif
            let nowork_xitions =
                if nullp nowork_branches then [] else [(ix_orl nowork_branches, dest)]  // If two arcs to same dest, just make disjunction of guards.

            let (m_to_delete, m_fresh_sbs) = (ref [], ref [])



            let refined =
                let rec folda = function
                        | [] -> []
                        | (g1, [])::tt -> folda tt // There is no stall so safe to delete nowork xitions - TODO verify
                        | (g1, intermediates) :: tt ->
                            let pathwork = list_flatten (map (fun x -> mutadd m_to_delete x.basis; x.work) intermediates)
                            gec_RA_if2(g1, pathwork, folda tt)
                let xlst = work_branches @ (if nullp nowork_branches then [] else [(ix_orl nowork_branches, [])])
                let new_work = folda xlst

                if nullp old_branches then // Where dest is a full postdominator, there are no old branches, a new softblock is not needed and the conditional work is appended to the work in the current block

                    let refined = { sb with work=sb.work @ new_work; branches=[(X_true, dest)] } /// TODO  correct predecs in this.
                    (refined)

                elif nullp new_work then
                    vprintln 3 (sprintf "revast: recomb: dont need a new softblock since no new_work")
                    let gxx = ix_orl(map fst xlst)
                    let refined = { sb with branches= (gxx, dest)::old_branches } 
                    (refined)

                else // A fresh softblock to contain the new_work is needed since the old soft_block still needs to make an xition to the old_branches
                    let nbasis = nexter()

                    vprintln 3 (sprintf "revast: recomb %i -> %i: path %s.  Fresh softblock basis=%i pathwork^=%i" sb.basis dest (sfold (fun x->i2s x.basis) []) nbasis (length new_work)) 

                    let nb = { basis=nbasis;  predecs=[sb.basis]; work=new_work; branches=[(X_true, dest)] }
                    mutadd m_fresh_sbs nb

                    let new_branch = (ix_orl (map fst xlst), nbasis)  // The nowork route can come to new block or straight to old dest.  We send it to the new block where it is a nop.

                    let refined = { sb with branches=new_branch::old_branches } /// TODO  correct predecs in this.
                    (refined)
                
            let deletes = !m_to_delete
            vprintln 3 (sprintf "revast: recomb %i-> %i  deletes=" sb.basis dest + sfold i2s deletes)


                
            let nodes =
                let recomb_updates sb1 cc =
                    if memberp sb1.basis deletes then cc
                    elif sb1.basis = dest then
                        let patch_ant ant = if memberp ant deletes then sb.basis else ant
                        { sb1 with predecs=map patch_ant sb1.predecs } :: cc
                    elif sb.basis = sb1.basis then
                        //vprintln 3 (sprintf "revast: Processed recomb_cand post= %A" refined)
                        refined :: cc 
                    else sb1 :: cc

                List.foldBack recomb_updates nodes []

            !m_fresh_sbs @ nodes



    // Find common immediate postdominators for each successor of blk (hence no other fanin) and also exclude back edges.
        let bf_scanner blk =
            let tf = blk.basis=71
            if tf then vprintln 3 (sprintf "revast: bf_scanner from %i" blk.basis)
            let rec bf_scan moves stack_ ldlst rdlst ll0 rr0 =
                if ll0=rr0 then
                    vprintln 3 (sprintf "recomb collide %i -> %i  at  %s    %s   moves=%i"  blk.basis ll0 (sfold i2s ldlst) (sfold i2s rdlst) moves)
                    Some(blk, ll0, moves)
                else
                let ll = valOf_or_fail "L604" (mm.TryFind ll0)
                let rr = valOf_or_fail "L605" (mm.TryFind rr0)

                let lf = map snd ll.branches
                match lst_intersection lf rdlst with
                    | a::b::_ -> muddy ("revast two or more left")
                    | [item] -> 
                        vprintln 3 (sprintf "recomb found left %i -> %i  at  %s    %s   moves=%i"  blk.basis item (sfold i2s ldlst) (sfold i2s rdlst) moves)
                        Some(blk, item, moves)
                    | [] ->
                        let ldlst1 = lf  @ ldlst
                        let rf = map snd rr.branches
                        match lst_intersection rf ldlst1 with
                            | a::b::tt -> muddy (sprintf "revast: %i two or more right " blk.basis + sfold i2s (a::b::tt))
                            | [item] ->
                                vprintln 3 (sprintf "recomb found right %i -> %i  at  %s    %s  moves=%i"  blk.basis item (sfold i2s ldlst) (sfold i2s rdlst) moves)
                                Some(blk, item, moves)
                            | [] -> 
                                let rdlst1 = rf @ rdlst
                                if tf then vprintln 3 (sprintf "revast: bf_scanner from %i   ldlst=%s  rdlst=%s" blk.basis (sfold i2s ldlst) (sfold i2s rdlst))
                                let move mylst bb =
                                    let bbx = valOf_or_fail "move-:541" (mm.TryFind bb)
                                    match bbx.branches with
                                        | [(gg, dest)] when xi_istrue gg && one_ant tf dest && not(memberp dest mylst) -> dest
                                        | _ ->
                                            if tf then vprintln 3 (sprintf "     not uncond from %i   %s" bb (sfold branchToStr bbx.branches))
                                            bb
                                let ll' = move ldlst ll0
                                let rr' = move rdlst rr0
                                if tf then vprintln 3 (sprintf "  move?  %i  %i" ll' rr')
                                if ll' = ll0 && rr' = rr0 then None
                                else
                                    let moves = moves + (if ll'<>ll0 then 1 else 0) + (if rr'<>rr0 then 1 else 0)
                                    bf_scan moves (ll0::rr0::stack_) ldlst1 rdlst1 ll' rr'

            let rec try_all_pairs cc = function
                | []  -> cc
                | [h] -> cc
                | (_, h0)::t0 ->
                    let rec try2 cc = function
                        | []          -> cc
                        | (_, hh)::tt ->
                            let rr =
                                if tf then vprintln 3 (sprintf "bf_scan start at  %i  %i" h0 hh)
                                bf_scan 0 [blk.basis] [h0] [hh] h0 hh // Consider each pairing of sb.branches when there are two or more.
                            let cc = if nonep rr then cc else (valOf rr) :: cc
                            try2 cc tt
                    try_all_pairs (try2 cc t0) t0
            try_all_pairs [] blk.branches

        let rec scan = function
            | []      -> (false, nodes)
            | blk::tt ->
                match bf_scanner blk with
                    | [] ->
                        scan tt
                    | findings ->
                        let findings = List.sortWith (fun (_, _, a) (_, _, b) -> a-b) findings //Sort into ascending order so that smallest no of moves is at the head.
                        let (target, dest, cost) = hd findings
                        vprintln 3  (sprintf "revast: (selections^=%i) will make fwd comb %i -> %i  cost=%A" (length findings) blk.basis dest cost)
                        let nodes = recombine blk dest nodes
                        (true, nodes)


        let (changed, nodes) = scan nodes
        if changed then fwd_iterate (limit-1) nodes else nodes


    let nodes = fwd_iterate limitval nodes
    nodes



let backwards_combiner ww vd entry_point nodes =
    let rec back_iterate nodes = 
        if length nodes < 2 then
            vprintln 3 (sprintf "revast: softblock combine at target node count %i" (length nodes))
            nodes
        else

#if SPARE
        // A check for single-block loops in input.
        let process_reflexive cand (changed, cc) =
            if not(memberp cand.basis cand.predecs) then (changed, cand::cc)
            else // The patch at the end of the last iteration should stop these, so this is just a check now.
                sf (sprintf "Found reflexive predec for basis=%i - should not arise with poor basic blocks (but an infinite hang or spin lock needs attention ... ? item=%A)" cand.basis cand)
                let cand = { cand with predecs= list_subtract(cand.predecs, [cand.basis]) }
                (true, cand::cc)
        let rec processes_reflexive_nodes nodes =
            let (changed, nodes) = List.foldBack process_reflexive nodes (false, [])
            if changed then processes_reflexive_nodes nodes else nodes
        let nodes = processes_reflexive_nodes nodes
#endif
        vprintln 0 (sprintf "revast: nodes in play x: " + sfold (fun node->i2s node.basis + ":" + sfold i2s node.predecs + "/" + sfold branchToStr node.branches + "    ") nodes)
        // Recombinations - find: a node with two routes to a common successor with no other earlier destinations.

        let mm =
            let addx (mm:Map<int, softblock_t>) sb = mm.Add(sb.basis, sb)
            List.fold addx Map.empty nodes

            
        // Aho - candidates with one anteriour.
        let cands = List.filter (fun h -> h.basis <> entry_point && length h.predecs = 1) nodes
            
        if vd>=4 then vprintln 4 (sprintf "revast iterate start: %i candidates have one predecessor out of %i nodes." (length cands) (length nodes))
        if nullp cands then
            if vd >= 5 then
                    vprintln 5 (sprintf "revast: no candidates: others=%s" (sfold i2s (map (fun x->x.basis) nodes))) 
                    app (fun cand -> if vd>=4 then vprintln 4 (sprintf "revast:   non-candidate %A,  preds now=%s" cand.basis (sfold i2s cand.predecs))) nodes
            nodes
        else
            let anal cand cc = 
                if vd>=5 then vprintln 5 (sprintf "revast: found candidate %A,  preds now=%s" cand.basis (sfold i2s cand.predecs))
                match scombine ww vd mm nodes cand with
                    | Some (cand, preference, newabsf, old_ant) when preference >= 0 ->
                        vprintln 0 (sprintf "Reduction: cand=%i ant=%i preference=%i" cand.basis old_ant preference)
                        (cand, preference, newabsf, old_ant) :: cc
                    | _ -> cc

            // Sort to find the most-simple candidate - one that does not increase AST exponentially by rewriting (replicating?) in multiple places.
            let sorted = List.sortWith (fun b a -> f2o4 a - f2o4 b) (List.foldBack anal cands [])
            match sorted with
                | [] -> sf "revast: No candidate nodes left"
                | (cand, preference, newabsf, ant_basis) :: _ ->
                    if vd >= 5 then vprintln 5 (sprintf "revast: SELECTED combine %i with %i having preference=%i" ant_basis cand.basis preference)
                    let newabs = newabsf()
                    //vprintln 0 (sprintf "newabs is %A" newabs)
                    let print_each_step = false
                    if print_each_step then ignore(render0 ww true vd "mini/interim ast print" ([newabs], None)) // If we do not provide leafwork function, this just gives a summary to log file
#if SPARE
                    if !m_nodes > 50000 then
                        dev_println "Stop on v-big"
                        let _ = render0 ww true vd "b-big ast print" ([newabs], None) // If we do not provide leafwork function, this just gives a summary to log file
                        sf "revast: Stop v-big"
#endif
                    let filterp node = node.basis<>cand.basis && node.basis<>ant_basis
                    let patch_traj anode = // Patch all nodes to refer to the ant where they referred to the candidate.
                        let patch_traj_serf basis (g, n) cc =
                            if n=basis then cc // Remove newly-formed reflexive traj (there were none originally)
                            elif n=cand.basis then (if ant_basis<>basis then singly_add (g, ant_basis) cc else cc) // Add remainder of candidate's successors to the ant. ie patch newabs
                            else singly_add (g, n) cc
                        let patch_ant_serf basis (n) cc =
                            if n=basis then cc // Remove newly-formed reflexive traj (there were none originally)
                            elif n=cand.basis then (if ant_basis<>basis then singly_add (ant_basis) cc else cc) // Add remainder of candidate's successors to the ant. ie patch newabs
                            else singly_add (n) cc

                        let ans = { anode with predecs=List.foldBack (patch_ant_serf anode.basis) anode.predecs []; branches=List.foldBack (patch_traj_serf anode.basis) anode.branches [] }
                        //dev_println (sprintf "Patched traj for basis=%i  ants=%s succs=%s" ans.basis (sfold i2s ans.predecs) (sfold i2s ans.branches))
                        ans
                    let newlst = map patch_traj (newabs::(List.filter filterp nodes))
                    //vprintln 0 (sprintf "newabs patched is %A" (hd newlst))
                    back_iterate newlst
    let nodes = back_iterate nodes
    nodes

    
// Aho limit graph algorithm for natural loops.
// Useful for visit ratio estimating too.
// Where a candidate block has one antecedent we merge it with that antecedent, making a new abstract block that takes on the candidate block's successors.
// Not implemented: if we have irreducible loops we will get stuck with no candidates, so we select a candidate with a minimal number of antecedents and replicate it for each ant.
let ast_reveng_find0 ww vd id sp2 = 
    let richf = (Some false) // We want so-called 'poor' blocks   
    let _ =
        let execs = [sp2]
        let title = filename_sanitize [ '.'; '_'] (id + "_KEYIN")
        let key = Some title   // Create dot cfg plot of the input machine.  // TODO turn off by default.
        if not_nonep key then
            let dot_reports = hpr_sp_dot_report ww title richf execs
            //if vd>=5 then vprintln 5 (sprintf "dotcode %A" code)
            let keyname = valOf key
            render_dot_plot ww keyname dot_reports
            ()


    let (director, ar0, ctrl) =
        match sp2 with
            | H2BLK(director, SP_dic(ar0, ctrl)) -> (director, ar0, ctrl)
            | _ -> sf "revast@ sp2 not DIC form"
    let (bblock_starts, numbering, fwd, back) = collate_dic_blocks ww 10 richf "ast_reveng_find0" ar0 // Analyse all control flow
    let entry_point = 0

    let bblock_starts =
        let reachable_bblock nn =
            nn = 0 ||
            match back.TryFind nn with
                | Some _ -> true
                | None ->
                    vprintln 3 (sprintf "revast: basic block %i is not reachable and so is now being discarded." nn)
                    false
        List.filter reachable_bblock bblock_starts


    let limitval = length bblock_starts
        
    let leafworkf pc =  // Non-control flow commands are called work and are returned by the leafwork function.
        let rec collect_work pc1 cc =
            if pc1 >= ar0.Length then cc
            else
            if pc <> valOf_or_fail "gwn" (numbering.TryFind pc1) then cc // If a different block then stop.
            else
                let tailer = collect_work (pc1+1) cc
                match ar0.[pc1] with
                    | Xbeq _
                    | Xgoto _               // We do not want the control flow commands as part of leaf work, except for Xreturn perhaps.
                    | Xlabel _ -> tailer
                    | cmd -> cmd :: tailer 
        collect_work pc []


    //if vd>=5 then vprintln 5 (sprintf "basic block numbering: %s" (Map.foldBack (fun a b cc -> sprintf "(%i %i)" a b + cc ) numbering ""))
    if vd>=5 then vprintln 5 (sprintf "basic block starts: %s" (sfold i2s bblock_starts))
    let m_nextnum = ref(max (ar0.Length + 10) 500)
    let nexter () = let r = !m_nextnum in (m_nextnum:=r+1; r) 



    let concourse_softblocks =
        let absnode_gen start cc =
            let predecessors = valOf_or_nil(back.TryFind start)
            let real_predecs = lst_subtract predecessors [start]
            let ante_arity = length real_predecs
            let branches = valOf_or_nil(fwd.TryFind start)
            let succs = map snd branches
            if vd>=5 then vprintln 5 (sprintf " basis %i -> %s " start (sfold (fun x->x) (map (fun (g,d) -> xbToStr g + ":::" + i2s d + "   ") (valOf_or_nil(fwd.TryFind start)))))

#if SPARE
             see hbev_is_decoration
            let tlp = function
                | Xlinepoint(_, Xskip) -> false
                | Xcomment _           -> false
                | _ -> true
            if length succs0 > 1 && length (List.filter tlp (leafworkf start)) > 0 then sf (sprintf "pc=%i : Double-check failed: Not a poor block %A" start (leafworkf start))
#endif
            let work =
                let is_decorationf = conjunctionate hbev_is_decoration (leafworkf start)
                let basiswork = RA_leaf(is_decorationf, start)
                [ basiswork ]
            let ans = { (*serial=next_serial(); *) basis=start; branches=branches; predecs=real_predecs; work=work }
            if vd>=4 then vprintln 4 (sprintf "revast: abstract_node: basis=%i  ants=%s" start (sfold i2s real_predecs))
            ans::cc
        List.foldBack absnode_gen bblock_starts []


    vprintln 3 (sprintf "revast: created %i softblocks from input cfg" (length concourse_softblocks))
    

    let nodes = concourse_softblocks
    vprintln 3 (sprintf "revast: start fwds combiner (recombiner) on %i softblocks" (length nodes))
    let nodes = fwd_combiner ww vd nexter limitval nodes
    vprintln 3 (sprintf "revast: move on to backwards combine with %i softblocks" (length nodes))

    if true then
        let tplot sb =
            vprintln 3 (sprintf "  tplot %i  (work^=%i) --->  %s" sb.basis  (length sb.work) (sfold (fun (g,d)->i2s d) sb.branches))

        app tplot nodes
    let ast = backwards_combiner ww vd entry_point nodes

    (director, ctrl, ast, leafworkf)


//
let ast_reveng_find ww vd msg  minfo sp3 = 
    let id = msg
    let ww = WF 2 "ast_reveng_find" ww ("Start " + id + sprintf " (vd=%i)" vd)
    let (director, ctrl, asts, leafwork) = ast_reveng_find0 ww vd id sp3
    let ww = WF 2 "ast_reveng_find" ww "Projection to AST complete."
    let _ = render0 ww false vd "Summary for log file" (asts, None) // This gives a summary to log file
    let ast =
        match asts with
            | [ast] -> ast
            | other -> sf (sprintf "Revast: expected one AST block, not %i." (length other))
    let newast = render0 ww false vd "Render to hbev" ([ast], Some leafwork)
    let queries =
      { g_null_queries with
          yd=              YOVD vd
          full_listing=    true
          concise_listing= false
          logic_costs=     None
          title=           "revast post-project report"
      }
    let new_sp = SP_l(ctrl.ast_ctrl, gec_Xblock newast)
    let newblk = H2BLK(director, new_sp)

    let _ = anal_block ww None queries newblk//(H2BLK(director, SP_l(ctrl.ast_ctrl, newast)))

    let _ =
        let key = filename_sanitize [ '.'; '_'] (id + "_KEYOUT")
        if key <> "" then
            vprintln 2 (sprintf "Making post-revast dot report key=%s" key)
            let dot_reports = hpr_sp_dot_report ww key None [newblk]
            //if vd >= 5 then vprintln 5 (sprintf "dotcode %A" code)
            render_dot_plot ww key dot_reports
            ()

    let return_roundtripped = true
    // Can now round-trip it back to a DIC for confidence tests
    if return_roundtripped then
        let ww = WF 2 "ast_reveng_find" ww "Roundtrip back to DIC start"
        let labprefix = "REVAST"
        let sp4 = compile_exec_to ww vd msg (minfo:minfo_t) newblk
        //let newast2 = compile_hbev_to_lst ww ctrl.ast_ctrl director msg minfo labprefix [newast]
        //let sp4 = H2BLK(director, newast2)
        let _ = anal_block ww None queries newblk
        let ww = WF 2 "ast_reveng_find" ww "Finished"
        sp4
    else newblk
// eof
    
