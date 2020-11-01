

// CBG Orangepath HPR L/S System

// Verilog output file generated at 24/11/2016 14:11:17
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.16m : 23rd-November-2016 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable -diosim-tl=100 test57.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test57.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -ataken-loglevel=0 -kiwic-kcode-dump=enable -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(input clk, input reset);
  integer T402_test57_test57_phase0_0_6_V_1;
  reg signed T402_test57_test57_phase0_0_6_SPILL_256;
  reg signed T402_test57_test57_phase0_0_6_V_2;
  reg [2:0] xpc10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test57
      if (reset)  begin 
               T402_test57_test57_phase0_0_6_V_2 <= 32'd0;
               T402_test57_test57_phase0_0_6_SPILL_256 <= 32'd0;
               T402_test57_test57_phase0_0_6_V_1 <= 32'd0;
               xpc10nz <= 32'd0;
               end 
               else  begin 
              if ((xpc10nz==3'sd5/*5:xpc10nz*/)) $display("   stringers %1d  %1d len=%1d", T402_test57_test57_phase0_0_6_V_1, T402_test57_test57_phase0_0_6_V_2
                  , 1'd0);
                  if ((T402_test57_test57_phase0_0_6_V_1>=32'sd4) && (xpc10nz==3'sd2/*2:xpc10nz*/))  begin 
                      $display("Test57 done.");
                      $finish(32'sd0);
                       end 
                      
              case (xpc10nz)
                  3'sd0/*0:xpc10nz*/:  begin 
                      $display("Kiwi Demo - Test57 starting.");
                       xpc10nz <= 3'sd1/*1:xpc10nz*/;
                       end 
                      
                  3'sd1/*1:xpc10nz*/:  begin 
                       T402_test57_test57_phase0_0_6_V_1 <= 32'sd0;
                       xpc10nz <= 3'sd2/*2:xpc10nz*/;
                       end 
                      
                  3'sd2/*2:xpc10nz*/: if ((T402_test57_test57_phase0_0_6_V_1<32'sd4))  xpc10nz <= 3'sd3/*3:xpc10nz*/;
                       else  xpc10nz <= 3'sd7/*7:xpc10nz*/;

                  3'sd3/*3:xpc10nz*/:  begin 
                      if ((32'sd2<T402_test57_test57_phase0_0_6_V_1))  T402_test57_test57_phase0_0_6_SPILL_256 <= "Bonjor Number Two"
                          ;

                           else  T402_test57_test57_phase0_0_6_SPILL_256 <= "Hello One";
                       xpc10nz <= 3'sd4/*4:xpc10nz*/;
                       end 
                      
                  3'sd4/*4:xpc10nz*/:  begin 
                       T402_test57_test57_phase0_0_6_V_2 <= T402_test57_test57_phase0_0_6_SPILL_256;
                       xpc10nz <= 3'sd5/*5:xpc10nz*/;
                       end 
                      
                  3'sd5/*5:xpc10nz*/:  xpc10nz <= 3'sd6/*6:xpc10nz*/;

                  3'sd6/*6:xpc10nz*/:  begin 
                       T402_test57_test57_phase0_0_6_V_1 <= 32'sd1+T402_test57_test57_phase0_0_6_V_1;
                       xpc10nz <= 3'sd2/*2:xpc10nz*/;
                       end 
                      endcase
               end 
              //End structure HPR test57


       end 
      

// 1 vectors of width 3
// 2 vectors of width 1
// 32 bits in scalar variables
// Total state bits in module = 37 bits.
// Total number of leaf cells = 0
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.16m : 23rd-November-2016
//24/11/2016 14:11:12
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable -diosim-tl=100 test57.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test57.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -ataken-loglevel=0 -kiwic-kcode-dump=enable -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T400:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T401:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from Abbreviation:::
//Setting up abbreviation Ttt0._SPILL for prefix T402/test57/test57_phase0/0.6/_SPILL

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T402:::
//Allocate phy reg purpose=localvar msg=allocation for thread T402 for V5000 dt=$star1$/@/16/SS usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T402 for V5001 dt=SINT usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T402 for V5002 dt=$star1$/@/16/SS usecount=1
//
//: Linear scan colouring done for 3 vregs using 3 pregs
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
//KiwiC: front end input processing of class or method called test57
//
//root_compiler: start elaborating class 'test57'
//
//elaborating class 'test57'
//
//compiling static method as entry point: style=Root idl=test57/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//root_compiler class done: test57
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=10000
//
//   kiwic-finish=enable
//
//   kiwic-cil-dump=disable
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
//   ataken-loglevel=0
//
//   gtrace-loglevel=20
//
//   firstpass-loglevel=20
//
//   overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=test57.exe
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
//| fp-fl-sp-mul              | 3       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//| res2-loadstore-port-count | 0       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for xpc10 
//*------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//| gb-flag/Pause    | eno | hwm         | root | exec | start | end | antecedants | next   |
//*------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//|   X0:"0:xpc10"   | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1      |
//|   X1:"1:xpc10"   | 901 | hwm=0.0.0   | 1    | 1    | -     | -   | ---         | 2      |
//|   X2:"2:xpc10"   | 903 | hwm=0.0.0   | 2    | 2    | -     | -   | ---         | <NONE> |
//|   X2:"2:xpc10"   | 902 | hwm=0.0.0   | 2    | 2    | -     | -   | ---         | 3      |
//|   X4:"4:xpc10"   | 905 | hwm=0.0.0   | 3    | 3    | -     | -   | ---         | 4      |
//|   X4:"4:xpc10"   | 904 | hwm=0.0.0   | 3    | 3    | -     | -   | ---         | 4      |
//|   X8:"8:xpc10"   | 906 | hwm=0.0.0   | 4    | 4    | -     | -   | ---         | 5      |
//|   X16:"16:xpc10" | 907 | hwm=0.0.0   | 5    | 5    | -     | -   | ---         | 6      |
//|   X32:"32:xpc10" | 908 | hwm=0.0.0   | 6    | 6    | -     | -   | ---         | 2      |
//*------------------+-----+-------------+------+------+-------+-----+-------------+--------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X0:"0:xpc10" 900 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X0:"0:xpc10"
//res2: Thread=xpc10 state=X0:"0:xpc10"
//*-----+-----+---------+------------------------------*
//| pc  | eno | Phaser  | Work                         |
//*-----+-----+---------+------------------------------*
//| 0   | -   | R0 CTRL |                              |
//| 0   | 900 | R0 DATA |                              |
//| 0+E | 900 | W0 DATA |  PLI:Kiwi Demo - Test57 s... |
//*-----+-----+---------+------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X1:"1:xpc10" 901 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X1:"1:xpc10"
//res2: Thread=xpc10 state=X1:"1:xpc10"
//*-----+-----+---------+------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                 |
//*-----+-----+---------+------------------------------------------------------*
//| 1   | -   | R0 CTRL |                                                      |
//| 1   | 901 | R0 DATA |                                                      |
//| 1+E | 901 | W0 DATA | T402/test57/test57_phase0/0.6/V_1 te=te:1 scalarw(0) |
//*-----+-----+---------+------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X2:"2:xpc10" 903 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X2:"2:xpc10" 902 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X2:"2:xpc10"
//res2: Thread=xpc10 state=X2:"2:xpc10"
//*-----+-----+---------+-----------------------------------------*
//| pc  | eno | Phaser  | Work                                    |
//*-----+-----+---------+-----------------------------------------*
//| 2   | -   | R0 CTRL |                                         |
//| 2   | 902 | R0 DATA |                                         |
//| 2+E | 902 | W0 DATA |                                         |
//| 2   | 903 | R0 DATA |                                         |
//| 2+E | 903 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:Test57 done. |
//*-----+-----+---------+-----------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X4:"4:xpc10" 905 :  major_start_pcl=3   edge_private_start/end=-1/-1 exec=3 (dend=0)
//,   Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X4:"4:xpc10" 904 :  major_start_pcl=3   edge_private_start/end=-1/-1 exec=3 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X4:"4:xpc10"
//res2: Thread=xpc10 state=X4:"4:xpc10"
//*-----+-----+---------+-------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                          |
//*-----+-----+---------+-------------------------------------------------------------------------------*
//| 3   | -   | R0 CTRL |                                                                               |
//| 3   | 904 | R0 DATA |                                                                               |
//| 3+E | 904 | W0 DATA | T402/test57/test57_phase0/0.6/_SPILL/256 te=te:3 scalarw("Hello One")         |
//| 3   | 905 | R0 DATA |                                                                               |
//| 3+E | 905 | W0 DATA | T402/test57/test57_phase0/0.6/_SPILL/256 te=te:3 scalarw("Bonjor Number Two") |
//*-----+-----+---------+-------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X8:"8:xpc10" 906 :  major_start_pcl=4   edge_private_start/end=-1/-1 exec=4 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X8:"8:xpc10"
//res2: Thread=xpc10 state=X8:"8:xpc10"
//*-----+-----+---------+-------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                  |
//*-----+-----+---------+-------------------------------------------------------*
//| 4   | -   | R0 CTRL |                                                       |
//| 4   | 906 | R0 DATA |                                                       |
//| 4+E | 906 | W0 DATA | T402/test57/test57_phase0/0.6/V_2 te=te:4 scalarw(E1) |
//*-----+-----+---------+-------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X16:"16:xpc10" 907 :  major_start_pcl=5   edge_private_start/end=-1/-1 exec=5 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X16:"16:xpc10"
//res2: Thread=xpc10 state=X16:"16:xpc10"
//*-----+-----+---------+------------------------------*
//| pc  | eno | Phaser  | Work                         |
//*-----+-----+---------+------------------------------*
//| 5   | -   | R0 CTRL |                              |
//| 5   | 907 | R0 DATA |                              |
//| 5+E | 907 | W0 DATA |  PLI:   stringers %u  %d ... |
//*-----+-----+---------+------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: Thread=xpc10 state=X32:"32:xpc10" 908 :  major_start_pcl=6   edge_private_start/end=-1/-1 exec=6 (dend=0)
//Simple greedy schedule for res2: Thread=xpc10 state=X32:"32:xpc10"
//res2: Thread=xpc10 state=X32:"32:xpc10"
//*-----+-----+---------+-------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                  |
//*-----+-----+---------+-------------------------------------------------------*
//| 6   | -   | R0 CTRL |                                                       |
//| 6   | 908 | R0 DATA |                                                       |
//| 6+E | 908 | W0 DATA | T402/test57/test57_phase0/0.6/V_1 te=te:6 scalarw(E2) |
//*-----+-----+---------+-------------------------------------------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= T402/test57/test57_phase0/0.6/_SPILL/256
//
//  E2 =.= 1+T402/test57/test57_phase0/0.6/V_1
//
//  E3 =.= T402/test57/test57_phase0/0.6/V_1<4
//
//  E4 =.= T402/test57/test57_phase0/0.6/V_1>=4
//
//  E5 =.= 2>=T402/test57/test57_phase0/0.6/V_1
//
//  E6 =.= 2<T402/test57/test57_phase0/0.6/V_1
//

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 3
//
//2 vectors of width 1
//
//32 bits in scalar variables
//
//Total state bits in module = 37 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 3 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 21 CIL instructions in 7 basic blocks
//Thread mpc10 has 7 bevelab control states (pauses)
//Reindexed thread xpc10 with 7 minor control states
// eof (HPR L/S Verilog)
