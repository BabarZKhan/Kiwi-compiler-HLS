// Kiwi Scientific Acceleration - Regression test: Virtex-like native FIFO demo.
//
// (C) 2017 DJ Greaves - University of Cambridge Computer Laboratory.
//

// The third one of these three illustrates the  envisioned typical end-user use case.

// test73r1 - native atomics, such as Interlocked.Add



using System;
using System.Threading;
using KiwiSystem;

//
//




class Test73r1
{

  static int shared0 = 1100;
  static int shared1 = 1110;

  static int InterlockedTest0()
  {

    for (int qq=0; qq<4; qq++)
      {
        Interlocked.Add(ref shared0, 1);
        Console.WriteLine("Reporting {0} shared0={1}", qq, shared0);
        Kiwi.Pause();        
      }
    shared0 = 333;
    shared1 = 222;
    return 0;
  }
  
  [Kiwi.HardwareEntryPoint()]
  public static void Main()
     {
       Kiwi.Pause();
       Console.WriteLine("Interlocked.Add 73r1 Test");
       int rc = InterlockedTest0();
       Kiwi.Pause();
       Console.WriteLine("Atomics 73r1 Test: End of Demo. rc={0}", rc);      
       Kiwi.Pause();
       Kiwi.ReportNormalCompletion(); // KiwiC should perhaps pop this on the end automatically?
    }

}


// eof
