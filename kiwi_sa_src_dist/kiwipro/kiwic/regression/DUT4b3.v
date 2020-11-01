

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:43:14
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b3.v -vnl-resets=synchronous -sim 3000 -res2-regfile-threshold=3 -res2-combram-threshold=5 -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=kiwicmiscio10 */
input din,
    output dout,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTESTMAIN400PC10nz_pc_export,
    
/* portgroup= abstractionName=bondout2 pi_name=bondout0 */
output reg bondout0_REQ0,
    input bondout0_ACK0,
    input bondout0_REQRDY0,
    output reg bondout0_ACKRDY0,
    output reg [21:0] bondout0_ADDR0,
    output reg bondout0_RWBAR0,
    output reg [63:0] bondout0_WDATA0,
    input [63:0] bondout0_RDATA0,
    output reg [7:0] bondout0_LANES0,
    output reg bondout0_REQ1,
    input bondout0_ACK1,
    input bondout0_REQRDY1,
    output reg bondout0_ACKRDY1,
    output reg [21:0] bondout0_ADDR1,
    output reg bondout0_RWBAR1,
    output reg [63:0] bondout0_WDATA1,
    input [63:0] bondout0_RDATA1,
    output reg [7:0] bondout0_LANES1,
    
/* portgroup= abstractionName=bondout0 pi_name=bondout1 */
output reg bondout1_REQ0,
    input bondout1_ACK0,
    input bondout1_REQRDY0,
    output reg bondout1_ACKRDY0,
    output reg [21:0] bondout1_ADDR0,
    output reg bondout1_RWBAR0,
    output reg [63:0] bondout1_WDATA0,
    input [63:0] bondout1_RDATA0,
    output reg [7:0] bondout1_LANES0,
    output reg bondout1_REQ1,
    input bondout1_ACK1,
    input bondout1_REQRDY1,
    output reg bondout1_ACKRDY1,
    output reg [21:0] bondout1_ADDR1,
    output reg bondout1_RWBAR1,
    output reg [63:0] bondout1_WDATA1,
    input [63:0] bondout1_RDATA1,
    output reg [7:0] bondout1_LANES1);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_6;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_4;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_3;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_1;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_0;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire signed [31:0] A_SINT_CC_SCALbx10_ARA0_rdata;
  reg [3:0] A_SINT_CC_SCALbx10_ARA0_addr;
  reg A_SINT_CC_SCALbx10_ARA0_wen;
  reg A_SINT_CC_SCALbx10_ARA0_ren;
  reg signed [31:0] A_SINT_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg signed [31:0] SINTCCSCALbx10ARA0rdatah10hold;
  reg SINTCCSCALbx10ARA0rdatah10shot0;
  reg [3:0] kiwiTESTMAIN400PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_4 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_0 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_1 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_3 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_6 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test4_arraypart_0_3_V_4 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_0 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_1 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_3 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_6 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                      
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!(!(((32'hc/*12:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata: SINTCCSCALbx10ARA0rdatah10hold
                          )%32'sd2)))  TESTMAIN400_test4_arraypart_0_3_V_0 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_0);
                               else  TESTMAIN400_test4_arraypart_0_3_V_1 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_1);
                           kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_test4_arraypart_0_3_V_4 <= ((32'hc/*12:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? A_SINT_CC_SCALbx10_ARA0_rdata
                          : SINTCCSCALbx10ARA0rdatah10hold);

                           end 
                          
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6)>=32'sh9))  begin 
                                  $write("%1d vale=%1d: ", TESTMAIN400_test4_arraypart_0_3_V_3, TESTMAIN400_test4_arraypart_0_3_V_4);
                                  $display("so far %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                  $display("Found %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                   end 
                                   else  begin 
                                  $write("%1d vale=%1d: ", TESTMAIN400_test4_arraypart_0_3_V_3, TESTMAIN400_test4_arraypart_0_3_V_4);
                                  $display("so far %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                   kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test4_arraypart_0_3_V_3 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_3);
                                   TESTMAIN400_test4_arraypart_0_3_V_6 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6);
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6)>=32'sh9))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'he/*14:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_test4_arraypart_0_3_V_3 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_3);
                                   TESTMAIN400_test4_arraypart_0_3_V_6 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6);
                                   end 
                                   end 
                          
                  32'he/*14:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10hold <= 32'd0;
               SINTCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16)  begin 
                  if (SINTCCSCALbx10ARA0rdatah10shot0)  SINTCCSCALbx10ARA0rdatah10hold <= A_SINT_CC_SCALbx10_ARA0_rdata;
                       kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
                   SINTCCSCALbx10ARA0rdatah10shot0 <= (32'hb/*11:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

 always   @(* )  begin 
       A_SINT_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wdata = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_SINT_CC_SCALbx10_ARA0_ren = 32'sd0;
       hpr_int_run_enable_DDX16 = 32'sd1;

      case (kiwiTESTMAIN400PC10nz)
          32'h0/*0:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd2;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh2;
               end 
              
          32'h1/*1:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd8;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh800;
               end 
              
          32'h2/*2:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd7;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh7e5;
               end 
              
          32'h3/*3:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd6;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh461;
               end 
              
          32'h4/*4:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd5;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh8;
               end 
              
          32'h5/*5:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd4;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh7;
               end 
              
          32'h6/*6:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd3;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh5;
               end 
              
          32'h7/*7:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd1;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh1;
               end 
              
          32'h8/*8:kiwiTESTMAIN400PC10nz*/:  begin 
               A_SINT_CC_SCALbx10_ARA0_addr = 32'd0;
               A_SINT_CC_SCALbx10_ARA0_wdata = 32'sh64;
               end 
              
          32'hb/*11:kiwiTESTMAIN400PC10nz*/:  A_SINT_CC_SCALbx10_ARA0_addr = TESTMAIN400_test4_arraypart_0_3_V_6;
      endcase
      if (hpr_int_run_enable_DDX16)  begin 
               A_SINT_CC_SCALbx10_ARA0_wen = ((32'h0/*0:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h2/*2:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || (32'h4/*4:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h6/*6:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || (32'h8/*8:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h7/*7:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || (32'h5/*5:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz) || (32'h3/*3:kiwiTESTMAIN400PC10nz*/==
              kiwiTESTMAIN400PC10nz) || (32'h1/*1:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);

               A_SINT_CC_SCALbx10_ARA0_ren = ((32'hb/*11:kiwiTESTMAIN400PC10nz*/==kiwiTESTMAIN400PC10nz)? 32'd1: 32'd0);
               end 
               hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       bondout0_REQ0 = 32'd0;
       bondout0_ACKRDY0 = 32'd0;
       bondout0_ADDR0 = 32'd0;
       bondout0_RWBAR0 = 32'd0;
       bondout0_WDATA0 = 64'd0;
       bondout0_LANES0 = 32'd0;
       bondout0_REQ1 = 32'd0;
       bondout0_ACKRDY1 = 32'd0;
       bondout0_ADDR1 = 32'd0;
       bondout0_RWBAR1 = 32'd0;
       bondout0_WDATA1 = 64'd0;
       bondout0_LANES1 = 32'd0;
       bondout1_REQ0 = 32'd0;
       bondout1_ACKRDY0 = 32'd0;
       bondout1_ADDR0 = 32'd0;
       bondout1_RWBAR0 = 32'd0;
       bondout1_WDATA0 = 64'd0;
       bondout1_LANES0 = 32'd0;
       bondout1_REQ1 = 32'd0;
       bondout1_ACKRDY1 = 32'd0;
       bondout1_ADDR1 = 32'd0;
       bondout1_RWBAR1 = 32'd0;
       bondout1_WDATA1 = 64'd0;
       bondout1_LANES1 = 32'd0;
       end 
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd32),
        .ADDR_WIDTH(32'sd4),
        .WORDS(32'sd9),
        .LANE_WIDTH(32'sd32),
        .trace_me(32'sd0
)) A_SINT_CC_SCALbx10_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_SINT_CC_SCALbx10_ARA0_rdata),
        .addr(A_SINT_CC_SCALbx10_ARA0_addr
),
        .wen(A_SINT_CC_SCALbx10_ARA0_wen),
        .ren(A_SINT_CC_SCALbx10_ARA0_ren),
        .wdata(A_SINT_CC_SCALbx10_ARA0_wdata
));

// Structural Resource (FU) inventory for DUT:// 2 vectors of width 4
// 4 vectors of width 1
// 7 vectors of width 32
// Total state bits in module = 236 bits.
// 32 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:43:11
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b3.v -vnl-resets=synchronous -sim 3000 -res2-regfile-threshold=3 -res2-combram-threshold=5 -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| AddressSpace | Name       | Protocol                      | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*

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
//| bondout-loadstore-port-count     | 1     |             |
//
//
//| bondout-loadstore-lane_addr-size | 22    |             |
//
//
//*----------------------------------+-------+-------------*
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class test4  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test4..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test4..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test4  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'test4' tid=TEST4401 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'test4' tid=TEST4401
//
//
//root_compiler class done: test4
//
//
//KiwiC: front end input processing of class test4.Main  wonky=test4 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test4.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test4.Main
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
//   bondout-loadstore-port-count=1
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
//   kiwic-cil-dump=disable
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
//   root=test4;test4.Main
//
//
//   srcfile=test4.exe
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
//Bondout Load/Store (and other) Ports 'Res2 Preliminary'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

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
//| res2-combram-threshold | 5       |                                                                                                            |
//| res2-regfile-threshold | 3       |                                                                                                            |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN400PC10 
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                  | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10" | 811 | 0       | hwm=0.0.9   | 0    |        | 1     | 9   | 11   |
//| XU32'2:"2:kiwiTESTMAIN400PC10" | 809 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 13   |
//| XU32'1:"1:kiwiTESTMAIN400PC10" | 810 | 11      | hwm=0.1.0   | 12   |        | 12    | 12  | 10   |
//| XU32'4:"4:kiwiTESTMAIN400PC10" | 807 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 11   |
//| XU32'4:"4:kiwiTESTMAIN400PC10" | 808 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 14   |
//| XU32'8:"8:kiwiTESTMAIN400PC10" | 806 | 14      | hwm=0.0.0   | 14   |        | -     | -   | -    |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                         |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                              |
//| F0   | E811 | R0 DATA |                                                                                                                              |
//| F0+E | E811 | W0 DATA | TESTMAIN400.test4.arraypart.0.3.V_6write(S32'0) TESTMAIN400.test4.arraypart.0.3.V_3write(S32'0) TESTMAIN400.test4.arraypart\ |
//|      |      |         | .0.3.V_1write(S32'0) TESTMAIN400.test4.arraypart.0.3.V_0write(S32'0) TESTMAIN400.test4.arraypart.0.3.V_4write(S32'0) @_SINT\ |
//|      |      |         | /CC/SCALbx10_ARA0_write(2, S32'2)                                                                                            |
//| F1   | E811 | W1 DATA | @_SINT/CC/SCALbx10_ARA0_write(8, S32'2048)                                                                                   |
//| F2   | E811 | W2 DATA | @_SINT/CC/SCALbx10_ARA0_write(7, S32'2021)                                                                                   |
//| F3   | E811 | W3 DATA | @_SINT/CC/SCALbx10_ARA0_write(6, S32'1121)                                                                                   |
//| F4   | E811 | W4 DATA | @_SINT/CC/SCALbx10_ARA0_write(5, S32'8)                                                                                      |
//| F5   | E811 | W5 DATA | @_SINT/CC/SCALbx10_ARA0_write(4, S32'7)                                                                                      |
//| F6   | E811 | W6 DATA | @_SINT/CC/SCALbx10_ARA0_write(3, S32'5)                                                                                      |
//| F7   | E811 | W7 DATA | @_SINT/CC/SCALbx10_ARA0_write(1, S32'1)                                                                                      |
//| F8   | E811 | W8 DATA | @_SINT/CC/SCALbx10_ARA0_write(0, S32'100)                                                                                    |
//| F9   | E811 | W9 DATA |                                                                                                                              |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F10   | -    | R0 CTRL |      |
//| F10   | E809 | R0 DATA |      |
//| F10+E | E809 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                                                   |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                                                                                                        |
//| F11   | E810 | R0 DATA | @_SINT/CC/SCALbx10_ARA0_read(E1)                                                                                                       |
//| F12   | E810 | R1 DATA |                                                                                                                                        |
//| F12+E | E810 | W0 DATA | TESTMAIN400.test4.arraypart.0.3.V_1write(E2) TESTMAIN400.test4.arraypart.0.3.V_0write(E3) TESTMAIN400.test4.arraypart.0.3.V_4write(E4) |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                                                           |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*
//| F13   | -    | R0 CTRL |                                                                                                                |
//| F13   | E808 | R0 DATA |                                                                                                                |
//| F13+E | E808 | W0 DATA | TESTMAIN400.test4.arraypart.0.3.V_6write(E5) TESTMAIN400.test4.arraypart.0.3.V_3write(E6)  PLI:Found %d Odd N\ |
//|       |      |         | umbers...  PLI:so far %d Odd Number...  PLI:%d vale=%d:                                                        |
//| F13   | E807 | R0 DATA |                                                                                                                |
//| F13+E | E807 | W0 DATA | TESTMAIN400.test4.arraypart.0.3.V_6write(E5) TESTMAIN400.test4.arraypart.0.3.V_3write(E6)  PLI:so far %d Odd \ |
//|       |      |         | Number...  PLI:%d vale=%d:                                                                                     |
//*-------+------+---------+----------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F14   | -    | R0 CTRL |                       |
//| F14   | E806 | R0 DATA |                       |
//| F14+E | E806 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout1 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Final'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= TESTMAIN400.test4.arraypart.0.3.V_6
//
//
//  E2 =.= C(1+TESTMAIN400.test4.arraypart.0.3.V_1)
//
//
//  E3 =.= C(1+TESTMAIN400.test4.arraypart.0.3.V_0)
//
//
//  E4 =.= C(@_SINT/CC/SCALbx10_ARA0[TESTMAIN400.test4.arraypart.0.3.V_6])
//
//
//  E5 =.= C(1+TESTMAIN400.test4.arraypart.0.3.V_6)
//
//
//  E6 =.= C(1+(C(TESTMAIN400.test4.arraypart.0.3.V_3)))
//
//
//  E7 =.= (C(1+TESTMAIN400.test4.arraypart.0.3.V_6))>=S32'9
//
//
//  E8 =.= (C(1+TESTMAIN400.test4.arraypart.0.3.V_6))<S32'9
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for DUT4b3 to DUT4b3

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//2 vectors of width 4
//
//4 vectors of width 1
//
//7 vectors of width 32
//
//Total state bits in module = 236 bits.
//
//32 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test4..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test4.Main uid=Main10 has 42 CIL instructions in 8 basic blocks
//Thread mpc10 has 5 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 15 minor control states
// eof (HPR L/S Verilog)
