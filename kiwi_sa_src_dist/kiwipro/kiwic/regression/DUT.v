

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:23
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test41r1.exe -sim=1800 -vnl-resets=synchronous -vnl DUT.v -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTESTMAIN400PC10nz_pc_export,
    
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
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test41r1_Main_V_4;
  reg signed [31:0] TESTMAIN400_test41r1_Main_V_3;
  reg signed [31:0] TESTMAIN400_test41r1_Main_V_2;
// abstractionName=res2-contacts pi_name=CV_INT_VL_DIVIDER_S
  reg isDIVIDERALUS32_10_REQ;
  wire isDIVIDERALUS32_10_ACK;
  wire signed [31:0] isDIVIDERALUS32_10_RR;
  reg signed [31:0] isDIVIDERALUS32_10_NN;
  reg signed [31:0] isDIVIDERALUS32_10_DD;
  wire isDIVIDERALUS32_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx10_ARA0_rdata;
  reg [13:0] A_SINT_CC_SCALbx10_ARA0_addr;
  reg A_SINT_CC_SCALbx10_ARA0_wen;
  reg A_SINT_CC_SCALbx10_ARA0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg signed [31:0] pipe12;
  reg signed [31:0] pipe10;
  reg kiwiTESTMAIN400PC10_stall;
  reg kiwiTESTMAIN400PC10_clear;
  reg isDIVIDERALUS3210RRh10primed;
  reg isDIVIDERALUS3210RRh10vld;
  reg signed [31:0] isDIVIDERALUS3210RRh10hold;
  reg signed [31:0] SINTCCSCALbx10ARA0rdatah12hold;
  reg SINTCCSCALbx10ARA0rdatah12shot0;
  reg signed [31:0] SINTCCSCALbx10ARA0rdatah10hold;
  reg SINTCCSCALbx10ARA0rdatah10shot0;
  reg [4:0] kiwiTESTMAIN400PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               isDIVIDERALUS3210RRh10primed <= 32'd0;
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test41r1_Main_V_3 <= 32'd0;
               TESTMAIN400_test41r1_Main_V_4 <= 32'd0;
               TESTMAIN400_test41r1_Main_V_2 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $display("Start Kiwi test41r1");
                                   TESTMAIN400_test41r1_Main_V_3 <= 32'sh0;
                                   TESTMAIN400_test41r1_Main_V_4 <= 32'sh0;
                                   TESTMAIN400_test41r1_Main_V_2 <= 32'sh0;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $display("The value of the integer: %1d", 32'sd1024);
                                   TESTMAIN400_test41r1_Main_V_2 <= 32'sh64;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14) if ((TESTMAIN400_test41r1_Main_V_2<32'sd180))  begin 
                              if (!kiwiTESTMAIN400PC10_stall)  TESTMAIN400_test41r1_Main_V_2 <= $signed(32'sd1+TESTMAIN400_test41r1_Main_V_2
                                  );

                                   kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                               end 
                               else  begin 
                              if (!kiwiTESTMAIN400PC10_stall)  begin 
                                       TESTMAIN400_test41r1_Main_V_3 <= 32'sh0;
                                       TESTMAIN400_test41r1_Main_V_4 <= 32'sh6e;
                                       end 
                                       kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                               end 
                              
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           isDIVIDERALUS3210RRh10primed <= !kiwiTESTMAIN400PC10_stall;
                           end 
                          
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'he/*14:kiwiTESTMAIN400PC10nz*/;
                      
                  32'he/*14:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hf/*15:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h10/*16:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h10/*16:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h11/*17:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h11/*17:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiTESTMAIN400PC10nz <= 32'h12/*18:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((TESTMAIN400_test41r1_Main_V_4>=32'sd120))  begin if (!kiwiTESTMAIN400PC10_stall)  begin 
                                      $display("Press any key to exit.");
                                      $finish(32'sd0);
                                       end 
                                       end 
                               else  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                          if ((TESTMAIN400_test41r1_Main_V_4>=32'sd120))  begin 
                                  if (!kiwiTESTMAIN400PC10_stall)  hpr_abend_syndrome <= 32'sd0;
                                       kiwiTESTMAIN400PC10nz <= 32'h13/*19:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                   end 
                          
                  32'h12/*18:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((TESTMAIN400_test41r1_Main_V_4<32'sd120) && !isDIVIDERALUS3210RRh10vld && !isDIVIDERALUS32_10_ACK && !kiwiTESTMAIN400PC10_stall
                          )  begin 
                                  $write("  readback vv=%1d pp=%1d qq=%1d", TESTMAIN400_test41r1_Main_V_4, ((32'h5/*5:kiwiTESTMAIN400PC10nz*/==
                                  kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold), ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==
                                  kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah12hold));
                                  $display("    dd=%1d ss=%1d", (isDIVIDERALUS3210RRh10vld? isDIVIDERALUS3210RRh10hold: isDIVIDERALUS32_10_RR
                                  ), $signed(TESTMAIN400_test41r1_Main_V_3+(isDIVIDERALUS3210RRh10vld? isDIVIDERALUS3210RRh10hold: isDIVIDERALUS32_10_RR
                                  )));
                                   end 
                                  if (isDIVIDERALUS3210RRh10vld || isDIVIDERALUS32_10_ACK)  begin 
                                  if ((TESTMAIN400_test41r1_Main_V_4<32'sd120) && !kiwiTESTMAIN400PC10_stall)  begin 
                                          $write("  readback vv=%1d pp=%1d qq=%1d", TESTMAIN400_test41r1_Main_V_4, ((32'h5/*5:kiwiTESTMAIN400PC10nz*/==
                                          kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold), ((32'h6
                                          /*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah12hold
                                          ));
                                          $display("    dd=%1d ss=%1d", (isDIVIDERALUS3210RRh10vld? isDIVIDERALUS3210RRh10hold: isDIVIDERALUS32_10_RR
                                          ), $signed(TESTMAIN400_test41r1_Main_V_3+(isDIVIDERALUS3210RRh10vld? isDIVIDERALUS3210RRh10hold
                                          : isDIVIDERALUS32_10_RR)));
                                           end 
                                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                           TESTMAIN400_test41r1_Main_V_3 <= $signed(TESTMAIN400_test41r1_Main_V_3+(isDIVIDERALUS3210RRh10vld
                                          ? isDIVIDERALUS3210RRh10hold: isDIVIDERALUS32_10_RR));

                                           TESTMAIN400_test41r1_Main_V_4 <= $signed(32'sd1+TESTMAIN400_test41r1_Main_V_4);
                                           end 
                                           kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                  if (!isDIVIDERALUS3210RRh10vld && !isDIVIDERALUS32_10_ACK && !kiwiTESTMAIN400PC10_stall)  begin 
                                   TESTMAIN400_test41r1_Main_V_3 <= $signed(TESTMAIN400_test41r1_Main_V_3+(isDIVIDERALUS3210RRh10vld? isDIVIDERALUS3210RRh10hold
                                  : isDIVIDERALUS32_10_RR));

                                   TESTMAIN400_test41r1_Main_V_4 <= $signed(32'sd1+TESTMAIN400_test41r1_Main_V_4);
                                   end 
                                   end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               pipe12 <= 32'd0;
               pipe10 <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10hold <= 32'd0;
               SINTCCSCALbx10ARA0rdatah12hold <= 32'd0;
               isDIVIDERALUS3210RRh10primed <= 32'd0;
               isDIVIDERALUS3210RRh10vld <= 32'd0;
               isDIVIDERALUS3210RRh10hold <= 32'd0;
               SINTCCSCALbx10ARA0rdatah12shot0 <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14)  begin 
                  if (isDIVIDERALUS32_10_ACK && isDIVIDERALUS3210RRh10primed)  begin 
                           isDIVIDERALUS3210RRh10primed <= 32'd0;
                           isDIVIDERALUS3210RRh10vld <= 32'd1;
                           isDIVIDERALUS3210RRh10hold <= isDIVIDERALUS32_10_RR;
                           end 
                          if (!kiwiTESTMAIN400PC10_stall && kiwiTESTMAIN400PC10_clear)  isDIVIDERALUS3210RRh10vld <= 32'd0;
                      if (SINTCCSCALbx10ARA0rdatah12shot0)  SINTCCSCALbx10ARA0rdatah12hold <= A_SINT_CC_SCALbx10_ARA0_rdata;
                      if (SINTCCSCALbx10ARA0rdatah10shot0)  SINTCCSCALbx10ARA0rdatah10hold <= A_SINT_CC_SCALbx10_ARA0_rdata;
                       kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   pipe12 <= 32'sd40+TESTMAIN400_test41r1_Main_V_4;
                   pipe10 <= ((32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold
                  );

                   SINTCCSCALbx10ARA0rdatah12shot0 <= (32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !kiwiTESTMAIN400PC10_stall
                  ;

                   SINTCCSCALbx10ARA0rdatah10shot0 <= (TESTMAIN400_test41r1_Main_V_4<32'sd120) && (32'h4/*4:kiwiTESTMAIN400PC10nz*/==
                  kiwiTESTMAIN400PC10nz) && !kiwiTESTMAIN400PC10_stall;

                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

 always   @(* )  begin 
       A_SINT_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wdata = 32'sd0;
       isDIVIDERALUS32_10_NN = 32'sd0;
       isDIVIDERALUS32_10_DD = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_ren = 32'sd0;
       isDIVIDERALUS32_10_REQ = 32'sd0;
       hpr_int_run_enable_DDX14 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h2/*2:kiwiTESTMAIN400PC10nz*/: if ((TESTMAIN400_test41r1_Main_V_2<32'sd180) && !kiwiTESTMAIN400PC10_stall)  begin 
                   A_SINT_CC_SCALbx10_ARA0_addr = TESTMAIN400_test41r1_Main_V_2;
                   A_SINT_CC_SCALbx10_ARA0_wdata = $signed(-32'sd98+TESTMAIN400_test41r1_Main_V_2);
                   end 
                  
          32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (!kiwiTESTMAIN400PC10_stall)  A_SINT_CC_SCALbx10_ARA0_addr = pipe12;
              
          32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (!kiwiTESTMAIN400PC10_stall)  begin 
                   isDIVIDERALUS32_10_NN = ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah12hold
                  );

                   isDIVIDERALUS32_10_DD = pipe10;
                   end 
                  
          32'h4/*4:kiwiTESTMAIN400PC10nz*/: if ((TESTMAIN400_test41r1_Main_V_4<32'sd120) && !kiwiTESTMAIN400PC10_stall)  A_SINT_CC_SCALbx10_ARA0_addr
               = TESTMAIN400_test41r1_Main_V_4;

              endcase
      if (!kiwiTESTMAIN400PC10_stall && hpr_int_run_enable_DDX14)  begin 
               A_SINT_CC_SCALbx10_ARA0_wen = (TESTMAIN400_test41r1_Main_V_2<32'sd180) && (32'h2/*2:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
              );

               A_SINT_CC_SCALbx10_ARA0_ren = (32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (TESTMAIN400_test41r1_Main_V_4
              <32'sd120) && (32'h4/*4:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

               isDIVIDERALUS32_10_REQ = ((32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);
               end 
               hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
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
      

always @(*) kiwiTESTMAIN400PC10_clear = (32'h0/*0:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h4/*4:kiwiTESTMAIN400PC10nz*/==
kiwiTESTMAIN400PC10nz) || (32'h2/*2:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

always @(*) kiwiTESTMAIN400PC10_stall = (32'h12/*18:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !isDIVIDERALUS3210RRh10vld && !isDIVIDERALUS32_10_ACK;

  CV_INT_VL_DIVIDER_S #(.RWIDTH(32'sd32), .NWIDTH(32'sd32), .DWIDTH(32'sd32), .trace_me(32'sd0)) isDIVIDERALUS32_10(
        .clk(clk
),
        .reset(reset),
        .REQ(isDIVIDERALUS32_10_REQ),
        .ACK(isDIVIDERALUS32_10_ACK),
        .RR(isDIVIDERALUS32_10_RR
),
        .NN(isDIVIDERALUS32_10_NN),
        .DD(isDIVIDERALUS32_10_DD),
        .FAIL(isDIVIDERALUS32_10_FAIL));

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd14),
        .WORDS(32'sd10000),
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

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 5
// 10 vectors of width 1
// 11 vectors of width 32
// 1 vectors of width 14
// Total state bits in module = 381 bits.
// 66 continuously assigned (wire/non-state) bits 
//   cell CV_INT_VL_DIVIDER_S count=1
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 2
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:20
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test41r1.exe -sim=1800 -vnl-resets=synchronous -vnl DUT.v -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| Class    | Style   | Dir Style                                                                                            | Timing Target | Method        | UID    | Skip  |
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*
//| test41r1 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test41r1.Main | Main10 | false |
//*----------+---------+------------------------------------------------------------------------------------------------------+---------------+---------------+--------+-------*

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
//KiwiC: front end input processing of class test41r1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test41r1.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test41r1.Main
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
//   srcfile=test41r1.exe
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
//PC codings points for kiwiTESTMAIN400PC10 
//*--------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause                  | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*--------------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10" | 811 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN400PC10" | 810 | 1       | hwm=0.0.0    | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10" | 808 | 2       | hwm=0.0.1    | 2    |        | 3     | 3   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10" | 809 | 2       | hwm=0.0.0    | 2    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiTESTMAIN400PC10" | 806 | 4       | hwm=0.14.0   | 18   |        | 5     | 18  | 4    |
//| XU32'4:"4:kiwiTESTMAIN400PC10" | 807 | 4       | hwm=0.0.0    | 4    |        | -     | -   | -    |
//*--------------------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                   |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                        |
//| F0   | E811 | R0 DATA |                                                                                                                                                        |
//| F0+E | E811 | W0 DATA | TESTMAIN400.test41r1.Main.V_2write(S32'0) TESTMAIN400.test41r1.Main.V_4write(S32'0) TESTMAIN400.test41r1.Main.V_3write(S32'0)  PLI:Start Kiwi test41r1 |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                     |
//*------+------+---------+--------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                          |
//| F1   | E810 | R0 DATA |                                                                          |
//| F1+E | E810 | W0 DATA | TESTMAIN400.test41r1.Main.V_2write(S32'100)  PLI:The value of the int... |
//*------+------+---------+--------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                  |
//*------+------+---------+---------------------------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                                       |
//| F2   | E809 | R0 DATA |                                                                                       |
//| F2+E | E809 | W0 DATA | TESTMAIN400.test41r1.Main.V_4write(S32'110) TESTMAIN400.test41r1.Main.V_3write(S32'0) |
//| F2   | E808 | R0 DATA |                                                                                       |
//| F2+E | E808 | W0 DATA | TESTMAIN400.test41r1.Main.V_2write(E1) @_SINT/CC/SCALbx10_ARA0_write(E2, E3)          |
//| F3   | E808 | W1 DATA |                                                                                       |
//*------+------+---------+---------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*---------+------+----------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc      | eno  | Phaser   | Work                                                                                                                            |
//*---------+------+----------+---------------------------------------------------------------------------------------------------------------------------------*
//| F4      | -    | R0 CTRL  |                                                                                                                                 |
//| F4      | E807 | R0 DATA  |                                                                                                                                 |
//| F4+E    | E807 | W0 DATA  |  PLI:GSAI:hpr_sysexit  PLI:Press any key to exi...                                                                              |
//| F4      | E806 | R0 DATA  | @_SINT/CC/SCALbx10_ARA0_read(E4)                                                                                                |
//| F5      | E806 | R1 DATA  | @_SINT/CC/SCALbx10_ARA0_read(E5)                                                                                                |
//| F6+S    | E806 | R2 DATA  | isDIVIDERALUS32_10_compute(E6, E7)                                                                                              |
//| F7      | E806 | R3 DATA  |                                                                                                                                 |
//| F8      | E806 | R4 DATA  |                                                                                                                                 |
//| F9      | E806 | R5 DATA  |                                                                                                                                 |
//| F10     | E806 | R6 DATA  |                                                                                                                                 |
//| F11     | E806 | R7 DATA  |                                                                                                                                 |
//| F12     | E806 | R8 DATA  |                                                                                                                                 |
//| F13     | E806 | R9 DATA  |                                                                                                                                 |
//| F14     | E806 | R10 DATA |                                                                                                                                 |
//| F15     | E806 | R11 DATA |                                                                                                                                 |
//| F16     | E806 | R12 DATA |                                                                                                                                 |
//| F17     | E806 | R13 DATA |                                                                                                                                 |
//| F18+S   | E806 | R14 DATA |                                                                                                                                 |
//| F18+E+S | E806 | W0 DATA  | TESTMAIN400.test41r1.Main.V_4write(E8) TESTMAIN400.test41r1.Main.V_3write(E9)  PLI:    dd=%d ss=%d  PLI:  readback vv=%d pp=... |
//*---------+------+----------+---------------------------------------------------------------------------------------------------------------------------------*

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
//  E1 =.= C(1+TESTMAIN400.test41r1.Main.V_2)
//
//
//  E2 =.= TESTMAIN400.test41r1.Main.V_2
//
//
//  E3 =.= C(-98+TESTMAIN400.test41r1.Main.V_2)
//
//
//  E4 =.= TESTMAIN400.test41r1.Main.V_4
//
//
//  E5 =.= 40+TESTMAIN400.test41r1.Main.V_4
//
//
//  E6 =.= C(@_SINT/CC/SCALbx10_ARA0[40+TESTMAIN400.test41r1.Main.V_4])
//
//
//  E7 =.= C(@_SINT/CC/SCALbx10_ARA0[TESTMAIN400.test41r1.Main.V_4])
//
//
//  E8 =.= C(1+TESTMAIN400.test41r1.Main.V_4)
//
//
//  E9 =.= C(TESTMAIN400.test41r1.Main.V_3+(C((C(@_SINT/CC/SCALbx10_ARA0[40+TESTMAIN400.test41r1.Main.V_4]))/(C(@_SINT/CC/SCALbx10_ARA0[TESTMAIN400.test41r1.Main.V_4])))))
//
//
//  E10 =.= TESTMAIN400.test41r1.Main.V_2>=180
//
//
//  E11 =.= TESTMAIN400.test41r1.Main.V_2<180
//
//
//  E12 =.= TESTMAIN400.test41r1.Main.V_4>=120
//
//
//  E13 =.= TESTMAIN400.test41r1.Main.V_4<120
//
//
//  E14 =.= {[|isDIVIDERALUS3210RRh10vld|]; [|isDIVIDERALUS32_10_ACK|]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for DUT to DUT

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 5
//
//10 vectors of width 1
//
//11 vectors of width 32
//
//1 vectors of width 14
//
//Total state bits in module = 381 bits.
//
//66 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test41r1.Main uid=Main10 has 38 CIL instructions in 8 basic blocks
//Thread mpc10 has 4 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 19 minor control states
// eof (HPR L/S Verilog)
