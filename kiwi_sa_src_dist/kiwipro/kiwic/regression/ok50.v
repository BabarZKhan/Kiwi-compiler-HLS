

// CBG Orangepath HPR L/S System

// Verilog output file generated at 17/06/2018 12:12:42
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.7 : 15th June 2018 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -bevelab-cfg-dotreport=enable -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -compose=disable test50.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg [7:0] ksubsAbendSyndrome,
    output reg [7:0] ksubsGpioLeds,
    output reg [7:0] ksubsManualWaypoint,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] test5010PC12nz_pc_export,
    output reg [4:0] test5010PC10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batch10 */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);
function [7:0] hpr_toChar0;
input [31:0] hpr_toChar0_a;
   hpr_toChar0 = hpr_toChar0_a & 8'hff;
endfunction

// abstractionName=kiwicmainnets10
  reg signed [31:0] test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2;
  reg signed [31:0] test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1;
  reg signed [31:0] test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0;
  reg signed [31:0] T403_test50_clearto_25_3_V_1;
  reg signed [31:0] T403_test50_clearto_25_3_V_0;
  reg signed [31:0] T403_test50_clearto_0_10_V_1;
  reg signed [31:0] T403_test50_clearto_0_10_V_0;
  reg signed [31:0] T403_test50_test50_phase0_0_12_V_0;
  reg signed [15:0] test50_command2;
  reg signed [31:0] test50_sum;
  reg test50_exiting;
