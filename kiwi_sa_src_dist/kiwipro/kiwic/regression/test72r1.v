

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:50
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test72r1.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test72r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwic-cil-dump=combined -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiTESTMAIN400PC10nz_pc_export);

function  rtl_unsigned_bitextract1;
   input [31:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[0:0]);
   endfunction


function [31:0] rtl_unsigned_extend0;
   input argbit;
   rtl_unsigned_extend0 = { 31'b0, argbit };
   endfunction

// abstractionName=bevelab-morenetskiwi.TESTMAIN400
  reg hprtestandsetres10;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0;
  reg signed [31:0] TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3;
  reg signed [31:0] TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2;
  reg signed [31:0] TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1;
  reg signed [31:0] TESTMAIN400_Test72r1_Main_V_0;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_SCALbx14_inptr10;
  reg signed [31:0] A_SINT_CC_SCALbx14_entries10;
  reg signed [31:0] A_SINT_CC_SCALbx14_outptr10;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx10_ARA0_rdata;
  reg [6:0] A_SINT_CC_SCALbx10_ARA0_addr;
  reg A_SINT_CC_SCALbx10_ARA0_wen;
  reg A_SINT_CC_SCALbx10_ARA0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg signed [31:0] SINTCCSCALbx10ARA0rdatah10hold;
  reg SINTCCSCALbx10ARA0rdatah10shot0;
  reg [3:0] kiwiTESTMAIN400PC10nz;
 always   @(* )  begin 
       A_SINT_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wdata = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_ren = 32'sd0;
       hpr_int_run_enable_DDX14 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h3/*3:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = A_SINT_CC_SCALbx14_inptr10;
               A_SINT_CC_SCALbx10_ARA0_wdata = $signed(32'sd101+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1);
               end 
              
          32'h5/*5:kiwiTESTMAIN400PC10nz*/: if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
          )>=32'sd10))  A_SINT_CC_SCALbx10_ARA0_addr = A_SINT_CC_SCALbx14_outptr10;
              
          32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (!(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10)))))  A_SINT_CC_SCALbx10_ARA0_addr
               = A_SINT_CC_SCALbx14_outptr10;

              
          32'h7/*7:kiwiTESTMAIN400PC10nz*/: if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3
          )<32'sd10))  A_SINT_CC_SCALbx10_ARA0_addr = A_SINT_CC_SCALbx14_outptr10;
              endcase
      if (hpr_int_run_enable_DDX14)  begin 
               A_SINT_CC_SCALbx10_ARA0_wen = ((32'h3/*3:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);
               A_SINT_CC_SCALbx10_ARA0_ren = (($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1)>=32'sd10) && (32'h5/*5:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3)<32'sd10) && (32'h7/*7:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz)) && (32'sd0>=A_SINT_CC_SCALbx14_entries10) || !rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10
              )) && (32'h9/*9:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

               end 
               hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               hprtestandsetres10 <= 32'd0;
               A_SINT_CC_SCALbx14_entries10 <= 32'd0;
               A_SINT_CC_SCALbx14_inptr10 <= 32'd0;
               A_SINT_CC_SCALbx14_outptr10 <= 32'd0;
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_Test72r1_Main_V_0 <= 32'd0;
               TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= 32'd0;
               TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= 32'd0;
               TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0 <= 32'd0;
               TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test72r1_Main_V_0 <= 32'sh0;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= 32'sh0;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= 32'sh0;
                           TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0 <= 32'sh0;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Start NativeFifo 72r1 Test");
                           kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           A_SINT_CC_SCALbx14_entries10 <= 32'sh0;
                           A_SINT_CC_SCALbx14_inptr10 <= 32'sh0;
                           A_SINT_CC_SCALbx14_outptr10 <= 32'sh0;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((A_SINT_CC_SCALbx14_entries10>=32'sd100)) $display("Enqueue step %1d", 32'sh0);
                               else  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= 32'sh0;
                                   end 
                                  if ((A_SINT_CC_SCALbx14_entries10>=32'sd100))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= 32'sh0;
                                   end 
                                   end 
                          
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Enqueue step %1d", TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1);
                           kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                           hprtestandsetres10 <= 32'h0;
                           A_SINT_CC_SCALbx14_entries10 <= $signed(32'sd1+A_SINT_CC_SCALbx14_entries10);
                           A_SINT_CC_SCALbx14_inptr10 <= ((32'h64/*100:USA14*/==$signed(32'sd1+A_SINT_CC_SCALbx14_inptr10))? 32'sh0: $signed(32'sd1
                          +A_SINT_CC_SCALbx14_inptr10));

                           end 
                          
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((A_SINT_CC_SCALbx14_entries10>=32'sd100) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                          )<32'sd10)) $display("Enqueue step %1d", $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1));
                              if ((32'sd0<A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                          )>=32'sd10))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= 32'sh0;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= 32'sh0;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                                  );

                                   end 
                                  if ((A_SINT_CC_SCALbx14_entries10>=32'sd100) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                          )<32'sd10))  TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                              );

                              if ((A_SINT_CC_SCALbx14_entries10<32'sd100) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                          )<32'sd10))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                                  );

                                   end 
                                  if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1
                          )>=32'sd10))  kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                               end 
                          
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1)>=
                          32'sd10)) $display("Dequeued %1d, sofar %1d", ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                              : SINTCCSCALbx10ARA0rdatah10hold), ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                              : SINTCCSCALbx10ARA0rdatah10hold));
                               kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= 32'sh0;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold);

                           TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0 <= ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold);

                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1);
                           end 
                          
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3)<
                          32'sd10)) $display("Dequeued %1d, sofar %1d", ((32'h8/*8:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                              : SINTCCSCALbx10ARA0rdatah10hold), $signed(TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2+((32'h8/*8:kiwiTESTMAIN400PC10nz*/==
                              kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold)));
                               kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3);
                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= $signed(TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2+((32'h8
                          /*8:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold
                          ));

                           TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0 <= ((32'h8/*8:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold);

                           end 
                          
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10)) && hpr_int_run_enable_DDX14
                  )  kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3)>=32'sd10))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_Main_V_0 <= TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3
                                  );

                                   end 
                                  if ((32'sd0<A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3
                          )<32'sd10))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3 <= $signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3
                                  );

                                   end 
                                  if ((32'sd0>=A_SINT_CC_SCALbx14_entries10) && ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3
                          )<32'sd10))  kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                               end 
                          
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10))))) $display("Dequeued %1d, sofar %1d"
                              , (!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10)))? TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0
                              : ((32'ha/*10:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold
                              )), $signed(TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2+(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10
                              )))? TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0: ((32'ha/*10:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
                              )? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold))));
                               kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           A_SINT_CC_SCALbx14_entries10 <= $signed(-32'sd1+A_SINT_CC_SCALbx14_entries10);
                           A_SINT_CC_SCALbx14_outptr10 <= (!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10)) && (32'h64
                          /*100:USA16*/==$signed(32'sd1+A_SINT_CC_SCALbx14_outptr10))? 32'sh0: $signed(32'sd1+A_SINT_CC_SCALbx14_outptr10
                          ));

                           TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2 <= $signed(TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_2+(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10
                          )))? TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0: ((32'ha/*10:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold)));

                           TESTMAIN400_KiwiFifo_1_DeQueue_4_2_V_0 <= ((32'ha/*10:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold);

                           end 
                          
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("NativeFifo 72r1 Test: End of Demo. rc=%1d", TESTMAIN400_Test72r1_Main_V_0);
                           kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $finish(32'sd0);
                          $finish(32'sd0);
                           kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10hold <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14)  begin 
                  if (SINTCCSCALbx10ARA0rdatah10shot0)  SINTCCSCALbx10ARA0rdatah10hold <= A_SINT_CC_SCALbx10_ARA0_rdata;
                       kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   SINTCCSCALbx10ARA0rdatah10shot0 <= (($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_3)<32'sd10) && (32'h7
                  /*7:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || ($signed(32'sd1+TESTMAIN400_Test72r1_NativeFifoTest_0_5_V_1)>=
                  32'sd10) && (32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)) && (32'sd0>=A_SINT_CC_SCALbx14_entries10) || 
                  !rtl_unsigned_extend0(rtl_unsigned_bitextract1(hprtestandsetres10)) && (32'h9/*9:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
                  );

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd7),
        .WORDS(32'sd100),
        .LANE_WIDTH(32'sd32
),
        .trace_me(32'sd0)) A_SINT_CC_SCALbx10_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_SCALbx10_ARA0_rdata
),
        .addr(A_SINT_CC_SCALbx10_ARA0_addr),
        .wen(A_SINT_CC_SCALbx10_ARA0_wen),
        .ren(A_SINT_CC_SCALbx10_ARA0_ren),
        .wdata(A_SINT_CC_SCALbx10_ARA0_wdata
));

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 4
// 5 vectors of width 1
// 10 vectors of width 32
// 1 vectors of width 7
// Total state bits in module = 336 bits.
// 32 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:47
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test72r1.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test72r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwic-cil-dump=combined -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Class    | Style   | Dir Style                                                                                            | Timing Target | Method        | UID    | Skip  |
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Test72r1 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | Test72r1.Main | Main10 | false |
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
//KiwiC: front end input processing of class Test72r1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test72r1.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Test72r1.Main
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
//   kiwic-cil-dump=combined
//
//
//   kiwic-kcode-dump=early
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
//   kiwife-gtrace-loglevel=0
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
//   srcfile=test72r1.exe
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
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                      | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"     | 873 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"     | 872 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"     | 870 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"     | 871 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 5    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"     | 869 | 3       | hwm=0.0.1   | 3    |        | 4     | 4   | 5    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 865 | 5       | hwm=0.1.0   | 6    |        | 6     | 6   | 7    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 866 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 9    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 867 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 3    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"     | 868 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiTESTMAIN400PC10"   | 862 | 7       | hwm=0.1.0   | 8    |        | 8     | 8   | 7    |
//| XU32'16:"16:kiwiTESTMAIN400PC10"   | 863 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 9    |
//| XU32'16:"16:kiwiTESTMAIN400PC10"   | 864 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 11   |
//| XU32'32:"32:kiwiTESTMAIN400PC10"   | 860 | 9       | hwm=0.1.0   | 10   |        | 10    | 10  | 7    |
//| XU32'32:"32:kiwiTESTMAIN400PC10"   | 861 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 9    |
//| XU32'64:"64:kiwiTESTMAIN400PC10"   | 859 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 12   |
//| XU32'128:"128:kiwiTESTMAIN400PC10" | 858 | 12      | hwm=0.0.0   | 12   |        | -     | -   | -    |
//*------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                     |
//| F0   | E873 | R0 DATA |                                                                                                                     |
//| F0+E | E873 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(S32'0) TESTMAIN400.KiwiFifo`1.DeQueue.4.2.V_0write(S32'0) TESTMAI\ |
//|      |      |         | N400.Test72r1.NativeFifoTest.0.5.V_2write(S32'0) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3write(S32'0) TESTMAIN4\ |
//|      |      |         | 00.Test72r1.Main.V_0write(S32'0)                                                                                    |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                 |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                                      |
//| F1   | E872 | R0 DATA |                                                                                                                                                      |
//| F1+E | E872 | W0 DATA | @_SINT/CC/SCALbx14_outptr10write(S32'0) @_SINT/CC/SCALbx14_inptr10write(S32'0) @_SINT/CC/SCALbx14_entries10write(S32'0)  PLI:Start NativeFifo 72r... |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                         |
//*------+------+---------+------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                              |
//| F2   | E871 | R0 DATA |                                                                              |
//| F2+E | E871 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(S32'0)  PLI:Enqueue step %d |
//| F2   | E870 | R0 DATA |                                                                              |
//| F2+E | E870 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(S32'0)                      |
//*------+------+---------+------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                                                                     |
//| F3   | E869 | R0 DATA |                                                                                                                                                                     |
//| F3+E | E869 | W0 DATA | @_SINT/CC/SCALbx14_inptr10write(E1) @_SINT/CC/SCALbx14_entries10write(E2) hprtestandsetres10write(U32'0) @_SINT/CC/SCALbx10_ARA0_write(E3, E4)  PLI:Enqueue step %d |
//| F4   | E869 | W1 DATA |                                                                                                                                                                     |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                                                                                                                |
//| F5   | E868 | R0 DATA |                                                                                                                                                                |
//| F5+E | E868 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(E5)  PLI:Enqueue step %d                                                                                      |
//| F5   | E867 | R0 DATA |                                                                                                                                                                |
//| F5+E | E867 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(E5)                                                                                                           |
//| F5   | E866 | R0 DATA |                                                                                                                                                                |
//| F5+E | E866 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(E5) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2write(S32'0) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3write\ |
//|      |      |         | (S32'0)                                                                                                                                                        |
//| F5   | E865 | R0 DATA | @_SINT/CC/SCALbx10_ARA0_read(E6)                                                                                                                               |
//| F6   | E865 | R1 DATA |                                                                                                                                                                |
//| F6+E | E865 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1write(E5) TESTMAIN400.KiwiFifo`1.DeQueue.4.2.V_0write(E7) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2write(E7) TES\ |
//|      |      |         | TMAIN400.Test72r1.NativeFifoTest.0.5.V_3write(S32'0)  PLI:Dequeued %d, sofar %...                                                                              |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                                                                                            |
//| F7   | E864 | R0 DATA |                                                                                                                            |
//| F7+E | E864 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3write(E8) TESTMAIN400.Test72r1.Main.V_0write(E9)                                |
//| F7   | E863 | R0 DATA |                                                                                                                            |
//| F7+E | E863 | W0 DATA | TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3write(E8)                                                                       |
//| F7   | E862 | R0 DATA | @_SINT/CC/SCALbx10_ARA0_read(E6)                                                                                           |
//| F8   | E862 | R1 DATA |                                                                                                                            |
//| F8+E | E862 | W0 DATA | TESTMAIN400.KiwiFifo`1.DeQueue.4.2.V_0write(E7) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2write(E10) TESTMAIN400.Test72r\ |
//|      |      |         | 1.NativeFifoTest.0.5.V_3write(E8)  PLI:Dequeued %d, sofar %...                                                             |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                       |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| F9    | -    | R0 CTRL |                                                                                                                                            |
//| F9    | E861 | R0 DATA |                                                                                                                                            |
//| F9+E  | E861 | W0 DATA |                                                                                                                                            |
//| F9    | E860 | R0 DATA | @_SINT/CC/SCALbx10_ARA0_read(E11)                                                                                                          |
//| F10   | E860 | R1 DATA |                                                                                                                                            |
//| F10+E | E860 | W0 DATA | TESTMAIN400.KiwiFifo`1.DeQueue.4.2.V_0write(E12) TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2write(E13) @_SINT/CC/SCALbx14_outptr10write(E\ |
//|       |      |         | 14) @_SINT/CC/SCALbx14_entries10write(E15)  PLI:Dequeued %d, sofar %...                                                                    |
//*-------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//*-------+------+---------+------------------------------*
//| pc    | eno  | Phaser  | Work                         |
//*-------+------+---------+------------------------------*
//| F11   | -    | R0 CTRL |                              |
//| F11   | E859 | R0 DATA |                              |
//| F11+E | E859 | W0 DATA |  PLI:NativeFifo 72r1 Test... |
//*-------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F12   | -    | R0 CTRL |                       |
//| F12   | E858 | R0 DATA |                       |
//| F12+E | E858 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= COND(XU32'100:"100:USA14"==(C(1+(C(@_SINT/CC/SCALbx14_inptr10)))), S32'0, C(1+(C(@_SINT/CC/SCALbx14_inptr10))))
//
//
//  E2 =.= C(1+@_SINT/CC/SCALbx14_entries10)
//
//
//  E3 =.= C(@_SINT/CC/SCALbx14_inptr10)
//
//
//  E4 =.= C(101+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1)
//
//
//  E5 =.= C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1)
//
//
//  E6 =.= @_SINT/CC/SCALbx14_outptr10
//
//
//  E7 =.= C(@_SINT/CC/SCALbx10_ARA0[@_SINT/CC/SCALbx14_outptr10])
//
//
//  E8 =.= C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3)
//
//
//  E9 =.= C(TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2)
//
//
//  E10 =.= C(TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2+(C(@_SINT/CC/SCALbx10_ARA0[@_SINT/CC/SCALbx14_outptr10])))
//
//
//  E11 =.= C(@_SINT/CC/SCALbx14_outptr10)
//
//
//  E12 =.= C(@_SINT/CC/SCALbx10_ARA0[C(@_SINT/CC/SCALbx14_outptr10)])
//
//
//  E13 =.= C(TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_2+(COND(|-|(Cu(C1u(hprtestandsetres10))), C(TESTMAIN400.KiwiFifo`1.DeQueue.4.2.V_0), C(@_SINT/CC/SCALbx10_ARA0[C(@_SINT/CC/SCALbx14_outptr10)]))))
//
//
//  E14 =.= COND({[!(|-|(Cu(C1u(hprtestandsetres10)))), XU32'100:"100:USA16"==(C(1+(C(@_SINT/CC/SCALbx14_outptr10))))]}, S32'0, C(1+(C(@_SINT/CC/SCALbx14_outptr10))))
//
//
//  E15 =.= C(-1+@_SINT/CC/SCALbx14_entries10)
//
//
//  E16 =.= @_SINT/CC/SCALbx14_entries10>=100
//
//
//  E17 =.= @_SINT/CC/SCALbx14_entries10<100
//
//
//  E18 =.= {[@_SINT/CC/SCALbx14_entries10>=100, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1))<10]}
//
//
//  E19 =.= {[@_SINT/CC/SCALbx14_entries10<100, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1))<10]}
//
//
//  E20 =.= {[0<@_SINT/CC/SCALbx14_entries10, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1))>=10]}
//
//
//  E21 =.= {[0>=@_SINT/CC/SCALbx14_entries10, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_1))>=10]}
//
//
//  E22 =.= (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3))>=10
//
//
//  E23 =.= {[0<@_SINT/CC/SCALbx14_entries10, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3))<10]}
//
//
//  E24 =.= {[0>=@_SINT/CC/SCALbx14_entries10, (C(1+TESTMAIN400.Test72r1.NativeFifoTest.0.5.V_3))<10]}
//
//
//  E25 =.= |-|(Cu(C1u(hprtestandsetres10)))
//
//
//  E26 =.= !(Cu(C1u(hprtestandsetres10)))
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test72r1 to test72r1

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 4
//
//5 vectors of width 1
//
//10 vectors of width 32
//
//1 vectors of width 7
//
//Total state bits in module = 336 bits.
//
//32 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread Test72r1.Main uid=Main10 has 120 CIL instructions in 33 basic blocks
//Thread mpc10 has 9 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 13 minor control states
// eof (HPR L/S Verilog)
