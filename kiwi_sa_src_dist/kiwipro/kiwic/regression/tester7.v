

// CBG Orangepath HPR L/S System

// Verilog output file generated at 14/03/2017 10:02:22
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1g : 8th-March-2017 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 test7.exe -sim 1800 -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step
`timescale 1ns/1ns


module tester7(    output hf1_dram0bank_opreq,
    input hf1_dram0bank_oprdy,
    input hf1_dram0bank_ack,
    output hf1_dram0bank_rwbar,
    output [255:0] hf1_dram0bank_wdata,
    output [21:0] hf1_dram0bank_addr,
    input [255:0] hf1_dram0bank_rdata,
    output [31:0] hf1_dram0bank_lanes,
    input clk,
    input reset);

function signed [63:0] rtl_sign_extend0;
   input [31:0] arg;
   rtl_sign_extend0 = { {32{arg[31]}}, arg[31:0] };
   endfunction

  integer T403_tester7_test7_static_overrides_0_2_V_3;
  reg signed T403_test7upper_adder_1_4_SPILL_256;
  integer T403_tester7_test7_static_overrides_0_2_V_4;
  reg signed T403_test7upper_adder_1_8_SPILL_256;
  integer T403_tester7_test7_static_overrides_0_2_V_5;
  reg signed T403_test7upper_adder_1_12_SPILL_256;
  reg signed [31:0] A_SINT_CC_SCALbx14_kv_table;
  reg signed [31:0] A_SINT_CC_SCALbx12_kv_table;
  reg signed [31:0] A_SINT_CC_SCALbx10_kv_table;
  reg [2:0] bevelab10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test7

      case (bevelab10nz)
          32'h0/*0:bevelab10nz*/:  begin 
              $display("Test7 static override test start");
               A_SINT_CC_SCALbx14_kv_table <= 32'sd1;
               A_SINT_CC_SCALbx12_kv_table <= 32'sd1;
               A_SINT_CC_SCALbx10_kv_table <= 32'sd0;
               T403_tester7_test7_static_overrides_0_2_V_3 <= 32'sd0;
               bevelab10nz <= 32'h1/*1:bevelab10nz*/;
               end 
              
          32'h1/*1:bevelab10nz*/:  bevelab10nz <= 32'h2/*2:bevelab10nz*/;

          32'h2/*2:bevelab10nz*/:  begin 
              
              case (A_SINT_CC_SCALbx14_kv_table)
                  32'h0/*0:USA10*/: $display("Test7 upper adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);

                  32'h1/*1:USA10*/: $display("Test7 lower override adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);
              endcase
              if (((32'h1/*1:USA10*/==A_SINT_CC_SCALbx14_kv_table)? 1'd1: (32'h0/*0:USA10*/!=A_SINT_CC_SCALbx14_kv_table)) || (32'h0/*0:USA10*/==
              A_SINT_CC_SCALbx14_kv_table))  bevelab10nz <= 32'h3/*3:bevelab10nz*/;
                  if (((32'h1/*1:USA10*/==A_SINT_CC_SCALbx14_kv_table)? 1'd1: (32'h0/*0:USA10*/!=A_SINT_CC_SCALbx14_kv_table)) || (32'h0
              /*0:USA10*/==A_SINT_CC_SCALbx14_kv_table))  T403_test7upper_adder_1_4_SPILL_256 <= ((32'h1/*1:USA10*/!=A_SINT_CC_SCALbx14_kv_table
                  ) && (32'h0/*0:USA10*/!=A_SINT_CC_SCALbx14_kv_table)? "$vtable_filler$": ((32'h0/*0:USA10*/==A_SINT_CC_SCALbx14_kv_table
                  )? rtl_sign_extend0(64'sd1002+rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3)): rtl_sign_extend0(64'sd102
                  +rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3))));

                   end 
              
          32'h3/*3:bevelab10nz*/:  begin 
              
              case (A_SINT_CC_SCALbx12_kv_table)
                  32'h0/*0:USA12*/: $display("Test7 upper adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);

                  32'h1/*1:USA12*/: $display("Test7 lower override adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);
              endcase
              if (((32'h1/*1:USA12*/==A_SINT_CC_SCALbx12_kv_table)? 1'd1: (32'h0/*0:USA12*/!=A_SINT_CC_SCALbx12_kv_table)) || (32'h0/*0:USA12*/==
              A_SINT_CC_SCALbx12_kv_table))  begin 
                       T403_tester7_test7_static_overrides_0_2_V_4 <= T403_test7upper_adder_1_4_SPILL_256;
                       T403_test7upper_adder_1_8_SPILL_256 <= ((32'h1/*1:USA12*/!=A_SINT_CC_SCALbx12_kv_table) && (32'h0/*0:USA12*/!=
                      A_SINT_CC_SCALbx12_kv_table)? "$vtable_filler$": ((32'h0/*0:USA12*/==A_SINT_CC_SCALbx12_kv_table)? rtl_sign_extend0(64'sd1002
                      +rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3)): rtl_sign_extend0(64'sd102+rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3
                      ))));

                       end 
                      if (((32'h1/*1:USA12*/==A_SINT_CC_SCALbx12_kv_table)? 1'd1: (32'h0/*0:USA12*/!=A_SINT_CC_SCALbx12_kv_table)) || 
              (32'h0/*0:USA12*/==A_SINT_CC_SCALbx12_kv_table))  bevelab10nz <= 32'h4/*4:bevelab10nz*/;
                   end 
              
          32'h4/*4:bevelab10nz*/:  begin 
              
              case (A_SINT_CC_SCALbx10_kv_table)
                  32'h0/*0:USA14*/: $display("Test7 upper adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);

                  32'h1/*1:USA14*/: $display("Test7 lower override adder vv=%1d", T403_tester7_test7_static_overrides_0_2_V_3);
              endcase
              if (((32'h1/*1:USA14*/==A_SINT_CC_SCALbx10_kv_table)? 1'd1: (32'h0/*0:USA14*/!=A_SINT_CC_SCALbx10_kv_table)) || (32'h0/*0:USA14*/==
              A_SINT_CC_SCALbx10_kv_table))  begin 
                       T403_tester7_test7_static_overrides_0_2_V_5 <= T403_test7upper_adder_1_8_SPILL_256;
                       T403_test7upper_adder_1_12_SPILL_256 <= ((32'h1/*1:USA14*/!=A_SINT_CC_SCALbx10_kv_table) && (32'h0/*0:USA14*/!=
                      A_SINT_CC_SCALbx10_kv_table)? "$vtable_filler$": ((32'h0/*0:USA14*/==A_SINT_CC_SCALbx10_kv_table)? rtl_sign_extend0(64'sd1002
                      +rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3)): rtl_sign_extend0(64'sd102+rtl_sign_extend0(T403_tester7_test7_static_overrides_0_2_V_3
                      ))));

                       end 
                      if (((32'h1/*1:USA14*/==A_SINT_CC_SCALbx10_kv_table)? 1'd1: (32'h0/*0:USA14*/!=A_SINT_CC_SCALbx10_kv_table)) || 
              (32'h0/*0:USA14*/==A_SINT_CC_SCALbx10_kv_table))  bevelab10nz <= 32'h5/*5:bevelab10nz*/;
                   end 
              
          32'h5/*5:bevelab10nz*/:  begin 
              if ((T403_tester7_test7_static_overrides_0_2_V_3>=32'sd1))  begin 
                      $display("Test7 static override i=%1d l=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_tester7_test7_static_overrides_0_2_V_4
                      );
                      $display("      static override i=%1d m=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_tester7_test7_static_overrides_0_2_V_5
                      );
                      $display("      static override i=%1d u=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_test7upper_adder_1_12_SPILL_256
                      );
                      $display("Test7 static override test done");
                      $finish(32'sd0);
                       end 
                       else  begin 
                      $display("Test7 static override i=%1d l=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_tester7_test7_static_overrides_0_2_V_4
                      );
                      $display("      static override i=%1d m=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_tester7_test7_static_overrides_0_2_V_5
                      );
                      $display("      static override i=%1d u=%1d", T403_tester7_test7_static_overrides_0_2_V_3, T403_test7upper_adder_1_12_SPILL_256
                      );
                       end 
                      if ((T403_tester7_test7_static_overrides_0_2_V_3<32'sd1))  bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                   else  bevelab10nz <= 32'h5/*5:bevelab10nz*/;
               T403_tester7_test7_static_overrides_0_2_V_3 <= 32'sd1+T403_tester7_test7_static_overrides_0_2_V_3;
               end 
              endcase
      //End structure HPR test7


       end 
      

// 1 vectors of width 3
// 3 vectors of width 32
// 3 vectors of width 1
// 96 bits in scalar variables
// Total state bits in module = 198 bits.
// Total number of leaf cells = 0
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.3.1g : 8th-March-2017
//14/03/2017 10:02:19
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 test7.exe -sim 1800 -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step


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

//Report from Abbreviation:::
//    setting up abbreviation Tta1._SPILL for prefix T403/test7upper/adder/1.4/_SPILL
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation Tta1SPILL10 for prefix T403/test7upper/adder/1.8/_SPILL
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation Tta1SPILL12 for prefix T403/test7upper/adder/1.12/_SPILL
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T403:::
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5000 dt=$star1$/test7lower usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5001 dt=$star1$/test7upper usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5002 dt=$star1$/test7upper usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5003 dt=SINT usecount=1
//
//Allocate phy reg purpose=mk_spill_var2 Tta1._SPILL_256 msg=allocation for thread T403 for V5010 dt=$star1$/@/16/SS usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5004 dt=SINT usecount=1
//
//Allocate phy reg purpose=mk_spill_var2 Tta1SPILL10_256 msg=allocation for thread T403 for V5013 dt=$star1$/@/16/SS usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5005 dt=SINT usecount=1
//
//Allocate phy reg purpose=mk_spill_var2 Tta1SPILL12_256 msg=allocation for thread T403 for V5016 dt=$star1$/@/16/SS usecount=3
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5006 dt=SINT usecount=1
//
//: Linear scan colouring done for 19 vregs using 8 pregs
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
//KiwiC: front end input processing of class or method called tester7
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called tester7
//
//root_compiler: start elaborating class 'tester7'
//
//elaborating class 'tester7'
//
//compiling static method as entry point: style=Root idl=tester7/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//Register sharing: general T403.test7upper.adder.1.12._SPILL.256/GP used for T403.test7upper.adder.1.12._SPILL.256
//
//Register sharing: general T403.test7upper.adder.1.12._SPILL.256/GP used for T403.test7upper.adder.1.8._SPILL.256
//
//Register sharing: general T403.test7upper.adder.1.12._SPILL.256/GP used for T403.test7upper.adder.1.4._SPILL.256
//
//root_compiler class done: tester7
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
//   kiwife-overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=test7.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports
//*-----------+----------+----------+--------+--------+-------+-----------*
//| Name      | Protocol | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*-----------+----------+----------+--------+--------+-------+-----------*
//| dram0bank | HFAST1   | 4194304  | 22     | 256    | 32    | 8         |
//*-----------+----------+----------+--------+--------+-------+-----------*
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
//| res2-loadstore-port-count | 1       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for bevelab10 
//*--------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//| gb-flag/Pause            | eno | hwm         | root | exec | start | end | antecedants | next |
//*--------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//|   XU32'0:"0:bevelab10"   | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1    |
//|   XU32'1:"1:bevelab10"   | 901 | hwm=0.0.0   | 1    | 1    | -     | -   | ---         | 2    |
//|   XU32'2:"2:bevelab10"   | 902 | hwm=0.0.0   | 2    | 2    | -     | -   | ---         | 3    |
//|   XU32'4:"4:bevelab10"   | 903 | hwm=0.0.0   | 3    | 3    | -     | -   | ---         | 4    |
//|   XU32'8:"8:bevelab10"   | 904 | hwm=0.0.0   | 4    | 4    | -     | -   | ---         | 5    |
//|   XU32'16:"16:bevelab10" | 906 | hwm=0.0.0   | 5    | 5    | -     | -   | ---         | 1    |
//|   XU32'16:"16:bevelab10" | 905 | hwm=0.0.0   | 5    | 5    | -     | -   | ---         | 5    |
//*--------------------------+-----+-------------+------+------+-------+-----+-------------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//res2: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                                                         |
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| 0   | -   | R0 CTRL |                                                                                                                                                              |
//| 0   | 900 | R0 DATA |                                                                                                                                                              |
//| 0+E | 900 | W0 DATA | T403.tester7.test7_static_overrides.0.2.V_3 te=te:0 scalarw(0) @_SINT/CC/SCALbx10__kv_table te=te:0 scalarw(0) @_SINT/CC/SCALbx12__kv_table te=te:0 scalarw\ |
//|     |     |         | (1) @_SINT/CC/SCALbx14__kv_table te=te:0 scalarw(1)  PLI:Test7 static overrid...                                                                             |
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//res2: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//*-----+-----+---------+------*
//| pc  | eno | Phaser  | Work |
//*-----+-----+---------+------*
//| 1   | -   | R0 CTRL |      |
//| 1   | 901 | R0 DATA |      |
//| 1+E | 901 | W0 DATA |      |
//*-----+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//res2: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                               |
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| 2   | -   | R0 CTRL |                                                                                                                    |
//| 2   | 902 | R0 DATA |                                                                                                                    |
//| 2+E | 902 | W0 DATA | T403.test7upper.adder.1.4._SPILL.256 te=te:2 scalarw(E1)  PLI:Test7 upper adder vv...  PLI:Test7 lower override... |
//*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//res2: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                   |
//*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//| 3   | -   | R0 CTRL |                                                                                                                        |
//| 3   | 903 | R0 DATA |                                                                                                                        |
//| 3+E | 903 | W0 DATA | T403.test7upper.adder.1.8._SPILL.256 te=te:3 scalarw(E2) T403.tester7.test7_static_overrides.0.2.V_4 te=te:3 scalarw(\ |
//|     |     |         | E3)  PLI:Test7 upper adder vv...  PLI:Test7 lower override...                                                          |
//*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//res2: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                    |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//| 4   | -   | R0 CTRL |                                                                                                                         |
//| 4   | 904 | R0 DATA |                                                                                                                         |
//| 4+E | 904 | W0 DATA | T403.test7upper.adder.1.12._SPILL.256 te=te:4 scalarw(E4) T403.tester7.test7_static_overrides.0.2.V_5 te=te:4 scalarw(\ |
//|     |     |         | E5)  PLI:Test7 upper adder vv...  PLI:Test7 lower override...                                                           |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//res2: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                                            |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| 5   | -   | R0 CTRL |                                                                                                                                                 |
//| 5   | 905 | R0 DATA |                                                                                                                                                 |
//| 5+E | 905 | W0 DATA | T403.tester7.test7_static_overrides.0.2.V_3 te=te:5 scalarw(E6)  PLI:GSAI:hpr_sysexit  PLI:Test7 static overrid...  PLI:      static overrid... |
//| 5   | 906 | R0 DATA |                                                                                                                                                 |
//| 5+E | 906 | W0 DATA | T403.tester7.test7_static_overrides.0.2.V_3 te=te:5 scalarw(E6)  PLI:      static overrid...  PLI:Test7 static overrid...                       |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//Highest off-chip SRAM/DRAM location in use on port dram0bank is <null> (--not-used--) bytes=1048576

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= COND({[XU32'1:"1:USA10"!=@_SINT/CC/SCALbx14__kv_table, XU32'0:"0:USA10"!=@_SINT/CC/SCALbx14__kv_table]}, "$vtable_filler$", COND(XU32'0:"0:USA10"==@_SINT/CC/SCALbx14__kv_table, C64(1002+T403.tester7.test7_static_overrides.0.2.V_3), C64(102+T403.tester7.test7_static_overrides.0.2.V_3)))
//
//  E2 =.= COND({[XU32'1:"1:USA12"!=@_SINT/CC/SCALbx12__kv_table, XU32'0:"0:USA12"!=@_SINT/CC/SCALbx12__kv_table]}, "$vtable_filler$", COND(XU32'0:"0:USA12"==@_SINT/CC/SCALbx12__kv_table, C64(1002+T403.tester7.test7_static_overrides.0.2.V_3), C64(102+T403.tester7.test7_static_overrides.0.2.V_3)))
//
//  E3 =.= C(T403.test7upper.adder.1.4._SPILL.256)
//
//  E4 =.= COND({[XU32'1:"1:USA14"!=@_SINT/CC/SCALbx10__kv_table, XU32'0:"0:USA14"!=@_SINT/CC/SCALbx10__kv_table]}, "$vtable_filler$", COND(XU32'0:"0:USA14"==@_SINT/CC/SCALbx10__kv_table, C64(1002+T403.tester7.test7_static_overrides.0.2.V_3), C64(102+T403.tester7.test7_static_overrides.0.2.V_3)))
//
//  E5 =.= C(T403.test7upper.adder.1.8._SPILL.256)
//
//  E6 =.= 1+T403.tester7.test7_static_overrides.0.2.V_3
//
//  E7 =.= {[XU32'0:"0:USA10"==@_SINT/CC/SCALbx14__kv_table]; [XU32'1:"1:USA10"==@_SINT/CC/SCALbx14__kv_table]; [XU32'1:"1:USA10"!=@_SINT/CC/SCALbx14__kv_table, XU32'0:"0:USA10"!=@_SINT/CC/SCALbx14__kv_table]}
//
//  E8 =.= {[XU32'0:"0:USA12"==@_SINT/CC/SCALbx12__kv_table]; [XU32'1:"1:USA12"==@_SINT/CC/SCALbx12__kv_table]; [XU32'1:"1:USA12"!=@_SINT/CC/SCALbx12__kv_table, XU32'0:"0:USA12"!=@_SINT/CC/SCALbx12__kv_table]}
//
//  E9 =.= {[XU32'0:"0:USA14"==@_SINT/CC/SCALbx10__kv_table]; [XU32'1:"1:USA14"==@_SINT/CC/SCALbx10__kv_table]; [XU32'1:"1:USA14"!=@_SINT/CC/SCALbx10__kv_table, XU32'0:"0:USA14"!=@_SINT/CC/SCALbx10__kv_table]}
//
//  E10 =.= T403.tester7.test7_static_overrides.0.2.V_3>=1
//
//  E11 =.= T403.tester7.test7_static_overrides.0.2.V_3<1
//

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 3
//
//3 vectors of width 32
//
//3 vectors of width 1
//
//96 bits in scalar variables
//
//Total state bits in module = 198 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 3 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 64 CIL instructions in 19 basic blocks
//Thread mpc10 has 6 bevelab control states (pauses)
//Reindexed thread bevelab10 with 6 minor control states
// eof (HPR L/S Verilog)
