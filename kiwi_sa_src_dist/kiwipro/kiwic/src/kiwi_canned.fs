//
// Kiwi Scientific Acceleration: KiwiC compiler.
//
// kiwi_canned.fs:  canned libraries - these are bits of AST in very much the same form as comes in from the pe/cil parser.
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

module kiwi_canned

open yout
open moscow
open cilgram
open asmout
open hprls_hdr

// We are using algebraic rather than nominal types, hence every occurence of something like System.String should be replaced with the g_canned_string primitive.
let g_orangelib     = Cil_cr "Kiwi"

let gec_Cil_fp (ty, id) = Cil_fp(None, None, ty, id)

let is_i32 = function
    | CTL_net(false, 32, Signed, g_native_net_at_flags) -> true
    | _ -> false


// Ideally we distinguish KiwiC implementations away from HPR implementations at all points.
// But in this C# front end they are all marked the same and the distinction is made later in the detailed table of canned/native methods.    
let g_flag_KiwiC = Cilflag_hprls // Temporary use of the old (poor) name.

let g_pointer_size = 8L     // A pointer takes 8 bytes since we have a 64-bit virtual machine. 

let c_arg v = (None, v)

let gec_call(opcode, kind, rtype, fr, signat) = Cili_call(opcode, kind, rtype, fr, signat, new_callsite "canned")

let gec_instance_call(rtype, fr, signat) = gec_call(KM_call, CK_default true, rtype, fr, signat)

let gec_static_call(rtype, (fr1, fr2), signat) = gec_call(KM_call, CK_default false, rtype, (fr1, fr2, []), signat)

let g_canned_srcfile = "kiwic-canned"

let g_canned_vd = ref 2

// The built-in array, CT_arr, implements the following interfaces.
let g_CT_arr_parents =
    [
        Cil_cr_idl [ "IEnumerable`1"; "Generic"; "Collections"; "System"]
        Cil_cr_idl [ "IStructuralComparable";  "Generic"; "Collections"; "System"] // Ticks need here probably.
        Cil_cr_idl [ "IStructuralEquatable";  "Generic"; "Collections"; "System"]
// These others need adding in the near future.
//: Array has ICloneable, IList, ICollection,  IEnumerable, IStructuralComparable, IStructuralEquatable
    ]


// Low-level builtin functions
type kiwi_bif_fn_t =
 {
    uname:          string list      // User name seen in C# src code
       // Arg types assumed to be correct by construction for now.
    rt:             ciltype_t option // Return type. 
    arity:          int              // This arity is the visible user args and for an instance call there is one further real arg that is not included in the figure stored here.
    hpr_name:       string        // HPR L/S logic library name or else "<hardcoded>" if implemented inside the KiwiC frontend.
    identity_types: bool          // Holds if return type is always the same as first arg type regardless of what we have canned.

    //has_fu_arg:  string option    // Denotes that invoking this function flags that then named argument is a structural reference to an FU.
    
    // These following fields are identical to native_fun_signature_t and we should share a common substruct please:
    instancef:   bool              // aka not static
    eis:         bool // An 'end in self' - something that is a useful piece of work in its own right.
    is_loaded:   bool
    hpr_native:  native_fun_signature_t option // Will be None for <hardcoded>
 }

// Functions that are converted to TLM calls on SRIs are no in the primitive table. They are flagged with the HprPrimitiveFunction attribute, but they just experience coverstion to TLM form and are ultimately implemented in imported IP blocks.
type kiwi_bif_table_t = Map<string list * bool * int, kiwi_bif_fn_t> // Builtin functions can only be overloaded by arity.


let g_baseline_bif = 
 {  rt=              None
    uname=           []
    identity_types = false
    hpr_name=        "nonewname"
    arity=           0
    instancef=       false // static.
    eis=             false // An 'end in self' - something that is a useful piece of work in its own right. Also implies not ref-trans.
    hpr_native=      None
    is_loaded=       false
    //has_fu_arg=      false
 }

let proto_kiwi_canned_bif_table = ref Map.empty

let additional_eis_table =
     [
        [         "hpr_testandset"; "hprls_primitives"; "KiwiSystem" ];
     ]

let g_macro_offset = ref -1000

let freshwrap handler_ctx ins =
    let n = !g_macro_offset
    g_macro_offset := n - 1 // Allocate unique numbers for all macro instruction offset fields
    CIL_instruct(n, handler_ctx, [], ins)

let gec_ins ins = freshwrap [] ins
    
let gec_CIL_method eis cr (flags1, fap, rt, id, formals, _, body) =
    let vd = !g_canned_vd
    let unique_id = funique (cil_classrefToStr id)
    let instancef = memberp Cilflag_instance flags1
    let staticf = memberp Cilflag_static flags1    
    if not(instancef || staticf) then sf ("missing hasthis for canned " + unique_id)
    let ck = CK_default instancef
    let revpath =
        let rec mm = function
            | Cil_cr c -> [c]
            | Cil_lib(a, b)       ->  mm b @ mm a
            | Cil_namespace(a, b) ->  mm b @ mm a
            | Cil_cr_dot(a, b)    ->  b :: mm a
            | Cil_cr_slash(a, b)  ->  b :: mm a
            | cr -> muddy (sprintf "path %A" cr)
        mm cr
    let lname =
        match id with
            | Cil_cr uname -> uname
            | other -> sf (sprintf "other form uname %A" other)
    let arity = length formals

    let (hpr_name, biff) = // Not all canned methods are considered built-in - some just have regular, perhaps unsafe, CIL bodies.
        let rec mine_hpr_name = function
            | [] -> ("<hardcoded>", false)
            | Cilflag_hprls :: _ -> ("<hardcoded>", true)
            | Cilflag_hpr s :: _ -> (s, true)
            | _ :: tt -> mine_hpr_name tt
        mine_hpr_name flags1
    let bif = 
            { g_baseline_bif with
               uname=lname::revpath
               rt=Some rt
               eis=eis
               instancef=instancef
               arity=arity
               hpr_name= hpr_name
            }

    if vd>=4 then vprintln 4 (sprintf "Installed canned/built-in function %s: bif=%A  instancef=%A eis=%A arity=%i rt=%s uid=%s hpr_name=%s" (htos bif.uname) biff bif.instancef bif.eis bif.arity (cil_typeToStr(valOf bif.rt)) unique_id hpr_name)
    if biff then proto_kiwi_canned_bif_table := (!proto_kiwi_canned_bif_table).Add((bif.uname, bif.instancef, bif.arity), bif)
    let flags2 =
        {
            is_startup_code=false
        }
    CIL_method(g_canned_srcfile, (cr, fap, []), flags1, ck, rt, (id, unique_id), formals, flags2, body)


    
let gec_class (flags, cr, gformals, extends, items) =
    CIL_class(g_canned_srcfile, flags, cr, gformals, extends, items)

let canned_WaitHandle cr =
    [
        gec_CIL_method false cr ([Cilflag_static; Cilflag_hprls], [], g_canned_bool, Cil_cr "WaitOne", [], [], [])
    ]


