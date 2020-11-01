



let pstyleToStr (PS_lockstep) = "lockstep"
|   pstyleToStr (PS_joining) = "joining"
|   pstyleToStr (PS_eureka) = "eureka"
|   pstyleToStr (_) = "PS_unspec"
;

    let anal_asm concisef (ar, len, _, _) =
                let rec i2 pos =
                    function
                        | (Icompiled(ins, code)) -> anal concisef pos None (Xeasc code)
                        | (_) -> ()
                let rec i1 pos =
                    if pos >= len then ()      
                    else let ins = ar.[pos] in (i2 pos ins; i1(pos+1))
                in i1 0






(*
 * Important optimisation here:   for boolean a : bufif(a&e, a, 0)  means a is always zero: not if set one elsewhere that does not get trimmed out by a PRIOR propagation : order of applications matters ?
 *                                for boolean a : bufif(~a, a, 1)  means a is always one
 *
 * Clauses...
 *)
let gen_XRTL_buf(x_true, l, r) = XIRTL_buf(x_true, l, r)

|   gen_XRTL_buf(g, l, r) = 
    let nominal = XIRTL_buf(g, l, r)
        let w = ewidth "gen_XRTL_buf" l
        let mr = xmonkey(r)
        let clauses = [g] (* FOR NOW xi_valOf(xi_clauses(g, xi_nil)) *)

        let l_is_member_pos (g::t) = x_eq(xi_blift g, l) || l_is_member_pos t
        |   l_is_member_pos [] = false

        let l_is_member_neg (g::t) = x_eq(xi_blift(xi_not g), l) || l_is_member_neg t
        |   l_is_member_neg [] = false

        in if (* THIS NBEEDS TO BE IN TERMS NOT CLAUSES *)
           w=Some 1 && l_is_member_pos clauses && mr=Some false then 
        (
        vprint(3, "Holder trim: Replacing " + xrtlToStr nominal + " with constant assign of false.\n");
        XIRTL_buf(x_true, l, xgen_num 0)
        )
        else if w=Some 1 && l_is_member_neg clauses && mr=Some true then
        (
        vprint(3, "Holder trim: Replacing " + xrtlToStr nominal + " with constant assign of true.\n");
        XIRTL_buf(x_true, l, xgen_num 1)
        )
        else nominal end
;

let gen_XRTL_arc(pp, g, l, r) = XIRTL_arc(pp, g, l, r) (* Can optimise me too ? *)
;


