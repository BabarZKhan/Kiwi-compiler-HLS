

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:32
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test50.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets12 */
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX22,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTESTMAIN4001PC10nz_pc_export,
    output reg [4:0] kiwiTSX480PC10nz_pc_export);
function [7:0] hpr_toChar0;
input [31:0] hpr_toChar0_a;
   hpr_toChar0 = hpr_toChar0_a & 8'hff;
endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets12
  reg hpr_int_run_enable_DDX18;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX22;
// abstractionName=kiwicmainnets10
  reg signed [31:0] _W0_TSX480_test50_secondProcess_V_2;
  reg signed [31:0] _W0_TSX480_test50_secondProcess_V_1;
  reg signed [31:0] _W0_TSX480_test50_secondProcess_V_0;
  reg signed [31:0] TESTMAIN400_test50_clearto_25_3_V_1;
  reg signed [31:0] TESTMAIN400_test50_clearto_25_3_V_0;
  reg signed [31:0] TESTMAIN400_test50_clearto_0_10_V_1;
  reg signed [31:0] TESTMAIN400_test50_clearto_0_10_V_0;
  reg signed [31:0] TESTMAIN400_test50_test50_phase0_0_12_V_0;
  reg signed [15:0] test50_command2;
  reg signed [31:0] test50_sum;
  reg test50_exiting;
  reg test50_sharedData;
// abstractionName=res2-contacts pi_name=CV_2P_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR14NoCE0_ARA0_rdata0;
  reg [4:0] A_SINT_CC_MAPR14NoCE0_ARA0_addr0;
  reg A_SINT_CC_MAPR14NoCE0_ARA0_wen0;
  reg A_SINT_CC_MAPR14NoCE0_ARA0_ren0;
  reg signed [31:0] A_SINT_CC_MAPR14NoCE0_ARA0_wdata0;
  wire signed [31:0] A_SINT_CC_MAPR14NoCE0_ARA0_rdata1;
  reg [4:0] A_SINT_CC_MAPR14NoCE0_ARA0_addr1;
  reg A_SINT_CC_MAPR14NoCE0_ARA0_wen1;
  reg A_SINT_CC_MAPR14NoCE0_ARA0_ren1;
  reg signed [31:0] A_SINT_CC_MAPR14NoCE0_ARA0_wdata1;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_MAPR14NoCE1_ARA0_rdata;
  reg [4:0] A_SINT_CC_MAPR14NoCE1_ARA0_addr;
  reg A_SINT_CC_MAPR14NoCE1_ARA0_wen;
  reg A_SINT_CC_MAPR14NoCE1_ARA0_ren;
  reg signed [31:0] A_SINT_CC_MAPR14NoCE1_ARA0_wdata;