let canned_Console cr = [
  gec_CIL_method true cr ([Cilflag_static], [], Ciltype_char, Cil_cr "ReadKey", [], [], 
        [
                gec_ins(Cili_ldc(Cil_suffix_i4, Cil_suffix_1));
                gec_ins(Cili_ret)
        ]);


  //0
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr  "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine", [], [], []);

  //1
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr  "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine", [gec_Cil_fp(Ciltype_string, "sarg")], [], []);

  //2
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr  "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine", [gec_Cil_fp(Ciltype_string, "sarg");  gec_Cil_fp(g_ctl_object_ref, "arg")], [], []);

  //3
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine",
             [gec_Cil_fp(Ciltype_string, "sarg"); 
              gec_Cil_fp(g_ctl_object_ref, "arg1"); 
              gec_Cil_fp(g_ctl_object_ref, "arg2")], [], []);

  //4
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr  "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine", 
                              [gec_Cil_fp(Ciltype_string, "sarg");
                               gec_Cil_fp(g_ctl_object_ref, "arg1");
                               gec_Cil_fp(g_ctl_object_ref, "arg2");
                                gec_Cil_fp(g_ctl_object_ref, "arg3")], [], []);
  //5
  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr  "hpr_writeln"], [], CTL_void, Cil_cr "WriteLine", 
                              [gec_Cil_fp(Ciltype_string, "sarg");
                               gec_Cil_fp(g_ctl_object_ref, "arg1");
                               gec_Cil_fp(g_ctl_object_ref, "arg2");
                               gec_Cil_fp(g_ctl_object_ref, "arg3");                               
                                gec_Cil_fp(g_ctl_object_ref, "arg4")], [], []);

  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr "hpr_write" ], [], CTL_void, Cil_cr "Write",
                             [gec_Cil_fp(Ciltype_string, "sarg")], [], []);


  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr   "hpr_write" ], [], CTL_void, Cil_cr "Write",              
                             [gec_Cil_fp(Ciltype_string, "sarg");
                              gec_Cil_fp(g_ctl_object_ref, "arg1")], [], []);

  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr   "hpr_write" ], [], CTL_void, Cil_cr "Write",              
                             [gec_Cil_fp(Ciltype_string, "sarg");
                              gec_Cil_fp(g_ctl_object_ref, "arg1");
                              gec_Cil_fp(g_ctl_object_ref, "arg2")], [], []);

  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr   "hpr_write" ], [], CTL_void, Cil_cr "Write",              
                             [gec_Cil_fp(Ciltype_string, "sarg");
                              gec_Cil_fp(g_ctl_object_ref, "arg1");
                              gec_Cil_fp(g_ctl_object_ref, "arg2");
                              gec_Cil_fp(g_ctl_object_ref, "arg3")], [], []);

  gec_CIL_method true cr ([Cilflag_static; Cilflag_hpr   "hpr_write" ], [], CTL_void, Cil_cr "Write",              
                             [gec_Cil_fp(Ciltype_string, "sarg");
                              gec_Cil_fp(g_ctl_object_ref, "arg1");
                              gec_Cil_fp(g_ctl_object_ref, "arg2");
                              gec_Cil_fp(g_ctl_object_ref, "arg3");                              
                              gec_Cil_fp(g_ctl_object_ref, "arg4")], [], []);
]


(* Whats the difference between ThreadStart and Thread.Start ? 
 * The former is a delegate to with the start whereas the latter is the 
 * invocation of the it ? 
 *
 *
 *)
let canned_Thread_ctor(cr) =
    gec_CIL_method false cr ([Cilflag_instance], [], g_ctl_object_ref, Cil_cr ".ctor", 
               [gec_Cil_fp(g_ctl_object_ref, "what")],
               [], 
               [
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_1);
                        gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "entryp", [])))
                        ])


let canned_Thread cr =
    let sys_threading_thread = cr
    let gct = gec_CIL_method false cr ([Cilflag_static; Cilflag_hprls], [], sys_threading_thread, Cil_cr("get_CurrentThread"),[], [], [])

    let ep_t = gec_cil_crl [ "System"; "Threading"; "ThreadStart" ] // Canned but not 'built-in'
    let gmtid = gec_CIL_method false cr ([Cilflag_instance; Cilflag_hprls], [], g_canned_i32, Cil_cr("get_ManagedThreadId"),[], [], [])
    in
     [
        gmtid;
        CIL_field(None, [], ep_t, Cil_cr "entryp", Cil_field_none, []);
        canned_Thread_ctor(sys_threading_thread);
        gec_CIL_method true cr ([Cilflag_hprls; Cilflag_instance], [], CTL_void, Cil_cr "Start", [], [], []);
        gec_CIL_method true cr ([Cilflag_hprls; Cilflag_instance], [], CTL_void, Cil_cr "Start", 
                              [gec_Cil_fp(g_ctl_object_ref, "argval")], [], []);
        gct;
     ]


// System.Threading.ThreadStart.ctor
let ThreadStart_ctor(cr) =
    gec_CIL_method false cr ([Cilflag_instance], [], g_ctl_object_ref, Cil_cr ".ctor", 
               [gec_Cil_fp(g_ctl_object_ref, "object");
                gec_Cil_fp(CTL_reflection_handle["method"], "method")],
               [],
               [
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_1);
                        gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "object", [])));
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_2);
                        gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "method", [])))
                        ])

(*
 * 
 * This is the same as the one above: really should inherit from a common parent.
 *)
let ParameterizedThreadStart_ctor(cr) =
    gec_CIL_method false cr ([Cilflag_instance], [], g_ctl_object_ref, Cil_cr ".ctor", 
               [gec_Cil_fp(g_ctl_object_ref, "object");
                gec_Cil_fp(g_canned_i32, "method")],
               [], 
               [
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_1);
                        gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "object", [])));
                        gec_ins(Cili_ldarg Cil_suffix_0);
                        gec_ins(Cili_ldarg Cil_suffix_2);
                        gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "method", [])))
                        ])


let canned_hpr_testandset cr =  // not used - can be in Kiwic.cs if really needed.
         gec_CIL_method true cr ([Cilflag_hpr "hpr_testandset"; Cilflag_static], [],
               g_canned_bool, Cil_cr "hpr_testandset",
               [gec_Cil_fp(g_ctl_object_ref, "mutex");
                gec_Cil_fp(g_canned_bool, "avale")],
               [Cilflag_cil; Cilflag_managed],
               [])
 

let canned_hpr_next cr = 
         gec_CIL_method true cr ([Cilflag_hprls; Cilflag_static], [],
               g_ctl_object_ref, Cil_cr "hpr_next", [gec_Cil_fp(g_ctl_object_ref, "arg")],
               [Cilflag_cil; Cilflag_managed],
               [])


let canned_hpr_prev cr = 
         gec_CIL_method true cr ([Cilflag_hprls; Cilflag_static], [],
               g_ctl_object_ref, Cil_cr "hpr_prev", [gec_Cil_fp(g_ctl_object_ref, "arg")],
               [Cilflag_cil; Cilflag_managed],
               [])
 

let canned_hpr_fell cr = 
         gec_CIL_method true cr ([Cilflag_hprls; Cilflag_static], [],
               g_canned_bool, Cil_cr "hpr_fell", [gec_Cil_fp(g_canned_bool, "arg")],
               [Cilflag_cil; Cilflag_managed],
               [])


let canned_hpr_rose cr = 
         gec_CIL_method true cr ([Cilflag_hprls; Cilflag_static], [],
               g_canned_bool, Cil_cr "hpr_rose", [gec_Cil_fp(g_canned_bool, "arg")],
               [Cilflag_cil; Cilflag_managed],
               [])

let g_hprls_primitives = Cil_lib(Cil_cr "KiwiSystem", Cil_cr "hprls_primitives")
    
