(*
 * HPR L/S Project. 
 *
// CBG Orangepath. HPR Logic Synthesis and Formal Codesign System.
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
 *
 * Repack arrays, records and scalars.  Some arrays can be split into smaller ones.  Othertimes we want to pack some scalars and/or arrays
 * together into a larger array.  Arrays that are read only can be classified as ROMs.
 *
 * Knowledge of ROMs and constant handles is important since they commonly occur in C#/Kiwi designs in a form where multiple levels of
 * indirection can be telescoped into a single, but straightforward constant propagation is insufficient owing to a variable index
 * being applied to a root array (e.g. an array of objects that each has an array handle that is not modifided post construction).
 *
 * Arrays may be sparse with all indecies having a common factor or offset: these are compacted.
 *
 * Although HPR only supports 1-D arrays there is metalevel dimensionality information in the format
 * of the subscript expressions.  They are sums of pairs where the second field is the numeric value
 * that is summed and the first, the tag, is a storage class and/or field description.
 *
 * Where storage classes are indicated, a prior pass has ensured that pointers are disambiguated, ie they only range over
 * the items listed in that class.  
 * 
 * Each term may be constant or variable.  Where the tag is a field
 * description it is a constant and we are assured that no other constant
 * or variable will alias to its value.  This corresponds to 'safe'
 * Java-like code where taking the address of a field in a record or of a
 * local variable is banned.
 * 
 * For safe dotnet programs, arrays are either created with newarr and
 * have a static base address or else are scalar field arrays which arise from
 * multiple instances of records that have differing base addresses but
 * are always used with constant tag offsets. Stack frames are also field arrays in this consideration.
 * 
 * 
 * For general treatment, we need to support any number of subscript
 * terms and split the terms into fully-constant and
 * mixed-constant/variable and have full information on the range of
 * variable terms and which constant values they can range over.

 *
 * Consider a pointer to a class that contains an array where that pointer gets flipped between multiple instances of the class. E.g. Test14x.
 * Kiwi ensures that the array in each instance has a static size and the number of instances is static. The heap address of each
 * array is static. 
 * An index of form ptr.arrow[6] where ptr is in V_0 is encoded by the Kiwi front end as 
 *    DSINT_AX[ {$offset,24} + {c27,CBG-ARRAY1D_AX[{$offset+arrow,20}+{c27,tTMT4Main_V_0}]} ]
 * which is a double indirection as expected.  NOW LOOOKS WRONG!

We used to scan along c22 and do stuff which was fine, but now its gone we loose purchase and dont make anything.


In this example we are building the datapath for the c22 and the control logic for that includes c44.

 * TODO: anything with c22 is old - we use, eg.,  d22 for the new system.
 *  We used to have  WARR[{c22,base} + {"$offset", e}]   = some item in c22 and the class of the base expression would be not visible here.
 *  We then had OLD      WARR[{"c44",base} + {"$offset", e}] = some item in c22 which shows instead the class of the base expression.  
 *  memdescs report that for c22 points_at=c44 but this reads backwards
 *  Better to say that c22 is pointed at by c44 or controlled by c44.  Nice phrase.
 *
 * This example is then repacked here as a mux tree.  The muxes are created using the guard expressions supported by the array opreation rewriter in meox (AR_mapping_t).
 * The operations to and from the separate array instances are routed by the mux tree. 
 *
 * Ultimately all or many of the separate arrays are likely to be mapped into a common DRAM bank in which case many
 * muxes may disappear since their inputs are commoned (g?e1:e1 --> e1).
 *
 * Algorithm for fields:
 *   Simple: for all objects of a given type we make a scalar field array for each field that contains a scalar value.  These scalar field arrays are then treated as per arrays.
 * For fields that are base pointers these form a storage class that is pari passu with the contents of mutable arrays of array base pointers that they may get interchanged with.
 * But, commonly, these array base pointers are never changed and we want to optimise.
 * Before optimisiation we create a pointer field array that commonly contains the identity function.
 * But, mapping field arrays into DRAM may ruin spatial locality for cached DRAM (notes to be added).
 
// Some fields may not be used with all base addresses, when scalarising registers can be deleted - ... explain other cases and how effective this is please.


 // When a memdesc has "$offset+" format this is a constant that identifies a field.
 // We make a fieldarray when there are multiple base item regions for a record subscription (or potentially for a one element sub array!) i.e. when we have more than one instance of a record  // we make an array for each of its fields.  This might be poor for DRAM spatial locality however .... TODO further study.
 
Note: that a set of arrays that each end up with only one scalar in them can/should be treated like a field array.

     
Other recipe stages, such as restructure, map arrays to technology-specfic RAMS which may have pipelines and porting structural hazards.

The following rule is not currently implemented:
  If we establish properties over which addresses are written we can render that sized array, which may be smaller than the read range, where out of range reads can return dont-cares.


Call graph:
   repack_top -> repack_one_vm
   repack_top -> repack_one_vm -> walk_exec walk the input VMs logging in subsbase_ops
   repack_top -> collate_RP2a1
   repack_top -> app RP2a2 aops 
   repack_top -> RP2a3 aka xformer0
     Report on Final Store Class Regions Summary
     LOCK
   repack_top -> app RP2c aops
   (repack_top -> RP2c xformer1) -> xform1 // read subsbase_ops and return pertinent_classes - only fold over the latter now ?
   repack_top -> RP3 - collate
   repack_top -> (RP4) scale_class
   repack_top -> map_array_class -> classed_scan -> get_fst_ans -> allocate_ops_to_each_class_partition
   repack_top -> map_array_class -> classed_scan -> second_partition_prepare1 -> second_partition_f
   repack_top -> map_array_class -> classed_scan -> form_new_lhs
   repack_top -> map_array_class -> classed_scan ->


Detailed data flow and procedures:

   // RP2a1: Conglomorate memdesc descriptions in to a final set of storage classes.
   // RP2a2: Process each indexed lhs to create the pertinent_classes database.
   // RP2a3: aka xformer0 - converts region representation.
   // RP2b: collate regions

   //  Find addressable storage regions associated with each class.
   // RP2c: aka xformer1 - colit spots and combines identical index pairs as a single VN.
   // RP2c -old: disabled region collate
   // RP3: (upcalled from RP4)
   // RP4: packing decisions
   // RP6a: First collate/partition is on region name or pointer/base address.
   // RP6b: Second partition on offset 
   // RP7: mapping rewrite directives
   // RP8: transclose -  Some subscript operations are inside others - so need to apply one set of maps to another.
   // RP9: Second pass

   Where xfomer has the following sub-stages:
     // Each input array (each lhs)  has all of the operations done on it collated.
     // Storage classes are formally organised in equivalence classes with one nominated in each class as its representative leader.
     // Per lhs we collate the operations by storage class to get the full range of subscripts that are used in each class.
     // For each class we check whether subscripts can arithmetically alias members of another class and conglomorate if so. This gives a revised set of classes.
     // For each revised class there may be both constant and variable subscripts.  If all are constant then the class can be replaced with a simple set of scalars since no muxers will be needed, but ultimately these scalars may be put back in a RAM or RAMs if there are many of them and access bandwidth will be sufficient.
     //

        we use the concept of regions - the kiwi newobj heap regions are always disjoint and we have pointers to the start of them ...   a field address will lie in a region but we tend to rely on kiwi annotating its subscripts with xi_pairs so this is explicit anyway...


   ROM subscripts at constant offsets can be replaced with the ROM value.
   Immuatable base pointer arrays, commonly generated by KiwiC front end from things likes arrays of objects and multi-dimensional arrays will be detected as ROMs here but KiwiC front end will already have propagated the constants in almost every instance, so there is little gain from detecting that sort of ROM here.
   The seven-segment encoder ROM and others in the Bowtie alignment matcher benefit from h/w treatment as combinational functions. 
 *)
module repack


open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics


open hprls_hdr
open meox
open moscow
open yout
open abstract_hdr
open abstracte
open opath_hdr
open protocols
open ksc

let m_repack_loglevel = ref 1  // Verbosity: Do not change here: this is reset from command line or recipe.

let g_shotgun_strings = true // We need to make this ON as new default for test57 and other runtime string handlers. Please say what it does!

type repack_control_t =
    { 
      split_prefix :  string
      max_mux_arity:  int option             // Maximum first dimension in 2-D array before multiplying out (instead of muxing RAMs).
      constidx_rcs:   int option
      varidx_rcs:     int option       
      //asa: bool;
      gen_roms:       bool // Designate arrays/memories that are assigned only once at each location with a constant value as ROMs.
      control:        control_t
      stage_name:     string
      bondouts:       ((bondout_address_space_t * bondout_port_t list) * bondout_memory_map_manager_t) list
      loglevel:       int
      ignore_leftover_error:  bool                 // Debugging kludge - not recommended to ever use
    }

type constant_write_indicator_t =
    | CO_unset                  // Unset
    | CO_reader                 // Not a write
    | CO_manifest of BigInteger // Write of this constant integer (floats are not relevant for array handles)
    | CO_varexp                 // Write of something that is not a constant


    
type repacked_regions_t = ListStore<ksc_clskey_t, memdesc_record_t> //TODO make disjoint with the previous indecies

let next_ser_no = ref 1000
let next_ser() =
    let _ = mutinc next_ser_no 1
    !next_ser_no


let p2ssx_(a, pow, sci) = p2ss1 pow sci
let p2ssy(sci) = sprintf "d%i" sci


// Resolve writen value information when conglomorating.  Both the stored expression and the stored address need to be constants.  
let co_disjunction = function
    | (CO_unset, b) -> b
    | (a, CO_unset) -> a    
    | (CO_reader, b) -> b
    | (a, CO_reader) -> a    

    | (_, CO_varexp) -> CO_varexp
    | (CO_varexp, _) -> CO_varexp

    | (CO_manifest x, CO_manifest y) -> if x=y then CO_manifest x else CO_varexp
    


(* ---------------------------------------------------------*)

// Enumeration mapping: with and without null pointer capability (null would be last entry)
// List of pairs: (new enum value, old region descriptor)
// Should possibly warn or uniquify over disagreements on region ends!



type sc_classed_t =
   {
       key:        ksc_clskey_t
       lsf:        bool
       ats:        att_att_t list
       has_nullf:  bool              // Whether the null pointer is a member of this class.
       bank:       string option
   }     

let meta_interesting cls = cls.lsf || cls.has_nullf || not_nullp cls.ats || not_nonep cls.bank
    
type region_t = int64 * int64
type region_mapping_t = (int * region_t) list

let power_inc x = A_subsc(x, None)

type subs_term_t = // There are two terms per array indexing operation.
     {
         serno:   string
         bss:     int64 option
         constf:  bool
         descs:   aid_t list   // DefintiveS: the sc descriptions here are those of the subscript expressions (controllers) and we need to later see what regions they point at (control).
         subs:    array_subs_inst_t
     }
and array_subs_usage_t =
    | Mono_use of hexp_t
    | Multi_use of (hexp_t * hbexp_t) list

and array_subs_t = 
| AS_rom of int * it_t * hexp_t * (int * int) option * it_t * bool // Fields are starting exp, rom itself, (base, width), new subscript and a bool that holds for constant subscripts.

| AS_vn of int * constant_write_indicator_t * array_subs_usage_t * subs_term_t * subs_term_t // Fields are serial no, usage expressions, guard?, base item1, subscript item2) 

| AS_rom1 of int * it_t * hexp_t * (int * int) option * it_t * bool // Fields are starting exp, rom itself, (lo, hi), new subscript and a bool that holds for constant subscripts.

// We make a fieldarray when there are multiple instances of a record: ie multiple base item regions with a given record tag/subscription (or potentially for a one element sub array!)
// Create a field array (or find existing) which is a transpose operation
// where an array of records becomes an independent array for each field.
//| AS_fieldarray of it_t * it_t list * string * array_subs_inst_t * (hexp_t * region_mapping_t) // now not used in this form

and array_subs_inst_t =  // Range and annotated ranges
    | AS_leaf of hexp_t * int64 option          // Annotated subscript expression 
    | AS_la of hexp_t * array_subs_inst_t       // Subscript expression with annotation
    | AS_rec of int * hexp_t * array_subs_t     // Recursive subscripting: fields are serial no, ...
    | AS_base_len_pair of (int64 * int64)       // (base, length) pair
    | AS_lh of (int64 * int64)                  // low and high values : addresses in a range or region.
    | AS_fa_2 of hexp_t * string * hexp_t       // original expression, tag name, offset (not needed), 
    | AS_an_map of (int * (int64 * int64)) list * subs_term_t // Multi-Instance (MI) base with associated mapping.
    | AS_an_map_vv of subs_term_t               // Deferred mapping generation now
    | AS_none


let g_blank_sp = { serno="anon"; bss=None; descs=[]; constf=false; subs=AS_none; }



let fa_pred arg =
    match arg.subs with 
         | AS_fa_2 _ -> true
         | _ -> false

let as_item = function
    | AS_vn(ser, _, _, item, _) -> item


let gec_bl msg (bs, len) = 
    if len<=0L then sf (msg + sprintf ": gec_bl %i,%i AS_lh(bs, bs) len < 0 or unspecified"  bs len)
    else AS_lh(bs, bs+len-1L) // Cvt from base,len to l,h

let gec_AS_lh (l,h) =
    if h<l then sf "gec_AS_lh AS_lh(bs, bs)"
    else AS_lh(l, h)


type pis_t = // this should be the same as destiny_t - don't need two types - TODO.
    | P_arithmetic of int64(*scale*) * int64(*offset*) * int64(*newlen*)
    | P_scalarise of int
    | P_mapped of int(*length*) * (int * (string * bool * region_t)) list * sri_digest_t option (*mapping: (region name, nullptrflag, region) *)
    | P_mapped2 of int64 list // please explain

type final_work_name_t = // type branding for temporary developer sanity!
    FWN of string

let dewn = function
    | FWN ss -> ss



let pisToStr = function
    | P_arithmetic(subs_scale, offset, newlen)   -> sprintf "P_arithmetic(%i, %i, %i)" subs_scale offset newlen
    | P_scalarise  n                             -> sprintf "P_scalarise(%i)" n
    | P_mapped(len, mapping, banko)              -> sprintf "P_mapped(%i, _)" len
    | P_mapped2 ilst                             -> sprintf "P_mapped2(%s)" (sfold i2s64 ilst)
    
type ideps_t  = OptionStore<string, final_work_name_t * pis_t>

type rominfo_updates_t = OptionStore<int64, BigInteger>

type rominfo_t = OptionStore<string, bool * constval_t option * rominfo_updates_t>


type groundfacts0_t  = OptionStore<string, memdesc0_t>
type groundfacts1_t  = OptionStore<string, memdesc0_t>
type string_literals_t  = ListStore<int, hexp_t>

type memdesc_map_t  = Dictionary<int64, (string list * memdesc_record_t)>

type flags_t  = ListStore<hexp_t,  int>

// All ops are first of all stored in ops and then xformed to out.
type memdesc_subsbase_ops_t  = ListStore<hexp_t, (int * constant_write_indicator_t) * (array_subs_t * array_subs_inst_t)>



// memdescs indexed by storage class (sc) - delete these when appropriate by swapping out a union form
type memdesc_idx1_t = ListStore<aid_t, string list * memdesc_record_t>

type memdesc_ties_t = aid_t list list ref

type object_remaps_t = Dictionary<string, region_mapping_t>

type wname_idx_t = ListStore<string, aid_t>

// type sc_classes_t = EquivClass<aid_t>

type mapping_forwarding_t = OptionStore<int64, string>

                                      
type pertinent_classes_t = Map<hexp_t, array_subs_t list * aid_t list * att_att_t list> // For each subscripted item we store a triple: 1. the complete list of indexing operations, 2 a list of disjoint regions or partitions that exist in the item's address space and a disused? list of attributes.

type heapconst_t = hexp_t * hexp_t * hexp_t * aid_t list * int64

type region_database_t = ListStore<string, (aid_t list * (int64 * int64)) * bool>

type keys_t = Dictionary<string, int ref>

// Active pattern that can be used to assign values to symbols in a pattern
let (|Let|) value input = (value, input)



(*
sc_classes_db_t: Equivalance classes on storage descriptions:

  R1 - Common parent:  d2->foo and d1->foo are equivalent if d1 == d2.
       The contrarvariant of R2 does not apply - child equivalences imply nothing about their parents - field  names might only be the same by coincidence.

  R2 - Linked list, one-step foldup:   if d1->foo is equiv to d1 then ->foo can always be dropped, hence d1->foo->foo is also equiv to d1.

  R2b - In a typeless src code we might get [d1] == d1 but this will not be present in strongly-typed src code. C++ might just allow { void *a, *b = &a;  a=b; } but thats C++ slackness.

 Note 0:  d1 could be equivalent to d2->bar while d1 != d2. This is not a fold up.

 Note 1: we must make sure we correctly handle  d2 == d3->foo where d2 and d3 are both equiv to d1, hence we get a foldup on d1->foo == d1.  This is no problem, just rewire both sides in preferred terms and it drops out.

 Note 2: there are also multi-step foldups, such as d1->foo->bar == d1 or d1->foo==d2 && d2->bar==d1.   The first example is manifest whereas the second is spotted only by a loop finder. But just having the loop represented in the graph of classes is sufficient provide walkers detect loops as they proceed. Note: tags are now unique (utag/ptag distinction).
*)



type repack_settings_t =
    {
        stage_name:          string
        m_flaws:             string list ref
        k2:                  repack_control_t
        keys:                keys_t                 // A source of unique names
        mfore:               mapping_forwarding_t   // The name/mapping of each numerical region
        sclasses_store: ksc_classes_db_t            // Equivalent storage classes
        //region_database_:     region_database_t   // unread
        //pertinent_classes:   pertinent_classes_t
        groundfacts0:        groundfacts0_t
        groundfacts1:        groundfacts1_t        
        string_literals:     string_literals_t
        heapconsts:          HashSet<heapconst_t>    // Collection of heap addresses in use.
        ideps:               ideps_t                 // Stores item decisions by item serial no
        rominfo:             rominfo_t               // 
        memdesc_map:         memdesc_map_t           // Store for the memdescs from the input VM(s) indexed by base address.
        memdesc_idx1:        memdesc_idx1_t          // memdescs indexed by storage class sc
        memdesc_ties:        memdesc_ties_t          // sc relations indexed by storage class sc
        object_remaps:       object_remaps_t 
        m_next_mci:          int ref
        metastore:           OptionStore<int, sc_classed_t>
        metastore_i:         OptionStore<sc_classed_t, int>
    }        



    
let demeta vinfo mci =
    match vinfo.metastore.lookup mci with
        | None -> sf(sprintf "demeta mci=%i failed" mci)
        | Some cls -> cls

        
