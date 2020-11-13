// DUT
// Generated by CBG Orangepath HPR/LS
// 6/30/2011 8:58:20 AM
//  /home/djg11/d320/hprls/kiwic/distro/lib/kiwic.exe -report-each-step -kiwic-oldunwind=enable -csharp-gen=enable test8.exe /home/djg11/d320/hprls/kiwic/distro/support/Kiwi.dll /home/djg11/d320/hprls/kiwic/distro/support/Kiwic.dll -finish true -conerefine disable -bevelab disable -verilog-gen=disable -root test8;test8.Main -vnl VNL.v -sim 1800

using SystemCsharp;
// opath operation: 8: csharp-gen

class  DUT  : sc_module
{
  sc_signal edict_zulu;
  sc_signal edict_moscow;
  sc_signal tTMT4Main_V_1;
  sc_signal tTMT4Main_V_0;
  sc_signal HPRtop;

  DUT( string name)  :    base("DUT")
   { 
      edict_zulu = (new sc_signal(32, "edict_zulu"));
      edict_moscow = (new sc_signal(32, "edict_moscow"));
      tTMT4Main_V_1 = (new sc_signal(32, "tTMT4Main_V_1"));
      tTMT4Main_V_0 = (new sc_signal(32, "tTMT4Main_V_0"));
      HPRtop = (new sc_signal(32, "HPRtop"));
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe10)).sensitive_pos(/*ub*/clk);
  }

   int exe10()
   { 
    //  Method no 0 mname=Main10
      /*skip*/;    startplace10:
      goto etarget10;
    etarget10:
    // killed return at end of rip.ctor
      /*skip*/;    startplace14:
      tTMT4Main_V_0.Write(32);
hpr_writeln("Test Call By Reference\n");
      tTMT4Main_V_0.Write(1);
      /*skip*/;    ucdest16:
      /*skip*/;    ucdest18:
hpr_write("%d%s", tTMT4Main_V_0.Read(), " ");
      tTMT4Main_V_0.Write(2+tTMT4Main_V_0.Read());
      tTMT4Main_V_1.Write(2+tTMT4Main_V_0.Read());
wait();
      tTMT4Main_V_0.Write(1+tTMT4Main_V_0.Read());
    //  src lab: Main : LL43
    //   Quad: None, Some ucdest18

      if (!(((tTMT4Main_V_0.Read())>=(21)))) goto ucdest18;
      /*skip*/;    disp10:
      return (0);
  }
};

// eof DUT