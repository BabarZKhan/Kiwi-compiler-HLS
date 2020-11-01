
// Used for testing meox.fs - currently in regression.

open yout
open meox
open hprls_hdr
open moscow
open abstract_hdr
open cpp_render



let _ = vprintln 0 "TEST START"

let _ = vprintln 0 ("Test 1a t = " + xbToStr(xi_and(X_true, X_true)))
let _ = vprintln 0 ("Test 1b f = " + xbToStr(xi_and(X_true, X_false)))
let _ = vprintln 0 ("Test 1c f = " + xbToStr(xi_deqd(xi_num 2, xi_num 3)))


let _ = vprintln 0 "TEST ENDED"

open hprxml

let filename = (if true then "../kiwi-fe-xml-example/test1-example/vmtop-ser.xml" else "http://www.mono-project.com/news/index.rss2")

let des_bnet atts lst = 
    let id = xml_once "ID" (fun a it -> killquotes(xml_get_atom it)) None lst
    let width = xml_once "WIDTH" (fun a it -> xml_get_int it) None lst
    let len = xml_once "LENGTH" (fun a it -> xml_get_int it) (Some -2) lst
    let len = if len = -2 then [] else [len]
    let max = xml_once "MAX" (fun a it -> xml_get_int it) (Some 0) lst    
    let signed = (xml_insist "SIGNED" ["TRUE"; "FALSE"] (Some 0) lst) = 0
    let varmode = match (xml_insist "VARMODE" ["LOCAL"; "RETVAL"; "INPUT"; "OUTPUT"; "INOUT"] (Some 0) lst) with
                                            | 0 -> LOCAL
                                            | 1 -> RETVAL
                                            | 2 -> INPUT
                                            | 3 -> OUTPUT
                                            | 4 -> INOUT
    let vtype = match (xml_insist "VTYPE" ["VALUE"; "EVENT"; "PARAMETER"; "REGISTER"; "MUTEX"; "ELAB"] (Some 0) lst) with
                                            | 0 -> V_VALUE
                                            | 1 -> V_EVENT
                                            | 2 -> V_PARAMETER
                                            | 3 -> V_REGISTER
                                            | 4 -> V_MUTEX
                                            | 5 -> V_ELAB("*anon*")

    let spint = false
    in net_retrieve(X_bnet (iogen_serf(id, len, max, width, varmode, signed, None, spint, atts)))
    


let des_BNET a lst =  net_retrieve(X_net(killquotes(xml_get_atom lst)))
    
let des_onet a lst = net_retrieve(X_net(killquotes(xml_get_atom lst)))
    

// List of nets
let des_nets atts lst =
    let a0 = xml_collate "bnet" des_bnet lst
    let a1 = xml_collate "BNET" des_BNET lst    
    let a2 = xml_collate "NET" des_onet lst    
    let _ = vprintln 0 ( "des_net " + sfold (xmlToStr 0) lst)
    let ans = a0 @ a1 @ a2
    in ans:hexp_t list

    
let des_net a lst =
    let ans = (des_nets a lst) 
    let _ = if ans = [] then sf ("no net in " + sfold (xmlToStr 0) lst) else ()
    in hd ans
    
let rec des_bexps ats lst = 
    let kbe1 x =
        match x with
        | (XML_ATOM("true")) -> X_true
        | (XML_ATOM("false")) -> X_false        
        | (XML_ATOM("X_true")) -> X_true
        | (XML_ATOM("X_false")) -> X_false        
        | (XML_ELEMENT("BDIOP", ats, l)) ->            
            let op = valOf_or_failf (fun () -> "no bdiop op " + xmlToStr 0 x) (op_assoc "op" ats)
            let pol = op_assoc "pol" ats
            let inv = if pol=None then false
                      elif pol=Some "neg" then true
                      elif pol=Some "pos" then false
                      else sf "bad pol"
            let (z, symb, prec, xml) = valOf_or_fail "unrec bdiop op" (strToXb_dop op)
            let args = des_exps ats l // need to delete whitespace...
            let ans = ix_bdiop(z, args, inv)
            ans

        | _ -> sf ("kbe1 desbexp" + xmlToStr 0 x)

    let rec kbe (x, c) =
        match x with
            | (XML_ELEMENT(token, a, l)::t) ->
                let v = kbe1 (hd x)
                in kbe(t, v::c)
            | (_ :: t) -> kbe(t, c)
            | [] -> rev c
    let ans = kbe (lst, [])
    in ans : hbexp_t list
    
and des_exps ats lst : hexp_t list = 
    let ke1 x =
        match x with
        | (XML_ELEMENT("NUM", ats, l)) ->
            let v = xml_get_int l
            in xgen_num v

        | (XML_ELEMENT("NODE", ats, l)) ->
            let args = list_flatten(xml_collate "ARGS" des_exps l)
            let op = valOf_or_failf (fun ()-> "no diop op : " + xmlToStr 0 x) (op_assoc "op" ats)
            let (z, symb, prec, xml) = valOf_or_fail "unrec bdiop op" (strToX_dop op)
            let ans = xi_node(z, args)
            in ans
            
        | (XML_ELEMENT("BNET", ats, l)) ->            
            let r = des_BNET ats l
            in r


        | (XML_ELEMENT("APPLY", a, l)) ->            
            let name = xml_once "NAME" (fun a it -> xml_get_atom it) None l
            let args = list_flatten(xml_collate "ARGS" des_exps l)
            let gis = valOf_or_fail ("not recognised: " + name) (builtin_fungis name)
            
            in xi_apply((name, gis), args)
            
        | _ -> sf ("desexp" + xmlToStr 0 x)
    let rec ke (x, c) =
        match x with
            | (XML_ELEMENT(token, a, l)::t) ->
                let _ = vprintln 0 "start"
                let v = ke1 (hd x)
                in ke(t, v::c)
            | (_ :: t) -> ke(t, c)
            | [] -> rev c

    let ans = ke(lst, [])
    in ans:hexp_t list

