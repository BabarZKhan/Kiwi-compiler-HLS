//
//Kiwi Project: SystemCsharp :  DJ Greaves and S Singh.
//
//Compilation of parallel programs written in C# to H/W
//(C) 2011 University of Cambridge and Microsoft Research
//
// $Id: Testbench.cs,v 1.1 2011/07/03 16:00:27 djg11 Exp $
//
//All of the IP and software in the compiler directory and its
//subdirectories belongs to David Greaves of the University of
//Cambridge.  It cannot be copied, distributed or used commercially
//without explicit permission.  This is experimental software: no
//guarantees are implied and no liability is assumed for consequential
//loss.
// 
// KiwiC regressions testbench.
//
//
//

using System;
using SystemCsharp;
using DUT.sysc;

class Bench : SystemCsharp.sc_module
{

  public SystemCsharp.sc_in<bool> clk, reset;
  public DUT dut1 = new DUT("dut1");

  void step() 
  {

    Console.WriteLine("step"); // + this.ToString());
  }

  public Bench(SystemCsharp.sc_module parent, String instance_name)  : base(instance_name)
  {
    SystemCsharp.SC.Register(parent, this, instance_name); // Can get type from reflection?
    clk = new SystemCsharp.sc_in<bool>(this);
    reset  = new SystemCsharp.sc_in<bool>(this);
//    SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(this.step));
    dut1.clk.bind(clk);
    dut1.reset.bind(reset);
  }


}


class resetgen : SystemCsharp.sc_module
{
  Int64 period;
  public SystemCsharp.sc_out<bool> reset;

  public resetgen(SystemCsharp.sc_module parent, String instance_name, Int64 period) : base(instance_name)
  {
    this.period = period;
    SystemCsharp.SC.Register(parent, this, instance_name); // Can get type from reflection?
    reset = new SystemCsharp.sc_out<bool>(this); // this, "reset");
//    var s = SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(this.step));
  //  s.sensitive(reset);
  }

  public override void end_of_elaboration(int code)
  {
    reset.Write(1, 0L);
    reset.Write(0, 7L * period/2L);
  }
}


public clkgen(SystemCsharp.sc_module parent, String instance_name, Int64 period) : base(instance_name)
  {
    this.period = period;
    SystemCsharp.SC.Register(parent, this, instance_name); // Can get type from reflection?
    clk = new SystemCsharp.sc_out<bool>(this); // this, "clk");
    var s = SystemCsharp.SC.sc_method(new SystemCsharp.sc_method_t(this.step));
    s.sensitive(clk);
  }

  public override void end_of_elaboration(int code)
  {
    clk.Write(1-(clk.Read() & 1L), period/2L);
  }
}






class circuit : SystemCsharp.sc_module
{
  public SystemCsharp.sc_signal clk = new SystemCsharp.sc_signal(1, "clk");
  public SystemCsharp.sc_signal reset = new SystemCsharp.sc_signal(1, "reset");
//  public SystemCsharp.sc_signal bus2 = new SystemCsharp.sc_signal(5, "bus2");

  resetgen resetgen1;
  clkgen clkgen1;
  Bench bench1;

  public circuit(SystemCsharp.sc_module parent, String instance_name)  : base(instance_name)
  {
    SystemCsharp.SC.Register(parent, this, instance_name); // Can get type from reflection?
    bench1 =  new Bench(this, "bench1");

    clkgen1  =  new clkgen(this, "clkgen1", 100);
    clkgen1.clk.bind(clk);
    bench1.clk.bind(clk);
    resetgen1  =  new resetgen(this, "resetgen1", 100);
    resetgen1.reset.bind(reset);
    bench1.reset.bind(reset);
  }

}



public class SIMSYS
{
   static circuit cct = new circuit(null, "toplevel");
   public static void Main()
   {
     Console.WriteLine("Starting 1");
     SystemCsharp.SC.end_of_elaboration();
     Console.WriteLine("Starting 2");

     var tr = new SystemCsharp.Trace_vcd("a.vcd");
     tr.Trace(cct.clk);
     tr.Trace(cct.reset);

     SystemCsharp.SC.sc_start(10000);
     Console.WriteLine("Finished at {0}", SystemCsharp.SC.tnow);

     tr.Close();
   }
}

// eof
