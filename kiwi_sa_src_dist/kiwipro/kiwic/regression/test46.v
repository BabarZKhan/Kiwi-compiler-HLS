

// CBG Orangepath HPR L/S System

// Verilog output file generated at 08/04/2017 22:17:43
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1j : 26th-March-2017 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=disable test46.exe -sim 1800 -vnl-resets=synchronous -vnl test46.v -vnl-rootmodname=DUT -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(input clk, input reset);
  reg [3:0] bevelab10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test46
      if (reset)  bevelab10nz <= 32'd0;
           else 
          case (bevelab10nz)
              32'h0/*0:bevelab10nz*/:  begin 
                  $display("Test test46 start.");
                   bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                   end 
                  
              32'h1/*1:bevelab10nz*/:  begin 
                  $display("Test p1 %1d %h", 32'sd200, 40'sd733007751850);
                   bevelab10nz <= 32'h2/*2:bevelab10nz*/;
                   end 
                  
              32'h2/*2:bevelab10nz*/:  begin 
                  $display("Test p2 %1d %h", 32'sd201, 48'sd187649984430080);
                   bevelab10nz <= 32'h3/*3:bevelab10nz*/;
                   end 
                  
              32'h3/*3:bevelab10nz*/:  begin 
                  $display("Test p3 %1d %h", 32'sd202, 32'sd2863311530);
                   bevelab10nz <= 32'h4/*4:bevelab10nz*/;
                   end 
                  
              32'h4/*4:bevelab10nz*/:  begin 
                  $display("Test p4 %1d %h", 32'sd203, 32'sd2863311360);
                   bevelab10nz <= 32'h5/*5:bevelab10nz*/;
                   end 
                  
              32'h5/*5:bevelab10nz*/:  begin 
                  $display("Test p5 %1d %h", 32'sd204, 32'sd2863267840);
                  $display("dstt_mac is hex=%H decimal=%1d", 64'h_cafe_0000, 64'h_cafe_0000);
                   bevelab10nz <= 32'h6/*6:bevelab10nz*/;
                   end 
                  
              32'h6/*6:bevelab10nz*/:  begin 
                  $display("dstt_mac now is hex=%H decimal=%1d", 32'sd3405643777, 32'sd3405643777);
                  $display("First    Checking Salvator's bug %1d hex=%h", 64'sh_8f00_0000, 64'sh_8f00_0000);
                   bevelab10nz <= 32'h7/*7:bevelab10nz*/;
                   end 
                  
              32'h7/*7:bevelab10nz*/:  begin 
                  $display("Second   Checking Salvator's bug %1d hex=%h", 64'h_8f00_0000, 64'h_8f00_0000);
                  $display("Test test46 finished.");
                   bevelab10nz <= 32'h8/*8:bevelab10nz*/;
                   end 
                  
              32'h8/*8:bevelab10nz*/:  begin 
                  $finish(32'sd0);
                   bevelab10nz <= 32'h8/*8:bevelab10nz*/;
                   end 
                  endcase
      //End structure HPR test46


       end 
      

// 1 vectors of width 4
// Total state bits in module = 4 bits.
// Total number of leaf cells = 0
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1j : 26th-March-2017
//08/04/2017 22:17:39
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=disable test46.exe -sim 1800 -vnl-resets=synchronous -vnl test46.v -vnl-rootmodname=DUT -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


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
//KiwiC: front end input processing of class or method called test460
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called test460
//
//root_compiler: start elaborating class 'test460'
//
//elaborating class 'test460'
//
//compiling static method as entry point: style=Root idl=test460/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//root_compiler class done: test460
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=10000
//
//   kiwic-finish=enable
//
//   kiwic-cil-dump=disable
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
//   kiwic-fpgaconsole-default=enable
//
//   directorate-style=basic
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
//   srcfile=test46.exe
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
//| fp-fl-dp-div              | 5       |                                                                                 |
//| fp-fl-dp-add              | 4       |                                                                                 |
//| fp-fl-dp-mul              | 3       |                                                                                 |
//| fp-fl-sp-div              | 5       |                                                                                 |
//| fp-fl-sp-add              | 4       |                                                                                 |
//| fp-fl-sp-mul              | 4       |                                                                                 |
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
//*----------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//| gb-flag/Pause              | eno | hwm         | root | exec | start | end | antecedants | next |
//*----------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//|   XU32'0:"0:bevelab10"     | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1    |
//|   XU32'1:"1:bevelab10"     | 901 | hwm=0.0.0   | 1    | 1    | -     | -   | ---         | 2    |
//|   XU32'2:"2:bevelab10"     | 902 | hwm=0.0.0   | 2    | 2    | -     | -   | ---         | 3    |
//|   XU32'4:"4:bevelab10"     | 903 | hwm=0.0.0   | 3    | 3    | -     | -   | ---         | 4    |
//|   XU32'8:"8:bevelab10"     | 904 | hwm=0.0.0   | 4    | 4    | -     | -   | ---         | 5    |
//|   XU32'16:"16:bevelab10"   | 905 | hwm=0.0.0   | 5    | 5    | -     | -   | ---         | 6    |
//|   XU32'32:"32:bevelab10"   | 906 | hwm=0.0.0   | 6    | 6    | -     | -   | ---         | 7    |
//|   XU32'64:"64:bevelab10"   | 907 | hwm=0.0.0   | 7    | 7    | -     | -   | ---         | 8    |
//|   XU32'128:"128:bevelab10" | 908 | hwm=0.0.0   | 8    | 8    | -     | -   | ---         | 8    |
//*----------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//*-----+-----+---------+-------------------------*
//| pc  | eno | Phaser  | Work                    |
//*-----+-----+---------+-------------------------*
//| 0   | -   | R0 CTRL |                         |
//| 0   | 900 | R0 DATA |                         |
//| 0+E | 900 | W0 DATA |  PLI:Test test46 start. |
//*-----+-----+---------+-------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//*-----+-----+---------+--------------------*
//| pc  | eno | Phaser  | Work               |
//*-----+-----+---------+--------------------*
//| 1   | -   | R0 CTRL |                    |
//| 1   | 901 | R0 DATA |                    |
//| 1+E | 901 | W0 DATA |  PLI:Test p1 %d %h |
//*-----+-----+---------+--------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//res2: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//*-----+-----+---------+--------------------*
//| pc  | eno | Phaser  | Work               |
//*-----+-----+---------+--------------------*
//| 2   | -   | R0 CTRL |                    |
//| 2   | 902 | R0 DATA |                    |
//| 2+E | 902 | W0 DATA |  PLI:Test p2 %d %h |
//*-----+-----+---------+--------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//res2: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//*-----+-----+---------+--------------------*
//| pc  | eno | Phaser  | Work               |
//*-----+-----+---------+--------------------*
//| 3   | -   | R0 CTRL |                    |
//| 3   | 903 | R0 DATA |                    |
//| 3+E | 903 | W0 DATA |  PLI:Test p3 %d %h |
//*-----+-----+---------+--------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//res2: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//*-----+-----+---------+--------------------*
//| pc  | eno | Phaser  | Work               |
//*-----+-----+---------+--------------------*
//| 4   | -   | R0 CTRL |                    |
//| 4   | 904 | R0 DATA |                    |
//| 4+E | 904 | W0 DATA |  PLI:Test p4 %d %h |
//*-----+-----+---------+--------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//res2: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//*-----+-----+---------+-------------------------------------------------*
//| pc  | eno | Phaser  | Work                                            |
//*-----+-----+---------+-------------------------------------------------*
//| 5   | -   | R0 CTRL |                                                 |
//| 5   | 905 | R0 DATA |                                                 |
//| 5+E | 905 | W0 DATA |  PLI:dstt_mac is hex=%H d...  PLI:Test p5 %d %h |
//*-----+-----+---------+-------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'32:"32:bevelab10"
//res2: Thread=bevelab10 state=XU32'32:"32:bevelab10"
//*-----+-----+---------+-----------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                      |
//*-----+-----+---------+-----------------------------------------------------------*
//| 6   | -   | R0 CTRL |                                                           |
//| 6   | 906 | R0 DATA |                                                           |
//| 6+E | 906 | W0 DATA |  PLI:First    Checking Sa...  PLI:dstt_mac now is hex=... |
//*-----+-----+---------+-----------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'64:"64:bevelab10"
//res2: Thread=bevelab10 state=XU32'64:"64:bevelab10"
//*-----+-----+---------+-----------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                      |
//*-----+-----+---------+-----------------------------------------------------------*
//| 7   | -   | R0 CTRL |                                                           |
//| 7   | 907 | R0 DATA |                                                           |
//| 7+E | 907 | W0 DATA |  PLI:Test test46 finished...  PLI:Second   Checking Sa... |
//*-----+-----+---------+-----------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'128:"128:bevelab10"
//res2: Thread=bevelab10 state=XU32'128:"128:bevelab10"
//*-----+-----+---------+-----------------------*
//| pc  | eno | Phaser  | Work                  |
//*-----+-----+---------+-----------------------*
//| 8   | -   | R0 CTRL |                       |
//| 8   | 908 | R0 DATA |                       |
//| 8+E | 908 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-----+-----+---------+-----------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  -- No expression aliases to report
//

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 4
//
//Total state bits in module = 4 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 3 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 58 CIL instructions in 6 basic blocks
//Thread mpc10 has 9 bevelab control states (pauses)
//Reindexed thread bevelab10 with 9 minor control states
// eof (HPR L/S Verilog)
