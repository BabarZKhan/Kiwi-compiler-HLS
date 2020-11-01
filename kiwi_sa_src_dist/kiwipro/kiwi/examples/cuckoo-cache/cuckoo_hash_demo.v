

// CBG Orangepath HPR L/S System

// Verilog output file generated at 10/01/2019 10:13:48
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.j : 5th January 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl=cuckoo_hash_demo.v cuckoo_hash_demo.exe -progress-monitor -vnl-resets=synchronous -kiwic-cil-dump=combined -obj-dir-name=. -log-dir-name=/tmp/kiwilogs/cuckoo_hash_demo -bondout-loadstore-port-count=0 -vnl-roundtrip=disable -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=10 -vnl-rootmodname=DUT
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=res2-directornets */
    output reg [6:0] kiwiTMAINIDL400PC10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);
function [7:0] hpr_toChar0;
input [31:0] hpr_toChar0_a;
   hpr_toChar0 = hpr_toChar0_a & 8'hff;
endfunction

// abstractionName=waypoints pi_name=kppIos10
  reg [639:0] KppWaypoint0;
  reg [639:0] KppWaypoint1;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_hash_3_10_V_0;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_hash_6_10_V_0;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_7;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_6;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_4;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_1;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1;
  reg signed [31:0] TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0;
  reg [63:0] TMAIN_IDL400_Testbench_Main_V_13;
  reg [63:0] TMAIN_IDL400_Testbench_Main_V_12;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_11;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_10;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_9;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_8;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_4;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_3;
  reg signed [31:0] TMAIN_IDL400_Testbench_Main_V_2;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_SCALbx24_statslookupprobes10;
  reg signed [31:0] A_SINT_CC_SCALbx24_statslookups10;
  reg signed [31:0] A_SINT_CC_SCALbx24_nextvictim10;
  reg signed [31:0] A_SINT_CC_SCALbx24_statsinsertevictions10;
  reg signed [31:0] A_SINT_CC_SCALbx24_statsinsertprobes10;
  reg signed [31:0] A_SINT_CC_SCALbx24_statsinserts10;
  reg signed [31:0] A_SINT_CC_SCALbx24_nextfree10;
  reg signed [31:0] A_SINT_CC_SCALbx28_seed10;
  reg signed [63:0] A_sA_SINT_CC_SCALbx20_ARA0[3:0];
  reg signed [63:0] A_sA_SINT_CC_SCALbx22_ARB0[3:0];
  reg [63:0] A_64_US_CC_SCALbx28_dk10;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire [63:0] A_64_US_CC_SCALbx26_ARA0_rdata;
  reg [14:0] A_64_US_CC_SCALbx26_ARA0_addr;
  reg A_64_US_CC_SCALbx26_ARA0_wen;
  reg A_64_US_CC_SCALbx26_ARA0_ren;
  reg [63:0] A_64_US_CC_SCALbx26_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR12NoCE0_ARA0_rdata;
  reg [12:0] A_SINT_CC_MAPR12NoCE0_ARA0_addr;
  reg A_SINT_CC_MAPR12NoCE0_ARA0_wen;
  reg A_SINT_CC_MAPR12NoCE0_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR12NoCE0_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR12NoCE1_ARA0_rdata;
  reg [12:0] A_SINT_CC_MAPR12NoCE1_ARA0_addr;
  reg A_SINT_CC_MAPR12NoCE1_ARA0_wen;
  reg A_SINT_CC_MAPR12NoCE1_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR12NoCE1_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR12NoCE2_ARA0_rdata;
  reg [12:0] A_SINT_CC_MAPR12NoCE2_ARA0_addr;
  reg A_SINT_CC_MAPR12NoCE2_ARA0_wen;
  reg A_SINT_CC_MAPR12NoCE2_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR12NoCE2_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR12NoCE3_ARA0_rdata;
  reg [12:0] A_SINT_CC_MAPR12NoCE3_ARA0_addr;
  reg A_SINT_CC_MAPR12NoCE3_ARA0_wen;
  reg A_SINT_CC_MAPR12NoCE3_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR12NoCE3_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR10NoCE0_ARB0_rdata;
  reg [12:0] A_SINT_CC_MAPR10NoCE0_ARB0_addr;
  reg A_SINT_CC_MAPR10NoCE0_ARB0_wen;
  reg A_SINT_CC_MAPR10NoCE0_ARB0_ren;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE0_ARB0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR10NoCE1_ARB0_rdata;
  reg [12:0] A_SINT_CC_MAPR10NoCE1_ARB0_addr;
  reg A_SINT_CC_MAPR10NoCE1_ARB0_wen;
  reg A_SINT_CC_MAPR10NoCE1_ARB0_ren;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE1_ARB0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR10NoCE2_ARB0_rdata;
  reg [12:0] A_SINT_CC_MAPR10NoCE2_ARB0_addr;
  reg A_SINT_CC_MAPR10NoCE2_ARB0_wen;
  reg A_SINT_CC_MAPR10NoCE2_ARB0_ren;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE2_ARB0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR10NoCE3_ARB0_rdata;
  reg [12:0] A_SINT_CC_MAPR10NoCE3_ARB0_addr;
  reg A_SINT_CC_MAPR10NoCE3_ARB0_wen;
  reg A_SINT_CC_MAPR10NoCE3_ARB0_ren;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE3_ARB0_wdata;
