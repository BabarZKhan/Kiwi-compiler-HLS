// test9
// Generated by CBG Orangepath HPR/LS
// 6/30/2011 8:42:54 AM
//  /home/djg11/d320/hprls/kiwic/distro/lib/kiwic.exe -report-each-step -kiwic-oldunwind=enable -csharp-gen=enable test9.exe /home/djg11/d320/hprls/kiwic/distro/support/Kiwi.dll /home/djg11/d320/hprls/kiwic/distro/support/Kiwic.dll -finish true -vnl test9.v -sim 1800

using SystemCsharp;
// opath operation: 8: csharp-gen

class  test9  : sc_module
{
  sc_signal Ttpa0_3_V_0;
  sc_signal mpc10;

  test9( string name)  :    base("test9")
   { 
      Ttpa0_3_V_0 = (new sc_signal(32, "Ttpa0_3_V_0"));
      mpc10 = (new sc_signal(32, "mpc10"));
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe12)).sensitive_pos(/*ub*/clk);
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe10)).sensitive_pos(/*ub*/clk);
  }

   void exe10()
   { 
    //  Method no 0 mname=test9

    {      if (((mpc10.Read())==(0)))
       hpr_write("%s%d", " ", 0x0ff&"Hello World\n"[0]);
      if (((mpc10.Read())==(2))&&((/*ub*/Ttpa0_3_V_0)<(11)))
       hpr_write("%s%d", " ", 0x0ff&"Hello World\n"[1+/*ub*/Ttpa0_3_V_0]);
      if (((mpc10.Read())==(2))&&((/*ub*/Ttpa0_3_V_0)>=(11)))
       hpr_writeln("\nDone");
      if (((mpc10.Read())==(2))&&((/*ub*/Ttpa0_3_V_0)>=(11)))
       hpr_sysexit(0);

    }
  }

   void exe12()
   { 
    //  Method no 1 mname=test9

    {      if (((mpc10.Read())==(0)))
      mpc10.Write(2/*2:mpc10XS:":pcIS29:AG"*/);
      if (((mpc10.Read())==(2)))
      mpc10.Write((((/*ub*/Ttpa0_3_V_0)>=(11)))? 1/*1:mpc10XS:":pcIS-10:AG"*/:(((/*ub*/Ttpa0_3_V_0)<(11)))? 2/*2:mpc10*/:mpc10.Read());
      if (((mpc10.Read())==(0)))
      /*ub*/Ttpa0_3_V_0.Write(0);
      if (((mpc10.Read())==(2)))
      /*ub*/Ttpa0_3_V_0.Write(1+/*ub*/Ttpa0_3_V_0);

    }
  }
};

// eof test9
