

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:50
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test56.exe -sim=18000 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
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
output reg [6:0] kiwiCORDMAIN4001PC10nz_pc_export);

function hpr_fp_dltd0; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd0_a, hpr_fp_dltd0_b;
  hpr_fp_dltd0 = ((hpr_fp_dltd0_a[63] && !hpr_fp_dltd0_b[63]) ? 1: (!hpr_fp_dltd0_a[63] && hpr_fp_dltd0_b[63])  ? 0 : (hpr_fp_dltd0_a[63]^(hpr_fp_dltd0_a[62:0]<hpr_fp_dltd0_b[62:0]))); 
   endfunction


function [31:0] rtl_unsigned_extend1;
   input argbit;
   rtl_unsigned_extend1 = { 31'b0, argbit };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg/*fp*/  [63:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_7;
  reg signed [31:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_6;
  reg signed [31:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_5;
  reg signed [31:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4;
  reg signed [31:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3;
  reg signed [31:0] CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2;
  reg CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0;
  reg/*fp*/  [63:0] CordicWithSillyFloatingAngle_sin_theta;
  reg [63:0] CORDMAIN400_CordicTestbench_Main_V_3;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_V_2;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_V_1;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_SPILL_257;
  reg signed [31:0] CORDMAIN400_CordicTestbench_Main_SPILL_258;
// abstractionName=repack-newnets
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
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_10_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_YY;
  wire ifMULTIPLIERALUF64_10_FAIL;
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
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F64I32_10
  wire/*fp*/  [63:0] CVFPCVTFL2F64I32_10_result;
  reg signed [31:0] CVFPCVTFL2F64I32_10_arg;
  wire CVFPCVTFL2F64I32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_18_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_18_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_18_YY;
  wire ifADDERALUF64_18_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_20_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_20_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_20_YY;
  wire ifADDERALUF64_20_FAIL;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire/*fp*/  [63:0] A_FPD_CC_SCALbx10_ARA0_rdata;
  reg [4:0] A_FPD_CC_SCALbx10_ARA0_addr;
  reg A_FPD_CC_SCALbx10_ARA0_wen;
  reg A_FPD_CC_SCALbx10_ARA0_ren;
  reg/*fp*/  [63:0] A_FPD_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] pipe10;
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
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh10hold;
  reg ifMULTIPLIERALUF6410RRh10shot0;
  reg ifMULTIPLIERALUF6410RRh10shot1;
  reg ifMULTIPLIERALUF6410RRh10shot2;
  reg/*fp*/  [63:0] CVFPCVTFL2F64I3210resulth10hold;
  reg CVFPCVTFL2F64I3210resulth10shot0;
  reg CVFPCVTFL2F64I3210resulth10shot1;
  reg/*fp*/  [63:0] ifADDERALUF6416RRh10hold;
  reg ifADDERALUF6416RRh10shot0;
  reg ifADDERALUF6416RRh10shot1;
  reg ifADDERALUF6416RRh10shot2;
  reg ifADDERALUF6416RRh10shot3;
  reg/*fp*/  [63:0] ifADDERALUF6414RRh10hold;
  reg ifADDERALUF6414RRh10shot0;
  reg ifADDERALUF6414RRh10shot1;
  reg ifADDERALUF6414RRh10shot2;
  reg ifADDERALUF6414RRh10shot3;
  reg/*fp*/  [63:0] FPDCCSCALbx10ARA0rdatah10hold;
  reg FPDCCSCALbx10ARA0rdatah10shot0;
  reg/*fp*/  [63:0] ifADDERALUF6418RRh10hold;
  reg ifADDERALUF6418RRh10shot0;
  reg ifADDERALUF6418RRh10shot1;
  reg ifADDERALUF6418RRh10shot2;
  reg ifADDERALUF6418RRh10shot3;
  reg/*fp*/  [63:0] ifADDERALUF6420RRh10hold;
  reg ifADDERALUF6420RRh10shot0;
  reg ifADDERALUF6420RRh10shot1;
  reg ifADDERALUF6420RRh10shot2;
  reg ifADDERALUF6420RRh10shot3;
  reg [6:0] kiwiCORDMAIN4001PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire [63:0] hprpin500718x10;
  reg/*fp*/  [63:0] hprpin501016x10;
  wire/*fp*/  [63:0] hprpin501631x10;
  wire signed [31:0] hprpin501632x10;
  wire signed [31:0] hprpin501633x10;
  wire/*fp*/  [63:0] hprpin501646x10;
  reg [63:0] hprpin501654x10;
 always   @(* )  begin 
       A_FPD_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_FPD_CC_SCALbx10_ARA0_wdata = 32'sd0;
       ifADDERALUF64_16_XX = 32'sd0;
       ifADDERALUF64_16_YY = 32'sd0;
       ifADDERALUF64_14_XX = 32'sd0;
       ifADDERALUF64_14_YY = 32'sd0;
       CVFPCVTFL2F64I32_10_arg = 32'sd0;
       ifMULTIPLIERALUF64_10_XX = 32'sd0;
       ifMULTIPLIERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_20_XX = 32'sd0;
       ifADDERALUF64_20_YY = 32'sd0;
       ifADDERALUF64_18_XX = 32'sd0;
       ifADDERALUF64_18_YY = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_12_XX = 32'sd0;
       ifADDERALUF64_12_YY = 32'sd0;
       A_FPD_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_FPD_CC_SCALbx10_ARA0_ren = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiCORDMAIN4001PC10nz)
          32'h0/*0:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd31;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3dff_ffff_ffff_fffb;
               end 
              
          32'h1/*1:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd30;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e10_0000_0000_0002;
               end 
              
          32'h2/*2:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd29;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e20_0000_0000_0007;
               end 
              
          32'h3/*3:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd28;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e2f_ffff_ffff_fff6;
               end 
              
          32'h4/*4:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd27;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e40_0000_0000_0001;
               end 
              
          32'h5/*5:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd26;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e50_0000_0000_000d;
               end 
              
          32'h6/*6:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd25;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e5f_ffff_ffff_fffc;
               end 
              
          32'h7/*7:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd24;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e6f_ffff_ffff_fffc;
               end 
              
          32'h8/*8:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd23;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e7f_ffff_ffff_ffed;
               end 
              
          32'h9/*9:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd22;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e8f_ffff_ffff_ff56;
               end 
              
          32'ha/*10:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd21;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3e9f_ffff_ffff_fd58;
               end 
              
          32'hb/*11:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd20;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3eaf_ffff_ffff_f556;
               end 
              
          32'hc/*12:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd19;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3ebf_ffff_ffff_d563;
               end 
              
          32'hd/*13:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd18;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3ecf_ffff_ffff_555e;
               end 
              
          32'he/*14:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd17;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3edf_ffff_fffd_5555;
               end 
              
          32'hf/*15:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd16;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3eef_ffff_fff5_556c;
               end 
              
          32'h10/*16:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd15;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3eff_ffff_ffd5_5556;
               end 
              
          32'h11/*17:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd14;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f0f_ffff_ff55_5559;
               end 
              
          32'h12/*18:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd13;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f1f_ffff_fd55_5547;
               end 
              
          32'h13/*19:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd12;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f2f_ffff_f555_5564;
               end 
              
          32'h14/*20:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd11;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f3f_ffff_d555_55b7;
               end 
              
          32'h15/*21:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd10;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f4f_ffff_5555_5bb8;
               end 
              
          32'h16/*22:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd9;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f5f_fffd_5555_bbc2;
               end 
              
          32'h17/*23:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd8;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f6f_fff5_555b_bbb3;
               end 
              
          32'h18/*24:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd7;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f7f_ffd5_55bb_ba96;
               end 
              
          32'h19/*25:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd6;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f8f_ff55_5bbb_7289;
               end 
              
          32'h1a/*26:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd5;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3f9f_fd55_bba9_762c;
               end 
              
          32'h1b/*27:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd4;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3faf_f55b_b72c_fdf1;
               end 
              
          32'h1c/*28:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd3;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3fbf_d5ba_9aac_2f4e;
               end 
              
          32'h1d/*29:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd2;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3fcf_5b75_f92c_80d8;
               end 
              
          32'h1e/*30:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd1;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3fdd_ac67_0561_bb4d;
               end 
              
          32'h1f/*31:kiwiCORDMAIN4001PC10nz*/:  begin 
               A_FPD_CC_SCALbx10_ARA0_addr = 32'd0;
               A_FPD_CC_SCALbx10_ARA0_wdata = 64'h3fe9_21fb_5444_2d15;
               end 
              
          32'h30/*48:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifADDERALUF64_16_XX = pipe10;
               ifADDERALUF64_16_YY = ((32'h30/*48:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? A_FPD_CC_SCALbx10_ARA0_rdata: FPDCCSCALbx10ARA0rdatah10hold
              );

               ifADDERALUF64_14_XX = pipe10;
               ifADDERALUF64_14_YY = 64'sh8000_0000_0000_0000^((32'h30/*48:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? A_FPD_CC_SCALbx10_ARA0_rdata
              : FPDCCSCALbx10ARA0rdatah10hold);

               end 
              
          32'h2d/*45:kiwiCORDMAIN4001PC10nz*/: if ((CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4>=32'sd32))  CVFPCVTFL2F64I32_10_arg
               = CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3;

               else  A_FPD_CC_SCALbx10_ARA0_addr = CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4;

          32'h36/*54:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = 64'h3e11_2e0b_e826_d695;
               ifMULTIPLIERALUF64_10_YY = CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_7;
               end 
              
          32'h21/*33:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifADDERALUF64_20_XX = 64'hc009_21fb_5444_2d11;
               ifADDERALUF64_20_YY = hprpin501016x10;
               end 
              
          32'h27/*39:kiwiCORDMAIN4001PC10nz*/:  begin 
               ifADDERALUF64_18_XX = 64'h4009_21fb_5444_2d11;
               ifADDERALUF64_18_YY = 64'sh8000_0000_0000_0000^CordicWithSillyFloatingAngle_sin_theta;
               end 
              
          32'h40/*64:kiwiCORDMAIN4001PC10nz*/: if (!hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h4019_21fb_5444_2d1c))  begin 
                   ifADDERALUF64_12_XX = CordicWithSillyFloatingAngle_sin_theta;
                   ifADDERALUF64_12_YY = 64'hc019_21fb_5444_2d1c;
                   end 
                   else  begin 
                   ifADDERALUF64_10_XX = CordicWithSillyFloatingAngle_sin_theta;
                   ifADDERALUF64_10_YY = 64'hc009_21fb_5444_2d11;
                   end 
                  endcase
      if (hpr_int_run_enable_DDX18)  begin 
               A_FPD_CC_SCALbx10_ARA0_wen = ((32'h0/*0:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h2/*2:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h4/*4:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h6/*6:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h8/*8:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'ha/*10:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'hc/*12:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'he/*14:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h10/*16:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h12/*18:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h14/*20:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h16/*22:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h18/*24:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h1a/*26:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h1c/*28:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h1e/*30:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h1f/*31:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h1d/*29:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h1b/*27:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h19/*25:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h17/*23:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h15/*21:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h13/*19:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h11/*17:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'hf/*15:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'hd/*13:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'hb/*11:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h9/*9:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h7/*7:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h5/*5:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz) || (32'h3/*3:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz) || (32'h1/*1:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz)? 32'd1: 32'd0);

               A_FPD_CC_SCALbx10_ARA0_ren = (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4<32'sd32) && (32'h2d/*45:kiwiCORDMAIN4001PC10nz*/==
              kiwiCORDMAIN4001PC10nz);

               end 
               hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.CORDMAIN400_1/1.0
      if (reset)  begin 
               kiwiCORDMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_SPILL_258 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_SPILL_257 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_V_3 <= 64'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_7 <= 64'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_6 <= 32'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_5 <= 32'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2 <= 32'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3 <= 32'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4 <= 32'd0;
               CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0 <= 32'd0;
               CordicWithSillyFloatingAngle_sin_theta <= 64'd0;
               CORDMAIN400_CordicTestbench_Main_V_1 <= 32'd0;
               CORDMAIN400_CordicTestbench_Main_V_2 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiCORDMAIN4001PC10nz)
                  32'h0/*0:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("cordic: Testbench start");
                           kiwiCORDMAIN4001PC10nz <= 32'h1/*1:kiwiCORDMAIN4001PC10nz*/;
                           done <= 32'h0;
                           CORDMAIN400_CordicTestbench_Main_SPILL_258 <= 32'sh0;
                           CORDMAIN400_CordicTestbench_Main_SPILL_257 <= 32'sh0;
                           CORDMAIN400_CordicTestbench_Main_V_3 <= 64'h0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_7 <= 64'h0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_6 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_5 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0 <= 32'h0;
                           CordicWithSillyFloatingAngle_sin_theta <= 64'h0;
                           CORDMAIN400_CordicTestbench_Main_V_1 <= 32'sh0;
                           CORDMAIN400_CordicTestbench_Main_V_2 <= 32'sh1;
                           end 
                          
                  32'h1/*1:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2/*2:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2/*2:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h3/*3:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h4/*4:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h4/*4:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h5/*5:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h5/*5:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h6/*6:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h6/*6:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h7/*7:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h7/*7:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h8/*8:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h8/*8:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h9/*9:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h9/*9:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'ha/*10:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'ha/*10:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hb/*11:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'hb/*11:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hc/*12:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'hc/*12:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hd/*13:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'hd/*13:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'he/*14:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'he/*14:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'hf/*15:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'hf/*15:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h10/*16:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h10/*16:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h11/*17:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h11/*17:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h12/*18:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h12/*18:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h13/*19:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h13/*19:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h14/*20:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h14/*20:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h15/*21:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h15/*21:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h16/*22:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h16/*22:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h17/*23:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h17/*23:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h18/*24:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h18/*24:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h19/*25:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h19/*25:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1a/*26:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1a/*26:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1b/*27:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1b/*27:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1c/*28:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1c/*28:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1d/*29:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1d/*29:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1e/*30:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1e/*30:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h1f/*31:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h1f/*31:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h20/*32:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h20/*32:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h3b/*59:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h22/*34:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h23/*35:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h23/*35:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h24/*36:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h24/*36:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h25/*37:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h25/*37:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h26/*38:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0 <= (hpr_fp_dltd0(hprpin501016x10, 64'h0)? 32'h1: 32'h0
                          );

                           CordicWithSillyFloatingAngle_sin_theta <= (hpr_fp_dltd0(hprpin501016x10, 64'h0)? ((32'h25/*37:kiwiCORDMAIN4001PC10nz*/==
                          kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_20_RR: ifADDERALUF6420RRh10hold): hprpin501016x10);

                           end 
                          
                  32'h26/*38:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h40/*64:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h28/*40:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h29/*41:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h29/*41:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2a/*42:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2a/*42:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2b/*43:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2b/*43:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (!hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h3ff9_21fb_5444_2d28))  CordicWithSillyFloatingAngle_sin_theta
                               <= ((32'h2b/*43:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_18_RR: ifADDERALUF6418RRh10hold
                              );

                               kiwiCORDMAIN4001PC10nz <= 32'h2c/*44:kiwiCORDMAIN4001PC10nz*/;
                           end 
                          
                  32'h2c/*44:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h2d/*45:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2 <= 32'sh2431_f1c7;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3 <= 32'sh0;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4 <= 32'sh0;
                           end 
                          
                  32'h2e/*46:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h2f/*47:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h2f/*47:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h36/*54:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_7 <= hprpin501646x10;
                           end 
                          
                  32'h30/*48:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h31/*49:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h31/*49:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h32/*50:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h32/*50:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h33/*51:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h33/*51:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h34/*52:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h34/*52:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h35/*53:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_6 <= hprpin501633x10;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_5 <= hprpin501632x10;
                           CordicWithSillyFloatingAngle_sin_theta <= hprpin501631x10;
                           end 
                          
                  32'h2d/*45:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4
                      <32'sd32))  kiwiCORDMAIN4001PC10nz <= 32'h30/*48:kiwiCORDMAIN4001PC10nz*/;
                           else  kiwiCORDMAIN4001PC10nz <= 32'h2e/*46:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h35/*53:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h2d/*45:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2 <= CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_5
                          ;

                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3 <= CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_6
                          ;

                           CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4 <= $signed(32'sd1+CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4
                          );

                           end 
                          
                  32'h36/*54:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h37/*55:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h37/*55:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h38/*56:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h38/*56:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h39/*57:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h39/*57:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h3a/*58:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_SPILL_258 <= ((A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_2
                          ]==hprpin501654x10)? 32'sh1: 32'sh0);

                           CORDMAIN400_CordicTestbench_Main_SPILL_257 <= CORDMAIN400_CordicTestbench_Main_V_1;
                           CORDMAIN400_CordicTestbench_Main_V_3 <= hprpin501654x10;
                           end 
                          
                  32'h3a/*58:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("Test: input=%1H expected=%1H output=%1H ", hprpin500718x10, A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_2
                          ], CORDMAIN400_CordicTestbench_Main_V_3);
                          if ((32'h0/*0:USA14*/!=(CORDMAIN400_CordicTestbench_Main_V_3^A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_2
                          ]))) $display("   test %1d hamming error=%1H", CORDMAIN400_CordicTestbench_Main_V_2, CORDMAIN400_CordicTestbench_Main_V_3
                              ^A_64_US_CC_SCALbx12_ARA0[CORDMAIN400_CordicTestbench_Main_V_2]);
                               kiwiCORDMAIN4001PC10nz <= 32'h3c/*60:kiwiCORDMAIN4001PC10nz*/;
                           CORDMAIN400_CordicTestbench_Main_V_1 <= $signed(CORDMAIN400_CordicTestbench_Main_SPILL_258+CORDMAIN400_CordicTestbench_Main_SPILL_257
                          );

                           end 
                          
                  32'h21/*33:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h22/*34:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3b/*59:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h21/*33:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3c/*60:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+CORDMAIN400_CordicTestbench_Main_V_2)>=32'sd36))  begin 
                                  $display("Result: %1d/%1d", CORDMAIN400_CordicTestbench_Main_V_1, 32'sd36);
                                  $display("cordic: Testbench finished");
                                   end 
                                   else  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h3b/*59:kiwiCORDMAIN4001PC10nz*/;
                                   CORDMAIN400_CordicTestbench_Main_V_2 <= $signed(32'sd1+CORDMAIN400_CordicTestbench_Main_V_2);
                                   end 
                                  if (($signed(32'sd1+CORDMAIN400_CordicTestbench_Main_V_2)>=32'sd36))  begin 
                                   kiwiCORDMAIN4001PC10nz <= 32'h3d/*61:kiwiCORDMAIN4001PC10nz*/;
                                   done <= 32'h1;
                                   CORDMAIN400_CordicTestbench_Main_V_2 <= $signed(32'sd1+CORDMAIN400_CordicTestbench_Main_V_2);
                                   end 
                                   end 
                          
                  32'h3d/*61:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h3e/*62:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3e/*62:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $finish(32'sd0);
                           kiwiCORDMAIN4001PC10nz <= 32'h4a/*74:kiwiCORDMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          
                  32'h41/*65:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h42/*66:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h42/*66:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h43/*67:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h43/*67:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h44/*68:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h3f/*63:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h49/*73:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h44/*68:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiCORDMAIN4001PC10nz <= 32'h3f/*63:kiwiCORDMAIN4001PC10nz*/;
                           CordicWithSillyFloatingAngle_sin_theta <= ((32'h44/*68:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_12_RR
                          : ifADDERALUF6412RRh10hold);

                           end 
                          
                  32'h45/*69:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h46/*70:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h46/*70:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h47/*71:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h47/*71:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h48/*72:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h27/*39:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h28/*40:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h48/*72:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (!hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h4009_21fb_5444_2d11))  begin 
                                   CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0 <= rtl_unsigned_extend1(!CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0
                                  );

                                   CordicWithSillyFloatingAngle_sin_theta <= ((32'h48/*72:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz
                                  )? ifADDERALUF64_10_RR: ifADDERALUF6410RRh10hold);

                                   end 
                                   kiwiCORDMAIN4001PC10nz <= 32'h27/*39:kiwiCORDMAIN4001PC10nz*/;
                           end 
                          
                  32'h40/*64:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if (hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta
                      , 64'h4019_21fb_5444_2d1c))  kiwiCORDMAIN4001PC10nz <= 32'h45/*69:kiwiCORDMAIN4001PC10nz*/;
                           else  kiwiCORDMAIN4001PC10nz <= 32'h41/*65:kiwiCORDMAIN4001PC10nz*/;
                      
                  32'h49/*73:kiwiCORDMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiCORDMAIN4001PC10nz <= 32'h40/*64:kiwiCORDMAIN4001PC10nz*/;
                      endcase
              if (reset)  begin 
               kiwiCORDMAIN4001PC10nz_pc_export <= 32'd0;
               pipe10 <= 64'd0;
               ifADDERALUF6420RRh10hold <= 64'd0;
               ifADDERALUF6420RRh10shot1 <= 32'd0;
               ifADDERALUF6420RRh10shot2 <= 32'd0;
               ifADDERALUF6420RRh10shot3 <= 32'd0;
               ifADDERALUF6418RRh10hold <= 64'd0;
               ifADDERALUF6418RRh10shot1 <= 32'd0;
               ifADDERALUF6418RRh10shot2 <= 32'd0;
               ifADDERALUF6418RRh10shot3 <= 32'd0;
               FPDCCSCALbx10ARA0rdatah10hold <= 64'd0;
               ifADDERALUF6414RRh10hold <= 64'd0;
               ifADDERALUF6414RRh10shot1 <= 32'd0;
               ifADDERALUF6414RRh10shot2 <= 32'd0;
               ifADDERALUF6414RRh10shot3 <= 32'd0;
               ifADDERALUF6416RRh10hold <= 64'd0;
               ifADDERALUF6416RRh10shot1 <= 32'd0;
               ifADDERALUF6416RRh10shot2 <= 32'd0;
               ifADDERALUF6416RRh10shot3 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10hold <= 64'd0;
               CVFPCVTFL2F64I3210resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot2 <= 32'd0;
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
               ifMULTIPLIERALUF6410RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10shot0 <= 32'd0;
               ifADDERALUF6416RRh10shot0 <= 32'd0;
               ifADDERALUF6414RRh10shot0 <= 32'd0;
               FPDCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               ifADDERALUF6418RRh10shot0 <= 32'd0;
               ifADDERALUF6420RRh10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (ifADDERALUF6412RRh10shot3)  ifADDERALUF6412RRh10hold <= ifADDERALUF64_12_RR;
                      if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh10shot2)  ifMULTIPLIERALUF6410RRh10hold <= ifMULTIPLIERALUF64_10_RR;
                      if (CVFPCVTFL2F64I3210resulth10shot1)  CVFPCVTFL2F64I3210resulth10hold <= CVFPCVTFL2F64I32_10_result;
                      if (ifADDERALUF6416RRh10shot3)  ifADDERALUF6416RRh10hold <= ifADDERALUF64_16_RR;
                      if (ifADDERALUF6414RRh10shot3)  ifADDERALUF6414RRh10hold <= ifADDERALUF64_14_RR;
                      if (FPDCCSCALbx10ARA0rdatah10shot0)  FPDCCSCALbx10ARA0rdatah10hold <= A_FPD_CC_SCALbx10_ARA0_rdata;
                      if (ifADDERALUF6418RRh10shot3)  ifADDERALUF6418RRh10hold <= ifADDERALUF64_18_RR;
                      if (ifADDERALUF6420RRh10shot3)  ifADDERALUF6420RRh10hold <= ifADDERALUF64_20_RR;
                       kiwiCORDMAIN4001PC10nz_pc_export <= kiwiCORDMAIN4001PC10nz;
                   pipe10 <= CordicWithSillyFloatingAngle_sin_theta;
                   ifADDERALUF6420RRh10shot1 <= ifADDERALUF6420RRh10shot0;
                   ifADDERALUF6420RRh10shot2 <= ifADDERALUF6420RRh10shot1;
                   ifADDERALUF6420RRh10shot3 <= ifADDERALUF6420RRh10shot2;
                   ifADDERALUF6418RRh10shot1 <= ifADDERALUF6418RRh10shot0;
                   ifADDERALUF6418RRh10shot2 <= ifADDERALUF6418RRh10shot1;
                   ifADDERALUF6418RRh10shot3 <= ifADDERALUF6418RRh10shot2;
                   ifADDERALUF6414RRh10shot1 <= ifADDERALUF6414RRh10shot0;
                   ifADDERALUF6414RRh10shot2 <= ifADDERALUF6414RRh10shot1;
                   ifADDERALUF6414RRh10shot3 <= ifADDERALUF6414RRh10shot2;
                   ifADDERALUF6416RRh10shot1 <= ifADDERALUF6416RRh10shot0;
                   ifADDERALUF6416RRh10shot2 <= ifADDERALUF6416RRh10shot1;
                   ifADDERALUF6416RRh10shot3 <= ifADDERALUF6416RRh10shot2;
                   CVFPCVTFL2F64I3210resulth10shot1 <= CVFPCVTFL2F64I3210resulth10shot0;
                   ifMULTIPLIERALUF6410RRh10shot1 <= ifMULTIPLIERALUF6410RRh10shot0;
                   ifMULTIPLIERALUF6410RRh10shot2 <= ifMULTIPLIERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifADDERALUF6412RRh10shot1 <= ifADDERALUF6412RRh10shot0;
                   ifADDERALUF6412RRh10shot2 <= ifADDERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot3 <= ifADDERALUF6412RRh10shot2;
                   ifADDERALUF6412RRh10shot0 <= !hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h4019_21fb_5444_2d1c) && (32'h40
                  /*64:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);

                   ifADDERALUF6410RRh10shot0 <= hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h4019_21fb_5444_2d1c) && (32'h40
                  /*64:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);

                   ifMULTIPLIERALUF6410RRh10shot0 <= (32'h36/*54:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   CVFPCVTFL2F64I3210resulth10shot0 <= (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4>=32'sd32) && (32'h2d/*45:kiwiCORDMAIN4001PC10nz*/==
                  kiwiCORDMAIN4001PC10nz);

                   ifADDERALUF6416RRh10shot0 <= (32'h30/*48:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   ifADDERALUF6414RRh10shot0 <= (32'h30/*48:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   FPDCCSCALbx10ARA0rdatah10shot0 <= (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4<32'sd32) && (32'h2d/*45:kiwiCORDMAIN4001PC10nz*/==
                  kiwiCORDMAIN4001PC10nz);

                   ifADDERALUF6418RRh10shot0 <= (32'h27/*39:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
                   ifADDERALUF6420RRh10shot0 <= (32'h21/*33:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz);
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
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_10_RR),
        .XX(ifMULTIPLIERALUF64_10_XX
),
        .YY(ifMULTIPLIERALUF64_10_YY),
        .FAIL(ifMULTIPLIERALUF64_10_FAIL));
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
  CV_FP_CVT_FL2_F64_I32 CVFPCVTFL2F64I32_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F64I32_10_result),
        .arg(CVFPCVTFL2F64I32_10_arg
),
        .FAIL(CVFPCVTFL2F64I32_10_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_18(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_18_RR),
        .XX(ifADDERALUF64_18_XX
),
        .YY(ifADDERALUF64_18_YY),
        .FAIL(ifADDERALUF64_18_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_20(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_20_RR),
        .XX(ifADDERALUF64_20_XX
),
        .YY(ifADDERALUF64_20_YY),
        .FAIL(ifADDERALUF64_20_FAIL));
  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd5),
        .WORDS(32'sd32),
        .LANE_WIDTH(32'sd64),
        .trace_me(32'sd0
)) A_FPD_CC_SCALbx10_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_FPD_CC_SCALbx10_ARA0_rdata),
        .addr(A_FPD_CC_SCALbx10_ARA0_addr
),
        .wen(A_FPD_CC_SCALbx10_ARA0_wen),
        .ren(A_FPD_CC_SCALbx10_ARA0_ren),
        .wdata(A_FPD_CC_SCALbx10_ARA0_wdata));

