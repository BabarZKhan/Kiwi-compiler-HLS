

// CBG Orangepath HPR L/S System

// Verilog output file generated at 01/10/2017 07:39:33
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.2h : 25th Sept 2017 Linux/X86_64:koo
//  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -repack-to-roms=enable -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test70.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test70.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] bevelab10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg port_iname=noname */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir port_iname=noname */
input reset);
// abstractionName=kiwicmainnets10
  integer T402_UncaughtExceptionTest_runner_1_4_V_0;
  integer bench_T402_bench_T402_bench_V_1;
// abstractionName=res2-morenets
  reg [2:0] bevelab10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test70/1.0
      if (reset)  begin 
               bevelab10nz_pc_export <= 32'd0;
               done <= 32'd0;
               T402_UncaughtExceptionTest_runner_1_4_V_0 <= 32'd0;
               bench_T402_bench_T402_bench_V_1 <= 32'd0;
               bevelab10nz <= 32'd0;
               end 
               else  begin 
              
              case (bevelab10nz)
                  32'h0/*0:bevelab10nz*/:  begin 
                      $display("Test70  starting.");
                       done <= 1'h0;
                       T402_UncaughtExceptionTest_runner_1_4_V_0 <= 32'sh0;
                       bench_T402_bench_T402_bench_V_1 <= 32'sd3;
                       bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                       end 
                      
                  32'h1/*1:bevelab10nz*/:  begin 
                       T402_UncaughtExceptionTest_runner_1_4_V_0 <= 32'sd0;
                       bevelab10nz <= 32'h2/*2:bevelab10nz*/;
                       end 
                      
                  32'h2/*2:bevelab10nz*/:  begin 
                      if ((T402_UncaughtExceptionTest_runner_1_4_V_0>=32'sd9) && (bench_T402_bench_T402_bench_V_1<32'sd1))  begin 
                              $display(" runner %1d", T402_UncaughtExceptionTest_runner_1_4_V_0);
                              if ((32'sd5==T402_UncaughtExceptionTest_runner_1_4_V_0)) $finish(32'sd101+T402_UncaughtExceptionTest_runner_1_4_V_0
                                  );
                                  $display("Test70 finished.");
                               end 
                              if ((T402_UncaughtExceptionTest_runner_1_4_V_0<32'sd9))  begin 
                              $display(" runner %1d", T402_UncaughtExceptionTest_runner_1_4_V_0);
                              if ((32'sd5==T402_UncaughtExceptionTest_runner_1_4_V_0)) $finish(32'sd101+T402_UncaughtExceptionTest_runner_1_4_V_0
                                  );
                                   end 
                              if ((T402_UncaughtExceptionTest_runner_1_4_V_0>=32'sd9) && (bench_T402_bench_T402_bench_V_1>=32'sd1))  begin 
                              $display(" runner %1d", T402_UncaughtExceptionTest_runner_1_4_V_0);
                              if ((32'sd5==T402_UncaughtExceptionTest_runner_1_4_V_0)) $finish(32'sd101+T402_UncaughtExceptionTest_runner_1_4_V_0
                                  );
                                   end 
                              if ((T402_UncaughtExceptionTest_runner_1_4_V_0>=32'sd9) && (bench_T402_bench_T402_bench_V_1<32'sd1))  begin 
                               done <= 1'h1;
                               T402_UncaughtExceptionTest_runner_1_4_V_0 <= 32'sd1+T402_UncaughtExceptionTest_runner_1_4_V_0;
                               bench_T402_bench_T402_bench_V_1 <= -32'sd1+bench_T402_bench_T402_bench_V_1;
                               bevelab10nz <= 32'h3/*3:bevelab10nz*/;
                               end 
                              if ((T402_UncaughtExceptionTest_runner_1_4_V_0>=32'sd9) && (bench_T402_bench_T402_bench_V_1>=32'sd1))  begin 
                               bench_T402_bench_T402_bench_V_1 <= -32'sd1+bench_T402_bench_T402_bench_V_1;
                               bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                               end 
                              if ((T402_UncaughtExceptionTest_runner_1_4_V_0<32'sd9))  bevelab10nz <= 32'h2/*2:bevelab10nz*/;
                          if (((T402_UncaughtExceptionTest_runner_1_4_V_0<32'sd9)? 1'd1: (bench_T402_bench_T402_bench_V_1>=32'sd1))) 
                       T402_UncaughtExceptionTest_runner_1_4_V_0 <= 32'sd1+T402_UncaughtExceptionTest_runner_1_4_V_0;
                           end 
                      
                  32'h3/*3:bevelab10nz*/:  begin 
                      $finish(32'sd0);
                       bevelab10nz <= 32'h4/*4:bevelab10nz*/;
                       end 
                      endcase
               bevelab10nz_pc_export <= bevelab10nz;
               end 
              //End structure HPR test70/1.0


       end 
      

// 1 vectors of width 3
// 64 bits in scalar variables
// Total state bits in module = 67 bits.
// Total number of leaf cells = 0
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.2h : 25th Sept 2017
//01/10/2017 07:39:30
//Cmd line args:  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -repack-to-roms=enable -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test70.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test70.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called bench
//
//root_walk start thread at a static method (used as an entry point). Method name=bench/.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) id=cctor12
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called UncaughtExceptionTest
//
//root_walk start thread at a static method (used as an entry point). Method name=UncaughtExceptionTest/.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) id=cctor10
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called bench
//
//root_compiler: start elaborating class 'bench'
//
//elaborating class 'bench'
//
//compiling static method as entry point: style=Root idl=bench/bench
//
//Performing root elaboration of method bench
//
//KiwiC start_thread (or entry point) id=Main10
//
//root_compiler class done: bench
//
//Report of all settings used from the recipe or command line:
//
//   kiwife-directorate-ready-flag=absent
//
//   kiwife-directorate-endmode=finish
//
//   kiwife-directorate-startmode=self-start
//
//   cil-uwind-budget=10000
//
//   kiwic-cil-dump=combined
//
//   kiwic-kcode-dump=disable
//
//   kiwic-register-colours=disable
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
//   kiwifefpgaconsole-default=enable
//
//   kiwife-directorate-style=basic
//
//   postgen-optimise=enable
//
//   kiwife-cil-loglevel=20
//
//   kiwife-ataken-loglevel=20
//
//   kiwife-gtrace-loglevel=20
//
//   kiwife-firstpass-loglevel=20
//
//   kiwife-overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=test70.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int-flr-mul               | -3000   |                                                                                 |
//| max-no-fp-addsubs         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls            | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors        | 8       | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing      | 8       | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div              | 5       |                                                                                 |
//| fp-fl-dp-add              | 4       |                                                                                 |
//| fp-fl-dp-mul              | 3       |                                                                                 |
//| fp-fl-sp-div              | 15      |                                                                                 |
//| fp-fl-sp-add              | 4       |                                                                                 |
//| fp-fl-sp-mul              | 5       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//| res2-loadstore-port-count | 0       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for bevelab10 
//*----------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause        | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:bevelab10" | 811 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:bevelab10" | 810 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:bevelab10" | 807 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//| XU32'2:"2:bevelab10" | 808 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 2    |
//| XU32'2:"2:bevelab10" | 809 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:bevelab10" | 806 | 3       | hwm=0.0.0   | 3    |        | -     | -   | -    |
//*----------------------+-----+---------+-------------+------+--------+-------+-----+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10" 811 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                                               |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                                                                    |
//| F0   | 811 | R0 DATA |                                                                                                                                                                    |
//| F0+E | 811 | W0 DATA | bench.T402.bench.T402.bench.V_1 te=te:F0 write(3) T402.UncaughtExceptionTest.runner.1.4.V_0 te=te:F0 write(S32'0) done te=te:F0 write(U1'0)  PLI:Test70  starting. |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10" 810 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//*------+-----+---------+-------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                        |
//*------+-----+---------+-------------------------------------------------------------*
//| F1   | -   | R0 CTRL |                                                             |
//| F1   | 810 | R0 DATA |                                                             |
//| F1+E | 810 | W0 DATA | T402.UncaughtExceptionTest.runner.1.4.V_0 te=te:F1 write(0) |
//*------+-----+---------+-------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10" 807 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10" 808 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10" 809 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F2   | -   | R0 CTRL |                                                                                                                                     |
//| F2   | 809 | R0 DATA |                                                                                                                                     |
//| F2+E | 809 | W0 DATA | bench.T402.bench.T402.bench.V_1 te=te:F2 write(E1) T402.UncaughtExceptionTest.runner.1.4.V_0 te=te:F2 write(E2) done te=te:F2 writ\ |
//|      |     |         | e(U1'1)  PLI:Test70 finished.  PLI:GSAI:hpr_sysexit  PLI: runner %d                                                                 |
//| F2   | 808 | R0 DATA |                                                                                                                                     |
//| F2+E | 808 | W0 DATA | T402.UncaughtExceptionTest.runner.1.4.V_0 te=te:F2 write(E2)  PLI:GSAI:hpr_sysexit  PLI: runner %d                                  |
//| F2   | 807 | R0 DATA |                                                                                                                                     |
//| F2+E | 807 | W0 DATA | bench.T402.bench.T402.bench.V_1 te=te:F2 write(E1) T402.UncaughtExceptionTest.runner.1.4.V_0 te=te:F2 write(E2)  PLI:GSAI:hpr_syse\ |
//|      |     |         | xit  PLI: runner %d                                                                                                                 |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10" 806 :  major_start_pcl=3   edge_private_start/end=-1/-1 exec=3 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//*------+-----+---------+-----------------------*
//| pc   | eno | Phaser  | Work                  |
//*------+-----+---------+-----------------------*
//| F3   | -   | R0 CTRL |                       |
//| F3   | 806 | R0 DATA |                       |
//| F3+E | 806 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*------+-----+---------+-----------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= -1+bench.T402.bench.T402.bench.V_1
//
//  E2 =.= 1+T402.UncaughtExceptionTest.runner.1.4.V_0
//
//  E3 =.= {[T402.UncaughtExceptionTest.runner.1.4.V_0>=9, bench.T402.bench.T402.bench.V_1<1]}
//
//  E4 =.= T402.UncaughtExceptionTest.runner.1.4.V_0<9
//
//  E5 =.= {[T402.UncaughtExceptionTest.runner.1.4.V_0>=9, bench.T402.bench.T402.bench.V_1>=1]}
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test70 to test70

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 3
//
//64 bits in scalar variables
//
//Total state bits in module = 67 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread bench/.cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread UncaughtExceptionTest/.cctor uid=cctor10 has 5 CIL instructions in 1 basic blocks
//Thread bench uid=Main10 has 29 CIL instructions in 9 basic blocks
//Thread mpc10 has 4 bevelab control states (pauses)
//Reindexed thread bevelab10 with 4 minor control states
// eof (HPR L/S Verilog)
