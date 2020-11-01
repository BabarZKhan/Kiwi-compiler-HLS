//
// CBG Orangepath. HPR L/S Formal Codesign System.
//
//
// The core constructs of hprls are split between abstracte.sml and meox.sml.
// This file contains the main imperative and functional programming constructs.
// It stores all expressions in a 'normalish form' in a memoising heap expression data structure and performs a multititude of peep-hole optimisations..
// Many other generic expression handling routines are in this file as well.
//
// (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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


module meox



let g_default_unsigned_width = 64 // Width of negative integers, when otherwise not determinable but needed.
let g_repack_disable = ref false // This should be controlled at the recipe stage, not here. Nasty side effector but only supresses warnings.
let g_litp_tagging = ref false
let g_covers = true
let g_bdd_enable = false
let g_use_espresso = ref g_covers;
let g_xor_heuristic_value = ref 5
let g_or_heuristic_value  = ref 5 // Probably a bigger default is sensible.
let g_use_linp = false
let g_sorting1 = ref false   // Why is this off by default?
let g_sorting2 = ref false   // rhs query lexographical nest sorting.  Why is this off by default?
let g_keep_goto_arcs = true  (* Keep all goto arcs for now: not necessary really.  Bevelab option... *)
let g_unspecified_array = 202L * 1000L // An unset or aribtrary array length... poor man's representation! TODO - make better.
let g_wondarray_marker_dims = [ g_unspecified_array ] 
let g_meo_vd = -1 // Normally -1 for off
let g_maxdepth = 10000000       // TODO please explain
let g_printing_limit = ref 1000 // xToStr token limit
let g_control_net_marked = "control_net_marked" // TODO rename as notrim or something.  Needed only for LOCALs since we know I/Os must be preserved for signature accuracy.
let s_autoformat = "$$AUTOFORMAT: This will be automatically replaced with a printf formatted string."

let g_enum_loglevel = ref -1 // normally -1 for off.
let g_mvd =           ref -1 

let g_bounda = true

// We use this flag on application-specific FUs (such as ROMs) to avoid reverting to the CV_ canned formal name.
let g_no_decl_alias = ("no-decl-alias", "true")


open yout
open hprls_hdr
open moscow
open linprog
open espresso
open System.Collections.Generic
open System.Numerics
open Microsoft.FSharp.Collections

let g_s32 = { widtho=Some 32; signed=Signed }
let g_s64 = { widtho=Some 64; signed=Signed }

let g_constuse = true

let g_nss_count = ref 100 // Number of times to print a certain warning message

let nemtokToStr = hptos
let profile_q0 = ref 0
let profile_qa = ref 0
let profile_q2 = ref 0

let asize l = // aka dimsfold
   if l=[] then -1L
   else 
       let ans = List.fold (fun a b -> a*b) 1L l
       ans
       // if ans = 0 then sf "asize: had an unspecified dimension/length"

type enumf_t = // Knowledge of equality of an enumeration type and its current value. (c.f. or integerate with linp ?)
// We aim to use knowledge of the limited ranges of enumeration types, but not yet ...
  |  Enum_p of xidx_t        // A bool literal (non-collated) 
  |  Enum_not_an_enum        // Not an enumeration predicate.

  // A comparison against a constant value.
  |  Enum_pv of xidx_t * xidx_t * (int * hexp_t list * xidx_t list) * int // Fields are (unary strobe expression (preferred), variable/exp index, manifest constant in numeric and constant forms of decreasing preference, constant forms being held twice in the same order, strobe group no). 

  |  Enum_none               // don't know yet.

type known_dp_x_t = Dictionary<int, hexp_t> // Dynamic programming
type known_dp_xb_t = Dictionary<int, hbexp_t> // Dynamic programming
type known_t =
    { facts: enumf_t list; // conjunction at the moment: later expand to include disjunction?
      //dp_x: known_dp_x_t; // Dynamic programming cache
      dp_xb: known_dp_xb_t; 
    }

let new_known_dp lst = { facts=lst
                         //dp_x=new known_dp_x_t();
                         dp_xb=new known_dp_xb_t()
                       }


let g_dynp_support = new Dictionary<int, (hexp_t * hexp_t list) list>()

// Use dictionaries not arrays going forward?
let blits = Array.create g_mya_limit None
let negated_covers = Array.create g_mya_limit None

let dontcare_sets = Array.create (2*g_mya_limit) Map.empty // TODO why twice


// Data structures for keeping track of one-hot coding for boolean literals that are exclusive, such as (x==3, x==4, ...)
// These are kept in groups where a group number is allocated for each variable 'x' used in this way: perhaps overly complex since currently we could use x.nn directly, but in future we will keep knowledge of the entire set of enumeration values that x can range over and this would be the same set (ie group) as for other variables or expressions.
let nextStrobegrp        = ref 500 // arbitrary starting point

// A variable that has type enum is actually an X_bnet int that is only used in comparisons against constants.
// Its encoding width changes once the group is closed to boundlog2. One-hot encodings are also possible in the future.
let g_strobe_groups        = OptionStore<xidx_t, int>("strobe_groups")

// Members are kept twice in two ascending ordered  lists. (A list of pairs might be nicer but the first list is a list of integers and we hope memberp for a list of integers is fast.)
// They are expressions of form x==3 etc.
let g_strobe_group_members = OptionStore<int, xidx_t list * enumf_t list * string(*basis*) * (string * int) option(*who closed and encoding width*)>("strobe_group_members") // The  option is set when group is closed to new members. 

// For each boolean expression we can store information about whether it is an enum predicate (strobe) here:
let unary_strobes        = Array.create g_mya_limit Enum_none // DP map from xbnn to a strobe group, if any.






let g_netbase_ss = Dictionary<string, net_att_t>()          // Nets by string id
let g_netbase_nn = Dictionary<xidx_t, net_att_t * further_net_att_t>()  // Nets by xidx_t


//
// Absolute bubble sort, eliding duplicates.
//
let abs_bubble lst =
    let rec pass = function
        | a::b::tt when a=b -> pass (b::tt)
        | a::b::tt when a = -b -> raise Invalid // sf "Invalid bubble sort - contrary terms"        
        | a::b::tt -> if (abs a > abs b) then b :: pass (a::tt) else a :: pass (b::tt)
        | other -> other
    let rec iterate lst =
        let lst' = pass lst
        if lst=lst' then lst'
        else iterate lst'
    iterate lst



(*
 * Some input forms generate very long net names, so we abbreviate them with following routine.*)


let prec2str (prec:precision_t) =
    let ss =
        match prec.widtho with
            | None -> "N"
            | Some -1 -> "StringHandle"
            // Some 0 -> "unspecified"
            | Some n -> i2s n
    sprintf "%A:%s" prec.signed ss



let g_abbreviations = System.Collections.Generic.Dictionary<string list, string>()
let g_abbreviations_rev = System.Collections.Generic.Dictionary<string, string list>()

let do_not_abbreviate s =
    let (found, ov) = g_abbreviations.TryGetValue s
    if not found then g_abbreviations.Add(s, htos s)
    elif ov = htos s then ()
    else sf ("Do not abbreviate error: " + htos s)


let htos_net = function
    | [ item ] -> item
    | [] -> "nullname??"
    | final::prefix ->
        let (found, ov) = g_abbreviations.TryGetValue prefix
        //vprintln 0  ( "+++ Prefix for " + htos prefix + " found = " + boolToStr found)
        let doit p = p + "_" + final
        if found then doit ov
        else
            let rec symb = function
                | ([], _) -> []
                | ([a], _) -> [a]
                | ([a; b], _) -> [a; b]
                | ((a::b::c::tt), n) -> if n=0 then (a::b::c::tt) else if n < 2 then [a;b] else [a]  
            let rec fl = function
                | h::t -> implode(symb(explode h, length t)) + (fl t)
                | [] -> ""

            let p = fl (rev prefix)         
            let (alreadyinuse, _) = g_abbreviations_rev.TryGetValue p
            //vprintln 0  ( "+++ p is >" + p + "<")
            let p1 = if alreadyinuse then funique p else p
            let ids1 = htos prefix
            if p1 <> ids1 then vprintln 2 (sprintf "    setting up abbreviation %s for prefix %s\n" p1 ids1)
            g_abbreviations.Add(prefix, p1)
            g_abbreviations_rev.Add(p1, prefix)
            doit p1

let m_interlocker = ref false

let lookup_net_by_string id = 
    let (found, nn) = g_netbase_ss.TryGetValue id
    if found then Some(X_bnet nn) else None

let lookup_net_by_xidx_o nn = 
    let (found, ov) = g_netbase_nn.TryGetValue nn
    if found then Some ov else None

let lookup_net2 nn = 
    let (found, ov) = g_netbase_nn.TryGetValue nn
    if found then snd ov else sf (sprintf "meox.lookup_net: xidx %i is not a net" nn)



let g_resetval = "init"

let nap_update lst key vale =
    let rec nap_update_serf = function
        | [] -> [ Nap(key, vale) ]
        | Nap(k, v)::tt when key=k -> Nap(key, vale)::tt
        | xother::tt -> xother::nap_update_serf tt
    nap_update_serf lst



let net_id = function
    | X_net (id, _) -> id
    | X_bnet ff     -> ff.id
    | other -> sf (sprintf "other form for net_id:  %A" other)


let nxt_it()  =  // Return the next natural number for sub-expression memoising.
    let nn = !mya_splay_nn
    mya_splay_nn := nn + mya_f_fieldwidth
    if nn >= g_mya_limit then failwith(sprintf "Too many memoisied heap expressions generated: %i/%i" nn g_mya_limit)
    nn

(*
 * Two-stage net definition API:
 *   1/ First, netsetup_start allocates a fresh net its own number 'n'.
 *   2/ Then user code is obliged to call netsetup_log.
 *)
let netsetup_start (id:string) = // Globally, all nets with the same name must have the same attributes.
    //if id = "testClock" then muddy "netsetup hitit"
    let (found, ff) = g_netbase_ss.TryGetValue id
    if !m_interlocker then hpr_yikes (sprintf "Previous netsetup not finished!  id=%s" id)
    let ans =
        if found then (ff.n, Some ff)
            //let (found, f2) = g_netbase_nn.TryGetValue ff.n
            //if found then (ff.n, Some (ff, f2)) else (ff.n, None)
        else
            m_interlocker := true
            let nn = nxt_it()
            (nn, None)
#if NNTRACE
    vprintln 0 ("netsetup_start " + id + " with nn=" + i2s(fst ans) + " ov=" + (if found then "Some" else "None"))
#endif
    ans

let sftToStr = function
    | Signed -> "Signed"
    | Unsigned -> "Unsigned"
    | FloatingPoint -> "FloatingPoint"

let array_lenfun len = if len = g_unspecified_array then "[<unspecified>]" else i2s64 len

let rec asubsLstToStr = function
    | [] -> ""
    | h::t when h=g_unspecified_array -> "[<unspecified>]" + asubsLstToStr t
    | h::t -> "[" + i2s64 h + "]" + asubsLstToStr t                     


let vartypeToStr = function
    | (V_VALUE)   -> "VALUE"
    | (V_EVENT)   -> "EVENT"
    | (V_MUTEX)   -> "MUTEX"
    | (V_ELAB s) -> "ELAB " + s
    | (_) -> "??"


let varmodeToStr = function
      | (INPUT)  -> "INPUT"
      | (OUTPUT) -> "OUTPUT"
      | (INOUT)  -> "INOUT"
      | (LOCAL)  -> "LOCAL"
      | (RETVAL) -> "RETVAL"

let rec atToStr = function
    | Nap(a,b)-> a+"=" + b
    | Napnode(a,lst)-> a + "={" + sfold atToStr lst + "}"       


// Second half of: this must be called immediately after netsetup_start without starting another new net.
let netsetup_log(ff:net_att_t, f2:further_net_att_t) =
    if ff.id = "" || abs (ff.n) < 2 then sf (sprintf "meox: illegal net name >%s< or no %i" ff.id ff.n)
    g_netbase_ss.Add(ff.id, ff)
    g_netbase_nn.Add(ff.n, (ff, f2)) 
    m_interlocker := false    
#if NNTRACE
    let detailsf = true
    let local_ff_summary = 
         (":netno=" + i2s ff.n + ":") + 
         //(if detailsf then ":" + varmodeToStr(f2.xnet_io) + ":" else "") +
         (if detailsf then ":" + sftToStr ff.signed else "") +
         //(if detailsf && ff.ats <> [] then "{"  + sfold atToStr f2.ats + "}" else "") +
         //(if detailsf then asubsLstToStr ff.length else "") 
         (if detailsf && ff.is_array then "[<...>]" else "") 
    vprintln 0 (sprintf "netsetup_complete %s " ff.id + local_ff_summary)
#endif
    //if ff.id = "i_heapManager00_port0_RESULT" then sf "Hit golden deep debug point"
    ff


//
// There should be only one net with a given name and it cannot be modifed after making (apart from strobe group (aka enum) closing).
// Actually, since hexp_t uses equality on xidx_t rather than built-in FSharp deep equality, changing a net's defintition is possible while having it still identify with its old self on the memoised heap. And now X_bnet holds just nn it is just bad practice.
let xgen_bnet (ff, f2) =
    let (found, ov) = g_netbase_nn.TryGetValue ff.n  // It would be more sensible/useful
    if found then
        let (ff, f2) = ov
            // paranoid!
        cassert(ff.width =   ff.width,   "net_retrieve changed width")
      //cassert(ov.xnet_io = ff.xnet_io, "net_retrieve changed xnet_io")
        cassert(ff.n =       ff.n,       "net_retrieve changed nn")
        cassert(ff.signed =  ff.signed,  "net_retrieve changed signed")
      //cassert(ov.ats =     ff.ats,     "net_retrieve changed ats")
    if not found then ignore(netsetup_log(ff, f2))
    X_bnet ff

let rec at_assoc id = function
    | Nap(s, v)::t when s=id -> Some v
    | _::t -> at_assoc id t
    | [] -> None

let rec atl_assoc id = function
    | Napnode(s, v)::t when s=id -> Some v
    | _::t -> atl_assoc id t
    | [] -> None

let g_atts_dp = new Dictionary<xidx_t, att_att_t list>()

let rec mine_atts cc0 (arg:hexp_t) =
    let nv =
        let nn = arg.x2nn
        let (found, ov) = g_atts_dp.TryGetValue nn
        if found then ov
        else
            let cc = []
            let nv = 
                match arg with
                  | X_blift _  // A boolean expression does not have any attributes?
                  | X_undef
                  | X_net _
                  | X_num _
                  | X_bnum _             -> cc
                  | X_bnet ff            -> (lookup_net2 ff.n).ats @ cc
                  | X_pair(l, _, _)      -> mine_atts cc l
                  | W_asubsc(l, _, _)    -> mine_atts cc l
                  | X_x(l, _, _)         -> mine_atts cc l
                  | W_query(_, l, r, _)  -> mine_atts (mine_atts cc r) l
                  | W_string _           -> singly_add (Nap("presentation", "char")) cc
                  | W_apply(_, _, lst, _)
                  // Unfortunately, the prec field of a cast cannot store the 'presentation=char' needed for dot net short/char disambiguation, so we plough on for now through the monadic W_node
                  | W_node(_, _, lst, _)  -> List.fold mine_atts cc lst    
                  | other ->
                      dev_println (sprintf "mine_atts other form: %A" other)
                      cc
            g_atts_dp.Add(nn, nv)
            nv
    lst_union cc0 nv

let rec mine_presentation arg = at_assoc "presentation" (mine_atts [] arg)


(*
// Old method of representing a read-only table (ROM).
// We also once used an aid begining with "ROM.." to denote these.
// Today we want to use constval field in net_att_t
let isROM_ = function
    | W_string(s1, XS_withval v1, _) ->
        match v1 with
            | X_bnet f ->
                let new_vale_ = f.constval
                match atl_assoc "constant" f.ats with
                    | None -> None
                    | Some rom_contents -> Some rom_contents
            | _ -> None    
    | _ -> None
*)   




let deMorganFlipOp = function
    | V_bitand -> V_bitor
    | V_bitor -> V_bitand
    | V_xor -> V_xor
    | other -> sf (sprintf "other operator deMorganFlipOp %A" other)


let netgen_start_fresh id (prec:precision_t) =
    let (n, ov) = netsetup_start id 
    match ov with 
        | Some ov -> sf (sprintf "cannot recreate net called '%s'.  meo supports only totally disjoint identifiers overall VM2 definitions (but not instances)" id)
        | None -> { g_null_net_att with id=id; n=n; signed=prec.signed; width=(valOf_or prec.widtho 32) }

let iogen_serf_ff(id, len, max, width, iot, signed, states, spint, ats, constval) =
    let (n, ov) = netsetup_start id 
    match ov with 
       | Some ff ->
           let f2 = lookup_net2 n
           assertf (not_nullp len = ff.is_array)     (fun() -> id + ": iogen_serf:  redefine differently: new len=" + sfold i2s64 len + " cf old=" + boolToStr ff.is_array)
           assertf (signed = ff.signed)  (fun() -> id + sprintf ": iogen_serf: redefine differently: new signedf=%A" signed) 
           assertf (width = ff.width)    (fun() -> sprintf "iogen_serf: redefine differently: new width %i instead of %i for %s " width ff.width id)
           //assertf(iot=f2.xnet_io)       (fun() -> id + ": iogen_serf: redefine differently new io setting " + varmodeToStr(ff.xnet_io) + " changed to " + varmodeToStr iot)
           // constval not checked
           (ff, f2)
        | None ->
              let ats = (if nonep(at_assoc g_resetval ats) then [Nap(g_resetval, "0")] else []) @ ats // The net has an initial value but is not a constant. TODO delete this old style for a variation on constval
              let vm = ref iot
              let vt = ref V_VALUE
              let e2 = 0
              let ff = 
                  { 
                    n=        n
                    id=       id
                    width=    width
                    constval= constval
                    signed=   signed
                    rh=        if spint then BigInteger(maxint()) else max
                    rl=        if not spint then 0I else if signed=Unsigned then 0I else -(BigInteger(maxint())+1I)
                    is_array= not_nullp len
                   }
              let f2 = 
                  { 
                    length=   len
                    dir=      false
                    pol=      false
                    xnet_io=  iot 
                    vtype=    !vt
                    ats=      ats
                  }
              let _ = netsetup_log (ff, f2)
              (ff, f2)


let iogen_serf arg = fst (iogen_serf_ff arg)


let xgen_gend uidtag (net_name, net_prec, net_dir, reset_val) =
    let ats = if nonep reset_val then [] else [Nap(g_resetval, i2s (valOf reset_val))]
    let net_name = if nonep uidtag then net_name else net_name + "_" + valOf uidtag
    xgen_bnet(iogen_serf_ff(net_name, [], 0I, (valOf net_prec.widtho), net_dir, net_prec.signed, None, false, ats, []))        

let netgen_serf_ff(id, len, max, width, signed) = iogen_serf_ff(id, len, max, width, LOCAL, signed, None, false, [], [])

let vectornet_m(id, max) = xgen_bnet(netgen_serf_ff(id, [], max, bound_log2 max, Unsigned))

let vectornet_w(id, width) = xgen_bnet(netgen_serf_ff(id, [], 0I, width, Unsigned))

let vectornet_w1 id width ats = xgen_bnet(iogen_serf_ff(id, [], 0I, width, LOCAL, Unsigned, None, false, ats, []))




(*
 * Create a net to hold something whose type is same as arg.  See restructure holding registers?
 *)
let autotypenet arg id = 
   let r = vectornet_w(id, 32)  (* For now *)
   vprintln 0 ( "+++ Using autotype crude form 32 for  " + id)
   r



// Make a net that ranges over a small fixed enumeration of constants.
// Of course, all upper-bounded ints can be held this way...

// Suppose v ranges over constants p,q,r.  Then there are one-hot tests P=(v==p), Q=(v==q) and R=(v==r) with consequent dont care
// conditions PQR = (-P,-Q,-R)  (P,Q), (Q,R), (R,P).  There is a quadratic number of don't cares associated with any enumeration.
// 
// so we have the following dont care sets 

// There's also a two-stage way of doing this ... ? TODO open/closed
// Enum nets need to be unsigned for diosim to work correctly - e.g. four members and set to number 3 will use 2 bits and 3 will read back as -1 if signed and fail to compare, in diosim, against 3, whereas the RTL generated is ok.
let create_enum_net(id, (typename, enum)) =
    let width = bound_log2 (BigInteger(length enum))
    let ats = []
    xgen_bnet(iogen_serf_ff(id, [], 0I, width, LOCAL, Unsigned, Some(typename, enum), false, ats, []))

let vectornet_ws(id, width, signed) = xgen_bnet(netgen_serf_ff(id, [], 0I, width, signed))

let ionet_w(id, width, io, signed, ats) = xgen_bnet(iogen_serf_ff(id, [], 0I, width, io, signed, None, false, ats, []))


let intnet(id, ats) = xgen_bnet(iogen_serf_ff(id, [], 0I, 0, LOCAL, Signed, None, true, ats, []))


let arraynet_w(id, len, width) = xgen_bnet(netgen_serf_ff(id, len, 0I, width, Unsigned))


let arraynet_ws(id, len, width, signed) = xgen_bnet(netgen_serf_ff(id, len, 0I, width, Signed))


let simplenet id = vectornet_w(id, 1)

let newnet_with_prec prec id =
    match prec.widtho with
        | Some wid ->     vectornet_ws(id, wid, prec.signed)
        | None -> sf("new_net_with_prec - null widtho not allowed when forming " + id)

    
type dpath_op_t = hexp_t * hexp_t * hbexp_t * hexp_t * hexp_t


type Dpath_t = { ops: dpath_op_t list ref;
                 id:string
               }
 



let hash_digest(a, b) =
    let k = (4096 * 4096)
    let rec z v = if v >= k then z(v-k) else v  
    (a*3 + b) % k


let xs_valOf = function
        | W_string(s, xs, _) -> xs
        |  _ -> sf "xs_valOf other"


// Strings are stored once and also strings with XW_withval form are used as enum constants and ROMs.
let stringbase = Dictionary<string * int, hexp_t>()

let rec xi_stringx xs ss =
    let key =
        match xs with
        | XS_withval v1 ->
            match x2nn v1 with
                //| None    -> 0 // XS_withval_strkey (sprintf "key-%A" v1)
                | nn -> nn

        | kk -> 0

    let (found, ov) = stringbase.TryGetValue((ss, key))
    if found then ov
    else
        let nn = nxt_it()
        let ans = W_string(ss, xs, { n=nn; sortorder=10000; (*hash__=h*) } )
        stringbase.Add((ss, key), ans)
#if NNTRACE
        vprintln 0 (sprintf "xi_stringx '%s':  allocate xidx_t n=%i" ss nn)
        match xs with
            | XS_withval v1 -> vprintln 0 (sprintf "string has XS_withval whose xidx_t is %A" (v1))
            | _             -> vprintln 0 "string has no XS_withval"
#endif
        ans


and xi_string ss = xi_stringx (XS_fill 0) ss

(*
 * x2nn gives the unique integer associated with an expression
 *)
and x2nn (hexp:hexp_t) = hexp.x2nn

and xb2nn (boolexp:hbexp_t) = boolexp.xb2nn // Will return -ve when inv flag set

let x2nn_str x =
    match x2nn x with
        //| None -> "none"
        | n -> i2s n
        
// All expressions have a sort order.  This is used to order the operands of commutative operators to approach a normal form.
let rec x2order = function
        | W_node(_, _, _, meo)   
        | W_asubsc(_, _, meo)      
        | W_apply(_, _, _, meo)       
        | W_lambda(_, _, _, _, meo)
        | W_string(_, _, meo)      
        | X_x(_, _, meo) 
        | W_valof(_, meo)           -> meo.sortorder

        | W_query(_, tt, _, meo)   ->  x2order tt  (* We normalise query trees to be right branching, sorted in order of lhs item *)
        | X_subsc(l, r) ->  muddy "l  (* Verilog-like I/O subsc only: deprecated for general use *)"
        | X_bnet ff -> ff.n          (* Sort in order of nn to bunch up operations on this net *)
        | X_pair(l, r, _) -> x2order r
        | X_blift b    -> xb2order b
        | X_net _      -> 1

        | X_bnum _ //  (* Constants have sort order zero, but they shold hash to diverse arbitrary values*)
        | X_num _ 
        | X_undef      -> 0
        | X_iodecl _   -> 0
        | X_quantif _  -> sf("x2order other quantif")
        | other -> sf("x2order other!")


and xb2order = function
        | W_cover(_, meo)
        | W_bnode(_, _, _, meo)
        | W_bdiop(_, _, _, meo) 
        | W_bitsel(_, _, _, meo)
        | W_linp(_, _, meo)
        | W_bsubsc(_, _, _, meo) -> meo.sortorder

        | W_bmux(gg, _, _, meo)  -> xb2order gg

        |   X_false  (* Constants should be 0 in ordering *)
        |   X_true 
        |   X_dontcare -> 0



// Verilog Operator Precedence:
//  4  unary  +,-,!,~ (unary) Highest
//  5   *, / %
//  6  +, - (binary)
//  7  <<. >>
//  8   <, < =, >, >=
//  9  =, ==. !=
//  9b  ===, !==
//  10  &, ~&
//  11  ^, ^~
//  12  |, ~|
//  13  &&
//  14  ||
//  15  ?: Lowest

// Wikipedia ordering for C/C++ (inverse polarity to ours)
//   4 .* ->* Pointer to member (C++ only) Left-to-right
//   5 * / % Multiplication, division, and modulus (remainder)
//   6 + - Addition and subtraction
//   7 << >> Bitwise left shift and right shift
//   8 < <= Relational “less than” and “less than or equal to”
//     > >= Relational “greater than” and “greater than or equal to”
//   9 == != Relational “equal to” and “not equal to”
//   10 & Bitwise AND
//   11 ^ Bitwise XOR (exclusive or)
//   12 | Bitwise OR (inclusive or)
//   13 && Logical AND
//   14 || Logical OR

let x_table =
    [
           (V_onesc,   "~", 11, "V_onesc", false);
           (V_neg,     "-", 11, "V_neg", false);        // 11-6 = 5
           (V_divide,  "/", 6, "V_divide", false);     // 11-5 = 6
           (V_times,   "*", 6, "V_times", true);       // 11-5 = 6
           (V_mod,     "%", 6, "V_mod", false);        // 11-5 = 6

           (V_exp,     "**",   5, "V_exp", false);
           (V_plus,    "+",   5, "V_plus", true);        // 11-6 = 5
           (V_minus,   "---",5, "V_minus", false);      // 11-6 = 5 // we should never see this

           (V_lshift,  "<<", 4, "V_lshift", false);    // 11-7 = 4

           // Arithmetic rshift
           (V_rshift Signed, ">>", 4, "V_rshift", false);    // 11-7 = 4
           // Logical rshift
           (V_rshift Unsigned, ">>>", 4, "V_rshift", false);    // 11-7 = 4 

           (V_bitand,  "&", 1, "V_bitand", true);      // 11-10= 1
           (V_xor,     "^",    0, "V_xor", true);         // 11-11 = 0
           (V_bitor,   "|", -1, "V_bitor", true);       // 11-12 =-1

           (V_interm, ":=", 0, "V_interm", true);
           (V_cast CS_preserve_represented_value, ":<-cvtcast-",  0, "V_cvtcast", true);
           (V_cast CS_maskcast,                   ":<-maskcast-", 0, "V_tmaskcast", true);
           (V_cast CS_typecast,                   ":<-typecast-", 0, "V_typecast", true);

      ]

let xToStr_dop a =
    let rec scan = function
        | [] -> None
        | (op, symb, prec, xml, assocf)::t when op=a -> Some(symb, (prec, assocf), xml)
        | _::t -> scan t 
    valOf(scan x_table)

    
let strToX_dop a =
    let rec scan = function
        | [] -> None
        | (op, symb, prec, xml, assocf)::t when xml=a -> Some(symb, prec, xml)
        | _::t -> scan t 
    scan x_table


let (* Boolean operators *)
   xabToStr_dop =
       function
           | (V_bor)  -> ("||", (0, true), "V_bor")
           | (V_band) -> ("&&", (1, true), "V_band")
           | (V_bxor) -> ("^",  (2, true), "V_bxor")


let xb_table =
    [
      (V_deqd, "==", 3, "V_deqd", Some "!=");  (* Must keep these precedence numbers in track with cpp_render too .. hmm *)
      (V_dned, "!=", 3, "V_dned", Some "==");
      (V_dltd Signed ,  "<", 3, "V_dltd",  Some ">=");
      (V_dltd Unsigned, "<", 3, "V_dltd",  Some ">=");      
      (V_dled Signed, "<=", 3, "V_dled", Some ">");
      (V_dled Unsigned, "<=", 3, "V_dled", Some ">");      
      (V_dgtd Signed, ">", 3, "V_dgtd",  Some "<=");
      (V_dgtd Unsigned, ">", 3, "V_dgtd",  Some "<=");      
      (V_dged Signed, ">=", 3, "V_dged", Some "<");
      (V_dged Unsigned, ">=", 3, "V_dged", Some "<");      
      (V_orred, "|-|", 11, "V_orred", None); // No negated symbol
     ]

let xbToStr_dop a =
    let rec scan =
        function
            | ((op, symb, prec, xml, nego)::t) -> if op=a then (symb, (prec, false), xml, nego) else scan t
    scan xb_table


let strToXb_dop a = // decode XML
    let rec scan = function
        | [] -> None 
        | ((op, symb, prec, xml, nego)::t) -> if xml=a then Some(op, symb, prec, xml, nego) else scan t            
    scan xb_table

// Shared resource info,
let sriToStr (idl, ats) = hptos idl

let rec xbkey = function
    | W_bnode(oo, lst, i, _) -> "bnode" + i2s(length lst)+ "." + (if i then "!" else "") + f1o3(xabToStr_dop oo)
    | X_true      -> "true"
    | X_false     -> "false"
    | W_bdiop(V_orred, [arg], i, _) -> (if i then "!" else "") + "bdiop.orred." + xkey arg
    | W_bdiop(oo, _, i, _) -> (if i then "!" else "") + "bdiop." + f3o4(xbToStr_dop oo)
    | W_bsubsc _  -> "bsubsc"
    | W_bitsel _  -> "bitsel"
    | W_cover(cubes, _)  -> sprintf "cover^%i" (length cubes)
    | W_bmux _    -> "bmux"
    | W_linp _    -> "linp"
    | X_dontcare  -> "X-dontcare"
    //| other     -> sprintf "xbkey???%A" other
        
and xkey = function
        | X_blift b -> "blift." + xbkey b 
        | X_num _   -> "num"
        | X_net _   -> "net"
        | X_bnet ff  ->
            //let f2 = lookup_net2 ff.n
            "bnet:" + (if not_nullp ff.constval then sprintf "const^%i:" (length ff.constval)  else "") + ff.id
        | X_x _ -> "x_x"
        | (X_psl_diadic _) -> "psl_diadic"
        | (X_psl_builtin _) -> "psl_builtin"
        | X_bnum(w, bn, _) -> "bnum" +  (if w.signed=Signed then "S" elif w.signed=Unsigned then "U" else "FP??")

        | W_string(s, md, meo) ->
            match md with
                | XS_sc sc      -> sfold mdToStr sc
                | XS_tailmark   -> "XS_tailmark"
                | XS_unit       -> "XS_unit"
                | XS_withval x  -> "XS_withval+" + xkey x   
                | XS_fill n     -> sprintf "XS_fill%i" n
                | XS_unquoted n -> sprintf "XS_unquoted%i" n
        | W_apply _ -> "apply"
        | X_subsc _ -> "subsc"
        | W_asubsc _ -> "asubsc"
        | W_node(prec, oo, l, m) ->   "W_node."  + f1o3(xToStr_dop oo)
        | W_query(g, a, b, _) -> "W_query"
        | (X_undef) -> "X_undef"
        | (X_pair _) -> "X_pair"
        | (_) -> "xkey?"


and mdToStr = function
    | Memdesc_sc n   -> sprintf "d%i" n
    | Memdesc_scs ss -> ss
    | Memdesc0(md:memdesc0_t) ->
        //let scs = sprintf "%c%i" memdesc0.f_sc_char memdesc0.f_sc
        let sc = sprintf "%c%i" md.f_sc_char md.f_sc
        let vim = (if not_nonep md.shared_resource_info then ":BANK=" + sriToStr (valOf md.shared_resource_info) else "") +
                  (if md.has_null then ":HAS_NULL_PTR" else " dtidl=" + htos md.uid.f_dtidl) +
                  sprintf " base=%i name=%s aka %s %s" md.uid.baser (htos md.uid.f_name) (htos_net md.uid.f_name) (if nonep md.uid.length then "" else sprintf "length=%i bytes" (valOf md.uid.length)) + (if md.vtno=0 then "" else sprintf " vtno=%i" md.vtno) 
        "MD0: " + sc + vim + (if nullp md.mats then "" else " {" + sfold atToStr md.mats + "}") + (if md.literalstring then ":LITERALSTRING" else "")

    | Memdesc_tie lst         -> sprintf "MD-ties{%s}" (sfold mdToStr lst)
    | Memdesc_ind (v, None)   -> sprintf "%s[-]" (mdToStr v)
    | Memdesc_ind (v, Some s) -> sprintf "%s[%s]" (mdToStr v) (mdToStr s)
    | Memdesc_via(v, tagger)  -> sprintf "%s->%s" (mdToStr v) (htos tagger)


let g_blank_memdesc0 =
  {
      literalstring=          false
      f_sc=                   0
      f_sc_char=              'x'
      uid=                    { f_name=["NULL_PTR"]; f_dtidl=[]; baser=0L; length=None }
      mats=                   []
      vtno=                   -1
      has_null=               false
      shared_resource_info=   None
  }

// This is the blank (aka null) memdesc but with the null flag set. We cannot use the terms null and default interchangeably owing to the has_null field.
let g_hasnull_memdesc0 = { g_blank_memdesc0 with has_null=true }

// oi64ToStr

(*
 * Operator binding precedence.  Do not edit lightly since these figures are also hardcoded in ... cpp_render and verilog_render and so on
 *)
let xb_op_binding = function
    | X_true 
    | X_false
    | X_dontcare -> (13, false)

    | W_bnode(_, _, true, _)   -> (13, false) // Inverted bnode needs parenthesis always
    | W_bnode(V_bor, _, _, _)  -> (-3, true) // 11-14=-3
    | W_bnode(V_band, _, _, _) -> (-2, true) // 11-13=-2
    | W_bnode(V_bxor, _, _, _) -> (-2, true)

    | W_bmux _ 
    | W_bitsel(_, _, _, _)
    | W_bsubsc(_, _, _, _) -> (12, false)
    | W_bdiop(oo, _, _, _) -> f2o4(xbToStr_dop oo)
    | W_linp(_, _, _)      -> (3, false) (* but not normally printed*)
    | W_cover _            -> (2, false) // Always printed with its own paren.

let x_op_binding = function
    | X_undef
    | X_num _
    | X_bnum _
    | X_net(_) 
    | W_string _
    | X_pair _ 
    | X_bnet(_) 
    | X_subsc(_, _) 
    | W_asubsc(_, _, _)
    | W_apply _
    | W_valof _
    | X_x(_, _, _) -> (13, false)
    
    | X_blift _
    | W_apply _ -> (11, false)
    | W_node(prec, oo, q, _) -> f2o3(xToStr_dop oo)
    
    | W_query(a, p, q, _) -> (11-15, false) //  = -4
    | X_quantif(p, q) -> (-4, false)
    
    | W_lambda _
    | X_iodecl _ -> (3, false)
    | other -> (sf("x_op_binding other: " + xkey other); (2, false))


// Get boolean literal number from an hbexp_t and
// store it in the the global array if not already present.
// new entry 
let deblit = function
    | 0 -> X_dontcare
    | 1 -> X_true
    | -1 -> X_false
    | n ->
        let nn = abs n
        let k = valOf_or_failf (fun ()->"bool literal was not set up " + i2s n) blits.[nn]
        if n>0 then k
        else
            match k with
                | W_bdiop(key, lst, false, meo) -> W_bdiop(key, lst, true, { meo with n=n })
                | W_bitsel(a, b, false, meo)    -> W_bitsel(a, b, true, { meo with n=n })
                | W_bsubsc(a, s, false, meo)    -> W_bsubsc(a, s, true, { meo with n=n }) 
                | W_bnode(key, lst, false, meo) -> W_bnode(key, lst, true, { meo with n=n })
                | W_linp(key, LIN(s, lst), meo) -> W_linp(key, LIN(not s, lst), { meo with n=n })
                | other -> sf (i2s n + " other " + xbkey other)

// Constant checks:
// We use the type system to split out boolean constants.
// Strings, ints and FPs can also be constant.
type net_constant_type_t =
    | Not_constant
    | Constant_int
    | Constant_string
    //| Constant_pair // see fconstantp for this for now ...
    | Constant_fp

let g_constantp_dp = new Dictionary<xidx_t, net_constant_type_t>()

let constant_class_resolve a = function
    | Not_constant -> Not_constant
    | b -> if a = Not_constant then Not_constant else b // Here we could possibly resolve a and b classes but just return b - most likely the same as a.

let rec bconstantp = function
          | X_true -> true
          | X_false -> true
          | W_cover(lst, _) ->
              let rec s2 = function
                  | []   -> true
                  | h::t -> bconstantp(deblit h) && s2 t
              let rec s1 = function
                  | []   -> true
                  | h::t -> s2 h && s1 t
              s1 lst
              
          | W_bitsel(x, _, _, _)       -> constantp x
          | W_bmux(gg, ff, tt, m)      -> bconstantp gg && bconstantp ff && bconstantp tt
          | W_bnode(oo, lst, inv, m)   -> conjunctionate bconstantp lst
          | W_linp(v, lst, meo)        -> false
          | W_bdiop(oo, lst, inv, meo) -> conjunctionate constantp lst
          | W_bsubsc(l, r, _, _)       -> constantp l && constantp r
          | X_dontcare                 -> false  // perhaps? can't call xi_manifest on it.  We should adsorb it in constant propagation arithmetic.
          //| k -> sf("match bconstantp " + xbkey k)

//
// Check whether an expression is constant and if so indicate its nature.
//
// We dont need dynamic programming on this.  Instead we trust that large constant expressions are never constructed owing to localised constant propagation.   
and classed_constantp (x:hexp_t) =
    let nn = x.x2nn
    let (found, ov) = g_constantp_dp.TryGetValue nn
    if found then ov
    else
    let ans =
       match x with
        | X_blift b -> if bconstantp b then Constant_int else Not_constant
        | X_undef -> Not_constant // can't call xi_manifest on this!
        
        | X_bnum _
        | X_num _ -> Constant_int

        | W_node(prec, V_cast cvtf,  [v], _) ->
            match classed_constantp v with // Perhaps now cast to a different constant class?
            | Not_constant -> Not_constant
            | _ -> if prec.signed = FloatingPoint then Constant_fp else Constant_int
            
        | W_string(_, XS_withval v, _) -> classed_constantp v
        | W_string _                   -> Constant_string 
        
        | W_query(g, a, b, _) ->
            if not (bconstantp g) then Not_constant else constant_class_resolve (classed_constantp a) (classed_constantp b)

        | W_node(prec, oo, hh::lst, m) -> foldl (fun(x,c)->constant_class_resolve c (classed_constantp x)) (classed_constantp hh) lst

        | X_bnet ff ->
            match ff.constval with
                | []                  -> Not_constant
                | (XC_string _ :: _)  -> Constant_string
                | (XC_double _ :: _)
                | (XC_float32 _ :: _) -> Constant_fp
                |  _                  -> Constant_int 

        | X_net(ss, meo) when ss = s_autoformat -> Constant_int // Silly special case

        // X_x of constants should never be formed
        
        | _ -> Not_constant (* actually, there are some other constants... *)
    //dev_println (sprintf "Constantp %i -> %A" x.x2nn ans)
    let _ = g_constantp_dp.Add(nn, ans)
    ans

and constantp x     = classed_constantp x <> Not_constant

let int_constantp x = classed_constantp x = Constant_int


let xi_bmanifest m x =
    match x with
    | X_true   -> true
    | X_false  -> false
    | other ->
        vprintln 0 (sprintf "arg=%A" other)
        sf (m + ": someone made a boolean constant expression: cant print here - use a later routine such as xbmonkey" )


// Delay operator: can refer to future and past values of an expression.
// Definitions: X_x(e, 0) === e
// Golden law a: "X(Q, 1)=d"    : ie. the next value of the Q output of a D-type is given by it's D input.
// Golden law b: "Q = X(d, -1)" : ie. we denote a registered/delayed value of an expression by adding a negative power of X.
// The laws a and b are equivalent, just divide or multiply both sides by X to move betweeb them.




// The Xblock form is not really needed: better to make commands such as IF accept command lists and a block is a 2-Handed if with one side null and the guard=X_true.
// Xskip then becomes [].  TODO change it. 
let rec gec_Xblock = function // Perhaps delete skips ...
    | [x]             -> x
    | []              -> Xskip
    | x::Xblock(y)::t -> gec_Xblock(x::y@t)
    | y               -> Xblock(y)

let gen_Xassign(l, r) = Xassign(l, r)
let gen_Xlabel s = Xlabel(s)
let gen_Xcomment s = Xcomment(s)
let gen_Xcomment1 s lst = gec_Xblock(gen_Xcomment s ::  lst)
let gen_Xreturn s = Xreturn(s)
let gen_Xgoto s = Xgoto(s, ref -1) // Value put in later by assembler.
let gen_Xbeq g s = Xbeq(g, s, ref -1) // Value put in later by assembler.
                   
let rec gen_Xpblock = function
          | [x] -> x
          | [ ] -> Xskip
          | x::Xpblock(y)::t  -> gen_Xpblock(x::y@t)
          | y   -> Xpblock(y)


// A strobe group (enumeration) is a variable that takes on a number of discrete values only.  The values are enumeration constants.  Such a constant can only be in one group so that we can infer the group from the constant.


let priv_getgrp site sgo vvn = // Find existing strobe group or allocate fresh one for variable vnn.
    match (sgo, g_strobe_groups.lookup vvn) with // (group from constant, group from expression/var being tested)
      | (Some strobegroup, None) ->
          g_strobe_groups.add vvn strobegroup
          strobegroup
      | (None, Some strobegroup) -> strobegroup
      | (Some sgl, Some sgr) when sgl=sgr -> sgl
      | (Some sgl, Some sgr) -> sf (sprintf "Expression and tag are in different strobe groups: cannot compare %i cf %i (vvn=%i)" sgl sgr vvn)
      | (None, None) ->
          let ans = !nextStrobegrp
          if !g_enum_loglevel>4 then vprintln 4 (sprintf " site=%s  - Allocate strobe group no %i for nn=%i" site ans vvn)
#if NNTRACE
          vprintln 0 (sprintf " site=%s  - Allocate strobe group no %i for nn=%i" site ans vvn)
#endif
          nextStrobegrp := ans + 1
          g_strobe_groups.add vvn ans
          ans

// Get the strobe group for a variable (that will be compared for equality against enum constants in that group).
let open_enum msg = function
    | X_bnet ff ->
        if !g_enum_loglevel>=4 then vprintln 4 (sprintf "Opening encoding group for nn=%i id=%s" ff.n ff.id)
        priv_getgrp msg None ff.n
    | arg ->
        vprintln 0 (msg + sprintf ": cannot open enum group for xkey=%s. xkey needs to be X_bnet. %s" (xkey arg) "cant-print")
        -1

// If an expression is a strobe group (aka an enum) we make it unsigned and have a bit width given by bound_log2 if closed.
let net_att_to_precision (ff:net_att_t) = 
    let vd = -1 // -1 for off
    let grp = g_strobe_groups.lookup ff.n
    let tailer ss = 
        let rr = if (ff.width)>0 then ff.width
                 elif ff.rh = BigInteger(maxint()) then 32
                 else bound_log2((ff.rh)-(ff.rl)+1I)
        if vd>=5 then vprintln 5 (sprintf "Encoding width tailer for %s is %i bits  ss=%A" ff.id rr ss)
        (rr, { g_default_prec with widtho=Some rr; signed= ss })

    let mag () = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "Using force unsigned on encoding group %s" ff.id)
    if nonep grp then tailer ff.signed
        else
            match g_strobe_group_members.lookup (valOf grp) with
                | Some (ovi, ovl, basis, Some(closer_, encoding_width)) ->
                    let _ = mag()
                    (encoding_width, { g_default_prec with  widtho=Some encoding_width; signed=Unsigned })
                | Some (ovi, ovl, basis, None) ->
                    let _ = mag()
                    tailer ff.signed // We cannot go unsigned before closing since every equality comparison against a constant creates an effect.
                     //Wrong: encoding_width_f xpc10 (closed strobe group) dwidth=5 big=32I ovi=[155; 158; 161; 164; 167; 170; 173]
                | None -> tailer ff.signed

// All bools have width 1
let encoding_bwidth (_) = 1

// See if a power of two: the standard 'trick' is (x) & (x-1) is zero only for zero and powers of 2.
let exact_power_of_two (bn:BigInteger) = 
    let vv = (bn) &&& (bn-1I)
    (vv = 0I)

let rec encoding_width arg =
    let ewbn (bn:BigInteger) = 
        let rec qq n (bn:BigInteger) = if bn=0I then n else qq (n+1) (bn / 2I) // Could check for power-of-two in this scan.
        if bn.Sign <0 then
            let pval = (0I - bn)
            let dextra =
                if exact_power_of_two pval then 0 // All other number have a swell when complemented.
                else 1 
            let ans = dextra + qq 0 pval //Hand for negative constants
            //dev_println(sprintf "swell %i for bn width check of %A (%A) giving %i" dextra bn pval ans)
            ans
        else qq 0 bn // Hand for positive constants.
    match arg with
    | X_bnet ff         -> fst (net_att_to_precision ff)
    | X_num n           -> ewbn (BigInteger n)
    | X_bnum(w, bn, _)  -> ewbn bn
    | W_asubsc(e, _, _) -> encoding_width e
    | X_blift e         -> encoding_bwidth e
    | other -> sf(sprintf "(please call ewidth instead) encoding_width: other:%A" other (* + xToStr other *) )


// Compose two precision specifications into one.
// When force_l_prec holds we give precedence to the lhs.
// Otherwise we should follow C-like semantics, when precisions are equal that is the result, else we return signed or unsigned int 32 or 64 with an unsigned operand taking precedence in the unsignedness of the result.  Note that 1<<1u is therefore unsigned in return value.
// Previously we did not integer_promote to 64/32 bits but now we do.
// The usual arithmetic conversions (C language specification, 6.3.1.8)
let precision_compose force_l_prec (l:precision_t) (r:precision_t) =
    if l.signed=FloatingPoint || r.signed=FloatingPoint then
        { g_default_prec with
            widtho= maxo l.widtho r.widtho; 
            signed= FloatingPoint;
        }
    else
        let disable_promotion = false // Always off now until custom-widthe floating point reintroduced.
        let mt32 x = not_nonep x && valOf x > 32
        let mt64 x = not_nonep x && valOf x > 64
        let lprec = force_l_prec || nonep r.widtho || (not_nonep l.widtho && valOf l.widtho > valOf r.widtho)
        let rprec = (not force_l_prec) && (nonep l.widtho || (not_nonep r.widtho && valOf l.widtho < valOf r.widtho))
        { g_default_prec with
            widtho=
              if disable_promotion then maxo l.widtho r.widtho; 
              elif mt64 l.widtho || mt64 r.widtho then maxo l.widtho r.widtho
              elif mt32 l.widtho || mt32 r.widtho then Some 64
              else Some 32
            signed= if lprec then l.signed
                    elif rprec then r.signed
                    elif l.signed=Unsigned || r.signed=Unsigned then Unsigned // C semantics: either operand to be unsigned for result be unsigned. (Verilog requires both signed to be signed which is the same behaviour, despite the default signnedness of expressions being different between the two languages).
                    else Signed
        }


let liststates1 =
    function
        | l -> ""
(*        |   liststates1 [(tag, encodingval)] = tag
        |   liststates1 ((h, _)::t) = h + ", " + liststates1 t
*)


let liststates (typename, l) = liststates1 l

let varmode_is_io = function
      | (INPUT) -> true
      | (OUTPUT) -> true
      | (INOUT) ->  true
      | (LOCAL) -> false
      | (RETVAL) ->
          let _ = vprintln 3 "varmode_is_io of RETVAL"
          true




// Swap inputs for outputs if we are instantiating a mirrored interface.
let reverse_varmode = function
      | (INPUT) -> OUTPUT
      | (OUTPUT) -> INPUT
      | (INOUT) -> INOUT
      | (LOCAL) -> LOCAL
      | (RETVAL) -> sf "reverse_varmode applied to RETVAL"

let hexpt_is_io = function
    | X_bnet ff -> varmode_is_io (lookup_net2 ff.n).xnet_io
    | _ -> false


let rec xi_onezerop = function // cf and intergrate xi_iszero ?
    | X_blift a        -> bconstantp a
    | X_num n          -> n=0 || n=1
    | X_bnum(w, bn, _) -> bn.IsZero || bn.IsOne
    | other            -> false

// Return a bool option for constant expressions saying whether they are non zero, or None if cannot find a constant.
let rec xi_monkey = function
    | X_pair(l, r, _) -> xi_monkey r
    | X_bnet ff ->
        match ff.constval with
            | [] -> None
            | XC_float32(sp) ::_ -> Some (sp <> 0.0f)
            | XC_double(dp) ::_  -> Some (dp <> 0.0)
            | XC_bnum(_, bn, _)::_ -> Some (bn<>0I)
            | _                  -> None
    | X_blift a        -> xbmonkey a
    | X_num n          -> Some (n<>0)
    | X_bnum(w, bn, _) -> Some(not bn.IsZero)
    | other          -> None

and xbmonkey = function
    | W_bdiop(V_orred, [a], false, _) -> xi_monkey a
    | X_false -> Some(false)
    | X_true -> Some true
    |  other -> None

let xi_iszero e = (xi_monkey e) = Some false

let xi_isnonz e = not (xi_iszero e)

let xi_isfalse b = (xbmonkey b) = Some false

let xi_istrue b = (xbmonkey b) = Some true

let xi_isundef = function
    | X_undef -> true
    | _       -> false



let gec_Xif gg a b =
    if a=Xskip && b=Xskip then Xskip
    else
        match xbmonkey gg with
            | Some true  -> a
            | Some false -> b
            | _          -> Xif(gg, a, b)


let gec_Xif1 gg ct =
    if ct = Xskip then Xskip
    else
        match xbmonkey gg with
            | Some true  -> ct
            | Some false -> Xskip
            | _          -> gec_Xif gg ct Xskip


let annotateToStr(v:annotate_t) =
  (if nonep v.palias then "" else "palias->" + (muddy "playToStr(valOf(v.palias)))")
   ) +
   "atts=[" +
   sfold (fun(a,b)->a+ "=" + b + " ") (v.atts) +
   "]"

let annToStr = function
            | (None) -> "{}"
            | (Some s) -> "{" + annotateToStr s + "}"


let bignum_SPLAY    = Dictionary<precision_t * BigInteger, hexp_t>();

// let g_prec_dp = new Dictionary<xidx_t, att_att_t list>()

//
// A precision is a structure (signed/unsigned/fp * width).
// mine_precision will return the precision of an hexp_t.
// ov can be supplied as the dfault via mine_prec.
// TODO: please say whether the ewidth function is presumably subsumed by this?            
let rec mine_precision bounding_modef ov arg =
    let rec scan cc = function
        // Note: Floating-point constants are stored in the X_bnet/XC_float32,_double forms.
        | X_x(ff, _, _)
        | X_pair(_, ff, _) -> scan cc ff // The lhs is an annotation for storage class or side-effecting cmd.  The rhs is whats returned.
        | W_node(caster, V_cast cvtf,  [arg], _) ->
            match caster.widtho with
                | None -> { scan cc arg with signed=caster.signed } : precision_t
                | Some w -> caster  // For no good reason this is happy to void the scan when widtho is null.
        | W_node(prec, _, lst, _) -> prec
        | W_asubsc(l, _, _) -> scan cc l
        | X_bnet ff -> precision_compose false cc (snd (net_att_to_precision ff))
        | W_query(g, l, r, _) -> scan (scan cc l) r
        | X_undef   -> cc
        | X_blift _ -> g_bool_prec

        | X_bnum(prec, bn, _) ->
            let nv =
                if bounding_modef && nonep prec.widtho then
                    let width = bound_log2 bn
                    { prec with widtho=Some width }
                else prec
            precision_compose false cc nv
        | X_num nn ->
            // Can potentially call bound_log2 for numeric constants but this would be rather vague. A better convention seems to be to assume 32 bits for X_num or when no precision is found            if bounding_modef && nonep prec.widtho then
            if bounding_modef then
                let width = bound_log2 (BigInteger nn)
                precision_compose false cc { widtho=Some width; signed=Signed }
            else cc

        | W_string(s1, XS_withval v1, _) -> scan cc v1             
        | W_string _ -> cc

        | W_apply((a, gis), _, l, _) -> precision_compose false cc gis.rv

        | X_net _ -> cc // X_net "str12" or similar crops up
            
        | other ->
            vprintln 0 (sprintf "+++Ignored other operator in mine_precision key=%s  expr=%s" (xkey other) (xToStr other))
            cc
    scan ov arg

// 
and infer_prec bounding_modef ll rr = combine_prec (mine_prec  bounding_modef ll) (mine_prec  bounding_modef rr)

        
and combine_prec (pp:precision_t) (qq:precision_t) = (precision_compose false pp qq):precision_t

and mine_prec bounding_modef arg = mine_precision bounding_modef g_default_prec arg

and mine_prec_no_float arg = // Supress floating point (for when packing and unpackings as unsigned integers with shifts and masks)
    let ans0 = mine_precision false g_default_prec arg 
    if ans0.signed=FloatingPoint then { ans0 with signed=Unsigned } else ans0

and infer_prec_no_float ll rr =
    let ans0 = infer_prec g_bounda ll rr
    if ans0.signed=FloatingPoint then { ans0 with signed=Unsigned } else ans0

    
// and playToStr (id, args) = id + "(" + xToStrList args + ")"



and romrenderx constval =
    let ritem = function
        | XC_string ss      -> ss
        | XC_bnum(w, bn, _) -> sprintf "%A" bn
        | XC_double db      -> sprintf "%A" db
        | XC_float32 fp     -> sprintf "%A" fp
        //| other -> sprintf "%A" other
        
    match constval with
        | [] -> ""
        | [item] -> sprintf "{ConstantScalar=%s}" (ritem item)
        | items -> sprintf "{ConstantROM^%i=%s} "  (length items) (sfold_ellipsis 7 ritem items)

and romrender ff = romrenderx ff.constval


and net_ffToStr detailsf (ff:net_att_t) =
    let grp = g_strobe_groups.lookup ff.n
    let pp = 
        if detailsf=false || ff.width = 1 || (ff.rl=0I && ff.rh=1I) then ""
        else 
            let i1 = 
                if ff.rh > 0I then (sprintf "[%A..%A]" ff.rl ff.rh)
                elif ff.width = -1 then ":StringHandle" // Aka (char *)
                elif ff.width = -2 then "(void *)"
                else ("[" + i2s(ff.width-1) + ":0]")
            let i2 = 
#if NNTRACE
                (":netno=" + i2s ff.n + ":") + 
#endif
                (if nonep grp || not detailsf then "" else sprintf ":EG%i" (valOf grp))

            let further = 
                let (found, ov) = g_netbase_nn.TryGetValue ff.n
                if found then
                    let f2 = snd ov
                    (if detailsf then ":" + varmodeToStr f2.xnet_io + ":" else "") +
                    (if detailsf && f2.ats <> [] then "{"  + sfold atToStr f2.ats + "}" else "") +
                    (if detailsf then asubsLstToStr f2.length else "")
                else (if ff.is_array then "[<...>]" else "<NoF2>")

            i1 + i2 + further +
            (if detailsf then ":" + sftToStr ff.signed else "") +
//          (if detailsf && ff.states <> None then ("{" + liststates(valOf(ff.states)) + "}") else "") +
            (if detailsf && not_nullp ff.constval then romrenderx ff.constval else "")
    ff.id + pp

and netToStr1 nt detailsf x =
    match x with
        | X_net(ss, _) ->
            if g_meo_vd >= 5 then vprintln 5 (sprintf ": netToStr: %i Ans so far " x.x2nn + ss)
            (if detailsf then "N:" else "") + ss

        | X_bnet ff ->
            let ans = net_ffToStr detailsf ff
            if g_meo_vd >= 5 then vprintln 5 (sprintf ": netToStr: %i Ans so far " ff.n + ans)
            ans

        | other -> xToStr_n nt other

and xoToStr = function
    | None -> "None"
    | Some x -> xToStr_n (ref !g_printing_limit) x

    
and xToStr arg = xToStr_ntr (ref !g_printing_limit) "TOP" arg

and xbToStr arg = xbToStr_n (ref !g_printing_limit) arg

and xToStr_list1 nt ff x =
    match x with
        | [] -> ""
        | h::t -> (if (ff) then "" else ", ") + xToStr_n nt h + (xToStr_list1 nt false t)

and xToStr_list nt v = xToStr_list1 nt true v

and xToStr_commaList n = function
        | [] -> ""
        | [item] -> xToStr_n n item
        | (h::t) -> xToStr_n n h + ", " + xToStr_commaList n t


and chklt((son_bp, son_assocf), (bp, assocf)) =
    if (son_bp < bp) then true
    elif (son_bp = bp) then (not son_assocf)||(not assocf)
    else false

and xbToStr_diadic_helper(p, q, fa, ff, os, i) = 
    let hl = fa p
    let hr = fa q
    in 
          (if (hl < i) then "(" +  ff p + ")" else ff p) +
          os + 
          (if (hr < i) then "(" +  ff q + ")" else ff q)

and
   xToStr_diadic_helper(p, q, fa, ff, os, i) =  (* These are both the same! but ml needs this! *)
        let hl = fa p
        let hr = fa q
        in 
          (if (hl < i) then "(" +  ff p + ")" else ff p) +
          os + 
          (if (hr < i) then "(" +  ff q + ")" else ff q)


and xToStr_diadic_helper_lst(lst, fa, ff, os, i) =  (* These are both the same! but ml needs this! *)
       let s p = if chklt(fa p, i) then "(" +  ff p + ")" else ff p
       let rec g = function
           | [] -> sf "[] xToStr list"
           | [item] -> s item
           | h::t -> (s h) + os + g t
       if length lst = 1 then ff (hd lst) else g lst


and xbToStr_diadic_helper_lst(lst, fa, ff, os, i) =  (* These are both the same! but ml needs this! *)
       let s p = if chklt(fa p, i) then "(" +  ff p + ")" else ff p
       let rec g = function
           | [] -> sf "[] xToStr list"
           | [item] -> s item
           | h::t -> (s h) + os + g t
       if length lst = 1 then ff (hd lst) else g lst

and xbToStr_n n b = xbiToStr n false b

and xbiToStr nt flip b =
    if (nt := !nt - 1; !nt) <= 0 then "..."
    else
    let pp = true // enable bbd nice printer.
    match b with
        | W_cover(cubes, _) ->
            let rec ssfold ff = function
                | [] -> ""
                | [item] -> ff item
                | h::tt ->
                    let s1 = (ff h) + "; "
                    if !nt <= 0 then s1 + "... ..."
                    else s1 + ssfold ff tt

            let xi2s n =
                if n = -1 then "X_false" // Put the X_ prefix in here to indicate a malformed (minimisable) cube.
                elif n = 1 then "X_true"
                else
                let v =
                    match blits.[abs n] with
                        | None -> sf("boolean literal not set up " + i2s n)
                        | Some v -> v
                //let neg g = if (fst (xb_op_binding v) < 11) then "!(" + g + ")" else "!" + g
                let s = xbiToStr nt (n < 0) v
                let s = if !g_litp_tagging then i2s n + "/" + s else s
                s
            let cubeToStr x = "[" + sfold xi2s x + "]"
            let s = "{" +  ssfold cubeToStr cubes + "}"
            (if flip then sprintf "!(%s)" s else s)

        | W_bmux(v, X_false, X_true, _)   -> xbiToStr nt flip v
        | W_bmux(v, X_true, X_false, meo) -> xbiToStr nt (not flip) v


        |  W_bmux(v, X_false, conj, _) when pp -> 
            let (os, i, _) = xabToStr_dop V_band  
            let s = (*"$(" + *)xbToStr_diadic_helper_lst([v; conj], xb_op_binding, xbToStr_n nt, os, i) (*+ "$)"*) 
            (if flip then sprintf "!(%s)" s else s)

        |  W_bmux(v, disj, X_true, _) when pp -> 
            let (os, i, _) = xabToStr_dop V_bor  
            let s = xbToStr_diadic_helper_lst([v; disj], xb_op_binding, xbToStr_n nt, os, i)
            (if flip then sprintf "!(%s)" s else s)

        | W_bmux(v, ff, tt, meo) -> "bmux(" + xbToStr_n nt v + "," + xbiToStr nt flip ff + "," + xbiToStr nt flip tt + ")"
            
        | W_bnode(oo, lst, inv, meo) ->
            (
                let (os, i, _) = xabToStr_dop oo
                let ans = xbToStr_diadic_helper_lst(lst, xb_op_binding, xbToStr_n nt, os, i)  + (if !g_litp_tagging then "/" + i2s meo.n else "")
                if inv <> flip then "!(" + ans + ")" else ans 
            )

            // Shorter printing of common pattern
        | W_bdiop(V_orred, [X_bnet ff], inv, _) -> if inv then "!" + ff.id else sprintf "|%s|" ff.id

        | W_bdiop(V_orred, [v], false, _) ->
            let chk_op_binding((q, assocf), r) = q<r || (q=r && (not assocf))
            let s = if chk_op_binding(x_op_binding v, 11) then "|-|(" + xToStr_ntr nt "VVORRED1" v + ")" else "|-|" + xToStr_ntr nt "VVORRED2" v
            if flip then sprintf "!(%s)" s else s
            
        | W_bdiop(oo, lst, inv, meo) ->
            let (pos_symbol, i, _, nego) = xbToStr_dop oo
            let (symbol, inv) = if nonep nego then (pos_symbol, flip<>inv)
                                elif (flip = inv) then (pos_symbol, false) else (valOf nego, false)
            let fuf v =
                let k = x2nn v
                xkey v + " /" + i2s k // (if k=None then "-" else i2s (valOf k))
            let debugs() = sfold fuf lst
            let ans = xToStr_diadic_helper_lst(lst, x_op_binding, xToStr_n nt, symbol, i) + (if !g_litp_tagging then "/" + i2s meo.n else "")
            let ans =
                match oo with
                    //| V_dled _
                    //| V_dltd _ -> ans + sprintf ":dltd:L=%A:dltd:R=%A:" (mine_prec (hd lst)) (mine_prec(cadr lst))
                    | _ -> ans
             //(if !g_litp_tagging then "{" + debugs() + "}" else "")
            if inv then "!(" + ans + ")" else ans
         
        | X_true  -> if flip then "false" else "true"
        | X_false -> if flip then "true" else "false"
        | X_dontcare -> "X"


        | W_bsubsc(k, l, inv, _) ->     
            let ans =  xToStr_n nt k + "[" + xToStr_ntr nt "bsubsc" l + "]"
            if flip<>inv then "!(" + ans + ")" else ans

        | W_linp(v, l, w) ->
            let s = "linp(" + xToStr_n nt v + "," + linpToStr l + ")"
            (if flip then sprintf "!" + s else s)

        | W_bitsel(v, b, inv, _) ->
            let s = "XSEL." + xToStr_n nt v + " [" + i2s b + "]"
            (if flip <> inv then "!" + s else s)

        | other -> sf("xbToStr other:" + xbkey other + " " + xbToStr other)




and linpToStr = function
    | LIN(true, [])  -> "TRUE"
    | LIN(false, []) -> ""

    | LIN(true, [n0]) -> "<" + i2s n0
    | LIN(false, [n0]) -> ">=" + i2s n0    
    | LIN(false, [n0;n1]) when n1=n0+1 -> "==" + i2s n0
    | LIN(true,  [n0;n1]) when n1=n0+1 -> "!=" + i2s n0    

    | LIN(true,  [n0;n1]) -> "<" + i2s n0 + "||>=" + i2s n1
    | LIN(false, n0::n1::tt) -> ">=" + i2s n0 + "&&<" + i2s n1 + linpToStr (LIN(false, tt))    

    | LIN(false, a::tt) -> ">=" + i2s a + linpToStr (LIN(true, tt))
    | LIN(true,  a::tt) -> "<"  + i2s a + linpToStr (LIN(false, tt))    
    

and baseup_string n (s0:string) = // Present a string literal whose base has potentially been bumped
    if n = 0 then s0
    else
        let ll = strlen s0
        if n <0 || n> ll then sprintf "bad-bumped-string(%s, %i/%i)" s0 n ll
        else s0.[n..]

and fgisToStr (fname, gis) =
    let fus = // If provided by a functional unit, give the kind name.  
        match gis.is_fu with
            | None -> ""
            | Some(block, None) -> sprintf "<%s>." (hptos block.kind)
            | Some(block, Some methoder) -> sprintf "<%s>.%s" (hptos block.kind) methoder
    fus  +
    (if nonep gis.overload_suffix then "" else sprintf "{%s}" (valOf gis.overload_suffix)) +
    fname

    
and cfToStr cf =
    let p1 = (if cf.externally_instantiated_block then "(EEB)" else "")
    let p2 =
        match cf.tlm_call with
            | None -> ""
            | Some true -> ".b."
            | Some false -> ".nb."
    p1 + p2
            
and xToStr_n nt arg =
    if (nt := !nt - 1; !nt) <= 0 then "..."
    else
    match arg with
    //| W_asubsc(k, subs, _)   -> sprintf  "asubsc.%i.%s[%s]" (x2nn arg) (xToStr_ntr nt "asubsc-base" k) (xToStr_ntr nt "asubsc-subs" subs)

    | W_asubsc(k, subs, meo)   -> sprintf  "%s[%s]" (xToStr_ntr nt "asubsc-base" k) (xToStr_ntr nt "asubsc-subs" subs)
    //| W_asubsc(k, subs, meo)   -> sprintf  "%s(nn=%i)[%s]" (xToStr_ntr nt "asubsc-base" k) meo.n (xToStr_ntr nt "asubsc-subs" subs)
    | X_pair(l, r, _) -> "{" + xToStr_ntr nt "pair-l" l + "," + xToStr_ntr nt "pair-r" r + "}"
    | W_apply((fname, gis), cf, args, meo) ->
        match cf.tlm_call with
            | Some blockingf when length args >= 1 ->
                xToStr_ntr nt "method call" (hd args) + "." + sprintf "APPLY%s" (cfToStr cf) + (fgisToStr (fname, gis)) + "(" +  xToStr_commaList nt (tl args) + ")"
            | _ -> sprintf "*APPLY%s:" (cfToStr cf) + (fgisToStr (fname, gis)) + "(" +  xToStr_commaList nt args + ")"
    | X_quantif(V_univ, r)   -> "\\A." + xbToStr_n nt r
    | X_quantif(V_exists, r) -> "\\E." + xbToStr_n nt r
    | W_lambda(args, body, spawned, atts, _) -> "lambda(" +(*  xToStr_list args +*) "){...}spawned="+ i2s(length spawned) + "[" + sfold (fun(a,b)->a+"=" + b + " ") atts + "]" 

    | W_valof(lst, _) -> "valof(" + hbevToStr(Xblock lst) + ")"
    | W_string(s, XS_tailmark, _) -> "x_tailmark"
    | W_string(s, XS_unit, _)     -> "x_unit"
    | W_string(s0, XS_unquoted n, _)              -> baseup_string n s0
    | W_string(s0, XS_fill n, _)                  -> sprintf "\"%s\"" (baseup_string n s0)    // Normal, quoted.

    | W_string(s0, XS_sc sc, _)                   -> sprintf "SC:%s" s0     // Storage class.
    | W_string(s0, XS_withval_strkey ss, _)       -> sprintf "XSS%s:\"%s\"" ss s0
    | W_string(s0, XS_withval_key n, _)           -> sprintf "XSK%i:\"%s\"" n  s0
    | W_string(s0, XS_withval x, _)               -> sprintf "X%s:\"%s\"" (xToStr_ntr nt "string-withval" x) s0
    | W_string(s0, xs, _)                         -> sprintf "XS?%A:\"%s\"" xs s0
    | X_num n  -> i2s n
    | X_net(f, m)  -> netToStr1 nt false (X_net(f, m))
    | X_bnet f -> netToStr1 nt false (X_bnet f)
    | X_x(p, n, _) when n=1 -> "X(" + xToStr(p) + ")"
    | X_x(p, n, _) when n=0 -> "tag(" + xToStr(p) + ")"
    | X_x(p, power, _) -> sprintf "Xn^%i(%s)" power (xToStr_ntr nt "power" p)
    | X_blift e -> xbToStr e 
    | W_query(g, t, f, _) -> "COND(" + (* i2s((xb2nn  g)) + ":" + *) xbToStr_n nt g + ", " + xToStr_ntr nt "VVQ1" t + ", " + xToStr_ntr nt "VVQ2" f + ")"
    | X_psl_builtin(F, x) -> F + "(" + xToStr_ntr nt "pls" x + ")"
        // |  (W_node(prec, xk, l, m)) -> "XNODE(" + f1o3(xToStr_dop xk) + "," + sfold xToStr_nt nt "psl2" l + ")"

    | W_node(caster, V_cast cvtf, [arg], _) ->
        // When the width cast to is 32 we do not display it.  Hence convert to float32 is displayed as Cf and to double as Cf64
        let kq = if caster.widtho=None then "?" elif valOf caster.widtho=32 then "" else i2s(valOf caster.widtho)
        let kw = if caster.signed=Signed then "" elif caster.signed=FloatingPoint then "f" else "u"
        let token = sprintf "C%s%s" kq kw
        let token = if cvtf=CS_maskcast then token
                    elif cvtf=CS_typecast then sprintf "(%s)" token // Cast in paren is typecast whereas without paren it is a conversion.
                    else sprintf "CVT(%s)" token // A full convert - tantamount to a function application infact.
        sprintf "%s(%s)" token (xToStr_ntr nt "node-1" arg) 

    | W_node(prec, oo, [v], _) when oo=V_neg || oo=V_onesc ->
        let (ops, binding, _) = xToStr_dop oo 
        if chklt(x_op_binding v, binding) then ops + "(" + xToStr_ntr nt "VVNOT1" v + ")" else ops + xToStr_ntr nt "VVNOT2" v
        
    | W_node(prec, oo, lst, meo)  ->
        //let xs = if oo = V_divide then sprintf "DIVIDEKEYSn=%i=" meo.n + sfold xToStr_ntr nt "node" (tl lst) else ""
        let (os, i, _) = xToStr_dop oo
        let ans = xToStr_diadic_helper_lst(lst, x_op_binding, xToStr_ntr nt "wnode", os, i)
        (*prec2str prec + ":" +*)
        ans

    | X_iodecl(id, hi, low, w) -> "x_iodecl(" + id + "," + i2s(hi) + "," + i2s(low) + varmodeToStr((lookup_net2 w.n).xnet_io) + ")"

    | X_subsc(a, n) -> (xToStr_ntr nt "subsc-l" a) + "[" + (xToStr_ntr nt "subsc-r" n) + "]"

#if TNUM
    |  (X_tnum []) -> "$tnum0" 
    |  (X_tnum lst) -> 
              (    
                  let rec k =
                    function
                    | (n::v) -> k v + "_" + hex3 n
                    | [] ->  "+"
                  if false then tnumToDec lst + "/" + k lst else tnumToDec lst 
              )
#endif
    | X_bnum(w, bn, _) ->
        let ss = if w.signed=Signed then "S" elif w.signed=Unsigned then "U" else "??"
        if nonep w.widtho then sprintf "%s'%A" ss bn
        else sprintf "%s%i'%A" ss (valOf w.widtho) bn
    | X_bnum(w, bn, _) -> System.Convert.ToString(bn)    
    | X_undef -> "*UNDEF"
    | other -> sf("xToStr other: " + xkey other)

and hex3 (n:uint32) =
    let i2xd (n:uint32) =
        let n = n  &&& 15u
        string(if n >= 10u then 'A' + char(n-10u) else '0' + char n)
    i2xd (n/256u) + i2xd (n/16u) + i2xd n
    //+ sprintf " (@%x) " n

and wideToX_simple digits = // Hexadecimal printout of a base 4096 number
    "[" + sfold hex3 digits + "]"

and wideToX_signed lower_casef negf digits0 = // Hexadecimal printout of a base 4096 number
    let i2xdf cc v = 
        let i2xd (n:uint32) =
            let n = n  &&& 15u
            string(if n >= 10u then (if lower_casef then 'a' else 'A') + char(n-10u) else '0' + char n)
        i2xd (v/256u)  :: i2xd (v/16u) :: i2xd v :: cc

    let digits1 = List.fold i2xdf [] digits0 // Convert to forward digit list - digits0 are backwards, ls-word first. 
    // For leading zero supression we need digits1 to be forward acting.
    let rec hex3lzs m = function
        | [] when m=0        -> if negf then (if lower_casef then "f" else "F") else "0" // Print a single 0 or F for the special case.
        | []                 -> ""
        | "0" :: tt when (not negf) && m=0 -> hex3lzs 0 tt
        | "f" :: tt when negf       && m=0 -> hex3lzs 0 tt
        | "F" :: tt when negf       && m=0 -> hex3lzs 0 tt        
        | n ::tt             -> n + hex3lzs 1 tt
    hex3lzs 0 digits1

and tnumToX = tnumToX_lc true // Default to lower case hex printing.

#if UNDEF
and xtoStrHex = function
    | X_bnum(w, bn, _) -> unfinished
#endif


and gec_X_bnum(prec, bn) = // If signed 32 bit precision and within the g_small_integer_pos_bound range, we could return an X_num instead?
    let kk = (prec, bn)
    let (found, ov) = bignum_SPLAY.TryGetValue kk
    if found then ov
    else
        let nn = nxt_it()
        let meo =  { (*hash__=0;*) n=nn; sortorder=int32(bn % 1000000I)  }
        let nv = X_bnum(prec, bn, meo)
        //dev_println(sprintf "gec_X_Bnum prec=%A vale=%A" prec bn)
        //if bn=34359738426I && nonep prec.widtho then sf "LATEAM hit it"
        bignum_SPLAY.Add(kk, nv)
        nv
        
and gec_XC_bnum(prec, vv) =
    let meo =
        match gec_X_bnum(prec, vv) with
            | X_bnum(_, _, meo) -> meo 
    XC_bnum(prec, vv, meo)


and tnumToX_bn bn = tnumToX (gec_X_bnum({widtho=None; signed=Signed}, bn))

and tnumToX_lc lower_casef = function // Hexadecimal printout of a BigInteger. - bnToHex
    | X_bnum(w, bn, _) ->
        let kz negf (bn:BigInteger) =
            let rec kz1 (bn:BigInteger) =
                if bn.IsZero then []
                else
                    let rem = ref 0I                    
                    let q = BigInteger.DivRem(bn, 4096I, rem)
                    let th =uint32(int !rem)
                    let digits = kz1 q
                    (if negf then 4095u-th else th) :: digits
            let digits0 = kz1 bn
            wideToX_signed lower_casef negf digits0
            
        match bn.Sign with
            | 0 -> "0"
            | 1 -> kz false bn
            | -1 ->
                // The printing of negative hex numbers can follow various conventions.
                // The first, using 'abs' is not very helpful in general. It prints -10 as (~)A
                //
                // The second, prints the hex digits of a two's complement number with a minus sign in front, which is a little misleading, since
                // the hex number is already 'correct' without the presence of the minus sign. Hence we put it in parenthesis.
                // It prints -10 as (-)6 or (-)16 or (-)76 etc..   
                // This second is perhaps the most useful but unfortunately has no best normal form in the absence of a width parameter.
                // The minus sign does, kind of denote, an elipsis for extra FF's. For example:
                //     >  pp1=-288230376151711744    FC00000000000000
                //     <  pp1=-288230376151711744  (-)C00000000000000

                // A curious leading zero situation is that minus1 would display incorrectly as follows:
                // Why do we see pp1=-1  (-)0
                // We need to trap -1 as a special case that does not get a zero rendered.
                
                // The third is as per the second but does not complement the bits and so has no ambiguity on leading ones.
                // It prints -10 as (~-)9 
                //
                // By 'correct' we mean correct except an infinite number of leadings ones are missing.
                //
                //  is there always at least one leading one shown ?  
                //
                //
                // Numbers that are powers of 2 have the slightly odd property that all their low bits are zero, regardless of sign.
                // For example:
                //      0x0100000000000000 =  72057594037927936
                //      0xff00000000000000 = -72057594037927936
                // So don't get tricked by that.
                let tilda_form = 2
                match tilda_form with
                    | 1 -> "(~)" + kz false (BigInteger.Abs bn)  // Here we simply print the magnitude in hex.
                    | 2 -> "(-)" + kz true (-1I - bn)    // Here we print one of the bit patterns of the equivalent two's complement number.
                    | 3 -> "(~-)" + kz false (-1I - bn)           // Here we print a tilda in paren and then the bit pattern of the equivalent two's complement number.

#if TNUM
    | X_tnum lst -> 
         let negf = wide_negativep lst
         let rec kk = function
             | [] -> ""
             | [item] -> (hex3 item) (* nead to know leading z to optimise *)
             | (h::t) -> (kk t) + (hex3 h)
         (if negf then "(-)" else "") + kk lst
#endif
    | (other) -> sf("tnumToX other: " + xToStr other)



and xToStr_ntr nt msg x =
    let ans = xToStr_n nt x
//  let ans = sprintf "<<%i>>(%s)" x.x2nn ans
#if NNPRINT
    let ans = sprintf "<%i>(%s)" x.x2nn ans
#endif
    if g_meo_vd >= 5 then vprintln 5 (msg + sprintf ": xToStr: %i Ans so far " x.x2nn + ans)
    ans
        
and xb_to_str_ntr nt msg x =
    let ans = xbToStr_n nt x
#if NNBPRINT
    let ans = sprintf "<%i>(%s)" x.xb2nn ans   
#endif
    if g_meo_vd >= 5 then vprintln 5  (msg + sprintf ": xbToStr :%i Ans so far " x.xb2nn + ans) 
    ans

// convert to string with indent
and xbToStrI i n = xb_to_str_ntr (ref !g_printing_limit) "" n 
and xToStrI i n = xToStr n 

// Imperative code to string
// marker is a start of line string
// cleaned means newlines removed from each end.
// This does a lot of string cats: better to return a string list with one string per line.
and hbevToStr_cleaned marker x = 
    let r = hbevToStr_m marker x
    let sl = String.length r
    if r.[0] = '\n' then r.[1..]
    elif r.[sl-1] = '\n' then r.[0..(sl-2)]
    else r

and hbevToStr = hbevToStr_m ""
    
and hbevToStr_m marker (a:hbev_t) =  
        let rec i n = if n=0 then marker else  "  " + (i (n-1)) 

        let atts l = if l=[]
                     then ""
                     else "/atts={" + sfold (fun(a,b)-> a + "=" + b + " ") l + "}"

        let limit = ref 300
        let limiting() = (limit := !limit - 1; !limit <= 0)
        let rec xbevToS n x =
            (
            match x with 
            | Xskip        -> "\n" + i(n) + "Xskip;"
            | Xcomment s   -> "\n" + i(n) + "// " + s + "\n"
            | Xdominate(s, v)      -> "\n" + i(n) + "Xdominate(" + s + ", " + (if !v < 0 then "_" else i2s !v) + ");\n"
            | Xgoto(s, v)          -> "\n" + i(n) + "Xgoto(" + s +  (if !v < 0 then "" else ", " + i2s !v) + ");\n"
            | Xfork(s, v)          -> "\n" + i(n) + "Xfork(" + s + ", " + (i2s (!v)) + ");\n"
            | Xlabel(s)            -> "\n" + s + ":"
            //| Xcall((s, gis), a)   -> "\n" + i(n) + "call(" + s + ": " + xToStr_commaList (ref !g_printing_limit) a +  ")"
            | Xwaitrel(g)          -> "\n" + i(n) + "Xwaitrel(" + (xToStr g) + ")"
            | Xwaitabs(g)          -> "\n" + i(n) + "Xwaitabs(" + (xToStr g) + ")"
            | Xwaituntil(g)        -> "\n" + i(n) + "Xwaituntil(" + xbToStrI n g + ")"

            | Xif(g, t, Xskip)     -> "\n" + i(n) + "Xif1 (" + xbToStrI n g + ") " + xbevToS (n+1)(t) // Ambigous else renders not distinguished. TODO.
            | Xif(g, t, f)         -> "\n" + i(n) + "Xif (" + xbToStrI n g + ") " + xbevToS (n+1)(t) + " else " + xbevToS (n+1)(f)

            
            | Xswitch(e, lst)      -> "\n" + i(n) + "switch (" + xToStr e + ") { " + sfold (fun (a,b) -> " pog ") lst + "}"
            | Xwhile(g, c)         -> "\n" + i(n) + "Xwhile (" + xbToStrI n g + ") " + xbevToS (n+1)(c)
            | Xassign(X_bnet ff as l, r) when at_assoc "constant" (lookup_net2 ff.n).ats <> None   -> "\n" + i(n) + xToStr l + " := " + xToStr r + "; // ASSIGN TO CONSTANT!\n"
            | Xassign(l, r)        -> "\n" + i(n) + xToStr l + " := " + xToStr r + ";\n"
            | Xblock(l)            -> "\n" + i(n) + "{" + xbevToS_l (n+1)(l)  + i(n) + "}"
            | Xpblock(l)           -> "\n" + i(n) + "P{" + xbevToS_l (n+1)(l) + i(n) + "P}"
            | Xbreak                -> "\n" + i(n) + "Xbreak"
            | Xjoin(n_, false)     -> "\n" + i(n) + "Xjoin( , killothers=false)"
            | Xjoin(n_, true)       -> "\n" + i(n) + "Xjoin( , killothers=true)"
            | Xbeq(X_false, s, v)  -> "\n" + i(n) + "Xgoto[beq uncond](" + s + ", " + (if !v < 0 then "_" else i2s !v) + ")"
            | Xbeq(g, s, v)        -> "\n" + i(n) + "beq( " + xbToStrI n g + ", " + s + (if !v >= 0 then ", " + i2s !v else "") + ")"
            | Xeasc e              -> "\n" + i(n) + "Xeasc " + xToStrI n e + ";"
            | Xreturn e            -> "\n" + i(n) + "Xreturn " + xToStr e + ";"
            | Xcontinue            -> "\n" + i(n) + "Xcontinue"
            | Xlinepoint(lp, Xskip) -> "\n" + i(n) + lpToStr lp
            | Xlinepoint(lp, s)    -> xbevToS n s
            | Xannotate(m, s)      -> "\n" + i(n) + (if not_nonep m.palias then "A(" + (annotateToStr m) + ", " + xbevToS n s + ")" else xbevToS n s) + atts(m.atts) + (if m.comment <> None then "// " + valOf(m.comment) else "")
            | other                -> "\n" + i(n) + sprintf "hbev-other %A" other
            )
        and xbevToS_l n x =
               match x with
                   | [] -> ""
                   | h::t -> if limiting() then "..." else (xbevToS (n+1) h) + " " + xbevToS_l n (t)

        let ans:string = xbevToS 0 a
        if (ans.[0]='\n') then ans.[1..] else ans


and xToStrList l = foldr (fun (x,y) -> (xToStr x) + " " + y) "" l



// Render an expression, deleting leading ORRED or BLIFT for conciseness.
let rec xToStr_concise = function
    | X_blift e -> xbToStr_concise e
    | other -> xToStr other

and xbToStr_concise = function
    | W_bdiop(V_orred, [e], false, _) -> xToStr_concise e
    | other -> xbToStr other

let netToStr = netToStr1 (ref !g_printing_limit) true

let rec is_nop = function
    | Xeasc x  when constantp x -> true
    | Xcomment _ -> true
    | Xblock l -> List.fold (fun c x -> c && is_nop x) true l
    | Xpblock l -> List.fold (fun c x -> c && is_nop x) true l   
    | Xannotate(_, a)
    | Xlinepoint(_, a) -> is_nop a
    | _ -> false


//
// ewidth: returns Some expression encoding width or None.  - cf mine_prec
//
let rec ewidth_ntr m x =
    match x with
        | X_net(id, _)-> None
        | X_bnet f    -> Some(encoding_width x) // Leaf forms.
        | X_num l     -> Some(encoding_width x)
        | X_bnum(w, l,_)-> if nonep w.widtho then Some(encoding_width x) else w.widtho        
        | X_blift _   -> Some 1
        | W_string _  -> None // With val! todo
        | W_query(_, l, r, _) ->
            (
                let zz = function
                    | (None, None) -> None
                    | (Some a, None) -> Some a
                    | (None, Some b) -> Some b
                    | (Some a, Some b) -> Some(max a b)
                in zz (ewidth m l, ewidth m r)
            )
        | X_subsc(e, _) -> ewidth m e
        | W_asubsc(e, _, _) -> ewidth m e

        | W_node(caster, V_cast cvtf,  [arg], _) ->
            match caster.widtho with
                | None -> ewidth m arg
                | Some n -> Some n
                
        | W_node(prec, oo, l, meo) ->
            let z = function
                | (None, None)     -> None
                | (Some a, None)   -> Some a
                | (None, Some b)   -> Some b
                | (Some a, Some b) when oo=V_bitand  -> Some(min a b)
                | (Some a, Some b) -> Some(max a b)                
            foldl z None (map (ewidth m) l)

        | X_pair(l, r, _) -> ewidth m r
        | X_psl_builtin _ -> Some 1
        | W_apply((f, gis), _, _, _) ->
            match gis.rv.widtho with
                | Some w when w > 0 -> Some w
                | other -> None
        | X_x(x, n, _) -> ewidth m x
        | X_undef -> None
        | other -> sf("ewidth " + m + " other x form: " + xToStr other + "\n")

and ewidth m x =
       let rr = ewidth_ntr m x
       let j =
           function
               | None -> "None"
               | (Some v) -> i2s v
       (* vprintln 0 ( "Ewidth " + xToStr x + "-->" + j rr)  *)
       rr

let xi_width_or_fail m x = valOf_or_failf (fun () ->  m + " " + xToStr x) (ewidth m x)


let xi_order_pred  a b  = x2order a - x2order b;
let xi_border_pred a b  = xb2order a - xb2order b;


let apply_SPLAY     = Dictionary<string * callers_flags_t * int list, hexp_t>()
let node_SPLAY      = Dictionary<precision_t * x_diop_t * int list, hexp_t>()
let cover_SPLAY     = ref (new Dictionary<cover_t, hbexp_t>())
let x_x_SPLAY       = Dictionary<int * int, hexp_t>()
let asubsc_SPLAY    = Dictionary<int * int, hexp_t>()
let bsubsc_SPLAY    = Dictionary<int * int, hbexp_t>();
let psubsc_SPLAY    = Dictionary<int * int, hbexp_t>();
let pair_SPLAY      = Dictionary<int * int, hexp_t>();
let net_SPLAY       = Dictionary<string, hexp_t>()
let bitsel_SPLAY    = Dictionary<int * int, hbexp_t>()
let g_casting_SPLAY = Dictionary<precision_t * cast_severity_t * int, hexp_t>()
let bnode_SPLAY     = Dictionary<x_abdiop_t * int list, hbexp_t>()
let bdiop_SPLAY     = Dictionary<x_bdiop_t * int list, hbexp_t>()
let query_SPLAY     = Dictionary<int * int * int, hexp_t>()



let gec_X_net netname =  // TODO cf netbase - we dont want the same string in both - TODO.
    let kk = netname
    let (found, ov) = net_SPLAY.TryGetValue kk
    if found then ov
    else
        let nn = nxt_it()
        let meo = { (*hash__=netname.GetHashCode();*) n=nn; sortorder=1  }
        let nv = X_net(netname, meo)
        net_SPLAY.Add(kk, nv)
        nv





// Create -n pipeline stages.  X_x is also denoted as Xn short for X^n(...) - the next state function raised to some power.
// Golden law: "X(Q, 1)=d" : ie, the next value of the Q output of a D-type is given by it's D input. 
// In RMODE X_x(d, -1) is the o/p of a D-type or broadside register whose input is d.  This is the z operator from DSP.
//On the lhs it is the other way around, since data flows backwards.
// For XRTL, an Rarc where the lhs is X^(-1) has the same semantic as XRTL_buf, ie. it is combinational.

let rec xi_X n arg =
    if n = 0 then arg
    else
        match arg with
            | X_x(arg, m, _) -> xi_X (n+m) arg
            | arg ->
                if constantp arg then arg // Future and past values of constants are themsevles.
                else
                    let kk = (n, x2nn arg)
                    let (found, ov) = x_x_SPLAY.TryGetValue kk
                    if found then ov
                    else
                        let nn = nxt_it()
                        let meo = { (*hash__=arg.GetHashCode();*) n=nn; sortorder= x2order arg  }
                        let nv = X_x(arg, n, meo)
                        x_x_SPLAY.Add(kk, nv)
                        nv


let gec_X_x(arg, n) = xi_X n arg

       

let clear_espresso_cache() = // NB: kill all cached espresso calls on close of enum group...
   // negated_covers also for clearing ?
   cover_SPLAY := new Dictionary<cover_t, hbexp_t>();
   ()



let mdig<'a> (x:System.Collections.Generic.Dictionary<int, Djg.WeakReference<'a> >) h =
    let (found, ov) = x.TryGetValue(h)
    //vprintln 0 (i2s h + " found = " + boolToStr found)
    if found && (ov).IsAlive() then (ov).Target () else []


// The dot net system has its own hash system so delete me.?
#if SPARE
let rec hexp_bhash__ = function
        | (X_false) -> 0
        | (X_true) -> 1
        | (X_dontcare) -> 2
        | W_bnode(key, l, i, m) -> m.hash__
        | W_cover(_, m)         -> m.hash__        
        | W_bdiop(key, l, i, m) -> m.hash__
        | W_bitsel(A, b, i, m)  -> m.hash__
        | W_bsubsc(A, b, i, m)  -> m.hash__
        | W_bmux(g, ff, tt, m)  -> m.hash__
        | W_linp(A, b, m)       -> m.hash__

// let h = hexp_hash argn
        
and hexp_hash__ =  function
    | X_num(n)          -> n // out of range? TODO please explain.
    | X_net(_, m)       
    | X_bnum(_, _, m)
    | W_node(_, _, _, m)
    | W_lambda(_, _, _, _, m)
    | W_query(_, _, _, m)    
    | W_asubsc(_, _, m)
    | W_valof(_, m)
    | W_apply(_, _, _, m)
    | W_string(_, _, m)    -> m.hash__
    | X_subsc(n, d)        -> hash_digest(hexp_hash__ n, hexp_hash__ d)

    | X_bnet ff            -> ff.id.GetHashCode()
    | X_x(d, n, _)         ->  hash_digest(n, hexp_hash__ d)
    | X_pair(d, n, _)      ->  hash_digest(hexp_hash__ n, hexp_hash__ d)
    | X_undef              -> 412
    | X_blift g  -> hexp_bhash g
    | other -> sf("hexp_hash other " + xkey other + " " + xToStr other)
#endif

let string_splaymap_create arg = Dictionary<int, net_att_t>()

// But subscription
let xint_write_bsubsc(argA, b, inv) = 
    let kk = (x2nn argA, x2nn b)
    let (found, ov) = bsubsc_SPLAY.TryGetValue kk
    let yielder nv =
        if not inv then nv
        else match nv with
              | W_bsubsc(argA, b, false, meo) -> W_bsubsc(argA, b, true, { meo with n= -meo.n })
    let ans = if found then yielder ov
              else 
                let nn = nxt_it()
                let meo =  { (*hash__=0;*) n=nn; sortorder=x2order argA  }
                let nv = W_bsubsc(argA, b, false, meo)
#if NNTRACE
                vprintln 0 (sprintf "write bsubsc nn=%i" nn)
#endif
                bsubsc_SPLAY.Add(kk, nv)
                yielder nv
    (ans:hbexp_t)


let xi_bsubsc(argA, s, inv) = xint_write_bsubsc(argA, s, inv)


let ix_asubsc argA s = 
    let kk = (x2nn argA, x2nn s)
    let (found, ov) = asubsc_SPLAY.TryGetValue kk
    if found then ov
    else
        let nn = nxt_it()
        let ans = W_asubsc(argA, s, { (*hash__=0;*) n=nn; sortorder=x2order argA })
#if NNTRACE
        vprintln 0 ("meox: allocate asubsc nn=" + i2s nn + " for " + xToStr ans)
#endif
        let _ = asubsc_SPLAY.Add(kk, ans)
        ans

 
let ix_pair l r =
    //vprintln 0 (sprintf "ix_pair of L=%s  R=%s" (xToStr l) (xToStr r))
    let kk = (x2nn l, x2nn r)
    let (found, ov) = pair_SPLAY.TryGetValue kk
    if found then ov
    else
        let nn = nxt_it()
        let ans = X_pair(l, r, { (*hash__=0;*) n=nn; sortorder=x2order r })
#if NNTRACE
        vprintln 0 ("meox: allocate pair nn=" + i2s nn + " for " + xToStr ans)
#endif
        let _ = pair_SPLAY.Add(kk, ans)
        ans


let g_prec_s64 = { widtho=Some 64; signed=Signed; }

let xi_int64 v = // legacy wrapper.
    if v=0L then X_num 0
    else gec_X_bnum(g_prec_s64, BigInteger v)

  
let comparisonip = function
    | V_deqd -> true
    | V_dned -> true
    | V_dltd _ -> true
    | V_dled _ -> true
    | V_dgtd _ -> sf ("Should not be a used form dgtd")
    | V_dged _ -> sf ("Should not be a used form dged")
    | (_) -> false


let rec pos_bnum_towide arg = 
    //vprintln 0 (sprintf "Start unw wo=%A arg=%A" wo arg)
    let rec cvtToBase4096 (bn:BigInteger) =
        //vprintln 0 (sprintf "biz2 %A" bn)
        if bn.Sign < 0 then sf "pos_bnum_towide: not positive"
        elif bn.IsZero then []        
        else
            let rem = ref 0I
            let q = BigInteger.DivRem(bn, 4096I, rem)
            //vprintln 0 ("Quot is " + (q).ToString())
            //vprintln 0 ("Remainder is " + (!rem).ToString())
            uint32(int !rem) :: cvtToBase4096 q
    let r = cvtToBase4096 arg
    //vprintln 0 (sprintf "Finish unw wo=%A %A len ^%i ANS=%A" wo arg (length r) r)    
    r

// Positive wide logical operations with zero extend implied.
let pos_wide_logic oo l0 r0 =
    let rec kat = function
       | ([], []) -> []
       | ([], r) -> kat ([0u], r)
       | (l, []) -> kat (l, [0u])
       | (l::lt, r::rt) ->
           let _ = if (l > 4095u) then sf (sprintf "do_wide_logic: denormal lhs: %A" l0)
           let _ = if (r > 4095u) then sf (sprintf "do_wide_logic: denormal rhs: %A" r0)
           let xx =
               match oo with
               | V_bitand -> (&&&) l r
               | V_bitor  -> (|||) l r
               | V_xor    -> (^^^) l r
           xx :: (kat (lt, rt))

    let kat_vd (l, r) =
        let ans = kat (l, r)
        let he (a:uint32) = sprintf "%03X" a
        vprintln 0 (sprintf "kat oo=%A  l=%s r=%s ans=%s" oo (sfold he l) (sfold he r) (sfold he ans))
        ans

//    let kat = kat_vd // comment me out when not needed
    kat (l0, r0)



// 
let complement_and_sign_extend digits arg =
    let rec complement_and_sign_extend1 digits arg =    
        match arg with
            | [] when digits >= 12    -> 4095u :: complement_and_sign_extend1 (digits-12) []
            | [] when digits <= 0     -> []
            | []                      -> [ (1u<<<digits)-1u ]
            | [item] when digits >=12 -> (4095u-item) :: complement_and_sign_extend1 (digits-12) []
            | [item] when digits <= 0 -> [ 4095u-item ]
            | [item]                  ->
                if (1u<<<digits) >= item then [ (1u<<<digits)-item]
                else
                    let _ = vprintln 3 (sprintf "complement+sex: tail item item=%A digits=%A" item digits)
                    let _ = vprintln 3 ("too few digits")
                    [item]
            | a::b::tt                -> (4095u-a) :: complement_and_sign_extend1 (digits - 12) (b::tt)
    let ans = complement_and_sign_extend1 digits arg
    let pw x = wideToX_signed true true x    
    //vprintln 0 (sprintf " complement_and_sign_extend digits %i  arg=%A ans=%A" digits (pw arg) (pw ans))
    ans

let wideToX = wideToX_signed true false

    
let wide_find_msb widen =
    let rec msb1 pos n = // find bit pos in base 12 digit.
        if n=0u then sf "unreachable L984"
        elif n=1u then pos
        else msb1 (pos+1) (n/2u)

    let rec fmsb p q = function
        | []                  -> (p, q)
        | nn::tt when nn = 0u -> fmsb (p+12) q tt // unnecessary? leading zero digit
        | nn::tt when q < 0   ->
            let msb = msb1 0 nn
            fmsb (p+11-msb) (msb) tt
        | nn::tt when q >= 0 -> fmsb p (q+12) tt // just count length of remainder
    // q is bit number of msb set bit.
    // p is number of leading zeros - not useful.
    let (p, q) = fmsb 0 -1 (rev widen)
    //vprintln 0 (sprintf "msb settings in %A %i %i" widen p q)
    (p, q)

//
let xdeval m = function
    | X_num v       -> v
    | X_bnum(w, bn, _) -> int32((*System.Convert.ToInt32*) bn) // Don't call me if I might overflow
    | other -> sf (m + "Constant number expected (xdeval). Not this: " + xToStr other)



(* Low Numbers arrays. *)
let numbers_positive  =  Array.create 4096 X_undef
let numbers_negative  =  Array.create 4096 X_undef



let xi_num n =
    if n < g_small_integer_neg_bound  || n > g_small_integer_pos_bound then
        gec_X_bnum({ signed=Signed; widtho=Some 32; }, BigInteger n)
    elif n > 4095 || n < -4095 then X_num n  (* Licensed *)
    else (if n >= 0 then numbers_positive.[n]
          else numbers_negative.[-n])

let xi_unum x =
    let n:uint32 = uint32(x)
    gec_X_bnum({ signed=Unsigned; widtho=Some 32; }, BigInteger n)



// Opposite to bnum_nf_unw
let rec bnum_nf_w_core = function
    | []   -> 0I
    | (h:uint32)::t -> BigInteger(int h) + 4096I * bnum_nf_w_core t

let xi_bnum (bn:BigInteger) = 
    if bn >= -4096I && bn < 4096I // TODO review this range - rather small now X_tnum no longer used.
    then xi_num(int32 bn) // This also has implied width of 32. Best avoid.
    else gec_X_bnum({widtho=None; signed=Signed}, bn)

let xi_bnum_n w (bn:BigInteger) = 
    if w=32 && (bn >= -4096I && bn < 4096I) then xi_num(int32 bn)
    else gec_X_bnum({widtho=Some w; signed=Signed}, bn)


let bnum_nf_w = function
    | [] -> xi_num 0
    | [item:uint32] -> xi_num(int item)
    | lst ->
        let ans = xi_bnum (bnum_nf_w_core lst)
        //vprintln 0 ("bnum_nf_w complete " + xToStr ans)
        ans

let lowmasks = Array.create 64 None
let himasks = Array.create 65536 None

// Returns (2^n-1)
let lowmask n = 
    let vd = -1
    assertf(n >=0 && n<64) (fun () -> "lowmask bad " + i2s n)   
    match lowmasks.[n] with
        | Some r -> r
        | None ->
            let xx = uint64(int64((two_to_the n)-1I))
            let _ = Array.set lowmasks n (Some xx)
            if vd>=4 then vprintln 4 (sprintf "LMask %A:%A\n" n xx)
            xx

let bn_unary (n:int) =
    // TODO may want to cache a table of these.
    BigInteger.Pow(2I, n)

// Same as lowmask but using wide arithmetic.
let rec himask n = 
        let r = himasks.[n]
        if r <> None then valOf r
        else
            let x = bn_unary n - 1I
            let _ = Array.set himasks n (Some x)
            x

let rec bnum_bitsel n (bn:BigInteger) = // bitsel/bit select/bit extract.
    match bn.Sign with
        | 0 -> false

        | -1 -> // bnum bitsel from -ve numbers 
            // We want to give the impression of two's complement but a bnum is stored sign and magnitude.
            // For all bit positions above a certain leading zero position we will return exclusively ones.
            // Example, bit 1 of -5 (111...011) is 1. 
            // We return the complement of the same bit from -1I-bn
            // Note that two's complement, like signed arithmetic, has the same bit derivative, meaning that setting a bit makes the number move towards +inf.
            // So we expect an even number of negations in this clause.  
            not (bnum_bitsel n (-1I-bn))
        | 1 -> 
            let rem = ref 0I // Repeated division - slow? 
            // NB: BigInteger supposedly has an IsEven property
            let rec bdiv n bn =
                let q = BigInteger.DivRem(bn, 2I, rem)
                if n = 0 then not (!rem).IsZero
                else bdiv (n-1) q
            let ans = bdiv n bn
            //vprintln 0 (sprintf "bnum_bitsel %i for %A is %A" n bn ans)
            ans

// Perform bitwise AND of signed big numbers.
let rec bnum_bitand_with_width signed width (l0:BigInteger) (r0:BigInteger) =
    let bvd = false
    let pw x = wideToX x
    let phex = tnumToX_bn 
    let ones_comp = map (fun x-> 4095u - x)
    let ans0=
        match (l0.Sign, r0.Sign) with
            | (-1, -1) ->
                let (ln, rn) = (-1I - l0, -1I - r0) // Form ones-complement - subtract from -1.
                // We can take the one's complement, do the de Morgan dual (V_bitor) on that data and then complement again.
                //vprintln 0 ("bnum_bitand_with_width: both args -ve")
                let ans = pos_wide_logic V_bitor (pos_bnum_towide ln) (pos_bnum_towide rn) 
                bnum_nf_w_core(ones_comp ans)

            | (-1, _) -> bnum_bitand_with_width signed width r0 l0 // commute - (prefer mask to be first arg since +ve in general).

            | (_, -1) -> 
                let l1 = pos_bnum_towide l0
                let r05 = pos_bnum_towide (-1I - r0)
                let rec compand p ps =
                    //vprintln 0 (sprintf " compand p=%i ps=%A" phex ps)
                    match ps with 
                    | ([], []) -> []
                    | ([], rr) -> compand p ([0u], rr)
                    | (ll, []) -> compand p (ll, [0u])
                    | (hl::tl, hr::tr) when p+12 <= width -> (hl &&& (4095u-hr))::compand(p+12) (tl, tr)
                    | (hl::_, hr::_) -> [ hl &&& ((1u<<<(width-p))-hr-1u)]
                    // | (other) -> sf (sprintf "compand other form %A" other)
                let ans = compand 0 (l1, r05)
                //vprintln 0 (sprintf "bitand_with_width (+-)  width=%i ????  bitand with width main conj %A with %A giving %A" width (pw l1) (pw r05) (pw ans))
                //let ans = pos_wide_logic V_bitand l1 r1
                bnum_nf_w_core ans

            | (_, _) -> // both args positive or zero: straightforward bitwise disjunction.
                // Assume at least one argument is no wider than the width parameter.
                let ans = pos_wide_logic V_bitand (pos_bnum_towide l0) (pos_bnum_towide r0)
                bnum_nf_w_core ans
    let ans1 = 
        if not signed then ans0
        else
            let topbit = bnum_bitsel (width-1) ans0
            if topbit then
                // If answer is -ve, we need to subtract 2^width.
                // Eg:, with width=4, an answer of 1110 (14) needs to be converted to -2
                // Eg:, with width=4, an answer of 1101 (13) needs to be converted to -3
                //vprintln 0 (sprintf "bnum_bitand_with_width %i: topbit set, ans0=%A" width ans0)
                ans0 - bn_unary width
            else ans0
    let _ = if bvd then vprintln 0 (sprintf "bnum_bitand_with_width signed=%A %i: bnum numbers %A & %A -> %A" signed width l0 r0 ans1)
    let _ = if bvd then vprintln 0 (sprintf "bnum_bitand_with_width signed=%A %i:              %A & %A -> %A" signed width (phex l0) (phex r0) (phex ans1))
    ans1





// Make manifest constant (this form used for floating point only infact)
let fps_core (fps:string) width constval =
    let signed = FloatingPoint
    let length = []
    let io = LOCAL
    let _ = if width <> 64 && fps.[strlen fps - 1] <> 'f' then sf ("Bad float32 string registered " + fps)
    let (nn, ov) = netsetup_start fps
    if not_nonep ov then 
        let ff = valOf ov
        let f2 = lookup_net2 nn
        assertf(length = f2.length) (fun () -> ff.id + ": xi_double new len " + sfold i2s64 length + " cf " + sfold i2s64 f2.length)
        cassert(signed = ff.signed, "xi_double new signed")
        assertf(width = ff.width) (fun () -> sprintf ": id=%s xi_double now has a new width %i cf old=%i" ff.id ff.width width)
        cassert(io = f2.xnet_io, "xi_double new io")
        ff
    else 
        let pol = ref false
        let dir = ref false
        let vt = ref V_REGISTER
        let states = ref None
        let e2 = 0
        let ff = { 
                    id=       fps
                    constval= [constval]
                    signed=   signed
                    is_array= not_nullp length
                    rh=       0I
                    rl=       0I
                    width=    width
//                    states= None
                    n=        nn
                }
        let f2 =
                {
                    length=   length
                    dir=      !dir
                    pol=      !pol
                    xnet_io=  io 
                    vtype=    !vt
                    ats= []; //  if constantp then [ Nap(g_resetval, fps); Nap("constant", "true"); ] else []; // old way TODO do not use this attribute for constants, only for reset values.
                }
        ignore(netsetup_log (ff, f2))
        ff

// Some numeric constants
let g_sp_zero = X_bnet(fps_core "0.0f" 32 (XC_float32 0.0f))
let g_sp_one  = X_bnet(fps_core "1.0f" 32 (XC_float32 1.0f))

let g_dp_zero = X_bnet(fps_core "0.0" 64 (XC_double 0.0))
let g_dp_one  = X_bnet(fps_core "1.0" 64 (XC_double 1.0))


let bix bn = tnumToX (gec_X_bnum({widtho=None; signed=Signed}, bn))

// BigIntegers use sign and magnitude representation.
// There exists an (unused) procedure for masking while preserving sign for unsized numbers.
// To get this effect on -ve BigIntegers we proceed as follows:
//     flip sign, subtract one, mask to the precision minus 1, add one and flip the sign.
// For example, if working to 8 bits, we want a number in the range 1 to 128 that we then set the sign bit on.
// If the value is -5 we mask 4 with 127, making 4, then convert back to -5.

// If we are signed and we mask a large number to a smaller width we adopt the sign of the msb of the masked result. This is C# behaviour when casting, say, an Int64 to an Int32.
// This is the same as doing a fixed-precision bitand with an operand of all ones.


// If we are unsigned and operate on a -ve arg we need to treat it as the equivalent positive number.  We need to know its input precision for this, rather than the returned precision.

let bn_masker (arg_prec:int option) (caster:precision_t) x =
    let vd = 3
    match caster.signed with
        | Unsigned ->
            match caster.widtho with // In the unsigned case, the input arg may be negative, but needs to be treated as the equivalent +ve bit pattern so we need _its_ width from arg_precision
                | Some w ->
                    let wmask = himask w
                    let x' =
                        if x >= 0I then x
                        elif nonep arg_prec then
                            dev_println ("Missing argument precision for bn_masker -ve argument " + bix x)
                            x
                        else
                            //vprintln 0 (sprintf "bn_masker bump  up -ve by adding 2^%i" (valOf arg_prec))
                            bn_unary (valOf arg_prec) + x // For example, -1 in a 3 bit field needs to be treated as the unsigned number 7.
                        
                    if vd>4 then vprintln vd (sprintf "bn_masker UNSIGNED signed=%A w=%i wmask=%A x=%s x'=%s" caster.signed w wmask (bix x) (bix x'))    
                    let y = bnum_bitand_with_width (caster.signed=Signed) w wmask x' // We prefer mask to be first arg since +ve in general.
                    if vd>4 then vprintln vd (sprintf "bn_masker unsigned w=%i x=%A returns y=%A" w x' (bix y))
                    gec_X_bnum(caster, y)
                | None -> muddy "missing masker - no width - please use unused procedure"
        | Signed ->
            match caster.widtho with
                | Some w ->
                    let wmask = himask w
                    if vd>4 then vprintln vd (sprintf "bn_masker SIGNED signed=%A w=%i wmask=%A x=%s" caster.signed w wmask (bix x))    
                    let y = bnum_bitand_with_width (caster.signed=Signed) w wmask x // We prefer mask to be first arg since +ve in general.
                    if vd>4 then vprintln vd (sprintf "bn_masker w=%i x=%A returns y=%A" w x (bix y))
                    gec_X_bnum(caster, y)
                | None -> muddy "missing masker - no width - please use unused procedure"

        | FloatingPoint ->
            //dev_println (sprintf "bn_masker floating point mask to %A precision on %A" caster.widtho x)
            let fps = sprintf "%A" x
            let fps = if fps.[strlen fps - 1] = 'I' then fps.[0..strlen fps - 2] else fps
            //vprintln 0 (sprintf "bn_masker fps=%s from %A" fps x)
            let constval =
                match caster.widtho with
                    | Some 32 -> (XC_float32 (float32 fps))
                    | Some 64 -> (XC_double  (double(fps)))
            let fps1 = fps + ".0" + (if caster.widtho=Some 32 then "f" else "")
            X_bnet(fps_core fps1 (valOf caster.widtho) constval)

            
// Bitwise AND of signed numbers can be done without knowing the width field precision.
// Using a bn, ANDing without a bit precision is a different operation from masking (which is anding with a bit precision).
//   - Masking needs to recreate a new sign bit - both for +ve and -ve inputs.
//   - ANDing with unspecified width only returns -ve if both operands are -ve.


// If one, A, is positive, and the other, B, is negative, then complement B and bitwise compute a.!b
// If both are negative, we use demorgan dual on their complements and complement again.
let rec bnum_bitand_unspec_width (l0:BigInteger) (r0:BigInteger) =
    let p x = tnumToX (gec_X_bnum({widtho=None; signed=Signed}, x))
    let ones_comp = map (fun x-> 4095u - x)
    let ans =
        match (l0.Sign, r0.Sign) with
            | (-1, -1) ->
                let (ln, rn) = (-1I - l0, -1I - r0) // Form ones-complement - subtract from -1.
                // We can take the one's complement, do the de Morgan dual (V_bitor) on that data and then complement again.
                let ans = pos_wide_logic V_bitor (pos_bnum_towide ln) (pos_bnum_towide rn) 
                bnum_nf_w_core(ones_comp ans)

            | (-1, _) -> bnum_bitand_unspec_width r0 l0 // commute

            | (_, -1) ->
                let pw x = wideToX x
                // We compute l.~r
                let l1 = pos_bnum_towide l0
                let (_, msb) = wide_find_msb l1
                let w = msb+1
                let r1 = complement_and_sign_extend w (pos_bnum_towide (-1I - r0))
                //vprintln 0 (sprintf "AND main conj %A with %A" (pw l1) (pw r1))
                let ans = pos_wide_logic V_bitand l1 r1
                bnum_nf_w_core ans

            | (_, _) -> // both args positive or zero: straightforward bitwise disjunction.
                //vprintln 0 ("bnum_bitand_unspec_width: both args +ve")
                let ans = pos_wide_logic V_bitand (pos_bnum_towide l0) (pos_bnum_towide r0)
                bnum_nf_w_core ans
    //vprintln 0 (sprintf "bnum_bitand_unspec_width: bnum numbers %A & %A -> %A" l0 r0 ans)
    //vprintln 0 (sprintf "bnum_bitand_unspec_width:              %A & %A -> %A" (p l0) (p r0) (p ans))
    ans


// Bitwise OR of signed numbers can be done without knowing the width field.
// If one, A, is positive, complement the other, B, and compute bitwise !a.b, then one's complement the result for the -ve answer.
// If both are negative, we use de Morgan dual on their complements and complement again.
let rec bnum_bitor (l0:BigInteger) (r0:BigInteger) =
    let p x = tnumToX (gec_X_bnum({widtho=None; signed=Signed}, x))
    let ones_comp = map (fun x-> 4095u - x)
    let ans =
        match (l0.Sign, r0.Sign) with
            | (-1, -1) ->
                let (ln, rn) = (-1I - l0, -1I - r0) // Form ones-complement - subtract from -1.
                // We can take the one's complement, do the de Morgan dual (V_bitand) on that data and then complement again.
                let ans = pos_wide_logic V_bitand (pos_bnum_towide ln) (pos_bnum_towide rn) 
                bnum_nf_w_core(ones_comp ans)

            | (-1, _) -> bnum_bitor r0 l0 // commute

            | (_, -1) ->
                let rn = -1I - r0 // We compute ~l.r
                let r1 = pos_bnum_towide rn
                let l1 = complement_and_sign_extend (12 * length r1) (pos_bnum_towide l0) // It matters not if we over extend here.
                let pw x = wideToX x
                //vprintln 0 (sprintf "OR main conj %A with %A" (pw l1) (pw r1))
                let ans = pos_wide_logic V_bitand l1 r1
                -1I - bnum_nf_w_core ans

            | (_, _) -> // both args positive or zero: straightforward bitwise disjunction.
                let ans = pos_wide_logic V_bitor (pos_bnum_towide l0) (pos_bnum_towide r0)
                bnum_nf_w_core ans
    //vprintln 0 (sprintf "bnum_bitor: bnum numbers %A | %A -> %A" l0 r0 ans)
    ans


    
// Bitwise XOR of signed numbers can be done without knowing the width field.
//   1.  If both are negative then the answer is positive and is the XOR of the one's complement of each arg.
//       For example, -2 xor -5 is 111...110 ^ 111...011 giving 000...101 = 1 ^ 4 = 5.    
//   2.  If they differ in sign, the answer is -ve and is given by the one's complement of the XOR of the positive one and the one's complement of the negative one.
//       For example,  2 xor -5 is  000...010 ^ 111...011 giving 111...001 = -(2 ^ 4) = -7.    
// Ones complement is achieved by subtracting from -1.
let bnum_bitxor (l:BigInteger) (r:BigInteger) =
    let p x = tnumToX (gec_X_bnum({widtho=None; signed=Signed}, x))
    let posate (bn:BigInteger) = if bn.Sign < 0 then -1I-bn else bn
    let ans = bnum_nf_w_core(pos_wide_logic V_xor (pos_bnum_towide (posate l)) (pos_bnum_towide (posate r)))
    let ans = if (l.Sign < 0) <> (r.Sign < 0) then -1I-ans else ans
    //vprintln 0 (sprintf "bnum_bitxor   %A ^ %A gives %A" (p l) (p r) (p ans))
    ans




(*--------------------------------------------------------------------------*)

// Cloned net is always a LOCAL.
let clonenet prefix suffix = function
    | X_bnet ff ->
        let f2 = lookup_net2 ff.n
        xgen_bnet(netgen_serf_ff(prefix  + ff.id + suffix, f2.length, ff.rh, ff.width, ff.signed))
    | X_net(id, meo_) ->  gec_X_net(prefix + id + suffix)
    | other -> sf("clonenet other: " + xToStr other)


// hmmm a few more atts are copied over by cloneio... better to copy all except varmode.
let cloneio prefix suffix as_io_opt = function
    | X_bnet ff     ->
        let f2 = lookup_net2 ff.n        
        let xnet_io = valOf_or as_io_opt f2.xnet_io
        xgen_bnet(iogen_serf_ff(prefix+ff.id+suffix, f2.length, ff.rh, ff.width, xnet_io, ff.signed, None, false, f2.ats, ff.constval))
    | X_net(id, _) -> gec_X_net(prefix + id + suffix)
    | other -> sf("cloneio other: " + xToStr other)




(*---------------------------------------------*)



(*
 * xToStr without quotes on a string and inserting escapes.
 *)
let xToStr_nq = function
    | W_string(s, _, _) -> 
         let ll = String.length s
         let rec ff p =
             if p>=ll then ""
             else
                 let h = s.[p]
                 let (n, d) =
                     if h = '\\' && p < ll-1 && s.[p+1] = 'n'
                     then ("\n", 2)
                     else (System.Convert.ToString(System.Convert.ToChar h), 1)
                 n + ff (p+d)
         ff 0 
    | (x) -> xToStr x


let xToStr_plus_key a = xkey a + "." + xToStr a

// Right-shift a BigInteger
let bnum_rshift ss (prec:precision_t) (ll :BigInteger) (rr :BigInteger) =
    if rr.Sign < 0 then sf "-ve bnum rshift amount"
    elif rr.Sign = 0 then ll
    else
        let vd = -1
        let unsigned = prec.signed=Unsigned || ss=Unsigned // Unsigned rhsift is also known as logical rshift. Signed is known as arithmetic.
        // Arithmetic right shift of an unsized negative number is not problematic - it is a divide by a power of two that rounds up in magnitude (towards -inf) instead of downwards.  For instance, -3 >> 1 = -2.
        if ll.Sign < 0 && not unsigned then
            let den = bn_unary (int32 rr)
            let ans = (ll-den+1I) / den // just use divide, but make sure rounds downwards.
            if vd>4 then vprintln vd (sprintf "Bnum_rshift: signed rshift -ve big number by %A by %A giving %A" (tnumToX_bn ll) rr (tnumToX_bn ans))
            ans
        elif ll.Sign < 0 && unsigned then // Bnum_rshift: unsigned rshift of -ve big number  

            let den = bn_unary (int32 rr)
            let num_bytes =
                 match prec.widtho with
                    | Some w ->
                        if w % 8 <> 0 then sf ("precision must be a multiple of 8") // FIXME error properly
                        w / 8
                    | None -> sf ("Cannot perform unsigned right shift on a BigInteger without a specified width") // FIXME error properly
            let ans =  // Get bytes at correct width and make sure unsigned
                let bytes =
                    ll.ToByteArray()
                    |> Seq.append <| Seq.initInfinite (fun _ -> 0xFFuy) // Fill the right with 1's (sign extension)
                    |> Seq.take num_bytes // Truncate to the correct width
                    |> Seq.append <| [ 0x00uy ] // Append a 0 to force the bytes to be interpreted as a positive number
                    |> Seq.toArray
                let ll_bits = new BigInteger(bytes)
                ll_bits >>> (int rr) // Perform 'unsigned' shift on the bits (because they are now interpreted as positive)

            if vd>4 then vprintln vd (sprintf "Bnum_rshift: unsigned rshift -ve big number by %A by %A giving %A" (tnumToX_bn ll) rr (tnumToX_bn ans))
            ans
        else
            let den = bn_unary(int32 rr)
            let ans = ll / den // just use divide.
            if vd>4 then vprintln vd (sprintf "Bnum_rshift: rshift positive big number by %A by %A giving %A" (tnumToX_bn ll) rr (tnumToX_bn ans))
            ans


// For speed, some non-inflating operations are kept using this old routine that uses signed int32.
let xgen_diop_man prec oo l r =
    match oo with 
    | V_divide _ -> if r = 0 then sf "divide by zero" else l/r
         
    | V_bitand    -> l &&& r
    | V_xor       -> l ^^^ r
    | V_bitor     -> l ||| r

          // lshift and exp can oveflow so are now disabled.
    | V_exp when false -> let rec k(ans, r) = if r=0 then ans else k(ans * l, r-1) in k (1, r)

    | V_lshift when false -> // This can overflow of course
        let m = two_to_the r
        let _ = if m=0I then sf("out of range left shift " + i2s r)
        l * int(m)

    | V_rshift issigned ->
        // Notes: -3/2 gives -2 owing to round to -inf.
        let ans = 
            match issigned with
                | Signed   ->  l >>> r // In FSharp, all right shifts are denoted with the trigram (so different from both Verilog and Java!).

                | Unsigned -> (int)((uint32 l) >>> r)
        //vprintln 0 (sprintf "rshift constant prec=%A %A   %i >> %i yielding %i" prec issigned l r ans)
        ans
    | V_mod ->
              let _ = if r=0 then sf("mod by zero")
              l % r
    | oo -> sf (sprintf " unsupported old xgen_diop_man %A" oo)

// Generate/render a member of an enumeration (rarely used - see enum_const instead)
let xi_named_constant p strobeo name = xi_stringx (XS_withval(xi_unum p)) name

let xi_named_big_constant p strobeo name = xi_stringx (XS_withval(xi_bnum p)) name


let xi_num64 (n:int64) = if n >= 2147483647L || n <= -2147483648L then  xi_bnum_n 64 (BigInteger n) else xi_num (int32 n)


//
// Perform comparision of a pair of integer/enum constants: return 1 if equal, 0 otherwise.
//
let rec constant_comp oo argpair =
    //vprintln 0 (sprintf "constant_comp %s cf %s" (xToStr a) (xToStr b))
    let tobn = BigInteger.Parse
    let tob32 (x:int32) = BigInteger(x)
    let tob64 (x:int64) = BigInteger(x)
    let ansf bbool = if bbool then 1 else 0
    let dconv (arg:BigInteger) = double(arg) // may overflow in extrodinary cases.
    let rec ansf_double flip ld rd =
        if flip then ansf_double false rd ld
        else
            match oo with
                | V_deqd         -> ansf (ld = rd) 
                | V_dltd signed_ -> ansf (ld < rd)   // signed_ should be FloatingPoint - we don't do unsigned floating point comparisons anyway
                | V_dled signed_ -> ansf (ld <= rd)
                | other -> sf (sprintf "ansf_double other oo=%A" oo)

    let ansf_double_bn flip ld rbn = //compare a double with a big number - may overflow . must watch for truncation too
        match oo with
            | V_deqd ->
                let i = (int64) ld
                let backagain = (double) i
                if ld <> backagain then ansf false else ansf(tob64 i = rbn)
            | V_dltd signed_ when not flip -> ansf(ld < dconv rbn) // signed_ should be FloatingPoint - we don't do unsigned floating point comparisons anyway
            | V_dled signed_ when not flip -> ansf(ld <= dconv rbn)            
            | V_dltd signed_ when flip -> ansf(ld > dconv rbn)
            | V_dled signed_ when flip -> ansf(ld >= dconv rbn)            
            | other -> sf (sprintf "ansf_double_bn other oo=%A" oo)                


    let posate prec bn = // Find equivalent +ve number by 
        if bn >= 0I then gec_X_bnum(prec, bn)
        else
            let (w, corrector) =
                match prec.widtho with
                    | Some w -> (w, two_to_the w)
                    | None -> sf(sprintf "posate: field width needed for negative constant in unsigned comparison %A" bn)
            let rr = bn + corrector
            let _ = if rr < 0I then sf (sprintf "posate still not +ve on prec=%i for %A" w bn)
            gec_X_bnum(prec, rr)
            
    let rec flipmatch oo flip argpair =
        if oo = V_dned then if constant_comp V_deqd argpair=0 then 1 else 0
        else
        match argpair with
        | (X_bnum(prec_l, l, _), X_bnum(prec_r, r, _)) ->
            match oo with // hmm signed_ is ignored
                | V_deqd -> ansf(l = r)

// To perform an unsigned comparison on a -ve bignumber we first convert it to the corresponding unsigned number based on its precision.
                | V_dltd Unsigned -> flipmatch (V_dltd Signed) flip (posate prec_l l, posate prec_r r)
                | V_dled Unsigned -> flipmatch (V_dled Signed) flip (posate prec_l l, posate prec_r r)

                | V_dltd signed_ when not flip  -> ansf(l < r)  
                | V_dled signed_ when not flip  -> ansf (l <= r)
                | V_dltd signed_ when flip      -> ansf (r < l) 
                | V_dled signed_ when flip      -> ansf (r <= l)

        | (X_num l, X_num r) ->
            match oo with
                | V_deqd                           -> ansf(l=r)
                | V_dltd Signed   when not flip -> ansf(l<r)
                | V_dltd Unsigned when not flip -> if (uint32 l) < (uint32 r) then 1 else 0        
                | V_dled Signed   when not flip -> if l<=r then 1 else 0
                | V_dled Unsigned when not flip -> if (uint32 l)<=(uint32 r) then 1 else 0
                | V_dltd Signed   when flip     -> if r<l then 1 else 0
                | V_dltd Unsigned when flip     -> if (uint32 r) < (uint32 l) then 1 else 0        
                | V_dled Signed   when flip     -> if r<=l then 1 else 0
                | V_dled Unsigned when flip     -> if (uint32 r)<=(uint32 l) then 1 else 0
       

        | (X_num l, X_bnum(rw, r, rm)) -> flipmatch oo flip (gec_X_bnum(g_s32, tob32 l), X_bnum(rw, r, rm))
        | (X_bnum(lw, l, lm), X_num r) -> flipmatch oo flip (X_bnum(lw, l, lm), gec_X_bnum(g_s32, tob32 r))   

        | (W_string(_, XS_withval v, _), r) -> flipmatch oo flip (v, r)
        | (l, W_string(_, XS_withval v, _)) -> flipmatch oo flip (l, v)

        | (X_bnet ff, rr) ->
            let f2 = lookup_net2 ff.n
            match ff.constval with
                | [] -> sf (sprintf "Cannot find constant setting in net details: %A" ff)
                | XC_bnum(lw, lbn, lm) :: _ ->
                    match rr with
                        | X_bnet ff' ->
                            //let f2' = lookup_net2 nn'
                            match ff'.constval with
                                | XC_double rd :: _     -> ansf_double_bn (not flip) rd lbn
                                | XC_float32 rs :: _      -> ansf_double_bn (not flip) (double rs) lbn 
                                | XC_bnum(rw, rbn, rm) :: _ -> flipmatch oo flip (gec_X_bnum(lw, lbn), X_bnum(rw, rbn, rm))
                                | other -> sf (sprintf "9/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other)
                        | X_num nb            -> flipmatch oo flip (X_bnum(lw, lbn, lm), X_num nb)
                        | X_bnum(rw, rbn, rm) -> flipmatch oo flip (X_bnum(lw, lbn, lm), X_bnum(rw, rbn, rm))
                        | other -> sf (sprintf "8/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other)

                | XC_string _ :: _ ->
                    match rr with
                        | X_bnet ff' ->
                            //let f2' = lookup_net2 ff'.n
                            match ff'.constval with
                                //| m -> muddy "string const comp"
                                | other -> muddy (sprintf "5/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other )
                | XC_double ld :: _ ->
                    match rr with
                        | W_node(_, V_cast _, [X_bnet ff'], _)  // TODO precision is ignored here.
                        | X_bnet ff' ->
                            //let f2' = lookup_net2 ff'.n
                            match ff'.constval with
                                | XC_bnum(_, rbn, _) :: _ -> ansf_double_bn flip ld rbn
                                | XC_double rd :: _       -> ansf_double flip ld rd
                                | XC_float32 rs :: _     -> ansf_double flip ld (double rs)
                                | other -> sf (sprintf "0/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other)

                            | X_num n -> ansf_double flip ld (double n)
                            | other -> sf (sprintf "6/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other)

                | XC_float32 ls :: _  ->
                    match rr with
                        | W_node(_, V_cast _, [X_bnet ff'], _)  // TODO precision is ignored here.
                        | X_bnet ff' ->
                            //let f2' = lookup_net ff'.n'
                            match ff'.constval with
                                | XC_bnum(_, rbn, _) :: _   -> ansf_double_bn flip (double ls) rbn    
                                | XC_double rd :: _         -> ansf_double flip (double ls) rd
                                | XC_float32 rs :: _        -> ansf_double flip (double ls) (double rs)
                                | other -> sf (sprintf "1/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other )
                            | X_num n -> ansf_double flip (double ls) (double n)
                            | other -> sf (sprintf "7/4 other form constant in constant_comp %s rhs=%s other=%A" (xToStr (X_bnet ff)) (xToStr rr) other)




        | (W_node(cl, V_cast cvtf, [al], _), r) -> // TODO  - need to process the cast
            let _ = vprintln 3 (sprintf "constant comp: l process cast ignored")
            flipmatch oo flip (al, r)

        | (l, W_node(cl, V_cast cvtf, [ar], _)) -> // TODO  - need to process the cast
            let _ = vprintln 3 (sprintf "constant comp: r process cast ignored")
            flipmatch oo flip (l, ar)


        | (W_node(prec_a, ao, al, _), W_node(prec_b, bo, bl, _)) ->
            let rec sho = function
                | [], [] -> 1
                | a::av, b::bv -> if flipmatch oo flip (a, b)=0 then 0 else sho(av, bv)
            if ao<>bo then 0
            else sho (al, bl)


        | (X_pair(a, b, _), X_pair(p, q, _)) -> if flipmatch oo flip (a, q)<>0 && flipmatch oo flip (b, q)<>0 then 1 else 0

        | (X_pair _, _) 
        | (_, X_pair _) when oo=V_deqd ->  if (x2nn(fst argpair))=(x2nn( snd argpair)) then 1 else 0

        | (X_undef, X_undef) when oo=V_deqd -> 1
        | (X_undef, _) when oo=V_deqd -> 0
        | (_, X_undef) when oo=V_deqd -> 0

        | (W_string _, _)
        | (_, W_string _) when oo=V_deqd -> if (x2nn(fst argpair))=(x2nn( snd argpair)) then 1 else 0

        | (W_node _, _)
        | (_, W_node _) when oo=V_deqd -> if (x2nn(fst argpair))=(x2nn( snd argpair)) then 1 else 0

        | (l, X_bnet f) ->
            if flip then sf (sprintf "already flipped constant_comp %A lhs=%s rhs=%s" oo (xToStr  l) (xToStr (X_bnet f)))
            else flipmatch oo true (X_bnet f, l)


        | (a, b) -> sf (sprintf "meox: constant_comp other " +  f3o4(xbToStr_dop oo) + " A=" + xToStr a + " B=" + xToStr b + "  (" + xkey a + " cf " + xkey b + ")")

    flipmatch oo false argpair


(*
 * Mask a wide integer as though it were stored in a reg of width w.
 * Now wide masking must trim bits and/or sign extend bits.
 * A naive call to the wide_and often adds leading zeros to a negative number, making it positive.
 *
 *  Example,  we want to mask -100000 to 16 bits, including the sign bit
 *  018_6a0 is 100000
 *  fe7_960 is -100000  The answer is 7_960 which is a postive result, which is correct because of
 * overflow rather than because we have added extra 0_ leaders.
 *
 * 
 *)

let wide_divmod(divf, l, r) =
    let rem = ref 0I
    let q = BigInteger.DivRem(l, r, rem)
    if divf then q else !rem

let get_f32_bits (f32:float32) =
    let bytes = System.BitConverter.GetBytes f32
    //let _ = for z in bytes do vprintln 0 (sprintf "Byte %x" z) done
    BigInteger(System.BitConverter.ToUInt32(bytes, 0))

let get_f64_bits (f64:double) =
    let bytes = System.BitConverter.GetBytes f64
    BigInteger(System.BitConverter.ToUInt64(bytes, 0))

//
// Make a floating point constant or net from string.   
//
let xi_fps width (fps0:string) =
    let signed = FloatingPoint
    let length = []
    let constantfp = (fps0.[0] >= '0' && fps0.[0] <= '9' ) || (fps0.[0] = '.') || (fps0.[0] = '-')
    let fps1 = if width <> 64 && fps0.[strlen fps0 - 1] <> 'f' then fps0 + "f" else fps0
    if constantfp && width=32 then   fps_core fps1 width (XC_float32 (float32 fps0))
    elif constantfp && width=64 then fps_core fps1 width (XC_double  (double fps0))
    elif constantfp then sf (sprintf "unsupported float size %i" width)
    else
        let io = LOCAL
        let (nn, ov) = netsetup_start fps1
        match ov with
            | Some ff ->
                let f2 = lookup_net2 ff.n
                assertf(length = f2.length) (fun () -> ff.id + ": xi_double new len " + sfold i2s64 length + " cf " + sfold i2s64 f2.length)
                cassert(signed = ff.signed, "xi_double new signed")
                assertf(width = ff.width) (fun () -> ff.id + ": xi_double new width " + i2s(ff.width))
                cassert(io = f2.xnet_io, "xi_double new io")
                ff
            | None ->
                let pol = ref false
                let dir = ref false
                let vt = ref V_REGISTER
                let states = ref None
                let e2 = 0
                let ff =
                    { 
                        n= nn
                        rh= 0I
                        rl= 0I
                        id= fps1
                        width=width
                        constval=      []  // Floating point net - has null constants list.
                        signed=        signed
                        is_array=      not_nullp length
                    }
                let f2 =
                    { 
                        ats=           (if constantfp then sf " unreachable " else [])
                        length=        length
                        dir=           !dir
                        pol=           !pol
                        xnet_io= io 
                        vtype= !vt
                    }
                ignore(netsetup_log (ff, f2))
                ff


// Parse numbers of the form S32'-1212122  and perhaps -S32'1212122
let sized_bignum_parse str0 =
    //dev_println (sprintf "sized_bignum_parse >%s<" str0)
    let bnp str =
        try (BigInteger.Parse str)
        with _ -> sf("malformed manifest constant or sized_bignum_parase in " + str0)

    if str0 = "" then (None, None, 0I)
    elif string_containsp ''' str0 then
        let (signo, str) =
            if str0.[0] = 'S' then (Some Signed, str0.[1..])
            elif str0.[0] = 'U' then (Some Unsigned, str0.[1..])
            else (None, str0)

        //dev_println (sprintf "sized_bignum_parse 2 >%s<" str)
        let (sizeo, str) =
            let rec scan_size sofar pos =
                if str.[pos] = ''' then (sofar, pos+1)
                elif isDigit str.[pos] then
                    let sofar = sofar * 10 + (int)str.[pos] - (int)'0'
                    dev_println (sprintf "sofar %i %i" pos sofar)
                    scan_size sofar (pos+1)
                else sf(sprintf "Bad size digit in sized bignum %s" str0)
            //dev_println (sprintf "sized_bignum_parse 3 >%s<" str)
            if str = "" then (None, str) else
                let (width, p) = scan_size 0 0
                (Some width, str.[p..])

        (signo, sizeo, bnp str)
    else (None, None, bnp str0)
       


let xi_float32 fv = X_bnet(fps_core (sprintf "%Af" fv) 32 (XC_float32 fv))

let xi_double dv = X_bnet(fps_core (sprintf "%A" dv) 64 (XC_double dv))

let rec get_float_bits prec = function
    | X_bnet ff ->
        //let f2 = lookup_net2 ff.n
        match ff.constval with
                | XC_bnum(w, bn, _) :: _   -> bn
                | XC_float32 f32 :: _      -> 
                    let bytes = System.BitConverter.GetBytes f32
                    BigInteger(System.BitConverter.ToUInt32(bytes, 0))

                | XC_double f64 :: _       -> 
                    let bytes = System.BitConverter.GetBytes f64
                    BigInteger(System.BitConverter.ToUInt64(bytes, 0))

                | other -> sf ("get_float_bits other constval form in " + netToStr (X_bnet ff))
        
    | X_bnum(_, bn, _) -> bn
    | X_num n -> BigInteger(n)    
    | other ->
        let _ = vprintln 3 (sprintf "get_float_bits other general form in key=%s other=%s" (xkey other) (netToStr other))
        BigInteger(int64(xi_manifest64 "get_float_bits" other))

and perform_bnum_arith (prec:precision_t) oo l r =  // l and r are constant bigInteger arguments.
    let a0 =
        match oo with
            | V_xor    -> bnum_bitxor l r
            | V_bitor  -> bnum_bitor l r
            | V_bitand -> bnum_bitand_unspec_width l r

            | V_exp    -> BigInteger.Pow(l, int32(*System.Convert.ToInt32*)r)
            | V_divide ->
                match prec.signed with
                    | Signed -> l / r
                    | Unsigned ->
                        // This is not a good unsigned divide semantic !
                        // This can be explored with a test in Kiwi.
                        let abs x = if x < 0I then -x else x
                        (abs l) / (abs r)

            | V_mod    -> l % r
            | V_times  -> l * r
            | V_minus  -> l - r
            | V_plus   -> l + r

            | V_lshift ->
                let ans = l * (BigInteger.Pow(2I, int32(*System.Convert.ToInt32*)r))
                //dev_println (sprintf "bnum_arith lshift %s << %s --> %s" (tnumToX_bn l) (tnumToX_bn r) (tnumToX_bn ans))        
                ans
            | V_rshift ss -> bnum_rshift ss prec l r

            | oo -> sf (f1o3(xToStr_dop oo) + " other match xgen_wide_logic")
    xi_bnum a0


and xgen_double_arith oo (l:double) (r:double) =
    match oo with
//  | (V_exp)      -> xi_double(BigInteger.Pow(l, int32(*System.Convert.ToInt32*)r))
    | (V_divide _) -> xi_double(l / r)
    | (V_times _)  -> xi_double(l * r)
    | (V_minus)    -> xi_double(l - r)
    | (V_plus)     -> xi_double(l + r)
//  | (V_lshift)   -> xi_double(l * (BigInteger.Pow(2I, int32(*System.Convert.ToInt32*)r)))
//  | (V_rshift ss)-> xi_double(l / (BigInteger.Pow(2I, int32(*System.Convert.ToInt32*)r))) // arith needs round -inf for -ve.
    | (oo) ->
        let prec = { widtho=Some 64; signed=Unsigned }
        vprintln 3 (sprintf "xgen_double_arith: non-arith operator oo=%A, treating as 64-bit field." oo)
        let (l, r) = (get_f64_bits l, get_f64_bits r)
        let ans = perform_bnum_arith (prec:precision_t) oo l r 
        ans

and xgen_float_arith oo (l:float32) (r:float32) =
    match oo with
    | V_divide   -> xi_float32(l / r)
    | V_times    -> xi_float32(l * r)
    | V_minus    -> xi_float32(l - r)
    | V_plus     -> xi_float32(l + r)
    | oo         ->
        let _ = vprintln 3 (sprintf " non-arith other operator match xgen_float_arith oo=%A, treating as 32-bit field. l=%f r=%f" oo (l) (r))
        let prec = { widtho=Some 32; signed=Unsigned }


        let (l, r) = (get_f32_bits l, get_f32_bits r)
        let _ = vprintln 3 (sprintf " non-arith other operator match xgen_float_arith oo=%A, treating as 32-bit field. hex l=%s r=%s" oo (tnumToX_bn l) (tnumToX_bn r))        
        let ans = perform_bnum_arith (prec:precision_t) oo l r 
        ans





// get_init:
//   Constants have an intrinsic initial value.
//   Certain other nets may have an initial value that is assigned on reset.
//
// This function should perhaps be named get_reset_or_const_value and it should tell us which it returns, or perhaps even an option pair.
and get_init msg arg = // Integer form only for now.
    match arg with
    | X_bnet ff ->
        //let f2 = lookup_net ff.n
        match ff.constval with
            | [] ->
                match at_assoc g_resetval (lookup_net2 ff.n).ats with
                    | None -> None  // An older form of constant value storage in init field. But this attribute is now intended for reset values, not constant values.
                    | Some ss ->
                        let (signo_, widtho_, vale) = sized_bignum_parse ss
                        Some vale
            | XC_string _ :: _ ->  sf (msg + " wrong type of initial value. A string found in " + ff.id)
            | XC_double f64 :: _ ->
                vprintln 0 (msg + " +++ wrong type of initial value. A F/P number found in " + ff.id)
                Some(BigInteger(int64 f64))
                
            | XC_float32 f32 :: _ ->
                vprintln 0 (msg + " +++ wrong type of initial value. A F/P number found in " + ff.id)
                Some(BigInteger(int64 f32))
            | XC_bnum(w, bn, _) :: _ -> Some bn

and xi_manifest_double msg arg =
    match arg with
        | X_bnet ff when not_nullp ff.constval ->
            match ff.constval with
                | XC_double dd :: _ -> (64, dd)
                | XC_float32 ff  :: _ -> (32, double ff)
                | other -> sf (msg + sprintf ": xi_manifest_double contval other %A" other) 
        | other -> sf (msg + sprintf ": xi_manifest_double other xkey=%s %A" (xkey other) other)

and xi_manifest_int msg arg =
    match arg with
        | X_num n           -> BigInteger n
        | X_bnum(_, bn, _)  -> bn
        | W_node(caster, V_cast cvtf,  [arg1], _) ->
            let bn = xi_manifest_int msg arg1
            match bn_masker (mine_prec g_bounda arg1).widtho caster bn with
                | X_bnum(prec, bn, _) -> bn
                | ans -> sf (sprintf "apply_constant_caster '%s' to " msg + xToStr arg + sprintf " a0=%A ans=%A" bn ans)
        | W_node(prec, V_plus, lst, _)  -> List.fold (fun c d -> c + xi_manifest_int msg d) 0I lst
        | W_node(prec, V_times, lst, _) -> List.fold (fun c d -> c * xi_manifest_int msg d) 1I lst
        | W_string(s, XS_withval x, _)  -> xi_manifest_int msg x
        | X_bnet ff ->
            //let ff = lookup_net2 ff.n
            match ff.constval with
                | [] ->
                    if  at_assoc "constant" (lookup_net2 ff.n).ats <> None  // old way
                    then valOf_or_fail "constant had no init" (get_init msg (X_bnet ff))
                    else sf(msg + sprintf ": xi_manifest_int : bnet not constant id=%s" ff.id)

                | XC_string _ :: _ ->  sf (msg + " (L2059) wrong type of initial value. String type found in " + ff.id)
                | XC_double dfp  :: _ -> BigInteger(int64 dfp)
                | XC_float32 sfp :: _ -> BigInteger(int64 sfp)
                | XC_bnum(w, bn, _) :: _ -> bn

        | X_pair(a, b, _) -> xi_manifest_int msg b

        | W_string _ -> sf("The supplied string does not embed a manifest constant integer: " + xkey arg + " " + xToStr arg + " in " + msg)
        | other -> sf("manifest constant required instead of " + xkey other + " " + xToStr other + " in " + msg)
                        

and xi_manifest m = function
    | X_num n -> n
    | n ->
    let a = (xi_manifest_int m n)
    try int(a)
    with overflow_ -> sf(sprintf "xi_manifest(32) overflow: '%A' -> '%A' " n a  + m + ": " + xkey n + " " + xToStr n)

and xi_manifest64 m = function
    | X_num n -> int64 n
    | n ->
    let a = (xi_manifest_int m n)
    try int64(a)
    with overflow_ -> sf(sprintf "xi_manifest(32) overflow: '%A' -> '%A' " n a  + m + ": " + xkey n + " " + xToStr n)



let set_net_resetval site initval = function
    | X_bnet ff ->
        let f2 = lookup_net2 ff.n
        let f2 = { f2 with ats= nap_update f2.ats g_resetval (xToStr initval) }
        let (found, _) =  g_netbase_nn.TryGetValue ff.n
        if found then ignore(g_netbase_nn.Remove ff.n)
        g_netbase_nn.Add(ff.n, (ff, f2))
        ()
    | other -> sf(site + sprintf ": attempt to set initval of non net " + xToStr other)



// The reset value of a net. Some doubling up of code here!
// TODO floating point resets are needed too
let resetval_raw = function // perhaps use part of  meox.get_init instead please
    | X_bnet ff ->
        let f2 = lookup_net2 ff.n
        match at_assoc g_resetval (lookup_net2 ff.n).ats with
            | None -> 0I
            | Some kk ->
                if f2.xnet_io = INPUT then hpr_yikes(sprintf "resetval: Verilog INPUT net with reset value cannot be supported: id='%s'  value='%s'" (netToStr (X_bnet ff)) kk)

                //vprintln 0 (sprintf "resetval value %s" kk)
                atoi kk
    | (_) -> 0I // Default to zero



let resetval_o = function // perhaps use meox.get_init instead please
    | X_bnet ff ->
        let f2 = lookup_net2 ff.n
        match at_assoc g_resetval f2.ats with
            | None -> None
            | Some kk ->
                if f2.xnet_io = INPUT then
                    hpr_yikes(sprintf "resetval1: Verilog INPUT net with reset value cannot be supported: id='%s'  value='%s'" (netToStr (X_bnet ff)) kk)
                    None
                else
                    //vprintln 0 (sprintf "resetval1 value %s" kk)
                    Some(xi_bnum(atoi kk))
    | (_) -> None

let xi_resetval arg = xi_bnum(resetval_raw arg)

                     
let xkToStr x = xkey x + "\\" + xToStr x


// We do not want these to use weak pointers. Ultimately perhaps yes, but there's plenty of capacity.
// This stores all boolean expressions, not just 'literals'
let blit_save nn ba = Array.set blits nn (Some ba)


//
// TODO: If the resulting expression is 0<=x<=maxv where x is an enumeration that always lies in that range then return true.
//
let xi_linp_leaf(av, LIN(pol, lst)) = sf "used linp"
#if USE_LINP
    cassert(g_use_linp, "Linp g_use_linp")
    if constantp av then
        let vv = xi_manifest "xi_linp_leaf" av
        //vprintln 0 ("xi_linp_leaf constant = " + i2s vv + "  lst=" + sfold i2s lst)
        let rec k1 pol = function
            | [] -> if (pol) then X_true else X_false
            | h::tt -> if vv < h then (if pol then X_true else X_false) else k1 (not pol) tt
        k1 pol lst
    elif (lst=[]) then (if pol then X_true else X_false)
    else
    let h = hash_digest(hexp_hash av, List.fold (+) 0 lst)
    let lans = LIN(pol, lst) (* use AS? *)
    let rec scan ov =
            match ov with
                | [] -> None
                | (W_linp(av', lans', _)::t) -> if (av=av' && lans=lans') then Some(hd ov) else scan t
                | (_::t) -> scan t
    let ovl = mdig<hbexp_t> (wb_SPLAY) h
    let ov = scan ovl
    if ov<>None then valOf ov
    else
        let nn = nxt_it()
        let nv = W_linp(av, lans, { hash=h; n=nn; sortorder=x2order av  + g_mya_limit })
        blit_save nn nv
#if NNTRACE
        vprintln 0 ("write linp nn=" + i2s nn + " " + i2s h + " ovl =" + sfold xbToStr ovl)
#endif
        if ovl<>[] then ignore(wb_SPLAY.Remove h)
        wb_SPLAY.Add(h, Djg.WeakReference(nv::ovl))
        nv
#endif




let arith0_pred arg = // Check whether an argument is manifestly zero.
    match xi_monkey arg with
        | Some(false) -> true
        | other  -> false

let arith1_pred = function
    | X_num 1   -> true
    | X_bnum(_, bn, _) -> bn.IsOne
    | X_bnet ff ->
        //let f2 = lookup_net2 ff.n
        match ff.constval with
            | XC_float32(sp) ::_ -> (sp = 1.0f)
            | XC_double(dp) ::_  -> (dp = 1.0)
            | XC_bnum(_, bn, _)::_ -> bn.IsOne
            | _                  -> false
    | other -> false

let arith_m1_pred = function
    | X_num -1   -> true
    | X_bnum(_, bn, _) -> bn = 0I-1I
    | X_bnet ff ->
        //let f2 = lookup_net2 ff.n
        match ff.constval with
            | XC_float32(sp) ::_ -> (sp = -1.0f)
            | XC_double(dp) ::_  -> (dp = 1.0)
            | XC_bnum(_, bn, _)::_ -> (bn = 0I-1I)
            | _                  -> false
    | other -> false

// A branding tag so that sorted and unsorted data do not get mixed by programming errors.
type xiSorted<'T> =
    | Xist of 'T list
    | Xi_filler

// Remove branding tag
let xi_valOf = function
    | Xist l -> l
    | Xi_filler -> sf "filler should not be instantiated"



let xis_length = function
    | Xist lst -> length lst

let xi_b[] = Xist []


let xiSort lst =
    let rec elide_duplicates = function
        | (a::b::c) -> if x2nn a = x2nn b then elide_duplicates(b::c) else a :: elide_duplicates(b::c)
        | (other) -> other
    let lst' = List.sortWith xi_order_pred lst
    let lst'' = elide_duplicates lst'
    Xist lst''


let ix_badd (Xist l) n =  (* Maintain an orderd xiS list - bool version *)
    let nn = xb2nn n
    let rec i = function
        | [] -> [n]
        | h::t ->
            if nn = xb2nn h then h :: t
            else if xi_border_pred n h >= 0 then n :: h :: t
            else h :: (i t)
    Xist(i l) 


let ix_add (Xist l) n =  // Insert while maintaining an ascending order xiS list.
    let nn = x2nn n
    let rec i =
        function
            | [] -> [n]
            | h::t ->
                if nn = x2nn h then h :: t
                elif xi_order_pred n h <= 0 then n :: h :: t
                else h :: (i t)
    Xist(i l) 


(*
 * Support/driven list: maintained in ascending order.
 * If any entry is full range, represented by [], then we do not keep any further details.
 *)
type sd_t = (hexp_t * int64 list)

let sd_join (kl1, kl) = if kl=[] || kl1=[] then [] else lst_union kl1 kl // We do not maintain order for the second component. 


// Insertion sort in order.
let sd_add l (n, kl) = 
    let nn = x2nn n
    let rec i x =
            match x with
                | [] -> [(n, kl)]
                | ((n1, kl1)::t) ->
                    if nn = x2nn n1 then (n1, sd_join(kl1, kl)) :: t
                    else if xi_order_pred n n1 >= 0 then (n, kl) :: (hd x) :: t
                    else (hd x) :: (i t)
    (i l)


let sdSort l = List.fold sd_add [] l



(*
 * Subtraction of lane items :
 *  rhs full range (denoted with []l) removes item
 *  lhs full range and specific rhs not implemented.
 *)
let sd_subtract =
    function
        | ([], b) -> []
        | (a, []) -> a
        | (a, b) ->
        let rec s =
            function
                | ([], r) -> []
                | (r, []) -> r
                | ((c, kc)::at, (r, kr)::pt) -> 
                   let cn = x2nn c
                   let rn = x2nn r
                   if cn=rn && kr=[] then s(at, pt)
                   else if cn=rn && kc=[] then muddy "sd_subtract lane items"
                   else if cn=rn then
                       let delta = list_subtract(kc, kr)
                       if delta=[] then s(at, pt) else (c, delta) :: s(at, pt)
                   else if xi_order_pred c r >= 0 then (c, kc) :: s(at, (r, kr)::pt)
                   else s(at, pt)
        s(a, b)


(*
 * Intersection of lane items: either being a wild card retains all of the other
 *)
let sd_join_is(kc, kr) = if kc=[] then kr else if kr=[] then kc else list_intersection(kc, kr)

let sd_intersection = function
    | ([], _) -> []
    | (_, []) -> []
    | (a, b)  ->
        let rec ii = function
            | ([], _) -> []
            | (_, []) -> []
            | ((c,  kc)::at, (r, kr)::pt) -> 
                let xi_order_pred (a:int64) b = a-b
                if c=r then (c, sd_join_is(kc, kr)) :: ii(at, pt)
                elif xi_order_pred c r >= 0L then ii(at, (r, kr)::pt)
                else ii((c, kc)::at, pt)
        ii(a, b)



// Return one member from the intersection (an example member).
let rec sd_intersection_example a b =
    let subs_intersection_pred (kc:int64 list) (kr:int64 list) =
        let ans = list_intersection_pred(kc, kr)
        //let _ = vprintln 3 (sprintf "subs_intersection_pred %s cf %s gives %A" (sfold i2s64 kc) (sfold i2s64 kr) ans)
        ans
    let ip = sd_intersection_example
    if nullp a || nullp b then []
    else
        let (c, kc) = hd a
        let (r, kr) = hd b
        if x2nn c = x2nn r then
            if kc=[] || kr=[] || subs_intersection_pred kc kr // The empty subs list denotes all subs.
            then [c]
            else ip (tl a) (tl b)
        elif xi_order_pred c r >= 0 then ip (tl a) b
        else ip a (tl b)

let sd_intersection_pred a b = not_nullp(sd_intersection_example a b)

let sd_union(a, b) =
        let rec u =
            function
                | ([], r)  -> r
                | (r, [])  -> r
                | ((c, kc)::at, (r, kr)::pt) -> 
                   if x2nn c = x2nn r then (c, sd_join(kc, kr))::u(at, pt)
                   else if xi_order_pred c r >= 0 then (c, kc) :: u(at, (r, kr)::pt)
                   else (r, kr) :: u((c, kc)::at, pt)
        u(a, b)


let rec sd_Union =
    function
        | (a :: b :: t) -> sd_Union(sd_union(a, b) :: t)
        | []            -> []
        | [item]        -> item



let rec sd_lookup n = function
    | []            -> None
    | ((n', kl)::t) -> if n=n' then Some kl else sd_lookup n t





(*--------------------------------------------------------------------------*)
//
// The xi_xxx list operations operate on sorted lists of nets (or other expressions)
// and do set operations in linear time.
let xi_union = function
        | (Xist [], b) -> b
        | (a, Xist []) -> a
        | (Xist a, Xist b) ->
              let rec u =
                  function
                      | ([], r) -> r
                      | (r, []) -> r
                      | (c::at, r::pt) -> 
                         if x2nn c = x2nn r then c::u(at, pt)
                         else if xi_order_pred c r >= 0 then c :: u(at, r::pt)
                              else r:: u(c::at, pt)
              Xist(u(a, b))

let rec xi_Union = function
    | (a :: b :: t) -> xi_Union(xi_union(a, b) :: t)
    | [] -> Xist []
    | [item] -> item



let xi_subtract = function
    | (Xist [], b) -> Xist []
    | (a, Xist []) -> a
    | (Xist a, Xist b) ->
              let rec s =
                  function
                      | ([], r) -> []
                      | (r, []) -> r
                      | (c::at, r::pt) -> 
                          if x2nn c = x2nn r then s(at, pt)
                          else if xi_order_pred c r >=0 then c:: s(at, r::pt)
                               else s(at, pt)
              Xist(s(a, b))

//
//
//
let xi_intersection = function
        | (Xist [], _) -> Xist []
        | (_, Xist []) -> Xist []
        | (Xist a, Xist b) ->
            let vpl x = netToStr x + "/" + i2s(x2order x)
            let errorf (x,y) =
                vprintln 0 ("Sorting error in xi_intersection " + i2s x + " " + i2s y)
                let _ = reportx 0 "xi_intersection in a" vpl a
                let _ = reportx 0 "xi_intersection in b" vpl b
                sf "Sorting error in xi_intersection"

            let offc chkval lst =
                 if nullp lst then 0
                 else
                     let nv = x2order(hd lst)
                     let _ = assertf(nv>chkval) (fun() -> errorf(nv, chkval))
                     nv

            let rec i =
                 function
                     | ([], _, _, _) -> []
                     | (_, [], _, _) -> []
                     | (c::at, r::bt, ao, bo) -> 
                     let eqf = x2nn c = x2nn r
                     // vprintln 0 ("eqf for " + vpl c + " cf " + vpl r + " returns " + boolToStr eqf)
                     if eqf then c :: i(at, bt, offc ao at, offc bo bt)
                     else if ao < bo then i(at, r::bt, offc ao at, bo)
                          else i(c::at, bt, ao, offc bo bt)

            //let _ = reportx 0 "xi_intersection in a" vpl a
            //let _ = reportx 0 "xi_intersection in b" vpl b
            let ans = i(a, b, x2order(hd a), x2order(hd b))
            //let _ = reportx 0 "xi_intersection res" vpl ans
            Xist ans

let xi_intersection_pred = function
        | (Xist [], _) -> false
        | (_, Xist []) -> false
        | (Xist a, Xist b) ->
             let rec i =
                 function
                     | ([], _) -> false
                     | (_, []) -> false
                     | (c::at, r::pt) -> x2nn c = x2nn r ||(if xi_order_pred c r >= 0 then i(at, r::pt) else i(c::at, pt))
             i(a, b)

let isscalar = function
    | X_bnet ff -> not ff.is_array
    | X_net _   -> true
    | X_blift _ -> true
    | (_)       -> false

(* Compare_reflect gives the l exhanged with r equivalent for a comparisonip.  dged and dgtd are normally highly deprecated, but have a uses as i/o forms and in preparation of a linprog range. *)
let compare_reflect = function
      | (V_deqd) -> V_deqd
      | (V_dned) -> V_dned
      | (V_dled signed) -> V_dged signed
      | (V_dltd signed) -> V_dgtd signed
      | (V_dgtd signed) -> V_dltd signed
      | (V_dged signed) -> V_dled signed
      | (_) -> sf("compare reflect other")


let g_unaddressable = -1000L


//
// Forms xi_asubsc(lhs, subsc) but checks array is in range if present.
//
let safe_xi_asubsc m (lhs, subsc) =
    match (lhs, subsc) with
        | (W_string _, subsc) -> ix_asubsc lhs subsc
        | (lhs, X_pair(X_pair(b, entries, _), e, _)) ->             
            if (constantp e) then 
                let l = xi_manifest64 "safe_xi_subsc lower/base" b
                let j = xi_manifest64 "safe_xi_asubsc size" e
                    (* NB: as Kiwi initially generates code, for anything with width w that is larger than a single byte, the upper bound u is actually w-1 lower than the value used here. However, after conerefine repacking, and for other hpr users, w does not matter in this check. *)
                let u = l + (xi_manifest64 "safe_xi_subsc r" entries)-1L
                if l <> g_unaddressable && (j < l || j > u)
                       then sf(m + ": safe_xi_asubsc: subscript out of range " + xToStr lhs + " " + xToStr subsc + " in integer terms " + i2s64 l  + " <= " + i2s64 j + " <= " + i2s64 u)
            ix_asubsc lhs subsc


        |  (lhs, subsc) -> ix_asubsc lhs subsc


// Euclids algorithm:
let rec euclid_gcd (a:int64) (b:int64) =
    if a=b then a
    else
        let a' = a % b
        if a' = 0L then b else euclid_gcd b a'


let rec euclid_gcd_bn (a:BigInteger) (b:BigInteger) =
    if a=b then a
    else
        let a' = a % b
        if a' = 0I then b else euclid_gcd_bn b a'


// Find factors of a natural number, using memoised trial division method.
let Factors = Array.create g_mya_limit None;


let rec find_factors n = 
    if n<0 then find_factors(0-n)
    else if n=0 || n>= g_mya_limit then sf("Factors of " + i2s n)
    else 
        let ov = Factors.[n]
        if ov<>None then valOf ov
        else
(*      let _ = print("Factors for " + i2s n + " are ") *)
           let rec fscan(k, c) =
               if k <= 1 then c else let c' = if (n % k) =0 then k::c else c in fscan(k-1, c')
           let ans = fscan(n/2, [n]) 
           (*let _ = print(" ans " + sfold i2s ans  + "\n")*)
           let _ = Array.set Factors  n (Some ans)
           ans




(*
 * Should use sml splay trees for this!
 *)
let satnet(id:string) = id.[0] = 'x'

let vsatnet = function
    | X_undef     -> true
    | X_net(d, _) -> satnet(d)
    | (_)         -> false

(*
 *  find rightmost element in a tree of pairs.  This is the value of a decorated expression.
 *)
let rec undecorate = function
    | X_pair(l, r, _) -> undecorate r
    | oo -> oo

let xi_getstring = function
    | W_string(_, s, _) -> s
    | other -> sf("xi_getstring other: " + xToStr other)



let logcounter = ref 0

let W_node_order = function
    |  V_plus      -> g_mya_limit * 1
    |  V_times _   -> g_mya_limit * 2
    |  V_divide _  -> g_mya_limit * 3
    |  V_bitor     -> g_mya_limit * 4
    |  V_bitand    -> g_mya_limit * 5
    |   _ -> g_mya_limit * 6


let wab_node_order = function
    | V_band -> g_mya_limit * 1
    | V_bor -> g_mya_limit * 2
    | _ -> g_mya_limit * 3


let wb_node_order = function
        | V_deqd -> g_mya_limit * 3
        | _ -> g_mya_limit * 9

let nodes_reused = ref 0;


let mya_stats(vd) =
    if vd>=4 then vprintln 4 ("Profile report: queries=" + sfold (fun x->i2s(!x)) [profile_q0;profile_qa;profile_q2])
    if vd>=4 then vprintln 4 ("myastats= ru=" + i2s (!nodes_reused) + "/al=" + i2s (!mya_splay_nn/ mya_f_fieldwidth))
    ()


(*
 *   W_node operators, commutative, associative : V_plus V_times V_bitor V_bitand V_xor - operans sorted when commutative pre call to write
 *)
let xint_write_node prec oo lst = 
    let key = (prec, oo, map x2nn lst)
    let (found, ov) = node_SPLAY.TryGetValue key
    if found then
        mutinc nodes_reused 1
        ov
    else
        let nn = nxt_it()
        let ans = W_node(prec, oo, lst, { (*hash__=0;*) n=nn; sortorder=nn + (W_node_order oo) })
#if NNTRACE
        vprintln 0 (sprintf "write node nn=%i  ovl=%s " nn (sfold xToStr lst))
#endif
        let _ = node_SPLAY.Add(key, ans)
        ans

let xi_apply_cf cf (gis, lst) = 
    let key = (fst gis, cf, map x2nn lst)
    let (found, ov) = apply_SPLAY.TryGetValue key
    if found then ov
    else
        let nn = nxt_it()
        let ans = W_apply(gis, cf, lst, { (*hash__=0;*) n=nn; sortorder=nn })
#if NNTRACE
        vprintln 0 (sprintf "meox: allocate xapply nn=%i for " nn + xToStr ans)
#endif
        apply_SPLAY.Add(key, ans)
        ans


let g_null_callers_flags =
    {
        externally_instantiated_block=       false // Here held as a property of the call not the callee, but really it is both.
        loaded_ip_=                          false
        tlm_call=                            None
    }

let xi_apply (gis, lst) = xi_apply_cf g_null_callers_flags (gis, lst)
    
let ix_abs = function
    | X_num n   -> xi_num(abs n)
    | X_bnum(prec, bn, _) -> gec_X_bnum(prec, BigInteger.Abs bn)
    | e -> xi_apply(g_hpr_abs_fgis, [e])

let xi_max = function
        | (X_num n, X_num m) -> xi_num(max n m)
        | (e, f) -> xi_apply(g_hpr_max_fgis, [e; f])

let xi_min = function
        | (X_num n, X_num m) -> xi_num(min n m)
        | (e, f) -> xi_apply(g_hpr_min_fgis, [e; f])

(*
 *   wb_node operators, commutative, associative : V_bor and so on
 *)
let xint_write_bnode(key, lst, inv) = 
    let kk = (key, map xb2nn lst)
    let yielder nv =
        if not inv then nv
        else match nv with
              | W_bnode(key, lst, false, meo) -> W_bnode(key, lst, true, { meo with n= -meo.n })
    let (found, ov) = bnode_SPLAY.TryGetValue kk
    if found then yielder ov
    else
        let nn = nxt_it()
        let meo =  { (*hash__=0;*) n=nn; sortorder=nn + (wab_node_order key); }
        let nv = W_bnode(key, lst, false, meo)
#if NNTRACE
        vprintln 0 ("meox: allocate bn nn=" + i2s nn + "  for " + xbToStr nv)
#endif
        blit_save nn nv
        bnode_SPLAY.Add(kk, nv) // Djg.WeakReference(nv)
        yielder nv 

let xint_write_bitsel arr b inv = 
    let kk = (x2nn arr, b)
    let (found, ov) = bitsel_SPLAY.TryGetValue kk

    let yielder nv =
        if not inv then nv
        else match nv with
               | W_bitsel(arr, b, false, meo) -> W_bitsel(arr, b, true, { meo with n= -meo.n })

    if found then yielder ov
    else
        let nn = nxt_it()
        let meo =  { (*hash__=0;*) n=nn; sortorder=x2order arr  }
        let nv = W_bitsel(arr, b, false, meo)
#if NNTRACE
        vprintln 0 ("write bitsel nn=" + i2s nn + " it=" + xbToStr nv)
#endif
        blit_save nn nv
        bitsel_SPLAY.Add(kk, nv) // Djg.WeakReference(nv::ovl)
        yielder nv

let xint_write_cast caster cvtf arg = 
    let kk = (caster, cvtf, x2nn arg)
    let (found, ov) = g_casting_SPLAY.TryGetValue kk
    if found then ov
    else
        let nn = nxt_it()
        let meo = { (*hash__=0;*) n=nn; sortorder=x2order arg  }
        let nv = W_node(caster, V_cast cvtf, [arg], meo)
#if NNTRACE
        vprintln 0 ("write caster nn=" + i2s nn + " it=" + xToStr nv)
#endif
        g_casting_SPLAY.Add(kk, nv) // Djg.WeakReference(nv::ovl)
        nv




// Floating point takes precedence always.
// For commutative operators, unsigned takes precedence over Signed only if the rank of the Unsigned arg is wider, otherwise
// we make a signed operation unless both are unsigned.        .
//
// As a cast, the outermost signness takes precedence (except for floatingpoint) - this is provided as the lhs operand and lprec0 is asserted.
//
//   
// In terms of width, the narrowest width always takes precedence, not the outermost.   


// Not always diadic - orred is one that is not.
let xint_write_bdiop key lst inv = 
    let kk = (key, map x2nn lst)
    let (found, ov) = bdiop_SPLAY.TryGetValue kk
    //vprintln 0 (i2s(length ovl) + " write bdiop length of ovl " + f1o4(xbToStr_dop key))
    let yielder nv =
        if not inv then nv
        else
             // We prefer to place equality before inequality in conjunctions, so a crude weighting here:
            let pref = 10000
            match nv with
                | W_bdiop(key, lst, false, meo) -> W_bdiop(key, lst, true, { meo with n= -meo.n; sortorder= meo.sortorder+pref})

    if found then yielder ov
    else 
                let nn = nxt_it()
                let meo =  { (*hash__=0;*) n=nn; sortorder=wb_node_order key;  }
                let nv = W_bdiop(key, lst, false, meo)
                blit_save nn nv
#if NNTRACE
                vprintln 0 ("write bdiop nn-" + i2s nn + ", it=" + xbToStr nv)
#endif
                bdiop_SPLAY.Add(kk, nv) //  Djg.WeakReference(nv::ovl)
                yielder nv



let linp_var = function
        | W_linp(v, l, _) -> v
        |  _ -> sf "linp_var"


let islinp = function
        | W_linp(v, l, _) -> true
        |  _ -> false


(* Insert sort with duplicates being deleted for logical ops (exc xor). *)
// Note x2nn ordering is not sort ordering.
let rec insert_bsort lg = function
    | [] -> []
    | [item] -> [item]
    | h::t ->
        let lt A1 A2 = 
           let a1=xb2order A1
           let a2=xb2order A2
           (a1<a2) || (a1=a2 && xb2nn A1 < xb2nn A2)
        let t' = insert_bsort lg t
        let rec f = function
            | [] -> [h]
            | (b::bs) -> if xb2nn h = xb2nn b && lg then b::bs else if lt h b then h::b::bs else b :: f bs 
        f t'


(* Insert sort with duplicates being deleted for logical ops. *)
// Note x2nn ordering is not sort ordering.
let rec insert_sort lg =
    function
        | [] -> []
        | [item] -> [item]
        | (h::t) ->
              let lt A1 A2 = 
                      let a1=x2order A1
                      let a2=x2order A2
                      in (a1<a2) || (a1=a2 && x2nn A1 < x2nn A2)
              let t' = insert_sort lg t
              let rec f =
                  function
                      | [] -> [h]
                      | (b::bs) -> if x2nn h = x2nn b && lg then b::bs else if lt h b then h::b::bs else b :: f bs 
              in f t'
;

(* Test it : insert_sort [ 12, 4, 5, 66, 33, 44, 99, 55, 55, 44, 22] *)


(*
 *  We need to ensure v and X(v) have adjacent indecies to preserve ordering under the ssmg revert operation.
 *) 

let lateconstant id = 
    if sf "symbolic_check (explode id)" then
        atoi id
    else (vprintln 0 (*orange_error*)("bad symbolic constant " + id); 0I)

// atoi - strictly ascii to integer, but here we convert our integer literals to BigInteger types.
let rec lc_atoi = function
    | X_num n -> BigInteger n
    | X_pair(l, r, _) -> lc_atoi r
    | W_string(s, XS_withval x, _) -> lc_atoi x
    | X_bnum(_, bn, _) -> bn 
    | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | [] ->
                    let v = at_assoc "constant" (lookup_net2 ff.n).ats // old way
                    let _ = if v = None then sf ("lc_atoi X_bnet has no constant attribute: " + netToStr (X_bnet ff))
                    BigInteger.Parse(valOf v)
                | XC_string _ :: _
                | XC_double _ :: _
                | XC_float32 _ :: _ ->  sf ("(L2697) Wrong type of initial value in " + ff.id)
                | XC_bnum(w, bn, _) :: _ -> bn

    | W_node(prec, V_cast cvtf, [arg], _) -> // When a number is cast to a range it cannot support we need to clip or mask it
        let v0 = lc_atoi arg

        match prec.widtho with
            | None -> v0 // Ignored unsigned or float conversions for now when no width.
            
            | Some w ->
                let _ = 
                    if prec.signed = Signed then
                        let hm = himask (w-1)
                        let _ = if v0 < -(hm+1I) || v0 > hm then hpr_warn(sprintf "masking %A is out of signed range %A to %A " v0 (0I-(hm+1I)) hm)
                        ()                                                 
                    else 
                        let hm = himask w
                        let _ = if v0 <0I || v0 > hm then hpr_warn(sprintf "masking %A is out of unsigned range 0 to %A " v0 hm)
                        ()
                v0
                
    | other -> sf (netToStr other + ": lc_atoi other form: " + xkey other)


// Different expressions are in different strobe groups and so it is wrong for this function to extract the strobe group
// from a string, which it no longer does.
let enum_constate = function
    | other -> (None, xi_manifest_int "determine_efo/enum_constate" other) // or use lc_atoi ...?


let lc_atoi32 n = // ASCII to Int32
    let q = lc_atoi n
    int32(q) // may overflow!


let xi_uqstring s = xi_stringx (XS_unquoted 0) s

let xi_unit = xi_stringx (XS_unit) "x_unit"

let xi_tailmark = xi_stringx (XS_tailmark) "x_tailmark"


let x_eqc l r = constant_comp V_deqd (l, r)


// Normalise function: we prefer to store only one polarity of a boolean expression
// Take absolute value of a boolean expression and return whether it was negated as second arg.
//
let w_babs w =
    match w with
        | W_bnode(oo, lst, true, meo) -> (xint_write_bnode(oo, lst, false), true)
        // Could eliminate dned, dgtd and other deprecated forms in the next line (but assume instead that no such bdiop is ever made.
        | W_bdiop(oo, lst, true, meo) -> (xint_write_bdiop oo lst false, true)
        | W_bsubsc(A, s, true, meo) -> (xint_write_bsubsc(A, s, false), true)
        | W_bitsel(A, n, true, meo) -> (xint_write_bitsel A n false, true)
        
        | (X_true)  -> (X_true, false)
        | (X_false) -> (X_true, true)

        | W_cover(lst, meo) -> (w, false) // for now?

        | W_bnode(oo, lst, false, meo) -> (w, false)
        | W_bdiop(oo, lst, false, meo) -> (w, false)
        | W_bsubsc(A, s, false, meo)   -> (w, false)
        | W_bitsel(A, n, false, meo)   -> (w, false)
        | W_bmux(g, X_false, X_true, meo)  -> (g, false)
        | W_bmux(g, X_true, X_false, meo)  -> (g, true)
        | W_bmux(_, _, _, meo)             -> (w, false)

        | X_dontcare
        | W_linp _ -> (w, false)
        
//        | other -> sf("w_babs other: " + xbkey w + "  " + xbToStr other)


// Expresions containing pairs are often constant, but not designated as such by constantp since in some constexts (e.g. tagged array subscripts) they cannot be replaced
// with a direct manifiest literal (e.g. integer).  Generally their value is their second argument and it is useful to know if this is constant in other contexts.
// This predicate makes that distinction.
let rec fconstantp x =
      let ans =
          match x with
          | X_pair(l, r, _) -> fconstantp l && fconstantp r
          // We check on addition here too, since { } + { } is a common constant form used in annotated array subscripts.
          | W_node(prec, V_plus, lst, _) -> conjunctionate fconstantp lst
          | other -> constantp other
      //vprintln 0 ("Fconstantp " + xkey x + "   " + xToStr x + " gave " + boolToStr ans)
      ans

let rec int_fconstantp x =
      let ans =
          match x with
          | X_pair(l, r, _) -> int_fconstantp l && int_fconstantp r
          // We check on addition here too, since { } + { } is a common constant form used in annotated array subscripts.
          | W_node(prec, V_plus, lst, _) -> conjunctionate int_fconstantp lst
          | other -> int_constantp other
      //vprintln 0 ("int_Fconstantp " + xkey x + "   " + xToStr x + " gave " + boolToStr ans)
      ans

// Deep equals functions x_eq and x_beq: could use dp.  Conservative : returns false on un-decidable variable values.
// There are various constant expression forms that are semantically equal, such as false==0 or 3==3.0,
// and these may be nested under non-constant, ref-transp operators and casts, such a 3*x == 3.0*x, or abs(3)==abs(3.0).
let rec x_udeq l r =
    if x2nn l = x2nn r then Some true  // Shallow equality early stop
    elif fconstantp l && fconstantp r then Some(x_eqc l r <> 0)
    else
        let deep = function
            | X_bnet _
            | X_net _ -> Some (x2nn l = x2nn r)
            | W_asubsc(a, s, _) ->
                    (match r with 
                     | W_asubsc(a', s', _) -> if x_eq a a' && x_eq s s' then Some true else None
                     | _ -> None
                     )

            | W_node(cl, V_cast cvtf, [al], _) ->
                match r with
                    | W_node(cr, V_cast _, [ar], _) when cl=cr -> x_udeq al ar
                    | _ -> None
                    
            | X_pair(a, b, _) ->
                    (match r with 
                       | X_pair(a', b', _) -> if x_eq a a' && x_eq b b' then Some true else None
                       | _ -> None
                     )
            | W_apply(gis, cf, lst, _) -> // TODO can ignore out only args.
                    (match r with 
                       | W_apply(gis', cf', lst', _) ->
                                 if cf=cf' && gis=gis' &&  length lst = length lst' &&
                                       foldl (fun((a,b),c)->c && x_eq a b) true (List.zip lst lst')
                                 then  Some true else None
                       | _ -> None
                     )

            | W_query(g, tv, fv, _) ->
                       (match r with 
                       |  W_query(g', l', r', _) -> if x_beq(g,g') && x_eq tv l' && x_eq fv r' then Some true else None
                       | _ -> None
                       )
            | X_x(l1, ln, _) -> 
                      (match r with
                       | X_x(r1, rn, _) when rn=ln -> x_udeq l1 r1
                       | _ -> None
                      )

            | X_blift l1 -> 
                      (match r with
                       | X_blift r1 -> if x_beq(l1, r1) then Some true else None
                       | _ -> None
                      )

            | X_undef -> None // could use : if r=X_undef then Some true else None

            | W_node _ -> None
        
            | other -> if fconstantp other then Some false // Not all constants are comparable but this is a CONSERVATIVE routine.
                           else muddy("r=" + xToStr r + "\nx_eq other: " + xkey other + "  l=" + xToStr other + "\n compared with rkey=" + xkey r + " r=" + xToStr r + "\n")    
        deep l


// Boolean deep equals function.
and x_beq(l, r) =
    xb2nn l = xb2nn r ||
    let ee l =
         match l with
            | W_cover(lst, _) ->
                       (match r with 
                       | W_cover(lst', _) -> e_equiv lst lst'
                       | _ -> false
                       )
                
            | W_bdiop(V_orred, [l1], i, _) ->
                       (match r with 
                       | W_bdiop(V_orred, [r1], i', _) -> i=i' && x_eq l1 r1
                       | _ -> false
                       )

            | W_bmux(gg, X_true, X_false, _) ->  (* LEAF INVERTED MATCH *)
                 (match r with 
                        | W_bmux(g1, X_true, X_false, _) -> 
                                    let (l1, l2) = w_babs gg  (* NB: babs of gg or g1 should never make any difference: assert on this? *)
                                    let (r1, r2) = w_babs g1
                                    in l2=r2 && x_beq(l1, r1)
                        | W_bmux(g1, X_false, X_true, _) -> 
                                    let (l1, l2) = w_babs gg  (* NB: babs of gg or g1 should never make any difference: assert on this? *)
                                    let (r1, r2) = w_babs g1
                                    in l2<>r2 && x_beq(l1, r1)
                        | _ -> 
                                    let (l1, l2) = w_babs gg
                                    let (r1, r2) = w_babs r(* Only call to babs we need in theory, if all bmux's are rezzed using it.*)
                                    in l2 <> r2 && x_beq(l1, r1)

                  )


            | W_bmux(gg, X_false, X_true, _) -> (* Identity form : really a type convert.  *)
                 (match r with 
                       | W_bmux(g1, X_false, X_true, _) -> x_beq(gg, g1)
                       | W_bmux(g1, X_true, X_false, _) -> 
                                    let (l1, l2) = w_babs gg  (* NB: babs of gg or g1 should never make any difference: assert on this? *)
                                    let (r1, r2) = w_babs g1
                                    in l2 <> r2 && x_beq(l1, r1)
                       | _ -> x_beq(gg, r)
                  )

            | W_bmux(gg, ll, rr, _) ->  (* Use of weak pointers might mean that two subtrees are identical owing to different sort orders :: need a deep compare ?, (or if the tree ever gets out of descending order normal form. *) false

            | W_bdiop(oo, lst1, i, _) ->
                 (match r with 
                       |  W_bdiop(oo', lst2, i', _) -> oo=oo' && i=i' && length lst1 = length lst2 && foldl (fun ((p,q),c)-> c && x_eq p q) true (List.zip lst1 lst2)
                       | _ -> false
                  )
            | W_bnode(oo, lst1, i, _) ->
                 (match r with 
                       | W_bnode(oo', lst2, i', _) -> oo=oo' && i=i' && length lst1 = length lst2 && foldl (fun ((p,q),c)-> c && x_beq(p, q)) true (List.zip lst1 lst2)
                       | _ -> false
                       )
            | W_linp _ -> xb2nn l = xb2nn r

            | W_bitsel _ ->  xb2nn l = xb2nn r // Conservative routine.

            | X_dontcare -> false // Verilog semantic - dontcare treated as false.
            
            | other -> if bconstantp other then false
                       else muddy("x_beq other: " + xbkey l + " " + xbToStr l + " cf " + xbToStr r + "\n")    
    ee l


(* Symbolic or numeric deep equals functions x_eq and x_beq: could use dp.  Conservative : returns false on un-decidable variable values.  *)
and x_eq l r = x2nn l = x2nn r || x_udeq l r = Some true

let x_eq_undecorate l r = // Verbose variant of x_eq_undecorate for debug purposes.
    let ans = x_eq (undecorate l) (undecorate r)
    ans

let x_eq_lifted (l, r) = if x_eq l r then X_true else X_false



let x_eq_vd msg l r = // Verbose variant of x_eq for debug purposes.
    let ans = x_eq l r
    let naive_ans = (l=r)
    dev_println (msg + " x_eq_vd : " + xToStr l + " ==? " + xToStr r + sprintf " --> ans=%A (builtin equlity gives %A" ans naive_ans)
    ans

let x_eq_undecorate_vd msg l r = // Verbose variant of x_eq_undecorate for debug purposes.
    let ans = x_eq (undecorate l) (undecorate r)
    let naive_ans = (l=r)
    dev_println (msg + " x_eq_undecorate_vd : " + xToStr l + " ==? " + xToStr r + sprintf " --> ans=%A (builtin equlity gives %A" ans naive_ans)
    ans





let x_complements(l, r) =
    let (l1, l2) = w_babs l
    let (r1, r2) = w_babs r
    x_beq(l1,r1) && l2<>r2

(* Low numbers *)
let rec set_up_numbers n =
    if n < 0 then ()
    else 
      (Array.set numbers_positive n (X_num n);
       Array.set numbers_negative n (X_num(-n));
       set_up_numbers(n-1)
      )



let _ = set_up_numbers 4095;


// Some further numeric constants
let xi_zero = X_num 0
let xi_one  = X_num 1



(*
 * A bmux tree has descending sort values of its guards and each son is either a bmux, true or false.
 * Wrap_bmux wraps a leaf predicate.
 * Only positive polarity guards are allowed.
 *)
let xi_bmux _ = muddy "xi_bmux"
let wrap_bmux B =
    match B with
        | (W_bmux _) -> B
        | (X_true) -> B
        | (X_false) -> B
        | (v) -> 
          let (absv, flipf) = w_babs(v)
          let ff = if flipf then X_true else X_false
          let tt = if flipf then X_false else X_true
          xi_bmux(absv, ff, tt)


let knw_enum_constant arg = 
    match arg with
    | Enum_pv(n, _, (iconst, _, _), _) -> iconst
    | Enum_p n ->
        match deblit n with
            | W_bdiop(V_deqd, [l;r], false, _) when int_fconstantp r -> xi_manifest "knw_enum_constant " l
            | other -> sf ("knw_enum_constant " + xbToStr other)

            
let enumfToStr = function
    | Enum_pv(n, _, _, _)
    | Enum_p n -> xbToStr(deblit n)
    | Enum_not_an_enum  -> "Enum_not_an_enum"
    | arg -> arg.ToString()

let knownToStr (x:known_t) = sfold enumfToStr x.facts

let knw1ToStr = cubeToStr

let enum_negate = function
    | Enum_p n -> Enum_p(-n)
    | Enum_pv(n, v, mm, grp) -> Enum_pv(-n, v, mm, grp)    
    | Enum_none -> Enum_none
    | Enum_not_an_enum  -> Enum_not_an_enum
    //| other -> sf ("enum_negate other form: " + enumfToStr other)


let is_enum_var_n vv =
    let strobegroup = g_strobe_groups.lookup vv
    strobegroup

let is_enum_var x =
    match x2nn x with
        | vv ->
            let grp = is_enum_var_n vv
            if grp=None then None else g_strobe_group_members.lookup(valOf grp)
        //| None -> None



// See if a boolean expression is an enumeration predicate : either side of it may dictate that it is (an enum compared with an expression or variable compared with some constant (in range)).    
let rec determine_efo c x =
    // Return its Enum status.
    let n = xb2nn x
    determine_efo_n c (n)

and determine_efo_c c x =
    // Return its Enum status.
    let n = xb2nn x
    determine_efo_n c (n)



// Close a strobe group - no further members may be added but tests based on the group can be better optimised.
// Close using access key in the form of an indexing variable
and close_enum msg vv =
    let pinit = (x2nn vv)
    let grp = priv_getgrp "close" None pinit
    let msg = msg + " " + xToStr vv
    match g_strobe_group_members.lookup grp with
        | None ->
            hpr_yikes(msg + sprintf ": forit=%i group %i has nothing in it when attempt to close" pinit grp)
            ()
        | Some (ovi, ovl, basis, closedo) -> 
            let _ = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   - Enum close grp=%i (%i members) request close " grp (length ovl)  + msg + " " + sfold enumfToStr ovl)
            if not_nonep closedo then
                let _ = vprintln 2 ("Enum group already closed: new=" + msg + "; closer was " + fst(valOf closedo) + ";")
                ()
            else
                let big:int = hd(rev(List.sort (map knw_enum_constant ovl)))
                let encoding_width = bound_log2 (BigInteger big)
                if (mine_prec g_bounda vv).signed <> Unsigned then vprintln 0 ("yikes: enum/strobe group variable was not unsigned in " + netToStr vv)
                if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   - Enum group %i close. Biggest %A , encoding_width=%i" grp big encoding_width)
                clear_espresso_cache()// NB: kill all cached espresso calls on close of enum group...
                g_strobe_group_members.add grp (ovi, ovl, basis, Some(msg, encoding_width))

and priv_grp_add grp nv = // Add a new enumf_t (strobe) to an existing group or create new
    match nv with
    | Enum_pv(n, v, (m, enum_const, enum_const_idx), grp) ->
        //let _ = cassert(xbToStr(deblit n)<> "2==mpc10", "found it 2") 
        match g_strobe_group_members.lookup grp with
            | Some(ovi, ovl, basis, closedo) ->
                let m_changed = ref false
                let rec insert lst =
                    match lst with
                    | [] -> (m_changed:=true; nv :: lst)
                    | Enum_pv(n', v', (m', enum_const', enum_const_idx'), g')::tt ->
                        if (n=n' && m=m') then lst
                        elif m=m' then
                            // TODO explain this please.
                            vprintln 0 ("+++Enumeration constant encoding minor issue: tried to use a second tag with the same value as a previous " + i2s n + " cf " + i2s n' + "  " + enumfToStr(hd lst) + " cf " + enumfToStr nv)
                            lst
                        elif m' < m then (hd lst) :: insert tt
                        else (m_changed:=true; nv :: lst)
                let nlst = insert ovl
                let _ = if !m_changed
                        then
                            if not_nonep closedo then sf ("Added (1) new member to closed enumeration: new=" + enumfToStr nv + "; existing=" + sfold enumfToStr ovl + "; " + fst(valOf closedo))
                            if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   Enumeration add:  grp=%i n=%i m=%i" grp n m + sfold enumfToStr nlst)
#if NNTRACE
                            vprintln 0 (sprintf "   Enumeration add:  grp=%i n=%i m=%i" grp n m + sfold enumfToStr nlst)
#endif
                            g_strobe_group_members.add grp (map priv_fi nlst, nlst, basis, None)
                ()
            | None ->
                let _ = 0
                let basis = funique "AUTB"
                g_strobe_group_members.add grp ([priv_fi nv], [nv], basis, None)


and enum_coding_width vv =
    let get_enum_ival cc = function
        | Enum_p nn -> if nonep cc then None else Some(max (valOf cc) 1)
        | Enum_pv(n, v, mm, grp) -> if nonep cc then None else Some(max (valOf cc) (f1o3 mm))
        | Enum_none        -> None // mixing 
        | Enum_not_an_enum -> None
    match x2nn vv with
        //| None -> if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   Coding with cannot be returned for %s since no nn" (xToStr vv))
        | nn ->
            let grp = priv_getgrp "enum_coding_width" None nn // This makes it a group!
            match g_strobe_group_members.lookup grp with
                | Some (ovi_, ovl, closedo, Some(basis, encoding_width)) ->
                    if !g_enum_loglevel>=4 then vprintln 4 (sprintf "Encoding width %i returned for grp=%i nn=%i for %s ovl=%s" encoding_width grp nn (xToStr vv) (sfold enumfToStr ovl))   
                    Some encoding_width
                | Some (ovi_, ovl, _, None) -> 
                    match List.fold get_enum_ival (Some 0) ovl with
                        | None ->
                            hpr_yikes(sprintf "Encoding width cannot be returned for %s owing to missing ival. ovi=%A" (xToStr vv) ovi_)
                            None
                        | Some maxe ->
                            let w = bound_log2 (BigInteger maxe)
                            vprintln 3 (sprintf "Encoding width should not really be returned for %s since not closed. Maxe=%A  w=%i" (xToStr vv) maxe w)
                            Some w
                | None ->
                    dev_println (sprintf "Enum has no record of an encoding width for %s grp=%i nn=%i" (xToStr vv) grp nn)
                    None
                                  
and enum_addf site ix_deqd vv sgo q = // Add to an unclosed enumeration set or return existing member, or add/redirect a synonym to a group that already has an exemplar for this q.
    let vv_i = x2nn vv
    let enum_constf basis = // The principle tagged-constant value element creator. 
        let printable_form = sprintf "%i:%s" q basis
        xi_stringx (XS_withval(xi_unum q)) printable_form
    //if nonep vv_i then (ix_deqd vv (enum_constf "UF"), Enum_not_an_enum) else
    let grp = priv_getgrp (site+"_1") sgo (vv_i)

    let basis = 
        match g_strobe_group_members.lookup grp with // looked up again: rather silly.
            | Some (ovi_, ovl, basis, closedo) -> basis
            | _ ->
                let rec basis =
                    match vv with
                        | X_net(s, _) -> s
                        | X_bnet ff -> ff.id
                        //| W_string (ss, XS_withval wv, _) -> basis wv
                        | _ -> funique "USA"
                if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   Establish/create new basis %s for strobe group %i site=%s" basis grp site)
                g_strobe_group_members.add grp  ([], [], basis, None) // Need to set straightaway since we get re-entrant via xi_deqd
                basis
    let enum_const = enum_constf basis
    let nn1 = x2nn enum_const
    let grp' = priv_getgrp (site + "_2") (Some grp) nn1

    if !g_enum_loglevel>4 then vprintln 4 (sprintf "   enum_addf vv=%s vvn=%i grp=%i    q=%i const=%s const_nn=%i  const_grp=%i   basis=%s"  (xToStr vv) (vv_i) grp  q (xToStr enum_const) nn1 grp' basis)
    
    let yield_new() =
        let strobe = ix_deqd vv enum_const
        if !g_enum_loglevel>=4 then vprintln 4 (sprintf "New strobe made %i " grp  + xbToStr strobe)
        (strobe, Enum_pv((xb2nn strobe), vv_i, (q, [enum_const], [x2nn enum_const]), grp))

    match g_strobe_group_members.lookup grp with
        | Some (ovi_, ovl, basis__, closedo) ->
            let m_newf = ref false
            let ans = ref (X_dontcare, Enum_none)
            let rec insert lst =
                match lst with
                    | [] -> (ans := yield_new(); m_newf := true; [snd !ans])
                    | Enum_pv(n', v', (m', forms, idx_forms), g')::tt ->
                        if q=m' && memberp vv_i idx_forms then
                            let _ = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   : Enumeration add already present: vv=%s grp=%i %i nn=%i" (xToStr vv) grp n' (vv_i))
                            (ans := (deblit n', hd lst); lst)
                        elif q=m' then
                            let _ = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   : Enumeration add as further alias: q=%i vv=%s grp=%i %i nn=%i" q (xToStr vv) grp n' (vv_i))
                            let na = Enum_pv(n', v', (m', forms @ [enum_const], idx_forms @ [x2nn enum_const]), g')
                            (ans := (deblit n', na); na::tt)
                        elif m' < q then (hd lst) :: insert tt
                        else (ans := yield_new(); m_newf := true; (snd !ans :: lst))
            let nlst = insert ovl // Insert in ordered list.
            let _ = g_strobe_group_members.add grp (map priv_fi nlst, nlst, basis, closedo)
            let _ = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   stored basis 1/2 u/d %s newf=%A for strobe group %i" basis !m_newf grp)
            let _ =
                if !m_newf then
                    let _ = if not_nonep closedo then sf ("Added (2) new member to (a closed?) enumeration: new=" + enumfToStr (snd !ans) + "; existing=" + sfold enumfToStr (ovl) + "; " + fst(valOf closedo))
                    ()
            let not_dontcare = function
                | X_dontcare -> false
                | _          -> true
            let _ = cassert(not_dontcare(fst !ans), "no strobe made")
            (!ans)
        | None ->
            let (s, nv) = yield_new()
            let _ = if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   Enumeration create for %s grp=%i " (xToStr vv) grp)
            g_strobe_group_members.add grp  ([priv_fi nv], [nv], basis, None)
            if !g_enum_loglevel>=4 then vprintln 4 (sprintf "   stored basis 2/2 e/c  %s for strobe group %i" basis grp)            
            (s, nv)

and priv_fi = function
    | Enum_pv(n, v, m, grp) -> n


and gec_pv(n, vv, m, r) =
    let grp = priv_getgrp "gec_pv" None vv
    let ans = Enum_pv(n, vv, (m, r, map x2nn r), grp)
    let _ = priv_grp_add grp ans
    ans
    
and determine_efo_n cc n = // check on a bexp to see if an enum strobe and if so singly add to cc.
    let unabsval v = if n < 0 then enum_negate v else v

    let get_grp sgo vv = 
        priv_getgrp "determine_efo" sgo vv
            
    let nn = abs n 
    match unary_strobes.[nn] with
        | Enum_none ->
            //If a floating point comparison, we skip strobe groups.
            let abs_l = deblit nn
            let nv =
                match abs_l with
                    | W_linp(vv, lin, meo) ->
                        let v = x2nn vv
                        muddy " if v=None then Enum_p(meo.n) else muddy \"Enum_pv(meo.n, valOf v)\""

                    | W_bdiop(V_deqd, [l; r], false, meo) when int_constantp l ->
                        let v = x2nn r
                        //if v=None then Enum_p(meo.n) else
                        let (sgo, m0) = enum_constate l
                        try let m = int(m0)
                            let grp = get_grp sgo (x2nn r)                         
                            let ans = Enum_pv(meo.n, v, (m, [l], [x2nn l]), grp)
                            let _ = priv_grp_add grp ans
                            ans // gec_pv(meo.n, valOf v, (m, l), 
                        with overflow_ -> Enum_not_an_enum

                    | W_bdiop(V_deqd, [l; r], false, meo) when int_constantp r ->
                        let v = x2nn l
                        //if v=None then Enum_p(meo.n) else
                        let (sgo, m0) = enum_constate r                        
                        try let m = int(m0)
                            let grp = get_grp sgo (x2nn l)
                            let ans = Enum_pv(meo.n, v, (m, [r], [x2nn r]), grp)
                            let _ = priv_grp_add grp ans
                            ans
                        with overflow_ -> Enum_not_an_enum

                    | W_bdiop(V_deqd, [l; r], false, meo) when int_constantp r || int_constantp l ->  
                        vprintln 0 ("+++enum opportunity lost?" + xbToStr abs_l)
                        Enum_not_an_enum

                    | _ -> Enum_not_an_enum
            //let _ = vprintln 3 (i2s nn + " determine_efo unary_strobe stored  " + xbToStr abs_l + " --> " + enumfToStr nv)
            let _ = Array.set unary_strobes (nn) nv
            match nv with
                | Enum_not_an_enum -> cc
                | _ -> new_known_dp(singly_add (unabsval nv) cc.facts)

        | other -> new_known_dp(singly_add (unabsval other) cc.facts)

let get_peersg grp = 
    let ov = g_strobe_group_members.lookup grp  
    let _ = cassert(ov<>None, "get_peersg: found")
    f1o4(valOf ov)

let get_peers n0 =
    let no = ([], false)
    let nn = abs n0
    if nonep blits.[nn] then sf "suspect problem: not a bexp"
    else
    let v = determine_efo_n (new_known_dp []) n0
       
    let extras = function // THIS SEEMS A LOT OF LOOKUP!
        | Enum_pv(n, v, m, grp) ->
            let pgo = g_strobe_group_members.lookup grp  
            let _ = cassert(not_nonep pgo, "get_peers: found")
            let (peers, _, basis, closedo) = valOf pgo
            //vprintln 0 ("peers of " + i2s n0 + " are " + sfold i2s peers) // + " akaor " + sfold (fun x->xbToStr(deblit x)) peers)
            (peers, not_nonep closedo)
        | _ -> no

    if nullp v.facts then no else extras (hd v.facts)


(*-------------------------------------------------------------------------*)
(*
 * Generating a don't care set for impossible input cases caused
 * by logical implications between predicates.  For instance, if pred 
 * p1 =   x < 10
 * p2 =   x < 20
 *then p1 -> p2 and hence we do not care what result our minimiser gives us 
 * when p1 holds and p2 does not.  p1 && ~p2 = X.
 *
 *
 *   a="l1<v" b="l2<v" l1<=l2 : b->a   
 *                     l2<=l1 : a->b   
 *   a="v<r1" b="v<r2" r1<=r2 : a->b   
 *                     r2<=r1:  b->a

 *   a="l<v" b="v<r"   l<r    : 
 *                     l>=r   : a&b cannot both hold
 *)
type i_t = IMP of (hbexp_t * int) * (hbexp_t * int)

type j_mutimp_t = // Mutual implications 
    | Jimp of int * int    // A implies B
    | Jrimp of int * int   // A is implied by B    
    | Jex of int * int     // A excludes B (having both is invalid)
    | Jeq of int * int     // A is equivalent to B
    | Jnull                // There are no mutual implications
    

let jimpToStr verbose arg =
    let xp p = if verbose then i2s p + "/" + xbToStr(deblit p) else i2s p
    match arg with
    | Jimp(p,q) -> xp p + " => " + xp q
    | Jrimp(p,q)-> xp p + " <= " + xp q    
    | Jex(p,i)  -> xp p + " excludes " + xp i
    | Jnull     -> "Jnull"
    | Jeq(p,i)  -> xp p + "==" + xp i

(*
 * Find implications between boolean literals.
 * Collate comparisons between scalars as constants in four forms v!=n, v==n, v<n and v>=n.  There
 * are 16-6=10 patterns of comparison between these four forms. Leading to a mutual implication j_mutinp_t.
 * We'll have an array of maps.  Owing to symmetry or contrapositive rules, every implication is entered at two locations: Jimp(p,q) and Jimp(-q,-p);
 * in the array.
 *)


type constant_ineq_form_t = CI_eq of int | CI_ne  of int | CI_lt  of int | CI_ge  of int 
//let contrapositives = function
//    | 

let rec constant_inequalities (p, q) = function
    | (CI_eq n, CI_eq m) -> if n=m then Jeq(p, q) else Jex(p,q)
    | (CI_eq n, CI_ne m) -> if n=m then Jex(p, q) else Jimp(p,q)
    | (CI_eq n, CI_lt m) -> if m<=n then Jex(p, q) else Jimp(p,q)
    | (CI_eq n, CI_ge m) -> if m<=n then Jimp(p, q) else Jex(p,q)

    | (CI_ne n, CI_lt m) -> if m<=n then Jrimp(p, q) else Jnull
    | (CI_ne n, CI_ge m) -> if m<=n then Jnull else Jrimp(p,q)
    | (CI_ne n, CI_ne m) -> if n=m then Jeq(p, q) else Jnull


    | (CI_lt n, CI_lt m) -> if m>=n then Jimp(p, q) else Jrimp(p,q)
    | (CI_lt n, CI_ge m) -> if n<m then Jex(p, q) else Jnull

    | (CI_ge n, CI_ge m) -> if n=m then Jeq(p, q) elif m>n then Jrimp(p, q) else Jimp(p, q)

    | (pp, qq) -> constant_inequalities (q, p) (qq, pp) // watch out for infinite commute!

    
let m_supress_multiple_warns_cas = ref 0 // count-based warning supressor just for now ...

let find_lit_implications vd (cover:cover_t) = 

    let litToStr(X, lhs, rhs, pol) = i2s X + "(" + xToStr lhs + ", " + xToStr rhs + ", " + boolToStr pol + ")"

    let plist = map (fun m -> (deblit m, m)) (list_once(list_flatten cover))
    //let _ = reportx vd "Soplits (in +ve or -ve form)" (fun (it, m)-> "   " + i2s m + " " + xbToStr it) plist
    let literals = List.fold cube_support [] cover
    let local_support c = function
        | W_bdiop(oo, [l; r], i, _) ->
            let c1 = if int_constantp l then c else singly_add l c
            if int_constantp r then c1 else singly_add r c1
        | other -> c
            
    let support = List.fold (fun c n -> local_support c (deblit n)) [] literals
    let per_var c (v) = //TODO: take note of bit insert values in implications
        // Collate tests of support item v.
        let correct_signed signed arg =
            let _ =
                if signed <> Signed then
                    let _ = mutinc m_supress_multiple_warns_cas 1
                    if !m_supress_multiple_warns_cas < 5 then vprintln 0 (sprintf "+++ correct_var treated unsigned compare as signed in %A" arg)
            arg
        let eval arg = int(xi_manifest_int "per_var" arg)
        let useable_guard c aa =
            try match aa with
                | (W_bdiop(V_deqd, [l; r], false, _), m) when r=v && int_constantp l -> (CI_eq(eval l), m)::c
                | (W_bdiop(V_deqd, [l; r], true, _), m)  when r=v && int_constantp l -> (CI_ne(eval l), m)::c 
                | (W_bdiop(V_dned, [l; r], false, _), m) when r=v && int_constantp l -> (CI_ne(eval l), m)::c
                | (W_bdiop(V_dned, [l; r], true, _), m)  when r=v && int_constantp l -> (CI_eq(eval l), m)::c 
                    
                | (W_bdiop(V_deqd, [l; r], false, _), m) when l=v && int_constantp r -> (CI_eq(eval r), m)::c
                | (W_bdiop(V_deqd, [l; r], true, _), m)  when l=v && int_constantp r -> (CI_ne(eval r), m)::c 
                | (W_bdiop(V_dned, [l; r], true, _), m)  when l=v && int_constantp r -> (CI_eq(eval r), m)::c 
 
                | (W_bdiop(V_dltd signed, [l; r], false, _), m) when l=v && int_constantp r -> correct_signed signed ((CI_lt(eval r), m)::c)
                | (W_bdiop(V_dltd signed, [l; r], false, _), m) when r=v && int_constantp l -> correct_signed signed ((CI_ge(eval l), m)::c)

                | (W_bdiop(V_dltd signed, [l; r], true, _), m)  when l=v && int_constantp r -> correct_signed signed ((CI_ge(eval r), m)::c)
                | (W_bdiop(V_dltd signed, [l; r], true, _), m)  when r=v && int_constantp l -> correct_signed signed ((CI_lt(eval l), m)::c)
                // dged etc should not be encountered owing to memoising
                
                | (W_linp(v, lp, _), m) -> muddy "linp still in use"
                | _ -> c
            with overflow_ -> c
        let terms = List.fold useable_guard [] plist

        // Elsewhere in meox, all four comparison operators are mapped on to dltd by reversing args and inverting the result.
        // This means that dltd does not always have any constant arg on the lhs. But when constants are
        // involved, a<=3 can be mapped to a<4, giving more freedom.
        // 
        //if vd>=4 then vprintln 4 (i2s (length terms) + " terms are equalities and inequalities over " + xToStr v)
        let _ :Map<int, int list list> [] = dontcare_sets
        let pair (pp, p) c (qq, q) =
            let dc_abs p = p + g_mya_limit
            let ppos = dc_abs p
            let ov = dontcare_sets.[ppos]
            let lk = ov.TryFind(q)            
            let ans =
                if lk <> None then valOf lk
                else 
                let ans0 = constant_inequalities (p, q) (pp, qq)
                let todc arg =
                    match arg with
                    | Jnull -> []
                    | Jimp(p, q) -> [[p; -q]]
                    | Jrimp(p, q) -> [[-p; q]]
                    | Jex(p, q) -> [[p; q]] // Consder a=4 and a=5: these are exlusive, but a<>4 and a<>5 are null.
                    | Jeq(p, q) ->
                        vprintln 0 ("+++Non-unified tautology ? " + jimpToStr true arg)
                        [[-p; q]; [p; -q]]  // p would normally be equal to q except for wierd tautologies
                //if vd>=4 then vprintln 4 ("Implications : " + (jimpToStr true) ans0)
                let dcs = todc ans0
                let _ = Array.set dontcare_sets (ppos) (ov.Add(q, dcs))
                let qpos = dc_abs q
                let _ = Array.set dontcare_sets (qpos) (dontcare_sets.[qpos].Add(p, dcs))
                dcs
                
            ans @ c
        let rec pairwise c = function // Compare each term with every other
            | a::tt ->
                let c' = List.fold (pair a) c tt
                in pairwise c' tt
            | _ -> c
        let ans = pairwise [] terms

        ans @ c
        
    let ans = List.fold per_var [] support

    ans


// def>xi_cover 
let xi_cover lst =
    let xint_write_cover_final klst =
        let (found, ov) = (!cover_SPLAY).TryGetValue klst //TODO avoid double lookup
        if found then ov
        else
            //let _ = app (fun lst -> if memberp -537 lst && memberp 536 lst then sf "here write final -537 and 536" else vprintln 0 "cover 4ok") klst
            let nn = nxt_it()
            let ans = W_cover(klst, { (*hash__=0;*) n=nn; sortorder=nn (* + wab_node_order key *) })
            // Do we want to save covers in blits: yes, this stores all boolean expressions, not just 'literals'
            let _ = blit_save nn ans // TODO - delete this: they are not blits!
            // Test4b is failing on level sens latch!
#if NNTRACE
            vprintln 0 ("Not saving blit " + i2s nn)
            vprintln 0 ("meox: allocate cover xb2nn=" + i2s nn + "  for " + xbToStr ans)
#endif
            let _ = (!cover_SPLAY).Add(klst, ans)
            ans
// todo ? Perhaps index cover's xb2nn with ok_covers array so dont need splay lookup?
// todo ? Perhaps cache negations of covers...  this would be done by xi_not anyway?
    match lst with
        | []       -> X_false
        | [[]]     -> X_true
        | [[item]] -> deblit item // xint_write_cover [[item]]    
        | (cover:cover_t) ->
            let (found, ov) = (!cover_SPLAY).TryGetValue cover   
            if found then ov
            else
            let cubecost lst =  List.fold (fun c x -> c + length x) (length lst) lst // copy 2/2
            let cst = cubecost cover
            let cover' =
               if !g_use_espresso && cst < !g_or_heuristic_value // Do not invoke above this cost
               then
                   let dont_care:cover_t = find_lit_implications 0 cover // find these anyway when espresso off ?
                   // let _ = map find_lit_implications lst
                   let ans = e_espresso 0 (cover, dont_care)
                   let _ = vprintln //(*deb*)0 ("Espresso v result:" + covToStr ans + " from f=" + covToStr cover + " dc=" + covToStr dont_care)
                   ans
               else cover
            let ans = xint_write_cover_final(cover') // todo avoid double lookup when cover=cover'
            let _ = if cover' <> cover then (!cover_SPLAY).Add(cover, ans)
            ans


let is_times = function
    | V_times _ -> true
    | _ -> false

let is_divide = function
    | V_divide _ -> true
    | _ -> false

let is_rshift = function
    | V_rshift _ -> true
    | _ -> false



// Big number less than compare.
// 
// TODO Post this improved version down to meox please: cf    let dltd a b = if signed=Signed then a<b else muddy "L4774 dltd"
let const_bn_dltd widtho sx (a:BigInteger) b =
    if nonep widtho then (a<b)
    else
        let width = valOf widtho 
        let signed = sx=Signed
        let bnummer ans0 = 
            let wmask = himask width
            //vprintln 0 (sprintf "const_bn_dltd: signed=%A w=%i wmask=%A x=%A" signed width wmask ans0)    
            let ans0 = bnum_bitand_with_width signed width wmask ans0
            ans0
        let ans = (bnummer a) < (bnummer b)
        //vprintln 0 (sprintf "const_bn_dltd width=%i signed=%A l=%A r=%A ->> %A" width signed a b ans)
        //vprintln 0 (sprintf "const_bn_dltd width=%i signed=%A l=0x%s r=0x%s ->> %A" width signed (bix a) (bix b) ans)        
        ans

// 64-bit integer less than operator, with width option and unsigned override.
let const_int64_dltd widtho sx (a:int64) b =
    let signed = sx=Signed
    if nonep widtho then
        if signed then (a<b) else (uint64 a) < (uint64 b)
    else
        let width = valOf widtho 
        let snummer (aa:int64) = 
            let ans0:uint64 = uint64 aa
            if width > 64 then sf "const_int_dtld not an int64"
            elif width = 64 then ans0
            else
                let wmask = lowmask width
                let sbit = uint64 ans0 &&& (wmask - lowmask(width-1))
                let ans1 = if sbit=0UL then (uint64 ans0 &&& wmask) else ans0 ||| ((~~~0UL) &&& (~~~wmask)) 
                //vprintln 0 (sprintf "const_int_dltd: signed=%A w=%i wmask=%A x=%X-->%X" signed width wmask ans0 ans1)
                ans1
        let ans = if signed then int64(snummer a) < int64(snummer b) else (snummer a) < (snummer b)
        //vprintln 0 (sprintf "const_int64_dltd widtho=%A signed=%A l=%A r=%A ->> %A" width signed a b ans)
        //vprintln 0 (sprintf "const_int64_dltd widtho=%A signed=%A l=0x%X r=0x%X ->> %A" width signed a b ans)        
        ans

// 32-bit integer less than operator, with width option and unsigned override.
let const_int32_dltd widtho sx (a:int) b =
    let signed = sx=Signed
    if nonep widtho then
        if signed then (a<b) else (uint32 a) < (uint32 b)
    else
        let width = valOf widtho 
        let snummer (aa:int32) = 
            let ans0:uint32 = uint32 aa
            if width > 32 then sf "const_int_dtld not an int32"
            elif width = 32 then ans0
            else
                let shin = (1u <<< width)
                let wmask = shin-1u
                let sbit = ans0 &&& wmask
                let ans1 = if sbit=0u then (ans0 &&& wmask) else ans0 ||| (~~~0u &&& (~~~wmask)) 
                //vprintln 0 (sprintf "const_int_dltd: signed=%A w=%i wmask=%A x=%X-->%X" signed width wmask ans0 ans1)
                ans1
        let ans = if signed then int32(snummer a) < int32(snummer b) else uint32(snummer a) < uint32(snummer b)
        //vprintln 0 (sprintf "const_int32_dltd widtho=%A signed=%A l=%A r=%A ->> %A" width signed a b ans)
        //vprintln 0 (sprintf "const_int32_dltd widtho=%A signed=%A l=0x%X r=0x%X ->> %A" width signed a b ans)        
        ans


    
let rec simplify_cover_assuming__ (known:known_t) lst = // NOT USED - THIS IS ALL LOWER NOW.

    let rec simpcube = function
        | [] -> []
        | h::t ->
            //if h is negative, and all the other peers of a closed enumeration set are present, then this cube is invalid, we might not be closed when conjunction was formed?
            let rec scan = function
                | [] -> [h] // Leave intact, no known information.
                | Enum_pv(n, v, m, grp) :: tt when n=h -> []
                | Enum_pv(n, v, m, grp) :: tt when n = -h -> raise Invalid
                | Enum_pv(n, v, m, grp) :: tt ->
                    let peero = g_strobe_group_members.lookup grp  // dont want to do this on each iteration of simpcube
                    if peero=None then scan tt
                    else
                    let peers = f1o4(valOf peero)
                    if h <> n && memberp h peers then raise Invalid
                    elif memberp -h peers then [] // Redundant factor in term (e.g. x!=4 when we know x==3) 
                    else scan tt
                
                | _ :: tt -> scan tt


            let a = scan known.facts
            a @ simpcube t

    let simpcube1 c cube =
        try (let v = simpcube cube in v::c)
        with Invalid -> c
    List.fold simpcube1 [] lst

#if OLDFORNW
    let ftail(arg) = 
        match arg with
        | Enum_pv(e, lp, m, grp) ->
            let rec scan = function
                | [] -> gg0 // TODO: really want to successively refine based on all knowledge
                | Enum_pl(e', lp_known)::tt when e'=e && lp=lp_known ->
                    // This is fast path through the general case where conjunction leads to known.
                    let _ = 0
#if SA_SSUMING_TRACE_ENABLED
                    let _ = vprintln 4 ("Conjunction assumption " + linpToStr(lp) + " fast path to X_true (exact match)")
#endif
                    in X_true
                | Enum_pl(e', lp_known)::tt when e'=e ->
                    let linpToStr_dir (LIN(v, lst)) = "(" + boolToStr v + "/" + sfold i2s lst + ")"
                    let r = linrange_conj(lp, lp_known)
#if SA_SSUMING_TRACE_ENABLED
                    let _ = vprintln 4 ("lp    = " + linpToStr_dir lp)
                    let _ = vprintln 4 ("lp_known   = " + linpToStr_dir lp_known)
                    let _ = vprintln 4 ("r     = " + linpToStr_dir r)
#endif
                    let r' =
                        if r=lp_known
                        then
                            let _ = vprintln //4 ("Conjunction assumption " + linpToStr(lp) + " cf known=" + linpToStr lp_known + " ===> same as known so X_true")
                            in LIN(true, [])
                        else
                            let _ = vprintln //4 ("Conjunction assumption " + linpToStr(lp) + " cf known=" + linpToStr lp_known + " ===> " + linpToStr r)
                            in r
                    match r' with
                        | LIN(false, [])-> X_false  // No possible overlap
                        | LIN(true, []) -> X_true
                        | _ -> scan tt // discards anything learned
                | Enum_place(e', _, _)::tt when e=e' -> muddy ("mosmo1" + xToStr e)
                | _ :: tt -> scan tt
            in scan known
            
        | Enum_place(e, cv, pol) -> // deprecated...
            let rec scan = function
                | Enum_place(e', cv', pol')::tt when e'=e ->
                    match (pol', pol) with
                        // Simple equality test when we know the true value:
                        | (false, false) ->
                            let _ = vprintln // 0 ("ff " + i2s cv + " == " + i2s cv') 
                            in if cv=cv'  then X_true else X_false

                        // Inequality test when we know the true value: 
                        | (false, true)  ->
                            let _ = vprintln // 0 "ft"
                            in if cv<>cv' then X_true else X_false

                        // Specific value case: we are testing for v when we know it is not v.
                        | (true, pol) when cv'=cv ->
                            let _ = vprintln // 0 "tp"
                            in if pol then X_true else X_false   
                        | _ -> scan tt
                        // vprintln 0 ("+++no shi optimisation unfinished " + xbToStr gg0) in gg0
                | Enum_place(e', cv', pol')::tt when e' <> e ->
                    let _ = vprintln // 0 ("Hmmm not equal " + xToStr e + " cf " + xToStr e')
                    in scan tt

                | Enum_pl(e', _)::tt when e=e' -> muddy ("mosmo2" + xToStr e)
                | _::tt -> scan tt
                | [] -> gg0
            scan known
        | _ ->
            let _ = vprintln // 0 ("Not a place " + xbToStr gg0)
            gg0
    let _ = vprintln // 0 ("Processing efo " + xbToStr gg0 + (if ans=gg0 then " was unchanged" else " now " + xbToStr ans))
    in
    match gg0 with
        | X_false
        | X_dontcare 
        | X_true -> gg0
        //| W_bnode(oo, lst, inv, _) ->
        //    let lst'  = map (simplify_assuming known) lst
        //    in (!xi_bnode_ref)(oo, lst', inv)
        | other -> ftail(arg)
#endif

(*
 *   W_node operators: three forms:
 *      commutative, associative arith : V_plus V_times V_bitor V_bitand V_xor
 *      commutative, associative logic : V_bitor V_bitand V_xor
 *      diadic, noncom : V_minus, V_mod, V_divide, shifts, ...
 *
 *)

and xint_store_in prec oo lst0 =
    //vprintln 0 (sprintf "xint_store_in prec=%s oo=%A arity=%i" (prec2str prec) oo (length lst0))
    if oo=V_minus then sf "minus used"
    else
        //let _ = if oo=V_divide then vprintln 0 (sprintf "Divide: start args keys=%s  vals=%s" (sfold xkey lst0) (sfold xToStr lst0))
        let noncoma =
            match oo with
               | V_minus | V_divide _ | V_interm | V_mod -> true
               | _ -> false
        let shiftop =
            match oo with
                | V_lshift | V_rshift _ -> true
                | _ -> false
        let noncoml = shiftop
        let monop = oo=V_neg || oo=V_onesc
        let noncom = noncoma || noncoml
        let key_is_times = is_times oo
        let arith = oo=V_plus || key_is_times || noncoma
        let keys = f1o3(xToStr_dop oo)
        let flatten (w, c) =
            match w with
                | (W_node(prec, oo', lst, meo)) -> if oo=oo' then  lst @ c else w::c
                | (other) -> other ::c
        let elidef = not arith && oo<>V_xor
        let lst1 = if noncom then lst0 else foldl flatten [] lst0
        let lst = if noncom then lst1 else insert_sort elidef lst1 
        let lzero = numbers_positive.[0] (* cannot yet access let constants *)
        let lone = numbers_positive.[1]
        // Note for RTL semantics, combining with an identity is not a NOP where the identity has a larger width than the original argument (e.g. 64'd0 + (32'd1 << 33)) gives a different answer without the addition.
        let xf _ = false
        let uf _ = false
        let identities = function
            | V_times _   -> (arith1_pred, lone)
            | V_plus      -> (arith0_pred, lzero)
            | V_minus     -> (arith0_pred, lzero) (* identity only for rhs arg of two *)
            | V_divide _  -> (arith1_pred, lone)  // identity only for rhs arg of two 
            | V_mod       -> (xf, X_undef)        // mod has no identity (except +Inf perhaps)
            | V_rshift _  -> (arith0_pred, lzero) (* identity only for rhs arg of two *)
            | V_lshift    -> (arith0_pred, lzero) (* identity only for rhs arg of two *)
            | V_bitor     -> (arith0_pred, lzero)
            | V_xor       -> (arith0_pred, lzero)
            | V_bitand    -> (arith1_pred, lone)  (* owing to width, not the real identity: See guard in use below *)
            | V_interm | V_neg | V_onesc -> (uf, xi_unit) // There is no identity: but this could be thought of as a self-inverse!

            | V_exp       -> (xf, X_undef) // rhs identity is unity but we dont bother yet.
            // (_) -> sf("identities other: " + keys)
        let (identpred, ident_vale) = identities oo 

        let simple_logic_elims(a, b) =
            if oo=V_interm then None
            elif oo=V_xor then
                 (if a=b then Some lzero else None) 
                 else None (* ("other logic_elims " + xbToStr a + " " + xbToStr b) *)

        let rec cfl =  // Find combinable logic pairs :ie those in the same variable (but not subexpression for now). *)
            function
                | [] -> []
                | [item] ->
                    //vprintln 0 (sprintf "cfl  yield-> %A" (xToStr item))
                    if identpred item && oo<>V_bitand then [] else [item]
                | (aA::bB::tt) ->
                    if identpred aA && oo<>V_bitand then cfl(bB::tt)
                    elif aA=bB && oo<> V_xor then cfl(bB::tt) (* Done in sort routine already, but not for xor. *)
                    elif constantp aA && constantp bB then
                        let r = cfl(ix_const_generic_arith prec oo aA bB::tt)
                        //vprintln 0 (sprintf "cfl const gen %A %A -> %A" aA bB (sfold xToStr r))
                        r
                    else
                        let se = simple_logic_elims(aA, bB)
                        if se <> None then cfl(valOf se::tt) else aA :: cfl(bB::tt)

        let rec cfa  =  // Find combinable arith pairs :ie those in the same variable (but not subexpression for now).
            function
                | [] -> []
                | [item] -> if identpred item then [] else [item]
                | (aA::bB::tt) ->
                      if identpred aA then cfa(bB::tt)
                      elif aA=bB && elidef then cfa(bB::tt)
                      elif constantp aA && constantp bB  then cfa(ix_const_generic_arith prec oo aA bB::tt)
                      elif (oo=V_bitand || key_is_times) && (aA=lzero || bB=lzero) then [lzero]
                      elif aA=X_undef || bB=X_undef then [X_undef]
                      else aA :: cfa(bB::tt)

        // Combinable non-com arith pairs : RHS identities considered only.
        let cfn arg =
            match arg with
                | [aA; bB] ->
                    if identpred bB then [aA]
                    // Invalid rule: elif A = identity then [B]
                    elif constantp aA && constantp bB then [ix_const_generic_arith prec oo aA bB]
                    elif aA=X_undef then [X_undef] 
                    else arg
                |  [solo] ->
                    let _ = vprintln 5 ("input list " + i2s(length lst) +  ":  " + sfold xToStr lst)
                    let _ = vprintln 5 ("cfn list " + i2s(length lst) +  ":  " + sfold xToStr arg)
                    let _ = vprintln 5 ("noncom operator not applied diadicly op=" + f1o3(xToStr_dop oo))
                    [solo]


        let lst' =
            if monop then lst
            else (if noncom then cfn elif arith then cfa else cfl) lst
        let ll = length lst'

//      let k kk = vprintln 0 ("special trace: "+ xToStr kk + "  id=" + xToStr identity)
//*let _ = if oo=v_plus && lst<>[] && hd lst = xint_store3(X_num (-2740)) then
//(app k lst; vprintln 0 ( "len=" + i2s (length lst) + " len'=" + i2s ll))
        //vprintln 0 (sprintf "cf pre list ^%i:" (length lst) +  sfold xToStr lst)
        //vprintln 0 (sprintf "cf post list ^%i:" (length lst) +  sfold xToStr lst')
        //if vd>=4 then vprintln 4 ("xint store node op='" + f1o3(xToStr_dop oo) + "' lst=" + sfold xToStr lst' + "\n")
        let ans = if ll=0 then ident_vale
                  elif ll=1 && hd lst' = X_undef then X_undef
                  elif ll=1 && oo=V_neg && xi_iszero(hd lst') then xi_num 0 // Special case: -0 is same as +0
                  elif ll=1 && (not monop) then hd lst'
                  else
                      let ans = xint_write_node prec oo lst'
                      let _ = vprintln // 0 "xint_write: write full list"
                      ans
                      //vprintln 0 ("xint_store_i : Ans is " + xToStr ans)
        //let _ = if oo=V_divide then vprintln 0 (sprintf "Divide: final yield n=%A %s" (x2nn ans) (xToStr ans) )
        ans


and xbinv inv w =  // Invert the argument if inv holds.  That is, perform an xor with inv. 
    match w with
    | X_true  -> if inv then X_false else X_true
    | X_false -> if inv then X_true else X_false
    | W_bitsel(n, v, i, meo) -> if inv then xi_bitseli (not i) (n, v) else w
    | W_cover(lst, meo)      ->
           if inv
           then
               let b = xb2nn w // nn is always +ve for a cover, we store negated pairs in an array.                       
               //vprintln 0 ("not " + xbToStr w + " n=" + i2s b)
               let _ = cassert(b > 0, "nn always +ve in cover")
               let v = negated_covers.[b]  // DP 
               if v<>None then valOf v
               else
                   let ans = xi_cover(minimising_shannon_not lst) 
                   //vprintln 0 ("not not " + xbToStr ans + " n=" + i2s(xb2nn ans))
                   let _ = Array.set negated_covers b (Some ans) // Main DP result
                   // Also set the contrary value.
                   // This can be a tautology, in which case xb2nn of answer is -1 (X_false).
                   let _ = if (xb2nn ans > 0) then Array.set negated_covers (xb2nn ans) (Some w)
                   ans
           else w 
    | W_bmux(g, ff, tt, _)   -> if inv then xi_bmux(g, xbinv inv ff, xbinv inv tt) else w
    | W_bsubsc(l, r, i, _)   -> if inv then xi_bsubsc(l, r, not i) else w
    | W_bdiop(oo, lst, i, _) -> if inv then xint_write_bdiop oo lst (not i) else w
    | W_bnode(oo, lst, i, _) -> if inv then xint_write_bnode(oo, lst, not i) else w
    | W_linp(v, LIN(s, lst), _) -> if inv then xi_linp_leaf(v, LIN(not s, lst)) else w
    | X_dontcare -> X_dontcare
    //| other -> sf(sprintf "xbinv other: key=%s: other=" (xbkey other) + xbToStr other)

and xi_not n = xbinv true n

(*
 * Memo heap store for the W_bnode operators : logical/boolean V_band | V_bor | V_bxor
 * All boolean ops are commutative.  We dont have implies or anything like that.
 *)
and xint_bstore_comassoc_i(key, lst, invf) =
        let vd = -1 // off
        let keys = f1o3(xabToStr_dop key)
        let flatten w c =
            match w with
                | W_bnode(key', lst', inv', _) when key=key' && invf=inv' -> lst' @ c
                | other -> other ::c
        let lst = if (invf) then lst else List.foldBack flatten lst  []

        let complements =
            function
                | (V_band) ->  X_false  
                | (V_bor)  ->  X_true 
                | (V_bxor) ->  X_true  

//                | (_) -> sf("bool complements other: " + keys)

        let identities =
            function
                | V_band ->  X_true  
                | V_bor  ->  X_false 
                | V_bxor ->  X_false 
//                | (_) -> sf("bool identities other: " + keys)
        let identity = identities(key)
        let die _ = sf ("xint_bstore_commute died")
        let F = if key=V_bor then linrange_disj else if key=V_band then linrange_conj else die
        let G =
            function
                | (W_linp(_, r, _), false) -> r
                | (W_linp(_, r, _), true) -> lin_complement r
                |  _ -> sf ("linp combine: one arg not a linp!")



        let linpcombine(av, a, b) = 
            let lans = F(G (a, false), G(b, false))
            match lans with
                | LIN(s, l) ->
                     if l=[] then (if s then X_true else X_false)
                     else xi_linp(av, lans)

        let simple_blogic_elims(A, B) =  // insert quine code here ? 
            let (aa, ap) = w_babs A
            let (ba, bp) = w_babs B
            if x_beq(aa, ba) && ap<>bp  // check for a pair of complements.
            then
                      (
      (*              vprint 0 ( "comp elims " + xbToStr A + " " + xbToStr B + " " + xbToStr(complements key) + "\n");
                      vprint 0 ( "comp elims " + xbToStr aa + " " + xbToStr ba + " " + xbToStr(complements key) + "\n");
                      vprint 0 ( "comp elims " + keys + "\n\n");
      *)
                      Some(complements key)
                      )
               else
               match (aa, ba) with
                    // We want to find falsities such as (4==B && 5==B) or redundant forms like (4==B && 5!=B) ... (c.f. bmux new code)
                | W_bdiop(lop, [l1; l2], lp, _), W_bdiop(rop, [r1; r2], rp, _) when l2=r2 && constantp r1 && constantp l1 ->
                    let _ = vprintln // 0 ("Case 1"  + f1o3(xabToStr_dop key) + " checking complements" + xbToStr A + " cf " + xbToStr B)
                    if (not (x_eq l1 r1)) && lop=V_deqd && rop=V_deqd && key=V_band && not ap && not bp && not lp && not rp then Some(complements key) 
                    else
                        let _ = vprintln //0 ("+++Perhaps optimise blogic " + f1o3(xabToStr_dop key) + " checking complements" + xbToStr A + " cf " + xbToStr B)
                        None
                | W_bdiop(lop, [l1; l2], lp, _), W_bdiop(rop, [r1; r2], rp, _) ->
                    let _ = vprintln // 0 ("Case 2/3 "  + f1o3(xabToStr_dop key) + " checking complements" + xbToStr A + " cf " + xbToStr B)
                    None
                | _ ->
                    let _ = vprintln // 0 ("Case 3/3 "  + f1o3(xabToStr_dop key) + " checking complements" + xbToStr A + " cf " + xbToStr B)                        
                    None


                      // We DO NOT ELIDE XORS IN INSERT SORT - why not ? perhaps add. 
        let sos m =
            (vprint 0 ( m + ": Error at " + f3o3(xabToStr_dop key) + ": [[ " + sfold xbToStr lst + "]]\n");
             vprint 0 ( "List =[[ " + sfold  xbToStr lst + "]]\n");
             sf ("sos arithmetic error: " + m)
            )


        let rec cf = function // Find combinable pairs :ie those in the same variable (but not subexpression for now).
            | (altered, []) -> (altered, [])
            | (altered, [item]) -> if item=identity then (true, []) else (altered, [item])
            | (altered, A::B::tt) ->
(* These clauses caught below with 'hd lst'' checks: given list is sorted.         
                if (A=X_true && key=V_bor) then (true, [X_true])
                else if (A=X_false && key=V_band) then X_false 
                else        *)
                if A=identity then cf(true, B::tt)
                elif A=B && key<>V_bxor then cf(true, B::tt)
                elif islinp A && islinp B && linp_var A =linp_var B && (key=V_bor || key=V_band) then cf(true, linpcombine(linp_var A, A, B)::tt)
                elif bconstantp A && bconstantp B then cf(true, xi_generic_barith(key, A, B)::tt)
                else
                    let se = simple_blogic_elims(A, B)
                    if se<>None then cf(true, valOf se::tt)
                    else 
                        let (a, b) = cf(altered, B::tt)
                        (a, A::b)

        let _ = vprintln // 0 (f3o3(xabToStr_dop key) + " cf list " + i2s(length lst) +   "  " + sfold xbToStr lst)
        let rec iterate lst = 
            let dosort x = insert_bsort (key <> V_bxor) x
            let (altered, lst') = cf(false, dosort lst)
            if altered then iterate lst' else lst'

        let lst = iterate lst
        let ll = length lst

//let k kk = vprint 0 ("special trace: "+ xbToStr kk + "  id=" + xbToStr identity + "\n")
//let _ = if key=Xint_plus && lst<>[] && hd lst = xint_store3(X_num (-2740)) then
//(app k lst; vprint 0 ( "len=" + i2s (length lst) + " len'=" + i2s ll + "\n")) else ()
        if vd>=4 then vprintln 4 ("xint store node " + f3o3(xabToStr_dop key) + " lst=" + sfold xbToStr lst)
        let iv v = xbinv invf v
        let ans =
            if ll=0 then iv identity
            else if ll=1 then iv(hd lst)
            else if ll>=1 && key=V_band && hd lst = X_false then iv X_false
            else if ll>=1 && key=V_bor && hd lst = X_true then iv X_true
            else xint_write_bnode(key, lst, invf)
(*      let _ = if ll>1 && memberp X_false lst then
        ( reportx 0 "error list" (fun x-> xbToStr x  + "  " + i2s(xb2order x)) lst; 
          vprintln 0 ("Ans is " + xbToStr ans + "\n"); 
          muddy "FOUND ERROR"
          )
           else ()
*)
        ans


and xint_store_bcom_deqd(key, P, Q, inv) = 
    if (constantp P && constantp Q) then
        let ans = if inv<>(constant_comp key (P, Q)<>0) then X_true else X_false
        //let _ = vprintln 3 (sprintf "Generic constant deqd ans=" + xbToStr ans)
        ans
    // Check for one-bit signals and project to binary layer.
    else if ewidth "store_deqd" P = Some 1 && ewidth "store_deqd" Q = Some 1
    then ix_xor (if inv then X_true else X_false)  (ix_xnor (xi_orred P) (xi_orred Q))

        
    elif x2order P < x2order Q 
    then xint_write_bdiop key [P; Q] inv
    else xint_write_bdiop key [Q; P] inv

and xi_bquery(g, t, f) = ix_or (ix_and g t) (ix_and (xi_not g) f)
and ix_bquery g t f    = ix_or (ix_and g t) (ix_and (xi_not g) f)


and xi_xornor i ll rr =
    let iid x = if i then xi_not x else x // identity
    let iiv x = if i then x else xi_not x // inverted identity
    match (ll, rr) with
    |  (X_false, a)
    |  (a, X_false) -> iid a
    |  (X_true, a)
    |  (a, X_true)  -> iiv a

    |  (ll, rr) ->
        let cubecost = function
            | W_cover(lst, _) ->  List.fold (fun c x -> c + length x) (length lst) lst
            | _ -> 1

        let c1 = cubecost ll
        let c2 = cubecost rr
        //dev_println (sprintf "xor crude heuristic %i+%i < %i" c1 c2 !g_xor_heuristic_value)
        if c1+c2 < !g_xor_heuristic_value then // Do not expand xor's above a given cost. This is a very crude heuristic.  It would be much better to see if the two sides have a number of literals in common that will suffer quadratic expansion in the xor'd result.
            if i then ix_or (ix_and ll rr) (ix_and (xi_not ll) (xi_not rr))
            else ix_or (ix_and (xi_not ll) rr) (ix_and ll (xi_not rr))               
        else xint_bstore_comassoc_i(V_bxor, [ll; rr], i)


and xi_bitandl lst =
    let rec prec = function
        | [] -> sf "prec null"
        | [item] -> mine_prec g_bounda item
        | h::tt  -> combine_prec (mine_prec g_bounda h) (prec tt)
    xint_store_in (prec lst) V_bitand lst

and xi_bnode (oo, lst, inv) = xint_bstore_comassoc_i(oo, lst, inv)


and minimising_shannon_not (cubes:cover_t) =
    let nn_item item = map (fun x -> [-x]) item
    let rec nn (arg:cover_t) =
        match arg with
        | [] -> [[]]
        | []::_ -> []
        | [item] -> nn_item item
        | lst ->
            let k = hd(hd lst) // todo: better pivot selector needed.
            let coft = (shannon_cofactor k lst)
            let coff = (shannon_cofactor -k lst)
            //vprintln 0 ("  cof with " + i2s k + " cov=" + covToStr coft)
            //vprintln 0 ("  cof with " + i2s -k + " cov=" + covToStr coff)
            let coft = nn (coft)
            let coff = nn (coff)
            //vprintln 0 ("  neg cof with " + i2s k + " cov=" + covToStr coft)
            //vprintln 0 ("  neg cof with " + i2s -k + " cov=" + covToStr coff)

            let ans = e_or (le_and [[k]] coft) (le_and [[-k]] coff)
            //let ans = e_mux k coft coff
            //vprintln 0 ("   ans is " + covToStr ans)
            ans

    nn cubes

and blit_meon x =  // Perform x2nn for boolean literals : these are stored in their database as a side effect. An option is not returned.
                   // If the argument is not a literal (i.e. it is a cover, conjunction or disjunction, then ...
    let meon_literal n =
        let nn = abs n
        let _ =
            if blits.[nn] <> None then ()
            else
                let _ = sf "not used"
                let (ba, bp) = w_babs x // +ve value is always stored.
                in blit_save nn ba
        n

    let meon = function
        | W_bitsel(_, _, _, meo) 
        | W_bsubsc(_, _, _, meo) 
        | W_linp(_, _, meo)  
        | W_bdiop(_, _, _, meo)  -> meon_literal meo.n // all saved by now anyway

        | W_cover(lst, meo) -> meo.n

        | X_dontcare       -> 0
        | X_true           -> 1
        | X_false          -> -1 // These are ok outside of covers.

        | W_bmux(_, _, _, meo)   -> sf "bmux used with covers"
        | W_bnode(_, _, _, meo)  -> sf "bnode used with covers"

        //| other -> sf ("meon other" + xbToStr other)
    in meon x

and blitco x =  // Return a literal as a cover
    let meon_literal n =
        let nn = abs n
        let _ =
            if nonep blits.[nn] then
                let (ba, bp) = w_babs x      // +ve value is always stored.
                Array.set blits nn (Some ba) //These blits are not weak pointers, they are a permanent record of the value.
        //vprintln 0 (sprintf "blitco %i" n)
        n

    let cov_inv = function
        | [[x]] -> [[ -x ]]
        | [] -> [[]]
        | [[]] -> []
                   

    let rec meon = function
        | W_bitsel(_, _, _, meo) 
        | W_bsubsc(_, _, _, meo) 
        | W_linp(_, _, meo)  
        | W_bnode(V_bxor, _, _, meo)
        | W_bdiop(_, _, _, meo)  -> [[ meon_literal meo.n ]]

        | W_bnode(V_bor, lst, i, meo)  ->
            let l = map meon lst
            if i then le_and_fold (map cov_inv l) else le_or_fold l

        | W_bnode(V_band, lst, i, meo)  -> 
            let l = map meon lst
            if i then le_or_fold (map cov_inv l) else le_and_fold l
        
        | W_cover(lst, meo) -> lst

        | X_true        -> [[]]
        | X_false       -> []


        | W_bmux(_, _, _, meo)   -> sf ("bmux used with covers: " + xbToStr x)
        | X_dontcare             -> sf ("meon on constant: " + xbToStr x)
        //| other -> sf ("meon other" + xbToStr other)
    meon x

and le_and_fold (lst:cover_t list) = // Form conjunction of a list of covers.
    match lst with
    | []      -> []
    | [item]  -> item
    | a::b::c -> le_and_fold ((le_and a b) :: c)

and le_or_fold (lst:cover_t list) = // Form disjunction of a list of covers.
    // Basically a list_flatten_and delete duplicates, but we need to be careful about DP of espresso calls: todo audit this.
    match lst with
    | []      -> []
    | [item]  -> item
    | a::b::c -> le_or_fold ((e_or a b) :: c)

// Conjunction: maintain terms in ascending absolute value.  Any contrary pairs result in
// a false (invalid) conjunction (e.g. abc & -a) that must be deleted from its parent sum.
// Essentially we make a cartesian product.
and le_and (a:cover_t) (b:cover_t) =
    //let _ = app (fun lst -> if memberp -537 lst && memberp 536 lst then sf "lh arg here" else vprintln 0 "cover 2ok") a
    //let _ = app (fun lst -> if memberp -537 lst && memberp 536 lst then sf "rh arg here" else vprintln 0 "cover 1ok") b    
    let rec and1x = function
        | ([], bb) -> bb
        | (aa, []) -> aa
        | (a::al, b::bl) ->
            let invalid() =
                let _ = vprintln //0 ("invalid on " + sfold i2s (a::al)  + " cf " + sfold i2s (b::bl))
                raise Invalid
            let aa = abs a
            let bb = abs b
            if aa=bb then (if a=b then a::and1x(al, bl) else invalid())
            elif aa<bb then a ::and1x(al, b::bl)
            else b :: and1x(a::al, bl)

    let and2x a b =  // and2x:cube -> cube -> cube
        //vprintln 0 ("and2x cube:" + sfold i2s a + "  & " + sfold i2s b)
        let tag_with_grp x =
            let (peers, closedf) = get_peers x
            (x, closedf, peers)

        let al = map tag_with_grp a
        let bl = map tag_with_grp b

        let collate_grps cc (v, cf, peers) = if peers=[] then cc else singly_add (cf, peers) cc
        let groups = List.fold collate_grps (List.fold collate_grps [] al) bl
        let grp_mitre  (cf, peers) (cc, cda, cdb) =
            let troll (x, cf, peers') (c, cr) = if peers=peers' then (x::c, cr) else (c, x::cr)  //maintain ascending absolute order (for asl and bsl in mitre: stragglers get resorted if any change is made).
            let (asl,ar) = List.foldBack troll al ([],[])
            let (bsl,br) = List.foldBack troll bl ([],[]) 
            if asl<>[] && bsl<>[] then ((cf, peers, asl, bsl)::cc, ar@cda, br@cdb) else (cc, asl@cda, bsl@cdb) 

        let (mitres, stragglers_a, stragglers_b) = List.foldBack grp_mitre groups ([], [], [])

        // R1. When a is negative, eg x!=3, and all the other peers of a closed enumeration set are present on either side, then this cube is invalid.
        // R2. When two different positive strobes are present the cube is invalid.
        // R3. When a is positive and all others are present it is a tautology. <--- TODO ???
        // R4. If we have one +ve then if present -ve we have an invalid conjunction otherwise we can ignore all negatives.
        
        // TODO: does the parent disjunction spot tautologies on full set of +ve strobes?
        let del_neg (cf, members, ll, rr) =
            let rule2_1 ll =
                let rule2_2 rr = if ll>0 && rr>0 && ll <> rr then raise Invalid // Rule R2.
                app rule2_2 rr
            app rule2_1 ll

            let consistent_chk = function
                | a::b::tt ->
                    if a > 0 then
                        if !g_nss_count > 0 then
                            vprintln 0 ("+++ non-singleton +ve strobe in a strobe group cube:" + sfold i2s ll + "  & " + sfold i2s rr + "\nall=" + sfold i2s members)
                            vprintln 0 ("+++ " + sfold (fun x->xbToStr(deblit x)) ll + " were the ll strobes")
                            vprintln 0 ("+++ " + sfold (fun x->xbToStr(deblit x)) rr + " were the rr strobes")
                            mutinc g_nss_count -1
                        [a]
                    else a::b::tt
                | other -> other
                
            let ll = consistent_chk ll // We hope never to form these but get rid of them if encountered: assoc_exp makes them?
            let rr = consistent_chk rr

            let rec collate_all_negs = function  // merge negatives into peer group attempt and abort on a +ve
                | (a::al, b::bl) when a>0 && b>0 && a<>b -> raise Invalid // Rule R2 again - but only called on closed sets.
                | ([], []) -> []
                | (a::al, bl) when a > 0 -> [] // early end
                | (al, b::bl) when b > 0 -> [] // early end                
                | (a::al, []) when a < 0 -> -a :: collate_all_negs(al, []) 
                | ([], b::bl) when b < 0 -> -b :: collate_all_negs([], bl) 
                | (a::al, b::bl) ->
                    if -a < -b then -a :: collate_all_negs(al, b::bl)
                    elif -a > -b then -b :: collate_all_negs(a::al, bl)
                    else -b :: collate_all_negs(al, bl)                                         

            let (cf, members, ll, rr) =
                if cf then
                    let complement = function
                        | [x]   ->  -x // If have complete complement but one then this is equivalent to the one missing.
                        | other ->  sf ("Poor complemented item: " + sfold i2s other + sfold (fun x ->xbToStr(deblit x)) other)
                    let lm = length members // TODO compute this length once!
                    let negs = collate_all_negs(ll, rr)
                    let ln = length negs
                    //vprintln 0 (sprintf "Collated %i/%i in " ln lm + sfold i2s negs)
                    if negs = members then raise Invalid // Rule R1: Full complement of negatives is an invalid cube.
                    elif ln > lm then sf ("Strangely have more negs than possible ")
                    
                    elif ln = lm-1 then
                        let c = complement(list_subtract(members, negs))
                        (cf, members, c::ll, c::rr) // will be tidied shortly. 
                    else (cf, members, ll, rr)
                else (cf, members, ll, rr)
            if false then  (cf, members, ll, rr) // Temp disable of rule R4.
            else
                if hd ll > 0 then
                    let _ = vprintln //0 ("R4: Discard rset L=" + sfold i2s ll + " R=" + sfold i2s rr + "\nall=" + sfold i2s members)
                    if memberp -(hd ll) rr then raise Invalid else (cf, members, [hd ll], []) // Rule R4
                elif hd rr > 0 then
                    let _ = vprintln //0 ("R4: Discard lset L=" + sfold i2s ll + " R=" + sfold i2s rr)
                    if memberp -(hd rr) ll then raise Invalid else (cf, members, [], [hd rr]) // Rule R4
                else (cf, members, ll, rr)
        let mitres' = map del_neg mitres

        let _ = if false && mitres' <> mitres then
                    vprintln 0 (sprintf "Conjunction forming mitres=%A\ngroups=%A\nmitres'=%A" mitres groups mitres')
                    vprintln 0 (sprintf "stragglers stl=%A str=%A" stragglers_a stragglers_b)
                    ()
        let (a1, b1) =
            if mitres' <> mitres then 
                // rebuild_needed
                let (xl, xr) =
                    let qix (_, _, aa, bb) (a, b) = (aa@a, bb@b)
                    List.foldBack qix mitres' (stragglers_a, stragglers_b)
                //vprintln 0 (sprintf "Conjunction formed stl=%A str=%A" stl str)
                (abs_bubble xl, abs_bubble xr)
            else (a, b)
        let ans = and1x(a1, b1)
        //let _ = (fun lst -> if memberp -537 lst && memberp 536 lst then vprintln 3 ("jig here" + sfold i2s a + " & " + sfold i2s b + " -> " + sfold i2s ans)) ans
        ans
       
    let ror1 a1 c h =
        let ans = try e_or [and2x a1 h] c 
                  with Invalid -> c
        ans:cover_t
    let ror c a1 = List.fold (ror1 a1) c b // Nested list folds form cartesian product.
    List.fold ror [] a


and xi_and(ll, rr) = ix_and ll rr

and ix_andl = function
    | [] -> X_true
    | [item] -> item
    | a::b::lst -> ix_andl ((ix_and a b)::lst)
    
and ix_orl = function
    | [] -> X_false
    | [item] -> item
    | a::b::lst -> ix_orl ((ix_or a b)::lst)
    
// def>ix_and
and ix_and ll rr =
    if ll=X_true then rr elif rr=X_true then ll elif (ll=X_false || rr=X_false) then X_false elif ll=rr then ll

    // We could build covers that have both an on-set and a dont-care set. But for now we follow espresso and
    // locally convert don't care to the smallest expression without it.
    elif ll=X_dontcare then rr elif rr=X_dontcare then ll
    elif g_covers then xi_cover (le_and (blitco ll) (blitco rr))
    elif g_bdd_enable then
        let ans = muddy "bmux_and(wrap_bmux ll, wrap_bmux rr)"
        //vprintln 0 ("xi_and " + xbToStr ll + " &&& " + xbToStr rr + " -->> " + xbToStr ans)
        ans
    elif islinp ll && islinp rr && (linp_var ll) = (linp_var rr) then
        let nv =
            match (ll, rr) with
            | (W_linp(lv, linl, _), W_linp(rv, linr, _)) -> linrange_conj(linl,linr)
        xi_linp(linp_var ll, nv)

    else xint_bstore_comassoc_i(V_band, [ll; rr], false)

and xi_or(ll, rr) = ix_or ll rr

// def>ix_or
and ix_or ll rr =
    if ll=rr then ll elif ll=X_false then rr elif rr=X_false then ll elif (ll=X_true || rr=X_true) then X_true
    elif g_covers then (if ll=X_dontcare  || rr=X_dontcare then X_dontcare else  xi_cover (e_or (blitco ll) (blitco rr)))
    elif g_bdd_enable then muddy "bmux_or(wrap_bmux ll, wrap_bmux rr)"
    elif islinp ll && islinp rr && (linp_var ll) = (linp_var rr) then
        let nv =
            match (ll, rr) with
                | (W_linp(lv, linl, _), W_linp(rv, linr, _)) -> linrange_disj(linl,linr)
        xi_linp(linp_var ll, nv)
    else xint_bstore_comassoc_i(V_bor, [ll; rr], false)


// def>xi_plus : looks unprotected but constant folding is in xint_store_i

and ix_plus ll rr = ix_plusp (infer_prec g_bounda ll rr) ll rr

// Sum a list of summands with inferred precision.  
and ix_plusl lst = 
    let rec plusl_serf = function
        | [item]     -> item
        | []         -> xi_num 0
        | aa::bb::tt -> ix_plus aa (plusl_serf (bb::tt))
    // We should scan for the precision first perhaps ... ?
    plusl_serf lst

and ix_plusp prec ll rr = xint_store_in prec V_plus [ll; rr]

and ix_xor ll rr  = xi_xornor false ll rr

and ix_xnor ll rr = xi_xornor true ll rr

// Here we handle an orred of a blift which is indeed an identity function:  |-|(orred(true) = true and |-|(orred(false) = false
// A blift of an orred is not the identity function when the arg is not zero:  For instance blift(|-|31) is one.
and xi_orredi inv = function
    | X_blift v (* when (inv=false) *) -> v     // (* A negated orred of lift x: is it the same as !x ? Yes. *)  

    | W_query(g, t1, f1, _) -> 
        let t2 = xi_orredi inv t1
        let f2 = xi_orredi inv f1
        ix_or (ix_and g t2) (ix_and (xi_not g) f2)
                     
    | W_node(prec, V_bitor, lst, _) ->
        let lst' = map (xi_orredi inv) lst
        let sc c b = ix_or c b
        let ans = List.fold sc X_false lst'
        if inv then xi_not ans else ans
            
    | X_undef -> X_dontcare // Not a true semantic equivalence, but both are typically denoted with 'X'.

    | arg ->
        match xi_monkey arg with
            | Some vale ->
                //vprintln 0 (sprintf "rez xi_orredi inv=%A constant vale=%A arg=%s" inv vale (xToStr arg))                
                xbinv inv (if vale then X_true else X_false)
            | None ->
                //vprintln 0 (sprintf "rez xi_orredi not constant xkey=%s arg=%s" (xkey arg) (xToStr arg))
                xint_write_bdiop V_orred [arg] inv

and xi_orred arg = xi_orredi false arg

and xi_bitseli inv (A, b) = xint_write_bitsel A b inv
and xi_bitsel(A, b) = xi_bitseli false (A, b)


and xi_blift arg =
    match arg with
        | X_true  -> xi_num 1
        | X_false -> xi_num 0
        | W_bmux(gg, X_false, X_true, _) -> xi_blift gg
        | W_bdiop(V_orred, [arg1], inv, meo) ->
            let wos = xi_width_or_fail "xi_blift" arg1
            let _ = vprintln //0 ("xi_blift gen " + i2s wos + " " + xbToStr arg)
            if (not inv) && wos=1
              then arg1
              else X_blift(arg)  // not memoised (for now)
        | X_dontcare -> X_undef
        | arg -> X_blift arg

and xi_deqd (p, q) = xi_deqdf false (p, q)
and ix_deqd a b    = xi_deqdf false (a, b) 

// Distributive law of deqd over conditionals - raise boolean conditionals as high as possible.
and xi_deqdf ignoretag (ll, rr) = 
    //dev_println (sprintf "xi_deqdf ll=%s  rr=%s" (xToStr ll) (xToStr rr))
    if x_eq ll rr then X_true
    else
    match (ll, rr) with
        | (W_query(g1, t1, f1, _), rr) when false -> xi_or(ix_and g1 (xi_deqdf ignoretag (t1, rr)), ix_and (xi_not g1) (xi_deqdf ignoretag (f1, rr)))   // DISABLED FOR NOW! Say why
        | (ll, W_query(g1, t1, f1, _)) when false -> xi_or(ix_and g1 (xi_deqdf ignoretag (ll, t1)), ix_and (xi_not g1) (xi_deqdf ignoretag (ll, f1)))


        | (ll, W_node(prec, V_plus, [lla; rr], _)) 
        | (W_node(prec, V_plus, [lla; ll], _), rr) when x_eq ll rr && constantp lla  ->
            vprintln 3 ("xi_deqdf: Using ineq rule 2  x+c != x")        
            if xi_iszero lla then
                hpr_yikes (sprintf "xi_deqdf: Adding zero detected in ll=%s rr=%s" (xToStr ll) (xToStr rr))
                X_true
            else X_false

        // Convey addition/subtraction of constants to lhs operand (constants should always appear on lhs).
        | (ll, W_node(prec, V_plus, [lla; rr], _)) when constantp ll && constantp lla -> // This does not match triadics etc..
            //vprintln 0 ("Apply deqd: subtract constant from both sides " + xToStr ll + " == " + xToStr rr)
            let ix_minus_lo l r = ix_const_generic_arith prec V_minus l r // xi_minus not yet in scope
            let ans = xi_deqdf ignoretag (ix_minus_lo ll lla, rr)
            ans
        | (ll, rr) -> xi_deqdf1 ignoretag (ll, rr)

// Constants begin compared against enums need mapping to the preferred constant name.
and xi_deqdf1 ignoretag (ll, rr) = 
    let site = "xi_deqdf1"
    let ans = 
        if x_eq ll rr then X_true
        else
        let lconst = int_fconstantp ll
        let rconst = int_fconstantp rr
        if lconst && rconst // nb also checked at lower level - waste
        then
            let rec x_eqc_undec = function // Not used anyway?
                | (X_pair(_, l, _), r) -> x_eqc_undec(l, r)
                | (l, X_pair(_, r, _)) -> x_eqc_undec(l, r)
                | (l, r)               -> x_eqc l r // It's probably better to put pair undecorate functionality into x_eqc anyway?

            if ignoretag then if x_eqc (undecorate ll) (undecorate rr)  <> 0 then X_true else X_false // This is the lifted result version - tidy please.
            elif x_eqc ll rr <>0 then X_true else X_false
        else
            let tailf () = xint_store_bcom_deqd(V_deqd, ll, rr, false)
            let ix_deqd_tail ll rr = xint_store_bcom_deqd(V_deqd, ll, rr, false)
            //dev_println (sprintf "xi_deqdf %s = %s    (%s = %s)" (xToStr ll) (xToStr rr) (xkey ll) (xkey rr))
            match (ll, rr) with
                | (X_bnet ff, rr) when g_use_linp && rconst ->
                    let v = lc_atoi32 rr
                    xi_linp(X_bnet ff, LIN(false, [v; v+1]))

                | (ll, X_bnet ff) when g_use_linp && lconst ->
                    let v = lc_atoi32 ll
                    xi_linp(X_bnet ff, LIN(false, [v; v+1]))                    

                | (X_bnet ff, rr) when rconst  -> 
                    //let f2 = lookup_net2 ff.n
                    let eo = is_enum_var_n ff.n
                    let q = lc_atoi rr
                    if nonep eo  || q < 0I || q > 32768I then tailf() // 32768 max enum. TODO make g_
                    else fst(enum_addf site ix_deqd_tail (X_bnet ff) eo (int32 q))
                        
                | (ll, X_bnet ff) when lconst ->
                    //let f2 = lookup_net2 ff.n
                    let eo = is_enum_var_n ff.n
                    let q = lc_atoi ll
                    if nonep eo || q < 0I || q > 32768I then tailf()
                    else fst(enum_addf site ix_deqd_tail (X_bnet ff) eo (int32 q))
                        
                | (ll, rr) when lconst ->
                    let (sgo, q) = enum_constate ll
                    let q = lc_atoi ll
                    if q < 0I || q > 32768I then tailf()
                    else fst(enum_addf  site ix_deqd_tail (rr)  sgo (int32 q))
                        
                | (ll, rr) when rconst ->
                    let (sgo, q) = enum_constate rr                    
                    if q < 0I || q > 32768I then tailf()
                    else fst(enum_addf site ix_deqd_tail (ll)  sgo (int32 q))

                | _ -> tailf()
    //vprintln 0 ("xi_deqdf of " + xToStr ll + " and " + xToStr rr + " gave " + xbToStr ans)
    ans


and ix_bdiop oo lst inv =
    let ignoretag = true // Best keep this on (certainly for bevelab/bassoc since constantp used)
    match (oo, lst) with
        | (V_dled signed, [ll; rr]) -> ix_bdiop (V_dltd signed) [rr; ll] (not inv)
        | (V_dged signed, [ll; rr]) -> ix_bdiop (V_dltd signed) [ll; rr] (not inv)        

        | (V_dgtd signed, [ll; rr]) -> ix_bdiop_dltd signed rr ll inv
        | (V_dltd signed, [ll; rr]) -> ix_bdiop_dltd signed ll rr inv
        | (V_dned, lst)      -> ix_bdiop V_deqd lst (not inv)
        | (V_deqd, [ll;rr])  ->
            let lw = ewidth "ix_bdiop" ll
            let rw = ewidth "ix_bdiop" rr
            //vprintln 0 ("deqd ix_bdiop" + oiToStr lw + " " + oiToStr rw)
            // Check for both args being boolean and if so hand over to boolean layer (replicted inside xi_deqdf in fact).
            if ((lw=Some 0 && constantp ll) || lw=Some 1) && (rw=Some 1) then
                let xl = xi_orred ll
                let xr = xi_orred rr
                let ans = (if inv then ix_xor else ix_xnor) xl xr
                //vprintln 0 ("made ans " + xbToStr ans)
                ans
            else
                let a = xi_deqdf ignoretag (ll, rr)
                //vprintln 0 ("Temp ans " + xbToStr a)
                xbinv inv a

        | (V_orred, [A]) -> xi_orredi inv A

        | (oo, _) -> sf ("ix_bdiop other: " + f3o4(xbToStr_dop oo))



// dltd is the 'less-than' operator '<'.
// Distributive law of dltd over conditionals - raise boolean conditionals as high as possible.
// ll < (g?t:f) ->  g ? (ll<t: ll<f)
and ix_bdiop_dltd signed ll rr inv =
    match (ll, rr) with
        | (W_query(g1, t1, f1, _), rr) -> xi_or(ix_and g1 (ix_bdiop_dltd signed t1 rr inv), ix_and (xi_not g1) (ix_bdiop_dltd signed f1 rr inv))  
        | (ll, W_query(g1, t1, f1, _)) -> xi_or(ix_and g1 (ix_bdiop_dltd signed ll t1 inv), ix_and (xi_not g1) (ix_bdiop_dltd signed ll f1 inv))  
        | (ll, rr) -> ix_bdiop_dltd1 signed ll rr inv 


and ix_bdiop_dltd1 signed ll rr inv =
    //vprintln  0 ("storing dltd1 A=" + xToStr ll + " B=" + xToStr rr)
    match (ll, rr) with
        | (W_node(prec_a, V_plus, [X_num an; A], _), W_node(prec_b, V_plus, [X_num bn; B], _)) -> ix_bdiop_dltd signed (ix_plus A (xi_num(an-bn))) B inv

        | (W_node(prec, V_plus, [X_num nn; A], _), X_num bn)  -> ix_bdiop (V_dltd signed) [A; xi_num(bn-nn)] inv

        | (W_node(prec, _, [X_num nn; _], _), X_num _)  -> (vprintln 1 ("+++ dltd matching: not an addition broken?:" + xToStr ll); ix_bdiop_dltd_leaf signed ll rr inv)

        | (W_node(prec, _, [_; X_num nn], _), X_num _)  -> (vprintln 1 ("++: sort order broken?:" + xToStr ll); ix_bdiop_dltd_leaf signed ll rr inv)

        | (X_num na, W_node(prec, V_plus, [X_num nb; B1], _))  -> ix_bdiop_dltd signed (X_num(na-nb)) B1 inv

        | (ll, rr)  -> ix_bdiop_dltd_leaf signed ll rr inv


and xi_linp(ee, LIN(pol, lst)) =
    match ee with
        | W_node(prec, V_plus, aa::plst, _) when constantp aa ->
            let a = xi_manifest "xi_linp" aa
            let ee' = List.foldBack (fun x y -> ix_plus x y) plst (xi_num 0)
            xi_linp(ee', LIN(pol, map (fun x -> x-a) lst))
                       
        | other -> xi_linp_leaf(ee, LIN(pol, lst))

and ix_bdiop_dltd_leaf signed ll rr inv =
    let vd = -1 // off
    // Less than: a<a is always false.
    if ll=rr then (if inv then X_true else X_false)
    elif ll=X_undef || rr=X_undef then X_dontcare
    else
    let const_dltd a b = const_int32_dltd None signed a b
    match (ll, rr) with
        | (X_bnet ff_, rr) when g_use_linp && int_constantp rr ->
            let v = lc_atoi32 rr
            xi_linp_leaf(ll, LIN(not inv, [v]))

        | (ll, X_bnet ff_) when g_use_linp && int_constantp ll -> 
            let v = lc_atoi32 ll
            xi_linp_leaf(rr, LIN(inv, [v+1]))  // net > f

        | (_, _) ->
            match (classed_constantp ll, classed_constantp rr) with
                | (Constant_int, Constant_int) ->
                    let a = xi_manifest_int "ix_bdiop dltd" ll
                    let b = xi_manifest_int "ix_bdiop dltd" rr
                    if vd>=4 then vprintln 4  (sprintf "ix_bdiop_dltd_lead of constants %A and %A" a b)
                    // Hmmm - signed now ignored here!
                    let _ = if signed=Signed && (a<0I || b<0I) then dev_println (sprintf "Ignored signedness in ix_bdiop_dltd_lead of -ve constants %A and %A" a b)
                    //if inv <> const_dltd a b then X_true else X_false
                    if inv <> (a < b) then X_true else X_false
                | (Constant_fp, Constant_int)
                | (Constant_int, Constant_fp) ->                    
                    //vprintln 0 (sprintf "ix_bdiop constant aspects rcc")
                    if inv <> (constant_comp (V_dltd FloatingPoint) (ll, rr) <> 0) then X_true else X_false
                    //vprintln 0 ("ix_bdiop: 'less than' of " + xToStr ll + " and " + xToStr rr + " inv=" + boolToStr inv)
                | (Constant_fp, Constant_fp) ->
                    let ll = xi_manifest_double "ix_bdiop_dltd_leaf ll" ll
                    let rr = xi_manifest_double "ix_bdiop_dltd_leaf rr" rr
                    let ans = ll < rr
                    if inv <> ans then X_true else X_false
                    
                | _ -> xint_write_bdiop (V_dltd signed) [ll; rr] inv

and ix_dned ll rr = xint_store_bcom_deqd(V_deqd, ll, rr, true)

and xi_dltd(ll, rr) = ix_bdiop_dltd Signed ll rr false

and xi_dged(ll, rr) = ix_bdiop (V_dltd Signed) [ll; rr] true

and xi_dled(ll, rr) = ix_bdiop (V_dltd Signed) [rr; ll] true

and xi_dgtd(ll, rr) = ix_bdiop (V_dltd Signed) [rr; ll] false


#if XI_LINP
and xint_store_comparisonip__(v, l, r) =   (* TODO: THIS CODE NEEDS TO BE CALLED FROM LOWEr ! *)
        let rec snf_mine = function
                | (X_num n) -> (X_num 0, n)
                | W_node(prec, V_plus, [l; r], _) ->
                    let (ld, lc) = snf_mine l
                    let (rd, rc) = snf_mine r
                    (ix_plus ld rd, lc+rc) 
(* Need to check signs on this one::::  |   snf_mine (x_diadic(V_minus, l, r)) =
                    let (ld, lc) = snf_mine l
                    let (rd, rc) = snf_mine r
                        (xi_sub(ld, rd), lc+rc) end *)
                | l -> (l, 0)

        let (ld, lc) = snf_mine l
        let (rd, rc) = snf_mine r
        let (lc', rc') = if (lc < rc) then (0, rc-lc) else (lc-rc, 0)
        let d = ix_plus ld (X_num lc')
        let e = ix_plus rd (X_num rc')
        if constantp e && isscalar d then xint_store_constraint(v, d, lcm_atoi "xint_store2" e)
        else if constantp d && isscalar e then xint_store_constraint(compare_reflect v, e, lcm_atoi "xint_store2 d arg" d)
        else muddy " call me other otder ix_bdiop(v, [d, e], false)" 



and (* We do not encourage negated LINPs since the LIN has its own way to hold negation *)
  xint_store_constraint = function
          | (V_deqd, l, r:int) -> xi_linp(l, LIN(false, [r; r+1]))
          | (V_dned, l, r:int) -> xi_linp(l, LIN(true, [r; r+1]))
          | (V_dltd, l, r:int) -> xi_linp(l, LIN(true, [r]))
          | (V_dled, l, r:int) -> xi_linp(l, LIN(true, [r+1]))
          | (V_dgtd, l, r:int) -> xi_linp(l, LIN(false, [r+1]))
          | (V_dged, l, r:int) -> xi_linp(l, LIN(false, [r]))
          | (_, _, e) -> sf("xint_store_constraint other")
#endif


    
// Generate a cast, perhaps telescoping a cast applied to a cast.
// Telescope rules:
//   tr1: A cast applied to the same cast should telescope to a single cast of the highest converting power.
//   tr2: Casts between different signnedness (same width) should telescope to the outer one with the highest power dominating.
// And as usual for most operators, we distribute over ternarary xi_query expressons.
and ix_casting inwidth_o caster cvt_power arg =
    let rec docast cast arg  =
        if constantp arg then apply_constant_casting inwidth_o caster cvt_power arg
        else     
            match arg with
                | W_query(gg, tt, ff, _) -> ix_query gg (ix_casting inwidth_o caster cvt_power tt) (ix_casting inwidth_o caster cvt_power ff)
                // precision_compose is commutative and not what we want here.
                //| W_node(old_cast, V_cast, [arg1], _) -> docast (precision_compose true old_cast cast) arg1
                | W_node(old_cast, V_cast old_cvt_power, [arg1], _) when old_cast=cast ->
                    let power = cvt_resolve old_cvt_power cvt_power
                    if power = old_cvt_power then arg // perf optimisation only.
                    else xint_write_cast cast power arg1

                | W_node(old_cast, V_cast old_cvt_power, [arg1], _) when old_cast.widtho = cast.widtho ->
                    let power = cvt_resolve old_cvt_power cvt_power
                    xint_write_cast cast power arg1

                | arg -> xint_write_cast cast cvt_power arg
    docast caster arg

and apply_constant_casting arg_prec casting cvt_power arg =
    let arg_prec = if nonep arg_prec then (mine_prec false arg).widtho else arg_prec
    //let _ = if nonep arg_prec then dev_println (sprintf "apply_constant_casting: Found no input width in arg key=%s %s" (xkey arg) (xToStr arg))
    let w1_ = match casting.widtho with
                | Some n when n>0 -> n
                | _ -> -1

    let tailer_bnum (inner_prec, arg) =
        let ans = bn_masker (inner_prec.widtho) casting arg
        //dev_println (sprintf "apply_constant_casting: L5492 tailer_bnum done: casting CS=%A of %s to %s giving %s" cvt_power (tnumToX_bn arg) (prec2str casting) (xToStr ans)) 
        ans


    match arg with
        | W_node(_, V_cast _, [X_bnet ff'], _)  // TODO precision of inner cast is ignored where an arg_prec is supplied too, which might not not always be correct.

        | X_bnet ff' ->
            //let f2' = lookup_net2 ff'.n
            let ans = 
                match ff'.constval with
                    | XC_bnum(inner_prec, bbn, _) :: _   -> tailer_bnum(inner_prec, bbn) // TODO should be the same as the X_bnum clause below
                    | XC_double f64 :: _        ->
                        if cvt_power = CS_preserve_represented_value then
                            if casting.signed=FloatingPoint && casting.widtho=Some 64 then arg // identity cast
                            elif casting.signed=FloatingPoint && casting.widtho=Some 32 then xi_float32 (float32 f64)
                            else
                                let bn = BigInteger f64
                                bn_masker (arg_prec:int option) casting bn
                        else
                            let bn = get_f64_bits f64
                            gec_X_bnum(casting, bn)

                    | XC_float32 f32 :: _       ->
                        if cvt_power = CS_preserve_represented_value then
                            if casting.signed=FloatingPoint && casting.widtho=Some 32 then arg // identity cast
                            elif casting.signed=FloatingPoint && casting.widtho=Some 64 then xi_double (double f32)
                            else
                                let bn = BigInteger f32
                                bn_masker (arg_prec:int option) casting bn
                        else
                            let bn = get_f32_bits f32
                            //dev_println (sprintf "  proc32 f32=%A all=%A result bn=%A" f32 f' bn)
                            gec_X_bnum(casting, bn)

                    | other -> sf (sprintf "z/4 other form constant  casting arg=%A" arg)
            //dev_println (sprintf "apply_constant_casting: L5521: processed constant casting CS=%A of %s to %s giving %s" cvt_power (xToStr arg) (prec2str casting) (xToStr ans))
            ans
        | X_num n -> bn_masker (arg_prec:int option) casting (BigInteger n)

        | X_bnum(inner_prec, arg, _) -> tailer_bnum(inner_prec, arg)

        | other ->
            vprintln 0 (sprintf "+++ TODO dad2-l/r constant_casting %A not applied tt %A" casting other)
            arg


and ix_const_generic_arith prec oo l0 r0 = // Arithmetic applied to constants only
    let vd = false // Keep as false unless debugging.
    if vd then dev_println (sprintf "ix_const_generic_arith start on oo=%s (keys are %s and %s) l=%s r=%s" (f1o3(xToStr_dop oo)) (xkey l0) (xkey r0) (xToStr l0) (xToStr r0))
    let rec cga_double l = function // Cast to a double-precision float 
        | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _)  :: _  -> xgen_double_arith oo l (double bn)
                | XC_float32 fv ::  _      -> xgen_double_arith oo l (double fv)
                | XC_double dv :: _        -> xgen_double_arith oo l dv
                | other -> sf "bnet double r arg not constant"
        | X_bnum(w, bn, _) ->  xgen_double_arith oo l (double bn)
        | X_num v   ->  xgen_double_arith oo l (double v)

        | W_node(casting, V_cast cvt_power, [X_bnet ff], _) when casting.signed=FloatingPoint -> // This pattern deep matches but I think we prefer cga_bnum approach please...
            // Or better, creating casts of constants should perhaps be disabled instead.
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _)  :: _  -> xgen_double_arith oo l (double bn)
                | XC_float32 fv :: _       -> xgen_double_arith oo l (double fv)
                | XC_double dv :: _        -> xgen_double_arith oo l dv
                | other -> sf "bnet double r arg not constant"

        | W_node(casting, V_cast cvt_power, [arg], _) when casting.signed=FloatingPoint ->

            muddy (sprintf "f/p arith L4841  oo=%A arg=%A. l0=%A r0=%A" oo arg l0 r0)

        | W_node(casting, V_cast cvt_power, [arg], _) ->
            muddy (sprintf "soffof casting snd form %A arg=%s"  casting (xToStr arg))

        | _ -> sf ("cga_double: const_generic_arith missing form: " + f1o3(xToStr_dop oo) + " (" +  xkey l0 + " " + xkey r0 + ") l=" + xToStr l0 + " r=" + xToStr r0)

    let cga_float l = function
        | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _)  :: _ -> xgen_float_arith oo l (float32(double bn))
                | XC_float32 fv ::    _   -> xgen_float_arith oo l fv
                | XC_double dv :: _       -> (xgen_double_arith oo (double l) dv)
                | other -> sf "bnet float r arg not constant"
        | (r) -> cga_double (double l) r

    let rec cga_bnum prec l = function // Constant generic arithmetic.
        | W_node(casting, V_cast cvt_power, [arg], _) when casting.signed =FloatingPoint -> muddy "f/p pops1b"

        | W_node(casting, V_cast cvt_power, [arg], meo) -> // Creating casts of constants should perhaps be disabled instead.
                                                    //L4914 ignore bnum casting CS_typecast to Unsigned:32 on 3.0f
            //let bn = ix_casting inwidth_o caster cvt_power arg 
            dev_println ("constant cast leaf code no longer expected L5578")
            dev_println (sprintf "L4914 ignore bnum casting %A to %s on %s - should never have been created %i" cvt_power (prec2str casting) (xToStr arg) (meo.n))            
            cga_bnum prec l arg

        | X_num r  -> perform_bnum_arith prec oo l (BigInteger r)
        | X_bnum(w, r, _) -> perform_bnum_arith prec oo l r        
        | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _) :: _  -> perform_bnum_arith prec oo l bn 
                | XC_float32 fv  ::    _  -> xgen_float_arith oo (float32(double l)) fv
                | XC_double dv :: _       -> xgen_double_arith oo (double l) dv
                | other -> sf "bnet l arg not constant"

        | r -> cga_double (double l) r

    let rec cga_int prec (l:int32) = function
        | X_bnet ff ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _)  ::_  -> perform_bnum_arith prec oo (BigInteger l) bn
                | XC_float32 fv ::_       -> xgen_float_arith oo (float32 l) fv
                | XC_double dv ::_        -> xgen_double_arith oo (double l) dv

                | other -> sf "bnet l arg not constant"

        | W_node(casting, V_cast cvt_power,  [arg], _) when casting.signed =FloatingPoint ->muddy "pops1"

        // width masking will already have been done on constants, so all we expect here is a signed override?
        | W_node(casting, V_cast cvt_power, [arg], _) ->
            dev_println ("constant cast leaf code no longer expected L5606")            
            if xi_iszero arg || casting.widtho = Some 32 then () else dev_println  (sprintf "TODO fix this: constant arith - temp ignore (pops2) %A on " casting + xToStr arg)
            cga_int prec l arg
        
        | X_num r when oo = V_bitor || oo=V_bitand || oo=V_xor || oo=V_mod || is_rshift oo || is_divide oo -> xi_num(xgen_diop_man prec oo l r) // For speed, some non inflating operations can be kept as int 32 when they are 32 bit quantities or we are sure that width and signness do not matter.
        
        | X_num r -> perform_bnum_arith prec oo (BigInteger l) (BigInteger r)
        | r       -> cga_bnum prec (BigInteger l) r

    let force_float_as_bits = function // These operators treat floating args as bitstrings instead of as floats.
        | V_lshift 
        | V_rshift _
        | V_bitand | V_bitor | V_xor -> true
        | _ -> false



    let rec cga prec = function
        | (l, W_node(prec, V_neg, [mr], _)) when oo=V_plus -> ix_const_generic_arith prec V_minus l mr
        | (_, X_undef)
        | (X_undef, _) -> X_undef         

        | (l, W_string(s, XS_withval r, _))
        | (W_string(s, XS_withval l, _), r) -> cga prec (l, r)

        | (X_bnet ff, r) ->
            //let f2 = lookup_net2 ff.n
            match ff.constval with
                | XC_bnum(w, bn, _) :: _   -> cga_bnum prec bn r
                | XC_float32 fv :: _       -> if force_float_as_bits oo then cga_float_as_bits 32 (X_bnet ff) r else cga_float fv r
                | XC_double dv :: _        -> if force_float_as_bits oo then cga_float_as_bits 64 (X_bnet ff) r else cga_double dv r
                | other -> sf "bnet l arg not constant"

        | (X_num l, r)  -> cga_int prec l r
        | (X_bnum(doNOTIGNOREME_, l, _), r) -> cga_bnum prec l r

        | (W_node(casting, V_cast cvt_power, [arg], _), r) ->
            dev_println ("constant cast leaf code no longer expected L5642")
            let arg' = apply_constant_casting None casting cvt_power arg
            cga prec (arg', r)

        | (l, W_node(casting, V_cast cvt_power, [arg], _)) ->
            dev_println ("constant cast leaf code no longer expected L5647")
            let arg' = apply_constant_casting None casting cvt_power arg
            cga prec (l, arg')

        | (l, r) ->
            sf(sprintf "ix_const_generic_arith other form: oo=%s (keys = %s + %s) l=%s r=%s" (f1o3(xToStr_dop oo)) (xkey l0) (xkey r0) (xToStr l0) (xToStr r0))

    and cga_float_as_bits w l r =
        let prec = { widtho=Some w; signed=Unsigned }
        let (l, r) = (get_float_bits prec l, get_float_bits prec r)
        cga prec (gec_X_bnum(prec, l), gec_X_bnum(prec, r))

    let ans = cga (infer_prec g_bounda l0 r0) (l0, r0)
    //dev_println (sprintf "cga returns %A" (xToStr ans))
    ans
    
and xi_generic_barith =
    function
        |   (V_band, X_false, X_false) -> X_false
        |   (V_band, X_true, X_false) -> X_false
        |   (V_band, X_false, X_true) -> X_false
        |   (V_band, X_true, X_true) -> X_true

        |   (V_bor, X_false, X_false) -> X_false
        |   (V_bor, X_true, X_false) -> X_true
        |   (V_bor, X_false, X_true) -> X_true
        |   (V_bor, X_true, X_true) -> X_true

        |   (V_bxor, X_false, X_false) -> X_false
        |   (V_bxor, X_true, X_false) -> X_true
        |   (V_bxor, X_false, X_true) -> X_true
        |   (V_bxor, X_true, X_true) -> X_false

        |   (oo, l, r) -> sf "xi_generic_barith other"

and x_assoc a =
    function
        | [] -> None
        | (t,v)::tt -> if x_eq a t then Some v else x_assoc a (tt)

and x_bassoc a =
    function
        | [] -> None
        | (t,v)::tt -> if x_beq(a,t) then Some v else x_bassoc a (tt)


// A simple tag used for temporary markings
and xi_tag = function
    | X_x _ -> sf "already tagged"
    | v -> gec_X_x(v, 0) (* for now xint_write(XINT_FUT(v, n)) *)


// Ones' complement operator.
// A binary number is related to its one's complement by the self-inverse ones_c(x) = -1 - x.
and xi_onesc prec d  =
    let rec toBigN = function // silly local function
        | X_num n  -> BigInteger n
        | X_bnum(_, l, _) -> l
        | v -> muddy("xi_onesc constants other: " + xToStr v)
    
    match d with
        | W_node(prec', V_onesc, [item], _) when prec = prec' -> item
        | d->
          match classed_constantp d with
            | Constant_int -> // We ignore input prec here.
                let ans = gec_X_bnum(prec, -1I - toBigN d)
                //vprintln 0 ("xi_onesc const " + xToStr ans)
                ans
            | Constant_string                
            | Constant_fp -> cleanexit(sprintf "ones complement of floating poinrt or string not defined. arg=%A" d)

            | _ ->
                let ans = xint_store_in (mine_prec g_bounda d) V_onesc [d]
                //vprintln 0 ("xi_onesc leaf store: " + xToStr ans)
                ans



and generic_arith1 widtho oo l r = 
     let ans = ix_const_generic_arith widtho oo l r
     let _ = vprintln 3 ("Generic arith " + xToStr l + " " + xToStr r + " = " + xToStr ans)
     ans



// let _ = xi_bnode_ref := xi_bnode in ()
//;

// Example - known 4<=x<=5
//    x==6 -> false
//    x==5 -> x==5
//    x>6  -> false
//    x<6  -> true (conjunction leads to known)

// The `Restrict' operator, also known as simplify_assuming.
and simplify_ass (known:known_t) depth arg =
    //vprintln 0 "simp off"
    if (*true || *)depth <= 0 then arg
    else
    let d1 = depth-1
    let tc = simplify_ass known d1
#if SA_SSUMING_TRACE_ENABLED
    let _ = vprintln 4 ("simplify_ass arg=" + xkey arg + "  " + xToStr arg)
#endif    
    let signed=Signed(*general default*)
    match arg with

    | W_query(g, t, f, _) ->
        let g' = simplifyb_ass known d1 g
#if SA_SSUMING_TRACE_ENABLED
        vprintln 4 ("simplify_ass q  g=" + xbToStr g)
        vprintln 4 ("simplify_ass q  g'=" + xbToStr g')        
        vprintln 4 ("simplify_ass q  t=" + xToStr t)
        vprintln 4 ("simplify_ass q  f=" + xToStr f)        
#endif
        if   g'=X_true  then tc t
        elif g'=X_false then tc f
        else xi_query(g', tc t, tc f)
    | X_blift(g) -> xi_blift(simplifyb_ass known d1 g)
    | X_pair(l, r, _) -> ix_pair (tc l) (tc r)
    | W_node(prec, oo, lst, _) -> ix_op prec oo (map tc lst)
    
    | other ->
        let _ = 0
#if SA_SSUMING_TRACE_ENABLED
        vprintln 0 ("simplify_ass other=" + xToStr other)
#endif
        other
                          

and simplifyb_ass (known:known_t) depth arg =
    if depth <= 0 then arg
    else
    let nn = xb2nn arg
    let (found, ov) = known.dp_xb.TryGetValue nn
#if SA_SSUMING_TRACE_ENABLED
    let _ = vprintln 4 ("simplifyb_ass nn=" + i2s nn + " found " + boolToStr found + (if found then xbToStr ov else ""))
#endif
    if found then ov
    else
    let d1 = depth-1
#if SA_SSUMING_TRACE_ENABLED
    let _ = vprintln 4 ("simplifyb_ass arg=" + xbkey arg + "  " + xbToStr arg + " with known=" + knownToStr known + ";")
#endif

    let ox x = // We have THREE COPIES OF THIS CODE!
        let rec trima = function
                | [] -> None
                | Enum_pv(n, _, _, grp)::tt when n=x -> Some X_true
                | Enum_pv(n, _, _, grp)::tt when n = -x -> Some X_false
                | Enum_pv(n, _, _, grp)::tt when n>0 ->
                    let peers = if n>0 then get_peersg grp else []
                    if -n = x || (n<>x && memberp x peers) then Some X_false
                    elif memberp -x peers then Some X_true // Redundant factor in term  (e.g. x!=4 when we know x==3) 
                    else trima tt
                | _::tt -> trima tt
        trima known.facts

        
    let ubild c = function
        | Enum_not_an_enum -> c
        | Enum_pv(n, v, mm, grp) -> n ::c        
        | other -> sf ("ubild other " + enumfToStr other)

    let delve arg =
        match arg with
        | W_cover(cover, _) ->
            let srand x c =
                // x==3 typically has peers x==3, x==4, x==5. We strike invalid if b is any of the peers of the peers of except a itself.
                // We delete factors such as !(x==4) when we know x==3.
                let rec trim = function
                    | [] -> x::c
                    | Enum_pv(n, _, _, grp)::tt when n=x -> c
                    | Enum_pv(n, _, _, grp)::tt when n = -x -> raise Invalid
                    | Enum_pv(n, _, _, grp)::tt when n>0 ->
                        let peers = if n>0 then get_peersg grp else []
                        if -n = x || (n<>x && memberp x peers) then raise Invalid
                        elif memberp -x peers then c // Redundant factor in term  (e.g. x!=4 when we know x==3) 
                        else trim tt
                    | _::tt -> trim tt
                trim known.facts

            let sror cc cube =
                let ans = try (List.foldBack srand  cube [])::cc
                          with Invalid -> cc
                ans:cover_t
            let cover1 = List.fold sror [] cover
            let cover2 = e1_simplif cover1 [List.fold ubild [] known.facts]
            //vprintln 0 ("Big 1")
            let ans = xi_cover cover2
            //vprintln 0 ("Big 2 " + xbToStr ans)            
            ans 

        | W_bnode(V_bxor, [l; r], inv, meo) ->
            let l' = simplifyb_ass (known:known_t) depth l
            let r' = simplifyb_ass (known:known_t) depth r
            ix_xor l' r'
            
        | W_bdiop(oo, lst, inv, meo) ->
            let c = [[meo.n]]
            let ans = e1_simplif c [List.fold ubild [] known.facts]
            let _ = vprintln // 0 ("bdiop clause: simplifyb_ass " + xbToStr arg + " gave mid " + covToStr ans)  
            xi_cover ans


        | W_bitsel _ 
        | X_true | X_false | X_dontcare -> arg


        | other ->        
            //let efo = determine_efo other
            vprintln 0 ("+++simplifyb_ass delv other " + xbkey other + "  " + xbToStr other)
                                // + " efo=" + enumfToStr efo)
            //let gg = simplify_assuming_efos known other efo   // pass in new efo ?
            other

    let oxa = ox nn
    let ans = if oxa<>None then valOf oxa else delve arg
#if SA_SSUMING_TRACE_ENABLED
    let _ = vprintln 4 ("simplifyb_ass save dp result nn=" + i2s nn + "; ans=" + xbToStr ans)
#endif
    let _ = known.dp_xb.Add(nn, ans)
    ans

// The depth field here is set to a +ve number which is a limit of how deep to look for simplifcations.
and simplify_ass_b gl depth arg =
    let known = List.fold determine_efo (new_known_dp []) gl
    //vprintln 0 ("simplify_ass_b gl=" + sfold xbToStr gl + " known=" + sfold enumfToStr known.facts + ";")
    simplify_ass known depth arg


and simplifyb_ass_b gl depth arg =
    let known = List.fold determine_efo (new_known_dp []) gl
    simplifyb_ass known depth arg

(*
 *  query or mux2 : these are heavily used and a strong normal form is needed:
 *   Swap to right: (g0, (g1, T1, F1), fv) becomes (g0&&g1, T1, (g0, F1, fv))
 *
 *   We consider bubble-sorting the resulting linear list of muxs so that the elide tv rule operates, but given that the sort
 * order ...
 *

 * Sorting Q(gg, tv, Q(g1, tv1, fv1)) is pushed down --> Q(g1 && !gg, tv1, Q(gg, tv, fv1)) 
 *
 *   Also, we normalise the sort order of left and right args.  This gives a fullish normal form ?
 *
 *   We could also consider a BDD-like ordering where the sort order of the guards is considered, but we discard
 *   that since our main aim is to have each leaf value referenced only once and to do boolean minimisation on the
 *   guard conditions.
 *
 *   It is also possible to do a simplify_assuming on nested guards...
 *
 * 
 *)
// def>xi_query
and xi_query (g0, tv, fv) = ix_query g0 tv fv

and ix_query g0 tv fv =
    let _ = mutinc profile_q0 1
    let w = (ref 0)
    let ans = xi_querya w (new_known_dp []) (g0, tv, fv)
    // vprintln 0 ("Query call depth = " + i2s(!w))
    ans

and xi_querya w (knwn:known_t) = function
  | (g0, tv, fv) ->
    let _ = mutinc profile_qa 1
    let sorting = false
    let _ = mutinc w 1
    let bool_answer g0 p q = xi_blift (ix_or (ix_and g0 p) (ix_and (xi_not g0) q))
#if XI_QUERY_TRACING_ENABLED
    vprintln 0  ("Query 0 g " + xbToStr g0)
    vprintln 0  ("Query 0 t " + xToStr tv)
    vprintln 0  ("Query 0 f " + xToStr fv)
    let _ = if xbToStr g0 = "{[46/Ttpa0.3_V_0<11]; [-46/!(Ttpa0.3_V_0<11)]}" then muddy "fof"
#endif
    let g0 = simplifyb_ass knwn g_maxdepth g0

    match (g0, tv, fv) with
        | (X_true, tv, R) -> tv
        | (X_false, Q, fv) -> fv

        | (g0, X_blift p, X_blift q) -> bool_answer g0 p q        
        | (g0, X_blift p, q) when xi_onezerop q -> bool_answer g0 p (if xi_isnonz q then X_true else X_false)
        | (g0, p, X_blift q) when xi_onezerop p -> bool_answer g0 (if xi_isnonz p then X_true else X_false) q


        | (g0, X_blift p, q) when ewidth "aa query" q = Some 1 -> bool_answer g0 p (xi_orred q)
            
        | (g0, p, X_blift q) when ewidth "bb query" p = Some 1 -> bool_answer g0 (xi_orred p) q
           

//      | (g0, p, X_blift q) ->            muddy ("query blift dd " + xbToStr g0 + " t=" + xToStr p + " f=" + xbToStr q + " key=" + xkey p)



        | (g0, W_query(g1, T1, F1, _), fv) when !g_sorting1 ->   // Swap left sub-query to right: Q(g0, Q(g1, t1, f1), fv) -->  Q(g0 && g1, t1, Q(g0, f1, fv))
            let conj = ix_and g0 g1
#if XI_QUERY_TRACING_ENABLED
            vprintln 0 ("Conj " + xbToStr g0 + "  " + xbToStr g1 + "   ----> " + xbToStr conj)
#endif
            let knwn' = determine_efo knwn (xi_not conj)
            xi_querya w knwn (conj, T1, xi_querya w knwn' (g0, F1, fv)) // can pass down known false


        | (gg, tv, W_query(g1, tv1, fv1, _))  when x_eq tv tv1 -> // rhs query same value
            let ggg = ix_or gg g1 // OR combine case
#if XI_QUERY_TRACING_ENABLED
            vprintln 0 ("start xi_query elide 2: gg=" + xbToStr gg + "\n g1=" + xbToStr g1 + "\n ggg=" + xbToStr ggg)
#endif
            let ans = xi_querya w knwn (ggg, tv1, fv1) // Rule to combine/elide adjacent equal tv values.
            ans

        | (gg, X_undef, W_query(g1, tv1, fv1, _)) ->  // X_undef not at bottom case: if this is don't care could just forget this mux completely... please explain.
              let gf = ix_and g1 (xi_not gg)
#if XI_QUERY_TRACING_ENABLED
              vprintln 0 ("Push down undef conj g1=" + xbToStr g1 + "  gg=" + xbToStr gg + "   ----> g1 && !gg = " + xbToStr gf)
              vprintln 0 ("Push down undef tv=" + xToStr tv + "\n tv1=" + xToStr tv1 + "\n fv1 = " + xToStr fv1)
              vprintln 0 ("Push down undef g1=" + xbToStr g1 + "\n gg=" + xbToStr gg + "\n ans = " + xbToStr gf)
#endif
              let knwn' = determine_efo knwn (xi_not gf)
              xi_querya w knwn (gf, tv1, xi_querya w knwn' (gg, tv, fv1)) // Can pass in known false


        | (gg, tv, W_query(g1, tv1, fv1, _)) when !g_sorting2 ->  // rhs query lexographical nest sorting.

          if (x2order tv < x2order tv1) then // RHS sort case
              let gf = ix_and g1 (xi_not gg)
#if XI_QUERY_TRACING_ENABLED
              vprintln 0 ("Push down conj g1=" + xbToStr g1 + "  gg=" + xbToStr gg + "   ----> g1 && !gg = " + xbToStr gf)
              vprintln 0 ("Push down tv=" + xToStr tv + "\n tv1=" + xToStr tv1 + "\n fv1 = " + xToStr fv1)
              vprintln 0 ("Push down g1=" + xbToStr g1 + "\n gg=" + xbToStr gg + "\n ans = " + xbToStr gf)
#endif
              let knwn' = determine_efo knwn (xi_not gf)
              //let knwn' = muddy "e_and (blitco(xi_not gf)) knwn"
              xi_querya w knwn (gf, tv1, xi_querya w knwn' (gg, tv, fv1)) // Can pass in known false
          else 
               (
#if XI_QUERY_TRACING_ENABLED
               vprintln 0 ("match yet noop");
#endif
               xi_query1 w (gg, tv, fv) 
               )

//see below        |   (gg, tv, fv) when (xb2nn gg) < 0 -> xi_query1 w (xi_not gg, fv, tv) // Positive guard case (TODO check interaction with g_sorting1)

        |   (gg, tv, fv) -> xi_query1 w (gg, tv, fv)

and xi_query1 w (gg, truearg, falsearg) = 
    let _ = mutinc w 1
#if XI_QUERY_TRACING_ENABLED
    vprintln 0  ("Query " + xbToStr gg)
#endif
    if truearg=falsearg then
       (
#if XI_QUERY_TRACING_ENABLED           
         vprintln 0 ("equijoin over g=" + xbToStr gg);
#endif
         truearg 
       )
            
    // elif false && (x2order truearg < x2order falsearg) then // This is the lexo sorting clause: gives negative guards quite a bit.
    elif xb2nn gg < 0 then // Positive guard case (TODO check interaction with g_sorting1)
#if XI_QUERY_TRACING_ENABLED
         vprintln 0 ("xi_query1: negating " + xbToStr gg)
#endif
         let ggbar = xi_not gg
#if XI_QUERY_TRACING_ENABLED
         vprintln 0 ("xi_query1: flip polarity (2)")
#endif
         let ans = xi_query2 w (ggbar, falsearg, truearg)//Need to ensure default value, in last position, is always lowest. : Causes new elide so should call sand 1 TODO
#if XI_QUERY_TRACING_ENABLED
         vprintln 0 ("xi_query1: flipped polarity (2)")
#endif
         ans
    else xi_query2 w (gg, truearg, falsearg)

and xi_query2 w (gg, truearg, falsearg) =
    let m = "xi_query"
    let _ = mutinc profile_q2 1
    let gg' = gg (* xi_simplif X_undef m gg *)
    let kk = (xb2nn gg, x2nn truearg, x2nn falsearg)
    let (found, ov) = query_SPLAY.TryGetValue kk
    if found then
           (
#if XI_QUERY_TRACING_ENABLED
             vprintln 0 ("Reused mux " + xToStr ov);
#endif
             ov
           )
    else
          let nn = nxt_it()
          let ans = W_query(gg', truearg, falsearg, { (*hash__=0;*) n=nn; sortorder=nn })
          query_SPLAY.Add(kk, ans) // , Djg.WeakReference(ans::ovl)
#if XI_QUERY_TRACING_ENABLED
          vprintln 0 ("Generated mux nn=" + i2s nn + " " + xToStr ans)
#endif
#if NNTRACE
          vprintln 0 ("Generated mux nn=" + i2s nn + " " + xToStr ans)
#endif
          ans

and ix_diop prec oo l r =
    //vprintln 0 ("ix_diop:  prec=" + prec2str prec + " l=" + xToStr l + " oo=" + f1o3(xToStr_dop(oo)) + "  r=" + xToStr r + " call")
    let ans = ix_diop_w  prec oo l r
    //vprintln 0 ("ix_diop:  prec=" + prec2str prec + " l=" + xToStr l + " oo=" + f1o3(xToStr_dop(oo)) + "  r=" + xToStr r + " gives ans=" + xToStr ans)
    ans

    
and ix_diop_w prec oo l r =
    match oo with
    | V_bitand -> (ix_bitand l r):hexp_t
    | V_bitor  -> ix_bitor l r
    | V_xor    -> ix_bitxor l r
        
    | V_lshift -> ix_lshift l r
    | V_rshift signed -> ix_rshift_both signed l r

    | V_plus   -> ix_plus l r

    | V_minus  ->
            let ans = ix_minusp prec l r
            //vprintln 0 ("xi_minus " + xToStr r + " -->> " + xToStr (xi_neg r))
            //vprintln 0 ("xi_minus returns " + xToStr ans)
            ans
    | V_divide -> ix_dividep prec  l r
    | V_exp    -> ix_expp    prec  l r
    | V_times  -> ix_timesp  prec  l r
    | V_mod    -> ix_modp    prec  l r
    | V_interm -> xint_store_in (mine_prec g_bounda r) V_interm [l; r]
    | oo -> sf(sprintf "bad generate for diadic operator: ix_diop " + f1o3(xToStr_dop oo))

and ix_monop prec oo arg =
    match oo with
        | V_neg       -> xi_neg prec arg
        | V_onesc     -> xi_onesc prec arg
        | V_cast cvt_power -> ix_casting None prec cvt_power arg
        | oo -> sf (sprintf "bad oo=%A for monadic operator" oo)


// xi_node and ix_diop are were confused - only want one - user code should always come in via ix_diop ix_monop or ix_op.
and ix_op prec oo lst =
    match oo with
        | V_neg when length lst = 1   -> ix_monop prec oo (hd lst)
        | V_onesc when length lst = 1 -> ix_monop prec oo (hd lst)
        | V_cast  cvt_power when length lst = 1  -> ix_monop prec oo (hd lst)                
        | oo ->
            match lst with
                | [l;r] -> ix_diop prec oo l r 
                | a::b::cc when (oo=V_plus || is_times oo || oo=V_bitor || oo=V_bitand || oo=V_xor) -> // Commutative+Associative fold - sort list first - sometimes we want balanced fold (e.g. in restructure) but here we go for left associative and could sort first for better normal form
                    //dev_println (sprintf "Polyadic oo=%A" oo)
                    ix_diop prec oo a (ix_op prec oo (b::cc))
                
//  xi_node prec oo lst = // DELETE ME -  xi_node and ix_diop are were confused - only want one - user code should always come in via ix_diop ix_monop or ix_op


and ix_bitand_prec prec p q   = xint_store_in prec V_bitand [p; q]
and ix_bitand p q   = xint_store_in (infer_prec_no_float p q) V_bitand [p; q]
and ix_bitor  p q   = xint_store_in (infer_prec_no_float p q) V_bitor  [p; q]
and ix_bitxor p q   = xint_store_in (infer_prec_no_float p q) V_xor    [p; q]
and ix_lshift l r   = if r=xi_zero then l else xint_store_in (mine_prec_no_float l)        V_lshift [l; r]

// Note that 1<<1u is unsigned in return value since the unsignedness of the r.h.s. operand also contributes.
and ix_rshift ss l r   = if r=xi_zero then l else xint_store_in (infer_prec_no_float l r)        (V_rshift ss) [l; r]
and ix_rshift_both ss l r =     if r=xi_zero then l else xint_store_in (infer_prec_no_float l r) (V_rshift ss) [l; r]




//
// Determine if an expression is unsigned. 
//
// 
and xi_is_unsigned arg =
    let p = mine_prec false arg
    (p.signed=Unsigned)
#if OLD_DEPRECATED_WAY
//let g_dynp_unsigned = Array.create g_mya_limit None
    //Probably it is now better to use mine_prec.
    let k = x2nn arg
    let ov = if k=None then None else g_dynp_unsigned.[valOf k]
    // let _ = if ov<>None then vprintln 20 ("Recalled from dp " + xToStr N + " as " +  boolToStr(valOf ov))
    if ov<>None && true then valOf ov
    else
        let rec jk n =
            match n with
                |               (X_bnet ff) -> ff.signed = Unsigned
                |                 (X_net _)
                |                 (X_num _) 
                |              (W_string _)
                |                (X_bnum _) -> false  
                | W_node(caster, V_cast cvt_power,  arg, _) -> caster.signed = Unsigned
// Diadic arithmetic operators are only signed when both args are.
// But we do not need to recurse here - we can get from the stored prec.
                |        W_node(prec, oo, lst, _) -> prec.signed = Unsigned // disjunctionate xi_is_unsigned lst
                |       W_query(g, l, r, _) -> xi_is_unsigned l || xi_is_unsigned r

                |          X_pair(_, e, _)
                |             X_x(e, _, _)
                |        W_asubsc(e, _, _) -> xi_is_unsigned e
                |    W_apply(fgis, lst, _) -> false // for now - fgis needs to store this
                |          W_valof(lst, _) -> false // for now
                |                X_undef   -> false
                |                X_blift b -> false
                |                    other -> sf ("xi_is_unsigned_c other:" + xkey other + " " + xToStr other)

        let r = jk (arg)
        let _ = if k=None then () else Array.set g_dynp_unsigned (valOf k) (Some r)
        // vprintln 0 ("xi_is_unsigned_c " + xToStrN + " is ans=" + i2s r)
        r
#endif

and xi_is_signed arg = not (xi_is_unsigned arg)

and xi_signed_depreciated_ arg =  if xi_is_unsigned arg then Unsigned else Signed  // Cannot cope with FP

// Arithmetic negate (subtract from zero).
and xi_neg prec d = // Note: some neg functionality is in xint_store_i
    let signed = Signed
    let rec toBigN = function // silly local function
        | X_num n  -> BigInteger n
        | X_bnum(_, l, _) -> l
            
        // negate of strings is not defined.
        // blift of true or false is stored as an integer so does not appear here.
        | v -> muddy("xi_neg constants other: " + xToStr v)
    let rec handle = function

        | W_node(caster, V_cast cvt_power, [arg], _) when caster.signed <> FloatingPoint && (mine_prec false arg).signed <> FloatingPoint -> // We fold negate inside a casting, aiming to hit a normal form.  But negate for integers and f/p is different, so should not be done in such cases.
            let v = handle arg
            ix_casting None caster cvt_power v

        | d ->
          match classed_constantp d with
            | Constant_int ->
                let ans = perform_bnum_arith (mine_prec false d) V_minus 0I (toBigN d)
                //vprintln 0 ("xi_neg const " + xToStr ans)
                ans
            | Constant_fp ->
                let ans = ix_const_generic_arith { widtho=None; signed=FloatingPoint} V_minus g_dp_zero d
                //vprintln 0 ("xi_neg const fp " + xToStr ans)
                ans
            | ot ->
                match d with
                // For a real normal form, we fold this in over certain diops.
                | W_node(prec, V_neg, [item], _) -> item
                | W_node(prec, V_plus, lst, _) -> xint_store_in prec V_plus (map (xi_neg prec) lst)            

                // We should really scan for negated terms when forming a product and ensure that the first is.
                | W_node(prec, V_times, h::t, _) -> xint_store_in prec V_times ((xi_neg prec h) :: t)            
                | other ->
                    let ans = xint_store_in (mine_prec false d) V_neg [d]
                    //vprintln 0 ("xi_neg leaf store: " + xToStr ans)
                    ans
    handle d
    
and ix_minus l r = ix_plus l (xi_neg (mine_prec g_bounda r) r)

and ix_minusp prec l r = ix_plusp prec l (xi_neg prec r)

and ix_exp ll rr = ix_expp (mine_prec g_bounda ll)  ll rr
    
and ix_expp prec ll rr =
    if arith0_pred rr then xi_num 1 // Even 0^0 is 1 - Quoting Don Knuth: "If I take away all your apples one or more times you have no apples left; but if I take away all your apples 0 times, you still have all your apples!"
    elif arith0_pred ll then ll
    elif arith1_pred rr then ll
    elif ll=X_undef || rr=X_undef then X_undef    
    elif constantp ll && constantp rr then 
       let rec doit = function
           | X_num n   -> BigInteger n
           | X_bnum(_, bn, _) -> bn
           | v -> sf("xi_exp constants other form: " + xToStr v)
       perform_bnum_arith prec V_exp (doit ll) (doit rr) // r will be undone straight away!
    else xint_store_in prec V_exp [ll; rr]

and ix_mod ll rr = ix_modp (infer_prec g_bounda ll rr) ll rr

and ix_modp prec ll rr =
    if arith0_pred rr then cleanexit "ix_mod by zero"
    elif arith1_pred rr then xi_zero
    elif ll=X_undef || rr=X_undef then X_undef
    else xint_store_in prec V_mod [ll; rr]

and ix_times l r = ix_timesp (infer_prec g_bounda l r) l r

and ix_timesp prec l r =    
    let ans = 
        if arith0_pred r then r
        elif arith0_pred l then l
        elif l=X_undef || r=X_undef then X_undef
        elif arith1_pred r then l
        elif arith1_pred l then r
        elif arith_m1_pred l then xi_neg prec r
        elif arith_m1_pred r then xi_neg prec l
        else xint_store_in prec V_times [l; r]
    //vprintln 0 ("Times " + xToStr_plus_key l + " multiplied with " + xToStr_plus_key r + " gives " + xToStr ans) 
    ans

and xi_timesl signed__ = function
    | [item] -> item
    | a::b::c -> xi_timesl signed__ ((ix_times a b) :: c)

and wide_divmod1 (l:BigInteger) (r:BigInteger) = 
    let (sl, sr) = (l.Sign < 0, r.Sign < 0)
    let num = if sl then 0I - l else l
    let den = if sr then 0I - r else r
    let rem = ref 0I
    let quot = BigInteger.DivRem(num, den, rem)
    (sl, sr, quot, !rem)


// Divide node constructor.
and ix_divide ll rr = ix_dividep (infer_prec g_bounda ll rr) ll rr

and ix_dividep sprec ll rr = 
    let backstop(n, d) =
        if arith1_pred d then n                  // divide by 1 returns numerator.
        elif arith_m1_pred d then xi_neg sprec n // divide by -1 is a negate/
        elif n=X_undef || d=X_undef then X_undef
        else
            //let lst0 = [ll; rr]
            //vprintln 0 (sprintf "ix_divide: peep args keys=%s  vals=%s" (sfold xkey lst0) (sfold xToStr lst0))
            xint_store_in sprec V_divide [n; d]
    let int_const_den = int_constantp rr
    let manit = xi_manifest_int "ix_divide" 
    let adiv = function // We distribute division over multiplication here: useful for array subscript computation simplification but possibly not ideal for floating point.
        | W_node(prec, V_times, lst, _) ->
            if memberp rr lst then xint_store_in prec V_times (list_delete_once(lst, rr)) // somewhat arbitrary choice of precision here! TODO.
            else backstop(ll, rr)
        | other -> backstop(ll, rr)

    let adiv_constant_denominator den = function
        | W_node(sprec, V_times, lst, _) ->
            let rec ff den = function
                | [] -> ([], den)
                | x::tt when constantp x ->
                    let (sl, sr, qt, rdr) = wide_divmod1 (manit x) den
                    let zsign r = if sl<>sr then 0I - r else r
                    let goes = rdr.IsZero // really want hcf here! todo.
                    if goes then
                        let hcf = 1I // TODO  euclid_gcd_bn 
                        let (tt', den') = ff hcf tt
                        let x' = zsign qt
                        ((if 1I =  x' then tt' else (xi_bnum x')::tt'), den')
                    else 
                        let (tt', den') = ff den tt
                        (x::tt', den')

                | x::tt ->
                    let (tt', den') = ff den tt
                    (x::tt', den')
                    
            if memberp rr lst then xint_store_in sprec V_times (list_delete_once(lst, rr)) // If denominator is a member of the numerator product list
            else
                let (lst', den') = ff den lst
                if nullp lst' then backstop(xi_num 1, gec_X_bnum({widtho=None; signed=Signed}, den')) 
                else backstop(xi_timesl sprec lst', gec_X_bnum({widtho=None; signed=Signed}, den'))

        | other -> backstop(ll, rr)

    let ans =
        if arith0_pred rr then cleanexit (sprintf "ix_divide by zero. Numerator=%s" (xToStr ll)) 
        elif ll=X_undef then ll                  
        elif ll=rr then
            let _ = vprintln 3 (sprintf "ix_dividep: ll=rr - return unity %s %s" (prec2str sprec) (xToStr ll))
            if sprec.signed=FloatingPoint && sprec.widtho = Some 64 then g_dp_one
            elif sprec.signed=FloatingPoint && sprec.widtho = Some 32 then g_sp_one
            else xi_one 
        elif int_constantp ll && int_const_den then perform_bnum_arith sprec V_divide (manit ll) (manit rr)
        elif int_const_den then adiv_constant_denominator (manit rr) ll
        // TODO: floating constant denominator is sensibly converted to multiply by reciprocal
        else adiv ll

    //vprintln 0 (sprintf "ix_dividep prec=%s int_const_den=%A den=%s ans=%s" (prec2str sprec) int_const_den (xToStr rr) (xToStr ans))
    ans

let ix_bool_divide num den =
    match (num, den) with
        | (_, X_true) -> Some(num, X_false)
        | (W_cover(nc, _), W_cover([dcube], _)) ->
            match bool_divide_cover nc dcube with
                | None -> None
                | Some (quot, remainder) -> Some(xi_cover quot, xi_cover remainder)
        | _ -> muddy (sprintf "ix_bool_divide: other form: num=%s den=%s" (xbToStr num) (xbToStr den))

//
//
let mya_bunpak_linp(v, LIN(s, lst)) =
    let vd = -1
    if lst = [] then (if s then X_true else X_false) else 
    if s && length lst = 2 && hd lst + 1 = hd(tl lst) 
    then ix_dned v (xi_num(hd lst))
    else
        let t0 = if s then [xi_dltd(v, X_num(hd lst))] else []
        let rec j = function
            | ([], c) -> (c, None)
            | ([item], c) -> (c, Some item)
            | (a::b::tt, c) ->
                if b=a+1 then j(tt, xi_deqd(v, X_num a) :: c)
                else j(tt, ix_and (xi_dged(v, X_num a)) (xi_dltd(v, X_num b)) :: c)

        let (t1, tail) = j ((if s then tl lst else lst), [])
        let t2 = if tail<>None then [xi_dged(v, X_num(valOf tail))] else []
        let rec k =
            function
                | [] -> X_false
                | [a; b] -> ix_or a b
                | (h::t) -> ix_or h (k t)
        let ans = k (t0 @ t1 @ t2) 
        if vd>=4 then vprintln 4 ("uplinp " + xbToStr ans)
        ans 



(* Return updated nets from an l%e expression. 
 *
 * If a constant subscript is used for array update, this must intersect with a variable subscript
 * read and vice versa.  We achieve this by making a double entry in the support list for
 * constant indexes but only a single entry in the driven list.  See REF1.
 *
 * In cone refine we need to chase dependencies to filter the trips and also filter the retained var list.
 *
 * Test 4 writes with constant subs and reads with a var subs ... 
 *)


(*
 * The driven_bev routine(s) return variables that are assigned in a bev block.
 *) 
let rec xi_driven_e cc e = cc (* For now - but e could be side-effecting thing in the future*)

and xi_bdriven_e cc bexp = cc (* For now - but e could be side-effecting thing in the future*)

and xi_driven_bev cc = function
          | Xswitch(e, lst)  -> List.fold (fun c (a, cmd) -> xi_driven_bev c cmd) cc lst
          | Xif(a, b, c)     -> xi_bdriven_e (xi_driven_bev (xi_driven_bev cc b) c) a
          | Xwhile(a, b)     -> xi_bdriven_e (xi_driven_bev cc b) a
          | ((Xassign(l, r)) as arg)  ->
              let _ = vprintln // 0 ("TTT xi_driven: " + hbevToStr arg)
              xi_driven (xi_driven_e (xi_driven_e cc r) l) l
          | Xannotate(_, a)
          | Xlinepoint(_, a) -> xi_driven_bev cc a
          
          | Xwaituntil(b) -> xi_bdriven_e cc b

          | Xblock(l) 
          | Xpblock(l) -> List.fold xi_driven_bev cc l

          | Xwaitabs(_) // Strictly we should look for side-effecting expressions, such as hpr_interlocked_add in all of the support.
          | Xwaitrel(_) 
          | Xgoto _ 
          | Xdominate _ 
          | Xfork _ 
          | Xcomment(_) 
          | Xlabel(_) 
          | Xreturn _ 
          | Xskip 
          | Xbreak 
          | Xjoin _ 
          | Xcontinue 
          | Xbeq(_, _, _) -> cc

          //| Xcall(fgis, args) -> cc (* for now *)
          | Xeasc s -> cc (* for now *)
          | other -> sf("match: hplrs_lsupport_bev:" + (hbevToStr other))

(*
 * Driven: call this on an lhs expression please. It gives variables assigned in an lhs expression.
 * Like the r-mode version, this function does not consider side effecting expressions at the moment, such as hpr primitives that modify their arguments, such a 'read' system call or hpr_interlocked_add ... TODO
 *)
and xi_driven cc nN = 
    let rec idr cc nN =
        match nN with
        | X_bnet _   
        | X_net _           -> sd_add cc (nN, [])
        | X_x(xx, p_, _)    -> idr cc xx // The X_x alters the way that a net is driven, but it is still driven.
        | W_asubsc(l, r, _) -> if constantp r then sd_add cc (l, [r])
                               else sd_add cc (l, [])

        | X_blift(W_bsubsc(l, r, _, _)) -> if constantp r then sd_add cc (l, [r])
                                           else sd_add cc (l, [])
                    
        | X_blift(W_bitsel(l, n, _, _)) -> sd_add cc (l, [xi_num n])

        | X_undef -> cc
        | other -> sf ("xi_driven other: k=" + xkey other + " v=" + xToStr other)
    let ans = idr cc nN
(*
let _ = vprint(3, "xi_driven " + xintToStr(XINT_MYA_AR.[k])) + " is " + sfold (xiToStr) (xi_valOf r) + "\n")
let _ = vprint(3, "xi_driven ans=" + sfold (xiToStr) (xi_valOf ans) + "\n")

*)
    ans



(*
 * The support of an expression is the list of (free) variables occurring in it.
 * Support is represented using so-called sd pairs: the net itself and a subscript or bit lane.
 * A null subscript or bit lane means all of it, or variable parts of it.
 *
 *)
let rec xi_bsupport cc arg = 
    let k = xb2nn arg
    let key = abs k
    let (found, ov) = g_dynp_support.TryGetValue key
    //if ov<>None then vprintln 20 ("Recalled from dp " + xToStrarg + " as " +  sfold (fun (a,_)->xToStr a) (valOf ov))
    if found then sd_union(ov, cc)
    else
        let rec jb c = function
            | W_cover(lst, _)        -> List.fold xi_bsupport c (map deblit (List.fold cube_support [] lst))
            | W_bdiop(_, lst, _, _)  -> List.fold xi_support c lst
            | W_bnode(oo, lst, _, _) -> List.fold jb c lst
            | W_bmux(gg, ff, tt, _)  -> jb (jb (jb c tt) ff) gg
            | W_bitsel(n, _,_, _)     
            | W_linp(n, _, _)        -> xi_support c n

            | W_bsubsc(L, r, _, _)   -> if constantp r then sd_add c (L, [r])
                                        else xi_support (sd_add c (L, [])) r
            | X_false
            | X_true   
            | X_dontcare -> c

        let rr = if bconstantp arg then [] else jb [] arg
        let (found, ov) = g_dynp_support.TryGetValue key
        let _ = if not found then g_dynp_support.Add(key, rr)
        //dev_println  ("xi_bsupport " + xbToStr arg + " is ans=" + sfold (fun (a, _)->xToStr a) rr)
        let ans = sd_union(rr, cc) // Do separate union for DP reasons.
        ans 
 
and xi_support_lmode cc = function 
    | W_asubsc(l, r, _) -> xi_support cc r
    | _ -> cc

and xi_support cc0 arg = 
    let k = x2nn arg
    let key = abs k
    let r0 = 
        let (found, ov) = g_dynp_support.TryGetValue key
        if found then ov
        else
            let r1 =
                if constantp arg then [] // TODO dp on constantp ?
                else
                    let c = []
                    match arg with
                        |                (X_bnet _) 
                        |                 (X_net _) -> sd_add c (arg, [])
                        |(W_string(_, XS_withval v1,  _)) -> xi_support c v1
                        |                 (X_num _) 
                        |                   X_undef            
                        |              (W_string _) 
                        |                (X_bnum _) -> c
                        |   W_node(prec, _, lst, _) -> List.fold xi_support c lst
                        |           X_pair(l, r, _) -> xi_support (xi_support c r) l
                        |       W_query(g, l, r, _) -> xi_bsupport (xi_support (xi_support c r) l) g
                        |              X_x(e, _, _) -> xi_support c e
                        |         W_asubsc(L, r, _) -> if constantp r then sd_add c (L, [r]) else xi_support (sd_add c (L, [])) r
                        |     W_apply(fgis, _, lst, _) -> List.fold xi_support c lst // TODO can ignore out-only args.
                        |           W_valof(lst, _) -> List.fold xi_support_bev c lst
                        |                 X_blift b -> xi_bsupport c b
                        |                   (other) -> sf ("xi_support other:" + xkey other + " " + xToStr other)

            //dev_println (sprintf "xi_support for hexp n=%i %s is %s" arg.x2nn (xToStr arg) (sfold (fun (a,_)->xToStr a) r1))
            let (found, ov) = g_dynp_support.TryGetValue key // It may have been added in the meantime.
            let _ = if not found then g_dynp_support.Add(key, r1)
            r1

    let ans = sd_union(r0, cc0) // Do separate union for DP reasons 
    ans 

and xi_support_bev cc = function
         | Xswitch(e, lst) -> xi_support (List.fold (fun c (a, b)->xi_support_bev (List.fold xi_support c a) b) cc lst) e
         | Xif(a, b, c) -> xi_bsupport (xi_support_bev (xi_support_bev cc c) b) a
         | Xwhile(a, b) -> xi_bsupport (xi_support_bev cc b) a
         | Xassign(l, r) -> xi_support_lmode (xi_support cc r) l
         | Xlinepoint(_, a) 
         | Xannotate(_, a) -> xi_support_bev cc a
         | Xwaituntil a -> xi_bsupport cc a
         | Xbeq(g, _, _) -> xi_bsupport cc g
         | Xblock l 
         | Xpblock l -> List.fold xi_support_bev cc l

         | Xwaitabs e 
         | Xwaitrel e
         | Xeasc e
         | Xreturn e -> xi_support cc e
         | Xgoto _ 
         | Xdominate _ 
         | Xfork _ 
         | Xcomment _
         | Xbreak
         | Xjoin _
         | Xskip 
         | Xlabel(_)
         | Xcontinue -> cc
         //| Xcall(fgis, l) -> List.fold xi_support cc l

         | other -> sf("match: xi_support_bev:" + hbevToStr other)



(*
 * lsupport is r-mode variables appearing in an l-mode expression, such as subscripts. 
 * This could be needed as a slave to xi_support_bev: not used by lsupport_bev.
 * Instead we allow xi_support_bev to return l and r %e expressions and the 
 * pure r%e expressions can always be found by subtracting the xi_driven (assuming no read before write!)
 *)
let xi_lsupport cc arg =
    let rec jk = function
        | W_asubsc(l, subsc, _) 
        | X_blift(W_bsubsc(l, subsc, _, _)) -> xi_support cc subsc        

        | X_blift(W_bitsel _)
        | X_undef 
        | X_net _ 
        | X_bnet _ -> cc

        | X_x(v, _, _) -> jk v
        | other -> sf ("xi_lsupport other: " + xToStr other)
    let ans = jk arg
    ans
    
let rec xi_lsubexps cc = function
    | W_asubsc(l, subsc, _) 
    | X_blift(W_bsubsc(l, subsc, _, _)) -> subsc :: cc
    
    | X_undef 
    | X_net _ 
    | X_bnet _ -> cc

    | X_x(v, _, _) -> xi_lsubexps cc v
    | other -> sf ("xi_lsubexps other: " + xToStr other)




// RTL output inside a Verilog cat would like a sized number.  This makes an unsized number.  It may
// be better design to keep bit masks as a first-class construct rather than resorting to anding.    
let ix_mask signed width v =
    let old = false
    if old then
        //vprintln 0 ("Starting himask " + i2s width)    
        let mm = gec_X_bnum({widtho=Some width; signed=signed}, himask width)
        //vprintln 0 ("Made himask " + i2s width)
        ix_bitand v mm 
    else
        ix_casting None { g_default_prec with widtho=Some width; signed=signed; } CS_maskcast v
        
let xi_abdiop = function
          | (V_band, l, r) -> ix_and l r
          | (V_bor,  l, r) -> ix_or l r
          | (V_bxor, l, r) -> ix_xor l  r



type strict_t = (stutter_t * hbexp_t)


// Skolem code: an old method of implementing conditional rewrites: we now use the agents instead.   
// For Boolean rewrites, the answer is/was subject to 'simplify assuming' the strict condition but we can do that in the abstracte.rewrite_rtl function now.
type skolem_t =
    {   timebase: int;
        strict: strict_t;  // An additional value to pass along.
        xr :hexp_t option;
    }


let g_sk_null:skolem_t =
   {
      timebase= -1;
      strict = (None, X_true)
      xr= None
   }


let skolem_bzoing(n, skolem:skolem_t) =
    n// not yet imlemented for bools : need to choose a disable value, like orred(X_undef)
    
let skolem_zoing(n, skolem:skolem_t) =
    if (n=X_undef && skolem.xr<>None) then (sf("sz: no longer used?"); valOf skolem.xr)
    else n;


let skolem_prefix = "xCNF";

type sparer_t = Sparer

type xi_rewrite_control_t = 
| XRC_rmode   
| XRC_lmode of strict_t  // was needed for guarded rewrites - but now we carry a strict_t through rmode as well... so delete this...



(*
 * arlrec : AR mapping information elements:
 *   for an array we have a shortcut mapping for all constant subscript maps as well
 *   as algebraic mapping list.
 *)
type arlrec_t =
 {
   a_offset    : hexp_t
   scale       : hexp_t
   replacement : hexp_t
   info        : hexp_t
   //guard       : hbexp_t;  - currently the use must resolve his own predication rather than supporting a guard field here.
 }

let vanilla_arlrec =
    {
        a_offset=  xi_zero
        scale=     xi_one
        //guard=X_true;
        replacement=xi_zero;
        info=      xi_zero
    }
    
let arlrecToStr (a:arlrec_t) = "offset=" + xToStr a.a_offset + ", scale=" + xToStr a.scale + " replacement=" + xToStr a.replacement // + " g=" + xbToStr a.guard


// Array mapping forms: these are instructions on how to recode one array operation as another or as a scalar.
type AR_subsc_mapping_t =
    |  ARG1 of hexp_t * hbexp_t * hexp_t  // Substitution modification: lhs, guard, rhs    
    |  ARFF of it_t * arlrec_t            // Arithmetic modification.

type AR_mapping_t =
    // General, non-boolean, expression substitution: the basic mapping, from exp to replacement when gg holds. 
    |  ARGM of hexp_t * hbexp_t * hexp_t * bool   // Guarded mapping form with fields (exp, gg, replacement, vf).

    // The boolean form of the ARGM.
    |  ARGB of hbexp_t * hbexp_t * hbexp_t * bool // Replacement mapping for a boolean subexpression but with extra guard (used on lmode only? )

    // The recoding of subscripts form.
    |  ARL of it_t * AR_subsc_mapping_t list * bool   // For an array: list of subscript->new_value?/subscript substitutions.


let ar_subsc_mappingToStr = function
    | ARG1(expr, X_true, replacement) -> "AR(" + xToStr expr + ", " + xToStr replacement + ")"
    | ARG1(expr, gg, replacement)     -> "ARG1(" + xToStr expr + ", "  + xbToStr gg + ", " + xToStr replacement + ")"
    | ARFF(the_array, r)              -> "ARFF(" + xToStr the_array + ", " + arlrecToStr r + ")"

let arToStr = function
    | ARGM(l, gg, r, volf)  -> "ARGM(" + xToStr l + ", " + xbToStr gg + ", " + xToStr r + "," + boolToStr volf + ")"
    | ARGB(l, gg, r, volf)  -> "ARGB(" + xbToStr l + ", " + xbToStr gg + ", " + xbToStr r + "," + boolToStr volf + ")"        
    | ARL(v, lst, volf)     -> "ARL(" + xToStr v + ", " + sfold ar_subsc_mappingToStr lst + ", " + boolToStr volf + ")"
        //| (_) -> "AR??"        


let is_ARFF = function
    | (ARFF _) -> true
    | _ -> false

let is_string = function // Any expression where prec.widtho = Some -1 is a string handle too.
    | W_string _ -> true
    | _ -> false



let rec arl_merge m = function
        | ([], r) -> r
        | (l, []) -> l
        | (l, r) -> 
              if conjunctionate is_ARFF l || conjunctionate is_ARFF r then lst_union l r
              else sf("undecidable arl union: " + sfold ar_subsc_mappingToStr l + " with " + sfold ar_subsc_mappingToStr r + "\n")


(* Assoc update: has a name
 * ARL updater: we delete or guard the existing updates to a variable: caller now adds on new update.
 *
 * When a new update with guard g is applied we can mask the exisiting ones with (~g & og) which will often be false, meaning the older update is completely overriden and can be dropped.
 * Can raise a name alias ?
 *)
let arl_updater s items =
    match s with
        | ARGM(l, g, r, volf) ->
            let gb = xi_not g
            let rec asc1 d =
                match d with
                | [] -> []
                | ARGM(l', g', r', volf')::t when l<>l'-> (hd d)::(asc1 t)
                | ARGM(l', g', r', volf')::t when l=l' ->
                    let ng = ix_and gb g'
                    if ng=X_false then (asc1 t) else ARGM(l, ng, r', volf || volf')::(asc1 t)
                | other::tt -> other :: asc1 tt
            asc1 items

        | ARGB(l, g, r, volf) ->
            let gb = xi_not g
            let rec asc2 d =
                match d with
                | [] -> []
                | ARGB(l', g', r', volf')::t when l<>l'-> (hd d)::(asc2 t)
                | ARGB(l', g', r', volf')::t when l=l' ->
                    let ng = ix_and gb g'
                    if ng=X_false then (asc2 t) else ARGB(l, ng, r', volf || volf')::(asc2 t)
                | other::tt -> other :: asc2 tt
            in asc2 items
                

        | ARL(l, nv, volf) ->
             let rec qf items =
                 match items with
                     | [] -> [s]
                     | ARL(l', ov, volf')::t when l<>l' -> (hd items)::(qf t)
                     | ARL(l', ov, volf')::t when l=l'  -> ARL(l, arl_merge l (ov, nv), volf || volf')::(qf t)
                     | other::t -> other :: qf t
             qf items

        //| _ -> sf ("more complex arl updater : other")


let arl1_update__ (s, items) = // not used
        match (s, items) with
            | (ARFF(l, _), items) ->
                let rec df items =
                    match items with
                         | [] -> [s]    (* Silently replaces old ARF for l with new: no attempt at merge. *)
                         | (ARFF(l', ov)::t) ->
                             if l=l' then sf(xToStr l + " Two or more ARFs must be stored under individual entries ")
                             else (hd items) :: df t
                         | (A::t) -> A :: df t
                df items

            | (_) -> sf ("more complex arl1 update : other")




(*
 * Create an assoc for use in xint_assoc and rewrite_bev etc
 * 
 * Currently a triple: the mapping proper, the DP memory and a spare.
 *)

let AR_idx arg =
    let v =
        match arg with
        | ARGM(lv, _, _,  volf) -> x2nn lv        
        | ARL(lv, _, volf)      -> x2nn lv
        | _ -> sf "AR_idx"
    //if v=None then sf ("no nn for assoc item")
    if  v < 0 then muddy "negative association"
    else v

// Array range scale, check and regenerate.
let apply_arff recursive_apply (arlrec:arlrec_t) subs = 
    //let signed = g_default_array_subscript_precision
    let nv = ix_pair (arlrec.info) (ix_divide (ix_plus arlrec.a_offset (recursive_apply (undecorate subs))) (arlrec.scale))
#if XI_ASSOC_TRACE_ENABLE
    dev_println ("apply_arff " + xToStr subs + " -> " + xToStr nv + "   " + arlrecToStr arlrec)
#endif
    let ans = safe_xi_asubsc "apply_arff" (arlrec.replacement, nv)
    ans

(*
 * These are mutable and non-mutable structures used for the dynamic programming and mapping.
 *)
type mvec_t = Map<int, AR_mapping_t list>
type pairmap_t = Map<hexp_t, hexp_t>
type functionmap_t = Map<string, string>
let mvec() = (Map.empty:mvec_t)
type bmvec_t = Map<xidx_t, xidx_t>
let bmvec() = (Map.empty:bmvec_t)
let dpvec<'a>() = new Dictionary<int, 'a>()



let g_pairs_in_argm = true // Set this true in the future and delete the old_pairmap

//
//
let Ftree_update_arl(m:mvec_t, loc, vale) =  // use a ListStore ?
    let ov = m.TryFind(loc) 
    let nv = vale :: (arl_updater vale (valOf_or_nil ov))
    let m' = m.Add(loc, nv)
    m'
    
// arll is different because ? silly ml binding?
let Ftree_update_arll_(m:mvec_t, loc, vale) = 
    let ov = m.TryFind(loc) 
    let nv = vale :: (arl_updater vale (valOf_or_nil ov))
    //if ov <> None then m.Remove(loc)
    let m' = m.Add(loc, nv)
    m'

// meo agent provides callbacks for additional user manipulations during an assoc/walker sweep.
type arcbody_t = hbexp_t * hexp_t * hexp_t // fwd ref to XRTL Rarc
type postmap_t = strict_t -> hbev_t -> hbev_t // function to call after rewriting bev assign
type meo_rw_agent_t =
    | Agent_exp  of (hexp_t  -> bool) * (strict_t -> hexp_t -> hexp_t)    * (bool * bool)
type meo_rw_bagent_t =
    | Agent_bexp of (hbexp_t -> bool) * (strict_t -> hbexp_t -> hbexp_t)  * (bool * bool)
type meo_rw_fagent_t =
    | Agent_bev  of (hbev_t  -> bool) * (strict_t -> hbev_t -> (hbev_t * postmap_t))
    | Agent_rtl of (stutter_t * hbexp_t * arcbody_t -> bool) * (strict_t -> (hexp_t*hexp_t)  -> ((hexp_t * hexp_t) * (stutter_t * hbexp_t * arcbody_t -> arcbody_t list)))   * (bool * bool)

let strictToStr = function
    | (None, b) -> "<NoPP, " + xbToStr b + ">"
    | (Some(pc, state), b) -> "<" + xToStr pc + "=" + xToStr state + ", " + xbToStr b + ">"    

type meo_rw_t = // Expression rewrite control structure.
     { 
         xlat:          mvec_t  // The primary map from old to new expressions. TODO - these are not mutable so the b_translater one at least is not doing anything.
         b_translater:  bmvec_t // A map for boolean expressions (maps +ve blit integer int to another one but both true and negated forms are mapped.
        
         dp:            Dictionary<int, hexp_t> // Dynamic programming cache for hexp_t
         bdp:           Dictionary<int, xidx_t> // Dynamic programming cache for bools

         agents:        meo_rw_agent_t list  // Agent for special/arithmetic manipulations
         bagents:       meo_rw_bagent_t list // boolean workers
         fagents:       meo_rw_fagent_t list
         ctrl:          xi_rewrite_control_t
         pairmap_old:   pairmap_t             // Mapper just for pairs - prior to their having a meo index - can delete this I think. Yes, please just use an ARGM for them now.
         functionmap:   functionmap_t
         waypoint: (hexp_t list * hexp_t list) option ref
         remove_undef_assigns: bool
     }



(*
 * Convert a list of AR_mapping_t items to a deployable mapping.
 *)
let makemap1 lst =
#if XI_ASSOC_TRACE_ENABLE
        dev_println (sprintf "makemap1 %i items" (length lst))
#endif
        let mkmap1_add m arg  =
            let nn = AR_idx arg
            match arg with
                | ARL(L, lst, true) -> m
                | ARL(L, lst, volf) ->
                    if !g_mvd>=5 then vprintln 5 ("Adding ARL mapping for nn=" + i2s nn + " " + xToStr L + " items=" + i2s(length lst))
                    Ftree_update_arl(m, nn, arg)

                | ARGM(L, g, r, true) -> m

                | ARGM(L, g, R, volf) ->
                     if !g_mvd>=5 then vprintln 5 ("Adding guarded mapping for " + xToStr L + " to " + xToStr R)
                     Ftree_update_arl(m, nn, arg)

                | arg -> sf ("match other makemap1_add: " + arToStr arg)
        in
        {    xlat=        List.fold mkmap1_add (mvec()) lst
             waypoint=    ref None
             b_translater=bmvec()
             dp=          dpvec<hexp_t>()
             bdp=         dpvec<xidx_t>()
             pairmap_old= Map.empty;
             functionmap= Map.empty  
             agents=      []
             fagents=     []
             bagents=     []
             ctrl=        XRC_rmode
             remove_undef_assigns= false
        }

let hbexp_agent_add MM (pred, mapper) = muddy "missing"

let fresh (MM:meo_rw_t) = { MM with dp= dpvec<hexp_t>(); bdp= dpvec<xidx_t>() }


let hexp_agent_add (MM:meo_rw_t) (pred, mapper, flags) =  { fresh MM with  agents= Agent_exp(pred, mapper, flags)::MM.agents; }
    

let arc_agent_add  (MM:meo_rw_t) (pred, mapper, flags) = { fresh MM with fagents= Agent_rtl(pred, mapper, flags)::MM.fagents; }
     

let hbev_agent_add  (MM:meo_rw_t) (pred, premap) = { fresh MM with fagents= Agent_bev(pred, premap)::MM.fagents; }
    
     


(*
 * Add a new mapping item (in the form of an AR or ARL etc) to an existing mapping.
 * May need to transclose again ?
 *)
let makemap1_add (mM:meo_rw_t) aA = // (M, v, Mb, v1, ss, xrc)
    let vd = -1
    match aA with
        | ARL(L, R, true) -> mM
        | ARL(L, R, false) -> 
            if vd>=5 then vprintln 5 ("Adding mapping (1) for " + xToStr L + " ton.\n")
            let M' = Ftree_update_arl(mM.xlat, AR_idx aA, aA) 
            { fresh mM with xlat=M' }

        | ARGM(L, g, R, true)  -> mM // Strangely discard volf=true ones. Why? Please explain.
        | ARGM(L, g, R, false) ->
            if vd>=5 then vprintln 5 ("Adding mapping (1) for " + xToStr L + " g=" + xbToStr g + " to " + xToStr R + "\n")
            let xl' = Ftree_update_arl(mM.xlat, AR_idx aA, aA) 
            { mM with  dp=dpvec<hexp_t>(); bdp=dpvec<xidx_t>(); xlat=xl' }
        | (aA) -> sf ("match makemap1_add: " + arToStr aA)


// An ARG is a general expression replacement with a guard.
let gen_ARG(expr, gg, replacement_expr) = ARGM(expr, gg, replacement_expr, false) 

// An AR is an expression replacement without the guard.
let gen_AR(expr, replacement_expr) =      ARGM(expr, X_true, replacement_expr, false)

// An ARL is a list of array subscript mappings for the given array.
let gen_ARL(the_array, aRlst) =           ARL(the_array, aRlst, false)




let add_pairmap_pairs (mm:meo_rw_t) lst =
    if g_pairs_in_argm then
        // No-longer a special routine: can just use gen_AR now g_pairs_in_argm holds
        let new_xlat = List.fold (fun (cc:mvec_t) (ov, nv) -> cc.Add (x2nn ov, [gen_AR(ov, nv)])) mm.xlat lst
        { fresh mm with xlat=new_xlat }


    else // old way
        let nv = List.fold (fun (c:pairmap_t) a -> c.Add a) mm.pairmap_old lst
        { fresh mm with pairmap_old=nv }

(*
 * The difference between makemap and makemap1 is ... the former accepts pairs and the other arl thingies...
 * makemap does not ignore volatile variables.
 *) 
let makemap lst =
    let vd = -1 // off
#if XI_ASSOC_TRACE_ENABLE
    dev_println (sprintf "makemap %i items" (length lst))
#endif
    let makemap_add m (ll, rr) = 
        let ar = gen_AR(ll, rr)
        if vd>=4 then vprintln 4 ("Adding mapping (2) for " + xToStr ll + " to " + xToStr rr)
        let lv = AR_idx ar
        Ftree_update_arl(m, lv, ar)
    { xlat=List.fold makemap_add (mvec()) lst
      waypoint=     ref None
      pairmap_old=      Map.empty
      functionmap=  Map.empty  
      b_translater= bmvec()
      dp=           dpvec<hexp_t>()
      bdp=          dpvec<xidx_t>(); agents=[]; fagents=[]; bagents=[]; ctrl=XRC_rmode
      remove_undef_assigns= false
    } 



// 
let xToStr_array_ats xx =
    match xx with
        | W_asubsc(the_array, vs, _) -> sprintf "{%s}[%s]"  (netToStr the_array) (xToStr vs)
        | other                      -> sprintf "NoArrayAts(%s)" (xToStr other)


// This code served as a kludge to give preference when repack.fs was giving ambiguous rewrite rules.  That should have stopped now.
//           
let array_subscript_ats xx =
    let ans = 
        match xx with
            | W_asubsc(X_bnet ff, vs, _) -> (lookup_net2 ff.n).ats
            | other                      ->
                //vprintln 3 "NoArrayAts(%s)" (xToStr other)
                []
    //dev_println (sprintf "array_subscript_ats %s are %A" (xToStr xx) ans)
    ans





let rec ar_subtract(A, B) =
    match (A, B) with
    | (a, []) -> a
    | ([], _) -> []
    | (ARGM(L, g, R, _)::t, s) -> if memberp L s then t else (hd A):: ar_subtract(t, s)

(*
 * Form a union or compose a pair of maps, leaving out any members in delete_list.
 *)
let rewrite_compose delete_list (a1:meo_rw_t) (a2:meo_rw_t) = 
    let R  = ref (mvec())
    let Rb = ref (bmvec())  

    let m_count = ref 0
    let mu (M:mvec_t ref) (k, v) =
        let _ = mutinc m_count 1
        M:= (!M).Add(k, ar_subtract(v, delete_list))

    let _ = for z in a1.xlat  do mu R (z.Key, z.Value) done
    let _ = for z in a2.xlat  do mu R (z.Key, z.Value) done

    let mu1 (M:bmvec_t ref) (k, v) = M := (!M).Add(k, v)
    let _ = ignore(for z in a1.b_translater  do mu1 Rb (z.Key, z.Value) done)
    let _ = ignore(for z in a2.b_translater  do mu1 Rb (z.Key, z.Value) done)

#if XI_ASSOC_TRACE_ENABLE
    dev_println (sprintf "makemap: composed %i items" !m_count)
#endif

    let npm =  Map.fold (fun (c:Map<_, _>) k v -> c.Add(k, v)) a1.pairmap_old a2.pairmap_old
    //for KeyValue(k,v) in npm do vprintln 0 ("  composed kv " + xToStr k + " --> " + xToStr v) done
    let rwc = if a1.ctrl=a2.ctrl then a1.ctrl else sf "rwc mismatch"
    {    xlat=         !R
         b_translater= !Rb
         waypoint=     ref(!a1.waypoint)
         functionmap=  Map.fold (fun c k v -> c.Add(k, v)) a1.functionmap a2.functionmap
         pairmap_old=  npm
         dp=           dpvec<hexp_t>()
         bdp=          dpvec<xidx_t>()
         agents=       a1.agents @ a2.agents
         fagents=      a1.fagents @ a2.fagents
         bagents=      a1.bagents @ a2.bagents
         ctrl=         rwc
         remove_undef_assigns= false
    }

let rec rewrite_Compose = function
    | [] -> makemap1 []// Nothing to compose - return an empty mapping.
    | [item] -> item
    | a::b::cc -> rewrite_Compose ((rewrite_compose [] a b)::cc)

let xi_enforce_rmode (m:meo_rw_t)   = { m with ctrl=XRC_rmode; }
let xi_enforce_lmode additional_g (m:meo_rw_t) = { m with ctrl=XRC_lmode additional_g; }


let xi_bor_l lst = xint_write_bnode(V_bor, lst, false);
let xi_band_l lst = xint_write_bnode(V_band, lst, false);



(*
 * Return a list of clauses (conjuncts) from the top-level of an expression.
 * When using covers this is approximately a nop for a single term, but we do not go as far as converting multiple terms to CNF here.
 *)
let rec xi_clauses cc arg  =
    let rec xic cc = function
      | W_bnode(V_band, lst, false, _) -> List.fold xic cc lst
      | W_bnode(V_bor, lst, true, _)   -> List.fold xic cc (map xi_not lst)

      //  gg(false, tt) has two clauses : gg and tt 
      | W_bmux(gg, X_false, tt, _) -> xic (ix_badd cc gg) tt 

      //  gg(ff, false) has two clauses : ~gg and ff
      | W_bmux(gg, ff, X_false, _) -> xic (ix_badd cc (xi_not gg)) ff


      | X_true ->  cc
      | X_false -> Xist [] // could flag
      
      // Are there other bmux clause forms ?  Negated ya ?
      | x ->  ix_badd cc x

    let ans = xic cc arg
    //vprintln 0 ("xi_clauses in " + xbToStr arg + " count=" + i2s(xis_length ans) + " - " + i2s(xis_length cc))
    ans


// Negative normal form: fold in the inversions using de Morgan. Causes no heap growth. (unless also expand out xors: ?)
let xi_nnf n =
    let rec nf1 inv =
        function
            | W_bnode(V_bor, lst, i, _)  -> if i=inv then xi_bor_l(map (nf1 false) lst) else xi_band_l(map (nf1 true) lst)
            | W_bnode(V_band, lst, i, _) -> if i=inv then xi_band_l(map (nf1 false) lst) else xi_bor_l(map (nf1 true) lst)
            | other -> xbinv inv other

    in nf1 false n
;
         

//
// Form the n-dimensional cartesian product of a number of lists.
//
let rec list_xprod =
    function
        | [] -> sf "[] xprod"
        | [item] -> item
        | (h1::h2::t) -> 
          let k2 x y = x@y
          let k1 (x, c) = (map (k2 x) h2) @ c
          let p = foldl k1 [] h1
          list_xprod(p::t)



(* SoP form: can cause exponential growth! Returns list of lists:
 *  [] denotes false: the empty disjunction
 *  [[]] denotes true: the term containing the empty conjunction.
 *)
let xi_sop e = 
    let e = xi_nnf e

    let rec pp = function
        | W_bnode(V_bor, lst, false, _) -> 
            let sl = (map pp lst) : hbexp_t list list list
            list_flatten sl

        | W_bnode(V_band, lst, false, _) -> 
            let sl = map pp lst
            list_xprod sl
            
        | other -> [[other]]

    let ans = pp e
    ans


// Translation lookup function for the assoc_exp and assoc_bexp rewriting walkers.
let tlookup (mM:Map<int, AR_mapping_t list>) arg =
    let tl1 = function
        //| None -> []
        | h -> valOf_or_nil(mM.TryFind h)

    let ans =
        match arg with
            | W_asubsc(v, _, _) -> // tl1(x2nn arg) @ tl1(x2nn v)  
                let a1 = tl1(x2nn arg)
                let a2 = tl1(x2nn v)  
                //vprintln 0 ("tlookup asubsc " + xToStr arg + " " + i2s(length a1) + " " + i2s(length a2))
                a1@a2
            | arg -> tl1(x2nn arg)
    ans
    


let dplookup<'a> (mM:Dictionary<int, 'a>) h =
   let (found, ov) = mM.TryGetValue h
   if found then Some ov else None



(*
 * Substitute sub-expressions from env in an expression.
 * xCNF nets are replaced with skolem versions.
 * g_undef is replaced with a skolem when using the nice one.
 *
 * For array subscriptions, a name alias can exist: if we have a rewrite for A[4] and
 * we find an instance of A[x] then we cannot decide whether to should replace it.
 * This is handled correctly using conditional subscript expressions (ix_query).
 *
 * The rewrite mapping for an array is either simple a similar array with a different name, or
 * a complete or partial mapping.  For both decideable and undecidable and complete and partial maps, the
 * same procedure applies.
 *)
let rec xint_assoc_bexp_w (skolem:skolem_t) e bnops arg k =
    let vd = -1 // off
    match arg with
      | X_true     -> 1
      | X_false    -> -1
      | X_dontcare -> 0
      
      | W_bdiop(oo, lst, inv, meo) -> // NB: orred is a bdiop with one arg.
        let rec ll = function
            | [] -> k
            | ARGB(W_bdiop(oo', lst', _, _), g, w1, volf)::tt when oo=oo' && lst=lst' ->
                let w2 = w1 // xi_bquery(g, w1, arg)
                cassert(g=X_true, "g=X_true in ARGB bdiop")
                if vd>=5 then vprintln 5 ( " " + xbToStr arg + " (bdiop) is mapped to " + xbToStr w2)
                let a = skolem_bzoing(xbinv inv w2, skolem)
                blit_meon a
            | _::tt -> ll tt

        let nv = ll bnops
        if nv<>k then nv
        else
           //if vd>=4 then reportx 4 "assoc_bexp: Args pre assoc_bexp diop 1/2" xToStr lst  
           let lst' = map (xint_assoc_exp_i skolem e meo.n) lst
#if XI_ASSOC_TRACE_ENABLE
           let m_lst = ref []
           for z in e.xlat do mutadd m_lst z.Key done
           dev_println (sprintf "b_diop site: keys are " + sfold i2s !m_lst)

           dev_println ("W_bdiop bassoc " + xbToStr arg + "; args'=" + sfold xToStr lst')
#endif
           if lst = lst' then
#if XI_ASSOC_TRACE_ENABLE
               dev_println ("W_bdiop bassoc arg list unchanged")
#endif
               k
           else 
               let ans = ix_bdiop oo lst' inv
#if XI_ASSOC_TRACE_ENABLE
               reportx 0 "assoc_bexp: Args pre assoc_bexp diop 2/2" xToStr lst
               reportx 0 "assoc_bexp: Args post assoc_bexp diop" xToStr lst'
               dev_println ("assoc bexp " + xbToStr arg + " changed to " + xbToStr ans) 
#endif
               blit_meon ans

      | W_bnode(oo, lst, inv, _) ->
            let rec lil = function
                | [] -> k
                | ARGB(W_bnode(oo', lst', _, _), g, w1, volf)::tt when oo=oo' && lst=lst' ->
                   let w2 = xi_bquery(g, w1, arg)
                   let _ =  cassert(g=X_true, "g=X_true in ARGB bnode")
                   if !g_mvd>=5 then vprintln 5 (" " + xbToStr arg + " (bdiop) is mapped to " + xbToStr w2)
                   let a = skolem_bzoing(xbinv inv w2, skolem)
                   blit_meon a


                | _::tt -> lil tt
            let nv = lil bnops
            if nv<>k then nv
            else
                let lst' = map (xi_assoc_bexp skolem e) lst
#if XI_ASSOC_TRACE_ENABLE
                dev_println ("Node bassoc " + xbToStr arg + "; args'=" + sfold xbToStr lst')
#endif

                if lst = lst' then k
                else
                    let rr = xi_bnode(oo, lst', inv)
#if XI_ASSOC_TRACE_ENABLE
                    dev_println ("Node bassoc ans="  + xbToStr rr)
#endif
                    blit_meon rr


      | W_cover(lst, _) ->
          let sum_changed = ref false
          let k1 n =
              let v = xint_assoc_bexp_i_n skolem e n
              if v = 1 then [[]]
              elif v = -1 then []
              else match deblit v with
                      | W_cover(lst, _) -> lst
                      | other -> [[v]]
          let assoc_cube c5 cube =
              let prod_changed = ref false
              let new_terms = ref false 
              let k2 (n:int) =
                  let n' = k1 n
                  let l1 =  length n'
                  let _ = if l1 <> 1 then new_terms := true
                  let _ = if l1 <> 1 || length (hd n') <> 1 || hd(hd n') <> n then prod_changed := true
                  in n'
              let cube' = map k2 cube // cube' is actually a cover.
              if !prod_changed
              then
                     let _ = sum_changed := true
                     let a1 =
                        if !new_terms
                        then
                            let cube'' = map (fun cover-> List.fold ee_sortc [] cover) cube'
                            let v = List.fold le_and [[]] cube''
                            let _ = vprintln //0 (xbToStr arg + " assoc: New terms " + boolToStr (!new_terms) + " input was " + sfold i2s cube + " output is " + covToStr v + " which is " + xbToStr(xi_cover v))
                            v @  c5 // need to multiply out later (all following cubes get treated this way once flag is set)

                        else
                            let flat c = function
                                | [[]] -> c   // The taut cover can be discarded from this term (that is the behaviour of the above line anyway).
                                | [terms] -> terms @ c // single item
                                | [] -> sf "should be a new_term case" // The empty cover make would make this term false.
                            ee_sortc c5 (List.fold flat [] cube')
                     a1
              else cube :: c5

          let lst' = List.fold assoc_cube [] lst
          if !sum_changed then blit_meon(xi_cover(ee_orl lst')) // could perhaps call espresso at this point?
          else k

      | W_bitsel(l, n, inv, meo) ->
            let rec lom  = function
                | [] -> k
                | ARGB(W_bitsel(l', n', _, _), g, w1, volf)::tt when l=l' && n=n' ->
                   let w2 = xi_bquery(g, w1, arg)
                   let _ = cassert(g=X_true, "g=X_true in ARGB bnode")
                   if vd>=5 then vprintln 5 ( " " + xbToStr arg + " (bitsel) is mapped to " + xbToStr w2)
                   let a = skolem_bzoing(xbinv inv w2, skolem)
                   blit_meon a

                | _::tt -> lom tt
            let nv = lom bnops
            if nv<>k then nv
            else
                let l' = xint_assoc_exp_i skolem e meo.n l
                blit_meon (xi_bitseli inv (l', n))
          
      | W_bsubsc(l, r, inv, meo) ->
            let rec lom  = function
                | [] -> k
                | ARGB(W_bsubsc(l', r', _, _), g, w1, volf)::tt when l=l' && r=r' ->
                   let w2 = xi_bquery(g, w1, arg)
                   let _ = cassert(g=X_true, "g=X_true in ARGB bnode")
                   if vd>=5 then vprintln 5 (" " + xbToStr arg + " (bsubsc) is mapped to " + xbToStr w2)
                   let a = skolem_bzoing(xbinv inv w2, skolem)
                   blit_meon a

                | _::tt -> lom tt
            let nv = lom bnops
            if nv<>k then nv
            else
                let l' = xint_assoc_exp_i skolem e meo.n l
                let r' = xint_assoc_exp_i skolem  (xi_enforce_rmode e) meo.n r
                blit_meon (xi_bsubsc(l', r', inv))

      | W_linp(v, lst, meo) ->
            let v' = xint_assoc_exp_i skolem e meo.n v
            if v' = v then k
            else
                 let r = if false then mya_bunpak_linp(v', lst)  (* Does this really need to get remade ? *)
                         else xi_linp(v', lst)
                 //vprintln 0 ("Result=" + xbToStr r + " assoc of XINT_LINP")
                 blit_meon r

      //| W_bmux(gg, ff, tt, _) -> xint_assoc_bmux skolem e n arg
      | other -> sf("Match error: xint_assoc_bexp_w: " + xbToStr  other)

// Add +ve and -ve fact to a list of known_t sets
and new_known c lst = // could simplify if x is now true or false.
    let add (ct, cf) efo = 
        if efo=Enum_not_an_enum then (ct, cf)
        else (new_known_dp((enum_negate efo)::ct.facts), new_known_dp((efo)::cf.facts)) // Returns false/true ordering, as per bmux.
    List.fold add (c, c) lst

and xint_assoc_exp_w skolem mM nvl_nops arg =
    let vd = -1 // off
    let recursive_apply = xint_assoc_exp_w skolem mM nvl_nops
    match arg with 
          | X_net(id, _) ->
             let rec lim ov = function
                   | [] -> ov
                   | ARGM(X_net(id', _), gg, w1, volf)::tt when id=id' ->
                       let w2 = ix_query gg w1 ov
                       if vd>=5 then vprintln 5 ( " " + xToStr arg + " (net) is mapped to " + xToStr w2)
                       lim w2 tt
                   | ARGM(X_bnet ff, gg, w1, volf)::tt when id=ff.id ->
                       let w2 = ix_query gg w1 ov
                       if vd>=5 then vprintln 5 ( " " + xToStr arg + " (net with bnet) is mapped to " + xToStr w2)
                       lim w2 tt
                   | _::tt -> lim ov tt
             let nv = lim arg nvl_nops
             if nv<>arg then nv
             else  // Now convert X_net to X_bnet if possible. -- There should be a rountine for that
                 let (found, ov) = g_netbase_ss.TryGetValue(id)
                 if found then X_bnet(ov) else arg


          | X_bnet ff -> 
              //let f2 = lookup_net2 ff.n
              let rec lim ov = function
                | [] -> ov
                | ARL(X_bnet ff', w1, volf)::tt when ff.n=ff'.n -> 
                    vprintln 0 ("tmp debug message: ARL on net " + netToStr arg + "\nARL mapping found for a raw net: (presume I am an array): IGNORED HERE: should have be trapped above at a subscription node by tlookup functionality: " + sfold ar_subsc_mappingToStr w1 + "\n" + sfold arToStr nvl_nops)
                    lim ov tt
                    
                | ARGM(X_bnet ff', g, w1, volf)::tt when ff.n=ff'.n ->
                        
                    let nv = xi_query(g, w1, ov)
                    if vd>=5 then vprintln 5 ( " " + xToStr arg + " (bnet) is mapped to " + xToStr nv)
                    lim nv tt

                | _::tt -> lim ov tt
              let nv = lim arg nvl_nops
              //vprintln 0 ("assoc bnet " + ff.id + " with " + i2s(length n) + " hashed candidates") 
              nv

          | W_string(v, _, _) ->
             let rec lim ov = function
                | [] -> ov
                | ARGM(W_string(v', _, _), g, w1, volf)::tt when v=v' ->           
                    let nv = xi_query(g, w1, ov)
                    if vd>=5 then vprintln 5 ( " " + xToStr arg + " (net) is mapped to " + xToStr nv);
                    lim nv tt
                | _::tt -> lim ov tt
             let nv = lim arg nvl_nops
             nv

          | X_undef 
          | X_num _ 
          | X_bnum _ ->  arg

          | X_blift item ->
              // silently ignore rewrites in n.
              let n = xi_assoc_bexp skolem mM item 
              let rr = xi_blift(n)
              //vprintln 3 (xToStr rr + " assoc of blift XINT_NODE")
              rr
              

          | X_x(l, n, _) -> xi_X n (xint_assoc_exp_i skolem mM 0 l)
          
          | X_pair(l, r, meo) ->
#if XI_ASSOC_TRACE_ENABLE
             dev_println (sprintf "X_pair scan assoc %s in ^%i " (xToStr arg) (length nvl_nops))
#endif
             let rec lam_scan ov = function
                | [] -> ov
                | ARGM(X_pair(l', r', meo'), gg, replacement, volf)::tt when meo'.n = meo.n ->
                    let nv = ix_query gg (skolem_zoing(replacement, skolem)) ov
                    if vd>=5 then vprintln 5 ( " " + xToStr arg + " (X_pair) is mapped to " + xToStr nv)
                    lam_scan nv tt
                //    vprintln 0 ("Pair did not match " + xToStr arg + " " + xToStr l' + " " + xToStr r')                    
                | _::tt -> lam_scan ov tt
             let nv = lam_scan arg nvl_nops
             if nv<>arg then nv else ix_pair (xint_assoc_exp_i skolem mM meo.n l) (xint_assoc_exp_i skolem mM meo.n r)

          | W_node(prec, oo, lst, meo) ->
             let rec lam ov = function
                | [] -> ov
                | ARGM(W_node(prec, oo', lst', _), g, w1, volf)::tt when oo=oo' && lst=lst' ->
                    let nv = xi_query(g, skolem_zoing(w1, skolem), ov)
                    if vd>=5 then vprintln 5 (" " + xToStr arg + " (W_node) is mapped to " + xToStr nv)
                    lam nv tt
                | _::tt -> lam ov tt
             let nv = lam arg nvl_nops
             if nv<>arg then nv
             else
                let lst' = map (xint_assoc_exp_i skolem mM meo.n) lst  // Recurse on children in suffix _w function.
                let rr = ix_op prec oo lst' // Note: prec not changed under assoc - could/should reinfer as an option in the mapping cmd.
                //vprintln 0 (sprintf "Node assoc oo=%A " oo + xToStr arg + "; args'=" + sfold xToStr lst' + "; ans=" + xToStr rr + " assoc of XINT_NODE")
#if XI_ASSOC_TRACE_ENABLE
                dev_println (sprintf "Node assoc oo=%A " oo + xToStr arg + "; args'=" + sfold xToStr lst' + "; ans=" + xToStr rr + " assoc of XINT_NODE")
#endif
                rr

          | W_query(g0, l, r, meo) -> 
             let rec lom ov = function
                | [] -> ov
                | ARGM(W_query(g', l', r', _), g, w1, volf)::tt when g0=g' && l=l' && r=r' ->
                    let nv = xi_query(g, skolem_zoing(w1, skolem), ov)
                    if vd>=5 then vprintln 5 ( " " + xToStr arg + " (query) is mapped to " + xToStr w1 + "\n")
                    lom nv tt

                | _::tt -> lom ov tt
             let nv = lom arg nvl_nops
             if nv<>arg then nv
             else
                // Could add a strictness modifier to mM here ...
#if XI_QUERY_TRACING_ENABLED
                vprintln 3 ("assoc of W_query: g0=" + xbToStr g0)
                vprintln 3 ("assoc of W_query: l=" + xToStr l)
                vprintln 3 ("assoc of W_query: r=" + xToStr r)
#endif
                let g9 = xi_assoc_bexp  skolem mM g0
                let t9 = xint_assoc_exp_i skolem mM meo.n l
                let f9 = xint_assoc_exp_i skolem mM meo.n r
                let rr = xi_query(g9, t9, f9)
#if XI_QUERY_TRACING_ENABLED
                vprintln 3 ("assoc of W_query: g9=" + xbToStr g9)
                vprintln 3 ("assoc of W_query: t9=" + xToStr t9)
                vprintln 3 ("assoc of W_query: f9=" + xToStr f9)
                vprintln 3 ("assoc of W_query: " + xToStr arg + " ===> " + xToStr rr)
#endif
                rr

          | W_asubsc(the_array, subs0, meo) -> // r-mode
                let mvd' = !g_mvd
                let subs_ud = undecorate subs0 // Remove pair tag for numeric match on both sides of x_eq check (ARGM should be created that way or else x_eq should have a mode to undecorate as it goes).
#if XI_ASSOC_TRACE_ENABLE
                dev_println (sprintf "xint_assoc asubsc r-mode: for %s start  nvl_nops^=%i " (xToStr arg) (length nvl_nops))
#endif                      

                //vprintln mvd' ("Digging for mapping for " + i2s(x2nn the_array) + " " + xToStr the_array)
                let nvl_ans =
                    let rec los_scan done_some_flag = function
                        | [] -> if done_some_flag then X_undef else arg // If we've mapped at least something, remaining cases are don't care... perhaps parameterise ARG with a flag to control this?
                        | ARGM(W_asubsc(arg', subs0', _), gg, w1, volf)::tt when the_array=arg' && x_eq_undecorate(* _vd "L7605" *) subs_ud subs0' ->
                            let vv = los_scan true tt
                            let nv = ix_query gg (skolem_zoing(w1, skolem)) vv
                            if mvd'>=5 then vprintln 5 (sprintf " (nn=%i,%i) %s  whole asubsc is mapped to %s" meo.n (x2nn the_array) (xToStr arg) (xToStr w1))
                            nv
                        | other::tt ->
#if XI_ASSOC_TRACE_ENABLE
                            dev_println (sprintf "xint_assoc asubsc r-mode: los_scan: Check ARGM element - no match on: " + arToStr other)
#endif                      
                            los_scan done_some_flag tt
                    los_scan false nvl_nops
                let arff_ans =
                    let subs2 = xint_assoc_exp_i skolem (xi_enforce_rmode mM) meo.n subs0
                    let mapping = tlookup mM.xlat the_array
                    let backstop0() = if x_eq subs0 subs2 then arg else safe_xi_asubsc "backstop0" (the_array, subs2) 
                    let backstop1() = safe_xi_asubsc "backstop1" (xint_assoc_exp_i skolem mM meo.n the_array, subs2) 

                    let rec muxtree2 a =
                        if mvd'>=5 then vprintln 5 "muxtree2 in use"
                        match a with
                            | [] -> backstop0() // perhaps return undef if some g have matched here.
                            | ARG1(tag, g, vale)::tt ->
                                let gf = ix_and g (xi_deqdf true (subs2, tag))
                                let _ = vprintln //0 ("Check the_array muxtree gf=" + xbToStr gf + " v=" + xToStr vale)
                                if gf=X_false then muxtree2 tt else ix_query gf vale (muxtree2 tt)
                            | ARFF(tag, ar)::tt -> 
                                if x_eq(* _vd "muxtree2" 0 *) subs_ud tag || x_eq subs0 tag then apply_arff recursive_apply ar subs2 else muxtree2 tt

                    let rec de_ar = function
                        | [] -> backstop1()
                        | ARL(lhs, arops, volf)::_ when lhs=the_array ->
#if XI_ASSOC_TRACE_ENABLE
                            dev_println ("xint_assoc asubsc r-mode: Check ARL matched: using muxtree2")
#endif
                            muxtree2 arops  

                        | ARGM(lhs, g, r, volf)::tt when lhs=the_array ->
                            let _ = hpr_yikes (sprintf "xint_assoc asubsc r-mode: ARGM ignored for %s -> %s L7620" (xToStr lhs) (xToStr r))
                            de_ar tt
                        // | ARGB(lhs, g, r, volf)::tt when lhs=the_array -> sf "L7622 boolean"

                        | other::tt -> 
                            dev_println (sprintf "xint_assoc asubsc r-mode: Check ARL element - no lhs match: " + arToStr other) // Should this happen ? Surely they are collated under lhs net to start with.
                            de_ar tt
#if XI_ASSOC_TRACE_ENABLE
                    dev_println (sprintf "xint_assoc asubsc r-mode: for %s mappings ^=%i  nvl_nops^=%i " (xToStr arg) (length mapping) (length nvl_nops))
#endif                      
                    de_ar mapping
                let ans =
                    if nvl_ans = arg then arff_ans 
                    elif arff_ans = arg then nvl_ans
                    else
                        let msg = sprintf "Two possible changes for xint_assoc asubsc r-mode nn=%i nvl_nops=^%i" (x2nn arg) (length nvl_nops)
                        // " %s and %s"  (xToStr arg) (xToStr_array_ats nvl_ans) (xToStr_array_ats arff_ans)
                        vprintln 3 msg
                        let nvl_ats = array_subscript_ats nvl_ans // Give preference to a change away from a wondarray
                        let arff_ats = array_subscript_ats arff_ans
                        if not_nonep(at_assoc g_wondtoken_marker  nvl_ats) then arff_ans
                        elif not_nonep(at_assoc g_wondtoken_marker  arff_ats) then nvl_ans                            
                        else
                            hpr_yikes ("ANYANS R-MODE " + msg) 
                            nvl_ans
#if XI_ASSOC_TRACE_ENABLE
                dev_println (sprintf "xint_assoc asubsc r-mode: finished: nn=%i nvl_nops=^%i  %s changed to %s" (x2nn arg) (length nvl_nops) (xToStr arg) (xToStr ans))
#endif                      
                ans

          | W_apply(fgis0, cf, l, meo) ->  
#if XI_ASSOC_TRACE_ENABLE
            dev_println (sprintf "xint_assoc W_apply: start: nn=%i nvl_nops=^%i  %s" (x2nn arg) (length nvl_nops) (xToStr arg))
#endif                      

            let fgis1 =
                let ov = mM.functionmap.TryFind(fst fgis0)
                if ov=None then fgis0 else (valOf ov, snd fgis0)
            let rec lm_scan ov = function
                | [] -> ov
                | ARGM(W_apply(fgis', cf', l', _), gg, w1, volf)::tt when cf=cf' && fgis1=fgis' && l=l' ->
                    let nv = ix_query gg (skolem_zoing(w1, skolem)) ov
                    if vd>=5 then vprintln 5 ( " " + xToStr arg + " (W_apply) is mapped to " + xToStr w1 + "\n")
                    lm_scan nv tt
                | _::tt -> lm_scan ov tt
            let nv = lm_scan arg nvl_nops
            let ans =
                if nv<>arg then nv
                else
                    let rr = xi_apply_cf cf (fgis1, map (xint_assoc_exp_i skolem mM meo.n) l)
                    rr
#if XI_ASSOC_TRACE_ENABLE
            dev_println (sprintf "xint_assoc W_apply: finished: nn=%i nvl_nops=^%i  %s changed to %s" (x2nn arg) (length nvl_nops) (xToStr arg) (xToStr ans))
#endif                      
            ans

          | other -> sf("Match error: other form: xint_assoc_exp_w: " + xToStr  other)

// Naming convention: the i routine calls the j routine that calls the w that calls the i again...
//  w matches against forms
//  i looks at agents
//  j implements dp and does the actual mapping
          
and xint_assoc_bexp_i_arg_n skolem (MM:meo_rw_t) x k =
#if XI_ASSOC_TRACE_ENABLE
    dev_println ("assoc_bexp_i " +  xbToStr x)
#endif
    let rec syndicate = function
        | [] -> xint_assoc_bexp_j_arg_n skolem MM x k

        | Agent_bexp(pred, mapper, (premunge, postmunge))::tt ->
            // Four possible behaviours accoding to pre and post munge options.  If both flags are false it is a silly NOP.
            if pred x then  // TODO pred needs to be applied post mapper for post munge as per assoc_exp!
                let x' = if premunge then xint_assoc_bexp_j_n skolem MM k else k
                let ans = mapper skolem.strict (deblit x')
                let ans' = if postmunge then xint_assoc_bexp_i_arg skolem MM ans else blit_meon ans // transitive... but not handy alt function.
                ans'
            else syndicate tt
        //| _::tt -> syndicate tt
    syndicate MM.bagents

// Don't need to look up arg unless there is an Agent_bexp in use...
and xint_assoc_bexp_i_n skolem (MM:meo_rw_t) k =    
    match MM.bagents with
        | [] ->
            let arg = deblit k
            let n = xint_assoc_bexp_i_arg_n skolem MM arg k
            (n:int)
        | agents -> xint_assoc_bexp_j_n skolem MM k
        

and xint_assoc_bexp_i_arg skolem MM arg =    
    match arg with
        | X_true     -> 1 // We (currently) do not allow rewrites from true, false or dontcare but for no good reason.
        | X_false    -> -1
        | X_dontcare -> 0
        | arg ->
             let k = xb2nn arg
#if XI_ASSOC_TRACE_ENABLE
             dev_println ("assoc_bexp_jarg " +  i2s k + " " + xbToStr arg)
#endif
             xint_assoc_bexp_i_arg_n skolem MM arg k
          
and xint_assoc_bexp_j_arg_n skolem MM arg k =    

#if XI_ASSOC_TRACE_ENABLE
    dev_println ("assoc_bexp_jargn " +  i2s k + " " + xbToStr arg)
#endif
    match arg with
        | X_true -> 1
        | X_false -> -1
        | X_dontcare -> 0
        | arg -> xint_assoc_bexp_j_n skolem MM k

and xint_assoc_bexp_j_n skolem mM k =    
    let (found, ov) = mM.bdp.TryGetValue k
    if found then
        let _ = vprintln //4  ("assoc_bexp_j yes dp res " + i2s(valOf dpv))
        ov
    else
#if XI_ASSOC_TRACE_ENABLE
           dev_println ("assoc_bexp_j no dp res " + i2s k)
#endif
           match mM.b_translater.TryFind (abs k) with
              | Some nvl -> (if k<0 then -nvl else nvl)
              | None ->
                    let ans =
                        let bnops = [] // muddy "need b nops"
                        let arg = deblit k
                        let zz = xint_assoc_bexp_w (skolem:skolem_t) mM bnops arg k
                        zz
                    let _ = mM.bdp.Add(k, ans)
#if XI_ASSOC_TRACE_ENABLE_PRINT
                    dev_println ("assoc_bexp_j " + i2s k + " returns " +  i2s ans)
#endif
                    // TODO we should store the xlat in b_translater otherwise it will always be empty...
                    ans


//   
and xint_assoc_exp_i skolem mM context_n__ xx =
#if XI_ASSOC_TRACE_ENABLE
    dev_println ("assoc_exp_i " + xToStr xx)
#endif
    let rec syndicate preflag xx = function
        | [] -> if preflag then xint_assoc_exp_j skolem mM xx else xx

        | Agent_exp(pred, mapper, (premunge, postmunge))::tt ->
            //vprintln 0 ("xint_assoc_exp i calling pred  " + xToStr xx)  
            if premunge && preflag && pred xx then
                let xx = mapper skolem.strict xx
                syndicate preflag xx tt    
            elif not preflag && postmunge && pred xx then // pred was not applied to the postmunge form previously, but this is more sensible now.
                let x1 = mapper skolem.strict xx
                syndicate preflag x1 (if x1 <> xx then mM.agents else tt) // Restart agent list if there has been a change.
            else syndicate preflag xx tt
                //vprintln 0 ("xint_assoc_exp i returned from mapper  " + xToStr xx)
            // else syndicate tt // Only one posmunge agent was applied in the past, so agent ordering mattered even more.

    //vprintln 0 ("Syndicating " + xToStr x + " with " + i2s(length agents) + " agents")
    let xx = syndicate false xx mM.agents // TODO copy to bexp for consistency.
    let xx = syndicate true xx mM.agents            
    xx
    
and xint_assoc_exp_j skolem mM arg =
    match arg with
        | X_num _
        | X_bnum _ -> arg
        | X_pair(l, r, _) when not g_pairs_in_argm ->
            //vprintln 0 ("Search for " + xToStr arg + " in " + Map.fold (fun c k v -> c + " " + xToStr k + "->" + xToStr v) "" mM.pairmap)
            match mM.pairmap_old.TryFind arg with
                | Some nv -> nv
                | None -> xint_assoc_exp_w skolem mM [] arg
        | arg ->
            let kn = x2nn arg
                //    let ans = xint_assoc_exp_w skolem mM (tlookup mM.xlat arg) arg
#if XI_ASSOC_TRACE_ENABLE
            dev_println (sprintf "assoc_exp_j lookup kn=%i arg=%s" kn (xToStr arg))
#endif
            let (found, ov) = mM.dp.TryGetValue kn
            let ans =
                if found then ov
                else 
                    let ans = xint_assoc_exp_w skolem mM (tlookup mM.xlat arg) arg
                    mM.dp.Add(kn, ans)
                    ans
#if XI_ASSOC_TRACE_ENABLE
            dev_println ("xint_assoc " + xToStr arg + " returns " +  xToStr ans)
#endif
            ans

and xi_assoc_exp sk rw e = xint_assoc_exp_i sk rw 0 e


// l-mode rewrite routines: returns a list of (guard, value) pairs, not just a single expression. 
// Using an ARG, a subscripted assignment can have multiple mappings each with a guard condition.  Hence a write
// to an array is potentially a demultiplex to a list of conditional scalar assigments.
// This is the converse to the muxtrees made in rmode.
and xint_assoc_lmode_w g4 mM nvl_nops arg =
    let sk = g_sk_null
    match arg with
        | X_x(xx, power, _) ->
            let gec (g, v) = (g, gec_X_x(v, power))
            map gec (xint_assoc_lmode_w g4 mM nvl_nops xx)
            
        | X_bnet ff ->
              let rec lm_scan ov = function
                | [] -> ov
                | ARGM(X_bnet ff', g1, w1, volf)::tt when ff.n=ff'.n ->
                    let _ = cassert(g1=X_true, "ARG guard in l-mode bnet")
#if XI_ASSOC_TRACE_ENABLE
                    dev_println (" " + xToStr arg + " (bnet l-mode) is mapped to " + xToStr w1)
#endif
                    lm_scan w1 tt
                | (_::tt) -> lm_scan ov tt
              let nv = lm_scan arg nvl_nops
              [ (g4, nv) ] 

        | X_blift(W_bitsel(xx, bitno, inv, _)) ->
            let gec_bitsel (g, v) = (g, xi_blift(xi_bitseli inv (v, bitno)))
            map gec_bitsel (xint_assoc_lmode_w g4 mM nvl_nops xx)


        | X_blift(W_bsubsc(l, r, inv, meo)) ->
            let ov = W_bsubsc(l, r, inv, meo)
            let rec lm_scan ov = function
                | [] -> ov
                | ARGB(W_bsubsc(l', r', _, _), g1, w1, volf)::tt when l=l' && r=r' ->
                       let _ = cassert(g1=X_true, "ARG guard in l-mode bnet")
#if XI_ASSOC_TRACE_ENABLE
                       let _= dev_println (" " + xToStr arg + " (bsubsc l-mode) is mapped to " + xbToStr w1)
#endif
                       xbinv inv w1
                | _::tt -> lm_scan ov tt
            let nv = lm_scan ov nvl_nops
            if nv<>ov then [ (g4, xi_blift(nv)) ]
            else
                let r' = xint_assoc_exp_i g_sk_null (xi_enforce_rmode mM) meo.n r
                let l' = xint_assoc_lmode_i mM l
                let kk (g, lv) = (g, xi_blift(xi_bsubsc(lv, r', inv)))
                map kk l'

        | W_asubsc(the_array, subs0, meo) -> // l-mode
            let nvl_ans = 
                let rec ll_scan ov = function
                    | [] -> ov
                    | ARGM(W_asubsc(arg', subs0', _), g1, w1, volf)::tt  when x_eq(*_vd*) the_array arg' && x_eq_undecorate(*_vd*) subs0 subs0'->
#if XI_ASSOC_TRACE_ENABLE
                       dev_println (sprintf "xint_assoc_lmode: n=%i  %s: whole asubsc is mapped to %s"  meo.n (xToStr arg) (xToStr w1))
                       dev_println (sprintf "xint_assoc_lmode: ARGM: n=%i  %s: whole asubsc is mapped to %s"  meo.n (xToStr arg) (xToStr w1))
#endif
                       let ng = (fst g4, ix_and g1 (snd g4))
                       ll_scan ((ng, w1)::ov) tt
                    | other::tt ->
                        //dev_println ("  advance nop scan: non ARGM is " + arToStr other)
                        ll_scan ov tt
#if XI_ASSOC_TRACE_ENABLE
                dev_println (sprintf "Start nvl_nop scan for nn=%i len ^%i =  " (x2nn the_array) (length nvl_nops) + (sfold arToStr nvl_nops))
#endif
                let nv = ll_scan [] nvl_nops
                nv
            let arff_ans =
                    let mapping = tlookup mM.xlat the_array // We look up the mapping of the array whereas nvl_nops is for a whole operation.
                    let rmode_apply ee = xint_assoc_exp_i g_sk_null (xi_enforce_rmode mM) meo.n ee
                    let subs_ud = undecorate subs0
                    let subs2 = rmode_apply subs0
#if XI_ASSOC_TRACE_ENABLE
                    dev_println (sprintf "xint_assoc asubsc l-mode arff-style %s -...new subs=%s, mapping=^%i" (xToStr arg) (xToStr subs2) (length mapping))
#endif
                    let rec storetree2 = function
                          | []  -> []
                          | ARFF(tag, ar)::tt -> 
                              let tailer = storetree2 tt
                              if x_eq_undecorate (*_vd "storetree2a" *) subs_ud tag || x_eq_undecorate(*_vd "storetree2b" *) subs0 tag then
#if XI_ASSOC_TRACE_ENABLE
                                  dev_println (sprintf "xint_assoc_l-mode: arf-style n=%i  %s: ARFF match"  meo.n (xToStr arg))
#endif                                  
                                  (g4, apply_arff rmode_apply ar subs2)::tailer
                              else tailer

                          | ARG1(tag, g1, vale)::tt -> 
                              let g2 = (fst g4, ix_and g1 (ix_and (snd g4) (xi_deqdf true (tag, subs2))))
                              let tailer = storetree2 tt
                              if xbmonkey(snd g2)=Some false then tailer
                              else
#if XI_ASSOC_TRACE_ENABLE
                                  dev_println (sprintf "xint_assoc_lmode: n=%i  %s: ARG1 operate"  meo.n (xToStr arg))
#endif                                  
                                  (g2, vale)::tailer 

                          //| other::tt -> sf ("storetree2 other " + ar_subsc_mappingToStr other)
                    //let _ = vprintln 3 ("Mapping for " + xToStr arg + " was " + (if mapping=None then "None" else "Some(...)"))
                    let backstop() = (g4, safe_xi_asubsc "backstop" (xint_assoc_exp_i sk mM meo.n the_array, subs2))
                    let checknull a = if nullp a then [backstop()] else a
                    let rec de_ar = function
                          | []                                         -> [backstop()]
                          | ARL(lhs, rewrites, volf)::tt when the_array=lhs -> checknull(storetree2 rewrites)
//                        | ARF0(lhs, arlrec)::t -> if the_array=lhs then [ (g, apply_arff arlrec subs2) ] else de_ar t
                          | _::tt -> de_ar tt
                    de_ar(mapping)
            let ans_changed ans = length ans <> 1 || x2nn (snd (hd ans)) <> x2nn arg
            let gxToStr ((_, g), v) = xbToStr g + "::" + xToStr_array_ats v
            let ans =
                if nullp nvl_ans then arff_ans
                elif not(ans_changed arff_ans) then nvl_ans
                else
                    let msg = sprintf "Two possible changes for xint_assoc asubsc l-mode nn=%i nops=^%i  %s -> %s and %s" (x2nn arg) (length nvl_nops) (xToStr arg) (sfold gxToStr nvl_ans) (sfold gxToStr arff_ans)
                    vprintln 3 msg
                    let array_subscript_ats1 (guardinfo_, arg) = array_subscript_ats arg
                    let nvl_ats = list_flatten(map array_subscript_ats1 nvl_ans) // Give preference to a change away from a wondarray
                    let arff_ats = list_flatten(map array_subscript_ats1 arff_ans) 
                    if not_nonep(at_assoc g_wondtoken_marker nvl_ats) then arff_ans
                    elif not_nonep(at_assoc g_wondtoken_marker  arff_ats) then nvl_ans
                    else
                        hpr_yikes ("ANYANS L-MODE " + msg)
                        //[(fst(hd arff_ans), X_undef)] // Return anything for now
                        nvl_ans
                        
#if XI_ASSOC_TRACE_ENABLE
            if ans_changed ans then dev_println (sprintf "xint_assoc asubsc l-mode nn=%i nops=^%i  %s -> %s" (x2nn arg) (length nvl_nops) (xToStr arg) (sfold gxToStr ans))
            dev_println (sprintf "xint_assoc asubsc l-mode %s -> %s" (xToStr arg) (sfold gxToStr ans))
#endif                      

            ans

        | X_undef ->
            if mM.remove_undef_assigns then
                [] // This X_undef form is generally used as a replacement lhs when an assignment needs to be 'commented out' and so we can delete it here, or return a false guard, or leave it unchanged - all should ultimately result in the same semantic.
            else [(g4, X_undef)]
        | other -> sf("other form xint_assoc_lmode_w: " + xkey other + " " + xToStr other)

and  // Lmode rewrite returns a (guard, vale) list - not just a single expression. See comment above. 
   xint_assoc_lmode_i mM arg =
        let vd = 300
        let get_guard =
            function
                | XRC_lmode g -> g
                | _ -> sf "assoc_lmode : no guard"
        let additional_g = get_guard mM.ctrl  
        let nvl_nops = tlookup mM.xlat arg
        let ans = xint_assoc_lmode_w additional_g mM nvl_nops arg
        // Do we do any dp for lmode? no. lmode_dp.Add(av, ans)
#if XI_ASSOC_TRACE_ENABLE
        dev_println ("assoc_lmode " + xToStr arg + " returns " +  sfold (fun ((a, g), v) -> "(( ))->" + xToStr v) ans)
#endif
        ans

and xi_assoc_exp_lmode g sk mM e = 
      let mM' = xi_enforce_lmode g mM
      xint_assoc_lmode_i mM' (e)

and xi_assoc_bexp sk mM e =
    //let n = blit_meon e
    let n' = xint_assoc_bexp_i_arg sk mM e
    deblit n'
    

and xi_assoc_bev sk (pp:stutter_t, g:hbexp_t) mM b =
    //vprintln 0 (sprintf "RECURSE assoc_bev " + hbevToStr b)
    let rec syndicate = function
        | [] -> xi_assoc_bev_j sk (pp, g) mM b

        | Agent_bev(pred, premap)::tt ->
            if pred b then
                let (r, postmap) = premap (pp, g) b
                let r = xi_assoc_bev_j sk (pp, g)  mM r
                let r = postmap (pp, g) r
                r
            else syndicate tt
        | (_::tt) -> syndicate tt
    syndicate mM.fagents

and xi_assoc_bev_j sk g rw arg =
    let rcall b = xi_assoc_bev sk g rw b
    // This is a cv1-style routine and DOES NOT CAPTURE ASSIGNS TO 'g' in an rcall body!!

    // Pathological example (cv1):
    // if (p && q) { p := false; r := r+1; } is converted to
    // begin if (p && q) { p := false; } 
    //       if (p && q) { r := r+1; } 
    //       end
    // If the assignment to p is blocking in the output formalism, then r fails to be incremented.
    // 

    // Slight bug. TODO.
    let rcallg g1 b = xi_assoc_bev sk (fst g, ix_and (snd g) g1) rw b
    let tce = xi_assoc_exp sk rw 
    let tcbe = xi_assoc_bexp sk rw 
    match arg with
        | Xswitch(e, lst) ->
            let e' = tce e
            let sa (a, cmd) =
                let a' = map tce a
                let g' = List.fold (fun c v -> ix_or c (xi_deqd(e', v))) X_false a // cf. other clauses - applied pre map ... no real reason
                (a, rcallg  g'  cmd)
            Xswitch(e', map sa lst)
        | Xif(a, b, c)  ->    Xif(tcbe a, rcallg (a) b, rcallg (xi_not a) c)
        | Xwhile(gd, b) -> Xwhile(tcbe gd, rcallg gd b)
        | Xassign(l, r) ->
            let l' = xi_assoc_exp_lmode g sk rw l 
            let r' = tce r
            let ja ((g, l), c) =
                let gdown = function
                   | (None, gc) -> gc
                   | (Some(pc, pp), gc) -> ix_and (xi_deqdf true (pc, pp)) gc
                let g9 = gdown g
                if g9=X_false then c else gec_Xif1 g9 (Xassign(l, r')) :: c
            gec_Xblock(foldl ja [] l')

        | Xannotate(v, a)     -> Xannotate(v, rcall a)
        | Xwaituntil(a)       -> Xwaituntil(xi_assoc_bexp sk rw a)
        | Xwaitrel(a)         -> Xwaitrel(xi_assoc_exp sk rw a)
        | Xwaitabs(a)         -> Xwaitabs(xi_assoc_exp sk rw a)
        | Xblock lst          -> Xblock(map rcall lst)
        | Xlinepoint(lp, s)   -> Xlinepoint(lp,  rcall s)

        | (Xgoto _)
        | (Xdominate _)
        | (Xfork _)
        | (Xcomment _)
        | (Xlabel _)
        | (Xskip)
        | (Xbreak)
        | (Xjoin _)
        | (Xcontinue)-> arg
        
        | Xreturn f         -> Xreturn(xint_assoc_exp_i sk rw 0 f)
        //| Xcall(fgis, args) -> Xcall(fgis, map (xi_assoc_exp sk rw) args)
        | Xbeq(g, w, n)     -> Xbeq(xi_assoc_bexp sk rw g, w, n)
        | Xeasc s           -> Xeasc(xint_assoc_exp_i sk rw 0 s)
        | other -> sf("match: xi_assoc_bev:" + hbevToStr other)

let register16(id, length) =
    let width = 16
    let signed = Unsigned
    let io = LOCAL
    let (n, ov) = netsetup_start id
    if not(nonep ov) then 
        let ff = valOf ov
        let f2 = lookup_net2 ff.n
        assertf(length = f2.length) (fun () -> id + ": netgen16 new len " + sfold i2s64 length + " cf " + sfold i2s64 f2.length)
        cassert(signed = ff.signed, "netgen16 new signed")
        assertf(width = ff.width) (fun () -> id + ": netgen16 new width " + i2s(ff.width))
        cassert(io = f2.xnet_io, "netgen16 new io")
        ff
    else 
        let pol = ref false
        let dir = ref false
        let h = ref 65535I
        let l = ref 0I
        let vt = ref 
        let states = ref None
        let e2 = 0
        let ff = { 
                    n= n
                    id= id
                    rh= !h
                    rl= !l
                    width=width
                    is_array= not_nullp length
                    constval=[]
                    signed=signed
                }
        let f2 = { 
                    vtype=     V_REGISTER
                    xnet_io=   io 
                    dir=       !dir
                    pol=       !pol
                    length=    length
                    ats=       []
                }
        netsetup_log (ff, f2)


(*
 * Some sort of clone or mapping function: how is this different from xi_assoc applied to a net ?
 * used in conerefine...
 *)
let rec xi_net_regen mM cc nN =
    match nN with
        | X_bnet ff ->
             let netsof cc = function
                 | ARFF(_, arlrec) -> singly_add arlrec.replacement cc
                 | other           -> sf("netsof " + ar_subsc_mappingToStr other)
             let ren cc = function
                 | ARL(X_bnet ff', w1, volf) -> if ff.n=ff'.n then List.fold netsof cc w1 else cc
                 | ARGM(X_bnet ff', X_true, w1, volf)  ->
                     if ff.n=ff'.n then 
                        if !g_mvd>=5 then vprintln 5 (" " + xToStr nN + " (net_regen) is mapped to " + xToStr w1);
                        w1 :: cc   
                     else cc
                 | _ -> cc

             let nv = List.fold ren [] (tlookup mM.xlat nN)
             //vprint 0 ( "assoc bnet " + f.id + " with " + i2s(length n) + " hashed candidates")
             if nv<>[] then nv @ cc else nN::cc

        | W_asubsc(n1, s, _) -> 
            //vprintln 0 ("+++ xi_net_regen array:" + xToStr nN)
            xi_net_regen mM cc n1
          
        | X_net _ -> cc           (* is this correct?? *)
        | W_string(_, _, _) -> cc (* is this correct?? *)
        
        | other -> sf ("xi_net_regen other:" + xkey other + "  " + netToStr other)




let xi_rewrite_exp rw e  = xi_assoc_exp g_sk_null rw e
let xi_rewrite_bexp rw e = xi_assoc_bexp g_sk_null rw e
let xi_rewrite rw e      = xi_rewrite_exp rw e
let xi_brewrite rw e     = xi_rewrite_bexp rw e
let xi_rewrite_xi_list rw l = xiSort(map (xi_rewrite rw) (xi_valOf l))

// The lmode rewrite of an expression returns a list of (guard,value) pairs.
//
let xi_rewrite_lmode additional_g rw e =
    let rw' = xi_enforce_lmode additional_g rw
    xint_assoc_lmode_i rw' e

let xbev_rewrite ww rw bev = xi_assoc_bev g_sk_null  (None, X_true) rw bev


(* 
 * Compose a mapping on itself. In other words,
 * If we have a mapping where x->Y and Y contains a y such that y->z then alter this so that x->Y'.
 *
 * A related issue is if we have provided with x+y->Z and y->R which is given precedence?  The x+y term will always be encountered first in the tree walk and so it gets precedence.
 *
 * (Alternatively, to compose a pair of different mappings we use rewrite_compose).
 *)
let map_transclose (mM:meo_rw_t) =
    let m_donesome = ref false
    let compose_step mM (nm:mvec_t) (key, ov) =
        let rewrite msg r =
            let r1 = xi_rewrite mM r
            if r <> r1 then
                vprintln 3 (msg + sprintf "map_transclose donesome: %s -> %s" (xToStr r) (xToStr r1))
                m_donesome := true
            r1

        let brewrite r =
            let r1 = xi_brewrite mM r
            if r <> r1 then
                vprintln 3 (sprintf "map_transclose bool donesome: %s -> %s" (xbToStr r) (xbToStr r1))
                m_donesome := true
            r1

        let arlf = function
            | ARG1(lhs, g, rhs) -> 
                let r' = rewrite "ARG1" rhs
                let g' = brewrite g                
                //vprintln 0 (i2s pos + " trclose-1 AR out " + xToStr j)
                ARG1(lhs, g', r')

            | ARFF(ll, arlrec) -> // Arithmetic translates are not idempotent so should not be iterated?
                let nv = rewrite "ARFF" arlrec.replacement
                //vprintln 0 (sprintf "mapper transclose ARFF: %s -> %s" (xToStr arlrec.replacement) (xToStr nv))
                ARFF(ll, { arlrec with replacement=nv })
            // The alrec_t record form was not processed by transclose: instead its rr.replacement is operated on during rewrite.
            // But that does not work for the atomic match clauses, so have applied the closure here too.
            //| _ -> sf "transclose arlf other"

        let rec de_ar = function
            | ARL(lhs, items, volf) -> ARL(lhs, map arlf items, volf)
            | ARGM(lhs, g, rhs, volf) ->
                let r' = rewrite "ARGM" rhs
                //vprintln 0 (sprintf "ARGM rewrite for lhs=%s rhs_was=%s and rhs_now=%s" (xToStr lhs) (xToStr rhs) (xToStr r'))
                let g' = brewrite g
                (* vprint 0 ( i2s pos + " trclose-2 AR out " + xToStr j + "\n") *)
                ARGM(lhs, g', r', volf)
            | _ -> muddy  "de_ar transclose: ARGB missing"
        nm.Add(key, map de_ar ov)


    let bcompose_step mM (nm:bmvec_t) (key, ov) =
        let _ = cassert (key>0, "-ve keys")
        let nv = xint_assoc_bexp_i_n g_sk_null mM ov 
        let nv = if key < 0 then 0-nv else nv 
        nm.Add(key, nv)

   
    let rec onestep count mM  =
        m_donesome := false
        let m'  = Map.fold (fun c k v ->compose_step mM c (k,v)) (mvec()) mM.xlat        
        let bm' = Map.fold (fun c k v ->bcompose_step mM c (k,v)) (bmvec()) mM.b_translater
        let ans =  { fresh mM with xlat=m'; b_translater=bm'; pairmap_old= mM.pairmap_old; // Pairs not transclosed at this point (only used by repack.fs field arrays at the moment)
                   } 
        if count > 20 then
            hpr_yikes (sprintf "Mapper transclose early stop at %i" count)
            ans // Kludge stop early
        elif not !m_donesome then ans
        else
            let _ = vprintln 3 (sprintf "mapper transclose - go around again %i" count)
            onestep (count+1) ans
    onestep 0 mM 

(*
 * For Verilog we cannot share subexpression that contain both array and functions calls, so we
 * we record which we have had so far!
 *)
type sofarhad_t = | SFH_none | SFH_array | SFH_fcall



type housekeeping0_t = // URGENT
    {
        baseidx:              int
        writeshed:            bool
        startarcf:            bool  // An arc from a resume where args are loaded and hence where input scalars may be volatile.
    }
    
//
type housekeeping1_t =
    | HK_prerel of int * string           // Before inserting in relative schedule
    | HK_rel of int * string              // Relative time as stored in pre-reindexed schedule.
    | HK_abs of (string * int) list * housekeeping1_t // After relocating in the time domain plus original. There is more than one timing point, e.g. for a RAM read, where control and data are at different times, hence the list.

type area_inventory_t = Set<xidx_t * si_area_t>

// A simple metric for logic cost, when we are concerned with critical path delay, is the maximum depth of any branch in the tree.  But synchronous memory reads should count as leaf reads.
// Sometime we need to estimate the total area or amount of logic generated, not just its critical depth.    
// And when it comes to readibility of subexpressions, the number of inputs is also a factor, hence we keep both metrics.
// Area costs do not simply sum in general owing to sub-expression sharing. An inventory union is then the only way to properly account for area.
type costvec_entry_t =
    {
        maxa:      int64 // For controlling levels of logic we need to threshold on max logic depth and not the total amount across parallel assigns and polyadic operators.
        total:     int64 // Total - proportional to area.
        inventory: area_inventory_t option
    }

type costvec_t =
    | CV_single of int64
    | CV_stranded of costvec_entry_t

let costToStr = function
    | CV_stranded v ->
        let area_ss =
            if nonep v.inventory then "-not-computed-"
            else
                let summer cc (nn, area) = area+cc
                let ans = Set.fold summer 0L (valOf v.inventory)
                i2s64 ans
        sprintf "[longest_path_delay=%i; naive_area=%i; area=%s]"  v.maxa v.total area_ss
    | CV_single n -> i2s64 n



// Area costs do not sum in general owing to sub-expression sharing, hence the inventory mechanism also exists. 
// 
let inventory_sum lo ro =
    if nonep lo then ro
    elif nonep ro then lo
    else Some(Set.union (valOf lo) (valOf ro))


let costvec_sequential_add (v:costvec_t) (depth_cost, area_cost, inventory) =
    match v with
        | CV_single n    -> CV_stranded {         maxa=n+depth_cost;        total=n+area_cost; inventory=inventory          }
        | CV_stranded v1 -> CV_stranded { v1 with maxa=v1.maxa+depth_cost;  total=v1.total + area_cost; inventory=inventory_sum v1.inventory inventory }

let g_costvec_zero = CV_single 0L // CV_stranded { maxa=0; total= 0 }

//
// The total cost of logic sums linearly, although we need to ensure shared subexpressions are not counted twice.
// The logic depth works on a maximum depth of support basis and is the max over strands.
let costvec_parallel_combine (v:costvec_t) q =
    match (v, q) with
        | (CV_single a, CV_single b) -> CV_single (a+b)
        | (CV_single av, CV_stranded v)
        | (CV_stranded v, CV_single av) ->
            //dev_println ("Combined with CV_single costvec!")
            CV_stranded { v with total= v.total+av; maxa=max v.maxa av }
        | (CV_stranded v, CV_stranded w) -> CV_stranded { v with total= v.total + w.total; maxa=max v.maxa w.maxa;  inventory=inventory_sum v.inventory w.inventory }
    
type walker_nodedata_t = 
 | TIMEOF_NODE of housekeeping0_t * hbexp_t * bool * housekeeping1_t // (aka starttime_t) variable latency guard net and ready schedule/time b
 | EIS_NODE of    bool
 | STIME_NODE of  hbexp_t * int   // Static timing analysis
 | SHARE_NODE of  sofarhad_t     // For shared subexpression finding
 | COST_NODE of   int * sofarhad_t      // Perhaps misnamed - used for finding the nature of a combinational subexpression since Verilog has different rules regarding function calls and array support etc.. See comments elsewhere.
 | ACOST of costvec_t                 // Principle delay and area cost metrics form
 | NEVER_SHARE 


// vl2s : variable latency flag is paired with estimated variable of fixed ready time.
let rec hkToStr kc = function
    | HK_prerel(n, tag) -> sprintf "%s/%s/HK_prerel(%i)" kc tag n
    | HK_rel(n, tag_)   -> sprintf "%sr%i" kc n // use lower-case 'r' for relative times.
    | HK_abs(lst, hk)   ->
        let p2s (id, nn) = sprintf "%s%i" id nn
        sprintf "%s/HK_abs(%s, %s)" kc (sfold p2s lst) (hkToStr kc hk)

// Variable-latency timing - used by restructure.
let vl2s = function
    | (startargf_, g, false, hk) -> hkToStr "R" hk + (if g <> X_true then "/" + xbToStr g else "")
    | (startargf_, g, true,  hk)  -> hkToStr "W" hk + (if g <> X_true then "/" + xbToStr g else "")


let qToStr = function
    | TIMEOF_NODE(startargf, gg, writef, hk) -> vl2s(startargf, gg, writef, hk)
    | COST_NODE(i, sfg)  -> "C" + i2s i
    | ACOST cost -> costToStr cost
    | other -> (sprintf "NONE-TIMEOF %A" other)




type walker_costs_t = OptionStore<xidx_t, walker_nodedata_t>

type walkctl_t = {
  vd:                       int  // verbal diorosity
  vdp:                      bool // Additional verbosity flag
  usecounts:                OptionStore<xidx_t, int>            // How many times a sub-expression occurs
  costs:                    walker_costs_t
  cn0:                      walker_nodedata_t
  cn1:                      walker_nodedata_t
  oncef:                    bool  // Where a sub-expression is shared, visit only one path to it.
  opfun:             int -> hbexp_t option -> hexp_t option -> walker_nodedata_t -> walker_nodedata_t list ->   walker_nodedata_t;
  bsonchange:        walker_costs_t -> walker_nodedata_t -> int -> hbexp_t * walker_nodedata_t ->   walker_nodedata_t
  sonchange:         walker_costs_t -> walker_nodedata_t -> int -> hexp_t * walker_nodedata_t ->   walker_nodedata_t
  bunitfn:           hbexp_t -> unit // R-mode bool exp visitor
  unitfn:            hexp_t -> unit  // R-mode non-bool exp visitor
  lfun:              (stutter_t * hbexp_t) -> directorate_t -> hexp_t -> hexp_t -> unit (* L-mode *)
//walker_waypoint: (hexp_t list * hexp_t list) option ref;
}

let bleafp a = a=X_true || a=X_false || a=X_dontcare

(*
 * Walk an x_tree tallying costs and use counts.
 * Return triple (local cost, bson nodes, xson nodes) for further walking.
 *)
let rec mya_bfwalk (m, ss, tallies, wlk_plifun) clkinfo_o = function
    | W_bitsel(a, N, inv, meo)    -> (ss.cn1, [], [a])
    | W_linp(v, LIN(s, lst), meo) -> (ss.cn1, [], [v])
    | W_bnode(v, lst, inv, meo)   -> (ss.cn1, lst, [])
    | W_cover(lst, meo)           -> (ss.cn1, map deblit (List.fold cube_support [] lst), [])

    | W_bmux(gg, ff, tt, meo)     -> (ss.cn1, [gg; ff; tt], [])
    | W_bdiop(_, lst, inv, meo)   -> (ss.cn1, [], lst)
    | W_bsubsc(l, r, inv, meo)    -> (ss.cn1, [], [l; r])
    | (f) -> if bleafp f then (ss.cn1, [], []) else sf("match mya_bfwalk other form:" + xbToStr f)


// Walk the lhs of an assignment.  Any subscripting expressions need walking in r-mode.
and mya_lwalk aux (pp, g) (clkinfo_o:directorate_t) arg rhs =
    let (m, ss, tallies, pli_log_) = aux
    match arg with
        //|  (X_x(a, N)) -> (ss.cn1, [], [a])
        | X_blift(W_bsubsc(l, subsc, i, m)) ->
            let _ = mya_walk_it aux clkinfo_o subsc
            let _ = ss.lfun (pp, g) clkinfo_o arg rhs
            (COST_NODE(1, SFH_array), [], [subsc])

        | X_blift(W_bitsel(l, n, i, m)) ->
            let _ = ss.lfun (pp, g) clkinfo_o arg rhs
            (COST_NODE(1, SFH_array), [], [])

        | X_undef 
        | X_bnet _   
        | X_net _     
        | X_x(_, _, _) ->
            let _ = ss.lfun (pp, g) clkinfo_o arg rhs
            (ss.cn1, [], [])

        | X_blift(W_bsubsc(l, subsc, _, _))
        | W_asubsc(l, subsc, _) ->
            let _ = mya_walk_it aux clkinfo_o subsc // Recurse on subscript expression in r-mode
            let _ = ss.lfun (pp, g) clkinfo_o arg rhs
            (COST_NODE(1, SFH_array), [], [subsc])
        
and mya_fwalk aux clkinfo_o arg =
    let (m, ss, tallies, wlk_plifun_) = aux
    match arg with
        |           X_x(a, x, _)    -> (ss.cn1, [], [a])
        |  W_apply(fgis, _, lst, _) -> (COST_NODE(100, SFH_fcall), [], lst)  // We need to know comb paths through the function please ...
        |      W_asubsc(l, r, _)    -> (COST_NODE(1, SFH_array), [], [l; r]) 
        |    W_query(g, t, f, _)    -> (ss.cn1, [g], [t; f])
        |    W_node(prec, _, lst, meo) -> (ss.cn1, [], lst)
        |        W_valof(b, meo) -> (walk_bev_lst aux clkinfo_o b, [], [])
        |        X_pair(a, b, m) ->
                                 // sf ("mya_fwalk: found pair " + boolToStr(leafp(X_pair(a,b,m))))    
                                 (ss.cn1, [], [a;b])
        |          X_subsc(l, r) -> (COST_NODE(1, SFH_array), [], [l; r])
        |              X_blift b -> mya_bfwalk aux clkinfo_o b
    
        |  (f) -> if leafp f then (ss.cn1, [], []) else sf("match mya_fwalk other form:" + xkey f + " " + xToStr f)

// Value n=0 used for bev invocation of opfun
and walk_bev_lst (m, (ss:walkctl_t), tallies, wlk_plifun) clkinfo_o lst = (ss.opfun) 0 None None ss.cn0 (map (walk_bev (m, ss, tallies, wlk_plifun) clkinfo_o) lst)

and walk_bev_ntr aux (clkinfo_o:directorate_t) code =
    let (m, ss, tallies, wlk_plifun) = aux
    let costlog cost =
        match tallies with
            | None -> cost
            | Some m_costlist ->
                mutadd m_costlist (m, cost)
                ss.cn0
    let mexpr arg = costlog(mya_walk_it aux clkinfo_o arg)
    match code with
        | Xcomment _
        | Xlabel _
        | Xskip      -> ss.cn0
        | Xgoto _    -> ss.cn1

        | Xblock l 
        | Xpblock l -> walk_bev_lst aux clkinfo_o l

        | Xeasc e -> mexpr e
        (*      |   walk_bev ss (Xeasc(x_apply((f, gis), lst))) -> 
            let _ = map aol lst'
            in ([], code, foldr xi_support [] lst', gis.eis)::c
        *)

        //| Xcall((f, gis), lst) -> (ss.opfun) 0 None None ss.cn1 (map mexpr lst) // TOOD - gis will say whether combinational paths are continued over this costlog. gis is passed up for PLI calls so mirror here. They should not really be so different. Please unify.

        | Xreturn e     -> mexpr e
        | Xbeq(g, _, _) -> costlog(mya_bwalk_it aux clkinfo_o g)
        | Xassign(l, rhs) -> 
             //let lss = xi_lsupport [] l  // Walking the support will not walk each subexpression!
             let lss = xi_lsubexps [] l             
             let rres = mexpr rhs
             let  ans = (ss.opfun) 0 None None rres (map mexpr lss) 
             (ss.lfun) (None, X_true) clkinfo_o l rhs
             ans
             
        | Xwaituntil(g)       -> costlog(mya_bwalk_it aux clkinfo_o g)
        | Xif(g, tt, ff)      ->
            ignore(costlog(mya_bwalk_it aux clkinfo_o g))
            ignore(walk_bev aux clkinfo_o tt)
            walk_bev aux clkinfo_o ff
        | Xlinepoint(lp, s)   -> walk_bev (m, ss, tallies, wlk_plifun) clkinfo_o s
        | other -> sf ("walk_bev ss other " + hbevToStr other)

and walk_bev aux clkinfo_o (b) =
   //vprintln  0 ("Walk bev " + hbevToStr b)
   let rr = walk_bev_ntr aux clkinfo_o b
   //vprintln 0 ("Walk bev " + hbevToStr b + "done")
   rr 


and mya_bwalk_it aux clkinfo_o n0 =
    let (m, ss:walkctl_t, tallies, wlk_plifun_) = aux
    let n = xb2nn n0
    let ans = mya_bwalk_maid aux clkinfo_o (n) n0
    ss.bunitfn n0
    ans
        
and mya_walk_it aux clkinfo_o n0 =
    let (m, ss:walkctl_t, tallies, wlk_plifun_) = aux
    match n0 with
    | n0 -> 
        let n = x2nn n0
        //vprintln 0  ("+++mya_walk_it " + xToStr n0 + "  " + (if nonep n then "None" else i2s(n)))
        let ans = (*if n=None then ss.cn1 else *) mya_walk_maid aux clkinfo_o (n) n0 // This walks the child expressions 
        ss.unitfn n0 // and then we apply unit function to the current node.
        ans

and mya_bwalk_maid aux clkinfo_o nn n0 =
    let (m, ss:walkctl_t, tallies, wlk_plifun_) = aux
    let n = abs nn
    let oc = valOf_or (ss.usecounts.lookup n) 0
    ss.usecounts.add n (oc+1)
    let ov = ss.costs.lookup n
    if oc>0 && ss.oncef then valOf_or_fail "mya_bwalk_it oc" ov
    else
        if ss.vd >= 8 then vprintln 8 ("Increment b use count to " + i2s (oc+1) + " of " + i2s n + " " + xbToStr n0)
        if not_nonep ov then
            if ss.vd>= 8 then vprintln 8 ("Use old cost for b node")
            valOf ov
        else
                //vprintln 0 ("Starting b for  " + xbToStr n0)
                let (nodeinfo, bsons, sons) = mya_bfwalk aux clkinfo_o n0
                let son_data_b = map (mya_bwalk_it aux clkinfo_o) bsons  
                let son_data = map (mya_walk_it aux clkinfo_o) sons  

                // TODO avoid zip and other work, if null, sonchange functions are provided. 
                let son_data_b_ = map (ss.bsonchange ss.costs nodeinfo nn) (List.zip bsons son_data_b) (* Converts disagreeable children *)
                let son_data_ = map (ss.sonchange ss.costs nodeinfo nn) (List.zip sons son_data)
                let kson0 f c x = 
                        let xn = f x
                        let v = ss.costs.lookup(abs xn)
                        if nonep v then sf ("kson0: bwalk_maid dp order error")  else (valOf v)::c
                let kson__ f c x = 
                        let xn = f x
                        if nonep xn then c
                        else 
                            match ss.costs.lookup (abs(valOf xn)) with
                                // Did fail with  kson: bwalk_maid dp order error n=1092 nn=1092 xn=950 on {SC:d30,0}==X3:"MS" son={SC:d30,0} 
                                | None -> sf (sprintf "kson: bwalk_maid dp order error n=%i nn=%i xn=%i on n0=%s,  son=%s " n nn (valOf xn) (xbToStr n0) (xToStr x))
                                | Some vv -> vv::c
                let kson9 = List.fold (kson0 x2nn) (List.fold (kson0 xb2nn) [] bsons) sons
                let mycost = (ss.opfun) nn (Some n0) None nodeinfo kson9
                if ss.vd >= 5 then vprintln 5 (sprintf "Set b cost of %i for %s" n (xbToStr n0))
                ss.costs.add n (mycost)
                mycost

and mya_walk_maid aux clkinfo_o n n0 =
    let (m, ss:walkctl_t, tallies, wlk_plifun_) = aux
    match n0 with
        // blift has no meo nn and we use that of its son. - should be automatic?
        // X_pair had none at all and was trapped in mya_walk_it
        | X_blift v -> mya_bwalk_maid aux clkinfo_o n v
        | other ->
        let n = abs n // can be negative in a blift.
        //assertf(n >0 && n < ss.dim) (fun() ->("mya_walk_it" + i2s n + " " + xToStr n0))
        let oc = valOf_or (ss.usecounts.lookup n) 0
        ss.usecounts.add n (oc + 1);
        let ov = ss.costs.lookup n
        if oc>0 && ss.oncef then
            match ov with
                | Some v -> v
                | None ->
                   (
                       vprintln 0 ("+++ " + "oc=" + i2s oc + " yet mya_walk_it no old cost nn=" +  i2s n + " " + xToStr n0);
                       ss.costs.add n ss.cn1;
                       ss.cn1
                       //sf ("mya_walk_it no old cost " +  i2s n + " " + xToStr n0)
                   )
        else
           if ss.vd >= 6 then vprintln 6 ("Increment use count to " + i2s (oc+1) + " of nn=" + i2s n + " " + xToStr n0)
           //if false && ov <> None && valOf ov > 5 && uses.[n]) > 1 then lprint 100 (fun()-> "+++ Share node " + xToStr(xint_retrieve(n0.3)) + "\n")
           match ov with
               | Some oc -> oc
               | None ->
                   // Our basic recursive operation is to invoke sonfun on each child expression and then invoke opfun on the expression node supplying also the results from sonfun invokations.
                let (nodeinfo, bsons, sons) = mya_fwalk aux clkinfo_o n0
                let son_data_b = map (mya_bwalk_it aux clkinfo_o) bsons  // This returned data is not passed to opfun - opfun instead uses retrieved data from costs array.
                let son_data   = map (mya_walk_it aux clkinfo_o) sons  
                //let _ = reportx 0 "sons" xToStr sons
                let son_data_b1 = map (ss.bsonchange ss.costs nodeinfo n) (List.zip bsons son_data_b) (* Converts disagreeable children *)
                let son_data1 = map (ss.sonchange ss.costs nodeinfo n) (List.zip sons son_data)
                let ks0 f c x = 
                     let xn = f x
                     let v = ss.costs.lookup(abs xn)
                     if nonep v then sf "ks0: walk dp order error"  else (valOf v)::c
                let ks f c x = 
                     let xn = f x
                     if xn=None then c 
                     else 
                          let v = ss.costs.lookup(abs(valOf xn))
                          //if nonep v then vprintln 0 ("Retrieved zilch  as cost for " + i2s (valOf xn) + " for " + xToStr x)
                          //else vprintln 0 ("Retrieved " + qToStr (valOf v) + " as cost for " + i2s(valOf xn) + " for " + xToStr x)
                          
                          if nonep v then sf "ks: walk L7932 dp order error"  else (valOf v)::c
                //let ks1 = List.fold (ks0 xb2nn) [] bsons
                //let ks2 = List.fold (ks x2nn) ks1 sons
                let mycost = ss.opfun n None (Some n0) nodeinfo (son_data1 @ son_data_b1) // ks2
                ss.costs.add n mycost
                //vprintln ss.vd ("Updated cost of " + i2s n + " " + xToStr n0 + " to " + qToStr mycost)
                mycost

(*
 * Based on costlevel, pin shared expressions (uses count > 1) with that cost or more...
 * This is based on whole mya_array and not a local scope ...
 *
 * Return a list of tuples: (type of event control, the expression pinned, pin net).
 *)
//type pina_t = hexp_t option array
type pina_t = OptionStore<xidx_t, hexp_t>
 
let mya_findshares (ss:walkctl_t) (pina:pina_t) =
    let vd = 4
    let rec kk cc pos =
        if pos >= !mya_splay_nn then cc
        else 
            let ov = ss.costs.lookup pos
            let netg prec id =
                let width = if nonep prec.widtho then 32 else valOf prec.widtho
                let signed = prec.signed
                X_bnet(fst(iogen_serf_ff(id, [], 0I, width, LOCAL, signed, None, false, [], [])))
            let pinner n it =
                let prec = mine_prec true it
                netg prec (funique (sprintf "hpr_pin%ix" n))

            let isshare = function
                | (SHARE_NODE t) -> Some t
                | _ -> None
            let xi_width_or_32 v = 
                    let r = ewidth "xi_width_or_32" v
                    if r<>None && r <> Some 0 then valOf r else 32 
            let c' =
                match pina.lookup pos with
                    | Some it when not_nonep ov  && isshare(valOf ov)<>None  && (valOf_or (ss.usecounts.lookup pos) 0) > 1 ->
                       let ecmode = valOf(isshare(valOf ov))
                       let np = pinner pos it
                       //dev_println ("Share node " + i2s pos + " " + ig2s pos + " w=" + i2s pw + " as  " + xToStr np)
                       (ecmode<>SFH_fcall, it, np)::cc 
                    | _ -> cc
            kk c' (pos+1)
    let ans = kk [] 4  // Iterate over all expressions - not just ones of interest!
    ans

(*--------------------------------------------------------------------------*)
(*
 * bdd 
 *)



(*
 * wot?
 *)
let g_fuse_plug_net_ = gec_X_net("*FUSEPLUG*")


let netwidth(item:net_att_t) = if item.width = 0 then bound_log2(item.rh) else item.width





(*--------------------------------------------------------------------------*)

(*
 * Some global nets defs, not used by simple designs, but available here should they be needed.  
 * In more-complex designs, we expect the directorate to set up clock domains and so on.
 *)
// IP-XACT-style net-attributes
let g_ip_xact_is_Address_tag = "IsAddress"
let g_ip_xact_is_Data_tag    = "IsData"
let g_ip_xact_is_Clock_tag   = "IsClock"
let g_ip_xact_is_Reset_tag   = "IsReset"

let bnetgen tag id = ionet_w(id, 1, INPUT, Unsigned, [Nap(tag, "true")])
let g_resetnet = bnetgen g_ip_xact_is_Reset_tag "reset"
let g_resetbar = bnetgen g_ip_xact_is_Reset_tag "RST_N"
let g_clknet   = bnetgen g_ip_xact_is_Clock_tag "clk"
let g_tnownet  = vectornet_w(g_tnow_string, 64)

// The following net is a psudo that has a single imaginary event when the system is realised.
// This is specified as the clock for start-up/constructor code where the decision as to which clock domain to put the code is made in late recipe stages.
let g_construction_big_bang = bnetgen g_ip_xact_is_Clock_tag "$$big_bang";

let g_reset_is_active_low = ref false // We now deprecate these globals and pass information in the director instead.
let default_reset_f() = if !g_reset_is_active_low then xi_not(xi_orred(g_resetbar)) else xi_orred g_resetnet
let reset_net() = if !g_reset_is_active_low then g_resetbar else g_resetnet


let g_autoformat = gec_X_net(s_autoformat)

let rec is_resetnet x =
    let ats = mine_atts [] x
    at_assoc g_ip_xact_is_Reset_tag ats <> None

and is_bresetnet  = function
    | W_bdiop(V_orred, [x], _, _) -> is_resetnet x
    | _ -> false


// Convert a list of hexp_t expressions containing concatenated items to a flat list of leaf expressions.
let rec remove_concats = function
    | ([]) -> []
    | W_apply(("hpr_concat", gis), _, [l; r], _)::tt -> remove_concats(l::r::tt)
    | (a::b) -> a::remove_concats b



(*
 * Create a printf style formatting string that matches the types of the args.
 * The first string is a special string that triggers the required action. This is then
 * discarded.
 *
 * This function should be called when output is finally needed and the output language is
 * known: ie in verilog_gen, cpp_gen and diosim.
 * 
 * A companion function that handles {n} style C# arguments exists... for in or out ? Kiwi handles input.
 * 
 * The same function is used for both C and Verilog output using verilogf.
 *
 * Some inputs have a placeholder s_autoformat that needs replacing with an appropriate string.  Others already have a format string.
 *
 * Additional behaviour: regardless of whether autoformatting, for write and writeline ...
 * For Verilog, all double-precision FP args need to wrapped in $bitstoreal() regardless of autoformatting.  For Verilog floats they need bottom padding to make them a double.
 *)
let g_bitstoreal_fgis = ("$bitstoreal", { g_default_native_fun_sig with rv=g_default_prec; args=[g_default_prec]; })

// Process percent signs here to double them up as self escapes
let despercent_escape arg =
    let despercent arg cc =
        match arg with
            | '%' -> '%' :: '%' :: cc  // Double up on percent signs
            | other -> other :: cc
    match arg with
        | W_string(ss, stype_, _) ->
            xi_string (implode (List.foldBack despercent (explode  ss) [])) // Has dropped stype attribute !
        | other -> other

        
let replace_autoformat_string verilog_flag args =
    let vd = -1 // Normally -1 for off
    if vd>=3 then vprintln 3 ("replace_autoformat_string " + sfold xkToStr args)
    let ans = 
      match args with
        | [] -> []
        | hh::tt ->
            let charise x = xi_apply(g_hpr_toChar_fgis, [x])
            let isauto = function
                | X_net(v, _)        -> (false, v=s_autoformat)
                | W_string(s, xs, _) -> (s = "%c", s=s_autoformat)
                | _                  -> (false, false)
            let (onechar, en) = isauto hh
            if vd>=4 then vprintln 4 (sprintf "replace_autoformat_string: enable decision=%A" en)
            if en && length tt = 1 && is_string(hd tt) then
                map despercent_escape tt (* No need to autoformat a singleton string *)
            elif onechar then
                [ hh; charise (hd tt) ]
                //muddy "shoutout" // A single character also gets converted toChar but this is a partial answer - we ideally need to spot all %c fields in the format string.
            else
                let classify_field arg = 
                       match arg with
                           | W_string _ -> (-1, "%s", false, false)
                           | arg ->
                               let prec = mine_prec g_bounda arg 
                               let s = prec.signed
                               let aschar = mine_presentation arg = Some "char"
                               if vd>=4 then vprintln 4 (sprintf "aschar=%A for %s" aschar (xToStr arg))
                               let w = valOf_or prec.widtho 32
                               if (not verilog_flag) && w > 64 then vprintln 0 ("+++ C++ printf format bigger than 64 bits")
                               let token =
                                   if aschar then "%c"
                                   elif s=FloatingPoint then "%f"
                                   elif verilog_flag then "%d"  // For Verilog we like to generate %1d when no width is specified to avoid tabs being generated by RTL simulators where mono renders only a space.
                                   else
                                       let wm = if w>32 then "ll" else ""     // The field width numbers are, however, inserted elsewhere by insert_string_escapes.
                                       let cx = if s=Signed then "i" else "u"
                                       "%" + wm + cx
                               (w, token, s=FloatingPoint, aschar)
                        
                let rec make_autoform_string = function
                    | [] -> ""
                    | (_, tok, fp, asChar) ::t ->  tok + make_autoform_string t
                let gg_fp ((w, tok, fp, asChar), x) =
                    //vprintln 0 (sprintf "   tok=%s asChar=%A w=%i" tok asChar w)
                    if fp && w=64 then xi_apply(g_bitstoreal_fgis, [x])
                    elif fp && w=32 then xi_apply(g_bitstoreal_fgis, [xi_apply(g_hpr_flt2dbl_fgis, [x])])
                    elif fp then sf "bad fp width"
                    elif asChar && w <> 8 then
                        let ans = charise x  // Convert brutally from unicode16 to ASCII chars since Verilog does not understand unicode and Verilator needs an 8-bit argument for '%c' fields in $write.
                        //vprintln 0 (sprintf "apply hpr_toChar yielding %s" (xToStr ans))
                        ans
                    else x
                let ans =
                       if en then 
                           let c = map classify_field tt
                           (xi_string(make_autoform_string c)) :: map gg_fp (List.zip c tt)
                       else
                           let c = map classify_field (hh::tt)
                           map gg_fp (List.zip c (hh::tt))

                ans
    if vd>=4 then vprintln 4 ("replace_autoformat_string src=" + sfold xkToStr args + ";  ans=" + sfold xkToStr ans)
    ans





(*---------------------------------------------------------------------*)
let x_get_id = function
    | X_net(id, _) -> id
    | v -> sf("x_get_id " + xToStr(v))


(* Perform long multiplication of y by constant x *)
let xgen_mpxconst(x:hbexp_t list, y:hbexp_t list) =
     let rec zp =
         function
             | ([], [], p, q) -> (rev p, rev q)
             | ([], a::b, p, q) -> zp([ X_false ], a::b, p, q)
             | (a::b, [], p, q) -> zp(a::b, [ X_false ], p, q)
             | (c::cc, d::dd, p, q) -> zp(cc, dd, c::p, d::q)

     let zeropad(a, b) = zp(a, b, [], [])
     let rec add c =
            function
                | ([], []) -> [c]
                | (a::at, b::bt) -> 
                      let s = ix_xor a b
                      let c1 = ix_and a b
                      let c2 = ix_and s c
                      (ix_xor s c)::(add (ix_or c2 c1) (at, bt))
                
     let add0(l, r) = add (X_false) (zeropad(l, r))
        
     let rec it =
            function
                | ([], y, c) -> c
                | (h::t, y, c) -> 
                    let c' = if h=X_false then c else if h=X_true then add0(c, y) 
                             else sf ("mpx constant was not by constant")
                    in it(t, (X_false)::y, c')
     let ans = it(x, y, []) 
     //vprintln 30 ("multiplier: one arg is constant done.")
     ans


// Sign extend a pandex'd value (lsb-first bool list)
let pandex_sex no arg =
    if nonep no then sf(sprintf "pandex_sex: no width specified for " + sfold xbToStr arg)
    else
        //let n = valOf no
        let rec prex n = function
            | [] -> if n<=0 then [] else sf "cannot do sex on an empty list"
            | [item] -> if n<=1 then [item] else item :: prex (n-1) [item]
            | h::tt -> h :: (prex (n-1) tt)
        prex (valOf no) arg        


let xi_blift22 a = xi_blift a (* This can always be  done at retrieve time, but we don't for simplicity. 
  Identities
     orred(blift b) = b
  but
     blift(orred e) is not always e (but is so if width of e is only 1).

  So we could store blift(orred e) as just orred e and keep all blifts implied.  Not much point in
that really.

  So we never need store orred(blift b) or create one in the first place.
*)


(*-------------------------------------------------------------*)



type pandexCtrl_t_ =
 {
     p_dp: Dictionary<int, hbexp_t list>;
     control: control_t;
     //structures: restr_t list ref;
 }


let new_pandex_dp(control) =
   {
     p_dp=     new Dictionary<int, hbexp_t list>();
     control=  control;
     // structures = ref [];
   }

let rec pand_k oo = function
    | ([], []) -> []
    | (a::at, b::bt) -> ix_diop oo a b :: pand_k oo (at, bt)



(* Zero pad extends the shorter arg with leading zeros. *)
let rec pand_zp = function
        | ([], [], p, q)       -> (rev p, rev q)
        | ([], a::b, p, q)     -> pand_zp([], b, X_false::p, a::q)
        | (a::b, [], p, q)     -> pand_zp(b, [], a::p, X_false::q)
        | (c::cc, d::dd, p, q) -> pand_zp(cc, dd, c::p, d::q)

let pand_zeropad(a, b) = pand_zp(a, b, [], [])


let rec pand_add c = function
        | ([], []) -> [c]
        | (a::at, b::bt) -> 
              let vd = -1
              let s = ix_xor a b
              if vd>=5 then vprintln 5 ("    pand_add a = " + xbToStr a )
              if vd>=5 then vprintln 5 ("    pand_add b = " + xbToStr b )
              let c1 = ix_and a b
              if vd>=5 then vprintln 5 ("    pand_add s = " + xbToStr s )
              if vd>=5 then vprintln 5 ("    pand_add c1 = " + xbToStr c1 )
              let c2 = ix_and s c
              if vd>=5 then vprintln 5 ("    pand_add c2 = ... \n")
              //if vd>=5 then vprintln 5 ("    pand_add c2 got  \n")
              //if vd>=5 then vprintln 5 ("    pand_add c2 got  " + xkey k)
              //if vd>=5 then vprintln 5 ("  >> " +  xToStr k + "<<\n")
              //if vd>=5 then vprintln 5 ("    pand_add c2 yeild  \n")
              //if vd>=5 then vprintln 5 ("    pand_add c2 = " + (xToStr c2) )
              (ix_xor s c)::(pand_add (ix_or c2 c1) (at, bt))
        | _ -> sf "unppand_added pand_add"


// Perform bitlist subtraction with carry (borrow bar) in.
let rec pand_subtract_equal_lengths c = function
    | ([], []) -> [c]
    | (a::at, b::bt) -> 
        let b' = xi_not b // Ones complement.
        let s = ix_xor a b' // Half-adder.
        let c1 = ix_and a b'
        // Other half of full adder:
        (ix_xor s c)::(pand_subtract_equal_lengths (ix_or c c1) (at, bt))
    | _ -> sf "pandex subtract differing lengths:  pand_sub"


let pand_add1(a,b) = 
    //vprintln 2 ("padd started")
    let l  = (pand_zeropad(a, b))
    //vprintln 2 (i2s(length(fst l)) + " add mid started")
    let ans = pand_add X_false l
    //vprintln 2 (fun()->i2s(length ans) + " add done and dusted")
(*    let _ = lprint 100 (fun()->" pand_add pandex'd to " + (sfold xToStr ans) + "\n") *)
    ans



let pand_guard g v =
    match g with
        | X_false -> [X_false]
        | X_true -> v
        | _ -> map (fun x -> ix_and g x) v


let rec pand_mux g =
    function
        | ([], []) -> []
        | (a::at, b::bt) -> xi_orred(xi_query(g, xi_blift22 a, xi_blift22 b)) :: (pand_mux g (at, bt))

let pand_muxzp g (a, b) = pand_mux g (pand_zeropad(a, b)) 


let rec pand_rshift (signed) (prec:precision_t) = function
    | (a, [], x) -> a
    | (a, h::t, x) -> 
        let r = pand_rshift signed prec (a, t, x*2)
        let unsigned = prec.signed = Unsigned || signed=Unsigned
        if not unsigned then muddy "pand_rshift signed"
        let rec pand_rshift1 a n = if n=0 || a=[] then a else pand_rshift1 (tl a) (n-1)
        pand_muxzp h (pand_rshift1 r x, r)

let rec pand_lshift = function
    | (a, [], x) -> a
    | (a, h::t, x) -> 
        let rec pand_lshift1 (a, n) = if n=0 then a else X_false::(pand_lshift1(a, n-1))
        let r = (pand_lshift(a, t, x*2))
        pand_muxzp h (pand_lshift1(r, x), r)

let rec pand_times signed_ = function // signed is ignored for now
    | ([], v) -> [ X_false]
    | (h::t, v) -> pand_add1(pand_guard h v, pand_times signed_ (t, X_false :: v))


let rec pand_ka oo = function
    | ([], []) -> []
    | (a::at, b::bt) -> 
        let rr = if oo=V_xor then ix_xor a b
                 else if oo=V_bitand then ix_and a b
                 else if oo=V_bitor then ix_or a b
                 else xi_orred(ix_diop g_default_prec oo (xi_blift22 a) (xi_blift22 b)) (* not used *)
        rr :: pand_ka oo (at, bt)
    | _ -> sf "unppand_added pand_ka"


// Not-equal predicate: disjunction of xors.
let rec pand_dneq r =  function
    | ([], []) -> r
    | (a::at, b::bt) -> pand_dneq (ix_or (ix_xor a b) r) (at, bt)


let rec pand_dltd r = function
        | ([], []) -> r
        | (a::at, b::bt) ->
            let r' = ix_or (ix_and (xi_not a) b) (ix_and (ix_xnor a b) r)
            //vprintln 0 ("pand_dltd a=" + xbToStr a + " b=" + xbToStr b + " r=" + xbToStr r + " r'=" + xbToStr r') 
            pand_dltd r' (at, bt)


// Signed multiply has one fewer product bits than unsigned if we negate the answer.
//
// With 3 bit unsigned fields the maximum product is 49 which is 110001, needing 6 bits.
// With 3 bit signed fields, ranges are 3*-4 = -12 which is 110100 and -4*-4=16 01000. A 5 bit result almost suffices, since it can hold -16 to +15. The negated product would fit!
//
// The difference between signed and unsigned multiply can be illustrated with -2 * -2 in 3-bit fields
//  -2 is 110  and -4 is 11100
//  -2 is also 6, and 6*6=36 is 100100 - which is the same in the low bits but different above.
//
// Assuming one argument is
// To make a signed multiplier from an unsigned core we need to correct for each argument is negative. A negative argument
// is a subtracted pair: for instance -2 in 3 bits is stored as (8-2).  If we have this as both operands to an unsigned multiplier, we get
//    (8-2)*(8-2) = (64 - 16 - 16 + 4) = 36 = 100100 (as above).  To fix this, we need to add on 16 for each operand.  The 64 is discarded since it is in Bit6 and we
// just want a six-bit result.            
//
#if SPARE
let rec pand_multiplier___ ww P (x, y) = 
    let xl = length x
    let yl = length y
    let rw = max xl yl
    let _ = vprintln 3 ("Pand: multiplier " + i2s xl + "x" + i2s yl)
    if conjunctionate bconstantp x then xgen_mpxconst(x, y)
    elif conjunctionate bconstantp y then xgen_mpxconst(y, x)
    else 
        let fgis = ("MPX", { g_default_native_fun_sig with rv={g_default_prec with widtho=Some rw}; args=[xl; yl] })
        let rn = vectornet_w("o2mpx", rw)       
        let r = ix_pair (xi_apply(fgis, [xi_num rw; xi_num xl; xi_num yl] @ rn :: (map xi_blift22 (x @ y)))) rn
        pandex ww 0 P r
#endif


(*
 * Pandex converts to bit-blasted form, lsb first.  w is a target width, needed for sign extensions.
 * 
 *
 *)
and pandex ww w P x = 
    //vprintln 4 ((xToStr x) + " pandex start")
    let rr = pandex_ntr ww w P x 
    //vprintln 4 ((xToStr x) + " pandex completed")
    rr

and bpandex_ntr ww pP bA =
    match bA with
        | X_false    ->  bA 
        | X_true     ->  bA 
        | X_dontcare ->  muddy "xi_dontcare" 

#if NOCOVER        
        | W_bnode(V_band, [t; f], false, _)  -> [ix_and t f] // WHY FALSE HERE ?   Why no clause for false!
        | W_bnode(V_bor, [t; f], false, _)   -> [ix_or t f]
        | W_bnode(V_bxor, [t; f], false, _)  -> [ix_xor t f]
#endif
        | W_cover(lst, _) ->
            let rec pandex_orred = function
                | [] -> X_false
                | [term] -> term
                | h::t -> ix_or h (pandex_orred t)

            let lf x = (*pandex_orred*) (bpandex ww pP (deblit x)) // This is bitwise code!! Not right.?  This should be orred here!
            let rec diog f keep = function
                | ([], []) -> []
                | (l::ls, r::rs) -> f(l, r) :: diog f keep (ls, rs)
                | (x, [])
                | ([], x) -> if keep then x else []
            let rec sqand = function
                | [] -> X_true
                | [item] -> lf item
                | h::t -> ix_and (lf h) (sqand t)
            let rec sqor = function
                | [] -> X_false
                | [term] -> sqand term
                | h::t -> ix_or (sqand h) (sqor t)
            sqor lst 

        | W_bdiop(V_orred, [item], iv, _) -> 
            let ivf x = if iv then xi_not x else x
            let lst = pandex ww None pP item
            let rec orred = function
                 | [item] -> item
                 | h::tt -> ix_or h (orred tt)
                 | [] -> sf "orred []"
            ivf (orred lst)


        | W_bdiop(oo, [t; f], iv, _) -> 
            let t' = pandex ww None pP t
            let f' = pandex ww None pP f
            let non_ivf x = if iv then xi_not x else x
            let ivf x = if iv then x else xi_not x
            //vprintln 0 ("pandex W_bdiop start" + xbToStr bA)
            let ans =
                match oo with
                    | V_dned -> non_ivf(pand_dneq X_false (pand_zeropad(t', f')))
                    | V_deqd -> ivf(pand_dneq X_false (pand_zeropad(t', f')))

                    | V_dltd signed__ -> non_ivf(pand_dltd X_false (pand_zeropad(t', f'))) // TODO signed__ is sadly ignored!
                    // a<=b is b>=a is !(b<a)
                    | V_dled signed__ -> ivf(pand_dltd X_false (pand_zeropad(f', t')))

                    | other-> sf("other form " + xbToStr bA + " bpandex bdiadic")
            //vprintln 0 ("pand bdiop end")
            ans

        | W_bsubsc(l, r, iv, _) -> 
            let ivf x = if iv then xi_not x else x
            let l' = pandex ww (Some 1) pP (ix_bitand (ix_rshift Unsigned l r) (xi_num 1))
            cassert(l' <> [], "Fewer than one bit in bsubsc")
            ivf(hd l')
            
        | W_bitsel(X_bnet f, n, iv, _) -> 
            let w = encoding_width (X_bnet f)
            if n < 0 || n >= w then sf(xbToStr bA + ": pandex: bitsel out of range")
            let ivf x = if iv then xi_not x else x
            ivf bA


        | other -> sf("bpandex other " + xbToStr other)

and bpandex ww P x =
    let vd = -1 // -1 for off
    let nn = xb2nn x
    let (found, ov) = P.p_dp.TryGetValue nn
    if found then hd ov
        else
            //vprintln 0 ("Bpandex start " + xbToStr x)
            let rr = bpandex_ntr ww P x
            //vprintln 0 ("Bpandex end " + xbToStr x)
            P.p_dp.Add(nn, [rr])
            rr
            
and pandex_ntr ww (wix:int option) pP aA =
    let vd = -1 // -1 for off
    let gwidth prec =
        if not_nonep wix then valOf wix
        elif not_nonep prec.widtho then valOf prec.widtho // this ordering needs checking on a clause-by-clause basis
        else sf ("pandex: need width in " + xToStr aA)

    match aA with
        | X_bnet f ->
           let dwidth = encoding_width aA
           //vprintln 0 ("Pandex " + xToStr aA + " dwidth=" + i2s dwidth)
           map (fun x -> xi_bitsel(aA, x)) [0 .. dwidth-1]

        | X_net(_) -> [ xi_orred aA ]
        | X_undef ->  [ xi_orred aA ]
        | W_query(g, t, f, _) -> 
            let g' = bpandex ww pP g
            let t' = pandex ww wix pP t
            let f' = pandex ww wix pP f
            let rec k = function
                | ([], []) -> []
                | (a, []) -> k(a, [ X_false ])
                | ([], b) -> k([ X_false ], b)
                | (a::at, b::bt) -> xi_query(g', xi_blift22 a, xi_blift22 b) :: k(at, bt)
            map xi_orred (k(t', f'))

        | W_string(s, XS_withval x, _) -> pandex_ntr ww wix pP x

        | W_string(s, _, _) -> muddy "pandex string needed"

        (*| (W_mask(h, l)) ->  
             let zgen n = if n <= 0 then [] else X_false :: zgen(n-1)
             let onegen n = if n <= 0 then [] else X_true :: onegen(n-1)
             let zeros = zgen (min(h,l))
             let ones = onegen (1+abs(h-l))
             in zeros @ ones 
        *)
#if TNUM
        | X_tnum lst -> 
             let rec k n = if wide_zerop n then []
                           else (if wide_oddp n then X_true else X_false) :: (k (wide_divmod(true, n, [2])))
             let rec q n = if wide_zerop n then [X_true] (* Final sign bit *)
                           else (if wide_oddp n then X_true else X_false) :: (q (wide_divmod(true, n, [2])))
             let _ = vprintln 10 ("pandex_ntr X_tnum " + xToStr aA)
             let ans = if wide_negativep lst then q(wide_addsub(false, wide_negate lst, [1]))
                       else if wide_zerop lst then [ X_false ] else k lst
             ans
#endif
        | X_bnum (w, bn, _) -> 
            let rec kk (bn:BigInteger) =
                if bn.Sign <0 then muddy "-ve bnum"
                if bn.IsZero then []        
                else
                    let rem = ref 0I
                    let q = BigInteger.DivRem(bn, 2I, rem)
                    (if (!rem).Sign > 0 then X_true else X_false) :: kk q
            kk bn

        | X_num n ->
            if n = 0 then [ X_false ]
            else 
              //Example:  -4 in a 6 bit field is 111100
             let rec k = function
                 | 0 -> []  (* lsb first *)
                 | n -> (if (n % 2)=1 then X_true else X_false) :: k (n/2)
             let rec q = function
                 | 0 -> [X_true]  (* final negative sign bit *)
                 | n -> (if (n % 2)=0 then X_true else X_false) :: q (n/2)
             if vd>=5 then vprint 5 (i2so wix + " width pandex_ntr X_num " + xToStr aA + "\n")
             let ans = if n >= 0 then k n else pandex_sex wix (q (0-1-n))
             if vd>=5 then vprintln 5 ("pandex_ntr X_num " + xToStr aA + " done")
             ans


        // Monadic operator: cast.
        | W_node(prec, V_cast cvtf, [arg], _) ->
            let ans = 
                match cvtf with
                    | CS_typecast ->                  // Just a typecast that may alter a subsequent operator overload selection.
                        pandex_ntr ww (prec.widtho) pP arg
                    | CS_maskcast ->                  // Clipping etc to field width.
                        let width = gwidth prec
                        let mask = gec_X_bnum({widtho=Some width; signed=Unsigned}, himask width)
                        pandex_ntr ww None pP (ix_bitand mask arg)
                    | CS_preserve_represented_value -> // A major bit-changing convert (e.g. int to float).
                        muddy (sprintf "Unsupported pandex cast " + xToStr aA)

            ans

        // Monadic operator: one's complement.
        | W_node(prec, V_onesc, [xx], _) ->
            let bitlist = pandex_ntr ww wix pP xx
            // Should warn if wix has a value different from length bitlist
            map (xi_not) bitlist

        // Monadic operator: two's complement.
        | W_node(prec, V_neg, [xx], _) ->
            let rhs = pandex_ntr ww wix pP xx
            let cin = X_true // no borrow
            pand_subtract_equal_lengths cin (map (fun bit -> X_false) rhs, rhs)


        | W_node(prec, oo, lst, _) ->
            let keep = oo=V_bitor||oo=V_xor
            let f = if oo=V_bitand then ix_and elif oo=V_bitor then ix_or else ix_xor
            let rec diog = function
                | ([], []) -> []
                | (l::ls, r::rs) -> (f l r) :: diog (ls, rs)
                | (x, [])
                | ([], x) -> if keep then x else []

            let rec bitwise = function
                | [item] -> item
                | a::b::tt -> bitwise(diog(a,b)::tt) 
            let lst' = map (pandex ww None pP) lst
            
            match oo with
                | V_bitor | V_bitand | V_xor -> bitwise lst'
                | V_plus                     -> pand_add1(hd lst', hd(tl lst'))
                | V_lshift -> pand_lshift(hd lst', hd(tl lst'), 1)                
                | V_rshift ss -> pand_rshift ss prec (hd lst', hd(tl lst'), 1)
                | V_times  -> pand_times prec (hd lst', hd(tl lst')) 
                | _ -> sf ("pandex unsupported diadic op " + xToStr aA)

        | X_x(g, n, _) -> muddy " map (fun q->xi_orred(xi_x_x(xi_blift22 q, n))) (pandex ww w g) "

        | X_pair(t, f, _) -> 
            let t' = t (* could leave as is: a void side effector no doubt. *)
            let f' = pandex ww wix pP f
            if length f' < 1 then [muddy "pandeX_pair t'" ]
            else xi_orred(ix_pair t' (muddy "pandeX_pair f'")) :: tl f'

        | X_blift b ->
            let b' = bpandex ww pP b
            [b]


        | other -> sf("pandex other " + xToStr other)


let pandor ww pP v = 
    let vd = -1 // off
    let v' = pandex ww (Some 1) pP v
    if vd>=5 then vprintln 5 ("or-reduce items " + (sfold xbToStr v'))

    let rec orred = function
        | [item] -> item
        | (h::t) -> ix_or h (orred t)
        | [] -> sf "orred []"
    orred v'


(*
 * Pid creates an imaginative name for various nets...
 *)
let pid1 (ss:string) =
    if String.length(ss) > 3 then ss.[0..2] + ss.[(String.length(ss)-3)..]
    else "pid"


let rec pid = function
    | X_bnet ff         -> pid1 (ff.id)
    | X_subsc(l, r)     -> pid l
    | W_asubsc(l, r, _) -> pid l
    | other -> (vprintln 0 ("+++pid id other " + xToStr(other)); funique "piddle")
  
let xi_queryp =
    function
        | (W_bdiop(V_orred, [W_query(g, l, r, _)], false, _)) -> Some(g, xi_orred l, xi_orred r)
        | _ -> None


(*
 * Factorise an expression into a conditional difference with respect to a reference expression.
 * A clock enable factorisor.
 *)
let rec lr_factor l r  = 
    match (xi_queryp r) with
     | Some(g, t, f) -> 
        let (y_t, need_t) = lr_factor l (t)
        let (y_f, need_f) = lr_factor l (f)
        let kquery(g, t, f) = ix_or (ix_and g t) (ix_and (xi_not g) f)
        let n = kquery(g, need_t, need_f)
        let r = if need_t = X_false then y_f
                else if need_f = X_false then y_t
                else xi_orred(xi_query(g, xi_blift y_t, xi_blift y_f))
        (r, n)

     | None -> 
        if l=r
        then (r, X_false)
        else (r, X_true)

(*
 * A generic expression walker - cannot walk hbev or RTL however.  You must use the abstracte.fs walker code for that.
 * new_walker sets up the control vector for a walk. The expression walk functions themselves are called mya_bwalk_it and mya_walk_it // (cf. xint_assoc_exp and _bexp.)
 *)
let new_walker vd vdp (oncef, opfun, bunitfn, lfun, unitfn, bsonchange, sonchange) = 
      let pina = new pina_t("pina") // Array.create g_mya_limit None
      let ss = 
          { costs=       new   walker_costs_t("costs")
            usecounts=   new OptionStore<xidx_t, int>("usecounts")
            oncef=         oncef            // Holds if we are to scan a subtree only once *)
            opfun=         opfun pina       //
            bsonchange=    bsonchange       //
            sonchange=     sonchange        //
            bunitfn=       bunitfn          //
            unitfn=        unitfn           //
            lfun=          lfun             //
            vd=            vd
            vdp=           vdp
            cn0=COST_NODE(0, SFH_none); 
            cn1=COST_NODE(1, SFH_none);
//          walker_waypoint= ref None;
          } : walkctl_t
      (pina, ss)




let clockgen(n, t) =
        [ Xassign(n, xi_blift X_false), Xwaitabs(ix_plus t g_tnownet),
          Xassign(n, xi_blift X_true), Xwaitabs(ix_plus t g_tnownet)
        ];


// Return true if arg could block.  Strictly, we need to check all expressions for blocking function callls ...*)
let rec blocking = function
        | Xskip -> false
        | (Xblock l) -> foldl (fun(a,b)->b || blocking a) false l
        | (Xassign _) -> false
        | (Xcomment _) -> false
        | (Xreturn _) -> false
        | (Xif(g, c, d)) -> blocking c || blocking d
        | (Xwhile _) -> true
        | (Xdominate _) -> true
        | (Xgoto _) -> true
        | (Xbeq _) -> true
        | (Xannotate(_, s)) -> blocking s
        
        | other -> sf("blocking other:" + hbevToStr other)



(*
 * Process a list of IF statements so that common guard conjuncts are shared.
 * Maintain order of statements.
 * Possible bug if one assigns to its own guard!  TODO check!
 *
 * ifshare1 scans for IF statements and adds them to the sofar list, emitting it if 
 * we encounter a non-if.
 *
 * ifshare_emit ...
 *)
let rec ifshare1 arg sofar =
    match arg with
    | []                   -> ifshare_emit(sofar)
    | Xif(g, t, Xskip)::tt -> ifshare1 tt ((xi_clauses (xi_b[]) g, t)::sofar)
    | other::tt            -> ifshare_emit(sofar) @ other :: hbev_ifshare(tt) 

and ifshare_emit items = 
    let rec emit1 x l = gec_Xif x (gec_Xblock(start_grp l)) Xskip
    
    and grp x c aA =
        match aA with
                | []              -> [emit1 x (rev c)]
                | (([], v)::tt)   -> (emit1 x (rev c)) :: (start_grp aA)  
                | ((a::b, v)::tt) ->    
                    if a=x then grp x ((b,v)::c) tt
                    else (emit1 x (rev c)) :: (start_grp aA)

    and start_grp arg =
        match arg with
        | [] -> []
        | (a, v)::tt ->
            if nullp a then v :: start_grp tt 
            else grp (hd a) [] arg


    let astart_grp (lst, cmd) = gec_Xif (ix_andl lst) cmd Xskip (* Poor man's version *)

    start_grp(rev(map (fun(l,v)->(xi_valOf l, v)) items))

and hbev_ifshare lst = ifshare1 lst []



// Delay operator: can refer to future and past values of an expression.
// Definitions: X_x(d, 0) === d
// In rmode X_x(d, -1) is the o/p of a D-type whose input is d.

let xib_X n arg = xi_orred(xi_X n (xi_blift arg))

let xi_X1 e = xi_X 1 e // Get the future value of e: i.e. the inputs to the D-type register that holds it, or equivalent.

let xi_Xm1 e = xi_X -1 e // Add a one cycle delay to e.


let gen_psl_fell(a) = xi_and(xi_not a, xi_orred(xi_X (-1) (xi_blift a)));
let gen_psl_rose(a) = xi_and(a, xi_not(xi_orred(xi_X (-1) (xi_blift a))));
let gen_psl_stable(a) = xi_deqd(a, xi_Xm1 a);
let gen_psl_prev(a) = xi_Xm1 (a);
let gen_psl_next(a) = xi_X1    (a);



let psl_compile E = function
    | (X_psl_builtin("fell", a)) -> xi_blift(gen_psl_fell (xi_orred a))
    | (X_psl_builtin("rose", a)) -> xi_blift(gen_psl_rose (xi_orred a))
    | (X_psl_builtin("prev", a)) -> gen_psl_prev a
    | (X_psl_builtin("next", a)) -> gen_psl_next a
    | (X_psl_builtin("stable", a)) -> xi_blift(gen_psl_stable a)
    | (other) -> sf("psl compile other" + xToStr other)



(*
 * VM machines have attributes: sometimes a new att will mask out an old one.
 * We shouldn't have the same att repeated at least.
 *)
let atts_merge (v, c) = v::c (* FOR NOW *)


let mutatt mlist newat = mlist := atts_merge(newat, !mlist)


(*
 * Find a setting for a conjunction of conditions that makes them all satisfied. 
 * Actually, these are all the ways of representing zero currently, so dont need djg_sat.
 *)
let unify s a =
    let rec u1 s = function
        | [] -> s
        | X_blift(W_bdiop(V_deqd, [X_bnet ff; X_num mm], false, _))::t -> u1 ((ff.id, X_num mm)::s) t
        | (other::t) -> sf("unify u1 match other:" + xToStr other)
    u1 s a


let xi_query_undef = function
        | (gg, X_undef, ff) -> ff
        | (gg, tt, X_undef) -> tt
        | (gg, tt, ff) -> xi_query(gg, tt, ff)


let xi_pairp = function
    | X_pair _ -> true
    | _ -> false


// This function attempts to fix an unpleasant interaction of our meox normal form, where
// (-3<unsignedx) is incorrectly rendered as such: instead we would like (0<3+unsignedx)
// We correct by subtracting from both sides.
let unsigned_correct_bdiop (ll, rr) =
    let msg = "unsigned_correct"
    let linearop = function
        | V_plus | V_minus ->true
        | _ -> false
    if xi_is_unsigned ll || xi_is_unsigned rr
    then 
        let usc = function
            | m when constantp m && constant_comp (V_dltd Unsigned) (m, xi_zero)=1 -> Some m
            | W_node(prec, oo, m::tt, _) when linearop oo && constantp m && constant_comp (V_dltd Unsigned) (m, xi_zero)=1 -> Some m
            | _ -> None
        let lv, rv = usc ll, usc rr
        if lv=None && rv=None then (ll, rr)
        else
            let d = if lv=None then valOf rv else if rv=None then valOf lv else xi_min (valOf lv, valOf rv) 
            //vprintln 20 ("Correcting unsigned negative comp by " + xToStr d)
            (ix_minus ll d, ix_minus rr d)
    else (ll, rr)



(* Array subscript comparisons code.
 * Some linear programming here ....
 * Check whether manifestly out of range, before discarding range info...
 *)
let rec subsm1 (aa, bb) =
    //vprintln 0 ("subsm1 " + xToStr_plus_key aa + " cf " + xToStr_plus_key bb)
    let ans = 
        match (aa, bb) with
        | (X_pair(range, b, _), X_num a)
        | (X_num a, X_pair(range, b, _)) ->
            let outside_range a b =
                  match a with
                  | X_pair(X_num baseer, X_num length, _) -> b < baseer || b >= baseer+length
                  | other ->
                      let _ = vprintln 1 ("+++not an array subscript range " + xToStr other)
                      false
            if outside_range range a then Some false else subsm1(X_num a, b)

        | (X_pair(baser, a, _), b) -> subsm1(a, b)
        | (a, X_pair(baser, b, _)) -> subsm1(a, b)

        | (X_num a, X_num b) -> Some(a=b) // Other int forms in last clause.

        // Todo: could divide by hcf, as conerefine does.

        // We used to normalise by moving constant offsets to lhs operand when commutative. Surely we still do?
        | (W_node(prec1, V_plus, lst1, _), W_node(prec2, V_plus, lst2, _)) ->
            let delta = lists_delta lst1 lst2
            if delta = [] then Some true
            else
                //vprintln 0 ("subsm: list delta " + sfold xToStr_plus_key delta)
                let c = length delta=2 && constantp (hd delta)
                if not c then None
                else Some(xi_iszero(ix_minus (hd delta) (hd(tl delta))))
                    
        | (A, W_node(prec, V_plus, [X_num c; B], _)) when A=B -> Some(c=0) // meo should never make such a node!

        // | (A, W_node(prec, V_minus, [B; X_num c], _)) -> subsm1(A, xi_node(V_plus, [B; xi_num(-c)]))  : minus now deprocated

        | (a, b) ->
            //vprintln 0 "Defaulting to udeq"
            let rd = x_udeq a b
            rd
    let _ = vprintln //0 ("subsm done " + booloToStr ans)
    ans


let subsmatch m (alpha, beta) =
    let r = subsm1(alpha, beta)
    in if r <> None then valOf r
       else
           let s = subsm1(beta, alpha)
           in if s <> None then valOf s
              else sf((m()) + ": name alias: cannot match\n   A=" + xToStr alpha + "\n   B=" + xToStr beta + "\nPlease insert a pause to force array operations into different regions.\n\n")

exception Name_alias of string


let subsmatch_o m = function
    | (Some alpha, Some beta) -> subsmatch m (alpha, beta)
    | (None, None) -> true
    | _ -> sf ("Mixed presence of subscripts: " + m() + ": subsmatch_o")


//
// Store a next-state function (nsf) and allow backwards projections.
//
type nsf_manager_t(name: string, clknet:edge_t, resetnet:hexp_t) = class
  let vd = 3 
  // For scalars we have a nominal 0 or -1 entry plus future values created by projection.
  let scalar_nsf = OptionStore<xidx_t, hexp_t * (int * hexp_t) list>("scalar_nsf")
  // For vectors we only store a single NSF +1 form (for now)
  let vector_nsf = Dictionary<xidx_t, hexp_t * (hbexp_t * hexp_t * hexp_t) list>()  
  let interlock = ref false
  member x.lookup n l =
      let ov = scalar_nsf.lookup(x2nn l)
      //if ov=None then None else
      op_assoc n (snd (valOf ov))

  // Here values of nx are in LMODE form, positive meaning 'future values of', negative meaning the past.
  // We expect the basic NSF (X(Q)=d) to be installed as 1 (Q, d) meaning that (d, 1) is stored for Q.
  member x.register_local m nx defval (g, l, r) =
      if vd>=4 then vprintln 4 m
      match l with
          | X_bnet ff ->
             let ov = scalar_nsf.lookup ff.n
             if ov=None then
                 let _ = 0
#if NSF_MANAGER_DEBUG
                 vprintln 0 ("Register scalar none already")
#endif                                               
                 let m1 = name + " Register m1" //  + xToStr l + " nx=" + i2s nx + " g=" + xbToStr g + " r= " + xToStr r
                 scalar_nsf.add ff.n (l, [(nx, xi_query(g, r, defval))])
             else
                 let ov1 = op_assoc nx (snd(valOf ov))
#if NSF_MANAGER_DEBUG
                 vprintln 0 ("Register scalar some already")
#endif                                               
                 if ov1=None then scalar_nsf.add ff.n (l, (nx, xi_query(g, r, l))::(snd(valOf ov)))
                 else
#if NSF_MANAGER_DEBUG
                           vprintln 0 ("Register scalar some but no same nx already")
#endif                                               

                           let rhs' = ix_query g r (valOf ov1)
#if NSF_MANAGER_DEBUG
                           vprintln 0 ("Register cond " + xbToStr g + " r=" + xToStr r + " ov=" + xToStr (valOf ov1) + " ans=" + xToStr rhs')
#endif
                           scalar_nsf.add ff.n (l, (nx, rhs')::(snd(valOf ov))) // Masking? One for each nx. TODO? remove item

          | W_asubsc(X_bnet ff, subs, _) when nx=1 ->
             let k = X_bnet ff
             let (found, ov) = vector_nsf.TryGetValue ff.n
             if not found then vector_nsf.Add(ff.n, (k, [(g, subs, r)]))
             else
                    let mfun () = name + " register asubsc " + xToStr l + " " + xToStr subs
#if NSF_MANAGER_DEBUG
                    vprintln 0 (mfun())
#endif
                    let rec scan = function
                        | [] -> None
                        | (g, h, vale)::tt ->
                            //vprintln 0 ("  subsm scan cf " + xToStr h)
                            let d = subsmatch mfun (h, subs)
                            if d then Some(g, h, vale) else scan tt
                    let d = scan (snd(ov))
#if NSF_MANAGER_DEBUG
                    vprintln 0 "scan'd"
#endif
                    if d=None then
                        ignore(vector_nsf.Remove ff.n)
                        vector_nsf.Add(ff.n, (k, (g, subs, r)::(snd(ov))))
                     else
                         ignore(vector_nsf.Remove ff.n)
                         let ov' = remove_item(snd(ov), valOf d)
                         let r' = xi_query(g, r, f3o3(valOf d))
                         let g' = ix_or g (f1o3(valOf d))
                         vector_nsf.Add(ff.n, (k, (g', subs, r')::ov'))


          | X_x(l1, n1, _) ->
              let _ = 0
#if NSF_MANAGER_DEBUG
              let m1 = name + " Register X_x m1" + xToStr l + " nx=" + i2s nx
              vprintln 0 m1
#endif
              x.register_local "m1" (nx+n1) defval (g, l1, r)
          
          | other -> sf ("nsf_manager: " + name + ":  invalid item lhs=" + xToStr l + " rhs=" + xToStr r)

  member x.register nx defval (g, l, r) =
      let m = (name + " Register " + xToStr l + " nx=" + i2s nx + " g=" + xbToStr g) // TODO make lazy
      //  + " r= " + xToStr r) 
      if !interlock then sf ("Interlock: " + m)
      if true then x.register_local m nx defval (g, l, r)

         
    // Do not register further leaves after first calling xi_x. The interlock assures this.
    // Well we do call it for debug printing in the interim ...
  member x.xi_x n vv =
        interlock := true
        let xpredicate arg =
            //vprintln 0 ("xpredicate arg=" + xToStr arg)
            match arg with
            | X_bnet _ -> true
            | W_asubsc(X_bnet f, _, _) -> true            
            | _ -> false
        let mop_pls1 g arg = // Replace 
            match arg with
            | W_asubsc(X_bnet ff, subs, _) ->
                let k = X_bnet ff
                let (found, ov) = vector_nsf.TryGetValue ff.n
                let mfun () = name + " vec lookup " + xToStr arg
                if not found then ix_asubsc k subs // It was not written then? 
                else
                let rec scan = function
                    | [] -> ix_asubsc k subs
                    | (g, h, vale)::tt -> xi_query(ix_and g (xi_deqd(h, subs)), vale, scan tt)
                        //let d = subsmatch mfun (h, subs)
                // put functional array back together:
                if not found then sf (mfun() + "needed vec c ")
                else
                    let ans = scan (snd ov)
                    //if vd>=4 then vprintln 4 (name + " vec rebuild ->" + xToStr ans)
                    ans
            | X_bnet _ ->
                match x.lookup 1 arg with
                    | Some y -> y
                        //vprintln 4 ("Projecting backwards for " + xToStr arg + " " + optToStr y)
                    | None ->
                        let y1 = x.lookup 0 arg
                        if y1=None then
                            vprintln 1 ("+++No buffer or one-step NSF registered for " + xToStr arg) // ok if RDD bus since that is backwards?
                            arg
                        else x.xi_x 1 (valOf y1)
                           
            | arg -> sf ("mop_pls1 other " + xToStr arg)
            


        let rec xi_xss n st vv = 
            if n=0 then vv
            elif memberp (n, vv) st then sf ("xi_x loop finding " + xToStr (xi_X n vv))
            else
                match x.lookup n vv with
                    | Some v -> v
                    | None ->
                        if n < 0 then muddy ("xi_x: n=" + i2s n + " rmode -ve temporal power: nsf delay reg gen not implemented:" +  xToStr vv)
                        else
                            let vp = xi_xss (n-1) ((n, vv)::st) vv
                            let mapping = hexp_agent_add (makemap []) (xpredicate, mop_pls1, (true, false))
                            let ans = xi_rewrite_exp mapping vp
                            let m = ("Projected backwards " + xToStr vp  + "\n  gave " + xToStr ans)
                            if vd>=4 then vprintln 4 m
                            let dp_cache n vv =
                                match vv with
                                    | X_bnet _ -> x.register_local m n (vv) (X_true, vv, ans)
                                    | _ -> ()
                            dp_cache n vv
                            ans

        xi_xss n [] vv

    member x.norm vv = // Non-boolean (scalar) version.
        // Norm removes all X_x forms from a stored function.
        // Negative ones require new registers to be created and +ve ones (that refer to sequential support) are eliminated by projecting back through the NSF. 
        let mpred = function
            | X_x(e, n, _) -> true
            | _            -> false
        let mmapper g arg =
            match arg with
            | X_x(e, n, _) -> x.xi_x n e                
            | arg          -> sf ("mmapper other " + xToStr arg)
            
        let mapping = hexp_agent_add (makemap []) (mpred, mmapper, (true, false))
        let ans = xi_rewrite_exp mapping vv
        ans
    

    member x.normb vv = // Boolean version
        // We aim to remove all X_x forms from a next-state function.
        // Negative ones require new registers to be created and +ve ones are eliminated by projecting back through the NSF. 
        let mpred = function
            | X_x(e, n, _) -> true
            | _            -> false
        let mmapper g arg =
            match arg with
            | X_x(e, n, _) -> x.xi_x n e                
            | arg       -> sf ("mmapperb other " + xToStr arg)
            
        let mapping = hexp_agent_add (makemap []) (mpred, mmapper, (true, false))
        let ans = xi_rewrite_bexp mapping vv
        ans

    member x.seq_lhs_list () =
        let ans = ref []
        let j_scal (k, v) =
            let rhso = op_assoc 1 v
            if rhso <> None then mutadd ans (k, valOf rhso)
        for z in scalar_nsf do j_scal z.Value done

        let j_vec (k, items) =
            let j_vec_serf (g, subs, vale) = 
                let aa = ix_asubsc k subs
                in mutadd ans (aa, xi_query(g, vale, aa))
            app j_vec_serf items
        for z in vector_nsf do j_vec z.Value done        
        !ans

    member x.comb_lhs_list () =
        let ans = ref []
        let j1 (k, v) =
            let rhso1 = op_assoc 1 v
            let rhso0 = op_assoc 0 v            
            if rhso1=None && rhso0 <> None then mutadd ans (k, valOf rhso0)
        for z in scalar_nsf do j1 z.Value done
        !ans
end



// Please use the right hand side function names for generating memo-heap entries.
// Lhs names are for compatibility with older genericgates code: legacy mappings.
let xi_true = X_true;
let xi_false = X_false;
let retrieve other = other
let bretrieve other = other
let xiplainToStr = xToStr;


// General coding style: ix_ or xi_ prefix means public function, with ix being curried forms (these are more efficient on FSharp)
// User API: was via xgen_ but now ix preferred.
// These xgen_ form calls are now deprecated - the user code should call the curried ix_ forms that form the new public interface.
let xgen_bsubsc(P, Q) = xi_bsubsc(P, Q, false)
let xgen_blift = xi_blift
let xgen_num n = xi_num n
let xgen_bitsel(e, s) = xi_bitsel(e, s)
let xgen_orred = xi_orred
let xgen_div(n, d) = ix_diop_w g_default_prec V_divide n d
let xgen_sub(l, r) = ix_minus l r
let xgen_deqd(l, r) = if l=r then X_true else xi_deqd(l, r)
let xgen_dned(l, r) = if l=r then X_false else ix_dned l r
let xgen_dged(l, r) = xi_dled(r, l);
//let xgen_logand = xi_and;
//let xgen_logor = xi_or;
//let xgen_query = xi_query;
//let xgen_lognot = xi_not;
//let xgen_not = xi_not;
//let xgen_x_assign(l, r) = xi_valof(Xassign(l, r))




let gen_Xwhile = function
    | (X_false, c)   -> Xskip
    | (g, c)         -> Xwhile(g, c)

let xi_simplif_ msg m1 x = x // This is disabled for now ... old

// Simplify assuming - simple boolean fact restrict API call.
let ix_bsimplif msg assumptions bx =
    let to_fact gb = Enum_p(xb2nn gb) // If this were a conjunction do we need to make it a list of assumptions? Please explain/check.
    simplifyb_ass (new_known_dp (map to_fact assumptions)) bx


let gen_Xfor(i, g, it, body) = gec_Xblock [i; gen_Xwhile(g, gec_Xblock[body; it])]


// Add a (new) constant to an enumeration and return the slightly-munged constant.
// Where the numeric value of the constant is already present add this as an alias and return the first/preferred one.
let enum_add site vv bashint_ q =
    let (strobe, r) = enum_addf site ix_deqd vv None q
    let mOf = function
        | Enum_pv(n, v, (q, econ_token::_, _), grp) -> econ_token
    mOf r


let pwr_of_two_pred = function
    | X_bnum(_, bn, _) ->
        let rec kk (bn:BigInteger) =
            let rem = ref 0I
            let q = BigInteger.DivRem(bn, 2I, rem)
            if !rem = 0I && q = 1I then true
            elif q <> 0I then false else kk (!rem)
        kk bn
    | X_num k ->
        let rec sk k = if k=1 then true elif k%2 = 0 then sk(k/2) else false
        sk k
    | z -> sf ("pwr_of_two_pred " + xToStr z)
    

let sign_extend from_w to_w arg =
    if arg = X_undef then X_undef
    else
    let rec power b n = if n<=1 then b else b * power b (n-1)
    if from_w < to_w then
        let s = xi_bitsel(arg, from_w - 1)
        let top = xi_bnum((power 2I (to_w-from_w) - 1I) *  power 2I from_w)
        //let prec = { widtho=Some to_w; signed=Signed; }
        ix_query s (ix_diop { g_default_prec with widtho=Some to_w; } V_bitor top arg) arg
    else arg

let meo_options =
        [
            Arg_enum_defaulting("meo-sorting1", ["disable"; "enable"; ], "disable", "Store muxs in canonical form 1");
            Arg_enum_defaulting("meo-sorting2", ["disable"; "enable"; ], "disable", "Store muxs in canonical form 2");

        ]

//let _ = install_operator ("dummy-meoxbase",  "Memoising heap infrastructure", opath_identity, [], [], prop_argpattern);

// Non-associates flattening node store.
// EG: We cannot compute (a * b * c * d) with diadic operators and we do not want a * (b * (c * d)) either since it take 50% longer than (a * b) * (c * d).
let xi_node_nonassoc = xint_write_node

let lc_atoi32x arg = if arg=X_undef then -1 else lc_atoi32 arg


// Exact divide of a boolean expression by a boolean term.  Returns None if the division has a remainder or the denominator is a disjunction.
// Boolean divide :  Give a quotient which when ANDED with the divisor gives the dividend: e.g. (a.b.d + b.!c.d)/(b.d) = a + !c
// TODO: Compare with simplify_assuming ...
let ix_bdivide num den =
    match (num, den) with
        | (W_cover(nn, _), X_true) -> Some num // Denominator identity for boolean division is true.
        | (W_cover(nn, _), W_cover([dd], _)) ->
            match bool_exact_divide_cover nn dd with
                | None -> None
                | Some ans -> Some(xi_cover ans)

        | (W_cover(nn, _), den) ->
            match bool_exact_divide_cover nn [(xb2nn den)] with
                | None -> None
                | Some ans -> Some(xi_cover ans)

        | (num, den) ->
            if xb2nn num = xb2nn den then Some X_true else None
        
// Approximate cost of an operator in arbitrary units... 
// Array reads above a threshold and ALU results are likely to be registered and so these have a different cost in terms of critical-path delay.
// We must avoid counting shared subexpressions twice in a total
let g_default_op_area_cost = 20  // Rough guess of gate area of average width integer addition

let logic_cost_walk_set_gen ww vd title report_areaf =
    let vdp = false
    let msg = title
    let dyn_prog_cache = new Dictionary<xidx_t, walker_nodedata_t>()
    
    let sSS =
        let null_unitfn arg =
            //vprintln // 0 ("Unitfn " + xToStr arg)
            ()

        let null_sonchange _ _ nn (a,b) =
            //vprintln //0 ("Null_sonchange " + xToStr a + " -> " + qToStr b)
            b

        let null_b_sonchange _ _ nn (a,b) =
            //vprintln //0 ("Null_b_sonchange " + xbToStr a)
            b

        let lfun (pp, g) clkinfo lhs rhs =
            //let m1 = ("lfun " + title + " lhs=" + xToStr lhs + " g=" + xbToStr g + " rhs=" + xToStr rhs)
            let _ = 
              match lhs with
                | W_asubsc(X_bnet f, subsc, _) -> ()
                                        
                | W_asubsc(l, ss, _) -> ()

                | X_bnet f -> ()
                    
                | other -> ()
                //sf ("acoster time anal lhs other " + xkey lhs + " " + xToStr lhs)
            ()

        // logic cost: Operator node code function:
        let opfun arg n bo xo _ soninfo2 =
            let say() = if not_nonep bo then xbToStr(valOf bo) elif not_nonep xo then xToStr(valOf xo) else "-sayblank-"
            //vprintln 0 ("Opfun " + s() + " acting on " +  sfold qToStr soninfo2)
            let nn = abs n
            let (found, ov) = dyn_prog_cache.TryGetValue nn
            if found then ov
            else
                let (depth_cost, breadth, area) = // Cost is a 2-D matter, correspondingly broadly to delay and area.
                    match xo with
                        | Some(W_apply((f, gis), _, args, _)) ->
                            // For maxa, we need to check in gis which args project combinational delays into the arguments. TODO
                            let dc = 
                               match gis.rv with
                                   | prec when not_nonep prec.widtho -> valOf prec.widtho
                                   | _ -> 1
                            (gis.cost_per_bit_of_result, dc, gis.area_estimate) // 


                        | Some(W_asubsc(l, r, _)) ->
                            let w = valOf_or (ewidth msg l) 32
                            let area = 4L * int64 w
                            (2, w, area) // Register file logic cost - vague answer. Will not be encountered very much post restructure.
                        | Some(W_query(g, tt, ff, _)) ->
                            let dc = 
                               match mine_prec g_bounda tt with
                                   | prec when not_nonep prec.widtho -> valOf prec.widtho
                                   | _ -> 1
                            let area = 4L * int64 dc
                            (3, dc, area)

                        | Some(X_pair _) | Some(X_bnet _) | Some(X_net _) -> (0, 0, 0L)

                        | Some(W_node(prec, oo, args, _)) when nonep prec.widtho ->
                            dev_println (sprintf "opfun: poor coding alert: no width in W_node for nn=%i %s" nn (say()))
                            (1, g_default_op_area_cost, int64 g_default_op_area_cost)

                        | Some(W_node(prec, oo, args, _)) when not_nonep prec.widtho ->
                            let arity = length args
                            let bits = valOf prec.widtho
                            let dc =
                                match oo with
                                    | V_cast _ | V_interm -> 1 // or zero
                                    | V_bitand | V_bitor | V_xor -> (arity-1)
                                    | V_plus | V_minus -> (arity - 1) * 4  + (bits/8) // Use 4 gates per bit lane - second term is a fast-carry approximation.
                                    | V_times -> bits * 2
                                    | V_mod  | V_exp | V_divide -> bits * 30  // These are quadratic and expensive unless one arg is a constant (we should check that here perhaps).

                                    | V_lshift | V_rshift _ -> 1

                                    // Monadic forms now follow
                                    | V_onesc ->1 // mondadic one's complement: flip all bits.
                                    | V_neg ->  2  // mondadic two's complement: negate.
                            let dc = if prec.signed=FloatingPoint then dc*4 else dc // Of course floating point is normally an instantiated ALU that will have its own accurate area metainfo.
                            let area = 4L * int64 dc // Arbitrary scalar multiple for now - no attempt has yet been made to calibrate area.
                            (bits, dc, area)

                        | Some other when constantp other -> (0, 0, 0L)

                        | Some X_undef ->  (0, 0, 0L)

                        | Some other ->
                            dev_println (sprintf "Cost walk ignored for key=%s vale=%s" (xkey other) (netToStr other))
                            (0, 0, 0L)
                        
                        | None ->
                            match bo with
                                | None ->
                                    let _ = // Why this is invoked with nn=0 (X_undef) I am not sure, but it happens a lot and is of no interest.
                                        if nn <> 0 then dev_println (sprintf "opfun: poor coding alert: Both xo and bo are unset! nn=%i %s" nn (say()))
                                    (1, 0, 0L)

                                | Some(W_cover(cubes, _)) ->
                                    let tally cc lst = length lst + cc
                                    let area = length(list_once (map abs (list_flatten cubes)))
                                    let d = if length cubes = 1 || conjunctionate (fun x -> length x = 1) cubes then area-1 else area
                                    (d, area, int64 area)
                                | Some(W_bdiop(oo, arg0::_, _, _)) ->
                                    let prec = mine_prec g_bounda arg0
                                    let w = if nonep prec.widtho then 1 else valOf prec.widtho
                                    let area = 4L * int64 w
                                    match oo with
                                        | V_dltd _ | V_dled _ 
                                        | V_dgtd _ | V_dged _ -> (1, 4 + w/8, area)
                                        | _ -> (1, w/8, area)

                                | Some(W_bnode(oo, arg0::_, _, _)) -> (1, 1, 1L) // A single AND/OR/XOR gate.


                                | Some other when bconstantp other -> (0, 0, 0L)

                                | Some xb ->
                                    dev_println (sprintf "opfun: No cost for bool %s " (xbToStr xb) + say())
                                    (1, 1, 1L)
                //dev_println (sprintf " opfun nn=%i cost=%i area=%i" nn cost area)

                let sonscan cc = function 
                    | ACOST(v) -> costvec_parallel_combine v cc
                    | _ -> cc
                let child_cost = (List.fold sonscan g_costvec_zero soninfo2)
                let new_inventory_item =
                    if report_areaf then
                        let operator_area = area
                        Some(set [ (abs nn, operator_area)  ])
                    else None
                let nc = costvec_sequential_add child_cost (int64 depth_cost, int64 depth_cost * int64 breadth, new_inventory_item)
                //vprintln 3 ("sons=" + sfold qToStr soninfo2 + "\nArgs ready for " + s() + sprintf "  %i " args_rdy)
                //vprintln 3 ("Timed " + s() + sprintf " result rdy %i " output_rdy)
                let rr = ACOST nc 
                dyn_prog_cache.Add(nn, rr)
                rr
        let (_, sSS) = new_walker vd vdp (true, opfun, (fun _ -> ()), lfun, null_unitfn, null_b_sonchange, null_sonchange)
        sSS
    sSS

let directorate_combine d1 d2 =
    { d1 with
         clk_enables= list_once (d1.clk_enables @ d2.clk_enables)
         clocks=      list_once (d1.clocks @ d2.clocks)      
         resets=      list_once (d1.resets @ d2.resets)      
    }

let directorate_resolve d1 d2 =
    if d1.duid = g_null_directorate.duid then
        if d2.duid <> g_null_directorate.duid then d2
        else
            //hpr_yikes(sprintf "dir_resolve: Both directors unset")
            d2
    else 
        if d2.duid = g_null_directorate.duid then d1
        else
            vprintln 2 (sprintf "dir_resolve: Both directors active: using d2")
            directorate_combine d1 d2


let isarray = function
    | X_bnet ff -> ff.is_array
    | (_) -> false


//
// Form the equality comparison predicate (==), converting to or_reduce (|x|) when operands are testing a boolean equality
//
let gup_ix_deqd a b =
    let is_bool = function
        | X_bnet ff when ff.width = 1 -> true
        | X_blift _ -> true
        | _         -> false

    let boolpred constval arg =
        if xi_iszero constval then xi_not(xi_orred arg)
        else xi_orred arg
    let term = 
        if is_bool a && constantp b then boolpred b a 
        elif is_bool b && constantp a then boolpred a b 
        else ix_deqd a b
    term



(* eof *)
