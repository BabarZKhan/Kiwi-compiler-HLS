

// CBG Orangepath HPR L/S System

// Verilog output file generated at 23/09/2014 11:31:53
// KiwiC (.net/CIL/C# to Verilog/SystemC compiler): Version alpha 55b: 1st-Sept-2014 Unix 3.16.2.200
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-resets=synchronous -repack=disable -restructure2=disable primesya.exe -sim=1999 -vnl=onchip.v -vnl-rootmodname=DUT -protocol none

module DUT(    input [31:0] volume,
    output reg [31:0] count,
    input start,
    output reg finished,
    input clk,
    input reset);
  integer pTMT4Main_V_0;
  integer pTMT4Main_V_1;
  integer pTMT4Main_V_2;
  integer pTMT4Main_V_3;
  reg [6:0] xpc10;
 always   @(posedge clk )  begin 
      //Start HPR primesya.exe
      if (reset)  begin 
               pTMT4Main_V_0 <= 32'd0;
               count <= 32'd0;
               finished <= 1'd0;
               pTMT4Main_V_3 <= 32'd0;
               xpc10 <= 7'd0;
               pTMT4Main_V_1 <= 32'd0;
               pTMT4Main_V_2 <= 32'd0;

               end 
               else  begin 
              
              case (xpc10)

              0/*0:US*/: $display("%s%d", "Primes Up To ", 100);

              1/*1:US*/: $display("Setting array %d %d", pTMT4Main_V_0, 1);

              2/*2:US*/:  begin 
                  if ((2*pTMT4Main_V_1>=100) && (pTMT4Main_V_1>=99)) $display("Now counting");
                      if ((2*pTMT4Main_V_1<100)) $display("Cross off %d %d", pTMT4Main_V_1, 2*pTMT4Main_V_1);
                      
                   end 
                  
              4/*4:US*/:  begin 
                  if (DBOOL_AX[400000+pTMT4Main_V_3]) $display("Tally counting %d %d", pTMT4Main_V_3, count);
                      if (!(DBOOL_AX[400000+pTMT4Main_V_3]) && (pTMT4Main_V_3>=99))  begin 
                          $display("Tally counting %d %d", pTMT4Main_V_3, count);
                          $display("There are %d primes below the natural number %d.", count, 100);

                           end 
                          if (!(DBOOL_AX[400000+pTMT4Main_V_3]) && (pTMT4Main_V_3<99)) $display("Tally counting %d %d", pTMT4Main_V_3
                      , count);
                      
                   end 
                  endcase
              if ((pTMT4Main_V_3>=99) && (xpc10==16/*16:US*/)) $display("There are %d primes below the natural number %d.", count, 100
                  );
                  
              case (xpc10)

              0/*0:US*/:  begin 
                   xpc10 <= 1/*1:xpc10:1*/;
                   pTMT4Main_V_0 <= 0;
                   count <= 0;
                   finished <= 0;
                   DBOOL_AX[400000+0] <= (0<volume);

                   end 
                  
              1/*1:US*/: if ((pTMT4Main_V_0<99))  begin 
                       pTMT4Main_V_0 <= 1+pTMT4Main_V_0;
                       DBOOL_AX[400000+pTMT4Main_V_0] <= 1;

                       end 
                       else  begin 
                       xpc10 <= 2/*2:xpc10:2*/;
                       pTMT4Main_V_1 <= 2;
                       pTMT4Main_V_0 <= 1+pTMT4Main_V_0;
                       DBOOL_AX[400000+pTMT4Main_V_0] <= 1;

                       end 
                      
              2/*2:US*/:  begin 
                  if ((2*pTMT4Main_V_1>=100) && (pTMT4Main_V_1>=99))  begin 
                           xpc10 <= 4/*4:xpc10:4*/;
                           pTMT4Main_V_3 <= 0;
                           pTMT4Main_V_1 <= 1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= 2*pTMT4Main_V_1;

                           end 
                          if ((2*pTMT4Main_V_1<100))  begin 
                           xpc10 <= 64/*64:xpc10:64*/;
                           pTMT4Main_V_2 <= 2*pTMT4Main_V_1;

                           end 
                          if ((2*pTMT4Main_V_1>=100) && (pTMT4Main_V_1<99))  begin 
                           pTMT4Main_V_1 <= 1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= 2*pTMT4Main_V_1;

                           end 
                          
                   end 
                  
              4/*4:US*/:  begin 
                  if (!(DBOOL_AX[400000+pTMT4Main_V_3]) && (pTMT4Main_V_3>=99))  begin 
                           xpc10 <= 32/*32:xpc10:32*/;
                           pTMT4Main_V_3 <= 1+pTMT4Main_V_3;

                           end 
                          if (DBOOL_AX[400000+pTMT4Main_V_3])  begin 
                           xpc10 <= 8/*8:xpc10:8*/;
                           count <= 1+count;

                           end 
                          if (!(DBOOL_AX[400000+pTMT4Main_V_3]) && (pTMT4Main_V_3<99))  pTMT4Main_V_3 <= 1+pTMT4Main_V_3;
                      
                   end 
                  
              16/*16:US*/: if ((pTMT4Main_V_3<99))  begin 
                       xpc10 <= 4/*4:xpc10:4*/;
                       pTMT4Main_V_3 <= 1+pTMT4Main_V_3;

                       end 
                       else  begin 
                       xpc10 <= 32/*32:xpc10:32*/;
                       pTMT4Main_V_3 <= 1+pTMT4Main_V_3;

                       end 
                      
              32/*32:US*/:  begin 
                  $finish(0);
                   finished <= 1;

                   end 
                  
              64/*64:US*/:  begin 
                  if ((pTMT4Main_V_1>=99) && (pTMT4Main_V_1+pTMT4Main_V_2>=100)) $display("Now counting");
                      if ((pTMT4Main_V_1+pTMT4Main_V_2<100)) $display("Cross off %d %d", pTMT4Main_V_1, pTMT4Main_V_1+pTMT4Main_V_2);
                      if ((pTMT4Main_V_1>=99) && (pTMT4Main_V_1+pTMT4Main_V_2>=100))  begin 
                           xpc10 <= 4/*4:xpc10:4*/;
                           pTMT4Main_V_3 <= 0;
                           pTMT4Main_V_1 <= 1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= pTMT4Main_V_1+pTMT4Main_V_2;
                           DBOOL_AX[400000+pTMT4Main_V_2] <= 0;

                           end 
                          if ((pTMT4Main_V_1<99) && (pTMT4Main_V_1+pTMT4Main_V_2>=100))  begin 
                           xpc10 <= 2/*2:xpc10:2*/;
                           pTMT4Main_V_1 <= 1+pTMT4Main_V_1;
                           pTMT4Main_V_2 <= pTMT4Main_V_1+pTMT4Main_V_2;
                           DBOOL_AX[400000+pTMT4Main_V_2] <= 0;

                           end 
                          if ((pTMT4Main_V_1+pTMT4Main_V_2<100))  begin 
                           pTMT4Main_V_2 <= pTMT4Main_V_1+pTMT4Main_V_2;
                           DBOOL_AX[400000+pTMT4Main_V_2] <= 0;

                           end 
                          
                   end 
                  endcase
              if ((xpc10==8/*8:US*/))  xpc10 <= 16/*16:xpc10:16*/;
                  
               end 
              //End HPR primesya.exe


       end 
      

//Total area 0
// 1 vectors of width 7
// 128 bits in scalar variables
// Total state bits in module = 135 bits.
// Total number of leaf cells = 0
endmodule

// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