let canned_orangelib =
   let cr = g_hprls_primitives
   [
        canned_hpr_testandset cr;
        canned_hpr_next cr;
        canned_hpr_prev cr;
        canned_hpr_rose cr;
        canned_hpr_fell cr;
        gec_CIL_method true cr ([Cilflag_static; Cilflag_hprls], [], CTL_void, Cil_cr "hpr_unroll", [], [], []);
        //gec_CIL_method true cr ([Cilflag_static; Cilflag_hprls], [], CTL_void, Cil_cr "hpr_pause", [], [], []);        
        gec_CIL_method true cr ([Cilflag_static; Cilflag_hprls], [], CTL_void, Cil_cr "hpr_barrier", [], [], []);
        //gec_CIL_method true cr ([Cilflag_static; Cilflag_hprls], [], CTL_void, Cil_cr "hpr_pause", [gec_Cil_fp(g_canned_i32, "pausemode")], [], []);
    ]


#if NOLONGER_HERE
let canned_Monitor cr =
    [
        canned_Enter cr;
        canned_Exit cr;
        canned_Wait cr;
        // Pulse/PulseAll are currently a NOP since in truely parallel hardware there is no overhead to polling.
        gec_CIL_method true cr ([Cilflag_static], [], CTL_void, Cil_cr "Pulse", [gec_Cil_fp(g_ctl_object_ref, "mutex")], [], []);
        gec_CIL_method true cr ([Cilflag_static], [], CTL_void, Cil_cr "PulseAll", [gec_Cil_fp(g_ctl_object_ref, "mutex")], [], []);
    ]
#endif    


let canned_RuntimeHelpers cr = [
        gec_CIL_method true cr ([Cilflag_hprls; Cilflag_static], [],
                   CTL_void, Cil_cr "InitializeArray",
                   [gec_Cil_fp(Cil_cr_dot(Cil_cr "System", "Array"), "array");
                    gec_Cil_fp(g_ctl_object_ref, "handle")],
                   [], [])
 ]

let canned_CompilerServices cr =
    let cr = Cil_cr_dot(cr, "RuntimeHelpers")
    [
        gec_class([Cilflag_hprls; Cilflag_autocons 1 ], Cil_cr "RuntimeHelpers", [], None, canned_RuntimeHelpers cr)
    ]

let canned_Runtime cr =
    let cr =  Cil_cr_dot(cr, "CompilerServices")
    [
        gec_class([Cilflag_hprls; Cilflag_autocons 1 ], Cil_cr "CompilerServices", [], None, canned_CompilerServices cr)
    ]



//  (["ThreadStart+co"], ref (4*3), ref [("object", (0, 4)), ("method", (4, 4))]);


// System.Threading
let canned_threading cr0 =
    let ts_name = Cil_cr_dot(cr0, "ThreadStart")
    let ts = gec_class([], Cil_cr "ThreadStart", [], None, 
                            [
                                CIL_field(None, [], g_ctl_object_ref, Cil_cr "object", Cil_field_none, []);
                                CIL_field(None, [], g_ctl_object_ref, Cil_cr "method", Cil_field_none, []);
                                ThreadStart_ctor(ts_name);
                            ])


    let bcr = gec_CIL_method true cr0 ([Cilflag_hprls; Cilflag_static], [], CTL_void, Cil_cr("BeginCriticalRegion"),[], [], [])

    let ecr = gec_CIL_method true cr0 ([Cilflag_hprls; Cilflag_static], [], CTL_void, Cil_cr("EndCriticalRegion"),[], [], [])

 
    let pts_name = Cil_cr_dot(cr0, "ParameterizedThreadStart")
    let pts = gec_class([], Cil_cr "ParameterizedThreadStart", [], None, 
                            [ // This is an extension of ThreadStart - so start the same.
                                CIL_field(None, [], g_ctl_object_ref, Cil_cr "object", Cil_field_none, []);
                                CIL_field(None, [], g_ctl_object_ref, Cil_cr "method", Cil_field_none, []);
                                ParameterizedThreadStart_ctor(pts_name)])

    [
      gec_class([], Cil_cr "Thread", [], None, canned_Thread(Cil_cr_dot(cr0, "Thread")));
      ts; pts; bcr; ecr; 
      //gec_class([], Cil_cr "Monitor", None, canned_Monitor ...);
      gec_class([Cilflag_hprls], Cil_cr "WaitHandle", [], None, canned_WaitHandle(Cil_cr_dot(cr0, "WaitHandle")));
      gec_class([Cilflag_hprls; Cilflag_autocons 1 ], Cil_cr "AutoResetEvent", [], None, []);
   ]


// We need these system classes defined, but so far it seems they can be empty... and many are missing anyway, like short
let canned_Int32 = []

let canned_Byte = []

let canned_Boolean = []

let object_items cr = // KiwiC canned members of System.Object
    [
       gec_CIL_method false cr ([Cilflag_hprls;Cilflag_instance],  [], g_canned_bool, Cil_cr "Equals",  [gec_Cil_fp(g_ctl_object_ref, "cfarg")],  [], []);
       gec_CIL_method false cr ([Cilflag_hprls;Cilflag_instance],  [], Cil_cr_idl [ "Type"; "System" ], Cil_cr "GetType",  [],  [], []);
       gec_CIL_method false cr ([Cilflag_hprls;Cilflag_instance],  [], f_canned_c16_str -1, Cil_cr "ToString",  [],  [], []);       
       gec_CIL_method false cr ([Cilflag_instance],                [], CTL_void,           Cil_cr ".ctor",     [],  [], []); // A vanilla, empty constructor.
    ]

let gen_Object_class = object_items (* Two different routes to same library ?*)


let g_object_overhead = 4L

let g_canned_object = CTL_record(g_core_object_path, g_canned_object_struct, [], g_object_overhead, [], [], None)

//  Read a char from a string (as in test9) KiwiC System/String/get_Chars. This method name is generated by C# compiler.
let canned_get_Chars_body = 
    [
        gec_ins(Cili_ldarg Cil_suffix_0);
        gec_ins(Cili_ldarg Cil_suffix_1);
        gec_ins(Cili_ldelem Ciltype_char);
        gec_ins(Cili_ret);
    ]


// To support simple functions like Max, there are 3 possible approaches:
//  1  Give definitions in Kiwic.cs that are expaned in-line
//  2  Can the bytecode in here
//  3  Project them down to the native ones in hpr_library.    
// A critical aspect is that we need to appreciate their referential transparency to facilitate constant folding the lasoo stems.
// The constant meet algorithm used by KiwiC handles control flow properly, so there is no benefit to having these canned in here.
// Another aspect is that lazy fetching of code from Kiwic.dll was not working, so putting implementations in there inflates all compiles.
// This implementation uses style 2 for now.
// Although max,min,abs etc are canned, there is no hprls flag since we want to treat its inlined body as though it came in via C#.
// Their args are for int only - where is the floating point one?    
let cil_max cr =
    gec_CIL_method false cr ([Cilflag_public; Cilflag_static; Cilflag_hidebysig],
               [],
               g_canned_i32, Cil_cr("Max"),
               [gec_Cil_fp(g_canned_i32, "maxa"); gec_Cil_fp(g_canned_i32, "maxb")],
               [Cilflag_cil; Cilflag_managed],
               [gec_ins(Cili_lab("IL_0000"));
                gec_ins(Cili_ldarg(Cil_suffix_0));
                gec_ins(Cili_lab("IL_0001"));
                gec_ins(Cili_ldarg(Cil_suffix_1));
                gec_ins(Cili_lab("IL_0002"));
                gec_ins(Cili_ble("IL_000d"));
                gec_ins(Cili_lab("IL_0007"));
                gec_ins(Cili_ldarg(Cil_suffix_0));
                gec_ins(Cili_lab("IL_0008"));
                gec_ins(Cili_br("IL_000e")); 
                gec_ins(Cili_lab("IL_000d"));
                gec_ins(Cili_ldarg(Cil_suffix_1));
                gec_ins(Cili_lab("IL_000e"));
                gec_ins(Cili_ret)]); 