assign hprpin500718x10 = A_64_US_CC_SCALbx14_ARB0[CORDMAIN400_CordicTestbench_Main_V_2];

always @(*) hprpin501016x10 = hprpin500718x10;

assign hprpin501631x10 = (hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h0) && (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4<32'sd32)? ((32'h34
/*52:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_16_RR: ifADDERALUF6416RRh10hold): ((32'h34/*52:kiwiCORDMAIN4001PC10nz*/==
kiwiCORDMAIN4001PC10nz)? ifADDERALUF64_14_RR: ifADDERALUF6414RRh10hold));

assign hprpin501632x10 = (hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h0) && (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4<32'sd32)? $signed(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2
+(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3>>>(32'sd31&CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4))): $signed(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2
+(0-(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3>>>(32'sd31&CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4)))));

assign hprpin501633x10 = (hpr_fp_dltd0(CordicWithSillyFloatingAngle_sin_theta, 64'h0) && (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4<32'sd32)? $signed(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3
+(0-(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2>>>(32'sd31&CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4)))): $signed(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_3
+(CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_2>>>(32'sd31&CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4))));

assign hprpin501646x10 = (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_0 && (CORDMAIN400_CordicWithSillyFloatingAngle_sin_3_7_V_4>=32'sd32)? 64'sh8000_0000_0000_0000
^((32'h2f/*47:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? CVFPCVTFL2F64I32_10_result: CVFPCVTFL2F64I3210resulth10hold): ((32'h2f
/*47:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? CVFPCVTFL2F64I32_10_result: CVFPCVTFL2F64I3210resulth10hold));

always @(*) hprpin501654x10 = ((32'h39/*57:kiwiCORDMAIN4001PC10nz*/==kiwiCORDMAIN4001PC10nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold);

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
      

// Structural Resource (FU) inventory for DUT:// 30 vectors of width 64
// 1 vectors of width 7
// 34 vectors of width 1
// 1 vectors of width 5
// 10 vectors of width 32
// 72 array locations of width 64
// Total state bits in module = 6894 bits.
// 840 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL4_ADDER_DP count=6
//   cell CV_FP_FL3_MULTIPLIER_DP count=1
//   cell CV_FP_CVT_FL2_F64_I32 count=1
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 9
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:46
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test56.exe -sim=18000 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


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
//KiwiC: front end input processing of class CordicWithSillyFloatingAngle  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=CordicWithSillyFloatingAngle..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=CordicWithSillyFloatingAngle..cctor
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
//   kiwife-firstpass-loglevel=3
//
//
//   kiwife-overloads-loglevel=3
//
//
//   root=$attributeroot
//
//
//   srcfile=test56.exe
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
//*-----------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause                           | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*-----------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiCORDMAIN4001PC10"         | 837 | 0       | hwm=0.0.32   | 0    |        | 1     | 32  | 59   |
//| XU32'2:"2:kiwiCORDMAIN4001PC10"         | 835 | 33      | hwm=0.4.0    | 37   |        | 34    | 37  | 38   |
//| XU32'4:"4:kiwiCORDMAIN4001PC10"         | 834 | 38      | hwm=0.0.0    | 38   |        | -     | -   | 64   |
//| XU32'16:"16:kiwiCORDMAIN4001PC10"       | 831 | 39      | hwm=0.4.0    | 43   |        | 40    | 43  | 44   |
//| XU32'32:"32:kiwiCORDMAIN4001PC10"       | 830 | 44      | hwm=0.0.0    | 44   |        | -     | -   | 45   |
//| XU32'64:"64:kiwiCORDMAIN4001PC10"       | 828 | 45      | hwm=0.5.0    | 52   |        | 48    | 52  | 53   |
//| XU32'64:"64:kiwiCORDMAIN4001PC10"       | 829 | 45      | hwm=0.2.0    | 47   |        | 46    | 47  | 54   |
//| XU32'128:"128:kiwiCORDMAIN4001PC10"     | 827 | 53      | hwm=0.0.0    | 53   |        | -     | -   | 45   |
//| XU32'256:"256:kiwiCORDMAIN4001PC10"     | 826 | 54      | hwm=0.3.0    | 57   |        | 55    | 57  | 58   |
//| XU32'512:"512:kiwiCORDMAIN4001PC10"     | 825 | 58      | hwm=0.0.0    | 58   |        | -     | -   | 60   |
//| XU32'1:"1:kiwiCORDMAIN4001PC10"         | 836 | 59      | hwm=0.0.0    | 59   |        | -     | -   | 33   |
//| XU32'1024:"1024:kiwiCORDMAIN4001PC10"   | 823 | 60      | hwm=0.0.0    | 60   |        | -     | -   | 59   |
//| XU32'1024:"1024:kiwiCORDMAIN4001PC10"   | 824 | 60      | hwm=0.0.0    | 60   |        | -     | -   | 61   |
//| XU32'2048:"2048:kiwiCORDMAIN4001PC10"   | 822 | 61      | hwm=0.0.0    | 61   |        | -     | -   | 62   |
//| XU32'4096:"4096:kiwiCORDMAIN4001PC10"   | 821 | 62      | hwm=0.0.0    | 62   |        | -     | -   | -    |
//| XU32'8192:"8192:kiwiCORDMAIN4001PC10"   | 820 | 63      | hwm=0.0.0    | 63   |        | -     | -   | 73   |
//| XU32'8:"8:kiwiCORDMAIN4001PC10"         | 832 | 64      | hwm=0.4.0    | 72   |        | 69    | 72  | 39   |
//| XU32'8:"8:kiwiCORDMAIN4001PC10"         | 833 | 64      | hwm=0.4.0    | 68   |        | 65    | 68  | 63   |
//| XU32'16384:"16384:kiwiCORDMAIN4001PC10" | 819 | 73      | hwm=0.0.0    | 73   |        | -     | -   | 64   |
//*-----------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'0:"0:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'0:"0:kiwiCORDMAIN4001PC10"
//*------+------+----------+--------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser   | Work                                                                                                               |
//*------+------+----------+--------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL  |                                                                                                                    |
//| F0   | E837 | R0 DATA  |                                                                                                                    |
//| F0+E | E837 | W0 DATA  | CORDMAIN400.CordicTestbench.Main.V_2write(S32'1) CORDMAIN400.CordicTestbench.Main.V_1write(S32'0) CordicWithSilly\ |
//|      |      |          | FloatingAngle.sin.thetawrite(??64'0) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_0write(U32'0) CORDMAIN400\ |
//|      |      |          | .CordicWithSillyFloatingAngle.sin.3.7.V_4write(S32'0) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3write(S\ |
//|      |      |          | 32'0) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2write(S32'0) CORDMAIN400.CordicWithSillyFloatingAngle.s\ |
//|      |      |          | in.3.7.V_5write(S32'0) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_6write(S32'0) CORDMAIN400.CordicWithSil\ |
//|      |      |          | lyFloatingAngle.sin.3.7.V_7write(??64'0) CORDMAIN400.CordicTestbench.Main.V_3write(U64'0) CORDMAIN400.CordicTestb\ |
//|      |      |          | ench.Main._SPILL.257write(S32'0) CORDMAIN400.CordicTestbench.Main._SPILL.258write(S32'0) donewrite(U32'0) @_FPD/C\ |
//|      |      |          | C/SCALbx10_ARA0_write(31, ??64'4467570830351532027)  PLI:cordic: Testbench st...                                   |
//| F1   | E837 | W1 DATA  | @_FPD/CC/SCALbx10_ARA0_write(30, ??64'4472074429978902530)                                                         |
//| F2   | E837 | W2 DATA  | @_FPD/CC/SCALbx10_ARA0_write(29, ??64'4476578029606273031)                                                         |
//| F3   | E837 | W3 DATA  | @_FPD/CC/SCALbx10_ARA0_write(28, ??64'4481081629233643510)                                                         |
//| F4   | E837 | W4 DATA  | @_FPD/CC/SCALbx10_ARA0_write(27, ??64'4485585228861014017)                                                         |
//| F5   | E837 | W5 DATA  | @_FPD/CC/SCALbx10_ARA0_write(26, ??64'4490088828488384525)                                                         |
//| F6   | E837 | W6 DATA  | @_FPD/CC/SCALbx10_ARA0_write(25, ??64'4494592428115755004)                                                         |
//| F7   | E837 | W7 DATA  | @_FPD/CC/SCALbx10_ARA0_write(24, ??64'4499096027743125500)                                                         |
//| F8   | E837 | W8 DATA  | @_FPD/CC/SCALbx10_ARA0_write(23, ??64'4503599627370495981)                                                         |
//| F9   | E837 | W9 DATA  | @_FPD/CC/SCALbx10_ARA0_write(22, ??64'4508103226997866326)                                                         |
//| F10  | E837 | W10 DATA | @_FPD/CC/SCALbx10_ARA0_write(21, ??64'4512606826625236312)                                                         |
//| F11  | E837 | W11 DATA | @_FPD/CC/SCALbx10_ARA0_write(20, ??64'4517110426252604758)                                                         |
//| F12  | E837 | W12 DATA | @_FPD/CC/SCALbx10_ARA0_write(19, ??64'4521614025879967075)                                                         |
//| F13  | E837 | W13 DATA | @_FPD/CC/SCALbx10_ARA0_write(18, ??64'4526117625507304798)                                                         |
//| F14  | E837 | W14 DATA | @_FPD/CC/SCALbx10_ARA0_write(17, ??64'4530621225134544213)                                                         |
//| F15  | E837 | W15 DATA | @_FPD/CC/SCALbx10_ARA0_write(16, ??64'4535124824761390444)                                                         |
//| F16  | E837 | W16 DATA | @_FPD/CC/SCALbx10_ARA0_write(15, ??64'4539628424386663766)                                                         |
//| F17  | E837 | W17 DATA | @_FPD/CC/SCALbx10_ARA0_write(14, ??64'4544132024005645657)                                                         |
//| F18  | E837 | W18 DATA | @_FPD/CC/SCALbx10_ARA0_write(13, ??64'4548635623599461703)                                                         |
//| F19  | E837 | W19 DATA | @_FPD/CC/SCALbx10_ARA0_write(12, ??64'4553139223092614500)                                                         |
//| F20  | E837 | W20 DATA | @_FPD/CC/SCALbx10_ARA0_write(11, ??64'4557642822183114167)                                                         |
//| F21  | E837 | W21 DATA | @_FPD/CC/SCALbx10_ARA0_write(10, ??64'4562146419663002552)                                                         |
//| F22  | E837 | W22 DATA | @_FPD/CC/SCALbx10_ARA0_write(9, ??64'4566650010700463042)                                                          |
//| F23  | E837 | W23 DATA | @_FPD/CC/SCALbx10_ARA0_write(8, ??64'4571153575968488371)                                                          |
//| F24  | E837 | W24 DATA | @_FPD/CC/SCALbx10_ARA0_write(7, ??64'4575657038163196566)                                                          |
//| F25  | E837 | W25 DATA | @_FPD/CC/SCALbx10_ARA0_write(6, ??64'4580160088135398025)                                                          |
//| F26  | E837 | W26 DATA | @_FPD/CC/SCALbx10_ARA0_write(5, ??64'4584661490348946988)                                                          |
//| F27  | E837 | W27 DATA | @_FPD/CC/SCALbx10_ARA0_write(4, ??64'4589156319577832945)                                                          |
//| F28  | E837 | W28 DATA | @_FPD/CC/SCALbx10_ARA0_write(3, ??64'4593625142376804174)                                                          |
//| F29  | E837 | W29 DATA | @_FPD/CC/SCALbx10_ARA0_write(2, ??64'4597994306818310360)                                                          |
//| F30  | E837 | W30 DATA | @_FPD/CC/SCALbx10_ARA0_write(1, ??64'4602023952714414925)                                                          |
//| F31  | E837 | W31 DATA | @_FPD/CC/SCALbx10_ARA0_write(0, ??64'4605249457297304853)                                                          |
//| F32  | E837 | W32 DATA |                                                                                                                    |
//*------+------+----------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2:"2:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2:"2:kiwiCORDMAIN4001PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                          |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*
//| F33   | -    | R0 CTRL |                                                                                                               |
//| F33   | E835 | R0 DATA | ifADDERALUF64_20_compute(-3.141592654, E1)                                                                    |
//| F34   | E835 | R1 DATA |                                                                                                               |
//| F35   | E835 | R2 DATA |                                                                                                               |
//| F36   | E835 | R3 DATA |                                                                                                               |
//| F37   | E835 | R4 DATA |                                                                                                               |
//| F37+E | E835 | W0 DATA | CordicWithSillyFloatingAngle.sin.thetawrite(E2) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_0write(E3) |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4:"4:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4:"4:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F38   | -    | R0 CTRL |      |
//| F38   | E834 | R0 DATA |      |
//| F38+E | E834 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16:"16:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16:"16:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                            |
//*-------+------+---------+-------------------------------------------------*
//| F39   | -    | R0 CTRL |                                                 |
//| F39   | E831 | R0 DATA | ifADDERALUF64_18_compute(3.14159265358979, E4)  |
//| F40   | E831 | R1 DATA |                                                 |
//| F41   | E831 | R2 DATA |                                                 |
//| F42   | E831 | R3 DATA |                                                 |
//| F43   | E831 | R4 DATA |                                                 |
//| F43+E | E831 | W0 DATA | CordicWithSillyFloatingAngle.sin.thetawrite(E5) |
//*-------+------+---------+-------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'32:"32:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'32:"32:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                   |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| F44   | -    | R0 CTRL |                                                                                                                                        |
//| F44   | E830 | R0 DATA |                                                                                                                                        |
//| F44+E | E830 | W0 DATA | CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4write(S32'0) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3write(S32'0) COR\ |
//|       |      |         | DMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2write(S32'607252935)                                                                  |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'64:"64:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'64:"64:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                 |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| F45   | -    | R0 CTRL |                                                                                                                      |
//| F45   | E829 | R0 DATA | CVFPCVTFL2F64I32_10_hpr_dbl_from_int32(E6)                                                                           |
//| F46   | E829 | R1 DATA |                                                                                                                      |
//| F47   | E829 | R2 DATA |                                                                                                                      |
//| F47+E | E829 | W0 DATA | CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_7write(E7)                                                        |
//| F45   | E828 | R0 DATA | @_FPD/CC/SCALbx10_ARA0_read(E8)                                                                                      |
//| F48   | E828 | R1 DATA | ifADDERALUF64_14_compute(E9, E10) ifADDERALUF64_16_compute(E9, E11)                                                  |
//| F49   | E828 | R2 DATA |                                                                                                                      |
//| F50   | E828 | R3 DATA |                                                                                                                      |
//| F51   | E828 | R4 DATA |                                                                                                                      |
//| F52   | E828 | R5 DATA |                                                                                                                      |
//| F52+E | E828 | W0 DATA | CordicWithSillyFloatingAngle.sin.thetawrite(E12) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_5write(E13) COR\ |
//|       |      |         | DMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_6write(E14)                                                          |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'128:"128:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'128:"128:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                          |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| F53   | -    | R0 CTRL |                                                                                                                               |
//| F53   | E827 | R0 DATA |                                                                                                                               |
//| F53+E | E827 | W0 DATA | CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4write(E15) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3write(E16\ |
//|       |      |         | ) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2write(E17)                                                              |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'256:"256:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'256:"256:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                       |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F54   | -    | R0 CTRL |                                                                                                                                                            |
//| F54   | E826 | R0 DATA | ifMULTIPLIERALUF64_10_compute(1E-09, E18)                                                                                                                  |
//| F55   | E826 | R1 DATA |                                                                                                                                                            |
//| F56   | E826 | R2 DATA |                                                                                                                                                            |
//| F57   | E826 | R3 DATA |                                                                                                                                                            |
//| F57+E | E826 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_3write(E19) CORDMAIN400.CordicTestbench.Main._SPILL.257write(E20) CORDMAIN400.CordicTestbench.Main._SPILL.258write(E21) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'512:"512:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'512:"512:kiwiCORDMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                     |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| F58   | -    | R0 CTRL |                                                                                                          |
//| F58   | E825 | R0 DATA |                                                                                                          |
//| F58+E | E825 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_1write(E22)  PLI:   test %d hamming e...  PLI:Test: input=%X expec... |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1:"1:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1:"1:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F59   | -    | R0 CTRL |      |
//| F59   | E836 | R0 DATA |      |
//| F59+E | E836 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1024:"1024:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'1024:"1024:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                            |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| F60   | -    | R0 CTRL |                                                                                                                 |
//| F60   | E824 | R0 DATA |                                                                                                                 |
//| F60+E | E824 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_2write(E23) donewrite(U32'1)  PLI:cordic: Testbench fi...  PLI:Result: %d/%d |
//| F60   | E823 | R0 DATA |                                                                                                                 |
//| F60+E | E823 | W0 DATA | CORDMAIN400.CordicTestbench.Main.V_2write(E23)                                                                  |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2048:"2048:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'2048:"2048:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F61   | -    | R0 CTRL |      |
//| F61   | E822 | R0 DATA |      |
//| F61+E | E822 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4096:"4096:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'4096:"4096:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F62   | -    | R0 CTRL |                       |
//| F62   | E821 | R0 DATA |                       |
//| F62+E | E821 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8192:"8192:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8192:"8192:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F63   | -    | R0 CTRL |      |
//| F63   | E820 | R0 DATA |      |
//| F63+E | E820 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8:"8:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'8:"8:kiwiCORDMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                            |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| F64   | -    | R0 CTRL |                                                                                                                 |
//| F64   | E833 | R0 DATA | ifADDERALUF64_12_compute(E9, -6.283185307)                                                                      |
//| F65   | E833 | R1 DATA |                                                                                                                 |
//| F66   | E833 | R2 DATA |                                                                                                                 |
//| F67   | E833 | R3 DATA |                                                                                                                 |
//| F68   | E833 | R4 DATA |                                                                                                                 |
//| F68+E | E833 | W0 DATA | CordicWithSillyFloatingAngle.sin.thetawrite(E24)                                                                |
//| F64   | E832 | R0 DATA | ifADDERALUF64_10_compute(E9, -3.141592654)                                                                      |
//| F69   | E832 | R1 DATA |                                                                                                                 |
//| F70   | E832 | R2 DATA |                                                                                                                 |
//| F71   | E832 | R3 DATA |                                                                                                                 |
//| F72   | E832 | R4 DATA |                                                                                                                 |
//| F72+E | E832 | W0 DATA | CordicWithSillyFloatingAngle.sin.thetawrite(E25) CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_0write(E26) |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16384:"16384:kiwiCORDMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiCORDMAIN4001PC10 state=XU32'16384:"16384:kiwiCORDMAIN4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F73   | -    | R0 CTRL |      |
//| F73   | E819 | R0 DATA |      |
//| F73+E | E819 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_2]))
//
//
//  E2 =.= COND((C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_2])))<0, C64f(-3.141592654+(C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_2])))), C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_2])))
//
//
//  E3 =.= COND((C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[CORDMAIN400.CordicTestbench.Main.V_2])))<0, U32'1, U32'0)
//
//
//  E4 =.= -CordicWithSillyFloatingAngle.sin.theta
//
//
//  E5 =.= C64f(3.14159265358979+-CordicWithSillyFloatingAngle.sin.theta)
//
//
//  E6 =.= CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3
//
//
//  E7 =.= COND({[|CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_0|, CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4>=32]}, C64f(-(CVT(C64f)(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3))), CVT(C64f)(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3))
//
//
//  E8 =.= CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4
//
//
//  E9 =.= CordicWithSillyFloatingAngle.sin.theta
//
//
//  E10 =.= -@_FPD/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4]
//
//
//  E11 =.= @_FPD/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4]
//
//
//  E12 =.= COND({[CordicWithSillyFloatingAngle.sin.theta<0, CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4<32]}, C64f(CordicWithSillyFloatingAngle.sin.theta+@_FPD/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4]), C64f(CordicWithSillyFloatingAngle.sin.theta+-@_FPD/CC/SCALbx10_ARA0[CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4]))
//
//
//  E13 =.= COND({[CordicWithSillyFloatingAngle.sin.theta<0, CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4<32]}, C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2+(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3>>(31&CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4))), C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2+-(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3>>(31&CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4))))
//
//
//  E14 =.= COND({[CordicWithSillyFloatingAngle.sin.theta<0, CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4<32]}, C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3+-(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2>>(31&CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4))), C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_3+(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_2>>(31&CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4))))
//
//
//  E15 =.= C(1+CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4)
//
//
//  E16 =.= C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_6)
//
//
//  E17 =.= C(CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_5)
//
//
//  E18 =.= CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_7
//
//
//  E19 =.= C64u(*APPLY:hpr_doubleToBits(1E-09*CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_7))
//
//
//  E20 =.= C(CORDMAIN400.CordicTestbench.Main.V_1)
//
//
//  E21 =.= COND((C64u(*APPLY:hpr_doubleToBits(1E-09*CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_7)))==@64_US/CC/SCALbx12_ARA0[CORDMAIN400.CordicTestbench.Main.V_2], S32'1, S32'0)
//
//
//  E22 =.= C(CORDMAIN400.CordicTestbench.Main._SPILL.258+CORDMAIN400.CordicTestbench.Main._SPILL.257)
//
//
//  E23 =.= C(1+CORDMAIN400.CordicTestbench.Main.V_2)
//
//
//  E24 =.= C64f(CordicWithSillyFloatingAngle.sin.theta+-6.283185307)
//
//
//  E25 =.= C64f(CordicWithSillyFloatingAngle.sin.theta+-3.141592654)
//
//
//  E26 =.= Cu(!CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_0)
//
//
//  E27 =.= CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4>=32
//
//
//  E28 =.= CORDMAIN400.CordicWithSillyFloatingAngle.sin.3.7.V_4<32
//
//
//  E29 =.= (C(1+CORDMAIN400.CordicTestbench.Main.V_2))>=36
//
//
//  E30 =.= (C(1+CORDMAIN400.CordicTestbench.Main.V_2))<36
//
//
//  E31 =.= CordicWithSillyFloatingAngle.sin.theta>=6.28318530717959
//
//
//  E32 =.= CordicWithSillyFloatingAngle.sin.theta<6.28318530717959
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test56 to test56

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//30 vectors of width 64
//
//1 vectors of width 7
//
//34 vectors of width 1
//
//1 vectors of width 5
//
//10 vectors of width 32
//
//72 array locations of width 64
//
//Total state bits in module = 6894 bits.
//
//840 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread CordicWithSillyFloatingAngle..cctor uid=cctor10 has 11 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread CordicTestbench..cctor uid=cctor12 has 15 CIL instructions in 1 basic blocks
//Thread CordicTestbench.Main uid=Main10 has 127 CIL instructions in 51 basic blocks
//Thread mpc10 has 16 bevelab control states (pauses)
//Reindexed thread kiwiCORDMAIN4001PC10 with 74 minor control states
// eof (HPR L/S Verilog)
