

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:50:09
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -obj-dir-name=. -res2-loglevel=0 -log-dir-name=d_obj_sw_test_b sw_test.exe -vnl=sw_test -vnl-rootmodname=DUT -vnl-resets=synchronous
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg signed [63:0] d_monitor,
    output reg [31:0] phase,
    
/* portgroup= abstractionName=res2-directornets */
output reg [6:0] kiwiSWTMAIN400PC10nz_pc_export,
    
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

function signed [31:0] rtl_signed_bitextract5;
   input [63:0] arg;
   rtl_signed_bitextract5 = $signed(arg[31:0]);
   endfunction


function [31:0] rtl_unsigned_bitextract2;
   input [63:0] arg;
   rtl_unsigned_bitextract2 = $unsigned(arg[31:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract1;
   input [31:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[7:0]);
   endfunction


function signed [63:0] rtl_sign_extend4;
   input [31:0] arg;
   rtl_sign_extend4 = { {32{arg[31]}}, arg[31:0] };
   endfunction


function signed [63:0] rtl_sign_extend3;
   input [31:0] arg;
   rtl_sign_extend3 = { {32{arg[31]}}, arg[31:0] };
   endfunction


function [63:0] rtl_unsigned_extend7;
   input [31:0] arg;
   rtl_unsigned_extend7 = { 32'b0, arg[31:0] };
   endfunction


function [63:0] rtl_unsigned_extend6;
   input [31:0] arg;
   rtl_unsigned_extend6 = { 32'b0, arg[31:0] };
   endfunction


function [31:0] rtl_unsigned_extend0;
   input [7:0] arg;
   rtl_unsigned_extend0 = { 24'b0, arg[7:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_44_V_0;
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_40_V_0;
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_39_V_0;
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_31_V_0;
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_23_V_0;
  reg [7:0] SW_TMAIN400_CRC32checker_process_byte_0_15_V_0;
  reg signed [63:0] SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256;
  reg signed [63:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_4;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_2;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1;
  reg signed [31:0] SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0;
// abstractionName=repack-newnets
  reg [31:0] A_UINT_CC_SCALbx12_crcreg10;
  reg [31:0] A_UINT_CC_SCALbx14_crcreg10;
  reg [31:0] A_UINT_CC_SCALbx14_byteno10;
  reg signed [15:0] A_16_SS_CC_SCALbx18_ARB0[19:0];
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_10_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_10_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_12_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_12_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_14_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_14_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_16_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_16_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_18_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_18_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_SINT/CC/SCALbx22_ARA0
  wire signed [31:0] SINTCCSCALbx22ARA0_10_rdata;
  reg [8:0] SINTCCSCALbx22ARA0_10_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_20_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_20_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_22_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_22_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_24_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_24_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_26_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_26_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_28_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_28_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_30_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_30_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@_UINT/CC/SCALbx10_ARA0
  wire [31:0] UINTCCSCALbx10ARA0_32_rdata;
  reg [7:0] UINTCCSCALbx10ARA0_32_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@16_SS/CC/SCALbx16_ARA0
  wire signed [15:0] Z16SSCCSCALbx16ARA0_10_rdata;
  reg [6:0] Z16SSCCSCALbx16ARA0_10_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@16_SS/CC/SCALbx20_ARC0
  wire signed [15:0] Z16SSCCSCALbx20ARC0_10_rdata;
  reg [6:0] Z16SSCCSCALbx20ARC0_10_addr;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [63:0] A_64_SS_CC_SCALbx56_ARC0_rdata;
  reg [7:0] A_64_SS_CC_SCALbx56_ARC0_addr;
  reg A_64_SS_CC_SCALbx56_ARC0_wen;
  reg A_64_SS_CC_SCALbx56_ARC0_ren;
  reg signed [63:0] A_64_SS_CC_SCALbx56_ARC0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [63:0] A_64_SS_CC_SCALbx58_ARD0_rdata;
  reg [7:0] A_64_SS_CC_SCALbx58_ARD0_addr;
  reg A_64_SS_CC_SCALbx58_ARD0_wen;
  reg A_64_SS_CC_SCALbx58_ARD0_ren;
  reg signed [63:0] A_64_SS_CC_SCALbx58_ARD0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [63:0] A_64_SS_CC_SCALbx52_ARA0_rdata;
  reg [7:0] A_64_SS_CC_SCALbx52_ARA0_addr;
  reg A_64_SS_CC_SCALbx52_ARA0_wen;
  reg A_64_SS_CC_SCALbx52_ARA0_ren;
  reg signed [63:0] A_64_SS_CC_SCALbx52_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [63:0] A_64_SS_CC_SCALbx54_ARB0_rdata;
  reg [7:0] A_64_SS_CC_SCALbx54_ARB0_addr;
  reg A_64_SS_CC_SCALbx54_ARB0_wen;
  reg A_64_SS_CC_SCALbx54_ARB0_ren;
  reg signed [63:0] A_64_SS_CC_SCALbx54_ARB0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx24_ARB0_rdata;
  reg [6:0] A_SINT_CC_SCALbx24_ARB0_addr;
  reg A_SINT_CC_SCALbx24_ARB0_wen;
  reg A_SINT_CC_SCALbx24_ARB0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx24_ARB0_wdata;
// abstractionName=res2-morenets
  reg [31:0] UINTCCSCALbx10ARA010rdatah10hold;
  reg UINTCCSCALbx10ARA010rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA016rdatah10hold;
  reg UINTCCSCALbx10ARA016rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA014rdatah10hold;
  reg UINTCCSCALbx10ARA014rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA012rdatah10hold;
  reg UINTCCSCALbx10ARA012rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA018rdatah10hold;
  reg UINTCCSCALbx10ARA018rdatah10shot0;
  reg signed [15:0] Z16SSCCSCALbx16ARA010rdatah10hold;
  reg Z16SSCCSCALbx16ARA010rdatah10shot0;
  reg signed [31:0] SINTCCSCALbx22ARA010rdatah10hold;
  reg SINTCCSCALbx22ARA010rdatah10shot0;
  reg signed [63:0] Z64SSCCSCALbx58ARD0rdatah10hold;
  reg Z64SSCCSCALbx58ARD0rdatah10shot0;
  reg signed [31:0] SINTCCSCALbx24ARB0rdatah10hold;
  reg SINTCCSCALbx24ARB0rdatah10shot0;
  reg signed [63:0] Z64SSCCSCALbx54ARB0rdatah10hold;
  reg Z64SSCCSCALbx54ARB0rdatah10shot0;
  reg signed [63:0] Z64SSCCSCALbx56ARC0rdatah10hold;
  reg Z64SSCCSCALbx56ARC0rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA020rdatah10hold;
  reg UINTCCSCALbx10ARA020rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA026rdatah10hold;
  reg UINTCCSCALbx10ARA026rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA024rdatah10hold;
  reg UINTCCSCALbx10ARA024rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA030rdatah10hold;
  reg UINTCCSCALbx10ARA030rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA028rdatah10hold;
  reg UINTCCSCALbx10ARA028rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA032rdatah10hold;
  reg UINTCCSCALbx10ARA032rdatah10shot0;
  reg [31:0] UINTCCSCALbx10ARA022rdatah10hold;
  reg UINTCCSCALbx10ARA022rdatah10shot0;
  reg signed [63:0] Z64SSCCSCALbx52ARA0rdatah10hold;
  reg Z64SSCCSCALbx52ARA0rdatah10shot0;
  reg signed [15:0] Z16SSCCSCALbx20ARC010rdatah10hold;
  reg Z16SSCCSCALbx20ARC010rdatah10shot0;
  reg [6:0] kiwiSWTMAIN400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire [31:0] hprpin507125x10;
  wire [31:0] hprpin507143x10;
  wire [31:0] hprpin507161x10;
  wire signed [63:0] hprpin507268x10;
  wire signed [63:0] hprpin507613x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.SW_TMAIN400/1.0
      if (reset)  begin 
               d_monitor <= 64'd0;
               A_UINT_CC_SCALbx14_crcreg10 <= 32'd0;
               A_UINT_CC_SCALbx12_crcreg10 <= 32'd0;
               kiwiSWTMAIN400PC10nz <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_44_V_0 <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_40_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0 <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_39_V_0 <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_31_V_0 <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_23_V_0 <= 32'd0;
               SW_TMAIN400_CRC32checker_process_byte_0_15_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2 <= 64'd0;
               SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_4 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_2 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7 <= 64'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256 <= 32'd0;
               A_UINT_CC_SCALbx14_byteno10 <= 32'd0;
               phase <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0 <= 32'd0;
               SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiSWTMAIN400PC10nz)
                  32'h0/*0:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("Smith Waterman Simple Test Start. Iterations=%1d", 32'sd1);
                          $display("waypoint %1d %1d", 32'sd2, 32'sd0);
                           kiwiSWTMAIN400PC10nz <= 32'h3/*3:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_CRC32checker_process_byte_0_44_V_0 <= 32'h0;
                           SW_TMAIN400_CRC32checker_process_byte_0_40_V_0 <= 32'h0;
                           SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0 <= 32'sh0;
                           SW_TMAIN400_CRC32checker_process_byte_0_39_V_0 <= 32'h0;
                           SW_TMAIN400_CRC32checker_process_byte_0_31_V_0 <= 32'h0;
                           SW_TMAIN400_CRC32checker_process_byte_0_23_V_0 <= 32'h0;
                           SW_TMAIN400_CRC32checker_process_byte_0_15_V_0 <= 32'h0;
                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2 <= 64'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_4 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_2 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7 <= 64'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256 <= 32'sh0;
                           A_UINT_CC_SCALbx14_byteno10 <= 32'h0;
                           phase <= 32'h2;
                           SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0 <= 32'sh0;
                           SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0 <= 32'sh0;
                           end 
                          
                  32'h2/*2:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((A_16_SS_CC_SCALbx18_ARB0[SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0
                      ]==((32'h2/*2:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? Z16SSCCSCALbx20ARC0_10_rdata: Z16SSCCSCALbx20ARC010rdatah10hold
                      )))  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h5/*5:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256 <= SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0
                              ;

                               end 
                               else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h4/*4:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0
                              );

                               end 
                              
                  32'h1/*1:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h2/*2:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h3/*3:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1/*1:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h6/*6:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h8/*8:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0)>=32'sh4d)) $display("waypoint %1d %1d"
                              , 32'sd3, 32'sd0);
                               else  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'h7/*7:kiwiSWTMAIN400PC10nz*/;
                                   SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0
                                  );

                                   SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0 <= 32'sh0;
                                   end 
                                  if (($signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0)>=32'sh4d))  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'h6/*6:kiwiSWTMAIN400PC10nz*/;
                                   SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0 <= 32'sh0;
                                   phase <= 32'h3;
                                   SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0
                                  );

                                   end 
                                   end 
                          
                  32'h4/*4:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_V_0
                      <32'sd20))  kiwiSWTMAIN400PC10nz <= 32'h3/*3:kiwiSWTMAIN400PC10nz*/;
                           else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h5/*5:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256 <= 32'sh0;
                               end 
                              
                  32'h7/*7:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h4/*4:kiwiSWTMAIN400PC10nz*/;
                      
                  32'hb/*11:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'hc/*12:kiwiSWTMAIN400PC10nz*/;
                      
                  32'ha/*10:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2
                      <32'sh4d))  kiwiSWTMAIN400PC10nz <= 32'hb/*11:kiwiSWTMAIN400PC10nz*/;
                           else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h9/*9:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0
                              );

                               end 
                              
                  32'hc/*12:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $write("%1d %1d : %1d   ", SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1, SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2
                          , ((32'hc/*12:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx52_ARA0_rdata: Z64SSCCSCALbx52ARA0rdatah10hold
                          ));
                          $display("");
                           kiwiSWTMAIN400PC10nz <= 32'ha/*10:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2
                          );

                           end 
                          
                  32'hd/*13:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h15/*21:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_0 <= $signed(((32'sd1+SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1
                          )%32'sd2));

                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1 <= 32'sh0;
                           end 
                          
                  32'h8/*8:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0>=32'sd1))  begin 
                                  $display("waypoint %1d %1d", 32'sd12, 32'sd0);
                                  $display("Smith Waterman CRC self check now.");
                                  $display("CRC RESET");
                                  $display("crc reg now:  reg=%1H", 32'hffff_ffff);
                                   end 
                                   else  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'h31/*49:kiwiSWTMAIN400PC10nz*/;
                                   SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0 <= 32'sh0;
                                   end 
                                  if ((SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0>=32'sd1))  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'h34/*52:kiwiSWTMAIN400PC10nz*/;
                                   A_UINT_CC_SCALbx12_crcreg10 <= 32'hffff_ffff;
                                   phase <= 32'hc;
                                   end 
                                   end 
                          
                  32'h10/*16:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h11/*17:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx14_crcreg10 <= $unsigned(-32'd1&(((32'h10/*16:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_32_rdata: UINTCCSCALbx10ARA032rdatah10hold)^((32'h10/*16:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_22_rdata: UINTCCSCALbx10ARA022rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10<<32'sd8)));

                           SW_TMAIN400_CRC32checker_process_byte_0_23_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&hprpin507125x10
                          ));

                           A_UINT_CC_SCALbx14_byteno10 <= $unsigned(32'd1+A_UINT_CC_SCALbx14_byteno10);
                           end 
                          
                  32'h11/*17:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h12/*18:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h12/*18:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h13/*19:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx14_crcreg10 <= $unsigned(-32'd1&(((32'h12/*18:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_30_rdata: UINTCCSCALbx10ARA030rdatah10hold)^((32'h12/*18:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_28_rdata: UINTCCSCALbx10ARA028rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10<<32'sd8)));

                           SW_TMAIN400_CRC32checker_process_byte_0_31_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&hprpin507143x10
                          ));

                           A_UINT_CC_SCALbx14_byteno10 <= $unsigned(32'd1+A_UINT_CC_SCALbx14_byteno10);
                           end 
                          
                  32'h13/*19:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h14/*20:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h14/*20:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h17/*23:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx14_crcreg10 <= $unsigned(-32'd1&(((32'h14/*20:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_26_rdata: UINTCCSCALbx10ARA026rdatah10hold)^((32'h14/*20:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_24_rdata: UINTCCSCALbx10ARA024rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10<<32'sd8)));

                           SW_TMAIN400_CRC32checker_process_byte_0_39_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&hprpin507161x10
                          ));

                           A_UINT_CC_SCALbx14_byteno10 <= $unsigned(32'd1+A_UINT_CC_SCALbx14_byteno10);
                           end 
                          
                  32'hf/*15:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h10/*16:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h16/*22:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1<32'sh4d)) $display("process unit word %1d word=%1d"
                              , A_UINT_CC_SCALbx14_byteno10, ((32'h16/*22:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? rtl_unsigned_bitextract2(A_64_SS_CC_SCALbx52_ARA0_rdata
                              ): rtl_unsigned_bitextract2(Z64SSCCSCALbx52ARA0rdatah10hold)));
                               kiwiSWTMAIN400PC10nz <= 32'hf/*15:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_CRC32checker_process_byte_0_15_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&(A_UINT_CC_SCALbx14_crcreg10
                          >>32'sd24)));

                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2 <= ((32'h16/*22:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? A_64_SS_CC_SCALbx52_ARA0_rdata: Z64SSCCSCALbx52ARA0rdatah10hold);

                           A_UINT_CC_SCALbx14_byteno10 <= $unsigned(32'd1+A_UINT_CC_SCALbx14_byteno10);
                           end 
                          
                  32'h17/*23:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h18/*24:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h15/*21:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1>=32'sh4d)) $display("step %1d crc is %1d    0x%1H"
                              , SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1, A_UINT_CC_SCALbx14_crcreg10, A_UINT_CC_SCALbx14_crcreg10
                              );
                               else  kiwiSWTMAIN400PC10nz <= 32'h16/*22:kiwiSWTMAIN400PC10nz*/;
                          if ((SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1>=32'sh4d))  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'he/*14:kiwiSWTMAIN400PC10nz*/;
                                   d_monitor <= rtl_sign_extend3(A_UINT_CC_SCALbx14_crcreg10);
                                   SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1
                                  );

                                   end 
                                   end 
                          
                  32'h18/*24:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h15/*21:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx14_crcreg10 <= $unsigned(-32'd1&(((32'h18/*24:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_22_rdata: UINTCCSCALbx10ARA022rdatah10hold)^((32'h18/*24:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_20_rdata: UINTCCSCALbx10ARA020rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10<<32'sd8)));

                           SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1
                          );

                           end 
                          
                  32'h1a/*26:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h1b/*27:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_4 <= ((32'h1a/*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? A_SINT_CC_SCALbx24_ARB0_rdata: SINTCCSCALbx24ARB0rdatah10hold);

                           end 
                          
                  32'h1b/*27:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1c/*28:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h1c/*28:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1d/*29:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h1d/*29:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1e/*30:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h1e/*30:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1f/*31:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h1f/*31:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h20/*32:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h20/*32:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h21/*33:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7 <= hprpin507613x10;
                           end 
                          
                  32'h21/*33:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h22/*34:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h22/*34:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (((32'h22/*34:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? (SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7
                          <A_64_SS_CC_SCALbx52_ARA0_rdata): (SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7<Z64SSCCSCALbx52ARA0rdatah10hold
                          )))  SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7 <= ((32'h22/*34:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                              )? A_64_SS_CC_SCALbx52_ARA0_rdata: Z64SSCCSCALbx52ARA0rdatah10hold);

                               kiwiSWTMAIN400PC10nz <= 32'h23/*35:kiwiSWTMAIN400PC10nz*/;
                           end 
                          
                  32'h23/*35:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h24/*36:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h24/*36:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h25/*37:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h25/*37:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h27/*39:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h19/*25:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h1a/*26:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h9/*9:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0>=32'sd2))  begin 
                                  $display("waypoint %1d %1d", 32'sd10, SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                                  $display("CRC RESET");
                                  $display("crc reg now:  reg=%1H", 32'hffff_ffff);
                                   end 
                                   else  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'ha/*10:kiwiSWTMAIN400PC10nz*/;
                                   SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1 <= $signed(((SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0
                                  +SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1)%32'sd2));

                                   SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2 <= 32'sh0;
                                   end 
                                  if ((SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0>=32'sd2))  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'hd/*13:kiwiSWTMAIN400PC10nz*/;
                                   A_UINT_CC_SCALbx14_crcreg10 <= 32'hffff_ffff;
                                   phase <= $signed(32'd10+32'd256*SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                                   end 
                                   end 
                          
                  32'h27/*39:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h28/*40:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h28/*40:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h29/*41:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
                          );

                           end 
                          
                  32'h29/*41:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h26/*38:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h26/*38:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3>=32'sh4d))  begin 
                                  $display("waypoint %1d %1d", 32'sd8, SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                                  $display("Scored h matrix %1d", SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                                   end 
                                   else  kiwiSWTMAIN400PC10nz <= 32'h19/*25:kiwiSWTMAIN400PC10nz*/;
                          if ((SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3>=32'sh4d))  begin 
                                   kiwiSWTMAIN400PC10nz <= 32'h9/*9:kiwiSWTMAIN400PC10nz*/;
                                   SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_0 <= 32'sh0;
                                   phase <= $signed(32'd8+32'd256*SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                                   end 
                                   end 
                          
                  32'h2c/*44:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h2d/*45:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h2a/*42:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h26/*38:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_2 <= SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256
                          ;

                           SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3 <= 32'sh0;
                           end 
                          
                  32'h2b/*43:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0
                      <32'sd20))  kiwiSWTMAIN400PC10nz <= 32'h2c/*44:kiwiSWTMAIN400PC10nz*/;
                           else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h2a/*42:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256 <= 32'sh0;
                               end 
                              
                  32'h2d/*45:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((A_16_SS_CC_SCALbx18_ARB0[SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0
                      ]==((32'h2d/*45:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? Z16SSCCSCALbx16ARA0_10_rdata: Z16SSCCSCALbx16ARA010rdatah10hold
                      )))  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h2a/*42:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_SPILL_256 <= SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0
                              ;

                               end 
                               else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h2b/*43:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0
                              );

                               end 
                              
                  32'he/*14:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1
                      <32'sd3))  begin 
                              $display("waypoint %1d %1d", 32'sd6, SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1);
                              $display("waypoint %1d %1d", 32'sd4, 32'sd0);
                               kiwiSWTMAIN400PC10nz <= 32'h2b/*43:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1 <= $signed(((32'sd1+SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1
                              )%32'sd2));

                               SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0 <= $signed((SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1
                              %32'sd2));

                               SW_TMAIN400_SmithWaterman_Program_t_encode_0_17_V_0 <= 32'sh0;
                               phase <= 32'h4;
                               end 
                               else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h8/*8:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_0
                              );

                               end 
                              
                  32'h2f/*47:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h30/*48:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
                          );

                           end 
                          
                  32'h30/*48:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h2e/*46:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h2e/*46:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
                      <32'sd78))  kiwiSWTMAIN400PC10nz <= 32'h2f/*47:kiwiSWTMAIN400PC10nz*/;
                           else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'he/*14:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1 <= 32'sh0;
                               end 
                              
                  32'h32/*50:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h33/*51:kiwiSWTMAIN400PC10nz*/;
                           d_monitor <= rtl_sign_extend4(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0);
                           SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0 <= $signed(32'sd1+SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
                          );

                           end 
                          
                  32'h31/*49:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18) if ((SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
                      <32'sd2))  kiwiSWTMAIN400PC10nz <= 32'h32/*50:kiwiSWTMAIN400PC10nz*/;
                           else  begin 
                               kiwiSWTMAIN400PC10nz <= 32'h2e/*46:kiwiSWTMAIN400PC10nz*/;
                               SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1 <= 32'sh1;
                               end 
                              
                  32'h33/*51:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h31/*49:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h34/*52:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h35/*53:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h35/*53:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h36/*54:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx12_crcreg10 <= 32'hffff_ffff;
                           end 
                          
                  32'h36/*54:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h37/*55:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h37/*55:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("self test startup %1d a ", ((32'h37/*55:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_18_rdata
                          : UINTCCSCALbx10ARA018rdatah10hold));
                           kiwiSWTMAIN400PC10nz <= 32'h38/*56:kiwiSWTMAIN400PC10nz*/;
                           end 
                          
                  32'h38/*56:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("self test startup %1d b ", 8'hff);
                           kiwiSWTMAIN400PC10nz <= 32'h39/*57:kiwiSWTMAIN400PC10nz*/;
                           end 
                          
                  32'h39/*57:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h3a/*58:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h3a/*58:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("self test startup %1d c ", ((32'h3a/*58:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_18_rdata
                          : UINTCCSCALbx10ARA018rdatah10hold));
                           kiwiSWTMAIN400PC10nz <= 32'h3b/*59:kiwiSWTMAIN400PC10nz*/;
                           end 
                          
                  32'h3b/*59:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h3c/*60:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h3c/*60:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h3d/*61:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx12_crcreg10 <= rtl_unsigned_bitextract2(-32'd1&(32'shff_ffff_ff00^((32'h3c/*60:kiwiSWTMAIN400PC10nz*/==
                          kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_18_rdata: UINTCCSCALbx10ARA018rdatah10hold)^((32'h3c/*60:kiwiSWTMAIN400PC10nz*/==
                          kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_12_rdata: UINTCCSCALbx10ARA012rdatah10hold)));

                           end 
                          
                  32'h3d/*61:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h3e/*62:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_CRC32checker_process_byte_0_40_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&(A_UINT_CC_SCALbx12_crcreg10
                          >>32'sd24)));

                           end 
                          
                  32'h3e/*62:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h3f/*63:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h3f/*63:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h40/*64:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx12_crcreg10 <= $unsigned(-32'd1&(((32'h3f/*63:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_16_rdata: UINTCCSCALbx10ARA016rdatah10hold)^((32'h3f/*63:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_14_rdata: UINTCCSCALbx10ARA014rdatah10hold)^(A_UINT_CC_SCALbx12_crcreg10<<32'sd8)));

                           end 
                          
                  32'h40/*64:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h41/*65:kiwiSWTMAIN400PC10nz*/;
                           SW_TMAIN400_CRC32checker_process_byte_0_44_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd255&(A_UINT_CC_SCALbx12_crcreg10
                          >>32'sd24)));

                           end 
                          
                  32'h41/*65:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiSWTMAIN400PC10nz <= 32'h42/*66:kiwiSWTMAIN400PC10nz*/;
                      
                  32'h42/*66:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiSWTMAIN400PC10nz <= 32'h43/*67:kiwiSWTMAIN400PC10nz*/;
                           A_UINT_CC_SCALbx12_crcreg10 <= $unsigned(-32'd1&(((32'h42/*66:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_12_rdata: UINTCCSCALbx10ARA012rdatah10hold)^((32'h42/*66:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                          )? UINTCCSCALbx10ARA0_10_rdata: UINTCCSCALbx10ARA010rdatah10hold)^(A_UINT_CC_SCALbx12_crcreg10<<32'sd8)));

                           end 
                          
                  32'h43/*67:kiwiSWTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("self test yields %1d (should be 821105832)", A_UINT_CC_SCALbx12_crcreg10);
                          $display("Smith Waterman Simple Test End.");
                          $finish(32'sd0);
                           kiwiSWTMAIN400PC10nz <= 32'h44/*68:kiwiSWTMAIN400PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  begin 
               kiwiSWTMAIN400PC10nz_pc_export <= 32'd0;
               Z16SSCCSCALbx20ARC010rdatah10hold <= 32'd0;
               Z64SSCCSCALbx52ARA0rdatah10hold <= 64'd0;
               UINTCCSCALbx10ARA022rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA032rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA028rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA030rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA024rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA026rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA020rdatah10hold <= 32'd0;
               Z64SSCCSCALbx56ARC0rdatah10hold <= 64'd0;
               Z64SSCCSCALbx54ARB0rdatah10hold <= 64'd0;
               SINTCCSCALbx24ARB0rdatah10hold <= 32'd0;
               Z64SSCCSCALbx58ARD0rdatah10hold <= 64'd0;
               SINTCCSCALbx22ARA010rdatah10hold <= 32'd0;
               Z16SSCCSCALbx16ARA010rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA018rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA012rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA014rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA016rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA010rdatah10hold <= 32'd0;
               UINTCCSCALbx10ARA012rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA010rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA016rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA014rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA018rdatah10shot0 <= 32'd0;
               Z16SSCCSCALbx16ARA010rdatah10shot0 <= 32'd0;
               Z64SSCCSCALbx52ARA0rdatah10shot0 <= 32'd0;
               SINTCCSCALbx22ARA010rdatah10shot0 <= 32'd0;
               Z64SSCCSCALbx58ARD0rdatah10shot0 <= 32'd0;
               Z64SSCCSCALbx56ARC0rdatah10shot0 <= 32'd0;
               Z64SSCCSCALbx54ARB0rdatah10shot0 <= 32'd0;
               SINTCCSCALbx24ARB0rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA022rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA020rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA026rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA024rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA030rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA028rdatah10shot0 <= 32'd0;
               UINTCCSCALbx10ARA032rdatah10shot0 <= 32'd0;
               Z16SSCCSCALbx20ARC010rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (UINTCCSCALbx10ARA010rdatah10shot0)  UINTCCSCALbx10ARA010rdatah10hold <= UINTCCSCALbx10ARA0_10_rdata;
                      if (UINTCCSCALbx10ARA016rdatah10shot0)  UINTCCSCALbx10ARA016rdatah10hold <= UINTCCSCALbx10ARA0_16_rdata;
                      if (UINTCCSCALbx10ARA014rdatah10shot0)  UINTCCSCALbx10ARA014rdatah10hold <= UINTCCSCALbx10ARA0_14_rdata;
                      if (UINTCCSCALbx10ARA012rdatah10shot0)  UINTCCSCALbx10ARA012rdatah10hold <= UINTCCSCALbx10ARA0_12_rdata;
                      if (UINTCCSCALbx10ARA018rdatah10shot0)  UINTCCSCALbx10ARA018rdatah10hold <= UINTCCSCALbx10ARA0_18_rdata;
                      if (Z16SSCCSCALbx16ARA010rdatah10shot0)  Z16SSCCSCALbx16ARA010rdatah10hold <= Z16SSCCSCALbx16ARA0_10_rdata;
                      if (SINTCCSCALbx22ARA010rdatah10shot0)  SINTCCSCALbx22ARA010rdatah10hold <= SINTCCSCALbx22ARA0_10_rdata;
                      if (Z64SSCCSCALbx58ARD0rdatah10shot0)  Z64SSCCSCALbx58ARD0rdatah10hold <= A_64_SS_CC_SCALbx58_ARD0_rdata;
                      if (SINTCCSCALbx24ARB0rdatah10shot0)  SINTCCSCALbx24ARB0rdatah10hold <= A_SINT_CC_SCALbx24_ARB0_rdata;
                      if (Z64SSCCSCALbx54ARB0rdatah10shot0)  Z64SSCCSCALbx54ARB0rdatah10hold <= A_64_SS_CC_SCALbx54_ARB0_rdata;
                      if (Z64SSCCSCALbx56ARC0rdatah10shot0)  Z64SSCCSCALbx56ARC0rdatah10hold <= A_64_SS_CC_SCALbx56_ARC0_rdata;
                      if (UINTCCSCALbx10ARA020rdatah10shot0)  UINTCCSCALbx10ARA020rdatah10hold <= UINTCCSCALbx10ARA0_20_rdata;
                      if (UINTCCSCALbx10ARA026rdatah10shot0)  UINTCCSCALbx10ARA026rdatah10hold <= UINTCCSCALbx10ARA0_26_rdata;
                      if (UINTCCSCALbx10ARA024rdatah10shot0)  UINTCCSCALbx10ARA024rdatah10hold <= UINTCCSCALbx10ARA0_24_rdata;
                      if (UINTCCSCALbx10ARA030rdatah10shot0)  UINTCCSCALbx10ARA030rdatah10hold <= UINTCCSCALbx10ARA0_30_rdata;
                      if (UINTCCSCALbx10ARA028rdatah10shot0)  UINTCCSCALbx10ARA028rdatah10hold <= UINTCCSCALbx10ARA0_28_rdata;
                      if (UINTCCSCALbx10ARA032rdatah10shot0)  UINTCCSCALbx10ARA032rdatah10hold <= UINTCCSCALbx10ARA0_32_rdata;
                      if (UINTCCSCALbx10ARA022rdatah10shot0)  UINTCCSCALbx10ARA022rdatah10hold <= UINTCCSCALbx10ARA0_22_rdata;
                      if (Z64SSCCSCALbx52ARA0rdatah10shot0)  Z64SSCCSCALbx52ARA0rdatah10hold <= A_64_SS_CC_SCALbx52_ARA0_rdata;
                      if (Z16SSCCSCALbx20ARC010rdatah10shot0)  Z16SSCCSCALbx20ARC010rdatah10hold <= Z16SSCCSCALbx20ARC0_10_rdata;
                       kiwiSWTMAIN400PC10nz_pc_export <= kiwiSWTMAIN400PC10nz;
                   UINTCCSCALbx10ARA012rdatah10shot0 <= (32'h3b/*59:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h41/*65:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz);

                   UINTCCSCALbx10ARA010rdatah10shot0 <= (32'h41/*65:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA016rdatah10shot0 <= (32'h3e/*62:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA014rdatah10shot0 <= (32'h3e/*62:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA018rdatah10shot0 <= (32'h3b/*59:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h36/*54:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz) || (32'h39/*57:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);

                   Z16SSCCSCALbx16ARA010rdatah10shot0 <= (32'h2c/*44:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   Z64SSCCSCALbx52ARA0rdatah10shot0 <= (32'h27/*39:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1
                  <32'sh4d) && (32'h15/*21:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'hb/*11:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
                  ) || (32'h21/*33:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);

                   SINTCCSCALbx22ARA010rdatah10shot0 <= (32'h23/*35:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   Z64SSCCSCALbx58ARD0rdatah10shot0 <= (32'h1c/*28:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1f/*31:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz);

                   Z64SSCCSCALbx56ARC0rdatah10shot0 <= (32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1f/*31:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz);

                   Z64SSCCSCALbx54ARB0rdatah10shot0 <= (32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1c/*28:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz);

                   SINTCCSCALbx24ARB0rdatah10shot0 <= (32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA022rdatah10shot0 <= (32'hf/*15:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h17/*23:kiwiSWTMAIN400PC10nz*/==
                  kiwiSWTMAIN400PC10nz);

                   UINTCCSCALbx10ARA020rdatah10shot0 <= (32'h17/*23:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA026rdatah10shot0 <= (32'h13/*19:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA024rdatah10shot0 <= (32'h13/*19:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA030rdatah10shot0 <= (32'h11/*17:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA028rdatah10shot0 <= (32'h11/*17:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   UINTCCSCALbx10ARA032rdatah10shot0 <= (32'hf/*15:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   Z16SSCCSCALbx20ARC010rdatah10shot0 <= (32'h1/*1:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.SW_TMAIN400/1.0


       end 
      

 always   @(* )  begin 
       Z16SSCCSCALbx20ARC0_10_addr = 32'sd0;
       A_SINT_CC_SCALbx24_ARB0_addr = 32'sd0;
       A_SINT_CC_SCALbx24_ARB0_wdata = 32'sd0;
       A_64_SS_CC_SCALbx52_ARA0_addr = 32'sd0;
       UINTCCSCALbx10ARA0_30_addr = 32'sd0;
       UINTCCSCALbx10ARA0_28_addr = 32'sd0;
       UINTCCSCALbx10ARA0_26_addr = 32'sd0;
       UINTCCSCALbx10ARA0_24_addr = 32'sd0;
       UINTCCSCALbx10ARA0_32_addr = 32'sd0;
       UINTCCSCALbx10ARA0_22_addr = 32'sd0;
       UINTCCSCALbx10ARA0_20_addr = 32'sd0;
       A_64_SS_CC_SCALbx56_ARC0_addr = 32'sd0;
       A_64_SS_CC_SCALbx56_ARC0_wdata = 32'sd0;
       A_64_SS_CC_SCALbx54_ARB0_addr = 32'sd0;
       A_64_SS_CC_SCALbx58_ARD0_addr = 32'sd0;
       A_64_SS_CC_SCALbx58_ARD0_wdata = 32'sd0;
       SINTCCSCALbx22ARA0_10_addr = 32'sd0;
       A_64_SS_CC_SCALbx52_ARA0_wdata = 32'sd0;
       A_64_SS_CC_SCALbx54_ARB0_wdata = 32'sd0;
       Z16SSCCSCALbx16ARA0_10_addr = 32'sd0;
       UINTCCSCALbx10ARA0_18_addr = 32'sd0;
       UINTCCSCALbx10ARA0_12_addr = 32'sd0;
       UINTCCSCALbx10ARA0_16_addr = 32'sd0;
       UINTCCSCALbx10ARA0_14_addr = 32'sd0;
       UINTCCSCALbx10ARA0_10_addr = 32'sd0;
       A_SINT_CC_SCALbx24_ARB0_wen = 32'sd0;
       A_SINT_CC_SCALbx24_ARB0_ren = 32'sd0;
       A_64_SS_CC_SCALbx54_ARB0_ren = 32'sd0;
       A_64_SS_CC_SCALbx56_ARC0_ren = 32'sd0;
       A_64_SS_CC_SCALbx58_ARD0_ren = 32'sd0;
       A_64_SS_CC_SCALbx52_ARA0_ren = 32'sd0;
       A_64_SS_CC_SCALbx56_ARC0_wen = 32'sd0;
       A_64_SS_CC_SCALbx58_ARD0_wen = 32'sd0;
       A_64_SS_CC_SCALbx52_ARA0_wen = 32'sd0;
       A_64_SS_CC_SCALbx54_ARB0_wen = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiSWTMAIN400PC10nz)
          32'h1/*1:kiwiSWTMAIN400PC10nz*/:  Z16SSCCSCALbx20ARC0_10_addr = SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0;

          32'h5/*5:kiwiSWTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx24_ARB0_addr = SW_TMAIN400_SmithWaterman_Program_t_install_query_0_6_V_0;
               A_SINT_CC_SCALbx24_ARB0_wdata = SW_TMAIN400_SmithWaterman_Program_t_encode_1_8_SPILL_256;
               end 
              
          32'hb/*11:kiwiSWTMAIN400PC10nz*/:  A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_2
          )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_testReport_2_13_V_1));


          32'h11/*17:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_30_addr = rtl_unsigned_bitextract1(8'd255&(rtl_unsigned_bitextract2(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2
              )>>32'sd8));

               UINTCCSCALbx10ARA0_28_addr = SW_TMAIN400_CRC32checker_process_byte_0_23_V_0;
               end 
              
          32'h13/*19:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_26_addr = rtl_unsigned_bitextract1(8'd255&(rtl_unsigned_bitextract2(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2
              )>>32'sd16));

               UINTCCSCALbx10ARA0_24_addr = SW_TMAIN400_CRC32checker_process_byte_0_31_V_0;
               end 
              
          32'hf/*15:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_32_addr = SW_TMAIN400_CRC32checker_process_byte_0_15_V_0;
               UINTCCSCALbx10ARA0_22_addr = rtl_unsigned_bitextract1(8'd255&rtl_unsigned_bitextract2(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2
              ));

               end 
              
          32'h17/*23:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_22_addr = rtl_unsigned_bitextract1(8'd255&rtl_unsigned_bitextract2(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_2
              ));

               UINTCCSCALbx10ARA0_20_addr = SW_TMAIN400_CRC32checker_process_byte_0_39_V_0;
               end 
              
          32'h15/*21:kiwiSWTMAIN400PC10nz*/: if ((SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1<32'sh4d))  A_64_SS_CC_SCALbx52_ARA0_addr
               = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1)+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_0
              ));

              
          32'h1a/*26:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

               A_64_SS_CC_SCALbx56_ARC0_wdata = (((32'h1a/*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? ($signed(-64'sd2+((32'h1a
              /*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx56_ARC0_rdata: Z64SSCCSCALbx56ARC0rdatah10hold))<A_64_SS_CC_SCALbx54_ARB0_rdata
              ): ($signed(-64'sd2+((32'h1a/*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx56_ARC0_rdata: Z64SSCCSCALbx56ARC0rdatah10hold
              ))<Z64SSCCSCALbx54ARB0rdatah10hold))? ((32'h1a/*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx54_ARB0_rdata
              : Z64SSCCSCALbx54ARB0rdatah10hold): $signed(-64'sd2+((32'h1a/*26:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx56_ARC0_rdata
              : Z64SSCCSCALbx56ARC0rdatah10hold)));

               end 
              
          32'h1c/*28:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx54_ARB0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

               A_64_SS_CC_SCALbx58_ARD0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

               end 
              
          32'h1d/*29:kiwiSWTMAIN400PC10nz*/:  begin 
              if (((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? ($signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
              )? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold))<A_64_SS_CC_SCALbx54_ARB0_rdata): ($signed(-64'sd2+
              ((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold
              ))<Z64SSCCSCALbx54ARB0rdatah10hold)))  begin 
                       A_64_SS_CC_SCALbx58_ARD0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
                      ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

                       A_64_SS_CC_SCALbx58_ARD0_wdata = ((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx54_ARB0_rdata
                      : Z64SSCCSCALbx54ARB0rdatah10hold);

                       end 
                      if (((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? ($signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold))>=A_64_SS_CC_SCALbx54_ARB0_rdata
              ): ($signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold
              ))>=Z64SSCCSCALbx54ARB0rdatah10hold)))  begin 
                       A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
                      ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

                       A_64_SS_CC_SCALbx56_ARC0_wdata = $signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata
                      : Z64SSCCSCALbx58ARD0rdatah10hold));

                       end 
                       end 
              
          32'h1f/*31:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx58_ARD0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0));

               A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0));

               end 
              
          32'h21/*33:kiwiSWTMAIN400PC10nz*/:  A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
          )+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0));


          32'h23/*35:kiwiSWTMAIN400PC10nz*/:  SINTCCSCALbx22ARA0_10_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_2
          )+64'sh14*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_4));


          32'h24/*36:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

               A_64_SS_CC_SCALbx52_ARA0_wdata = hprpin507268x10;
               end 
              
          32'h19/*25:kiwiSWTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx24_ARB0_addr = SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3;
               A_64_SS_CC_SCALbx54_ARB0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0));

               A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_0));

               end 
              
          32'h27/*39:kiwiSWTMAIN400PC10nz*/:  A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
          ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));


          32'h28/*40:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx54_ARB0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(64'd1+rtl_unsigned_extend7(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_3
              ))+64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_1));

               A_64_SS_CC_SCALbx54_ARB0_wdata = $signed(-64'sd10+((32'h28/*40:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx52_ARA0_rdata
              : Z64SSCCSCALbx52ARA0rdatah10hold));

               end 
              
          32'h2c/*44:kiwiSWTMAIN400PC10nz*/:  Z16SSCCSCALbx16ARA0_10_addr = SW_TMAIN400_SmithWaterman_Program_t_runTest_0_12_V_1;

          32'h2f/*47:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx54_ARB0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
              ));

               A_64_SS_CC_SCALbx54_ARB0_wdata = -64'sha;
               A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
              ));

               A_64_SS_CC_SCALbx52_ARA0_wdata = 64'sh0;
               A_64_SS_CC_SCALbx58_ARD0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
              ));

               A_64_SS_CC_SCALbx58_ARD0_wdata = 64'sh0;
               A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_1
              ));

               A_64_SS_CC_SCALbx56_ARC0_wdata = 64'sh0;
               end 
              
          32'h32/*50:kiwiSWTMAIN400PC10nz*/:  begin 
               A_64_SS_CC_SCALbx54_ARB0_addr = rtl_signed_bitextract5(64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
              ));

               A_64_SS_CC_SCALbx54_ARB0_wdata = -64'sha;
               A_64_SS_CC_SCALbx52_ARA0_addr = rtl_signed_bitextract5(64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
              ));

               A_64_SS_CC_SCALbx52_ARA0_wdata = 64'sh0;
               A_64_SS_CC_SCALbx58_ARD0_addr = rtl_signed_bitextract5(64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
              ));

               A_64_SS_CC_SCALbx58_ARD0_wdata = 64'sh0;
               A_64_SS_CC_SCALbx56_ARC0_addr = rtl_signed_bitextract5(64'sh4e*rtl_unsigned_extend6(SW_TMAIN400_SmithWaterman_Program_t_sw_reset_1_2_V_0
              ));

               A_64_SS_CC_SCALbx56_ARC0_wdata = 64'sh0;
               end 
              
          32'h36/*54:kiwiSWTMAIN400PC10nz*/:  UINTCCSCALbx10ARA0_18_addr = 32'd255;

          32'h39/*57:kiwiSWTMAIN400PC10nz*/:  UINTCCSCALbx10ARA0_18_addr = 32'd255;

          32'h3b/*59:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_18_addr = 32'd255;
               UINTCCSCALbx10ARA0_12_addr = 32'd1;
               end 
              
          32'h3e/*62:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_16_addr = 32'd128;
               UINTCCSCALbx10ARA0_14_addr = SW_TMAIN400_CRC32checker_process_byte_0_40_V_0;
               end 
              
          32'h41/*65:kiwiSWTMAIN400PC10nz*/:  begin 
               UINTCCSCALbx10ARA0_12_addr = 32'd1;
               UINTCCSCALbx10ARA0_10_addr = SW_TMAIN400_CRC32checker_process_byte_0_44_V_0;
               end 
              endcase
      if (hpr_int_run_enable_DDX18)  begin 
               A_SINT_CC_SCALbx24_ARB0_wen = (32'h5/*5:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);
               A_SINT_CC_SCALbx24_ARB0_ren = ((32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);
               A_64_SS_CC_SCALbx54_ARB0_ren = ((32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1c/*28:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);

               A_64_SS_CC_SCALbx56_ARC0_ren = ((32'h19/*25:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1f/*31:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);

               A_64_SS_CC_SCALbx58_ARD0_ren = ((32'h1c/*28:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1f/*31:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);

               A_64_SS_CC_SCALbx52_ARA0_ren = (32'h21/*33:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'hb/*11:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) || (SW_TMAIN400_SmithWaterman_Program_t_crcReport_2_16_V_1<32'sh4d) && (32'h15/*21:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) || (32'h27/*39:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);

               A_64_SS_CC_SCALbx56_ARC0_wen = (32'h2f/*47:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1a/*26:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) || (32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) && ($signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold))>=A_64_SS_CC_SCALbx54_ARB0_rdata
              ) || (32'h32/*50:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz);

               A_64_SS_CC_SCALbx58_ARD0_wen = (32'h2f/*47:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h1d/*29:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) && ($signed(-64'sd2+((32'h1d/*29:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata
              : Z64SSCCSCALbx58ARD0rdatah10hold))<A_64_SS_CC_SCALbx54_ARB0_rdata) || (32'h32/*50:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz
              );

               A_64_SS_CC_SCALbx52_ARA0_wen = ((32'h32/*50:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h24/*36:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) || (32'h2f/*47:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);

               A_64_SS_CC_SCALbx54_ARB0_wen = ((32'h32/*50:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz) || (32'h28/*40:kiwiSWTMAIN400PC10nz*/==
              kiwiSWTMAIN400PC10nz) || (32'h2f/*47:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? 32'd1: 32'd0);

               end 
               hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
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
      

  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_10(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_10_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_10_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_12(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_12_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_12_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_14(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_14_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_14_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_16(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_16_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_16_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_18(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_18_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_18_addr
));
  AS_SSROM_A_SINT_CC_SCALbx22_ARA0 SINTCCSCALbx22ARA0_10(.clk(clk), .reset(reset), .ASROM16_rdata(SINTCCSCALbx22ARA0_10_rdata), .ASROM16_addr(SINTCCSCALbx22ARA0_10_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_20(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_20_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_20_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_22(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_22_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_22_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_24(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_24_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_24_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_26(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_26_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_26_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_28(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_28_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_28_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_30(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_30_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_30_addr
));
  AS_SSROM_A_UINT_CC_SCALbx10_ARA0 UINTCCSCALbx10ARA0_32(.clk(clk), .reset(reset), .ASROM14_rdata(UINTCCSCALbx10ARA0_32_rdata), .ASROM14_addr(UINTCCSCALbx10ARA0_32_addr
));
  AS_SSROM_A_16_SS_CC_SCALbx16_ARA0 Z16SSCCSCALbx16ARA0_10(.clk(clk), .reset(reset), .ASROM12_rdata(Z16SSCCSCALbx16ARA0_10_rdata), .ASROM12_addr(Z16SSCCSCALbx16ARA0_10_addr
));
  AS_SSROM_A_16_SS_CC_SCALbx20_ARC0 Z16SSCCSCALbx20ARC0_10(.clk(clk), .reset(reset), .ASROM10_rdata(Z16SSCCSCALbx20ARC0_10_rdata), .ASROM10_addr(Z16SSCCSCALbx20ARC0_10_addr
));
  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd8),
        .WORDS(32'sd156),
        .LANE_WIDTH(32'sd64
),
        .trace_me(32'sd0)) A_64_SS_CC_SCALbx56_ARC0(
        .clk(clk),
        .reset(reset),
        .rdata(A_64_SS_CC_SCALbx56_ARC0_rdata
),
        .addr(A_64_SS_CC_SCALbx56_ARC0_addr),
        .wen(A_64_SS_CC_SCALbx56_ARC0_wen),
        .ren(A_64_SS_CC_SCALbx56_ARC0_ren
),
        .wdata(A_64_SS_CC_SCALbx56_ARC0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd8),
        .WORDS(32'sd156),
        .LANE_WIDTH(32'sd64
),
        .trace_me(32'sd0)) A_64_SS_CC_SCALbx58_ARD0(
        .clk(clk),
        .reset(reset),
        .rdata(A_64_SS_CC_SCALbx58_ARD0_rdata
),
        .addr(A_64_SS_CC_SCALbx58_ARD0_addr),
        .wen(A_64_SS_CC_SCALbx58_ARD0_wen),
        .ren(A_64_SS_CC_SCALbx58_ARD0_ren
),
        .wdata(A_64_SS_CC_SCALbx58_ARD0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd8),
        .WORDS(32'sd156),
        .LANE_WIDTH(32'sd64
),
        .trace_me(32'sd0)) A_64_SS_CC_SCALbx52_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_64_SS_CC_SCALbx52_ARA0_rdata
),
        .addr(A_64_SS_CC_SCALbx52_ARA0_addr),
        .wen(A_64_SS_CC_SCALbx52_ARA0_wen),
        .ren(A_64_SS_CC_SCALbx52_ARA0_ren
),
        .wdata(A_64_SS_CC_SCALbx52_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd8),
        .WORDS(32'sd156),
        .LANE_WIDTH(32'sd64
),
        .trace_me(32'sd0)) A_64_SS_CC_SCALbx54_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_64_SS_CC_SCALbx54_ARB0_rdata
),
        .addr(A_64_SS_CC_SCALbx54_ARB0_addr),
        .wen(A_64_SS_CC_SCALbx54_ARB0_wen),
        .ren(A_64_SS_CC_SCALbx54_ARB0_ren
),
        .wdata(A_64_SS_CC_SCALbx54_ARB0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd7),
        .WORDS(32'sd77),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) A_SINT_CC_SCALbx24_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_SCALbx24_ARB0_rdata),
        .addr(A_SINT_CC_SCALbx24_ARB0_addr
),
        .wen(A_SINT_CC_SCALbx24_ARB0_wen),
        .ren(A_SINT_CC_SCALbx24_ARB0_ren),
        .wdata(A_SINT_CC_SCALbx24_ARB0_wdata
));

assign hprpin507125x10 = ($unsigned(-32'd1&(((32'h10/*16:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_32_rdata: UINTCCSCALbx10ARA032rdatah10hold
)^((32'h10/*16:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_22_rdata: UINTCCSCALbx10ARA022rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10
<<32'sd8)))>>32'sd24);

assign hprpin507143x10 = ($unsigned(-32'd1&(((32'h12/*18:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_30_rdata: UINTCCSCALbx10ARA030rdatah10hold
)^((32'h12/*18:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_28_rdata: UINTCCSCALbx10ARA028rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10
<<32'sd8)))>>32'sd24);

assign hprpin507161x10 = ($unsigned(-32'd1&(((32'h14/*20:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_26_rdata: UINTCCSCALbx10ARA026rdatah10hold
)^((32'h14/*20:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? UINTCCSCALbx10ARA0_24_rdata: UINTCCSCALbx10ARA024rdatah10hold)^(A_UINT_CC_SCALbx14_crcreg10
<<32'sd8)))>>32'sd24);

assign hprpin507268x10 = (($signed(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7+((32'h24/*36:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? rtl_sign_extend4(SINTCCSCALbx22ARA0_10_rdata
): rtl_sign_extend4(SINTCCSCALbx22ARA010rdatah10hold)))<64'sh0)? 64'sh0: $signed(SW_TMAIN400_SmithWaterman_Program_t_next_data_2_10_V_7
+((32'h24/*36:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? rtl_sign_extend4(SINTCCSCALbx22ARA0_10_rdata): rtl_sign_extend4(SINTCCSCALbx22ARA010rdatah10hold
))));

assign hprpin507613x10 = (((32'h20/*32:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? (A_64_SS_CC_SCALbx58_ARD0_rdata<A_64_SS_CC_SCALbx56_ARC0_rdata): (Z64SSCCSCALbx58ARD0rdatah10hold
<Z64SSCCSCALbx56ARC0rdatah10hold))? ((32'h20/*32:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx56_ARC0_rdata: Z64SSCCSCALbx56ARC0rdatah10hold
): ((32'h20/*32:kiwiSWTMAIN400PC10nz*/==kiwiSWTMAIN400PC10nz)? A_64_SS_CC_SCALbx58_ARD0_rdata: Z64SSCCSCALbx58ARD0rdatah10hold));

 initial        begin 
      //ROM data table: 20 words of 16 bits.
       A_16_SS_CC_SCALbx18_ARB0[0] = 16'h61;
       A_16_SS_CC_SCALbx18_ARB0[1] = 16'h63;
       A_16_SS_CC_SCALbx18_ARB0[2] = 16'h64;
       A_16_SS_CC_SCALbx18_ARB0[3] = 16'h65;
       A_16_SS_CC_SCALbx18_ARB0[4] = 16'h66;
       A_16_SS_CC_SCALbx18_ARB0[5] = 16'h67;
       A_16_SS_CC_SCALbx18_ARB0[6] = 16'h68;
       A_16_SS_CC_SCALbx18_ARB0[7] = 16'h69;
       A_16_SS_CC_SCALbx18_ARB0[8] = 16'h6b;
       A_16_SS_CC_SCALbx18_ARB0[9] = 16'h6c;
       A_16_SS_CC_SCALbx18_ARB0[10] = 16'h6d;
       A_16_SS_CC_SCALbx18_ARB0[11] = 16'h6e;
       A_16_SS_CC_SCALbx18_ARB0[12] = 16'h70;
       A_16_SS_CC_SCALbx18_ARB0[13] = 16'h71;
       A_16_SS_CC_SCALbx18_ARB0[14] = 16'h72;
       A_16_SS_CC_SCALbx18_ARB0[15] = 16'h73;
       A_16_SS_CC_SCALbx18_ARB0[16] = 16'h74;
       A_16_SS_CC_SCALbx18_ARB0[17] = 16'h76;
       A_16_SS_CC_SCALbx18_ARB0[18] = 16'h77;
       A_16_SS_CC_SCALbx18_ARB0[19] = 16'h79;
       end 
      

// Structural Resource (FU) inventory for DUT:// 4 vectors of width 7
// 31 vectors of width 1
// 2 vectors of width 16
// 10 vectors of width 64
// 37 vectors of width 32
// 22 vectors of width 8
// 1 vectors of width 9
// 20 array locations of width 16
// Total state bits in module = 2420 bits.
// 960 continuously assigned (wire/non-state) bits 
//   cell AS_SSROM_A_UINT_CC_SCALbx10_ARA0 count=12
//   cell AS_SSROM_A_SINT_CC_SCALbx22_ARA0 count=1
//   cell AS_SSROM_A_16_SS_CC_SCALbx16_ARA0 count=1
//   cell AS_SSROM_A_16_SS_CC_SCALbx20_ARC0 count=1
//   cell CV_SP_SSRAM_FL1 count=5
// Total number of leaf cells = 20
endmodule



module AS_SSROM_A_16_SS_CC_SCALbx20_ARC0(    
/* portgroup= abstractionName=res2-contacts pi_name=ASROM10 */
    output reg signed [15:0] ASROM10_rdata,
    input [6:0] ASROM10_addr,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets12 */
input clk,
    input reset);
// abstractionName=res2-sim-nets pi_name=ASROM10
  reg signed [15:0] RomData10[76:0];
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogAS_SSROM_@16_SS/CC/SCALbx20_ARC0/1.0
      if (reset)  ASROM10_rdata <= 32'd0;
           else  ASROM10_rdata <= RomData10[ASROM10_addr];
      //End structure cvtToVerilogAS_SSROM_@16_SS/CC/SCALbx20_ARC0/1.0


       end 
      

//Resource=SROM iname=Z16SSCCSCALbx20ARC0_10 77x16 clk=posedge(clk) synchronous/pipeline=1 no_ports=1 <NONE> used by threads kiwiSWTMAIN400PC10
 initial        begin 
      //ROM data table: 77 words of 16 bits.
       RomData10[0] = 16'h64;
       RomData10[1] = 16'h6b;
       RomData10[2] = 16'h68;
       RomData10[3] = 16'h6b;
       RomData10[4] = 16'h6c;
       RomData10[5] = 16'h69;
       RomData10[6] = 16'h74;
       RomData10[7] = 16'h6b;
       RomData10[8] = 16'h74;
       RomData10[9] = 16'h65;
       RomData10[10] = 16'h61;
       RomData10[11] = 16'h6b;
       RomData10[12] = 16'h71;
       RomData10[13] = 16'h65;
       RomData10[14] = 16'h79;
       RomData10[15] = 16'h6c;
       RomData10[16] = 16'h6c;
       RomData10[17] = 16'h6b;
       RomData10[18] = 16'h64;
       RomData10[19] = 16'h63;
       RomData10[20] = 16'h64;
       RomData10[21] = 16'h6c;
       RomData10[22] = 16'h65;
       RomData10[23] = 16'h6b;
       RomData10[24] = 16'h72;
       RomData10[25] = 16'h65;
       RomData10[26] = 16'h70;
       RomData10[27] = 16'h70;
       RomData10[28] = 16'h6c;
       RomData10[29] = 16'h6b;
       RomData10[30] = 16'h66;
       RomData10[31] = 16'h69;
       RomData10[32] = 16'h76;
       RomData10[33] = 16'h6b;
       RomData10[34] = 16'h6b;
       RomData10[35] = 16'h6e;
       RomData10[36] = 16'h70;
       RomData10[37] = 16'h68;
       RomData10[38] = 16'h68;
       RomData10[39] = 16'h73;
       RomData10[40] = 16'h71;
       RomData10[41] = 16'h77;
       RomData10[42] = 16'h67;
       RomData10[43] = 16'h64;
       RomData10[44] = 16'h6d;
       RomData10[45] = 16'h6b;
       RomData10[46] = 16'h6c;
       RomData10[47] = 16'h79;
       RomData10[48] = 16'h6c;
       RomData10[49] = 16'h6b;
       RomData10[50] = 16'h6c;
       RomData10[51] = 16'h71;
       RomData10[52] = 16'h69;
       RomData10[53] = 16'h76;
       RomData10[54] = 16'h6b;
       RomData10[55] = 16'h72;
       RomData10[56] = 16'h73;
       RomData10[57] = 16'h6c;
       RomData10[58] = 16'h65;
       RomData10[59] = 16'h76;
       RomData10[60] = 16'h77;
       RomData10[61] = 16'h67;
       RomData10[62] = 16'h73;
       RomData10[63] = 16'h71;
       RomData10[64] = 16'h65;
       RomData10[65] = 16'h61;
       RomData10[66] = 16'h6c;
       RomData10[67] = 16'h65;
       RomData10[68] = 16'h65;
       RomData10[69] = 16'h61;
       RomData10[70] = 16'h6b;
       RomData10[71] = 16'h65;
       RomData10[72] = 16'h76;
       RomData10[73] = 16'h72;
       RomData10[74] = 16'h71;
       RomData10[75] = 16'h65;
       RomData10[76] = 16'h6e;
       end 
      

// Structural Resource (FU) inventory for AS_SSROM_A_16_SS_CC_SCALbx20_ARC0:// 77 array locations of width 16
// Total state bits in module = 1232 bits.
// Total number of leaf cells = 0
endmodule



module AS_SSROM_A_16_SS_CC_SCALbx16_ARA0(    
/* portgroup= abstractionName=res2-contacts pi_name=ASROM12 */
    output reg signed [15:0] ASROM12_rdata,
    input [6:0] ASROM12_addr,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets14 */
input clk,
    input reset);
// abstractionName=res2-sim-nets pi_name=ASROM12
  reg signed [15:0] RomData12[74:0];
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogAS_SSROM_@16_SS/CC/SCALbx16_ARA0/1.0
      if (reset)  ASROM12_rdata <= 32'd0;
           else  ASROM12_rdata <= RomData12[ASROM12_addr];
      //End structure cvtToVerilogAS_SSROM_@16_SS/CC/SCALbx16_ARA0/1.0


       end 
      

//Resource=SROM iname=Z16SSCCSCALbx16ARA0_10 75x16 clk=posedge(clk) synchronous/pipeline=1 no_ports=1 <NONE> used by threads kiwiSWTMAIN400PC10
 initial        begin 
      //ROM data table: 75 words of 16 bits.
       RomData12[0] = 16'h70;
       RomData12[1] = 16'h70;
       RomData12[2] = 16'h65;
       RomData12[3] = 16'h61;
       RomData12[4] = 16'h69;
       RomData12[5] = 16'h70;
       RomData12[6] = 16'h66;
       RomData12[7] = 16'h72;
       RomData12[8] = 16'h70;
       RomData12[9] = 16'h65;
       RomData12[10] = 16'h79;
       RomData12[11] = 16'h61;
       RomData12[12] = 16'h6e;
       RomData12[13] = 16'h72;
       RomData12[14] = 16'h6c;
       RomData12[15] = 16'h6c;
       RomData12[16] = 16'h67;
       RomData12[17] = 16'h74;
       RomData12[18] = 16'h73;
       RomData12[19] = 16'h79;
       RomData12[20] = 16'h70;
       RomData12[21] = 16'h65;
       RomData12[22] = 16'h61;
       RomData12[23] = 16'h65;
       RomData12[24] = 16'h71;
       RomData12[25] = 16'h69;
       RomData12[26] = 16'h61;
       RomData12[27] = 16'h69;
       RomData12[28] = 16'h6c;
       RomData12[29] = 16'h6b;
       RomData12[30] = 16'h72;
       RomData12[31] = 16'h6c;
       RomData12[32] = 16'h67;
       RomData12[33] = 16'h63;
       RomData12[34] = 16'h72;
       RomData12[35] = 16'h76;
       RomData12[36] = 16'h65;
       RomData12[37] = 16'h67;
       RomData12[38] = 16'h65;
       RomData12[39] = 16'h67;
       RomData12[40] = 16'h70;
       RomData12[41] = 16'h74;
       RomData12[42] = 16'h79;
       RomData12[43] = 16'h72;
       RomData12[44] = 16'h76;
       RomData12[45] = 16'h74;
       RomData12[46] = 16'h70;
       RomData12[47] = 16'h70;
       RomData12[48] = 16'h73;
       RomData12[49] = 16'h68;
       RomData12[50] = 16'h72;
       RomData12[51] = 16'h6c;
       RomData12[52] = 16'h64;
       RomData12[53] = 16'h6c;
       RomData12[54] = 16'h72;
       RomData12[55] = 16'h6c;
       RomData12[56] = 16'h65;
       RomData12[57] = 16'h65;
       RomData12[58] = 16'h64;
       RomData12[59] = 16'h6c;
       RomData12[60] = 16'h76;
       RomData12[61] = 16'h65;
       RomData12[62] = 16'h65;
       RomData12[63] = 16'h76;
       RomData12[64] = 16'h61;
       RomData12[65] = 16'h72;
       RomData12[66] = 16'h69;
       RomData12[67] = 16'h71;
       RomData12[68] = 16'h67;
       RomData12[69] = 16'h79;
       RomData12[70] = 16'h65;
       RomData12[71] = 16'h74;
       RomData12[72] = 16'h69;
       RomData12[73] = 16'h70;
       RomData12[74] = 16'h6c;
       end 
      

// Structural Resource (FU) inventory for AS_SSROM_A_16_SS_CC_SCALbx16_ARA0:// 75 array locations of width 16
// Total state bits in module = 1200 bits.
// Total number of leaf cells = 0
endmodule



module AS_SSROM_A_UINT_CC_SCALbx10_ARA0(    
/* portgroup= abstractionName=res2-contacts pi_name=ASROM14 */
    output reg [31:0] ASROM14_rdata,
    input [7:0] ASROM14_addr,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets16 */
input clk,
    input reset);
// abstractionName=res2-sim-nets pi_name=ASROM14
  reg [31:0] RomData14[255:0];
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogAS_SSROM_@_UINT/CC/SCALbx10_ARA0/1.0
      if (reset)  ASROM14_rdata <= 32'd0;
           else  ASROM14_rdata <= RomData14[ASROM14_addr];
      //End structure cvtToVerilogAS_SSROM_@_UINT/CC/SCALbx10_ARA0/1.0


       end 
      

//Resource=SROM iname=UINTCCSCALbx10ARA0_32 256x32 clk=posedge(clk) synchronous/pipeline=1 no_ports=1 <NONE> used by threads kiwiSWTMAIN400PC10
 initial        begin 
      //ROM data table: 256 words of 32 bits.
       RomData14[0] = 32'h0;
       RomData14[1] = 32'h4c1_1db7;
       RomData14[2] = 32'h982_3b6e;
       RomData14[3] = 32'hd43_26d9;
       RomData14[4] = 32'h1304_76dc;
       RomData14[5] = 32'h17c5_6b6b;
       RomData14[6] = 32'h1a86_4db2;
       RomData14[7] = 32'h1e47_5005;
       RomData14[8] = 32'h2608_edb8;
       RomData14[9] = 32'h22c9_f00f;
       RomData14[10] = 32'h2f8a_d6d6;
       RomData14[11] = 32'h2b4b_cb61;
       RomData14[12] = 32'h350c_9b64;
       RomData14[13] = 32'h31cd_86d3;
       RomData14[14] = 32'h3c8e_a00a;
       RomData14[15] = 32'h384f_bdbd;
       RomData14[16] = 32'h4c11_db70;
       RomData14[17] = 32'h48d0_c6c7;
       RomData14[18] = 32'h4593_e01e;
       RomData14[19] = 32'h4152_fda9;
       RomData14[20] = 32'h5f15_adac;
       RomData14[21] = 32'h5bd4_b01b;
       RomData14[22] = 32'h5697_96c2;
       RomData14[23] = 32'h5256_8b75;
       RomData14[24] = 32'h6a19_36c8;
       RomData14[25] = 32'h6ed8_2b7f;
       RomData14[26] = 32'h639b_0da6;
       RomData14[27] = 32'h675a_1011;
       RomData14[28] = 32'h791d_4014;
       RomData14[29] = 32'h7ddc_5da3;
       RomData14[30] = 32'h709f_7b7a;
       RomData14[31] = 32'h745e_66cd;
       RomData14[32] = -32'h67dc_4920;
       RomData14[33] = -32'h631d_54a9;
       RomData14[34] = -32'h6e5e_7272;
       RomData14[35] = -32'h6a9f_6fc7;
       RomData14[36] = -32'h74d8_3fc4;
       RomData14[37] = -32'h7019_2275;
       RomData14[38] = -32'h7d5a_04ae;
       RomData14[39] = -32'h799b_191b;
       RomData14[40] = -32'h41d4_a4a8;
       RomData14[41] = -32'h4515_b911;
       RomData14[42] = -32'h4856_9fca;
       RomData14[43] = -32'h4c97_827f;
       RomData14[44] = -32'h52d0_d27c;
       RomData14[45] = -32'h5611_cfcd;
       RomData14[46] = -32'h5b52_e916;
       RomData14[47] = -32'h5f93_f4a3;
       RomData14[48] = -32'h2bcd_9270;
       RomData14[49] = -32'h2f0c_8fd9;
       RomData14[50] = -32'h224f_a902;
       RomData14[51] = -32'h268e_b4b7;
       RomData14[52] = -32'h38c9_e4b4;
       RomData14[53] = -32'h3c08_f905;
       RomData14[54] = -32'h314b_dfde;
       RomData14[55] = -32'h358a_c26b;
       RomData14[56] = -32'hdc5_7fd8;
       RomData14[57] = -32'h904_6261;
       RomData14[58] = -32'h447_44ba;
       RomData14[59] = -32'h86_590f;
       RomData14[60] = -32'h1ec1_090c;
       RomData14[61] = -32'h1a00_14bd;
       RomData14[62] = -32'h1743_3266;
       RomData14[63] = -32'h1382_2fd3;
       RomData14[64] = 32'h3486_7077;
       RomData14[65] = 32'h3047_6dc0;
       RomData14[66] = 32'h3d04_4b19;
       RomData14[67] = 32'h39c5_56ae;
       RomData14[68] = 32'h2782_06ab;
       RomData14[69] = 32'h2343_1b1c;
       RomData14[70] = 32'h2e00_3dc5;
       RomData14[71] = 32'h2ac1_2072;
       RomData14[72] = 32'h128e_9dcf;
       RomData14[73] = 32'h164f_8078;
       RomData14[74] = 32'h1b0c_a6a1;
       RomData14[75] = 32'h1fcd_bb16;
       RomData14[76] = 32'h18a_eb13;
       RomData14[77] = 32'h54b_f6a4;
       RomData14[78] = 32'h808_d07d;
       RomData14[79] = 32'hcc9_cdca;
       RomData14[80] = 32'h7897_ab07;
       RomData14[81] = 32'h7c56_b6b0;
       RomData14[82] = 32'h7115_9069;
       RomData14[83] = 32'h75d4_8dde;
       RomData14[84] = 32'h6b93_dddb;
       RomData14[85] = 32'h6f52_c06c;
       RomData14[86] = 32'h6211_e6b5;
       RomData14[87] = 32'h66d0_fb02;
       RomData14[88] = 32'h5e9f_46bf;
       RomData14[89] = 32'h5a5e_5b08;
       RomData14[90] = 32'h571d_7dd1;
       RomData14[91] = 32'h53dc_6066;
       RomData14[92] = 32'h4d9b_3063;
       RomData14[93] = 32'h495a_2dd4;
       RomData14[94] = 32'h4419_0b0d;
       RomData14[95] = 32'h40d8_16ba;
       RomData14[96] = -32'h535a_3969;
       RomData14[97] = -32'h579b_24e0;
       RomData14[98] = -32'h5ad8_0207;
       RomData14[99] = -32'h5e19_1fb2;
       RomData14[100] = -32'h405e_4fb5;
       RomData14[101] = -32'h449f_5204;
       RomData14[102] = -32'h49dc_74db;
       RomData14[103] = -32'h4d1d_696e;
       RomData14[104] = -32'h7552_d4d1;
       RomData14[105] = -32'h7193_c968;
       RomData14[106] = -32'h7cd0_efbf;
       RomData14[107] = -32'h7811_f20a;
       RomData14[108] = -32'h6656_a20d;
       RomData14[109] = -32'h6297_bfbc;
       RomData14[110] = -32'h6fd4_9963;
       RomData14[111] = -32'h6b15_84d6;
       RomData14[112] = -32'h1f4b_e219;
       RomData14[113] = -32'h1b8a_ffb0;
       RomData14[114] = -32'h16c9_d977;
       RomData14[115] = -32'h1208_c4c2;
       RomData14[116] = -32'hc4f_94c5;
       RomData14[117] = -32'h88e_8974;
       RomData14[118] = -32'h5cd_afab;
       RomData14[119] = -32'h10c_b21e;
       RomData14[120] = -32'h3943_0fa1;
       RomData14[121] = -32'h3d82_1218;
       RomData14[122] = -32'h30c1_34cf;
       RomData14[123] = -32'h3400_297a;
       RomData14[124] = -32'h2a47_797d;
       RomData14[125] = -32'h2e86_64cc;
       RomData14[126] = -32'h23c5_4213;
       RomData14[127] = -32'h2704_5fa6;
       RomData14[128] = 32'h690c_e0ee;
       RomData14[129] = 32'h6dcd_fd59;
       RomData14[130] = 32'h608e_db80;
       RomData14[131] = 32'h644f_c637;
       RomData14[132] = 32'h7a08_9632;
       RomData14[133] = 32'h7ec9_8b85;
       RomData14[134] = 32'h738a_ad5c;
       RomData14[135] = 32'h774b_b0eb;
       RomData14[136] = 32'h4f04_0d56;
       RomData14[137] = 32'h4bc5_10e1;
       RomData14[138] = 32'h4686_3638;
       RomData14[139] = 32'h4247_2b8f;
       RomData14[140] = 32'h5c00_7b8a;
       RomData14[141] = 32'h58c1_663d;
       RomData14[142] = 32'h5582_40e4;
       RomData14[143] = 32'h5143_5d53;
       RomData14[144] = 32'h251d_3b9e;
       RomData14[145] = 32'h21dc_2629;
       RomData14[146] = 32'h2c9f_00f0;
       RomData14[147] = 32'h285e_1d47;
       RomData14[148] = 32'h3619_4d42;
       RomData14[149] = 32'h32d8_50f5;
       RomData14[150] = 32'h3f9b_762c;
       RomData14[151] = 32'h3b5a_6b9b;
       RomData14[152] = 32'h315_d626;
       RomData14[153] = 32'h7d4_cb91;
       RomData14[154] = 32'ha97_ed48;
       RomData14[155] = 32'he56_f0ff;
       RomData14[156] = 32'h1011_a0fa;
       RomData14[157] = 32'h14d0_bd4d;
       RomData14[158] = 32'h1993_9b94;
       RomData14[159] = 32'h1d52_8623;
       RomData14[160] = -32'hed0_a9f2;
       RomData14[161] = -32'ha11_b447;
       RomData14[162] = -32'h752_92a0;
       RomData14[163] = -32'h393_8f29;
       RomData14[164] = -32'h1dd4_df2e;
       RomData14[165] = -32'h1915_c29b;
       RomData14[166] = -32'h1456_e444;
       RomData14[167] = -32'h1097_f9f5;
       RomData14[168] = -32'h28d8_444a;
       RomData14[169] = -32'h2c19_59ff;
       RomData14[170] = -32'h215a_7f28;
       RomData14[171] = -32'h259b_6291;
       RomData14[172] = -32'h3bdc_3296;
       RomData14[173] = -32'h3f1d_2f23;
       RomData14[174] = -32'h325e_09fc;
       RomData14[175] = -32'h369f_144d;
       RomData14[176] = -32'h42c1_7282;
       RomData14[177] = -32'h4600_6f37;
       RomData14[178] = -32'h4b43_49f0;
       RomData14[179] = -32'h4f82_5459;
       RomData14[180] = -32'h51c5_045e;
       RomData14[181] = -32'h5504_19eb;
       RomData14[182] = -32'h5847_3f34;
       RomData14[183] = -32'h5c86_2285;
       RomData14[184] = -32'h64c9_9f3a;
       RomData14[185] = -32'h6008_828f;
       RomData14[186] = -32'h6d4b_a458;
       RomData14[187] = -32'h698a_b9e1;
       RomData14[188] = -32'h77cd_e9e6;
       RomData14[189] = -32'h730c_f453;
       RomData14[190] = -32'h7e4f_d28c;
       RomData14[191] = -32'h7a8e_cf3d;
       RomData14[192] = 32'h5d8a_9099;
       RomData14[193] = 32'h594b_8d2e;
       RomData14[194] = 32'h5408_abf7;
       RomData14[195] = 32'h50c9_b640;
       RomData14[196] = 32'h4e8e_e645;
       RomData14[197] = 32'h4a4f_fbf2;
       RomData14[198] = 32'h470c_dd2b;
       RomData14[199] = 32'h43cd_c09c;
       RomData14[200] = 32'h7b82_7d21;
       RomData14[201] = 32'h7f43_6096;
       RomData14[202] = 32'h7200_464f;
       RomData14[203] = 32'h76c1_5bf8;
       RomData14[204] = 32'h6886_0bfd;
       RomData14[205] = 32'h6c47_164a;
       RomData14[206] = 32'h6104_3093;
       RomData14[207] = 32'h65c5_2d24;
       RomData14[208] = 32'h119b_4be9;
       RomData14[209] = 32'h155a_565e;
       RomData14[210] = 32'h1819_7087;
       RomData14[211] = 32'h1cd8_6d30;
       RomData14[212] = 32'h29f_3d35;
       RomData14[213] = 32'h65e_2082;
       RomData14[214] = 32'hb1d_065b;
       RomData14[215] = 32'hfdc_1bec;
       RomData14[216] = 32'h3793_a651;
       RomData14[217] = 32'h3352_bbe6;
       RomData14[218] = 32'h3e11_9d3f;
       RomData14[219] = 32'h3ad0_8088;
       RomData14[220] = 32'h2497_d08d;
       RomData14[221] = 32'h2056_cd3a;
       RomData14[222] = 32'h2d15_ebe3;
       RomData14[223] = 32'h29d4_f654;
       RomData14[224] = -32'h3a56_d987;
       RomData14[225] = -32'h3e97_c432;
       RomData14[226] = -32'h33d4_e2e9;
       RomData14[227] = -32'h3715_ff60;
       RomData14[228] = -32'h2952_af5b;
       RomData14[229] = -32'h2d93_b2ee;
       RomData14[230] = -32'h20d0_9435;
       RomData14[231] = -32'h2411_8984;
       RomData14[232] = -32'h1c5e_343f;
       RomData14[233] = -32'h189f_298a;
       RomData14[234] = -32'h15dc_0f51;
       RomData14[235] = -32'h111d_12e8;
       RomData14[236] = -32'hf5a_42e3;
       RomData14[237] = -32'hb9b_5f56;
       RomData14[238] = -32'h6d8_798d;
       RomData14[239] = -32'h219_643c;
       RomData14[240] = -32'h7647_02f7;
       RomData14[241] = -32'h7286_1f42;
       RomData14[242] = -32'h7fc5_3999;
       RomData14[243] = -32'h7b04_2430;
       RomData14[244] = -32'h6543_742b;
       RomData14[245] = -32'h6182_699e;
       RomData14[246] = -32'h6cc1_4f45;
       RomData14[247] = -32'h6800_52f4;
       RomData14[248] = -32'h504f_ef4f;
       RomData14[249] = -32'h548e_f2fa;
       RomData14[250] = -32'h59cd_d421;
       RomData14[251] = -32'h5d0c_c998;
       RomData14[252] = -32'h434b_9993;
       RomData14[253] = -32'h478a_8426;
       RomData14[254] = -32'h4ac9_a2fd;
       RomData14[255] = -32'h4e08_bf4c;
       end 
      

// Structural Resource (FU) inventory for AS_SSROM_A_UINT_CC_SCALbx10_ARA0:// 256 array locations of width 32
// Total state bits in module = 8192 bits.
// Total number of leaf cells = 0
endmodule



module AS_SSROM_A_SINT_CC_SCALbx22_ARA0(    
/* portgroup= abstractionName=res2-contacts pi_name=ASROM16 */
    output reg signed [31:0] ASROM16_rdata,
    input [8:0] ASROM16_addr,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets18 */
input clk,
    input reset);
// abstractionName=res2-sim-nets pi_name=ASROM16
  reg signed [31:0] RomData28[399:0];
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogAS_SSROM_@_SINT/CC/SCALbx22_ARA0/1.0
      if (reset)  ASROM16_rdata <= 32'd0;
           else  ASROM16_rdata <= RomData28[ASROM16_addr];
      //End structure cvtToVerilogAS_SSROM_@_SINT/CC/SCALbx22_ARA0/1.0


       end 
      

//Resource=SROM iname=SINTCCSCALbx22ARA0_10 400x32 clk=posedge(clk) synchronous/pipeline=1 no_ports=1 <NONE> used by threads kiwiSWTMAIN400PC10
 initial        begin 
      //ROM data table: 400 words of 32 bits.
       RomData28[0] = 32'h2;
       RomData28[1] = -32'h2;
       RomData28[2] = 32'h0;
       RomData28[3] = 32'h0;
       RomData28[4] = -32'h4;
       RomData28[5] = 32'h1;
       RomData28[6] = -32'h1;
       RomData28[7] = -32'h1;
       RomData28[8] = -32'h1;
       RomData28[9] = -32'h2;
       RomData28[10] = -32'h1;
       RomData28[11] = 32'h0;
       RomData28[12] = 32'h1;
       RomData28[13] = 32'h0;
       RomData28[14] = -32'h2;
       RomData28[15] = 32'h1;
       RomData28[16] = 32'h1;
       RomData28[17] = 32'h0;
       RomData28[18] = -32'h1;
       RomData28[19] = -32'h3;
       RomData28[20] = -32'h2;
       RomData28[21] = 32'hc;
       RomData28[22] = -32'h5;
       RomData28[23] = -32'h5;
       RomData28[24] = -32'h4;
       RomData28[25] = -32'h3;
       RomData28[26] = -32'h3;
       RomData28[27] = -32'h2;
       RomData28[28] = -32'h5;
       RomData28[29] = -32'h6;
       RomData28[30] = -32'h5;
       RomData28[31] = -32'h4;
       RomData28[32] = -32'h3;
       RomData28[33] = -32'h5;
       RomData28[34] = -32'h4;
       RomData28[35] = 32'h0;
       RomData28[36] = -32'h2;
       RomData28[37] = -32'h2;
       RomData28[38] = -32'h8;
       RomData28[39] = 32'h0;
       RomData28[40] = 32'h0;
       RomData28[41] = -32'h5;
       RomData28[42] = 32'h4;
       RomData28[43] = 32'h3;
       RomData28[44] = -32'h6;
       RomData28[45] = 32'h1;
       RomData28[46] = 32'h1;
       RomData28[47] = -32'h2;
       RomData28[48] = 32'h0;
       RomData28[49] = -32'h4;
       RomData28[50] = -32'h3;
       RomData28[51] = 32'h2;
       RomData28[52] = -32'h1;
       RomData28[53] = 32'h2;
       RomData28[54] = -32'h1;
       RomData28[55] = 32'h0;
       RomData28[56] = 32'h0;
       RomData28[57] = -32'h2;
       RomData28[58] = -32'h7;
       RomData28[59] = -32'h4;
       RomData28[60] = 32'h0;
       RomData28[61] = -32'h5;
       RomData28[62] = 32'h3;
       RomData28[63] = 32'h4;
       RomData28[64] = -32'h5;
       RomData28[65] = 32'h0;
       RomData28[66] = 32'h1;
       RomData28[67] = -32'h2;
       RomData28[68] = 32'h0;
       RomData28[69] = -32'h3;
       RomData28[70] = -32'h2;
       RomData28[71] = 32'h1;
       RomData28[72] = -32'h1;
       RomData28[73] = 32'h2;
       RomData28[74] = -32'h1;
       RomData28[75] = 32'h0;
       RomData28[76] = 32'h0;
       RomData28[77] = -32'h2;
       RomData28[78] = -32'h7;
       RomData28[79] = -32'h4;
       RomData28[80] = -32'h4;
       RomData28[81] = -32'h4;
       RomData28[82] = -32'h6;
       RomData28[83] = -32'h5;
       RomData28[84] = 32'h9;
       RomData28[85] = -32'h5;
       RomData28[86] = -32'h2;
       RomData28[87] = 32'h1;
       RomData28[88] = -32'h5;
       RomData28[89] = 32'h2;
       RomData28[90] = 32'h0;
       RomData28[91] = -32'h4;
       RomData28[92] = -32'h5;
       RomData28[93] = -32'h5;
       RomData28[94] = -32'h4;
       RomData28[95] = -32'h3;
       RomData28[96] = -32'h3;
       RomData28[97] = -32'h1;
       RomData28[98] = 32'h0;
       RomData28[99] = 32'h7;
       RomData28[100] = 32'h1;
       RomData28[101] = -32'h3;
       RomData28[102] = 32'h1;
       RomData28[103] = 32'h0;
       RomData28[104] = -32'h5;
       RomData28[105] = 32'h5;
       RomData28[106] = -32'h2;
       RomData28[107] = -32'h3;
       RomData28[108] = -32'h2;
       RomData28[109] = -32'h4;
       RomData28[110] = -32'h3;
       RomData28[111] = 32'h0;
       RomData28[112] = -32'h1;
       RomData28[113] = -32'h1;
       RomData28[114] = -32'h3;
       RomData28[115] = 32'h1;
       RomData28[116] = 32'h0;
       RomData28[117] = -32'h1;
       RomData28[118] = -32'h7;
       RomData28[119] = -32'h5;
       RomData28[120] = -32'h1;
       RomData28[121] = -32'h3;
       RomData28[122] = 32'h1;
       RomData28[123] = 32'h1;
       RomData28[124] = -32'h2;
       RomData28[125] = -32'h2;
       RomData28[126] = 32'h6;
       RomData28[127] = -32'h2;
       RomData28[128] = 32'h0;
       RomData28[129] = -32'h2;
       RomData28[130] = -32'h2;
       RomData28[131] = 32'h2;
       RomData28[132] = 32'h0;
       RomData28[133] = 32'h3;
       RomData28[134] = 32'h2;
       RomData28[135] = -32'h1;
       RomData28[136] = -32'h1;
       RomData28[137] = -32'h2;
       RomData28[138] = -32'h3;
       RomData28[139] = 32'h0;
       RomData28[140] = -32'h1;
       RomData28[141] = -32'h2;
       RomData28[142] = -32'h2;
       RomData28[143] = -32'h2;
       RomData28[144] = 32'h1;
       RomData28[145] = -32'h3;
       RomData28[146] = -32'h2;
       RomData28[147] = 32'h5;
       RomData28[148] = -32'h2;
       RomData28[149] = 32'h2;
       RomData28[150] = 32'h2;
       RomData28[151] = -32'h2;
       RomData28[152] = -32'h2;
       RomData28[153] = -32'h2;
       RomData28[154] = -32'h2;
       RomData28[155] = -32'h1;
       RomData28[156] = 32'h0;
       RomData28[157] = 32'h4;
       RomData28[158] = -32'h5;
       RomData28[159] = -32'h1;
       RomData28[160] = -32'h1;
       RomData28[161] = -32'h5;
       RomData28[162] = 32'h0;
       RomData28[163] = 32'h0;
       RomData28[164] = -32'h5;
       RomData28[165] = -32'h2;
       RomData28[166] = 32'h0;
       RomData28[167] = -32'h2;
       RomData28[168] = 32'h5;
       RomData28[169] = -32'h3;
       RomData28[170] = 32'h0;
       RomData28[171] = 32'h1;
       RomData28[172] = -32'h1;
       RomData28[173] = 32'h1;
       RomData28[174] = 32'h3;
       RomData28[175] = 32'h0;
       RomData28[176] = 32'h0;
       RomData28[177] = -32'h2;
       RomData28[178] = -32'h3;
       RomData28[179] = -32'h4;
       RomData28[180] = -32'h2;
       RomData28[181] = -32'h6;
       RomData28[182] = -32'h4;
       RomData28[183] = -32'h3;
       RomData28[184] = 32'h2;
       RomData28[185] = -32'h4;
       RomData28[186] = -32'h2;
       RomData28[187] = 32'h2;
       RomData28[188] = -32'h3;
       RomData28[189] = 32'h6;
       RomData28[190] = 32'h4;
       RomData28[191] = -32'h3;
       RomData28[192] = -32'h3;
       RomData28[193] = -32'h2;
       RomData28[194] = -32'h3;
       RomData28[195] = -32'h3;
       RomData28[196] = -32'h2;
       RomData28[197] = 32'h2;
       RomData28[198] = -32'h2;
       RomData28[199] = -32'h1;
       RomData28[200] = -32'h1;
       RomData28[201] = -32'h5;
       RomData28[202] = -32'h3;
       RomData28[203] = -32'h2;
       RomData28[204] = 32'h0;
       RomData28[205] = -32'h3;
       RomData28[206] = -32'h2;
       RomData28[207] = 32'h2;
       RomData28[208] = 32'h0;
       RomData28[209] = 32'h4;
       RomData28[210] = 32'h6;
       RomData28[211] = -32'h2;
       RomData28[212] = -32'h2;
       RomData28[213] = -32'h1;
       RomData28[214] = 32'h0;
       RomData28[215] = -32'h2;
       RomData28[216] = -32'h1;
       RomData28[217] = 32'h2;
       RomData28[218] = -32'h4;
       RomData28[219] = -32'h2;
       RomData28[220] = 32'h0;
       RomData28[221] = -32'h4;
       RomData28[222] = 32'h2;
       RomData28[223] = 32'h1;
       RomData28[224] = -32'h4;
       RomData28[225] = 32'h0;
       RomData28[226] = 32'h2;
       RomData28[227] = -32'h2;
       RomData28[228] = 32'h1;
       RomData28[229] = -32'h3;
       RomData28[230] = -32'h2;
       RomData28[231] = 32'h2;
       RomData28[232] = -32'h1;
       RomData28[233] = 32'h1;
       RomData28[234] = 32'h0;
       RomData28[235] = 32'h1;
       RomData28[236] = 32'h0;
       RomData28[237] = -32'h2;
       RomData28[238] = -32'h4;
       RomData28[239] = -32'h2;
       RomData28[240] = 32'h1;
       RomData28[241] = -32'h3;
       RomData28[242] = -32'h1;
       RomData28[243] = -32'h1;
       RomData28[244] = -32'h5;
       RomData28[245] = -32'h1;
       RomData28[246] = 32'h0;
       RomData28[247] = -32'h2;
       RomData28[248] = -32'h1;
       RomData28[249] = -32'h3;
       RomData28[250] = -32'h2;
       RomData28[251] = -32'h1;
       RomData28[252] = 32'h6;
       RomData28[253] = 32'h0;
       RomData28[254] = 32'h0;
       RomData28[255] = 32'h1;
       RomData28[256] = 32'h0;
       RomData28[257] = -32'h1;
       RomData28[258] = -32'h6;
       RomData28[259] = -32'h5;
       RomData28[260] = 32'h0;
       RomData28[261] = -32'h5;
       RomData28[262] = 32'h2;
       RomData28[263] = 32'h2;
       RomData28[264] = -32'h5;
       RomData28[265] = -32'h1;
       RomData28[266] = 32'h3;
       RomData28[267] = -32'h2;
       RomData28[268] = 32'h1;
       RomData28[269] = -32'h2;
       RomData28[270] = -32'h1;
       RomData28[271] = 32'h1;
       RomData28[272] = 32'h0;
       RomData28[273] = 32'h4;
       RomData28[274] = 32'h1;
       RomData28[275] = -32'h1;
       RomData28[276] = -32'h1;
       RomData28[277] = -32'h2;
       RomData28[278] = -32'h5;
       RomData28[279] = -32'h4;
       RomData28[280] = -32'h2;
       RomData28[281] = -32'h4;
       RomData28[282] = -32'h1;
       RomData28[283] = -32'h1;
       RomData28[284] = -32'h4;
       RomData28[285] = -32'h3;
       RomData28[286] = 32'h2;
       RomData28[287] = -32'h2;
       RomData28[288] = 32'h3;
       RomData28[289] = -32'h3;
       RomData28[290] = 32'h0;
       RomData28[291] = 32'h0;
       RomData28[292] = 32'h0;
       RomData28[293] = 32'h1;
       RomData28[294] = 32'h6;
       RomData28[295] = 32'h0;
       RomData28[296] = -32'h1;
       RomData28[297] = -32'h2;
       RomData28[298] = 32'h2;
       RomData28[299] = -32'h4;
       RomData28[300] = 32'h1;
       RomData28[301] = 32'h0;
       RomData28[302] = 32'h0;
       RomData28[303] = 32'h0;
       RomData28[304] = -32'h3;
       RomData28[305] = 32'h1;
       RomData28[306] = -32'h1;
       RomData28[307] = -32'h1;
       RomData28[308] = 32'h0;
       RomData28[309] = -32'h3;
       RomData28[310] = -32'h2;
       RomData28[311] = 32'h1;
       RomData28[312] = 32'h1;
       RomData28[313] = -32'h1;
       RomData28[314] = 32'h0;
       RomData28[315] = 32'h2;
       RomData28[316] = 32'h1;
       RomData28[317] = -32'h1;
       RomData28[318] = -32'h2;
       RomData28[319] = -32'h3;
       RomData28[320] = 32'h1;
       RomData28[321] = -32'h2;
       RomData28[322] = 32'h0;
       RomData28[323] = 32'h0;
       RomData28[324] = -32'h3;
       RomData28[325] = 32'h0;
       RomData28[326] = -32'h1;
       RomData28[327] = 32'h0;
       RomData28[328] = 32'h0;
       RomData28[329] = -32'h2;
       RomData28[330] = -32'h1;
       RomData28[331] = 32'h0;
       RomData28[332] = 32'h0;
       RomData28[333] = -32'h1;
       RomData28[334] = -32'h1;
       RomData28[335] = 32'h1;
       RomData28[336] = 32'h3;
       RomData28[337] = 32'h0;
       RomData28[338] = -32'h5;
       RomData28[339] = -32'h3;
       RomData28[340] = 32'h0;
       RomData28[341] = -32'h2;
       RomData28[342] = -32'h2;
       RomData28[343] = -32'h2;
       RomData28[344] = -32'h1;
       RomData28[345] = -32'h1;
       RomData28[346] = -32'h2;
       RomData28[347] = 32'h4;
       RomData28[348] = -32'h2;
       RomData28[349] = 32'h2;
       RomData28[350] = 32'h2;
       RomData28[351] = -32'h2;
       RomData28[352] = -32'h1;
       RomData28[353] = -32'h2;
       RomData28[354] = -32'h2;
       RomData28[355] = -32'h1;
       RomData28[356] = 32'h0;
       RomData28[357] = 32'h4;
       RomData28[358] = -32'h6;
       RomData28[359] = -32'h2;
       RomData28[360] = -32'h6;
       RomData28[361] = -32'h8;
       RomData28[362] = -32'h7;
       RomData28[363] = -32'h7;
       RomData28[364] = 32'h0;
       RomData28[365] = -32'h7;
       RomData28[366] = -32'h3;
       RomData28[367] = -32'h5;
       RomData28[368] = -32'h3;
       RomData28[369] = -32'h2;
       RomData28[370] = -32'h4;
       RomData28[371] = -32'h4;
       RomData28[372] = -32'h6;
       RomData28[373] = -32'h5;
       RomData28[374] = 32'h2;
       RomData28[375] = -32'h2;
       RomData28[376] = -32'h5;
       RomData28[377] = -32'h6;
       RomData28[378] = 32'h11;
       RomData28[379] = 32'h0;
       RomData28[380] = -32'h3;
       RomData28[381] = 32'h0;
       RomData28[382] = -32'h4;
       RomData28[383] = -32'h4;
       RomData28[384] = 32'h7;
       RomData28[385] = -32'h5;
       RomData28[386] = 32'h0;
       RomData28[387] = -32'h1;
       RomData28[388] = -32'h4;
       RomData28[389] = -32'h1;
       RomData28[390] = -32'h2;
       RomData28[391] = -32'h2;
       RomData28[392] = -32'h5;
       RomData28[393] = -32'h4;
       RomData28[394] = -32'h4;
       RomData28[395] = -32'h3;
       RomData28[396] = -32'h3;
       RomData28[397] = -32'h2;
       RomData28[398] = 32'h0;
       RomData28[399] = 32'ha;
       end 
      

// Structural Resource (FU) inventory for AS_SSROM_A_SINT_CC_SCALbx22_ARA0:// 400 array locations of width 32
// Total state bits in module = 12800 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:50:02
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -obj-dir-name=. -res2-loglevel=0 -log-dir-name=d_obj_sw_test_b sw_test.exe -vnl=sw_test -vnl-rootmodname=DUT -vnl-resets=synchronous


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-------------------------+---------+--------------------------------------------------------------------+---------------+------------------------------+--------+-------*
//| Class                   | Style   | Dir Style                                                          | Timing Target | Method                       | UID    | Skip  |
//*-------------------------+---------+--------------------------------------------------------------------+---------------+------------------------------+--------+-------*
//| sw_test_single_threaded | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | sw_test_single_threaded.Main | Main10 | false |
//|                         |         | dmode, enable/directorate-pc-export                                |               |                              |        |       |
//*-------------------------+---------+--------------------------------------------------------------------+---------------+------------------------------+--------+-------*

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
//KiwiC: front end input processing of class SmithWaterman.Program_t  wonky=SmithWaterman igrf=false
//
//
//root_compiler: method compile: entry point. Method name=SmithWaterman.Program_t..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=SmithWaterman.Program_t..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class CRC32checker  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=CRC32checker..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=CRC32checker..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 3/prev
//
//
//KiwiC: front end input processing of class sw_test_single_threaded  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=sw_test_single_threaded.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=sw_test_single_threaded.Main
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
//   srcfile=sw_test.exe
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

//Report from restructure2:::
//PC codings points for kiwiSWTMAIN400PC10 
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                   | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiSWTMAIN400PC10"   | 907 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 3    |
//| XU32'2:"2:kiwiSWTMAIN400PC10"   | 904 | 1       | hwm=1.1.0   | 2    |        | -     | -   | 4    |
//| XU32'2:"2:kiwiSWTMAIN400PC10"   | 905 | 1       | hwm=1.1.0   | 2    |        | -     | -   | 5    |
//| XU32'1:"1:kiwiSWTMAIN400PC10"   | 906 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 1    |
//| XU32'3:"3:kiwiSWTMAIN400PC10"   | 902 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'3:"3:kiwiSWTMAIN400PC10"   | 903 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiSWTMAIN400PC10"   | 900 | 5       | hwm=0.0.1   | 5    |        | 7     | 7   | 4    |
//| XU32'4:"4:kiwiSWTMAIN400PC10"   | 901 | 5       | hwm=0.0.1   | 5    |        | 6     | 6   | 8    |
//| XU32'5:"5:kiwiSWTMAIN400PC10"   | 898 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 49   |
//| XU32'5:"5:kiwiSWTMAIN400PC10"   | 899 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 52   |
//| XU32'12:"12:kiwiSWTMAIN400PC10" | 885 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'12:"12:kiwiSWTMAIN400PC10" | 886 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 13   |
//| XU32'13:"13:kiwiSWTMAIN400PC10" | 883 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 9    |
//| XU32'13:"13:kiwiSWTMAIN400PC10" | 884 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'14:"14:kiwiSWTMAIN400PC10" | 882 | 11      | hwm=0.1.0   | 12   |        | 12    | 12  | 10   |
//| XU32'15:"15:kiwiSWTMAIN400PC10" | 881 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 21   |
//| XU32'8:"8:kiwiSWTMAIN400PC10"   | 892 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 43   |
//| XU32'8:"8:kiwiSWTMAIN400PC10"   | 893 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 8    |
//| XU32'17:"17:kiwiSWTMAIN400PC10" | 878 | 15      | hwm=0.1.0   | 16   |        | 16    | 16  | 17   |
//| XU32'18:"18:kiwiSWTMAIN400PC10" | 877 | 17      | hwm=0.1.0   | 18   |        | 18    | 18  | 19   |
//| XU32'19:"19:kiwiSWTMAIN400PC10" | 876 | 19      | hwm=0.1.0   | 20   |        | 20    | 20  | 23   |
//| XU32'16:"16:kiwiSWTMAIN400PC10" | 879 | 21      | hwm=0.0.0   | 21   |        | -     | -   | 14   |
//| XU32'16:"16:kiwiSWTMAIN400PC10" | 880 | 21      | hwm=0.1.0   | 22   |        | 22    | 22  | 15   |
//| XU32'20:"20:kiwiSWTMAIN400PC10" | 875 | 23      | hwm=0.1.0   | 24   |        | 24    | 24  | 21   |
//| XU32'21:"21:kiwiSWTMAIN400PC10" | 874 | 25      | hwm=0.1.1   | 26   |        | 26    | 27  | 28   |
//| XU32'22:"22:kiwiSWTMAIN400PC10" | 873 | 28      | hwm=0.1.1   | 29   |        | 29    | 30  | 31   |
//| XU32'23:"23:kiwiSWTMAIN400PC10" | 872 | 31      | hwm=0.1.0   | 32   |        | 32    | 32  | 33   |
//| XU32'24:"24:kiwiSWTMAIN400PC10" | 871 | 33      | hwm=0.1.0   | 34   |        | 34    | 34  | 35   |
//| XU32'25:"25:kiwiSWTMAIN400PC10" | 870 | 35      | hwm=0.1.1   | 36   |        | 36    | 37  | 39   |
//| XU32'11:"11:kiwiSWTMAIN400PC10" | 887 | 38      | hwm=0.0.0   | 38   |        | -     | -   | 9    |
//| XU32'11:"11:kiwiSWTMAIN400PC10" | 888 | 38      | hwm=0.0.0   | 38   |        | -     | -   | 25   |
//| XU32'26:"26:kiwiSWTMAIN400PC10" | 869 | 39      | hwm=0.1.1   | 40   |        | 40    | 41  | 38   |
//| XU32'10:"10:kiwiSWTMAIN400PC10" | 889 | 42      | hwm=0.0.0   | 42   |        | -     | -   | 38   |
//| XU32'9:"9:kiwiSWTMAIN400PC10"   | 890 | 43      | hwm=0.0.0   | 43   |        | -     | -   | 42   |
//| XU32'9:"9:kiwiSWTMAIN400PC10"   | 891 | 43      | hwm=0.0.0   | 43   |        | -     | -   | 44   |
//| XU32'27:"27:kiwiSWTMAIN400PC10" | 867 | 44      | hwm=1.1.0   | 45   |        | -     | -   | 43   |
//| XU32'27:"27:kiwiSWTMAIN400PC10" | 868 | 44      | hwm=1.1.0   | 45   |        | -     | -   | 42   |
//| XU32'7:"7:kiwiSWTMAIN400PC10"   | 894 | 46      | hwm=0.0.0   | 46   |        | -     | -   | 14   |
//| XU32'7:"7:kiwiSWTMAIN400PC10"   | 895 | 46      | hwm=0.0.0   | 46   |        | -     | -   | 47   |
//| XU32'28:"28:kiwiSWTMAIN400PC10" | 866 | 47      | hwm=0.0.1   | 47   |        | 48    | 48  | 46   |
//| XU32'6:"6:kiwiSWTMAIN400PC10"   | 896 | 49      | hwm=0.0.0   | 49   |        | -     | -   | 46   |
//| XU32'6:"6:kiwiSWTMAIN400PC10"   | 897 | 49      | hwm=0.0.0   | 49   |        | -     | -   | 50   |
//| XU32'29:"29:kiwiSWTMAIN400PC10" | 865 | 50      | hwm=0.0.1   | 50   |        | 51    | 51  | 49   |
//| XU32'30:"30:kiwiSWTMAIN400PC10" | 864 | 52      | hwm=0.0.0   | 52   |        | -     | -   | 53   |
//| XU32'31:"31:kiwiSWTMAIN400PC10" | 863 | 53      | hwm=0.0.0   | 53   |        | -     | -   | 54   |
//| XU32'32:"32:kiwiSWTMAIN400PC10" | 862 | 54      | hwm=0.1.0   | 55   |        | 55    | 55  | 56   |
//| XU32'33:"33:kiwiSWTMAIN400PC10" | 861 | 56      | hwm=0.0.0   | 56   |        | -     | -   | 57   |
//| XU32'34:"34:kiwiSWTMAIN400PC10" | 860 | 57      | hwm=0.1.0   | 58   |        | 58    | 58  | 59   |
//| XU32'35:"35:kiwiSWTMAIN400PC10" | 859 | 59      | hwm=0.1.0   | 60   |        | 60    | 60  | 61   |
//| XU32'36:"36:kiwiSWTMAIN400PC10" | 858 | 61      | hwm=0.0.0   | 61   |        | -     | -   | 62   |
//| XU32'37:"37:kiwiSWTMAIN400PC10" | 857 | 62      | hwm=0.1.0   | 63   |        | 63    | 63  | 64   |
//| XU32'38:"38:kiwiSWTMAIN400PC10" | 856 | 64      | hwm=0.0.0   | 64   |        | -     | -   | 65   |
//| XU32'39:"39:kiwiSWTMAIN400PC10" | 855 | 65      | hwm=0.1.0   | 66   |        | 66    | 66  | 67   |
//| XU32'40:"40:kiwiSWTMAIN400PC10" | 854 | 67      | hwm=0.0.0   | 67   |        | -     | -   | -    |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'0:"0:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'0:"0:kiwiSWTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                                     |
//| F0   | E907 | R0 DATA |                                                                                                                                                                     |
//| F0+E | E907 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0write(S32'0) phasewrite(U32'2) @_UINT/CC/S\ |
//|      |      |         | CALbx14_byteno10write(U32'0) SW_TMAIN400.SmithWaterman.Program_t.encode.1.8._SPILL.256write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0write(S32'0\ |
//|      |      |         | ) SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1write(S32'0) SW_TMAIN400.SmithWaterman.Prog\ |
//|      |      |         | ram_t.encode.0.17.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.encode.0.17._SPILL.256write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3wri\ |
//|      |      |         | te(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1write(S32'0) SW_TMAIN400.SmithW\ |
//|      |      |         | aterman.Program_t.next_data.2.10.V_7write(S64'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_2write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.\ |
//|      |      |         | 2.10.V_4write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2write(S32'0) SW_TM\ |
//|      |      |         | AIN400.SmithWaterman.Program_t.testReport.2.13.V_1write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1write(S32'0) SW_TMAIN400.SmithWaterman.Progra\ |
//|      |      |         | m_t.crcReport.2.16.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2write(S64'0) SW_TMAIN400.CRC32checker.process_byte.0.15.V_0write(U32'0) S\ |
//|      |      |         | W_TMAIN400.CRC32checker.process_byte.0.23.V_0write(U32'0) SW_TMAIN400.CRC32checker.process_byte.0.31.V_0write(U32'0) SW_TMAIN400.CRC32checker.process_byte.0.39.V_\ |
//|      |      |         | 0write(U32'0) SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0write(S32'0) SW_TMAIN400.CRC32checker.process_byte.0.40.V_0write(U32'0) SW_TMAIN400.CRC32checker\ |
//|      |      |         | .process_byte.0.44.V_0write(U32'0)  PLI:waypoint %d %d  PLI:Smith Waterman Simpl...                                                                                 |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'2:"2:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'2:"2:kiwiSWTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                               |
//*------+------+---------+--------------------------------------------------------------------*
//| F1   | -    | R0 CTRL | Z16SSCCSCALbx20ARC0_10_read(E1)                                    |
//| F2   | -    | R1 CTRL |                                                                    |
//| F1   | E905 | R0 DATA |                                                                    |
//| F2   | E905 | R1 DATA |                                                                    |
//| F2+E | E905 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.1.8._SPILL.256write(E2) |
//| F1   | E904 | R0 DATA |                                                                    |
//| F2   | E904 | R1 DATA |                                                                    |
//| F2+E | E904 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0write(E3)        |
//*------+------+---------+--------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'1:"1:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'1:"1:kiwiSWTMAIN400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F3   | -    | R0 CTRL |      |
//| F3   | E906 | R0 DATA |      |
//| F3+E | E906 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'3:"3:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'3:"3:kiwiSWTMAIN400PC10"
//*------+------+---------+-----------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                  |
//*------+------+---------+-----------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                       |
//| F4   | E903 | R0 DATA |                                                                       |
//| F4+E | E903 | W0 DATA |                                                                       |
//| F4   | E902 | R0 DATA |                                                                       |
//| F4+E | E902 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.1.8._SPILL.256write(S32'0) |
//*------+------+---------+-----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'4:"4:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'4:"4:kiwiSWTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                       |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                                                                                            |
//| F5   | E901 | R0 DATA |                                                                                                                                            |
//| F5+E | E901 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0write(E4) phasewrite(U32'3) SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0\ |
//|      |      |         | write(S32'0) @_SINT/CC/SCALbx24_ARB0_write(E1, E5)  PLI:waypoint %d %d                                                                     |
//| F6   | E901 | W1 DATA |                                                                                                                                            |
//| F5   | E900 | R0 DATA |                                                                                                                                            |
//| F5+E | E900 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0write(E4) @_SINT/\ |
//|      |      |         | CC/SCALbx24_ARB0_write(E1, E5)                                                                                                             |
//| F7   | E900 | W1 DATA |                                                                                                                                            |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'5:"5:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'5:"5:kiwiSWTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                          |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                                                                                                               |
//| F8   | E899 | R0 DATA |                                                                                                                                                               |
//| F8+E | E899 | W0 DATA | phasewrite(U32'12) @_UINT/CC/SCALbx12_crcreg10write(U32'4294967295)  PLI:crc reg now:  reg=%X  PLI:CRC RESET  PLI:Smith Waterman CRC s...  PLI:waypoint %d %d |
//| F8   | E898 | R0 DATA |                                                                                                                                                               |
//| F8+E | E898 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0write(S32'0)                                                                                              |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'12:"12:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'12:"12:kiwiSWTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                 |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                                                                                                      |
//| F9   | E886 | R0 DATA |                                                                                                                                      |
//| F9+E | E886 | W0 DATA | phasewrite(E6) @_UINT/CC/SCALbx14_crcreg10write(U32'4294967295)  PLI:crc reg now:  reg=%X  PLI:CRC RESET  PLI:waypoint %d %d         |
//| F9   | E885 | R0 DATA |                                                                                                                                      |
//| F9+E | E885 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_1write(E7) |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'13:"13:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'13:"13:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F10   | -    | R0 CTRL |                                                                  |
//| F10   | E884 | R0 DATA |                                                                  |
//| F10+E | E884 | W0 DATA |                                                                  |
//| F10   | E883 | R0 DATA |                                                                  |
//| F10+E | E883 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0write(E8) |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'14:"14:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'14:"14:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                       |
//*-------+------+---------+--------------------------------------------------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                                                            |
//| F11   | E882 | R0 DATA | @64_SS/CC/SCALbx52_ARA0_read(E9)                                                           |
//| F12   | E882 | R1 DATA |                                                                                            |
//| F12+E | E882 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2write(E10)  PLI:  PLI:%d %d : %d    |
//*-------+------+---------+--------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'15:"15:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'15:"15:kiwiSWTMAIN400PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F13   | -    | R0 CTRL |                                                                                                                                     |
//| F13   | E881 | R0 DATA |                                                                                                                                     |
//| F13+E | E881 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_0write(E11) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'8:"8:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'8:"8:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                       |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F14   | -    | R0 CTRL |                                                                                                                                                            |
//| F14   | E893 | R0 DATA |                                                                                                                                                            |
//| F14+E | E893 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0write(E12)                                                                                             |
//| F14   | E892 | R0 DATA |                                                                                                                                                            |
//| F14+E | E892 | W0 DATA | phasewrite(U32'4) SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0write(E13) SW_TMA\ |
//|       |      |         | IN400.SmithWaterman.Program_t.next_data.2.10.V_1write(E11)  PLI:waypoint %d %d                                                                             |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'17:"17:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'17:"17:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                 |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F15   | -    | R0 CTRL |                                                                                                                                      |
//| F15   | E878 | R0 DATA | UINTCCSCALbx10ARA0_22_read(E14) UINTCCSCALbx10ARA0_32_read(E15)                                                                      |
//| F16   | E878 | R1 DATA |                                                                                                                                      |
//| F16+E | E878 | W0 DATA | @_UINT/CC/SCALbx14_byteno10write(E16) SW_TMAIN400.CRC32checker.process_byte.0.23.V_0write(E17) @_UINT/CC/SCALbx14_crcreg10write(E18) |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'18:"18:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'18:"18:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                 |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F17   | -    | R0 CTRL |                                                                                                                                      |
//| F17   | E877 | R0 DATA | UINTCCSCALbx10ARA0_28_read(E19) UINTCCSCALbx10ARA0_30_read(E20)                                                                      |
//| F18   | E877 | R1 DATA |                                                                                                                                      |
//| F18+E | E877 | W0 DATA | @_UINT/CC/SCALbx14_byteno10write(E16) SW_TMAIN400.CRC32checker.process_byte.0.31.V_0write(E21) @_UINT/CC/SCALbx14_crcreg10write(E22) |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'19:"19:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'19:"19:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                 |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F19   | -    | R0 CTRL |                                                                                                                                      |
//| F19   | E876 | R0 DATA | UINTCCSCALbx10ARA0_24_read(E23) UINTCCSCALbx10ARA0_26_read(E24)                                                                      |
//| F20   | E876 | R1 DATA |                                                                                                                                      |
//| F20+E | E876 | W0 DATA | @_UINT/CC/SCALbx14_byteno10write(E16) SW_TMAIN400.CRC32checker.process_byte.0.39.V_0write(E25) @_UINT/CC/SCALbx14_crcreg10write(E26) |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'16:"16:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'16:"16:kiwiSWTMAIN400PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                          |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| F21   | -    | R0 CTRL |                                                                                                                               |
//| F21   | E880 | R0 DATA | @64_SS/CC/SCALbx52_ARA0_read(E27)                                                                                             |
//| F22   | E880 | R1 DATA |                                                                                                                               |
//| F22+E | E880 | W0 DATA | @_UINT/CC/SCALbx14_byteno10write(E16) SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2write(E28) SW_TMAIN400.CRC32chec\ |
//|       |      |         | ker.process_byte.0.15.V_0write(E29)  PLI:process unit word %u...                                                              |
//| F21   | E879 | R0 DATA |                                                                                                                               |
//| F21+E | E879 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1write(E30) d_monitorwrite(E31)  PLI:step %d crc is %u   ...               |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'20:"20:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'20:"20:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                   |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------*
//| F23   | -    | R0 CTRL |                                                                                                        |
//| F23   | E875 | R0 DATA | UINTCCSCALbx10ARA0_20_read(E32) UINTCCSCALbx10ARA0_22_read(E14)                                        |
//| F24   | E875 | R1 DATA |                                                                                                        |
//| F24+E | E875 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1write(E33) @_UINT/CC/SCALbx14_crcreg10write(E34) |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'21:"21:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'21:"21:kiwiSWTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                     |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| F25   | -    | R0 CTRL |                                                                                                          |
//| F25   | E874 | R0 DATA | @64_SS/CC/SCALbx56_ARC0_read(E35) @64_SS/CC/SCALbx54_ARB0_read(E35) @_SINT/CC/SCALbx24_ARB0_read(E36)    |
//| F26   | E874 | R1 DATA |                                                                                                          |
//| F26+E | E874 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_4write(E37) @64_SS/CC/SCALbx56_ARC0_write(E38, E39) |
//| F27   | E874 | W1 DATA |                                                                                                          |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'22:"22:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'22:"22:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------*
//| F28   | -    | R0 CTRL |                                                                                 |
//| F28   | E873 | R0 DATA | @64_SS/CC/SCALbx58_ARD0_read(E40) @64_SS/CC/SCALbx54_ARB0_read(E40)             |
//| F29   | E873 | R1 DATA |                                                                                 |
//| F29+E | E873 | W0 DATA | @64_SS/CC/SCALbx56_ARC0_write(E38, E41) @64_SS/CC/SCALbx58_ARD0_write(E38, E42) |
//| F30   | E873 | W1 DATA |                                                                                 |
//*-------+------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'23:"23:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'23:"23:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                |
//*-------+------+---------+---------------------------------------------------------------------*
//| F31   | -    | R0 CTRL |                                                                     |
//| F31   | E872 | R0 DATA | @64_SS/CC/SCALbx56_ARC0_read(E43) @64_SS/CC/SCALbx58_ARD0_read(E43) |
//| F32   | E872 | R1 DATA |                                                                     |
//| F32+E | E872 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_7write(E44)    |
//*-------+------+---------+---------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'24:"24:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'24:"24:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F33   | -    | R0 CTRL |                                                                  |
//| F33   | E871 | R0 DATA | @64_SS/CC/SCALbx52_ARA0_read(E43)                                |
//| F34   | E871 | R1 DATA |                                                                  |
//| F34+E | E871 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_7write(E45) |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'25:"25:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'25:"25:kiwiSWTMAIN400PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F35   | -    | R0 CTRL |                                         |
//| F35   | E870 | R0 DATA | SINTCCSCALbx22ARA0_10_read(E46)         |
//| F36   | E870 | R1 DATA |                                         |
//| F36+E | E870 | W0 DATA | @64_SS/CC/SCALbx52_ARA0_write(E38, E47) |
//| F37   | E870 | W1 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'11:"11:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'11:"11:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F38   | -    | R0 CTRL |                                                                                                                                 |
//| F38   | E888 | R0 DATA |                                                                                                                                 |
//| F38+E | E888 | W0 DATA |                                                                                                                                 |
//| F38   | E887 | R0 DATA |                                                                                                                                 |
//| F38+E | E887 | W0 DATA | phasewrite(E48) SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0write(S32'0)  PLI:Scored h matrix %d  PLI:waypoint %d %d |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'26:"26:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'26:"26:kiwiSWTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                     |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*
//| F39   | -    | R0 CTRL |                                                                                                          |
//| F39   | E869 | R0 DATA | @64_SS/CC/SCALbx52_ARA0_read(E38)                                                                        |
//| F40   | E869 | R1 DATA |                                                                                                          |
//| F40+E | E869 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3write(E49) @64_SS/CC/SCALbx54_ARB0_write(E38, E50) |
//| F41   | E869 | W1 DATA |                                                                                                          |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'10:"10:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'10:"10:kiwiSWTMAIN400PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F42   | -    | R0 CTRL |                                                                                                                                     |
//| F42   | E889 | R0 DATA |                                                                                                                                     |
//| F42+E | E889 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3write(S32'0) SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_2write(E51) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'9:"9:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'9:"9:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                   |
//*-------+------+---------+------------------------------------------------------------------------*
//| F43   | -    | R0 CTRL |                                                                        |
//| F43   | E891 | R0 DATA |                                                                        |
//| F43+E | E891 | W0 DATA |                                                                        |
//| F43   | E890 | R0 DATA |                                                                        |
//| F43+E | E890 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.0.17._SPILL.256write(S32'0) |
//*-------+------+---------+------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'27:"27:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'27:"27:kiwiSWTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F44   | -    | R0 CTRL | Z16SSCCSCALbx16ARA0_10_read(E52)                                     |
//| F45   | -    | R1 CTRL |                                                                      |
//| F44   | E868 | R0 DATA |                                                                      |
//| F45   | E868 | R1 DATA |                                                                      |
//| F45+E | E868 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.0.17._SPILL.256write(E53) |
//| F44   | E867 | R0 DATA |                                                                      |
//| F45   | E867 | R1 DATA |                                                                      |
//| F45+E | E867 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0write(E54)        |
//*-------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'7:"7:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'7:"7:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F46   | -    | R0 CTRL |                                                                  |
//| F46   | E895 | R0 DATA |                                                                  |
//| F46+E | E895 | W0 DATA |                                                                  |
//| F46   | E894 | R0 DATA |                                                                  |
//| F46+E | E894 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1write(S32'0) |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'28:"28:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'28:"28:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                                       |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F47   | -    | R0 CTRL |                                                                                                                                                            |
//| F47   | E866 | R0 DATA |                                                                                                                                                            |
//| F47+E | E866 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1write(E55) @64_SS/CC/SCALbx56_ARC0_write(E56, S64'0) @64_SS/CC/SCALbx58_ARD0_write(E56, S64'0) @64_SS\ |
//|       |      |         | /CC/SCALbx52_ARA0_write(E56, S64'0) @64_SS/CC/SCALbx54_ARB0_write(E56, S64'-10)                                                                            |
//| F48   | E866 | W1 DATA |                                                                                                                                                            |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'6:"6:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'6:"6:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                             |
//*-------+------+---------+------------------------------------------------------------------*
//| F49   | -    | R0 CTRL |                                                                  |
//| F49   | E897 | R0 DATA |                                                                  |
//| F49+E | E897 | W0 DATA |                                                                  |
//| F49   | E896 | R0 DATA |                                                                  |
//| F49+E | E896 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1write(S32'1) |
//*-------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'29:"29:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'29:"29:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                             |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------*
//| F50   | -    | R0 CTRL |                                                                                                                  |
//| F50   | E865 | R0 DATA |                                                                                                                  |
//| F50+E | E865 | W0 DATA | SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0write(E57) d_monitorwrite(E58) @64_SS/CC/SCALbx56_ARC0_writ\ |
//|       |      |         | e(E59, S64'0) @64_SS/CC/SCALbx58_ARD0_write(E59, S64'0) @64_SS/CC/SCALbx52_ARA0_write(E59, S64'0) @64_SS/CC/SCA\ |
//|       |      |         | Lbx54_ARB0_write(E59, S64'-10)                                                                                   |
//| F51   | E865 | W1 DATA |                                                                                                                  |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'30:"30:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'30:"30:kiwiSWTMAIN400PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F52   | -    | R0 CTRL |      |
//| F52   | E864 | R0 DATA |      |
//| F52+E | E864 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'31:"31:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'31:"31:kiwiSWTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                             |
//*-------+------+---------+--------------------------------------------------*
//| F53   | -    | R0 CTRL |                                                  |
//| F53   | E863 | R0 DATA |                                                  |
//| F53+E | E863 | W0 DATA | @_UINT/CC/SCALbx12_crcreg10write(U32'4294967295) |
//*-------+------+---------+--------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'32:"32:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'32:"32:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------*
//| pc    | eno  | Phaser  | Work                            |
//*-------+------+---------+---------------------------------*
//| F54   | -    | R0 CTRL |                                 |
//| F54   | E862 | R0 DATA | UINTCCSCALbx10ARA0_18_read(255) |
//| F55   | E862 | R1 DATA |                                 |
//| F55+E | E862 | W0 DATA |  PLI:self test startup %u...    |
//*-------+------+---------+---------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'33:"33:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'33:"33:kiwiSWTMAIN400PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F56   | -    | R0 CTRL |                              |
//| F56   | E861 | R0 DATA |                              |
//| F56+E | E861 | W0 DATA |  PLI:self test startup %u... |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'34:"34:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'34:"34:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------*
//| pc    | eno  | Phaser  | Work                            |
//*-------+------+---------+---------------------------------*
//| F57   | -    | R0 CTRL |                                 |
//| F57   | E860 | R0 DATA | UINTCCSCALbx10ARA0_18_read(255) |
//| F58   | E860 | R1 DATA |                                 |
//| F58+E | E860 | W0 DATA |  PLI:self test startup %u...    |
//*-------+------+---------+---------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'35:"35:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'35:"35:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                          |
//*-------+------+---------+---------------------------------------------------------------*
//| F59   | -    | R0 CTRL |                                                               |
//| F59   | E859 | R0 DATA | UINTCCSCALbx10ARA0_12_read(1) UINTCCSCALbx10ARA0_18_read(255) |
//| F60   | E859 | R1 DATA |                                                               |
//| F60+E | E859 | W0 DATA | @_UINT/CC/SCALbx12_crcreg10write(E60)                         |
//*-------+------+---------+---------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'36:"36:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'36:"36:kiwiSWTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                     |
//*-------+------+---------+----------------------------------------------------------*
//| F61   | -    | R0 CTRL |                                                          |
//| F61   | E858 | R0 DATA |                                                          |
//| F61+E | E858 | W0 DATA | SW_TMAIN400.CRC32checker.process_byte.0.40.V_0write(E61) |
//*-------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'37:"37:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'37:"37:kiwiSWTMAIN400PC10"
//*-------+------+---------+-----------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                            |
//*-------+------+---------+-----------------------------------------------------------------*
//| F62   | -    | R0 CTRL |                                                                 |
//| F62   | E857 | R0 DATA | UINTCCSCALbx10ARA0_14_read(E62) UINTCCSCALbx10ARA0_16_read(128) |
//| F63   | E857 | R1 DATA |                                                                 |
//| F63+E | E857 | W0 DATA | @_UINT/CC/SCALbx12_crcreg10write(E63)                           |
//*-------+------+---------+-----------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'38:"38:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'38:"38:kiwiSWTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                     |
//*-------+------+---------+----------------------------------------------------------*
//| F64   | -    | R0 CTRL |                                                          |
//| F64   | E856 | R0 DATA |                                                          |
//| F64+E | E856 | W0 DATA | SW_TMAIN400.CRC32checker.process_byte.0.44.V_0write(E61) |
//*-------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'39:"39:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'39:"39:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                          |
//*-------+------+---------+---------------------------------------------------------------*
//| F65   | -    | R0 CTRL |                                                               |
//| F65   | E855 | R0 DATA | UINTCCSCALbx10ARA0_10_read(E64) UINTCCSCALbx10ARA0_12_read(1) |
//| F66   | E855 | R1 DATA |                                                               |
//| F66+E | E855 | W0 DATA | @_UINT/CC/SCALbx12_crcreg10write(E65)                         |
//*-------+------+---------+---------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'40:"40:kiwiSWTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiSWTMAIN400PC10 state=XU32'40:"40:kiwiSWTMAIN400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------*
//| F67   | -    | R0 CTRL |                                                                                 |
//| F67   | E854 | R0 DATA |                                                                                 |
//| F67+E | E854 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:Smith Waterman Simpl...  PLI:self test yields %u ... |
//*-------+------+---------+---------------------------------------------------------------------------------*

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
//  E1 =.= SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0
//
//
//  E2 =.= C(SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0)
//
//
//  E3 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0)
//
//
//  E4 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0)
//
//
//  E5 =.= C(SW_TMAIN400.SmithWaterman.Program_t.encode.1.8._SPILL.256)
//
//
//  E6 =.= Cu(10+256*SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1)
//
//
//  E7 =.= C((SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0+SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1)%2)
//
//
//  E8 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0)
//
//
//  E9 =.= CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_1)))
//
//
//  E10 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2)
//
//
//  E11 =.= C((1+SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1)%2)
//
//
//  E12 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0)
//
//
//  E13 =.= C(SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1%2)
//
//
//  E14 =.= CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2)))
//
//
//  E15 =.= SW_TMAIN400.CRC32checker.process_byte.0.15.V_0
//
//
//  E16 =.= Cu(1+@_UINT/CC/SCALbx14_byteno10)
//
//
//  E17 =.= Cu(CVT(C8u)(255&(Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2)))]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.15.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8)))>>>24))
//
//
//  E18 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2)))]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.15.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8))
//
//
//  E19 =.= SW_TMAIN400.CRC32checker.process_byte.0.23.V_0
//
//
//  E20 =.= CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>8)
//
//
//  E21 =.= Cu(CVT(C8u)(255&(Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>8)]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.23.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8)))>>>24))
//
//
//  E22 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>8)]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.23.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8))
//
//
//  E23 =.= SW_TMAIN400.CRC32checker.process_byte.0.31.V_0
//
//
//  E24 =.= CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>16)
//
//
//  E25 =.= Cu(CVT(C8u)(255&(Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>16)]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.31.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8)))>>>24))
//
//
//  E26 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2))>>>16)]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.31.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8))
//
//
//  E27 =.= CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_0)))
//
//
//  E28 =.= C64(@64_SS/CC/SCALbx52_ARA0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_0)))])
//
//
//  E29 =.= Cu(CVT(C8u)(255&@_UINT/CC/SCALbx14_crcreg10>>>24))
//
//
//  E30 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1)
//
//
//  E31 =.= CVT(C64)(@_UINT/CC/SCALbx14_crcreg10)
//
//
//  E32 =.= SW_TMAIN400.CRC32checker.process_byte.0.39.V_0
//
//
//  E33 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1)
//
//
//  E34 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[CVT(C8u)(255&(CVT(Cu)(SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_2)))]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.39.V_0]^@_UINT/CC/SCALbx14_crcreg10<<8))
//
//
//  E35 =.= CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))
//
//
//  E36 =.= SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3
//
//
//  E37 =.= C(@_SINT/CC/SCALbx24_ARB0[SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3])
//
//
//  E38 =.= CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1)))
//
//
//  E39 =.= COND((C64(-2+@64_SS/CC/SCALbx56_ARC0[CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]))<@64_SS/CC/SCALbx54_ARB0[CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))], C64(@64_SS/CC/SCALbx54_ARB0[CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]), C64(-2+@64_SS/CC/SCALbx56_ARC0[CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]))
//
//
//  E40 =.= CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1)))
//
//
//  E41 =.= C64(-2+@64_SS/CC/SCALbx58_ARD0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1)))])
//
//
//  E42 =.= C64(@64_SS/CC/SCALbx54_ARB0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1)))])
//
//
//  E43 =.= CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))
//
//
//  E44 =.= COND(@64_SS/CC/SCALbx58_ARD0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]<@64_SS/CC/SCALbx56_ARC0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))], C64(@64_SS/CC/SCALbx56_ARC0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]), C64(@64_SS/CC/SCALbx58_ARD0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))]))
//
//
//  E45 =.= C64(@64_SS/CC/SCALbx52_ARA0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_0)))])
//
//
//  E46 =.= CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_2))+S64'20*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_4)))
//
//
//  E47 =.= COND((C64(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_7+(CVT(C64)(@_SINT/CC/SCALbx22_ARA0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_2))+S64'20*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_4)))]))))<S64'0, S64'0, C64(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_7+(CVT(C64)(@_SINT/CC/SCALbx22_ARA0[CVT(C)((CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_2))+S64'20*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_4)))]))))
//
//
//  E48 =.= Cu(8+256*SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1)
//
//
//  E49 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3)
//
//
//  E50 =.= C64(-10+@64_SS/CC/SCALbx52_ARA0[CVT(C)((CVT(C64u)(1+SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3))+S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_1)))])
//
//
//  E51 =.= C(SW_TMAIN400.SmithWaterman.Program_t.encode.0.17._SPILL.256)
//
//
//  E52 =.= SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1
//
//
//  E53 =.= C(SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0)
//
//
//  E54 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0)
//
//
//  E55 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1)
//
//
//  E56 =.= CVT(C)(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1))
//
//
//  E57 =.= C(1+SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0)
//
//
//  E58 =.= CVT(C64)(SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0)
//
//
//  E59 =.= CVT(C)(S64'78*(CVT(C64u)(SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0)))
//
//
//  E60 =.= Cu(-1&(S'1099511627520^@_UINT/CC/SCALbx10_ARA0[1]^@_UINT/CC/SCALbx10_ARA0[255]))
//
//
//  E61 =.= Cu(CVT(C8u)(255&@_UINT/CC/SCALbx12_crcreg10>>>24))
//
//
//  E62 =.= SW_TMAIN400.CRC32checker.process_byte.0.40.V_0
//
//
//  E63 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[128]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.40.V_0]^@_UINT/CC/SCALbx12_crcreg10<<8))
//
//
//  E64 =.= SW_TMAIN400.CRC32checker.process_byte.0.44.V_0
//
//
//  E65 =.= Cu(-1&(@_UINT/CC/SCALbx10_ARA0[1]^@_UINT/CC/SCALbx10_ARA0[SW_TMAIN400.CRC32checker.process_byte.0.44.V_0]^@_UINT/CC/SCALbx12_crcreg10<<8))
//
//
//  E66 =.= @16_SS/CC/SCALbx18_ARB0[SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0]==(COND(XU32'2:"2:kiwiSWTMAIN400PC10nz"==kiwiSWTMAIN400PC10nz, Z16SSCCSCALbx20ARC0_10_rdata, Z16SSCCSCALbx20ARC010rdatah10hold))
//
//
//  E67 =.= @16_SS/CC/SCALbx18_ARB0[SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0]!=(COND(XU32'2:"2:kiwiSWTMAIN400PC10nz"==kiwiSWTMAIN400PC10nz, Z16SSCCSCALbx20ARC0_10_rdata, Z16SSCCSCALbx20ARC010rdatah10hold))
//
//
//  E68 =.= SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0<20
//
//
//  E69 =.= SW_TMAIN400.SmithWaterman.Program_t.encode.1.8.V_0>=20
//
//
//  E70 =.= (C(1+SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0))>=S32'77
//
//
//  E71 =.= (C(1+SW_TMAIN400.SmithWaterman.Program_t.install_query.0.6.V_0))<S32'77
//
//
//  E72 =.= SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0>=1
//
//
//  E73 =.= SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_0<1
//
//
//  E74 =.= SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0>=2
//
//
//  E75 =.= SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_0<2
//
//
//  E76 =.= SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2<S32'77
//
//
//  E77 =.= SW_TMAIN400.SmithWaterman.Program_t.testReport.2.13.V_2>=S32'77
//
//
//  E78 =.= SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1>=3
//
//
//  E79 =.= SW_TMAIN400.SmithWaterman.Program_t.runTest.0.12.V_1<3
//
//
//  E80 =.= SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1<S32'77
//
//
//  E81 =.= SW_TMAIN400.SmithWaterman.Program_t.crcReport.2.16.V_1>=S32'77
//
//
//  E82 =.= SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3<S32'77
//
//
//  E83 =.= SW_TMAIN400.SmithWaterman.Program_t.next_data.2.10.V_3>=S32'77
//
//
//  E84 =.= SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0<20
//
//
//  E85 =.= SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0>=20
//
//
//  E86 =.= @16_SS/CC/SCALbx18_ARB0[SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0]==(COND(XU32'45:"45:kiwiSWTMAIN400PC10nz"==kiwiSWTMAIN400PC10nz, Z16SSCCSCALbx16ARA0_10_rdata, Z16SSCCSCALbx16ARA010rdatah10hold))
//
//
//  E87 =.= @16_SS/CC/SCALbx18_ARB0[SW_TMAIN400.SmithWaterman.Program_t.encode.0.17.V_0]!=(COND(XU32'45:"45:kiwiSWTMAIN400PC10nz"==kiwiSWTMAIN400PC10nz, Z16SSCCSCALbx16ARA0_10_rdata, Z16SSCCSCALbx16ARA010rdatah10hold))
//
//
//  E88 =.= SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1<78
//
//
//  E89 =.= SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_1>=78
//
//
//  E90 =.= SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0<2
//
//
//  E91 =.= SW_TMAIN400.SmithWaterman.Program_t.sw_reset.1.2.V_0>=2
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for sw_test to sw_test

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for sw_testAS_SSROM_@16_SS/CC/SCALbx20_ARC0 to sw_testAS_SSROM_@16_SS_CC_SCALbx20_ARC0

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for sw_testAS_SSROM_@16_SS/CC/SCALbx16_ARA0 to sw_testAS_SSROM_@16_SS_CC_SCALbx16_ARA0

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for sw_testAS_SSROM_@_UINT/CC/SCALbx10_ARA0 to sw_testAS_SSROM_@_UINT_CC_SCALbx10_ARA0

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for sw_testAS_SSROM_@_SINT/CC/SCALbx22_ARA0 to sw_testAS_SSROM_@_SINT_CC_SCALbx22_ARA0

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//4 vectors of width 7
//
//31 vectors of width 1
//
//2 vectors of width 16
//
//10 vectors of width 64
//
//37 vectors of width 32
//
//22 vectors of width 8
//
//1 vectors of width 9
//
//20 array locations of width 16
//
//Total state bits in module = 2420 bits.
//
//960 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for AS_SSROM_A_16_SS_CC_SCALbx20_ARC0:
//77 array locations of width 16
//
//Total state bits in module = 1232 bits.
//
//Total number of leaf cells = 0
//

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for AS_SSROM_A_16_SS_CC_SCALbx16_ARA0:
//75 array locations of width 16
//
//Total state bits in module = 1200 bits.
//
//Total number of leaf cells = 0
//

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for AS_SSROM_A_UINT_CC_SCALbx10_ARA0:
//256 array locations of width 32
//
//Total state bits in module = 8192 bits.
//
//Total number of leaf cells = 0
//

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for AS_SSROM_A_SINT_CC_SCALbx22_ARA0:
//400 array locations of width 32
//
//Total state bits in module = 12800 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread SmithWaterman.Program_t..cctor uid=cctor10 has 37 CIL instructions in 1 basic blocks
//Thread CRC32checker..cctor uid=cctor12 has 7 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread sw_test_single_threaded.Main uid=Main10 has 306 CIL instructions in 69 basic blocks
//Thread mpc10 has 41 bevelab control states (pauses)
//Reindexed thread kiwiSWTMAIN400PC10 with 68 minor control states
// eof (HPR L/S Verilog)
