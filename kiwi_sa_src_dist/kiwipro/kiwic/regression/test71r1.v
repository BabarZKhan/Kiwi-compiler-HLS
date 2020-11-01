

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:46
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test71r1.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test71r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwic-cil-dump=combined -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step
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
output reg [2:0] kiwiFINAMAIN4001PC10nz_pc_export);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] FINAMAIN400_FinallySimple_Main_V_0;
// abstractionName=res2-morenets
  reg [2:0] kiwiFINAMAIN4001PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.FINAMAIN400_1/1.0
      if (reset)  begin 
               kiwiFINAMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               FINAMAIN400_FinallySimple_Main_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiFINAMAIN4001PC10nz)
                  32'h0/*0:kiwiFINAMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiFINAMAIN4001PC10nz <= 32'h1/*1:kiwiFINAMAIN4001PC10nz*/;
                           done <= 32'h0;
                           FINAMAIN400_FinallySimple_Main_V_0 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiFINAMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Start FinallySimple71r1 Test");
                           kiwiFINAMAIN4001PC10nz <= 32'h2/*2:kiwiFINAMAIN4001PC10nz*/;
                           end 
                          
                  32'h2/*2:kiwiFINAMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("FinallySimple71r1 Test: Enter Top Try");
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try A pp=%1d", 32'sh0);
                          $display("FinallySimple71r1 Test: Inner Finally A. pp=%1d", 32'sh0);
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try B. pp=%1d", 32'sh1);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh1, 32'sh2a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh1, 32'sh3a2);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh1, 32'sh71a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh1, 32'sha92);
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try A pp=%1d", 32'sh2);
                          $display("FinallySimple71r1 Test: Inner Finally A. pp=%1d", 32'sh2);
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try B. pp=%1d", 32'sh3);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh3, 32'sh2a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh3, 32'sh3a2);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh3, 32'sh71a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh3, 32'sha92);
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try A pp=%1d", 32'sh4);
                          $display("FinallySimple71r1 Test: Inner Finally A. pp=%1d", 32'sh4);
                          $display("\nFinallySimple71r1 Test: Enter Loop Body");
                          $display("FinallySimple71r1 Test: Inner Try B. pp=%1d", 32'sh5);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh5, 32'sh2a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh5, 32'sh3a2);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh5, 32'sh71a);
                          $display("FinallySimple71r1 Test: Inner Finally B. pp=%1d qqq=%1d", 32'sh5, 32'sha92);
                          $display("FinallySimple71r1 Test: Final handler");
                           kiwiFINAMAIN4001PC10nz <= 32'h3/*3:kiwiFINAMAIN4001PC10nz*/;
                           FINAMAIN400_FinallySimple_Main_V_0 <= 32'sh1f9;
                           end 
                          
                  32'h3/*3:kiwiFINAMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("FinallySimple71r1 Test: End of Demo. rc=%1d", FINAMAIN400_FinallySimple_Main_V_0);
                           kiwiFINAMAIN4001PC10nz <= 32'h4/*4:kiwiFINAMAIN4001PC10nz*/;
                           end 
                          
                  32'h4/*4:kiwiFINAMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           kiwiFINAMAIN4001PC10nz <= 32'h5/*5:kiwiFINAMAIN4001PC10nz*/;
                           done <= 32'h1;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  kiwiFINAMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX16)  kiwiFINAMAIN4001PC10nz_pc_export <= kiwiFINAMAIN4001PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.FINAMAIN400_1/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 3
// 1 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 36 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:44
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test71r1.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test71r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwic-cil-dump=combined -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*---------------+---------+------------------------------------------------------------------------------------------------------+---------------+--------------------+--------+-------*
//| Class         | Style   | Dir Style                                                                                            | Timing Target | Method             | UID    | Skip  |
//*---------------+---------+------------------------------------------------------------------------------------------------------+---------------+--------------------+--------+-------*
//| FinallySimple | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | FinallySimple.Main | Main10 | false |
//*---------------+---------+------------------------------------------------------------------------------------------------------+---------------+--------------------+--------+-------*

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
//KiwiC: front end input processing of class FinallySimple  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=FinallySimple..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=FinallySimple..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class FinallySimple  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=FinallySimple.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=FinallySimple.Main
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
//   kiwic-kcode-dump=early
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
//   kiwife-gtrace-loglevel=0
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
//   srcfile=test71r1.exe
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
//PC codings points for kiwiFINAMAIN4001PC10 
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                   | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiFINAMAIN4001PC10" | 845 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiFINAMAIN4001PC10" | 844 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiFINAMAIN4001PC10" | 843 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiFINAMAIN4001PC10" | 842 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiFINAMAIN4001PC10" | 841 | 4       | hwm=0.0.0   | 4    |        | -     | -   | -    |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'0:"0:kiwiFINAMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'0:"0:kiwiFINAMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                            |
//*------+------+---------+-----------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                 |
//| F0   | E845 | R0 DATA |                                                                 |
//| F0+E | E845 | W0 DATA | FINAMAIN400.FinallySimple.Main.V_0write(S32'0) donewrite(U32'0) |
//*------+------+---------+-----------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'1:"1:kiwiFINAMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'1:"1:kiwiFINAMAIN4001PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F1   | -    | R0 CTRL |                              |
//| F1   | E844 | R0 DATA |                              |
//| F1+E | E844 | W0 DATA |  PLI:Start FinallySimple7... |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'2:"2:kiwiFINAMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'2:"2:kiwiFINAMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                       |
//*------+------+---------+------------------------------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                                            |
//| F2   | E843 | R0 DATA |                                                                                                            |
//| F2+E | E843 | W0 DATA | FINAMAIN400.FinallySimple.Main.V_0write(S32'505)  PLI:FinallySimple71r1 Te...  PLI:
//FinallySimple71r1 T... |
//*------+------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'4:"4:kiwiFINAMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'4:"4:kiwiFINAMAIN4001PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F3   | -    | R0 CTRL |                              |
//| F3   | E842 | R0 DATA |                              |
//| F3+E | E842 | W0 DATA |  PLI:FinallySimple71r1 Te... |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'8:"8:kiwiFINAMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiFINAMAIN4001PC10 state=XU32'8:"8:kiwiFINAMAIN4001PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F4   | -    | R0 CTRL |                                        |
//| F4   | E841 | R0 DATA |                                        |
//| F4+E | E841 | W0 DATA | donewrite(U32'1)  PLI:GSAI:hpr_sysexit |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  -- No expression aliases to report
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test71r1 to test71r1

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 3
//
//1 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 36 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread FinallySimple..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread FinallySimple.Main uid=Main10 has 75 CIL instructions in 30 basic blocks
//Thread mpc10 has 5 bevelab control states (pauses)
//Reindexed thread kiwiFINAMAIN4001PC10 with 5 minor control states
// eof (HPR L/S Verilog)
