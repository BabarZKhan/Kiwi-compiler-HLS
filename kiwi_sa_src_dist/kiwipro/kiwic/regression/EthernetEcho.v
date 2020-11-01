

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:58
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable EthernetEcho.exe
`timescale 1ns/1ns


module EthernetEcho(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=kiwicmiscio10 */
input rx_eof_n,
    input [7:0] rx_data,
    input rx_src_rdy_n,
    input rx_sof_n,
    output reg [7:0] tx_data,
    output reg tx_eof_n,
    output reg tx_src_rdy_n,
    output reg tx_sof_n,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiLOCAOINT4001PC10nz_pc_export,
    
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

function [7:0] rtl_unsigned_bitextract3;
   input [31:0] arg;
   rtl_unsigned_bitextract3 = $unsigned(arg[7:0]);
   endfunction


function  rtl_unsigned_bitextract1;
   input [31:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[0:0]);
   endfunction


function [31:0] rtl_unsigned_extend2;
   input [7:0] arg;
   rtl_unsigned_extend2 = { 24'b0, arg[7:0] };
   endfunction


function [31:0] rtl_unsigned_extend0;
   input argbit;
   rtl_unsigned_extend0 = { 31'b0, argbit };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3;
  reg signed [31:0] LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2;
  reg signed [31:0] LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1;
  reg LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0;
  reg LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_257;
  reg LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_256;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire [7:0] A_8_US_CC_SCALbx10_ARA0_rdata;
  reg [9:0] A_8_US_CC_SCALbx10_ARA0_addr;
  reg A_8_US_CC_SCALbx10_ARA0_wen;
  reg A_8_US_CC_SCALbx10_ARA0_ren;
  reg [7:0] A_8_US_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg [7:0] Z8USCCSCALbx10ARA0rdatah10hold;
  reg Z8USCCSCALbx10ARA0rdatah10shot0;
  reg [4:0] kiwiLOCAOINT4001PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.LOCAOINT400_1/1.0
      if (reset)  begin 
               tx_data <= 32'd0;
               kiwiLOCAOINT4001PC10nz <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3 <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1 <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0 <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_257 <= 32'd0;
               tx_sof_n <= 32'd0;
               tx_src_rdy_n <= 32'd0;
               tx_eof_n <= 32'd0;
               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_256 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiLOCAOINT4001PC10nz)
                  32'h0/*0:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h1/*1:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= 32'sh0;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3 <= 32'h0;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1 <= 32'sh0;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0 <= 32'h0;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_257 <= 32'h0;
                           tx_sof_n <= 32'h1;
                           tx_src_rdy_n <= 32'h1;
                           tx_eof_n <= 32'h1;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_256 <= (rx_sof_n? 32'h0: rtl_unsigned_extend0(!rx_src_rdy_n
                          ));

                           end 
                          
                  32'h1/*1:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'ha/*10:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_256
                          ));

                           end 
                          
                  32'h2/*2:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h5/*5:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3 <= rtl_unsigned_extend0(!rx_eof_n);
                           end 
                          
                  32'h4/*4:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiLOCAOINT4001PC10nz <= 32'h2/*2:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'h5/*5:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiLOCAOINT4001PC10nz <= 32'h3/*3:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'h6/*6:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h9/*9:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= $signed(32'sd1+LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                          );

                           end 
                          
                  32'h8/*8:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2==-32'sd1+LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1
                          ))  tx_eof_n <= 32'h0;
                               kiwiLOCAOINT4001PC10nz <= 32'h6/*6:kiwiLOCAOINT4001PC10nz*/;
                           tx_data <= ((32'h8/*8:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz)? rtl_unsigned_extend2(rtl_unsigned_bitextract3(A_8_US_CC_SCALbx10_ARA0_rdata
                          )): rtl_unsigned_extend2(rtl_unsigned_bitextract3(Z8USCCSCALbx10ARA0rdatah10hold)));

                           end 
                          
                  32'h9/*9:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiLOCAOINT4001PC10nz <= 32'h7/*7:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'ha/*10:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiLOCAOINT4001PC10nz <= 32'h13/*19:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'hb/*11:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiLOCAOINT4001PC10nz <= 32'ha/*10:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'hd/*13:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'he/*14:kiwiLOCAOINT4001PC10nz*/;
                           tx_data <= ((32'hd/*13:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz)? rtl_unsigned_extend2(rtl_unsigned_bitextract3(A_8_US_CC_SCALbx10_ARA0_rdata
                          )): rtl_unsigned_extend2(rtl_unsigned_bitextract3(Z8USCCSCALbx10ARA0rdatah10hold)));

                           end 
                          
                  32'h7/*7:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                      <LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1))  kiwiLOCAOINT4001PC10nz <= 32'h8/*8:kiwiLOCAOINT4001PC10nz*/;
                           else  begin 
                               kiwiLOCAOINT4001PC10nz <= 32'hb/*11:kiwiLOCAOINT4001PC10nz*/;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0 <= 32'h0;
                               tx_src_rdy_n <= 32'h1;
                               tx_eof_n <= 32'h1;
                               end 
                              
                  32'he/*14:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'hc/*12:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= $signed(32'sd1+LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                          );

                           end 
                          
                  32'h10/*16:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h11/*17:kiwiLOCAOINT4001PC10nz*/;
                           tx_data <= ((32'h10/*16:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz)? rtl_unsigned_extend2(rtl_unsigned_bitextract3(A_8_US_CC_SCALbx10_ARA0_rdata
                          )): rtl_unsigned_extend2(rtl_unsigned_bitextract3(Z8USCCSCALbx10ARA0rdatah10hold)));

                           tx_sof_n <= rtl_unsigned_extend0((32'h0/*0:USA10*/==(32'sd6==LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                          )));

                           end 
                          
                  32'hc/*12:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                      <32'sd6))  kiwiLOCAOINT4001PC10nz <= 32'hd/*13:kiwiLOCAOINT4001PC10nz*/;
                           else  begin 
                               kiwiLOCAOINT4001PC10nz <= 32'h7/*7:kiwiLOCAOINT4001PC10nz*/;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= 32'shc;
                               end 
                              
                  32'hf/*15:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                      <32'sd12))  kiwiLOCAOINT4001PC10nz <= 32'h10/*16:kiwiLOCAOINT4001PC10nz*/;
                           else  begin 
                               kiwiLOCAOINT4001PC10nz <= 32'hc/*12:kiwiLOCAOINT4001PC10nz*/;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= 32'sh0;
                               end 
                              
                  32'h11/*17:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'hf/*15:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= $signed(32'sd1+LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                          );

                           end 
                          
                  32'h12/*18:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h14/*20:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_257 <= (rx_sof_n? 32'h0: rtl_unsigned_extend0(!rx_src_rdy_n
                          ));

                           end 
                          
                  32'h3/*3:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18) if (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3
                      )  begin 
                               kiwiLOCAOINT4001PC10nz <= 32'hf/*15:kiwiLOCAOINT4001PC10nz*/;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2 <= 32'sh6;
                               tx_sof_n <= 32'h1;
                               tx_src_rdy_n <= 32'h0;
                               end 
                               else  begin 
                              if (!rx_src_rdy_n)  LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1 <= $signed(32'sd1+LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1
                                  );

                                   kiwiLOCAOINT4001PC10nz <= 32'h4/*4:kiwiLOCAOINT4001PC10nz*/;
                               end 
                              
                  32'h13/*19:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18) if (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0
                      )  begin 
                               kiwiLOCAOINT4001PC10nz <= 32'h3/*3:kiwiLOCAOINT4001PC10nz*/;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3 <= 32'h0;
                               LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1 <= 32'sh0;
                               end 
                               else  kiwiLOCAOINT4001PC10nz <= 32'h12/*18:kiwiLOCAOINT4001PC10nz*/;
                      
                  32'h14/*20:kiwiLOCAOINT4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiLOCAOINT4001PC10nz <= 32'h13/*19:kiwiLOCAOINT4001PC10nz*/;
                           LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_SPILL_257
                          ));

                           end 
                          endcase
              if (reset)  begin 
               kiwiLOCAOINT4001PC10nz_pc_export <= 32'd0;
               Z8USCCSCALbx10ARA0rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (Z8USCCSCALbx10ARA0rdatah10shot0)  Z8USCCSCALbx10ARA0rdatah10hold <= A_8_US_CC_SCALbx10_ARA0_rdata;
                       kiwiLOCAOINT4001PC10nz_pc_export <= kiwiLOCAOINT4001PC10nz;
                   Z8USCCSCALbx10ARA0rdatah10shot0 <= (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<32'sd12) && (32'hf
                  /*15:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz) || (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
                  <LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1) && (32'h7/*7:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz
                  ) || (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<32'sd6) && (32'hc/*12:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz
                  );

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.LOCAOINT400_1/1.0


       end 
      

 always   @(* )  begin 
       A_8_US_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_8_US_CC_SCALbx10_ARA0_wdata = 32'sd0;
       A_8_US_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_8_US_CC_SCALbx10_ARA0_ren = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiLOCAOINT4001PC10nz)
          32'h7/*7:kiwiLOCAOINT4001PC10nz*/: if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1
          ))  A_8_US_CC_SCALbx10_ARA0_addr = LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2;
              
          32'hc/*12:kiwiLOCAOINT4001PC10nz*/: if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<32'sd6))  A_8_US_CC_SCALbx10_ARA0_addr
               = LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2;

              
          32'hf/*15:kiwiLOCAOINT4001PC10nz*/: if ((LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<32'sd12))  A_8_US_CC_SCALbx10_ARA0_addr
               = LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2;

              
          32'h3/*3:kiwiLOCAOINT4001PC10nz*/: if (!rx_src_rdy_n && !LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3)  begin 
                   A_8_US_CC_SCALbx10_ARA0_addr = LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1;
                   A_8_US_CC_SCALbx10_ARA0_wdata = rtl_unsigned_extend2(rtl_unsigned_bitextract3(rx_data));
                   end 
                  endcase
      if (hpr_int_run_enable_DDX18)  begin 
               A_8_US_CC_SCALbx10_ARA0_wen = !rx_src_rdy_n && !LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_3 && (32'h3/*3:kiwiLOCAOINT4001PC10nz*/==
              kiwiLOCAOINT4001PC10nz);

               A_8_US_CC_SCALbx10_ARA0_ren = (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<32'sd6) && (32'hc/*12:kiwiLOCAOINT4001PC10nz*/==
              kiwiLOCAOINT4001PC10nz) || (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2<LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_1
              ) && (32'h7/*7:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz) || (LOCAOINT400_LocalLinkLoopBackTest_EthernetEcho_echo_1_1_V_2
              <32'sd12) && (32'hf/*15:kiwiLOCAOINT4001PC10nz*/==kiwiLOCAOINT4001PC10nz);

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
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd8),
        .ADDR_WIDTH(32'sd10),
        .WORDS(32'sd1024),
        .LANE_WIDTH(32'sd8
),
        .trace_me(32'sd0)) A_8_US_CC_SCALbx10_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_8_US_CC_SCALbx10_ARA0_rdata
),
        .addr(A_8_US_CC_SCALbx10_ARA0_addr),
        .wen(A_8_US_CC_SCALbx10_ARA0_wen),
        .ren(A_8_US_CC_SCALbx10_ARA0_ren),
        .wdata(A_8_US_CC_SCALbx10_ARA0_wdata
));

// Structural Resource (FU) inventory for EthernetEcho:// 1 vectors of width 5
// 8 vectors of width 1
// 2 vectors of width 8
// 1 vectors of width 10
// 2 vectors of width 32
// Total state bits in module = 103 bits.
// 8 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:55
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable EthernetEcho.exe


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*------------------------------------+---------+----------------------------------------------+---------------+-----------------------------------------------+--------------+-------*
//| Class                              | Style   | Dir Style                                    | Timing Target | Method                                        | UID          | Skip  |
//*------------------------------------+---------+----------------------------------------------+---------------+-----------------------------------------------+--------------+-------*
//| LocalLinkLoopBackTest.EthernetEcho | MM_root | DS_normal self-start/directorate-startmode,\ |               | LocalLinkLoopBackTest.EthernetEcho.EntryPoint | EntryPoint10 | false |
//|                                    |         |  finish/directorate-endmode, enable/directo\ |               |                                               |              |       |
//|                                    |         | rate-pc-export                               |               |                                               |              |       |
//*------------------------------------+---------+----------------------------------------------+---------------+-----------------------------------------------+--------------+-------*

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
//KiwiC: front end input processing of class LocalLinkLoopBackTest  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=LocalLinkLoopBackTest..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=LocalLinkLoopBackTest..cctor
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
//KiwiC: front end input processing of class LocalLinkLoopBackTest.EthernetEcho  wonky=LocalLinkLoopBackTest igrf=false
//
//
//root_compiler: method compile: entry point. Method name=LocalLinkLoopBackTest.EthernetEcho..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=LocalLinkLoopBackTest.EthernetEcho..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class LocalLinkLoopBackTest.EthernetEcho  wonky=LocalLinkLoopBackTest igrf=false
//
//
//root_compiler: method compile: entry point. Method name=LocalLinkLoopBackTest.EthernetEcho.EntryPoint uid=EntryPoint10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=EntryPoint10 full_idl=LocalLinkLoopBackTest.EthernetEcho.EntryPoint
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
//   srcfile=EthernetEcho.exe
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
//PC codings points for kiwiLOCAOINT4001PC10 
//*-----------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                           | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiLOCAOINT4001PC10"         | 843 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiLOCAOINT4001PC10"         | 842 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 10   |
//| XU32'16:"16:kiwiLOCAOINT4001PC10"       | 836 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 5    |
//| XU32'8:"8:kiwiLOCAOINT4001PC10"         | 837 | 3       | hwm=0.0.1   | 3    |        | 4     | 4   | 2    |
//| XU32'8:"8:kiwiLOCAOINT4001PC10"         | 838 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 15   |
//| XU32'32:"32:kiwiLOCAOINT4001PC10"       | 835 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 3    |
//| XU32'512:"512:kiwiLOCAOINT4001PC10"     | 828 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 9    |
//| XU32'256:"256:kiwiLOCAOINT4001PC10"     | 829 | 7       | hwm=0.1.0   | 8    |        | 8     | 8   | 6    |
//| XU32'256:"256:kiwiLOCAOINT4001PC10"     | 830 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 11   |
//| XU32'1024:"1024:kiwiLOCAOINT4001PC10"   | 827 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 7    |
//| XU32'2:"2:kiwiLOCAOINT4001PC10"         | 841 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 19   |
//| XU32'2048:"2048:kiwiLOCAOINT4001PC10"   | 826 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 10   |
//| XU32'128:"128:kiwiLOCAOINT4001PC10"     | 831 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 7    |
//| XU32'128:"128:kiwiLOCAOINT4001PC10"     | 832 | 12      | hwm=0.1.0   | 13   |        | 13    | 13  | 14   |
//| XU32'4096:"4096:kiwiLOCAOINT4001PC10"   | 825 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 12   |
//| XU32'64:"64:kiwiLOCAOINT4001PC10"       | 833 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 12   |
//| XU32'64:"64:kiwiLOCAOINT4001PC10"       | 834 | 15      | hwm=0.1.0   | 16   |        | 16    | 16  | 17   |
//| XU32'8192:"8192:kiwiLOCAOINT4001PC10"   | 824 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 15   |
//| XU32'16384:"16384:kiwiLOCAOINT4001PC10" | 823 | 18      | hwm=0.0.0   | 18   |        | -     | -   | 20   |
//| XU32'4:"4:kiwiLOCAOINT4001PC10"         | 839 | 19      | hwm=0.0.0   | 19   |        | -     | -   | 3    |
//| XU32'4:"4:kiwiLOCAOINT4001PC10"         | 840 | 19      | hwm=0.0.0   | 19   |        | -     | -   | 18   |
//| XU32'32768:"32768:kiwiLOCAOINT4001PC10" | 822 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 19   |
//*-----------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'0:"0:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'0:"0:kiwiLOCAOINT4001PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                   |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                        |
//| F0   | E843 | R0 DATA | rx_sof_nargread(<NONE>) rx_src_rdy_nargread(<NONE>)                                                                                                    |
//| F0+E | E843 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1._SPILL.256write(E1) tx_eof_nwrite(U32'1) tx_src_rdy_nwrite(U32'1) tx_sof_nwrite(U32'1) LOCAOI\ |
//|      |      |         | NT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1._SPILL.257write(U32'0) LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0write(U32'0) LOCA\ |
//|      |      |         | OINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1write(S32'0) LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_3write(U32'0) LOCAOINT4\ |
//|      |      |         | 00.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(S32'0)                                                                                         |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'1:"1:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'1:"1:kiwiLOCAOINT4001PC10"
//*------+------+---------+----------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                 |
//*------+------+---------+----------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                      |
//| F1   | E842 | R0 DATA |                                                                      |
//| F1+E | E842 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0write(E2) |
//*------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'16:"16:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'16:"16:kiwiLOCAOINT4001PC10"
//*------+------+---------+---------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                            |
//*------+------+---------+---------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                 |
//| F2   | E836 | R0 DATA | rx_eof_nargread(<NONE>)                                                         |
//| F2+E | E836 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_3write(Cu(!rx_eof_n)) |
//*------+------+---------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'8:"8:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'8:"8:kiwiLOCAOINT4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                    |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                         |
//| F3   | E838 | R0 DATA |                                                                                                                         |
//| F3+E | E838 | W0 DATA | tx_src_rdy_nwrite(U32'0) tx_sof_nwrite(U32'1) LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(S32'6)   |
//| F3   | E837 | R0 DATA | rx_src_rdy_nargread(<NONE>) rx_dataargread(<NONE>)                                                                      |
//| F3+E | E837 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1write(E3) @8_US/CC/SCALbx10_ARA0_write(E4, Cu(C8u(rx_data))) |
//| F4   | E837 | W1 DATA |                                                                                                                         |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'32:"32:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'32:"32:kiwiLOCAOINT4001PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F5   | -    | R0 CTRL |      |
//| F5   | E835 | R0 DATA |      |
//| F5+E | E835 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'512:"512:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'512:"512:kiwiLOCAOINT4001PC10"
//*------+------+---------+----------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                 |
//*------+------+---------+----------------------------------------------------------------------*
//| F6   | -    | R0 CTRL |                                                                      |
//| F6   | E828 | R0 DATA |                                                                      |
//| F6+E | E828 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(E5) |
//*------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'256:"256:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'256:"256:kiwiLOCAOINT4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                  |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                                                                                       |
//| F7   | E830 | R0 DATA |                                                                                                                       |
//| F7+E | E830 | W0 DATA | tx_eof_nwrite(U32'1) tx_src_rdy_nwrite(U32'1) LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0write(U32'0) |
//| F7   | E829 | R0 DATA | @8_US/CC/SCALbx10_ARA0_read(E6)                                                                                       |
//| F8   | E829 | R1 DATA |                                                                                                                       |
//| F8+E | E829 | W0 DATA | tx_eof_nwrite(U32'0) tx_datawrite(E7)                                                                                 |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'1024:"1024:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'1024:"1024:kiwiLOCAOINT4001PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F9   | -    | R0 CTRL |      |
//| F9   | E827 | R0 DATA |      |
//| F9+E | E827 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'2:"2:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'2:"2:kiwiLOCAOINT4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F10   | -    | R0 CTRL |      |
//| F10   | E841 | R0 DATA |      |
//| F10+E | E841 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'2048:"2048:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'2048:"2048:kiwiLOCAOINT4001PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F11   | -    | R0 CTRL |      |
//| F11   | E826 | R0 DATA |      |
//| F11+E | E826 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'128:"128:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'128:"128:kiwiLOCAOINT4001PC10"
//*-------+------+---------+--------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                     |
//*-------+------+---------+--------------------------------------------------------------------------*
//| F12   | -    | R0 CTRL |                                                                          |
//| F12   | E832 | R0 DATA | @8_US/CC/SCALbx10_ARA0_read(E6)                                          |
//| F13   | E832 | R1 DATA |                                                                          |
//| F13+E | E832 | W0 DATA | tx_datawrite(E7)                                                         |
//| F12   | E831 | R0 DATA |                                                                          |
//| F12+E | E831 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(S32'12) |
//*-------+------+---------+--------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'4096:"4096:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'4096:"4096:kiwiLOCAOINT4001PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F14   | -    | R0 CTRL |                                                                      |
//| F14   | E825 | R0 DATA |                                                                      |
//| F14+E | E825 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(E5) |
//*-------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'64:"64:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'64:"64:kiwiLOCAOINT4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                    |
//*-------+------+---------+-------------------------------------------------------------------------*
//| F15   | -    | R0 CTRL |                                                                         |
//| F15   | E834 | R0 DATA | @8_US/CC/SCALbx10_ARA0_read(E6)                                         |
//| F16   | E834 | R1 DATA |                                                                         |
//| F16+E | E834 | W0 DATA | tx_sof_nwrite(E8) tx_datawrite(E7)                                      |
//| F15   | E833 | R0 DATA |                                                                         |
//| F15+E | E833 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(S32'0) |
//*-------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'8192:"8192:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'8192:"8192:kiwiLOCAOINT4001PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F17   | -    | R0 CTRL |                                                                      |
//| F17   | E824 | R0 DATA |                                                                      |
//| F17+E | E824 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2write(E5) |
//*-------+------+---------+----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'16384:"16384:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'16384:"16384:kiwiLOCAOINT4001PC10"
//*-------+------+---------+-----------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                        |
//*-------+------+---------+-----------------------------------------------------------------------------*
//| F18   | -    | R0 CTRL |                                                                             |
//| F18   | E823 | R0 DATA | rx_sof_nargread(<NONE>) rx_src_rdy_nargread(<NONE>)                         |
//| F18+E | E823 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1._SPILL.257write(E1) |
//*-------+------+---------+-----------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'4:"4:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'4:"4:kiwiLOCAOINT4001PC10"
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                            |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*
//| F19   | -    | R0 CTRL |                                                                                                                                                 |
//| F19   | E840 | R0 DATA |                                                                                                                                                 |
//| F19+E | E840 | W0 DATA |                                                                                                                                                 |
//| F19   | E839 | R0 DATA |                                                                                                                                                 |
//| F19+E | E839 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1write(S32'0) LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_3write(U32'0) |
//*-------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'32768:"32768:kiwiLOCAOINT4001PC10"
//res2: scon1: nopipeline: Thread=kiwiLOCAOINT4001PC10 state=XU32'32768:"32768:kiwiLOCAOINT4001PC10"
//*-------+------+---------+----------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                 |
//*-------+------+---------+----------------------------------------------------------------------*
//| F20   | -    | R0 CTRL |                                                                      |
//| F20   | E822 | R0 DATA |                                                                      |
//| F20+E | E822 | W0 DATA | LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0write(E9) |
//*-------+------+---------+----------------------------------------------------------------------*

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
//  E1 =.= COND(|rx_sof_n|, U32'0, Cu(!rx_src_rdy_n))
//
//
//  E2 =.= Cu(C1u(LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1._SPILL.256))
//
//
//  E3 =.= C(1+LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1)
//
//
//  E4 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1
//
//
//  E5 =.= C(1+LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2)
//
//
//  E6 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2
//
//
//  E7 =.= Cu(C8u(@8_US/CC/SCALbx10_ARA0[LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2]))
//
//
//  E8 =.= Cu(XU32'0:"0:USA10"==6==LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2)
//
//
//  E9 =.= Cu(C1u(LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1._SPILL.257))
//
//
//  E10 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_3
//
//
//  E11 =.= !LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_3
//
//
//  E12 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2>=LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1
//
//
//  E13 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2<LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_1
//
//
//  E14 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2<6
//
//
//  E15 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2>=6
//
//
//  E16 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2<12
//
//
//  E17 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_2>=12
//
//
//  E18 =.= !LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0
//
//
//  E19 =.= LOCAOINT400.LocalLinkLoopBackTest.EthernetEcho.echo.1.1.V_0
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for EthernetEcho to EthernetEcho

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for EthernetEcho:
//1 vectors of width 5
//
//8 vectors of width 1
//
//2 vectors of width 8
//
//1 vectors of width 10
//
//2 vectors of width 32
//
//Total state bits in module = 103 bits.
//
//8 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread LocalLinkLoopBackTest..cctor uid=cctor10 has 4 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread LocalLinkLoopBackTest.EthernetEcho..cctor uid=cctor12 has 4 CIL instructions in 1 basic blocks
//Thread LocalLinkLoopBackTest.EthernetEcho.EntryPoint uid=EntryPoint10 has 79 CIL instructions in 32 basic blocks
//Thread mpc10 has 17 bevelab control states (pauses)
//Reindexed thread kiwiLOCAOINT4001PC10 with 21 minor control states
// eof (HPR L/S Verilog)
