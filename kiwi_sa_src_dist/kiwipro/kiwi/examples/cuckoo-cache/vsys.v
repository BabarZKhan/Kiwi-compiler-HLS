//
// Kiwi Scientific Acceleration  
// University of Cambridge, Computer Laboratory.
//
// vsys.v - A test wrapper for simulating very simple tests with clock and reset.
// (C) 2010-16 DJ Greaves, University of Cambridge.
// 
//
//
`timescale 1ns/1ns

module SIMSYS();

   integer counter;
   integer old_counter;
   reg [639:0] old_KppWaypoint;
   wire [639:0] KppWaypoint0, KppWaypoint1;   

   wire 	done;
   reg 		clk, reset;
   initial begin counter =  0; reset = 1; clk = 1; # 43 reset = 0; end
   always #5 clk = !clk;   
   
   wire [7:0]  hpr_abend_syndrome;
   wire        finished = (hpr_abend_syndrome != 255);
 
   initial # (1000 * 1000 * 1000) begin
      old_KppWaypoint = 0;
      $display("Finish HDL simulation on timeout %t.", $time); 
      $finish(); 
      end

//   initial begin $dumpfile("top.vcd"); $dumpvars(1, the_dut.xpc10nz, old_KppWaypoint, KppWaypoint0, reset, done); end

   DUT the_dut(.clk(clk), 
	       .reset(reset),
	       .hpr_abend_syndrome(hpr_abend_syndrome),
	       .KppWaypoint0(KppWaypoint0),
	       .KppWaypoint1(KppWaypoint1)
	       );


     always @(posedge clk) begin
	if (finished) begin
	   $display("RTLSIM: Exit on finished asserted after %t cycles.", $time/10 - 3); // Adjust for clock freq and reset duration in the cycles printed.
	   $display("Kiwi done at time %1t.  %1d cycles", $time, counter);
	   #5 $finish;
	end
	
	counter <= counter + 1;
	if (old_KppWaypoint === 640'bx || old_KppWaypoint !=  KppWaypoint0) begin
	   old_KppWaypoint <=  KppWaypoint0;
	   old_counter <= counter;
	   if (KppWaypoint0) $display("%1t, delta=%1d pc=%1d, new waypoint -->%1s", $time, counter-old_counter, the_dut.xpc10nz, KppWaypoint0);
	   end
	//if ((counter & 32'hFFFF)==0) $display("%1t, pc, thread_pc10=%1d:  v3=0x%1h", $time, the_dut.xpc10nz, 3);
	end

endmodule
// eof
