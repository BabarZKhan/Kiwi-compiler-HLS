// $Id: hprxml.fs,v 1.14 2013-06-08 11:26:57 djg11 Exp $


module hprxml

open moscow
open yout
open System.Xml

type xml_ns_ref_t =
    {
        prefix: string
        namespaceName: string
    }

type xml_t =
  | XML_ATOM of string 
  | XML_ELEM2 of xml_ns_ref_t * string * (string * string) list * xml_t list // Element that supports XML namespaces.
  | XML_ELEMENT of string * (string * string) list * xml_t list // Old element - used without namespaces
  | XML_INT of int64  // Integers
  | XML_WS of string  // White space
  | XML_COMMENT of string
  | XML_RAWOPEN of string * (string * string) list
  | XML_RAWCLOSE of string 


let xml_id_escape x = x


let xmlToStr dd  = function
    | (XML_ELEMENT(v, ats, lst)) -> "Element " + v + sfold (fun (a, b) -> a + "=" + b) ats
    | (XML_COMMENT s)-> "Comment + s"
    | (XML_WS s)     -> "String " + s
    | (XML_ATOM s)   -> "Atom " + s
    | (XML_INT n)    -> "Int " + (i2s64 n)


// Write out XML document to file system.
let hpr_xmlout ww filename autows tree = 
    let _ = WF 2 "hpr_xmlout" ww (sprintf "Write file %s. Start." filename)
    let cos =
        if false then System.Console.Out
        else ((System.IO.File.CreateText filename) :> System.IO.TextWriter)

    let settings = new XmlWriterSettings()
    settings.Indent <- autows
    let w = XmlWriter.Create(cos, settings)
    //This prefix is added by the dotnet output library now.
    //let _ = w.WriteRaw("<?xml version='1.0'?>\n\n")
    let rec xml_out dd (w:XmlWriter) = function
        | (XML_ELEM2(ns, v, a, lst)) -> 
           (
            //if autows then for i in 0..dd do w.WriteRaw("  ");
            w.WriteStartElement(ns.prefix, v, ns.namespaceName);
            app (fun (key, vale) -> w.WriteAttributeString(key, "", vale)) a;
            app (xml_out (dd+1) w) lst;
            w.WriteEndElement();
            //if autows then w.WriteRaw("\n");
           )          

        | (XML_ELEMENT(v, a, lst)) -> 
           let toks = v.Split[| ':' |] // This form relies on any namespace having been already issued via ELEM2
           (
            //if autows then for i in 0..dd do w.WriteRaw("  ");
            if toks.Length = 2 then w.WriteStartElement(toks.[0], toks.[1]) else w.WriteStartElement(v);
            app (fun (a,b) -> w.WriteAttributeString(a, b)) a;
            app (xml_out (dd+1) w) lst;
            w.WriteEndElement();
            //if autows then w.WriteRaw("\n");
           )          
        | (XML_COMMENT s)-> w.WriteComment s
        | (XML_WS s)     -> w.WriteRaw("\"" + xml_id_escape s + "\"")
        | (XML_ATOM s)   -> w.WriteRaw(xml_id_escape s)
        | (XML_INT n)    -> w.WriteRaw(i2s64 n)
    xml_out 0 w tree
    w.Flush()
    cos.Close()  // w.Close() closes cos? Can get a sharing violation on subsequent rewrite despite calling w.Close().
    let _ = WF 2 "hpr_xmlout" ww (sprintf "Write file %s. Finished." filename)
    ()


//
// Read in an XML document
//    
let rec hpr_xml_lex ww (reader:XmlTextReader) =
    let r = ref []
    let _ =
       while (not reader.EOF) do 
          let ee = reader.IsEmptyElement
          let name = reader.Name
          let nt = reader.NodeType
          let vale = reader.Value
          //vprintln 0 (sprintf  " Name=%s Value=%s  empty=%b nodetype=%s " name vale ee (nt.ToString())); 
          let (rc, tok) =
              match nt with
              | XmlNodeType.Text       -> (1, XML_ATOM (vale))
              | XmlNodeType.Comment    -> (1, XML_COMMENT (vale))
              | XmlNodeType.Whitespace -> (0, XML_WS vale)
              | XmlNodeType.Element    ->
                  let ats = ref []
                  let _ = if (reader.HasAttributes)
                          then
                           (
                               //vprintln 0 ("Attributes of <" + reader.Name + ">");
                               while (reader.MoveToNextAttribute())
                                  do  (
                                         //vprintln 0 (sprintf " {%s}={%s}" reader.Name reader.Value);
                                         mutadd ats (reader.Name, reader.Value)
                                      )
                           )
                  (1, XML_RAWOPEN(name, !ats))
              | XmlNodeType.EndElement -> (1, XML_RAWCLOSE(name))
          if rc > 0 then mutadd r tok
          if ee then mutadd r (XML_RAWCLOSE name)
          let _ = reader.Read()
          ()
    rev (!r)
    


