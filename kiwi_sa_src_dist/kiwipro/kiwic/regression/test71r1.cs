// Kiwi Scientific Acceleration - Exception Testing -
//
// Regression test test71r1 for try/finally without catch.
//
// (C) 2017 DJ Greaves - University of Cambridge Computer Laboratory.
//

using System;
using System.Text;
using KiwiSystem;

//
// FinallySimple71r1 - complex finally blocks - no catching
// FinallySimple71r2 - complex finally blocks - with catching 


class FinallySimple
{

  [Kiwi.OutputBitPort("done")] static bool done = false;

  public static int FinallySimpleTest()
     {
       Kiwi.Pause();
       try
         {
           Console.WriteLine("FinallySimple71r1 Test: Enter Top Try");
           for (int pp=0; pp<10; pp++)
           {
               Console.WriteLine("\nFinallySimple71r1 Test: Enter Loop Body");
               if ((pp & 1)==0) try
                   {
                        Console.WriteLine("FinallySimple71r1 Test: Inner Try A pp={0}", pp);
                        if (pp == 5) return 101;
                        if (pp == 8) return 808; // Second exceptional exit from this block (will not be invoked of course).
                   }
                finally
                   {
                        Console.WriteLine("FinallySimple71r1 Test: Inner Finally A. pp={0}", pp);
                   }
               else try
                   {
                        Console.WriteLine("FinallySimple71r1 Test: Inner Try B. pp={0}", pp);
                        if (pp == 5) return 505;
                   }
                finally
                   {
                      for (int qqq=42; qqq<2929; qqq+=888)
                        Console.WriteLine("FinallySimple71r1 Test: Inner Finally B. pp={0} qqq={1}", pp, qqq);
                   }
           }
           return 429;
         }


      finally
        {
          Console.WriteLine("FinallySimple71r1 Test: Final handler");
        }

      return 212;

    }


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
     {
       Kiwi.Pause();
       Console.WriteLine("Start FinallySimple71r1 Test");
       int rc = FinallySimpleTest();
       Kiwi.Pause();
       Console.WriteLine("FinallySimple71r1 Test: End of Demo. rc={0}", rc);      
       Kiwi.Pause();
       done = true;
    }

}


// eof
