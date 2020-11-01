//
// Kiwi Scientific Acceleration: KiwiC compiler.
//
// kiwic.fs - KiwiC compiler: Recipe stage that converts CIL to HPR-L/S.
//
// All rights reserved. (C) 2007-17, DJ Greaves, University of Cambridge, Computer Laboratory.
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
//
//
//
//
module kiwic

open System.IO

open yout
open moscow
open opath_hdr
open opath
open asmout
open restructure
open abstracte
open dpmap
open abstract_hdr


open cecilfe
open cilgram
open firstpass
open intcil
open cilnorm


open meox


//let _ = g_litp_tagging := true

let kargpattern stagename =   // stagename is normally "kiwife"
        [
          // We will want to turn on autodispose=enable by default in the near future when further escape analysis is completed.
          Arg_enum_defaulting("kiwic-autodispose", ["enable"; "disable"; ], "disable", "Automatically invoke Kiwi.Dispose() where helpful.")


          
          Arg_required("srcfile", -1, "Input file (dll/exe) for KiwiC conversion", "")
          Arg_defaulting("root", "$attributeroot", "Root class(es) and entry points for h/w generation")
          Arg_int_defaulting(stagename + "-overloads-loglevel", 3, "Verbosity level for typechecking of method overloads")
          Arg_int_defaulting(stagename + "-firstpass-loglevel", 3, "Verbosity level for initial first passes of CIL methods")
          Arg_int_defaulting(stagename + "-hgen-loglevel", 3, "Verbosity level for final code (hbev_t) generation (higher is more verbose)")
          Arg_int_defaulting(stagename + "-constvol-loglevel", 3, "Verbosity level for inter-thread volatile/constant analysis (higher is more verbose)") 
          Arg_int_defaulting(stagename + "-gtrace-loglevel", 3, "Verbosity level for gtrace code generation passes of CIL methods (higher is more verbose)")
          Arg_int_defaulting(stagename + "-ataken-loglevel", 3, "Verbosity level for address taken (higher is more verbose)")
          Arg_int_defaulting(stagename + "-cil-loglevel", 3, "Verbosity level for PE/CIL reader (higher is more verbose)")
          Arg_int_defaulting(stagename + "-filesearch-loglevel", 3, "Verbosity level for looking up files on search paths (higher is more verbose)")          


          Arg_enum_defaulting(stagename + "-allow-hpr-alloc", ["enable"; "disable"; ], "enable", "Enable generted code to allocate run-time heap space using hpr_alloc and the heapmanager FU.")
          Arg_enum_defaulting("kiwife-postgen-optimise", ["enable"; "disable"; ], "enable", "Post-generation optimisations")
          Arg_enum_defaulting(stagename + "-directorate-style", ["minimal"; "basic"; "normal"; "advanced" ], "normal", "Complexity of start/stop/singlestep/abend interface")       

          // kiwic prefix is deprecated for kiwife which is the value of stagename
          Arg_enum_defaulting(stagename + "fpgaconsole-default", ["enable"; "disable"; ], "enable", "Make Console.WriteLine emit on the FPGA hardware console by default.") // not implemented?

          Arg_enum_defaulting("kiwic-zerolength-arrays", ["enable"; "disable"; ], "disable", "Stop on error if an array has zero length")

          Arg_defaulting("kiwic-dll", "Kiwic.dll", "KiwiC (previously canned) class definitions dll name or full path")
          Arg_defaulting("kiwi-dll", "Kiwi.dll", "Kiwi runtime system dll name or full path");          

          Arg_defaulting("array-2d-name", "KIWIARRAY2D", "Name of class to use for 2d arrays (you can divert this to your own class) (canned name is KIWIARRAY2D)")
          Arg_defaulting("array-3d-name", "KIWIARRAY3D", "Name of class to use for 3d arrays (you can divert this to your own class)")
          Arg_defaulting("array-4d-name", "KIWIARRAY4D", "Name of class to use for 4d arrays (you can divert this to your own class)")

          Arg_enum_defaulting("kiwic-register-colours", ["disable"; "1"; "2"], "disable", "Aggressiveness of register allocation affinity") // Colouring still OFF - will be turned on by default soon

          Arg_enum_defaulting("kiwic-library-redirects", ["enable"; "disable" ], "enable", "Automatically redirect library substitutions to Kiwi versions")
          // Temporary kludge flag to workaround a bug
          Arg_enum_defaulting(stagename + "-dynpoly", ["enable"; "disable" ], "enable", "Enable dynamic method dispatch.")
          Arg_enum_defaulting("kiwic-supress-zero-inits", ["enable"; "disable" ], "disable", "Code to clear nets initially to zero is typically not needed for FPGA.")
          Arg_enum_defaulting("kiwic-kcode-dump", ["enable"; "disable"; "early" ], "disable", "Write intermediate kcode to a dump file. Early means pre-constant fold.")
          Arg_enum_defaulting("kiwic-cil-dump", ["separately"; "combined"; "disable"; "early"; "late" ], "disable", "Write disassembly of CIL/PE code to separate dump files or a combined large file.");          
          //Arg_enum_defaulting("kiwic-finish", ["enable"; "disable"], "enable", "Include simulator (RTL/SystemC) exit using $finish on return from main thread")
          Arg_int_defaulting("cil-uwind-budget", 10000, "Number of attempted steps in loop/recursion unwinding (structural generate phase)")
          Arg_defaulting("kiwic-default-dynamic-heapalloc-bytes", "1073741824", "Bondout dynamic heap (per logical bank) default size")  
        ]





