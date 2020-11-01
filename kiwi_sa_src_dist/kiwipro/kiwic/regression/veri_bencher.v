// Kiwi Scientific Accleration
// Simple Verilator Testbench
//
// 
//
module VERI_BENCHER(
		    input 	  clk,
		    input 	  reset,
		    output [31:0] mon0,
		    output [31:0] mon1,
		    input [2:0] din,
		    output finish
	       );

   reg 			   start;
   
   wire [7:0] 		   hpr_abend_syndrome;
   wire 		   finish = (hpr_abend_syndrome != 255);
   
   always @(posedge clk) begin
      //1$display("running finish=%1h %1h", finish, dout);
      if (!reset && finish) begin
	 $display("Finished at time %1t. Code 0x02X", $time, hpr_abend_syndrome);
	 $finish;
      end
   end
   
   DUT the_dut(
	       .clk(clk),
	       .reset(reset),
	       .hpr_abend_syndrome(hpr_abend_syndrome)
	       );



//   assign mon0 = { dout[47:32], 1'b0, start, finish, reset };
//   assign mon1 = dout[31:0];
   
endmodule

// eof