let cil_min cr =
    gec_CIL_method false cr ([Cilflag_public; Cilflag_static; Cilflag_hidebysig],
               [], g_canned_i32,
               Cil_cr("Min"),
               [gec_Cil_fp(g_canned_i32, "a2");
                gec_Cil_fp(g_canned_i32, "b")],
               [Cilflag_cil; Cilflag_managed],
               [gec_ins(Cili_lab("IL_0000"));
                gec_ins(Cili_ldarg(Cil_suffix_0)); 
                gec_ins(Cili_lab("IL_0001"));
                gec_ins(Cili_ldarg(Cil_suffix_1));
                gec_ins(Cili_lab("IL_0002"));
                gec_ins(Cili_bge("IL_000d"));
                gec_ins(Cili_lab("IL_0007"));
                gec_ins(Cili_ldarg(Cil_suffix_0));
                gec_ins(Cili_lab("IL_0008"));
                gec_ins(Cili_br("IL_000e"));
                gec_ins(Cili_lab("IL_000d"));
                gec_ins(Cili_ldarg(Cil_suffix_1));
                gec_ins(Cili_lab("IL_000e"));
                gec_ins(Cili_ret)]); 

let cil_abs cr = // System.Math.Abs - surely better in Kiwic.dll now?  No. Put in KiwiSystem.Math and add a library redirection.
    gec_CIL_method false cr (
        [Cilflag_public; Cilflag_static; Cilflag_hidebysig],
        [],
        g_canned_i32, 
        Cil_cr("Abs"),
        [gec_Cil_fp(g_canned_i32, "a3")],
        [Cilflag_cil; Cilflag_managed],
        [gec_ins(Cili_lab("IL_0000"));
         gec_ins(Cili_ldarg(Cil_suffix_0));
         gec_ins(Cili_lab("IL_0001"));
         gec_ins(Cili_ldc(Cil_suffix_i4, Cil_suffix_0));
         gec_ins(Cili_lab("IL_0002"));
         gec_ins(Cili_bge("IL_000e"));
         gec_ins(Cili_lab("IL_0007"));
         gec_ins(Cili_ldarg(Cil_suffix_0));
         gec_ins(Cili_lab("IL_0008")); 
         gec_ins(Cili_neg);
         gec_ins(Cili_lab("IL_0009"));
         gec_ins(Cili_br("IL_000f"));
         gec_ins(Cili_lab("IL_000e"));
         gec_ins(Cili_ldarg(Cil_suffix_0)); 
         gec_ins(Cili_lab("IL_000f"));
         gec_ins(Cili_ret)])

 
// Structs extend this type. It is a struct and not a class.
let canned_SystemValueType_contents cr = [ ]


// This just needs to exist to avoid unknown type identifier when declaring child exceptions.
let old_canned_SystemException_contents cr =
    [

       gec_CIL_method false cr ([Cilflag_instance],                [], CTL_void,           Cil_cr ".ctor",     [],  [], []); // A vanilla, empty constructor.
    ]    







let canned_Microsoft cr0 =
    [
        //gec_class([], Cil_cr "CSharp", [], None, canned_Microsoft_CSharp (Cil_cr_dot(cr0, "CSharp")))
    ]
    
let canned_Math cr =
    [
        cil_abs cr;
        cil_min cr;
        cil_max cr;
    ]

// The following wrapper classes are provided by the system to create function and action delegates.
// Examples:
//     callvirtual default true [System]Void [System]Action`1<(valuetype [System]Int32)>::Invoke([] !0)
//     callvirtual default true !1 [System]Func`2<(valuetype [System]Int32), (valuetype [System]Int32)>::Invoke([] !0)
//
// These could be just as easily in Kiwic.cs but would have 8 to change on every edit. So instead we macro generate them here.
//
let canned_System_Func_or_Action name ret_arity arg_arity cr =
    let gin n = CTVar{ g_def_CTVar with idx=Some n; }
    let goz n = Cil_tp_arg(false, n, sprintf "QT%i" n)
    let fap = map goz [0 .. ret_arity+arg_arity-1]
    let args = map (fun n -> gec_Cil_fp(gin n, sprintf "arg%i" n)) [0 .. arg_arity - 1]
    let eis = ret_arity = 0 // Tacitly assume actions are eis and functions are not ...
    let rt = (if ret_arity = 0 then CTL_void else gin arg_arity) // last one is return type
    let closure = CIL_field(None, [Cilflag_private], g_ctl_object_ref, Cil_cr("closure"), Cil_field_none, [])
    let ep = CIL_field(None, [Cilflag_private], g_IntPtr, Cil_cr("entry_point"), Cil_field_none, []) 
    let ctor = gec_CIL_method false cr ([Cilflag_instance], [], g_ctl_object_ref, Cil_cr ".ctor", [gec_Cil_fp(g_ctl_object_ref, "p_closure"); gec_Cil_fp(g_IntPtr, "p_ep")], [],
                    [
                      gec_ins(Cili_ldarg Cil_suffix_0); 
                      gec_ins(Cili_ldarg Cil_suffix_1);
                      gec_ins(Cili_stfld(g_ctl_object_ref, (cr, "closure", [])));
                      gec_ins(Cili_ldarg Cil_suffix_0); 
                      gec_ins(Cili_ldarg Cil_suffix_2);
                      gec_ins(Cili_stfld(g_IntPtr, (cr, "entry_point", [])));
                    ])
    let m = if ret_arity = 1 then "KiwiC-FInvoke" else "KiwiC-ActionInvoke"
    let invoke =  gec_CIL_method eis cr ([Cilflag_hpr m; Cilflag_instance], fap, rt, Cil_cr "Invoke", args, [], []);

    [ closure; ep; ctor; invoke ] // There are these four components in each wrapper.  The method and field names are hardcoded elsewhere as well.


(* These need to appear both under System/String for mono and mscorlib/System/String for MS .net *)
let canned_String cr =
 [
   CIL_field(None, [Cilflag_literal; Cilflag_static],
             Ciltype_string, Cil_cr("Empty"), Cil_field_lit(Cil_string ""), []);
     
   // static method with 2 args: prototype is/was in Kiwic.cs but we need to enter in the table as well
   gec_CIL_method false cr ([Cilflag_hpr "hpr_concat"; Cilflag_static], [], Ciltype_string, Cil_cr "Concat",
              [gec_Cil_fp(Ciltype_string, "str1"); gec_Cil_fp(Ciltype_string, "b")], [], []);

   // static method implementations with 3 and 4 args are in Kiwic.cs 

   // This second version of concat is a built-in instance method.
   gec_CIL_method false cr ([Cilflag_hpr "hpr_concat"; Cilflag_instance], [], Ciltype_string, Cil_cr "Concat", [gec_Cil_fp(Ciltype_string, "b")], [], []);

   //  Read a char from a string (as in test9) KiwiC System/String/get_Chars. This method name is generated by C# compiler.
   gec_CIL_method false cr ([Cilflag_instance], [], Ciltype_char, Cil_cr "get_Chars", [gec_Cil_fp(g_canned_i32, "idx")], [], canned_get_Chars_body); // There is no hprls flag on this one since we want to treat its inlined body as though it came in via C#.

   //  Get length propert for a string. This method name is generated by C# compiler.
   gec_CIL_method false cr ([Cilflag_hprls; Cilflag_instance], [], g_canned_i32, Cil_cr "get_Length", [], [], []);
]


