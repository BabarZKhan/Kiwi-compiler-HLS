

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:50:14
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -obj-dir-name=. -cfg-plot-each-step -kiwic-kcode-dump=enable -log-dir-name=d_obj_loopclassic_b -kiwic-autodispose=enable loopclassic.exe -vnl=loopclassic -vnl-rootmodname=DUT -vnl-resets=synchronous -repack-loglevel=0
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
    
/* portgroup= abstractionName=res2-directornets */
output reg [6:0] kiwiBENCMAIN4001PC10nz_pc_export,
    
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
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg signed [31:0] fastspilldup54;
  reg signed [31:0] fastspilldup50;
  reg [63:0] BENCMAIN400_bench_SortingTest_0_67_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_Check_3_14_V_1;
  reg signed [31:0] BENCMAIN400_SortingHat_Check_3_14_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_PrintOut_3_12_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_BubbleSort_2_2_V_2;
  reg BENCMAIN400_SortingHat_BubbleSort_2_2_V_1;
  reg signed [31:0] BENCMAIN400_SortingHat_BubbleSort_2_2_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_Swap_5_11_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_Swap_2_13_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_QsPartition_1_20_V_2;
  reg signed [31:0] BENCMAIN400_SortingHat_QsPartition_1_20_V_1;
  reg signed [31:0] BENCMAIN400_SortingHat_QsPartition_1_20_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_QuickSort_1_1_V_4;
  reg signed [31:0] BENCMAIN400_SortingHat_QuickSort_1_1_V_2;
  reg signed [31:0] BENCMAIN400_SortingHat_QuickSort_1_1_V_1;
  reg signed [31:0] BENCMAIN400_SortingHat_QuickSort_1_1_V_0;
  reg signed [31:0] BENCMAIN400_SortingHat_DataGen_0_2_V_0;
  reg [63:0] BENCMAIN400_bench_SortingTest_0_61_V_0;
  wire [63:0] hpr_tnow;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_SCALbx22_swaps10;
  reg signed [31:0] A_SINT_CC_SCALbx16_seed10;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx12_ARB0_rdata;
  reg [9:0] A_SINT_CC_SCALbx12_ARB0_addr;
  reg A_SINT_CC_SCALbx12_ARB0_wen;
  reg A_SINT_CC_SCALbx12_ARB0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx12_ARB0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR10NoCE0_ARA0_rdata;
  reg [9:0] A_SINT_CC_MAPR10NoCE0_ARA0_addr;
  reg A_SINT_CC_MAPR10NoCE0_ARA0_wen;
  reg A_SINT_CC_MAPR10NoCE0_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE0_ARA0_wdata;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx18_ARE0_rdata;
  reg [3:0] A_SINT_CC_SCALbx18_ARE0_addr;
  reg A_SINT_CC_SCALbx18_ARE0_wen;
  reg A_SINT_CC_SCALbx18_ARE0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx18_ARE0_wdata;
