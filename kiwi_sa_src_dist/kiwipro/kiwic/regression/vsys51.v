// (C) 2010-16 DJ Greaves, University of Cambridge.
// //
// Kiwi Scientific Acceleration  
// University of Cambridge, Computer Laboratory
//
// vsys51.v - A test wrapper for simulating very simple tests with clock and reset.


//
//
`timescale 1ns/1ns

module ACME_SIMSYS();

   reg clk, reset;
   initial begin reset = 1; clk = 1; # 43 reset = 0; end
   always #5 clk = !clk;

   wire [7:0]  ksubsAbendSyndrome;
   wire        finished_signal = (ksubsAbendSyndrome != 255);
   reg [7:0]   finished_shifter;
   integer     clock_ticks;
   initial # (1 * 1000 * 1000) begin
      $display("\nFinish HDL simulation on timeout %t.", $time); 
      $finish(); 
      end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   wire signed [15:0] uword128_out_bopper;
   wire [63:0] uword128_out_word0;
   wire [63:0] uword128_out_word1;


   reg signed [15:0] uword128_in_bopper;
   reg [63:0] 	  uword128_in_word0;
   reg [63:0] 	   uword128_in_word1;

   DUT the_dut(.clk(clk), 
	       .reset(reset),
	       .hpr_abend_syndrome(ksubsAbendSyndrome),

	       .uword128_in_bopper(uword128_in_bopper),
	       .uword128_in_word0(uword128_in_word0),
	       .uword128_in_word1(uword128_in_word1),	       

	       .uword128_out_bopper(uword128_out_bopper),
	       .uword128_out_word0(uword128_out_word0),
	       .uword128_out_word1(uword128_out_word1)

	       
	       );

     always @(posedge clk) begin
	clock_ticks <= (reset) ?0: clock_ticks+1;


	uword128_in_word0 <= (reset) ? 0: 	uword128_in_word0 + 2;
	uword128_in_word1 <= (reset) ? 0: 	uword128_in_word1 + (1<<24);		
	uword128_in_bopper <= (reset) ? 0: 	uword128_in_bopper + 1;
	$display("Yuckout %h %h %h",   uword128_in_bopper, uword128_in_word1,  uword128_in_word0);

	finished_shifter = (reset)?0: { finished_shifter, (finished_shifter[0]||finished_signal) };
	if (finished_shifter[7]) begin
	   $display("Finished with code 0x%02h at %1t after %d clocks", ksubsAbendSyndrome, $time, clock_ticks);
	   $finish(0);
	   end
//	        $display("%1t, pc, thread_pc10=%1d:  rdy=%1d req=%1d rr=%1d nn=%1d dd=%1d err=%1d v1=%1d", $time, the_dut.xpc10nz, the_dut.iuDIVIDER10_rdy, the_dut.iuDIVIDER10_req, the_dut.iuDIVIDER10_RR, the_dut.iuDIVIDER10_NN, the_dut.iuDIVIDER10_DD, the_dut.iuDIVIDER10_err, the_dut.TKWr1_5_V_1);
	//$display("%1t, pc, thread_pc10=%1d:  rdy=%1d req=%1d rr=%1d nn=%1d dd=%1d err=%1d v1=%1d", $time, the_dut.xpc10nz, the_dut.iuDIVIDER10_rdy, the_dut.iuDIVIDER10_req, the_dut.iuDIVIDER10_RR, the_dut.iuDIVIDER10_NN, the_dut.iuDIVIDER10_DD, the_dut.iuDIVIDER10_err, the_dut.TKWr1_5_V_1)
	
// 	$display("%1t pc %1d", $time, the_dut.bevelab10nz);
//	$display("pc %1d", the_dut.bevelab10nz);	
	end


endmodule
// eof
