(*
 *  $Id: moscow.fs $
 *
 * CBG Orangepath.
 *
 * HPR L/S Formal Codesign System.
 * (C) 2003-14 DJ Greaves. All rights reserved.
 *
 *
 * Contains:
 * 
 *
 *
 *)

module moscow

open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Numerics
open linepoint_hdr
open yout

let f1o3(a, b, c) = a
let f2o3(a, b, c) = b
let f3o3(a, b, c) = c

let f1o4(a, b, c, d) = a
let f2o4(a, b, c, d) = b
let f3o4(a, b, c, d) = c
let f4o4(a, b, c, d) = d

let f1o5(a, b, c, d, e) = a
let f2o5(a, b, c, d, e) = b
let f3o5(a, b, c, d, e) = c
let f4o5(a, b, c, d, e) = d
let f5o5(a, b, c, d, e) = e

let f1o6(a, b, c, d, e, f) = a
let f2o6(a, b, c, d, e, f) = b
let f3o6(a, b, c, d, e, f) = c
let f4o6(a, b, c, d, e, f) = d
let f5o6(a, b, c, d, e, f) = e
let f6o6(a, b, c, d, e, f) = f

let maxint() = 2147483647
let minint() = -2147483648


// Since mono fsharp also runs out of stack with the builtin equality, we avoide writing "x=None" and use nonep instead.
let nonep = function
    | None  -> true
    | _     -> false

let not_nonep = function
    | None  -> false
    | _     -> true


let optToStr x = if nonep x then "None" else "Some(...)"
let print (x:string) = System.Console.WriteLine(x)
let valOf x = match x with (Some a) -> a | None -> failwith "valOf None"
let valOf_or_nil x = match x with (Some a) -> a | None -> []
let valOf_or_zero x = match x with (Some a) -> a | None -> 0
let optionToStr ff = function
    | None -> "None"
    | Some x -> ff x
    
let valOf_or_ns = function
   | None   -> "" 
   | Some l -> l

let valOf_or_blank = function
    | None    -> "BLANK"
    | Some ss -> ss

let valOf_or_null x = match x with (Some a) -> [a] | None -> []
let valOf_or_fail m x = match x with (Some a) -> a | None -> sf("valOf_or_fail " + m)
let valOf_or_failf m x = match x with (Some a) -> a | None -> sf("valOf_or_failf " + m())
let grab ov nv = if ov=None then Some nv elif nv=valOf ov then ov else sf "grab changed value"

let optapp ff = function
    | None -> None
    | Some x -> Some(ff x)

            
let hd = List.head
let tl = List.tail
let rev = List.rev
let boolToStr x = if x then "true" else "false"
let booloToStr x = if x=None then "None" else "Some(" + boolToStr(valOf x) + ")"
let osToStr x = if x=None then "None" else valOf x
let oiToStr x = if x=None then "None" else i2s(valOf x)
let oi64ToStr x = if x=None then "None" else i2s64(valOf x)
let singly_add c list = if List.exists (fun elem -> elem = c) list then list else c::list

// This creates quite a lot of garbage:
let memberp__ c list = List.exists (fun elem -> elem = c) list

let rec memberp needle = function
    | [] -> false
    | h::tt -> h=needle || memberp needle tt


let rec member_twicep needle = function
    | []    -> false
    | h::tt -> (needle=h && memberp needle tt) || member_twicep needle tt
    
let rec list_uniq = function
    | []   -> []
    | [a]  -> [a]
    | a::b::tt  when a=b -> list_uniq (b::tt)
    | a::b::tt           -> a :: list_uniq (b::tt)    


let rec list_once lst =
    match lst with
        | [] -> []
        | h::t -> if memberp h t then list_once t else h :: (list_once t)



let nullp = function
    | [] -> true
    | _ -> false

let not_nullp = function
    | [] -> false
    | _ -> true

let rec lst_subtract a b = if nullp a then [] else if memberp (hd a) b then lst_subtract (tl a) b else (hd a)::(lst_subtract (tl a) b)

let rec list_subtract(a, b)=if nullp a then [] else if memberp (hd a) b then list_subtract(tl a, b) else (hd a)::(list_subtract(tl a, b))


let groom2 fp lst = // First argument returned is the list for which the predicate holds.
    let rec g2(d, t, f) = if nullp d then (rev t, rev f) 
                          else if fp(hd d) then g2(tl d, (hd d)::t, f) 
                                           else g2(tl d, t, (hd d)::f)
    (g2 (lst, [], []))

let mutdel lst v = 
    let rec del lst = if nullp lst then [] else if hd lst = v then tl lst else (hd lst)::(del lst)
    (lst := del (!lst))

let mutdelkey lst v = 
    let rec del = function
        | [] -> []
        | (a,b)::tt when a=v -> tt
        | h::tt -> h :: del tt
    (lst := del !lst)

let lst_union b cc =
    match cc with
        | [] -> b
        | cc ->
            let rec lunion cc = function
                | [] -> cc
                | h::tt -> if memberp h cc then lunion cc tt else h::(lunion cc tt)
            lunion cc b

let rec list_Union = function
    | [] -> []
    | h::t -> lst_union h (list_Union t)

let rec list_take v = function
    | [] -> []
    | h::tt when h=v -> tt
    | h::tt -> h :: (list_take v tt)


    
let list_oncef f lst =
    let mem item = List.exists (fun elem -> f(item, elem))
    let rec lo = function
        | [] -> []
        | (h::t) -> if mem h t then lo t else h :: (lo t)
    lo lst


let singly_addf f c list =
    let mem item = List.exists (fun elem -> f(item, elem))
    if mem c list then list else c::list


let list_subtractf (f:'a * 'a -> bool) (a, b) =
    let mem item = List.exists (fun elem -> f(item, elem))
    let rec sc = function
        | [] -> []
        | h::t -> if mem h b then sc t else h :: (sc t)
    sc a


let rec list_unionf f (l, r) =
    let mem item = List.exists (fun elem -> f(item, elem))
    let rec lu = function
        | ([], y) -> y
        | (y, []) -> y
        | (h::t, y) -> if (mem h y) then lu(t, y) else h::lu(t, y)
    lu (l, r)


let rec list_Unionf f = function
    | [] -> []
    | h::t -> list_unionf f (h, list_Unionf f t)


let rec list_intersection = function
    | ([], y) -> []
    | (h::t, y) -> if (memberp h y) then h::list_intersection(t, y) else list_intersection(t, y)

// This curried variant also removes duplicates in the result.
let rec lst_intersection lst = function
    | []    -> []
    | h::tt -> if memberp h lst then singly_add h (lst_intersection lst tt) else lst_intersection lst tt


let rec list_Intersection = function
    | [item] -> item
    | []     -> []
    | h::t   -> list_intersection(h, list_Intersection t)

let rec list_intersection_pred = function
    | ([], y) -> false
    | (h::t, y) -> memberp h y || list_intersection_pred(t, y)
    
let lists_delta a b = list_subtract(lst_union a b, list_intersection(a, b))

// Check pred holds for all of a list (aka all)
let rec conjunctionate f lst =
    let rec kk = function
        | [] -> true
        | h::t -> (f h) && (kk t)
    kk lst

