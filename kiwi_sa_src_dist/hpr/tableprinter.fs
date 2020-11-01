(*
 *  $Id: tableprinter.fs $
 *
 * CBG Orangepath.
 *
 * HPR L/S Formal Codesign System.
 * (C) 2003-14 DJ Greaves. All rights reserved.
 *
 *
 * Contains: write an ASCII art table.
 *         : read in and parse the same table - after human edits we expect.
 *
 *
 *)
module tableprinter
open yout

open moscow

let rec find_slash_idx = function
    | ([]) -> 0
    | ('/'::t) -> 0
    | ('\\'::t) -> 0
    | (_::t) -> 1 + find_slash_idx(t)


let rec get_first_n(x, n) = if n <= 0 then [] else (hd x) :: get_first_n(tl x, n-1)


//(* generate source c directory *)
let rec get_dir_of_filename = function
    | ('.' :: '/' :: t) -> "./"
    | ('.' :: '\\' :: t) -> ".\\"
    | (x) -> implode(get_first_n(x, length(x) - find_slash_idx(rev x)))



let rec table_get_widths c row =
    let rec fpair_proper_name = function
        | ([], _) -> []
        | (r, []) -> r
        | (h::t, p::q) -> (max h p) :: fpair_proper_name(t, q)
    fpair_proper_name(map (String.length) row, c)


    
let rec  table_make_hchar(n) = if (n=0) then "" else "-" + table_make_hchar(n-1)


let rec table_spaces(n) = if (n = 0) then "" else " " +  table_spaces(n-1)

let g_table_max_width = ref 180


 
//
// This version makes the table as a string.
// Is there a version that uses a writer upcall?
//
let create_table(title, hdr, list) =
    let widths = List.fold table_get_widths [] (hdr :: list)
    let rec fold_wide_cols widths =
        let total = List.fold (fun a b -> a+b) 0 widths
        if total < !g_table_max_width then widths
        else
            let widest = List.fold (fun c x -> (if c<0 || x > c then x else c)) -1 widths
            if widest < 10 then widths // give up - all cols getting ridiculously narrow
            else
                let rec partition = function
                    | [] -> (vprintln 0 ("Did not partition table column!"); [])
                    | h::t when h >= widest -> ((h*2)/3) :: t
                    | h :: t -> h :: partition t
                fold_wide_cols (partition widths) //Iterate until happy.

    let widths = fold_wide_cols widths
    let hbars =
        let rec table_gen_hbar cc = function
            | []    -> "*"
            | h::tt -> cc + table_make_hchar(h+2) + table_gen_hbar "+" tt
        table_gen_hbar "*" widths
    let table_gen_rows h cc =

        // If a long entry then split and put in a fresh row of overflows. 
        // Long is defined as larger than g_table_max_width.
        let rec table_gen_row cs overflowing overflows = function
            | ([], _)       -> (cs + "|", overflowing, overflows)
            | (h::t, w::wt) ->
                let s = strlen h
                let (d, d_over, overflowing, padding) =
                    if s <= w then (h, "", overflowing, w-s)
                    else (h.[0..w-2] + "\\", h.[w-1..s-1], true, 0)
                    //let (ans_r, cc, overflowing, overflows) = 
                let cs = cs + ("| " + d + table_spaces(padding + 1))
                table_gen_row cs overflowing (d_over :: overflows) (t, wt)

        let rec dol h cc =
            let (oneline, overflowing, overflows) = table_gen_row "" false [] (h, widths)
            if not overflowing then oneline :: cc
            else oneline :: dol (rev overflows) cc
        dol h cc

    let rows = List.foldBack table_gen_rows list []
    let hdr' = table_gen_rows hdr []
    let ans =
        if nullp list then [ title + " = Nothing to Report\n" ]
        else [ title; hbars] @  hdr' @ [ hbars ] @ rows @ [ hbars ]
    //(sprintf "%i table lines" (length ans)) :: ans
    ans
(*
 * Simplistic text input routines - read back in a table that was printed out.
 *)
let isanumdol(c) = 
     (c >= 'a' && c <= 'z') ||
     (c >= 'A' && c <= 'Z') ||
     (c >= '0' && c <= '9') ||
     (c = '$') || (c = '-') || (c = '_')

let rec nextword_skip_white (cis:System.IO.StreamReader)  =
    if cis.Peek() = -1  then ""
    else 
    let c = System.Convert.ToChar(cis.Read())
    if (isanumdol c) then implode [c] else nextword_skip_white(cis)


(*
 * Read the next word or selected punctuation.  White space needed between
 * each one.  Perhaps use a builtin lexer in future ?
 *)
let rec nextword(cis:System.IO.StreamReader) = 
     let c = nextword_skip_white(cis)
     in c + nextword1(cis)

and nextword1(cis) =
    let c = cis.Peek()
    if c = -1 then ""
    else 
       let ch = System.Convert.ToChar(cis.Read())
       if ch = '\n' || ch = '\n' then "\n"
       elif (isanumdol ch) then implode [ch] + nextword1(cis)
       else ""



let rec lexate_the_file(fname) =
    let cis = new System.IO.StreamReader(fname:string)
    lexate_the_file_fd(cis)

and lexate_the_file_fd(cis:System.IO.StreamReader) =
        if cis.Peek() = -1 then []
        else nextword(cis) :: lexate_the_file_fd(cis)


(*
 * Read down the list of tokens and split out sublists delimited with the
 * sot and eot markers.
 *)
let rec parse_the_file(fname) =
   let toks = lexate_the_file(fname)
   parse_the_file1(toks, false, [], [])

and parse_the_file1 = function
    | ([], _, c, r) -> r
    | (h::t, flag, c, r) ->
     if (h = "$TTSET") then parse_the_file1(t, true, [], r)
     elif (h = "$" && flag) then (rev c) :: parse_the_file1(t, false, [], r)
     elif (flag) then parse_the_file1(t, flag, h::c, r)
     else parse_the_file1(t, flag, [], r)


let rec implode_strings = function
    | [] -> ""
    | h::t -> h + " " + implode_strings(t)


(* Token conversion to string  *)
let rec el_arg_s = function
    | (pos, [], cmdref)  -> ( sf("Insufficient args to command: " + cmdref); "")
    | (pos, h::t, cmdref)  -> if (pos = 1) then h else el_arg_s(pos-1, t, cmdref)


(* Token conversion to string or int *)
let rec el_arg_i = function
    | (pos, [], cmdref)  -> ( sf("Insufficient args to command: " + cmdref); 0 )
    | (pos, h::t, cmdref)  ->
        if (pos = 1)
          then
          (       
                  if (h >= "0" && h <= "9") then int32(atoi h)
                  else (sf("Numeric arg needed: " + cmdref); 0)
          )
          else el_arg_i(pos-1, t, cmdref)


(* eof *)

