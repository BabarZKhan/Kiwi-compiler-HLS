// $Id: opath_hdr.fs,v 1.8 2011-12-19 20:35:30 djg11 Exp $
//
// HPR L/S
//

module opath_hdr


(*  $Id: opath_hdr.fs,v 1.8 2011-12-19 20:35:30 djg11 Exp $
 * Orangepath: recipe follower.
 *
 * As far as possible, all operations are 'src-to-src' like operations on
 * HPR virtual machines and the operations are stored in a standard opath
 * command format to be executed by an orangepath recipe (program of commands).
 * 
 *
 *)



open moscow
open yout
open abstract_hdr

let g_ip_incdir = ref ["."] // List of folders to search for IP blocks.  // Default to ".", the current directory only.

let cmd_line_assoc_set(args, key, mem) =
    let a = cmd_line_assoc(args, key, false)
    if a<>None then mem := atoi(valOf a)
    () 


let argformats__unused_ = // OLD
   ref [
        ("-root", 2, "top level name for simulation/compilation");
        ("-render-root", 2, "top level name for output rendering");
        ("-version", 2, "give tool version and help string"); 
        ("-verbose", 1, "enable console verbosity"); 
        ("-verbose2", 1, "enable greater console verbosity"); 
        ("-help", 2, "give tool version and help string");


        ("", 0, "Optimisation options:");
        ("-no-roms", 1, "Disable optimisation of unwritten arrays into ROMs");
        ("-no-clear-arrays", 1, "Disable clearing arrays to zero on reset");
        ("-no-cone-refine", 1, "Disable removal of redundant logic");
        ("-no-espresso", 1, "Disable call outs to unix espresso");
        ("-restructure", 2, "Offer a list of identifiers for removal of structural hazards and off chip array memories");


        ("", 0, "Simulation options:");
        ("-sim", 2, "simulate the design"); 
        ("-traces", 2, "net patterns to trace"); 
        ("-opentrace", 2, "opentrace level"); 
        ("-rwtrace", 2, "rwtrace level");
        ("-plot", 2, "plot simulation to a diogif/plot file");
        ("-title", 2, "plot title");
        ("-sim-ucode", 1, "divert: simulate microcode instead of VM");
        ("-bypass-verilog-roundtrip", 1, "divert: simulate gates intead of RTL: IGNORED?");
        ("-sim-rtl", 1, "divert: simulate RTL instead of VM");


        ("", 0, "Synthesis control options:");
        ("-nobuild", 1, "skip all build operations between front end and backend");
        ("-xtor", 2, "generate transactors: -xtor { monitor | initiator | target }");

        ("-ucode", 2, "generate microprocessor code for the design");

        ("-preserve-sequencer", 2, "keep the per-thread vestigal sequencer in output structures: deprecated to becontrol");
        ("-gatelib", 2, "convert RTL to gates using specified lib");


        ("", 0, "Backend options:");
        ("-sysc", 2, "generate SystemC output files");
        ("-smv", 2, "generate SMV encoding of design");
        ("-vnl", 2, "generate Verilog to the following file name: -vnl fn.v");

        ("", 0, "Developer only options:");
        ("-okerror", 2, "Specify an error exit to be treated as a correct exit (for regression)");

        ("", 0, "Front-end options:");  (* Users front-end can mutaddtail to this list *)
        //("-finish", 1, "Exit rather than hang on (main) thread termination")
  ] 


type control_t = string list list

// Orangepath recipe steps.
type opath_op_t = 
  | OPATH_do of bool * string * string * control_t * control_t
  | OPATH_comment of string
  | OPATH_label of string
  //  | OPATH_beq of hbexp_t * string
  | OPATH_dup
  | OPATH_pop
  | OPATH_pushnull


let opath_cmdToStr =
    function
        | (OPATH_do(true,  stagename, cmd, s, _)) -> "DO " + stagename + ":" + cmd
        | (OPATH_do(false, stagename, cmd, s, _)) -> "DO " + stagename + ":" + cmd + " DISABLED (will be skipped)"
        | (OPATH_comment(s)) -> "// " + s
        | (OPATH_label(s)) ->  s + ":"
        |  _ -> "cmdToStr??"

let g_null_mc = null_hpr_machine "*nullmc*"



type vm_t = (vm2_iinfo_t * hpr_machine_t option)


type op_invoker_t =
    {
        banner_msg:        string
        argpattern:        argpattern_t list
        command:           opath_op_t option
        stagename:         string
        c3:                control_t
    }

let g_null_op_invoker =
    {
        banner_msg=        "null_op_invoker"
        argpattern=        []
        command=           None
        stagename=         "null_op_invoker"
        c3=                []
    }

type opath_plugin_ifc_t =

    abstract member banner : unit -> string

    abstract member run : WW_t -> vm_t list -> vm_t list

(*
 * Identity or nop
 *)
let opth_identity ww (opi:op_invoker_t) avm =
      let _ = 0
      // (idl, HPR_VM2(minfo, gdecls, ldecls, sons, execs, assertions))) = avm
      avm

type opath_operator_t = WW_t -> op_invoker_t -> vm_t list -> vm_t list

type opath_op_t1 = string * string * opath_operator_t * control_t * string list * argpattern_t list


let opath_table = ref([]:opath_op_t1 list) // Dictionary of plugins

let install_operator x =
    let (a, b, c, d, e, argpattern) = x
    vprintln 2 ("Installing HPR component " + a)
    mutadd opath_table x

install_operator ("identity", "Nop", opth_identity, [], [], [])
install_operator ("nop",      "Nop", opth_identity, [], [], [])


// eof
