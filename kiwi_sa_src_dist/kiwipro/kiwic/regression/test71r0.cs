// Kiwi Scientific Acceleration - Exception Testing -
//
// Regression test test71r0 for try/finally without catch.
//
// (C) 2017 DJ Greaves - University of Cambridge Computer Laboratory.
//

using System;
using System.Text;
using KiwiSystem;

//
// FinallySimple71r0 - simple final block
// FinallySimple71r0 - complex finally blocks - no catching
// FinallySimple71r2 - complex finally blocks - with catching 


class FinallySimpleTest
{

  [Kiwi.OutputBitPort("done")] static bool done = false;

  public static int SBodyTest()
     {
       Kiwi.Pause();
       try
         {
           Console.WriteLine("FinallySimple71r0 Test: Enter Top Try");
           for (int pp=0; pp<10; pp++)
           {
                Console.WriteLine("FinallySimple71r0 Test: Inner Try A pp={0}", pp);
                if (pp == 5) return 2101;
           }
           return 3429;
         }

      finally
        {
          Console.WriteLine("FinallySimple71r0 Test: Final handler");
        }

      return 4212;

    }


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
     {
       Kiwi.Pause();
       Console.WriteLine("Start FinallySimple71r0 Test");
       int rc = SBodyTest();
       Kiwi.Pause();
       Console.WriteLine("FinallySimple71r0 Test: End of Demo. rc={0}", rc);      
       Kiwi.Pause();
       done = true;
    }

}


// eof
