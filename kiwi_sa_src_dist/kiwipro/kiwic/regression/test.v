

// CBG Orangepath HPR L/S System

// Verilog output file generated at 08/01/2019 10:45:10
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.i : 18th December 2018 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=enable -compose=disable test0.exe -restructure=disable -sim=1800 -diosim-loglevel=10 -give-backtrace -report-each-step
`timescale 1ns/1ns


module test(    
/* portgroup= abstractionName=kiwicmiscio10 */
    output reg signed [63:0] test_wout,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);

function hpr_fp_dltd5; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd5_a, hpr_fp_dltd5_b;
  hpr_fp_dltd5 = ((hpr_fp_dltd5_a[63] && !hpr_fp_dltd5_b[63]) ? 1: (!hpr_fp_dltd5_a[63] && hpr_fp_dltd5_b[63])  ? 0 : (hpr_fp_dltd5_a[63]^(hpr_fp_dltd5_a[62:0]<hpr_fp_dltd5_b[62:0]))); 
   endfunction


function [15:0] rtl_unsigned_bitextract4;
   input [63:0] arg;
   rtl_unsigned_bitextract4 = $unsigned(arg[15:0]);
   endfunction


function [31:0] rtl_unsigned_bitextract2;
   input [63:0] arg;
   rtl_unsigned_bitextract2 = $unsigned(arg[31:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract1;
   input [31:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[7:0]);
   endfunction


function [31:0] rtl_unsigned_extend3;
   input [15:0] arg;
   rtl_unsigned_extend3 = { 16'b0, arg[15:0] };
   endfunction


function [31:0] rtl_unsigned_extend0;
   input [7:0] arg;
   rtl_unsigned_extend0 = { 24'b0, arg[7:0] };
   endfunction

// abstractionName=kiwicmainnets10
  reg/*fp*/  [63:0] TMAIN_IDL400_System_Math_Max_4_17_SPILL_256;
  reg/*fp*/  [63:0] TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256;
  reg signed [31:0] TMAIN_IDL400_System_Math_Max_1_13_SPILL_256;
  reg signed [31:0] TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256;
  reg signed [31:0] TMAIN_IDL400_test_st7_0_32_V_2;
  reg/*fp*/  [63:0] TMAIN_IDL400_test_st7_0_32_V_1;
  reg signed [31:0] TMAIN_IDL400_test_st7_0_32_V_0;
  reg [63:0] TMAIN_IDL400_test_st6_0_29_V_1;
  reg signed [31:0] TMAIN_IDL400_test_st6_0_29_V_0;
  reg signed [31:0] TMAIN_IDL400_test_st5_0_26_V_1;
  reg signed [31:0] TMAIN_IDL400_test_st4_0_23_V_2;
  reg [31:0] TMAIN_IDL400_test_st4_0_23_V_0;
  reg [31:0] TMAIN_IDL400_test_st3_0_20_V_3;
  reg [31:0] TMAIN_IDL400_test_st3_0_20_V_2;
  reg signed [31:0] TMAIN_IDL400_test_st3_0_20_V_1;
  reg [63:0] TMAIN_IDL400_test_st3_0_20_V_0;
  reg [31:0] TMAIN_IDL400_test_st0_0_11_V_7;
  reg [31:0] TMAIN_IDL400_test_st0_0_11_V_6;
  reg signed [31:0] TMAIN_IDL400_test_st0_0_11_V_5;
  reg [7:0] TMAIN_IDL400_test_st0_0_11_V_4;
  reg [7:0] TMAIN_IDL400_test_st0_0_11_V_3;
  reg [31:0] TMAIN_IDL400_test_st0_0_11_V_2;
  reg signed [31:0] TMAIN_IDL400_test_st0_0_11_V_1;
  reg [7:0] TMAIN_IDL400_test_st0_0_11_V_0;
  reg signed [31:0] test_t2;
  reg signed [31:0] test_t1;
// abstractionName=bevelab-pc-nets
  reg [4:0] kiwiTMAINIDL4001PC10;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogtest0/1.0
      if (reset)  begin 
               test_wout <= 64'd0;
               TMAIN_IDL400_test_st0_0_11_V_2 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_1 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_0 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_7 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_6 <= 32'd0;
               TMAIN_IDL400_test_st3_0_20_V_0 <= 64'd0;
               TMAIN_IDL400_test_st4_0_23_V_0 <= 32'd0;
               TMAIN_IDL400_test_st6_0_29_V_1 <= 64'd0;
               TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256 <= 32'd0;
               TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256 <= 64'd0;
               TMAIN_IDL400_test_st7_0_32_V_1 <= 64'd0;
               TMAIN_IDL400_System_Math_Max_4_17_SPILL_256 <= 64'd0;
               TMAIN_IDL400_test_st7_0_32_V_2 <= 32'd0;
               TMAIN_IDL400_System_Math_Max_1_13_SPILL_256 <= 32'd0;
               TMAIN_IDL400_test_st7_0_32_V_0 <= 32'd0;
               TMAIN_IDL400_test_st6_0_29_V_0 <= 32'd0;
               TMAIN_IDL400_test_st5_0_26_V_1 <= 32'd0;
               test_t2 <= 32'd0;
               test_t1 <= 32'd0;
               TMAIN_IDL400_test_st4_0_23_V_2 <= 32'd0;
               TMAIN_IDL400_test_st3_0_20_V_3 <= 32'd0;
               TMAIN_IDL400_test_st3_0_20_V_2 <= 32'd0;
               TMAIN_IDL400_test_st3_0_20_V_1 <= 32'd0;
               kiwiTMAINIDL4001PC10 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_5 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_3 <= 32'd0;
               TMAIN_IDL400_test_st0_0_11_V_4 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTMAINIDL4001PC10)
                  32'h1e/*30:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display(" st0 byte arith %1d: summ=%1d  prod=%1d", TMAIN_IDL400_test_st0_0_11_V_5, TMAIN_IDL400_test_st0_0_11_V_6
                          , TMAIN_IDL400_test_st0_0_11_V_7);
                           kiwiTMAINIDL4001PC10 <= 32'h3/*3:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st0_0_11_V_5 <= $signed(32'sd1+TMAIN_IDL400_test_st0_0_11_V_5);
                           TMAIN_IDL400_test_st0_0_11_V_3 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd1+TMAIN_IDL400_test_st0_0_11_V_3
                          ));

                           TMAIN_IDL400_test_st0_0_11_V_4 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd4+TMAIN_IDL400_test_st0_0_11_V_4
                          ));

                           end 
                          
                  32'h1d/*29:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("v2 %1H", TMAIN_IDL400_test_st3_0_20_V_2);
                          $display("v3 %1H", TMAIN_IDL400_test_st3_0_20_V_3);
                           kiwiTMAINIDL4001PC10 <= 32'h4/*4:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st3_0_20_V_1 <= $signed(32'sd1+TMAIN_IDL400_test_st3_0_20_V_1);
                           end 
                          
                  32'h1c/*28:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'h1d/*29:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st3_0_20_V_3 <= rtl_unsigned_bitextract2(64'shffff&TMAIN_IDL400_test_st3_0_20_V_0);
                           TMAIN_IDL400_test_st3_0_20_V_2 <= rtl_unsigned_extend3(rtl_unsigned_bitextract4(TMAIN_IDL400_test_st3_0_20_V_0
                          ));

                           end 
                          
                  32'h1b/*27:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("inloop  shr.uns %1h", TMAIN_IDL400_test_st4_0_23_V_0);
                          $display("        shr     %1h", -32'sd1);
                           kiwiTMAINIDL4001PC10 <= 32'h5/*5:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st4_0_23_V_2 <= $signed(32'sd1+TMAIN_IDL400_test_st4_0_23_V_2);
                           end 
                          
                  32'h1a/*26:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'h7/*7:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st5_0_26_V_1 <= $signed(32'sd1+TMAIN_IDL400_test_st5_0_26_V_1);
                           test_t2 <= $signed((test_t2>>>32'sd1));
                           test_t1 <= $signed((test_t1<<32'sd1));
                           end 
                          
                  32'h19/*25:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'h9/*9:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st6_0_29_V_0 <= $signed(32'sd1+TMAIN_IDL400_test_st6_0_29_V_0);
                           end 
                          
                  32'h18/*24:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'h19/*25:kiwiTMAINIDL4001PC10*/;
                      
                  32'h17/*23:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed((TMAIN_IDL400_test_st6_0_29_V_1<<32'sd1))<64'sh0)) $display("Went -ve");
                               kiwiTMAINIDL4001PC10 <= 32'h18/*24:kiwiTMAINIDL4001PC10*/;
                           end 
                          
                  32'h16/*22:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'hb/*11:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st7_0_32_V_0 <= $signed(32'sd1+TMAIN_IDL400_test_st7_0_32_V_0);
                           end 
                          
                  32'h15/*21:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("               st7 int max test %1d %1d", TMAIN_IDL400_test_st7_0_32_V_0, TMAIN_IDL400_System_Math_Max_1_13_SPILL_256
                          );
                           kiwiTMAINIDL4001PC10 <= 32'h16/*22:kiwiTMAINIDL4001PC10*/;
                           end 
                          
                  32'h14/*20:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'h15/*21:kiwiTMAINIDL4001PC10*/;
                      
                  32'h13/*19:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((32'sd0>=TMAIN_IDL400_test_st7_0_32_V_0)) $display("  st7 int abs test %1d %1d", TMAIN_IDL400_test_st7_0_32_V_0
                              , TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256);
                               else  begin 
                                  $display("  st7 int abs test %1d %1d", TMAIN_IDL400_test_st7_0_32_V_0, TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256
                                  );
                                   kiwiTMAINIDL4001PC10 <= 32'h15/*21:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Max_1_13_SPILL_256 <= TMAIN_IDL400_test_st7_0_32_V_0;
                                   end 
                                  if ((32'sd0>=TMAIN_IDL400_test_st7_0_32_V_0))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h14/*20:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Max_1_13_SPILL_256 <= 32'sh0;
                                   end 
                                   end 
                          
                  32'h12/*18:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'h13/*19:kiwiTMAINIDL4001PC10*/;
                      
                  32'h11/*17:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'hc/*12:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st7_0_32_V_2 <= $signed(32'sd1+TMAIN_IDL400_test_st7_0_32_V_2);
                           end 
                          
                  32'h10/*16:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("               st7 fp max test %1d %f", TMAIN_IDL400_test_st7_0_32_V_2, $bitstoreal(TMAIN_IDL400_System_Math_Max_4_17_SPILL_256
                          ));
                           kiwiTMAINIDL4001PC10 <= 32'h11/*17:kiwiTMAINIDL4001PC10*/;
                           end 
                          
                  32'hf/*15:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'h10/*16:kiwiTMAINIDL4001PC10*/;
                      
                  32'he/*14:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!hpr_fp_dltd5(64'h3ff8_0000_0000_0000, TMAIN_IDL400_test_st7_0_32_V_1)) $display("  st7 fp abs test %1d %f"
                              , TMAIN_IDL400_test_st7_0_32_V_2, $bitstoreal(TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256));
                               else  begin 
                                  $display("  st7 fp abs test %1d %f", TMAIN_IDL400_test_st7_0_32_V_2, $bitstoreal(TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256
                                  ));
                                   kiwiTMAINIDL4001PC10 <= 32'h10/*16:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Max_4_17_SPILL_256 <= TMAIN_IDL400_test_st7_0_32_V_1;
                                   end 
                                  if (!hpr_fp_dltd5(64'h3ff8_0000_0000_0000, TMAIN_IDL400_test_st7_0_32_V_1))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'hf/*15:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Max_4_17_SPILL_256 <= 64'h3ff8_0000_0000_0000;
                                   end 
                                   end 
                          
                  32'hd/*13:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'he/*14:kiwiTMAINIDL4001PC10*/;
                      
                  32'hc/*12:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st7_0_32_V_2>=32'sd6))  begin 
                                  $display("Test0 complete %% %%%% %%%%%% percent.");
                                  $finish(32'sd0);
                                   kiwiTMAINIDL4001PC10 <= 32'bx;
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                  if ((TMAIN_IDL400_test_st7_0_32_V_2<32'sd6) && hpr_fp_dltd5(64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1
                          , 32'sd0))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'he/*14:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256 <= 64'sh8000_0000_0000_0000^64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1
                                  ;

                                   TMAIN_IDL400_test_st7_0_32_V_1 <= 64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1;
                                   end 
                                  if ((TMAIN_IDL400_test_st7_0_32_V_2<32'sd6) && !hpr_fp_dltd5(64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1
                          , 32'sd0))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'hd/*13:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256 <= 64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1
                                  ;

                                   TMAIN_IDL400_test_st7_0_32_V_1 <= 64'h3ff1_9999_9999_999a+TMAIN_IDL400_test_st7_0_32_V_1;
                                   end 
                                   end 
                          
                  32'hb/*11:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st7_0_32_V_0>=32'sd3))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'hc/*12:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st7_0_32_V_2 <= -32'sh2;
                                   TMAIN_IDL400_test_st7_0_32_V_1 <= 64'hc000_0000_0000_0000;
                                   end 
                                  if ((TMAIN_IDL400_test_st7_0_32_V_0<32'sd0))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h13/*19:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256 <= $signed((0-TMAIN_IDL400_test_st7_0_32_V_0));
                                   end 
                                  if ((TMAIN_IDL400_test_st7_0_32_V_0<32'sd3) && (TMAIN_IDL400_test_st7_0_32_V_0>=32'sd0))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h12/*18:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256 <= TMAIN_IDL400_test_st7_0_32_V_0;
                                   end 
                                   end 
                          
                  32'ha/*10:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'hb/*11:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st7_0_32_V_0 <= -32'sh2;
                           end 
                          
                  32'h9/*9:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st6_0_29_V_0>=32'sd4)) $display("Test0 st7");
                               else  begin 
                                  $display("dfsin ulong to long -ve kk=%1d pp=%1H %1d", TMAIN_IDL400_test_st6_0_29_V_0, $unsigned((64'hff00_0000
                                  <<(32'sd63&32'sd16*TMAIN_IDL400_test_st6_0_29_V_0))), ($unsigned((64'hff00_0000<<(32'sd63&32'sd16*TMAIN_IDL400_test_st6_0_29_V_0
                                  )))<<32'sd1));
                                   kiwiTMAINIDL4001PC10 <= 32'h17/*23:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st6_0_29_V_1 <= $unsigned((64'hff00_0000<<(32'sd63&32'sd16*TMAIN_IDL400_test_st6_0_29_V_0
                                  )));

                                   end 
                                  if ((TMAIN_IDL400_test_st6_0_29_V_0>=32'sd4))  kiwiTMAINIDL4001PC10 <= 32'ha/*10:kiwiTMAINIDL4001PC10*/;
                               end 
                          
                  32'h8/*8:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'h9/*9:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st6_0_29_V_0 <= 32'sh0;
                           end 
                          
                  32'h7/*7:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st5_0_26_V_1>=32'sd2)) $display("Test0 st6");
                               else  begin 
                                  $display("shipton test st5b: one's complement  %1d %1d --> %1d", test_t1, test_t2, $signed(~test_t1
                                  +~test_t2));
                                   kiwiTMAINIDL4001PC10 <= 32'h1a/*26:kiwiTMAINIDL4001PC10*/;
                                   end 
                                  if ((TMAIN_IDL400_test_st5_0_26_V_1>=32'sd2))  kiwiTMAINIDL4001PC10 <= 32'h8/*8:kiwiTMAINIDL4001PC10*/;
                               end 
                          
                  32'h6/*6:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTMAINIDL4001PC10 <= 32'h7/*7:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st5_0_26_V_1 <= 32'sh0;
                           end 
                          
                  32'h5/*5:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st4_0_23_V_2>=32'sd3))  begin 
                                  $display("Test0 st5");
                                  $display("shipton test st5a: one's complement  %1d %1d --> %1d", 32'sd32, 32'sd101, -32'sd135);
                                   end 
                                   else  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h1b/*27:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st4_0_23_V_0 <= $unsigned((TMAIN_IDL400_test_st4_0_23_V_0>>32'sd8));
                                   end 
                                  if ((TMAIN_IDL400_test_st4_0_23_V_2>=32'sd3))  kiwiTMAINIDL4001PC10 <= 32'h6/*6:kiwiTMAINIDL4001PC10*/;
                               end 
                          
                  32'h4/*4:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st3_0_20_V_1>=32'sd2))  begin 
                                  $display("Test0 st4");
                                  $display("preloop   shr.uns %1h", 32'shff_ffff);
                                  $display("preloop   shr     %1h", -32'sd1);
                                  $display();
                                   end 
                                   else  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h1c/*28:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st3_0_20_V_0 <= $unsigned(64'sh10_0000+TMAIN_IDL400_test_st3_0_20_V_0);
                                   end 
                                  if ((TMAIN_IDL400_test_st3_0_20_V_1>=32'sd2))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h5/*5:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st4_0_23_V_2 <= 32'sh0;
                                   TMAIN_IDL400_test_st4_0_23_V_0 <= 32'hffff_ffff;
                                   end 
                                   end 
                          
                  32'h3/*3:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TMAIN_IDL400_test_st0_0_11_V_5>=32'sd4))  begin 
                                  $display("Test0 st1");
                                  $display("<><><>> mask: %1H", 64'hffff_ffff_ffff_00ff);
                                  $display("Test0 st2");
                                  $display("<><><>> data: %1H", 32'shdead_00ef);
                                  $display("Test0 st3");
                                   end 
                                   else  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h1e/*30:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st0_0_11_V_7 <= $unsigned(rtl_unsigned_extend0(TMAIN_IDL400_test_st0_0_11_V_4)*rtl_unsigned_extend0(TMAIN_IDL400_test_st0_0_11_V_3
                                  ));

                                   TMAIN_IDL400_test_st0_0_11_V_6 <= $unsigned(rtl_unsigned_extend0(TMAIN_IDL400_test_st0_0_11_V_4)+rtl_unsigned_extend0(TMAIN_IDL400_test_st0_0_11_V_3
                                  ));

                                   end 
                                  if ((TMAIN_IDL400_test_st0_0_11_V_5>=32'sd4))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h4/*4:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st3_0_20_V_1 <= 32'sh0;
                                   TMAIN_IDL400_test_st3_0_20_V_0 <= 64'h1234_5678;
                                   end 
                                   end 
                          
                  32'h2/*2:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display(" st0 byte left shift: %1H %1H", TMAIN_IDL400_test_st0_0_11_V_0, TMAIN_IDL400_test_st0_0_11_V_2);
                          if (($signed(32'sd1+TMAIN_IDL400_test_st0_0_11_V_1)<32'sd2))  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h1/*1:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st0_0_11_V_2 <= $unsigned((rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd1+TMAIN_IDL400_test_st0_0_11_V_0
                                  ))<<32'sd24));

                                   TMAIN_IDL400_test_st0_0_11_V_1 <= $signed(32'sd1+TMAIN_IDL400_test_st0_0_11_V_1);
                                   TMAIN_IDL400_test_st0_0_11_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd1+TMAIN_IDL400_test_st0_0_11_V_0
                                  ));

                                   end 
                                   else  begin 
                                   kiwiTMAINIDL4001PC10 <= 32'h3/*3:kiwiTMAINIDL4001PC10*/;
                                   TMAIN_IDL400_test_st0_0_11_V_5 <= 32'sh0;
                                   TMAIN_IDL400_test_st0_0_11_V_4 <= 32'h0;
                                   TMAIN_IDL400_test_st0_0_11_V_3 <= 32'hfa;
                                   TMAIN_IDL400_test_st0_0_11_V_1 <= $signed(32'sd1+TMAIN_IDL400_test_st0_0_11_V_1);
                                   TMAIN_IDL400_test_st0_0_11_V_0 <= rtl_unsigned_extend0(rtl_unsigned_bitextract1(8'd1+TMAIN_IDL400_test_st0_0_11_V_0
                                  ));

                                   end 
                                   end 
                          
                  32'h1/*1:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  kiwiTMAINIDL4001PC10 <= 32'h2/*2:kiwiTMAINIDL4001PC10*/;
                      
                  32'h0/*0:kiwiTMAINIDL4001PC10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test0 st0");
                           kiwiTMAINIDL4001PC10 <= 32'h1/*1:kiwiTMAINIDL4001PC10*/;
                           TMAIN_IDL400_test_st0_0_11_V_2 <= 32'h4400_0000;
                           TMAIN_IDL400_test_st0_0_11_V_1 <= 32'sh0;
                           TMAIN_IDL400_test_st0_0_11_V_0 <= 32'h44;
                           test_wout <= 64'sh20_0000_0000;
                           TMAIN_IDL400_test_st0_0_11_V_3 <= 32'h0;
                           TMAIN_IDL400_test_st0_0_11_V_4 <= 32'h0;
                           TMAIN_IDL400_test_st0_0_11_V_5 <= 32'sh0;
                           TMAIN_IDL400_test_st0_0_11_V_6 <= 32'h0;
                           TMAIN_IDL400_test_st0_0_11_V_7 <= 32'h0;
                           TMAIN_IDL400_test_st3_0_20_V_0 <= 64'h0;
                           TMAIN_IDL400_test_st3_0_20_V_2 <= 32'h0;
                           TMAIN_IDL400_test_st3_0_20_V_3 <= 32'h0;
                           TMAIN_IDL400_test_st3_0_20_V_1 <= 32'sh0;
                           TMAIN_IDL400_test_st4_0_23_V_0 <= 32'h0;
                           TMAIN_IDL400_test_st4_0_23_V_2 <= 32'sh0;
                           TMAIN_IDL400_test_st5_0_26_V_1 <= 32'sh0;
                           TMAIN_IDL400_test_st6_0_29_V_0 <= 32'sh0;
                           TMAIN_IDL400_test_st6_0_29_V_1 <= 64'h0;
                           TMAIN_IDL400_test_st7_0_32_V_0 <= 32'sh0;
                           TMAIN_IDL400_System_Math_Abs_1_5_SPILL_256 <= 32'sh0;
                           TMAIN_IDL400_System_Math_Max_1_13_SPILL_256 <= 32'sh0;
                           TMAIN_IDL400_test_st7_0_32_V_1 <= 64'h0;
                           TMAIN_IDL400_test_st7_0_32_V_2 <= 32'sh0;
                           TMAIN_IDL400_System_Math_Abs_4_9_SPILL_256 <= 64'h0;
                           TMAIN_IDL400_System_Math_Max_4_17_SPILL_256 <= 64'h0;
                           test_t2 <= 32'sh65;
                           test_t1 <= 32'sh20;
                           end 
                          endcase
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogtest0/1.0


       end 
      

// Structural Resource (FU) inventory for test:// 1 vectors of width 1
// 1 vectors of width 5
// 18 vectors of width 32
// 3 vectors of width 8
// 5 vectors of width 64
// Total state bits in module = 926 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.8.i : 18th December 2018
//08/01/2019 10:45:07
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=enable -compose=disable test0.exe -restructure=disable -sim=1800 -diosim-loglevel=10 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+-----------+--------+-------*
//| Class | Style   | Dir Style                                                                                            | Timing Target | Method    | UID    | Skip  |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+-----------+--------+-------*
//| test  | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test.Main | Main10 | false |
//*-------+---------+------------------------------------------------------------------------------------------------------+---------------+-----------+--------+-------*

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
//KiwiC: front end input processing of class test  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test.Main
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
//   srcfile=test0.exe
//
//
//   kiwic-autodispose=disable
//
//
//END OF KIWIC REPORT FILE
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test to test

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test:
//1 vectors of width 1
//
//1 vectors of width 5
//
//18 vectors of width 32
//
//3 vectors of width 8
//
//5 vectors of width 64
//
//Total state bits in module = 926 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test..cctor uid=cctor10 has 2 CIL instructions in 1 basic blocks
//Thread test.Main uid=Main10 has 176 CIL instructions in 51 basic blocks
//Thread mpc10 has 31 bevelab control states (pauses)
// eof (HPR L/S Verilog)
