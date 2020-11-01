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


module VERI_SIMSYS(input clk, input reset);

   integer counter;
   integer old_counter;
   reg [639:0] old_KppWaypoint;
   wire [639:0] KppWaypoint0, KppWaypoint1;   
   wire [7:0] 	hpr_abend_syndrome;
   wire 	done = (hpr_abend_syndrome != 255);
   wire [7:0] 	hpr_unary_leds;   
 
  //wire [31:0] 	thread0_pc = { 31'd0, the_dut.bevelab10nz }; // the_dut.xpc10nz
   wire [31:0] ksubsManualWaypoint;
//   initial begin $dumpfile("top.vcd"); $dumpvars(1, the_dut.xpc10nz, old_KppWaypoint, KppWaypoint0, reset, done); end
   wire [31:0] cuckoohashdemo10PC10nz_pc_export;
   wire [31:0] thread0_pc = cuckoohashdemo10PC10nz_pc_export;
   
   DUT the_dut(.clk(clk), 
	       .reset(reset),
	       //.KppWaypoint0(KppWaypoint0),
	       //.KppWaypoint1(KppWaypoint1),
//	       .ksubsManualWaypoint(ksubsManualWaypoint),
//	       .hpr_unary_leds(hpr_unary_leds),
//	       .cuckoohashdemo10PC10nz_pc_export(cuckoohashdemo10PC10nz_pc_export),
	       .hpr_abend_syndrome(hpr_abend_syndrome)
	       );


     always @(posedge clk) begin
	if (reset) begin
	  counter <=0;
	end
	if (done && !reset) begin
	   $display("Kiwi done at time %1t.  %1d cycles", $time, counter);
	   $finish;
	end
	
	counter <= counter + 1;
	if (old_KppWaypoint === 640'bx || old_KppWaypoint !=  KppWaypoint0) begin
	   old_KppWaypoint <=  KppWaypoint0;
	   old_counter <= counter;
	   if (KppWaypoint0) $display("%1t, delta=%1d pc=%1d, new waypoint -->%1s", $time, counter-old_counter, thread0_pc, KppWaypoint0);
	   end
	//if ((counter & 32'hFFFF)==0) $display("%1t, pc, thread_pc10=%1d:  v3=0x%1h", $time, the_dut.xpc10nz, 3);
	end

endmodule
// eof
