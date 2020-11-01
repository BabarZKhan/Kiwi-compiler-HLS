(*
 *  $Id: yout.fs $
 *  yout.fs
 * HPR L/S Formal Codesign System.
 * (C) 2003-14 DJ Greaves. All rights reserved.
 *
 *)
module yout

let g_report_banner = ref []
let g_aux_report_file_name = ref "opath.rpt"
let g_aux_reports:(string * string list) list ref = ref [] // Tables and other reports to perhaps be inserted in primary output files (.e.g vnl) and also saved as a final report file.
let g_version_string = ref "--Tool Version String Not Set--"
let version_string (_) = !g_version_string

let g_developer_mode = ref false
let g_hl_call_trace = ref false
let g_earlier_backtrace:string list ref = ref []

let devmode() = !g_developer_mode // Enable print-out of messages for the assistance of the KiwiC toolchain developers.

let set_opath_report_banner_and_name(banner, name) =
    g_report_banner := banner
    g_aux_report_file_name := name
    ()



   
    
let report_banner_toStr comment_escape =
    match !g_report_banner with
        | [] -> comment_escape +  "Report Banner Not Set"
        | items -> List.fold (fun c s -> comment_escape + s + "\n" + c) "" items
        
let stringOf = function
    | None -> "None"
    | Some(s) -> "Some(" + s + ")"

let proto_valOf = function
    | Some a -> a
    | None -> failwith "take valOf None"

let snd (x,y) = y;
let fst (x,y) = x;

let i2s (x:int) = System.Convert.ToString x

let i2s64 (x:int64) = System.Convert.ToString x

let i2x s = sprintf "%x" s

let rec i20x s = if s<0 then "-" + (i20x(0-s)) else "0x" + (i2x s)

let log_fd = ref(false, None); (* Need bool as well since outstream is not an equality type *)

let i2so = function
    | None -> "None"
    | Some n -> i2s n

let i2sm n = if n < 0 then "-" else i2s n
    
let close_log (m:string) =
    function
        | (true, Some(fd:System.IO.StreamWriter)) ->
             (
                    fd.Write(m + ": End of log\n");
                    fd.Close()
              )
        | _ -> ()

// Hmmm: this does not close any diverted logs...
let close_logs(m) =
    (
        close_log m (!log_fd);
        log_fd := (false, None)
    )



// May 2016: System.IO.Path.Combine has started using backslash even on mono, and the OS is reported as Windows NT 5.2 even when it is GNU/Linux,
// so let's force it with an env variable we can set ourselves.    This is spurious - arose when bash invokes wine on a .exe. I always add 'mono' manually and get unix behaviour.
let g_unix_really = ref false
let path_combine(a, b) =
    if !g_unix_really then a + "/" + b
    else System.IO.Path.Combine(a,b)
    
let get_os() =
   let v = System.Environment.GetEnvironmentVariable("MONO_OS_OVERRIDE")
   if String.length v > 1 && (v.[0] = 'L' || v.[0] = 'l') then
       g_unix_really := true
       v
   else System.Environment.OSVersion.VersionString
   
let _ = get_os()




let debug_println s = ()// System.Console.WriteLine(s:string)
let vnum = ref 0;
let vnum1 = ref 0;
let g_obj_dir = ref "."
let g_log_dir = ref "obj"
let g_workingdir = ref "obj"
let i2sa n = (if n<10 then "0" else "") + i2s n
let dnamef n tag = !g_log_dir + "/h" + i2sa n + (if tag="" then "" else "_" + tag) 
let mkDir s = System.IO.Directory.CreateDirectory s
let existsDir s =  try (System.IO.Directory.Exists s) with _ -> false
let existsFile s = System.IO.File.Exists s
let openDir s = System.IO.Directory.GetFileSystemEntries s
let closeDir s = () (* for now *)
let rmDir s = System.IO.Directory.Delete s
let remove s = System.IO.File.Delete s


let rec rmrDir s =
    if existsDir s then 
       let _ = debug_println("Delete dir " + s)
       let ds = openDir s
       //let _ = debug_println("Delete dir contents " + ds.[0])
       //let _ = debug_println("Delete dir length " + i2s(Array.length(ds)))
       let _ = Array.iter (fun s1 -> rmr s s1) ds
       let _ = closeDir ds
       let _ = rmDir s
       in ()

