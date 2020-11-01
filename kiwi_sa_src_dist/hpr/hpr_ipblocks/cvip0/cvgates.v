// Vanilla gates - cvgates.cv
//
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// +  Verilog Source File
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// +
// +  (C) 1995-2015 DJ Greaves.
// +  HPR L/S
// +  University of Cambridge, Computer Laboratory.
// +  Cambridge, UK.
// +  
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// +
// +    
// + This file contains:
// +
// +   Leaf gates for CBG CSYN Verilog Compiler Version cv2 and HPR L/S library.
// +
// +
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//

// library cvgates.cv


module DFF_AR(q, ck, d, ar);
  output q;
  input ck, d, ar;
  CVDFF dff(q, d, ck, 1'd1, ar, 1'b0);
 
endmodule

module DFF_CEAR(q, ck, d, ce, ar);
  output q;
  input ck, d, ar, ce;
  CVDFF dff(q, d, ck, ce, ar, 1'd0);
endmodule


/* primitive celldefine */ module CVMUX2(o, sel, e1, e0); 
  output o;
  input sel, e1, e0;
  assign #5 o = (sel) ? e1 : e0;
endmodule

/* primitive celldefine */ module CVDFF(q, d, clk, ce, ar, spare);
  output q;
  reg q;
  initial q = 0;
  input clk;         // Clock (positive edge triggered)
  input d;           // Data input
  input ce;          // Clock enable
  input ar;          // Asynchronous reset
  input spare;


  integer last_d;
  integer last_clk;

  always @(posedge clk or posedge ar) 
	if (ar) q <= #10 0; 
	else if (ce)
		begin
		if ($time - last_d < 5)
			$display("Time %t, DFF %m violated set-up time", $time);
		last_clk = $time;
		q <= #10 (d & 1);
		end 

  always @(d) 
	begin
	last_d = $time;
	if ($time - last_clk < 4)
		$display("Time %t, DFF %m violated hold time", $time);
	end

endmodule 
//
//------------------------------------------------
//
/* primitive celldefine */ module CVXOR2(o, i1,i2);
  input i1, i2;
  output o;
  assign #3 o = i1 != i2;
endmodule

/* primitive celldefine */ module CVXOR3(o, i1,i2,i3);
  input i1, i2, i3;
  output o;
  assign #2 o = (i1 != i2) != i3;
endmodule

/* primitive celldefine */ module CVXOR4(o, i1,i2,i3,i4);
  input i1, i2, i3, i4;
  output o;
  assign #2 o = (i1 != i2) != (i3 != i4);
endmodule

//
//
//------------------------------------------------
/* primitive celldefine */ module CVAND2(o, i1,i2);
  input i1, i2;
  output o;
  assign #2 o = i1 & i2;
endmodule

/* primitive celldefine */ module CVAND3(o, i1,i2,i3);
  input i1, i2, i3;
  output o;
  assign #2 o = i1 & i2 & i3;
endmodule

/* primitive celldefine */ module CVAND4(o, i1,i2,i3,i4);
  input i1, i2, i3, i4;
  output o;
  assign #2 o = i1 & i2 & i3 & i4;
endmodule

/* primitive celldefine */ module CVAND5(o, i1,i2,i3,i4,i5);
  input i1, i2, i3, i4,i5;
  output o;
  assign #2 o = i1 & i2 & i3 & i4 & i5;
endmodule
//
//------------------------------------------------
/* primitive celldefine */ module CVOR2(o, i1,i2);
  input i1, i2;
  output o;
  assign #2 o = i1 | i2;
endmodule


/* primitive celldefine */ module CVOR3(o, i1,i2,i3);
  input i1, i2, i3;
  output o;
  assign #2 o = i1 | i2 | i3;
endmodule

/* primitive celldefine */ module CVOR4(o, i1,i2,i3,i4);
  input i1, i2, i3, i4;
  output o;
  assign #2 o = i1 | i2 | i3 | i4;
endmodule

/* primitive celldefine */ module CVOR5(o, i1,i2,i3,i4,i5);
  input i1, i2, i3, i4, i5;
  output o;
  assign #2 o = i1 | i2 | i3 | i4 | i5;
endmodule
//
//
//
//------------------------------------------------

/* primitive celldefine */ module CVBUF(o, i);
  output o;
  input i;
  assign #2 o = i;
endmodule

/* primitive celldefine */ module CVBUFIF1(o, i, g);
  output reg o;
  input i, g;
  always @(i or g) if (g) #2 o = i;
endmodule
//
//
//
//------------------------------------------------
/* primitive celldefine */ module CVINV(o, i);
  output o;
  input i;
  assign #2 o = ~i;  
endmodule
//
//
//------------------------------------------------
//
//
//------------------------------------------------
/* primitive celldefine */ module LO(lo);
  output lo;
  assign lo = 0;
endmodule
//
//
//------------------------------------------------
/* primitive celldefine */ module HI(hi);
  output hi;
  assign hi = 1;
endmodule
//
//
//------------------------------------------------

module CV_INT_FL1_MULTIPLIER_US  // Fixed latency of 1, fully-pipelined, unsigned multiplier. Used from 5 to 16 bits of result width or so.
  #(parameter RWIDTH = 16,
   parameter A0WIDTH = 16,
   parameter A1WIDTH = 16,
   parameter trace_me = 0)
  (
   input 		     clk,
   input 		     reset,
  
   output [RWIDTH-1:0]   RR,
   input /*unsigned*/ [A0WIDTH-1:0] XX,
   input /*unsigned*/ [A1WIDTH-1:0] YY,
   output FAIL);
   reg /*unsigned*/ [RWIDTH-1:0] aa1, bb1;
   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
   end
   assign RR = aa1 * bb1;
   //      $display("%m  %d = %d * %d", aa1, bb1, RR);
   assign FAIL = 0; // Need to detect overflow in 'checked' mode.
