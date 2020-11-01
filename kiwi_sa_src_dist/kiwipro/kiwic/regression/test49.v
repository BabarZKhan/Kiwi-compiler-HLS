

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:27
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test49.exe -sim=1800 -diosim-vcd=test49.vcd -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test49.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=res2-directornets */
output reg [5:0] kiwiTESTMAIN4001PC10nz_pc_export);
function [31:0] hpr_dbl2flt1;
input [63:0] arg;

reg          signi;
reg [10:0]   expi;
reg [51:0]   manti;
reg [7:0]    expo;
reg [22:0]   manto;
reg  overflow, scase_inf, scase_zero, scase_nan, fail;

begin
  { signi, expi, manti } = arg;  // Deconstruct input arg
  scase_zero = (arg[62:0] == 63'd0);
  scase_inf = (expi == 11'h7ff) && (manti == 0);
  scase_nan = (expi == 11'h7ff) && (manti != 0);
// We can report fail on overflow but better to report infinity.
  fail = 0;
  overflow = (expi[10] == expi[9]) ||(expi[10] == expi[8]) ||(expi[10] == expi[7]);
  expo = { expi[10], expi[6:0]};
  manto = manti[51:51-22];
  scase_inf = scase_inf || overflow;
  hpr_dbl2flt1[31]    = signi;
  hpr_dbl2flt1[30:23] = (scase_inf)? 8'hff: (scase_nan)? 8'hff: (scase_zero)? 8'd0: expo;
  hpr_dbl2flt1[22:0]  = (scase_inf)? 23'd0: (scase_nan)? -23'd1: (scase_zero)? 23'd0: manto;
end
endfunction


function [63:0] hpr_flt2dbl0;//Floating-point convert single to double precision.
input [31:0] darg;
  hpr_flt2dbl0 =  ((darg & 32'h7F80_0000)==32'h7F80_0000)?
     {darg[31], {4{darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}}:
     {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};
endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test49_test49_phase1_0_6_V_1;
  reg signed [31:0] TESTMAIN400_test49_test49_phase0_0_5_V_4;
  reg/*fp*/  [31:0] TESTMAIN400_test49_test49_phase0_0_5_V_3;
  reg/*fp*/  [31:0] TESTMAIN400_test49_test49_phase0_0_5_V_2;
  reg/*fp*/  [63:0] TESTMAIN400_test49_test49_phase0_0_5_V_1;
  reg signed [31:0] TESTMAIN400_test49_test49_phase0_0_5_V_0;
  reg signed [31:0] test49_volx;
// abstractionName=repack-newnets
  reg/*fp*/  [63:0] A_FPD_CC_SCALbx10_ARA0[5:0];
// abstractionName=res2-contacts pi_name=CV_FP_FL5_DIVIDER_DP
  wire/*fp*/  [63:0] ifDIVIDERALUF64_10_RR;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_10_NN;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_10_DD;
  wire ifDIVIDERALUF64_10_FAIL;
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
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_12_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_YY;
  wire ifMULTIPLIERALUF64_12_FAIL;
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
// abstractionName=res2-contacts pi_name=CVFPCVTFL2I32F32_10
  wire signed [31:0] CVFPCVTFL2I32F32_10_result;
  reg/*fp*/  [31:0] CVFPCVTFL2I32F32_10_arg;
  wire CVFPCVTFL2I32F32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_14_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_14_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_14_YY;
  wire ifMULTIPLIERALUF64_14_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F64I32_12
  wire/*fp*/  [63:0] CVFPCVTFL2F64I32_12_result;
  reg signed [31:0] CVFPCVTFL2F64I32_12_arg;
  wire CVFPCVTFL2F64I32_12_FAIL;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh10hold;
  reg ifMULTIPLIERALUF6410RRh10shot0;
  reg ifMULTIPLIERALUF6410RRh10shot1;
  reg ifMULTIPLIERALUF6410RRh10shot2;
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
  reg/*fp*/  [63:0] ifDIVIDERALUF6410RRh10hold;
  reg ifDIVIDERALUF6410RRh10shot0;
  reg ifDIVIDERALUF6410RRh10shot1;
  reg ifDIVIDERALUF6410RRh10shot2;
  reg ifDIVIDERALUF6410RRh10shot3;
  reg ifDIVIDERALUF6410RRh10shot4;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6412RRh10hold;
  reg ifMULTIPLIERALUF6412RRh10shot0;
  reg ifMULTIPLIERALUF6412RRh10shot1;
  reg ifMULTIPLIERALUF6412RRh10shot2;
  reg/*fp*/  [63:0] CVFPCVTFL2F64I3210resulth10hold;
  reg CVFPCVTFL2F64I3210resulth10shot0;
  reg CVFPCVTFL2F64I3210resulth10shot1;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF3210RRh10hold;
  reg ifMULTIPLIERALUF3210RRh10shot0;
  reg ifMULTIPLIERALUF3210RRh10shot1;
  reg ifMULTIPLIERALUF3210RRh10shot2;
  reg ifMULTIPLIERALUF3210RRh10shot3;
  reg ifMULTIPLIERALUF3210RRh10shot4;
  reg signed [31:0] CVFPCVTFL2I32F3210resulth10hold;
  reg CVFPCVTFL2I32F3210resulth10shot0;
  reg CVFPCVTFL2I32F3210resulth10shot1;
  reg/*fp*/  [31:0] CVFPCVTFL2F32I3210resulth10hold;
  reg CVFPCVTFL2F32I3210resulth10shot0;
  reg CVFPCVTFL2F32I3210resulth10shot1;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6414RRh10hold;
  reg ifMULTIPLIERALUF6414RRh10shot0;
  reg ifMULTIPLIERALUF6414RRh10shot1;
  reg ifMULTIPLIERALUF6414RRh10shot2;
  reg/*fp*/  [63:0] CVFPCVTFL2F64I3212resulth10hold;
  reg CVFPCVTFL2F64I3212resulth10shot0;
  reg CVFPCVTFL2F64I3212resulth10shot1;
  reg [5:0] kiwiTESTMAIN4001PC10nz;
 always   @(* )  begin 
       CVFPCVTFL2F64I32_12_arg = 32'sd0;
       ifMULTIPLIERALUF64_14_XX = 32'sd0;
       ifMULTIPLIERALUF64_14_YY = 32'sd0;
       ifMULTIPLIERALUF32_10_XX = 32'sd0;
       ifMULTIPLIERALUF32_10_YY = 32'sd0;
       CVFPCVTFL2I32F32_10_arg = 32'sd0;
       CVFPCVTFL2F32I32_10_arg = 32'sd0;
       CVFPCVTFL2F64I32_10_arg = 32'sd0;
       ifMULTIPLIERALUF64_12_XX = 32'sd0;
       ifMULTIPLIERALUF64_12_YY = 32'sd0;
       ifMULTIPLIERALUF64_10_XX = 32'sd0;
       ifMULTIPLIERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_12_XX = 32'sd0;
       ifADDERALUF64_12_YY = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifDIVIDERALUF64_10_NN = 32'sd0;
       ifDIVIDERALUF64_10_DD = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;

      case (kiwiTESTMAIN4001PC10nz)
          32'h1/*1:kiwiTESTMAIN4001PC10nz*/:  CVFPCVTFL2F64I32_12_arg = test49_volx;

          32'h3/*3:kiwiTESTMAIN4001PC10nz*/:  begin 
               ifMULTIPLIERALUF64_14_XX = ((32'h3/*3:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? CVFPCVTFL2F64I32_12_result: CVFPCVTFL2F64I3212resulth10hold
              );

               ifMULTIPLIERALUF64_14_YY = 64'h40aa_0466_6666_6666;
               end 
              
          32'h9/*9:kiwiTESTMAIN4001PC10nz*/:  begin 
               ifMULTIPLIERALUF32_10_XX = 32'h40e3_f34d;
               ifMULTIPLIERALUF32_10_YY = ((32'h9/*9:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? CVFPCVTFL2F32I32_10_result: CVFPCVTFL2F32I3210resulth10hold
              );

               end 
              
          32'h7/*7:kiwiTESTMAIN4001PC10nz*/:  begin 
               CVFPCVTFL2I32F32_10_arg = TESTMAIN400_test49_test49_phase0_0_5_V_2;
               CVFPCVTFL2F32I32_10_arg = TESTMAIN400_test49_test49_phase0_0_5_V_0;
               end 
              
          32'h11/*17:kiwiTESTMAIN4001PC10nz*/: if (($signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0)<32'sd6))  CVFPCVTFL2F64I32_10_arg
               = test49_volx+$signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0);

              
          32'h13/*19:kiwiTESTMAIN4001PC10nz*/:  begin 
               ifMULTIPLIERALUF64_12_XX = 64'h40aa_0466_6666_6666;
               ifMULTIPLIERALUF64_12_YY = ((32'h13/*19:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? CVFPCVTFL2F64I32_10_result: CVFPCVTFL2F64I3210resulth10hold
              );

               end 
              
          32'h18/*24:kiwiTESTMAIN4001PC10nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = 64'h4059_0000_0000_0000;
               ifMULTIPLIERALUF64_10_YY = A_FPD_CC_SCALbx10_ARA0[64'd1];
               ifADDERALUF64_12_XX = 64'hc059_0000_0000_0000;
               ifADDERALUF64_12_YY = A_FPD_CC_SCALbx10_ARA0[64'd2];
               ifADDERALUF64_10_XX = 64'h4059_0000_0000_0000;
               ifADDERALUF64_10_YY = A_FPD_CC_SCALbx10_ARA0[64'd4];
               ifDIVIDERALUF64_10_NN = A_FPD_CC_SCALbx10_ARA0[64'd3];
               ifDIVIDERALUF64_10_DD = 64'h4059_0000_0000_0000;
               end 
              endcase
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400_1/1.0
      if (reset)  begin 
               A_FPD_CC_SCALbx10_ARA0[64'd4] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[64'd3] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[64'd2] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[64'd1] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[64'd5] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[32'sh4] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[32'sh3] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[32'sh2] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[32'sh1] <= 64'd0;
               A_FPD_CC_SCALbx10_ARA0[32'sh0] <= 64'd0;
               kiwiTESTMAIN4001PC10nz <= 32'd0;
               test49_volx <= 32'd0;
               TESTMAIN400_test49_test49_phase1_0_6_V_1 <= 32'd0;
               TESTMAIN400_test49_test49_phase0_0_5_V_4 <= 32'd0;
               TESTMAIN400_test49_test49_phase0_0_5_V_3 <= 32'd0;
               TESTMAIN400_test49_test49_phase0_0_5_V_2 <= 32'd0;
               TESTMAIN400_test49_test49_phase0_0_5_V_1 <= 64'd0;
               TESTMAIN400_test49_test49_phase0_0_5_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN4001PC10nz)
                  32'h0/*0:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Kiwi Demo - Test49 starting.");
                           kiwiTESTMAIN4001PC10nz <= 32'h1/*1:kiwiTESTMAIN4001PC10nz*/;
                           test49_volx <= 32'sh64;
                           TESTMAIN400_test49_test49_phase1_0_6_V_1 <= 32'sh0;
                           TESTMAIN400_test49_test49_phase0_0_5_V_4 <= 32'sh0;
                           TESTMAIN400_test49_test49_phase0_0_5_V_3 <= 32'h0;
                           TESTMAIN400_test49_test49_phase0_0_5_V_2 <= 32'h0;
                           TESTMAIN400_test49_test49_phase0_0_5_V_1 <= 64'h0;
                           TESTMAIN400_test49_test49_phase0_0_5_V_0 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h2/*2:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h2/*2:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h4/*4:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h5/*5:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h6/*6:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Kiwi Demo - Test49 phase0 starting.");
                           kiwiTESTMAIN4001PC10nz <= 32'h10/*16:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase0_0_5_V_1 <= ((32'h6/*6:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifMULTIPLIERALUF64_14_RR
                          : ifMULTIPLIERALUF6414RRh10hold);

                           TESTMAIN400_test49_test49_phase0_0_5_V_0 <= 32'sh0;
                           end 
                          
                  32'h8/*8:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h9/*9:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'ha/*10:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'hb/*11:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'hc/*12:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hc/*12:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'hd/*13:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'he/*14:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'he/*14:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'hf/*15:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase0_0_5_V_4 <= ((32'h9/*9:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? CVFPCVTFL2I32F32_10_result
                          : CVFPCVTFL2I32F3210resulth10hold);

                           TESTMAIN400_test49_test49_phase0_0_5_V_3 <= ((32'he/*14:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifMULTIPLIERALUF32_10_RR
                          : ifMULTIPLIERALUF3210RRh10hold);

                           end 
                          
                  32'hf/*15:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("                  qfp1=%f  qfp2=%f  qfp3=%1d", $bitstoreal(hpr_flt2dbl0(TESTMAIN400_test49_test49_phase0_0_5_V_2
                          )), $bitstoreal(hpr_flt2dbl0(TESTMAIN400_test49_test49_phase0_0_5_V_3)), TESTMAIN400_test49_test49_phase0_0_5_V_4
                          );
                           kiwiTESTMAIN4001PC10nz <= 32'h11/*17:kiwiTESTMAIN4001PC10nz*/;
                           end 
                          
                  32'h7/*7:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h8/*8:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h11/*17:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0)>=32'sd6)) $display("Kiwi Demo - Test49 phase1 starting."
                              );
                               else  kiwiTESTMAIN4001PC10nz <= 32'h12/*18:kiwiTESTMAIN4001PC10nz*/;
                          if (($signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0)>=32'sd6))  begin 
                                   kiwiTESTMAIN4001PC10nz <= 32'h17/*23:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test49_test49_phase0_0_5_V_0 <= $signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0
                                  );

                                   end 
                                   end 
                          
                  32'h12/*18:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h13/*19:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h13/*19:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h14/*20:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h14/*20:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h15/*21:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h15/*21:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h16/*22:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h10/*16:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("data %1d  qfp0=%f", TESTMAIN400_test49_test49_phase0_0_5_V_0, $bitstoreal(TESTMAIN400_test49_test49_phase0_0_5_V_1
                          ));
                           kiwiTESTMAIN4001PC10nz <= 32'h7/*7:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase0_0_5_V_2 <= hpr_dbl2flt1(TESTMAIN400_test49_test49_phase0_0_5_V_1);
                           end 
                          
                  32'h16/*22:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h10/*16:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase0_0_5_V_1 <= ((32'h16/*22:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifMULTIPLIERALUF64_12_RR
                          : ifMULTIPLIERALUF6412RRh10hold);

                           TESTMAIN400_test49_test49_phase0_0_5_V_0 <= $signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0);
                           end 
                          
                  32'h17/*23:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h18/*24:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase1_0_6_V_1 <= 32'sh0;
                           A_FPD_CC_SCALbx10_ARA0[64'd5] <= 64'h4005_ae14_7ae1_47ae;
                           A_FPD_CC_SCALbx10_ARA0[32'sh4] <= 64'h4009_21ca_c083_126f;
                           A_FPD_CC_SCALbx10_ARA0[32'sh3] <= 64'h4009_21ca_c083_126f;
                           A_FPD_CC_SCALbx10_ARA0[32'sh2] <= 64'h4009_21ca_c083_126f;
                           A_FPD_CC_SCALbx10_ARA0[32'sh1] <= 64'h4009_21ca_c083_126f;
                           A_FPD_CC_SCALbx10_ARA0[32'sh0] <= 64'h4009_21ca_c083_126f;
                           end 
                          
                  32'h19/*25:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h1a/*26:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1a/*26:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h1b/*27:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1b/*27:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h1c/*28:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1c/*28:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h1d/*29:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1d/*29:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test49_test49_phase1_0_6_V_1)>=32'sd3))  begin 
                                  $display("phase1: data %1d  is %f", 32'sh0, $bitstoreal(A_FPD_CC_SCALbx10_ARA0[32'sh0]));
                                  $display("phase1: data %1d  is %f", 32'sh1, $bitstoreal(((32'h1b/*27:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh2, $bitstoreal(((32'h1c/*28:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifADDERALUF64_12_RR: ifADDERALUF6412RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh3, $bitstoreal(((32'h1d/*29:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh4, $bitstoreal(((32'h1c/*28:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifADDERALUF64_10_RR: ifADDERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh5, $bitstoreal(A_FPD_CC_SCALbx10_ARA0[32'sh5]));
                                  $display("Kiwi Demo - Test49 phase1 finished.");
                                  $display("Test49 done.");
                                  $finish(32'sd0);
                                   A_FPD_CC_SCALbx10_ARA0[64'd4] <= ((32'h1c/*28:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifADDERALUF64_10_RR
                                  : ifADDERALUF6410RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd3] <= ((32'h1d/*29:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifDIVIDERALUF64_10_RR
                                  : ifDIVIDERALUF6410RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd2] <= ((32'h1c/*28:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifADDERALUF64_12_RR
                                  : ifADDERALUF6412RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd1] <= ((32'h1b/*27:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifMULTIPLIERALUF64_10_RR
                                  : ifMULTIPLIERALUF6410RRh10hold);

                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   kiwiTESTMAIN4001PC10nz <= 32'h23/*35:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase1_0_6_V_1 <= $signed(32'sd1+TESTMAIN400_test49_test49_phase1_0_6_V_1);
                           end 
                          
                  32'h1e/*30:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h1f/*31:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1f/*31:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h20/*32:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h20/*32:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h21/*33:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h21/*33:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz <= 32'h22/*34:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h18/*24:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16) if (($signed(32'sd1+TESTMAIN400_test49_test49_phase1_0_6_V_1
                      )<32'sd3))  kiwiTESTMAIN4001PC10nz <= 32'h1e/*30:kiwiTESTMAIN4001PC10nz*/;
                           else  kiwiTESTMAIN4001PC10nz <= 32'h19/*25:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h22/*34:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test49_test49_phase1_0_6_V_1)<32'sd3))  begin 
                                  $display("phase1: data %1d  is %f", 32'sh0, $bitstoreal(A_FPD_CC_SCALbx10_ARA0[32'sh0]));
                                  $display("phase1: data %1d  is %f", 32'sh1, $bitstoreal(((32'h20/*32:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh2, $bitstoreal(((32'h21/*33:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifADDERALUF64_12_RR: ifADDERALUF6412RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh3, $bitstoreal(((32'h22/*34:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh4, $bitstoreal(((32'h21/*33:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
                                  )? ifADDERALUF64_10_RR: ifADDERALUF6410RRh10hold)));
                                  $display("phase1: data %1d  is %f", 32'sh5, $bitstoreal(A_FPD_CC_SCALbx10_ARA0[32'sh5]));
                                   A_FPD_CC_SCALbx10_ARA0[64'd4] <= ((32'h21/*33:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifADDERALUF64_10_RR
                                  : ifADDERALUF6410RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd3] <= ((32'h22/*34:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifDIVIDERALUF64_10_RR
                                  : ifDIVIDERALUF6410RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd2] <= ((32'h21/*33:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifADDERALUF64_12_RR
                                  : ifADDERALUF6412RRh10hold);

                                   A_FPD_CC_SCALbx10_ARA0[64'd1] <= ((32'h20/*32:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz)? ifMULTIPLIERALUF64_10_RR
                                  : ifMULTIPLIERALUF6410RRh10hold);

                                   end 
                                   kiwiTESTMAIN4001PC10nz <= 32'h18/*24:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test49_test49_phase1_0_6_V_1 <= $signed(32'sd1+TESTMAIN400_test49_test49_phase1_0_6_V_1);
                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN4001PC10nz_pc_export <= 32'd0;
               CVFPCVTFL2F64I3212resulth10hold <= 64'd0;
               CVFPCVTFL2F64I3212resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF6414RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6414RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6414RRh10shot2 <= 32'd0;
               CVFPCVTFL2F32I3210resulth10hold <= 32'd0;
               CVFPCVTFL2F32I3210resulth10shot1 <= 32'd0;
               CVFPCVTFL2I32F3210resulth10hold <= 32'd0;
               CVFPCVTFL2I32F3210resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10hold <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot4 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10hold <= 64'd0;
               CVFPCVTFL2F64I3210resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6412RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot2 <= 32'd0;
               ifDIVIDERALUF6410RRh10hold <= 64'd0;
               ifDIVIDERALUF6410RRh10shot1 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot2 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot3 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot4 <= 32'd0;
               ifADDERALUF6410RRh10hold <= 64'd0;
               ifADDERALUF6410RRh10shot1 <= 32'd0;
               ifADDERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10shot3 <= 32'd0;
               ifADDERALUF6412RRh10hold <= 64'd0;
               ifADDERALUF6412RRh10shot1 <= 32'd0;
               ifADDERALUF6412RRh10shot2 <= 32'd0;
               ifADDERALUF6412RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot0 <= 32'd0;
               ifADDERALUF6412RRh10shot0 <= 32'd0;
               ifADDERALUF6410RRh10shot0 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot0 <= 32'd0;
               CVFPCVTFL2I32F3210resulth10shot0 <= 32'd0;
               CVFPCVTFL2F32I3210resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF6414RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I3212resulth10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if (ifMULTIPLIERALUF6410RRh10shot2)  ifMULTIPLIERALUF6410RRh10hold <= ifMULTIPLIERALUF64_10_RR;
                      if (ifADDERALUF6412RRh10shot3)  ifADDERALUF6412RRh10hold <= ifADDERALUF64_12_RR;
                      if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifDIVIDERALUF6410RRh10shot4)  ifDIVIDERALUF6410RRh10hold <= ifDIVIDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6412RRh10shot2)  ifMULTIPLIERALUF6412RRh10hold <= ifMULTIPLIERALUF64_12_RR;
                      if (CVFPCVTFL2F64I3210resulth10shot1)  CVFPCVTFL2F64I3210resulth10hold <= CVFPCVTFL2F64I32_10_result;
                      if (ifMULTIPLIERALUF3210RRh10shot4)  ifMULTIPLIERALUF3210RRh10hold <= ifMULTIPLIERALUF32_10_RR;
                      if (CVFPCVTFL2I32F3210resulth10shot1)  CVFPCVTFL2I32F3210resulth10hold <= CVFPCVTFL2I32F32_10_result;
                      if (CVFPCVTFL2F32I3210resulth10shot1)  CVFPCVTFL2F32I3210resulth10hold <= CVFPCVTFL2F32I32_10_result;
                      if (ifMULTIPLIERALUF6414RRh10shot2)  ifMULTIPLIERALUF6414RRh10hold <= ifMULTIPLIERALUF64_14_RR;
                      if (CVFPCVTFL2F64I3212resulth10shot1)  CVFPCVTFL2F64I3212resulth10hold <= CVFPCVTFL2F64I32_12_result;
                       kiwiTESTMAIN4001PC10nz_pc_export <= kiwiTESTMAIN4001PC10nz;
                   CVFPCVTFL2F64I3212resulth10shot1 <= CVFPCVTFL2F64I3212resulth10shot0;
                   ifMULTIPLIERALUF6414RRh10shot1 <= ifMULTIPLIERALUF6414RRh10shot0;
                   ifMULTIPLIERALUF6414RRh10shot2 <= ifMULTIPLIERALUF6414RRh10shot1;
                   CVFPCVTFL2F32I3210resulth10shot1 <= CVFPCVTFL2F32I3210resulth10shot0;
                   CVFPCVTFL2I32F3210resulth10shot1 <= CVFPCVTFL2I32F3210resulth10shot0;
                   ifMULTIPLIERALUF3210RRh10shot1 <= ifMULTIPLIERALUF3210RRh10shot0;
                   ifMULTIPLIERALUF3210RRh10shot2 <= ifMULTIPLIERALUF3210RRh10shot1;
                   ifMULTIPLIERALUF3210RRh10shot3 <= ifMULTIPLIERALUF3210RRh10shot2;
                   ifMULTIPLIERALUF3210RRh10shot4 <= ifMULTIPLIERALUF3210RRh10shot3;
                   CVFPCVTFL2F64I3210resulth10shot1 <= CVFPCVTFL2F64I3210resulth10shot0;
                   ifMULTIPLIERALUF6412RRh10shot1 <= ifMULTIPLIERALUF6412RRh10shot0;
                   ifMULTIPLIERALUF6412RRh10shot2 <= ifMULTIPLIERALUF6412RRh10shot1;
                   ifDIVIDERALUF6410RRh10shot1 <= ifDIVIDERALUF6410RRh10shot0;
                   ifDIVIDERALUF6410RRh10shot2 <= ifDIVIDERALUF6410RRh10shot1;
                   ifDIVIDERALUF6410RRh10shot3 <= ifDIVIDERALUF6410RRh10shot2;
                   ifDIVIDERALUF6410RRh10shot4 <= ifDIVIDERALUF6410RRh10shot3;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifADDERALUF6412RRh10shot1 <= ifADDERALUF6412RRh10shot0;
                   ifADDERALUF6412RRh10shot2 <= ifADDERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot3 <= ifADDERALUF6412RRh10shot2;
                   ifMULTIPLIERALUF6410RRh10shot1 <= ifMULTIPLIERALUF6410RRh10shot0;
                   ifMULTIPLIERALUF6410RRh10shot2 <= ifMULTIPLIERALUF6410RRh10shot1;
                   ifMULTIPLIERALUF6410RRh10shot0 <= (32'h18/*24:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   ifADDERALUF6412RRh10shot0 <= (32'h18/*24:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   ifADDERALUF6410RRh10shot0 <= (32'h18/*24:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   ifDIVIDERALUF6410RRh10shot0 <= (32'h18/*24:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   ifMULTIPLIERALUF6412RRh10shot0 <= (32'h13/*19:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   CVFPCVTFL2F64I3210resulth10shot0 <= ($signed(32'sd1+TESTMAIN400_test49_test49_phase0_0_5_V_0)<32'sd6) && (32'h11/*17:kiwiTESTMAIN4001PC10nz*/==
                  kiwiTESTMAIN4001PC10nz);

                   ifMULTIPLIERALUF3210RRh10shot0 <= (32'h9/*9:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   CVFPCVTFL2I32F3210resulth10shot0 <= (32'h7/*7:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   CVFPCVTFL2F32I3210resulth10shot0 <= (32'h7/*7:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   ifMULTIPLIERALUF6414RRh10shot0 <= (32'h3/*3:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   CVFPCVTFL2F64I3212resulth10shot0 <= (32'h1/*1:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400_1/1.0


       end 
      

  CV_FP_FL5_DIVIDER_DP ifDIVIDERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifDIVIDERALUF64_10_RR),
        .NN(ifDIVIDERALUF64_10_NN
),
        .DD(ifDIVIDERALUF64_10_DD),
        .FAIL(ifDIVIDERALUF64_10_FAIL));
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
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_12_RR),
        .XX(ifMULTIPLIERALUF64_12_XX
),
        .YY(ifMULTIPLIERALUF64_12_YY),
        .FAIL(ifMULTIPLIERALUF64_12_FAIL));
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
  CV_FP_CVT_FL2_I32_F32 CVFPCVTFL2I32F32_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2I32F32_10_result),
        .arg(CVFPCVTFL2I32F32_10_arg
),
        .FAIL(CVFPCVTFL2I32F32_10_FAIL));
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_14(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_14_RR),
        .XX(ifMULTIPLIERALUF64_14_XX
),
        .YY(ifMULTIPLIERALUF64_14_YY),
        .FAIL(ifMULTIPLIERALUF64_14_FAIL));
  CV_FP_CVT_FL2_F64_I32 CVFPCVTFL2F64I32_12(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F64I32_12_result),
        .arg(CVFPCVTFL2F64I32_12_arg
),
        .FAIL(CVFPCVTFL2F64I32_12_FAIL));
// Structural Resource (FU) inventory for DUT:// 1 vectors of width 6
// 36 vectors of width 1
// 21 vectors of width 64
// 15 vectors of width 32
// 6 array locations of width 64
// Total state bits in module = 2250 bits.
// 619 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL5_DIVIDER_DP count=1
//   cell CV_FP_FL4_ADDER_DP count=2
//   cell CV_FP_FL3_MULTIPLIER_DP count=3
//   cell CV_FP_CVT_FL2_F64_I32 count=2
//   cell CV_FP_FL5_MULTIPLIER_SP count=1
//   cell CV_FP_CVT_FL2_F32_I32 count=1
//   cell CV_FP_CVT_FL2_I32_F32 count=1
// Total number of leaf cells = 11
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:24
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test49.exe -sim=1800 -diosim-vcd=test49.vcd -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test49.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test49 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test49.Main | Main10 | false |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*

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
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class test49  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test49..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test49..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test49  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test49.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test49.Main
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
//   srcfile=test49.exe
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
//PC codings points for kiwiTESTMAIN4001PC10 
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                     | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN4001PC10"   | 819 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN4001PC10"   | 818 | 1       | hwm=0.5.0   | 6    |        | 2     | 6   | 16   |
//| XU32'4:"4:kiwiTESTMAIN4001PC10"   | 816 | 7       | hwm=0.7.0   | 14   |        | 8     | 14  | 15   |
//| XU32'8:"8:kiwiTESTMAIN4001PC10"   | 815 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 17   |
//| XU32'2:"2:kiwiTESTMAIN4001PC10"   | 817 | 16      | hwm=0.0.0   | 16   |        | -     | -   | 7    |
//| XU32'16:"16:kiwiTESTMAIN4001PC10" | 813 | 17      | hwm=0.5.0   | 22   |        | 18    | 22  | 16   |
//| XU32'16:"16:kiwiTESTMAIN4001PC10" | 814 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 23   |
//| XU32'32:"32:kiwiTESTMAIN4001PC10" | 812 | 23      | hwm=0.0.0   | 23   |        | -     | -   | 24   |
//| XU32'64:"64:kiwiTESTMAIN4001PC10" | 810 | 24      | hwm=0.5.0   | 34   |        | 30    | 34  | 24   |
//| XU32'64:"64:kiwiTESTMAIN4001PC10" | 811 | 24      | hwm=0.5.0   | 29   |        | 25    | 29  | -    |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                |
//| F0   | E819 | R0 DATA |                                                                                                                |
//| F0+E | E819 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_0write(S32'0) TESTMAIN400.test49.test49_phase0.0.5.V_1write(??64'0) TE\ |
//|      |      |         | STMAIN400.test49.test49_phase0.0.5.V_2write(??32'0) TESTMAIN400.test49.test49_phase0.0.5.V_3write(??32'0) TES\ |
//|      |      |         | TMAIN400.test49.test49_phase0.0.5.V_4write(S32'0) TESTMAIN400.test49.test49_phase1.0.6.V_1write(S32'0) test49\ |
//|      |      |         | .volxwrite(S32'100)  PLI:Kiwi Demo - Test49 s...                                                               |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                     |
//| F1   | E818 | R0 DATA | CVFPCVTFL2F64I32_12_hpr_dbl_from_int32(test49.volx)                                                                                 |
//| F2   | E818 | R1 DATA |                                                                                                                                     |
//| F3   | E818 | R2 DATA | ifMULTIPLIERALUF64_14_compute(CVT(C64f)(test49.volx), 3330.2)                                                                       |
//| F4   | E818 | R3 DATA |                                                                                                                                     |
//| F5   | E818 | R4 DATA |                                                                                                                                     |
//| F6   | E818 | R5 DATA |                                                                                                                                     |
//| F6+E | E818 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_0write(S32'0) TESTMAIN400.test49.test49_phase0.0.5.V_1write(E1)  PLI:Kiwi Demo - Test49 p... |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*
//| F7    | -    | R0 CTRL |                                                                                                     |
//| F7    | E816 | R0 DATA | CVFPCVTFL2F32I32_10_hpr_flt_from_int32(E2) CVFPCVTFL2I32F32_10_hpr_flt2int32(E3)                    |
//| F8    | E816 | R1 DATA |                                                                                                     |
//| F9    | E816 | R2 DATA | ifMULTIPLIERALUF32_10_compute(7.12345f, E4)                                                         |
//| F10   | E816 | R3 DATA |                                                                                                     |
//| F11   | E816 | R4 DATA |                                                                                                     |
//| F12   | E816 | R5 DATA |                                                                                                     |
//| F13   | E816 | R6 DATA |                                                                                                     |
//| F14   | E816 | R7 DATA |                                                                                                     |
//| F14+E | E816 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_3write(E5) TESTMAIN400.test49.test49_phase0.0.5.V_4write(E6) |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F15   | -    | R0 CTRL |                              |
//| F15   | E815 | R0 DATA |                              |
//| F15+E | E815 | W0 DATA |  PLI:                  qf... |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                    |
//*-------+------+---------+-------------------------------------------------------------------------*
//| F16   | -    | R0 CTRL |                                                                         |
//| F16   | E817 | R0 DATA |                                                                         |
//| F16+E | E817 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_2write(E7)  PLI:data %d  qfp0=%f |
//*-------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                 |
//*-------+------+---------+------------------------------------------------------------------------------------------------------*
//| F17   | -    | R0 CTRL |                                                                                                      |
//| F17   | E814 | R0 DATA |                                                                                                      |
//| F17+E | E814 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_0write(E8)  PLI:Kiwi Demo - Test49 p...                       |
//| F17   | E813 | R0 DATA | CVFPCVTFL2F64I32_10_hpr_dbl_from_int32(E9)                                                           |
//| F18   | E813 | R1 DATA |                                                                                                      |
//| F19   | E813 | R2 DATA | ifMULTIPLIERALUF64_12_compute(3330.2, E10)                                                           |
//| F20   | E813 | R3 DATA |                                                                                                      |
//| F21   | E813 | R4 DATA |                                                                                                      |
//| F22   | E813 | R5 DATA |                                                                                                      |
//| F22+E | E813 | W0 DATA | TESTMAIN400.test49.test49_phase0.0.5.V_0write(E8) TESTMAIN400.test49.test49_phase0.0.5.V_1write(E11) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32:"32:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32:"32:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F23   | -    | R0 CTRL |                                                      |
//| F23   | E812 | R0 DATA |                                                      |
//| F23+E | E812 | W0 DATA | TESTMAIN400.test49.test49_phase1.0.6.V_1write(S32'0) |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'64:"64:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'64:"64:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                  |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F24   | -    | R0 CTRL |                                                                                                                                                       |
//| F24   | E811 | R0 DATA | ifDIVIDERALUF64_10_compute(E12, 100) ifADDERALUF64_10_compute(100, E13) ifADDERALUF64_12_compute(-100.0, E14) ifMULTIPLIERALUF64_10_compute(100, E15) |
//| F25   | E811 | R1 DATA |                                                                                                                                                       |
//| F26   | E811 | R2 DATA |                                                                                                                                                       |
//| F27   | E811 | R3 DATA |                                                                                                                                                       |
//| F28   | E811 | R4 DATA |                                                                                                                                                       |
//| F29   | E811 | R5 DATA |                                                                                                                                                       |
//| F29+E | E811 | W0 DATA | TESTMAIN400.test49.test49_phase1.0.6.V_1write(E16)  PLI:GSAI:hpr_sysexit  PLI:Test49 done.  PLI:Kiwi Demo - Test49 p...  PLI:phase1: data %d  is ...  |
//| F24   | E810 | R0 DATA | ifDIVIDERALUF64_10_compute(E12, 100) ifADDERALUF64_10_compute(100, E13) ifADDERALUF64_12_compute(-100.0, E14) ifMULTIPLIERALUF64_10_compute(100, E15) |
//| F30   | E810 | R1 DATA |                                                                                                                                                       |
//| F31   | E810 | R2 DATA |                                                                                                                                                       |
//| F32   | E810 | R3 DATA |                                                                                                                                                       |
//| F33   | E810 | R4 DATA |                                                                                                                                                       |
//| F34   | E810 | R5 DATA |                                                                                                                                                       |
//| F34+E | E810 | W0 DATA | TESTMAIN400.test49.test49_phase1.0.6.V_1write(E16)  PLI:phase1: data %d  is ...                                                                       |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= CVT(C64f)((CVT(C64f)(test49.volx))*3330.2)
//
//
//  E2 =.= TESTMAIN400.test49.test49_phase0.0.5.V_0
//
//
//  E3 =.= TESTMAIN400.test49.test49_phase0.0.5.V_2
//
//
//  E4 =.= CVT(Cf)(TESTMAIN400.test49.test49_phase0.0.5.V_0)
//
//
//  E5 =.= Cf(7.12345f*(CVT(Cf)(TESTMAIN400.test49.test49_phase0.0.5.V_0)))
//
//
//  E6 =.= CVT(C)(TESTMAIN400.test49.test49_phase0.0.5.V_2)
//
//
//  E7 =.= CVT(Cf)(TESTMAIN400.test49.test49_phase0.0.5.V_1)
//
//
//  E8 =.= C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0)
//
//
//  E9 =.= test49.volx+(C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0))
//
//
//  E10 =.= CVT(C64f)(test49.volx+(C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0)))
//
//
//  E11 =.= CVT(C64f)(3330.2*(CVT(C64f)(test49.volx+(C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0)))))
//
//
//  E12 =.= @_FPD/CC/SCALbx10_ARA0[3]
//
//
//  E13 =.= @_FPD/CC/SCALbx10_ARA0[4]
//
//
//  E14 =.= @_FPD/CC/SCALbx10_ARA0[2]
//
//
//  E15 =.= @_FPD/CC/SCALbx10_ARA0[1]
//
//
//  E16 =.= C(1+TESTMAIN400.test49.test49_phase1.0.6.V_1)
//
//
//  E17 =.= (C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0))>=6
//
//
//  E18 =.= (C(1+TESTMAIN400.test49.test49_phase0.0.5.V_0))<6
//
//
//  E19 =.= (C(1+TESTMAIN400.test49.test49_phase1.0.6.V_1))>=3
//
//
//  E20 =.= (C(1+TESTMAIN400.test49.test49_phase1.0.6.V_1))<3
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test49 to test49

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 6
//
//36 vectors of width 1
//
//21 vectors of width 64
//
//15 vectors of width 32
//
//6 array locations of width 64
//
//Total state bits in module = 2250 bits.
//
//619 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test49..cctor uid=cctor10 has 5 CIL instructions in 1 basic blocks
//Thread test49.Main uid=Main10 has 58 CIL instructions in 19 basic blocks
//Thread mpc10 has 8 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN4001PC10 with 35 minor control states
// eof (HPR L/S Verilog)