and rmr s q = if existsDir(q)
              then (debug_println (s + "delete dir " + q); rmDir ( q))
              else (debug_println (s + "delete file " + q); remove( q))


//
//
let verylong_ctr = ref 0

let verylongf id =
        if String.length id > 180
        then
            let ans = id.[0..4] + "_" + "verylong" + i2s(!verylong_ctr) + ".txt"
            let _ = verylong_ctr := !verylong_ctr + 1
            let _ = System.Console.WriteLine("Replacing very long log file name with " + ans + "\nWas=" + id)
            ans
        else id

let rec implode1 l =
    let a c = sprintf "%c" c
    match l with
        | [] -> ""
        | h::t -> a h + implode1 t


let explode1 (s:string) =
    let ll = String.length(s)
    let rec ff p = if p>=ll then [] else (s.[p]) :: (ff (p+1))
    ff 0


// Remove toublesome filename characters. Perhaps make another variant of the string_fold_and_sanitize code?
let fn_sanitize l =
    let fn_san = function
        | ('?') 
        | ('$') 
        | ('`') 
        | (' ')  // Spaces in file names are allowed these days by I dont like them
        | ('<') 
        | ('>') 
        | (',') 
        | ('/') 
        | ('\\')
        | ('(') 
        | (')') -> '_'
        | other -> other
    implode1(List.map fn_san (explode1 l))


// ok for use in filenames
let g_okl = [ '.'; '-'; '='; '+'; ':'; '_' ]

// A list of 'ok' chars
// generally ok - excludes quotes and escapes.
let g_okl2 = g_okl @ [ '>'; '<'; '='; '!' ; '*'; '|'; '/'; '('; ')'; '{'; '}'; '['; ']'; '?'; ':' ]


// Collapse a list of strings into a single string made of 'sanitary' characters.
let string_fold_and_sanitize target_linelen okl x = 
    let hd = List.head
    let tl = List.tail
    let memberp c list = List.exists (fun elem -> elem = c) list    
    let isDigit c = '0' <= c && c <= '9'
    let isAlpha c = 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z'
    let rec kk ll x =
        match x with 
        |   [] -> []
        |   c::tt ->
            if c = '\\' && tt<>[] then hd tt :: kk (ll+1) (tl tt)
            else
                let tailer () = kk (ll+1) tt
                if isAlpha c || isDigit c then c :: tailer()
                elif  memberp c okl then
                    (if target_linelen > 0 && ll >= target_linelen then c :: '\n' :: kk 0 tt else c :: tailer())
                else
                    let tx = tailer ()
                    if tx <> [] && hd tx <> '_' then '_' :: tx
                    else tx
                           
    let imp (cl: char list) = (implode1 cl):(string)
    imp (kk 0 (explode1 x))




let string_fold_and_sanitize_no_leading_digit target_linelen okl x = 
   let a0 = string_fold_and_sanitize target_linelen okl x 
   if a0 = "" then "string-vanished"
   else
       let isDigit c = '0' <= c && c <= '9'
       if isDigit a0.[0] then "Z" + a0 else a0 // Prefix this letter on the front to avoid a leading digit.
       //a0


let kind_sanitise ss = string_fold_and_sanitize_no_leading_digit 155 [ '_' ] ss
        
let filename_sanitize okl x = string_fold_and_sanitize -1 okl x  

let fully_sanitise s = string_fold_and_sanitize_no_leading_digit 55 g_okl2 s

// Put a backslash in front of a quote char.
// Also escape newlines
let string_escape_generic escapes x =
    let memberp c list = List.exists (fun elem -> elem = c) list
    let rec kk = function
        | [] -> []
        | '\n' :: tt -> '\\' :: 'n' :: (kk tt)
        | ch :: tt when memberp ch escapes -> '\\' :: ch :: (kk tt)
        | ch :: tt -> ch :: (kk tt)
    let ii (cl: char list) = (implode1 cl):string
    ii (kk (explode1 x))

let string_escape_quotes x = string_escape_generic [ '\"' ] x

let string_can s = "\"" + string_escape_quotes s + "\""

