

// CBG Orangepath HPR L/S System

// Verilog output file generated at 30/08/2015 11:11:23
// KiwiC (.net/CIL/C# to Verilog/SystemC compiler): Version alpha 55c: 10-Jan-2015 Unix 3.11.10.301
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-resets=synchronous -bevelab-default-pause-mode=hard -verilog-roundtrip=disable primesya.exe -vnl=offchip.v -vnl-rootmodname=DUT -res2-offchip-threshold=90 -diosim-vcd=vcd.vcd

module DUT(    input [31:0] volume,
    output reg [31:0] count,
    output reg signed [31:0] elimit,
    output reg finished,
    output reg hf1_dram0bank_opreq,
    input hf1_dram0bank_oprdy,
    input hf1_dram0bank_ack,
    output reg hf1_dram0bank_rwbar,
    output reg [255:0] hf1_dram0bank_wdata,
    output reg [21:0] hf1_dram0bank_addr,
    input [255:0] hf1_dram0bank_rdata,
    output reg [31:0] hf1_dram0bank_lanes,
    input clk,
    input reset);
  wire clk;
  wire reset;
  wire [63:0] tnow;
  integer primesya_count1;
  integer pTMT4Main_V_1;
  integer pTMT4Main_V_2;
  integer pTMT4Main_V_3;
  integer pTMT4Main_V_5;
  reg A_BOOL_CC_A_BOOL_CC_BASEn0nA_BOOL_CC_SOL[99:0];
  wire [8:0] xpc10;
  wire [31:0] isMULTIPLIER10_RR;
  reg [31:0] isMULTIPLIER10_NN;
  reg [31:0] isMULTIPLIER10_DD;
  wire isMULTIPLIER10_err;
  reg xpc10_trk2;
  reg xpc10_trk1;
  reg xpc10_trk0;
  reg xpc10_stall;
  reg xpc10_clear;
  reg [31:0] isMULTIPLIER10RRh10hold;
  reg isMULTIPLIER10RRh10shot0;
  reg isMULTIPLIER10RRh10shot1;
  reg BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld;
  reg BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold;
  reg [4:0] xpc10nz;
 always   @(* )  begin 
      //Start HPR primesya.exe
       isMULTIPLIER10_NN = 0;
       isMULTIPLIER10_DD = 0;
       hf1_dram0bank_addr = 0;
       hf1_dram0bank_wdata = 0;
       hf1_dram0bank_rwbar = 0;
       hf1_dram0bank_opreq = 0;
       hf1_dram0bank_lanes = 0;
      if (!xpc10_stall)  begin 
               hf1_dram0bank_lanes = ((xpc10nz==5'd17/*17:US*/)? 32'h1: ((xpc10nz==4'd13/*13:US*/)? (32'h1<<(pTMT4Main_V_1%6'd32)): ((xpc10nz
              ==1'd1/*1:US*/)? (32'h1<<(pTMT4Main_V_3%6'd32)): 32'd0)));

               hf1_dram0bank_opreq = (xpc10nz==4'd13/*13:US*/) || (xpc10nz==4'd10/*10:US*/) || (xpc10nz==1'd1/*1:US*/) || (xpc10nz==4'd8
              /*8:US*/) || (xpc10nz==5'd17/*17:US*/);

               end 
              if ((xpc10nz==1'd1/*1:US*/))  begin 
              if ((pTMT4Main_V_2<7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && !xpc10_stall)  begin 
                       hf1_dram0bank_rwbar = 1'd0;
                       hf1_dram0bank_wdata = 256'd0;
                       hf1_dram0bank_addr = (pTMT4Main_V_3>>>5);
                       end 
                      if ((pTMT4Main_V_2>=7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && !xpc10_stall)  begin 
                       hf1_dram0bank_rwbar = 1'd0;
                       hf1_dram0bank_wdata = 256'd0;
                       hf1_dram0bank_addr = (pTMT4Main_V_3>>>5);
                       end 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3<7'd100) && !xpc10_stall)  begin 
                       hf1_dram0bank_rwbar = 1'd0;
                       hf1_dram0bank_wdata = 256'd0;
                       hf1_dram0bank_addr = (pTMT4Main_V_3>>>5);
                       end 
                       end 
              if (!xpc10_stall) 
          case (xpc10nz)

          4'd8/*8:US*/:  begin 
               hf1_dram0bank_rwbar = 1'd1;
               hf1_dram0bank_addr = (pTMT4Main_V_5>>>5);
               end 
              
          4'd10/*10:US*/:  begin 
               hf1_dram0bank_rwbar = 1'd1;
               hf1_dram0bank_addr = (pTMT4Main_V_2>>>5);
               isMULTIPLIER10_DD = pTMT4Main_V_2;
               isMULTIPLIER10_NN = pTMT4Main_V_2;
               end 
              endcase
          if ((xpc10nz==4'd13/*13:US*/))  begin 
              if ((pTMT4Main_V_1>=7'd99) && !xpc10_stall)  begin 
                       hf1_dram0bank_rwbar = 1'd0;
                       hf1_dram0bank_wdata = 256'h101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101;
                       hf1_dram0bank_addr = (pTMT4Main_V_1>>>5);
                       end 
                      if ((pTMT4Main_V_1<7'd99) && !xpc10_stall)  begin 
                       hf1_dram0bank_rwbar = 1'd0;
                       hf1_dram0bank_wdata = 256'h101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101_0101;
                       hf1_dram0bank_addr = (pTMT4Main_V_1>>>5);
                       end 
                       end 
              if ((xpc10nz==5'd17/*17:US*/) && !xpc10_stall)  begin 
               hf1_dram0bank_rwbar = 1'd0;
               hf1_dram0bank_wdata = (256'hff&(0<volume))|{256'hff&(0<volume), 8'd0}|{256'hff&(0<volume), 16'd0}|{256'hff&(0<volume), 24'd0
              }|{256'hff&(0<volume), 32'd0}|{256'hff&(0<volume), 40'd0}|{256'hff&(0<volume), 48'd0}|{256'hff&(0<volume), 56'd0}|{256'hff
              &(0<volume), 64'd0}|{256'hff&(0<volume), 72'd0}|{256'hff&(0<volume), 80'd0}|{256'hff&(0<volume), 88'd0}|{256'hff&(0<volume
              ), 96'd0}|{256'hff&(0<volume), 104'd0}|{256'hff&(0<volume), 112'd0}|{256'hff&(0<volume), 120'd0}|{256'hff&(0<volume), 128'd0
              }|{256'hff&(0<volume), 136'd0}|{256'hff&(0<volume), 144'd0}|{256'hff&(0<volume), 152'd0}|{256'hff&(0<volume), 160'd0}|{256'hff
              &(0<volume), 168'd0}|{256'hff&(0<volume), 176'd0}|{256'hff&(0<volume), 184'd0}|{256'hff&(0<volume), 192'd0}|{256'hff&(0
              <volume), 200'd0}|{256'hff&(0<volume), 208'd0}|{256'hff&(0<volume), 216'd0}|{256'hff&(0<volume), 224'd0}|{256'hff&(0<volume
              ), 232'd0}|{256'hff&(0<volume), 240'd0}|{256'hff&(0<volume), 248'd0};

               hf1_dram0bank_addr = 22'd0;
               end 
              //End HPR primesya.exe


       end 
      

 always   @(posedge clk )  begin 
      //Start HPR primesya.exe
      if (reset)  begin 
               pTMT4Main_V_1 <= 32'd0;
               isMULTIPLIER10RRh10hold <= 32'd0;
               isMULTIPLIER10RRh10shot1 <= 1'd0;
               BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'd0;
               BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd0;
               pTMT4Main_V_5 <= 32'd0;
               pTMT4Main_V_2 <= 32'd0;
               pTMT4Main_V_3 <= 32'd0;
               count <= 32'd0;
               primesya_count1 <= 32'd0;
               finished <= 1'd0;
               elimit <= 32'd0;
               isMULTIPLIER10RRh10shot0 <= 1'd0;
               xpc10_trk2 <= 1'd0;
               xpc10_trk1 <= 1'd0;
               xpc10_trk0 <= 1'd0;
               xpc10nz <= 5'd0;
               end 
               else  begin 
              if ((xpc10nz==4'd13/*13:US*/) && !xpc10_stall) $display("Setting initial array flag to hold : addr=%d readback=%d", pTMT4Main_V_1
                  , 1'd1);
                  
              case (xpc10nz)

              4'd9/*9:US*/:  begin 
                  if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(!(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32)))) && !xpc10_stall) && (pTMT4Main_V_5<7'd99)) $display("Tally counting %d %d at %d"
                      , pTMT4Main_V_5, 1'd1+count, $time);
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32))) && !xpc10_stall) && (pTMT4Main_V_5<7'd99)) $display("Tally counting %d %d at %d"
                      , pTMT4Main_V_5, count, $time);
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(!(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32)))) && !xpc10_stall) && (pTMT4Main_V_5>=7'd99))  begin 
                          $display("Tally counting %d %d at %d", pTMT4Main_V_5, 1'd1+count, $time);
                          $display("There are %d primes below the natural number %d.", 1'd1+count, 7'd100);
                          $display("(count1 is %d).", primesya_count1);
                           end 
                          if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&
                      (hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_5%6'd32))) && !xpc10_stall) && (pTMT4Main_V_5>=7'd99))  begin 
                          $display("Tally counting %d %d at %d", pTMT4Main_V_5, count, $time);
                          $display("There are %d primes below the natural number %d.", count, 7'd100);
                          $display("(count1 is %d).", primesya_count1);
                           end 
                           end 
                  
              4'd12/*12:US*/:  begin 
                  if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_2%6'd32))) && !xpc10_stall)) $display(" tnow=%d: check back %d = %d ", $time, pTMT4Main_V_2
                      , (BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold: 8'hff&(hf1_dram0bank_rdata>>4'd8
                      *(pTMT4Main_V_2%6'd32))));
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_2%6'd32))) && !xpc10_stall) && (pTMT4Main_V_2>=7'd99)) $display("Now counting");
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                       && !xpc10_stall: (isMULTIPLIER10RRh10hold>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall): (isMULTIPLIER10RRh10shot1
                      ? (isMULTIPLIER10_RR>=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall: (isMULTIPLIER10RRh10hold
                      >=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall)))  begin 
                          $display(" tnow=%d: check back %d = %d ", $time, pTMT4Main_V_2, (BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                          : 8'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32))));
                          $display("Skip out on square");
                          $display("Now counting");
                           end 
                          if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                       && !xpc10_stall: (isMULTIPLIER10RRh10hold<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall): (isMULTIPLIER10RRh10shot1
                      ? (isMULTIPLIER10_RR<7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall: (isMULTIPLIER10RRh10hold
                      <7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall)))  begin 
                          $display(" tnow=%d: check back %d = %d ", $time, pTMT4Main_V_2, (BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                          : 8'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32))));
                          $display("Cross off %d %d   (count1=%d)", pTMT4Main_V_2, (isMULTIPLIER10RRh10shot1? isMULTIPLIER10_RR: isMULTIPLIER10RRh10hold
                          ), 1'd1+primesya_count1);
                           end 
                           end 
                  endcase
              if ((xpc10nz==3'd5/*5:US*/) && !xpc10_stall) $finish(0);
                  if ((xpc10nz==1'd1/*1:US*/))  begin 
                      if ((pTMT4Main_V_2+pTMT4Main_V_3<7'd100) && !xpc10_stall) $display("Cross off %d %d   (count1=%d)", pTMT4Main_V_2
                          , pTMT4Main_V_2+pTMT4Main_V_3, primesya_count1);
                          if ((pTMT4Main_V_2>=7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && !xpc10_stall) $display("Now counting"
                          );
                           end 
                      if ((xpc10nz==0/*0:US*/) && !xpc10_stall) $display("%s%d", "Primes Up To ", 7'd100);
                  
              case (xpc10nz)

              1'd1/*1:US*/:  begin 
                  if ((pTMT4Main_V_2>=7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && !xpc10_stall)  begin 
                           pTMT4Main_V_2 <= 32'd1+pTMT4Main_V_2;
                           pTMT4Main_V_5 <= 32'd0;
                           pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                           end 
                          if ((pTMT4Main_V_2<7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && !xpc10_stall)  begin 
                           pTMT4Main_V_2 <= 32'd1+pTMT4Main_V_2;
                           pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                           end 
                          if ((pTMT4Main_V_2+pTMT4Main_V_3<7'd100) && !xpc10_stall)  pTMT4Main_V_3 <= pTMT4Main_V_2+pTMT4Main_V_3;
                      if ((pTMT4Main_V_2+pTMT4Main_V_3<7'd100) && hf1_dram0bank_oprdy)  xpc10nz <= 2'd2/*2:US*/;
                      if ((pTMT4Main_V_2>=7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && hf1_dram0bank_oprdy)  xpc10nz <= 2'd3/*3:xpc10nz*/;
                      if ((pTMT4Main_V_2<7'd99) && (pTMT4Main_V_2+pTMT4Main_V_3>=7'd100) && hf1_dram0bank_oprdy)  xpc10nz <= 3'd4/*4:xpc10nz*/;
                       end 
                  
              4'd9/*9:US*/:  begin 
                  if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32))) && !xpc10_stall) && (pTMT4Main_V_5>=7'd99))  pTMT4Main_V_5 <= 32'd1+pTMT4Main_V_5
                      ;

                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(!(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32)))) && !xpc10_stall) && (pTMT4Main_V_5>=7'd99))  begin 
                           pTMT4Main_V_5 <= 32'd1+pTMT4Main_V_5;
                           count <= 1'd1+count;
                           end 
                          if ((pTMT4Main_V_5<7'd99) && !xpc10_stall)  pTMT4Main_V_5 <= 32'd1+pTMT4Main_V_5;
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(!(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_5%6'd32)))) && !xpc10_stall) && (pTMT4Main_V_5<7'd99))  count <= 1'd1+count;
                      if ((pTMT4Main_V_5<7'd99) && hf1_dram0bank_ack)  xpc10nz <= 4'd8/*8:xpc10nz*/;
                      if (hf1_dram0bank_ack && (pTMT4Main_V_5>=7'd99))  xpc10nz <= 3'd7/*7:xpc10nz*/;
                       end 
                  
              4'd12/*12:US*/:  begin 
                  if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                       && !xpc10_stall: (isMULTIPLIER10RRh10hold>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall): (isMULTIPLIER10RRh10shot1
                      ? (isMULTIPLIER10_RR>=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall: (isMULTIPLIER10RRh10hold
                      >=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall)))  begin 
                           pTMT4Main_V_5 <= 32'd0;
                           pTMT4Main_V_3 <= (isMULTIPLIER10RRh10shot1? isMULTIPLIER10_RR: isMULTIPLIER10RRh10hold);
                           primesya_count1 <= 1'd1+primesya_count1;
                           end 
                          if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                       && !xpc10_stall: (isMULTIPLIER10RRh10hold<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall): (isMULTIPLIER10RRh10shot1
                      ? (isMULTIPLIER10_RR<7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall: (isMULTIPLIER10RRh10hold
                      <7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))) && !xpc10_stall)))  begin 
                           pTMT4Main_V_3 <= (isMULTIPLIER10RRh10shot1? isMULTIPLIER10_RR: isMULTIPLIER10RRh10hold);
                           primesya_count1 <= 1'd1+primesya_count1;
                           end 
                          if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&
                      (hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32))) && !xpc10_stall) && (pTMT4Main_V_2>=7'd99))  begin 
                           pTMT4Main_V_2 <= 32'd1+pTMT4Main_V_2;
                           pTMT4Main_V_5 <= 32'd0;
                           end 
                          if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold && !xpc10_stall: !(1'hff&
                      (hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32))) && !xpc10_stall) && (pTMT4Main_V_2<7'd99))  pTMT4Main_V_2 <= 32'd1
                      +pTMT4Main_V_2;

                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold: !(1'hff&(hf1_dram0bank_rdata
                      >>4'd8*(pTMT4Main_V_2%6'd32)))) && (pTMT4Main_V_2<7'd99))  xpc10nz <= 4'd10/*10:xpc10nz*/;
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                      : (isMULTIPLIER10RRh10hold>=7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold) || (pTMT4Main_V_2>=7'd99) && !BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                      : (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR>=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2
                      %6'd32)))): (isMULTIPLIER10RRh10hold>=7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32))))) || 
                      (pTMT4Main_V_2>=7'd99) && !(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))))  xpc10nz <= 4'd8/*8:xpc10nz*/;
                      if ((BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld? (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold
                      : (isMULTIPLIER10RRh10hold<7'd100) && BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold): (isMULTIPLIER10RRh10shot1? (isMULTIPLIER10_RR
                      <7'd100) && !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))): (isMULTIPLIER10RRh10hold<7'd100) && 
                      !(!(1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32)))))))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                       end 
                  endcase
              if (!xpc10_stall) 
                  case (xpc10nz)

                  0/*0:US*/:  begin 
                       count <= 32'd0;
                       primesya_count1 <= 32'd0;
                       finished <= 1'd0;
                       elimit <= 32'd100;
                       end 
                      
                  3'd7/*7:US*/:  finished <= 1'd1;

                  5'd16/*16:US*/:  begin 
                       pTMT4Main_V_1 <= 32'd0;
                       count <= 32'd0;
                       primesya_count1 <= 32'd2;
                       end 
                      endcase
                  if ((xpc10==4'd8/*8:US*/) && hf1_dram0bank_ack)  begin 
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_2%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       end 
                      if ((xpc10==5'd16/*16:US*/) && hf1_dram0bank_ack)  begin 
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_5%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_5%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_5%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10hold <= 1'hff&(hf1_dram0bank_rdata>>4'd8*(pTMT4Main_V_5%6'd32));
                       BOOLCCBOOLCCBASEn0nBOOLCCSOLRRh10vld <= 1'd1;
                       end 
                      
              case (xpc10nz)

              0/*0:US*/:  xpc10nz <= 5'd17/*17:xpc10nz*/;

              4'd13/*13:US*/:  begin 
                  if ((pTMT4Main_V_1>=7'd99) && !xpc10_stall)  begin 
                           pTMT4Main_V_1 <= 32'd1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= 32'd2;
                           end 
                          if ((pTMT4Main_V_1<7'd99) && !xpc10_stall)  pTMT4Main_V_1 <= 32'd1+pTMT4Main_V_1;
                      if ((pTMT4Main_V_1<7'd99) && hf1_dram0bank_oprdy)  xpc10nz <= 4'd14/*14:xpc10nz*/;
                      if ((pTMT4Main_V_1>=7'd99) && hf1_dram0bank_oprdy)  xpc10nz <= 4'd15/*15:xpc10nz*/;
                       end 
                  endcase
              if ( isMULTIPLIER10RRh10shot1)  begin 
                       isMULTIPLIER10RRh10hold <= isMULTIPLIER10_RR;
                       isMULTIPLIER10RRh10hold <= isMULTIPLIER10_RR;
                       isMULTIPLIER10RRh10hold <= isMULTIPLIER10_RR;
                       isMULTIPLIER10RRh10hold <= isMULTIPLIER10_RR;
                       isMULTIPLIER10RRh10hold <= isMULTIPLIER10_RR;
                       end 
                      if ((xpc10nz==5'd16/*16:US*/))  xpc10nz <= 4'd13/*13:xpc10nz*/;
                  if (hf1_dram0bank_oprdy)  begin 
                      if ((xpc10nz==4'd8/*8:US*/))  xpc10nz <= 4'd9/*9:xpc10nz*/;
                          if ((xpc10nz==4'd10/*10:US*/))  xpc10nz <= 4'd11/*11:xpc10nz*/;
                          if ((xpc10nz==5'd17/*17:US*/))  xpc10nz <= 5'd18/*18:xpc10nz*/;
                           end 
                      
              case (xpc10nz)

              3'd5/*5:US*/:  xpc10nz <= 3'd5/*5:xpc10nz*/;

              3'd7/*7:US*/:  xpc10nz <= 3'd6/*6:US*/;
              endcase
               isMULTIPLIER10RRh10shot1 <= isMULTIPLIER10RRh10shot0;
               isMULTIPLIER10RRh10shot1 <= isMULTIPLIER10RRh10shot0;
               isMULTIPLIER10RRh10shot1 <= isMULTIPLIER10RRh10shot0;
               isMULTIPLIER10RRh10shot1 <= isMULTIPLIER10RRh10shot0;
               isMULTIPLIER10RRh10shot1 <= isMULTIPLIER10RRh10shot0;
               isMULTIPLIER10RRh10shot0 <= (xpc10nz==4'd10/*10:US*/) && !xpc10_stall;
               xpc10_trk2 <= ((5'd19==xpc10nz) || (5'd18==xpc10nz) || (4'd15==xpc10nz) || (4'd12==xpc10nz) || (4'd10==xpc10nz) || (4'd9
              ==xpc10nz) || (4'd8==xpc10nz) || (3'd7==xpc10nz) || (2'd3==xpc10nz) || (xpc10nz==2'd2/*2:US*/)) && !xpc10_clear && !xpc10_stall
              ;

               xpc10_trk1 <= ((5'd18==xpc10nz) || (5'd17==xpc10nz) || (4'd14==xpc10nz) || (4'd11==xpc10nz) || (4'd9==xpc10nz) || (4'd8
              ==xpc10nz) || (3'd7==xpc10nz) || (3'd6==xpc10nz) || (2'd2==xpc10nz) || (xpc10nz==1'd1/*1:US*/)) && !xpc10_clear && !xpc10_stall
              ;

               xpc10_trk0 <= ((5'd17==xpc10nz) || (5'd16==xpc10nz) || (4'd13==xpc10nz) || (4'd10==xpc10nz) || (4'd8==xpc10nz) || (3'd7
              ==xpc10nz) || (3'd6==xpc10nz) || (3'd5==xpc10nz) || (1'd1==xpc10nz) || (xpc10nz==0/*0:US*/)) && !xpc10_clear && !xpc10_stall
              ;

              if ((xpc10nz==2'd2/*2:US*/))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                  if ((xpc10nz==2'd3/*3:US*/))  xpc10nz <= 4'd8/*8:xpc10nz*/;
                  if ((xpc10nz==3'd4/*4:US*/))  xpc10nz <= 4'd10/*10:xpc10nz*/;
                  if ((xpc10nz==3'd6/*6:US*/))  xpc10nz <= 3'd5/*5:xpc10nz*/;
                  if (hf1_dram0bank_ack)  begin if ((xpc10nz==4'd11/*11:US*/))  xpc10nz <= 4'd12/*12:xpc10nz*/;
                       end 
                  if ((xpc10nz==4'd14/*14:US*/))  xpc10nz <= 4'd13/*13:xpc10nz*/;
                  if ((xpc10nz==4'd15/*15:US*/))  xpc10nz <= 4'd10/*10:xpc10nz*/;
                  if ((xpc10nz==5'd18/*18:US*/))  xpc10nz <= 5'd16/*16:xpc10nz*/;
                   end 
              //End HPR primesya.exe


       end 
      

  CV_INT_FL2_MULTIPLIER_S i102CV_INT_FL2_MULTIPLIER_S(clk, reset, isMULTIPLIER10_RR, isMULTIPLIER10_NN, isMULTIPLIER10_DD, isMULTIPLIER10_err
);
always @(*) xpc10_clear = (xpc10nz==0/*0:US*/) || (xpc10nz==1'd1/*1:US*/) || (xpc10nz==3'd5/*5:US*/) || (xpc10nz==3'd6/*6:US*/) || (xpc10nz==3'd7/*7:US*/) || (xpc10nz
==4'd8/*8:US*/) || (xpc10nz==4'd10/*10:US*/) || (xpc10nz==4'd13/*13:US*/) || (xpc10nz==5'd17/*17:US*/) || (xpc10nz==5'd16/*16:US*/);

always @(*) xpc10_stall = ((xpc10nz==1'd1/*1:US*/) || (xpc10nz==4'd8/*8:US*/) || (xpc10nz==4'd10/*10:US*/) || (xpc10nz==4'd13/*13:US*/) || (xpc10nz==5'd17/*17:US*/)) && 
!hf1_dram0bank_oprdy || ((xpc10nz==4'd9/*9:US*/) || (xpc10nz==4'd11/*11:US*/)) && !hf1_dram0bank_ack;

//Total area 0
// 9 vectors of width 1
// 1 vectors of width 5
// 3 vectors of width 32
// 160 bits in scalar variables
// Total state bits in module = 270 bits.
// 208 continuously assigned (wire/non-state) bits 
//   cell CV_INT_FL2_MULTIPLIER_S count=1
// Total number of leaf cells = 1
endmodule

// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
