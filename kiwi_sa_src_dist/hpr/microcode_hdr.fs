(*
 * $Id: microcode_hdr.fs,v 1.3 2013-03-26 15:05:40 djg11 Exp $
 * microcode_hdr.sml
 *
 *
 * Contains definitions of the machine code for a microcontroller.
 *)
module microcode_hdr

open hprls_hdr

type mcode_t = 
  INST of  string * string * string * int * string

(*
 * Everyday ALU operations:
 *  (... perhaps basically aimed at the CBG PU17 microprocessor).
 * 
 *)

type addressing_mode_t =  
|       VREG of int  (* virtual register *)
|       PREG of int  (* physical register *)
|       INDEXEDS of string * addressing_mode_t  (* symbolic indexed *)
|       ABSS of string  (* symbolic abs or rel *)
|       ABS of int      (* memory location abs *)
|       IMMEDS of string * int option ref (* symbolic immediate *)
|       IMMED of int      (* constant immediate *)
|       NAM
|       FILLER




type  mcode_alu_t = MC_add | MC_sub | MC_adc | MC_sbb | MC_neg | MC_cmi
                      | MC_and | MC_or  | MC_xor | MC_mov | MC_not | MC_times
                      | MC_asl | MC_lsr | MC_asr | MC_rol


type inst_t = { nm:string; am:string; p:string; e:string }

(* Intermediate, imperative assembler code form *)
type icode_t =
    | Ij of inst_t * addressing_mode_t * addressing_mode_t * addressing_mode_t * int ref
    | Icompiled of icode_t * it_t
    | Ilabel of string * icode_t
    | Icomment of string
    | Idefs of int * string
    | I_filler3  (* Used only to suppress the unused cases warning in my other match traps *)



let operand_forms = [
  ("L", "label",     12, "", "");
  ("R", "register",  3,  "", "");
  ("X", "register",  3,  "", "");
  ("D", "register",  3,  "", "");
  ("A", "register",  3,  "", "");
  ("B", "register",  3,  "", "");
  ("i", "immediate", 9,  "", "")
 ]





(* Miscellaneous instructions: 0-*)
let i_rts  = { nm="rts";  am="0"; p="sp=sp+2;pc=[sp]";   e="[15:12]=0;[11:6]=0" }
let i_stop = { nm="stop"; am="0"; p="pc=-1";             e="[15:12]=0;[11:6]=1" }
let i_nop  = { nm="nop";  am="0"; p="";                  e="[15:12]=0;[11:6]=2" }
let i_disc = { nm="disc"; am="R"; p="sp=sp+R+R";         e="[15:12]=0;[11:6]=3;[2:0]=R"}
let i_rslt = { nm="rslt"; am="R"; p="Z=R";               e="[15:12]=0;[11:6]=4;[2:0]=R"}
let i_push = { nm="push"; am="R"; p="[sp]=R;sp=sp-2";    e="[15:12]=0;[11:6]=5;[2:0]=R"}
let i_pop  = { nm="pop";  am="R"; p="sp=sp+2;R=[sp]";    e="[15:12]=0;[11:6]=6;[2:0]=R"}

(* Computed unconditional; subroutine and conditional branch:  *)
let i_jmpr = { nm="jmpr"; am="(R)";   p="pc=D";          e="[15:12]=0;      [11:6]=7;[2:0]=D"}
let i_jsrr = { nm="jsrr"; am="(R)";   p="[sp]=pc;pc=D;sp=sp-2";e="[15:12]=0;[11:6]=8;[2:0]=D"}
let i_brzr = { nm="brzr"; am="R;(R)"; p="pc=R?pc:D";     e="[15:12]=0;[11:6]=9;[5:3]=R;[2:0]=D"}

(* Test and set *)
let i_tas  = { nm="tas";  am="(R);(R)"; p="R=[D];[D]=1";e="[15:12]=0;[11:6]=10;[5:3]=R;[2:0]=D"}

(* Jump; jsr : Not really needed?  0400-07FF : pc relative on real machine *)
let i_jsr  = { nm="jsr";  am="L";   p="[sp]=pc;pc=L;sp=sp-2";  e="[15:9]=2;[8:0]=L" }
let i_jmp  = { nm="jmp";  am="L";   p="pc=L";                  e="[15:9]=3;[8:0]=L" }



(* Immediate addressing: 0800-1FFF *)
let i_subi = { nm="subi"; am="Di"; p="D=D-i"; e="[15:11]=1;[10:8]=D;[7:0]=i"}
let i_lodi = { nm="lodi"; am="Di"; p="D=i";   e="[15:11]=2;[10:8]=D;[7:0]=i"}
let i_addi = { nm="addi"; am="Di"; p="D=D+i"; e="[15:11]=3;[10:8]=D;[7:0]=i"}






(* Conditional branch rnz : 2000-3FFF *)
let i_brz  = { nm="brz";  am="RL";    p="pc=R?pc:L";     e="[15:13]=1;[12:10]=R;[9:0]=L"}

