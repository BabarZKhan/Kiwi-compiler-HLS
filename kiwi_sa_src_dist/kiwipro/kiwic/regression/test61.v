

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:17
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test61.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test61.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg done,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiBENCMAIN4001PC10nz_pc_export);

function [31:0] rtl_unsigned_bitextract0;
   input [63:0] arg;
   rtl_unsigned_bitextract0 = $unsigned(arg[31:0]);
   endfunction


function [31:0] rtl_unsigned_extend1;
   input [7:0] arg;
   rtl_unsigned_extend1 = { 24'b0, arg[7:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg [31:0] BENCMAIN400_bench_Main_V_8;
  reg [31:0] BENCMAIN400_bench_Main_V_7;
  reg [31:0] BENCMAIN400_bench_Main_V_6;
  reg [31:0] BENCMAIN400_bench_Main_V_5;
  reg [31:0] BENCMAIN400_bench_Main_V_4;
  reg [31:0] BENCMAIN400_bench_Main_V_3;
  reg [31:0] BENCMAIN400_bench_Main_V_2;
  reg [31:0] BENCMAIN400_bench_Main_V_1;
  reg signed [31:0] BENCMAIN400_bench_Main_V_0;
// abstractionName=res2-contacts pi_name=AS_SSROM_@8_US/CC/SCALbx10_ARA0
  wire [7:0] Z8USCCSCALbx10ARA0_10_rdata;
  reg [7:0] Z8USCCSCALbx10ARA0_10_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@8_US/CC/SCALbx10_ARA0
  wire [7:0] Z8USCCSCALbx10ARA0_12_rdata;
  reg [7:0] Z8USCCSCALbx10ARA0_12_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@8_US/CC/SCALbx10_ARA0
  wire [7:0] Z8USCCSCALbx10ARA0_14_rdata;
  reg [7:0] Z8USCCSCALbx10ARA0_14_addr;
// abstractionName=res2-contacts pi_name=AS_SSROM_@8_US/CC/SCALbx10_ARA0
  wire [7:0] Z8USCCSCALbx10ARA0_16_rdata;
  reg [7:0] Z8USCCSCALbx10ARA0_16_addr;
// abstractionName=res2-morenets
  reg [7:0] Z8USCCSCALbx10ARA016rdatah10hold;
  reg Z8USCCSCALbx10ARA016rdatah10shot0;
  reg [7:0] Z8USCCSCALbx10ARA014rdatah10hold;
  reg Z8USCCSCALbx10ARA014rdatah10shot0;
  reg [7:0] Z8USCCSCALbx10ARA012rdatah10hold;
  reg Z8USCCSCALbx10ARA012rdatah10shot0;
  reg [7:0] Z8USCCSCALbx10ARA010rdatah10hold;
  reg Z8USCCSCALbx10ARA010rdatah10shot0;
  reg [3:0] kiwiBENCMAIN4001PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire [31:0] hprpin502035x10;
  wire [31:0] hprpin502049x10;
  wire [31:0] hprpin502062x10;
  wire [31:0] hprpin502076x10;
  wire [31:0] hprpin502090x10;
  wire [31:0] hprpin502104x10;
  wire [31:0] hprpin502118x10;
  wire [31:0] hprpin502132x10;
  wire [31:0] hprpin502146x10;
  wire [31:0] hprpin502160x10;
  wire [31:0] hprpin502174x10;
  wire [31:0] hprpin502188x10;
  wire [31:0] hprpin502202x10;
  wire [31:0] hprpin502216x10;
  wire [31:0] hprpin502230x10;
  wire [31:0] hprpin502244x10;
  wire [31:0] hprpin502261x10;
  wire [31:0] hprpin502279x10;
  wire [31:0] hprpin502283x10;
  wire [31:0] hprpin502287x10;
  wire [31:0] hprpin502291x10;
  wire [31:0] hprpin502295x10;
  wire [31:0] hprpin502299x10;
  wire [31:0] hprpin502303x10;
  wire [31:0] hprpin502307x10;
  wire [31:0] hprpin502311x10;
  wire [31:0] hprpin502315x10;
  wire [31:0] hprpin502319x10;
  wire [31:0] hprpin502323x10;
  wire [31:0] hprpin502327x10;
  wire [31:0] hprpin502331x10;
  wire [31:0] hprpin502335x10;
  wire [31:0] hprpin502496x10;
 always   @(* )  begin 
       Z8USCCSCALbx10ARA0_16_addr = 32'sd0;
       Z8USCCSCALbx10ARA0_14_addr = 32'sd0;
       Z8USCCSCALbx10ARA0_12_addr = 32'sd0;
       Z8USCCSCALbx10ARA0_10_addr = 32'sd0;
       hpr_int_run_enable_DDX18 = 32'sd1;

      case (kiwiBENCMAIN4001PC10nz)
          32'h7/*7:kiwiBENCMAIN4001PC10nz*/:  begin 
               Z8USCCSCALbx10ARA0_16_addr = $unsigned(32'd255&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd8));
               Z8USCCSCALbx10ARA0_14_addr = $unsigned(32'd255&$signed(32'd31*BENCMAIN400_bench_Main_V_0));
               Z8USCCSCALbx10ARA0_12_addr = $unsigned(32'd255&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd16));
               Z8USCCSCALbx10ARA0_10_addr = $unsigned(32'd255&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd24));
               end 
              endcase
       hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.BENCMAIN400_1/1.0
      if (reset)  begin 
               kiwiBENCMAIN4001PC10nz <= 32'd0;
               done <= 32'd0;
               BENCMAIN400_bench_Main_V_8 <= 32'd0;
               BENCMAIN400_bench_Main_V_7 <= 32'd0;
               BENCMAIN400_bench_Main_V_6 <= 32'd0;
               BENCMAIN400_bench_Main_V_5 <= 32'd0;
               BENCMAIN400_bench_Main_V_4 <= 32'd0;
               BENCMAIN400_bench_Main_V_3 <= 32'd0;
               BENCMAIN400_bench_Main_V_2 <= 32'd0;
               BENCMAIN400_bench_Main_V_1 <= 32'd0;
               BENCMAIN400_bench_Main_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiBENCMAIN4001PC10nz)
                  32'h0/*0:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("%s%1d", "BitTally 1 Limit=", 32'sh5f5_e100);
                           kiwiBENCMAIN4001PC10nz <= 32'h1/*1:kiwiBENCMAIN4001PC10nz*/;
                           done <= 32'h0;
                           BENCMAIN400_bench_Main_V_8 <= 32'h0;
                           BENCMAIN400_bench_Main_V_7 <= 32'h0;
                           BENCMAIN400_bench_Main_V_6 <= 32'h0;
                           BENCMAIN400_bench_Main_V_5 <= 32'h0;
                           BENCMAIN400_bench_Main_V_4 <= 32'h0;
                           BENCMAIN400_bench_Main_V_3 <= 32'h0;
                           BENCMAIN400_bench_Main_V_2 <= 32'h0;
                           BENCMAIN400_bench_Main_V_1 <= 32'h0;
                           BENCMAIN400_bench_Main_V_0 <= 32'sh1;
                           end 
                          
                  32'h1/*1:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h2/*2:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_bench_Main_V_2 <= hprpin502244x10;
                           BENCMAIN400_bench_Main_V_1 <= $signed(32'd31*BENCMAIN400_bench_Main_V_0);
                           end 
                          
                  32'h2/*2:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0))) $display("   %1d  00 answers %1d", BENCMAIN400_bench_Main_V_1
                              , BENCMAIN400_bench_Main_V_2);
                               else  begin 
                                  $display("   %1d  00 answers %1d", BENCMAIN400_bench_Main_V_1, BENCMAIN400_bench_Main_V_2);
                                   kiwiBENCMAIN4001PC10nz <= 32'h3/*3:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= 32'sh1;
                                   end 
                                  if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0)))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h1/*1:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(32'sd21*BENCMAIN400_bench_Main_V_0);
                                   end 
                                   end 
                          
                  32'h3/*3:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h4/*4:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_bench_Main_V_4 <= $unsigned((rtl_unsigned_bitextract0(32'sh101_0101*(32'shf0f_0f0f&hprpin502261x10
                          +(hprpin502261x10>>32'sd4)))>>32'sd24));

                           BENCMAIN400_bench_Main_V_3 <= $signed(32'd31*BENCMAIN400_bench_Main_V_0);
                           end 
                          
                  32'h4/*4:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0))) $display("   %1d  01 answers %1d", BENCMAIN400_bench_Main_V_3
                              , BENCMAIN400_bench_Main_V_4);
                               else  begin 
                                  $display("   %1d  01 answers %1d", BENCMAIN400_bench_Main_V_3, BENCMAIN400_bench_Main_V_4);
                                   kiwiBENCMAIN4001PC10nz <= 32'h5/*5:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= 32'sh1;
                                   end 
                                  if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0)))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h3/*3:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(32'sd21*BENCMAIN400_bench_Main_V_0);
                                   end 
                                   end 
                          
                  32'h5/*5:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h6/*6:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_bench_Main_V_6 <= $unsigned(hprpin502335x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>
                          32'sd31)));

                           BENCMAIN400_bench_Main_V_5 <= $signed(32'd31*BENCMAIN400_bench_Main_V_0);
                           end 
                          
                  32'h6/*6:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0))) $display("   %1d  02 answers %1d", BENCMAIN400_bench_Main_V_5
                              , BENCMAIN400_bench_Main_V_6);
                               else  begin 
                                  $display("   %1d  02 answers %1d", BENCMAIN400_bench_Main_V_5, BENCMAIN400_bench_Main_V_6);
                                   kiwiBENCMAIN4001PC10nz <= 32'h7/*7:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= 32'sh1;
                                   end 
                                  if ((32'sh5f5_e100>=$signed(32'sd21*BENCMAIN400_bench_Main_V_0)))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h5/*5:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(32'sd21*BENCMAIN400_bench_Main_V_0);
                                   end 
                                   end 
                          
                  32'h8/*8:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiBENCMAIN4001PC10nz <= 32'h9/*9:kiwiBENCMAIN4001PC10nz*/;
                           BENCMAIN400_bench_Main_V_8 <= hprpin502496x10;
                           BENCMAIN400_bench_Main_V_7 <= $signed(32'd31*BENCMAIN400_bench_Main_V_0);
                           end 
                          
                  32'h7/*7:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiBENCMAIN4001PC10nz <= 32'h8/*8:kiwiBENCMAIN4001PC10nz*/;
                      
                  32'h9/*9:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          if ((32'sh5f5_e100<$signed(32'sd21*BENCMAIN400_bench_Main_V_0)))  begin 
                                  $display("   %1d  03 answers %1d", BENCMAIN400_bench_Main_V_7, BENCMAIN400_bench_Main_V_8);
                                  $display("Test61 BitTally finished.");
                                   end 
                                   else $display("   %1d  03 answers %1d", BENCMAIN400_bench_Main_V_7, BENCMAIN400_bench_Main_V_8);
                          if ((32'sh5f5_e100<$signed(32'sd21*BENCMAIN400_bench_Main_V_0)))  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'ha/*10:kiwiBENCMAIN4001PC10nz*/;
                                   done <= 32'h1;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(32'sd21*BENCMAIN400_bench_Main_V_0);
                                   end 
                                   else  begin 
                                   kiwiBENCMAIN4001PC10nz <= 32'h7/*7:kiwiBENCMAIN4001PC10nz*/;
                                   BENCMAIN400_bench_Main_V_0 <= $signed(32'sd21*BENCMAIN400_bench_Main_V_0);
                                   end 
                                   end 
                          
                  32'ha/*10:kiwiBENCMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $finish(32'sd0);
                           kiwiBENCMAIN4001PC10nz <= 32'hb/*11:kiwiBENCMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  begin 
               kiwiBENCMAIN4001PC10nz_pc_export <= 32'd0;
               Z8USCCSCALbx10ARA010rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA012rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA014rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA016rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA016rdatah10shot0 <= 32'd0;
               Z8USCCSCALbx10ARA014rdatah10shot0 <= 32'd0;
               Z8USCCSCALbx10ARA012rdatah10shot0 <= 32'd0;
               Z8USCCSCALbx10ARA010rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18)  begin 
                  if (Z8USCCSCALbx10ARA016rdatah10shot0)  Z8USCCSCALbx10ARA016rdatah10hold <= Z8USCCSCALbx10ARA0_16_rdata;
                      if (Z8USCCSCALbx10ARA014rdatah10shot0)  Z8USCCSCALbx10ARA014rdatah10hold <= Z8USCCSCALbx10ARA0_14_rdata;
                      if (Z8USCCSCALbx10ARA012rdatah10shot0)  Z8USCCSCALbx10ARA012rdatah10hold <= Z8USCCSCALbx10ARA0_12_rdata;
                      if (Z8USCCSCALbx10ARA010rdatah10shot0)  Z8USCCSCALbx10ARA010rdatah10hold <= Z8USCCSCALbx10ARA0_10_rdata;
                       kiwiBENCMAIN4001PC10nz_pc_export <= kiwiBENCMAIN4001PC10nz;
                   Z8USCCSCALbx10ARA016rdatah10shot0 <= (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   Z8USCCSCALbx10ARA014rdatah10shot0 <= (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   Z8USCCSCALbx10ARA012rdatah10shot0 <= (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   Z8USCCSCALbx10ARA010rdatah10shot0 <= (32'h7/*7:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.BENCMAIN400_1/1.0


       end 
      

  AS_SSROM_A_8_US_CC_SCALbx10_ARA0 Z8USCCSCALbx10ARA0_10(.clk(clk), .reset(reset), .ASROM10_rdata(Z8USCCSCALbx10ARA0_10_rdata), .ASROM10_addr(Z8USCCSCALbx10ARA0_10_addr
));
  AS_SSROM_A_8_US_CC_SCALbx10_ARA0 Z8USCCSCALbx10ARA0_12(.clk(clk), .reset(reset), .ASROM10_rdata(Z8USCCSCALbx10ARA0_12_rdata), .ASROM10_addr(Z8USCCSCALbx10ARA0_12_addr
));
  AS_SSROM_A_8_US_CC_SCALbx10_ARA0 Z8USCCSCALbx10ARA0_14(.clk(clk), .reset(reset), .ASROM10_rdata(Z8USCCSCALbx10ARA0_14_rdata), .ASROM10_addr(Z8USCCSCALbx10ARA0_14_addr
));
  AS_SSROM_A_8_US_CC_SCALbx10_ARA0 Z8USCCSCALbx10ARA0_16(.clk(clk), .reset(reset), .ASROM10_rdata(Z8USCCSCALbx10ARA0_16_rdata), .ASROM10_addr(Z8USCCSCALbx10ARA0_16_addr
));
assign hprpin502035x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd1)? $unsigned(32'd1+(32'sd1&$signed(32'd31*BENCMAIN400_bench_Main_V_0)? 32'h1
: 32'h0)): (32'sd1&$signed(32'd31*BENCMAIN400_bench_Main_V_0)? 32'h1: 32'h0));

assign hprpin502049x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd3)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd2
)? $unsigned(32'd1+hprpin502035x10): hprpin502035x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd2)? $unsigned(32'd1
+hprpin502035x10): hprpin502035x10));

assign hprpin502062x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd5)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd4
)? $unsigned(32'd1+hprpin502049x10): hprpin502049x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd4)? $unsigned(32'd1
+hprpin502049x10): hprpin502049x10));

