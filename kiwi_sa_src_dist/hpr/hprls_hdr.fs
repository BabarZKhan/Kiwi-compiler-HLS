// CBG Orangepath HPR L/S.  HPR Logic Synthesis and Formal Codesign System.
//
// Contains:
//   hexp_t  : type of scalar bit vector expressions and 1-D arrays.
//   hbexp_t : type of boolen expressions
//   hbev_t  :  type of behavioural statements
//
// Expressions were to be held on a memoised heap using weak pointers for all non-boolen expression.
//
//
// There is a strong concept of a normal form, in that certain forms are for I/O only and never
// used in internal representations, such as '!='. All expressions are formed using
// constructors that memoise and normalise as they go.
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

module hprls_hdr

open System.Numerics
open moscow
open linepoint_hdr
open linprog


type netst_t =
    | Signed
    | Unsigned
    | FloatingPoint

type cast_severity_t =
    | CS_typecast                   // Just a typecast that may alter a subsequent operator overload selection.
    | CS_maskcast                   // Clipping etc to field width.
    | CS_preserve_represented_value // A major bit-changing convert (e.g. int to float).

// Take the max (strongest) of two severities.
let cvt_resolve cvtf = function
    | CS_preserve_represented_value -> CS_preserve_represented_value 
    | CS_maskcast                   -> if cvtf=CS_preserve_represented_value then cvtf else CS_maskcast
    | CS_typecast                   -> if cvtf=CS_preserve_represented_value || cvtf=CS_maskcast then cvtf else CS_typecast

(* Diadic operators:
   These are partitioned by predicate into commutative_and_associative and not and monadic.
 *)
type x_diop_t =
    | V_interm | V_bitand | V_bitor | V_xor
    | V_plus
    | V_minus // Deprecated - we get a better normal form by preferring to add negated numbers.
    | V_times
    | V_divide
    | V_mod
    | V_lshift 
    | V_rshift of netst_t
    | V_exp
    // Monadic forms now follow
    | V_onesc // mondadic one's complement: flip all bits.
    | V_neg   // mondadic two's complement: negate.
    | V_cast of cast_severity_t 


(*
 * Boolean diadic and monadic operators: scalar * scalar -> bool, scalar->bool.
 * In many programming languages, C,C# and Verilog, whether a comparison is signed or unsigned depends on its operand signnedness (ditto right shift).
 * Dotnet byte code has an explicit denotation in the operator and that is reflected in our AST encoding in our hexp_t bidops.  This is clear.  When converting to an x_bdiop_t (from a non-dotnet source) we must put in the correct designation and when converting out, as in cpp_render or verilog_gen, we must generate casts when needed.
 *)
