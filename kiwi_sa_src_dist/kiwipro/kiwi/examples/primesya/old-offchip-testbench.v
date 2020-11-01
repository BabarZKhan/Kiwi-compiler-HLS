// $Id: primes-offchip-testbench.v,v 1.3 2011/06/08 17:45:10 djg11 Exp $
//
// A test wrapper for designs with one hsimple off-chip port.
// No lanes within the word are used here.

module hsimple1();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 400 reset = 0; end
   always #100 clk = !clk;

   wire [63:0] outerv;

   wire portx_req, portx_ack, portx_rwbar;
   wire [31:0] portx_addr, portx_rdata, portx_wdata;

   wire drambank0_req, drambank0_ack, drambank0_rwbar;
   wire [31:0] drambank0_addr, drambank0_rdata, drambank0_wdata;

   DUT dut(.clk(clk), .reset(reset),

//      .portx_req(portx_req),       .portx_ack(portx_ack),       .portx_rwbar(portx_rwbar),       .portx_rdata(portx_rdata),       .portx_addr(portx_addr),       .portx_wdata(portx_wdata),

     .dram0bank_req(drambank0_req),       .dram0bank_ack(drambank0_ack),       .dram0bank_rwbar(drambank0_rwbar),       .dram0bank_rdata(drambank0_rdata),       .dram0bank_addr(drambank0_addr),       .dram0bank_wdata(drambank0_wdata)	   

   );


   initial begin #(2700 * 1000 * 1000) $display ("old-offchip-testbench: CBG timeout and finish at %t", $time); $finish(); end

//   membank portx(clk, reset, portx_rwbar, portx_rdata, portx_wdata, portx_addr, portx_ack, portx_req);

   membank drambank0(clk, reset, drambank0_rwbar, drambank0_rdata, drambank0_wdata, drambank0_addr, drambank0_ack, drambank0_req);

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
//  	 	   $display("req=%d addr=%d rwbar=%d", req, addr, rwbar);
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
