(*
 *  $Id: microcode.fs $
 *
 * CBG Orangepath.
 * HPR L/S Formal Codesign System.
 *
 *
 * Microcode.sml
 * Contains a compiler and emulator for microprocessor machine code.
 * See the csew platform for a standalone assembler.
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
 *
 *)

module microcode

open hprls_hdr
open moscow
open yout
open protocols
open microcode_hdr
open m6805_hdr
open meox
open abstract_hdr
open abstracte
open linepoint_hdr
open verilog_hdr
open verilog_gen

type itsa_t = 
  ITSA_reg of int
| ITSA_mem of string
| ITSA_const of string
| ITSA_symb of string
| ITSA_indexed of string * itsa_t
| ITSA_void of string


type icode_lab_t = 
  ICODE_FWD of int
| ICODE_FWD_G of itsa_t * int
| ICODE_LAB of int
| ICODE_START
| ICODE_UDEF of string



let ii_stop = Ij(i_stop, NAM, NAM, NAM, ref 0);
let ii_rts = Ij(i_rts, NAM, NAM, NAM, ref 0);
let ii_nop = Ij(i_nop, NAM, NAM, NAM, ref 0);

type object_container_t = (int * int) list ref

type lititem_t = LITITEM_chars of int * string * char list
;


type DIC_t = {  id: string; 
                r: icode_t array;
                ptr: int ref; 
                len: int; 
                ep: (int * int) list option ref;
                stack :  itsa_t ref;
                litpool : lititem_t list ref;
                object_code : object_container_t;
                origin: int
             }


let rec amodeToS = function
    |(VREG v) -> "V" + (i2s v)
    |(PREG v) -> "R" + (i2s v)
    |(IMMED v) -> "->" + (i2s v)
    |(IMMEDS(s, ir)) -> "=" + s + "; v=" + (if !ir = None then "NONE" else i2s(valOf (!ir)))
    |(ABS v) ->  (i20x v)
    |(ABSS s) ->  s
    |(INDEXEDS(s, a)) ->  s + "[" + (amodeToS a) + "]"
    |(NAM) -> "???NAM"
    |(_) -> "???"
;


let xcomp_debug(f) = print(f())

let mc_disassemble_alu x =
    let rec s = function
        | [] -> "???"
        | ((opcode, code, nem, exec)::t) -> if x=code then nem else s t
    in s alu_codes
    

let rec mc_disassemble1(ins:inst_t, a0, a1, a2, n) = 
        (ins.nm) +
                 " " +
                 (
                   if (ins.am = "0") then ""
                   elif (ins.am = "R") then (amodeToS a0)   
                   elif (ins.am = "Di") then (amodeToS a0) + "," + (amodeToS a1) + " ; i=" + (i2s(!n))
                   elif (ins.am = "DA") then (amodeToS a0) + "," + (amodeToS a1)
                   elif (ins.am = "DAB") then (amodeToS a0) + "," + (amodeToS a1) + ", " + (amodeToS a2)
                   elif (ins.am = "RXb") then (amodeToS a0) + "," + (amodeToS a1)  (* b is inside X *) + " ; b=" + (i2s(!n))
                   elif (ins.am = "RL") then (amodeToS a0) + "," + (amodeToS a1) + " ; L=" + (i2s(!n))
                   elif (ins.am = "L") then (amodeToS a0) + " ; " + (i2s(!n))
                   else "???bad am"
                 )



let rec mc_disassemble = function
    | Ij(ins, a0, a1, a2, n)      -> "        " + ( mc_disassemble1(ins, a0, a1, a2, n))
    | (Icompiled(ins, code)) -> (mc_disassemble ins) + "\n>>" + xToStr code
    | (Icomment s) -> "; " + s
    | (Ilabel(s, ins)) -> s + ":" + mc_disassemble(ins)
    | (Idefs(v, s)) -> "        defs " + (i20x v) + "; " + s
    |  (v) -> "         mc_disassemble: Other instruction???"
;





let new_icode(id) =
    let len = 5000
    let r = Array.create len ii_stop
    let ptr = ref 0
    in { id=id; r=r; ptr=ptr; len=len; ep=ref None; stack=ref (ITSA_void "no-return-from-here"); litpool= ref []; object_code= ref []; origin=0; }


// let new_bc_labs() = (ref(ICODE_UDEF "break"), ref(ICODE_UDEF "continue"), (ref(ICODE_START), ref []))


let new_bc_labs() = ("dummy", None, None, (funique "_RLA", ref []))


let bmon_manifest_true = function
    | (ITSA_reg _) -> false
    | (ITSA_mem n) -> false
    | (ITSA_indexed _) -> false
    | (ITSA_const n) -> (atoi n) <> 0I
    | (ITSA_void s) -> sf(s + ": expression required")
    | (_) -> sf("bmon_manifest other")
;

let bmon_manifest_false = function
    | (ITSA_reg _) -> false
    | (ITSA_mem n) -> false
    | (ITSA_indexed _) -> false
    | (ITSA_const n) -> (atoi n) = 0I
    | (ITSA_void s ) -> sf(s + ": expression required")
    | (_) -> sf("bmon_manifest other")
;

let itsa_false_value = ITSA_const "0"
let itsa_true_value = ITSA_const "1"





let regindx = ref 1

(* Fresh and new virtual register number *)
let x_compile_fresh() =
    let r = !regindx 
    let _ = regindx := r+1
    in (VREG r, r)



(*
 * Generate an assembly code line (here called icode) and append it to DIC. 
 *)
let icode_emit(v, dic:DIC_t) =
    let ptr = dic.ptr
    let p = !ptr
    ptr := p+1
    if p > (dic.len - 20) then sf("Too much intermediate code generated")
    Array.set dic.r p v
    lprintln 10 (fun () -> i2s(!ptr) + mc_disassemble v)
    ()


let rec x_compile_loadto dic a =
    match a with
        | ITSA_void s -> VREG -1
        | ITSA_reg r -> VREG r
        | v ->
            let (r, _) = x_compile_fresh()
            let _ =
                match (v) with
                | ITSA_const j -> icode_emit(Ij(i_lodi, r, IMMEDS(j, ref(Some(int32(atoi j)))), NAM, ref -1), dic)
                | (ITSA_indexed(s, j)) -> icode_emit(Ij(i_lod, r, INDEXEDS(s, x_compile_loadto dic j), NAM, ref -1), dic)
                | (ITSA_symb j) -> icode_emit(Ij(i_lodi, r, IMMEDS(j, ref None), NAM, ref -1), dic)
                | (ITSA_mem j) -> icode_emit(Ij(i_lod, r, INDEXEDS(j, VREG 999), NAM, ref -1), dic)
                | (j) -> sf("not: itsa loadto")
            in r



(* load effective address *)
let x_compile_load_ea dic v =
    let (r, _) = x_compile_fresh()
    let _ = 
        match (v) with
        | ITSA_indexed(s, j) -> icode_emit(Ij(i_add, r, IMMEDS(s, ref None), x_compile_loadto dic j, ref -1), dic)
        | ITSA_mem j         -> icode_emit(Ij(i_add, r, IMMEDS(j, ref None), VREG 999, ref -1), dic)
        | (_) -> sf("not: load_ea")
    in r


let tomcu = function
    | (V_xor) -> i_xor
    | (V_bitand) -> i_and
    | (V_bitor) ->  i_or
    | (V_plus) ->  i_add
    | (V_minus) ->  i_sub
    | (V_rshift Unsigned) ->  i_lsr (* TODO: this function is not complete - look up in the table *)
    | (V_rshift Signed) ->  i_asr (* TODO: this function is not complete - look up in the table *)    
    | (V_times) ->  i_mul
    | (a) -> sf(f1o3(xToStr_dop a) + " tomcu other")


(*
 * dltd "A<B" is compiled as "A cmi B" which means "A-B<0?1:0".
 * Since 
 *)
let x_compile_compare = function
    | (V_deqd, l, r) -> (i_cpz, l, r, false)
    | (V_dned, l, r) -> (i_cpz, l, r, true)
    | (V_dltd Signed, l, r) -> (i_cmi, l, r, false) (* l-r isless than zero iff l<r *)
    | (V_dled Signed, l, r) -> (i_cmi, r, l, true) (* l<->r iff !(r<l) *)
    | (oo, l, r) -> sf (sprintf "x_compile_compare other operator %A" oo)


let x_compile_math (oo, l, r) = (tomcu oo, l, r, false)



(* Test and set *)
let icode_tas(mutexaddr, resultreg, dic) =
    (
     icode_emit(Ij(i_tas, mutexaddr, resultreg, NAM, ref -1), dic);
     ()
    )


let rec x_compile_tas e dic (a1, a2) =
    let (ll, _) = x_compile_e e dic a1

    let clearme = function
        | (ITSA_mem m) ->
           let (rr, _) =  x_compile_e e dic a2
           let r = x_compile_loadto dic rr
           let _ = icode_emit(Ij(i_str, r, INDEXEDS(m, VREG 999), NAM, ref -1), dic)
           in (rr, [])
        | (_) -> sf("illegal mutex in microcode test and set clear")

    let setme(a1) =
        let (rr, _) =  x_compile_e e dic a2
        let (r, resi) = x_compile_fresh()
        let mutex = x_compile_load_ea dic a1
        let _ = icode_tas(mutex, r, dic)
        in (ITSA_reg resi, [])

    let ans = if a2 = xi_num 1 then setme(ll) elif a2 = xi_num 0 then clearme(ll) else sf("Bad arg to test and set")
    in ans

