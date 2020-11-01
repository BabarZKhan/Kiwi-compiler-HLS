

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:05
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test58r0.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58r0.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=waypoint_nets pi_name=kppIos10 */
    output reg [11:0] KppWaypoint0,
    output reg [639:0] KppWaypoint1,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=res2-directornets */
output reg [7:0] kiwiTESTMAIN400PC10nz_pc_export);

function [63:0] hpr_flt2dbl0;//Floating-point convert single to double precision.
input [31:0] darg;
  hpr_flt2dbl0 =  ((darg & 32'h7F80_0000)==32'h7F80_0000)?
     {darg[31], {4{darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}}:
     {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};
endfunction


function hpr_fp_dltd4; //Floating-point 'less-than' predicate.
   input [31:0] hpr_fp_dltd4_a, hpr_fp_dltd4_b;
  hpr_fp_dltd4 = ((hpr_fp_dltd4_a[31] && !hpr_fp_dltd4_b[31]) ? 1: (!hpr_fp_dltd4_a[31] && hpr_fp_dltd4_b[31])  ? 0 : (hpr_fp_dltd4_a[31]^(hpr_fp_dltd4_a[30:0]<hpr_fp_dltd4_b[30:0]))); 
   endfunction


function hpr_fp_dltd1; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd1_a, hpr_fp_dltd1_b;
  hpr_fp_dltd1 = ((hpr_fp_dltd1_a[63] && !hpr_fp_dltd1_b[63]) ? 1: (!hpr_fp_dltd1_a[63] && hpr_fp_dltd1_b[63])  ? 0 : (hpr_fp_dltd1_a[63]^(hpr_fp_dltd1_a[62:0]<hpr_fp_dltd1_b[62:0]))); 
   endfunction


function signed [31:0] rtl_signed_bitextract3;
   input [63:0] arg;
   rtl_signed_bitextract3 = $signed(arg[31:0]);
   endfunction


function [63:0] rtl_unsigned_extend2;
   input [31:0] arg;
   rtl_unsigned_extend2 = { 32'b0, arg[31:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4;
  reg/*fp*/  [63:0] TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3;
  reg/*fp*/  [63:0] TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256;
  reg/*fp*/  [63:0] TESTMAIN400_Test58r0_double_test_0_11_V_1;
  reg signed [63:0] TESTMAIN400_Test58r0_double_test_0_11_V_0;
  reg/*fp*/  [31:0] TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
  reg signed [63:0] TESTMAIN400_Test58r0_single_test_0_7_V_0;
// abstractionName=repack-newnets
  reg [31:0] A_UINT_CC_SCALbx10_ARA0[31:0];
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
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_10_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_YY;
  wire ifMULTIPLIERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F64I64_10
  wire/*fp*/  [63:0] CVFPCVTFL2F64I64_10_result;
  reg signed [63:0] CVFPCVTFL2F64I64_10_arg;
  wire CVFPCVTFL2F64I64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_12_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_YY;
  wire ifMULTIPLIERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_12_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_12_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_12_YY;
  wire ifADDERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_DIVIDER_DP
  wire/*fp*/  [63:0] ifDIVIDERALUF64_12_RR;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_12_NN;
  reg/*fp*/  [63:0] ifDIVIDERALUF64_12_DD;
  wire ifDIVIDERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL15_DIVIDER_SP
  wire/*fp*/  [31:0] ifDIVIDERALUF32_10_RR;
  reg/*fp*/  [31:0] ifDIVIDERALUF32_10_NN;
  reg/*fp*/  [31:0] ifDIVIDERALUF32_10_DD;
  wire ifDIVIDERALUF32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_SP
  wire/*fp*/  [31:0] ifADDERALUF32_10_RR;
  reg/*fp*/  [31:0] ifADDERALUF32_10_XX;
  reg/*fp*/  [31:0] ifADDERALUF32_10_YY;
  wire ifADDERALUF32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_MULTIPLIER_SP
  wire/*fp*/  [31:0] ifMULTIPLIERALUF32_10_RR;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_10_XX;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_10_YY;
  wire ifMULTIPLIERALUF32_10_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2F32I64_10
  wire/*fp*/  [31:0] CVFPCVTFL2F32I64_10_result;
  reg signed [63:0] CVFPCVTFL2F32I64_10_arg;
  wire CVFPCVTFL2F32I64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_MULTIPLIER_SP
  wire/*fp*/  [31:0] ifMULTIPLIERALUF32_12_RR;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_12_XX;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_12_YY;
  wire ifMULTIPLIERALUF32_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL5_MULTIPLIER_SP
  wire/*fp*/  [31:0] ifMULTIPLIERALUF32_14_RR;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_14_XX;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF32_14_YY;
  wire ifMULTIPLIERALUF32_14_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_SP
  wire/*fp*/  [31:0] ifADDERALUF32_12_RR;
  reg/*fp*/  [31:0] ifADDERALUF32_12_XX;
  reg/*fp*/  [31:0] ifADDERALUF32_12_YY;
  wire ifADDERALUF32_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_SP
  wire/*fp*/  [31:0] ifADDERALUF32_14_RR;
  reg/*fp*/  [31:0] ifADDERALUF32_14_XX;
  reg/*fp*/  [31:0] ifADDERALUF32_14_YY;
  wire ifADDERALUF32_14_FAIL;
// abstractionName=res2-morenets
  reg/*fp*/  [31:0] pipe164;
  reg/*fp*/  [31:0] pipe162;
  reg/*fp*/  [31:0] pipe160;
  reg/*fp*/  [31:0] pipe158;
  reg/*fp*/  [31:0] pipe156;
  reg/*fp*/  [31:0] pipe154;
  reg/*fp*/  [31:0] pipe152;
  reg/*fp*/  [31:0] pipe150;
  reg/*fp*/  [31:0] pipe148;
  reg/*fp*/  [31:0] pipe146;
  reg/*fp*/  [31:0] pipe144;
  reg/*fp*/  [31:0] pipe142;
  reg/*fp*/  [31:0] pipe140;
  reg/*fp*/  [31:0] pipe138;
  reg/*fp*/  [31:0] pipe136;
  reg/*fp*/  [31:0] pipe134;
  reg/*fp*/  [31:0] pipe132;
  reg/*fp*/  [31:0] pipe130;
  reg/*fp*/  [31:0] pipe128;
  reg/*fp*/  [31:0] pipe126;
  reg/*fp*/  [31:0] pipe124;
  reg/*fp*/  [31:0] pipe122;
  reg/*fp*/  [31:0] pipe120;
  reg/*fp*/  [31:0] pipe118;
  reg/*fp*/  [31:0] pipe116;
  reg/*fp*/  [31:0] pipe114;
  reg/*fp*/  [31:0] pipe112;
  reg/*fp*/  [31:0] pipe110;
  reg/*fp*/  [31:0] pipe108;
  reg/*fp*/  [31:0] pipe106;
  reg/*fp*/  [31:0] pipe104;
  reg/*fp*/  [31:0] pipe102;
  reg/*fp*/  [31:0] pipe100;
  reg/*fp*/  [31:0] pipe98;
  reg/*fp*/  [31:0] pipe96;
  reg/*fp*/  [31:0] pipe94;
  reg/*fp*/  [31:0] pipe92;
  reg/*fp*/  [31:0] pipe90;
  reg/*fp*/  [31:0] pipe88;
  reg/*fp*/  [31:0] pipe86;
  reg/*fp*/  [31:0] pipe84;
  reg/*fp*/  [31:0] pipe82;
  reg/*fp*/  [31:0] pipe80;
  reg/*fp*/  [31:0] pipe78;
  reg/*fp*/  [31:0] pipe76;
  reg/*fp*/  [31:0] pipe74;
  reg/*fp*/  [31:0] pipe72;
  reg/*fp*/  [31:0] pipe70;
  reg/*fp*/  [31:0] pipe68;
  reg/*fp*/  [31:0] pipe66;
  reg/*fp*/  [31:0] pipe64;
  reg/*fp*/  [31:0] pipe62;
  reg/*fp*/  [31:0] pipe60;
  reg/*fp*/  [31:0] pipe58;
  reg/*fp*/  [31:0] pipe56;
  reg/*fp*/  [31:0] pipe54;
  reg/*fp*/  [31:0] pipe52;
  reg/*fp*/  [31:0] pipe50;
  reg/*fp*/  [31:0] pipe48;
  reg/*fp*/  [31:0] pipe46;
  reg/*fp*/  [31:0] pipe44;
  reg/*fp*/  [31:0] pipe42;
  reg/*fp*/  [31:0] pipe40;
  reg/*fp*/  [31:0] pipe38;
  reg/*fp*/  [31:0] pipe36;
  reg/*fp*/  [31:0] pipe34;
  reg/*fp*/  [31:0] pipe32;
  reg/*fp*/  [31:0] pipe30;
  reg/*fp*/  [63:0] pipe28;
  reg/*fp*/  [63:0] pipe26;
  reg/*fp*/  [63:0] pipe24;
  reg/*fp*/  [63:0] pipe22;
  reg/*fp*/  [63:0] pipe20;
  reg/*fp*/  [63:0] pipe18;
  reg/*fp*/  [63:0] pipe16;
  reg/*fp*/  [63:0] pipe14;
  reg/*fp*/  [63:0] pipe12;
  reg/*fp*/  [63:0] pipe10;
  reg/*fp*/  [63:0] ifDIVIDERALUF6410RRh10hold;
  reg ifDIVIDERALUF6410RRh10shot0;
  reg ifDIVIDERALUF6410RRh10shot1;
  reg ifDIVIDERALUF6410RRh10shot2;
  reg ifDIVIDERALUF6410RRh10shot3;
  reg ifDIVIDERALUF6410RRh10shot4;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh10hold;
  reg ifADDERALUF6410RRh10shot0;
  reg ifADDERALUF6410RRh10shot1;
  reg ifADDERALUF6410RRh10shot2;
  reg ifADDERALUF6410RRh10shot3;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh10hold;
  reg ifMULTIPLIERALUF6410RRh10shot0;
  reg ifMULTIPLIERALUF6410RRh10shot1;
  reg ifMULTIPLIERALUF6410RRh10shot2;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6412RRh10hold;
  reg ifMULTIPLIERALUF6412RRh10shot0;
  reg ifMULTIPLIERALUF6412RRh10shot1;
  reg ifMULTIPLIERALUF6412RRh10shot2;
  reg/*fp*/  [63:0] ifADDERALUF6412RRh10hold;
  reg ifADDERALUF6412RRh10shot0;
  reg ifADDERALUF6412RRh10shot1;
  reg ifADDERALUF6412RRh10shot2;
  reg ifADDERALUF6412RRh10shot3;
  reg/*fp*/  [63:0] ifDIVIDERALUF6412RRh10hold;
  reg ifDIVIDERALUF6412RRh10shot0;
  reg ifDIVIDERALUF6412RRh10shot1;
  reg ifDIVIDERALUF6412RRh10shot2;
  reg ifDIVIDERALUF6412RRh10shot3;
  reg ifDIVIDERALUF6412RRh10shot4;
  reg/*fp*/  [63:0] CVFPCVTFL2F64I6410resulth10hold;
  reg CVFPCVTFL2F64I6410resulth10shot0;
  reg CVFPCVTFL2F64I6410resulth10shot1;
  reg/*fp*/  [31:0] ifADDERALUF3210RRh10hold;
  reg ifADDERALUF3210RRh10shot0;
  reg ifADDERALUF3210RRh10shot1;
  reg ifADDERALUF3210RRh10shot2;
  reg ifADDERALUF3210RRh10shot3;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF3210RRh10hold;
  reg ifMULTIPLIERALUF3210RRh10shot0;
  reg ifMULTIPLIERALUF3210RRh10shot1;
  reg ifMULTIPLIERALUF3210RRh10shot2;
  reg ifMULTIPLIERALUF3210RRh10shot3;
  reg ifMULTIPLIERALUF3210RRh10shot4;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF3212RRh10hold;
  reg ifMULTIPLIERALUF3212RRh10shot0;
  reg ifMULTIPLIERALUF3212RRh10shot1;
  reg ifMULTIPLIERALUF3212RRh10shot2;
  reg ifMULTIPLIERALUF3212RRh10shot3;
  reg ifMULTIPLIERALUF3212RRh10shot4;
  reg/*fp*/  [31:0] ifADDERALUF3212RRh10hold;
  reg ifADDERALUF3212RRh10shot0;
  reg ifADDERALUF3212RRh10shot1;
  reg ifADDERALUF3212RRh10shot2;
  reg ifADDERALUF3212RRh10shot3;
  reg/*fp*/  [31:0] ifDIVIDERALUF3210RRh12hold;
  reg ifDIVIDERALUF3210RRh12shot0;
  reg ifDIVIDERALUF3210RRh12shot1;
  reg ifDIVIDERALUF3210RRh12shot2;
  reg ifDIVIDERALUF3210RRh12shot3;
  reg ifDIVIDERALUF3210RRh12shot4;
  reg ifDIVIDERALUF3210RRh12shot5;
  reg ifDIVIDERALUF3210RRh12shot6;
  reg ifDIVIDERALUF3210RRh12shot7;
  reg ifDIVIDERALUF3210RRh12shot8;
  reg ifDIVIDERALUF3210RRh12shot9;
  reg ifDIVIDERALUF3210RRh12shot10;
  reg ifDIVIDERALUF3210RRh12shot11;
  reg ifDIVIDERALUF3210RRh12shot12;
  reg ifDIVIDERALUF3210RRh12shot13;
  reg ifDIVIDERALUF3210RRh12shot14;
  reg/*fp*/  [31:0] ifMULTIPLIERALUF3214RRh10hold;
  reg ifMULTIPLIERALUF3214RRh10shot0;
  reg ifMULTIPLIERALUF3214RRh10shot1;
  reg ifMULTIPLIERALUF3214RRh10shot2;
  reg ifMULTIPLIERALUF3214RRh10shot3;
  reg ifMULTIPLIERALUF3214RRh10shot4;
  reg/*fp*/  [31:0] ifADDERALUF3214RRh10hold;
  reg ifADDERALUF3214RRh10shot0;
  reg ifADDERALUF3214RRh10shot1;
  reg ifADDERALUF3214RRh10shot2;
  reg ifADDERALUF3214RRh10shot3;
  reg/*fp*/  [31:0] ifDIVIDERALUF3210RRh10hold;
  reg ifDIVIDERALUF3210RRh10shot0;
  reg ifDIVIDERALUF3210RRh10shot1;
  reg ifDIVIDERALUF3210RRh10shot2;
  reg ifDIVIDERALUF3210RRh10shot3;
  reg ifDIVIDERALUF3210RRh10shot4;
  reg ifDIVIDERALUF3210RRh10shot5;
  reg ifDIVIDERALUF3210RRh10shot6;
  reg ifDIVIDERALUF3210RRh10shot7;
  reg ifDIVIDERALUF3210RRh10shot8;
  reg ifDIVIDERALUF3210RRh10shot9;
  reg ifDIVIDERALUF3210RRh10shot10;
  reg ifDIVIDERALUF3210RRh10shot11;
  reg ifDIVIDERALUF3210RRh10shot12;
  reg ifDIVIDERALUF3210RRh10shot13;
  reg ifDIVIDERALUF3210RRh10shot14;
  reg/*fp*/  [31:0] CVFPCVTFL2F32I6410resulth10hold;
  reg CVFPCVTFL2F32I6410resulth10shot0;
  reg CVFPCVTFL2F32I6410resulth10shot1;
  reg [7:0] kiwiTESTMAIN400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  reg [31:0] hprpin501158x10;
  reg/*fp*/  [31:0] hprpin501163x10;
  reg [63:0] hprpin501304x10;
  wire [63:0] hprpin501316x10;
  reg/*fp*/  [63:0] hprpin501318x10;
  wire/*fp*/  [31:0] hprpin502015x10;
 always   @(* )  begin 
       ifDIVIDERALUF32_10_NN = 32'sd0;
       ifDIVIDERALUF32_10_DD = 32'sd0;
       ifADDERALUF32_14_XX = 32'sd0;
       ifADDERALUF32_14_YY = 32'sd0;
       ifMULTIPLIERALUF32_14_XX = 32'sd0;
       ifMULTIPLIERALUF32_14_YY = 32'sd0;
       ifADDERALUF32_12_XX = 32'sd0;
       ifADDERALUF32_12_YY = 32'sd0;
       ifMULTIPLIERALUF32_12_XX = 32'sd0;
       ifMULTIPLIERALUF32_12_YY = 32'sd0;
       CVFPCVTFL2F32I64_10_arg = 32'sd0;
       ifADDERALUF32_10_XX = 32'sd0;
       ifADDERALUF32_10_YY = 32'sd0;
       ifMULTIPLIERALUF32_10_XX = 32'sd0;
       ifMULTIPLIERALUF32_10_YY = 32'sd0;
       ifADDERALUF64_12_XX = 32'sd0;
       ifADDERALUF64_12_YY = 32'sd0;
       ifMULTIPLIERALUF64_12_XX = 32'sd0;
       ifMULTIPLIERALUF64_12_YY = 32'sd0;
       ifDIVIDERALUF64_12_NN = 32'sd0;
       ifDIVIDERALUF64_12_DD = 32'sd0;
       CVFPCVTFL2F64I64_10_arg = 32'sd0;
       ifMULTIPLIERALUF64_10_XX = 32'sd0;
       ifMULTIPLIERALUF64_10_YY = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifDIVIDERALUF64_10_NN = 32'sd0;
       ifDIVIDERALUF64_10_DD = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h3/*3:kiwiTESTMAIN400PC10nz*/:  begin 
               ifDIVIDERALUF32_10_NN = ((32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold
              );

               ifDIVIDERALUF32_10_DD = hprpin501163x10;
               end 
              
          32'h12/*18:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF32_14_XX = pipe164;
               ifADDERALUF32_14_YY = ((32'h12/*18:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF32_10_RR: ifDIVIDERALUF3210RRh10hold
              );

               end 
              
          32'h16/*22:kiwiTESTMAIN400PC10nz*/:  begin 
               ifMULTIPLIERALUF32_14_XX = 32'h3f00_0000;
               ifMULTIPLIERALUF32_14_YY = ((32'h16/*22:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF32_14_RR: ifADDERALUF3214RRh10hold
              );

               end 
              
          32'h1b/*27:kiwiTESTMAIN400PC10nz*/:  begin 
               ifDIVIDERALUF32_10_NN = pipe134;
               ifDIVIDERALUF32_10_DD = ((32'h1b/*27:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_14_RR: ifMULTIPLIERALUF3214RRh10hold
              );

               end 
              
          32'h2a/*42:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF32_12_XX = pipe86;
               ifADDERALUF32_12_YY = ((32'h2a/*42:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF32_10_RR: ifDIVIDERALUF3210RRh12hold
              );

               end 
              
          32'h2e/*46:kiwiTESTMAIN400PC10nz*/:  begin 
               ifMULTIPLIERALUF32_12_XX = 32'h3f00_0000;
               ifMULTIPLIERALUF32_12_YY = ((32'h2e/*46:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF32_12_RR: ifADDERALUF3212RRh10hold
              );

               end 
              
          32'h1/*1:kiwiTESTMAIN400PC10nz*/:  CVFPCVTFL2F32I64_10_arg = TESTMAIN400_Test58r0_single_test_0_7_V_0;

          32'h3a/*58:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF32_10_XX = pipe34;
               ifADDERALUF32_10_YY = ((32'h3a/*58:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_10_RR: ifMULTIPLIERALUF3210RRh10hold
              );

               end 
              
          32'h3e/*62:kiwiTESTMAIN400PC10nz*/:  begin 
               ifDIVIDERALUF32_10_NN = ((32'h3e/*62:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF32_10_RR: ifADDERALUF3210RRh10hold
              );

               ifDIVIDERALUF32_10_DD = pipe42;
               end 
              
          32'h35/*53:kiwiTESTMAIN400PC10nz*/:  begin 
               CVFPCVTFL2F32I64_10_arg = TESTMAIN400_Test58r0_single_test_0_7_V_0;
               ifMULTIPLIERALUF32_10_XX = TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
               ifMULTIPLIERALUF32_10_YY = 32'sh8000_0000^TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
               end 
              
          32'h52/*82:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF32_10_XX = pipe48;
               ifADDERALUF32_10_YY = ((32'h52/*82:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_10_RR: ifMULTIPLIERALUF3210RRh10hold
              );

               end 
              
          32'h56/*86:kiwiTESTMAIN400PC10nz*/:  begin 
               ifDIVIDERALUF32_10_NN = ((32'h56/*86:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF32_10_RR: ifADDERALUF3210RRh10hold
              );

               ifDIVIDERALUF32_10_DD = pipe56;
               end 
              
          32'h6e/*110:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF64_12_XX = pipe28;
               ifADDERALUF64_12_YY = ((32'h6e/*110:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF64_12_RR: ifDIVIDERALUF6412RRh10hold
              );

               end 
              
          32'h72/*114:kiwiTESTMAIN400PC10nz*/:  begin 
               ifMULTIPLIERALUF64_12_XX = 64'h3fe0_0000_0000_0000;
               ifMULTIPLIERALUF64_12_YY = ((32'h72/*114:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF64_12_RR: ifADDERALUF6412RRh10hold
              );

               end 
              
          32'h69/*105:kiwiTESTMAIN400PC10nz*/: if ((TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4<32'sd3))  begin 
                   ifDIVIDERALUF64_12_NN = TESTMAIN400_Test58r0_double_test_0_11_V_1;
                   ifDIVIDERALUF64_12_DD = TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3;
                   end 
                  
          32'h66/*102:kiwiTESTMAIN400PC10nz*/:  CVFPCVTFL2F64I64_10_arg = TESTMAIN400_Test58r0_double_test_0_11_V_0;

          32'h77/*119:kiwiTESTMAIN400PC10nz*/:  begin 
               CVFPCVTFL2F64I64_10_arg = TESTMAIN400_Test58r0_double_test_0_11_V_0;
               ifMULTIPLIERALUF64_10_XX = TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256;
               ifMULTIPLIERALUF64_10_YY = 64'sh8000_0000_0000_0000^TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256;
               end 
              
          32'h7a/*122:kiwiTESTMAIN400PC10nz*/:  begin 
               ifADDERALUF64_10_XX = pipe10;
               ifADDERALUF64_10_YY = ((32'h7a/*122:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold
              );

               end 
              
          32'h7e/*126:kiwiTESTMAIN400PC10nz*/:  begin 
               ifDIVIDERALUF64_10_NN = ((32'h7e/*126:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh10hold
              );

               ifDIVIDERALUF64_10_DD = pipe18;
               end 
              endcase
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'd0;
               TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4 <= 32'd0;
               TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3 <= 64'd0;
               TESTMAIN400_Test58r0_double_test_0_11_V_1 <= 64'd0;
               TESTMAIN400_Test58r0_double_test_0_11_V_0 <= 64'd0;
               TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= 32'd0;
               TESTMAIN400_Test58r0_single_test_0_7_V_0 <= 64'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test58r0 start - sqrt");
                          $display("Test58r0: Single Test");
                           kiwiTESTMAIN400PC10nz <= 32'h34/*52:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'h0;
                           TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4 <= 32'sh0;
                           TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3 <= 64'h0;
                           TESTMAIN400_Test58r0_double_test_0_11_V_1 <= 64'h0;
                           TESTMAIN400_Test58r0_double_test_0_11_V_0 <= 64'sh0;
                           TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= 32'h0;
                           TESTMAIN400_Test58r0_single_test_0_7_V_0 <= 64'sh2;
                           KppWaypoint0 <= 32'sd1;
                           KppWaypoint1 <= "Starting Single";
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'he/*14:kiwiTESTMAIN400PC10nz*/;
                      
                  32'he/*14:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hf/*15:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h10/*16:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h10/*16:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h11/*17:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h11/*17:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h12/*18:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h12/*18:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h13/*19:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h13/*19:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h14/*20:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h14/*20:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h15/*21:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h15/*21:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h16/*22:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h16/*22:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h17/*23:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h17/*23:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h18/*24:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h18/*24:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h19/*25:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h19/*25:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1a/*26:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1a/*26:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1b/*27:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1b/*27:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1c/*28:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1c/*28:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1d/*29:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1d/*29:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1e/*30:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1e/*30:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1f/*31:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h1f/*31:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h20/*32:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h20/*32:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h21/*33:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h21/*33:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h22/*34:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h22/*34:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h23/*35:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h23/*35:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h24/*36:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h24/*36:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h25/*37:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h25/*37:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h26/*38:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h26/*38:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h27/*39:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h27/*39:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h28/*40:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h28/*40:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h29/*41:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h29/*41:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2a/*42:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2a/*42:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2b/*43:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2b/*43:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2c/*44:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2c/*44:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2d/*45:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2d/*45:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2e/*46:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2e/*46:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2f/*47:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2f/*47:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h30/*48:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h30/*48:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h31/*49:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h31/*49:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h32/*50:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h32/*50:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h33/*51:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h33/*51:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("    p=%1d   pf=%f", TESTMAIN400_Test58r0_single_test_0_7_V_0, $bitstoreal(hpr_flt2dbl0(((32'h3/*3:kiwiTESTMAIN400PC10nz*/==
                          kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold))));
                           kiwiTESTMAIN400PC10nz <= 32'h35/*53:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= hprpin502015x10;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h36/*54:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h37/*55:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h37/*55:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h38/*56:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h38/*56:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h39/*57:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h39/*57:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3a/*58:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3a/*58:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3b/*59:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3b/*59:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3c/*60:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3c/*60:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3d/*61:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3d/*61:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3e/*62:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3e/*62:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3f/*63:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3f/*63:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h40/*64:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h40/*64:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h41/*65:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h41/*65:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h42/*66:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h42/*66:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h43/*67:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h43/*67:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h44/*68:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h44/*68:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h45/*69:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h45/*69:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h46/*70:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h46/*70:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h47/*71:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h47/*71:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h48/*72:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h48/*72:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h49/*73:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h49/*73:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4a/*74:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4a/*74:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4b/*75:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4b/*75:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4c/*76:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4c/*76:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4d/*77:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h34/*52:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4d/*77:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(64'shcc*TESTMAIN400_Test58r0_single_test_0_7_V_0)<64'sh3_8d7e_a4c6_8000)) $display("         root sp=%f            error=%f"
                              , $bitstoreal(hpr_flt2dbl0(TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256)), $bitstoreal(hpr_flt2dbl0(((32'h4d
                              /*77:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF32_10_RR: ifDIVIDERALUF3210RRh12hold
                              ))));
                               kiwiTESTMAIN400PC10nz <= 32'h34/*52:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test58r0_single_test_0_7_V_0 <= $signed(64'shcc*TESTMAIN400_Test58r0_single_test_0_7_V_0);
                           end 
                          
                  32'h35/*53:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16) if (($signed(64'shcc*TESTMAIN400_Test58r0_single_test_0_7_V_0
                      )<64'sh3_8d7e_a4c6_8000))  kiwiTESTMAIN400PC10nz <= 32'h36/*54:kiwiTESTMAIN400PC10nz*/;
                           else  kiwiTESTMAIN400PC10nz <= 32'h4e/*78:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4e/*78:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4f/*79:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4f/*79:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h50/*80:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h50/*80:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h51/*81:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h51/*81:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h52/*82:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h52/*82:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h53/*83:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h53/*83:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h54/*84:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h54/*84:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h55/*85:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h55/*85:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h56/*86:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h56/*86:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h57/*87:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h57/*87:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h58/*88:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h58/*88:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h59/*89:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h59/*89:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5a/*90:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5a/*90:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5b/*91:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5b/*91:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5c/*92:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5c/*92:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5d/*93:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5d/*93:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5e/*94:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5e/*94:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5f/*95:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5f/*95:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h60/*96:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h60/*96:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h61/*97:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h61/*97:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h62/*98:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h62/*98:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h63/*99:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h63/*99:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h64/*100:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h64/*100:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h65/*101:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h65/*101:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(64'shcc*TESTMAIN400_Test58r0_single_test_0_7_V_0)>=64'sh3_8d7e_a4c6_8000))  begin 
                                  $display("         root sp=%f            error=%f", $bitstoreal(hpr_flt2dbl0(TESTMAIN400_test58_sqrt_Sqrt_sp_1_12_SPILL_256
                                  )), $bitstoreal(hpr_flt2dbl0(((32'h65/*101:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF32_10_RR
                                  : ifDIVIDERALUF3210RRh12hold))));
                                  $display("Test58r1: Double Test");
                                   KppWaypoint0 <= 32'sd2;
                                   KppWaypoint1 <= "Starting Double";
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h76/*118:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test58r0_double_test_0_11_V_0 <= 64'sh2;
                           TESTMAIN400_Test58r0_single_test_0_7_V_0 <= $signed(64'shcc*TESTMAIN400_Test58r0_single_test_0_7_V_0);
                           end 
                          
                  32'h67/*103:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h68/*104:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h68/*104:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? !hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I64_10_result
                          ): !hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I6410resulth10hold))) $display("    p=%1d   pf=%f", TESTMAIN400_Test58r0_double_test_0_11_V_0
                              , $bitstoreal(((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F64I64_10_result
                              : CVFPCVTFL2F64I6410resulth10hold)));
                              if (((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I64_10_result
                          ): hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I6410resulth10hold)))  begin 
                                  $display("    p=%1d   pf=%f", TESTMAIN400_Test58r0_double_test_0_11_V_0, $bitstoreal(((32'h68/*104:kiwiTESTMAIN400PC10nz*/==
                                  kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F64I64_10_result: CVFPCVTFL2F64I6410resulth10hold)));
                                   kiwiTESTMAIN400PC10nz <= 32'h69/*105:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4 <= 32'sh0;
                                   TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3 <= hprpin501318x10;
                                   TESTMAIN400_Test58r0_double_test_0_11_V_1 <= ((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
                                  )? CVFPCVTFL2F64I64_10_result: CVFPCVTFL2F64I6410resulth10hold);

                                   end 
                                  if (((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? !hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I64_10_result
                          ): !hpr_fp_dltd1(64'h0, CVFPCVTFL2F64I6410resulth10hold)))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h77/*119:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'h0;
                                   TESTMAIN400_Test58r0_double_test_0_11_V_1 <= ((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
                                  )? CVFPCVTFL2F64I64_10_result: CVFPCVTFL2F64I6410resulth10hold);

                                   end 
                                   end 
                          
                  32'h6a/*106:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6b/*107:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6b/*107:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6c/*108:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6c/*108:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6d/*109:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6d/*109:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6e/*110:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6e/*110:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6f/*111:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6f/*111:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h70/*112:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h70/*112:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h71/*113:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h71/*113:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h72/*114:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h72/*114:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h73/*115:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h73/*115:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h74/*116:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h74/*116:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h75/*117:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h69/*105:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4<32'sd3
                      ))  kiwiTESTMAIN400PC10nz <= 32'h6a/*106:kiwiTESTMAIN400PC10nz*/;
                           else  begin 
                               kiwiTESTMAIN400PC10nz <= 32'h77/*119:kiwiTESTMAIN400PC10nz*/;
                               TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256 <= TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3;
                               end 
                              
                  32'h75/*117:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h69/*105:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4 <= $signed(32'sd1+TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4);
                           TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3 <= ((32'h75/*117:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF64_12_RR
                          : ifMULTIPLIERALUF6412RRh10hold);

                           end 
                          
                  32'h66/*102:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h67/*103:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h77/*119:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h78/*120:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h78/*120:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h79/*121:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h79/*121:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7a/*122:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7a/*122:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7b/*123:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7b/*123:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7c/*124:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7c/*124:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7d/*125:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7d/*125:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7e/*126:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7e/*126:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7f/*127:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7f/*127:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h80/*128:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h80/*128:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h81/*129:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h81/*129:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h82/*130:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h82/*130:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h83/*131:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h76/*118:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TESTMAIN400_Test58r0_double_test_0_11_V_0>=64'sh3_8d7e_a4c6_8000))  begin 
                                  $display("Test58 finished.");
                                  $finish(32'sd0);
                                   end 
                                   else  kiwiTESTMAIN400PC10nz <= 32'h66/*102:kiwiTESTMAIN400PC10nz*/;
                          if ((TESTMAIN400_Test58r0_double_test_0_11_V_0>=64'sh3_8d7e_a4c6_8000))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h84/*132:kiwiTESTMAIN400PC10nz*/;
                                   KppWaypoint0 <= 32'sd3;
                                   KppWaypoint1 <= "Finished";
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h83/*131:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("         root dp=%f            error=%f", $bitstoreal(TESTMAIN400_test58_sqrt_Sqrt_1_12_SPILL_256
                          ), $bitstoreal(((32'h83/*131:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifDIVIDERALUF64_10_RR: ifDIVIDERALUF6410RRh10hold
                          )));
                           kiwiTESTMAIN400PC10nz <= 32'h76/*118:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test58r0_double_test_0_11_V_0 <= $signed(64'shcc*TESTMAIN400_Test58r0_double_test_0_11_V_0);
                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               pipe164 <= 32'd0;
               pipe162 <= 32'd0;
               pipe160 <= 32'd0;
               pipe158 <= 32'd0;
               pipe156 <= 32'd0;
               pipe154 <= 32'd0;
               pipe152 <= 32'd0;
               pipe150 <= 32'd0;
               pipe148 <= 32'd0;
               pipe146 <= 32'd0;
               pipe144 <= 32'd0;
               pipe142 <= 32'd0;
               pipe140 <= 32'd0;
               pipe138 <= 32'd0;
               pipe136 <= 32'd0;
               pipe134 <= 32'd0;
               pipe132 <= 32'd0;
               pipe130 <= 32'd0;
               pipe128 <= 32'd0;
               pipe126 <= 32'd0;
               pipe124 <= 32'd0;
               pipe122 <= 32'd0;
               pipe120 <= 32'd0;
               pipe118 <= 32'd0;
               pipe116 <= 32'd0;
               pipe114 <= 32'd0;
               pipe112 <= 32'd0;
               pipe110 <= 32'd0;
               pipe108 <= 32'd0;
               pipe106 <= 32'd0;
               pipe104 <= 32'd0;
               pipe102 <= 32'd0;
               pipe100 <= 32'd0;
               pipe98 <= 32'd0;
               pipe96 <= 32'd0;
               pipe94 <= 32'd0;
               pipe92 <= 32'd0;
               pipe90 <= 32'd0;
               pipe88 <= 32'd0;
               pipe86 <= 32'd0;
               pipe84 <= 32'd0;
               pipe82 <= 32'd0;
               pipe80 <= 32'd0;
               pipe78 <= 32'd0;
               pipe76 <= 32'd0;
               pipe74 <= 32'd0;
               pipe72 <= 32'd0;
               pipe70 <= 32'd0;
               pipe68 <= 32'd0;
               pipe66 <= 32'd0;
               pipe64 <= 32'd0;
               pipe62 <= 32'd0;
               pipe60 <= 32'd0;
               pipe58 <= 32'd0;
               pipe56 <= 32'd0;
               pipe54 <= 32'd0;
               pipe52 <= 32'd0;
               pipe50 <= 32'd0;
               pipe48 <= 32'd0;
               pipe46 <= 32'd0;
               pipe44 <= 32'd0;
               pipe42 <= 32'd0;
               pipe40 <= 32'd0;
               pipe38 <= 32'd0;
               pipe36 <= 32'd0;
               pipe34 <= 32'd0;
               pipe32 <= 32'd0;
               pipe30 <= 32'd0;
               pipe28 <= 64'd0;
               pipe26 <= 64'd0;
               pipe24 <= 64'd0;
               pipe22 <= 64'd0;
               pipe20 <= 64'd0;
               pipe18 <= 64'd0;
               pipe16 <= 64'd0;
               pipe14 <= 64'd0;
               pipe12 <= 64'd0;
               pipe10 <= 64'd0;
               CVFPCVTFL2F32I6410resulth10hold <= 32'd0;
               CVFPCVTFL2F32I6410resulth10shot1 <= 32'd0;
               ifDIVIDERALUF3210RRh10hold <= 32'd0;
               ifDIVIDERALUF3210RRh10shot1 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot2 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot3 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot4 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot5 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot6 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot7 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot8 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot9 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot10 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot11 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot12 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot13 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot14 <= 32'd0;
               ifADDERALUF3214RRh10hold <= 32'd0;
               ifADDERALUF3214RRh10shot1 <= 32'd0;
               ifADDERALUF3214RRh10shot2 <= 32'd0;
               ifADDERALUF3214RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3214RRh10hold <= 32'd0;
               ifMULTIPLIERALUF3214RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF3214RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF3214RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3214RRh10shot4 <= 32'd0;
               ifDIVIDERALUF3210RRh12hold <= 32'd0;
               ifDIVIDERALUF3210RRh12shot1 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot2 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot3 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot4 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot5 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot6 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot7 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot8 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot9 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot10 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot11 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot12 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot13 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot14 <= 32'd0;
               ifADDERALUF3212RRh10hold <= 32'd0;
               ifADDERALUF3212RRh10shot1 <= 32'd0;
               ifADDERALUF3212RRh10shot2 <= 32'd0;
               ifADDERALUF3212RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3212RRh10hold <= 32'd0;
               ifMULTIPLIERALUF3212RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF3212RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF3212RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3212RRh10shot4 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10hold <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot4 <= 32'd0;
               ifADDERALUF3210RRh10hold <= 32'd0;
               ifADDERALUF3210RRh10shot1 <= 32'd0;
               ifADDERALUF3210RRh10shot2 <= 32'd0;
               ifADDERALUF3210RRh10shot3 <= 32'd0;
               CVFPCVTFL2F64I6410resulth10hold <= 64'd0;
               CVFPCVTFL2F64I6410resulth10shot1 <= 32'd0;
               ifDIVIDERALUF6412RRh10hold <= 64'd0;
               ifDIVIDERALUF6412RRh10shot1 <= 32'd0;
               ifDIVIDERALUF6412RRh10shot2 <= 32'd0;
               ifDIVIDERALUF6412RRh10shot3 <= 32'd0;
               ifDIVIDERALUF6412RRh10shot4 <= 32'd0;
               ifADDERALUF6412RRh10hold <= 64'd0;
               ifADDERALUF6412RRh10shot1 <= 32'd0;
               ifADDERALUF6412RRh10shot2 <= 32'd0;
               ifADDERALUF6412RRh10shot3 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6412RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10hold <= 64'd0;
               ifADDERALUF6410RRh10shot1 <= 32'd0;
               ifADDERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10shot3 <= 32'd0;
               ifDIVIDERALUF6410RRh10hold <= 64'd0;
               ifDIVIDERALUF6410RRh10shot1 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot2 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot3 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot4 <= 32'd0;
               ifDIVIDERALUF6410RRh10shot0 <= 32'd0;
               ifADDERALUF6410RRh10shot0 <= 32'd0;
               CVFPCVTFL2F64I6410resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot0 <= 32'd0;
               ifADDERALUF6412RRh10shot0 <= 32'd0;
               ifDIVIDERALUF6412RRh10shot0 <= 32'd0;
               ifDIVIDERALUF3210RRh12shot0 <= 32'd0;
               ifADDERALUF3210RRh10shot0 <= 32'd0;
               CVFPCVTFL2F32I6410resulth10shot0 <= 32'd0;
               ifMULTIPLIERALUF3210RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF3212RRh10shot0 <= 32'd0;
               ifADDERALUF3212RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF3214RRh10shot0 <= 32'd0;
               ifADDERALUF3214RRh10shot0 <= 32'd0;
               ifDIVIDERALUF3210RRh10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if (ifDIVIDERALUF6410RRh10shot4)  ifDIVIDERALUF6410RRh10hold <= ifDIVIDERALUF64_10_RR;
                      if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh10shot2)  ifMULTIPLIERALUF6410RRh10hold <= ifMULTIPLIERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6412RRh10shot2)  ifMULTIPLIERALUF6412RRh10hold <= ifMULTIPLIERALUF64_12_RR;
                      if (ifADDERALUF6412RRh10shot3)  ifADDERALUF6412RRh10hold <= ifADDERALUF64_12_RR;
                      if (ifDIVIDERALUF6412RRh10shot4)  ifDIVIDERALUF6412RRh10hold <= ifDIVIDERALUF64_12_RR;
                      if (CVFPCVTFL2F64I6410resulth10shot1)  CVFPCVTFL2F64I6410resulth10hold <= CVFPCVTFL2F64I64_10_result;
                      if (ifADDERALUF3210RRh10shot3)  ifADDERALUF3210RRh10hold <= ifADDERALUF32_10_RR;
                      if (ifMULTIPLIERALUF3210RRh10shot4)  ifMULTIPLIERALUF3210RRh10hold <= ifMULTIPLIERALUF32_10_RR;
                      if (ifMULTIPLIERALUF3212RRh10shot4)  ifMULTIPLIERALUF3212RRh10hold <= ifMULTIPLIERALUF32_12_RR;
                      if (ifADDERALUF3212RRh10shot3)  ifADDERALUF3212RRh10hold <= ifADDERALUF32_12_RR;
                      if (ifDIVIDERALUF3210RRh12shot14)  ifDIVIDERALUF3210RRh12hold <= ifDIVIDERALUF32_10_RR;
                      if (ifMULTIPLIERALUF3214RRh10shot4)  ifMULTIPLIERALUF3214RRh10hold <= ifMULTIPLIERALUF32_14_RR;
                      if (ifADDERALUF3214RRh10shot3)  ifADDERALUF3214RRh10hold <= ifADDERALUF32_14_RR;
                      if (ifDIVIDERALUF3210RRh10shot14)  ifDIVIDERALUF3210RRh10hold <= ifDIVIDERALUF32_10_RR;
                      if (CVFPCVTFL2F32I6410resulth10shot1)  CVFPCVTFL2F32I6410resulth10hold <= CVFPCVTFL2F32I64_10_result;
                       kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   pipe164 <= pipe162;
                   pipe162 <= pipe160;
                   pipe160 <= pipe158;
                   pipe158 <= pipe156;
                   pipe156 <= pipe154;
                   pipe154 <= pipe152;
                   pipe152 <= pipe150;
                   pipe150 <= pipe148;
                   pipe148 <= pipe146;
                   pipe146 <= pipe144;
                   pipe144 <= pipe142;
                   pipe142 <= pipe140;
                   pipe140 <= pipe138;
                   pipe138 <= pipe136;
                   pipe136 <= hprpin501163x10;
                   pipe134 <= pipe132;
                   pipe132 <= pipe130;
                   pipe130 <= pipe128;
                   pipe128 <= pipe126;
                   pipe126 <= pipe124;
                   pipe124 <= pipe122;
                   pipe122 <= pipe120;
                   pipe120 <= pipe118;
                   pipe118 <= pipe116;
                   pipe116 <= pipe114;
                   pipe114 <= pipe112;
                   pipe112 <= pipe110;
                   pipe110 <= pipe108;
                   pipe108 <= pipe106;
                   pipe106 <= pipe104;
                   pipe104 <= pipe102;
                   pipe102 <= pipe100;
                   pipe100 <= pipe98;
                   pipe98 <= pipe96;
                   pipe96 <= pipe94;
                   pipe94 <= pipe92;
                   pipe92 <= pipe90;
                   pipe90 <= pipe88;
                   pipe88 <= ((32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold
                  );

                   pipe86 <= pipe84;
                   pipe84 <= pipe82;
                   pipe82 <= pipe80;
                   pipe80 <= pipe78;
                   pipe78 <= pipe76;
                   pipe76 <= pipe74;
                   pipe74 <= pipe72;
                   pipe72 <= pipe70;
                   pipe70 <= pipe68;
                   pipe68 <= pipe66;
                   pipe66 <= pipe64;
                   pipe64 <= pipe62;
                   pipe62 <= pipe60;
                   pipe60 <= pipe58;
                   pipe58 <= ((32'h1b/*27:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_14_RR: ifMULTIPLIERALUF3214RRh10hold
                  );

                   pipe56 <= pipe54;
                   pipe54 <= pipe52;
                   pipe52 <= pipe50;
                   pipe50 <= pipe48;
                   pipe48 <= pipe46;
                   pipe46 <= pipe44;
                   pipe44 <= ((32'h4f/*79:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold
                  );

                   pipe42 <= pipe40;
                   pipe40 <= pipe38;
                   pipe38 <= pipe36;
                   pipe36 <= pipe34;
                   pipe34 <= pipe32;
                   pipe32 <= pipe30;
                   pipe30 <= ((32'h37/*55:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold
                  );

                   pipe28 <= pipe26;
                   pipe26 <= pipe24;
                   pipe24 <= pipe22;
                   pipe22 <= pipe20;
                   pipe20 <= TESTMAIN400_test58_sqrt_Sqrt_1_12_V_3;
                   pipe18 <= pipe16;
                   pipe16 <= pipe14;
                   pipe14 <= pipe12;
                   pipe12 <= pipe10;
                   pipe10 <= ((32'h79/*121:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F64I64_10_result: CVFPCVTFL2F64I6410resulth10hold
                  );

                   CVFPCVTFL2F32I6410resulth10shot1 <= CVFPCVTFL2F32I6410resulth10shot0;
                   ifDIVIDERALUF3210RRh10shot1 <= ifDIVIDERALUF3210RRh10shot0;
                   ifDIVIDERALUF3210RRh10shot2 <= ifDIVIDERALUF3210RRh10shot1;
                   ifDIVIDERALUF3210RRh10shot3 <= ifDIVIDERALUF3210RRh10shot2;
                   ifDIVIDERALUF3210RRh10shot4 <= ifDIVIDERALUF3210RRh10shot3;
                   ifDIVIDERALUF3210RRh10shot5 <= ifDIVIDERALUF3210RRh10shot4;
                   ifDIVIDERALUF3210RRh10shot6 <= ifDIVIDERALUF3210RRh10shot5;
                   ifDIVIDERALUF3210RRh10shot7 <= ifDIVIDERALUF3210RRh10shot6;
                   ifDIVIDERALUF3210RRh10shot8 <= ifDIVIDERALUF3210RRh10shot7;
                   ifDIVIDERALUF3210RRh10shot9 <= ifDIVIDERALUF3210RRh10shot8;
                   ifDIVIDERALUF3210RRh10shot10 <= ifDIVIDERALUF3210RRh10shot9;
                   ifDIVIDERALUF3210RRh10shot11 <= ifDIVIDERALUF3210RRh10shot10;
                   ifDIVIDERALUF3210RRh10shot12 <= ifDIVIDERALUF3210RRh10shot11;
                   ifDIVIDERALUF3210RRh10shot13 <= ifDIVIDERALUF3210RRh10shot12;
                   ifDIVIDERALUF3210RRh10shot14 <= ifDIVIDERALUF3210RRh10shot13;
                   ifADDERALUF3214RRh10shot1 <= ifADDERALUF3214RRh10shot0;
                   ifADDERALUF3214RRh10shot2 <= ifADDERALUF3214RRh10shot1;
                   ifADDERALUF3214RRh10shot3 <= ifADDERALUF3214RRh10shot2;
                   ifMULTIPLIERALUF3214RRh10shot1 <= ifMULTIPLIERALUF3214RRh10shot0;
                   ifMULTIPLIERALUF3214RRh10shot2 <= ifMULTIPLIERALUF3214RRh10shot1;
                   ifMULTIPLIERALUF3214RRh10shot3 <= ifMULTIPLIERALUF3214RRh10shot2;
                   ifMULTIPLIERALUF3214RRh10shot4 <= ifMULTIPLIERALUF3214RRh10shot3;
                   ifDIVIDERALUF3210RRh12shot1 <= ifDIVIDERALUF3210RRh12shot0;
                   ifDIVIDERALUF3210RRh12shot2 <= ifDIVIDERALUF3210RRh12shot1;
                   ifDIVIDERALUF3210RRh12shot3 <= ifDIVIDERALUF3210RRh12shot2;
                   ifDIVIDERALUF3210RRh12shot4 <= ifDIVIDERALUF3210RRh12shot3;
                   ifDIVIDERALUF3210RRh12shot5 <= ifDIVIDERALUF3210RRh12shot4;
                   ifDIVIDERALUF3210RRh12shot6 <= ifDIVIDERALUF3210RRh12shot5;
                   ifDIVIDERALUF3210RRh12shot7 <= ifDIVIDERALUF3210RRh12shot6;
                   ifDIVIDERALUF3210RRh12shot8 <= ifDIVIDERALUF3210RRh12shot7;
                   ifDIVIDERALUF3210RRh12shot9 <= ifDIVIDERALUF3210RRh12shot8;
                   ifDIVIDERALUF3210RRh12shot10 <= ifDIVIDERALUF3210RRh12shot9;
                   ifDIVIDERALUF3210RRh12shot11 <= ifDIVIDERALUF3210RRh12shot10;
                   ifDIVIDERALUF3210RRh12shot12 <= ifDIVIDERALUF3210RRh12shot11;
                   ifDIVIDERALUF3210RRh12shot13 <= ifDIVIDERALUF3210RRh12shot12;
                   ifDIVIDERALUF3210RRh12shot14 <= ifDIVIDERALUF3210RRh12shot13;
                   ifADDERALUF3212RRh10shot1 <= ifADDERALUF3212RRh10shot0;
                   ifADDERALUF3212RRh10shot2 <= ifADDERALUF3212RRh10shot1;
                   ifADDERALUF3212RRh10shot3 <= ifADDERALUF3212RRh10shot2;
                   ifMULTIPLIERALUF3212RRh10shot1 <= ifMULTIPLIERALUF3212RRh10shot0;
                   ifMULTIPLIERALUF3212RRh10shot2 <= ifMULTIPLIERALUF3212RRh10shot1;
                   ifMULTIPLIERALUF3212RRh10shot3 <= ifMULTIPLIERALUF3212RRh10shot2;
                   ifMULTIPLIERALUF3212RRh10shot4 <= ifMULTIPLIERALUF3212RRh10shot3;
                   ifMULTIPLIERALUF3210RRh10shot1 <= ifMULTIPLIERALUF3210RRh10shot0;
                   ifMULTIPLIERALUF3210RRh10shot2 <= ifMULTIPLIERALUF3210RRh10shot1;
                   ifMULTIPLIERALUF3210RRh10shot3 <= ifMULTIPLIERALUF3210RRh10shot2;
                   ifMULTIPLIERALUF3210RRh10shot4 <= ifMULTIPLIERALUF3210RRh10shot3;
                   ifADDERALUF3210RRh10shot1 <= ifADDERALUF3210RRh10shot0;
                   ifADDERALUF3210RRh10shot2 <= ifADDERALUF3210RRh10shot1;
                   ifADDERALUF3210RRh10shot3 <= ifADDERALUF3210RRh10shot2;
                   CVFPCVTFL2F64I6410resulth10shot1 <= CVFPCVTFL2F64I6410resulth10shot0;
                   ifDIVIDERALUF6412RRh10shot1 <= ifDIVIDERALUF6412RRh10shot0;
                   ifDIVIDERALUF6412RRh10shot2 <= ifDIVIDERALUF6412RRh10shot1;
                   ifDIVIDERALUF6412RRh10shot3 <= ifDIVIDERALUF6412RRh10shot2;
                   ifDIVIDERALUF6412RRh10shot4 <= ifDIVIDERALUF6412RRh10shot3;
                   ifADDERALUF6412RRh10shot1 <= ifADDERALUF6412RRh10shot0;
                   ifADDERALUF6412RRh10shot2 <= ifADDERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot3 <= ifADDERALUF6412RRh10shot2;
                   ifMULTIPLIERALUF6412RRh10shot1 <= ifMULTIPLIERALUF6412RRh10shot0;
                   ifMULTIPLIERALUF6412RRh10shot2 <= ifMULTIPLIERALUF6412RRh10shot1;
                   ifMULTIPLIERALUF6410RRh10shot1 <= ifMULTIPLIERALUF6410RRh10shot0;
                   ifMULTIPLIERALUF6410RRh10shot2 <= ifMULTIPLIERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifDIVIDERALUF6410RRh10shot1 <= ifDIVIDERALUF6410RRh10shot0;
                   ifDIVIDERALUF6410RRh10shot2 <= ifDIVIDERALUF6410RRh10shot1;
                   ifDIVIDERALUF6410RRh10shot3 <= ifDIVIDERALUF6410RRh10shot2;
                   ifDIVIDERALUF6410RRh10shot4 <= ifDIVIDERALUF6410RRh10shot3;
                   ifDIVIDERALUF6410RRh10shot0 <= (32'h7e/*126:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifADDERALUF6410RRh10shot0 <= (32'h7a/*122:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   CVFPCVTFL2F64I6410resulth10shot0 <= (32'h66/*102:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h77/*119:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz);

                   ifMULTIPLIERALUF6410RRh10shot0 <= (32'h77/*119:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifMULTIPLIERALUF6412RRh10shot0 <= (32'h72/*114:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifADDERALUF6412RRh10shot0 <= (32'h6e/*110:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifDIVIDERALUF6412RRh10shot0 <= (TESTMAIN400_test58_sqrt_Sqrt_1_12_V_4<32'sd3) && (32'h69/*105:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz);

                   ifDIVIDERALUF3210RRh12shot0 <= (32'h3e/*62:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h1b/*27:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz) || (32'h56/*86:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

                   ifADDERALUF3210RRh10shot0 <= (32'h52/*82:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h3a/*58:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz);

                   CVFPCVTFL2F32I6410resulth10shot0 <= (32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h35/*53:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz);

                   ifMULTIPLIERALUF3210RRh10shot0 <= (32'h35/*53:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifMULTIPLIERALUF3212RRh10shot0 <= (32'h2e/*46:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifADDERALUF3212RRh10shot0 <= (32'h2a/*42:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifMULTIPLIERALUF3214RRh10shot0 <= (32'h16/*22:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifADDERALUF3214RRh10shot0 <= (32'h12/*18:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   ifDIVIDERALUF3210RRh10shot0 <= (32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


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
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_10_RR),
        .XX(ifMULTIPLIERALUF64_10_XX
),
        .YY(ifMULTIPLIERALUF64_10_YY),
        .FAIL(ifMULTIPLIERALUF64_10_FAIL));
  CV_FP_CVT_FL2_F64_I64 CVFPCVTFL2F64I64_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F64I64_10_result),
        .arg(CVFPCVTFL2F64I64_10_arg
),
        .FAIL(CVFPCVTFL2F64I64_10_FAIL));
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_12_RR),
        .XX(ifMULTIPLIERALUF64_12_XX
),
        .YY(ifMULTIPLIERALUF64_12_YY),
        .FAIL(ifMULTIPLIERALUF64_12_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_12_RR),
        .XX(ifADDERALUF64_12_XX
),
        .YY(ifADDERALUF64_12_YY),
        .FAIL(ifADDERALUF64_12_FAIL));
  CV_FP_FL5_DIVIDER_DP ifDIVIDERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifDIVIDERALUF64_12_RR),
        .NN(ifDIVIDERALUF64_12_NN
),
        .DD(ifDIVIDERALUF64_12_DD),
        .FAIL(ifDIVIDERALUF64_12_FAIL));
  CV_FP_FL15_DIVIDER_SP ifDIVIDERALUF32_10(
        .clk(clk),
        .reset(reset),
        .RR(ifDIVIDERALUF32_10_RR),
        .NN(ifDIVIDERALUF32_10_NN
),
        .DD(ifDIVIDERALUF32_10_DD),
        .FAIL(ifDIVIDERALUF32_10_FAIL));
  CV_FP_FL4_ADDER_SP ifADDERALUF32_10(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF32_10_RR),
        .XX(ifADDERALUF32_10_XX
),
        .YY(ifADDERALUF32_10_YY),
        .FAIL(ifADDERALUF32_10_FAIL));
  CV_FP_FL5_MULTIPLIER_SP ifMULTIPLIERALUF32_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF32_10_RR),
        .XX(ifMULTIPLIERALUF32_10_XX
),
        .YY(ifMULTIPLIERALUF32_10_YY),
        .FAIL(ifMULTIPLIERALUF32_10_FAIL));
  CV_FP_CVT_FL2_F32_I64 CVFPCVTFL2F32I64_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2F32I64_10_result),
        .arg(CVFPCVTFL2F32I64_10_arg
),
        .FAIL(CVFPCVTFL2F32I64_10_FAIL));
  CV_FP_FL5_MULTIPLIER_SP ifMULTIPLIERALUF32_12(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF32_12_RR),
        .XX(ifMULTIPLIERALUF32_12_XX
),
        .YY(ifMULTIPLIERALUF32_12_YY),
        .FAIL(ifMULTIPLIERALUF32_12_FAIL));
  CV_FP_FL5_MULTIPLIER_SP ifMULTIPLIERALUF32_14(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF32_14_RR),
        .XX(ifMULTIPLIERALUF32_14_XX
),
        .YY(ifMULTIPLIERALUF32_14_YY),
        .FAIL(ifMULTIPLIERALUF32_14_FAIL));
  CV_FP_FL4_ADDER_SP ifADDERALUF32_12(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF32_12_RR),
        .XX(ifADDERALUF32_12_XX
),
        .YY(ifADDERALUF32_12_YY),
        .FAIL(ifADDERALUF32_12_FAIL));
  CV_FP_FL4_ADDER_SP ifADDERALUF32_14(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF32_14_RR),
        .XX(ifADDERALUF32_14_XX
),
        .YY(ifADDERALUF32_14_YY),
        .FAIL(ifADDERALUF32_14_FAIL));
always @(*) hprpin501158x10 = ((32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F32I64_10_result: CVFPCVTFL2F32I6410resulth10hold);

always @(*) hprpin501163x10 = $unsigned(32'sh1fbb_4000+(hprpin501158x10>>32'sd1));

always @(*) hprpin501304x10 = ((32'h68/*104:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? CVFPCVTFL2F64I64_10_result: CVFPCVTFL2F64I6410resulth10hold);

assign hprpin501316x10 = $unsigned(64'sh1ff8_0000_0000_0000+(hprpin501304x10>>32'sd1))+(0-(rtl_unsigned_extend2(A_UINT_CC_SCALbx10_ARA0[rtl_signed_bitextract3(64'sh1f
&($unsigned(64'sh1ff8_0000_0000_0000+(hprpin501304x10>>32'sd1))>>32'sd47))])<<32'sd32));

always @(*) hprpin501318x10 = hprpin501316x10;

assign hprpin502015x10 = (((32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? hpr_fp_dltd4(32'h0, CVFPCVTFL2F32I64_10_result): hpr_fp_dltd4(32'h0, CVFPCVTFL2F32I6410resulth10hold
))? ((32'h33/*51:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? ifMULTIPLIERALUF32_12_RR: ifMULTIPLIERALUF3212RRh10hold): 32'h0);

 initial        begin 
      //ROM data table: 32 words of 32 bits.
       A_UINT_CC_SCALbx10_ARA0[0] = 32'h0;
       A_UINT_CC_SCALbx10_ARA0[1] = 32'h400;
       A_UINT_CC_SCALbx10_ARA0[2] = 32'hbf6;
       A_UINT_CC_SCALbx10_ARA0[3] = 32'h1672;
       A_UINT_CC_SCALbx10_ARA0[4] = 32'h23e9;
       A_UINT_CC_SCALbx10_ARA0[5] = 32'h3424;
       A_UINT_CC_SCALbx10_ARA0[6] = 32'h46f2;
       A_UINT_CC_SCALbx10_ARA0[7] = 32'h5c28;
       A_UINT_CC_SCALbx10_ARA0[8] = 32'h739e;
       A_UINT_CC_SCALbx10_ARA0[9] = 32'h8d31;
       A_UINT_CC_SCALbx10_ARA0[10] = 32'ha8c2;
       A_UINT_CC_SCALbx10_ARA0[11] = 32'hc634;
       A_UINT_CC_SCALbx10_ARA0[12] = 32'he56d;
       A_UINT_CC_SCALbx10_ARA0[13] = 32'h1_0656;
       A_UINT_CC_SCALbx10_ARA0[14] = 32'h1_28d8;
       A_UINT_CC_SCALbx10_ARA0[15] = 32'h1_4cdf;
       A_UINT_CC_SCALbx10_ARA0[16] = 32'h1_468f;
       A_UINT_CC_SCALbx10_ARA0[17] = 32'h1_16d2;
       A_UINT_CC_SCALbx10_ARA0[18] = 32'hec0c;
       A_UINT_CC_SCALbx10_ARA0[19] = 32'hc5d7;
       A_UINT_CC_SCALbx10_ARA0[20] = 32'ha3d9;
       A_UINT_CC_SCALbx10_ARA0[21] = 32'h85c6;
       A_UINT_CC_SCALbx10_ARA0[22] = 32'h6b56;
       A_UINT_CC_SCALbx10_ARA0[23] = 32'h544d;
       A_UINT_CC_SCALbx10_ARA0[24] = 32'h4073;
       A_UINT_CC_SCALbx10_ARA0[25] = 32'h2f97;
       A_UINT_CC_SCALbx10_ARA0[26] = 32'h218c;
       A_UINT_CC_SCALbx10_ARA0[27] = 32'h162a;
       A_UINT_CC_SCALbx10_ARA0[28] = 32'hd4b;
       A_UINT_CC_SCALbx10_ARA0[29] = 32'h6ce;
       A_UINT_CC_SCALbx10_ARA0[30] = 32'h295;
       A_UINT_CC_SCALbx10_ARA0[31] = 32'h82;
       end 
      

// Structural Resource (FU) inventory for DUT:// 38 vectors of width 64
// 95 vectors of width 32
// 1 vectors of width 8
// 86 vectors of width 1
// 32 array locations of width 32
// Total state bits in module = 6590 bits.
// 815 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL5_DIVIDER_DP count=2
//   cell CV_FP_FL4_ADDER_DP count=2
//   cell CV_FP_FL3_MULTIPLIER_DP count=2
//   cell CV_FP_CVT_FL2_F64_I64 count=1
//   cell CV_FP_FL15_DIVIDER_SP count=1
//   cell CV_FP_FL4_ADDER_SP count=3
//   cell CV_FP_FL5_MULTIPLIER_SP count=3
//   cell CV_FP_CVT_FL2_F32_I64 count=1
// Total number of leaf cells = 15
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:01
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test58r0.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58r0.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Class    | Style   | Dir Style                                                                                            | Timing Target | Method        | UID    | Skip  |
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Test58r0 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | Test58r0.Main | Main10 | false |
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*

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
//KiwiC: front end input processing of class test58_sqrt  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test58_sqrt..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test58_sqrt..cctor
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
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class Test58r0  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test58r0.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Test58r0.Main
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
//   srcfile=test58r0.exe
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
//PC codings points for kiwiTESTMAIN400PC10 
//*----------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"   | 823 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 52   |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 821 | 1       | hwm=0.50.0   | 51   |        | 2     | 51  | 53   |
//| XU32'1:"1:kiwiTESTMAIN400PC10"   | 822 | 52      | hwm=0.0.0    | 52   |        | -     | -   | 1    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 819 | 53      | hwm=0.24.0   | 101  |        | 78    | 101 | 118  |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 820 | 53      | hwm=0.24.0   | 77   |        | 54    | 77  | 52   |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 815 | 102     | hwm=2.2.0    | 104  |        | -     | -   | 105  |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 816 | 102     | hwm=2.2.0    | 104  |        | -     | -   | 119  |
//| XU32'32:"32:kiwiTESTMAIN400PC10" | 813 | 105     | hwm=0.12.0   | 117  |        | 106   | 117 | 105  |
//| XU32'32:"32:kiwiTESTMAIN400PC10" | 814 | 105     | hwm=0.0.0    | 105  |        | -     | -   | 119  |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 817 | 118     | hwm=0.0.0    | 118  |        | -     | -   | 102  |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 818 | 118     | hwm=0.0.0    | 118  |        | -     | -   | -    |
//| XU32'64:"64:kiwiTESTMAIN400PC10" | 812 | 119     | hwm=0.12.0   | 131  |        | 120   | 131 | 118  |
//*----------------------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                     |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                          |
//| F0   | E823 | R0 DATA |                                                                                                                                          |
//| F0+E | E823 | W0 DATA | TESTMAIN400.Test58r0.single_test.0.7.V_0write(S64'2) TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256write(??32'0) TESTMAIN400.Test58r0.\ |
//|      |      |         | double_test.0.11.V_0write(S64'0) TESTMAIN400.Test58r0.double_test.0.11.V_1write(??64'0) TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3write(??64\ |
//|      |      |         | '0) TESTMAIN400.test58_sqrt.Sqrt.1.12.V_4write(S32'0) TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256write(??64'0)  PLI:Test58r0: Single T\ |
//|      |      |         | es...  W/P:Starting Single  PLI:Test58r0 start - sqr...                                                                                  |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*-------+------+----------+--------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                           |
//*-------+------+----------+--------------------------------------------------------------------------------*
//| F1    | -    | R0 CTRL  |                                                                                |
//| F1    | E821 | R0 DATA  | CVFPCVTFL2F32I64_10_hpr_flt_from_int64(E1)                                     |
//| F2    | E821 | R1 DATA  |                                                                                |
//| F3    | E821 | R2 DATA  | ifDIVIDERALUF32_10_compute(E2, E3)                                             |
//| F4    | E821 | R3 DATA  |                                                                                |
//| F5    | E821 | R4 DATA  |                                                                                |
//| F6    | E821 | R5 DATA  |                                                                                |
//| F7    | E821 | R6 DATA  |                                                                                |
//| F8    | E821 | R7 DATA  |                                                                                |
//| F9    | E821 | R8 DATA  |                                                                                |
//| F10   | E821 | R9 DATA  |                                                                                |
//| F11   | E821 | R10 DATA |                                                                                |
//| F12   | E821 | R11 DATA |                                                                                |
//| F13   | E821 | R12 DATA |                                                                                |
//| F14   | E821 | R13 DATA |                                                                                |
//| F15   | E821 | R14 DATA |                                                                                |
//| F16   | E821 | R15 DATA |                                                                                |
//| F17   | E821 | R16 DATA |                                                                                |
//| F18   | E821 | R17 DATA | ifADDERALUF32_14_compute(E3, E4)                                               |
//| F19   | E821 | R18 DATA |                                                                                |
//| F20   | E821 | R19 DATA |                                                                                |
//| F21   | E821 | R20 DATA |                                                                                |
//| F22   | E821 | R21 DATA | ifMULTIPLIERALUF32_14_compute(0.5f, E5)                                        |
//| F23   | E821 | R22 DATA |                                                                                |
//| F24   | E821 | R23 DATA |                                                                                |
//| F25   | E821 | R24 DATA |                                                                                |
//| F26   | E821 | R25 DATA |                                                                                |
//| F27   | E821 | R26 DATA | ifDIVIDERALUF32_10_compute(E2, E6)                                             |
//| F28   | E821 | R27 DATA |                                                                                |
//| F29   | E821 | R28 DATA |                                                                                |
//| F30   | E821 | R29 DATA |                                                                                |
//| F31   | E821 | R30 DATA |                                                                                |
//| F32   | E821 | R31 DATA |                                                                                |
//| F33   | E821 | R32 DATA |                                                                                |
//| F34   | E821 | R33 DATA |                                                                                |
//| F35   | E821 | R34 DATA |                                                                                |
//| F36   | E821 | R35 DATA |                                                                                |
//| F37   | E821 | R36 DATA |                                                                                |
//| F38   | E821 | R37 DATA |                                                                                |
//| F39   | E821 | R38 DATA |                                                                                |
//| F40   | E821 | R39 DATA |                                                                                |
//| F41   | E821 | R40 DATA |                                                                                |
//| F42   | E821 | R41 DATA | ifADDERALUF32_12_compute(E6, E7)                                               |
//| F43   | E821 | R42 DATA |                                                                                |
//| F44   | E821 | R43 DATA |                                                                                |
//| F45   | E821 | R44 DATA |                                                                                |
//| F46   | E821 | R45 DATA | ifMULTIPLIERALUF32_12_compute(0.5f, E8)                                        |
//| F47   | E821 | R46 DATA |                                                                                |
//| F48   | E821 | R47 DATA |                                                                                |
//| F49   | E821 | R48 DATA |                                                                                |
//| F50   | E821 | R49 DATA |                                                                                |
//| F51   | E821 | R50 DATA |                                                                                |
//| F51+E | E821 | W0 DATA  | TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256write(E9)  PLI:    p=%d   pf=%f |
//*-------+------+----------+--------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F52   | -    | R0 CTRL |      |
//| F52   | E822 | R0 DATA |      |
//| F52+E | E822 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*--------+------+----------+----------------------------------------------------------------------------------------------------------------------------*
//| pc     | eno  | Phaser   | Work                                                                                                                       |
//*--------+------+----------+----------------------------------------------------------------------------------------------------------------------------*
//| F53    | -    | R0 CTRL  |                                                                                                                            |
//| F53    | E820 | R0 DATA  | ifMULTIPLIERALUF32_10_compute(E10, E11) CVFPCVTFL2F32I64_10_hpr_flt_from_int64(E1)                                         |
//| F54    | E820 | R1 DATA  |                                                                                                                            |
//| F55    | E820 | R2 DATA  |                                                                                                                            |
//| F56    | E820 | R3 DATA  |                                                                                                                            |
//| F57    | E820 | R4 DATA  |                                                                                                                            |
//| F58    | E820 | R5 DATA  | ifADDERALUF32_10_compute(E2, E12)                                                                                          |
//| F59    | E820 | R6 DATA  |                                                                                                                            |
//| F60    | E820 | R7 DATA  |                                                                                                                            |
//| F61    | E820 | R8 DATA  |                                                                                                                            |
//| F62    | E820 | R9 DATA  | ifDIVIDERALUF32_10_compute(E13, E2)                                                                                        |
//| F63    | E820 | R10 DATA |                                                                                                                            |
//| F64    | E820 | R11 DATA |                                                                                                                            |
//| F65    | E820 | R12 DATA |                                                                                                                            |
//| F66    | E820 | R13 DATA |                                                                                                                            |
//| F67    | E820 | R14 DATA |                                                                                                                            |
//| F68    | E820 | R15 DATA |                                                                                                                            |
//| F69    | E820 | R16 DATA |                                                                                                                            |
//| F70    | E820 | R17 DATA |                                                                                                                            |
//| F71    | E820 | R18 DATA |                                                                                                                            |
//| F72    | E820 | R19 DATA |                                                                                                                            |
//| F73    | E820 | R20 DATA |                                                                                                                            |
//| F74    | E820 | R21 DATA |                                                                                                                            |
//| F75    | E820 | R22 DATA |                                                                                                                            |
//| F76    | E820 | R23 DATA |                                                                                                                            |
//| F77    | E820 | R24 DATA |                                                                                                                            |
//| F77+E  | E820 | W0 DATA  | TESTMAIN400.Test58r0.single_test.0.7.V_0write(E14)  PLI:         root sp=%f ...                                            |
//| F53    | E819 | R0 DATA  | ifMULTIPLIERALUF32_10_compute(E10, E11) CVFPCVTFL2F32I64_10_hpr_flt_from_int64(E1)                                         |
//| F78    | E819 | R1 DATA  |                                                                                                                            |
//| F79    | E819 | R2 DATA  |                                                                                                                            |
//| F80    | E819 | R3 DATA  |                                                                                                                            |
//| F81    | E819 | R4 DATA  |                                                                                                                            |
//| F82    | E819 | R5 DATA  | ifADDERALUF32_10_compute(E2, E12)                                                                                          |
//| F83    | E819 | R6 DATA  |                                                                                                                            |
//| F84    | E819 | R7 DATA  |                                                                                                                            |
//| F85    | E819 | R8 DATA  |                                                                                                                            |
//| F86    | E819 | R9 DATA  | ifDIVIDERALUF32_10_compute(E13, E2)                                                                                        |
//| F87    | E819 | R10 DATA |                                                                                                                            |
//| F88    | E819 | R11 DATA |                                                                                                                            |
//| F89    | E819 | R12 DATA |                                                                                                                            |
//| F90    | E819 | R13 DATA |                                                                                                                            |
//| F91    | E819 | R14 DATA |                                                                                                                            |
//| F92    | E819 | R15 DATA |                                                                                                                            |
//| F93    | E819 | R16 DATA |                                                                                                                            |
//| F94    | E819 | R17 DATA |                                                                                                                            |
//| F95    | E819 | R18 DATA |                                                                                                                            |
//| F96    | E819 | R19 DATA |                                                                                                                            |
//| F97    | E819 | R20 DATA |                                                                                                                            |
//| F98    | E819 | R21 DATA |                                                                                                                            |
//| F99    | E819 | R22 DATA |                                                                                                                            |
//| F100   | E819 | R23 DATA |                                                                                                                            |
//| F101   | E819 | R24 DATA |                                                                                                                            |
//| F101+E | E819 | W0 DATA  | TESTMAIN400.Test58r0.single_test.0.7.V_0write(E14) TESTMAIN400.Test58r0.double_test.0.11.V_0write(S64'2)  PLI:Test58r1: D\ |
//|        |      |          | ouble Tes...  W/P:Starting Double  PLI:         root sp=%f ...                                                             |
//*--------+------+----------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*--------+------+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                                                                               |
//*--------+------+---------+--------------------------------------------------------------------------------------------------------------------*
//| F102   | -    | R0 CTRL | CVFPCVTFL2F64I64_10_hpr_dbl_from_int64(E15)                                                                        |
//| F103   | -    | R1 CTRL |                                                                                                                    |
//| F104   | -    | R2 CTRL |                                                                                                                    |
//| F102   | E816 | R0 DATA |                                                                                                                    |
//| F103   | E816 | R1 DATA |                                                                                                                    |
//| F104   | E816 | R2 DATA |                                                                                                                    |
//| F104+E | E816 | W0 DATA | TESTMAIN400.Test58r0.double_test.0.11.V_1write(E16) TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256write(??64'0)  PL\ |
//|        |      |         | I:    p=%d   pf=%f                                                                                                 |
//| F102   | E815 | R0 DATA |                                                                                                                    |
//| F103   | E815 | R1 DATA |                                                                                                                    |
//| F104   | E815 | R2 DATA |                                                                                                                    |
//| F104+E | E815 | W0 DATA | TESTMAIN400.Test58r0.double_test.0.11.V_1write(E16) TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3write(E17) TESTMAIN400.t\ |
//|        |      |         | est58_sqrt.Sqrt.1.12.V_4write(S32'0)  PLI:    p=%d   pf=%f                                                         |
//*--------+------+---------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*--------+------+----------+-------------------------------------------------------------------------------------------------*
//| pc     | eno  | Phaser   | Work                                                                                            |
//*--------+------+----------+-------------------------------------------------------------------------------------------------*
//| F105   | -    | R0 CTRL  |                                                                                                 |
//| F105   | E814 | R0 DATA  |                                                                                                 |
//| F105+E | E814 | W0 DATA  | TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256write(E18)                                          |
//| F105   | E813 | R0 DATA  | ifDIVIDERALUF64_12_compute(E19, E20)                                                            |
//| F106   | E813 | R1 DATA  |                                                                                                 |
//| F107   | E813 | R2 DATA  |                                                                                                 |
//| F108   | E813 | R3 DATA  |                                                                                                 |
//| F109   | E813 | R4 DATA  |                                                                                                 |
//| F110   | E813 | R5 DATA  | ifADDERALUF64_12_compute(E20, E21)                                                              |
//| F111   | E813 | R6 DATA  |                                                                                                 |
//| F112   | E813 | R7 DATA  |                                                                                                 |
//| F113   | E813 | R8 DATA  |                                                                                                 |
//| F114   | E813 | R9 DATA  | ifMULTIPLIERALUF64_12_compute(0.5, E22)                                                         |
//| F115   | E813 | R10 DATA |                                                                                                 |
//| F116   | E813 | R11 DATA |                                                                                                 |
//| F117   | E813 | R12 DATA |                                                                                                 |
//| F117+E | E813 | W0 DATA  | TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3write(E23) TESTMAIN400.test58_sqrt.Sqrt.1.12.V_4write(E24) |
//*--------+------+----------+-------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*--------+------+---------+-----------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                      |
//*--------+------+---------+-----------------------------------------------------------*
//| F118   | -    | R0 CTRL |                                                           |
//| F118   | E818 | R0 DATA |                                                           |
//| F118+E | E818 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:Test58 finished.  W/P:Finished |
//| F118   | E817 | R0 DATA |                                                           |
//| F118+E | E817 | W0 DATA |                                                           |
//*--------+------+---------+-----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//*--------+------+----------+-------------------------------------------------------------------------------------*
//| pc     | eno  | Phaser   | Work                                                                                |
//*--------+------+----------+-------------------------------------------------------------------------------------*
//| F119   | -    | R0 CTRL  |                                                                                     |
//| F119   | E812 | R0 DATA  | ifMULTIPLIERALUF64_10_compute(E25, E26) CVFPCVTFL2F64I64_10_hpr_dbl_from_int64(E15) |
//| F120   | E812 | R1 DATA  |                                                                                     |
//| F121   | E812 | R2 DATA  |                                                                                     |
//| F122   | E812 | R3 DATA  | ifADDERALUF64_10_compute(E16, E27)                                                  |
//| F123   | E812 | R4 DATA  |                                                                                     |
//| F124   | E812 | R5 DATA  |                                                                                     |
//| F125   | E812 | R6 DATA  |                                                                                     |
//| F126   | E812 | R7 DATA  | ifDIVIDERALUF64_10_compute(E28, E16)                                                |
//| F127   | E812 | R8 DATA  |                                                                                     |
//| F128   | E812 | R9 DATA  |                                                                                     |
//| F129   | E812 | R10 DATA |                                                                                     |
//| F130   | E812 | R11 DATA |                                                                                     |
//| F131   | E812 | R12 DATA |                                                                                     |
//| F131+E | E812 | W0 DATA  | TESTMAIN400.Test58r0.double_test.0.11.V_0write(E29)  PLI:         root dp=%f ...    |
//*--------+------+----------+-------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= TESTMAIN400.Test58r0.single_test.0.7.V_0
//
//
//  E2 =.= CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0)
//
//
//  E3 =.= Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))
//
//
//  E4 =.= Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))))
//
//
//  E5 =.= (Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))))
//
//
//  E6 =.= Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))))))
//
//
//  E7 =.= (CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))))))))
//
//
//  E8 =.= (Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))))))))+(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))))))))
//
//
//  E9 =.= COND(0f<(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0)), Cf(0.5f*((Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1))))))))))+(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))+(Cf((CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))))>>>1)))))))))))), ??32'0)
//
//
//  E10 =.= Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256)
//
//
//  E11 =.= -(Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//
//  E12 =.= (Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256))*-(Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//
//  E13 =.= (CVT(Cf)(TESTMAIN400.Test58r0.single_test.0.7.V_0))+(Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256))*-(Cf(TESTMAIN400.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//
//  E14 =.= C64(S64'204*TESTMAIN400.Test58r0.single_test.0.7.V_0)
//
//
//  E15 =.= TESTMAIN400.Test58r0.double_test.0.11.V_0
//
//
//  E16 =.= CVT(C64f)(TESTMAIN400.Test58r0.double_test.0.11.V_0)
//
//
//  E17 =.= C64f(*APPLY:hpr_bitsToDouble(C64u((C64u(S64'2303591209400008704+((C64u(*APPLY:hpr_doubleToBits(CVT(C64f)(TESTMAIN400.Test58r0.double_test.0.11.V_0))))>>>1)))+-((CVT(C64u)(@_UINT/CC/SCALbx10_ARA0[CVT(C)(S64'31&(C64u(S64'2303591209400008704+((C64u(*APPLY:hpr_doubleToBits(CVT(C64f)(TESTMAIN400.Test58r0.double_test.0.11.V_0))))>>>1)))>>>47)]))<<32))))
//
//
//  E18 =.= C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3)
//
//
//  E19 =.= TESTMAIN400.Test58r0.double_test.0.11.V_1
//
//
//  E20 =.= TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3
//
//
//  E21 =.= TESTMAIN400.Test58r0.double_test.0.11.V_1/TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3
//
//
//  E22 =.= TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3+TESTMAIN400.Test58r0.double_test.0.11.V_1/TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3
//
//
//  E23 =.= C64f(0.5*(TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3+TESTMAIN400.Test58r0.double_test.0.11.V_1/TESTMAIN400.test58_sqrt.Sqrt.1.12.V_3))
//
//
//  E24 =.= C(1+TESTMAIN400.test58_sqrt.Sqrt.1.12.V_4)
//
//
//  E25 =.= C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256)
//
//
//  E26 =.= -(C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//
//  E27 =.= (C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256))*-(C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//
//  E28 =.= (CVT(C64f)(TESTMAIN400.Test58r0.double_test.0.11.V_0))+(C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256))*-(C64f(TESTMAIN400.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//
//  E29 =.= C64(S64'204*TESTMAIN400.Test58r0.double_test.0.11.V_0)
//
//
//  E30 =.= (C64(S64'204*TESTMAIN400.Test58r0.single_test.0.7.V_0))<S64'1000000000000000
//
//
//  E31 =.= (C64(S64'204*TESTMAIN400.Test58r0.single_test.0.7.V_0))>=S64'1000000000000000
//
//
//  E32 =.= {[XU32'104:"104:kiwiTESTMAIN400PC10nz"==kiwiTESTMAIN400PC10nz, 0>=CVFPCVTFL2F64I64_10_result]; [XU32'104:"104:kiwiTESTMAIN400PC10nz"!=kiwiTESTMAIN400PC10nz, 0>=CVFPCVTFL2F64I6410resulth10hold]}
//
//
//  E33 =.= {[XU32'104:"104:kiwiTESTMAIN400PC10nz"==kiwiTESTMAIN400PC10nz, 0<CVFPCVTFL2F64I64_10_result]; [XU32'104:"104:kiwiTESTMAIN400PC10nz"!=kiwiTESTMAIN400PC10nz, 0<CVFPCVTFL2F64I6410resulth10hold]}
//
//
//  E34 =.= TESTMAIN400.test58_sqrt.Sqrt.1.12.V_4>=3
//
//
//  E35 =.= TESTMAIN400.test58_sqrt.Sqrt.1.12.V_4<3
//
//
//  E36 =.= TESTMAIN400.Test58r0.double_test.0.11.V_0>=S64'1000000000000000
//
//
//  E37 =.= TESTMAIN400.Test58r0.double_test.0.11.V_0<S64'1000000000000000
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test58r0 to test58r0

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//38 vectors of width 64
//
//95 vectors of width 32
//
//1 vectors of width 8
//
//86 vectors of width 1
//
//32 array locations of width 32
//
//Total state bits in module = 6590 bits.
//
//815 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread test58_sqrt..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread Test58r0.Main uid=Main10 has 82 CIL instructions in 16 basic blocks
//Thread mpc10 has 8 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 132 minor control states
// eof (HPR L/S Verilog)
