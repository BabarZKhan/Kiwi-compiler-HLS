

// CBG Orangepath HPR L/S System

// Verilog output file generated at 07/02/2019 10:44:37
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.l : 5th Feb 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe primesya.exe -vnl=offchip64.v -vnl-rootmodname=DUT -kiwife-directorate-endmode=finish -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwic-register-colours=1 -bondout-loadstore-port-count=1 -bondout-loadstore-port-lanes=8 -bondout-loadstore-lane-width=8 -bondout-loadstore-lane-addr-size=22 -res2-offchip-threshold=32 -kiwife-directorate-style=advanced
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=kiwicmiscio10 */
    input [31:0] volume,
    output reg signed [31:0] ksubsResultHi,
    output reg signed [31:0] ksubsResultLo,
    output reg signed [31:0] ksubsDesignSerialNumber,
    output reg signed [31:0] edesign,
    output reg signed [31:0] evariant,
    output reg signed [31:0] elimit,
    output reg [31:0] count,
    
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
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16);

function signed [7:0] rtl_signed_bitextract3;
   input [31:0] arg;
   rtl_signed_bitextract3 = $signed(arg[7:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract1;
   input [63:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[7:0]);
   endfunction


function signed [31:0] rtl_sign_extend2;
   input [7:0] arg;
   rtl_sign_extend2 = { {24{arg[7]}}, arg[7:0] };
   endfunction


function [31:0] rtl_unsigned_extend0;
   input argbit;
   rtl_unsigned_extend0 = { 31'b0, argbit };
   endfunction

// abstractionName=waypoints pi_name=kppIos10
  reg [639:0] KppWaypoint0;
  reg [639:0] KppWaypoint1;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TMAIN_IDL400_primesya_Main_V_4;
  reg signed [31:0] TMAIN_IDL400_primesya_Main_V_3;
  reg signed [31:0] TMAIN_IDL400_primesya_Main_V_2;
  reg signed [31:0] TMAIN_IDL400_primesya_Main_V_1;
// abstractionName=res2-morenets
  reg kiwiTMAINIDL4001PC10_stall;
  reg kiwiTMAINIDL4001PC10_clear;
  reg bondout11RDATAh10primed;
  reg bondout11RDATAh10vld;
  reg [63:0] bondout11RDATAh10hold;
  reg [4:0] kiwiTMAINIDL4001PC10nz;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin500339x10;
  wire signed [31:0] hprpin500681x10;
  wire [31:0] hprpin500683x10;
 always   @(* )  begin 
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
       bondout1_ACKRDY1 = 32'd0;
       bondout1_ADDR1 = 32'd0;
       bondout1_RWBAR1 = 32'd0;
       bondout1_WDATA1 = 64'd0;
       bondout1_LANES1 = 32'd0;
       bondout1_REQ1 = 32'sd0;
       bondout1_LANES1 = 32'sd0;
       bondout1_RWBAR1 = 32'sd0;
       bondout1_WDATA1 = 32'sd0;
       bondout1_ADDR1 = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;
      if (hpr_int_run_enable_DDX16)  begin 
              if (!kiwiTMAINIDL4001PC10_stall)  begin 
                       bondout1_REQ1 = ((32'hd/*13:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h1/*1:kiwiTMAINIDL4001PC10nz*/==
                      kiwiTMAINIDL4001PC10nz) || (32'h4/*4:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'ha/*10:kiwiTMAINIDL4001PC10nz*/==
                      kiwiTMAINIDL4001PC10nz)? 32'd1: 32'd0);

                      if ((32'h1/*1:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz))  begin 
                               bondout1_LANES1 = 1'h1;
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = hprpin500339x10;
                               bondout1_ADDR1 = 32'd0;
                               end 
                               end 
                      
              case (kiwiTMAINIDL4001PC10nz)
                  32'h4/*4:kiwiTMAINIDL4001PC10nz*/:  begin 
                      if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_LANES1 = (rtl_unsigned_extend0(1'h1)<<(TMAIN_IDL400_primesya_Main_V_1%32'sd8));
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = 64'sh101_0101_0101_0101;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_1>>>3);
                               end 
                              if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1)<32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_LANES1 = (rtl_unsigned_extend0(1'h1)<<(TMAIN_IDL400_primesya_Main_V_1%32'sd8));
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = 64'sh101_0101_0101_0101;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_1>>>3);
                               end 
                               end 
                      
                  32'ha/*10:kiwiTMAINIDL4001PC10nz*/:  begin 
                      if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4)<32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_RWBAR1 = 32'd1;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_4>>>3);
                               end 
                              if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_RWBAR1 = 32'd1;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_4>>>3);
                               end 
                               end 
                      
                  32'hd/*13:kiwiTMAINIDL4001PC10nz*/:  begin 
                      if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_2
                      )>=32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_LANES1 = (rtl_unsigned_extend0(1'h1)<<(TMAIN_IDL400_primesya_Main_V_3%32'sd8));
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = 64'd0;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_3>>>3);
                               end 
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1
                      +TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                               bondout1_LANES1 = (rtl_unsigned_extend0(1'h1)<<(TMAIN_IDL400_primesya_Main_V_3%32'sd8));
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = 64'd0;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_3>>>3);
                               end 
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall
                      )  begin 
                               bondout1_LANES1 = (rtl_unsigned_extend0(1'h1)<<(TMAIN_IDL400_primesya_Main_V_3%32'sd8));
                               bondout1_RWBAR1 = 32'd0;
                               bondout1_WDATA1 = 64'd0;
                               bondout1_ADDR1 = (TMAIN_IDL400_primesya_Main_V_3>>>3);
                               end 
                               end 
                      endcase
               end 
               hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogprimesya/1.0
      if (reset)  begin 
               kiwiTMAINIDL4001PC10nz_pc_export <= 32'd0;
               ksubsResultLo <= 32'd0;
               ksubsResultHi <= 32'd0;
               count <= 32'd0;
               evariant <= 32'd0;
               edesign <= 32'd0;
               ksubsDesignSerialNumber <= 32'd0;
               TMAIN_IDL400_primesya_Main_V_4 <= 32'd0;
               TMAIN_IDL400_primesya_Main_V_3 <= 32'd0;
               TMAIN_IDL400_primesya_Main_V_2 <= 32'd0;
               TMAIN_IDL400_primesya_Main_V_1 <= 32'd0;
               elimit <= 32'd0;
               bondout11RDATAh10primed <= 32'd0;
               bondout11RDATAh10vld <= 32'd0;
               bondout11RDATAh10hold <= 64'd0;
               hpr_abend_syndrome <= 32'd255;
               KppWaypoint0 <= 640'd0;
               KppWaypoint1 <= 640'd0;
               kiwiTMAINIDL4001PC10nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if ((32'hd/*13:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz))  begin 
                          if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          ) $display("Cross off %1d %1d   (count1=%1d)", TMAIN_IDL400_primesya_Main_V_2, $signed(TMAIN_IDL400_primesya_Main_V_3
                              +TMAIN_IDL400_primesya_Main_V_2), 32'sd2);
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1
                          +TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall) $display("Now counting");
                               end 
                          if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4)>=32'sd200) && (32'hb/*11:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz
                  ) && !kiwiTMAINIDL4001PC10_stall)  begin 
                          $display("Tally counting %1d %1d", TMAIN_IDL400_primesya_Main_V_4, hprpin500683x10);
                          $display("There are %1d primes below the natural number %1d.", hprpin500683x10, 32'sd200);
                          $display("Optimisation variant=%1d (count1 is %1d).", 32'sd0, 32'sd2);
                           end 
                          if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4)<32'sd200) && (32'hc/*12:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz
                  ) && !kiwiTMAINIDL4001PC10_stall) $display("Tally counting %1d %1d", TMAIN_IDL400_primesya_Main_V_4, hprpin500683x10
                      );
                      if ((32'h9/*9:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz))  begin 
                          if (($signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          ) $display("Cross off %1d %1d   (count1=%1d)", TMAIN_IDL400_primesya_Main_V_2, $signed(TMAIN_IDL400_primesya_Main_V_2
                              +TMAIN_IDL400_primesya_Main_V_2), 32'sd2);
                              if (($signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          )  begin 
                                  $display("Skip out on square");
                                  $display("Now counting");
                                   end 
                                   end 
                          if (!kiwiTMAINIDL4001PC10_stall) 
                      case (kiwiTMAINIDL4001PC10nz)
                          32'h0/*0:kiwiTMAINIDL4001PC10nz*/:  begin 
                              $display("%s%1d", "Primes Up To ", 32'sd200);
                               count <= 32'h0;
                               evariant <= 32'sh0;
                               edesign <= 32'shfc0;
                               ksubsDesignSerialNumber <= 32'sh1010;
                               TMAIN_IDL400_primesya_Main_V_4 <= 32'sh0;
                               TMAIN_IDL400_primesya_Main_V_3 <= 32'sh0;
                               TMAIN_IDL400_primesya_Main_V_2 <= 32'sh0;
                               TMAIN_IDL400_primesya_Main_V_1 <= 32'sh0;
                               elimit <= 32'shc8;
                               KppWaypoint0 <= 32'sd1;
                               KppWaypoint1 <= "START";
                               end 
                              
                          32'h3/*3:kiwiTMAINIDL4001PC10nz*/:  begin 
                               count <= 32'h0;
                               TMAIN_IDL400_primesya_Main_V_1 <= 32'sh0;
                               end 
                              
                          32'h4/*4:kiwiTMAINIDL4001PC10nz*/: $display("Setting initial array flag to hold : addr=%1d readback=%1d", TMAIN_IDL400_primesya_Main_V_1
                          , 32'h1);

                          32'h7/*7:kiwiTMAINIDL4001PC10nz*/:  begin 
                               KppWaypoint0 <= 32'sd5;
                               KppWaypoint1 <= "FINISH";
                               end 
                              
                          32'h8/*8:kiwiTMAINIDL4001PC10nz*/:  begin 
                              $finish(32'sd0);
                              $finish(32'sd0);
                               KppWaypoint0 <= 32'sd0;
                               KppWaypoint1 <= "FINISHED";
                               hpr_abend_syndrome <= 32'sd0;
                               hpr_abend_syndrome <= 32'sd0;
                               end 
                              
                          32'hb/*11:kiwiTMAINIDL4001PC10nz*/:  begin 
                               ksubsResultLo <= hprpin500681x10;
                               ksubsResultHi <= 32'shc8;
                               TMAIN_IDL400_primesya_Main_V_4 <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4);
                               end 
                              
                          32'hc/*12:kiwiTMAINIDL4001PC10nz*/:  TMAIN_IDL400_primesya_Main_V_4 <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4
                          );

                      endcase
                      
                  case (kiwiTMAINIDL4001PC10nz)
                      32'h0/*0:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h1/*1:kiwiTMAINIDL4001PC10nz*/;

                      32'h4/*4:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                                   TMAIN_IDL400_primesya_Main_V_2 <= 32'sh2;
                                   TMAIN_IDL400_primesya_Main_V_1 <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1);
                                   KppWaypoint0 <= 32'sd2;
                                   KppWaypoint1 <= "wp2";
                                   end 
                                  if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1)<32'sd200))  kiwiTMAINIDL4001PC10nz <= 32'h5/*5:kiwiTMAINIDL4001PC10nz*/;
                               else  kiwiTMAINIDL4001PC10nz <= 32'h6/*6:kiwiTMAINIDL4001PC10nz*/;
                          if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1)<32'sd200) && !kiwiTMAINIDL4001PC10_stall)  TMAIN_IDL400_primesya_Main_V_1
                               <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_1);

                               end 
                          
                      32'h8/*8:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h11/*17:kiwiTMAINIDL4001PC10nz*/;

                      32'h9/*9:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if (($signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          )  begin 
                                   TMAIN_IDL400_primesya_Main_V_4 <= 32'sh0;
                                   TMAIN_IDL400_primesya_Main_V_3 <= $signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2
                                  );

                                   KppWaypoint0 <= 32'sd3;
                                   KppWaypoint1 <= "wp3";
                                   end 
                                  if (($signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          )  TMAIN_IDL400_primesya_Main_V_3 <= $signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2);
                              if (($signed(TMAIN_IDL400_primesya_Main_V_2+TMAIN_IDL400_primesya_Main_V_2)<32'sd200))  kiwiTMAINIDL4001PC10nz
                               <= 32'hd/*13:kiwiTMAINIDL4001PC10nz*/;

                               else  kiwiTMAINIDL4001PC10nz <= 32'ha/*10:kiwiTMAINIDL4001PC10nz*/;
                           end 
                          
                      32'ha/*10:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if (($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_4)<32'sd200))  kiwiTMAINIDL4001PC10nz <= 32'hc/*12:kiwiTMAINIDL4001PC10nz*/;
                               else  kiwiTMAINIDL4001PC10nz <= 32'hb/*11:kiwiTMAINIDL4001PC10nz*/;
                           bondout11RDATAh10primed <= !kiwiTMAINIDL4001PC10_stall;
                           bondout11RDATAh10primed <= !kiwiTMAINIDL4001PC10_stall;
                           end 
                          
                      32'hb/*11:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if ((bondout11RDATAh10vld? !(!bondout11RDATAh10hold) && !kiwiTMAINIDL4001PC10_stall: !(!rtl_unsigned_bitextract1((bondout1_RDATA1
                          >>32'sd8*(TMAIN_IDL400_primesya_Main_V_4%32'sd8)))) && !kiwiTMAINIDL4001PC10_stall))  count <= $unsigned(32'd1
                              +count);

                              if (bondout11RDATAh10vld || bondout1_ACK1)  kiwiTMAINIDL4001PC10nz <= 32'h7/*7:kiwiTMAINIDL4001PC10nz*/;
                               end 
                          
                      32'hc/*12:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if ((bondout11RDATAh10vld? !(!bondout11RDATAh10hold) && !kiwiTMAINIDL4001PC10_stall: !(!rtl_unsigned_bitextract1((bondout1_RDATA1
                          >>32'sd8*(TMAIN_IDL400_primesya_Main_V_4%32'sd8)))) && !kiwiTMAINIDL4001PC10_stall))  count <= $unsigned(32'd1
                              +count);

                              if (bondout11RDATAh10vld || bondout1_ACK1)  kiwiTMAINIDL4001PC10nz <= 32'ha/*10:kiwiTMAINIDL4001PC10nz*/;
                               end 
                          
                      32'hd/*13:kiwiTMAINIDL4001PC10nz*/:  begin 
                          if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1+TMAIN_IDL400_primesya_Main_V_2
                          )>=32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                                   TMAIN_IDL400_primesya_Main_V_4 <= 32'sh0;
                                   TMAIN_IDL400_primesya_Main_V_3 <= $signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2
                                  );

                                   TMAIN_IDL400_primesya_Main_V_2 <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_2);
                                   KppWaypoint0 <= 32'sd3;
                                   KppWaypoint1 <= "wp3";
                                   end 
                                  if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1
                          +TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall)  begin 
                                   TMAIN_IDL400_primesya_Main_V_3 <= $signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2
                                  );

                                   TMAIN_IDL400_primesya_Main_V_2 <= $signed(32'sd1+TMAIN_IDL400_primesya_Main_V_2);
                                   end 
                                  if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)<32'sd200) && !kiwiTMAINIDL4001PC10_stall
                          )  TMAIN_IDL400_primesya_Main_V_3 <= $signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2);
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1
                          +TMAIN_IDL400_primesya_Main_V_2)<32'sd200))  kiwiTMAINIDL4001PC10nz <= 32'hf/*15:kiwiTMAINIDL4001PC10nz*/;
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)>=32'sd200) && ($signed(32'sd1
                          +TMAIN_IDL400_primesya_Main_V_2)>=32'sd200))  kiwiTMAINIDL4001PC10nz <= 32'h10/*16:kiwiTMAINIDL4001PC10nz*/;
                              if (($signed(TMAIN_IDL400_primesya_Main_V_3+TMAIN_IDL400_primesya_Main_V_2)<32'sd200))  kiwiTMAINIDL4001PC10nz
                               <= 32'he/*14:kiwiTMAINIDL4001PC10nz*/;

                               end 
                          endcase
                  if (bondout1_ACK1 && bondout11RDATAh10primed)  begin 
                           bondout11RDATAh10primed <= 32'd0;
                           bondout11RDATAh10vld <= 32'd1;
                           bondout11RDATAh10hold <= rtl_unsigned_bitextract1((bondout1_RDATA1>>32'sd8*(TMAIN_IDL400_primesya_Main_V_4
                          %32'sd8)));

                           end 
                          
                  case (kiwiTMAINIDL4001PC10nz)
                      32'h3/*3:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h4/*4:kiwiTMAINIDL4001PC10nz*/;

                      32'h7/*7:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h8/*8:kiwiTMAINIDL4001PC10nz*/;
                  endcase
                  if (!kiwiTMAINIDL4001PC10_stall && kiwiTMAINIDL4001PC10_clear)  bondout11RDATAh10vld <= 32'd0;
                      
                  case (kiwiTMAINIDL4001PC10nz)
                      32'h1/*1:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h2/*2:kiwiTMAINIDL4001PC10nz*/;

                      32'h2/*2:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h3/*3:kiwiTMAINIDL4001PC10nz*/;

                      32'h5/*5:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h4/*4:kiwiTMAINIDL4001PC10nz*/;

                      32'h6/*6:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h9/*9:kiwiTMAINIDL4001PC10nz*/;

                      32'he/*14:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'hd/*13:kiwiTMAINIDL4001PC10nz*/;

                      32'hf/*15:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'h9/*9:kiwiTMAINIDL4001PC10nz*/;

                      32'h10/*16:kiwiTMAINIDL4001PC10nz*/:  kiwiTMAINIDL4001PC10nz <= 32'ha/*10:kiwiTMAINIDL4001PC10nz*/;
                  endcase
                   kiwiTMAINIDL4001PC10nz_pc_export <= kiwiTMAINIDL4001PC10nz;
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogprimesya/1.0


       end 
      

always @(*) kiwiTMAINIDL4001PC10_clear = (32'h0/*0:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h1/*1:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h3/*3:kiwiTMAINIDL4001PC10nz*/==
kiwiTMAINIDL4001PC10nz) || (32'h4/*4:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h7/*7:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz
) || (32'h8/*8:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'h9/*9:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'hd
/*13:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'ha/*10:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz);

always @(*) kiwiTMAINIDL4001PC10_stall = ((32'hc/*12:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz) || (32'hb/*11:kiwiTMAINIDL4001PC10nz*/==kiwiTMAINIDL4001PC10nz)) && !bondout1_ACK1
 && !bondout11RDATAh10vld;

assign hprpin500339x10 = (rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0<$signed(volume)))))<<32'sd8)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0
<$signed(volume)))))<<32'sd16)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0<$signed(volume)))))<<32'sd24)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0
<$signed(volume)))))<<32'sd32)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0<$signed(volume)))))<<32'sd40)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0
<$signed(volume)))))<<32'sd48)|(rtl_sign_extend2(rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0<$signed(volume)))))<<32'sd56)|rtl_signed_bitextract3(rtl_unsigned_extend0((32'sd0
<$signed(volume))));