and bmonkey msg e dic g = 
    let (r, _) = x_bcompile_e e dic g
    in r

and x_compile_ee e dic v = fst(x_compile_e e dic v)

and x_bcompile_ee e dic v = fst(x_bcompile_e e dic v)


and x_bcompile_e e dic x =
    match x with
    | W_bdiop(oo, [l; r], false, _) ->
        let (l', _) = x_compile_e e dic l
        let (r', _) = x_compile_e e dic r
        let l'' = x_compile_loadto dic l' (*Should optimise here*)
        let r'' = x_compile_loadto dic r' (*Should optimise here*)
        let (res, resi) = x_compile_fresh()
        let (oo', l3, r3, flip) = x_compile_compare(oo, l'', r'')
        let _ = icode_emit(Ij(oo', res, l3, r3, ref 0), dic)
        let _ = if (flip) then icode_emit(Ij(i_not, res, res, res, ref 0), dic)
        (ITSA_reg resi, [])
        
    |  W_bdiop(V_orred, [v], false, _) -> 
        let w = ewidth "x_bcompile_e" v
        let (v', l) = x_compile_e e dic v
        let poormans_orred(a) = 
            let res0 = x_compile_loadto dic a 
            let (res, resi) = x_compile_fresh()
            in
                (  icode_emit(Ij(i_not, res, res0, res0, ref 0), dic);
                   icode_emit(Ij(i_not, res, res, res, ref 0), dic); 
                   (ITSA_reg resi, [])
                )
        let _ = lprint 1000 (fun()->xToStr v + " width " + (if w=None then "NONE" else i2s(valOf w)) + "\n")
        in if w = Some 1 then (v', l) else poormans_orred(v') (* It would be better to have a variants itsa_reg form *)
    |  (X_true) -> x_compile_e e dic (X_num 1)
    |  (X_false) -> x_compile_e e dic (X_num 0)
    |  (X_dontcare) -> (ITSA_void "x_dontcare", [])
    |  (_) -> sf "x_bcompile_e other"

and x_compile_e e dic arg =
    match arg with
    | X_blift v -> x_bcompile_e e dic v
    | X_bnet ff -> (ITSA_mem ff.id, [])
    | W_string(s, xs, _) -> 
        let e = explode s
        let symb = symbolic_check e
// Old for reference:        
//      let kk(LITITEM_chars(here, lit, e)) = if xs<>XS_fill || symb then (ITSA_const s, []) else (ITSA_symb lit, [])
        let kk(LITITEM_chars(here, lit, e)) =
            match xs with
                | XS_fill n when not symb ->  (ITSA_symb (baseup_string n lit), []) // Strange - not symb deploys ITSA_symb ?
                | other -> (ITSA_const s, [])
        let rec scan = function
            | [] -> None
            | ((LITITEM_chars(h, l, e')) as aa)::t -> if e' = e then Some(kk aa) else scan t
            //| _::t -> scan t
        let ov = scan (!(dic.litpool))
        if ov <> None then valOf ov
        else 
           let lit = funique "LIT"
           let here = !(dic.ptr)
           let ans = LITITEM_chars(here, lit, e)
           let _ = if (symb) then () else (mutadd (dic.litpool) (ans))
           kk ans

    | W_apply(("hpr_testandset", gis), _, [a1; a2], _) -> x_compile_tas e dic (a1, a2)

    | W_query(g, t, f, _) -> 
        let gg' = bmonkey "guard to beq statement" e dic g
        if bmon_manifest_true gg' then x_compile_e e dic t
        elif bmon_manifest_false gg' then x_compile_e e dic f
        else
            let middle_lab = funique("_LQM")
            let final_lab = funique("_LQF")
            let g3 = x_compile_loadto dic gg'
            let lab = icode_emit(Ij(i_brz, g3, ABSS middle_lab, NAM, ref -1), dic)

            let a_t = x_compile_loadto dic (fst(x_compile_e e dic t))
            let _ = icode_emit(Ij(i_jmp, ABSS final_lab, NAM, NAM, ref -1), dic)
            let _ = icode_emit(Ilabel(middle_lab, Icomment ""), dic)
            let a_f = x_compile_loadto dic (fst(x_compile_e e dic f))
            
            let mover = function
                | (VREG n_to, VREG n_from) -> if n_to=n_from then () (* unlikely since vregs ! *)
                                              else icode_emit(Ij(i_mov, VREG n_to, VREG n_from, NAM, ref -1), dic)
                | (_) -> sf("other: query mover")
            let _ = mover (a_t, a_f)
            let _ = icode_emit(Ilabel(final_lab, Icomment ""), dic)
            let u = function
                | (VREG n) -> n
                | _ -> sf("other: query u")
            in (ITSA_reg(u a_t), [])
    | X_num ss -> (ITSA_const(i2s ss + ":num"), [])
    | X_undef  -> (ITSA_void "x_undef", [])
    | X_x(v, 1, _) -> x_compile_e e dic v

    | W_asubsc(l, r, _) ->
        let (l', _) = x_compile_e e dic l
        let (r', _) = x_compile_e e dic r
        let k = function
            | (ITSA_mem s) -> s
            | (ITSA_symb s) -> s
            | _ -> sf(xToStr arg + " : cannot ucode compile") 
        in (ITSA_indexed(k l', r'), [])

    |  W_node(prec, oo, [l; r], _) ->
        let (l', _) = x_compile_e e dic l
        let (r', _) = x_compile_e e dic r
        let l'' = x_compile_loadto dic l' (*Should optimise here*)
        let r'' = x_compile_loadto dic r' (*Should optimise here*)
        let (res, resi) = x_compile_fresh()
        let (oo', l3, r3, flip) = x_compile_math(oo, l'', r'')
        icode_emit(Ij(oo', res, l3, r3, ref 0), dic)
        if (flip) then icode_emit(Ij(i_not, res, res, res, ref 0), dic)
        (ITSA_reg resi, [])

    | (other) -> sf((xToStr other) + ": other in x_compile_e")

and x_compile_not dic v =
    if bmon_manifest_false v then itsa_true_value
    elif bmon_manifest_true v then itsa_false_value
    else
        let r = x_compile_loadto dic v
        let  (s, si) = x_compile_fresh()
        icode_emit(Ij(i_not, s, r, r, ref 0), dic)
        ITSA_reg si



(* 
 * A literal pool is a region of memory containing initialised constants.
 * 
 *)
let emit_litpool (dic:DIC_t) =
    let lits = dic.litpool
    let chd (p:char) = Idefs(System.Convert.ToInt32 p, implode[p])
    let defb p = icode_emit(chd p, dic)
    let emit(LITITEM_chars(user, label, contents)) = 
        (
                icode_emit(Ilabel(label, chd(hd contents)), dic);
                app defb (tl contents);
                icode_emit(Idefs(0, ""), dic)
        )
    let _ = app emit (!lits)
    in lits := []

(* Unconditional goto: followed by litpool opportunity *)
let icode_goto(l, dic) =
    (
     icode_emit(Ij(i_jmp, ABSS l, NAM, NAM, ref -1), dic);
     emit_litpool  dic;
     ()
     )

(*
 * Compiler for hbev_t raw to microcode.
 *)
let rec x_compile_ec m e x =
    let dic = new_icode(funique "EC-TEMP-dic")
    let ov = !(dic.ptr)
    let rr  = x_compile_e e dic x
    if !dic.ptr <> ov then cleanexit(m + ": no side effects allowed here: " + xToStr x)
    rr

and x_compile_mc (e, b, c, r) dic = function
    | Xlinepoint(x, a) -> 
        let old = log_linepoint x
        let _ = icode_emit(Icomment(lpToStr x), dic)
        let rr = x_compile_mc (e, b, c, r) dic a
        //log_linepoint old
        rr
    | Xbeq(gg, s, _) -> 
        let gg' = bmonkey "guard to beq statement" e dic (gg)
        if bmon_manifest_true gg' then ()
        elif bmon_manifest_false gg' then icode_emit(Ij(i_jmp, ABSS s, NAM, NAM, ref -1), dic)
        else
            let g3 = x_compile_loadto dic gg'
            let lab = icode_emit(Ij(i_brz, g3, ABSS s, NAM, ref -1), dic)
            ()
#if SPARE
    | Xcall((s, gis), l) -> 
        let argcnt = length l
        let jg(v) = 
            let rr = x_compile_loadto dic (x_compile_ee e dic v)
            icode_emit(Ij(i_push, rr, NAM, NAM, ref 0), dic)
        app jg (rev l)
        icode_emit(Ij(i_jsr, ABSS s, NAM, NAM, ref -1), dic)
        if (argcnt > 0) then icode_emit(Ij(i_disc, IMMED argcnt, NAM, NAM, ref 0), dic)
        ()
#endif        
    | (Xassign(lhs, rhs)) -> 
        let (ll, _) = x_compile_ec "lhs of assignment rule subscripts"  e (lhs)
        let (rr, _) =  x_compile_e e dic (rhs)
        let r = x_compile_loadto dic rr

        let u = function
            | (ITSA_mem m) -> icode_emit(Ij(i_str, r, INDEXEDS(m, VREG 999), NAM, ref -1), dic)
            | (ITSA_const m) -> icode_emit(Ij(i_str, r, INDEXEDS(m, VREG 0), NAM, ref -1), dic) (* Should check not an n: const *)
            | (ITSA_indexed(m, idx)) -> icode_emit(Ij(i_str, r, INDEXEDS(m, x_compile_loadto dic idx), NAM, ref -1), dic) 
            | (_) -> sf("illegal lvalue in microcode assign")
        u ll
    (*| (Xe_as_c(el)) -> 
        let _ = x_compile_e e dic el
        in ()
    *)

    | (Xbreak) -> icode_goto(valOf_or_fail "unbound break" b, dic)

    | (Xannotate(_, s)) ->  x_compile_mc (e, b, c, r) dic s

    | (Xgoto(s, _)) -> icode_goto(s, dic)
    
    | (Xcomment _) -> ()
    
    | (Xcontinue) -> icode_goto(valOf_or_fail "unbound continue" c, dic)
    
    | (Xwaitrel dt) as arg -> 
        //let _ = constantp dt
        if constantp dt && xi_manifest_int "Xwaitrel c_compile_mc" dt < 2I then ()
        else muddy("x_compile_mc: " + hbevToStr arg)

    | (Xwaituntil g) -> x_compile_mc (e, b, c, r) dic (Xwhile(xi_not g, Xskip))
(*    let 
        let g' = bmonkey "wait statement predicate expression" e dic g
        let _ = icode_wait(g', dic) 
       in ()
   end 
*)

    | (Xlabel s) -> icode_emit(Ilabel(s, Icomment ""), dic) 

    | Xreturn v -> 
        let (rla, rexps) = r
        let (v', T) = x_compile_e e dic (v)
        let v'' = x_compile_loadto dic v'
        let _ = icode_emit(Ij(i_rslt, v'', NAM, NAM, ref 0), dic) 
        let _ = icode_goto(rla, dic) 
        let _ = xcomp_debug(fun()->(dic.id) + ": Resultis at " + i2s(!(dic.ptr)) + "\n")
        let _ = rexps := v'  :: (!rexps)
        in ()

    | Xwhile(g, s) -> 
        let continue_lab = funique("_LWC")
        let _ = icode_emit(Ilabel(continue_lab, Icomment ""), dic)
        let (g', T) = x_bcompile_e e dic (g)
        if bmon_manifest_false g' then ()
        elif bmon_manifest_true g' then
            let _ = x_compile_mc (e, b, c, r) dic s  (* infinte loop match *)
            let _ = icode_goto(continue_lab, dic)
            in ()
        else
            let break_lab = funique("_LWB")
            let g'' = x_compile_loadto dic g'
            let _ = icode_emit(Ij(i_brz, g'', ABSS break_lab, NAM, ref -1), dic)
            let b' = Some break_lab
            let c' = Some continue_lab
            let _ = x_compile_mc (e, b', c', r) dic s
            let _ = icode_goto(continue_lab, dic)
            let _ = icode_emit(Ilabel(break_lab, Icomment ""), dic)
            in ()

    | (Xblock l) -> app (x_compile_mc (e, b, c, r) dic) l
    
    | (Xskip) -> ()
    
    | Xif(g, t, f) -> sf("An 'if' should have become Xbeq by now")

(*
| x_compile_mc (e, b, c, r) dic (A as Xeasc(xs3(x_apply(("hpr_testandset", gis), [a1, a2])))) -> 
        let _ = x_compile_tas e dic (a1, a2) 
        in () end

| x_compile_mc (e, b, c, r) dic (A as Xeasc(x_apply(("cx_barrier", gis), _))) -> 
        let _ = vprint 3 ("Ignore compile of " + hbevToStr A + " to microcode\n")
        in () end
*)

    | (other) -> sf("x_compile_mc other:  " + (hbevToStr other) + ": Likely you need to do a VM compile first\n")


(*
 * This compiler entry point converts from abstract machine VM to microcode.
 *)
let vm_compile_mc kk dic v = 
    let r = (x_compile_mc kk dic v) //with Match -> sf("Match vm_compile_mc rethrow")
    in r


(*
 * Register colouring: convert from virtual to physical.
 *)
let mc_colour (dic:DIC_t) =
    let diclen = !(dic.ptr)
    let maxpr = 1000
    let first = Array.create maxpr diclen
    let last = Array.create maxpr -1
    let mapping = Array.create maxpr -1
    let free_regs = ref []
    let next_reg = ref 0

    let rec firstuse a arg =
        match arg with
            | VREG x -> let _ = Array.set first x (min first.[x] a) in arg
            | INDEXEDS(s, reg) -> firstuse a reg
            | (other) -> other
    let rec lastuse a arg =
            match arg with
                | VREG x -> let _ = Array.set last x (max last.[x] a) in arg
                | (INDEXEDS(s, reg)) -> lastuse a reg
                | (other) -> other

    let nextreg() =
            if !free_regs = [] then
                let r = !next_reg
                let _ = next_reg := !next_reg + 1
                in r
            else
                let r = hd (!free_regs)
                let _ = free_regs := tl(!free_regs)
                in r

    let freereg(x) = free_regs := singly_add x (!free_regs) 

    let rec doit a = function
            | VREG x ->
                (if first.[x]=a then Array.set mapping x  (nextreg());
                 if last.[x]=a then freereg mapping.[x];
                 PREG(mapping.[x])
                )
            | INDEXEDS(s, reg) -> INDEXEDS(s, doit a reg)
            | (other) -> other
    let scan1 k a old =
            match old with
                | Ij(ins, a0, a1, a2, e) ->
                    let nv = Ij(ins, k a a0, k a a1, k a a2, e)
                    let _ = vprintln 10 ("Coloured from " + mc_disassemble old + " to " + mc_disassemble nv)
                    in nv 
                | other -> other
            
    let rec scan m k pos =
            if pos=diclen then ()
            else 
                let nv = scan1 k pos dic.r.[pos]
                let _ = if m then Array.set dic.r pos nv
                in scan m k (pos+1) 
    let _ = scan false firstuse 0
    let _ = scan false lastuse 0
    let _ = scan true doit 0
    let _ = vprintln 3 ("Register colouring completed with " + (i2s(!next_reg)) + " registers")
    in ()



(*
 * mc_assemble handles assembler labels, calculating and inserting their dic values.
 * A separate routine generates binary machine code form and binptr values if needed.
 *)
let mc_assemble symbols (dic:DIC_t) =
    let diclen = !(dic.ptr)
    let next = ref(15 * 4096)
    let rec sl l = function
        | [] -> 
            if builtin_function l then -1 
            else 
                let r = !next
                let _ = next := r + 2
                let _ = mutadd symbols (l, (r, ref r))
                in r
        | (id,(dicptr, binptr))::t -> if (l=id) then dicptr else sl l t

    let rec fpf p = function
        | Ilabel(s, ins) -> (mutadd symbols (s, (p, ref -1)); fpf p ins)
        | _ -> ()

    let spf p = function
            | Ij(ins, INDEXEDS(a0, _), a1, a2, s) -> s:= sl a0 (!symbols)
            | Ij(ins, a0, INDEXEDS(a1, _), a2, s) -> s:= sl a1 (!symbols)
            | Ij(ins, a0, IMMEDS(a1, k), a2, s) -> 
                if !k=None then s:= sl a1 (!symbols)
                else s := valOf(!k) 
            | Ij(ins, ABSS a0, a1, a2, s) -> s:= sl a0 (!symbols)
            | Ij(ins, a0, ABSS a1, a2, s) -> s:= sl a1 (!symbols)
            | (_) -> ()
    let rec pass f pos = if pos<>diclen then (f pos dic.r.[pos]; pass f (pos+1))
    let _ = icode_emit(ii_stop, dic)
    let _ = pass fpf 0
    (*      let _ = compute_litpool symbols dic *)
    let _ = pass spf 0
    in map (fun (name,value) -> (name, value)) (!symbols)



(*
 * Assemble the code and append litpool (to the litpool field in dic).
 *
 *)
let x_compile_finish symbols dic =
    let _ = emit_litpool dic
    let _ = mc_assemble symbols dic
    let _ = mc_colour dic
    in ()   


let x_compile dic v = 
    let (e, b, c, r) = new_bc_labs() 
    let symbols = ref []
    let _ = icode_emit(Icomment "Start of x compile", dic)
    let _ = x_compile_mc(e, b, c, r) dic v
    let _ = x_compile_finish symbols dic
    let _ = icode_emit(Icomment "End of x compile", dic)
    in symbols


(*
 * Compile an H2 machine to assembly icode storing it in dic
 *)
let ucode_compile3 symbols dic = function
    | H2BLK(dir_, SP_l(ast_ctrl, v)) -> x_compile dic v

    | H2BLK(dir_, SP_par(_, v)) -> sf("Cannot compile parallel code construct to assembly")

    | H2BLK(dir_, SP_seq v) -> sf("Can only compile head of serial block to assembly")

    | H2BLK(dir, SP_dic(ar, ctrl)) ->
        let K = new_bc_labs() 
        let rec k pos = (if pos<ar.Length then (vm_compile_mc K dic ar.[pos]; k (pos+1)))
        let _ = if not_nullp dir.clocks then vprintln 0 ("+++Ignoring clock sensitivity in microcode output")
        let _ = icode_emit(Icomment ("Start of VM compile of " + ctrl.ast_ctrl.id), dic)
        let _ = icode_emit(Ij(i_lodi, VREG 999, IMMED (0*g_iobase()), NAM, ref 0), dic)
        let _ = k 0
        let _ = x_compile_finish symbols dic
        let _ = icode_emit(Icomment "End of VM compile", dic)
        let _ = vprintln 3 (sprintf "Compile of VM to microcode: %i " ar.Length + " input commands, " + (i2s (!(dic.ptr))) + " bytecodes")
        symbols

    | other -> sf("Other form exec block to microcode compile " + execSummaryToStr other)


(*
 * CBG squanderer:  A two-stack operator precedence parser.
 * Parsing pushes operators and operands alternately on the two stacks.
 * The operator stack must remain in increasing precedence and when an operator of
 * lower precedence needs to be pushed, the reduce operation is instead called. Reduce
 * pops an operator and two operands and pushes the resulting AST (or evaluation in
 * an interpreter) on the operand stack, thereby reducing each stack depth by one and
 * reducing the precedence on the top of the operand stack.  
 *)
type uc_t = 
    | UC_rname of string
    | UC_digit of string
    | UC_var of string
    | UC_diop of (char * uc_t) * (char * uc_t) * (char * uc_t)
    | UCd_equals
    | UCd_times | UCd_divide | UCd_mod
    | UCd_lbra | UCd_rbra
    | UCd_plus | UCd_minus
    | UCd_xor
    | UCd_neg
    | UCd_query | UCd_colon
    | UCd_semi
    | UCd_not
    | UCd_logor | UCd_bitor | UCd_logand | UCd_bitand
    | UCd_comma
    | UCd_lpar | UCd_rpar | UCd_subs
    | UCd_lshift
    | UCd_rshift
    | UCd_deqd | UCd_dned
    | UCd_dltd | UCd_dgtd
    | UCd_dled | UCd_dged
    | UC_filler


let UCd_convert = function
    | (a, UCd_plus)   -> (Some V_plus, None)
    | (a, UCd_minus)  -> (Some V_minus, None)
    | (a, UCd_equals) -> (Some V_interm, None)
    | (a, UCd_colon)  -> (None, None)
    | (a, UCd_semi)   -> (None, None)
    | (a, UCd_bitand) -> (Some V_bitand, None)
    | (a, UCd_bitor)  -> (Some V_bitor, None)
    | (a, UCd_lshift) -> (Some V_lshift, None)
    | (a, UCd_rshift) -> (Some (V_rshift Signed), None)
    | (a, UCd_times)  -> (Some (V_times), None)
    | (a, UCd_divide) -> (Some (V_divide), None)
    | (a, UCd_xor)    -> (Some V_xor, None)
    | (a, UCd_deqd)   -> (None, Some V_deqd)
    | (a, UCd_dltd)   -> (None, Some(V_dltd Signed))
    | (a, UCd_dled)   -> (None, Some(V_dled Signed))
    | (a, b) -> sf ("UCd_convert other: " + (implode [a]))

(*
 * Convert parser ast to hexp form.
 *)
let rec bs_bconvert = function
    | (_, UC_diop((_, UCd_not), l, r)) -> xi_not(bs_bconvert r) // l is ignored
    | (_, UC_diop((_, UCd_bitand (*really logand*)), l, r)) -> xi_and(bs_bconvert l, bs_bconvert r)
    | (_, UC_diop((_, UCd_bitor (*really logor*)), l, r)) -> ix_or (bs_bconvert l) (bs_bconvert r)
    | (_, UC_diop((_, UCd_xor (*really logxor*)), l, r)) -> ix_xor (bs_bconvert l) (bs_bconvert r)
    | (_, UC_diop(j, l, r)) ->
        let (_, oo) = UCd_convert j
        if oo=None then xi_orred(ix_pair (bs_convert l) (bs_convert r))
        else ix_bdiop (valOf  oo) [bs_convert l; bs_convert r] false

    | (_, UC_rname s) -> xgen_orred(gec_X_net s)
    | (_, UC_digit s) -> xgen_orred(xi_bnum(atoi s))
    | (_, UC_var s) -> xgen_orred(gec_X_net s)

    | (_) -> sf "bs_bconvert other"

and bs_convert = function
    | (_, UC_rname s) -> gec_X_net(s)
    | (_, UC_digit s) -> xi_bnum(atoi s)
    | (_, UC_var s) -> gec_X_net(s)
    | (_, UC_diop((_, UCd_not), l, r)) -> xgen_blift(xi_not(bs_bconvert r)) // l is ignored
    | (_, UC_diop((_, UCd_query), l, r)) -> 
        let r' = bs_convert r
        let m = function
            | X_pair(t, f, _) -> ix_query (bs_bconvert l) t f
            | (other) -> sf("malformed query")
        m r' 

    | (_, UC_diop((_, UCd_comma), l, r)) -> ix_pair(bs_convert l) (bs_convert r)
    | (_, UC_diop((_, UCd_subs), l, r))  -> ix_asubsc (bs_convert l) (bs_convert r)
    | (_, UC_diop((_, UCd_neg), l, rr))   ->
        let v = bs_convert rr // monadic in r?
        xi_neg (mine_prec g_bounda v) v 
    | (_, UC_diop(j, l, r)) ->
        let (oo, _) = UCd_convert j
        let prec = g_default_prec // for now!
        if not_nonep oo then ix_diop prec (valOf oo) (bs_convert l) (bs_convert r)
        else ix_pair (bs_convert l) (bs_convert r)
    |(_) -> sf "bs_convert other"

let bs_report(v) = (print("-->" + xToStr(bs_convert v)); print "\n"; v)

let rec ucomp_lex = function
    | [] -> []
    | h::t -> 
        if h = ' ' then ucomp_lex t
        elif 'A' <= h && h <= 'Z' then (h, UC_rname(implode[h]))::(ucomp_lex t)
        elif h = 's' && t<>[] && hd t = 'p' then (h, UC_var "sp")::(ucomp_lex(tl t))
        elif h = 'p' && t<>[] && hd t = 'c' then (h, UC_var "pc")::(ucomp_lex(tl t))
        elif 'a' <= h && h <= 'z' then (h, UC_var(implode[h]))::(ucomp_lex t)
        elif '0' <= h && h <= '9' then 
            let rec kk = function
                | (h::t, r) when (h >='a' && h <= 'f') || (h >='A' && h <= 'F') || (h = 'x') || '0' <= h && h <= '9' -> kk(t, h::r) // Allow 0x0 style hex digits
                | (h::t, r) -> (h::t, r)
                | ([], r) -> ([], r)
            let (t0, r) = kk (h::t, [])
            in (h, UC_digit(implode(rev r)))::(ucomp_lex t0)
        elif h = '=' && (hd t) = '=' then (h, UCd_deqd)::(ucomp_lex(tl t))
        elif h = '=' then (h, UCd_equals)::(ucomp_lex t)
        elif h = '&' && (hd t) = '&' then (h, UCd_logand)::(ucomp_lex(tl t))
        elif h = '&' then (h, UCd_bitand)::(ucomp_lex t)
        elif h = '|' && (hd t) = '|' then (h, UCd_logor)::(ucomp_lex(tl t))
        elif h = '|' then (h, UCd_bitor)::(ucomp_lex t)
        elif h = '[' then (h, UCd_lbra)::(ucomp_lex t)
        elif h = ']' then (h, UCd_rbra)::(ucomp_lex t)
        elif h = '(' then (h, UCd_lpar)::(ucomp_lex t)
        elif h = ')' then (h, UCd_rpar)::(ucomp_lex t)
        elif h = '/' then (h, UCd_divide)::(ucomp_lex t)
        elif h = '*' then (h, UCd_times)::(ucomp_lex t)
        elif h = '%' then (h, UCd_mod)::(ucomp_lex t)
        elif h = '+' then (h, UCd_plus)::(ucomp_lex t)
        elif h = '-' then (h, UCd_minus)::(ucomp_lex t)
        elif h = '^' then (h, UCd_xor)::(ucomp_lex t)
        elif h = '!' && (hd t) = '=' then (h, UCd_dned)::(ucomp_lex(tl t))
        elif h = '!' then (h, UCd_not)::(ucomp_lex t)
        elif h = ';' then (h, UCd_semi)::(ucomp_lex t)
        elif h = '-' then (h, UCd_neg)::(ucomp_lex t)
        elif h = '?' then (h, UCd_query)::(ucomp_lex t)
        elif h = ':' then (h, UCd_colon)::(ucomp_lex t)
        elif h = ',' then (h, UCd_comma)::(ucomp_lex t)
        elif h = '<' && (hd t) = '<' then (h, UCd_lshift)::(ucomp_lex(tl t))
        elif h = '<' && (hd t) = '=' then (h, UCd_dled)::(ucomp_lex(tl t))
        elif h = '<' then (h, UCd_dltd)::(ucomp_lex t)
        elif h = '>' && (hd t) = '>' then (h, UCd_rshift)::(ucomp_lex(tl t))
        elif h = '>' && (hd t) = '=' then (h, UCd_dged)::(ucomp_lex(tl t))
        elif h = '>' then (h, UCd_dgtd)::(ucomp_lex t)
        else sf(("Bad ucomp_lex character: '" + (implode[h])) + "'")


let precedence_order = [
        UCd_subs;
        UCd_not; UCd_neg;
        UCd_times; UCd_divide; UCd_mod;
        UCd_plus; UCd_minus;
        UCd_lshift; UCd_rshift;
        UCd_xor;
        UCd_bitor;
        UCd_bitand;
        UCd_dltd; UCd_dgtd; UCd_dled; UCd_dged; UCd_deqd; UCd_dned;
        UCd_logor;
        UCd_logand;
        UCd_colon; UCd_query;
        UCd_equals; UCd_comma; UCd_semi
]
;

let bs_ast2 (l, r) h = ('.', UC_diop(h, l, r))


let monadic_filler = ('_', UC_filler)

let bs_reduce_item = function
    | (r::l::st1, h::st2) -> ((bs_ast2 (l, r) h)::st1, st2)
    | (r::st1, _) -> sf("bs_reduce_item: missing arg")
    | _ -> sf("bs_reduce_item: no args")
;

let rec bs_reduce m = function
    | (_,      (items, [])) -> (items, [])
    | (None,   (st1, st2))  -> bs_reduce m (None, bs_reduce_item(st1, st2))
    | (Some k, (st1, (v, j)::st2)) ->
        if k=j then (st1, st2)
        else bs_reduce m (Some k, bs_reduce_item(st1, (v, j)::st2))
    //| (Some v,  _) -> sf(m + ": bs_reduce: syntax error")


let rec bs_push v (st1, st2) t =
    match st2 with
        | [] -> bs_parse1 (st1, [v]) t 
        | h::st2 -> 
            let rec hs = function
                | [] -> sf("operator not in precedence order")
                | b::bs -> if b=(snd v) || b=(snd h) then b else hs bs
            let higher = hs precedence_order
            if higher<>(snd h) then bs_parse1(st1, v::h::st2) t
            else bs_push v (bs_reduce_item(st1, h::st2)) t

and bs_parse1 (st1,st2) (h::t) =
    match h with
        | (k, UC_rname a) ->  bs_parse2 (h::st1,st2) t
        | (k, UC_var a)   ->  bs_parse2 (h::st1,st2) t
        | (k, UC_digit a) ->  bs_parse2 (h::st1,st2) t
        | (k, UCd_minus)  ->  bs_parse1 (('0', UC_digit "0")::st1,h::st2) t
        | (k, UCd_lpar)   ->  bs_parse1 (st1,h::st2) t
        | (k, UCd_lbra)   ->  bs_parse1 (('M', UC_var "mem")::st1,h::('[', UCd_subs)::st2) t
        | (k, UCd_not)    ->  bs_parse1 (monadic_filler::st1,h::st2) t
        | (k, UCd_neg)    ->  bs_parse1 (monadic_filler::st1,h::st2) t
        | (k, other) -> sf("ucomp parse 1 error: " + (implode [k]))

and bs_parse2 (st1,st2) = function
        | [] -> bs_reduce "msg" (None, (st1, st2))
        | h::t ->
            match h with
                | (k, UCd_query)  -> bs_push h (st1,st2) t
                | (k, UCd_colon)  -> bs_push h (st1,st2) t
                | (k, UCd_equals) -> bs_push h (st1,st2) t
                | (k, UCd_plus)   -> bs_push h (st1,st2) t 
                | (k, UCd_comma)  -> bs_push h (st1,st2) t 
                | (k, UCd_xor)    -> bs_push h (st1,st2) t 
                | (k, UCd_deqd)   -> bs_push h (st1,st2) t 
                | (k, UCd_dltd)   -> bs_push h (st1,st2) t 
                | (k, UCd_dled)   -> bs_push h (st1,st2) t 
                | (k, UCd_bitand) -> bs_push h (st1,st2) t 
                | (k, UCd_bitor)  -> bs_push h (st1,st2) t 
                | (k, UCd_minus)  -> bs_push h (st1,st2) t
                | (k, UCd_times)  -> bs_push h (st1,st2) t
                | (k, UCd_divide) -> bs_push h (st1,st2) t
                | (k, UCd_lshift) -> bs_push h (st1,st2) t
                | (k, UCd_semi)   -> bs_push h (st1,st2) t
                | (k, UCd_rshift) -> bs_push h (st1,st2) t
                | (k, UCd_rbra)   -> bs_parse2 (bs_reduce "msg" (Some UCd_lbra, (st1,st2))) t
                | (k, UCd_rpar)   -> bs_parse2 (bs_reduce "msg"(Some UCd_lpar, (st1,st2))) t
                | (k, other)      -> sf("ucomp parse 2 error: " + (implode [k]))


(*
 * Parse the processor description and then convert it to hexp_t.
 *)
let ucomp_p s =
    if s = "" then xi_num 0
    else
        let vd = 0
        let toks = (ucomp_lex(explode s))
        if vd>=4 then vprintln 4 ("ucomp compile " + s + "  " + sfold (sprintf "%A") toks)
        let (items, ops) = bs_parse1([], []) toks
        if ops<>[] then sf("bs_parse: dangle ops")
        if length items <>1 then sf("bs_parse: dangle args")
        let rr = bs_convert(hd items)
        if vd>=4 then vprintln 4 ("ucomp parse of " + s + " gave " + xToStr rr)
        rr



(*
 * Parse instruction set behavioural form (p field) and parse e field too.
 * 
 *)
let myset _  = map (fun x ->(x, (ucomp_p x.p, ucomp_p x.e))) uisa_set.instructions



let register16(id, length) =
    let width = 16
    let signed = Unsigned
    let io = LOCAL
    let (n, ov) = netsetup_start id
    match ov with
        | Some ff ->
            let f2 = lookup_net2 ff.n
            assertf(length = f2.length) (fun () -> id + ": netgen16 new len " + sfold i2s64 length + " cf " + sfold i2s64 f2.length)
            cassert(signed = ff.signed, "netgen16 new signed")
            assertf(width = ff.width) (fun () -> id + ": netgen16 new width " + i2s ff.width)
            cassert(io = f2.xnet_io, "netgen16 new io")
            ff
        | None ->

            let pol = ref false
            let dir = ref false
            let h = ref 65535I
            let l = ref 0I
            let vt = ref V_REGISTER
            let e2 = 0
            let ff = 
                 {
                    n=          n
                    rh=         !h
                    rl=         !l
                    id=         id
                    width=      width
                    constval=   []
                    signed=     signed
                    is_array=   not_nullp length
                }
            let f2 =
                {
                    length=     length
                    ats=        []
                    dir=        !dir
                    pol=        !pol
                    xnet_io=    io 
                    vtype=      !vt
                }
            netsetup_log (ff, f2)

(* myregs would be better placed inside uisa_set in future - when its String.length gets fixed! *)
let myregs = (1000L, X_bnet(register16("vReg", [1000L])))



(*
 * Binary assemble, the n field in the instruction is no use and values must be 
 * looked up in the binptr field of syms
 *)
let rec mc_bin_assemble no syms  (pos, dis) = function
    | (Ij(ins, a0, a1, a2, n)) ->
      let v0 = op_assoc ins (myset())
      if v0=None then sf("undefined instruction:" + dis)
      vprint 0 ( "Instruction " + dis + "\n")
      let  (_, v) = valOf v0 
      let fields = map (fun x -> implode [x]) (explode(ins.am))


      let lookup a = 
          let rec k = function
              | [] -> (ignore(if no=2 then verror("Mising symbol " + a + "\n")); 0)
              | (id, (dic, bin))::t -> if id=a then !bin else k t
          k syms

      let rec encode f = function
          | (PREG k)  -> k 
          | (VREG k)  -> k 
          | (IMMED k) -> k
          | (ABS k)   -> k
          | (ABSS k)  -> lookup k
          | (IMMEDS(k, v)) -> if !v <> None then valOf (!v) else lookup k
          | (INDEXEDS(offset, basereg)) -> if f then encode f basereg else lookup offset
          | (other) -> sf(dis + ":" + amodeToS other + " cannot appear in machine code")

      let garg1 k =
          if (length fields > 0 && k=hd fields) then encode true a0
          elif (length fields > 1 && k=hd(tl fields)) then encode true a1
          elif (length fields > 2 && k=hd(tl(tl fields))) then (if k="b" then encode false a1 else encode true a2)
          else sf(dis +  " unset field " + k)


      let garg k = 
          let r = garg1 k
          let r' = if (k="L") (* Pc relative *) then (r-pos) / 2 else r
          in r'

      let width = uisa_set.ins_width

      let signed = function
          | "L" -> true
          | _   -> false

      let basewidth a = function
          | (W_asubsc(_, X_pair(X_num hi, X_num lo, _) ,_)) -> (lo, hi-lo+1, signed a)
          | (W_asubsc(_, X_num n, _)) -> (n, 1, false)
          | (W_asubsc(_, k, _)) -> sf("bk bad subsc " + (xToStr k ))
          | (other) -> sf("opcode basewidth"  + (xToStr other))

      let rec dofields = function
          | X_pair(a, b, _) -> (dofields a) @ (dofields b)
          | W_node(prec, V_interm, [l; X_num n], _) -> [(basewidth "" l, n)]
          | W_node(prec, V_interm, [l; r], _) -> [(basewidth (xToStr r) l, garg(xToStr r))]
          | other -> sf("dofields other " + (xToStr other))

      let cerror ss = if no=2 then verror ss

      let mask = function
          | (w, n, false) ->
              let _ = if n < 0 || n >= two_to_the32 w then cerror(dis + sprintf ": Out of range: %A\n" n)
              n
          | (w, n, true)  ->
              let _ = if n < -(two_to_the32(w-1)) || n >= two_to_the32(w-1)-1 then cerror(dis + sprintf ": Out of range: %A\n" n)
              two_to_the32(w) - n
                       
      let rec tobin = function
          | [] -> 0
          | ((b, w, false), vale)::tt -> 
              let r = mask(w, vale, false) * two_to_the32(b) + tobin tt
              let _ = if vale >= two_to_the32(w) then cerror(dis + sprintf ": operand too large for bit field: %A " vale + " in " + (i2s w) + " bits.\n")
              r

          | ((b, w, true), vale)::tt -> 
              let r = mask(w, vale, true) * two_to_the32 b + tobin tt
              let _ = if vale >= two_to_the32(w-1) || vale < -(two_to_the32(w-1)) then cerror(dis + sprintf ": signed operand outside range of bit field: %A " vale + " in " + (i2s w) + " bits.\n")
              r
      let instruction = tobin(dofields v)

      (* let _ = print("here " + xToStr (v) + ":" + (i2x instruction) + "\n") *)
      Some(instruction, width)

    | Icompiled(ins, code) -> (mc_bin_assemble no syms  (pos, dis) ins)
    | Icomment ss -> None
    | Ilabel(ss, ins) -> 
        let k = op_assoc ss syms 
        if k=None then sf ("mc_bin_assemble: missing symbol " + ss)
        if no=1 then (snd(valOf k)) := pos 
        elif !(snd(valOf k)) <> pos then sf ("mc_bin_assemble: slipped symbol " + ss) 
        mc_bin_assemble no syms (pos, dis) ins
    | Idefs(v, s) -> Some(v, 16)
    | (v) -> sf "mc_bin_assemble: Other instruction???"



(*
 * Generate Intel Hex16 Format
 *)
let ihex_output f data =
    let l = ref []
    let address = ref 0
    let sum = ref 0
    let s str  = vprint 0 (str)
    let hex1 v = s(i2x v)
    let hex2 v = (hex1(v / 16); hex1(v % 16); mutinc sum v)
    let hex4 v = (hex2(v / 256); hex2(v % 256))
    let emit_csum() = (hex2( 255-(!sum % 256)); s "\n")
    let emit() = 
        let q = length !l   
        let _ = (s ":"; hex2 q; hex4(!address); hex2 0)
        app (fun j -> (hex2(j); mutinc address 1; ())) (!l)
        (emit_csum(); sum := 0; l := [])

    let rec k = function
        | [] -> ()
        | h::tt -> (mutadd l h; if length(!l)>=16 then emit(); k tt)

    let emit_end_of_list() = (s":"; hex2 0; hex4 0; hex2 1; emit_csum())
    k data
    if length(!l)>0 then emit()
    emit_end_of_list()


(*
 * Does first and second pass assembly.
 *)
let icode_asm_dump f syms (dic:DIC_t)  =
    let pass no f = 
        let addr = ref dic.origin
        let _ = f ("Origin " + i2x(!addr) + "\n")
        let rec kk pos = 
            if pos >= !(dic.ptr) then ()
            else 
                let k = dic.r.[pos]
                let dis = (mc_disassemble k)
                let addr0 = !addr
                let code = mc_bin_assemble no syms (addr0, dis) k
                let _ = if code<>None then
                                 (
                                     mutadd dic.object_code (valOf code);
                                     mutinc addr (snd(valOf code) / 8);
                                     ()
                                 )
                let code1 = if (code=None) then "____" else i2x(addr0) + " " + i2x(fst(valOf code))
                let _ = f((i2s pos) + ": " + code1 + "  " + dis + "\n")
                in kk (pos+1)
            in kk 0

    let std() =
            (
              f ("\n\nSymbol Table\n");
              app (fun (id, (dicptr, binptr))-> f(id + "\t\t" + (i2s dicptr) + " bin=" + (i20x(!binptr)) + "\n")) (syms)
             )

    let _ = pass 1 (fun x -> ())
    let _ = std()
    let _ = f ("\n\nAssembler listing\n")
    let _ = pass 2 f
    in ()
;       



let NOTUSED_icode_register_allocate (dic:DIC_t) =
    let endp = !(dic.ptr)
    let u = Array.create (endp+1) []
    let editing = ref false

    let record p n = Array.set u p (n :: u.[p])
    let play p n = PREG 22

    let rec mc1 p = function
        | (VREG n) as a -> if !editing then play p n else (record p n; a)
        | (INDEXEDS(k, l)) as a -> INDEXEDS(k, mc1 p l)
        | (other) -> other
                     
    let rec mc p = function
            | (Ij(ins, a0, a1, a2, n)) -> Ij(ins, mc1 p a0, mc1 p a1, mc1 p a2, n)
            | Icompiled(ins, code) -> Icompiled(mc p ins, code)
            | (Icomment s) as a -> a
            | (Idefs(v, s)) as a -> a
            | (v) -> v

    let rec scan0 pos =
            if pos >= endp then ()
            else 
                let _ = mc pos dic.r.[pos]
                in scan0 (pos+1)
    let _ = scan0 0
    in ()

(*
 * Compile ucode into lifted simulator form. ie generate some xcode for each instruction.
 *
 * We have generated nice symbolic assembly language, but here
 * the assembly machine code output is disregarded and we make alternate conversion to xcode.
 *)
let rec lifted_compile (set, regs, symbs) = function
    | (Ilabel(s, ins)) -> lifted_compile (set, regs, symbs) ins
    | (Icomment s) as arg ->  arg
    | (Idefs _) as arg ->  arg
    | (Ij(ins, a0, a1, a2, s)) as arg ->  
        let v0 = op_assoc ins set
        if v0=None then sf("instruction not in set")
        let (v, _) = valOf v0
        let am = explode(ins.am)
        let using id = 
                let j = op_assoc id symbs
                let s = if j=None then (verror(id + ": undefined label in lifted assembly"); 0) else valOf j
                in X_num s
        let reg v = ix_asubsc regs (X_num v)
        let rec j = function
            | ([], _) -> []
            | (r::rt, (VREG v)::vt) -> (gec_X_net(implode[r]), reg v)::j(rt, vt)
            | (r::rt, (PREG v)::vt) -> (gec_X_net(implode[r]), reg v)::j(rt, vt)
            | (r::rt, (INDEXEDS(offset, PREG basereg))::vt) -> (gec_X_net "b", using offset)::(gec_X_net(implode[r]), reg basereg)::j(rt, vt)
            | (r::rt, (IMMEDS(k, item))::vt) -> 
                let k0 = op_assoc k symbs
(* let _ = print("looked up " + k + " and found " +(if k0=None then "None" else "Some")+"\n")*)
                let k1 = if k0<>None then xi_num(valOf k0) elif !item=None then gec_X_net k else xi_num(valOf(!item))
                (gec_X_net(implode[r]), k1)::j(rt, vt)

            | (r::rt, (IMMED(k))::vt) -> (gec_X_net(implode[r]), xi_num k)::j(rt, vt)
            | (r::rt, (ABSS k)::vt) -> (gec_X_net(implode[r]), gec_X_net k)::j(rt, vt)
            | (r::rt, NAM::vt) -> j(rt, vt)
            | (r::rt, a::vt) -> sf((amodeToS a) + ": cannot map addressing mode")
        let e = j(am, [a0; a1; a2])
        let st = []
        let v' = xi_assoc_exp g_sk_null (makemap e) v
        Icompiled(arg, v')
    | other -> sf ((mc_disassemble other) + ": lifted_compile: unsupported ucomp opcode")




let tnow_as_a_net = X_bnet(register16("tnow16", []))  (*  *)

type OPCODEMAP_t = 
    | MAP_gully
    | MAP of inst_t


let opcode_map = Array.create (two_to_the32 uisa_set.ins_width) MAP_gully

type x = Notset | Set of hexp_t * int

let opcode_validate A (ins:inst_t, (v, y)) =
    let _ = lprint 30 (fun()->"Opcode validate  " + ins.nm + " " + xToStr y + "\n")
    let width = uisa_set.ins_width
    let bits = Array.create width Notset

    let setbit1 r pos lane  = 
        let ov = bits.[lane]
        let _ = if (ov<>Notset) then sf("bit set mto by " + ins.nm)
        in Array.set bits lane (Set(r, pos))

    let rec setbit r pos = function
        | [] -> ()
        | h::t -> (setbit1 r pos h; setbit r (pos+1) t)
    let range = function
        | (W_asubsc(_, X_pair(X_num hi, X_num lo, _), _)) -> [lo .. hi]
        | (W_asubsc(_, X_num n, _)) -> [n]
        | (W_asubsc(_, k, _)) -> sf("bk bad subsc " + (xToStr k ))
        | (other) -> sf("opcode range: "  + (xToStr other))

    let rec analyze = function
        | X_pair(a, b, _) -> (analyze a; analyze b)
        | (W_node(prec, V_interm, [l; r], _)) -> setbit r 0 (range l)
        | other -> sf("analyze other " + (xToStr other))
    let _ = analyze y
    let demark pos0 = function
        | Notset -> []
        | Set(X_net(n, _), pos) -> [ (pos0, -1) ]
        | Set(X_num n, pos) -> [ (pos0, (n / int(two_to_the pos)) % 2) ]
        | (_) -> sf "demark"
    let rec gmarking pos = if (pos >= width) then []
                           else let d = bits.[pos] in (demark pos d) @ (gmarking (pos+1))

    let rec xproduct = function
        | [] -> [[]]
        | ((pos, -1)::t) ->
            let r = xproduct t
            let k0 s = (pos, 0) :: s
            let k1 s = (pos, 1) :: s
            in (map k0 r) @ (map k1 r)
        | ((pos, v)::t) -> map (fun x -> (pos, v)::x) (xproduct t)
    let markings = xproduct (gmarking 0)
    vprintln 10 (i2s(length markings) + " markings for " + ins.nm)
    let rec idx = function
        | [] -> 0
        | (pos, k)::t -> 
            ( (* print("idx " + (i2s pos) + " " + (i2s k) + ","); *)
            (if k>0 then two_to_the32 pos else 0) + idx t
            )
    let vof (MAP k) = k
    let insert k = 
        let ov = opcode_map.[k]
        let _ = if (ov <> MAP_gully) then sf("opcode encoding clash on " + ins.nm + " and  " + (vof ov).nm + " at " + i2x k)
        Array.set opcode_map k (MAP ins)

    app insert (map idx markings)
    ()


let memmap = (ref [], ref(int64(g_iobase() + 400)))

let memmap_lookup = function
    | (X_bnet ff) as k ->
        let f2 = lookup_net2 ff.n
        let id = ff.id
        let len = asize f2.length
        let len = if len=0L then 1L else len
        let len' = ((len+1L) / 2L)*2L
        let ov = op_assoc id !(fst memmap)
        if ov<> None then (valOf ov, id)
        else
            let j = !(snd memmap)
            (snd memmap) := j + len'
            mutadd (fst memmap) (id, j)
            (j, id)

    | other -> sf((xToStr other) + ": symbol lookup")



let ucode_compile2 decls (no, x) =
    let dic = new_icode(funique "monolithic")
    let define_symb (v) = 
        let scalar = xToStr v
        let (addr, id) = memmap_lookup v
        let _ =  icode_emit(Icomment(sprintf "iospace: %020X" addr + "=" + (i20x (g_iobase())) + sprintf "%020X" (addr - int64(g_iobase())) + " EQU " + id + " ; " + scalar), dic)
        (id, (int32 addr, ref -1))

    let decls = List.foldBack (db_flatten []) decls []

    let iospace = map define_symb (tnow_as_a_net::(map snd decls))

    let symbols = ref iospace
    let _ = ucode_compile3 symbols dic x
    let abis (no, id, key) = mutadd symbols (id, ((g_jbase()+key.biosnumber), ref -1))
    let _ = app abis hpr_native_table
    let filename = "microcode_" + i2s(no)
    let _ = vprintln 3 ("Write microcode assembly to " + filename)
    let cos = yout_open_out filename
    let f = fun ss -> (gprint 5 ss; yout cos ss)
    let _ = icode_asm_dump f (!symbols) dic (* Display assembly code to file *)
    let ll = !dic.ptr
    let remove_bin (id, (dicptr, binptr)) = (id, dicptr)
    let symbols1 = map remove_bin !symbols 
    let sss = myset()
    app (opcode_validate opcode_map) (List.zip (uisa_set.instructions) (map snd sss))
    let rec lci pos = if pos<=ll then (Array.set dic.r  pos (lifted_compile (sss, snd myregs, symbols1) (dic.r.[pos])); lci (pos+1))
    let _ = lci 0
    let _ = icode_asm_dump f (!symbols) dic
    let unpair((d, _), c) = (d / 256) :: (d % 256) :: c
    let ocode = rev (!(dic.object_code))
    let _ = ihex_output cos (foldr unpair [] ocode)
    yout_close cos
    let remove_bin(id, (dicptr, binptr)) = (id, dicptr)
    SP_asm(dic.r, ll, myregs, map (fun (a,b)->(a,int64 b)) symbols1, ocode)





(*
 * Generate instruction decoder.
 *)
let opcode_decode opcode jumping regs e (pc, sp) ((ins:inst_t, (v, y)), gully) =
    let vd = 0
    if vd>=4 then vprintln 4 ("Opcode decode  " + ins.nm + " " + xToStr y)
    let width = uisa_set.ins_width 
    let tests = ref xi_true
    let maketest((b, w), n) = 
        let pp = ix_bitand (ix_lshift (gec_X_bnum({widtho=Some w; signed=Unsigned}, himask w)) (xi_num b)) opcode
        let t = xi_deqd(pp, ix_lshift (xi_num n) (xi_num b))
        //let _ = print((#nm ins) + ": maketest b=" +(i2s b)+ " w=" + (i2s w) + " n=" + (i2s n) + " res=" + (xToStr rb) + "\n")
        tests := xi_and(!tests, t)

    let fields = ref []
    let field((b, w), name) = mutadd fields (name, (b, w))

    let basewidth = function
        | W_asubsc(_, X_pair(X_num hi, X_num lo, _), _) -> if (hi < lo) then sf ("hi<lo") else (lo, hi-lo+1)
        | (W_asubsc(_, X_num n, _)) -> (n, 1)
        | (W_asubsc(_, k, _)) -> sf("bk bad subsc " + (xToStr k ))
        | (other) -> sf("opcode basewidth"  + (xToStr other))

    let rec dofields = function
        | X_pair(a, b, _) -> (dofields a; dofields b)
        | W_node(prec, V_interm, [l; X_num n], _) -> maketest(basewidth l, n)
        | W_node(prec, V_interm, [l; r], _) -> field (basewidth l, r)
        | other -> sf("dofields other " + xToStr other)
    let _ = dofields y

    let reg v = ix_asubsc regs v
    let maskit(x, w) = ix_bitand x (gec_X_bnum({widtho=Some w; signed=Unsigned}, himask w))
    let sex(w, x) = x (* for now *)
                    

    let gep (name, (b, w)) = if vd>=4 then vprintln 4 (xToStr name + ": b="  + i2s b + " w=" + i2s w)
    let gep1 (name, x) = if vd>=4 then vprintln 4 (xToStr name + ": to " + xToStr x)
    let ge1 = function
        | ("D", b, w) -> reg(maskit(ix_rshift Unsigned opcode (xi_num b), w))
        | ("A", b, w) -> reg(maskit(ix_rshift Unsigned opcode (xi_num b), w))
        | ("B", b, w) -> reg(maskit(ix_rshift Unsigned opcode (xi_num b), w))
        | ("R", b, w) -> reg(maskit(ix_rshift Unsigned opcode (xi_num b), w))
        | ("X", b, w) -> reg(maskit(ix_rshift Unsigned opcode (xi_num b), w))
        | ("L", b, w) -> ix_plus pc (ix_lshift (sex(w, maskit(ix_rshift Unsigned opcode (xi_num b), w))) (xi_num 1))        

        | ("b", b, w) -> maskit(ix_rshift Unsigned opcode (xi_num b), w)
        | ("i", b, w) -> maskit(ix_rshift Unsigned opcode (xi_num b), w)
        | (other, b, w) -> sf("bad ge addressing mode " + other)

    let ge (n, (b, w)) = (n, ge1(xToStr n, b, w))
    app gep (!fields)
    let zreg = (gec_X_net "Z", reg(xi_num 0))
    let e = (map ge (!fields)) @ (zreg :: e)
    app gep1 e
    let body = Xeasc(xi_assoc_exp g_sk_null (makemap e) v)
    if vd>=4 then vprintln 4 (ins.nm + ": decoder " + xbToStr (!tests) + " for " + xToStr v + " gives " + hbevToStr body + "\n")
    let guard = !tests
    if op_assoc (gec_X_net "L") e <> None then jumping := xi_or(guard, !jumping)
    gen_Xcomment1 ins.nm [gec_Xif guard body gully]




let output_microcontroller ww p (idl, (HPR_VM2(minfo, decls, sons, execs, assertions)) as m) =
    let id = vlnvToStr minfo.name
    let filename = id + "_" + p + ".v"
    let fd = yout_open_out filename
    
(*
   let bnetgen id = ionet_w(id, 1, OUTPUT) (* perverse: declare as o/p since gets flipped *)
let rst = bnetgen "resetnasty" 
*)
    let ww' = WN "microcontroller output" ww
    let _ = yout fd ( "\n\n// H2 Microcontroller Synthesised Verilog\n\n")
    let gates = muddy " List.fold (xrtl_to_rtl ww') (map (h2_as_rtl ww') sons) execs"
    let pcs = []
    let ios = [  ] @ decls
    let vd = 0
    let ddctrl = g_null_ddctrl
    let _ = rtl_output_auxold ww' ddctrl vd fd (id, pcs, ios, gates)
    let _ = yout_close fd
    let _ = unwhere ww
    ()





let rec output_romcode id pos = function
    | [] -> () 
    | code::t -> 
        let filename = fn_sanitize id + "_romcode" + (i2s pos) + ".v"
        vprint 3 ("Writing ROM code to " + filename + "\n")
        let fd = yout_open_out filename
        yout fd ( "\n\n// H2 machine code ROM initialisation Verilog\n\n")
        yout fd ( " initial begin\n")
        let z (l, r) = yout fd ( "   mem[16'h" + (i2x l) + "] = 16'h" + (i2x r) + ";\n")
        app z code
        yout fd (" end\n\n")
        yout_close fd
        vprintln 3 ("Finished writing ROM code to " + filename)
        output_romcode id (pos+1) t


(* 
 * This function takes the microcontroller ISA description and generates an H2machine
 * that is a microcontroller for that architecture.
 *)
let gen_microcontroller(isa:hpr_isa_t) =
    //let _ = if (uisa_set = isa) then ()
    let vlnv = { vendor="HPRLS"; library="OPATH1"; kind=[isa.name]; version="1.0"}
    let minfo = { g_null_minfo with name=vlnv }
    let mem    = arraynet_w("mem", [int64(two_to_the isa.data_width)], 16)
    let opcode = X_bnet(register16("OCODE", [1L]))
    let regs   = X_bnet(register16("regfile", [int64 isa.ngpregs]))
    let pc     = X_bnet(register16("pc", [1L]))
    let sp     = X_bnet(register16("sp", [1L]))
    let c      = simplenet("c") (* Carry bit *)
    let bnetgen id = ionet_w(id, 1, OUTPUT, Unsigned, []) (* perverse: declare as o/p since gets flipped *)
    let clk_net = g_clknet
    let reset_net = g_resetnet
    
    let jumping = ref xi_false
    let e = [ (gec_X_net "mem", mem) ]
    let dec = foldr (opcode_decode opcode jumping regs e (pc, sp)) Xskip (myset()) // do not reparse myset!

    let jumpnet = simplenet "jumping"
    let jump = Xassign(jumpnet, X_blift(!jumping))
    let spclear = gec_Xif (xi_orred reset_net) (Xassign(sp, xi_num(isa.tos))) Xskip
    let advance = gec_Xif (xi_or(xi_orred reset_net, xi_not(xi_orred jumpnet))) (Xassign(pc, xi_query(xi_orred reset_net, xi_zero, ix_plus (xi_num 2) pc))) Xskip
    let ans = gec_Xblock [advance; dec; spclear ]
    let decls =
        let cpi = { g_null_db_metainfo with kind= "microcode-control-nets" }
        [DB_group(cpi, map db_netwrap_null [ clk_net; reset_net ])]

    let locals =
        let cpi = { g_null_db_metainfo with kind= "microcode-locals" }
        [DB_group(cpi, map db_netwrap_null [ opcode; regs; c; pc; sp; mem; jumpnet ])]
    
    let fetch = Xassign(opcode, ix_asubsc mem (ix_rshift Unsigned pc (xi_num 1)))
    let ast_ctrl = { g_null_ast_ctrl with id=isa.name }
    let seq  = H2BLK({g_null_directorate with clocks=[E_pos clk_net]; duid=next_duid()}, SP_l(ast_ctrl, ans))
    let comb = H2BLK(g_null_directorate, SP_l(ast_ctrl, gec_Xblock[fetch; jump]))
    HPR_VM2(minfo, decls@locals, [], [seq; comb], [])

(* Just cat all the binaries here, for now, but only the first will run *)
let romgen id (l, cc) = 
    let nos = map (fun (a,b)->a) l
    let a = ref 0
        (* let d = eqnToLogic "" (arraynet_w("mem", 11, 11))  *)

    let setup (l, r) = (l, r); (* V_NBA(V_SUBSC(d, V_NUM l), V_NUM r) *)
    let r = map (fun x ->setup((let r = !a in let _ = a:=(!a)+1 in r), x)) nos
    let _ = vprintln 0 (id + ": " + i2s(!a) + " ROM locations initialised")
    if r = [] then cc else r::cc


(*
 * Compile to microcode.
 * Also spit out a vnl microcontroller as side effect.
 *)
let rec opath_ucode_compile ww (msg, control, c2) = function
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls, children, execs, assertions)))  ->
        let phasename = "ucode-compile"
        let newname = funique("microcode") :: minfo.name.kind
        let vlnv = { vendor="HPRLS"; library="custom"; kind=newname; version="1.0" }
        let minfo' = { g_null_minfo with name=vlnv } 
        let mname = vlnvToStr minfo'.name
        let ww' = WN (phasename + " " + mname) ww
        let _ = if execs=[] then hpr_warn ("Warning: no exec code for ucode_compile of " + mname)
        let ans = if (execs <> []) then map (ucode_compile2 decls) (List.zip [1 .. length execs] execs)
                  else []
        let children' = map (opath_ucode_compile ww (msg, control, c2)) children

        (* Verilog outputs...*)
        let mcu = ([funique "anon", "microcontroller"], gen_microcontroller(uisa_set))
        let z = function
                | SP_asm(_, ll, regs, symbols, code) -> code
                | (_) -> sf "zans other"
        let code = foldr (romgen mname) [] (map z ans)
        output_romcode mname 0 code
        output_microcontroller ww' "plain" mcu
        (*      let _ = output_microcontroller ww' "pipe1" (opath_restructure_vm ww' ("micocontroller", control, []) mcu)
        *)
        let dir = { g_null_directorate with duid=next_duid() }
        let _ = unwhere ww
        let vm = HPR_VM2(minfo', decls, children', [H2BLK(dir, gen_SP_par PS_joining ans)], assertions)
        let ii =
            { ii with
                  generated_by= phasename
                  vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
            }
        (ii, Some vm)


// Output some initialised C# for use in Gas63 assembler.
let gen_gas_table(x: hpr_isa_t) =
    let db = new ListStore<string, inst_t>("db")
    let y = YOVD 0 


    let collate_by_mnemonic (i:inst_t) =
        //youtln y ("hello " + i.nm)
        db.add i.nm i
        ()
    app collate_by_mnemonic x.instructions
    let jig (il:inst_t list) =
        let nm = (hd il).nm
        // example biw_op("JSR", (128+32)+('4'), 0xaa9DADBD);
        // Pack these with immed8 in the bottom 8 bits, then direct, extended, x0, x8 and x16 (six bytes). 
        let dig e =
            //let _ = vprintln 0 ("dig encoding " + nm + " from " + e)
            match ucomp_p e with
                | W_node(prec, V_interm, [l; r], _) ->
                    int64(xi_manifest "dig" r)

        let col (c:int64) (i:inst_t) =
            match i.am with
                | "RL"   ->  c ||| (dig i.e)
                | "0"    ->  c ||| (dig i.e)                

                | "I"   ->  c ||| (dig i.e)
                | "D"   ->  c ||| ((dig i.e) * 0x100L)
                | "E"   ->  c ||| ((dig i.e) * 0x10000L)
                | "xI"  ->  c ||| ((dig i.e) * 0x1000000L)
                | "xE"  ->  c ||| ((dig i.e) * 0x100000000L)
                | "a"   ->  c ||| ((dig i.e) * 0x10000000000L)
                | "x"   ->  c ||| ((dig i.e) * 0x1000000000000L)                                
                | other -> sf ("other am form " + other)
        let code = function
                | "RL"   ->  '1'
                | "0"    ->  '0'

                | "xI"  
                | "a"  
                | "I"  
                | "D"  
                | "E"  
                | "x" 
                | "xE"  ->  '6'
                | other -> sf ("other am code form " + other)
        let packed = List.fold col (0L) il
        youtln y (sprintf "biw_op(\"%s\", (128+32)+('%c'), 0x%016XL);" nm (code (hd il).am) packed)
        ()
    for i in db do jig i.Value done
    ()


(* eof *)
