

// CBG Orangepath HPR L/S System

// Verilog output file generated at 21/07/2015 23:45:55
// KiwiC (.net/CIL/C# to Verilog/SystemC compiler): Version alpha 55c: 10-Jan-2015 Unix 3.17.7.200
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -csharp-gen=disable -bypass-verilog-roundtrip=enable -report-each-step -verbose -kiwic-kcode-dump=enable -kiwic-cil-dump=separately test14w.exe -sim 1800 -conerefine=disable -restructure2=disable -give-backtrace -report-each-step

module test14w(input clk, input reset);
  reg signed [31:0] CS0_3_refxxarray12;
  reg [31:0] tTMaCZ_0_4_blockrefxxnewobj12;
  reg signed [31:0] CS0_3_refxxarray10;
  reg [31:0] tTMaCZ_0_2_blockrefxxnewobj10;
  reg [31:0] tTMT4Main_V_3;
  integer tTMT4Main_V_2;
  reg [31:0] tTMT4Main_V_1;
  reg [31:0] tTMT4Main_V_0;
  reg [31:0] KiKiwi_old_pausemode_value;
  wire [31:0] HPRtop;
  reg signed [31:0] A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow;
  wire signed [31:0] A_sA_SINT_CC_BASEn1n;
  reg signed [31:0] A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow;
  wire signed [31:0] A_sA_SINT_CC_arrow;
  wire signed [31:0] A_sA_SINT_CC_BASEn0n;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_right;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_right;
  wire signed [31:0] A_SINT_CC_right;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_left;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_left;
  wire signed [31:0] A_SINT_CC_left;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPA1_0;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPA1_0;
  wire signed [31:0] A_SINT_CC_RPA1_0;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPB0_0;
  wire signed [31:0] A_SINT_CC_BASEn1n;
  reg signed [31:0] A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPB0_0;
  wire signed [31:0] A_SINT_CC_RPB0_0;
  wire signed [31:0] A_SINT_CC_BASEn0n;
  integer mpc10;
  reg [4:0] xpc10;
  wire signed [31:0] hprpin17010;
  wire signed [31:0] hprpin18610;
 always   @(posedge clk )  begin 
      //Start HPR test14w.exe
      if ((xpc10==0/*0:US*/))  begin 
              $display("  Pre-test ha.left=%d, harrow=%d", 22, 1001);
              $display("  Pre-test hb.left=%d, harrow=%d", 32, 1003);
              $display("First printed value should be 32 %d", 32);
               end 
              if ((xpc10==2/*2:US*/))  begin 
              $display("  North test14w line1 : left=%d", ((1==tTMT4Main_V_1)? A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_left: ((0==tTMT4Main_V_1
              )? A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_left: 1'bx)));
              $display("  North test14w line2 : arrow[1]=%d", hprpin18610);
              $display("  North test14w line3 : arrow[15]=%d", hprpin17010);
               end 
              if ((tTMT4Main_V_2>=3) && (xpc10==32/*32:US*/))  begin 
              $display("End of test14w %d", 0);
              $finish(0);
               end 
              if ((xpc10==0/*0:US*/))  begin 
               xpc10 <= 1/*1:xpc10:1*/;
               tTMT4Main_V_2 <= 32'd0;
               A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPB0_0 <= 32'd1004;
               A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPA1_0 <= 32'd1003;
               A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPB0_0 <= 32'd1002;
               A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPA1_0 <= 32'd1001;
               A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_right <= 32'd33;
               A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_left <= 32'd32;
               A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_right <= 32'd23;
               A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_left <= 32'd22;
               tTMT4Main_V_1 <= 32'd1;
               A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow <= 32'd1;
               CS0_3_refxxarray12 <= 32'd1;
               tTMaCZ_0_4_blockrefxxnewobj12 <= 32'd1;
               tTMT4Main_V_0 <= 32'd0;
               A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow <= 32'd0;
               CS0_3_refxxarray10 <= 32'd0;
               tTMaCZ_0_2_blockrefxxnewobj10 <= 32'd0;
               KiKiwi_old_pausemode_value <= 32'd2;
               end 
              if ((((1==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow: ((0==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow
      : 1'bx))==1/*1:MS*/))  begin if ((xpc10==2/*2:US*/))  begin 
                   xpc10 <= 4/*4:xpc10:4*/;
                   A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPB0_0 <= 32'h1_86a0+((((1==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow
                  : ((0==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow: 1'bx))==1/*1:MS*/)? A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPB0_0
                  : ((((1==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow: ((0==tTMT4Main_V_0)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow
                  : 1'bx))==0/*0:MS*/)? A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPB0_0: 32'bx));

                   tTMT4Main_V_0 <= tTMT4Main_V_1;
                   tTMT4Main_V_1 <= tTMT4Main_V_0;
                   tTMT4Main_V_3 <= tTMT4Main_V_1;
                   end 
                   end 
           else if ((xpc10==2/*2:US*/))  begin 
                   xpc10 <= 8/*8:xpc10:8*/;
                   tTMT4Main_V_0 <= tTMT4Main_V_1;
                   tTMT4Main_V_1 <= tTMT4Main_V_0;
                   tTMT4Main_V_3 <= tTMT4Main_V_1;
                   end 
                  if ((xpc10==32/*32:US*/)) if ((tTMT4Main_V_2<3))  begin 
                   xpc10 <= 1/*1:xpc10:1*/;
                   tTMT4Main_V_2 <= 32'd1+tTMT4Main_V_2;
                   end 
                   else  tTMT4Main_V_2 <= 32'd1+tTMT4Main_V_2;
          if ((((1==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow: ((0==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow
      : 1'bx))==0/*0:MS*/))  begin if ((xpc10==8/*8:US*/))  begin 
                   xpc10 <= 16/*16:xpc10:16*/;
                   A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPB0_0 <= 32'h1_86a0+hprpin17010;
                   end 
                   end 
           else if ((xpc10==8/*8:US*/))  xpc10 <= 32/*32:xpc10:32*/;
              if ((xpc10==1/*1:US*/))  xpc10 <= 2/*2:xpc10:2*/;
          if ((xpc10==4/*4:US*/))  xpc10 <= 8/*8:xpc10:8*/;
          if ((xpc10==16/*16:US*/))  xpc10 <= 32/*32:xpc10:32*/;
          //End HPR test14w.exe


       end 
      

assign  #1 hprpin17010 = ((((1==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow: ((0==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow
: 1'bx))==1/*1:MS*/)? A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPB0_0: ((((1==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow
: ((0==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow: 1'bx))==0/*0:MS*/)? A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPB0_0
: 32'bx));

assign  #1 hprpin18610 = ((((1==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow: ((0==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow
: 1'bx))==1/*1:MS*/)? A_SINT_CC_A_SINT_CC_BASEn1nA_SINT_CC_RPA1_0: ((((1==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn1nA_sA_SINT_CC_arrow
: ((0==tTMT4Main_V_1)? A_sA_SINT_CC_A_sA_SINT_CC_BASEn0nA_sA_SINT_CC_arrow: 1'bx))==0/*0:MS*/)? A_SINT_CC_A_SINT_CC_BASEn0nA_SINT_CC_RPA1_0
: 32'bx));

//Total area 0
// 1 vectors of width 5
// 18 vectors of width 32
// 32 bits in scalar variables
// Total state bits in module = 613 bits.
// 416 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
