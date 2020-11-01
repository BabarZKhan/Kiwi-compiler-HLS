// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test67: sine/cos/tan/sqrt/log/exp
// Test67c: SqRootFp
// Test67d: Sine/Cosine

using System;
using System.Text;
using KiwiSystem;

// We expect Sqrt, Sin and Cos to be readily available from the standard math library.
// Such methods are defined in Kiwic.cs and marked up as Kiwi.Remote loadable modules.


class Test67c_KiwiFpSineCosineTest
{

  [Kiwi.OutputWordPort(63,0)] static double win_monitor;
  [Kiwi.OutputWordPort(63,0)] static double wout_monitor;

  public static void run()
  {
    Console.WriteLine("-------------------------------------------");    
    Console.WriteLine("Test67c  Sine/Cosine Double-Precision Test.");    
    for (int px=6; px!=2; px--)
      {
	Kiwi.Pause();
	double myarg =
             (px==6)? 2.0:
          (px==5)? 0.01:
             (px==4)? 1e3:
                (double)px * 1.9;

	double dsr = HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin(myarg);
        win_monitor = myarg;
        wout_monitor = dsr;
	Console.WriteLine("D/P Test Sine/Cosine  arg={0}  result={1}", myarg, dsr);
    //Console.WriteLine("            libans={0}", Math.Sqrt(myarg));

	Console.WriteLine("\n\n");
      }

    Console.WriteLine("-------------------------------------------");    
    Console.WriteLine("Test67d  Sine/Cosine Single-Precision Test.");    
    for (int px=6; px!=2; px--)
      {
	Kiwi.Pause();
	float myarg =
             (px==6)? 2.0f:
          (px==5)? 0.01f:
             (px==4)? 1e3f:
                (float)px * 1.9f;

	float ssr = HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin(myarg);
        win_monitor = (double)myarg;
        wout_monitor = (double)ssr;
	Console.WriteLine("S/P Test Sine/Cosine  arg={0}  result={1}", myarg, ssr);
    //Console.WriteLine("            libans={0}", Math.Sqrt(myarg));

	Console.WriteLine("\n\n");
      }
    Console.WriteLine("-------------------------------------------");    
  }

}

class Test67d
{

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Kiwi.Pause();
    Console.WriteLine("Test67d starting. Kiwi SineCosineTest");
    Kiwi.Pause();

    Test67c_KiwiFpSineCosineTest.run();
    Console.WriteLine("Test67d finished.");
    //done = true;
    Kiwi.Pause();
  }
}

// eof