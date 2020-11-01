

// CBG Orangepath HPR L/S System

// Verilog output file generated at 09/03/2017 18:07:32
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1g : 8th-March-2017 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 test62.exe -sim 1800 -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwife-overloads-loglevel=0 -kiwife-gtrace-loglevel=0 -vnl=test62.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(output reg done, input clk, input reset);
  reg [1:0] bevelab10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test62
      if (reset)  begin 
               done <= 32'd0;
               bevelab10nz <= 32'd0;
               end 
               else 
          case (bevelab10nz)
              32'h0/*0:bevelab10nz*/:  begin 
                  $display("Mammal is returned false");
                  $display("%1d is not a Mammal", "System.Type");
                  $display("%1d is not a Mammal", "System.Type");
                  $display("Test62finished.");
                   done <= 1'h1;
                   bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                   end 
                  
              32'h1/*1:bevelab10nz*/:  begin 
                  $finish(32'sd0);
                   bevelab10nz <= 32'h2/*2:bevelab10nz*/;
                   end 
                  endcase
      //End structure HPR test62


       end 
      

// 1 vectors of width 2
// Total state bits in module = 2 bits.
// Total number of leaf cells = 0
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1g : 8th-March-2017
//09/03/2017 18:07:29
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 test62.exe -sim 1800 -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwife-overloads-loglevel=0 -kiwife-gtrace-loglevel=0 -vnl=test62.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T400:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T401:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T402:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T403:::
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5001 dt=$star1$/SafeCasting/Giraffe usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5006 dt=$star1$/SafeCasting/Mammal usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5002 dt=$star1$/SafeCasting/SuperNova usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5008 dt=$star1$/SafeCasting/Mammal usecount=2
//
//: Linear scan colouring done for 7 vregs using 3 pregs
//

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
//KiwiC: front end input processing of class or method called bench
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called bench
//
//root_compiler: start elaborating class 'bench'
//
//elaborating class 'bench'
//
//compiling static method as entry point: style=Root idl=bench/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//Register sharing: general T403.SafeCasting.UseAsOperator.0.15.V_0/GP used for T403.SafeCasting.UseAsOperator.0.15.V_0
//
//Register sharing: general T403.SafeCasting.UseAsOperator.0.15.V_0/GP used for T403.SafeCasting.UseAsOperator.0.10.V_0
//
//root_compiler class done: bench
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=10000
//
//   kiwic-finish=enable
//
//   kiwic-cil-dump=combined
//
//   kiwic-kcode-dump=enable
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
//   kiwife-cil-loglevel=20
//
//   kiwife-ataken-loglevel=20
//
//   kiwife-gtrace-loglevel=0
//
//   kiwife-firstpass-loglevel=20
//
//   kiwife-overloads-loglevel=0
//
//   root=$attributeroot
//
//   srcfile=test62.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation SaSuperNova for prefix SafeCasting/SuperNova
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation SaGiraffe for prefix SafeCasting/Giraffe
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
//*------------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//| gb-flag/Pause          | eno | hwm         | root | exec | start | end | antecedants | next   |
//*------------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//|   XU32'0:"0:bevelab10" | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1      |
//|   XU32'1:"1:bevelab10" | 901 | hwm=0.0.0   | 1    | 1    | -     | -   | ---         | <NONE> |
//*------------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//*-----+-----+---------+------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                 |
//*-----+-----+---------+------------------------------------------------------------------------------------------------------*
//| 0   | -   | R0 CTRL |                                                                                                      |
//| 0   | 900 | R0 DATA |                                                                                                      |
//| 0+E | 900 | W0 DATA | done te=te:0 scalarw(U1'1)  PLI:Test62finished.  PLI:%d is not a Mammal  PLI:Mammal is returned f... |
//*-----+-----+---------+------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//*-----+-----+---------+-----------------------*
//| pc  | eno | Phaser  | Work                  |
//*-----+-----+---------+-----------------------*
//| 1   | -   | R0 CTRL |                       |
//| 1   | 901 | R0 DATA |                       |
//| 1+E | 901 | W0 DATA |  PLI:GSAI:hpr_sysexit |
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
//1 vectors of width 2
//
//Total state bits in module = 2 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 3 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 33 CIL instructions in 8 basic blocks
//Thread mpc10 has 2 bevelab control states (pauses)
//Reindexed thread bevelab10 with 2 minor control states
// eof (HPR L/S Verilog)