//
// opath_kiwi : read a CIL assembly and then convert it to one or more HPR virtual machines (one per thread).
//
//   
let opath_kiwi_vm ww (op_args:op_invoker_t) sidekick_vms =
    let stagename = op_args.stagename
    let names = rev(control_get_strings op_args.c3 "srcfile") // Reverse since returned in wrong order 
    if nullp names then cleanexit "No CIL/PE exe/dll files listed for compilation by KiwiC"

    let composite_root = control_get_s stagename op_args.c3 "root" None

    g_kiwi_autodispose_enabled := control_get_s stagename op_args.c3 "kiwic-autodispose" None = "enable"
    firstpass.g_unif_loglevel  := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-overloads-loglevel") -1)
    g_firstpass_vd             := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-firstpass-loglevel") -1)
    //g_intcil_vd              := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-intcil-loglevel")    -1)
    let hgen_loglevel           = max !g_global_vd ( control_get_i op_args.c3 (stagename + "-hgen-loglevel")      -1)
    let constvol_loglevel       = max !g_global_vd ( control_get_i op_args.c3 (stagename + "-constvol-loglevel")      -1)     
    g_gtrace_vd                := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-gtrace-loglevel")    -1)
    kcode.g_ataken_vd          := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-ataken-loglevel")    -1)
    cecilfe.g_fevd             := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-cil-loglevel")       -1)
    kcode.g_filesearch_vd      := max !g_global_vd ( control_get_i op_args.c3 (stagename + "-filesearch-loglevel")    3)    


    let libnames =
       let shan_library cc id = 
           // g_ip_incdir // we should perhaps look in here in future?
           let tok = control_get_s stagename op_args.c3 id None
           kcode.support_lib_dir cc tok
       List.fold shan_library [] [ "kiwic-dll"; "kiwi-dll"; ]

    let kfec1:kfec_settings_1_t = 
     {
       d2name=           control_get_s stagename op_args.c3 "array-2d-name" None
       d3name=           control_get_s stagename op_args.c3 "array-3d-name" None
       d4name=           control_get_s stagename op_args.c3 "array-4d-name" None
       reg_colouring=    control_get_s stagename op_args.c3 "kiwic-register-colours" None
       cil_unwind_limit= control_get_i op_args.c3 "cil-uwind-budget" 10000
       default_dynamic_heapalloc_bytes = atoi64(control_get_s stagename op_args.c3 "kiwic-default-dynamic-heapalloc-bytes" (Some "1073741824"))
       multid_names= []
     }     

    let (pattern_, checker) = install_directorate_parsing_option_defaults ww stagename
    let directorate_attributes = checker ww op_args.c3
    vprintln 3 (sprintf "making kickoff directorate checker L5196  %A" directorate_attributes)

    let reset_mode =
        let shortname = "vnl" // Should not really be vnl since applies to SystemC etc..
        control_get_s stagename op_args.c3 (shortname + "-resets") (Some "none")

    let kfec2:kcode.kfec_settings_2_t = // There is no need for separate kfec1 and kfec2 really.
        {
            stagename=                  stagename
            reset_mode=                 reset_mode

            early_cil_dumpf=            control_get_s stagename op_args.c3 "kiwic-cil-dump" None = "early" // These write to log file not spool file.
            late_cil_dumpf=             control_get_s stagename op_args.c3 "kiwic-cil-dump" None = "late"
            // Note the combined and separately options are processed in kiwic.fs
            cil_dump_combined=          control_get_s stagename op_args.c3 "kiwic-cil-dump" None = "combined"
            cil_dump_separately=        control_get_s stagename op_args.c3 "kiwic-cil-dump" None = "separately"    

            supress_zero_struct_inits=  control_get_s stagename op_args.c3 "kiwic-supress-zero-inits" None = "enable"
            constvol_loglevel=          constvol_loglevel
            hgen_loglevel=              hgen_loglevel
            //finish_not_hang= finish_not_hang
            kcode_dump=
                match control_get_s stagename op_args.c3 "kiwic-kcode-dump" None with
                    | "enable"  -> 1
                    | "early"   -> 2
                    | _         -> 0

            dynpoly=                    control_get_s stagename op_args.c3 (stagename + "-dynpoly") None <> "disable"
            zerolength_arrays=          control_get_s stagename op_args.c3 "kiwic-zerolength-arrays" None = "enable"

            postgen_optimise=           0=cmdline_flagset_validate "kiwife-postgen-optimise" ["enable"; "disable" ] 0 op_args.c3
            directorate_style=          parse_directorate_style ww (control_get_s stagename op_args.c3 (stagename + "-directorate-style") None)
            directorate_attributes=     directorate_attributes
        }

    // We need the names of multidimensional names for special handling of get_Length, Address, Rank and so on.
    let kfec1 = { kfec1 with multid_names= map (fun x-> x + "`1") [kfec1.d2name;kfec1.d3name;kfec1.d4name]; }
     
    let islib x =
        let s1 l = strlen l <= strlen x && x.[(strlen x)-(strlen l)-1 .. strlen(x)-1] = l
        disjunctionate s1 libnames

    let names = List.filter (fun x -> not (islib x)) names
    let readfun = kiwic_main_factory kfec1 kfec2 // There is seemingly no reason for this to be higher-order.

    let all_file_names =
        let divide_fname fn =
            let (root, suf) = strip_suffix fn
            (fn, root, suf)
        map divide_fname (libnames @ list_subtract (names, libnames))

    let topname =
        let rec find_a_topname = function
            | [] ->
                vprintln 1 ("no cmdline exe file used as basis for topname in " + sfold (fun x->x) names)
                "anontop"
            | (x, root, "exe")::_ -> root
            | _::tt               -> find_a_topname tt
        find_a_topname all_file_names
        
    vprintln 3 ("Topname=" + topname)


    // TODO not yet used. ast.cil can be written out
    //let printtree = control_get_s stagename op_args.c3 "kiwi-print-parsetree" None <> "disable"
    //let alltrees  = control_get_s stagename op_args.c3 "kiwi-print-parsetree" None = "all"


    // As well as finding IP-XACT files on the ip-incdir path we should also allow them to be explict on the command line here, pari-passu with .net dll and exe files.

    let all_file_names2 = map (fun ((name:string, root, suf), nn)->(nn, name, rev (Array.toList(name.Split("\\/".ToCharArray()))), root, suf)) (zipWithIndex all_file_names)

    let _ =
        // TODO: could get some sort of file digest crypto sum to be surer over this.
        let give_list vd = reportx vd "All PE names (.dll + .exe)" (fun (n, xname, hl, root, suf) -> sprintf " %i:  %s suf=%s" n xname suf) all_file_names2
        let tails = map (fun (nn, name, hl, root, suf) -> (hd hl, nn)) all_file_names2
        let rec scan_for_dups = function
            | [] -> None 
            | (n, x, idl, root_, suf_) :: tt ->
                match op_assoc (hd idl) tails with
                    | Some n1 when n1 <> n -> Some (n1, n, x)
                    | _ -> scan_for_dups tt
        match scan_for_dups all_file_names2 with 

        | None -> give_list 3
        | Some (n1, n, dup) ->
            hpr_warn(sprintf "+++ the following PE file is seemingly supplied twice %i cf %i : %s " n1 n dup)
            give_list 0
            ()


    // control_get_s "KiwiC" op_args.c3 "kiwic-cil-dump" None = "early" is the default

    let asts =
        List.foldBack (kcode.read_dotnetfile_and_dump ww kfec2) all_file_names []
    
    if kfec2.cil_dump_combined then kcode.cil_writeout ww (list_flatten (map f3o3 asts)) (path_combine(!g_log_dir, "ast.cil"))


    // Readfun is called once : it takes a number of asts and returns one vm2 that may be just a wrapper around multiple threads and server entry points.
    let (vm2, _) = readfun (WN "KiwiC front end" ww) ([], [topname], composite_root, op_args.c3) asts
    let lastroot =
        let scan_best_name (best_name, best_pri) (nn_, name, idl, root, suf) =
            if suf = "exe" then (root, 3)
            elif suf = "dll" && best_pri < 3 then (root, 2) 
            elif best_pri < 2 then (root, 2) 
            else (best_name, best_pri)            
        let (name, pri_) = List.fold scan_best_name ("anonymous-name", -1) all_file_names2
        [name] // Somewhat longwinded

    let bname = hd lastroot // This will be the innermost identifier owing to reversed representation.
    vprintln 1 (sprintf "The selected best default name for the compilation is '%s' from '%s'" bname (hptos lastroot))

    let ii = { rez_kiwi_ii (bname) [bname] false with
                  vlnv=          (hpr_minfo vm2).name
                  definitionf=   true
                  generated_by=  stagename
             }

    (ii, Some vm2) :: sidekick_vms


