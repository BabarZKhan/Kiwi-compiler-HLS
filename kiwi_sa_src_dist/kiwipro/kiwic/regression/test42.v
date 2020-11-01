

// CBG Orangepath HPR L/S System

// Verilog output file generated at 09/03/2016 16:11:16
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.04: 2-Mar-2016 Unix 3.19.8.100
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-kcode-dump=enable test42.exe -sim 1800 -vnl-resets=synchronous -vnl test42.v -res2-no-dram-ports=0 -vnl-rootmodname=TEST42SLAVE -kiwic-finish=disable -conerefine=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -give-backtrace -report-each-step
`timescale 1ns/10ps


module TEST42SLAVE(    input signed [31:0] test42_pioRegfileRead_addr,
    output reg signed [31:0] test42_pioRegfileRead_return,
    input test42_pioRegfileRead_req,
    output reg test42_pioRegfileRead_ack,
    output reg signed [31:0] test42_pioRegfileWrite_return,
    input signed [31:0] test42_pioRegfileWrite_data,
    input signed [31:0] test42_pioRegfileWrite_addr,
    input test42_pioRegfileWrite_req,
    output reg test42_pioRegfileWrite_ack,
    input clk,
    input reset);
  reg signed [31:0] test42_pio_slave_reg0;
  reg signed [31:0] test42_pio_slave_reg1;
  reg signed [31:0] test42_pio_slave_reg2;
  reg signed [31:0] CS0_3_refxxarray10;
  reg signed [31:0] tTMT4Main_V_0;
  integer tTpT4pioRegfileWrite_V_0;
  integer tTpT4pioRegfileRead_V_0;
  wire [31:0] ktop14;
  wire [31:0] ktop12;
  reg [31:0] KiKiwi_old_pausemode_value;
  wire [31:0] ktop10;
  integer mpc14;
  integer mpc12;
  integer mpc10;
  reg [3:0] xpc14nz;
  reg [3:0] xpc12nz;
  reg [2:0] xpc10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test42.exe
      if (reset)  begin 
               tTpT4pioRegfileRead_V_0 <= 32'd0;
               test42_pioRegfileRead_return <= 32'd0;
               test42_pioRegfileRead_ack <= 1'd0;
               KiKiwi_old_pausemode_value <= 32'd0;
               xpc14nz <= 4'd0;
               tTpT4pioRegfileWrite_V_0 <= 32'd0;
               test42_pioRegfileWrite_return <= 32'd0;
               test42_pioRegfileWrite_ack <= 1'd0;
               xpc12nz <= 4'd0;
               test42_pio_slave_reg0 <= 32'd0;
               test42_pio_slave_reg1 <= 32'd0;
               test42_pio_slave_reg2 <= 32'd0;
               CS0_3_refxxarray10 <= 32'd0;
               tTMT4Main_V_0 <= 32'd0;
               xpc10nz <= 3'd0;
               end 
               else  begin 
              if ((xpc14nz==3'd4/*4:US*/)) $display("pioRegfileRead addr=%1d data=0x%h.", test42_pioRegfileRead_addr, tTpT4pioRegfileRead_V_0
                  );
                  if (test42_pioRegfileWrite_req && (xpc12nz==4'd8/*8:US*/)) $display("pioRegfileWrite addr=%1d data=0x%h.", test42_pioRegfileWrite_addr
                  , 32'hffffffff&test42_pioRegfileWrite_data);
                  
              case (xpc10nz)
                  0/*0:US*/: $display("test42");

                  3'd4/*4:US*/: $display("The value of the integer: %1d", 7'd101);
              endcase
              if (test42_pioRegfileWrite_req) 
                  case (xpc12nz)
                      2'd2/*2:US*/:  xpc12nz <= 1'd1/*1:xpc12nz*/;

                      4'd8/*8:US*/:  xpc12nz <= 3'd6/*6:xpc12nz*/;
                  endcase
                   else 
                  case (xpc12nz)
                      2'd2/*2:US*/:  begin 
                           test42_pioRegfileWrite_ack <= 1'd0;
                           xpc12nz <= 0/*0:xpc12nz*/;
                           end 
                          
                      4'd8/*8:US*/:  xpc12nz <= 3'd7/*7:xpc12nz*/;
                  endcase
              if ((xpc12nz==4'd8/*8:US*/))  begin 
                      if (test42_pioRegfileWrite_req && (test42_pioRegfileWrite_addr==4'd8/*8:US*/))  begin 
                               tTpT4pioRegfileWrite_V_0 <= test42_pioRegfileWrite_data;
                               test42_pio_slave_reg0 <= test42_pioRegfileWrite_data;
                               end 
                              if (test42_pioRegfileWrite_req && (test42_pioRegfileWrite_addr!=4'd8/*8:US*/))  tTpT4pioRegfileWrite_V_0
                           <= test42_pioRegfileWrite_data;

                           end 
                      if ((xpc14nz==0/*0:US*/))  begin 
                       KiKiwi_old_pausemode_value <= 32'd2;
                       test42_pio_slave_reg0 <= 64'd100;
                       test42_pio_slave_reg1 <= 64'd444;
                       test42_pio_slave_reg2 <= 64'h_2710;
                       xpc14nz <= 4'd10/*10:xpc14nz*/;
                       end 
                      if (test42_pioRegfileRead_req) 
                  case (xpc14nz)
                      2'd2/*2:US*/:  xpc14nz <= 1'd1/*1:xpc14nz*/;

                      4'd8/*8:US*/:  xpc14nz <= 3'd6/*6:xpc14nz*/;
                  endcase
                   else 
                  case (xpc14nz)
                      2'd2/*2:US*/:  begin 
                           test42_pioRegfileRead_ack <= 1'd0;
                           xpc14nz <= 4'd10/*10:xpc14nz*/;
                           end 
                          
                      4'd8/*8:US*/:  xpc14nz <= 3'd7/*7:xpc14nz*/;
                  endcase

              case (xpc10nz)
                  0/*0:US*/:  begin 
                       CS0_3_refxxarray10 <= 32'd0;
                       tTMT4Main_V_0 <= 32'd0;
                       xpc10nz <= 3'd4/*4:xpc10nz*/;
                       end 
                      
                  2'd3/*3:US*/:  begin 
                       test42_pio_slave_reg0 <= 64'd1024;
                       test42_pio_slave_reg1 <= 64'd1025;
                       test42_pio_slave_reg2 <= 64'd1026;
                       xpc10nz <= 2'd2/*2:xpc10nz*/;
                       end 
                      endcase

              case (xpc14nz)
                  3'd4/*4:US*/:  begin 
                       test42_pioRegfileRead_return <= tTpT4pioRegfileRead_V_0;
                       test42_pioRegfileRead_ack <= 1'd1;
                       xpc14nz <= 2'd3/*3:xpc14nz*/;
                       end 
                      
                  4'd8/*8:US*/:  begin 
                      if (test42_pioRegfileRead_req && (test42_pioRegfileRead_addr==4'd8/*8:US*/))  tTpT4pioRegfileRead_V_0 <= test42_pio_slave_reg0
                          ;

                          if (test42_pioRegfileRead_req && (test42_pioRegfileRead_addr!=4'd8/*8:US*/))  tTpT4pioRegfileRead_V_0 <= 64'h1_e240
                          ;

                           end 
                      endcase
              if ((xpc12nz==3'd4/*4:US*/))  begin 
                       test42_pioRegfileWrite_return <= 32'd505;
                       test42_pioRegfileWrite_ack <= 1'd1;
                       xpc12nz <= 2'd3/*3:xpc12nz*/;
                       end 
                      if ((xpc10nz==3'd4/*4:US*/))  xpc10nz <= 2'd3/*3:xpc10nz*/;
                  
              case (xpc12nz)
                  3'd5/*5:US*/:  begin 
                      if ((test42_pioRegfileWrite_addr==5'd24/*24:US*/))  test42_pio_slave_reg2 <= tTpT4pioRegfileWrite_V_0;
                           xpc12nz <= 3'd4/*4:xpc12nz*/;
                       end 
                      
                  3'd6/*6:US*/:  begin 
                      if ((test42_pioRegfileWrite_addr==5'd16/*16:US*/))  test42_pio_slave_reg1 <= tTpT4pioRegfileWrite_V_0;
                           xpc12nz <= 3'd5/*5:xpc12nz*/;
                       end 
                      endcase

              case (xpc14nz)
                  3'd5/*5:US*/:  begin 
                      if ((test42_pioRegfileRead_addr==5'd24/*24:US*/))  tTpT4pioRegfileRead_V_0 <= test42_pio_slave_reg2;
                           xpc14nz <= 3'd4/*4:xpc14nz*/;
                       end 
                      
                  3'd6/*6:US*/:  begin 
                      if ((test42_pioRegfileRead_addr==5'd16/*16:US*/))  tTpT4pioRegfileRead_V_0 <= 32'd256+test42_pio_slave_reg1;
                           xpc14nz <= 3'd5/*5:xpc14nz*/;
                       end 
                      endcase
              if ((xpc14nz==1'd1/*1:US*/))  xpc14nz <= 2'd2/*2:xpc14nz*/;
                  if ((xpc14nz==2'd3/*3:US*/))  xpc14nz <= 2'd2/*2:xpc14nz*/;
                  if ((xpc14nz==3'd7/*7:US*/))  xpc14nz <= 4'd8/*8:xpc14nz*/;
                  if ((xpc14nz==4'd9/*9:US*/))  xpc14nz <= 4'd8/*8:xpc14nz*/;
                  if ((xpc14nz==4'd10/*10:US*/))  xpc14nz <= 4'd9/*9:xpc14nz*/;
                  if ((xpc12nz==0/*0:US*/))  xpc12nz <= 4'd9/*9:xpc12nz*/;
                  if ((xpc12nz==1'd1/*1:US*/))  xpc12nz <= 2'd2/*2:xpc12nz*/;
                  if ((xpc12nz==2'd3/*3:US*/))  xpc12nz <= 2'd2/*2:xpc12nz*/;
                  if ((xpc12nz==3'd7/*7:US*/))  xpc12nz <= 4'd8/*8:xpc12nz*/;
                  if ((xpc12nz==4'd9/*9:US*/))  xpc12nz <= 4'd8/*8:xpc12nz*/;
                  if ((xpc10nz==1'd1/*1:US*/))  xpc10nz <= 2'd2/*2:xpc10nz*/;
                  if ((xpc10nz==2'd2/*2:US*/))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                   end 
              //End structure HPR test42.exe


       end 
      

// 1 vectors of width 3
// 2 vectors of width 4
// 6 vectors of width 32
// 64 bits in scalar variables
// Total state bits in module = 267 bits.
// 192 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

// 
/*

// res3: Thread=xpc14 state=X1:"1:xpc14:1"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 10  | -   | R0 CTRL |       |
| 10  | 935 | R0 DATA |       |
| 10  | 935 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc14 state=X2:"2:xpc14:2"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 9   | -   | R0 CTRL |       |
| 9   | 934 | R0 DATA |       |
| 9   | 934 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc14 state=X4:"4:xpc14:4"
*-----+-----+---------+-------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                          |
*-----+-----+---------+-------------------------------------------------------------------------------*
| 8   | -   | R0 CTRL |                                                                               |
| 8   | 931 | R0 DATA |                                                                               |
| 8   | 931 | W0 DATA | EXEC                                                                          |
| 8   | 932 | R0 DATA |                                                                               |
| 8   | 932 | W0 DATA | EXEC ;tTpT4pioRegfileRead_V_0 te=8-te-8 scalarw args=C(test42_pio_slave_reg0) |
| 8   | 933 | R0 DATA |                                                                               |
| 8   | 933 | W0 DATA | EXEC ;tTpT4pioRegfileRead_V_0 te=8-te-8 scalarw args=C64(123456)              |
*-----+-----+---------+-------------------------------------------------------------------------------*

// res3: Thread=xpc14 state=X8:"8:xpc14:8"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 7   | -   | R0 CTRL |       |
| 7   | 930 | R0 DATA |       |
| 7   | 930 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc14 state=X16:"16:xpc14:16"
*-----+-----+---------+---------------------------------------------------------*
| npc | eno | Phaser  | Work                                                    |
*-----+-----+---------+---------------------------------------------------------*
| 6   | -   | R0 CTRL |                                                         |
| 6   | 928 | R0 DATA |                                                         |
| 6   | 928 | W0 DATA | EXEC ;tTpT4pioRegfileRead_V_0 te=6-te-6 scalarw args=E3 |
| 6   | 929 | R0 DATA |                                                         |
| 6   | 929 | W0 DATA | EXEC                                                    |
*-----+-----+---------+---------------------------------------------------------*

// res3: Thread=xpc14 state=X32:"32:xpc14:32"
*-----+-----+---------+-------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                          |
*-----+-----+---------+-------------------------------------------------------------------------------*
| 5   | -   | R0 CTRL |                                                                               |
| 5   | 926 | R0 DATA |                                                                               |
| 5   | 926 | W0 DATA | EXEC ;tTpT4pioRegfileRead_V_0 te=5-te-5 scalarw args=C(test42_pio_slave_reg2) |
| 5   | 927 | R0 DATA |                                                                               |
| 5   | 927 | W0 DATA | EXEC                                                                          |
*-----+-----+---------+-------------------------------------------------------------------------------*

// res3: Thread=xpc14 state=X64:"64:xpc14:64"
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                             |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*
| 4   | -   | R0 CTRL |                                                                                                                                                                  |
| 4   | 925 | R0 DATA |                                                                                                                                                                  |
| 4   | 925 | W0 DATA | EXEC ;test42/pioRegfileRead_ack te=4-te-4 scalarw args=1;test42/pioRegfileRead_return te=4-te-4 scalarw args=tTpT4pioRegfileRead_V_0 W/P:pioRegfileRead addr=... |
*-----+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc14 state=X128:"128:xpc14:128"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 3   | -   | R0 CTRL |       |
| 3   | 924 | R0 DATA |       |
| 3   | 924 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc14 state=X256:"256:xpc14:256"
*-----+-----+---------+----------------------------------------------------------*
| npc | eno | Phaser  | Work                                                     |
*-----+-----+---------+----------------------------------------------------------*
| 2   | -   | R0 CTRL |                                                          |
| 2   | 922 | R0 DATA |                                                          |
| 2   | 922 | W0 DATA | EXEC                                                     |
| 2   | 923 | R0 DATA |                                                          |
| 2   | 923 | W0 DATA | EXEC ;test42/pioRegfileRead_ack te=2-te-2 scalarw args=0 |
*-----+-----+---------+----------------------------------------------------------*

// res3: Thread=xpc14 state=X512:"512:xpc14:512"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 1   | -   | R0 CTRL |       |
| 1   | 921 | R0 DATA |       |
| 1   | 921 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc14 state=X0:"0:xpc14:start0"
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                                   |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*
| 0   | -   | R0 CTRL |                                                                                                                                                        |
| 0   | 920 | R0 DATA |                                                                                                                                                        |
| 0   | 920 | W0 DATA | EXEC ;test42_pio_slave_reg2 te=0-te-0 scalarw args=C64(10000);test42_pio_slave_reg1 te=0-te-0 scalarw args=C64(444);test42_pio_slave_reg0 te=0-te-0 s\ |
|     |     |         | calarw args=C64(100);KiKiwi_old_pausemode_value te=0-te-0 scalarw args=Cu(2)                                                                           |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc12 state=X1:"1:xpc12:1"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 9   | -   | R0 CTRL |       |
| 9   | 919 | R0 DATA |       |
| 9   | 919 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc12 state=X2:"2:xpc12:2"
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                 |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*
| 8   | -   | R0 CTRL |                                                                                                                                      |
| 8   | 916 | R0 DATA |                                                                                                                                      |
| 8   | 916 | W0 DATA | EXEC                                                                                                                                 |
| 8   | 917 | R0 DATA |                                                                                                                                      |
| 8   | 917 | W0 DATA | EXEC ;test42_pio_slave_reg0 te=8-te-8 scalarw args=E2;tTpT4pioRegfileWrite_V_0 te=8-te-8 scalarw args=E2 W/P:pioRegfileWrite addr... |
| 8   | 918 | R0 DATA |                                                                                                                                      |
| 8   | 918 | W0 DATA | EXEC ;tTpT4pioRegfileWrite_V_0 te=8-te-8 scalarw args=E2 W/P:pioRegfileWrite addr...                                                 |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc12 state=X4:"4:xpc12:4"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 7   | -   | R0 CTRL |       |
| 7   | 915 | R0 DATA |       |
| 7   | 915 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc12 state=X8:"8:xpc12:8"
*-----+-----+---------+-------------------------------------------------------*
| npc | eno | Phaser  | Work                                                  |
*-----+-----+---------+-------------------------------------------------------*
| 6   | -   | R0 CTRL |                                                       |
| 6   | 913 | R0 DATA |                                                       |
| 6   | 913 | W0 DATA | EXEC ;test42_pio_slave_reg1 te=6-te-6 scalarw args=E1 |
| 6   | 914 | R0 DATA |                                                       |
| 6   | 914 | W0 DATA | EXEC                                                  |
*-----+-----+---------+-------------------------------------------------------*

// res3: Thread=xpc12 state=X16:"16:xpc12:16"
*-----+-----+---------+-------------------------------------------------------*
| npc | eno | Phaser  | Work                                                  |
*-----+-----+---------+-------------------------------------------------------*
| 5   | -   | R0 CTRL |                                                       |
| 5   | 911 | R0 DATA |                                                       |
| 5   | 911 | W0 DATA | EXEC ;test42_pio_slave_reg2 te=5-te-5 scalarw args=E1 |
| 5   | 912 | R0 DATA |                                                       |
| 5   | 912 | W0 DATA | EXEC                                                  |
*-----+-----+---------+-------------------------------------------------------*

// res3: Thread=xpc12 state=X32:"32:xpc12:32"
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                               |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*
| 4   | -   | R0 CTRL |                                                                                                                    |
| 4   | 910 | R0 DATA |                                                                                                                    |
| 4   | 910 | W0 DATA | EXEC ;test42/pioRegfileWrite_ack te=4-te-4 scalarw args=1;test42/pioRegfileWrite_return te=4-te-4 scalarw args=505 |
*-----+-----+---------+--------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc12 state=X64:"64:xpc12:64"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 3   | -   | R0 CTRL |       |
| 3   | 909 | R0 DATA |       |
| 3   | 909 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc12 state=X128:"128:xpc12:128"
*-----+-----+---------+-----------------------------------------------------------*
| npc | eno | Phaser  | Work                                                      |
*-----+-----+---------+-----------------------------------------------------------*
| 2   | -   | R0 CTRL |                                                           |
| 2   | 907 | R0 DATA |                                                           |
| 2   | 907 | W0 DATA | EXEC                                                      |
| 2   | 908 | R0 DATA |                                                           |
| 2   | 908 | W0 DATA | EXEC ;test42/pioRegfileWrite_ack te=2-te-2 scalarw args=0 |
*-----+-----+---------+-----------------------------------------------------------*

// res3: Thread=xpc12 state=X256:"256:xpc12:256"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 1   | -   | R0 CTRL |       |
| 1   | 906 | R0 DATA |       |
| 1   | 906 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc12 state=X0:"0:xpc12:start0"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 0   | -   | R0 CTRL |       |
| 0   | 905 | R0 DATA |       |
| 0   | 905 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc10 state=X1:"1:xpc10:1"
*-----+-----+---------+-----------------------------------*
| npc | eno | Phaser  | Work                              |
*-----+-----+---------+-----------------------------------*
| 4   | -   | R0 CTRL |                                   |
| 4   | 904 | R0 DATA |                                   |
| 4   | 904 | W0 DATA | EXEC  W/P:The value of the int... |
*-----+-----+---------+-----------------------------------*

// res3: Thread=xpc10 state=X2:"2:xpc10:2"
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                              |
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------*
| 3   | -   | R0 CTRL |                                                                                                                   |
| 3   | 903 | R0 DATA |                                                                                                                   |
| 3   | 903 | W0 DATA | EXEC ;test42_pio_slave_reg2 te=3-te-3 scalarw args=C64(1026);test42_pio_slave_reg1 te=3-te-3 scalarw args=C64(10\ |
|     |     |         | 25);test42_pio_slave_reg0 te=3-te-3 scalarw args=C64(1024)                                                        |
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X4:"4:xpc10:4"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 2   | -   | R0 CTRL |       |
| 2   | 902 | R0 DATA |       |
| 2   | 902 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc10 state=X8:"8:xpc10:8"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 1   | -   | R0 CTRL |       |
| 1   | 901 | R0 DATA |       |
| 1   | 901 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc10 state=X0:"0:xpc10:start0"
*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                   |
*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------*
| 0   | -   | R0 CTRL |                                                                                                                                        |
| 0   | 900 | R0 DATA |                                                                                                                                        |
| 0   | 900 | W0 DATA | EXEC ;tTMT4Main_V_0 te=0-te-0 scalarw args={SC:d22,0};CS0.3_refxxarray10 te=0-te-0 scalarw args={SC:d22,0} W/P:GSAI:hpr_writeln:$$A... |
*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------*

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

// Offchip Memory Physical Ports/Banks = Nothing to Report

// */

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