// advf should be set if we are to move on to a new folder of log files.
let establish_log advf id =
    let id = verylongf (filename_sanitize g_okl id)
    let advfile tag =
        let dname =
            if advf then
                let _ = vnum := (!vnum) + 1
                let _ = vnum1 := 0;
                let _ = g_workingdir := dnamef (!vnum) tag
                in !g_workingdir
            else
                let _ = vnum1 := (!vnum1) + 1
                !g_workingdir 
        //let _ = debug_println (sprintf "advfile: dname=%s tag id=%s %b" dname id advf)  

        let cos = 
              if advf then
                  let _ = if existsDir(dname) then rmrDir(dname)
                  let _ = mkDir dname
                  let d1 = "s" + i2sa (!vnum1) + ".log.txt"
                  System.IO.File.CreateText(System.IO.Path.Combine(dname, d1))
              else
                  let d1 = "s" + i2sa (!vnum1) + "." + id + ".log.txt"
                  System.IO.File.CreateText(System.IO.Path.Combine(dname, d1))
        let _ = log_fd := (true, Some cos)
        cos
        
    if id="" then // some log is started already
        if fst(!log_fd) then ()
        else
          let tag = ""
          let cos = advfile tag
          cos.WriteLine("Start of HPR log.")
          cos.WriteLine(version_string 0)
          ()
    else
          if fst(!log_fd) then close_log ("End of this log file. Log continues in file where tag=" + id) (!log_fd)
          log_fd := (false, None)          
          let cos = advfile id
          cos.WriteLine("Start of log: tag="+ id)
          cos.WriteLine(version_string 0)
          ()

exception StopError of string


let errors = ref 0;


// Set this higher to get larger log files.
// Set smaller for smaller log files.
let g_current_log_verbosity = ref 3; (* Normally 3. 10 in verbose level, or set from command line. *)


let stop_on_error (m) = if (!errors > 0) then raise (StopError m)


// verbosity level - lower values are used for more important messages, of which there tend to be fewer.
//   g_console_verbose render level defaults to 0 - only level 0 messages are printed by default.
//   g_log_verbose  render level defaults to 3 - only level 3 and lower messages are put in a log file by default.
//   log file render level is always greater than or equal to log file render level.

// To get more of either form of output we increase the appropriate logging render level.

// Each logging message has its own local static level associated with it. It is highly wasteful of time to generate log messages that are not put in the log, so logging message
// generation is protected by control flow. If the local static level is less than or equal to the log file render level, we generate the message. Preferred programming style
// is
//    if global_vd >= local_vd then vprintln local_vd msg


(*
 *  Levels on vprint: 0=console output always
 *                    1=verbose mode console output
 *                    3=normal logging level for major phases of compilation: output to console in verbose2 mode.
 *                   7=verbose logging level (to /tmp/h2biglog)
 *                   8=much output 
 *                  10=All output and WW 
 *)


// This is set to 0 normally so that only level 0 is printed on the console.
// Set higher for more console output.
let g_console_verbose = ref 0 (* Set this higher to 1 or 2 to enable level 1 logging on console *)

let f_verbose() = !g_console_verbose

let g_global_vd = ref -1 // This detaults to minus one which allows each recipe stage to set its own default. But when this has is increased for greater output, this will dominate the recipe setting owing to the use of 'max' at each recipe stage start.

let set_log_verbose new_vd =
    g_current_log_verbosity := new_vd
    g_global_vd := new_vd // We have two variables for pretty much the same thing.
    ()

let set_console_verbose new_vd =
    g_console_verbose := new_vd
    // If attempt to go higher than log_verbose, set log_verbose to this as well
    if new_vd > !g_current_log_verbosity then set_log_verbose new_vd
    ()
    
    
//let Verbose x = g_verbose := x

let rec verror msg = 
        (
          errors := 1 + (!errors);
          vprint 0 msg
        )

and vprintln vd ss =
    vprint vd ss // Avoid string concat of newline.
    vprint vd "\n"    
    //if vd>=4 then failwith ("Temp stop on verbosity overspill " + ss)
    
and vprint (level:int) (m:string) = 
        (
          //debug_println ("v1 " + m);
          establish_log true ("");
          //debug_println ("v2");
          if level <= !g_console_verbose then
             System.Console.Write(m)
             System.Console.Out.Flush()
          // A change: we are now relying on log message generators to observe vd settings and anything created is now recorded to the log.
          if (fst !log_fd (* && level <= !g_current_log_verbosity*)) then
            (
              proto_valOf(snd !log_fd).Write(m);
              if (level <= 1 || !g_console_verbose>=10) then (proto_valOf(snd(!log_fd))).Flush()
            )
  (*      if (fst(!biglog_fd) andalsoandalso level < 100) then TextIO.output(snd(!biglog_fd), m); *)
        )





