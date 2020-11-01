(*
 * HPR L/S sml defintions for output in the SMV model checker concrete syntax.
 *
 * $Id: smv_hdr.fs,v 1.11 2013-07-08 08:32:14 djg11 Exp $
 *)

module smv_hdr

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics


open hprls_hdr
open meox
open moscow
open yout
open abstracte
open opath_hdr

type smv_type_t = 
     | Smv_bool 
     | Smv_uword of int
     | Smv_word of int     
     | Smv_range of BigInteger * BigInteger
     | Smv_array of int * smv_type_t
     | Smv_enum of string list


type smv_diop_t = Smv_and | Smv_or | Smv_deqd | Smv_implies | Smv_xor | Smv_biimp | Smv_dltd | Smv_dled | Smv_plus | Smv_lshift | Smv_rshift | Smv_minus | Smv_filler


let g_max_id_len = 25

let id_shortens = new Dictionary<string, string>()

type smv_exp_t =
     | Smv_int64 of BigInteger * int option
     | Smv_id of string * hexp_t option
     | Smv_next of smv_exp_t
     | Smv_init of smv_exp_t
     | Smv_subsc of smv_exp_t * smv_exp_t
     | Smv_diop of smv_diop_t * smv_exp_t * smv_exp_t     
     | Smv_match of (smv_bexp_t * smv_exp_t) list
     | Smv_word1 of smv_bexp_t     
     | Smv_ctl of string * smv_exp_t
     
and smv_bexp_t = 
     | Smv_bid of string * hexp_t option
     | Smv_bnext of smv_bexp_t
     | Smv_not of smv_bexp_t
     | Smv_binit of smv_bexp_t
     | Smv_bdiop of smv_diop_t * smv_exp_t * smv_exp_t
     | Smv_bnode of smv_diop_t * smv_bexp_t * smv_bexp_t     
     | Smv_bmatch of (smv_bexp_t * smv_bexp_t) list
     | Smv_boolred of smv_exp_t
     | Smv_true
     | Smv_false


type un_t = Bexp of smv_bexp_t | Exp of smv_exp_t


// TODO : not global static for this
let g_id_2_smv = new OptionStore<string, un_t>("id_2_smv")



type smv_checkable_t =
    SMV_CHECKABLE of
        string *
        (string * un_t * smv_type_t) list *   // Declarations: with pre and post sanitize lhs
        (string * smv_exp_t * smv_exp_t) list *          // Initials 
        (string * smv_exp_t * smv_exp_t) list * // Defines: combination buffers, with pre and post sanitize lhs
        (string * smv_exp_t * smv_exp_t) list *          // Executables: next state function
        smv_exp_t list                  (* Assertions to be checked *)

let smv_diopToStr = function
    | (Smv_and) -> "&"
    | (Smv_or) -> "|"
    | (Smv_implies) -> "->"
    | (Smv_biimp)   -> "<->"
    | (Smv_dltd)   -> "<"
    | (Smv_dled)   -> "<="
    | (Smv_deqd)   -> "="
    | (Smv_xor)    -> "^" // correct?
    | (Smv_plus)   -> "+"
    | (Smv_minus)  -> "-"    
    | (Smv_lshift) -> "<<"
    | (Smv_rshift) -> ">>"
    | (other) -> sf( sprintf "smv_diopToStr???%A" other)



let rec smv_typeToStr = function
    | (Smv_bool)    -> "boolean"
    | Smv_word n    ->  "word[" + i2s n + "]"
    | Smv_uword n    ->  "unsigned word[" + i2s n + "]"    
    | Smv_range(f, t) ->  sprintf "%A:%A" f t 
    | (Smv_array(f, t)) ->  "array " + i2s f + " .. 0 of " + smv_typeToStr t 
    | Smv_enum l ->  
        let rec ff = function
             | [] -> ""
             | h::t -> h + (if t=[] then "" else ",  ") + (ff t)
        "{"  +  ff l + "}"


