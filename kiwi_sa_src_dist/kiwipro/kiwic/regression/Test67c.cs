// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test67: sine/cos/tan/sqrt/log/exp
// Test67c: SqRootFp

using System;
using System.Text;
using KiwiSystem;

// We expect Sqrt, Sin and Cos to be readily available from the standard math library.
// Such methods are defined in Kiwic.cs and marked up as Kiwi.Remote loadable modules.


class Test67c_KiwiFpSqRootTest
{

  [Kiwi.OutputWordPort(63,0)] static double win_monitor;
  [Kiwi.OutputWordPort(63,0)] static double wout_monitor;

  public static void run()
  {
    Console.WriteLine("-------------------------------------------");    
    Console.WriteLine("Test67c  Square Root Double-Precision Test.");    
    for (int px=6; px!=2; px--)
      {
	Kiwi.Pause();
	double myarg =
             (px==6)? 2.0:
             (px==5)? 200000.0:
             (px==4)? 1e100:
                (double)px * 1.9;

	double dsr = HprlsMathsPrimsCrude.KiwiFpSqRoot.Sqrt(myarg);
        win_monitor = myarg;
        wout_monitor = dsr;
	Console.WriteLine("D/P Test Square Root  arg={0}  result={1}", myarg, dsr);
    //Console.WriteLine("            libans={0}", Math.Sqrt(myarg));
	Console.WriteLine("            D/P subsback={0}", dsr*dsr);
	Console.WriteLine("\n\n");
      }

    Console.WriteLine("-------------------------------------------");    
    Console.WriteLine("Test67c  Square Root Single-Precision Test.");    
    for (int px=6; px!=2; px--)
      {
	Kiwi.Pause();
	float myarg =
             (px==6)? 2.0f:
             (px==5)? 200000.0f:
             (px==4)? 1e30f:
                (float)px * 1.9f;

	float ssr = HprlsMathsPrimsCrude.KiwiFpSqRoot.Sqrt(myarg);
        win_monitor = (double)myarg;
        wout_monitor = (double)ssr;
	Console.WriteLine("S/P Test Square Root  arg={0}  result={1}", myarg, ssr);
    //Console.WriteLine("            libans={0}", Math.Sqrt(myarg));
	Console.WriteLine("            S/P subsback={0}", ssr*ssr);
	Console.WriteLine("\n\n");
      }
    Console.WriteLine("-------------------------------------------");    
  }

}

class Test67c
{
  [Kiwi.OutputBitPort("done")] static bool done = false;



  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Kiwi.Pause();
    Console.WriteLine("Test67 starting. Kiwi SqRootTest");
    Kiwi.Pause();

    Test67c_KiwiFpSqRootTest.run();
    Console.WriteLine("Test67 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof