// $Id: opath.fs,v 1.70 2013-08-28 14:19:35 djg11 Exp $

// opath.fs - recipe and command line obedience.
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

module opath

open System.Collections.Generic
open Microsoft.FSharp.Collections
open System
open System.IO
open System.Reflection



(*  $Id: opath.fs,v 1.70 2013-08-28 14:19:35 djg11 Exp $
 * Orangepath: recipe follower.
 *
 * As far as possible, all operations are 'src-to-src' like operations on
 * HPR virtual machines and the operations are stored in a standard opath
 * command format to be executed by an orangepath recipe (program of commands).
 * 
 *
 *)
open meox
open abstract_hdr
open abstracte
open moscow
open yout
open dpmap
open hprxml
open gbuild
open opath_interface
open protocols


open smv_hdr
open cpp_render
open verilog_render
open verilog_gen

open opath_hdr


// Asynchronous monitor of execution progress.
// Need thread-safe output routines.
let rec ww_mon counter =
    System.Threading.Thread.Sleep(5*1000)     // Every 5 seconds.
    let ff ss =
        Console.Out.Write("progress monitor + " + ss)
        ()
    let ss = whereAmI ff
    Console.Out.Write("   progress:" + timestamp true)
    ww_mon(counter+1)

let g_progress_monitor_delegate = async { ww_mon 0 }

let opvd = 2

let cmd_line_assoc_set(args, key, mem) =
    let a = cmd_line_assoc(args, key, false)
    if a<>None then mem := int32(atoi(valOf a))
    () 


install_smv()

//install_operator ("spin",     "Generate Promela/Spin code", opath_smv_vm, [], [], [])


//-------------
                

let g_cfg_plot_each_step = ref false // These do not need to be global or mutable infact.
let g_issue_area_reports = ref false


let g_used_parameter_settings:(string * string) list ref = ref []

let recipe_print vd s = if vd>=4 then vprintln 4 ("  -~~ " + opath_cmdToStr s)



let rec aux_report ww cc = function
    | (ii, None) -> cc
    | (ii, Some(HPR_VM2(minfo, decls, childmachines, execs, assertions))) ->
        let title = vlnvToStr minfo.name
        let cc = List.fold (aux_report ww) cc childmachines
        let richf = None
        let cfg = reporter.hpr_sp_dot_report ww title richf execs
        cfg @ cc

let report_step ww stage step_banner_msg (control, c2, m_fatal_marks) vms =
    let ss = "stage" + i2s stage
    let rr = vmreport_top ww (ss, step_banner_msg, control, c2, m_fatal_marks, !g_issue_area_reports) vms
    if !g_cfg_plot_each_step then
        let dot_reports = List.fold (aux_report ww) [] vms
        //vprintln 0 (sprintf "cfg_plot_each_step: dotcode %A" code)
        let keyname = "cfg_plot_" + ss
        reporter.render_dot_plot ww keyname dot_reports
        ()
    rr
    
let report_cmd =
    function
        | OPATH_do(enabled, _, "report", _, _) -> enabled
        | _ -> false

// Conglomorate args with default args. Here just append: but arg may be an override for the defaults
let compile_control vd (args, defs) =
    let ans = args @ defs
    let aqf x = "'" + x + "'"
    let sr =
        function
            | [] -> "*NIL?"
            | (h::t) -> "  " + h + "=" + sfold aqf t
    if vd>=5 then reportx 5 "Compile Control" sr ans
    ans



(*
 * All of the cmd-line supplied args are added to the flagged array and marked off as converted to list of lists form. 
 * This must be done after all argpatterns have been registered.
 * The length of the sublist is not at first known.
 * Any unflagged args are reported at the end as unused.
 *)
let flagged_args1 = ref []

// Some arguments are processed by opath itself, instead of being passed to any component:

// Monadic:
let opath_early_args0 = [ "-verbose"; "-verbose2"; "-report-each-step"; "-noreport-each-step"; "-give-backtrace"; "meox-enable-sorter1";  "-cfg-plot-each-step"; "-meox-enable-sorter2";  "-issue-shorter-reports";  "-issue-area-reports"; ]

// Diadic:
let opath_early_args1 = [ "-recipe"; "-print-limit"; "-loglevel"; "-verboselevel"; "-log-dir-name"; "-obj-dir-name"; "-devx"; "-ip-incdir"; "-xor-heuristic-value"; "-or-heuristic-value" ]


type arg_t = { plugin:string; arity : int; descr : string ; vales : string list }

// Database of all parameters settable in the recipe.
let argbase = Dictionary<string, arg_t>()