let rec smv_expToStr = function
    | Smv_id(s, xo)    -> s
    | Smv_diop(oo, l, r) -> 
        "(" + smv_expToStr l + smv_diopToStr oo + smv_expToStr r + ")"
    | Smv_word1 e      -> "word1(" + smv_bexpToStr e + ")"        
    | Smv_subsc(l, r) -> smv_expToStr l + "[" + smv_expToStr r + "]"
    | Smv_match l ->  "case " + List.foldBack (fun (a,b) c-> smv_bexpToStr a + ":" + smv_expToStr b + ";\n    " + c) l  "" + "esac"
    | Smv_ctl(s, e)    -> s + "(" + smv_expToStr e + ")"
    | Smv_next e       -> "next(" + smv_expToStr e + ")"
    | Smv_init e       -> "init(" + smv_expToStr e + ")"
    | Smv_int64(n, None)   -> n.ToString()
    | Smv_int64(n, Some w)   ->
        if n.Sign < 0 then "-0d" + i2s w + "_" + (-n).ToString()
        else "0d" + i2s w + "_" + n.ToString()   
    
and smv_bexpToStr = function
    | Smv_bid(s, xo)    -> s

    | Smv_not e        -> "!(" + smv_bexpToStr e + ")"
    | Smv_boolred e    -> "bool(" + smv_expToStr e + ")"

    | Smv_bnext e       -> "next(" + smv_bexpToStr e + ")"
    | Smv_binit e       -> "init(" + smv_bexpToStr e + ")"

    | Smv_true         -> "TRUE"
    | Smv_false        -> "FALSE"    
    | Smv_bmatch l ->  "case " + List.foldBack (fun (a,b) c-> smv_bexpToStr a + ":" + smv_bexpToStr b + ";\n    " + c) l  "" + "esac"
    | Smv_bdiop(oo, l, r) -> 
        "(" + smv_expToStr l + smv_diopToStr oo + smv_expToStr r + ")"

    | Smv_bnode(oo, l, r) -> 
        "(" + smv_bexpToStr l + smv_diopToStr oo + smv_bexpToStr r + ")"

let smv_unToStr = function
    | Exp e -> smv_expToStr e
    | Bexp be -> smv_bexpToStr be    


let smv_render_vars fd l =
    (
        yout fd ("VAR\n");
        app (fun (id, a, ty) -> yout fd ("    -- " + id  + "\n    " + smv_unToStr a + ": " + smv_typeToStr ty + ";\n")) l;
        yout fd ("\n");
        ()
    )     


let gec_Smv_word1 = function
    | Smv_boolred x -> x // TODO when x has width one only!
    | other -> Smv_word1 other
    
let gec_Smv_boolred = function
    | Smv_word1 x -> x 
    | Smv_int64(n, _) -> if n.IsZero then Smv_false else Smv_true
    | other ->
        //if ewidth "xbToSmv orred" e = Some 1 then zi inv
        //    else (Smv_bdiop(Smv_deqd, xToSmv e, Smv_int64 0L))
        Smv_boolred other

let asf fd oo (id, a, b) =
    match a with
        | Smv_init(Smv_word1 a) -> yout fd ("  init(" + smv_bexpToStr(a) + ")" + oo + smv_bexpToStr(gec_Smv_boolred b) + "; -- b ini\n")
        | Smv_next(Smv_word1 a) -> yout fd ("  next(" + smv_bexpToStr(a) + ")" + oo + smv_bexpToStr(gec_Smv_boolred b) + "; -- b nxt\n")
        | Smv_word1(a) -> yout fd ("   " + smv_bexpToStr(a) + oo + smv_bexpToStr(gec_Smv_boolred b) + "; -- b\n") 
        | a -> yout fd ("   " + smv_expToStr(a) + oo + smv_expToStr b + "; -- other \n")


let smv_render_execs fd l =
        (
        yout fd ("ASSIGN\n");
        app (asf fd " := ") l;
        yout fd ("\n");
        ())     


let smv_render_defines fd l =
        (
        yout fd ("DEFINE \n");
        app (asf fd " := ") l;
        yout fd ("\n");
        ())     

        
let smv_render_initials fd l =
        (
            // Initial predicates: the equals in the following is a deqd operator not an assignment!
        app (fun x -> yout fd ("INIT "); asf fd " = " x) l;
        yout fd ("\n");
        ())     


let smv_render_specs fd l =
        (
        yout fd ("SPEC\n");
        app (asf fd " := ") l;
        yout fd ("\n");
        ())     


type statespace_report_t =
    { scalars : int ref;
      vectors : int array;
      arrays : int array;
      otherbits : int ref;
      maxwidth : int;
      active : bool;
      continuous : int ref;
      }