// abstractionName=res2-morenets
  reg signed [31:0] SINTCCMAPR14NoCE1ARA0rdatah10hold;
  reg SINTCCMAPR14NoCE1ARA0rdatah10shot0;
  reg signed [31:0] SINTCCMAPR14NoCE0ARA01rdatah10hold;
  reg SINTCCMAPR14NoCE0ARA01rdatah10shot0;
  reg [3:0] kiwiTSX480PC10nz;
  reg [4:0] kiwiTESTMAIN4001PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin500842x10;
  wire signed [31:0] hprpin500848x10;
  wire signed [31:0] hprpin500852x10;
  wire signed [31:0] hprpin500858x10;
  wire signed [31:0] hprpin500864x10;
  wire signed [31:0] hprpin500879x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TSX480/1.0
      if (reset)  begin 
               test50_sum <= 32'd0;
               test50_command2 <= 32'd0;
               kiwiTSX480PC10nz <= 32'd0;
               _W0_TSX480_test50_secondProcess_V_2 <= 32'd0;
               _W0_TSX480_test50_secondProcess_V_1 <= 32'd0;
               _W0_TSX480_test50_secondProcess_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiTSX480PC10nz)
                  32'h1/*1:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (!test50_exiting && (32'sd68==test50_command2)) $display("sp: Print data: sharedData[%1d] = %1d", 32'sh0
                              , hprpin500842x10);
                               kiwiTSX480PC10nz <= 32'h2/*2:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                           _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                           _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          
                  32'h0/*0:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (test50_exiting) $finish(32'sd0);
                              if (!test50_exiting && (32'sd80==test50_command2)) $display("sp: data sum %1d", test50_sum);
                              if (test50_exiting)  begin 
                                   kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                  if (!test50_exiting && (32'sd80==test50_command2))  begin 
                                   kiwiTSX480PC10nz <= 32'he/*14:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   end 
                                  if (!test50_exiting && (32'sd83==test50_command2))  begin 
                                   kiwiTSX480PC10nz <= 32'h4/*4:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   test50_sum <= 32'sh0;
                                   end 
                                  if ((32'sd73!=test50_command2) && !test50_exiting && (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                          ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2))  begin 
                                   kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   test50_command2 <= 32'sh49;
                                   end 
                                  if (!test50_exiting && (32'sd85==test50_command2))  begin 
                                   kiwiTSX480PC10nz <= 32'h8/*8:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   end 
                                  if ((32'sd73==test50_command2) && !test50_exiting)  begin 
                                   kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   end 
                                  if (!test50_exiting && (32'sd68==test50_command2))  kiwiTSX480PC10nz <= 32'h1/*1:kiwiTSX480PC10nz*/;
                               end 
                          
                  32'h3/*3:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)<32'sd30)) $display("sp: Print data: sharedData[%1d] = %1d"
                              , $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2), hprpin500848x10);
                               kiwiTSX480PC10nz <= 32'h2/*2:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_2 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2);
                           end 
                          
                  32'h5/*5:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiTSX480PC10nz <= 32'h4/*4:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_1 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1);
                           test50_sum <= $signed(test50_sum+hprpin500864x10);
                           end 
                          
                  32'h6/*6:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)>=32'sd30))  begin 
                                  $finish(32'sd0);
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_1 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1);
                           test50_sum <= $signed(test50_sum+hprpin500858x10);
                           test50_command2 <= 32'sh49;
                           end 
                          
                  32'h7/*7:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_1 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1);
                           test50_sum <= $signed(test50_sum+hprpin500852x10);
                           test50_command2 <= 32'sh49;
                           end 
                          
                  32'h9/*9:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiTSX480PC10nz <= 32'h8/*8:kiwiTSX480PC10nz*/;
                      
                  32'ha/*10:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                      
                  32'hb/*11:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                      
                  32'h8/*8:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0)>=32'sd30))  begin 
                                  $finish(32'sd0);
                                   kiwiTSX480PC10nz <= 32'ha/*10:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_0 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0);
                                   test50_command2 <= 32'sh49;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                  if (!test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0)>=32'sd30))  begin 
                                   kiwiTSX480PC10nz <= 32'hb/*11:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_0 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0);
                                   test50_command2 <= 32'sh49;
                                   end 
                                  if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0)<32'sd30))  begin 
                                   kiwiTSX480PC10nz <= 32'h9/*9:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_0 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0);
                                   end 
                                   end 
                          
                  32'h4/*4:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)>=32'sd30))  kiwiTSX480PC10nz <= 32'h6
                              /*6:kiwiTSX480PC10nz*/;

                              if (!test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)>=32'sd30))  kiwiTSX480PC10nz
                               <= 32'h7/*7:kiwiTSX480PC10nz*/;

                              if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)<32'sd30))  kiwiTSX480PC10nz <= 32'h5/*5:kiwiTSX480PC10nz*/;
                               end 
                          
                  32'h2/*2:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if (test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)>=32'sd30))  begin 
                                  $finish(32'sd0);
                                   kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2);
                                   test50_command2 <= 32'sh49;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                  if (!test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)>=32'sd30))  begin 
                                   kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_2 <= $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2);
                                   test50_command2 <= 32'sh49;
                                   end 
                                  if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)<32'sd30))  kiwiTSX480PC10nz <= 32'h3/*3:kiwiTSX480PC10nz*/;
                               end 
                          
                  32'hd/*13:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sd68==test50_command2)) $display("sp: Print data: sharedData[%1d] = %1d", 32'sh0, hprpin500879x10);
                               kiwiTSX480PC10nz <= 32'h2/*2:kiwiTSX480PC10nz*/;
                           _W0_TSX480_test50_secondProcess_V_2 <= 32'sh0;
                           end 
                          
                  32'hc/*12:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sd80==test50_command2)) $display("sp: data sum %1d", test50_sum);
                              if (((32'sd73==test50_command2)? test50_exiting: (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                          ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2) && test50_exiting))  begin 
                                  if (((32'sd73==test50_command2)? test50_exiting: (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                                  ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2) && test50_exiting))  begin 
                                          $finish(32'sd0);
                                           test50_command2 <= 32'sh49;
                                           hpr_abend_syndrome <= 32'sd0;
                                           end 
                                           kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                                   end 
                                  
                          case (test50_command2)
                              32'sd80:  kiwiTSX480PC10nz <= 32'he/*14:kiwiTSX480PC10nz*/;

                              32'sd83:  begin 
                                   kiwiTSX480PC10nz <= 32'h4/*4:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_1 <= 32'sh0;
                                   test50_sum <= 32'sh0;
                                   end 
                                  
                              32'sd85:  begin 
                                   kiwiTSX480PC10nz <= 32'h8/*8:kiwiTSX480PC10nz*/;
                                   _W0_TSX480_test50_secondProcess_V_0 <= 32'sh0;
                                   end 
                                  endcase
                          if ((32'sd73!=test50_command2) && !test50_exiting && (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                          ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2))  test50_command2 <= 32'sh49;
                              if ((32'sd68==test50_command2))  kiwiTSX480PC10nz <= 32'hd/*13:kiwiTSX480PC10nz*/;
                               end 
                          
                  32'he/*14:kiwiTSX480PC10nz*/: if (hpr_int_run_enable_DDX18) if (test50_exiting)  begin 
                              $finish(32'sd0);
                               kiwiTSX480PC10nz <= 32'hf/*15:kiwiTSX480PC10nz*/;
                               test50_command2 <= 32'sh49;
                               hpr_abend_syndrome <= 32'sd0;
                               end 
                               else  begin 
                               kiwiTSX480PC10nz <= 32'hc/*12:kiwiTSX480PC10nz*/;
                               test50_command2 <= 32'sh49;
                               end 
                              endcase
              if (reset)  begin 
               kiwiTSX480PC10nz_pc_export <= 32'd0;
               SINTCCMAPR14NoCE0ARA01rdatah10hold <= 32'd0;
               SINTCCMAPR14NoCE1ARA0rdatah10hold <= 32'd0;
               SINTCCMAPR14NoCE1ARA0rdatah10shot0 <= 32'd0;
               SINTCCMAPR14NoCE0ARA01rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (SINTCCMAPR14NoCE1ARA0rdatah10shot0)  SINTCCMAPR14NoCE1ARA0rdatah10hold <= A_SINT_CC_MAPR14NoCE1_ARA0_rdata;
                      if (SINTCCMAPR14NoCE0ARA01rdatah10shot0)  SINTCCMAPR14NoCE0ARA01rdatah10hold <= A_SINT_CC_MAPR14NoCE0_ARA0_rdata1
                      ;

                       kiwiTSX480PC10nz_pc_export <= kiwiTSX480PC10nz;
                   SINTCCMAPR14NoCE1ARA0rdatah10shot0 <= (32'h4/*4:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2
                  )<32'sd30) && (32'h2/*2:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || (!test50_exiting && (32'h0/*0:kiwiTSX480PC10nz*/==
                  kiwiTSX480PC10nz) || (32'hc/*12:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)) && (32'sd68==test50_command2);

                   SINTCCMAPR14NoCE0ARA01rdatah10shot0 <= (32'h4/*4:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2
                  )<32'sd30) && (32'h2/*2:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || (!test50_exiting && (32'h0/*0:kiwiTSX480PC10nz*/==
                  kiwiTSX480PC10nz) || (32'hc/*12:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)) && (32'sd68==test50_command2);

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TSX480/1.0


      //Start structure cvtToVerilogkiwi.TESTMAIN400_1/1.0
      if (reset)  begin 
               TESTMAIN400_test50_clearto_25_3_V_1 <= 32'd0;
               TESTMAIN400_test50_clearto_25_3_V_0 <= 32'd0;
               TESTMAIN400_test50_test50_phase0_0_12_V_0 <= 32'd0;
               TESTMAIN400_test50_clearto_0_10_V_1 <= 32'd0;
               TESTMAIN400_test50_clearto_0_10_V_0 <= 32'd0;
               kiwiTESTMAIN4001PC10nz <= 32'd0;
               test50_sharedData <= 32'd0;
               test50_exiting <= 32'd0;
               test50_sum <= 32'd0;
               test50_command2 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX22) 
              case (kiwiTESTMAIN4001PC10nz)
                  32'h0/*0:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  begin 
                          $display("Kiwi Demo - Test50 starting.");
                           kiwiTESTMAIN4001PC10nz <= 32'h1/*1:kiwiTESTMAIN4001PC10nz*/;
                           test50_sharedData <= 32'h0;
                           test50_exiting <= 32'h0;
                           test50_sum <= 32'shbc_614e;
                           test50_command2 <= 32'sh78;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  begin 
                          $display("Kiwi Demo - Test50 phase0 starting.");
                          $display("  Test50 Remote Status=%c, sum= %1d", hpr_toChar0(test50_command2), test50_sum);
                           kiwiTESTMAIN4001PC10nz <= 32'h2/*2:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test50_clearto_0_10_V_1 <= 32'sh0;
                           TESTMAIN400_test50_clearto_0_10_V_0 <= 32'sh1f;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h6/*6:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if (($signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_1
                      )<32'sd30))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h5/*5:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test50_clearto_0_10_V_1 <= $signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_1);
                               TESTMAIN400_test50_clearto_0_10_V_0 <= $signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_0);
                               end 
                               else  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h4/*4:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test50_clearto_0_10_V_1 <= $signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_1);
                               end 
                              
                  32'h5/*5:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h8/*8:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh44;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'h7/*7:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h7/*7:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h8/*8:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh44;
                           end 
                          
                  32'h8/*8:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                              $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", 32'sh0, hpr_toChar0(test50_command2), test50_sum
                              );
                               kiwiTESTMAIN4001PC10nz <= 32'h1a/*26:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test50_test50_phase0_0_12_V_0 <= 32'sh0;
                               test50_command2 <= 32'sh50;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'h1b/*27:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'hc/*12:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh50;
                           end 
                          
                  32'hc/*12:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'he/*14:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh55;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'hd/*13:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'he/*14:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh55;
                           end 
                          
                  32'he/*14:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h10/*16:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh53;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'hf/*15:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'hf/*15:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h10/*16:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh53;
                           end 
                          
                  32'h10/*16:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h12/*18:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh50;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'h11/*17:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h11/*17:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h12/*18:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh50;
                           end 
                          
                  32'h12/*18:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h13/*19:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test50_clearto_25_3_V_1 <= 32'sh0;
                               TESTMAIN400_test50_clearto_25_3_V_0 <= $signed(32'sd1+$signed(32'sd40+TESTMAIN400_test50_test50_phase0_0_12_V_0
                              ));

                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'h18/*24:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h13/*19:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h15/*21:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h14/*20:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test50_test50_phase0_0_12_V_0)>=32'sd3))  begin 
                                  $display("Finished main process.");
                                  $display("Test50 starting join.");
                                  $display("Test50 done.");
                                  $finish(32'sd0);
                                   end 
                                   else  begin 
                                  $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", $signed(32'sd1+TESTMAIN400_test50_test50_phase0_0_12_V_0
                                  ), hpr_toChar0(test50_command2), test50_sum);
                                   kiwiTESTMAIN4001PC10nz <= 32'h1a/*26:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test50_test50_phase0_0_12_V_0 <= $signed(32'sd1+TESTMAIN400_test50_test50_phase0_0_12_V_0
                                  );

                                   test50_command2 <= 32'sh50;
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_test50_test50_phase0_0_12_V_0)>=32'sd3))  begin 
                                   kiwiTESTMAIN4001PC10nz <= 32'h1c/*28:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test50_test50_phase0_0_12_V_0 <= $signed(32'sd1+TESTMAIN400_test50_test50_phase0_0_12_V_0
                                  );

                                   test50_exiting <= 32'h1;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h16/*22:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h14/*20:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h17/*23:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h15/*21:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h18/*24:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h19/*25:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test50_clearto_25_3_V_1 <= 32'sh0;
                           TESTMAIN400_test50_clearto_25_3_V_0 <= $signed(32'sd1+$signed(32'sd40+TESTMAIN400_test50_test50_phase0_0_12_V_0
                          ));

                           end 
                          
                  32'h15/*21:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1)>=32'sd30)) $display("   point2 %c %1d.", hpr_toChar0(test50_command2
                              ), test50_sum);
                               else  begin 
                                   kiwiTESTMAIN4001PC10nz <= 32'h17/*23:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test50_clearto_25_3_V_1 <= $signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1);
                                   TESTMAIN400_test50_clearto_25_3_V_0 <= $signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_0);
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1)>=32'sd30))  begin 
                                   kiwiTESTMAIN4001PC10nz <= 32'h16/*22:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test50_clearto_25_3_V_1 <= $signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1);
                                   end 
                                   end 
                          
                  32'h19/*25:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz <= 32'h15/*21:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'hc/*12:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh50;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'hb/*11:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'ha/*10:kiwiTESTMAIN4001PC10nz*/;
                           test50_command2 <= 32'sh53;
                           end 
                          
                  32'h1a/*26:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX22) if ((32'sd73==test50_command2))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'ha/*10:kiwiTESTMAIN4001PC10nz*/;
                               test50_command2 <= 32'sh53;
                               end 
                               else  kiwiTESTMAIN4001PC10nz <= 32'h9/*9:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h1b/*27:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2) && hpr_int_run_enable_DDX22)  begin 
                          $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", 32'sh0, hpr_toChar0(test50_command2), test50_sum);
                           kiwiTESTMAIN4001PC10nz <= 32'h1a/*26:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test50_test50_phase0_0_12_V_0 <= 32'sh0;
                           test50_command2 <= 32'sh50;
                           end 
                          endcase
              if (reset)  kiwiTESTMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX22)  kiwiTESTMAIN4001PC10nz_pc_export <= kiwiTESTMAIN4001PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400_1/1.0


       end 
      

 always   @(* )  begin 
       A_SINT_CC_MAPR14NoCE1_ARA0_addr = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = 32'sd0;
       A_SINT_CC_MAPR14NoCE1_ARA0_wdata = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_wdata1 = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_wen1 = 32'sd0;
       A_SINT_CC_MAPR14NoCE1_ARA0_wen = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_ren1 = 32'sd0;
       A_SINT_CC_MAPR14NoCE1_ARA0_ren = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiTSX480PC10nz)
          32'h0/*0:kiwiTSX480PC10nz*/: if (!test50_exiting && (32'sd68==test50_command2))  begin 
                   A_SINT_CC_MAPR14NoCE1_ARA0_addr = 32'sh0;
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = 32'sh0;
                   end 
                  
          32'h8/*8:kiwiTSX480PC10nz*/:  begin 
              if ((32'h1/*1:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0)<32'sd30)) 
               begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE1_ARA0_wdata = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                      if ((32'h0/*0:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0)<32'sd30
              ))  begin 
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE0_ARA0_wdata1 = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                      if (test50_exiting && (32'h1/*1:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0
              )>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE1_ARA0_wdata = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                      if (test50_exiting && (32'h0/*0:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0
              )>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE0_ARA0_wdata1 = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                      if (!test50_exiting && (32'h1/*1:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0
              )>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE1_ARA0_wdata = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                      if (!test50_exiting && (32'h0/*0:test50.sharedData*/==test50_sharedData) && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_0
              )>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_0;
                       A_SINT_CC_MAPR14NoCE0_ARA0_wdata1 = $signed(test50_sum+_W0_TSX480_test50_secondProcess_V_0);
                       end 
                       end 
              
          32'h4/*4:kiwiTSX480PC10nz*/:  begin 
              if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)<32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_1;
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_1;
                       end 
                      if (test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_1;
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_1;
                       end 
                      if (!test50_exiting && ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_1)>=32'sd30))  begin 
                       A_SINT_CC_MAPR14NoCE1_ARA0_addr = _W0_TSX480_test50_secondProcess_V_1;
                       A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = _W0_TSX480_test50_secondProcess_V_1;
                       end 
                       end 
              
          32'h2/*2:kiwiTSX480PC10nz*/: if (($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)<32'sd30))  begin 
                   A_SINT_CC_MAPR14NoCE1_ARA0_addr = $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2);
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = $signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2);
                   end 
                  
          32'hc/*12:kiwiTSX480PC10nz*/: if ((32'sd68==test50_command2))  begin 
                   A_SINT_CC_MAPR14NoCE1_ARA0_addr = 32'sh0;
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr1 = 32'sh0;
                   end 
                  endcase
      if (hpr_int_run_enable_DDX18)  begin 
               A_SINT_CC_MAPR14NoCE0_ARA0_wen1 = (32'h0/*0:test50.sharedData*/==test50_sharedData) && (32'h8/*8:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
              );

               A_SINT_CC_MAPR14NoCE1_ARA0_wen = (32'h1/*1:test50.sharedData*/==test50_sharedData) && (32'h8/*8:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
              );

               A_SINT_CC_MAPR14NoCE0_ARA0_ren1 = ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)<32'sd30) && (32'h2/*2:kiwiTSX480PC10nz*/==
              kiwiTSX480PC10nz) || (32'h4/*4:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || ((32'hc/*12:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
              ) || !test50_exiting && (32'h0/*0:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)) && (32'sd68==test50_command2);

               A_SINT_CC_MAPR14NoCE1_ARA0_ren = ($signed(32'sd1+_W0_TSX480_test50_secondProcess_V_2)<32'sd30) && (32'h2/*2:kiwiTSX480PC10nz*/==
              kiwiTSX480PC10nz) || (32'h4/*4:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz) || ((32'hc/*12:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
              ) || !test50_exiting && (32'h0/*0:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)) && (32'sd68==test50_command2);

               end 
               hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = 32'sd0;
       A_SINT_CC_MAPR14NoCE0_ARA0_wen0 = 32'sd0;
       hpr_int_run_enable_DDX22 = 32'sd1;

      case (kiwiTESTMAIN4001PC10nz)
          32'h1/*1:kiwiTESTMAIN4001PC10nz*/:  begin 
               A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'sh0;
               A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = 32'sh1e;
               end 
              
          32'h3/*3:kiwiTESTMAIN4001PC10nz*/: if (($signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_1)>=32'sd30))  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'd29;
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = 32'sh63;
                   end 
                   else  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = $signed(32'sd1+TESTMAIN400_test50_clearto_0_10_V_1);
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = TESTMAIN400_test50_clearto_0_10_V_0;
                   end 
                  
          32'h12/*18:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2))  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'sh0;
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = $signed(32'sd40+TESTMAIN400_test50_test50_phase0_0_12_V_0);
                   end 
                  
          32'h18/*24:kiwiTESTMAIN4001PC10nz*/: if ((32'sd73==test50_command2))  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'sh0;
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = $signed(32'sd40+TESTMAIN400_test50_test50_phase0_0_12_V_0);
                   end 
                  
          32'h15/*21:kiwiTESTMAIN4001PC10nz*/: if (($signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1)>=32'sd30))  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = 32'd29;
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = 32'sh63;
                   end 
                   else  begin 
                   A_SINT_CC_MAPR14NoCE0_ARA0_addr0 = $signed(32'sd1+TESTMAIN400_test50_clearto_25_3_V_1);
                   A_SINT_CC_MAPR14NoCE0_ARA0_wdata0 = TESTMAIN400_test50_clearto_25_3_V_0;
                   end 
                  endcase
      if (hpr_int_run_enable_DDX22)  A_SINT_CC_MAPR14NoCE0_ARA0_wen0 = (32'h3/*3:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz) || 
          (32'h1/*1:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz) || (32'h15/*21:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
          ) || ((32'h18/*24:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz) || (32'h12/*18:kiwiTESTMAIN4001PC10nz*/==kiwiTESTMAIN4001PC10nz
          )) && (32'sd73==test50_command2);

           hpr_int_run_enable_DDX22 = (32'sd255==hpr_abend_syndrome);
       A_SINT_CC_MAPR14NoCE0_ARA0_ren0 = 32'd0;
       end 
      

  CV_2P_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd5),
        .WORDS(32'sd30),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) A_SINT_CC_MAPR14NoCE0_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata0(A_SINT_CC_MAPR14NoCE0_ARA0_rdata0),
        .addr0(A_SINT_CC_MAPR14NoCE0_ARA0_addr0
),
        .wen0(A_SINT_CC_MAPR14NoCE0_ARA0_wen0),
        .ren0(A_SINT_CC_MAPR14NoCE0_ARA0_ren0),
        .wdata0(A_SINT_CC_MAPR14NoCE0_ARA0_wdata0
),
        .rdata1(A_SINT_CC_MAPR14NoCE0_ARA0_rdata1),
        .addr1(A_SINT_CC_MAPR14NoCE0_ARA0_addr1),
        .wen1(A_SINT_CC_MAPR14NoCE0_ARA0_wen1
),
        .ren1(A_SINT_CC_MAPR14NoCE0_ARA0_ren1),
        .wdata1(A_SINT_CC_MAPR14NoCE0_ARA0_wdata1));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd5),
        .WORDS(32'sd30),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) A_SINT_CC_MAPR14NoCE1_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_MAPR14NoCE1_ARA0_rdata),
        .addr(A_SINT_CC_MAPR14NoCE1_ARA0_addr
),
        .wen(A_SINT_CC_MAPR14NoCE1_ARA0_wen),
        .ren(A_SINT_CC_MAPR14NoCE1_ARA0_ren),
        .wdata(A_SINT_CC_MAPR14NoCE1_ARA0_wdata
));

assign hprpin500842x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'h1/*1:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'h1/*1:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

assign hprpin500848x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'h3/*3:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'h3/*3:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

assign hprpin500852x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'h7/*7:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'h7/*7:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

assign hprpin500858x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'h6/*6:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'h6/*6:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

assign hprpin500864x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'h5/*5:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'h5/*5:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

assign hprpin500879x10 = ((32'h1/*1:test50.sharedData*/==test50_sharedData)? ((32'hd/*13:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz)? A_SINT_CC_MAPR14NoCE1_ARA0_rdata
: SINTCCMAPR14NoCE1ARA0rdatah10hold): ((32'h0/*0:test50.sharedData*/==test50_sharedData)? ((32'hd/*13:kiwiTSX480PC10nz*/==kiwiTSX480PC10nz
)? A_SINT_CC_MAPR14NoCE0_ARA0_rdata1: SINTCCMAPR14NoCE0ARA01rdatah10hold): 32'bx));

// Structural Resource (FU) inventory for DUT:// 4 vectors of width 5
// 1 vectors of width 4
// 12 vectors of width 1
// 14 vectors of width 32
// 1 vectors of width 16
// Total state bits in module = 500 bits.
// 288 continuously assigned (wire/non-state) bits 
//   cell CV_2P_SSRAM_FL1 count=1
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 2
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:28
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test50.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test50 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test50.Main | Main10 | false |
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
//KiwiC: front end input processing of class test50  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test50..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test50..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test50  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test50.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test50.Main
//
//
//Logging start thread entry point=CE_region<&(CTL_record(System.Threading.ThreadStart, ...))>(System.Threading.ThreadStart.400120%System.Threading.ThreadStart%400120%24, nemtok=TESTMAIN400/test50/Main/CZ:0:6/item10, ats={wondarray=true, constant=true}) user_arg_arity=0: USER_THREAD1(CE_x(bnumU U64'0), CE_conv(CT_cr(System.Object), CS_maskcast, CE_reflection(idl=test50.secondProcess)), ())
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=secondProcess10 full_idl=test50.secondProcess
//
//
//KiwiC: front end input processing of class test50  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test50..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test50..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test50  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test50.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test50.Main
//
//
//Logging start thread entry point=CE_region<&(CTL_record(System.Threading.ThreadStart, ...))>(System.Threading.ThreadStart.400280%System.Threading.ThreadStart%400280%24, nemtok=TESTMAIN400/test50/Main/CZ:0:6/item14, ats={wondarray=true, constant=true}) user_arg_arity=0: USER_THREAD1(CE_x(bnumU U64'0), CE_conv(CT_cr(System.Object), CS_maskcast, CE_reflection(idl=test50.secondProcess)), ())
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
//   srcfile=test50.exe
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
//*-------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                             | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN4001PC10"           | 904 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN4001PC10"           | 903 | 1       | hwm=0.0.1   | 1    |        | 2     | 2   | 3    |
//| XU32'2:"2:kiwiTESTMAIN4001PC10"           | 901 | 3       | hwm=0.0.1   | 3    |        | 5     | 5   | 3    |
//| XU32'2:"2:kiwiTESTMAIN4001PC10"           | 902 | 3       | hwm=0.0.1   | 3    |        | 4     | 4   | 6    |
//| XU32'4:"4:kiwiTESTMAIN4001PC10"           | 899 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 7    |
//| XU32'4:"4:kiwiTESTMAIN4001PC10"           | 900 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 8    |
//| XU32'8:"8:kiwiTESTMAIN4001PC10"           | 897 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 7    |
//| XU32'8:"8:kiwiTESTMAIN4001PC10"           | 898 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 8    |
//| XU32'16:"16:kiwiTESTMAIN4001PC10"         | 895 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 26   |
//| XU32'16:"16:kiwiTESTMAIN4001PC10"         | 896 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 27   |
//| XU32'64:"64:kiwiTESTMAIN4001PC10"         | 891 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 9    |
//| XU32'64:"64:kiwiTESTMAIN4001PC10"         | 892 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'128:"128:kiwiTESTMAIN4001PC10"       | 889 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'128:"128:kiwiTESTMAIN4001PC10"       | 890 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 12   |
//| XU32'256:"256:kiwiTESTMAIN4001PC10"       | 887 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 11   |
//| XU32'256:"256:kiwiTESTMAIN4001PC10"       | 888 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 12   |
//| XU32'512:"512:kiwiTESTMAIN4001PC10"       | 885 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 13   |
//| XU32'512:"512:kiwiTESTMAIN4001PC10"       | 886 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 14   |
//| XU32'1024:"1024:kiwiTESTMAIN4001PC10"     | 883 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 13   |
//| XU32'1024:"1024:kiwiTESTMAIN4001PC10"     | 884 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 14   |
//| XU32'2048:"2048:kiwiTESTMAIN4001PC10"     | 881 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 15   |
//| XU32'2048:"2048:kiwiTESTMAIN4001PC10"     | 882 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 16   |
//| XU32'4096:"4096:kiwiTESTMAIN4001PC10"     | 879 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 15   |
//| XU32'4096:"4096:kiwiTESTMAIN4001PC10"     | 880 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 16   |
//| XU32'8192:"8192:kiwiTESTMAIN4001PC10"     | 877 | 16      | hwm=0.0.0   | 16   |        | -     | -   | 17   |
//| XU32'8192:"8192:kiwiTESTMAIN4001PC10"     | 878 | 16      | hwm=0.0.0   | 16   |        | -     | -   | 18   |
//| XU32'16384:"16384:kiwiTESTMAIN4001PC10"   | 875 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 17   |
//| XU32'16384:"16384:kiwiTESTMAIN4001PC10"   | 876 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 18   |
//| XU32'32768:"32768:kiwiTESTMAIN4001PC10"   | 873 | 18      | hwm=0.0.1   | 18   |        | 19    | 19  | 21   |
//| XU32'32768:"32768:kiwiTESTMAIN4001PC10"   | 874 | 18      | hwm=0.0.0   | 18   |        | -     | -   | 24   |
//| XU32'131072:"131072:kiwiTESTMAIN4001PC10" | 869 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 26   |
//| XU32'131072:"131072:kiwiTESTMAIN4001PC10" | 870 | 20      | hwm=0.0.0   | 20   |        | -     | -   | -    |
//| XU32'65536:"65536:kiwiTESTMAIN4001PC10"   | 871 | 21      | hwm=0.0.1   | 21   |        | 23    | 23  | 21   |
//| XU32'65536:"65536:kiwiTESTMAIN4001PC10"   | 872 | 21      | hwm=0.0.1   | 21   |        | 22    | 22  | 20   |
//| XU32'262144:"262144:kiwiTESTMAIN4001PC10" | 867 | 24      | hwm=0.0.1   | 24   |        | 25    | 25  | 21   |
//| XU32'262144:"262144:kiwiTESTMAIN4001PC10" | 868 | 24      | hwm=0.0.0   | 24   |        | -     | -   | 24   |
//| XU32'32:"32:kiwiTESTMAIN4001PC10"         | 893 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 9    |
//| XU32'32:"32:kiwiTESTMAIN4001PC10"         | 894 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 10   |
//| XU32'524288:"524288:kiwiTESTMAIN4001PC10" | 865 | 27      | hwm=0.0.0   | 27   |        | -     | -   | 26   |
//| XU32'524288:"524288:kiwiTESTMAIN4001PC10" | 866 | 27      | hwm=0.0.0   | 27   |        | -     | -   | 27   |
//*-------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                       |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                            |
//| F0   | E904 | R0 DATA |                                                                                                                                                            |
//| F0+E | E904 | W0 DATA | test50.command2write(S32'120) test50.sumwrite(S32'12345678) test50.exitingwrite(U32'0) test50.sharedDatawrite(Cu({SC:c30,0}))  PLI:Kiwi Demo - Test50 s... |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                    |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                         |
//| F1   | E903 | R0 DATA |                                                                                                                                         |
//| F1+E | E903 | W0 DATA | TESTMAIN400.test50.clearto.0.10.V_0write(S32'31) TESTMAIN400.test50.clearto.0.10.V_1write(S32'0) @_SINT/CC/MAPR14NoCE0_ARA0_0write(S32\ |
//|      |      |         | '0, S32'30)  PLI:  Test50 Remote Stat...  PLI:Kiwi Demo - Test50 p...                                                                   |
//| F2   | E903 | W1 DATA |                                                                                                                                         |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                                     |
//| F3   | E902 | R0 DATA |                                                                                                                                     |
//| F3+E | E902 | W0 DATA | TESTMAIN400.test50.clearto.0.10.V_1write(E1) @_SINT/CC/MAPR14NoCE0_ARA0_0write(29, S32'99)                                          |
//| F4   | E902 | W1 DATA |                                                                                                                                     |
//| F3   | E901 | R0 DATA |                                                                                                                                     |
//| F3+E | E901 | W0 DATA | TESTMAIN400.test50.clearto.0.10.V_0write(E2) TESTMAIN400.test50.clearto.0.10.V_1write(E1) @_SINT/CC/MAPR14NoCE0_ARA0_0write(E1, E3) |
//| F5   | E901 | W1 DATA |                                                                                                                                     |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F6   | -    | R0 CTRL |                              |
//| F6   | E900 | R0 DATA |                              |
//| F6+E | E900 | W0 DATA | test50.command2write(S32'68) |
//| F6   | E899 | R0 DATA |                              |
//| F6+E | E899 | W0 DATA |                              |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F7   | -    | R0 CTRL |                              |
//| F7   | E898 | R0 DATA |                              |
//| F7+E | E898 | W0 DATA | test50.command2write(S32'68) |
//| F7   | E897 | R0 DATA |                              |
//| F7+E | E897 | W0 DATA |                              |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                            |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                                                                 |
//| F8   | E896 | R0 DATA |                                                                                                                 |
//| F8+E | E896 | W0 DATA |                                                                                                                 |
//| F8   | E895 | R0 DATA |                                                                                                                 |
//| F8+E | E895 | W0 DATA | test50.command2write(S32'80) TESTMAIN400.test50.test50_phase0.0.12.V_0write(S32'0)  PLI:  Test50 fancy itera... |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'64:"64:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'64:"64:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F9   | -    | R0 CTRL |                              |
//| F9   | E892 | R0 DATA |                              |
//| F9+E | E892 | W0 DATA | test50.command2write(S32'83) |
//| F9   | E891 | R0 DATA |                              |
//| F9+E | E891 | W0 DATA |                              |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'128:"128:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'128:"128:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F10   | -    | R0 CTRL |                              |
//| F10   | E890 | R0 DATA |                              |
//| F10+E | E890 | W0 DATA | test50.command2write(S32'80) |
//| F10   | E889 | R0 DATA |                              |
//| F10+E | E889 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'256:"256:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'256:"256:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F11   | -    | R0 CTRL |                              |
//| F11   | E888 | R0 DATA |                              |
//| F11+E | E888 | W0 DATA | test50.command2write(S32'80) |
//| F11   | E887 | R0 DATA |                              |
//| F11+E | E887 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'512:"512:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'512:"512:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F12   | -    | R0 CTRL |                              |
//| F12   | E886 | R0 DATA |                              |
//| F12+E | E886 | W0 DATA | test50.command2write(S32'85) |
//| F12   | E885 | R0 DATA |                              |
//| F12+E | E885 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1024:"1024:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1024:"1024:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F13   | -    | R0 CTRL |                              |
//| F13   | E884 | R0 DATA |                              |
//| F13+E | E884 | W0 DATA | test50.command2write(S32'85) |
//| F13   | E883 | R0 DATA |                              |
//| F13+E | E883 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2048:"2048:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2048:"2048:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F14   | -    | R0 CTRL |                              |
//| F14   | E882 | R0 DATA |                              |
//| F14+E | E882 | W0 DATA | test50.command2write(S32'83) |
//| F14   | E881 | R0 DATA |                              |
//| F14+E | E881 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4096:"4096:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4096:"4096:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F15   | -    | R0 CTRL |                              |
//| F15   | E880 | R0 DATA |                              |
//| F15+E | E880 | W0 DATA | test50.command2write(S32'83) |
//| F15   | E879 | R0 DATA |                              |
//| F15+E | E879 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8192:"8192:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8192:"8192:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F16   | -    | R0 CTRL |                              |
//| F16   | E878 | R0 DATA |                              |
//| F16+E | E878 | W0 DATA | test50.command2write(S32'80) |
//| F16   | E877 | R0 DATA |                              |
//| F16+E | E877 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16384:"16384:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16384:"16384:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F17   | -    | R0 CTRL |                              |
//| F17   | E876 | R0 DATA |                              |
//| F17+E | E876 | W0 DATA | test50.command2write(S32'80) |
//| F17   | E875 | R0 DATA |                              |
//| F17+E | E875 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32768:"32768:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32768:"32768:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                      |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| F18   | -    | R0 CTRL |                                                                                                                                           |
//| F18   | E874 | R0 DATA |                                                                                                                                           |
//| F18+E | E874 | W0 DATA |                                                                                                                                           |
//| F18   | E873 | R0 DATA |                                                                                                                                           |
//| F18+E | E873 | W0 DATA | TESTMAIN400.test50.clearto.25.3.V_0write(E4) TESTMAIN400.test50.clearto.25.3.V_1write(S32'0) @_SINT/CC/MAPR14NoCE0_ARA0_0write(S32'0, E5) |
//| F19   | E873 | W1 DATA |                                                                                                                                           |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'131072:"131072:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'131072:"131072:kiwiTESTMAIN4001PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                 |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                                                                                      |
//| F20   | E870 | R0 DATA |                                                                                                                      |
//| F20+E | E870 | W0 DATA | test50.exitingwrite(U32'1) TESTMAIN400.test50.test50_phase0.0.12.V_0write(E6)  PLI:GSAI:hpr_sysexit  PLI:Test50 don\ |
//|       |      |         | e.  PLI:Test50 starting join...  PLI:Finished main proces...                                                         |
//| F20   | E869 | R0 DATA |                                                                                                                      |
//| F20+E | E869 | W0 DATA | test50.command2write(S32'80) TESTMAIN400.test50.test50_phase0.0.12.V_0write(E6)  PLI:  Test50 fancy itera...         |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'65536:"65536:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'65536:"65536:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*
//| F21   | -    | R0 CTRL |                                                                                                                                     |
//| F21   | E872 | R0 DATA |                                                                                                                                     |
//| F21+E | E872 | W0 DATA | TESTMAIN400.test50.clearto.25.3.V_1write(E7) @_SINT/CC/MAPR14NoCE0_ARA0_0write(29, S32'99)  PLI:   point2 %c %d.                    |
//| F22   | E872 | W1 DATA |                                                                                                                                     |
//| F21   | E871 | R0 DATA |                                                                                                                                     |
//| F21+E | E871 | W0 DATA | TESTMAIN400.test50.clearto.25.3.V_0write(E8) TESTMAIN400.test50.clearto.25.3.V_1write(E7) @_SINT/CC/MAPR14NoCE0_ARA0_0write(E7, E9) |
//| F23   | E871 | W1 DATA |                                                                                                                                     |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'262144:"262144:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'262144:"262144:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                      |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| F24   | -    | R0 CTRL |                                                                                                                                           |
//| F24   | E868 | R0 DATA |                                                                                                                                           |
//| F24+E | E868 | W0 DATA |                                                                                                                                           |
//| F24   | E867 | R0 DATA |                                                                                                                                           |
//| F24+E | E867 | W0 DATA | TESTMAIN400.test50.clearto.25.3.V_0write(E4) TESTMAIN400.test50.clearto.25.3.V_1write(S32'0) @_SINT/CC/MAPR14NoCE0_ARA0_0write(S32'0, E5) |
//| F25   | E867 | W1 DATA |                                                                                                                                           |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32:"32:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'32:"32:kiwiTESTMAIN4001PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F26   | -    | R0 CTRL |                              |
//| F26   | E894 | R0 DATA |                              |
//| F26+E | E894 | W0 DATA | test50.command2write(S32'83) |
//| F26   | E893 | R0 DATA |                              |
//| F26+E | E893 | W0 DATA |                              |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'524288:"524288:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'524288:"524288:kiwiTESTMAIN4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                            |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*
//| F27   | -    | R0 CTRL |                                                                                                                 |
//| F27   | E866 | R0 DATA |                                                                                                                 |
//| F27+E | E866 | W0 DATA |                                                                                                                 |
//| F27   | E865 | R0 DATA |                                                                                                                 |
//| F27+E | E865 | W0 DATA | test50.command2write(S32'80) TESTMAIN400.test50.test50_phase0.0.12.V_0write(S32'0)  PLI:  Test50 fancy itera... |
//*-------+------+---------+-----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTSX480PC10 
//*-----------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause               | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTSX480PC10"   | 923 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 12   |
//| XU32'0:"0:kiwiTSX480PC10"   | 924 | 0       | hwm=0.1.0   | 1    |        | 1     | 1   | 2    |
//| XU32'0:"0:kiwiTSX480PC10"   | 925 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 4    |
//| XU32'0:"0:kiwiTSX480PC10"   | 926 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 8    |
//| XU32'0:"0:kiwiTSX480PC10"   | 927 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 14   |
//| XU32'0:"0:kiwiTSX480PC10"   | 928 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 12   |
//| XU32'0:"0:kiwiTSX480PC10"   | 929 | 0       | hwm=0.0.0   | 0    |        | -     | -   | -    |
//| XU32'2:"2:kiwiTSX480PC10"   | 913 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 12   |
//| XU32'2:"2:kiwiTSX480PC10"   | 914 | 2       | hwm=0.0.0   | 2    |        | -     | -   | -    |
//| XU32'2:"2:kiwiTSX480PC10"   | 915 | 2       | hwm=0.1.0   | 3    |        | 3     | 3   | 2    |
//| XU32'4:"4:kiwiTSX480PC10"   | 910 | 4       | hwm=0.1.0   | 7    |        | 7     | 7   | 12   |
//| XU32'4:"4:kiwiTSX480PC10"   | 911 | 4       | hwm=0.1.0   | 6    |        | 6     | 6   | -    |
//| XU32'4:"4:kiwiTSX480PC10"   | 912 | 4       | hwm=0.1.0   | 5    |        | 5     | 5   | 4    |
//| XU32'8:"8:kiwiTSX480PC10"   | 907 | 8       | hwm=0.0.1   | 8    |        | 11    | 11  | 12   |
//| XU32'8:"8:kiwiTSX480PC10"   | 908 | 8       | hwm=0.0.1   | 8    |        | 10    | 10  | -    |
//| XU32'8:"8:kiwiTSX480PC10"   | 909 | 8       | hwm=0.0.1   | 8    |        | 9     | 9   | 8    |
//| XU32'1:"1:kiwiTSX480PC10"   | 916 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 12   |
//| XU32'1:"1:kiwiTSX480PC10"   | 917 | 12      | hwm=0.0.0   | 12   |        | -     | -   | -    |
//| XU32'1:"1:kiwiTSX480PC10"   | 918 | 12      | hwm=0.1.0   | 13   |        | 13    | 13  | 2    |
//| XU32'1:"1:kiwiTSX480PC10"   | 919 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 4    |
//| XU32'1:"1:kiwiTSX480PC10"   | 920 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 8    |
//| XU32'1:"1:kiwiTSX480PC10"   | 921 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 14   |
//| XU32'1:"1:kiwiTSX480PC10"   | 922 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 12   |
//| XU32'16:"16:kiwiTSX480PC10" | 905 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 12   |
//| XU32'16:"16:kiwiTSX480PC10" | 906 | 14      | hwm=0.0.0   | 14   |        | -     | -   | -    |
//*-----------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'0:"0:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'0:"0:kiwiTSX480PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                               |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                    |
//| F0   | E929 | R0 DATA |                                                                                                                    |
//| F0+E | E929 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0) %W0.TSX480.test50\ |
//|      |      |         | .secondProcess.V_2write(S32'0)  PLI:GSAI:hpr_sysexit                                                               |
//| F0   | E928 | R0 DATA |                                                                                                                    |
//| F0+E | E928 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0) %W0.TSX480.test50\ |
//|      |      |         | .secondProcess.V_2write(S32'0)                                                                                     |
//| F0   | E927 | R0 DATA |                                                                                                                    |
//| F0+E | E927 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0) %W0.TSX480.test50\ |
//|      |      |         | .secondProcess.V_2write(S32'0)  PLI:sp: data sum %d                                                                |
//| F0   | E926 | R0 DATA |                                                                                                                    |
//| F0+E | E926 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0) %W0.TSX480.test50\ |
//|      |      |         | .secondProcess.V_2write(S32'0)                                                                                     |
//| F0   | E925 | R0 DATA |                                                                                                                    |
//| F0+E | E925 | W0 DATA | test50.sumwrite(S32'0) %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S\ |
//|      |      |         | 32'0) %W0.TSX480.test50.secondProcess.V_2write(S32'0)                                                              |
//| F0   | E924 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(S32'0) @_SINT/CC/MAPR14NoCE1_ARA0_read(S32'0)                                     |
//| F1   | E924 | R1 DATA |                                                                                                                    |
//| F1+E | E924 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0) %W0.TSX480.test50\ |
//|      |      |         | .secondProcess.V_2write(S32'0)  PLI:sp: Print data: shar...                                                        |
//| F0   | E923 | R0 DATA |                                                                                                                    |
//| F0+E | E923 | W0 DATA | test50.command2write(S32'73) %W0.TSX480.test50.secondProcess.V_0write(S32'0) %W0.TSX480.test50.secondProcess.V_1w\ |
//|      |      |         | rite(S32'0) %W0.TSX480.test50.secondProcess.V_2write(S32'0)                                                        |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'2:"2:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'2:"2:kiwiTSX480PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                             |
//*------+------+---------+--------------------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                                  |
//| F2   | E915 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(E10) @_SINT/CC/MAPR14NoCE1_ARA0_read(E10)                       |
//| F3   | E915 | R1 DATA |                                                                                                  |
//| F3+E | E915 | W0 DATA | %W0.TSX480.test50.secondProcess.V_2write(E10)  PLI:sp: Print data: shar...                       |
//| F2   | E914 | R0 DATA |                                                                                                  |
//| F2+E | E914 | W0 DATA | test50.command2write(S32'73) %W0.TSX480.test50.secondProcess.V_2write(E10)  PLI:GSAI:hpr_sysexit |
//| F2   | E913 | R0 DATA |                                                                                                  |
//| F2+E | E913 | W0 DATA | test50.command2write(S32'73) %W0.TSX480.test50.secondProcess.V_2write(E10)                       |
//*------+------+---------+--------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'4:"4:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'4:"4:kiwiTSX480PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                  |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                       |
//| F4   | E912 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(E11) @_SINT/CC/MAPR14NoCE1_ARA0_read(E11)                                            |
//| F5   | E912 | R1 DATA |                                                                                                                       |
//| F5+E | E912 | W0 DATA | test50.sumwrite(E12) %W0.TSX480.test50.secondProcess.V_1write(E13)                                                    |
//| F4   | E911 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(E11) @_SINT/CC/MAPR14NoCE1_ARA0_read(E11)                                            |
//| F6   | E911 | R1 DATA |                                                                                                                       |
//| F6+E | E911 | W0 DATA | test50.command2write(S32'73) test50.sumwrite(E12) %W0.TSX480.test50.secondProcess.V_1write(E13)  PLI:GSAI:hpr_sysexit |
//| F4   | E910 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(E11) @_SINT/CC/MAPR14NoCE1_ARA0_read(E11)                                            |
//| F7   | E910 | R1 DATA |                                                                                                                       |
//| F7+E | E910 | W0 DATA | test50.command2write(S32'73) test50.sumwrite(E12) %W0.TSX480.test50.secondProcess.V_1write(E13)                       |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'8:"8:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'8:"8:kiwiTSX480PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                                                                            |
//| F8   | E909 | R0 DATA |                                                                                                                            |
//| F8+E | E909 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(E14) @_SINT/CC/MAPR14NoCE0_ARA0_1write(E15, E16) @_SINT/CC/MAPR14NoCE1_ARA0_writ\ |
//|      |      |         | e(E15, E16)                                                                                                                |
//| F9   | E909 | W1 DATA |                                                                                                                            |
//| F8   | E908 | R0 DATA |                                                                                                                            |
//| F8+E | E908 | W0 DATA | test50.command2write(S32'73) %W0.TSX480.test50.secondProcess.V_0write(E14) @_SINT/CC/MAPR14NoCE0_ARA0_1write(E15, E16) @_\ |
//|      |      |         | SINT/CC/MAPR14NoCE1_ARA0_write(E15, E16)  PLI:GSAI:hpr_sysexit                                                             |
//| F10  | E908 | W1 DATA |                                                                                                                            |
//| F8   | E907 | R0 DATA |                                                                                                                            |
//| F8+E | E907 | W0 DATA | test50.command2write(S32'73) %W0.TSX480.test50.secondProcess.V_0write(E14) @_SINT/CC/MAPR14NoCE0_ARA0_1write(E15, E16) @_\ |
//|      |      |         | SINT/CC/MAPR14NoCE1_ARA0_write(E15, E16)                                                                                   |
//| F11  | E907 | W1 DATA |                                                                                                                            |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'1:"1:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'1:"1:kiwiTSX480PC10"
//*-------+------+---------+--------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                           |
//*-------+------+---------+--------------------------------------------------------------------------------*
//| F12   | -    | R0 CTRL |                                                                                |
//| F12   | E922 | R0 DATA |                                                                                |
//| F12+E | E922 | W0 DATA |                                                                                |
//| F12   | E921 | R0 DATA |                                                                                |
//| F12+E | E921 | W0 DATA |  PLI:sp: data sum %d                                                           |
//| F12   | E920 | R0 DATA |                                                                                |
//| F12+E | E920 | W0 DATA | %W0.TSX480.test50.secondProcess.V_0write(S32'0)                                |
//| F12   | E919 | R0 DATA |                                                                                |
//| F12+E | E919 | W0 DATA | test50.sumwrite(S32'0) %W0.TSX480.test50.secondProcess.V_1write(S32'0)         |
//| F12   | E918 | R0 DATA | @_SINT/CC/MAPR14NoCE0_ARA0_1read(S32'0) @_SINT/CC/MAPR14NoCE1_ARA0_read(S32'0) |
//| F13   | E918 | R1 DATA |                                                                                |
//| F13+E | E918 | W0 DATA | %W0.TSX480.test50.secondProcess.V_2write(S32'0)  PLI:sp: Print data: shar...   |
//| F12   | E917 | R0 DATA |                                                                                |
//| F12+E | E917 | W0 DATA | test50.command2write(S32'73)  PLI:GSAI:hpr_sysexit                             |
//| F12   | E916 | R0 DATA |                                                                                |
//| F12+E | E916 | W0 DATA | test50.command2write(S32'73)                                                   |
//*-------+------+---------+--------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'16:"16:kiwiTSX480PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX480PC10 state=XU32'16:"16:kiwiTSX480PC10"
//*-------+------+---------+----------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                               |
//*-------+------+---------+----------------------------------------------------*
//| F14   | -    | R0 CTRL |                                                    |
//| F14   | E906 | R0 DATA |                                                    |
//| F14+E | E906 | W0 DATA | test50.command2write(S32'73)  PLI:GSAI:hpr_sysexit |
//| F14   | E905 | R0 DATA |                                                    |
//| F14+E | E905 | W0 DATA | test50.command2write(S32'73)                       |
//*-------+------+---------+----------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(1+TESTMAIN400.test50.clearto.0.10.V_1)
//
//
//  E2 =.= C(1+(C(TESTMAIN400.test50.clearto.0.10.V_0)))
//
//
//  E3 =.= C(TESTMAIN400.test50.clearto.0.10.V_0)
//
//
//  E4 =.= C(1+(C(40+TESTMAIN400.test50.test50_phase0.0.12.V_0)))
//
//
//  E5 =.= C(40+TESTMAIN400.test50.test50_phase0.0.12.V_0)
//
//
//  E6 =.= C(1+TESTMAIN400.test50.test50_phase0.0.12.V_0)
//
//
//  E7 =.= C(1+TESTMAIN400.test50.clearto.25.3.V_1)
//
//
//  E8 =.= C(1+(C(TESTMAIN400.test50.clearto.25.3.V_0)))
//
//
//  E9 =.= C(TESTMAIN400.test50.clearto.25.3.V_0)
//
//
//  E10 =.= C(1+%W0.TSX480.test50.secondProcess.V_2)
//
//
//  E11 =.= %W0.TSX480.test50.secondProcess.V_1
//
//
//  E12 =.= C(test50.sum+(COND(XU32'1:"1:test50.sharedData"==test50.sharedData, @_SINT/CC/MAPR14NoCE1_ARA0[%W0.TSX480.test50.secondProcess.V_1], COND(XU32'0:"0:test50.sharedData"==test50.sharedData, @_SINT/CC/MAPR14NoCE0_ARA0[%W0.TSX480.test50.secondProcess.V_1], *UNDEF))))
//
//
//  E13 =.= C(1+%W0.TSX480.test50.secondProcess.V_1)
//
//
//  E14 =.= C(1+%W0.TSX480.test50.secondProcess.V_0)
//
//
//  E15 =.= %W0.TSX480.test50.secondProcess.V_0
//
//
//  E16 =.= C(test50.sum+%W0.TSX480.test50.secondProcess.V_0)
//
//
//  E17 =.= (C(1+TESTMAIN400.test50.clearto.0.10.V_1))>=30
//
//
//  E18 =.= (C(1+TESTMAIN400.test50.clearto.0.10.V_1))<30
//
//
//  E19 =.= (C(1+TESTMAIN400.test50.test50_phase0.0.12.V_0))>=3
//
//
//  E20 =.= (C(1+TESTMAIN400.test50.test50_phase0.0.12.V_0))<3
//
//
//  E21 =.= (C(1+TESTMAIN400.test50.clearto.25.3.V_1))>=30
//
//
//  E22 =.= (C(1+TESTMAIN400.test50.clearto.25.3.V_1))<30
//
//
//  E23 =.= {[73==test50.command2, |test50.exiting|]}
//
//
//  E24 =.= {[|test50.exiting|, 80==test50.command2]}
//
//
//  E25 =.= {[|test50.exiting|, 85==test50.command2]}
//
//
//  E26 =.= {[|test50.exiting|, 83==test50.command2]}
//
//
//  E27 =.= {[|test50.exiting|, 68==test50.command2]}
//
//
//  E28 =.= {[73!=test50.command2, |test50.exiting|, 85!=test50.command2, 83!=test50.command2, 80!=test50.command2, 68!=test50.command2]}
//
//
//  E29 =.= (C(1+%W0.TSX480.test50.secondProcess.V_2))<30
//
//
//  E30 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_2))>=30]}
//
//
//  E31 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_2))>=30]}
//
//
//  E32 =.= (C(1+%W0.TSX480.test50.secondProcess.V_1))<30
//
//
//  E33 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_1))>=30]}
//
//
//  E34 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_1))>=30]}
//
//
//  E35 =.= (C(1+%W0.TSX480.test50.secondProcess.V_0))<30
//
//
//  E36 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_0))>=30]}
//
//
//  E37 =.= {[|test50.exiting|, (C(1+%W0.TSX480.test50.secondProcess.V_0))>=30]}
//
//
//  E38 =.= {[73==test50.command2, |test50.exiting|]}
//
//
//  E39 =.= {[73!=test50.command2, |test50.exiting|, 85!=test50.command2, 83!=test50.command2, 80!=test50.command2, 68!=test50.command2]; [73==test50.command2, |test50.exiting|]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test50 to test50

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//4 vectors of width 5
//
//1 vectors of width 4
//
//12 vectors of width 1
//
//14 vectors of width 32
//
//1 vectors of width 16
//
//Total state bits in module = 500 bits.
//
//288 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test50..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread test50.Main uid=Main10 has 115 CIL instructions in 46 basic blocks
//Thread test50.secondProcess uid=secondProcess10 has 54 CIL instructions in 26 basic blocks
//Thread test50..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread test50.Main uid=Main10 has 109 CIL instructions in 46 basic blocks
//Thread mpc10 has 21 bevelab control states (pauses)
//Thread mpc12 has 6 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN4001PC10 with 28 minor control states
//Reindexed thread kiwiTSX480PC10 with 15 minor control states
// eof (HPR L/S Verilog)
