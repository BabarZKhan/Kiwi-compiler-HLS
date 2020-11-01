

// CBG Orangepath HPR L/S System

// Verilog output file generated at 14/08/2016 14:19:37
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.15p : 11th-August-2016 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-kcode-dump=enable -kiwic-cil-dump=separately test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b2.v -vnl-resets=synchronous -sim 3000 -bevelab-default-pause-mode=hard -res2-reprint=enable -res2-regfile-threshold=4 -res2-combram-threshold=20 -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    input din,
    output dout,
    output hf1_dram0bank_opreq,
    input hf1_dram0bank_oprdy,
    input hf1_dram0bank_ack,
    output hf1_dram0bank_rwbar,
    output [255:0] hf1_dram0bank_wdata,
    output [21:0] hf1_dram0bank_addr,
    input [255:0] hf1_dram0bank_rdata,
    output [31:0] hf1_dram0bank_lanes,
    input clk,
    input reset);
function signed [31:0] rtl_signed_bitextract0;
   input [31:0] arg;
   rtl_signed_bitextract0 = $signed(arg[31:0]);
   endfunction

  integer Ttar0_3_V_0;
  integer Ttar0_3_V_1;
  integer Ttar0_3_V_3;
  integer Ttar0_3_V_5;
  integer Ttar0_3_V_6;
  wire signed [31:0] A_SINT_CC_SCALbx10_ARA0_RDD0;
  reg [3:0] A_SINT_CC_SCALbx10_ARA0_AD0;
  reg A_SINT_CC_SCALbx10_ARA0_WEN0;
  reg A_SINT_CC_SCALbx10_ARA0_REN0;
  reg signed [31:0] A_SINT_CC_SCALbx10_ARA0_WRD0;
  reg signed [31:0] SINTCCSCALbx10ARA0RRh10hold;
  reg SINTCCSCALbx10ARA0RRh10shot0;
  reg [3:0] xpc10nz;
 always   @(* )  begin 
       A_SINT_CC_SCALbx10_ARA0_AD0 = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_REN0 = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_WEN0 = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_WEN0 = ((xpc10nz==4'sd1/*1:xpc10nz*/) || (xpc10nz==4'sd3/*3:xpc10nz*/) || (xpc10nz==4'sd5/*5:xpc10nz*/) || 
      (xpc10nz==4'sd7/*7:xpc10nz*/) || (xpc10nz==4'sd8/*8:xpc10nz*/) || (xpc10nz==4'sd6/*6:xpc10nz*/) || (xpc10nz==4'sd4/*4:xpc10nz*/) || 
      (xpc10nz==4'sd2/*2:xpc10nz*/) || (xpc10nz==4'sd0/*0:xpc10nz*/)? 1'd1: 1'd0);

       A_SINT_CC_SCALbx10_ARA0_REN0 = (32'sd1+Ttar0_3_V_5<32'sh9) && (xpc10nz==4'sd10/*10:xpc10nz*/);

      case (xpc10nz)
          4'sd0/*0:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd2;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd2;
               end 
              
          4'sd1/*1:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd2048;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd8;
               end 
              
          4'sd2/*2:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd2021;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd7;
               end 
              
          4'sd3/*3:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd1121;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd6;
               end 
              
          4'sd4/*4:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd8;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd5;
               end 
              
          4'sd5/*5:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd7;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd4;
               end 
              
          4'sd6/*6:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd5;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd3;
               end 
              
          4'sd7/*7:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd1;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd1;
               end 
              
          4'sd8/*8:xpc10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_WRD0 = 32'sd0;
               A_SINT_CC_SCALbx10_ARA0_AD0 = 4'd0;
               end 
              endcase
      if ((32'sd1+Ttar0_3_V_5<32'sh9))  begin if ((xpc10nz==4'sd10/*10:xpc10nz*/))  A_SINT_CC_SCALbx10_ARA0_AD0 = 32'sd1+Ttar0_3_V_5;
               end 
           end 
      

 always   @(posedge clk )  begin 
      //Start structure HPR test4.exe
      if (reset)  begin 
               Ttar0_3_V_0 <= 32'd0;
               Ttar0_3_V_3 <= 32'd0;
               Ttar0_3_V_5 <= 32'd0;
               Ttar0_3_V_6 <= 32'd0;
               Ttar0_3_V_1 <= 32'd0;
               SINTCCSCALbx10ARA0RRh10hold <= 32'd0;
               SINTCCSCALbx10ARA0RRh10shot0 <= 1'd0;
               xpc10nz <= 4'd0;
               end 
               else  begin 
              
              case (xpc10nz)
                  4'sd11/*11:xpc10nz*/:  begin 
                      if ((32'sd1+Ttar0_3_V_5<32'sh9) && !(rtl_signed_bitextract0(((xpc10nz==4'sd11/*11:xpc10nz*/)? A_SINT_CC_SCALbx10_ARA0_RDD0
                      : SINTCCSCALbx10ARA0RRh10hold))%32'sd2))  begin 
                              $write("%1d vale=%1d: ", rtl_signed_bitextract0(Ttar0_3_V_3), Ttar0_3_V_6);
                              $display("so far %1d Odd Numbers, and %1d Even Numbers.", Ttar0_3_V_0, Ttar0_3_V_1);
                               end 
                              if ((32'sd1+Ttar0_3_V_5<32'sh9) && !(!(rtl_signed_bitextract0(((xpc10nz==4'sd11/*11:xpc10nz*/)? A_SINT_CC_SCALbx10_ARA0_RDD0
                      : SINTCCSCALbx10ARA0RRh10hold))%32'sd2)))  begin 
                              $write("%1d vale=%1d: ", rtl_signed_bitextract0(Ttar0_3_V_3), Ttar0_3_V_6);
                              $display("so far %1d Odd Numbers, and %1d Even Numbers.", Ttar0_3_V_0, Ttar0_3_V_1);
                               end 
                               end 
                      
                  4'sd12/*12:xpc10nz*/: $finish(32'sd0);
              endcase
              if ((32'sd1+Ttar0_3_V_5>=32'sh9))  begin if ((xpc10nz==4'sd10/*10:xpc10nz*/))  begin 
                          $write("%1d vale=%1d: ", rtl_signed_bitextract0(Ttar0_3_V_3), Ttar0_3_V_6);
                          $display("so far %1d Odd Numbers, and %1d Even Numbers.", Ttar0_3_V_0, Ttar0_3_V_1);
                          $display("Found %1d Odd Numbers, and %1d Even Numbers.", Ttar0_3_V_0, Ttar0_3_V_1);
                           end 
                           end 
                   else 
                  case (xpc10nz)
                      4'sd10/*10:xpc10nz*/:  xpc10nz <= 4'sd11/*11:xpc10nz*/;

                      4'sd11/*11:xpc10nz*/:  begin 
                           Ttar0_3_V_3 <= 32'sd1+rtl_signed_bitextract0(Ttar0_3_V_3);
                           Ttar0_3_V_5 <= 32'sd1+Ttar0_3_V_5;
                           Ttar0_3_V_6 <= rtl_signed_bitextract0(((xpc10nz==4'sd11/*11:xpc10nz*/)? A_SINT_CC_SCALbx10_ARA0_RDD0: SINTCCSCALbx10ARA0RRh10hold
                          ));

                           end 
                          endcase
              if ((32'sd1+Ttar0_3_V_5>=32'sh9) && (xpc10nz==4'sd10/*10:xpc10nz*/))  begin 
                       Ttar0_3_V_3 <= 32'sd1+rtl_signed_bitextract0(Ttar0_3_V_3);
                       Ttar0_3_V_5 <= 32'sd1+Ttar0_3_V_5;
                       xpc10nz <= 4'sd12/*12:xpc10nz*/;
                       end 
                      
              case (xpc10nz)
                  4'sd0/*0:xpc10nz*/:  begin 
                       Ttar0_3_V_0 <= 32'sd0;
                       Ttar0_3_V_3 <= 32'sd0;
                       Ttar0_3_V_5 <= 32'sd0;
                       Ttar0_3_V_6 <= 32'sh0;
                       Ttar0_3_V_1 <= 32'sd1;
                       xpc10nz <= 4'sd1/*1:xpc10nz*/;
                       end 
                      
                  4'sd11/*11:xpc10nz*/:  begin 
                      if ((32'sd1+Ttar0_3_V_5<32'sh9) && !(rtl_signed_bitextract0(((xpc10nz==4'sd11/*11:xpc10nz*/)? A_SINT_CC_SCALbx10_ARA0_RDD0
                      : SINTCCSCALbx10ARA0RRh10hold))%32'sd2))  Ttar0_3_V_1 <= 32'sd1+Ttar0_3_V_1;
                          if ((32'sd1+Ttar0_3_V_5<32'sh9) && !(!(rtl_signed_bitextract0(((xpc10nz==4'sd11/*11:xpc10nz*/)? A_SINT_CC_SCALbx10_ARA0_RDD0
                      : SINTCCSCALbx10ARA0RRh10hold))%32'sd2)))  Ttar0_3_V_0 <= 32'sd1+Ttar0_3_V_0;
                           xpc10nz <= 4'sd10/*10:xpc10nz*/;
                       end 
                      
                  4'sd12/*12:xpc10nz*/:  xpc10nz <= 4'sd12/*12:xpc10nz*/;
              endcase
              if (SINTCCSCALbx10ARA0RRh10shot0)  SINTCCSCALbx10ARA0RRh10hold <= A_SINT_CC_SCALbx10_ARA0_RDD0;
                   SINTCCSCALbx10ARA0RRh10shot0 <= (32'sd1+Ttar0_3_V_5<32'sh9) && (xpc10nz==4'sd10/*10:xpc10nz*/);
              if ((xpc10nz==4'sd1/*1:xpc10nz*/))  xpc10nz <= 4'sd2/*2:xpc10nz*/;
                  if ((xpc10nz==4'sd2/*2:xpc10nz*/))  xpc10nz <= 4'sd3/*3:xpc10nz*/;
                  if ((xpc10nz==4'sd3/*3:xpc10nz*/))  xpc10nz <= 4'sd4/*4:xpc10nz*/;
                  if ((xpc10nz==4'sd4/*4:xpc10nz*/))  xpc10nz <= 4'sd5/*5:xpc10nz*/;
                  if ((xpc10nz==4'sd5/*5:xpc10nz*/))  xpc10nz <= 4'sd6/*6:xpc10nz*/;
                  if ((xpc10nz==4'sd6/*6:xpc10nz*/))  xpc10nz <= 4'sd7/*7:xpc10nz*/;
                  if ((xpc10nz==4'sd7/*7:xpc10nz*/))  xpc10nz <= 4'sd8/*8:xpc10nz*/;
                  if ((xpc10nz==4'sd8/*8:xpc10nz*/))  xpc10nz <= 4'sd9/*9:xpc10nz*/;
                  if ((xpc10nz==4'sd9/*9:xpc10nz*/))  xpc10nz <= 4'sd10/*10:xpc10nz*/;
                   end 
              //End structure HPR test4.exe


       end 
      

  CV_SP_SSRAM_FL1 #(32'sd32, 32'sd4, 32'sd9, 32'sd32) A_SINT_CC_SCALbx10_ARA0(clk, reset, A_SINT_CC_SCALbx10_ARA0_RDD0, A_SINT_CC_SCALbx10_ARA0_AD0
, A_SINT_CC_SCALbx10_ARA0_WEN0, A_SINT_CC_SCALbx10_ARA0_REN0, A_SINT_CC_SCALbx10_ARA0_WRD0);

// 2 vectors of width 4
// 3 vectors of width 1
// 2 vectors of width 32
// 160 bits in scalar variables
// Total state bits in module = 235 bits.
// 32 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.15p : 11th-August-2016
//14/08/2016 14:19:32
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-kcode-dump=enable -kiwic-cil-dump=separately test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b2.v -vnl-resets=synchronous -sim 3000 -bevelab-default-pause-mode=hard -res2-reprint=enable -res2-regfile-threshold=4 -res2-combram-threshold=20 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called KiwiSystem/Kiwi
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) id=cctor10
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called System/BitConverter
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) id=cctor12
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called test4
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called test4
//
//root_compiler: start elaborating class 'test4'
//
//elaborating class 'test4'
//
//root_compiler class done: test4
//
//KiwiC: front end input processing of class or method called test4/Main
//
//root_walk start thread at a static method (used as an entry point). Method name=Main uid=Main10
//
//KiwiC start_thread (or entry point) id=Main10
//
//Root method elaborated: specificf=S_root_method leftover=0+0
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=10000
//
//   kiwic-finish=enable
//
//   kiwic-cil-dump=separately
//
//   kiwic-kcode-dump=enable
//
//   array-4d-name=KIWIARRAY4D
//
//   array-3d-name=KIWIARRAY3D
//
//   array-2d-name=KIWIARRAY2D
//
//   kiwi-dll=Kiwi.dll
//
//   kiwic-dll=Kiwic.dll
//
//   kiwic-zerolength-arrays=disable
//
//   kiwic-fpgaconsole-default=enable
//
//   postgen-optimise=enable
//
//   gtrace-loglevel=20
//
//   firstpass-loglevel=20
//
//   root=test4;test4.Main
//
//   srcfile=test4.exe
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  -- No expression aliases to report
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports
//*-----------+----------+----------+--------+--------+-------+-----------*
//| Name      | Protocol | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*-----------+----------+----------+--------+--------+-------+-----------*
//| dram0bank | HFAST1   | 4194304  | 22     | 256    | 32    | 8         |
//*-----------+----------+----------+--------+--------+-------+-----------*
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int_flr_mul               | -3000   |                                                                                 |
//| fp_fl_dp_div              | 5       |                                                                                 |
//| fp_fl_dp_add              | 4       |                                                                                 |
//| fp_fl_dp_mul              | 3       |                                                                                 |
//| fp_fl_sp_div              | 5       |                                                                                 |
//| fp_fl_sp_add              | 4       |                                                                                 |
//| fp_fl_sp_mul              | 3       |                                                                                 |
//| res2-loadstore-port-count | 1       |                                                                                 |
//| max_no_fp_muls            | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max_no_fp_muls            | 6       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max_no_int_muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max_no_fp_divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max_no_int_divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 20      |                                                                                 |
//| res2-regfile-threshold    | 4       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for xpc10 
//*----------------+-----+-------------+------+------+-------+-----+-------------+------*
//| gb-flag/Pause  | eno | hwm         | root | exec | start | end | antecedants | next |
//*----------------+-----+-------------+------+------+-------+-----+-------------+------*
//|   X0:"0:xpc10" | 900 | hwm=0.0.9   | 0    | 0    | 1     | 9   | ---         | 10   |
//|   X1:"1:xpc10" | 902 | hwm=0.0.0   | 10   | 10   | -     | -   | ---         | 12   |
//|   X1:"1:xpc10" | 901 | hwm=0.1.0   | 10   | 11   | 11    | 11  | ---         | 10   |
//|   X2:"2:xpc10" | 903 | hwm=0.0.0   | 12   | 12   | -     | -   | ---         | 12   |
//*----------------+-----+-------------+------+------+-------+-----+-------------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X0:"0:xpc10" 900 :  major_start_pcl=0   edge_private_start/end=1/9 exec=0 (dend=9)
//Simple greedy schedule for res2: Thread=xpc10 state=X0:"0:xpc10"
//res2: Thread=xpc10 state=X0:"0:xpc10"
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                                    |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| 0   | -   | R0 CTRL |                                                                                                                                         |
//| 0   | 900 | R0 DATA |                                                                                                                                         |
//| 0+E | 900 | W0 DATA | Ttar0.3_V_1 te=te:0 scalarw(1) Ttar0.3_V_6 te=te:0 scalarw(S32'0I) Ttar0.3_V_5 te=te:0 scalarw(0) Ttar0.3_V_3 te=te:0 scalarw(0) Ttar0\ |
//|     |     |         | .3_V_0 te=te:0 scalarw(0) @_SINT/CC/SCALbx10_ARA0 te=te:0 write(2, 2)                                                                   |
//| 1   | 900 | W1 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:1 write(8, 2048)                                                                                          |
//| 2   | 900 | W2 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:2 write(7, 2021)                                                                                          |
//| 3   | 900 | W3 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:3 write(6, 1121)                                                                                          |
//| 4   | 900 | W4 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:4 write(5, 8)                                                                                             |
//| 5   | 900 | W5 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:5 write(4, 7)                                                                                             |
//| 6   | 900 | W6 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:6 write(3, 5)                                                                                             |
//| 7   | 900 | W7 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:7 write(1, 1)                                                                                             |
//| 8   | 900 | W8 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:8 write(0, 0)                                                                                             |
//| 9   | 900 | W9 DATA |                                                                                                                                         |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1:"1:xpc10" 902 :  major_start_pcl=10   edge_private_start/end=-1/-1 exec=10 (dend=0)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1:"1:xpc10" 901 :  major_start_pcl=10   edge_private_start/end=11/11 exec=11 (dend=1)
//Simple greedy schedule for res2: Thread=xpc10 state=X1:"1:xpc10"
//res2: Thread=xpc10 state=X1:"1:xpc10"
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------*
//| 10   | -   | R0 CTRL |                                                                                                                     |
//| 10   | 901 | R0 DATA | @_SINT/CC/SCALbx10_ARA0 te=te:10 read(1+Ttar0.3_V_5)                                                                |
//| 11   | 901 | R1 DATA |                                                                                                                     |
//| 11+E | 901 | W0 DATA | Ttar0.3_V_1 te=te:11 scalarw(1+Ttar0.3_V_1) Ttar0.3_V_6 te=te:11 scalarw(E1) Ttar0.3_V_5 te=te:11 scalarw(1+Ttar0.\ |
//|      |     |         | 3_V_5) Ttar0.3_V_3 te=te:11 scalarw(1+(C(Ttar0.3_V_3))) Ttar0.3_V_0 te=te:11 scalarw(1+Ttar0.3_V_0)  PLI:so far %u\ |
//|      |     |         |  Odd Number...  PLI:%u vale=%u:                                                                                     |
//| 10   | 902 | R0 DATA |                                                                                                                     |
//| 10+E | 902 | W0 DATA | Ttar0.3_V_5 te=te:10 scalarw(1+Ttar0.3_V_5) Ttar0.3_V_3 te=te:10 scalarw(1+(C(Ttar0.3_V_3)))  PLI:Found %u Odd Num\ |
//|      |     |         | bers...  PLI:so far %u Odd Number...  PLI:%u vale=%u:                                                               |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X2:"2:xpc10" 903 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X2:"2:xpc10"
//res2: Thread=xpc10 state=X2:"2:xpc10"
//*------+-----+---------+-----------------------*
//| pc   | eno | Phaser  | Work                  |
//*------+-----+---------+-----------------------*
//| 12   | -   | R0 CTRL |                       |
//| 12   | 903 | R0 DATA |                       |
//| 12+E | 903 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*------+-----+---------+-----------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//Highest off-chip SRAM/DRAM location in use on port dram0bank is <null> (--not-used--) bytes=1048576

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= C(@_SINT/CC/SCALbx10_ARA0[1+Ttar0.3_V_5])
//

//----------------------------------------------------------

//Report from verilog_render:::
//2 vectors of width 4
//
//3 vectors of width 1
//
//2 vectors of width 32
//
//160 bits in scalar variables
//
//Total state bits in module = 235 bits.
//
//32 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 33 CIL instructions in 7 basic blocks
//Thread mpc10 has 3 bevelab control states (pauses)
//Reindexed thread xpc10 with 13 minor control states
// eof (HPR L/S Verilog)
