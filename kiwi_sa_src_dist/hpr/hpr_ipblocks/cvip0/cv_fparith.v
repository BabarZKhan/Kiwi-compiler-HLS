// cv_fparith.v
// (C) 1996-2014 DJ Greaves.
//  University of Cambridge, Computer Laboratory.
//
// Several floating-point hardware modules.
// Check also David Bishop’s IEEE Proposed library 1 .  http://www.eda.org/fphdl/
//

// Overall, this file should be junked and using good quality output from FloPoCo instead should be substituted.
// In theory, the ALU name and clock-enable details  should be specifiable in IP-XACT sometime soon, as is supported for black-box
// and incremental compilation components, but for now the ALU signatures are hardcoded in KiwiC/hpr.restructure, except for their
// latencies.

// June 2017 - Better clock speed for FP_DIVIDER, but big latency and still rather poor - let's find a better public-domain one.


//=============================================================================================================
// Single-Precision, Flash, Floating Point Multiplier.  32=1+8+23
// 
module CV_FP_FLASH_MULTIPLIER_SP(
       input clk,
       input reset,
       output  [31:0] RR,
       input   [31:0] XX,
       input   [31:0] YY,
       output FAIL
       );

  wire [7:0] exp1, exp2;
  wire [22:0] mant1,mant2; 
  wire s1, s2;
  reg [31:0] arg1, arg2, res0, res1, res2, res3; // Fixed pipeline latency 5 (FL5).
  reg 	     fl0, fl1, fl2, fl3;
  assign { s1, exp1, mant1 } = arg1;  // Deconstruct input arg1
  assign { s2, exp2, mant2 } = arg2;  // Deconstruct input arg2

  wire z1 = exp1 == 8'd0; // Check arg1 is zero or denormal.
  wire z2 = exp2 == 8'd0; // Check arg2 is zero or denormal.

   wire nan1 = exp1 == 8'hff && mant1 != 0;
   wire nan2 = exp2 == 8'hff && mant2 != 0;

   wire inf1 = exp1 == 8'hff && mant1 == 0;
   wire inf2 = exp2 == 8'hff && mant2 == 0;

   wire nanr = nan1 || nan2; // Highest precedence
   wire zr = z1 || z2;       // Zero comes next.
   wire infr = inf1 || inf2; // Infinity lowest precedence special result.

   wire [8:0] exp_r0 = {1'b0, exp1 } + { 1'b0, exp2 }; // Add exponents as exponent for result
   wire [47:0] mant_r0 = {1'b1, mant1 } * {1'b1, mant2};
   
   wire growth = mant_r0[105];
   wire [8:0] exp_r1 =  (growth) ? exp_r0+12'd1: exp_r0;
   wire [47:0] mant_r1 = (growth) ? mant_r0 >> 1: mant_r0;

   wire too_small = !zr && exp_r1 < 0+128;
   wire too_large = !zr && exp_r1 > 256+127;

   wire [10:0] exp_r2 = exp_r1 - 8'd127;
   wire [22:0] mant_r2 = mant_r1 >> 23;  // Bit 23 would be the hidden bit. TODO: Need to round.

  // For multiply of normalised two-bit numbers we have the following cases:
  //   2*2=4, 2*3=6 and 3*3=9.  The maximum amount of renomalisation is one place.
  // Treating all denomals as zero means that no other renormalisation is ever needed.
  wire [31:0] rslt =
    nanr ? { 1'b0, 8'hff, -23'd1 }:                // NaN highest	      
    (too_small || zr) ?   { s1 ^ s2, 8'd0,   23'd0 }: // Zero mid
    (too_large || infr) ? { s1 ^ s2, 8'hff, 23'd0 }:  // Inf: lowest precedence special result
    { s1 ^ s2, exp_r2, mant_r2[22:0] };

   wire FAIL_flag = too_large; 
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck!
      arg1 <= XX;
      arg2 <= YY;
      { res0, fl0 } <= { rslt, FAIL_flag };
      { res1, fl1 } <= { res0, fl0 };
      { res2, fl2 } <= { res1, fl1 };
      { res3, fl3 } <= { res2, fl2 };
   end

   assign RR = res3;
   assign FAIL = fl3;
endmodule

//=======================================================================================
// fixed-latency, Single-Precision multiply: IEEE 1+8+23 = 32-bit format.
module CV_FP_FL4_MULTIPLIER_SP(
       input clk,
       input reset,
       output [31:0] RR,
       input  [31:0] XX,
       input  [31:0] YY,
       output FAIL
       );


   reg [7:0]  exp_l_1, exp_r_1;
   reg [22:0] maint_l_1, maint_r_1; 
   reg s_l_1, s_r_1;

   reg [7:0] exp_l_2, exp_r_2;
   reg [31:0] arg_l_1, arg_r_1; // Fixed pipeline latency (FL) 4 now.
   reg [31:0] res_4;
   reg 	      fl_4;
   always @(*) begin
       { s_l_1, exp_l_1, maint_l_1 } <= arg_l_1;  // Deconstruct input arg1 - ensure maint is registered for DSP input
      { s_r_1, exp_r_1, maint_r_1 } <= arg_r_1;  // Deconstruct input arg2
   end
   
   // IEEE decode
   wire        z_l_1 = exp_l_1 == 8'd0; // Check left arg is zero or denormal.
   wire        z_r_1 = exp_r_1 == 8'd0; // Check right arg is zero or denormal.
   wire nan_l_1 = exp_l_1 == 8'hff && maint_l_1 != 0;
   wire nan_r_1 = exp_r_1 == 8'hff && maint_r_1 != 0;
   wire inf_l_1 = exp_l_1 == 8'hff && maint_l_1 == 0;
   wire inf_r_1 = exp_r_1 == 8'hff && maint_r_1 == 0;

   // Result logic
   wire nan_y_1 = nan_l_1 || nan_r_1; // Highest precedence
   wire z_y_1 = z_l_1 || z_r_1;       // Zero comes next.
   wire inf_y_1 = inf_l_1 || inf_r_1; // Infinity lowest precedence special result.

   reg 	nan_y_2, z_y_2, inf_y_2, s_y_2;
   reg [47:0] maint_y_3;
   reg 	      too_small_y_3, too_large_y_3, inf_y_3, z_y_3, s_y_3;
   wire        too_small_y_2 = !z_y_2 && exp_y_2 < 128+128-126;
   wire        too_large_y_2 = !z_y_2 && exp_y_2 > 128+128+126;
   reg [8:0] exp_y_2, exp_y_3;
   reg 	     nan_y_3;
   reg [23:0] maint_l_2, maint_r_2;
   always @ (posedge clk) begin
      { nan_y_2, z_y_2, inf_y_2 } <= { nan_y_1, z_y_1, inf_y_1 };
      exp_y_2 <= {1'b0, exp_l_1 } + { 1'b0, exp_r_1 }; // Add exponents as exponent for result
      exp_y_3 <= exp_y_2;
      { maint_l_2, maint_r_2 } <= {  { 1'b1, maint_l_1 }, { 1'b1, maint_r_1}};
      maint_y_3 <= maint_l_2 * maint_r_2;
      s_y_2 <= s_l_1 ^ s_r_1;
      {  z_y_3, s_y_3, too_small_y_3, too_large_y_3, inf_y_3, nan_y_3 } <=  {  z_y_2, s_y_2, too_small_y_2, too_large_y_2, inf_y_2, nan_y_2 };
   end 
   
   wire growth_y_3 = maint_y_3[47];
   wire [8:0] exp_yy_3 =  (growth_y_3) ? {1'b0, exp_y_3} + 9'd1: exp_y_3;
   wire [47:0] maint_yy_3 = (growth_y_3) ? maint_y_3 >> 1: maint_y_3;
    
  wire [7:0] exp_yyy_3 = exp_yy_3 - 8'd127;
  wire [22:0] maint_yyy_3 = maint_yy_3 >> 23; // Bit 23 would be the hidden bit. TODO: Need to round.

  // For multiply of normalised two-bit numbers we have the following cases:
  //   2*2=4, 2*3=6 and 3*3=9.  The maximum amount of renomalisation is one place.
  // Treating all denomals as zero means that no other renormalisation is ever needed.
  wire [31:0] rslt_3 =
    nan_y_3 ? { 1'b0, 8'hff, -23'd1 }: // highest	      
    (too_small_y_3 || z_y_3) ?   { s_y_3, 8'd0,   23'd0 }:  // mid precedence, return 0.
    (too_large_y_3 || inf_y_3) ? { s_y_3, 8'hff, 23'd0 }:  // lowest precedence special result
    { s_y_3, exp_yyy_3, maint_yyy_3[22:0] };
 
   wire       FAIL_flag = 0; // FAIL signalled with NaN for now but could be on too_large.
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck! hmm.
      arg_l_1 <= XX;
      arg_r_1 <= YY;
      { res_4, fl_4 } <= { rslt_3, FAIL_flag };
   end

   assign RR = res_4; // Drive outputs
   assign FAIL = fl_4;

   // This DP code at the end here is for debug printing only.
   wire [63:0] a0_double = {XX[31], XX[30], {3{~XX[30]}}, XX[29:23], XX[22:0], {29{1'b0}}};
   wire [63:0] a1_double = {YY[31], YY[30], {3{~YY[30]}}, YY[29:23], YY[22:0], {29{1'b0}}};
   wire [63:0] r_double = {RR[31], RR[30], {3{~RR[30]}}, RR[29:23], RR[22:0], {29{1'b0}}};
   always@(posedge clk)  if (0) begin
      $display("%1t, %m operate:  %f * %f -> %f", $time,
	       $bitstoreal(a0_double),
	       $bitstoreal(a1_double),
	       $bitstoreal(r_double));
   end
   
endmodule

//=======================================================================================
// fixed-latency, Single-Precision multiply: IEEE 1+8+23 = 32-bit format.
module CV_FP_FL5_MULTIPLIER_SP(
       input clk,
       input reset,
       output [31:0] RR,
       input  [31:0] XX,
       input  [31:0] YY,
       output FAIL
       );


   reg [7:0]  exp_l_1, exp_r_1;
   reg [22:0] maint_l_1, maint_r_1; 
   reg s_l_1, s_r_1;

   reg [7:0] exp_l_2, exp_r_2;
   reg [31:0] arg_l_1, arg_r_1; // Fixed pipeline latency (FL) 4 now.
   reg [31:0] res_4, res_5;
   reg 	      fl_4, fl_5;
   always @(*) begin
       { s_l_1, exp_l_1, maint_l_1 } <= arg_l_1;  // Deconstruct input arg1 - ensure maint is registered for DSP input
      { s_r_1, exp_r_1, maint_r_1 } <= arg_r_1;  // Deconstruct input arg2
   end
   
   // IEEE decode
   wire        z_l_1 = exp_l_1 == 8'd0; // Check left arg is zero or denormal.
   wire        z_r_1 = exp_r_1 == 8'd0; // Check right arg is zero or denormal.
   wire nan_l_1 = exp_l_1 == 8'hff && maint_l_1 != 0;
   wire nan_r_1 = exp_r_1 == 8'hff && maint_r_1 != 0;
   wire inf_l_1 = exp_l_1 == 8'hff && maint_l_1 == 0;
   wire inf_r_1 = exp_r_1 == 8'hff && maint_r_1 == 0;

   // Result logic
   wire nan_y_1 = nan_l_1 || nan_r_1; // Highest precedence
   wire z_y_1 = z_l_1 || z_r_1;       // Zero comes next.
   wire inf_y_1 = inf_l_1 || inf_r_1; // Infinity lowest precedence special result.

   reg 	nan_y_2, z_y_2, inf_y_2, s_y_2;
   reg [47:0] maint_y_3, maint_y_4;
   reg 	      too_small_y_3, too_large_y_3, inf_y_3, z_y_3, s_y_3, nan_y_3;
   reg 	      too_small_y_4, too_large_y_4, inf_y_4, z_y_4, s_y_4, nan_y_4;   
   wire        too_small_y_2 = !z_y_2 && exp_y_2 < 128+128-126;
   wire        too_large_y_2 = !z_y_2 && exp_y_2 > 128+128+126;
   reg [8:0] exp_y_2, exp_y_3, exp_y_4;
   reg [23:0] maint_l_2, maint_r_2;
   always @ (posedge clk) begin
      { nan_y_2, z_y_2, inf_y_2 } <= { nan_y_1, z_y_1, inf_y_1 };
      exp_y_2 <= {1'b0, exp_l_1 } + { 1'b0, exp_r_1 }; // Add exponents as exponent for result
      exp_y_3 <= exp_y_2;
      exp_y_4 <= exp_y_3;      
      { maint_l_2, maint_r_2 } <= {  { 1'b1, maint_l_1 }, { 1'b1, maint_r_1}};
      maint_y_3 <= maint_l_2 * maint_r_2;
      maint_y_4 <= maint_y_3;
      s_y_2 <= s_l_1 ^ s_r_1;
      {  z_y_3, s_y_3, too_small_y_3, too_large_y_3, inf_y_3, nan_y_3 } <=  {  z_y_2, s_y_2, too_small_y_2, too_large_y_2, inf_y_2, nan_y_2 };
      {  z_y_4, s_y_4, too_small_y_4, too_large_y_4, inf_y_4, nan_y_4 } <=  {  z_y_3, s_y_3, too_small_y_3, too_large_y_3, inf_y_3, nan_y_3 };
   end 
   
   wire growth_y_4 = maint_y_4[47];
   wire [8:0] exp_yy_4 =  (growth_y_4) ? {1'b0, exp_y_4} + 9'd1: exp_y_4;
   wire [47:0] maint_yy_4 = (growth_y_4) ? maint_y_4 >> 1: maint_y_4;
    
  wire [7:0] exp_yyy_4 = exp_yy_4 - 8'd127;
  wire [22:0] maint_yyy_4 = maint_yy_4 >> 23; // Bit 23 would be the hidden bit. TODO: Need to round.

  // For multiply of normalised two-bit numbers we have the following cases:
  //   2*2=4, 2*3=6 and 3*3=9.  The maximum amount of renomalisation is one place.
  // Treating all denomals as zero means that no other renormalisation is ever needed.
  wire [31:0] rslt_4 =
    nan_y_4 ? { 1'b0, 8'hff, -23'd1 }: // highest	      
    (too_small_y_4 || z_y_4) ?   { s_y_4, 8'd0,   23'd0 }:  // mid precedence, return 0.
    (too_large_y_4 || inf_y_4) ? { s_y_4, 8'hff, 23'd0 }:  // lowest precedence special result
    { s_y_4, exp_yyy_4, maint_yyy_4[22:0] };
 
   wire       FAIL_flag = 0; // FAIL signalled with NaN for now but could be on too_large.
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck! hmm.
      arg_l_1 <= XX;
      arg_r_1 <= YY;
      { res_5, fl_5 } <= { rslt_4, FAIL_flag };
   end

   assign RR = res_5; // Drive outputs
   assign FAIL = fl_5;

   // This DP code at the end here is for debug printing only.
   wire [63:0] a0_double = {XX[31], XX[30], {3{~XX[30]}}, XX[29:23], XX[22:0], {29{1'b0}}};
   wire [63:0] a1_double = {YY[31], YY[30], {3{~YY[30]}}, YY[29:23], YY[22:0], {29{1'b0}}};
   wire [63:0] r_double = {RR[31], RR[30], {3{~RR[30]}}, RR[29:23], RR[22:0], {29{1'b0}}};
   always@(posedge clk)  if (0) begin
      $display("%1t, %m operate:  %f * %f -> %f", $time,
	       $bitstoreal(a0_double),
	       $bitstoreal(a1_double),
	       $bitstoreal(r_double));
   end
   
endmodule


//=======================================================================================
// Fixed-latency, Double-Precision multiply: IEEE 1+11+52 = 64 bit format.
// Doing this in 3 on Xilinx may not really work.  See the FL6 version.
module CV_FP_FL3_MULTIPLIER_DP(
       input clk,
       input reset,
       output [63:0] RR,
       input  [63:0] XX,
       input  [63:0] YY,
       output FAIL
       );


  wire [10:0] exp1, exp2;
  wire [51:0] mant1,mant2; 
  wire s1, s2;
  reg [63:0] arg1, arg2, res0, res1; // Fixed pipeline latency (FL) 3.
  reg 	     fl0, fl1;
  assign { s1, exp1, mant1 } = arg1;  // Deconstruct input arg1
  assign { s2, exp2, mant2 } = arg2;  // Deconstruct input arg2

  wire z1 = exp1 == 11'd0; // Check arg1 is zero or denormal.
  wire z2 = exp2 == 11'd0; // Check arg2 is zero or denormal.

   wire nan1 = exp1 == 11'h7ff && mant1 != 0;
   wire nan2 = exp2 == 11'h7ff && mant2 != 0;

   wire inf1 = exp1 == 11'h7ff && mant1 == 0;
   wire inf2 = exp2 == 11'h7ff && mant2 == 0;

   wire nanr = nan1 || nan2; // Highest precedence
   wire zr = z1 || z2;       // Zero comes next.
   wire infr = inf1 || inf2; // Infinity lowest precedence special result.


  wire [12:0] exp_r0 = {1'b0, exp1 } + { 1'b0, exp2 }; // Add exponents as exponent for result


  wire [105:0] mant_r0 = {1'b1, mant1 } * {1'b1, mant2};

   
   wire growth = mant_r0[105];
   wire [12:0] exp_r1 =  (growth) ? {1'b0, exp_r0} + 13'd1: exp_r0;
   wire [105:0] mant_r1 = (growth) ? mant_r0 >> 1: mant_r0;



  wire       too_small = !zr && exp_r1 < 2048-1022;
  wire       too_large = !zr && exp_r1 > 2048+1023;

    
  wire [10:0] exp_r2 = exp_r1 - 12'd1023;
  wire [51:0] mant_r2 = mant_r1 >> 52;

  // For multiply of normalised two-bit numbers we have the following cases:
  //   2*2=4, 2*3=6 and 3*3=9.  The maximum amount of renomalisation is one place.
  // Treating all denomals as zero means that no other renormalisation is ever needed.
  wire [63:0] rslt =
    nanr ? { 1'b0, 11'h7ff, -52'd1 }: // highest	      
    (too_small || zr) ?   { s1 ^ s2, 11'd0,   52'd0 }:  // mid precedence, return 0.
    (too_large || infr) ? { s1 ^ s2, 11'h7ff, 52'd0 }:  // lowest precedence special result
    { s1 ^ s2, exp_r2, mant_r2[51:0] };

   wire FAIL_flag = too_large; 
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck!
      arg1 <= XX;
      arg2 <= YY;
      { res0, fl0 } <= { rslt, FAIL_flag };
      { res1, fl1 } <= { res0, fl0 };
//      { res2, fl2 } <= { res1, fl1 };
   end

   assign RR = res1;
   assign FAIL = fl1;

endmodule

//=======================================================================================
// Fully pipelined SP mantissa divider:
// Better to use a CoreGen result or the full SRT algorithm using carry save accumulation of the quotient.
module RAD4_IDIVCELL#(parameter h_offset = 0,
		     parameter d_offset = 0
		     )
   (
	     input 	       clk, 
	     input [23:0]      num_in,
	     output reg [23:0] num_out,
	     input [23:0]      den_in,
	     output reg [23:0]  den_out, 
	     input [23:0]      quot_in,
	     output reg [23:0] quot_out, 
	     input [12:0]      mein,
	     output reg [12:0] mout

    );

   parameter offset = h_offset * 2;
   
   wire [47:0] 		   den_shifted = { 24'd0, den_in } << (offset);


   reg 			   goes0, goes1;
   reg [23:0] 		   quot1, num1;
      always @(posedge clk) begin
	 goes0 = num_in >= (den_shifted << 1);
	 num1 =  num_in - (goes0 ? (den_shifted<<1):0);
	 quot1 = quot_in | (goes0 ? (2<<offset):0);

	 goes1 = num1 >= den_shifted;
	 num_out <=  num1 - ((goes1) ? den_shifted:0);
	 quot_out <= quot1 | (goes1 ? (1<<offset):0);
	 //$display("%m radix4 goes=%d qin=%1d quot=%1d", {goes1, goes0}, quot_in, quot_out);
	 mout <= mein;
	 den_out <= den_in;
      end
   
   
endmodule	    



module RAD4_FDIVCELL#(parameter h_offset = 0,
		     parameter d_offset = 24
		     )
   (
    input 	      clk, 
    input [23:0]      num_in,
    output reg [23:0] num_out,
    input [23:0]      den_in,
    output reg [23:0] den_out, 
    input [25:0]      quot_in,
    output reg [25:0] quot_out, 
    input [12:0]      mein,
    output reg [12:0] mout

    );

   parameter offset = h_offset * 2;
   
   wire [47:0] 		       den_shifted = { 24'd0, den_in } >> (d_offset - offset);
   reg 			       goes0, goes1;
   reg [25:0] 		       quot1;
   reg [23:0] 		       num1;   
   always @(posedge clk) begin
      goes0 = num_in >= (den_shifted << 1);
      num1 =  num_in - (goes0 ? (den_shifted<<1):0);
      quot1 = quot_in | (goes0 ? (1<<offset):0);

      goes1 = num1 >= den_shifted;
      num_out <=  num1 - ((goes1) ? den_shifted:0);
      quot_out <= quot1 | (goes1 ? (1<<(offset-1)):0);
      //$display("%m radix4 goes=%d qin=%1d quot=%1d", {goes1, goes0}, quot_in, quot_out);
      mout <= mein;
      den_out <= den_in;
   end
   
   
endmodule	    


// Fully pipelined divider, 24-bit.
// For integer division, assuming denominator is not zero, the maximum result is the numerator size.
// When operating on normalised mantissas, with msb=1, the resulting range is between just below 2.0 and just above 0.5. So only one bit of extra quotient is needed 
// that may come into view post normalising.
module Divide24F_FL13(
		     input 	   clk, 
		     input [23:0]  num,
		     input [23:0]  den,
		     output [23:0] quotient,
		     input [12:0] meta_in,
		     output [12:0] meta_out
		     );

   
   wire [23:0] 		   d00, d01, d02, d03, d04, d05, d06, d07, d08, d09, d10, d11, d12;
   wire [23:0] 		   n00, n01, n02, n03, n04, n05, n06, n07, n08, n09, n10, n11, n12;   
   wire [25:0] 		   q00, q01, q02, q03, q04, q05, q06, q07, q08, q09, q10, q11, q12;
   wire [12:0] 		   m00, m01, m02, m03, m04, m05, m06, m07, m08, m09, m10, m11, m12;   


   RAD4_FDIVCELL #(12) bix12 (.mein(meta_in), .mout(m12), .clk(clk), .num_in(num), .num_out(n12), .den_in(den), .den_out(d12), .quot_in(26'd0), .quot_out(q12));   
   RAD4_FDIVCELL #(11) bix11 (.mein(m12), .mout(m11), .clk(clk), .num_in(n12), .num_out(n11), .den_in(d12), .den_out(d11), .quot_in(q12), .quot_out(q11));
   RAD4_FDIVCELL #(10) bix10 (.mein(m11), .mout(m10), .clk(clk), .num_in(n11), .num_out(n10), .den_in(d11), .den_out(d10), .quot_in(q11), .quot_out(q10));
   RAD4_FDIVCELL #(09) bix09 (.mein(m10), .mout(m09), .clk(clk), .num_in(n10), .num_out(n09), .den_in(d10), .den_out(d09), .quot_in(q10), .quot_out(q09));
   RAD4_FDIVCELL #(08) bix08 (.mein(m09), .mout(m08), .clk(clk), .num_in(n09), .num_out(n08), .den_in(d09), .den_out(d08), .quot_in(q09), .quot_out(q08));
   RAD4_FDIVCELL #(07) bix07 (.mein(m08), .mout(m07), .clk(clk), .num_in(n08), .num_out(n07), .den_in(d08), .den_out(d07), .quot_in(q08), .quot_out(q07));
   RAD4_FDIVCELL #(06) bix06 (.mein(m07), .mout(m06), .clk(clk), .num_in(n07), .num_out(n06), .den_in(d07), .den_out(d06), .quot_in(q07), .quot_out(q06));
   RAD4_FDIVCELL #(05) bix05 (.mein(m06), .mout(m05), .clk(clk), .num_in(n06), .num_out(n05), .den_in(d06), .den_out(d05), .quot_in(q06), .quot_out(q05));
   RAD4_FDIVCELL #(04) bix04 (.mein(m05), .mout(m04), .clk(clk), .num_in(n05), .num_out(n04), .den_in(d05), .den_out(d04), .quot_in(q05), .quot_out(q04));
   RAD4_FDIVCELL #(03) bix03 (.mein(m04), .mout(m03), .clk(clk), .num_in(n04), .num_out(n03), .den_in(d04), .den_out(d03), .quot_in(q04), .quot_out(q03));
   RAD4_FDIVCELL #(02) bix02 (.mein(m03), .mout(m02), .clk(clk), .num_in(n03), .num_out(n02), .den_in(d03), .den_out(d02), .quot_in(q03), .quot_out(q02));
   RAD4_FDIVCELL #(01) bix01 (.mein(m02), .mout(m01), .clk(clk), .num_in(n02), .num_out(n01), .den_in(d02), .den_out(d01), .quot_in(q02), .quot_out(q01));
   RAD4_FDIVCELL #(00) bix00 (.mein(m01), .mout(m00), .clk(clk), .num_in(n01), .num_out(n00), .den_in(d01), .den_out(d00), .quot_in(q01), .quot_out(q00));
   
   assign quotient = q00;
   assign meta_out = m00;
   // assign remainder = d00;
endmodule

//=======================================================================================
// Fixed-latency, Single-Precision divide: IEEE 1+8+23 = 32 bit format.
// Not realistic for synthesis.
module CV_FP_FL15_DIVIDER_SP(
       input clk,
       input reset,
       output [31:0] RR,
       input  [31:0] NN, // numerator
       input  [31:0] DD, // denominator
       output FAIL       // Report NaN instead of using this FAIL output.
       );

   parameter traceme = 0;
   wire [7:0] exp1, exp2;
   wire [22:0] mant1, mant2; 
   wire        s1, s2;
   reg [31:0]  arg1, arg2;  
   assign { s1, exp1, mant1 } = arg1;  // Deconstruct input arg1
   assign { s2, exp2, mant2 } = arg2;  // Deconstruct input arg2
   
   
   wire        z1 = exp1 == 8'd0; // Check num is zero or denormal.
   wire        z2 = exp2 == 8'd0; // Check den is zero or denormal.
   
   wire        nan1 = exp1 == 8'hff && mant1 != 0;
   wire        nan2 = exp2 == 8'hff && mant2 != 0;
   
   wire        inf1 = exp1 == 8'hff && mant1 == 0;
   wire        inf2 = exp2 == 8'hff && mant2 == 0;
   
   wire        nan_r0 = nan1 || nan2; // Highest precedence
   wire        z_r0 = z1;             // Zero comes next.
   wire        inf_r0 = inf1 || z2;   // Infinity lowest precedence special result.
   wire        s_r0 = s1 ^ s2;

   // Is S/P 1.0 is 0x3f80_0000 which has the identity exponent under multiplication and division.  Its value if 0x7F=127.  So this much must be added on after exponent subtraction (ignoring justification), but we add on twice this to more easily spot exponent overflows.

   wire [8:0] exp_r0 = {1'b0, exp1 } + 9'd127 + 9'd127 - { 1'b0, exp2 }; // Subtract exponents as exponent for result, and add on 128 to correct.


   // Considering 3 bit numbers in the range 4 to 7, the quotients possible are
   //   7/7 = 4/4 = 1
   //   7/4 = 1...
   //   4/7 = 0...
   reg [5:0]   justify2;
   wire [23:0] mant_r00;
   reg [23:0]  mant_r0;   
   reg [5:0]   bounder;
   wire [12:0] mein = { s_r0, nan_r0, z_r0, inf_r0, exp_r0 };
   wire [12:0] mout;
   Divide24F_FL13 the_mantissa_divider(
				      .clk(clk),
				      .num({1'b1, mant1 }),
				      .den({1'b1, mant2 }),
				      .quotient(mant_r00),
				      .meta_in(mein),
				      .meta_out(mout)
				      );

   always @(mant_r00) begin
      mant_r0 = mant_r00;
      //$display("div24 ans pre-norm nn=%h dd=%h pp=%h r=%h", nn, dd, pp, mant_r0);
      // Renormalise the result.
      justify2 = 0;
      bounder = 24;      
      while (justify2 < 63 && !mant_r0[23] && bounder != 0) begin
	 justify2 = justify2 + 1;
	 mant_r0 = mant_r0 << 1;
	 bounder = bounder-1;	 
	 end
      //if (traceme) $display("%m: midans  nn=%h dd=%h justify2=%1d r=%h", arg1, arg2, justify2, RR);
   end
   
//   assign { nan_r1, z_r1, inf_r1, exp_r1 } = mout;

   wire [8:0] exp_r1 = mout[8:0];
   wire inf_r1 =       mout[9];   
   wire z_r1 =         mout[10];   
   wire nan_r1 =       mout[11];
   wire s_r1 =         mout[12];   
   wire [23:0]         mant_r1 = mant_r0;   
   
   wire [8:0] exp_r1a = exp_r1 - justify2; // Apply justification
   //    
   wire too_small = !z_r1 && exp_r1a < 127;
   wire too_large = !z_r1 && exp_r1a > 127+254;

   
   wire [7:0] exp_r2 = exp_r1a - 127; // Remove excess offset.
   wire [22:0] mant_r2 = mant_r1[22:0];  //  Delete hidden bit.
   reg [31:0] rslt;
   always @(posedge clk) begin 
      rslt <=
	     nan_r1 ? { 1'b0, 8'hff, -23'd1 }:                // NaN highest	      
	     (too_small || z_r1) ?   { s1 ^ s2, 8'd0,   23'd0 }: // Zero mid
	     (too_large || inf_r1) ? { s1 ^ s2, 8'hff, 23'd0 }:  // Inf: lowest precedence special result
	     { s_r1, exp_r2, mant_r2[22:0] };
   end
   

   wire FAIL_flag = too_large; 
   // Generate r_double just for print/debug
   wire [63:0] r_double = {RR[31], RR[30], {3{~RR[30]}}, RR[29:23], RR[22:0], {29{1'b0}}};
   
   always @(posedge clk) begin 
      if (traceme)  $display("%m: inf1=%1h  exp_r1=%1d-%1d=%1d  exp_r2=%1d too=(%1h,%1h) nn=%1h dd=%1h r=%1h", inf1,
			     exp1, exp2, exp_r1, exp_r2, too_small, too_large, arg1, arg2, RR);
      //if (traceme) $display("%m: midans  
      //if (traceme)  $display("%m:  nan1=%h nan2=%h        z1=%h z2=%h           inf1=%h inf2=%h", nan1, nan2, z1, z2, inf1, inf2);  
      arg1 <= NN;
      arg2 <= DD;
      if (traceme) $display("%1t, %m: result=%1e", $time,  $bitstoreal(r_double));
   end

   assign RR = rslt;
   assign FAIL = FAIL_flag;

endmodule // divider s/p FL15

//=======================================================================================
// fixed-latency, Double-Precision divide: IEEE 1+11+52 = 64 bit format.
// Too slow to really use under synthesis. !
module CV_FP_FL5_DIVIDER_DP(
       input clk,
       input reset,
       output [63:0] RR,
       input  [63:0] NN, // numerator
       input  [63:0] DD, // denominator
       output FAIL
       );

   
  wire [10:0] exp1, exp2;
  wire [51:0] mant1,mant2; 
  wire s1, s2;
  reg [63:0] arg1, arg2, res0, res1, res2, res3; // Fixed pipeline latency 5 (FL5).
  reg 	     fl0, fl1, fl2, fl3;
  assign { s1, exp1, mant1 } = arg1;  // Deconstruct input arg1
  assign { s2, exp2, mant2 } = arg2;  // Deconstruct input arg2
   
  wire z1 = exp1 == 11'd0; // Check arg1 is zero or denormal.
  wire z2 = exp2 == 11'd0; // Check arg2 is zero or denormal.

   wire nan1 = exp1 == 11'h7ff && mant1 != 0;
   wire nan2 = exp2 == 11'h7ff && mant2 != 0;

   wire inf1 = exp1 == 11'h7ff && mant1 == 0;
   wire inf2 = exp2 == 11'h7ff && mant2 == 0;

   wire nanr = nan1 || nan2; // Highest precedence
   wire zr = z1;             // Zero comes next.
   wire infr = inf1 || z2;   // Infinity lowest precedence special result.


   wire [12:0] exp_r0 = {1'b0, exp1 } + 13'd2048 - { 1'b0, exp2 }; // Subtract exponents as exponent for result


   // Considering 3 bit numbers in the range 4 to 7, the quotients possible are
   //   7/7 = 4/4 = 1
   //   7/4 = 1...
   //   4/7 = 0...

   // Integer long division steps the P register left a variable amount during denominator preshifting, but for floating point we always
   // start the main phase with P in the msb and adjust the final exponent by the preshift amount.
   reg [5:0]   justify2;
   reg [52:0]  mant_r0, pp, nn, dd;
   reg [5:0] bounder;
   always @(mant1 or mant2) begin
      bounder = 63;
      pp = 1 << 52;
      nn = {1'b1, mant1 };
      dd = {1'b1, mant2 };
      mant_r0 = 0;
      //justify = 0;
      //while (!dd[52] && dd != 0) begin // preshift - never used except for denormal denominators that we ignore.
	 //justify = justify + 1;
	 //dd = dd << 1;
      //end

      //$display("div53 preshifted  nn=%h dd=%h pp=%h r=%h", nn, dd, pp, mant_r0);
      // This will be too slow in logic terms to use....
      while (nn != 0 && dd != 0 && bounder > 0) begin
	 //$display("div53 step nn=%h dd=%h pp=%h r=%h", nn, dd, pp, mant_r0);
	 if (nn >= dd) begin
	    mant_r0 = mant_r0 | pp;
	    nn = nn - dd;
	 end
	 dd = dd >> 1;
	 pp = pp >> 1;
         bounder = bounder - 1;
      end
      //$display("div53 ans pre-norm nn=%h dd=%h pp=%h r=%h", nn, dd, pp, mant_r0);

      // Renormalise the result.
      justify2 = 0;
      while (justify2 < 63 && !mant_r0[52]) begin
	 justify2 = justify2 + 1;
	 mant_r0 = mant_r0 << 1;
	 end
      //$display("div53 ans  nn=%h dd=%h pp=%h justify2=%1d r=%h", nn, dd, pp, justify2, mant_r0);
   end
   
   wire [11:0] exp_r1 =  exp_r0; //+justify;


   wire       too_small = !zr && exp_r1 < 2048-1022;
   wire       too_large = !zr && exp_r1 > 2048+1023;
    
   wire [10:0] exp_r2 = exp_r1 - 2048 + 1023 - justify2; // Reinsert excess offset.
   wire [51:0] mant_r2 = mant_r0[51:0];  //  Delete hidden bit.


   wire [63:0] rslt =
    nanr ?                { 1'b0, 11'h7ff, -52'd1 }: // highest	      
    (too_small || zr) ?   { s1 ^ s2, 11'd0,   52'd0 }:  // mid
    (too_large || infr) ? { s1 ^ s2, 11'h7ff, 52'd0 }:  // lowest precedence special result
                          { s1 ^ s2, exp_r2, mant_r2 };

   wire FAIL_flag = too_large; 
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck!
      //$display("divider:  exp_r1=%d-%d=%d  exp_r2=%d too_small=%h  too_large=%h", exp1, exp2, exp_r1, exp_r2, too_small, too_large);
      //$display("divider:  nan1=%h nan2=%h        z1=%h z2=%h           inf1=%h inf2=%h", nan1, nan2, z1, z2, inf1, inf2);  
      arg1 <= NN;
      arg2 <= DD;
      { res0, fl0 } <= { rslt, FAIL_flag };
      { res1, fl1 } <= { res0, fl0 };
      { res2, fl2 } <= { res1, fl1 };
      { res3, fl3 } <= { res2, fl2 };
      //$display("%1t, %m: result=%1e", $time,  $bitstoreal(res3));
   end

   assign RR = res3;
   assign FAIL = fl3;

endmodule // divider

//=======================================================================================
// fixed-latency, Double-Precision adder: IEEE 1+11+52 = 64 bit format.

module CV_FP_FL4_ADDER_DP(
       input clk,
       input reset,
       output [63:0] RR,
       input  [63:0] XX,
       input  [63:0] YY, // denominator
       output FAIL
       );
   CV_FP_FL4_ADDSUB_DP addcore(clk, reset, RR, XX, YY, FAIL, 1'b0);
endmodule

module CV_FP_FL4_SUBTRACTOR_DP(
       input clk,
       input reset,
       output [63:0] RR,
       input  [63:0] XX, 
       input  [63:0] YY, 
       output FAIL
       );
   CV_FP_FL4_ADDSUB_DP subcore(clk, reset, RR, XX, YY, FAIL, 1'b1);
endmodule


//
// When exponents differ by two or more, the maximum amount of renormalisation is 1 place.
//
module CV_FP_FL4_ADDSUB_DP(
       input 	     clk,
       input 	     reset,
       output [63:0] RR,
       input [63:0]  XX, 
       input [63:0]  YY, 
       output 	     FAIL,
       input 	     subtract  
       );

  wire [10:0] exp1, exp2;
  wire [51:0] mant1,mant2; 
  wire s1, s2, s20;
  reg [63:0] arg1, arg2, res0, res1, res2; // Fixed pipeline latency (FL) 4.
  reg 	     fl0, fl1, fl2;
  assign { s1, exp1, mant1 } = arg1;  // Deconstruct input arg1
  assign { s20, exp2, mant2 } = arg2;  // Deconstruct input arg2


  assign s2 = s1 ^ s20 ^ subtract; //Flip sign of second arg for subtraction.  Treat s1 as positive always and by flipping s2 here if negative and dittio in the final result.
   
  wire z1 = exp1 == 11'd0; // Check arg1 is zero or denormal.
  wire z2 = exp2 == 11'd0; // Check arg2 is zero or denormal.

   wire nan1 = exp1 == 11'h7ff && mant1 != 0;
   wire nan2 = exp2 == 11'h7ff && mant2 != 0;

   wire inf1 = exp1 == 11'h7ff && mant1 == 0;
   wire inf2 = exp2 == 11'h7ff && mant2 == 0;

   wire nanr = nan1 || nan2; // Highest precedence
   wire infr = inf1 || inf2;   // Infinity lowest precedence special result.
   // todo inf + -inf is zero?

   reg [11:0] justify1, justify2;
   reg        smaller;
   // IEEE has an 11 bit exponent held excess 1023 with range 1 to 2046 which encodes -1022 to +1023.

   always @(*) begin
      if (exp1 > exp2) begin // 
	 smaller = 1;
	 justify1 = exp1 - exp2;
      end
      else begin
	 smaller = 0;
	 justify1 = exp2 - exp1;
      end
      //$display("adder work smaller smaller=%h justify1=%d", smaller, justify1);
   end

   wire [53:0] lhs = (smaller) ?  (z1 ? 0: { 2'b01, mant1 })   :  (z1 ? 0: ({ 2'b01, mant1 } >> justify1));
   wire [53:0] rhs = (smaller) ?  (z2 ? 0 : ({ 2'b01, mant2 } >> justify1)) : (z2 ? 0 : { 2'b01, mant2 });

   wire [53:0] sum  =  lhs + rhs; // Can gain one bit in add or subtract, so 54-bit intermediate result.
   wire [53:0] diff =  lhs - rhs; 
   wire zr = (!s2) ? (sum==54'b0): (diff==54'b0);
   wire [53:0] mant_r0 = (!s2) ? sum: (diff [53]) ? rhs-lhs: diff;
   wire        flip    = s2 && diff[53]; // Flip indicates a negative result (modulo lhs being positive).
   
   wire [11:0] exp_r1 =  z1 ? exp2: z2 ? exp1: smaller ? exp1 : exp2;

    
   // Renormalise the result
   reg [53:0]  mant_r1;
   reg 	       rjustify2;
   reg [5:0] bounder;
   always @(mant_r0) begin
      justify2 = 0;
      rjustify2 = 0;
      bounder = 63;
      mant_r1 = mant_r0;
      if (mant_r1[53]) begin
	 mant_r1 = mant_r1 >> 1;
	 rjustify2 = 1;
      end
      
      if (mant_r1) begin
	 while (!mant_r1 [52] && mant_r1 && bounder > 0) begin
	    justify2 = justify2 + 1;
	    mant_r1 = mant_r1 << 1;
            bounder = bounder - 1;
	 end
      end
      //$display("adder post just: r1=%h   r2=%h  rjustify2=%h justify2=%d", mant_r1, mant_r1[51:0], rjustify2, justify2);
   end
   wire [51:0] mant_r2 = mant_r1[51:0]; // remove hidden bit and carry bit.
   wire [12:0] exp_r22 = exp_r1 - justify2 + 1024 + rjustify2;
      
   wire       too_small = !zr && exp_r22 < 1023+1024-1022;
   wire       too_large = !zr && exp_r22 > 1023+1024+1023;

   wire [10:0] exp_r2 = exp_r22 - 1024;
   wire [63:0] rslt =
    nanr ?                { 1'b0, 11'h7ff, -52'd1 }: // highest	      
    (too_small || zr) ?   { s1 ^ flip, 11'd0,   52'd0 }:  // mid
    (too_large || infr) ? { s1 ^ flip, 11'h7ff, 52'd0 }:  // lowest precedence special result
                          { s1 ^ flip, exp_r2, mant_r2 };

   wire FAIL_flag = too_large; 
   always @(posedge clk) begin // ASIC tools will backprop the logic with luck!
      //$display("adder:  arg1=%h  arg2=%h s1=%h flip=%h rslt=%h", arg1, arg2, s1, flip, rslt);
      //$display("adder:  lhs=%h  rhs=%h  sum=%h diff=%h mant_r0=%h", lhs, rhs, sum, diff, mant_r0);      
      //$display("adder:  exp_r1=%d  exp_r22=%d exp_r2=%d smaller=%d too_small=%h  too_large=%h", exp_r1, exp_r22, exp_r2, smaller, too_small, too_large);
      //$display("adder:  nan1=%h nan2=%h        z1=%h z2=%h           inf1=%h inf2=%h", nan1, nan2, z1, z2, inf1, inf2);  
      arg1 <= XX;
      arg2 <= YY;
      { res0, fl0 } <= { rslt, FAIL_flag };
      { res1, fl1 } <= { res0, fl0 };
      { res2, fl2 } <= { res1, fl1 };
//      { res3, fl3 } <= { res2, fl2 };
   end

   assign RR = res2;
   assign FAIL = fl2;

endmodule // DP adder

//=======================================================================================
// fixed-latency, Single-Precision adder: IEEE 1+8+23 = 32 bit format.

module CV_FP_FL4_ADDER_SP(
       input clk,
       input reset,
       output [31:0] RR,
       input  [31:0] XX, // numerator
       input  [31:0] YY, // denominator
       output FAIL
       );
   CV_FP_FL4_ADDSUB_SP addcore(clk, reset, RR, XX, YY, FAIL, 1'b0);
endmodule

module CV_FP_FL4_SUBTRACTOR_SP(
       input clk,
       input reset,
       output [31:0] RR,
       input  [31:0] XX, // numerator
       input  [31:0] YY, // denominator
       output FAIL
       );
   CV_FP_FL4_ADDSUB_SP subcore(clk, reset, RR, XX, YY, FAIL, 1'b1);
endmodule


module CV_FP_FL4_ADDSUB_SP(
       input 	     clk,
       input 	     reset,
       output [31:0] RR,
       input [31:0]  XX, // numerator
       input [31:0]  YY, // denominator
       output 	     FAIL,
       input 	     subtract  
       );

   // Fixed-latency, Single-Precision adder: IEEE 1+8+23 = 32 bit format.
   parameter traceme = 0;
   wire [7:0] 	     exp1, exp2;
   wire [22:0] 	     mant1, mant2;  // Mantissa without hidden bit: (23 visible + 1 hidden) = 24 bit mantissa arithemtic.
   wire 	     s1, s2, s20;
   reg [31:0] 	     arg1, arg2, res0, res1, res2, res3; // Fixed pipeline latency 4 or 5 (FL4, FL5).
   reg 		     fl0, fl1, fl2, fl3;
   assign { s1, exp1, mant1 } = arg1;   // Deconstruct input arg1
   assign { s20, exp2, mant2 } = arg2;  // Deconstruct input arg2


  assign s2 = s1 ^ s20 ^ subtract; //Flip sign of second arg for subtraction.  Treat s1 as positive always and by flipping s2 here if negative and ditto in the final result.
   
  wire z1 = exp1 == 11'd0; // Check arg1 is zero or denormal.
  wire z2 = exp2 == 11'd0; // Check arg2 is zero or denormal.

   wire nan1 = exp1 == 8'hff && mant1 != 0;
   wire nan2 = exp2 == 8'hff && mant2 != 0;

   wire inf1 = exp1 == 8'hff && mant1 == 0;
   wire inf2 = exp2 == 8'hff && mant2 == 0;
   wire nanr = nan1 || nan2; // Highest precedence
   wire infr = inf1 || inf2;   // Infinity lowest precedence special result.
   // todo inf + -inf is zero?

   reg [7:0] justify1, justify2;
   reg        smaller;

   always @(*) begin
      if (exp1 > exp2) begin // 
	 smaller = 1;
	 justify1 = exp1 - exp2;
      end
      else begin
	 smaller = 0;
	 justify1 = exp2 - exp1;
      end
      //$display("adder work smaller smaller=%h justify1=%d", smaller, justify1);
   end

   wire [24:0] lhs = (smaller) ?  (z1 ? 0: { 2'b01, mant1 })   :  (z1 ? 0: ({ 2'b01, mant1 } >> justify1));
   wire [24:0] rhs = (smaller) ?  (z2 ? 0 : ({ 2'b01, mant2 } >> justify1)) : (z2 ? 0 : { 2'b01, mant2 });

   wire [24:0] sum  =  lhs + rhs; // Can gain one bit in add or subtract, so 25-bit intermediate result.
   wire [24:0] diff =  lhs - rhs; 
   wire zr = (!s2) ? (sum==25'b0): (diff==25'b0);
   wire [24:0] mant_r0 = (!s2) ? sum: (diff [24]) ? rhs-lhs: diff;
   wire        flip    = s2 && diff[24]; // Flip indicates a negative result (modulo lhs being positive).
   
   wire [7:0] exp_r1 =  z1 ? exp2: z2 ? exp1: smaller ? exp1 : exp2;

   reg [5:0] bounder;    
   // Renormalise the result.
   reg [24:0]  mant_r1;
   reg 	       rjustify2;
   always @(mant_r0) begin
      justify2 = 0;
      rjustify2 = 0;
      mant_r1 = mant_r0;
      if (mant_r1[24]) begin
	 mant_r1 = mant_r1 >> 1;
	 rjustify2 = 1;
      end
      
      if (mant_r1) begin
	 bounder = 24;
	 while (!mant_r1 [23] && mant_r1 && bounder != 0) begin
	    justify2 = justify2 + 1;
	    mant_r1 = mant_r1 << 1;
	    bounder = bounder - 1;
	 end
      end
      if (traceme) $display("%m: post just: r1=%h   rjustify2=%d", mant_r1, rjustify2);
   end
   wire [22:0]  mant_r2 = mant_r1[22:0]; // remove hidden bit and carry bit.
   wire [9:0] exp_r22 = exp_r1 - justify2 + 128 + rjustify2;

   // IEEE SP has an 8-bit exponent held excess 128 with range 1 to 254
   wire        too_small = !zr && exp_r22 < 128+128-126;
   wire        too_large = !zr && exp_r22 > 128+128+126;

   wire [7:0] exp_r2 = exp_r22 - 128;
   wire [31:0] rslt =
    nanr ?                { 1'b0, 8'hff, -23'd1 }:       // highest	      
    (too_small || zr) ?   { s1 ^ flip, 8'd0,   23'd0 }:  // mid
    (too_large || infr) ? { s1 ^ flip, 8'hff, 23'd0 }:   // lowest precedence special result
                          { s1 ^ flip, exp_r2, mant_r2 };

   wire FAIL_flag = too_large; 
   // Generate r_double just for print/debug
   wire [63:0] r_double = {rslt[31], rslt[30], {3{~rslt[30]}}, rslt[29:23], rslt[22:0], {29{1'b0}}};

   always @(posedge clk) begin // ASIC tools will backprop the pipeline registers with luck!
      if (rslt && traceme > 0) $display("%1t: %m: result %h %f", $time, rslt, $bitstoreal(r_double));
      if (traceme > 1) begin
	 $display("%m:  arg1=%h  arg2=%h s1=%h flip=%h rslt=%h", arg1, arg2, s1, flip, rslt);
	 $display("%m:  lhs=%h  rhs=%h  sum=%h diff=%h mant_r0=%h", lhs, rhs, sum, diff, mant_r0);      
	 $display("%m:  exp_r1=%d  exp_r22=%d exp_r2=%d smaller=%d too_small=%h  too_large=%h", exp_r1, exp_r22, exp_r2, smaller, too_small, too_large);
	 $display("%m:  nan1=%h nan2=%h        z1=%h z2=%h           inf1=%h inf2=%h", nan1, nan2, z1, z2, inf1, inf2);  
	 end
      arg1 <= XX;
      arg2 <= YY;
      { res0, fl0 } <= { rslt, FAIL_flag };
      { res1, fl1 } <= { res0, fl0 };
      { res2, fl2 } <= { res1, fl1 };
//      { res3, fl3 } <= { res2, fl2 };
   end

   assign RR = res2;
   assign FAIL = fl2;

endmodule // SP adder

//==================================================================================================
// Format convertors:
//
//                   
// The floating-point to/from integer convertors required normally are:

// 1 CV_FP_CVT_FL2_F32_I32  // Integer 32 to float 32 with fixed-latency of 2
// 2 CV_FP_CVT_FL2_F32_I64  // Integer 64 to float 32 with fixed-latency of 2
// 3 CV_FP_CVT_FL2_F64_I32  // Integer 32 to float 64 with fixed-latency of 2
// 4 CV_FP_CVT_FL2_F64_I64  // Integer 64 to float 64 with fixed-latency of 2

// 5 CV_FP_CVT_FL2_I32_F32  // Integer 32 from float 32 with fixed-latency of 2
// 6 CV_FP_CVT_FL2_I32_F64  // Integer 32 from float 64 with fixed-latency of 2
// 7 CV_FP_CVT_FL2_I64_F32  // Integer 64 from float 32 with fixed-latency of 2
// 8 CV_FP_CVT_FL2_I64_F64  // Integer 64 from float 64 with fixed-latency of 2

// 9 CV_FP_CVT_FL0_F32_F64  // Float 32 from float 64 (FL=0 implies combinational) 
// 10 CV_FP_CVT_FL0_F64_F32  // Float 64 from float 32 (FL=0 implies combinational)          


// Convertors currently in this file: 1-4, 5
// Convertors 9 and 10 are implemented as RTL functions hpr_flt2dbl and hpr_dbl2flt


//
// Fixed-latency integer to single-precision convertor. 
// Convertor 1 
module CV_FP_CVT_FL2_F32_I32(input clk, input reset, output reg [31:0] result, input [31:0] arg, output FAIL);

  reg [30:0] absv;
  reg scase_zero1, scase_minint1, signf1;

  assign FAIL = 0; // All i32 values are renderable
   reg [7:0] expo;
   reg [30:0] mantissa;
   reg [6:0]  bounder;
   
  always @(posedge clk) begin
     scase_zero1 <= (arg == 32'h0);
     scase_minint1 <= (arg == 32'h8000_0000);
     absv <= (arg[31]) ? ((32'h0 - arg) & 32'h7FFF_FFFF) : arg[30:0]; 
     signf1 <= arg[31];
     expo = 128+22;
     mantissa = absv;
     bounder = 25;
     if (mantissa != 0) begin
	if (mantissa == (1<<23) && bounder > 0) begin
	   // nothing to do.
        end
	else if (mantissa < (1<<23)) begin
	   while(mantissa < (1<<23) && bounder > 0) begin
	      mantissa = mantissa << 1;
	      expo = expo - 1;
	      bounder = bounder - 1;
	   end
	end
	else begin
	   while(mantissa >= (1<<24) && bounder > 0) begin
	      mantissa = mantissa >> 1;
	      expo = expo + 1;
	      bounder = bounder - 1;
	   end
	end
     end 
     if (scase_zero1) result <= 32'b0;
     else if (scase_minint1) result <= 32'hCF00_0000;
     else begin  
	result[22:0]  <= mantissa[22:0];
	result[30:23] <= expo;
	result[31]    <= signf1;
     end
  end // always @ (posedge clk)
   
   wire [63:0] r_double = {result[31], result[30], {3{~result[30]}}, result[29:23], result[22:0], {29{1'b0}}};
   
   always@(posedge clk)  if (0) begin
      $display("%m operate:  %1d -> 0x%1h 0x%1h %e", 
	       arg, result, r_double, $bitstoreal(r_double));
   end

endmodule

//
// Fixed-latency int64 to single-precision convertor. 
// Convertor 2
module CV_FP_CVT_FL2_F32_I64(input clk, input reset, output reg [31:0] result, input [63:0] arg, output FAIL);

  reg [62:0] absv;
  reg scase_zero1, scase_minint1, signf1;

   assign FAIL = 1'b0; // All int64 values are renderable.

   reg [7:0] expo;  // Not all exponent range is possible: only 128 to 128+64 might be used.
   reg [62:0] mantissa;
   reg [6:0]  bounder;
   always @(posedge clk) begin
      scase_zero1 <= (arg == 64'h0);
      scase_minint1 <= (arg == 64'h8000_0000_0000_0000);
      absv <= (arg[63]) ? ((64'h0 - arg) & 64'h7FFF_FFFF_FFFF_FFFF) : arg[62:0]; 
      signf1 <= arg[63];
      
      expo = 128+22;
      mantissa = absv;
      bounder = 64;
      if (mantissa != 0) begin
	 if (mantissa == (1<<23)) begin
	    // nothing to do.
         end
	 else if (mantissa < (1<<23)) begin
	    while(mantissa < (1<<23) && bounder > 0) begin
	       mantissa = mantissa << 1;
	       expo = expo - 1;
	       bounder = bounder - 1;
	       //$display("%m: l         mant=%8h  e=%1d\n", mantissa, expo);
	    end
	 end
	 else begin
	    while(mantissa >= (1<<24) && bounder > 0) begin
	       mantissa = mantissa >> 1;
	       expo = expo + 1;
	       bounder = bounder - 1;
	       //$display("%m: r         mant=%8h  e=%1d\n", mantissa, expo);
	    end
	 end
      end
      if (scase_zero1) result <= 32'b0;
      else if (scase_minint1) result <= 32'hCF00_0000;
      else begin  
	 result[22:0]  <= mantissa;
	 result[30:23] <= expo;
	 result[31]    <= signf1;
      end
   end
endmodule


// Convertor 3 (improved/direct implementation)
// Convert int32 to double-precision multiply: IEEE 1+11+52 = 64 bit format. 
module CV_FP_CVT_FL2_F64_I32(input clk, input reset, output reg [63:0] result, input [31:0] arg, output FAIL);
  reg [31:0] absv;
  reg scase_zero1, scase_minint1, signf1;

   assign FAIL = 0; // All input values are supported, so this cannot FAIL.
   
   reg [10:0] expo;
   reg [52:0] mantissa;
   reg [5:0] bounder;
   always @(posedge clk) begin
	scase_zero1 <= (arg == 32'h0);
	signf1 <= arg[31] ;
	absv <= (arg[31]) ? (32'h0 - arg) : arg; 
	bounder = 63;
	expo = 1024+32;
      mantissa = absv << (52-33);
      if (mantissa != 0) begin
	 while(mantissa < (1<<52) && bounder > 0) begin
	    //$display("%m:           a=%16h  e=%1d\n", mantissa, expo);
	    mantissa = mantissa << 1;
	   expo = expo - 1;
	   bounder = bounder - 1;
	 end
      end
      
      //$display("%m: zero1=%1d signf1=%1d expo1=%1d mant1=0x%1h", scase_zero1, signf1, expo, mantissa);
      if (scase_zero1) result <= 64'b0;
      else begin  
	 result[51:0]  <= mantissa [51:0];
	 result[62:52] <= expo;
	 result[63]    <= signf1;
      end
   end
endmodule

// Convertor 4
module CV_FP_CVT_FL2_F64_I64(input clk, input reset, output reg [63:0] result, input [63:0] arg, output FAIL);
// Convert int64 to double-precision multiply: IEEE 1+11+52 = 64 bit format. 
  reg [62:0] absv;
  reg scase_zero1, scase_minint1, signf1;

   assign FAIL = 0; // All input values are supported, so this cannot FAIL.
   
  reg [10:0] expo;
  reg [62:0] mantissa;
  reg [6:0] bounder;
  always @(posedge clk) begin
     scase_zero1 <= (arg == 64'h0);
     scase_minint1 <= (arg == 64'h8000_0000_0000_0000);
     absv <= (arg[63]) ? ((64'h0 - arg) & 64'h7FFF_FFFF_FFFF_FFFF) : arg[62:0]; 
     signf1 <= arg[63];
     expo = 1024+51;
     mantissa = absv;
     bounder = 64;
     if (mantissa != 0) begin
	while(mantissa < (1<<52) && bounder > 0) begin
	   //$display("%m: l         mant=%16h  e=%1d\n", mantissa, expo);
	   mantissa = mantissa << 1;
	   expo = expo - 1;
	   bounder = bounder - 1;
	end
	while(mantissa >= (1<<53) && bounder > 0) begin
	   //$display("%m:  r         mant=%16h  e=%1d\n", mantissa, expo);
	   mantissa = mantissa >> 1;
	   expo = expo + 1;
	   bounder = bounder - 1;
	end
     end

 
    //$display("%m: zero1=%1d minint1=%1d  signf1=%1d expo1=%1d mant1=0x%1h", scase_zero1, scase_minint1, signf1, expo, mantissa);
    if (scase_zero1) result <= 64'b0;
    else if (scase_minint1) result <= 64'hc3e0_0000_0000_0000;
    else begin  
       result[51:0]  <= mantissa [51:0];
       result[62:52] <= expo;
       result[63]    <= signf1;
    end
  end
   
endmodule




// Maximum single-precision is 3.4e38 but maximum int32 is 0x4f000000  2.147484e+09 which has exponent 128+30, so any exponents above 30 will overflow.

//==================================================================================================
// Convertor 5 - get int32 from a single-precision float.

module CV_FP_CVT_FL2_I32_F32(input clk, input reset, output [31:0] result, input [31:0] arg, output FAIL);

   parameter traceme = 0;
   wire [7:0]  exp0;
   wire [22:0] mant0;
   wire        signf0;
   assign { signf0, exp0, mant0 } = arg; // Deconstruct input arg1

   reg [31:0]  rr1, rr2;
   reg [7:0]   exp1, expx, bounder;
   reg [22:0]  mant1;
   reg         er2a, er2b;
   reg         signf1;
   reg         scase_zero1;
   reg         scase_nan1;
   reg         scase_inf1;
   wire        scase_zero0 = (mant0 == 0 && exp0 == 0); // special case zero.

   always @(posedge clk) begin
      scase_zero1 <= scase_zero0;
      scase_nan1  <= (mant0 != 0) && (exp0 == 8'hff);
      scase_inf1  <= (mant0 == 0) && (exp0 == 8'hff);
      signf1 <= signf0;
      mant1 <= mant0;
      exp1 <= exp0;
      er2a <= scase_nan1 || scase_inf1;
      er2b = 0;

      if (traceme) $display("%1t: %m arg0=%1h signf0=%1d  exp0=%1d  mant0=0x%h", $time, arg, signf0, exp0, mant0);
      if (traceme) $display("%1t: %m  signf1=%1d  exp1=%1d  mant1=0x%h", $time, signf1, exp1, mant1);      
      if (traceme) $display("%1t: %m: zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1d result=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, result);
      if (scase_zero1) begin
	 if (traceme) $display("%1t: %m:  zero", $time); 
	 rr2 <= 64'd0;
	 end
      else if (exp1 < 127) begin
	 if (traceme) $display("%1t: %m:  underflow to zero, exp1=%1d", $time, exp1); 
	 rr2 <= 64'd0;
	 end
      else if (exp1 < 127+23) begin
	 bounder = 23;
	 expx = exp1;
	 rr1 = mant1 | (1<<23);
	 while(expx < 127+23 && bounder > 0) begin
	    rr1 = rr1 >> 1;
	    expx = expx + 1;
	    bounder = bounder - 1;
	    //if (traceme) $display("%1t: %m:    bbbbbbbbbbbb   zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1d result=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, result);
	 end
	 rr2 <= (signf1) ? 32'd0-rr1: rr1;
      end
      else if (exp1 > 128+30) begin
	 er2b = 1;
	 rr2 <= 0;
	 if (traceme) $display("%1t: %m:  overflow, exp1=%1d\n", $time, exp1); 
      end
      else begin
	 expx = exp1;
	 rr1 = mant1 | (1<<23);
	 bounder = 8;
	 while(expx > 127+23 && bounder > 0) begin
	    rr1 = rr1 << 1;
	    expx = expx - 1;
	    bounder = bounder - 1;
	    //if (traceme) $display("%1t: %m:  aaaaaaaaaaaaa     zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1d result=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, result);
	 end
	 rr2 <= (signf1) ? 32'd0-rr1: rr1;
      end // else: !if(expx > 128+30)
      
   end // always @ (posedge clk)
   
   assign result = rr2;
   assign FAIL = er2a || er2b;
endmodule // CV_FP_CVT_FL2_I32_F32

//==================================================================================================
// Convertor no 6 - get 32-bit int from double-precision FP
// 
module CV_FP_CVT_FL2_I32_F64_NONSYNTH(input clk, input reset, output [31:0] result, input [63:0] arg, output FAIL);

   reg [63:0] b1, b2;

   // Perhaps use convertor 8 and then trim answer to 32 bits or assert FAIL
   always @(posedge clk) begin // Temporary, unsynthesisable implementation, not checking for overflow.
      b1 <= $rtoi(arg);
      b2 <= b1;
   end
   assign result = b2[31:0]; 
endmodule   

// Convertor no 6 - get 32-bit int from double-precision FP
// Fixed-latency, Double-Precision IEEE 1+11+52 = 64 bit format.
module CV_FP_CVT_FL2_I32_F64(input clk, input reset, output [31:0] result, input [63:0] arg, output FAIL);   

   parameter traceme = 0;
   wire [10:0]  exp0;
   wire [51:0] mant0;
   wire        signf0;
   assign { signf0, exp0, mant0 } = arg; // Deconstruct input arg1

   reg [52:0]  rr1, rr2;
   reg [10:0]  exp1, expx, bounder;
   reg [52:0]  mant1;
   reg         er2a, er2b;
   reg         signf1;
   reg         scase_zero1;
   reg         scase_nan1;
   reg         scase_inf1;
   wire        scase_zero0 = (mant0 == 0 && exp0 == 0); // special case zero.

   always @(posedge clk) begin
      scase_zero1 <= scase_zero0;
      scase_nan1  <= (mant0 != 0) && (exp0 == 11'h7ff);
      scase_inf1  <= (mant0 == 0) && (exp0 == 11'h7ff);
      signf1 <= signf0;
      mant1 <= mant0;
      exp1 <= exp0;
      er2a <= scase_nan1 || scase_inf1;
      er2b = 0;

      if (traceme) $display("%1t: %m arg0=%1h signf0=%1d  exp0=%1d  mant0=0x%h", $time, arg, signf0, exp0, mant0);
      if (traceme) $display("%1t: %m  signf1=%1d  exp1=%1d  mant1=0x%h", $time, signf1, exp1, mant1);      
      if (traceme) $display("%1t: %m: zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1d result=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, result);
      if (scase_zero1) begin
	 if (traceme) $display("%1t: %m:  zero", $time); 
	 rr2 <= 32'd0;
	 end
      else if (exp1 < 11'h3ff) begin
	 if (traceme) $display("%1t: %m:  underflow to zero, exp1=%1d", $time, exp1); 
	 rr2 <= 64'd0;
	 end
      else if (exp1 < 11'h3ff+52) begin
	 bounder = 52;
	 expx = exp1;
	 rr1 = mant1 | (1<<52);
	 while(expx < 11'h3ff+52 && bounder > 0) begin
	    rr1 = rr1 >> 1;
	    expx = expx + 1;
	    bounder = bounder - 1;
	    //if (traceme)  $display("%1t: %m:   rshifting   zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1h expx=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, expx);
	 end
	 rr2 <= (signf1) ? 32'd0-rr1: rr1;
      end
      else if (exp1 > 11'h200+30) begin
	 er2b = 1;
	 rr2 <= 0;
	 if (traceme) $display("%1t: %m:  overflow, exp1=%1d\n", $time, exp1); 
      end
      else begin
	 expx = exp1;
	 rr1 = mant1 | (1<<52);
	 bounder = 8;
	 while(expx > 11'h3ff+52 && bounder > 0) begin
	    rr1 = rr1 << 1;
	    expx = expx - 1;
	    bounder = bounder - 1;
	    //if (traceme) $display("%1t: %m:  aaaaaaaaaaaaa     zero1=%1d inf1=%1d nan1=%1d signf1=%1d rr1=%1d result=%1d\n", $time, scase_zero1, scase_inf1, scase_nan1, signf1, rr1, result);
	 end
	 rr2 <= (signf1) ? 32'd0-rr1: rr1;
      end // else: !if(expx > 128+30)
      
   end // always @ (posedge clk)
   
   assign result = rr2;
   assign FAIL = er2a || er2b;
endmodule

//==================================================================================================
// Convertor no 7 OLD VERSION - where is the new one then ?
// Assert error on inf and nan
module CV_FP_CVT_FL2_I64_F32__(input clk, input reset, output [63:0] result, input [31:0] arg, output FAIL);
   wire [7:0]  exp0;
   wire [22:0] mant0;
   wire        signf0;
   assign { signf0, exp0, mant0 } = arg; // Deconstruct input arg1

   // ... unfinished ...
      always @(posedge clk) $display ("%m is missing - TODO");
endmodule


//==================================================================================================
// Convertor no 8
// 
module CV_FP_CVT_FL2_I64_F64__ (input clk, output [63:0] result, input [63:0] arg, output FAIL);

  // missing ...
   always @(posedge clk) $display ("%m is missing - TODO");
endmodule   


// Convertor 9 - aka hpr_dbl2flt
// Convert double-precision to single-precision floating point.
// This can overflow. Combinatation FL (fixed-latency) of zero clocks.
module CV_FP_CVT_FL0_F32_F64(
			     input 	   clk,    // clock not used fro FL0 (combinational)
			     input         reset,  // reset is not used on most/all of these convertors, but having it may be useful in the future and reduces IP-XACT bus spec proliferations	   
			     output [31:0] result,
			     input [63:0]  arg,
			     output 	   FAIL);
   
   wire 				   signi;
   wire [10:0] 				   expi;
   wire [51:0] 				   manti;
   assign { signi, expi, manti } = arg;  // Deconstruct input arg
   
   wire 				   signo = signi;
   wire [7:0] 				   expo;
   wire [22:0] 				   manto;
   
   //   always @(posedge clk) $display("arg=%d", arg);
   
   wire 				   scase_zero = (arg[62:0] == 63'd0);
   wire 				   scase_inf_in = (expi == 11'h7ff) && (manti == 0);
   wire 				   scase_nan = (expi == 11'h7ff) && (manti != 0);
   
   // We can report FAIL on overflow but better to report infinity.
   assign FAIL = 0;
   
   wire 				   overflow = (expi[10] == expi[9]) ||(expi[10] == expi[8]) ||(expi[10] == expi[7]);
   assign                                  expo = { expi[10], expi[6:0]};
   assign                                  manto = manti[51:51-22];
   
   wire 				   scase_inf = scase_inf_in || overflow;
   
   assign result[31]    = signi;
   assign result[30:23] = (scase_inf)? 8'hff: (scase_nan)? 8'hff: (scase_zero)? 8'd0: expo;
   assign result[22:0]  = (scase_inf)? 23'd0: (scase_nan)? -23'd1: (scase_zero)? 23'd0: manto;

endmodule // CV_FP_CVT_FL0_F32_F64



// Convertor 10 - aka hpr_flt2dbl
// Convert single-precision to double-precision floating point.
module CV_FP_CVT_FL0_F64_F32(input clk,   // clock not used fro FL0 (combinational)
			     input reset,
			     output [63:0] result,
			     input [31:0]  arg,
			     output 	   FAIL);

   assign FAIL = 0; // FAIL not possible in this direction.
   assign result = {arg[31], arg[30], {3{~arg[30]}}, arg[29:23], arg[22:0], {29{1'b0}}};

endmodule // CV_FP_CVT_FL0_F64_F32


//==================================================================================================


// Variable-latency square-root operators - currently implemented in CSharp and Kiwi-Compiled - these will go via IP-XACT in the future

//  module CV_FP_SQRT_F32_VL(
//  module CV_DP_SQRT_F32_VL(
//    input 	clk,
//    input 	reset,
//    output 	rdy, //  Result ready.
//    input 	req, //  Request new operation starts. 
//    output [31:0] RR,
//    output [31:0] ARG,
//    output 	FAIL);
//   * ====================================================
//   * Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
//   *
//   * Developed at SunPro, a Sun Microsystems, Inc. business.
//   * Permission to use, copy, modify, and distribute this
//   * software is freely granted, provided that this notice
//   * is preserved.
//   * ====================================================
//  
//  	     ------------------------  	     ------------------------
//  	x0:  |s|   e    |    f1     |	 x1: |          f2           |
//  	     ------------------------  	     ------------------------
//  
//  	By performing shifts and subtracts on x0 and x1 (both regarded
//  	as integers), we obtain an 8-bit approximation of sqrt(x) as
//  	follows.
//  
//  		k  := (x0>>1) + 0x1ff80000;
//  		y0 := k - T1[31&(k>>15)].	... y ~ sqrt(x) to 8 bits
//  	Here k is a 32-bit integer and T1[] is an integer array containing
//  	correction terms. Now magically the floating value of y (y's
//  	leading 32-bit word is y0, the value of its trailing word is 0)
//  	approximates sqrt(x) to almost 8-bit.
//  
//  	Value of T1:
//  	static int T1[32]= {
//  	0,	1024,	3062,	5746,	9193,	13348,	18162,	23592,
//  	29598,	36145,	43202,	50740,	58733,	67158,	75992,	85215,
//  	83599,	71378,	60428,	50647,	41945,	34246,	27478,	21581,
//  	16499,	12183,	8588,	5674,	3403,	1742,	661,	130,};
//  
//      (2)	Iterative refinement
//  
//  	Apply Heron's rule three times to y, we have y approximates
//  	sqrt(x) to within 1 ulp (Unit in the Last Place):
//  
//  		y := (y+x/y)/2		... almost 17 sig. bits
//  		y := (y+x/y)/2		... almost 35 sig. bits
//  		y := y-(y-x/y)/2	... within 1 ulp
//
//     .... missing see Kiwi regression test58.cs
//  
//  endmodule // CV_FP_SQRT_F32_VL


// eof
