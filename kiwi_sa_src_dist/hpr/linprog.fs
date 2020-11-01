(* $Id: linprog.fs,v 1.5 2011-06-06 10:03:02 djg11 Exp $ *)

module linprog

(*
 * The following is rather like a the 20 mark ML questions set for
 * Cambridge IA Computer Science students at the end of their first year.  However,
 * this question might take a bit longer than the normal 30 minutes allowed.  Typically,
 * a candidate takes four papers with five questions each, spread over a variety of subjects and topics.
 *
 * Question:
 *
 * The linrange ML datatype is used to represent a range of allowable values for an
 * integer variable.  A leading bool represents whether -infinity is allowable
 * and an ascending integer list represents change points in allowability.
 *
 *   Examples:  all_values = LIN(true, []) = X_true
 *               no_values = LIN(false, []) = X_false
 *               x >= 4    = LIN(false, [4])    ie four is allowable
 *               x < 4     = LIN(true, [4])     ie four is not allowable
 *               x==4      = LIN(false, [4, 5])
 *               x!=4      = LIN(true, [4, 5])
 *)



(* 
 * a) Write a function test that returns one of Yes, No or Possibly when 
 * asked whether a variable conforming to a given linrange is less than 
 * a specific integer.  The type of test is
 * 
 *     test : fn  linrange_t * int -> reply_t;
 *   
 * where    datatype reply_t = Yes | No | Possibly;
 * 
 * HINT: show the code that detects the No condition, assume similar code
 * exists for the Yes condition and then compose the results.
 *    
 * b) Create a function conj that performs the conjunction (logical ANDing) of a pair
 * of linranges.  Illustrate your answer using:
 *     conj : fn linrange_t * linrange_t -> linrange_t
 * val xge4 = LIN(false, [4]);
 * conj(LIN(false, [2, 4, 6, 8, 10]), xge4);
 *)


type linrange_t = LIN of bool * int list
type reply_t = Yes | No | Possibly

(* Answer to a *)

let valOf x = match x with (Some a) -> a | None -> failwith "valOf None"

(* 
 * Test whether a variable known to conform to a given linrange is less than v.
 * Do this by collecting three flags for possibly lower, eq, or greater.
 * (NB: This is a much more full answer than needed to just answer the question as posed.)
 *)
let linrange_itest =
    function
        | (LIN(s, lst), v) ->
             let rec scan (s, p, l, e, g) x =
                 match x with
                     | [] -> ((s && (p=None || valOf p < v)) || l, (s && (p=None || valOf p <= v)) || e, s || g)
                     | h::t -> 
                       if (h < v) then (scan (not s, Some h, true, e, g) t)
                       elif (h = v) then (scan (not s, Some h, s || l, (not s) || e, g) t)
                       else (* h > v *)
                         (scan (not s, Some h, l, (s && (p=None || valOf p <= v)) || e, (h > v+1) || g) t)
             scan (s, None, s, false, false) lst


let linrange_test_le(a, v) =
    match (linrange_itest(a, v)) with
                | (true, _, false)  -> Yes
                | (_,  true, false) -> Yes
                | (false, false, _) -> No
                | (_, _, _) -> Possibly

(* Bonus, extra functions: Once one of these is written, the others drop out quickly enough. *)

let linrange_test_lt(a, v) =
    match (linrange_itest(a, v)) with
                | (true, false, false) -> Yes
                | (false, _, _) -> No
                | (_, _, _) -> Possibly



let linrange_test_gt(a, v) =
    match (linrange_itest(a, v)) with
                  (false, false, true)  -> Yes
                | (_, _, false) -> No
                | (_, _, _) -> Possibly


let linrange_test_ge(a, v) =
    match (linrange_itest(a, v)) with
                  (false, _, true)  -> Yes
                | (false, true, _) -> Yes
                | (_, false, false) -> No
                | (_, _, _) -> Possibly


// Basis for an answer to b: Perform arbitrary diadic op f on a pair of linranges.
let linrange_op f (LIN(sl, valsl), LIN(sr, valsr)) =
    let sy = f (sl, sr)
    let rec merger = function
        |   (y, pl, [], pr, []) -> []   (* assert y==f(pl, pr) *)

        |   (y, pl, [], pr, hr::lstr) -> 
                let pr' = not pr
                let y' = f(pl, pr')
                let tailer = merger(y', pl, [], pr', lstr)
                if y=y' then tailer else hr::tailer

        |   (y, pl, hl::lstl, pr, []) -> 
                        let pl' = not pl
                        let y' = f(pl', pr)
                        let tailer = merger(y', pl', lstl, pr, [])
                        if y=y' then tailer else hl::tailer

        |   (y, pl, hl::lstl, pr, hr::lstr) -> 
                if hl < hr then
                        let pl' = not pl
                        let y' = f(pl', pr)
                        let tailer = merger(y', pl', lstl, pr, hr::lstr)
                        if y=y' then tailer else hl::tailer
                else if hl > hr then
                        let pr' = not pr
                        let y' = f(pl, pr')
                        let tailer = merger(y', pl, hl::lstl, pr', lstr)
                        if y=y' then tailer else hr::tailer
                else    let pl' = not pl (* hl==hr case *)
                        let pr' = not pr
                        let y' = f(pl', pr')
                        let tailer = merger(y', pl', lstl, pr', lstr)
                        if y=y' then tailer else hl::tailer

    let alst = merger(sy, sl, valsl, sr, valsr)
    LIN(sy, alst)


// Conjunction of a pair of linranges
let linrange_conj (a,b) = linrange_op (fun(a,b)->a && b) (a, b)

// Disjunction of a pair of linranges
let linrange_disj (a,b) = linrange_op (fun(a,b)->a || b) (a, b)

// To complement a linrange one just flips the bool.
let lin_complement = function
    | LIN(v, l) -> LIN(not v, l)




(* eof *)


