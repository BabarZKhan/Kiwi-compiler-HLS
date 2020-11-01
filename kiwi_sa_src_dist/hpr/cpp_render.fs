(* $Id: cpp_render.fs,v 1.47 2013-04-21 07:51:26 djg11 Exp $
 *
 * HPR L/S / Orangepath: cpp-gen C, C++ and C# output
 *
 * All rights reserved. (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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

module cpp_render

open System.Numerics

open moscow
open hprls_hdr
open abstract_hdr
open meox
open yout
open abstracte
open opath_hdr
open linprog
open bevctrl
open gbuild

let tnow_macro = "(&sc_get_curr_simcontext()->time_stamp())->value()"


(* AST for some limited C++ datatypes *)
type Ansi_t = 
| Ansi_int 
| Ansi_bool
| Ansi_int64
| Ansi_uint64
| Ansi_ushort
| Ansi_ubyte
| Ansi_uint
| Ansi_string
| Ansi_void
| Ansi_struct of string * string * int 

// Active pattern that can be used to assign values to symbols in a pattern
let (|Let|) value input = (value, input)


type cpp_bexp_t =
    | CB_leaf of hbexp_t * (int * bool)
    | CB_opl of cpp_bexp_t list * string * (int * bool)


let g_autoactual = "autoactual"

let g_not_ports = [ g_autoactual; "" ]


type cppToStrLmode_t = 
    | RMODE
    | LMODE   (* lhs of an assignment (but not a subscript expression on lhs etc).  *)
    | AMODE   (* Lazy lambda mode: generate an AST and not an actual C++ expression). *)

type cppToStrFlags_t =
    | CLASSPREFIX of string
    | CPP_TO_STR_FILLER



type cppToStr_t =
    {
      mode:     cppToStrLmode_t
      flags:    cppToStrFlags_t list
      signals:  hexp_t list 
      hdrflg:   bool
      netlist:  (hexp_t * (string list * string * string * hexp_t)) list // idl id facet x
      cpp1:     bool
      use_bool: bool   // Make one-bit data use type bool (helpful for clk.pos())
      use_ansi: bool   // Use standard C types when available
      aliases:  (string * (hexp_t * string option * string option * string)) list
    }


let g_ansi_widths = [ 8; 16; 32; 64 ]

let ansi1ToStr = function
    | 1  -> "bool"
    | 8  -> "char"
    | 16 -> "short"
    | 32 -> "int"
    | 64 -> "long int"


let cpp_filename_sanitize filename = 
    let allowed_chars =  [ '.'; '+'; '-'; '_'; ]
    filename_sanitize allowed_chars filename

let strict_cpp_id_sanitize id =
    let rec kk x = // Cannot start with a number either - TODO perhaps
        match x with 
        |  []   -> []
        |  c::t ->
            let nn =
                if  c = '\\' && t<> [] then (hd t)::(kk(tl t)) 
                elif c = ':' || isAlpha c || isDigit c then c :: (kk t) // Allow colon for :: construct.
                else '_' :: (kk t)
            nn
    let ii (cl: char list) = (implode cl):(string)
    let ans = ii (kk (explode id))
    //dev_println (sprintf "Strict san to " + ans)
    ans





let ansiToStr cpp x =
    match x with
        |  Ansi_int    -> "int"
        |  Ansi_ubyte  -> if (cpp) then "unsigned char" else "byte" // SByte is signed
        |  Ansi_bool   -> "bool"
        |  Ansi_uint64 -> if cpp then "unsigned INT64" else "ulong"
        |  Ansi_ushort -> if cpp then "unsigned short" else "ushort"
        |  Ansi_uint   -> if cpp then "unsigned int" else "uint" 
        |  Ansi_int64  -> if cpp then "INT64" else "long"
        |  Ansi_string -> if cpp then "const char *" else "String"
        |  Ansi_void   -> "void"
        |  Ansi_struct(key, id, stars) -> key + " " + id + (if stars=0 then "" else if stars=1 then "*" else sf ("too many stars"))




(*
 * A half-way to c++ type...
 *)
type cpp_unit_t = 

// Why two forms here: X_bnet and X_net
    | CPP_BNETDECL of net_att_t 
    | CPP_NETDECL of hexp_t * Ansi_t // net and type

    | CPP_BEV of ast_ctrl_t * hbev_t

    | CPP_COMMENT of string

    | CPP_METHOD of 
           bool *         (* inline if set *)
           string *       (* Method name *)
           Ansi_t *       (* Return type *)
           (Ansi_t * hexp_t) list 
           * cpp_unit_t list

    | CPP_CONSTRUCTOR of 
         string option *           // Macro to use (SC_CTOR) instead of C++ heading
         string *                  // name
         cpp_unit_t list *         // parents constructor calls and  explicit calls to field uctors.
         (Ansi_t * hexp_t) list *  // Args
         cpp_unit_t list *         // Body of constructor
         string

    | CPP_CLASS of 
                string option *   // Use this macro to define the class (SC_MODULE) if wanted
                string *          // token
                string *          // name
                cpp_unit_t list * 
                string            // parents inherited

    | CPP_FILLER


type xpp_rom_inits_t = OptionStore<int, net_att_t * string * int> 
type xpp_gen_t =
    {
        stagename:  string
        shortname:  string        
        systemcf:   bool      // False if assume no SystemC or SystemCSharp - IE no just plain HLL without EDA library.
        cppf:       bool      // False if CSharp instead of C++.
        prefs:      gbuild_prefs_t
        add_aux_reports: bool // Add extra reports to bottom of rendered file.
        xtor:       string list  //
        topclass:   string   //
        ifshare:    int      // which pretty sharing mode
        write_adjunct_module: string  // Whether to write definitions of included IP blocks
        vd:         int      // Verbal diorosity debug level
        unaliased_sanitized: bool // Use install C++ instead of hpr names iin reflection API
        resets_synchronous: bool
        // Mutables
        m_includes: (string list * string) list ref // original and sanitized names
        m_rom_inits: xpp_rom_inits_t
    }


// Used when an expression needs to be preserved as a signal since passed by reference etc..
let no_signal xx =
    let xs = xToStr xx
    //let _ = vprintln 3 ("Desig (no_signal) gives " + xkey xx + " " + xs)
    gec_X_net(strict_cpp_id_sanitize(xs)) // Bypass bnet to avoid signal.Write generation!


let mk_lmode (x:cppToStr_t) = { x with mode=LMODE }
let mk_amode (x:cppToStr_t) = { x with mode=AMODE }
let mk_rmode (x:cppToStr_t) = { x with mode=RMODE }

// Make a note that a ROM has contents that will need to be rendered (in initial statements or for readmemh in future).
let note_rom_init_needed ww (gen:xpp_gen_t) (net_att:net_att_t) vnet len =
    match gen.m_rom_inits.lookup net_att.n with
        | None ->
            let _ = vprintln 2 ("cpp_render: ROMs: Noted a ROM init that will provide contents for " + net_att.id)
            gen.m_rom_inits.add net_att.n (net_att, vnet, len)
        | Some ov -> ()
        


let sysc_map_f function_name =
    match function_name with
        | "pos" | "neg" -> function_name
        | "hpr_pause"   -> "wait"
        | "wait"        -> "wait_until" // please explain this one!
        | other ->
            let sfn = strict_cpp_id_sanitize function_name
            //vprintln 3 (sprintf "cpp_render: function name %s not SystemC" function_name)
            // Route for non-SystemC calls
            if false && other.IndexOf "hpr_" = 0 then "hprls::" + sfn
            else sfn

// We may wish to use SystemC integers only for widths not representable in ANSI, as set by use_ansi.
let tyToCpp gen cp w signed =
    let msg = ""
    let ansif = (cp.use_bool && w=1) || (cp.use_ansi && memberp w g_ansi_widths)
    let ty =
        if ansif || signed=FloatingPoint then
            if (w <> 1 && signed=Unsigned) then "unsigned " + ansi1ToStr w else ansi1ToStr w
        elif not gen.systemcf then cleanexit(msg + sprintf " : Cannot render a net of this width without a SystemC-like or ac_int  multiprecision library: width=%i" w)
        else
            let bb = if signed=Signed then "sc_int" else "sc_uint" // NB floats always via ansi route
            sprintf "%s<%i>" bb w
    ty

let precToTy gen cp prec =
    let w = if nonep prec.widtho then 32 else valOf prec.widtho
    tyToCpp gen cp w prec.signed

let cpp_bn_yield width_ (bn:BigInteger) =
    let (bn, ss) = if bn.Sign < 0 then (0I-bn, "-") else (bn, "")
    sprintf "%s0x%s" (ss) (tnumToX_bn bn)

let rec cpp_xToStr_list ww (gen:xpp_gen_t) cp addcasts lst =

    let ax arg =
        let a0 = cpp_xToStr ww gen cp arg
        if addcasts then
            match ewidth "cpp_xToStr_list" arg with
                | None -> a0
                | Some w ->
                    if (w <= 32) then sprintf "((int)(%s))" a0
                    elif (w > 32 && w <= 64) then sprintf "((long long int)(%s))" a0
                    else a0
                    
        else a0
    let rec serf = function
       | []      -> ""
       | [item]  -> ax item
       | h::tt   -> ax h + ", " + serf tt
    serf lst
       
and cpp_xToStr1 ww (gen:xpp_gen_t) cp tag_net_for_debug nx =

    let deconst_scalar = function
        // TODO find better method elsehwere.
        | XC_bnum(width, bn, _) ->  cpp_bn_yield width bn
        | _ -> sf "cpp: deconst L267 L1410"



    let lambda_suffix nx1 =
        let lambda = (cp.mode = AMODE)
        if lambda then ".delayed()"
        elif memberp nx1 cp.signals && (cp.mode <> LMODE) then ".read()"
        else ""

    let rec cpp_str_cat joiner = function
        | [item] -> item
        | h::tt -> h + joiner + cpp_str_cat joiner tt

    let ans =
        match nx with
            | X_net(id,_) ->
                (if tag_net_for_debug then "N:" else "") + id

            | W_asubsc(X_bnet ff, idx, _) ->   // For arrays of signals, the .read() suffix needs be after the subscription operator.
                //let f2 = lookup_net2 ff.n
                let rec depair = function
                    | X_pair(l, r, _) -> depair r
                    | other -> other
                let ss = 
                    match op_assoc (X_bnet ff) cp.netlist with
                        | Some (idl, id, facet, net_) -> 
                            let joiner = if facet="Ointerface" then "->" else "_"
                            strict_cpp_id_sanitize(cpp_str_cat joiner idl) (* + lambda_suffix (X_bnet ff) *) // We do not want the .read on the subscript base.
                        | _ ->
                            let id = aliasfun strict_cpp_id_sanitize cp.aliases ff
                            // sfold (fst >> xToStr) cp.netlist +
                            sprintf "/*%s ub in netlist*/" ff.id + id

        
                ss + "[" + cpp_xToStr ww gen (mk_rmode cp) (depair idx) + "]" + lambda_suffix (X_bnet ff)

            | X_bnet ff -> 
                //let f2 = lookup_net2 ff.n
                if ff.id = g_tnow_string then tnow_macro
                else
                    match ff.constval with
                    | [ item ] -> deconst_scalar item
                    | items ->
                        if not_nullp items then vprintln 3 (sprintf "Assuming %s is a ROM base since has multiple constvals" ff.id)
                        match op_assoc nx cp.netlist with
                            | None ->
                                let id = aliasfun strict_cpp_id_sanitize cp.aliases ff
                                // sfold (fst >> xToStr) cp.netlist +
                                sprintf "/*%s ub in netlist*/" ff.id + id
                            | Some (idl, id, facet, net_) ->
                                let joiner = if facet="Ointerface" then "->" else "_"
                                strict_cpp_id_sanitize(cpp_str_cat joiner idl) + lambda_suffix (X_bnet ff)

            |  other ->
                dev_println (sprintf "cpp_xToStr1 Pish other form %s %s "  (xkey other) (xToStr other))
                cpp_xToStr ww gen cp other
    //dev_println ("Pish cpp_xToStr1 ans=" + ans + " from " + netToStr nx)
    if false && ans = "CVRAM14_addr.read()" then // debug code
        reportx 3 (sprintf "cpp_class: %s aliases" "mname") (fun (x, (ya, fu_name_o, pi_name_o, lname)) -> sprintf "%20s --> %60s  %i   %A %A  %s" x (netToStr ya) (x2nn ya ) fu_name_o pi_name_o lname) cp.aliases
        sf (sprintf "hit it %i" (x2nn nx))
    ans   


and arg_funnel fgis args =
    let args = 
        if fgis.needs_printf_formatting then
            replace_autoformat_string false (remove_concats args)
        else args
    args

and cpp_xb_diadic ww gen cp lst x =
  if lst = [] then sf "cpp_xb_diadic []"
  else match x with
        | (os, i, _) ->
             let rec zz n = 
                 let hl = xb_op_binding n
                 in if (hl < i) then "(" +  cpp_xbToStr ww gen cp n + ")" else cpp_xbToStr ww gen cp n 
             and n x =
                     match x with 
                     | [item] -> zz item
                     | (h::t) -> (zz h) + os + (n t)
             n lst


and cpp_x_diadic ww gen cp (p, q, os, i) =
        let hl = x_op_binding(p)
        let hr = x_op_binding(q)
        in 
        (if (hl < i) then "(" +  cpp_xToStr ww gen cp p + ")" else cpp_xToStr ww gen cp p) +
        os + (if (hr < i) then "(" +  cpp_xToStr ww gen cp q + ")" else cpp_xToStr ww gen cp q)


(*   cpp_xbToStr f (x_not(x_logxor(l, r)))
         = cpp_xToStr f (x_bdiadic(V_deqd, [l; r], false, _))
 *)
// Convert syntax tree to a string : we do not generate a target abstract syntax tree as an intermediate stage (as we do for most other output forms) we instead make a string directly.
and cpp_xbToStr ww gen cp x =
      let btc = cpp_xbToStr ww gen cp
      let tc  = cpp_xToStr ww gen cp
      match x with
          | W_bdiop(V_deqd, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, "==", (3, false))
          | W_bdiop(V_dned, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, "!=", (3, false))
          | W_bdiop(V_dltd _, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, "<", (3, false))
          | W_bdiop(V_dled _, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, "<=", (3, false))
          | W_bdiop(V_dgtd _, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, ">", (3, false))
          | W_bdiop(V_dged _, [l; r], false, _) -> cpp_x_diadic ww gen cp (l, r, ">=", (3, false))
          
          | W_bdiop(V_deqd, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, "!=", (3, false))
          | W_bdiop(V_dned, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, "==", (3, false))
          | W_bdiop(V_dltd _, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, ">=", (3, false))
          | W_bdiop(V_dled _, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, ">", (3, false))
          | W_bdiop(V_dgtd _, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, "<=", (3, false))
          | W_bdiop(V_dged _, [l; r], true, _) -> cpp_x_diadic ww gen cp (l, r, "<", (3, false))


          | W_bmux(v, X_false, X_true, _) -> cpp_xbToStr ww gen cp v
          | W_bmux(v, X_true, X_false, meo) -> if chklt(xb_op_binding v, (11, false)) then "!(" + cpp_xbToStr ww gen cp v + ")" else "!" + cpp_xbToStr ww gen cp (v)


          | W_bmux(v, X_false, conj, _) -> 
                   let (os, i, _) = xabToStr_dop V_band  
                   xbToStr_diadic_helper_lst([v; conj], xb_op_binding, cpp_xbToStr ww gen cp, os, i)
        

          | W_bmux(v, disj, X_true, _) -> 
            let (os, i, _) = xabToStr_dop V_bor  
            xbToStr_diadic_helper_lst([v; disj], xb_op_binding, cpp_xbToStr ww gen cp, os, i)
         
          | W_bmux(v, ff, tt, meo) -> "((" + cpp_xbToStr ww gen cp v + ")?" + cpp_xbToStr ww gen cp tt + ":" + cpp_xbToStr ww gen cp ff + ")"

          
          | X_true  -> if not cp.cpp1 then "true" else "1"
          | X_false -> if not cp.cpp1 then "false" else "0"
          | W_linp(v, LIN(pol, lst), _) -> 
              let v' = cpp_xToStr ww gen cp v
              let q_logor  (a, b) = "((" + a + ")||(" + b + "))" // TODO: nasty paren!
              let q_logand (a, b) = "((" + a + ")&&(" + b + "))"
              let q_dltd   (a, b) = "((" + a + ")<(" + b + "))"
              let q_dged   (a, b) = "((" + a + ")>=(" + b + "))"
              let q_deqd   (a, b) = "((" + a + ")==(" + b + "))"
              let q_dned   (a, b) = "((" + a + ")!=(" + b + "))"
              let rec k pol = function
                   | []  -> btc (if pol then X_true else X_false)
                   | [a] -> (if pol then q_dltd else q_dged) (v', tc(xi_num a))

                   | [a;b] when b=a+1 -> (if pol then q_dned else q_deqd) (v', tc (xi_num a))

                   | a::tt when pol ->
                       let c0 = k (not pol) tt
                       in q_logor (q_dltd (v', tc(xi_num a)), c0)


                   | a::b::tt when (not pol) && b=a+1 -> // deqd disjunct
                       let c0 = k pol tt
                       // never dned
                       let h = (if pol then q_dned else q_deqd) (v', tc (xi_num a))
                       in q_logor (h, c0)

                   | a::b::tt when (not pol) -> // generic range disjunction
                       let c0 = k pol tt
                       let h = q_logand (q_dged (v', tc (xi_num a)), q_dltd (v', tc (xi_num b)))
                       in q_logor (h, c0)

              k pol lst

          | W_bnode(v, lst, true, _) -> 
                let v = xi_bnode(v, lst, false)
                in if chklt(xb_op_binding v, (11, false)) then "!(" + cpp_xbToStr ww gen cp v + ")" else "!" + cpp_xbToStr ww gen cp v

          | W_bnode(oo, lst, false, _) -> cpp_xb_diadic ww gen cp lst (xabToStr_dop oo)

          | W_bdiop(oo, [d; e], false, _) ->
                let (p, q) = unsigned_correct_bdiop(d, e)
                let (os, i, _, _) = xbToStr_dop oo
                let hl = x_op_binding(p)
                let hr = x_op_binding(q)
                in (if (hl < i) then "(" +  cpp_xToStr ww gen cp p + ")" else cpp_xToStr ww gen cp p) +
                    os + (if (hr < i) then "(" +  cpp_xToStr ww gen cp q + ")" else cpp_xToStr ww gen cp q)


   (* In C and C++ we can represent orred by x<>0 *)

          | W_bdiop(V_orred, [e], false, _) ->
//              let _ = vprintln 0 "gish0"
              if cp.cpp1 && xi_width_or_fail "cpp_xbToStr orred" (e) = 1 then cpp_xToStr ww gen cp e
              else "(" + cpp_xToStr ww gen cp e + "!=0L)"
                (* muddy ("x_orred im lazy11: " + xToStr e) *)

          | W_bdiop(V_orred,  [e], true, _) ->
               (* dont use ewidth since it cant handle x_it *)  
              if cp.cpp1 && xi_width_or_fail "cpp_xbToStr orred" (e) = 1 then
                 (if chklt(x_op_binding e, (11, false)) then "!(" + cpp_xToStr ww gen cp e + ")" else "!" + cpp_xToStr ww gen cp e)
              else cpp_xbToStr ww gen cp (ix_bdiop V_dned [xgen_num 0; e] true)
              (* muddy ("x_orred im lazy11: " + xToStr e) *)
              
        (* |  cpp_xbToStr ww gen cp (x_not(v)) -> 
           if chklt(xb_op_binding v, (11, false)) then "!(" + cpp_xbToStr ww gen cp v + ")" else "!" + cpp_xbToStr ww gen cp v
           *)

          | W_bitsel(a, n, false, _) -> (cpp_xToStr ww gen cp a) + "[" + (i2s n) + "]"

          | X_dontcare -> "X"

          | W_cover(cover, _) ->
                let p = 0
                let prods0 = function                 
                    | []     -> sf "cpp_render: prods0 g_vtrue // should be unused."
                    | [item] -> deblit item
                    | lst    -> W_bnode(V_band, map deblit lst, false, g_unmeo_meo)
                    
                let sums0 = function                 
                    | []     -> sf "cpp_render: prods0 g_vtrue // should be unused."
                    | [cube] -> prods0 cube
                    | cover  -> W_bnode(V_bor, map prods0 cover, false, g_unmeo_meo)
                    
                let r0 = sums0 cover
                cpp_xbToStr ww gen cp r0

          | other      -> sf("cpp_xbToStr other:" + xbkey other + " " + xbToStr other)

and apply_args_funnel ww gen cp ff gis args =
    let ans = sysc_map_f ff + "(" + cpp_xToStr_list ww gen cp true (arg_funnel gis args) + ")"
    ans

and cpp_xToStr ww gen cp x =
    match x with
           | W_apply((ff, gis), cf_, args, _) -> 
               let ans = apply_args_funnel ww gen cp ff gis args
               //dev_println (sprintf "W_apply mapped to %s" ans)
               ans
           | X_blift e ->
               if cp.cpp1 then cpp_xbToStr ww gen cp e
               else cpp_xToStr ww gen cp (xi_query(e, xi_num 1, xi_num 0))
           | W_valof _ -> "valof {... }"

           // String forms: fill unit tailmark or withval
           | W_string(s, XS_fill n, _)     -> "\"" + insert_string_escapes false (baseup_string n s) + "\""
           | W_string(s, XS_unquoted n, _) -> baseup_string n s
           | W_string(s, XS_withval x, _)  -> cpp_xToStr ww gen cp x + "/*" + s + "*/"
           | W_string(s, _, _)             -> s

           | X_num(n)  -> i2s(n)
           | X_bnum(w, bn, _)  -> cpp_bn_yield w bn

           | X_x(X_subsc(v, idx), n, _) -> // Bit subscription needs converting ideally to the .range mechanisms in SystemC.
                let ss = cpp_xToStr ww gen cp (gec_X_x(v, n))
                ss + "[" + cpp_xToStr ww gen cp idx + "]"

           | X_net _
           | W_asubsc(X_bnet _, _, _)
           | X_bnet(_) -> cpp_xToStr1 ww gen cp false x
          
           |  W_asubsc(v, idx, _) ->  
              let rec depair = function
                  | X_pair(l, r, _) -> depair r
                  | other -> other

              let ss = cpp_xToStr ww gen cp v
              ss + "[" + cpp_xToStr ww gen cp (depair idx) + "]"

           |  X_x(p, n, _) ->  
                  if n=0 then cpp_xToStr ww gen cp p
                  // verilog_gen copes with other D-type insertion on lower values of n.
                  else if (n<>1) then sf("cpp_xToStr: other temporal power: " + xToStr x)
                  // A value of +n in RMODE means the same as a value of -n in LMODE
                  else if cp.mode=LMODE && memberp p cp.signals then
                       cpp_xToStr ww gen cp p
                       else 
                       (vprintln 0 ("+++ ERROR: cpp output: misuse of X() (non-lmode or non-signal) " + cpp_xToStr ww gen cp p);
                        cpp_xToStr ww gen cp p
                       )       


                       //|  W_query(g, te, W_query(_, te2, fe, _), _) when xToStr te = xToStr te2 -> sf ("Hit double query " + xToStr x)
           |  W_query(g, te, fe, _) ->
               // Note extra parens are needed to assert left associativity of this operator when it is (rarely) encountered.
               // Without extra parens
               //    "a ? b : c ? d : e" can be generated by two different parse trees : "(a?b:c)?d:e" and the more common "a?b:(c?d:e)".
               // TODO spot the left associativity and only generate the extra paren in that case.
               let flag = "/*" + i2s(x2nn te) + "*/"
               "((" + cpp_xbToStr ww gen cp g + ")?" + flag + cpp_xToStr_ntr ww gen cp ("VVQ1", te) + ":" + cpp_xToStr_ntr ww gen cp ("VVQ2", fe) + ")"

               
           // Temporary hack using pair for this dot access.
           | X_pair(p, q, _) ->  cpp_xToStr ww gen cp (p) + "." + cpp_xToStr ww gen cp (q)

           | W_node(prec, oo, [v], _) when oo=V_onesc || oo=V_neg -> // mondaic 
               let body = (if chklt(x_op_binding v, (11, false)) then "(" + cpp_xToStr_ntr ww gen cp ("VVNOT1", v) + ")" else cpp_xToStr_ntr ww gen cp ("VVNOT2", v))
               let kc   = (if oo=V_neg then "-" else "~")
               kc + body

           | W_node(prec, V_cast cvt_power, [W_query(g, te, fe, _)], _) ->
               // C/C++ requires casts to be multiplied into queries otherwise we have "args of ?: have different types" complaints from C++
               let nv = ix_query g (ix_casting None prec cvt_power te) (ix_casting None prec cvt_power fe)
               cpp_xToStr_ntr ww gen cp ("cast-distribute-query", nv)

           | W_node(prec, V_cast cvtf_, [arg], _) ->
               // TODO C semantics are to convert sometimes: eg int to float. So do not ignore cvtf_
               let arg1 = cpp_xToStr_ntr ww gen cp ("VVCAST", arg)
               let ty = precToTy gen cp prec
               sprintf "(%s)(%s)" ty arg1

           // lst will have length=2 for non commassoc operators.
           | W_node(prec, oo, lst, _) when oo<>V_onesc && oo<>V_neg ->
               let (os, i, _) = xToStr_dop oo
               let os = if os = ">>>" then ">>"  else os // Correct certain right shift operators for C language
               let k0 p =
                    let hl = x_op_binding(p)
                    let p' = cpp_xToStr_ntr ww gen cp ("VVL", p)
                    if chklt(hl, i)
                       then "(" +  p'  + ")"
                       else p'

               let rec kk = function
                    | [p;q] -> k0 p + os + k0 q
                    | p::q::tt -> k0 p + os + (kk(p::tt))
                    | other -> sf("cpp_render: kk: nullary W_node " + xToStr x)
               kk lst  

            | X_iodecl(id, hi, low, ff) ->
                let f2 = lookup_net2 ff.n
                "x_iodecl(" + id + "," + i2s(hi) + "," + i2s(low) + varmodeToStr(f2.xnet_io) + ")"

            | X_undef -> "0/*UNDEF*/"


           //| X_subsc(a, n) -> (cpp_xToStr ww gen cp a) + "[" + (cpp_xToStr ww gen cp n) + "]"
   
            |  other -> sf("cpp_xToStr other: " + xToStr other)


and cpp_xToStr_ntr ww gen cp (msg, x) =
    let ans = cpp_xToStr ww gen cp (x)
    //dev_println (sprintf "cpp_xToStr: Returned %s for %s" ans (xToStr x))
    ans

and cpp_xToStr_list_paren ww gen cp contacts =
    let rec serf firstf = function
        | [] -> ""

        | (ty, X_bnet ff)::tt -> 
             let kp n = if n=0L then "[]" else "[" + i2s64 n + "]"
             let f2 = lookup_net2 ff.n
             let id = aliasfun strict_cpp_id_sanitize cp.aliases ff
             (if firstf then "" else ", ") + ansiToStr cp.cpp1 ty + " " + id
             + (sfold kp (f2.length)) + serf false tt

        | (ty, X_net(id, _))::tt -> 
            (if firstf then "" else ", ") + ansiToStr cp.cpp1 ty + " " + strict_cpp_id_sanitize id
            + serf false tt

        | _::tt  -> sf("cpp_xToStr_list_paren other form")

    "(" + serf true contacts + ")"



let cpp_unitToStr_summary x =
    match x with
    | CPP_BNETDECL(ff) -> ff.id
    | CPP_NETDECL(id, t) -> netToStr id
    | _ -> "??unit other"



// Kludge: catenate a sequence of calls with commas instead of semicolons
// as used in parent constructor calls
let uctor_ref lst =
    let chng (s:string) =
        let l = String.length(s)
        in (if l>3 && s.[l-2] = ';' then s.[0..l-3] else s)
    in sfold chng lst
;


(*
 * Rather than use a C++ AST we print hbev in C++ form.
 *)
let rec (* TODO use prettyprinter library? *) cpp_render_unit ww gen n (cp:cppToStr_t) x =
    match x with
    | CPP_COMMENT s ->  "// " + s + "\n"

    | CPP_BEV(ast_ctrl, hbev) ->  (* ef holds if a dangling else must be eliminated *)
        let rec i n = if n=0 then "" else "  " + i (n-1)

        let rec cpp_xToStr_commalist ww (gen:xpp_gen_t) cp x =
          match x with
              | [] -> ""
              | [item] -> cpp_xToStr ww gen cp item
              | h::t   -> (cpp_xToStr ww gen cp h + ", " + cpp_xToStr_commalist ww gen cp t)

        let rec ka (ef, nn) x =
          let vv_assign nn lhs rhs =
              let sigf = memberp lhs cp.signals
              let kw =
                  if not sigf then " = "
                  elif true || nn = 0 then ".write" // The X_n nn disparity has already been countenanced.
                  else muddy ("bad X_x vv_assign n=" + i2s nn)
              (i n) +  "  " + (cpp_xToStr ww gen (mk_lmode cp) lhs + kw + "(" + cpp_xToStr ww gen cp rhs + ");\n")

          match x with
              | Xlinepoint(lp, s) -> kk (ef, n) s
              | Xlabel s -> (i n) +  strict_cpp_id_sanitize s + ":\n"

              | Xassign(X_x(l, n, _), X_x(r, m, _)) -> vv_assign (n-m) l r
              | Xassign(X_x(l, n, _), r)            -> vv_assign n l r
              | Xassign(l, X_x(r, m, _))            -> vv_assign -m l r
              | Xassign(l, r)                       -> vv_assign 0 l r

              
              | Xreturn X_undef    -> (i n) +  "  return;\n"
              | Xreturn l          -> (i n) +  "  return (" + cpp_xToStr ww gen cp l + ");\n"
              | Xbeq(l, dest, _)   -> (i n) +  "  if (!(" + cpp_xbToStr ww gen cp l + ")) goto " + strict_cpp_id_sanitize dest + ";\n"

              | Xif(guard, v, Xskip)   ->
                  if ef 
                  then (i n) +  " {  if (" + cpp_xbToStr ww gen cp guard + ") "  + kk (false, n+1) v + "}"
                  else (i n) +  "  if (" + cpp_xbToStr ww gen cp guard + ")\n"  + kk (false, n+1) v

              | Xswitch(e, lst) ->
                  let sh (a, cmd) c = "\n" + i n + sfold (fun a -> "case " + cpp_xToStr ww gen cp a + ":") a + "\n" + i n + kk(false, n+1) cmd + i n + "break;" + c
                  let bod = List.foldBack sh lst ""
                  (i n) +  "  switch (" + cpp_xToStr ww gen cp e + ")" + i n + " {\n "  + bod + " }"
              | Xif(l, vt, vf) -> (i n) +  "  if (" + cpp_xbToStr ww gen cp l + ")\n"  + kk (true, n+1) vt + "\n" + i n + " else " + kk (false, n+1) vf
              | Xcontinue -> (i n) + " continue;"
              | Xcomment s -> if String.length s > 3 && s.[0..2] = "///" then "" else (i n) + "// " + s + "\n"

              | Xbreak         -> (i n) + " break;"
              | Xwhile(l, v)   -> (i n) +  "  while (" + cpp_xbToStr ww gen cp l + ") "  + kk (false, n+1) v
              | Xblock l -> "\n" + (i n) +  "{"  + mapstring (kk (false, n+1)) l + "\n" + (i n) + "}\n"
              | Xgoto(dest, _) -> (i n) +  "  goto " + strict_cpp_id_sanitize dest + ";\n"
              | Xskip -> (i n) +  "  /*skip*/;"
//            | Xcall((ff, gis), args) -> (i n) + apply_args_funnel ww gen cp ff gis args + ";\n"
(*        | (v as Xeasc(x_apply((ff, gis), args))) -> (i n) + (sysc_map_f ff) + "("  + cpp_xToStr_commalist gen cp (af args) + ");\n"
*)
              | Xwaituntil e    ->   (i n) + " wait_until (" + cpp_xbToStr ww gen (mk_amode cp) e + ");\n"
              | Xeasc e         -> cpp_xToStr ww gen cp e + ";\n"
              | Xannotate(_, v) -> kk (ef, n) v
              | other -> sf("cpp_bev render other: " + hbevToStr other)
                
        and kk (ef, n) v = 
           ( (* vprint(0, "ka cpp " + hbevToStr v + "\n"); *)
           ka (ef, n) v)
           
    
        kk (false, n) hbev

    | CPP_CLASS(Some macro, token, name, units, parents) ->
        if cp.hdrflg || (not cp.cpp1) then "\n" + macro + "(" + strict_cpp_id_sanitize name + ")" +  "\n{\n" + mapstring (cpp_render_unit ww gen (n+1) cp) units + "};\n\n"
        else noninline_methods ww n gen cp x

    | CPP_CLASS(None, token, name, units, parents) ->
       if cp.hdrflg || (not cp.cpp1) then 
         "\n" + token + "  " + strict_cpp_id_sanitize name + " " + parents + "\n{\n" +
         mapstring (cpp_render_unit ww gen (n+1) cp) units +
         "};\n\n"
       else noninline_methods ww n gen cp x

    | CPP_BNETDECL(ff) ->
        let net = X_bnet ff
       //let id = strict_cpp_id_sanitize(xToStr net)
        let f2 = lookup_net2 ff.n
        let signalf = varmode_is_io(f2.xnet_io) || memberp net cp.signals
        let (idl, id_, facet, x) = valOf_or (op_assoc net cp.netlist) ([ff.id], "spare", "", net)
(*
        let idl = map strict_cpp_id_sanitize idl
       
        let id = if facet="Ointerface" then (hd(rev idl))
                elif idl <> [] then underscore_fold (rev idl)
                else id + "/*nln*/"
*)
        let id = aliasfun strict_cpp_id_sanitize cp.aliases ff
        let array_length = asize(f2.length)
        let rec i n = if n = 0 then "" else "  " + i(n-1)
        let star = if array_length=0L then "*" else ""
        let bra = if array_length > 0L then "[" + (if cp.cpp1 then i2s64 array_length else "") + "]" else ""
        let cpp_ty = tyToCpp gen cp ff.width ff.signed 
        let sigwrapped =
            let ws1 = "< " + cpp_ty + " >"
            if f2.xnet_io=INPUT then    "sc_in"  + ws1
            elif f2.xnet_io=OUTPUT then "sc_out"  + ws1
            elif not signalf then cpp_ty
            else                        "sc_signal" + ws1

        let (ty, cargs) =
            if true || signalf then ((if cp.cpp1 then "" else "public ") + sigwrapped, "") else (cpp_ty, "")
        let init = // malloc it always needed for C# arrays wereas C++ has arrays in object record.
            if cp.cpp1 || not ff.is_array then ""
            else
               let signal_cargs =
                   if not signalf then ""
                   else sprintf "(%i, \"%s\")" array_length id
               " = new " + ty + signal_cargs + " [ " + i2s64 array_length + "]"
        (i n) + ty + " " + star + (if cp.cpp1 then id + bra  + cargs else  cargs + bra + id)  + init + ";\n"
 
    | CPP_NETDECL(X_net(id, _), t) ->
       let id = strict_cpp_id_sanitize id
       let rec i n = if n=0 then "" else "  " + i (n-1)
       (i n) + ansiToStr cp.cpp1 t + " " + id + ";\n"

    | CPP_METHOD(inlinef, fname, rt, contacts, units) ->
       let rec i n = if n=0 then "" else "  " + i (n-1)
       let rec classprefix x =
           if not cp.cpp1 then ""
           else
           match x with
               | [] -> ""
               | CLASSPREFIX s::tt -> s + "::"
               | _:: tt            -> classprefix tt
       let siga = "\n" + i n + " " + ansiToStr cp.cpp1 rt + " " + classprefix(cp.flags) + strict_cpp_id_sanitize fname + cpp_xToStr_list_paren ww gen cp contacts
       if (not cp.cpp1) || inlinef= cp.hdrflg then
                      siga + "\n"
                      + i n + " { \n"  
                      + mapstring (cpp_render_unit ww gen (n+1) cp) units
                      + i n + "}\n"
       else siga + ";\n"
   
 
    | CPP_CONSTRUCTOR(macro0, id, explicit_calls, args, units, calls) ->
        let ecalls = map (cpp_render_unit ww gen 0 cp) explicit_calls
        let rec i n = if n = 0 then "" else "  " + i (n-1)
        let id1 = (if cp.cpp1 then "" else "public ") + strict_cpp_id_sanitize id

        in "\n" + i n + id1 + cpp_xToStr_list_paren ww gen cp args
         + (if explicit_calls <> [] then i n + ": " + uctor_ref(ecalls) + "\n" else "")
         + i n + " { \n"  
         + mapstring (cpp_render_unit ww gen (n+1) cp) units
         + i n + "}\n"

    | other -> sf (sprintf "cpp_render_unit other form %A" other) 

and noninline_methods ww n gen cp x = // they are all inline in C#
    match x with
     | CPP_CLASS(_, token, name, units, parents) ->
        let classprefix ss (cp:cppToStr_t) = { cp with flags=(CLASSPREFIX ss) :: cp.flags }
        let cp' = classprefix (strict_cpp_id_sanitize name) cp
        let rec kk x =
            match x with
                | (CPP_METHOD(false, fname, rt, contacts, units)) -> cpp_render_unit ww gen n cp' x
                |  _ -> ""  (* non-inline constuctors not supported *)
        mapstring kk units

     | other -> "noninline_methods"


//
// Write C++ or C# to a file
//     
let cpp_to_file ww (gen:xpp_gen_t) ((filename, suffix), hdrflg, design, netlist, signals, aliases, includes) = 
    let cp =
        { mode=      RMODE
          netlist=   netlist
          signals=   signals
          flags=     []
          hdrflg=    hdrflg && gen.cppf
          cpp1=      gen.cppf
          use_bool=  true
          use_ansi=  false
          aliases=   aliases
        }
    let s = mapstring (cpp_render_unit ww gen 0 cp) design
    let write1() = 
                let fname = cpp_filename_sanitize filename 
                let filename =
                    let where = !g_obj_dir
                    if where <> "." then path_combine(where, fname) else fname

                let mname = strict_cpp_id_sanitize filename 
                let cos = yout_open_out(filename + suffix)
                let mo ss = yout cos ss
                let _ = mo(      "// " + mname + "\n" +
                                 sprintf "// Generated by CBG HPR L/S stagename=%s\n" gen.stagename +
                                 "//" + version_string 0 +
                                 "// " + timestamp true + "\n" +
                                 "// " + !g_argstring + "\n\n")
                let ziginc n = if gen.cppf then "#include \"" + n + ".h\"" else "using " + n + ";"
                let _ =
                    if (not cp.cpp1) then app (fun (vlnv_, san) -> mo(ziginc san + "\n")) (includes)
                    elif (hdrflg) then 
                         (mo("#ifndef " + mname + "_H\n");
                          mo("#define " + mname + "_H\n");
                          mo("#define HPR_NEEDS_SYSTEMC 1\n");
                          app (fun (vlnv, san) -> mo(ziginc san + "\n")) (includes)
                          )
                    else mo(ziginc fname + "\n\n")
                
                mo s
                if hdrflg then mo "#endif\n"
                let _ =
                    if gen.add_aux_reports then
                        mo "/*"
                        ignore(render_aux_reports "//" cos)
                        mo "*/" 
                let _ = mo ("// eof " + mname + "\n")
                let _ = yout_close cos
                ()
    let _ = write1 ()
    vprintln 1 ("C#/cpp_render " + filename + " write finished.\n\n")


// 
// Return true if a statement should not be permuted.
let rec osensitivep signals arg =
    let rec osp = function
        | Xassign(l, r) ->
            if memberp l signals then false
            else true  // blocking assignments are order sensitive

        | Xeasc(_) -> true  (* pli calls normally have side effects *)
        | Xcomment _ -> false
        | Xblock l      -> List.fold (fun b bev -> b || osp bev) false l
        // | V_SWITCH(_, l) -> List.fold (fun b (tag, bev) -> b || osp bev) false l
        | Xif(g, ct, cf) -> osp ct || osp cf
        | other -> (vprintln 1 ("+++cpp osensitivep other. evc? " + hbevToStr other + " \n"); true)
    osp arg


//
// Tidy up presentation of a list of RTL by introducing a switch/if nesting structure.
//
let new_ifshare_cpp ww (gen:xpp_gen_t) rst_net cens signals resetgrds lst =
    let vd = 3
    let sharep_none = (gen.ifshare=0)
    //if sharep_none then vprintln 1 ("Output ifshare set to none.")
    let k0 = [] // for now!
    let cvt  x = (*xiToRtl ww p *) x
    let bcvt x = (*beqnToL ww p k0 *) (* xi_simplif X_undef "verilog_gen bcvt" *) x
    let gen_ifl (g, ttlst, fflst) = gec_Xif (bcvt(deblit g)) (gec_Xblock ttlst) (gec_Xblock fflst) 

    //let gen_finish_with_conjunction (on_, guards, cmd, reset) = gen_Xif1(bcvt (List.fold (fun c a -> ix_and c (deblit a)) X_true (guards)), cmd)
    let gen_finish_with_conjunction     (on_, guards, cmd, reset) = gec_Xif1 (bcvt (ix_andl (map deblit guards))) cmd
    
    //let rec finish complainf = function
    let rec gen_finish complainf = function
        | (on, [], cmd, rst) -> cmd
        | (on, h::t, cmd, rst) ->
            let h = deblit h
            (
                if complainf then dev_println ("+++ finish work had repeated guard " + xbToStr h);
                gec_Xif1 (bcvt h) (gen_finish complainf (on, t, cmd, rst)) 
            )

    if not_nullp resetgrds then reportx 3 "Reset guards for cpp ifshare poly" xbToStr resetgrds

    let gen_case (dispatch_exp, lst) =
        let switch = Xswitch(cvt dispatch_exp, map (fun (a, b) -> (map cvt a, gec_Xblock b)) lst)
        switch
                                          
        
    let lst' =
        let flatten (g, cmds, reset) cc = map (fun cmd->(g, cmd, reset)) cmds @ cc
        List.foldBack flatten lst []
    if sharep_none then
        let k0 = []
        let fwc (gl, cmd, reset) = gec_Xif1 (bcvt(ix_andl gl)) cmd
        //app (fun (g,cmd,reset)->if vd>=4 then vprintln 4 ("Ordering at ifshare:" + hbevToStr cmd)) lst'
        map fwc lst'

    else 
        let sharep_simple = (gen.ifshare=1)
        let ans = new_ifshare_poly ww (sharep_simple, gen_finish, gen_finish_with_conjunction, osensitivep signals, gen_ifl, Xskip, gen_case, cens, resetgrds) lst'
        if vd>=4 then vprintln 4 "new_ifshare done"
        ans 



let cpp_ifshare gen lst =
    let bypass = false
    if bypass then
        hpr_yikes ("cpp ifshare is bypassed - nastier looking code likely to be generated.")
        lst
    else
        let ast_ctrl = { g_null_ast_ctrl with id=funique "cpp_ifshare" }
        let rec cpp_ifshare1 sofar x = // This diverts to hbev's ifshare code
            match x with
                | []                -> [CPP_BEV(ast_ctrl, gec_Xblock(hbev_ifshare(rev sofar)))]
                | CPP_BEV(_, v)::tt -> cpp_ifshare1 (v::sofar) tt
                | other::tt         -> CPP_BEV(ast_ctrl, gec_Xblock(hbev_ifshare(rev sofar))) :: other :: cpp_ifshare1 [] tt


        cpp_ifshare1 [] lst 




(*
 * Scan a method body to work out its return type.
 *)
let rec rt_scan2 x =
    match x with 
    | (Xreturn X_undef) -> None
    | (Xreturn v) -> Some v
    | (Xblock l) -> foldl (fun(a,b)->if b<>None then b else rt_scan2 a) None l
    | (Xannotate(_, s)) -> rt_scan2 s
    | (_) -> None


let rec rt_scan1 x =
    match x with
        | [] -> None
        | CPP_BEV(ast_ctrl, code)::t -> 
             let r = rt_scan2 code
             if r <> None then r else rt_scan1 t
        | CPP_COMMENT _ :: t   -> rt_scan1 t
        | CPP_NETDECL _ :: t   -> rt_scan1 t
        | CPP_BNETDECL _ :: t  -> rt_scan1 t
        | CPP_CLASS _ :: t     -> rt_scan1 t
        | CPP_METHOD _ :: t    -> rt_scan1 t
        | other -> sf ("rt scan other than CPP_BEV" )

let rt_scan x = 
    let v = rt_scan1 x
    if v=None then Ansi_void
    else Ansi_int (* for now *)


let rec ctype msg x =
    match x with 
        | X_bnet ff -> 
            let f2 = lookup_net2 ff.n
            if f2.vtype = V_SPARAMETER then Ansi_string
            else
                let w = encoding_width x
                if w > 64 then cleanexit(msg + ": Variable is too wide for Ansi C++ output but should be fine in SystemC output mode: " + netToStr x)
                elif w > 32 then Ansi_uint64
                elif w > 16 then Ansi_uint
                elif w > 8 then Ansi_ushort
                elif w > 1 then Ansi_ubyte
                elif w = 1 then Ansi_bool // Should use a bool for clock nets owing to clk.pos() calls.
                else Ansi_uint

        | other -> sf(msg + " ctype other " + xToStr other)


let target_env_includes gen =
    let includes = if gen.cppf then [ "hprls" ] else [ "SystemCsharp" ]
    map (fun x -> ([x], x)) includes

let encTocpp cp msg nx =
    match nx with
        | X_bnet ff -> CPP_BNETDECL ff
        | other     -> sf (msg + " : encTocpp other " + xToStr other)
(*
 *
 *
 * We iron out three forms of parallelism here arising respectively from SP_par, H2BLKs in parallel and
 * the presence one or more child machine.  The child machines have different var scopings
 * but actually all identifiers are part of a flat space anyway.  
 *
 * Currently we put child machines as separate SC_MODULES and place exec code as
 * seperate SC_THREADs in each module. 
 *
 * For a server, the first exec is an entry point, remotely callable and for a client
 * the first exec makes upcalls to a remote entry point.  The remaining execs are
 * autonomous, helper processes.
 *)
type cdb_group_t =
    | CDB_group of decl_binder_metainfo_t * cdb_group_t list
    | CDB_leaf of string * hexp_t
     
let gec_CDB_group (meta,b) = if nullp b then [] else [CDB_group(meta,b)]


let ftag_fun assoctag =
    match formaltag_handler false assoctag with
        | Some ftag -> ftag
        | None -> sf "ftag expected"


let cpp_cvt_flatten (skip_params, max_depth) cx io_select_ctrl lst =
    //let _ = dev_println (sprintf "cpp_cvt_flatten input %A" lst)
    let rec cvt depth ml arg cc =
        match arg with
        | DB_group(meta, lst) ->
            let is_pram = (meta.form=DB_form_pramor) // Those where meta.form=DB_form_meta_pram are not RTL rendered (but could usefully put in comments)
            let skip = meta.norender || // norender is used for internal nets in behavioural internal model of library cell, such as RAM.
                       (depth >= max_depth) ||
                       (is_pram && skip_params=Some true) ||
                       (not is_pram && skip_params=Some false)
            if skip then cc
            else
                let b = List.foldBack (cvt (depth+1) (meta::ml)) lst []
                gec_CDB_group(meta, b) @ cc


        | (Let None (assoctag, DB_leaf(Some kk, None))) // Formal with no assoc tag - None is bound to assoctag with this 'Let' construct.
        | DB_leaf(assoctag, Some kk) -> // Formal/actual pair.  
            //let _ = dev_println (sprintf "cvt flatten encounter %s" (netToStr kk))
            if not_nonep io_select_ctrl && hexpt_is_io kk <> valOf io_select_ctrl then cc // Skip some decls if we want locals and vice versa. 
            else
                CDB_leaf(ftag_fun assoctag, cx kk) :: cc
    List.foldBack (cvt 0 []) lst []


// Filter predicate for which nets to convert to C.
let funidvar x =
    match x with
        | X_bnet ff ->
            let f2 = lookup_net2 ff.n
            if ff.width= -1 // if a string
               || f2.vtype = V_VALUE // or a valuetype
               then true
               else (vprintln 3 ("Not converting " + netToStr x  + " to cpp field"); false)
        | x -> (vprint 3 ("Not converting " + xkey x  + " to cpp field\n"); false)

(* joining comment: This is a target-side transactor for now: TODO create initiators.... *)



// Determine which nets are signals.
// We have VHDL-like semantics for SystemC: the form of assignment is controlled by lhs being signal or variable, rather than having debib generate two forms of assignment operator.
// We need a signal if using synchronous logic and for all I/O nets which includes our own I/O and nets connected to children instances
// There's little room for non-signals.
let sigstuff ww gen anal_results netlist contacts_down = 

    // No variables written are turned into signals! ?
    let signalfinder_not cc _ = cc

    // In the RTL style, all variables written are turned into signals.
    // But in the behavioural style they are not.
    let signalfinder cc (rtl_assigned__, arg) =
        let sff2 = function
            | X_x(v, _, _) -> v // Combinational assigns to local vars can be non-signal. But we ignore that right now.
            | v            -> v
        let sff1 cc (net, lanes_) = singly_add (sff2 net) cc
        match arg with
            | DRIVE(l, _) -> List.fold sff1 cc l
            | _ -> cc


    let own_io_signal cc = function
        | (X_bnet ff, (idl, id_, facet, x)) ->
            let f2 = lookup_net2 ff.n
            if varmode_is_io f2.xnet_io || facet="Ointerface" || memberp (X_bnet ff) contacts_down then
                singly_add x cc
            else cc

    let other_signals =
        let notarray cc = function
            | (_, (idl, id, facet, X_bnet ff)) when false && ff.is_array -> cc // Arrays can be signals now  
            | (_, (idl, id, facet, a)) -> a :: cc
        if gen.cppf then List.fold signalfinder [] anal_results
        else List.fold notarray [] netlist

    let signals = map fst netlist // lst_union (List.fold own_io_signal [] netlist) other_signals

    let _ = reportx 3 "cpp_render: hpr_signals " netToStr signals
    let _ = reportx 3 "cpp_render: netlist directory" (fun (flatid, (idl, id, y, z))->rdot_fold idl + ":" + y + ":" + netToStr z) netlist
    signals



let netlist_scan ww gen msg xtop =
    let m_anal_results = ref []

    let rec mandrake aliases idl_ net =

        match net with
            | X_bnet ff ->
                //let f2 = lookup_net2 ff.n
                let id = aliasfun strict_cpp_id_sanitize aliases ff // <- this needs callee alias map - but the formal should be in there ml_ field?
                [ id ]
            | _ ->
                hpr_yikes (sprintf "Other net in mandrake " + netToStr net)
                [ xToStr net ]
(*
        match idl with
            | [] -> [ s ] //  Get a fuller pathname ... split s at underscores ?
            | h::t -> 
                let sh = String.length h
                let ss = String.length s 
                if sh+1<ss && s.[0..sh-1]=h && s.[sh]='_' then h :: mandrake(s.[sh+1..], t) else [ s ] 
*)
    let rec contacts_down_scan arg cc =
        match arg with
            | DB_group(_, lst) -> List.foldBack contacts_down_scan lst cc
            | DB_leaf(Some formal, Some actual) -> actual ::cc
            | DB_leaf _ -> cc

    let rec scan_serf topf depth prefix (cc, cd, ce) = function  
        | (iinfo, design) when not topf && iinfo.definitionf ->
            vprintln 2( sprintf "skipping definition of " + iinfo.iname)
            (cc, cd, ce)
        | (iinfo, design) ->
            match design with
                | None -> (cc, cd, ce)
                | Some(HPR_VM2(minfo, decls, children, execs, assertions)) ->
                    let prefix = iinfo.iname :: prefix
                    let flat = List.foldBack (db_flatten []) decls []
                    let idl = [ protocols.vlnvToStr minfo.name ]
                    let facet = valOf_or (at_assoc "facet" minfo.atts) ""
                    let aliases = mine_decl_aliases ww false decls
                    let queries =
                                { g_null_queries with
                                    full_listing=true
                                }
                    app (anal_block ww (Some m_anal_results) queries) execs
                    let ce = List.foldBack contacts_down_scan decls ce
                    let (cc, cd) =
                        let cc = 
                            let fg (ml_, net) cc =
                                let idl = mandrake aliases idl net
                                //dev_println (sprintf "scan_serf netlist setup: %s %s adding %50s as %s" msg (htos prefix) (netToStr net) (hptos idl))
                                (net, (idl, "unused-tuple-field-sparrow2", facet, net)) :: cc
                            List.foldBack fg flat cc
                        let cd = aliases @ cd
                        (cc, cd)
                    if depth <= 0 then (cc, cd, ce) else List.fold (scan_serf false (depth-1) prefix) (cc, cd, ce) children

    let (netlist, aliases, contacts_down) =
        //dev_println "scan_serf start."
        scan_serf true 1 [] ([], [], []) xtop

    
    let check_rom (rtl_assigned, sup) =
        match sup with
        | DRIVE _ -> ()
        | SUPPORT lst ->
            let check_rom_serf (x, lanes_) =
                match x with
                    | X_bnet ff ->
                        //let f2 = lookup_net2 ff.n
                        let id = strict_cpp_id_sanitize ff.id
                        match length ff.constval with
                            | n when n > 1 ->
                                // TODO - abstract to gbuild so SystemC output etc can use it.
                                // A ROM is inferred from an X_bnet with a list of initial values.
                                note_rom_init_needed ww gen ff id n
                            | _ -> ()
                    | _ -> ()
            app check_rom_serf lst
    app check_rom !m_anal_results

#if SPARE
    let m_reset_vals = ref []
    let reset_collate (rtl_assigned_, arg) =
        match arg with
        | SUPPORT lst -> ()
        | DRIVE(lst, _) -> 
            let check_reset_serf (x, lanes_) =
                match x with
                    | X_bnet ff ->
                        //let f2 = lookup_net2 ff.n
                        match resetval_o x with
                            | Some v ->
                                mutaddonce m_reset_vals (ff.id, v)
                            | None -> ()
            app check_reset_serf lst
    app reset_collate !m_anal_results
#endif
    vprintln 2 (sprintf "netlist_scan: %i results" (length !m_anal_results))

    let signals = sigstuff ww gen !m_anal_results netlist contacts_down
    (netlist, aliases, signals, !m_anal_results) // end of netlist_scan 


let rec cvt_hpr_machine ww (gen:xpp_gen_t) topf classname cc (ii, design) =
    match design with
        | None -> cc
        | Some(HPR_VM2(minfo, decls, children, execs, assertions)) ->
            let flatten_children = false
            let (cc, fu_instances0) =
                if flatten_children then
                    (List.fold (cvt_hpr_machine ww gen false classname) cc children, [])
                else
                    let groc_fu_instance cc = function
                        | (ii, Some(HPR_VM2(minfo, decls, children_, execs_, assertions_))) when not ii.definitionf -> (ii, minfo, decls) :: cc
                        | (ii, _) -> cc
                    let instances = List.fold groc_fu_instance [] children
                    vprintln 2 (sprintf "Component instantiates %i FUs/children." (length instances))
                    (cc, instances)
                    
            let mname = if topf then classname else ii_fold [ii]
            let ast_ctrl = { g_null_ast_ctrl with id=mname }
            let mm = "Machine " + mname
            let ww = WF 3 "cvt_hpr_machine" ww mm


            let (netlist, aliases, signals, anal_results) = netlist_scan ww gen mm (ii, design)

            let xtor = if gen.xtor=[] then None else Some(hd gen.xtor)
            let targetf = xtor  = Some "target"
            let initiatorf = (xtor= Some "initiator")


            
            let has_top_ep  = initiatorf  (* The initiator on the bus has the EP: its not truely an initiator: it just responds to TLM side requests! *)
            let msg = if initiatorf then i2s(length execs - 1) + " CPP processes and an entry point called "
                      elif targetf then i2s(length execs) + " CPP processes where the first makes upcalls to "
                      else i2s(length execs) + " CPP excs to become processes (before collating) "
            vprintln 3  ("Converting an HPR machine to a " + msg +  mname)

            let rec debib_disparity sofar = function
                | (X_x(lhs, nn, _), rhs) -> debib_disparity (sofar-nn) (lhs, rhs)
                | (lhs, X_x(rhs, nn, _)) -> debib_disparity (sofar+nn) (lhs, rhs)                
                | _ -> sofar
            let seqpred (gl, lst, _) =
                let af (c1, d1) = function // Make a better debib equaliser please
                    // | Xif _ ...
//                  | Xcall _           -> (true, d1)
                    | Xassign(lhs, rhs) ->
                        match debib_disparity 0 (lhs, rhs) with
                            | 0 -> (true, d1)
                            | 1 -> (c1, true)                            
                            | other -> sf(sprintf "seqpred greater disparity=%i" other)
                    | other -> sf(sprintf "seqpred other %A" other)
                let (seqf, combf) = List.fold af (false, false) lst
                if seqf && not combf then true
                elif combf && not seqf then false
                else muddy "Mixed seqpred form - easy enough to unmix"

            let seqpred_ = function 
                    //| (a, b, resets) when false -> sf "splat"
                    | Xassign(lhs, rhs) -> sf "splog"
                    | other -> sf(sprintf "seqpred other %A" other)

            let rec jz (dic:hbev_t array, len) (pos, c) = 
                if (pos >= len) then c
                else CPP_BEV(ast_ctrl, dic.[pos]) :: jz (dic, len) (pos+1, c)

            let jfsm dir (SP_fsm(i, edges)) = 
                let notfancy = true
                let cens = map xi_orred dir.clk_enables
                let (ans) = 
                    if notfancy then
                        let (rtl_cmds, machine_reset) = fsm_flatten_to_rtl ww "cpp_render: jfsm" (SP_fsm(i, edges))
                        let resets = xrtl_resets ww (map snd rtl_cmds)
                        let reset_rtl = map (fun (a, b) -> gen_XRTL_arc(None, X_true, a, b)) resets
                        let (seq_arcs, comb_arcs) = groom2 seqpred (map (snd>>xrtl2hbev_1) rtl_cmds)
                        vprintln 2 (sprintf "Group fsm %s:  %i splits to %i seq and %i comb." (xToStr i.pc) (length rtl_cmds) (length seq_arcs) (length comb_arcs))
                        let resetgrds = map greset dir.resets
                        let reset_cond = ix_orl resetgrds
                        let rst_net = xi_blift reset_cond
                        let resets =
                            CPP_BEV(ast_ctrl, gec_Xif reset_cond (gec_Xblock(map xrtl2hbev (machine_reset @ reset_rtl))) Xskip)  // new ifshare does this?
                        let (seq_r, comb_r) = if gen.resets_synchronous then ([resets], []) else ([], [resets])


                        let an_seq = new_ifshare_cpp ww gen rst_net cens signals resetgrds seq_arcs
//                        let an_seq = new_ifshare_cpp ww gen rst_net signals resetgrds seq_arcs
                        
                        let an_comb = new_ifshare_cpp ww gen rst_net [] signals resetgrds comb_arcs
//                        let an_comb = new_ifshare_cpp ww gen rst_net signals resetgrds comb_arcs
                        
                        (seq_r @ [CPP_BEV(ast_ctrl, gec_Xblock an_seq)], comb_r @ [CPP_BEV(ast_ctrl, gec_Xblock an_comb)])
//                      (seq_r @ [CPP_BEV(gec_Xblock an_seq)], comb_r @ [CPP_BEV(gec_Xblock an_comb)])


                    else
                        let wrap_edgework_jj state a cc = CPP_BEV(ast_ctrl, xrtl2hbev a)::cc
                        let ka_fsm (si:vliw_arc_t) cc =
                            let edgework = List.foldBack (wrap_edgework_jj si.resume) (rtl_once si.cmds) []
                            let resets = xrtl_resets ww si.cmds
                            // let _ = dir.clk_enables
                            muddy "need more than edgework!"
                        muddy "(resets, cpp_ifshare gen (List.foldBack ka_fsm edges []))"
                ans


            vprintln 2 (sprintf "cpp_render: has_top_ep=%A   %i aliases for port formals and so on created." has_top_ep (length aliases))

            reportx 3 (sprintf "cpp_class: %s aliases" mname) (fun (id, (net, fu_name, pi_name, lname)) -> sprintf "%20s --> %60s  %s %s  %s" (netToStr net) id (valOf_or fu_name "fu=None  ") (valOf_or pi_name "pi=None  ") lname) aliases


            let rec cvt3 dir x cc =
              match x with
                  |  SP_l(ast_ctrl, v)    -> (dir, [CPP_BEV(ast_ctrl, v)], [])::cc
                  |  SP_seq(items)  -> List.foldBack (cvt3 dir) items cc
                  |  SP_dic(dic, id)      -> (dir, jz (dic, dic.Length) (0, []), [])::cc
                  |  SP_fsm(info, machine_states) ->
                      let (seq, comb) = jfsm dir (SP_fsm(info, machine_states))
                      (dir, seq, comb)::cc
                  |  SP_rtl(ii, cmds)           ->
                      let cmds = rtl_once cmds
                      let (seq, comb) = groom2 seqpred (map xrtl2hbev_1 cmds)
                      let reset_clause =
                          let resets = xrtl_resets ww cmds
                          let reset_rtl = map (fun (a, b) -> gen_XRTL_arc(None, X_true, a, b)) resets
                          let reset_cond = ix_orl (map greset dir.resets)
                          CPP_BEV(ast_ctrl, gec_Xif reset_cond (gec_Xblock(map xrtl2hbev reset_rtl)) Xskip) // new ifshare does this?

                      let ww' = WN "new_ifshare" ww
                      // We should add the reset to either the comb or the seq depending on whether it is synchronous.
                      let (seq_r, comb_r) = if gen.resets_synchronous then ([reset_clause], []) else ([], [reset_clause])
                      let resetgrds = map (f3o3>>xi_orred) dir.resets
                      let rst_expr = ix_orl (map greset dir.resets)

                      let an_seq = new_ifshare_cpp ww' gen rst_expr (map xi_orred dir.clk_enables) signals resetgrds seq
                      let an_comb = new_ifshare_cpp ww' gen rst_expr [] signals resetgrds comb                      
                      (dir, seq_r @ [CPP_BEV(ast_ctrl, gec_Xblock an_seq)], comb_r @ [CPP_BEV(ast_ctrl, gec_Xblock an_comb)]) :: cc


                  |  SP_comment ss        -> (dir, [CPP_COMMENT ss], []) :: cc
                  |  SP_asm _ -> muddy "convert to cpp: syscit asm form not implemented"
                  |  (_) -> muddy "convert cvt3 to cpp: syscit form not implemented"

            let rec cvt2 dir cc = function
                | SP_par(_, lst) -> List.fold (cvt2 dir) cc lst
                | other -> (cvt3 dir other []) :: cc


            let cvt1 x =
                match x with
                    | H2BLK(dir, tof) when false && nullp dir.clocks -> // OLD combinational logic route
                        let m_sh = ref []
                        let queries =
                            { g_null_queries with
                                 concise_listing=   true
                            }
                        let _ = anal_block ww (Some m_sh) queries x // We find the support for combinational logic here.
                        // all variables written are turned into signals!
                        let signalfinder c (rtl_assigned, arg) = // Repeated code
                            let sff2 = function
                                | X_x(v, _, _) -> v
                                | v            -> v
                            let sff1 c (net, sk) = singly_add (sff2 net) c
                            match arg with
                                | SUPPORT(l) -> List.fold sff1 c l
                                | _ -> c
                        let support = List.fold signalfinder [] !m_sh
                        (support, cvt2 dir [] tof)
                    | H2BLK(dir, tof) -> 
                        (* let _ = vprint(3, " clock is " + sfold xToStr dir.clocks)  *)
                        let clk_ens = dir.clk_enables // TODO
                        let ans =
                            let clk__ = hd dir.clocks // Clocks beyond head are ignored - but we only have one at most. This field not used now.
                            if x_eq (de_edge clk__) g_construction_big_bang
                            then (hpr_yikes ("+++ big bang constructor dropped in C++/C# output"); (*cv2  ... (tof, c) *) [])
                            else cvt2 dir [] tof
                        ([], ans) // Don't need support if clocked.

            (* We convert to a list of parallel pairs of form (clk, imperative_item list) *)
            let rtl_ans = map cvt1 execs  

            let nosignal xx = gec_X_net(strict_cpp_id_sanitize(xToStr xx)) // Bypass bnet to avoid signal.write or signal.read generation.
            let portname = valOf_or (at_assoc "portname" minfo.atts)  (funique "anonport")
            let explicit_signal_initializer_rez (ml_, xx) cc = // Call the signal/instance constuctor to give the item its name so that more meaningful run-time errors are possible.
                let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_void_prec]  }
                match xx with
                    | X_bnet ff -> 
                       //let f2 = lookup_net2 ff.n
                       if ff.is_array then // TODO check also the star-style arrays that have handles.
                           vprintln 3 (sprintf "Skip explicit net/item initializer for %s since this is a valuetype array." ff.id)
                           cc
                       elif memberp xx signals then
                          let (idl, id_, facet, x) = valOf_or (op_assoc (xx) netlist) ([ff.id], "spare", "", xx)
                          let ftag = ftag_fun (Some xx)
                          //dev_println(sprintf "explicit_signal_initializer_rez %20s  mapped_id=%s  ml=%A ftag=%s" (hptos idl) id_ ml_ ftag)

                          let id = aliasfun strict_cpp_id_sanitize aliases ff // <- this needs callee alias map - but the formal should be in there ml_ field?
                          let unaliased = if gen.unaliased_sanitized then strict_cpp_id_sanitize  ff.id else ff.id
                          CPP_BEV(ast_ctrl, gec_Xcall((id, gis), [ xi_string (unaliased) ]))::cc
                       else cc
                    | other -> cc
                       
            let body_signal_initializer_rez (ml_, xx) cc =
                let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_void_prec]; }
                match xx with
                    | X_bnet ff -> 
                       let f2 = lookup_net2 ff.n
                       if ff.is_array then // TODO check also the star-style arrays that have handles.
                           vprintln 3 (sprintf "Skip net/item initializer for %s since this is a valuetype array." ff.id)
                           cc

                       elif memberp xx signals then
                          let (idl, id_, facet, x) = valOf_or (op_assoc (xx) netlist) ([ff.id], "spare", "", xx)
                          let ws = ansiToStr gen.cppf (ctype (mm + ":" + ff.id) xx) 
                          let ws1 = if f2.xnet_io=INPUT || f2.xnet_io=OUTPUT then [ gec_X_net "this" ] else [ xi_num(encoding_width xx) ]
                          let sigwrapped =
                              let ws1 = "< " + ws + " >"
                              if f2.xnet_io=INPUT then "sc_in"  + ws1
                              else if f2.xnet_io=OUTPUT then "sc_out"  + ws1
                              else "sc_signal" + ws1
                          let xs = no_signal xx 
                          let id = aliasfun strict_cpp_id_sanitize aliases ff
                          let a = Xassign(xs,  xi_apply(("new\\ " + sigwrapped, gis), ws1 @ [ xi_string id ])) // Pass signal/instance name in to it.
                          singly_add (CPP_BEV(ast_ctrl, a)) cc
                       else
                           vprintln 3 (sprintf "Skip net/item initializer_rez %s since not a signal/instance." ff.id)
                           cc
                    | other -> cc

            let flat = 
                let cx = ((fun (formal, actual)-> (formal, actual)), (fun (meta_, b) -> b))
                cvt_flatten (None, 1) cx None decls
            vprintln 2 (sprintf "Report: First call cvt_flatten %i nets from %i declarations" (length flat) (length decls))
            let fu_instances =
                let gec_fu_instance (iinfo, (minfo:minfo_t), instance_decls) =
                    let kind = minfo.name
                    let xkind = strict_cpp_id_sanitize(hptos kind.kind)
                    let dt = Ansi_struct(xkind, "", 0)
                    let iname = gec_X_net (strict_cpp_id_sanitize iinfo.iname)
                    let cx x = x
                    let actuals0  = cpp_cvt_flatten (Some true, 1) cx None instance_decls 

// Override coding style: from https://www.doulos.com/knowhow/systemc/faq/
//                    SC_HAS_PROCESS(ram);
//                    ram(sc_module_name name_, int size_=64, bool debug_ = false) : sc_module(name_), size(size_), debug(debug_)
                    let overrides = cpp_cvt_flatten (Some false, 1) cx None decls
                    vprintln 2 (sprintf "cpp_render: Instance of %s with iname %s" (protocols.vlnvToStr kind) (xToStr iname))
                    (iname, kind, xkind, overrides, actuals0, CPP_NETDECL(iname, dt))
                map gec_fu_instance fu_instances0

            let fu_type_names =
                let fu_type_mine cc (iname, kind, xkind, overrides, actuals0, _) = singly_add xkind cc
                List.fold fu_type_mine fu_instances []

            let temp_rose_reveng (x:edge_t) = // Create net.pos() or net.neg()
                let rend keyw clk =
                    let posprec = { g_default_native_fun_sig with rv=g_absmm_prec; args=[g_bool_prec] }
                    ix_pair (no_signal clk) (xi_apply((keyw, posprec), [])) // We use the pair construct for dot field access
                match x with
                    | E_pos clk -> rend "pos" clk
                    | E_neg clk -> rend "neg" clk                        
                    | E_any sup -> muddy "cpp_render anyedge" // These two should go the combinational route and not be encountered here.
                    | E_anystar -> muddy "cpp_render anystar"

            let thrd = if gen.cppf then "SC_THREAD" else  "SystemCsharp\.SC\.sc_thread"
            let mthd = if gen.cppf then "SC_METHOD" else  "SystemCsharp\.SC\.sc_method"


            let fu_ctor_call (iname, kind, xkind, overrides, actuals, _) cc = 
                let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_void_prec]  }
                let id = xToStr iname
                let basename = xi_string id  // The instance name is passed first for printing in run-time messages. 
                let override_calls =
                    let rec oridegen arg cc =
                        match arg with
                            | CDB_group(meta_, items)       -> List.foldBack oridegen items cc
                            | CDB_leaf(formal_string, hexp) -> hexp :: cc

                    List.foldBack oridegen overrides []
                CPP_BEV(ast_ctrl, gec_Xcall((id, gis), basename :: override_calls))::cc

            let bwrap x = CPP_BEV(ast_ctrl, x)
            let rez_methods id spawned =
                // Collate on clock domain, one of which will be the constructor big bang.
                let (collated_domains, comb) =
                    let (clocked, comb) = groom2 (fun (_, dir, _) -> not_nullp dir.clocks) spawned
                    let pivotf (_, ci, _) = ci
                    let collated_domains = generic_collate pivotf clocked
                    vprintln 2 (sprintf "Report: Collating clock domains: %i items,  %i clocked,  %i comb,  %i collated" (length spawned) (length clocked) (length comb) (length collated_domains))
                    (collated_domains, comb)

                let directors = list_once(map fst collated_domains)
                let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_default_prec]; }
                let gen_synchronous_method (dir, worklist) (cc, cd) =
                    // Put all synchronous work on this clock domain in one wrapper method if there is more than one worklist item.
                    //dev_println (sprintf "gen_synchronous_method")
                    let make_wrapper = length worklist > 1
                    let callee = 
                        if make_wrapper then funique "hprls_tickdomain" else f1o3 (hd worklist)

                    let tickdomains =
                        if make_wrapper then
                            let awrap arg = arg // Not correct for CSharp?                            
                            let gen_synchronous_serf(member_id, _, _) cc = Xeasc (xi_apply((mthd, gis), [ awrap (gec_X_net member_id) ])) :: cc
                            let ticker_work = List.foldBack gen_synchronous_serf worklist []
                            let inlinef = true
                            [ CPP_METHOD(inlinef, callee, Ansi_void, [], map bwrap ticker_work) ] // New aggregate method called a tickdomain
                        else []
                    let newx = 
                        if gen.cppf then
                            let awrap arg = arg
                            let sensitive = gec_X_net "sensitive"
                            //  SystemC:   sensitive << clk.pos() is the new coding style (old was sensitive_pos << clk).
                            let baser = xi_apply((mthd, gis), [ awrap (gec_X_net callee) ])
                            match dir.clocks with // SystemC
                                | [ci] ->
                                    if de_edge ci = g_construction_big_bang
                                    then
                                        vprintln 3 (sprintf "Synchronous method %s has constructor big bang. So no clock." id)
                                        [gec_Xcall((id, gis), [])] // Want to call it directly from within the constructor, not as a callback.
                                    else
                                        let splatclk = (temp_rose_reveng ci)
                                        vprintln 3 (sprintf "Synchronous method %s has clock %s." id (xToStr splatclk))
                                        [ Xeasc baser; Xeasc(ix_lshift sensitive splatclk) ]
                                | _ -> sf "Not synchronous L1096"
                        else
                            let awrap arg = xi_apply(("new\\ SystemCsharp\\.sc_method_t", gis),  [ arg ])
                            match dir.clocks with  // SystemCSharp
                                //     SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(this.step)).sensitive_pos(clk);
                                | [ ci ] ->
                                    if de_edge ci = g_construction_big_bang
                                    then [gec_Xcall((id, gis), []) ]
                                    else [Xeasc(ix_pair (xi_apply((mthd, gis), [ awrap(gec_X_net id) ])) (xi_apply(("sensitive_pos", gis), [no_signal(temp_rose_reveng ci)]))) ]
                                | [] -> sf "Not Synchronous L1107"
                    (newx @ cc, tickdomains @ cd) // This reverses, so invoke with fold not foldBack?
                let (items2, tickdomains) = List.foldBack gen_synchronous_method collated_domains ([], [])
                (tickdomains, directors, items2, comb)
                
            let rez_constructor id directors items2 comb rom_initializer_calls = // Construct: create the ctor method.
                let signal_ctor_calls = if gen.cppf then List.foldBack explicit_signal_initializer_rez flat [] else [] // Constructor calls for signal.
                let fu_ctor_calls = if gen.cppf then List.foldBack fu_ctor_call fu_instances [] else [] // Constructor calls for FUs.
                let body_initializers = if gen.cppf then [] else List.foldBack body_signal_initializer_rez flat []
                let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_default_prec]; }
                let cbody = if gen.cppf then [ CPP_BEV(ast_ctrl, gec_Xcall(("SC_HAS_PROCESS", gis), [ gec_X_net id ])) ] else []

