

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:47:08
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test31.exe -diosim-vcd=test31.vcd -sim=1800 -give-backtrace -report-each-step
`timescale 1ns/1ns


module test31(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiTESTMAIN400PC10nz_pc_export,
    
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
  reg signed [31:0] TESTMAIN400_test31_Main_V_1;
  reg signed [31:0] TESTMAIN400_test31_Main_V_0;
// abstractionName=res2-morenets
  reg [2:0] kiwiTESTMAIN400PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test31_Main_V_0 <= 32'd0;
               TESTMAIN400_test31_Main_V_1 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test31_Main_V_0 <= 32'sh0;
                           TESTMAIN400_test31_Main_V_1 <= 32'sh4d2;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test31_Main_V_1 <= 32'sh2710;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (((32'sd5==TESTMAIN400_test31_Main_V_0)? 1'd1: (32'sd6!=TESTMAIN400_test31_Main_V_0) && (32'sd7==TESTMAIN400_test31_Main_V_0
                          )) && (32'sd4!=TESTMAIN400_test31_Main_V_0) && (32'sd3!=TESTMAIN400_test31_Main_V_0) && (32'sd2!=TESTMAIN400_test31_Main_V_0
                          ) && (32'sd1!=TESTMAIN400_test31_Main_V_0))  begin 
                                  if (((32'sd5==TESTMAIN400_test31_Main_V_0)? 1'd1: (32'sd6!=TESTMAIN400_test31_Main_V_0) && (32'sd7==
                                  TESTMAIN400_test31_Main_V_0)) && (32'sd4!=TESTMAIN400_test31_Main_V_0) && (32'sd3!=TESTMAIN400_test31_Main_V_0
                                  ) && (32'sd2!=TESTMAIN400_test31_Main_V_0) && (32'sd1!=TESTMAIN400_test31_Main_V_0))  TESTMAIN400_test31_Main_V_1
                                       <= ((32'sd1!=TESTMAIN400_test31_Main_V_0) && (32'sd2!=TESTMAIN400_test31_Main_V_0) && (32'sd3!=
                                      TESTMAIN400_test31_Main_V_0) && (32'sd4!=TESTMAIN400_test31_Main_V_0) && (32'sd5==TESTMAIN400_test31_Main_V_0
                                      )? 32'sh3e8: 32'sh64);

                                       kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                  if (((32'sd1==TESTMAIN400_test31_Main_V_0)? 1'd1: (32'sd2!=TESTMAIN400_test31_Main_V_0) && (32'sd3!=
                          TESTMAIN400_test31_Main_V_0) && (32'sd4==TESTMAIN400_test31_Main_V_0)))  begin 
                                  if ((32'sd1==TESTMAIN400_test31_Main_V_0))  TESTMAIN400_test31_Main_V_1 <= 32'sha;
                                       kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                  if ((32'sd2==TESTMAIN400_test31_Main_V_0) || (32'sd6==TESTMAIN400_test31_Main_V_0))  kiwiTESTMAIN400PC10nz
                               <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;

                              if (((32'sd3==TESTMAIN400_test31_Main_V_0)? 1'd1: (32'sd1!=TESTMAIN400_test31_Main_V_0) && (32'sd2!=TESTMAIN400_test31_Main_V_0
                          ) && (32'sd4!=TESTMAIN400_test31_Main_V_0) && (32'sd5!=TESTMAIN400_test31_Main_V_0) && (32'sd6!=TESTMAIN400_test31_Main_V_0
                          ) && (32'sd7!=TESTMAIN400_test31_Main_V_0)))  kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                               end 
                          
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((32'sd44!=TESTMAIN400_test31_Main_V_0) && (32'sd102!=TESTMAIN400_test31_Main_V_0))  begin 
                                  $display("%1d not set", 32'sd1234);
                                   kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                  if ((32'sd44==TESTMAIN400_test31_Main_V_0) || (32'sd102==TESTMAIN400_test31_Main_V_0))  kiwiTESTMAIN400PC10nz
                               <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;

                               end 
                          
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test31_Main_V_0)>=32'sd5))  begin 
                                  $display("%1d -> %1d", TESTMAIN400_test31_Main_V_0, TESTMAIN400_test31_Main_V_1);
                                  $display(" Test31 complete");
                                  $finish(32'sd0);
                                   end 
                                   else  begin 
                                  $display("%1d -> %1d", TESTMAIN400_test31_Main_V_0, TESTMAIN400_test31_Main_V_1);
                                   kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test31_Main_V_0 <= $signed(32'sd1+TESTMAIN400_test31_Main_V_0);
                                   TESTMAIN400_test31_Main_V_1 <= 32'sh4d2;
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_test31_Main_V_0)>=32'sd5))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test31_Main_V_0 <= $signed(32'sd1+TESTMAIN400_test31_Main_V_0);
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test31_Main_V_1 <= 32'sh14;
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
      

// Structural Resource (FU) inventory for test31:// 1 vectors of width 3
// 2 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 68 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:47:05
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test31.exe -diosim-vcd=test31.vcd -sim=1800 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test31 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test31.Main | Main10 | false |
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
//KiwiC: front end input processing of class test31  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test31.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test31.Main
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
//   srcfile=test31.exe
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
//| XU32'0:"0:kiwiTESTMAIN400PC10"   | 823 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 3    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 818 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 819 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 5    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 820 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 4    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 821 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 6    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 817 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 5    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"   | 822 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 1    |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 813 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 814 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 6    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 815 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 3    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 816 | 5       | hwm=0.0.0   | 5    |        | -     | -   | -    |
//| XU32'32:"32:kiwiTESTMAIN400PC10" | 812 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 5    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                               |
//*------+------+---------+------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                    |
//| F0   | E823 | R0 DATA |                                                                                    |
//| F0+E | E823 | W0 DATA | TESTMAIN400.test31.Main.V_1write(S32'1234) TESTMAIN400.test31.Main.V_0write(S32'0) |
//*------+------+---------+------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------*
//| pc   | eno  | Phaser  | Work                                     |
//*------+------+---------+------------------------------------------*
//| F1   | -    | R0 CTRL |                                          |
//| F1   | E821 | R0 DATA |                                          |
//| F1+E | E821 | W0 DATA |                                          |
//| F1   | E820 | R0 DATA |                                          |
//| F1+E | E820 | W0 DATA |                                          |
//| F1   | E819 | R0 DATA |                                          |
//| F1+E | E819 | W0 DATA | TESTMAIN400.test31.Main.V_1write(E1)     |
//| F1   | E818 | R0 DATA |                                          |
//| F1+E | E818 | W0 DATA | TESTMAIN400.test31.Main.V_1write(S32'10) |
//*------+------+---------+------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------*
//| pc   | eno  | Phaser  | Work                                        |
//*------+------+---------+---------------------------------------------*
//| F2   | -    | R0 CTRL |                                             |
//| F2   | E817 | R0 DATA |                                             |
//| F2+E | E817 | W0 DATA | TESTMAIN400.test31.Main.V_1write(S32'10000) |
//*------+------+---------+---------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F3   | -    | R0 CTRL |      |
//| F3   | E822 | R0 DATA |      |
//| F3+E | E822 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*------+------+---------+-----------------*
//| pc   | eno  | Phaser  | Work            |
//*------+------+---------+-----------------*
//| F4   | -    | R0 CTRL |                 |
//| F4   | E814 | R0 DATA |                 |
//| F4+E | E814 | W0 DATA |                 |
//| F4   | E813 | R0 DATA |                 |
//| F4+E | E813 | W0 DATA |  PLI:%d not set |
//*------+------+---------+-----------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                           |
//*------+------+---------+------------------------------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                                                |
//| F5   | E816 | R0 DATA |                                                                                                |
//| F5+E | E816 | W0 DATA | TESTMAIN400.test31.Main.V_0write(E2)  PLI:GSAI:hpr_sysexit  PLI: Test31 complete  PLI:%d -> %d |
//| F5   | E815 | R0 DATA |                                                                                                |
//| F5+E | E815 | W0 DATA | TESTMAIN400.test31.Main.V_1write(S32'1234) TESTMAIN400.test31.Main.V_0write(E2)  PLI:%d -> %d  |
//*------+------+---------+------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------*
//| pc   | eno  | Phaser  | Work                                     |
//*------+------+---------+------------------------------------------*
//| F6   | -    | R0 CTRL |                                          |
//| F6   | E812 | R0 DATA |                                          |
//| F6+E | E812 | W0 DATA | TESTMAIN400.test31.Main.V_1write(S32'20) |
//*------+------+---------+------------------------------------------*

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
//  E1 =.= COND({[1!=TESTMAIN400.test31.Main.V_0, 2!=TESTMAIN400.test31.Main.V_0, 3!=TESTMAIN400.test31.Main.V_0, 4!=TESTMAIN400.test31.Main.V_0, 5==TESTMAIN400.test31.Main.V_0]}, S32'1000, S32'100)
//
//
//  E2 =.= C(1+TESTMAIN400.test31.Main.V_0)
//
//
//  E3 =.= {[6==TESTMAIN400.test31.Main.V_0]; [2==TESTMAIN400.test31.Main.V_0]}
//
//
//  E4 =.= {[1!=TESTMAIN400.test31.Main.V_0, 2!=TESTMAIN400.test31.Main.V_0, 3!=TESTMAIN400.test31.Main.V_0, 4!=TESTMAIN400.test31.Main.V_0, 5!=TESTMAIN400.test31.Main.V_0, 6!=TESTMAIN400.test31.Main.V_0, 7!=TESTMAIN400.test31.Main.V_0]; [3==TESTMAIN400.test31.Main.V_0]}
//
//
//  E5 =.= {[1!=TESTMAIN400.test31.Main.V_0, 2!=TESTMAIN400.test31.Main.V_0, 3!=TESTMAIN400.test31.Main.V_0, 4!=TESTMAIN400.test31.Main.V_0, 5==TESTMAIN400.test31.Main.V_0]; [1!=TESTMAIN400.test31.Main.V_0, 2!=TESTMAIN400.test31.Main.V_0, 3!=TESTMAIN400.test31.Main.V_0, 4!=TESTMAIN400.test31.Main.V_0, 5!=TESTMAIN400.test31.Main.V_0, 6!=TESTMAIN400.test31.Main.V_0, 7==TESTMAIN400.test31.Main.V_0]}
//
//
//  E6 =.= {[1==TESTMAIN400.test31.Main.V_0]; [1!=TESTMAIN400.test31.Main.V_0, 2!=TESTMAIN400.test31.Main.V_0, 3!=TESTMAIN400.test31.Main.V_0, 4==TESTMAIN400.test31.Main.V_0]}
//
//
//  E7 =.= {[102==TESTMAIN400.test31.Main.V_0]; [44==TESTMAIN400.test31.Main.V_0]}
//
//
//  E8 =.= {[44!=TESTMAIN400.test31.Main.V_0, 102!=TESTMAIN400.test31.Main.V_0]}
//
//
//  E9 =.= (C(1+TESTMAIN400.test31.Main.V_0))>=5
//
//
//  E10 =.= (C(1+TESTMAIN400.test31.Main.V_0))<5
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test31 to test31

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test31:
//1 vectors of width 3
//
//2 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 68 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test31.Main uid=Main10 has 43 CIL instructions in 21 basic blocks
//Thread mpc10 has 7 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 7 minor control states
// eof (HPR L/S Verilog)
