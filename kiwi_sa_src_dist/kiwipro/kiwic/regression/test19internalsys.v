// $Id: test19sys.v,v 1.2 2011/03/01 23:47:19 djg11 Exp $
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// (C) 2011-16 DJ Greaves
//
// Test wrapper for the RPC demo. Internally-instantiated version. This is no different from normal vsys.v.
//

// Using a synthesised $finish rather than a return code to the substrate.

module ACME_SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 33 reset = 0; end
   always #5 clk = !clk;

   initial # 1000000 $finish();

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

  client19_internal theclient(  
      .reset(reset),   
      .clk(clk)   
		       
		       );
endmodule
// eof