// abstractionName=res2-morenets
  reg signed [31:0] pipe26;
  reg signed [31:0] pipe24;
  reg signed [31:0] pipe22;
  reg signed [31:0] pipe20;
  reg signed [31:0] pipe18;
  reg signed [31:0] pipe16;
  reg signed [31:0] pipe14;
  reg signed [31:0] pipe12;
  reg signed [31:0] pipe10;
  reg signed [31:0] SINTCCMAPR10NoCE0ARA0rdatah12hold;
  reg SINTCCMAPR10NoCE0ARA0rdatah12shot0;
  reg signed [31:0] SINTCCMAPR10NoCE0ARA0rdatah10hold;
  reg SINTCCMAPR10NoCE0ARA0rdatah10shot0;
  reg signed [31:0] SINTCCSCALbx12ARB0rdatah12hold;
  reg SINTCCSCALbx12ARB0rdatah12shot0;
  reg signed [31:0] SINTCCSCALbx12ARB0rdatah10hold;
  reg SINTCCSCALbx12ARB0rdatah10shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah26hold;
  reg SINTCCSCALbx18ARE0rdatah26shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah24hold;
  reg SINTCCSCALbx18ARE0rdatah24shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah22hold;
  reg SINTCCSCALbx18ARE0rdatah22shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah20hold;
  reg SINTCCSCALbx18ARE0rdatah20shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah18hold;
  reg SINTCCSCALbx18ARE0rdatah18shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah16hold;
  reg SINTCCSCALbx18ARE0rdatah16shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah14hold;
  reg SINTCCSCALbx18ARE0rdatah14shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah12hold;
  reg SINTCCSCALbx18ARE0rdatah12shot0;
  reg signed [31:0] SINTCCSCALbx18ARE0rdatah10hold;
  reg SINTCCSCALbx18ARE0rdatah10shot0;
  reg [6:0] kiwiBENCMAIN4001PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin501343x10;
  wire signed [31:0] hprpin501349x10;
  wire signed [31:0] hprpin501355x10;
  wire signed [31:0] hprpin501361x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.BENCMAIN400_1/1.0
      if (reset)  begin 
               A_SINT_CC_SCALbx22_swaps10 <= 32'd0;
               fastspilldup54 <= 32'd0;
               fastspilldup50 <= 32'd0;
               BENCMAIN400_bench_SortingTest_0_67_V_0 <= 64'd0;
               BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'd0;
               BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_BubbleSort_2_2_V_1 <= 32'd0;
               BENCMAIN400_SortingHat_BubbleSort_2_2_V_2 <= 32'd0;
               BENCMAIN400_SortingHat_BubbleSort_2_2_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_QuickSort_1_1_V_4 <= 32'd0;
               BENCMAIN400_SortingHat_Swap_5_11_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_Swap_2_13_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_QsPartition_1_20_V_1 <= 32'd0;
               BENCMAIN400_SortingHat_QsPartition_1_20_V_2 <= 32'd0;
               BENCMAIN400_SortingHat_QsPartition_1_20_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_QuickSort_1_1_V_1 <= 32'd0;
               BENCMAIN400_SortingHat_QuickSort_1_1_V_0 <= 32'd0;
               BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= 32'd0;
               BENCMAIN400_bench_SortingTest_0_61_V_0 <= 64'd0;
               BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= 32'd0;
               A_SINT_CC_SCALbx16_seed10 <= 32'd0;
               kiwiBENCMAIN4001PC10nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiBENCMAIN4001PC10nz)
                  32'h0/*0:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h1/*1:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h1/*1:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2/*2:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2/*2:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h3/*3:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h4/*4:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h4/*4:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h5/*5:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h5/*5:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h6/*6:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h6/*6:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h7/*7:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h7/*7:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h8/*8:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h8/*8:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h9/*9:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h9/*9:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("LoopClassic Demo Start");
                          $display("   LCD jojo=%1d  test result=%1d", 32'sd3, 42'sh320_d3df_8666);
                          $display("   DATADEP jojo=%1d  test result=%1d", 32'sd3, 32'sd10);
                          $display("   LOOPFWD jojo=%1d  test result=%1d", 32'sd3, $signed(hprpin501361x10+(((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==
                          kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah24hold)^((32'h9/*9:kiwiBENCMAIN4001PC10nz*/==
                          kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah26hold))));
                          $display("   ASSOCRED jojo=%1d  test result=%1d", 32'sd3, 32'sd0);
                           kiwiBENCMAIN4001PC10nz <= 32'ha/*10:kiwiBENCMAIN4001PC10nz*/;
                           A_SINT_CC_SCALbx22_swaps10 <= 32'sh0;
                           fastspilldup54 <= 32'sh0;
                           fastspilldup50 <= 32'sh0;
                           BENCMAIN400_bench_SortingTest_0_67_V_0 <= 64'h0;
                           BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'sh0;
                           BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_1 <= 32'h0;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_2 <= 32'sh0;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_QuickSort_1_1_V_4 <= 32'sh0;
                           BENCMAIN400_SortingHat_Swap_5_11_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_Swap_2_13_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_QsPartition_1_20_V_1 <= 32'sh0;
                           BENCMAIN400_SortingHat_QsPartition_1_20_V_2 <= 32'sh0;
                           BENCMAIN400_SortingHat_QsPartition_1_20_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_QuickSort_1_1_V_1 <= 32'sh0;
                           BENCMAIN400_SortingHat_QuickSort_1_1_V_0 <= 32'sh0;
                           BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= 32'sh0;
                           BENCMAIN400_bench_SortingTest_0_61_V_0 <= 64'h0;
                           BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= 32'sh0;
                           A_SINT_CC_SCALbx16_seed10 <= -32'sh5567_3ee2;
                           KppWaypoint0 <= 32'sd1;
                           KppWaypoint1 <= "START";
                           KppWaypoint0 <= 32'sd2;
                           KppWaypoint1 <= "START-LOOP-CARRIED";
                           KppWaypoint0 <= 32'sd3;
                           KppWaypoint1 <= "START-DATADEP";
                           KppWaypoint0 <= 32'sd4;
                           KppWaypoint1 <= "START-LOOPFWD";
                           KppWaypoint0 <= 32'sd5;
                           KppWaypoint1 <= "START-ASSOCRED";
                           KppWaypoint0 <= 32'sd6;
                           KppWaypoint1 <= "START-SORT0";
                           end 
                          
                  32'hc/*12:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'he/*14:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'hb/*11:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+BENCMAIN400_SortingHat_DataGen_0_2_V_0)>=32'sh400)) $display("Sort arity=%1d start at %1d"
                              , 32'sd1024, $time);
                               else  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'hd/*13:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_DataGen_0_2_V_0);
                                   A_SINT_CC_SCALbx16_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx16_seed10);
                                   end 
                                  if (($signed(32'sd1+BENCMAIN400_SortingHat_DataGen_0_2_V_0)>=32'sh400))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'hc/*12:kiwiBENCMAIN4001PC10nz*/;
                                   A_SINT_CC_SCALbx22_swaps10 <= 32'sh0;
                                   BENCMAIN400_SortingHat_BubbleSort_2_2_V_0 <= 32'sh0;
                                   BENCMAIN400_bench_SortingTest_0_61_V_0 <= $time;
                                   BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_DataGen_0_2_V_0);
                                   end 
                                   end 
                          
                  32'ha/*10:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'hb/*11:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'hd/*13:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'ha/*10:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'he/*14:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'hf/*15:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_1 <= 32'h0;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_2 <= 32'sh0;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_BubbleSort_2_2_V_0);
                           end 
                          
                  32'h10/*16:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h11/*17:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h11/*17:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (((32'h10/*16:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (A_SINT_CC_SCALbx12_ARB0_rdata<SINTCCSCALbx12ARB0rdatah12hold
                          ): (SINTCCSCALbx12ARB0rdatah10hold<SINTCCSCALbx12ARB0rdatah12hold) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/!=
                          kiwiBENCMAIN4001PC10nz)) || (SINTCCSCALbx12ARB0rdatah10hold<A_SINT_CC_SCALbx12_ARB0_rdata) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/==
                          kiwiBENCMAIN4001PC10nz))  begin 
                                   A_SINT_CC_SCALbx22_swaps10 <= $signed(32'sd1+A_SINT_CC_SCALbx22_swaps10);
                                   BENCMAIN400_SortingHat_BubbleSort_2_2_V_1 <= 32'h1;
                                   end 
                                   kiwiBENCMAIN4001PC10nz <= 32'h12/*18:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h12/*18:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h13/*19:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h13/*19:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h14/*20:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'hf/*15:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_BubbleSort_2_2_V_2>=32'sd1023) && !BENCMAIN400_SortingHat_BubbleSort_2_2_V_1)  begin 
                                  $display("BubbleSort finished with %1d iterations and %1d swaps.", BENCMAIN400_SortingHat_BubbleSort_2_2_V_0
                                  , A_SINT_CC_SCALbx22_swaps10);
                                  $display("Sort arity=%1d quick=%1d finished after %1d", 32'sd1024, 32'sd0, $time+(0-BENCMAIN400_bench_SortingTest_0_61_V_0
                                  ));
                                   kiwiBENCMAIN4001PC10nz <= 32'h15/*21:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh0;
                                   end 
                                  if ((BENCMAIN400_SortingHat_BubbleSort_2_2_V_2>=32'sd1023) && BENCMAIN400_SortingHat_BubbleSort_2_2_V_1
                          )  kiwiBENCMAIN4001PC10nz <= 32'he/*14:kiwiBENCMAIN4001PC10nz*/;
                              if ((BENCMAIN400_SortingHat_BubbleSort_2_2_V_2<32'sd1023))  kiwiBENCMAIN4001PC10nz <= 32'h10/*16:kiwiBENCMAIN4001PC10nz*/;
                               end 
                          
                  32'h14/*20:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'hf/*15:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_BubbleSort_2_2_V_2 <= $signed(32'sd1+BENCMAIN400_SortingHat_BubbleSort_2_2_V_2);
                           end 
                          
                  32'h16/*22:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          )) $display("   ... snip ...");
                              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  begin 
                                   BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'sh0;
                                   BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'sh0;
                                   end 
                                  if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sd1014))  BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh3f6;
                               kiwiBENCMAIN4001PC10nz <= 32'h19/*25:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h17/*23:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          ): 1'd1) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sd1014)) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sh400)) $display("     Sorted %1d is %1d", BENCMAIN400_SortingHat_PrintOut_3_12_V_0, ((32'h17/*23:kiwiBENCMAIN4001PC10nz*/==
                              kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata: SINTCCSCALbx12ARB0rdatah12hold));
                              if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          )) $display("   ... snip ...");
                              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  begin 
                                   BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'sh0;
                                   BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'sh0;
                                   end 
                                  if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sd1014))  BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh3f6;
                               kiwiBENCMAIN4001PC10nz <= 32'h18/*24:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h15/*21:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  kiwiBENCMAIN4001PC10nz <= 32'h16/*22:kiwiBENCMAIN4001PC10nz*/;
                              if (((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          ): 1'd1) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          >=32'sd1014))  kiwiBENCMAIN4001PC10nz <= 32'h17/*23:kiwiBENCMAIN4001PC10nz*/;
                               end 
                          
                  32'h18/*24:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h15/*21:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_PrintOut_3_12_V_0);
                           end 
                          
                  32'h1a/*26:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h1b/*27:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h1b/*27:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (((32'h1a/*26:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (A_SINT_CC_SCALbx12_ARB0_rdata<SINTCCSCALbx12ARB0rdatah10hold
                          ): (SINTCCSCALbx12ARB0rdatah12hold<SINTCCSCALbx12ARB0rdatah10hold) && (32'h1b/*27:kiwiBENCMAIN4001PC10nz*/!=
                          kiwiBENCMAIN4001PC10nz)) || (SINTCCSCALbx12ARB0rdatah12hold<A_SINT_CC_SCALbx12_ARB0_rdata) && (32'h1b/*27:kiwiBENCMAIN4001PC10nz*/==
                          kiwiBENCMAIN4001PC10nz))  BENCMAIN400_SortingHat_Check_3_14_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_Check_3_14_V_0
                              );

                               kiwiBENCMAIN4001PC10nz <= 32'h1c/*28:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h19/*25:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_Check_3_14_V_1>=32'sd1023))  begin 
                                  $display("SortingHat Checker: violations=%1d", BENCMAIN400_SortingHat_Check_3_14_V_0);
                                  $display("Sort finished and checked. Violations=%1d", BENCMAIN400_SortingHat_Check_3_14_V_0);
                                   end 
                                   else  kiwiBENCMAIN4001PC10nz <= 32'h1a/*26:kiwiBENCMAIN4001PC10nz*/;
                          if ((BENCMAIN400_SortingHat_Check_3_14_V_1>=32'sd1023))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h3f/*63:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= 32'sh0;
                                   KppWaypoint0 <= 32'sd7;
                                   KppWaypoint1 <= "START-SORT1";
                                   end 
                                   end 
                          
                  32'h1c/*28:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h19/*25:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_Check_3_14_V_1 <= $signed(32'sd1+BENCMAIN400_SortingHat_Check_3_14_V_1);
                           end 
                          
                  32'h1f/*31:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h1d/*29:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h1d/*29:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h20/*32:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_QsPartition_1_20_V_2 <= $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_2);
                           end 
                          
                  32'h22/*34:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h23/*35:kiwiBENCMAIN4001PC10nz*/;
                           A_SINT_CC_SCALbx22_swaps10 <= $signed(32'sd1+A_SINT_CC_SCALbx22_swaps10);
                           BENCMAIN400_SortingHat_Swap_2_13_V_0 <= ((32'h22/*34:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
                          : SINTCCSCALbx12ARB0rdatah12hold);

                           BENCMAIN400_SortingHat_QsPartition_1_20_V_1 <= $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1);
                           end 
                          
                  32'h1e/*30:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h1f/*31:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h23/*35:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h1e/*30:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h21/*33:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (((32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (BENCMAIN400_SortingHat_QsPartition_1_20_V_0
                          <A_SINT_CC_SCALbx12_ARB0_rdata): (BENCMAIN400_SortingHat_QsPartition_1_20_V_0<SINTCCSCALbx12ARB0rdatah12hold
                          )) && (-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_1>=BENCMAIN400_SortingHat_QsPartition_1_20_V_2))  kiwiBENCMAIN4001PC10nz
                               <= 32'h1d/*29:kiwiBENCMAIN4001PC10nz*/;

                              if (((32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (BENCMAIN400_SortingHat_QsPartition_1_20_V_0
                          >=A_SINT_CC_SCALbx12_ARB0_rdata): (BENCMAIN400_SortingHat_QsPartition_1_20_V_0>=SINTCCSCALbx12ARB0rdatah12hold
                          )) && (-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_1>=BENCMAIN400_SortingHat_QsPartition_1_20_V_2))  kiwiBENCMAIN4001PC10nz
                               <= 32'h22/*34:kiwiBENCMAIN4001PC10nz*/;

                              if ((-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_1<BENCMAIN400_SortingHat_QsPartition_1_20_V_2))  kiwiBENCMAIN4001PC10nz
                               <= 32'h24/*36:kiwiBENCMAIN4001PC10nz*/;

                               end 
                          
                  32'h24/*36:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h25/*37:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h25/*37:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h26/*38:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h26/*38:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h27/*39:kiwiBENCMAIN4001PC10nz*/;
                           A_SINT_CC_SCALbx22_swaps10 <= $signed(32'sd1+A_SINT_CC_SCALbx22_swaps10);
                           BENCMAIN400_SortingHat_Swap_5_11_V_0 <= ((32'h25/*37:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
                          : SINTCCSCALbx12ARB0rdatah12hold);

                           end 
                          
                  32'h27/*39:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2e/*46:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h29/*41:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2a/*42:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2a/*42:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2b/*43:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h20/*32:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h21/*33:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2b/*43:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h20/*32:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_QsPartition_1_20_V_1 <= $signed(-32'sd1+((32'h29/*41:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
                          )? A_SINT_CC_MAPR10NoCE0_ARA0_rdata: SINTCCMAPR10NoCE0ARA0rdatah10hold));

                           BENCMAIN400_SortingHat_QsPartition_1_20_V_2 <= ((32'h29/*41:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
                          )? A_SINT_CC_MAPR10NoCE0_ARA0_rdata: SINTCCMAPR10NoCE0ARA0rdatah10hold);

                           BENCMAIN400_SortingHat_QsPartition_1_20_V_0 <= ((32'h2b/*43:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
                          )? A_SINT_CC_SCALbx12_ARB0_rdata: SINTCCSCALbx12ARB0rdatah12hold);

                           BENCMAIN400_SortingHat_QuickSort_1_1_V_1 <= ((32'h2a/*42:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_MAPR10NoCE0_ARA0_rdata
                          : SINTCCMAPR10NoCE0ARA0rdatah12hold);

                           BENCMAIN400_SortingHat_QuickSort_1_1_V_0 <= ((32'h29/*41:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_MAPR10NoCE0_ARA0_rdata
                          : SINTCCMAPR10NoCE0ARA0rdatah10hold);

                           BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= $signed(-32'sd1+$signed(-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2
                          ));

                           end 
                          
                  32'h2d/*45:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h28/*40:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2f/*47:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h31/*49:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2e/*46:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_QuickSort_1_1_V_0>=-32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                          ))) $display("QsPartition left=%1d axant=%1d right=%1d", BENCMAIN400_SortingHat_QuickSort_1_1_V_0, BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                              , BENCMAIN400_SortingHat_QuickSort_1_1_V_1);
                               else  begin 
                                  $display("QsPartition left=%1d axant=%1d right=%1d", BENCMAIN400_SortingHat_QuickSort_1_1_V_0, BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                                  , BENCMAIN400_SortingHat_QuickSort_1_1_V_1);
                                   kiwiBENCMAIN4001PC10nz <= 32'h30/*48:kiwiBENCMAIN4001PC10nz*/;
                                   fastspilldup50 <= $signed(32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2));
                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_4 <= $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                                  );

                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= $signed(32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2
                                  ));

                                   end 
                                  if ((BENCMAIN400_SortingHat_QuickSort_1_1_V_0>=-32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                          )))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h2f/*47:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_4 <= $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
                                  );

                                   end 
                                   end 
                          
                  32'h30/*48:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h33/*51:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h2c/*44:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2d/*45:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h32/*50:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h2c/*44:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h33/*51:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h34/*52:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h31/*49:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_4
                      <BENCMAIN400_SortingHat_QuickSort_1_1_V_1))  begin 
                               kiwiBENCMAIN4001PC10nz <= 32'h32/*50:kiwiBENCMAIN4001PC10nz*/;
                               fastspilldup54 <= $signed(32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2));
                               BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= $signed(32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2
                              ));

                               end 
                               else  kiwiBENCMAIN4001PC10nz <= 32'h28/*40:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h34/*52:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h31/*49:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h36/*54:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          )) $display("   ... snip ...");
                              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  begin 
                                   BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'sh0;
                                   BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'sh0;
                                   end 
                                  if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sd1014))  BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh3f6;
                               kiwiBENCMAIN4001PC10nz <= 32'h39/*57:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h37/*55:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          ): 1'd1) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sd1014)) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sh400)) $display("     Sorted %1d is %1d", BENCMAIN400_SortingHat_PrintOut_3_12_V_0, ((32'h37/*55:kiwiBENCMAIN4001PC10nz*/==
                              kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata: SINTCCSCALbx12ARB0rdatah12hold));
                              if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          )) $display("   ... snip ...");
                              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  begin 
                                   BENCMAIN400_SortingHat_Check_3_14_V_0 <= 32'sh0;
                                   BENCMAIN400_SortingHat_Check_3_14_V_1 <= 32'sh0;
                                   end 
                                  if ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          <32'sd1014))  BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh3f6;
                               kiwiBENCMAIN4001PC10nz <= 32'h38/*56:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h35/*53:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  kiwiBENCMAIN4001PC10nz <= 32'h36/*54:kiwiBENCMAIN4001PC10nz*/;
                              if (((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014
                          ): 1'd1) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          >=32'sd1014))  kiwiBENCMAIN4001PC10nz <= 32'h37/*55:kiwiBENCMAIN4001PC10nz*/;
                               end 
                          
                  32'h38/*56:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h35/*53:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_PrintOut_3_12_V_0);
                           end 
                          
                  32'h3a/*58:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h3b/*59:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h3b/*59:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (((32'h3a/*58:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (A_SINT_CC_SCALbx12_ARB0_rdata<SINTCCSCALbx12ARB0rdatah10hold
                          ): (SINTCCSCALbx12ARB0rdatah12hold<SINTCCSCALbx12ARB0rdatah10hold) && (32'h3b/*59:kiwiBENCMAIN4001PC10nz*/!=
                          kiwiBENCMAIN4001PC10nz)) || (SINTCCSCALbx12ARB0rdatah12hold<A_SINT_CC_SCALbx12_ARB0_rdata) && (32'h3b/*59:kiwiBENCMAIN4001PC10nz*/==
                          kiwiBENCMAIN4001PC10nz))  BENCMAIN400_SortingHat_Check_3_14_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_Check_3_14_V_0
                              );

                               kiwiBENCMAIN4001PC10nz <= 32'h3c/*60:kiwiBENCMAIN4001PC10nz*/;
                           end 
                          
                  32'h39/*57:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_Check_3_14_V_1>=32'sd1023))  begin 
                                  $display("SortingHat Checker: violations=%1d", BENCMAIN400_SortingHat_Check_3_14_V_0);
                                  $display("Sort finished and checked. Violations=%1d", BENCMAIN400_SortingHat_Check_3_14_V_0);
                                  $display(" Test LoopClassic finished.");
                                  $finish(32'sd0);
                                   end 
                                   else  kiwiBENCMAIN4001PC10nz <= 32'h3a/*58:kiwiBENCMAIN4001PC10nz*/;
                          if ((BENCMAIN400_SortingHat_Check_3_14_V_1>=32'sd1023))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h3d/*61:kiwiBENCMAIN4001PC10nz*/;
                                   KppWaypoint0 <= 32'sd8;
                                   KppWaypoint1 <= "FINISH";
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h3c/*60:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h39/*57:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_Check_3_14_V_1 <= $signed(32'sd1+BENCMAIN400_SortingHat_Check_3_14_V_1);
                           end 
                          
                  32'h3d/*61:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h3e/*62:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h3e/*62:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $finish(32'sd0);
                           kiwiBENCMAIN4001PC10nz <= 32'h44/*68:kiwiBENCMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          
                  32'h40/*64:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h41/*65:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h28/*40:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((BENCMAIN400_SortingHat_QuickSort_1_1_V_2<32'sd0
                      ))  begin 
                              $display("QuickSort finished after %1d swaps.", A_SINT_CC_SCALbx22_swaps10);
                              $display("Sort arity=%1d quick=%1d finished after %1d", 32'sd1024, 32'sd1, $time+(0-BENCMAIN400_bench_SortingTest_0_67_V_0
                              ));
                               kiwiBENCMAIN4001PC10nz <= 32'h35/*53:kiwiBENCMAIN4001PC10nz*/;
                               BENCMAIN400_SortingHat_PrintOut_3_12_V_0 <= 32'sh0;
                               end 
                               else  kiwiBENCMAIN4001PC10nz <= 32'h29/*41:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h41/*65:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h28/*40:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h42/*66:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h43/*67:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_SortingHat_DataGen_0_2_V_0 <= $signed(32'sd1+BENCMAIN400_SortingHat_DataGen_0_2_V_0);
                           end 
                          
                  32'h3f/*63:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((BENCMAIN400_SortingHat_DataGen_0_2_V_0>=32'sh400)) $display("Sort arity=%1d start at %1d", 32'sd1024, $time);
                               else  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h42/*66:kiwiBENCMAIN4001PC10nz*/;
                                   A_SINT_CC_SCALbx16_seed10 <= $signed(32'sh2aa0_1d31+32'sh7ff8_a3ed*A_SINT_CC_SCALbx16_seed10);
                                   end 
                                  if ((BENCMAIN400_SortingHat_DataGen_0_2_V_0>=32'sh400))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h40/*64:kiwiBENCMAIN4001PC10nz*/;
                                   A_SINT_CC_SCALbx22_swaps10 <= 32'sh0;
                                   BENCMAIN400_bench_SortingTest_0_67_V_0 <= $time;
                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_1 <= 32'sh3ff;
                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_0 <= 32'sh0;
                                   BENCMAIN400_SortingHat_QuickSort_1_1_V_2 <= 32'sh1;
                                   end 
                                   end 
                          
                  32'h43/*67:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h3f/*63:kiwiBENCMAIN4001PC10nz*/;
                      endcase
              if (reset)  begin 
               kiwiBENCMAIN4001PC10nz_pc_export <= 32'd0;
               pipe26 <= 32'd0;
               pipe24 <= 32'd0;
               pipe22 <= 32'd0;
               pipe20 <= 32'd0;
               pipe18 <= 32'd0;
               pipe16 <= 32'd0;
               pipe14 <= 32'd0;
               pipe12 <= 32'd0;
               pipe10 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah10hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah12hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah14hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah16hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah18hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah20hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah22hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah24hold <= 32'd0;
               SINTCCSCALbx18ARE0rdatah26hold <= 32'd0;
               SINTCCSCALbx12ARB0rdatah10hold <= 32'd0;
               SINTCCSCALbx12ARB0rdatah12hold <= 32'd0;
               SINTCCMAPR10NoCE0ARA0rdatah10hold <= 32'd0;
               SINTCCMAPR10NoCE0ARA0rdatah12hold <= 32'd0;
               SINTCCSCALbx12ARB0rdatah10shot0 <= 32'd0;
               SINTCCSCALbx12ARB0rdatah12shot0 <= 32'd0;
               SINTCCMAPR10NoCE0ARA0rdatah12shot0 <= 32'd0;
               SINTCCMAPR10NoCE0ARA0rdatah10shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah26shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah24shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah22shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah20shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah18shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah16shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah14shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah12shot0 <= 32'd0;
               SINTCCSCALbx18ARE0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (SINTCCMAPR10NoCE0ARA0rdatah12shot0)  SINTCCMAPR10NoCE0ARA0rdatah12hold <= A_SINT_CC_MAPR10NoCE0_ARA0_rdata;
                      if (SINTCCMAPR10NoCE0ARA0rdatah10shot0)  SINTCCMAPR10NoCE0ARA0rdatah10hold <= A_SINT_CC_MAPR10NoCE0_ARA0_rdata;
                      if (SINTCCSCALbx12ARB0rdatah12shot0)  SINTCCSCALbx12ARB0rdatah12hold <= A_SINT_CC_SCALbx12_ARB0_rdata;
                      if (SINTCCSCALbx12ARB0rdatah10shot0)  SINTCCSCALbx12ARB0rdatah10hold <= A_SINT_CC_SCALbx12_ARB0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah26shot0)  SINTCCSCALbx18ARE0rdatah26hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah24shot0)  SINTCCSCALbx18ARE0rdatah24hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah22shot0)  SINTCCSCALbx18ARE0rdatah22hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah20shot0)  SINTCCSCALbx18ARE0rdatah20hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah18shot0)  SINTCCSCALbx18ARE0rdatah18hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah16shot0)  SINTCCSCALbx18ARE0rdatah16hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah14shot0)  SINTCCSCALbx18ARE0rdatah14hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah12shot0)  SINTCCSCALbx18ARE0rdatah12hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                      if (SINTCCSCALbx18ARE0rdatah10shot0)  SINTCCSCALbx18ARE0rdatah10hold <= A_SINT_CC_SCALbx18_ARE0_rdata;
                       kiwiBENCMAIN4001PC10nz_pc_export <= kiwiBENCMAIN4001PC10nz;
                   pipe26 <= BENCMAIN400_SortingHat_BubbleSort_2_2_V_2;
                   pipe24 <= pipe22;
                   pipe22 <= 32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1;
                   pipe20 <= pipe18;
                   pipe18 <= pipe16;
                   pipe16 <= BENCMAIN400_SortingHat_QuickSort_1_1_V_1;
                   pipe14 <= $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1);
                   pipe12 <= BENCMAIN400_SortingHat_QuickSort_1_1_V_2;
                   pipe10 <= BENCMAIN400_SortingHat_Check_3_14_V_1;
                   SINTCCSCALbx12ARB0rdatah10shot0 <= (32'h3a/*58:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h1a/*26:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || (BENCMAIN400_SortingHat_BubbleSort_2_2_V_2<32'sd1023) && (32'hf/*15:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || (32'h25/*37:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);

                   SINTCCSCALbx12ARB0rdatah12shot0 <= ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400)? ((32'h35/*53:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                  >=32'sd1014): (32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz)) || ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? ((32'h35/*53:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                  <32'sd1014): (32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz)) || (-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_1>=BENCMAIN400_SortingHat_QsPartition_1_20_V_2
                  ) && (32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) && (BENCMAIN400_SortingHat_QsPartition_1_20_V_0>=
                  A_SINT_CC_SCALbx12_ARB0_rdata) || (32'h20/*32:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h10/*16:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || (32'h24/*36:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h2a/*42:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz) || ((32'h39/*57:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h19/*25:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz)) && (BENCMAIN400_SortingHat_Check_3_14_V_1<32'sd1023);

                   SINTCCMAPR10NoCE0ARA0rdatah12shot0 <= (32'h29/*41:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCMAPR10NoCE0ARA0rdatah10shot0 <= (BENCMAIN400_SortingHat_QuickSort_1_1_V_2>=32'sd0) && (32'h28/*40:kiwiBENCMAIN4001PC10nz*/==
                  kiwiBENCMAIN4001PC10nz);

                   SINTCCSCALbx18ARE0rdatah26shot0 <= (32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah24shot0 <= (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah22shot0 <= (32'h6/*6:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah20shot0 <= (32'h5/*5:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah18shot0 <= (32'h4/*4:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah16shot0 <= (32'h3/*3:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah14shot0 <= (32'h2/*2:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah12shot0 <= (32'h1/*1:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   SINTCCSCALbx18ARE0rdatah10shot0 <= (32'h0/*0:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.BENCMAIN400_1/1.0


       end 
      

 always   @(* )  begin 
       A_SINT_CC_SCALbx18_ARE0_addr = 32'sd0;
       A_SINT_CC_SCALbx12_ARB0_addr = 32'sd0;
       A_SINT_CC_SCALbx12_ARB0_wdata = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARA0_wdata = 32'sd0;
       A_SINT_CC_SCALbx18_ARE0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARA0_ren = 32'sd0;
       A_SINT_CC_SCALbx12_ARB0_ren = 32'sd0;
       A_SINT_CC_MAPR10NoCE0_ARA0_wen = 32'sd0;
       A_SINT_CC_SCALbx12_ARB0_wen = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiBENCMAIN4001PC10nz)
          32'h0/*0:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd2;

          32'h1/*1:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd1;

          32'h2/*2:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd3;

          32'h3/*3:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd4;

          32'h4/*4:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd5;

          32'h5/*5:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd6;

          32'h6/*6:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd7;

          32'h7/*7:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd8;

          32'h8/*8:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx18_ARE0_addr = 32'd9;

          32'hb/*11:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_DataGen_0_2_V_0;
               A_SINT_CC_SCALbx12_ARB0_wdata = A_SINT_CC_SCALbx16_seed10;
               end 
              
          32'h10/*16:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = pipe26;

          32'h11/*17:kiwiBENCMAIN4001PC10nz*/: if (((32'h10/*16:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (A_SINT_CC_SCALbx12_ARB0_rdata
          <SINTCCSCALbx12ARB0rdatah12hold): (SINTCCSCALbx12ARB0rdatah10hold<SINTCCSCALbx12ARB0rdatah12hold) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/!=
          kiwiBENCMAIN4001PC10nz)) || (SINTCCSCALbx12ARB0rdatah10hold<A_SINT_CC_SCALbx12_ARB0_rdata) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/==
          kiwiBENCMAIN4001PC10nz))  begin 
                   A_SINT_CC_SCALbx12_ARB0_addr = 32'd1+BENCMAIN400_SortingHat_BubbleSort_2_2_V_2;
                   A_SINT_CC_SCALbx12_ARB0_wdata = ((32'h11/*17:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
                  : SINTCCSCALbx12ARB0rdatah12hold);

                   end 
                  
          32'h12/*18:kiwiBENCMAIN4001PC10nz*/: if (((32'h10/*16:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (A_SINT_CC_SCALbx12_ARB0_rdata
          <SINTCCSCALbx12ARB0rdatah12hold): (SINTCCSCALbx12ARB0rdatah10hold<SINTCCSCALbx12ARB0rdatah12hold) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/!=
          kiwiBENCMAIN4001PC10nz)) || (SINTCCSCALbx12ARB0rdatah10hold<A_SINT_CC_SCALbx12_ARB0_rdata) && (32'h11/*17:kiwiBENCMAIN4001PC10nz*/==
          kiwiBENCMAIN4001PC10nz))  begin 
                   A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_BubbleSort_2_2_V_2;
                   A_SINT_CC_SCALbx12_ARB0_wdata = ((32'h10/*16:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
                  : SINTCCSCALbx12ARB0rdatah10hold);

                   end 
                  
          32'hf/*15:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_BubbleSort_2_2_V_2<32'sd1023))  A_SINT_CC_SCALbx12_ARB0_addr
               = 32'd1+BENCMAIN400_SortingHat_BubbleSort_2_2_V_2;

              
          32'h15/*21:kiwiBENCMAIN4001PC10nz*/:  begin 
              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                  ;

                  if (((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014): 1'd1
              ) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sd1014))  begin 
                      if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          ;

                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          ;

                           A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0;
                       end 
                       end 
              
          32'h1a/*26:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = pipe10;

          32'h19/*25:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_Check_3_14_V_1<32'sd1023))  A_SINT_CC_SCALbx12_ARB0_addr = 32'd1
              +BENCMAIN400_SortingHat_Check_3_14_V_1;

              
          32'h22/*34:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_SCALbx12_ARB0_addr = $signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1);
               A_SINT_CC_SCALbx12_ARB0_wdata = ((32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
              : SINTCCSCALbx12ARB0rdatah12hold);

               end 
              
          32'h1e/*30:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_QsPartition_1_20_V_2;
               A_SINT_CC_SCALbx12_ARB0_wdata = BENCMAIN400_SortingHat_Swap_2_13_V_0;
               end 
              
          32'h21/*33:kiwiBENCMAIN4001PC10nz*/: if (((32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? (BENCMAIN400_SortingHat_QsPartition_1_20_V_0
          >=A_SINT_CC_SCALbx12_ARB0_rdata): (BENCMAIN400_SortingHat_QsPartition_1_20_V_0>=SINTCCSCALbx12ARB0rdatah12hold)) && (-32'sd1
          +BENCMAIN400_SortingHat_QuickSort_1_1_V_1>=BENCMAIN400_SortingHat_QsPartition_1_20_V_2))  A_SINT_CC_SCALbx12_ARB0_addr = pipe14
              ;

              
          32'h24/*36:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = pipe24;

          32'h25/*37:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = pipe20;

          32'h26/*38:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_SCALbx12_ARB0_addr = 32'd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1;
               A_SINT_CC_SCALbx12_ARB0_wdata = ((32'h26/*38:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx12_ARB0_rdata
              : SINTCCSCALbx12ARB0rdatah10hold);

               end 
              
          32'h29/*41:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_MAPR10NoCE0_ARA0_addr = pipe12;

          32'h2a/*42:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = ((32'h2a/*42:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
          )? A_SINT_CC_MAPR10NoCE0_ARA0_rdata: SINTCCMAPR10NoCE0ARA0rdatah12hold);


          32'h20/*32:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_QsPartition_1_20_V_2;

          32'h2e/*46:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_QuickSort_1_1_V_0>=-32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
          )))  begin 
                   A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_QuickSort_1_1_V_1;
                   A_SINT_CC_SCALbx12_ARB0_wdata = BENCMAIN400_SortingHat_Swap_5_11_V_0;
                   end 
                   else  begin 
                   A_SINT_CC_MAPR10NoCE0_ARA0_addr = $signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2);
                   A_SINT_CC_MAPR10NoCE0_ARA0_wdata = BENCMAIN400_SortingHat_QuickSort_1_1_V_0;
                   A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_QuickSort_1_1_V_1;
                   A_SINT_CC_SCALbx12_ARB0_wdata = BENCMAIN400_SortingHat_Swap_5_11_V_0;
                   end 
                  
          32'h2c/*44:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_MAPR10NoCE0_ARA0_addr = fastspilldup54;
               A_SINT_CC_MAPR10NoCE0_ARA0_wdata = BENCMAIN400_SortingHat_QuickSort_1_1_V_1;
               end 
              
          32'h33/*51:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_MAPR10NoCE0_ARA0_addr = fastspilldup50;
               A_SINT_CC_MAPR10NoCE0_ARA0_wdata = $signed(-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_4);
               end 
              
          32'h31/*49:kiwiBENCMAIN4001PC10nz*/: if ((32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_4<BENCMAIN400_SortingHat_QuickSort_1_1_V_1
          ))  begin 
                   A_SINT_CC_MAPR10NoCE0_ARA0_addr = $signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2);
                   A_SINT_CC_MAPR10NoCE0_ARA0_wdata = $signed(32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_4);
                   end 
                  
          32'h35/*53:kiwiBENCMAIN4001PC10nz*/:  begin 
              if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                  ;

                  if (((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0)? (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014): 1'd1
              ) || (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sd1014))  begin 
                      if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          ;

                          if ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sh400))  A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0
                          ;

                           A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_PrintOut_3_12_V_0;
                       end 
                       end 
              
          32'h3a/*58:kiwiBENCMAIN4001PC10nz*/:  A_SINT_CC_SCALbx12_ARB0_addr = pipe10;

          32'h39/*57:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_Check_3_14_V_1<32'sd1023))  A_SINT_CC_SCALbx12_ARB0_addr = 32'd1
              +BENCMAIN400_SortingHat_Check_3_14_V_1;

              
          32'h40/*64:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_MAPR10NoCE0_ARA0_addr = 32'd0;
               A_SINT_CC_MAPR10NoCE0_ARA0_wdata = 32'sh0;
               end 
              
          32'h28/*40:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_QuickSort_1_1_V_2>=32'sd0))  A_SINT_CC_MAPR10NoCE0_ARA0_addr
               = $signed(-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_2);

              
          32'h42/*66:kiwiBENCMAIN4001PC10nz*/:  begin 
               A_SINT_CC_SCALbx12_ARB0_addr = BENCMAIN400_SortingHat_DataGen_0_2_V_0;
               A_SINT_CC_SCALbx12_ARB0_wdata = A_SINT_CC_SCALbx16_seed10;
               end 
              
          32'h3f/*63:kiwiBENCMAIN4001PC10nz*/: if ((BENCMAIN400_SortingHat_DataGen_0_2_V_0>=32'sh400))  begin 
                   A_SINT_CC_MAPR10NoCE0_ARA0_addr = 32'd1;
                   A_SINT_CC_MAPR10NoCE0_ARA0_wdata = 32'sh3ff;
                   end 
                  endcase
      if (hpr_int_run_enable_DDX18)  begin 
               A_SINT_CC_SCALbx18_ARE0_ren = ((32'h0/*0:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h2/*2:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h4/*4:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h6/*6:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h5/*5:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h3/*3:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h1/*1:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? 32'd1: 32'd0);

               A_SINT_CC_MAPR10NoCE0_ARA0_ren = (32'h29/*41:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (BENCMAIN400_SortingHat_QuickSort_1_1_V_2
              >=32'sd0) && (32'h28/*40:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);

               A_SINT_CC_SCALbx12_ARB0_ren = ((BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sh400)? ((32'sd10<BENCMAIN400_SortingHat_PrintOut_3_12_V_0
              )? ((32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              )) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0<32'sd1014): (32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)) || ((32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)) && (BENCMAIN400_SortingHat_PrintOut_3_12_V_0>=32'sd1014
              ): (32'h35/*53:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h15/*21:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              )) || (32'h3a/*58:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (-32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_1
              >=BENCMAIN400_SortingHat_QsPartition_1_20_V_2) && (32'h21/*33:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) && (BENCMAIN400_SortingHat_QsPartition_1_20_V_0
              >=A_SINT_CC_SCALbx12_ARB0_rdata) || (32'h25/*37:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (BENCMAIN400_SortingHat_BubbleSort_2_2_V_2
              <32'sd1023) && (32'hf/*15:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h10/*16:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || (32'h1a/*26:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h20/*32:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || (32'h24/*36:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h2a/*42:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || ((32'h19/*25:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h39/*57:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              )) && (BENCMAIN400_SortingHat_Check_3_14_V_1<32'sd1023);

               A_SINT_CC_MAPR10NoCE0_ARA0_wen = (32'h40/*64:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'sd1+BENCMAIN400_SortingHat_QuickSort_1_1_V_4
              <BENCMAIN400_SortingHat_QuickSort_1_1_V_1) && (32'h31/*49:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h2c/*44:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (BENCMAIN400_SortingHat_QuickSort_1_1_V_0<-32'sd1+$signed(32'sd1+BENCMAIN400_SortingHat_QsPartition_1_20_V_1
              )) && (32'h2e/*46:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h33/*51:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              ) || (BENCMAIN400_SortingHat_DataGen_0_2_V_0>=32'sh400) && (32'h3f/*63:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz
              );

               A_SINT_CC_SCALbx12_ARB0_wen = (32'h2e/*46:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h22/*34:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h1e/*30:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h12/*18:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) && (SINTCCSCALbx12ARB0rdatah10hold<SINTCCSCALbx12ARB0rdatah12hold) || (32'hb/*11:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz) || (32'h11/*17:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) && (SINTCCSCALbx12ARB0rdatah10hold
              <A_SINT_CC_SCALbx12_ARB0_rdata) || (32'h26/*38:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz) || (32'h42/*66:kiwiBENCMAIN4001PC10nz*/==
              kiwiBENCMAIN4001PC10nz);

               end 
               hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       A_SINT_CC_SCALbx18_ARE0_wen = 32'd0;
       A_SINT_CC_SCALbx18_ARE0_wdata = 32'sd0;
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
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd10),
        .WORDS(32'sd1024),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_SCALbx12_ARB0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_SCALbx12_ARB0_rdata
),
        .addr(A_SINT_CC_SCALbx12_ARB0_addr),
        .wen(A_SINT_CC_SCALbx12_ARB0_wen),
        .ren(A_SINT_CC_SCALbx12_ARB0_ren),
        .wdata(A_SINT_CC_SCALbx12_ARB0_wdata
));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd10),
        .WORDS(32'sd1024),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_MAPR10NoCE0_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR10NoCE0_ARA0_rdata
),
        .addr(A_SINT_CC_MAPR10NoCE0_ARA0_addr),
        .wen(A_SINT_CC_MAPR10NoCE0_ARA0_wen),
        .ren(A_SINT_CC_MAPR10NoCE0_ARA0_ren
),
        .wdata(A_SINT_CC_MAPR10NoCE0_ARA0_wdata));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd4),
        .WORDS(32'sd10),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) A_SINT_CC_SCALbx18_ARE0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_SCALbx18_ARE0_rdata),
        .addr(A_SINT_CC_SCALbx18_ARE0_addr
),
        .wen(A_SINT_CC_SCALbx18_ARE0_wen),
        .ren(A_SINT_CC_SCALbx18_ARE0_ren),
        .wdata(A_SINT_CC_SCALbx18_ARE0_wdata
));

