// CBG Orangepath HPR L/S System

// Verilog output file generated at 10/01/2019 14:18:29
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.j : 5th January 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe dfsin-floatnat.exe -vnl=dfsin-floatnat.v ../softfloat.dll
`timescale 1ns/1ns


module dfsin_floatnat(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTMAINIDL4001PC10nz_pc_export,
    
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
    output reg [7:0] bondout1_LANES1,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);

function hpr_fp_dltd0; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd0_a, hpr_fp_dltd0_b;
  hpr_fp_dltd0 = ((hpr_fp_dltd0_a[63] && !hpr_fp_dltd0_b[63]) ? 1: (!hpr_fp_dltd0_a[63] && hpr_fp_dltd0_b[63])  ? 0 : (hpr_fp_dltd0_a[63]^(hpr_fp_dltd0_a[62:0]<hpr_fp_dltd0_b[62:0]))); 
   endfunction

// abstractionName=kiwicmainnets10
  reg/*fp*/  [63:0] TMAIN_IDL400_System_Math_Abs_1_29_SPILL_256;
  reg/*fp*/  [63:0] TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_3;
  reg signed [31:0] TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2;
  reg/*fp*/  [63:0] TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1;
  reg/*fp*/  [63:0] TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0;
  reg [63:0] TMAIN_IDL400_dfsin_bm_main_0_3_V_2;
  reg signed [31:0] TMAIN_IDL400_dfsin_bm_main_0_3_V_1;
  reg signed [31:0] TMAIN_IDL400_dfsin_bm_main_0_3_V_0;
  reg signed [31:0] TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_257;
  reg signed [31:0] TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_258;
  wire [63:0] hpr_tnow;
  reg signed [31:0] dfsin_gloops;
// abstractionName=repack-newnets
  reg [63:0] A_64_US_CC_SCALbx12_ARA0[35:0];
  reg [63:0] A_64_US_CC_SCALbx14_ARB0[35:0];
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_10_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_10_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_10_YY;
  wire ifADDERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_DIVIDER_DP
  wire/*fp*/  [63:0] ifDIVIDERALUF64_10_RR;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_10_NN;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_10_DD;
  wire ifDIVIDERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F64I32_10
  wire/*fp*/  [63:0] CVFPCVTFL2F64I32_10_result;
  reg signed [31:0] CVFPCVTFL2F64I32_10_arg;
  wire CVFPCVTFL2F64I32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_INT_FL3_MULTIPLIER_S
  wire signed [31:0] isMULTIPLIERALUS32_10_RR;
  reg signed [31:0] isMULTIPLIERALUS32_10_XX;
  reg signed [31:0] isMULTIPLIERALUS32_10_YY;
  wire isMULTIPLIERALUS32_10_FAIL;
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
// abstractionName=res2-morenets
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
  reg/*fp*/  [63:0] CVFPCVTFL2F64I3210resulth10hold;
  reg CVFPCVTFL2F64I3210resulth10shot0;
  reg CVFPCVTFL2F64I3210resulth10shot1;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6412RRh10hold;
  reg ifMULTIPLIERALUF6412RRh10shot0;
  reg ifMULTIPLIERALUF6412RRh10shot1;
  reg ifMULTIPLIERALUF6412RRh10shot2;
  reg [4:0] kiwiTMAINIDL4001PC10nz;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=share-nets pi_name=shareAnets10
  reg [63:0] hprpin501365x10;
  wire [63:0] hprpin501498x10;
  reg/*fp*/  [63:0] hprpin502469x10;
  wire/*fp*/  [63:0] hprpin502954x10;
 always   @(* )  begin 
       CVFPCVTFL2F64I32_10_arg = 32'sd0;
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
       ifADDERALUF64_10_XX = 64'd0;
       ifADDERALUF64_10_YY = 64'd0;
       ifDIVIDERALUF64_10_NN = 64'd0;
       ifDIVIDERALUF64_10_DD = 64'd0;
       ifMULTIPLIERALUF64_10_XX = 64'd0;
       ifMULTIPLIERALUF64_10_YY = 64'd0;
       ifMULTIPLIERALUF64_12_XX = 64'd0;
       ifMULTIPLIERALUF64_12_YY = 64'd0;
       isMULTIPLIERALUS32_10_XX = 32'sd0;
       isMULTIPLIERALUS32_10_YY = 32'sd0;
       ifMULTIPLIERALUF64_12_YY = 32'sd0;
       ifMULTIPLIERALUF64_12_XX = 32'sd0;
       isMULTIPLIERALUS32_10_YY = 32'sd0;
       isMULTIPLIERALUS32_10_XX = 32'sd0;
       CVFPCVTFL2F64I32_10_arg = 32'sd0;
       ifDIVIDERALUF64_10_DD = 32'sd0;
       ifDIVIDERALUF64_10_NN = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;
      if (hpr_int_run_enable_DDX18) 
          case (kiwiTMAINIDL4001PC10nz)
              32'h1/*1:kiwiTMAINIDL4001PC10nz*/:  begin 
                   ifMULTIPLIERALUF64_12_YY = 64'sh8000_0000_0000_0000^hprpin502469x10;
                   ifMULTIPLIERALUF64_12_XX = hprpin502469x10;
                   end 
                  
              32'h5/*5:kiwiTMAINIDL4001PC10nz*/:  begin 
                   isMULTIPLIERALUS32_10_YY = 32'sd1+32'sd2*TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2;
                   isMULTIPLIERALUS32_10_XX = TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2;
                   ifMULTIPLIERALUF64_12_YY = TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1;
                   ifMULTIPLIERALUF64_12_XX = TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_3;
                   end 
                  
              32'h8/*8:kiwiTMAINIDL4001PC10nz*/:  CVFPCVTFL2F64I32_10_arg = 32'sd2*TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2*(32'sd1+32'sd2
              *TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2);


              32'ha/*10:kiwiTMAINIDL4001PC10nz*/:  begin 
                   ifDIVIDERALUF64_10_DD = ((32'ha/*10:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? CVFPCVTFL2F64I32_10_result: CVFPCVTFL2F64I3210resulth10hold
                  );

                   ifDIVIDERALUF64_10_NN = ((32'h8/*8:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifMULTIPLIERALUF64_12_RR: ifMULTIPLIERALUF6412RRh10hold
                  );

                   end 
                  
              32'hf/*15:kiwiTMAINIDL4001PC10nz*/:  begin 
                   ifADDERALUF64_10_YY = ((32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold
                  );

                   ifADDERALUF64_10_XX = TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0;
                   end 
                  endcase
           hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogdfsin-floatnat/1.0
      if (reset)  begin 
               kiwiTMAINIDL4001PC10nz_pc_export <= 32'd0;
               dfsin_gloops <= 32'd0;
               done <= 32'd0;
               TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_258 <= 32'd0;
               TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_257 <= 32'd0;
               TMAIN_IDL400_dfsin_bm_main_0_3_V_2 <= 64'd0;
               TMAIN_IDL400_System_Math_Abs_1_29_SPILL_256 <= 64'd0;
               TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0 <= 64'd0;
               TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2 <= 32'd0;
               TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_3 <= 64'd0;
               TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1 <= 64'd0;
               TMAIN_IDL400_dfsin_bm_main_0_3_V_0 <= 32'd0;
               TMAIN_IDL400_dfsin_bm_main_0_3_V_1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6412RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot2 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10hold <= 64'd0;
               CVFPCVTFL2F64I3210resulth10shot1 <= 32'd0;
               ifDIVIDERALUF6410RRh10hold <= 64'd0;
               ifDIVIDERALUF6410RRh10shot1 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot2 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot3 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot4 <= 32'd0;
               ifADDERALUF6410RRh10hold <= 64'd0;
               ifADDERALUF6410RRh10shot1 <= 32'd0;
               ifADDERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10shot3 <= 32'd0;
               ifADDERALUF6410RRh10shot0 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I3210resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot0 <= 32'd0;
               hpr_abend_syndrome <= 32'd255;
               kiwiTMAINIDL4001PC10nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  
                  case (kiwiTMAINIDL4001PC10nz)
                      32'h0/*0:kiwiTMAINIDL4001PC10nz*/:  begin 
                          $display("dfsin: Testbench start");
                           dfsin_gloops <= 32'sh0;
                           done <= 32'h0;
                           TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_258 <= 32'sh0;
                           TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_257 <= 32'sh0;
                           TMAIN_IDL400_dfsin_bm_main_0_3_V_2 <= 64'h0;
                           TMAIN_IDL400_System_Math_Abs_1_29_SPILL_256 <= 64'h0;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0 <= 64'h0;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2 <= 32'sh0;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_3 <= 64'h0;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1 <= 64'h0;
                           TMAIN_IDL400_dfsin_bm_main_0_3_V_0 <= 32'sh0;
                           TMAIN_IDL400_dfsin_bm_main_0_3_V_1 <= 32'sh1;
                           kiwiTMAINIDL4001PC10nz <= 32'h17/*23:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h4/*4:kiwiTMAINIDL4001PC10nz*/:  begin 
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0 <= hprpin502469x10;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2 <= 32'sh1;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_3 <= ((32'h4/*4:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifMULTIPLIERALUF64_12_RR
                          : ifMULTIPLIERALUF6412RRh10hold);

                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1 <= hprpin502469x10;
                           kiwiTMAINIDL4001PC10nz <= 32'h5/*5:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h13/*19:kiwiTMAINIDL4001PC10nz*/:  begin 
                           dfsin_gloops <= $signed(32'sd1+dfsin_gloops);
                           TMAIN_IDL400_System_Math_Abs_1_29_SPILL_256 <= hprpin502954x10;
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0 <= ((32'h13/*19:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifADDERALUF64_10_RR
                          : ifADDERALUF6410RRh10hold);

                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2 <= $signed(32'sd1+TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_2);
                           TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_1 <= ((32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifDIVIDERALUF64_10_RR
                          : ifDIVIDERALUF6410RRh10hold);

                           kiwiTMAINIDL4001PC10nz <= 32'h14/*20:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h14/*20:kiwiTMAINIDL4001PC10nz*/: if (hpr_fp_dltd0(64'h3ee4_f8b5_88e3_68f1, TMAIN_IDL400_System_Math_Abs_1_29_SPILL_256
                      ))  kiwiTMAINIDL4001PC10nz <= 32'h5/*5:kiwiTMAINIDL4001PC10nz*/;
                           else  begin 
                               TMAIN_IDL400_dfsin_bm_main_0_3_V_2 <= hprpin501365x10;
                               kiwiTMAINIDL4001PC10nz <= 32'h15/*21:kiwiTMAINIDL4001PC10nz*/;
                               end 
                              
                      32'h15/*21:kiwiTMAINIDL4001PC10nz*/:  begin 
                           TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_258 <= ((TMAIN_IDL400_dfsin_bm_main_0_3_V_2==A_64_US_CC_SCALbx12_ARA0
                          [TMAIN_IDL400_dfsin_bm_main_0_3_V_1])? 32'sh1: 32'sh0);

                           TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_257 <= TMAIN_IDL400_dfsin_bm_main_0_3_V_0;
                           kiwiTMAINIDL4001PC10nz <= 32'h16/*22:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h16/*22:kiwiTMAINIDL4001PC10nz*/:  begin 
                          $display("Test: input=%1H expected=%1H output=%1H ", hprpin501498x10, A_64_US_CC_SCALbx12_ARA0[TMAIN_IDL400_dfsin_bm_main_0_3_V_1
                          ], TMAIN_IDL400_dfsin_bm_main_0_3_V_2);
                          if ((32'h0/*0:USA14*/!=(TMAIN_IDL400_dfsin_bm_main_0_3_V_2^A_64_US_CC_SCALbx12_ARA0[TMAIN_IDL400_dfsin_bm_main_0_3_V_1
                          ]))) $display("   hamming error=%1H", TMAIN_IDL400_dfsin_bm_main_0_3_V_2^A_64_US_CC_SCALbx12_ARA0[TMAIN_IDL400_dfsin_bm_main_0_3_V_1
                              ]);
                               TMAIN_IDL400_dfsin_bm_main_0_3_V_0 <= $signed(TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_258+TMAIN_IDL400_dfsin_bm_main_0_3_SPILL_257
                          );

                           kiwiTMAINIDL4001PC10nz <= 32'h18/*24:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h18/*24:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if ((32'sd36==TMAIN_IDL400_dfsin_bm_main_0_3_V_0) && ($signed(32'sd1+TMAIN_IDL400_dfsin_bm_main_0_3_V_1)>=32'sd36
                          )) $display("Result: %1d/%1d", TMAIN_IDL400_dfsin_bm_main_0_3_V_0, 32'sd36);
                              if ((32'sd36==TMAIN_IDL400_dfsin_bm_main_0_3_V_0) && ($signed(32'sd1+TMAIN_IDL400_dfsin_bm_main_0_3_V_1
                          )>=32'sd36)) $display("RESULT: PASS");
                              if (($signed(32'sd1+TMAIN_IDL400_dfsin_bm_main_0_3_V_1)>=32'sd36))  begin 
                                  $display("Result: %1d/%1d", TMAIN_IDL400_dfsin_bm_main_0_3_V_0, 32'sd36);
                                  if ((32'sd36!=TMAIN_IDL400_dfsin_bm_main_0_3_V_0)) $display("RESULT: FAIL");
                                       end 
                                  if (($signed(32'sd1+TMAIN_IDL400_dfsin_bm_main_0_3_V_1)<32'sd36))  kiwiTMAINIDL4001PC10nz <= 32'h17
                              /*23:kiwiTMAINIDL4001PC10nz*/;

                               else  kiwiTMAINIDL4001PC10nz <= 32'h19/*25:kiwiTMAINIDL4001PC10nz*/;
                           TMAIN_IDL400_dfsin_bm_main_0_3_V_1 <= $signed(32'sd1+TMAIN_IDL400_dfsin_bm_main_0_3_V_1);
                           end 
                          
                      32'h19/*25:kiwiTMAINIDL4001PC10nz*/:  begin 
                          $display("dfsin: Testbench finished");
                          $display("gloops=%1d", dfsin_gloops);
                           kiwiTMAINIDL4001PC10nz <= 32'h1a/*26:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h1a/*26:kiwiTMAINIDL4001PC10nz*/:  begin 
                           done <= 32'h1;
                           kiwiTMAINIDL4001PC10nz <= 32'h1b/*27:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'h1b/*27:kiwiTMAINIDL4001PC10nz*/:  begin 
                          $display("Driven Done at %1d", $time);
                           hpr_abend_syndrome <= 32'sd0;
                           kiwiTMAINIDL4001PC10nz <= 32'h1c/*28:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          endcase
                  if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifDIVIDERALUF6410RRh10shot4)  ifDIVIDERALUF6410RRh10hold <= ifDIVIDERALUF64_10_RR;
                      if (CVFPCVTFL2F64I3210resulth10shot1)  CVFPCVTFL2F64I3210resulth10hold <= CVFPCVTFL2F64I32_10_result;
                      if (ifMULTIPLIERALUF6412RRh10shot2)  ifMULTIPLIERALUF6412RRh10hold <= ifMULTIPLIERALUF64_12_RR;
                      
                  case (kiwiTMAINIDL4001PC10nz)
                      32'h1/*1:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h2/*2:kiwiTMAINIDL4001PC10nz*/;

                      32'h2/*2:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h3/*3:kiwiTMAINIDL4001PC10nz*/;

                      32'h3/*3:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h4/*4:kiwiTMAINIDL4001PC10nz*/;

                      32'h5/*5:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h6/*6:kiwiTMAINIDL4001PC10nz*/;

                      32'h6/*6:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h7/*7:kiwiTMAINIDL4001PC10nz*/;

                      32'h7/*7:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h8/*8:kiwiTMAINIDL4001PC10nz*/;

                      32'h8/*8:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h9/*9:kiwiTMAINIDL4001PC10nz*/;

                      32'h9/*9:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'ha/*10:kiwiTMAINIDL4001PC10nz*/;

                      32'ha/*10:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'hb/*11:kiwiTMAINIDL4001PC10nz*/;

                      32'hb/*11:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'hc/*12:kiwiTMAINIDL4001PC10nz*/;

                      32'hc/*12:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'hd/*13:kiwiTMAINIDL4001PC10nz*/;

                      32'hd/*13:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'he/*14:kiwiTMAINIDL4001PC10nz*/;

                      32'he/*14:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'hf/*15:kiwiTMAINIDL4001PC10nz*/;

                      32'hf/*15:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h10/*16:kiwiTMAINIDL4001PC10nz*/;

                      32'h10/*16:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h11/*17:kiwiTMAINIDL4001PC10nz*/;

                      32'h11/*17:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h12/*18:kiwiTMAINIDL4001PC10nz*/;

                      32'h12/*18:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h13/*19:kiwiTMAINIDL4001PC10nz*/;

                      32'h17/*23:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h1/*1:kiwiTMAINIDL4001PC10nz*/;
                  endcase
                   kiwiTMAINIDL4001PC10nz_pc_export <= kiwiTMAINIDL4001PC10nz;
                   ifMULTIPLIERALUF6412RRh10shot1 <= ifMULTIPLIERALUF6412RRh10shot0;
                   ifMULTIPLIERALUF6412RRh10shot2 <= ifMULTIPLIERALUF6412RRh10shot1;
                   CVFPCVTFL2F64I3210resulth10shot1 <= CVFPCVTFL2F64I3210resulth10shot0;
                   ifDIVIDERALUF6410RRh10shot1 <= ifDIVIDERALUF6410RRh10shot0;
                   ifDIVIDERALUF6410RRh10shot2 <= ifDIVIDERALUF6410RRh10shot1;
                   ifDIVIDERALUF6410RRh10shot3 <= ifDIVIDERALUF6410RRh10shot2;
                   ifDIVIDERALUF6410RRh10shot4 <= ifDIVIDERALUF6410RRh10shot3;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifADDERALUF6410RRh10shot0 <= (32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz);
                   ifDIVIDERALUF6410RRh10shot0 <= (32'ha/*10:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz);
                   CVFPCVTFL2F64I3210resulth10shot0 <= (32'h8/*8:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz);
                   ifMULTIPLIERALUF6412RRh10shot0 <= (32'h1/*1:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h5/*5:kiwiTMAINIDL4001PC10nz*/==
                  kiwiTMAINIDL4001PC10nz);

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogdfsin-floatnat/1.0


       end 
      

  CV_FP_FL4_ADDER_DP ifADDERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_10_RR),
        .XX(ifADDERALUF64_10_XX
),
        .YY(ifADDERALUF64_10_YY),
        .FAIL(ifADDERALUF64_10_FAIL));
  CV_FP_FL5_DIVIDER_DP ifDIVIDERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifDIVIDERALUF64_10_RR),
        .NN(ifDIVIDERALUF64_10_NN
),
        .DD(ifDIVIDERALUF64_10_DD),
        .FAIL(ifDIVIDERALUF64_10_FAIL));
  CV_FP_CVT_FL2_F64_I32 CVFPCVTFL2F64I32_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F64I32_10_result),
        .arg(CVFPCVTFL2F64I32_10_arg
),
        .FAIL(CVFPCVTFL2F64I32_10_FAIL));
  CV_INT_FL3_MULTIPLIER_S #(.RWIDTH(32'sd32), .A0WIDTH(32'sd32), .A1WIDTH(32'sd32), .trace_me(32'sd0)) isMULTIPLIERALUS32_10(
        .clk(clk
),
        .reset(reset),
        .RR(isMULTIPLIERALUS32_10_RR),
        .XX(isMULTIPLIERALUS32_10_XX),
        .YY(isMULTIPLIERALUS32_10_YY
),
        .FAIL(isMULTIPLIERALUS32_10_FAIL));

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
always @(*) hprpin501365x10 = TMAIN_IDL400_dfsin_sin_floatnat_3_6_V_0;

assign hprpin501498x10 = A_64_US_CC_SCALbx14_ARB0[TMAIN_IDL400_dfsin_bm_main_0_3_V_1];

always @(*) hprpin502469x10 = hprpin501498x10;

assign hprpin502954x10 = (((32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? hpr_fp_dltd0(ifDIVIDERALUF64_10_RR, 32'sd0): hpr_fp_dltd0(ifDIVIDERALUF6410RRh10hold
, 32'sd0))? 64'sh8000_0000_0000_0000^((32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold
): ((32'hf/*15:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold));

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
      

// Structural Resource (FU) inventory for dfsin_floatnat:// 19 vectors of width 64
// 15 vectors of width 1
// 1 vectors of width 5
// 9 vectors of width 32
// 72 array locations of width 64
// Total state bits in module = 6132 bits.
// 550 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL4_ADDER_DP count=1
//   cell CV_FP_FL5_DIVIDER_DP count=1
//   cell CV_FP_CVT_FL2_F64_I32 count=1
//   cell CV_INT_FL3_MULTIPLIER_S count=1
//   cell CV_FP_FL3_MULTIPLIER_DP count=2
// Total number of leaf cells = 6
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.j : 5th January 2019
//10/01/2019 14:18:25
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe dfsin-floatnat.exe -vnl=dfsin-floatnat.v ../softfloat.dll


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-------+---------+------------------------------------------------------------------------------------------------------------+---------------+--------------+----------+-------*
//| Class | Style   | Dir Style                                                                                                  | Timing Target | Method       | UID      | Skip  |
//*-------+---------+------------------------------------------------------------------------------------------------------------+---------------+--------------+----------+-------*
//| dfsin | MM_root | DS_normal self-start/directorate-startmode, auto-restart/directorate-endmode, enable/directorate-pc-export |               | dfsin.HWMain | HWMain10 | false |
//*-------+---------+------------------------------------------------------------------------------------------------------------+---------------+--------------+----------+-------*

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
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor16 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor16 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class softfloat  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=softfloat..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=softfloat..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class dfsin  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=dfsin..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=dfsin..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class dfsin  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=dfsin.HWMain uid=HWMain10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=HWMain10 full_idl=dfsin.HWMain
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
//   kiwife-directorate-endmode=auto-restart
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
//   kiwife-dynpoly=enable
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
//   ?>?=srcfile, dfsin-floatnat.exe, ../softfloat.dll
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
//*------------------------+---------+---------------------------------------------------------------------------------*
//| Key                    | Value   | Description                                                                     |
//*------------------------+---------+---------------------------------------------------------------------------------*
//| int-flr-mul            | 1000    |                                                                                 |
//| max-no-fp-addsubs      | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls         | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls        | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs         | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs        | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors     | 8       | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing   | 8       | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div           | 5       |                                                                                 |
//| fp-fl-dp-add           | 4       |                                                                                 |
//| fp-fl-dp-mul           | 3       |                                                                                 |
//| fp-fl-sp-div           | 15      |                                                                                 |
//| fp-fl-sp-add           | 4       |                                                                                 |
//| fp-fl-sp-mul           | 5       |                                                                                 |
//| res2-offchip-threshold | 1000000 |                                                                                 |
//| res2-combrom-threshold | 64      |                                                                                 |
//| res2-combram-threshold | 32      |                                                                                 |
//| res2-regfile-threshold | 8       |                                                                                 |
//*------------------------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTMAINIDL4001PC10 
//*-------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause                       | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*-------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTMAINIDL4001PC10"     | 825 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 23   |
//| XU32'2:"2:kiwiTMAINIDL4001PC10"     | 823 | 1       | hwm=0.3.0    | 4    |        | 2     | 4   | 5    |
//| XU32'4:"4:kiwiTMAINIDL4001PC10"     | 822 | 5       | hwm=0.14.0   | 19   |        | 6     | 19  | 20   |
//| XU32'8:"8:kiwiTMAINIDL4001PC10"     | 820 | 20      | hwm=0.0.0    | 20   |        | -     | -   | 5    |
//| XU32'8:"8:kiwiTMAINIDL4001PC10"     | 821 | 20      | hwm=0.0.0    | 20   |        | -     | -   | 21   |
//| XU32'16:"16:kiwiTMAINIDL4001PC10"   | 819 | 21      | hwm=0.0.0    | 21   |        | -     | -   | 22   |
//| XU32'32:"32:kiwiTMAINIDL4001PC10"   | 818 | 22      | hwm=0.0.0    | 22   |        | -     | -   | 24   |
//| XU32'1:"1:kiwiTMAINIDL4001PC10"     | 824 | 23      | hwm=0.0.0    | 23   |        | -     | -   | 1    |
//| XU32'64:"64:kiwiTMAINIDL4001PC10"   | 816 | 24      | hwm=0.0.0    | 24   |        | -     | -   | 25   |
//| XU32'64:"64:kiwiTMAINIDL4001PC10"   | 817 | 24      | hwm=0.0.0    | 24   |        | -     | -   | 23   |
//| XU32'128:"128:kiwiTMAINIDL4001PC10" | 815 | 25      | hwm=0.0.0    | 25   |        | -     | -   | 26   |
//| XU32'256:"256:kiwiTMAINIDL4001PC10" | 814 | 26      | hwm=0.0.0    | 26   |        | -     | -   | 27   |
//| XU32'512:"512:kiwiTMAINIDL4001PC10" | 813 | 27      | hwm=0.0.0    | 27   |        | -     | -   | -    |
//*-------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'0:"0:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'0:"0:kiwiTMAINIDL4001PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                        |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                             |
//| F0   | E825 | R0 DATA |                                                                                                                                             |
//| F0+E | E825 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3.V_1 te=te:F0 write(S32'1) TMAIN_IDL400.dfsin.bm_main.0.3.V_0 te=te:F0 write(S32'0) TMAIN_IDL400.dfsin.sin_f\ |
//|      |      |         | loatnat.3.6.V_1 te=te:F0 write(??64'0) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3 te=te:F0 write(??64'0) TMAIN_IDL400.dfsin.sin_floatnat.3.6.\ |
//|      |      |         | V_2 te=te:F0 write(S32'0) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_0 te=te:F0 write(??64'0) TMAIN_IDL400.System.Math.Abs.1.29._SPILL.256 te=t\ |
//|      |      |         | e:F0 write(??64'0) TMAIN_IDL400.dfsin.bm_main.0.3.V_2 te=te:F0 write(U64'0) TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.257 te=te:F0 write(S32'0\ |
//|      |      |         | ) TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.258 te=te:F0 write(S32'0) done te=te:F0 write(U32'0) dfsin.gloops te=te:F0 write(S32'0)  PLI:dfsin\ |
//|      |      |         | : Testbench sta...                                                                                                                          |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'2:"2:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'2:"2:kiwiTMAINIDL4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                                                |
//| F1   | E823 | R0 DATA | ifMULTIPLIERALUF64_12_ te=te:F1 compute(E1, E2)                                                                                                                |
//| F2   | E823 | R1 DATA |                                                                                                                                                                |
//| F3   | E823 | R2 DATA |                                                                                                                                                                |
//| F4   | E823 | R3 DATA |                                                                                                                                                                |
//| F4+E | E823 | W0 DATA | TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1 te=te:F4 write(E3) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3 te=te:F4 write(E4) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2\ |
//|      |      |         |  te=te:F4 write(S32'1) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_0 te=te:F4 write(E3)                                                                              |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'4:"4:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'4:"4:kiwiTMAINIDL4001PC10"
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                                                                                |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F5    | -    | R0 CTRL  |                                                                                                                                     |
//| F5    | E822 | R0 DATA  | isMULTIPLIERALUS32_10_ te=te:F5 compute(E5, E6) ifMULTIPLIERALUF64_12_ te=te:F5 compute(E7, E8)                                     |
//| F6    | E822 | R1 DATA  |                                                                                                                                     |
//| F7    | E822 | R2 DATA  |                                                                                                                                     |
//| F8    | E822 | R3 DATA  | CVFPCVTFL2F64I32_10_ te=te:F8 hpr_dbl_from_int32(E9)                                                                                |
//| F9    | E822 | R4 DATA  |                                                                                                                                     |
//| F10   | E822 | R5 DATA  | ifDIVIDERALUF64_10_ te=te:F10 compute(E10, E11)                                                                                     |
//| F11   | E822 | R6 DATA  |                                                                                                                                     |
//| F12   | E822 | R7 DATA  |                                                                                                                                     |
//| F13   | E822 | R8 DATA  |                                                                                                                                     |
//| F14   | E822 | R9 DATA  |                                                                                                                                     |
//| F15   | E822 | R10 DATA | ifADDERALUF64_10_ te=te:F15 compute(E12, E13)                                                                                       |
//| F16   | E822 | R11 DATA |                                                                                                                                     |
//| F17   | E822 | R12 DATA |                                                                                                                                     |
//| F18   | E822 | R13 DATA |                                                                                                                                     |
//| F19   | E822 | R14 DATA |                                                                                                                                     |
//| F19+E | E822 | W0 DATA  | TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1 te=te:F19 write(E13) TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2 te=te:F19 write(E14) TMAIN_ID\ |
//|       |      |          | L400.dfsin.sin_floatnat.3.6.V_0 te=te:F19 write(E15) TMAIN_IDL400.System.Math.Abs.1.29._SPILL.256 te=te:F19 write(E16) dfsin.gloop\ |
//|       |      |          | s te=te:F19 write(C(1+dfsin.gloops))                                                                                                |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'8:"8:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'8:"8:kiwiTMAINIDL4001PC10"
//*-------+------+---------+---------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                    |
//*-------+------+---------+---------------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                         |
//| F20   | E821 | R0 DATA |                                                         |
//| F20+E | E821 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3.V_2 te=te:F20 write(E17) |
//| F20   | E820 | R0 DATA |                                                         |
//| F20+E | E820 | W0 DATA |                                                         |
//*-------+------+---------+---------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'16:"16:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'16:"16:kiwiTMAINIDL4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                          |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| F21   | -    | R0 CTRL |                                                                                                                               |
//| F21   | E819 | R0 DATA |                                                                                                                               |
//| F21+E | E819 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.257 te=te:F21 write(E18) TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.258 te=te:F21 write(E19) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'32:"32:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'32:"32:kiwiTMAINIDL4001PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                          |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*
//| F22   | -    | R0 CTRL |                                                                                                               |
//| F22   | E818 | R0 DATA |                                                                                                               |
//| F22+E | E818 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3.V_0 te=te:F22 write(E20)  PLI:   hamming error=%X  PLI:Test: input=%X expec... |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'1:"1:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'1:"1:kiwiTMAINIDL4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F23   | -    | R0 CTRL |      |
//| F23   | E824 | R0 DATA |      |
//| F23+E | E824 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'64:"64:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'64:"64:kiwiTMAINIDL4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                           |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F24   | -    | R0 CTRL |                                                                                                                |
//| F24   | E817 | R0 DATA |                                                                                                                |
//| F24+E | E817 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3.V_1 te=te:F24 write(E21)                                                        |
//| F24   | E816 | R0 DATA |                                                                                                                |
//| F24+E | E816 | W0 DATA | TMAIN_IDL400.dfsin.bm_main.0.3.V_1 te=te:F24 write(E21)  PLI:RESULT: FAIL  PLI:Result: %d/%d  PLI:RESULT: PASS |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'128:"128:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'128:"128:kiwiTMAINIDL4001PC10"
//*-------+------+---------+---------------------------------------------*
//| pc    | eno  | Phaser  | Work                                        |
//*-------+------+---------+---------------------------------------------*
//| F25   | -    | R0 CTRL |                                             |
//| F25   | E815 | R0 DATA |                                             |
//| F25+E | E815 | W0 DATA |  PLI:gloops=%d  PLI:dfsin: Testbench fin... |
//*-------+------+---------+---------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'256:"256:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'256:"256:kiwiTMAINIDL4001PC10"
//*-------+------+---------+-----------------------------*
//| pc    | eno  | Phaser  | Work                        |
//*-------+------+---------+-----------------------------*
//| F26   | -    | R0 CTRL |                             |
//| F26   | E814 | R0 DATA |                             |
//| F26+E | E814 | W0 DATA | done te=te:F26 write(U32'1) |
//*-------+------+---------+-----------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'512:"512:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'512:"512:kiwiTMAINIDL4001PC10"
//*-------+------+---------+----------------------------------------------*
//| pc    | eno  | Phaser  | Work                                         |
//*-------+------+---------+----------------------------------------------*
//| F27   | -    | R0 CTRL |                                              |
//| F27   | E813 | R0 DATA |                                              |
//| F27+E | E813 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:Driven Done at %u |
//*-------+------+---------+----------------------------------------------*

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
//  E1 =.= *APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1])
//
//
//  E2 =.= -*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1])
//
//
//  E3 =.= C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1]))
//
//
//  E4 =.= C64f(*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1])*-*APPLY:hpr_bitsToDouble(@64_US/CC/SCALbx14_ARB0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1]))
//
//
//  E5 =.= TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2
//
//
//  E6 =.= 1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2
//
//
//  E7 =.= TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3
//
//
//  E8 =.= TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1
//
//
//  E9 =.= 2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2)
//
//
//  E10 =.= TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1
//
//
//  E11 =.= CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2))
//
//
//  E12 =.= TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_0
//
//
//  E13 =.= C64f((TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1)/(CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2))))
//
//
//  E14 =.= C(1+TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2)
//
//
//  E15 =.= C64f(TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_0+(C64f((TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1)/(CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2))))))
//
//
//  E16 =.= COND((C64f((TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1)/(CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2)))))<0, C64f(-(C64f((TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1)/(CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2)))))), C64f((TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_3*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_1)/(CVT(C64f)(2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2*(1+2*TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_2)))))
//
//
//  E17 =.= C64u(*APPLY:hpr_doubleToBits(TMAIN_IDL400.dfsin.sin_floatnat.3.6.V_0))
//
//
//  E18 =.= C(TMAIN_IDL400.dfsin.bm_main.0.3.V_0)
//
//
//  E19 =.= COND(TMAIN_IDL400.dfsin.bm_main.0.3.V_2==@64_US/CC/SCALbx12_ARA0[TMAIN_IDL400.dfsin.bm_main.0.3.V_1], S32'1, S32'0)
//
//
//  E20 =.= C(TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.258+TMAIN_IDL400.dfsin.bm_main.0.3._SPILL.257)
//
//
//  E21 =.= C(1+TMAIN_IDL400.dfsin.bm_main.0.3.V_1)
//
//
//  E22 =.= 1E-05>=TMAIN_IDL400.System.Math.Abs.1.29._SPILL.256
//
//
//  E23 =.= 1E-05<TMAIN_IDL400.System.Math.Abs.1.29._SPILL.256
//
//
//  E24 =.= (C(1+TMAIN_IDL400.dfsin.bm_main.0.3.V_1))<36
//
//
//  E25 =.= (C(1+TMAIN_IDL400.dfsin.bm_main.0.3.V_1))>=36
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for dfsin-floatnat to dfsin-floatnat

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for dfsin_floatnat:
//19 vectors of width 64
//
//15 vectors of width 1
//
//1 vectors of width 5
//
//9 vectors of width 32
//
//72 array locations of width 64
//
//Total state bits in module = 6132 bits.
//
//550 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread softfloat..cctor uid=cctor12 has 8 CIL instructions in 1 basic blocks
//Thread dfsin..cctor uid=cctor10 has 16 CIL instructions in 1 basic blocks
//Thread dfsin.HWMain uid=HWMain10 has 76 CIL instructions in 20 basic blocks
//Thread mpc10 has 11 bevelab control states (pauses)
//Reindexed thread kiwiTMAINIDL4001PC10 with 28 minor control states
// eof (HPR L/S Verilog)
