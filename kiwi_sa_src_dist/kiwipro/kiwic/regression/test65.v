

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:21
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test65.exe -sim=1800 -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test65.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=waypoint_nets pi_name=kppIos10 */
    output reg [11:0] KppWaypoint0,
    output reg [639:0] KppWaypoint1,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiBENCMAIN4001PC10nz_pc_export);

function signed [31:0] rtl_sign_extend0;
   input [15:0] arg;
   rtl_sign_extend0 = { {16{arg[15]}}, arg[15:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg signed [31:0] BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0;
  reg signed [31:0] BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0;
// abstractionName=repack-newnets
  reg [7:0] A_8_US_CC_SCALbx10_ARA0[5:0];
// abstractionName=res2-morenets
  reg [2:0] kiwiBENCMAIN4001PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX18 = 32'sd1;
       hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.BENCMAIN400_1/1.0
      if (reset)  begin 
               kiwiBENCMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0 <= 32'd0;
               BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiBENCMAIN4001PC10nz)
                  32'h0/*0:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("Test65 starting.");
                           kiwiBENCMAIN4001PC10nz <= 32'h1/*1:kiwiBENCMAIN4001PC10nz*/;
                           done <= 32'h0;
                           BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0 <= 32'sh0;
                           BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h2/*2:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0 <= 32'sh0;
                           KppWaypoint0 <= 32'sd2;
                           KppWaypoint1 <= "START-VARIDX";
                           end 
                          
                  32'h2/*2:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0)>=32'sh6)) $display("  %1d vardix %1d", BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0
                              , A_8_US_CC_SCALbx10_ARA0[BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0]);
                               else  begin 
                                  $display("  %1d vardix %1d", BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0, A_8_US_CC_SCALbx10_ARA0[BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0
                                  ]);
                                   BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0 <= $signed(32'sd1+BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0
                                  );

                                   end 
                                  if (($signed(32'sd1+BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0)>=32'sh6))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h3/*3:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0 <= $signed(32'sd1+BENCMAIN400_ConstRoms1_RunVarIdx_0_8_V_0
                                  );

                                   end 
                                   end 
                          
                  32'h3/*3:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h4/*4:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0 <= 32'sh0;
                           KppWaypoint0 <= 32'sd2;
                           KppWaypoint1 <= "START-CONSTIDX";
                           end 
                          
                  32'h4/*4:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0)>=32'sd3))  begin 
                                  $display("  %1d constidx %1d", BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0, BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0
                                  +rtl_sign_extend0(16'h37));
                                  $display("Test65 finished.");
                                   end 
                                   else  begin 
                                  $display("  %1d constidx %1d", BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0, BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0
                                  +rtl_sign_extend0(16'h37));
                                   BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0 <= $signed(32'sd1+BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0
                                  );

                                   end 
                                  if (($signed(32'sd1+BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0)>=32'sd3))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h5/*5:kiwiBENCMAIN4001PC10nz*/;
                                   done <= 32'h1;
                                   BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0 <= $signed(32'sd1+BENCMAIN400_ConstRoms1_RunConstIdx_0_13_V_0
                                  );

                                   KppWaypoint0 <= 32'sd2;
                                   KppWaypoint1 <= "FINISH-VARIDX";
                                   end 
                                   end 
                          
                  32'h5/*5:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $finish(32'sd0);
                           kiwiBENCMAIN4001PC10nz <= 32'h6/*6:kiwiBENCMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  kiwiBENCMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz_pc_export <= kiwiBENCMAIN4001PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.BENCMAIN400_1/1.0


       end 
      

 initial        begin 
      //ROM data table: 6 words of 8 bits.
       A_8_US_CC_SCALbx10_ARA0[0] = 8'h16;
       A_8_US_CC_SCALbx10_ARA0[1] = 8'h21;
       A_8_US_CC_SCALbx10_ARA0[2] = 8'h2c;
       A_8_US_CC_SCALbx10_ARA0[3] = 8'h37;
       A_8_US_CC_SCALbx10_ARA0[4] = 8'h42;
       A_8_US_CC_SCALbx10_ARA0[5] = 8'h4d;
       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 3
