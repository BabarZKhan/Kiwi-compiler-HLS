

// CBG Orangepath HPR L/S System

// Verilog output file generated at 15/03/2019 07:59:15
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9a : 7th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=110 Test67d.exe /home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg /*fp*/ [63:0] Test67c_KiwiFpSineCosineTest_wout_monitor,
    output reg /*fp*/ [63:0] Test67c_KiwiFpSineCosineTest_win_monitor,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTESTMAIN400PC10nz_pc_export,
    
/* portgroup= abstractionName=bondout2 pi_name=bondout0 */
output reg bondout0_REQ0,
    input bondout0_ACK0,
    input bondout0_REQRDY0,
    output reg bondout0_ACKRDY0,
    output reg [21:0] bondout0_ADDR0,
    output reg bondout0_RWBAR0,
    output reg [63:0] bondout0_WDATA0,
    input [63:0] bondout0_RDATA0,
    output reg [7:0] bondout0_LANES0,
    output reg bondout0_REQ1,
    input bondout0_ACK1,
    input bondout0_REQRDY1,
    output reg bondout0_ACKRDY1,
    output reg [21:0] bondout0_ADDR1,
    output reg bondout0_RWBAR1,
    output reg [63:0] bondout0_WDATA1,
    input [63:0] bondout0_RDATA1,
    output reg [7:0] bondout0_LANES1,
    
/* portgroup= abstractionName=bondout0 pi_name=bondout1 */
output reg bondout1_REQ0,
    input bondout1_ACK0,
    input bondout1_REQRDY0,
    output reg bondout1_ACKRDY0,
    output reg [21:0] bondout1_ADDR0,
    output reg bondout1_RWBAR0,
    output reg [63:0] bondout1_WDATA0,
    input [63:0] bondout1_RDATA0,
    output reg [7:0] bondout1_LANES0,
    output reg bondout1_REQ1,
    input bondout1_ACK1,
    input bondout1_REQRDY1,
    output reg bondout1_ACKRDY1,
    output reg [21:0] bondout1_ADDR1,
    output reg bondout1_RWBAR1,
    output reg [63:0] bondout1_WDATA1,
    input [63:0] bondout1_RDATA1,
    output reg [7:0] bondout1_LANES1);

