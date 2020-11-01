(*
 *  $Id: espresso.fs,v 1.17 2013-07-05 07:54:20 djg11 Exp $
 *
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
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
 *
 *
 * The core constructs of hprls are split between abstract.sml and genericgates.sml.
 * This file contains the main imperative and functional programming constructs.
 * It stores all expressions in a 'normalish form' in a memoising heap expression data structure and performs a multititude of peep-hole optimisations..
 * All sorts of other generic expression handling routines are in this file as well.
 *


Moscow ml bug fixes (to be incorporated)
 2. deqd of one-bit signals  -> xnor ?

 *
 *
 *)

module espresso


open yout
open hprls_hdr
open moscow
open linprog

// http://www.engr.sjsu.edu/caohuut/EE270/Documents/Espresso.pdf
// http://users.ece.utexas.edu/~adnan/syn-03/sop-4.pdf

type cube_t = int list


type cover_t = cube_t list
;


let cubeToStr x = "{" + sfold i2s x + "}"
let covToStr cubes =
    let rec ssfold f = function
                | [] -> ""
                | [item] -> f item
                | (h::t) -> (f h) + "; " + (ssfold f t)
    in "[" + ssfold cubeToStr cubes + "]"


// Implies: a,b  if cube a is a subset of b (or equal to b) then 1 else if b is a subset of a then -1 else 0


exception Adsorb


// If different in just one variable then can adsorb
let ee_implies (a:cube_t) (b:cube_t) =
    let rec imp v = function
        | ([], []) -> if    v=0  then 1 else v
        | ([], _)  -> if v <> -1 then 1 else 0
        | (_, [])  -> if v <> 1  then -1 else 0
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if a = b then imp v (al, bl)
               elif a = -b then // see how many differences we have had
                   if v = 0 && al=bl then raise Adsorb
                   else 0
               elif aa<bb then (if v = 1  then 0 else imp -1 (al, b::bl))
               elif aa>bb then (if v = -1 then 0 else imp 1  (a::al, bl))
               else sf "unreachable"
    in imp 0 (a, b)
;

// Cube disjunction:
// Cubes: the term that is the empty list is true.  The sum that is an empty list is false.

let rec conjoin = function // Remove the one difference in literals
    | ([], []) -> []
    | (a::al, b::bl) ->
        let aa = abs a
        let bb = abs b
        if a = b then a :: conjoin(al, bl)
        elif a = -b then (cassert(al=bl, "al=bl"); bl)
        else sf ("conjoin: not one difference ") //  + covToStr a0 + " cf " + covToStr b0)

let hack_cache = new System.Collections.Generic.Dictionary<cover_t * cube_t, cover_t>()

// Other optimisations: remove cubes completely covered by others (irredundant)
// Conjoin adjacent cubes ( a pair of cubes who differ in one variable) (repeat this until closure - does not repeat here todo).
let e_or (a0:cover_t) (b0:cover_t) =
    if memberp [] a0 || memberp [] b0 then [[]]
    elif false then lst_union a0 b0
    else
        //let _ = if length a0 > 100 || length b0 > 100 then vprintln 0  ("big e_or " + covToStr a0 + " cf " + covToStr b0)        
        let rec orrer4 q = function
            | [] -> [q]
            | p::tt ->
               try 
                  let v = ee_implies p q
                  let _ = vprintln //0 (cubeToStr q + " orrer4 implies " + sfold cubeToStr (p::tt) + sprintf " is %A" v)
                  in if v=0 then p :: (orrer4 q tt)
                      elif v=1 then p::tt
                      else q::tt
               with Adsorb ->
                  let _ = vprintln //0 (cubeToStr q + " orrer4 Adsorb " + sfold cubeToStr (p::tt))
                  in (conjoin(p, q)) :: tt

#if BLOWS_UP
        let orrer0 c q = List.fold orrer0 a0 b0
            let k = (c, q)
            let (fond, ov) = hack_cache.TryGetValue k
            if fond then ov
            else
                let v = (orrer4 q) c
                let _ = hack_cache.Add(k, v)
                in v 
        in List.fold orrer0 a0 b0
#endif
        let orrer0 c q = (orrer4 q) c
        in List.fold orrer0 a0 b0        
;

// e_orl: Form disjunction on a list of cubes (making a perhaps shorter cover). A heavy version that calls espresso is also available.
// Using this as the basis for e_or would be more expensive since e_or assumes no redundancy within each diadic arg.
let ee_orl (lst:cube_t list) =
    match lst with
    | [] -> []
    | [item] -> [item]
    | ha::ta ->
        let rec orrer1 q = function
            | [] -> [q]
            | p::tt ->
               try 
                   let v = ee_implies p q
                   in if v=0 then p :: (orrer1 q tt)
                      elif v=1 then p::tt
                      else q::tt
               with Adsorb -> (conjoin(p, q)) :: tt                      
        let orrer0 c q = orrer1 q c
        in List.fold orrer0 [ha] ta
