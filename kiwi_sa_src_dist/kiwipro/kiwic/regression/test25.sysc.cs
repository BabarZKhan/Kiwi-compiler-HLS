// test25
// Generated by CBG Orangepath HPR/LS
// 8/3/2011 11:12:23 AM
//  /home/djg11/d320/hprls/kiwic/distro/lib/kiwic.exe -report-each-step -csharp-gen=enable -gtrace-loglevel=0 -firstpass-loglevel=0 test25.exe /home/djg11/d320/hprls/kiwic/distro/support/Kiwi.dll /home/djg11/d320/hprls/kiwic/distro/support/Kiwic.dll -finish true -diosim-vcd=test25.vcd -sim 1800

using SystemCsharp;
// opath operation: 8: csharp-gen

public class  test25  : sc_module
{
  public sc_signal tTMT4Main_V_0;
  public sc_signal tTMT4Main_V_1;
  public sc_signal tTMT4Main_V_2;
  public sc_signal mpc10;
  public sc_in < bool > clk;
  public sc_out  < uint > dout;
  public sc_in < uint > din;

  public test25( string name)  :    base("test25")
   { 
      clk = (new sc_in<bool>(this, "clk"));
      dout = (new sc_out<uint>(this, "dout"));
      din = (new sc_in<uint>(this, "din"));
      tTMT4Main_V_0 = (new sc_signal(1, "tTMT4Main_V_0"));
      tTMT4Main_V_1 = (new sc_signal(1, "tTMT4Main_V_1"));
      tTMT4Main_V_2 = (new sc_signal(32, "tTMT4Main_V_2"));
      mpc10 = (new sc_signal(32, "mpc10"));
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe12)).sensitive_pos(clk);
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe10)).sensitive_pos(clk);
  }

   void exe10()
   { 
    //  Method no 0 mname=test25

    {      switch (mpc10.Read())     {
 
    case 2:
    
    {      if (((0<(100+dout.Read())%2)?false:(tTMT4Main_V_1.Read()!=0L)&&(tTMT4Main_V_0.Read()!=0L)))
      if (((tTMT4Main_V_2.Read())<(19)))
       hpr_writeln("dout=%u", 101+101+din.Read());
      if (((0<dout.Read()%2)?(((tTMT4Main_V_0.Read()!=0L))?false:((tTMT4Main_V_2.Read())<(19))):(((tTMT4Main_V_1.Read()!=0L))?(((tTMT4Main_V_0.Read()!=0L))?false:((tTMT4Main_V_2.Read())<(19))):((tTMT4Main_V_2.Read())<(19)))))
       hpr_writeln("dout=%u", 1+1+din.Read());
      if (((tTMT4Main_V_2.Read())>=(19)))
      if ((tTMT4Main_V_0.Read()!=0L))

    {      if (((0<(100+dout.Read())%2)?false:(tTMT4Main_V_1.Read()!=0L)))

    {       hpr_writeln("dout=%u", 101+101+din.Read());
       hpr_sysexit(0);

    }
      if (((0<dout.Read()%2)?false:!((tTMT4Main_V_1.Read()!=0L))))

    {       hpr_writeln("dout=%u", 1+1+din.Read());
       hpr_sysexit(0);

    }

    }

     else       if (0>=dout.Read()%2)

    {       hpr_writeln("dout=%u", 1+1+din.Read());
       hpr_sysexit(0);

    }

     else 
    {       hpr_writeln("dout=%u", 1+1+din.Read());
       hpr_sysexit(0);

    }

    }
    break;
    case 4:
           hpr_writeln("dout=%u", 1+1+din.Read());
    break; }      if (((mpc10.Read())==(4))&&((tTMT4Main_V_2.Read())>=(19)))
       hpr_sysexit(0);

    }
  }

   void exe12()
   { 
    //  Method no 1 mname=test25
      switch (mpc10.Read())     {
 
    case 0:
    
    {      mpc10.Write(2/*2:mpc10XS:":pcIS10:AG"*/);
      tTMT4Main_V_0.Write(0);
      tTMT4Main_V_1.Write(0);
      tTMT4Main_V_2.Write(0);

    }
    break;
    case 2:
    
    {      mpc10.Write((((0<(100+dout.Read())%2)?((0<dout.Read()%2)?(((tTMT4Main_V_0.Read()!=0L))?false:((tTMT4Main_V_2.Read())>=(19))):(((tTMT4Main_V_1.Read()!=0L))?(((tTMT4Main_V_0.Read()!=0L))?false:((tTMT4Main_V_2.Read())>=(19))):((tTMT4Main_V_2.Read())>=(19)))):((0<dout.Read()%2)?(((tTMT4Main_V_1.Read()!=0L))?((tTMT4Main_V_2.Read())>=(19)):(((tTMT4Main_V_0.Read()!=0L))?false:((tTMT4Main_V_2.Read())>=(19)))):((tTMT4Main_V_2.Read())>=(19)))))?1/*1:mpc10XS:":pcIS-10:AG"*/:(0<(100+dout.Read())%2&&(tTMT4Main_V_1.Read()!=0L)&&(tTMT4Main_V_0.Read()!=0L))?4/*4:mpc10*/:(((0<(100+dout.Read())%2)?((0<dout.Read()%2)?(((tTMT4Main_V_1.Read()!=0L))?(((tTMT4Main_V_0.Read()!=0L))?false:!(((tTMT4Main_V_2.Read())>=(19)))):(tTMT4Main_V_0.Read()!=0L)||!(((tTMT4Main_V_2.Read())>=(19)))):(((tTMT4Main_V_1.Read()!=0L))?(((tTMT4Main_V_0.Read()!=0L))?false:!(((tTMT4Main_V_2.Read())>=(19)))):!(((tTMT4Main_V_2.Read())>=(19))))):((0<dout.Read()%2)?(((tTMT4Main_V_1.Read()!=0L))?!(((tTMT4Main_V_2.Read())>=(19))):(tTMT4Main_V_0.Read()!=0L)||!(((tTMT4Main_V_2.Read())>=(19)))):!(((tTMT4Main_V_2.Read())>=(19))))))?2/*2:mpc10*/:mpc10.Read());
      tTMT4Main_V_0.Write((((0<(100+dout.Read())%2)?((0<dout.Read()%2)?!((tTMT4Main_V_0.Read()!=0L)):(((tTMT4Main_V_1.Read()!=0L))?!((tTMT4Main_V_0.Read()!=0L)):true)):((0<dout.Read()%2)?(tTMT4Main_V_1.Read()!=0L)||!((tTMT4Main_V_0.Read()!=0L)):true)))?(((tTMT4Main_V_1.Read())==(0)))?1:0:tTMT4Main_V_0.Read());
      dout.Write((((0<(100+dout.Read())%2)?false:(tTMT4Main_V_1.Read()!=0L)&&(tTMT4Main_V_0.Read()!=0L)))?101+101+din.Read():(0<(100+dout.Read())%2&&(tTMT4Main_V_1.Read()!=0L)&&(tTMT4Main_V_0.Read()!=0L))?100+dout.Read():(((0<dout.Read()%2)?!((tTMT4Main_V_0.Read()!=0L)):(((tTMT4Main_V_1.Read()!=0L))?!((tTMT4Main_V_0.Read()!=0L)):true)))?1+1+din.Read():dout.Read());
      tTMT4Main_V_1.Write((((0<(100+dout.Read())%2)?((0<dout.Read()%2)?!((tTMT4Main_V_0.Read()!=0L)):(((tTMT4Main_V_1.Read()!=0L))?!((tTMT4Main_V_0.Read()!=0L)):true)):((0<dout.Read()%2)?(tTMT4Main_V_1.Read()!=0L)||!((tTMT4Main_V_0.Read()!=0L)):true)))?(((tTMT4Main_V_1.Read())==(0)))?1:0:tTMT4Main_V_1.Read());
      tTMT4Main_V_2.Write((((0<(100+dout.Read())%2)?((0<dout.Read()%2)?!((tTMT4Main_V_0.Read()!=0L)):(((tTMT4Main_V_1.Read()!=0L))?!((tTMT4Main_V_0.Read()!=0L)):true)):((0<dout.Read()%2)?(tTMT4Main_V_1.Read()!=0L)||!((tTMT4Main_V_0.Read()!=0L)):true)))?1+tTMT4Main_V_2.Read():tTMT4Main_V_2.Read());

    }
    break;
    case 4:
    
    {      mpc10.Write((((tTMT4Main_V_2.Read())<(19)))?2/*2:mpc10XS:":pcIS10:AG"*/:(((tTMT4Main_V_2.Read())>=(19)))?1/*1:mpc10XS:":pcIS-10:AG"*/:(((tTMT4Main_V_2.Read())<(19)))?2/*2:mpc10*/:mpc10.Read());
      tTMT4Main_V_0.Write((((tTMT4Main_V_1.Read())==(0)))?1:0);
      dout.Write(1+1+din.Read());
      tTMT4Main_V_1.Write((((tTMT4Main_V_1.Read())==(0)))?1:0);
      tTMT4Main_V_2.Write(1+tTMT4Main_V_2.Read());

    }
    break; }  }
};

// eof test25