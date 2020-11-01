

// CBG Orangepath HPR L/S System

// Verilog output file generated at 30/01/2016 17:02:49
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.01: January-2016 Unix 3.19.8.100
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-resets=synchronous -bevelab-default-pause-mode=hard -verilog-roundtrip=disable primesya.exe -vnl=offchip.v -vnl-rootmodname=DUT -kiwic-finish=disable -diosim-vcd=vcd.vcd
`timescale 1ns/10ps


module DUT(    output reg [639:0] KppWaypoint0,
    output reg [639:0] KppWaypoint1,
    input [31:0] volume,
    output reg [31:0] count,
    output reg signed [31:0] elimit,
    output reg signed [31:0] evariant,
    output reg signed [31:0] edesign,
    output reg finished,
    output hf1_dram0bank_opreq,
    input hf1_dram0bank_oprdy,
    input hf1_dram0bank_ack,
    output hf1_dram0bank_rwbar,
    output [255:0] hf1_dram0bank_wdata,
    output [21:0] hf1_dram0bank_addr,
    input [255:0] hf1_dram0bank_rdata,
    output [31:0] hf1_dram0bank_lanes,
    input clk,
    input reset);
  wire [63:0] tnow;
  integer pTMT4Main_V_1;
  integer pTMT4Main_V_2;
  integer pTMT4Main_V_3;
  integer pTMT4Main_V_4;
  reg A_BOOL_CC_SCALbx10_ARA0[999:0];
  wire hprKppMarkres10;
  wire hprKppMarkres12;
  wire hprKppMarkres14;
  wire hprKppMarkres16;
  reg A_BOOL_CC_SCALbx10_ARA0_RDD0;
  reg [31:0] A_BOOL_CC_SCALbx10_ARA0_registered_AD0;
  reg A_BOOL_CC_SCALbx10_ARA0_WEN0;
  reg A_BOOL_CC_SCALbx10_ARA0_REN0;
  reg A_BOOL_CC_SCALbx10_ARA0_WRD0;
  reg [31:0] A_BOOL_CC_SCALbx10_ARA0_AD0;
  reg BOOLCCSCALbx10ARA0RRh10hold;
  reg BOOLCCSCALbx10ARA0RRh10shot0;
  reg [4:0] xpc10nz;
//Start structure HPR HPR_SSRAM_1000_1
//End structure HPR HPR_SSRAM_1000_1


 always   @(* )  begin 
    
   A_BOOL_CC_SCALbx10_ARA0_AD0 = 0;
       A_BOOL_CC_SCALbx10_ARA0_WRD0 = 0;
       A_BOOL_CC_SCALbx10_ARA0_WEN0 = 0;
       A_BOOL_CC_SCALbx10_ARA0_REN0 = 0;
       A_BOOL_CC_SCALbx10_ARA0_REN0 = ((xpc10nz==4'd10/*10:US*/) || (xpc10nz==4'd13/*13:US*/)? 1'd1: 1'd0);
       A_BOOL_CC_SCALbx10_ARA0_WEN0 = (((32'hffffffff&32'd1+pTMT4Main_V_1)<32'd1000)? (xpc10nz==4'd14/*14:US*/): (xpc10nz==5'd16/*16:US*/)) || 
      (xpc10nz==5'd19/*19:US*/) || (xpc10nz==2'd2/*2:US*/);

      if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)<32'd1000) && (xpc10nz==2'd2/*2:US*/)) 
           A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'd0;
          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)<32'd1000) && (xpc10nz==1'd1
          /*1:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3;
          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)>=32'd1000) && (xpc10nz==2'd2
          /*2:US*/))  A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'd0;
          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)>=32'd1000) && (xpc10nz==1'd1
          /*1:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3;
          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)<32'd1000) && (xpc10nz==2'd2/*2:US*/))  A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'd0;
          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)<32'd1000) && (xpc10nz==1'd1/*1:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_3
          ;

          if ((xpc10nz==4'd9/*9:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = 32'h1*pTMT4Main_V_4;
          if ((xpc10nz==4'd12/*12:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = 1'h1*pTMT4Main_V_1;
          if (((32'hffffffff&32'd1+pTMT4Main_V_1)>=32'd1000))  begin 
              if ((xpc10nz==5'd16/*16:US*/))  A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'd1;
                  if ((xpc10nz==4'd15/*15:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_1;
                   end 
              if ((xpc10nz==4'd12/*12:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = 1'h1*pTMT4Main_V_1;
          if (((32'hffffffff&32'd1+pTMT4Main_V_1)<32'd1000))  begin 
              if ((xpc10nz==4'd14/*14:US*/))  A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'd1;
                  if ((xpc10nz==4'd13/*13:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = pTMT4Main_V_1;
                   end 
              if ((xpc10nz==5'd19/*19:US*/))  A_BOOL_CC_SCALbx10_ARA0_WRD0 = 1'h1&(0<volume);
          if ((xpc10nz==5'd18/*18:US*/))  A_BOOL_CC_SCALbx10_ARA0_AD0 = 32'd0;
           end 
      
   integer p;
   always   @(posedge clk )  begin 
    p = 0; // manual
    while (p < 32) begin
       $write("%d", A_BOOL_CC_SCALbx10_ARA0[p]);
       p = p + 1;
    end // UNMATCHED !!
    $display();

      //Start structure HPR HPR_SSRAM_1000_1
      if (!reset)  begin if (A_BOOL_CC_SCALbx10_ARA0_WEN0)
	begin
	   $display("%d write %d with %d", xpc10nz , A_BOOL_CC_SCALbx10_ARA0_registered_AD0, A_BOOL_CC_SCALbx10_ARA0_WRD0);
	   A_BOOL_CC_SCALbx10_ARA0[A_BOOL_CC_SCALbx10_ARA0_registered_AD0] <= A_BOOL_CC_SCALbx10_ARA0_WRD0;
	end
               end 
          //End structure HPR HPR_SSRAM_1000_1


      //Start structure HPR primesya.exe
      if (reset)  begin 
               A_BOOL_CC_SCALbx10_ARA0_registered_AD0 <= 32'd0;
               pTMT4Main_V_1 <= 32'd0;
               pTMT4Main_V_4 <= 32'd0;
               pTMT4Main_V_2 <= 32'd0;
               pTMT4Main_V_3 <= 32'd0;
               count <= 32'd0;
               evariant <= 32'd0;
               edesign <= 32'd0;
               finished <= 1'd0;
               elimit <= 32'd0;
               KppWaypoint1 <= 640'd0;
               KppWaypoint0 <= 640'd0;
               BOOLCCSCALbx10ARA0RRh10hold <= 1'd0;
               BOOLCCSCALbx10ARA0RRh10shot0 <= 1'd0;
               xpc10nz <= 5'd0;
               end 
               else  begin 
              if (((32'hffffffff&32'd1+pTMT4Main_V_1)<32'd1000))  begin if ((xpc10nz==4'd13/*13:US*/)) $display("Setting initial array flag to hold : addr=%d readback=%d"
                      , pTMT4Main_V_1, ((pTMT4Main_V_1==1'h1*pTMT4Main_V_1)? 1'd1: A_BOOL_CC_SCALbx10_ARA0_RDD0));
                       end 
                   else if ((xpc10nz==4'd15/*15:US*/)) $display("Setting initial array flag to hold : addr=%d readback=%d", pTMT4Main_V_1
                      , ((pTMT4Main_V_1==1'h1*pTMT4Main_V_1)? 1'd1: A_BOOL_CC_SCALbx10_ARA0_RDD0));
                      
              case (xpc10nz)

                  0/*0:US*/:  begin 
                      $display("%s%d", "Primes Up To ", 32'd1000);
                       count <= 32'd0;
                       evariant <= 32'd0;
                       edesign <= 32'd4032;
                       finished <= 1'd0;
                       elimit <= 32'd1000;
                       KppWaypoint1 <= "INITIALISE";
                       KppWaypoint0 <= "START";
                       xpc10nz <= 5'd18/*18:xpc10nz*/;
                       end 
                      
                  1'd1/*1:US*/:  begin 
                      if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)<32'd1000)) $display("a Cross off %d %d   (count1=%d)", pTMT4Main_V_2
                          , 32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3, 32'd2);
                          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)>=32'd1000
                          )) $display("Now counting");
                          if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)>=32'd1000
                          ))  begin 
                               pTMT4Main_V_2 <= 32'd1+pTMT4Main_V_2;
                               KppWaypoint1 <= "COUNTING";
                               KppWaypoint0 <= "wp3";
                               pTMT4Main_V_4 <= 32'd0;
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               xpc10nz <= 2'd3/*3:xpc10nz*/;
                               end 
                              if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)>=32'd1000) && ((32'hffffffff&32'd1+pTMT4Main_V_2)<32'd1000
                          ))  begin 
                               pTMT4Main_V_2 <= 32'd1+pTMT4Main_V_2;
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               xpc10nz <= 3'd4/*4:xpc10nz*/;
                               end 
                              if (((32'hffffffff&pTMT4Main_V_2+pTMT4Main_V_3)<32'd1000))  begin 
                               pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                               xpc10nz <= 2'd2/*2:xpc10nz*/;
                               end 
                               end 
                      
                  4'd10/*10:US*/:  begin 
                      if (((32'hffffffff&32'd1+pTMT4Main_V_4)<32'd1000) && A_BOOL_CC_SCALbx10_ARA0_RDD0) $display("Tally counting %d %d at %d"
                          , pTMT4Main_V_4, 32'hffffffff&32'd1+count, $time);
                          if (((32'hffffffff&32'd1+pTMT4Main_V_4)<32'd1000) && !A_BOOL_CC_SCALbx10_ARA0_RDD0) $display("Tally counting %d %d at %d"
                          , pTMT4Main_V_4, count, $time);
                          if (((32'hffffffff&32'd1+pTMT4Main_V_4)>=32'd1000) && A_BOOL_CC_SCALbx10_ARA0_RDD0)  begin 
                              $display("Tally counting %d %d at %d", pTMT4Main_V_4, 32'hffffffff&32'd1+count, $time);
                              $display("There are %d primes below the natural number %d.", 32'hffffffff&32'd1+count, 32'd1000);
                              $display("Optimisation variant=%d (count1 is %d).", 0, 32'd2);
                               end 
                              if (((32'hffffffff&32'd1+pTMT4Main_V_4)>=32'd1000) && !A_BOOL_CC_SCALbx10_ARA0_RDD0)  begin 
                              $display("Tally counting %d %d at %d", pTMT4Main_V_4, count, $time);
                              $display("There are %d primes below the natural number %d.", count, 32'd1000);
                              $display("Optimisation variant=%d (count1 is %d).", 0, 32'd2);
                               end 
                              if (((32'hffffffff&32'd1+pTMT4Main_V_4)>=32'd1000) && A_BOOL_CC_SCALbx10_ARA0_RDD0)  begin 
                               pTMT4Main_V_4 <= 32'd1+pTMT4Main_V_4;
                               count <= 32'd1+count;
                               end 
                              if (((32'hffffffff&32'd1+pTMT4Main_V_4)>=32'd1000) && !A_BOOL_CC_SCALbx10_ARA0_RDD0)  pTMT4Main_V_4 <= 32'd1
                          +pTMT4Main_V_4;

                          if (((32'hffffffff&32'd1+pTMT4Main_V_4)<32'd1000))  begin 
                               pTMT4Main_V_4 <= 32'd1+pTMT4Main_V_4;
                               xpc10nz <= 4'd9/*9:xpc10nz*/;
                               end 
                               else  xpc10nz <= 4'd8/*8:xpc10nz*/;
                      if (((32'hffffffff&32'd1+pTMT4Main_V_4)<32'd1000) && A_BOOL_CC_SCALbx10_ARA0_RDD0)  count <= 32'd1+count;
                           end 
                      
                  4'd11/*11:US*/:  begin 
                      if (((32'hffffffff&pTMT4Main_V_2)>=32'd1000))  begin 
                              $display("Skip out on square");
                              $display("Now counting");
                               end 
                               else $display("b Cross off %d %d   (count1=%d)", pTMT4Main_V_2, 32'hffffffff&pTMT4Main_V_2, 32'd2);
                      if (((32'hffffffff&pTMT4Main_V_2)<32'd1000))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                           else  begin 
                               KppWaypoint1 <= "COUNTING";
                               KppWaypoint0 <= "wp3";
                               pTMT4Main_V_4 <= 32'd0;
                               xpc10nz <= 4'd9/*9:xpc10nz*/;
                               end 
                               pTMT4Main_V_3 <= pTMT4Main_V_2;
                       end 
                      endcase
              if (((32'hffffffff&32'd1+pTMT4Main_V_1)<32'd1000)) 
                  case (xpc10nz)

                      4'd12/*12:US*/:  xpc10nz <= 4'd13/*13:xpc10nz*/;

                      4'd13/*13:US*/:  pTMT4Main_V_1 <= 32'd1+pTMT4Main_V_1;
                  endcase
                   else 
                  case (xpc10nz)

                      4'd12/*12:US*/:  xpc10nz <= 4'd15/*15:xpc10nz*/;

                      4'd15/*15:US*/:  begin 
                           KppWaypoint1 <= "CROSSOFF";
                           KppWaypoint0 <= "wp2";
                           pTMT4Main_V_1 <= 32'd1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= 32'd2;
                           end 
                          endcase

              case (xpc10nz)

                  3'd6/*6:US*/:  begin 
                       finished <= 1'd1;
                       xpc10nz <= 3'd5/*5:xpc10nz*/;
                       end 
                      
                  3'd7/*7:US*/:  begin 
                       KppWaypoint0 <= "FINISH";
                       xpc10nz <= 3'd6/*6:xpc10nz*/;
                       end 
                      
                  4'd13/*13:US*/:  xpc10nz <= 4'd14/*14:xpc10nz*/;

                  4'd15/*15:US*/:  xpc10nz <= 5'd16/*16:xpc10nz*/;

                  5'd17/*17:US*/:  begin 
                       pTMT4Main_V_1 <= 32'd0;
                       count <= 32'd0;
                       xpc10nz <= 4'd12/*12:xpc10nz*/;
                       end 
                      endcase
               A_BOOL_CC_SCALbx10_ARA0_registered_AD0 <= A_BOOL_CC_SCALbx10_ARA0_AD0;
              if (BOOLCCSCALbx10ARA0RRh10shot0)  BOOLCCSCALbx10ARA0RRh10hold <= A_BOOL_CC_SCALbx10_ARA0_RDD0;
                   BOOLCCSCALbx10ARA0RRh10shot0 <= (xpc10nz==4'd9/*9:US*/) || (xpc10nz==4'd12/*12:US*/);
              if ((xpc10nz==2'd2/*2:US*/))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                  if ((xpc10nz==2'd3/*3:US*/))  xpc10nz <= 4'd9/*9:xpc10nz*/;
                  if ((xpc10nz==3'd4/*4:US*/))  xpc10nz <= 4'd11/*11:xpc10nz*/;
                  if ((xpc10nz==3'd5/*5:US*/))  xpc10nz <= 3'd5/*5:xpc10nz*/;
                  if ((xpc10nz==4'd8/*8:US*/))  xpc10nz <= 3'd7/*7:xpc10nz*/;
                  if ((xpc10nz==4'd9/*9:US*/))  xpc10nz <= 4'd10/*10:xpc10nz*/;
                  if ((xpc10nz==4'd14/*14:US*/))  xpc10nz <= 4'd12/*12:xpc10nz*/;
                  if ((xpc10nz==5'd16/*16:US*/))  xpc10nz <= 4'd11/*11:xpc10nz*/;
                  if ((xpc10nz==5'd18/*18:US*/))  xpc10nz <= 5'd19/*19:xpc10nz*/;
                  if ((xpc10nz==5'd19/*19:US*/))  xpc10nz <= 5'd17/*17:xpc10nz*/;
                   end 
              //End structure HPR primesya.exe


       end 
      

//Resource=SRAM i_@_BOOL/CC/SCALbx10_ARA0 1000x1 clk=posedge(clk) synchronous/pipeline=1 ports=1 <NONE>
always @(*) A_BOOL_CC_SCALbx10_ARA0_RDD0 = A_BOOL_CC_SCALbx10_ARA0[A_BOOL_CC_SCALbx10_ARA0_registered_AD0];

// 1 vectors of width 5
// 6 vectors of width 1
// 2 vectors of width 32
// 1000 array locations of width 1
// 128 bits in scalar variables
// Total state bits in module = 1203 bits.
// 68 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

// 
/*

// Highest off-chip SRAM/DRAM location in use on port dram0bank is <null> (--not-used--) bytes=1048576
// res3: Thread=xpc10 state=X1:"1:xpc10:1"
*-----+-----+---------+---------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                |
*-----+-----+---------+---------------------------------------------------------------------*
| 18  | -   | R0 ctrl |                                                                     |
| 18  | 0   | R0 DATA |                                                                     |
| 18  | 0   | W0 DATA | EXEC ;@_BOOL/CC/SCALbx10_ARA0 end=18 write args=0, 0, C1u(0<volume) |
| 19  | 0   | W1 DATA |                                                                     |
*-----+-----+---------+---------------------------------------------------------------------*

// res3: Thread=xpc10 state=X2:"2:xpc10:2"
*-----+-----+---------+------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                         |
*-----+-----+---------+------------------------------------------------------------------------------*
| 17  | -   | R0 ctrl |                                                                              |
| 17  | 0   | R0 DATA |                                                                              |
| 17  | 0   | W0 DATA | EXEC ;count end=17 scwrite args=Cu(0);pTMT4Main_V_1 end=17 scwrite args=C(0) |
*-----+-----+---------+------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X4:"4:xpc10:4"
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                       |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
| 12  | -   | R0 ctrl |                                                                                                                                                            |
| 12  | 0   | R0 DATA | @_BOOL/CC/SCALbx10_ARA0 end=12 read args=0, 1*pTMT4Main_V_1                                                                                                |
| 13  | 0   | R1 DATA |                                                                                                                                                            |
| 13  | 0   | W0 DATA | EXEC ;@_BOOL/CC/SCALbx10_ARA0 end=13 write args=0, pTMT4Main_V_1, C1u(1);pTMT4Main_V_1 end=13 scwrite args=C(1+pTMT4Main_V_1) W/P:Setting initial arra...  |
| 14  | 0   | W1 DATA |                                                                                                                                                            |
| 12  | 1   | R0 DATA | @_BOOL/CC/SCALbx10_ARA0 end=12 read args=0, 1*pTMT4Main_V_1                                                                                                |
| 15  | 1   | R1 DATA |                                                                                                                                                            |
| 15  | 1   | W0 DATA | EXEC ;@_BOOL/CC/SCALbx10_ARA0 end=15 write args=0, pTMT4Main_V_1, C1u(1);pTMT4Main_V_2 end=15 scwrite args=C(2);pTMT4Main_V_1 end=15 scwrite args=C(1+pTM\ |
|     |     |         | T4Main_V_1);hprKppMarkres12 end=15 scwrite args=E5 W/P:Setting initial arra...                                                                             |
| 16  | 1   | W1 DATA |                                                                                                                                                            |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X8:"8:xpc10:8"
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                               |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| 11  | -   | R0 ctrl |                                                                                                                                                                    |
| 11  | 0   | R0 DATA |                                                                                                                                                                    |
| 11  | 0   | W0 DATA | EXEC ;pTMT4Main_V_3 end=11 scwrite args=C(pTMT4Main_V_2);pTMT4Main_V_4 end=11 scwrite args=C(0);hprKppMarkres14 end=11 scwrite args=E3 W/P:GSAI:hpr_writeln:$$A... |
| 11  | 1   | R0 DATA |                                                                                                                                                                    |
| 11  | 1   | W0 DATA | EXEC ;pTMT4Main_V_3 end=11 scwrite args=C(pTMT4Main_V_2) W/P:Cross off %u %u   (c...                                                                               |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X16:"16:xpc10:16"
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                         |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------*
| 9   | -   | R0 ctrl | @_BOOL/CC/SCALbx10_ARA0 end=9 read args=0, 1*pTMT4Main_V_4                                                                   |
| 10  | -   | R1 ctrl |                                                                                                                              |
| 10  | 0   | R1 DATA |                                                                                                                              |
| 10  | 0   | W0 DATA | EXEC ;count end=10 scwrite args=Cu(1+count);pTMT4Main_V_4 end=10 scwrite args=C(1+pTMT4Main_V_4) W/P:Tally counting %u %d... |
| 10  | 1   | R1 DATA |                                                                                                                              |
| 10  | 1   | W0 DATA | EXEC ;count end=10 scwrite args=Cu(1+count);pTMT4Main_V_4 end=10 scwrite args=C(1+pTMT4Main_V_4) W/P:Optimisation variant... |
| 10  | 2   | R1 DATA |                                                                                                                              |
| 10  | 2   | W0 DATA | EXEC ;pTMT4Main_V_4 end=10 scwrite args=C(1+pTMT4Main_V_4) W/P:Optimisation variant...                                       |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X32:"32:xpc10:32"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 8   | -   | R0 ctrl |       |
| 8   | 0   | R0 DATA |       |
| 8   | 0   | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc10 state=X64:"64:xpc10:64"
*-----+-----+---------+---------------------------------------------*
| npc | eno | Phaser  | Work                                        |
*-----+-----+---------+---------------------------------------------*
| 7   | -   | R0 ctrl |                                             |
| 7   | 0   | R0 DATA |                                             |
| 7   | 0   | W0 DATA | EXEC ;hprKppMarkres16 end=7 scwrite args=E4 |
*-----+-----+---------+---------------------------------------------*

// res3: Thread=xpc10 state=X128:"128:xpc10:128"
*-----+-----+---------+------------------------------------------*
| npc | eno | Phaser  | Work                                     |
*-----+-----+---------+------------------------------------------*
| 6   | -   | R0 ctrl |                                          |
| 6   | 0   | R0 DATA |                                          |
| 6   | 0   | W0 DATA | EXEC ;finished end=6 scwrite args=C1u(1) |
*-----+-----+---------+------------------------------------------*

// res3: Thread=xpc10 state=X256:"256:xpc10:256"
*-----+-----+---------+------*
| npc | eno | Phaser  | Work |
*-----+-----+---------+------*
| 5   | -   | R0 ctrl |      |
*-----+-----+---------+------*

// res3: Thread=xpc10 state=X1024:"1024:xpc10:1024"
*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                                            |
*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| 1   | -   | R0 ctrl |                                                                                                                                                                                 |
| 1   | 0   | R0 DATA |                                                                                                                                                                                 |
| 1   | 0   | W0 DATA | EXEC ;pTMT4Main_V_3 end=1 scwrite args=E2;@_BOOL/CC/SCALbx10_ARA0 end=1 write args=0, pTMT4Main_V_3, C1u(0) W/P:Cross off %u %u   (c...                                         |
| 2   | 0   | W1 DATA |                                                                                                                                                                                 |
| 1   | 1   | R0 DATA |                                                                                                                                                                                 |
| 1   | 1   | W0 DATA | EXEC ;pTMT4Main_V_3 end=1 scwrite args=E2;@_BOOL/CC/SCALbx10_ARA0 end=1 write args=0, pTMT4Main_V_3, C1u(0);pTMT4Main_V_4 end=1 scwrite args=C(0);hprKppMarkres14 end=1 scwrit\ |
|     |     |         | e args=E3;pTMT4Main_V_2 end=1 scwrite args=C(1+pTMT4Main_V_2) W/P:GSAI:hpr_writeln:$$A...                                                                                       |
| 3   | 1   | W1 DATA |                                                                                                                                                                                 |
| 1   | 2   | R0 DATA |                                                                                                                                                                                 |
| 1   | 2   | W0 DATA | EXEC ;pTMT4Main_V_3 end=1 scwrite args=E2;@_BOOL/CC/SCALbx10_ARA0 end=1 write args=0, pTMT4Main_V_3, C1u(0);pTMT4Main_V_2 end=1 scwrite args=C(1+pTMT4Main_V_2)                 |
| 4   | 2   | W1 DATA |                                                                                                                                                                                 |
*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X0:"0:xpc10:start0"
*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                            |
*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------------*
| 0   | -   | R0 ctrl |                                                                                                                                                                 |
| 0   | 0   | R0 DATA |                                                                                                                                                                 |
| 0   | 0   | W0 DATA | EXEC ;hprKppMarkres10 end=0 scwrite args=E1;elimit end=0 scwrite args=C(1000);finished end=0 scwrite args=C1u(0);edesign end=0 scwrite args=C(4032);evariant e\ |
|     |     |         | nd=0 scwrite args=C(0);count end=0 scwrite args=Cu(0) W/P:GSAI:hpr_writeln:$$A...                                                                               |
*-----+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------------------------------*

// Restructure Technology Settings
*------------------------+---------+---------------------------------------------------------------------------------*
| Key                    | Value   | Description                                                                     |
*------------------------+---------+---------------------------------------------------------------------------------*
| int_flr_mul            | 16000   |                                                                                 |
| fp_fl_dp_div           | 5       |                                                                                 |
| fp_fl_dp_add           | 5       |                                                                                 |
| fp_fl_dp_mul           | 5       |                                                                                 |
| fp_fl_sp_div           | 5       |                                                                                 |
| fp_fl_sp_add           | 5       |                                                                                 |
| fp_fl_sp_mul           | 5       |                                                                                 |
| max_no_fp_muls         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
| max_no_fp_muls         | 6       | Maximum number of f/p dividers to instantiate per thread.                       |
| max_no_int_muls        | 3       | Maximum number of int multipliers to instantiate per thread.                    |
| max_no_fp_divs         | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
| max_no_int_divs        | 2       | Maximum number of int dividers to instantiate per thread.                       |
| res2-offchip-threshold | 1000000 |                                                                                 |
| res2-combram-threshold | 32      |                                                                                 |
| res2-regfile-threshold | 8       |                                                                                 |
*------------------------+---------+---------------------------------------------------------------------------------*

// Offchip Memory Physical Ports/Banks
*-----------+----------+----------+--------+--------+-------+-----------*
| Name      | Protocol | No Words | Awidth | Dwidth | Lanes | LaneWidth |
*-----------+----------+----------+--------+--------+-------+-----------*
| dram0bank | HFAST1   | 4194304  | 22     | 256    | 32    | 8         |
*-----------+----------+----------+--------+--------+-------+-----------*

// */

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