;


// e_orl: Form disjunction on a list of covers (making a larger cover). A version that finds dont cares and calls espresso is also available (and preferable).
let rec e_orl__ (lst:cover_t list) =
    match lst with
    | [] -> []
    | [item] -> item
    | ha::hb::ta -> e_orl__ ((e_or ha hb)::ta)  
;

exception Invalid

exception Tautol


// Cube distance: number of null variables in the intersection of two cubes (0, 1, 2=more than 1).
// A null variable appears +ve in one cube and -ve in the other, meaning their intersection is empty.
let ee_dist (a:cube_t) (b:cube_t) =
    let rec dist c arg =
        if c >= 2 then 2
        else
            match arg with
                | ([], bb) -> c
                | (aa, []) -> c
                | (a::al, b::bl) ->
                    if a = b then dist (c) (al, bl)
                    elif a = -b then dist (c+1) (al, bl)
                    elif (abs a < abs b) then dist c (al, b::bl) else dist c (a::al, bl)
    in dist 0 (a, b)
;    

// 
// http://en.wikipedia.org/wiki/Consensus_theorem
// Consensus of two cubes is their conjunction with null variables deleted.
// The consensus is an additional cube that can be added to a disjunction that makes no difference except for
// hazard prevention in gate-level circuits.
// X.Y + X'.Z + Y.Z = X.Y + X'.Z
let ee_consensus (a:cube_t) (b:cube_t) =
    let rec consensus = function
        | ([], bb) -> bb
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            if a=b then a :: consensus (al, bl)
            elif a = -b then consensus (al, bl)
            elif (abs a < abs b) then a::consensus (al, b::bl) else b :: consensus (a::al, bl)
    in consensus (a, b)
;    

// Consensus of a cover and a cube is the disjunction of the consensus of that covers cubes. 
let e1_consensus (a:cover_t) (b:cube_t) =
    let jj cc acube =
        let r = ee_consensus acube b
        in e_or [r] cc  // use e1_or
    let ans = List.fold jj [] a 
    in ans
;

let rec ee_invalid arg =
    match arg with
    | [] -> false
    | [-1] -> true // A cube containing false is invalid.
    | [a] -> false    
    | a::b::tt ->
        if a = -1 || a = -b then true
        else ee_invalid (tl arg)
;

        
// Sort a cube into ascending order and kill it if invalid.
// Also, should ideally delete any members +1 == X_true.
let ee_sort = List.sortWith (fun a b -> (abs a) - (abs b))

let ee_sortc c lst =
    let lst' = ee_sort lst
    in if ee_invalid lst' then c else lst' :: c
;

// Form conjunction of a pair of cubes.
let rec ee_and_ c arg = 
    let rec and1 = function
        | ([], bb) -> (bb:cube_t)
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if aa=bb then (if a=b then a::and1(al, bl) else raise Invalid)
               elif aa<bb then a ::and1(al, b::bl)
               else b :: and1(a::al, bl)
    let ans = try and1 arg
              with Invalid -> c
    in ans

// Supercube: smallest cube containing a cover
let rec ee_supercube (arg:cover_t) = 
    let rec sc1 = function
        | ([], bb) -> (bb:cube_t)
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if aa=bb then (if a=b then a::sc1(al, bl) else sc1(al, bl))
               elif aa<bb then a ::sc1(al, b::bl)
               else b :: sc1(a::al, bl)
    let scc a b = sc1(a, b)
    in if arg = [] then sf "cannot make supercube of false"
       else List.fold scc (hd arg) (tl arg)

// Conjunction: maintain terms in ascending absolute value.  Any contrary pairs result in
// a false (invalid) conjunction (e.g. abc & -a) that must be deleted from its parent sum.
// Need cartesian product.
let e_and (a:cover_t) (b:cover_t) =
    let rec and1 = function
        | ([], bb) -> (bb:cube_t)
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if aa=bb then (if a=b then a::and1(al, bl) else raise Invalid)
               elif aa<bb then a ::and1(al, b::bl)
               else b :: and1(a::al, bl)
    let ror1 a1 c h =
        let ans = try e_or [and1(a1, h)] c // or e_orl at the end
                  with Invalid -> c
        in ans:cover_t
    let ror c a1 = List.fold (ror1 a1) c b
    in List.fold ror [] a
        