(* Data processing ALU operations: 0x4000-7FFF *)
let i_add  = { nm="add";  am="DAB"; p="D=A+B"; e="[15:14]=1;[13:9]=0;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_sub  = { nm="sub";  am="DAB"; p="D=A-B"; e="[15:14]=1;[13:9]=1;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_xor  = { nm="xor";  am="DAB"; p="D=A^B"; e="[15:14]=1;[13:9]=2;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_and  = { nm="and";  am="DAB"; p="D=A&B"; e="[15:14]=1;[13:9]=3;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_or   = { nm="or";   am="DAB"; p="D=A|B"; e="[15:14]=1;[13:9]=4;[8:6]=A;[5:3]=B;[2:0]=D"}

let i_not  = { nm="not";  am="DA"; p="D=!A";  e="[15:14]=1;[13:9]=5;[8:6]=A;[5:3]=0;[2:0]=D"}
let i_neg  = { nm="neg";  am="DA"; p="D=-A";  e="[15:14]=1;[13:9]=5;[8:6]=A;[5:3]=1;[2:0]=D"}
let i_inv  = { nm="inv";  am="DA"; p="D=~A";  e="[15:14]=1;[13:9]=5;[8:6]=A;[5:3]=2;[2:0]=D"}
let i_mov  = { nm="mov";  am="DA"; p="D=A";   e="[15:14]=1;[13:9]=5;[8:6]=A;[5:3]=3;[2:0]=D"}

let i_adc  = { nm="adc";  am="DAB"; p="D=A+B+c"; e="[15:14]=1;[13:9]=6;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_sbb  = { nm="sbb";  am="DAB"; p="D=A-B+c"; e="[15:14]=1;[13:9]=7;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_mul  = { nm="mul";  am="DAB"; p="D=A*B";  e="[15:14]=1;[13:9]=8;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_cmi  = { nm="cmi";  am="DAB"; p="D=(A-B)<0?1:0";  e="[15:14]=1;[13:9]=11;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_cpz  = { nm="cpz";  am="DAB"; p="D=(A-B)==0?1:0";  e="[15:14]=1;[13:9]=12;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_rol  = { nm="rol";  am="DA"; p="D=(A<<1)|(A>>15)";  e="[15:14]=1;[13:9]=13;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_lsr  = { nm="lsr";  am="DA"; p="D=(A>>1)|(D&32768)";  e="[15:14]=1;[13:9]=14;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_asr  = { nm="asr";  am="DA"; p="D=A>>1";  e="[15:14]=1;[13:9]=15;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_asl  = { nm="asl";  am="DA"; p="D=A<<1";  e="[15:14]=1;[13:9]=16;[8:6]=A;[5:3]=B;[2:0]=D"}
let i_ror  = { nm="ror";  am="DA"; p="D=(A>>1)|(A<<15)";  e="[15:14]=1;[13:9]=17;[8:6]=A;[5:3]=B;[2:0]=D"}




(* Load: 8000-BFFF. Store :C000-FFFF store *)
let i_lod  = { nm="lod";  am="RXb"; p="R=[X+b]";  e="[15:14]=2;[13]=0;[12:10]=R;[9:7]=X;[6:0]=b"}
let i_str  = { nm="str";  am="RXb"; p="[X+b]=R";  e="[15:14]=3;[13]=0;[12:10]=R;[9:7]=X;[6:0]=b"}

let i_lodb = { nm="lodb"; am="RXb"; p="R=m8[X+b]";  e="[15:14]=2;[13]=1;[12:10]=R;[9:7]=X;[6:0]=b"}
let i_strb = { nm="strb"; am="RXb"; p="m8[X+b]=R";  e="[15:14]=3;[13]=1;[12:10]=R;[9:7]=X;[6:0]=b"}

let instructions = [ i_jsr; i_jmp; i_brz; i_jmpr; i_brzr; i_jsrr; i_rts; i_stop; i_nop; i_disc;
i_rslt; i_push; i_pop; i_addi; i_subi; i_lodi; i_add; i_sub; i_xor; i_and; i_or;
i_neg; i_not; i_inv; i_mov; i_adc; i_sbb; i_mul; i_cmi; i_cpz; i_rol; i_lsr; i_asr; i_asl;
i_ror; i_lod; i_str; i_tas ]


let alu_codes = [
   (1, MC_add, "add", "a+b");
   (2, MC_sub, "sub", "a-b");
   (3, MC_xor, "xor", "a^b");
   (5, MC_and, "and", "a&b");
   (6, MC_or, "or", "a|b");
   (7, MC_not, "not", "!a");
   (8, MC_neg, "neg", "~a");
   (9, MC_mov, "mov", "a");
   (10, MC_adc, "adc", "a+b+c");
   (11, MC_sbb, "sbb", "a-b+c");
   (12, MC_cmi, "cmi", "(a-b<0)?1:0")
]

type hpr_isa_t = 
 {
   instructions: inst_t list;
   ins_width: int;
   data_width: int;
   tos: int;
   ngpregs: int;
   alu_codes: (int * mcode_alu_t * string * string) list;
   miscregs: (int * string) list;
   operand_forms: (string * string  * int * string * string) list;
   name :string
 }
;

let miscregs = [ (16, "pc"); (16, "sp") ]

let uisa_set:hpr_isa_t =
  { instructions= instructions;
    alu_codes=alu_codes;
    ins_width=16;
    data_width=16;
    tos=14 * 4096;
    ngpregs=8;
    miscregs= miscregs;
    operand_forms=operand_forms;
    name = "UISA_MICROCONTROLLER"
  }



(*
  An issue arises with the intermediate representation of any
  machine code - convert to stack code or three instruction code ?
  Three instruction virtual register code is easy to make 
  and stack code appears to have no advantages.

  

  Imperative to imperative transform - with essentially no
  changes to the data representation or volcabulary of 
  operations.

  It is easy to describe each opcode as an executable IMP sequence, ranging
  over registers and memory locations, but making this a suitable form for
  code generation/synthesis is the new challenge.
  
     The job amounts to finding a covering of the required imperative
   movements using the available instructions - but we need to constrain
 the search space...

 *)
(* eof *)
