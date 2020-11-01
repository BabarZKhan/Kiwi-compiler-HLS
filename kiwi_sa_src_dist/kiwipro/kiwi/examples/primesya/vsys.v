// $Id: vsys.v,v 1.2 2010/08/11 08:51:35 djg11 Exp $
// Kiwi Scientific Acceleration
// vsys.v - A test wrapper for simulating very simple tests with clock and reset.
//

`timescale 1ns/10ps
`define timelimit (100 * 1000 * 1000)


module ACME_SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 400 reset = 0; end
   always #100 clk = !clk;

   initial begin # `timelimit $display("Finish HDL simulation on timeout %t.", $time); $finish(); end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   wire [7:0]  ksubsAbendSyndrome;
   wire        finished = (ksubsAbendSyndrome != 0) && (ksubsAbendSyndrome != 128);

   always @(posedge clk) begin

	//$display("pc, thread0 = %d", the_dut.xpc10nz);
	end

   DUT dut(.clk(clk), 
	   .reset(reset),
	   .ksubsAbendSyndrome(ksubsAbendSyndrome)
	   );


  always @(posedge clk) begin
      if (finished) begin
	 $display("Exit on finished syndrome 0x%02h reported after %d clocks.", ksubsAbendSyndrome, $time/10);

	 #5000  $finish;
	 end
     //$display("%1t,  pc=%1d, codesent=%h" , $time, the_dut.xpc10nz, codesent);
  end


endmodule
// eof