// www.ece.msstate.edu/courses/ece8013/presentations/14mvl.ppt
// http://docs.google.com/viewer?a=v&q=cache:5qjKR6iFcVYJ:www.ece.msstate.edu/courses/ece8013/presentations/14mvl.ppt+disjoint+sharp+cubes&hl=en&gl=uk&pid=bl&srcid=ADGEESjSiT7SxVvCCPQya1Gdzwqh8Dvbbv4OpXmUBZAwhcPPZuAiSY6Utf7uflnshlkIAVu0gFhiJg6IlkNN0qglj6zGrleMuM6-r0ltmIX0GdOUgxi2sQYf9mPmwpQEW-PeLZOq9U-d&sig=AHIEtbRF9XTPmlEIHFy8bE2kLfaDHooySA

// Sharp and disjoint sharp operation A#B is A/\B' which is just B' when A is a tautology.
// Non-commutative.  Sharp of two cubes returns a cover.
// For each variable b_i in b add a cube to the result which is a copy of a with var i = (a_i # b_i).
// But delete any invalid cubes so formed.
// Note: x#y for two leaf variables is just (x . y')
// Example A=[2;3] and B=[1;2;3;4] gives A#B=[[-1;2;3]; [2;3;-4]] : i.e. it is precisely A /\ B'. 
let ee_sharp (a:cube_t) (b:cube_t) =
    let d = ee_dist a b
    //let _ = vprintln 0 ("ee_sharp: Distance between " + cubeToStr a + " and " + cubeToStr b  + " is " + i2s d)
    in if d > 0 then [a]
       else
           let kk cc bv =
               let bbb = abs bv
               let rec hh sofar = function
                   | [] -> (rev sofar) :: cc
                   | h::t when h = -bv   -> ((rev sofar) @ (h :: t))::cc // keep +ve h since this is in a.
                   | h::t when h = bv  -> cc // invalid term, ignore sofar
                   | h::t when abs h < bbb -> hh (h::sofar) t
                   | other -> ((rev sofar) @ other)::cc
               in hh [] a
           in List.fold kk [] b
;


// Disjoint sharp of two cubes:
// Non-commutative.  Disjoint sharp of two cubes also returns a cover.
// For each variable b_i in b add a cube to the result which is a copy of a with var i = (a_i # b_i) and vars (0..i-1) = a_i . b_i.
// But delete any invalid cubes so formed.
let ee_dj_sharp a b =
    let d = ee_dist a b // not dj yet <<==================== TODO
    in if d > 0 then [a]
       else
           let kk cc bv =
               let bbb = abs bv
               let rec hh sofar = function
                   | [] -> (rev sofar) :: cc
                   | h::t when h = -bv   -> ((rev sofar) @ (h :: t))::cc // keep +ve h since this is in a.
                   | h::t when h = bv  -> cc // invalid term, ignore sofar
                   | h::t when abs h < bbb -> hh (h::sofar) t
                   | other -> ((rev sofar) @ other)::cc
               in hh [] a
           in List.fold kk [] b
;

// The cover sharp operator fully distributes over cubes in its lhs or rhs operands as an association of cube sharps.
let e1_dj_sharp (a:cover_t) (b:cube_t) =
    let aux c av = e_or (ee_dj_sharp av b) c
    let ans = List.fold aux [] a 
    ans

let e1_sharp (a:cover_t) (b:cube_t) =
    let aux c av = e_or (ee_sharp av b) c
    let ans = List.fold aux [] a 
    ans


let e3_sharp (a:cover_t) (b:cover_t) =
    let ans = List.fold e1_sharp a b
    ans

let e3_dj_sharp (a:cover_t) (b:cover_t) =
    let ans = List.fold e1_dj_sharp a b
    ans
    


// The cover sharp operator fully distributes over lhs operands and the order of the rhs operands make no difference.
let e2_sharp (a:cube_t) (b:cover_t) =
    if b=[] then [a] else e3_sharp [a] b


// The disjoint cover sharp operator fully distributes over its lhs operands and the order of the rhs operands make no difference to the semantic of the result but alters the cover structure.
let e2_dj_sharp (a:cube_t) (b:cover_t) =
    if b=[] then [a] else e3_dj_sharp [a] b



// Boolean divide :  Give a quotient which when ANDED with the divisor gives the dividend: e.g. (a.b + b.!c)/b = a + !c
// Boolean division can have a remainder  : e.g. (a,b+c)/b = a remainder c.  
// Boolean signed division 1:    If you suppose a.b/!a = !b you are wrong. Multiplying back you get !a.!b not the correct answer a.b.
// Boolean signed division 2:    There is no sensible answer for  !a.b/a.
// Straightforward boolean division has different behaviour from Shannon cofactor division when the dividend is not a multiple of the divisor.

// Boolean division by zero errors:  Dividing by 'false' is tantamount to dividing by the empty cover and we do not support divide by cover here.
// Dividing by 'true' is the identity operator and returns the numerator.    

