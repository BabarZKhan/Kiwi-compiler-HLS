

// CBG Orangepath HPR L/S System

// Verilog output file generated at 25/08/2016 20:53:35
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.15t : 21st-August-2016 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-register-colours=1 primesya.exe -sim=21999 -vnl=onchip.v -vnl-rootmodname=DUT -kiwic-finish=disable -protocol=none
`timescale 1ns/1ns


module DUT(    output reg [639:0] KppWaypoint0,
    output reg [639:0] KppWaypoint1,
    input [31:0] volume,
    output reg [31:0] count,
    output reg signed [31:0] elimit,
    output reg signed [31:0] evariant,
    output reg signed [31:0] edesign,
    output reg finished,
    input clk,
    input reset);
  integer pTMT4Main_V_1;
  integer pTMT4Main_V_2;
  integer pTMT4Main_V_3;
  integer pTMT4Main_V_4;
  wire A_BOOL_CC_SCALbx10_ARA0_RDD0;
  reg [9:0] A_BOOL_CC_SCALbx10_ARA0_AD0;
  reg A_BOOL_CC_SCALbx10_ARA0_WEN0;
  reg A_BOOL_CC_SCALbx10_ARA0_REN0;
  reg A_BOOL_CC_SCALbx10_ARA0_WRD0;
  reg BOOLCCSCALbx10ARA0RRh10hold;
  reg BOOLCCSCALbx10ARA0RRh10shot0;
  reg [4:0] xpc10nz;
 always   @(* )  begin 
       KppWaypoint1 = 32'sd0;
       KppWaypoint0 = 32'sd0;
       A_BOOL_CC_SCALbx10_ARA0_AD0 = 32'sd0;
       A_BOOL_CC_SCALbx10_ARA0_WRD0 = 32'sd0;
       A_BOOL_CC_SCALbx10_ARA0_WEN0 = 32'sd0;
       A_BOOL_CC_SCALbx10_ARA0_REN0 = 32'sd0;
       A_BOOL_CC_SCALbx10_ARA0_REN0 = ((xpc10nz==5'sd8/*8:xpc10nz*/)? 1'd1: 1'd0);
       A_BOOL_CC_SCALbx10_ARA0_WEN0 = (xpc10nz==5'sd4/*4:xpc10nz*/) || (xpc10nz==5'sd1/*1:xpc10nz*/) || (xpc10nz==5'sd14/*14:xpc10nz*/);

      case (xpc10nz)
          5'sd1/*1:xpc10nz*/:  begin 
               A_BOOL_CC_SCALbx10_ARA0_WRD0 = (32'sd0<volume);
               A_BOOL_CC_SCALbx10_ARA0_AD0 = 10'd0;
               end 
              
          5'sd4/*4:xpc10nz*/:  begin 
               A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'h1;
               A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_1;
               end 
              endcase
      if ((xpc10nz==5'sd8/*8:xpc10nz*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_4;
          if ((xpc10nz==5'sd14/*14:xpc10nz*/))  begin 
              if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2<32'sd999))  begin 
                       A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'h0;
                       A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3;
                       end 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2>=32'sd999))  begin 
                       A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'h0;
                       A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3;
                       end 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3<32'sd1000))  begin 
                       A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'h0;
                       A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3;
                       end 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2>=32'sd999))  begin 
                       KppWaypoint0 = "wp3";
                       KppWaypoint1 = "COUNTING";
                       end 
                       end 
              if ((xpc10nz==5'sd11/*11:xpc10nz*/))  KppWaypoint0 = "FINISH";
          if ((pTMT4Main_V_2+pTMT4Main_V_2>=32'sd1000) && (xpc10nz==5'sd7/*7:xpc10nz*/))  begin 
               KppWaypoint0 = "wp3";
               KppWaypoint1 = "COUNTING";
               end 
              if ((pTMT4Main_V_1>=32'sd999) && (xpc10nz==5'sd4/*4:xpc10nz*/))  begin 
               KppWaypoint0 = "wp2";
               KppWaypoint1 = "CROSSOFF";
               end 
              if ((xpc10nz==5'sd0/*0:xpc10nz*/))  begin 
               KppWaypoint0 = "START";
               KppWaypoint1 = "INITIALISE";
               end 
               end 
      

 always   @(posedge clk )  begin 
      //Start structure HPR primesya
      if (reset)  begin 
               pTMT4Main_V_4 <= 32'd0;
               pTMT4Main_V_3 <= 32'd0;
               pTMT4Main_V_2 <= 32'd0;
               pTMT4Main_V_1 <= 32'd0;
               count <= 32'd0;
               evariant <= 32'd0;
               edesign <= 32'd0;
               finished <= 1'd0;
               elimit <= 32'd0;
               BOOLCCSCALbx10ARA0RRh10hold <= 1'd0;
               BOOLCCSCALbx10ARA0RRh10shot0 <= 1'd0;
               xpc10nz <= 5'd0;
               end 
               else  begin 
              
              case (xpc10nz)
                  5'sd0/*0:xpc10nz*/:  begin 
                      $display("%s%1d", "Primes Up To ", 32'sd1000);
                       count <= 32'h0;
                       evariant <= 32'sd0;
                       edesign <= 32'sd4032;
                       finished <= 1'h0;
                       elimit <= 32'sd1000;
                       xpc10nz <= 5'sd1/*1:xpc10nz*/;
                       end 
                      
                  5'sd3/*3:xpc10nz*/:  begin 
                       pTMT4Main_V_1 <= 32'sd0;
                       count <= 32'h0;
                       xpc10nz <= 5'sd4/*4:xpc10nz*/;
                       end 
                      
                  5'sd4/*4:xpc10nz*/:  begin 
                      $display("Setting initial array flag to hold : addr=%1d readback=%1d", pTMT4Main_V_1, 1'h1);
                      if ((pTMT4Main_V_1<32'sd999))  begin 
                               pTMT4Main_V_1 <= 32'sd1+pTMT4Main_V_1;
                               xpc10nz <= 5'sd5/*5:xpc10nz*/;
                               end 
                               else  begin 
                               pTMT4Main_V_2 <= 32'sd2;
                               pTMT4Main_V_1 <= 32'sd1+pTMT4Main_V_1;
                               xpc10nz <= 5'sd6/*6:xpc10nz*/;
                               end 
                               end 
                      
                  5'sd7/*7:xpc10nz*/:  begin 
                      if ((pTMT4Main_V_2+pTMT4Main_V_2>=32'sd1000))  begin 
                              $display("Skip out on square");
                              $display("Now counting");
                               end 
                               else $display("Cross off %1d %1d   (count1=%1d)", pTMT4Main_V_2, pTMT4Main_V_2+pTMT4Main_V_2, 32'sd2);
                      if ((pTMT4Main_V_2+pTMT4Main_V_2<32'sd1000))  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_2;
                               xpc10nz <= 5'sd14/*14:xpc10nz*/;
                               end 
                               else  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_2;
                               pTMT4Main_V_4 <= 32'sd0;
                               xpc10nz <= 5'sd8/*8:xpc10nz*/;
                               end 
                               end 
                      
                  5'sd9/*9:xpc10nz*/:  begin 
                      if (((xpc10nz==5'sd9/*9:xpc10nz*/)? A_BOOL_CC_SCALbx10_ARA0_RDD0: BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      <32'sd999)) $display("Tally counting %1d %1d", pTMT4Main_V_4, 32'sd1+count);
                          if (((xpc10nz==5'sd9/*9:xpc10nz*/)? !A_BOOL_CC_SCALbx10_ARA0_RDD0: !BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      <32'sd999)) $display("Tally counting %1d %1d", pTMT4Main_V_4, count);
                          if (((xpc10nz==5'sd9/*9:xpc10nz*/)? A_BOOL_CC_SCALbx10_ARA0_RDD0: BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      >=32'sd999))  begin 
                              $display("Tally counting %1d %1d", pTMT4Main_V_4, 32'sd1+count);
                              $display("There are %1d primes below the natural number %1d.", 32'sd1+count, 32'sd1000);
                              $display("Optimisation variant=%1d (count1 is %1d).", 32'sd0, 32'sd2);
                               end 
                              if (((xpc10nz==5'sd9/*9:xpc10nz*/)? !A_BOOL_CC_SCALbx10_ARA0_RDD0: !BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      >=32'sd999))  begin 
                              $display("Tally counting %1d %1d", pTMT4Main_V_4, count);
                              $display("There are %1d primes below the natural number %1d.", count, 32'sd1000);
                              $display("Optimisation variant=%1d (count1 is %1d).", 32'sd0, 32'sd2);
                               end 
                              if (((xpc10nz==5'sd9/*9:xpc10nz*/)? A_BOOL_CC_SCALbx10_ARA0_RDD0: BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      >=32'sd999))  begin 
                               pTMT4Main_V_4 <= 32'sd1+pTMT4Main_V_4;
                               count <= 32'sd1+count;
                               end 
                              if (((xpc10nz==5'sd9/*9:xpc10nz*/)? !A_BOOL_CC_SCALbx10_ARA0_RDD0: !BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      >=32'sd999))  pTMT4Main_V_4 <= 32'sd1+pTMT4Main_V_4;
                          if ((pTMT4Main_V_4<32'sd999))  begin 
                               pTMT4Main_V_4 <= 32'sd1+pTMT4Main_V_4;
                               xpc10nz <= 5'sd8/*8:xpc10nz*/;
                               end 
                              if (((xpc10nz==5'sd9/*9:xpc10nz*/)? A_BOOL_CC_SCALbx10_ARA0_RDD0: BOOLCCSCALbx10ARA0RRh10hold) && (pTMT4Main_V_4
                      <32'sd999))  count <= 32'sd1+count;
                          if ((pTMT4Main_V_4>=32'sd999))  xpc10nz <= 5'sd10/*10:xpc10nz*/;
                           end 
                      
                  5'sd12/*12:xpc10nz*/:  begin 
                       finished <= 1'h1;
                       xpc10nz <= 5'sd13/*13:xpc10nz*/;
                       end 
                      
                  5'sd14/*14:xpc10nz*/:  begin 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3<32'sd1000)) $display("Cross off %1d %1d   (count1=%1d)", pTMT4Main_V_2, pTMT4Main_V_2
                          +pTMT4Main_V_3, 32'sd2);
                          if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2>=32'sd999)) $display("Now counting");
                          if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2>=32'sd999))  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               pTMT4Main_V_4 <= 32'sd0;
                               pTMT4Main_V_2 <= 32'sd1+pTMT4Main_V_2;
                               xpc10nz <= 5'sd16/*16:xpc10nz*/;
                               end 
                              if ((pTMT4Main_V_2+pTMT4Main_V_3>=32'sd1000) && (pTMT4Main_V_2<32'sd999))  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               pTMT4Main_V_2 <= 32'sd1+pTMT4Main_V_2;
                               xpc10nz <= 5'sd17/*17:xpc10nz*/;
                               end 
                              if ((pTMT4Main_V_2+pTMT4Main_V_3<32'sd1000))  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               xpc10nz <= 5'sd15/*15:xpc10nz*/;
                               end 
                               end 
                      endcase
              if (BOOLCCSCALbx10ARA0RRh10shot0)  BOOLCCSCALbx10ARA0RRh10hold <= A_BOOL_CC_SCALbx10_ARA0_RDD0;
                   BOOLCCSCALbx10ARA0RRh10shot0 <= (xpc10nz==5'sd8/*8:xpc10nz*/);
              if ((xpc10nz==5'sd1/*1:xpc10nz*/))  xpc10nz <= 5'sd2/*2:xpc10nz*/;
                  if ((xpc10nz==5'sd2/*2:xpc10nz*/))  xpc10nz <= 5'sd3/*3:xpc10nz*/;
                  if ((xpc10nz==5'sd5/*5:xpc10nz*/))  xpc10nz <= 5'sd4/*4:xpc10nz*/;
                  if ((xpc10nz==5'sd6/*6:xpc10nz*/))  xpc10nz <= 5'sd7/*7:xpc10nz*/;
                  if ((xpc10nz==5'sd8/*8:xpc10nz*/))  xpc10nz <= 5'sd9/*9:xpc10nz*/;
                  if ((xpc10nz==5'sd10/*10:xpc10nz*/))  xpc10nz <= 5'sd11/*11:xpc10nz*/;
                  if ((xpc10nz==5'sd11/*11:xpc10nz*/))  xpc10nz <= 5'sd12/*12:xpc10nz*/;
                  if ((xpc10nz==5'sd13/*13:xpc10nz*/))  xpc10nz <= 5'sd13/*13:xpc10nz*/;
                  if ((xpc10nz==5'sd15/*15:xpc10nz*/))  xpc10nz <= 5'sd14/*14:xpc10nz*/;
                  if ((xpc10nz==5'sd16/*16:xpc10nz*/))  xpc10nz <= 5'sd8/*8:xpc10nz*/;
                  if ((xpc10nz==5'sd17/*17:xpc10nz*/))  xpc10nz <= 5'sd7/*7:xpc10nz*/;
                   end 
              //End structure HPR primesya


       end 
      

  CV_SP_SSRAM_FL1 #(32'sd1, 32'sd10, 32'sd1000, 32'sd1) A_BOOL_CC_SCALbx10_ARA0(clk, reset, A_BOOL_CC_SCALbx10_ARA0_RDD0, A_BOOL_CC_SCALbx10_ARA0_AD0
, A_BOOL_CC_SCALbx10_ARA0_WEN0, A_BOOL_CC_SCALbx10_ARA0_REN0, A_BOOL_CC_SCALbx10_ARA0_WRD0);

// 1 vectors of width 5
// 5 vectors of width 1
// 1 vectors of width 10
// 128 bits in scalar variables
// Total state bits in module = 148 bits.
// 1 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.15t : 21st-August-2016
//25/08/2016 20:53:29
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-register-colours=1 primesya.exe -sim=21999 -vnl=onchip.v -vnl-rootmodname=DUT -kiwic-finish=disable -protocol=none


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
//KiwiC: front end input processing of class or method called primesya
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called primesya
//
//root_compiler: start elaborating class 'primesya'
//
//elaborating class 'primesya'
//
//compiling static method as entry point: style=Root idl=primesya/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//root_compiler class done: primesya
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=10000
//
//   kiwic-finish=disable
//
//   kiwic-cil-dump=combined
//
//   kiwic-kcode-dump=disable
//
//   kiwic-register-colours=1
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
//   root=$attributeroot
//
//   srcfile=primesya.exe
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
//| dram0bank | unset    | 4194304  | 22     | 256    | 32    | 8         |
//*-----------+----------+----------+--------+--------+-------+-----------*
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int_flr_mul               | -3000   |                                                                                 |
//| max_no_fp_addsubs         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max_no_fp_muls            | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max_no_int_muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max_no_fp_divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max_no_int_divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| fp_fl_dp_div              | 5       |                                                                                 |
//| fp_fl_dp_add              | 4       |                                                                                 |
//| fp_fl_dp_mul              | 3       |                                                                                 |
//| fp_fl_sp_div              | 5       |                                                                                 |
//| fp_fl_sp_add              | 4       |                                                                                 |
//| fp_fl_sp_mul              | 3       |                                                                                 |
//| res2-loadstore-port-count | 1       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for xpc10 
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//| gb-flag/Pause        | eno | hwm         | root | exec | start | end | antecedants | next |
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//|   X0:"0:xpc10"       | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1    |
//|   X1:"1:xpc10"       | 901 | hwm=0.0.1   | 1    | 1    | 2     | 2   | ---         | 3    |
//|   X2:"2:xpc10"       | 902 | hwm=0.0.0   | 3    | 3    | -     | -   | ---         | 4    |
//|   X4:"4:xpc10"       | 904 | hwm=0.0.1   | 4    | 4    | 6     | 6   | ---         | 7    |
//|   X4:"4:xpc10"       | 903 | hwm=0.0.1   | 4    | 4    | 5     | 5   | ---         | 4    |
//|   X8:"8:xpc10"       | 906 | hwm=0.0.0   | 7    | 7    | -     | -   | ---         | 14   |
//|   X8:"8:xpc10"       | 905 | hwm=0.0.0   | 7    | 7    | -     | -   | ---         | 8    |
//|   X16:"16:xpc10"     | 909 | hwm=1.1.0   | 8    | 9    | -     | -   | ---         | 10   |
//|   X16:"16:xpc10"     | 908 | hwm=1.1.0   | 8    | 9    | -     | -   | ---         | 10   |
//|   X16:"16:xpc10"     | 907 | hwm=1.1.0   | 8    | 9    | -     | -   | ---         | 8    |
//|   X32:"32:xpc10"     | 910 | hwm=0.0.0   | 10   | 10   | -     | -   | ---         | 11   |
//|   X64:"64:xpc10"     | 911 | hwm=0.0.0   | 11   | 11   | -     | -   | ---         | 12   |
//|   X128:"128:xpc10"   | 912 | hwm=0.0.0   | 12   | 12   | -     | -   | ---         | 13   |
//|   X1024:"1024:xpc10" | 915 | hwm=0.0.1   | 14   | 14   | 17    | 17  | ---         | 7    |
//|   X1024:"1024:xpc10" | 914 | hwm=0.0.1   | 14   | 14   | 16    | 16  | ---         | 8    |
//|   X1024:"1024:xpc10" | 913 | hwm=0.0.1   | 14   | 14   | 15    | 15  | ---         | 14   |
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X0:"0:xpc10" 900 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X0:"0:xpc10"
//res2: Thread=xpc10 state=X0:"0:xpc10"
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                  |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| 0   | -   | R0 CTRL |                                                                                                                       |
//| 0   | 900 | R0 DATA |                                                                                                                       |
//| 0+E | 900 | W0 DATA | elimit te=te:0 scalarw(1000) finished te=te:0 scalarw(U1'0) edesign te=te:0 scalarw(4032) evariant te=te:0 scalarw(0\ |
//|     |     |         | ) count te=te:0 scalarw(U32'0)  PLI:Primes Up To   W/P:START                                                          |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1:"1:xpc10" 901 :  major_start_pcl=1   edge_private_start/end=2/2 exec=1 (dend=1)
//Simple greedy schedule for res2: Thread=xpc10 state=X1:"1:xpc10"
//res2: Thread=xpc10 state=X1:"1:xpc10"
//*-----+-----+---------+----------------------------------------------------*
//| pc  | eno | Phaser  | Work                                               |
//*-----+-----+---------+----------------------------------------------------*
//| 1   | -   | R0 CTRL |                                                    |
//| 1   | 901 | R0 DATA |                                                    |
//| 1+E | 901 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:1 write(0, 0<volume) |
//| 2   | 901 | W1 DATA |                                                    |
//*-----+-----+---------+----------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X2:"2:xpc10" 902 :  major_start_pcl=3   edge_private_start/end=-1/-1 exec=3 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X2:"2:xpc10"
//res2: Thread=xpc10 state=X2:"2:xpc10"
//*-----+-----+---------+---------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                          |
//*-----+-----+---------+---------------------------------------------------------------*
//| 3   | -   | R0 CTRL |                                                               |
//| 3   | 902 | R0 DATA |                                                               |
//| 3+E | 902 | W0 DATA | count te=te:3 scalarw(U32'0) pTMT4Main_V_1 te=te:3 scalarw(0) |
//*-----+-----+---------+---------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X4:"4:xpc10" 904 :  major_start_pcl=4   edge_private_start/end=6/6 exec=4 (dend=1)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X4:"4:xpc10" 903 :  major_start_pcl=4   edge_private_start/end=5/5 exec=4 (dend=1)
//Simple greedy schedule for res2: Thread=xpc10 state=X4:"4:xpc10"
//res2: Thread=xpc10 state=X4:"4:xpc10"
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                  |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| 4   | -   | R0 CTRL |                                                                                                                       |
//| 4   | 903 | R0 DATA |                                                                                                                       |
//| 4+E | 903 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:4 write(pTMT4Main_V_1, U1'1) pTMT4Main_V_1 te=te:4 scalarw(1+pTMT4Main_V_1)  PLI:Setti\ |
//|     |     |         | ng initial arra...                                                                                                    |
//| 5   | 903 | W1 DATA |                                                                                                                       |
//| 4   | 904 | R0 DATA |                                                                                                                       |
//| 4+E | 904 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:4 write(pTMT4Main_V_1, U1'1) pTMT4Main_V_1 te=te:4 scalarw(1+pTMT4Main_V_1) pTMT4Main_\ |
//|     |     |         | V_2 te=te:4 scalarw(2)  W/P:wp2  PLI:Setting initial arra...                                                          |
//| 6   | 904 | W1 DATA |                                                                                                                       |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X8:"8:xpc10" 906 :  major_start_pcl=7   edge_private_start/end=-1/-1 exec=7 (dend=0)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X8:"8:xpc10" 905 :  major_start_pcl=7   edge_private_start/end=-1/-1 exec=7 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X8:"8:xpc10"
//res2: Thread=xpc10 state=X8:"8:xpc10"
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                  |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//| 7   | -   | R0 CTRL |                                                                                                                       |
//| 7   | 905 | R0 DATA |                                                                                                                       |
//| 7+E | 905 | W0 DATA | pTMT4Main_V_4 te=te:7 scalarw(0) pTMT4Main_V_3 te=te:7 scalarw(E1)  PLI:Now counting  W/P:wp3  PLI:Skip out on square |
//| 7   | 906 | R0 DATA |                                                                                                                       |
//| 7+E | 906 | W0 DATA | pTMT4Main_V_3 te=te:7 scalarw(E1)  PLI:Cross off %u %u   (c...                                                        |
//*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X16:"16:xpc10" 909 :  major_start_pcl=8   edge_private_start/end=-1/-1 exec=9 (dend=1)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X16:"16:xpc10" 908 :  major_start_pcl=8   edge_private_start/end=-1/-1 exec=9 (dend=1)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X16:"16:xpc10" 907 :  major_start_pcl=8   edge_private_start/end=-1/-1 exec=9 (dend=1)
//Simple greedy schedule for res2: Thread=xpc10 state=X16:"16:xpc10"
//res2: Thread=xpc10 state=X16:"16:xpc10"
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                                                              |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| 8   | -   | R0 CTRL | @_BOOL/CC/SCALbx10_ARA0 te=te:8 read(pTMT4Main_V_4)                                                                                                               |
//| 9   | -   | R1 CTRL |                                                                                                                                                                   |
//| 8   | 907 | R0 DATA |                                                                                                                                                                   |
//| 9   | 907 | R1 DATA |                                                                                                                                                                   |
//| 9+E | 907 | W0 DATA | count te=te:9 scalarw(1+count) pTMT4Main_V_4 te=te:9 scalarw(1+pTMT4Main_V_4)  PLI:Tally counting %u %d                                                           |
//| 8   | 908 | R0 DATA |                                                                                                                                                                   |
//| 9   | 908 | R1 DATA |                                                                                                                                                                   |
//| 9+E | 908 | W0 DATA | count te=te:9 scalarw(1+count) pTMT4Main_V_4 te=te:9 scalarw(1+pTMT4Main_V_4)  PLI:Optimisation variant...  PLI:There are %d primes ...  PLI:Tally counting %u %d |
//| 8   | 909 | R0 DATA |                                                                                                                                                                   |
//| 9   | 909 | R1 DATA |                                                                                                                                                                   |
//| 9+E | 909 | W0 DATA | pTMT4Main_V_4 te=te:9 scalarw(1+pTMT4Main_V_4)  PLI:Optimisation variant...  PLI:There are %d primes ...  PLI:Tally counting %u %d                                |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X32:"32:xpc10" 910 :  major_start_pcl=10   edge_private_start/end=-1/-1 exec=10 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X32:"32:xpc10"
//res2: Thread=xpc10 state=X32:"32:xpc10"
//*------+-----+---------+------*
//| pc   | eno | Phaser  | Work |
//*------+-----+---------+------*
//| 10   | -   | R0 CTRL |      |
//| 10   | 910 | R0 DATA |      |
//| 10+E | 910 | W0 DATA |      |
//*------+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X64:"64:xpc10" 911 :  major_start_pcl=11   edge_private_start/end=-1/-1 exec=11 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X64:"64:xpc10"
//res2: Thread=xpc10 state=X64:"64:xpc10"
//*------+-----+---------+-------------*
//| pc   | eno | Phaser  | Work        |
//*------+-----+---------+-------------*
//| 11   | -   | R0 CTRL |             |
//| 11   | 911 | R0 DATA |             |
//| 11+E | 911 | W0 DATA |  W/P:FINISH |
//*------+-----+---------+-------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X128:"128:xpc10" 912 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X128:"128:xpc10"
//res2: Thread=xpc10 state=X128:"128:xpc10"
//*------+-----+---------+---------------------------------*
//| pc   | eno | Phaser  | Work                            |
//*------+-----+---------+---------------------------------*
//| 12   | -   | R0 CTRL |                                 |
//| 12   | 912 | R0 DATA |                                 |
//| 12+E | 912 | W0 DATA | finished te=te:12 scalarw(U1'1) |
//*------+-----+---------+---------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X256:"256:xpc10"
//res2: Thread=xpc10 state=X256:"256:xpc10"
//*----+-----+---------+------*
//| pc | eno | Phaser  | Work |
//*----+-----+---------+------*
//| 13 | -   | R0 CTRL |      |
//*----+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1024:"1024:xpc10" 915 :  major_start_pcl=14   edge_private_start/end=17/17 exec=14 (dend=1)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1024:"1024:xpc10" 914 :  major_start_pcl=14   edge_private_start/end=16/16 exec=14 (dend=1)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1024:"1024:xpc10" 913 :  major_start_pcl=14   edge_private_start/end=15/15 exec=14 (dend=1)
//Simple greedy schedule for res2: Thread=xpc10 state=X1024:"1024:xpc10"
//res2: Thread=xpc10 state=X1024:"1024:xpc10"
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                    |
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| 14   | -   | R0 CTRL |                                                                                                                                         |
//| 14   | 913 | R0 DATA |                                                                                                                                         |
//| 14+E | 913 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:14 write(pTMT4Main_V_3, U1'0) pTMT4Main_V_3 te=te:14 scalarw(E2)  PLI:Cross off %u %u   (c...             |
//| 15   | 913 | W1 DATA |                                                                                                                                         |
//| 14   | 914 | R0 DATA |                                                                                                                                         |
//| 14+E | 914 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:14 write(pTMT4Main_V_3, U1'0) pTMT4Main_V_2 te=te:14 scalarw(1+pTMT4Main_V_2) pTMT4Main_V_4 te=te:14 sca\ |
//|      |     |         | larw(0) pTMT4Main_V_3 te=te:14 scalarw(E2)  PLI:Now counting  W/P:wp3                                                                   |
//| 16   | 914 | W1 DATA |                                                                                                                                         |
//| 14   | 915 | R0 DATA |                                                                                                                                         |
//| 14+E | 915 | W0 DATA | @_BOOL/CC/SCALbx10_ARA0 te=te:14 write(pTMT4Main_V_3, U1'0) pTMT4Main_V_2 te=te:14 scalarw(1+pTMT4Main_V_2) pTMT4Main_V_3 te=te:14 sca\ |
//|      |     |         | larw(E2)                                                                                                                                |
//| 17   | 915 | W1 DATA |                                                                                                                                         |
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//Highest off-chip SRAM/DRAM location in use on port dram0bank is <null> (--not-used--) bytes=1048576

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= pTMT4Main_V_2+pTMT4Main_V_2
//
//  E2 =.= pTMT4Main_V_2+pTMT4Main_V_3
//
//  E3 =.= pTMT4Main_V_2+pTMT4Main_V_2>=1000
//
//  E4 =.= pTMT4Main_V_2+pTMT4Main_V_2<1000
//
//  E5 =.= {[pTMT4Main_V_4>=999, xpc10nz!=X9:"9:xpc10nz", |-|BOOLCCSCALbx10ARA0RRh10hold]; [pTMT4Main_V_4>=999, xpc10nz==X9:"9:xpc10nz", |-|@_BOOL/CC/SCALbx10_ARA0_RDD0]}
//
//  E6 =.= {[pTMT4Main_V_4>=999, xpc10nz!=X9:"9:xpc10nz", !(|-|BOOLCCSCALbx10ARA0RRh10hold)]; [pTMT4Main_V_4>=999, xpc10nz==X9:"9:xpc10nz", !(|-|@_BOOL/CC/SCALbx10_ARA0_RDD0)]}
//
//  E7 =.= pTMT4Main_V_2+pTMT4Main_V_3<1000
//
//  E8 =.= {[pTMT4Main_V_2+pTMT4Main_V_3>=1000, pTMT4Main_V_2>=999]}
//
//  E9 =.= {[pTMT4Main_V_2+pTMT4Main_V_3>=1000, pTMT4Main_V_2<999]}
//

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 5
//
//5 vectors of width 1
//
//1 vectors of width 10
//
//128 bits in scalar variables
//
//Total state bits in module = 148 bits.
//
//1 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 10 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 76 CIL instructions in 18 basic blocks
//Thread mpc10 has 12 bevelab control states (pauses)
//Reindexed thread xpc10 with 18 minor control states
// eof (HPR L/S Verilog)
