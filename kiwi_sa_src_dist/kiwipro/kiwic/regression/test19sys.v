// $Id: test19sys.v,v 1.2 2011/03/01 23:47:19 djg11 Exp $
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// (C) 2011-16 DJ Greaves
//
// Test wrapper for the RPC demo.  Currently Kiwi does not automate the generation of these boring wrappers.
//

module SIMSYS();
   reg clk, reset;
   initial begin reset = 1; clk = 0; # 400 reset = 0; end
   always #100 clk = !clk;


   wire [31:0] count, vol;
   wire finished;  
   reg start; 

   wire [31:0] test19_server1_setget_pixel_x;
   wire [31:0] test19_server1_setget_pixel_y;
   wire        test19_server1_setget_pixel_readf;
   wire [7:0]  test19_server1_setget_pixel_wdata;
   wire        test19_server1_setget_pixel_ack;
   wire        test19_server1_setget_pixel_req;
   wire [7:0]  test19_server1_setget_pixel_return;
   wire test19_server1_start_ack;   
   wire test19_server1_start_req;    
   
   wire test19_server1_get_id_ack;  
   wire test19_server1_get_id_req;    
   wire [31:0] test19_server1_get_id_return;

   
  always @(posedge clk)

    if (0) 
     $display("rst=%d x=%d y=%d readf=%d wdata=%d ack=%d req=%d",
         reset,
	 test19_server1_setget_pixel_x,  
	 test19_server1_setget_pixel_y,
	 test19_server1_setget_pixel_readf,
	 test19_server1_setget_pixel_wdata,
	 test19_server1_setget_pixel_ack,
	 test19_server1_setget_pixel_req);



  client19 theclient(  
      .reset(reset),   
      .clk(clk),   

      .Test19_Server1_start_ack(test19_server1_start_ack),   
      .Test19_Server1_start_req(test19_server1_start_req),    

      .Test19_Server1_get_id_ack(test19_server1_get_id_ack),   
      .Test19_Server1_get_id_req(test19_server1_get_id_req),    
      .Test19_Server1_get_id_return(test19_server1_get_id_return),


		       

		       
      .Test19_Server1_setget_pixel_wdata(test19_server1_setget_pixel_wdata),   
      .Test19_Server1_setget_pixel_readf(test19_server1_setget_pixel_readf),   
      .Test19_Server1_setget_pixel_y(test19_server1_setget_pixel_y),   
      .Test19_Server1_setget_pixel_x(test19_server1_setget_pixel_x),   
      .Test19_Server1_setget_pixel_ack(test19_server1_setget_pixel_ack),   
      .Test19_Server1_setget_pixel_req(test19_server1_setget_pixel_req),    
      .Test19_Server1_setget_pixel_return(test19_server1_setget_pixel_return));

  Test19_Server1 theserver(  
      .reset(reset),   
      .clk(clk),   

      .Test19_Server1_start_ack(test19_server1_start_ack),   
      .Test19_Server1_start_req(test19_server1_start_req),    

      .Test19_Server1_get_id_ack(test19_server1_get_id_ack),   
      .Test19_Server1_get_id_req(test19_server1_get_id_req),    
      .Test19_Server1_get_id_return(test19_server1_get_id_return),

      .Test19_Server1_setget_pixel_wdata(test19_server1_setget_pixel_wdata),   
      .Test19_Server1_setget_pixel_readf(test19_server1_setget_pixel_readf),   
      .Test19_Server1_setget_pixel_y(test19_server1_setget_pixel_y),   
      .Test19_Server1_setget_pixel_x(test19_server1_setget_pixel_x),   
      .Test19_Server1_setget_pixel_ack(test19_server1_setget_pixel_ack),   
      .Test19_Server1_setget_pixel_req(test19_server1_setget_pixel_req),    
      .Test19_Server1_setget_pixel_return(test19_server1_setget_pixel_return));

   initial # 1000000 $finish();

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end
endmodule
// eof
