// 
// Kiwi Scientific Acceleration
// (C) 2010-15 - DJ Greaves - University of Cambridge Computer Laboratory

//
//  Neutral simulation substrate.
//
// A test wrapper for designs with one a wide DRAM-style off-chip port but which still uses the HFAST protocol and a cache.
// Bytelanes are used and back-to-back cycles are supported.


// Note that we no longer use this style very much - instead our load/store ports are wired via protocol adapators to AXI memory by HPR System Integrator.

`timescale 1ns/10ps
`define half_period 5
`define timelimit 100 * 2 *1000 * 1000
module SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 1; # 42 reset = 0; end
   always #`half_period clk = !clk;

   parameter dram_dwidth = 256;          // 32 byte DRAM burst size or cache line.
   parameter bits_per_lane = 8;
   parameter noLanes = dram_dwidth / bits_per_lane; // Bytelanes.
     
   wire [noLanes-1:0] bs_r0bank_lanes;
   wire [21:0] 	       bs_r0bank_addr;
   wire bs_r0bank_oprdy, bs_r0bank_opreq, bs_r0bank_ack, bs_r0bank_rwbar;
   wire [dram_dwidth-1:0]  bs_r0bank_rdata, bs_r0bank_wdata;

   wire [noLanes-1:0] fs_r0bank_lanes;
   wire [21:0] 	       fs_r0bank_addr;
   wire fs_r0bank_oprdy, fs_r0bank_opreq, fs_r0bank_ack, fs_r0bank_rwbar;
   wire [dram_dwidth-1:0]  fs_r0bank_rdata, fs_r0bank_wdata;
   wire 		   finished;
   wire [31:0] 		   elimit, evariant, edesign;

   wire [639:0] 	   KppWaypoint0, KppWaypoint1;
   
   DUT dut(.clk(clk), .reset(reset),
	   .finished(finished),
	   .elimit(elimit),	   
	   .evariant(evariant),	   
	   .edesign(edesign),	   
//	   .KppWaypoint0(KppWaypoint0),	   .KppWaypoint1(KppWaypoint1),
	   .hf1_dram0bank_lanes(fs_r0bank_lanes),
	   .hf1_dram0bank_oprdy(fs_r0bank_oprdy), 
	   .hf1_dram0bank_opreq(fs_r0bank_opreq),
	   .hf1_dram0bank_ack(fs_r0bank_ack),       
	   .hf1_dram0bank_rwbar(fs_r0bank_rwbar),   .hf1_dram0bank_rdata(fs_r0bank_rdata),
	   .hf1_dram0bank_addr(fs_r0bank_addr),     .hf1_dram0bank_wdata(fs_r0bank_wdata)   
   );

// 700 *1000 *1000 * 1000
   initial begin #( `timelimit ) $display ("vsys.v CBG timeout and finish at %t", $time); $finish(); end


   // The front and back side of the cache both use hfast protocol so the cache can be easily 'commented out'
   // The high-level DRAM model has the same signature as the DDR2 DRAM controller so it can be used instead on the cache backside or even directly connected to the Kiwi-generated code.

   KIWI_DDR2_CONTROLLER controller_sans_cache(.clk(clk), .reset(reset), 
				   .ctr_rwbar(fs_r0bank_rwbar), .ctr_rdata(fs_r0bank_rdata), .ctr_wdata(fs_r0bank_wdata), .ctr_wordAddr(fs_r0bank_addr), 
				   .ctr_oprdy(fs_r0bank_oprdy), .ctr_opreq(fs_r0bank_opreq), .ctr_ack(fs_r0bank_ack), .ctr_lanes(fs_r0bank_lanes));

   initial #4 controller_sans_cache.traceLevel = 1;
   initial #4 controller_sans_cache.ddr.traceLevel = 2;

   
`ifdef NOTDEF 
   cache256_hf1 cache0(clk, reset,
 				   fs_r0bank_rwbar, fs_r0bank_rdata, fs_r0bank_wdata, fs_r0bank_addr, 
				   fs_r0bank_oprdy, fs_r0bank_opreq, fs_r0bank_ack, fs_r0bank_lanes,

   				   bs_r0bank_rwbar, bs_r0bank_rdata, bs_r0bank_wdata, bs_r0bank_addr, 
				   bs_r0bank_oprdy, bs_r0bank_opreq, bs_r0bank_ack, bs_r0bank_lanes);

   membank256_hf1 drambank0_hfast1(.clk(clk), .reset(reset), 
				   .ctr_rwbar(bs_r0bank_rwbar), .ctr_rdata(bs_r0bank_rdata), .ctr_wdata(bs_r0bank_wdata), .ctr_wordAddr(bs_r0bank_addr), 
				   .ctr_oprdy(bs_r0bank_oprdy), .ctr_opreq(bs_r0bank_opreq), .ctr_ack(bs_r0bank_ack), .ctr_lanes(bs_r0bank_lanes));
`endif
      initial begin $dumpfile("vcd.vcd"); $dumpvars(); end
   
`include "kpp_testbench_mon_onethread.v"


   
endmodule

// eof