// abstractionName=res2-contacts pi_name=CV_2P_SSRAM_FL1
  wire signed [31:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata0;
  reg [4:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0;
  reg i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen0;
  wire i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren0;
  reg signed [31:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0;
  wire signed [31:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1;
  reg [4:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1;
  reg i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen1;
  reg i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren1;
  reg signed [31:0] i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1;
// abstractionName=res2-morenets
  reg [4:0] test5010PC10nz;
  reg signed [31:0] iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold;
  reg iSINTCCsharedDataSCALbx10sharedDataARA0RRh10shot0;
  reg [3:0] test5010PC12nz;
 always   @(* )  begin 
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen0 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren1 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen1 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1 = 32'sd0;
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = 32'sd0;

      case (test5010PC12nz)
          32'h0/*0:test5010PC12nz*/: if (!test50_exiting && (32'sd68==test50_command2))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
               = 32'd0;

              
          32'h8/*8:test5010PC12nz*/:  begin 
              if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0<32'sd29))  begin 
                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1 = test50_sum+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       end 
                      if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0>=32'sd29))  begin 
                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1 = test50_sum+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       end 
                      if (!test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0>=32'sd29))  begin 
                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1 = test50_sum+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                      ;

                       end 
                       end 
              
          32'h4/*4:test5010PC12nz*/:  begin 
              if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1<32'sd29))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
                   = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1;

                  if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1>=32'sd29))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
                   = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1;

                  if (!test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1>=32'sd29))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
                   = test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1;

                   end 
              
          32'h2/*2:test5010PC12nz*/: if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2<32'sd29))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
               = 32'd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2;

              
          32'hc/*12:test5010PC12nz*/: if ((32'sd68==test50_command2))  i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = 32'd0;
              endcase
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen1 = (32'h8/*8:test5010PC12nz*/==test5010PC12nz);
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren1 = (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2<32'sd29
      ) && (32'h2/*2:test5010PC12nz*/==test5010PC12nz) || (32'h4/*4:test5010PC12nz*/==test5010PC12nz) || ((32'hc/*12:test5010PC12nz*/==
      test5010PC12nz) || !test50_exiting && (32'h0/*0:test5010PC12nz*/==test5010PC12nz)) && (32'sd68==test50_command2);


      case (test5010PC10nz)
          32'h1/*1:test5010PC10nz*/:  begin 
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd0;
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = 32'sh1e;
               end 
              
          32'h3/*3:test5010PC10nz*/: if ((T403_test50_clearto_0_10_V_1>=32'sd29))  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd29;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = 32'sd99;
                   end 
                   else  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd1+T403_test50_clearto_0_10_V_1;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = T403_test50_clearto_0_10_V_0;
                   end 
                  
          32'h12/*18:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd0;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = $signed(32'sd40+T403_test50_test50_phase0_0_12_V_0);
                   end 
                  
          32'h18/*24:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd0;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = $signed(32'sd40+T403_test50_test50_phase0_0_12_V_0);
                   end 
                  
          32'h15/*21:test5010PC10nz*/: if ((T403_test50_clearto_25_3_V_1>=32'sd29))  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd29;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = 32'sd99;
                   end 
                   else  begin 
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd1+T403_test50_clearto_25_3_V_1;
                   i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = T403_test50_clearto_25_3_V_0;
                   end 
                  endcase
       i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen0 = (32'h3/*3:test5010PC10nz*/==test5010PC10nz) || (32'h1/*1:test5010PC10nz*/==
      test5010PC10nz) || (32'h15/*21:test5010PC10nz*/==test5010PC10nz) || ((32'h18/*24:test5010PC10nz*/==test5010PC10nz) || (32'h12/*18:test5010PC10nz*/==
      test5010PC10nz)) && (32'sd73==test50_command2);

       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogtest50/1.0
      if (reset)  begin 
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1 = 32'd0;
               test50_sum <= 32'd0;
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1 = 32'd0;
               test50_command2 <= 32'd0;
               test5010PC12nz <= 32'd0;
               test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'd0;
               test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'd0;
               test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'd0;
               end 
               else 
          case (test5010PC12nz)
              32'h1/*1:test5010PC12nz*/:  begin 
                  if (!test50_exiting && (32'sd68==test50_command2)) $display("sp: Print data: sharedData[%1d] = %1d", 32'sd0, ((32'h1
                      /*1:test5010PC12nz*/==test5010PC12nz)? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1: iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold
                      ));
                       test5010PC12nz <= 32'h2/*2:test5010PC12nz*/;
                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sd0;
                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                   end 
                  
              32'h0/*0:test5010PC12nz*/:  begin 
                  if (test50_exiting) $finish(32'sd0);
                      if (!test50_exiting && (32'sd80==test50_command2))  begin 
                          $display("sp: data sum %1d", test50_sum);
                           test5010PC12nz <= 32'he/*14:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          if (!test50_exiting && (32'sd83==test50_command2))  begin 
                           test5010PC12nz <= 32'h4/*4:test5010PC12nz*/;
                           test50_sum <= 32'sd0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sd0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          if ((32'sd73!=test50_command2) && !test50_exiting && (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                  ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2))  begin 
                           test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;
                           test50_command2 <= 16'sh49;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          if (test50_exiting)  begin 
                           test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          if (!test50_exiting && (32'sd85==test50_command2))  begin 
                           test5010PC12nz <= 32'h8/*8:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sd0;
                           end 
                          if ((32'sd73==test50_command2) && !test50_exiting)  begin 
                           test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sh0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sh0;
                           end 
                          if (!test50_exiting && (32'sd68==test50_command2))  test5010PC12nz <= 32'h1/*1:test5010PC12nz*/;
                       end 
                  
              32'h3/*3:test5010PC12nz*/:  begin 
                  if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2<32'sd29)) $display("sp: Print data: sharedData[%1d] = %1d"
                      , 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2, ((32'h3/*3:test5010PC12nz*/==test5010PC12nz
                      )? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1: iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold));
                       test5010PC12nz <= 32'h2/*2:test5010PC12nz*/;
                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2
                  ;

                   end 
                  
              32'h5/*5:test5010PC12nz*/:  begin 
                   test5010PC12nz <= 32'h4/*4:test5010PC12nz*/;
                   test50_sum <= test50_sum+((32'h5/*5:test5010PC12nz*/==test5010PC12nz)? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1
                  : iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold);

                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1
                  ;

                   end 
                  
              32'h6/*6:test5010PC12nz*/:  begin 
                  if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1>=32'sd29)) $finish(32'sd0);
                       test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;
                   test50_command2 <= 16'sh49;
                   test50_sum <= test50_sum+((32'h6/*6:test5010PC12nz*/==test5010PC12nz)? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1
                  : iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold);

                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1
                  ;

                   end 
                  
              32'h7/*7:test5010PC12nz*/:  begin 
                   test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;
                   test50_command2 <= 16'sh49;
                   test50_sum <= test50_sum+((32'h7/*7:test5010PC12nz*/==test5010PC12nz)? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1
                  : iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold);

                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1
                  ;

                   end 
                  
              32'h9/*9:test5010PC12nz*/:  test5010PC12nz <= 32'h8/*8:test5010PC12nz*/;

              32'ha/*10:test5010PC12nz*/:  test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;

              32'hb/*11:test5010PC12nz*/:  test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;

              32'h8/*8:test5010PC12nz*/:  begin 
                  if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0>=32'sd29))  begin 
                          $finish(32'sd0);
                           test5010PC12nz <= 32'ha/*10:test5010PC12nz*/;
                           test50_command2 <= 16'sh49;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                          ;

                           end 
                          if (!test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0>=32'sd29))  begin 
                           test5010PC12nz <= 32'hb/*11:test5010PC12nz*/;
                           test50_command2 <= 16'sh49;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                          ;

                           end 
                          if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0<32'sd29))  begin 
                           test5010PC12nz <= 32'h9/*9:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0
                          ;

                           end 
                           end 
                  
              32'h4/*4:test5010PC12nz*/:  begin 
                  if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1>=32'sd29))  test5010PC12nz <= 32'h6
                      /*6:test5010PC12nz*/;

                      if (!test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1>=32'sd29))  test5010PC12nz
                       <= 32'h7/*7:test5010PC12nz*/;

                      if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1<32'sd29))  test5010PC12nz <= 32'h5/*5:test5010PC12nz*/;
                       end 
                  
              32'h2/*2:test5010PC12nz*/:  begin 
                  if (test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2>=32'sd29))  begin 
                          $finish(32'sd0);
                           test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;
                           test50_command2 <= 16'sh49;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2
                          ;

                           end 
                          if (!test50_exiting && (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2>=32'sd29))  begin 
                           test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;
                           test50_command2 <= 16'sh49;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sd1+test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2
                          ;

                           end 
                          if ((test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2<32'sd29))  test5010PC12nz <= 32'h3/*3:test5010PC12nz*/;
                       end 
                  
              32'hd/*13:test5010PC12nz*/:  begin 
                  if ((32'sd68==test50_command2)) $display("sp: Print data: sharedData[%1d] = %1d", 32'sd0, ((32'hd/*13:test5010PC12nz*/==
                      test5010PC12nz)? i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1: iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold
                      ));
                       test5010PC12nz <= 32'h2/*2:test5010PC12nz*/;
                   test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2 <= 32'sd0;
                   end 
                  
              32'hc/*12:test5010PC12nz*/:  begin 
                  if ((32'sd80==test50_command2)) $display("sp: data sum %1d", test50_sum);
                      if (((32'sd73==test50_command2)? test50_exiting: (32'sd85!=test50_command2) && (32'sd83!=test50_command2) && (32'sd80
                  !=test50_command2) && (32'sd68!=test50_command2) && test50_exiting))  begin 
                          if (((32'sd73==test50_command2)? test50_exiting: (32'sd85!=test50_command2) && (32'sd83!=test50_command2) && 
                          (32'sd80!=test50_command2) && (32'sd68!=test50_command2) && test50_exiting))  begin 
                                  $finish(32'sd0);
                                  if (((32'sd73==test50_command2)? test50_exiting: (32'sd85!=test50_command2) && (32'sd83!=test50_command2
                                  ) && (32'sd80!=test50_command2) && (32'sd68!=test50_command2) && test50_exiting))  test50_command2 <= 16'sh49
                                      ;

                                       end 
                                   test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;
                           end 
                          
                  case (test50_command2)
                      32'sd80:  test5010PC12nz <= 32'he/*14:test5010PC12nz*/;

                      32'sd83:  begin 
                           test5010PC12nz <= 32'h4/*4:test5010PC12nz*/;
                           test50_sum <= 32'sd0;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_1 <= 32'sd0;
                           end 
                          
                      32'sd85:  begin 
                           test5010PC12nz <= 32'h8/*8:test5010PC12nz*/;
                           test50_T404_test50_secondProcess_T404_test50_secondProcess_V_0 <= 32'sd0;
                           end 
                          endcase
                  if ((32'sd73!=test50_command2) && !test50_exiting && (32'sd85!=test50_command2) && (32'sd83!=test50_command2) && (32'sd80
                  !=test50_command2) && (32'sd68!=test50_command2))  test50_command2 <= 16'sh49;
                      if ((32'sd68==test50_command2))  test5010PC12nz <= 32'hd/*13:test5010PC12nz*/;
                       end 
                  
              32'he/*14:test5010PC12nz*/: if (test50_exiting)  begin 
                      $finish(32'sd0);
                       test5010PC12nz <= 32'hf/*15:test5010PC12nz*/;
                       test50_command2 <= 16'sh49;
                       end 
                       else  begin 
                       test5010PC12nz <= 32'hc/*12:test5010PC12nz*/;
                       test50_command2 <= 16'sh49;
                       end 
                      endcase
      if (reset)  begin 
               test5010PC12nz_pc_export <= 32'd0;
               iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold <= 32'd0;
               iSINTCCsharedDataSCALbx10sharedDataARA0RRh10shot0 <= 32'd0;
               end 
               else  begin 
              if (iSINTCCsharedDataSCALbx10sharedDataARA0RRh10shot0)  iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold <= i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1
                  ;

                   test5010PC12nz_pc_export <= test5010PC12nz;
               iSINTCCsharedDataSCALbx10sharedDataARA0RRh10shot0 <= (32'h4/*4:test5010PC12nz*/==test5010PC12nz) || (test50_T404_test50_secondProcess_T404_test50_secondProcess_V_2
              <32'sd29) && (32'h2/*2:test5010PC12nz*/==test5010PC12nz) || (!test50_exiting && (32'h0/*0:test5010PC12nz*/==test5010PC12nz
              ) || (32'hc/*12:test5010PC12nz*/==test5010PC12nz)) && (32'sd68==test50_command2);

               end 
              if (reset)  begin 
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0 = 32'd0;
               i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0 = 32'd0;
               test5010PC10nz <= 32'd0;
               test50_exiting <= 32'd0;
               ksubsManualWaypoint <= 32'd0;
               ksubsGpioLeds <= 32'd0;
               ksubsAbendSyndrome <= 32'd0;
               T403_test50_clearto_25_3_V_1 <= 32'd0;
               T403_test50_clearto_25_3_V_0 <= 32'd0;
               T403_test50_test50_phase0_0_12_V_0 <= 32'd0;
               T403_test50_clearto_0_10_V_1 <= 32'd0;
               T403_test50_clearto_0_10_V_0 <= 32'd0;
               test50_command2 <= 32'd0;
               test50_sum <= 32'd0;
               end 
               else 
          case (test5010PC10nz)
              32'h0/*0:test5010PC10nz*/:  begin 
                   test5010PC10nz <= 32'h1/*1:test5010PC10nz*/;
                   test50_exiting <= 1'h0;
                   ksubsManualWaypoint <= 8'h0;
                   ksubsGpioLeds <= 8'h80;
                   ksubsAbendSyndrome <= 8'h80;
                   T403_test50_clearto_25_3_V_1 <= 32'sh0;
                   T403_test50_clearto_25_3_V_0 <= 32'sh0;
                   T403_test50_test50_phase0_0_12_V_0 <= 32'sh0;
                   T403_test50_clearto_0_10_V_1 <= 32'sh0;
                   T403_test50_clearto_0_10_V_0 <= 32'sh0;
                   test50_command2 <= 16'sh78;
                   test50_sum <= 32'shbc_614e;
                  $display("Kiwi Demo - Test50 starting.");
                   end 
                  
              32'h1/*1:test5010PC10nz*/:  begin 
                   test5010PC10nz <= 32'h2/*2:test5010PC10nz*/;
                   T403_test50_clearto_0_10_V_1 <= 32'sd0;
                   T403_test50_clearto_0_10_V_0 <= 32'sd31;
                  $display("Kiwi Demo - Test50 phase0 starting.");
                  $display("  Test50 Remote Status=%c, sum= %1d", hpr_toChar0(test50_command2), test50_sum);
                   end 
                  
              32'h2/*2:test5010PC10nz*/:  test5010PC10nz <= 32'h3/*3:test5010PC10nz*/;

              32'h4/*4:test5010PC10nz*/:  test5010PC10nz <= 32'h6/*6:test5010PC10nz*/;

              32'h3/*3:test5010PC10nz*/: if ((T403_test50_clearto_0_10_V_1<32'sd29))  begin 
                       test5010PC10nz <= 32'h5/*5:test5010PC10nz*/;
                       T403_test50_clearto_0_10_V_1 <= 32'sd1+T403_test50_clearto_0_10_V_1;
                       T403_test50_clearto_0_10_V_0 <= 32'sd1+T403_test50_clearto_0_10_V_0;
                       end 
                       else  begin 
                       test5010PC10nz <= 32'h4/*4:test5010PC10nz*/;
                       T403_test50_clearto_0_10_V_1 <= 32'sd1+T403_test50_clearto_0_10_V_1;
                       end 
                      
              32'h5/*5:test5010PC10nz*/:  test5010PC10nz <= 32'h3/*3:test5010PC10nz*/;

              32'h6/*6:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h8/*8:test5010PC10nz*/;
                       test50_command2 <= 16'sh44;
                       end 
                       else  test5010PC10nz <= 32'h7/*7:test5010PC10nz*/;

              32'h7/*7:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h8/*8:test5010PC10nz*/;
                       test50_command2 <= 16'sh44;
                       end 
                      
              32'h8/*8:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                      $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", 32'sd0, hpr_toChar0(test50_command2), test50_sum);
                       test5010PC10nz <= 32'h1a/*26:test5010PC10nz*/;
                       T403_test50_test50_phase0_0_12_V_0 <= 32'sd0;
                       test50_command2 <= 16'sh50;
                       end 
                       else  test5010PC10nz <= 32'h1b/*27:test5010PC10nz*/;

              32'hb/*11:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'hc/*12:test5010PC10nz*/;
                       test50_command2 <= 16'sh50;
                       end 
                      
              32'hc/*12:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'he/*14:test5010PC10nz*/;
                       test50_command2 <= 16'sh55;
                       end 
                       else  test5010PC10nz <= 32'hd/*13:test5010PC10nz*/;

              32'hd/*13:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'he/*14:test5010PC10nz*/;
                       test50_command2 <= 16'sh55;
                       end 
                      
              32'he/*14:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h10/*16:test5010PC10nz*/;
                       test50_command2 <= 16'sh53;
                       end 
                       else  test5010PC10nz <= 32'hf/*15:test5010PC10nz*/;

              32'hf/*15:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h10/*16:test5010PC10nz*/;
                       test50_command2 <= 16'sh53;
                       end 
                      
              32'h10/*16:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h12/*18:test5010PC10nz*/;
                       test50_command2 <= 16'sh50;
                       end 
                       else  test5010PC10nz <= 32'h11/*17:test5010PC10nz*/;

              32'h11/*17:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h12/*18:test5010PC10nz*/;
                       test50_command2 <= 16'sh50;
                       end 
                      
              32'h12/*18:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h13/*19:test5010PC10nz*/;
                       T403_test50_clearto_25_3_V_1 <= 32'sd0;
                       T403_test50_clearto_25_3_V_0 <= 32'sd1+$signed(32'sd40+T403_test50_test50_phase0_0_12_V_0);
                       end 
                       else  test5010PC10nz <= 32'h18/*24:test5010PC10nz*/;

              32'h13/*19:test5010PC10nz*/:  test5010PC10nz <= 32'h15/*21:test5010PC10nz*/;

              32'h14/*20:test5010PC10nz*/:  begin 
                  if ((T403_test50_test50_phase0_0_12_V_0>=32'sd2))  begin 
                          $display("Finished main process.");
                          $display("Test50 starting join.");
                          $display("Test50 done.");
                          $finish(32'sd0);
                           end 
                           else  begin 
                          $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", 32'sd1+T403_test50_test50_phase0_0_12_V_0, hpr_toChar0(test50_command2
                          ), test50_sum);
                           test5010PC10nz <= 32'h1a/*26:test5010PC10nz*/;
                           T403_test50_test50_phase0_0_12_V_0 <= 32'sd1+T403_test50_test50_phase0_0_12_V_0;
                           test50_command2 <= 16'sh50;
                           end 
                          if ((T403_test50_test50_phase0_0_12_V_0>=32'sd2))  begin 
                           test5010PC10nz <= 32'h1c/*28:test5010PC10nz*/;
                           test50_exiting <= 1'h1;
                           T403_test50_test50_phase0_0_12_V_0 <= 32'sd1+T403_test50_test50_phase0_0_12_V_0;
                           end 
                           end 
                  
              32'h16/*22:test5010PC10nz*/:  test5010PC10nz <= 32'h14/*20:test5010PC10nz*/;

              32'h17/*23:test5010PC10nz*/:  test5010PC10nz <= 32'h15/*21:test5010PC10nz*/;

              32'h18/*24:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'h19/*25:test5010PC10nz*/;
                       T403_test50_clearto_25_3_V_1 <= 32'sd0;
                       T403_test50_clearto_25_3_V_0 <= 32'sd1+$signed(32'sd40+T403_test50_test50_phase0_0_12_V_0);
                       end 
                      
              32'h15/*21:test5010PC10nz*/:  begin 
                  if ((T403_test50_clearto_25_3_V_1>=32'sd29)) $display("   point2 %c %1d.", hpr_toChar0(test50_command2), test50_sum
                      );
                       else  begin 
                           test5010PC10nz <= 32'h17/*23:test5010PC10nz*/;
                           T403_test50_clearto_25_3_V_1 <= 32'sd1+T403_test50_clearto_25_3_V_1;
                           T403_test50_clearto_25_3_V_0 <= 32'sd1+T403_test50_clearto_25_3_V_0;
                           end 
                          if ((T403_test50_clearto_25_3_V_1>=32'sd29))  begin 
                           test5010PC10nz <= 32'h16/*22:test5010PC10nz*/;
                           T403_test50_clearto_25_3_V_1 <= 32'sd1+T403_test50_clearto_25_3_V_1;
                           end 
                           end 
                  
              32'h19/*25:test5010PC10nz*/:  test5010PC10nz <= 32'h15/*21:test5010PC10nz*/;

              32'ha/*10:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'hc/*12:test5010PC10nz*/;
                       test50_command2 <= 16'sh50;
                       end 
                       else  test5010PC10nz <= 32'hb/*11:test5010PC10nz*/;

              32'h9/*9:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'ha/*10:test5010PC10nz*/;
                       test50_command2 <= 16'sh53;
                       end 
                      
              32'h1a/*26:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                       test5010PC10nz <= 32'ha/*10:test5010PC10nz*/;
                       test50_command2 <= 16'sh53;
                       end 
                       else  test5010PC10nz <= 32'h9/*9:test5010PC10nz*/;

              32'h1b/*27:test5010PC10nz*/: if ((32'sd73==test50_command2))  begin 
                      $display("  Test50 fancy iteration=%1d rs=%c sum=%1d.", 32'sd0, hpr_toChar0(test50_command2), test50_sum);
                       test5010PC10nz <= 32'h1a/*26:test5010PC10nz*/;
                       T403_test50_test50_phase0_0_12_V_0 <= 32'sd0;
                       test50_command2 <= 16'sh50;
                       end 
                      endcase
      if (reset)  begin 
               test5010PC10nz_pc_export <= 32'd0;
               iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold <= 32'd0;
               end 
               else  begin 
              if (iSINTCCsharedDataSCALbx10sharedDataARA0RRh10shot0)  iSINTCCsharedDataSCALbx10sharedDataARA0RRh10hold <= i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1
                  ;

                   test5010PC10nz_pc_export <= test5010PC10nz;
               end 
              //End structure cvtToVerilogtest50/1.0


       end 
      

  CV_2P_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd5),
        .WORDS(32'sd30),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata0(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata0
),
        .addr0(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr0),
        .wen0(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen0
),
        .ren0(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren0),
        .wdata0(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata0
),
        .rdata1(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_rdata1),
        .addr1(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_addr1
),
        .wen1(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wen1),
        .ren1(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_ren1
),
        .wdata1(i_A_SINT_CC_sharedData_SCALbx10_sharedData_ARA0_wdata1));

// Structural Resource (FU) inventory:// 1 vectors of width 4
// 5 vectors of width 1
// 12 vectors of width 32
// 3 vectors of width 5
// 1 vectors of width 16
// Total state bits in module = 424 bits.
// 65 continuously assigned (wire/non-state) bits 
//   cell CV_2P_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.7 : 15th June 2018
//17/06/2018 12:12:38
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -bevelab-cfg-dotreport=enable -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -compose=disable test50.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation SyThreading for prefix System/Threading
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation @System for prefix @/System
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation @$SyThreading for prefix @/$star1$/System/Threading
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation $SyThreading for prefix $star1$/System/Threading
//

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called KiwiSystem.Kiwi
//
//root_walk start thread at a static method (used as an entry point). Method name=KiwiSystem/Kiwi/.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called System.BitConverter
//
//root_walk start thread at a static method (used as an entry point). Method name=System/BitConverter/.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called test50
//
//root_walk start thread at a static method (used as an entry point). Method name=test50/.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) uid=cctor10 full_idl=test50..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called test50
//
//root_compiler: start elaborating class 'test50'
//
//elaborating class 'test50'
//
//compiling static method as entry point: style=Root idl=test50/Main
//
//Performing root elaboration of method test50.Main
//
//KiwiC start_thread (or entry point) uid=Main10 full_idl=test50.Main
//
//Logging start thread entry point = CE_region<&(CTL_record(System.Threading.ThreadStart, ...))>(System.Threading.ThreadStart.400120%System.Threading.ThreadStart%400120%24, nemtok=test50/T403/test50/Main/CZ:0:6/item10, ats={wondarray=true, constant=true}): USER_THREAD1(CE_x(bnumU U64'0), CE_conv(CT_cr(System.Object), CS_maskcast, CE_reflection(idl=test50.secondProcess)), ())
//
//Logging start thread entry point = CE_region<&(CTL_record(System.Threading.ThreadStart, ...))>(System.Threading.ThreadStart.400120%System.Threading.ThreadStart%400120%24, nemtok=test50/T403/test50/Main/CZ:0:6/item10, ats={wondarray=true, constant=true}): USER_THREAD1(CE_x(bnumU U64'0), CE_conv(CT_cr(System.Object), CS_maskcast, CE_reflection(idl=test50.secondProcess)), ())
//
//KiwiC start_thread (or entry point) uid=secondProcess10 full_idl=test50.secondProcess
//
//root_compiler class done: test50
//
//Report of all settings used from the recipe or command line:
//
//   kiwife-directorate-ready-flag=absent
//
//   kiwife-directorate-endmode=finish
//
//   kiwife-directorate-startmode=self-start
//
//   cil-uwind-budget=10000
//
//   kiwic-cil-dump=disable
//
//   kiwic-kcode-dump=disable
//
//   kiwife-dynpoly=disable
//
//   kiwic-register-colours=disable
//
//   array-4d-name=KIWIARRAY4D
//
//   array-3d-name=KIWIARRAY3D
//
//   array-2d-name=KIWIARRAY2D
//
//   kiwi-dll=Kiwi.dll
//
//   kiwic-dll=Kiwic.dll
//
//   kiwic-zerolength-arrays=disable
//
//   kiwifefpgaconsole-default=enable
//
//   kiwife-directorate-style=basic
//
//   postgen-optimise=enable
//
//   kiwife-cil-loglevel=20
//
//   kiwife-ataken-loglevel=20
//
//   kiwife-gtrace-loglevel=20
//
//   kiwife-firstpass-loglevel=20
//
//   kiwife-overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=test50.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation SThThreadStart for prefix System/Threading/ThreadStart
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation SThThread for prefix System/Threading/Thread
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int-flr-mul               | 1000    |                                                                                 |
//| max-no-fp-addsubs         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls            | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors        | 8       | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing      | 8       | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div              | 5       |                                                                                 |
//| fp-fl-dp-add              | 4       |                                                                                 |
//| fp-fl-dp-mul              | 3       |                                                                                 |
//| fp-fl-sp-div              | 15      |                                                                                 |
//| fp-fl-sp-add              | 4       |                                                                                 |
//| fp-fl-sp-mul              | 5       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//| res2-loadstore-port-count | 0       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for test5010PC12 
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause             | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:test5010PC12"   | 976 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 12   |
//| XU32'0:"0:test5010PC12"   | 977 | 0       | hwm=0.1.0   | 1    |        | 1     | 1   | 2    |
//| XU32'0:"0:test5010PC12"   | 978 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 4    |
//| XU32'0:"0:test5010PC12"   | 979 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 8    |
//| XU32'0:"0:test5010PC12"   | 980 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 14   |
//| XU32'0:"0:test5010PC12"   | 981 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 12   |
//| XU32'0:"0:test5010PC12"   | 982 | 0       | hwm=0.0.0   | 0    |        | -     | -   | -    |
//| XU32'2:"2:test5010PC12"   | 966 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 12   |
//| XU32'2:"2:test5010PC12"   | 967 | 2       | hwm=0.0.0   | 2    |        | -     | -   | -    |
//| XU32'2:"2:test5010PC12"   | 968 | 2       | hwm=0.1.0   | 3    |        | 3     | 3   | 2    |
//| XU32'4:"4:test5010PC12"   | 963 | 4       | hwm=0.1.0   | 7    |        | 7     | 7   | 12   |
//| XU32'4:"4:test5010PC12"   | 964 | 4       | hwm=0.1.0   | 6    |        | 6     | 6   | -    |
//| XU32'4:"4:test5010PC12"   | 965 | 4       | hwm=0.1.0   | 5    |        | 5     | 5   | 4    |
//| XU32'8:"8:test5010PC12"   | 960 | 8       | hwm=0.0.1   | 8    |        | 11    | 11  | 12   |
//| XU32'8:"8:test5010PC12"   | 961 | 8       | hwm=0.0.1   | 8    |        | 10    | 10  | -    |
//| XU32'8:"8:test5010PC12"   | 962 | 8       | hwm=0.0.1   | 8    |        | 9     | 9   | 8    |
//| XU32'1:"1:test5010PC12"   | 969 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 12   |
//| XU32'1:"1:test5010PC12"   | 970 | 12      | hwm=0.0.0   | 12   |        | -     | -   | -    |
//| XU32'1:"1:test5010PC12"   | 971 | 12      | hwm=0.1.0   | 13   |        | 13    | 13  | 2    |
//| XU32'1:"1:test5010PC12"   | 972 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 4    |
//| XU32'1:"1:test5010PC12"   | 973 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 8    |
//| XU32'1:"1:test5010PC12"   | 974 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 14   |
//| XU32'1:"1:test5010PC12"   | 975 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 12   |
//| XU32'16:"16:test5010PC12" | 958 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 12   |
//| XU32'16:"16:test5010PC12" | 959 | 14      | hwm=0.0.0   | 14   |        | -     | -   | -    |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 976 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 977 :  major_start_pcl=0   edge_private_start/end=1/1 exec=1 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 978 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 979 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 980 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 981 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12" 982 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'0:"0:test5010PC12"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                               |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                                    |
//| F0   | 982 | R0 DATA |                                                                                                                                    |
//| F0+E | 982 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0)  PL\ |
//|      |     |         | I:GSAI:hpr_sysexit                                                                                                                 |
//| F0   | 981 | R0 DATA |                                                                                                                                    |
//| F0+E | 981 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0)      |
//| F0   | 980 | R0 DATA |                                                                                                                                    |
//| F0+E | 980 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0)  PL\ |
//|      |     |         | I:sp: data sum %d                                                                                                                  |
//| F0   | 979 | R0 DATA |                                                                                                                                    |
//| F0+E | 979 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(0) test50.T404.test50.secondProcess.T404.test50.sec\ |
//|      |     |         | ondProcess.V_1 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0)          |
//| F0   | 978 | R0 DATA |                                                                                                                                    |
//| F0+E | 978 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F0 write(0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0) test50.\ |
//|      |     |         | sum te=te:F0 write(0)                                                                                                              |
//| F0   | 977 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F0 read(0)                                                                 |
//| F1   | 977 | R1 DATA |                                                                                                                                    |
//| F1+E | 977 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F1 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F1 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F1 write(0)  PLI:sp\ |
//|      |     |         | : Print data: shar...                                                                                                              |
//| F0   | 976 | R0 DATA |                                                                                                                                    |
//| F0+E | 976 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50\ |
//|      |     |         | .secondProcess.V_1 te=te:F0 write(S32'0) test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F0 write(S32'0) tes\ |
//|      |     |         | t50.command2 te=te:F0 write(S16'73)                                                                                                |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'2:"2:test5010PC12" 966 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'2:"2:test5010PC12" 967 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'2:"2:test5010PC12" 968 :  major_start_pcl=2   edge_private_start/end=3/3 exec=3 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'2:"2:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'2:"2:test5010PC12"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                           |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------*
//| F2   | -   | R0 CTRL |                                                                                                                                                |
//| F2   | 968 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F2 read(E1)                                                                            |
//| F3   | 968 | R1 DATA |                                                                                                                                                |
//| F3+E | 968 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F3 write(E1)  PLI:sp: Print data: shar...                                 |
//| F2   | 967 | R0 DATA |                                                                                                                                                |
//| F2+E | 967 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F2 write(E1) test50.command2 te=te:F2 write(S16'73)  PLI:GSAI:hpr_sysexit |
//| F2   | 966 | R0 DATA |                                                                                                                                                |
//| F2+E | 966 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F2 write(E1) test50.command2 te=te:F2 write(S16'73)                       |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'4:"4:test5010PC12" 963 :  major_start_pcl=4   edge_private_start/end=7/7 exec=7 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'4:"4:test5010PC12" 964 :  major_start_pcl=4   edge_private_start/end=6/6 exec=6 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'4:"4:test5010PC12" 965 :  major_start_pcl=4   edge_private_start/end=5/5 exec=5 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'4:"4:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'4:"4:test5010PC12"
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                               |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| F4   | -   | R0 CTRL |                                                                                                                    |
//| F4   | 965 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F4 read(E2)                                                |
//| F5   | 965 | R1 DATA |                                                                                                                    |
//| F5+E | 965 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1 te=te:F5 write(E3) test50.sum te=te:F5 write(E4)    |
//| F4   | 964 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F4 read(E2)                                                |
//| F6   | 964 | R1 DATA |                                                                                                                    |
//| F6+E | 964 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1 te=te:F6 write(E3) test50.sum te=te:F6 write(E4) t\ |
//|      |     |         | est50.command2 te=te:F6 write(S16'73)  PLI:GSAI:hpr_sysexit                                                        |
//| F4   | 963 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F4 read(E2)                                                |
//| F7   | 963 | R1 DATA |                                                                                                                    |
//| F7+E | 963 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1 te=te:F7 write(E3) test50.sum te=te:F7 write(E4) t\ |
//|      |     |         | est50.command2 te=te:F7 write(S16'73)                                                                              |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'8:"8:test5010PC12" 960 :  major_start_pcl=8   edge_private_start/end=11/11 exec=8 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'8:"8:test5010PC12" 961 :  major_start_pcl=8   edge_private_start/end=10/10 exec=8 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'8:"8:test5010PC12" 962 :  major_start_pcl=8   edge_private_start/end=9/9 exec=8 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'8:"8:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'8:"8:test5010PC12"
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                            |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| F8   | -   | R0 CTRL |                                                                                                                                                 |
//| F8   | 962 | R0 DATA |                                                                                                                                                 |
//| F8+E | 962 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F8 write(E5) i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F8 w\ |
//|      |     |         | rite(E6, E7)                                                                                                                                    |
//| F9   | 962 | W1 DATA |                                                                                                                                                 |
//| F8   | 961 | R0 DATA |                                                                                                                                                 |
//| F8+E | 961 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F8 write(E5) i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F8 w\ |
//|      |     |         | rite(E6, E7) test50.command2 te=te:F8 write(S16'73)  PLI:GSAI:hpr_sysexit                                                                       |
//| F10  | 961 | W1 DATA |                                                                                                                                                 |
//| F8   | 960 | R0 DATA |                                                                                                                                                 |
//| F8+E | 960 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F8 write(E5) i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F8 w\ |
//|      |     |         | rite(E6, E7) test50.command2 te=te:F8 write(S16'73)                                                                                             |
//| F11  | 960 | W1 DATA |                                                                                                                                                 |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 969 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 970 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 971 :  major_start_pcl=12   edge_private_start/end=13/13 exec=13 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 972 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 973 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 974 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12" 975 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'1:"1:test5010PC12"
//*-------+-----+---------+-----------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                            |
//*-------+-----+---------+-----------------------------------------------------------------------------------------------------------------*
//| F12   | -   | R0 CTRL |                                                                                                                 |
//| F12   | 975 | R0 DATA |                                                                                                                 |
//| F12+E | 975 | W0 DATA |                                                                                                                 |
//| F12   | 974 | R0 DATA |                                                                                                                 |
//| F12+E | 974 | W0 DATA |  PLI:sp: data sum %d                                                                                            |
//| F12   | 973 | R0 DATA |                                                                                                                 |
//| F12+E | 973 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0 te=te:F12 write(0)                               |
//| F12   | 972 | R0 DATA |                                                                                                                 |
//| F12+E | 972 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1 te=te:F12 write(0) test50.sum te=te:F12 write(0) |
//| F12   | 971 | R0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F12 read(0)                                             |
//| F13   | 971 | R1 DATA |                                                                                                                 |
//| F13+E | 971 | W0 DATA | test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2 te=te:F13 write(0)  PLI:sp: Print data: shar...  |
//| F12   | 970 | R0 DATA |                                                                                                                 |
//| F12+E | 970 | W0 DATA | test50.command2 te=te:F12 write(S16'73)  PLI:GSAI:hpr_sysexit                                                   |
//| F12   | 969 | R0 DATA |                                                                                                                 |
//| F12+E | 969 | W0 DATA | test50.command2 te=te:F12 write(S16'73)                                                                         |
//*-------+-----+---------+-----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'16:"16:test5010PC12" 958 :  major_start_pcl=14   edge_private_start/end=-1/-1 exec=14 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC12 state=XU32'16:"16:test5010PC12" 959 :  major_start_pcl=14   edge_private_start/end=-1/-1 exec=14 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC12 state=XU32'16:"16:test5010PC12"
//res2: nopipeline: Thread=test5010PC12 state=XU32'16:"16:test5010PC12"
//*-------+-----+---------+---------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                          |
//*-------+-----+---------+---------------------------------------------------------------*
//| F14   | -   | R0 CTRL |                                                               |
//| F14   | 959 | R0 DATA |                                                               |
//| F14+E | 959 | W0 DATA | test50.command2 te=te:F14 write(S16'73)  PLI:GSAI:hpr_sysexit |
//| F14   | 958 | R0 DATA |                                                               |
//| F14+E | 958 | W0 DATA | test50.command2 te=te:F14 write(S16'73)                       |
//*-------+-----+---------+---------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for test5010PC10 
//*-----------------------------------+------+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                     | eno  | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------+------+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:test5010PC10"           | 1022 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:test5010PC10"           | 1021 | 1       | hwm=0.0.1   | 1    |        | 2     | 2   | 3    |
//| XU32'2:"2:test5010PC10"           | 1019 | 3       | hwm=0.0.1   | 3    |        | 5     | 5   | 3    |
//| XU32'2:"2:test5010PC10"           | 1020 | 3       | hwm=0.0.1   | 3    |        | 4     | 4   | 6    |
//| XU32'4:"4:test5010PC10"           | 1017 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 7    |
//| XU32'4:"4:test5010PC10"           | 1018 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 8    |
//| XU32'8:"8:test5010PC10"           | 1015 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 7    |
//| XU32'8:"8:test5010PC10"           | 1016 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 8    |
//| XU32'16:"16:test5010PC10"         | 1013 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 26   |
//| XU32'16:"16:test5010PC10"         | 1014 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 27   |
//| XU32'64:"64:test5010PC10"         | 1009 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 9    |
//| XU32'64:"64:test5010PC10"         | 1010 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'128:"128:test5010PC10"       | 1007 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'128:"128:test5010PC10"       | 1008 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 12   |
//| XU32'256:"256:test5010PC10"       | 1005 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 11   |
//| XU32'256:"256:test5010PC10"       | 1006 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 12   |
//| XU32'512:"512:test5010PC10"       | 1003 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 13   |
//| XU32'512:"512:test5010PC10"       | 1004 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 14   |
//| XU32'1024:"1024:test5010PC10"     | 1001 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 13   |
//| XU32'1024:"1024:test5010PC10"     | 1002 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 14   |
//| XU32'2048:"2048:test5010PC10"     | 999  | 14      | hwm=0.0.0   | 14   |        | -     | -   | 15   |
//| XU32'2048:"2048:test5010PC10"     | 1000 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 16   |
//| XU32'4096:"4096:test5010PC10"     | 997  | 15      | hwm=0.0.0   | 15   |        | -     | -   | 15   |
//| XU32'4096:"4096:test5010PC10"     | 998  | 15      | hwm=0.0.0   | 15   |        | -     | -   | 16   |
//| XU32'8192:"8192:test5010PC10"     | 995  | 16      | hwm=0.0.0   | 16   |        | -     | -   | 17   |
//| XU32'8192:"8192:test5010PC10"     | 996  | 16      | hwm=0.0.0   | 16   |        | -     | -   | 18   |
//| XU32'16384:"16384:test5010PC10"   | 993  | 17      | hwm=0.0.0   | 17   |        | -     | -   | 17   |
//| XU32'16384:"16384:test5010PC10"   | 994  | 17      | hwm=0.0.0   | 17   |        | -     | -   | 18   |
//| XU32'32768:"32768:test5010PC10"   | 991  | 18      | hwm=0.0.1   | 18   |        | 19    | 19  | 21   |
//| XU32'32768:"32768:test5010PC10"   | 992  | 18      | hwm=0.0.0   | 18   |        | -     | -   | 24   |
//| XU32'131072:"131072:test5010PC10" | 987  | 20      | hwm=0.0.0   | 20   |        | -     | -   | 26   |
//| XU32'131072:"131072:test5010PC10" | 988  | 20      | hwm=0.0.0   | 20   |        | -     | -   | -    |
//| XU32'65536:"65536:test5010PC10"   | 989  | 21      | hwm=0.0.1   | 21   |        | 23    | 23  | 21   |
//| XU32'65536:"65536:test5010PC10"   | 990  | 21      | hwm=0.0.1   | 21   |        | 22    | 22  | 20   |
//| XU32'262144:"262144:test5010PC10" | 985  | 24      | hwm=0.0.1   | 24   |        | 25    | 25  | 21   |
//| XU32'262144:"262144:test5010PC10" | 986  | 24      | hwm=0.0.0   | 24   |        | -     | -   | 24   |
//| XU32'32:"32:test5010PC10"         | 1011 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 9    |
//| XU32'32:"32:test5010PC10"         | 1012 | 26      | hwm=0.0.0   | 26   |        | -     | -   | 10   |
//| XU32'524288:"524288:test5010PC10" | 983  | 27      | hwm=0.0.0   | 27   |        | -     | -   | 26   |
//| XU32'524288:"524288:test5010PC10" | 984  | 27      | hwm=0.0.0   | 27   |        | -     | -   | 27   |
//*-----------------------------------+------+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'0:"0:test5010PC10" 1022 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'0:"0:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'0:"0:test5010PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                        |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                             |
//| F0   | 1022 | R0 DATA |                                                                                                                                                             |
//| F0+E | 1022 | W0 DATA | test50.sum te=te:F0 write(S32'12345678) test50.command2 te=te:F0 write(S16'120) T403.test50.clearto.0.10.V_0 te=te:F0 write(S32'0) T403.test50.clearto.0.1\ |
//|      |      |         | 0.V_1 te=te:F0 write(S32'0) T403.test50.test50_phase0.0.12.V_0 te=te:F0 write(S32'0) T403.test50.clearto.25.3.V_0 te=te:F0 write(S32'0) T403.test50.cleart\ |
//|      |      |         | o.25.3.V_1 te=te:F0 write(S32'0) ksubsAbendSyndrome te=te:F0 write(U8'128) ksubsGpioLeds te=te:F0 write(U8'128) ksubsManualWaypoint te=te:F0 write(U8'0) t\ |
//|      |      |         | est50.exiting te=te:F0 write(U1'0)  PLI:Kiwi Demo - Test50 s...                                                                                             |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'1:"1:test5010PC10" 1021 :  major_start_pcl=1   edge_private_start/end=2/2 exec=1 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'1:"1:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'1:"1:test5010PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                     |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                                          |
//| F1   | 1021 | R0 DATA |                                                                                                                                                          |
//| F1+E | 1021 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F1 write(0, S32'30) T403.test50.clearto.0.10.V_0 te=te:F1 write(31) T403.test50.clearto.0.10.V_\ |
//|      |      |         | 1 te=te:F1 write(0)  PLI:  Test50 Remote Stat...  PLI:Kiwi Demo - Test50 p...                                                                            |
//| F2   | 1021 | W1 DATA |                                                                                                                                                          |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'2:"2:test5010PC10" 1019 :  major_start_pcl=3   edge_private_start/end=5/5 exec=3 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'2:"2:test5010PC10" 1020 :  major_start_pcl=3   edge_private_start/end=4/4 exec=3 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'2:"2:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'2:"2:test5010PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                             |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                  |
//| F3   | 1020 | R0 DATA |                                                                                                                  |
//| F3+E | 1020 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F3 write(29, 99) T403.test50.clearto.0.10.V_1 te=te:F3 \ |
//|      |      |         | write(E8)                                                                                                        |
//| F4   | 1020 | W1 DATA |                                                                                                                  |
//| F3   | 1019 | R0 DATA |                                                                                                                  |
//| F3+E | 1019 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F3 write(E8, E9) T403.test50.clearto.0.10.V_0 te=te:F3 \ |
//|      |      |         | write(E10) T403.test50.clearto.0.10.V_1 te=te:F3 write(E8)                                                       |
//| F5   | 1019 | W1 DATA |                                                                                                                  |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'4:"4:test5010PC10" 1017 :  major_start_pcl=6   edge_private_start/end=-1/-1 exec=6 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'4:"4:test5010PC10" 1018 :  major_start_pcl=6   edge_private_start/end=-1/-1 exec=6 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'4:"4:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'4:"4:test5010PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F6   | -    | R0 CTRL |                                        |
//| F6   | 1018 | R0 DATA |                                        |
//| F6+E | 1018 | W0 DATA | test50.command2 te=te:F6 write(S16'68) |
//| F6   | 1017 | R0 DATA |                                        |
//| F6+E | 1017 | W0 DATA |                                        |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'8:"8:test5010PC10" 1015 :  major_start_pcl=7   edge_private_start/end=-1/-1 exec=7 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'8:"8:test5010PC10" 1016 :  major_start_pcl=7   edge_private_start/end=-1/-1 exec=7 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'8:"8:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'8:"8:test5010PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F7   | -    | R0 CTRL |                                        |
//| F7   | 1016 | R0 DATA |                                        |
//| F7+E | 1016 | W0 DATA | test50.command2 te=te:F7 write(S16'68) |
//| F7   | 1015 | R0 DATA |                                        |
//| F7+E | 1015 | W0 DATA |                                        |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'16:"16:test5010PC10" 1013 :  major_start_pcl=8   edge_private_start/end=-1/-1 exec=8 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'16:"16:test5010PC10" 1014 :  major_start_pcl=8   edge_private_start/end=-1/-1 exec=8 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'16:"16:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'16:"16:test5010PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                     |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                                                                          |
//| F8   | 1014 | R0 DATA |                                                                                                                          |
//| F8+E | 1014 | W0 DATA |                                                                                                                          |
//| F8   | 1013 | R0 DATA |                                                                                                                          |
//| F8+E | 1013 | W0 DATA | test50.command2 te=te:F8 write(S16'80) T403.test50.test50_phase0.0.12.V_0 te=te:F8 write(0)  PLI:  Test50 fancy itera... |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'64:"64:test5010PC10" 1009 :  major_start_pcl=9   edge_private_start/end=-1/-1 exec=9 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'64:"64:test5010PC10" 1010 :  major_start_pcl=9   edge_private_start/end=-1/-1 exec=9 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'64:"64:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'64:"64:test5010PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F9   | -    | R0 CTRL |                                        |
//| F9   | 1010 | R0 DATA |                                        |
//| F9+E | 1010 | W0 DATA | test50.command2 te=te:F9 write(S16'83) |
//| F9   | 1009 | R0 DATA |                                        |
//| F9+E | 1009 | W0 DATA |                                        |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'128:"128:test5010PC10" 1007 :  major_start_pcl=10   edge_private_start/end=-1/-1 exec=10 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'128:"128:test5010PC10" 1008 :  major_start_pcl=10   edge_private_start/end=-1/-1 exec=10 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'128:"128:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'128:"128:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F10   | -    | R0 CTRL |                                         |
//| F10   | 1008 | R0 DATA |                                         |
//| F10+E | 1008 | W0 DATA | test50.command2 te=te:F10 write(S16'80) |
//| F10   | 1007 | R0 DATA |                                         |
//| F10+E | 1007 | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'256:"256:test5010PC10" 1005 :  major_start_pcl=11   edge_private_start/end=-1/-1 exec=11 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'256:"256:test5010PC10" 1006 :  major_start_pcl=11   edge_private_start/end=-1/-1 exec=11 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'256:"256:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'256:"256:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F11   | -    | R0 CTRL |                                         |
//| F11   | 1006 | R0 DATA |                                         |
//| F11+E | 1006 | W0 DATA | test50.command2 te=te:F11 write(S16'80) |
//| F11   | 1005 | R0 DATA |                                         |
//| F11+E | 1005 | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'512:"512:test5010PC10" 1003 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'512:"512:test5010PC10" 1004 :  major_start_pcl=12   edge_private_start/end=-1/-1 exec=12 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'512:"512:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'512:"512:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F12   | -    | R0 CTRL |                                         |
//| F12   | 1004 | R0 DATA |                                         |
//| F12+E | 1004 | W0 DATA | test50.command2 te=te:F12 write(S16'85) |
//| F12   | 1003 | R0 DATA |                                         |
//| F12+E | 1003 | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'1024:"1024:test5010PC10" 1001 :  major_start_pcl=13   edge_private_start/end=-1/-1 exec=13 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'1024:"1024:test5010PC10" 1002 :  major_start_pcl=13   edge_private_start/end=-1/-1 exec=13 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'1024:"1024:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'1024:"1024:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F13   | -    | R0 CTRL |                                         |
//| F13   | 1002 | R0 DATA |                                         |
//| F13+E | 1002 | W0 DATA | test50.command2 te=te:F13 write(S16'85) |
//| F13   | 1001 | R0 DATA |                                         |
//| F13+E | 1001 | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'2048:"2048:test5010PC10" 999 :  major_start_pcl=14   edge_private_start/end=-1/-1 exec=14 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'2048:"2048:test5010PC10" 1000 :  major_start_pcl=14   edge_private_start/end=-1/-1 exec=14 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'2048:"2048:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'2048:"2048:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F14   | -    | R0 CTRL |                                         |
//| F14   | 1000 | R0 DATA |                                         |
//| F14+E | 1000 | W0 DATA | test50.command2 te=te:F14 write(S16'83) |
//| F14   | 999  | R0 DATA |                                         |
//| F14+E | 999  | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'4096:"4096:test5010PC10" 997 :  major_start_pcl=15   edge_private_start/end=-1/-1 exec=15 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'4096:"4096:test5010PC10" 998 :  major_start_pcl=15   edge_private_start/end=-1/-1 exec=15 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'4096:"4096:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'4096:"4096:test5010PC10"
//*-------+-----+---------+-----------------------------------------*
//| pc    | eno | Phaser  | Work                                    |
//*-------+-----+---------+-----------------------------------------*
//| F15   | -   | R0 CTRL |                                         |
//| F15   | 998 | R0 DATA |                                         |
//| F15+E | 998 | W0 DATA | test50.command2 te=te:F15 write(S16'83) |
//| F15   | 997 | R0 DATA |                                         |
//| F15+E | 997 | W0 DATA |                                         |
//*-------+-----+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'8192:"8192:test5010PC10" 995 :  major_start_pcl=16   edge_private_start/end=-1/-1 exec=16 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'8192:"8192:test5010PC10" 996 :  major_start_pcl=16   edge_private_start/end=-1/-1 exec=16 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'8192:"8192:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'8192:"8192:test5010PC10"
//*-------+-----+---------+-----------------------------------------*
//| pc    | eno | Phaser  | Work                                    |
//*-------+-----+---------+-----------------------------------------*
//| F16   | -   | R0 CTRL |                                         |
//| F16   | 996 | R0 DATA |                                         |
//| F16+E | 996 | W0 DATA | test50.command2 te=te:F16 write(S16'80) |
//| F16   | 995 | R0 DATA |                                         |
//| F16+E | 995 | W0 DATA |                                         |
//*-------+-----+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'16384:"16384:test5010PC10" 993 :  major_start_pcl=17   edge_private_start/end=-1/-1 exec=17 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'16384:"16384:test5010PC10" 994 :  major_start_pcl=17   edge_private_start/end=-1/-1 exec=17 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'16384:"16384:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'16384:"16384:test5010PC10"
//*-------+-----+---------+-----------------------------------------*
//| pc    | eno | Phaser  | Work                                    |
//*-------+-----+---------+-----------------------------------------*
//| F17   | -   | R0 CTRL |                                         |
//| F17   | 994 | R0 DATA |                                         |
//| F17+E | 994 | W0 DATA | test50.command2 te=te:F17 write(S16'80) |
//| F17   | 993 | R0 DATA |                                         |
//| F17+E | 993 | W0 DATA |                                         |
//*-------+-----+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'32768:"32768:test5010PC10" 991 :  major_start_pcl=18   edge_private_start/end=19/19 exec=18 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'32768:"32768:test5010PC10" 992 :  major_start_pcl=18   edge_private_start/end=-1/-1 exec=18 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'32768:"32768:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'32768:"32768:test5010PC10"
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                               |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| F18   | -   | R0 CTRL |                                                                                                                    |
//| F18   | 992 | R0 DATA |                                                                                                                    |
//| F18+E | 992 | W0 DATA |                                                                                                                    |
//| F18   | 991 | R0 DATA |                                                                                                                    |
//| F18+E | 991 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F18 write(0, E11) T403.test50.clearto.25.3.V_0 te=te:F18 \ |
//|       |     |         | write(E12) T403.test50.clearto.25.3.V_1 te=te:F18 write(0)                                                         |
//| F19   | 991 | W1 DATA |                                                                                                                    |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'131072:"131072:test5010PC10" 987 :  major_start_pcl=20   edge_private_start/end=-1/-1 exec=20 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'131072:"131072:test5010PC10" 988 :  major_start_pcl=20   edge_private_start/end=-1/-1 exec=20 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'131072:"131072:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'131072:"131072:test5010PC10"
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                                           |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| F20   | -   | R0 CTRL |                                                                                                                                |
//| F20   | 988 | R0 DATA |                                                                                                                                |
//| F20+E | 988 | W0 DATA | T403.test50.test50_phase0.0.12.V_0 te=te:F20 write(E13) test50.exiting te=te:F20 write(U1'1)  PLI:GSAI:hpr_sysexit  PLI:Test5\ |
//|       |     |         | 0 done.  PLI:Test50 starting join...  PLI:Finished main proces...                                                              |
//| F20   | 987 | R0 DATA |                                                                                                                                |
//| F20+E | 987 | W0 DATA | test50.command2 te=te:F20 write(S16'80) T403.test50.test50_phase0.0.12.V_0 te=te:F20 write(E13)  PLI:  Test50 fancy itera...   |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'65536:"65536:test5010PC10" 989 :  major_start_pcl=21   edge_private_start/end=23/23 exec=21 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'65536:"65536:test5010PC10" 990 :  major_start_pcl=21   edge_private_start/end=22/22 exec=21 (dend=1)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'65536:"65536:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'65536:"65536:test5010PC10"
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                                 |
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------*
//| F21   | -   | R0 CTRL |                                                                                                                      |
//| F21   | 990 | R0 DATA |                                                                                                                      |
//| F21+E | 990 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F21 write(29, 99) T403.test50.clearto.25.3.V_1 te=te:F21 wr\ |
//|       |     |         | ite(E14)  PLI:   point2 %c %d.                                                                                       |
//| F22   | 990 | W1 DATA |                                                                                                                      |
//| F21   | 989 | R0 DATA |                                                                                                                      |
//| F21+E | 989 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F21 write(E14, E15) T403.test50.clearto.25.3.V_0 te=te:F21 \ |
//|       |     |         | write(E16) T403.test50.clearto.25.3.V_1 te=te:F21 write(E14)                                                         |
//| F23   | 989 | W1 DATA |                                                                                                                      |
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'262144:"262144:test5010PC10" 985 :  major_start_pcl=24   edge_private_start/end=25/25 exec=24 (dend=1)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'262144:"262144:test5010PC10" 986 :  major_start_pcl=24   edge_private_start/end=-1/-1 exec=24 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'262144:"262144:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'262144:"262144:test5010PC10"
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                               |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*
//| F24   | -   | R0 CTRL |                                                                                                                    |
//| F24   | 986 | R0 DATA |                                                                                                                    |
//| F24+E | 986 | W0 DATA |                                                                                                                    |
//| F24   | 985 | R0 DATA |                                                                                                                    |
//| F24+E | 985 | W0 DATA | i_@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0 te=te:F24 write(0, E11) T403.test50.clearto.25.3.V_0 te=te:F24 \ |
//|       |     |         | write(E12) T403.test50.clearto.25.3.V_1 te=te:F24 write(0)                                                         |
//| F25   | 985 | W1 DATA |                                                                                                                    |
//*-------+-----+---------+--------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'32:"32:test5010PC10" 1011 :  major_start_pcl=26   edge_private_start/end=-1/-1 exec=26 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'32:"32:test5010PC10" 1012 :  major_start_pcl=26   edge_private_start/end=-1/-1 exec=26 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'32:"32:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'32:"32:test5010PC10"
//*-------+------+---------+-----------------------------------------*
//| pc    | eno  | Phaser  | Work                                    |
//*-------+------+---------+-----------------------------------------*
//| F26   | -    | R0 CTRL |                                         |
//| F26   | 1012 | R0 DATA |                                         |
//| F26+E | 1012 | W0 DATA | test50.command2 te=te:F26 write(S16'83) |
//| F26   | 1011 | R0 DATA |                                         |
//| F26+E | 1011 | W0 DATA |                                         |
//*-------+------+---------+-----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'524288:"524288:test5010PC10" 983 :  major_start_pcl=27   edge_private_start/end=-1/-1 exec=27 (dend=0)
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=test5010PC10 state=XU32'524288:"524288:test5010PC10" 984 :  major_start_pcl=27   edge_private_start/end=-1/-1 exec=27 (dend=0)
//Schedule for res2: nopipeline: Thread=test5010PC10 state=XU32'524288:"524288:test5010PC10"
//res2: nopipeline: Thread=test5010PC10 state=XU32'524288:"524288:test5010PC10"
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno | Phaser  | Work                                                                                                                       |
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F27   | -   | R0 CTRL |                                                                                                                            |
//| F27   | 984 | R0 DATA |                                                                                                                            |
//| F27+E | 984 | W0 DATA |                                                                                                                            |
//| F27   | 983 | R0 DATA |                                                                                                                            |
//| F27+E | 983 | W0 DATA | test50.command2 te=te:F27 write(S16'80) T403.test50.test50_phase0.0.12.V_0 te=te:F27 write(0)  PLI:  Test50 fancy itera... |
//*-------+-----+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= 1+test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2
//
//  E2 =.= test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1
//
//  E3 =.= 1+test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1
//
//  E4 =.= test50.sum+@_SINT/CC/sharedData__SCALbx10_sharedData__ARA0[test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1]
//
//  E5 =.= 1+test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0
//
//  E6 =.= test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0
//
//  E7 =.= test50.sum+test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0
//
//  E8 =.= 1+T403.test50.clearto.0.10.V_1
//
//  E9 =.= C(T403.test50.clearto.0.10.V_0)
//
//  E10 =.= 1+(C(T403.test50.clearto.0.10.V_0))
//
//  E11 =.= C(40+T403.test50.test50_phase0.0.12.V_0)
//
//  E12 =.= 1+(C(40+T403.test50.test50_phase0.0.12.V_0))
//
//  E13 =.= 1+T403.test50.test50_phase0.0.12.V_0
//
//  E14 =.= 1+T403.test50.clearto.25.3.V_1
//
//  E15 =.= C(T403.test50.clearto.25.3.V_0)
//
//  E16 =.= 1+(C(T403.test50.clearto.25.3.V_0))
//
//  E17 =.= {[73==test50.command2, |test50.exiting|]}
//
//  E18 =.= {[|test50.exiting|, 80==test50.command2]}
//
//  E19 =.= {[|test50.exiting|, 85==test50.command2]}
//
//  E20 =.= {[|test50.exiting|, 83==test50.command2]}
//
//  E21 =.= {[|test50.exiting|, 68==test50.command2]}
//
//  E22 =.= {[73!=test50.command2, |test50.exiting|, 85!=test50.command2, 83!=test50.command2, 80!=test50.command2, 68!=test50.command2]}
//
//  E23 =.= test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2<29
//
//  E24 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2>=29]}
//
//  E25 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_2>=29]}
//
//  E26 =.= test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1<29
//
//  E27 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1>=29]}
//
//  E28 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_1>=29]}
//
//  E29 =.= test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0<29
//
//  E30 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0>=29]}
//
//  E31 =.= {[|test50.exiting|, test50.T404.test50.secondProcess.T404.test50.secondProcess.V_0>=29]}
//
//  E32 =.= {[73!=test50.command2, |test50.exiting|, 85!=test50.command2, 83!=test50.command2, 80!=test50.command2, 68!=test50.command2]; [73==test50.command2, |test50.exiting|]}
//
//  E33 =.= T403.test50.clearto.0.10.V_1>=29
//
//  E34 =.= T403.test50.clearto.0.10.V_1<29
//
//  E35 =.= T403.test50.test50_phase0.0.12.V_0>=2
//
//  E36 =.= T403.test50.test50_phase0.0.12.V_0<2
//
//  E37 =.= T403.test50.clearto.25.3.V_1>=29
//
//  E38 =.= T403.test50.clearto.25.3.V_1<29
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test50 to test50

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory:
//1 vectors of width 4
//
//5 vectors of width 1
//
//12 vectors of width 32
//
//3 vectors of width 5
//
//1 vectors of width 16
//
//Total state bits in module = 424 bits.
//
//65 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem/Kiwi/.cctor uid=cctor14 has 6 CIL instructions in 1 basic blocks
//Thread System/BitConverter/.cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread test50/.cctor uid=cctor10 has 8 CIL instructions in 1 basic blocks
//Thread test50/Main uid=Main10 has 115 CIL instructions in 34 basic blocks
//Thread test50/secondProcess uid=secondProcess10 has 54 CIL instructions in 24 basic blocks
//Thread mpc10 has 21 bevelab control states (pauses)
//Thread mpc12 has 6 bevelab control states (pauses)
//Reindexed thread test5010PC12 with 15 minor control states
//Reindexed thread test5010PC10 with 28 minor control states
// eof (HPR L/S Verilog)