//  bool_divide_cube: Both num and den are cubes.
let bool_divide_cube major_wont_go wont_go num den =
    let rec divcube = function
        | ([], _)       -> []
        | (num, [])     -> num        
        | (h::t, d::dt) when abs h < abs d -> h :: divcube (t, d::dt)
        | (h::t, d::dt) when abs h > abs d -> (wont_go := true; [])
        | (h::t, d::dt) when h =  d -> divcube(t, dt)
        | (h::t, d::dt) when h = -d -> (major_wont_go := true; wont_go := true; []) // It's a major wont go when the denominator has a complementary literal!
        | _ -> sf "impossible"
    let ans = divcube (num, den)
    if !wont_go then None else Some ans
   
// Num is a cover, den is a cube.
// Return Some(result) if an exact multiple, or None.
let bool_exact_divide_cover num den =    
    let wont_go = ref false
    let rec divcover = function
        | []       -> []
        | cube :: tt -> 
            match bool_divide_cube wont_go wont_go cube den with
               | _ when !wont_go -> []
               | Some v -> v :: divcover tt
    let ans = divcover num
    if !wont_go then None else Some ans

// Num is a cover, den is a cube.
// Return Some(quotient, remainder) or None.
// We get a 'wont_go' None result if the denominator contains a complemented term w.r.t. the numerator.
let bool_divide_cover num den =    
    let remainder = ref []
    let major_wont_go = ref false
    let rec divcover = function
        | []       -> []
        | cube :: tt ->
            let wont_go = ref false            
            match bool_divide_cube wont_go major_wont_go cube den with
               | _ when !wont_go -> (mutadd remainder cube; divcover tt)
                   
               | Some v -> v :: divcover tt
    let ans = divcover num
    if !major_wont_go then None else Some(ans, !remainder)

    
//
// Divide a cover (list of cubes) by v.  See shannon_not for use pattern.
// Cubes that do not include v are retained unchanged.
// Cubes that include v negated are discarded entirely.
// Division compared with Shannon cofactors: an expression can always be 'divided' by an expression into a pair of expressions that when muxed by that give the original. They will be simpler expressions if the divisor appears in the original.      
let shannon_cofactor v cubes =
    let va = abs v
    let add c = function
        | [] -> [[]]
        | other -> if c = [[]] then c else other::c
    // c is a disjunctive accumulator for folding over a list of cubes.
    let rec sc sofar c = function
        | h::tt when abs h < va -> sc (h::sofar) c tt 

        | h::tt when h = v  -> add c ((rev sofar) @ tt) 
        | h::tt when h = -v -> c // v here occurs negated so no cube is contributed to the result.
        | h::tt when abs h > va -> add c ((rev sofar) @ (h::tt))  
        | [] -> add c (rev sofar)
    List.fold (sc []) [] cubes


// Restrict:  c1p or "c/|p" is the cofactor of p w.r.t. c. 
// Example  c=[1;2;3] p=[1] c1p =[2;3] 
// A form of algebraic division c/p - implemented as a subtraction of literals c-p.
// Restrict is also 'simplify assuming'.  If c does not depend on p it is unaltered.
// If c holds when p does not, ee_restrict aborts.
let rec ee_restrict (c:cube_t) (p:cube_t) =
    match (c, p) with
    | ([], pl) -> []
    | (cl, []) -> cl
    | (c::cl, p::pl) ->
        let pp = abs p
        let cc = abs c
        if c=p then ee_restrict cl pl
        elif c = -p then sf "restrict: distance greater than zero"
        elif cc<pp then c::ee_restrict (cl) (p::pl)
        elif cc>pp then ee_restrict (c::cl) (pl)
        else sf "unreachable"

// cf a don't care set: restrict c1p means fill cube p with don't cares and expand c.

// Restrict a cover to a cube: those parts of the cover that are outside the cube
// are discarded and those parts that overlap or are inside the cube are retained, simplified accordingly.
let rec e_restrict (c:cover_t) (p:cube_t) =
    let rec ee_restrict1 cd c1 = 
        let rec res1 = function
            | ([], pl) -> []
            | (cl, []) -> cl
            | (c::cl, p::pl) ->
                let pp = abs p
                let cc = abs c
                in if c=p then res1(cl, pl)
                   elif c = -p then raise Invalid
                   elif cc<pp then c::res1 (cl, p::pl)
                   elif cc>pp then res1 (c::cl, pl)
                   else sf "unreachable"
        in try res1 (c1, p) :: cd
           with Invalid _ -> cd 
    let ans = List.fold ee_restrict1 [] c
    ans