// 
let canned_MulticastDelegate cr = 
    let that = "closure" // should this be "that" - or is it ok as "closure" as in Func and Action ?
    [
      CIL_field(None, [Cilflag_private], g_ctl_object_ref, Cil_cr that, Cil_field_none, []);
      CIL_field(None, [Cilflag_private], CTL_reflection_handle["method"], Cil_cr("entry_point"), Cil_field_none, []); 
      gec_CIL_method false cr ([Cilflag_instance; Cilflag_private; Cilflag_hidebysig; Cilflag_specialname; Cilflag_rtspecialname], [], CTL_void, Cil_cr(".ctor"), 
                 [gec_Cil_fp(g_ctl_object_ref, "obj"); gec_Cil_fp(g_canned_i32, "method")],
                 [Cilflag_cil; Cilflag_managed],
                 [
                  
                  gec_ins(Cili_ldarg Cil_suffix_0); 
                  gec_ins(Cili_ldarg(Cil_suffix_1));
                  gec_ins(Cili_stfld(g_ctl_object_ref, (Cil_cr "MulticastDelegate", that, [])));
                  
                  gec_ins(Cili_ldarg(Cil_suffix_0));
                  gec_ins(Cili_ldarg(Cil_suffix_2));
                  gec_ins(Cili_stfld(CTL_reflection_handle["method"], (Cil_cr "MulticastDelegate", "entry_point", [])));
                  
                  gec_ins(Cili_ret)])
  ]


let canned_Attribute cr = [ ];
let canned_Enum cr = [ ];
let canned_ApplicationException cr = [];


let canned_CollectionsGeneric cr =
   let cr = Cil_cr_dot(cr, "IEnumerator`1")
   //let tparg = CTVar{ g_def_CTVar with idx=Some 0; }
   let tparg = Cil_tp_arg(false, 0, "TA0")
   [
        gec_class([], cr, [tparg], None, 
                  [
                    gec_CIL_method true cr ([Cilflag_instance], [], g_canned_bool, Cil_cr "MoveNext", [], [], []);
                    gec_CIL_method false cr ([Cilflag_instance], [], (CTVar{ g_def_CTVar with idx=Some 0; }), Cil_cr "get_Current", [], [], []);
                  ])

   ]

let canned_Collections cr =
   let cr1 = Cil_cr_dot(cr, "Generic") 
   let cr2 = Cil_cr_dot(cr, "IEnumerator")
   [
     gec_class([], cr1, [], None, canned_CollectionsGeneric cr1);

     gec_class([], cr2, [], None, 
        [
          gec_CIL_method true cr2 ([Cilflag_instance], [], g_canned_bool, Cil_cr "MoveNext", [], [], []);
        ]);
  ]



//
//
//
let canned_valuetypes cr0 = // cr0 is "System"
    let vd = 4
    let parent = g_core_object_lib // TODO double check that it is correct for valuetypes to inherit object. Seems a little odd.
    let create_valuetype_class (idl, ct, classitems) =
        let crf = Cil_cr_dot(cr0, hd idl)
        let gec_CompareTo ct = gec_CIL_method false crf ([Cilflag_hprls; Cilflag_instance], [], g_canned_i32,  Cil_cr "Equals",    [gec_Cil_fp(ct, "cf_arg")], [], []);
        let gec_Equals    ct = gec_CIL_method false crf ([Cilflag_hprls; Cilflag_instance], [], g_canned_bool, Cil_cr "CompareTo", [gec_Cil_fp(ct, "cf_arg")], [], []);        

        // Note: these should be declared as structs and not as classes infact. TODO change over once structs are better implemented.

        let valuetype_universals = function
            | Some ct -> [ gec_Equals ct; gec_CompareTo ct]
            | None -> []
        let items = valuetype_universals ct @ valOf_or_nil classitems
        if vd>=4 then vprintln 4 (sprintf "kiwi_canned: Created valuetype class %s with %i members" (cil_typeToStr crf) (length items))
        gec_class([ Cilflag_valuetype], crf, [], Some parent, items)
    map create_valuetype_class
             [
              ( [ "Array" ],   None,                                              None); // Not a valuetype actually!
              ( [ "Decimal" ], Some(CTL_net(false, 128, FloatingPoint, g_native_net_at_flags)),  None);           
              ( [ "Double" ],  Some g_canned_double,                              None);
              ( [ "Int8" ],    Some(CTL_net(false, 8, Signed, g_native_net_at_flags)),  None); 
              ( [ "Int16" ],   Some g_canned_i16,                                 None);
              ( [ "Int32" ],   Some g_canned_i32,                                 Some canned_Int32);
              ( [ "Int64" ],   Some g_canned_i64,                                 None);
              //( [ "Int" ],    Some g_canned_i32,                                 None);
              ( [ "UInt8" ],   Some(CTL_net(false, 8, Unsigned, g_native_net_at_flags)), None);
              ( [ "UInt16" ],  Some g_canned_u16,                                 None);
              ( [ "UInt32" ],  Some g_canned_u32,                                 None);
              ( [ "UInt64" ],  Some g_canned_u64,                                 None);
              ( [ "Boolean" ], Some g_canned_bool,                                Some canned_Boolean);
              ( [ "Char" ],    Some g_canned_char,                                None);
              ( [ "Float32" ], Some g_canned_float,                               None);
              ( [ "Float64" ], Some g_canned_double,                              None);
              ( [ "SByte" ],   Some(CTL_net(false, 8, Signed, g_native_net_at_flags)),  None);
              ( [ "Byte" ],    Some(CTL_net(false, 8, Unsigned, g_native_net_at_flags)), Some canned_Byte);
              ( [ "Void" ],    None, None);
             ]

let canned_System_Convert cr0 =
    let parent = Some g_core_object_lib
    let canned_convert_items = 
           [
               // Many basic System.Convert methods are hardwired in KiwiC. But where are the method bodies?  Invoked by Cili_conv ?
               // cf System.BitConverter (for endian-specific xvertion between char arrays that is, for KiwiC use, implemented in C# in Kiwic.cs
              (false, [ "ToInt8" ],    None, g_canned_i8);
              (false, [ "ToInt16" ],   None, g_canned_i16);
              (false, [ "ToInt32" ],   None, g_canned_i32);
              (false, [ "ToInt64" ],   None, g_canned_i64);
              (false, [ "ToInt" ],     None, g_canned_i32);
              (false, [ "ToUInt8" ],   None, g_canned_u8);
              (false, [ "ToUInt16" ],  None, g_canned_u16);
              (false, [ "ToUInt32" ],  None, g_canned_u32);
              (false, [ "ToUInt64" ],  None, g_canned_u64);
              (false, [ "ToBoolean" ], None, g_canned_bool);
              (false, [ "ToChar" ],    None, g_canned_char);

//              (false, [ "ToFloat32" ], None, g_canned_float);
//              (false, [ "ToFloat64" ], None, g_canned_double);
              (false, [ "ToSByte" ],   None, g_canned_u8);
              (false, [ "ToByte" ],    None, g_canned_i8);
           ]

    let func (eis, name, oo, rt) =
        gec_CIL_method eis cr0 ([Cilflag_static; Cilflag_hprls], [], rt, Cil_cr (hd name),
                   [ gec_Cil_fp(g_ctl_object_ref, "arg")],
                   [], [])
    map func canned_convert_items


let gec_fap n = Cil_tp_arg(false, n, sprintf "TX%i" n) // Generate a type formal parameter.


let canned_MemberInfo cr0 =
    let crf = Cil_cr_dot(cr0, "memberInfo")
    [
        gec_CIL_method false crf ([Cilflag_instance; Cilflag_hprls], [], g_ctl_object_ref, Cil_cr "get_Name", [], [], [])
    ]