assign hprpin502076x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd7)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd6
)? $unsigned(32'd1+hprpin502062x10): hprpin502062x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd6)? $unsigned(32'd1
+hprpin502062x10): hprpin502062x10));

assign hprpin502090x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd9)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd8
)? $unsigned(32'd1+hprpin502076x10): hprpin502076x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd8)? $unsigned(32'd1
+hprpin502076x10): hprpin502076x10));

assign hprpin502104x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd11)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd10
)? $unsigned(32'd1+hprpin502090x10): hprpin502090x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd10)? $unsigned(32'd1
+hprpin502090x10): hprpin502090x10));

assign hprpin502118x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd13)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd12
)? $unsigned(32'd1+hprpin502104x10): hprpin502104x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd12)? $unsigned(32'd1
+hprpin502104x10): hprpin502104x10));

assign hprpin502132x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd15)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd14
)? $unsigned(32'd1+hprpin502118x10): hprpin502118x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd14)? $unsigned(32'd1
+hprpin502118x10): hprpin502118x10));

assign hprpin502146x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd17)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd16
)? $unsigned(32'd1+hprpin502132x10): hprpin502132x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd16)? $unsigned(32'd1
+hprpin502132x10): hprpin502132x10));

assign hprpin502160x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd19)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd18
)? $unsigned(32'd1+hprpin502146x10): hprpin502146x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd18)? $unsigned(32'd1
+hprpin502146x10): hprpin502146x10));