endmodule
//
//
//

module CV_INT_FL2_MULTIPLIER_US  // Fixed latency of 2, fully-pipelined, unsigned multiplier.  Typically used for 16 to 32 bits.
  #(parameter RWIDTH = 32,
   parameter A0WIDTH = 32,
   parameter A1WIDTH = 32,
   parameter trace_me = 0)    
  (
   input 		     clk,
   input 		     reset,
  
   output reg [RWIDTH-1:0]   RR,
   input /*unsigned*/ [A0WIDTH-1:0] XX,
   input /*unsigned*/ [A1WIDTH-1:0] YY,
   output FAIL);

   reg /*unsigned*/ [A0WIDTH-1:0] 	  aa1;
   reg /*unsigned*/ [A1WIDTH-1:0] 	  bb1;   


   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      RR <= aa1 * bb1;
//      $display("%m  %d = %d * %d", aa1, bb1, RR);
   end
   assign FAIL = 0;
   
endmodule
//
//
module CV_INT_FL3_MULTIPLIER_US  // Fixed latency of 2, fully-pipelined, unsigned multiplier.  Typically used for 16 to 32 bits.
  #(parameter RWIDTH = 32,
   parameter A0WIDTH = 32,
   parameter A1WIDTH = 32,
   parameter trace_me = 0)    
  (
   input 		     clk,
   input 		     reset,
  
   output reg [RWIDTH-1:0]   RR,
   input /*unsigned*/ [A0WIDTH-1:0] XX,
   input /*unsigned*/ [A1WIDTH-1:0] YY,
   output FAIL);

   reg /*unsigned*/ [A0WIDTH-1:0] 	  aa1;
   reg /*unsigned*/ [A1WIDTH-1:0] 	  bb1;   
   reg /*unsigned*/ [RWIDTH-1:0] 	  rr0;   

   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      rr0 <= aa1 * bb1;
      RR <= rr0;
//      $display("%m  %d = %d * %d", aa1, bb1, RR);
   end
   assign FAIL = 0;
   
endmodule
//
//
//
module CV_INT_FL1_MULTIPLIER_S  // Fixed latency of 1, fully-pipelined, signed multiplier. Used from 5 to 16 bits of result width or so.
  #(parameter RWIDTH = 16,
    parameter A0WIDTH = 16,
    parameter A1WIDTH = 16,
    parameter trace_me = 0)
  (
   input 		     clk,
   input 		     reset,
  
   output signed [RWIDTH-1:0] RR,
   input signed [A0WIDTH-1:0] XX,
   input signed [A1WIDTH-1:0] YY,
   output FAIL);

   reg signed [A0WIDTH-1:0] 	  aa1;
   reg signed [A1WIDTH-1:0] 	  bb1;   



   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      if (trace_me) $display("%m  %d * %d -> %d", aa1, bb1, RR);
   end
   assign RR = aa1 * bb1;

   assign FAIL = 0;
