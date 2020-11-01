// Kiwi Scientific Acceleration
//
// A test wrapper for designs with one a wide DRAM-style off-chip port but which still uses the low-performance HSIMPLE protocol.
// Bytelanes are used.

module hsimple1();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 400 reset = 0; end
   always #100 clk = !clk;

   parameter dram_dwidth = 256;          // 32 byte DRAM burst size or cache line.
   parameter no_lanes = dram_dwidth / 8; // Bytelanes.
   
   wire [63:0] outerv;
   wire [no_lanes-1:0] eram0bank_lanes;
   wire [21:0] 	  eram0bank_addr;

   wire eram0bank_req, eram0bank_ack, eram0bank_rwbar;
   wire [dram_dwidth-1:0]  eram0bank_rdata, eram0bank_wdata;

   DUT dut(.clk(clk), .reset(reset),
     .hs_dram0bank_LANES(eram0bank_lanes),
     .hs_dram0bank_REQ(eram0bank_req),       .hs_dram0bank_ACK(eram0bank_ack),       
     .hs_dram0bank_RWBAR(eram0bank_rwbar),   .hs_dram0bank_RDATA(eram0bank_rdata),
     .hs_dram0bank_ADDR(eram0bank_addr),     .hs_dram0bank_WDATA(eram0bank_wdata)	   
   );


   initial begin #( 700 *1000 * 1000) $display ("vsys cbg timeout and finish"); $finish(); end

   membank256 drambank0(clk, reset, eram0bank_rwbar, eram0bank_rdata, eram0bank_wdata, eram0bank_addr, eram0bank_ack, eram0bank_req, eram0bank_lanes);

  initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

endmodule

// Membank. - see newer versions in the support folder and Kiwi substrate.
// Original hsimple memory bank (ie without Xilinx or other DRAM controller)
// this would be mapped to block rams by the Xilinx tools if included
// on the FPGA.  Otherwise, it is just used as a simulation reference model.
module membank256(input clk, input reset, input rwbar, 
  output [255:0] rdata,
  input [255:0] wdata,		  
  input [21:0] wordAddr, 
  output reg 	ack,
  input 	req, 
  input [31:0] 	lanes32);

  parameter memsize = 100 * 1000 * 8; // lanes
  parameter laneSize = 8;
  parameter lanes_per_word = 32;

  reg [laneSize-1:0] data [0:(memsize-1)];

  wire [31:0] byte_addr = wordAddr * lanes_per_word;
   reg [255:0] rans, wd;
   integer wvv,   rvv;

   always @(posedge clk) $display(" req=%d ack=%d rwbar=%d  pc=%d" , req, ack, rwbar, dut.xpc10);

   always @(posedge clk) begin
      if (req && rwbar & !ack) begin
	 rvv = 31;
	 rans = 0;
	 while (rvv >= 0) begin
	    rans = (rans << laneSize) | ((byte_addr+rvv < memsize) ? data[byte_addr+rvv]:32'bx);
	    //$display("%m: lane read req=%d wordAddr=%d addr=%d laneData=%h rwbar=%d rd=$%x", req, wordAddr, byte_addr+rvv, data[byte_addr+rvv], rwbar, rans);
	    rvv = rvv - 1;

	 end
	 $display("%t, %m: read-word req=%d wordAddr=%d rwbar=%d rd=$%x", $time, req, wordAddr, rwbar, rans);
      end
   end
   assign rdata = rans;

  always @(posedge clk) begin
     //$display("%m clk : req=%d addr=%d rwbar=%d", req, wordAddr, rwbar);
     ack <= !reset && req;

     if (req && !ack && !rwbar) begin
	wvv = 0;
	wd = wdata;
	$display("%m write-word lanes=$%x [$%x] := $%x", lanes32, byte_addr + wvv, wd);	
	while (wvv < lanes_per_word) begin
	   if (byte_addr+wvv < memsize && lanes32[wvv]) begin
	      //$display("%m write byte [%d]=%d   q=%h" , byte_addr + wvv, wd & 8'hFF, wd);
	      data[byte_addr+wvv] <= wd & 8'hFF;
	   end
	   wd = wd >> 8;
	   wvv = wvv + 1;
	end
     end
     //else if (req && !ack && !rwbar)  $display("%m write out of range %d", addr);

     //if (!req && ack && rwbar)  $display("%m read [%d] gives %d ", addr, rdata);
  end


endmodule

// Membank. - see newer versions in the support folder and Kiwi substrate.
// Original hsimple memory bank (ie without Xilinx or other DRAM controller)
// this would be mapped to block rams by the Xilinx tools if included
// on the FPGA.  Otherwise, it is just used as a simulation reference model.
module membank32(clk, reset, rwbar, rdata, wdata, addr, ack, req);
  input clk, reset, req, rwbar;
  output ack;
  input [31:0] addr, wdata;
  output [31:0] rdata;
  reg ack;

  parameter memsize = 200000;

  reg [31:0] data [0:(memsize-1)];


  assign rdata = (addr < memsize) ? data[addr]:32'bx;

  always @(posedge clk) begin
     $display("%m clk : req=%d addr=%d rwbar=%d", req, addr, rwbar);
     ack <= !reset && req;
     if (req && !ack && !rwbar && addr < memsize) 
       begin
	  //$display("%m write [%d]=%d", addr, wdata);
	  data[addr] <= wdata;
       end
     else if (req && !ack && !rwbar)  $display("%m write out of range %d", addr);
     
     //if (!req && ack && rwbar)  $display("%m read [%d] gives %d ", addr, rdata);
  end


endmodule
// eof

