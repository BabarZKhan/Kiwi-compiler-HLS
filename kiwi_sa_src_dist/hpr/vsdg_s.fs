// (C) 2012 DJ Greaves.
// $Id: vsdg_s.fs,v 1.4 2012-05-09 20:39:07 djg11 Exp $
//
// Convert to and from value-state data graph.
//   The 'to' route that is first being made to wotk is from the SP_fsm form (output of bevelab) with other library components being invoked to get to this form.
//   The 'from' route will generate XRTL form typically ?
//
//   
// via a spatial computing VSDG_S (as per Ali Zaidi) to RTL.
//
module vsdg_s

let vsdg_s_banner = "d320 $Id: vsdg_s.fs,v 1.4 2012-05-09 20:39:07 djg11 Exp $ "


(*
 *
 * vsdg_s.fs : 
 *
 * (C) 2012 DJ Greaves. SAM Zaidi Cambridge. CBG.
 *
   
This is an alternative to the bevelab plugin - it uses distributed dataflow instead of having a centralised micro-sequencer per thread.
It is based on the paper  `A New Dataflow Compiler IR for Accelerating Control-Intensive Code in Spatial Hardware'   A M Zaidi and DJ Greaves. At IPDPS'14.    It can achieve greater throughput with heavily pipelined components in the presence of complex control flow compared with traditional loop unwinding and static schedulling.

 *)



open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open linepoint_hdr
open dpmap // for now
open Microsoft.FSharp.Collections





type S1_t =
    {
          dummy:bool;
    }


// TODO: this graph representation should become one of the SP_forms in the VM as yet another way of representing an exec.

// Value edge    
type val_edge_t = hexp_t * hbexp_t * hbexp_t  // value + valid tag (vv, vt)

// State edge
type state_edge_t = hbexp_t * hbexp_t  // value + valid tag (vv, vt)

type idx_t = string

// Dataflow operators and load/stores.
type vsdg_opnode_t =
    | OP_load  of val_edge_t * hexp_t * hexp_t option // optional subscript for 1-D vectors
    | OP_store of val_edge_t * hexp_t * hexp_t option // optional subscript
    | OP_alu of val_edge_t * (precision_t * x_diop_t) * val_edge_t * val_edge_t 

// State edge joins
type se_join_t = state_edge_t * state_edge_t * state_edge_t  // A two-way join in the state edge graph

// Value nodes
type vsdg_node_t =
    | SE_end   of idx_t
    | SE_cond  of idx_t * state_edge_t * val_edge_t * state_edge_t * state_edge_t * se_join_t   // Conditional fork/join
    | SE_seq   of idx_t * state_edge_t * state_edge_t(*not really!*)  * state_edge_t  // Sequential
    
type vsdg_s_elab_t = 
      {
          clknet: hexp_t;
          nets: hexp_t list ref;            // Generated nets
          nodes: vsdg_node_t list ref;      // Generated state control nodes
          opnodes: vsdg_opnode_t list ref;  // Generates value nodes (dataflow).
          generated_rtl:  xrtl_t list ref;  // Output RTL is finally written to this list.

          endmark: idx_t option ref;
      }

// Return a new temporary guarded register (i.e a register with a valid tag bit).
let fresh_ve (rv:vsdg_s_elab_t) (width) =
    let tmp = funique "tmp"
    let rdy = vectornet_w(tmp + "RDY", 1)
    let ack = vectornet_w(tmp + "ACK", 1)
    let v = vectornet_w(tmp + "V", width)    
    let _ = mutadd rv.nets rdy
    let _ = mutadd rv.nets ack
    let _ = mutadd rv.nets v    
    (v, xi_orred rdy, xi_orred ack)

let fresh_se (rv:vsdg_s_elab_t) (width) =
    let key = funique "key"
    let rdy = vectornet_w(key + "RDY", 1)
    let ack = vectornet_w(key + "ACK", 1)
    let _ = mutadd rv.nets rdy
    let _ = mutadd rv.nets ack
    (xi_orred rdy, xi_orred ack)


let emit_se (rv:vsdg_s_elab_t) x = mutadd rv.nodes x

let emit_op (rv:vsdg_s_elab_t) x = mutadd rv.opnodes x


let ve2se (vale, rdy, ack) = (rdy, ack) // Ideally returned a branded type here.
    
let rec gen_vsdg_exp (rv:vsdg_s_elab_t) se arg =
    match arg with
        | X_bnet f ->  // SCALAR LOAD (reg or mem)
            let tmp = fresh_ve rv (encoding_width arg)
            let key = funique "scalar_load"
            let se_x = fresh_se rv key            
            let _ = emit_op rv (OP_load(tmp, arg, None))
            let _ = emit_se rv (SE_seq(key, se, ve2se tmp, se_x))      
            (tmp, se_x) 
        | W_node(prec, oo, [l;r], _) ->  // diadic operators (strict)
            let (l', se') = gen_vsdg_exp rv se l
            let (r', se'') = gen_vsdg_exp rv se' r
            let tmp = fresh_ve rv (encoding_width arg)
            let _ = emit_op rv (OP_alu(tmp, (prec, oo), l', r'))
            (tmp, se'') // xi_diop(oo, l', r'), ix_and lg rg, se'')

let rec gen_vsdg_cmd (rv:vsdg_s_elab_t) se cmd =
    match cmd with
        | Xassign(l, r) -> // Scalar STORE (reg or mem)
            let (r', se') = gen_vsdg_exp rv se r
            let _ = emit_op rv (OP_store(r', l, None))
            let key = funique "scalar_store"
            let se_x = fresh_se rv (key)            
            let _ = emit_se rv (SE_seq(key, se', ve2se r', se_x))
            in se_x
            
        | Xblock lst -> // Sequential compostion of commands
            let rec seqlst se = function
                | [] -> se
                | h::t ->
                    let se' = gen_vsdg_cmd rv se h
                    in seqlst se' t
            let se' = seqlst se lst
            se'

        | Xif(g, tt, ff) -> // Conditional execution
            let (grd, se') = gen_vsdg_exp rv se (xi_blift g) // for now
            let key = funique "IF-"
            let se_t = fresh_se rv (key + "T")
            let se_f = fresh_se rv (key + "F")
            let se_x = fresh_se rv (key + "X")            
            let se_tx = gen_vsdg_cmd rv se_t tt
            let se_fx = gen_vsdg_cmd rv se_f ff
            let _ = emit_se rv (SE_cond(key + "J", se', grd,  se_t, se_f, (se_tx, se_fx, se_x)))
            se_x
            
// Generate 2-input OR (gate or RTL) with integer casting on in and out nets.
let ii_or a b = xi_blift(ix_or (xi_orred a) (xi_orred b))

// Generate 2-input AND (gate or RTL) with integer casting on in and out nets.
let ii_and a b = xi_blift(ix_and (xi_orred a) (xi_orred b))

// Generate an arbitrary fan-in AND gate.
let ii_andl lst = xi_blift(ix_andl (map xi_orred lst))

// Generate an inverter (gate or RTL) with integer casting on in and out nets.
// ii constructors accept boolean arguments instead of integer or vice versa as compared to ix or xi forms.
// The difference between xi and ix forms is currying, which makes not difference for the mondaic xi_not.
let ii_not a = xi_blift(xi_not (xi_orred a))

// Create a register assign (D-type/broadside with input mux) with clock enable
let gen_DFF (ce, q, d) = gen_XRTL_arc(None, xi_orred ce, q, d)
let ii_DFF (ce, q, d) = gen_XRTL_arc(None, ce, xi_blift q, xi_blift d)

// Create a buffer to drive l from r.
let gen_BUF (l, r) = gen_XRTL_buf(X_true, l, r)
let ii_BUF (l,r) = gen_BUF(xi_blift l, xi_blift r)

// RTL implementation of a JK flipflop used as the basic synchronous stage.
let gen_JKUNIT((req, rdy), (req_out, rdy_out)) =
    let set = ix_and req rdy
    let clr = ix_and req_out rdy_out
    let arc = ii_DFF(ix_or set clr, req_out, ix_or set (ix_and (xi_not clr) req_out))
    let buf = ii_BUF(rdy, xi_not req_out)
    in [arc; buf]


let emit (rv:vsdg_s_elab_t) rtl_items = rv.generated_rtl := rtl_items @ !rv.generated_rtl


#if NON_VSDG

// RTL implementation: If unit: steers a request one of two ways according to a guard
let gen_IFUNIT(req1, g1, rdy1, req_tt, rdy_tt, req_ff, rdy_ff) =
    let g1 = gen_BUF (req_tt, ii_andl [ rdy1; req1; g; ])
    let g2 = gen_BUF (req_tt, ii_andl [ rdy1; req1; ii_not g; ])

    let r1 = gen_BUF (rdy1, ii_or (ii_and rdy_tt req_tt) (ii_and rdy_ff req_ff))
    [g1; g2; r1]

// RTL implementation of a Joiner unit, placed when two flows of control join (i.e. not VSDG style)
let gen_JOINUNIT(req1, rdy1, req2, rdy2_o, req_out, rdy') =

    let g1 = gen_BUF (req_tt, ii_or rdy1 rdy2)
//    let
    [ g1 ]




// Straightforward, control-graph directed spatial compilation function: does not have VSDG parallelism.
let rec non_vsdg_compile_cmd rv req rdy_out = function
    | C_skip ->
        let (_, rdy) = fresh rv (1)
        let (_, req_out) = fresh rv (1)
        let _ = emit rv (gen_JKUNIT(req, rdy, req_out, rdy_out))
        in (req_out, rdy)

    | C_if(g, tt, ff) ->
        let (_, rdy1) = fresh rv (1)
        let (g2, req2, rdy) = vsdg_compile_exp rv req rdy1 (xi_blift g)
        
        let (req_tt', req_ff') = (fresh rv (1), fresh rv (1))
        let (rdy_tt, rdy_ff) = (fresh rv (1), fresh rv (1))        

        let (req_tt', rdy_tt) = vsdg_compile_cmd rv req_tt rdy_tt' tt
        let (req_ff', rdy_ff) = vsdg_compile_cmd rv req_ff rdy_ff' ff
        let _ = emit rv (gen_IFUNIT(req1, g1, rdy1, req_tt, rdy_tt, req_ff, rdy_ff))
             
        let req_out = fresh rv (1)

        let _ = emit rv (gen_JOINUNIT(req_tt', rdy_tt', req_ff', Some rdy_ff', req_out, rdy'))
        (req_out, rdy)
#endif


let op_compile (rv:vsdg_s_elab_t) g0 op =
    match op with
        | OP_store((vv,vg,vack), l, None) ->
            let g1 = ix_and g0 vg // need rendezvous
            let _ = emit rv [gen_BUF(xi_blift vack, xi_num 1); gen_DFF(xi_blift g1, l, vv)]
            g0
            
// State-edge directed compiler
let rec se_compile (rv:vsdg_s_elab_t) g0 se =
    let mux g t f = ix_or (ix_and g t) (ix_and (xi_not g) f)
    match se with
        | SE_end idx ->
            let _ = cassert (!rv.endmark=None, "endmark not yet set")
            let _ = rv.endmark := Some idx //emit rv (gen_BUF(rv.endmark, idx))
            idx // need to emit

        | SE_cond(idx, se_enter, grd, se_t, se_f, join) ->
            let gen_IFUNIT((srdy, sack), (gval, grdy, gack), (trdy, tack), (frdy, fack)) =
                let gv = xi_orred(gval)
                let go = ix_and srdy grdy
                [

                  ii_BUF (trdy, ix_and go gv);
                  ii_BUF (frdy, ix_and go (xi_not gv));

                  ii_BUF (gack, ix_and go (mux gv tack fack));
                  ii_BUF (sack, ix_and go (mux gv tack fack));

                ]


            let gen_JOINUNIT((trdy, tack), (frdy, fack), (xrdy, xack)) =
                [ ii_BUF (xrdy, ix_or trdy frdy);
                  ii_BUF (tack, xack);
                  ii_BUF (fack, xack);
                ]
            let _ = emit rv (gen_IFUNIT(se_enter, grd, se_t, se_f))
            let _ = emit rv (gen_JOINUNIT join)

            idx
            

        | SE_seq(idx, se_in, op, se_x) ->
            let gen_RENDZUNIT((rdy1, ack1), (rdy2, ack2), (xrdy, xack)) =
                [ ii_BUF (xrdy, ix_and rdy1 rdy2);
                  ii_BUF (ack1, xack);
                  ii_BUF (ack2, xack);
                ]
            let se_mid = fresh_se rv (idx + "REND")
            let _ = emit rv (gen_RENDZUNIT(se_in, op, se_mid))
            let _ = emit rv (gen_JKUNIT(se_mid, se_x))            
            idx
            

let spatial_compile =
    let program = Xblock [ Xskip ]

    let  rv =
        {
          clknet=   g_clknet;
          nets=     ref [];        // Generated nets
          nodes=    ref [];        // Generated state nodes
          opnodes=  ref [];        // Generates dataflow operation nodes
          generated_rtl= ref [];   // Output RTL is finally written to this list.
          endmark=  ref None;
      }

    let se0 = fresh_se rv ("TOPSTART")
    let _ = emit rv [ gen_BUF(xi_blift(fst se0), xi_num 1) ] // Set intitial request input to logic 1.
    let se = gen_vsdg_cmd rv se0 program
    let _ = map (se_compile rv X_true) (!rv.nodes)
    ()

                         
// Convert from SP_fsm form (output of bevelab) via a VSDG_S (as per Ali Zaidi) to RTL.
//
let opath_vsdg_s_core ww0 (S1:S1_t, K1) mc =
    match mc with
    | (ii, None) -> (ii, None)
    | (ii, Some(HPR_VM2(minfo, decls_, sons_, execs_, assertions))) ->
        let phasename = "vsdg-s"
        //let _ = establish_log false "vsdg_s"
        let ww = WF 1 "Vsdg_s:" ww0 "Commence"
        let rec flatten (decs, execs, serts) = function
            | (ii, None) -> (decs, execs, serts)
            | (ii, Some(HPR_VM2(minfo, l, sons, es, ass))) ->
                List.fold flatten (l @ decs, es @ execs, ass @ serts) sons
        let (nets, execs, serts) = flatten ([], [], []) mc
        let ww = WF 1 "Vsdg_s:" ww0 "Flattened"

        let pcs = ref []
        let rec scanfsm0 clk arg (c, d) =
            match arg with
            | SP_fsm(info, a) ->
                mutadd pcs info.pc
                ((clk, arg)::c, d)
            | SP_par(_, lst) -> List.foldBack (scanfsm0 clk) lst (c,d)
            | other ->
                let _ = vprintln 2 ("Not composing an item of type " + hprSPSummaryToStr other)
                (c, H2BLK(clk, other)::d)


        let scanfsm1 arg c =
            match arg with
            | H2BLK(clko, rtl) -> (*List.foldBack*) (scanfsm0 clko) rtl c
            //| _ -> c

        let ((acode, otherexecs), anets1) = (List.foldBack scanfsm1 execs ([],[]), !pcs)

        //let _ = rebal (WN "rebal" ww0) K1 acode anets1


        let newseq = false // S1.scheduler = "parallelnew"

        
        let dir = if acode<>[] then fst(hd acode) else {g_null_directorate with clocks=[E_pos g_clknet] } // WARNING: this uses the first clk for all ... ? // TODO set g_clknet polarity? See previous copy too.

        // Convert back - second stage.
        let code = muddy "VSDG back convert not implemented yet"
        let execs = [H2BLK(dir, code)]


        let clks =
            let cpi = { g_null_db_metainfo with kind= "vsdg-clks" }
            //let _ = reportx 3 "pc regs needed " xToStr pcnets
            [DB_group(cpi, map db_netwrap_null (map de_edge dir.clocks))]
        let ii =
            { ii with
                  generated_by= phasename
              //    vlnv= { ii.vlnv with kind= newname }  // do not do this rename perhaps 
            }
        let ans = (ii, Some(HPR_VM2(minfo, clks@nets, [], execs @otherexecs, assertions)))
        let ww = WF 1 "Vsdg_s:" ww0 " Core Complete"  
        ans 


//
// This is the Orangepath plugin
//
let opath_vsdg_s ww op_args M =
    let c1:control_t = op_args.c3
    let disabled = 1= cmdline_flagset_validate op_args.stagename ["enable"; "disable" ] 0 c1
    let _ = vprintln 1 (vsdg_s_banner)
    if disabled
    then
        let _ = vprintln 1 "Stage is disabled";
        M
    else

        let _ = WF 3 "vsdg_s" ww "Starting"
#if NOT_WRITTEN_YET
        let M' = opath_vsdg_s_core (WN "vsdg_s_core" ww)  (S1, K1) M
#endif
        let _ = WF 3 "vsdg_s" ww "Finished"
        M




let install_vsdg_s() =
    let argpattern =
        [
          Arg_enum_defaulting("vsdg_s", ["enable"; "disable"; ], "enable", "Disable this stage");

        ]

    install_operator ("vsdg_s",  "Behavioural code to FSM Generator - TODO not right?", opath_vsdg_s, [], [], argpattern)
    ()


// eof