let cmdlinehelp() =
    let aqf x = "'" + x + "'"
    (
    vprintln 0 ("Command line help");
    vprintln 0 ("Early args with no prams include " + sfold (fun x->x) opath_early_args0);
    vprintln 0 ("Early args taking one pram include " + sfold (fun x->x) opath_early_args1);
    for a in System.Environment.GetCommandLineArgs() do
        vprintln 0 (" user arg: " + a.ToString())
    for a in argbase do
        vprintln 0 ("plugin " + a.Value.plugin + " acceptable arg: -" + a.Key + (if a.Value.vales=[] then  (sprintf " (length=%i)" a.Value.arity) else (" options=" + sfold aqf a.Value.vales)) + " " + a.Value.descr)
    vprintln 0 ("... detailed Command line help missing");
    )


let opath_cmd_line_prep () =
    let argv = ref []
    let l_argstring = ref ""
    for a in System.Environment.GetCommandLineArgs() do
        l_argstring := !l_argstring + " " + a
        // The intension is that args are generally of the form -a=b but the equals sign is only used by convention and not enforced yet.
        argv := !argv @ (split_string_at ['='] a)
        done
    //vprintln 0 (sprintf "Argstring is " + !l_argstring)
    //moscow_dummy_fun()
    set_g_argstring  !l_argstring
    map (fun x -> (x, ref false)) ((!argv))

//
//
//
let get_flagged_args program_name =
    let collate_argpattern plugin = function
        | Arg_string_list(id, descr) ->
            let (found, ov) = argbase.TryGetValue(id)
            if found then ()
            else argbase.Add(id, {arity=0; plugin=plugin; descr=descr; vales=[]})
// ned arg optional
               
        | Arg_required(id, len, descr, spare) ->
            let (found, ov) = argbase.TryGetValue(id)
            if found then cassert(ov.arity = len, id + " cmdarg redefined differently")
            else argbase.Add(id, {arity=len; plugin=plugin; descr=descr; vales=[]})

        | Arg_int_defaulting(id, defv, descr) ->
            let (found, ov) = argbase.TryGetValue(id)
            if found then cassert(ov.arity = 1, id + " cmdarg redefined differently")
            else argbase.Add(id, {arity=1; plugin=plugin; descr=descr; vales=[]})

        | Arg_defaulting(id, defv, descr) ->
            let (found, ov) = argbase.TryGetValue(id)
            if found then cassert(ov.arity = 1, id + " cmdarg redefined differently")
            else argbase.Add(id, {arity=1; plugin=plugin; descr=descr; vales=[]})

        | Arg_enum_defaulting(id, setof, defv, descr) ->            
            let (found, ov) = argbase.TryGetValue(id)
            if found then cassert(ov.arity = 1, id + " cmdarg redefined differently")
            else argbase.Add(id, {arity=1; plugin=plugin; descr=descr; vales=setof})

        //| _ -> sf "other collate arg pattern"
    let collate_argpatterns (id, _, opp, c0, _, argpatterns) = app (collate_argpattern id) argpatterns

    // Why does not verbose and so on need a descr?
    // Cant we insert these automatically?
    argbase.Add("print-depth", {arity=1; vales=[]; plugin="opath"; descr="Number of nodes to print in general expressions before emitting ellipsis"})
    argbase.Add("recipe", {arity=1; vales=[]; plugin="opath"; descr="Name of file containing sequence of opath instructions"})    
    argbase.Add("log-dir-name", {arity=1; vales=[]; plugin="opath"; descr="Name of folder containing logs (defaults to obj)"})
    argbase.Add("obj-dir-name", {arity=1; vales=[]; plugin="opath"; descr="Name of folder to place primary outputs (defaults to dot)."})    
    argbase.Add("verbose", {arity=0; vales=[]; plugin="opath"; descr="Set verbosity level mid"})
    argbase.Add("verbose2", {arity=0; vales=[]; plugin="opath"; descr="Set verbosity level high"}) 
    argbase.Add("issue-area-reports", {arity=0; vales=[]; plugin="opath"; descr="Include an area estimate with each VM"})   
    argbase.Add("progress-monitor", {arity=0; vales=[]; plugin="opath"; descr="Enable asynchronous compilation progress monitoring."})    
    
    app collate_argpatterns (!opath_table)
    let fa = opath_cmd_line_prep ()
    if opvd >= 2 then vprintln 2 ("cmd line args: " + sfold (fun (a, b) -> a) fa)
    // Discard argv[0] which is the mono binary name for opath itself.
    flagged_args1 := tl fa
    

// This is a destructive read from flagged args for a list terminated with --
let cmdline_getlist id =
    let rec sscavenge = function
        | [] -> []
        | (v:string, f)::tt when !f -> []
        | ("--", f)::tt             -> (f:= true; [])
        | (v:string, f)::tt         -> (f:= true; v :: sscavenge tt)

    let mid =  "-" + id
    let rec scan = function
        | [] -> []
        | (v:string, f)::tt when !f=false && v=mid ->
            let _ = f := true
            id :: sscavenge tt
        | _::tt -> scan tt
    scan !flagged_args1


// Non-destructive read
let cmdline_argpresent id =
    let rec chk = function
        | [] -> false
        | ((v:string, f)::tt) -> v=id || chk tt
    chk (!flagged_args1)


