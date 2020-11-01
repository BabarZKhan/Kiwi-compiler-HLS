// (C) 2010-16 DJ Greaves, University of Cambridge.
// //
// Kiwi Scientific Acceleration  
// University of Cambridge, Computer Laboratory
//
// vsys-dualduplex.v

// Note that we no longer use this style very much - instead our load/store ports are wired via protocol adapators to AXI memory by HPR System Integrator.

//
//
`timescale 1ns/1ns

module ACME_SIMSYS();

   reg clk, reset;
   initial begin reset = 1; clk = 1; # 43 reset = 0; end
   always #5 clk = !clk;

   wire [7:0]  ksubsAbendSyndrome;
   wire        finished_signal = (ksubsAbendSyndrome != 0) && (ksubsAbendSyndrome != 128);
   reg [7:0]   finished_shifter;
   integer     clock_ticks;
   initial # (1 * 1000 * 1000) begin
      $display("\nFinish HDL simulation on timeout %t.", $time); 
      $finish(); 
      end

   initial begin $dumpfile("vcd.vcd"); $dumpvars(); end

   parameter dram_dwidth = 64;                      // perhaps more normal is 32 byte DRAM burst size or cache line.
   parameter bits_per_lane = 8;
   parameter noLanes = dram_dwidth / bits_per_lane; // Bytelanes.
     
   wire [noLanes-1:0] bs_r0_0bank_lanes;
   wire [21:0] 	       bs_r0_0bank_addr;
   wire bs_r0_0bank_oprdy, bs_r0_0bank_opreq, bs_r0_0bank_ack, bs_r0_0bank_rwbar;
   wire [dram_dwidth-1:0]  bs_r0_0bank_rdata, bs_r0_0bank_wdata;
   wire [noLanes-1:0] fs_r0_0bank_lanes;
   wire [21:0] 	       fs_r0_0bank_addr;
   wire fs_r0_0bank_oprdy, fs_r0_0bank_opreq, fs_r0_0bank_ack, fs_r0_0bank_rwbar;
   wire [dram_dwidth-1:0]  fs_r0_0bank_rdata, fs_r0_0bank_wdata;

   wire [noLanes-1:0] bs_r0_1bank_lanes;
   wire [21:0] 	       bs_r0_1bank_addr;
   wire bs_r0_1bank_oprdy, bs_r0_1bank_opreq, bs_r0_1bank_ack, bs_r0_1bank_rwbar;
   wire [dram_dwidth-1:0]  bs_r0_1bank_rdata, bs_r0_1bank_wdata;
   wire [noLanes-1:0] fs_r0_1bank_lanes;
   wire [21:0] 	       fs_r0_1bank_addr;
   wire fs_r0_1bank_oprdy, fs_r0_1bank_opreq, fs_r0_1bank_ack, fs_r0_1bank_rwbar;
   wire [dram_dwidth-1:0]  fs_r0_1bank_rdata, fs_r0_1bank_wdata;

   
   KIWI_DDR2_CONTROLLER bank_r0_0(.clk(clk), .reset(reset), 
				   .ctr_rwbar(fs_r0_0bank_rwbar), .ctr_rdata(fs_r0_0bank_rdata), .ctr_wdata(fs_r0_0bank_wdata), .ctr_wordAddr(fs_r0_0bank_addr), 
				   .ctr_oprdy(fs_r0_0bank_oprdy), .ctr_opreq(fs_r0_0bank_opreq), .ctr_ack(fs_r0_0bank_ack), .ctr_lanes(fs_r0_0bank_lanes));

   KIWI_DDR2_CONTROLLER bank_r0_1(.clk(clk), .reset(reset), 
				   .ctr_rwbar(fs_r0_1bank_rwbar), .ctr_rdata(fs_r0_1bank_rdata), .ctr_wdata(fs_r0_1bank_wdata), .ctr_wordAddr(fs_r0_1bank_addr), 
				   .ctr_oprdy(fs_r0_1bank_oprdy), .ctr_opreq(fs_r0_1bank_opreq), .ctr_ack(fs_r0_1bank_ack), .ctr_lanes(fs_r0_0bank_lanes));

   wire [noLanes-1:0] bs_r1_0bank_lanes;
   wire [21:0] 	       bs_r1_0bank_addr;
   wire bs_r1_0bank_oprdy, bs_r1_0bank_opreq, bs_r1_0bank_ack, bs_r1_0bank_rwbar;
   wire [dram_dwidth-1:0]  bs_r1_0bank_rdata, bs_r1_0bank_wdata;
   wire [noLanes-1:0] fs_r1_0bank_lanes;
   wire [21:0] 	       fs_r1_0bank_addr;
   wire fs_r1_0bank_oprdy, fs_r1_0bank_opreq, fs_r1_0bank_ack, fs_r1_0bank_rwbar;
   wire [dram_dwidth-1:0]  fs_r1_0bank_rdata, fs_r1_0bank_wdata;

   wire [noLanes-1:0] bs_r1_1bank_lanes;
   wire [21:0] 	       bs_r1_1bank_addr;
   wire bs_r1_1bank_oprdy, bs_r1_1bank_opreq, bs_r1_1bank_ack, bs_r1_1bank_rwbar;
   wire [dram_dwidth-1:0]  bs_r1_1bank_rdata, bs_r1_1bank_wdata;
   wire [noLanes-1:0] fs_r1_1bank_lanes;
   wire [21:0] 	       fs_r1_1bank_addr;
   wire fs_r1_1bank_oprdy, fs_r1_1bank_opreq, fs_r1_1bank_ack, fs_r1_1bank_rwbar;
   wire [dram_dwidth-1:0]  fs_r1_1bank_rdata, fs_r1_1bank_wdata;


   // NOTE - IT IS WRONG TO HAVE TWO BANKS HERE - WE SHOULD HAVE ONE AND A MULTIPLEXOR
   KIWI_DDR2_CONTROLLER bank_r1_0(.clk(clk), .reset(reset), 
				   .ctr_rwbar(fs_r1_0bank_rwbar), .ctr_rdata(fs_r1_0bank_rdata), .ctr_wdata(fs_r1_0bank_wdata), .ctr_wordAddr(fs_r1_0bank_addr), 
				   .ctr_oprdy(fs_r1_0bank_oprdy), .ctr_opreq(fs_r1_0bank_opreq), .ctr_ack(fs_r1_0bank_ack), .ctr_lanes(fs_r1_0bank_lanes));

   KIWI_DDR2_CONTROLLER bank_r1_1(.clk(clk), .reset(reset), 
				   .ctr_rwbar(fs_r1_1bank_rwbar), .ctr_rdata(fs_r1_1bank_rdata), .ctr_wdata(fs_r1_1bank_wdata), .ctr_wordAddr(fs_r1_1bank_addr), 
				   .ctr_oprdy(fs_r1_1bank_oprdy), .ctr_opreq(fs_r1_1bank_opreq), .ctr_ack(fs_r1_1bank_ack), .ctr_lanes(fs_r1_1bank_lanes));

   
   DUT the_dut(.clk(clk), 
	       .reset(reset),

	       // Bank0 port 0 - duplex
	       .bondout0_LANES0(fs_r0_0bank_lanes),
	       .bondout0_REQRDY0(fs_r0_0bank_oprdy), 
	       .bondout0_REQ0(fs_r0_0bank_opreq),
	       .bondout0_ACK0(fs_r0_0bank_ack),       
	       .bondout0_RWBAR0(fs_r0_0bank_rwbar),   .bondout0_RDATA0(fs_r0_0bank_rdata),
	       .bondout0_ADDR0(fs_r0_0bank_addr),     .bondout0_WDATA0(fs_r0_0bank_wdata),   

	       .bondout0_LANES1(fs_r0_1bank_lanes),
	       .bondout0_REQRDY1(fs_r0_1bank_oprdy), 
	       .bondout0_REQ1(fs_r0_1bank_opreq),
	       .bondout0_ACK1(fs_r0_1bank_ack),       
	       .bondout0_RWBAR1(fs_r0_1bank_rwbar),   .bondout0_RDATA1(fs_r0_1bank_rdata),
	       .bondout0_ADDR1(fs_r0_1bank_addr),     .bondout0_WDATA1(fs_r0_1bank_wdata),   

	       .bondout1_LANES0(fs_r1_0bank_lanes),
	       .bondout1_REQRDY0(fs_r1_0bank_oprdy), 
	       .bondout1_REQ0(fs_r1_0bank_opreq),
	       .bondout1_ACK0(fs_r1_0bank_ack),       
	       .bondout1_RWBAR0(fs_r1_0bank_rwbar),   .bondout1_RDATA0(fs_r1_0bank_rdata),
	       .bondout1_ADDR0(fs_r1_0bank_addr),     .bondout1_WDATA0(fs_r1_0bank_wdata),   

	       .bondout1_LANES1(fs_r1_1bank_lanes),
	       .bondout1_REQRDY1(fs_r1_1bank_oprdy), 
	       .bondout1_REQ1(fs_r1_1bank_opreq),
	       .bondout1_ACK1(fs_r1_1bank_ack),       
	       .bondout1_RWBAR1(fs_r1_1bank_rwbar),   .bondout1_RDATA1(fs_r1_1bank_rdata),
	       .bondout1_ADDR1(fs_r1_1bank_addr),     .bondout1_WDATA1(fs_r1_1bank_wdata),   
	       

	       .ksubsAbendSyndrome(ksubsAbendSyndrome)
       
	       );

     always @(posedge clk) begin
	clock_ticks <= (reset) ?0: clock_ticks+1;
	finished_shifter = (reset)?0: { finished_shifter, (finished_shifter[0]||finished_signal) };
	if (finished_shifter[7]) begin
	   $display("Finished with code 0x%02h at %1t after %d clocks", ksubsAbendSyndrome, $time, clock_ticks);
	   $finish(0);
	   end
//	        $display("%1t, pc, thread_pc10=%1d:  rdy=%1d req=%1d rr=%1d nn=%1d dd=%1d err=%1d v1=%1d", $time, the_dut.xpc10nz, the_dut.iuDIVIDER10_rdy, the_dut.iuDIVIDER10_req, the_dut.iuDIVIDER10_RR, the_dut.iuDIVIDER10_NN, the_dut.iuDIVIDER10_DD, the_dut.iuDIVIDER10_err, the_dut.TKWr1_5_V_1);
	//$display("%1t, pc, thread_pc10=%1d:  rdy=%1d req=%1d rr=%1d nn=%1d dd=%1d err=%1d v1=%1d", $time, the_dut.xpc10nz, the_dut.iuDIVIDER10_rdy, the_dut.iuDIVIDER10_req, the_dut.iuDIVIDER10_RR, the_dut.iuDIVIDER10_NN, the_dut.iuDIVIDER10_DD, the_dut.iuDIVIDER10_err, the_dut.TKWr1_5_V_1)
	
// 	$display("%1t pc %1d", $time, the_dut.bevelab10nz);
//	$display("pc %1d", the_dut.bevelab10nz);	
	end


endmodule
// eof
