// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// test66.cs - RTL Parameters
//
// 

using System;
using System.Text;
using KiwiSystem;


class RTLprams1
{
  //  These should be parameters to the generated RTL that can be redefined at module instantiation time.
  //  If you loook in the h02_kiwife/report-full file you should seem them listed as declarations with attribute rtl_parameter=true and referenced in the code.
  [Kiwi.RtlParameter("rtl_pram1", 1001)] public static int rtl_pram1 = 1001;
  [Kiwi.RtlParameter("rtl_pram2")] public static int rtl_pram2;

  static readonly short[] Box2 = new short[] { 22, 33, 44, 55, 66, 77 };
  

  public static void Run66()
  {
    for (int p=0; p<3;p++)
      {
        Kiwi.NoUnroll();
        Console.WriteLine("  {0}  RTL Parameterising {1},  {2}", p, Box2[3]+rtl_pram1, rtl_pram2+p);
      }
  }
}

class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false;



  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test66 starting.");
    //RTLprams1 app = new RTLprams1();
    RTLprams1.Run66();
    Console.WriteLine("Test66 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof