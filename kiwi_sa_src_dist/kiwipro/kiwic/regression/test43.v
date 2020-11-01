

// CBG Orangepath HPR L/S System

// Verilog output file generated at 02/03/2016 16:08:51
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 2.03: 28-Feb-2016 Unix 3.19.8.100
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-kcode-dump=enable test43.exe -sim 1800 -vnl-resets=synchronous -vnl test43.v -res2-no-dram-ports=0 -vnl-rootmodname=TEST43SLAVE -kiwic-finish=disable -conerefine=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -give-backtrace -report-each-step
`timescale 1ns/10ps


module TEST43SLAVE(input clk, input reset);
  integer tTMT4Main_V_0;
  wire [31:0] ktop12;
  reg [31:0] KiKiwi_old_pausemode_value;
  wire [31:0] ktop10;
  integer mpc10;
  reg [1:0] xpc10nz;
 always   @(posedge clk )  begin 
      //Start structure HPR test43.exe
      if (reset)  begin 
               KiKiwi_old_pausemode_value <= 32'd0;
               tTMT4Main_V_0 <= 32'd0;
               xpc10nz <= 2'd0;
               end 
               else  begin 
              
              case (xpc10nz)
                  0/*0:US*/:  begin 
                      $display("test43 start");
                       KiKiwi_old_pausemode_value <= 32'd2;
                       tTMT4Main_V_0 <= 32'd1;
                       xpc10nz <= 2'd2/*2:xpc10nz*/;
                       end 
                      
                  1'd1/*1:US*/:  begin 
                      $display("The value of the integer: %1d", tTMT4Main_V_0);
                       tTMT4Main_V_0 <= 32'd1001*tTMT4Main_V_0;
                       xpc10nz <= 2'd2/*2:xpc10nz*/;
                       end 
                      endcase
              if ((xpc10nz==2'd2/*2:US*/))  xpc10nz <= 1'd1/*1:xpc10nz*/;
                   end 
              //End structure HPR test43.exe


       end 
      

// 1 vectors of width 2
// 1 vectors of width 32
// 32 bits in scalar variables
// Total state bits in module = 66 bits.
// 96 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

// 
/*

// res3: Thread=xpc10 state=X1:"1:xpc10:1"
*-----+-----+---------+-------*
| npc | eno | Phaser  | Work  |
*-----+-----+---------+-------*
| 2   | -   | R0 CTRL |       |
| 2   | 902 | R0 DATA |       |
| 2   | 902 | W0 DATA | EXEC  |
*-----+-----+---------+-------*

// res3: Thread=xpc10 state=X2:"2:xpc10:2"
*-----+-----+---------+----------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                         |
*-----+-----+---------+----------------------------------------------------------------------------------------------*
| 1   | -   | R0 CTRL |                                                                                              |
| 1   | 901 | R0 DATA |                                                                                              |
| 1   | 901 | W0 DATA | EXEC ;tTMT4Main_V_0 te=1-te-1 scalarw args=C(1001*tTMT4Main_V_0) W/P:The value of the int... |
*-----+-----+---------+----------------------------------------------------------------------------------------------*

// res3: Thread=xpc10 state=X0:"0:xpc10:start0"
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*
| npc | eno | Phaser  | Work                                                                                                                                |
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*
| 0   | -   | R0 CTRL |                                                                                                                                     |
| 0   | 900 | R0 DATA |                                                                                                                                     |
| 0   | 900 | W0 DATA | EXEC ;tTMT4Main_V_0 te=0-te-0 scalarw args=C(1);KiKiwi_old_pausemode_value te=0-te-0 scalarw args=Cu(2) W/P:GSAI:hpr_writeln:$$A... |
*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------*

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