let use_union newlst oldlst =
    let decap = function
        | Mono_use x -> [((x, X_true), x2nn x)]
        | Multi_use lst -> map (fun (x,g) -> ((x, g), x2nn x)) lst
    let (xl, yl) = (decap newlst, decap oldlst)
    let add1 ((arg, g), n) cc = 
        let rec scan = function
            | [] -> ((arg, g), n) :: cc
            | ((arg', g'), n') :: tt when n = n' -> ((arg, ix_or g g'), n) :: cc  // Form disjunction of guards
            | other::tt-> other::(scan tt)
        scan cc
    let a0 = List.foldBack add1 xl yl
    Multi_use(map fst a0)
    
let vasToStr = function
    | (Some (baser,limit), b) -> i2s baser + "for" + i2s limit + " " + xToStr b
    | (None, b)               -> xToStr b


let rec assToStr_dl dl = function
    | AS_an_map(mapping, xitem) -> sprintf "AS_an_map(m, %s)" (stToStr_dl dl xitem)
    | AS_leaf(v, co) -> sprintf "AS_leaf(%s)" (xToStr v)
    | AS_la(v, anno) -> "AS_la(" + xToStr v + ", "  + assToStr_dl dl anno + ")"
    | AS_rec(ser, v, i) ->
        if dl>0 then sprintf "AS_rec(S%i, %s, %s)" ser (xToStr v) (asToStr_dl dl i)
        else sprintf "AS_rec(S%i, ...)" ser 
    | AS_lh(l, h)    -> sprintf "AS_lh(%i, %i)" l h
    | AS_base_len_pair(b, w)  -> sprintf "AS_base_len_pair(%i, %i)" b w
    | AS_none        -> "AS_none"
    | AS_fa_2(orig_l, tagkey, offsit)   -> sprintf "AS_fa_2(%s, %s)" tagkey (xToStr offsit)
    //| other          -> sprintf "AAS_v???%A" other


and assToStr arg = assToStr_dl 1 arg
and asToStr arg = asToStr_dl 1 arg    
        
and dToStr (n, ss) = p2ss1 n ss

and asToStr_dl dl arg =
    let z0 = function
        | None -> ""

    let z2 orig =
        let s = xToStr orig
        if strlen s > 12 then s.[0..9] + ".." else s
        
    let z1 = function
        //| AS_fieldarray(l_, elst, token, offsit, (nv, lst)) -> "AS_fieldarray([" + sfold xToStr elst + "] ," + token + ", item=" + assToStr dl offsit + "," + netToStr nv +  ", ..." + i2s(length lst) + ")"
        
        | AS_rom(ser, orig, rom, Some(l,h),  (v), constf) -> sprintf "AS_rom(S%i, " ser + z2 orig + "," + xToStr rom + ", b=" + i2s l + ", w=" + i2s h + "," + xToStr v + ", " + boolToStr constf + ")"    

        | AS_rom1(ser, orig, rom, Some(l,h),  (v), constf) -> sprintf "AS_rom1(S%i, " ser + z2 orig + "," + xToStr rom + ", lohi=(" + i2s l + ".." + i2s h + ")," + xToStr v + ", " + boolToStr constf + ")"    

        | AS_vn(ser, writer, origs, item, offsit) ->
            sprintf "S%i:AS_vn([...]" ser (* useToStr origs *) + "; item1=" + stToStr_dl dl item + "; item2=" + stToStr_dl dl offsit + ")"
            //sprintf "S%i:AS_vn([" ser + useToStr origs + "]; item1=" + stToStr_dl dl item + "; item2=" + stToStr_dl dl offsit + ")"
        | _ -> "AS_v???"
    z1 arg

and useToStr = function
    | Mono_use x -> xToStr x
    | Multi_use lst -> sprintf "Multi(%s)" (sfold (fun (x,g) -> xToStr x + "/" + xbToStr g) lst)

and stToStr_dl dl sp = sprintf "%s:const=%A,desc=%s,subs=%s" sp.serno sp.constf (sfold aidToStr sp.descs) (assToStr_dl (dl-1) sp.subs)

and stToStr sp = stToStr_dl 1 sp




let g_spid_dir = new OptionStore<bool * int64 * string, string>("spid_dir")

// Add a serial no to a subs_term_t.
let add_spid sp =
    let tailer serno = { sp with serno=serno; }
    match sp.subs with
        | AS_fa_2(orig_l, tagkey, offsit)  ->  { sp with serno= tagkey + xToStr offsit; }
        | AS_leaf(v, Some co) ->
            let s = sfold aidToStr sp.descs
            let key = (true, co, s)
            match g_spid_dir.lookup key with
                        | Some serno -> tailer serno
                        | None ->
                            let serno = funique "xuid"
                            let _ = g_spid_dir.add key serno
                            tailer serno

        | AS_leaf(v, None) ->
            match x2nn v with
                | n ->
                    let s = sfold aidToStr sp.descs
                    let key = (false, int64 n, s)
                    match g_spid_dir.lookup key with
                        | Some serno -> tailer serno
                        | None ->
                            let serno = funique "xuid"
                            let _ = g_spid_dir.add key serno
                            tailer serno
                //| None -> sf (sprintf "xer %A " sp)

        | _      -> { sp with serno= funique("uidz"); }    

// export_sc: convert back to Memdesc form.
// move to ksc.fs please        
let ndTox ww vinfo msg aid =
    let sc = sc_export ww vinfo.sclasses_store msg false [] aid
    let xx = xi_stringx (XS_sc sc) (aidToStr aid) // Note: if sc_export is not working properly the problem will be obscured in this presentation!
    xx

// get memdesc0 class: Move to ksc.fs ?
let to_ndsc3a_f_scs memdesc0 = sprintf "%c%i" memdesc0.f_sc_char memdesc0.f_sc

let to_ndsc3a_f_sc memdesc0 = A_loaf(to_ndsc3a_f_scs memdesc0)


    
// import_sc convert from global Memdesc format to local SC_xx form.
// Efficiency: this is called about four times --- better to import once!
let import_sc vinfo sc cc =
    let rec xlate sc cc =
      match sc with 
        | Memdesc0 memdesc0 when memdesc0.has_null -> g_null_aid :: cc // All other info in that record is ignored for now
        | Memdesc0 memdesc0 ->
            let key0 = to_ndsc3a_f_scs memdesc0 // Strangely repeated code 2/2
            let key1 = htos memdesc0.uid.f_name
            vinfo.groundfacts0.add key0 memdesc0
            vinfo.groundfacts1.add key1 memdesc0 
            to_ndsc3a_f_sc memdesc0 :: cc

        | Memdesc_scs scs -> A_loaf scs :: cc

        | Memdesc_sc si  -> muddy "SC_sc si :: cc"

        | Memdesc_ind(lz, None) -> A_subsc(xlate_one lz, None) :: cc

        | Memdesc_ind(lz, Some sz) -> A_subsc(xlate_one lz, Some(xlate_one sz)) :: cc        
       
        | Memdesc_via(lz, tagger) ->
            let ss = htos tagger
            A_tagged(xlate_one lz, ss, ss) :: cc
        | other ->
            vprintln 3 (sprintf "repack: import_scxo: ignore form %A" other)
            sf (sprintf "repack: xlate: ignore form %A" other)            
            cc

    and xlate_one sc =
        match xlate sc [] with
            | [item] -> item
            | other ->  sf (sprintf "repack: xlate: not one sc in %A" other)
    xlate sc cc

// memdesc printing:
let mdpToStr (aid, md) =  htos aid + ": " + mdToStr md

let happenToStr = function
    | true -> "happening"
    | false -> " not happening"

let vcscal_heuristic_pred vinfo msg len =
        let r = vinfo.k2.constidx_rcs = None || len <= valOf vinfo.k2.constidx_rcs
        let _ = vprintln 3 (sprintf "Scalarising %i entries owing to an allconst subscripted region of " len + msg + " is " + happenToStr r)
        r

let vvscal_heuristic_pred vinfo l len =
        let r = vinfo.k2.varidx_rcs = None || len <= valOf vinfo.k2.varidx_rcs
        let _ = vprintln 3 (sprintf "Scalarising owing to being a small array (%i) (with some variable indexing) " len + xToStr l + " is " + happenToStr r)
        r


// Find range of whole class. We will later subtract lower bound from each item.
// Also reap classes
let dynrange_get its msg (sel_grd_, arga) =
    let rec resolve_lg cc arg =
        //vprintln 4 ("lg clause " + asToStr arg)
        match (arg, cc) with //  Range resolution operator
        | (AS_la(_, anno), _) -> resolve_lg cc anno

        // Field array offsets are inconsequential
        | (AS_fa_2(_, tag, v), AS_none)             -> gec_AS_lh(0L, 0L)
        | (AS_fa_2(_, tag, v), AS_lh(ll, hh))       -> gec_AS_lh(min ll 0L, max hh 0L)


        | (AS_leaf(X_num n, _), AS_none)            -> arg
        | (AS_leaf(X_bnum(_, n, _), _), AS_none)    -> arg        

        | (AS_leaf(_, Some nn), AS_lh(l, h))        -> gec_AS_lh(min nn l, max nn h)

        | (AS_leaf _, _) ->
            vprintln 3 (its + sprintf "  Cannot find dynrange info in %s" (assToStr arg))
            cc

        | (AS_lh(l,h), AS_none)           -> AS_lh(l,h)
        | (AS_lh(l,h), AS_lh(l1, h1))     -> gec_AS_lh(min l l1 , max h  h1)

        | (AS_rec(ser, v, _), _) ->
            vprintln 3 (its + sprintf ": dynrange_get : AS_rec need dynrange of S%i : msg=%s. Later get from sc. descs=%s" ser msg (sfold aidToStr arga.descs))
            cc
            
        | (AS_an_map(mapping, _), _) -> muddy "shillaor"
        | (arg, _) -> sf("dynrange_get: lg other " + assToStr arg)
    let lcls cc fx = singly_add fx cc

    let dynrange = resolve_lg AS_none arga.subs
    let classes = List.fold lcls [] arga.descs
    (dynrange, classes)

let get_ser = function
    | AS_vn(ser, _, _, item', offa') -> ser
    | other -> sf ("get ser other "+ asToStr other)

let as_valOf m = function // applicable if constf holds
    | AS_la(hexpt, _)
    | AS_leaf(hexpt, _) -> hexpt
    | other -> sf(m + ": as_valOf other: " + assToStr other)

//
// Turn a set of regions into an enumeration (aka mapping) - associate each with a natural number. 'null' may also be included as a member.
// The mapping can be an extension of a previous one for the same work_name. If multiple work_names already have mappings they should be the same mapping.
//
let seed_enumap (vinfo:repack_settings_t) msg has_nullf scl_ regions =
    // We do not want to conglomorate storage classes in this phase, but we do want a unique name for each mapping - TODO explain further
    let mnames =
        let get_mname cc (baser, _) =
            match vinfo.mfore.lookup baser with
                | None -> cc
                | Some x -> singly_add x cc
        List.fold get_mname [] regions
    let final_work_name =
        match mnames with
            | [] ->
                let newidd = funique "MAPR"
                app (fun (baser, _)-> vinfo.mfore.add baser newidd) regions
                newidd
            | [idd] ->
                app (fun (baser, _)-> vinfo.mfore.add baser idd) regions
                idd

            | multiple -> muddy (sprintf "multiple mapping work names need to be unified: %A" multiple)
            
    // The number of (non null) regions is sometimes greater than number of nemtoks since these may be instantiated more than once (e.g. a malloc inside a loop).
    //let _ = cassert(nregions >= 2, "nregions >= 2")
    // The mapping is performed on a work_names basis and shared over some number of field arrays.
    // Presumably we can compute the mapping later instead, just before creating rewrites to heapconsts.
    let already_has_null = disjunctionate (fun (a,b)->a=0L) regions
    
    let should_include_null =
        if true then has_nullf 
        else 
            let check_null sc =
                let members = muddy "vinfo.sclasses_store.members (aidToStr sc)"
                vinfo.sclasses_store.lookup_null_flag (vinfo.sclasses_store.classOf_i "msg" sc)
            disjunctionate check_null  scl_

    let _ =
        let check_null_is_zero (a, b) = if b=0L && a <> 0L then sf ("Null was not denoted with zero in mapping")
        app check_null_is_zero regions
    let regions = if should_include_null && not already_has_null then (0L,0L)::regions else regions
    // If we want to extend an existing mapping without null so that it now has null then we cannot keep the null mapping denoted with 0. A renumbering would be needed.
    let nregions = length regions
    vprintln 3 (sprintf "seed_enumap:  scl=%s  nregions=%i  msg=%s  work_name=%s  already_has_null=%A  should_include_null=%A" (sfold aidToStr scl_) nregions msg final_work_name already_has_null should_include_null)

    let mapping = List.zip [0..nregions-1] regions
    let mp2str (a, (b:int64, _)) = sprintf "%i:%i" a b
    let storise xkey =
        let (found, ov) = vinfo.object_remaps.TryGetValue xkey
        if found then
            if ov <> mapping then
                //vprintln 3 ("New and old mapping differ:" + msg + " key=" + xkey + "\nNew=" + sfold mp2str mapping + "\nOld=" + sfold mp2str ov)
                let new_ones =
                    let old_bases = map (snd >> fst) ov
                    let not_there_before (v, _) = not(memberp v old_bases)
                    List.filter not_there_before regions
                let startvale = length ov
                let augments = List.zip [startvale .. startvale + length new_ones - 1] new_ones
                let nowhave = ov @ augments
                vprintln 3 (sprintf "Extended/consolidated, %i further entries, giving " (length new_ones) + sfold mp2str nowhave)
                let _ = vinfo.object_remaps.Remove(xkey) in vinfo.object_remaps.Add(xkey, nowhave)
                nowhave
            else
                vprintln 3 ("using existing mapping for key=" + xkey)
                ov
        else
            vprintln 3 ("Storing/seeding mapping (" + msg + ") len=" + i2s(length mapping) + "; mapping=" + sfold mp2str mapping + " for regions key=" + xkey)
            vinfo.object_remaps.Add(xkey, mapping)
            mapping
    let nowhave = storise final_work_name
    (FWN final_work_name, nowhave, nregions)
    //let sorter (a, _) (b, _) = b-a
    //(List.sortWith sorter nowhave, nregions)

let conglomorate2 ww msg (vinfo:repack_settings_t) args =
    let wx = vinfo.sclasses_store.conglom msg args
    wx:int
    



//
// Create a field array (or find existing) which is a transpose operation
// where an array of records becomes an independent array for each field.
//
let generate_field_arr_details ll len token constidx_replacement_nets =
    if len=0L then
        let _ = vprintln 3 ("skip create of zero length field array " + token)
        None
    else
    let w = encoding_width ll
    match ll with
        | X_bnet ff ->
            let f2 = lookup_net2 ff.n
            let id = xToStr ll + "$" + token
            let ov = op_assoc id (!constidx_replacement_nets)
            if not (nonep ov) then ov
            else
            //let msg = xToStr ll
            let vol(f:net_att_t) = at_assoc "volatile" f2.ats = Some "true"
            vprintln 0 "            // Wrong f ... want memdesc ats! TODO - get from regions ?"
            let ats = f2.ats @ (if vol ff then [ Nap("volatile", "true") ] else [])

            let dims = if len=1L then (vprintln 3 ("Unity length field array, using scalar for " + id); [])
                       else [len]
            let ff2 =
                let (n2, ov) = netsetup_start id
                match ov with
                    | Some vv -> vv
                    | None ->
                        vprintln 3 ("gen field array:" + token + sprintf " width=%i len=%i" w len)
                        let ff = 
                            { ff with 
                               n=        n2
                               rh= -1I; rl= -1I; width=w;
                               id=       id
                               is_array= not_nullp dims
                            }
                        let f2 =
                            { f2 with
                               xnet_io=  LOCAL
                               vtype=    V_VALUE
                               ats=      ats
                               length=   dims
                            }
                        netsetup_log (ff, f2)
            let ans = X_bnet ff2
            mutadd constidx_replacement_nets (id, ans)                  
            Some ans


let as_manifest m = function
    | AS_leaf(v, _) 
    | AS_la(v, _) 
    | AS_rec(_, v, _)   -> xi_manifest64 m v                  
    | AS_fa_2(orig_l, id, offset) -> xi_manifest64 m offset // Not useful - all tags offsets are irrelevant constants in 'safe' code.  TODO.
    | other -> sf("as_manifest unsupported form : " + assToStr other)

let as_fconstantp = function
    | AS_leaf(v, Some _) -> true
    | AS_leaf(_, None)   -> false
    | AS_la(v, _) -> fconstantp v
    | AS_rec(ser, v, i) -> fconstantp v
    | AS_fa_2 _ -> true
    | other -> sf("as_fconstantp unsupported form " + assToStr other)

let div2 arg hcf =
    if hcf < 2L then arg
    else ix_divide arg (xi_num64 hcf)

let regionToStr ((a:int64),(b:int64)) = sprintf "(%i:%i)" a b

let nextkey (vinfo:repack_settings_t) k0 =
        let (found, ov) = vinfo.keys.TryGetValue(k0)
        let key = if found then ov
                  else let kr = ref 0
                       let _ = vinfo.keys.Add(k0, kr)
                       kr
        let v = !key
        let _ = key := v+1
        let rec base26 s =
            let a = System.Convert.ToString(chr(s % 26 + 65))
            if s >= 26 then (base26(s/26) + a) else a
        base26 v

type disparse_t = // intermediate structure for sc annotation parsing.
    {
        isROM:      bool
        strings:    aid_t list
        field:      string option
        cls:        string option
        offset:     bool
        hx:         hexp_t option
    }


(*
 *
 * Scan for array operations and record nature of subscripts used.
 * Looking inside subscripts for nesting is done by the walker.
 *
 * This creates basic AS_vn entries. Writing them to opsref:memdesc_subsbase_ops_t
 * Later?? hmm it is easier here we recursively expand them in xform to have better-annotated leafa style entries.
 *)
let arrayOpLogger ww (vinfo:repack_settings_t) aops writer argA = 
  let vd = vinfo.k2.loglevel   //dev_println (sprintf "repack: arrayOpLogger vd=%i" vd)

  let cong spanners =
      if nullp spanners then []
      else
          let _ = conglomorate2 ww "aol" vinfo spanners
          [hd spanners] // Any one representative is sufficient. Better to use a member than the mci returned by conglom since the mci can change as we go.


  let rec log1 m argA = 
    let ser = next_ser()
    if vd>=4 then vprintln 4 (sprintf "arrayOpLogger S%i of " ser + xToStr argA)
    match argA with
    | W_asubsc(lhs, usage, _) ->
        let ann = x2nn argA
        let lnn = x2nn lhs
        let existing_items = (aops:memdesc_subsbase_ops_t).lookup lhs
        match op_assoc (ann, writer) existing_items with
            | Some ov when true ->
                let ans = snd ov
                // Writer may be different - a read and a write to a common subscript is a common programming idiom.
                if vd >= 5 then vprintln 5 (sprintf "reuse existing vn for %s vn=%s" (xToStr argA) (asToStr (fst ov)))
                ans
            | _ ->
                let char_find = is_string lhs
                let kf = fconstantp usage
                let its = xToStr lhs
                if vd >= 5 then vprintln 5 (m + sprintf ": array_op_log note for %s " its + ": r=" + xToStr usage + ", const=" + boolToStr kf)
                let rec mining (q:disparse_t) = function
                    | X_pair(a, b, _) -> mining (mining q a) b
                    | W_string(ss, qf, _) ->
                            match qf with
                                | XS_sc scl -> { q with strings=(List.foldBack (import_sc vinfo) scl []) @ q.strings }
                                | _ ->
                                    let to_ndsc4 sci = A_loaf (sprintf "imp%i" sci) // Old, string-parsing based  import code. 'imp' is now the wrong string anyway, if resurrected.
                                    let l = strlen ss
                                     //vprintln 3 ("mining of string parse " + ss)
                                    if l >= 4 && ss.[0..3] = "ROM" then { q with isROM=true; }                   // As an old backdoor, "ROM" is inserted in the aid of readonly strings.
                                    elif l >= 8 && ss.[0..7] = "$offset+" then { q with field=Some(ss.[8..]); }
                                    elif l >= 7 && ss.[0..6] = "$offset" then  { q with offset=true; }
                                    elif l >= 4 && ss.[0] = '[' then
                                            //vprintln 0 (sprintf "[ ] sscanf applied to %s" ss)
                                            let si = sscanf (if ss.[1] = 'd' then "[d%i]" else "[c%i]") ss
                                            let inds = -1//if qf = XS_unquoted then -1 else 0 
                                            { q with strings= to_ndsc4 si :: q.strings }
                                    else
                                            //vprintln 0 (sprintf "sscanf applied to %s" ss)
                                            let si = sscanf (if ss.[0] = 'd' then "d%i" else "c%i") ss
                                            let inds = 0//if qf = XS_unquoted then -1 else 0 
                                            { q with strings= A_subsc(to_ndsc4 si, muddy "L878 None") :: q.strings } // TODO check this NONE is correct
                    | oo ->
                        if nonep q.hx then { q with hx=Some oo; }
                        else sf ("repack: Cannot disassemble/parse array index markup " + xToStr usage)
                let n1 = 
                    if char_find then 
                        if vd>=4 then vprintln 4 ("Simple unannotated string indexing " + its + " with " + xToStr usage)
                        // TODO? Perhaps send round again after adding the X_pair annotation to the usage
                        let desc = A_loaf (funique "$stringlit") // SC_ind(SC_sc 0) // can do a withval strlen its ?
                        let co = (if constantp usage then Some((int64)(xi_manifest "L1103" usage)) else None)
                        let item = {  g_blank_sp with descs=[desc]; constf=kf; subs=AS_leaf(usage, co); bss=None;  }
                        let ans = AS_vn(ser, writer, Mono_use usage, item, item)
                        if vd >= 4 then vprintln 4 ("logged modern char_find vn " + asToStr ans)
                        ans
                    else 
                        let discoveries =
                            let normarrow x = x
                            let mfun = (mining { isROM=false; strings=[]; field=None; offset=false; cls=None; hx=None; } >> normarrow)
                            match usage with
                                | X_num _
                                | X_bnum _
                                | X_pair _ -> [ mfun usage ] 
                                | W_node(prec, V_plus, lst, _) -> map mfun lst
                                | other ->
                                    if vd >= 3 then vprintln 3 (sprintf "Repack other form monitor: L1032: key=%s other=%s" (xkey other) (netToStr other))
                                    [ mfun usage ] 
                        let (isROM, pitem1, pitem2) =
                            match discoveries with
                                | [ a; b] ->
                                     if vd>=6 then vprintln 6 (sprintf "isoffset/field a=%A %A %A;   b=%A %A %A" a.offset a.field a.strings b.offset b.field b.strings)
                                     if b.offset || not (nonep b.field) // Sort so that item2 is the field - it acts like a constant rhs to the subscript operator.
                                     then (a.isROM || b.isROM, (cong a.strings, valOf a.hx), (cong b.strings, valOf b.hx, b.field))
                                     else (a.isROM || b.isROM, (cong b.strings, valOf b.hx), (cong a.strings, valOf a.hx, a.field))
                                | [a] -> //Diadic addition of zero gets discarded, so we just get the base here and re-infer the 0 offset.
                                     (a.isROM, (cong a.strings, valOf a.hx), ([], xi_num 0, None))
                                | _ -> sf ("repack: diadic plus in array subscript expected in " + xToStr usage)                                    
                        let item1 = log1 ("sub1 " + m) (snd pitem1) // Recursive calls on inner subscripts.
                        let item2 = log1 ("sub2 " + m) (f2o3 pitem2)
                        if nullp (fst pitem1) && nullp (f1o3 pitem2) then hpr_yikes(sprintf "repack: lhs=%s: both item1 and item2 have no description: subsc=%s" (xToStr lhs) (xToStr usage))
                        if isROM then // This annotation technique is not used at the moment by Kiwi. (Instead ROMs are inferred locally here inside repack.)
                              let item = snd pitem1
                              let rom_length = function
                                      | W_string(s, _, _) -> 2*strlen(s) // When indexed for char access.
                                      | other -> sf("repack rom_length other " + xToStr other)
                              if vd >= 3 then vprintln 3 ("Treating as ROM: " + xToStr argA)
                              let b = 0
                              let l = rom_length item // bit of a bodge at the moment!  discards type!
                              AS_rom(ser, usage, item, Some(b, l), f2o3 pitem2, kf)
                        else
                            let gec_ps descs subs =
                              let kf = as_fconstantp subs
                              {  g_blank_sp with descs=descs; constf=kf; subs=subs; bss=None; }
                            let gec_ps_via ft descs subs =
                              let kf = as_fconstantp subs
                              let add_via sc = A_tagged(sc, ft, ft)
                              {  g_blank_sp with descs=map add_via descs; constf=kf; subs=subs; bss=None; }
                            let ans =
                              match f3o3 pitem2 with
                              | None  ->AS_vn(ser, writer, Mono_use usage, gec_ps (fst pitem1) item1, gec_ps (fst pitem1 @ f1o3 pitem2) item2) // data array - put both descs in item2 for now - a kludge?
                              // Put the fa in item2 
                              | Some ft->AS_vn(ser, writer, Mono_use usage, gec_ps (fst pitem1) item1, gec_ps (f1o3 pitem2) (AS_fa_2(argA, ft, f2o3 pitem2)))
                            if vd>=4 then vprintln 4 (sprintf "logger: writer=%A: logged modern vn " writer + asToStr ans)
                            ans

//                      | usage -> 
//                          lprintln 0 (fun()-> its + sprintf ": No descriptive pair tags found in " + xToStr usage)
//                          muddy (sprintf "bad usage: its=%s: key=%s usage=%s ast=%A other usage char_find=%A" its (xkey usage) (xToStr usage) usage char_find)

                if vd>=3 then vprintln 3 ("Result stored for AS_rec is: " + asToStr n1)
                let vn = AS_rec(ser, argA, n1) // This AS_rec is really only needed for nested aops, but we create it always in case there is a parent.
                aops.add lhs ((ann, writer), (n1, vn))
                vn
    | X_num n -> AS_leaf(argA, Some(int64  n))
    | other ->
        // This is ok: generally a raw pointer variable.
        if vd>=3 then vprintln 3 ("arrayOpLogger other: " + xToStr other)
        let co = (if constantp argA then Some(xi_manifest64 "L1103" argA) else None)
        AS_leaf(argA, co) 
  log1 "top" argA

let conglom_vn_contents ww vinfo msg vn =
    let conglom_item item =
        if length  item.descs > 0 then
            let (cls) = conglomorate2 ww "conglom_item" vinfo item.descs // was normalise_sp_names vinfo msg item.descs
            ()
    match vn with
        | AS_vn(ser, writer, usages, item1, item2) ->
            let _ = conglom_item item1
            let _ = conglom_item item2
            ()
        | other -> ()

let g_repack_cls_char = 'd'
        
let save_classed site vinfo classed =
    let vd = vinfo.k2.loglevel
    let clsToStr cls = sprintf "classed{clskey=%s bank=%A literalstring=%A has_nullf=%A}" (vinfo.sclasses_store.sciToStr cls.key) cls.bank cls.lsf cls.has_nullf
    if true || meta_interesting classed then
        match vinfo.metastore_i.lookup classed with
            | None ->
                let mci = !vinfo.m_next_mci
                mutinc vinfo.m_next_mci 1
                vinfo.metastore.add mci classed
                vinfo.metastore_i.add classed mci
                vprintln 3 (site + sprintf ": Allocate mci=MCI%i for class %s" mci (clsToStr classed))
                A_metainfo mci
            | Some ov ->
                if vd >= 4 then vprintln 4 (sprintf "Retrieve saved MCI%i for %s" ov (clsToStr classed))
                A_metainfo ov
    else A_loaf (vinfo.sclasses_store.sciToStr classed.key) // TODO use this but we desire cleaner support for parsing classes out of A_loaf really

let rename_vn ww vinfo msg vn =
    let vd = vinfo.k2.loglevel
    let rename_sc site has_nullf item =
        let item = 
            if nullp item.descs then item
            else
                // This call to conglomorate should do no merging owing to the pass=0 calls to conglom.
                let sci = conglomorate2 ww "rename_sc" vinfo item.descs 
                let banko = vinfo.sclasses_store.attribute_get "bank" sci
                let literalstring_flag = 
                    let zmembers = vinfo.sclasses_store.members -1 sci
                    let lit_string_flag_find cc (_, aid) = 
                        match aid with 
                            | A_loaf scs -> 
                                 match vinfo.groundfacts0.lookup scs with
                                     | None -> cc
                                     | Some memdesc -> memdesc.literalstring :: cc
                            |  _ -> cc // (sprintf "repack pussy: other form %A" arg)

                    //vprintln 0  (sprintf "wx=%s zmembers %A" wx zmembers)
                    let dd = List.fold lit_string_flag_find [] zmembers
                    not_nullp dd && disjunctionate id dd

                let ans = { item with descs=[ save_classed "L993" vinfo { key=sci; bank=banko; lsf=literalstring_flag; ats=[]; has_nullf=has_nullf}] } 
                if vd >= 5 then vprintln 5 (msg + sprintf " rename_vn: %s,  descs were %s and now are %s" site (sfold aidToStr item.descs) (sfold aidToStr ans.descs))
                ans
        add_spid item

    let has_nullf scl =
        let nullf_check sci = vinfo.sclasses_store.lookup_null_flag sci
        disjunctionate nullf_check (map (vinfo.sclasses_store.classOf_i "has_nullf") scl)

    match vn with
        | AS_vn(ser, writer, usages, item1, item2) ->
            if vd >= 5 then vprintln 5 (msg + sprintf " rename_vn of S%i" ser)
            let has_nullf1 = has_nullf item1.descs
            let has_nullf2 = has_nullf item2.descs
            if vd >= 5 then vprintln 5 (sprintf "rename:_vb S%i : has null flags pox are %A %A" ser has_nullf1 has_nullf2)
            AS_vn(ser, writer, usages, rename_sc "item1" has_nullf1 item1, rename_sc "item2" has_nullf2 item2)
        | other ->
            muddy (sprintf "rename_vn of other form %s " (asToStr other))
            other
              
// xformer: 
// ll is the item under consideration
// All operation are first stored in ops and then decisions about xform stored in out.
// xformer0 - this is just a call to conglom? nothing is xformed?
let rec xformer0 ww (vinfo, marked:flags_t) (pertinent_classes:pertinent_classes_t) lhs_ll = 
    let its = xToStr lhs_ll
    let _ = WF 3 (sprintf "xformer0") ww (sprintf "start on %i lhs=%s" (x2nn lhs_ll) its)
    let (lst0, wlst0_, ats_) = valOf_or_fail "L1034" (pertinent_classes.TryFind lhs_ll) // Find all work for this lhs (ie for this input array).
    vprintln 3 (sprintf "lhs=%s: %i items of work" its (length lst0))
    app (conglom_vn_contents ww vinfo "L953") lst0
    ()



// Collate subscript expressions within a tag group and stored in the wlst of a new, shared AS_vn that has a fresh serial no.
let xform1_collate lst0 = 
    // Partitioning is linear when we keep the discards at each point and we can also then easily trap anything left out since it remains at the end.
    let collate lst0 = // XF3: Group together operations that differ only by originally-specified sc within an scl and rewrite with shared elist where desc is leader.
//    Hmmm TODO - we really want a union of regions and certainly not a conglomoration of target classes.
        let (named, anon) =
            let vn_pred = function
                | AS_vn(ser, _, elst, item1, offa) -> true
                | _ -> false
            List.partition vn_pred lst0                
//This is really the first partition ?  NO - TODO explain
            // This collates multiple 'identical' vn's to a single.
            // - we seek identity on item1 and item2 where each has its leader inserted as its sc
//Everything is collated now, so no need to keep old names and so on
//We can do it on serial nos infact?  Indeed already mostly done, so this step is not needed ... 
        let report_group_leaders =
            let gtl cc = function
                | AS_vn(ser, _, elst, item1, item2_offa) -> singly_add (item1, item2_offa, item1.serno, item2_offa.serno) cc
            List.fold gtl [] named
        //vprintln 3 (its + sprintf " report_group_leaders are ^%i  %s " (length report_group_leaders) (sfoldcr (fun x->sprintf "   item1.serno -- item2.serno --> %s -- %s " (f3o4 x) (f4o4 x)) report_group_leaders))
        let colit (named_in, named_out) (item1, item2_offa, i0s, i1s) =
            let grooma (w, c, sc, others) arg =
                match arg with  // The use_union creates the disjunction that is later made into a multiplexor by ARFF
                | AS_vn(ser, writer, xuse, item', item2_offa') when i0s=item'.serno && i1s=item2_offa'.serno -> (co_disjunction(w, writer), use_union xuse c, ser :: sc, others)
                | _ -> (w, c, sc, arg::others)
            if nullp named_in then 
                vprintln 3 (sprintf "Strange: null named_in for i0s=%s  i1s=%s" i0s i1s)
                (named_in, named_out)
            else
                let (writer, elst, sers, named_in) = List.fold grooma (CO_unset, Multi_use [],[],[]) named_in
                if writer=CO_unset then 
                    vprintln 3 (sprintf "Strange: writer remains unset for i0s=%s  i1s=%s" i0s i1s)
                    (named_in, named_out)
                else
                    let ser' = next_ser()
                    vprintln 3 (sprintf "id=%s Collating as one AS_vn under S%i the following: " (sfold aidToStr item1.descs) ser' + sfold (fun x -> "S" + i2s  x) sers)
                    let nv = AS_vn(ser', writer,  elst, item1, item2_offa)
                    (named_in, nv::named_out)
            
        let (left_over, named') = List.fold colit (named, []) report_group_leaders           
        if not (nullp left_over) then sf (sprintf "colit: had some operations left over %A" left_over)
        anon @ named'
    let ans = collate lst0
    ans // end of xform1_collate


// xformer1_top - Convert to v1 form: l and h margins, instead of base and limit, Share single copy of identicals, and so on.
// 
// Even within a store class, array ops may be manifestly disjoint, so now make numerical checks on pointer aliases as far as is readily decidable.

// xform1: The first transformation (repacking decision) for a subscription - explodes all
// a later repack will typically follow for efficient DRAM use patterns...
    
// If we make the assumption that a given heapconstant is only used in one controlling class then we do not need the up-then-down operation to find the appropriate mapping for it.   This holds: if it occurred in more than one then it must have been passed by reference and those classes should be unified, unless we consider uni-directional transfers, which we dont.  Its just possible the range of heap consts in one section of code is a subset of that in another section where flow passes from the former to the latter.  We skip that idea.


// xform1 decides between the four forms of indexing: each form is a pair made of  SI/MI - single instance or multiple instance - for item1 generally.  F/DA field or data array - for item2 generally.

// SI F - a singleton scalar (not commonly found in heapspace infact).

// MI F - a set of scalars or perhaps the MI is partitioned to give one or more field arrays if arity of MI is large (so it effectivel can become  F DA)

// SI DA - a data array with range scale (common factor divide and base subtract)

// MI DA - could become 2-D arrays in the future, but generally, for small arity a mux over data arrays that are all range scaled with common parameters.

// When DA is low arity it is restructure that maps to register files and so on.  The same for a field array.
    // We generally aim for an indexable form by converting MI value with a random pattern to an (indexable) sequence of positive integers by generating a mapping function.
    // Factor and range analysis is used on arithmetically indexed arrays whereas a mapping is used for sparse patterns.
    // TODO describe heuristic parameters regarding sparseness and scalarising small sets. e.g. if all subscripts are constants we do not scalarise for large sets of constants.
    // But note that the HCF analysis etc can be used on both MI and DA forms, but the MI form, when mapped, trivially has hcf=1 and offset=0.
    // Determine whether a field array (MI-F).  Make a mapping for MI or sparse DA forms.

let xformer1_top ww (vinfo) (pertinent_classes:pertinent_classes_t) cc lhs_ll lst0 wlst0 ats_ =
    let its = xToStr lhs_ll
    let _ = WF 3 (sprintf "xformer1") ww ("start on lhs=" + its)
    vprintln 3 (sprintf "xformer1: worklist (wlst0) for its=%s lengths are lst0=^%i and wlst0=^%i. Details=" its (length lst0) (length wlst0) + sfold aidToStr wlst0)
    let lst0 = map (rename_vn ww vinfo "L964") lst0
//    let pertinent_classes = pertinent_classes.Add (lhs_ll, (lst0, wlst0, ats_)) // Store back post rename 
#if SPARE
    let rec add_info_lg = function
        | ([(n:int64)], AS_none)       -> AS_leaf(xi_num64 n, Some n) // Deploy AS_leaf for a constant 
        | ([(n:int64)], AS_base_len_pair(l, h)) -> gec_bl its (min l n, max n h)
        | ([], dd) ->
            hpr_yikes (sprintf "+++ no annotation added to subscript expression %s" (assToStr dd))
            dd // No annotation added... perhaps warn since likely to lead to "array op left over" later on.
        | (lst, dd) ->
            if length lst >= 2 then hpr_yikes (sprintf "+++ hdcase lowget add_info_lg - more than one annotation at ass=%s for %s " (assToStr dd) (sfold i2s64 lst))
            // Arbitrary return of the hd?
            add_info_lg([hd lst] (*arb!*), dd)
    let rec lohi_collect_constants = function
        | W_query(g, p, q, _) -> lst_union (lohi_collect_constants p) (lohi_collect_constants q) 
        | W_node(prec, V_plus, [X_num n; e], _)
        | W_node(prec, V_plus, [e; X_num n], _) -> singly_add (int64 n) (lohi_collect_constants e)
        | W_node(prec, V_times _, _, _) -> []
        | X_num n -> [ int64 n ]
        | (other) -> (vprintln 1 ("+++ lohi_collect_constants other " + xkey other + " other=" + xToStr other); [])

    let lohi_annotate1__ v = function
        | X_num n -> add_info_lg ([int64 n], v)
        | X_pair(X_pair(X_num l, X_num w, _), _, _) -> sf "done already ?? in op note:  lg([l], lg([l+w-1], d))"
        | exp -> add_info_lg (lohi_collect_constants exp, v) 

    let lohi_annotate2__ v a =
        match a with 
           | AS_leaf(x, _) -> lohi_annotate1 v x
#endif

    let lst0 = xform1_collate lst0
      
    let rec xform1 arg xcc =
        let msg = (sprintf "for %s on %s " its (asToStr arg)) 
        let ww = WF 3 "repack" ww ("\nXF1.00 xform1 " + msg)
        let ans =
            match arg with
            | AS_rom(ser, e, rom, Some(b,w), e', kf)  -> (AS_rom1(ser, e, rom, Some(b, b+w-1), e', kf))
            | AS_vn(ser, writer_, elst, item1, item2) ->
                let region_final_collate msg xitem =
                    let fieldo = match xitem.subs with
                                  | AS_fa_2(ll, key, offsit) -> Some key
                                  | _ -> None
                    vprintln 3 (sprintf "XF1.048: %s: fieldo=%A item=%s sc=%s" msg fieldo (stToStr xitem) (sfold aidToStr xitem.descs))
                    if not_nonep fieldo then (xitem, [])
                    else
                        let ndsc_serf (moders0, classes0) ndsc = ("all/quoted/immediate"::moders0, ndsc::classes0)
                        // generate a mapping - if some classes are already in a mapping use that, if they are split over more than one then a problem that needs eliding.
                        let (moders_, vimclasses) = List.fold ndsc_serf ([], []) xitem.descs // Just a reverse/nop. Delete me.
                        vprintln 3 (sprintf "XF1.049: %s: classes=%s " msg (sfold aidToStr vimclasses))
                        (xitem, vimclasses)

                let (i1, i2) = (region_final_collate "item1-col" item1, region_final_collate "item2-col" item2)

                let yieldx msg (xitem, classes_) =  // THIS IS A NOP - DELETE ME - no field arays made ever at this point in the code!
                    //vprintln 3 (sprintf "yieldx: %s" msg)
                    (None, X_true, xitem) // TODO do not need a list here since cartesian product is now later on.

                let (l1, l2) = (yieldx "item1-yld" i1, yieldx "item2-yld" i2) 
                // Sort FA if we wish here:
                match (l1, l2) with
                    | ((fa1, g1, j1), (fa2, g2, j2)) ->
                        let l2__ = if not (nonep fa1) then valOf fa1 elif not(nonep fa2) then valOf fa2 else lhs_ll // Old code: Insert field-array basis.  There should be at most one of them.  Not used!
                        (AS_vn(ser, writer_, elst, j1, j2))
            | other -> sf (vinfo.stage_name + ": xform1 other " + its + " " + asToStr other) //  + " regions=" + i2s (length regions))

        match ans with
                | answer ->
                    vprintln 3 ("xform1 returns " + (fun b -> xToStr lhs_ll + ": " + asToStr b) answer + ";")
                    //cassert(ans <> [], "ans<>[]")
                    answer ::  xcc 
                    
    let lst1 = List.foldBack xform1 lst0 [] // process each collated set of operations in isolation - (could do later if mapping made later)
    let xformer1_ans = (lhs_ll, lst1, wlst0, ats_) :: cc
    xformer1_ans // End of xformer1_top


(*
 * First pass for a VM.
 *)
let rec repack_one_vm ww mMSC (vinfo:repack_settings_t) nats = function
    | (ii, None) -> nats
    | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->        
        let vd = vinfo.k2.loglevel
        let m = (" id=" + vlnvToStr ii.vlnv)
        let ww' = WF 3 "repack_one_vm" ww m 

        // Import: Convert from memdesc form to our own         
        let to_ndsc3b memdesc0 =
            let shared_resource_info = not_nonep memdesc0.shared_resource_info
            let key0 = to_ndsc3a_f_scs memdesc0 // Strangely repeated code 1/2
            let key1 = htos memdesc0.uid.f_name
            vinfo.groundfacts0.add key0 memdesc0
            vinfo.groundfacts1.add key1 memdesc0        
            let aid = to_ndsc3a_f_sc memdesc0
            //dev_println (sprintf "Grocking/readin memdesc key0=%s key1=%s aid=%s" key0 key1 (aidToStr aid))
            aid

        let _  =
            let kx0 = function
                | (tie_no_, Memdesc0 md) -> // Put all id's into one eqiv class and use first member for now on
                    let sc_lst = [ to_ndsc3b md ]
                    //let _ = conglomorate2 ww "L1081-repack-kx0a" vinfo sc_lst
                    ()
                | (tie_no, Memdesc_tie mdl) -> // Put all id's into one eqiv class and use first member for now on
                    let sc_lst = List.foldBack (import_sc vinfo) mdl []
                    let (has_null, sc_lst) = if memberp g_null_aid sc_lst then (true, list_subtract(sc_lst, [g_null_aid])) else (false, sc_lst)
                    let clsof aid = vinfo.sclasses_store.classOf_i "clsof"  aid
                    let has_bank =
                        let gbank cc = function
                            | Memdesc0 md ->
                                match md.shared_resource_info with
                                    | None -> cc
                                    | Some bank -> singly_add bank cc
                            | _ -> cc
                        List.fold gbank [] mdl
                    if not_nullp has_bank then
                        if length has_bank <> 1 then sf (sprintf "More than one memory bank requested for pool %s" "blah")
                        let setbank arg =
                            let clskey:int = clsof arg
                            vprintln 3 (sprintf "Note hasbank flag in sc%i" clskey)
                            vinfo.sclasses_store.attribute_set clskey "bank" (sri_digest (hd has_bank))
                            ()
                        app setbank sc_lst

                    if has_null then
                        let setnull arg =
                            if vd>=4 then vprintln 4 (sprintf "Note null member flag in %s" (aidToStr arg))
                            vinfo.sclasses_store.setNullFlag "msg" arg
                        app setnull sc_lst
                    mutadd vinfo.memdesc_ties sc_lst
                    // No - do not conglomorate here: instead this is done properly after all ties and zeroth mappings are conglomorated.
                    //let _ = conglomorate2 ww "L1081-repack-kx0b" vinfo sc_lst
                    ()

            let kx2 nv0 =
                match nv0 with
                    | (i, Memdesc0 md0) ->
                        let sc = to_ndsc3a_f_sc md0
#if OLD
                        let key1 = aidToStr sc 
                        let _ =
                          if md.f_nemtok <> []
                          then
                              let key0 = htos md.f_nemtok // have to htos it since it was later compared with a string is later extracted from a $offset field. Not used now?
                              if nvd >= 5 then vprintln 5 ("   MD0 Noted m0 memdesc " + key0 + " ::: " + mdpToStr nv0)
                              //vinfo.memdesc_idx0.add key0 nv0
                              ()
                        if nvd >= 5 then vprintln 5 ("   MD0 Noted m1 memdesc " + key1 + " ::: " + mdpToStr nv0)
#endif
                        vinfo.memdesc_idx1.add sc nv0

                    | (i, _) -> ()

            let kx3 nv0 =
                match nv0 with
      (*
         //This work now in collate:
                      let link_to_sc cc nemtok = // we need to augment the points_at2 entries since they are incomplete.
                          let target_scl = vinfo.memdesc_idx0.lookup (htos nemtok)
                          let vias_ = md.via // TODO ignored here
                          let pr cc = function
                              | (_, Memdesc0 x)-> x.sc @ cc
                              | _ -> cc
                          let shot = List.fold pr cc target_scl
                          let _ = vprintln 0 (sprintf "inferred points at link_to %s / %A via %A" (htos nemtok) (sfold p2ssx shot) vias_)
                          shot
                      let additionals = List.fold link_to_sc [] md.points_at1

                      let mdv sc = 
                         let key1 = p2ssx sc 
                         let _ = lprintln nvd (fun () -> "  MD1 Noted m1 memdesc " + key1 + " ::: " + mdpToStr nv0)
                         let x = { md with points_at2 = list_union(additionals, md.points_at2) }
                      let _ = app mdv md.sc
                      ()
      *)
                      | (i, _) -> ()

      // Do this in multiple passes so we can infer inter-class relationships from MD1 entries that relate an sc to just an aid. 
            app kx0 minfo.memdescs // Copy/re-index data from memdescs to idx1 and sc_classes
            app kx2 minfo.memdescs
            app kx3 minfo.memdescs
            ()
        //reportx 3 "memdescs repack" mdpToStr (mdescs_sort minfo.memdescs)
        let nats = List.fold (repack_one_vm ww' mMSC vinfo) nats sons
        let vdp = true
        let _ = map (walk_exec ww vdp mMSC) execs
        let _ = unwhere ww
        let nats =
              let kx4 cax = function
                  | (i, Memdesc0 md) ->
                      //let _ = vprintln 0 (sprintf "scrib ats %A" md.mats)
                      let kk = int64 md.uid.baser
                      let nv = (i, Memdesc0 md)
                      let (found, ov) = vinfo.memdesc_map.TryGetValue kk
                      let _ =
                          if found then
                              let _ = vinfo.memdesc_map.Remove kk
                              let _ = if kk <> int64 g_unaddressable then vprintln 3 (sprintf "+++Map entry redefined at %i  old=%A new=%A" kk (ov) nv)
                              ()
                      let _ = vinfo.memdesc_map.Add(kk, nv)
                      lst_union md.mats cax
                  | (i, _) -> cax
              List.fold kx4 nats minfo.memdescs
        lst_union nats minfo.atts



(*
 * Second pass repack - writes out the new machines following the input machine hierarchic structure.
 * The memdesc memory descriptions are all consumed in this stage.  We do not currently add new ones in the output. But need to?
 *)
let repack_two_dic ww kK2 msg (vinfo, mm) dir dic =
    let vd = -1
    //let ww = WF 3 "repack_two_dic" ww "rewrite_h2sp start" 
    let r = rewrite_h2sp (WN "rewrite_h2sp dic" ww) vd dir X_true mm dic
    r:HPR_SP


let rec repack_two_vm ww kK2 (vinfo:repack_settings_t, array_mapping) newnets vm =
    let vd = -1
    match vm with
        | (ii, None) -> (ii, None)
        | (ii, Some(HPR_VM2(minfo, decls, sons, execs, assertions))) ->
            let ww = WF 3 "repack_two_vm" ww ("start " + ii.iname) 
            let saved_flaws = !vinfo.m_flaws
            vinfo.m_flaws := []
            let m = (" id=" + vlnvToStr ii.vlnv)
            let repack_two_xrtl arc = // RTL input to repack stage
                let mm = array_mapping
                let ww = WF 30 "repack_two_xrtl" ww "bar" 
                let ww = WF 30 "repack_two_xrtl" ww "drivegen done" 
                let keep_rtl =
                    match arc with
                        | _ -> true
                        //| other -> sf (sprintf "repack: other RTL %A" other)
                vprintln 0 (sprintf ": keep_rtl=%A for %s" keep_rtl (xrtlToStr arc))
                let ans = if keep_rtl then Some(rewrite_rtl ww vd X_true mm arc []) else None
                //if keep_rtl then vprint vd ("CR2 Rewritten arcs: " + sfold xrtlToStr (valOf ans) + "\n")
                ans

            let rec filter fpre = function // todo use List.filter? // or delete_nones
                | [] -> []
                | h::t -> let r = fpre h in if r=None then filter fpre t else (valOf r)@(filter fpre t)

            let rec kf dir arg =
                match arg with
                | SP_fsm(fsm_info, states) ->
                    let statetrim (si:vliw_arc_t) =
                        { si with
                            cmds = filter repack_two_xrtl si.cmds
                        }
                    SP_fsm(fsm_info, map statetrim states)
                | SP_l(ast_ctrl, cmd)-> SP_l(ast_ctrl, xbev_rewrite (WN "SP_l" ww) (array_mapping) cmd)
                | SP_rtl(ii, xin) -> 
                    let rr = delete_nones(map repack_two_xrtl xin)
                    vprintln 2 (sprintf "repack: SP_rtl: %i arcs became %i" (length xin) (length rr))
                    if not_nullp rr then gen_SP_lockstep (map (fun x->SP_rtl(ii, x)) rr)
                    else SP_comment ""
                | SP_comment s        -> arg
                | SP_seq lst          -> SP_seq (map (kf dir) lst)
                | SP_par(pstyle, lst) -> gen_SP_par pstyle (map (kf dir) lst)
                | SP_dic _            -> repack_two_dic ww kK2 m (vinfo, array_mapping) dir arg
                | other -> sf("repack_two SP other: " + hprSPSummaryToStr other)

            let execs' =
                let jx (H2BLK(dir, s)) = H2BLK(dir, kf dir s)
                map jx execs
            let ww' = WN "repack_two_inner_vm" ww
            let sons = map (repack_two_vm ww' kK2 (vinfo, array_mapping) None) sons
            let newnets =
                match newnets with
                    | Some lst when not_nullp lst ->
                        let cpi = { g_null_db_metainfo with kind= "repack-newnets" }
                        //let _ = reportx 3 "newnets" xToStr pcnets
                        [DB_group(cpi, map db_netwrap_null lst)]

                    | _ -> []
            let minfo' = { minfo with memdescs=[]; fatal_flaws= !vinfo.m_flaws @ minfo.fatal_flaws }
            vinfo.m_flaws := saved_flaws // Yuck!
            (ii, Some(HPR_VM2(minfo', decls @ newnets, sons, execs', assertions))) // end of repack_two_vm


//
//   
let cr_walker_gen ww (vinfo:repack_settings_t) aops = 
    let isoffset a = strlen(a) >= 7 && a.[0..6] = "$offset" // copy 1/2
    let isfield  a = if strlen(a) >= 8 && a.[0..7] = "$offset+" then Some(a.[8..]) else None // copy 1/2
    let vdp = true
    let vd = 3
    let unitfn arg =
        match arg with
            | X_pair((W_string(sx, sa, _)) as a1, r, _) when constantp r ->
                if isoffset sx || isfield sx <> None then ()
                else
                    let vale = xi_manifest64 "heapconst_mapping" r
                    match sa with
                        | XS_sc scl ->
                            let scl = List.foldBack (import_sc vinfo) scl []
                            ignore(vinfo.heapconsts.Add(arg, a1, r, scl, vale))
                        | _ -> 
                            vprintln 0 (sprintf "Old style +++ log heapconst ignored: os=" + xToStr arg)
                            ()
                ()

//          | W_string _ -> vprintln 0 (sprintf "+++ walk string arg " + xToStr arg)

            | W_asubsc(l, r, _) -> (ignore(arrayOpLogger ww vinfo aops CO_reader arg); ())
            | _ -> ()

    let strings_catch rhs arg = // Find string handle variables.
        if not g_shotgun_strings then ()
        else
        match arg with
            | X_bnet ff ->
                //let f2 = lookup_net2 ff.n
                let mdo = vinfo.groundfacts1.lookup ff.id
                let strings =
                    let rec mine_strings cc arg = // Find any strings that a variable ranges over ... hence determine whether a variable is a string handle.
                        match arg with
                        | W_string _ ->
                            //let _ = vprintln 3 (sprintf "strings_catch %A  %s" (x2nn arg) (xToStr arg))
                            singly_add arg cc
                        | W_query(_, l, r, _) -> mine_strings (mine_strings cc l) r
                        | X_pair(a, b, _) -> mine_strings cc b
                        | X_blift _ | X_bnum _ | W_node _ | X_num _ | X_bnet _ -> cc
                        | other ->
                            // There's no need to print this ... except when debugging a known to be missing string handles.
                            let _ = dev_println (sprintf "repack: mine_strings_catch other form %s   %s" (xkey other) (netToStr other))
                            cc
                    mine_strings [] rhs
                //vprintln 3 (sprintf "Assigned to string handle arg=%s strings=%s  md=%A" (netToStr arg) (sfold xToStr strings) mdo)
                match mdo with
                    | None ->
                        if not_nullp strings then dev_println (sprintf "repack: No memdesc for (potential?) stringhandle arg%s. rhs=%s  strings=%s" (netToStr arg) (netToStr rhs) (sfold xToStr strings))
                        ()
                    | Some md ->
                        //vprintln 3 (sprintf "Assigned to string handle %s strings=%s  md=d%i" (netToStr arg) (sfold xToStr strings) md.sc)
                        app (vinfo.string_literals.add md.f_sc) strings
                        ()
                        
            | _ ->
                dev_println (sprintf "unexpected strings_catch arg %s" (netToStr arg))
                ()

    let lfun strict clkinfo arg rhs =
        match arg with
            | W_asubsc(ll, subsc, _) ->
                let m = "repack lfun L866a"
                //let opindo = if fconstantp rhs && fconstantp r && not (is_string rhs) then Some(true, xi_manifest m r, xi_manifest m rhs) else Some(false, 0, 0)
                let iconstantp x = classed_constantp x = Constant_int
                let consto = if iconstantp rhs && not (is_string rhs) then CO_manifest(xi_manifest_int m rhs) else CO_varexp // What about float literals?
                let _ = strings_catch rhs ll
                (ignore(arrayOpLogger ww vinfo aops consto arg); ())

            | X_bnet ff when ff.width = -1 ->
                strings_catch rhs arg
                ()
            | _ -> ()

    let null_sonchange _ _ nn (a,b) = b
    let opfun arg N bo xo a b = a 
    let (_, SS) = new_walker vd vdp (true, opfun, (fun (_) -> ()), lfun, unitfn, null_sonchange, null_sonchange)
    SS


let as_plus offset = function
    | AS_leaf(v, None) -> AS_leaf(ix_plus offset v, None)
    | AS_leaf(v, Some cv) ->        
        let co = if constantp offset then Some(cv + xi_manifest64 "as_plus" offset) else None
        AS_leaf(ix_plus offset v, co)

    | other -> sf("as_plus other: " + assToStr other)




let lh_valOf1 m = function
    | AS_la(v, _)
    | AS_leaf(v, None)    -> sf " xi_manifest lh_valOf1 v "
    | AS_leaf(v, Some cv) -> cv
    | AS_lh(v, _) -> v
    | other -> sf(m + ": lh_valOf1 other: " + assToStr other)
 

(*
  We had two steps to this partition since there are two terms to the subscript.

  DefinitiveS: The rule in preparing equivalence classes is that assignments betwee the different brand/tagsn dxx->foo and dxx->bar do not conglomorate sets, but they can share a mapping function.  A mapping can be shared over a number of different scalar field array address busses and data array multiplexors.  For instance, dxx->foo and dxx->bar will both use the dxx regions and mapping in the repack stage.    DefinitiveS: The rule in preparing equivalence classes is that assignments betwee the different brand/tagsn dxx->foo and dxx->bar do not conglomorate sets, but dxx and dxx->foo will both use the dxx regions and mapping in the repack stage.

  The advantage of the old way is that partitions were nested. we keep that -
    DefinitiveS:   Interdependence between the terms of a subcript:  A naive cartesian product destroys too much information so we wish to preserve dependencies.

     When item2 is a scalar field array type+offset this presents no dependecies for item1. Item1 is never a field array.
     Otherwise, we'd like to support item1 and item2 being largely commutative, although we recognise that item1 is commonly a mapped encoding and item2 is a general arithmetic expression or constant. We can rely on a sorter, perhaps at the outset, 

  In a potential new monolithic treatment we no longer have, for instance, per-base data array partitioning decisions.

  We'd like to keep the nesting, but allow the same code to operate for both levels.  So the code has to be 'a -> 'a type preserving, then we can compose.  

   *)
// RP6a: First (zeroth) collate/partition is on region name or pointer/base address (i.e. item1).
// Return list of pairs (region * operation)


type sel_t =
    | SEL1 of array_subs_t
    | SEL2 of string



// Determine and report on which regions contain mutable data.
// We find a ROM or ROM region in an array when a location is assigned only one constant value: we are therefore ignoring behaviour where there is an initial read of a default value before the first write. TODO: perhaps flag up or fix, but currently this is not a used paradigm.

// Note: ROMs are rendered in the output of repack as straightforward X_bnet form vector with an initialisation list (not as structural components).  Restructure generates the structural instance and perhaps mirrors it as needed. System Integrator will also mirror ROMs or else share them with arbitration logic.

let make_romcheck ww its prec vinfo k0 k1 k2 r_scale ops =
    let ww = WF 3 "make_romcheck" ww (sprintf "k0=%A k1=%s k2=%s start considering whether a ROM for " k0 (pisToStr k1) (pisToStr k2) + its)
    let updates = new rominfo_updates_t("rominfo_updates")
    let hwm = ref -1L
    let check_same kv kd =
        let kv = kv / r_scale
        match updates.lookup kv with
            | None ->
                updates.add kv kd
                let _ = if kv > !hwm then hwm := kv
                true
            | Some ov -> ov = kd

    let m_writes = ref 0
    let check_write cc arg =
        match arg with
        | AS_vn(ser, writer, usages, item, offsit) ->
            //dev_println (sprintf "%s sofar=%A make_romcheck %s -> writer=%A " its cc (asToStr arg) writer)
            let cc = 
              match writer with
                | CO_unset  -> sf (sprintf "repack: writer mode should be set for S%i" ser)
                | CO_reader -> cc
                
                | CO_manifest vale when item.constf && offsit.constf ->
                    let a1 = as_manifest "romcheck-manifested1" item.subs
                    let a2 = as_manifest "romcheck-manifested2" offsit.subs
                    //let _ = vprintln 0 (its + sprintf ": rom_check clause manifest constf a1=%i a2=%i val=%A serial=S%i" a1 a2 vale ser)
                    let _ = mutinc m_writes 1
                    let aa = if a1 >= g_ssm_static_vector_base then a2 else a1 // A little of a Kludge for now to remove the base offset.
                    cc && check_same aa vale
                    
                | CO_varexp
                | CO_manifest _ ->
                    false
            cc
              
        | other ->
            let _ = dev_println (sprintf "romcheck: check_write: other form ignored: " + asToStr other)
            cc

    // We record the number of writes - if it is zero we treat it as an uninitialised RAM, which is more helpful when parts of the user's app are missing during development.
    let written_once = List.fold check_write true ops
    // Must form this conjunction after the above fold.
    let written_once = !m_writes > 0 && written_once
    vprintln 3 (sprintf "romcheck k0=%A k1=%A k2=%A r_scale=%i hwm=%i no_of_writes=%i  romcheck ans=%A" k0 (pisToStr k1) (pisToStr k2) r_scale !hwm !m_writes written_once)
    let isrom = written_once
    let _ = WF 3 "make_romcheck" ww (sprintf "check for %s %s is %A" its k0 isrom)
    if written_once then
        let scalar_contento =
            let m_lst = ref []
            let rom_setup1 kk vv =
                let item = gec_XC_bnum(prec, vv)
                mutadd m_lst item
            let _ = for z in updates do rom_setup1 z.Key z.Value done
            match !m_lst with
                | [item] ->
                    vprintln 3 (sprintf "  Potential ROM %s has one value in it: %s" its (romrenderx [item]))
                    Some item
                | _ -> None
        Some(!hwm, scalar_contento, updates)
    else None

// RP6b: (second on item2 old way)
// arithmetic partition on offset 
let arithmetic_partition_f ww vd msg lst1 = 
    let region_or_id = "" // no longer present here
    let its1 = msg + " " + region_or_id
    vprintln 3 (sprintf "doing second_partition_f %A |lst1|=%i" region_or_id (length lst1))
    let intvales = new ListStore<int64, sel_t * subs_term_t>("intvales")// List of expressions that eval to the same constant subscript.
    let gasToStr(l, r) = sprintf " { %s %s } " ("selToStr l") (assToStr r.subs)
    let gas_listToStr lst = "LST:" + sfold gasToStr lst
    let make_2nd_partition (grd, partition_item) sets_sofar =
        if partition_item.constf then
            let e' = 
                match partition_item.subs with
                    | AS_leaf(e, _)         -> e
                    | AS_la(e', AS_lh(l,h)) -> e'
                    | AS_fa_2(_, _, e')     -> e' // Put field arrays through the const grinder - does no harm.
                    | other -> sf (sprintf "other form partition_itema %A" partition_item)
            let v = (xi_manifest64 "make_2nd_partition-2" e') // + (as_manifest "make_2nd_partition-2" item')
            //let _ = vprintln 3 (sprintf "   %s record constant integer subscript (intval) %A" (grd_name grd) v)
            let _ = intvales.add v (grd, partition_item)
            sets_sofar
        else
            let overlap_vidx lst20 =
                match partition_item.subs with
                    | AS_la(e', AS_lh(l, h)) ->
                          let rec overlap_vidx_lh = function // copy 1/2
                              | [] -> false
                              | (grd_, itemx)::tt ->
                                match itemx.subs with
                                  | AS_la(_, AS_lh(l1, h1)) ->
                                              let y = not(h1 < l || h < l1)
                                              //vprintln 0 ("Vobug " + asToStr arg + " cf " + asToStr(hd lst) + "  yield=" + boolToStr y)
                                              y || overlap_vidx_lh tt
                                  | other -> sf ("overlap_vidx_lh other L1686: "  + assToStr other)
                          overlap_vidx_lh lst20
                    | AS_leaf(e, cv) ->
                          let rec overlap_vidx_fa lst = // copy 1/2
                              match lst with
                                  | [] -> false
                                  | (grd_, itemx)::tt ->
                                      match itemx.subs with
                                          // match resolved_region_size with
                                          | AS_leaf(ex, cvx) ->
                                              if not (nonep cv) && not (nonep cvx) then sf(sprintf "fault - compared two constants in varidx clause %s cf %s" (stToStr itemx) (assToStr partition_item.subs))
                                              else true // Conservative - return that they overlap since we cannot discriminate at compile time.
                                          | _ -> sf (sprintf "overlap_vidx_fa other compare: %s cf %s" (stToStr itemx) (assToStr partition_item.subs))
                          overlap_vidx_fa lst20
                    | other when nullp sets_sofar -> false
                    | other ->
                        vprintln 0 (msg + sprintf ": +++ TODO should check regions sets_sofar logged. other form arithmetic partition_itemb %s\n sets_sofar=%A" (stToStr partition_item) (sfold gas_listToStr sets_sofar))
                        true // Conservative - can check sc vns...
                    //| other ->
                    //sf (sprintf "other form partition_item %A sets_sofar=%s" (assToStr partition_item) (sfold gas_listToStr sets_sofar))
            let overlaps = List.filter overlap_vidx sets_sofar
            //let _ = vprintln 0 (sprintf "Blib v overlaps %s ar e%i" (assToStr partition_item) (length overlaps))
            match overlaps with
                | [] -> singly_add ([(grd, partition_item)]) sets_sofar
                | [solo] ->
                    let a = singly_add (grd, partition_item) solo 
                    (a) :: list_subtract(sets_sofar, overlaps) // robust subtract.
                | multiple_ -> muddy("varidx overlaps more than one set: need to merge them: " + stToStr partition_item)
    let varidx_classes = List.foldBack make_2nd_partition lst1 []
    let add_constants sets (n, lst2) = // Look at each constant in turn and pop it in its relevant varidx partition or make a fresh partition for that constant.
        let overlap_cidx lst4 = // copy 2/2 
            let rec olc = function
              | [] -> false
              | (grd_, subsx)::tt ->
                  match subsx.subs with
                    | AS_fa_2(orig_l, tagname, offsit) -> xi_manifest64 "L1737 fa2" offsit  = n || olc tt // TODO not useful
                    | AS_leaf(ee, Some vv) -> vv = n || olc tt
                    | AS_leaf(ee, None)    -> true // Since we have already disambiguated on storage class we conservatively return true for all indecies not known until compile time.
                    | AS_la(_, AS_lh(l1, h1)) ->
                        let y = (n >= l1 && n <= h1)
                        y || olc tt
                    | AS_rec _ -> // overly conservative - TODO
                        vprintln 0 (sprintf "+++ overlap rec conservative - can we compare sc ? n=%i L1737:  %s\nGRD=%A"  n (stToStr subsx) grd_)
                        true//Conservative
                    | _ -> sf (sprintf "overlap other n=%i L1737:  %s\nGRD=%A"  n (stToStr subsx) grd_)
            olc lst4
        let overlaps = List.filter overlap_cidx sets
        match overlaps with
              | [lst_solo] ->
                  let rec patchit sofar = function
                      | [] -> sf "overlap lost"
                      | (existing_lst)::tt when lst_solo = existing_lst ->
                          let nn = lst_union lst2 existing_lst
                          (rev sofar) @ nn :: tt
                      | arg::tt -> patchit (arg::sofar) tt
                  let rems = patchit [] sets
                  rems

              | [] -> lst2 :: sets
              | many_ ->
                  reportx 0 ("+++ non disjoint: " + i2s64 n) (fun x -> sfold (fun (grd_, y) -> "\n    ::" + stToStr y) x) overlaps
                  sf ("constidx overlaps two varidx classes: they could not have been disjoint for constant k=" + i2s64 n)
    let ctr = ref 0
    let subclasses =
          let rat = ref []
          let _ = for z in intvales do (mutinc ctr 1; mutadd rat (z.Key, z.Value)) done
          if !ctr = 0 then varidx_classes
          else
              let _ = vprintln 3 (its1 + sprintf "%i constidx operations to be matched with %i varidx classes: " !ctr (length varidx_classes) + sfold (fun l -> i2s(length l)) varidx_classes)
              //let _ = reportx 3 (its1 + " varidx classes") (fun (g, s) -> sfold asToStr s) varidx_classes
              List.fold add_constants varidx_classes (!rat)
    let _ = vprintln 3 (its1 + sprintf ": Final number of subclasses, after adding %i constidx operations to %i varidx subclasses is %i" !ctr (length varidx_classes) (length subclasses))
    subclasses // end of arithmetic_partition_f



let get_fst_ans ww vinfo its ll (xformed_subs_ops, repacked_regions, ll_wlst, old_memdesc_ats) = // The real first partition?
    let vd = vinfo.k2.loglevel
    let flaw_log ss =
        vprintln 0 (sprintf "Fatal flaw: %s" ss)
        mutadd vinfo.m_flaws ss
        
    let goon2 = false
    //vprintln 0 (sprintf "repack.get_fst_ans: %s  old_memdesc_ats=%A" its old_memdesc_ats)
    let sitemsg = sprintf "get_fst_ans+%s+%s" its (if goon2 then "item2" else "item1")
    let _ = WF 3 "get_fst_ans" ww (sprintf "CS1.1 start - ll=%s" its)
    reportx 3 ("xform'd array ops for " + " " + its) asToStr xformed_subs_ops
              (*Add each subscription  to list of sets, conglomorating the sets when we find overlap. 
               *For constant index members, make sure they are not split by normalising through intvales.
               *)
    vprintln 3 (sprintf "get_fst_ans: ll_wlst for %s is " its + sfoldcr aidToStr ll_wlst)

    
// The issue is that mappings for multiplexors are not necessarily just for this current ll 

// TODO please explain - we want, I think, to have a flat list of regions for this ll, but then allocate them into classes based on wlst entries.  We are deleting the regions themselves from the wlst and collecting them separately post lock of the classes_store.
    let classed_regions00 =
        let get_numeric_regions2 cc arg =
            let yielder key lsf =
                    let rpp = (repacked_regions:repacked_regions_t).lookup key 
                    let regions =
                        let rx dd = function
                            | Memdesc0 md when isIndexable (int64 md.uid.baser) ->
                                let b = int64  md.uid.baser 
                                let lo = md.uid.length
                                match lo with
                                    | None -> dd
                                    | Some lo ->
                                        let region = (b, int64 lo)
                                        vprintln 3 (sprintf "get_numeric_regions2 k1=sc%i, b=%A lo=%A" key b lo)
                                        singly_add (aidToStr (to_ndsc3a_f_sc md), region, md.mats) dd
                            | other -> dd 
                        List.fold rx [] rpp
                    (arg, key, lsf, regions, None)

            match arg with
                | A_metainfo mci ->
                    let cls = demeta vinfo mci
                    if not_nonep cls.bank then
                        vprintln 2 (sprintf "Ignore regions for offchip bank %s" (valOf cls.bank))
                        (arg, cls.key, cls.lsf, [], cls.bank)::cc
                    else (yielder cls.key cls.lsf)::cc
                | A_loaf scs -> muddy "need cleaner A_loaf scanf decode scs again!"  

                | other ->
                    vprintln 0 (sprintf "+++ Ignore L1487 (we expect one SC_classed to be present at least) others=" + aidToStr other)
                    cc
        List.fold get_numeric_regions2 [] ll_wlst
    //vprintln 0 (sprintf "get_fst_ans: got point 2 %A" classed_regions00)

// For simplicity we conglomorate into one region class any region classes that overlap, then we can make a mapping/packing for each region class.
// wlst has  sets of sets of regions. Need to place in region classes and then allocate subs_ops to each region class.


(*
    let regionClassStore = new ClassStore<int64, string * region_t>("first partition")
    let run_p0 region_sets_in =
        let prep (clsKey, regions) = map (fun (ke, region) -> (fst region, (ke, region))) regions  // Overlap? Here we have reverted to just identifying on common start address which is sufficient for now.  We are using the classStore really just as a means to eliminate duplicates. 
        // But we may also, unlikely, get further conglomoration of storage classes - treat as an error for now.
        let _ = regionClassStore.addItems ("CIX" funique "CI" + "X")) (prep region_sets_in)
        ()
    let _ = app run_p0_ classed_regions00

    let _ =
        let c0 = regionClassStore.readout()//readout just for logging
        reportx 3 ("regionClassStore for " + its) (fun (cls, x) -> "ignore_cls=" + cls + "->>" + sfold (fun (baser, (newBaseName, region)) -> newBaseName + " : " + regionToStr region) x) c0
*)

// ... are the regions or partitioning of them ever likely to change ?  No, so the mapping making should be done without reference to an ll.  The regions need names that can get appened to the 

// do we ever have a mapping that itself is partitioned ... yes, this can happen - two copies of the same program in parallel with no intercommunication will always gen this.
    let classed_regions01 =
        let bungy (classed, wx_, lsf_, regions1, banko) = // Bypass/remove the fancy code
            //let _ = dev_println (sprintf "bungy %A" classed)
            let pis = P_scalarise(length regions1)
            let k3 = FWN(funique "bx")
            (classed, k3, pis, regions1, banko)
        map bungy classed_regions00

 (*     let rec add_cls_pis_and_newBaseName sofar = function
            | [] -> []
            | (clsKey_, regions1) :: tt ->
                let lots = list_once(map (fun (_, (baser, _)) -> regionClassStore.classOf baser) regions1)
                let _ = vprintln 3 (sprintf "clsKey_=%s toslots region item lots=%A" clsKey_ lots)
                let tailer sofar = add_cls_pis_and_newBaseName sofar tt
                match lots  with
                    | []     ->
                        let _ = vprintln 3  "could just ignore and move on 1"
                        tailer sofar
                    | None :: _ ->
                        let _ = vprintln 3 "could just ignore and move on 2"
                        tailer sofar
                    | [Some temp_cls_name] ->                        
                        if memberp temp_cls_name sofar then tailer sofar
                        else
                            let regions2 = map snd (regionClassStore.members temp_cls_name)
                            let arity = length regions2
                            let destiny_piz1 = P_scalarise arity
                            // Note currently we do not make P_arithmetic for on first paritition at the moment. TODO
                            let no = length sofar // neat?
                            let newBaseName = funique ((if arity < 2 then "S" else "B") + sprintf "%iN" no)
                            let _ = vprintln 3 (sprintf "For temp_cls_name %s we created newBaseName %s" temp_cls_name newBaseName)
                            // TODO use a different style basename when scalarised
                            // We will not need local basenames when mapped to DA but the basenames are used for the addressees?
                            let x = (clsKey_, newBaseName, destiny_piz1, regions2)
                            x :: tailer (temp_cls_name::sofar) 
                    | multiple -> sf (sprintf "clsKey_=%s cross region readout " clsKey_ + sfold (stringOf) multiple)
        //let _ = vprintln 0 (sprintf " SI/MI fst_ans ll=%s clsKey=%s(wont 2) of these test38 poppa %i  destiny=%A" its clsKey destiny_piz1)
        //(clsKey, map add_newBaseName2 (zipWithIndex birs))
        //let _ = reportx 3 (its + " classed region class for clsKey=" + clsKey) (fun (cls, x) -> cls + "->>" + sfold (fun (baser, (newBaseName, region)) -> newBaseName + " : " + regionToStr region) x) c0
        add_cls_pis_and_newBaseName [] classed_regions00
*)
    let _:(aid_t * final_work_name_t * pis_t * (string * region_t * att_att_t list) list * sri_digest_t option) list = classed_regions01
    reportx 3 (its + " initial classed_regions") (fun (classed, k3, pis, regions, banko) -> sprintf "final_clsKey=%s  bname=%s bank=%s pis=%s, n_regions=%i" (aidToStr classed) (dewn k3) (valOf_or banko "--o--") (pisToStr pis) (length regions)) classed_regions01


(* first_partition: deals with region handles.  Regions are either arrays or records.
  Each may contain primitive types or handles on further regions or arrays.  Arrays of structs will be considered in another revision.
  They naturally partition on this division and their content type.

 Beyond
 this they partition further into classes between which no handle is
 exchanged.  For instance, with three arrays of integers, one of them
 might have a fixed, once-and-for-all handle, while the other two are
 pointed at alternately by various handles with values copied between
 these handles.  There would then be two equivalence classes of
 integer arrays, one with one entry and the other with two.

 The once-and-for-all handles disappear.  The classes with multiple
 handles in them are replaced with field arrays for each scalar entry in a record
 and with multiplexor trees over other arrays for regions that are arrays.  These may
 be further mapped down to flip-flops or up to DRAM or BRAM in subsequent recipe stages.


 The subscription operations can also be partitioned on the same basis on item1 (item2 may lead to further partitioning).
 Any item1 single item1 expression cannot range over scs in different sc classes since this would combine them.
*)

    // Make mappings for these regions or otherwise reform their destiny 
    // These regions commonly belong to a different ll
    let (classed_regions02, mapping_lst, newnet_rewrites) =
        let gen_map_wrapper (A_metainfo mfi, k3, pis, regions0, banko) cc =
            //dev_println (sprintf "kat k3=%A" k3)
            let _:final_work_name_t = k3
            let cls = demeta vinfo mfi
            if pis = P_scalarise(0) then
                vprintln 2 (sprintf "Discard P_scalarise(0) entry for %A" k3) // Should not be made in the first place!
                cc
            else
            let (pis', (k33:final_work_name_t), newnet_rewrites, ats) = 
                let ats = List.fold (fun cc (_, r, ats) -> lst_union ats cc) [] regions0
                match pis with
                    | P_scalarise(0) when cls.lsf && false ->
                        muddy(sprintf  "shzzz literalstring sc%i and k3=%A" cls.key k3)
                        //(pis', workname, newnet_rewrites)
                    | P_scalarise(1) ->
                        vprintln 3 (its + sprintf ": CR02: Replacing scalarise ONE, clsKey=sc%i with a mapping for %s ats=%A" cls.key (dewn k3) ats)
                        let fwn = k3
                        (pis, fwn, [], ats)
                    | P_scalarise(n) ->
                        let warner =
                            if not_nonep banko then "DYNAMIC-ALLOC-OFFCHIP"
                            elif nullp regions0 then "YIKES-NOREGIONS" else ""
                        vprintln 3 (its + sprintf ": CR02: Replacing scalarise %i, clsKey=sc%i with a mapping for %s. %s." n (cls.key) (dewn k3) warner)
                        let regions = map (fun (_, r, ats) -> r) regions0
                        let (work_name, mapping, len) = seed_enumap vinfo sitemsg cls.has_nullf [A_metainfo mfi] (rev regions) (* try to keep ascending order with a rev *)
                        let rnames = map (fun n -> sprintf "%sNoCE%i" (dewn work_name) n) [0..len-1] 

                        if cls.has_nullf then vprintln 2 (sprintf "Notie: rname %s is not supposed to be used - it is addressed via the null pointer." (hd rnames))
                        let not_to_be_used = if cls.has_nullf then hd rnames else ""
                        vprintln 3 (sprintf "rnames= " + sfolds rnames)
                        let mapping' = map (fun (rname, (no, reg)) -> (no, (rname, rname=not_to_be_used, reg))) (List.zip rnames mapping) // 

                        let classes_to_change = vinfo.sclasses_store.members -1 cls.key

                        let vars_to_change =
                            let pos cc (_, aid_lst) = lst_union cc (map snd (vinfo.memdesc_idx1.lookup aid_lst))
                            let dememdesc = function
                                | Memdesc0 md -> (htos_net (md.uid.f_name), md.uid.length)
                                | _ -> sf "not memdesc0"
                            map dememdesc (List.fold pos [] classes_to_change)
                        //close_enum ("repack fo_lookup workName=" + workName) vvale
                        let w = bound_log2 (BigInteger(length mapping - 1))
                        vprintln 3 (sprintf "We need to change types of storage classes to %i enum %s" w (sfold (fun (_, aid)->aidToStr aid) classes_to_change))
                        vprintln 3 (sprintf "We need to change var types to w=%i enum %A" w vars_to_change)
                        // We need a new name owing to a new type.
                        let newnet_rewrites =
                            let gen_rew cc (x, len___) =
                                //dev_println ("making newnet rewrite for " + x)
                                let (found, ov) = g_netbase_ss.TryGetValue x // This is a poor interface to netbase!
                                if found then
                                    let ss = x + nxt_ctoken()
                                    vprintln 2 (sprintf "Scalarise(%i): Create new vectornet %s w=%i"  n ss w)
                                    (X_bnet ov, vectornet_w(ss, w)) :: cc
                                else
                                    vprintln 3 (sprintf "cannot recode variable since not set up: %s" x)
                                    cc
                            List.fold gen_rew [] vars_to_change
                        let newnet_rewrites = [] // Kill off for now
                        (P_mapped(len, mapping', banko), work_name, newnet_rewrites, ats)
            ((save_classed "L1888" vinfo cls, k33, pis', regions0), (k33, map f1o3 regions0, pis'), newnet_rewrites) :: cc
        List.unzip3(List.foldBack gen_map_wrapper classed_regions01 [])

    // "get_fst_ans"
    let _ = 
        let report4fun (A_metainfo mfi, k3, pis, regions) =
            let cls = demeta vinfo mfi
            sprintf "cls.key=sc%i bank=%A literalstring=%A bname=%s pis=%s, n_regions=%i" cls.key cls.bank cls.lsf (dewn k3) (pisToStr pis) (length regions)
        reportx 3 ("final classed_regions for " + its) report4fun classed_regions02        


    let bad_lin_lookup vn = function
        | A_metainfo mci ->
            let cls = demeta vinfo mci
            match List.filter (fun x -> (muddy "get clskey f1o4 x") = cls.key) classed_regions02 with
                | [] -> flaw_log ("no mapping for " + asToStr vn + sprintf " under desc=sc%i" cls.key) 
                | [(_, k3, pis, regions)] -> ()
                | _ -> sf "multiple entries"
        | _ -> sf "L1639"
        

    let is_desc_string = function
        | A_metainfo mci ->
            let cls = demeta vinfo mci
            cls.lsf
        | other ->
            dev_println ("Unclassed description at late stage " + aidToStr other)
            false

    // "get_fst_ans"
    let allocate_ops_to_each_class_partition xformed_lst = // Collates on k1. Currently a nop, but partition depth type increases by one - we are ignoring the sc info TODO. Or is this another attempt at zeroth and it is done already under classStore ...  
        let crm = "RP6a"
        let rec regroom ops = function
            | [] ->
                //dev_println ("null regroom clause")
                let ops1_ =
                    let non_string_lit_pred = function
                        | AS_vn(ser, writer_, usages, item1, item2) -> not (disjunctionate is_desc_string (item1.descs @ item2.descs))
                        | _ -> true
                        
                    List.filter non_string_lit_pred ops
                vprintln 3 "completely deleting a rewrite for a stringliteral subscription op (one site deletes all others)"
                //dev_println (sprintf "non_string_lit_pred %i  %i" (length ops) (length ops1_))
                if not_nullp ops then
                    let msg = (sprintf "+++ repack: %s (item2-select-goon2=%A) %i array ops were left over: for %s being: %s" crm goon2 (length ops) its (sfoldcr asToStr ops))
                    if vinfo.k2.ignore_leftover_error then hpr_yikes msg else sf msg
                []
            | (A_metainfo mci, (k3:final_work_name_t), pis1, new_lhs_basename)::tt ->
                //dev_println (sprintf "mci clause its=%s" its)
                let cls = demeta vinfo mci
                let xlist_intersection_pred lst needle =
                    let ans = memberp needle lst
                    //if its = "@16_SS" then dev_println (sprintf " xlist_intersection_pred  goon2=%A  int_lst=%s    <cf with>   %i      ans=%A" goon2 (sfold i2s lst) needle ans)
                    ans
                let extract_cls_key = function
                    | A_metainfo mci ->
                        let cls = demeta vinfo mci
                        cls.key
                    | other ->sf (sprintf "repack: L1896 other %s" (aidToStr other))
                let regroom1 (ins, outs) arg =
                    match arg with
                        | AS_vn(ser, writer_, usages, item1, item2) ->
                            let descs = (if goon2 then item2 else item1).descs
                            //dev_println (sprintf "goon2=%A  item1=%A" goon2 item1.descs)
                            if xlist_intersection_pred (map extract_cls_key descs) cls.key then (arg::ins, outs) else (ins, arg::outs)
                        | _ ->  (ins, arg::outs)
                let (ops1, leftover) = List.fold regroom1 ([], []) ops
                (k3, cls, pis1, ops1) :: (regroom leftover tt)
            | other -> sf (sprintf "repack: regroom: other spioshon form %A" other)
        let _ = WF 3  crm ww ("RP6a: Start collate memdesc and region info for " + its)
        let ans = regroom xformed_lst classed_regions02
        let _ = WF 3  crm ww ("RP6a: Finished collate memdesc and region info for " + its)
        reportx 3 ("RP6a: allocate_ops_to_each_class_partition for " + its)  (fun (regionClassId, lsf, pis, ops) -> dewn regionClassId + sprintf "  -=%s-> " (pisToStr pis) + sfoldcr asToStr ops) ans // We print pis here - but it does get changed...
        ans


    let regions_partitioned = allocate_ops_to_each_class_partition xformed_subs_ops
    let _ = WF 3 "get_fst_ans " ww (sprintf "CS1.9 finished - ll=%s" its)

    // Make idep_ctrl registrations arising from allocate_ops_to_each_class_partition
    let serials_lst = 
        let gen_idep_ctrl (work_name, lsf, pis_, asl) =
            let xq cc = function
                | AS_vn(_, writer_, _, item1, item2) -> singly_add (if goon2 then item2 else item1).serno cc
            let serials = List.fold xq [] asl
            //vprintln 0 ("idep_ctrl: L1661 serials=" + sfolds serials)
            (work_name, serials)
        map gen_idep_ctrl regions_partitioned

    let recombined =
        let recom (keyx, _, pis)=
            //vprintln 0 (sprintf " kaax rec sc%i %s" keyx k2_) // like k2
            (keyx, pis, valOf_or_fail ("norecombine on " + dewn keyx) (op_assoc keyx serials_lst))
        map recom mapping_lst

// Region names:
//   If a DA is partitioned we need an rname for each partition. If the partition size is one element we say that partition is scalarised.  If all subscripts are constants, all used locations become scalars and the reset dissappears.
//   Where we have MI and the number is not more than than max_mux_arity we will need an rname 
//   for each 

    let save_idep_ctrl (clsKey, pis, serials) =
        let savit serno =
            //vprintln 0 (sprintf "destiny MI/SI idep_ctrl: ideps.add L1669 serno=%s; clskey=sc%i; " serno clsKey + sprintf "destiny=%A" pis)
            vinfo.ideps.add serno (clsKey, pis)
            ()
        app savit serials
        ()
    app save_idep_ctrl recombined
    let _ = WF 3 "get_fst_ans " ww (sprintf "finished for ll=%s" its)
    (classed_regions02, regions_partitioned, newnet_rewrites) // end of get_fst_ans





let manifestise arg = (as_manifest "manifestisef-3i" arg.subs) 
    

////////////////////////////////////
//
// Rename: scale_or_map scale or map ?

// DefinitiveS: Subscript expressions are sums of terms.  We can either perform scaling or mapping on a term. One or other should be applied to every term.  Mapping reduces sparseness and is the preferred solution but it cannot be applied when the sc uses unknown functions or complex arithmetic.  Mapping is essential for the heap object basses generated by Kiwi which are in a very sparse space.  Scaling performs linear arithmetic on a term and all terms in a given expression that are scaled must use the same multiplier but they can use different offsets.  Scaling subtracts the lowest value from a term and divides by the HCF of the resulting unmapped terms.  The kcode scan in Kiwi reports which storage classes contain foreign functions or arithmetic and is used to select which approach.  

// Should the same mapping be used for all heap objects in a class, even if some have a clearly different access patern to others?  Did you create a pathological example for this ?  e.g. a common var is assigned upcasts from two child classes but that var is never used.
    
// Perhaps: If an sc only has constants it can be removed during repack since each will be in its own class.  Variables can be present too, provided they are not used as subscripts. If there are variables used as subscripts then we assume constants are propagated freely between the variables and the class must remain as one.   If the resulting space is sparse clearly we should use a mapping, if it is dense we can still use a mapping.  Heap constants are typically sparse and irregular and need to be mapped.  Also, address arithmetic is not done on them.  An obvious distinction is whether used as a data array offset but we seek general code.  An sc that has function domain and range classes in it cannot be mapped, since we do not have access to the function.  Everything is considered a function except multiplexors and addition and multiplication by a constant.  So do we need richer static analysis ?  Indeed if we have addition or multiplication thats enough to push the sc to arithmetic.  The only thing that can be mapped is multiplex and assign. If that's all there is we map.  So how do we find out ? more processing in kcode scan to return an ARX arithmetic dontknow value.

let scalez ww its msg itemsize_bytes resolved_region_size_f members =
    let (lower, oldlen) =
        let ranges_and_classes = map (dynrange_get its msg) members
        reportx 3 "scalez Classes and Ranges" (fun (r, aids) -> sprintf "  %s   %s" (sfold aidToStr aids) (assToStr r)) ranges_and_classes

        let resolve_minmax msg l r = 
            match (l, r) with
                | (AS_lh(l, h), AS_lh(l', h')) -> AS_base_len_pair(min l l', max h h' - min l l')
                | (AS_lh(l, h), AS_base_len_pair(b', len')) -> AS_base_len_pair(min l b', max h (b'+len') - min l b')
                | (AS_none, r) -> r // Give complete priority over AS_none.
                | (l, AS_none) -> l
                | (AS_base_len_pair(b, l), AS_base_len_pair(b', l')) -> AS_base_len_pair(min b b', max (b+l) (b'+l') - min b b')
                | (l, r) -> sf (sprintf "resolve_minmax via %s, other form %A cf %A" msg l r)

        let rec not_none cc (range, classer) =
            match range with
            | AS_leaf(_, Some v) -> resolve_minmax "via AS_leaf" (AS_base_len_pair(v, int64 itemsize_bytes)) cc
            | AS_leaf(_, None)
            | AS_none ->
                match resolved_region_size_f classer with
                    | None -> sf (its + sprintf ": At least one item with no range and no externally-resolved size from ranges^%i=%s\nmembers=%i %s" (length ranges_and_classes) (sfoldcr (fun (a,b) -> assToStr a + sprintf "%A" b) ranges_and_classes) (length members) (sfold (fun (sel, st) -> stToStr st) members))
                    | Some region_size ->
                        vprintln 3 (sprintf "via resolved_region size from classer=%s as %i" (sfold aidToStr classer) region_size)
                        resolve_minmax "via resolved_region_size" (AS_base_len_pair(0L, region_size)) cc
            | AS_lh(l, h) -> resolve_minmax "via AS_lh" (AS_lh(l, h)) cc
            | other -> sf (sprintf "not none other %A" other)
        match List.fold not_none AS_none ranges_and_classes with
            | AS_base_len_pair(baser, lengther) -> (baser, lengther)
            | other -> sf (sprintf "lower/oldlen other %A" other)
    (lower, oldlen)



let real_scale_class ww vinfo (ll, msg) resolved_region_size_f (members, number) (cc:(string * pis_t * (sel_t * subs_term_t) list) list) =
    let vd = 3
    let ww = WN "scale_class" ww
    let (itemsize_bits, prec) =
        let prec = mine_prec g_bounda ll
        (valOf_or_fail "real_scale_width" prec.widtho, prec)
    let itemsize_bytes = (itemsize_bits + 7) / 8
    let its = xToStr ll
    let clsKey =
        //if oneclass then (cassert(number=0, "not zero when only one class"); "SOL")
        "AR" + nextkey vinfo msg + i2s number
    let name_wh = msg + ":" + clsKey
    let allconst = conjunctionate (fun (grd_, x)->x.constf) members
    let mstats = map (snd>>fa_pred) members
    if disjunctionate id mstats then // Field arrays offsets are constants that are dropped and so do not need scaling.
         //We test for disjunction, but conjunction should also hold - i.e. they are all field array offsets.
        if not (conjunctionate id mstats) then dev_println (sprintf "Yikes not all field arrays after all in %s \nmembers=^%i %A"  (sfoldcr (snd>>stToStr) members) (length members) 0)
        vprintln 3 (sprintf " scale_class: no scaling needed for field index (pass over) %s %A" (sfold (snd>>stToStr) members) mstats)
        //let scalarise = true // field tags are scalars in the field-type dimension.
        (funique "noNetNameRequired", P_scalarise(1), members) :: cc
    else

    let (lower, oldlen) = scalez ww its msg itemsize_bytes resolved_region_size_f members // for roms only

    let each_as_own_class items =
        let items' = map (fun (grd, sp) -> ((grd, sp), manifestise sp)) items
        //reportx 0 "Each_as_own class (eaoc) bigprint" (fun x -> sprintf ">  %A" x) items
        let vales = List.fold (fun c ((grd, sp), ix) -> singly_add ix c) [] items'
        let arity = length vales
        vprintln 3 (sprintf "eaoc: no of distinct numeric items=%i  itemsize=%i lower=%i " arity itemsize_bytes lower)
        // For sparse ROM scalarising we need to also store information about the subscript value so the ROM entry can be selected later.
        let rec k2 p (m0:Map<int64, int>) = function // Enumerate the distinct items in this mapping.
            | [] -> m0
            | h::t ->
                let m' = m0.Add(h, p)
                k2 (p+1) m' t
        let themap =
            let mapping = k2 0 (Map.empty) vales
            let mapgen ff ix no = (clsKey + "_" + i2s no, P_scalarise(arity), [ff]) // TODO check arity or 1 here...
            let serget cc (_, y)  = singly_add y.serno cc
            let idep_ctrl (repack_key, pis, pops) =
                let serials = List.fold serget [] pops
                let pis' = if arity=1 then P_scalarise(1) else P_mapped2 vales 
                //vprintln 0 (sprintf "ideps.add L2045 serno=%A; clskey=%s; " serials clsKey + sprintf "destiny=%A" pis')
                app (fun serial -> vinfo.ideps.add serial (FWN repack_key, pis'(*themap*))) serials
                (repack_key, pis', pops)//vprintln 0 ("idep_ctrl L1838: eaoc: repack_key=" + repack_key + "   serials=" + sfolds serials)
            let themap0 = map (fun ((grd, sp), ix) -> mapgen (grd, sp) ix (valOf_or_fail "map" (mapping.TryFind ix))) items'
            map idep_ctrl themap0
        themap
        //reportx 3 (sprintf "operations for %s " name_wh) asToStr members
    let eaoc = allconst && vcscal_heuristic_pred vinfo msg (length members) // TODO this heuristic should be applied post item product.
    vprintln 3 ("Make scalarise decision for " + name_wh + sprintf ", allconst=%A,  member count=%i. eaoc=%A" allconst (length members) eaoc)
    if eaoc then (each_as_own_class members) @ cc
    //elif vvscal_heuristic_pred ll (length members) then (eachasownclass members) @ cc   // This needs generate mux's - done in logic_assoc_exp. Yes.
    else
        //vprintln 0 (sprintf "scale_class: members^%i=%A"  (length members) (sfoldcr (snd>>stToStr) members))
        if nullp members then sf "stop now - no members"
        // Want to decide on arith dynrange or mapping
        let (lower, oldlen) = scalez ww its msg itemsize_bytes resolved_region_size_f members
        let offset = lower
        let lowerx = xi_num64 lower
        let (found, mdo) = vinfo.memdesc_map.TryGetValue lower
        let mdo = if found then Some mdo else None
        vprintln 3 ("Not all subs const (eaoc) " + name_wh + sprintf " is NO. Instead, so far: offset=%i" offset + " md=" + (if mdo=None then "None" else mdpToStr (valOf mdo)))
        let normalise_lower_bound (grd, arg) =
            let reducer arg = // Subtract lowest base from all members.
                match arg with
                | AS_la(e', AS_lh(l,h)) -> AS_la(ix_plus lowerx e', gec_AS_lh(l-lower, h-lower)) // move to as_plus
                | AS_leaf(e', co)       -> as_plus lowerx arg

                | AS_rec(ser, v, i) ->
                    // The values in the array containing indecies will themselves have been scaled, so we need do nothing here. We can just return arg.
                    arg
                | AS_fa_2 _ ->
                    sf (sprintf "repack: as_reducer: its=%s: Could not reduce %s by %s" its (assToStr arg) (xToStr lowerx))
                    arg


                | other ->
                    sf(sprintf "repack: as_reducer: its=%s: Could not reduce: other form %A" its other)
                    other
            (grd, { arg with subs=reducer arg.subs; })
        let members1 = map normalise_lower_bound members
        vprintln 3 ("tmp:Starting factor find/scale " + name_wh)
        // The zero value subscript has every factor and must be discarded at every opportunity.
        let hcf =
            if length members <= 1 then 0L // won't need the hcf if there is only one member.
            else 
                let rec factor_finder cc ex =
                    //lprintln 100 (fun()->"Factor_find1 " + xToStr ex + " const=" + boolToStr(constantp ex))
                    if constantp ex then
                        let k = xi_manifest64 "ff-2" ex 
                        if k = 0L then cc else k::cc 
                    else
                    match ex with
                        | W_query(g, l, r, _) -> factor_finder (factor_finder cc l) r
                                      
                        | W_node(prec, V_times, lst, _) -> 
                      // For a product we want the product of the common constant factors in each term.
                            let gg a (b:int64) = a*b
                            let l1 = List.fold factor_finder [] lst
                            if l1=[] then cc else (List.fold gg 1L l1) :: cc
                        | W_node(prec, V_plus, lst, _) -> 
                      // For a sum we want the hcf of the constant factors in each summand.
                            let rec gcd_of_list = function
                                | [item] -> item
                                | a::b::c-> gcd_of_list (euclid_gcd a b :: c)
                            let a0 = List.fold factor_finder [] lst
                            if a0=[] then cc else (gcd_of_list a0)::cc

                        | ex ->
                            if vd>=4 then vprintln 4 ("Adding unity for leaf or unrecognised subs form (forcing hcf=1) ex=" + xToStr ex)
                            singly_add 1L cc // Nb: ex is not a constant since checked above.

                let factor_finder_worker cc (grd_, arg) =
                    match arg.subs with
                        | AS_la(offsit, _)
                        | AS_leaf(offsit, _) -> factor_finder cc offsit
                        | _ ->
                            hpr_yikes ("factor finder (factor_finder_worker) other L1881: " + stToStr arg)
                            cc
                let members2 = List.fold factor_finder_worker [] members1
                //vprintln 3 (sprintf "factor finder: lengths: before %i, after %i. " (length members1) (length members2) + " were " + sfold i2s members2)
                let rec factor_find = function
                    | [] -> 0L // When no constants to work on
                    | [e] -> e
                    | a::b::t -> factor_find(euclid_gcd a b :: t)
                factor_find members2

        // Both input subscripts are, somewhat strangely, in bytes.  In the absence of any hcf information, we assume that only multiples of itemsize are already being used in the subscript expressions but the byte length needs dividing by the itemsize to get the word length in bytes.
        let subs_scale = (if hcf < 1L then 1L else hcf) // Avoid divide by zero when no hcf exists.
        let length_scale = (if hcf < 1L then int64 itemsize_bytes else hcf) 
        let div1 a b = if hcf < 2L then a else a/b
        let offss = sprintf "%i" (0L-offset)
        let nopf = subs_scale=1L && length_scale=1L && offset=0L
        vprintln 3 (sprintf "Scaling compute for %s, itemsize=%i bits (%i bytes), oldlen=%i hcf=%i, subs_scale=%i, length_scale=%i, offset=%s  (nopf=%A)"  name_wh itemsize_bits itemsize_bytes oldlen hcf subs_scale length_scale offss nopf)
        let rec scale_subscripts (grd, arg) =
            let scaler arg =
                match arg with
                | AS_leaf(x, co) ->
                    let co = if nonep co then None else Some((valOf co) / subs_scale)
                    AS_leaf(div2 x hcf, co)

                | AS_la(e', AS_lh(l,h)) -> AS_la(div2 e' hcf, gec_AS_lh(div1 l (int64 hcf), div1 h (int64 hcf))) // dont want to apply both - TODO

                | AS_rec _ -> // The contents of the array should themselves have been scaled before storing in there.
                    arg 
                    
                | other -> sf(sprintf "scaler other form: " + assToStr other)

            (grd, { arg with subs= scaler arg.subs; })
        let members3 = if nopf then members1 else  map scale_subscripts members1
        let newlen = (oldlen + int64(length_scale) - 1L) / length_scale
        vprintln 3 (sprintf "Scaling computed for %s, newlen=%i"  name_wh newlen)
        let destiny = P_arithmetic(subs_scale, offset, newlen)
        let rx = (clsKey, destiny, members3)
        let _ =
            let idep_ctrl (s, _, p) =
                let xs (_, y)  = y.serno
                let serials = map xs p
                let savit2 serno = vinfo.ideps.add serno (FWN clsKey, destiny)
                app savit2 serials
                //vprintln 3 (sprintf "idep_ctrl: L2175 ideps.add clsKey=%s s=%s destiny=%A serials=" clsKey s destiny + sfolds serials)
                ()
            idep_ctrl rx
        singly_add rx cc


//
//   only called on item2 at the moment
let scale_classa ww vinfo (ll, banko, msg, piz1, newats) resolved_region_size (members, number) cc =
    //let cc:(string * sri_digest_t option option * (pis_t * pis_t) * att_att_t list * (sel_t * subs_term_t) list) list = cc
    let ans = real_scale_class ww vinfo (ll, msg) resolved_region_size (members, number) []
    //dev_println (sprintf "Carolin: scale_classa  banko=%A  arity=%i" banko (length ans)) // not here
    let ans = map (fun (clskey, destinypiz, d) -> (clskey, banko, (piz1, destinypiz), newats, d)) ans
    ans @ cc
    
let scalarContentToStr = function
    | None   -> "None"
    | Some v -> romrenderx [v]

let zprint_rom = function
    | None -> "None"
    | Some i ->
        sfold_ellipsis 7 (fun (n, constval_o, (rominfo_updates:rominfo_updates_t)) -> sprintf "%i:%s:%i" n (scalarContentToStr constval_o) rominfo_updates.Count) [i]


// Additionally synthesised nets.
type snets_t = OptionStore<string, hexp_t>
        



// (call1)
let repack_fp_scans ww0 vinfo flatats aops  =
    let crm = vinfo.k2.stage_name
    let ww = WF 2 crm ww0 "first pass"

    let collate_RP2a1 () = // RP2a - conglomorate storage classes
        let jx00 = function // Take a note of all the tie structures and direct sons  - now a nop - delete me.
            | sc_lst -> 
                vprintln 3 (sprintf "collate: jx00 zeroth pass  : atter1 %s" (sfold aidToStr sc_lst))                
                ()
                
        let m_report_ans = ref [] // This for report only.

        let jx01 cc sc_lst = 
            vprintln 3 (sprintf "collate: jx1 first pass  : atter1 %s" (sfold aidToStr sc_lst))
            // On first pass we converge all zero-order classes.
            let (zeroes, highers) =
                let zeroth_pred = function //cf other aid complexity methods (depth_measure and so on)
                    | A_loaf _ -> true
                    | _ -> false
                List.partition zeroth_pred sc_lst

            let leadero = // Conglomorate step in collate.
                match zeroes with 
                | []    -> None
                | items ->
                    let newLeaderProtoClassName_ = conglomorate2 ww "L1280" vinfo items
                    // Any one representative is sufficient. Better to use a member than the mci returned by conglom since the mci can change as we go. So stick with a representative member (prefer low complexity so perhaps select more carefully).
                    let newLeader = hd items 
                    let msg = sprintf "collate_rp2a: hdcase pointer mapping: scl=%s" (sfold aidToStr sc_lst)
                    mutaddonce m_report_ans msg
                    Some(newLeader)
            (leadero, highers) :: cc

        let jx02 (leadero, highers) =
            //  We conglom inds and vias on this second pass.
            vprintln 3 (sprintf "collate: jx02 second pass: leadero=%A" leadero)
            let lst = if nonep leadero then highers else (valOf leadero) :: highers
            let _ = conglomorate2 ww "jx02" vinfo lst
            ()
        app jx00 !vinfo.memdesc_ties
        let highers = List.fold jx01 [] !vinfo.memdesc_ties        
        app jx02 highers
        reportx 3 "Storage class relationships (generated by collate_RP2a)" (fun x->x) !m_report_ans
        ()

    let collate_RP2a2 (pertinent_classes:pertinent_classes_t) = function // Called for each subscripted lhs to create the pertinent_classes database.
        | (ll, lst0) when is_string ll -> pertinent_classes
        | (ll, lst0) ->
            let cvd = -1 // -1 normally
            let its = xToStr ll
            if cvd >= 4 then vprintln 4 (sprintf "collate_RP2a2 vinfo its=%s, subscripting with pre ^%i " its (length lst0) + sfold asToStr lst0)
            let col0 cc = function
                  | AS_vn(ser, writer_, usages, item1, item2) ->
                      let descs = item1.descs // item1 only.
                      if cvd >= 4 then vprintln 4 (sprintf "collate_RP2a2: harvested from S%i %s descs=%s" ser (useToStr usages) (sfold aidToStr descs))
                      lst_union descs cc
                  | _ -> cc
            let pertinent_sclass_lst_00 = List.fold col0 [] lst0 // all sclasses

            let pertinent_sclass_lst =
                let folded4 cc ndsc =
                    match ndsc with // a NOP now.
                        |  _ -> singly_add ndsc cc  
                List.fold folded4 [] pertinent_sclass_lst_00
            if cvd >= 4 then vprintln 4 ("XF4: Pertinent seed storage classes for " + its + " are " + sfold aidToStr pertinent_sclass_lst)
            let upscan_from_desc_outer sc4 = 
                let m1 = "(ll=" + its + ", sc4=" + aidToStr sc4 + ")"
                let rec upscan_from_desc2 stack sc3 cc =
                    if memberp sc3 stack then sf (sprintf "upscan2 looped %s" (aidToStr sc3))
                    let stack_ = sc3 :: stack
                    match sc3 with // collected_targets why form here? collate0 would better do that.
                        | A_loaf _            -> singly_add sc3 cc                            
                        | A_tagged(scd, _, _) -> singly_add sc3 cc
                        | A_subsc(scd, _)       -> singly_add sc3 cc
                (sc4, List.foldBack (upscan_from_desc2  []) [sc4] [])
            let classes = map upscan_from_desc_outer pertinent_sclass_lst
            let wlst = list_once(list_flatten(map snd classes))
            let old_atts = valOf_or_nil (atl_assoc its flatats) 
            vprintln 3 (sprintf "collate1: RP2a2: wlst items for its=%s are (^%i) begin=%s old_atts=%A" its (length wlst) (sfoldcr aidToStr wlst) old_atts)
            pertinent_classes.Add (ll, (lst0, wlst, old_atts)) // repack_top.collate_RP2a2, main 1st write to pertinent_classes. 
            

    let collate_RP2b () =   // Find/collate all regions
        let unaddessable_discard_pred = function // Should throw these out earlier perhaps.
            | Memdesc0 md -> md.uid.baser < 0L
            | _           -> false
        let repacked_regions =
            let repacked_regions = new repacked_regions_t("repacked_regions")
            let relabel sck (sl, memdesc_rec) =
                if unaddessable_discard_pred memdesc_rec then
                    vprintln 6 (sprintf "rp22: discard unaddressable region            %s : %s " (aidToStr sck) (mdToStr memdesc_rec))
                else
                let sc =
                    match memdesc_rec with // Seems a bit late to be importing again?
                        | Memdesc0 md -> to_ndsc3a_f_sc md
                        | _  -> sf "Lost memdesc0"
                let sci = vinfo.sclasses_store.classOf_i "RP2b" sc
                match vinfo.sclasses_store.attribute_get "bank" sci with
                    | Some bank when false ->
                        //vprintln 3 (sprintf "rp22: add offchip region %s : %s : %s bank=%s" xleader (aidToStr sck) (mdToStr memdesc_rec) bank)
                        //repacked_regions.add xleader memdesc_rec
                        ()
                    | _ ->
                        match sci with
                            | (*Some*)xleader ->
                                vprintln 3 (sprintf "rp22: add region sc%i : %s : %s " xleader (aidToStr sck) (mdToStr memdesc_rec))
                                repacked_regions.add xleader memdesc_rec
                                ()
                            | _ when false (*None*) ->
                                vprintln 3 (sprintf "rp22: discard region %s : %s " (aidToStr sck) (mdToStr memdesc_rec))
                                ()
            for z in vinfo.memdesc_idx1 do app (relabel z.Key) z.Value done
            repacked_regions
        repacked_regions // end of collate_RP2b


    let _ = WF 3  crm ww0 "RP2a1: Starting collate memdesc info"
    let _ = collate_RP2a1 ()

    let _ = WF 3  crm ww0 "RP2a2: Form wlst and pertinent classes"

    let (all_lhs, pertinent_classes) =
        let m_all_lhs = ref []
        let dash pertintent_classes ll xlst =
            mutadd m_all_lhs ll
            collate_RP2a2 pertintent_classes (ll, map (fun (nnn_, (a, rec_vn_)) -> a) xlst)
        let pertinent_classes = (aops:memdesc_subsbase_ops_t).fold dash Map.empty
        (!m_all_lhs, pertinent_classes)


    // Please explain why we conglom second in this batch form? 
    let _ = WF 3 crm ww0 "RP2a3: xformer0" // This congloms all adjacent sc names occurring in the raw aops
    let _ =
        let marked = new flags_t("marked")
        let last = ref None
        let xforma0 ll lst0 =
            let same = if nonep !last then "---" else sprintf ("%A") (valOf !last = ll)
            last := Some ll
            vprintln 3 (sprintf "identity scanning %i %s same=%s" (x2nn ll) (netToStr ll) same)
            if is_string ll then ()
            else xformer0 ww (vinfo, marked) pertinent_classes ll
        for zz in aops do xforma0 zz.Key zz.Value done

    // Must do all renames before locking.
    vinfo.sclasses_store.resolveWildcards "repack:xformer"
    vinfo.sclasses_store.Lock "repack:xformer"
        

    vprintln 3 ("\n--Repack classes LOCKED now--\n")

#if OLD    
    let final_classes_summary__ =
        let m_sum = ref []
        let mxToStr (kk, items) =
            let bank =
                if nullp items then "n/a"
                else
                match vinfo.sclasses_store.offchip_bankOf kk with
                    | Some bank -> bank
                    | _         -> "----"
            sprintf "     key=sc%i bank=%s contents=^%i %s" kk bank (length items) (sfold aidToStr items)
        let summarise kk (nk_, items) = mutadd m_sum (kk, items)
        for z in vinfo.sclasses_store do summarise z.Key z.Value done
        reportx 3 "Final Store Classes Summary" mxToStr !m_sum
        !m_sum
#endif

    let _ = WF 3  crm ww0 "RP2b: Starting collate region info"    
    let repacked_regions = collate_RP2b ()

#if OLD
    let final_region_summary__ = // perhaps dont need repacked_regions if we have this 
        let m_sum = ref []
        let mxToStr (wx, items) =
            let regions = (repacked_regions:repacked_regions_t).lookup wx // they are in here now = map over all of these.        
            sprintf "     key=sc%i regions^%i %s" wx (length regions) (sfold mdToStr regions)
        let summarise kk items = mutadd m_sum (kk, items)
        for z in vinfo.sclasses_store do summarise z.Key z.Value done
        reportx 3 "Final Store Class Regions Summary" mxToStr !m_sum
        !m_sum
#endif
        
    let worklist = // New RP2c 
        // We nominally do this it in all_lhs order, but recursion of VNs perverts this and marked keeps track of what is complete.
        //let marked = new flags_t("marked")
        let xforma1 cc lhs_ll = // xform each array in turn, making an entry in the out dictionary.
            match pertinent_classes.TryFind lhs_ll with
                | None -> cc
                | Some(lst0, wlst0, ats) when is_string lhs_ll -> muddy "pertinent_classes"
                | Some(lst0, wlst0, ats) -> xformer1_top ww (vinfo) pertinent_classes cc lhs_ll lst0 wlst0 ats
        List.fold xforma1 [] all_lhs 

    vprintln 2 (sprintf "Length all_lhs=%i  Length worklist=%i" (length all_lhs) (length worklist))

    let wlToStr (lhs_ll, vnl, aids, ats_) = "lhs=" + xToStr lhs_ll + sprintf "  vnl^=%i" (length vnl) + " pqr scl=" + sfold aidToStr aids
    reportx 2 "Worklist Summary" wlToStr worklist

#if OLDCODE
    let ww = WF 3  crm ww0 "RP2c: Collating Regions"
    // Old RP2c -- collate regions
    let collate_RP2c = function // Called for each subscripted lhs to create the pertinent_classes database.
        | (ll, lst0) when is_string ll -> ()
        | (ll, lst0) ->
            let its = xToStr ll
            let (lst0_, classes, ats) = valOf(vinfo.pertinent_classes.lookup ll) // Find all work for this lhs (ie for this input wondarray).  This is possibly not helpful ever ?
            let regions_for_sc sc_lst  =
                let gather_regions1 m (cc, ls_flag) sc =
                    let sckey = aidToStr sc
                    let gather_regions2 m (dd, lsf) = function // keep if non trivial and collect wname.
                        | (a, Memdesc0 md) -> 
                                //vprintln 3 ("Non-trivial region considera-2 " + m + " " + mdToStr(Memdesc0 md))
                                let b = int64  md.uid.baser 
                                let lo = md.uid.length
                                let lsf = lsf || md.literalstring
                                match lo with
                                    | None -> (dd, lsf)
                                    | Some lo ->
                                        let e = (b, int64 lo)
                                        //vprintln 3 (sprintf "Considera-2a gather_regions2 %s, b=%A lo=%A" m b lo)
                                        (singly_add ([to_ndsc3a_f_sc md], e) dd, lsf)
                        | other -> (dd, lsf)
                    let regions_raw = memdesc_idx1.lookup sc // The immediate code is no longer the default/special case so we do not need the aboveclause
                    //vprintln 3 (sprintf "Considera-1b gather_regions1b %s, sc=%s no=%i" m sckey (length regions_raw))  
                    List.fold (gather_regions2 m) (cc, ls_flag) regions_raw
                                
                List.fold (gather_regions1 "col") ([], false) sc_lst

            let wlst_raw__ = // Result now unused but written to region_database_ but that is unused too!
                let prepare_wlst_entry sc_lst =
                    let (regions, ls_flag) = regions_for_sc sc_lst
                    vprintln 3 (sprintf "   Rp2c scToRegion info scl=%s regions=%s literalstring=%A" (sfold aidToStr sc_lst) (sfold (fun (_, reg) -> regionToStr reg) regions) ls_flag)
                    let regions =
                        let heapspace_p (vnds, region) = isHeap (fst region)
                        List.filter heapspace_p regions
                    let _ =
                        let addit sc = // somewhat heavyweight - add it to all sclx
                            let scs = aidToStr sc
                            //app (fun reg -> vinfo.region_database_.add scs (reg, ls_flag)) regions // mutable output for the regions - no longer used?!
                            ()
                        app addit sc_lst
                    (sc_lst, regions, ls_flag)
                prepare_wlst_entry classes
            ()

    let _ = // Side effecting iterate through the raw logged ops. No longer used?  TODO Disable it and run regression!
        for z in aops do collate_RP2c (z.Key, map (fun x -> fst(snd x)) z.Value) done
        ()
#endif
    let _ = WF 3  crm ww0 "RP2c: Finished xformer1"
    let _ = WF 3  crm ww0 "RP2c: Finished repack_fp_scans"
    (worklist, repacked_regions)  // end of repack_fp_scans (aka call1)



// -- call2
let repack_fp_rp3etc ww0 vinfo m_constidx_replacement_nets worklist repacked_regions =
    let k2 = vinfo.k2
    let crm = k2.stage_name
    let vd = k2.loglevel
    // RP3 - upcalled from rp4 
    // Map dn to xleader form here (remove older code above please: now removed using ifdef.) 
    let rp3 ww its ll ll_wlst0 =
        let pairup cc sc3 =
            match vinfo.sclasses_store.classOf_q sc3 with
                | None -> 
                    //dev_println (sprintf "rp3: its=%s: yikes  DROPPED OTHER FORM %A" its (aidToStr sc3))
                    cc

                | Some sci ->

            //collate on disjoint sci now please... TODO
                    let members = vinfo.sclasses_store.members -1 sci  // This code now copied into conglom - please avoid repeating it here. TODO.
                    //dev_println (sprintf "rp3: its=%s  For %s Members of sci=%i are %A" its (aidToStr sc3) sci members)
                    let zmembers =
                        let piz (_, arg) =
                            match arg with
                            | A_metainfo mci ->
                                let cls = demeta vinfo mci
                                let key0 = muddy (sprintf "need skey0 from %A" cls)
                                dev_println (sprintf "INPUT SIDE USE OF META !!")
                                match vinfo.groundfacts0.lookup key0 with
                                        | None -> (sci, false, [])
                                        | Some md -> (sci, md.literalstring, md.mats)
                                    
                            | other ->
                                let sci = vinfo.sclasses_store.classOf_i "rp3:pairup" other
                                // dev_println (sprintf "rp3 its=%s Is it ok to drop this?  %A  sc%i" its arg sci)
                                // dev_println ("This literalstring flag code is not doing anything now?")
                                (sci, (*literal string*)false, [])
                        map piz members
                    if vd >= 3 then vprintln 3 (sprintf "redundant code ?  hdcase sci was %i and now %s" sci (sfold (f1o3>>i2s) zmembers))  // we had sci just above ...  
                    let sci = f1o3 (hd zmembers)  // we had sci just above ...
                    let banko =
                        if nullp members then None
                        else vinfo.sclasses_store.attribute_get "bank" sci

                    let has_nullf =
                        let nullf_check arg = vinfo.sclasses_store.lookup_null_flag sci
                        disjunctionate nullf_check members
                    //vprintln 0  (sprintf "OXFORD repack.L2452 wx=%s zmembers %A  has_nullf=%A" wx zmembers has_nullf)
                    let literalstring_flag = disjunctionate f2o3 zmembers

                    (save_classed "L2557" vinfo {key=sci; bank=banko; lsf=literalstring_flag; ats=list_once(list_flatten(map f3o3 zmembers)); has_nullf=has_nullf }, sc3)::cc
        let pairs = List.fold pairup [] ll_wlst0
        let ans = list_once(map fst pairs)
        // make pairs for a report please
        let afun x = " ---  " + aidToStr x + " from " + sfold aidToStr (map snd (List.filter (fun (a,b)->a=x) pairs))
        reportx 3 ("RP3: region names for " + its) afun ans
        ans // end of RP3
        

    //-------------------------DETERMINE PACKING--------------------
    let ww = WF 3 crm ww0 "RP4: Starting packing decisions"
    let rp4_classed_scan ww (cc, cd) = function
        | (lhs_ll, vnl, ll_wlst00, old_memdesc_ats_) when is_string lhs_ll -> (cc, cd)
        | (lhs_ll, xformed_subs_ops, ll_wlst00, old_memdesc_ats) ->
            let its = xToStr lhs_ll
            let ww = WF 3  crm ww ("rp4_classed_scan start " + its)

            let wlToStr (lhs_ll, vnl, aids, ats_) = "lhs=" + xToStr lhs_ll + sprintf "  vnl^=%i" (length vnl) + " pqr scl=" + sfold aidToStr aids
            reportx 2 "Worklist Summary Again" wlToStr [(lhs_ll, xformed_subs_ops, ll_wlst00, old_memdesc_ats)]

            let ll_wlst = rp3 ww its lhs_ll ll_wlst00
            let lnn = x2nn lhs_ll
#if SPARE
            let newats__ = // Some ats of interest are more commonly in the memdesc0 records (old_memdesc_ats).
                  match ll with
                      | X_bnet ff -> (lookup_net nn).ats  // certainly want the init value if not other ats
                      | _ -> []
#endif
// tentative new approach do all the work of rp4 by leaf routines not any of the code now following
            let _ =
              if false then
                let vlst = xformed_subs_ops
                let arith_partition_prepare2 msg goon =
                    let getallwork arg cc = 
                       match arg with
                        | AS_vn(ser, writer_, usages, item1, item2) ->
                            let item = if goon then item2 else item1
                            //List.fold (fun cc x -> (SEL2 item2.serno, x)::cc) cc [item]
                            List.fold (fun cc x -> (SEL1 arg, x)::cc) cc [item]
                    let allwork = List.foldBack getallwork vlst []
                    let reorganised_classes = arithmetic_partition_f ww vd msg allwork
                    ()
                  
                let rk1_ = arith_partition_prepare2 (its + "-unif2") true // not needed since called later anway
                let rk2_ = arith_partition_prepare2 (its + "-unif1") false // we might want to use the returned value here.  - to make some vns of first partition return P_arithmetic
                ()

// old approach - still in use
            // FIRST PARTITION
            let (classed_regions, regions_partitioned, newnet_rewrites) = get_fst_ans ww vinfo its lhs_ll (xformed_subs_ops, repacked_regions, ll_wlst, old_memdesc_ats)
            let msg = its + ":CS1.2n"
            vprintln 3 (sprintf "CS1.2n %s no classes=%i, no regions=%i " msg (length classed_regions) (length regions_partitioned))


            // SECOND PARTITION
// The offchip bank allocations TODO explain how they interact with first and second partition.


// We invoke second_partition_f per region_id : the guarder and item_zero are a poorly-paired pair, so repair here using temporary ITEMGRD form.  One subtelty is that
// item_one partitions need to be done on the whole region, not just on given guards, I think, so collate them and reguard afterwards.
            let second_partition_prepare1 (clsKey, cls, piz1, vlst) =
                let banko = cls.bank
                //dev_println(sprintf "Carolin2 clsKey=%s banko=%s  " clsKey (valOf_or banko "None"))

                let get_region_size_f tfx = // If we find differring lengths then there is no resolution.
                    let cls_scanned =
                        match tfx with
                            |[A_metainfo mfi] ->
                                let cls = demeta vinfo mfi
                                cls.key
                            | _ -> -101 //
                    let check_only_one_lengther_scan (donesome, cc) (sc_, (baser, lengther), atts_) =
                        //vprintln 3 (sprintf "check_only_one_lengther_scan sc=%A, baser=%A, lengther=%A" sc_ baser lengther)
                        if not (isHeap baser) then (donesome, cc)
                        elif donesome && nonep cc then (donesome, cc)
                        elif donesome && valOf cc = lengther then (donesome, cc)
                        elif donesome then (donesome, None)
                        else (true, Some lengther)
                    let grs cc ((A_metainfo mfi, k2, _, regions), no) =
                        let cls = demeta vinfo mfi
                        if cls_scanned > 0 && cls_scanned <> cls.key then cc
                        else
                            //vprintln 0 (msg +  sprintf " tfx=%A resolved_region_size %i %s %s regions=%A" tfx no k11 k2 regions)
                            let cool = List.fold check_only_one_lengther_scan cc regions
                            vprintln 3 (sprintf "check_only_one_lengther_scan returns cool=%A" cool)
                            cool

                    let (donesome, bytesize_op) = List.fold grs (false, None) (zipWithIndex classed_regions)
                    //vprintln 3 (sprintf "resolved_region_size: donesome=%A, size in bytes=%A from sources %A " donesome bytesize_op ll_wlst)
                    bytesize_op
                let resolved_region_size_f = get_region_size_f
                let second_partition_prepare2 arg cc =
                    let goon = true // do item2. TODO simplify this code.
                    match arg with
                        | AS_vn(ser, writer_, usages, item1, item2) ->
                            let item = if goon then item2 else item1
                            //List.fold (fun cc x -> (SEL2 item2.serno, x)::cc) cc [item]
                            List.fold (fun cc x -> (SEL1 arg, x)::cc) cc [item]
                let allwork = List.foldBack second_partition_prepare2 vlst []
                let reorganised_classes = arithmetic_partition_f ww vd its allwork
                vprintln 3 (sprintf "lengths vlst=%i allwork=%i reorganised_classes=%i" (length vlst) (length allwork) (length reorganised_classes))
                // This is the sensible yet only call site currently - SI-DA and MI-FA if FA is placed in first item.
                //vprintln 0 (sprintf "bday: old_memdesc_ats=%A" old_memdesc_ats)
                //dev_println(sprintf "Carolin3 banko=%s  " (valOf_or banko "None")) // not here
                let ans = List.foldBack (scale_classa ww vinfo (lhs_ll, banko, its, piz1, cls.ats) resolved_region_size_f) (zipWithIndex reorganised_classes) []
                (clsKey, ans)
            let classes2 = map second_partition_prepare1 regions_partitioned
// The point is, does a given item_one or item_two get scaled differently in different use sites?  Item1's are unique over ll's and so, therefore, are item1,item2 pairs.

            
            let classes_scaled2 = 
                let tu = function
                    | (SEL1 os, subs_term) -> os // just pull it out again for now ==== ITEMGRD form IGNORED - TODO delete it.
                    | (SEL2 serno, substerm) -> muddy "demux me"
                let fof newname ((classname:string), banko, pis, b, d) = (newname, banko, pis, b, map tu d)
                let temp_unfix (clsKey, fivelst) = map (fof clsKey) fivelst
                list_flatten(map temp_unfix classes2)

            let _:(final_work_name_t * sri_digest_t option * (pis_t * pis_t) * att_att_t list * array_subs_t list) list = classes_scaled2

            let recollated =
                let keys = List.fold (fun cc (classname, banko, pis, ats, work) -> singly_add (classname, banko, pis, ats) cc) [] classes_scaled2 
                let collate (classname, banko, pis, ats) =
                    let sel cc (c, bo, p, a, w) = if c=classname && bo=banko && p=pis && ats=a then w@cc else cc
                    (classname, banko, pis, ats, List.fold sel [] classes_scaled2)
                map collate keys

            ((lhs_ll, if true then recollated else classes_scaled2)::cc, newnet_rewrites@cd) // end of rp4_classed_scan

    let m_new_lhs_cache = new OptionStore<int * string, string * net_att_t * further_net_att_t>("new_lhs_by_classname")
    let snets = new snets_t("snets") // scalar nets?
    let m_dynamic_heaps = ref [] // Freshly allocated heap spaces are put on here for inclusion in net list.
     
    let new_lhs_core ww site lhs_ll clsname v_length memdesc_ats rom2f banko romidxo = // Create a new net which is a scalar or finite slice of an input wond array.
        if not_nonep banko then
            let bankname = valOf banko
            let (_, ids, ff, f2) = protocols.retrieve_named_bondout_bank_pseudo ww k2 m_dynamic_heaps bankname
            vprintln 3 (sprintf "Rez offchip_named_bank name=%s" bankname)
            let f2 = lookup_net2 ff.n
            (ids, ff, f2)

        else
        match lhs_ll with
            | W_string _ -> sf "A read-only string should by called, not in ROM handler." // When indexed for char access.
            | X_bnet orig_ff ->
                let orig_f2 = lookup_net2 orig_ff.n
                match m_new_lhs_cache.lookup (orig_ff.n, clsname) with
                    | Some(ids, ff, f2) ->
                        vprintln 3 (site + sprintf ": new_lhs_core: returning cached net clsname=%s ans=%s" clsname (netToStr(X_bnet ff)))
                        (ids, ff, f2)
                    | None ->
                        let id =
                            match at_assoc "hintname" memdesc_ats with
                                | None          -> clsname
                                | Some "NEWOBJ" -> clsname
                                | Some hintname -> if clsname.Contains hintname then clsname else hintname + "_" + clsname
                        let itemsize_bits = encoding_width lhs_ll
                        let dims = if nonep v_length then []
                                   elif valOf v_length < 1L then sf (sprintf "should be trapped above ll=%i" (valOf v_length))
                                   else [ int64 (valOf v_length) ]
                        let vol(f:net_att_t) = at_assoc "volatile" orig_f2.ats = Some "true"
                        let remove_wond_annotation lst =
                            let kill_selected_attribute = function
                                | Nap(k, v) when k = g_wondtoken_marker -> false
                                | _                                     -> true
                            List.filter kill_selected_attribute lst
                        let ats = list_once (remove_wond_annotation orig_f2.ats @ memdesc_ats @ (if vol orig_ff then [ Nap("volatile", "true") ] else []))
                        let proto_ff = // Do not log this to net base
                               { orig_ff with
                                  n=        nxt_it()
                                  constval= [] // Remove constant data for now and reinsert later when needed.
                                  id=       id
                                  is_array= not_nullp dims
                               }
                        let proto_f2 =
                               { orig_f2 with
                                  xnet_io=  LOCAL
                                  vtype=    V_VALUE
                                  length=   dims
                               }
                        vprintln 3 (sprintf "Proto replacement clsname/id=%s : attributes=%A dims=%A rom2f=%A is %s" id  ats dims rom2f (netToStr(X_bnet proto_ff)))
                        let ans =
                            match rom2f with
                                | None ->
                                    let f2 = { proto_f2 with ats= ats }
                                    (clsname, proto_ff, f2)
                                | Some(hwm, scalar_contento, rom_contents) -> // If a ROM then insert constval field in proto - this will be helpful for subsequent recipe stages but is not used here.
                                    let const_items =
                                        if false then // leave this as 'false' for normal operation.
                                            dev_println ("TEMPORARY DISABLE ROM MODE for " + proto_ff.id)
                                            []
                                        else
                                        let scf = not_nonep scalar_contento && nonep v_length
                                        vprintln 3 (sprintf "rominfo log: Potentially converting %s to a constant or constant array (ROM)  [subs=%s] no_inits=%i : scalar_constant_flag=%A scalar_contento=%s" id (asubsLstToStr proto_f2.length) hwm scf (scalarContentToStr scalar_contento))
                                        vinfo.rominfo.add id (true, scalar_contento, rom_contents)
                                        if scf then [valOf scalar_contento] // ROM with scalar content
                                        else
                                            let rom_setup2 aa =
                                                let vv = match (rom_contents:rominfo_updates_t).lookup aa with
                                                            | None   -> 0I // uninitialised ROM locations are 0
                                                            | Some d -> d
                                                gec_XC_bnum({widtho=Some itemsize_bits; signed=orig_ff.signed}, vv)
                                            map rom_setup2 [0L..hwm]
                                    let romwords = length const_items
                                    let (ff, f2) =
                                        if nonep v_length then
                                            let idx =
                                                match romidxo with
                                                    | None ->
                                                        vprintln 3 ("repack: missing rom index but spurious run")
                                                        0
                                                    | Some v ->
                                                        let idx = (int) v
                                                        if idx < 0 || idx >= romwords then
                                                            vprintln 3  (sprintf "rom index is out of range: %i not in 0..%i " idx romwords)
                                                            if romwords > 0 then 0 else sf "L2756: no constant for ROM"
                                                        else idx
                                            let scalar_const_ff = // Alternative scalar const route - please explain
                                                      { proto_ff with
                                                            is_array=  false
                                                            constval= [const_items.[idx]]
                                                       }
                                            let scalar_const_f2 = // Alternative scalar const route - please explain
                                                      { proto_f2 with
                                                            ats=      ats
                                                            length=   []
                                                       }
                                            vprintln 3 (sprintf "clsname=%s create SCALAR CONST FROM ROM romidx=%A net=%s" clsname romidxo (netToStr (X_bnet scalar_const_ff)))
                                            (scalar_const_ff, scalar_const_f2)
                                        else // Full ROM
                                            let dims = (if romwords < 2 then [] else [ int64 romwords ])
                                            let ff = 
                                                   { proto_ff with
                                                         is_array=    not_nullp dims
                                                         constval=    const_items
                                                   }
                                            let f2 = 
                                                   { proto_f2 with
                                                         ats=      ats
                                                         length=   dims
                                                   }
                                            (ff, f2)
                                    (clsname, ff, f2) // End of ROM clause
                        m_new_lhs_cache.add (orig_ff.n, clsname) ans
                        vprintln 3 (site + sprintf ": new_lhs_core: returning newly-created net clsname=%s ans=%s" clsname (net_ffToStr true (f2o3 ans)))
                        ans
            | ll -> sf("classed_replacement other form: " + xToStr ll)



    let form_new_lhs ww ll cc9 (final_work_name, (banko:string option), (piz1, piz2), memdesc_ats, members) = 
        vprintln 3 "\n\n"
        let classname = dewn final_work_name
        let its = xToStr ll
        let (itemsize_bits, prec) =
            let prec = mine_prec g_bounda ll
            (valOf_or_fail "real_scale_width" prec.widtho, prec)
        let name_wh = its + ":/" + classname
        //vprintln 3 ("start new resol cache")
        let m_resolved = new OptionStore<int * int option, hexp_t>("resolved")
        
        vprintln 3 (name_wh + sprintf ": form_new_lhs: classname=%s banko=%A piz1=%s, piz2=%s" classname banko (pisToStr piz1) (pisToStr piz2))

        let lh_dyn_str = function
            | AS_lh (lower, upper) -> sprintf "lower=%i, upper=%i" lower upper
            | _ -> "<->"

        let (r_scale, r_offset, lh_str, scalarise2, v_netstr, v_length) = // new lhs basis info
            match (piz1, piz2) with
                | (P_scalarise(1), P_scalarise(1)) ->
                    //vprintln 3 (sprintf " Mode PS PS - rom2f=%A" rom2f)
                    (1L, 0L, "<-->", true, "scalarise_mode2: converted to scalar net", None)

                | (P_mapped(len, mapping, banko_), P_scalarise(1)) 
                | (P_scalarise(1), P_mapped(len, mapping, banko_)) ->
                    // TODO this heuristic compare is ignored for a mapped.
                      vprintln 3 (sprintf "one mapped: consider a mpx versus an array: len=%i max_mux_arity=%s" len (optionToStr i2s vinfo.k2.max_mux_arity))
                      //muddy "h/r express"
                      (1L, 0L, "<-->", false, "one mapped, one scalarised.", None)

                | (P_scalarise(1),  P_arithmetic(scale, offset, newlen))
                | (P_arithmetic(scale, offset, newlen), P_scalarise(1)) ->
                    let ww = WF 3 "form_new_lhs" ww (sprintf "Piz=AS Class=%s newlen=%i scale=%i offset=%i" name_wh newlen scale offset)
                    vprintln 3 (sprintf "tempprint: members^%i  = " (length members) + sfold asToStr members)
                    vprintln 3 (sprintf "\n\npiz=AS Class %s members^%i=[[" name_wh (length members) + sfold asToStr members + sprintf "]]\n" + " memdesc_ats=" + sfold atToStr memdesc_ats)
                    let dynranges = AS_lh(0L, newlen-1L) // List.fold dynrange AS_none members
                    let lh_str = lh_dyn_str dynranges
                    match dynranges with
                        | AS_lh (lower, upper) ->  // NOTE THIS WILL ALWAYS MATCH - JUST CREATED ABOVE
                            let llb = abs(upper-lower)+1L
                            let _ = cassert(llb > 0L, sprintf "Illegal array length after repacking: ll=%i" llb)
                            if llb=0L then
                                let m = "UnUSED/ ZERO LENGTH FIELD ARRAY " + xToStr ll
                                (scale, offset, lh_str, true, m, None)
                            elif llb=1L then (scale, offset, lh_str, false, "DEGENERATE ARRAY TYPE A (length=1) hence scalar net", None)
                            else (scale, offset, lh_str, false, sprintf "ARRAY (length=%i)" llb, Some llb)
                        | _ when length members = 1 -> (scale, offset, lh_str, false, "DEGENERATE ARRAY TYPE B (length=1) hence scalar net", None)
                        | other -> sf (sprintf "need dynrange other")

                | (P_arithmetic(scale, offset, newlen), P_mapped(len, mapping, banko_))
                | (P_mapped(len, mapping, banko_), P_arithmetic(scale, offset, newlen)) -> // This clause pair is just a copy of the one above at the moment.
                    let ww = WF 3 "form_new_lhs" ww (sprintf "piz=MA Class=%s newlen=%i scale=%i offset=%i" name_wh newlen scale offset)
                    vprintln 3 (sprintf "tempprint: members^%i  = " (length members) + sfold asToStr members)
                    vprintln 3 (sprintf "\n\npiz=MA Class %s members^%i=[[" name_wh (length members) + sfold asToStr members + sprintf "]]\n" + " memdesc_ats=" + sfold atToStr memdesc_ats)
                    let dynranges = AS_lh(0L, newlen-1L) // List.fold dynrange AS_none members
                    let lh_str = lh_dyn_str dynranges
                    match dynranges with
                        | AS_lh (lower, upper) ->  // NOTE THIS WILL ALWAYS MATCH - JUST CREATED ABOVE
                            let llb = abs(upper-lower)+1L
                            let _ = cassert(llb > 0L, "Illegal array length after repacking: ll=" + i2s64 llb)
                            if llb=0L then
                                let m = "UnUSED/ ZERO LENGTH FIELD ARRAY " + xToStr ll
                                (scale, offset, lh_str, true, m, None)
                            elif llb=1L then (scale, offset, lh_str, false, "DEGENERATE ARRAY TYPE A (length=1) hence scalar net", None)
                            else (scale, offset, lh_str, false, sprintf "ARRAY mapped (length=%i)" llb, Some llb)
                        | _ when length members = 1 -> (scale, offset, lh_str, false, "DEGENERATE ARRAY TYPE B (length=1) hence scalar net", None)
                        | other -> sf (sprintf "need dynrange other")

                | (piz1, piz2) -> sf (name_wh + sprintf ": unsupported combination: piz1=%s,  piz2=%s " (pisToStr piz1) (pisToStr piz2))

        let rom2f =
            if k2.gen_roms then make_romcheck ww its prec vinfo v_netstr piz1 piz2 r_scale members
            else None
        vprintln 3 (sprintf "form_new_lhs: %s %s is_rom=%A" classname v_netstr (not_nonep rom2f))
            
        let xgoToStr = function
            | (None, g, x)              -> optionToStr xToStr x + "/g=" + xbToStr g
            | (Some(clsname, ff, f2), g, x) -> optionToStr xToStr x + "/g=" + xbToStr g + "/ff=" + netToStr(X_bnet ff)
        let xgToStr = function
            | (l,g,x) -> optionToStr xToStr x + "/g=" + xbToStr g + "/ll=" + netToStr ll

        let def_new_lhs() =
            let (clsname, ff, f2) = new_lhs_core ww "defsite" ll classname v_length(*nasty free var*) memdesc_ats rom2f banko None
            let ff = { ff with id= htos [ ff.id; k2.split_prefix; its ] } 
            vprintln 3 (sprintf "repack: def_new_lhs: resol default resol ff.id=%s" ff.id)

            match snets.lookup ff.id with
                    | Some v -> v
                    | None ->
                        let l' = X_bnet (netsetup_log (ff, f2))
                        vprintln 3 (sprintf "%s: def_new_lhs classed_replacement is a(n) " name_wh + v_netstr + ": " + netToStr l' + " range=" + sprintf "%s" lh_str)
                        snets.add ff.id l'
                        l' 

        let additional_log msg v_length (ff:net_att_t, f2) = // The first log is unused. This is the second with full net name.
            let _ =
                if not_nonep rom2f then // Need to also log this extended net name as a ROM. Not nice.
                    vprintln 3 (sprintf "Additional rominfo log: %s: v_length=%A Converting the following constant array to a ROM: %s" msg v_length ff.id)
                    vinfo.rominfo.add ff.id (true, f2o3(valOf rom2f), f3o3(valOf rom2f))
                    ()
            (ff, f2)


        let vector_lhs_net romidx_o v_length t1 t2 =
            vprintln 3 (sprintf "vector_lhs_net  t1=%s t2=%s rom2f=%s" t1 t2 (zprint_rom rom2f))
            let nid = if t2 = "" then t1 else (t1 + "_" + t2)
            let (clsname, ff, f2) = new_lhs_core ww "vnet site" ll nid v_length memdesc_ats rom2f banko romidx_o
            (clsname, ff, f2)

        let scalar_lhs_net site tags = // A scalar net from a ROM should delete the other constvals from its attribute list, but we do not have incomming ROMs to repack currently.
            let v_length = None
            vprintln 3 (sprintf "scalar_lhs_net  t1=%s rom2f=%s" tags (zprint_rom rom2f))
            let (clsname, ff, f2) = new_lhs_core ww " snet site" ll tags v_length memdesc_ats rom2f (if false && site="item1" then banko else None) None
            (clsname, ff, f2)


        let dimsfold = (List.fold (fun c x -> c*x) 1L)
        let mx dims = if nullp dims then None else Some(dimsfold dims)
        let resol_function romidxo = function // Compose the effects of item1 and item2 into a composite subscription.
            | (None, None) -> def_new_lhs()
            | (Some(_, ff, f2), None) 
            | (None, Some(_, (ff:net_att_t), (f2:further_net_att_t))) ->
                let bondout_banko = at_assoc g_named_bondout_bank_pseudo f2.ats
                let bondout_ff_o =
                    match bondout_banko with
                        | None -> None
                        | Some bankname ->
                            let (_, ids, ff, f2) = protocols.retrieve_named_bondout_bank_pseudo ww k2 m_dynamic_heaps bankname
                            Some ff
                match m_resolved.lookup (ff.n, None) with
                    | Some ov -> ov
                    | None ->
                        match bondout_ff_o with
                            | Some bondout_ff -> X_bnet bondout_ff
                            | None ->
                                let (ff, f2) =
                                    let v_length = mx f2.length
                                    let (clsname, ff, f2) = vector_lhs_net None v_length ff.id ""
                                    additional_log "resol1" v_length ({ ff with id= htos [ ff.id; k2.split_prefix; its ]}, f2)

                                vprintln 3 (sprintf "resol solo resol ff.id=%s  (bondout bank=%A)" ff.id bondout_banko) 
                                let rr = 
                                   match snets.lookup ff.id with
                                       | Some v -> v
                                       | None ->
                                           let l' = X_bnet (netsetup_log(ff, f2))
                                           mutadd m_constidx_replacement_nets l'
                                           snets.add ff.id l'
                                           l'
                                m_resolved.add (ff.n, None) rr
                                rr
            | (Some (_, ff1, f21), Some (ff2x, ff2, f22)) -> // Quadratic clause
                let bondout_banko = at_assoc g_named_bondout_bank_pseudo f21.ats
                let bondout_ff_o =
                    match bondout_banko with
                        | None -> None
                        | Some bankname ->
                            let (_, ids, ff, f2) = protocols.retrieve_named_bondout_bank_pseudo ww k2 m_dynamic_heaps bankname
                            Some ff

                match m_resolved.lookup (ff1.n, Some ff2.n) with
                    | Some ov -> ov
                    | None ->
                        vprintln 3 (sprintf "resol quad %s vs %s (%i, %i)" ff1.id ff2.id ff1.n ff2.n)
                        match bondout_ff_o with
                            | Some bondout_ff -> X_bnet bondout_ff
                            | None ->
                                let (ff, f2) =
                                    let vec_leno =
                                        match (f21.length, f22.length) with
                                            | ([], []) -> None
                                            | (l1, l2) -> Some(dimsfold (l1@l2))
                                    let (clsname, ff, f2) = vector_lhs_net romidxo vec_leno ff1.id ff2.id
                                    additional_log "resol2" vec_leno ({ ff with id= htos [ ff.id; k2.split_prefix; its ] }, f2)
                                vprintln 3 (sprintf "resol quad resol ff.id=%s  (bondout bank=%A)" ff.id bondout_banko) 
                                let rr = 
                                   match snets.lookup ff.id with
                                       | Some v -> v
                                       | None ->
                                           let l' = X_bnet (netsetup_log (ff, f2))
                                           mutadd m_constidx_replacement_nets l'                
                                           snets.add ff.id l'
                                           l'
                                m_resolved.add (ff1.n, Some ff2.n) rr // Cache the result
                                rr

        let product piz1 piz2 =
            match (piz1, piz2) with
                | (P_arithmetic(scale, offset, newlen), P_mapped(len, mapping, banko_))
                | (P_mapped(len, mapping, banko_), P_arithmetic(scale, offset, newlen)) ->
                    (scale, offset, newlen) // mapped aspect does not matter - please say why.
                    
                | (P_scalarise(1), P_arithmetic(scale, offset, newlen))
                | (P_arithmetic(scale, offset, newlen), P_scalarise(1)) -> (scale, offset, newlen)


                | (P_arithmetic(scale, offset, newlen), P_arithmetic(scale2, offset2, newlen2)) -> muddy "(scale, offset, newlen * newlen2)"
                | _ -> sf(sprintf "other product %A x %A" piz1 piz2)

        let fo_lookup site item =
            let m0 = (sprintf "fo_lookup %s: item=" site  + stToStr item)
            vprintln 3 m0

            let get_vvale msg () =
                match item.subs with
                    | AS_leaf(v, _) -> v
                    | AS_rec(_, v, n1) ->
                        //dev_println (sprintf "AS_rec Lee %s " (asToStr n1))
                        v 
                    | other -> sf (msg + sprintf ": L2748 vvale other %A " item.subs)

            match item.subs with 
            |  AS_fa_2(orig_l, tagKey, offset) ->
                if nonep banko then // Onchip, the tag commonly becomes a scalar net or field array. Offchip it remains a constant offset.
                    ([Some (scalar_lhs_net site tagKey), X_true, None], X_false) 
                else
                    //dev_println (sprintf "Carolin fa2 left (not untouched) %s" (xToStr offset))
                    ([(None, X_true, Some offset)], X_false) // Onchip case.
            | _ ->


            match vinfo.ideps.lookup item.serno with
                | Some(workName, P_scalarise(1)) ->
                    let workName_s = dewn workName //(sprintf "SWX%i" workName)
                    //vprintln 3 (sprintf "repack: fo_lookup %s: scalarising clause workName=%s romidx=%A" site workName_s romidx)
                    let rname = "SCAL" + workName_s
                    ([(Some (scalar_lhs_net site rname), X_true, None)], X_false)
                     
                | Some(workName, P_mapped(len, mapping, banko)) ->
                    let workName_s = dewn workName //(sprintf "SWX%i" workName)
                    let msg = (sprintf "fo_lookup %s: P_mapped clause: len=%i workName=%s banko=%A" site len workName_s banko)
                    vprintln 3 msg

                    if not_nonep banko then
                        let (_, ids, ff, f2) = retrieve_named_bondout_bank_pseudo ww k2 m_dynamic_heaps (valOf banko)
                        ([(Some(ids, ff, f2), X_true, Some(get_vvale msg ()))], X_false)

                    elif as_fconstantp item.subs then
                        let cvale = match item.subs with 
                                       | AS_leaf(_, Some n64) -> n64
                                       //| AS_la(n64, _)   -> n64
                                       | other -> sf (sprintf "fo_lookup 2 cvale other %A " item.subs)
                        //vprintln  0 (sprintf "find_map_entry: : cvale=%A cf %A" cvale mapping)
                        let rec find_map_entry = function
                            | [] -> sf (m0 + sprintf ":find_map_entry: workName=%s. No mapping found: cvale=%A   mapping=%A" workName_s cvale mapping)
                            | (idx, (rname, no_usef, (bb, _)))::_ when bb=cvale ->
                                if no_usef then
                                    hpr_yikes(sprintf "%s: Compile-time null-pointer dereference for array region %s address %i.  " m0 rname cvale)
                                    ([], X_true)
                                else ([(Some (scalar_lhs_net site rname), X_true, None)], X_false)
                            | _ :: tt -> find_map_entry tt

                        find_map_entry mapping
                    else // leave to run time with a mux
                        let vvale = get_vvale msg ()
                        let is_a_group = // Seems an odd place to define strobe groups - vvale is not necessarily a simple scalar to which a strobe group is readily attached (it can be any general expression).
                            match vvale with
                                | X_bnet _ -> (ignore(open_enum msg vvale); true)
                                | other -> false // sf (sprintf "Cannot open enum for %s" (xToStr vvale))
                        let strobes =
                            let scanall (idx, (rname, no_usef, (bb, _))) =
                                // This will create an enum that needs closing for logic optimisations and tidy width fields in rendered RTL.
                                if no_usef then (None, X_false, None)
                                else (Some(scalar_lhs_net site rname), ix_deqd (xi_num idx) vvale, None) // 
                            map scanall mapping
                        let segfault_check_cond = // Condition for reporting null-pointer deref error abend.
                            let faultall cc (idx, (rname, no_usef, (bb, _))) =
                                // This will create an enum that needs closing for logic optimisations and tidy width fields in rendered RTL.
                                if no_usef then ix_or (ix_deqd (xi_num idx) vvale) cc else cc
                            List.fold faultall X_false mapping
                        let _ =
                            if is_a_group then
                                let _ = close_enum (msg + sprintf ": close_enum. members=^%i" (length mapping)) vvale
                                vprintln 3 (sprintf "closed strobegroup %s" workName_s)
                        (strobes, segfault_check_cond)

                | Some(workName, P_mapped2(mapping)) ->
                    let workName_s = dewn workName
                    vprintln 3 (sprintf "fo_lookup %s: P_mapped2 clause workName=%s" site workName_s)
                    let vale =
                        match item.subs with
                            | AS_leaf(_, Some n64) -> n64
                            //| AS_la(n64, _)   -> n64
                            | other -> sf (sprintf "fo_lookup 3 vale other %A " item.subs)
                    let rec scan idx = function
                        | [] -> sf (m0 + sprintf ": workName=%s no mapping for value %A in mapping list %A" workName_s vale mapping)
                        | bb::_ when bb=vale -> xi_num idx
                        | _ :: tt -> scan (idx+1) tt
                    let mv2 = scan 0 mapping
                    let segfault_check_cond = X_false // TODO - missing for P_mapped2 forms
                    ([(Some(scalar_lhs_net site workName_s), X_true, Some mv2)], segfault_check_cond)

                | Some(workName, P_arithmetic(scale, offset, newlen)) ->
                    let workName_s = dewn workName
                    vprintln 3 (sprintf "fo_lookup %s: arithmetic clause final_work_name=%s" site workName_s)
                    let gexer = function
                        | AS_leaf(e, _) -> e
                        | AS_la(e, _)   -> e
                        | AS_rec(_, e, _) -> e
                        | other -> sf (sprintf "gexer other")
                    let ns = gexer item.subs
                    let ns = if scale > 1L then ix_divide ns (xi_num64 scale) else ns
                    ([(Some(vector_lhs_net None (Some newlen) workName_s ""), X_true, Some ns)], X_false)
                     
                | None -> sf (sprintf "fo_lookup %s missing from ideps (index descriptions). site=%s" item.serno site)

        let ix_plus_opt = function
            | (None, None)   -> None
            | (None, Some x) -> Some x 
            | (Some x, None) -> Some x 
            | (Some x, Some y) ->
                hpr_yikes "ix_plus_opt: who tests scale one by the arity of the other"
                dev_println "ix_plus_opt: who tests scale one by the arity of the other"                
                Some (ix_plus x y) 

        let gen_a_one_map cc = function
            | AS_vn(ser, writer, uitem,  item1, item2) ->
                let (item1', flt1) = fo_lookup "item1" item1
                let (item2', flt2) = fo_lookup "item2" item2
                let segfault_expr___ = ix_or flt1 flt2 // ignored for now - can generate a runtime abend syndrome on out-of-bounds array subscript please.
                let array_subs_fault___ = false // for now 
                let romidxo = if as_fconstantp item2.subs then Some(as_manifest "romidxo-item2" item2.subs) else None
                vprintln 3 (sprintf "repack: gen_a_one_map AS_vn:item1=%s item2=%s" (assToStr item1.subs) (assToStr item2.subs))
                vprintln 3 (sprintf "repack: gen_a_one_map item1'=" + (sfold xgoToStr item1') +  "  item2'=" + sfold xgoToStr item2')
                let e'' = map (fun ((l1o, ga, a), (l2o, gb, b)) -> (resol_function romidxo (l1o,l2o), ix_and ga gb, ix_plus_opt(a, b))) (cartesian_pairs item1' item2')
                if nullp e'' then hpr_yikes(sprintf "repack: no pairs for %i x %i" (length item1') (length item2'))
                vprintln 3 (name_wh + ": vn catchall: plos " + useToStr uitem + " to " + sfold xgToStr e'' + sprintf " scalarise=%A prodarity=%i x %i" scalarise2 (length item1') (length item2'))
(*
                let newvl = if scalarise2 || spred
                                    then map (fun (l, g, e_) -> (g, l)) e''
                                    else map (fun (l, g, e)  -> (g, ix_asubsc l e)) e''
*)
                let newvl =
                    let sx = function
                        | (l, g, None)    -> (g, l)                 // Scalarised
                        | (l, g, Some e)  -> (g, ix_asubsc l e)     // Indexed
                    map sx e''
                let catchall_do cc9 (xe, g) =
                    let oldv = ix_asubsc ll xe // This needs to remain decorated since it is used as a lookup key.
                    let genans cc (g1, newv) =
                        let ans = gen_ARG(oldv, ix_and g1 g, newv) // Creates an ARGM actually.  An ARFF would apply to all users of a wondarray.
                        // let ans = ARFF(undecorate e, { scale=xi_num64 scale; offset=xi_num64 offset; replacement=l'; info=ix_pair (xi_num 0) (xi_num64(valOf v_length)) })
                        lprintln 3 (fun()->name_wh + ": 'vn' rewrite-rule: one_map ARGM for " + xToStr xe + sprintf "(nn=%i) ans=%s " (x2nn oldv) (arToStr ans))
                        singly_add ans cc
                    lst_union (List.fold genans [] newvl) cc9

                match uitem with
                    | Mono_use e    -> catchall_do cc (e, X_true)
                    | Multi_use lst -> List.fold catchall_do cc lst
                        
            | other -> sf ("repack gen_a_one_map other:" + asToStr other)

        let one_maps = List.fold gen_a_one_map [] members
        let gen_a_discrete_mapping cc2 = function
           | AS_vn(ser, writer, uitem, item, item2) -> 
                let l' = def_new_lhs()
                match item2.subs with
                    | AS_leaf(e, _) -> // data array clause
                        vprintln 3 (name_wh + ": vn: map " + useToStr uitem + " to " + xToStr e + sprintf " scalarise=%A" scalarise2)
                        let e'' = e // xi_plus(as_valOf "l1448" item, e)
                        let newv = if scalarise2 then l' 
                                   else ix_asubsc l' e''
                        let one_map_dofi cc9 (e, g) =
                            let oldv = ix_asubsc ll e
                            // The actual mutliplexors are created by the expression re-writer based on ARFF instructions.
                            let (scale, offset, newlen) = product piz1 piz2
                            let ans = ARFF(undecorate e, { scale=xi_num64 scale; a_offset=xi_num64 offset; replacement=l'; info=ix_pair (xi_num 0) (xi_num64(valOf v_length)) })
                            lprintln 3 (fun()->name_wh + ": 'vn' rewrite-rule: discrete_map ARFF for " + xToStr e + " ans=" + ar_subsc_mappingToStr ans)
                            // BUG: ARFF should be stored on ll, not on oldv.
                            //singly_add (ll, ans) cc9      <- ans is discarded completely now!
                            //singly_add (oldv, ans) cc9                            
                            cc9
                            
                        match uitem with
                            | Mono_use e    -> one_map_dofi cc2 (e, X_true)
                            | Multi_use lst -> List.fold one_map_dofi cc2 lst

       // We rewrite heap constants using the packed enumeration coding held in mapping and any uses with constant subscripts are recoded here ...
       // or are the constants just above. ??
                    |  other ->
                        hpr_yikes (name_wh + ": repack gen_a_discrete_mapping other L2093: " + assToStr other)
                        cc2
                            
           |  other -> sf (name_wh + ": repack gen_a_discrete_mapping other: " + asToStr other)
        //OLD : fails for disparate updates [ARF(l, { scale=scale; offset=offset; replacement=l'; info=ix_pair (xi_num 0, xi_num(valOf length)) })]
        let discrete_maps =
            if scalarise2 || nonep v_length then []// We do not need a discrete map when scalarise holds since this work is handled by one_maps.
            else
                let work = List.fold gen_a_discrete_mapping [] members
                let items = List.fold (fun c (l,b) -> singly_add l c) [] work // Collate since meox assoc wants all in one ARL.
                vprintln 3 (name_wh + ": 'vx' rewrite-rule: ARLs for " + xToStr ll + sprintf " ans=^%i" (length items))
                map (fun l -> gen_ARL(l, List.fold (fun c (l',b)->(if l=l' then singly_add b c else c)) [] work)) items
        discrete_maps @ one_maps @ cc9

    let ww = WF 3  crm ww0 "RP4: starting classify"

    let _:(hexp_t * array_subs_t list * aid_t list * att_att_t list) list = worklist
    //let worklist = list_flatten (map (fun (zp, wlst0, ats) -> map (fun z -> (z, wlst0, ats)) zp) worklist) // Crazy reformat
    let (classed_arrays, newnet_rewrites) = List.fold (rp4_classed_scan ww) ([], []) worklist


    let (stringpool_indecies, stringpool_roms) = // Create stringpool ROMs
        let sl =
            let m_sl = ref []
            for z in vinfo.string_literals do mutadd m_sl (z.Key, z.Value) done // TODO support a fold combinator for ListStores. Done now? 
            !m_sl
        let _ = WF 3 crm ww0 (sprintf "Creating %i stringpools" (length sl))
        let create_stringpools (mm:Map<string, hexp_t list>) (kk, vv) =
            match vinfo.sclasses_store.classOf_q (A_loaf(sprintf "SLSC%i" kk)) with
                | Some sci ->
                    //let wy_ = vinfo.sclasses_store.members -1 sci
                    //vprintln 0 (sprintf "stringpool ROM create name=%A  %A wx=%A wy=%A" kk (sfold xToStr vv) sci wy_)
                    let wx = vinfo.sclasses_store.sciToStr sci
                    mm.Add(wx, lst_union vv (valOf_or_nil(mm.TryFind wx)))
                | None ->
                    vprintln 0 (sprintf "+++ repack: dropped stringpool item %A %A" kk vv)
                    mm
        let pools = List.fold create_stringpools Map.empty sl


        let (stringpool_indecies, stringpool_roms) =
            let create_stringpool (wx, strings) =
                let create_str_string cc = function
                    | W_string(ss, _, _) -> singly_add ss cc
                    | _ -> sf ("create_str_string L2962")
                let str_strings = strings //List.fold create_str_string [] strings
                let name = "stringpool_" + wx
                let indexed =
                    let rec indexer pos = function
                        | [] -> []
                        | ((W_string(ss, _, _)) as h)::tt ->
                            let bytes = map (fun (chr:char) -> int(chr)) (explode ss) @ [0] // Null terminate
                            let l = length bytes
                            let _ = vprintln 3 (sprintf "   %s  offset=%i  string=%s" name pos (netToStr h))
                            (h, pos, bytes) :: indexer (pos+l+1)  tt
                    indexer 0 str_strings
                let string_rom_data = list_flatten (map f3o3 indexed)
                let (n, ov) = netsetup_start name
                let prec = {signed=Signed; widtho=Some 8}// Using 8-bit chars.
                let ats = [ Nap("partial_array_subscript", "true")]
                let ff =
                    { g_null_net_att with
                          n=        n
                          constval=map(fun (n:int) -> gec_XC_bnum(prec, BigInteger n)) string_rom_data
                          width=valOf prec.widtho; 
                          id=       name
                          signed=   prec.signed
                          is_array= true
                    }
                let f2 = 
                    { g_null_further_net_att with
                          length=   [int64(length string_rom_data)]
                          ats=      ats
                          // Might be nicer to store these as XC_strings - more humanreadable
                    }

                let stringpool_rom = netsetup_log (ff, f2)
                let romnet = X_bnet (stringpool_rom)
                ((wx, romnet, indexed), romnet)
            List.unzip(map create_stringpool (Map.toList pools))
        (stringpool_indecies, stringpool_roms)
        
    let ww = WF 3  crm ww0 "RP7: start making mapping rewrite directives"

    let _ = // Write out stringrom tables
        let tabgen (wx_, romnet, indexed) =
            let mat (h, pos, bytes) = [ i2s pos; i2s (length bytes); netToStr h ] 
            let table = tableprinter.create_table("LiteralString ROM " + netToStr romnet, [ "Offset"; "Length"; "Content String"], map mat indexed)
            aux_report_log 2 vinfo.stage_name table
        app tabgen stringpool_indecies
        
    let stringrom_rewrites =
        let mk_stringrom_mapping (wx_, romnet, indexed) =
            let mgen (h, pos, bytes_) =
                (h, ix_asubsc romnet (xi_num pos))
            map mgen indexed
        list_flatten(map mk_stringrom_mapping stringpool_indecies)
        
    let array_mapping =
            let create_rewrites cc = function
               | (l, []) -> cc
               | (l, classes) ->
                   let ww = WF 3 "form_new_lhs" ww ("for lhs " + netToStr l)
                   List.fold (form_new_lhs ww l) cc classes
            List.fold create_rewrites [] classed_arrays

    let _ = WF 3  crm ww0 "RP8: starting transclose" // Some subscript operations are inside others - so need to apply one set of maps to another.
    let heapconst_mapping =
        let rrr = ref []
        let heapconst_mgen hca =
            match hca with
              | (orig, hexp_pair_1_, hexp_pair_2_, scl, bi) ->
                  let prefer_shortest_form cc = function
                      | A_loaf sc -> Some (A_loaf sc)
                      | _ -> cc
                  let s9 =
                      match List.fold prefer_shortest_form None scl with
                          | Some x -> x
                          | None ->
                              if length scl >= 2 then if vd>=3 then vprintln 3 (sprintf "hdcase: selecting hd L329 from " + sfold aidToStr scl)
                              if length scl=0 then sf("no scl") else hd scl
                              
                  if vd>=3 then vprintln 3 (sprintf "RP8.1bb: using %s as leader for class containing %s" (aidToStr s9) (sfold aidToStr scl))
                   
                  let trawl c sc =
                      let key = aidToStr sc
                      vprintln 3 (sprintf "RP8.1b: using %s key" key)
                      let (found, mapping) = vinfo.object_remaps.TryGetValue key // Dont use ListStore since want to check old on write.
                      if found then lst_union mapping c else c
                  let mapping =
                      match List.fold trawl [] scl with
                          | mapping when not(nullp mapping) -> (mapping)
                          | [] ->
                              vprintln 3 (sprintf "was null for %s - looking up on %s" (sfold aidToStr scl) (sfold aidToStr scl))
                              // need new mapping - create on-the-fly

                              let get_region cc sc =
                                  let idx1 = vinfo.memdesc_idx1.lookup sc
                                  let reger cc = function
                                      | (_, Memdesc0 md) when isHeap md.uid.baser -> (int64 md.uid.baser, valOf_or md.uid.length 0L)::cc
                                      | _ -> cc
                                  List.fold reger cc idx1
                              let regions = List.fold get_region [] scl
                              vprintln 3 (sprintf "heapsconst_mapping: Now have regions %A" regions)
                              let has_nullf = false
                              let (work_name_, mapping, len_) = seed_enumap vinfo "on-the-fly" has_nullf scl regions
                              mapping  

                  let str_form = ndTox ww vinfo "msg" s9
                  let ans = ref None
                  let report_nomapping msg =
                      vprintln 2 msg
                      dev_println msg

                  let rec gen = function // copy 2/2.  TODO please abstract this code.
                      | (p, (q, _))::tt when q=bi ->
                          if vd>=4 then vprintln 4 (sprintf "RP8.2: heapconstant map %s qcx %s " (xToStr str_form) (aidToStr s9) + "  -> " + i2s p)
                          if !ans <> None && !ans <> Some p then vprintln 0 ("+++ repack: oh dear: differing rewrites for " + i2s64 q + " old=" + i2s(valOf !ans) + " new=" + i2s p)
                          ans := Some p
                          mutadd rrr (f1o5 hca, ix_pair str_form (xi_num p)) // The actual mapping pair
                          gen tt
                      | _::tt -> gen tt
                      | [] when !ans <> None -> ()
                      | [] -> report_nomapping (k2.stage_name + ": RP8.2b0: No mapping for " + xToStr(f1o5 hca) + " scl=" + sfold aidToStr scl + "; bi=" + i2s64 bi + ";")
                  vprintln 3 (sprintf "RP8.2b1: %i items in mapping for " (length mapping) + xToStr str_form + " scl=" + sfold aidToStr scl + "; bi=" + i2s64 bi + ";")
                  //if nullp scl then report_nomapping  ("+++" + k2.stage_name + ": heapconst mapping: for " + xToStr (f1o5 hca) + sprintf " did not find any store classes for qc%i " ss + sprintf " (needed for %A)" bi)
                  if nullp mapping then
                      report_nomapping  (k2.stage_name + ": heapconst mapping: for " + xToStr (f1o5 hca) + sprintf " did not find any heapconst mappings for %s " (aidToStr s9) + sprintf " (needed for %A)" bi  + " scl=" + sfold aidToStr scl)
                  else gen mapping
                  // let f = memdesc_idx0.TryGetValue s  not needed afterall ? s could have stayed a string list
                  ()
        for a in vinfo.heapconsts do heapconst_mgen a done
        vprintln 3 (i2s(length !rrr) + " field const subscripts in use");
        !rrr

    let mm0 = add_pairmap_pairs (makemap1 array_mapping) (heapconst_mapping)
    let add_net_mapping cc (ll, rr) =
        vprintln 3 (sprintf ": 'cp' rewrite-rule: net declaration mapping: AR rewrite:  %s to %s " (xToStr ll) (xToStr rr))
        makemap1_add cc (gen_AR(ll, rr))
    //let const_pairs =  [] // None propagated in this recipe stage
    //let mm0 = List.fold augment_mapping mm0 const_pairs

    let newnet_rewrites = list_flatten newnet_rewrites
    let mm0 = List.fold add_net_mapping mm0 (newnet_rewrites @ stringrom_rewrites)
    let newnets = stringpool_roms @ map snd newnet_rewrites @ !m_dynamic_heaps
    let array_mapping = map_transclose mm0
    let _ = WF 3  crm ww0 "finished RP8 transclose"
    (newnets, array_mapping) // end of repack_fp_rp3etc (aka call2)



(*
 * Repack 
 *)
let repack_top ww0 (k2:repack_control_t) vm =
    let vd = 3
    let crm = k2.stage_name

    let memdesc_idx1 = new memdesc_idx1_t("memdesc_idx1") // Indexed by sc

    let vinfo:repack_settings_t =
        {
            k2=k2
            stage_name=              crm
            keys=                    new keys_t();
            mfore=                   new mapping_forwarding_t("mfore")
            sclasses_store=          new ksc_classes_db_t("FX", g_repack_cls_char)
            heapconsts=              new HashSet<heapconst_t>()
            groundfacts0=            new groundfacts0_t("groundfacts0")
            groundfacts1=            new groundfacts1_t("groundfacts1")
            string_literals=         new string_literals_t("string_literals")
            rominfo=                 new rominfo_t("rominfo")
            ideps=                   new ideps_t("ideps") // Index descriptions: stores subscript item decisions by item serial no
            memdesc_map=             new memdesc_map_t()
            object_remaps=           new object_remaps_t()
            memdesc_idx1=            memdesc_idx1
            memdesc_ties=            ref []
            m_flaws=                 ref []
            m_next_mci=              ref 8000
            metastore=               new OptionStore<int, sc_classed_t>("metastore")
            metastore_i=             new OptionStore<sc_classed_t, int>("metastore_i")            
        }

    let cc0 =  ([])

    let (pertinent_classes, repacked_regions) =
        let aops = new memdesc_subsbase_ops_t("subsbase_in")
        let flatats =
            let wMSC = ("repacker", cr_walker_gen ww0 vinfo aops, None, g_null_pliwalk_fun)
            repack_one_vm ww0 wMSC vinfo cc0 vm
        repack_fp_scans ww0 vinfo flatats aops // aka call1


    let m_constidx_replacement_nets = ref []
    let (newnets, array_mapping) = repack_fp_rp3etc ww0 vinfo m_constidx_replacement_nets pertinent_classes repacked_regions // aka call2

    let array_mapping =
        let is_rom_gone id = match vinfo.rominfo.lookup id with    // We here delete assigns that set up a ROM since that information is now conveyed as constval attributes.
                              | Some(true, _, _) -> true
                              | _ -> false

        let arcpred (pp_, gg_, (g_, l_, r_)) =
            //vprintln 0 ("arcpred " + xToStr l_)
            true

        let arcmapper (g2:strict_t) (lhs0, rhs0) =
            let ans =
                match lhs0 with
                    | W_asubsc(X_bnet ff, subs0, _) when is_rom_gone ff.id ->
                        (X_undef, rhs0) // An assign to X_undef will be deleted.
                    | other ->
                        // vprintln 0  (sprintf "arcmapper other form " + xToStr other)
                        (lhs0, rhs0)
            let mpf = fun (pp, g, x) -> [ x ]
            (ans, mpf)

        let bpred_bev1 = function
            | b ->
                //let _ = vprintln 0 ("bpred " + hbevToStr b)
                true

        let premap_bev1 g2 pre_bev =
            //dev_println (sprintf ": premap consider %s" (hbevToStr pre_bev))
            let postmap strict bevcmd = // postmap for bev
                //dev_println (sprintf ": postmap consider %s" (hbevToStr bevcmd))
                let rec postmapper bevcmd =
                    match bevcmd with
                    | Xif(g, b, f)       -> Xif(g, postmapper b, postmapper f)
                    | Xblock lst         -> Xblock(map postmapper lst)
                    | Xassign(W_asubsc(X_bnet ff, subs0, _), rhs0) when is_rom_gone ff.id ->
                        let bb = hbevToStr bevcmd
                        if vd>=4 then vprintln 4 (sprintf "ROM initialising assignment no longer needed " + bb)
                        Xcomment (" site of ROM setup " + bb + "\n")

                    | Xassign(X_bnet ff, rhs0) when is_rom_gone ff.id ->
                        if vd>=4 then vprintln 4 ("Commented out as scalar constant (trivial ROM):" + ff.id)
                        Xcomment (" site of write-once scalar setup " + hbevToStr bevcmd + "\n")

                    | bevcmd ->
                        if vd>=4 then vprintln 4 ("postmap misc of " + hbevToStr bevcmd)
                        bevcmd
                postmapper bevcmd
            (pre_bev, postmap)    


        hbev_agent_add (arc_agent_add array_mapping (arcpred, arcmapper, (true, false))) (bpred_bev1, premap_bev1)  
    
    let ww = WF 3  crm ww0 "mapping creation complete"

    let newnets = list_once(newnets @  !m_constidx_replacement_nets)
    //-------------------------SECOND PASS--------------------        
    let ww = WF 3  crm ww0 "RP9 second pass"
    let ans = repack_two_vm ww k2 (vinfo, array_mapping) (Some newnets) vm
    let _ = WF 1  crm ww0 "completed"
    ans



let repack_args =
        [
          Arg_int_defaulting("repack-max-mux-arity", 4, "Maximum first dimension in 2-D array before multiplying out (instead of muxing RAMs)");
          Arg_int_defaulting("repack-loglevel", 1, "Log clip level (set higher for more output)");
          Arg_int_defaulting("repack-constidx-scalarise", 128, "Upper limit of number of entries in an array with only constant index expressions to scalarise. Scalarise means convert array/RAM to register file.");
          Arg_int_defaulting("repack-varidx-scalarise", 128, "Upper limit of number of entries in an array with some variable index expressions to scalarise (convert to registers)");
          // Could have a bit limit instead... ?
          Arg_enum_defaulting("repack", ["enable"; "disable"], "enable", "Enable control for this operation");
          Arg_enum_defaulting("repack-to-roms", ["enable"; "disable"], "enable", "Designate arrays/memories that are assigned only once at each location with a constant value as ROMs and control their mirroring factor.");
          Arg_enum_defaulting("repack-ignore-leftover-error", ["enable"; "disable"], "disable", "Bypass abend on operation-leftover error (debugging use only).");           
          // need max size of array region to convert to regfile if all subscripts are constant
          //Arg_enum_defaulting("array-scalarise", ["true"; "false"], "false", "True to convert all array contents to scalars");
          
          Arg_required("split-prefix", 1, "Prefix string for RAM arrays", "");            

        ]


let opath_repack ww op_args vms =
    let c3 = op_args.c3
    let stagename = op_args.stagename
    let repack = (control_get_s stagename c3 op_args.stagename None) = "enable"
    if repack then
        let loglevel = max !g_global_vd (control_get_i c3 "repack-loglevel" 1)
        m_repack_loglevel := loglevel

        let m_settings_report = ref [] // Contents Discarded ?!
        let log_setting_for_report key def vale desc = mutadd m_settings_report (key, vale, desc)
        let bondouts = get_bondout_port_prams ww c3 stagename log_setting_for_report 

        let k2:repack_control_t
              = { 
                  max_mux_arity=  Some(control_get_i c3 "repack-max-mux-arity" 4) // Ignored at the moment...
                  constidx_rcs=   Some(control_get_i c3 "repack-constidx-scalarise" 128)
                  varidx_rcs=     Some(control_get_i c3 "repack-varidx-scalarise" 128)
                  stage_name=     stagename
                  split_prefix=   control_get_s stagename c3 "split-prefix" None
                  control=        c3
                  gen_roms=       (control_get_s stagename c3 "repack-to-roms" None) = "enable"    
                  bondouts=       bondouts
                  loglevel=       loglevel
                  ignore_leftover_error=  (control_get_s stagename c3 "repack-ignore-leftover-error" None) = "enable"                     // Debugging kludge - not recommended to ever use
                }

        


        let vms = 
            if false then [ systolic.gec_systolic_vm2 ww ] // for now very temp
            else vms

        map (repack_top ww k2) vms
    else
        g_repack_disable := true // suppress later warnings
        vms




let install_repack() =
    let _ = install_operator ("repack",  "Repack", opath_repack, [], [], repack_args @ g_bondout_arg_patterns)    
    0


(*
// Typical DefinitiveS examples
//  27:      @$star1$_TestClass[{SC:d21,400000}+{$offset,8}] := {SC:d33,400052};
//  38:      @$star1$_Capsule[{SC:d33,400009}+{$offset+datum,4}] := {SC:d19,400000};
// Line 27 gives us a tie {[d21], d33}
// Line 38 gives us a tie {d33->datum, d19}
*)


(* eof *)
