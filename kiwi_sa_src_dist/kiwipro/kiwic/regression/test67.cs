// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test67: sine/cos/tan/sqrt/log/exp
// 

using System;
using System.Text;
using KiwiSystem;

// We expect Sin and Cos to be readily available from the standard math library.
// Such methods are defined in kickoff libraries and have a redirection from System.Math to KiwiSystem.Math in the g_hardcoded_library_substitutions structure or agumented by recipe.

namespace System
{
  public class Math
  {
     public static double Sin(double arg) // Temporary local definition
     {
        return arg;
     }
  }
}

class SineCosTest
{
  public static void run()
  {
    Console.WriteLine("Test67  sine and cosine starting.");    
    for (int px=3; px>=0; px--)
      {
	Kiwi.Pause();
	Kiwi.Pause();
	Kiwi.Pause();
	double angle = (double)px * 1.9;
	double sr = Math.Sin(angle);
	double cr = 2.2; //  Math.Cos(angle);
	Console.WriteLine("  sine and cosine {0}  {1}  {2}", angle, sr, cr);
      }
  }

}

class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false;



  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test67  starting.");
    SineCosTest.run();
    Console.WriteLine("Test67 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof