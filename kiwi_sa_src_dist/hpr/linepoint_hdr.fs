(*
 *  $Id: linepoint_hdr.fs,v 1.4 2012-11-19 22:20:46 djg11 Exp $
 *
 * CBG Orangepath HPR L/S Formal Codesign System.
 * (C) 2003 DJ Greaves. All rights reserved.
 *
 *
 * Contains:
 * 
 *
 *
 *)


module linepoint_hdr

type linepoint_t = LP of string * int


let g_line = ref -1


let g_file = ref "noname"


let log_linepoint = function
    | (LP(x, y)) -> ( g_line := y; g_file := x)


let get_linepoint() = LP(!g_file, !g_line)


let lpToStr0(LP(x, y)) =  x + ": " + System.Convert.ToString y


let lpToStr1(LP(x, y)) =  x + ": " + System.Convert.ToString y + "\n"


let lpToStr(lp) = "linepoint: " + lpToStr0 lp


(* eof *)