// This is a destructive read from flagged args for items of length lenwanted.  It returns a string list with normally the first string being the only one used or expected.
// For example "-verbose " has length lenwanted=0 and "-vnl a.v" has lenwanted=1.
// And lenwanted -1 refers to unqualified arguments.
// This implementation does not require the = sign used by convention, e.g. in -answer=42, but we should change it to be more fussy.

let cmdline_getarg id lenwanted =
    let rec scavenge = function
        | [] -> []
        | ((vv:string, f)::tt) when (* !f=false && *) vv.[0]<>'-' && vv.[0]<>'+' -> (f := true; vv :: scavenge tt)
        | (vv, f)::tt when (vv.[0] = '-' || vv.[0] = '+') ->
            //let _ = vprintln 0 (vv + " ya ya ya " + boolToStr(memberp vv opath_early_args))
            // The flag '--' is just a terminator that should be ignored when encountered.
            if memberp vv opath_early_args0 || vv="--" then scavenge (tt)
            elif memberp vv opath_early_args1 then
                if length tt >= 1 then scavenge (tl tt)
                else cleanexit(sprintf "(early) command line flag %s needs an argument" vv)
            else                
                let (found, arg) = argbase.TryGetValue(vv.[1..])
                if not found then cmdlinehelp()
                if not found then cleanexit("command line flag " + vv + " is not accepted by any installed HPR L/S component")
                cassert(found, " command line flag " + vv + " is not accepted by any installed HPR L/S component")
                //vprintln 0 ("Dropping " + i2s len + " for " + id)
                scavenge (safe_drop_first_n(arg.arity, tt))
        | other_::tt -> scavenge tt

    let mid =  "-" + id
    let rec grab = function
        | [] -> []
        | (v, f)::tt when (* !f=false && *) v=mid && length tt >= lenwanted ->
            let rec scol l = function
                | ((v, f)::tt) when (* !f=false && *) l > 0 -> (f := true; v)::(scol (l-1) tt)
                | _ -> []
            let body = rev (scol lenwanted tt)
            f := true
            //let _ = vprintln 0 ("Cmdline grabbed " + mid + " from " + v)
            id :: body
        | _::tt -> grab tt

    if lenwanted < 0
    then // All remaining, non leading -/+ flags
           id :: (scavenge (!flagged_args1))
    else
           grab (!flagged_args1)

let unused_args () =
    let unused = ref []
    let ss (v,f) = if !f then () else mutadd unused v
    let _ = app ss (!flagged_args1)
    if !unused = [] then ()
    else hpr_warn ("The following command line args were unused " + sfold (fun x->x) (rev(!unused)))




//
// Collate all args required or requested by a component, whether from command line or from recipe.
//
let collate_args m argpattern c2 defs =
    let vd = 3
    let aqf x = "'" + x + "'"
    let _ = if vd>=4 then vprintln 4 (m + ": Validate " + i2s(length argpattern) + " arg slots, " + i2s(length c2) + " c2ars");


    // Give precedence to user preferences
    let getarg id len =
        let userpref = cmdline_getarg id len
        if userpref <> [] then userpref
        else let defv = control_get_strings defs id
             if vd > 4 then vprintln 4 ("Control_Get_strings for " + id + " for defaults returned " + sfold (fun x-> ">" + x + "<") defv)
             if defv = [] then [] else id :: defv
             
    let bypat = function
        | Arg_enum_defaulting(id, values, defaultv, descr) ->        
            let s = getarg id 1
            let _ = cassert(memberp defaultv values, id + " default recipe setting is not a valid setting")
            if s<>[] && memberp (hd(tl s)) values then s
            elif s<>[] then
                   (
                       vprintln 0 (sprintf "Flag '%s' has wrong value:" id);
                       let m = (sprintf "Allowable values for flag '%s' are: " id + sfold aqf values + "\n")
                       let m = m + ("Default values is: " + defaultv + "\n")
                       in cleanexit(m + "Invalid value '" + hd(tl s) +  "' for command line flag '" + id + "'")
                   )
            else [id; defaultv]

        | Arg_int_defaulting(id, defaultv, descr) ->
            let s = getarg id 1
            if s<>[] then s
            else [id; i2s defaultv]

        | Arg_defaulting(id, defaultv, descr) ->
            let s = getarg id 1
            if s<>[] then s
            else [id; defaultv] // todo expand $ in default
         
        | Arg_string_list(id, descr) -> cmdline_getlist id // not looking in defs !!
            
        | Arg_required(id, len, descr, spare) ->
            let s = getarg id (len)
            if s<>[] then
                (if vd>=4 then vprintln 4 ("Arg " + id + " fetched from command line\n");
                 s
                )
            else
                   let k = control_get_s "opath" c2 id (Some "$missing")
                   if k <> "$missing" then
                      (if vd>=4 then vprintln 4 ("Arg " + id + " fectched from command defaults\n");
                       [id; k] // really want more than one in general
                      )
                   else (cleanexit("Command line arg '" + id + "' was not provided.\nPurpose=" + descr); [])
        //| _ -> sf "validate: other arg pattern"
    //let _ = app byarg
        
    let ans = map bypat argpattern
    let _ =
        let note = function
            | [a;b] -> mutadd g_used_parameter_settings (a, b)
            | other -> mutadd g_used_parameter_settings ("?>?", sfold (fun x->x) other)
        app note ans
    let _ = vprintln 3 (sprintf "Validated(1) " + i2s(length argpattern) + " args");    
    ans
    