let lprint_pred level =  !g_console_verbose >= level || !g_current_log_verbosity >= level

let gprint level m = vprint level m (*Curried versions*)

let lprint level m = if  !g_console_verbose >= level || !g_current_log_verbosity >= level then vprint level (m())

let lprintln level m = if  !g_console_verbose >= level || !g_current_log_verbosity >= level then vprintln level (m())

type logger_t = | YOVD of int | YOFD of System.IO.StreamWriter | YOSL of string * string list ref | YO_null | YO_divert of logger_t * (bool * System.IO.StreamWriter option)

// A YOSL (output string list) is built up backwards, so we need to reverse it on print out.
let yoslToStr = function
    | YOSL(od, m_entries) -> List.foldBack (fun a cc -> cc + a) !m_entries ""


let rec yout_active = function
    | YOVD k        -> k>=0
    | YOFD k        -> true
    | YOSL(id, m_k) -> true
    | YO_divert(current, older) -> yout_active current
    | YO_null       ->false

let rec yout_vd_level = function
    | YOVD k        -> k
    | YOFD k        -> 10
    | YOSL(id, m_k) -> 10
    | YO_divert(current, older) -> yout_vd_level current
    | YO_null       -> -1

        
let rec yout yy msg =
    match yy with
        | YOVD k        -> vprint k msg
        | YOFD k        -> k.Write msg
        | YOSL(id, m_k) -> m_k := msg :: !m_k
        | YO_divert(current, older) -> yout current msg
        | YO_null       -> ()



let youtln fd msg =
    yout fd msg // Avoid string cat
    yout fd "\n"

let yout_open_report(f) = YOSL(f, ref [])

let yout_open_out(f) =
    //vprintln 5 ("Opening log file: fn pre sanitise " + f)
    let f = verylongf (filename_sanitize ('/' :: g_okl) f)
    vprintln 2 ("Opening log file  " + f)
    try YOFD(System.IO.File.CreateText f) with
        _ -> 
             (vprintln 0 ("yout_open_out could not open for writing " + f);
              YOVD 3)

let yout_open_log filename = yout_open_out ((!g_workingdir) + "/" + filename)


// Temporarily open a new log file.
let yout_divert_log filename =
    let n = yout_open_out filename
    let ns =
        match n with
        | YOFD k -> k
    let ans = YO_divert(n, !log_fd)
    (proto_valOf(snd(!log_fd))).Flush()
    log_fd := (true, Some ns)
    ans


let aux_report_log printnow_vd id contents =
    // This prints to log file also at this point.
    let _ =
        vprintln printnow_vd ("Start of aux report " + id)
        let printnow m = vprintln 1 m
        if printnow_vd >= 1 then for x in contents do printnow x done 
        vprintln printnow_vd ("End of aux report " + id)
        ()
    g_aux_reports := (id, contents) :: !g_aux_reports


let rec yout_close = function
        | YOSL(id, m_contents) ->  aux_report_log 2 id (List.rev !m_contents) // A YOSL (output string list) is built up backwards, so we need to reverse it when rendered.
            
        | YOFD s      -> s.Close()
        | YO_divert(n, older) ->
           yout_close n
           log_fd := older
           ()
        | (_)           -> ()

// Collect textual lines containing major statistics about the compilation for putting in report files.
//
let g_majorstat_lines:(string * string) list ref = ref []


let majorstat_line_log (stage:string) (majorstat_line:string) =
    //vprintln 0 (sprintf "Majorstat line >>> %s" majorstat_line)
    g_majorstat_lines  := (stage, majorstat_line)::!g_majorstat_lines 
    ()