// 2 vectors of width 32
// 1 vectors of width 1
// 6 array locations of width 8
// Total state bits in module = 116 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:18
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test65.exe -sim=1800 -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test65.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


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
//KiwiC: front end input processing of class ConstRoms1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=ConstRoms1..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=ConstRoms1..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor16 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor16 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class bench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=bench..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=bench..cctor
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
//   kiwic-cil-dump=disable
//
//
//   kiwic-kcode-dump=enable
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
//   srcfile=test65.exe
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
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                     | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiBENCMAIN4001PC10"   | 825 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiBENCMAIN4001PC10"   | 824 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"   | 822 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"   | 823 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiBENCMAIN4001PC10"   | 821 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"   | 819 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"   | 820 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiBENCMAIN4001PC10" | 818 | 5       | hwm=0.0.0   | 5    |        | -     | -   | -    |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                     |
//| F0   | E825 | R0 DATA |                                                                                                                                                     |
//| F0+E | E825 | W0 DATA | BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0write(S32'0) BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0write(S32'0) donewrite(U32'0)  PLI:Test65 starting. |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                   |
//*------+------+---------+------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                        |
//| F1   | E824 | R0 DATA |                                                                        |
//| F1+E | E824 | W0 DATA | BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0write(S32'0)  W/P:START-VARIDX |
//*------+------+---------+------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                  |
//*------+------+---------+-----------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                       |
//| F2   | E823 | R0 DATA |                                                                       |
//| F2+E | E823 | W0 DATA | BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0write(E1)  PLI:  %d vardix %u |
//| F2   | E822 | R0 DATA |                                                                       |
//| F2+E | E822 | W0 DATA | BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0write(E1)  PLI:  %d vardix %u |
//*------+------+---------+-----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                        |
//*------+------+---------+-----------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                             |
//| F3   | E821 | R0 DATA |                                                                             |
//| F3+E | E821 | W0 DATA | BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0write(S32'0)  W/P:START-CONSTIDX |
//*------+------+---------+-----------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                 |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                                      |
//| F4   | E820 | R0 DATA |                                                                                                                                      |
//| F4+E | E820 | W0 DATA | BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0write(E2) donewrite(U32'1)  W/P:FINISH-VARIDX  PLI:Test65 finished.  PLI:  %d constidx %d |
//| F4   | E819 | R0 DATA |                                                                                                                                      |
//| F4+E | E819 | W0 DATA | BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0write(E2)  PLI:  %d constidx %d                                                           |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------*
//| pc   | eno  | Phaser  | Work                  |
//*------+------+---------+-----------------------*
//| F5   | -    | R0 CTRL |                       |
//| F5   | E818 | R0 DATA |                       |
//| F5+E | E818 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(1+BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0)
//
//
//  E2 =.= C(1+BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0)
//
//
//  E3 =.= (C(1+BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0))>=S32'6
//
//
//  E4 =.= (C(1+BENCMAIN400.ConstRoms1.RunVarIdx.0.8.V_0))<S32'6
//
//
//  E5 =.= (C(1+BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0))>=3
//
//
//  E6 =.= (C(1+BENCMAIN400.ConstRoms1.RunConstIdx.0.13.V_0))<3
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test65 to test65

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 3
//
//2 vectors of width 32
//
//1 vectors of width 1
//
//6 array locations of width 8
//
//Total state bits in module = 116 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread ConstRoms1..cctor uid=cctor10 has 14 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread bench..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread bench.Main uid=Main10 has 28 CIL instructions in 9 basic blocks
//Thread mpc10 has 6 bevelab control states (pauses)
//Reindexed thread kiwiBENCMAIN4001PC10 with 6 minor control states
// eof (HPR L/S Verilog)
