

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:36
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test68.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test68.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiBENCMAIN4001PC10nz_pc_export);
function [7:0] hpr_toChar0;
input [31:0] hpr_toChar0_a;
   hpr_toChar0 = hpr_toChar0_a & 8'hff;
endfunction


function signed [15:0] rtl_signed_bitextract2;
   input [31:0] arg;
   rtl_signed_bitextract2 = $signed(arg[15:0]);
   endfunction


function signed [31:0] rtl_sign_extend1;
   input [15:0] arg;
   rtl_sign_extend1 = { {16{arg[15]}}, arg[15:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] BENCMAIN400_bench_Main_V_0;
  reg signed [15:0] bench_c2;
  reg signed [15:0] bench_c1;
  reg signed [31:0] bench_a2;
  reg signed [31:0] bench_a1;
// abstractionName=res2-morenets
  reg [1:0] kiwiBENCMAIN4001PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.BENCMAIN400_1/1.0
      if (reset)  begin 
               kiwiBENCMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               bench_a1 <= 32'd0;
               bench_a2 <= 32'd0;
               bench_c1 <= 32'd0;
               bench_c2 <= 32'd0;
               BENCMAIN400_bench_Main_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiBENCMAIN4001PC10nz)
                  32'h0/*0:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test68  starting.");
                           kiwiBENCMAIN4001PC10nz <= 32'h1/*1:kiwiBENCMAIN4001PC10nz*/;
                           done <= 32'h0;
                           bench_a1 <= 32'sh3;
                           bench_a2 <= 32'sh4;
                           bench_c1 <= 32'sh79;
                           bench_c2 <= 32'sh78;
                           BENCMAIN400_bench_Main_V_0 <= 32'sh3;
                           end 
                          
                  32'h1/*1:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(-32'sd1+BENCMAIN400_bench_Main_V_0)<32'sd0))  begin 
                                  $display("   Swapsies %1d  %1d  %c", BENCMAIN400_bench_Main_V_0, bench_a2, hpr_toChar0(((32'sd1==BENCMAIN400_bench_Main_V_0
                                  )? rtl_sign_extend1(rtl_signed_bitextract2(bench_c2)): bench_c1)));
                                  $display("Test68 finished.");
                                   end 
                                   else $display("   Swapsies %1d  %1d  %c", BENCMAIN400_bench_Main_V_0, bench_a2, hpr_toChar0(((32'sd1
                              ==BENCMAIN400_bench_Main_V_0)? rtl_sign_extend1(rtl_signed_bitextract2(bench_c2)): bench_c1)));
                          if (($signed(-32'sd1+BENCMAIN400_bench_Main_V_0)<32'sd0))  begin 
                                  if ((32'sd1==BENCMAIN400_bench_Main_V_0))  begin 
                                           bench_c1 <= rtl_sign_extend1(rtl_signed_bitextract2(bench_c2));
                                           bench_c2 <= rtl_sign_extend1(rtl_signed_bitextract2(rtl_sign_extend1(rtl_signed_bitextract2(bench_c1
                                          ))));

                                           end 
                                           kiwiBENCMAIN4001PC10nz <= 32'h2/*2:kiwiBENCMAIN4001PC10nz*/;
                                   done <= 32'h1;
                                   bench_a1 <= bench_a2;
                                   bench_a2 <= bench_a1;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(-32'sd1+BENCMAIN400_bench_Main_V_0);
                                   end 
                                   else  begin 
                                  if ((32'sd1==BENCMAIN400_bench_Main_V_0))  begin 
                                           bench_c1 <= rtl_sign_extend1(rtl_signed_bitextract2(bench_c2));
                                           bench_c2 <= rtl_sign_extend1(rtl_signed_bitextract2(rtl_sign_extend1(rtl_signed_bitextract2(bench_c1
                                          ))));

                                           end 
                                           bench_a1 <= bench_a2;
                                   bench_a2 <= bench_a1;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(-32'sd1+BENCMAIN400_bench_Main_V_0);
                                   end 
                                   end 
                          
                  32'h2/*2:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           kiwiBENCMAIN4001PC10nz <= 32'h3/*3:kiwiBENCMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  kiwiBENCMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX16)  kiwiBENCMAIN4001PC10nz_pc_export <= kiwiBENCMAIN4001PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.BENCMAIN400_1/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 2
// 3 vectors of width 32
// 2 vectors of width 16
// 1 vectors of width 1
// Total state bits in module = 131 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:33
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test68.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test68.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+------------+--------+-------*
//| Class | Style   | Dir Style                                                                                            | Timing Target | Method     | UID    | Skip  |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+------------+--------+-------*
//| bench | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | bench.Main | Main10 | false |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+------------+--------+-------*

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
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class bench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=bench..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=bench..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class bench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=bench.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=bench.Main
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
//   kiwic-cil-dump=combined
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
//   srcfile=test68.exe
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
//PC codings points for kiwiBENCMAIN4001PC10 
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                   | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiBENCMAIN4001PC10" | 807 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiBENCMAIN4001PC10" | 805 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiBENCMAIN4001PC10" | 806 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiBENCMAIN4001PC10" | 804 | 2       | hwm=0.0.0   | 2    |        | -     | -   | -    |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                |
//| F0   | E807 | R0 DATA |                                                                                                                |
//| F0+E | E807 | W0 DATA | BENCMAIN400.bench.Main.V_0write(S32'3) bench.c2write(S32'120) bench.c1write(S32'121) bench.a2write(S32'4) ben\ |
//|      |      |         | ch.a1write(S32'3) donewrite(U32'0)  PLI:Test68  starting.                                                      |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                     |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                                          |
//| F1   | E806 | R0 DATA |                                                                                                                                                          |
//| F1+E | E806 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E1) bench.c2write(C(C16(C(C16(bench.c1))))) bench.c1write(C(C16(bench.c2))) bench.a2write(C(bench.a1)) bench.a1write(C(\ |
//|      |      |         | bench.a2)) donewrite(U32'1)  PLI:Test68 finished.  PLI:   Swapsies %d  %d  ...                                                                           |
//| F1   | E805 | R0 DATA |                                                                                                                                                          |
//| F1+E | E805 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E1) bench.c2write(C(C16(C(C16(bench.c1))))) bench.c1write(C(C16(bench.c2))) bench.a2write(C(bench.a1)) bench.a1write(C(\ |
//|      |      |         | bench.a2))  PLI:   Swapsies %d  %d  ...                                                                                                                  |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------*
//| pc   | eno  | Phaser  | Work                  |
//*------+------+---------+-----------------------*
//| F2   | -    | R0 CTRL |                       |
//| F2   | E804 | R0 DATA |                       |
//| F2+E | E804 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(-1+BENCMAIN400.bench.Main.V_0)
//
//
//  E2 =.= (C(-1+BENCMAIN400.bench.Main.V_0))<0
//
//
//  E3 =.= (C(-1+BENCMAIN400.bench.Main.V_0))>=0
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test68 to test68

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 2
//
//3 vectors of width 32
//
//2 vectors of width 16
//
//1 vectors of width 1
//
//Total state bits in module = 131 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread bench..cctor uid=cctor10 has 5 CIL instructions in 1 basic blocks
//Thread bench.Main uid=Main10 has 24 CIL instructions in 7 basic blocks
//Thread mpc10 has 3 bevelab control states (pauses)
//Reindexed thread kiwiBENCMAIN4001PC10 with 3 minor control states
// eof (HPR L/S Verilog)
