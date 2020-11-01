

// CBG Orangepath HPR L/S System

// Verilog output file generated at 04/12/2016 21:48:36
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.17b : 1st-December-2016 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable test45.exe -cil-uwind-budget=400 -kiwic-kcode-dump=enable -res2-regfile-threshold=8 -kiwic-autodispose=enable -sim 1800 -vnl-resets=synchronous -vnl test45.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -vnl-rootmodname=DUT -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(output reg [639:0] KppWaypoint0, output [639:0] KppWaypoint1, input clk, input reset);
function [31:0] hpr_dbl2flt4;
input [63:0] arg;

reg          signi;
reg [10:0]   expi;
reg [51:0]   manti;
reg [7:0]    expo;
reg [22:0]   manto;
reg  overflow, scase_inf, scase_zero, scase_nan, fail;

begin
  { signi, expi, manti } = arg;  // Deconstruct input arg
  scase_zero = (arg[62:0] == 63'd0);
  scase_inf = (expi == 11'h7ff) && (manti == 0);
  scase_nan = (expi == 11'h7ff) && (manti != 0);
// We can report fail on overflow but better to report infinity.
  fail = 0;
  overflow = (expi[10] == expi[9]) ||(expi[10] == expi[8]) ||(expi[10] == expi[7]);
  expo = { expi[10], expi[6:0]};
  manto = manti[51:51-22];
  scase_inf = scase_inf || overflow;
  hpr_dbl2flt4[31]    = signi;
  hpr_dbl2flt4[30:23] = (scase_inf)? 8'hff: (scase_nan)? 8'hff: (scase_zero)? 8'd0: expo;
  hpr_dbl2flt4[22:0]  = (scase_inf)? 23'd0: (scase_nan)? -23'd1: (scase_zero)? 23'd0: manto;
end
endfunction


function [63:0] hpr_flt2dbl3;//Floating-point convert single to double precision.
input [31:0] darg;
  hpr_flt2dbl3 = {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};
endfunction


function [7:0] rtl_unsigned_bitextract6;
   input [31:0] arg;
   rtl_unsigned_bitextract6 = $unsigned(arg[7:0]);
   endfunction


function [7:0] rtl_unsigned_bitextract0;
   input [31:0] arg;
   rtl_unsigned_bitextract0 = $unsigned(arg[7:0]);
   endfunction


function signed [31:0] rtl_sign_extend5;
   input [7:0] arg;
   rtl_sign_extend5 = { {24{arg[7]}}, arg[7:0] };
   endfunction


function signed [31:0] rtl_sign_extend2;
   input [7:0] arg;
   rtl_sign_extend2 = { {24{arg[7]}}, arg[7:0] };
   endfunction


function /*fp*/[63:0] rtl_unsigned_extend7;
   input [31:0] arg;
   rtl_unsigned_extend7 = { 32'b0, arg[31:0] };
   endfunction


function [31:0] rtl_unsigned_extend1;
   input [7:0] arg;
   rtl_unsigned_extend1 = { 24'b0, arg[7:0] };
   endfunction

  wire CVFPMULTIPLIER10_fail;
  wire CVFPMULTIPLIER12_fail;
  wire CVFPMULTIPLIER14_fail;
  reg/*fp*/  [63:0] T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP;
  integer T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
  integer T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
  integer T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0;
  integer T403_System_BitConverter_ToInt32_6_2_V_0;
  integer T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8;
  reg/*fp*/  [63:0] A_FPD_CC_SCALbx12_ARA0[3:0];
  reg [7:0] A_8_US_CC_SCALbx10_ARA0[3:0];
  reg/*fp*/  [63:0] CVFPMULTIPLIER10_A0;
  reg/*fp*/  [63:0] CVFPMULTIPLIER10_A1;
  wire/*fp*/  [63:0] CVFPMULTIPLIER10_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12_A0;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12_A1;
  wire/*fp*/  [63:0] CVFPMULTIPLIER12_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER14_A0;
  reg/*fp*/  [63:0] CVFPMULTIPLIER14_A1;
  wire/*fp*/  [63:0] CVFPMULTIPLIER14_FPRR;
  reg [4:0] xpc10nz;
  reg/*fp*/  [63:0] CVFPMULTIPLIER10RRh10hold;
  reg CVFPMULTIPLIER10RRh10shot0;
  reg CVFPMULTIPLIER10RRh10shot1;
  reg CVFPMULTIPLIER10RRh10shot2;
  reg/*fp*/  [63:0] CVFPMULTIPLIER14RRh10hold;
  reg CVFPMULTIPLIER14RRh10shot0;
  reg CVFPMULTIPLIER14RRh10shot1;
  reg CVFPMULTIPLIER14RRh10shot2;
  reg/*fp*/  [63:0] CVFPMULTIPLIER12RRh10hold;
  reg CVFPMULTIPLIER12RRh10shot0;
  reg CVFPMULTIPLIER12RRh10shot1;
  reg CVFPMULTIPLIER12RRh10shot2;
 always   @(* )  begin 
       KppWaypoint0 = 32'sd0;
       CVFPMULTIPLIER14_A0 = 32'sd0;
       CVFPMULTIPLIER14_A1 = 32'sd0;
       CVFPMULTIPLIER12_A0 = 32'sd0;
       CVFPMULTIPLIER12_A1 = 32'sd0;
       CVFPMULTIPLIER10_A0 = 32'sd0;
       CVFPMULTIPLIER10_A1 = 32'sd0;
      if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sh4) && (xpc10nz==5'sd2/*2:xpc10nz*/))  begin 
               CVFPMULTIPLIER10_A1 = T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP;
               CVFPMULTIPLIER10_A0 = 64'h_4028_0000_0000_0000;
               end 
              if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd1))  begin if ((xpc10nz==5'sd7/*7:xpc10nz*/))  begin 
                   CVFPMULTIPLIER14_A1 = T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP;
                   CVFPMULTIPLIER14_A0 = 64'h_4059_0000_0000_0000;
                   end 
                   end 
           else if ((xpc10nz==5'sd7/*7:xpc10nz*/))  begin 
                   CVFPMULTIPLIER12_A1 = T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP;
                   CVFPMULTIPLIER12_A0 = 64'h_4059_0000_0000_0000;
                   end 
                  
      case (xpc10nz)
          5'sd14/*14:xpc10nz*/:  KppWaypoint0 = "LoopTop";

          5'sd15/*15:xpc10nz*/: if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd2))  begin 
                   KppWaypoint0 = "LoopBot";
                   KppWaypoint0 = "BitConvTest";
                   end 
                   else  KppWaypoint0 = "LoopBot";
      endcase
      if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd1) && (xpc10nz==5'sd13/*13:xpc10nz*/))  KppWaypoint0 = "FloatLoop";
          
      case (xpc10nz)
          5'sd1/*1:xpc10nz*/:  begin 
               KppWaypoint0 = "HeadMark";
               KppWaypoint0 = "FloatConvert0";
               end 
              
          5'sd6/*6:xpc10nz*/:  KppWaypoint0 = "FloatConvert1";
      endcase
       end 
      

 always   @(posedge clk )  begin 
      //Start structure HPR test45
      if (reset)  begin 
               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8 <= 32'd0;
               T403_System_BitConverter_ToInt32_6_2_V_0 <= 32'd0;
               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'd0;
               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0 <= 32'd0;
               T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'd0;
               T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= 64'd0;
               CVFPMULTIPLIER10RRh10hold <= 64'd0;
               CVFPMULTIPLIER10RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER14RRh10hold <= 64'd0;
               CVFPMULTIPLIER14RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER12RRh10hold <= 64'd0;
               CVFPMULTIPLIER12RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot0 <= 32'd0;
               xpc10nz <= 32'd0;
               end 
               else  begin 
              
              case (xpc10nz)
                  5'sd15/*15:xpc10nz*/: if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd2))  begin 
                          $display("  %1d  %1d", T403_BitConverterTest_BitConvTest_0_11_V_0_GP, T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                          );
                          $display("Bit Convertor Test");
                          $display("IsLittleEndian=%1d\n", 1'h1);
                           end 
                           else $display("  %1d  %1d", T403_BitConverterTest_BitConvTest_0_11_V_0_GP, T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                      );

                  5'sd17/*17:xpc10nz*/:  begin 
                      if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4))  begin 
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[32'd0]));
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[8'd1]));
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[8'd2]));
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[8'd3]));
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      >=32'sh4))  begin 
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[32'd0]));
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[8'd1]));
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4)) $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                          [32'd0]));
                           end 
                      
                  5'sd18/*18:xpc10nz*/:  begin 
                      if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4)) $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                          [rtl_unsigned_extend1(8'd1)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]));
                          if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4))  begin 
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[rtl_unsigned_extend1(8'd2)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                              ]));
                              $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[rtl_unsigned_extend1(8'd3)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                              ]));
                               end 
                              if (((32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4)? (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      >=32'sh4): 1'd1) && (32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4)) $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                          [rtl_unsigned_extend1(8'd1)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]));
                          if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4)) $display("   %h", rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                          [rtl_unsigned_extend1(8'd2)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]));
                           end 
                      
                  5'sd19/*19:xpc10nz*/: $display("Sum after Int32 %1d sum=%1d reconverted=%1d", -32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                  , T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0, T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8);

                  5'sd20/*20:xpc10nz*/: if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd3))  begin 
                          $display("Test45 iteration %1d: checksum = %1d", T403_BitConverterTest_BitConvTest_0_11_V_0_GP, T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                          );
                          $display("Test45 finished.");
                          $finish(32'sd0);
                           end 
                           else $display("Test45 iteration %1d: checksum = %1d", T403_BitConverterTest_BitConvTest_0_11_V_0_GP, T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      );

                  5'sd21/*21:xpc10nz*/: $display("Sum after Int32 %1d sum=%1d reconverted=%1d", -32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                  , T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0, T403_System_BitConverter_ToInt32_6_2_V_0);

                  5'sd22/*22:xpc10nz*/: $display("Sum after Int32 %1d sum=%1d reconverted=%1d", -32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                  , T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0, $unsigned($unsigned(T403_System_BitConverter_ToInt32_6_2_V_0+
                  (rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3])<<32'sd24
                  )));
              endcase
              if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sd1))  begin if ((xpc10nz==5'sd10/*10:xpc10nz*/)) $display("FloatConvTest1:%F  becomes %F"
                      , $bitstoreal(T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP), $bitstoreal(hpr_flt2dbl3(hpr_dbl2flt4(T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP
                      ))));
                       end 
                   else if ((xpc10nz==5'sd13/*13:xpc10nz*/)) $display("FloatConvTest1:%F  becomes %F", $bitstoreal(T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP
                      ), $bitstoreal(hpr_flt2dbl3(hpr_dbl2flt4(T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP))));
                      
              case (xpc10nz)
                  5'sd0/*0:xpc10nz*/: $display("Hello from Test45");

                  5'sd6/*6:xpc10nz*/:  begin 
                      $display("FloatConvTest0: %1d  has %F  ", 32'sd0, $bitstoreal(A_FPD_CC_SCALbx12_ARA0[64'd0]));
                      $display("FloatConvTest0: %1d  has %F  ", 32'sd1, $bitstoreal(A_FPD_CC_SCALbx12_ARA0[32'sd1]));
                      $display("FloatConvTest0: %1d  has %F  ", 32'sd2, $bitstoreal(A_FPD_CC_SCALbx12_ARA0[32'sd2]));
                      $display("FloatConvTest0: %1d  has %F  ", 32'sd3, $bitstoreal(A_FPD_CC_SCALbx12_ARA0[32'sd3]));
                       end 
                      
                  5'sd15/*15:xpc10nz*/: if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sd2))  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
                           xpc10nz <= 5'sd14/*14:xpc10nz*/;
                           end 
                           else  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd0;
                           xpc10nz <= 5'sd16/*16:xpc10nz*/;
                           end 
                          
                  5'sd17/*17:xpc10nz*/:  begin 
                      if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4))  T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                           <= 32'sd3;

                          if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4))  begin 
                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
                               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0 <= $signed(rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                              [rtl_unsigned_extend1(8'd3)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]))+$signed(rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                              [rtl_unsigned_extend1(8'd2)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]))+$signed(T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                              +rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[rtl_unsigned_extend1(8'd1)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                              ])))));

                               xpc10nz <= 5'sd17/*17:xpc10nz*/;
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4))  begin 
                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd0;
                               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0 <= $signed(rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0
                              [rtl_unsigned_extend1(8'd2)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP]))+$signed(T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                              +rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[rtl_unsigned_extend1(8'd1)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                              ]))));

                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      >=32'sh4))  begin 
                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd1;
                               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0 <= $signed(T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                              +rtl_sign_extend2(rtl_unsigned_bitextract0(A_8_US_CC_SCALbx10_ARA0[rtl_unsigned_extend1(8'd1)+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                              ])));

                               end 
                              if (((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4)? ((32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4)? (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4): 1'd1): 1'd1))  xpc10nz <= 5'sd18/*18:xpc10nz*/;
                           end 
                      
                  5'sd18/*18:xpc10nz*/:  begin 
                      if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4))  begin 
                               T403_System_BitConverter_ToInt32_6_2_V_0 <= $unsigned($unsigned($unsigned(rtl_sign_extend2($unsigned(A_8_US_CC_SCALbx10_ARA0
                              [32'd0]))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh1])<<32'sd8))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0
                              [64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3])<<32'sd24));

                               T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8 <= $unsigned($unsigned($unsigned(rtl_sign_extend2($unsigned(A_8_US_CC_SCALbx10_ARA0
                              [32'd0]))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh1])<<32'sd8))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0
                              [64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3])<<32'sd24));

                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
                               xpc10nz <= 5'sd19/*19:xpc10nz*/;
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4))  begin 
                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
                               xpc10nz <= 5'sd18/*18:xpc10nz*/;
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      <32'sh4) && (32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP>=32'sh4))  begin 
                               T403_System_BitConverter_ToInt32_6_2_V_0 <= $unsigned(rtl_sign_extend2($unsigned(A_8_US_CC_SCALbx10_ARA0
                              [32'd0]))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh1])<<32'sd8));

                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd3+T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
                               xpc10nz <= 5'sd22/*22:xpc10nz*/;
                               end 
                              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_1_GP<32'sh4) && (32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP
                      >=32'sh4))  begin 
                               T403_System_BitConverter_ToInt32_6_2_V_0 <= $unsigned($unsigned($unsigned(rtl_sign_extend2($unsigned(A_8_US_CC_SCALbx10_ARA0
                              [32'd0]))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh1])<<32'sd8))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0
                              [64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3])<<32'sd24));

                               T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd2+T403_BitConverterTest_BitConvTest_0_11_V_1_GP;
                               xpc10nz <= 5'sd21/*21:xpc10nz*/;
                               end 
                               end 
                      endcase
              if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sd1)) 
                  case (xpc10nz)
                      5'sd7/*7:xpc10nz*/:  xpc10nz <= 5'sd8/*8:xpc10nz*/;

                      5'sd10/*10:xpc10nz*/:  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
                           T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= ((xpc10nz==5'sd10/*10:xpc10nz*/)? CVFPMULTIPLIER12_FPRR
                          : CVFPMULTIPLIER12RRh10hold);

                           end 
                          endcase
                   else 
                  case (xpc10nz)
                      5'sd7/*7:xpc10nz*/:  xpc10nz <= 5'sd11/*11:xpc10nz*/;

                      5'sd13/*13:xpc10nz*/:  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd0;
                           T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= ((xpc10nz==5'sd13/*13:xpc10nz*/)? CVFPMULTIPLIER14_FPRR
                          : CVFPMULTIPLIER14RRh10hold);

                           end 
                          endcase

              case (xpc10nz)
                  5'sd6/*6:xpc10nz*/:  begin 
                       T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd0;
                       T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= 64'h_462f_29c4_eeb1_55e5;
                       xpc10nz <= 5'sd7/*7:xpc10nz*/;
                       end 
                      
                  5'sd16/*16:xpc10nz*/:  begin 
                       T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= 32'sd1;
                       T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0 <= $signed(rtl_sign_extend5(rtl_unsigned_bitextract6(8'd255
                      &-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP))+rtl_sign_extend2(rtl_unsigned_bitextract6(8'd255
                      &(-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP>>>32'sd8))));

                       A_8_US_CC_SCALbx10_ARA0[64'sh3] <= rtl_unsigned_bitextract6(8'd255&(-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                      >>>32'sd24));

                       A_8_US_CC_SCALbx10_ARA0[64'sh2] <= rtl_unsigned_bitextract6(8'd255&(-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                      >>>32'sd16));

                       A_8_US_CC_SCALbx10_ARA0[64'sh1] <= rtl_unsigned_bitextract6(8'd255&(-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                      >>>32'sd8));

                       A_8_US_CC_SCALbx10_ARA0[32'd0] <= rtl_unsigned_bitextract6(8'd255&-32'sh_4996_02d3+T403_BitConverterTest_BitConvTest_0_11_V_0_GP
                      );

                       xpc10nz <= 5'sd17/*17:xpc10nz*/;
                       end 
                      
                  5'sd20/*20:xpc10nz*/:  begin 
                      if ((T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sd3))  xpc10nz <= 5'sd16/*16:xpc10nz*/;
                           else  xpc10nz <= 5'sd20/*20:xpc10nz*/;
                       T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
                       end 
                      endcase
              if ((32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sh4)) 
                  case (xpc10nz)
                      5'sd2/*2:xpc10nz*/:  xpc10nz <= 5'sd3/*3:xpc10nz*/;

                      5'sd5/*5:xpc10nz*/:  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
                           T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= ((xpc10nz==5'sd5/*5:xpc10nz*/)? CVFPMULTIPLIER10_FPRR: CVFPMULTIPLIER10RRh10hold
                          );

                           A_FPD_CC_SCALbx12_ARA0[64'd1+rtl_unsigned_extend7(T403_BitConverterTest_BitConvTest_0_11_V_0_GP)] <= T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP
                          ;

                           end 
                          endcase
                   else if ((xpc10nz==5'sd2/*2:xpc10nz*/))  begin 
                           T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP;
                           xpc10nz <= 5'sd6/*6:xpc10nz*/;
                           end 
                          
              case (xpc10nz)
                  5'sd0/*0:xpc10nz*/:  xpc10nz <= 5'sd1/*1:xpc10nz*/;

                  5'sd1/*1:xpc10nz*/:  begin 
                       T403_BitConverterTest_BitConvTest_0_11_V_0_GP <= 32'sd0;
                       T403_BitConverterTest_FloatConvTest1_0_9_V_0_GP <= 64'h_4102_15a1_2ca5_7a79;
                       A_FPD_CC_SCALbx12_ARA0[64'd0] <= 64'h_40c8_1cd6_e631_f8a1;
                       xpc10nz <= 5'sd2/*2:xpc10nz*/;
                       end 
                      
                  5'sd5/*5:xpc10nz*/:  xpc10nz <= 5'sd2/*2:xpc10nz*/;

                  5'sd10/*10:xpc10nz*/:  xpc10nz <= 5'sd7/*7:xpc10nz*/;

                  5'sd13/*13:xpc10nz*/:  xpc10nz <= 5'sd14/*14:xpc10nz*/;

                  5'sd19/*19:xpc10nz*/:  begin 
                       T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= T403_BitConverterTest_BitConvTest_0_11_V_1_GP+T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                      ;

                       xpc10nz <= 5'sd20/*20:xpc10nz*/;
                       end 
                      
                  5'sd21/*21:xpc10nz*/:  begin 
                       T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8 <= T403_System_BitConverter_ToInt32_6_2_V_0;
                       T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= T403_BitConverterTest_BitConvTest_0_11_V_1_GP+T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                      ;

                       xpc10nz <= 5'sd20/*20:xpc10nz*/;
                       end 
                      
                  5'sd22/*22:xpc10nz*/:  begin 
                       T403_System_BitConverter_ToInt32_6_2_V_0 <= $unsigned($unsigned(T403_System_BitConverter_ToInt32_6_2_V_0+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0
                      [64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3])<<32'sd24));

                       T403_BitConverterTest_RoundTripTest_Int32_1_10_V_8 <= $unsigned($unsigned(T403_System_BitConverter_ToInt32_6_2_V_0
                      +(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh2])<<32'sd16))+(rtl_sign_extend2(A_8_US_CC_SCALbx10_ARA0[64'sh3
                      ])<<32'sd24));

                       T403_BitConverterTest_BitConvTest_0_11_V_1_GP <= T403_BitConverterTest_BitConvTest_0_11_V_1_GP+T403_BitConverterTest_RoundTripTest_Int32_1_10_V_0
                      ;

                       xpc10nz <= 5'sd20/*20:xpc10nz*/;
                       end 
                      endcase
              if (CVFPMULTIPLIER12RRh10shot2)  CVFPMULTIPLIER12RRh10hold <= CVFPMULTIPLIER12_FPRR;
                  if (CVFPMULTIPLIER14RRh10shot2)  CVFPMULTIPLIER14RRh10hold <= CVFPMULTIPLIER14_FPRR;
                  if (CVFPMULTIPLIER10RRh10shot2)  CVFPMULTIPLIER10RRh10hold <= CVFPMULTIPLIER10_FPRR;
                  
              case (xpc10nz)
                  5'sd3/*3:xpc10nz*/:  xpc10nz <= 5'sd4/*4:xpc10nz*/;

                  5'sd4/*4:xpc10nz*/:  xpc10nz <= 5'sd5/*5:xpc10nz*/;

                  5'sd8/*8:xpc10nz*/:  xpc10nz <= 5'sd9/*9:xpc10nz*/;

                  5'sd9/*9:xpc10nz*/:  xpc10nz <= 5'sd10/*10:xpc10nz*/;

                  5'sd11/*11:xpc10nz*/:  xpc10nz <= 5'sd12/*12:xpc10nz*/;

                  5'sd12/*12:xpc10nz*/:  xpc10nz <= 5'sd13/*13:xpc10nz*/;

                  5'sd14/*14:xpc10nz*/:  xpc10nz <= 5'sd15/*15:xpc10nz*/;
              endcase
               CVFPMULTIPLIER10RRh10shot1 <= CVFPMULTIPLIER10RRh10shot0;
               CVFPMULTIPLIER10RRh10shot2 <= CVFPMULTIPLIER10RRh10shot1;
               CVFPMULTIPLIER14RRh10shot1 <= CVFPMULTIPLIER14RRh10shot0;
               CVFPMULTIPLIER14RRh10shot2 <= CVFPMULTIPLIER14RRh10shot1;
               CVFPMULTIPLIER12RRh10shot1 <= CVFPMULTIPLIER12RRh10shot0;
               CVFPMULTIPLIER12RRh10shot2 <= CVFPMULTIPLIER12RRh10shot1;
               CVFPMULTIPLIER12RRh10shot0 <= (T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sd1) && (xpc10nz==5'sd7/*7:xpc10nz*/);
               CVFPMULTIPLIER14RRh10shot0 <= (T403_BitConverterTest_BitConvTest_0_11_V_0_GP>=32'sd1) && (xpc10nz==5'sd7/*7:xpc10nz*/);
               CVFPMULTIPLIER10RRh10shot0 <= (32'sd1+T403_BitConverterTest_BitConvTest_0_11_V_0_GP<32'sh4) && (xpc10nz==5'sd2/*2:xpc10nz*/);
               end 
              //End structure HPR test45


       end 
      

  CV_FP_FL3_DP_MULTIPLIER CVFPMULTIPLIER10(clk, reset, CVFPMULTIPLIER10_FPRR, CVFPMULTIPLIER10_A0, CVFPMULTIPLIER10_A1, CVFPMULTIPLIER10_fail
);
  CV_FP_FL3_DP_MULTIPLIER CVFPMULTIPLIER12(clk, reset, CVFPMULTIPLIER12_FPRR, CVFPMULTIPLIER12_A0, CVFPMULTIPLIER12_A1, CVFPMULTIPLIER12_fail
);
  CV_FP_FL3_DP_MULTIPLIER CVFPMULTIPLIER14(clk, reset, CVFPMULTIPLIER14_FPRR, CVFPMULTIPLIER14_A0, CVFPMULTIPLIER14_A1, CVFPMULTIPLIER14_fail
);
// 9 vectors of width 1
// 10 vectors of width 64
// 1 vectors of width 5
// 4 array locations of width 8
// 4 array locations of width 64
// 160 bits in scalar variables
// Total state bits in module = 1102 bits.
// 195 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL3_DP_MULTIPLIER count=3
// Total number of leaf cells = 3
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.17b : 1st-December-2016
//04/12/2016 21:48:25
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable test45.exe -cil-uwind-budget=400 -kiwic-kcode-dump=enable -res2-regfile-threshold=8 -kiwic-autodispose=enable -sim 1800 -vnl-resets=synchronous -vnl test45.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=soft -vnl-rootmodname=DUT -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T400:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T401:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T402:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T403:::
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5001 dt=FPD usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5002 dt=SINT usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5003 dt=SINT usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5004 dt=FPD usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5005 dt=SINT usecount=3
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5007 dt=SINT usecount=4
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5010 dt=SINT usecount=5
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5013 dt=SINT usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5016 dt=$star1$/@/8/US usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5017 dt=SINT usecount=1
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5019 dt=$star1$/@/8/US usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5020 dt=SINT usecount=2
//
//Allocate phy reg purpose=localvar msg=allocation for thread T403 for V5011 dt=SINT usecount=3
//
//: Linear scan colouring done for 16 vregs using 5 pregs
//

//----------------------------------------------------------

//Report from Abbreviation:::
//Setting up abbreviation @8 for prefix @/8

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called KiwiSystem/Kiwi
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) id=cctor10
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called System/BitConverter
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) id=cctor12
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called BitConverterTest
//
//root_walk start thread at a static method (used as an entry point). Method name=.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called BitConverterTest
//
//root_compiler: start elaborating class 'BitConverterTest'
//
//elaborating class 'BitConverterTest'
//
//compiling static method as entry point: style=Root idl=BitConverterTest/Main
//
//Performing root elaboration of method Main
//
//KiwiC start_thread (or entry point) id=Main10
//
//Register sharing: general T403/BitConverterTest/FloatConvTest1/0.9/V_0/GP used for T403/BitConverterTest/FloatConvTest1/0.9/V_0
//
//Register sharing: general T403/BitConverterTest/FloatConvTest1/0.9/V_0/GP used for T403/BitConverterTest/FloatConvTest0/0.8/V_0
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_1/GP used for T403/BitConverterTest/BitConvTest/0.11/V_1
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_1/GP used for T403/BitConverterTest/RoundTripTest_Int32/1.10/V_7
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_1/GP used for T403/BitConverterTest/RoundTripTest_Int32/1.10/V_4
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_0/GP used for T403/BitConverterTest/BitConvTest/0.11/V_0
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_0/GP used for T403/BitConverterTest/FloatLoopTest/0.10/V_0
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_0/GP used for T403/BitConverterTest/FloatConvTest1/0.9/V_1
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_0/GP used for T403/BitConverterTest/FloatConvTest0/0.8/V_2
//
//Register sharing: general T403/BitConverterTest/BitConvTest/0.11/V_0/GP used for T403/BitConverterTest/FloatConvTest0/0.8/V_1
//
//Register sharing: general T403/BitConverterTest/RoundTripTest_Int32/1.10/V_6/GP used for T403/BitConverterTest/RoundTripTest_Int32/1.10/V_6
//
//Register sharing: general T403/BitConverterTest/RoundTripTest_Int32/1.10/V_6/GP used for T403/BitConverterTest/RoundTripTest_Int32/1.10/V_3
//
//root_compiler class done: BitConverterTest
//
//Report of all settings used from the recipe or command line:
//
//   cil-uwind-budget=400
//
//   kiwic-finish=enable
//
//   kiwic-cil-dump=disable
//
//   kiwic-kcode-dump=enable
//
//   kiwic-register-colours=1
//
//   array-4d-name=KIWIARRAY4D
//
//   array-3d-name=KIWIARRAY3D
//
//   array-2d-name=KIWIARRAY2D
//
//   kiwi-dll=Kiwi.dll
//
//   kiwic-dll=Kiwic.dll
//
//   kiwic-zerolength-arrays=disable
//
//   kiwic-fpgaconsole-default=enable
//
//   postgen-optimise=enable
//
//   ataken-loglevel=20
//
//   gtrace-loglevel=20
//
//   firstpass-loglevel=20
//
//   overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=test45.exe
//
//   kiwic-autodispose=enable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int-flr-mul               | -3000   |                                                                                 |
//| max-no-fp-addsubs         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls            | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| fp-fl-dp-div              | 5       |                                                                                 |
//| fp-fl-dp-add              | 4       |                                                                                 |
//| fp-fl-dp-mul              | 3       |                                                                                 |
//| fp-fl-sp-div              | 5       |                                                                                 |
//| fp-fl-sp-add              | 4       |                                                                                 |
//| fp-fl-sp-mul              | 3       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//| res2-loadstore-port-count | 0       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for xpc10 
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//| gb-flag/Pause        | eno | hwm         | root | exec | start | end | antecedants | next |
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//|   X0:"0:xpc10"       | 900 | hwm=0.0.0   | 0    | 0    | -     | -   | ---         | 1    |
//|   X1:"1:xpc10"       | 901 | hwm=0.0.0   | 1    | 1    | -     | -   | ---         | 2    |
//|   X2:"2:xpc10"       | 903 | hwm=0.0.0   | 2    | 2    | -     | -   | ---         | 6    |
//|   X2:"2:xpc10"       | 902 | hwm=0.3.0   | 2    | 5    | 3     | 5   | ---         | 2    |
//|   X4:"4:xpc10"       | 904 | hwm=0.0.0   | 6    | 6    | -     | -   | ---         | 7    |
//|   X8:"8:xpc10"       | 906 | hwm=0.3.0   | 7    | 13   | 11    | 13  | ---         | 14   |
//|   X8:"8:xpc10"       | 905 | hwm=0.3.0   | 7    | 10   | 8     | 10  | ---         | 7    |
//|   X16:"16:xpc10"     | 907 | hwm=0.0.0   | 14   | 14   | -     | -   | ---         | 15   |
//|   X32:"32:xpc10"     | 909 | hwm=0.0.0   | 15   | 15   | -     | -   | ---         | 14   |
//|   X32:"32:xpc10"     | 908 | hwm=0.0.0   | 15   | 15   | -     | -   | ---         | 16   |
//|   X64:"64:xpc10"     | 910 | hwm=0.0.0   | 16   | 16   | -     | -   | ---         | 17   |
//|   X128:"128:xpc10"   | 914 | hwm=0.0.0   | 17   | 17   | -     | -   | ---         | 18   |
//|   X128:"128:xpc10"   | 913 | hwm=0.0.0   | 17   | 17   | -     | -   | ---         | 18   |
//|   X128:"128:xpc10"   | 912 | hwm=0.0.0   | 17   | 17   | -     | -   | ---         | 18   |
//|   X128:"128:xpc10"   | 911 | hwm=0.0.0   | 17   | 17   | -     | -   | ---         | 17   |
//|   X256:"256:xpc10"   | 918 | hwm=0.0.0   | 18   | 18   | -     | -   | ---         | 22   |
//|   X256:"256:xpc10"   | 917 | hwm=0.0.0   | 18   | 18   | -     | -   | ---         | 21   |
//|   X256:"256:xpc10"   | 916 | hwm=0.0.0   | 18   | 18   | -     | -   | ---         | 19   |
//|   X256:"256:xpc10"   | 915 | hwm=0.0.0   | 18   | 18   | -     | -   | ---         | 18   |
//|   X512:"512:xpc10"   | 919 | hwm=0.0.0   | 19   | 19   | -     | -   | ---         | 20   |
//|   X1024:"1024:xpc10" | 921 | hwm=0.0.0   | 20   | 20   | -     | -   | ---         | 16   |
//|   X1024:"1024:xpc10" | 920 | hwm=0.0.0   | 20   | 20   | -     | -   | ---         | 20   |
//|   X2048:"2048:xpc10" | 922 | hwm=0.0.0   | 21   | 21   | -     | -   | ---         | 20   |
//|   X4096:"4096:xpc10" | 923 | hwm=0.0.0   | 22   | 22   | -     | -   | ---         | 20   |
//*----------------------+-----+-------------+------+------+-------+-----+-------------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X0:"0:xpc10"
//res2: Thread=xpc10 state=X0:"0:xpc10"
//*-----+-----+---------+------------------------*
//| pc  | eno | Phaser  | Work                   |
//*-----+-----+---------+------------------------*
//| 0   | -   | R0 CTRL |                        |
//| 0   | 900 | R0 DATA |                        |
//| 0+E | 900 | W0 DATA |  PLI:Hello from Test45 |
//*-----+-----+---------+------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X1:"1:xpc10"
//res2: Thread=xpc10 state=X1:"1:xpc10"
//*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                 |
//*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------*
//| 1   | -   | R0 CTRL |                                                                                                                      |
//| 1   | 901 | R0 DATA |                                                                                                                      |
//| 1+E | 901 | W0 DATA | T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP te=te:1 scalarw(148148.1468) T403/BitConverterTest/BitConvTest/0.11\ |
//|     |     |         | /V_0_GP te=te:1 scalarw(0)  W/P:FloatConvert0  W/P:HeadMark                                                          |
//*-----+-----+---------+----------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X2:"2:xpc10"
//res2: Thread=xpc10 state=X2:"2:xpc10"
//*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                                  |
//*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//| 2   | -   | R0 CTRL |                                                                                                                                       |
//| 2   | 902 | R0 DATA | CVFPMULTIPLIER10 te=te:2 *fixed-func-ALU*(12, E1)                                                                                     |
//| 3   | 902 | R1 DATA |                                                                                                                                       |
//| 4   | 902 | R2 DATA |                                                                                                                                       |
//| 5   | 902 | R3 DATA |                                                                                                                                       |
//| 5+E | 902 | W0 DATA | T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP te=te:5 scalarw(E2) T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:5 scalarw(E3) |
//| 2   | 903 | R0 DATA |                                                                                                                                       |
//| 2+E | 903 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:2 scalarw(E3)                                                                     |
//*-----+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X4:"4:xpc10"
//res2: Thread=xpc10 state=X4:"4:xpc10"
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc  | eno | Phaser  | Work                                                                                                                          |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------*
//| 6   | -   | R0 CTRL |                                                                                                                               |
//| 6   | 904 | R0 DATA |                                                                                                                               |
//| 6+E | 904 | W0 DATA | T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP te=te:6 scalarw(1.2345E+30) T403/BitConverterTest/BitConvTest/0.11/V_0_GP te\ |
//|     |     |         | =te:6 scalarw(0)  W/P:FloatConvert1  PLI:FloatConvTest0: %u  ...                                                              |
//*-----+-----+---------+-------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X8:"8:xpc10"
//res2: Thread=xpc10 state=X8:"8:xpc10"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                   |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//| 7    | -   | R0 CTRL |                                                                                                                        |
//| 7    | 905 | R0 DATA | CVFPMULTIPLIER12 te=te:7 *fixed-func-ALU*(100, E1)                                                                     |
//| 8    | 905 | R1 DATA |                                                                                                                        |
//| 9    | 905 | R2 DATA |                                                                                                                        |
//| 10   | 905 | R3 DATA |                                                                                                                        |
//| 10+E | 905 | W0 DATA | T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP te=te:10 scalarw(E4) T403/BitConverterTest/BitConvTest/0.11/V_0_GP te\ |
//|      |     |         | =te:10 scalarw(E3)  PLI:FloatConvTest1:%F  b...                                                                        |
//| 7    | 906 | R0 DATA | CVFPMULTIPLIER14 te=te:7 *fixed-func-ALU*(100, E1)                                                                     |
//| 11   | 906 | R1 DATA |                                                                                                                        |
//| 12   | 906 | R2 DATA |                                                                                                                        |
//| 13   | 906 | R3 DATA |                                                                                                                        |
//| 13+E | 906 | W0 DATA | T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP te=te:13 scalarw(E4) T403/BitConverterTest/BitConvTest/0.11/V_0_GP te\ |
//|      |     |         | =te:13 scalarw(0)  W/P:FloatLoop  PLI:FloatConvTest1:%F  b...                                                          |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X16:"16:xpc10"
//res2: Thread=xpc10 state=X16:"16:xpc10"
//*------+-----+---------+--------------*
//| pc   | eno | Phaser  | Work         |
//*------+-----+---------+--------------*
//| 14   | -   | R0 CTRL |              |
//| 14   | 907 | R0 DATA |              |
//| 14+E | 907 | W0 DATA |  W/P:LoopTop |
//*------+-----+---------+--------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X32:"32:xpc10"
//res2: Thread=xpc10 state=X32:"32:xpc10"
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                                          |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| 15   | -   | R0 CTRL |                                                                                                                                                               |
//| 15   | 908 | R0 DATA |                                                                                                                                                               |
//| 15+E | 908 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:15 scalarw(0)  W/P:BitConvTest  PLI:IsLittleEndian=%d
//  PLI:Bit Convertor Test  W/P:LoopBot  PLI:  %u  %u |
//| 15   | 909 | R0 DATA |                                                                                                                                                               |
//| 15+E | 909 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:15 scalarw(E3)  W/P:LoopBot  PLI:  %u  %u                                                                 |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X64:"64:xpc10"
//res2: Thread=xpc10 state=X64:"64:xpc10"
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                      |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| 16   | -   | R0 CTRL |                                                                                                                                           |
//| 16   | 910 | R0 DATA |                                                                                                                                           |
//| 16+E | 910 | W0 DATA | T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0 te=te:16 scalarw(E5) T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:16 scalarw(1) |
//*------+-----+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X128:"128:xpc10"
//res2: Thread=xpc10 state=X128:"128:xpc10"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                                 |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//| 17   | -   | R0 CTRL |                                                                                                                                                      |
//| 17   | 911 | R0 DATA |                                                                                                                                                      |
//| 17+E | 911 | W0 DATA | T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0 te=te:17 scalarw(E6) T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:17 scalarw(E7)           |
//| 17   | 912 | R0 DATA |                                                                                                                                                      |
//| 17+E | 912 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:17 scalarw(3)  PLI:   %h                                                                         |
//| 17   | 913 | R0 DATA |                                                                                                                                                      |
//| 17+E | 913 | W0 DATA | T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0 te=te:17 scalarw(E8) T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:17 scalarw(1)  PLI:   %h |
//| 17   | 914 | R0 DATA |                                                                                                                                                      |
//| 17+E | 914 | W0 DATA | T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0 te=te:17 scalarw(E9) T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:17 scalarw(0)  PLI:   %h |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X256:"256:xpc10"
//res2: Thread=xpc10 state=X256:"256:xpc10"
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                    |
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//| 18   | -   | R0 CTRL |                                                                                                                                         |
//| 18   | 915 | R0 DATA |                                                                                                                                         |
//| 18+E | 915 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:18 scalarw(E7)  PLI:   %h                                                           |
//| 18   | 916 | R0 DATA |                                                                                                                                         |
//| 18+E | 916 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:18 scalarw(E10) T403/BitConverterTest/RoundTripTest_Int32/1.10/V_8 te=te:18 scalar\ |
//|      |     |         | w(E11) T403/System/BitConverter/ToInt32/6.2/V_0 te=te:18 scalarw(E11)                                                                   |
//| 18   | 917 | R0 DATA |                                                                                                                                         |
//| 18+E | 917 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:18 scalarw(E12) T403/System/BitConverter/ToInt32/6.2/V_0 te=te:18 scalarw(E11)  PL\ |
//|      |     |         | I:   %h                                                                                                                                 |
//| 18   | 918 | R0 DATA |                                                                                                                                         |
//| 18+E | 918 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:18 scalarw(E7) T403/System/BitConverter/ToInt32/6.2/V_0 te=te:18 scalarw(E13)  PLI\ |
//|      |     |         | :   %h                                                                                                                                  |
//*------+-----+---------+-----------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X512:"512:xpc10"
//res2: Thread=xpc10 state=X512:"512:xpc10"
//*------+-----+---------+--------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                             |
//*------+-----+---------+--------------------------------------------------------------------------------------------------*
//| 19   | -   | R0 CTRL |                                                                                                  |
//| 19   | 919 | R0 DATA |                                                                                                  |
//| 19+E | 919 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:19 scalarw(E14)  PLI:Sum after Int32 %u s... |
//*------+-----+---------+--------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X1024:"1024:xpc10"
//res2: Thread=xpc10 state=X1024:"1024:xpc10"
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                        |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//| 20   | -   | R0 CTRL |                                                                                                                                             |
//| 20   | 920 | R0 DATA |                                                                                                                                             |
//| 20+E | 920 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:20 scalarw(E3)  PLI:GSAI:hpr_sysexit  PLI:Test45 finished.  PLI:Test45 iteration %u:... |
//| 20   | 921 | R0 DATA |                                                                                                                                             |
//| 20+E | 921 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_0_GP te=te:20 scalarw(E3)  PLI:Test45 iteration %u:...                                             |
//*------+-----+---------+---------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X2048:"2048:xpc10"
//res2: Thread=xpc10 state=X2048:"2048:xpc10"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                             |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------*
//| 21   | -   | R0 CTRL |                                                                                                                  |
//| 21   | 922 | R0 DATA |                                                                                                                  |
//| 21+E | 922 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:21 scalarw(E14) T403/BitConverterTest/RoundTripTest_Int32/1\ |
//|      |     |         | .10/V_8 te=te:21 scalarw(E15)  PLI:Sum after Int32 %u s...                                                       |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//<NONE>Simple greedy schedule for res2: Thread=xpc10 state=X4096:"4096:xpc10"
//res2: Thread=xpc10 state=X4096:"4096:xpc10"
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                                       |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| 22   | -   | R0 CTRL |                                                                                                                                                            |
//| 22   | 923 | R0 DATA |                                                                                                                                                            |
//| 22+E | 923 | W0 DATA | T403/BitConverterTest/BitConvTest/0.11/V_1_GP te=te:22 scalarw(E14) T403/BitConverterTest/RoundTripTest_Int32/1.10/V_8 te=te:22 scalarw(E16) T403/System/\ |
//|      |     |         | BitConverter/ToInt32/6.2/V_0 te=te:22 scalarw(E16)  PLI:Sum after Int32 %u s...                                                                            |
//*------+-----+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP
//
//  E2 =.= 12*T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP
//
//  E3 =.= 1+T403/BitConverterTest/BitConvTest/0.11/V_0_GP
//
//  E4 =.= 100*T403/BitConverterTest/FloatConvTest1/0.9/V_0_GP
//
//  E5 =.= C((C(C8u(255&S32'-1234567891+T403/BitConverterTest/BitConvTest/0.11/V_0_GP)))+(C8u(255&S32'-1234567891+T403/BitConverterTest/BitConvTest/0.11/V_0_GP>>8)))
//
//  E6 =.= C((C8u(@8_US/CC/SCALbx10_ARA0[3+T403/BitConverterTest/BitConvTest/0.11/V_1_GP]))+(C((C8u(@8_US/CC/SCALbx10_ARA0[2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP]))+(C(T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0+(C8u(@8_US/CC/SCALbx10_ARA0[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP])))))))
//
//  E7 =.= 3+T403/BitConverterTest/BitConvTest/0.11/V_1_GP
//
//  E8 =.= C(T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0+(C8u(@8_US/CC/SCALbx10_ARA0[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP])))
//
//  E9 =.= C((C8u(@8_US/CC/SCALbx10_ARA0[2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP]))+(C(T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0+(C8u(@8_US/CC/SCALbx10_ARA0[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP])))))
//
//  E10 =.= 1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP
//
//  E11 =.= C((C((C((C(@8_US/CC/SCALbx10_ARA0[0]))+(@8_US/CC/SCALbx10_ARA0[S64'1]<<8)))+(@8_US/CC/SCALbx10_ARA0[S64'2]<<16)))+(@8_US/CC/SCALbx10_ARA0[S64'3]<<24))
//
//  E12 =.= 2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP
//
//  E13 =.= C((C(@8_US/CC/SCALbx10_ARA0[0]))+(@8_US/CC/SCALbx10_ARA0[S64'1]<<8))
//
//  E14 =.= T403/BitConverterTest/BitConvTest/0.11/V_1_GP+(C(T403/BitConverterTest/RoundTripTest_Int32/1.10/V_0))
//
//  E15 =.= C(T403/System/BitConverter/ToInt32/6.2/V_0)
//
//  E16 =.= C((C(T403/System/BitConverter/ToInt32/6.2/V_0+(@8_US/CC/SCALbx10_ARA0[S64'2]<<16)))+(@8_US/CC/SCALbx10_ARA0[S64'3]<<24))
//
//  E17 =.= 1+T403/BitConverterTest/BitConvTest/0.11/V_0_GP<S32'4
//
//  E18 =.= 1+T403/BitConverterTest/BitConvTest/0.11/V_0_GP>=S32'4
//
//  E19 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP<1
//
//  E20 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP>=1
//
//  E21 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP>=2
//
//  E22 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP<2
//
//  E23 =.= {[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4, 2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4, 3+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4]}
//
//  E24 =.= 1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP>=S32'4
//
//  E25 =.= {[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4, 2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP>=S32'4]}
//
//  E26 =.= {[1+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4, 2+T403/BitConverterTest/BitConvTest/0.11/V_1_GP<S32'4, 3+T403/BitConverterTest/BitConvTest/0.11/V_1_GP>=S32'4]}
//
//  E27 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP>=3
//
//  E28 =.= T403/BitConverterTest/BitConvTest/0.11/V_0_GP<3
//

//----------------------------------------------------------

//Report from verilog_render:::
//9 vectors of width 1
//
//10 vectors of width 64
//
//1 vectors of width 5
//
//4 array locations of width 8
//
//4 array locations of width 64
//
//160 bits in scalar variables
//
//Total state bits in module = 1102 bits.
//
//195 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread .cctor uid=cctor10 has 3 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread .cctor uid=cctor14 has 3 CIL instructions in 1 basic blocks
//Thread Main uid=Main10 has 103 CIL instructions in 24 basic blocks
//Thread mpc10 has 14 bevelab control states (pauses)
//Reindexed thread xpc10 with 23 minor control states
// eof (HPR L/S Verilog)