// Copy out auxiliary reports to yd.
// The report will be prefixed with comment string on each line. So there should be no everyday newlines in the report strings themselves, since these are implied and we want to be able to width-trim them easily.
let render_aux_reports comment_string yd =
    youtln yd (comment_string + "HPR L/S (orangepath) auxiliary reports.")
    youtln yd (report_banner_toStr comment_string)    
    let m_count = ref 0
    let prepend_comment_strings (text:string) =
        if comment_string = "" then [text]
        else List.map (fun x->comment_string + x) (Array.toList(text.Split [| '\n' |]))

    let add_cr_if_not_preset (s1:string) =
        let sl = String.length(s1)
        if sl > 0 && s1.[sl-1] = '\n' then s1 else s1 + "\n"

    let wrf (id, strings) =
        m_count := !m_count + 1
        yout yd ("\n" + comment_string  + "----------------------------------------------------------\n\n" + comment_string + "Report from " + id + ":::\n")
        for s:string in strings do
            if comment_string = "" then youtln yd s
            else
                for s1:string in s.Split [| '\n' |] do
                    //vprintln 0 (sprintf "The report string is >%s<" s1)
                    yout yd (comment_string + add_cr_if_not_preset s1)
    for r in List.rev !g_aux_reports do wrf r done
    let _ =
        yout yd ("\n" + comment_string + "Major Statistics Report:\n")        
        let write_majorstat (tool_, line) = youtln yd (comment_string + line)
        List.map write_majorstat (List.rev !g_majorstat_lines)
    !m_count


let write_master_report_file() = // Writes a single file such as KiwiC.rpt that contains all of the auxiliary reports.
    let f = !g_aux_report_file_name
    let status = ref "unset"
    vprintln 2 ("Opening log file: fn pre sanitise " + f)
    let f = verylongf (filename_sanitize ('/' :: g_okl) f)
    let f =
        let where = !g_obj_dir
        if where <> "." then path_combine(where, f) else f
    let _ = vprintln 2 ("Opening log file  " + f)
    let yd = 
        try YOFD(System.IO.File.CreateText f) with
            _ -> 
                 (vprintln 0 ("yout_open_out could not open for writing " + f);
                  status := "failed to open report log file";
                  YOVD 1)
    let count = render_aux_reports "" yd
    yout_close yd
    vprintln 2 (sprintf "close report file. count=%i status=%A " count !status + f)
    ()


// Write a report list
let reporty vd banner f lst =    
    let yy ss = yout vd ss
    match lst with
        | [] ->
             yy("Report on " + banner + ": none exist.\n")
        | items ->
              (
                yy ("Report on " + banner + "\n");
                List.iter (fun x -> yy("  " + f x + "\n")) items;
                yy ("End report on " + banner + " (" + i2s(List.length items) + " items)\n\n");
              ()
              )


let reportx vd banner f lst = if vd > !g_current_log_verbosity then () else reporty (YOVD vd) banner f lst

let rec underscore_fold = function
    | []     -> "NIL"
    | [item] -> item
    | h::tt  -> underscore_fold tt + "_" + h

let rec dot_fold = function // This reverses since the innermost tag is at the head of the list.
    | [] -> "NIL"
    | [item] -> item
    | h::tt -> dot_fold tt + "." + h

let hptos =
    //sprintf "BOZZER%iRR" (List.length s) +
    dot_fold

let rdot_fold l = dot_fold(List.rev l)

let gc_stats()= ""


//---------------------------------
// High-level backtrace debugging aid.
//
type WW_t =
    | WW_end
    | WW_nest of string * WW_t * int

let logpoint = ref WW_end

let unwhere ww a =
    if !g_current_log_verbosity >= 10 then vprintln 0 ("WN unnest")
    match a with                                                                
    | (WW_nest(_, v, idx)) -> (logpoint := v; v)


let whereami_del del ff ww = 
    let rec scan = function
        | WW_end -> ff("BASE" + del)
        | WW_nest(msg, parent, idx) ->
            if del = "" then ff msg else ff("  " + i2s idx + " " + msg + del)
            scan parent 
    scan ww
    //vprintln 0 (sprintf "g_hl_call_trace=%A complete" !g_hl_call_trace)

let whereAmI f = whereami_del "\n" f (!logpoint)

let whereAmI_str(k) =
    let r = ref "/"
    let f s = (r:= (!r) + "/" + s)
    whereami_del "\n sfp - " f  (!logpoint)
    k + (!r)

let whereAmI_concise ww =
    let r = ref "/"
    let f ss = (r := !r + "/" + ss)
    whereami_del "" f ww 
    (!r)
    
let WN msg ww =
    let rr = WW_nest(msg, ww, 0)
    if !g_hl_call_trace then whereami_del " --- " (vprintln 0) rr
    elif !g_current_log_verbosity >= 10 then vprintln 0 ("WN nesting " + msg)
    logpoint := rr
    rr

