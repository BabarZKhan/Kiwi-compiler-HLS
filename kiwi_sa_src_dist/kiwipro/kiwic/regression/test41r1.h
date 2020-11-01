// test41r1
// Generated by CBG HPR L/S stagename=cgen2
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.18b : 5th-December-2016 Linux/X86_64:koo// 08/12/2016 06:56:38
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable test41r1.exe -sim 1800 -vnl-resets=synchronous -vnl DUT.v -give-backtrace -report-each-step

#ifndef test41r1_H
#define test41r1_H
#define HPR_NEEDS_SYSTEMC 1
#include "hprls.h"
// opath operation: 10: cgen2

struct  test41r1  : public sc_module
{
  unsigned int test41r1_T402_Main_T402_Main_V_3_GP;
  unsigned int test41r1_T402_Main_T402_Main_V_4;
  sc_uint<5> xpc10nz;
  unsigned int SINTCCSCALbx10ARA0RRh10hold;
  sc_uint<1> SINTCCSCALbx10ARA0RRh10shot0;
  unsigned int SINTCCSCALbx10ARA0RRh12hold;
  sc_uint<1> SINTCCSCALbx10ARA0RRh12shot0;
  unsigned int isDIVIDER10RRh10hold;
  sc_uint<1> isDIVIDER10RRh10primed;
  sc_uint<1> isDIVIDER10RRh10vld;
  sc_uint<1> xpc10_stall;
  sc_uint<1> xpc10_clear;
  sc_out  < sc_uint<1> > hf1_dram0bank_opreq;
  sc_in < sc_uint<1> > hf1_dram0bank_oprdy;
  sc_in < sc_uint<1> > hf1_dram0bank_ack;
  sc_out  < sc_uint<1> > hf1_dram0bank_rwbar;
  sc_out  < sc_uint<256> > hf1_dram0bank_wdata;
  sc_out  < sc_uint<22> > hf1_dram0bank_addr;
  sc_in < sc_uint<256> > hf1_dram0bank_rdata;
  sc_out  < unsigned int > hf1_dram0bank_lanes;
  sc_uint<1> isDIVIDER10_rdy;
  sc_uint<1> isDIVIDER10_req;
  unsigned int isDIVIDER10_RR;
  unsigned int isDIVIDER10_NN;
  unsigned int isDIVIDER10_DD;
  sc_uint<1> isDIVIDER10_err;
  unsigned int __SINT_CC_SCALbx10_ARA0[10000];
  sc_in < sc_uint<1> > clk;
  sc_in < sc_uint<1> > reset;
  unsigned int __SINT_CC_SCALbx10_ARA0_RDD0;
  sc_uint<14> __SINT_CC_SCALbx10_ARA0_AD0;
  sc_uint<1> __SINT_CC_SCALbx10_ARA0_WEN0;
  sc_uint<1> __SINT_CC_SCALbx10_ARA0_REN0;
  unsigned int __SINT_CC_SCALbx10_ARA0_WRD0;

  test41r1( sc_core::sc_module_name name)  :    sc_core::sc_module("test41r1")
   { 
       SC_HAS_PROCESS(test41r1);
SC_METHOD(new SystemCsharp.sc_method_t(exe5410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe4010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe3110)).sensitive_pos(clk);
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
SC_METHOD(new SystemCsharp.sc_method_t(exe212)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe114)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe014)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe112)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe012)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe210));
SC_METHOD(new SystemCsharp.sc_method_t(exe110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe010)).sensitive_pos(clk);
  }

   void exe014();

   void exe114();

   void exe212();

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

   void exe3110();

   void exe3210();

   void exe3310();

   void exe3410();

   void exe3510();

   void exe3610();

   void exe3710();

   void exe3810();

   void exe3910();

   void exe4010();

   void exe4110();

   void exe4210();

   void exe4310();

   void exe4410();

   void exe4510();

   void exe4610();

   void exe4710();

   void exe4810();

   void exe4910();

   void exe5010();

   void exe5110();

   void exe5210();

   void exe5310();

   void exe5410();

   void exe012();

   void exe112();

   void exe010();

   void exe110();

   void exe210();
};

#endif
// eof test41r1