// Check pred holds for any member of a list (aka any)
let rec disjunctionate f lst =
    let rec kk = function
        | [] -> false
        | h::t -> (f h) || (kk t)
    kk lst

let cadr = function
    | a::b::_ -> b


let caddr = function
    | a::b::c::_ -> c


let rec lower_product cc = function // Make all cartesian pairs below (or above if you like) the leading diagonal.
    | a::b::tt ->
        let cc = List.fold (fun c x -> (a,x)::c) cc (b::tt)
        lower_product cc (b::tt)
    | _ -> cc

let all_pairs lst =
    let rec allp1 cc = function
        | a::tt -> List.fold (fun c x -> (a,x)::c) (allp1 cc tt) tt
        | _ -> cc
    allp1 [] lst

// Straightforward caretesian product    
let cartesian_pairs xlst ylst =
    let rec scan2 c = function
        | [] -> c
        | x::xs ->
            let c' = List.fold (fun c y -> (x,y)::c) c ylst
            scan2 c' xs
    scan2 [] xlst
    
// missing comment !
let cartesian_lists xlst ylst =
    let rec scan2 c = function
        | [] -> c
        | x::xs ->
            let c' = List.fold (fun c y -> (x::y)::c) c ylst
            scan2 c' xs
    scan2 [] xlst

// missing comment !    
let cartesian_lists_full xlst ylst =
    let rec scan2 c = function
        | [] -> c
        | x::xs ->
            let c' = List.fold (fun c y -> (x@y)::c) c ylst
            in scan2 c' xs
    scan2 [] xlst
    