let canned_Reflection cr0 =
    let cr0 = Cil_cr_dot(cr0, "Reflection")
    let parent = None
    [
      gec_class([], Cil_cr "MemberInfo", [], parent, canned_MemberInfo cr0);
    ]




let canned_SystemType crf =
    [
      gec_CIL_method false crf ([Cilflag_static; g_flag_KiwiC], [], CTL_reflection_handle ["type"], Cil_cr "GetTypeFromHandle", [ gec_Cil_fp(CTL_reflection_handle ["poly"], "handx") ], [], [])
    ]




// System
let canned_System cr0 =
    let parent = Some g_core_object_lib
    canned_valuetypes cr0
    @
    [
        gec_class([], Cil_cr "Reflection", [], parent, canned_Reflection cr0) // Reflection API
        
        gec_class([], Cil_cr "Type", [], parent, canned_SystemType(Cil_cr_dot(cr0, "Type")))  // Needed for Reflection-like/dynamic API

        gec_class([], Cil_cr "RuntimeFieldHandle", [], parent, [])

        gec_class([], Cil_cr "Console", [], parent, canned_Console (Cil_cr_dot(cr0, "Console")))

        gec_class([], Cil_cr "Collections", [], parent, canned_Collections(Cil_cr_dot(cr0, "Collections")));

        gec_class([], Cil_cr "Threading", [], parent, canned_threading(Cil_cr_dot(cr0, "Threading")));

        gec_class([], Cil_cr "Convert", [], parent, canned_System_Convert (Cil_cr_dot(cr0, "Convert")));

        gec_class([], Cil_cr "InvalidOperationException", [], parent, []);

        gec_class([], Cil_cr "Attribute", [], parent, canned_Attribute (Cil_cr_dot(cr0, "Attribute")));

        gec_class([], Cil_cr "IntPtr", [], parent, []);        

        gec_class([], Cil_cr "ApplicationException", [], parent, canned_ApplicationException(Cil_cr_dot(cr0, "ApplicationException")));

        gec_class([], Cil_cr "IFormatProvider", [], parent, [])

        gec_class([], Cil_cr "DateTime", [], parent, [])        

        gec_class([], Cil_cr "TypeCode", [], parent, [])        

        gec_class([], Cil_cr "Enum", [], parent, canned_Enum (Cil_cr_dot(cr0, "Enum")));

        gec_class([], Cil_cr "Math", [], parent, canned_Math (Cil_cr_dot(cr0, "Math")))

        gec_class([], Cil_cr "MulticastDelegate", [], parent, canned_MulticastDelegate(Cil_cr_dot(cr0, "MulticastDelegate")));

        gec_class([], Cil_cr "AsyncCallback", [], parent, []);

        gec_class([], Cil_cr "IAsyncResult", [], parent, []);

        gec_class([], Cil_cr "Object", [], None, gen_Object_class (Cil_cr_dot(cr0, "Object")))

        gec_class([], Cil_cr "Runtime", [], parent, canned_Runtime(Cil_cr_dot(cr0, "Runtime")))

        gec_class([], Cil_cr "String", [], parent, canned_String(Cil_cr_dot(cr0, "String")))

        gec_class([Cilflag_valuetype], Cil_cr "ValueType", [], parent, canned_SystemValueType_contents(Cil_cr_dot(cr0, "ValueType")))

        //Exceptions are now defined in Kiwic.cs not in here.
        //gec_class([], Cil_cr "Exception", [], parent, old_canned_SystemException_contents(Cil_cr_dot(cr0, "Exception")))        

        gec_class([], Cil_cr "Func`1", (map gec_fap [0..0]), parent, canned_System_Func_or_Action "Func`1" 1 0 (Cil_cr_dot(cr0, "Func`1")))
        gec_class([], Cil_cr "Func`2", (map gec_fap [0..1]), parent, canned_System_Func_or_Action "Func`2" 1 1 (Cil_cr_dot(cr0, "Func`2")))
        gec_class([], Cil_cr "Func`3", (map gec_fap [0..2]), parent, canned_System_Func_or_Action "Func`3" 1 2 (Cil_cr_dot(cr0, "Func`3")))
        gec_class([], Cil_cr "Func`4", (map gec_fap [0..3]), parent, canned_System_Func_or_Action "Func`4" 1 3 (Cil_cr_dot(cr0, "Func`4")))

        gec_class([], Cil_cr "Action`0", (map gec_fap []), parent, canned_System_Func_or_Action "Action`0" 0 0 (Cil_cr_dot(cr0, "Action`0")))
        gec_class([], Cil_cr "Action`1", (map gec_fap [0..0]), parent, canned_System_Func_or_Action "Action`1" 0 1 (Cil_cr_dot(cr0, "Action`1")))
        gec_class([], Cil_cr "Action`2", (map gec_fap [0..1]), parent, canned_System_Func_or_Action "Action`2" 0 2 (Cil_cr_dot(cr0, "Action`2")))
        gec_class([], Cil_cr "Action`3", (map gec_fap [0..2]), parent, canned_System_Func_or_Action "Action`3" 0 3 (Cil_cr_dot(cr0, "Action`3")))


]

// No longer needed with this prefix?  Perhaps needed on Windows?
let canned_mscorlib = 
    let cr0 = Cil_cr "mscorlib" 
    in [
          gec_class([], Cil_cr "System", [], None, canned_System(Cil_lib(cr0, Cil_cr "System")))
       ]              


let g_canned_libs =
    [
        gec_class([], g_orangelib, [], None, canned_orangelib);
        //gec_class([Cilflag_partial], Cil_cr "string", [], None, canned_String);
        //gec_class([], Cil_cr "math", [], None, canned_Math);
        //gec_class([], Cil_cr "object", [], None, object_items);
        //gec_class([], Cil_cr "mscorlib", [], None, canned_mscorlib);
        // Attribute and mscorlib/System is/are temporarily added in at the top level while cecil parser class paths are not correct
        gec_class([], Cil_cr "System", [], None, canned_System (Cil_cr "System"))
        gec_class([], Cil_cr "Microsoft", [], None, canned_Microsoft (Cil_cr "Microsoft")) 
        //gec_class([], Cil_cr "Attribute", [], None, canned_Attribute);

    ]




let g_bif_sysexit = 
    { g_baseline_bif with
       uname=     [ "hpr_sysexit"; "hpr_primitives"; "KiwiSystem" ]
       hpr_name=  "hpr_sysexit"
       eis=       true
       arity=     1      
       rt=        Some CTL_void
       hpr_native= Some (snd abstract_hdr.g_hpr_sysexit_fgis) // Why are these not manually added to other ones?  Should be present for all that are not <hardcoded>.
    };

let g_bif_pause0 =
    { g_baseline_bif with
       uname=     [ "Pause"; "Kiwi"; "KiwiSystem" ]
       hpr_name=  "hpr_pause"
       eis=       true
       rt=        Some CTL_void
    }

let g_bif_pause1 = { g_bif_pause0 with arity = 1; }

let g_bif_PauseControlSet =
    { g_baseline_bif with
       uname=     [ "PauseControlSet"; "Kiwi"; "KiwiSystem" ];
       hpr_name=  "hpr_pause_control"
       eis=       true
       arity=     1
       rt=        Some g_canned_i32; // Actually returns enum [ "PauseControl"; "Kiwi" ]
    }

#if OLD
// This is now replaced with a flag inside hpr_PauseFlags
let g_bif_newregion =
    { g_baseline_bif with
       uname=     ["Unroll"; "Kiwi"; "KiwiSystem" ];
       rt=        Some CTL_void;
       eis=true;
       hpr_name=  "<hardcoded>";
    }