// Simlpify cover c assuming cover p:  c1pp is a disjunction: c1p0 | c1p2 | ... c1p3
// There is no rule  c1(p+q) === c1p + c1q
// Or                c1(p+q) === (c1p)1q
// 
let rec e1_simplif (c:cover_t) (pp:cover_t) =
    match pp with
        | [[]] -> c
        | [item] -> e_restrict c item
        | _ ->
            let _ = vprintln 1 ("+++e1_simplif: dont know how to do this " + covToStr pp)
            c
            



// A pair of cubes have null cofactor if they are one or more apart (i.e. disagree in the polarity of at least one leaf).
// If distance is zero then return restrict  c1p.   
// ee_cofactor is a version of ee_restrict that returns false if c only holds outside p.
let ee_cofactor (p:cube_t) (c:cube_t) =
    let d = ee_dist p c        
    if d>0 then []
    else ee_restrict c p



//  "The cofactor of a cover against a cube c is a cover that is the disjunction (list) of the cofactors of each cube in the cover against c."
let e1_cofactor (a:cover_t) (c:cube_t) =
    let aux cc av = e_or [(ee_cofactor av c)] cc // use e1_or ?
    let ans = List.fold aux [] a 
    ans
   

        
let cube_support c cube =
    let rec sup = function
        | ([], bb) -> bb
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if aa=bb then aa::sup(al, bl)
               elif aa<bb then aa :: sup(al, b::bl)
               else bb :: sup(a::al, bl)
    sup (c, cube)

   //
   // Test data is [1, 2], [3]
   //  Decompos on 1 ->  [2][3]  and [3]
   //
   //
   //
   //
   //

// A more-efficient complement operation
let shannon_not (cubes:cover_t) =
    let nn_item item = map (fun x -> [-x]) item
    let rec nn (arg:cover_t) =
        match arg with
        | [] -> [[]]
        | []::_ -> []
        | [item] -> nn_item item
        | lst ->
            let k = hd(hd lst) // todo: better pivot selector needed.
            let coft = (shannon_cofactor k lst)
            let coff = (shannon_cofactor -k lst)
            //let _ = vprintln 0 ("  cof with " + i2s k + " cov=" + covToStr coft)
            //let _ = vprintln 0 ("  cof with " + i2s -k + " cov=" + covToStr coff)
            let coft = nn (coft)
            let coff = nn (coff)
            //let _ = vprintln 0 ("  neg cof with " + i2s k + " cov=" + covToStr coft)
            //let _ = vprintln 0 ("  neg cof with " + i2s -k + " cov=" + covToStr coff)

            let ans = e_or (e_and [[k]] coft) (e_and [[-k]] coff)  // todo: nice mux function needed.    
            //let ans = e_mux k coft coff
            //let _ = vprintln 0 ("   ans is " + covToStr ans)
            ans

    nn cubes


//
let rec e_tautology(f:cover_t) =
    if f=[] then false
    // if (f == {-...-}) then true // cube with all ‘-’
    elif hd f = [] then true
    else
        let select_variable f = hd(hd f)  // todo: binate-based pivot would be better
        let xi = select_variable f
        let c0 = shannon_cofactor -xi f
        if not (e_tautology c0) then false
        else
            let c1 = shannon_cofactor xi f
            e_tautology c1


// cube_is_covered predicate.
// Example 1: a=[[3]] c=[1;2;3]  a1c = [] a tautology, so a does cover c.
// Example 2: a=[[1;2]; [-1; 3]] c = [2;3], cofactor of c w.r.t a is [[1]; [-1]], a tautology, so a does cover c.
//             
let cube_is_covered (t:cover_t) (c:cube_t) = e_tautology(e1_cofactor t c)



// invert the support of a cover.
let shannon_flipin (cubes:cover_t) =    
    let support = List.fold cube_support [] cubes
    let sorted = support // for now
    //let _ = vprintln 0 ("sorted order is " + sfold i2s sorted)

    let rec step (cubes:cover_t) k =
        let coft = shannon_cofactor k cubes
        let coff = shannon_cofactor -k cubes
        let _ = vprintln 0 ("  cof with " + i2s k + " cov=" + covToStr coft + " post-and gives " + covToStr (e_and [[-k]] coft))
        let _ = vprintln 0 ("  cof with " + i2s -k + " cov=" + covToStr coff + " post-and gives " + covToStr (e_and [[k]] coff))
        let ans = e_or (e_and [[-k]] coft) (e_and [[k]] coff) 
        //let _ = vprintln 0 ("   ans is " + covToStr ans)
        in ans

    let ans = List.fold step cubes sorted 
    in ans

let e_not = shannon_not