endmodule
//
//
//
module CV_INT_FL2_MULTIPLIER_S  // Fixed latency of 2, fully-pipelined, signed multiplier.  Typically used for 16 to 32 bits.
  #(parameter RWIDTH = 32,
    parameter A0WIDTH = 32,
    parameter A1WIDTH = 32,
    parameter trace_me = 0)    
  (
   input 		     clk,
   input 		     reset,
  
   output reg signed [RWIDTH-1:0] RR,

   input signed [A0WIDTH-1:0] XX,
   input signed [A1WIDTH-1:0] YY,
   output FAIL); // Overflow FAIL for use in checked mode.

   reg signed [A0WIDTH-1:0] 	  aa1;
   reg signed [A1WIDTH-1:0] 	  bb1;   


   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      RR <= aa1 * bb1; // Asterisk will be automatically replaced by modern FPGA tool flows.
      if (trace_me) $display("%m  %d * %d = X(%d)", aa1, bb1, RR);
   end

   assign FAIL = 0;
endmodule



module CV_INT_FL3_MULTIPLIER_S  // Fixed latency of 3, fully-pipelined, signed multiplier.  Typically used for 16 to 32 bits on Xilinx. Altera is quicker?
  #(parameter RWIDTH = 32,
    parameter A0WIDTH = 32,
    parameter A1WIDTH = 32,
    parameter trace_me = 0)    
  (
   input 		     clk,
   input 		     reset,
  
   output reg signed [RWIDTH-1:0] RR,

   input signed [A0WIDTH-1:0] XX,
   input signed [A1WIDTH-1:0] YY,
   output FAIL); // Overflow FAIL for use in checked mode.

   reg 	signed [A0WIDTH-1:0] 	  aa1;
   reg  signed [A1WIDTH-1:0] 	  bb1;   
   reg  signed [RWIDTH-1:0] 	  r0;

   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      r0 <= aa1 * bb1; // Asterisk will be automatically replaced by modern FPGA tool flows.
      RR <= r0;
      if (trace_me) $display("%m  %d * %d = X(%d)", aa1, bb1, RR);
   end

   assign FAIL = 0;
endmodule



module CV_INT_FL5_MULTIPLIER_S  // Fixed latency of 5, fully-pipelined, signed multiplier.  Typically used for 64 bits or so.
  #(parameter RWIDTH = 32,
    parameter A0WIDTH = 32,
    parameter A1WIDTH = 32,
    parameter trace_me = 0)        
  (
   input 		     clk,
   input 		     reset,
  
   output reg signed [RWIDTH-1:0]   RR,
   input signed [A0WIDTH-1:0] XX,
   input signed [A1WIDTH-1:0] YY,
   output FAIL); // Overflow error for use in checked mode in future...

   reg signed [RWIDTH-1:0] 	  aa1, bb1;
   reg signed [RWIDTH-1:0] 	  mid0, mid1, mid2;

   always @(posedge clk) begin
      aa1 <= XX; // This assign will sign extend when RR is wider.
      bb1 <= YY;
      mid0 <= aa1 * bb1; // Asterisk will be replaced by FPGA synthesiser.
      if (trace_me) $display("%m signed multiplier %d * %d --> %d", aa1, bb1, RR);

      mid1 <= mid0;
      mid2 <= mid1;
      RR <= mid2;
   end

   assign error = 0;
endmodule


module CV_INT_FL5_MULTIPLIER_US  // Fixed latency of 5, fully-pipelined, un-signed multiplier.  Typically used for 64 bits or so.
  #(parameter RWIDTH = 32,
    parameter A0WIDTH = 32,
    parameter A1WIDTH = 32,
    parameter trace_me = 0)        
  (
   input 		     clk,
   input 		     reset,
  
   output reg [RWIDTH-1:0]   RR,
   input [A0WIDTH-1:0] XX,
   input [A1WIDTH-1:0] YY,
   output FAIL); // Overflow error for use in checked mode. Not currently driven.

   reg [RWIDTH-1:0] 	  aa1, bb1;
   reg [RWIDTH-1:0] 	  mid0, mid1, mid2;

   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
      mid0 <= aa1 * bb1; // Asterisk will be automatically replaced by modern FPGA tool flows using DSP slices and so on.
      if (trace_me) $display("%m unsigned multiplier 0x%h * 0x%h -> X(0x%h)", aa1, bb1, mid0);

      mid1 <= mid0;
      mid2 <= mid1;
      RR <= mid2;
   end

   assign error = 0;
endmodule