// test(sc_core::sc_module_name name1)  :
//    sc_module(name1),    oppp_DL("oppp.DL"),    oppp_Strobe("oppp.Strobe")
                let parent = if gen.cppf then "sc_core::sc_module_name" else "string"
                let kind_formal = gec_X_net "_kind_name"
                let fp = (Ansi_struct("", parent, 0), kind_formal)

                let gen_asynchronous_method (id, dir, support) cc =
                    if gen.cppf then
                        let awrap arg = arg
                        let sensitive = gec_X_net "sensitive"
                        //  SystemC:   sensitive << clk.pos() is the new coding style (old was sensitive_pos << clk).
                        let baser = xi_apply((mthd, gis), [ awrap (gec_X_net id) ])
                        match dir.clocks with // SystemC
                            | [] ->
                                if nullp support then hpr_yikes (sprintf "No support for combinational method " + id)
                                let add_sens cc arg = ix_lshift cc (no_signal arg)
                                Xeasc baser :: Xeasc (List.fold add_sens sensitive support)::cc
                            | _ -> sf "not asynchronous L1076"
                    else
                        let awrap arg = xi_apply(("new\\ SystemCsharp\\.sc_method_t", gis),  [ arg ])
                        match dir.clocks with  // SystemCSharp
                            //     SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(this.step)).sensitive_pos(clk);
                            | [] ->
                                let baser = xi_apply((mthd, gis), [ awrap (gec_X_net id) ])
                                let add_sens baser arg = ix_pair baser (xi_apply(("sensitive", gis), [no_signal arg]))
                                Xeasc (List.fold add_sens baser support) :: cc
                            | _ -> sf "not asynchronous L1082"

                let _ = vprintln 2 (sprintf "Generating constructor for %s with %i and %i items and combs" id (length items2) (length comb))
                let parent1 = if gen.cppf then "sc_core::sc_module" else "base"
                let sc_call = [CPP_BEV(ast_ctrl, gec_Xcall((parent1, gis), [ xi_string id]))]
                let sc_call = [CPP_BEV(ast_ctrl, gec_Xcall((parent1, gis), [ kind_formal ]))]                



                let fu_netlist =