let statespace_report(active) =
    {
        maxwidth=32768;
        active= active;
        scalars=ref 0;
        vectors=Array.create 32769 0;
        arrays=Array.create  32769 0;
        continuous=ref 0;
        otherbits=ref 0;
    } : statespace_report_t



let reportstate f com (sr:statespace_report_t) = 
    if not(sr.active) then f(com + " no state report\n") else
    let total = ref 0
    let rec rs m p (A:int array) =
        if p>sr.maxwidth then ()
        else
             (if A.[p]<>0 then f(com + i2s(A.[p]) + m + i2s(p) + "\n") else ();
              mutinc total (A.[p]*p);
              rs m (p+1) A
             )
    let rd m n = (mutinc total n; if n <> 0 then f(com + i2s n + m + "\n") else ())
    rs " vectors of width " 0 (sr.vectors)
    rs " array locations of width " 0 (sr.arrays)
    rd " other bits " (!(sr.otherbits))
    rd " bits in scalar variables" (!(sr.scalars))
    f(com + "Total state bits in module = " + i2s (!total) + " bits.\n")
    rd " continuously assigned (wire/non-state) bits " (!(sr.continuous))
    ()



(*
 *  Write an SMV checkable to a file.
 *)
let render_Smv_to_file id filename_option lst =
    let f1 = if nonep filename_option then id + ".smv" else valOf filename_option
    let f1 = filename_sanitize [ '+'; '.'; '_'; ] f1
    let prefix_to_discard = "_HPRLS_USER_"
    let f1 = if strlen f1 > strlen prefix_to_discard + 5 then f1.[strlen prefix_to_discard ..] else f1
    vprintln 3 ("Writing smv output to filename=" + f1)
    let fd = yout_open_out f1
    let _ = yout fd ("\n--SMV output from HPR L/S\n" +
                     "-- SMV file generated at " + timestamp(true) + "\n" +
                     "-- " + version_string(0) + "\n" +
                     "-- " + (!g_argstring) + "\n\n")
    let sr = statespace_report(true)
    let srx (id, arg, ty) =
        let wo, x = 
            match arg with
                | Bexp(Smv_bid(_, Some arg)) -> (Some 1, arg)
                | Exp(Smv_id(_, Some arg)) -> (ewidth "state report" arg, arg)
                    //if (not reg) then mutincu (sr.continuous) bits
        let (arrayf, aq) =
            match x with
                | X_bnet ff when ff.is_array ->
                    let f2 = lookup_net2 ff.n
                    (true, asize f2.length)
                | _                          -> (false, 0L)
        let _ =
            if wo<>None && valOf wo > sr.maxwidth then mutincu (sr.otherbits) (valOf wo)
            elif arrayf then Array.set sr.arrays (int32(valOf wo)) (int32 aq + sr.arrays.[int32(valOf wo)])
            elif wo<>None then Array.set sr.vectors (valOf wo) (1 + sr.vectors.[valOf wo])
            else mutincu (sr.scalars) (valOf wo)
        ()
            
    let sq = function
         | SMV_CHECKABLE(id, vars, initials, defines, execs, assertions) ->
            let id1 = "main"
            yout fd ("MODULE " + id1 + "\n\n")
            smv_render_vars fd vars
            if initials = [] then () else smv_render_initials fd initials
            
            smv_render_defines fd defines
            smv_render_execs fd execs
            //let _ =  if assertions <> [] then smv_render_specs fd assertions
            app srx vars // Does not include defines
            yout fd ("-- Number of vars (excluding defines): " + i2s(length vars) + "\n")
            reportstate (fun x->yout fd (x)) "-- " sr
            ()
    let _ = app sq lst


    yout fd ("--eof " + id + "\n\n")
    yout_close fd
    vprint 3 ("Saved " + f1 + " smv file.\n")
    ()


(* ... eof? *)


(*
 *  $Id: smv_hdr.fs,v 1.11 2013-07-08 08:32:14 djg11 Exp $ 
 * smv gen
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
 *)


open abstract_hdr
//open smv_hdr


(*-----------------------------------------------------------*)
(* Machine render as an SMV file *)
exception NoSmv of string;              


let enum_prefix = "EK_";


