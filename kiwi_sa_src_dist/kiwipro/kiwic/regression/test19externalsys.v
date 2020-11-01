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

   // abstractionName=res2-contacts pi_name=the_Test19_Server1
  wire the_Test19_Server1_start_ack;
  wire the_Test19_Server1_start_req;
  wire the_Test19_Server1_get_id_ack;
  wire the_Test19_Server1_get_id_req;
  wire [15:0] the_Test19_Server1_get_id_return;
  wire the_Test19_Server1_setget_pixel_ack;
  wire the_Test19_Server1_setget_pixel_req;
  wire [7:0] the_Test19_Server1_setget_pixel_return;
  wire [31:0] the_Test19_Server1_setget_pixel_axx;
  wire [31:0] the_Test19_Server1_setget_pixel_ayy;
  wire the_Test19_Server1_setget_pixel_readf;
  wire [7:0] the_Test19_Server1_setget_pixel_wdata;

   client19_external theclient(  
				 .reset(reset),   
				 .clk(clk),
				 .Test19_Server1_start_ack(the_Test19_Server1_start_ack),
				 .Test19_Server1_start_req(the_Test19_Server1_start_req),
				 .Test19_Server1_get_id_ack(the_Test19_Server1_get_id_ack),
				 .Test19_Server1_get_id_req(the_Test19_Server1_get_id_req),
				 .Test19_Server1_get_id_return(the_Test19_Server1_get_id_return),
				 .Test19_Server1_setget_pixel_ack(the_Test19_Server1_setget_pixel_ack),
				 .Test19_Server1_setget_pixel_req(the_Test19_Server1_setget_pixel_req),
				 .Test19_Server1_setget_pixel_return(the_Test19_Server1_setget_pixel_return),
				 .Test19_Server1_setget_pixel_axx(the_Test19_Server1_setget_pixel_axx),
				 .Test19_Server1_setget_pixel_ayy(the_Test19_Server1_setget_pixel_ayy),
				 .Test19_Server1_setget_pixel_readf(the_Test19_Server1_setget_pixel_readf),
				 .Test19_Server1_setget_pixel_wdata(the_Test19_Server1_setget_pixel_wdata));



				  
  Test19_Server1 the_Test19_Server1(
				    .clk(clk),
				    .reset(reset),
				    .start_ack(the_Test19_Server1_start_ack),
				    .start_req(the_Test19_Server1_start_req),
				    .get_id_ack(the_Test19_Server1_get_id_ack),
				    .get_id_req(the_Test19_Server1_get_id_req),
				    .get_id_return(the_Test19_Server1_get_id_return),
				    .setget_pixel_ack(the_Test19_Server1_setget_pixel_ack),
				    .setget_pixel_req(the_Test19_Server1_setget_pixel_req),
				    .setget_pixel_return(the_Test19_Server1_setget_pixel_return),
				    .setget_pixel_axx(the_Test19_Server1_setget_pixel_axx),
				    .setget_pixel_ayy(the_Test19_Server1_setget_pixel_ayy),
				    .setget_pixel_readf(the_Test19_Server1_setget_pixel_readf),
				    .setget_pixel_wdata(the_Test19_Server1_setget_pixel_wdata));
		       

endmodule
// eof
