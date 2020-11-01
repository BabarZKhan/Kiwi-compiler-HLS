

// CBG Orangepath HPR L/S System

// Verilog output file generated at 10/01/2018 00:52:06
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.4h : 24th Dec 2017 Linux/X86_64:koo
//  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test67.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test67.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] test6710PC10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batch10 */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);
// abstractionName=kiwicmainnets10
  integer T403_SineCosTest_run_0_4_V_0;
// abstractionName=res2-contacts pi_name=CV_FP_CVT_FL2_F64_I32
  wire/*fp*/  [63:0] fpcvt10_result;
  reg signed [31:0] fpcvt10_arg;
  wire fpcvt10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] CVFPMULTIPLIER10_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER10_XX;
  reg/*fp*/  [63:0] CVFPMULTIPLIER10_YY;
  wire CVFPMULTIPLIER10_fail;
// abstractionName=res2-contacts pi_name=CV_FP_CVT_FL2_F64_I32
  wire/*fp*/  [63:0] fpcvt12_result;
  reg signed [31:0] fpcvt12_arg;
  wire fpcvt12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] CVFPMULTIPLIER12_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12_XX;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12_YY;
  wire CVFPMULTIPLIER12_fail;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] CVFPMULTIPLIER10RRh10hold;
  reg CVFPMULTIPLIER10RRh10shot0;
  reg CVFPMULTIPLIER10RRh10shot1;
  reg CVFPMULTIPLIER10RRh10shot2;
  reg/*fp*/  [63:0] fpcvt10RRh10hold;
  reg fpcvt10RRh10shot0;
  reg fpcvt10RRh10shot1;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12RRh10hold;
  reg CVFPMULTIPLIER12RRh10shot0;
  reg CVFPMULTIPLIER12RRh10shot1;
  reg CVFPMULTIPLIER12RRh10shot2;
  reg/*fp*/  [63:0] fpcvt12RRh10hold;
  reg fpcvt12RRh10shot0;
  reg fpcvt12RRh10shot1;
  reg [3:0] test6710PC10nz;
 always   @(* )  begin 
       CVFPMULTIPLIER12_XX = 32'sd0;
       CVFPMULTIPLIER12_YY = 32'sd0;
       fpcvt12_arg = 32'sd0;
       CVFPMULTIPLIER10_XX = 32'sd0;
       CVFPMULTIPLIER10_YY = 32'sd0;
       fpcvt10_arg = 32'sd0;
      if ((T403_SineCosTest_run_0_4_V_0>=32'sd1)) 
          case (test6710PC10nz)
              32'h3/*3:test6710PC10nz*/:  fpcvt12_arg = T403_SineCosTest_run_0_4_V_0;

              32'ha/*10:test6710PC10nz*/:  begin 
                   CVFPMULTIPLIER12_YY = ((32'ha/*10:test6710PC10nz*/==test6710PC10nz)? fpcvt12_result: fpcvt12RRh10hold);
                   CVFPMULTIPLIER12_XX = 64'h3ffe_6666_6666_6666;
                   end 
                  endcase
           else 
          case (test6710PC10nz)
              32'h3/*3:test6710PC10nz*/:  fpcvt10_arg = T403_SineCosTest_run_0_4_V_0;

              32'h5/*5:test6710PC10nz*/:  begin 
                   CVFPMULTIPLIER10_YY = ((32'h5/*5:test6710PC10nz*/==test6710PC10nz)? fpcvt10_result: fpcvt10RRh10hold);
                   CVFPMULTIPLIER10_XX = 64'h3ffe_6666_6666_6666;
                   end 
                  endcase
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogtest67/1.0
      if (reset)  begin 
               test6710PC10nz_pc_export <= 32'd0;
               done <= 32'd0;
               T403_SineCosTest_run_0_4_V_0 <= 32'd0;
               fpcvt12RRh10hold <= 64'd0;
               fpcvt12RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER12RRh10hold <= 64'd0;
               CVFPMULTIPLIER12RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot2 <= 32'd0;
               fpcvt10RRh10hold <= 64'd0;
               fpcvt10RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER10RRh10hold <= 64'd0;
               CVFPMULTIPLIER10RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot0 <= 32'd0;
               fpcvt10RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot0 <= 32'd0;
               fpcvt12RRh10shot0 <= 32'd0;
               test6710PC10nz <= 32'd0;
               end 
               else  begin 
              if ((32'he/*14:test6710PC10nz*/==test6710PC10nz)) $finish(32'sd0);
                  if ((T403_SineCosTest_run_0_4_V_0<32'sd1))  begin if ((32'h8/*8:test6710PC10nz*/==test6710PC10nz))  begin 
                          $display("  sine and cosine %f  %f  %f", $bitstoreal(((32'h8/*8:test6710PC10nz*/==test6710PC10nz)? CVFPMULTIPLIER10_FPRR
                          : CVFPMULTIPLIER10RRh10hold)), $bitstoreal(((32'h8/*8:test6710PC10nz*/==test6710PC10nz)? CVFPMULTIPLIER10_FPRR
                          : CVFPMULTIPLIER10RRh10hold)), $bitstoreal(64'h4001_9999_9999_999a));
                          $display("Test67 finished.");
                           end 
                           end 
                   else if ((32'hd/*13:test6710PC10nz*/==test6710PC10nz)) $display("  sine and cosine %f  %f  %f", $bitstoreal(((32'hd
                      /*13:test6710PC10nz*/==test6710PC10nz)? CVFPMULTIPLIER12_FPRR: CVFPMULTIPLIER12RRh10hold)), $bitstoreal(((32'hd
                      /*13:test6710PC10nz*/==test6710PC10nz)? CVFPMULTIPLIER12_FPRR: CVFPMULTIPLIER12RRh10hold)), $bitstoreal(64'h4001_9999_9999_999a
                      ));
                      if ((32'h0/*0:test6710PC10nz*/==test6710PC10nz))  begin 
                      $display("Test67  starting.");
                      $display("Test67  sine and cosine starting.");
                       end 
                      if ((T403_SineCosTest_run_0_4_V_0<32'sd1)) 
                  case (test6710PC10nz)
                      32'h3/*3:test6710PC10nz*/:  test6710PC10nz <= 32'h4/*4:test6710PC10nz*/;

                      32'h8/*8:test6710PC10nz*/:  begin 
                           done <= 1'h1;
                           T403_SineCosTest_run_0_4_V_0 <= -32'sd1+T403_SineCosTest_run_0_4_V_0;
                           end 
                          endcase
                   else 
                  case (test6710PC10nz)
                      32'h3/*3:test6710PC10nz*/:  test6710PC10nz <= 32'h9/*9:test6710PC10nz*/;

                      32'hd/*13:test6710PC10nz*/:  T403_SineCosTest_run_0_4_V_0 <= -32'sd1+T403_SineCosTest_run_0_4_V_0;
                  endcase

              case (test6710PC10nz)
                  32'h0/*0:test6710PC10nz*/:  begin 
                       done <= 1'h0;
                       T403_SineCosTest_run_0_4_V_0 <= 32'sd3;
                       test6710PC10nz <= 32'h1/*1:test6710PC10nz*/;
                       end 
                      
                  32'h8/*8:test6710PC10nz*/:  test6710PC10nz <= 32'he/*14:test6710PC10nz*/;

                  32'hd/*13:test6710PC10nz*/:  test6710PC10nz <= 32'h1/*1:test6710PC10nz*/;

                  32'he/*14:test6710PC10nz*/:  test6710PC10nz <= 32'hf/*15:test6710PC10nz*/;
              endcase
              if (CVFPMULTIPLIER10RRh10shot2)  CVFPMULTIPLIER10RRh10hold <= CVFPMULTIPLIER10_FPRR;
                  if (fpcvt10RRh10shot1)  fpcvt10RRh10hold <= fpcvt10_result;
                  if (CVFPMULTIPLIER12RRh10shot2)  CVFPMULTIPLIER12RRh10hold <= CVFPMULTIPLIER12_FPRR;
                  if (fpcvt12RRh10shot1)  fpcvt12RRh10hold <= fpcvt12_result;
                  
              case (test6710PC10nz)
                  32'h1/*1:test6710PC10nz*/:  test6710PC10nz <= 32'h2/*2:test6710PC10nz*/;

                  32'h2/*2:test6710PC10nz*/:  test6710PC10nz <= 32'h3/*3:test6710PC10nz*/;

                  32'h4/*4:test6710PC10nz*/:  test6710PC10nz <= 32'h5/*5:test6710PC10nz*/;

                  32'h5/*5:test6710PC10nz*/:  test6710PC10nz <= 32'h6/*6:test6710PC10nz*/;

                  32'h6/*6:test6710PC10nz*/:  test6710PC10nz <= 32'h7/*7:test6710PC10nz*/;

                  32'h7/*7:test6710PC10nz*/:  test6710PC10nz <= 32'h8/*8:test6710PC10nz*/;

                  32'h9/*9:test6710PC10nz*/:  test6710PC10nz <= 32'ha/*10:test6710PC10nz*/;

                  32'ha/*10:test6710PC10nz*/:  test6710PC10nz <= 32'hb/*11:test6710PC10nz*/;

                  32'hb/*11:test6710PC10nz*/:  test6710PC10nz <= 32'hc/*12:test6710PC10nz*/;

                  32'hc/*12:test6710PC10nz*/:  test6710PC10nz <= 32'hd/*13:test6710PC10nz*/;
              endcase
               test6710PC10nz_pc_export <= test6710PC10nz;
               fpcvt12RRh10shot1 <= fpcvt12RRh10shot0;
               CVFPMULTIPLIER12RRh10shot1 <= CVFPMULTIPLIER12RRh10shot0;
               CVFPMULTIPLIER12RRh10shot2 <= CVFPMULTIPLIER12RRh10shot1;
               fpcvt10RRh10shot1 <= fpcvt10RRh10shot0;
               CVFPMULTIPLIER10RRh10shot1 <= CVFPMULTIPLIER10RRh10shot0;
               CVFPMULTIPLIER10RRh10shot2 <= CVFPMULTIPLIER10RRh10shot1;
               CVFPMULTIPLIER10RRh10shot0 <= (T403_SineCosTest_run_0_4_V_0<32'sd1) && (32'h5/*5:test6710PC10nz*/==test6710PC10nz);
               fpcvt10RRh10shot0 <= (T403_SineCosTest_run_0_4_V_0<32'sd1) && (32'h3/*3:test6710PC10nz*/==test6710PC10nz);
               CVFPMULTIPLIER12RRh10shot0 <= (T403_SineCosTest_run_0_4_V_0>=32'sd1) && (32'ha/*10:test6710PC10nz*/==test6710PC10nz);
               fpcvt12RRh10shot0 <= (T403_SineCosTest_run_0_4_V_0>=32'sd1) && (32'h3/*3:test6710PC10nz*/==test6710PC10nz);
               end 
              //End structure cvtToVerilogtest67/1.0


       end 
      

  CV_FP_CVT_FL2_F64_I32 fpcvt10(
        .clk(clk),
        .reset(reset),
        .result(fpcvt10_result),
        .arg(fpcvt10_arg),
        .FAIL(fpcvt10_FAIL
));
  CV_FP_FL3_MULTIPLIER_DP CVFPMULTIPLIER10(
        .clk(clk),
        .reset(reset),
        .RR(CVFPMULTIPLIER10_FPRR),
        .XX(CVFPMULTIPLIER10_XX
),
        .YY(CVFPMULTIPLIER10_YY),
        .FAIL(CVFPMULTIPLIER10_fail));
  CV_FP_CVT_FL2_F64_I32 fpcvt12(
        .clk(clk),
        .reset(reset),
        .result(fpcvt12_result),
        .arg(fpcvt12_arg),
        .FAIL(fpcvt12_FAIL
));
  CV_FP_FL3_MULTIPLIER_DP CVFPMULTIPLIER12(
        .clk(clk),
        .reset(reset),
        .RR(CVFPMULTIPLIER12_FPRR),
        .XX(CVFPMULTIPLIER12_XX
),
        .YY(CVFPMULTIPLIER12_YY),
        .FAIL(CVFPMULTIPLIER12_fail));
// 1 vectors of width 4
// 10 vectors of width 1
// 8 vectors of width 64
// 2 vectors of width 32
// 32 bits in scalar variables
// Total state bits in module = 622 bits.
// 260 continuously assigned (wire/non-state) bits 
//   cell CV_FP_CVT_FL2_F64_I32 count=2
//   cell CV_FP_FL3_MULTIPLIER_DP count=2
// Total number of leaf cells = 4
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.4h : 24th Dec 2017
//10/01/2018 00:52:03
//Cmd line args:  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test67.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test67.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


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
//KiwiC: front end input processing of class or method called bench
//
//root_walk start thread at a static method (used as an entry point). Method name=bench/.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) uid=cctor10 full_idl=bench..cctor
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
//Performing root elaboration of method bench.Main
//
//KiwiC start_thread (or entry point) uid=Main10 full_idl=bench.Main
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
//   srcfile=test67.exe
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
//PC codings points for test6710PC10 
//*-------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause           | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:test6710PC10" | 811 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:test6710PC10" | 810 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:test6710PC10" | 809 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:test6710PC10" | 807 | 3       | hwm=0.5.0   | 13   |        | 9     | 13  | 1    |
//| XU32'4:"4:test6710PC10" | 808 | 3       | hwm=0.5.0   | 8    |        | 4     | 8   | 14   |
//| XU32'8:"8:test6710PC10" | 806 | 14      | hwm=0.0.0   | 14   |        | -     | -   | -    |
//*-------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'0:"0:test6710PC10" 811 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=test6710PC10 state=XU32'0:"0:test6710PC10"
//res2: nopipeline: Thread=test6710PC10 state=XU32'0:"0:test6710PC10"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                         |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                              |
//| F0   | 811 | R0 DATA |                                                                                                                              |
//| F0+E | 811 | W0 DATA | T403.SineCosTest.run.0.4.V_0 te=te:F0 write(3) done te=te:F0 write(U1'0)  PLI:Test67  sine and cos...  PLI:Test67  starting. |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'1:"1:test6710PC10" 810 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=test6710PC10 state=XU32'1:"1:test6710PC10"
//res2: nopipeline: Thread=test6710PC10 state=XU32'1:"1:test6710PC10"
//*------+-----+---------+------*
//| pc   | eno | Phaser  | Work |
//*------+-----+---------+------*
//| F1   | -   | R0 CTRL |      |
//| F1   | 810 | R0 DATA |      |
//| F1+E | 810 | W0 DATA |      |
//*------+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'2:"2:test6710PC10" 809 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=test6710PC10 state=XU32'2:"2:test6710PC10"
//res2: nopipeline: Thread=test6710PC10 state=XU32'2:"2:test6710PC10"
//*------+-----+---------+------*
//| pc   | eno | Phaser  | Work |
//*------+-----+---------+------*
//| F2   | -   | R0 CTRL |      |
//| F2   | 809 | R0 DATA |      |
//| F2+E | 809 | W0 DATA |      |
//*------+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'4:"4:test6710PC10" 807 :  major_start_pcl=3   edge_private_start/end=9/13 exec=13 (dend=5)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'4:"4:test6710PC10" 808 :  major_start_pcl=3   edge_private_start/end=4/8 exec=8 (dend=5)
//Simple greedy schedule for res2: nopipeline: Thread=test6710PC10 state=XU32'4:"4:test6710PC10"
//res2: nopipeline: Thread=test6710PC10 state=XU32'4:"4:test6710PC10"
//*-------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                                         |
//*-------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//| F3    | -   | R0 CTRL |                                                                                                                              |
//| F3    | 808 | R0 DATA | fpcvt10 te=te:F3 cvt(E1)                                                                                                     |
//| F4    | 808 | R1 DATA |                                                                                                                              |
//| F5    | 808 | R2 DATA | CVFPMULTIPLIER10 te=te:F5 *fixed-func-ALU*(1.9, E2)                                                                          |
//| F6    | 808 | R3 DATA |                                                                                                                              |
//| F7    | 808 | R4 DATA |                                                                                                                              |
//| F8    | 808 | R5 DATA |                                                                                                                              |
//| F8+E  | 808 | W0 DATA | T403.SineCosTest.run.0.4.V_0 te=te:F8 write(E3) done te=te:F8 write(U1'1)  PLI:Test67 finished.  PLI:  sine and cosine %f... |
//| F3    | 807 | R0 DATA | fpcvt12 te=te:F3 cvt(E1)                                                                                                     |
//| F9    | 807 | R1 DATA |                                                                                                                              |
//| F10   | 807 | R2 DATA | CVFPMULTIPLIER12 te=te:F10 *fixed-func-ALU*(1.9, E2)                                                                         |
//| F11   | 807 | R3 DATA |                                                                                                                              |
//| F12   | 807 | R4 DATA |                                                                                                                              |
//| F13   | 807 | R5 DATA |                                                                                                                              |
//| F13+E | 807 | W0 DATA | T403.SineCosTest.run.0.4.V_0 te=te:F13 write(E3)  PLI:  sine and cosine %f...                                                |
//*-------+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test6710PC10 state=XU32'8:"8:test6710PC10" 806 :  major_start_pcl=14   edge_private_start/end=-1/-1 exec=14 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=test6710PC10 state=XU32'8:"8:test6710PC10"
//res2: nopipeline: Thread=test6710PC10 state=XU32'8:"8:test6710PC10"
//*-------+-----+---------+-----------------------*
//| pc    | eno | Phaser  | Work                  |
//*-------+-----+---------+-----------------------*
//| F14   | -   | R0 CTRL |                       |
//| F14   | 806 | R0 DATA |                       |
//| F14+E | 806 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+-----+---------+-----------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= T403.SineCosTest.run.0.4.V_0
//
//  E2 =.= CVT(C64f)(T403.SineCosTest.run.0.4.V_0)
//
//  E3 =.= -1+T403.SineCosTest.run.0.4.V_0
//
//  E4 =.= T403.SineCosTest.run.0.4.V_0<1
//
//  E5 =.= T403.SineCosTest.run.0.4.V_0>=1
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test67 to test67

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 4
//
//10 vectors of width 1
//
//8 vectors of width 64
//
//2 vectors of width 32
//
//32 bits in scalar variables
//
//Total state bits in module = 622 bits.
//
//260 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem/Kiwi/.cctor uid=cctor14 has 3 CIL instructions in 1 basic blocks
//Thread System/BitConverter/.cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread bench/.cctor uid=cctor10 has 2 CIL instructions in 1 basic blocks
//Thread bench/Main uid=Main10 has 23 CIL instructions in 4 basic blocks
//Thread mpc10 has 5 bevelab control states (pauses)
//Reindexed thread test6710PC10 with 15 minor control states
// eof (HPR L/S Verilog)