let _ :('a list -> 'a list list -> 'a list list) = cartesian_lists


let _ :('a list list -> 'a list list -> 'a list list) = cartesian_lists_full



let mutadd lst v = lst := v::(!lst)
let mutapp lst v = lst := v@(!lst)
let mutapptail lst v = lst := (!lst) @ v
let mutaddonce lst v = if memberp v (!lst) then () else mutadd lst v
let mutaddlist lst v = lst := v @ !lst
let mutaddoncelist lst v = lst := lst_union v !lst



let mutaddoncetail r s =
    if memberp s (!r) then ()
    else
        let _ = r := (!r) @ [s]
        in ()
                            
let rec list_delitem v lst = if lst=[] then [] else if hd lst = v then tl lst else (hd lst)::(list_delitem v (tl lst))
// Fsharp builtin List.fold is a left fold.
let tally f lst = List.fold (fun c x -> c + (if f x then 1 else 0)) 0 lst



// foldr preserves order in terms of returned list
let rec foldr f c = function
    | []   -> c
    | h::t -> f(h, foldr f c t)

// foldl preserves order in terms of side effects
let rec foldl f c = function
    | []   -> c
    | h::t -> foldl f (f(h, c)) t 


let openOut s = s
let closeOut s = ()

let g_argstring = ref ""

let set_g_argstring x =
    g_argstring := x

let valOf_or x d =
    match x with
        | None -> d
        | Some x -> x

let rec list_flatten x = 
        match x with
        | [] -> []
        | (h::t) -> h @ (list_flatten t)

let sfold_delim lim delim ff arg =
    let rec sfold1 nn = function
        | [] -> ""
        | [item] -> ff item
        | hh::tt ->
            if nn > lim then "..." 
            else ff hh + delim + sfold1 (nn+1) tt
    match arg with
        | [] -> "<NONE>"
        | arg -> sfold1 0 arg

let sfold ff lst = sfold_delim 20 ", " ff lst

let sfold_unlimited ff lst = sfold_delim (maxint()) ", " ff lst    

let rec sfolds = sfold (fun x->x)

let rec sfoldcr ff = function
    | []     -> ""
    | [item] -> "\n --- " + ff item + "\n"
    | h::t   -> "\n --- " + ff h +  (sfoldcr ff t)

let sfoldcr_lite ff lst =
    let rec sfl = function
        | []     -> ""
        | [item] -> ff item 
        | h::tt   -> "\n   " + (ff h) + "\n   " + sfl tt
    match lst with
        | []  -> "<NIL>"
        | lst -> sfl lst

let rec sfold_quiet ff = function
    | []     -> ""
    | [item] -> ff item 
    | h::tt   -> ff h + "," + sfold_quiet ff tt

let sfold_ellipsis maxno ff lst =
    let rec sfe n = function
        | []     -> ""
        | [item] -> ff item 
        | h::tt   ->
            let tail = if n >= maxno then "..." else sfe (n+1) tt
            ff h + "," + tail
    sfe 0 lst


//let rec app ff = function
//    | [] -> ()
//    | (h::t) -> (ignore(f h); app ff t)

let app = List.iter

let length = List.length
let map = List.map
let i2s (x:int) = System.Convert.ToString x
let i2s64 (x:int64) = System.Convert.ToString x


let mino l r =
    if nonep l then r
    elif nonep r then l
    else Some(min (valOf l) (valOf r))

let maxo l r =
    if nonep l then r
    elif nonep r then l
    else Some(max (valOf l) (valOf r))
    

let lsf = sf

    
let cassert(g, m) = if (g) then () else lsf("assert: " + m)
let cassertnf(g, m) = if (g) then () else vprintln 0 ("+++assert: " + m)
let assertf g m = if (g) then () else lsf("assert: " + m())


let rec implode l =
    let a c = System.Convert.ToString(c:char)
    match l with
        | [] -> ""
        | h::t -> a h + implode t
        

//-------------------------
// hprls_sml_lib bits now follow

let trimming_sfold f lst = 
      let res = ref ""
//      let rec kk = function
//          | [] -> ()
//          | (p::t) -> 
//              let nv = f p
//              let _ = if nv = "" then () else if !res = "" then res := nv else res := !res + ", " + nv        
//              in kk t
//      let _ = kk lst
      !res

let powers = Array.create 1024 None

let rec two_to_the n =
    if n <= 0 then 1I
    elif n >= 1024 then
        (
         printfn "ERROR : Two_to_the %i OUT OF RANGE\n" n
         failwith "ERROR"
        )
    else
    let ov = powers.[n]
    if ov<>None then valOf ov
    else
        let r = 2I * two_to_the (n-1)
        let _ = Array.set powers n (Some r)
        r

let two_to_the32 n = int(two_to_the n)

(*
  Test code:
       let _ =
        let test_bound_log2 (x:int) =
            let x = BigInteger x
            vprintln 0 (sprintf " bound_log2 of %A -> %i " x (bound_log2 x))
        app test_bound_log2 [0..9]

 bound_log2 of 0I -> 0 
 bound_log2 of 1I -> 1 
 bound_log2 of 2I -> 2 
 bound_log2 of 3I -> 2 
 bound_log2 of 4I -> 3 
 bound_log2 of 5I -> 3 
 bound_log2 of 6I -> 3 
 bound_log2 of 7I -> 3 
 bound_log2 of 8I -> 4 
 bound_log2 of 9I -> 4 
*)
let bound_log2 n =
    // Return number of bits needed to hold arg as an unsigned binary number.
    // For examples:   6->3,   7->3,    8->4,   9->4,  31->5,  32->6.
    // Further examples: 1->1  2->2, 3->2.
    // Is there a correct answer for a negative number ? 1+the first digit that's changed.  So -5 which is 111011, could be said to need four bits.
    // Do not confuse this function with the alternative that gives the number of bits needed to hold a set of packed binary numbers of arity arg. This would give 2->1.
    let rec k r = if (two_to_the r) > n then r else k (r+1)
    k 0 


let bound_log2_arity set_size = bound_log2 (BigInteger(set_size-1)) // Numbered 0 to set_size-1

let rec elide_duplicates = function
    | (a::b::c) -> if a=b then elide_duplicates(b::c) else a :: elide_duplicates(b::c)
    | (other) -> other


let mutinco vv delta =
    let ov = !vv
    let _ = vv  := ov + delta
    ov
    
let mutinc vv delta = (vv  := (!vv) + delta)
let mutinc64 vv (delta:int64) = (vv  := (!vv) + delta)
let mutincu vv s = (mutinc vv s; ())
let mutincu64 vv (s:int64) = (vv:= !vv + s; ())


let mutsub r =
    if nullp !r then lsf("mutsub empty list")
    else
    let ans = hd(!r)
    r := tl(!r)
    ans

let mutaddtail r s = 
    r := (!r) @ [s]
    ()

let Array_cons(a:'a array, s, d)        = Array.set a s (d::a.[s])
let Array_cons_once(a:'a array, s, d)   = Array.set a s (singly_add d a.[s])
let Array_sub(a:'a array, s, d)         = Array.set a s (list_subtract(a.[s], [d]))
let Array_add(a:int array, s, d)        = Array.set a s (a.[s]+d)


let zipWithIndex lst = List.zip lst [0 .. lst.Length - 1]

let strlen s = String.length(s)

let toupper (ss:string) = String.map System.Char.ToUpper ss

let has_str_prefix pref ss = 
    let qp = strlen pref
    let qs = strlen ss
    if qs >= qp && ss.[0..qp-1] = pref then Some(ss.[qp..]) else None

let explode (s:string) =
    let ll = strlen s
    let rec ff p = if p>=ll then [] else (s.[p]) :: (ff (p+1))
    ff 0

let isWhite c = (c = ' ') || (c = '\n') || (c = '\t')
let isDigit c = '0' <= c && c <= '9'
let isDigit_or_minus c = c = '-' || '0' <= c && c <= '9'
let isAlpha c = 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z'



let string_containsp char str = // where is strstr in dot net?
    let ss = strlen str
    let rec str_contains_pred pos = 
        if pos >= ss then false
        elif str.[pos] = char then true
        else str_contains_pred (pos + 1)
    str_contains_pred 0

(* This predicate holds if a string constant represents an annotated integer *)
let rec symbolic_check = function
  | [] -> false
  | ':'::tt -> true
  | n::tt ->isDigit n && symbolic_check tt

let chr_ord = function
    | '0'  ->0
    | '1'  ->1
    | '2'  ->2
    | '3'  ->3
    | '4'  ->4
    | '5'  ->5
    | '6'  ->6
    | '7'  ->7
    | '8'  ->8
    | '9'  ->9
    | 'a' | 'A'  -> 10
    | 'b' | 'B'  -> 11
    | 'c' | 'C'  -> 12
    | 'd' | 'D'  -> 13
    | 'e' | 'E'  -> 14
    | 'f' | 'F'  -> 15
    | _ -> -1

let ord_chr = function
    | 0  -> '0'
    | 1  -> '1'
    | 2  -> '2'
    | 3  -> '3'
    | 4  -> '4'
    | 5  -> '5'
    | 6  -> '6'
    | 7  -> '7'
    | 8  -> '8'
    | 9  -> '9'
    | _ -> '?'

(* Counts from zero: zero is the first element *)
// Note this is builtin to F# as x.[n]
let select_nth (n0:int) l =
    let rec ss n = function
            | h::t -> if n = 0 then h else ss (n-1) t
            | _ -> lsf("Select_Nth:" + i2s n0)
    ss n0 l

let select_nth_or n0 l def =
    let rec ss n = function
        | h::t -> if n = 0 then h else  ss (n-1) t
        | _  -> def
    ss n0 l


let delete_nones lst = 
    let z = function
      | (None, c)->c
      | (Some a, c)->a::c
    foldr z [] lst

let rec delete_assoc v = function
    | [] -> []
    | (v', x)::t -> if v=v' then t else (v', x)::(delete_assoc v t)






let list_remove predf lst =
    let rec del sofar = function
        | [] -> (None, rev sofar)
        | h::tt when predf h -> (Some h, (rev sofar) @ tt)
        | h::tt -> del (h::sofar) tt
    del [] lst



let remove_item(l, vale) =
    let rec really_remove_item =
        function
            | [] -> []
            | (h::t) -> if (h=vale) then t else h::really_remove_item t
    if memberp vale l then really_remove_item l else l

let list_delete_once(r, lst) = remove_item(r, lst)  (* delete once only !*)


let rec drop_first_n = function
    | (0, []) -> []
    | (n, h::t) -> if n=0 then h::t else drop_first_n(n-1, t)
    | (_, _) -> lsf "drop first n"


let rec safe_drop_first_n = function
    | (0, []) -> []
    | (n, h::t) -> if n=0 then h::t else safe_drop_first_n(n-1, t)
    | (_, _) -> []


let list_split n lst =
    let rec splitter p sofar = function
        | []            -> (rev sofar, [])
        | h::t when p=n -> (rev sofar, h::t)
        | h::t          -> splitter (p+1) (h::sofar) t
    splitter 0 [] lst

type moscow_sort_t =
    | LESS
    | EQUAL
    | GREATER


(*
 * Insert into a linear assoc with assic.
 *)
let rec op_assic = function
    | (x, [], y) -> [(x, y)]
    | (x, (p,q)::tt, y) -> if (p=x) then (x,y)::tt else (p,q)::op_assic(x, tt, y)


    
let rec op_assoc v lst =
    match lst with
        | [] ->  None
        | (key, vale)::tt -> if v=key then Some vale else op_assoc v tt



let atoi64 (s:string) = System.Convert.ToInt64 s


let atoi32 (s:string) = System.Convert.ToInt32 s

// Big number to hex string.
let bigPrintHex (n:BigInteger) =
    let rec spex n =
        if n = 0I then "."
        else
            let rem = ref 0I
            let q = BigInteger.DivRem(n, 16I, rem)
            let c = sprintf "%X" (int !rem)
            spex q + c
    if n.IsZero then "0."
    elif n < 0I then "-" + (spex (0I-n))
    else (spex n)

// Ascii to integer, supporting bases 10 and 16, leading unsigned designator and width prefix.
let atoi_p ss =
    let m_unsignedf = ref false
    let m_widtho = ref None
    let sign = ref 1I
    let rec fatoi baser cc = function
        | [] -> cc
        | '_'::tt -> fatoi baser cc tt// As per Verilog, allow meaningless underscores in numbers by ignoring them.
        | '''::tt -> (m_widtho := Some cc;   fatoi 10I 0I tt)
        | 'U'::tt -> (m_unsignedf := true;   fatoi baser cc tt)
        | '-'::tt -> (sign := 0I - !sign; fatoi baser cc tt)
        | 'x'::tt -> fatoi 16I cc tt
        | h::tt    when h >= '0' && h <= '9' -> fatoi baser (BigInteger(chr_ord h) + baser * cc) tt
        | h::tt    when h >= 'a' && h <= 'f' -> fatoi baser (BigInteger(chr_ord h) - 6I - 32I + baser * cc) tt
        | h::tt    when h >= 'A' && h <= 'F' -> fatoi baser (BigInteger(chr_ord h) - 6I + baser * cc) tt
        | h::_ -> sf (sprintf "atoi: bad digit char in number: did not expect '%c' in '%s'" h ss)
    let a0 = (!sign) * fatoi 10I 0I (explode ss)
    let ans = (!sign) * a0
    //vprintln 0 (sprintf "  atoi monitoring (sign=%A, unsignedf=%A, widtho=%A, a0=%A) string %s -> %A" !sign !m_unsignedf !m_widtho a0 s ans + " in hex " + bigPrintHex ans)
    (!m_unsignedf, !m_widtho, ans)

let atoi ss =
    let (unsignedf, widtho, vale) = atoi_p ss
    vale
    
let lpToStr1 = function
    | LP(x, y) ->  x + ": " + i2s y + "\n"


let lpToStr(lp) = "linepoint: " + lpToStr1 lp

let usplay = new Dictionary<string, int>()

// Generate unique identifiers using the seedkey arg as a basis and a key to each unique sequence.
let funique seedkeey = 
    let seedkeey = if seedkeey="" then "uu" else seedkeey
    let rec get n lst = if lst=[] then [] else (hd lst) :: (get (n-1) (tl lst))
    let isanum c = (c >= 'A' && c<= 'Z')||(c >= 'a' && c<= 'z')||(c >= '0' && c<= '9')
    let x1 = get 10 (rev(List.filter isanum (explode seedkeey)))  // Use at most 10 trailing chars of the seed key.
    let v = implode(rev x1)
    let (found, ov) = usplay.TryGetValue(v) 
    let idx = if found then (ignore(usplay.Remove v); ov + 1) else 10
    usplay.Add(v, idx+1)
    let isDigit c = '0' <= c && c <= '9'
    let v = if isDigit v.[0] then "Z" + v else v   // If starts with a digit, prefix with a 'Z' to avoid leading digits in the result.
    let v = if isDigit (hd x1) then v + "_" else v // If ends with a digit, add un underscore to isolate our index.
    v + i2s idx



let mut_hasht_updatee<'a, 'b> table h item update_function new_value =
    let _ = h : (Dictionary<'a, 'b>)
    let (found, ov) = h.TryGetValue(item)
    let _ = if found
            then (ignore(h.Remove(item)); h.Add(item, update_function(Some ov, new_value)))
            else h.Add(item, update_function(None, new_value))
    ()


let chr (i:int) = System.Convert.ToChar i

(*
 * ANSI strings are generated in C/C++ format, but for Verilog, we need to change
 *  %i and %u   to  %d  and
 * and 
 *  %x          to  %h.
 * For Verilog we like to generate %1d when no width is specified to avoid tabs being generated by RTL simulators where mono renders only a space.
 *)
let insert_string_escapes verilogmode s =
    let rec kk = function
        | [] -> []
        | ('%' :: h :: tt) ->
            if verilogmode && (h= 'u' || h='i' || h='d') then kk('%':: '1' :: 'd' :: tt)
            elif verilogmode && h= 'x' then kk('%' :: '1' :: 'h' :: tt)
            elif verilogmode && h= 'X' then kk('%' :: '1' :: 'H' :: tt)   
            else '%' :: kk (h::tt)

        | h::t  -> if h= '\n' then '\\' :: 'n' :: kk t
                   elif h= '\"' then '\\' :: h :: kk t
                   else h :: kk t

    implode(kk (explode s))


let rec mapstring f lst =
    let rec ms = function
        | [] -> ""
        | h :: tt -> f h + ms tt
    ms lst



let timestamp(full) = System.DateTime.Now.ToString(if full then "G" else "T")


let rec htos1 jchar idl = foldr (fun(a,c)->if c="" then a else a + jchar + c) "" (rev idl)

let htos idl = htos1 "/" idl

let htos_explicit idl = htos1 "-/-" idl

let moscow_dummy_fun() = 102

(*
 * Remove any .suffix in a file name, if present.
 * Return a pair if the prefix and the suffix.
 *)
let strip_suffix s =
    let d = rev(explode s)
    let ss = ref []
    let rec st = function
        | [] -> []
        | (h::t) -> if h = '.' then t else (mutadd ss h; st t)
    if (memberp '.'  d) then (implode(rev(st d)), implode(!ss)) else (s, "")
    


//
//
//
let rec arg_assoc_or m = function
    | (v, [], def)  -> def
    | (v, ([])::tt, def) -> arg_assoc_or m (v, tt, def)
    | (v, (x::y)::tt, def) -> if v=x then (if y=[] then sf (m + ": cmdline/recipe control field may not be []. tag=" + v) else y) else arg_assoc_or m (v, tt, def)


let rec arg_assoc v m = function
    | [] -> []
    | (x::y)::tt when v=x ->
        if y=[] then sf (m + ": cmdline/recipe control field may not be []. tag=" + v) else y
    | _::tt -> arg_assoc v m tt



type control_t = string list list



let rec arg_assoc_or_fail m = function
    | (v, [])  -> sf (m + ": requires cmdline/recipe control field tag=" + v) 
    | (v, ([])::tt) -> arg_assoc_or_fail m (v, tt)
    | (v, (x::y)::tt) -> if v=x then (if y=[] then sf (m + ": cmdline/recipe control field may not be []. tag=" + v) else y) else arg_assoc_or_fail m (v, tt)


type argpattern_t =
   |Arg_string_list of string * // Arg identifier
                 string         // Description

   |Arg_int_defaulting of string * // Arg identifier
                 int  *            // Default
                 string            // Description

   |Arg_defaulting of string *     // Arg identifier
                 string  *         // Default - could be a '$argname' meaning to default to the same as another arg.
                 string            // Description

   |Arg_required of string *   // Arg identifier 
                 int *         // No of subsequent parameters, 0 for a flag that is present or absent, -1 for a default arg identifier that needs no introduction.
                 string *      // Description
                 string // spare? occurence count ? recipe stage mapping ?
   |Arg_enum_defaulting of string *   // Arg identifier 
                 string list *        // Possible values
                 string *      // Default
                 string // Description


let g_c3:string list list option ref = ref None

let opath_standing_defs() = valOf_or_nil !g_c3
// Control option binder: masks standing values from g_c3.
let obind (tag, value) = [ tag; value; ]
   
// Get an integer value from the recipe (or override thereof from the command line)
let control_get_i fec id defaultv = 
    let rec s = function
        | [] -> defaultv
        | (h::v::_)::t when id=h -> int32(atoi v)
        | _::t -> s t
    s fec

let control_get_i64 fec id defaultv = 
    let rec s = function
        | [] -> defaultv
        | (h::v::_)::t when id=h -> int64(atoi v)
        | _::t -> s t
    s fec

// Get an integer option value from a control command line option.
let control_get_io fec id = 
    let rec s = function
        | [] -> None
        | (h::v::_)::t when id=h -> Some(int32(atoi v))
        | _::t -> s t
    s fec



// In a control_t of string list list form, return a single string or else the default.
let control_get_s user fec id defaultv = 
    //let _ = vprintln 0 (sprintf "control_get_s START")
    let rec ss al =
        //let _ = vprintln 0 (sprintf "control_get_s" + sfold (fun x-> "<" + sfold (fun x->x) x + ">") al)
        match al with
            | [] -> if defaultv=None then sf("control_get_s : no arg binding for '" + id + "'") else valOf defaultv
            | (h::vv::_)::_ when id=h ->
                //let _ = vprintln 0 (sprintf "have matched >%s< with >%s<" id h)
                vv
            | _::tt -> ss tt
    ss fec


//
// Supposedly a list of strings terminated with -- as in conerefine-keep ?
let control_get_strings fec id = 
    let rec s = function
        | [] -> []
        | (h::tt)::t when id=h -> tt
        | _::t -> s t
    s fec


let substring (s:string, start, len) = s.[start .. start+len]


let ws3 x = if x=None then "EQUIVOCAL" else if x=Some true then "TRUE" else "FALSE"

let rec membercount = function
    | (x, []) -> 0
    | (x, h::t) -> (if x=h then 1 else 0) + membercount(x, t)


let cbg_ordinal = function
    | (0) -> "zeroth"
    | (1) -> "first"
    | (2) -> "second"
    | (3) -> "third"
    | (4) -> "fourth"
    | (5) -> "fifth"
    | (6) -> "sixth"
    | (7) -> "seventh"
    | (8) -> "eighth"
    | (n) -> (i2s n) + (if (n % 10)=1 then "st" else if (n % 10)=2 then "nd" else "th")


let isPrefix p (s:string) =
    let ll = String.length p
    let ss = String.length s
    ll <= ss && ll >= 2  && s.[0..ll-1] = p


let rec idl_ordering = function
    | ([], []) -> EQUAL
    | ([], _) -> LESS
    | (_, []) -> GREATER
    | (((a:string)::af), (b::bf)) -> 
                if a=b then idl_ordering(af, bf) else
                if (a<b) then LESS else GREATER


//  val testdata = [ [1,2,3,4], [10, 20, 30, 40], [100, 200, 300, 400]];
let rec transpose = function
    | [] -> []
    | l -> if length(hd l) = 0 then [] else (map hd l) :: transpose(map tl l)


type myseq<'T> =
    | Nil
    | Cons of 'T *  (unit -> myseq<'T>)



    
let mys_cons a b  = Cons(a, fun()->b)

let mys_list1 v = Cons(v, fun () -> Nil)


let mys_null = function
    | Nil       -> true
    | Cons(a,x) -> false


let mys_nonnull = function
    | Nil       -> false
    | Cons(a,x) -> true


let mys_length arg =
    let rec count c = function
        | Nil        -> sf "  c "
        | Cons(_, x) -> sf " count (c+1) (x()) "
    count 0 arg


let mys_hd = function
    | Nil -> sf "mys hd"
    | Cons(a, x) -> a


let mys_tl = function
    | Nil -> sf "mys tl"
    | Cons(a, x) -> x()

    
let rec mys_equal a b =
    match a with
    | Nil        -> not(mys_nonnull b)
    | Cons(xv, x) -> mys_nonnull b && mys_hd b = xv && mys_equal (x()) (mys_tl b)


let rec mys_append a b =
    match b with
        | Nil -> a
        | _ ->
            match a with
                | Nil -> b
                | Cons(a,x) -> Cons(a, fun () -> mys_append (x()) b)


let rec mys_toLst x = if mys_nonnull x then (mys_hd x) :: (mys_toLst (mys_tl x)) else []


let mys_unfold = function
    | None -> Nil
    | Some(a,b) -> Cons(a, b)
   

// foldl preserves order in terms of side effects
let rec mys_foldl f c = function
    | Nil        -> c
    | Cons(h, t) -> mys_foldl f (f c h) (t()) 



// A mutable dictionary that uses None/Some paradigm.
type OptionStore<'k_t, 'v_t when 'k_t : equality>(id:string, initfrom:OptionStore<'k_t, 'v_t> option) = class

    let dict =
        if nonep initfrom then new Dictionary<'k_t, 'v_t>()
        else
            let gv:Dictionary<'k_t, 'v_t> = (valOf initfrom).getDict()
            new Dictionary<'k_t, 'v_t>(gv)

    let vd = false
    
    member x.getDict() = dict
    
    new(id: string) = OptionStore(id, None) // First variant constructor
   

    interface IEnumerable<KeyValuePair<'k_t, 'v_t>>
       with
           member x.GetEnumerator() = dict.GetEnumerator() :> IEnumerator<KeyValuePair<'k_t, 'v_t>>
           member x.GetEnumerator() = dict.GetEnumerator() :> System.Collections.IEnumerator

    member x.Count = dict.Count

    member x.lookup (k: 'k_t) =
      let (found, ov) = dict.TryGetValue k
      if vd then print(sprintf "OptionStore lookup: id=%s key=%A found=%A" id k found)      
      if not found then None else Some ov

    member x.add k v =
      let (found, ov) = dict.TryGetValue k
      let _ = if found then dict.Remove k else false
      if vd then print(sprintf "OptionStore add: id=%s key=%A oldfound=%A" id k found)
      dict.Add(k, v)
      ()

    member x.remove k rf =
      if vd then print(sprintf "OptionStore add: id=%s key=%A remove" id k)       
      ignore(dict.Remove k)
      ()

    member x.fold folder state = dict |> Seq.fold (fun acc pair -> folder acc pair.Key pair.Value) state

end // end of OptionStore

// A mutable store for lists
type ListStore<'k_t, 'v_t when 'v_t : equality and 'k_t : equality>(id:string, dict:Dictionary<'k_t, 'v_t list>) = class

    new (id:string) = new ListStore<'k_t, 'v_t>(id, (new Dictionary<'k_t, 'v_t list>()))

    interface IEnumerable<KeyValuePair<'k_t, 'v_t list>>
       with
           member x.GetEnumerator() = dict.GetEnumerator() :> IEnumerator<KeyValuePair<'k_t, 'v_t list>>
           member x.GetEnumerator() = dict.GetEnumerator() :> System.Collections.IEnumerator

    member x.lookup k =
      let (found, ov) = dict.TryGetValue k
      if not found then [] else ov

    member x.add k v =
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, [v])
              else let _ = dict.Remove k in dict.Add(k, singly_add v ov) // singly_add is used to store each item once - requires equality on content type.
      ()

    member x.addflexi addf k v = // This hof allows the user to avoid the singly_add method which can explode the stack with complex comparisons.
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, [v])
              else let _ = dict.Remove k in dict.Add(k, addf v ov)
      ()

    member x.KeyCount = dict.Count

    member x.remove k rf =
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, [])
              else let _ = dict.Remove k in dict.Add(k, (List.filter rf ov))
      ()

    member x.clear k =
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, [])
              else let _ = dict.Remove k in dict.Add(k, [])
      ()

    member x.replace k rf v =
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, [v])
              else let _ = dict.Remove k in dict.Add(k, singly_add v (List.filter rf ov))
      ()

    member x.adda k v = // Add multiple at once
      let (found, ov) = dict.TryGetValue k
      let _ = if not found then dict.Add(k, v)
              else let _ = dict.Remove k in dict.Add(k, lst_union v ov)
      ()

    member x.clone (id:string) = new ListStore<'k_t, 'v_t>(id, new Dictionary<'k_t, 'v_t list>(dict))

    member x.fold folder state = dict |> Seq.fold (fun acc pair -> folder acc pair.Key pair.Value) state

end // end of ListStore




// Class Engine : Equivalence classes with automatic class names that are strings for items of type v_t.
// The equality predicate is needed on the members, but that is not what determines equivalence of course. Equivalence is
// explicitly implied by the pattern of method calls made as members are added or as classes are conglomorated.
type EquivClass<'v_t  when 'v_t : equality >(id:string) = class    
    let classes = new Dictionary<string, int * 'v_t list>()
    let iidx =    new Dictionary<'v_t, string>()
    let nxt = ref 1
    let merge_lock = ref false
    let report_rez = ref 100
   
    let m_items_stored = ref 0
    let m_classes_in_use = ref 0    

    let merge callback (onc:string, nnc) =  // newest one disappears if both have members
        if onc=nnc then nnc
        else
        let vd = 4
        let (ofound, g0) = classes.TryGetValue onc
        if not ofound then nnc
        else
        if !merge_lock then sf (sprintf "attempt merge on a locked set of equivalence classes: id=%s  onc=%A nnc=%A" id onc nnc)
        else
        let (nfound, g1) = classes.TryGetValue(nnc)
        if not nfound  then onc
        else
        let (onv, omembers) = g0
        let (nnv, nmembers) = g1
        let ovl = if ofound then let _ = classes.Remove(onc) in omembers else []
        let nvl = if nfound then let _ = classes.Remove(nnc) in nmembers else []
        // Swap over old and new so that the one which persists (called here the new one) is actually the oldest (numerically lowest).


        let (ovl, nvl, onc, nnc, onv, nnv, omembers, nmembers) = if nnv < onv then (ovl, nvl, onc, nnc, onv, nnv, omembers, nmembers) else  (nvl, ovl, nnc, onc, nnv, onv, nmembers, omembers)
        //vprintln 0 ("+++ equivclass_merge from nnc=" + nnc + " to onc=" + onc + " old=" + sfold (fun x->sprintf "%A" x) ovl + " new=" + sfold (fun x-> sprintf "%A" x) nvl)
        // Append is sufficient since no atom should be in two classes
        classes.Add(nnc, (nnv, nvl @ ovl))

        //vprintln 0 ("+++ equivclass_merge 2")
        // Move all of old to new in inverted index
        let change aid =
            let (found, v) = iidx.TryGetValue aid
            let _ = cassert (found, "aid had no ptr")
            let _ = iidx.Remove aid
            //vprintln 0 (sprintf "+++ equivclass_merge 3 add %A" aid)            
            iidx.Add(aid, nnc)
        app change omembers
        //vprintln 0 ("+++ equivclass_merge 4 done")
        // We run the callback at the end since some users mess with this class during the callback, which is not nice!
        mutinc m_classes_in_use -1
        app (callback onc nnc) ovl // Apply the callback to each old member
        nnc


    interface IEnumerable<KeyValuePair<string, int * 'v_t list>>
        with
            member x.GetEnumerator() = classes.GetEnumerator() :> IEnumerator<KeyValuePair<string, int * 'v_t list>>
            member x.GetEnumerator() = classes.GetEnumerator() :> System.Collections.IEnumerator


    member x.anotherEnumerator() = classes.GetEnumerator()

    member x.Lock msg_ =
        merge_lock := true
        ()

    member x.ClassOf item =
        let (found, ov) = iidx.TryGetValue(item)
        if not found then None else Some ov

    member x.setVerbose vd = report_rez := vd
    
    // Add a new member, seeding a fresh class if not already present in any class.
    member x.AddMember item =
        //        let _ = vprintln 0 (iname + " Add member " + htos m + " to " + p2ss c)
        let (found0, ov0) = iidx.TryGetValue(item)
        if found0 then ov0
        else
            let q = !nxt
            nxt := q+1
            if !report_rez < 100 then vprintln !report_rez (id + sprintf ": AddMember %A allocs fresh class %s%i" item id q)
            mutinc m_items_stored 1
            mutinc m_classes_in_use 1            
            //let _ = vprintln 0 (id + sprintf ": YES AddMember %A allocs fresh class %s%i" item id q)            
            let key = id + i2s q
            classes.Add(key, (q, [item]))
            iidx.Add(item, key)
            key

    // Add a new member to an existing class.
    // Causes conglomoration and extinction of existing classes if 'new' member were in an old class.
    // Existing class must be killed so that "app (AddClass cls) lst" works.
    member x.AddToClass callback cls item =
        let (found, ocls) = iidx.TryGetValue item
        if found then merge callback (ocls, cls)
        else
            mutinc m_items_stored 1
            let (ofound, (onv, omembers)) = classes.TryGetValue cls 
            let ovl = if ofound then let _ = classes.Remove cls in omembers else []
            if not ofound then mutinc m_classes_in_use 1            
            app (callback ocls cls) omembers
            classes.Add(cls, (onv, ovl @ [item])) // Retain leader of ovl at least.
            iidx.Add(item, cls)
            cls
            
    // Move a member between classes: perhaps add such a method?
    
    member x.Members key =
       let (found, ov) = classes.TryGetValue(key)
       if found then snd ov else []

    member x.Friends item = // aka Peers
        let (found, cls) = iidx.TryGetValue item        
        if found then x.Members cls else [item] // We return just itself but this hides that we've never heard of it.

    // AddFreshClass causes automatic conglomoration of classes where a member is already in another.
    member x.AddFreshClass callback args =
        if nullp args then ""
        else
            let rec prescan = function     // This is optimised to first find one that does have a class
                | h::tt ->
                    let (found, cls) = iidx.TryGetValue(h)
                    if found then (cls, h) else prescan tt
                | [] -> (x.AddMember(hd args), hd args)
            let (cls, doner) = prescan args
            let rec rester cls = function
               | [] -> cls
               | h::tt ->
                   if h=doner then rester cls tt
                   else
                       let cls' = x.AddToClass callback cls h
                       rester cls' tt
            rester cls args

    member x.Stats() = (id, !m_items_stored, !m_classes_in_use)

end

type clskey_t = string

// A mutable store for named items in equivalence classes
type ClassStore<'k_t, 'v_t when 'v_t : equality and 'k_t : equality>(id:string) = class
   let dict = new Dictionary<'k_t, (clskey_t * 'v_t)>()
   let classContents = new ListStore<clskey_t, 'k_t>("classContents")


(*   member x.enum() =
       let s0 = dict.GetEnumerator()
       let gc () = muddy "gc"
       { s0 with member _.Current=s0.Current; }

   interface IEnumerable<KeyValuePair<clskey_t, ('k_t * 'v_t) list>>
        with
            member x.GetEnumerator() = x.enum() :> IEnumerator<KeyValuePair<clskey_t, ('k_t * 'v_t) list>>
            member x.GetEnumerator() = x.enum() :> System.Collections.IEnumerator
*)

   member x.readout() =
        let rr = ref []
        let repot v =
           let (found, ov) = dict.TryGetValue v
           (v, snd ov)
        let _ = for pp in classContents do mutadd rr (pp.Key, map repot pp.Value) done
        !rr

   member x.members clsKey =
        let repot v =
           let (found, ov) = dict.TryGetValue v
           (v, snd ov)
        map repot (classContents.lookup clsKey)


   member x.conglom oc nc =
        let swapCls k =
            let (found, ov) = dict.TryGetValue k
            let _ = cassert(found, "ov not found")
            (ignore(dict.Remove k); dict.Add(k, (nc, snd ov)))
        let prev = classContents.lookup oc
        app swapCls prev
        classContents.clear oc
        classContents.adda nc prev
        nc

   // Add a fresh named group of items to the store as a fresh equivalence class, but if one of them overlaps with
   // an existing item then they are added to the existing items class instead. Multiple overlaps cause conglomoration
   // of existing ones
   member x.addItems (nameBasis:string) (items :('k_t * 'v_t) list)  =
        let add_where sofar (k, v) =
            let (found, ov) = dict.TryGetValue k
            if not found then sofar
            else
               match sofar with
                   | None -> Some(fst ov)
                   | Some e -> Some(x.conglom e (fst ov))
                       
        let clsKey =
            match List.fold add_where None items with
                | None -> funique nameBasis
                | Some clsKey -> clsKey
        let insert (k, v) = if not (dict.ContainsKey k) then dict.Add(k, (clsKey, v))
        app insert items
        classContents.adda clsKey (map fst items)
        clsKey

   member x.classOf k =
       let (found, ov) = dict.TryGetValue k
       if not found then None
       else Some (fst ov)

   member x.valueOf k =
       let (found, ov) = dict.TryGetValue k
       if not found then None
       else Some (snd ov)



end

//
//
  
let rec i2base26 v =
    let ch v = System.Convert.ToString(System.Convert.ToChar(v + System.Convert.ToInt32('a')))
    if (v < 0) then sf "negative"
    elif (v < 26) then (ch v)
    else (i2base26 (v/26)) + (ch (v % 26))

let rec hyper_op_assoc v lst =
    match lst with
        | [] ->  None
        | ((x,y)::tt) -> if memberp v x then Some(y) else hyper_op_assoc v tt


let assoc_update f tag alist =
    let rec scan = function
        | [] -> [(tag, f None)]
        | (t, v)::tt when t=tag -> (t, f (Some v)) :: tt
        | (t, v)::tt -> (t, v) :: scan tt
    scan alist

    //
// sscanf implementation from Wolfgang Meyer http://fssnip.net/4I
//
open System
open System.Text
open System.Text.RegularExpressions
open Microsoft.FSharp.Reflection


let check f x =
    if f x then x
    else failwithf "format failure \"%s\"" x

    
let parseDecimal x = Decimal.Parse(x, System.Globalization.CultureInfo.InvariantCulture)


let parsers = dict
                [
                    'b', Boolean.Parse >> box
                    'd', int >> box
                    'i', int >> box
                    's', box
                    'u', uint32 >> int >> box
                    'x', check (String.forall Char.IsLower) >> ((+) "0x") >> int >> box
                    'X', check (String.forall Char.IsUpper) >> ((+) "0x") >> int >> box
                    'o', ((+) "0o") >> int >> box
                    'e', float >> box // no check for correct format for floats
                    'E', float >> box
                    'f', float >> box
                    'F', float >> box
                    'g', float >> box
                    'G', float >> box
                    'M', parseDecimal >> box
                    'c', char >> box
                ]

  
// array of all possible formatters, i.e. [|"%b"; "%d"; ...|]
let separators =
    parsers.Keys
    |> Seq.map (fun c -> "%" + sprintf "%c" c)
    |> Seq.toArray


// Creates a list of formatter characters from a format string,
// for example "(%s,%d)" -> ['s', 'd']
let rec getFormatters xs =
    match xs with
        | '%'::'%'::xr -> getFormatters xr
        | '%'::x::xr -> if parsers.ContainsKey x then x::getFormatters xr
                        else failwithf "Unknown formatter %%%c" x
        | x::xr -> getFormatters xr
        | [] -> []


let sscanf (pf:PrintfFormat<_,_,_,_,'t>) s : 't =
    let formatStr = pf.Value.Replace("%%", "%")
    let constants = formatStr.Split(separators, StringSplitOptions.None)
    let regex = Regex("^" + String.Join("(.*?)", constants |> Array.map Regex.Escape) + "$")
    let formatters = pf.Value.ToCharArray() // need original string here (possibly with "%%"s)
                     |> Array.toList |> getFormatters
    let groups =
        regex.Match(s).Groups
        |> Seq.cast<Group>
        |> Seq.skip 1
    let matches =
        (groups, formatters)
        ||> Seq.map2 (fun g f -> g.Value |> parsers.[f])
        |> Seq.toArray
            
    if matches.Length = 1 then matches.[0] :?> 't
    else FSharpValue.MakeTuple(matches, typeof<'t>) :?> 't

(* // some basic testing
let (a,b) = sscanf "(%%%s,%M)" "(%hello, 4.53)"
let (x,y,z) = sscanf "%s-%s-%s" "test-this-string"
let (c,d,e,f,g,h,i) = sscanf "%b-%d-%i,%u,%x,%X,%o" "false-42--31,13,ff,FF,42"
let (j,k,l,m,n,o,p) = sscanf "%f %F %g %G %e %E %c" "1 2.1 3.4 .3 43.2e32 0 f"

*)

let rec zipWithIndex_based baseref = function
    | [] -> []
    | a::tt ->
        let nv = !baseref
        let _ = baseref := nv + 1
        (a, nv) :: zipWithIndex_based baseref tt
        
// Check if a list contains repeated entries
let contains_repetitions lst =
    let rec crf = function
        | a::c -> memberp a c || crf c
        | _ -> false
    crf lst

// pastel colours for shape filling
let g_pastels = [ "skyblue"; "yellow"; "seagreen1"; "paleturquoise1"; "hotpink1"; "powderblue"; ]

// Insert int in ascending list deleteing duplicates.
let ascending_int_sort_add v lst =
    let rec iiv = function
        | [] -> [v]
        | h::t  when h = v -> h::t
        | h::t  when h < v -> h :: iiv t
        | h::t  when h > v -> v :: h :: t
    iiv lst

// Substract a pair of ascending lists, preserving order.
let rec ascending_int_subtract = function
      | (ll, []) -> ll
      | ([], _)  -> []
      | (ll::llt, rr::rrt) ->
          if   ll=rr then ascending_int_subtract(llt, rrt)
          elif ll>rr then ascending_int_subtract(ll::llt, rrt)
          else (* ll<rr *) ll::ascending_int_subtract(llt, rr::rrt)                    

// Combine two ascending lists, deleting any duplicates.
let rec union_ascending_int_sort a b =
    if a=[] then b
    elif b=[] then a
    else union_ascending_int_sort (tl a) (ascending_int_sort_add (hd a) b)

// Flattern a list of ascending lists, deleting any duplicates.
let rec Union_ascending_int_sort = function
    | [] -> []
    | hh::tt -> union_ascending_int_sort hh (Union_ascending_int_sort tt)


let list_tally pred lst =  List.fold (fun cc item -> (if pred item then 1+cc else cc)) 0 lst


type string_burner_t(ss:string, seekpos) = class
    let dptr:int ref = ref seekpos

    let len = strlen ss

    let ch() = 
        if !dptr >= len then (char 0)
        else
            //let _ = vprintln 0 (sprintf "  string_burner Ch %i %c" !dptr  ss.[!dptr])
            ss.[!dptr]

    let nextch() =
        if !dptr >= len then (char 0)
        else
            let rr = ss.[!dptr]
            let _ = dptr := !dptr + 1
            rr


    let rec atoi rr =
        if ch() >= '0' && ch() <= '9' then atoi (rr*10L + int64(chr_ord(nextch()) - chr_ord '0'))
        else rr

    member x.NextCh() = nextch()

    member x.Ch() = ch()

    member x.Reset() = dptr := 0

    member x.Adv() = dptr := !dptr + 1

    member x.Atoi () = atoi 0L // TODO cope with leading -ve flag?
    
end
        
// Insert totally ascending ordered seq in binary search tree in order leads to complete skew - refurl permutes the list to finally yield a balanced tree. Perhaps a better standard techniques exist?
let refurl items =
    let list_split_rev n lst =
       let rec splitter p sofar = function
           | []            -> (sofar, [])
           | h::t when p=n -> (sofar, h::t)
           | h::t          -> splitter (p+1) (h::sofar) t
       splitter 0 [] lst
    
    let rec refurl1 cc len items =
        if len < 3 then items @ cc
        else match items with
               | [] -> cc
               | [item] -> item::cc
               | items ->
                   let len2 = len / 2 // Will round down, so any odd member will be at hd b.
                   let (a, b) = list_split_rev len2 items
                   let cc = refurl1 cc len2 a
                   if nullp b then cc else hd b :: (refurl1 cc len2 (tl b))
    refurl1 [] (length items) items

    
// Certain net names contain troublesome characters or are Verilog keywords: munge them here.
let v_id_san = function
    |  ('@') -> "A_"    
    |  ('?') -> "Q_"
    |  ('$')
    |  ('*')
    |  ('`')
    |  ('.')
    |  (':')
    |  ('!')
    |  ('{')
    |  ('}')
    |  ('-')
    |  ('%')
    |  (' ')
    |  ('<')
    |  ('>')
    |  (',')
    |  ('/')
    |  ('(')
    |  (')')
    |  ('[')
    |  (']') -> "_"
    |  ('\"') -> "q"
    |  other -> ""


let vsanitize l =
    let rec no_double_underscores =
        function
            | [] -> []
            | [item] -> [ item ]
            | (('_')::('_')::t) -> no_double_underscores(('_')::t)
            | (h::t) -> h::no_double_underscores(t)

    let san1 c cc =
        match v_id_san c with
            | "" -> c :: cc
            | s  -> (explode s) @ cc
    let r = no_double_underscores(List.foldBack san1 (explode l) [])
    implode r


// Floyd-Warshall's Algorithm : min cost all pairs.
let floyd_warshall undirected arity args =
    let infinity = -1
    let m_array = Array2D.create arity arity (infinity, [])
    for i=0 to arity-1 do m_array.[i,i] <- (0, [])
    let insert0 (a, b, cost) =
        m_array.[a,b] <- (cost, [(a,b)])
        if undirected then m_array.[b,a] <- (cost, [(b,a)])
    let _ = app insert0 args
    for i=0 to arity-1 do
        for j=0 to arity-1 do
            for k=0 to arity-1 do
                if fst(m_array.[i,k])<>infinity && fst(m_array.[k,j])<>infinity then
                    let s = fst(m_array.[i,k]) + fst(m_array.[k,j])
                    if  fst(m_array.[i,j])=infinity || fst(m_array.[i,j]) > s then m_array.[i,j] <- (s, snd(m_array.[i,k]) @ snd(m_array.[k,j]))  
    (m_array, infinity)


// Generic list collator
let generic_collate pivotf lst =
    let tags = List.fold (fun cc x -> singly_add (pivotf x) cc) [] lst
    let collator tag = (tag, List.filter (fun x->pivotf x = tag) lst)
    map collator tags

let unzip4 lst =
    let uz4 (a, b, c, d) (aa, bb, cc, dd) = (a::aa, b::bb, c::cc, d::dd)
    List.foldBack uz4 lst ([], [], [], [])



// Here we break up long strings, justifying them to an approximate target width.
let string_autoinsert_newlines target_width sx =
    // TODO add option to not break line inside quotes
    // TODO add option to insert backslash to escap newlines
    let tolerance = 10
    let rec sanwidth lnow = function
        | [] -> []
        | h::tt   when lnow <  target_width           -> h :: sanwidth (lnow+1) tt
        | ' '::tt when lnow >= target_width           -> '\\' :: 'n' :: sanwidth (0) tt
        | h::tt   when lnow <  target_width+tolerance -> h :: sanwidth (lnow+1) tt
        // TODO perhaps split sx evenly when the trailing final line is very short.
        | sx      when lnow >= target_width+tolerance -> '\\' :: 'n' :: sanwidth (0) sx
        
    let ans = implode(sanwidth 0 (explode sx))
    //dev_println (sprintf ">%s< was imploded" ans)
    ans


let transclose_augmenter lst = 
    let aug (p, q) = List.fold (fun cc (a,b) -> (if q=a then (p,b)::cc else cc)) [] lst
    let ans = list_flatten(lst :: map aug lst)
    list_once ans


let rec fixed_point_iterate metric augmenter arg = 
    let nv = augmenter arg
    if metric nv = metric arg then arg else fixed_point_iterate metric augmenter nv

// Form the transitive closure of a list of directed pairs.    
let transclose arg = fixed_point_iterate List.length transclose_augmenter arg


type route_matrix_t<'a when 'a: equality> = ListStore<'a, 'a> 

// Return only the nodes in live paths (assumed connected path) from a route matrix.    
let generic_live_trim ww (route_matrix:route_matrix_t<'a>) report  = 
    let nodes = map (fun (kk:KeyValuePair<'a, 'a list>)->kk.Key) (route_matrix |> Seq.toList)
    let rec live_trim inl = // Lists give quadratic cost here !  Use Set
        let check nn = disjunctionate (fun a-> memberp a inl) (route_matrix.lookup nn)
        let outl = List.filter check inl
        let inll = length inl
        let _ = report("Generic live trim: " + i2s(inll) + " states in the running.")
        let changed = (inll <> List.length outl)
        if changed then live_trim outl else inl
    live_trim nodes

        
let rec apply_n_times ff nn arg =
    if nn = 0 then arg
    else apply_n_times ff (nn-1) (ff arg)

(* eof *)
