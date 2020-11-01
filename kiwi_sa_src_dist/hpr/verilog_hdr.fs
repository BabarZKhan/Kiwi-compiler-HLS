//
// CBG Orangepath. HPR Logic Synthesis and Formal Codesign System.
// HPR L/S library.
// 
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


module verilog_hdr

open Microsoft.FSharp.Collections
open System.Collections.Generic
open hprls_hdr


type v_netdir_t =
     | V_IN | V_OUT | V_INOUT | VNT_WIRE | VNT_REG | VNT_EVENT | VNT_PARAMETER | VNT_INT

type v_diadic_t =
     | V_PLUS | V_MINUS | V_TIMES | V_DIVIDE | V_LSHIFT | V_ArithRSHIFT | V_LogicalRSHIFT | V_DEQD | V_DLTD | V_DLED | V_BITAND | V_BITOR | V_LOGAND | V_LOGOR | V_XOR | V_DMOD


// Verilog did not originally support signed right numbers.  Signed right shift was later introduced with triple >>> graph.
// Java uses the operators the other way around.
     
(* The following forms are for i/o modes only and are deprecated in internal forms *)
                      | V_DGTD | V_DGED | V_DNED

type v_decl_binder_t =
    | VDB_group of abstract_hdr.decl_binder_metainfo_t * v_decl_binder_t list
    | VDB_formal of v_exp_t 
    | VDB_actual of string option * v_exp_t  // String option is the formal name for associative instancing


and v_exp_t =
|       V_NUM of int * bool * string * hexp_t (* width, signed, rendering base and value *)
|       V_MASK of int * int            // Constant number with bit positions (h..l) set to ones. For example, V_MASK(3,1) == 14

|       V_PARAM of string * net_att_t option
|       V_NET of net_att_t * string * int     // Net and presentation name: Single constant bit select of a bus or -1 for all of it or unwidth'd
|       V_CAT of (int * v_exp_t) list
|       V_SUBSC of v_exp_t * v_exp_t          // Used for array subscription (not for for single bit selection)
|       V_BITSEL of v_exp_t * int * int       // Denotes bitselect [a:b] syntax.  Both bitselect forms can only be applied directly to a net (5.2.1 IEEE Std 1364-2005)
|       V_SLICE of v_exp_t * v_exp_t * int    // Denotes the lhs[subs +: width] vector bit select syntax.
|       V_LOGNOT of v_exp_t                   // logical not, denoted with !
|       V_1sCOMPLEMENT of v_exp_t             // One's complement flips all bits, denoted with ~
|       V_2sCOMPLEMENT of v_exp_t             // Two's complement subtracts from zero, denoted with -
|       V_STRING of hexp_t * string
|       V_CALL of callers_flags_t * (native_fun_def_t * vsfg_order_t option) * v_exp_t list
|       V_DIADIC of precision_t * v_diadic_t * v_exp_t * v_exp_t
|       V_QUERY of v_exp_t * v_exp_t * v_exp_t
|       V_X of int                            // Dont-care or dont-know value with width
|       V_ECOMMENT of string * v_exp_t

type v_evc_t =
|       V_EVC_NEG of v_exp_t
|       V_EVC_POS of v_exp_t
|       V_EVC_ANY of v_exp_t
|       V_EVC_OR of v_evc_t * v_evc_t
|       V_EVC_STAR // Fully-supported @( * ) form.

type v_case_flags =
    {
      full_case: bool
      parallel_case: bool
    }
    
type v_bev_t = 
   | V_BLOCK of v_bev_t list
   | V_IF of v_exp_t * v_bev_t
   // For switch statements, the default is the entry with no tags.
   // Fullcase can be avoided using a default, but we can also be controlled with flags.
   | V_SWITCH of v_exp_t * (v_exp_t list * v_bev_t) list * v_case_flags
   | V_COMMENT of string
   | V_IFE of v_exp_t * v_bev_t * v_bev_t
   | V_EASC of v_exp_t
   | V_EVC of v_evc_t
   | V_NBA of v_exp_t * v_exp_t
   | V_BA  of v_exp_t * v_exp_t


type loid_id_t = string list // layout_zone identifier.


type verilog_cell_kind_t =
    {
        name:     string
        dims:     int * int                  // Physical layout dimensions or 0,0
        delays:   (int * int list) list      // When backannotated?
    }


type verilog_unit_t = 
   | V_INSTANCE of loid_id_t * verilog_cell_kind_t * string * v_decl_binder_t list * v_decl_binder_t list * att_att_t list// loid, kind, iname, overrides, actuals, ats.
   | V_NETDECL of hexp_t * int option * net_att_t * int * string * int64 * v_netdir_t * v_exp_t option // Fields: orig, delay, atts, encoding width, alias, array max idx, initialisation option.
   | V_CONT of (int option * bool) * v_exp_t * v_exp_t  (* int is delay, bool specifies what sort of continuous assignment to make : there are problems with function calls in one type and arrays in the other type *)

   | V_PRAMDEF of v_exp_t * v_exp_t option // This form is not needed now NETDECL has an initialisation option?

   | V_ALWAYS of v_bev_t
   | V_INITIAL of v_bev_t
   | V_SCOMMENT of string
   | V_LAYOUT_REGION of string * int * verilog_unit_t list
   | V_FILLER