module CV_INT_FL1_ADDER  // Fixed latency of 1, fully-pipelined, adder with signed and unsigned overflow outputs.
  #(parameter RWIDTH = 32,
   parameter A0WIDTH = 32,
   parameter A1WIDTH = 32,
   parameter trace_me = 0)            
   
  (
   input 	       clk,
   input 	       reset,
  
   output [RWIDTH-1:0] RR,
   input [A0WIDTH-1:0]  XX,
   input [A1WIDTH-1:0]  YY,
   output 	       unsigned_ovf, // Overflow errors for use in checked mode.
   output 	       signed_error); 

   reg [RWIDTH-1:0] 	  aa1, bb1;

   always @(posedge clk) begin
      aa1 <= XX;
      bb1 <= YY;
   end

   assign unsigned_ovf = ({1'b0, aa1} + {1'b0, bb1}) >> RWIDTH;
   assign signed_ovf = (aa1[A0WIDTH-1] && bb1[A1WIDTH-1] && !RR[RWIDTH-1]) ||
		       (!aa1[A0WIDTH-1] && !bb1[A1WIDTH-1] && RR[RWIDTH-1]);
   assign RR = aa1 + bb1; 
//      $display("%m FL1 adder %d = %d * %d", aa1, bb1, RR);


   assign error = 0;
endmodule


// This is a fixed-latency divider whose latency is the word width.
// It is not fully-pipelined.
// It is slower for typical use than the standard preshifting divider but has better worst case delay by a factor of two.
// You could run this in parallel with the variable-latency divider and return the earlier result while abending the partner.
// The latency is the result precision, roughly, but this needs checking!
module CV_INT_FL_DIVIDER_US // slower_on_average than vari-latency.
  #(parameter RWIDTH = 32,
    parameter NWIDTH = 32,
    parameter DWIDTH = 32,
    parameter trace_me=0,
    parameter divmod = 1)
   (
    input 		    clk,
    input 		    reset,
    output reg 		    rdy,
    input 		    REQ,

    
    output [RWIDTH-1:0] RR,
    input [NWIDTH-1:0] 	    NN,
    input [DWIDTH-1:0] 	    DD,
    output reg 		    FAIL);

   reg [DWIDTH-1:0] 	    dd;
   reg [NWIDTH-1:0] 	    pp;
   reg [RWIDTH-1:0] 	    qq, nnh, nnl;      
   
   reg phase;

   assign RR = (divmod) ? qq : nnh; // Select quotient or remainder as output.
   
   always @(posedge clk) begin
     rdy <= 0; // Gets overwritten.

     if (REQ && phase != 0) begin
	$display("%t: cvgates: %m +++ START REQ WHILE BUSY", $time);
	FAIL <= 1; // Complain via this error msg.
	end

     
     if (reset) begin
	phase <= 0;
	FAIL <= 0;
     end
     
     else case (phase)
	    0:begin  // Inputs are accepted on the REQ cycle.
	       if (REQ) begin
		  qq <= 0;                     
		  dd <= DD;
		  nnl <= NN;
		  nnh <= 0;
		  if (dd == 0) begin
		     FAIL <= 1; // Sticky until reset for now
		     phase <= 0; // div by zero error;
		  end
		  else begin
 		     if (trace_me) $display("divider: init nn=0x%1h dd=0x%1h qq=0x%1h", nnl, dd, qq);
		     pp <= 1 << (RWIDTH-1);
		     phase <= 1;
		  end
		  if (trace_me) $display("%t: cvgates: %m start divide %d/%d", $time, NN, DD);
               end
	    end // case: 0
	    
	    1:begin
	       pp <= pp >> 1;
	       { nnh, nnl } = { nnh, nnl } << 1;
	       if (nnh >= dd) begin	 
		  nnh = nnh - dd;
		  qq <= qq | pp;
	       end


	       if (pp == 1) begin
		  phase <= 0;
		  rdy <= 1;
	       end

	       if (trace_me)  $display("%1t, %m: cvgates divider: pp=%8h nnh=%8h nnl=%8h dd=%8h  qq=%8h", $time, pp, nnh, nnl, dd, qq);

	    end
	    
	  endcase
     end

endmodule



// Variable-latency ALUs. VLA protocol. VLA-IHN-OHY
// Handshake is HFAST1:
//    New input args are latched on a cycle where REQ is asserted. REQ should just be asserted for one cycle but will, in any case, be ignored if asserted when the unit is busy.
//    Results are ready on the outputs during the cycle when rdy is asserted, which will be just one cycle in response to a REQ.
//    It is allowed to assert REQ in a cycle when the unit is driving rdy from the previous operation.
//    The output value, once live, remains valid until another operation starts (the cycle after a cycle where REQ holds).
//    No combinational path between inputs and outputs, including {\tt REQ} and {\tt rdy}, is allowed inside the component.
//
// 32-bit (by default) integer divider. unsigned.  Radix 2 - Prefer Radix 4 perhaps.
// 
// 