let WF lev current_method_name ww action_being_taken =
    let m = (current_method_name + ": " + action_being_taken)
    vprintln lev ("WF:" + m)
    let rr = WN m ww
    rr




let whereAmIf () = whereAmI_concise (!logpoint)


// Normally set this to true in opath, unless early arg kills it.
// The devx mode perhaps should replace it now.
let g_give_niceexit = ref false


exception NiceExit of string


let cleanexit s =
    (
      write_master_report_file() 
      vprintln 0 ("=ERROR=-=-=->" + s);
      if (!g_give_niceexit) then
          let _ = vprintln 0 (sprintf "You may wish to add -give-backtrace to your command line for a high-level backtrace.")
          raise (NiceExit s)
      else (
              vprintln 0 ("high-level backtrace");              
              whereAmI (fun x -> vprint 0 x);
              failwith s
           )
    )

let sf s =
    (
      write_master_report_file();
      vprintln 0 ("=ERROR=-=-=->" + s);
      vprintln 0 ("high-level backtrace");                    
      whereAmI (fun x -> vprint 0 x);
      failwith s
    )
    
let muddy s = sf("Muddy bog: a part of the compiler tool is missing: " + s) 

let valOf = function
    | Some a -> a
    | None ->
        sf "applied valOf to None"

let once msg = function
    | [item] -> item
    | [] -> sf (msg + ": once - no args")
    | _  -> sf (msg + ": once - multiple args")    

// Convert a string to a list of strings, splitting at dots or other delimiter.  List has last element first?
let split_string_at delims l = 
    let memberp c list = List.exists (fun elem -> elem = c) list
    let trim1(l:string, c) =
        let l' = l.Trim()
        if l' = "" then c else l' :: c
    let rec kk  = function
        | ([], a, c)   -> (implode1(List.rev a)::c)
        | (h::t, a, c) -> 
            if memberp h delims then kk(t, [], trim1(implode1(List.rev a), c))
            else kk(t, h::a, c)
    List.rev(kk(explode1 l, [], []))



let string_to_assoc_list (ss:string) =
    let items = split_string_at [ ';' ; ':' ] ss
    let split_eq item =
        match split_string_at [ '=' ] item with
            | []         -> ("", "")
            | [single]   -> (single, "")
            | kk::vv::_  -> (kk, vv) // Remainder ignored
    let pairs = List.map split_eq items
    pairs

let split_string key l = split_string_at [key] l


let split_at_dots(l) =  split_string_at ['.'] l

let rec sfold_colons = function // Assemble a search path
    | []     -> ""
    | [item] -> item
    | h::tt  -> (if h="" then "" else h + ":") + sfold_colons tt

// Find a file within some set of folders on a search path.
let pathopen_any searchpath vd delims files (suffix:string) =
    let m_places_looked = ref [] // For debug reporting
    let suffix = if suffix.Length > 0 && suffix.[0] = '.' then suffix.[1..] else suffix
    let places = split_string_at delims searchpath
    let rec kk_file = function
        | [] ->
            if vd >= 4 then vprintln 4 (sprintf "pathopen: end of %i file alternatives without finding pot of gold." (List.length files))
            None

        | file::alt_files ->
            let rec kk_place = function
                | [] ->
                    if vd >= 4 then vprintln 4 (sprintf "pathopen: end of search path reached without finding pot of gold.")
                    None
                | h::tt ->
                    if existsDir h then
                        let n = System.IO.Path.Combine(h, file) + "." + suffix
                        let ee = existsFile n 
                        m_places_looked := (sprintf "Looked in %s  result=%A" n ee) :: !m_places_looked 
                        if vd >= 4 then vprintln 4 (sprintf "pathopen: Try open success=%A for %s" ee n)
                        if ee then Some n else kk_place tt
                    else
                        if vd >= 4 then vprintln 4 (sprintf "pathopen: non-existent folder on search path: %s" h)
                        kk_place tt
            match kk_place places with
                | None -> kk_file alt_files
                | Some ans -> Some ans
    let ans = kk_file files
    (ans, List.rev !m_places_looked)


let pathopen searchpath vd delims file suffix = pathopen_any searchpath vd delims [file] suffix
        
(*-------------------------------------------------------------------------------------
 * 
 *
 *
 *
 *)