assign hprpin502174x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd21)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd20
)? $unsigned(32'd1+hprpin502160x10): hprpin502160x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd20)? $unsigned(32'd1
+hprpin502160x10): hprpin502160x10));

assign hprpin502188x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd23)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd22
)? $unsigned(32'd1+hprpin502174x10): hprpin502174x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd22)? $unsigned(32'd1
+hprpin502174x10): hprpin502174x10));

assign hprpin502202x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd25)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd24
)? $unsigned(32'd1+hprpin502188x10): hprpin502188x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd24)? $unsigned(32'd1
+hprpin502188x10): hprpin502188x10));

assign hprpin502216x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd27)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd26
)? $unsigned(32'd1+hprpin502202x10): hprpin502202x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd26)? $unsigned(32'd1
+hprpin502202x10): hprpin502202x10));

assign hprpin502230x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd29)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd28
)? $unsigned(32'd1+hprpin502216x10): hprpin502216x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd28)? $unsigned(32'd1
+hprpin502216x10): hprpin502216x10));

assign hprpin502244x10 = (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd31)? $unsigned(32'd1+(32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd30
)? $unsigned(32'd1+hprpin502230x10): hprpin502230x10)): (32'sd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd30)? $unsigned(32'd1
+hprpin502230x10): hprpin502230x10));

