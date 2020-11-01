//
// HPR L/S 
// Temporary heap space manager
// hpr_alloc accepts an address space number or name.  This is mapped to the structural instance that manages that space. The structural components do not have a space argument.
module HPR_HEAPMANGER_T0(
		       input 		 clk,
		       input 		 reset,
		       output reg [63:0] RESULT,
		       input [63:0] 	 SIZE_IN_BYTES,
		       input 		 REQ,
		       output reg 	 ACK,
		       output reg 	 FAIL);

   reg [32:0] 			     base;
   always @(posedge clk) if (reset) begin
      ACK <= 0;
      FAIL <= 0;
      base <= 4096; // Do not start at zero, since null must be reserved.
      RESULT <= 1;
   end
   else begin
      ACK <= REQ;
      if (REQ) begin
	 RESULT <= base;
	 $display("%1t, %m: Alloc at %1d at 0x%1h", $time, SIZE_IN_BYTES, base);
	 base <= base + ((SIZE_IN_BYTES+7)/8)*8;
      end
   end

endmodule

// eof

