// test2r1
// Generated by CBG HPR L/S stagename=cgen2
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version alpha 0.2.18b : 5th-December-2016 Linux/X86_64:koo// 08/12/2016 06:48:11
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -vnl-roundtrip=disable -report-each-step -kiwic-finish=enable -kiwic-register-colours=1 -repack-to-roms=disable test2r1.exe -vnl-resets=synchronous -vnl DUT.v -sim 1180 -diosim-vcd=test2r1.vcd -kiwic-kcode-dump=enable -give-backtrace -report-each-step

#ifndef test2r1_H
#define test2r1_H
#define HPR_NEEDS_SYSTEMC 1
#include "hprls.h"
// opath operation: 10: cgen2

struct  test2r1  : public sc_module
{
  bool test2r1_T403_Main_T403_Main_V_6_GP;
  unsigned int test2r1_T403_Main_T403_Main_V_0;
  unsigned int test2r1_T403_Main_T403_Main_V_1;
  unsigned int test2r1_T404_process_T404_process_V_0;
  bool test2r1_T404_process_T404_process_V_1_GP;
  unsigned int __SINT_CC_MAPR10NoCE1_capint;
  unsigned int __SINT_CC_MAPR10NoCE0_capint;
  unsigned int __star1__Capsule_CC_SCALbx18_datum;
  unsigned int __star1__Capsule_CC_SCALbx16_datum;
  sc_uint<1> __BOOL_CC_MAPR10NoCE1_newlinef;
  sc_uint<1> __BOOL_CC_MAPR10NoCE0_newlinef;
  sc_uint<1> __BOOL_CC_SCALbx18_emptyflag;
  sc_uint<1> __BOOL_CC_SCALbx16_emptyflag;
  sc_uint<5> xpc10nz;
  unsigned int isMULTIPLIER10RRh10hold;
  sc_uint<1> isMULTIPLIER10RRh10shot0;
  unsigned int isMULTIPLIER12RRh10hold;
  sc_uint<1> isMULTIPLIER12RRh10shot0;
  sc_uint<5> xpc12nz;
  sc_out  < sc_uint<1> > hf1_dram0bank_opreq;
  sc_in < sc_uint<1> > hf1_dram0bank_oprdy;
  sc_in < sc_uint<1> > hf1_dram0bank_ack;
  sc_out  < sc_uint<1> > hf1_dram0bank_rwbar;
  sc_out  < sc_uint<256> > hf1_dram0bank_wdata;
  sc_out  < sc_uint<22> > hf1_dram0bank_addr;
  sc_in < sc_uint<256> > hf1_dram0bank_rdata;
  sc_out  < unsigned int > hf1_dram0bank_lanes;
  unsigned int isMULTIPLIER12_RR;
  unsigned int isMULTIPLIER12_A0;
  unsigned int isMULTIPLIER12_A1;
  sc_uint<1> isMULTIPLIER12_err;
  sc_in < sc_uint<1> > clk;
  sc_in < sc_uint<1> > reset;
  unsigned int isMULTIPLIER10_RR;
  unsigned int isMULTIPLIER10_A0;
  unsigned int isMULTIPLIER10_A1;
  sc_uint<1> isMULTIPLIER10_err;

  test2r1( sc_core::sc_module_name name)  :    sc_core::sc_module("test2r1")
   { 
       SC_HAS_PROCESS(test2r1);
SC_METHOD(new SystemCsharp.sc_method_t(exe10610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe10010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe9010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe8010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe7010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6510)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6410)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6310)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe6010)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5910)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5810)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5710)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5610)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe5510)).sensitive_pos(clk);
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
SC_METHOD(new SystemCsharp.sc_method_t(exe210)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe110)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe014)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe012)).sensitive_pos(clk);
SC_METHOD(new SystemCsharp.sc_method_t(exe010)).sensitive_pos(clk);
  }

   void exe014();

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

   void exe5510();

   void exe5610();

   void exe5710();

   void exe5810();

   void exe5910();

   void exe6010();

   void exe6110();

   void exe6210();

   void exe6310();

   void exe6410();

   void exe6510();

   void exe6610();

   void exe6710();

   void exe6810();

   void exe6910();

   void exe7010();

   void exe7110();

   void exe7210();

   void exe7310();

   void exe7410();

   void exe7510();

   void exe7610();

   void exe7710();

   void exe7810();

   void exe7910();

   void exe8010();

   void exe8110();

   void exe8210();

   void exe8310();

   void exe8410();

   void exe8510();

   void exe8610();

   void exe8710();

   void exe8810();

   void exe8910();

   void exe9010();

   void exe9110();

   void exe9210();

   void exe9310();

   void exe9410();

   void exe9510();

   void exe9610();

   void exe9710();

   void exe9810();

   void exe9910();

   void exe10010();

   void exe10110();

   void exe10210();

   void exe10310();

   void exe10410();

   void exe10510();

   void exe10610();

   void exe012();

   void exe010();
};

#endif
// eof test2r1