let des_bexp ats lst = 
    let ans = des_bexps ats lst
    in if length ans = 1 then hd ans else sf "too many bexps"

let des_exp ats lst = 
    let ans = des_exps ats lst
    in if length ans = 1 then hd ans else sf "too many exps"

    
let des_bev a lst =
    let rec kb c = function
        | XML_ELEMENT("Xlabel", a, l) -> gen_Xlabel (killquotes(xml_get_atom l))::c
        | XML_ELEMENT("Xcomment", a, l) -> gen_Xcomment (killquotes(xml_get_atom l))::c        
        | XML_ELEMENT("Xskip", a, l) -> Xskip::c
        | XML_ELEMENT("Xreturn", a, l) -> gen_Xreturn(des_exp a l)::c

        | XML_ELEMENT("Xgoto", ats, l) ->
            let s = ref None
            let rec sc x =
                match x with
                    | (XML_ATOM(v)::t)    -> (s := Some(killquotes(v)); sc t)
                    | ( _ ::t) -> sc t
                    | [] -> ()
            let _ = sc l
            let _ = if  !s = None then sf ("Xbeq wanted two args " + sfold (xmlToStr 0) l) else ()
            in (gen_Xgoto (valOf (!s)))::c

        | XML_ELEMENT("Xbeq", a, l) ->
            let e = ref None
            let s = ref None
            let rec sc x =
                match x with
                    | (XML_ATOM(v)::t)    -> (s := Some(killquotes(v)); sc t)
                    | (XML_ELEMENT _ ::t) -> (e := Some(des_bexp [] [hd x]); sc t)
                    | ( _ ::t) -> sc t
                    | [] -> ()
            let _ = sc l
            let _ = if !e = None || !s = None then sf ("Xbeq wanted two args " + sfold (xmlToStr 0) l) else ()
            in (gen_Xbeq (valOf (!e)) (valOf (!s)))::c
            
        | XML_ELEMENT("Xassign", a, l) ->
            let exps = des_exps [] l
            let _ = if length exps <> 2 then sf ("Xassign wanted two args") else ()
            in gen_Xassign(hd exps, hd(tl exps))::c
            
        | XML_ELEMENT("Xeasc", a, l) -> Xeasc(des_exp a l)::c

        | _ -> sf ("desbev" + sfold (xmlToStr 0) lst)
    in List.fold kb [] lst

let rec des_sp ats lst = 
    //    let _ = sf ("temp stop "  + sfold (xmlToStr 0) lst)
    let des_step ats lst =
        let bev = des_bev ats lst
        //        let _ = if length bevs <> 1 then sf ("exactly one step expected") else ()
        in bev

    let des_dic ats lst = 
        let bevs = list_flatten(xml_collate "STEP" des_step lst)
        let str = valOf_or_fail "no SP_dic id" (op_assoc "id" ats)
        let len = List.length lst
        let ar = Array.create len Xskip
        let rec store pos =
               function
                   | [] -> ()
                   | (h::t) -> (vprintln 0 " storing"; Array.set ar pos h; store (pos+1) t)

        let _ = store 0 bevs
        let _ = vprintln 0 ("dic steps: " + sfold hbevToStr bevs)
        in SP_dic(ar, len, str)

    let dics = xml_collate "SP_dic" des_dic lst           



    let des_l v    = SP_l v
    let ls = map des_l (list_flatten(xml_collate "SP_l" des_bev lst))
    
    let des_par a v  = muddy "SP_par v"
    let pars = list_flatten(xml_collate "SP_par" des_sp lst)

    let des_seq a v  = SP_seq v
    let seqs = list_flatten(xml_collate "SP_seq" des_sp lst)       

    in dics @ pars @ seqs @ ls (* For now *)
    
let des_blk a lst = 
    let sens = xml_collate "SENSITIVITY" des_net lst
    let sp = des_sp a lst
    let sens = if length sens > 1 then sf "two or more sensitivites" else if sens=[] then None else Some(hd sens)
    in H2BLK(sens, SP_par(PS_joining, sp))

let des_exec a lst = 
    let blks = xml_collate "H2BLK" des_blk lst
    in (*list_flatten*) (blks)
    
let rec des_vm2 ats lst =
    let str = valOf_or_fail "no vm2 minfo_name id" (op_assoc "minfo_name" ats)
    let _ = vprintln 0 ("xml: Reading vm2 " + str)
    let idl = [str]
    let minfo = { name = idl; atts = []; }
    let gdecls = list_flatten(xml_collate "GDECLS" des_nets lst )
    let _ = vprintln 0 "gdelcs done"
    let ldecls = list_flatten(xml_collate "LDECLS" des_nets lst) 
    let  _ = reportx 0 "ldecls" netToStr ldecls
    
    let sons = list_flatten(xml_collate "SONS" des_son lst) 
    let execs = list_flatten(xml_collate "EXECS" des_exec lst)
    let assertions = []
    //let  _ = reportx 0 "gdecls" netToStr gdecls

    in (idl, HPR_VM2(minfo, gdecls, ldecls, sons, execs, assertions))

and des_son a lst = xml_collate "VM2" des_vm2 lst


let tree = readxml(filename)

let vm = xml_collate "VM2" des_vm2 [tree]

let ww = ["wherewhere"]
//let l = convert_to_cpp ww "tester" (hd vm, "sostest", Some "")
//rtl_output2 ww (root, filename, vernets, items) =
    
let l = rtl_output2 ww (root, filename, nets, items)

let _ = vprintln 0 " got vm"




;




;