// abstractionName=res2-morenets
  reg [63:0] Z64USCCSCALbx26ARA0rdatah10hold;
  reg Z64USCCSCALbx26ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR12NoCE3ARA0rdatah10hold;
  reg SINTCCMAPR12NoCE3ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR12NoCE2ARA0rdatah10hold;
  reg SINTCCMAPR12NoCE2ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR12NoCE1ARA0rdatah10hold;
  reg SINTCCMAPR12NoCE1ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR12NoCE0ARA0rdatah10hold;
  reg SINTCCMAPR12NoCE0ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR10NoCE3ARB0rdatah10hold;
  reg SINTCCMAPR10NoCE3ARB0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR10NoCE2ARB0rdatah10hold;
  reg SINTCCMAPR10NoCE2ARB0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR10NoCE1ARB0rdatah10hold;
  reg SINTCCMAPR10NoCE1ARB0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR10NoCE0ARB0rdatah10hold;
  reg SINTCCMAPR10NoCE0ARB0rdatah10shot0;
  reg [6:0] kiwiTMAINIDL400PC10nz;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin501640x10;
  wire signed [31:0] hprpin501729x10;
  wire signed [31:0] hprpin501731x10;
  wire signed [31:0] hprpin501745x10;
  wire signed [31:0] hprpin501747x10;
  wire signed [31:0] hprpin501762x10;
  wire signed [31:0] hprpin501768x10;
  wire signed [31:0] hprpin501770x10;
  wire signed [31:0] hprpin501781x10;
  wire signed [31:0] hprpin501783x10;
 always   @(* )  begin 
       A_64_US_CC_SCALbx26_ARA0_addr = 32'd0;
       A_64_US_CC_SCALbx26_ARA0_wdata = 64'd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_addr = 32'd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_addr = 32'd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_addr = 32'd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_addr = 32'd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_addr = 32'd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_addr = 32'd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_addr = 32'd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_addr = 32'd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_wen = 32'sd0;
       A_64_US_CC_SCALbx26_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_wen = 32'sd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_wen = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_wen = 32'sd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_ren = 32'sd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_ren = 32'sd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_ren = 32'sd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_ren = 32'sd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_ren = 32'sd0;
       A_64_US_CC_SCALbx26_ARA0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE2_ARB0_addr = 32'sd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE1_ARB0_addr = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARB0_addr = 32'sd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE3_ARB0_addr = 32'sd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR12NoCE3_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE2_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE1_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR12NoCE0_ARA0_wdata = 32'sd0;
       A_64_US_CC_SCALbx26_ARA0_wdata = 32'sd0;
       A_64_US_CC_SCALbx26_ARA0_addr = 32'sd0;
       hpr_int_run_enable_DDX14 = 32'sd1;
      if (hpr_int_run_enable_DDX14)  begin 
               A_SINT_CC_MAPR10NoCE3_ARB0_wen = (32'h3/*3:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h17
              /*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1<32'sd4) && (32'h3/*3:USA38*/==
              A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1]) && (32'h9/*9:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              ) || ((32'he/*14:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) && !A_SINT_CC_MAPR10NoCE3_ARB0_rdata || (32'sd4!=TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)) && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]);

               A_64_US_CC_SCALbx26_ARA0_wen = (TMAIN_IDL400_Testbench_Main_V_4<32'sh5555) && (32'h8000/*32768:USA10*/!=A_SINT_CC_SCALbx24_nextfree10
              ) && !(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10)) && (32'h2a/*42:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              );

               A_SINT_CC_MAPR10NoCE2_ARB0_wen = (32'h2/*2:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h2d
              /*45:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h2/*2:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1
              ]) && (32'h3/*3:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || ((32'h10/*16:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              ) || (32'h1a/*26:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)) && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]);

               A_SINT_CC_MAPR10NoCE1_ARB0_wen = (32'h1/*1:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h2f
              /*47:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h1/*1:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1
              ]) && (32'h5/*5:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || ((32'h12/*18:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              ) || (32'h1c/*28:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)) && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]);

               A_SINT_CC_MAPR10NoCE0_ARB0_wen = (32'h0/*0:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h31
              /*49:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h0/*0:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1
              ]) && (32'h7/*7:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || ((32'h14/*20:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              ) || (32'h1e/*30:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)) && (32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]);

               A_SINT_CC_MAPR12NoCE3_ARA0_wen = (32'h3/*3:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]) && (32'h20/*32:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h3/*3:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
              ]) && (32'h33/*51:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE2_ARA0_wen = (32'h2/*2:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]) && (32'h22/*34:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h2/*2:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
              ]) && (32'h35/*53:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE1_ARA0_wen = (32'h1/*1:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]) && (32'h24/*36:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h1/*1:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
              ]) && (32'h37/*55:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE0_ARA0_wen = (32'h0/*0:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
              ]) && (32'h26/*38:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h0/*0:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
              ]) && (32'h39/*57:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR10NoCE0_ARB0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR10NoCE1_ARB0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR10NoCE2_ARB0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR10NoCE3_ARB0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE0_ARA0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE1_ARA0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE2_ARA0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_SINT_CC_MAPR12NoCE3_ARA0_ren = (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) || (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

               A_64_US_CC_SCALbx26_ARA0_ren = ((32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? 32'd1: 32'd0);
              if ((32'h2/*2:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1]) && (32'h3/*3:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = 32'sh0;
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0;
                       end 
                      if ((32'h1/*1:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1]) && (32'h5/*5:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = 32'sh0;
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0;
                       end 
                      if ((32'h0/*0:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1]) && (32'h7/*7:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = 32'sh0;
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0;
                       end 
                      if ((TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1<32'sd4) && (32'h3/*3:USA38*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1
              ]) && (32'h9/*9:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE3_ARB0_wdata = 32'sh0;
                       A_SINT_CC_MAPR10NoCE3_ARB0_addr = TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0;
                       end 
                      if ((32'hd/*13:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE3_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));
                       end 
                      if ((32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'he/*14:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz) && !A_SINT_CC_MAPR10NoCE3_ARB0_rdata)  begin 
                       A_SINT_CC_MAPR10NoCE3_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE3_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));
                       end 
                      if ((32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h10/*16:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h12/*18:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h14/*20:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz))  begin 
                      if ((32'sd4!=TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                      ]))  begin 
                               A_SINT_CC_MAPR10NoCE3_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                               A_SINT_CC_MAPR10NoCE3_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               end 
                              if ((32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4))  begin 
                               A_SINT_CC_MAPR12NoCE0_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR12NoCE1_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR12NoCE2_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR12NoCE3_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR10NoCE0_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR10NoCE1_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR10NoCE2_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               A_SINT_CC_MAPR10NoCE3_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                               end 
                               end 
                      if ((32'h3/*3:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h17/*23:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE3_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE3_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h1a/*26:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h1c/*28:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h1e/*30:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h3/*3:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h20/*32:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE3_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE3_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h2/*2:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h22/*34:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE2_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE2_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h1/*1:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h24/*36:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE1_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE1_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h0/*0:USA36*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h26/*38:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE0_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE0_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((TMAIN_IDL400_Testbench_Main_V_4<32'sh5555) && (32'h8000/*32768:USA10*/!=A_SINT_CC_SCALbx24_nextfree10) && 
              !(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10)) && (32'h2a/*42:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
              ))  begin 
                       A_64_US_CC_SCALbx26_ARA0_wdata = A_64_US_CC_SCALbx28_dk10;
                       A_64_US_CC_SCALbx26_ARA0_addr = A_SINT_CC_SCALbx24_nextfree10;
                       end 
                      if ((32'h2/*2:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h2d/*45:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE2_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h1/*1:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h2f/*47:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE1_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h0/*0:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h31/*49:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_0;
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h3/*3:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h33/*51:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE3_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE3_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h2/*2:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h35/*53:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE2_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE2_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h1/*1:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h37/*55:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE1_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE1_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      if ((32'h0/*0:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10]) && (32'h39/*57:kiwiTMAINIDL400PC10nz*/==
              kiwiTMAINIDL400PC10nz))  begin 
                       A_SINT_CC_MAPR12NoCE0_ARA0_wdata = TMAIN_IDL400_CuckooHasher_insert_1_9_V_2;
                       A_SINT_CC_MAPR12NoCE0_ARA0_addr = TMAIN_IDL400_CuckooHasher_insert_1_9_V_5;
                       end 
                      
              case (kiwiTMAINIDL400PC10nz)
                  32'h3e/*62:kiwiTMAINIDL400PC10nz*/:  begin 
                       A_SINT_CC_MAPR10NoCE0_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_3_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE1_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_3_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE2_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_3_10_V_0%32'sh2000));
                       A_SINT_CC_MAPR10NoCE3_ARB0_addr = $signed((TMAIN_IDL400_CuckooHasher_hash_3_10_V_0%32'sh2000));
                       end 
                      
                  32'h40/*64:kiwiTMAINIDL400PC10nz*/:  begin 
                       A_SINT_CC_MAPR12NoCE0_ARA0_addr = TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1;
                       A_SINT_CC_MAPR12NoCE1_ARA0_addr = TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1;
                       A_SINT_CC_MAPR12NoCE2_ARA0_addr = TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1;
                       A_SINT_CC_MAPR12NoCE3_ARA0_addr = TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1;
                       end 
                      
                  32'h41/*65:kiwiTMAINIDL400PC10nz*/:  A_64_US_CC_SCALbx26_ARA0_addr = hprpin501783x10;
              endcase
               end 
               hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogcuckoo_hash_demo/1.0
      if (reset)  begin 
               kiwiTMAINIDL400PC10nz_pc_export <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_10 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_8 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_9 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_12 <= 64'd0;
               TMAIN_IDL400_Testbench_Main_V_13 <= 64'd0;
               TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_hash_3_10_V_0 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_11 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_4 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_2 <= 32'd0;
               TMAIN_IDL400_Testbench_Main_V_3 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_7 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_6 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_1 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_5 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_hash_6_10_V_0 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_4 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_2 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_insert_1_9_V_0 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1 <= 32'd0;
               TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0 <= 32'd0;
               SINTCCMAPR10NoCE0ARB0rdatah10hold <= 32'd0;
               SINTCCMAPR10NoCE1ARB0rdatah10hold <= 32'd0;
               SINTCCMAPR10NoCE2ARB0rdatah10hold <= 32'd0;
               SINTCCMAPR10NoCE3ARB0rdatah10hold <= 32'd0;
               SINTCCMAPR12NoCE0ARA0rdatah10hold <= 32'd0;
               SINTCCMAPR12NoCE1ARA0rdatah10hold <= 32'd0;
               SINTCCMAPR12NoCE2ARA0rdatah10hold <= 32'd0;
               SINTCCMAPR12NoCE3ARA0rdatah10hold <= 32'd0;
               Z64USCCSCALbx26ARA0rdatah10hold <= 64'd0;
               Z64USCCSCALbx26ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR12NoCE3ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR12NoCE2ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR12NoCE1ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR12NoCE0ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR10NoCE3ARB0rdatah10shot0 <= 32'd0;
               SINTCCMAPR10NoCE2ARB0rdatah10shot0 <= 32'd0;
               SINTCCMAPR10NoCE1ARB0rdatah10shot0 <= 32'd0;
               SINTCCMAPR10NoCE0ARB0rdatah10shot0 <= 32'd0;
               hpr_abend_syndrome <= 32'd255;
               KppWaypoint0 <= 640'd0;
               KppWaypoint1 <= 640'd0;
               kiwiTMAINIDL400PC10nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14)  begin 
                  if ((TMAIN_IDL400_Testbench_Main_V_10>=32'sh5555) && (32'h44/*68:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz))  begin 
                          $display("Cuckoo cache retrieved items %1d/%1d", TMAIN_IDL400_Testbench_Main_V_9, TMAIN_IDL400_Testbench_Main_V_8
                          );
                          $display("cuckoo cache: this=%1d, inserts=%1d, lookups=%1d", hpr_toChar0(32'sd0), A_SINT_CC_SCALbx24_statsinserts10
                          , A_SINT_CC_SCALbx24_statslookups10);
                          $display("cuckoo cache: insert_probes=%1d, insert_evictions=%1d", A_SINT_CC_SCALbx24_statsinsertprobes10, A_SINT_CC_SCALbx24_statsinsertevictions10
                          );
                          $display("cuckoo cache: lookup_probes=%1d", A_SINT_CC_SCALbx24_statslookupprobes10);
                          $display("Cuckoo cache demo finished.");
                           end 
                          if ((TMAIN_IDL400_Testbench_Main_V_4>=32'sh5555) && (32'h2a/*42:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
                  )) $display("Cuckoo cache inserted items %1d/%1d", TMAIN_IDL400_Testbench_Main_V_3, TMAIN_IDL400_Testbench_Main_V_2
                      );
                      if ((32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
                  )) $display("Eviction %1d needed", TMAIN_IDL400_CuckooHasher_insert_1_9_V_1);
                      if ((TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0>=32'sh2000) && (32'h2/*2:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
                  )) $display("Cuckoo cache cleared");
                      
                  case (kiwiTMAINIDL400PC10nz)
                      32'h0/*0:kiwiTMAINIDL400PC10nz*/:  begin 
                          $display("Cuckoo cache testbench start. Capacity=%1d", 32'sh8000);
                           TMAIN_IDL400_Testbench_Main_V_10 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_8 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_9 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_12 <= 64'h0;
                           TMAIN_IDL400_Testbench_Main_V_13 <= 64'h0;
                           TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_hash_3_10_V_0 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_11 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_4 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_2 <= 32'sh0;
                           TMAIN_IDL400_Testbench_Main_V_3 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_7 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_6 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_1 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_5 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_hash_6_10_V_0 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_4 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_2 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_0 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0 <= 32'sh0;
                           A_SINT_CC_SCALbx24_nextfree10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_nextvictim10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_statsinserts10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_statsinsertprobes10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_statsinsertevictions10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_statslookups10 <= 32'sh0;
                           A_SINT_CC_SCALbx24_statslookupprobes10 <= 32'sh0;
                           A_SINT_CC_SCALbx28_seed10 <= 32'sh1_e240;
                           A_64_US_CC_SCALbx28_dk10 <= 64'h23_86f2_69cb_1f00;
                           KppWaypoint0 <= 32'sd0;
                           KppWaypoint1 <= "Start";
                           A_sA_SINT_CC_SCALbx20_ARA0[64'sd3] <= 64'sh3;
                           A_sA_SINT_CC_SCALbx20_ARA0[64'sd2] <= 64'sh2;
                           A_sA_SINT_CC_SCALbx20_ARA0[64'sd1] <= 64'sh1;
                           A_sA_SINT_CC_SCALbx20_ARA0[64'sd0] <= 64'sh0;
                           A_sA_SINT_CC_SCALbx22_ARB0[64'sd3] <= 64'sh3;
                           A_sA_SINT_CC_SCALbx22_ARB0[64'sd2] <= 64'sh2;
                           A_sA_SINT_CC_SCALbx22_ARB0[64'sd1] <= 64'sh1;
                           A_sA_SINT_CC_SCALbx22_ARB0[64'sd0] <= 64'sh0;
                           kiwiTMAINIDL400PC10nz <= 32'h1/*1:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h2/*2:kiwiTMAINIDL400PC10nz*/: if ((TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0<32'sh2000))  begin 
                               TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1 <= 32'sh0;
                               kiwiTMAINIDL400PC10nz <= 32'h9/*9:kiwiTMAINIDL400PC10nz*/;
                               end 
                               else  begin 
                               TMAIN_IDL400_Testbench_Main_V_4 <= 32'sh0;
                               TMAIN_IDL400_Testbench_Main_V_2 <= 32'sh0;
                               TMAIN_IDL400_Testbench_Main_V_3 <= 32'sh0;
                               A_SINT_CC_SCALbx28_seed10 <= 32'sh1_e240;
                               A_64_US_CC_SCALbx28_dk10 <= 64'h23_86f2_69cb_1f00;
                               KppWaypoint0 <= 32'sd1;
                               KppWaypoint1 <= "Cache Cleared";
                               kiwiTMAINIDL400PC10nz <= 32'h2a/*42:kiwiTMAINIDL400PC10nz*/;
                               end 
                              
                      32'h9/*9:kiwiTMAINIDL400PC10nz*/: if ((TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1<32'sd4))  kiwiTMAINIDL400PC10nz
                           <= 32'ha/*10:kiwiTMAINIDL400PC10nz*/;

                           else  begin 
                               TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0 <= $signed(32'sd1+TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0);
                               kiwiTMAINIDL400PC10nz <= 32'h2/*2:kiwiTMAINIDL400PC10nz*/;
                               end 
                              
                      32'hc/*12:kiwiTMAINIDL400PC10nz*/: if ((TMAIN_IDL400_CuckooHasher_insert_1_9_V_4<32'sd4))  begin 
                               TMAIN_IDL400_CuckooHasher_hash_6_10_V_0 <= hprpin501640x10;
                               A_SINT_CC_SCALbx24_statsinsertprobes10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statsinsertprobes10);
                               kiwiTMAINIDL400PC10nz <= 32'hd/*13:kiwiTMAINIDL400PC10nz*/;
                               end 
                               else  kiwiTMAINIDL400PC10nz <= 32'h16/*22:kiwiTMAINIDL400PC10nz*/;

                      32'he/*14:kiwiTMAINIDL400PC10nz*/:  begin 
                          if (((32'he/*14:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? !(!A_SINT_CC_MAPR10NoCE0_ARB0_rdata) && (32'h0
                          /*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!A_SINT_CC_MAPR10NoCE1_ARB0_rdata
                          ) && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!A_SINT_CC_MAPR10NoCE2_ARB0_rdata
                          ) && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!A_SINT_CC_MAPR10NoCE3_ARB0_rdata
                          ) && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]): !(!SINTCCMAPR10NoCE0ARB0rdatah10hold
                          ) && (32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!SINTCCMAPR10NoCE1ARB0rdatah10hold
                          ) && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!SINTCCMAPR10NoCE2ARB0rdatah10hold
                          ) && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !(!SINTCCMAPR10NoCE3ARB0rdatah10hold
                          ) && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4])))  begin 
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_5 <= $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000
                                  ));

                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_4 <= $signed(32'sd1+TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                                  );

                                   kiwiTMAINIDL400PC10nz <= 32'hc/*12:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                  if (((32'he/*14:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? !A_SINT_CC_MAPR10NoCE0_ARB0_rdata && 
                          (32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE1_ARB0_rdata
                           && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE2_ARB0_rdata
                           && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE3_ARB0_rdata
                           && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]): !SINTCCMAPR10NoCE0ARB0rdatah10hold
                           && (32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE1ARB0rdatah10hold
                           && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE2ARB0rdatah10hold
                           && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE3ARB0rdatah10hold
                           && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4])) || (32'h0/*0:USA30*/!=
                          A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h1/*1:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0
                          [TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h2/*2:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                          ]) && (32'h3/*3:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]))  TMAIN_IDL400_CuckooHasher_insert_1_9_V_5
                               <= $signed((TMAIN_IDL400_CuckooHasher_hash_6_10_V_0%32'sh2000));

                              if (((32'he/*14:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? !A_SINT_CC_MAPR10NoCE0_ARB0_rdata && (32'h0
                          /*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE1_ARB0_rdata
                           && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE2_ARB0_rdata
                           && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !A_SINT_CC_MAPR10NoCE3_ARB0_rdata
                           && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]): !SINTCCMAPR10NoCE0ARB0rdatah10hold
                           && (32'h0/*0:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE1ARB0rdatah10hold
                           && (32'h1/*1:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE2ARB0rdatah10hold
                           && (32'h2/*2:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) || !SINTCCMAPR10NoCE3ARB0rdatah10hold
                           && (32'h3/*3:USA30*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4])) || (32'h0/*0:USA30*/!=
                          A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h1/*1:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0
                          [TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]) && (32'h2/*2:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                          ]) && (32'h3/*3:USA30*/!=A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_insert_1_9_V_4]))  kiwiTMAINIDL400PC10nz
                               <= 32'hf/*15:kiwiTMAINIDL400PC10nz*/;

                               end 
                          
                      32'h17/*23:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_7 <= hprpin501747x10;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_6 <= hprpin501731x10;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_1 <= $signed(32'sd1+TMAIN_IDL400_CuckooHasher_insert_1_9_V_1);
                           A_SINT_CC_SCALbx24_statsinsertevictions10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statsinsertevictions10);
                           kiwiTMAINIDL400PC10nz <= 32'h18/*24:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h2a/*42:kiwiTMAINIDL400PC10nz*/:  begin 
                          if ((TMAIN_IDL400_Testbench_Main_V_4>=32'sh5555))  begin 
                                   TMAIN_IDL400_Testbench_Main_V_10 <= 32'sh0;
                                   TMAIN_IDL400_Testbench_Main_V_8 <= 32'sh0;
                                   TMAIN_IDL400_Testbench_Main_V_9 <= 32'sh0;
                                   A_SINT_CC_SCALbx28_seed10 <= 32'sh1_e240;
                                   A_64_US_CC_SCALbx28_dk10 <= 64'h23_86f2_69cb_1f00;
                                   KppWaypoint0 <= 32'sd2;
                                   KppWaypoint1 <= "Data Entered";
                                   kiwiTMAINIDL400PC10nz <= 32'h44/*68:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                  if ((TMAIN_IDL400_Testbench_Main_V_4<32'sh5555) && (32'h8000/*32768:USA10*/!=A_SINT_CC_SCALbx24_nextfree10
                          ) && !(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10)))  begin 
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_1 <= 32'sh0;
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_2 <= A_SINT_CC_SCALbx24_nextfree10;
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_0 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                                  );

                                   A_SINT_CC_SCALbx24_nextfree10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_nextfree10);
                                   A_SINT_CC_SCALbx24_statsinserts10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statsinserts10);
                                   A_SINT_CC_SCALbx28_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10);
                                   A_64_US_CC_SCALbx28_dk10 <= $unsigned(64'sh1+A_64_US_CC_SCALbx28_dk10);
                                   kiwiTMAINIDL400PC10nz <= 32'h2b/*43:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                  if ((!(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10))? (32'h8000/*32768:USA10*/==
                          A_SINT_CC_SCALbx24_nextfree10): 1'd1) && (TMAIN_IDL400_Testbench_Main_V_4<32'sh5555))  begin 
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256 <= ((TMAIN_IDL400_Testbench_Main_V_4<32'sh5555) && 
                                  (32'h8000/*32768:USA10*/==A_SINT_CC_SCALbx24_nextfree10) && !(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed
                                  *A_SINT_CC_SCALbx28_seed10))? -32'sh2: -32'sh4);

                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_1 <= 32'sh0;
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_2 <= 32'sh0;
                                   TMAIN_IDL400_CuckooHasher_insert_1_9_V_0 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                                  );

                                   A_SINT_CC_SCALbx28_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10);
                                   A_64_US_CC_SCALbx28_dk10 <= $unsigned(64'sh1+A_64_US_CC_SCALbx28_dk10);
                                   end 
                                  if ((!(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10))? (32'h8000/*32768:USA10*/==
                          A_SINT_CC_SCALbx24_nextfree10): 1'd1) && (TMAIN_IDL400_Testbench_Main_V_4<32'sh5555))  kiwiTMAINIDL400PC10nz
                               <= 32'h29/*41:kiwiTMAINIDL400PC10nz*/;

                               end 
                          
                      32'h2c/*44:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_Testbench_Main_V_4 <= $signed(32'sd1+TMAIN_IDL400_Testbench_Main_V_4);
                           TMAIN_IDL400_Testbench_Main_V_2 <= $signed(32'sd1+TMAIN_IDL400_Testbench_Main_V_2);
                           kiwiTMAINIDL400PC10nz <= 32'h2a/*42:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h3b/*59:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_5 <= 32'sh0;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_4 <= 32'sh0;
                           kiwiTMAINIDL400PC10nz <= 32'hc/*12:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h3c/*60:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_2 <= TMAIN_IDL400_CuckooHasher_insert_1_9_V_7;
                           TMAIN_IDL400_CuckooHasher_insert_1_9_V_0 <= TMAIN_IDL400_CuckooHasher_insert_1_9_V_6;
                           A_SINT_CC_SCALbx24_nextvictim10 <= $signed(($signed(32'sd1+A_SINT_CC_SCALbx24_nextvictim10)%32'sd4));
                           kiwiTMAINIDL400PC10nz <= 32'h3b/*59:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h3d/*61:kiwiTMAINIDL400PC10nz*/: if ((TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0<32'sd4))  begin 
                               TMAIN_IDL400_CuckooHasher_hash_3_10_V_0 <= hprpin501762x10;
                               A_SINT_CC_SCALbx24_statslookupprobes10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statslookupprobes10);
                               kiwiTMAINIDL400PC10nz <= 32'h3e/*62:kiwiTMAINIDL400PC10nz*/;
                               end 
                               else  kiwiTMAINIDL400PC10nz <= 32'h40/*64:kiwiTMAINIDL400PC10nz*/;

                      32'h3f/*63:kiwiTMAINIDL400PC10nz*/:  begin 
                          if ((TMAIN_IDL400_Testbench_Main_V_11==((32'h3/*3:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
                          ])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE3_ARB0_rdata: SINTCCMAPR10NoCE3ARB0rdatah10hold
                          ): ((32'h2/*2:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==
                          kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE2_ARB0_rdata: SINTCCMAPR10NoCE2ARB0rdatah10hold): ((32'h1/*1:USA26*/==
                          A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==
                          kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE1_ARB0_rdata: SINTCCMAPR10NoCE1ARB0rdatah10hold): ((32'h0/*0:USA26*/==
                          A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==
                          kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE0_ARB0_rdata: SINTCCMAPR10NoCE0ARB0rdatah10hold): 1'bx))))))  kiwiTMAINIDL400PC10nz
                               <= 32'h40/*64:kiwiTMAINIDL400PC10nz*/;

                               else  begin 
                                   TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0 <= $signed(32'sd1+TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
                                  );

                                   kiwiTMAINIDL400PC10nz <= 32'h3d/*61:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                   TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1 <= $signed((TMAIN_IDL400_CuckooHasher_hash_3_10_V_0%32'sh2000
                          ));

                           end 
                          
                      32'h42/*66:kiwiTMAINIDL400PC10nz*/:  begin 
                          if ((32'sd4!=TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0))  TMAIN_IDL400_Testbench_Main_V_13 <= ((32'h42/*66:kiwiTMAINIDL400PC10nz*/==
                              kiwiTMAINIDL400PC10nz)? A_64_US_CC_SCALbx26_ARA0_rdata: Z64USCCSCALbx26ARA0rdatah10hold);

                               TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256 <= ((32'sd4==TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0)? -32'sh5
                          : 32'sh0);

                           kiwiTMAINIDL400PC10nz <= 32'h43/*67:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h44/*68:kiwiTMAINIDL400PC10nz*/:  begin 
                          if ((TMAIN_IDL400_Testbench_Main_V_10<32'sh5555) && !(!$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                          )))  begin 
                                   TMAIN_IDL400_Testbench_Main_V_12 <= A_64_US_CC_SCALbx28_dk10;
                                   TMAIN_IDL400_Testbench_Main_V_13 <= 64'h0;
                                   TMAIN_IDL400_CuckooHasher_lookup_6_9_V_1 <= 32'sh0;
                                   TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0 <= 32'sh0;
                                   TMAIN_IDL400_Testbench_Main_V_11 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                                  );

                                   A_SINT_CC_SCALbx24_statslookups10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statslookups10);
                                   A_SINT_CC_SCALbx28_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10);
                                   A_64_US_CC_SCALbx28_dk10 <= $unsigned(64'sh1+A_64_US_CC_SCALbx28_dk10);
                                   kiwiTMAINIDL400PC10nz <= 32'h3d/*61:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                  if ((TMAIN_IDL400_Testbench_Main_V_10<32'sh5555) && !$signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                          ))  begin 
                                   TMAIN_IDL400_Testbench_Main_V_12 <= A_64_US_CC_SCALbx28_dk10;
                                   TMAIN_IDL400_Testbench_Main_V_13 <= 64'h0;
                                   TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256 <= -32'sh4;
                                   TMAIN_IDL400_Testbench_Main_V_11 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10
                                  );

                                   A_SINT_CC_SCALbx24_statslookups10 <= $signed(32'sd1+A_SINT_CC_SCALbx24_statslookups10);
                                   A_SINT_CC_SCALbx28_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx28_seed10);
                                   A_64_US_CC_SCALbx28_dk10 <= $unsigned(64'sh1+A_64_US_CC_SCALbx28_dk10);
                                   kiwiTMAINIDL400PC10nz <= 32'h43/*67:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                  if ((TMAIN_IDL400_Testbench_Main_V_10>=32'sh5555))  begin 
                                   KppWaypoint0 <= 32'sd3;
                                   KppWaypoint1 <= "Readback Done";
                                   kiwiTMAINIDL400PC10nz <= 32'h46/*70:kiwiTMAINIDL400PC10nz*/;
                                   end 
                                   end 
                          
                      32'h45/*69:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_Testbench_Main_V_10 <= $signed(32'sd1+TMAIN_IDL400_Testbench_Main_V_10);
                           TMAIN_IDL400_Testbench_Main_V_8 <= $signed(32'sd1+TMAIN_IDL400_Testbench_Main_V_8);
                           kiwiTMAINIDL400PC10nz <= 32'h44/*68:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h46/*70:kiwiTMAINIDL400PC10nz*/:  begin 
                           hpr_abend_syndrome <= 32'sd0;
                           hpr_abend_syndrome <= 32'sd0;
                           kiwiTMAINIDL400PC10nz <= 32'h47/*71:kiwiTMAINIDL400PC10nz*/;
                           end 
                          endcase
                  if ((32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4))  begin if ((32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
                      ))  kiwiTMAINIDL400PC10nz <= 32'h17/*23:kiwiTMAINIDL400PC10nz*/;
                           end 
                       else if ((32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz))  kiwiTMAINIDL400PC10nz <= 32'h19/*25:kiwiTMAINIDL400PC10nz*/;
                          
                  case (kiwiTMAINIDL400PC10nz)
                      32'h1/*1:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_Clear_0_16_V_0 <= 32'sh0;
                           kiwiTMAINIDL400PC10nz <= 32'h2/*2:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'hb/*11:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1 <= $signed(32'sd1+TMAIN_IDL400_CuckooHasher_Clear_0_16_V_1);
                           kiwiTMAINIDL400PC10nz <= 32'h9/*9:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h28/*40:kiwiTMAINIDL400PC10nz*/:  begin 
                           TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256 <= 32'sh0;
                           kiwiTMAINIDL400PC10nz <= 32'h29/*41:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h29/*41:kiwiTMAINIDL400PC10nz*/:  begin 
                          if (!(!(!TMAIN_IDL400_CuckooHasher_insert_1_9_SPILL_256)))  TMAIN_IDL400_Testbench_Main_V_3 <= $signed(32'sd1
                              +TMAIN_IDL400_Testbench_Main_V_3);

                               kiwiTMAINIDL400PC10nz <= 32'h2c/*44:kiwiTMAINIDL400PC10nz*/;
                           end 
                          
                      32'h43/*67:kiwiTMAINIDL400PC10nz*/:  begin 
                          if ((TMAIN_IDL400_Testbench_Main_V_12==TMAIN_IDL400_Testbench_Main_V_13) && !TMAIN_IDL400_CuckooHasher_lookup_6_9_SPILL_256
                          )  TMAIN_IDL400_Testbench_Main_V_9 <= $signed(32'sd1+TMAIN_IDL400_Testbench_Main_V_9);
                               kiwiTMAINIDL400PC10nz <= 32'h45/*69:kiwiTMAINIDL400PC10nz*/;
                           end 
                          endcase
                  if (Z64USCCSCALbx26ARA0rdatah10shot0)  Z64USCCSCALbx26ARA0rdatah10hold <= A_64_US_CC_SCALbx26_ARA0_rdata;
                      if (SINTCCMAPR12NoCE3ARA0rdatah10shot0)  SINTCCMAPR12NoCE3ARA0rdatah10hold <= A_SINT_CC_MAPR12NoCE3_ARA0_rdata;
                      if (SINTCCMAPR12NoCE2ARA0rdatah10shot0)  SINTCCMAPR12NoCE2ARA0rdatah10hold <= A_SINT_CC_MAPR12NoCE2_ARA0_rdata;
                      if (SINTCCMAPR12NoCE1ARA0rdatah10shot0)  SINTCCMAPR12NoCE1ARA0rdatah10hold <= A_SINT_CC_MAPR12NoCE1_ARA0_rdata;
                      if (SINTCCMAPR12NoCE0ARA0rdatah10shot0)  SINTCCMAPR12NoCE0ARA0rdatah10hold <= A_SINT_CC_MAPR12NoCE0_ARA0_rdata;
                      if (SINTCCMAPR10NoCE3ARB0rdatah10shot0)  SINTCCMAPR10NoCE3ARB0rdatah10hold <= A_SINT_CC_MAPR10NoCE3_ARB0_rdata;
                      if (SINTCCMAPR10NoCE2ARB0rdatah10shot0)  SINTCCMAPR10NoCE2ARB0rdatah10hold <= A_SINT_CC_MAPR10NoCE2_ARB0_rdata;
                      if (SINTCCMAPR10NoCE1ARB0rdatah10shot0)  SINTCCMAPR10NoCE1ARB0rdatah10hold <= A_SINT_CC_MAPR10NoCE1_ARB0_rdata;
                      if (SINTCCMAPR10NoCE0ARB0rdatah10shot0)  SINTCCMAPR10NoCE0ARB0rdatah10hold <= A_SINT_CC_MAPR10NoCE0_ARB0_rdata;
                      
                  case (kiwiTMAINIDL400PC10nz)
                      32'h3/*3:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h4/*4:kiwiTMAINIDL400PC10nz*/;

                      32'h4/*4:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h5/*5:kiwiTMAINIDL400PC10nz*/;

                      32'h5/*5:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h6/*6:kiwiTMAINIDL400PC10nz*/;

                      32'h6/*6:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h7/*7:kiwiTMAINIDL400PC10nz*/;

                      32'h7/*7:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h8/*8:kiwiTMAINIDL400PC10nz*/;

                      32'h8/*8:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'hb/*11:kiwiTMAINIDL400PC10nz*/;

                      32'ha/*10:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h3/*3:kiwiTMAINIDL400PC10nz*/;

                      32'hd/*13:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'he/*14:kiwiTMAINIDL400PC10nz*/;

                      32'hf/*15:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h10/*16:kiwiTMAINIDL400PC10nz*/;

                      32'h10/*16:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h11/*17:kiwiTMAINIDL400PC10nz*/;

                      32'h11/*17:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h12/*18:kiwiTMAINIDL400PC10nz*/;

                      32'h12/*18:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h13/*19:kiwiTMAINIDL400PC10nz*/;

                      32'h13/*19:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h14/*20:kiwiTMAINIDL400PC10nz*/;

                      32'h14/*20:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h15/*21:kiwiTMAINIDL400PC10nz*/;

                      32'h15/*21:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h16/*22:kiwiTMAINIDL400PC10nz*/;

                      32'h18/*24:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h2d/*45:kiwiTMAINIDL400PC10nz*/;

                      32'h19/*25:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1a/*26:kiwiTMAINIDL400PC10nz*/;

                      32'h1a/*26:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1b/*27:kiwiTMAINIDL400PC10nz*/;

                      32'h1b/*27:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1c/*28:kiwiTMAINIDL400PC10nz*/;

                      32'h1c/*28:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1d/*29:kiwiTMAINIDL400PC10nz*/;

                      32'h1d/*29:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1e/*30:kiwiTMAINIDL400PC10nz*/;

                      32'h1e/*30:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h1f/*31:kiwiTMAINIDL400PC10nz*/;

                      32'h1f/*31:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h20/*32:kiwiTMAINIDL400PC10nz*/;

                      32'h20/*32:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h21/*33:kiwiTMAINIDL400PC10nz*/;

                      32'h21/*33:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h22/*34:kiwiTMAINIDL400PC10nz*/;

                      32'h22/*34:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h23/*35:kiwiTMAINIDL400PC10nz*/;

                      32'h23/*35:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h24/*36:kiwiTMAINIDL400PC10nz*/;

                      32'h24/*36:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h25/*37:kiwiTMAINIDL400PC10nz*/;

                      32'h25/*37:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h26/*38:kiwiTMAINIDL400PC10nz*/;

                      32'h26/*38:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h27/*39:kiwiTMAINIDL400PC10nz*/;

                      32'h27/*39:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h28/*40:kiwiTMAINIDL400PC10nz*/;

                      32'h2b/*43:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h3b/*59:kiwiTMAINIDL400PC10nz*/;

                      32'h2d/*45:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h2e/*46:kiwiTMAINIDL400PC10nz*/;

                      32'h2e/*46:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h2f/*47:kiwiTMAINIDL400PC10nz*/;

                      32'h2f/*47:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h30/*48:kiwiTMAINIDL400PC10nz*/;

                      32'h30/*48:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h31/*49:kiwiTMAINIDL400PC10nz*/;

                      32'h31/*49:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h32/*50:kiwiTMAINIDL400PC10nz*/;

                      32'h32/*50:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h33/*51:kiwiTMAINIDL400PC10nz*/;

                      32'h33/*51:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h34/*52:kiwiTMAINIDL400PC10nz*/;

                      32'h34/*52:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h35/*53:kiwiTMAINIDL400PC10nz*/;

                      32'h35/*53:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h36/*54:kiwiTMAINIDL400PC10nz*/;

                      32'h36/*54:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h37/*55:kiwiTMAINIDL400PC10nz*/;

                      32'h37/*55:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h38/*56:kiwiTMAINIDL400PC10nz*/;

                      32'h38/*56:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h39/*57:kiwiTMAINIDL400PC10nz*/;

                      32'h39/*57:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h3a/*58:kiwiTMAINIDL400PC10nz*/;

                      32'h3a/*58:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h3c/*60:kiwiTMAINIDL400PC10nz*/;

                      32'h3e/*62:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h3f/*63:kiwiTMAINIDL400PC10nz*/;

                      32'h40/*64:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h41/*65:kiwiTMAINIDL400PC10nz*/;

                      32'h41/*65:kiwiTMAINIDL400PC10nz*/:  kiwiTMAINIDL400PC10nz <= 32'h42/*66:kiwiTMAINIDL400PC10nz*/;
                  endcase
                   kiwiTMAINIDL400PC10nz_pc_export <= kiwiTMAINIDL400PC10nz;
                   Z64USCCSCALbx26ARA0rdatah10shot0 <= (32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);
                   SINTCCMAPR12NoCE3ARA0rdatah10shot0 <= (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                  ) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR12NoCE2ARA0rdatah10shot0 <= (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                  ) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR12NoCE1ARA0rdatah10shot0 <= (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                  ) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR12NoCE0ARA0rdatah10shot0 <= (32'h40/*64:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
                  ) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR10NoCE3ARB0rdatah10shot0 <= (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR10NoCE2ARB0rdatah10shot0 <= (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR10NoCE1ARB0rdatah10shot0 <= (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz);

                   SINTCCMAPR10NoCE0ARB0rdatah10shot0 <= (32'h3e/*62:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz) || (32'hd/*13:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz) || (32'sd4==TMAIN_IDL400_CuckooHasher_insert_1_9_V_4) && (32'h16/*22:kiwiTMAINIDL400PC10nz*/==
                  kiwiTMAINIDL400PC10nz);

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogcuckoo_hash_demo/1.0


       end 
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd64),
        .ADDR_WIDTH(32'sd15),
        .WORDS(32'sd32768),
        .LANE_WIDTH(32'sd64
),
        .trace_me(32'sd0)) A_64_US_CC_SCALbx26_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_64_US_CC_SCALbx26_ARA0_rdata
),
        .addr(A_64_US_CC_SCALbx26_ARA0_addr),
        .wen(A_64_US_CC_SCALbx26_ARA0_wen),
        .ren(A_64_US_CC_SCALbx26_ARA0_ren
),
        .wdata(A_64_US_CC_SCALbx26_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR12NoCE0_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR12NoCE0_ARA0_rdata
),
        .addr(A_SINT_CC_MAPR12NoCE0_ARA0_addr),
        .wen(A_SINT_CC_MAPR12NoCE0_ARA0_wen),
        .ren(A_SINT_CC_MAPR12NoCE0_ARA0_ren
),
        .wdata(A_SINT_CC_MAPR12NoCE0_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR12NoCE1_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR12NoCE1_ARA0_rdata
),
        .addr(A_SINT_CC_MAPR12NoCE1_ARA0_addr),
        .wen(A_SINT_CC_MAPR12NoCE1_ARA0_wen),
        .ren(A_SINT_CC_MAPR12NoCE1_ARA0_ren
),
        .wdata(A_SINT_CC_MAPR12NoCE1_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR12NoCE2_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR12NoCE2_ARA0_rdata
),
        .addr(A_SINT_CC_MAPR12NoCE2_ARA0_addr),
        .wen(A_SINT_CC_MAPR12NoCE2_ARA0_wen),
        .ren(A_SINT_CC_MAPR12NoCE2_ARA0_ren
),
        .wdata(A_SINT_CC_MAPR12NoCE2_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR12NoCE3_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR12NoCE3_ARA0_rdata
),
        .addr(A_SINT_CC_MAPR12NoCE3_ARA0_addr),
        .wen(A_SINT_CC_MAPR12NoCE3_ARA0_wen),
        .ren(A_SINT_CC_MAPR12NoCE3_ARA0_ren
),
        .wdata(A_SINT_CC_MAPR12NoCE3_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR10NoCE0_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR10NoCE0_ARB0_rdata
),
        .addr(A_SINT_CC_MAPR10NoCE0_ARB0_addr),
        .wen(A_SINT_CC_MAPR10NoCE0_ARB0_wen),
        .ren(A_SINT_CC_MAPR10NoCE0_ARB0_ren
),
        .wdata(A_SINT_CC_MAPR10NoCE0_ARB0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR10NoCE1_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR10NoCE1_ARB0_rdata
),
        .addr(A_SINT_CC_MAPR10NoCE1_ARB0_addr),
        .wen(A_SINT_CC_MAPR10NoCE1_ARB0_wen),
        .ren(A_SINT_CC_MAPR10NoCE1_ARB0_ren
),
        .wdata(A_SINT_CC_MAPR10NoCE1_ARB0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR10NoCE2_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR10NoCE2_ARB0_rdata
),
        .addr(A_SINT_CC_MAPR10NoCE2_ARB0_addr),
        .wen(A_SINT_CC_MAPR10NoCE2_ARB0_wen),
        .ren(A_SINT_CC_MAPR10NoCE2_ARB0_ren
),
        .wdata(A_SINT_CC_MAPR10NoCE2_ARB0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd13),
        .WORDS(32'sd8192),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR10NoCE3_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR10NoCE3_ARB0_rdata
),
        .addr(A_SINT_CC_MAPR10NoCE3_ARB0_addr),
        .wen(A_SINT_CC_MAPR10NoCE3_ARB0_wen),
        .ren(A_SINT_CC_MAPR10NoCE3_ARB0_ren
),
        .wdata(A_SINT_CC_MAPR10NoCE3_ARB0_wdata));

assign hprpin501640x10 = ((TMAIN_IDL400_CuckooHasher_insert_1_9_V_4<32'sd4) && ($signed(TMAIN_IDL400_CuckooHasher_insert_1_9_V_0+32'sd51*TMAIN_IDL400_CuckooHasher_insert_1_9_V_4
)<32'sd0)? $signed(-32'sd51*TMAIN_IDL400_CuckooHasher_insert_1_9_V_4+(0-TMAIN_IDL400_CuckooHasher_insert_1_9_V_0)): $signed(TMAIN_IDL400_CuckooHasher_insert_1_9_V_0
+32'sd51*TMAIN_IDL400_CuckooHasher_insert_1_9_V_4));

assign hprpin501729x10 = ((32'h1/*1:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR10NoCE1_ARB0_rdata: SINTCCMAPR10NoCE1ARB0rdatah10hold): ((32'h0/*0:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10
])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE0_ARB0_rdata: SINTCCMAPR10NoCE0ARB0rdatah10hold
): 32'hffffffff&32'bx));

assign hprpin501731x10 = ((32'h3/*3:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR10NoCE3_ARB0_rdata: SINTCCMAPR10NoCE3ARB0rdatah10hold): ((32'h2/*2:USA40*/==A_sA_SINT_CC_SCALbx22_ARB0[A_SINT_CC_SCALbx24_nextvictim10
])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE2_ARB0_rdata: SINTCCMAPR10NoCE2ARB0rdatah10hold
): hprpin501729x10));

assign hprpin501745x10 = ((32'h1/*1:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR12NoCE1_ARA0_rdata: SINTCCMAPR12NoCE1ARA0rdatah10hold): ((32'h0/*0:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR12NoCE0_ARA0_rdata: SINTCCMAPR12NoCE0ARA0rdatah10hold
): 32'hffffffff&32'bx));

assign hprpin501747x10 = ((32'h3/*3:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR12NoCE3_ARA0_rdata: SINTCCMAPR12NoCE3ARA0rdatah10hold): ((32'h2/*2:USA42*/==A_sA_SINT_CC_SCALbx20_ARA0[A_SINT_CC_SCALbx24_nextvictim10
])? ((32'h17/*23:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR12NoCE2_ARA0_rdata: SINTCCMAPR12NoCE2ARA0rdatah10hold
): hprpin501745x10));

assign hprpin501762x10 = ((TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0<32'sd4) && ($signed(TMAIN_IDL400_Testbench_Main_V_11+32'sd51*TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
)<32'sd0)? $signed(-32'sd51*TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0+(0-TMAIN_IDL400_Testbench_Main_V_11)): $signed(TMAIN_IDL400_Testbench_Main_V_11
+32'sd51*TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0));

assign hprpin501768x10 = ((32'h1/*1:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR10NoCE1_ARB0_rdata: SINTCCMAPR10NoCE1ARB0rdatah10hold): ((32'h0/*0:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE0_ARB0_rdata: SINTCCMAPR10NoCE0ARB0rdatah10hold
): 32'bx));

assign hprpin501770x10 = ((32'h3/*3:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR10NoCE3_ARB0_rdata: SINTCCMAPR10NoCE3ARB0rdatah10hold): ((32'h2/*2:USA26*/==A_sA_SINT_CC_SCALbx22_ARB0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
])? ((32'h3f/*63:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR10NoCE2_ARB0_rdata: SINTCCMAPR10NoCE2ARB0rdatah10hold
): hprpin501768x10));

assign hprpin501781x10 = ((32'h1/*1:USA28*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR12NoCE1_ARA0_rdata: SINTCCMAPR12NoCE1ARA0rdatah10hold): ((32'h0/*0:USA28*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
])? ((32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR12NoCE0_ARA0_rdata: SINTCCMAPR12NoCE0ARA0rdatah10hold
): 32'hffffffff&32'bx));

assign hprpin501783x10 = ((32'h3/*3:USA28*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0])? ((32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz
)? A_SINT_CC_MAPR12NoCE3_ARA0_rdata: SINTCCMAPR12NoCE3ARA0rdatah10hold): ((32'h2/*2:USA28*/==A_sA_SINT_CC_SCALbx20_ARA0[TMAIN_IDL400_CuckooHasher_lookup_6_9_V_0
])? ((32'h41/*65:kiwiTMAINIDL400PC10nz*/==kiwiTMAINIDL400PC10nz)? A_SINT_CC_MAPR12NoCE2_ARA0_rdata: SINTCCMAPR12NoCE2ARA0rdatah10hold
): hprpin501781x10));

// Structural Resource (FU) inventory for DUT:// 28 vectors of width 1
// 1 vectors of width 7
// 46 vectors of width 32
// 5 vectors of width 64
// 8 vectors of width 13
// 1 vectors of width 15
// 2 vectors of width 640
// 8 array locations of width 64
// Total state bits in module = 3738 bits.
// 640 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=9
// Total number of leaf cells = 9
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.j : 5th January 2019
//10/01/2019 10:13:44
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl=cuckoo_hash_demo.v cuckoo_hash_demo.exe -progress-monitor -vnl-resets=synchronous -kiwic-cil-dump=combined -obj-dir-name=. -log-dir-name=/tmp/kiwilogs/cuckoo_hash_demo -bondout-loadstore-port-count=0 -vnl-roundtrip=disable -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=10 -vnl-rootmodname=DUT


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-----------+---------+------------------------------------------------------------------------------------------------------------+---------------+----------------+--------+-------*
//| Class     | Style   | Dir Style                                                                                                  | Timing Target | Method         | UID    | Skip  |
//*-----------+---------+------------------------------------------------------------------------------------------------------------+---------------+----------------+--------+-------*
//| Testbench | MM_root | DS_normal self-start/directorate-startmode, auto-restart/directorate-endmode, enable/directorate-pc-export |               | Testbench.Main | Main10 | false |
//*-----------+---------+------------------------------------------------------------------------------------------------------------+---------------+----------------+--------+-------*

//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports = Nothing to Report
//

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
//KiwiC: front end input processing of class Testbench  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Testbench.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Testbench.Main
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
//   kiwic-cil-dump=combined
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
//   srcfile=cuckoo_hash_demo.exe
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
//PC codings points for kiwiTMAINIDL400PC10 
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTMAINIDL400PC10"   | 903 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTMAINIDL400PC10"   | 902 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTMAINIDL400PC10"   | 900 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 9    |
//| XU32'2:"2:kiwiTMAINIDL400PC10"   | 901 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 42   |
//| XU32'4:"4:kiwiTMAINIDL400PC10"   | 897 | 3       | hwm=0.0.1   | 3    |        | 4     | 4   | 5    |
//| XU32'5:"5:kiwiTMAINIDL400PC10"   | 896 | 5       | hwm=0.0.1   | 5    |        | 6     | 6   | 7    |
//| XU32'6:"6:kiwiTMAINIDL400PC10"   | 895 | 7       | hwm=0.0.1   | 7    |        | 8     | 8   | 11   |
//| XU32'3:"3:kiwiTMAINIDL400PC10"   | 898 | 9       | hwm=0.0.1   | 9    |        | 10    | 10  | 3    |
//| XU32'3:"3:kiwiTMAINIDL400PC10"   | 899 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 2    |
//| XU32'7:"7:kiwiTMAINIDL400PC10"   | 894 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 9    |
//| XU32'10:"10:kiwiTMAINIDL400PC10" | 888 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 13   |
//| XU32'10:"10:kiwiTMAINIDL400PC10" | 889 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 22   |
//| XU32'11:"11:kiwiTMAINIDL400PC10" | 886 | 13      | hwm=1.1.0   | 14   |        | -     | -   | 12   |
//| XU32'11:"11:kiwiTMAINIDL400PC10" | 887 | 13      | hwm=1.1.1   | 14   |        | 15    | 15  | 16   |
//| XU32'12:"12:kiwiTMAINIDL400PC10" | 885 | 16      | hwm=0.0.1   | 16   |        | 17    | 17  | 18   |
//| XU32'13:"13:kiwiTMAINIDL400PC10" | 884 | 18      | hwm=0.0.1   | 18   |        | 19    | 19  | 20   |
//| XU32'14:"14:kiwiTMAINIDL400PC10" | 883 | 20      | hwm=0.0.1   | 20   |        | 21    | 21  | 22   |
//| XU32'15:"15:kiwiTMAINIDL400PC10" | 881 | 22      | hwm=0.0.1   | 22   |        | 25    | 25  | 26   |
//| XU32'15:"15:kiwiTMAINIDL400PC10" | 882 | 22      | hwm=0.1.1   | 23   |        | 23    | 24  | 45   |
//| XU32'16:"16:kiwiTMAINIDL400PC10" | 880 | 26      | hwm=0.0.1   | 26   |        | 27    | 27  | 28   |
//| XU32'17:"17:kiwiTMAINIDL400PC10" | 879 | 28      | hwm=0.0.1   | 28   |        | 29    | 29  | 30   |
//| XU32'18:"18:kiwiTMAINIDL400PC10" | 878 | 30      | hwm=0.0.1   | 30   |        | 31    | 31  | 32   |
//| XU32'19:"19:kiwiTMAINIDL400PC10" | 877 | 32      | hwm=0.0.1   | 32   |        | 33    | 33  | 34   |
//| XU32'20:"20:kiwiTMAINIDL400PC10" | 876 | 34      | hwm=0.0.1   | 34   |        | 35    | 35  | 36   |
//| XU32'21:"21:kiwiTMAINIDL400PC10" | 875 | 36      | hwm=0.0.1   | 36   |        | 37    | 37  | 38   |
//| XU32'22:"22:kiwiTMAINIDL400PC10" | 874 | 38      | hwm=0.0.1   | 38   |        | 39    | 39  | 40   |
//| XU32'23:"23:kiwiTMAINIDL400PC10" | 873 | 40      | hwm=0.0.0   | 40   |        | -     | -   | 41   |
//| XU32'24:"24:kiwiTMAINIDL400PC10" | 872 | 41      | hwm=0.0.0   | 41   |        | -     | -   | 44   |
//| XU32'8:"8:kiwiTMAINIDL400PC10"   | 891 | 42      | hwm=0.0.1   | 42   |        | 43    | 43  | 59   |
//| XU32'8:"8:kiwiTMAINIDL400PC10"   | 892 | 42      | hwm=0.0.0   | 42   |        | -     | -   | 41   |
//| XU32'8:"8:kiwiTMAINIDL400PC10"   | 893 | 42      | hwm=0.0.0   | 42   |        | -     | -   | 68   |
//| XU32'25:"25:kiwiTMAINIDL400PC10" | 871 | 44      | hwm=0.0.0   | 44   |        | -     | -   | 42   |
//| XU32'26:"26:kiwiTMAINIDL400PC10" | 870 | 45      | hwm=0.0.1   | 45   |        | 46    | 46  | 47   |
//| XU32'27:"27:kiwiTMAINIDL400PC10" | 869 | 47      | hwm=0.0.1   | 47   |        | 48    | 48  | 49   |
//| XU32'28:"28:kiwiTMAINIDL400PC10" | 868 | 49      | hwm=0.0.1   | 49   |        | 50    | 50  | 51   |
//| XU32'29:"29:kiwiTMAINIDL400PC10" | 867 | 51      | hwm=0.0.1   | 51   |        | 52    | 52  | 53   |
//| XU32'30:"30:kiwiTMAINIDL400PC10" | 866 | 53      | hwm=0.0.1   | 53   |        | 54    | 54  | 55   |
//| XU32'31:"31:kiwiTMAINIDL400PC10" | 865 | 55      | hwm=0.0.1   | 55   |        | 56    | 56  | 57   |
//| XU32'32:"32:kiwiTMAINIDL400PC10" | 864 | 57      | hwm=0.0.1   | 57   |        | 58    | 58  | 60   |
//| XU32'9:"9:kiwiTMAINIDL400PC10"   | 890 | 59      | hwm=0.0.0   | 59   |        | -     | -   | 12   |
//| XU32'33:"33:kiwiTMAINIDL400PC10" | 863 | 60      | hwm=0.0.0   | 60   |        | -     | -   | 59   |
//| XU32'35:"35:kiwiTMAINIDL400PC10" | 858 | 61      | hwm=0.0.0   | 61   |        | -     | -   | 62   |
//| XU32'35:"35:kiwiTMAINIDL400PC10" | 859 | 61      | hwm=0.0.0   | 61   |        | -     | -   | 64   |
//| XU32'36:"36:kiwiTMAINIDL400PC10" | 856 | 62      | hwm=1.1.0   | 63   |        | -     | -   | 61   |
//| XU32'36:"36:kiwiTMAINIDL400PC10" | 857 | 62      | hwm=1.1.0   | 63   |        | -     | -   | 64   |
//| XU32'37:"37:kiwiTMAINIDL400PC10" | 855 | 64      | hwm=0.2.0   | 66   |        | 65    | 66  | 67   |
//| XU32'38:"38:kiwiTMAINIDL400PC10" | 854 | 67      | hwm=0.0.0   | 67   |        | -     | -   | 69   |
//| XU32'34:"34:kiwiTMAINIDL400PC10" | 860 | 68      | hwm=0.0.0   | 68   |        | -     | -   | 61   |
//| XU32'34:"34:kiwiTMAINIDL400PC10" | 861 | 68      | hwm=0.0.0   | 68   |        | -     | -   | 67   |
//| XU32'34:"34:kiwiTMAINIDL400PC10" | 862 | 68      | hwm=0.0.0   | 68   |        | -     | -   | 70   |
//| XU32'39:"39:kiwiTMAINIDL400PC10" | 853 | 69      | hwm=0.0.0   | 69   |        | -     | -   | 68   |
//| XU32'40:"40:kiwiTMAINIDL400PC10" | 852 | 70      | hwm=0.0.0   | 70   |        | -     | -   | -    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'0:"0:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'0:"0:kiwiTMAINIDL400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                 |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                      |
//| F0   | E903 | R0 DATA |                                                                                                                      |
//| F0+E | E903 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F0 write(U64'9999999900000000) @_SINT/CC/SCALbx28_seed10 te=te:F0 write(S32'123456) @\ |
//|      |      |         | _SINT/CC/SCALbx24_statslookupprobes10 te=te:F0 write(S32'0) @_SINT/CC/SCALbx24_statslookups10 te=te:F0 write(S32'0)\ |
//|      |      |         |  @_SINT/CC/SCALbx24_statsinsertevictions10 te=te:F0 write(S32'0) @_SINT/CC/SCALbx24_statsinsertprobes10 te=te:F0 wr\ |
//|      |      |         | ite(S32'0) @_SINT/CC/SCALbx24_statsinserts10 te=te:F0 write(S32'0) @_SINT/CC/SCALbx24_nextvictim10 te=te:F0 write(S\ |
//|      |      |         | 32'0) @_SINT/CC/SCALbx24_nextfree10 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0 te=te:F0 write(S\ |
//|      |      |         | 32'0) TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9.V_0 te=te\ |
//|      |      |         | :F0 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9.V_2 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.\ |
//|      |      |         | 9.V_4 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.hash.6.10.V_0 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher\ |
//|      |      |         | .insert.1.9.V_5 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9.V_1 te=te:F0 write(S32'0) TMAIN_IDL400.C\ |
//|      |      |         | uckooHasher.insert.1.9.V_6 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9.V_7 te=te:F0 write(S32'0) TMA\ |
//|      |      |         | IN_IDL400.CuckooHasher.insert.1.9._SPILL.256 te=te:F0 write(S32'0) TMAIN_IDL400.Testbench.Main.V_3 te=te:F0 write(S\ |
//|      |      |         | 32'0) TMAIN_IDL400.Testbench.Main.V_2 te=te:F0 write(S32'0) TMAIN_IDL400.Testbench.Main.V_4 te=te:F0 write(S32'0) T\ |
//|      |      |         | MAIN_IDL400.Testbench.Main.V_11 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0 te=te:F0 write(S32'0\ |
//|      |      |         | ) TMAIN_IDL400.CuckooHasher.hash.3.10.V_0 te=te:F0 write(S32'0) TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1 te=te:F0 w\ |
//|      |      |         | rite(S32'0) TMAIN_IDL400.CuckooHasher.lookup.6.9._SPILL.256 te=te:F0 write(S32'0) TMAIN_IDL400.Testbench.Main.V_13 \ |
//|      |      |         | te=te:F0 write(U64'0) TMAIN_IDL400.Testbench.Main.V_12 te=te:F0 write(U64'0) TMAIN_IDL400.Testbench.Main.V_9 te=te:\ |
//|      |      |         | F0 write(S32'0) TMAIN_IDL400.Testbench.Main.V_8 te=te:F0 write(S32'0) TMAIN_IDL400.Testbench.Main.V_10 te=te:F0 wri\ |
//|      |      |         | te(S32'0)  PLI:Cuckoo cache testben...  W/P:Start                                                                    |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'1:"1:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'1:"1:kiwiTMAINIDL400PC10"
//*------+------+---------+----------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                           |
//*------+------+---------+----------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                |
//| F1   | E902 | R0 DATA |                                                                |
//| F1+E | E902 | W0 DATA | TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0 te=te:F1 write(S32'0) |
//*------+------+---------+----------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'2:"2:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'2:"2:kiwiTMAINIDL400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                           |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                                                                                |
//| F2   | E901 | R0 DATA |                                                                                                                                                |
//| F2+E | E901 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F2 write(U64'9999999900000000) @_SINT/CC/SCALbx28_seed10 te=te:F2 write(S32'123456) TMAIN_IDL400.Testbench.Main\ |
//|      |      |         | .V_3 te=te:F2 write(S32'0) TMAIN_IDL400.Testbench.Main.V_2 te=te:F2 write(S32'0) TMAIN_IDL400.Testbench.Main.V_4 te=te:F2 write(S32'0)  W/P:C\ |
//|      |      |         | ache Cleared  PLI:Cuckoo cache cleared                                                                                                         |
//| F2   | E900 | R0 DATA |                                                                                                                                                |
//| F2+E | E900 | W0 DATA | TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1 te=te:F2 write(S32'0)                                                                                 |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'4:"4:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'4:"4:kiwiTMAINIDL400PC10"
//*------+------+---------+-------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                  |
//*------+------+---------+-------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                       |
//| F3   | E897 | R0 DATA |                                                       |
//| F3+E | E897 | W0 DATA | @_SINT/CC/MAPR10NoCE2_ARB0_ te=te:F3 write(E1, S32'0) |
//| F4   | E897 | W1 DATA |                                                       |
//*------+------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'5:"5:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'5:"5:kiwiTMAINIDL400PC10"
//*------+------+---------+-------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                  |
//*------+------+---------+-------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                       |
//| F5   | E896 | R0 DATA |                                                       |
//| F5+E | E896 | W0 DATA | @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F5 write(E1, S32'0) |
//| F6   | E896 | W1 DATA |                                                       |
//*------+------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'6:"6:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'6:"6:kiwiTMAINIDL400PC10"
//*------+------+---------+-------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                  |
//*------+------+---------+-------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                       |
//| F7   | E895 | R0 DATA |                                                       |
//| F7+E | E895 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F7 write(E1, S32'0) |
//| F8   | E895 | W1 DATA |                                                       |
//*------+------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'3:"3:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'3:"3:kiwiTMAINIDL400PC10"
//*------+------+---------+-------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                        |
//*------+------+---------+-------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                             |
//| F9   | E899 | R0 DATA |                                                             |
//| F9+E | E899 | W0 DATA | TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0 te=te:F9 write(E2) |
//| F9   | E898 | R0 DATA |                                                             |
//| F9+E | E898 | W0 DATA | @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F9 write(E1, S32'0)       |
//| F10  | E898 | W1 DATA |                                                             |
//*------+------+---------+-------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'7:"7:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'7:"7:kiwiTMAINIDL400PC10"
//*-------+------+---------+--------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                         |
//*-------+------+---------+--------------------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                              |
//| F11   | E894 | R0 DATA |                                                              |
//| F11+E | E894 | W0 DATA | TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1 te=te:F11 write(E3) |
//*-------+------+---------+--------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'10:"10:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'10:"10:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                   |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| F12   | -    | R0 CTRL |                                                                                                                        |
//| F12   | E889 | R0 DATA |                                                                                                                        |
//| F12+E | E889 | W0 DATA |                                                                                                                        |
//| F12   | E888 | R0 DATA |                                                                                                                        |
//| F12+E | E888 | W0 DATA | @_SINT/CC/SCALbx24_statsinsertprobes10 te=te:F12 write(E4) TMAIN_IDL400.CuckooHasher.hash.6.10.V_0 te=te:F12 write(E5) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'11:"11:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'11:"11:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                         |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------*
//| F13   | -    | R0 CTRL | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F13 read(E6) @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F13 read(E6) @_SINT/CC/MAPR10NoCE2_ARB0_ t\ |
//|       |      |         | e=te:F13 read(E6) @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F13 read(E6)                                                             |
//| F14   | -    | R1 CTRL |                                                                                                                              |
//| F13   | E887 | R0 DATA |                                                                                                                              |
//| F14   | E887 | R1 DATA |                                                                                                                              |
//| F14+E | E887 | W0 DATA | TMAIN_IDL400.CuckooHasher.insert.1.9.V_5 te=te:F14 write(E6) @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F14 write(E6, E7)             |
//| F15   | E887 | W1 DATA |                                                                                                                              |
//| F13   | E886 | R0 DATA |                                                                                                                              |
//| F14   | E886 | R1 DATA |                                                                                                                              |
//| F14+E | E886 | W0 DATA | TMAIN_IDL400.CuckooHasher.insert.1.9.V_4 te=te:F14 write(E8) TMAIN_IDL400.CuckooHasher.insert.1.9.V_5 te=te:F14 write(E6)    |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'12:"12:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'12:"12:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F16   | -    | R0 CTRL |                                                     |
//| F16   | E885 | R0 DATA |                                                     |
//| F16+E | E885 | W0 DATA | @_SINT/CC/MAPR10NoCE2_ARB0_ te=te:F16 write(E9, E7) |
//| F17   | E885 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'13:"13:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'13:"13:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F18   | -    | R0 CTRL |                                                     |
//| F18   | E884 | R0 DATA |                                                     |
//| F18+E | E884 | W0 DATA | @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F18 write(E9, E7) |
//| F19   | E884 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'14:"14:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'14:"14:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                     |
//| F20   | E883 | R0 DATA |                                                     |
//| F20+E | E883 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F20 write(E9, E7) |
//| F21   | E883 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'15:"15:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'15:"15:kiwiTMAINIDL400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                           |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F22   | -    | R0 CTRL |                                                                                                                |
//| F22   | E882 | R0 DATA | @_SINT/CC/MAPR12NoCE0_ARA0_ te=te:F22 read(E9) @_SINT/CC/MAPR12NoCE1_ARA0_ te=te:F22 read(E9) @_SINT/CC/MAPR1\ |
//|       |      |         | 2NoCE2_ARA0_ te=te:F22 read(E9) @_SINT/CC/MAPR12NoCE3_ARA0_ te=te:F22 read(E9) @_SINT/CC/MAPR10NoCE0_ARB0_ te\ |
//|       |      |         | =te:F22 read(E9) @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F22 read(E9) @_SINT/CC/MAPR10NoCE2_ARB0_ te=te:F22 read(E9\ |
//|       |      |         | ) @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F22 read(E9)                                                               |
//| F23   | E882 | R1 DATA |                                                                                                                |
//| F23+E | E882 | W0 DATA | @_SINT/CC/SCALbx24_statsinsertevictions10 te=te:F23 write(E10) TMAIN_IDL400.CuckooHasher.insert.1.9.V_1 te=te\ |
//|       |      |         | :F23 write(E11) TMAIN_IDL400.CuckooHasher.insert.1.9.V_6 te=te:F23 write(E12) TMAIN_IDL400.CuckooHasher.inser\ |
//|       |      |         | t.1.9.V_7 te=te:F23 write(E13) @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F23 write(E9, E7)  PLI:Eviction %d needed     |
//| F24   | E882 | W1 DATA |                                                                                                                |
//| F22   | E881 | R0 DATA |                                                                                                                |
//| F22+E | E881 | W0 DATA | @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F22 write(E9, E7)                                                            |
//| F25   | E881 | W1 DATA |                                                                                                                |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'16:"16:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'16:"16:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F26   | -    | R0 CTRL |                                                     |
//| F26   | E880 | R0 DATA |                                                     |
//| F26+E | E880 | W0 DATA | @_SINT/CC/MAPR10NoCE2_ARB0_ te=te:F26 write(E9, E7) |
//| F27   | E880 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'17:"17:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'17:"17:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F28   | -    | R0 CTRL |                                                     |
//| F28   | E879 | R0 DATA |                                                     |
//| F28+E | E879 | W0 DATA | @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F28 write(E9, E7) |
//| F29   | E879 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'18:"18:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'18:"18:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F30   | -    | R0 CTRL |                                                     |
//| F30   | E878 | R0 DATA |                                                     |
//| F30+E | E878 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F30 write(E9, E7) |
//| F31   | E878 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'19:"19:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'19:"19:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F32   | -    | R0 CTRL |                                                      |
//| F32   | E877 | R0 DATA |                                                      |
//| F32+E | E877 | W0 DATA | @_SINT/CC/MAPR12NoCE3_ARA0_ te=te:F32 write(E9, E14) |
//| F33   | E877 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'20:"20:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'20:"20:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F34   | -    | R0 CTRL |                                                      |
//| F34   | E876 | R0 DATA |                                                      |
//| F34+E | E876 | W0 DATA | @_SINT/CC/MAPR12NoCE2_ARA0_ te=te:F34 write(E9, E14) |
//| F35   | E876 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'21:"21:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'21:"21:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F36   | -    | R0 CTRL |                                                      |
//| F36   | E875 | R0 DATA |                                                      |
//| F36+E | E875 | W0 DATA | @_SINT/CC/MAPR12NoCE1_ARA0_ te=te:F36 write(E9, E14) |
//| F37   | E875 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'22:"22:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'22:"22:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F38   | -    | R0 CTRL |                                                      |
//| F38   | E874 | R0 DATA |                                                      |
//| F38+E | E874 | W0 DATA | @_SINT/CC/MAPR12NoCE0_ARA0_ te=te:F38 write(E9, E14) |
//| F39   | E874 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'23:"23:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'23:"23:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                   |
//*-------+------+---------+------------------------------------------------------------------------*
//| F40   | -    | R0 CTRL |                                                                        |
//| F40   | E873 | R0 DATA |                                                                        |
//| F40+E | E873 | W0 DATA | TMAIN_IDL400.CuckooHasher.insert.1.9._SPILL.256 te=te:F40 write(S32'0) |
//*-------+------+---------+------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'24:"24:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'24:"24:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F41   | -    | R0 CTRL |                                                      |
//| F41   | E872 | R0 DATA |                                                      |
//| F41+E | E872 | W0 DATA | TMAIN_IDL400.Testbench.Main.V_3 te=te:F41 write(E15) |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'8:"8:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'8:"8:kiwiTMAINIDL400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                             |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------*
//| F42   | -    | R0 CTRL |                                                                                                                                  |
//| F42   | E893 | R0 DATA |                                                                                                                                  |
//| F42+E | E893 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F42 write(U64'9999999900000000) @_SINT/CC/SCALbx28_seed10 te=te:F42 write(S32'123456) TMAIN_IDL40\ |
//|       |      |         | 0.Testbench.Main.V_9 te=te:F42 write(S32'0) TMAIN_IDL400.Testbench.Main.V_8 te=te:F42 write(S32'0) TMAIN_IDL400.Testbench.Main.\ |
//|       |      |         | V_10 te=te:F42 write(S32'0)  W/P:Data Entered  PLI:Cuckoo cache inserte...                                                       |
//| F42   | E892 | R0 DATA |                                                                                                                                  |
//| F42+E | E892 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F42 write(E16) @_SINT/CC/SCALbx28_seed10 te=te:F42 write(E17) TMAIN_IDL400.CuckooHasher.insert.1.\ |
//|       |      |         | 9.V_0 te=te:F42 write(E17) TMAIN_IDL400.CuckooHasher.insert.1.9.V_2 te=te:F42 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9\ |
//|       |      |         | .V_1 te=te:F42 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9._SPILL.256 te=te:F42 write(E18)                                 |
//| F42   | E891 | R0 DATA |                                                                                                                                  |
//| F42+E | E891 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F42 write(E16) @_SINT/CC/SCALbx28_seed10 te=te:F42 write(E17) @_SINT/CC/SCALbx24_statsinserts10 t\ |
//|       |      |         | e=te:F42 write(E19) @_SINT/CC/SCALbx24_nextfree10 te=te:F42 write(E20) TMAIN_IDL400.CuckooHasher.insert.1.9.V_0 te=te:F42 write\ |
//|       |      |         | (E17) TMAIN_IDL400.CuckooHasher.insert.1.9.V_2 te=te:F42 write(E21) TMAIN_IDL400.CuckooHasher.insert.1.9.V_1 te=te:F42 write(S3\ |
//|       |      |         | 2'0) @64_US/CC/SCALbx26_ARA0_ te=te:F42 write(E21, E22)                                                                          |
//| F43   | E891 | W1 DATA |                                                                                                                                  |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'25:"25:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'25:"25:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                      |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------*
//| F44   | -    | R0 CTRL |                                                                                                           |
//| F44   | E871 | R0 DATA |                                                                                                           |
//| F44+E | E871 | W0 DATA | TMAIN_IDL400.Testbench.Main.V_2 te=te:F44 write(E23) TMAIN_IDL400.Testbench.Main.V_4 te=te:F44 write(E24) |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'26:"26:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'26:"26:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F45   | -    | R0 CTRL |                                                     |
//| F45   | E870 | R0 DATA |                                                     |
//| F45+E | E870 | W0 DATA | @_SINT/CC/MAPR10NoCE2_ARB0_ te=te:F45 write(E9, E7) |
//| F46   | E870 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'27:"27:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'27:"27:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F47   | -    | R0 CTRL |                                                     |
//| F47   | E869 | R0 DATA |                                                     |
//| F47+E | E869 | W0 DATA | @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F47 write(E9, E7) |
//| F48   | E869 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'28:"28:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'28:"28:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                |
//*-------+------+---------+-----------------------------------------------------*
//| F49   | -    | R0 CTRL |                                                     |
//| F49   | E868 | R0 DATA |                                                     |
//| F49+E | E868 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F49 write(E9, E7) |
//| F50   | E868 | W1 DATA |                                                     |
//*-------+------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'29:"29:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'29:"29:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F51   | -    | R0 CTRL |                                                      |
//| F51   | E867 | R0 DATA |                                                      |
//| F51+E | E867 | W0 DATA | @_SINT/CC/MAPR12NoCE3_ARA0_ te=te:F51 write(E9, E14) |
//| F52   | E867 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'30:"30:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'30:"30:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F53   | -    | R0 CTRL |                                                      |
//| F53   | E866 | R0 DATA |                                                      |
//| F53+E | E866 | W0 DATA | @_SINT/CC/MAPR12NoCE2_ARA0_ te=te:F53 write(E9, E14) |
//| F54   | E866 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'31:"31:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'31:"31:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F55   | -    | R0 CTRL |                                                      |
//| F55   | E865 | R0 DATA |                                                      |
//| F55+E | E865 | W0 DATA | @_SINT/CC/MAPR12NoCE1_ARA0_ te=te:F55 write(E9, E14) |
//| F56   | E865 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'32:"32:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'32:"32:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F57   | -    | R0 CTRL |                                                      |
//| F57   | E864 | R0 DATA |                                                      |
//| F57+E | E864 | W0 DATA | @_SINT/CC/MAPR12NoCE0_ARA0_ te=te:F57 write(E9, E14) |
//| F58   | E864 | W1 DATA |                                                      |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'9:"9:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'9:"9:kiwiTMAINIDL400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F59   | -    | R0 CTRL |                                                                                                                                 |
//| F59   | E890 | R0 DATA |                                                                                                                                 |
//| F59+E | E890 | W0 DATA | TMAIN_IDL400.CuckooHasher.insert.1.9.V_4 te=te:F59 write(S32'0) TMAIN_IDL400.CuckooHasher.insert.1.9.V_5 te=te:F59 write(S32'0) |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'33:"33:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'33:"33:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                  |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| F60   | -    | R0 CTRL |                                                                                                                       |
//| F60   | E863 | R0 DATA |                                                                                                                       |
//| F60+E | E863 | W0 DATA | @_SINT/CC/SCALbx24_nextvictim10 te=te:F60 write(E25) TMAIN_IDL400.CuckooHasher.insert.1.9.V_0 te=te:F60 write(E26) T\ |
//|       |      |         | MAIN_IDL400.CuckooHasher.insert.1.9.V_2 te=te:F60 write(E27)                                                          |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'35:"35:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'35:"35:kiwiTMAINIDL400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                     |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| F61   | -    | R0 CTRL |                                                                                                                          |
//| F61   | E859 | R0 DATA |                                                                                                                          |
//| F61+E | E859 | W0 DATA |                                                                                                                          |
//| F61   | E858 | R0 DATA |                                                                                                                          |
//| F61+E | E858 | W0 DATA | @_SINT/CC/SCALbx24_statslookupprobes10 te=te:F61 write(E28) TMAIN_IDL400.CuckooHasher.hash.3.10.V_0 te=te:F61 write(E29) |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'36:"36:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'36:"36:kiwiTMAINIDL400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F62   | -    | R0 CTRL | @_SINT/CC/MAPR10NoCE0_ARB0_ te=te:F62 read(E30) @_SINT/CC/MAPR10NoCE1_ARB0_ te=te:F62 read(E30) @_SINT/CC/MAPR10NoCE2_ARB0_ te\ |
//|       |      |         | =te:F62 read(E30) @_SINT/CC/MAPR10NoCE3_ARB0_ te=te:F62 read(E30)                                                               |
//| F63   | -    | R1 CTRL |                                                                                                                                 |
//| F62   | E857 | R0 DATA |                                                                                                                                 |
//| F63   | E857 | R1 DATA |                                                                                                                                 |
//| F63+E | E857 | W0 DATA | TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1 te=te:F63 write(E30)                                                                   |
//| F62   | E856 | R0 DATA |                                                                                                                                 |
//| F63   | E856 | R1 DATA |                                                                                                                                 |
//| F63+E | E856 | W0 DATA | TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0 te=te:F63 write(E31) TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1 te=te:F63 write(E30)     |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'37:"37:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'37:"37:kiwiTMAINIDL400PC10"
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                            |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F64   | -    | R0 CTRL |                                                                                                                                 |
//| F64   | E855 | R0 DATA | @_SINT/CC/MAPR12NoCE0_ARA0_ te=te:F64 read(E32) @_SINT/CC/MAPR12NoCE1_ARA0_ te=te:F64 read(E32) @_SINT/CC/MAPR12NoCE2_ARA0_ te\ |
//|       |      |         | =te:F64 read(E32) @_SINT/CC/MAPR12NoCE3_ARA0_ te=te:F64 read(E32)                                                               |
//| F65   | E855 | R1 DATA | @64_US/CC/SCALbx26_ARA0_ te=te:F65 read(E33)                                                                                    |
//| F66   | E855 | R2 DATA |                                                                                                                                 |
//| F66+E | E855 | W0 DATA | TMAIN_IDL400.CuckooHasher.lookup.6.9._SPILL.256 te=te:F66 write(E34) TMAIN_IDL400.Testbench.Main.V_13 te=te:F66 write(E35)      |
//*-------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'38:"38:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'38:"38:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                 |
//*-------+------+---------+------------------------------------------------------*
//| F67   | -    | R0 CTRL |                                                      |
//| F67   | E854 | R0 DATA |                                                      |
//| F67+E | E854 | W0 DATA | TMAIN_IDL400.Testbench.Main.V_9 te=te:F67 write(E36) |
//*-------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'34:"34:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'34:"34:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                              |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*
//| F68   | -    | R0 CTRL |                                                                                                                                   |
//| F68   | E862 | R0 DATA |                                                                                                                                   |
//| F68+E | E862 | W0 DATA |  PLI:Cuckoo cache demo fi...  PLI:cuckoo cache: lookup...  PLI:cuckoo cache: insert...  PLI:cuckoo cache: this=%...  W/P:Readbac\ |
//|       |      |         | k Done  PLI:Cuckoo cache retriev...                                                                                               |
//| F68   | E861 | R0 DATA |                                                                                                                                   |
//| F68+E | E861 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F68 write(E16) @_SINT/CC/SCALbx28_seed10 te=te:F68 write(E17) @_SINT/CC/SCALbx24_statslookups10 te\ |
//|       |      |         | =te:F68 write(E37) TMAIN_IDL400.Testbench.Main.V_11 te=te:F68 write(E17) TMAIN_IDL400.CuckooHasher.lookup.6.9._SPILL.256 te=te:F\ |
//|       |      |         | 68 write(S32'-4) TMAIN_IDL400.Testbench.Main.V_13 te=te:F68 write(U64'0) TMAIN_IDL400.Testbench.Main.V_12 te=te:F68 write(E22)    |
//| F68   | E860 | R0 DATA |                                                                                                                                   |
//| F68+E | E860 | W0 DATA | @64_US/CC/SCALbx28_dk10 te=te:F68 write(E16) @_SINT/CC/SCALbx28_seed10 te=te:F68 write(E17) @_SINT/CC/SCALbx24_statslookups10 te\ |
//|       |      |         | =te:F68 write(E37) TMAIN_IDL400.Testbench.Main.V_11 te=te:F68 write(E17) TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0 te=te:F68 writ\ |
//|       |      |         | e(S32'0) TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1 te=te:F68 write(S32'0) TMAIN_IDL400.Testbench.Main.V_13 te=te:F68 write(U64'0)\ |
//|       |      |         |  TMAIN_IDL400.Testbench.Main.V_12 te=te:F68 write(E22)                                                                            |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'39:"39:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'39:"39:kiwiTMAINIDL400PC10"
//*-------+------+---------+------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                       |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------*
//| F69   | -    | R0 CTRL |                                                                                                            |
//| F69   | E853 | R0 DATA |                                                                                                            |
//| F69+E | E853 | W0 DATA | TMAIN_IDL400.Testbench.Main.V_8 te=te:F69 write(E38) TMAIN_IDL400.Testbench.Main.V_10 te=te:F69 write(E39) |
//*-------+------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'40:"40:kiwiTMAINIDL400PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL400PC10 state=XU32'40:"40:kiwiTMAINIDL400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F70   | -    | R0 CTRL |                       |
//| F70   | E852 | R0 DATA |                       |
//| F70+E | E852 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0
//
//
//  E2 =.= C(1+TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0)
//
//
//  E3 =.= C(1+TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1)
//
//
//  E4 =.= C(1+@_SINT/CC/SCALbx24_statsinsertprobes10)
//
//
//  E5 =.= COND({[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4<4, (C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_0+51*TMAIN_IDL400.CuckooHasher.insert.1.9.V_4))<0]}, C(-51*TMAIN_IDL400.CuckooHasher.insert.1.9.V_4+-TMAIN_IDL400.CuckooHasher.insert.1.9.V_0), C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_0+51*TMAIN_IDL400.CuckooHasher.insert.1.9.V_4))
//
//
//  E6 =.= C(TMAIN_IDL400.CuckooHasher.hash.6.10.V_0%S'8192)
//
//
//  E7 =.= C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_0)
//
//
//  E8 =.= C(1+TMAIN_IDL400.CuckooHasher.insert.1.9.V_4)
//
//
//  E9 =.= TMAIN_IDL400.CuckooHasher.insert.1.9.V_5
//
//
//  E10 =.= C(1+@_SINT/CC/SCALbx24_statsinsertevictions10)
//
//
//  E11 =.= C(1+TMAIN_IDL400.CuckooHasher.insert.1.9.V_1)
//
//
//  E12 =.= COND(XU32'3:"3:USA40"==@$s@_SINT/CC/SCALbx22_ARB0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR10NoCE3_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'2:"2:USA40"==@$s@_SINT/CC/SCALbx22_ARB0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR10NoCE2_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'1:"1:USA40"==@$s@_SINT/CC/SCALbx22_ARB0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR10NoCE1_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'0:"0:USA40"==@$s@_SINT/CC/SCALbx22_ARB0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR10NoCE0_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), C(*UNDEF)))))
//
//
//  E13 =.= COND(XU32'3:"3:USA42"==@$s@_SINT/CC/SCALbx20_ARA0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR12NoCE3_ARA0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'2:"2:USA42"==@$s@_SINT/CC/SCALbx20_ARA0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR12NoCE2_ARA0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'1:"1:USA42"==@$s@_SINT/CC/SCALbx20_ARA0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR12NoCE1_ARA0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), COND(XU32'0:"0:USA42"==@$s@_SINT/CC/SCALbx20_ARA0[@_SINT/CC/SCALbx24_nextvictim10], C(@_SINT/CC/MAPR12NoCE0_ARA0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_5]), C(*UNDEF)))))
//
//
//  E14 =.= C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_2)
//
//
//  E15 =.= C(1+TMAIN_IDL400.Testbench.Main.V_3)
//
//
//  E16 =.= C64u(S64'1+(C64u(@64_US/CC/SCALbx28_dk10)))
//
//
//  E17 =.= C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10)
//
//
//  E18 =.= COND({[TMAIN_IDL400.Testbench.Main.V_4<S32'21845, XU32'32768:"32768:USA10"==@_SINT/CC/SCALbx24_nextfree10, |-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10))]}, S32'-2, S32'-4)
//
//
//  E19 =.= C(1+@_SINT/CC/SCALbx24_statsinserts10)
//
//
//  E20 =.= C(1+(C(@_SINT/CC/SCALbx24_nextfree10)))
//
//
//  E21 =.= C(@_SINT/CC/SCALbx24_nextfree10)
//
//
//  E22 =.= C64u(@64_US/CC/SCALbx28_dk10)
//
//
//  E23 =.= C(1+TMAIN_IDL400.Testbench.Main.V_2)
//
//
//  E24 =.= C(1+TMAIN_IDL400.Testbench.Main.V_4)
//
//
//  E25 =.= C((C(1+@_SINT/CC/SCALbx24_nextvictim10))%4)
//
//
//  E26 =.= C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_6)
//
//
//  E27 =.= C(TMAIN_IDL400.CuckooHasher.insert.1.9.V_7)
//
//
//  E28 =.= C(1+@_SINT/CC/SCALbx24_statslookupprobes10)
//
//
//  E29 =.= COND({[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0<4, (C(TMAIN_IDL400.Testbench.Main.V_11+51*TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0))<0]}, C(-51*TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0+-TMAIN_IDL400.Testbench.Main.V_11), C(TMAIN_IDL400.Testbench.Main.V_11+51*TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0))
//
//
//  E30 =.= C(TMAIN_IDL400.CuckooHasher.hash.3.10.V_0%S'8192)
//
//
//  E31 =.= C(1+TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0)
//
//
//  E32 =.= TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1
//
//
//  E33 =.= COND(XU32'3:"3:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE3_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'2:"2:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE2_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'1:"1:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE1_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'0:"0:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE0_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), C(*UNDEF)))))
//
//
//  E34 =.= COND(4==TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0, S32'-5, S32'0)
//
//
//  E35 =.= C64u(@64_US/CC/SCALbx26_ARA0[COND(XU32'3:"3:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE3_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'2:"2:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE2_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'1:"1:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE1_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), COND(XU32'0:"0:USA28"==@$s@_SINT/CC/SCALbx20_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], C(@_SINT/CC/MAPR12NoCE0_ARA0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_1]), C(*UNDEF)))))])
//
//
//  E36 =.= C(1+TMAIN_IDL400.Testbench.Main.V_9)
//
//
//  E37 =.= C(1+@_SINT/CC/SCALbx24_statslookups10)
//
//
//  E38 =.= C(1+TMAIN_IDL400.Testbench.Main.V_8)
//
//
//  E39 =.= C(1+TMAIN_IDL400.Testbench.Main.V_10)
//
//
//  E40 =.= TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0>=S'8192
//
//
//  E41 =.= TMAIN_IDL400.CuckooHasher.Clear.0.16.V_0<S'8192
//
//
//  E42 =.= TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1>=4
//
//
//  E43 =.= TMAIN_IDL400.CuckooHasher.Clear.0.16.V_1<4
//
//
//  E44 =.= TMAIN_IDL400.CuckooHasher.insert.1.9.V_4>=4
//
//
//  E45 =.= TMAIN_IDL400.CuckooHasher.insert.1.9.V_4<4
//
//
//  E46 =.= {[XU32'3:"3:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE3_ARB0_rdata|]; [XU32'3:"3:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE3ARB0rdatah10hold|]; [XU32'1:"1:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE1_ARB0_rdata|]; [XU32'1:"1:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE1ARB0rdatah10hold|]; [XU32'3:"3:USA30"!=@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'2:"2:USA30"!=@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'1:"1:USA30"!=@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'0:"0:USA30"!=@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4]]; [XU32'0:"0:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE0_ARB0_rdata|]; [XU32'0:"0:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE0ARB0rdatah10hold|]; [XU32'2:"2:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE2_ARB0_rdata|]; [XU32'2:"2:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE2ARB0rdatah10hold|]}
//
//
//  E47 =.= {[XU32'3:"3:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE3_ARB0_rdata|]; [XU32'3:"3:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE3ARB0rdatah10hold|]; [XU32'1:"1:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE1_ARB0_rdata|]; [XU32'1:"1:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE1ARB0rdatah10hold|]; [XU32'0:"0:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE0_ARB0_rdata|]; [XU32'0:"0:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE0ARB0rdatah10hold|]; [XU32'2:"2:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, |@_SINT/CC/MAPR10NoCE2_ARB0_rdata|]; [XU32'2:"2:USA30"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.insert.1.9.V_4], XU32'14:"14:kiwiTMAINIDL400PC10nz"!=kiwiTMAINIDL400PC10nz, |SINTCCMAPR10NoCE2ARB0rdatah10hold|]}
//
//
//  E48 =.= 4==TMAIN_IDL400.CuckooHasher.insert.1.9.V_4
//
//
//  E49 =.= 4!=TMAIN_IDL400.CuckooHasher.insert.1.9.V_4
//
//
//  E50 =.= TMAIN_IDL400.Testbench.Main.V_4>=S32'21845
//
//
//  E51 =.= {[TMAIN_IDL400.Testbench.Main.V_4<S32'21845, XU32'32768:"32768:USA10"==@_SINT/CC/SCALbx24_nextfree10, |-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10))]; [TMAIN_IDL400.Testbench.Main.V_4<S32'21845, !(|-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10)))]}
//
//
//  E52 =.= {[TMAIN_IDL400.Testbench.Main.V_4<S32'21845, XU32'32768:"32768:USA10"!=@_SINT/CC/SCALbx24_nextfree10, |-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10))]}
//
//
//  E53 =.= TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0>=4
//
//
//  E54 =.= TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0<4
//
//
//  E55 =.= TMAIN_IDL400.Testbench.Main.V_11==(COND(XU32'3:"3:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE3_ARB0_rdata, SINTCCMAPR10NoCE3ARB0rdatah10hold), COND(XU32'2:"2:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE2_ARB0_rdata, SINTCCMAPR10NoCE2ARB0rdatah10hold), COND(XU32'1:"1:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE1_ARB0_rdata, SINTCCMAPR10NoCE1ARB0rdatah10hold), COND(XU32'0:"0:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE0_ARB0_rdata, SINTCCMAPR10NoCE0ARB0rdatah10hold), *UNDEF)))))
//
//
//  E56 =.= TMAIN_IDL400.Testbench.Main.V_11!=(COND(XU32'3:"3:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE3_ARB0_rdata, SINTCCMAPR10NoCE3ARB0rdatah10hold), COND(XU32'2:"2:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE2_ARB0_rdata, SINTCCMAPR10NoCE2ARB0rdatah10hold), COND(XU32'1:"1:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE1_ARB0_rdata, SINTCCMAPR10NoCE1ARB0rdatah10hold), COND(XU32'0:"0:USA26"==@$s@_SINT/CC/SCALbx22_ARB0[TMAIN_IDL400.CuckooHasher.lookup.6.9.V_0], COND(XU32'63:"63:kiwiTMAINIDL400PC10nz"==kiwiTMAINIDL400PC10nz, @_SINT/CC/MAPR10NoCE0_ARB0_rdata, SINTCCMAPR10NoCE0ARB0rdatah10hold), *UNDEF)))))
//
//
//  E57 =.= TMAIN_IDL400.Testbench.Main.V_10>=S32'21845
//
//
//  E58 =.= {[TMAIN_IDL400.Testbench.Main.V_10<S32'21845, !(|-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10)))]}
//
//
//  E59 =.= {[TMAIN_IDL400.Testbench.Main.V_10<S32'21845, |-|(C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx28_seed10))]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for cuckoo_hash_demo to cuckoo_hash_demo

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//28 vectors of width 1
//
//1 vectors of width 7
//
//46 vectors of width 32
//
//5 vectors of width 64
//
//8 vectors of width 13
//
//1 vectors of width 15
//
//2 vectors of width 640
//
//8 array locations of width 64
//
//Total state bits in module = 3738 bits.
//
//640 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread Testbench.Main uid=Main10 has 269 CIL instructions in 54 basic blocks
//Thread mpc10 has 41 bevelab control states (pauses)
//Reindexed thread kiwiTMAINIDL400PC10 with 71 minor control states
// eof (HPR L/S Verilog)