// Input: Cubelist F that covers function f
// Output: Cubelist that covers complement of function f, i.e. cover of f'
// For example: !(1.2 + 2.3) = !(1.2) . ! (2.3)
//                           = (!1+!2) . (!2+!3)
//                           = (!1.!2) + (!1.!3) +  (!2.!2) + (!2.!3)  
//                           = (!1.!2) + (!1.!3) +  (!2) + (!2.!3)  
let olde_not (cube:cover_t) =
    let rec aand1 = function
        | ([], bb) -> bb
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let aa = abs a
            let bb = abs b
            in if aa=bb then (if a=b then a::aand1(al, bl) else raise Invalid)
               elif aa<bb then a ::aand1(al, b::bl)
               else b :: aand1(a::al, bl)

    let rec rd = function
        | [] -> sf "[] xprod"
        | [item] -> item
        | (h1::h2::t) -> 
          let k2 x y c =
              try aand1(x, y) :: c
              with Invalid -> c
          let k1 x c  = List.foldBack (k2 x) h2 c
          let p = List.foldBack k1 h1 []
          in rd(p::t)
    let flipped = map (fun x -> map (fun y -> [-y]) x) cube
    rd flipped





//Improvements
//• Variable ordering:
//     – pick variable that minimizes the two sub-cases (“-”s get replicated into both cases)
//• Quick decision at leaf:
//    – return TRUE if C contains at least one complete “-” cube among others (case 1)
//    – return FALSE if number of minterms in onset is < 2n (case 2)
//    – return FALSE if C contains same literal in every cube (case 3)


// Check whether two expressions are equivalent.
let rec e_equiv (f:cover_t) (g:cover_t) =
    let a = e_or (e_and (e_not f) (e_not g)) (e_and f g) // The XNOR function is the same as A==B  (A deqd B).
    let ans = e_tautology a
    (ans:bool)

// Check whether two expressions are equivalent outside of a dont-care space.
let rec e_equiv_dc (dc:cover_t) (f:cover_t) (g:cover_t) =
    let a = e_or (e_and (e_not f) (e_not g)) (e_and f g)
    e_tautology (e_or dc a)






    