// Wiring example in structural SystemC netlist            let fu_wiring = 
            //dff dff1, dff2; // Instantiate FFs
            //SC_CTOR(shiftreg) : dff1("dff1"), dff2("dff2")
            //  { dff1.clk(clk);
            //    dff1.reset(reset); }
                    let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[g_void_prec]; }                  
                    let netconnects (iname, kind, xkind, overrides, actuals0, _) cc = // Concect to formal contact of child component.
                        let rec netconnect act cc =
                            match act with
                                | CDB_group(_, lst) -> List.foldBack netconnect lst cc
                                | CDB_leaf(formal, actual_arg) ->
                                    let formal_fgis = (formal, gis)
                                    let nosignal xx = gec_X_net(strict_cpp_id_sanitize(xToStr xx)) // Bypass bnet to avoid signal.write or signal.read generation.
                                    CPP_BEV(ast_ctrl, Xeasc(ix_pair (iname) (xi_apply(formal_fgis, [ no_signal actual_arg ])))) :: cc
                        List.foldBack netconnect actuals0 cc
                    List.foldBack netconnects fu_instances []

                let items1 = List.foldBack gen_asynchronous_method comb []
                let cbody = cbody @ body_initializers @ map bwrap (items1 @ items2) @ fu_netlist @ rom_initializer_calls
                let cons = CPP_CONSTRUCTOR(Some "struct", id, sc_call @ signal_ctor_calls @ fu_ctor_calls, [fp], cbody, "")
                vprintln 2 (sprintf "Report: %s: %i directors" mname (length directors))
                let _ = app (fun (_, (kind:ip_xact_vlnv_t), xkind, _, _, _) -> mutaddonce gen.m_includes (kind.kind, xkind)) fu_type_names
                (cons, fu_type_names)  // end of rez_constructor: create the ctor method.

            let (gdecls, ldecls) = groom2 (snd >> hexpt_is_io) flat
            let gdecls = list_once (map snd gdecls)


            let facetf = at_assoc "facet" minfo.atts = Some "Ointerface"

            let gen_formal_decl (nx) =
                match nx with 
                    | X_bnet ff ->
                        //let f2 = lookup_net2 ff.n
                        (ctype (mm + ":" + ff.id) nx, nx)
                    | other -> sf( "gen_formal_decl other: " + xToStr other)

            let ep_formals = if has_top_ep then map gen_formal_decl gdecls else []

            (* Copy formals used by non-ep methods into the object immediately after ep. *)
            let queries =
                { g_null_queries with
                    full_listing=true
                }

            let fp_copies_needed =
                let m_non_ep_anal = ref []
                if (execs <> []) then app (anal_block ww (Some m_non_ep_anal) queries) (tl execs)
                let fp(ct, x) = sd_mentioned (x) (map snd !m_non_ep_anal) 
                let fp_copies_needed = List.filter fp ep_formals
                let _ = reportx 3 "Ep formal prams signals needed in other threads" (fun(ct,x)->netToStr x) fp_copies_needed
                fp_copies_needed

            let formal_rename((ct, s)) = if memberp (ct, s) fp_copies_needed then (ct, clonenet "_" "" s) else (ct, s)
            let formal_copy g (ct, x) cc = if not g then cc else CPP_BEV(ast_ctrl, Xassign(x, clonenet "_" "" x))::cc 
                
            if facetf then
                let vlnv = { vendor="HPRLS"; library=(if gen.cppf then "OPATH-SYSTEMC" else "OPATH-SYSCSHARP"); kind=[portname]; version="1.0"}
                mutadd gen.m_includes (vlnv.kind, portname)

            let new_support lst =   // We find the support for combinational logic here.
                let _:cpp_unit_t list = lst
                let m_sh = ref []
                let queries =
                            { g_null_queries with
                                 concise_listing=   true
                            }
                let _ =
                    let anal_cpp = function
                        | CPP_BEV(ast_ctrl, hbev) ->
                            //dev_println (sprintf "Supporters getting %s" (hbevToStr hbev))          
                            anal_block ww (Some m_sh) queries (H2BLK(g_null_directorate, SP_l(ast_ctrl, hbev)))
                        | other -> sf (sprintf "anal_cpp other %A" other)
                        
                    app anal_cpp lst
                        // all variables written are turned into signals!
                let signalfinder c (rtl_assigned, arg) = // Repeated code
                    let sff2 = function
                        | X_x(v, _, _) -> v
                        | v            -> v
                    let sff1 c (net, lane_) = singly_add (sff2 net) c
                    match arg with
                        | SUPPORT(l) -> List.fold sff1 c l
                        | _ -> c
                let support = List.fold signalfinder [] !m_sh
                //dev_println (sprintf "Supporters were %s" (sfold xToStr support))
                support

            let (directors, work_methods, spawned) =
                let m_spawned = ref []

                let ignoreable lst =
                    let ignoreable_pred = function
                        | CPP_BEV(_, Xskip) -> true
                        | CPP_BEV(_, other) ->
                            //hpr_yikes(sprintf "Ignoreable: other CPP_BEV item %s" (hbevToStr other))
                            false
                        | other ->
                            //hpr_yikes(sprintf "Ignoreable: other item %A" (other))
                            false
                    conjunctionate ignoreable_pred lst // Will return true for an empty list.
                let work =
                    let bwork (support_, items) cc =
                        let bwork_serf (dir, seq_units, comb_units) cc =
                            if nullp dir.clocks then
                                let all_units = seq_units@comb_units
                                if ignoreable all_units then cc else (dir, all_units, new_support all_units)::cc
                            else
                                //dev_println (sprintf "Friday group %i seqs" (length seq_units))
                                let seq  = if ignoreable seq_units then [] else [(dir, seq_units, [])]
                                let comb = if ignoreable comb_units then [] else [({dir with clocks=[]}, comb_units, new_support comb_units)]
                                seq @ comb @ cc
                        List.foldBack bwork_serf (list_flatten items) cc
                    List.foldBack bwork rtl_ans []
                let (clocked, combinational) = groom2 (fun (dir, _, _) -> not_nullp dir.clocks) work
                let collated_domains = generic_collate f1o3 clocked
                vprintln 2 (sprintf "Report: Collating work_methods: %i items,  %i clocked,  %i comb,  %i directing domain." (length work) (length clocked) (length combinational) (length collated_domains))

                let cpp_method_generate_synch ((dir, lst), idxno) =
                    let epf = has_top_ep && idxno = 0
                    let id = if epf then mname else funique("exe_" + i2s idxno + "_")
                    let kx = (sprintf " Synch method %s with %i operations." id (length lst))
                    let comment = CPP_BEV(ast_ctrl, Xcomment(" Method no " + i2s idxno + " mname=" + mname + kx))
                    vprintln 3 kx
                    let content = (List.foldBack (formal_copy epf) fp_copies_needed []) @ [ comment ] @ list_flatten (map f2o3 lst)
                    let contacts = if not epf then [] else map formal_rename ep_formals
                    let rt = Ansi_void // rt_scan code needed for xactors with a result <--- TODO
                    let rr = CPP_METHOD(false, id, rt, contacts, content)
                    mutadd m_spawned (id, dir, [])
                    rr

                let cpp_method_generate_asynch (dir, lst, support) =
                   let id = funique "combgroup"
                   let comment = CPP_BEV(ast_ctrl, Xcomment(" Method no " + id + " mname=" + mname ))
                   let content = comment :: lst
                   let rr = CPP_METHOD(false, id, Ansi_void, [], content)
                   mutadd m_spawned (id, dir, support)
                   rr

                let synch = map cpp_method_generate_synch (zipWithIndex collated_domains)
                let asynch = map cpp_method_generate_asynch combinational
                let directors = map fst collated_domains
                (directors, synch @ asynch, !m_spawned)
                    

            let dir_nets =
                let roundup_director_nets cc dir =
                    let nnets = get_directorate_nets dir
                    vprintln 3 (sprintf "%i nets for director" (length nnets))
                    lst_union nnets cc
                let nets = List.fold (roundup_director_nets) [] directors
                //let wrap_db net = ([], net) // Give it a null nominal db.
                //map wrap_db nets
                nets

            let m_portformals = ref []                

            let fields =
                if has_top_ep
                then lst_union dir_nets (map snd ldecls) @ map (fun (ansi, net) -> (net)) fp_copies_needed
                else lst_union dir_nets (map snd flat)

            reportx 3 (sprintf "cpp_class: %s hexp fields" mname)  xToStr fields

                
            //reportx 3 "cpp_class: fields once" (fun (a, b) -> a + netToStr b) fields                
            let fields = map (encTocpp gen mm)  (List.filter(fun x -> funidvar x && not(memberp x (!m_portformals))) (list_once fields))
            //reportx 3 "cpp_class: portformal fields" netToStr (!m_portformals)

            reportx 3 (sprintf "cpp_class: %s cpp fields" mname)  cpp_unitToStr_summary fields

            let rom_initializers =     // Find ROMs - imperative initializer is not ideal - TODO change for C++ literal structures.
                let m_rom_initializers = ref []
                vprintln 2 (sprintf "cpp_render: ROMs: Checking for ROM initializers")
                let render_rom_initializer (ff:net_att_t, vnet, len) =
                    let net = X_bnet ff
                    let w = encoding_width net
                    let mid = ff.id + "_rom_initializer"
                    let deconst arg = // TODO find better method elsehwere.
                        match arg with
                            | XC_bnum(w_, bn, _) ->  xi_bnum_n w bn
                            | _ -> sf "deconst L1410"
                    let gen_init_assign (dd, idx) = Xassign(ix_asubsc net (xi_num idx), deconst dd) 
                    let leader = Xcomment(sprintf "cpp_render: ROMs: ROM data table: %i words of %i bits." len w) 
                    let content = CPP_BEV(ast_ctrl, gec_Xblock(leader :: map gen_init_assign (zipWithIndex ff.constval)))
                    vprintln 3 ("Converted a ROM initializer for " + ff.id)
                    let meth = CPP_METHOD(false, mid, Ansi_void, [], [content])
                    let gis = { g_default_native_fun_sig with fsems={g_default_fsems with fs_eis=true; fs_nonref=true }; rv=g_void_prec; args=[] }
                    let call = CPP_BEV(ast_ctrl, gec_Xcall((mid, gis), [ ]))
                    mutadd m_rom_initializers (meth, call)
                for z in gen.m_rom_inits do render_rom_initializer z.Value done
                !m_rom_initializers


            let (tickdomains, directors__, items2, comb) = rez_methods (strict_cpp_id_sanitize mname) spawned
            let _ = vprintln 2 (sprintf "Report: Generate C++/CSharp/SystemC module %s. Summary net/port count is %i external ports, %i lnets and %i directors." mname (length gdecls) (length ldecls) (length directors__))
               
            let (mM, eitems) =
                let rom_setup_calls = map snd rom_initializers
                let (cons, further_directors___) = rez_constructor (strict_cpp_id_sanitize mname) directors items2 comb rom_setup_calls
                let methods = cons :: tickdomains
                let ext_class() = CPP_CLASS(None, "struct", portname, methods @ (map (encTocpp gen mm) gdecls), ": public sc_module")
                let ext_class_ref()= CPP_NETDECL(gec_X_net mname, Ansi_struct("struct", portname, 1))

                if topf then (methods @ work_methods, [])
                elif nullp rtl_ans && facetf then ([ext_class_ref()], [ext_class()])
                else (work_methods,  [])

            let mM = mM @ (map f6o6 fu_instances) @ (map fst rom_initializers)


            let ww = WF 3 "cvt_hpr_machine" ww ("Finished converting an HPR machine to a CPP method called " + mname)
            let (nN0, mM0, pP0, eitems00) = cc

            ((netlist, signals, aliases) :: nN0, mM @ mM0, lst_union fields pP0, eitems @ eitems00) // end of cvt_hpr_machine