let listcmds () =
    let k = function
        | (cmds, name, _, _, _, _) -> vprintln 0 ("     "  + cmds + " " + name) 
    let _ = vprintln 0 "Installed HPR commands are :"       
    app k (!opath_table)

let zassoc cmds = 
    let rec z = function
        | [] -> None
        | (cmd', mode, f, c1, _, argpattern)::_ when cmds=cmd' -> Some(mode, f, c1, argpattern)
        | _::tt -> z tt
    z !opath_table    










// Add a new search location to the dll load Path
// Environment.SetEnvironmentVariable("Path",  Environment.GetEnvironmentVariable("Path") + ";" + __SOURCE_DIRECTORY__ + @"\..\packages\Microsoft.TeamFoundationServer.ExtendedClient.15.112.1\lib\native\x86\ ")



// Experimental code to dynamically load additional recipe stages.
// This will shortly be used to dynamically load HPR System Integrator to finish off a build etc..
let chain_system_integrator () =
    let fullName = "sysglobl, Version=4.0.0.0, Culture=neutral, " +  "PublicKeyToken=b03f5f7f11d50a3a, processor architecture=MSIL"
    let fullName = "system_integrator"
    let an = new System.Reflection.AssemblyName(fullName)
    let assem = System.Reflection.Assembly.Load(an)
    vprintln 0 (sprintf "Public types in assembly %A" assem.FullName)
    for t in assem.GetTypes() do
        if (t.IsPublic) then
            vprintln 0 (sprintf "%A" t.FullName)
            for t1 in t.GetMethods() do
                if (t1.IsPublic) then
                    vprintln 0 (sprintf "   sub %A" t1.Name)
                    if t1.Name = "get_banner" then
                        //let obj = System.Activator.CreateInstance(t, null)
                        let banner = t1.Invoke(null, null)
                        match banner with
                            | :? string as str ->
                                vprintln 0 (sprintf "rezzed and " + str)
                            | _ ->
                                dev_println "wont downcast"
                    elif t1.Name = "soc_inst_test" then
                        //let obj = System.Activator.CreateInstance(t, null)
                        let args:obj [] = [| WW_end |]
                        let banner = t1.Invoke(null, args)
                        match banner with
                            | :? string as str ->
                                vprintln 0 (sprintf "rezzed and " + str)
                            | _ ->
                                dev_println "wont downcast"

#if SPARE
            let obj = System.Activator.CreateInstance(t, null)
            match obj with
                |  :? opath_plugin_ifc_t as obj -> 
                    let obj: opath_plugin_ifc_t = obj
                    let banner:string = obj.banner()
             //let _ = new t()
                    vprintln 0 (sprintf "rezzed and " + banner)
                | _ ->
                    dev_println "wont downcast"
#endif


let rec pathcmd ww (stage, recipe) st cmd =
    let vd = 3
    match cmd with
        | OPATH_do(false, stagename, cmds, defs, args) ->
            vprintln 1 ("Opath recipe stage " + stagename + " is disabled (and so is being skipped)")
            vprintln 1 ("Timestamp: (disabled step).   T/S  " + timestamp true)
            ()

        | OPATH_do(true, stagename, cmds, recipe_defs, args) ->
            let vms =
                if nullp !st then
                    vprintln 2  ("Orangepath stack empty. No explicit input VMs for recipe stage " + cmds + ":" + opath_cmdToStr cmd)
                    []
                else mutsub st
                    
            let sr = function
                | [] -> "*NIL?"
                | h::t -> "  " + h + "=" + sfold (fun x->x) t

            establish_log true stagename

            vprintln 2 ("Orangepath: Executing step " + i2s stage + ": '" + cmds + "' of the following recipe:")
            vprintln 1 ("Timestamp: (start).   T/S  " + timestamp true)
            app (recipe_print 2) recipe

            vprintln 3 ("Args= " + !g_argstring + "\n")
            let (mode, f, plugin_defs, argpattern) =
                match zassoc cmds with
                | None -> sf(listcmds(); "orangepath: no orangepath plugin called " + cmds)
                | Some(mode, f, plugin_defs, argpattern) -> (mode, f, plugin_defs, argpattern)
            let step_banner_msg = "opath operation: " + i2s stage + ": " + stagename
            let ww' = WN step_banner_msg ww

            let defs = compile_control vd (recipe_defs, plugin_defs)
            let c3 = collate_args step_banner_msg argpattern args defs
            g_c3 := Some c3
            reportx vd ("Orangepath/recipe stage parameter settings: name=" + stagename) (sr) c3

//Pre-report was being overwritten since same name as post report in reality.
//            let _ = if !g_report_each_step && (not (report_cmd cmd)) 
//                    then report_step ww stage (c3, [["filename"; "pre-step"]; ["suffix"; "-ser.xml"]], m_fatal_marks) vm

            let wrep = function
                | []   -> vprintln 3 ("    arg binding: <nil>")
                | k::l -> vprintln 3 ("    arg binding: " + k + "= " + sfold (fun x->x) l) 
            app wrep c3
            vprintln 3 (sprintf "Starting opath_do %s cmd=%s" stagename cmds)
            let op_stage_actuals:op_invoker_t =
                {
                   banner_msg=        step_banner_msg
                   argpattern=        argpattern
                   command=           Some cmd
                   stagename=         stagename
                   c3=                c3
                }
            let debug_notrap_mode = false // Set this to expose backtraces that mono fails to capture.
            let vms' =
                if debug_notrap_mode then  (f ww' op_stage_actuals vms)
                else
                    try (f ww' op_stage_actuals vms)
                    with
                          e ->
                                vprintln 0 (sprintf "Error: Unhandled exception in stage " + i2s stage + " of recipe: " + cmds + ":" + stagename)
                                let btrc = e.StackTrace
                                mutadd g_earlier_backtrace btrc
                                vprintln 0 (sprintf "\n\nOpath Command Backtrace\n%s\n\n" btrc)
                                raise e // Rethrow it
            vprintln 1 ("Timestamp: (.end.).   T/S  " + timestamp true + " finished " + stagename)
            st := vms' :: (!st)
            let fatal_marks = list_flatten (map fatal_marked vms')
            if not_nullp fatal_marks || (!g_report_each_step && (not (report_cmd cmd)))
            then
                        let samef =  vms = vms' // This equality test seems unlikely to be cheap but mostly will be owing to pointer sharing.
                        //vprintln 2 (sprintf "Report-each-step=%A   in==out is %A" g_report_each_step samef)
                        vprintln 2 (sprintf "=== Post step VM machine is %s pre-step. ===" (if samef then "the same as" else "different from"))
                        if not samef then report_step ww stage step_banner_msg (c3, [["filename"; "post-step"]; ["suffix"; ""]], ref [](*spare_fatal_marks*)) vms'
            mya_stats 3
            if not_nullp fatal_marks then cleanexit("Stopping owing to fatal error in recipe step " + stagename)
            ()

        |  OPATH_pop ->
            if (!st)=[] then sf ("Orangepath stack empty. Cannot do recipe stage " + opath_cmdToStr cmd)
            st := tl (!st)
            vprint 2 "Opath popped\n";
            () 

        | (OPATH_dup) ->
            if (!st)=[] then sf ("Orangepath stack empty. Cannot do recipe stage " + opath_cmdToStr cmd)
            st := hd(!st) :: (!st)
            vprintln 2  ("Opath dup");
            ()

        | (OPATH_label _) -> ()

        | (OPATH_comment _) -> ()

        | (OPATH_pushnull) ->
            st := [ g_null_mc ] :: (!st)
            vprint 2 "Opath push null VM\n";
            ()

        //| other -> sf("opath recipe command not recognised " + opath_cmdToStr other)







let rec runpath1 ww (stage, recipe) st = function
    | [] ->
        let l = length !st
        let _ =
            if l=0 then vprint 1 ("+++ No VM on stack at end of Orangepath recipe.\n")
            elif l> 1 then vprint 1 ("+++ More than one VM on stack at end of Orangepath recipe.\n")
            else vprint 1 ("Runpath: reached end of orange path\n")
        if l=0 then []
        else hd !st

    | cmd::tt ->
      let _ =
        if report_cmd cmd && !g_report_each_step then ()
        else
        (
            vprintln opvd "Starting path cmd"
            pathcmd ww (stage, recipe) st cmd // Stack is mutated as side effect.
            vprintln opvd "Finished path cmd"
        )
      runpath1 ww (stage+1, recipe) st tt

let runpath ww vms recipe = 
    let msg = i2s(length recipe) + " stage Orangepath recipe"
    let _ = vprintln 1 ("Orangepath: about to run " + msg)
    let _ = app (recipe_print 1) recipe
    let _ = vprintln 1 ("Orangepath: commence " + msg + ".")
    let st = ref vms 

    let _ = runpath1 (WN msg ww) (0, recipe) st recipe
    if nullp !st then cleanexit("Orangepath stack empty at end of recipe")
    let ans = !st
    vprintln 1 ("Orangepath: finished " + msg)
    ans


//
//
//   
let deserialise_recipe filename tree =
    let vd = 3
    let msg = (sprintf "orangepath recipe decode filename=%s" filename)
    let rec find_cmd v = 0
    let aqf x = "'" + x + "'"
    let ff1 xstage =
        match xstage with
        | XML_ELEMENT("stage", ats, lst) -> 
            let dess a c =
                match a with
                | XML_ATOM "" -> c
                | XML_ATOM x ->
                    let _ = 0 // vprintln 0 ("Atom x >" + x + "<")
                    x::c
                | _ -> sf ("recipe: malformed stage option component in : " + xmlToStr "" xstage)
            let o_cmd ats x = List.foldBack dess x [] 
            let options = xml_multi "option" o_cmd lst
            let resolvedols = function
                // Various special flags and macros are supported that import settings from other parts of the recipe.  Please document, at least a little!
                // $as-at
                // $as-arg 
                // $require-arg 
                | [a; b; c] when  b = "$as-arg" ->
                    let _ = vprintln 10 (a + "nb=>" + b + "<" + c) ;
                    let nvl = cmdline_getarg c 1
                    let nv = if (nvl=[]) then sf ("Command line flag '" + c + "' is needed (for redirection of '" + a + "'") else tl nvl
                    if vd>=4 then  vprintln 4 ("as rep is " + sfold aqf  nv)
                    a::nv
                | other -> other


            let enabled0 = (xml_insist "enable" ["true"; "false"] (Some 0) lst)=0

            // An enabler token makes a stage run when it is present (typically associated with an output file name).
            let enabler = (xml_once msg "enabler" (fun a it -> killquotes(xml_get_atom it)) (Some "") lst)

            let enabled = enabled0 && (enabler="" || cmdline_argpresent ("-" + enabler)|| cmdline_argpresent ("+" + enabler))            

            // dont do resolvedols if not enabled ... causes problems with redirections
            let defaults =
                if enabled then
                    let d = map resolvedols (xml_multi "defaultsetting" o_cmd lst)
                    if vd>=4 then app (fun x-> vprintln 4 ("defaults are " + sfold aqf x)) d
                    d
                else []
            let sr = function
                | [] -> "*NIL?"
                | h::t -> "  " + h + "=" + sfold aqf t

            //reportx 0 (" recipe defaults") sr defaults

            let stagename =
                let f_cmd ats = function
                    | [XML_ATOM x] -> x
                    | _ -> sf ("recipe: malformed stagename: " + xmlToStr "" xstage)
                xml_multi "stagename" f_cmd lst

            let cmd =            
                let f_cmd ats = function
                    | [XML_ATOM cmds] ->
                        let stagename = if nullp stagename then cmds else hd stagename // Can have multiple issue of some commands with different stage (instance) names
                        let _ = vprintln 3 (sprintf "Parsed from recipe: %A %s cmd=%s" enabled stagename cmds)
                        OPATH_do(enabled, stagename, cmds, defaults, options)
                    | _ -> sf ("recipe: malformed stage cmd: " + xmlToStr "" xstage)
                xml_once msg "cmd" f_cmd None lst
            cmd
            
        | other -> sf ("Unrecognised recipe ingrediant " + xmlToStr "" other)
    
    let recipe_parse00 = function // Parse the recipe
        | XML_ELEMENT("recipe", ats, lst) -> map ff1 lst
        | other -> sf ("recipe_parse00: Unrecognised recipe structure: " + xmlToStr "" other)

    recipe_parse00 tree





// Search paths: return a colon separated list of folders for search. IncDir and so on.
// The search path for redirected libraries is based on the HPRLS environment variable, but
// if this is not set, it relative paths names with respect to the {\tt lib} folder where the {\tt  applicationname.exe} binary is executing from.

let get_hpr_path sol =
    let v = System.Environment.GetEnvironmentVariable("HPRLS")
    let base_directory = System.AppDomain.CurrentDomain.BaseDirectory;    
    vprintln 3 ("get_hpr_path: base_directory = " + base_directory)
    let hprls_ans =
        if v = ""
        then
          ( vprintln 0 ("HPR L/S system env var unset: using " + base_directory);
            []
          )
        elif sol then []
        else [v]
    sfold_colons (hprls_ans @ [ base_directory ])


// Toolchain controlflow entry.    
let opath_main_real productname = 
    let verbosel = cmdline_getarg "verbose" 0
    let verbosef = verbosel <> []
    let verbose2f = cmdline_getarg "verbose2" 0 <> []
    if (cmdline_getarg "meox-enable-sorter1" 0 <> []) then g_sorting1 := true
    if (cmdline_getarg "meox-enable-sorter2" 0 <> []) then g_sorting2 := true    
    if (cmdline_getarg "noreport-each-step" 0 <> []) then g_report_each_step := false
    if (cmdline_getarg "cfg-plot-each-step" 0 <> []) then g_cfg_plot_each_step := true    
    if (cmdline_getarg "issue-area-reports" 0 <> []) then g_issue_area_reports := true
    if (cmdline_getarg "report-each-step" 0 <> []) then g_report_each_step := true
    if (cmdline_getarg "give-backtrace" 0 <> []) then g_give_niceexit := false else g_give_niceexit := true
    if (cmdline_getarg "litp-tagging" 0 <> []) then g_litp_tagging := true

    // Turn on developer-mode warnings and notifications with the command line flag -devx or by setting
    // env variable HPRLS_DEVX.
    let developer_mode =
        let developer_mode = cmdline_getarg "devx" 0 <> []
        let devxs = "HPRLS_DEVX"
        let vale = Environment.GetEnvironmentVariable(devxs)
        let developer_mode = developer_mode  || vale = "1"
        if developer_mode then vprintln 0 (sprintf " devx  (getenv %A=%A)  developer mode=%A" devxs vale developer_mode)
        g_developer_mode := developer_mode
        developer_mode 

    let loglevel = cmdline_getarg "loglevel" 1          // Amount written to log files
    let verboselevel = cmdline_getarg "verboselevel" 1  // Amount listed to console
        
    let print_limit = cmdline_getarg "print-limit" 1 // Limit expression complexity when printed to log files etc
    if not_nullp print_limit then
        let pd = int32(atoi(hd(tl print_limit)))
        meox.g_printing_limit := pd
        ()
    let recipe0 = cmdline_getarg "recipe" 1

    vprintln 2 "Start install_canned_FU_metainfo"
    cvipgen.install_canned_FU_metainfo()
    vprintln 2 "Finished install_canned_FU_metainfo"


    let _ =
        let generic_heuristic_value_override (ivale_ref, cmd_line_name, description_) =
            match cmdline_getarg cmd_line_name 1 with
                | [] -> ()
                | key::vale::_ ->
                    let iv = int32(atoi vale)
                    let _ =  vprintln 1 (sprintf "Setting %s from command line or recipe to %s (%i)" cmd_line_name vale iv)
                    ivale_ref := iv
        map generic_heuristic_value_override [ (g_xor_heuristic_value, "xor-heuristic-value", ""); (g_or_heuristic_value, "or-heuristic-value", "") ]

    let _ =
        if not_nullp loglevel then
            let l = int32(atoi(hd(tl loglevel)))
            vprintln 0 ("Setting loglevel from command line to " + i2s l + "\nHigher values cause the logfiles to be larger. (For greater console output use -verbose or -verbose2.)")
            set_log_verbose l
            
    // Turn on developer-mode warnings and notifications with the command line flag -devx or by setting
                    // env variable HPRLS_DEVX.
    let developer_mode =
        let developer_mode = cmdline_getarg "devx" 0 <> []
        let devxs = "HPRLS_DEVX"
        let vale = Environment.GetEnvironmentVariable(devxs)
        let developer_mode = developer_mode  || vale = "1"
        vprintln 3 (sprintf " devx developer mode status: (getenv %A=%A)  developer mode=%A" devxs vale developer_mode)
        g_developer_mode := developer_mode
        developer_mode


    let _ = if verboselevel <> []
            then
                let vv = atoi32(cadr verboselevel)
                vprintln 0 (sprintf "Setting console verbose level from command line to %i. Range 0 to 10, default 0. Higher is more console output." vv)
                set_console_verbose vv
            elif verbose2f then set_console_verbose(2)
            elif verbosef then set_console_verbose(1)
    //    let _ = vprintln 0 ("Verbosity control 2=" + boolToStr(verbose2f) + " 1=" + boolToStr verbosef + sfold (fun x->">" + x + "<") verbosel)
    let hpr_path = get_hpr_path false
    let log_dir_name = cmdline_getarg "log-dir-name" 1
    let obj_dir_name = cmdline_getarg "obj-dir-name" 1    

    let _ = //  If we set only one of log_dir and obj_dir the other is set the same way.
        if length log_dir_name = 2 then 
            // Need to be able to put full path names here.
            g_log_dir := cadr log_dir_name
            if length obj_dir_name < 2 then g_obj_dir := cadr log_dir_name
            ()

        if length obj_dir_name = 2 then
            g_obj_dir := cadr obj_dir_name
            if length log_dir_name < 2 then g_log_dir := cadr obj_dir_name
            ()
    let vd = 2

    let progress_monitor_enable = (cmdline_getarg "progress-monitor" 0 <> [])
    vprintln 2 (sprintf "Periodic progress monitor enabled=%A." progress_monitor_enable)
    if progress_monitor_enable then Async.Start(g_progress_monitor_delegate)

    let ww_0 = WW_end
    let recipe =
        match zassoc productname with
            | Some _ -> // Synthetic 1-step recipe (when running recipeless)
               [ OPATH_do(true, productname, productname, [], []) ]

            | None ->
                let recipe = if recipe0 <> []
                             then (vprintln 0 ("got rec 0: " + sfold (fun x->x) recipe0); hd(tl recipe0))
                             else Path.Combine("recipes", productname + "00.rcp")
                let filename1 = fst(strip_suffix recipe) 
                match pathopen hpr_path 3 [';'; ':'] filename1 "rcp" with
                    | (None, where_looked) ->
                        //let _ = if nonep filename || (not(existsFile(valOf filename)))
                        failwith ("HPRLS path=" + hpr_path + "\nCannot open hprls recipe on HPRLS path : " + recipe + " filename=" + filename1 + " Looked in " + sfoldcr_lite id where_looked)
                    | (Some filename, _) ->
                        let tree = hpr_xmlin ww_0 (filename)
                        if vd>=4 then vprintln 4 (productname + " using opath receipe: " + filename)
                        let recipe = deserialise_recipe (filename) tree
                        recipe

    app (recipe_print vd) recipe
    let vms = [] // Dont need any to start with now!  // was null_hpr_machine "initial_empty_machine"


    let _ =
        // The  HPRLS_IP_INCDIR environment variable and the  -ip-incdir command line or recipe flag can be set to a string that contains a colon-separated (semicolon on Windows) list of search folders.        
        let cvale = cmdline_getarg "ip-incdir" 1
        let cvale = sfold_colons cvale
        let f = "HPRLS_IP_INCDIR"
        let evale = Environment.GetEnvironmentVariable(f)
        vprintln 2(sprintf "IP search paths:\n  ip-incdir:  cmdline=%s\n   ip-incdir:     env %s=%s" cvale f evale)
        g_ip_incdir := [ "."; cvale ] @ (if evale=null then [] else [ evale ])
        ()

    let normal_run() =
        let vms = runpath ww_0 vms recipe
        unused_args ()
        0
    let vms =
        try normal_run()
        with NiceExit(s) ->
            ( vprintln 0 (productname + " +++ Error exit: " + s);
              1
            )
    vms

//---------------------------------------------
// Main Program Starts Here: opath.exe
let loadPlugins() =
    let vd = 0
    let typeImplementsInterface (interfaceTy : Type) (ty: Type) =
        printfn "Checking %s" ty.Name
        match ty.GetInterface(interfaceTy.Name) with
            | null -> false
            | _    -> true

    if true then ()
    else
    let plugins =
        Directory.GetFiles(Environment.CurrentDirectory, "*.dll")
        |> Array.map (fun file -> Assembly.LoadFile(file))
        |> Array.map (fun asm -> (if vd>=4 then vprintln 4 ("plugin? " + asm.ToString())); (*asm.GetTypes()*))
        //|> Array.concat
        //|> Array.filter (typeImplementsInterface typeof<IOpathPlugin>)
        //|> Array.map (fun plugin -> Activator.CreateInstance(plugin))
        //|> Array.map (fun plugin -> plugin :?> IOpathPlugin)
        //vprintln 3 (i2s(plugins.Count) + "plugins found for loading")
    //plugins |> Array.iter (fun pi -> printfn "Loaded plugin - %s" pi.MethodName)
    ()
    


let dump_all_recipe_settings_and_overrides fd =
    let yy ss = youtln fd ss
    yy ("Report of all settings used from the recipe or command line:")
    for a in !g_used_parameter_settings do
        yy (sprintf "   %s=%s" (fst a) (snd a))
    ()


let generate_final_reports() =
    let _ =
        let m_lines = ref []
        let abr kk vv =
            let ids1 = htos kk
            //vprintln 2 (sprintf "    setting up abbreviation %s for prefix %s\n" vv ids1)
            if vv <> ids1 then mutadd m_lines (sprintf "%30s  %s" vv ids1)
        for e in meox.g_abbreviations do abr e.Key e.Value done
        aux_report_log 1 "Abbreviation" !m_lines
        ()
    ()




// The application <EntryPoint> should jump here after checking for loaded recipe ingredients.
let opath_main (name: string, argv : string array) =
    //vprintln 1 ("Type is " + abstracte.rewrite_rtl.GetType().ToString())
    vprintln 1 (name + "starting.")
    get_flagged_args("old_entry_point")

    let rc = opath_main_real name
    vprintln 1 (name + " finished. Still to write report files. rc="+ i2s rc)
    generate_final_reports()
    write_master_report_file()
    vprintln 1 (name + " finished. Report files written. rc="+ i2s rc)    
    rc
    

//Your C# should reference its data files using the following code snippet:
//    string base_directory = System.AppDomain.CurrentDomain.BaseDirectory;
//    Then you can do things like:
//       string splash_file = Path.Combine (base_directory, "splash.jpg");
//PREFIX/bin
//   * Should contain a script to call your application, typically it contains two lines.
//    PREFIX/lib/APPLICATION
//        * A directory that holds your main executable, libraries and any extra data files.
//        The script in PREFIX/bin typically contains the following two lines:
//          #!/bin/sh
//            exec /usr/bin/mono PREFIX/lib/APPLICATION/app.exe "$@"
       

   

// eof