assign hprpin502261x10 = (32'sh3333_3333&($unsigned($signed(32'd31*BENCMAIN400_bench_Main_V_0)+(0-(32'sh5555_5555&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>
32'sd1))))>>32'sd2))+(32'sh3333_3333&$unsigned($signed(32'd31*BENCMAIN400_bench_Main_V_0)+(0-(32'sh5555_5555&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd1)))));

assign hprpin502279x10 = $unsigned($unsigned(32'd1&$signed(32'd31*BENCMAIN400_bench_Main_V_0))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd1)))+(32'd1
&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd2));

assign hprpin502283x10 = $unsigned(hprpin502279x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd3)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd4));

assign hprpin502287x10 = $unsigned(hprpin502283x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd5)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd6));

assign hprpin502291x10 = $unsigned(hprpin502287x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd7)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd8));

assign hprpin502295x10 = $unsigned(hprpin502291x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd9)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd10));

assign hprpin502299x10 = $unsigned(hprpin502295x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd11)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd12));

assign hprpin502303x10 = $unsigned(hprpin502299x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd13)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd14));

assign hprpin502307x10 = $unsigned(hprpin502303x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd15)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd16));

assign hprpin502311x10 = $unsigned(hprpin502307x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd17)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd18));

assign hprpin502315x10 = $unsigned(hprpin502311x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd19)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd20));

assign hprpin502319x10 = $unsigned(hprpin502315x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd21)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd22));

assign hprpin502323x10 = $unsigned(hprpin502319x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd23)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd24));

assign hprpin502327x10 = $unsigned(hprpin502323x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd25)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd26));

assign hprpin502331x10 = $unsigned(hprpin502327x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd27)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd28));