let validate_cmd_line_assoc patterns args =
    let args1_to_string (_) = List.fold (fun c a -> a + " " + c) "\n" args
    let rec usenix s x =
        match x with
            | ([]) -> failwith(args1_to_string()  + "\nUnknown command line flag: " + s)
            | ((key, n, msg)::t) -> 
               (if n=0 then vprintln 0 ("\n\n" + msg)
                else vprintln 0 (key + "  \t" + (i2s n) + ": " + msg);
                usenix s t
               )
    let rec j s x =
            match x with
            | [] -> -1
            | ((key, n, msg)::t) -> (if n > 0 && s=key then n else j s t)
    
    let rec kk = function
        | ([], c) -> List.rev c
        | (s::t, c) -> 
                let r = j s patterns 
                in 
                   if r=1 then kk(t, (s, None)::c)
                   elif r=2 && t<>[] then kk(List.tail t, (s, Some(List.head t))::c)
                   elif r=2 then (failwith(s + ": requires an argument"); c)
                   else (usenix s (patterns); c)
    kk(args, [])



let rec cmd_line_assoc =
    function
        | ((x,y)::t, key, fatalf) -> (if key=x then y else cmd_line_assoc(t, key, fatalf))
        | (_, key, fatalf) -> if fatalf then (failwith("Cannot find arg " + key); None) else None


// Return a list of all occurrences of this flag: eg multiple -sonthcontrol flags.
let rec cmd_line_assoc_any =
    function
        | ((x,y)::t, key) -> if key=x && y<>None then (valOf y)::cmd_line_assoc_any(t, key) else cmd_line_assoc_any(t, key)
        | ([], key) -> []


let rec cmd_line_flag =
    function
        | ((x,y)::t, key) -> ((key = x) || cmd_line_flag(t, key))
        | (_, key) -> false

let rec find_index key =
    function
        | (p, [])  -> -1
        | (p, h::t) -> if h=key then p else find_index key (p+1, t)


let select_nth (n0:int) l =
    let rec ss n = function
            | h::t -> if n = 0 then h else ss (n-1) t
            | _ -> sf("Select_Nth:" + i2s n0)
    ss n0 l



(*
 * Check a flag has one of its allowable values and return its index.
 *)
let cmdline_flagset_validate key allowable defaultv lst =
    let rec sfold f x =
        match x with
            | [] -> ""
            | [item] -> f item
            | (h::t) -> (f h) + ", " + (sfold f t)

    let failed(k) =
        let _ = if k <> "" then vprintln 0 ("Flag '" + key + "' was '" + k + "'")
        in sf("Command line flag '" + key + "' must be one of " + sfold (fun x->x) allowable)

    let default_action() =
        if defaultv < 0 then failed("")
        else (vprintln 3 ("Using default value of " + select_nth defaultv allowable + " for " + key); defaultv)

    let action j = 
            let r = find_index j (0, allowable)
            in if r < 0 then failed(j) else r

    let rec k = function
            | [] -> default_action()
            | ((q::j::_)::tt) -> if q=key then action j else k tt
            | ((q::_)::tt) -> if q=key then action "*not-specified*" else k tt
    k lst


let args_to_string args =
    let hd = List.head
    let tl = List.tail
    List.fold (fun c (a, b) ->a + " " + (if b=None then "" else valOf b + " ") + c) "\n" args




let a_assoc msg lst v =
    let rec kk = function
        | [] -> sf (sprintf "a_assoc: %s:  value '%A' missing in list %A" msg v lst)
        | (x,y)::tt -> if v=x then y else kk tt
    kk lst

// Return true if this is an absolute file path, but do not get confused with "/r:" windows-style flags that we allow on file names to make Makefiles easier to write.
let abs_file_path_pred (ss:string) =
    if ss.Length < 3 then false
    elif ss.[0] = '\\' then true  // Windows
    elif ss.[1] = ':' then true   // Windows drive specifier
    elif ss.[0] = '/' && ss.[2] <> ':' then true // Unix 
    else false
    

let hpr_warn(x) = vprintln 0 ("** Warning: " + x)
let hpr_yikes(x) = hpr_warn(x)


let logdir_path s = System.IO.Path.Combine(!g_log_dir, s)


let string_clip len (ss:string) =
    if ss.Length > len then ss.[0..len-1] + "..." else ss

let mutadd_str sofar (ss:string) = sofar := !sofar + ss

let dev_println msg = if !g_developer_mode then vprintln 0 ("+++ devx: " + msg)

(* eof *)