function [63:0] hpr_flt2dbl0;//Floating-point convert single to double precision.
input [31:0] darg;
  hpr_flt2dbl0 =  ((darg & 32'h7F80_0000)==32'h7F80_0000)?
     {darg[31], {4{darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}}:
     {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};
endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3;
  reg signed [31:0] TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0;
  reg/*fp*/  [31:0] TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256;
  reg/*fp*/  [63:0] TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257;
// abstractionName=res2-contacts pi_name=KiwiFpSineCosine10
  wire KiwiFpSineCosine10_Sin_SQ1_F64_ack;
  reg KiwiFpSineCosine10_Sin_SQ1_F64_req;
  wire [63:0] KiwiFpSineCosine10_Sin_SQ1_F64_return;
  reg [63:0] KiwiFpSineCosine10_Sin_SQ1_F64_xd;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_10_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_YY;
  wire ifMULTIPLIERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F64I32_10
  wire/*fp*/  [63:0] CVFPCVTFL2F64I32_10_result;
  reg signed [31:0] CVFPCVTFL2F64I32_10_arg;
  wire CVFPCVTFL2F64I32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_MULTIPLIER_SP
  wire/*fp*/  [31:0] ifMULTIPLIERALUF32_10_RR;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_10_XX;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_10_YY;
  wire ifMULTIPLIERALUF32_10_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F32I32_10
  wire/*fp*/  [31:0] CVFPCVTFL2F32I32_10_result;
  reg signed [31:0] CVFPCVTFL2F32I32_10_arg;
  wire CVFPCVTFL2F32I32_10_FAIL;
// abstractionName=res2-morenets
  reg kiwiTESTMAIN400PC10_stall;
  reg kiwiTESTMAIN400PC10_clear;
  reg KiwiFpSineCosine10SinSQ1F64returnh10primed;
  reg KiwiFpSineCosine10SinSQ1F64returnh10vld;
  reg signed [63:0] KiwiFpSineCosine10SinSQ1F64returnh10hold;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh10hold;
  reg ifMULTIPLIERALUF6410RRh10shot0;
  reg ifMULTIPLIERALUF6410RRh10shot1;
  reg ifMULTIPLIERALUF6410RRh10shot2;
  reg/*fp*/  [63:0] CVFPCVTFL2F64I3210resulth10hold;
  reg CVFPCVTFL2F64I3210resulth10shot0;
  reg CVFPCVTFL2F64I3210resulth10shot1;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF3210RRh10hold;
  reg ifMULTIPLIERALUF3210RRh10shot0;
  reg ifMULTIPLIERALUF3210RRh10shot1;
  reg ifMULTIPLIERALUF3210RRh10shot2;
  reg ifMULTIPLIERALUF3210RRh10shot3;
  reg ifMULTIPLIERALUF3210RRh10shot4;
  reg/*fp*/  [31:0] CVFPCVTFL2F32I3210resulth10hold;
  reg CVFPCVTFL2F32I3210resulth10shot0;
  reg CVFPCVTFL2F32I3210resulth10shot1;
  reg [4:0] kiwiTESTMAIN400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire/*fp*/  [31:0] hprpin500404x10;
  wire/*fp*/  [63:0] hprpin500423x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               KiwiFpSineCosine10SinSQ1F64returnh10primed <= 32'd0;
               Test67c_KiwiFpSineCosineTest_win_monitor <= 64'd0;
               Test67c_KiwiFpSineCosineTest_wout_monitor <= 64'd0;
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256 <= 32'd0;
               TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3 <= 32'd0;
               TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257 <= 64'd0;
               TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256 <= 32'h0;
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3 <= 32'sh0;
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257 <= 64'h0;
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0 <= 32'sh0;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall) $display("Test67d starting. Kiwi SineCosineTest");
                               kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $display("-------------------------------------------");
                                  $display("Test67c  Sine/Cosine Double-Precision Test.");
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0 <= 32'sh6;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h14/*20:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? 1'd1: (32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                          ) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                          ) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)) && !kiwiTESTMAIN400PC10_stall)  TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256
                               <= hprpin500404x10;

                               kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? 1'd1: (32'sd4
                  !=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                  )) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                  )) && hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $display("S/P Test Sine/Cosine  arg=%f  result=%f", $bitstoreal(hpr_flt2dbl0(TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256
                                  )), $bitstoreal(hpr_flt2dbl0(32'h3f9e_0610)));
                                  $display("\n\n");
                                   Test67c_KiwiFpSineCosineTest_win_monitor <= hpr_flt2dbl0(TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_256
                                  );

                                   Test67c_KiwiFpSineCosineTest_wout_monitor <= 64'h3ff3_c0c2_0000_0000;
                                   TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3 <= $signed(-32'sd1+TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                                  );

                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'hd/*13:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $finish(32'sd0);
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h17/*23:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'hf/*15:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h10/*16:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h10/*16:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h11/*17:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h11/*17:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h12/*18:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h12/*18:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h13/*19:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h13/*19:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 1'd1: (32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                          ) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                          ) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)) && !kiwiTESTMAIN400PC10_stall)  TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
                               <= hprpin500423x10;

                               kiwiTESTMAIN400PC10nz <= 32'h15/*21:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'he/*14:kiwiTESTMAIN400PC10nz*/: if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 1'd1: (32'sd4
                  !=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                  )) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                  )) && hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14) if ((32'sd2==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                      ))  begin 
                              if (!kiwiTESTMAIN400PC10_stall)  begin 
                                      $display("-------------------------------------------");
                                      $display("Test67d finished.");
                                       end 
                                       kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                               end 
                               else  kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h15/*21:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h16/*22:kiwiTESTMAIN400PC10nz*/;
                           KiwiFpSineCosine10SinSQ1F64returnh10primed <= !kiwiTESTMAIN400PC10_stall;
                           end 
                          
                  32'h14/*20:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14) if ((32'sd2==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                      ))  begin 
                              if (!kiwiTESTMAIN400PC10_stall)  begin 
                                      $display("-------------------------------------------");
                                      $display("Test67d  Sine/Cosine Single-Precision Test.");
                                       TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3 <= 32'sh6;
                                       end 
                                       kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                               end 
                               else  kiwiTESTMAIN400PC10nz <= 32'he/*14:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h16/*22:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  if (!KiwiFpSineCosine10SinSQ1F64returnh10vld && !KiwiFpSineCosine10_Sin_SQ1_F64_ack)  begin 
                                          $display("D/P Test Sine/Cosine  arg=%f  result=%f", $bitstoreal(TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
                                          ), $bitstoreal((KiwiFpSineCosine10SinSQ1F64returnh10vld? KiwiFpSineCosine10SinSQ1F64returnh10hold
                                          : KiwiFpSineCosine10_Sin_SQ1_F64_return)));
                                          $display("\n\n");
                                           end 
                                          if (KiwiFpSineCosine10SinSQ1F64returnh10vld || KiwiFpSineCosine10_Sin_SQ1_F64_ack)  begin 
                                          $display("D/P Test Sine/Cosine  arg=%f  result=%f", $bitstoreal(TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
                                          ), $bitstoreal((KiwiFpSineCosine10SinSQ1F64returnh10vld? KiwiFpSineCosine10SinSQ1F64returnh10hold
                                          : KiwiFpSineCosine10_Sin_SQ1_F64_return)));
                                          $display("\n\n");
                                           Test67c_KiwiFpSineCosineTest_win_monitor <= TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
                                          ;

                                           Test67c_KiwiFpSineCosineTest_wout_monitor <= (KiwiFpSineCosine10SinSQ1F64returnh10vld? KiwiFpSineCosine10SinSQ1F64returnh10hold
                                          : KiwiFpSineCosine10_Sin_SQ1_F64_return);

                                           TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0 <= $signed(-32'sd1+TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                                          );

                                           end 
                                          if (!KiwiFpSineCosine10SinSQ1F64returnh10vld && !KiwiFpSineCosine10_Sin_SQ1_F64_ack)  begin 
                                           Test67c_KiwiFpSineCosineTest_win_monitor <= TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
                                          ;

                                           Test67c_KiwiFpSineCosineTest_wout_monitor <= (KiwiFpSineCosine10SinSQ1F64returnh10vld? KiwiFpSineCosine10SinSQ1F64returnh10hold
                                          : KiwiFpSineCosine10_Sin_SQ1_F64_return);

                                           TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0 <= $signed(-32'sd1+TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                                          );

                                           end 
                                           end 
                                  if (KiwiFpSineCosine10SinSQ1F64returnh10vld || KiwiFpSineCosine10_Sin_SQ1_F64_ack)  kiwiTESTMAIN400PC10nz
                               <= 32'h14/*20:kiwiTESTMAIN400PC10nz*/;

                               end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               CVFPCVTFL2F32I3210resulth10hold <= 32'd0;
               CVFPCVTFL2F32I3210resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10hold <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot4 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10hold <= 64'd0;
               CVFPCVTFL2F64I3210resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot2 <= 32'd0;
               KiwiFpSineCosine10SinSQ1F64returnh10primed <= 32'd0;
               KiwiFpSineCosine10SinSQ1F64returnh10vld <= 32'd0;
               KiwiFpSineCosine10SinSQ1F64returnh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot0 <= 32'd0;
               CVFPCVTFL2F32I3210resulth10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14)  begin 
                  if (KiwiFpSineCosine10_Sin_SQ1_F64_ack && KiwiFpSineCosine10SinSQ1F64returnh10primed)  begin 
                           KiwiFpSineCosine10SinSQ1F64returnh10primed <= 32'd0;
                           KiwiFpSineCosine10SinSQ1F64returnh10vld <= 32'd1;
                           KiwiFpSineCosine10SinSQ1F64returnh10hold <= KiwiFpSineCosine10_Sin_SQ1_F64_return;
                           end 
                          if (!kiwiTESTMAIN400PC10_stall && kiwiTESTMAIN400PC10_clear)  KiwiFpSineCosine10SinSQ1F64returnh10vld <= 32'd0
                      ;

                      if (ifMULTIPLIERALUF6410RRh10shot2)  ifMULTIPLIERALUF6410RRh10hold <= ifMULTIPLIERALUF64_10_RR;
                      if (CVFPCVTFL2F64I3210resulth10shot1)  CVFPCVTFL2F64I3210resulth10hold <= CVFPCVTFL2F64I32_10_result;
                      if (ifMULTIPLIERALUF3210RRh10shot4)  ifMULTIPLIERALUF3210RRh10hold <= ifMULTIPLIERALUF32_10_RR;
                      if (CVFPCVTFL2F32I3210resulth10shot1)  CVFPCVTFL2F32I3210resulth10hold <= CVFPCVTFL2F32I32_10_result;
                       kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   CVFPCVTFL2F32I3210resulth10shot1 <= CVFPCVTFL2F32I3210resulth10shot0;
                   ifMULTIPLIERALUF3210RRh10shot1 <= ifMULTIPLIERALUF3210RRh10shot0;
                   ifMULTIPLIERALUF3210RRh10shot2 <= ifMULTIPLIERALUF3210RRh10shot1;
                   ifMULTIPLIERALUF3210RRh10shot3 <= ifMULTIPLIERALUF3210RRh10shot2;
                   ifMULTIPLIERALUF3210RRh10shot4 <= ifMULTIPLIERALUF3210RRh10shot3;
                   CVFPCVTFL2F64I3210resulth10shot1 <= CVFPCVTFL2F64I3210resulth10shot0;
                   ifMULTIPLIERALUF6410RRh10shot1 <= ifMULTIPLIERALUF6410RRh10shot0;
                   ifMULTIPLIERALUF6410RRh10shot2 <= ifMULTIPLIERALUF6410RRh10shot1;
                   ifMULTIPLIERALUF6410RRh10shot0 <= (32'h10/*16:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !kiwiTESTMAIN400PC10_stall
                  ;

                   CVFPCVTFL2F64I3210resulth10shot0 <= (((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 1'd1: (32'sd4
                  !=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                  )) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
                  )) && !kiwiTESTMAIN400PC10_stall && (32'he/*14:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

                   ifMULTIPLIERALUF3210RRh10shot0 <= (32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !kiwiTESTMAIN400PC10_stall
                  ;

                   CVFPCVTFL2F32I3210resulth10shot0 <= (((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? 1'd1: (32'sd4
                  !=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                  )) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
                  )) && !kiwiTESTMAIN400PC10_stall && (32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

 always   @(* )  begin 
       ifMULTIPLIERALUF32_10_XX = 32'sd0;
       ifMULTIPLIERALUF32_10_YY = 32'sd0;
       CVFPCVTFL2F32I32_10_arg = 32'sd0;
       ifMULTIPLIERALUF64_10_XX = 32'sd0;
       ifMULTIPLIERALUF64_10_YY = 32'sd0;
       CVFPCVTFL2F64I32_10_arg = 32'sd0;
       KiwiFpSineCosine10_Sin_SQ1_F64_xd = 32'sd0;
       KiwiFpSineCosine10_Sin_SQ1_F64_req = 32'sd0;
       hpr_int_run_enable_DDX14 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (!kiwiTESTMAIN400PC10_stall)  begin 
                   ifMULTIPLIERALUF32_10_XX = 32'h3ff3_3333;
                   ifMULTIPLIERALUF32_10_YY = ((32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I32_10_result: CVFPCVTFL2F32I3210resulth10hold
                  );

                   end 
                  
          32'h3/*3:kiwiTESTMAIN400PC10nz*/: if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? 1'd1: (32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
          ) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
          ) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)) && !kiwiTESTMAIN400PC10_stall)  CVFPCVTFL2F32I32_10_arg
               = TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3;

              
          32'h10/*16:kiwiTESTMAIN400PC10nz*/: if (!kiwiTESTMAIN400PC10_stall)  begin 
                   ifMULTIPLIERALUF64_10_XX = 64'h3ffe_6666_6666_6666;
                   ifMULTIPLIERALUF64_10_YY = ((32'h10/*16:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F64I32_10_result
                  : CVFPCVTFL2F64I3210resulth10hold);

                   end 
                  
          32'he/*14:kiwiTESTMAIN400PC10nz*/: if ((((32'sd6==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 1'd1: (32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
          ) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)) || (32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0
          ) || (32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)) && !kiwiTESTMAIN400PC10_stall)  CVFPCVTFL2F64I32_10_arg
               = TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0;

              
          32'h15/*21:kiwiTESTMAIN400PC10nz*/: if (!kiwiTESTMAIN400PC10_stall)  KiwiFpSineCosine10_Sin_SQ1_F64_xd = TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_SPILL_257
              ;

              endcase
      if (!kiwiTESTMAIN400PC10_stall && hpr_int_run_enable_DDX14)  KiwiFpSineCosine10_Sin_SQ1_F64_req = ((32'h15/*21:kiwiTESTMAIN400PC10nz*/==
          kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);

           hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       bondout0_REQ0 = 32'd0;
       bondout0_ACKRDY0 = 32'd0;
       bondout0_ADDR0 = 32'd0;
       bondout0_RWBAR0 = 32'd0;
       bondout0_WDATA0 = 64'd0;
       bondout0_LANES0 = 32'd0;
       bondout0_REQ1 = 32'd0;
       bondout0_ACKRDY1 = 32'd0;
       bondout0_ADDR1 = 32'd0;
       bondout0_RWBAR1 = 32'd0;
       bondout0_WDATA1 = 64'd0;
       bondout0_LANES1 = 32'd0;
       bondout1_REQ0 = 32'd0;
       bondout1_ACKRDY0 = 32'd0;
       bondout1_ADDR0 = 32'd0;
       bondout1_RWBAR0 = 32'd0;
       bondout1_WDATA0 = 64'd0;
       bondout1_LANES0 = 32'd0;
       bondout1_REQ1 = 32'd0;
       bondout1_ACKRDY1 = 32'd0;
       bondout1_ADDR1 = 32'd0;
       bondout1_RWBAR1 = 32'd0;
       bondout1_WDATA1 = 64'd0;
       bondout1_LANES1 = 32'd0;
       end 
      

always @(*) kiwiTESTMAIN400PC10_clear = (32'h0/*0:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h2/*2:kiwiTESTMAIN400PC10nz*/==
kiwiTESTMAIN400PC10nz) || (32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'hb/*11:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
) || (32'hc/*12:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'hd/*13:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'he
/*14:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h15/*21:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h14/*20:kiwiTESTMAIN400PC10nz*/==
kiwiTESTMAIN400PC10nz);

always @(*) kiwiTESTMAIN400PC10_stall = (32'h16/*22:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !KiwiFpSineCosine10SinSQ1F64returnh10vld && !KiwiFpSineCosine10_Sin_SQ1_F64_ack
;

  KiwiFpSineCosine KiwiFpSineCosine10(
        .clk(clk),
        .reset(reset),
        .Sin_SQ1_F64_ack(KiwiFpSineCosine10_Sin_SQ1_F64_ack
),
        .Sin_SQ1_F64_req(KiwiFpSineCosine10_Sin_SQ1_F64_req),
        .Sin_SQ1_F64_return(KiwiFpSineCosine10_Sin_SQ1_F64_return),
        .Sin_SQ1_F64_xd(KiwiFpSineCosine10_Sin_SQ1_F64_xd
));
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_10_RR),
        .XX(ifMULTIPLIERALUF64_10_XX
),
        .YY(ifMULTIPLIERALUF64_10_YY),
        .FAIL(ifMULTIPLIERALUF64_10_FAIL));
  CV_FP_CVT_FL2_F64_I32 CVFPCVTFL2F64I32_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F64I32_10_result),
        .arg(CVFPCVTFL2F64I32_10_arg
),
        .FAIL(CVFPCVTFL2F64I32_10_FAIL));
  CV_FP_FL5_MULTIPLIER_SP ifMULTIPLIERALUF32_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF32_10_RR),
        .XX(ifMULTIPLIERALUF32_10_XX
),
        .YY(ifMULTIPLIERALUF32_10_YY),
        .FAIL(ifMULTIPLIERALUF32_10_FAIL));
  CV_FP_CVT_FL2_F32_I32 CVFPCVTFL2F32I32_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F32I32_10_result),
        .arg(CVFPCVTFL2F32I32_10_arg
),
        .FAIL(CVFPCVTFL2F32I32_10_FAIL));
