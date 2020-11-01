

// CBG Orangepath HPR L/S System

// Verilog output file generated at 13/06/2018 14:59:58
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.6b : 15th April 2018 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -bevelab-cfg-dotreport=enable -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full test51a.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51a.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg [63:0] wideout,
    input reg [63:0] widein,
    output reg [7:0] ksubsAbendSyndrome,
    output reg [7:0] ksubsGpioLeds,
    output reg [7:0] ksubsManualWaypoint,
    
/* portgroup= abstractionName=res2-directornets */
output reg [1:0] test51a10PC10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batch10 */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);
// abstractionName=res2-morenets
  reg test51a10PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogtest51a/1.0
      if (reset)  begin 
               test51a10PC10nz <= 32'd0;
               widein <= 64'd0;
               wideout <= 64'd0;
               ksubsManualWaypoint <= 32'd0;
               ksubsGpioLeds <= 32'd0;
               ksubsAbendSyndrome <= 32'd0;
               end 
               else 
          case (test51a10PC10nz)
              32'h0/*0:test51a10PC10nz*/:  begin 
                   test51a10PC10nz <= 32'h1/*1:test51a10PC10nz*/;
                   widein <= 64'd0;
                   wideout <= 64'd0;
                   ksubsManualWaypoint <= 8'h0;
                   ksubsGpioLeds <= 8'h80;
                   ksubsAbendSyndrome <= 8'h80;
                  $finish(32'sd0);
                   end 
                  endcase
      if (reset)  test51a10PC10nz_pc_export <= 32'd0;
           else  test51a10PC10nz_pc_export <= test51a10PC10nz;
      //End structure cvtToVerilogtest51a/1.0


       end 
      

// Structural Resource (FU) inventory:// 1 vectors of width 1
// Total state bits in module = 1 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.6b : 15th April 2018
//13/06/2018 14:59:56
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -bevelab-cfg-dotreport=enable -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full test51a.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51a.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called KiwiSystem.Kiwi
//
//root_walk start thread at a static method (used as an entry point). Method name=KiwiSystem/Kiwi/.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called System.BitConverter
//
//root_walk start thread at a static method (used as an entry point). Method name=System/BitConverter/.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called WideWordDemoUsingArrays
//
//root_walk start thread at a static method (used as an entry point). Method name=WideWordDemoUsingArrays/.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) uid=cctor10 full_idl=WideWordDemoUsingArrays..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called WideWordDemoUsingArrays
//
//root_compiler: start elaborating class 'WideWordDemoUsingArrays'
//
//elaborating class 'WideWordDemoUsingArrays'
//
//compiling static method as entry point: style=Root idl=WideWordDemoUsingArrays/Main
//
//Performing root elaboration of method WideWordDemoUsingArrays.Main
//
//KiwiC start_thread (or entry point) uid=Main10 full_idl=WideWordDemoUsingArrays.Main
//
//root_compiler class done: WideWordDemoUsingArrays
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
//   kiwic-cil-dump=disable
//
//   kiwic-kcode-dump=disable
//
//   kiwife-dynpoly=disable
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
//   srcfile=test51a.exe
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
//| int-flr-mul               | 1000    |                                                                                 |
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

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for test51a10PC10 
//*--------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause            | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:test51a10PC10" | 809 | 0       | hwm=0.0.0   | 0    |        | -     | -   | -    |
//*--------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test51a10PC10 state=XU32'0:"0:test51a10PC10" 809 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Schedule for res2: nopipeline: Thread=test51a10PC10 state=XU32'0:"0:test51a10PC10"
//res2: nopipeline: Thread=test51a10PC10 state=XU32'0:"0:test51a10PC10"
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                       |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                                            |
//| F0   | 809 | R0 DATA |                                                                                                                                            |
//| F0+E | 809 | W0 DATA | ksubsAbendSyndrome te=te:F0 write(U8'128) ksubsGpioLeds te=te:F0 write(U8'128) ksubsManualWaypoint te=te:F0 write(U8'0) wideout te=te:F0 \ |
//|      |     |         | write({SC:d4,0}) widein te=te:F0 write({SC:d3,0})  PLI:GSAI:hpr_sysexit                                                                    |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  -- No expression aliases to report
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test51a to test51a

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory:
//1 vectors of width 1
//
//Total state bits in module = 1 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem/Kiwi/.cctor uid=cctor14 has 6 CIL instructions in 1 basic blocks
//Thread System/BitConverter/.cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread WideWordDemoUsingArrays/.cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread WideWordDemoUsingArrays/Main uid=Main10 has 9 CIL instructions in 1 basic blocks
//Thread mpc10 has 1 bevelab control states (pauses)
//Reindexed thread test51a10PC10 with 1 minor control states
// eof (HPR L/S Verilog)
