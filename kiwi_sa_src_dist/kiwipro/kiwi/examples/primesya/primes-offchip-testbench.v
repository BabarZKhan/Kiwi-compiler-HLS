`timescale 1ns/10ps
// Kiwi Scientific Acceleration
//
// A test wrapper for designs with one a wide DRAM-style off-chip port which uses the HFAST protocol.
// Bytelanes are used and back-to-back cycles are supported.

module ACME_SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 43 reset = 0; end
   always #5 clk = !clk;

   wire [7:0]  ksubsAbendSyndrome;
   wire        finished = (ksubsAbendSyndrome != 0) && (ksubsAbendSyndrome != 255);

   
   parameter dram_dwidth = 256;          // 32 byte DRAM burst size or cache line.
   parameter no_lanes = dram_dwidth / 8; // Bytelanes.
   
   wire [63:0] outerv;
   wire [no_lanes-1:0] eram0bank_lanes;
   wire [21:0] 	  eram0bank_addr;

   wire eram0bank_oprdy, eram0bank_opreq, eram0bank_ack, eram0bank_rwbar;
   wire [dram_dwidth-1:0]  eram0bank_rdata, eram0bank_wdata;

   DUT dut(.clk(clk), .reset(reset),
	   //.hpr_ext_run_enable(1),
	   .hpr_abend_syndrome(ksubsAbendSyndrome),
	   .bondout1_LANES1(eram0bank_lanes),
	   .bondout1_REQRDY1(eram0bank_oprdy), 
	   .bondout1_REQ1(eram0bank_opreq),
	   .bondout1_ACK1(eram0bank_ack),       
	   .bondout1_RWBAR1(eram0bank_rwbar),   .bondout1_RDATA1(eram0bank_rdata),
	   .bondout1_ADDR1(eram0bank_addr),     .bondout1_WDATA1(eram0bank_wdata)	   
   );

   initial begin #(1000 * 32) $display ("vsys testbench: CBG TIMEOUT AND FINISH"); $finish(); end

   membank256_hf1 drambank0_hfast1(clk, reset, 
    eram0bank_rwbar, eram0bank_rdata,
    eram0bank_wdata, eram0bank_addr, 
    eram0bank_oprdy, eram0bank_opreq, 
    eram0bank_ack, eram0bank_lanes);

  initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   
  always @(posedge clk) begin
      if (finished) begin
	 $display("Exit on finished syndrome 0x%02h reported after %d clocks.", ksubsAbendSyndrome, $time/10);

	 #5000  $finish;
	 end
     //$display("%1t,  pc=%1d, codesent=%h" , $time, the_dut.xpc10nz, codesent);
  end

endmodule




module membank256_hf1
  (input clk, 
   input 		      reset, 
   input 		      rwbar, 
   output [lanes_per_word*laneSize-1:0] 	      rdata,
   input [lanes_per_word*laneSize-1:0] 	      wdata, 
   input [21:0] 	      wordAddr, 
   output reg 		      oprdy, // ready for new request   - a new cmd is presented for every clock cycle where rdy and req both hold.
   input 		      opreq, // new op request          -
   output reg 		      ack, // output from last cycle is available.
   input [lanes_per_word-1:0] lanes
   );

   parameter lanes_per_word=32;
   parameter laneSize=8;
   parameter memsize = 100 * 1000 * 8; 
   
   reg [laneSize-1:0] 		     data [0:(memsize-1)];
   
   wire [31:0] 			     byte_addr = wordAddr * lanes_per_word;
   reg [lanes_per_word*laneSize-1:0] rans, wd;
   integer     wvv,   rvv;
   wire   start = opreq & oprdy;

   //always @(posedge clk) $display(" HFAST1 req=%d ack=%d rwbar=%d  pc=%d" , start, ack, rwbar, dut.xpc10);
   
   always @(posedge clk) begin
      if (start & rwbar) begin
	 rvv = lanes_per_word - 1;
	 rans = 0;
	 while (rvv >= 0) begin
	    rans = (rans << laneSize) | ((byte_addr+rvv < memsize) ? data[byte_addr+rvv]:32'bx);
	    //$display("%m: lane read req=%d wordAddr=%d addr=%d laneData=%h rwbar=%d rd=$%x", req, wordAddr, byte_addr+rvv, data[byte_addr+rvv], rwbar, rans);
	    rvv = rvv - 1;

	 end
	 $display("%t, %m: read-word start=%d wordAddr=%d rwbar=%d rd=$%x", $time, start, wordAddr, rwbar, rans);
      end
   end
   assign rdata = rans;

   always @(posedge clk) begin
     //$display("%m clk : req=%d addr=%d rwbar=%d", start, wordAddr, rwbar);

      if (0) begin
	 ack <= opreq; /// some pseudo stalling 
	 oprdy <= !opreq;
      end
      else begin
	 ack <= 1;
	 oprdy <= !reset;
      end
      
     if (opreq && oprdy && !rwbar) begin
	wvv = 0;
	wd = wdata;
	$display("%t, %m: write-word lanes=$%x [$%x] := $%x", $time, lanes, byte_addr + wvv, wd);	
	while (wvv < lanes_per_word) begin
	   if (byte_addr+wvv < memsize && lanes[wvv]) begin
	      //$display("%m write byte [%d]=%d   q=%h" , byte_addr + wvv, wd & 8'hFF, wd);
	      data[byte_addr+wvv] <= wd & 8'hFF;
	   end
	   wd = wd >> 8;
	   wvv = wvv + 1;
	end
     end
     //else if (req && !ack && !rwbar)  $display("%m write out of range %d", addr);

  end


endmodule
// eof