module CV_INT_VL_DIVIDER_US
  #(parameter RWIDTH = 32,
    parameter NWIDTH = 32,
    parameter DWIDTH = 32,
    parameter trace_me=0)
   (
    input 		clk,
    input 		reset,
    input 		REQ,
    output 		ACK,

    
    output [RWIDTH-1:0] RR,
    input [NWIDTH-1:0] 	NN,
    input [DWIDTH-1:0] 	DD,
    output 		FAIL);

   CV_INT_VL_DIVMOD_US  // unsigned core instance.
     #(1, // Pass 1 to divmod for divide
       RWIDTH, NWIDTH, DWIDTH, trace_me)
   corat
     (
      .clk(clk),      .reset(reset),
      .ACK(ACK),      .REQ(REQ),

      
      .RR(RR),
      .NN(NN),
      .DD(DD),
      .FAIL(FAIL));
   
endmodule

module CV_INT_VL_MODULUS_US
  #(parameter RWIDTH = 32,
    parameter NWIDTH = 32,
    parameter DWIDTH = 32,
    parameter trace_me=0)
   (
    input 		clk,
    input 		reset,
    output 		ACK,
    input 		REQ,
    
    output [RWIDTH-1:0] RR,
    input [NWIDTH-1:0] 	NN,
    input [DWIDTH-1:0] 	DD,
    output  		FAIL);

   CV_INT_VL_DIVMOD_US  // unsigned core instance.
     #(0, // Pass 1 to divmod for modulus
       RWIDTH, NWIDTH, DWIDTH, trace_me)
   corat
     (
      .clk(clk),
      .reset(reset),
      .REQ(REQ),
      .ACK(ACK),
      
      .RR(RR),
      .NN(NN),
      .DD(DD),
      .FAIL(FAIL));
   
endmodule

// 32-bit (by default) integer divider. Signed.
// Inputs are accepted on the REQ cycle.  ACK holds for one cycle when the answer is ready. REQ can then be asserted during the ACK cycle for new computation.
module CV_INT_VL_MODULUS_S
  #(parameter RWIDTH = 32,
   parameter NWIDTH = 32,
   parameter DWIDTH = 32,
   parameter trace_me=0)
  (
  input 		     clk,
  input 		     reset,
  output 		     ACK, //  Result ready.
  input 		     REQ, //  REQuest new operation starts. 
  output signed [RWIDTH-1:0] RR,
  input signed [NWIDTH-1:0]  NN,
  input signed [DWIDTH-1:0]  DD,
  output 		     FAIL);

   
   // Instantiate an unsigned mod unit with sign correction rules around it.
   // It has the same width so that it can handle abs(MININT).
  reg sign;
   
   always @(posedge clk) 
     if (reset) sign <= 0;
     else if (REQ) sign <= NN[NWIDTH-1] ^ DD[DWIDTH-1];
   
   wire [RWIDTH-1:0] rr_unsigned; 
   wire [NWIDTH-1:0] corat_nn = NN[NWIDTH-1] ? 0-NN: NN; 
   wire [DWIDTH-1:0] corat_dd = DD[DWIDTH-1] ? 0-DD: DD;

   assign RR = (sign) ? 0-rr_unsigned: rr_unsigned;

  // We want the same behaviour as dotnet or C# for integer division with one argument negative.  This is round towards zero.
   
   CV_INT_VL_MODULUS_US  // unsigned core instance.
     #(RWIDTH, NWIDTH, DWIDTH, trace_me)
   corat
     (
      .clk(clk),
      .reset(reset),
      .REQ(REQ),
      .ACK(ACK),
      
      .RR(rr_unsigned),
      .NN(corat_nn),
      .DD(corat_dd),
      .FAIL(FAIL));
     

endmodule