let kiwic_used () =
    let ww = WW_end
    let stagename = "kiwife"
    let (pattern, checker_) = install_directorate_parsing_option_defaults ww stagename
    in
     (
        install_operator (stagename,   "Kiwi Front End", opath_kiwi_vm, [], [], kargpattern stagename @ pattern @ protocols.g_bondout_arg_patterns)    
     )

// Rather than listing all these modules here, we should use auto plugin scanning - this crashed on mono the last time I tried.
open conerefine
open repack
open bevelab
open verilog_gen
open cpp_render
open diosim
open systolic


                    
// The example displays the following output:
//   Public types in assembly sysglobl, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a:
//      System.Globalization.CultureAndRegionInfoBuilder
//      System.Globalization.CultureAndRegionModifiers
[<EntryPoint>]
let main (argv : string array)  =
    loadPlugins() // This is supposed to autoload most of the below!
    kiwic_used()
    install_abstracte()
    ignore(install_coneref())
    ignore(install_repack())
    ignore(install_systolic())
    install_restructure()    
    bevelab.install_bevelab()
    compose.install_compose()
    install_verilog()
    install_dpath()
    install_diosim()
    install_c_output_modes()
//    run_e_test()
    let rc = opath_main ("kiwic", argv)
    //chain_system_integrator ()

    //system_integrator.system_integrator_used()
    rc

(* eof *)