// Write SystemC versions of instantiated component files if requested.
and write_adjunct_module ww (gen:xpp_gen_t) orangepath_defs (kind, xname (*, rides*) )  =
    let path = sfold_colons !g_ip_incdir
    let vd = gen.vd
    let suffix = ".h"
    let enabled =
        match gen.write_adjunct_module with
            | "disable" -> false
            | "enable"  -> true
            | "if-missing" ->
                if path = "" then hpr_yikes(sprintf "SystemC library block search path was empty. Please set one with -ip-incdir=a:b:c (or semicolon on Windows)")
                match pathopen_any path vd [';'; ':'] [xname ] suffix with
                    | (None, places_looked) -> true
                    | (Some filename, _) ->
                        vprintln 1 (sprintf "Not writing SystemC files for %s since a definition already found on ip-incdir at %s" xname filename)
                        false
            | other -> cleanexit(sprintf "Write_adjunct_module flag was not one of disable, enable or if-missing. It was %s" other)
                            
    if not enabled then ()
    else
        let ww = WF 3 "cpp_render: write_adjunct_module: " ww xname
        let portmeta = []
        let rides =
            let aw = 10
            let dw = 32
            // Create some default port meta that should be overriden in constructions when relevant
            [ ("ADDR_WIDTH", sprintf "%A" aw); ("DATA_WIDTH", sprintf "%A" dw); ("LANE_WIDTH", sprintf "%A" dw); ("WORDS", sprintf "%A" (two_to_the aw)) ] 
        //
        let vlnv = { vendor="HPRLS"; library=(if gen.cppf then "OPATH-SYSTEMC" else "OPATH-SYSCSHARP"); kind=kind; version="1.0"}
        let ip_block =
            //let _:ip_xact_vlnv_t = vlnv
            match op_assoc (hptos kind) orangepath_defs with
                | None ->
                    let failfun () = sprintf "Local Definitions Included ^%i:\n" (length orangepath_defs) + sfoldcr_lite fst orangepath_defs + "\n End of inventory.\n"
                    cvipgen.dyno_rez_ipblock ww failfun vlnv portmeta (map (fun (k,v)->(k,v)) rides)

                | Some(iinfo_, ip_block) -> ip_block
        let gen_aux = { gen with add_aux_reports=false; m_includes= ref[]; m_rom_inits= new xpp_rom_inits_t("rom_inits")  } // Abstract this cleaner
        let mm = (sprintf "%s auto created by HPR cpp_render." (protocols.vlnvToStr vlnv))
        let ii =
            { g_null_vm2_iinfo with
                  vlnv=vlnv
                  definitionf=true
                  generated_by=gen.stagename
             }
        let (x, methods, fields, eitems_) = cvt_hpr_machine ww gen_aux true xname ([], [], [], []) (ii, Some ip_block)
        let (netlist, signals, aliases) = List.unzip3 x
        let netlist = list_flatten netlist
        let signals = list_flatten signals
        let aliases = list_flatten aliases
        let includes = target_env_includes gen
        let design' =
            if false then eitems_
            else
               [
                   CPP_COMMENT mm;
                   
                   CPP_CLASS(None,
                             (if gen.cppf then "struct" else "public class"),
                             xname,
                             fields @ methods,
                             (if gen.cppf then " : public sc_module" else " : sc_module")
                             )
               ]

        if gen.cppf then cpp_to_file ww gen_aux ((xname, suffix), true, design', netlist, signals, aliases, includes) // Write .h file

        let includes = target_env_includes gen @ [ (vlnv.kind, xname) ]
        cpp_to_file ww gen_aux ((xname, ".cpp"), false, design', netlist, signals, aliases, includes) // Write .cpp file        
        ()
(*
 * This is a vanilla(wot?) routine that converts an HPR VM to a C++ class in various ways.
 * Some HPR machines epresent entry points run by other threads whereas others have their own thread or else are essentially declarative.
 *  we should look at minfo.definitionf as a guide ...
 * A machine with no code or rules can become a C struct, since it is just a logical wrapper around a set of gdecls.
 *)
let convert_to_cpp ww msg (gen:xpp_gen_t) orangepath_defs ((iinfo, design), classname) =
    match design with
      | None -> None
      | Some(HPR_VM2(minfo, decls_, children_, execs_, assertions_)) ->
            let name = protocols.vlnvToStr minfo.name
            let mm = ("convert_to_cpp " + name)
            let ww' = WN mm ww
            vprint 3 ("Cpp conversion of HPR machines.  Topname=" + name + ". Starting\n")
            let norecurse = false 
            let _ = analyse ww norecurse "Machine(s) for conversion to CPP" (YOVD 1) true  (*costs*)false false (HPR_VM2(minfo, decls_, children_, execs_, assertions_))


            // Compute a useful digest of the netlist, including ...

#if NEWCODE
            let aliases = []
            let rvu = new_rvu "cpp_misc" (muddy "rvu=need")
            let pp = muddy "pp need"
            let pas = (rvu, pp, aliases)
            let fv_cpp = muddy "fv==need"
            let debib = fun _ -> muddy "debib=need"
            let (dir_rcode, dir_bb, dir_cc) = gbuild_director ww (debib, cpp_xbToStr fv_cpp, cpp_xbToStr fv_cpp, pas) pp
#endif

            let includes = target_env_includes gen
   
            let (x, methods, fields, eitems_) = cvt_hpr_machine ww gen true classname ([], [], [], []) (iinfo, design)
            let (netlist, signals, aliases) = List.unzip3 x
            let netlist = list_flatten netlist
            let signals = list_flatten signals        
            let aliases = list_flatten aliases


            // Control ports. Put inside cvt please.
            let cports =
               let cpi = { g_null_db_metainfo with kind= "cpp-control-nets" }
               DB_group(cpi, map db_netwrap_null [ g_clknet; g_resetnet ])

            let gports = if true then [] else map (encTocpp gen mm) (muddy "cports")  (* NOT WANTED IN HERE *) // get other directorate nets and abend syndrome etc please...

            let design' =
               [
                   CPP_COMMENT msg;
                   CPP_CLASS(None,
                             (if gen.cppf then "struct" else "public class"),
                             classname,
                             gports @ fields @ methods,
                             (if gen.cppf then " : public sc_module" else " : sc_module")
                             )
               ]

            let _ = app (write_adjunct_module ww gen orangepath_defs) !gen.m_includes

            let _ = if gen.cppf then cpp_to_file ww gen ((classname, ".h"), true, design', netlist, signals, aliases, includes @ !gen.m_includes) // C#, like Java, has not separate .h files
            let suffix = (if gen.systemcf then ".sysc" else "") + (if gen.cppf then ".cpp" else ".cs")
            let _ = cpp_to_file ww gen ((classname, suffix), false, design', netlist, signals, aliases, includes @ !gen.m_includes) // Write .cpp or .cs file.
            let _ = vprintln 3 ("Syscit " + name + " finished.")
            design

let g_def_cc prefs =
   {
         shortname=       "cpp"
         stagename=       ""
         topclass=        "DUT"
         prefs=           prefs
         xtor=            []
         cppf=            false
         systemcf=        false
         ifshare=         0
         add_aux_reports= false 
         write_adjunct_module=     "if-missing"
         vd=              20
         unaliased_sanitized= false
         resets_synchronous=true
         // Mutables
         m_includes=      ref []
         m_rom_inits=     new xpp_rom_inits_t("rom_inits")
   }


//
// Parse settings
let xpp_gen_args cppf stagename (filename:string) c2 =
    let shortname = "cpp"
    let topclass = hd(arg_assoc_or_fail "opath_sysc" (stagename + "-topclass", c2))
    let topclass = (if filename.[0] = '$' then filename else topclass) // Set filename to $ for same as classname?  See 'resolvedols' notes.
    let xtor = arg_assoc_or "opath_sysc" (stagename + "-xtor", c2, [])
    let gbuild_prefs = get_gbuild_recipe_prefs stagename shortname c2
    in
      { g_def_cc gbuild_prefs with
         shortname=  shortname
         stagename=  stagename
         topclass=   topclass
         xtor=       xtor
         cppf=       cppf
         systemcf=   true
         ifshare=    cmdline_flagset_validate (shortname + "-ifshare") ["none"; "simple"; "on" ] 0 c2
         add_aux_reports=    control_get_s stagename c2 (shortname + "-add-aux-reports") (Some "disable") = "enable"          
         vd=         max !g_global_vd (control_get_i c2 (stagename + "-loglevel") 1)
      }

    
(*
 * Convert to SystemC
 *)
let opath_cvt_to_sysc ww (op_args:op_invoker_t) vms =
    let stagename = op_args.stagename
    let msg = op_args.banner_msg
    let pre = op_args.stagename + "-"

    let orangepath_defs = collate_defs_from_vm2_tree ww vms

    let ww = WF 3 "cvt_to_sysc" ww ("Convert to C++/SystemC. " + msg)
    let filename0 = arg_assoc_or "sysc filename" (stagename + "-filename", op_args.c3, [])
    let preferred_name = if nullp vms then None else vm_att_assoc g_preferred_name (hd vms)

    let filename =
        if filename0 <> [] && filename0 <> [ "" ] then hd filename0
        elif preferred_name <> None then valOf preferred_name
        else "DUT"
           
    let disabled = 1= cmdline_flagset_validate stagename ["enable"; "disable" ] 0 op_args.c3
    let _ = 
        if disabled || filename = ""
        then vprintln 2 "SystemC/C++ stage is disabled";
        else                
            match length vms with
                | 0 -> vprintln 2 (sprintf "Zero VMs in input list - cannot render anything")
                
                | n ->
                    let filename = cpp_filename_sanitize filename
                    vprintln 2 (sprintf "%i VMs in input list - rendering first one only. Output file name is %s" n filename)
                    let design = hd vms // TODO handle multiple as per RTL output, e.g. for threaded coded 
        
                    let gen:xpp_gen_t =
                        { xpp_gen_args true(*cppf*) stagename filename op_args.c3
                            with cppf=true
                        }
                    let _ = convert_to_cpp ww msg gen orangepath_defs (design, fst(strip_suffix filename))
                    ()
    vms


(*
 * C# aka CSharp output: Convert to SystemC#
 *)
let opath_cvt_to_syscs ww (op_args:op_invoker_t) vms =
    let stagename = op_args.stagename
    let msg = op_args.banner_msg

// So that different values for filenames and so on can be used when this is invoked at more than one point in a recipe, we would like to use the stagename as the root of the command line flag names, but that is not supported by their pre-registration, so we will need to adjust opath to support a more flexible method in the future.... meanwhile we manually support editions 1 and 2 for up to two uses and conflate stagename and toolname.
    let pre = op_args.stagename + "-"
    let ww = WF 3 "cvt_to_syscs" ww ("Convert to C#/SystemC#. " + stagename + " : " + op_args.banner_msg)
    let disabled = 1= cmdline_flagset_validate stagename ["enable"; "disable" ] 0 op_args.c3

    let orangepath_defs = collate_defs_from_vm2_tree ww vms
        
    let _ = 
        if disabled then vprintln 2 (sprintf "SystemC/C#/SystemC# stage is disabled. stagename=%s" stagename)
        else
            match length vms with
                | 0 -> vprintln 2 (sprintf "Zero vms in input list - cannot render anything")
                | n ->
                    vprintln 2 (sprintf "%i VMs in input list - rendering first one only" n)
                    let design = hd vms // TODO handle multiple as per RTL output 
                    let preferred_name = vm_att_assoc g_preferred_name design
                    let filename0 = arg_assoc_or (stagename + "-filename") ("csharp", op_args.c3, [])

                    let filename =
                        if not_nullp filename0 && filename0 <> [ "" ] then hd filename0
                        elif preferred_name <> None then (valOf preferred_name)
                        else "DUT"


                    //let _ = vprintln 0 ("FN name " + htos filename + " " + boolToStr disabled + " " + boolToStr(filename=[]))
                    //let _ = vprintln 1 (bevelab_banner)
                    let filename = cpp_filename_sanitize filename
                    let gen:xpp_gen_t = xpp_gen_args false(*cppf*) stagename filename op_args.c3
                    let _ = convert_to_cpp ww op_args.banner_msg gen orangepath_defs (design, fst(strip_suffix filename))
                    ()
    vprintln 2 (sprintf "Return %i vms at top level, exactly as per input to recipe stage." (length vms))
    vms



//
// This file creates two plugins called cgen and csharpgen.  They share much common code.   
// The cgen could ideally be either plain C++ or SystemC but ...

let install_c_output_modes () =
    let gen_argpattern pre toolname =
        [
            Arg_int_defaulting(pre + "-loglevel", 1, "Verbosity level for C++/CSharp output (higher is more verbose)");                    
            Arg_defaulting(pre + "-topclass", "DUT", "Name of C++/SystemC/C# class to be generated");
            Arg_defaulting(pre + "-xtor",     "", "Transactor ?");            
            Arg_enum_defaulting(pre + "-ifshare", [ "none"; "simple"; "on" ], "on", "Whether to gather up 'if' statement bodies (when possible)");
            Arg_enum_defaulting(pre + "-add-aux-reports", ["enable"; "disable"], "enable", "Insert report file as comment into bottom of C++ file");          
        ] @ rez_gbuild_recipe_prefs toolname pre

    let systemc_argpattern toolname = // Ansi C or SystemC or some mix ?
        [
            Arg_enum_defaulting(toolname, ["enable"; "disable"; ], "enable", "Disable this stage");
            Arg_defaulting(toolname + "-filename", "", "Name of C++ file to be generated");
        ] @ gen_argpattern toolname toolname

        
    let systemcs_argpattern toolname =
        [
          Arg_enum_defaulting(toolname, ["enable"; "disable"; ], "enable", "Disable this stage (also requires a " + toolname + "-filename=filename to become enabled)");
          Arg_defaulting(toolname + "-filename", "", "Name of C# file to be generated");
        ] @ gen_argpattern toolname toolname
        
    let _ = install_operator ("cgen1",         "Generator SystemC/C++", opath_cvt_to_sysc,  [], [], systemc_argpattern "cgen1")
    let _ = install_operator ("csharpgen1",    "Generator SystemC#/C#", opath_cvt_to_syscs, [], [], systemcs_argpattern "csharpgen1")    
        
    let _ = install_operator ("cgen2",         "Generator SystemC/C++", opath_cvt_to_sysc,  [], [], systemc_argpattern "cgen2")
    let _ = install_operator ("csharpgen2",    "Generator SystemC#/C#", opath_cvt_to_syscs, [], [], systemcs_argpattern "csharpgen2")    
    ()

(* eof *)