// This is now replaced with a flag inside hpr_PauseFlags
let g_bif_nounroll = 
    { g_baseline_bif with
       uname=     [ "NoUnroll"; "Kiwi"; "KiwiSystem" ];
       rt=        Some CTL_void;
       hpr_name=  "<hardcoded>"
       eis=true;
    }
#endif

let g_local_kiwi_bif_table =
   [    
    g_bif_pause0; g_bif_pause1; g_bif_PauseControlSet;

    { g_baseline_bif with
       uname=     ["Convert"; "Binder"; "RuntimeBinder"; "CSharp"; "Microsoft"]
       arity=     3
       hpr_name=  "<hardcoded>"
       rt=        Some(Cil_cr_idl[ "CallSiteBinder"; "CompilerServices"; "Runtime"; "System" ])
    };

    { g_baseline_bif with
       uname=     ["BinaryOperation"; "Binder"; "RuntimeBinder"; "CSharp"; "Microsoft"]
       arity=     4
       hpr_name=  "<hardcoded>"
       rt=        Some(Cil_cr_idl[ "CallSiteBinder"; "CompilerServices"; "Runtime"; "System" ])
    };

    { g_baseline_bif with
       uname=     ["KiwiDynDispWrap"; "CallSite`1"; "CompilerServices"; "Runtime"; "System"]
       arity=     1
       hpr_name=  "<hardcoded>" 
       rt=        Some (CTL_reflection_handle [ "KiwiCallSite" ])
    };

    { g_baseline_bif with
       uname=     [ "KPragma"; "Kiwi"; "KiwiSystem" ]
       arity=     2
       hpr_name=  "<hardcoded>" 
       rt=       Some g_canned_i32
    };

    { g_baseline_bif with
       uname=     [ "KPragma"; "Kiwi"; "KiwiSystem" ]
       arity=     3
       hpr_name=  "<hardcoded>" 
       rt=Some g_canned_i32
    };

    { g_baseline_bif with
       uname=     [ "KPragma"; "Kiwi"; "KiwiSystem" ]
       arity=     4
       hpr_name=  "<hardcoded>" 
       rt=Some g_canned_i32
    };


    { g_baseline_bif with
       uname=     ["op_Equality"; "String"; "System" ] // Do we need other implementations of op_Equality?
       hpr_name=  "<hardcoded>"
       arity=     2
       rt=        Some g_canned_bool
    };


    { g_baseline_bif with
       uname=     [ "ToCharArray"; "String"; "System" ] // Convert a string to a character array so it can be indexed.
       arity=     0 // Instance method - the arity 0 is beyond the implicit 'this'.
       instancef= true
       rt=        Some (f_canned_c16_str 0)
       hpr_name=  "<hardcoded>"
    };

    { g_baseline_bif with // Exists with and without instancef.
       uname=     [ "get_ManagedThreadId";  ]
       rt=        Some g_canned_i32
       hpr_name=  "<hardcoded>"
    };

    { g_baseline_bif with // Exists with and without instancef - old and new API.
       instancef= true;
       uname=     [ "get_ManagedThreadId"; "Thread"; "Threading"; "System" ]
       rt=        Some g_canned_i32
       hpr_name=  "<hardcoded>"
    };

    { g_baseline_bif with
       uname=     [ "KiwiC-FInvoke" ]
       rt=        Some g_ctl_object_ref // polymorphic place holder - value comes from a generic  - we need a cleaner way of denoting this also may be a valuetype so CT_unkown may be preferable.
       hpr_name=  "<hardcoded>" // Dissapears during kcode symbolic eval
    };

    { g_baseline_bif with
       uname=     [ "KiwiC-ActionInvoke" ]
       hpr_name=  "<hardcoded>" // Dissapears during kcode symbolic eval
    };

    { g_baseline_bif with
       uname=     [ "inHardware"; "Kiwi"; "KiwiSystem" ]
       rt=        Some g_canned_bool
       hpr_name=  "<hardcoded>"
    };

    { g_baseline_bif with
       uname=     [ "KppMarkPrimtive"; "Kiwi"; "KiwiSystem" ]
       rt=        Some CTL_void
       hpr_name=  "hpr_KppMark" // This is used to label waypoints and other control-flow nodes.
       arity=     1 // There are arity 3, 2 and 1 versions of this.
       eis=       true
    };
    
    { g_baseline_bif with
       uname=     [ "KppMarkPrimitive"; "Kiwi"; "KiwiSystem" ]
       rt=        Some CTL_void
       hpr_name=  "hpr_KppMark" // This is used to label waypoints and other control-flow nodes.
       arity=     2 // There are arity 3, 2 and 1 versions of this.
       eis=       true
    };

    
    { g_baseline_bif with
       uname=     [ "KppMarkPrimitive"; "Kiwi"; "KiwiSystem" ]
       rt=        Some CTL_void
       hpr_name=  "hpr_KppMark" // This is used to label waypoints and other control-flow nodes.
       arity=     3 // There are arity 3, 2 and 1 versions of this.
       eis=       true
    };

    { g_baseline_bif with
       uname=     [ "Postdom"; "Kiwi"; "KiwiSystem" ]
       hpr_name=  "system-postdom" // hardcoded (in KiwiC) I think? or else in bevelab?
    };

    { g_baseline_bif with
      // Invoked by a C# using block when variables go out of scope.
       uname=     [ "Dispose"; "IDisposable"; "System" ]
       instancef= true
       hpr_name=  "<hardcoded>"
    };


    { g_baseline_bif with
       uname=     [ "Dispose"; "Kiwi"; "KiwiSystem" ]
       hpr_name=  "<hardcoded>" // hardcoded in KiwiC.
       arity=     1
       eis=       true
    };


    { g_baseline_bif with
       uname=     [ "handleArith"; ]
       arity=     3
       instancef= true // Not static
       hpr_name=  "<hardcoded>" 
       rt=        Some g_ctl_object_ref // Return type is cast in CIL code.
    };


    { g_baseline_bif with                             // This is the attribute getter.
       uname=     [ "get_Length"; "Array"; "System" ] // We can take the length of the native types 1-D array and string.
       arity=     0
       instancef= true // Not static
       hpr_name= "<hardcoded>"; 
       rt=Some g_canned_i32
    };

    { g_baseline_bif with                            // This is used normally for 2-D arrays but could be used with an arg of zero on 1-D arrays as an alias for get_Length
       uname=     [ "GetLength"; "Array"; "System" ] // TODO - check whether this is fully implemented in Kiwic.cs ?
       arity=     1
       instancef= true; // Not static
       hpr_name= "<hardcoded>"; 
       rt=Some g_canned_i32;
    };

    { g_baseline_bif with                            //
       uname=     [ "GetLength"; "Array"; "System" ] // TODO - check whether this is fully implemented in Kiwic.cs ?
       arity=     0 // Not sure if this is used? 
       instancef= true // Not static
       hpr_name= "<hardcoded>"; 
       rt=Some g_canned_i32;
    };

    { g_baseline_bif with                           
       uname=     [ "get_Rank"; "Array"; "System" ]  // Return the number of dimensions - this will be for 1-D arrays that are canned here and so returns 1.
       arity=     0
       instancef= true; // Not static
       hpr_name=  "<hardcoded>" 
       rt=        Some g_canned_i32
    };

    { g_baseline_bif with                           
       uname=     [ "size"; "ObjectHandler`1"; "Kiwi"; "KiwiSystem" ] // Kiwi2 object getsize backdoor (or else use unsafe C# to do the same).
       arity=     0
       instancef= true // Not static
       hpr_name= "<hardcoded>" 
       rt=       Some g_canned_i32
    };

    { g_baseline_bif with                           
       uname=     [ "handleArith"; "ObjectHandler`1"; "Kiwi"; "KiwiSystem" ] // Kiwi2 object getsize backdoor (or else use unsafe C# to do the same).
       arity=     2
       instancef= true // Not static
       hpr_name= "<hardcoded>" 
       rt=       Some g_canned_i32
    };

    { g_baseline_bif with                           
       uname=     ["get_Name"; "MemberInfo"; "Reflection"; "System"]
       arity=     0
       instancef= true // Not static
       hpr_name= "<hardcoded>" 
       rt=       Some Ciltype_string
    };

#if REMOVED
    { g_baseline_bif with                           
       uname=      [ "hpr_atomic_add"; "Interlocked"; "KiwiSystem"]
       arity=      2
       instancef=  false// Static
       hpr_name=   "hpr_atomic_add"
       rt=         Some g_canned_i64  // Although defined with 64-bit args, any width and precision is supported.
       eis=        true // eis flag is in fsems now - not needed here - please delete
       hpr_native= Some (snd abstract_hdr.g_hpr_atomic_add_fgis) // Why are these not manually added to other ones?
    };

    { g_baseline_bif with                           
       uname=      [ "hpr_exchange"; "Interlocked"; "KiwiSystem"]
       arity=      4
       instancef=  false// Static
       hpr_name=   "hpr_exchange" 
       rt=         Some g_canned_i64 // Although defined with 64-bit args, any width and precision is supported.
       eis=        true
       hpr_native= Some(snd abstract_hdr.g_hpr_exchange_fgis)
    };
#endif
    g_bif_sysexit;


] 

