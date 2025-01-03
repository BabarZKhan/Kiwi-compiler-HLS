// test14x
// Generated by CBG HPR L/S stagename=cgen2
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.18b : 5th-December-2016 Linux/X86_64:koo// 08/12/2016 06:51:33
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable test14x.exe -sim 1800 -vnl DUT.v -res2-regfile-threshold=121 -res2-combram-threshold=122 -give-backtrace -report-each-step

#ifndef test14x_H
#define test14x_H
#define HPR_NEEDS_SYSTEMC 1
#include "hprls.h"
// opath operation: 10: cgen2

struct  test14x  : public sc_module
{
  unsigned int test14x_T402_Main_T402_Main_V_2;
  sc_uint<1> test14x_T402_Main_T402_Main_V_0a;
  sc_uint<1> test14x_T402_Main_T402_Main_V_1a;
  unsigned int __SINT_CC_MAPR10NoCE1_left;
  unsigned int __SINT_CC_MAPR10NoCE0_left;
  unsigned int __SINT_CC_MAPR12NoCE1_SCALARA1_0;
  unsigned int __SINT_CC_MAPR12NoCE0_SCALARA1_0;
  unsigned int __SINT_CC_MAPR12NoCE1_SCALARB0_0;
  unsigned int __SINT_CC_MAPR12NoCE0_SCALARB0_0;
  unsigned int __s__SINT_CC_MAPR10NoCE0_arrow;
  unsigned int __s__SINT_CC_MAPR10NoCE1_arrow;
  sc_uint<3> xpc10nz;
  sc_in < sc_uint<1> > clk;
  sc_out  < sc_uint<1> > hf1_dram0bank_opreq;
  sc_in < sc_uint<1> > hf1_dram0bank_oprdy;
  sc_in < sc_uint<1> > hf1_dram0bank_ack;
  sc_out  < sc_uint<1> > hf1_dram0bank_rwbar;
  sc_out  < sc_uint<256> > hf1_dram0bank_wdata;
  sc_out  < sc_uint<22> > hf1_dram0bank_addr;
  sc_in < sc_uint<256> > hf1_dram0bank_rdata;
  sc_out  < unsigned int > hf1_dram0bank_lanes;

  test14x( sc_core::sc_module_name name)  :    sc_core::sc_module("test14x")
   { 
       SC_HAS_PROCESS(test14x);
SC_METHOD(new SystemCsharp.sc_method_t(exe3010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe2010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe1010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe010)).sensitive_pos(clk);
  }

   void exe010();

   void exe110();

   void exe210();

   void exe310();

   void exe410();

   void exe510();

   void exe610();

   void exe710();

   void exe810();

   void exe910();

   void exe1010();

   void exe1110();

   void exe1210();

   void exe1310();

   void exe1410();

   void exe1510();

   void exe1610();

   void exe1710();

   void exe1810();

   void exe1910();

   void exe2010();

   void exe2110();

   void exe2210();

   void exe2310();

   void exe2410();

   void exe2510();

   void exe2610();

   void exe2710();

   void exe2810();

   void exe2910();

   void exe3010();
};

#endif
// eof test14x
