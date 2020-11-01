(*  $Id: bevctrl.fs,v 1.12 2012-11-12 11:52:21 djg11 Exp $
 * Orangepath: bevctrl - behavioural code manipulations
 *
 *
 *)
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

module bevctrl


open hprls_hdr
open moscow
open meox
open abstract_hdr
open abstracte
open dpmap
open yout
open opath_hdr
open opath_interface
open linprog


//
// Bubble sort, largest returned first, deleting items with arity below theshold.
//
let trimming_bubble_sort thresh lst = 
    let m_changed = ref false
    let rec pass = function
        | (a0, a1, abody)::tt when a0+a1 < thresh   -> (m_changed := true; pass tt)
        | (a0, a1, abody)::(b0, b1, bbody)::tt when a0+a1<b0+b1 -> (m_changed := true; (b0,b1,bbody) :: pass ((a0,a1,abody)::tt))
        | x::tt -> x::(pass tt)
        | other -> other
    let rec iterate lst =
        let lst' = pass lst
        if !m_changed then
            let _ = m_changed := false
            iterate lst'
        else lst
    iterate lst



// Heuristic for rendering as an n-way case statement.
// We consider only where the tags to be constant (but in Verilog output they do not have to be).
// Essentially are gl and g members of the same unary strobe group.
let caseshare gl g =
    let gl = deblit gl
    let g = deblit g // Can do this better via unary strobes
    let ans =
        //let _ = vprintln 0 ("xbkey " + xbkey g)
        match g with
        | W_linp(v, LIN(false, [a;b]), _) when b=a+1 ->
            //vprintln 0 ("A const comp lhs at least")  // strings sort after variables?
            let metric1 = function
                | W_linp(v', LIN(false, [a'; b']), _) when v = v' && b' = a'+1 -> true
                | _ ->false              
            metric1 (gl)

        | W_bdiop(V_deqd, [a;b], false, _) when constantp a ->
            //vprintln 0 ("A const comp lhs at least")  // strings sort after variables?
            let metric1 = function
                | W_bdiop(V_deqd, [a'; b'], false, _) when b'=b -> true
                | _ ->false              
            metric1 (gl)

        | W_bdiop(V_deqd, [a;b], false, _) when constantp b ->
            //vprintln 0 ("A const comp rhs at least")
            let metric1 = function
                | W_bdiop(V_deqd, [a'; b'], false, _) when a'=a -> true
                | _ ->false              
            metric1 (gl)

        | W_bdiop(V_deqd, [a;b], false, _)  ->
            let _ = dev_println (xToStr a + " caseshare not const - no action" + boolToStr (constantp a))
            false
            
        | _ -> false

    // let _ = vprintln 0 ("caser " + xbToStr gl + " cf " + xbToStr g + " gives " + boolToStr ans)
    ans


let caseshare_split (arg, cmds) =
    match deblit arg with
        | W_linp(v, LIN(false, [a;b]), _) when b=a+1 -> (v, xi_num a, cmds)
        | W_bdiop(V_deqd, [a;b], false, _) when constantp a -> (b, a, cmds)
        | W_bdiop(V_deqd, [a;b], false, _) when constantp b -> (a, b, cmds)
        | other -> sf ("caseshare_split other:" + xbToStr other)


let case_clause_sort x =
    let lcm_atoi = xi_manifest_int 
    let pred (_, x, _) (_, y, _) = int(lcm_atoi "case_clause_sort" x - lcm_atoi "case_clause_sort" y)
    List.sortWith pred x


// We would expect the melon sort to have put adjacent ones together ?
let case_elide lst =  
    let rec elide = function
        | [] -> []
        | (de, v, cmds) :: (de', v', cmds') :: ts when v=v' ->
           let _ = cassert(de=de', "de not the same")
           elide ((de, v, cmds@cmds')::ts)
        | other::ts -> other :: elide ts

    let sorted = case_clause_sort lst
    elide sorted


    
(* 
 * Build a two-handed IF then ELSE tree, with most frequent guard outermost.
 * NB: Compare this with ifshare in genericgates.sml which seems weaker since it does not tally literals!

 * Clearly we must maintain order of certain commands:
      1.  PLI calls must be kept in the same relative order as they were supplied
      2.  The blocking assign arcs are semantically continuous assigments (buffers) whose execution
          order is sorted and compile-time instead of by the EDS runtime: they must be sorted to
          evaluate before being used.
      
    The default {\tt ifshare} operation is that guards are tally counted
    and the most frequently used guard expressions are placed outermost
    in a nested tree of {\tt if} statements.

    The {\tt ifshare} flag turns off if-block generation in output code.
    If set to 'none' then every statement has its own 'if' statement around
    it.  If it is set to 'simple' then minimal processing is performed.

 *)


// (* Intermediate form between gen1 and gen2. *)
type d_t<'T>  =
    | D_T of xidx_t * d_t<'T> list * d_t<'T> list
    | D_TL of (xidx_t * d_t<'T> list) list
    | D_L of 'T
    | D_skip


// tallySet - How to write this cleanly ? I here have a whacky hybrid of mutable and immutable and I  cannot simply inherit Set since it is sealed.

type TallySet<'k_t when 'k_t : comparison>() = class
    //    inherit Map<'k_t, int>
    let m_map:((Map<'k_t, int>) ref) = ref (Map.empty)
   
    //member x.empty with get() = !m_map
    
    member x.add item =
        m_map := 
            match (!m_map).TryFind item with
                | None -> (!m_map).Add(item, 1)
                | Some n -> (!m_map).Add(item, 1 + n)
        x


    member x.getmap() = !m_map
    member x.Count0 with get() = Map.fold (fun cc k v -> cc + 1) 0 !m_map 
    member x.Count1 with get() = Map.fold (fun cc k v -> cc + v) 0 !m_map 
end



// Share if guards over commands, respecting order of commands that meet osensitivep.
//               
// Accepts list of (guards, cmd, reset) triples. 
// All commands are guarded by the conjunction of clock enables (cens).
let new_ifshare_poly ww (sharep_simple, finish, finish_with_conjunction, osensitivep, gen_ifl, skip_cmd, makecase, cens, resetnets) lst =
    let ww' = WN "new_ifshare_poly" ww
    let vd = -1 // Normally set this to -1 to supress reporting.

    let l' =
        // Annotate with a natural number for each item that needs conservative sorting (PLI order and ordering over fences must be retained)
        let rec add_ordering p = function
            | [] -> []
            | (gl, cmd, reset)::t -> 
                let a = osensitivep cmd
                ((if a then Some p else None), gl, cmd, reset) :: add_ordering (if a then p+1 else p) t

        let annotate_as_clauses (glst, cmd, reset) =
            let clauses = List.fold xi_clauses (Xist []) (cens @ glst)
            let sp grd = let (n, pol) = w_babs grd in (xb2nn n, pol, grd)
            (map sp (xi_valOf clauses), cmd, reset)


        add_ordering 0 (map annotate_as_clauses lst)

           //reportx 0 "new order" (fun (a, b, cmd)->(if a=None then "-" else i2s(valOf a)) + ":" + verilog_render_bev cmd) l'

    let ww = WF 2 "new_ifshare_poly" ww (sprintf "ordering annotations added %i at timestamp: %s" (length lst) (timestamp true))

    let guards = List.fold (fun c (on, gl, cmd, reset)-> List.fold (fun (cc:TallySet<int>) (nn, pol, grd) -> cc.add nn) c gl) (new TallySet<int>()) l'
            //List.fold (fun c (on, gl, cmd, reset)-> List.fold (fun (cc:TallySet<int>) (nn, pol, grd) -> cc.add nn) c gl) TallySet.empty l'


    // We want most frequently used guard listed first.
    let thresh = 2
    let get_sorted_guards () =
        let boosted =
            let booster cc body tally =
                let p0 = deblit body
                let mpc_pred s = strlen s >= 3 && s.[0..2] = "zpc"
                let boost = if memberp p0 resetnets || is_bresetnet p0 then 100000 elif mpc_pred (xbToStr p0) then 5 else 1
                (tally, boost, body)::cc
            Map.fold booster [] (guards.getmap())

        let ww = WF 2 "new_ifshare_poly" ww (sprintf "%i and %i guards formed at timestamp: %s" guards.Count0 guards.Count1 (timestamp true))
        let ww = WF 2 "new_ifshare_poly" ww (sprintf "%i guards sorted at timestamp: %s" (length boosted) (timestamp true))
        trimming_bubble_sort thresh boosted




    // For each cmd tag, compare it with each guard as to whether used or not -> quadratic cost!
    // Ideally eliminate guards that are seldom used once only from the main consideration : those below thresh. ?
    // A cmd may occur on both true and false hands of an IF, but that does not make it unconditional on that guard: for instance
    // consider A.~B + ~A.B where we cannot ignore A.
    let melon_classify g (on, lst, cmd, reset) =
        let P = ref false
        let M = ref false
        let rec ff cc = function
            | g1::tt ->
                if g1 = g then (P := true; ff cc tt)
                elif g1 = -g then (M := true; ff cc tt)
                else ff (g1::cc) tt
            | [] -> rev cc
        let lst' = ff [] lst
        ((!P, !M), on, lst', cmd, reset) 
             

    let rec melon_elide k sofar = function
        | [] -> if nullp sofar then [] else [ (k, rev sofar) ]
        | (kp, on, lst, cmd, reset)::t ->
            if kp=k then melon_elide k ((on, lst, cmd, reset)::sofar) t
            else
                let n = if sofar=[] then [] else [ (k, rev sofar) ]
                n @ melon_elide kp [] ((kp, on, lst, cmd, reset)::t)

    // melon_boat_shuffle: move items together so they share common k guards but do not let order-dependent items overtake each other.
    let rec shuf cmd0 pl ml ucl x =
        match x with
        | [] -> (rev cmd0) @ (rev pl) @ (rev ml) @ (rev ucl)

        | (kp, None, g, w, r)::t ->
            let item = hd x
            if kp = (true, false) then shuf cmd0 (item::pl) ml ucl t
            elif kp = (false, true) then shuf cmd0 pl (item::ml) ucl t
            elif kp = (false, false) then shuf cmd0 pl ml (item::ucl) t
            elif kp = (true, true) then shuf cmd0 (item::pl) (item::ml) ucl t
            else sf "melon shuf"

        | (kp, Some s, g, w, r)::t ->
            let item = hd x
            if cmd0=[] then shuf [item] pl ml ucl t
            elif f1o5(hd cmd0) = f1o5 item then shuf (item::cmd0) pl ml ucl t
            else (rev cmd0) @ shuf [item] pl ml ucl t

    let rec gen1 = function
        | (work, []) ->
            if vd>=5 then vprintln 5 "gen1 last leg"
            map (fun x->D_L(finish false x)) work  (*TODO simple_block_gen these - they could be separate still ? *)
        | ([], _) -> []
        | (work, (freq, boost, _)::_) when freq+boost < thresh ->
            //if vd>=5 then vprintln 5 (i2s(length work) + " gen1 frq < thresh")
            map (fun x->D_L(finish_with_conjunction x)) work 
        | (work, []) ->            
            //if vd>=5 then vprintln 5 (i2s(length work) + " no grds")
            map (fun x->D_L(finish_with_conjunction x)) work 

        | (work, (freq_, boost_, h)::t) -> 
            let g_melon_elide_disable = false       //A debug mode: normally this should be false.
            let g_noshuf = false                    //A debug mode: normally this should be false.
            if g_noshuf then vprintln 1 ("No melon shuffle!")
            let l2 = map (melon_classify h) work
            let l3 = if g_noshuf then l2 else shuf [] [] [] [] l2
            let l4 = if g_melon_elide_disable
                         then map (fun(kp, on, lst, cmd, reset) -> (kp, [(on, lst, cmd, reset)])) l3 
                         else melon_elide  (false, false) [] l3
//suspect this is needless recursion when there are no clauses - hence hyper runtime.
            let rec gengrp = function
                    | ((true, true), wa)::tt   -> D_T(h, gen1(wa, t), gen1(wa, t)) :: gengrp tt
                    | ((false, false), wa)::tt -> gen1(wa, t) @ gengrp tt
                    | ((true, false), wa)::((false, true), wb)::tt -> D_T(h, gen1(wa, t), gen1(wb, t)):: gengrp tt
                    | ((false, true), wa)::((true, false), wb)::tt -> D_T(-h, gen1(wa, t), gen1(wb, t)) :: gengrp tt
                    | ((true, false), wa)::tt -> D_T(h, gen1(wa, t), []) :: gengrp tt
                    | ((false, true), wa)::tt -> D_T(h, [], gen1(wa, t)) :: gengrp tt
                    | [] -> []
            let a0 = gengrp l4
            a0


    let iix_and a b = xb2nn (ix_and (deblit a) (deblit b)) 
    // Collapse where no IF structure is needed by forming conjunctions of guards:
    let rec squash = function
        | D_T(g0, [D_T(g1, [], s)], []) -> squash(D_T(iix_and g0 (-g1), s, []))
        | D_T(g0, [D_T(g1, s, [])], []) -> squash(D_T(iix_and g0 g1, s, []))
        | D_T(g0, [], [D_T(g1, [], s)]) -> squash(D_T(iix_and (-g0) (-g1), s, []))
        | D_T(g0, [], [D_T(g1, s, [])]) -> squash(D_T(iix_and (-g0) g1, s, []))
        | D_T(g, tt, ff) -> D_T(g, map squash tt, map squash ff)
        | other -> other

    // Convert to target lang, matching for 'case/switch' statements:
    let rec gen2 x =
        let rec csx = function 
            | [] -> []

            // Case accumulating clauses:
            | D_TL((g, tt)::tls) :: D_T(g', tt', []) :: ts when caseshare g' g -> csx(D_TL((g',tt')::(g,tt)::tls):: ts)

            | D_T(g, tt, []) :: D_T(g', tt', []) :: ts when caseshare g' g -> csx(D_TL([(g',tt');(g,tt)]) :: ts)

            // Leaf conversion clauses:
            | D_TL(lst) :: ts ->
                let dispatch_exps = map (caseshare_split) (rev lst)
                let dispatch_exp = f1o3(hd dispatch_exps)
                let elided = case_elide dispatch_exps
                let bodyfun (net, vale, cmds) = ([vale], csx cmds)
                (makecase (dispatch_exp, map bodyfun elided)) :: (csx ts)
                
            | D_T(g, tt, ff)::ts -> gen_ifl(g, gen2 tt, gen2 ff)  :: (csx ts)          
            | D_L(cmd)::ts       -> cmd :: (csx ts)
            | D_skip::ts         -> skip_cmd :: (csx ts)
        csx (map squash x)

    //----------------------
    // Block up version : simple conjunctions but sharing commands in a block: preserves order.
    let simple_block_gen l' =
        let guards = List.foldBack (fun (on, gl, cmd, reset) c-> singly_add (on, gl) c) l' []
        let gen_block lst = gen_ifl(1(*X_true*), lst, [])
        let zz (on, gl) (on', gl', cmd, reset)  cc = if on=on' && gl=gl' then cmd::cc else cc
        let bup (on, gl) = finish_with_conjunction(None, gl, gen_block(List.foldBack (zz (on, gl)) l' []), [])
        let ans = map bup guards
        ans


    let ww = WF 2 "new_ifshare_poly" ww (sprintf "simple=%A.  calling gen timestamp: %s" sharep_simple (timestamp true))
    let l2 =
        let l2f (nn, pol, g_) = (if pol then -nn else nn)
        map (fun (no, grds, cmd, resets) -> (no, map l2f grds, cmd, resets)) l'
    let ans =
        if sharep_simple then
            vprintln 2 "Output ifshare set to simple."
            simple_block_gen l2
        else
            let sorted_guards = get_sorted_guards()
            if vd>=4 then reportx 4 "ifshare sorted guards"  (fun (freq, boost, nn) -> sprintf "%s freq=%i boost=%i " (xbToStr (deblit nn)) freq boost)  sorted_guards 
            let v = gen1(l2, sorted_guards)
            let ww = WF 2 "new_ifshare_poly" ww (sprintf "gen1 complete timestamp:%s" (timestamp true))
            gen2 v
    let ww = WF 2 "new_ifshare_poly" ww (sprintf "done timestamp:%s" (timestamp true))
    ans




// eof

