

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:36
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test51b.exe -sim 10 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=kiwicmiscio10 */
input signed [15:0] uword128_in_bopper,
    input [63:0] uword128_in_word0,
    input [63:0] uword128_in_word1,
    output signed [15:0] uword128_out_bopper,
    output reg [63:0] uword128_out_word0,
    output reg [63:0] uword128_out_word1,
    output reg [63:0] normal,
    
/* portgroup= abstractionName=res2-directornets */
output reg [1:0] kiwiWIDEMAIN400PC10nz_pc_export);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=res2-morenets
  reg [1:0] kiwiWIDEMAIN400PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX14 = 32'sd1;
       hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.WIDEMAIN400/1.0
      if (reset)  begin 
               kiwiWIDEMAIN400PC10nz <= 32'd0;
               normal <= 64'd0;
               uword128_out_word0 <= 64'd0;
               uword128_out_word1 <= 64'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiWIDEMAIN400PC10nz)
                  32'h0/*0:kiwiWIDEMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Starting widebus1 incrementer.");
                           kiwiWIDEMAIN400PC10nz <= 32'h1/*1:kiwiWIDEMAIN400PC10nz*/;
                           normal <= 64'h4;
                           uword128_out_word0 <= $unsigned(64'sh1+uword128_in_word0);
                           uword128_out_word1 <= $unsigned(((64'sh0==uword128_in_word0)? 64'sh1: 64'sh0)+uword128_in_word1);
                           end 
                          
                  32'h1/*1:kiwiWIDEMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           normal <= 64'h4;
                           uword128_out_word0 <= $unsigned(64'sh1+uword128_in_word0);
                           uword128_out_word1 <= $unsigned(((64'sh0==uword128_in_word0)? 64'sh1: 64'sh0)+uword128_in_word1);
                           end 
                          endcase
              if (reset)  kiwiWIDEMAIN400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX14)  kiwiWIDEMAIN400PC10nz_pc_export <= kiwiWIDEMAIN400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.WIDEMAIN400/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 2
// 1 vectors of width 1
// Total state bits in module = 3 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:34
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test51b.exe -sim 10 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------------------------+---------+--------------------------------------------------------------------+---------------+-------------------------------+--------+-------*
//| Class                    | Style   | Dir Style                                                          | Timing Target | Method                        | UID    | Skip  |
//*--------------------------+---------+--------------------------------------------------------------------+---------------+-------------------------------+--------+-------*
//| WideWordDemoUsingStructs | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | WideWordDemoUsingStructs.Main | Main10 | false |
//|                          |         | dmode, enable/directorate-pc-export                                |               |                               |        |       |
//*--------------------------+---------+--------------------------------------------------------------------+---------------+-------------------------------+--------+-------*

//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports = Nothing to Report
//

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
//| bondout-loadstore-port-count     | 0     |             |
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
//KiwiC: front end input processing of class WideWordDemoUsingStructs  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=WideWordDemoUsingStructs.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=WideWordDemoUsingStructs.Main
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
//   bondout-loadstore-port-count=0
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
//   srcfile=test51b.exe
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
//PC codings points for kiwiWIDEMAIN400PC10 
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                  | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiWIDEMAIN400PC10" | 803 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiWIDEMAIN400PC10" | 802 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 1    |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiWIDEMAIN400PC10 state=XU32'0:"0:kiwiWIDEMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiWIDEMAIN400PC10 state=XU32'0:"0:kiwiWIDEMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                    |
//*------+------+---------+---------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                         |
//| F0   | E803 | R0 DATA | uword128_in_word0argread(<NONE>) uword128_in_word1argread(<NONE>)                                       |
//| F0+E | E803 | W0 DATA | uword128_out_word1write(E1) uword128_out_word0write(E2) normalwrite(U64'4)  PLI:Starting widebus1 in... |
//*------+------+---------+---------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiWIDEMAIN400PC10 state=XU32'1:"1:kiwiWIDEMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiWIDEMAIN400PC10 state=XU32'1:"1:kiwiWIDEMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                       |
//*------+------+---------+----------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                            |
//| F1   | E802 | R0 DATA | uword128_in_word0argread(<NONE>) uword128_in_word1argread(<NONE>)          |
//| F1+E | E802 | W0 DATA | uword128_out_word1write(E1) uword128_out_word0write(E2) normalwrite(U64'4) |
//*------+------+---------+----------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C64u((COND(S64'0==uword128_in_word0, S64'1, S64'0))+(C64u(uword128_in_word1)))
//
//
//  E2 =.= C64u(S64'1+uword128_in_word0)
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test51b to test51b

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 2
//
//1 vectors of width 1
//
//Total state bits in module = 3 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread WideWordDemoUsingStructs.Main uid=Main10 has 29 CIL instructions in 6 basic blocks
//Thread mpc10 has 2 bevelab control states (pauses)
//Reindexed thread kiwiWIDEMAIN400PC10 with 2 minor control states
// eof (HPR L/S Verilog)
