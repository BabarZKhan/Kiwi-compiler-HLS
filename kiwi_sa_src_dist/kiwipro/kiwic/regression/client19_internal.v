

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:45:40
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.client19_internal test19.exe -kiwic-cil-dump=combined -vnl client19_internal.v -vnl-kandr=disable -root test19_client1;test19_client1.Main
`timescale 1ns/1ns


module client19_internal(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg signed [15:0] dout,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiTESTMAIN400PC10nz_pc_export);

function signed [15:0] rtl_signed_bitextract2;
   input [31:0] arg;
   rtl_signed_bitextract2 = $signed(arg[15:0]);
   endfunction


function signed [15:0] rtl_signed_bitextract1;
   input [31:0] arg;
   rtl_signed_bitextract1 = $signed(arg[15:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract5;
   input [31:0] arg;
   rtl_unsigned_bitextract5 = $unsigned(arg[7:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract4;
   input [31:0] arg;
   rtl_unsigned_bitextract4 = $unsigned(arg[7:0]);
   endfunction


function signed [31:0] rtl_sign_extend0;
   input [15:0] arg;
   rtl_sign_extend0 = { {16{arg[15]}}, arg[15:0] };
   endfunction


function [31:0] rtl_unsigned_extend3;
   input [7:0] arg;
   rtl_unsigned_extend3 = { 24'b0, arg[7:0] };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg [31:0] TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2;
  reg [31:0] TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0;
// abstractionName=res2-contacts pi_name=i_Test19_Server1
  wire i_Test19_Server1_start_ack;
  reg i_Test19_Server1_start_req;
  wire i_Test19_Server1_get_id_ack;
  reg i_Test19_Server1_get_id_req;
  wire [15:0] i_Test19_Server1_get_id_return;
  wire i_Test19_Server1_setget_pixel_ack;
  reg i_Test19_Server1_setget_pixel_req;
  wire [7:0] i_Test19_Server1_setget_pixel_return;
  reg [31:0] i_Test19_Server1_setget_pixel_axx;
  reg [31:0] i_Test19_Server1_setget_pixel_ayy;
  reg i_Test19_Server1_setget_pixel_readf;
  reg [7:0] i_Test19_Server1_setget_pixel_wdata;
// abstractionName=res2-morenets
  reg kiwiTESTMAIN400PC10_stall;
  reg kiwiTESTMAIN400PC10_clear;
  reg iTest19Server1setgetpixelreturnh10primed;
  reg iTest19Server1setgetpixelreturnh10vld;
  reg signed [7:0] iTest19Server1setgetpixelreturnh10hold;
  reg iTest19Server1getidreturnh10primed;
  reg iTest19Server1getidreturnh10vld;
  reg signed [15:0] iTest19Server1getidreturnh10hold;
  reg [3:0] kiwiTESTMAIN400PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               iTest19Server1setgetpixelreturnh10primed <= 32'd0;
               iTest19Server1getidreturnh10primed <= 32'd0;
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2 <= 32'd0;
               TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0 <= 32'd0;
               dout <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  $display("Start of Test 19");
                                   TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2 <= 32'h0;
                                   TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0 <= 32'h0;
                                   dout <= 32'sh0;
                                   end 
                                   kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           iTest19Server1getidreturnh10primed <= !kiwiTESTMAIN400PC10_stall;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  begin 
                                  if (!iTest19Server1getidreturnh10vld && !i_Test19_Server1_get_id_ack) $display("Remote server id is %1d"
                                      , (iTest19Server1getidreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract1(iTest19Server1getidreturnh10hold
                                      )))): rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract2(i_Test19_Server1_get_id_return
                                      ))))));
                                      if (iTest19Server1getidreturnh10vld || i_Test19_Server1_get_id_ack)  begin 
                                          $display("Remote server id is %1d", (iTest19Server1getidreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract1(iTest19Server1getidreturnh10hold
                                          )))): rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract2(i_Test19_Server1_get_id_return
                                          ))))));
                                           dout <= (iTest19Server1getidreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract1(iTest19Server1getidreturnh10hold
                                          )))): rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract2(i_Test19_Server1_get_id_return
                                          )))));

                                           end 
                                          if (!iTest19Server1getidreturnh10vld && !i_Test19_Server1_get_id_ack)  dout <= (iTest19Server1getidreturnh10vld
                                      ? rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract1(iTest19Server1getidreturnh10hold
                                      )))): rtl_sign_extend0(rtl_signed_bitextract1(rtl_sign_extend0(rtl_signed_bitextract2(i_Test19_Server1_get_id_return
                                      )))));

                                       end 
                                  if (iTest19Server1getidreturnh10vld || i_Test19_Server1_get_id_ack)  kiwiTESTMAIN400PC10nz <= 32'h3
                              /*3:kiwiTESTMAIN400PC10nz*/;

                               end 
                          
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0 <= 32'h2c;
                               kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2<32'd54) && !iTest19Server1setgetpixelreturnh10vld
                           && !i_Test19_Server1_setget_pixel_ack && !kiwiTESTMAIN400PC10_stall) $display(" Readback %1d  %1d", TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2
                              , (iTest19Server1setgetpixelreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract4(iTest19Server1setgetpixelreturnh10hold
                              )))): rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract5(i_Test19_Server1_setget_pixel_return
                              ))))));
                              if (iTest19Server1setgetpixelreturnh10vld || i_Test19_Server1_setget_pixel_ack)  begin 
                                  if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2<32'd54) && !kiwiTESTMAIN400PC10_stall
                                  ) $display(" Readback %1d  %1d", TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2, (iTest19Server1setgetpixelreturnh10vld
                                      ? rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract4(iTest19Server1setgetpixelreturnh10hold
                                      )))): rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract5(i_Test19_Server1_setget_pixel_return
                                      ))))));
                                      if (!kiwiTESTMAIN400PC10_stall)  dout <= (iTest19Server1setgetpixelreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract4(iTest19Server1setgetpixelreturnh10hold
                                      )))): rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract5(i_Test19_Server1_setget_pixel_return
                                      )))));

                                       kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                                   end 
                                  if (!iTest19Server1setgetpixelreturnh10vld && !i_Test19_Server1_setget_pixel_ack && !kiwiTESTMAIN400PC10_stall
                          )  dout <= (iTest19Server1setgetpixelreturnh10vld? rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract4(iTest19Server1setgetpixelreturnh10hold
                              )))): rtl_sign_extend0(rtl_signed_bitextract2(rtl_unsigned_extend3(rtl_unsigned_bitextract5(i_Test19_Server1_setget_pixel_return
                              )))));

                               end 
                          
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2 <= $unsigned(32'd1
                              +TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2);

                               kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if ((i_Test19_Server1_setget_pixel_ack || iTest19Server1setgetpixelreturnh10vld) && 
                  hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2>=32'd54))  begin if (!kiwiTESTMAIN400PC10_stall
                              )  begin 
                                      $display("End of Test 19");
                                      $finish(32'sd0);
                                       end 
                                       end 
                               else  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                                   iTest19Server1setgetpixelreturnh10primed <= !kiwiTESTMAIN400PC10_stall;
                                   end 
                                  if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2>=32'd54))  begin 
                                  if (!kiwiTESTMAIN400PC10_stall)  hpr_abend_syndrome <= 32'sd0;
                                       kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                                   iTest19Server1setgetpixelreturnh10primed <= !kiwiTESTMAIN400PC10_stall;
                                   end 
                                   end 
                          
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0
                      <32'd54))  begin 
                               kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                               iTest19Server1setgetpixelreturnh10primed <= !kiwiTESTMAIN400PC10_stall;
                               end 
                               else  begin 
                              if (!kiwiTESTMAIN400PC10_stall)  TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2 <= 32'h2c
                                  ;

                                   kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                               iTest19Server1setgetpixelreturnh10primed <= !kiwiTESTMAIN400PC10_stall;
                               end 
                              
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!kiwiTESTMAIN400PC10_stall)  TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0 <= $unsigned(32'd1
                              +TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0);

                               kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               iTest19Server1getidreturnh10primed <= 32'd0;
               iTest19Server1getidreturnh10vld <= 32'd0;
               iTest19Server1getidreturnh10hold <= 32'd0;
               iTest19Server1setgetpixelreturnh10primed <= 32'd0;
               iTest19Server1setgetpixelreturnh10vld <= 32'd0;
               iTest19Server1setgetpixelreturnh10hold <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if (i_Test19_Server1_get_id_ack && iTest19Server1getidreturnh10primed)  begin 
                           iTest19Server1getidreturnh10primed <= 32'd0;
                           iTest19Server1getidreturnh10vld <= 32'd1;
                           iTest19Server1getidreturnh10hold <= i_Test19_Server1_get_id_return;
                           end 
                          if (i_Test19_Server1_setget_pixel_ack && iTest19Server1setgetpixelreturnh10primed)  begin 
                           iTest19Server1setgetpixelreturnh10primed <= 32'd0;
                           iTest19Server1setgetpixelreturnh10vld <= 32'd1;
                           iTest19Server1setgetpixelreturnh10hold <= i_Test19_Server1_setget_pixel_return;
                           end 
                          if (!kiwiTESTMAIN400PC10_stall && kiwiTESTMAIN400PC10_clear)  begin 
                           iTest19Server1getidreturnh10vld <= 32'd0;
                           iTest19Server1setgetpixelreturnh10vld <= 32'd0;
                           end 
                           kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

 always   @(* )  begin 
       i_Test19_Server1_setget_pixel_axx = 32'sd0;
       i_Test19_Server1_setget_pixel_ayy = 32'sd0;
       i_Test19_Server1_setget_pixel_readf = 32'sd0;
       i_Test19_Server1_setget_pixel_wdata = 32'sd0;
       i_Test19_Server1_get_id_req = 32'sd0;
       i_Test19_Server1_setget_pixel_req = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h4/*4:kiwiTESTMAIN400PC10nz*/: if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2<32'd54) && !kiwiTESTMAIN400PC10_stall
          )  begin 
                   i_Test19_Server1_setget_pixel_axx = TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2;
                   i_Test19_Server1_setget_pixel_ayy = $unsigned(32'd63+TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2);
                   i_Test19_Server1_setget_pixel_readf = 32'd1;
                   i_Test19_Server1_setget_pixel_wdata = 32'd0;
                   end 
                  
          32'h7/*7:kiwiTESTMAIN400PC10nz*/: if ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0<32'd54) && !kiwiTESTMAIN400PC10_stall
          )  begin 
                   i_Test19_Server1_setget_pixel_axx = TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0;
                   i_Test19_Server1_setget_pixel_ayy = $unsigned(32'd63+TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0);
                   i_Test19_Server1_setget_pixel_readf = 32'd0;
                   i_Test19_Server1_setget_pixel_wdata = rtl_unsigned_bitextract5(TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0
                  +$unsigned(32'd63+TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0));

                   end 
                  endcase
      if (!kiwiTESTMAIN400PC10_stall && hpr_int_run_enable_DDX16)  begin 
               i_Test19_Server1_get_id_req = ((32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);
               i_Test19_Server1_setget_pixel_req = ((TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_0<32'd54) && (32'h7/*7:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || (TESTMAIN400_test19_client1_framestore_draw_diagonal_0_12_V_2<32'd54) && (32'h4/*4:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);

               end 
               hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       i_Test19_Server1_start_req = 32'd0;
       end 
      

always @(*) kiwiTESTMAIN400PC10_clear = (32'h0/*0:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h3/*3:kiwiTESTMAIN400PC10nz*/==
kiwiTESTMAIN400PC10nz) || (32'h4/*4:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h6/*6:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz
) || (32'h9/*9:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h7/*7:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);

always @(*) kiwiTESTMAIN400PC10_stall = ((32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h8/*8:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)) && !i_Test19_Server1_setget_pixel_ack
 && !iTest19Server1setgetpixelreturnh10vld || (32'h2/*2:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) && !iTest19Server1getidreturnh10vld
 && !i_Test19_Server1_get_id_ack;

  Test19_Server1 i_Test19_Server1(
        .clk(clk),
        .reset(reset),
        .start_ack(i_Test19_Server1_start_ack),
        .start_req(i_Test19_Server1_start_req
),
        .get_id_ack(i_Test19_Server1_get_id_ack),
        .get_id_req(i_Test19_Server1_get_id_req),
        .get_id_return(i_Test19_Server1_get_id_return
),
        .setget_pixel_ack(i_Test19_Server1_setget_pixel_ack),
        .setget_pixel_req(i_Test19_Server1_setget_pixel_req),
        .setget_pixel_return(i_Test19_Server1_setget_pixel_return
),
        .setget_pixel_axx(i_Test19_Server1_setget_pixel_axx),
        .setget_pixel_ayy(i_Test19_Server1_setget_pixel_ayy),
        .setget_pixel_readf(i_Test19_Server1_setget_pixel_readf
),
        .setget_pixel_wdata(i_Test19_Server1_setget_pixel_wdata));

// Structural Resource (FU) inventory for client19_internal:// 1 vectors of width 4
// 1 vectors of width 16
// 11 vectors of width 1
// 2 vectors of width 8
// 4 vectors of width 32
// Total state bits in module = 175 bits.
// 27 continuously assigned (wire/non-state) bits 
//   cell Test19_Server1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:45:37
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.client19_internal test19.exe -kiwic-cil-dump=combined -vnl client19_internal.v -vnl-kandr=disable -root test19_client1;test19_client1.Main


//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from kiwife:::
//Enumeration codepoints for KiwiSystem.Kiwi.PauseControl
//*----------------------+------------+---*
//| Token                | Code point | p |
//*----------------------+------------+---*
//| autoPauseEnable      | 0          | 6 |
//| hardPauseEnable      | 1          | 6 |
//| softPauseEnable      | 2          | 6 |
//| maximalPauseEnable   | 3          | 6 |
//| bblockPauseEnable    | 4          | 6 |
//| pipelinedAccelerator | 5          | 6 |
//*----------------------+------------+---*

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Read IP-XACT component definition Test19_Server1 from ./Test19_Server1.xml

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busDefinition AUTOMETA_setget_pixel

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busAbstraction AUTOMETA_setget_pixel_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busDefinition AUTOMETA_get_id

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busAbstraction AUTOMETA_get_id_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busDefinition AUTOMETA_start

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//IP-XACT read of busAbstraction AUTOMETA_start_rtl

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//Bondout Port Settings
//
//
//*----------------------------------+-------+-------------*
//
//
//| Key                              | Value | Description |
//
//
//*----------------------------------+-------+-------------*
//
//
//| bondout-loadstore-port-count     | 0     |             |
//
//
//| bondout-loadstore-lane_addr-size | 22    |             |
//
//
//*----------------------------------+-------+-------------*
//
//
//KiwiC: front end input processing of class Test19_Server1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test19_Server1..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=Test19_Server1..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class test19_client1  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'test19_client1' tid=TESTENT1401 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'test19_client1' tid=TESTENT1401
//
//
//root_compiler class done: test19_client1
//
//
//KiwiC: front end input processing of class test19_client1.Main  wonky=test19_client1 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test19_client1.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test19_client1.Main
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//Report of all settings used from the recipe or command line:
//
//
//   bondout-schema=bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8
//
//
//   bondout-protocol=HFAST1
//
//
//   bondout-loadstore-lane-width=8
//
//
//   bondout-loadstore-port-lanes=32
//
//
//   bondout-loadstore-port-count=0
//
//
//   bondout-loadstore-simplex-ports=disable
//
//
//   bondout-loadstore-lane-addr-size=22
//
//
//   kiwife-directorate-pc-export=enable
//
//
//   kiwife-directorate-endmode=finish
//
//
//   kiwife-directorate-startmode=self-start
//
//
//   kiwic-default-dynamic-heapalloc-bytes=1073741824
//
//
//   cil-uwind-budget=10000
//
//
//   kiwic-cil-dump=combined
//
//
//   kiwic-kcode-dump=disable
//
//
//   kiwic-supress-zero-inits=disable
//
//
//   kiwife-dynpoly=disable
//
//
//   kiwic-library-redirects=enable
//
//
//   kiwic-register-colours=disable
//
//
//   array-4d-name=KIWIARRAY4D
//
//
//   array-3d-name=KIWIARRAY3D
//
//
//   array-2d-name=KIWIARRAY2D
//
//
//   kiwi-dll=Kiwi.dll
//
//
//   kiwic-dll=Kiwic.dll
//
//
//   kiwic-zerolength-arrays=disable
//
//
//   kiwifefpgaconsole-default=enable
//
//
//   kiwife-directorate-style=normal
//
//
//   kiwife-postgen-optimise=enable
//
//
//   kiwife-allow-hpr-alloc=enable
//
//
//   kiwife-filesearch-loglevel=3
//
//
//   kiwife-cil-loglevel=3
//
//
//   kiwife-ataken-loglevel=3
//
//
//   kiwife-gtrace-loglevel=3
//
//
//   kiwife-constvol-loglevel=3
//
//
//   kiwife-hgen-loglevel=3
//
//
//   kiwife-firstpass-loglevel=3
//
//
//   kiwife-overloads-loglevel=3
//
//
//   root=test19_client1;test19_client1.Main
//
//
//   srcfile=test19.exe
//
//
//   kiwic-autodispose=disable
//
//
//END OF KIWIC REPORT FILE
//
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| Key                    | Value   | Description                                                                                                |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| int-flr-mul            | 1000    | Fixed-latency integer ALU integer latency scaling value for multiply.                                      |
//| max-no-fp-addsubs      | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread.                            |
//| max-no-fp-muls         | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.                                   |
//| max-no-int-muls        | 3       | Maximum number of int multipliers to instantiate per thread.                                               |
//| max-no-fp-divs         | 2       | Maximum number of f/p dividers to instantiate per thread.                                                  |
//| max-no-int-divs        | 2       | Maximum number of int dividers to instantiate per thread.                                                  |
//| max-no-rom-mirrors     | 8       | Maximum number of times to mirror a ROM per thread.                                                        |
//| max-ram-data_packing   | 8       | Maximum number of user words to pack into one RAM/loadstore word line.                                     |
//| fp-fl-dp-div           | 5       | Fixed-latency ALU floating-point, double-precision floating-point latency value for divide.                |
//| fp-fl-dp-add           | 4       | Fixed-latency ALU floating-point, double-precision floating-point latency value for add/sub.               |
//| fp-fl-dp-mul           | 3       | Fixed-latency ALU floating-point, double-precision floating-point latency value for multiply.              |
//| fp-fl-sp-div           | 15      | Fixed-latency ALU floating-point, single-precision floating-point floating-point latency value for divide. |
//| fp-fl-sp-add           | 4       | Fixed-latency ALU floating-point, single-precision floating-point latency value for add/sub.               |
//| fp-fl-sp-mul           | 5       | Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.              |
//| res2-offchip-threshold | 1000000 |                                                                                                            |
//| res2-combrom-threshold | 64      |                                                                                                            |
//| res2-combram-threshold | 32      |                                                                                                            |
//| res2-regfile-threshold | 8       |                                                                                                            |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN400PC10 
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"   | 817 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"   | 816 | 1       | hwm=0.1.0   | 2    |        | 2     | 2   | 3    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"   | 815 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 7    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 811 | 4       | hwm=0.1.0   | 5    |        | 5     | 5   | 6    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"   | 812 | 4       | hwm=0.0.0   | 4    |        | -     | -   | -    |
//| XU32'16:"16:kiwiTESTMAIN400PC10" | 810 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 813 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 4    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"   | 814 | 7       | hwm=0.1.0   | 8    |        | 8     | 8   | 9    |
//| XU32'32:"32:kiwiTESTMAIN400PC10" | 809 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 7    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                       |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                            |
//| F0   | E817 | R0 DATA |                                                                                                                            |
//| F0+E | E817 | W0 DATA | doutwrite(S32'0) TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0write(U32'0) TESTMAIN400.test19_client1.fram\ |
//|      |      |         | estore_draw_diagonal.0.12.V_2write(U32'0)  PLI:Start of Test 19                                                            |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*--------+------+---------+--------------------------------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                                           |
//*--------+------+---------+--------------------------------------------------------------------------------*
//| F1     | -    | R0 CTRL |                                                                                |
//| F1+S   | E816 | R0 DATA | i_Test19_Server1_get_idget_id(<NONE>)                                          |
//| F2+S   | E816 | R1 DATA |                                                                                |
//| F2+E+S | E816 | W0 DATA | doutwrite(E1) blockrefreftranfastspill10write(E2)  PLI:Remote server id is ... |
//*--------+------+---------+--------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                      |
//*------+------+---------+---------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                           |
//| F3   | E815 | R0 DATA |                                                                           |
//| F3+E | E815 | W0 DATA | TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0write(U32'44) |
//*------+------+---------+---------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*--------+------+---------+-------------------------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                                    |
//*--------+------+---------+-------------------------------------------------------------------------*
//| F4     | -    | R0 CTRL |                                                                         |
//| F4     | E812 | R0 DATA |                                                                         |
//| F4+E   | E812 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:End of Test 19                               |
//| F4+S   | E811 | R0 DATA | i_Test19_Server1_setget_pixelsetget_pixel(E3, E4, 1, 0)                 |
//| F5+S   | E811 | R1 DATA |                                                                         |
//| F5+E+S | E811 | W0 DATA | doutwrite(E5) blockrefreftranfastspill14write(E6)  PLI: Readback %u  %d |
//*--------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*------+------+---------+-----------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                  |
//*------+------+---------+-----------------------------------------------------------------------*
//| F6   | -    | R0 CTRL |                                                                       |
//| F6   | E810 | R0 DATA |                                                                       |
//| F6+E | E810 | W0 DATA | TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2write(E7) |
//*------+------+---------+-----------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*--------+------+---------+---------------------------------------------------------------------------*
//| pc     | eno  | Phaser  | Work                                                                      |
//*--------+------+---------+---------------------------------------------------------------------------*
//| F7     | -    | R0 CTRL |                                                                           |
//| F7+S   | E814 | R0 DATA | i_Test19_Server1_setget_pixelsetget_pixel(E8, E9, 0, E10)                 |
//| F8+S   | E814 | R1 DATA |                                                                           |
//| F8+E+S | E814 | W0 DATA | blockrefreftranfastspill12write(E11)                                      |
//| F7+S   | E813 | R0 DATA |                                                                           |
//| F7+E+S | E813 | W0 DATA | TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2write(U32'44) |
//*--------+------+---------+---------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                   |
//*------+------+---------+------------------------------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                                        |
//| F9   | E809 | R0 DATA |                                                                        |
//| F9+E | E809 | W0 DATA | TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0write(E12) |
//*------+------+---------+------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(C16(C(C16(*APPLY:<Test19_Server1>.get_id()))))
//
//
//  E2 =.= C(C16(*APPLY:<Test19_Server1>.get_id()))
//
//
//  E3 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2
//
//
//  E4 =.= Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2)
//
//
//  E5 =.= C(C16(Cu(C8u(*APPLY:<Test19_Server1>.setget_pixel(TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2, Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2), 1, 0)))))
//
//
//  E6 =.= Cu(C8u(*APPLY:<Test19_Server1>.setget_pixel(TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2, Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2), 1, 0)))
//
//
//  E7 =.= Cu(1+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2)
//
//
//  E8 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0
//
//
//  E9 =.= Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0)
//
//
//  E10 =.= CVT(C8u)(TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0+(Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0)))
//
//
//  E11 =.= Cu(C8u(*APPLY:<Test19_Server1>.setget_pixel(TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0, Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0), 0, CVT(C8u)(TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0+(Cu(63+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0))))))
//
//
//  E12 =.= Cu(1+TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0)
//
//
//  E13 =.= {[|iTest19Server1getidreturnh10vld|]; [|i_Test19_Server1_get_id_ack|]}
//
//
//  E14 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2>=54
//
//
//  E15 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_2<54
//
//
//  E16 =.= {[|iTest19Server1setgetpixelreturnh10vld|]; [|i_Test19_Server1_setget_pixel_ack|]}
//
//
//  E17 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0<54
//
//
//  E18 =.= TESTMAIN400.test19_client1.framestore_draw_diagonal.0.12.V_0>=54
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for client19_internal to client19_internal

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for client19_internal:
//1 vectors of width 4
//
//1 vectors of width 16
//
//11 vectors of width 1
//
//2 vectors of width 8
//
//4 vectors of width 32
//
//Total state bits in module = 175 bits.
//
//27 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread Test19_Server1..cctor uid=cctor10 has 4 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test19_client1.Main uid=Main10 has 35 CIL instructions in 11 basic blocks
//Thread mpc10 has 7 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 10 minor control states
// eof (HPR L/S Verilog)
