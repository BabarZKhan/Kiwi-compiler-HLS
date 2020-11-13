// test26
// Generated by CBG Orangepath HPR/LS
// 8/4/2011 8:42:33 AM
//  /home/djg11/d320/hprls/kiwic/distro/lib/kiwic.exe -report-each-step -csharp-gen=enable -gtrace-loglevel=0 -firstpass-loglevel=0 test26.exe /home/djg11/d320/hprls/kiwic/distro/support/Kiwi.dll /home/djg11/d320/hprls/kiwic/distro/support/Kiwic.dll -finish true -sim 1800 -conerefine disable -bevelab disable -verilog-gen=disable

using SystemCsharp;
// opath operation: 8: csharp-gen

public class  test26  : sc_module
{
  public sc_signal test26_dc;
  public sc_signal tedc_not_used;
  public sc_signal edict_zulu;
  public sc_signal edict_moscow;
  public sc_signal n0_dereftro10_ptr;
  public sc_signal tTMT4Main_V_1;
  public sc_signal tTMT4Main_V_0;
  public sc_signal HPRtop;
  uint []DRSX32SS_AX_CC_SOL = new uint [ 5];
  uint []tedc_AX_CC_SOL = new uint [ 2];
  public sc_out  < uint > dout;
  public sc_in < uint > din;

  public test26( string name)  :    base("test26")
   { 
      test26_dc = (new sc_signal(32, "test26_dc"));
      tedc_not_used = (new sc_signal(32, "tedc_not_used"));
      edict_zulu = (new sc_signal(32, "edict_zulu"));
      edict_moscow = (new sc_signal(32, "edict_moscow"));
      n0_dereftro10_ptr = (new sc_signal(32, "n0_dereftro10_ptr"));
      tTMT4Main_V_1 = (new sc_signal(32, "tTMT4Main_V_1"));
      tTMT4Main_V_0 = (new sc_signal(32, "tTMT4Main_V_0"));
      HPRtop = (new sc_signal(32, "HPRtop"));
SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(exe10)).sensitive_pos(clk);
  }

   int exe10()
   { 
    //  Method no 0 mname=test10
      /*skip*/;    // After all generate loops are unwound we are here: 0
    T400_0:
      /*skip*/;      n0_dereftro10_ptr.Write(400000);
      tTMT4Main_V_0.Write(400000);
      tTMT4Main_V_1.Write(32);
      DRSX32SS_AX_CC_SOL[2] = (22);
      DRSX32SS_AX_CC_SOL[3] = (42);
      tedc_AX_CC_SOL[1] = (400000);
wait();
hpr_write("%d%s", DRSX32SS_AX_CC_SOL[3], " ");
      DRSX32SS_AX_CC_SOL[3] = (2+DRSX32SS_AX_CC_SOL[3]);
      tTMT4Main_V_1.Write(2+DRSX32SS_AX_CC_SOL[3]);
      return (0);
  }
};

// eof test26