type verilog_module_t = (* format is (name, newprams, formals, locals, code) *)
          V_MOD of string * verilog_unit_t list * verilog_unit_t list * verilog_unit_t list * verilog_unit_t list

let vnl_skip = V_BLOCK []

let is_vnl_skip = function
      | (V_BLOCK []) -> true
      | (other) -> false



let rec v_deblock = function
    | [] -> []
    | (V_BLOCK(a)::t) -> v_deblock(a@t)
    | other::tt       -> other :: v_deblock tt


let gen_V_BLOCK = function
    | [item] -> item
    | other -> V_BLOCK (v_deblock other)


exception Sfault_v_exp_t of string * v_exp_t;


let gec_VDB_group(meta, b) =
    match b with
        | [] -> []
        | b ->  [ VDB_group(meta, b) ]

//----------------------------------------------
open meox
open moscow

let netgut = function
    | X_bnet ff -> ff

let gec_V_NET n ff = V_NET(ff, vsanitize ff.id, n)
let g_tnow_vnet(n)  = gec_V_NET n (netgut (vectornet_w("$time", 32)))
let g_undef_vnet    = V_X 1
let g_vfalse        = V_NUM(1, false, "b", xi_num 0)
let g_vtrue         = V_NUM(1, false, "b", xi_num 1)

type netinfo_t =
    {
        net:    v_exp_t                                   // The net itself
        driver: (verilog_unit_t * loid_id_t) option ref   // may be an input in which case no driver, or driver may be added later as a buffer.
        net_static_time: int option ref
        phy_data: (int * int) option ref                  // length and delay
        uses: (verilog_unit_t * loid_id_t * int) list ref // component, region and contact number.
    }

type netinfo_dir_t = Dictionary<string, netinfo_t>


let g_unsigned_precision = { widtho=None;  signed=Unsigned; }
let g_signed_precision   = { widtho=None;  signed=Signed; }

// Floating point 'less-than' predicae
let f_hpr_fp_dltd w =
    let prec = { widtho=Some w; signed=FloatingPoint; }
    ("hpr_fp_dltd", { g_default_native_fun_sig with rv=prec; args=[ prec; prec ]})

let g_verilog_signed_cast = ("$signed", { g_default_native_fun_sig with rv=g_signed_precision; args=[g_default_prec]; })

let g_verilog_unsigned_cast = ("$unsigned", { g_default_native_fun_sig with  rv=g_unsigned_precision; args=[g_default_prec]; })

// RTL dialect style/compatibility options
type ddctrl_t =
    {
        kandr:     bool               // Verilog 2005 port declarations are used unless this is set.
        uniquify_all_functions:  bool // Functions can generally be reused if they are stateless.  In new Verilog the automatic keyword should be used to make functions that appear stateless to be stateless.  Some tools may still confuse separate calls to a common function so this flag is provided to force all functions to be used only once textually.
        use_integers: bool
    }

let g_null_ddctrl =
    {
        use_integers=            false // Have signed registers these days so don't need integers.
        kandr=                   true // Default to Verilog 2005 these days!
        uniquify_all_functions=  false
        //timescale=g_default_timescale;
    }

let ecToStr = function
    | V_EVC_NEG(arg) -> ("V_EVC_NEG", [arg])
    | V_EVC_POS(arg) -> ("V_EVC_POS", [arg])
    | V_EVC_ANY(arg) -> ("V_EVC_ANY", [arg])    
    | V_EVC_OR(l, r) -> ("V_EVC_OR...", [])
    | V_EVC_STAR     -> ("*", []) // Fully-supported @( * ) form.

let g_rtl_finessed_pli__ = [ "hpr_KppMark"; "hpr_sysexit" ] // not used afterall

let rec pli_mapper dir f lst =
      let rec m = function
          | [] -> f
          | ((l, r)::t) when dir       -> if l=f then r else m t 
          | ((l, r)::t) when (not dir) -> if r=f then l else m t 
      m lst

let g_pli_mapping = 
    [ ("hpr_writeln", "$display");
      ("hpr_printf", "$write");
      ("hpr_sysexit", "$finish");
      ("hpr_write", "$write") 
    ]

let verilog_plimap f =
    let ans = pli_mapper true f g_pli_mapping
    //let _ = vprintln 0 ("Mapped " + f + " to " + ans)
    ans

let verilog_un_plimap f = pli_mapper false f g_pli_mapping

let rec vdb_flatten arg cc =
    match arg with
        | VDB_formal(net) -> net::cc
        | VDB_actual(_, net) -> net::cc        
        | VDB_group(_, lst) -> List.foldBack vdb_flatten lst cc
    
(* eof *)