// Certain net names contain troublesome characters or are keywords: munge them here.
let smv_sanitize str =
    let (found, ov) = id_shortens.TryGetValue str
    if found then ov
    else
        let ok_list = []
        let l = str.Length
        if l <= g_max_id_len
        then
            let ans = filename_sanitize ok_list str
            id_shortens.Add(str, ans)
            ans
        else
            let ending = str.[l-g_max_id_len .. l-1]
            let ans = funique("smv") + "_" + filename_sanitize ok_list ending
            id_shortens.Add(str, ans)
            ans
       


(*
 * Entry point: Output complete design as SMV
 *)
let smv_tout ww filename_option ((ii0, mch0), idx) =
   
    let enums = ref []
    let nexttoken = ref 0

    // Add a numeric suffix to subsequent machines when/if more then one machine is being converted.
    let filename_option = if nonep filename_option || idx = 0 then filename_option else Some(valOf filename_option + "_" + i2s idx)

    // SMV supprts enumeration types. : Does not matter if hpr type is open or closed when we output this snapshot.
    let smv_exp_enum_lookup s0 = 
        let s = enum_prefix + s0
        let ov = op_assoc s !enums
        if ov<>None then Smv_id(smv_sanitize s, None)
        else
            let k = !nexttoken
            mutinc nexttoken 1
            mutadd enums (s, k)
            Smv_id (smv_sanitize s, None)
        

    let smv_exp_enums() = Smv_enum(map (fun (a,b) -> smv_sanitize a) (!enums))


    let rec xbToSmvDiop = function
        | (V_dltd Signed) -> Smv_dltd
        | (V_dled Signed) -> Smv_dled
        | (oo) -> sf(f3o4(xbToStr_dop oo) + ": xbToSmvDiop other")
    and xToSmvDiop = function
        | V_bitor    -> Smv_or
        | V_bitand   -> Smv_and        
        | V_plus     -> Smv_plus
        | V_minus    -> Smv_minus
        | V_lshift   -> Smv_lshift
        | V_rshift _ -> Smv_rshift
        | oo        -> sf(f3o3(xToStr_dop oo) + ": xToSmvDiop other")
    let zi inv bx = if inv then Smv_not bx else bx

    let rec xbToSmv arg =
        match arg with
        | W_bnode(V_band, [l; r], false, _) -> Smv_bnode(Smv_and, xbToSmv l, xbToSmv  r)
        | W_bnode(V_bor, [l; r], false, _)  -> Smv_bnode(Smv_or, xbToSmv l, xbToSmv  r)
        | W_bnode(V_bxor, [l; r], false, _) -> Smv_bnode(Smv_xor, xbToSmv l, xbToSmv  r)
        
        | X_true  -> Smv_true
        | X_false -> Smv_false
        | W_bdiop(V_orred, [e], inv, _) -> gec_Smv_boolred(xToSmv None e)

        | W_bdiop(oo, [l;r], inv, _) ->
            let wlo = ewidth "bdiop-l" l
            let wro = ewidth "bdiop-r" r
            let resolve = function
                | (None, None) -> None
                | (None, Some x)
                | (Some x, None) -> Some x                
                | (Some x, Some y) ->
                    if x=y then Some x
                    else
                        vprintln 3 ("Width mismatch in " + xbToStr arg)
                        Some (if x>y then x else y)

            let ow = resolve (wlo, wro)
            let qo = function
                | V_deqd -> Smv_deqd
                | V_dltd Signed -> Smv_dltd
            zi inv (Smv_bdiop(qo oo, xToSmv ow l, xToSmv ow r))

        | W_bitsel(v, bit, inv, _) ->            // bit select, r-mode expansion using shift and mask
            let lx = ix_rshift Unsigned v (xi_num bit)
            let lx = ix_bitand (xi_num 1) lx
            zi (not inv) (Smv_bdiop(Smv_deqd, xToSmv (Some 1) lx, xToSmv (Some 1) (xi_num 1)))


        | W_cover(lst, _) ->
            let tc = xbToSmv
            let c_logor (a, b) = Smv_bnode(Smv_or, a, b)
            let c_logand (a, b) = Smv_bnode(Smv_and, a, b)            

            let rec sums = function
                | []     -> muddy "g_vtrue // should be unused."
                | [item] -> tc(deblit item)
                | h::tt  -> c_logand (tc(deblit h), sums tt)
            let rec prods = function
                | []     -> muddy "g_vfalse // should be unused."
                | [item] -> sums item
                | h::tt  -> c_logor (sums h, prods tt)
            prods lst
            
        | other -> sf ("xbkey=" + xbkey other + ": xbToSmv other:" + xbToStr other)

    and xToSmv ow arg00 =
        let gwidth prec =
            if not_nonep ow then valOf ow
            elif not_nonep prec.widtho then valOf prec.widtho // this ordering needs checking on a clause-by-clause basis
            else sf ("smv: need width in " + xToStr arg00)
        match arg00 with

        | X_undef ->
            // does SMV have don't care?
            let ans = xi_num 0
            vprintln 1 (sprintf "smv output warning: Render X_undef as %s"  (xToStr ans))
            xToSmv None ans
                
        | X_bnet f   ->
            match g_id_2_smv.lookup f.id with
                | Some ov ->
                    match ov with
                        | Bexp bx -> Smv_word1(bx)
                        | Exp x -> x
                | None ->
                    vprintln 1 ("+++ " + f.id + " used without declaration")
                    let w = ewidth "xToSmv sans decl" (X_bnet f)
                    if w=Some 1 then Smv_word1(Smv_bid(smv_sanitize f.id, Some (X_bnet f)))
                    else Smv_id(smv_sanitize f.id, Some (X_bnet f))

        | X_bnum(prec, bn, _)  ->
            let width_o =
                if nonep prec.widtho then ow else prec.widtho
            Smv_int64(bn, width_o)
        | X_num n              -> Smv_int64(BigInteger n, ow)
        | X_blift e            -> gec_Smv_word1(xbToSmv e)
        | W_string(s, _, _)    -> smv_exp_enum_lookup s
        | X_subsc(a, b)        -> Smv_subsc(xToSmv ow a, xToSmv None b)
        | X_x(e, 1, _)         -> Smv_next(xToSmv ow e)

        // Monadic operator: two's complement, can render as subtract from zero since smv supports subtract.
        | W_node(prec, V_neg, [arg], _) ->
            let zero = xToSmv ow (xi_num 0)
            let a1 = xToSmv ow arg
            Smv_diop(Smv_minus, zero, a1)

        // Monadic operator: one's complement, can render as bitwise xor if we have the width
        | W_node(prec, V_onesc, [arg], _) ->
            let width = gwidth prec
            let flip = gec_X_bnum({widtho=Some width; signed=Unsigned}, himask width)
            //vprintln 0 ("Made himask " + i2s width)
            let a1  = xToSmv ow arg
            Smv_diop(Smv_xor, xToSmv ow flip, a1)

        // Monadic operator: cast.
        | W_node(prec, V_cast cvtf, [arg], _) ->
            let a1 = xToSmv None arg
            match cvtf with
                | CS_typecast ->                  // Just a typecast that may alter a subsequent operator overload selection.
                    a1
                | CS_maskcast ->                  // Clipping etc to field width.
                    let width = gwidth prec
                    let mask = gec_X_bnum({widtho=Some width; signed=Unsigned}, himask width)
                    Smv_diop(Smv_and, xToSmv (Some width) mask, a1)
                | CS_preserve_represented_value -> // A major bit-changing convert (e.g. int to float).
                    muddy (sprintf "Unsupported smv cast " + xToStr arg00)
            //xToSmv ow arg

        | W_node(prec, vo, [l; r], _) -> Smv_diop(xToSmvDiop vo, xToSmv ow l, xToSmv ow r)

        | W_node(prec, vo, lst, _) ->
            muddy (sprintf "polyadic panglish len=%i vo=%A" (length lst) vo)

        | W_query(g, t, f, _)  ->
            let here() = (xbToSmv g, xToSmv ow t)
            let bhere() = (xbToSmv g, xbToSmv(xi_orred t))  //Is this a corred reduction always? TODO: we could have a query between a wide word and an item of width 1
            let tailer = xToSmv ow f
            match tailer with
                | Smv_word1(Smv_bmatch lst) -> muddy "here"
                | Smv_match lst -> Smv_match(here() :: lst)
                | Smv_word1 other -> Smv_word1(Smv_bmatch [ bhere(); (Smv_true, other) ])
                | other -> Smv_match [ here(); (Smv_true, other) ]                

        | other ->
            let msg = "xkey=" + xkey other + ": xToSmv other form not supported " + xToStr other
            vprintln 0 (msg)
            raise (NoSmv msg)

    let rec exec_to_smv2 clk g (dc, ec) = function
        | Xassign(X_x(l, 1, _), r) ->
            let rr = xi_query(g, r, l)
            let ow = ewidth "Xassign find" rr
            let a = (xToStr l, xToSmv ow (xi_X 1 l), xToSmv ow rr)
            (dc, a::ec)
            
        | Xblock l -> List.fold (exec_to_smv2 clk g) (dc, ec) l // nb: reverses
                
        //| Xif(_, Xeasc(x_apply _), Xskip) ->
        //| Xif(g1, tt, ff), c) ->
        //exec_to_smv2 clk (x_logand(g, g1)) (tt, exec_to_smv2 clk (xgen_logand(xgen_lognot g, g1)) (ff, c))
        | Xskip -> (dc, ec)

        | other -> sf(hbevToStr other + " exec_to_smv2 other\n")

    let rec rtl_to_smv_rtl clk g0 (dc, ec) = function
        | XRTL_nop _ -> (dc, ec)
        | XRTL(None, ga, lst) ->
            let sn cde (dc, ec) =
                match cde with
                | Rarc(gb, l ,r) ->
                    let gg = xi_and(g0, xi_and(ga, gb)) // TODO
                    let  rr = xi_query(gg, r, l)
                    let ow = ewidth "XIRTL_arc" rr
                    let a = (xToStr l, xToSmv ow (xi_X 1 l), xToSmv ow rr)
                    (dc, a::ec)
                | Rpli _ ->            (dc, ec) // For now
                | Rnop s -> (dc, ec)
            List.foldBack sn lst (dc, ec)

        | XIRTL_buf(g, l ,r) ->
            let gg = xi_and(g0, g) // TODO
            let rr = xi_query(gg, r, l)
            let ow = ewidth "XIRTL_buf" rr
            let a = (xToStr l, xToSmv ow l, xToSmv ow rr)
            (a::dc, ec)

        | XIRTL_pli(ppo, g, ((f, _), _), args) ->
            (dc, ec) // For now
        | other -> sf ("exec_to_smv_rtl other: " + xrtlToStr other)
        
    let rec exec_to_smv1 clk c = function
        | SP_par(_, a)        -> List.fold (exec_to_smv1 clk) c a
        | SP_l(ast_ctrl, bev) -> exec_to_smv2 clk X_true c bev
        | SP_rtl(ii_, lst)    -> List.fold (rtl_to_smv_rtl clk X_true) c lst
        | other               -> sf ("exec_to_smv1: " +  hprSPSummaryToStr other)

    let exec_to_smv c = function
        // Strictly here should collate on clock domains and issue as separate SMV processes.
        | H2BLK(clk, a) -> exec_to_smv1 clk c a