assign hprpin500404x10 = ((32'sd6!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3) && 
(32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? ((32'ha/*10:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_10_RR
: ifMULTIPLIERALUF3210RRh10hold): ((32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3)? 32'h447a_0000: ((32'sd5==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_3
)? 32'h3c23_d70a: 32'h4000_0000)));

assign hprpin500423x10 = ((32'sd6!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) && (32'sd5!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0) && 
(32'sd4!=TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? ((32'h13/*19:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF64_10_RR
: ifMULTIPLIERALUF6410RRh10hold): ((32'sd4==TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 64'h408f_4000_0000_0000: ((32'sd5==
TESTMAIN400_Test67c_KiwiFpSineCosineTest_run_0_6_V_0)? 64'h3f84_7ae1_47ae_147b: 64'h4000_0000_0000_0000)));

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 5
// 18 vectors of width 1
// 9 vectors of width 32
// 7 vectors of width 64
// Total state bits in module = 759 bits.
// 357 continuously assigned (wire/non-state) bits 
//   cell KiwiFpSineCosine count=1
//   cell CV_FP_FL3_MULTIPLIER_DP count=1
//   cell CV_FP_CVT_FL2_F64_I32 count=1
//   cell CV_FP_FL5_MULTIPLIER_SP count=1
//   cell CV_FP_CVT_FL2_F32_I32 count=1
// Total number of leaf cells = 5
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9a : 7th March 2019
//15/03/2019 07:59:12
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=110 Test67d.exe /home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*---------------------------------------+-----------+--------------------------------------------------------------------+---------------+-------------------------------------------+--------+-------*
//| Class                                 | Style     | Dir Style                                                          | Timing Target | Method                                    | UID    | Skip  |
//*---------------------------------------+-----------+--------------------------------------------------------------------+---------------+-------------------------------------------+--------+-------*
//| HprlsMathsPrimsCrude.KiwiFpSineCosine | MM_remote | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin | Sin10  | true  |
//|                                       |           | dmode, enable/directorate-pc-export                                |               |                                           |        |       |
//| Test67d                               | MM_root   | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | Test67d.Main                              | Main10 | false |
//|                                       |           | dmode, enable/directorate-pc-export                                |               |                                           |        |       |
//*---------------------------------------+-----------+--------------------------------------------------------------------+---------------+-------------------------------------------+--------+-------*

//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| AddressSpace | Name       | Protocol                      | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*

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

//Report from IP-XACT input/output:::
//Read IP-XACT component definition HprlsMathsPrimsCrude.KiwiFpSineCosine from ./KiwiFpSineCosine.xml

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busDefinition AUTOMETA_Sin_SQ1_F64

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busAbstraction AUTOMETA_Sin_SQ1_F64_rtl

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
//| bondout-loadstore-port-count     | 1     |             |
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
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class Test67d  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test67d.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Test67d.Main
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
//   bondout-loadstore-port-count=1
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
//   ?>?=srcfile, Test67d.exe, /home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll
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
//Bondout Load/Store (and other) Ports 'Res2 Preliminary'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

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

//Report from IP-XACT input/output:::
//Read IP-XACT component definition KiwiFpSineCosine from ./KiwiFpSineCosine.xml

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busDefinition AUTOMETA_Sin_SQ1_F64

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busAbstraction AUTOMETA_Sin_SQ1_F64_rtl

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN400PC10 
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                      | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"     | 823 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"     | 822 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"     | 821 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 20   |
//| XU32'16:"16:kiwiTESTMAIN400PC10"   | 816 | 3       | hwm=0.7.0   | 10   |        | 4     | 10  | 12   |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 817 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 3    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 818 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 13   |
//| XU32'32:"32:kiwiTESTMAIN400PC10"   | 815 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 11   |
//| XU32'64:"64:kiwiTESTMAIN400PC10"   | 814 | 13      | hwm=0.0.0   | 13   |        | -     | -   | -    |
//| XU32'128:"128:kiwiTESTMAIN400PC10" | 813 | 14      | hwm=0.5.0   | 19   |        | 15    | 19  | 21   |
//| XU32'4:"4:kiwiTESTMAIN400PC10"     | 819 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 11   |
//| XU32'4:"4:kiwiTESTMAIN400PC10"     | 820 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 14   |
//| XU32'256:"256:kiwiTESTMAIN400PC10" | 812 | 21      | hwm=0.1.0   | 22   |        | 22    | 22  | 20   |
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                            |
//| F0   | E823 | R0 DATA |                                                                                                                            |
//| F0+E | E823 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0write(S32'0) TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.\ |
//|      |      |         | 257write(??64'0) TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3write(S32'0) TESTMAIN400.Test67c_KiwiFpSineCosineTes\ |
//|      |      |         | t.run.0.6._SPILL.256write(??32'0)                                                                                          |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F1   | -    | R0 CTRL |                              |
//| F1   | E822 | R0 DATA |                              |
//| F1+E | E822 | W0 DATA |  PLI:Test67d starting. Ki... |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                                                            |
//| F2   | E821 | R0 DATA |                                                                                                                            |
//| F2+E | E821 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0write(S32'6)  PLI:Test67c  Sine/Cosine...  PLI:--------------------... |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F3    | -    | R0 CTRL |                                                                      |
//| F3    | E816 | R0 DATA | CVFPCVTFL2F32I32_10_hpr_flt_from_int32(E1)                           |
//| F4    | E816 | R1 DATA |                                                                      |
//| F5    | E816 | R2 DATA | ifMULTIPLIERALUF32_10_compute(1.9f, E2)                              |
//| F6    | E816 | R3 DATA |                                                                      |
//| F7    | E816 | R4 DATA |                                                                      |
//| F8    | E816 | R5 DATA |                                                                      |
//| F9    | E816 | R6 DATA |                                                                      |
//| F10   | E816 | R7 DATA |                                                                      |
//| F10+E | E816 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.256write(E3) |
//*-------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                     |
//| F11   | E818 | R0 DATA |                                                     |
//| F11+E | E818 | W0 DATA |  PLI:Test67d finished.  PLI:--------------------... |
//| F11   | E817 | R0 DATA |                                                     |
//| F11+E | E817 | W0 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*
//| F12   | -    | R0 CTRL |                                                                                                                                                     |
//| F12   | E815 | R0 DATA |                                                                                                                                                     |
//| F12+E | E815 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3write(E4) Test67c_KiwiFpSineCosineTest.wout_monitorwrite(??64'4608238783186337792) Test67c_Kiw\ |
//|       |      |         | iFpSineCosineTest.win_monitorwrite(E5)  PLI:
//
//  PLI:S/P Test Sine/Cosine...                                                                         |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F13   | -    | R0 CTRL |                       |
//| F13   | E814 | R0 DATA |                       |
//| F13+E | E814 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F14   | -    | R0 CTRL |                                                                      |
//| F14   | E813 | R0 DATA | CVFPCVTFL2F64I32_10_hpr_dbl_from_int32(E6)                           |
//| F15   | E813 | R1 DATA |                                                                      |
//| F16   | E813 | R2 DATA | ifMULTIPLIERALUF64_10_compute(1.9, E7)                               |
//| F17   | E813 | R3 DATA |                                                                      |
//| F18   | E813 | R4 DATA |                                                                      |
//| F19   | E813 | R5 DATA |                                                                      |
//| F19+E | E813 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.257write(E8) |
//*-------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                       |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                                                                                            |
//| F20   | E820 | R0 DATA |                                                                                                                            |
//| F20+E | E820 | W0 DATA |                                                                                                                            |
//| F20   | E819 | R0 DATA |                                                                                                                            |
//| F20+E | E819 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3write(S32'6)  PLI:Test67d  Sine/Cosine...  PLI:--------------------... |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'256:"256:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'256:"256:kiwiTESTMAIN400PC10"
//*---------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| pc      | eno  | Phaser  | Work                                                                                                                                  |
//*---------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| F21     | -    | R0 CTRL |                                                                                                                                       |
//| F21+S   | E812 | R0 DATA | KiwiFpSineCosine10_Sin_SQ1_F64Sin(E9)                                                                                                 |
//| F22+S   | E812 | R1 DATA |                                                                                                                                       |
//| F22+E+S | E812 | W0 DATA | TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0write(E10) Test67c_KiwiFpSineCosineTest.wout_monitorwrite(E11) Test67c_KiwiFpSin\ |
//|         |      |         | eCosineTest.win_monitorwrite(E9) TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_2write(E11) blockrefreftranfastspill10write(E11)\ |
//|         |      |         |   PLI:
//
//  PLI:D/P Test Sine/Cosine...                                                                                                 |
//*---------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout1 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Final'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3
//
//
//  E2 =.= CVT(Cf)(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3)
//
//
//  E3 =.= COND({[6!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, 5!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, 4!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3]}, Cf(1.9f*(CVT(Cf)(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3))), COND(4==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, ??32'1148846080, COND(5==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, ??32'1008981770, ??32'1073741824)))
//
//
//  E4 =.= C(-1+TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3)
//
//
//  E5 =.= CVT(C64f)(Cf(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.256))
//
//
//  E6 =.= TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0
//
//
//  E7 =.= CVT(C64f)(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0)
//
//
//  E8 =.= COND({[6!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, 5!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, 4!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0]}, C64f(1.9*(CVT(C64f)(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0))), COND(4==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, ??64'4652007308841189376, COND(5==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, ??64'4576918229304087675, ??64'4611686018427387904)))
//
//
//  E9 =.= C64f(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.257)
//
//
//  E10 =.= C(-1+TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0)
//
//
//  E11 =.= C64f(*APPLY:<KiwiFpSineCosine>.Sin_SQ1_F64{_SQ1_F64}Sin(C64f(TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6._SPILL.257)))
//
//
//  E12 =.= {[6!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, 5!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3, 4!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3]; [4==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3]; [6==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3]; [5==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3]}
//
//
//  E13 =.= 2==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3
//
//
//  E14 =.= 2!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_3
//
//
//  E15 =.= {[6!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, 5!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0, 4!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0]; [4==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0]; [6==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0]; [5==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0]}
//
//
//  E16 =.= 2!=TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0
//
//
//  E17 =.= 2==TESTMAIN400.Test67c_KiwiFpSineCosineTest.run.0.6.V_0
//
//
//  E18 =.= {[|KiwiFpSineCosine10SinSQ1F64returnh10vld|]; [|KiwiFpSineCosine10_Sin_SQ1_F64_ack|]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for Test67d to Test67d

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 5
//
//18 vectors of width 1
//
//9 vectors of width 32
//
//7 vectors of width 64
//
//Total state bits in module = 759 bits.
//
//357 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread Test67d.Main uid=Main10 has 75 CIL instructions in 24 basic blocks
//Thread mpc10 has 10 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 23 minor control states
// eof (HPR L/S Verilog)