// These method names are somewhat magical - they are present in some user classes with Cilflag_runtime markup but the full name cannot be held in bif table because it is application-specific.
let g_hybrid_bif_method_names = [ "Invoke"; "BeginInvoke"; "EndInvoke" ]

let g_hpr_notneg_pseudo_gis =
    { g_default_native_fun_sig with fsems=abstract_hdr.l_reftran; rv=abstract_hdr.g_signed_prec; args=[abstract_hdr.g_signed_prec] }


let g_bif_not = // The primitives neg and not are operators in hpr_ form, not functions.  
     { g_baseline_bif with
        uname=    [ "*system-not*" ] // One's complement - bitwise complement.
        rt=Some   g_canned_i32  // This has identity type transfer
        identity_types= true
        arity=    1
        hpr_name= "<native-operator>"
        hpr_native= Some g_hpr_notneg_pseudo_gis
     }

let g_bif_neg =
     { g_baseline_bif with
        uname=    ["*system-neg*"; ]  // Subtract from zero - two's complement.
        rt=Some   g_canned_i32; // possibly neg i64 is also needed? TODO test it.
        arity=    1
        hpr_name= "<native-operator>"
        hpr_native= Some g_hpr_notneg_pseudo_gis
     }

let g_bif_ldlen = // cf strlen
     { g_baseline_bif with
        uname=    [ "ldlen"; ]
        hpr_name= "<hardcoded>" // Implemented in KiwiC recipe stage
        rt=       Some g_canned_i32  // Bounded size for 32-bit machines
        arity=    1
     }

let g_bif_hpr_alloc =
     { g_baseline_bif with       // This one is for Kiwi 1+ DRAM allocation.  
        uname=    [ "new"; ]     // C# will convert everyday 'new' calls into Cili_newobj that are handled elsewhere and matched with Kiwi.Dispose() in the absence of deallocation inference.
        hpr_name= "hpr_alloc"    // Should be <hardcoded> since this is a call through to an hpr native call?
        rt=       Some g_canned_pointer_type
        arity=    1
        eis=      true           // Not referentially transparent
     }

let g_bif_strlen =
     {
       g_baseline_bif with
         hpr_name=   "hpr_strlen"
         rt=         Some g_canned_i32
         arity=      1
         uname=      [ "get_Length"; "String"; "System" ]  
     }

// BIF=built-in function: create table for BIF lookup.
// Insert the local canned built-in-functions in the global index table.
let g_kiwi_canned_bif =
    let ins (m:kiwi_bif_table_t) v =
        //vprintln 0 (sprintf "Installed bif %s instancef=%A arity %i" (htos v.uname) v.instancef v.arity)
        m.Add((v.uname, v.instancef, v.arity), v)
    List.fold ins !proto_kiwi_canned_bif_table g_local_kiwi_bif_table



type cil_blkmacro_t =
    {
        ikey:           string
        sptr_or_vale:   int
        dptr:           int
        count:          int
    }

// Macro-expand the cpblk or initblk etc CIL instruction to a sequence of CIL.
//     initblk(dest, vale, count)
//     cpblk(dest, src, count)    
let cpblk_initblk_render bm kind =
    let lr n = Cil_argexp(Cil_number32 n)
    let mm = sprintf "Macro-expanded cpblk/initblk instruction %s" bm.ikey
    let continue_lab = bm.ikey + "_continue"
    let break_lab = bm.ikey + "_break"    
    let dt = g_canned_i8
    let unstack =
        [
            Cili_comment ("Start of " + mm) 
            Cili_stloc (lr bm.count) // Pop args in reverse order of 
            Cili_stloc (lr bm.sptr_or_vale)
            Cili_stloc (lr bm.dptr)
            Cili_lab continue_lab
        ]

    let test =
        [
            Cili_ldloc(lr bm.count)
            Cili_ldc(g_canned_i32, lr 0)
            Cili_beq break_lab
            Cili_ldloc(lr bm.count)
            Cili_ldc(g_canned_i32, lr 1)
            Cili_sub Cil_unsigned
            Cili_stloc(lr bm.count)                        
        ]

    let body_cpblk =
        [
            Cili_ldloc(lr bm.dptr)              // Copy one datum
            Cili_ldloc(lr bm.sptr_or_vale)
            Cili_ldind dt
            Cili_stind dt            

            Cili_ldloc(lr bm.sptr_or_vale)      // Increment src ptr
            Cili_ldc(g_canned_i32, lr 1)
            Cili_add Cil_unsigned
            Cili_stloc(lr bm.sptr_or_vale)               
        ]
        
    let body_initblk =
        [
            Cili_ldloc(lr bm.dptr)              // Write one datum
            Cili_ldloc(lr bm.sptr_or_vale)
            Cili_stind dt            
        ]

    let houser =
        [
            Cili_ldloc(lr bm.dptr)                // Increment dest ptr
            Cili_ldc(g_canned_i32, lr 1)
            Cili_add Cil_unsigned
            Cili_stloc(lr bm.dptr)                        

            Cili_br continue_lab
        ]

    let postamble =
        [
            Cili_comment ("End of " + mm) 
            Cili_lab break_lab
        ]
    let items =
        match kind with
            | "cpblk" -> unstack @ test @ body_cpblk @ houser @ postamble
            | "initblk" -> unstack @ test @ body_initblk @ houser @ postamble            
            | other -> sf (sprintf "other bm kind %s" other)

    let ans = map gec_ins items
    app (fun x -> vprintln 3 (sprintf "  ecil " +  cil_classitemToStr "" x "")) ans
    ans


let printout_canned_bif_db__ _ =
    vprintln 0 ("BIF database contents are:")
    let pout (idl, instancef, lookup_arity) = vprintln 0 (sprintf "   item  %A %i %s " instancef lookup_arity (hptos idl))
    for k in g_kiwi_canned_bif do pout k.Key done

let g_kiwi_tnow_path    =  [ "tnow"; "Kiwi"; "KiwiSystem" ]
    
(* eof *)
