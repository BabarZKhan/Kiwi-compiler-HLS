//
// $Id: dotreport.fs,v 1.4 2013-07-09 08:10:21 djg11 Exp $
// (* Dot output routines - for plotting with the dot package *)
//
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
//
//
module dotreport

open yout
open moscow

type dot_atts_t = (string * string) list

type dotitem_t = 
  | DARC of dotitem_t * dotitem_t * dot_atts_t
  | DNODE of string * string             // Just a place holder for a node with port option
  | DNODE_DEF of string * dot_atts_t     // String is 'graph' or 'digraph' or 'strict'
  | DSUB of string list * dot_atts_t * dotitem_t list // Make first string 'subgraph' for clustering


type dot_digraph_t = DOT_DIGRAPH of string * dotitem_t list

let gec_DNODE_plain s = DNODE(s, "")

let cfold = sfold

// string_escape_quotes cf string_can: this is string_can again
let dot_at_to_str(a,b) = sprintf "%s=\"%s\"" a (string_escape_quotes b)

// Styles are dotted, bold, filled=colour".7 .3 1.0", 

let dot_nodesan s = filename_sanitize ['_'; ] s

let rec dot_ats_to_str = function
    | [] -> ""
    | l -> "[" + cfold dot_at_to_str l + "]"

let rec dotout_arc fd = function
    | DSUB(l, ats, items) -> 
        let l = map dot_nodesan l
        ( app (fun x -> yout fd (" " + x + " ")) l;
          yout fd (" { ");
          yout fd ( cfold dot_at_to_str ats + ";\n");
          app (dotout_arc fd) items;
          yout fd ("}\n")
          ()
        )
    | DNODE_DEF(l, ats0) -> 
        let nname = dot_nodesan l
        let ats = if nonep(op_assoc "label" ats0) then ("label", "\"" + l + "\"") :: ats0 else ats0
        ( yout fd ("  " + nname + " ");
          yout fd (dot_ats_to_str ats + ";\n");
          ()
        )

    | DARC(l, r, ats) -> 
        (dotout_arc fd l;
         yout fd (" -> ");
         dotout_arc fd r;
         yout fd (dot_ats_to_str ats + ";\n");
         ()
        )

    | DNODE(id, "")  -> 
        let id = dot_nodesan id
        yout fd (id)

    | DNODE(id, port)  -> 
        let id = dot_nodesan id
        let port = dot_nodesan port
        yout fd (id + ":" + port)

// Method to write a dot file from our internal abstract syntax.
let dotout fd = function
    | DOT_DIGRAPH(id, l) ->
        (
         youtln fd "// Graphviz dot program written by HPR L/S dotreport";
         // The keyword strict, if included, only allows one directed edge between any pair of nodes.
         youtln fd (sprintf "digraph \"%s\" { size = \"160x170\";"  id);
         app (dotout_arc fd) l;
         yout fd ("}\n\n");
         ()
        )
            


(* eof *)