let e_reduce(f_on:cover_t, f_dc:cover_t) =
//  Espresso Operation: Reduce
//  Input:   Cubelist F that covers function f
//  Output:  Cubelist that covers f with each implicant reduced - maybe not prince- so that no implicants overlap on any minterm.
//  Strategy:  For (each cube C in F, now from heavy to light) {
//                Intersect C with reset of the cover F
//                Remove from C the minterms covered elsewhere;
//                Find the biggest cube that covers this: the supercupe of the essential points in it.
//                Replace C with this reduced cube. }
// Espresso reduce.c says "The reduction of a cube is the smallest cube contained in the cube
//    which can replace the cube in the original cover without changing
//    the cover.  This is equivalent to the supercube of all of the essential points in the cube."
//
//
    let sorted = f_on // for now
    let rec red sofar cubes =
        //if sofar = [[]] then sofar
        match cubes with
        | [] -> sofar
        
        | item::tt ->
            let overlap = e_and [item] (sofar @ tt)
            let item' = e2_sharp item overlap // Dont need disjoint sharp if then going to make supercube.
            // let _ = vprintln 0 ("reduce:  item=" + cubeToStr item + "; sofar=" + sfold cubeToStr sofar + "; overlap=" + covToStr overlap + " item'=" + covToStr item' + (if item'=[] then "" else  "; supercube=" + cubeToStr(ee_supercube item')))
            in if item' = [[]] then [[]] // nothing else matters!
               elif item' = [] then red sofar tt
               else red (ee_supercube item' :: sofar) tt
    red [] sorted // f_dc is ignored!



let e_expand (a, f_off) =  
//    Input:  Cubelist F that covers function f
//    Output: Cubelist that covers f with each implicant as big as possible, i.e. prime.
//    Strategy: Assign each cube Ci a priority weight wi
//              For (each cube Ci in priority order from small to large)
//              {
//                Determine which vars in cube Ci we can remove;
//                Remove those vars to make cube a bigger, expanded cube.
//              }
   let report = vprintln 3
   let expand() =
       if a=[] then a
       elif f_off=[] then raise Tautol  // Not stopped by anything: expands to tautology.
       else
       let sorted = a // for now - sorting optimisiation missing.
       let expand cover = // Intersect each variable with each cover in the off set
          if cover=[] then raise Tautol // if one cube is [] then no others needed infact.
          else
          let _ = vprintln //(*deb*) 0 ("e_expand f=" + covToStr a)
          let _ = vprintln //(*deb*) 0 ("e_expand off=" + covToStr f_off)      
          let scanoff v offcover =
              let vm = -v
              let va = abs v
              let rec scn = function
                  | [] -> 0
                  | h::t when abs h < va -> scn t
                  | h::t when abs h > va -> 0
                  | h::t -> if h<>v then 1 else 0
              in scn offcover
          let gen_block_row v = (v, map (scanoff v) f_off) // One Col per cube in the off set.
          let block_rows = map gen_block_row cover // One row per literal in current cube.
          let target = length (snd (hd block_rows))
          let tally (id, row) = (id, row, List.fold (fun c n -> c+n) 0 row)
          let tallied = map (tally) block_rows 
          let ascending = List.sortWith (fun (id, _, a) (_, _, b) -> -a+b) tallied // decending tally (process fullest first).
          let _ = reportx //(*deb*) 0 "blockingtab" (fun (id, row, cnt) -> "id=" + i2s id + "; row=" + sfold i2s row + "; cnt=" + i2s cnt) ascending
          let rec grabb (sofar, c) lst =
              if c=target then lst // If we have them all then just return old list.
              else
                  let rec splice = function // splice in a new row (i.e. keep a literal of the old cube).
    //TODO if new row adds nothing, then dont keep it!
                      | (1::olda, _::newa, sofar, c) -> splice(olda, newa, 1::sofar, c)
                      | (0::olda, 0::newa, sofar, c) -> splice(olda, newa, 0::sofar, c)
                      | (0::olda, 1::newa, sofar, c) -> splice(olda, newa, 1::sofar, c+1)
                      | ([],      [],      sofar, c) -> (rev sofar, c)
                  if lst=[] then sf "no further stoplist rows"
                  else
                      let _ = report //(*deb*)  ("Blocking tab step: pre=" + sfold i2s sofar + ": cnt=" + i2s c)
                      let (sofar, c) = splice(sofar, f2o3 (hd lst), [], c)
                      let _ = report //(*deb*)  ("Blocking tab step: post=" + sfold i2s sofar + ": cnt=" + i2s c)
                      grabb (sofar, c) (tl lst)
          let raised = grabb (f2o3 (hd ascending), f3o3 (hd ascending)) (tl ascending) // Take first one always (otherwise offset was empty),
          if raised=[] then
                let _ = report //(*deb*) "Raised no literals"
                cover
          else
                let raised = map f1o3 raised
                let _ = report //(*deb*) ("Raising following literals " + sfold i2s raised)
                let cover' = List.filter (fun x -> not(memberp x raised)) cover
                if cover' = [] then raise Tautol else cover'

       map expand sorted // expand each cube in turn.
   let expanded = 
       try expand()
       with
           | Tautol ->
               let _ = report //(*deb*) "Expanded to tautology"               
               in [[]]
           | Invalid ->
               let _ = report //(*deb*) "Expanded to invalid (false) result."               
               in []
   expanded




// An essential term is a prime implicant that covers essential minterms. Essential minterms can only be covered in one way.
// Essential terms will be present in any good cover so find them as a first priority.

// "A term c is essential iff    consensus((F u D) # c, c) u D    does not contain c."
// Miller R Switching Theory Vol 1, 1965: Given a prime cube, c_i, if the consensus of c_i with all other on cubes and dc cubes completely covers c_i, then c_i is not essential, otherwise it is.


// Regarding cubes "completely covers" means "is implied by" or we can use the restrict operator.
let e_essentials(f_x, f_dc) = // TODO : do we need each element of x to be prime for correctness ?
    let conc = e_or f_x f_dc
    let essential c =
        let h1 = e1_consensus (e1_dj_sharp conc c) c
        let h2 = e_or h1 f_dc
        // If h2 covers c then it is not essential.
        let ans = cube_is_covered h2 c // (ee_implies h2 c) = 1
        //let _ = report ("Essential cube? " + cubeToStr c + " ans=" + boolToStr ans)
        in ans
    let ans = List.filter essential f_x
    ans

// 
// Ideally we find irredundant by a nice covering matrix algorithm that looks at all cubes at once...
// Naively we consider each cube in turn and delete any that only affects the dc set.
//
let e_irredundant(x, f_dc) =
    if length x < 2 then x
    else
        let rec scan sofar = function
            | h::t ->
                let remainder = sofar @ t
                let redundant = e_equiv_dc f_dc remainder x
                in if redundant then scan sofar t else scan (h::sofar) t
            | [] -> sofar
        scan [] x

//
//
//
let e_espresso (effort_) (f_on:cover_t, f_dc:cover_t) =
    let report = vprintln 3
    let _ = report //DD ("Input function is " + covToStr f_on)
    let _ = report //DD ("Input dc is " + covToStr f_dc)
    let _ = report //DD ("Their conjunction is " + covToStr(e_or f_on f_dc))
    let fFoff = e_not (e_or f_on f_dc)   // Get cover of OFF-set for later use.
    let _ = report //DD ("Cover of off set is " + covToStr fFoff)
    let fF = e_expand (f_on, fFoff)      // Get first cube list cover of fF (not on the OFF set).
    let _ = report //DD ("Expanded input is " + covToStr fF)
    let fF = e_irredundant (fF, f_dc)    // Get rid of redundant cubes.
    let _ = report //DD ("Irredundant-trimmed expanded input is " + covToStr fF)    
    let eE = e_essentials (fF, f_dc)     // Remove essentials - no need to look.
    let _ = report //DD ("Essential cubes are " + covToStr eE)    
    let fF = e3_dj_sharp fF eE           // for a cover of essentials. // or sharp subtract
    let _ = report //DD ("Input sans essential implicants is " + covToStr fF)    
    let f_dc = e_or f_dc  eE             // Place essentials temporarily in DC set.
    let _ = report //DD ("Temporary don't care containing essentials is " + covToStr f_dc)    
    let e_cost f = length f
    let oldcost = e_cost fF
    let rec iterate oldcost fF0 =
        let _ = report //DD ("Start iteration on " + covToStr fF0 + " cost=" + i2s oldcost)
        let fF = e_reduce (fF0, f_dc)       // Shrink this cover.
        // Surely this check should be an e_equiv_dc ?  
        //let  chk1 = e_equiv fF0 fF
        //let _ = assertf chk1 (fun () -> "espresso reduce failed " + covToStr fF0 + " became " + covToStr fF + " (with dontcare=" + boolToStr(e_equiv_dc f_dc fF0 fF) + ")")
        let _ = report //DD ("Shrunk is " + covToStr fF)
        let fF = e_expand (fF, fFoff)      // Regrow, some cubes may improve.
        let _ = report //DD ("Re-expanded is " + covToStr fF)
        let fF = e_irredundant (fF, f_dc)  // Get rid of redundant cubes.
        let new_cost = e_cost fF
        let _ = report //DD ("Trimmed is " + covToStr fF + " cost=" + i2s new_cost)

        if new_cost >= oldcost then fF0 // Loop termination rule.
        else iterate new_cost fF
    let fF = iterate oldcost fF
    let ans = e_or fF eE  // Put back essentials.
    let _ = report //DD ("Finished, essentials re-inserted is " + covToStr ans)
    ans

    
let run_e_tester data =
    let report z = 0
    let astest msg g =
        let _ = report (msg + " " + boolToStr g)
        in if g then () else sf "as test failed"


    let _ = report //DD("Test data is " + covToStr data)
    let _ = report //DD("Inv data is " + covToStr (e_not data))
    let _ = report //DD("eqiv: data == data " + boolToStr(e_equiv data data))
    let _ = astest "eqiv: data == rev data " (e_equiv data (rev data))  

    let data1 = e_espresso 0 (data, [])
    let _ = report //DD("Minimised data is " + covToStr data1)
    let _ = astest "eqiv: data == data " (e_equiv data1 data)

    let _ = report //DD("Test cof 2: " + covToStr (shannon_cofactor 2 data))
    let _ = report //DD("Test cof 2: " + covToStr (shannon_cofactor -2 data))                 
    let flip c d =
        let c' = e_not c
        let _ = report //DD("Negated is " + covToStr c')
        let _ = report //DD("eqiv: Vale == data " + boolToStr(e_equiv data c'))
        in c'
    let _ = List.fold flip data [0..2]
    ()

let run_cube_tester f cube =
    let vd = 0
    vprintln vd ("cube test " + cubeToStr cube +  " --> " + f(cube))
    

let run_e_test() =
    let _ = vprintln 0 ("[2] implies [2] "   + i2s (ee_implies [2] [2]))
    let _ = vprintln 0 ("[2] implies [2;3] " + i2s (ee_implies [2] [2;3]))
    let _ = vprintln 0 ("[2;4] implies [2] " + i2s (ee_implies [2;4] [2]))

    let _ = run_cube_tester (fun a -> " sorted " + cubeToStr(ee_sort a)) [199; -4; 44; -45]
    let _ = run_cube_tester (fun a -> " invalid " + covToStr(ee_sortc [] a)) [199; -4; 44; -45]
    let _ = run_cube_tester (fun a -> " invalid " + covToStr(ee_sortc [] a)) [199; 45; 44; -45]      

    let _ = run_e_tester  [[22]]
    let _ = run_e_tester  [[-12]]
    let _ = run_e_tester  [[-22]; [23]]
    let _ = run_e_tester  [[-32]; [-32]]
    let _ = run_e_tester  [[1;2]; [-1; 3]; [2;3]]    
    let _ = run_e_tester  [[1;2]; [1;-2;3]; [4;5;6]]
    ()

    
// eof

