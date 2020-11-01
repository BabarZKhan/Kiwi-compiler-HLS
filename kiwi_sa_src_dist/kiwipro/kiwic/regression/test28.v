

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:46:39
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test28.exe -diosim-vcd=test28.vcd -sim=1800 -give-backtrace -report-each-step
`timescale 1ns/1ns


module test28(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiTESTMAIN400PC10nz_pc_export,
    
/* portgroup= abstractionName=bondout2 pi_name=bondout0 */
output reg bondout0_REQ0,
    input bondout0_ACK0,
    input bondout0_REQRDY0,
    output reg bondout0_ACKRDY0,
    output reg [21:0] bondout0_ADDR0,
    output reg bondout0_RWBAR0,
    output reg [63:0] bondout0_WDATA0,
    input [63:0] bondout0_RDATA0,
    output reg [7:0] bondout0_LANES0,
    output reg bondout0_REQ1,
    input bondout0_ACK1,
    input bondout0_REQRDY1,
    output reg bondout0_ACKRDY1,
    output reg [21:0] bondout0_ADDR1,
    output reg bondout0_RWBAR1,
    output reg [63:0] bondout0_WDATA1,
    input [63:0] bondout0_RDATA1,
    output reg [7:0] bondout0_LANES1,
    
/* portgroup= abstractionName=bondout0 pi_name=bondout1 */
output reg bondout1_REQ0,
    input bondout1_ACK0,
    input bondout1_REQRDY0,
    output reg bondout1_ACKRDY0,
    output reg [21:0] bondout1_ADDR0,
    output reg bondout1_RWBAR0,
    output reg [63:0] bondout1_WDATA0,
    input [63:0] bondout1_RDATA0,
    output reg [7:0] bondout1_LANES0,
    output reg bondout1_REQ1,
    input bondout1_ACK1,
    input bondout1_REQRDY1,
    output reg bondout1_ACKRDY1,
    output reg [21:0] bondout1_ADDR1,
    output reg bondout1_RWBAR1,
    output reg [63:0] bondout1_WDATA1,
    input [63:0] bondout1_RDATA1,
    output reg [7:0] bondout1_LANES1);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test28_Main_V_7;
  reg signed [31:0] TESTMAIN400_test28_Main_V_6;
  reg signed [31:0] TESTMAIN400_test28_Main_V_5;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_MAPR10NoCE0_SCALARF0_0;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE1_SCALARF0_0;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE2_SCALARF0_0;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE3_SCALARF0_0;
  reg signed [63:0] A_sA_SINT_CC_SCALbx12_ARA0[4:0];
// abstractionName=res2-morenets
  reg [2:0] kiwiTESTMAIN400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin500304x10;
  wire signed [31:0] hprpin500312x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test28_Main_V_7 <= 32'd0;
               TESTMAIN400_test28_Main_V_5 <= 32'd0;
               TESTMAIN400_test28_Main_V_6 <= 32'd0;
               A_SINT_CC_MAPR10NoCE3_SCALARF0_0 <= 32'd0;
               A_SINT_CC_MAPR10NoCE2_SCALARF0_0 <= 32'd0;
               A_SINT_CC_MAPR10NoCE1_SCALARF0_0 <= 32'd0;
               A_SINT_CC_MAPR10NoCE0_SCALARF0_0 <= 32'd0;
               A_sA_SINT_CC_SCALbx12_ARA0[64'sd4] <= 64'd0;
               A_sA_SINT_CC_SCALbx12_ARA0[64'sd3] <= 64'd0;
               A_sA_SINT_CC_SCALbx12_ARA0[64'sd2] <= 64'd0;
               A_sA_SINT_CC_SCALbx12_ARA0[64'sd1] <= 64'd0;
               A_sA_SINT_CC_SCALbx12_ARA0[64'sd0] <= 64'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test28_Main_V_7 <= 32'sh0;
                           TESTMAIN400_test28_Main_V_5 <= 32'sh0;
                           TESTMAIN400_test28_Main_V_6 <= 32'sh0;
                           A_SINT_CC_MAPR10NoCE3_SCALARF0_0 <= 32'shd;
                           A_SINT_CC_MAPR10NoCE2_SCALARF0_0 <= 32'shd5;
                           A_SINT_CC_MAPR10NoCE1_SCALARF0_0 <= 32'sh139;
                           A_SINT_CC_MAPR10NoCE0_SCALARF0_0 <= 32'sh19d;
                           A_sA_SINT_CC_SCALbx12_ARA0[64'sd4] <= 64'sh2;
                           A_sA_SINT_CC_SCALbx12_ARA0[64'sd3] <= 64'sh0;
                           A_sA_SINT_CC_SCALbx12_ARA0[64'sd2] <= 64'sh1;
                           A_sA_SINT_CC_SCALbx12_ARA0[64'sd1] <= 64'sh2;
                           A_sA_SINT_CC_SCALbx12_ARA0[64'sd0] <= 64'sh3;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test28_Main_V_5 <= 32'sh0;
                           TESTMAIN400_test28_Main_V_6 <= 32'sh0;
                           end 
                          
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test28_Main_V_7 <= $signed(32'sd1+TESTMAIN400_test28_Main_V_7);
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((TESTMAIN400_test28_Main_V_7>=32'sh5)) $finish(32'sd0);
                               else  begin 
                                  $display(" Test28 p2 i=%1d  s=%1d.", TESTMAIN400_test28_Main_V_7, $signed(TESTMAIN400_test28_Main_V_5
                                  +((32'h3/*3:USA14*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_7])? A_SINT_CC_MAPR10NoCE3_SCALARF0_0
                                  : hprpin500312x10)));
                                   kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test28_Main_V_5 <= $signed(TESTMAIN400_test28_Main_V_5+((32'h3/*3:USA14*/==A_sA_SINT_CC_SCALbx12_ARA0
                                  [TESTMAIN400_test28_Main_V_7])? A_SINT_CC_MAPR10NoCE3_SCALARF0_0: hprpin500312x10));

                                   end 
                                  if ((TESTMAIN400_test28_Main_V_7>=32'sh5))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14) if ((TESTMAIN400_test28_Main_V_6<32'sh5))  begin 
                              $display(" Test28 p1 i=%1d  s=%1d.", TESTMAIN400_test28_Main_V_6, $signed(32'sh3_0d40+((32'h3/*3:USA16*/==
                              A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_6])? A_SINT_CC_MAPR10NoCE3_SCALARF0_0: hprpin500304x10
                              )));
                               kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                               TESTMAIN400_test28_Main_V_5 <= $signed(32'sh3_0d40+((32'h3/*3:USA16*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_6
                              ])? A_SINT_CC_MAPR10NoCE3_SCALARF0_0: hprpin500304x10));

                               end 
                               else  begin 
                               kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                               TESTMAIN400_test28_Main_V_7 <= 32'sh0;
                               TESTMAIN400_test28_Main_V_5 <= 32'sh0;
                               end 
                              
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test28_Main_V_6 <= $signed(32'sd1+TESTMAIN400_test28_Main_V_6);
                           end 
                          endcase
              if (reset)  kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

 always   @(* )  begin 
       hpr_int_run_enable_DDX14 = 32'sd1;
       hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       bondout0_REQ0 = 32'd0;
       bondout0_ACKRDY0 = 32'd0;
       bondout0_ADDR0 = 32'd0;
       bondout0_RWBAR0 = 32'd0;
       bondout0_WDATA0 = 64'd0;
       bondout0_LANES0 = 32'd0;
       bondout0_REQ1 = 32'd0;
       bondout0_ACKRDY1 = 32'd0;
       bondout0_ADDR1 = 32'd0;
       bondout0_RWBAR1 = 32'd0;
       bondout0_WDATA1 = 64'd0;
       bondout0_LANES1 = 32'd0;
       bondout1_REQ0 = 32'd0;
       bondout1_ACKRDY0 = 32'd0;
       bondout1_ADDR0 = 32'd0;
       bondout1_RWBAR0 = 32'd0;
       bondout1_WDATA0 = 64'd0;
       bondout1_LANES0 = 32'd0;
       bondout1_REQ1 = 32'd0;
       bondout1_ACKRDY1 = 32'd0;
       bondout1_ADDR1 = 32'd0;
       bondout1_RWBAR1 = 32'd0;
       bondout1_WDATA1 = 64'd0;
       bondout1_LANES1 = 32'd0;
       end 
      

assign hprpin500304x10 = ((32'h2/*2:USA16*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_6])? A_SINT_CC_MAPR10NoCE2_SCALARF0_0: ((32'h1/*1:USA16*/==A_sA_SINT_CC_SCALbx12_ARA0
[TESTMAIN400_test28_Main_V_6])? A_SINT_CC_MAPR10NoCE1_SCALARF0_0: ((32'h0/*0:USA16*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_6
])? A_SINT_CC_MAPR10NoCE0_SCALARF0_0: 32'bx)));

assign hprpin500312x10 = ((32'h2/*2:USA14*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_7])? A_SINT_CC_MAPR10NoCE2_SCALARF0_0: ((32'h1/*1:USA14*/==A_sA_SINT_CC_SCALbx12_ARA0
[TESTMAIN400_test28_Main_V_7])? A_SINT_CC_MAPR10NoCE1_SCALARF0_0: ((32'h0/*0:USA14*/==A_sA_SINT_CC_SCALbx12_ARA0[TESTMAIN400_test28_Main_V_7
])? A_SINT_CC_MAPR10NoCE0_SCALARF0_0: 32'bx)));

// Structural Resource (FU) inventory for test28:// 1 vectors of width 3
// 7 vectors of width 32
// 1 vectors of width 1
// 5 array locations of width 64
// Total state bits in module = 548 bits.
// 64 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:46:36
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test28.exe -diosim-vcd=test28.vcd -sim=1800 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test28 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test28.Main | Main10 | false |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*

//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| AddressSpace | Name       | Protocol                      | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*

//----------------------------------------------------------

//Report from kiwife:::
//Enumeration codepoints for KiwiSystem.Kiwi.PauseControl
//*----------------------+------------+---*
//| Token                | Code point | p |
//*----------------------+------------+---*
//| autoPauseEnable      | 0          | 6 |
//| hardPauseEnable      | 1          | 6 |
//| softPauseEnable      | 2          | 6 |
//| maximalPauseEnable   | 3          | 6 |
//| bblockPauseEnable    | 4          | 6 |
//| pipelinedAccelerator | 5          | 6 |
//*----------------------+------------+---*

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//Bondout Port Settings
//
//
//*----------------------------------+-------+-------------*
//
//
//| Key                              | Value | Description |
//
//
//*----------------------------------+-------+-------------*
//
//
//| bondout-loadstore-port-count     | 1     |             |
//
//
//| bondout-loadstore-lane_addr-size | 22    |             |
//
//
//*----------------------------------+-------+-------------*
//
//
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class test28  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test28.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test28.Main
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//Report of all settings used from the recipe or command line:
//
//
//   bondout-schema=bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8
//
//
//   bondout-protocol=HFAST1
//
//
//   bondout-loadstore-lane-width=8
//
//
//   bondout-loadstore-port-lanes=32
//
//
//   bondout-loadstore-port-count=1
//
//
//   bondout-loadstore-simplex-ports=disable
//
//
//   bondout-loadstore-lane-addr-size=22
//
//
//   kiwife-directorate-pc-export=enable
//
//
//   kiwife-directorate-endmode=finish
//
//
//   kiwife-directorate-startmode=self-start
//
//
//   kiwic-default-dynamic-heapalloc-bytes=1073741824
//
//
//   cil-uwind-budget=10000
//
//
//   kiwic-cil-dump=disable
//
//
//   kiwic-kcode-dump=disable
//
//
//   kiwic-supress-zero-inits=disable
//
//
//   kiwife-dynpoly=disable
//
//
//   kiwic-library-redirects=enable
//
//
//   kiwic-register-colours=disable
//
//
//   array-4d-name=KIWIARRAY4D
//
//
//   array-3d-name=KIWIARRAY3D
//
//
//   array-2d-name=KIWIARRAY2D
//
//
//   kiwi-dll=Kiwi.dll
//
//
//   kiwic-dll=Kiwic.dll
//
//
//   kiwic-zerolength-arrays=disable
//
//
//   kiwifefpgaconsole-default=enable
//
//
//   kiwife-directorate-style=normal
//
//
//   kiwife-postgen-optimise=enable
//
//
//   kiwife-allow-hpr-alloc=enable
//
//
//   kiwife-filesearch-loglevel=3
//
//
//   kiwife-cil-loglevel=3
//
//
//   kiwife-ataken-loglevel=3
//
//
//   kiwife-gtrace-loglevel=3
//
//
//   kiwife-constvol-loglevel=3
//
//
//   kiwife-hgen-loglevel=3
//
//
//   kiwife-firstpass-loglevel=3
//
//
//   kiwife-overloads-loglevel=3
//
//
//   root=$attributeroot
//
//
//   srcfile=test28.exe
//
//
//   kiwic-autodispose=disable
//
//
//END OF KIWIC REPORT FILE
//
//

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Preliminary'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| Key                    | Value   | Description                                                                                                |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| int-flr-mul            | 1000    | Fixed-latency integer ALU integer latency scaling value for multiply.                                      |
//| max-no-fp-addsubs      | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread.                            |
//| max-no-fp-muls         | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.                                   |
//| max-no-int-muls        | 3       | Maximum number of int multipliers to instantiate per thread.                                               |
//| max-no-fp-divs         | 2       | Maximum number of f/p dividers to instantiate per thread.                                                  |
//| max-no-int-divs        | 2       | Maximum number of int dividers to instantiate per thread.                                                  |
//| max-no-rom-mirrors     | 8       | Maximum number of times to mirror a ROM per thread.                                                        |
//| max-ram-data_packing   | 8       | Maximum number of user words to pack into one RAM/loadstore word line.                                     |
//| fp-fl-dp-div           | 5       | Fixed-latency ALU floating-point, double-precision floating-point latency value for divide.                |
//| fp-fl-dp-add           | 4       | Fixed-latency ALU floating-point, double-precision floating-point latency value for add/sub.               |
//| fp-fl-dp-mul           | 3       | Fixed-latency ALU floating-point, double-precision floating-point latency value for multiply.              |
//| fp-fl-sp-div           | 15      | Fixed-latency ALU floating-point, single-precision floating-point floating-point latency value for divide. |
//| fp-fl-sp-add           | 4       | Fixed-latency ALU floating-point, single-precision floating-point latency value for add/sub.               |
//| fp-fl-sp-mul           | 5       | Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.              |
//| res2-offchip-threshold | 1000000 |                                                                                                            |
//| res2-combrom-threshold | 64      |                                                                                                            |
//| res2-combram-threshold | 32      |                                                                                                            |
//| res2-regfile-threshold | 8       |                                                                                                            |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN400PC10 
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"   | 815 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"   | 814 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 810 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 811 | 2       | hwm=0.0.0   | 2    |        | -     | -   | -    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 809 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 812 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 813 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 808 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 4    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                     |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                          |
//| F0   | E815 | R0 DATA |                                                                                                                                          |
//| F0+E | E815 | W0 DATA | @_SINT/CC/MAPR10NoCE0_SCALARF0_0write(S32'413) @_SINT/CC/MAPR10NoCE1_SCALARF0_0write(S32'313) @_SINT/CC/MAPR10NoCE2_SCALARF0_0write(S32\ |
//|      |      |         | '213) @_SINT/CC/MAPR10NoCE3_SCALARF0_0write(S32'13) TESTMAIN400.test28.Main.V_6write(S32'0) TESTMAIN400.test28.Main.V_5write(S32'0) TES\ |
//|      |      |         | TMAIN400.test28.Main.V_7write(S32'0)                                                                                                     |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                            |
//*------+------+---------+---------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                 |
//| F1   | E814 | R0 DATA |                                                                                 |
//| F1+E | E814 | W0 DATA | TESTMAIN400.test28.Main.V_6write(S32'0) TESTMAIN400.test28.Main.V_5write(S32'0) |
//*------+------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*------+------+---------+-------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                              |
//*------+------+---------+-------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                   |
//| F2   | E811 | R0 DATA |                                                                   |
//| F2+E | E811 | W0 DATA |  PLI:GSAI:hpr_sysexit                                             |
//| F2   | E810 | R0 DATA |                                                                   |
//| F2+E | E810 | W0 DATA | TESTMAIN400.test28.Main.V_5write(E1)  PLI: Test28 p2 i=%d  s=%... |
//*------+------+---------+-------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*------+------+---------+--------------------------------------*
//| pc   | eno  | Phaser  | Work                                 |
//*------+------+---------+--------------------------------------*
//| F3   | -    | R0 CTRL |                                      |
//| F3   | E809 | R0 DATA |                                      |
//| F3+E | E809 | W0 DATA | TESTMAIN400.test28.Main.V_7write(E2) |
//*------+------+---------+--------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                            |
//*------+------+---------+---------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                 |
//| F4   | E813 | R0 DATA |                                                                                 |
//| F4+E | E813 | W0 DATA | TESTMAIN400.test28.Main.V_5write(E3)  PLI: Test28 p1 i=%d  s=%...               |
//| F4   | E812 | R0 DATA |                                                                                 |
//| F4+E | E812 | W0 DATA | TESTMAIN400.test28.Main.V_5write(S32'0) TESTMAIN400.test28.Main.V_7write(S32'0) |
//*------+------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*------+------+---------+--------------------------------------*
//| pc   | eno  | Phaser  | Work                                 |
//*------+------+---------+--------------------------------------*
//| F5   | -    | R0 CTRL |                                      |
//| F5   | E808 | R0 DATA |                                      |
//| F5+E | E808 | W0 DATA | TESTMAIN400.test28.Main.V_6write(E4) |
//*------+------+---------+--------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout1 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Final'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(TESTMAIN400.test28.Main.V_5+(COND(XU32'3:"3:USA14"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_7], @_SINT/CC/MAPR10NoCE3_SCALARF0_0, COND(XU32'2:"2:USA14"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_7], @_SINT/CC/MAPR10NoCE2_SCALARF0_0, COND(XU32'1:"1:USA14"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_7], @_SINT/CC/MAPR10NoCE1_SCALARF0_0, COND(XU32'0:"0:USA14"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_7], @_SINT/CC/MAPR10NoCE0_SCALARF0_0, *UNDEF))))))
//
//
//  E2 =.= C(1+TESTMAIN400.test28.Main.V_7)
//
//
//  E3 =.= C(S32'200000+(COND(XU32'3:"3:USA16"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_6], @_SINT/CC/MAPR10NoCE3_SCALARF0_0, COND(XU32'2:"2:USA16"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_6], @_SINT/CC/MAPR10NoCE2_SCALARF0_0, COND(XU32'1:"1:USA16"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_6], @_SINT/CC/MAPR10NoCE1_SCALARF0_0, COND(XU32'0:"0:USA16"==@$s@_SINT/CC/SCALbx12_ARA0[TESTMAIN400.test28.Main.V_6], @_SINT/CC/MAPR10NoCE0_SCALARF0_0, *UNDEF))))))
//
//
//  E4 =.= C(1+TESTMAIN400.test28.Main.V_6)
//
//
//  E5 =.= TESTMAIN400.test28.Main.V_7>=S32'5
//
//
//  E6 =.= TESTMAIN400.test28.Main.V_7<S32'5
//
//
//  E7 =.= TESTMAIN400.test28.Main.V_6<S32'5
//
//
//  E8 =.= TESTMAIN400.test28.Main.V_6>=S32'5
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test28 to test28

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test28:
//1 vectors of width 3
//
//7 vectors of width 32
//
//1 vectors of width 1
//
//5 array locations of width 64
//
//Total state bits in module = 548 bits.
//
//64 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test28.Main uid=Main10 has 67 CIL instructions in 10 basic blocks
//Thread mpc10 has 6 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 6 minor control states
// eof (HPR L/S Verilog)
