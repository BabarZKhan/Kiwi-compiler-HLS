// Kiwi Scientific Acceleration - Exception Testing -
//
// Regression test test71r2 for try/finally without catch.
//
// (C) 2017 DJ Greaves - University of Cambridge Computer Laboratory.
//

using System;
using System.Text;
using KiwiSystem;

//
// FinallySimple71r2 - complex finally blocks - no catching
// FinallySimple71r2 - complex finally blocks - with catching 

public class EmilyLights : System.Exception
{
  
}

class EmilyExceptionTest
{

  [Kiwi.OutputBitPort("done")] static bool done = false;

  public static int SBodyTest()
     {
       Kiwi.Pause();
       try
         {
           Console.WriteLine("EmilyException71r2 Test: Enter Top Try");
           for (int pp=0; pp<10; pp++)
           {
               Console.WriteLine("\nEmilyException71r2 Test: Enter Loop Body");
               if ((pp & 1)==0) try
                   {
                        Console.WriteLine("EmilyException71r2 Test: Inner Try A pp={0}", pp);
                        if (pp == 5) return 101;
                   }
                finally
                   {
                        Console.WriteLine("EmilyException71r2 Test: Inner Finally A. pp={0}", pp);
                   }
               else try
                   {
                        Console.WriteLine("EmilyException71r2 Test: Inner Try B. pp={0}", pp);
                        if (pp == 5) throw (new EmilyLights());
                   }
                finally
                   {
                        Console.WriteLine("EmilyException71r2 Test: Inner Try Finally B. pp={0}", pp);
                   }
           }
           return 429;
         }

      catch (AggregateException e)
         {
            Console.WriteLine("An action has thrown an exception. THIS WAS UNEXPECTED.\n{0}", e.InnerException.ToString());
         }

      catch (EmilyLights em)
         {
            Console.WriteLine("Emily has been caught\n{0}", em.InnerException.ToString());
         }

      finally
        {
          Console.WriteLine("EmilyException71r2 Test: Final handler");
        }

      return 212;

    }


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
     {
       Kiwi.Pause();
       Console.WriteLine("Start EmilyException71r2 Test");
       int rc = SBodyTest();
       Kiwi.Pause();
       Console.WriteLine("EmilyException71r2 Test: End of Demo. rc={0}", rc);      
       Kiwi.Pause();
       done = true;
    }

}


// eof
