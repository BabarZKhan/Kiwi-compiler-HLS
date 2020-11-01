//
// Testbench
//
module PARPSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 54 reset = 0; end
   always #10 clk = !clk;
   initial #20000 $finish;

   wire [8:0] dout;  // Bit8 is a parity bit for the byte.
   wire strobe;
   reg 	ack0, ack1;
   DUT dut(.clk(clk), 
	   .reset(reset), 
	   .ack(ack), 
	   .parallelPort_dout(dout), 
	   .strobe(strobe));

   always @(posedge clk) begin
      ack1 <= !reset & ack0;
      ack0 <= !reset & strobe;
   // Feed ack from strobe after a delay.
   end

   assign ack = ack1;

   wire parity_ok = (^(dout)); // Odd parity - means value 000 is not allowed.
   always @(posedge strobe) begin
         $display("+ve Parallel Print Char '%x'", dout[7:0]);
         if (!parity_ok) $display("+ve Parallel Print Char Parity Error in '%x'", dout[8:0]);      
      end
   always @(negedge strobe) $display("-ve Parallel Print Char '%x'", dout[7:0]);   
// Do this in your test bench

   initial
     begin
	$dumpfile("vcd.vcd");
	$dumpvars();
     end


//   reg[18*8-1:0] hws = "Hello World\n";
//   initial if (1) begin
//      $display("Second char of string is %h", hws[1*8 +: 8]);
//      end
endmodule

// eof