// Variable-latency integed ivider or modulus unit. 
module CV_INT_VL_DIVMOD_US
  #(parameter divmod = 1,
    parameter RWIDTH = 32,
    parameter NWIDTH = 32,
    parameter DWIDTH = 32,
    parameter trace_me=0)
   (
    input 		clk,
    input 		reset,
    output reg 		ACK,
    input 		REQ,

    
    output [RWIDTH-1:0] RR,
    input [NWIDTH-1:0] 	NN,
    input [DWIDTH-1:0] 	DD,
    output reg 		FAIL);
   
   reg [RWIDTH-1:0] 	rr;
   reg [DWIDTH-1:0] 	dd;   
   reg [NWIDTH-1:0] 	pp, nn;   
   
   reg [1:0] 		phase;

   parameter radix = 2; // Radix 4 seems to help very little in reality.
   
   assign RR = (divmod) ? rr : nn; // Select quotient or remainder as output.

   reg 			sixteen, eight;
   reg 			    c0, c1, c2;
   reg [1:0] 		    r4r;
  always @(posedge clk) begin
     ACK <= 0; // Gets overwritten.

     if (REQ && phase != 0) begin
	$display("%t: cvgates: %m +++ START REQ WHILE BUSY", $time);
	FAIL <= 1; // Complain about REQuest when already busy.
	end
     
     if (reset) begin
	phase <= 0;
	FAIL <= 0;
	sixteen <= 0;
	eight <= 0;
     end
     
     else case (phase)
	    0:begin  // Inputs are accepted on the REQ cycle.
	       if (REQ) begin
		  phase <= 1;
		  pp <= 1;
		  dd <= DD;
		  nn <= NN;
		  //sixteen <= (DWIDTH > 16 && DD[DWIDTH-1:16]==0) && (NWIDTH > 16 && NN[DWIDTH-1:16]==0);
		  
		  rr <= 0;
		  if (trace_me) $display("%t: cvgates: %m start divide %d/%d", $time, NN, DD);
               end
	    end // case: 0
	    
	    1:begin // Preshift.
	       if (dd == 0) begin
		  FAIL <= 1;
		  phase <= 0; // div by zero error;
	       end
	       
	       if (dd==1) begin
		  ACK <= 1;
		  rr <= nn;
		  nn <= 0;
		  phase <= 0; // divide by one shortcut (tests more situations even if not worthwhile performancewise).
	       end

	       if (dd[NWIDTH-1] || dd >= nn) phase <= 2;
//	       else if (0 && !dd[NWIDTH-1:NWIDTH-8]) begin 
//		  dd <= dd << 7;
//		  pp <= pp << 7;
//	       end
	       else begin 
		  dd <= dd << 1;
		  pp <= pp << 1;
	       end
	    end
	    
	    2:begin  
	       if (trace_me) $display("%t: cvgates: %m divide step nn=%1h dd=%1h rr=%1h pp=%1h", $time, nn, dd, rr, pp);      
	       if (radix == 4 && pp >=2) begin
		  c0 = nn >= (dd - dd / 2);
		  c1 = nn >= dd;
		  c2 = nn >= (dd + dd / 2);
		  rr <= rr | ((c2) ? (pp+pp/2): (c1)?pp: (c0)?(pp/2): 0);
		  nn <= nn - ((c2) ? (dd+dd/2): (c1)?dd: (c0)?(dd/2): 0);
		  dd <= dd >> 2;
		  pp <= pp >> 2;
		  //$display("%m: radix4 op c=%1d%1d%1d dd=%1d pp=%1d", c0, c1, c2, dd, pp);
	       end
	       else begin
		  if (nn >= dd) begin	 
		     rr <= rr | pp;
		     nn <= nn - dd;
		  end
		  dd <= dd >> 1;
		  pp <= pp >> 1;
	       end // else: !if(radix == 4 && pp >=2)
	       if (pp == 1) begin
		  if (trace_me) $display("%t: cvgates: %m divide ready ans = %d", $time, rr);      		
		  phase <= 0;
		  ACK <= 1;
	       end

	    end
	    
	  endcase
     end

endmodule
//

