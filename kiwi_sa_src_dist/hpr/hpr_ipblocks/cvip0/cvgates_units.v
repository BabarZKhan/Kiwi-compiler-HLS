//
// HPR L/S cvgates: Some minimal unit tests
//
module vsys();

         initial begin # (100 * 1000 * 1000) $display("Finish HDL simulation on timeout %t.", $time); $finish(); end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end
   
   reg clk, reset;
   initial clk = 0;
   initial reset = 1;

   initial forever #5 clk = !clk;
   initial #33 reset = 0;

   monadic_vsys the_dut (clk, reset);
//   diadic_vsys the_dut (clk, reset);

endmodule





module monadic_vsys(input clk, input reset);

	 
   reg [31:0] count;
   reg [14:0] prbs0, prbs1, prbs2, prbs3;
   wire       advancer = 1;
   always @(posedge clk) if (reset) begin
      prbs0 <= 12345567;
      prbs1 <= 1;
      prbs2 <= 44;
      prbs3 <= 1;
      count <= 0;
   end
   else if(advancer) begin
      count <= count + 1;
      prbs0 <= (prbs0 << 1) | (prbs0[14] ^ prbs0[13]);
      prbs1 <= ((prbs1 << 1) | (prbs1[14] ^ prbs1[13])) ^ prbs0;
      prbs2 <= ((prbs2 << 1) | (prbs2[14] ^ prbs2[13])) ^ prbs1;
      prbs3 <= ((prbs3 << 1) | (prbs3[14] ^ prbs3[13])) ^ prbs2;
   end

   wire [63:0] arg64 = 
	       (count < 3) ? 64'h3ff0_1000_0000_0000:
	       (count < 4) ? 64'h4040_0000_0000_0000:
	       (count < 5) ? 64'hC040_0000_0000_0000:
	       (count < 6) ? 64'h41d2_6580_b480_0000: // 1234567890
	       (count < 6) ? 0: 
	       { 5{prbs0}};
   wire signed [31:0] res32;
   wire        fail;
   reg [63:0]  a0, a1;

   always @(posedge clk) begin
      a0 <= arg64;
      a1 <= a0;
      $display("Test i32f64   arg=%h arg=%f  ->  res=%d     fail=%1d", a1,  $bitstoreal(a1), res32, fail);
   end

   CV_FP_CVT_FL2_I32_F64 dut(.clk(clk), .result(res32), .arg(arg64), .fail(fail));
   
endmodule // monadic_vsys




   
module diadic_vsys(input thunker_);

   initial begin # (100 * 1000 * 1000) $display("Finish HDL simulation on timeout %t.", $time); $finish(); end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end
   
   reg clk, reset;
   initial clk = 0;
   initial reset = 1;

   initial forever #5 clk = !clk;
   initial #33 reset = 0;
			 

   reg [14:0] prbs0, prbs1, prbs2, prbs3, advancer;
   always @(posedge clk) if (reset) begin
      prbs0 <= 12345567;
      prbs1 <= 1;
      prbs2 <= 44;
      prbs3 <= 1;
   end
   else if(advancer) begin
      prbs0 <= (prbs0 << 1) | (prbs0[14] ^ prbs0[13]);
      prbs1 <= ((prbs1 << 1) | (prbs1[14] ^ prbs1[13])) ^ prbs0;
      prbs2 <= ((prbs2 << 1) | (prbs2[14] ^ prbs2[13])) ^ prbs1;
      prbs3 <= ((prbs3 << 1) | (prbs3[14] ^ prbs3[13])) ^ prbs2;
   end


   wire signed [31:0] op0 = (prbs2[7] ? 0 : (prbs0 << 17 )) ^ (prbs1);
   wire signed [31:0] op1 = (prbs0[6] ? 0: (prbs3 << 16 )) ^ (prbs2);   
//   wire signed [31:0] op0 = 1000000000;
//   wire signed [31:0] op1 = 3;
		
   wire       div_rdy;
   reg 	      div_req, div_busy;

   wire [31:0] div_result;
   reg [31:0] div_nominal;   
   integer    fails;
   integer    tests;
   always @(posedge clk) if (reset) begin
      advancer <= 0;
      div_busy <= 0;
      fails = 0;
      div_req <= 0;
      tests = 100;
   end
   else begin

      if (div_req) begin
	 div_nominal <= op0/op1;
	 $display("%1t: Divide %1d / %1d", $time, op0, op1);
      end
      
      div_req <= 0;
      
      
      if (!div_busy) begin
	 if (tests == 0) begin
	    $display("Finish. Time is %t. Number of fails is %1d", $time, fails);
	    $finish;
	    end
	 div_busy <= 1;
	 div_req <= 1;
	 advancer <= 1;
	 tests = tests - 1;
      end

      if (div_rdy) begin
	 div_busy <= 0;
	 if (div_nominal != div_result) begin
	    $write ("FAIL: ");
	    fails = fails + 1;
	    end
	 $display("%1t: divider expect=0x%1h cf achieved=0x%1h", $time, div_nominal, div_result);
      end
   end
   
   //CV_INT_VL_DIVIDER_S dut
   CV_INT_FL_DIVIDER_US dut     
     (
      .clk(clk),
      .reset(reset),
      .req(div_req),
      .rdy(div_rdy),
      .RR(div_result),
      .NN(op0),
      .DD(op1));



endmodule