(*//
// Wrap an RTL arc as an HPR VM executable cmd (exec).
//*)
let gen_SP_rtl([]) = [] 
|   gen_SP_rtl(other) = [ SP_rtl(other) ]
;







(* ---------------------------------------------------------*)

let hpr_flatten lostios (m as HPR_VM2(minfo, topports, decls, sons, a, b)) = 
    let 
        let dp(n as x_bnet f, (p, l)) = if op_assoc(#id f, lostios)<>None ||
                isio(#xnet_io f) then (n::p, l) else (p, n::l)
          | dp _ = sf "dp other"


        let flat((idl, HPR_VM2(minfo, ports, decls, sons, a, b)), (d, al, bl)) = 
            let c' = (ports @ decls @ d, a @ al, b@bl)
            let _ = reportx 3 ("Flatten item ports " + underscore_fold idl) netToStr ports
            let _ = reportx 3 ("Flatten item decls " + underscore_fold idl) netToStr decls
            in foldl flat c' sons end

        let (df, af, bf)  = flat(([], m), ([], [], []))
        let _ = reportx 3 "Flattened nets " netToStr df
        let (ports, locals) = foldl dp ([], []) df
        let _ = reportx 3 "Flattened top ports " netToStr ports
        let _ = reportx 3 "Flattened locals " netToStr locals
    in HPR_VM2(minfo, ports, locals, [], af, bf) end
;


(* ---------------------------------------------------------*)


(*
 * Easy to convert from sd to xi sorted forms:
 *)
let sd2xi l = xiS(map fst l)  (* Both internally use same sort order! *)




let 
  hprSPSummaryToStr (SP_l b) = "SP_l ..."
| hprSPSummaryToStr (SP_asm(ar, len, _, _, _)) = "SP_asm len=" + (i2s len)
| hprSPSummaryToStr (SP_dic(dic, lendic, id)) = "SP_dic id=" + id + " lendic=" + (i2s lendic)
| hprSPSummaryToStr (SP_par l) = "SP_par ..."
| hprSPSummaryToStr (SP_seq l) = "SP_seq ..."
| hprSPSummaryToStr (SP_rtl l) = "SP_rtl ..."
| hprSPSummaryToStr (SP_fsm l) = "SP_fsm ..."
| hprSPSummaryToStr (SP_constdata d) = "SP_constdata ..."
| hprSPSummaryToStr (SP_comment s) = "// " + s + " //" 
| hprSPSummaryToStr x = "??hprSPSummaryToStr"
;

let
  execSummaryToStr (H2BLK(None, v)) = "No sensitivity: " + hprSPSummaryToStr v
| execSummaryToStr (H2BLK(Some s, v)) = "Sensitivity = " + xToStr s + ": " + hprSPSummaryToStr v
;




let gen_SP_lockstep items = gen_SP_par PS_lockstep items
;


(*
 * New symbolic evaluator: this one uses a simple scalar env but perhaps better to
 * use logic_assoc_exp : TODO move the code from hw_e2 here please.
 *)
let 
  eval_exp ww SS x_undef -> x_undef

| eval_exp ww SS (x_net s) -> x_net s (* Could look up in SS ? *)

| eval_exp ww SS (x_bnet f) -> 
  let l = op_assoc(x_bnet f, SS)
  in if l=None then x_bnet f
  else valOf l
  end

| eval_exp ww SS (x_pair(l, r)) -> x_pair(eval_exp ww SS l, eval_exp ww SS r)

| eval_exp ww SS (w_apply(gis, l, _)) -> xi_apply(gis, map (eval_exp ww SS) l)

| eval_exp ww SS (w_string(s, k, w)) -> w_string(s, k, w)

| eval_exp ww SS (w_node(oo, l, _)) -> xi_node(oo, map (eval_exp ww SS) l)

| eval_exp ww SS other -> 
  if constantp other then other 
  else sf ("other in eval_exp: " + xkey other + ": " + xToStr other)

and
  eval_bexp w SS (A as X_true) -> A
| eval_bexp w SS (A as X_false) -> A
| eval_bexp w SS (A as X_dontcare) -> A


| eval_bexp ww SS (w as w_bdiop(oo, lst, inv, _)) ->
        (* NB: orred is a bdiop with one arg. *)
    let lst' = map (eval_exp ww SS) lst
        let ans = xi_bdiop(oo, lst', inv)
        in ans end

| eval_bexp ww SS(w as w_bdiop(v_orred, [item], inv, _)) ->
        let 
        let n = eval_exp  ww SS item 
        let xii_orred inv a = xint_write_bdiop(v_orred, [a], inv)
        let r = xii_orred inv n
        let _ = lprint 30 (fun()-> xbToStr r + " assoc of orred XINT_NODE\n")
        in r end

| eval_bexp ww SS(w as w_bnode(oo, lst, inv, _)) ->
  let
        let lst' = map (eval_bexp ww SS) lst
        let r = xi_bnode(oo, lst', inv)
        in r end

| eval_bexp ww SS(w as w_bsubsc(l, r, inv, _)) ->
        let 
            let l' = eval_exp ww SS l
            let r' = eval_exp ww SS  r
        let r = xi_bsubsc(l', r', inv)
        in r end




| eval_bexp ww SS(x as w_linp(v, lst, _)) ->
  let v' = eval_exp ww SS v
      in if v' = v then x  
      else let 
      let r = if false then mya_bunpak_linp(v', lst)  (* Does this really need to get remade ? *)
            else xi_linp(v', lst)
      let _ = lprint 30 (fun()-> "Result=" + xbToStr r + " assoc of XINT_LINP\n")
      in r end end


| eval_bexp ww SS other -> 
  if bconstantp other then other 
  else sf ("other in beval_exp: " + xbkey other + ": " + xbToStr other)
;

let eval_xbev ww SS pc (Xlinepoint(_, s)) -> eval_xbev ww SS pc s
|   eval_xbev ww SS pc (Xskip) -> ([(SS, pc+1)], Xskip)
|   eval_xbev ww SS pc (Xlabel s) -> ([(SS, pc+1)], Xlabel s)
|   eval_xbev ww SS pc (Xcomment s) -> ([(SS, pc+1)], Xcomment s)

|   eval_xbev ww SS pc (Xassign (l, r)) -> 
    let r' = eval_exp ww SS r
        let lokf (x_bnet f) = Some(x_bnet f)
        |   lokf _ = None
        let lok = lokf l 
        let SS' = if lok=None then SS else (valOf lok, r') :: (delete_assoc (valOf lok) SS)
        in ([(SS', pc+1)], Xassign(l, r'))
    end

|   eval_xbev ww SS pc (Xeasc e) -> ([(SS, pc+1)], Xeasc(eval_exp ww SS e))

|   eval_xbev ww SS pc (Xcall(gis, l)) -> ([(SS, pc+1)], Xcall(gis, map (eval_exp ww SS) l))

|   eval_xbev ww SS pc (Xbeq(g, d, v)) -> 
    let g' = eval_bexp ww SS g
        let n = Xbeq(g', d, v)
    in if bconstantp g' then 
    (if xi_bmanifest "eval_xbev" g'
      then ([(SS, !v)], n)
      else ([(SS, pc+1)], n)
    )
    else ([(SS, !v), (SS, pc+1)], n)
    end

|   eval_xbev ww SS pc (Xgoto(d, v)) -> ([(SS, !v)], Xgoto(d, v))
|   eval_xbev ww SS pc (Xreturn(v)) -> ([], Xreturn(eval_exp ww SS v))
|   eval_xbev ww SS pc other -> sf("other in eval_xbev: " + hbevToStr other)
;

let isreturn (Xreturn _) -> true
|   isreturn _ -> false
;

(*
 * Different from diosim's one. Partially symbolic?
 *)
let rec eval_h2sp ww e = function
    | (SP_dic(dic, lendic, id)) -> 
        let dic' = Array.create(lendic, Xbreak)
        let assumptions = Array.create(lendic, None)
        let ww' = WN ("eval_dic " + id) ww 
        let find_delta (a,b) = list_union(list_subtract(a,b), list_subtract(b,a))
        let write p (b', a) = (Array.set(dic', p, b'); Array.set(assumptions, p, Some a))
        let ans = ref []
        let vd = 0
        let display (l,r) = xToStr l + ":=" + xToStr r
        let loopies = ref []
        let _ = lprint vd (fun()->"---------------------------------------elet dic\n")
        let m ((e, pc)::t) = 
            if pc >= lendic then (mutadd ans e; m t) else
            let b = Array.sub(dic, pc)
                let _ = lprint vd (fun()->i2s pc + ": dic elet " + hbevToStr b + "\n")
                let ov = Array.sub(assumptions, pc)
                let delta = someOf_or_none(if ov=None then [] else find_delta(valOf ov, e))
                let _ = if ov<>None && delta<>None then 
                    (
                    reportx vd "Previous" display (valOf ov); 
                    reportx vd "Current" display (e); 
                    reportx vd "Differences" display (valOf delta); 
                    sf "outdate constant assupmtions" 

                    )
                    else ()
                in if ov <> None then (mutadd loopies e; m t)
                else 
                let
                let (el, b') = eval_xbev ww' e pc b
                let _ = write pc (b', e)
                let _ = if isreturn b then mutadd ans e else () (* TODO side effecting call in exp *)
            in m (t @ el) end end

        |   m [] = ()

        let _ = m [(e, 0)]
        let ansa = list_once (!ans)
        let lans = length ansa
        let _ = lprint vd (fun()->"---------------------------------------end dic\n")
        let lloopies = length(!loopies)
        let efinal = if lans = 0 && lloopies > 0 then hd (!loopies) else
           if lans<>1 then sf("eval_h2sp gave " + i2s lans + " answers\n") else hd (ansa)
        in (efinal, SP_dic(dic', lendic, id)) end 

| eval_h2sp ww e (other) = muddy("eval_h2sp other form:" + hprSPSummaryToStr other)

;











(*----------------------------------*)





(*
 * 

(*
 * Temporary routine.  
 * Routine to ensure non-referentially transparent functions are only called once.
 * Only works on dics. Only matches certain forms: eg branch conditions.
 *)
let uniquify_non_reftransp ww morenets (SP_dic(ar, len, id)) =
    let vd = 3
    let _ = WF 3 "uniquify non ref transparent" ww "start"

    let tnet id = 
        let r = x_bnet(iogen_serf(id, [], 0, 1, LOCAL, false, None, false, []))
        let _ = mutadd morenets r
        in r


        //(* Can we use restructure for this or some other walker? *)

    let rec buni_maid m (nd, it) = 
            let kf x = uni_maid m(nd, x)
            let bf2 (w_linp(v, _, _), N) = N
            |   bf2 (w_bnode(v, l, i, _), N) = xi_bnode(v, map bf l, i)
            |   bf2 (w_bdiop(v, l, i, _), N) = xi_bdiop(v, map kf l, i)
            |   bf2 (w_bitsel(l, r, i, _), N) = xint_write_bitsel(kf l, r, i)
            |   bf2 (w_bsubsc(l, n, i, _), N) = xi_bsubsc(kf l, kf n, i)
            |   bf2 (other, _) = muddy ("abstract bf other: " + xbToStr other)
            and bf n = bf2(n, n)
            in bf it

        and uni_maid m(nd, it) =
            let bf x = buni_maid m(nd, x)
            and kf2 (x_net _, N) = N
            |   kf2 (x_bnet _, N) = N
            |   kf2 (w_string _, N) = N
            |   kf2 (x_undef, N) = N
            |   kf2 (x_num _, N) = N
            |   kf2 (x_tnum _, N) = N
            |   kf2 (x_blift b, N) = xi_blift(bf b)
            |   kf2 (x_pair(l, r), N) = xi_pair(kf l, kf r)


            |   kf2 (w_node(k, l, _), N) = xi_node(k, map kf l)
            |   kf2 (w_asubsc(l, r, _), N) = xi_asubsc(kf l, kf r)
            |   kf2 (w_query(g, t, f, _), N) = xi_query(bf g, kf t, kf f)

            // Of course this buggers up strictness: making it always called exactly once, which is what we want in the current demos of test and set.
            |   kf2 (w_apply((f, gis), args, _), N) = if not(#nonref gis) 
                   then xi_apply((f, gis), map kf args)
                   else let
                        let t = tnet(funique(f + "_res"))
                        let _ = vprint(vd, m + ": unr operating for f=" + xToStr N + ", new ret net=" + xToStr t + "\n")
                        let _ = mutadd nd (Xassign(t, xi_apply((f, gis), map kf args)))
                        in t end

            |   kf2 (v, N) = sf("uniqifify kf other: " + xkey v + " " + xToStr v)
            and kf n = kf2(n, n)
        in kf it end


        let uni m it =
            let nd = ref []
            let ans = uni_maid m(nd, it)
            in (!nd, ans)

        let buni m it =
            let nd = ref []
            let ans = buni_maid m(nd, it)
            in (!nd, ans)

        let rk m (Xbeq(starter, d, n)) =        
            let (nd, k) = buni m starter
            in xgen_block(nd @ [ Xbeq(k, d, n)] )


            //|   rk m (A as Xbeq(x_not(x_orred(g as x_apply((f, gis), args))), d, n)) = 
            //    if not(#nonref gis) then A

        |   rk m (Xassign(w_asubsc(l, s, _), r)) = 
            let (nd, r') = uni m r
                let (nd1, s') = uni m s
            in xgen_block(nd @ nd1 @ [ Xassign(xi_asubsc(l, s'), r')] ) end 

        |   rk m (Xassign(l, r)) = (* need really to do lhs subcs rmodes too *)         
            let (nd, r') = uni m r
            in xgen_block(nd @ [ Xassign(l, r')] ) end 

        |   rk m other = other



        let _ = WF 3 "uniquify non ref transparent" ww "end"
        in () end
;








(*
 * This is a very fussy and temporary routine - it performs the 'limited subset
 * of decideable logic and integer arithmetic' referred to in the mitercon paper.
 * Another evaluator is xsim in diosim.sml (uses lifted concrete data), or else use 
 * rewrite_exp or eval_exp. fsmgen.sml has a specialised elet called hw_e2.
 *)
let xeval_diop (oo, d, e) = 
    let exception diop of string
        let k_diop_help(f, x_num a, x_num b) = x_num(f(a,b))
        |   k_diop_help(f, a ,b) = raise diop(xToStr a + ": xeval_diop_help : " + xToStr b)

        fun
(*          k_bdiop (x_logand(d, e)) = k_diop_help(fn(a,b)->if a=0 then a else b, d, e) *)
(*      | k_diop (v_xor, d, e) = if d = x_not(e) || x_not(d)=e then x_num 1 *)

          k_bdiop (v_dltd, d, e) = k_diop_help(fn(a,b)->if a<b then 1 else 0, d, e)

        | k_bdiop (v_dled, d, e) = k_diop_help(fn(a,b)->if a<=b then 1 else 0, d, e)
        | k_bdiop (v_deqd, d, e) = 
                if d=e then x_num 1
                else k_diop_help(fn(a,b)->if a=b then 1 else 0, d, e)
        | k_bdiop (v_dned, d, e) = 
                if d=e then x_num 0
                else k_diop_help(fn(a,b)->if a<>b then 1 else 0, d, e)
        | k_bdiop (oo, d, e) = raise sfault (fst(xbToStr_dop oo) + ": k_bdiop")

        let k_diop (V_bitor, d, e) = k_diop_help(fn(a,b)->perform_bitor 31 a b, d, e)

        | k_diop (V_bitand, e, a as X_num 0) = a
        | k_diop (V_bitand, a as X_num 0, e) = a


        | k_diop (V_bitand, d, e) = k_diop_help(fn(a,b)->perform_bitand 31 a b, d, e)

        | k_diop (V_xor, d, e) = 
                 if d=e then x_num 0
                else k_diop_help(fn(a,b)->perform_bitxor a b, d, e)

        | k_diop (V_minus, d, e) = k_diop_help(op-, d, e)
        | k_diop (V_plus, d, e) = k_diop_help(op+, d, e)
        | k_diop (V_times, d, e) = k_diop_help(op*, d, e)
        | k_diop (V_divide, d, e) = k_diop_help(fn(a,b)->a div b, d, e)
        | k_diop (V_mod, d, e) = k_diop_help(fn(a,b)->a mod b, d, e)
        | k_diop (oo, d, e) = raise sfault (fst(xToStr_dop oo) + ": k_diop")
        let ans = k_diop(oo, d, e) handle (diop _) -> xi_node(oo, [d, e])
        in ans
        end
;

let 
  xbeval m p G (x as X_false) = xgen_num 0
| xbeval m p G (x as X_true) = xgen_num 1
| xbeval m p G (w_bdiop(v_orred, [k], false, _)) = x_num(if (xeval m p G k)<>x_num 0 then 1 else 0)
| xbeval m p G (w_bdiop(v_orred, [k], true, _)) = x_num(if (xeval m p G k)=x_num 0 then 1 else 0)
| xbeval m p G (w_bnode(v_band, [a, b], false, _)) = xi_num(if (xbeval m p G a)=x_num 0 then 0 else if xbeval m p G b <> x_num 0 then 1 else 0)
| xbeval m p G (w_bnode(b_bor, [a, b], false, _)) = xi_num(if (xbeval m p G a)<>x_num 0 then 1 else if xbeval m p G b <> x_num 0 then 1 else 0)
| xbeval m p G (w_bnode(v_bxor, [a, b], false, _)) =
     let a' = xbeval m p G a
         let b' = xbeval m p G b
         in xgen_num(if (a'=x_num 0 && b'<>x_num 0) || (a'<>x_num 0 && b'=x_num  0) then 1 else 0) end
| xbeval m p G (w_bitsel(e, s, i, _)) = raise sfault("xeval: bitsel used instead of subsc")
| xbeval m p G (other) = raise sfault(m + ":" + (xbToStr other) + " xbeval p other")

and
  xeval m p G (x as x_num n) = x
| xeval m p G (x as x_tnum n) = x
| xeval m p G (x_blift x) = xbeval m p G x
| xeval m p G (w_asubsc(e, s, _)) = 
  let s' = xeval m p G s
        let e' = xeval m p G e
        in if constantp e' then xi_bitand(xgen_rshift(e', s'), x_num 1) else let
      let dix(p, [item]) = item
        | dix(p, h::t) = if p=0 then h else dix(p-1, t)
      let sv = dix(p, G)
      let r0 = op_assoc(xToStr(x_subsc(e, s')), sv)
  in if r0=None then xi_asubsc(xeval m p G e, s')
     else valOf r0
  end end

| xeval m p G (x_x(e, n)) = xeval m (p+n) G e
| xeval m p G (w_string(s, XS_withlet x, _)) = x
| xeval m p G (A as w_string _) = A
| xeval m p G (w_query(g, d, e, _)) =
  let let j (x_num n) = if n=0 then xeval m p G e else xeval m p G d
        |   j (other) = xgen_query(xgen_orred other, xeval m p G d, xeval m p G e)
      let  g' = xbeval m p G g
  in j g'  
  end
| xeval m p G (w_node(oo, [d, e], _)) = xeval_diop(oo, xeval m p G d,  xeval m p G e)
| xeval m p G (net as x_bnet f) = 
  let let dix(p, [item]) = item
        | dix(p, h::t) = if p=0 then h else dix(p-1, t)
        let abend1 (p,v) = vprint(0, p + " binding " + xToStr v + "\n")
        let abend l = (vprint(0, "Abend site: \n"); app abend1 l)       
      let sv = dix(p, G)
      let r0 = op_assoc(#id f, sv)
        let r0 = if r0=None && (*TODO kludge *) #id f = "roger" then Some(xgen_num 0) else r0
      let _ = if r0=None 
        then (app abend G; raise sfault(m + ": unbound xeval var: " + (#id f))) 
        else ()
  in if valOf r0=x_undef then net else valOf r0
  end
| xeval m p G (other) = raise sfault(m + ":" + (xToStr other) + " xeval p other")
;








(* Partition executable code into construction and runtime *)
let constructor_pred (H2BLK(Some a, _)) = (a = g_construction_big_bang)
|   constructor_pred (H2BLK _) = false
;



//

(*
 * used only by newarr
 * TODO: use table based on bin_types found in cilgram.sml
 *)
let classToType ww C2 = function
    | (Cil_lib(s, Cil_cr_dot(a, "Int32"))) -> (Ciltype_int32)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Int64"))) -> (Ciltype_int64)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Int16"))) -> (Ciltype_int16)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Int8"))) ->  (Ciltype_int8)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Char"))) ->  (Ciltype_char)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "object"))) ->  (Ciltype_object)
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Boolean"))) ->  Ciltype_bool
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "String")))  ->  Ciltype_string

    // Byte and Sbyte are found in the C# source code and in the System class library but are not a separate part of the CIL type system.
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Byte")))    ->  Ciltype_unsigned_int8
    |   classToType ww C2 (Cil_lib(s, Cil_cr_dot(a, "Sbyte")))    ->  Ciltype_int8
    (*|   classToType ww C2 (Cil_cr_array(x, y))  -> Ciltype_array(classToType ww C2 x, y)
    *)
    |   classToType ww C2 (C as Cil_cr_tpram_id s)  -> (* rehydrate_ctype ww C2 *) (Ciltype_class C)
    |   classToType ww C2 (C as Cil_cr_tpram_arg n) -> (* rehydrate_ctype ww C2 *) (Ciltype_class C)

    |   classToType ww C2 (Cil_cr_bin t) -> (* rehydrate_ctype ww C2 we dont have cgrs in this leaf rtn *) t

    |   classToType ww C2 (cr as Cil_cr_dot _) -> Ciltype_class cr
    |   classToType ww C2 (cr) -> (vprint(3,"+++other classToType: " + cil_classrefToStr cr + "\n"); Ciltype_class cr) 
;


y
(* 
 * Simulate using builtin diosim.
 *)
let opath_simulate_vm ww0 (msg, control, c2) design =
    let ww = (WN "opath_simulate" ww0)
    let plot   = arg_assoc_or "opath_simulate" ("plot", c2, [])
    let title  = arg_assoc_or "opath_simulate" ("title", c2, [])
    let traces = arg_assoc_or "opath_simulate" ("traces", c2, [])
    let sim_cycles = control_get_i c2 "sim" 250
    let dl = [design]
    let k x = if x=[] then None else Some(hd x)
    let options = (k plot, k title, k traces)

    let anals = list_flatten(map (analyse ww "Simulate analysis:" (YOVD 3)) dl)
    
    //    let _ = try diosimulate (WN "diosim final" ww) ("simulate", options, dl, anals, sim_cycles)
    //                 with Match -> sf("rethrow match in simulate") 
    in design
;

let opath_fsmgen ww K2 M =
    let (msg, control, c2) = K2
    let skip = control_get_s c2 "skip-propagate" ""
    let M' = opath_fsmgen_core (WN "fsmgen_core" ww)  K2 M
    let M'' = if true || skip<>"" then M'
              else opath_propagate (WN "fsmgen_propagate" ww)  K2 M'
    in M'' 
;


(*
 * output smv code for model checking
 *) 
let opath_smv_vm ww (m, _, c1:string list list) ((idl, vm)) =
   let xrtl_design = (idl, vm)
   (*
   let _ =  smv_tout(underscore_fold(fst xrtl_design), [], xrtl_design, [])
                handle NoSmv(s) -> vprint(0, "No SMV generated because of " + s + "\n")


   let _ = spin_tout(underscore_fold(fst xrtl_design), [], xrtl_design, [])
                handle NoSpin(s) -> vprint(0, "No Promela/Spin generated because of " + s + "\n")

   *)
   in (idl, vm)
;

let opath_fsmgen ww K2 M =
    let (msg, control, c2) = K2
    let skip = control_get_s c2 "skip-propagate" ""
    let M' = opath_fsmgen_core (WN "fsmgen_core" ww)  K2 M
    let M'' = if true || skip<>"" then M'
              else opath_propagate (WN "fsmgen_propagate" ww)  K2 M'
    in M'' 
;