assign hprpin500681x10 = ((bondout11RDATAh10vld? !(!bondout11RDATAh10hold): !(!rtl_unsigned_bitextract1((bondout1_RDATA1>>32'sd8*(TMAIN_IDL400_primesya_Main_V_4
%32'sd8)))))? $unsigned(32'sd1+count): count);

assign hprpin500683x10 = ((bondout11RDATAh10vld? !(!bondout11RDATAh10hold): !(!rtl_unsigned_bitextract1((bondout1_RDATA1>>32'sd8*(TMAIN_IDL400_primesya_Main_V_4
%32'sd8)))))? $unsigned(32'd1+count): count);

// Structural Resource (FU) inventory for DUT:// 5 vectors of width 1
// 1 vectors of width 5
// 1 vectors of width 64
// 4 vectors of width 32
// 2 vectors of width 640
// Total state bits in module = 1482 bits.
// 96 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.l : 5th Feb 2019
//07/02/2019 10:44:34
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe primesya.exe -vnl=offchip64.v -vnl-rootmodname=DUT -kiwife-directorate-endmode=finish -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwic-register-colours=1 -bondout-loadstore-port-count=1 -bondout-loadstore-port-lanes=8 -bondout-loadstore-lane-width=8 -bondout-loadstore-lane-addr-size=22 -res2-offchip-threshold=32 -kiwife-directorate-style=advanced


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------+---------+--------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Class    | Style   | Dir Style                                                                                              | Timing Target | Method        | UID    | Skip  |
//*----------+---------+--------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| primesya | MM_root | DS_advanced self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | primesya.Main | Main10 | false |
//*----------+---------+--------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*

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

//Report from kiwife virtual to physical register colouring/mapping for thread TMAIN_CTOR403:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread TMAIN_CTOR411:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread TMAIN_IDL400:::
//: Linear scan colouring done for 1 vregs using 1 pregs
//
//Allocate phy reg purpose=refxarray msg=allocation for thread TMAIN_IDL400 for V5011 dt=$star1$/@/BOOL usecount=1
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread TMAIN_IDL400:::
//: Linear scan colouring done for 9 vregs using 6 pregs
//
//Allocate phy reg purpose=localvar msg=allocation for thread TMAIN_IDL400 for V5018 dt=SINT usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread TMAIN_IDL400 for V5017 dt=SINT usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread TMAIN_IDL400 for V5016 dt=SINT usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread TMAIN_IDL400 for V5015 dt=SINT usecount=1
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread TMAIN_IDL400 for V5005 dt=UINT usecount=2
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread TMAIN_IDL400 for V5006 dt=SINT usecount=1
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread TMAIN_IDL400 for V5003 dt=SINT usecount=1
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread TMAIN_IDL400 for V5004 dt=$star1$/@/BOOL usecount=1
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread TMAIN_IDL400 for V5019 dt=UINT usecount=1
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
//KiwiC: front end input processing of class primesya  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=primesya..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=primesya..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class primesya  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=primesya.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=primesya.Main
//
//
//Register sharing: general primesya.count/GP used for primesya.count
//
//
//Register sharing: general primesya.count/GP used for primesya.vol
//
//
//Register sharing: general TMAIN_IDL400.primesya.Main.V_4/GP used for TMAIN_IDL400.primesya.Main.V_4
//
//
//Register sharing: general TMAIN_IDL400.primesya.Main.V_4/GP used for TMAIN_IDL400.primesya.Main.V_3
//
//
//Register sharing: general TMAIN_IDL400.primesya.Main.V_2/GP used for TMAIN_IDL400.primesya.Main.V_2
//
//
//Register sharing: general TMAIN_IDL400.primesya.Main.V_2/GP used for TMAIN_IDL400.primesya.Main.V_1
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
//   bondout-loadstore-port-lanes=8
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
//   kiwic-cil-dump=combined
//
//
//   kiwic-kcode-dump=enable
//
//
//   kiwife-dynpoly=enable
//
//
//   kiwic-library-redirects=enable
//
//
//   kiwic-register-colours=1
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
//   kiwife-directorate-style=advanced
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
//   srcfile=primesya.exe
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
//*------------------------+-------+---------------------------------------------------------------------------------*
//| Key                    | Value | Description                                                                     |
//*------------------------+-------+---------------------------------------------------------------------------------*
//| int-flr-mul            | 1000  |                                                                                 |
//| max-no-fp-addsubs      | 6     | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls         | 6     | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls        | 3     | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs         | 2     | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs        | 2     | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors     | 8     | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing   | 8     | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div           | 5     |                                                                                 |
//| fp-fl-dp-add           | 4     |                                                                                 |
//| fp-fl-dp-mul           | 3     |                                                                                 |
//| fp-fl-sp-div           | 15    |                                                                                 |
//| fp-fl-sp-add           | 4     |                                                                                 |
//| fp-fl-sp-mul           | 5     |                                                                                 |
//| res2-offchip-threshold | 32    |                                                                                 |
//| res2-combrom-threshold | 64    |                                                                                 |
//| res2-combram-threshold | 32    |                                                                                 |
//| res2-regfile-threshold | 8     |                                                                                 |
//*------------------------+-------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTMAINIDL4001PC10 
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                       | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTMAINIDL4001PC10"     | 827 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTMAINIDL4001PC10"     | 826 | 1       | hwm=0.0.1   | 1    |        | 2     | 2   | 3    |
//| XU32'2:"2:kiwiTMAINIDL4001PC10"     | 825 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiTMAINIDL4001PC10"     | 823 | 4       | hwm=0.0.1   | 4    |        | 6     | 6   | 9    |
//| XU32'4:"4:kiwiTMAINIDL4001PC10"     | 824 | 4       | hwm=0.0.1   | 4    |        | 5     | 5   | 4    |
//| XU32'32:"32:kiwiTMAINIDL4001PC10"   | 818 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 8    |
//| XU32'64:"64:kiwiTMAINIDL4001PC10"   | 817 | 8       | hwm=0.0.0   | 8    |        | -     | -   | -    |
//| XU32'8:"8:kiwiTMAINIDL4001PC10"     | 821 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'8:"8:kiwiTMAINIDL4001PC10"     | 822 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 13   |
//| XU32'16:"16:kiwiTMAINIDL4001PC10"   | 819 | 10      | hwm=0.1.0   | 12   |        | 12    | 12  | 10   |
//| XU32'16:"16:kiwiTMAINIDL4001PC10"   | 820 | 10      | hwm=0.1.0   | 11   |        | 11    | 11  | 7    |
//| XU32'128:"128:kiwiTMAINIDL4001PC10" | 814 | 13      | hwm=0.0.1   | 13   |        | 16    | 16  | 10   |
//| XU32'128:"128:kiwiTMAINIDL4001PC10" | 815 | 13      | hwm=0.0.1   | 13   |        | 15    | 15  | 9    |
//| XU32'128:"128:kiwiTMAINIDL4001PC10" | 816 | 13      | hwm=0.0.1   | 13   |        | 14    | 14  | 13   |
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'0:"0:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'0:"0:kiwiTMAINIDL4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                            |
//| F0   | E827 | R0 DATA |                                                                                                                            |
//| F0+E | E827 | W0 DATA | elimit te=te:F0 write(S32'200) TMAIN_IDL400.primesya.Main.V_1 te=te:F0 write(S32'0) TMAIN_IDL400.primesya.Main.V_2 te=te:\ |
//|      |      |         | F0 write(S32'0) TMAIN_IDL400.primesya.Main.V_3 te=te:F0 write(S32'0) TMAIN_IDL400.primesya.Main.V_4 te=te:F0 write(S32'0)\ |
//|      |      |         |  ksubsDesignSerialNumber te=te:F0 write(S32'4112) edesign te=te:F0 write(S32'4032) evariant te=te:F0 write(S32'0) count t\ |
//|      |      |         | e=te:F0 write(U32'0)  PLI:Primes Up To   W/P:START                                                                         |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'1:"1:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'1:"1:kiwiTMAINIDL4001PC10"
//*--------+------+---------+----------------------------------------*
//| pc     | eno  | Phaser  | Work                                   |
//*--------+------+---------+----------------------------------------*
//| F1     | -    | R0 CTRL |                                        |
//| F1     | E826 | R0 DATA |                                        |
//| F1+E+S | E826 | W0 DATA | bondout1_1 te=te:F1 write(0, E1, U1'1) |
//| F2     | E826 | W1 DATA |                                        |
//*--------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'2:"2:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'2:"2:kiwiTMAINIDL4001PC10"
//*------+------+---------+----------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                             |
//*------+------+---------+----------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                  |
//| F3   | E825 | R0 DATA |                                                                                  |
//| F3+E | E825 | W0 DATA | TMAIN_IDL400.primesya.Main.V_1 te=te:F3 write(S32'0) count te=te:F3 write(U32'0) |
//*------+------+---------+----------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'4:"4:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'4:"4:kiwiTMAINIDL4001PC10"
//*--------+------+---------+------------------------------------------------------------------------------------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                                                                                               |
//*--------+------+---------+------------------------------------------------------------------------------------------------------------------------------------*
//| F4     | -    | R0 CTRL |                                                                                                                                    |
//| F4     | E824 | R0 DATA |                                                                                                                                    |
//| F4+E+S | E824 | W0 DATA | TMAIN_IDL400.primesya.Main.V_1 te=te:F4 write(E2) bondout1_1 te=te:F4 write(E3, S'72340172838076673, E4)  PLI:Setting initial arr\ |
//|        |      |         | a...                                                                                                                               |
//| F5     | E824 | W1 DATA |                                                                                                                                    |
//| F4+S   | E823 | R0 DATA |                                                                                                                                    |
//| F4+E+S | E823 | W0 DATA | TMAIN_IDL400.primesya.Main.V_1 te=te:F4 write(E2) TMAIN_IDL400.primesya.Main.V_2 te=te:F4 write(S32'2) bondout1_1 te=te:F4 write(\ |
//|        |      |         | E3, S'72340172838076673, E4)  W/P:wp2  PLI:Setting initial arra...                                                                 |
//| F6     | E823 | W1 DATA |                                                                                                                                    |
//*--------+------+---------+------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'32:"32:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'32:"32:kiwiTMAINIDL4001PC10"
//*------+------+---------+-------------*
//| pc   | eno  | Phaser  | Work        |
//*------+------+---------+-------------*
//| F7   | -    | R0 CTRL |             |
//| F7   | E818 | R0 DATA |             |
//| F7+E | E818 | W0 DATA |  W/P:FINISH |
//*------+------+---------+-------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'64:"64:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'64:"64:kiwiTMAINIDL4001PC10"
//*------+------+---------+-------------------------------------*
//| pc   | eno  | Phaser  | Work                                |
//*------+------+---------+-------------------------------------*
//| F8   | -    | R0 CTRL |                                     |
//| F8   | E817 | R0 DATA |                                     |
//| F8+E | E817 | W0 DATA |  PLI:GSAI:hpr_sysexit  W/P:FINISHED |
//*------+------+---------+-------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'8:"8:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'8:"8:kiwiTMAINIDL4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                      |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                                                                                                                           |
//| F9   | E822 | R0 DATA |                                                                                                                                                           |
//| F9+E | E822 | W0 DATA | TMAIN_IDL400.primesya.Main.V_3 te=te:F9 write(E5)  PLI:Cross off %d %d   (c...                                                                            |
//| F9   | E821 | R0 DATA |                                                                                                                                                           |
//| F9+E | E821 | W0 DATA | TMAIN_IDL400.primesya.Main.V_3 te=te:F9 write(E5) TMAIN_IDL400.primesya.Main.V_4 te=te:F9 write(S32'0)  PLI:Now counting  W/P:wp3  PLI:Skip out on square |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'16:"16:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'16:"16:kiwiTMAINIDL4001PC10"
//*---------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc      | eno  | Phaser  | Work                                                                                                                                                              |
//*---------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F10     | -    | R0 CTRL |                                                                                                                                                                   |
//| F10+S   | E820 | R0 DATA | bondout1_1 te=te:F10 read(E6)                                                                                                                                     |
//| F11+S   | E820 | R1 DATA |                                                                                                                                                                   |
//| F11+E+S | E820 | W0 DATA | TMAIN_IDL400.primesya.Main.V_4 te=te:F11 write(E7) count te=te:F11 write(Cu(1+count)) ksubsResultHi te=te:F11 write(S32'200) ksubsResultLo te=te:F11 write(E8)  \ |
//|         |      |         | PLI:Optimisation variant...  PLI:There are %u primes ...  PLI:Tally counting %d %u                                                                                |
//| F10+S   | E819 | R0 DATA | bondout1_1 te=te:F10 read(E6)                                                                                                                                     |
//| F12+S   | E819 | R1 DATA |                                                                                                                                                                   |
//| F12+E+S | E819 | W0 DATA | TMAIN_IDL400.primesya.Main.V_4 te=te:F12 write(E7) count te=te:F12 write(Cu(1+count))  PLI:Tally counting %d %u                                                   |
//*---------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'128:"128:kiwiTMAINIDL4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTMAINIDL4001PC10 state=XU32'128:"128:kiwiTMAINIDL4001PC10"
//*---------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc      | eno  | Phaser  | Work                                                                                                                                                 |
//*---------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F13     | -    | R0 CTRL |                                                                                                                                                      |
//| F13     | E816 | R0 DATA |                                                                                                                                                      |
//| F13+E+S | E816 | W0 DATA | TMAIN_IDL400.primesya.Main.V_3 te=te:F13 write(E9) bondout1_1 te=te:F13 write(E10, 0, E11)  PLI:Cross off %d %d   (c...                              |
//| F14     | E816 | W1 DATA |                                                                                                                                                      |
//| F13+S   | E815 | R0 DATA |                                                                                                                                                      |
//| F13+E+S | E815 | W0 DATA | TMAIN_IDL400.primesya.Main.V_2 te=te:F13 write(E12) TMAIN_IDL400.primesya.Main.V_3 te=te:F13 write(E9) bondout1_1 te=te:F13 write(E10, 0, E11)       |
//| F15     | E815 | W1 DATA |                                                                                                                                                      |
//| F13+S   | E814 | R0 DATA |                                                                                                                                                      |
//| F13+E+S | E814 | W0 DATA | TMAIN_IDL400.primesya.Main.V_2 te=te:F13 write(E12) TMAIN_IDL400.primesya.Main.V_3 te=te:F13 write(E9) TMAIN_IDL400.primesya.Main.V_4 te=te:F13 wri\ |
//|         |      |         | te(S32'0) bondout1_1 te=te:F13 write(E10, 0, E11)  PLI:Now counting  W/P:wp3                                                                         |
//| F16     | E814 | W1 DATA |                                                                                                                                                      |
//*---------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is Some 25 (25) bytes=4194304

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
//Bondout/Offchip Memory Map - Lane addressed.
//*-------------------------+---------+---------+--------------+----------+-------+------------*
//| Resource                | N-Width | R-Width | Base Address | No Items | End+1 | Portname   |
//*-------------------------+---------+---------+--------------+----------+-------+------------*
//| @_BOOL/CC/SCALbx10_ARA0 | 1       | 8       | 0            | 200      | 25    | bondout0_0 |
//*-------------------------+---------+---------+--------------+----------+-------+------------*

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
//  E1 =.= (C8(Cu(0<volume)))<<8|(C8(Cu(0<volume)))<<16|(C8(Cu(0<volume)))<<24|(C8(Cu(0<volume)))<<32|(C8(Cu(0<volume)))<<40|(C8(Cu(0<volume)))<<48|(C8(Cu(0<volume)))<<56|C8(Cu(0<volume))
//
//
//  E2 =.= C(1+TMAIN_IDL400.primesya.Main.V_1)
//
//
//  E3 =.= TMAIN_IDL400.primesya.Main.V_1/8
//
//
//  E4 =.= U1'1<<TMAIN_IDL400.primesya.Main.V_1%8
//
//
//  E5 =.= C(TMAIN_IDL400.primesya.Main.V_2+TMAIN_IDL400.primesya.Main.V_2)
//
//
//  E6 =.= TMAIN_IDL400.primesya.Main.V_4/8
//
//
//  E7 =.= C(1+TMAIN_IDL400.primesya.Main.V_4)
//
//
//  E8 =.= COND(|-|@_BOOL/CC/SCALbx10_ARA0[TMAIN_IDL400.primesya.Main.V_4], C(1+count), C(count))
//
//
//  E9 =.= C(TMAIN_IDL400.primesya.Main.V_3+TMAIN_IDL400.primesya.Main.V_2)
//
//
//  E10 =.= TMAIN_IDL400.primesya.Main.V_3/8
//
//
//  E11 =.= U1'1<<TMAIN_IDL400.primesya.Main.V_3%8
//
//
//  E12 =.= C(1+TMAIN_IDL400.primesya.Main.V_2)
//
//
//  E13 =.= (C(1+TMAIN_IDL400.primesya.Main.V_1))<200
//
//
//  E14 =.= (C(1+TMAIN_IDL400.primesya.Main.V_1))>=200
//
//
//  E15 =.= (C(TMAIN_IDL400.primesya.Main.V_2+TMAIN_IDL400.primesya.Main.V_2))<200
//
//
//  E16 =.= (C(TMAIN_IDL400.primesya.Main.V_2+TMAIN_IDL400.primesya.Main.V_2))>=200
//
//
//  E17 =.= (C(1+TMAIN_IDL400.primesya.Main.V_4))>=200
//
//
//  E18 =.= {[|bondout11RDATAh10vld|]; [|bondout1_ACK1|]}
//
//
//  E19 =.= (C(1+TMAIN_IDL400.primesya.Main.V_4))<200
//
//
//  E20 =.= (C(TMAIN_IDL400.primesya.Main.V_3+TMAIN_IDL400.primesya.Main.V_2))<200
//
//
//  E21 =.= {[(C(TMAIN_IDL400.primesya.Main.V_3+TMAIN_IDL400.primesya.Main.V_2))>=200, (C(1+TMAIN_IDL400.primesya.Main.V_2))<200]}
//
//
//  E22 =.= {[(C(TMAIN_IDL400.primesya.Main.V_3+TMAIN_IDL400.primesya.Main.V_2))>=200, (C(1+TMAIN_IDL400.primesya.Main.V_2))>=200]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for offchip64 to offchip64

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//5 vectors of width 1
//
//1 vectors of width 5
//
//1 vectors of width 64
//
//4 vectors of width 32
//
//2 vectors of width 640
//
//Total state bits in module = 1482 bits.
//
//96 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread primesya..cctor uid=cctor10 has 11 CIL instructions in 1 basic blocks
//Thread primesya.Main uid=Main10 has 67 CIL instructions in 22 basic blocks
//Thread mpc10 has 9 bevelab control states (pauses)
//Reindexed thread kiwiTMAINIDL4001PC10 with 17 minor control states
// eof (HPR L/S Verilog)
