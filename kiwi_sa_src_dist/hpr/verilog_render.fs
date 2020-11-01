(* 
 * HPR L/S  Orangepath: Write Verilog RTL out to a file.
 *
 * (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met: redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer;
 * redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution;
 * neither the name of the copyright holders nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *)

module verilog_render


open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics

open verilog_hdr
open meox
open hprls_hdr
open opath_hdr
open moscow
open yout
open abstract_hdr
open abstracte

type cell_count_t = Dictionary<string, int>

type regstat_t =
    {
        is_reg:      bool // Should be declared as reg not wire
        is_combreg:  bool // Is assigned full-supported (always_comb) so no reset should be applied.
    }
type regslist_t = OptionStore<string, regstat_t>

type default_rtl_fun_atts_t = 
  {
      fsems:                fun_semantic_t  
      //stateless_:           bool // This set when instances can be freely shared. - currently we use gis.fsems.fs_mirrorable flag instead
  }

type array_size_log_t = OptionStore<int, int64>

type statereport_t =
    { scalars : int64 ref;
      vectors : array_size_log_t;
      arrays : array_size_log_t;
      active : bool;
      continuous : int64 ref;
      cell_counts: cell_count_t;
    }


type range_t = precision_t 
type dom_t = { in_prec: precision_t }

type directory_t = OptionStore<string, v_exp_t * hexp_t> 

type dd_t =
    {
        sr:        statereport_t option
        directory: directory_t option              // Mapping of identifier names to v_exp_t and hexp_t declarations.
        ver_apps:  ((string * dom_t list * range_t) * string) list ref
        nextbase:  int ref
        ddctrl:    ddctrl_t
    }

let g_default_timescale = "`timescale 1ns/1ns"

// Pretty printer.
type cbg_pp(maxwidth : int, emitter_callback : (string -> unit) option) = class
  let mutable lines:string list = []
  let mutable lineno = 0
  let mutable line = ""
  let mutable idx = 0
  let mutable indent = 0
  let mutable indents = []
  let mutable breakpend = false

  let rec spaces i = if i=0 then "" else " " + spaces (i-1)
  let rec flush = function 
      | [] -> ""
      | [item] -> item
      | h::t -> (flush t) + "\n" + h

  member x.linept = lineno 
  member x.end_block = (indent <- hd indents; indents <- tl indents)
  member x.begin_block n = (indents <- indent :: indents; indent <- indent + n)

  member x.newlinef(nlf) =
      let _ =
          match emitter_callback with
          | None    -> lines  <- line :: lines
          | Some ff -> ff (line + (if nlf then "\n" else "")); 
      (line <- ""; lineno <- lineno + (if nlf then 1 else 0); idx <- 0)

  member x.newline() = x.newlinef(true)
      
  member x.add_string ss =
      let sl = strlen ss
      if idx = 0
      then (line <- spaces indent + ss; idx <- indent + sl)
      else (line <- line + ss; idx <- idx+sl)

  member x.add_break (a, b) = (if idx > maxwidth then x.newline() else x.add_string "")

  member x.force(nlf) = (x.newlinef(nlf); flush lines)
end




// Count number of leaf cells.
let record_cell (sr:statereport_t) cell =
    let (found, ov) = sr.cell_counts.TryGetValue cell
    let nv =
        if found then
             let _ =  sr.cell_counts.Remove cell 
             ov
        else 0
    sr.cell_counts.Add(cell, nv+1)


let statereport_ctor(active) =
    {
        cell_counts = new cell_count_t();
        active= active;
        vectors=   new  array_size_log_t("vector sizes")
        arrays=    new  array_size_log_t("array sizes")
        scalars=ref 0L;
        continuous=ref 0L;
//        otherbits=ref 0L;
    } : statereport_t



let report_resource_use ff com modname (sr:statereport_t) = 
    if not(sr.active) then ff(com + " no state report\n") else
    ff (com + sprintf "Structural Resource (FU) inventory for %s:" modname)
    let total = ref 0L
    let rs m p (asl:array_size_log_t) =
        let asreport k v =
            mutincu64 total (int64 k * v)
            ff(com + i2s64 v + m + i2s k + "\n");
        for z in asl do asreport z.Key z.Value done
    let rd m n = (mutincu64 total n; if n <> 0L then ff(com + i2s64 n + m + "\n"))
    rs " vectors of width " 0 (sr.vectors)
    rs " array locations of width " 0 (sr.arrays)
//  rd " other bits " (!(sr.otherbits))
    rd " bits in scalar variables" (!(sr.scalars))
    ff(com + sprintf "Total state bits in module = %i bits.\n" !total)
    rd " continuously assigned (wire/non-state) bits " (!(sr.continuous))
    let total_cells = ref 0
    let cellp k v =
        mutinc total_cells v
        ff (com + "  cell " + k + sprintf " count=%i\n" v)
    for z in sr.cell_counts do cellp z.Key z.Value done
    ff (com + sprintf "Total number of leaf cells = %i\n" !total_cells)
    ()


let g_default_rtl_fun_atts =  // Just use fsems directly instead ...
  {
      fsems=g_default_fsems
  }


// Return expression width in bits
let rec vprec ww arg =
    let def = 0 // 0 denotes unspecified and means the same as widtho=None
    let deopt = function
        | Some w -> w
        | _      -> def
    match arg with
    | V_X w                       -> w
    | V_NUM(w, signed, baser, vv) -> w
    | V_SLICE(v, _, w)            -> w
    | V_MASK(h,l)                 ->  h+1
    | V_PARAM _                   -> 32
    | V_NET(ff, _, -1)            -> encoding_width (X_bnet ff)
    | V_NET(_, _, _)              -> 1
    | V_CAT lst                   -> List.fold (fun c (count, v) -> c + count * vprec ww v) 0 lst
    | V_1sCOMPLEMENT v
    | V_2sCOMPLEMENT v
    | V_LOGNOT v
    | V_SUBSC(v, _)               -> vprec ww v
    | V_BITSEL _                  -> 1
    | V_STRING(_, ss)             -> 8 * strlen ss
    | V_CALL(cf, (nfs, _), [arg]) when nfs=g_verilog_signed_cast || nfs=g_verilog_unsigned_cast -> vprec ww arg
    | V_CALL(cf, ((f, gis), _), args) -> deopt gis.rv.widtho    
    | V_DIADIC(prec, oo, l, r)    -> deopt prec.widtho
    | V_QUERY(g, l, r)            -> max (vprec ww l) (vprec ww r)
    | V_ECOMMENT _                -> def

// Floating point 'less-than' predicate - needs to be a function for Verilog bit extract operator to work on its operands.
// Compare two floating point numbers to see if a<b: straightforward integer comparison of the mantissas works when the signs match, but is the wrong way round when 
// they are both negative. Otherwise it is obvious from the sign difference.
let fp_dltd_compare_pred w id =
    let t = w - 1
    let n = w - 2
    //  "((!a[31] && b[31]) ? 1: (a[31] && !b[31])  ? 0 : (a[31]^(a[30:0]>b[30:0])))"
    let ss:string = sprintf "((%s_a[%i] && !%s_b[%i]) ? 1: (!%s_a[%i] && %s_b[%i])  ? 0 : (%s_a[%i]^(%s_a[%i:0]<%s_b[%i:0])))" id t id t id t id t id t id n id n    
    // a > %s_b in single prec floating point if 
    ss//
// These canned RTL functions are inserted into the modules where they are used.
//
let g_hpr_rtl_canned =
  let vtype (vt:precision_t) =
      let w = valOf_or (*"vtype bus width"*) vt.widtho 32
      (if vt.signed=FloatingPoint then "/*fp*/" elif vt.signed=Signed then "signed " else "") + (if w<2 then "" else sprintf "[%i:0]" (w-1))      

                  
  [
   // Modern Verilog's do not require unique function instances if we can use the 'automatic' keyword.
    // We should add a facility to use this instead of churning out multiple identical functions (allbeit with slightly different names).
   ("hpr_dbl2flt", g_default_rtl_fun_atts,
    fun (id, dom, range) ->
        sprintf "function [31:0] %s;\n" id +
        "input [63:0] arg;\n" +
        "\n" +
        "reg          signi;\n" +
        "reg [10:0]   expi;\n" +
        "reg [51:0]   manti;\n" +
        "reg [7:0]    expo;\n" +
        "reg [22:0]   manto;\n" +
        "reg  overflow, scase_inf, scase_zero, scase_nan, fail;\n" +
        "\nbegin\n" +
        "  { signi, expi, manti } = arg;  // Deconstruct input arg\n" +
        "  scase_zero = (arg[62:0] == 63'd0);\n" +
        "  scase_inf = (expi == 11'h7ff) && (manti == 0);\n" +
        "  scase_nan = (expi == 11'h7ff) && (manti != 0);\n" +
        "// We can report fail on overflow but better to report infinity.\n" +
        "  fail = 0;\n" +
        "  overflow = (expi[10] == expi[9]) ||(expi[10] == expi[8]) ||(expi[10] == expi[7]);\n" +
        "  expo = { expi[10], expi[6:0]};\n" +
        "  manto = manti[51:51-22];\n" +
        "  scase_inf = scase_inf || overflow;\n" +
        sprintf "  %s[31]    = signi;\n" id +
        sprintf "  %s[30:23] = (scase_inf)? 8'hff: (scase_nan)? 8'hff: (scase_zero)? 8'd0: expo;\n" id +
        sprintf "  %s[22:0]  = (scase_inf)? 23'd0: (scase_nan)? -23'd1: (scase_zero)? 23'd0: manto;\n" id +
        "end\nendfunction\n\n"
   );

   ("hpr_toChar", g_default_rtl_fun_atts, // Verilator is very fussy about width of %c arguments to $write but this function pacifies.
    fun (id, dom, range) ->
    sprintf "function [7:0] %s;\n" id +
    sprintf "input [31:0] %s_a;\n" id +
    //sprintf "begin\n" +
    sprintf "   %s = %s_a & 8'hff;\n" id id +
    //sprintf "   end\n" +
    sprintf "endfunction\n\n"
    );

   ("hpr_flt2dbl", g_default_rtl_fun_atts,
    fun (id, dom, range) -> sprintf "\nfunction [63:0] %s;//Floating-point convert single to double precision.\n" id +
                            sprintf "input [31:0] darg;\n" +
                            sprintf "  %s =  ((darg & 32'h7F80_0000)==32'h7F80_0000)?\n     {darg[31], {4{darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}}:\n     {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};\n" id +
                            sprintf "endfunction\n\n"
   );

   ("hpr_fp_dltd", g_default_rtl_fun_atts, // Floating point 'less-than' predicate - needs to be a function for Verilog bit extract operator to work on its operands.
     fun (id, dom, range) ->
               let w = valOf_or (*"vtype bus width"*) range.widtho 32
               let comps = fp_dltd_compare_pred w id
               sprintf "\nfunction %s; //Floating-point 'less-than' predicate.\n" id +
               sprintf "   input [%i:0] %s_a, %s_b;\n" (valOf range.widtho - 1) id id +
               sprintf "  %s = %s; \n" id comps +
               "   endfunction\n\n"
   );

   ("hpr_fmax", g_default_rtl_fun_atts, // Floating point max
     fun (id, dom, range) ->
               let w = valOf_or (*"vtype bus width"*) range.widtho 32
               let comps = fp_dltd_compare_pred w id
               sprintf "\nfunction %s %s;// Floating-point max\n" (vtype range) id +
               sprintf "   input [%i:0] %s_a, %s_b;\n" (valOf range.widtho - 1) id id +
               sprintf "  %s = %s ? %s_b: %s_a ;\n" id comps id id +
               "   endfunction\n\n"
   );

   ("hpr_fmin", g_default_rtl_fun_atts, // Floating point min
     fun (id, dom, range) ->
      // TODO: assert range=hd dom
               let w = valOf_or (*"vtype bus width"*) range.widtho 32
               let comps = fp_dltd_compare_pred w id
               sprintf "\nfunction %s %s;//Floating-point minimum\n" (vtype range) id +
               sprintf "   input [%i:0] %s_a, %s_b;\n" (valOf range.widtho - 1) id id +
               sprintf "  %s = %s ? %s_a: %s_b ;\n" id comps id id +
               "   endfunction\n\n"
   );

   ("hpr_max", g_default_rtl_fun_atts,
      // TODO: assert range=hd dom
     fun (id, dom, range) ->
               sprintf "\nfunction %s %s;\n" (vtype range) id +
               sprintf "   input signed [%i:0] %s_a, %s_b;\n" (valOf range.widtho - 1) id id +
               sprintf "  %s = (%s_a > %s_b ) ? %s_a: %s_b ;\n" id id id id id +
               "   endfunction\n\n"
   );

   ("hpr_min", g_default_rtl_fun_atts,
//why is signed hardcoded?
     fun (id, dom, range) ->
               sprintf "\nfunction %s %s;\n" (vtype range) id +
               sprintf "   input signed [%i:0] %s_a, %s_b;\n" (valOf range.widtho - 1) id id +
               sprintf "  %s = (%s_a < %s_b ) ? %s_a: %s_b ;\n" id id id id id +
               "   endfunction\n\n"
   );

   ("hpr_abs", g_default_rtl_fun_atts,
     fun (id, dom, range) ->
          sprintf "\nfunction %s %s;\n" (vtype range) id +
          sprintf "   input %s %s_a;\n" (vtype range) id +
          sprintf "  %s = (%s_a > 0) ? %s_a: -%s_a;\n" id id id id +
          "   endfunction\n\n"
   );


   ("hpr_fabs", g_default_rtl_fun_atts, // Floating-point abs
      // TODO: assert range=hd dom
     fun (id, dom, range) ->
          let w = valOf_or (*"vtype bus width"*) range.widtho 32
          sprintf "\nfunction %s %s;//Floating-point absolute value function\n" (vtype range) id +
          sprintf "   input %s %s_a;\n" (vtype range) id +
          sprintf "  %s = { 1'b0, %s_a[%i:0] };\n" id id (w-2) +
          "   endfunction\n\n"
   );


   ("hpr_unary_encode2", g_default_rtl_fun_atts,
     fun (id, dom, range) ->
            "\nfunction [31:0] " + id + ";//Unary encode\n" +
            "   input a, b;\n" +
            "   " + id + " = (a) ? 1: (b) ? 2: 0;\n" +
            "   endfunction\n\n"
   );

   ("hpr_unary_encode3", g_default_rtl_fun_atts,
     fun (id, dom, range) ->
            "\nfunction [31:0] " + id + ";\n" +
            "   input a, b, c;\n" +
            "   " + id + " = (a) ? 1: (b) ? 2: (c) ? 3: 0;\n" +
            "   endfunction\n\n"
   );


// The motivation for rtl_signed_bitextract as a function is that bitextracts cannot, in Verilog, be applied to arbitrary expressings: just function arguments and net declarations.  A bitwise AND with mask always returns an unsigned value and applying PLI $signed() to an AND will result in a +ve number owing to the leading zeros in the top bits that have been masked to zero.

   ("rtl_signed_bitextract", g_default_rtl_fun_atts,
     fun (id, dom, range) ->
            let domw = valOf (hd dom).in_prec.widtho
            sprintf "\nfunction %s %s;\n" (vtype range) id +
            sprintf "   input [%i:0] arg;\n" (domw-1) +
            sprintf "   %s = $signed(arg[%i:0]);\n" id (valOf range.widtho - 1) +
            sprintf "   endfunction\n\n"
   );

   ("rtl_unsigned_bitextract", g_default_rtl_fun_atts,
     fun (id, dom, range) ->
            let domw = valOf (hd dom).in_prec.widtho
            sprintf "\nfunction %s %s;\n" (vtype range) id +
            sprintf "   input [%i:0] arg;\n" (domw-1) +
            sprintf "   %s = $unsigned(arg[%i:0]);\n" id (valOf range.widtho - 1) +
            sprintf "   endfunction\n\n"
   );

   ("rtl_sign_extend", g_default_rtl_fun_atts,
     // Sign extender: eg signed convert from 4 to 8 bits with " { (4){arg[3]}, arg[3:0] }"
     fun (id, dom, range) ->
            let domw   = valOf (hd dom).in_prec.widtho
            let rangew = valOf range.widtho
            let mid =
                if domw > 1 then
                   sprintf "   input [%i:0] arg;\n" (domw-1) +
                   sprintf "   %s = { {%i{arg[%i]}}, arg[%i:0] };\n" id (rangew-domw) (domw-1) (domw-1)
                else
                   sprintf "   input argbit;\n"  +
                   sprintf "   %s = { {%i{argbit}} };\n" id rangew 
            sprintf "\nfunction %s %s;\n" (vtype range) id +
            mid +
            sprintf "   endfunction\n\n"
   );

   ("rtl_unsigned_extend", g_default_rtl_fun_atts,
     // Unsigned extender: eg convert from 4 to 8 bits with " { 4'b0, arg[3:0] }"
     fun (id, dom, range) ->
            let domw   = valOf (hd dom).in_prec.widtho
            let rangew = valOf range.widtho
            let mid =
                if domw > 1 then                
                    sprintf "   input [%i:0] arg;\n" (domw-1) +
                    sprintf "   %s = { %i'b0, arg[%i:0] };\n" id (rangew-domw) (domw-1)
                else
                    sprintf "   input argbit;\n" +
                    sprintf "   %s = { %i'b0, argbit };\n" id (rangew-1) 
            sprintf "\nfunction %s %s;\n" (vtype range) id +
            mid +
            sprintf "   endfunction\n\n"
   );

              
  ]



// Canned functions are elaborated from their can here.
let g_canned_rtl_names = map f1o3 g_hpr_rtl_canned

let is_hpr_rtl_canned x = memberp x g_canned_rtl_names

let v_netdir_to_str = function
      | (V_IN) -> "input"
      | (V_OUT) -> "output"
      | (V_INOUT) -> "inout"
      | (VNT_REG) -> "reg"
      | (VNT_INT) -> "integer"
      | (VNT_WIRE) -> "wire"
      | (_) -> "netdir??"

(*
 * Parenthesis are inserted around a node if the parent context is more strongly binding than the node.
 * For non-associative operators, parenthesis are needed on the same power : eg a-(b+c) needs them but a+(b-c) strictly does not, but we do.
 *)
// Verilog Operator Precedence - Most tightly binding first
//   Reduction and unary complement       &, ~&, ^, ~^, |, ~|, !, ~
//   Unary arith                          +, -, 
//   Multiply, Divide, Modulus            *, /, %
//   Add, Subtract,                       +, -
//   Shift                                <<, >>, >>>
//   Relation,                            <, >, <=, >=
//   Logical Equality                     ==, !=, 
//   Case Equality                         ===, !==
//   Diadic conjunction                   &
//   Diadic xor, xnor                     ^, ~^
//   Diadic disjunction                   |
//   Logical AND                          &&
//   Logical OR                           || 
//   Conditional                          ? :
let vdop_table = function
      | (V_TIMES) -> ("*", 10, true)
      | (V_DIVIDE) -> ("/", 10, false)
      | (V_DMOD) -> ("%", 10, false )
      
      | (V_PLUS) -> ("+", 9, true) 
      | (V_MINUS) -> ("-", 9, false)
      
      | (V_LSHIFT) -> ("<<", 7, false)

      // Note, owing to signed numbers being a late addition to Verilog, these two operators are the other way around in Java.
      | V_ArithRSHIFT    -> (">>>", 7, false) // sign preserving
      | V_LogicalRSHIFT  -> (">>",  7, false) // unsigned - shifts in zeros
      
      | (V_DEQD) -> ("==", 6, false)
      | (V_DLTD) -> ("<", 6, false)
      | (V_DLED) -> ("<=", 6, false)

      // These three for tidy output only: not for internal AST use. 
      | (V_DNED) -> ("!=", 6, false)
      | (V_DGTD) -> (">", 6, false)
      | (V_DGED) -> (">=", 6, false)
      
      | V_BITAND  -> ("&", 5, true)
      | V_XOR     -> ("^", 4, true)
      | V_BITOR   -> ("|", 3, true)

      
      
      | V_LOGAND  -> (" && ", 2, true)
      | V_LOGOR   -> (" || ", 1, true)
      
      
//    | (_) -> sf "vdopToS"


let vdopToS a = f1o3(vdop_table a)


let verilog_binding_power (b, parent_assocf) operator = 
    let (_, son_bp, son_assocf) = vdop_table operator
    let paren_needed = b > son_bp || (((not son_assocf) || (not parent_assocf) && b=son_bp))
    (paren_needed, (son_bp, son_assocf))

let bp_top = (0, true)


let rec verilog_pp_list site_ (n:cbg_pp) ff args =
    let rec yit_oneline = function
        | []    -> ()
        | [s]   -> (ff s; ())
        | h::tt -> (ff h; n.add_string ", "; yit_oneline tt)

    let rec yit_multiline = function
        | []    -> ()
        | h::tt -> (n.add_string "\n        "; ff h; (if not_nullp tt then n.add_string ","); yit_multiline tt)

    let tabulate = length args > 4
    if tabulate then yit_multiline args else yit_oneline args
    
and verilog_pp_exp (n:cbg_pp) (b:int * bool) ddd arg =
    let vd = -1
    ////dev_println (sprintf "XY verilog_pp_exp start %i" (arg.GetHashCode()))
    let ans = 
        match arg with

        | V_MASK(h,l) ->  
              let p = i2s(h+1) + "'h"
              let hexval(pos, v) = if pos>=l && pos <= h then v else 0
              let rec z4 pos =
                    if (pos > h) then ""
                    else (z4(pos+4)) +
                                ( if pos < l-3 then "0"
                                  else i2x(hexval(pos, 1)+hexval(pos+1, 2)+hexval(pos+2, 4)+hexval(pos+3, 8))
                                )
              let ans = p + (z4 0)
              if vd >= 8 then vprintln 8 ("vmask of " + (i2s h) + ":" + (i2s l) +  " is " + ans + "\n")
              (n.add_string ans; n.add_break (0, 0))

        | V_NUM(w, signed, baser, vv) ->
            let signeds = if signed then "s" else ""
            let bnum_render (bn:BigInteger) =
                let (sg, bn) = if bn.Sign < 0 then (true, 0I - bn) else (false, bn)
                let hex_it = true//TODO use decimal under some circumstances
                if hex_it && w > 0 then
                    let rec bnToHex p n =
                        if n=0I then ""
                        else 
                            let rem = ref 0I
                            let q = BigInteger.DivRem(n, 16I, rem) // Insert an underscore every four digits.
                            bnToHex (p+1) q + (if p%4=3 then "_" else "") + [| "0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "a"; "b"; "c"; "d"; "e"; "f" |].[int(!rem)]
                    let bnToHex1 n =
                        if n=0I then "0"
                        else
                            let ss = bnToHex 0 n
                            // We can supress a leading underscore in the mantissa part of the number to make iverilog 10.2 happy.
                            if ss.[0] = '_' then ss.[1..] else ss
                    (if sg then "-" else "") + i2s w + "'" + signeds + "h" +  bnToHex1 bn
                elif (signed && (bn < -2147483648I || bn > 2147483647I))||(not signed && bn < 0I || bn >= 2147483648I * 2I) then
                        let determined_width = bound_log2 bn
                        vprintln 0 (sprintf "+++ verilog_render: out-of-range bnum %A : specified w=%i but needs width %i signeds=%A" bn w determined_width signeds)
                        // We print something anyway, but ideally this troublesome code path no longer gets exercised.
                        (if sg then "-" else "") + i2s determined_width + "'" + signeds + "d" + bn.ToString()

                else (if sg then "-" else "") + (if w > 0 then i2s w + "'" + signeds + "d" else "") + bn.ToString()

            let pp_vnum_hexp = function
                    | X_bnum(w_, bn, _) -> (n.add_string (bnum_render bn); n.add_break (0, 1))

                    | X_num v  ->
                        let sv = if v = -2147483648 then "2147483648" else i2s(abs v)
                        (n.add_string ((if v < 0 then "-" else "") + (if w <> -1 then i2s w + "'" + signeds + "d" else "") + sv);
                         n.add_break (0, 1)
                        )

                    | X_bnet ff when not_nullp ff.constval -> // Numeric constants
                        //let f2 = lookup_net2 ff.n
                        let ss = 
                            match ff.constval with
                                | XC_double dv :: _    -> // May want to render these two as hex constants via floattobits
                                    sprintf "%f" dv
                                | XC_float32 fv ::  _  -> sprintf "%f" fv

                                | XC_bnum(w, bn, _) :: _  -> bnum_render bn

                                | XC_string ss :: tt ->
                                    dev_println (sprintf "ignore/mishandle XC_string init/constant now %A" ff.constval)
                                    ss
                                | other -> sf (sprintf "unexpected verilog literal (constval) form: %A" other)
                        (n.add_string ss;  n.add_break (0, 1)
                        )

                    | other -> sf (sprintf "unexpected verilog number form: key=%s vale=%s" (xkey other) (xToStr other))
            pp_vnum_hexp vv

        | V_PARAM(ss, _) -> (n.add_string ss; n.add_break (0, 1))

        | V_NET(ff, alias, nN) ->
            if ff.id = g_tnow_string then n.add_string "$time"
            else
                let id = if alias <> "" then alias else vsanitize ff.id
                n.add_string (if (nN<0 || ff.width < 2) then id else id + "[" + (i2s nN) + "]") 
                n.add_break (0,1)
                ()

        | V_SLICE(v, s, width) -> // a vector bit select.
           (
            if (fst b > 12) then n.add_string "(";
            verilog_pp_exp n (12, false) ddd v;        
            n.add_string "[";
            verilog_pp_exp n bp_top ddd s;
            n.add_string (sprintf " +: %i]" width);
            if (fst b > 12) then n.add_string ")";
            n.add_break (0,0)
           )
        | V_SUBSC(v, s) -> 
           (
            if (fst b > 12) then n.add_string "(";
            verilog_pp_exp n (12, false) ddd v;        
            n.add_string "[";
            verilog_pp_exp n bp_top ddd s;
            n.add_string "]";
            if (fst b > 12) then n.add_string ")";
            n.add_break (0,0)
           )

        | V_LOGNOT(V_DIADIC(prec, V_XOR, l, r))  -> verilog_pp_exp n b ddd (V_DIADIC(prec, V_DEQD, l, r))

        | V_LOGNOT(V_DIADIC(prec, V_DLTD, l, r)) -> verilog_pp_exp n b ddd (V_DIADIC(prec, V_DGED, l, r))

        | V_LOGNOT(V_DIADIC(prec, V_DLED, l, r)) -> verilog_pp_exp n b ddd (V_DIADIC(prec, V_DGTD, l, r))

        | V_LOGNOT(V_DIADIC(prec, V_DEQD, l, r)) -> verilog_pp_exp n b ddd (V_DIADIC(prec, V_DNED, l, r))

        | V_LOGNOT(V_DIADIC(prec, V_DNED, l, r)) -> verilog_pp_exp n b ddd (V_DIADIC(prec, V_DEQD, l, r))

        | V_LOGNOT sx ->
          (
            if (fst b >= 13) then n.add_string "(";
            n.add_string "!";
            verilog_pp_exp n (13, true) ddd sx;
            if (fst b >= 13) then n.add_string ")";
            n.add_break (0,0)
          )

        | V_1sCOMPLEMENT sx ->
           (
            if (fst b >= 13) then n.add_string "(";
            n.add_string "~";
            verilog_pp_exp n (13, true) ddd sx;
            if (fst b >= 13) then n.add_string "(";
            n.add_break (0,0)
           )

        | V_2sCOMPLEMENT sx ->
           (
            if (fst b >= 13) then n.add_string "(";
            n.add_string "-";
            verilog_pp_exp n (13, true) ddd sx;
            if (fst b >= 13) then n.add_string "(";
            n.add_break (0,0)
           )

        | V_ECOMMENT(s, x) -> (verilog_pp_exp n b ddd x; n.add_string ("/*" + s + "*/"))
        //| V_E1COMMENT(s)   -> n.add_string ("/*" + s + "*/")    

        | V_QUERY(g, l, r) -> 
           (
           // Note extra parens are needed to assert left associativity of this operator when it is (rarely) encountered.
           // Without extra parens
           //    "a ? b : c ? d : e" can be generated by two different parse trees : "(a?b:c)?d:e" and the more common "a?b:(c?d:e)".
           //
           // TODO: this code adds the parens unconditionally at the moment since the checks need improving.
            n.begin_block 0;
            if true || (fst b > 0) then n.add_string "(";
            verilog_pp_exp n bp_top ddd g;
            n.add_string "? ";
            verilog_pp_exp n bp_top ddd l;
            n.add_string ": ";
            verilog_pp_exp n bp_top ddd r;
            if true ||(fst b > 0) then n.add_string ")";
            n.end_block
           )

        | V_DIADIC(prec_, g, l, r) ->
            let (pn, b1) = verilog_binding_power b g
            in (
                (* n.begin_block  0; *)
                if (pn) then n.add_string "(";
                verilog_pp_exp n b1 ddd l;
                n.add_string (vdopToS g);
                n.add_break (0, 1);
                verilog_pp_exp n b1 ddd r;
                if (pn) then n.add_string ")"
                (* n.end_block *)
               )

        | V_CAT(l) -> 
            let rec scat first = function
                | [] -> ()
                | (tw, h)::tt ->
                    if (not first) then n.add_string ", "
                    verilog_pp_exp n bp_top ddd h
                    scat false tt
            n.add_string "{"
            scat true l
            n.add_string "}"
            ()

        | V_STRING(hexp, s) -> (n.add_string ("\"" + s + "\""); n.add_break (0,0))
        | V_X w  -> (n.add_string (sprintf "%i'bx" w); n.add_break (0,0))
        | V_CALL(cf, ((fname, gis), _), args)  ->
            let km fname =
                let key =
                    let wrap_arg_prec x = { in_prec=x; }
                    (fname, map wrap_arg_prec gis.args, gis.rv)
                let oldone = if gis.fsems.fs_eis || ddd.ddctrl.uniquify_all_functions then None else op_assoc key !ddd.ver_apps 
                let gen_fresh_call () = 
                    let instance_name = fname + i2s(!ddd.nextbase)
                    mutinc ddd.nextbase 1
                    mutadd ddd.ver_apps (key, instance_name)
                    instance_name
                if nonep oldone then gen_fresh_call() else valOf oldone
            let kill_args = args=[] && (fname = "$finish") // Do not put unit () call on end of finish.
            let fname' = if fname.[0]<>'$' then km fname else fname

            if kill_args then n.add_string fname'
            else
               (n.add_string (fname' +  "(");
                verilog_pp_list "V_CALL" n (verilog_pp_exp n bp_top ddd) args;
                n.add_string ")"
               )

    //  | (other) -> sf (sprintf "verilog_pp_exp other form. other=%A" other)
    ////dev_println (sprintf "XY verilog_pp_exp end %i" (arg.GetHashCode()))
    ans

(*
 * b is binding tightness.
 *)
let verilog_render_exp b ddd x =
    let n = new cbg_pp(132, None)
    let _ = verilog_pp_exp n b ddd x
    n.force(false)




let g_null_ddd() = // This is a function since we require a fresh one on each call owing to ref fields.
    {
        sr=None; directory=None; ver_apps=ref []; nextbase=ref 1000; ddctrl=g_null_ddctrl
    }

let vToStr x = verilog_render_exp bp_top (g_null_ddd()) x

let ecToStr1 arg =
    let (s, args) = ecToStr arg
    sprintf "%s(%s)" s (sfold vToStr args)

let verilog_render_exp_list_paren d_1st separate_lines (contacts) =
    let (sfst, srem) = if separate_lines then ("    ", ",\n    ") else ("", ", ")
    let rec verilog_render_exp_list_paren_serf firstf = function
        | [] -> ""
        | h::t -> 
            (if firstf then sfst else srem) + "  " + verilog_render_exp bp_top d_1st h + verilog_render_exp_list_paren_serf false t

    "(" + verilog_render_exp_list_paren_serf true contacts + ")"

let verilog_render_port_list_paren ddd (regslist:regslist_t) kill_io_attr (contacts) =
    let separate_lines = length contacts > 5

    let m_replicants = ref []
    let netDeclToStr = function
        | V_NETDECL(N, delo, ff, width, alias, amax, netdir, inito_) ->
            let id = vsanitize(if alias <> "" then alias else ff.id)
            if memberp id !m_replicants then
                hpr_warn("verilog_render: suppressing duplicate net declaration " + ff.id)
                ""
            else
            mutadd m_replicants id
            // Need to put reg before signed otherwise illegal syntax.
            let regdeclf =
                match regslist.lookup ff.id with
                    | Some entry -> entry.is_reg
                    | _ -> false
            let reg_string =
                if regdeclf then
                    if not kill_io_attr && (netdir=V_IN || netdir=V_INOUT) then hpr_warn(sprintf "RTL makes sequential assignment to INPUT %s" ff.id)
                    "reg "
                else ""
            let ss = if ff.signed=FloatingPoint then "/*fp*/ " elif ff.signed=Signed then "signed " else ""

            let dir_string = if not kill_io_attr then v_netdir_to_str netdir elif regdeclf then "reg" else "wire"
            let range =
                if (width < 0) || netdir=VNT_INT then "[31:0] "
                elif width >= 2 then "[" + i2s(width-1) + ":" + "0] " else ""
            let del =
                match delo with
                    | None -> ""
                    | Some 0 -> ""
                    | Some n -> sprintf " #%i " n

            dir_string + " " + reg_string + ss + range + del + id
        | other -> sf (sprintf "Other forml L801 %A" other)
    let (sfst, srem) = if separate_lines then ("    ", ",\n    ") else ("", ", ")

    let rec verilog_render_port_list_paren_serf firstf = function
        | [] -> ""
        | V_SCOMMENT ss::tt ->
            let it = sprintf "/* portgroup=%s */\n" ss
            let it = if separate_lines then "\n" + it else it
            (if firstf then sfst else "") + it + verilog_render_port_list_paren_serf firstf tt

        | h::tt -> 
            let it = netDeclToStr h
            (if firstf then sfst else "") + it + (if nullp tt then "" else srem) + verilog_render_port_list_paren_serf false tt

    // Curently, some plugins tend to double-up on gdecl listing, so we suppress them a little here using list_once and m_replicants although the latter was not needed yet.
    "(" + verilog_render_port_list_paren_serf true (list_once contacts) + ")"


let block_sanitise l = List.filter (fun x -> not(is_vnl_skip x)) l


let rec verilog_pp_evc (n:cbg_pp) ddd = function
    | V_EVC_OR(l, r) -> 
        (
            verilog_pp_evc n ddd l;
            n.add_string "or "
            verilog_pp_evc n ddd r;
        )

    | V_EVC_POS s -> 
        (
            n.add_string "posedge "; 
            verilog_pp_exp n bp_top ddd s;
            n.add_string " "
        )

    | V_EVC_NEG s -> 
        (
            n.add_string "negedge "; 
            verilog_pp_exp n bp_top ddd s;
            n.add_string " "
        )

    | V_EVC_ANY s -> 
        (
            verilog_pp_exp n bp_top ddd s;
            n.add_string " "
        )

    | V_EVC_STAR -> 
        (
            n.add_string "* "
        )

(*
 * The k flag overcomes the dangling if ambiguity by being set if it would be
 * an error to have an nested one-handed if statement
 *)
let rec verilog_pp_bev_list (n:cbg_pp) ddd = function
    | []   -> ()
    | h::t -> ((verilog_pp_bev n ddd false h); (verilog_pp_bev_list n ddd t))

and verilog_pp_bev n ddd k arg =
    match arg with
    | V_BLOCK ((V_EVC s)::t) -> 
        //dev_println (sprintf "XY v_block with ec start");
        (
         n.add_string "@(";             
         verilog_pp_evc n ddd s;
         n.add_string ") ";
         n.begin_block 4;
         n.add_string " begin ";
         n.newline();
         verilog_pp_bev_list n ddd (block_sanitise t);
         n.add_string " end ";
         n.newline();
         n.add_break (0, 1);
         n.end_block
        )

    | (V_BLOCK k) ->
        //dev_println (sprintf "XY v_block without ec start");            
        (
          n.begin_block 4;
          n.add_string " begin ";
          n.newline();
          n.add_break (1, 0);
          verilog_pp_bev_list n ddd (block_sanitise k);
          n.add_string " end ";
          n.newline();
          n.add_break (0, 1);
          n.end_block
        )

    | V_EASC(s) -> (verilog_pp_exp n bp_top ddd s; n.add_string ";"; n.newline())

    | V_COMMENT(s) -> (n.add_string ("//" + s); n.newline())

    | V_SWITCH(g, body, flags) -> 
        //dev_println (sprintf "XY V_SWITCH block start. %i clauses" (length body))
        let rec ktag = function
            // The following clause now produces 'S' and other non-RTL syntax so comment it out.
            //| V_NUM(width, signed_, baser, nn) -> n.add_string (xiplainToStr nn + ": ")
            | tagv -> (verilog_pp_exp n bp_top ddd tagv; n.add_string ": ")
        let caseway(tags, b) =
              (
               n.newline();
               if nullp tags then n.add_string "default:" else app ktag tags;
               verilog_pp_bev n ddd false b
              )
        let flag_string =
            (if flags.full_case || flags.parallel_case then " // synthesis " else "") +
            (if flags.full_case then "full_case " else "") +
            (if flags.parallel_case then "parallel_case " else "")
        let _ = 
              (
               n.newline();
               n.add_string "case (";
               verilog_pp_exp n bp_top ddd g;
               n.add_string (")" + flag_string);
               n.add_break (0, 1);//               n.newline();
               n.begin_block 4;
               app caseway body;
               n.end_block;
               n.add_string "endcase";
               n.newline()
              )
        //dev_println (sprintf "XY V_SWITCH done")
        ()
        
    | V_IF(g, s) -> 
        (
         if k then n.add_string " begin ";
         n.add_string "if (";
         verilog_pp_exp n bp_top ddd g;
         n.add_string  ") "; 
         n.add_break (0, 1); // If there is a newline the new block needs indent ... something not pretty here!
         n.add_break (0, 1);
         n.begin_block 4;
         verilog_pp_bev n ddd true s;
         n.add_break (0, 1);
         n.end_block;
         n.add_break (0, 1)
         if k then (n.add_string " end "; n.newline());
        )


    | V_IFE(g, s, e) -> 
       (n.add_string "if (";
        verilog_pp_exp n bp_top ddd g;
        n.add_string ") ";
        n.add_break (0, 1);
        n.begin_block 4;
        verilog_pp_bev n ddd true s;
        n.add_break (0, 1);
        n.end_block;
        n.add_string " else ";
        n.begin_block 4;
        verilog_pp_bev n ddd k e;
        n.end_block
       )

    | V_NBA(l, r) -> 
       let start = n.linept
       in
       (n.add_string " ";
        verilog_pp_exp n bp_top ddd l;
        n.add_string " <= "; 
        verilog_pp_exp n bp_top ddd r; 
        n.add_string ";";
        if (n.linept > start) then n.newline(); // Doublespace if it was a large assign
        n.newline()
       )

    | V_BA(l, r) -> 
        let start = n.linept
        //dev_println (sprintf "XY V_BA blocking assignment start")
        let _ = 
            n.add_string " "
            verilog_pp_exp n bp_top ddd l
            n.add_string " = " 
            verilog_pp_exp n bp_top ddd r 
            n.add_string ";"
            if (n.linept > start) then n.newline() // Doublespace if it was a large assign
            n.newline()
        //dev_println (sprintf "XY V_BA blocking assignment done") 
        ()

    | other -> sf(sprintf "verilog_pp_bev other form: other=%A" other)


// Invoke the pretty printer handing ff as the callback
let verilog_pps_bev width indent ff D s =
   let n = new cbg_pp(width, Some ff)
   let _ = n.begin_block indent
   verilog_pp_bev n D false s
   ff(n.force(true))
   ()

let rec spaces1 i ff = if i <= 0 then () else (ff " "; spaces1 (i-1) ff)

let spaces k = spaces1 (if k=0 then 1 else k)


let verilog_pp_list_paren site (n:cbg_pp) ff  lst =
    //let nlf = length lst > 6 // Do each on new line todo!
      (n.add_string "(";
       verilog_pp_list site n ff lst;
       n.add_string ")"
      )


let rec verilog_npp_unit_summary depth (arg, idx) =
    dev_println (sprintf "verilog_npp_unit_summary (depth=%i) of %i idx=%i"  depth (arg.GetHashCode()) idx)
    let ans = 
        match arg with
            | V_LAYOUT_REGION(loid, lon, units) ->
                vprintln 0 (sprintf "V_LAYOUT_REGION()")
                app (verilog_npp_unit_summary (depth+1)) (zipWithIndex units)
            | V_INSTANCE(lon, kind, iname, overrides, contacts, ats) -> vprintln 0 (sprintf "V_INSTANCE()")
            | V_SCOMMENT ss -> vprintln 0 (sprintf "V_SCOMMENT : Scomment body is '%s'" ss)
            | V_CONT((delo, as_real_cont_assign), lhs, r) ->  vprintln 0 (sprintf "V_CONT lhs=%A" lhs)
            | V_PRAMDEF(l, Some r) ->   vprintln 0 (sprintf "V_PRAMDEF S")
            | V_PRAMDEF(l, None) -> vprintln 0 (sprintf "V_PRAMDEF N")
            | V_NETDECL(orig, delo, f, width, alias, amax, netdir, inito) ->  vprintln 0 (sprintf "V_NETDECL")
            | V_ALWAYS(s) ->   vprintln 0 (sprintf "V_ALWAYS")
            | V_INITIAL(s) ->  vprintln 0 (sprintf "V_INITIAL")
            | other -> sf(sprintf "verilog_npp_unit_summary: unexpected form: other=%A" other) 
    ans

//
// Apply new pretty print a unit.         
//
let rec verilog_npp_unit width ff dD arg =
    //dev_println (sprintf "verilog_npp_unit of %i"  (arg.GetHashCode()))
    let ans = 
        match arg with
            | V_LAYOUT_REGION(loid, lon, units) ->
                app (verilog_npp_unit width ff dD) units

            | V_INSTANCE(lon, kind, iname, overrides, contacts, ats) -> 
                let n = new cbg_pp(width, Some ff)
                n.add_string ("  " +  kind_sanitise(vsanitize kind.name) + " ")

                // In Verilog, all contacts must be associative for associative render - conduct an audit first.
                let rec pos_scan (p, a)  = function
                    | VDB_group(meta, items) ->
                        List.fold pos_scan (p, a) items

                    | VDB_formal _
                    | VDB_actual(None, _)    -> (p+1, a) // Positional count
                    | VDB_actual(Some id, x) -> (p, a+1) // Associative count

                let rec gg_render_contact force_positional = function
                    | VDB_group(meta, items) ->
                        n.add_string(sprintf " /* %s %s */" meta.pi_name meta.kind)
                        app (gg_render_contact force_positional) items

                    | VDB_actual(Some id, x) when not force_positional -> // Associative render
                        //dev_println (sprintf "verilog_render: Assoc render from %s         to bind %s" id (vToStr x))
                        n.add_string(sprintf ".%s(" (vsanitize id))
                        verilog_pp_exp n bp_top dD x
                        n.add_string(")")
                        ()

                    | VDB_formal x
                    | VDB_actual(_, x) -> verilog_pp_exp n bp_top dD x // Positional render

                let flat1 arg = // Stupid code removes opportunity to delineate port groups.
                    let rec flt = function
                        | VDB_group(_, items) -> list_flatten(map flt items)
                        | other -> [other]
                    flt arg
                let _ =
                    if not_nullp overrides then
                        let args = list_flatten(map flat1 overrides)
                        let (p,a) = List.fold pos_scan (0, 0) args
                        if p>0 && a>0 then vprintln 2 (sprintf "Note: instance %s has mixed positional and associative overrides.  p=%i a=%i.  Rendering positionally" iname p a)
                        let force_positional = p > 0
                        (n.add_string "#"; verilog_pp_list_paren "pram-overrides" n (gg_render_contact force_positional) args; n.add_string " ")
                let _ = n.add_string (iname)
                let actuals =
                    let args = list_flatten(map flat1 contacts)
                    let (p,a) = List.fold pos_scan (0, 0) args
                    let _ = if p>0 && a>0 then vprintln 2 (sprintf "Note: instance %s has mixed positional and associative contacts.  p=%i a=%i.  Rendering positionally" iname p a)
                    let force_positional = p > 0
                    verilog_pp_list_paren "instance-actuals" n (gg_render_contact force_positional) args
                n.add_string ";"
                if dD.sr <> None then record_cell (valOf dD.sr) kind.name
                ff(n.force true)
                // Have a blank spacing line if a big component instance.
                let rec inst_args_length cc = function
                    | VDB_formal _
                    | VDB_actual _ -> cc+1
                    | VDB_group(_, lst) -> List.fold inst_args_length cc lst
                let _ = if List.fold inst_args_length 0 contacts + List.fold inst_args_length 0 overrides  > 7 then ff(n.force true)
                ()

            | V_SCOMMENT ss ->
                //let _ = vprintln 0 (sprintf "Scomment body is '%s'" ss)
                (ff ("//" + ss); ff "\n")

            | V_CONT((delo, as_real_cont_assign), l, r) ->  
                //let _ = vprintln 0 (sprintf "render V_CONT +++ l=%s  as_real_cont_assign=%A r=%s " (vToStr l) as_real_cont_assign (vToStr r))
               (ff (if as_real_cont_assign then "assign " else "always @(*) ");
                (match delo with
                    | Some delay when delay >= 0 -> ff (" #" + i2s delay + " ");
                    | _ -> ()
                );
                ff(verilog_render_exp bp_top dD l);
                ff  " = ";
                ff(verilog_render_exp bp_top dD r);
                ff ";\n";
                ff "\n"
               ) 

            | V_PRAMDEF(l, Some r) ->  
               (ff "  parameter ";
                ff(verilog_render_exp bp_top dD l);
                ff (sprintf  " = ")
                ff(verilog_render_exp bp_top dD r);
                ff (sprintf  ";\n")
               ) 

            | V_PRAMDEF(l, None) ->
               (ff "  parameter ";
                ff(verilog_render_exp bp_top dD l);
                ff " = 0;\n"
               ) 

            | V_NETDECL(orig, delo, f, width, alias, amax, netdir, inito) ->
                let arrayf = amax >= 0L // We allow arrays of one location, but prefer not to generate them (e.g. removed or not created in repack recipe stage).
                let scalarf = netdir=VNT_INT
                let id = if alias<>"" then alias else vsanitize f.id
                ff ("  " + v_netdir_to_str(netdir) + 
                    (if f.signed=FloatingPoint && not scalarf then "/*fp*/ " else if f.signed=Signed && not scalarf then " signed" else "") +
                    (if delo<>None then " #" + i2s (valOf delo) + " " else "") +
                    (if scalarf || width<2 then " " else sprintf " [%i:0] " (width-1)) + id + 
                    (if not arrayf then "" else sprintf "[%i:0]" amax))
                if not_nonep inito then ff (" = " + verilog_render_exp bp_top dD (valOf inito))
                ff ";\n"

            | V_ALWAYS(s) -> 
               (ff " always ";
                //dev_println (sprintf "XY always block start");
                verilog_pps_bev width 2 ff dD s;
                ff "\n"
                //dev_println (sprintf "XY always block done");        
               )

            | V_INITIAL(s) -> 
               (ff " initial ";
                verilog_pps_bev width 2 ff dD s;
                ff "\n" 
               )

            | other -> sf(sprintf "verilog_npp_unit: unexpected form: other=%A" other) 
    //dev_println (sprintf "verilog_npp_unit of %i done."  (arg.GetHashCode()))
    ans        
(*
 *
 *)
let verilog_render_unit_vanilla msg width dd fA (u, idx) =
    //dev_println(sprintf "XY verilog_render_unit_vanilla start. msg=%s idx=%i  arg=%A" msg idx (u.GetHashCode()))
    //verilog_npp_unit_summary 0 (u, idx)
    let _ = 
        match fA with
            | None    -> verilog_npp_unit width (fun ss->()) dd u
            | Some ff -> verilog_npp_unit width ff dd u 
    //dev_println(sprintf "XY verilog_render_unit_vanilla done. msg=%s idx=%i" msg idx)
    ()
            
let verilog_render_unit_backdoor u =
    let dd = g_null_ddd()
    let m_ans = ref []
    let ff ss = mutadd m_ans ss
    verilog_render_unit_vanilla "backdoor" 132 dd (Some ff) (u, -1)
    !m_ans

let verilog_render_unit_x width dd u =
    let m_ans = ref []
    let ff ss = mutadd m_ans ss
    dev_println(sprintf "XY Resource Cons")
    verilog_render_unit_vanilla "unit_x"  width dd (Some ff) (u, -1)
    !m_ans

(*
 * Utility function, not used locally.  Non pp version - dont need anymore!
 *) 
let verilog_render_bev bev =
    let dd = g_null_ddd()
    let m_ans = ref []
    let ff ss = mutadd m_ans ss
    verilog_pps_bev 132 0 ff dd bev
    !m_ans


let g_rcont = true;

// Elide adjacent always blocks on same event control: assumes simplistic synthesiable RTL.
let rec cosmetic = function
    | a::b::tt -> 
        let gec = function
            | V_ALWAYS(V_BLOCK((V_EVC evc)::tt)) -> Some(evc, tt)
            | _ -> None

        // TODO: need to check t is non blocking ...
        let a_ec = gec a
        let b_ec = gec b
        //let _ = vprintln 0 ("Consider cosmetic " + verilog_render_unit_backdoor a)
        if a_ec<>None && b_ec<>None && fst(valOf a_ec)=fst(valOf b_ec)
        then cosmetic(V_ALWAYS(V_BLOCK(V_EVC(fst(valOf a_ec)):: snd(valOf a_ec) @ snd(valOf b_ec)))::tt) 
        else a::(cosmetic(b::tt))

    | other -> other



(*
 * Output a Verilog module (or modules) to a file.
 *)
let verilog_render_module ww0 (width, ddctrl) sr f1 regslist = function
    | V_MOD(name, newpramdefs, formals, locals, units0) ->
        let kind_name = hd(rev(split_at_dots name))
        let ww = WF 2 "verilog_render_module" ww0 ("start " + kind_name)
        let is_a_pramdef = function
            | V_PRAMDEF _ -> true
            | _           -> false
        let (pramdefs, units) = List.partition is_a_pramdef units0
        let formal_filter_reg_decls arg cc =
            match arg with 
            | V_NETDECL(orig, delo, ff, _, alias, _, dir, _) -> if dir=VNT_REG then cc else V_NET(ff, alias, -1)::cc
            | _ -> sf "verilog render module k"
        let directory = new directory_t("directory")
        let v2_make_directory arg =
            match arg with
            | V_PRAMDEF(V_PARAM(aid, xx), _) ->
                directory.add aid (V_PARAM(aid, xx), gec_X_net aid)
            | V_NETDECL(orig, delo, ff, _, alias, _, m, _) ->
                //vprintln 0 (sprintf "verilog name=%s v2_make_directory net id='%s'" name ff.id);
                directory.add ff.id (V_NET(ff, ff.id, -1), X_bnet ff)
            | V_SCOMMENT _ -> ()
            | other ->
                vprintln 3 (sprintf "Ignoring other form in net directory %A" other)

        let m_ver_apps = ref []
        app v2_make_directory (formals @ locals @ newpramdefs @ pramdefs)
        //let _ = reportx 3 "Width directory" (fun (a, (b,c)) -> a + "     " + netToStr c) (valOf directory)
        let d_1st = { g_null_ddd() with sr=Some sr; directory= Some directory; ver_apps=m_ver_apps; nextbase=ref 0; ddctrl=ddctrl } 
        let d_0th = { d_1st with nextbase = ref 0; sr=None }


        (* Whether using unit code or not do this first so side effects are caught in ver_apps *)
        //dev_println(sprintf "XY Starting cosmetic rewrite on units")
        let units' = cosmetic units
        //dev_println(sprintf "XY cosmetic adapt of %i units became %i units " (length units) (length units'))
        let uu2 = zipWithIndex units'
        //app (verilog_npp_unit_summary 0) uu2
        app (verilog_render_unit_vanilla "Zeroth pass" width d_0th None) uu2  //Zeroth PASS.
        // fails to return here?
        //dev_println(sprintf "XY Did zeroth pass of units")
        
        let function_code =
            //vprintln 3 (i2s(length !m_ver_apps) + " function applications from body of code require canned definitions inserted at head of code.")
            let funs (name, statelessf, bodyf) =
                let funs1 ((fname1, dom, range), instance_name) = if name<>fname1 then "" else bodyf(instance_name, dom, range)
                mapstring funs1 !m_ver_apps
            map funs g_hpr_rtl_canned // Write out canned functions, at most once per module if automatic Verilog.

        //dev_println(sprintf "XY Resource Write")
        let modname = kind_sanitise (vsanitize kind_name) 
        let _ = // Write resource use to main report file.
            let f2 = yout_open_report "verilog_render"
            let ff2 str = yout f2 str
            report_resource_use ff2 "" modname sr
            yout_close f2

        //dev_println(sprintf "XY Pramdef Fold")
            
        let verilog_pramdefs_to_str d_1st lst = 
            let rec fx = function
                | V_PRAMDEF(pname, ro)::tt ->  
                    let ss = "parameter " + verilog_render_exp bp_top d_1st pname
                    let ss = if nonep ro then ss else ss + "=" +  verilog_render_exp bp_top d_1st (valOf ro)
                    let ss = if nullp tt then ss else ss + ", "
                    ss + fx tt
                | [] -> ""
                | other -> sf (sprintf "other form in verilog_pramdefs_to_str: %A " other)
            fx lst

        let kill_io_attr = false // Code referring to hh_level and h_level can now be deleted.
        let ww = WF 2 "verilog_render_module" ww0 ("directories compiled for " + kind_name)
        let _ =
          (
            f1("\n\n" ); 
            f1("module " + modname);
            if not_nullp newpramdefs then (f1 " #(";  f1 (verilog_pramdefs_to_str d_1st newpramdefs);  f1 ") "); 
            if ddctrl.kandr then f1(verilog_render_exp_list_paren d_1st true (List.foldBack formal_filter_reg_decls formals [])); 
            else f1(verilog_render_port_list_paren d_1st regslist kill_io_attr formals);
            f1(";\n");
            if ddctrl.kandr then app (fun l->(verilog_render_unit_vanilla "Main pass" width d_1st (Some f1) l)) (zipWithIndex formals);  // kill_io_attr facility missing from kandr mode which is no longer needed since all tools accept Verilog 2001 these days!
            app f1 function_code; 
            app (fun l->(verilog_render_unit_vanilla "pramdefs" width d_1st (Some f1)  l)) (zipWithIndex pramdefs); 
            app (fun l->(verilog_render_unit_vanilla "locals" width d_1st (Some f1)  l)) (zipWithIndex locals); 
            app (fun u->verilog_render_unit_vanilla "units" width d_1st (Some f1)  u) (zipWithIndex units');
            report_resource_use f1 "// " modname sr;  // Write resource use to end of RTL output file.
            f1("endmodule\n\n")
          )
        let ww = WF 2 "verilog_render_module" ww0 ("finshed " + kind_name)
        ()





// Scan an RTL module to find which nets need to be declared as reg type.
//      These are the ones assigned inside an always/initial thread. Some continuous assigns use that form too.
// We also need to know the default bus range for vector nets so we know when not to emit it (which is most of the time) but that's taken from the declarations directly (directory).
//
// Scan hardware design to find nets that are assigned by 'sequential' logic, including initials, that need to be declared as 'reg' and not 'wire'
//
let rtl_reglist c arg =
    let rec augment (c:regslist_t) = function
        | V_CAT lst            -> List.fold augment c (map snd lst)
        | V_BITSEL(net, _, _)  // can only be applied to a net properl
        | V_SUBSC(net, _ )     -> augment c net
        | V_NET(ff, id, _)     ->
            let entry = { is_reg=true; is_combreg=false }
            c.add id entry
            c.add ff.id  entry // Add under both keys forms for ease!
            c
        | other -> sf (sprintf "rtl_reglist other form skipped %A" other)
    match arg with
    | V_ALWAYS  l   
    | V_INITIAL l   ->

        let rec rtl_reglist_serf c = function
            | V_EVC _ -> c
            | V_BLOCK l -> List.fold rtl_reglist_serf c l

            | V_NBA(l, r)
            | V_BA(l, r)          -> augment c l

            | V_IF(g, l) -> rtl_reglist_serf c l
            | V_SWITCH(_, l, flags) -> List.fold rtl_reglist_serf c (map snd l)
            | V_IFE(g, l, r) -> rtl_reglist_serf (rtl_reglist_serf c r) l
            | V_COMMENT _ -> c
            | V_EASC _ -> c
            // | _ -> sf ("rtl_reglist_serf other")
        rtl_reglist_serf c l

    | V_NETDECL  _   
    | V_INSTANCE _  
    | V_SCOMMENT _  
    | V_PRAMDEF  _ -> c

    | V_CONT((_, true), l, r)               ->  c
    | V_CONT((_, false), l, r)              ->  augment c l
    | a -> sf(sprintf "rtl_reglist other form %A" a)




(*
 * vernet_{io/local}_decls: Process a list of Verilog nets to get the formal and local declarations needed
 * for a Verilog output module, make approprate 'reg' declarations where behavioural assigns are made.
 * 
 * OLD pre Verilog 2000, (K+R): A net that is a regtype needs declaring as such, and if also an output we use a separate 'redec' output line (that must be listed first in some Verilogs?).
 * NEW: Verilog 2001: can do it all in one go 'output reg'.
 * The ios function returns just the I/O declarations and if a reg then any K+R style redeclaraton
 * is/was returned in the decls list, not the ios list.  Nets of type 'elab' are just removed.
 *)
let vernet_io_decls bag vvf aliases (netinfo_dir, regslist) flip arg cc =
    let xvd = -1 // Normally -1 for off
    let (width, ddd) = vvf
    let scan ff cc = 
        let f2 = lookup_net2 ff.n
        let intcond() = bag.use_integers && ff.rh = BigInteger(maxint())
        let alias = aliasfun vsanitize aliases ff 
        let mm = f2.xnet_io
        let regdeclf =  // regslist contains both sanitised and non-sanitised net names
            match (regslist:regslist_t).lookup ff.id with
                | Some entry -> entry.is_reg
                | _ -> false

        let ndef() = if nullp f2.length then VNT_REG elif intcond() then VNT_INT else VNT_WIRE
        let first =
                match mm with
                    | INPUT  -> if flip then V_OUT else V_IN
                    | OUTPUT -> if flip then V_IN else V_OUT
                    | RETVAL -> if flip then V_IN else V_OUT (* Returned value is through an extra formal *) // Delete this old RETVAL functionality please.
                    | _      -> ndef()

        let net = X_bnet ff
        let delo = None
        let wid = if intcond() then 32 else encoding_width net
        let nonio = f2.vtype<>V_VALUE || mm=LOCAL
        if xvd >= 5 then vprintln 5 (xToStr net + " considered as vernet_io_decls : " + boolToStr (not nonio))
        if nonio then
            hpr_warn("vernet_io_decls: +++ dropping non I/O net " + netToStr net)
            cc
        else //lprintln 0 (fun ()->"   vernet_io_decls: " + netToStr net + " width=" + i2s w)  
        //let signed = ff.signed = Signed
            let dec =
                let pram = false
                let wfinal = if intcond() then -1 else wid
                vprintln 3 ("   vernet_io_decls: " + netToStr net + sprintf " width final=%i" wfinal)  
                if pram then V_NETDECL(net, delo, ff, wfinal, alias, asize (f2.length) - 1L, VNT_PARAMETER, None)
                else V_NETDECL(net, delo, ff, wfinal, alias, asize f2.length - 1L, first, None)

            let second = if intcond() then VNT_INT else VNT_REG
            // In kandr-style RTL, registered outputs require two V_NETDECLs (output n; reg n;) and this provides the second.
            let redec =
                if (not ddd.kandr) || first=second then []
                elif regdeclf then 
                    if xvd >= 5 then vprintln 5 (sprintf "   vernet_local_declarations: redec: %s" (netToStr net))
                    let amax = -1L // I/O is not an array
                    [V_NETDECL(net, delo, ff, wid, alias, amax, second, None)]
                else []
            dec :: redec @ cc

    let rec doop arg cc =
        match arg with
            | VDB_formal(V_NET(ff, _, _))     -> scan ff cc
            | VDB_actual(_, V_NET(ff, _, _))  -> scan ff cc
            | VDB_group(meta, items)          ->
                // Need to keep order happy here: put the title of port group first.
                let ss = if meta.pi_name <> g_null_db_metainfo.pi_name then " pi_name=" +  meta.pi_name else ""
                let items = List.foldBack (doop) items []
                if nullp items then cc else V_SCOMMENT(sprintf " abstractionName=%s%s" meta.kind ss) :: items @ cc
            | other ->let _ = vprintln 1 (sprintf "+++vernet_io_decls: getf other: %A" other) in cc
 
    doop arg cc

(*
 * Declare local nets to a Verilog module.
 * The underlying netdir may (mostly in the past) be an I/O type owing to a net being an I/O contact (gdecl) of a lower VM, but if passed to this decls routine that needs converting as a local reg/wire/integer form.
 *)
let vernet_local_decls bag vvf aliases (netinfo_dir:netinfo_dir_t) (regslist:regslist_t) (sr:statereport_t) (gv_nets_tf:Map<v_exp_t, bool>) arg cc =
    let (width, ddd) = vvf
    let xvd = -1 // Normally -1 for off
    let fin arg cc =
        match arg with
        //| V_NUM _ ->            // ROMs are converted to numbers and confusingly can appear here.
        | arg when not_nonep (gv_nets_tf.TryFind arg) ->
            //vprintln 0 (sprintf "gv_net_replication skip %A" arg)
            cc // skip repeats if carelessly got this far.
        | V_NET(ff, alias_, _) ->            
            let f2 = lookup_net2 ff.n
            let orig = X_bnet ff
            let alias = aliasfun vsanitize aliases ff // why lookup again?

            if xvd>=5 then vprintln 5 (xToStr orig + " enter vernet_local_decls")

            let elab = function // Discard elab variables from output render.
                | V_ELAB _ -> true
                | other    -> false
            if elab f2.vtype then cc
            else
               let (found, netinfo) = netinfo_dir.TryGetValue ff.id //TODO not all bits have same delay!
               //vprintln 3 (sprintf "vernet_local_decls: Search net %s: found=%A "  ff.id found)
               let regdeclf =  // regslist contains both sanitised and non-sanitised net names
                   match regslist.lookup ff.id with
                       | Some entry -> entry.is_reg
                       | _ -> false
               let delo =
                   if regdeclf then None
                   elif found && !netinfo.phy_data<>None then Some(snd(valOf(!netinfo.phy_data))) else None
               let intcond() = bag.use_integers && (ff.rh) = BigInteger(maxint())
               let intcond_flag = intcond()
               let io_ = isio f2.xnet_io
               //vprintln 0 (sprintf "Temp debug: vernet_local_decls: net id=%s is in reglist=%b, is_io=%b is_array=%b, intcond=%b " ff.id regdeclf io_ ff.is_array (intcond()))
               let first  = if ff.is_array then VNT_REG 
                            elif intcond_flag then VNT_INT
                            elif regdeclf then VNT_REG 
                            else VNT_WIRE
               let w = if intcond_flag then 32 else encoding_width orig
               let amax = if ff.is_array then asize f2.length - 1L else -1L
               let dummy = (amax = g_unspecified_array-1L) && (not !g_repack_disable)  
               if dummy then vprintln 3 ("Ignored dummy array declaration " + ff.id)
               if amax = 0L then hpr_yikes(sprintf "Verilog render: an array with only one location: %s[0..%i]" ff.id amax)
               let dec =
                    if dummy then [] // This did previously suppress listing for I/O nets, but now we have kept lnets and gnets separate throughout this module.
                    else
                        let sparse_inc (asl:array_size_log_t) w delta = asl.add w (delta + valOf_or (asl.lookup w) 0L)
                        if xvd>=4 then vprintln 4 ("   vernet_local_decls: " + netToStr orig + sprintf " width=%i" w + (if amax > 0L then " amax=" + i2s64 amax else "") + " reg=" + boolToStr regdeclf)
                        if (ff.is_array && amax+1L < 0L) then sf ("bad size-A: " + netToStr orig)
                        let bits = (int64 w) * (if ff.is_array then amax+1L else 1L)
                        let _ = // Increment accounts.
                            if (not regdeclf) then mutincu64 (sr.continuous) bits
                            //elif w > sr.maxwidth then mutincu64 (sr.otherbits) bits
                            elif intcond_flag then mutincu64 (sr.scalars) (int64 w)
                            elif ff.is_array then sparse_inc sr.arrays w (amax+1L)
                            else sparse_inc sr.vectors w 1L
                        [V_NETDECL(orig, delo, ff, w, alias, amax, first, None)]
               dec @ cc


    let rec piop arg cc =
        match arg with
            | VDB_formal(V_NET(ff, id, nn))      -> fin (V_NET(ff, id, nn)) cc
            | VDB_actual(_, V_NET(ff, id, nn))   -> fin (V_NET(ff, id, nn)) cc
            | VDB_group(meta, items)            ->
                let ss = if meta.pi_name <> g_null_db_metainfo.pi_name then " pi_name=" +  meta.pi_name else ""
                let items = List.foldBack piop items []
                if nullp items then cc else V_SCOMMENT(sprintf " abstractionName=%s%s" meta.kind ss) :: items @ cc
            | other ->let _ = vprintln 1 (sprintf "+++vernet_local_decls: getf other form: %A" other) in cc
    piop arg cc


(*
 * Entry point to this module (a subroutine of opath_rtl_writer) : Write one or more modules to one file.
 *)
let rtl_output2 ww bag filename timescale aux_messages add_aux_reports mods =
    let vd = 3
    let cos = yout_open_out filename
    let write ss = yout cos ss
    let ff = write
    let _ =
        (
            ff("\n\n// CBG Orangepath HPR L/S System\n\n" ); 
            ff("// Verilog output file generated at " );   ff(timestamp true);  ff("\n" );
            ff("// " );             ff(version_string 0);             ff("\n" ); 
            ff("// " );             ff(!g_argstring + "\n");
            ff(timescale + "\n");
         )

    let render22 (netinfo_dir, asrf, root, vvf, hh_level, (aliases, newpramdefs, gv_nets, lv_nets), units) =
        let m0 = "rtl_output2: render22: Module name = " + root
        let _ = WF 2 m0 ww "start"
        let units = map fst units // discard layout info (if present).
        let regslist =
            let regslist = new regslist_t("regslist")// Set.empty
            List.fold rtl_reglist regslist units
#if OFF
        if false then
            reportx vd (m0 + ": Verilog Registers" (xToStr) reglist0)
        // A net may have an I/O varmode_t but is not an upper I/O to the current VM unless listed in the gdecls part.
            reportx 3 (m0 + ": termnets") (vnetToStr) termnets
            reportx 3 (m0 + ": Verilog Module I/Os") vnetToStr gv_nets
            reportx 3 (m0 + ": Verilog Local Nets")  vnetToStr lv_nets
#endif
            
#if OLD_RESORT
        let (locs, gv_nets) =
            // Locals in the gv_nets will not be rendered by vernet_io_decls so pull them out first.  Instantiated ROMs generate these currently but best not to put them in gv_nets to start with for child components.
            let lvp = function
                | V_NET(ff, _)       when ff.xnet_io = LOCAL -> true
                | _ -> false
                //| other -> sf (sprintf "lvp other %A" other)
            let (locs, gv_nets) = groom2 lvp gv_nets
            (locs @ lv_nets, gv_nets)
#endif
        let sr = statereport_ctor true
        let formal_decls = List.foldBack (vernet_io_decls bag vvf aliases (netinfo_dir, regslist) false) gv_nets  [] 
        //
        // OLD: Do a subtraction of the contacts since where a net is passed down as a contact of a lower component it gets included in both locals and contacts.
        let gv_nets_tf =
            let rec vdb_build (mm:Map<v_exp_t, bool>) = function
                | VDB_group(_, lst)  -> List.fold vdb_build mm lst
                | VDB_actual(_, v)
                | VDB_formal(v)     -> mm.Add (v, true)
            Map.empty
            // Removed on h_level basis now
            // List.fold vdb_build Map.empty gv_nets

        let l_decls = List.foldBack (vernet_local_decls bag vvf aliases netinfo_dir regslist sr gv_nets_tf) lv_nets [] 
        let module1 = V_MOD(root, newpramdefs, formal_decls, (*termnets' @ *)l_decls, units)
        let ww1 = WF 2 m0 ww "calling verilog_render_module"
        vprintln 1 ("Writing verilog module " + root + " to " + filename + " with " + i2s(length units) + " threads/gates/units")
        verilog_render_module ww1 vvf sr write regslist module1
        let _ = WF 2 m0 ww "called verilog_render_module"
        vprintln 1 ("Wrote verilog module " + root + " to " + filename + " with " + i2s(length units) + " threads/gates/units")
        ()
    app render22 mods


    app (fun x -> write("// " + x + "\n")) aux_messages  
    if add_aux_reports then ignore(render_aux_reports "//" cos)
    write("// eof (HPR L/S Verilog)\n")
    yout_close cos 
    let _ = unwhere ww
    () 



(*
 * Bug
 *    a == b?c:d   is parsing to (a==b) ? c:d
 *
 * Solution
 *    I want a == (b?c:d)
 * My parent has higher binding power than me so I need paren.
 *)


(* eof *)