let rec make_xml_tree endtag arg =
    match arg with
    | [] -> ([], [])


    | (XML_ATOM s)::tt -> 
        let (next, rest) = make_xml_tree endtag tt
        let sl = split_string_at [ ' '; '\n'; '\t'  ] s
        ((map (fun s->XML_ATOM s) sl) @ next, rest)


    | (XML_RAWCLOSE s)::tt -> 
        if (endtag = []) then (failwith("bad xml file"); ([], tt))
        elif (hd endtag = s) then ([], tt)
        else make_xml_tree (tl endtag) ((XML_RAWCLOSE(s))::tt)

    | (XML_RAWOPEN(s, atts))::tt -> 
        let (subtree, rest) = make_xml_tree (s::endtag) tt
        let (next, rest') = make_xml_tree endtag rest
        (XML_ELEMENT(s, atts, subtree)::next, rest')

    | (XML_COMMENT _)::tt
    | (XML_WS _)::tt ->
        let (next, rest) = make_xml_tree endtag tt
        ((hd arg)::next, rest)

    | other -> sf(sprintf "make_xml_tree other form %A" other)


// Read in an XML document.  There is a 
let hpr_xmlin ww filename =
    let vd = -1
    let ww = WF 3 "hpr_xmlin" ww (sprintf "Start XML read file %s" filename)
#if USENEW
    // Use the dotnet library XML reader perhaps ?
#endif
    let tokens = 
       try
           let textReader = new XmlTextReader(filename:string)
           let _ =
            (
                ignore(textReader.Read());
                // System.Console.WriteLine (reader.NodeType); // It prints "XmlDeclaration"
                textReader.MoveToContent();
            )
           hpr_xml_lex ww textReader
       with ss ->
           vprintln 0 ("+++ Error reading XML file " + filename)
           raise ss
       
    if vd >=4 then vprintln 4 (sprintf "Read " + i2s(length tokens) + " tokens from xml file " + filename)
    let (tree, leftover) = make_xml_tree [] tokens
    if leftover <> [] || length tree <> 1 then hpr_yikes(sprintf "+++ XML document read: LEFT OVER TOKENS AT END OF " + filename)
    //hpr_xmlout ww "" "sos" (hd tree)
    let ww = WF 3 "hpr_xmlin" ww (sprintf "Finished XML read file %s. %i top elements." filename (length tree))    
    hd tree


let xml_collate token ff lst =
    //vprintln 0 ("xml_collate: Collating for " + token )
    let rec scan x cc =
        //vprintln 0 (" s 1 " + sfold (xmlToStr 0) x)
        match x with
            | XML_ELEMENT(id, ats, lst1) when id=token ->
                //vprintln 0 ("xml_collate: Collating found " + token + " lst=" + i2s(length lst1) + " ats=" + i2s(length ats));
                (ff ats lst1)::cc
            | _ -> cc
    List.foldBack scan lst []


let rec skipwhite (s:string) = if isWhite(s.[0]) then skipwhite(s.[1..]) else s

let killquotes (s:string) = if s.[0] = '\"' then s.[1..(String.length s - 2)] else s

let rec xml_get_atom = function
    | XML_WS(ws)::tt  -> xml_get_atom tt
    | XML_ATOM(i)::t  -> skipwhite i
    | other -> sf ("xml_get_atom: no atom: " + sfold (xmlToStr 0) other)


// Clearly, there is a difference between functions that find a token or else return a default and those that report an error when that token is missing. More subtly, there is a third (more fundamental) variant that returns None when the token that is not present and Some v when it is. 
// Our naming scheme is that x_option is the latter (fundamental) and without the _option suffix we accept an optional default that will throw an error if the default itself is missing.

let xml_once_option msg token lst =
    let rec scan x cc =
        match x with
            | XML_ELEMENT(id, ats, elem) when id=token -> (ats, elem)::cc
            | _ -> cc
            //| _ -> failwith ("Wot Else" + xmlToStr 0 (hd x))
    match List.foldBack scan lst [] with
        | [item] -> Some item
        | []     -> None
        | multiple -> cleanexit(msg + ": xml_once: only one " + token + " expected in " + sfold (xmlToStr 0) lst)


//
// xml_once : Find at most one element called token. When found, apply ff to its body.
//            If defv<>None return that on error, else runtime error.
//
let xml_once msg token ff defv lst =
    match xml_once_option msg token lst with
        | Some(ats, elm) -> ff ats elm
        | None ->
            if nonep defv then cleanexit(msg + ": xml_once: at least one " + token + " expected amongst " + sfold (xmlToStr 0) lst )
            else valOf defv


let rec xgao lst =
    match lst with
        | XML_WS(ws)::tt  -> xgao tt
        | XML_ATOM(i)::tt -> Some(skipwhite i)
        | _ -> None // Hmmm or error

// Get an optional atom (i.e. string).
let xml_once_atom_option msg token xml =
    match xml_once_option msg token xml with
        | Some(ats, elm) ->
            let rec xgao lst =
                match lst with
                    | XML_WS(ws)::tt  -> xgao tt
                    | XML_ATOM(i)::tt -> Some(skipwhite i)
                    | _ -> None // Hmmm or error
            xgao elm
        | None -> None


let xml_once_atom msg token defv xml =
    let a0 =
        match xml_once_option msg token xml with
            | Some(ats, elm) -> xgao elm
            | None -> None
    match a0 with
        | Some ans -> ans
        | None ->
            if nonep defv then cleanexit(msg + ": xml_once: at least one " + token + " expected amongst " + sfold (xmlToStr 0) xml)
            else valOf defv

// Get an optional boolean flag
let xml_once_bool_option msg token xml =
    match xml_once_atom_option msg token xml with
        | Some "true"  -> Some true
        | Some "false" -> Some false
        | None -> None
        | Some ss -> cleanexit(msg + sprintf ": xml_once_bool_option: expected true or false for token '%s' not '%s' amongst " token ss + sfold (xmlToStr 0) xml)

// Get a mandatory boolean flag
let xml_once_bool msg token defv xml =
    match xml_once_bool_option msg token xml with
        | Some bv -> bv
        | None ->
            match defv with
                | Some bv -> bv
                | None ->
                    vprintln 1 (sprintf "Input XML fragment was %A" xml)
                    cleanexit(msg + sprintf ": xml_once_bool: atom true or false expected for token '%s', not '%s'" token "None")

// Get an optional integer value
let xml_once_int_option msg token xml =
    let rec xml_get_int lst =
        match lst with
        | XML_WS(ws)::t  -> xml_get_int t
        | XML_ATOM(i)::t ->
            let i = skipwhite i
            if isDigit_or_minus(i.[0]) then atoi64(i)
            else sf ("xml_get_int: not a digit or minus " + sfold (xmlToStr 0) lst)    
        | _ -> sf ("xml_get_int: other form " + sfold (xmlToStr 0) lst)    

    match xml_once_option msg token xml with
        | Some(ats, elem) -> Some(xml_get_int elem)
        | None -> None


// Get a mandatory integer value
let xml_once_int msg token defv xml =
    match xml_once_int_option msg token xml with
        | Some ans -> ans
        | None ->
            if nonep defv then cleanexit(msg + ": xml_once_int: at least one numeric " + token + " expected amongst " + sfold (xmlToStr 0) xml)
            else valOf defv



// Get an optional double value
let xml_once_double_option msg token xml =
    let rec xml_get_double lst =
        match lst with
        | XML_WS(ws)::t  -> xml_get_double t
        | XML_ATOM(i)::t ->
            let i = skipwhite i
            if isDigit_or_minus(i.[0]) then sf (sprintf "xml Need double from %s " i)
            else sf ("xml_get_double: not a digit or minus: " + sfold (xmlToStr 0) lst)    
        | _ -> sf ("xml_get_double " + sfold (xmlToStr 0) lst)    

    match xml_once_option msg token xml with
        | Some(ats, elem) -> Some(xml_get_double elem)
        | None -> None


//
// xml_multi : Find none or more elements called token.
//
let xml_multi token ff lst =
    let rec scan cc x =
        match x with
            | [] -> rev cc
            | XML_ELEMENT(id, a, lst1)::tt ->
                let cc = if id=token then (ff a lst1)::cc else cc
                let ans = scan cc tt
                ans
            | _::tt -> scan cc tt
    scan [] lst

     

// xml_insist: find setting for a flag called token out of allowable options for it.
// Return natural index of that flag in the options list. Zero is first.
let xml_insist token options defv lst =
    let def () = if defv=None then failwith ("Missing xml parameter for " + token)
                 else valOf defv
    let rec gs = function
        | XML_WS _::t    -> gs t
        | XML_ATOM(v)::t -> killquotes(skipwhite v)
        // | (XML_S(v)::t) -> skipwhite v        
        | _ -> sf("malformed insist for " + token)
        
    let rec scant = function
            | [] -> None
            | XML_ELEMENT(id, a, lst)::tt when id=token -> Some (gs lst)
            | _::tt -> scant tt
    let d = scant lst

    let rec mtch p = function
        | []      -> None
        | a::tt ->
                (
                    //vprintln 0 ("insist " + i2s p + " cf " + a + " with " + (valOf d));
                    if a=valOf d then Some p else mtch (p+1) tt
                )
    if d=None then def()
    else
        let oo = mtch 0 options
        if nonep oo then sf (token + "=" + valOf d + " in XML file did not match an allowable option:" + sfold (fun x->x) options)
        else valOf oo


// eof