type x_bdiop_t = 
    | V_deqd   (* Equal t:     ==                                *)
    | V_dned   (* Not equal t: !=    (do not use below a bdiop in normal form *)
    | V_dltd of netst_t  (* Less than:   <     (NB:linprog is used in normal form if possible *)
    | V_orred  (* Or-reduce: see if any bits are set:  (|(e)) in Verilog            *)

(* The following diadic forms are only for I/O use and are completely avoided in internal ASTs *)
    | V_dled of netst_t  (* Less or equal:    <=  (do not use in normal form) *)
    | V_dged of netst_t  (* Greater or equal: >=  (do not use in normal form) *)
    | V_dgtd of netst_t  (* Greater:          >   (do not use in normal form) *)

type v_quantif_t = V_univ | V_exists


type varmode_t =
    | INPUT
    | OUTPUT
    | INOUT
    | LOCAL
    | RETVAL // RETVAL is no-longer used. please delete it.

type vartype_t =
    | V_VALUE           // An integer value of range/precision set by h/l or width.
    | V_EVENT
    | V_PARAMETER
    | V_SPARAMETER      // Strings of various types    
    | V_REGISTER
    | V_MUTEX
    | V_ELAB of string  // temporary place holder during elaboration


// Enumeration: And tag and encoding value pair list. 
//type enum_t = string option * (int * string * string) list

(* Beyond precision_t we don't have any types as such, but one convention over widtho fields is 
 *   n>0 is an integer variable of than number of bits
 *   n=0 unspecified integer width
 *   n=~1 string - but better perhaps to use g_string_prec explicitly.
 *   n=~2 void - but better better use g_void_prec that has widtho=None.
*)
type precision_t =
    {
       widtho: int option;
       signed: netst_t;
    }

let g_default_prec =
    {
        widtho=   None;
        signed=   Signed;
    }

let g_void_prec =      { g_default_prec with widtho = None; }
let g_string_prec =    { g_default_prec with widtho = Some -1; } // Special string handle type.
let g_bool_prec =      { g_default_prec with widtho=Some 1; signed=Unsigned; }
let g_default_array_subscript_prec = { g_default_prec with  widtho=Some 64; signed=Signed; }
let g_pointer_prec = g_default_array_subscript_prec
let g_absmm_prec={g_default_prec with widtho=None; } // A signed int of unspec width.
let g_spf = {g_default_prec with signed=Signed; widtho=Some 32; }
let g_dpf = {g_default_prec with signed=Signed; widtho=Some 64; }

// -1 to +1 are the boolean false, dontcare and true.
let g_undef_nn = 100 // Singletons (what?) can have small numbers down here in this range.  
let g_tnow_string = "hpr_tnow"
let g_small_integer_neg_bound = -100000
let g_small_integer_pos_bound = 100000
let g_small_integer_centre_nn = g_small_integer_pos_bound * 3
let g_first_heap_nn = g_small_integer_pos_bound * 5

let g_mya_limit =  g_first_heap_nn + 600 * 1000 // Total number of expressions allowed

let mya_f_fieldwidth = 1  // Spacing

let mya_splay_nn = ref g_first_heap_nn

let g_m_ctoken = ref [ 'a'; 'a' ]  // 

let nxt_ctoken() = // Issue next unique charstring
    let ord c =
        let ba = System.BitConverter.GetBytes (c:char)
        System.Convert.ToInt32(ba.[0])
    let suc x = chr(ord(x) + 1)
    let rec clinc (h::t) = if h = 'z' && t = [] then [ 'a'; 'a'; ] elif h='z' then 'a' :: (clinc t) else (suc h) :: t
    let aa = !g_m_ctoken
    let _ = g_m_ctoken := clinc aa
    implode aa



let int32_meo_nn n = 
    if n < g_small_integer_neg_bound  || n > g_small_integer_pos_bound then failwith ("small integer is not small")
    n + g_small_integer_centre_nn
// String constants: HPR L/S has limited handling for strings and string constants, which are either treated in the same way that they are handled in Verilog, which
// is as an expression or register of width 8 times the string length in characters, or as a special string handle type (where widtho=-1).
// (The Kiwi front-end and repack can map a fixed set of strings to an enumeration type of a suitable width with the strings stored only once and indexed by the enumeration.)



  (* Net attributes:  commonly used are:
           "volatile", "true"
           "portname" has TWO purposes: offchip arrays and external i/f ptrs
           "init" : reset value of a net or array if a Napnode, but not for a ROM since ROM's use constval.
     No longer used owing to constval_t now being used:
           "constant" : now deprecated for constval field : was constant value (or ROM if a Napnode within an XS_withval string).

   *)

type att_att_t =
    | Nap of string * string
    | Napnode of string * att_att_t list

type xidx_t = int // Unique number for expression - for booleans negative values are used for complements.

(*
 * All memoised expression nodes have a meo_t field.  For commutative operators, the
 * list of arguments is sorted by sortorder.
 *)
type meo_t = 
     {
         //hash__:    int      //Delete me might use dot net native one now or else just xidx.
        n:         xidx_t      // Unique number for expression
        sortorder: int         // Value to sort on when saving an associative-operator expression
     }


type constval_t = // Constant indicators for bnets and ROM flag for arrays. A string can also be a constant and can have a withval that refers to one of these forms.
    | XC_string of string
    | XC_bnum of precision_t * BigInteger * meo_t
    | XC_float32 of float32
    | XC_double of double
    
type net_att_t = { 
        n:        xidx_t              // Unique net number: now the same as netbase number.
        id:       string              // Textual name 
        is_array: bool                // 1-D arrays are all that is supported at the hexp_t level. Higer orders should be built on top.
        // Three possible encoding forms for integers: width, range or enum. *)
        width:   int                 // When width is zero; encoding not known or uses h/l. -2 denotes void* and -1 a string.
        signed:  netst_t              // signed/unsigned/floating etc
        rh: BigInteger                // (* Scalar with range from l to h inclusive. *)
        rl :BigInteger 
        constval:  constval_t list   // When non-null a constant indicator. A list of them denotes the contents of a ROM.  Do not confuse with initval used for reset.
    }


type further_net_att_t =
    {
        length: int64 list   (* dimensions: [] if not an array.  [0] if length unknown.  Zero Indexed. Only one dimensional is supported. *)
        // pol was used in h2 but nowhere else. spare really.
        pol: bool     (* Set if an active low signal; ie its print name is (!id). Perhaps better not to use this flag and instead put the negation sign in the id. *)

        dir: bool     (* Set if flipped in parser: i.e. inputs are outputs. NOT USED IN MAIN LIBRARY *)
        xnet_io:    varmode_t           (* Where an I, O, local and so on. *)
        vtype:      vartype_t             // (* Variable storage class *)

        ats:       att_att_t list         // When the g_reserval attribute is stored for a net, this is the reset value assigned at start of day.

    }
exception Sfault of string; (* This value of width used for special 'marker' nets that are actually compile-time constant tokens. They should have a marker attribute.  *)

exception Sfatal of string;

let g_ssm_static_vector_base = 300000L
let g_ssm_heap_base = 400000L

let isIndexable baser = baser >= g_ssm_static_vector_base
let isHeap baser = baser >= g_ssm_heap_base 


let op_fatal(v) =
        (
        print "***Fatal error\n";
        print (v + "\n");
        failwith(v);
        ()
        )

type cost_t = // cf logic cost
    {
        area:              int
        static_power:      int
        dynamic_energy:    int
        intangible:        int
    }

type ip_xact_vlnv_t = // Industry standard IP block naming vector.
    {
        vendor:   string
        library:  string
        kind:     string list // aka name
        version:  string
    }

type si_area_t = int64  // Units of area - one NAND2 gate has unit area in the current reckoning.

(* Built-in primitive functions:
 * Most function calls will be expanded in line by front ends,
    but the abstract.sml code also supports
    a small, canned library that it expands itself. Finally, certain functions, like
    hpr_writeline are supported by most backends (e.g. mapped to printf or $display).
 * 
 *)

type fun_semantic_t =
    {
        // We add both of these flags, which should normally be inverses, in case some ambiguity arises in certain future definitions.
       fs_nonref:          bool          // Non-referentially transparent (calling it more/less often than intended would be wrong)
       fs_reftran:         bool          // Referentially transparent - same result for same arg always.


       fs_eis:             bool          // An end in itself: ie calling this function has useful side effects (generally the opposite of reftran)
       fs_yielding:        bool          // aka blocking: Values of volatile variables may be changed meantimes.
       fs_mirrorable:      bool          // The implementation/implementor of this function may be freely mirrored (i.e. multiple instances created)
       fs_asynch:          bool          // Happy to be posted (only normally relevant for actions that will typically be eis and have void result).
    // H/W level                     

       fs_inhold:          bool          // This is set when a (non-pipelined) FU can rely on its caller holding its inputs stable throughout the computation.
       fs_outhold:         bool          // Whether an FU implementation maintains its output valid for as long as possible until a new result needs be returned.
       fs_overload_disam:  bool          // Include types in names to distinguish overloaded method variants.
    }

type native_fun_signature_t = 
    {  rv:                     precision_t         // Return type.
       args:                   precision_t  list   // Arg types
       fsems:                  fun_semantic_t
       biosnumber:             int                 // ROM jump table position for microcontroller s/w style implementation.
       needs_printf_formatting: bool
       is_identity_fn_in_hw_terms: bool
       is_fu:                  (ip_xact_vlnv_t * string option) option  // The name (and pi_name when different from function's own name) for an HPR or CV (synonyms) standard IP block containing the FU.
       //is_canned_fu:           bool               // Holds when schema and sim model are not loaded from external files (i.e. holds when hard compiled into the HPR source code).
       latency:                (int * int) option // Latency and rei_delay in clock cycles: None for a postable value (rv=Void)
       cost:                   cost_t option
       cost_per_bit_of_result: int
       overload_suffix:        string option
       outargs:                string             // Describes which arguments are mutated by this operation.
       area_estimate:          si_area_t
    }

type native_fun_def_t =  string * native_fun_signature_t (* Textual name and attributes *)



     
let g_unmeo_meo = { n=0; sortorder=0; } // Occasionally we need unmemoised, non-normal-form expressions (e.g. for operator tree balancing or readable output) and we use this special reserved meo for them.  
         

type bloop_prams_t = // Bounded loop parameters
    {
        parf:     bool        // Holds if a parallel loop
        start:    int64       // First value
        endv:     int64       // Last value
        step:     int64       // Step increment
    }
(*
 * Boolean diadic operators: bool * bool -> bool
 * V_bxor is used but we prefer to represent conjunction and disjunction as bmux trees or covers.
 * The linrange also holds a disjunction when it contains several ranges.
 * These may not be used at all when covers are used.
 *)
type x_abdiop_t = V_band | V_bor | V_bxor

type cube_t = xidx_t list

type cover_t = cube_t list

type callers_flags_t =
    {
        externally_instantiated_block: bool
        tlm_call:                      bool option // The bool holds for a blocking TLM call, and is false for a non-blocking call.
        loaded_ip_:                    bool
    }


type shared_resource_info_t = string list * (string * string) list // Name of a shared resource, such as a memory bank, and key/value attributes.

(*
 * Boolean expression nodes:
 *)
[<CustomEquality;CustomComparison>] 
type hbexp_t = 
    | X_true
    | X_false
    | X_dontcare
    | W_bitsel of hexp_t * int * bool * meo_t             // Static bit extract/select/insert. 
    | W_bnode of x_abdiop_t * hbexp_t list * bool * meo_t // Associative ops: and, or, xor - when not using bdd or covers.
    | W_bmux of hbexp_t * hbexp_t * hbexp_t * meo_t       // Two-input multiplexor
    | W_cover of cover_t * meo_t                          // Sum-of-products boolean expression
    | W_linp of hexp_t * linrange_t * meo_t               // Linear range: disjunction of constant ranges - do not use with covers.
    | W_bdiop of x_bdiop_t * hexp_t list * bool * meo_t   // Comparison operators equality and less than and also orreduce where list is a singleton. Bool denotes inverted output. 
    | W_bsubsc of hexp_t * hexp_t * bool * meo_t          // Dynamic, single-bit subscription with invert flag.
    with
        member x.xb2nn 
            with get() = // Will return -ve when inv flag set
                match x with
                    | W_bnode(_, _, _, meo)
                    | W_cover(_, meo)     
                    | W_bdiop(_, _, _, meo) 
                    | W_bitsel(_, _, _, meo) -> (meo.n)
                    
                    | W_bsubsc(_, _, _, meo) -> (meo.n)
                    | W_bmux(_, _, _, meo)   -> (meo.n)
                    | W_linp(_, _, meo)      -> (meo.n)
                    
                    | X_true                 -> 1
                    | X_false                -> -1
                    | X_dontcare             -> 0

        interface System.IComparable with
            member this.CompareTo obj =
                match obj with
                    | :? hbexp_t as y ->  this.xb2nn - y.xb2nn

        override this.GetHashCode () = this.xb2nn
              
        override x.Equals (y: obj) =
           match y with
               | :? hbexp_t as y -> x.xb2nn = y.xb2nn
               | _ -> false

                    

and x_string_t = (* Special constant expression values and/or markers
                    NB: All forms of x_string are constants ! *)
    | XS_fill of int // A normal string when arg is 0, otherwise one whose base pointer has been advanced accordingly.
    | XS_unquoted of int
    | XS_unit
    | XS_tailmark
    | XS_withval of hexp_t
    | XS_withval_key of int | XS_withval_strkey of string | XS_sc of memdesc_record_t list


(*
 * Non-boolean expression type (integer, bit vector and floating point)
 *)
and [<CustomEquality;CustomComparison>] hexp_t  = 
    | X_undef         // The undefined value.

    // Function call: could also be used for W_nodes really. 
    | W_apply of (string * native_fun_signature_t) * callers_flags_t * hexp_t list * meo_t

    // 1-D array subscription 
    | W_asubsc of hexp_t * hexp_t  * meo_t (* Array subsc *)

    // Operator: Most monadic and diadic operators are held like this: 
    | W_node of precision_t * x_diop_t * hexp_t list * meo_t (* All commassoc operators in sorted order and exp,mod,div and minus in meaningful order. Minus is deprecated! *)

    // Pair: first arg is merely annotation or side-effecting companion to the second arg whose value is returned.
    | X_pair of hexp_t * hexp_t * meo_t

    // Conditional expression: if/then/else  ?: question mark/colon mux2 operator.
    | W_query of hbexp_t * hexp_t * hexp_t * meo_t

    // Boolean lifting: false is zero and true is one.
    | X_blift of hbexp_t

// Delay operator: can refer to future and past values of an expression. X is the next state operator and has the same semantics as in model checkers.
// Golden law: "X(Q)=X_x(Q, 1)=d" or "Q=X_x(d,-1)": ie, the next value of the Q output of a D-type is given by its D input.
// A delay value of +n in RMODE means the same as a value of -n in LMODE
// In RMODE X_x(d, -1) is the output of a D-type whose input is d.
// Definitions: X_x(d, 0) === d
// An XRTL sequential arc denotes this as "Rarc(Q, D)".  The X operator is implicit in the sequential arc.
// An XRTL sequential arc can also denote a combinational buffer using "Rarc(X_x(Q, -1), D)" or "Rarc(Q, X_x(D, 1))". These denote "assign q = d" in Verilog terms.
    | X_x of hexp_t * int * meo_t

     // Net forms: X_net is a shorthand for highly-temporary nets 
    | X_net of string * meo_t
    | X_bnet of net_att_t 

    // String constants and other special constants 
    | W_string of string * x_string_t * meo_t

    // Binary constants
    // | X_mask of int * int // Constant number with bit positions (h..l) set to ones
    
    // Short and small numbers in 32 bit field use X_num but everything else is an X_bnum
    | X_num of int
    | X_bnum of precision_t * BigInteger * meo_t

    //     A declaration: has no value as an expression form 
    | X_iodecl of string * int * int * net_att_t // replaced with a bnet attribute.

    //  Quantification 
    | X_quantif of v_quantif_t * hbexp_t

    // For verilog I/O only there's no distinction between an array and dynamic bit subscript in the concrete syntax and the subsc form is normalised to asubsc or bsubsc for internal processing. 
    | X_subsc of hexp_t * hexp_t  // Verilog-like I/O only: deprecated for general use.


    // Anonymous function 
    | W_lambda of hexp_t list * (hexp_t option * hbev_t list) * (hexp_t option * hbev_t list) list * (string * string) list * meo_t
         (* Anon function and spawned subprocesses needed to run it: ideally latter arg would be an HPR SP block.  *)

    // Valof/Resultis form as in BCPL:  for putting statements inside expressions. 
    | W_valof of hbev_t list * meo_t

        (* PSL embedding: *)
    | X_psl_builtin of string * hexp_t (* NB: rose/fell/stable/prev/next *)
    | X_psl_diadic of psl_diop_t * hexp_t * hexp_t

    // The equality operator can be the slowest thing so we crudely add this clause to make it unapplicable.
    // hprls_hdr/hexp_t:Equals (object,System.Collections.IEqualityComparer)    
    | X_no_equality_enforcer of (int -> int)


    with
        member x.x2nn
          with get() =
              match x with
                  | X_undef -> g_undef_nn
                  | X_num n -> int32_meo_nn n
                  | X_net(_, meo)
                  | X_bnum(_, _, meo)
                  | W_node(_, _, _, meo)   
                  | W_asubsc(_, _, meo) 
                  | W_query(_, _, _, meo)
                  | W_apply(_, _, _, meo)
                  | W_valof(_, meo)
                  | W_lambda(_, _, _, _, meo)
                  | X_pair(_, _, meo)
                  | X_x(_, _, meo)
                  | W_string(_, _, meo)   -> meo.n
                  | X_bnet ff             -> ff.n
                  | X_blift b             -> b.xb2nn
                  | other -> failwith(sprintf "other x2nn L393 %A" other)

        
        interface System.IComparable with
            //override m.frog x = 32
            member this.CompareTo obj =
                match obj with
                    | :? hexp_t as y ->  this.x2nn - y.x2nn 

        override this.GetHashCode () = this.x2nn

        override x.Equals (y: obj) =
           match y with
               | :? hexp_t as y -> x.x2nn = y.x2nn
               | _ -> false
 

(*
// also concatenation (;) and fusion (:) are diadic
// s_within s_until  s_implies repetition

// seres alternation: cannot use logor because ?
// two conjunctions operators: length matching (&&) and non-length matching(&).

*)

and psl_diop_t = 
       | Psl_next_a
       | Psl_next_e
       | Psl_next_event
       | Psl_before
       | Psl_before_
       | Psl_until
       | Psl_until_


// storeclass_t: A means of naming abstract dataflow classes.  Using strongly-typed source languages, the classes can be based on types, since we expect no assignments between types, but various brandings can get conglomorated. Conglomoration was supported using reference inside the class names but now we use externally held mapping. The names also need to have certain algebraic qualities, so for instance, the class name arising by referencing and then derferencing a pointer returns to where it started. The same goes for fields within a class.
//
//     The first component is a debug handle only.
//     The second component is 0 for a region and -1 for a scalar, denoting whether the sc applies to the address of the entity or its contents, respectively.
//     The third component is the sc integer (sci). This is a ref so it can be remotely changed during
// conglomoration of storage classes. Nonetheless, the final memdesc_record_t
// allows a list since conglomoration under a single monica may not have happened.

and storeclass_t = (string * int * int) // TODO delete all this and use a simple int
    
and memdesc0_t = // Describes an 'object' in memory.  Every object is in an equivalence class called its storage class (aka memtok).  Owing to conglomoration the class may gave more than one name hence a list of them is provided, but they are nominally all held under the first name in that list.
    {
        uid: vimfo_t           //
        literalstring: bool
        mats: att_att_t list   // Attributes - prefer not to use and to instead expand this struct.
        f_sc:     int          // Storage class or dataflow number.  (TODO: need to implement a manager to make globally unique...)  For scalars we annotate with the content class that that scalar ranges over. For regions we annotate the base address with the sc and the SC_ind of that will be the content type. 
        f_sc_char:  char
        vtno:       int        // Vtable no
        has_null:   bool       // Marker for the special null pointer that is not a member of all storage classes, but which is shared between them without conglomoration.
        shared_resource_info: shared_resource_info_t option
    }

// Memory region description (typically an object or record, struct or array).
// Can be nested when length encompasses other base addresses.
and vimfo_t =
    {
         f_name:   string list  // instance name or [] for anon
         f_dtidl:  string list  // class/type name
         baser:    int64        // base heap index
         length:   int64 option // If length is g_wondarray_marker (and base is zero) then a wondarray (an array of unlimited size).
//         f_vtno:   int          // Associated virtual method table (for Kiwife->Repack use)
   }


and memdesc_record_t =
    | Memdesc0 of memdesc0_t
    | Memdesc_sc of int
    | Memdesc_scs of string
    | Memdesc_tie of memdesc_record_t list // A number of classes at the same level - can delete list form in memdesc0_t
    | Memdesc_ind of memdesc_record_t * memdesc_record_t option   // Data array or pointer indirect annotation 
    | Memdesc_via of memdesc_record_t * string list // Field access annotation    


(*
 * Annotation:
 *)
and annotate_t = { 
        atts: (string * string) list; 
        palias: (string * hexp_t list) option;
        comment: string option }

(*
 * Orangepath HPR imperative programming dialect (IMP)
 *)
and hbev_t = 
      (* Timing primitives: only acceptable to certain processes. *) 
    | Xwaitabs of hexp_t         (* Waitabs waits until abs time. *)
    | Xwaitrel of hexp_t         (* Wait waits a number of time units *)
    | Xwaituntil of hbexp_t      (* Waituntil waits for a bool condition to hold *)


    (* The following four constructs are the main ones used for internal processing: beq, goto and label are used when hbev is stored in a DIC array. *)  
    | Xbeq    of hbexp_t * string * int ref  (* Conditional branch, taken if the guard is false. The int is filled in by assemble as the address of the label *)
    | Xgoto   of string * int ref            (* Unconditional jump. *)
    | Xlabel  of string                      (* Label, serves as branch dest *)
    | Xassign of hexp_t * hexp_t             (* Assignment *)

    | Xbloop of hexp_t * hbev_t * bloop_prams_t (* Bounded loop:  induction variable, body and parameters *)


    | Xdominate   of string * int ref        (* Domainates annotation. *)

    (* The following forms are used mainly high-level behavioural C-like code and AST parse trees *)
    | Xblock  of hbev_t list                (* Core language *)
    | Xpblock  of hbev_t list               (* Core language *)
    | Xif     of hbexp_t * hbev_t * hbev_t  (* Only in AST: yet core language *)


    | Xskip                                 (* Do nothing. *)

    | Xwhile  of hbexp_t * hbev_t           (* Loop constucts: not used in dic array form  *)
    | Xdo_while  of hbexp_t * hbev_t        (* Loop constucts: not used in dic array form  *)
    | Xbreak                                (* Loop constucts: not used in dic array form  *)
    | Xcontinue                             (* Loop constucts: not used in dic array form  *)
    // Xfor should perhaps be added here and can be expanded by compile_hbev


    (*
     * We model fork in an HPR hbev_t imperative machine code as follows
     * 20 fork 50
     * 30 S1
     * 40 goto 60
     * 50 S2
     * 60 join (2, false)   A two thread barrier: false=dont kill the others.
     * 
     *)
    | Xfork   of string * int ref
    | Xjoin of int * bool        (* Number of threads to barrier and whether to abort the others *)


    | Xsafelive of hexp_t * hexp_t          (* Short form assertion. *)


//  | Xcall of native_fun_def_t * hexp_t list // cf is missing (* Procedure call: also denotable using Xeasc(apply(_)). *)

    | Xreturn of hexp_t          (* Xreturn doubles up as resultis if inside an W_valof *)
    | Xswitch of hexp_t * (hexp_t list * hbev_t) list


    | Xeasc of hexp_t                       (* Side effection expression as command *)

    | Xlinepoint of linepoint_t * hbev_t    (* Source-code annotation *)
    | Xannotate of annotate_t * hbev_t      (* Other annotation *)
    | Xcomment of string                    (* Comment *)

let g_dic_hwm_marker = Xcomment "There are no DIC locations used beyond this high water mark"

// The following pair is used for stuttering compositions of machines and in strictness analysis for RTL and state machines: a program counter and its state: the RTL should be executed when the pair halves are equal, the second term must be a constant (or at least equality comparable with all peers).
type pcp_t = (hexp_t * hexp_t)
type stutter_t = pcp_t option

(* NB: type it_t is defined in abstract header too, but double definition is no problem for now.
 * it_t is the memoised heap expression format.
 *)
type it_t = hexp_t


exception Hexp_t of string * hexp_t


let leafp x =
    match x with
      |   (X_num _) -> true
      |   (X_bnum _) -> true
      |   (X_undef) -> true
      |   (X_net _) -> true
      |   (X_bnet _) -> true
      |   (W_string _) -> true
      |   _ -> false


let isio x = x=INPUT || x=OUTPUT || x=INOUT || x=RETVAL


let g_null_an = { palias= None; atts=[]; comment=None }

let g_named_bondout_bank_pseudo = "named-bondout-bank-pseudo"

let g_module_name_reflection = "percent-modulename"     // Diosimulator reflection to get current module name (or %m in Verilog terms).
let g_bondout_heapspace_size = "bondout-heapspace-size" // Default size to allocate for a heap (in offchip DRAM).
let g_bondout_heapspace_name = "bondout-heapspace-name" // The user's dynamic heap can be partitioned into separate disjoint regions by repack (these can then go in different physical memory banks without multiplexor cost).


let nullan (an:annotate_t) = nonep an.palias && nullp an.atts && nonep an.comment


// This is our representation of the VSFG state edge.
// We must keep fences, PLI and other side-effecting operations in ascending order (except when branching backwards).
// The string represents the ordering domain.
// The first integer is a total ordering within the domain, although this could perhaps be a partial order for some applications and we manage that by replicating values of this integer.
// The second int is a generation id that can be used as a tie break but which can be ignored when eliding duplicate calls.
type vsfg_order_t = (string * string list * int * int) 


type edge_t =
    | E_any of hexp_t
    | E_pos of hexp_t
    | E_neg of hexp_t
    | E_anystar

let g_int_prec = { g_default_prec with widtho=Some 32; signed=Signed; }


let g_default_fsems = 
     {
       fs_nonref=      false    // Non-referentially transparent: calling it more often than intended is wrong *)
       fs_reftran=     false
       fs_eis=         false    // An end in itself: ie calling this function has useful side effects. *)
       fs_yielding=    false    // aka blocking: Values of volatile variables may be changed meantimes. *)
       fs_mirrorable=  true     // The implementation/implementor of this function may be freely mirrored (i.e. multiple instances created)
       fs_inhold=      false    // By default, input nets are only held valid until request handshake cycle.
       fs_outhold=     false
       fs_asynch=      false
       fs_overload_disam=false   // Include types in names to distinguish overloaded method variants.

     }
       
let g_default_native_fun_sig =
     { rv=g_default_prec          // type (width) of return value or special value from above table.
       // Type designation (crude system): A string is -1, (-2 is/was void * - now use widtho=None). 0 is not known or in h/l fields of bnet.
       args=[]       // Arity: Precisions (types including signedness and width) of args.
       fsems=                       g_default_fsems
       biosnumber=                  -1  // ROM jump table position
       is_identity_fn_in_hw_terms=  false 
       is_fu=                       None
       needs_printf_formatting=     false // Specifies language-specific processing for string formatting characters.
       cost=                        None
       latency=                     None
       cost_per_bit_of_result=      1
       overload_suffix=             None
       outargs=                     ""
       area_estimate=               100L // An arbitrary default silicon area.
     }


let g_alpha_prec = {g_default_prec with signed=FloatingPoint; widtho=None}  // Place holder for polymorphic type var alpha - this is not supported so needs concrete rehydration on every use currently.

let g_sp_prec = {g_default_prec with signed=FloatingPoint; widtho=Some 32}
let g_dp_prec = {g_default_prec with signed=FloatingPoint; widtho=Some 64}

let g_hpr_max_fgis = ("hpr_max", { g_default_native_fun_sig with biosnumber=8; rv=g_absmm_prec; args=[g_default_prec; g_default_prec;]})
let g_hpr_min_fgis = ("hpr_min", { g_default_native_fun_sig with biosnumber=9; rv=g_absmm_prec; args=[g_default_prec; g_default_prec;]})
let g_hpr_abs_fgis = ("hpr_abs", { g_default_native_fun_sig with biosnumber=2; rv=g_absmm_prec; args=[g_default_prec]})

// Convert between floating point precisions
let g_hpr_flt2dbl_fgis = ("hpr_flt2dbl", { g_default_native_fun_sig with  biosnumber=14; rv=g_dpf; args=[g_spf]})
let g_hpr_dbl2flt_fgis = ("hpr_dbl2flt", { g_default_native_fun_sig with  biosnumber=15; rv=g_spf; args=[g_dpf]})

let g_canned_default_iplib = { vendor= "HPRLS"; library= "HPR_CVIP0_IPBLOCKS"; kind=["-vanilla-"]; version= "1.0" }

// Generate the name for a CV standard convert block (fp only)
let gec_cv_fpcvt_name pto pfrom latency =
    let cv_s prec = // cf sq_prec
        if nonep prec.widtho then "BADWILD" // error
        else (if prec.signed=FloatingPoint then "F" else "I") + i2s(valOf prec.widtho) 

    let kkey = sprintf "CV_FP_CVT_FL%i_%s_%s" latency (cv_s pto) (cv_s pfrom)
    let kind_vlnv = { g_canned_default_iplib with kind=[kkey] }
    (kind_vlnv)

let add_ip_block_name (fname, gis) =
    let ip_block_name =
        match fname with
            | "hpr_alloc"       -> { g_canned_default_iplib with kind=[ "HPR_HEAPMANGER_T0" ] }
            | "hpr_exchange"    -> { g_canned_default_iplib with kind=[ "HPR_ATOMIC_XCHG" ] }
            | "hpr_atomic_add"  -> { g_canned_default_iplib with kind=[ "HPR_ATOMIC_ALU" ] }

            | other -> //  fpcvt
                let latency = 2
                let pto = gis.rv
                let pfrom = hd(gis.args)
                gec_cv_fpcvt_name pto pfrom latency 
                
    (fname, { gis with is_fu=Some(ip_block_name, None) })

 // Convert to integer forms from floating point forms.
let g_hpr_flt2int32_fgis = add_ip_block_name ("hpr_flt2int32", { g_default_native_fun_sig with  biosnumber=16; rv={ g_default_prec with widtho=Some 32 }; args=[g_spf]})
let g_hpr_flt2int64_fgis = add_ip_block_name ("hpr_flt2int64", { g_default_native_fun_sig with  biosnumber=17; rv={ g_default_prec with widtho=Some 64 }; args=[g_spf]})
let g_hpr_dbl2int32_fgis = add_ip_block_name ("hpr_dbl2int32", { g_default_native_fun_sig with  biosnumber=18; rv={ g_default_prec with widtho=Some 32 }; args=[g_dpf]})
let g_hpr_dbl2int64_fgis = add_ip_block_name ("hpr_dbl2int64", { g_default_native_fun_sig with  biosnumber=19; rv={ g_default_prec with widtho=Some 64 }; args=[g_dpf]})

// Further convert from floating point forms from integer forms.
let g_hpr_flt_from_int32_fgis = add_ip_block_name ("hpr_flt_from_int32", { g_default_native_fun_sig with  biosnumber=20; rv=g_spf; args=[{signed=Signed; widtho=Some 32; }]})
let g_hpr_flt_from_int64_fgis = add_ip_block_name ("hpr_flt_from_int64", { g_default_native_fun_sig with  biosnumber=21; rv=g_spf; args=[{signed=Signed; widtho=Some 64; }]})
let g_hpr_dbl_from_int32_fgis = add_ip_block_name ("hpr_dbl_from_int32", { g_default_native_fun_sig with  biosnumber=22; rv=g_dpf; args=[{signed=Signed; widtho=Some 32; }]})
let g_hpr_dbl_from_int64_fgis = add_ip_block_name ("hpr_dbl_from_int64", { g_default_native_fun_sig with  biosnumber=23; rv=g_dpf; args=[{signed=Signed; widtho=Some 64; }]})

let g_hpr_toChar_fgis = ("hpr_toChar", { g_default_native_fun_sig with  biosnumber=24; rv={g_default_prec with signed=Signed; widtho=Some 8; }; args=[g_default_prec]})



// The definitions with this flag set are needed because hexp_t represents the difference between integers and FP whereas real hardware does not.  These functions have no manifestation in real hardware.
let g_bcf_fun_sig = { g_default_native_fun_sig with is_identity_fn_in_hw_terms= true }

// Get access to floating point as raw bits.
let g_hpr_bitsToSingle_fgis = ("hpr_bitsToSingle", { g_bcf_fun_sig with  biosnumber=25; rv={g_default_prec with signed=FloatingPoint; widtho=Some 32; }; args=[g_default_prec]})
let g_hpr_bitsToDouble_fgis = ("hpr_bitsToDouble", { g_bcf_fun_sig with  biosnumber=26; rv={g_default_prec with signed=FloatingPoint; widtho=Some 64; }; args=[{g_default_prec with widtho=Some 64}]})

let g_hpr_singleToBits_fgis = ("hpr_singleToBits", { g_bcf_fun_sig with  biosnumber=27; rv={g_default_prec with signed=Unsigned; widtho=Some 32; }; args=[g_sp_prec]})
let g_hpr_doubleToBits_fgis = ("hpr_doubleToBits", { g_bcf_fun_sig with  biosnumber=28; rv={g_default_prec with signed=Unsigned; widtho=Some 64; }; args=[g_dp_prec]})


let g_hpr_strlen_fgis =  ("hpr_strlen", { g_default_native_fun_sig with  biosnumber=29; rv=g_int_prec; args=[g_string_prec]})

// Why is only sqrt here and not the rest of the maths library?  Please explain?  The rest do not have biosnumbers?
let g_hpr_sqrt_sp_fgis = add_ip_block_name ("hpr_sqrt_sp", { g_default_native_fun_sig with  biosnumber=30; rv=g_sp_prec; args=[g_sp_prec]})

let g_hpr_sqrt_dp_fgis = add_ip_block_name ("hpr_sqrt_dp", { g_default_native_fun_sig with  biosnumber=31; rv=g_dp_prec; args=[g_dp_prec]})


// The hpr_pending_read primitive is like the X_x primitive, in that it can read the current pending update to a register, but it defaults to the value of the register instead of failing if no update is pending.
// In RTL terms  "aa<=e1; bb<=aa; cc<=pending_read(aa)" will result in bb taking on the old value of aa as normal but cc getting e1.

let g_hpr_pending_read_fgis = ("hpr_pending_read", { g_default_native_fun_sig with  biosnumber=32; rv=g_alpha_prec; args=[g_alpha_prec]}) // Type is polymorphic: alpha -> alpha


let g_null_net_att =  // The imutable part - memoised arithmetic could become invalid if these properties were mutated.
    {
         n=        -12345678
         width=    32
         signed=   Signed
         id=       ""
         rh= -1I; rl= -1I;
         is_array= false
         constval= []
    }

let g_null_further_net_att = // The mutable part - preferably not mutated.
    {
         pol=      false
         dir=      false
         xnet_io=  LOCAL
         vtype=    V_VALUE
         ats=      []
         length=   []
    }

type directorate_style_t =  // Sets the complexity of the management interface in coarse granules.
    | DS_minimal
    | DS_basic              // A single-bit FAIL output only?
    | DS_normal
    | DS_advanced

type hfast_prams_t = // HFAST (includes AXI streaming when all four handshake nets are specified)
    {
        max_outstanding:    int       // Use -1 in this field as another way to mark fully-pipelined
        
        posted_writes_only: bool      // When present there is no ack signal (and hence no ackrdy of course)
        req_present:        bool
        ack_present:        bool        
        reqrdy_present:     bool
        ackrdy_present:     bool
    }

type directorate_hangmode_t =
    | DHM_hang                          // Just hang in a dead state (achieved sometimes via abend syndrome not being 255)
    | DHM_finish                        // Include a $finish/exit statement at the point of abend/or exit, but no $finish which is hateful to some synthesis tools.
    | DHM_auto_restart                  // OLD for HSIMPLE: Go back to the top, where we may wait for a new request. Not really needed now.
    | DHM_pipelined_no_handshake        // Fully/pipelined or with finite II, but always auto_restarting without dally.
    | DHM_pipelined_stream_handshake of string * hfast_prams_t 

type performance_target_t =
    {
        latency_target:   int
        ii_target:        int
        hardness:         double
    }

// Synch or asynch reset: active high or low.
type dir_reset_t = (bool * bool * hexp_t) // (is_pos, is_asynch, net)


// Program direction is the ability to breakpoint and pause it etc..
// We also need to separate into different clock domains for some designs.
type directorate_t = // Here we specify clock, reset, runstop and other debug information for a subsystem.  Other information is on the cmd line and processed at other times, like reset polarity, and a more-uniform approach would be welcome.
    {
        style:               directorate_style_t
        resets:              dir_reset_t list // 
        clocks:              edge_t list // Clocks however have an edge spec here.
        clk_enables:         hexp_t list // External clock enable - active high always.
        handshakes:          hexp_t list // Req/ack/rdy etc handshake nets
        abend_register:      hexp_t option
        pc_export:           bool   // Export PC value alongside waypoints for debugging visibility.
        unary_leds:          hexp_t option
        self_start:          bool
        hang_mode:           directorate_hangmode_t
        performance_target:  performance_target_t option
        ats:                 (string * string) list  // Several other fields are generated from these ats but we keep a note of the ats here too.
        duid:                string
    }
    

// Here we specify clock, reset, runstop and other debug information for a subsystem.
let g_null_directorate = 
    {
        style=               DS_minimal
        resets=              []
        clocks=              []
        clk_enables=         []
        pc_export=           true // Need to be able to set this from C# attributes and recipe etc..
        handshakes=          []
        abend_register=      None
        unary_leds=          None
        self_start=          true
        hang_mode=           DHM_finish
        performance_target=  None
        ats=                 []
        duid=                "anon-1"
    }


let g_wondtoken_marker = "wondarray"
// Can use this token but a better predicate in future is: If length is g_wondarray_length (and base is zero) then a wondarray (an array of unlimited size).

// Round up to allocation size - multiple of 8
let heap_alloc_roundup x = ((x+7L)/8L)*8L 

(* eof *)