// 32-bit (by default) integer divider. Signed.
// HFAST1 - maximum of one job outstanding.
// Inputs are accepted on the REQ cycle.  ACK holds for one cycle when the answer is ready. REQ can then be asserted during the ACK cycle for new computation.
module CV_INT_VL_DIVIDER_S
  #(parameter RWIDTH = 32,
   parameter NWIDTH = 32,
   parameter DWIDTH = 32,
   parameter trace_me=0)
  (
  input 		     clk,
  input 		     reset,
  output 		     ACK, //  Result ready.
  input 		     REQ, //  REQuest new operation starts. 
  output signed [RWIDTH-1:0] RR,
  input signed [NWIDTH-1:0]  NN,
  input signed [DWIDTH-1:0]  DD,
  output 		     FAIL);

   
   // Instantiate an unsigned divider with sign correction rules around it.
   // It has the same width so that it can handle abs(MININT).
  reg sign;
   
   always @(posedge clk) 
     if (reset) sign = 0;
     else if (REQ) sign <= NN[NWIDTH-1] ^ DD[DWIDTH-1];
   
   wire [RWIDTH-1:0]  rr_unsigned; 
   wire [NWIDTH-1:0] corat_nn = NN[NWIDTH-1] ? 0-NN: NN; 
   wire [DWIDTH-1:0] corat_dd = DD[DWIDTH-1] ? 0-DD: DD;

   assign RR = (sign) ? 0-rr_unsigned: rr_unsigned;

  // We want the same behaviour as dotnet or C# for integer division with one argument negative.  This is round towards zero.
   
   CV_INT_VL_DIVIDER_US  // unsigned core instance.
     #(RWIDTH, NWIDTH, DWIDTH, trace_me)
   corat
     (
      .clk(clk), .reset(reset),
      .ACK(ACK), .REQ(REQ),
     
      .RR(rr_unsigned),
      .NN(corat_nn),
      .DD(corat_dd),
      .FAIL(FAIL));
     

endmodule
//
//


// ---------------------------------------------------------------------------------------------------------------
// RAMs
/*
The HPR CV_IP0 library provides some definitions of RAMs in cvgates.
These are all single-laned at the moment.  The alternative to having them is for the
FPGA tools to infer RAMs directly from the RTL output from
verilog_gen, which should work just as well in terms of efficiency and
performance, but they are provided and used instead for several reasons:

    a) When simulating the RTL, it is handy to have a level of hierarchy in the viewer and netlist browser.

    b) Using explicit component instances assists with layout and area budget computation.

    c) Using explicit component instances allows the code inside the
       definitions to be manually coded in a style that matches the known
       RAM-inference rules of the logic synthesiser and provides a site for
       any manual manual markup with packing, positioning and other pragmas
       (distributed versus block) etc of the data array to match particular
       back-end preferences. Alternatively, RAM-inferencing in these
       definitions need not be used at all and they instead might use
       structural instances via vendor blocks such as Virtex 7 BRAM_SINGLE_MACRO.

    d) RTL output via verilog_gen is not always part of the recipe where RAMs are needed

    e) For multiple write lanes in the future, laned versions of the
       library blocks will be needed. Note that the Virtex 7 seems to support
       up to 8 lanes in its BRAM_SINGLE_MACRO.  But does this really do
       anything on Virtex 7 different from using multiple independent RAMs
       that have their address busses commoned?
*/	       

// Synchronous Static RAM SSRAM.   
// Single-ported, zero latency (combinational)  read SSRAM.
module CV_SP_SSRAM_FL0 #(
   parameter DATA_WIDTH = 32,
   parameter ADDR_WIDTH = 10,
   parameter WORDS = 1024,
   parameter LANE_WIDTH = 32,
   parameter trace_me=0)
   (
   input 			     clk,
   input 			     reset,
   output [DATA_WIDTH-1:0] 	     rdata,
   input [ADDR_WIDTH-1:0] 	     addr,
   input [DATA_WIDTH/LANE_WIDTH-1:0] wen,
   input 			     ren,
   input [DATA_WIDTH-1:0] 	     wdata);

   parameter n_lanes = DATA_WIDTH/LANE_WIDTH;
   
   reg [DATA_WIDTH-1:0] data_array [WORDS-1:0];

   assign rdata = data_array[addr];

   generate
     genvar i;
     for (i = 0; i < n_lanes; i = i+1) begin
       always @(posedge clk) 
       if (wen[i]) begin
	  if (trace_me) $display("%t,%m: Lane %1d: Write [%1d] := %1d",  $time, i, addr, wdata[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH]);
	  data_array[addr][(i+1)*LANE_WIDTH-1:i*LANE_WIDTH] <= wdata[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH];
	  end
       end
   endgenerate

endmodule