assign hprpin502335x10 = $unsigned(hprpin502331x10+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0)>>32'sd29)))+(32'd1&($signed(32'd31*BENCMAIN400_bench_Main_V_0
)>>32'sd30));

assign hprpin502496x10 = rtl_unsigned_extend1(((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? Z8USCCSCALbx10ARA0_16_rdata: Z8USCCSCALbx10ARA016rdatah10hold
))+rtl_unsigned_extend1(((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? Z8USCCSCALbx10ARA0_14_rdata: Z8USCCSCALbx10ARA014rdatah10hold
))+rtl_unsigned_extend1(((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? Z8USCCSCALbx10ARA0_12_rdata: Z8USCCSCALbx10ARA012rdatah10hold
))+rtl_unsigned_extend1(((32'h8/*8:kiwiBENCMAIN4001PC10nz*/==kiwiBENCMAIN4001PC10nz)? Z8USCCSCALbx10ARA0_10_rdata: Z8USCCSCALbx10ARA010rdatah10hold
));

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 4
// 5 vectors of width 1
// 8 vectors of width 8
// 9 vectors of width 32
// Total state bits in module = 361 bits.
// 1088 continuously assigned (wire/non-state) bits 
//   cell AS_SSROM_A_8_US_CC_SCALbx10_ARA0 count=4
// Total number of leaf cells = 4
endmodule



module AS_SSROM_A_8_US_CC_SCALbx10_ARA0(    
/* portgroup= abstractionName=res2-contacts pi_name=ASROM10 */
    output reg [7:0] ASROM10_rdata,
    input [7:0] ASROM10_addr,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets12 */
input clk,
    input reset);
// abstractionName=res2-sim-nets pi_name=ASROM10
  reg [7:0] RomData10[255:0];
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogAS_SSROM_@8_US/CC/SCALbx10_ARA0/1.0
      if (reset)  ASROM10_rdata <= 32'd0;
           else  ASROM10_rdata <= RomData10[ASROM10_addr];
      //End structure cvtToVerilogAS_SSROM_@8_US/CC/SCALbx10_ARA0/1.0


       end 
      

//Resource=SROM iname=Z8USCCSCALbx10ARA0_16 256x8 clk=posedge(clk) synchronous/pipeline=1 no_ports=1 <NONE> used by threads kiwiBENCMAIN4001PC10
 initial        begin 
      //ROM data table: 256 words of 8 bits.
       RomData10[0] = 8'h0;
       RomData10[1] = 8'h1;
       RomData10[2] = 8'h1;
       RomData10[3] = 8'h2;
       RomData10[4] = 8'h1;
       RomData10[5] = 8'h2;
       RomData10[6] = 8'h2;
       RomData10[7] = 8'h3;
       RomData10[8] = 8'h1;
       RomData10[9] = 8'h2;
       RomData10[10] = 8'h2;
       RomData10[11] = 8'h3;
       RomData10[12] = 8'h2;
       RomData10[13] = 8'h3;
       RomData10[14] = 8'h3;
       RomData10[15] = 8'h4;
       RomData10[16] = 8'h1;
       RomData10[17] = 8'h2;
       RomData10[18] = 8'h2;
       RomData10[19] = 8'h3;
       RomData10[20] = 8'h2;
       RomData10[21] = 8'h3;
       RomData10[22] = 8'h3;
       RomData10[23] = 8'h4;
       RomData10[24] = 8'h2;
       RomData10[25] = 8'h3;
       RomData10[26] = 8'h3;
       RomData10[27] = 8'h4;
       RomData10[28] = 8'h3;
       RomData10[29] = 8'h4;
       RomData10[30] = 8'h4;
       RomData10[31] = 8'h5;
       RomData10[32] = 8'h1;
       RomData10[33] = 8'h2;
       RomData10[34] = 8'h2;
       RomData10[35] = 8'h3;
       RomData10[36] = 8'h2;
       RomData10[37] = 8'h3;
       RomData10[38] = 8'h3;
       RomData10[39] = 8'h4;
       RomData10[40] = 8'h2;
       RomData10[41] = 8'h3;
       RomData10[42] = 8'h3;
       RomData10[43] = 8'h4;
       RomData10[44] = 8'h3;
       RomData10[45] = 8'h4;
       RomData10[46] = 8'h4;
       RomData10[47] = 8'h5;
       RomData10[48] = 8'h2;
       RomData10[49] = 8'h3;
       RomData10[50] = 8'h3;
       RomData10[51] = 8'h4;
       RomData10[52] = 8'h3;
       RomData10[53] = 8'h4;
       RomData10[54] = 8'h4;
       RomData10[55] = 8'h5;
       RomData10[56] = 8'h3;
       RomData10[57] = 8'h4;
       RomData10[58] = 8'h4;
       RomData10[59] = 8'h5;
       RomData10[60] = 8'h4;
       RomData10[61] = 8'h5;
       RomData10[62] = 8'h5;
       RomData10[63] = 8'h6;
       RomData10[64] = 8'h1;
       RomData10[65] = 8'h2;
       RomData10[66] = 8'h2;
       RomData10[67] = 8'h3;
       RomData10[68] = 8'h2;
       RomData10[69] = 8'h3;
       RomData10[70] = 8'h3;
       RomData10[71] = 8'h4;
       RomData10[72] = 8'h2;
       RomData10[73] = 8'h3;
       RomData10[74] = 8'h3;
       RomData10[75] = 8'h4;
       RomData10[76] = 8'h3;
       RomData10[77] = 8'h4;
       RomData10[78] = 8'h4;
       RomData10[79] = 8'h5;
       RomData10[80] = 8'h2;
       RomData10[81] = 8'h3;
       RomData10[82] = 8'h3;
       RomData10[83] = 8'h4;
       RomData10[84] = 8'h3;
       RomData10[85] = 8'h4;
       RomData10[86] = 8'h4;
       RomData10[87] = 8'h5;
       RomData10[88] = 8'h3;
       RomData10[89] = 8'h4;
       RomData10[90] = 8'h4;
       RomData10[91] = 8'h5;
       RomData10[92] = 8'h4;
       RomData10[93] = 8'h5;
       RomData10[94] = 8'h5;
       RomData10[95] = 8'h6;
       RomData10[96] = 8'h2;
       RomData10[97] = 8'h3;
       RomData10[98] = 8'h3;
       RomData10[99] = 8'h4;
       RomData10[100] = 8'h3;
       RomData10[101] = 8'h4;
       RomData10[102] = 8'h4;
       RomData10[103] = 8'h5;
       RomData10[104] = 8'h3;
       RomData10[105] = 8'h4;
       RomData10[106] = 8'h4;
       RomData10[107] = 8'h5;
       RomData10[108] = 8'h4;
       RomData10[109] = 8'h5;
       RomData10[110] = 8'h5;
       RomData10[111] = 8'h6;
       RomData10[112] = 8'h3;
       RomData10[113] = 8'h4;
       RomData10[114] = 8'h4;
       RomData10[115] = 8'h5;
       RomData10[116] = 8'h4;
       RomData10[117] = 8'h5;
       RomData10[118] = 8'h5;
       RomData10[119] = 8'h6;
       RomData10[120] = 8'h4;
       RomData10[121] = 8'h5;
       RomData10[122] = 8'h5;
       RomData10[123] = 8'h6;
       RomData10[124] = 8'h5;
       RomData10[125] = 8'h6;
       RomData10[126] = 8'h6;
       RomData10[127] = 8'h7;
       RomData10[128] = 8'h1;
       RomData10[129] = 8'h2;
       RomData10[130] = 8'h2;
       RomData10[131] = 8'h3;
       RomData10[132] = 8'h2;
       RomData10[133] = 8'h3;
       RomData10[134] = 8'h3;
       RomData10[135] = 8'h4;
       RomData10[136] = 8'h2;
       RomData10[137] = 8'h3;
       RomData10[138] = 8'h3;
       RomData10[139] = 8'h4;
       RomData10[140] = 8'h3;
       RomData10[141] = 8'h4;
       RomData10[142] = 8'h4;
       RomData10[143] = 8'h5;
       RomData10[144] = 8'h2;
       RomData10[145] = 8'h3;
       RomData10[146] = 8'h3;
       RomData10[147] = 8'h4;
       RomData10[148] = 8'h3;
       RomData10[149] = 8'h4;
       RomData10[150] = 8'h4;
       RomData10[151] = 8'h5;
       RomData10[152] = 8'h3;
       RomData10[153] = 8'h4;
       RomData10[154] = 8'h4;
       RomData10[155] = 8'h5;
       RomData10[156] = 8'h4;
       RomData10[157] = 8'h5;
       RomData10[158] = 8'h5;
       RomData10[159] = 8'h6;
       RomData10[160] = 8'h2;
       RomData10[161] = 8'h3;
       RomData10[162] = 8'h3;
       RomData10[163] = 8'h4;
       RomData10[164] = 8'h3;
       RomData10[165] = 8'h4;
       RomData10[166] = 8'h4;
       RomData10[167] = 8'h5;
       RomData10[168] = 8'h3;
       RomData10[169] = 8'h4;
       RomData10[170] = 8'h4;
       RomData10[171] = 8'h5;
       RomData10[172] = 8'h4;
       RomData10[173] = 8'h5;
       RomData10[174] = 8'h5;
       RomData10[175] = 8'h6;
       RomData10[176] = 8'h3;
       RomData10[177] = 8'h4;
       RomData10[178] = 8'h4;
       RomData10[179] = 8'h5;
       RomData10[180] = 8'h4;
       RomData10[181] = 8'h5;
       RomData10[182] = 8'h5;
       RomData10[183] = 8'h6;
       RomData10[184] = 8'h4;
       RomData10[185] = 8'h5;
       RomData10[186] = 8'h5;
       RomData10[187] = 8'h6;
       RomData10[188] = 8'h5;
       RomData10[189] = 8'h6;
       RomData10[190] = 8'h6;
       RomData10[191] = 8'h7;
       RomData10[192] = 8'h2;
       RomData10[193] = 8'h3;
       RomData10[194] = 8'h3;
       RomData10[195] = 8'h4;
       RomData10[196] = 8'h3;
       RomData10[197] = 8'h4;
       RomData10[198] = 8'h4;
       RomData10[199] = 8'h5;
       RomData10[200] = 8'h3;
       RomData10[201] = 8'h4;
       RomData10[202] = 8'h4;
       RomData10[203] = 8'h5;
       RomData10[204] = 8'h4;
       RomData10[205] = 8'h5;
       RomData10[206] = 8'h5;
       RomData10[207] = 8'h6;
       RomData10[208] = 8'h3;
       RomData10[209] = 8'h4;
       RomData10[210] = 8'h4;
       RomData10[211] = 8'h5;
       RomData10[212] = 8'h4;
       RomData10[213] = 8'h5;
       RomData10[214] = 8'h5;
       RomData10[215] = 8'h6;
       RomData10[216] = 8'h4;
       RomData10[217] = 8'h5;
       RomData10[218] = 8'h5;
       RomData10[219] = 8'h6;
       RomData10[220] = 8'h5;
       RomData10[221] = 8'h6;
       RomData10[222] = 8'h6;
       RomData10[223] = 8'h7;
       RomData10[224] = 8'h3;
       RomData10[225] = 8'h4;
       RomData10[226] = 8'h4;
       RomData10[227] = 8'h5;
       RomData10[228] = 8'h4;
       RomData10[229] = 8'h5;
       RomData10[230] = 8'h5;
       RomData10[231] = 8'h6;
       RomData10[232] = 8'h4;
       RomData10[233] = 8'h5;
       RomData10[234] = 8'h5;
       RomData10[235] = 8'h6;
       RomData10[236] = 8'h5;
       RomData10[237] = 8'h6;
       RomData10[238] = 8'h6;
       RomData10[239] = 8'h7;
       RomData10[240] = 8'h4;
       RomData10[241] = 8'h5;
       RomData10[242] = 8'h5;
       RomData10[243] = 8'h6;
       RomData10[244] = 8'h5;
       RomData10[245] = 8'h6;
       RomData10[246] = 8'h6;
       RomData10[247] = 8'h7;
       RomData10[248] = 8'h5;
       RomData10[249] = 8'h6;
       RomData10[250] = 8'h6;
       RomData10[251] = 8'h7;
       RomData10[252] = 8'h6;
       RomData10[253] = 8'h7;
       RomData10[254] = 8'h7;
       RomData10[255] = 8'h8;
       end 
      

// Structural Resource (FU) inventory for AS_SSROM_A_8_US_CC_SCALbx10_ARA0:// 256 array locations of width 8
// Total state bits in module = 2048 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:13
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test61.exe -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test61.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -give-backtrace -report-each-step


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
//KiwiC: front end input processing of class BitTally  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=BitTally..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=BitTally..cctor
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
//   srcfile=test61.exe
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
//PC codings points for kiwiBENCMAIN4001PC10 
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                       | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiBENCMAIN4001PC10"     | 827 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiBENCMAIN4001PC10"     | 826 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"     | 824 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'2:"2:kiwiBENCMAIN4001PC10"     | 825 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//| XU32'4:"4:kiwiBENCMAIN4001PC10"     | 823 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"     | 821 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'8:"8:kiwiBENCMAIN4001PC10"     | 822 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 3    |
//| XU32'16:"16:kiwiBENCMAIN4001PC10"   | 820 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 6    |
//| XU32'32:"32:kiwiBENCMAIN4001PC10"   | 818 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 7    |
//| XU32'32:"32:kiwiBENCMAIN4001PC10"   | 819 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 5    |
//| XU32'64:"64:kiwiBENCMAIN4001PC10"   | 817 | 7       | hwm=0.1.0   | 8    |        | 8     | 8   | 9    |
//| XU32'128:"128:kiwiBENCMAIN4001PC10" | 815 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 7    |
//| XU32'128:"128:kiwiBENCMAIN4001PC10" | 816 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'256:"256:kiwiBENCMAIN4001PC10" | 814 | 10      | hwm=0.0.0   | 10   |        | -     | -   | -    |
//*-------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'0:"0:kiwiBENCMAIN4001PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                     |
//| F0   | E827 | R0 DATA |                                                                                                                     |
//| F0+E | E827 | W0 DATA | BENCMAIN400.bench.Main.V_0write(S32'1) BENCMAIN400.bench.Main.V_1write(U32'0) BENCMAIN400.bench.Main.V_2write(U32'\ |
//|      |      |         | 0) BENCMAIN400.bench.Main.V_3write(U32'0) BENCMAIN400.bench.Main.V_4write(U32'0) BENCMAIN400.bench.Main.V_5write(U\ |
//|      |      |         | 32'0) BENCMAIN400.bench.Main.V_6write(U32'0) BENCMAIN400.bench.Main.V_7write(U32'0) BENCMAIN400.bench.Main.V_8writ\ |
//|      |      |         | e(U32'0) donewrite(U32'0)  PLI:BitTally 1 Limit=                                                                    |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'1:"1:kiwiBENCMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                    |
//*------+------+---------+-------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                         |
//| F1   | E826 | R0 DATA |                                                                         |
//| F1+E | E826 | W0 DATA | BENCMAIN400.bench.Main.V_1write(E1) BENCMAIN400.bench.Main.V_2write(E2) |
//*------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'2:"2:kiwiBENCMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                             |
//*------+------+---------+------------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                                  |
//| F2   | E825 | R0 DATA |                                                                  |
//| F2+E | E825 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E3)  PLI:   %u  00 answers %u    |
//| F2   | E824 | R0 DATA |                                                                  |
//| F2+E | E824 | W0 DATA | BENCMAIN400.bench.Main.V_0write(S32'1)  PLI:   %u  00 answers %u |
//*------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'4:"4:kiwiBENCMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                    |
//*------+------+---------+-------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                         |
//| F3   | E823 | R0 DATA |                                                                         |
//| F3+E | E823 | W0 DATA | BENCMAIN400.bench.Main.V_3write(E1) BENCMAIN400.bench.Main.V_4write(E4) |
//*------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'8:"8:kiwiBENCMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                             |
//*------+------+---------+------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                  |
//| F4   | E822 | R0 DATA |                                                                  |
//| F4+E | E822 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E3)  PLI:   %u  01 answers %u    |
//| F4   | E821 | R0 DATA |                                                                  |
//| F4+E | E821 | W0 DATA | BENCMAIN400.bench.Main.V_0write(S32'1)  PLI:   %u  01 answers %u |
//*------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'16:"16:kiwiBENCMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                    |
//*------+------+---------+-------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                         |
//| F5   | E820 | R0 DATA |                                                                         |
//| F5+E | E820 | W0 DATA | BENCMAIN400.bench.Main.V_5write(E1) BENCMAIN400.bench.Main.V_6write(E5) |
//*------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32:"32:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'32:"32:kiwiBENCMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                             |
//*------+------+---------+------------------------------------------------------------------*
//| F6   | -    | R0 CTRL |                                                                  |
//| F6   | E819 | R0 DATA |                                                                  |
//| F6+E | E819 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E3)  PLI:   %u  02 answers %u    |
//| F6   | E818 | R0 DATA |                                                                  |
//| F6+E | E818 | W0 DATA | BENCMAIN400.bench.Main.V_0write(S32'1)  PLI:   %u  02 answers %u |
//*------+------+---------+------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'64:"64:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'64:"64:kiwiBENCMAIN4001PC10"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                        |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                                                                                             |
//| F7   | E817 | R0 DATA | Z8USCCSCALbx10ARA0_10_read(E6) Z8USCCSCALbx10ARA0_12_read(E7) Z8USCCSCALbx10ARA0_14_read(E8) Z8USCCSCALbx10ARA0_16_read(E9) |
//| F8   | E817 | R1 DATA |                                                                                                                             |
//| F8+E | E817 | W0 DATA | BENCMAIN400.bench.Main.V_7write(E1) BENCMAIN400.bench.Main.V_8write(E10)                                                    |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'128:"128:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'128:"128:kiwiBENCMAIN4001PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                        |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                                                                             |
//| F9   | E816 | R0 DATA |                                                                                                             |
//| F9+E | E816 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E3) donewrite(U32'1)  PLI:Test61 BitTally fini...  PLI:   %u  03 answers %u |
//| F9   | E815 | R0 DATA |                                                                                                             |
//| F9+E | E815 | W0 DATA | BENCMAIN400.bench.Main.V_0write(E3)  PLI:   %u  03 answers %u                                               |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'256:"256:kiwiBENCMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiBENCMAIN4001PC10 state=XU32'256:"256:kiwiBENCMAIN4001PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F10   | -    | R0 CTRL |                       |
//| F10   | E814 | R0 DATA |                       |
//| F10+E | E814 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= Cu(31*BENCMAIN400.bench.Main.V_0)
//
//
//  E2 =.= COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>31), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>30), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>29), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>28), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>27), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>26), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>25), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>24), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>23), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>22), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>21), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>20), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>19), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>18), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>17), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>16), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>15), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>14), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>13), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>12), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>11), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>10), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>9), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>8), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>7), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>6), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>5), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>4), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>3), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>3), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>4), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>3), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>3), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0)))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1), Cu(1+(COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))), U32'1, U32'0))))), COND(|-|(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2), Cu(1+(COND(|-|(1&...), ..., ...))), ...)))))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...))), ...)
//
//
//  E3 =.= C(21*BENCMAIN400.bench.Main.V_0)
//
//
//  E4 =.= Cu((Cu(S32'16843009*(S32'252645135&(Cu((S32'858993459&(Cu((Cu(31*BENCMAIN400.bench.Main.V_0))+-(S32'1431655765&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1)))>>>2)+(S32'858993459&(Cu((Cu(31*BENCMAIN400.bench.Main.V_0))+-(S32'1431655765&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1))))))+((Cu((S32'858993459&(Cu((Cu(31*BENCMAIN400.bench.Main.V_0))+-(S32'1431655765&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1)))>>>2)+(S32'858993459&(Cu((Cu(31*BENCMAIN400.bench.Main.V_0))+-(S32'1431655765&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1))))))>>>4))))>>>24)
//
//
//  E5 =.= Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu((Cu(1&(Cu(31*BENCMAIN400.bench.Main.V_0))))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>1)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>2)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>3)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>4)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>5)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>6)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>7)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>8)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>9)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>10)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>11)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>12)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>13)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>14)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>15)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>16)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>17)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>18)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>19)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>20)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>21)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>22)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>23)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>24)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>25)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>26)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>27)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>28)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>29)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>30)))+(1&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>31))
//
//
//  E6 =.= CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>24)
//
//
//  E7 =.= CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>16)
//
//
//  E8 =.= CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0)))
//
//
//  E9 =.= CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>8)
//
//
//  E10 =.= Cu(@8_US/CC/SCALbx10_ARA0[CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>8)]+@8_US/CC/SCALbx10_ARA0[CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0)))]+@8_US/CC/SCALbx10_ARA0[CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>16)]+@8_US/CC/SCALbx10_ARA0[CVT(Cu)(255&(Cu(31*BENCMAIN400.bench.Main.V_0))>>>24)])
//
//
//  E11 =.= S32'100000000>=(C(21*BENCMAIN400.bench.Main.V_0))
//
//
//  E12 =.= S32'100000000<(C(21*BENCMAIN400.bench.Main.V_0))
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test61 to test61

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test61AS_SSROM_@8_US/CC/SCALbx10_ARA0 to test61AS_SSROM_@8_US_CC_SCALbx10_ARA0

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 4
//
//5 vectors of width 1
//
//8 vectors of width 8
//
//9 vectors of width 32
//
//Total state bits in module = 361 bits.
//
//1088 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for AS_SSROM_A_8_US_CC_SCALbx10_ARA0:
//256 array locations of width 8
//
//Total state bits in module = 2048 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread BitTally..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread bench..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread bench.Main uid=Main10 has 92 CIL instructions in 26 basic blocks
//Thread mpc10 has 10 bevelab control states (pauses)
//Reindexed thread kiwiBENCMAIN4001PC10 with 11 minor control states
// eof (HPR L/S Verilog)
