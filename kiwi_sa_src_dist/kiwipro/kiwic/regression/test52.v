

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:41
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test52.exe /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test52.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
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
output reg [5:0] kiwiMAINCESS400PC10nz_pc_export);
function [7:0] hpr_toChar0;
input [31:0] hpr_toChar0_a;
   hpr_toChar0 = hpr_toChar0_a & 8'hff;
endfunction


function signed [15:0] rtl_signed_bitextract2;
   input [31:0] arg;
   rtl_signed_bitextract2 = $signed(arg[15:0]);
   endfunction


function [15:0] rtl_unsigned_bitextract3;
   input [31:0] arg;
   rtl_unsigned_bitextract3 = $unsigned(arg[15:0]);
   endfunction


function signed [31:0] rtl_sign_extend1;
   input [15:0] arg;
   rtl_sign_extend1 = { {16{arg[15]}}, arg[15:0] };
   endfunction


function [31:0] rtl_unsigned_extend4;
   input [15:0] arg;
   rtl_unsigned_extend4 = { 16'b0, arg[15:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] MAINCESS400_KiwiStringIO_Write_0_15_V_0;
  reg signed [31:0] MAINCESS400_KiwiStringIO_WriteU_2_3_V_1;
  reg [31:0] MAINCESS400_KiwiStringIO_WriteU_2_3_V_0;
  reg signed [31:0] MAINCESS400_KiwiStringIO_WriteU_1_5_V_1;
  reg [31:0] MAINCESS400_KiwiStringIO_WriteU_1_5_V_0;
  reg signed [31:0] MAINCESS400_KiwiStringIO_Write_0_2_V_0;
// abstractionName=repack-newnets
  reg [31:0] A_UINT_CC_SCALbx10_ARA0[9:0];
// abstractionName=res2-contacts pi_name=CV_INT_FL3_MULTIPLIER_US
  wire [31:0] iuMULTIPLIERALUU32_10_RR;
  reg [31:0] iuMULTIPLIERALUU32_10_XX;
  reg [31:0] iuMULTIPLIERALUU32_10_YY;
  wire iuMULTIPLIERALUU32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_INT_VL_DIVIDER_US
  reg iuDIVIDERALUU32_10_REQ;
  wire iuDIVIDERALUU32_10_ACK;
  wire [31:0] iuDIVIDERALUU32_10_RR;
  reg [31:0] iuDIVIDERALUU32_10_NN;
  reg [31:0] iuDIVIDERALUU32_10_DD;
  wire iuDIVIDERALUU32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_INT_FL3_MULTIPLIER_US
  wire [31:0] iuMULTIPLIERALUU32_12_RR;
  reg [31:0] iuMULTIPLIERALUU32_12_XX;
  reg [31:0] iuMULTIPLIERALUU32_12_YY;
  wire iuMULTIPLIERALUU32_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_INT_VL_DIVIDER_US
  reg iuDIVIDERALUU32_12_REQ;
  wire iuDIVIDERALUU32_12_ACK;
  wire [31:0] iuDIVIDERALUU32_12_RR;
  reg [31:0] iuDIVIDERALUU32_12_NN;
  reg [31:0] iuDIVIDERALUU32_12_DD;
  wire iuDIVIDERALUU32_12_FAIL;
// abstractionName=res2-morenets
  reg [31:0] pipe56;
  reg [31:0] pipe54;
  reg [31:0] pipe52;
  reg [31:0] pipe50;
  reg [31:0] pipe48;
  reg [31:0] pipe46;
  reg [31:0] pipe44;
  reg [31:0] pipe42;
  reg [31:0] pipe40;
  reg [31:0] pipe38;
  reg [31:0] pipe36;
  reg [31:0] pipe34;
  reg [31:0] pipe32;
  reg [31:0] pipe30;
  reg [31:0] pipe28;
  reg [31:0] pipe26;
  reg [31:0] pipe24;
  reg [31:0] pipe22;
  reg [31:0] pipe20;
  reg [31:0] pipe18;
  reg [31:0] pipe16;
  reg [31:0] pipe14;
  reg [31:0] pipe12;
  reg [31:0] pipe10;
  reg kiwiMAINCESS400PC10_stall;
  reg kiwiMAINCESS400PC10_clear;
  reg [31:0] iuMULTIPLIERALUU3210RRh10hold;
  reg iuMULTIPLIERALUU3210RRh10shot0;
  reg iuMULTIPLIERALUU3210RRh10shot1;
  reg iuMULTIPLIERALUU3210RRh10shot2;
  reg iuDIVIDERALUU3210RRh10primed;
  reg iuDIVIDERALUU3210RRh10vld;
  reg [31:0] iuDIVIDERALUU3210RRh10hold;
  reg [31:0] iuMULTIPLIERALUU3212RRh10hold;
  reg iuMULTIPLIERALUU3212RRh10shot0;
  reg iuMULTIPLIERALUU3212RRh10shot1;
  reg iuMULTIPLIERALUU3212RRh10shot2;
  reg iuDIVIDERALUU3212RRh10primed;
  reg iuDIVIDERALUU3212RRh10vld;
  reg [31:0] iuDIVIDERALUU3212RRh10hold;
  reg [5:0] kiwiMAINCESS400PC10nz;
 always   @(* )  begin 
       iuMULTIPLIERALUU32_12_XX = 32'sd0;
       iuMULTIPLIERALUU32_12_YY = 32'sd0;
       iuDIVIDERALUU32_12_NN = 32'sd0;
       iuDIVIDERALUU32_12_DD = 32'sd0;
       iuMULTIPLIERALUU32_10_XX = 32'sd0;
       iuMULTIPLIERALUU32_10_YY = 32'sd0;
       iuDIVIDERALUU32_10_NN = 32'sd0;
       iuDIVIDERALUU32_10_DD = 32'sd0;
       iuDIVIDERALUU32_12_REQ = 32'sd0;
       iuDIVIDERALUU32_10_REQ = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;

      case (kiwiMAINCESS400PC10nz)
          32'hf/*15:kiwiMAINCESS400PC10nz*/: if (!kiwiMAINCESS400PC10_stall)  begin 
                  if (!iuDIVIDERALUU3212RRh10vld && !iuDIVIDERALUU32_12_ACK)  begin 
                           iuMULTIPLIERALUU32_12_XX = (iuDIVIDERALUU3212RRh10vld? iuDIVIDERALUU3212RRh10hold: iuDIVIDERALUU32_12_RR);
                           iuMULTIPLIERALUU32_12_YY = pipe56;
                           end 
                          if (iuDIVIDERALUU3212RRh10vld || iuDIVIDERALUU32_12_ACK)  begin 
                           iuMULTIPLIERALUU32_12_XX = (iuDIVIDERALUU3212RRh10vld? iuDIVIDERALUU3212RRh10hold: iuDIVIDERALUU32_12_RR);
                           iuMULTIPLIERALUU32_12_YY = pipe56;
                           end 
                           end 
                  
          32'h3/*3:kiwiMAINCESS400PC10nz*/: if ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1>=32'sd0) && !kiwiMAINCESS400PC10_stall)  begin 
                   iuDIVIDERALUU32_12_NN = MAINCESS400_KiwiStringIO_WriteU_1_5_V_0;
                   iuDIVIDERALUU32_12_DD = A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_1_5_V_1];
                   end 
                  
          32'h22/*34:kiwiMAINCESS400PC10nz*/: if (!kiwiMAINCESS400PC10_stall)  begin 
                  if (!iuDIVIDERALUU3210RRh10vld && !iuDIVIDERALUU32_10_ACK)  begin 
                           iuMULTIPLIERALUU32_10_XX = (iuDIVIDERALUU3210RRh10vld? iuDIVIDERALUU3210RRh10hold: iuDIVIDERALUU32_10_RR);
                           iuMULTIPLIERALUU32_10_YY = pipe32;
                           end 
                          if (iuDIVIDERALUU3210RRh10vld || iuDIVIDERALUU32_10_ACK)  begin 
                           iuMULTIPLIERALUU32_10_XX = (iuDIVIDERALUU3210RRh10vld? iuDIVIDERALUU3210RRh10hold: iuDIVIDERALUU32_10_RR);
                           iuMULTIPLIERALUU32_10_YY = pipe32;
                           end 
                           end 
                  
          32'h16/*22:kiwiMAINCESS400PC10nz*/: if ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1>=32'sd0) && !kiwiMAINCESS400PC10_stall)  begin 
                   iuDIVIDERALUU32_10_NN = MAINCESS400_KiwiStringIO_WriteU_2_3_V_0;
                   iuDIVIDERALUU32_10_DD = A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_2_3_V_1];
                   end 
                  endcase
      if (!kiwiMAINCESS400PC10_stall && hpr_int_run_enable_DDX16)  begin 
               iuDIVIDERALUU32_12_REQ = ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1>=32'sd0) && (32'h3/*3:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz
              )? 32'd1: 32'd0);

               iuDIVIDERALUU32_10_REQ = ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1>=32'sd0) && (32'h16/*22:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz
              )? 32'd1: 32'd0);

               end 
               hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.MAINCESS400/1.0
      if (reset)  begin 
               iuDIVIDERALUU3210RRh10primed <= 32'd0;
               iuDIVIDERALUU3212RRh10primed <= 32'd0;
               kiwiMAINCESS400PC10nz <= 32'd0;
               MAINCESS400_KiwiStringIO_Write_0_15_V_0 <= 32'd0;
               MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= 32'd0;
               MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= 32'd0;
               MAINCESS400_KiwiStringIO_WriteU_2_3_V_0 <= 32'd0;
               MAINCESS400_KiwiStringIO_WriteU_1_5_V_0 <= 32'd0;
               MAINCESS400_KiwiStringIO_WriteU_1_5_V_1 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiMAINCESS400PC10nz)
                  32'h0/*0:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiMAINCESS400PC10_stall)  begin 
                                  $display("Tester52 Demo Start");
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd0)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd1)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd2)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd3)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd4)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd5)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd6)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd7)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd8)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd9)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd10)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd11)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd12)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd13)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd14)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd15)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str20[((16-32'sd16)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(16'sha));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd0)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd1)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd2)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd3)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd4)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd5)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str18[((6-32'sd6)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(16'sh30));
                                  $write("%c", hpr_toChar0(16'sha));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd0)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd1)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd2)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd3)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd4)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd5)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str16[((6-32'sd6)<<3) +: 8]
                                  ))));
                                  $write("%c", hpr_toChar0(16'sh2d));
                                   MAINCESS400_KiwiStringIO_Write_0_15_V_0 <= 32'sh0;
                                   MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= 32'sh0;
                                   MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= 32'sh11;
                                   MAINCESS400_KiwiStringIO_WriteU_2_3_V_0 <= 32'h0;
                                   MAINCESS400_KiwiStringIO_WriteU_1_5_V_0 <= 32'h141;
                                   MAINCESS400_KiwiStringIO_WriteU_1_5_V_1 <= 32'sh0;
                                   KppWaypoint0 <= 32'sd1;
                                   KppWaypoint1 <= "START";
                                   end 
                                   kiwiMAINCESS400PC10nz <= 32'h1/*1:kiwiMAINCESS400PC10nz*/;
                           end 
                          
                  32'h1/*1:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1>=32'sd9) || (32'h141<A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_1_5_V_1
                          ]))  kiwiMAINCESS400PC10nz <= 32'h2/*2:kiwiMAINCESS400PC10nz*/;
                              if ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1<32'sd9) && (32'h141>=A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_1_5_V_1
                          ]) && !kiwiMAINCESS400PC10_stall)  MAINCESS400_KiwiStringIO_WriteU_1_5_V_1 <= $signed(32'sd1+MAINCESS400_KiwiStringIO_WriteU_1_5_V_1
                              );

                               end 
                          
                  32'h2/*2:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiMAINCESS400PC10_stall)  MAINCESS400_KiwiStringIO_WriteU_1_5_V_1 <= $signed(-32'sd1+MAINCESS400_KiwiStringIO_WriteU_1_5_V_1
                              );

                               kiwiMAINCESS400PC10nz <= 32'h3/*3:kiwiMAINCESS400PC10nz*/;
                           end 
                          
                  32'h4/*4:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h5/*5:kiwiMAINCESS400PC10nz*/;
                      
                  32'h5/*5:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h6/*6:kiwiMAINCESS400PC10nz*/;
                      
                  32'h6/*6:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h7/*7:kiwiMAINCESS400PC10nz*/;
                      
                  32'h7/*7:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h8/*8:kiwiMAINCESS400PC10nz*/;
                      
                  32'h8/*8:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h9/*9:kiwiMAINCESS400PC10nz*/;
                      
                  32'h9/*9:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'ha/*10:kiwiMAINCESS400PC10nz*/;
                      
                  32'ha/*10:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'hb/*11:kiwiMAINCESS400PC10nz*/;
                      
                  32'hb/*11:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'hc/*12:kiwiMAINCESS400PC10nz*/;
                      
                  32'hc/*12:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'hd/*13:kiwiMAINCESS400PC10nz*/;
                      
                  32'hd/*13:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'he/*14:kiwiMAINCESS400PC10nz*/;
                      
                  32'he/*14:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'hf/*15:kiwiMAINCESS400PC10nz*/;
                      
                  32'hf/*15:kiwiMAINCESS400PC10nz*/: if ((iuDIVIDERALUU32_12_ACK || iuDIVIDERALUU3212RRh10vld) && hpr_int_run_enable_DDX16
                  )  kiwiMAINCESS400PC10nz <= 32'h10/*16:kiwiMAINCESS400PC10nz*/;
                      
                  32'h10/*16:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h11/*17:kiwiMAINCESS400PC10nz*/;
                      
                  32'h11/*17:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h12/*18:kiwiMAINCESS400PC10nz*/;
                      
                  32'h3/*3:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1<32'sd0
                      ))  begin 
                              if (!kiwiMAINCESS400PC10_stall)  begin 
                                      $write("%c", hpr_toChar0(16'sha));
                                       MAINCESS400_KiwiStringIO_Write_0_15_V_0 <= 32'sh0;
                                       end 
                                       kiwiMAINCESS400PC10nz <= 32'h13/*19:kiwiMAINCESS400PC10nz*/;
                               iuDIVIDERALUU3212RRh10primed <= !kiwiMAINCESS400PC10_stall;
                               end 
                               else  begin 
                               kiwiMAINCESS400PC10nz <= 32'h4/*4:kiwiMAINCESS400PC10nz*/;
                               iuDIVIDERALUU3212RRh10primed <= !kiwiMAINCESS400PC10_stall;
                               end 
                              
                  32'h12/*18:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_WriteU_1_5_V_1>=32'sd0) && !kiwiMAINCESS400PC10_stall) $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255
                              &rtl_unsigned_bitextract3(rtl_unsigned_extend4(16'd48)+(iuDIVIDERALUU3212RRh10vld? iuDIVIDERALUU3212RRh10hold
                              : iuDIVIDERALUU32_12_RR))))));
                              if (!kiwiMAINCESS400PC10_stall)  begin 
                                   MAINCESS400_KiwiStringIO_WriteU_1_5_V_0 <= $unsigned(MAINCESS400_KiwiStringIO_WriteU_1_5_V_0+((32'h12
                                  /*18:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz)? iuMULTIPLIERALUU32_12_RR: iuMULTIPLIERALUU3212RRh10hold
                                  ));

                                   MAINCESS400_KiwiStringIO_WriteU_1_5_V_1 <= $signed(-32'sd1+MAINCESS400_KiwiStringIO_WriteU_1_5_V_1
                                  );

                                   end 
                                   kiwiMAINCESS400PC10nz <= 32'h3/*3:kiwiMAINCESS400PC10nz*/;
                           end 
                          
                  32'h13/*19:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((MAINCESS400_KiwiStringIO_Write_0_15_V_0<64'sh7
                      ))  begin if (!kiwiMAINCESS400PC10_stall)  begin 
                                  $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str14[((6-MAINCESS400_KiwiStringIO_Write_0_15_V_0
                                  )<<3) +: 8]))));
                                   MAINCESS400_KiwiStringIO_Write_0_15_V_0 <= $signed(32'sd1+MAINCESS400_KiwiStringIO_Write_0_15_V_0);
                                   end 
                                   end 
                           else  begin 
                              if (!kiwiMAINCESS400PC10_stall)  begin 
                                       MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= 32'sh0;
                                       MAINCESS400_KiwiStringIO_WriteU_2_3_V_0 <= 32'h526_aa2c;
                                       end 
                                       kiwiMAINCESS400PC10nz <= 32'h14/*20:kiwiMAINCESS400PC10nz*/;
                               end 
                              
                  32'h14/*20:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1>=32'sd9) || (32'h526_aa2c<A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_2_3_V_1
                          ]))  kiwiMAINCESS400PC10nz <= 32'h15/*21:kiwiMAINCESS400PC10nz*/;
                              if ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1<32'sd9) && (32'h526_aa2c>=A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_2_3_V_1
                          ]) && !kiwiMAINCESS400PC10_stall)  MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= $signed(32'sd1+MAINCESS400_KiwiStringIO_WriteU_2_3_V_1
                              );

                               end 
                          
                  32'h15/*21:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiMAINCESS400PC10_stall)  MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= $signed(-32'sd1+MAINCESS400_KiwiStringIO_WriteU_2_3_V_1
                              );

                               kiwiMAINCESS400PC10nz <= 32'h16/*22:kiwiMAINCESS400PC10nz*/;
                           end 
                          
                  32'h17/*23:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h18/*24:kiwiMAINCESS400PC10nz*/;
                      
                  32'h18/*24:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h19/*25:kiwiMAINCESS400PC10nz*/;
                      
                  32'h19/*25:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1a/*26:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1a/*26:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1b/*27:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1b/*27:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1c/*28:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1c/*28:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1d/*29:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1d/*29:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1e/*30:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1e/*30:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h1f/*31:kiwiMAINCESS400PC10nz*/;
                      
                  32'h1f/*31:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h20/*32:kiwiMAINCESS400PC10nz*/;
                      
                  32'h20/*32:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h21/*33:kiwiMAINCESS400PC10nz*/;
                      
                  32'h21/*33:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h22/*34:kiwiMAINCESS400PC10nz*/;
                      
                  32'h22/*34:kiwiMAINCESS400PC10nz*/: if ((iuDIVIDERALUU32_10_ACK || iuDIVIDERALUU3210RRh10vld) && hpr_int_run_enable_DDX16
                  )  kiwiMAINCESS400PC10nz <= 32'h23/*35:kiwiMAINCESS400PC10nz*/;
                      
                  32'h23/*35:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h24/*36:kiwiMAINCESS400PC10nz*/;
                      
                  32'h24/*36:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiMAINCESS400PC10nz <= 32'h25/*37:kiwiMAINCESS400PC10nz*/;
                      
                  32'h16/*22:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1<32'sd0
                      ))  begin 
                              if (!kiwiMAINCESS400PC10_stall)  begin 
                                      $write("%c", hpr_toChar0(16'sha));
                                       MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= 32'sh0;
                                       end 
                                       kiwiMAINCESS400PC10nz <= 32'h26/*38:kiwiMAINCESS400PC10nz*/;
                               iuDIVIDERALUU3210RRh10primed <= !kiwiMAINCESS400PC10_stall;
                               end 
                               else  begin 
                               kiwiMAINCESS400PC10nz <= 32'h17/*23:kiwiMAINCESS400PC10nz*/;
                               iuDIVIDERALUU3210RRh10primed <= !kiwiMAINCESS400PC10_stall;
                               end 
                              
                  32'h25/*37:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_WriteU_2_3_V_1>=32'sd0) && !kiwiMAINCESS400PC10_stall) $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255
                              &rtl_unsigned_bitextract3(rtl_unsigned_extend4(16'd48)+(iuDIVIDERALUU3210RRh10vld? iuDIVIDERALUU3210RRh10hold
                              : iuDIVIDERALUU32_10_RR))))));
                              if (!kiwiMAINCESS400PC10_stall)  begin 
                                   MAINCESS400_KiwiStringIO_WriteU_2_3_V_1 <= $signed(-32'sd1+MAINCESS400_KiwiStringIO_WriteU_2_3_V_1
                                  );

                                   MAINCESS400_KiwiStringIO_WriteU_2_3_V_0 <= $unsigned(MAINCESS400_KiwiStringIO_WriteU_2_3_V_0+((32'h25
                                  /*37:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz)? iuMULTIPLIERALUU32_10_RR: iuMULTIPLIERALUU3210RRh10hold
                                  ));

                                   end 
                                   kiwiMAINCESS400PC10nz <= 32'h16/*22:kiwiMAINCESS400PC10nz*/;
                           end 
                          
                  32'h26/*38:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_Write_0_2_V_0>=64'shc))  begin if (!kiwiMAINCESS400PC10_stall) $write("%c", hpr_toChar0(16'sha
                                  ));
                                   end 
                               else if (!kiwiMAINCESS400PC10_stall)  begin 
                                      $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str12[((11-MAINCESS400_KiwiStringIO_Write_0_2_V_0
                                      )<<3) +: 8]))));
                                       MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= $signed(32'sd1+MAINCESS400_KiwiStringIO_Write_0_2_V_0
                                      );

                                       end 
                                      if ((MAINCESS400_KiwiStringIO_Write_0_2_V_0>=64'shc))  begin 
                                  if (!kiwiMAINCESS400PC10_stall)  MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= 32'sh0;
                                       kiwiMAINCESS400PC10nz <= 32'h27/*39:kiwiMAINCESS400PC10nz*/;
                                   end 
                                   end 
                          
                  32'h27/*39:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((MAINCESS400_KiwiStringIO_Write_0_2_V_0>=64'shd))  begin if (!kiwiMAINCESS400PC10_stall)  begin 
                                      $write("%c", hpr_toChar0(16'sha));
                                      $display("Tester52 Demo Finished");
                                       end 
                                       end 
                               else if (!kiwiMAINCESS400PC10_stall)  begin 
                                      $write("%c", hpr_toChar0(rtl_sign_extend1(rtl_signed_bitextract2(16'sd255&str10[((12-MAINCESS400_KiwiStringIO_Write_0_2_V_0
                                      )<<3) +: 8]))));
                                       MAINCESS400_KiwiStringIO_Write_0_2_V_0 <= $signed(32'sd1+MAINCESS400_KiwiStringIO_Write_0_2_V_0
                                      );

                                       end 
                                      if ((MAINCESS400_KiwiStringIO_Write_0_2_V_0>=64'shd))  kiwiMAINCESS400PC10nz <= 32'h28/*40:kiwiMAINCESS400PC10nz*/;
                               end 
                          
                  32'h28/*40:kiwiMAINCESS400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiMAINCESS400PC10_stall)  begin 
                                  $finish(32'sd0);
                                   KppWaypoint0 <= 32'sd2;
                                   KppWaypoint1 <= "END";
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   kiwiMAINCESS400PC10nz <= 32'h29/*41:kiwiMAINCESS400PC10nz*/;
                           end 
                          endcase
              if (reset)  begin 
               kiwiMAINCESS400PC10nz_pc_export <= 32'd0;
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
               pipe28 <= 32'd0;
               pipe26 <= 32'd0;
               pipe24 <= 32'd0;
               pipe22 <= 32'd0;
               pipe20 <= 32'd0;
               pipe18 <= 32'd0;
               pipe16 <= 32'd0;
               pipe14 <= 32'd0;
               pipe12 <= 32'd0;
               pipe10 <= 32'd0;
               iuDIVIDERALUU3212RRh10primed <= 32'd0;
               iuDIVIDERALUU3212RRh10vld <= 32'd0;
               iuDIVIDERALUU3212RRh10hold <= 32'd0;
               iuMULTIPLIERALUU3212RRh10hold <= 32'd0;
               iuMULTIPLIERALUU3212RRh10shot1 <= 32'd0;
               iuMULTIPLIERALUU3212RRh10shot2 <= 32'd0;
               iuDIVIDERALUU3210RRh10primed <= 32'd0;
               iuDIVIDERALUU3210RRh10vld <= 32'd0;
               iuDIVIDERALUU3210RRh10hold <= 32'd0;
               iuMULTIPLIERALUU3210RRh10hold <= 32'd0;
               iuMULTIPLIERALUU3210RRh10shot1 <= 32'd0;
               iuMULTIPLIERALUU3210RRh10shot2 <= 32'd0;
               iuMULTIPLIERALUU3210RRh10shot0 <= 32'd0;
               iuMULTIPLIERALUU3212RRh10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if (iuDIVIDERALUU32_12_ACK && iuDIVIDERALUU3212RRh10primed)  begin 
                           iuDIVIDERALUU3212RRh10primed <= 32'd0;
                           iuDIVIDERALUU3212RRh10vld <= 32'd1;
                           iuDIVIDERALUU3212RRh10hold <= iuDIVIDERALUU32_12_RR;
                           end 
                          if (iuDIVIDERALUU32_10_ACK && iuDIVIDERALUU3210RRh10primed)  begin 
                           iuDIVIDERALUU3210RRh10primed <= 32'd0;
                           iuDIVIDERALUU3210RRh10vld <= 32'd1;
                           iuDIVIDERALUU3210RRh10hold <= iuDIVIDERALUU32_10_RR;
                           end 
                          if (!kiwiMAINCESS400PC10_stall && kiwiMAINCESS400PC10_clear)  begin 
                           iuDIVIDERALUU3212RRh10vld <= 32'd0;
                           iuDIVIDERALUU3210RRh10vld <= 32'd0;
                           end 
                          if (iuMULTIPLIERALUU3210RRh10shot2)  iuMULTIPLIERALUU3210RRh10hold <= iuMULTIPLIERALUU32_10_RR;
                      if (iuMULTIPLIERALUU3212RRh10shot2)  iuMULTIPLIERALUU3212RRh10hold <= iuMULTIPLIERALUU32_12_RR;
                       kiwiMAINCESS400PC10nz_pc_export <= kiwiMAINCESS400PC10nz;
                   pipe56 <= pipe54;
                   pipe54 <= pipe52;
                   pipe52 <= pipe50;
                   pipe50 <= pipe48;
                   pipe48 <= pipe46;
                   pipe46 <= pipe44;
                   pipe44 <= pipe42;
                   pipe42 <= pipe40;
                   pipe40 <= pipe38;
                   pipe38 <= pipe36;
                   pipe36 <= pipe34;
                   pipe34 <= (0-A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_1_5_V_1]);
                   pipe32 <= pipe30;
                   pipe30 <= pipe28;
                   pipe28 <= pipe26;
                   pipe26 <= pipe24;
                   pipe24 <= pipe22;
                   pipe22 <= pipe20;
                   pipe20 <= pipe18;
                   pipe18 <= pipe16;
                   pipe16 <= pipe14;
                   pipe14 <= pipe12;
                   pipe12 <= pipe10;
                   pipe10 <= (0-A_UINT_CC_SCALbx10_ARA0[MAINCESS400_KiwiStringIO_WriteU_2_3_V_1]);
                   iuMULTIPLIERALUU3212RRh10shot1 <= iuMULTIPLIERALUU3212RRh10shot0;
                   iuMULTIPLIERALUU3212RRh10shot2 <= iuMULTIPLIERALUU3212RRh10shot1;
                   iuMULTIPLIERALUU3210RRh10shot1 <= iuMULTIPLIERALUU3210RRh10shot0;
                   iuMULTIPLIERALUU3210RRh10shot2 <= iuMULTIPLIERALUU3210RRh10shot1;
                   iuMULTIPLIERALUU3210RRh10shot0 <= (32'h22/*34:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) && !kiwiMAINCESS400PC10_stall
                  ;

                   iuMULTIPLIERALUU3212RRh10shot0 <= (32'hf/*15:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) && !kiwiMAINCESS400PC10_stall
                  ;

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.MAINCESS400/1.0


       end 
      

always @(*) kiwiMAINCESS400PC10_clear = (32'h0/*0:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h1/*1:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h2/*2:kiwiMAINCESS400PC10nz*/==
kiwiMAINCESS400PC10nz) || (32'h3/*3:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h13/*19:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz
) || (32'h14/*20:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h15/*21:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h16
/*22:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h26/*38:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) || (32'h28/*40:kiwiMAINCESS400PC10nz*/==
kiwiMAINCESS400PC10nz) || (32'h27/*39:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz);

always @(*) kiwiMAINCESS400PC10_stall = (32'hf/*15:kiwiMAINCESS400PC10nz*/==kiwiMAINCESS400PC10nz) && !iuDIVIDERALUU3212RRh10vld && !iuDIVIDERALUU32_12_ACK || (32'h22/*34:kiwiMAINCESS400PC10nz*/==
kiwiMAINCESS400PC10nz) && !iuDIVIDERALUU3210RRh10vld && !iuDIVIDERALUU32_10_ACK;

  CV_INT_FL3_MULTIPLIER_US #(.RWIDTH(32'sd32), .A0WIDTH(32'sd32), .A1WIDTH(32'sd32), .trace_me(32'sd0)) iuMULTIPLIERALUU32_10(
        .clk(clk
),
        .reset(reset),
        .RR(iuMULTIPLIERALUU32_10_RR),
        .XX(iuMULTIPLIERALUU32_10_XX),
        .YY(iuMULTIPLIERALUU32_10_YY
),
        .FAIL(iuMULTIPLIERALUU32_10_FAIL));

  CV_INT_VL_DIVIDER_US #(.RWIDTH(32'sd32), .NWIDTH(32'sd32), .DWIDTH(32'sd32), .trace_me(32'sd0)) iuDIVIDERALUU32_10(
        .clk(clk
),
        .reset(reset),
        .REQ(iuDIVIDERALUU32_10_REQ),
        .ACK(iuDIVIDERALUU32_10_ACK),
        .RR(iuDIVIDERALUU32_10_RR
),
        .NN(iuDIVIDERALUU32_10_NN),
        .DD(iuDIVIDERALUU32_10_DD),
        .FAIL(iuDIVIDERALUU32_10_FAIL));

  CV_INT_FL3_MULTIPLIER_US #(.RWIDTH(32'sd32), .A0WIDTH(32'sd32), .A1WIDTH(32'sd32), .trace_me(32'sd0)) iuMULTIPLIERALUU32_12(
        .clk(clk
),
        .reset(reset),
        .RR(iuMULTIPLIERALUU32_12_RR),
        .XX(iuMULTIPLIERALUU32_12_XX),
        .YY(iuMULTIPLIERALUU32_12_YY
),
        .FAIL(iuMULTIPLIERALUU32_12_FAIL));

  CV_INT_VL_DIVIDER_US #(.RWIDTH(32'sd32), .NWIDTH(32'sd32), .DWIDTH(32'sd32), .trace_me(32'sd0)) iuDIVIDERALUU32_12(
        .clk(clk
),
        .reset(reset),
        .REQ(iuDIVIDERALUU32_12_REQ),
        .ACK(iuDIVIDERALUU32_12_ACK),
        .RR(iuDIVIDERALUU32_12_RR
),
        .NN(iuDIVIDERALUU32_12_NN),
        .DD(iuDIVIDERALUU32_12_DD),
        .FAIL(iuDIVIDERALUU32_12_FAIL));

 initial        begin 
      //ROM data table: 10 words of 32 bits.
       A_UINT_CC_SCALbx10_ARA0[0] = 32'h1;
       A_UINT_CC_SCALbx10_ARA0[1] = 32'ha;
       A_UINT_CC_SCALbx10_ARA0[2] = 32'h64;
       A_UINT_CC_SCALbx10_ARA0[3] = 32'h3e8;
       A_UINT_CC_SCALbx10_ARA0[4] = 32'h2710;
       A_UINT_CC_SCALbx10_ARA0[5] = 32'h1_86a0;
       A_UINT_CC_SCALbx10_ARA0[6] = 32'hf_4240;
       A_UINT_CC_SCALbx10_ARA0[7] = 32'h98_9680;
       A_UINT_CC_SCALbx10_ARA0[8] = 32'h5f5_e100;
       A_UINT_CC_SCALbx10_ARA0[9] = 32'h3b9a_ca00;
       end 
      

  reg signed [135:0] str20 = "Test 52 from Kiwi";
  reg signed [55:0] str18 = "Test A ";
  reg signed [55:0] str16 = "Test B ";
  reg signed [55:0] str14 = "Test D ";
  reg signed [103:0] str12 = "Test52 fin.\n";
  reg signed [111:0] str10 = "Test52 Done.\n";
// Structural Resource (FU) inventory for DUT:// 1 vectors of width 6
// 42 vectors of width 32
// 15 vectors of width 1
// 10 array locations of width 32
// Total state bits in module = 1685 bits.
// 134 continuously assigned (wire/non-state) bits 
//   cell CV_INT_FL3_MULTIPLIER_US count=2
//   cell CV_INT_VL_DIVIDER_US count=2
// Total number of leaf cells = 4
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:38
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test52.exe /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test52.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------+-------------+-------*
//| Class | Style   | Dir Style                                                                                            | Timing Target | Method         | UID         | Skip  |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------+-------------+-------*
//| main  | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | main.HwProcess | HwProcess10 | false |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+----------------+-------------+-------*

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
//KiwiC: front end input processing of class KiwiStringIO  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiStringIO..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=KiwiStringIO..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class main  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=main.HwProcess uid=HwProcess10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=HwProcess10 full_idl=main.HwProcess
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
//   ?>?=srcfile, test52.exe, /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll
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
//PC codings points for kiwiMAINCESS400PC10 
//*------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause                      | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiMAINCESS400PC10"     | 835 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiMAINCESS400PC10"     | 833 | 1       | hwm=0.0.0    | 1    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiMAINCESS400PC10"     | 834 | 1       | hwm=0.0.0    | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiMAINCESS400PC10"     | 832 | 2       | hwm=0.0.0    | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiMAINCESS400PC10"     | 830 | 3       | hwm=0.15.0   | 18   |        | 4     | 18  | 3    |
//| XU32'4:"4:kiwiMAINCESS400PC10"     | 831 | 3       | hwm=0.0.0    | 3    |        | -     | -   | 19   |
//| XU32'8:"8:kiwiMAINCESS400PC10"     | 828 | 19      | hwm=0.0.0    | 19   |        | -     | -   | 19   |
//| XU32'8:"8:kiwiMAINCESS400PC10"     | 829 | 19      | hwm=0.0.0    | 19   |        | -     | -   | 20   |
//| XU32'16:"16:kiwiMAINCESS400PC10"   | 826 | 20      | hwm=0.0.0    | 20   |        | -     | -   | 20   |
//| XU32'16:"16:kiwiMAINCESS400PC10"   | 827 | 20      | hwm=0.0.0    | 20   |        | -     | -   | 21   |
//| XU32'32:"32:kiwiMAINCESS400PC10"   | 825 | 21      | hwm=0.0.0    | 21   |        | -     | -   | 22   |
//| XU32'64:"64:kiwiMAINCESS400PC10"   | 823 | 22      | hwm=0.15.0   | 37   |        | 23    | 37  | 22   |
//| XU32'64:"64:kiwiMAINCESS400PC10"   | 824 | 22      | hwm=0.0.0    | 22   |        | -     | -   | 38   |
//| XU32'128:"128:kiwiMAINCESS400PC10" | 821 | 38      | hwm=0.0.0    | 38   |        | -     | -   | 38   |
//| XU32'128:"128:kiwiMAINCESS400PC10" | 822 | 38      | hwm=0.0.0    | 38   |        | -     | -   | 39   |
//| XU32'256:"256:kiwiMAINCESS400PC10" | 819 | 39      | hwm=0.0.0    | 39   |        | -     | -   | 39   |
//| XU32'256:"256:kiwiMAINCESS400PC10" | 820 | 39      | hwm=0.0.0    | 39   |        | -     | -   | 40   |
//| XU32'512:"512:kiwiMAINCESS400PC10" | 818 | 40      | hwm=0.0.0    | 40   |        | -     | -   | -    |
//*------------------------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'0:"0:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'0:"0:kiwiMAINCESS400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                           |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                                |
//| F0   | E835 | R0 DATA |                                                                                                                                                                |
//| F0+E | E835 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.1.5.V_1write(S32'0) MAINCESS400.KiwiStringIO.WriteU.1.5.V_0write(U32'321) MAINCESS400.KiwiStringIO.WriteU.2.3.V_0write(U32'0)\ |
//|      |      |         |  MAINCESS400.KiwiStringIO.Write.0.2.V_0write(S32'17) MAINCESS400.KiwiStringIO.WriteU.2.3.V_1write(S32'0) MAINCESS400.KiwiStringIO.Write.0.15.V_0write(S32'0) \ |
//|      |      |         |  PLI:%c  W/P:START  PLI:Tester52 Demo Start                                                                                                                    |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'1:"1:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'1:"1:kiwiMAINCESS400PC10"
//*------+------+---------+--------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                             |
//*------+------+---------+--------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                  |
//| F1   | E834 | R0 DATA |                                                  |
//| F1+E | E834 | W0 DATA |                                                  |
//| F1   | E833 | R0 DATA |                                                  |
//| F1+E | E833 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.1.5.V_1write(E1) |
//*------+------+---------+--------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'2:"2:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'2:"2:kiwiMAINCESS400PC10"
//*------+------+---------+--------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                             |
//*------+------+---------+--------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                  |
//| F2   | E832 | R0 DATA |                                                  |
//| F2+E | E832 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.1.5.V_1write(E2) |
//*------+------+---------+--------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'4:"4:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'4:"4:kiwiMAINCESS400PC10"
//*-------+------+----------+-----------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                                                      |
//*-------+------+----------+-----------------------------------------------------------------------------------------------------------*
//| F3    | -    | R0 CTRL  |                                                                                                           |
//| F3    | E831 | R0 DATA  |                                                                                                           |
//| F3+E  | E831 | W0 DATA  | MAINCESS400.KiwiStringIO.Write.0.15.V_0write(S32'0)  PLI:%c                                               |
//| F3+S  | E830 | R0 DATA  | iuDIVIDERALUU32_12_compute(E3, E4)                                                                        |
//| F4    | E830 | R1 DATA  |                                                                                                           |
//| F5    | E830 | R2 DATA  |                                                                                                           |
//| F6    | E830 | R3 DATA  |                                                                                                           |
//| F7    | E830 | R4 DATA  |                                                                                                           |
//| F8    | E830 | R5 DATA  |                                                                                                           |
//| F9    | E830 | R6 DATA  |                                                                                                           |
//| F10   | E830 | R7 DATA  |                                                                                                           |
//| F11   | E830 | R8 DATA  |                                                                                                           |
//| F12   | E830 | R9 DATA  |                                                                                                           |
//| F13   | E830 | R10 DATA |                                                                                                           |
//| F14   | E830 | R11 DATA |                                                                                                           |
//| F15+S | E830 | R12 DATA | iuMULTIPLIERALUU32_12_compute(E5, E6)                                                                     |
//| F16   | E830 | R13 DATA |                                                                                                           |
//| F17   | E830 | R14 DATA |                                                                                                           |
//| F18   | E830 | R15 DATA |                                                                                                           |
//| F18+E | E830 | W0 DATA  | MAINCESS400.KiwiStringIO.WriteU.1.5.V_1write(E2) MAINCESS400.KiwiStringIO.WriteU.1.5.V_0write(E7)  PLI:%c |
//*-------+------+----------+-----------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'8:"8:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'8:"8:kiwiMAINCESS400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                           |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F19   | -    | R0 CTRL |                                                                                                                |
//| F19   | E829 | R0 DATA |                                                                                                                |
//| F19+E | E829 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.2.3.V_0write(U32'86420012) MAINCESS400.KiwiStringIO.WriteU.2.3.V_1write(S32'0) |
//| F19   | E828 | R0 DATA |                                                                                                                |
//| F19+E | E828 | W0 DATA | MAINCESS400.KiwiStringIO.Write.0.15.V_0write(E8)  PLI:%c                                                       |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'16:"16:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'16:"16:kiwiMAINCESS400PC10"
//*-------+------+---------+--------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                             |
//*-------+------+---------+--------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                  |
//| F20   | E827 | R0 DATA |                                                  |
//| F20+E | E827 | W0 DATA |                                                  |
//| F20   | E826 | R0 DATA |                                                  |
//| F20+E | E826 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.2.3.V_1write(E9) |
//*-------+------+---------+--------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'32:"32:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'32:"32:kiwiMAINCESS400PC10"
//*-------+------+---------+---------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                              |
//*-------+------+---------+---------------------------------------------------*
//| F21   | -    | R0 CTRL |                                                   |
//| F21   | E825 | R0 DATA |                                                   |
//| F21+E | E825 | W0 DATA | MAINCESS400.KiwiStringIO.WriteU.2.3.V_1write(E10) |
//*-------+------+---------+---------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'64:"64:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'64:"64:kiwiMAINCESS400PC10"
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                                                        |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------*
//| F22   | -    | R0 CTRL  |                                                                                                             |
//| F22   | E824 | R0 DATA  |                                                                                                             |
//| F22+E | E824 | W0 DATA  | MAINCESS400.KiwiStringIO.Write.0.2.V_0write(S32'0)  PLI:%c                                                  |
//| F22+S | E823 | R0 DATA  | iuDIVIDERALUU32_10_compute(E11, E12)                                                                        |
//| F23   | E823 | R1 DATA  |                                                                                                             |
//| F24   | E823 | R2 DATA  |                                                                                                             |
//| F25   | E823 | R3 DATA  |                                                                                                             |
//| F26   | E823 | R4 DATA  |                                                                                                             |
//| F27   | E823 | R5 DATA  |                                                                                                             |
//| F28   | E823 | R6 DATA  |                                                                                                             |
//| F29   | E823 | R7 DATA  |                                                                                                             |
//| F30   | E823 | R8 DATA  |                                                                                                             |
//| F31   | E823 | R9 DATA  |                                                                                                             |
//| F32   | E823 | R10 DATA |                                                                                                             |
//| F33   | E823 | R11 DATA |                                                                                                             |
//| F34+S | E823 | R12 DATA | iuMULTIPLIERALUU32_10_compute(E13, E14)                                                                     |
//| F35   | E823 | R13 DATA |                                                                                                             |
//| F36   | E823 | R14 DATA |                                                                                                             |
//| F37   | E823 | R15 DATA |                                                                                                             |
//| F37+E | E823 | W0 DATA  | MAINCESS400.KiwiStringIO.WriteU.2.3.V_0write(E15) MAINCESS400.KiwiStringIO.WriteU.2.3.V_1write(E10)  PLI:%c |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'128:"128:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'128:"128:kiwiMAINCESS400PC10"
//*-------+------+---------+------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                       |
//*-------+------+---------+------------------------------------------------------------*
//| F38   | -    | R0 CTRL |                                                            |
//| F38   | E822 | R0 DATA |                                                            |
//| F38+E | E822 | W0 DATA | MAINCESS400.KiwiStringIO.Write.0.2.V_0write(S32'0)  PLI:%c |
//| F38   | E821 | R0 DATA |                                                            |
//| F38+E | E821 | W0 DATA | MAINCESS400.KiwiStringIO.Write.0.2.V_0write(E16)  PLI:%c   |
//*-------+------+---------+------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'256:"256:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'256:"256:kiwiMAINCESS400PC10"
//*-------+------+---------+----------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                     |
//*-------+------+---------+----------------------------------------------------------*
//| F39   | -    | R0 CTRL |                                                          |
//| F39   | E820 | R0 DATA |                                                          |
//| F39+E | E820 | W0 DATA |  PLI:Tester52 Demo Finish...  PLI:%c                     |
//| F39   | E819 | R0 DATA |                                                          |
//| F39+E | E819 | W0 DATA | MAINCESS400.KiwiStringIO.Write.0.2.V_0write(E16)  PLI:%c |
//*-------+------+---------+----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'512:"512:kiwiMAINCESS400PC10"
//res2: scon1: nopipeline: Thread=kiwiMAINCESS400PC10 state=XU32'512:"512:kiwiMAINCESS400PC10"
//*-------+------+---------+--------------------------------*
//| pc    | eno  | Phaser  | Work                           |
//*-------+------+---------+--------------------------------*
//| F40   | -    | R0 CTRL |                                |
//| F40   | E818 | R0 DATA |                                |
//| F40+E | E818 | W0 DATA |  PLI:GSAI:hpr_sysexit  W/P:END |
//*-------+------+---------+--------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(1+MAINCESS400.KiwiStringIO.WriteU.1.5.V_1)
//
//
//  E2 =.= C(-1+MAINCESS400.KiwiStringIO.WriteU.1.5.V_1)
//
//
//  E3 =.= MAINCESS400.KiwiStringIO.WriteU.1.5.V_0
//
//
//  E4 =.= @_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1]
//
//
//  E5 =.= Cu(MAINCESS400.KiwiStringIO.WriteU.1.5.V_0/@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1])
//
//
//  E6 =.= -@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1]
//
//
//  E7 =.= Cu(MAINCESS400.KiwiStringIO.WriteU.1.5.V_0+(Cu(MAINCESS400.KiwiStringIO.WriteU.1.5.V_0/@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1]))*-@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1])
//
//
//  E8 =.= C(1+MAINCESS400.KiwiStringIO.Write.0.15.V_0)
//
//
//  E9 =.= C(1+MAINCESS400.KiwiStringIO.WriteU.2.3.V_1)
//
//
//  E10 =.= C(-1+MAINCESS400.KiwiStringIO.WriteU.2.3.V_1)
//
//
//  E11 =.= MAINCESS400.KiwiStringIO.WriteU.2.3.V_0
//
//
//  E12 =.= @_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1]
//
//
//  E13 =.= Cu(MAINCESS400.KiwiStringIO.WriteU.2.3.V_0/@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1])
//
//
//  E14 =.= -@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1]
//
//
//  E15 =.= Cu(MAINCESS400.KiwiStringIO.WriteU.2.3.V_0+(Cu(MAINCESS400.KiwiStringIO.WriteU.2.3.V_0/@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1]))*-@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1])
//
//
//  E16 =.= C(1+MAINCESS400.KiwiStringIO.Write.0.2.V_0)
//
//
//  E17 =.= {[U32'321<@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1]]; [MAINCESS400.KiwiStringIO.WriteU.1.5.V_1>=9]}
//
//
//  E18 =.= {[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1<9, U32'321>=@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.1.5.V_1]]}
//
//
//  E19 =.= MAINCESS400.KiwiStringIO.WriteU.1.5.V_1<0
//
//
//  E20 =.= MAINCESS400.KiwiStringIO.WriteU.1.5.V_1>=0
//
//
//  E21 =.= {[|iuDIVIDERALUU3212RRh10vld|]; [|iuDIVIDERALUU32_12_ACK|]}
//
//
//  E22 =.= MAINCESS400.KiwiStringIO.Write.0.15.V_0>=S64'7
//
//
//  E23 =.= MAINCESS400.KiwiStringIO.Write.0.15.V_0<S64'7
//
//
//  E24 =.= {[U32'86420012<@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1]]; [MAINCESS400.KiwiStringIO.WriteU.2.3.V_1>=9]}
//
//
//  E25 =.= {[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1<9, U32'86420012>=@_UINT/CC/SCALbx10_ARA0[MAINCESS400.KiwiStringIO.WriteU.2.3.V_1]]}
//
//
//  E26 =.= MAINCESS400.KiwiStringIO.WriteU.2.3.V_1<0
//
//
//  E27 =.= MAINCESS400.KiwiStringIO.WriteU.2.3.V_1>=0
//
//
//  E28 =.= {[|iuDIVIDERALUU3210RRh10vld|]; [|iuDIVIDERALUU32_10_ACK|]}
//
//
//  E29 =.= MAINCESS400.KiwiStringIO.Write.0.2.V_0>=S64'12
//
//
//  E30 =.= MAINCESS400.KiwiStringIO.Write.0.2.V_0<S64'12
//
//
//  E31 =.= MAINCESS400.KiwiStringIO.Write.0.2.V_0>=S64'13
//
//
//  E32 =.= MAINCESS400.KiwiStringIO.Write.0.2.V_0<S64'13
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test52 to test52

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 6
//
//42 vectors of width 32
//
//15 vectors of width 1
//
//10 array locations of width 32
//
//Total state bits in module = 1685 bits.
//
//134 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiStringIO..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread main.HwProcess uid=HwProcess10 has 251 CIL instructions in 80 basic blocks
//Thread mpc10 has 11 bevelab control states (pauses)
//Reindexed thread kiwiMAINCESS400PC10 with 41 minor control states
// eof (HPR L/S Verilog)
