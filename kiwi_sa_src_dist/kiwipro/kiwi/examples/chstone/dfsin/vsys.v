//
// Kiwi Scientific Acceleration  
// University of Cambridge, Computer Laboratory.
//
// vsys.v - A test wrapper for RTLSIM execution environment
//
`timescale 1ns/1ns

module SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; #33 reset = 0; end
   always #5 clk = !clk;

   initial begin # (1000 * 100 * 1000) $display("Finish HDL simulation on timeout %t.", $time); $finish(); end
   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   wire done;
   wire [31:0] codesent;
   dfsin_floatnat the_dfsin(.clk(clk), 
	       .reset(reset),
	       .done(done)
	       );

   always @(posedge clk) begin
      if (done) begin
	 $display("Exit on done asserted after %d clocks.", $time/10);
	 @(posedge clk)	 $finish;
	 end
//      $display("%1t,  pc=%1d, codesent=%h" , $time, the_dfsin.xpc10nz, 0);
   end
   
endmodule
// eof