//        | (_) -> sf ("exec_to_smv other")

    let rec smv_type (X_bnet ff) = 
        let enumer (kn) = "ENUM_" + enumfToStr kn
        let enumo = is_enum_var_n ff.n
        let enumo_grp = if nonep enumo then None else g_strobe_group_members.lookup(valOf enumo)
        let isclosed_enum = not_nonep enumo_grp && not_nonep(f4o4(valOf enumo_grp))
        let rr = 
            if isclosed_enum then
                    // If not closed, we might include one dummy element to reflect future possibilities?  Is one actually enough ?  Some predicates might rely on their being more than one distinct future element... so lets do this only for closed groups.
                let (_, ovl, _, _) = valOf enumo_grp
                Smv_enum(map enumer ovl)
            elif (ff.rl = 0I && ff.rh = 1I) || ff.width = 1 then Smv_bool
            elif ff.rh = 0I && ff.signed=Unsigned then Smv_uword(ff.width)
            elif ff.rh = 0I && ff.signed=Signed then Smv_word(ff.width)
            else Smv_range(ff.rl, ff.rh)
        if not ff.is_array then rr
        else
            let f2 = lookup_net2 ff.n
            Smv_array(int32(hd f2.length) - 1, rr) // nasty cast - use asize.

    let var_to_smv (X_bnet f) =
        let ty = smv_type (X_bnet f)
        let isbool = function
            | Smv_bool -> true
            | _        -> false
        let rr =
            if isbool ty then Bexp(Smv_bid(smv_sanitize f.id, Some(X_bnet f)))
            else Exp(Smv_id(smv_sanitize f.id, Some(X_bnet f)))         
        g_id_2_smv.add f.id rr
        (f.id, rr, ty)

    // This flattens VM machines, but SMV supports module hierarchy, so we could preserve (todo).
    let reset_generator() =
        let n = reset_net()
        let flop = xi_orred(vectornet_w("SMV_RESET_GENERATOR", 1))
        // (Smv_init(xToSmv flop), xbToSmv X_false); - Has its own init property.
        let ii = { id="from_smv_reset" } : rtl_ctrl_t
        let logic = SP_rtl(ii, [ XRTL(None, X_true, [ Rarc(X_true, xi_blift flop, xi_num 1)]) ])
        let defines = [ (xToStr n, xToSmv (Some 1) n, gec_Smv_word1(xbToSmv(if !g_reset_is_active_low then xi_not flop else flop))) ]
        //
        vprintln 3 ("Defined reset flip-flop " + xbToStr flop)
        let dir = { g_null_directorate with clocks=[E_pos g_clknet]; duid=next_duid() }
        ([xi_blift flop], defines, [H2BLK(dir, logic)])
        
    let find_inits c x =
        let iv = get_init "find_inits" x
        let ow = ewidth "find_inits" x
        if iv=None then c else (xToStr x, xToSmv ow x, Smv_int64(valOf iv, ow)) :: c
        
    let rec yielde prefix cc (ii, mch) =
        match mch with
            | None -> cc // TODO should look-up the referenced machine in a generator
            | Some(HPR_VM2(minfo, decls, sons, execs, assertions)) ->
                let (r_vars, r_defines, r_execs) = reset_generator()
                let r_vars =
                    let cpi = { g_null_db_metainfo with kind= "smv-r_vars" }
                    [DB_group(cpi, map db_netwrap_null r_vars)]

                let allnets = map snd (List.foldBack (db_flatten []) (r_vars@decls) [])
                let vars0 = map var_to_smv allnets // Do the definitions first: sets entries in the dictionary.

                let (defines, xecs) = List.fold exec_to_smv (r_defines, []) (r_execs @ execs)
                let defs = map f1o3 defines
                let qp = function
                    | (id, _, _) when memberp id defs -> false
                    | _ -> true
                let vars = List.filter qp vars0
                let inits = List.filter qp (List.fold find_inits [] allnets)
                let cc = List.fold (yielde (ii::prefix)) cc sons
                let assertions' = [] // for now
                let checkable = SMV_CHECKABLE(ii_fold(ii::prefix), vars, inits, defines, xecs, assertions')
                checkable :: cc
                
    let lst = yielde [] [] (ii0, mch0) 
    let ans = render_Smv_to_file (ii_fold [ii0]) filename_option lst
    ()


(*
 * Write out as SMV (allowable forms only)
 *)
let opath_smv_writer ww op_args vms =
    // let disabled = 1= cmdline_flagset_validate op_args.stagename ["enable"; "disable" ] 0 c1

    let enable = (control_get_s op_args.stagename op_args.c3 op_args.stagename None) = "enable"
    
    let filename_option =
        match control_get_s op_args.stagename op_args.c3 "smv" None with
            | "enable" -> None
            | filename ->
                let (root__, old_suffix) = strip_suffix filename
                let filename = if old_suffix = "" then filename + ".smv" else filename
                vprintln 2 (sprintf "smv_writer: set filename to  %A" filename)
                Some filename

    if enable then app (smv_tout ww filename_option) (zipWithIndex vms)
    vms


open opath_hdr
open opath_interface

let install_smv() =
    let toolname = "smv-gen"
    let argpattern =
        [
          Arg_defaulting("smv-rootmodname", "", "Root module name in output SMV file.");
          Arg_defaulting("smv", "smvout", "Output file name for SMV.");

        
          Arg_int_defaulting("pagewidth", 132, "Page column width for pretty printer");

          //Arg_enum_defaulting("smv-separate-files", ["enable"; "disable"], "disable", "Write each SMV module to a new file.");
          Arg_enum_defaulting(toolname, ["enable"; "disable"], "enable", "Enable control for this operation");
        ]
    install_operator (toolname, "Generate SMV (allowable forms only)", opath_smv_writer, [], [], argpattern)
    ()


// Make appear as a loadable plugin
type Smv_gen() =
    interface IOpathPlugin with
        member this.OpathInstaller() = install_smv()
        member this.MethodName = "install_smv"
    
(* eof *)
