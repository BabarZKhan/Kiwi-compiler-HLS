

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 07:11:04
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test56b.exe -sim=18000 -kiwife-firstpass-loglevel=5 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [5:0] kiwiCORDMAIN4001PC10nz_pc_export);

function hpr_fp_dltd0; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd0_a, hpr_fp_dltd0_b;
  hpr_fp_dltd0 = ((hpr_fp_dltd0_a[63] && !hpr_fp_dltd0_b[63]) ? 1: (!hpr_fp_dltd0_a[63] && hpr_fp_dltd0_b[63])  ? 0 : (hpr_fp_dltd0_a[63]^(hpr_fp_dltd0_a[62:0]<hpr_fp_dltd0_b[62:0]))); 
   endfunction


function signed [31:0] rtl_signed_bitextract1;
   input [63:0] arg;
   rtl_signed_bitextract1 = $signed(arg[31:0]);
   endfunction


function signed [63:0] rtl_sign_extend3;
   input [31:0] arg;
   rtl_sign_extend3 = { {32{arg[31]}}, arg[31:0] };
   endfunction


function [31:0] rtl_unsigned_extend2;
   input argbit;
   rtl_unsigned_extend2 = { 31'b0, argbit };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg/*fp*/  [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8;
  reg [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6;
  reg signed [31:0] CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4;
  reg CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0;
  reg/*fp*/  [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256;
  reg [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13;
  reg [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12;
  reg signed [31:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11;
  reg CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10;
  reg CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3;
  reg signed [31:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258;
  reg signed [63:0] CordicWithIntegerAngle_sin_kernel_itheta;
  reg signed [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8;
  reg [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_6;
  reg signed [31:0] CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4;
  reg/*fp*/  [63:0] CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256;
  reg/*fp*/  [63:0] CordicWithIntegerAngle_sin_theta;
  reg [63:0] CORDMAIN400_CordicTestbench_Main_V_5;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_V_4;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_V_3;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_SPILL_257;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_SPILL_258;
// abstractionName=repack-newnets
  reg signed [63:0] A_64_SS_CC_SCALbx10_ARA0[55:0];
  reg [63:0] A_64_US_CC_SCALbx12_ARA0[35:0];
  reg [63:0] A_64_US_CC_SCALbx14_ARB0[35:0];
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_10_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_10_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_10_YY;
  wire ifADDERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_12_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_12_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_12_YY;
  wire ifADDERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_14_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_14_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_14_YY;
  wire ifADDERALUF64_14_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_16_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_16_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_16_YY;
  wire ifADDERALUF64_16_FAIL;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] ifADDERALUF6412RRh10hold;
  reg ifADDERALUF6412RRh10shot0;
  reg ifADDERALUF6412RRh10shot1;
  reg ifADDERALUF6412RRh10shot2;
  reg ifADDERALUF6412RRh10shot3;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh10hold;
  reg ifADDERALUF6410RRh10shot0;
  reg ifADDERALUF6410RRh10shot1;
  reg ifADDERALUF6410RRh10shot2;
  reg ifADDERALUF6410RRh10shot3;
  reg/*fp*/  [63:0] ifADDERALUF6414RRh10hold;
  reg ifADDERALUF6414RRh10shot0;
  reg ifADDERALUF6414RRh10shot1;
  reg ifADDERALUF6414RRh10shot2;
  reg ifADDERALUF6414RRh10shot3;
  reg/*fp*/  [63:0] ifADDERALUF6416RRh10hold;
  reg ifADDERALUF6416RRh10shot0;
  reg ifADDERALUF6416RRh10shot1;
  reg ifADDERALUF6416RRh10shot2;
  reg ifADDERALUF6416RRh10shot3;
  reg [5:0] kiwiCORDMAIN4001PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  reg [63:0] hprpin500704x10;
  reg/*fp*/  [63:0] hprpin500736x10;
  reg [63:0] hprpin500771x10;
  reg [63:0] hprpin500799x10;
  wire [63:0] hprpin500973x10;
  reg/*fp*/  [63:0] hprpin501348x10;
  wire signed [63:0] hprpin501939x10;
  wire signed [63:0] hprpin501940x10;
  wire signed [63:0] hprpin501941x10;
 always   @(* )  begin 
       ifADDERALUF64_16_XX = 32'sd0;
       ifADDERALUF64_16_YY = 32'sd0;
       ifADDERALUF64_14_XX = 32'sd0;
       ifADDERALUF64_14_YY = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_12_XX = 32'sd0;
       ifADDERALUF64_12_YY = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiCORDMAIN4001PC10nz)
          32'hc/*12:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifADDERALUF64_16_XX = 64'hc009_21fb_5444_2d11;
               ifADDERALUF64_16_YY = hprpin501348x10;
               end 
              
          32'h12/*18:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifADDERALUF64_14_XX = 64'h4009_21fb_5444_2d11;
               ifADDERALUF64_14_YY = 64'sh8000_0000_0000_0000^CordicWithIntegerAngle_sin_theta;
               end 
              
          32'h28/*40:kiwiCORDMAIN4001PC10nz*/: if (!hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta, 64'h4019_21fb_5444_2d1c))  begin 
                   ifADDERALUF64_12_XX = CordicWithIntegerAngle_sin_theta;
                   ifADDERALUF64_12_YY = 64'hc019_21fb_5444_2d1c;
                   end 
                   else  begin 
                   ifADDERALUF64_10_XX = CordicWithIntegerAngle_sin_theta;
                   ifADDERALUF64_10_YY = 64'hc009_21fb_5444_2d11;
                   end 
                  endcase
       hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.CORDMAIN400_1/1.0
      if (reset)  begin 
               kiwiCORDMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_SPILL_258 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_SPILL_257 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_V_5 <= 64'd0;
               CORDMAIN400_CordicTestbench_Main_V_3 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_V_4 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= 32'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= 32'd0;
               CordicWithIntegerAngle_sin_kernel_itheta <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_6 <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4 <= 32'd0;
               CordicWithIntegerAngle_sin_theta <= 64'd0;
               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256 <= 64'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiCORDMAIN4001PC10nz)
                  32'h0/*0:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("cordic: Testbench start");
                          $display("  theta now %f", $bitstoreal(64'h3fb9_a027_5254_60aa));
                          if (($unsigned(64'sh7fff_ffff_ffff_ffff&$unsigned(64'h3fb9_a027_5254_60aa))<64'sh3e40_0000))  begin 
                                  if (($unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500704x10)<64'sh3e40_0000))  kiwiCORDMAIN4001PC10nz <= 32'hb
                                      /*11:kiwiCORDMAIN4001PC10nz*/;

                                       done <= 32'h0;
                                   CORDMAIN400_CordicTestbench_Main_SPILL_258 <= 32'sh0;
                                   CORDMAIN400_CordicTestbench_Main_SPILL_257 <= 32'sh0;
                                   CORDMAIN400_CordicTestbench_Main_V_5 <= 64'h0;
                                   CORDMAIN400_CordicTestbench_Main_V_3 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0 <= 32'h0;
                                   CORDMAIN400_CordicTestbench_Main_V_4 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10 <= 32'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7 <= 32'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= 32'sh0;
                                   CordicWithIntegerAngle_sin_kernel_itheta <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_6 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4 <= 32'sh0;
                                   CordicWithIntegerAngle_sin_theta <= 64'h3fb9_a027_5254_60aa;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256 <= 64'h3fb9_a027_5254_60aa;
                                   end 
                                   else  begin 
                                  if (($unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500704x10)>=64'sh3e40_0000))  kiwiCORDMAIN4001PC10nz <= 32'h1
                                      /*1:kiwiCORDMAIN4001PC10nz*/;

                                       done <= 32'h0;
                                   CORDMAIN400_CordicTestbench_Main_SPILL_258 <= 32'sh0;
                                   CORDMAIN400_CordicTestbench_Main_SPILL_257 <= 32'sh0;
                                   CORDMAIN400_CordicTestbench_Main_V_5 <= 64'h0;
                                   CORDMAIN400_CordicTestbench_Main_V_3 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0 <= 32'h0;
                                   CORDMAIN400_CordicTestbench_Main_V_4 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10 <= 32'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7 <= 32'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= 32'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= 64'h0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= 32'sh0;
                                   CordicWithIntegerAngle_sin_kernel_itheta <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_6 <= $unsigned(64'sh10_0000_0000_0000|$unsigned(64'shf_ffff_ffff_ffff
                                  &$unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500704x10)));

                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4 <= rtl_signed_bitextract1(64'sh7ff&($unsigned(64'sh7fff_ffff_ffff_ffff
                                  &hprpin500704x10)>>32'sd52));

                                   CordicWithIntegerAngle_sin_theta <= 64'h3fb9_a027_5254_60aa;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256 <= 64'h0;
                                   end 
                                   end 
                          
                  32'h1/*1:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h2/*2:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_6;
                           end 
                          
                  32'h2/*2:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((32'sd1020<CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4
                      ))  begin 
                               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= $signed(64'sh2*CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8
                              );

                               CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4 <= $signed(-32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4
                              );

                               end 
                               else  kiwiCORDMAIN4001PC10nz <= 32'h3/*3:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4>=32'sd1020)) $display(" theta_i %1d   exp %1d", CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8
                              , CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4);
                               else  begin 
                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8 <= $signed((CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8
                                  >>>1));

                                   CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4
                                  );

                                   end 
                                  if ((CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_4>=32'sd1020))  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h4/*4:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= 64'sh9_b74e_da84_35e0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= 32'sh0;
                                   CordicWithIntegerAngle_sin_kernel_itheta <= CORDMAIN400_CordicWithIntegerAngle_sin_2_12_V_8;
                                   end 
                                   end 
                          
                  32'h4/*4:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                      <32'sd56))  begin 
                               kiwiCORDMAIN4001PC10nz <= 32'h5/*5:kiwiCORDMAIN4001PC10nz*/;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4 <= hprpin501941x10;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3 <= hprpin501940x10;
                               CordicWithIntegerAngle_sin_kernel_itheta <= hprpin501939x10;
                               end 
                               else  begin 
                               kiwiCORDMAIN4001PC10nz <= 32'h6/*6:kiwiCORDMAIN4001PC10nz*/;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10 <= ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                              >=32'sd56) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1<64'sh0)? 32'h1: 32'h0);

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7 <= rtl_unsigned_extend2((32'h0/*0:USA24*/==CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
                              ));

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5 <= ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                              >=32'sd56) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1<64'sh0)? $signed((0-CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
                              )): CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1);

                               end 
                              
                  32'h5/*5:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h4/*4:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3
                          ;

                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4
                          ;

                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                          );

                           end 
                          
                  32'h6/*6:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h7/*7:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= 32'sh3ff;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5
                          ;

                           end 
                          
                  32'h7/*7:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((64'sh10_0000_0000_0000<CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                      ))  kiwiCORDMAIN4001PC10nz <= 32'h8/*8:kiwiCORDMAIN4001PC10nz*/;
                           else  begin 
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= $signed(-32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                              );

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= $unsigned(64'sh2*CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                              );

                               end 
                              
                  32'h8/*8:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                          )  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'ha/*10:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= 64'h0;
                                   end 
                                  if ((64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && !CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                          )  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h9/*9:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258 <= (!CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10
                                   && (64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && !CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                                  ? 64'sh0: -64'sh8000_0000_0000_0000);

                                   end 
                                  if ((64'sh20_0000_0000_0000<CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12))  begin 
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                                  );

                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= $unsigned((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                                  >>1));

                                   end 
                                   end 
                          
                  32'h9/*9:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'ha/*10:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= $unsigned(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258
                          |64'shf_ffff_ffff_ffff&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12|(rtl_sign_extend3(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                          )<<32'sd52));

                           end 
                          
                  32'ha/*10:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'hb/*11:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256 <= hprpin500736x10;
                           end 
                          
                  32'hb/*11:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("t0 %f  Sin=%f  cordic=%f", $bitstoreal(64'h3fb9_a027_5254_60aa), $bitstoreal(64'h3ff3_c0be_9c32_7b27
                          ), $bitstoreal(CORDMAIN400_CordicWithIntegerAngle_sin_2_12_SPILL_256));
                          $display("Please return NaN outside supported range");
                           kiwiCORDMAIN4001PC10nz <= 32'h25/*37:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_V_3 <= 32'sh0;
                           CORDMAIN400_CordicTestbench_Main_V_4 <= 32'sh1;
                           end 
                          
                  32'hd/*13:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'he/*14:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'he/*14:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hf/*15:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'hf/*15:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h10/*16:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h10/*16:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h11/*17:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0 <= (hpr_fp_dltd0(hprpin501348x10, 64'h0)? 32'h1: 32'h0);
                           CordicWithIntegerAngle_sin_theta <= (hpr_fp_dltd0(hprpin501348x10, 64'h0)? ((32'h10/*16:kiwiCORDMAIN4001PC10nz*/==
                          kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_16_RR: ifADDERALUF6416RRh10hold): hprpin501348x10);

                           end 
                          
                  32'h11/*17:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h28/*40:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h13/*19:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h14/*20:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h14/*20:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h15/*21:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h15/*21:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h16/*22:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h16/*22:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (!hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta, 64'h3ff9_21fb_5444_2d28))  CordicWithIntegerAngle_sin_theta
                               <= ((32'h16/*22:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_14_RR: ifADDERALUF6414RRh10hold
                              );

                               kiwiCORDMAIN4001PC10nz <= 32'h17/*23:kiwiCORDMAIN4001PC10nz*/;
                           end 
                          
                  32'h17/*23:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("  theta now %f", $bitstoreal(CordicWithIntegerAngle_sin_theta));
                          if (($unsigned(64'sh7fff_ffff_ffff_ffff&$unsigned(CordicWithIntegerAngle_sin_theta))<64'sh3e40_0000))  begin 
                                  if (($unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500771x10)<64'sh3e40_0000))  kiwiCORDMAIN4001PC10nz <= 32'h23
                                      /*35:kiwiCORDMAIN4001PC10nz*/;

                                       CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256 <= CordicWithIntegerAngle_sin_theta;
                                   end 
                                   else  begin 
                                  if (($unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500771x10)>=64'sh3e40_0000))  kiwiCORDMAIN4001PC10nz <= 32'h18
                                      /*24:kiwiCORDMAIN4001PC10nz*/;

                                       CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6 <= $unsigned(64'sh10_0000_0000_0000|$unsigned(64'shf_ffff_ffff_ffff
                                  &$unsigned(64'sh7fff_ffff_ffff_ffff&hprpin500771x10)));

                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= rtl_signed_bitextract1(64'sh7ff&($unsigned(64'sh7fff_ffff_ffff_ffff
                                  &hprpin500771x10)>>32'sd52));

                                   end 
                                   end 
                          
                  32'h18/*24:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h19/*25:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_6;
                           end 
                          
                  32'h19/*25:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((32'sd1020<CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4
                      ))  begin 
                               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= $signed(64'sh2*CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8
                              );

                               CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= $signed(-32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4
                              );

                               end 
                               else  kiwiCORDMAIN4001PC10nz <= 32'h1a/*26:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1a/*26:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4>=32'sd1020)) $display(" theta_i %1d   exp %1d", CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8
                              , CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4);
                               else  begin 
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8 <= $signed((CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8
                                  >>>1));

                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4
                                  );

                                   end 
                                  if ((CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_4>=32'sd1020))  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h1b/*27:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= 64'sh9_b74e_da84_35e0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= 64'sh0;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= 32'sh0;
                                   CordicWithIntegerAngle_sin_kernel_itheta <= CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_8;
                                   end 
                                   end 
                          
                  32'h1b/*27:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                      <32'sd56))  begin 
                               kiwiCORDMAIN4001PC10nz <= 32'h1c/*28:kiwiCORDMAIN4001PC10nz*/;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4 <= hprpin501941x10;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3 <= hprpin501940x10;
                               CordicWithIntegerAngle_sin_kernel_itheta <= hprpin501939x10;
                               end 
                               else  begin 
                               kiwiCORDMAIN4001PC10nz <= 32'h1d/*29:kiwiCORDMAIN4001PC10nz*/;
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10 <= ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                              >=32'sd56) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1<64'sh0)? 32'h1: 32'h0);

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7 <= rtl_unsigned_extend2((32'h0/*0:USA24*/==CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
                              ));

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5 <= ((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                              >=32'sd56) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1<64'sh0)? $signed((0-CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
                              )): CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1);

                               end 
                              
                  32'h1c/*28:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h1b/*27:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_3
                          ;

                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_4
                          ;

                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2
                          );

                           end 
                          
                  32'h1d/*29:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h1e/*30:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= 32'sh3ff;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_5
                          ;

                           end 
                          
                  32'h1e/*30:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((64'sh10_0000_0000_0000<CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                      ))  kiwiCORDMAIN4001PC10nz <= 32'h1f/*31:kiwiCORDMAIN4001PC10nz*/;
                           else  begin 
                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= $signed(-32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                              );

                               CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= $unsigned(64'sh2*CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                              );

                               end 
                              
                  32'h1f/*31:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                          )  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h21/*33:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= 64'h0;
                                   end 
                                  if ((64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && !CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                          )  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h20/*32:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258 <= (!CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_10
                                   && (64'sh20_0000_0000_0000>=CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12) && !CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_7
                                  ? 64'sh0: -64'sh8000_0000_0000_0000);

                                   end 
                                  if ((64'sh20_0000_0000_0000<CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12))  begin 
                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11 <= $signed(32'sd1+CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                                  );

                                   CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12 <= $unsigned((CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12
                                  >>1));

                                   end 
                                   end 
                          
                  32'h20/*32:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h21/*33:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13 <= $unsigned(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_SPILL_258
                          |64'shf_ffff_ffff_ffff&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_12|(rtl_sign_extend3(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_11
                          )<<32'sd52));

                           end 
                          
                  32'h21/*33:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h22/*34:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9 <= (CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0? 64'sh8000_0000_0000_0000
                          ^hprpin500736x10: hprpin500736x10);

                           end 
                          
                  32'h22/*34:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h23/*35:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256 <= CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_9;
                           end 
                          
                  32'h23/*35:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h24/*36:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_SPILL_258 <= ((A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_4
                          ]==hprpin500799x10)? 32'sh1: 32'sh0);

                           CORDMAIN400_CordicTestbench_Main_SPILL_257 <= CORDMAIN400_CordicTestbench_Main_V_3;
                           CORDMAIN400_CordicTestbench_Main_V_5 <= hprpin500799x10;
                           end 
                          
                  32'h24/*36:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("Test: input=%1H expected=%1H output=%1H ", hprpin500973x10, A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_4
                          ], CORDMAIN400_CordicTestbench_Main_V_5);
                          if ((32'h0/*0:USA18*/!=(CORDMAIN400_CordicTestbench_Main_V_5^A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_4
                          ]))) $display("   test %1d hamming error=%1H", CORDMAIN400_CordicTestbench_Main_V_4, CORDMAIN400_CordicTestbench_Main_V_5
                              ^A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_4]);
                               kiwiCORDMAIN4001PC10nz <= 32'h26/*38:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_V_3 <= $signed(CORDMAIN400_CordicTestbench_Main_SPILL_258+CORDMAIN400_CordicTestbench_Main_SPILL_257
                          );

                           end 
                          
                  32'hc/*12:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hd/*13:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h25/*37:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((CORDMAIN400_CordicTestbench_Main_V_4>=32'sd36))  begin 
                                  $display("Result: %1d/%1d", CORDMAIN400_CordicTestbench_Main_V_3, 32'sd36);
                                  $display("cordic: Testbench finished");
                                   end 
                                   else  kiwiCORDMAIN4001PC10nz <= 32'hc/*12:kiwiCORDMAIN4001PC10nz*/;
                          if ((CORDMAIN400_CordicTestbench_Main_V_4>=32'sd36))  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h32/*50:kiwiCORDMAIN4001PC10nz*/;
                                   done <= 32'h1;
                                   end 
                                   end 
                          
                  32'h26/*38:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h25/*37:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_V_4 <= $signed(32'sd1+CORDMAIN400_CordicTestbench_Main_V_4);
                           end 
                          
                  32'h29/*41:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2a/*42:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2a/*42:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2b/*43:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2b/*43:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2c/*44:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h27/*39:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h31/*49:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2c/*44:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h27/*39:kiwiCORDMAIN4001PC10nz*/;
                           CordicWithIntegerAngle_sin_theta <= ((32'h2c/*44:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_12_RR
                          : ifADDERALUF6412RRh10hold);

                           end 
                          
                  32'h2d/*45:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2e/*46:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2e/*46:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2f/*47:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2f/*47:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h30/*48:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h12/*18:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h13/*19:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h30/*48:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (!hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta, 64'h4009_21fb_5444_2d11))  begin 
                                   CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0 <= rtl_unsigned_extend2(!CORDMAIN400_CordicWithIntegerAngle_sin_3_6_V_0
                                  );

                                   CordicWithIntegerAngle_sin_theta <= ((32'h30/*48:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_10_RR
                                  : ifADDERALUF6410RRh10hold);

                                   end 
                                   kiwiCORDMAIN4001PC10nz <= 32'h12/*18:kiwiCORDMAIN4001PC10nz*/;
                           end 
                          
                  32'h28/*40:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if (hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta
                      , 64'h4019_21fb_5444_2d1c))  kiwiCORDMAIN4001PC10nz <= 32'h2d/*45:kiwiCORDMAIN4001PC10nz*/;
                           else  kiwiCORDMAIN4001PC10nz <= 32'h29/*41:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h31/*49:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h28/*40:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h32/*50:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h33/*51:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h33/*51:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $finish(32'sd0);
                           kiwiCORDMAIN4001PC10nz <= 32'h34/*52:kiwiCORDMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  begin 
               kiwiCORDMAIN4001PC10nz_pc_export <= 32'd0;
               ifADDERALUF6416RRh10hold <= 64'd0;
               ifADDERALUF6416RRh10shot1 <= 32'd0;
               ifADDERALUF6416RRh10shot2 <= 32'd0;
               ifADDERALUF6416RRh10shot3 <= 32'd0;
               ifADDERALUF6414RRh10hold <= 64'd0;
               ifADDERALUF6414RRh10shot1 <= 32'd0;
               ifADDERALUF6414RRh10shot2 <= 32'd0;
               ifADDERALUF6414RRh10shot3 <= 32'd0;
               ifADDERALUF6410RRh10hold <= 64'd0;
               ifADDERALUF6410RRh10shot1 <= 32'd0;
               ifADDERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10shot3 <= 32'd0;
               ifADDERALUF6412RRh10hold <= 64'd0;
               ifADDERALUF6412RRh10shot1 <= 32'd0;
               ifADDERALUF6412RRh10shot2 <= 32'd0;
               ifADDERALUF6412RRh10shot3 <= 32'd0;
               ifADDERALUF6412RRh10shot0 <= 32'd0;
               ifADDERALUF6410RRh10shot0 <= 32'd0;
               ifADDERALUF6414RRh10shot0 <= 32'd0;
               ifADDERALUF6416RRh10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (ifADDERALUF6412RRh10shot3)  ifADDERALUF6412RRh10hold <= ifADDERALUF64_12_RR;
                      if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifADDERALUF6414RRh10shot3)  ifADDERALUF6414RRh10hold <= ifADDERALUF64_14_RR;
                      if (ifADDERALUF6416RRh10shot3)  ifADDERALUF6416RRh10hold <= ifADDERALUF64_16_RR;
                       kiwiCORDMAIN4001PC10nz_pc_export <= kiwiCORDMAIN4001PC10nz;
                   ifADDERALUF6416RRh10shot1 <= ifADDERALUF6416RRh10shot0;
                   ifADDERALUF6416RRh10shot2 <= ifADDERALUF6416RRh10shot1;
                   ifADDERALUF6416RRh10shot3 <= ifADDERALUF6416RRh10shot2;
                   ifADDERALUF6414RRh10shot1 <= ifADDERALUF6414RRh10shot0;
                   ifADDERALUF6414RRh10shot2 <= ifADDERALUF6414RRh10shot1;
                   ifADDERALUF6414RRh10shot3 <= ifADDERALUF6414RRh10shot2;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifADDERALUF6412RRh10shot1 <= ifADDERALUF6412RRh10shot0;
                   ifADDERALUF6412RRh10shot2 <= ifADDERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot3 <= ifADDERALUF6412RRh10shot2;
                   ifADDERALUF6412RRh10shot0 <= !hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta, 64'h4019_21fb_5444_2d1c) && (32'h28/*40:kiwiCORDMAIN4001PC10nz*/==
                  kiwiCORDMAIN4001PC10nz);

                   ifADDERALUF6410RRh10shot0 <= hpr_fp_dltd0(CordicWithIntegerAngle_sin_theta, 64'h4019_21fb_5444_2d1c) && (32'h28/*40:kiwiCORDMAIN4001PC10nz*/==
                  kiwiCORDMAIN4001PC10nz);

                   ifADDERALUF6414RRh10shot0 <= (32'h12/*18:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   ifADDERALUF6416RRh10shot0 <= (32'hc/*12:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.CORDMAIN400_1/1.0


       end 
      

  CV_FP_FL4_ADDER_DP ifADDERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_10_RR),
        .XX(ifADDERALUF64_10_XX
),
        .YY(ifADDERALUF64_10_YY),
        .FAIL(ifADDERALUF64_10_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_12_RR),
        .XX(ifADDERALUF64_12_XX
),
        .YY(ifADDERALUF64_12_YY),
        .FAIL(ifADDERALUF64_12_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_14(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_14_RR),
        .XX(ifADDERALUF64_14_XX
),
        .YY(ifADDERALUF64_14_YY),
        .FAIL(ifADDERALUF64_14_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_16(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_16_RR),
        .XX(ifADDERALUF64_16_XX
),
        .YY(ifADDERALUF64_16_YY),
        .FAIL(ifADDERALUF64_16_FAIL));
always @(*) hprpin500704x10 = 64'h3fb9_a027_5254_60aa;

always @(*) hprpin500736x10 = CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_13;

always @(*) hprpin500771x10 = CordicWithIntegerAngle_sin_theta;

always @(*) hprpin500799x10 = CORDMAIN400_CordicWithIntegerAngle_sin_3_6_SPILL_256;

assign hprpin500973x10 = A_64_US_CC_SCALbx14_ARB0[CORDMAIN400_CordicTestbench_Main_V_4];

always @(*) hprpin501348x10 = hprpin500973x10;

assign hprpin501939x10 = ((CordicWithIntegerAngle_sin_kernel_itheta<64'sh0) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2<32'sd56)? $signed(CordicWithIntegerAngle_sin_kernel_itheta
+A_64_SS_CC_SCALbx10_ARA0[CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2]): $signed(CordicWithIntegerAngle_sin_kernel_itheta+
(0-A_64_SS_CC_SCALbx10_ARA0[CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2])));

assign hprpin501940x10 = ((CordicWithIntegerAngle_sin_kernel_itheta<64'sh0) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2<32'sd56)? $signed(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0
+(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1>>>(32'sd63&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2))): $signed(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0
+(0-(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1>>>(32'sd63&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2)))));

assign hprpin501941x10 = ((CordicWithIntegerAngle_sin_kernel_itheta<64'sh0) && (CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2<32'sd56)? $signed(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
+(0-(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0>>>(32'sd63&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2)))): $signed(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_1
+(CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_0>>>(32'sd63&CORDMAIN400_CordicWithIntegerAngle_sin_kernel_26_9_V_2))));

 initial        begin 
      //ROM data table: 56 words of 64 bits.
       A_64_SS_CC_SCALbx10_ARA0[0] = 64'h64_87ed_5110_b460;
       A_64_SS_CC_SCALbx10_ARA0[1] = 64'h3b_58ce_0ac3_769e;
       A_64_SS_CC_SCALbx10_ARA0[2] = 64'h1f_5b75_f92c_80dd;
       A_64_SS_CC_SCALbx10_ARA0[3] = 64'hf_eadd_4d56_17b7;
       A_64_SS_CC_SCALbx10_ARA0[4] = 64'h7_fd56_edcb_3f7a;
       A_64_SS_CC_SCALbx10_ARA0[5] = 64'h3_ffaa_b775_2ec4;
       A_64_SS_CC_SCALbx10_ARA0[6] = 64'h1_fff5_55bb_b729;
       A_64_SS_CC_SCALbx10_ARA0[7] = 64'hfffe_aaad_ddd4;
       A_64_SS_CC_SCALbx10_ARA0[8] = 64'h7fff_d555_6eee;
       A_64_SS_CC_SCALbx10_ARA0[9] = 64'h3fff_faaa_ab77;
       A_64_SS_CC_SCALbx10_ARA0[10] = 64'h1fff_ff55_555b;
       A_64_SS_CC_SCALbx10_ARA0[11] = 64'hfff_ffea_aaaa;
       A_64_SS_CC_SCALbx10_ARA0[12] = 64'h7ff_fffd_5555;
       A_64_SS_CC_SCALbx10_ARA0[13] = 64'h3ff_ffff_aaaa;
       A_64_SS_CC_SCALbx10_ARA0[14] = 64'h1ff_ffff_f555;
       A_64_SS_CC_SCALbx10_ARA0[15] = 64'hff_ffff_feaa;
       A_64_SS_CC_SCALbx10_ARA0[16] = 64'h7f_ffff_ffd5;
       A_64_SS_CC_SCALbx10_ARA0[17] = 64'h3f_ffff_fffa;
       A_64_SS_CC_SCALbx10_ARA0[18] = 64'h1f_ffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[19] = 64'hf_ffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[20] = 64'h7_ffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[21] = 64'h3_ffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[22] = 64'h1_ffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[23] = 64'hffff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[24] = 64'h7fff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[25] = 64'h3fff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[26] = 64'h1fff_ffff;
       A_64_SS_CC_SCALbx10_ARA0[27] = 64'h1000_0000;
       A_64_SS_CC_SCALbx10_ARA0[28] = 64'h800_0000;
       A_64_SS_CC_SCALbx10_ARA0[29] = 64'h400_0000;
       A_64_SS_CC_SCALbx10_ARA0[30] = 64'h200_0000;
       A_64_SS_CC_SCALbx10_ARA0[31] = 64'h100_0000;
       A_64_SS_CC_SCALbx10_ARA0[32] = 64'h80_0000;
       A_64_SS_CC_SCALbx10_ARA0[33] = 64'h40_0000;
       A_64_SS_CC_SCALbx10_ARA0[34] = 64'h20_0000;
       A_64_SS_CC_SCALbx10_ARA0[35] = 64'h10_0000;
       A_64_SS_CC_SCALbx10_ARA0[36] = 64'h8_0000;
       A_64_SS_CC_SCALbx10_ARA0[37] = 64'h4_0000;
       A_64_SS_CC_SCALbx10_ARA0[38] = 64'h2_0000;
       A_64_SS_CC_SCALbx10_ARA0[39] = 64'h1_0000;
       A_64_SS_CC_SCALbx10_ARA0[40] = 64'h8000;
       A_64_SS_CC_SCALbx10_ARA0[41] = 64'h4000;
       A_64_SS_CC_SCALbx10_ARA0[42] = 64'h2000;
       A_64_SS_CC_SCALbx10_ARA0[43] = 64'h1000;
       A_64_SS_CC_SCALbx10_ARA0[44] = 64'h800;
       A_64_SS_CC_SCALbx10_ARA0[45] = 64'h400;
       A_64_SS_CC_SCALbx10_ARA0[46] = 64'h200;
       A_64_SS_CC_SCALbx10_ARA0[47] = 64'h100;
       A_64_SS_CC_SCALbx10_ARA0[48] = 64'h80;
       A_64_SS_CC_SCALbx10_ARA0[49] = 64'h40;
       A_64_SS_CC_SCALbx10_ARA0[50] = 64'h20;
       A_64_SS_CC_SCALbx10_ARA0[51] = 64'h10;
       A_64_SS_CC_SCALbx10_ARA0[52] = 64'h8;
       A_64_SS_CC_SCALbx10_ARA0[53] = 64'h4;
       A_64_SS_CC_SCALbx10_ARA0[54] = 64'h2;
       A_64_SS_CC_SCALbx10_ARA0[55] = 64'h1;
       end 
      

 initial        begin 
      //ROM data table: 36 words of 64 bits.
       A_64_US_CC_SCALbx12_ARA0[0] = 64'h0;
       A_64_US_CC_SCALbx12_ARA0[1] = 64'h3fc6_3a1a_335a_adcd;
       A_64_US_CC_SCALbx12_ARA0[2] = 64'h3fd5_e3a8_2b09_bf3e;
       A_64_US_CC_SCALbx12_ARA0[3] = 64'h3fdf_ffff_91f9_aa91;
       A_64_US_CC_SCALbx12_ARA0[4] = 64'h3fe4_91b7_16c2_42e3;
       A_64_US_CC_SCALbx12_ARA0[5] = 64'h3fe8_836f_6726_14a6;
       A_64_US_CC_SCALbx12_ARA0[6] = 64'h3feb_b67a_c40b_2bed;
       A_64_US_CC_SCALbx12_ARA0[7] = 64'h3fee_11f6_127e_28ad;
       A_64_US_CC_SCALbx12_ARA0[8] = 64'h3fef_838b_6adf_fac0;
       A_64_US_CC_SCALbx12_ARA0[9] = 64'h3fef_ffff_e1cb_d7aa;
       A_64_US_CC_SCALbx12_ARA0[10] = 64'h3fef_838b_b014_7989;
       A_64_US_CC_SCALbx12_ARA0[11] = 64'h3fee_11f6_92d9_62b4;
       A_64_US_CC_SCALbx12_ARA0[12] = 64'h3feb_b67b_77c0_142d;
       A_64_US_CC_SCALbx12_ARA0[13] = 64'h3fe8_8370_9d4e_a869;
       A_64_US_CC_SCALbx12_ARA0[14] = 64'h3fe4_91b8_1d72_d8e8;
       A_64_US_CC_SCALbx12_ARA0[15] = 64'h3fe0_0000_ea5f_43c8;
       A_64_US_CC_SCALbx12_ARA0[16] = 64'h3fd5_e3aa_4e05_90c5;
       A_64_US_CC_SCALbx12_ARA0[17] = 64'h3fc6_3a1d_2189_552c;
       A_64_US_CC_SCALbx12_ARA0[18] = 64'h3ea6_aedf_fc45_4b91;
       A_64_US_CC_SCALbx12_ARA0[19] = -64'h4039_c5eb_bb22_4c84;
       A_64_US_CC_SCALbx12_ARA0[20] = -64'h402a_1c5b_1970_70c2;
       A_64_US_CC_SCALbx12_ARA0[21] = -64'h4020_0002_b6b3_0695;
       A_64_US_CC_SCALbx12_ARA0[22] = -64'h401b_6e49_e346_5c2d;
       A_64_US_CC_SCALbx12_ARA0[23] = -64'h4017_7c91_4d23_07eb;
       A_64_US_CC_SCALbx12_ARA0[24] = -64'h4014_4985_8bf5_51ce;
       A_64_US_CC_SCALbx12_ARA0[25] = -64'h4011_ee0a_6ed2_dea9;
       A_64_US_CC_SCALbx12_ARA0[26] = -64'h4010_7c74_e539_b504;
       A_64_US_CC_SCALbx12_ARA0[27] = -64'h4010_0000_3d1a_2371;
       A_64_US_CC_SCALbx12_ARA0[28] = -64'h4010_7c74_a15d_1816;
       A_64_US_CC_SCALbx12_ARA0[29] = -64'h4011_ee08_eed2_51d9;
       A_64_US_CC_SCALbx12_ARA0[30] = -64'h4014_4983_d3ce_34b6;
       A_64_US_CC_SCALbx12_ARA0[31] = -64'h4017_7c8e_9190_287f;
       A_64_US_CC_SCALbx12_ARA0[32] = -64'h401b_6e46_32e4_a2aa;
       A_64_US_CC_SCALbx12_ARA0[33] = -64'h401f_fffd_e2f3_5cf3;
       A_64_US_CC_SCALbx12_ARA0[34] = -64'h402a_1c52_f596_3509;
       A_64_US_CC_SCALbx12_ARA0[35] = -64'h4039_c5dc_3b77_9c23;
       end 
      

 initial        begin 
      //ROM data table: 36 words of 64 bits.
       A_64_US_CC_SCALbx14_ARB0[0] = 64'h0;
       A_64_US_CC_SCALbx14_ARB0[1] = 64'h3fc6_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[2] = 64'h3fd6_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[3] = 64'h3fe0_c151_fdb2_0051;
       A_64_US_CC_SCALbx14_ARB0[4] = 64'h3fe6_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[5] = 64'h3feb_ecdd_fc28_ab31;
       A_64_US_CC_SCALbx14_ARB0[6] = 64'h3ff0_c151_fdb2_0051;
       A_64_US_CC_SCALbx14_ARB0[7] = 64'h3ff3_8c34_fd4f_ab09;
       A_64_US_CC_SCALbx14_ARB0[8] = 64'h3ff6_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[9] = 64'h3ff9_21fa_fc8b_0079;
       A_64_US_CC_SCALbx14_ARB0[10] = 64'h3ffb_ecdd_fc28_ab31;
       A_64_US_CC_SCALbx14_ARB0[11] = 64'h3ffe_b7c0_fbc6_55e9;
       A_64_US_CC_SCALbx14_ARB0[12] = 64'h4000_c151_fdb2_0051;
       A_64_US_CC_SCALbx14_ARB0[13] = 64'h4002_26c3_7d80_d5ad;
       A_64_US_CC_SCALbx14_ARB0[14] = 64'h4003_8c34_fd4f_ab09;
       A_64_US_CC_SCALbx14_ARB0[15] = 64'h4004_f1a6_7d1e_8065;
       A_64_US_CC_SCALbx14_ARB0[16] = 64'h4006_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[17] = 64'h4007_bc89_7cbc_2b1d;
       A_64_US_CC_SCALbx14_ARB0[18] = 64'h4009_21fa_fc8b_0079;
       A_64_US_CC_SCALbx14_ARB0[19] = 64'h400a_876c_7c59_d5d5;
       A_64_US_CC_SCALbx14_ARB0[20] = 64'h400b_ecdd_fc28_ab31;
       A_64_US_CC_SCALbx14_ARB0[21] = 64'h400d_524f_7bf7_808d;
       A_64_US_CC_SCALbx14_ARB0[22] = 64'h400e_b7c0_fbc6_55e9;
       A_64_US_CC_SCALbx14_ARB0[23] = 64'h4010_0e99_3dca_95a3;
       A_64_US_CC_SCALbx14_ARB0[24] = 64'h4010_c151_fdb2_0051;
       A_64_US_CC_SCALbx14_ARB0[25] = 64'h4011_740a_bd99_6aff;
       A_64_US_CC_SCALbx14_ARB0[26] = 64'h4012_26c3_7d80_d5ad;
       A_64_US_CC_SCALbx14_ARB0[27] = 64'h4012_d97c_3d68_405b;
       A_64_US_CC_SCALbx14_ARB0[28] = 64'h4013_8c34_fd4f_ab09;
       A_64_US_CC_SCALbx14_ARB0[29] = 64'h4014_3eed_bd37_15b7;
       A_64_US_CC_SCALbx14_ARB0[30] = 64'h4014_f1a6_7d1e_8065;
       A_64_US_CC_SCALbx14_ARB0[31] = 64'h4015_a45f_3d05_eb13;
       A_64_US_CC_SCALbx14_ARB0[32] = 64'h4016_5717_fced_55c1;
       A_64_US_CC_SCALbx14_ARB0[33] = 64'h4017_09d0_bcd4_c06f;
       A_64_US_CC_SCALbx14_ARB0[34] = 64'h4017_bc89_7cbc_2b1d;
       A_64_US_CC_SCALbx14_ARB0[35] = 64'h4018_6f42_3ca3_95cb;
       end 
      

// Structural Resource (FU) inventory for DUT:// 35 vectors of width 64
// 1 vectors of width 6
// 20 vectors of width 1
// 8 vectors of width 32
// 128 array locations of width 64
// Total state bits in module = 10714 bits.
// 516 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL4_ADDER_DP count=4
// Total number of leaf cells = 4
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 07:11:00
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test56b.exe -sim=18000 -kiwife-firstpass-loglevel=5 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-----------------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------------+--------+-------*
//| Class           | Style   | Dir Style                                                                                            | Timing Target | Method               | UID    | Skip  |
//*-----------------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------------+--------+-------*
//| CordicTestbench | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | CordicTestbench.Main | Main10 | false |
//*-----------------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------------+--------+-------*

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
//KiwiC: front end input processing of class CordicWithIntegerAngle  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=CordicWithIntegerAngle..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=CordicWithIntegerAngle..cctor
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
//KiwiC: front end input processing of class CordicTestbench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=CordicTestbench..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=CordicTestbench..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class CordicTestbench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=CordicTestbench.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=CordicTestbench.Main
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
//   kiwife-firstpass-loglevel=5
//
//
//   kiwife-overloads-loglevel=3
//
//
//   root=$attributeroot
//
//
//   srcfile=test56b.exe
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
//PC codings points for kiwiCORDMAIN4001PC10 
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                     | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiCORDMAIN4001PC10"   | 902 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'0:"0:kiwiCORDMAIN4001PC10"   | 903 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 11   |
//| XU32'1:"1:kiwiCORDMAIN4001PC10"   | 901 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiCORDMAIN4001PC10"   | 899 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiCORDMAIN4001PC10"   | 900 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'3:"3:kiwiCORDMAIN4001PC10"   | 897 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 3    |
//| XU32'3:"3:kiwiCORDMAIN4001PC10"   | 898 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiCORDMAIN4001PC10"   | 895 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'4:"4:kiwiCORDMAIN4001PC10"   | 896 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 6    |
//| XU32'5:"5:kiwiCORDMAIN4001PC10"   | 894 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 4    |
//| XU32'6:"6:kiwiCORDMAIN4001PC10"   | 893 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 7    |
//| XU32'7:"7:kiwiCORDMAIN4001PC10"   | 891 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 7    |
//| XU32'7:"7:kiwiCORDMAIN4001PC10"   | 892 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 8    |
//| XU32'8:"8:kiwiCORDMAIN4001PC10"   | 888 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 8    |
//| XU32'8:"8:kiwiCORDMAIN4001PC10"   | 889 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 9    |
//| XU32'8:"8:kiwiCORDMAIN4001PC10"   | 890 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 10   |
//| XU32'9:"9:kiwiCORDMAIN4001PC10"   | 887 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'10:"10:kiwiCORDMAIN4001PC10" | 886 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'11:"11:kiwiCORDMAIN4001PC10" | 885 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 37   |
//| XU32'13:"13:kiwiCORDMAIN4001PC10" | 882 | 12      | hwm=0.4.0   | 16   |        | 13    | 16  | 17   |
//| XU32'14:"14:kiwiCORDMAIN4001PC10" | 881 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 40   |
//| XU32'16:"16:kiwiCORDMAIN4001PC10" | 878 | 18      | hwm=0.4.0   | 22   |        | 19    | 22  | 23   |
//| XU32'17:"17:kiwiCORDMAIN4001PC10" | 876 | 23      | hwm=0.0.0   | 23   |        | -     | -   | 24   |
//| XU32'17:"17:kiwiCORDMAIN4001PC10" | 877 | 23      | hwm=0.0.0   | 23   |        | -     | -   | 35   |
//| XU32'18:"18:kiwiCORDMAIN4001PC10" | 875 | 24      | hwm=0.0.0   | 24   |        | -     | -   | 25   |
//| XU32'19:"19:kiwiCORDMAIN4001PC10" | 873 | 25      | hwm=0.0.0   | 25   |        | -     | -   | 25   |
//| XU32'19:"19:kiwiCORDMAIN4001PC10" | 874 | 25      | hwm=0.0.0   | 25   |        | -     | -   | 26   |
//| XU32'20:"20:kiwiCORDMAIN4001PC10" | 871 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 26   |
//| XU32'20:"20:kiwiCORDMAIN4001PC10" | 872 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 27   |
//| XU32'21:"21:kiwiCORDMAIN4001PC10" | 869 | 27      | hwm=0.0.0   | 27   |        | -     | -   | 28   |
//| XU32'21:"21:kiwiCORDMAIN4001PC10" | 870 | 27      | hwm=0.0.0   | 27   |        | -     | -   | 29   |
//| XU32'22:"22:kiwiCORDMAIN4001PC10" | 868 | 28      | hwm=0.0.0   | 28   |        | -     | -   | 27   |
//| XU32'23:"23:kiwiCORDMAIN4001PC10" | 867 | 29      | hwm=0.0.0   | 29   |        | -     | -   | 30   |
//| XU32'24:"24:kiwiCORDMAIN4001PC10" | 865 | 30      | hwm=0.0.0   | 30   |        | -     | -   | 30   |
//| XU32'24:"24:kiwiCORDMAIN4001PC10" | 866 | 30      | hwm=0.0.0   | 30   |        | -     | -   | 31   |
//| XU32'25:"25:kiwiCORDMAIN4001PC10" | 862 | 31      | hwm=0.0.0   | 31   |        | -     | -   | 31   |
//| XU32'25:"25:kiwiCORDMAIN4001PC10" | 863 | 31      | hwm=0.0.0   | 31   |        | -     | -   | 32   |
//| XU32'25:"25:kiwiCORDMAIN4001PC10" | 864 | 31      | hwm=0.0.0   | 31   |        | -     | -   | 33   |
//| XU32'26:"26:kiwiCORDMAIN4001PC10" | 861 | 32      | hwm=0.0.0   | 32   |        | -     | -   | 33   |
//| XU32'27:"27:kiwiCORDMAIN4001PC10" | 860 | 33      | hwm=0.0.0   | 33   |        | -     | -   | 34   |
//| XU32'28:"28:kiwiCORDMAIN4001PC10" | 859 | 34      | hwm=0.0.0   | 34   |        | -     | -   | 35   |
//| XU32'29:"29:kiwiCORDMAIN4001PC10" | 858 | 35      | hwm=0.0.0   | 35   |        | -     | -   | 36   |
//| XU32'30:"30:kiwiCORDMAIN4001PC10" | 857 | 36      | hwm=0.0.0   | 36   |        | -     | -   | 38   |
//| XU32'12:"12:kiwiCORDMAIN4001PC10" | 883 | 37      | hwm=0.0.0   | 37   |        | -     | -   | 12   |
//| XU32'12:"12:kiwiCORDMAIN4001PC10" | 884 | 37      | hwm=0.0.0   | 37   |        | -     | -   | 50   |
//| XU32'31:"31:kiwiCORDMAIN4001PC10" | 856 | 38      | hwm=0.0.0   | 38   |        | -     | -   | 37   |
//| XU32'32:"32:kiwiCORDMAIN4001PC10" | 855 | 39      | hwm=0.0.0   | 39   |        | -     | -   | 49   |
//| XU32'15:"15:kiwiCORDMAIN4001PC10" | 879 | 40      | hwm=0.4.0   | 48   |        | 45    | 48  | 18   |
//| XU32'15:"15:kiwiCORDMAIN4001PC10" | 880 | 40      | hwm=0.4.0   | 44   |        | 41    | 44  | 39   |
//| XU32'33:"33:kiwiCORDMAIN4001PC10" | 854 | 49      | hwm=0.0.0   | 49   |        | -     | -   | 40   |
//| XU32'34:"34:kiwiCORDMAIN4001PC10" | 853 | 50      | hwm=0.0.0   | 50   |        | -     | -   | 51   |
//| XU32'35:"35:kiwiCORDMAIN4001PC10" | 852 | 51      | hwm=0.0.0   | 51   |        | -     | -   | -    |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'0:"0:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'0:"0:kiwiCORDMAIN4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                |
//| F0   | E903 | R0 DATA |                                                                                                                |
//| F0+E | E903 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12._SPILL.256write(??64'4591877385826361514) CordicWithIntegerAngle.\ |
//|      |      |         | sin.thetawrite(??64'4591877385826361514) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4write(S32'0) CORDMAIN\ |
//|      |      |         | 400.CordicWithIntegerAngle.sin.2.12.V_6write(U64'0) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8write(S64'\ |
//|      |      |         | 0) CordicWithIntegerAngle.sin_kernel.ithetawrite(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_\ |
//|      |      |         | 2write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1write(S64'0) CORDMAIN400.CordicWithIntege\ |
//|      |      |         | rAngle.sin_kernel.26.9.V_0write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_3write(S64'0) COR\ |
//|      |      |         | DMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_4write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel\ |
//|      |      |         | .26.9.V_5write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(U64'0) CORDMAIN400.CordicW\ |
//|      |      |         | ithIntegerAngle.sin_kernel.26.9.V_11write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7write(\ |
//|      |      |         | U32'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_10write(U32'0) CORDMAIN400.CordicWithIntegerAngle\ |
//|      |      |         | .sin_kernel.26.9._SPILL.258write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(U64'0) C\ |
//|      |      |         | ORDMAIN400.CordicTestbench.Main.V_4write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0write(U32'0) CO\ |
//|      |      |         | RDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_6write(\ |
//|      |      |         | U64'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.\ |
//|      |      |         | V_9write(??64'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6._SPILL.256write(??64'0) CORDMAIN400.CordicTestben\ |
//|      |      |         | ch.Main.V_3write(S32'0) CORDMAIN400.CordicTestbench.Main.V_5write(U64'0) CORDMAIN400.CordicTestbench.Main._SP\ |
//|      |      |         | ILL.257write(S32'0) CORDMAIN400.CordicTestbench.Main._SPILL.258write(S32'0) donewrite(U32'0)  PLI:  theta now\ |
//|      |      |         |  %f  PLI:cordic: Testbench st...                                                                               |
//| F0   | E902 | R0 DATA |                                                                                                                |
//| F0+E | E902 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12._SPILL.256write(??64'0) CordicWithIntegerAngle.sin.thetawrite(??6\ |
//|      |      |         | 4'4591877385826361514) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4write(E1) CORDMAIN400.CordicWithInteger\ |
//|      |      |         | Angle.sin.2.12.V_6write(E2) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8write(S64'0) CordicWithIntegerAngl\ |
//|      |      |         | e.sin_kernel.ithetawrite(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2write(S32'0) CORDMAIN40\ |
//|      |      |         | 0.CordicWithIntegerAngle.sin_kernel.26.9.V_1write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V\ |
//|      |      |         | _0write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_3write(S64'0) CORDMAIN400.CordicWithInteg\ |
//|      |      |         | erAngle.sin_kernel.26.9.V_4write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_5write(S64'0) CO\ |
//|      |      |         | RDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(U64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kern\ |
//|      |      |         | el.26.9.V_11write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7write(U32'0) CORDMAIN400.Cordi\ |
//|      |      |         | cWithIntegerAngle.sin_kernel.26.9.V_10write(U32'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9._SPILL.\ |
//|      |      |         | 258write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(U64'0) CORDMAIN400.CordicTestben\ |
//|      |      |         | ch.Main.V_4write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0write(U32'0) CORDMAIN400.CordicWithInte\ |
//|      |      |         | gerAngle.sin.3.6.V_4write(S32'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_6write(U64'0) CORDMAIN400.Cordi\ |
//|      |      |         | cWithIntegerAngle.sin.3.6.V_8write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_9write(??64'0) CORDMAI\ |
//|      |      |         | N400.CordicWithIntegerAngle.sin.3.6._SPILL.256write(??64'0) CORDMAIN400.CordicTestbench.Main.V_3write(S32'0) \ |
//|      |      |         | CORDMAIN400.CordicTestbench.Main.V_5write(U64'0) CORDMAIN400.CordicTestbench.Main._SPILL.257write(S32'0) CORD\ |
//|      |      |         | MAIN400.CordicTestbench.Main._SPILL.258write(S32'0) donewrite(U32'0)  PLI:  theta now %f  PLI:cordic: Testben\ |
//|      |      |         | ch st...                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1:"1:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1:"1:kiwiCORDMAIN4001PC10"
//*------+------+---------+----------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                     |
//*------+------+---------+----------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                          |
//| F1   | E901 | R0 DATA |                                                          |
//| F1+E | E901 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8write(E3) |
//*------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2:"2:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2:"2:kiwiCORDMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                              |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                                                   |
//| F2   | E900 | R0 DATA |                                                                                                                   |
//| F2+E | E900 | W0 DATA |                                                                                                                   |
//| F2   | E899 | R0 DATA |                                                                                                                   |
//| F2+E | E899 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4write(E4) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8write(E5) |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'3:"3:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'3:"3:kiwiCORDMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                              |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                                   |
//| F3   | E898 | R0 DATA |                                                                                                                                   |
//| F3+E | E898 | W0 DATA | CordicWithIntegerAngle.sin_kernel.ithetawrite(E6) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2write(S32'0) CORDMAIN400\ |
//|      |      |         | .CordicWithIntegerAngle.sin_kernel.26.9.V_1write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0write(S64'27348240\ |
//|      |      |         | 91825632)  PLI: theta_i %d   exp %d                                                                                               |
//| F3   | E897 | R0 DATA |                                                                                                                                   |
//| F3+E | E897 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4write(E7) CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8write(E8)                 |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4:"4:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4:"4:kiwiCORDMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                              |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                                   |
//| F4   | E896 | R0 DATA |                                                                                                                                   |
//| F4+E | E896 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_5write(E9) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7write(E10)\ |
//|      |      |         |  CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_10write(E11)                                                                |
//| F4   | E895 | R0 DATA |                                                                                                                                   |
//| F4+E | E895 | W0 DATA | CordicWithIntegerAngle.sin_kernel.ithetawrite(E12) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_3write(E13) CORDMAIN400.\ |
//|      |      |         | CordicWithIntegerAngle.sin_kernel.26.9.V_4write(E14)                                                                              |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'5:"5:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'5:"5:kiwiCORDMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                              |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                                                                                   |
//| F5   | E894 | R0 DATA |                                                                                                                                   |
//| F5+E | E894 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2write(E15) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1write(E16\ |
//|      |      |         | ) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0write(E17)                                                                |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'6:"6:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'6:"6:kiwiCORDMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                     |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| F6   | -    | R0 CTRL |                                                                                                                                          |
//| F6   | E893 | R0 DATA |                                                                                                                                          |
//| F6+E | E893 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E18) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(S32'1023) |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'7:"7:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'7:"7:kiwiCORDMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                                                                                                     |
//| F7   | E892 | R0 DATA |                                                                                                                                     |
//| F7+E | E892 | W0 DATA |                                                                                                                                     |
//| F7   | E891 | R0 DATA |                                                                                                                                     |
//| F7+E | E891 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E19) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(E20) |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8:"8:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8:"8:kiwiCORDMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                                                                                     |
//| F8   | E890 | R0 DATA |                                                                                                                                     |
//| F8+E | E890 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(U64'0)                                                                 |
//| F8   | E889 | R0 DATA |                                                                                                                                     |
//| F8+E | E889 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9._SPILL.258write(E21)                                                             |
//| F8   | E888 | R0 DATA |                                                                                                                                     |
//| F8+E | E888 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E22) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(E23) |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'9:"9:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'9:"9:kiwiCORDMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                              |
//*------+------+---------+-------------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                                   |
//| F9   | E887 | R0 DATA |                                                                   |
//| F9+E | E887 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(E24) |
//*------+------+---------+-------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'10:"10:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'10:"10:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F10   | -    | R0 CTRL |                                                                  |
//| F10   | E886 | R0 DATA |                                                                  |
//| F10+E | E886 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.2.12._SPILL.256write(E25) |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'11:"11:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'11:"11:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                        |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                                                                                                                             |
//| F11   | E885 | R0 DATA |                                                                                                                                                             |
//| F11+E | E885 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_4write(S32'1) CORDMAIN400.CordicTestbench.Main.V_3write(S32'0)  PLI:Please return NaN ou...  PLI:t0 %f  Sin=%f  cordi... |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'13:"13:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'13:"13:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| F12   | -    | R0 CTRL |                                                                                                     |
//| F12   | E882 | R0 DATA | ifADDERALUF64_16_compute(-3.141592654, E26)                                                         |
//| F13   | E882 | R1 DATA |                                                                                                     |
//| F14   | E882 | R2 DATA |                                                                                                     |
//| F15   | E882 | R3 DATA |                                                                                                     |
//| F16   | E882 | R4 DATA |                                                                                                     |
//| F16+E | E882 | W0 DATA | CordicWithIntegerAngle.sin.thetawrite(E27) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0write(E28) |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'14:"14:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'14:"14:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F17   | -    | R0 CTRL |      |
//| F17   | E881 | R0 DATA |      |
//| F17+E | E881 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16:"16:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16:"16:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                            |
//*-------+------+---------+-------------------------------------------------*
//| F18   | -    | R0 CTRL |                                                 |
//| F18   | E878 | R0 DATA | ifADDERALUF64_14_compute(3.14159265358979, E29) |
//| F19   | E878 | R1 DATA |                                                 |
//| F20   | E878 | R2 DATA |                                                 |
//| F21   | E878 | R3 DATA |                                                 |
//| F22   | E878 | R4 DATA |                                                 |
//| F22+E | E878 | W0 DATA | CordicWithIntegerAngle.sin.thetawrite(E30)      |
//*-------+------+---------+-------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'17:"17:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'17:"17:kiwiCORDMAIN4001PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                  |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| F23   | -    | R0 CTRL |                                                                                                                                       |
//| F23   | E877 | R0 DATA |                                                                                                                                       |
//| F23+E | E877 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6._SPILL.256write(E31)  PLI:  theta now %f                                                   |
//| F23   | E876 | R0 DATA |                                                                                                                                       |
//| F23+E | E876 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4write(E32) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_6write(E33)  PLI:  theta now %f |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'18:"18:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'18:"18:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                     |
//*-------+------+---------+----------------------------------------------------------*
//| F24   | -    | R0 CTRL |                                                          |
//| F24   | E875 | R0 DATA |                                                          |
//| F24+E | E875 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8write(E34) |
//*-------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'19:"19:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'19:"19:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                              |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| F25   | -    | R0 CTRL |                                                                                                                   |
//| F25   | E874 | R0 DATA |                                                                                                                   |
//| F25+E | E874 | W0 DATA |                                                                                                                   |
//| F25   | E873 | R0 DATA |                                                                                                                   |
//| F25+E | E873 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4write(E35) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8write(E36) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'20:"20:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'20:"20:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                              |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F26   | -    | R0 CTRL |                                                                                                                                   |
//| F26   | E872 | R0 DATA |                                                                                                                                   |
//| F26+E | E872 | W0 DATA | CordicWithIntegerAngle.sin_kernel.ithetawrite(E37) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2write(S32'0) CORDMAIN40\ |
//|       |      |         | 0.CordicWithIntegerAngle.sin_kernel.26.9.V_1write(S64'0) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0write(S64'2734824\ |
//|       |      |         | 091825632)  PLI: theta_i %d   exp %d                                                                                              |
//| F26   | E871 | R0 DATA |                                                                                                                                   |
//| F26+E | E871 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4write(E38) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8write(E39)                 |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'21:"21:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'21:"21:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                              |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F27   | -    | R0 CTRL |                                                                                                                                   |
//| F27   | E870 | R0 DATA |                                                                                                                                   |
//| F27+E | E870 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_5write(E9) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7write(E10)\ |
//|       |      |         |  CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_10write(E11)                                                                |
//| F27   | E869 | R0 DATA |                                                                                                                                   |
//| F27+E | E869 | W0 DATA | CordicWithIntegerAngle.sin_kernel.ithetawrite(E12) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_3write(E13) CORDMAIN400.\ |
//|       |      |         | CordicWithIntegerAngle.sin_kernel.26.9.V_4write(E14)                                                                              |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'22:"22:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'22:"22:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                              |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F28   | -    | R0 CTRL |                                                                                                                                   |
//| F28   | E868 | R0 DATA |                                                                                                                                   |
//| F28+E | E868 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2write(E15) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1write(E16\ |
//|       |      |         | ) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0write(E17)                                                                |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'23:"23:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'23:"23:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                     |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| F29   | -    | R0 CTRL |                                                                                                                                          |
//| F29   | E867 | R0 DATA |                                                                                                                                          |
//| F29+E | E867 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E18) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(S32'1023) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'24:"24:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'24:"24:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F30   | -    | R0 CTRL |                                                                                                                                     |
//| F30   | E866 | R0 DATA |                                                                                                                                     |
//| F30+E | E866 | W0 DATA |                                                                                                                                     |
//| F30   | E865 | R0 DATA |                                                                                                                                     |
//| F30+E | E865 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E19) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(E20) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'25:"25:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'25:"25:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F31   | -    | R0 CTRL |                                                                                                                                     |
//| F31   | E864 | R0 DATA |                                                                                                                                     |
//| F31+E | E864 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(U64'0)                                                                 |
//| F31   | E863 | R0 DATA |                                                                                                                                     |
//| F31+E | E863 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9._SPILL.258write(E21)                                                             |
//| F31   | E862 | R0 DATA |                                                                                                                                     |
//| F31+E | E862 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12write(E22) CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11write(E23) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'26:"26:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'26:"26:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                              |
//*-------+------+---------+-------------------------------------------------------------------*
//| F32   | -    | R0 CTRL |                                                                   |
//| F32   | E861 | R0 DATA |                                                                   |
//| F32+E | E861 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13write(E24) |
//*-------+------+---------+-------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'27:"27:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'27:"27:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                     |
//*-------+------+---------+----------------------------------------------------------*
//| F33   | -    | R0 CTRL |                                                          |
//| F33   | E860 | R0 DATA |                                                          |
//| F33+E | E860 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_9write(E40) |
//*-------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'28:"28:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'28:"28:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                            |
//*-------+------+---------+-----------------------------------------------------------------*
//| F34   | -    | R0 CTRL |                                                                 |
//| F34   | E859 | R0 DATA |                                                                 |
//| F34+E | E859 | W0 DATA | CORDMAIN400.CordicWithIntegerAngle.sin.3.6._SPILL.256write(E41) |
//*-------+------+---------+-----------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'29:"29:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'29:"29:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                       |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F35   | -    | R0 CTRL |                                                                                                                                                            |
//| F35   | E858 | R0 DATA |                                                                                                                                                            |
//| F35+E | E858 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_5write(E42) CORDMAIN400.CordicTestbench.Main._SPILL.257write(E43) CORDMAIN400.CordicTestbench.Main._SPILL.258write(E44) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'30:"30:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'30:"30:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                     |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| F36   | -    | R0 CTRL |                                                                                                          |
//| F36   | E857 | R0 DATA |                                                                                                          |
//| F36+E | E857 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_3write(E45)  PLI:   test %d hamming e...  PLI:Test: input=%X expec... |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'12:"12:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'12:"12:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F37   | -    | R0 CTRL |                                                                  |
//| F37   | E884 | R0 DATA |                                                                  |
//| F37+E | E884 | W0 DATA | donewrite(U32'1)  PLI:cordic: Testbench fi...  PLI:Result: %d/%d |
//| F37   | E883 | R0 DATA |                                                                  |
//| F37+E | E883 | W0 DATA |                                                                  |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'31:"31:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'31:"31:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                           |
//*-------+------+---------+------------------------------------------------*
//| F38   | -    | R0 CTRL |                                                |
//| F38   | E856 | R0 DATA |                                                |
//| F38+E | E856 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_4write(E46) |
//*-------+------+---------+------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'32:"32:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'32:"32:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F39   | -    | R0 CTRL |      |
//| F39   | E855 | R0 DATA |      |
//| F39+E | E855 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'15:"15:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'15:"15:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| F40   | -    | R0 CTRL |                                                                                                     |
//| F40   | E880 | R0 DATA | ifADDERALUF64_12_compute(E47, -6.283185307)                                                         |
//| F41   | E880 | R1 DATA |                                                                                                     |
//| F42   | E880 | R2 DATA |                                                                                                     |
//| F43   | E880 | R3 DATA |                                                                                                     |
//| F44   | E880 | R4 DATA |                                                                                                     |
//| F44+E | E880 | W0 DATA | CordicWithIntegerAngle.sin.thetawrite(E48)                                                          |
//| F40   | E879 | R0 DATA | ifADDERALUF64_10_compute(E47, -3.141592654)                                                         |
//| F45   | E879 | R1 DATA |                                                                                                     |
//| F46   | E879 | R2 DATA |                                                                                                     |
//| F47   | E879 | R3 DATA |                                                                                                     |
//| F48   | E879 | R4 DATA |                                                                                                     |
//| F48+E | E879 | W0 DATA | CordicWithIntegerAngle.sin.thetawrite(E49) CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0write(E50) |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'33:"33:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'33:"33:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F49   | -    | R0 CTRL |      |
//| F49   | E854 | R0 DATA |      |
//| F49+E | E854 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'34:"34:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'34:"34:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F50   | -    | R0 CTRL |      |
//| F50   | E853 | R0 DATA |      |
//| F50+E | E853 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'35:"35:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'35:"35:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F51   | -    | R0 CTRL |                       |
//| F51   | E852 | R0 DATA |                       |
//| F51+E | E852 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= CVT(C)(S64'2047&(C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(0.1001)))))>>>52)
//
//
//  E2 =.= C64u(S64'4503599627370496|C64u(S64'4503599627370495&(C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(0.1001)))))))
//
//
//  E3 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_6)
//
//
//  E4 =.= C(-1+CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4)
//
//
//  E5 =.= C64(S64'2*CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8)
//
//
//  E6 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8)
//
//
//  E7 =.= C(1+CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4)
//
//
//  E8 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_8/S64'2)
//
//
//  E9 =.= COND({[CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2>=56, (C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1))<S64'0]}, C64(-CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1), C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1))
//
//
//  E10 =.= Cu(XU32'0:"0:USA24"==(C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1)))
//
//
//  E11 =.= COND({[CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2>=56, (C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1))<S64'0]}, U32'1, U32'0)
//
//
//  E12 =.= COND({[CordicWithIntegerAngle.sin_kernel.itheta<S64'0, CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2<56]}, C64(CordicWithIntegerAngle.sin_kernel.itheta+@64_SS/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2]), C64(CordicWithIntegerAngle.sin_kernel.itheta+-@64_SS/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2]))
//
//
//  E13 =.= COND({[CordicWithIntegerAngle.sin_kernel.itheta<S64'0, CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2<56]}, C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0+(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1>>(63&CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2))), C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0+-(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1>>(63&CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2))))
//
//
//  E14 =.= COND({[CordicWithIntegerAngle.sin_kernel.itheta<S64'0, CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2<56]}, C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1+-(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0>>(63&CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2))), C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_1+(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_0>>(63&CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2))))
//
//
//  E15 =.= C(1+CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2)
//
//
//  E16 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_4)
//
//
//  E17 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_3)
//
//
//  E18 =.= C64u(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_5)
//
//
//  E19 =.= C64u(S64'2*CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12)
//
//
//  E20 =.= C(-1+CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11)
//
//
//  E21 =.= COND({[|CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_10|, S64'9007199254740992>=CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12, |CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7|]}, S64'0, S64'-9223372036854775808)
//
//
//  E22 =.= C64u(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12/S64'2)
//
//
//  E23 =.= C(1+CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11)
//
//
//  E24 =.= C64u(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9._SPILL.258|S64'4503599627370495&CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12|(CVT(C64)(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_11))<<52)
//
//
//  E25 =.= C64f(*APPLY:hpr_bitsToDouble(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13))
//
//
//  E26 =.= C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_4]))
//
//
//  E27 =.= COND((C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_4])))<0, C64f(-3.141592654+(C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_4])))), C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_4])))
//
//
//  E28 =.= COND((C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_4])))<0, U32'1, U32'0)
//
//
//  E29 =.= -CordicWithIntegerAngle.sin.theta
//
//
//  E30 =.= C64f(3.14159265358979+-CordicWithIntegerAngle.sin.theta)
//
//
//  E31 =.= C64f(CordicWithIntegerAngle.sin.theta)
//
//
//  E32 =.= CVT(C)(S64'2047&(C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(CordicWithIntegerAngle.sin.theta)))))>>>52)
//
//
//  E33 =.= C64u(S64'4503599627370496|C64u(S64'4503599627370495&(C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(CordicWithIntegerAngle.sin.theta)))))))
//
//
//  E34 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_6)
//
//
//  E35 =.= C(-1+CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4)
//
//
//  E36 =.= C64(S64'2*CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8)
//
//
//  E37 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8)
//
//
//  E38 =.= C(1+CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4)
//
//
//  E39 =.= C64(CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_8/S64'2)
//
//
//  E40 =.= COND(|CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0|, C64f(-(C64f(*APPLY:hpr_bitsToDouble(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13)))), C64f(*APPLY:hpr_bitsToDouble(CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_13)))
//
//
//  E41 =.= C64f(CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_9)
//
//
//  E42 =.= C64u(*APPLY:hpr_doubleToBits(CORDMAIN400.CordicWithIntegerAngle.sin.3.6._SPILL.256))
//
//
//  E43 =.= C(CORDMAIN400.CordicTestbench.Main.V_3)
//
//
//  E44 =.= COND((C64u(*APPLY:hpr_doubleToBits(CORDMAIN400.CordicWithIntegerAngle.sin.3.6._SPILL.256)))==@64_US/CC/SCALbx12_ARA0[CORDMAIN400.CordicTestbench.Main.V_4], S32'1, S32'0)
//
//
//  E45 =.= C(CORDMAIN400.CordicTestbench.Main._SPILL.258+CORDMAIN400.CordicTestbench.Main._SPILL.257)
//
//
//  E46 =.= C(1+CORDMAIN400.CordicTestbench.Main.V_4)
//
//
//  E47 =.= CordicWithIntegerAngle.sin.theta
//
//
//  E48 =.= C64f(CordicWithIntegerAngle.sin.theta+-6.283185307)
//
//
//  E49 =.= C64f(CordicWithIntegerAngle.sin.theta+-3.141592654)
//
//
//  E50 =.= Cu(!CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_0)
//
//
//  E51 =.= (C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(0.1001)))))<S64'1044381696
//
//
//  E52 =.= (C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(0.1001)))))>=S64'1044381696
//
//
//  E53 =.= 1020>=CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4
//
//
//  E54 =.= 1020<CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4
//
//
//  E55 =.= CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4>=1020
//
//
//  E56 =.= CORDMAIN400.CordicWithIntegerAngle.sin.2.12.V_4<1020
//
//
//  E57 =.= CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2>=56
//
//
//  E58 =.= CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_2<56
//
//
//  E59 =.= S64'4503599627370496<CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12
//
//
//  E60 =.= S64'4503599627370496>=CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12
//
//
//  E61 =.= {[S64'9007199254740992>=CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12, |CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7|]}
//
//
//  E62 =.= {[S64'9007199254740992>=CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12, |CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_7|]}
//
//
//  E63 =.= S64'9007199254740992<CORDMAIN400.CordicWithIntegerAngle.sin_kernel.26.9.V_12
//
//
//  E64 =.= (C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(CordicWithIntegerAngle.sin.theta)))))<S64'1044381696
//
//
//  E65 =.= (C64u(S64'9223372036854775807&(C64u(*APPLY:hpr_doubleToBits(CordicWithIntegerAngle.sin.theta)))))>=S64'1044381696
//
//
//  E66 =.= 1020>=CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4
//
//
//  E67 =.= 1020<CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4
//
//
//  E68 =.= CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4>=1020
//
//
//  E69 =.= CORDMAIN400.CordicWithIntegerAngle.sin.3.6.V_4<1020
//
//
//  E70 =.= CORDMAIN400.CordicTestbench.Main.V_4>=36
//
//
//  E71 =.= CORDMAIN400.CordicTestbench.Main.V_4<36
//
//
//  E72 =.= CordicWithIntegerAngle.sin.theta>=6.28318530717959
//
//
//  E73 =.= CordicWithIntegerAngle.sin.theta<6.28318530717959
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test56b to test56b

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//35 vectors of width 64
//
//1 vectors of width 6
//
//20 vectors of width 1
//
//8 vectors of width 32
//
//128 array locations of width 64
//
//Total state bits in module = 10714 bits.
//
//516 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread CordicWithIntegerAngle..cctor uid=cctor10 has 11 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread CordicTestbench..cctor uid=cctor12 has 15 CIL instructions in 1 basic blocks
//Thread CordicTestbench.Main uid=Main10 has 431 CIL instructions in 146 basic blocks
//Thread mpc10 has 36 bevelab control states (pauses)
//Reindexed thread kiwiCORDMAIN4001PC10 with 52 minor control states
// eof (HPR L/S Verilog)
