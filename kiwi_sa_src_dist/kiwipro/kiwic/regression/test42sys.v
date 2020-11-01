//
// Kiwi Scientific Acceleration
// vsys.v - A test wrapper for offchip programmed-I/O and substrate tests.
// (C) 2016 DJ Greaves, University of Cambridge.
// 
//
//
`timescale 1ns/10ps


// Under construction: this uses low performance HSIMPLE I/O protocol and does not report errors yet!


module SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 400 reset = 0; end
   always #100 clk = !clk;

   reg [31:0] pioRegfileRead_addr;
   wire [31:0] pioRegfileRead_return;
   reg 	       pioRegfileRead_req;
   wire        pioRegfileRead_ack;
	       
   reg [31:0]  pioRegfileWrite_data;
   reg [31:0]  pioRegfileWrite_addr;
   reg 	       pioRegfileWrite_req;
   wire        pioRegfileWrite_ack;

   TEST42SLAVE the_dut(.clk(clk), .reset(reset),
	       
	       .test42_pioRegfileRead_addr(pioRegfileRead_addr),
	       .test42_pioRegfileRead_return(pioRegfileRead_return),
	       .test42_pioRegfileRead_req(pioRegfileRead_req),
	       .test42_pioRegfileRead_ack(pioRegfileRead_ack),
	       .test42_pioRegfileWrite_data(pioRegfileWrite_data),
	       .test42_pioRegfileWrite_addr(pioRegfileWrite_addr),
	       .test42_pioRegfileWrite_req(pioRegfileWrite_req),
	       .test42_pioRegfileWrite_ack(pioRegfileWrite_ack)
	       );
	       
   wire [31:0] count, vol;
   wire finished;  
   reg [31:0] counter;


   initial begin # (100 * 1000 * 1000) $display("Finish HDL simulation on timeout %t.", $time); $finish(); end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   

     always @(posedge clk) if (reset) begin
	counter <= 0;
	pioRegfileRead_addr <= 0;
	pioRegfileWrite_req <= 0;	
	pioRegfileRead_req <= 0;
     end
     else begin

	if (pioRegfileWrite_ack) pioRegfileWrite_req <= 0;	
	if (pioRegfileRead_ack) pioRegfileRead_req <= 0;


	if (pioRegfileRead_ack) $display("Read back 0x%1h from address %1d", pioRegfileRead_return, pioRegfileRead_addr);
	
	counter <= counter + 1;

	if (counter == 10) begin
	   pioRegfileWrite_req <= 1;
	   pioRegfileWrite_addr <= 8;
	   pioRegfileWrite_data <= 32'hdeadbeef;	   	   
	end

	if (counter == 20) begin
	   pioRegfileWrite_req <= 1;
	   pioRegfileWrite_addr <= 16;
	   pioRegfileWrite_data <= 32'h12345678;	   	   
	end

	if (counter == 30) begin
	   pioRegfileRead_req <= 1;
	   pioRegfileRead_addr <= 8;
	end

	if (counter == 40) begin
	   pioRegfileRead_req <= 1;
	   pioRegfileRead_addr <= 16;
	end

	if (counter == 50) begin
	   pioRegfileRead_req <= 1;
	   pioRegfileRead_addr <= 8;
	end

	
//	$display("pc, thread0 = %d", the_dut.xpc10nz);
	end

endmodule
// eof
