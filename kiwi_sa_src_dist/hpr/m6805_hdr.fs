(*
 * $Id: m6805_hdr.fs,v 1.5 2013-04-21 07:51:26 djg11 Exp $
 * microcode_hdr.sml
 *
 *
 * Contains definitions of the machine code for a microcontroller.
 *)
module m6805_hdr


open microcode_hdr
open hprls_hdr





let operand_forms6805 = [
  ("L", "label",     12, "", "");
  ("R", "register",  3,  "", "");
  ("x", "indexed",  3,  "", "");
  ("I", "monobye",  3,  "", "");
  ("E", "dibyte",  3,  "", "");
  ("D", "direct",  3,  "", "");
  ("i", "immediate", 9,  "", "")
 ]



// Flag bits: H I N Z C


(* Conditional branch rnz : 2000-3FFF *)
let i_bra  = { nm="bra";  am="RL";    p="pc=L";          e="[7:0]=0x20"}
let i_brn  = { nm="brn";  am="RL";    p="pc=pc";         e="[7:0]=0x21"}
let i_bhi  = { nm="bhi";  am="RL";    p="pc=(cc&3)==0?pc:L";     e="[7:0]=0x22"} // Not borrow and not zero
let i_bls  = { nm="bls";  am="RL";    p="pc=(cc&3)!=0?pc:L";     e="[7:0]=0x23"} // Borrow or zero
let i_bcc  = { nm="bcc";  am="RL";    p="pc=(cc&1)==0?pc:L";     e="[7:0]=0x24"}
//t i_bhs  = { nm="bhs";  am="RL";    p="pc=(cc&1)==0?pc:L";     e="[7:0]=0x24"}
let i_bcs  = { nm="bcs";  am="RL";    p="pc=(cc&1)==1?pc:L";     e="[7:0]=0x25"}
//t i_blo  = { nm="blo";  am="RL";    p="pc=(cc&1)==1?pc:L";     e="[7:0]=0x25"}
let i_bne  = { nm="bne";  am="RL";    p="pc=(cc&2)==0?pc:L";     e="[7:0]=0x26"}
let i_beq  = { nm="beq";  am="RL";    p="pc=(cc&2)==2?pc:L";     e="[7:0]=0x27"}
let i_bhcc = { nm="bhcc"; am="RL";    p="pc=(cc&16)==16?pc:L";     e="[7:0]=0x28"}
let i_bhcs = { nm="bhcs"; am="RL";    p="pc=(cc&16)==0?pc:L";     e="[7:0]=0x29"}
let i_bpl  = { nm="bpl";  am="RL";    p="pc=(cc&4)==0?pc:L";     e="[7:0]=0x2a"}
let i_bmi  = { nm="bmi";  am="RL";    p="pc=(cc&4)==4?pc:L";     e="[7:0]=0x2b"}
let i_bmc  = { nm="bmc";  am="RL";    p="pc=(cc&8)==0?pc:L";     e="[7:0]=0x2c"}
let i_bms  = { nm="bms";  am="RL";    p="pc=(cc&8)==8?pc:L";     e="[7:0]=0x2d"}
//t i_bil  = { nm="bil";  am="RL";    p="pc=R?pc:L";     e="[7:0]=0x2e"}
//t i_bih  = { nm="bih";  am="RL";    p="pc=R?pc:L";     e="[7:0]=0x2f"}
let i_bsr  = { nm="bsr";  am="RL";    p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=L";     e="[7:0]=0xad"}


let i_brset  = { nm="brset";  am="n3,RL"; p="pc=[D]&(1<<N)?L:pc";      e="[7:0]=0;e[3:1]=N;e[0]=0;" }
let i_brclr  = { nm="brclr";  am="n3,RL"; p="pc=[D]&(1<<N)?pc:L";      e="[7:0]=0;e[3:1]=N;e[0]=1;" }

(* Miscellaneous instructions: 0-*)
let i_tax  = { nm="tax";  am="0"; p="x=a";                         e="[7:0]=0x97" }
let i_txa  = { nm="txa";  am="0"; p="a=x";                         e="[7:0]=0x9F" }
let i_sec  = { nm="sec";  am="0"; p="cc=cc|1";                     e="[7:0]=0x99" }
let i_clc  = { nm="clc";  am="0"; p="cc=cc&~1";                    e="[7:0]=0x98" }
let i_sei  = { nm="sei";  am="0"; p="cc=cc|16";                    e="[7:0]=0x9b" }
let i_cli  = { nm="cli";  am="0"; p="cc=cc&~16";                   e="[7:0]=0x9a" }
let i_swi  = { nm="swi";  am="0"; p="pc=1";                        e="[7:0]=0x83" }
let i_rts  = { nm="rts";  am="0"; p="sp=sp+2;pc=[sp]+[sp-1]<<8";   e="[7:0]=0x81" }
let i_rti  = { nm="rti";  am="0"; p="sp=sp+2;pc=[sp]+[sp-1]<<8;cc=cc&~8";   e="[7:0]=0x80" }
let i_rsp  = { nm="rsp";  am="0"; p="sp=0xc0";                     e="[7:0]=0x9c" }
let i_nop  = { nm="nop";  am="0"; p="";                            e="[7:0]=0x9d" }

let i_bset  = { nm="bset";  am="n3,D"; p="c:[D]=[D]|(1<<N)";       e="[7:4]=1;e[3:1]=N;e[0]=0;" }
let i_clr  = { nm="bclr";  am="n3,D"; p="c:[D]=[D]&~(1<<N)";      e="[7:4]=1;e[3:1]=N;e[0]=1;" }




// Bit instructions 
let i_lda_i  = { nm="lda";  am="I"; p="a=i";     e="[7:0]=0xa6"}  // immediate load
let i_lda_d  = { nm="lda";  am="D"; p="a=[D]";   e="[7:0]=0xb6"}  // direct
let i_lda_e  = { nm="lda";  am="E"; p="a=[E]";   e="[7:0]=0xc6"}  // extended
let i_lda_x0 = { nm="lda";  am="x"; p="a=[x]";   e="[7:0]=0xf6"}  // indexed no offset
let i_lda_x8 = { nm="lda";  am="xI"; p="a=[x+I]"; e="[7:0]=0xe6"}  // indexed8
let i_lda_x16= { nm="lda";  am="xE"; p="a=[x+E]"; e="[7:0]=0xd6"}  // indexed16


let i_ldx_i  = { nm="ldx";  am="I"; p="x=i";     e="[7:0]=0xae"}  // immediate load to x
let i_ldx_d  = { nm="ldx";  am="D"; p="x=[D]";   e="[7:0]=0xbe"}  // direct
let i_ldx_e  = { nm="ldx";  am="E"; p="x=[E]";   e="[7:0]=0xce"}  // extended
let i_ldx_x0 = { nm="ldx";  am="x"; p="x=[x]";   e="[7:0]=0xfe"}  // indexed no offset
let i_ldx_x8 = { nm="ldx";  am="xI"; p="x=[x+I]"; e="[7:0]=0xee"}  // indexed8
let i_ldx_x16 = { nm="ldx";  am="xE"; p="x=[x+E]"; e="[7:0]=0xde"}  // indexed16


let i_sta_d  = { nm="sta";  am="D"; p="[D]=a";   e="[7:0]=0xb7"}  // direct store
let i_sta_e  = { nm="sta";  am="E"; p="[E]=a";   e="[7:0]=0xc7"}  // extended
let i_sta_x0 = { nm="sta";  am="x"; p="[x]=a";   e="[7:0]=0xf7"}  // indexed no offset
let i_sta_x8 = { nm="sta";  am="xI"; p="[x+I]=a"; e="[7:0]=0xe7"}  // indexed8
let i_sta_x16= { nm="sta";  am="xE"; p="[x+E]=a"; e="[7:0]=0xd7"}  // indexed16

let i_stx_d  = { nm="stx";  am="D"; p="[D]=x";   e="[7:0]=0xbf"}  // direct store
let i_stx_e  = { nm="stx";  am="E"; p="[E]=x";   e="[7:0]=0xcf"}  // extended
let i_stx_x0 = { nm="stx";  am="x"; p="[x]=x";   e="[7:0]=0xff"}  // indexed no offset
let i_stx_x8 = { nm="stx";  am="I"; p="[x+I]=x"; e="[7:0]=0xef"}  // indexed8
let i_stx_x16= { nm="stx";  am="xE"; p="[x+E]=x"; e="[7:0]=0xdf"}  // indexed16

let i_add_i  = { nm="add";  am="I"; p="zncv:a=a+i";     e="[7:0]=0xab"}  // immediate add
let i_add_e  = { nm="add";  am="D"; p="zncv:a=a+[D]";   e="[7:0]=0xbb"}  // direct
let i_add_d  = { nm="add";  am="E"; p="zncv:a=a+[E]";   e="[7:0]=0xcb"}  // extended
let i_add_x0 = { nm="add";  am="x"; p="zncv:a=a+[x]";   e="[7:0]=0xfb"}  // indexed no offset
let i_add_x8 = { nm="add";  am="xI"; p="zncv:a=a+[x+I]"; e="[7:0]=0xeb"}  // indexed8
let i_add_x16= { nm="add";  am="xE"; p="zncv:a=a+[x+E]"; e="[7:0]=0xdb"}  // indexed16

let i_adc_i  = { nm="adc";  am="I"; p="zncv:a=(cc&1)+a+i";     e="[7:0]=0xa9"}  // immediate add with carry
let i_adc_e  = { nm="adc";  am="D"; p="zncv:a=(cc&1)+a+[D]";   e="[7:0]=0xb9"}  // direct
let i_adc_d  = { nm="adc";  am="E"; p="zncv:a=(cc&1)+a+[E]";   e="[7:0]=0xc9"}  // extended
let i_adc_x0 = { nm="adc";  am="x"; p="zncv:a=(cc&1)+a+[x]";   e="[7:0]=0xf9"}  // indexed no offset
let i_adc_x8 = { nm="adc";  am="xI"; p="zncv:a=(cc&1)+a+[x+I]"; e="[7:0]=0xe9"}  // indexed8
let i_adc_x16= { nm="adc";  am="xE"; p="zncv:a=(cc&1)+a+[x+E]"; e="[7:0]=0xd9"}  // indexed16

let i_sub_i  = { nm="sub";  am="I"; p="zncv:a=a-i";     e="[7:0]=0xa0"}  // immediate sub
let i_sub_e  = { nm="sub";  am="D"; p="zncv:a=a-[D]";   e="[7:0]=0xb0"}  // direct
let i_sub_d  = { nm="sub";  am="E"; p="zncv:a=a-[E]";   e="[7:0]=0xc0"}  // extended
let i_sub_x0 = { nm="sub";  am="x"; p="zncv:a=a-[x]";   e="[7:0]=0xf0"}  // indexed no offset
let i_sub_x8 = { nm="sub";  am="xI"; p="zncv:a=a-[x+I]"; e="[7:0]=0xe0"}  // indexed8
let i_sub_x16= { nm="sub";  am="xE"; p="zncv:a=a-[x+E]"; e="[7:0]=0xd0"}  // indexed16

let i_sbc_i  = { nm="sbc";  am="I"; p="zncv:a=a-(cc&1)-i";     e="[7:0]=0xa0"}  // immediate sbc
let i_sbc_e  = { nm="sbc";  am="D"; p="zncv:a=a-(cc&1)-[D]";   e="[7:0]=0xb0"}  // direct
let i_sbc_d  = { nm="sbc";  am="E"; p="zncv:a=a-(cc&1)-[E]";   e="[7:0]=0xc0"}  // extended
let i_sbc_x0 = { nm="sbc";  am="x"; p="zncv:a=a-(cc&1)-[x]";   e="[7:0]=0xf0"}  // indexed no offset
let i_sbc_x8 = { nm="sbc";  am="xI"; p="zncv:a=a-(cc&1)-[x+I]"; e="[7:0]=0xe0"}  // indexed8
let i_sbc_x16= { nm="sbc";  am="xE"; p="zncv:a=a-(cc&1)-[x+E]"; e="[7:0]=0xd0"}  // indexed16


let i_and_i  = { nm="and";  am="I"; p="zncv:a=a&i";     e="[7:0]=0xa4"}  // immediate and
let i_and_e  = { nm="and";  am="D"; p="zncv:a=a&[D]";   e="[7:0]=0xb4"}  // direct
let i_and_d  = { nm="and";  am="E"; p="zncv:a=a&[E]";   e="[7:0]=0xc4"}  // extended
let i_and_x0 = { nm="and";  am="x"; p="zncv:a=a&[x]";   e="[7:0]=0xf4"}  // indexed no offset
let i_and_x8 = { nm="and";  am="xI"; p="zncv:a=a&[x+I]"; e="[7:0]=0xe4"}  // indexed8
let i_and_x16= { nm="and";  am="xE"; p="zncv:a=a&[x+E]"; e="[7:0]=0xd4"}  // indexed16


let i_ora_i  = { nm="ora";  am="I"; p="zncv:a=a|i";     e="[7:0]=0xaa"}  // immediate ora
let i_ora_e  = { nm="ora";  am="D"; p="zncv:a=a|[D]";   e="[7:0]=0xba"}  // direct
let i_ora_d  = { nm="ora";  am="E"; p="zncv:a=a|[E]";   e="[7:0]=0xca"}  // extended
let i_ora_x0 = { nm="ora";  am="x"; p="zncv:a=a|[x]";   e="[7:0]=0xfa"}  // indexed no offset
let i_ora_x8 = { nm="ora";  am="xI"; p="zncv:a=a|[x+I]"; e="[7:0]=0xea"}  // indexed8
let i_ora_x16= { nm="ora";  am="xE"; p="zncv:a=a|[x+E]"; e="[7:0]=0xda"}  // indexed16

let i_eor_i  = { nm="eor";  am="I"; p="zncv:a=a^i";     e="[7:0]=0xa8"}  // immediate eor
let i_eor_e  = { nm="eor";  am="D"; p="zncv:a=a^[D]";   e="[7:0]=0xb8"}  // direct
let i_eor_d  = { nm="eor";  am="E"; p="zncv:a=a^[E]";   e="[7:0]=0xc8"}  // extended
let i_eor_x0 = { nm="eor";  am="x"; p="zncv:a=a^[x]";   e="[7:0]=0xf8"}  // indexed no offset
let i_eor_x8 = { nm="eor";  am="xI"; p="zncv:a=a^[x+I]"; e="[7:0]=0xe8"}  // indexed8
let i_eor_x16= { nm="eor";  am="xE"; p="zncv:a=a^[x+E]"; e="[7:0]=0xd8"}  // indexed16

let i_cmp_i  = { nm="cmp";  am="I"; p="zncv:a-i";     e="[7:0]=0xa1"}  // immediate cmp
let i_cmp_e  = { nm="cmp";  am="D"; p="zncv:a-[D]";   e="[7:0]=0xb1"}  // direct
let i_cmp_d  = { nm="cmp";  am="E"; p="zncv:a-[E]";   e="[7:0]=0xc1"}  // extended
let i_cmp_x0 = { nm="cmp";  am="x"; p="zncv:a-[x]";   e="[7:0]=0xf1"}  // indexed no offset
let i_cmp_x8 = { nm="cmp";  am="xI"; p="zncv:a-[x+I]"; e="[7:0]=0xe1"}  // indexed8
let i_cmp_x16= { nm="cmp";  am="xE"; p="zncv:a-[x+E]"; e="[7:0]=0xd1"}  // indexed16

let i_cpx_i  = { nm="cpx";  am="I"; p="zncv:x-i";     e="[7:0]=0xa3"}  // immediate cpx
let i_cpx_e  = { nm="cpx";  am="D"; p="zncv:x-[D]";   e="[7:0]=0xb3"}  // direct
let i_cpx_d  = { nm="cpx";  am="E"; p="zncv:x-[E]";   e="[7:0]=0xc3"}  // extended
let i_cpx_x0 = { nm="cpx";  am="x"; p="zncv:x-[x]";   e="[7:0]=0xf3"}  // indexed no offset
let i_cpx_x8 = { nm="cpx";  am="xI"; p="zncv:x-[x+I]"; e="[7:0]=0xe3"}  // indexed8
let i_cpx_x16= { nm="cpx";  am="xE"; p="zncv:x-[x+E]"; e="[7:0]=0xd3"}  // indexed16

let i_bit_i  = { nm="bit";  am="I"; p="zncv:a&i";     e="[7:0]=0xa5"}  // immediate bit
let i_bit_e  = { nm="bit";  am="D"; p="zncv:a&[D]";   e="[7:0]=0xb5"}  // direct
let i_bit_d  = { nm="bit";  am="E"; p="zncv:a&[E]";   e="[7:0]=0xc5"}  // extended
let i_bit_x0 = { nm="bit";  am="x"; p="zncv:a&[x]";   e="[7:0]=0xf5"}  // indexed no offset
let i_bit_x8 = { nm="bit";  am="xI"; p="zncv:a&[x+I]"; e="[7:0]=0xe5"}  // indexed8
let i_bit_x16= { nm="bit";  am="xE"; p="zncv:a&[x+E]"; e="[7:0]=0xd5"}  // indexed16

let i_jmp_e  = { nm="jmp";  am="D"; p="pc=[D]";   e="[7:0]=0xbc"}  // direct
let i_jmp_d  = { nm="jmp";  am="E"; p="pc=[E]";   e="[7:0]=0xcc"}  // extended
let i_jmp_x0 = { nm="jmp";  am="x"; p="pc=[x]";   e="[7:0]=0xfc"}  // indexed no offset
let i_jmp_x8 = { nm="jmp";  am="I"; p="pc=[x+I]"; e="[7:0]=0xec"}  // indexed8
let i_jmp_x16= { nm="jmp";  am="xE"; p="pc=[x+E]"; e="[7:0]=0xdc"}  // indexed16

let i_jsr_e  = { nm="jsr";  am="D"; p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=[D]";   e="[7:0]=0xbd"}  // direct
let i_jsr_d  = { nm="jsr";  am="E"; p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=[E]";   e="[7:0]=0xcd"}  // extended
let i_jsr_x0 = { nm="jsr";  am="x"; p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=[x]";   e="[7:0]=0xfd"}  // indexed no offset
let i_jsr_x8 = { nm="jsr";  am="xI"; p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=[x+I]"; e="[7:0]=0xed"}  // indexed8
let i_jsr_x16= { nm="jsr";  am="xE"; p="[sp]=pc;[sp-1]=pc>>8;sp=sp-2;pc=[x+E]"; e="[7:0]=0xdd"}  // indexed16


let i_inc_a   = { nm="inca";  am="0";  p="zncv:a=a+1";                  e="[7:0]=0x4c"}  // inherent a
let i_inc_x   = { nm="incx";  am="0";  p="zncv:x=x+1";                  e="[7:0]=0x5c"}  // inherent x
let i_inc_d   = { nm="inc";  am="D";  p="zncv:[x]=[x]+1";              e="[7:0]=0x3c"}  // direct
let i_inc_x0  = { nm="inc";  am="xI"; p="zncv:[x+I]=[x+I]+1";          e="[7:0]=0x7c"}  // indexed
let i_inc_x8  = { nm="inc";  am="xE"; p="zncv:[x+E]=[x+E]+1";          e="[7:0]=0x6c"}  // indexed8

let i_dec_a   = { nm="deca";  am="0";  p="zncv:a=a-1";                  e="[7:0]=0x4a"}  // inherent a
let i_dec_x   = { nm="decx";  am="0";  p="zncv:x=x-1";                  e="[7:0]=0x5a"}  // inherent x
let i_dec_d   = { nm="dec";  am="D";  p="zncv:[x]=[x]-1";              e="[7:0]=0x3a"}  // direct
let i_dec_x0  = { nm="dec";  am="xI"; p="zncv:[x+I]=[x+I]-1";          e="[7:0]=0x7a"}  // indexed
let i_dec_x8  = { nm="dec";  am="xE"; p="zncv:[x+E]=[x+E]-1";          e="[7:0]=0x6a"}  // indexed8

let i_clr_a   = { nm="clra";  am="0";  p="zncv:0    ";                  e="[7:0]=0x4f"}  // inherent a
let i_clr_x   = { nm="clrx";  am="0";  p="zncv:0    ";                  e="[7:0]=0x5f"}  // inherent x
let i_clr_d   = { nm="clr";  am="D";  p="zncv:[x]=0    ";              e="[7:0]=0x3f"}  // direct
let i_clr_x0  = { nm="clr";  am="xI"; p="zncv:[x+I]=0      ";          e="[7:0]=0x7f"}  // indexed
let i_clr_x8  = { nm="clr";  am="xE"; p="zncv:[x+E]=0      ";          e="[7:0]=0x6f"}  // indexed8

let i_com_a   = { nm="coma";  am="0";  p="zncv:a=~a";                  e="[7:0]=0x43"}  // inherent a
let i_com_x   = { nm="comx";  am="0";  p="zncv:x=~x";                  e="[7:0]=0x53"}  // inherent x
let i_com_d   = { nm="com";  am="D";  p="zncv:[x]=~[x]";              e="[7:0]=0x33"}  // direct
let i_com_x0  = { nm="com";  am="xI"; p="zncv:[x+I]=~[x+I]";          e="[7:0]=0x73"}  // indexed
let i_com_x8  = { nm="com";  am="xE"; p="zncv:[x+E]=~[x+E]";          e="[7:0]=0x63"}  // indexed8

let i_neg_a   = { nm="nega";  am="0";  p="zncv:a=0-a";                  e="[7:0]=0x40"}  // inherent a
let i_neg_x   = { nm="negx";  am="0";  p="zncv:x=0-x";                  e="[7:0]=0x50"}  // inherent x
let i_neg_d   = { nm="neg";  am="D";  p="zncv:[x]=0-[x]";              e="[7:0]=0x30"}  // direct
let i_neg_x0  = { nm="neg";  am="xI"; p="zncv:[x+I]=0-[x+I]";          e="[7:0]=0x70"}  // indexed
let i_neg_x8  = { nm="neg";  am="xE"; p="zncv:[x+E]=0-[x+E]";          e="[7:0]=0x60"}  // indexed8

let i_rol_a   = { nm="rola";  am="0";  p="a=(a<<1)|(cc&1);cc=(cc&~1)|((a>>7)&1)";  e="[7:0]=0x49"}  // inherent a
let i_rol_x   = { nm="rolx";  am="0";  p="x=(x<<1)|(cc&1);cc=(cc&~1)|((x>>7)&1)";  e="[7:0]=0x59"}  // inherent x
let i_rol_d   = { nm="rol";  am="D";  p="[x]=([x]<<1)|(cc&1);cc=(cc&~1)|(([x]>>7)&1)";  e="[7:0]=0x39"}  // direct
let i_rol_x0  = { nm="rol";  am="xI"; p="[x+I]=([x+I]<<1)|(cc&1);cc=(cc&~1)|(([x+I]>>7)&1)";  e="[7:0]=0x79"}  // indexed
let i_rol_x8  = { nm="rol";  am="xE"; p="[x+E]=([x+E]<<1)|(cc&1);cc=(cc&~1)|(([x+E]>>7)&1)";  e="[7:0]=0x69"}  // indexed8

let i_ror_a   = { nm="rora";  am="0";  p="a=(a>>1)|((cc<7)&128);cc=(cc&~1)|((a)&1)";              e="[7:0]=0x46"}  // inherent a
let i_ror_x   = { nm="rorx";  am="0";  p="x=(x>>1)|((cc<7)&128);cc=(cc&~1)|((x)&1)";              e="[7:0]=0x56"}  // inherent x
let i_ror_d   = { nm="ror";  am="D";  p="[x]=([x]>>1)|((cc<7)&128);cc=(cc&~1)|(([x])&1)";        e="[7:0]=0x36"}  // direct
let i_ror_x0  = { nm="ror";  am="xI"; p="[x+I]=([x+I]>>1)|((cc<7)&128);cc=(cc&~1)|(([x+I])&1)";  e="[7:0]=0x76"}  // indexed
let i_ror_x8  = { nm="ror";  am="xE"; p="[x+E]=([x+E]>>1)|((cc<7)&128);cc=(cc&~1)|(([x+E])&1)";  e="[7:0]=0x66"}  // indexed8

let i_lsl_a   = { nm="lsla";  am="0";  p="a=(a<<1)";          e="[7:0]=0x48"}  // inherent a
let i_lsl_x   = { nm="lslx";  am="0";  p="x=(x<<1)";          e="[7:0]=0x58"}  // inherent x
let i_lsl_d   = { nm="lsl";  am="D";  p="[x]=[x]";           e="[7:0]=0x38"}  // direct
let i_lsl_x0  = { nm="lsl";  am="xI"; p="[x+I]=([x+I]<<1)";  e="[7:0]=0x78"}  // indexed
let i_lsl_x8  = { nm="lsl";  am="xE"; p="[x+E]=([x+E]<<1)";  e="[7:0]=0x68"}  // indexed8

let i_lsr_a   = { nm="lsra";  am="0";  p="a=(a<<1)";          e="[7:0]=0x44"}  // inherent a
let i_lsr_x   = { nm="lsrx";  am="0";  p="x=(x<<1)";          e="[7:0]=0x54"}  // inherent x
let i_lsr_d   = { nm="lsr";  am="D";  p="[x]=[x]";           e="[7:0]=0x34"}  // direct
let i_lsr_x0  = { nm="lsr";  am="xI"; p="[x+I]=([x+I]<<1)";  e="[7:0]=0x74"}  // indexed
let i_lsr_x8  = { nm="lsr";  am="xE"; p="[x+E]=([x+E]<<1)";  e="[7:0]=0x64"}  // indexed8



let i_asr_a   = { nm="asra";  am="0";  p="a=(a&128)|(a>>1)";               e="[7:0]=0x47"}  // inherent a
let i_asr_x   = { nm="asrx";  am="0";  p="x=(x&128)|(x>>1)";               e="[7:0]=0x57"}  // inherent x
let i_asr_d   = { nm="asr";  am="D";  p="[x]=([x]&128)|[x]";              e="[7:0]=0x37"}  // direct
let i_asr_x0  = { nm="asr";  am="xI"; p="[x+I]==([x+I]&128)|([x+I]>>1)";  e="[7:0]=0x77"}  // indexed
let i_asr_x8  = { nm="asr";  am="xE"; p="[x+E]==([x+E]&128)|([x+E]>>1)";  e="[7:0]=0x67"}  // indexed8

let i_tst_a   = { nm="tsta";  am="0";  p="zncv:a";          e="[7:0]=0x4d"}  // inherent a
let i_tst_x   = { nm="tstx";  am="0";  p="zncv:x";          e="[7:0]=0x5d"}  // inherent x
let i_tst_d   = { nm="tst";  am="D";  p="zncv:[x]";           e="[7:0]=0x3d"}  // direct
let i_tst_x0  = { nm="tst";  am="xI"; p="zncv:[x+I]";  e="[7:0]=0x7d"}  // indexed
let i_tst_x8  = { nm="tst";  am="xE"; p="zncv:[x+E]";  e="[7:0]=0x6d"}  // indexed8




let instructions =

    [ i_bra; i_brn; i_bhi; i_bls; i_bcc; (* i_bhs; *) i_bcs;
      (* i_blo; *) i_bne; i_beq; i_bhcc; i_bhcs; i_bpl; i_bmi; i_bmc; i_bms;
      (* i_bil; i_bih; *) i_bsr;
       // i_brset; i_brclr; i_bset; i_clr;
      i_tax; i_txa; i_sec; i_clc;
      i_sei; i_cli; i_swi; i_rts; i_rti; i_rsp; i_nop; 
      i_lda_i; i_lda_d; i_lda_e; i_lda_x0; i_lda_x8; i_lda_x16; i_ldx_i;
      i_ldx_d; i_ldx_e; i_ldx_x0; i_ldx_x8; i_ldx_x16; i_sta_d; i_sta_e;
      i_sta_x0; i_sta_x8; i_sta_x16; i_stx_d; i_stx_e; i_stx_x0; i_stx_x8;
      i_stx_x16; i_add_i; i_add_e; i_add_d; i_add_x0; i_add_x8; i_add_x16;
      i_adc_i; i_adc_e; i_adc_d; i_adc_x0; i_adc_x8; i_adc_x16; i_sub_i;
      i_sub_e; i_sub_d; i_sub_x0; i_sub_x8; i_sub_x16; i_sbc_i; i_sbc_e;
      i_sbc_d; i_sbc_x0; i_sbc_x8; i_sbc_x16; i_and_i; i_and_e; i_and_d;
      i_and_x0; i_and_x8; i_and_x16; i_ora_i; i_ora_e; i_ora_d; i_ora_x0;
      i_ora_x8; i_ora_x16; i_eor_i; i_eor_e; i_eor_d; i_eor_x0; i_eor_x8;
      i_eor_x16; i_cmp_i; i_cmp_e; i_cmp_d; i_cmp_x0; i_cmp_x8; i_cmp_x16;
      i_cpx_i; i_cpx_e; i_cpx_d; i_cpx_x0; i_cpx_x8; i_cpx_x16; i_bit_i;
      i_bit_e; i_bit_d; i_bit_x0; i_bit_x8; i_bit_x16; i_jmp_e; i_jmp_d;
      i_jmp_x0; i_jmp_x8; i_jmp_x16; i_jsr_e; i_jsr_d; i_jsr_x0; i_jsr_x8;
      i_jsr_x16; i_inc_a; i_inc_x; i_inc_d; i_inc_x0; i_inc_x8; i_dec_a;
      i_dec_x; i_dec_d; i_dec_x0; i_dec_x8; i_clr_a; i_clr_x; i_clr_d;
      i_clr_x0; i_clr_x8; i_com_a; i_com_x; i_com_d; i_com_x0; i_com_x8;
      i_neg_a; i_neg_x; i_neg_d; i_neg_x0; i_neg_x8; i_rol_a; i_rol_x;
      i_rol_d; i_rol_x0; i_rol_x8; i_ror_a; i_ror_x; i_ror_d; i_ror_x0;
      i_ror_x8;
      i_lsl_a; i_lsl_x; i_lsl_d; i_lsl_x0; i_lsl_x8;
      i_lsr_a; i_lsr_x; i_lsr_d; i_lsr_x0; i_lsr_x8;
      i_asr_a; i_asr_x; i_asr_d; i_asr_x0; i_asr_x8;
      i_tst_a; i_tst_x; i_tst_d; i_tst_x0; i_tst_x8;
    ]


let alu_codes = [
                ]

let miscregs = [ (13, "pc"); (6, "sp"); (5, "cc"); (8, "a"); (8, "x"); ]

let m6805_set:hpr_isa_t =
  { instructions= instructions;
    alu_codes=alu_codes;
    ins_width=8;
    data_width=8;
    tos=14 * 4096;
    ngpregs=0;
    miscregs= miscregs;
    operand_forms=operand_forms6805;
    name = "M6805"
  }


(* eof *)