assign hprpin501343x10 = (((32'h1/*1:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah10hold)^((32'h2
/*2:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah12hold))+$signed(32'sh3
^((32'h2/*2:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah12hold));

assign hprpin501349x10 = $signed(hprpin501343x10+(((32'h1/*1:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah10hold
)^((32'h3/*3:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah14hold)))+(((32'h3
/*3:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah14hold)^((32'h4/*4:kiwiBENCMAIN4001PC10nz*/==
kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah16hold));

assign hprpin501355x10 = $signed(hprpin501349x10+(((32'h4/*4:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah16hold
)^((32'h5/*5:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah18hold)))+(((32'h5
/*5:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah18hold)^((32'h6/*6:kiwiBENCMAIN4001PC10nz*/==
kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah20hold));

assign hprpin501361x10 = $signed(hprpin501355x10+(((32'h6/*6:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah20hold
)^((32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah22hold)))+(((32'h7
/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah22hold)^((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==
kiwiBENCMAIN4001PC10nz)? A_SINT_CC_SCALbx18_ARE0_rdata: SINTCCSCALbx18ARE0rdatah24hold));

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 7
// 21 vectors of width 1
// 44 vectors of width 32
// 1 vectors of width 4
// 2 vectors of width 10
// 2 vectors of width 64
// Total state bits in module = 1588 bits.
// 288 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=3
// Total number of leaf cells = 3
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:50:10
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -obj-dir-name=. -cfg-plot-each-step -kiwic-kcode-dump=enable -log-dir-name=d_obj_loopclassic_b -kiwic-autodispose=enable loopclassic.exe -vnl=loopclassic -vnl-rootmodname=DUT -vnl-resets=synchronous -repack-loglevel=0


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
//KiwiC: front end input processing of class LoopClassic  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=LoopClassic..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=LoopClassic..cctor
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
//   srcfile=loopclassic.exe
//
//
//   kiwic-autodispose=enable
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
//PC codings points for kiwiBENCMAIN4001PC10 
//*-----------------------------------------------+------+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                                 | eno  | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------------------+------+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiBENCMAIN4001PC10"               | 1152 | 0       | hwm=0.9.0   | 9    |        | 1     | 9   | 10   |
//| XU32'1:"1:kiwiBENCMAIN4001PC10"               | 1151 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"               | 1149 | 11      | hwm=0.0.1   | 11   |        | 13    | 13  | 10   |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"               | 1150 | 11      | hwm=0.0.1   | 11   |        | 12    | 12  | 14   |
//| XU32'4:"4:kiwiBENCMAIN4001PC10"               | 1148 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 15   |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"               | 1145 | 15      | hwm=0.2.2   | 17   |        | 16    | 19  | 20   |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"               | 1146 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 14   |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"               | 1147 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 21   |
//| XU32'16:"16:kiwiBENCMAIN4001PC10"             | 1144 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 15   |
//| XU32'32:"32:kiwiBENCMAIN4001PC10"             | 1142 | 21      | hwm=0.1.0   | 23   |        | 23    | 23  | 24   |
//| XU32'32:"32:kiwiBENCMAIN4001PC10"             | 1143 | 21      | hwm=0.1.0   | 22   |        | 22    | 22  | 25   |
//| XU32'64:"64:kiwiBENCMAIN4001PC10"             | 1141 | 24      | hwm=0.0.0   | 24   |        | -     | -   | 21   |
//| XU32'128:"128:kiwiBENCMAIN4001PC10"           | 1139 | 25      | hwm=0.2.0   | 27   |        | 26    | 27  | 28   |
//| XU32'128:"128:kiwiBENCMAIN4001PC10"           | 1140 | 25      | hwm=0.0.0   | 25   |        | -     | -   | 63   |
//| XU32'256:"256:kiwiBENCMAIN4001PC10"           | 1138 | 28      | hwm=0.0.0   | 28   |        | -     | -   | 25   |
//| XU32'2048:"2048:kiwiBENCMAIN4001PC10"         | 1130 | 29      | hwm=0.0.0   | 29   |        | -     | -   | 32   |
//| XU32'4096:"4096:kiwiBENCMAIN4001PC10"         | 1129 | 30      | hwm=0.0.1   | 30   |        | 31    | 31  | 29   |
//| XU32'8192:"8192:kiwiBENCMAIN4001PC10"         | 1131 | 32      | hwm=1.4.1   | 38   |        | 36    | 39  | 46   |
//| XU32'8192:"8192:kiwiBENCMAIN4001PC10"         | 1132 | 32      | hwm=1.2.1   | 34   |        | 34    | 35  | 30   |
//| XU32'8192:"8192:kiwiBENCMAIN4001PC10"         | 1133 | 32      | hwm=1.1.0   | 33   |        | -     | -   | 29   |
//| XU32'1024:"1024:kiwiBENCMAIN4001PC10"         | 1134 | 40      | hwm=0.3.0   | 43   |        | 41    | 43  | 32   |
//| XU32'1024:"1024:kiwiBENCMAIN4001PC10"         | 1135 | 40      | hwm=0.0.0   | 40   |        | -     | -   | 53   |
//| XU32'32768:"32768:kiwiBENCMAIN4001PC10"       | 1124 | 44      | hwm=0.0.1   | 44   |        | 45    | 45  | 40   |
//| XU32'65536:"65536:kiwiBENCMAIN4001PC10"       | 1127 | 46      | hwm=0.0.1   | 46   |        | 48    | 48  | 51   |
//| XU32'65536:"65536:kiwiBENCMAIN4001PC10"       | 1128 | 46      | hwm=0.0.1   | 46   |        | 47    | 47  | 49   |
//| XU32'16384:"16384:kiwiBENCMAIN4001PC10"       | 1125 | 49      | hwm=0.0.1   | 49   |        | 50    | 50  | 44   |
//| XU32'16384:"16384:kiwiBENCMAIN4001PC10"       | 1126 | 49      | hwm=0.0.0   | 49   |        | -     | -   | 40   |
//| XU32'131072:"131072:kiwiBENCMAIN4001PC10"     | 1123 | 51      | hwm=0.0.1   | 51   |        | 52    | 52  | 49   |
//| XU32'262144:"262144:kiwiBENCMAIN4001PC10"     | 1121 | 53      | hwm=0.1.0   | 55   |        | 55    | 55  | 56   |
//| XU32'262144:"262144:kiwiBENCMAIN4001PC10"     | 1122 | 53      | hwm=0.1.0   | 54   |        | 54    | 54  | 57   |
//| XU32'524288:"524288:kiwiBENCMAIN4001PC10"     | 1120 | 56      | hwm=0.0.0   | 56   |        | -     | -   | 53   |
//| XU32'1048576:"1048576:kiwiBENCMAIN4001PC10"   | 1118 | 57      | hwm=0.2.0   | 59   |        | 58    | 59  | 60   |
//| XU32'1048576:"1048576:kiwiBENCMAIN4001PC10"   | 1119 | 57      | hwm=0.0.0   | 57   |        | -     | -   | 61   |
//| XU32'2097152:"2097152:kiwiBENCMAIN4001PC10"   | 1117 | 60      | hwm=0.0.0   | 60   |        | -     | -   | 57   |
//| XU32'4194304:"4194304:kiwiBENCMAIN4001PC10"   | 1116 | 61      | hwm=0.0.0   | 61   |        | -     | -   | 62   |
//| XU32'8388608:"8388608:kiwiBENCMAIN4001PC10"   | 1115 | 62      | hwm=0.0.0   | 62   |        | -     | -   | -    |
//| XU32'512:"512:kiwiBENCMAIN4001PC10"           | 1136 | 63      | hwm=0.0.2   | 63   |        | 64    | 65  | 40   |
//| XU32'512:"512:kiwiBENCMAIN4001PC10"           | 1137 | 63      | hwm=0.0.0   | 63   |        | -     | -   | 66   |
//| XU32'16777216:"16777216:kiwiBENCMAIN4001PC10" | 1114 | 66      | hwm=0.0.1   | 66   |        | 67    | 67  | 63   |
//*-----------------------------------------------+------+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//*------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc   | eno   | Phaser  | Work                                                                                                                  |
//*------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| F0   | -     | R0 CTRL |                                                                                                                       |
//| F0   | E1152 | R0 DATA | @_SINT/CC/SCALbx18_ARE0_read(2)                                                                                       |
//| F1   | E1152 | R1 DATA | @_SINT/CC/SCALbx18_ARE0_read(1)                                                                                       |
//| F2   | E1152 | R2 DATA | @_SINT/CC/SCALbx18_ARE0_read(3)                                                                                       |
//| F3   | E1152 | R3 DATA | @_SINT/CC/SCALbx18_ARE0_read(4)                                                                                       |
//| F4   | E1152 | R4 DATA | @_SINT/CC/SCALbx18_ARE0_read(5)                                                                                       |
//| F5   | E1152 | R5 DATA | @_SINT/CC/SCALbx18_ARE0_read(6)                                                                                       |
//| F6   | E1152 | R6 DATA | @_SINT/CC/SCALbx18_ARE0_read(7)                                                                                       |
//| F7   | E1152 | R7 DATA | @_SINT/CC/SCALbx18_ARE0_read(8)                                                                                       |
//| F8   | E1152 | R8 DATA | @_SINT/CC/SCALbx18_ARE0_read(9)                                                                                       |
//| F9   | E1152 | R9 DATA |                                                                                                                       |
//| F9+E | E1152 | W0 DATA | @_SINT/CC/SCALbx16_seed10write(S32'-1432829666) BENCMAIN400.SortingHat.DataGen.0.2.V_0write(S32'0) BENCMAIN400.bench\ |
//|      |       |         | .SortingTest.0.61.V_0write(U64'0) BENCMAIN400.SortingHat.QuickSort.1.1.V_2write(S32'0) BENCMAIN400.SortingHat.QuickS\ |
//|      |       |         | ort.1.1.V_0write(S32'0) BENCMAIN400.SortingHat.QuickSort.1.1.V_1write(S32'0) BENCMAIN400.SortingHat.QsPartition.1.20\ |
//|      |       |         | .V_0write(S32'0) BENCMAIN400.SortingHat.QsPartition.1.20.V_2write(S32'0) BENCMAIN400.SortingHat.QsPartition.1.20.V_1\ |
//|      |       |         | write(S32'0) BENCMAIN400.SortingHat.Swap.2.13.V_0write(S32'0) BENCMAIN400.SortingHat.Swap.5.11.V_0write(S32'0) BENCM\ |
//|      |       |         | AIN400.SortingHat.QuickSort.1.1.V_4write(S32'0) BENCMAIN400.SortingHat.BubbleSort.2.2.V_0write(S32'0) BENCMAIN400.So\ |
//|      |       |         | rtingHat.BubbleSort.2.2.V_2write(S32'0) BENCMAIN400.SortingHat.BubbleSort.2.2.V_1write(U32'0) BENCMAIN400.SortingHat\ |
//|      |       |         | .PrintOut.3.12.V_0write(S32'0) BENCMAIN400.SortingHat.Check.3.14.V_1write(S32'0) BENCMAIN400.SortingHat.Check.3.14.V\ |
//|      |       |         | _0write(S32'0) BENCMAIN400.bench.SortingTest.0.67.V_0write(U64'0) fastspilldup50write(S32'0) fastspilldup54write(S32\ |
//|      |       |         | '0) @_SINT/CC/SCALbx22_swaps10write(S32'0)  W/P:START-SORT0  PLI:   ASSOCRED jojo=%d ...  W/P:START-ASSOCRED  PLI:  \ |
//|      |       |         |  LOOPFWD jojo=%d  ...  W/P:START-LOOPFWD  PLI:   DATADEP jojo=%d  ...  W/P:START-DATADEP  PLI:   LCD jojo=%d  test..\ |
//|      |       |         | .  W/P:START-LOOP-CARRIED  W/P:START  PLI:LoopClassic Demo Sta...                                                     |
//*------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+------*
//| pc    | eno   | Phaser  | Work |
//*-------+-------+---------+------*
//| F10   | -     | R0 CTRL |      |
//| F10   | E1151 | R0 DATA |      |
//| F10+E | E1151 | W0 DATA |      |
//*-------+-------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                   |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*
//| F11   | -     | R0 CTRL |                                                                                                                        |
//| F11   | E1150 | R0 DATA |                                                                                                                        |
//| F11+E | E1150 | W0 DATA | BENCMAIN400.SortingHat.DataGen.0.2.V_0write(E1) BENCMAIN400.bench.SortingTest.0.61.V_0write(C64u(hpr_tnow)) BENCMAIN4\ |
//|       |       |         | 00.SortingHat.BubbleSort.2.2.V_0write(S32'0) @_SINT/CC/SCALbx22_swaps10write(S32'0) @_SINT/CC/SCALbx12_ARB0_write(E2,\ |
//|       |       |         |  E3)  PLI:Sort arity=%d start ...                                                                                      |
//| F12   | E1150 | W1 DATA |                                                                                                                        |
//| F11   | E1149 | R0 DATA |                                                                                                                        |
//| F11+E | E1149 | W0 DATA | @_SINT/CC/SCALbx16_seed10write(E4) BENCMAIN400.SortingHat.DataGen.0.2.V_0write(E1) @_SINT/CC/SCALbx12_ARB0_write(E2, \ |
//|       |       |         | E3)                                                                                                                    |
//| F13   | E1149 | W1 DATA |                                                                                                                        |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                                           |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F14   | -     | R0 CTRL |                                                                                                                                                                |
//| F14   | E1148 | R0 DATA |                                                                                                                                                                |
//| F14+E | E1148 | W0 DATA | BENCMAIN400.SortingHat.BubbleSort.2.2.V_0write(E5) BENCMAIN400.SortingHat.BubbleSort.2.2.V_2write(S32'0) BENCMAIN400.SortingHat.BubbleSort.2.2.V_1write(U32'0) |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                            |
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F15   | -     | R0 CTRL |                                                                                                                                 |
//| F15   | E1147 | R0 DATA |                                                                                                                                 |
//| F15+E | E1147 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'0)  PLI:Sort arity=%d quick=...  PLI:BubbleSort finished ...                  |
//| F15   | E1146 | R0 DATA |                                                                                                                                 |
//| F15+E | E1146 | W0 DATA |                                                                                                                                 |
//| F15   | E1145 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E6)                                                                                                |
//| F16   | E1145 | R1 DATA | @_SINT/CC/SCALbx12_ARB0_read(E7)                                                                                                |
//| F17   | E1145 | R2 DATA |                                                                                                                                 |
//| F17+E | E1145 | W0 DATA | BENCMAIN400.SortingHat.BubbleSort.2.2.V_1write(U32'1) @_SINT/CC/SCALbx22_swaps10write(E8) @_SINT/CC/SCALbx12_ARB0_write(E6, E9) |
//| F18   | E1145 | W1 DATA | @_SINT/CC/SCALbx12_ARB0_write(E7, E10)                                                                                          |
//| F19   | E1145 | W2 DATA |                                                                                                                                 |
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-----------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                |
//*-------+-------+---------+-----------------------------------------------------*
//| F20   | -     | R0 CTRL |                                                     |
//| F20   | E1144 | R0 DATA |                                                     |
//| F20+E | E1144 | W0 DATA | BENCMAIN400.SortingHat.BubbleSort.2.2.V_2write(E11) |
//*-------+-------+---------+-----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32:"32:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32:"32:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                   |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| F21   | -     | R0 CTRL |                                                                                                                                        |
//| F21   | E1143 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E12)                                                                                                      |
//| F22   | E1143 | R1 DATA |                                                                                                                                        |
//| F22+E | E1143 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'1014) BENCMAIN400.SortingHat.Check.3.14.V_1write(S32'0) BENCMAIN400.SortingHat.Chec\ |
//|       |       |         | k.3.14.V_0write(S32'0)  PLI:   ... snip ...  PLI:     Sorted %d is %d                                                                  |
//| F21   | E1142 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E12)                                                                                                      |
//| F23   | E1142 | R1 DATA |                                                                                                                                        |
//| F23+E | E1142 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'1014) BENCMAIN400.SortingHat.Check.3.14.V_1write(S32'0) BENCMAIN400.SortingHat.Chec\ |
//|       |       |         | k.3.14.V_0write(S32'0)  PLI:   ... snip ...  PLI:     Sorted %d is %d                                                                  |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'64:"64:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'64:"64:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                               |
//*-------+-------+---------+----------------------------------------------------*
//| F24   | -     | R0 CTRL |                                                    |
//| F24   | E1141 | R0 DATA |                                                    |
//| F24+E | E1141 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(E13) |
//*-------+-------+---------+----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'128:"128:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'128:"128:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                          |
//*-------+-------+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| F25   | -     | R0 CTRL |                                                                                                                               |
//| F25   | E1140 | R0 DATA |                                                                                                                               |
//| F25+E | E1140 | W0 DATA | BENCMAIN400.SortingHat.DataGen.0.2.V_0write(S32'0)  W/P:START-SORT1  PLI:Sort finished and ch...  PLI:SortingHat Checker: ... |
//| F25   | E1139 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E14)                                                                                             |
//| F26   | E1139 | R1 DATA | @_SINT/CC/SCALbx12_ARB0_read(E15)                                                                                             |
//| F27   | E1139 | R2 DATA |                                                                                                                               |
//| F27+E | E1139 | W0 DATA | BENCMAIN400.SortingHat.Check.3.14.V_0write(E16)                                                                               |
//*-------+-------+---------+-------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'256:"256:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'256:"256:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                            |
//*-------+-------+---------+-------------------------------------------------*
//| F28   | -     | R0 CTRL |                                                 |
//| F28   | E1138 | R0 DATA |                                                 |
//| F28+E | E1138 | W0 DATA | BENCMAIN400.SortingHat.Check.3.14.V_1write(E17) |
//*-------+-------+---------+-------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2048:"2048:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2048:"2048:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                  |
//*-------+-------+---------+-------------------------------------------------------*
//| F29   | -     | R0 CTRL |                                                       |
//| F29   | E1130 | R0 DATA |                                                       |
//| F29+E | E1130 | W0 DATA | BENCMAIN400.SortingHat.QsPartition.1.20.V_2write(E18) |
//*-------+-------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4096:"4096:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4096:"4096:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-----------------------------------------*
//| pc    | eno   | Phaser  | Work                                    |
//*-------+-------+---------+-----------------------------------------*
//| F30   | -     | R0 CTRL |                                         |
//| F30   | E1129 | R0 DATA |                                         |
//| F30+E | E1129 | W0 DATA | @_SINT/CC/SCALbx12_ARB0_write(E19, E20) |
//| F31   | E1129 | W1 DATA |                                         |
//*-------+-------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8192:"8192:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8192:"8192:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                  |
//*-------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| F32   | -     | R0 CTRL | @_SINT/CC/SCALbx12_ARB0_read(E19)                                                                                     |
//| F33   | -     | R1 CTRL |                                                                                                                       |
//| F32   | E1133 | R0 DATA |                                                                                                                       |
//| F33   | E1133 | R1 DATA |                                                                                                                       |
//| F33+E | E1133 | W0 DATA |                                                                                                                       |
//| F32   | E1132 | R0 DATA |                                                                                                                       |
//| F33   | E1132 | R1 DATA | @_SINT/CC/SCALbx12_ARB0_read(E21)                                                                                     |
//| F34   | E1132 | R2 DATA |                                                                                                                       |
//| F34+E | E1132 | W0 DATA | BENCMAIN400.SortingHat.QsPartition.1.20.V_1write(E21) BENCMAIN400.SortingHat.Swap.2.13.V_0write(E22) @_SINT/CC/SCALb\ |
//|       |       |         | x22_swaps10write(E8) @_SINT/CC/SCALbx12_ARB0_write(E21, E23)                                                          |
//| F35   | E1132 | W1 DATA |                                                                                                                       |
//| F32   | E1131 | R0 DATA |                                                                                                                       |
//| F33   | E1131 | R1 DATA |                                                                                                                       |
//| F36   | E1131 | R2 DATA | @_SINT/CC/SCALbx12_ARB0_read(E24)                                                                                     |
//| F37   | E1131 | R3 DATA | @_SINT/CC/SCALbx12_ARB0_read(E25)                                                                                     |
//| F38   | E1131 | R4 DATA |                                                                                                                       |
//| F38+E | E1131 | W0 DATA | BENCMAIN400.SortingHat.Swap.5.11.V_0write(E26) @_SINT/CC/SCALbx22_swaps10write(E8) @_SINT/CC/SCALbx12_ARB0_write(E24\ |
//|       |       |         | , E27)                                                                                                                |
//| F39   | E1131 | W1 DATA |                                                                                                                       |
//*-------+-------+---------+-----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1024:"1024:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1024:"1024:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                        |
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| F40   | -     | R0 CTRL |                                                                                                                                             |
//| F40   | E1135 | R0 DATA |                                                                                                                                             |
//| F40+E | E1135 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'0)  PLI:Sort arity=%d quick=...  PLI:QuickSort finished a...                              |
//| F40   | E1134 | R0 DATA | @_SINT/CC/MAPR10NoCE0_ARA0_read(E28)                                                                                                        |
//| F41   | E1134 | R1 DATA | @_SINT/CC/MAPR10NoCE0_ARA0_read(E29)                                                                                                        |
//| F42   | E1134 | R2 DATA | @_SINT/CC/SCALbx12_ARB0_read(E30)                                                                                                           |
//| F43   | E1134 | R3 DATA |                                                                                                                                             |
//| F43+E | E1134 | W0 DATA | BENCMAIN400.SortingHat.QuickSort.1.1.V_2write(E31) BENCMAIN400.SortingHat.QuickSort.1.1.V_0write(E32) BENCMAIN400.SortingHat.QuickSort.1.1\ |
//|       |       |         | .V_1write(E30) BENCMAIN400.SortingHat.QsPartition.1.20.V_0write(E33) BENCMAIN400.SortingHat.QsPartition.1.20.V_2write(E32) BENCMAIN400.Sor\ |
//|       |       |         | tingHat.QsPartition.1.20.V_1write(E34)                                                                                                      |
//*-------+-------+---------+---------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32768:"32768:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32768:"32768:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                  |
//*-------+-------+---------+-------------------------------------------------------*
//| F44   | -     | R0 CTRL |                                                       |
//| F44   | E1124 | R0 DATA |                                                       |
//| F44+E | E1124 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARA0_write(fastspilldup54, E35) |
//| F45   | E1124 | W1 DATA |                                                       |
//*-------+-------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'65536:"65536:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'65536:"65536:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                                           |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F46   | -     | R0 CTRL |                                                                                                                                                                |
//| F46   | E1128 | R0 DATA |                                                                                                                                                                |
//| F46+E | E1128 | W0 DATA | BENCMAIN400.SortingHat.QuickSort.1.1.V_4write(E21) @_SINT/CC/SCALbx12_ARB0_write(E25, E36)  PLI:QsPartition left=%d ...                                        |
//| F47   | E1128 | W1 DATA |                                                                                                                                                                |
//| F46   | E1127 | R0 DATA |                                                                                                                                                                |
//| F46+E | E1127 | W0 DATA | BENCMAIN400.SortingHat.QuickSort.1.1.V_2write(E37) BENCMAIN400.SortingHat.QuickSort.1.1.V_4write(E21) fastspilldup50write(E37) @_SINT/CC/SCALbx12_ARB0_write(\ |
//|       |       |         | E25, E36) @_SINT/CC/MAPR10NoCE0_ARA0_write(E38, E39)  PLI:QsPartition left=%d ...                                                                              |
//| F48   | E1127 | W1 DATA |                                                                                                                                                                |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16384:"16384:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16384:"16384:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                   |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*
//| F49   | -     | R0 CTRL |                                                                                                                        |
//| F49   | E1126 | R0 DATA |                                                                                                                        |
//| F49+E | E1126 | W0 DATA |                                                                                                                        |
//| F49   | E1125 | R0 DATA |                                                                                                                        |
//| F49+E | E1125 | W0 DATA | BENCMAIN400.SortingHat.QuickSort.1.1.V_2write(E37) fastspilldup54write(E37) @_SINT/CC/MAPR10NoCE0_ARA0_write(E38, E40) |
//| F50   | E1125 | W1 DATA |                                                                                                                        |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'131072:"131072:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'131072:"131072:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                  |
//*-------+-------+---------+-------------------------------------------------------*
//| F51   | -     | R0 CTRL |                                                       |
//| F51   | E1123 | R0 DATA |                                                       |
//| F51+E | E1123 | W0 DATA | @_SINT/CC/MAPR10NoCE0_ARA0_write(fastspilldup50, E41) |
//| F52   | E1123 | W1 DATA |                                                       |
//*-------+-------+---------+-------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'262144:"262144:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'262144:"262144:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                   |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| F53   | -     | R0 CTRL |                                                                                                                                        |
//| F53   | E1122 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E12)                                                                                                      |
//| F54   | E1122 | R1 DATA |                                                                                                                                        |
//| F54+E | E1122 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'1014) BENCMAIN400.SortingHat.Check.3.14.V_1write(S32'0) BENCMAIN400.SortingHat.Chec\ |
//|       |       |         | k.3.14.V_0write(S32'0)  PLI:   ... snip ...  PLI:     Sorted %d is %d                                                                  |
//| F53   | E1121 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E12)                                                                                                      |
//| F55   | E1121 | R1 DATA |                                                                                                                                        |
//| F55+E | E1121 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(S32'1014) BENCMAIN400.SortingHat.Check.3.14.V_1write(S32'0) BENCMAIN400.SortingHat.Chec\ |
//|       |       |         | k.3.14.V_0write(S32'0)  PLI:   ... snip ...  PLI:     Sorted %d is %d                                                                  |
//*-------+-------+---------+----------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'524288:"524288:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'524288:"524288:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+----------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                               |
//*-------+-------+---------+----------------------------------------------------*
//| F56   | -     | R0 CTRL |                                                    |
//| F56   | E1120 | R0 DATA |                                                    |
//| F56+E | E1120 | W0 DATA | BENCMAIN400.SortingHat.PrintOut.3.12.V_0write(E13) |
//*-------+-------+---------+----------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1048576:"1048576:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1048576:"1048576:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                     |
//*-------+-------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| F57   | -     | R0 CTRL |                                                                                                                          |
//| F57   | E1119 | R0 DATA |                                                                                                                          |
//| F57+E | E1119 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI: Test LoopClassic fi...  W/P:FINISH  PLI:Sort finished and ch...  PLI:SortingHat Checker: ... |
//| F57   | E1118 | R0 DATA | @_SINT/CC/SCALbx12_ARB0_read(E14)                                                                                        |
//| F58   | E1118 | R1 DATA | @_SINT/CC/SCALbx12_ARB0_read(E15)                                                                                        |
//| F59   | E1118 | R2 DATA |                                                                                                                          |
//| F59+E | E1118 | W0 DATA | BENCMAIN400.SortingHat.Check.3.14.V_0write(E16)                                                                          |
//*-------+-------+---------+--------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2097152:"2097152:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2097152:"2097152:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                            |
//*-------+-------+---------+-------------------------------------------------*
//| F60   | -     | R0 CTRL |                                                 |
//| F60   | E1117 | R0 DATA |                                                 |
//| F60+E | E1117 | W0 DATA | BENCMAIN400.SortingHat.Check.3.14.V_1write(E17) |
//*-------+-------+---------+-------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4194304:"4194304:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4194304:"4194304:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+------*
//| pc    | eno   | Phaser  | Work |
//*-------+-------+---------+------*
//| F61   | -     | R0 CTRL |      |
//| F61   | E1116 | R0 DATA |      |
//| F61+E | E1116 | W0 DATA |      |
//*-------+-------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8388608:"8388608:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8388608:"8388608:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+-----------------------*
//| pc    | eno   | Phaser  | Work                  |
//*-------+-------+---------+-----------------------*
//| F62   | -     | R0 CTRL |                       |
//| F62   | E1115 | R0 DATA |                       |
//| F62+E | E1115 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+-------+---------+-----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'512:"512:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'512:"512:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                                                                                 |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F63   | -     | R0 CTRL |                                                                                                                                                      |
//| F63   | E1137 | R0 DATA |                                                                                                                                                      |
//| F63+E | E1137 | W0 DATA | @_SINT/CC/SCALbx16_seed10write(E4)                                                                                                                   |
//| F63   | E1136 | R0 DATA |                                                                                                                                                      |
//| F63+E | E1136 | W0 DATA | BENCMAIN400.SortingHat.QuickSort.1.1.V_2write(S32'1) BENCMAIN400.SortingHat.QuickSort.1.1.V_0write(S32'0) BENCMAIN400.SortingHat.QuickSort.1.1.V_1w\ |
//|       |       |         | rite(S32'1023) BENCMAIN400.bench.SortingTest.0.67.V_0write(C64u(hpr_tnow)) @_SINT/CC/SCALbx22_swaps10write(S32'0) @_SINT/CC/MAPR10NoCE0_ARA0_write(\ |
//|       |       |         | 1, S32'1023)  PLI:Sort arity=%d start ...                                                                                                            |
//| F64   | E1136 | W1 DATA | @_SINT/CC/MAPR10NoCE0_ARA0_write(0, S32'0)                                                                                                           |
//| F65   | E1136 | W2 DATA |                                                                                                                                                      |
//*-------+-------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16777216:"16777216:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16777216:"16777216:kiwiBENCMAIN4001PC10"
//*-------+-------+---------+---------------------------------------------------------------------------------------*
//| pc    | eno   | Phaser  | Work                                                                                  |
//*-------+-------+---------+---------------------------------------------------------------------------------------*
//| F66   | -     | R0 CTRL |                                                                                       |
//| F66   | E1114 | R0 DATA |                                                                                       |
//| F66+E | E1114 | W0 DATA | BENCMAIN400.SortingHat.DataGen.0.2.V_0write(E1) @_SINT/CC/SCALbx12_ARB0_write(E2, E3) |
//| F67   | E1114 | W1 DATA |                                                                                       |
//*-------+-------+---------+---------------------------------------------------------------------------------------*

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
//  E1 =.= C(1+BENCMAIN400.SortingHat.DataGen.0.2.V_0)
//
//
//  E2 =.= BENCMAIN400.SortingHat.DataGen.0.2.V_0
//
//
//  E3 =.= C(@_SINT/CC/SCALbx16_seed10)
//
//
//  E4 =.= C(S32'715136305+S32'2147001325*@_SINT/CC/SCALbx16_seed10)
//
//
//  E5 =.= C(1+BENCMAIN400.SortingHat.BubbleSort.2.2.V_0)
//
//
//  E6 =.= 1+BENCMAIN400.SortingHat.BubbleSort.2.2.V_2
//
//
//  E7 =.= BENCMAIN400.SortingHat.BubbleSort.2.2.V_2
//
//
//  E8 =.= C(1+@_SINT/CC/SCALbx22_swaps10)
//
//
//  E9 =.= C(@_SINT/CC/SCALbx12_ARB0[BENCMAIN400.SortingHat.BubbleSort.2.2.V_2])
//
//
//  E10 =.= C(@_SINT/CC/SCALbx12_ARB0[1+BENCMAIN400.SortingHat.BubbleSort.2.2.V_2])
//
//
//  E11 =.= C(1+BENCMAIN400.SortingHat.BubbleSort.2.2.V_2)
//
//
//  E12 =.= BENCMAIN400.SortingHat.PrintOut.3.12.V_0
//
//
//  E13 =.= C(1+BENCMAIN400.SortingHat.PrintOut.3.12.V_0)
//
//
//  E14 =.= 1+BENCMAIN400.SortingHat.Check.3.14.V_1
//
//
//  E15 =.= BENCMAIN400.SortingHat.Check.3.14.V_1
//
//
//  E16 =.= C(1+BENCMAIN400.SortingHat.Check.3.14.V_0)
//
//
//  E17 =.= C(1+BENCMAIN400.SortingHat.Check.3.14.V_1)
//
//
//  E18 =.= C(1+BENCMAIN400.SortingHat.QsPartition.1.20.V_2)
//
//
//  E19 =.= BENCMAIN400.SortingHat.QsPartition.1.20.V_2
//
//
//  E20 =.= C(BENCMAIN400.SortingHat.Swap.2.13.V_0)
//
//
//  E21 =.= C(1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1)
//
//
//  E22 =.= C(@_SINT/CC/SCALbx12_ARB0[C(1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1)])
//
//
//  E23 =.= C(@_SINT/CC/SCALbx12_ARB0[BENCMAIN400.SortingHat.QsPartition.1.20.V_2])
//
//
//  E24 =.= 1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1
//
//
//  E25 =.= BENCMAIN400.SortingHat.QuickSort.1.1.V_1
//
//
//  E26 =.= C(@_SINT/CC/SCALbx12_ARB0[1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1])
//
//
//  E27 =.= C(@_SINT/CC/SCALbx12_ARB0[BENCMAIN400.SortingHat.QuickSort.1.1.V_1])
//
//
//  E28 =.= C(-1+(C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)))
//
//
//  E29 =.= C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)
//
//
//  E30 =.= C(@_SINT/CC/MAPR10NoCE0_ARA0[C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)])
//
//
//  E31 =.= C(-1+(C(-1+(C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)))))
//
//
//  E32 =.= C(@_SINT/CC/MAPR10NoCE0_ARA0[C(-1+(C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)))])
//
//
//  E33 =.= C(@_SINT/CC/SCALbx12_ARB0[C(@_SINT/CC/MAPR10NoCE0_ARA0[C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)])])
//
//
//  E34 =.= C(-1+(C(@_SINT/CC/MAPR10NoCE0_ARA0[C(-1+(C(BENCMAIN400.SortingHat.QuickSort.1.1.V_2)))])))
//
//
//  E35 =.= C(BENCMAIN400.SortingHat.QuickSort.1.1.V_1)
//
//
//  E36 =.= C(BENCMAIN400.SortingHat.Swap.5.11.V_0)
//
//
//  E37 =.= C(1+(C(1+BENCMAIN400.SortingHat.QuickSort.1.1.V_2)))
//
//
//  E38 =.= C(1+BENCMAIN400.SortingHat.QuickSort.1.1.V_2)
//
//
//  E39 =.= C(BENCMAIN400.SortingHat.QuickSort.1.1.V_0)
//
//
//  E40 =.= C(1+BENCMAIN400.SortingHat.QuickSort.1.1.V_4)
//
//
//  E41 =.= C(-1+BENCMAIN400.SortingHat.QuickSort.1.1.V_4)
//
//
//  E42 =.= (C(1+BENCMAIN400.SortingHat.DataGen.0.2.V_0))>=S32'1024
//
//
//  E43 =.= (C(1+BENCMAIN400.SortingHat.DataGen.0.2.V_0))<S32'1024
//
//
//  E44 =.= {[BENCMAIN400.SortingHat.BubbleSort.2.2.V_2>=1023, |BENCMAIN400.SortingHat.BubbleSort.2.2.V_1|]}
//
//
//  E45 =.= {[BENCMAIN400.SortingHat.BubbleSort.2.2.V_2>=1023, |BENCMAIN400.SortingHat.BubbleSort.2.2.V_1|]}
//
//
//  E46 =.= BENCMAIN400.SortingHat.BubbleSort.2.2.V_2<1023
//
//
//  E47 =.= {[BENCMAIN400.SortingHat.PrintOut.3.12.V_0>=S32'1024]}
//
//
//  E48 =.= {[10<BENCMAIN400.SortingHat.PrintOut.3.12.V_0, BENCMAIN400.SortingHat.PrintOut.3.12.V_0<1014]; [BENCMAIN400.SortingHat.PrintOut.3.12.V_0<S32'1024, BENCMAIN400.SortingHat.PrintOut.3.12.V_0>=1014]; [10>=BENCMAIN400.SortingHat.PrintOut.3.12.V_0]}
//
//
//  E49 =.= BENCMAIN400.SortingHat.Check.3.14.V_1>=1023
//
//
//  E50 =.= BENCMAIN400.SortingHat.Check.3.14.V_1<1023
//
//
//  E51 =.= {[-1+BENCMAIN400.SortingHat.QuickSort.1.1.V_1>=BENCMAIN400.SortingHat.QsPartition.1.20.V_2, XU32'33:"33:kiwiBENCMAIN4001PC10nz"==kiwiBENCMAIN4001PC10nz, BENCMAIN400.SortingHat.QsPartition.1.20.V_0<@_SINT/CC/SCALbx12_ARB0_rdata]; [-1+BENCMAIN400.SortingHat.QuickSort.1.1.V_1>=BENCMAIN400.SortingHat.QsPartition.1.20.V_2, XU32'33:"33:kiwiBENCMAIN4001PC10nz"!=kiwiBENCMAIN4001PC10nz, BENCMAIN400.SortingHat.QsPartition.1.20.V_0<SINTCCSCALbx12ARB0rdatah12hold]}
//
//
//  E52 =.= {[-1+BENCMAIN400.SortingHat.QuickSort.1.1.V_1>=BENCMAIN400.SortingHat.QsPartition.1.20.V_2, XU32'33:"33:kiwiBENCMAIN4001PC10nz"==kiwiBENCMAIN4001PC10nz, BENCMAIN400.SortingHat.QsPartition.1.20.V_0>=@_SINT/CC/SCALbx12_ARB0_rdata]; [-1+BENCMAIN400.SortingHat.QuickSort.1.1.V_1>=BENCMAIN400.SortingHat.QsPartition.1.20.V_2, XU32'33:"33:kiwiBENCMAIN4001PC10nz"!=kiwiBENCMAIN4001PC10nz, BENCMAIN400.SortingHat.QsPartition.1.20.V_0>=SINTCCSCALbx12ARB0rdatah12hold]}
//
//
//  E53 =.= -1+BENCMAIN400.SortingHat.QuickSort.1.1.V_1<BENCMAIN400.SortingHat.QsPartition.1.20.V_2
//
//
//  E54 =.= BENCMAIN400.SortingHat.QuickSort.1.1.V_2<0
//
//
//  E55 =.= BENCMAIN400.SortingHat.QuickSort.1.1.V_2>=0
//
//
//  E56 =.= BENCMAIN400.SortingHat.QuickSort.1.1.V_0>=-1+(C(1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1))
//
//
//  E57 =.= BENCMAIN400.SortingHat.QuickSort.1.1.V_0<-1+(C(1+BENCMAIN400.SortingHat.QsPartition.1.20.V_1))
//
//
//  E58 =.= 1+BENCMAIN400.SortingHat.QuickSort.1.1.V_4>=BENCMAIN400.SortingHat.QuickSort.1.1.V_1
//
//
//  E59 =.= 1+BENCMAIN400.SortingHat.QuickSort.1.1.V_4<BENCMAIN400.SortingHat.QuickSort.1.1.V_1
//
//
//  E60 =.= BENCMAIN400.SortingHat.DataGen.0.2.V_0<S32'1024
//
//
//  E61 =.= BENCMAIN400.SortingHat.DataGen.0.2.V_0>=S32'1024
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for loopclassic to loopclassic

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 7
//
//21 vectors of width 1
//
//44 vectors of width 32
//
//1 vectors of width 4
//
//2 vectors of width 10
//
//2 vectors of width 64
//
//Total state bits in module = 1588 bits.
//
//288 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread LoopClassic..cctor uid=cctor10 has 5 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread bench..cctor uid=cctor12 has 12 CIL instructions in 1 basic blocks
//Thread bench.Main uid=Main10 has 396 CIL instructions in 96 basic blocks
//Thread mpc10 has 26 bevelab control states (pauses)
//Reindexed thread kiwiBENCMAIN4001PC10 with 68 minor control states
// eof (HPR L/S Verilog)