// Synchronous Static RAM SSRAM.   
// Single-ported, fixed latency of one-cycle read SSRAM.
// For writes: data, address, lanes and wen are all presented in a common clock cycle and are 'posted' in that there is no response.
// For reads: address and ren are presented in one clock cycle and the data out appears, fully-pipelined, FLn cycles later.
module CV_SP_SSRAM_FL1 #(
   parameter DATA_WIDTH = 32,
   parameter ADDR_WIDTH = 10,
   parameter WORDS = 1024,
   parameter LANE_WIDTH = 32,
   parameter trace_me=0)
   (
   input 			     clk,
   input 			     reset,
   output reg [DATA_WIDTH-1:0] 	     rdata,
   input [ADDR_WIDTH-1:0] 	     addr,
   input [DATA_WIDTH/LANE_WIDTH-1:0] wen,
   input 			     ren,
   input [DATA_WIDTH-1:0] 	     wdata);

   parameter n_lanes = DATA_WIDTH/LANE_WIDTH;
   
   reg [DATA_WIDTH-1:0] data_array [WORDS-1:0];

   always @(posedge clk) begin
     if (ren) begin
	rdata <= data_array[addr];
	if (trace_me) $display("%1t, %m: Read a=0x%1h, d=0x%1h", $time, addr, data_array[addr]);
     end
   end

   generate
     genvar i;
     for (i = 0; i < n_lanes; i = i+1) begin
       always @(posedge clk) 
       if (wen[i]) begin
	  if (trace_me) $display("%t,%m: Lane %1d: Write [%1d] := %1d",  $time, i, addr, wdata[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH]);
	  data_array[addr][(i+1)*LANE_WIDTH-1:i*LANE_WIDTH] <= wdata[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH];
	  end
       end
   endgenerate

endmodule

// The RAMs and FIFOs (in future) should really be in their own file.

// Synchronous Static RAM SSRAM.   
// Dual-ported, fixed latency of one-cycle read SSRAM.
// For writes: data, address, lanes and wen are all presented in a common clock cycle and are 'posted' in that there is no response.
// For reads: address and ren are presented in one clock cycle and the data out appears, fully-pipelined, FLn cycles later.
module CV_2P_SSRAM_FL1 #(
   parameter DATA_WIDTH = 32,
   parameter ADDR_WIDTH = 10,
   parameter WORDS = 1024,
   parameter LANE_WIDTH = 32,
   parameter trace_me=0)
   (
   input 			     clk,         // TODO this clock should be part of the port for a future domain crossing variant.
   input 			     reset,

    // Port 0
   output reg [DATA_WIDTH-1:0] 	     rdata0,
   input [ADDR_WIDTH-1:0] 	     addr0,
   input [DATA_WIDTH/LANE_WIDTH-1:0] wen0,
   input 			     ren0,
   input [DATA_WIDTH-1:0] 	     wdata0,

    // Port 1
   output reg [DATA_WIDTH-1:0] 	     rdata1,
   input [ADDR_WIDTH-1:0] 	     addr1,
   input [DATA_WIDTH/LANE_WIDTH-1:0] wen1,
   input 			     ren1,
   input [DATA_WIDTH-1:0] 	     wdata1);

   parameter n_lanes = DATA_WIDTH/LANE_WIDTH;
   
   reg [DATA_WIDTH-1:0] data_array [WORDS-1:0];

   always @(posedge clk) begin
     if (ren0) begin
	rdata0 <= data_array[addr0];
	if (trace_me) $display("%1t, %m: Read a=0x%1h, d=0x%1h", $time, addr0, data_array[addr0]);
     end
      
      if (ren1) begin
	 rdata1 <= data_array[addr1];
	 if (trace_me) $display("%1t, %m: Read a=0x%1h, d=0x%1h", $time, addr1, data_array[addr1]);
	 
      end
   end

   generate
     genvar i;
     for (i = 0; i < n_lanes; i = i+1) begin
       always @(posedge clk) begin
	  if (wen0[i] && wen1[i] && addr0 == addr1) $display("%1t, %m: Warning - both ports write to same address 0x%1h lane %1d",$time, addr0, i);

	  if (wen0[i]) begin
	     if (trace_me) $display("%t,%m: Lane %1d: Write [%1d] := %1d",  $time, i, addr0, wdata0[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH]);
	     data_array[addr0][(i+1)*LANE_WIDTH-1:i*LANE_WIDTH] <= wdata0[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH];
	  end
	  
	  if (wen1[i]) begin
	     if (trace_me) $display("%t,%m: Lane %1d: Write [%1d] := %1d",  $time, i, addr1, wdata1[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH]);
	     data_array[addr1][(i+1)*LANE_WIDTH-1:i*LANE_WIDTH] <= wdata1[(i+1)*LANE_WIDTH-1:i*LANE_WIDTH];
	  end
       end
     end
   endgenerate

endmodule



// end of cvgates.cv
