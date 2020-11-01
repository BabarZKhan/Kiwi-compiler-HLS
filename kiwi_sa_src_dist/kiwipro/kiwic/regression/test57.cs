//
// Kiwi Scientific Acceleration Regression Test - string and array base direct manipulations over pause statements.
//
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//


using System;
using System.Text;
using KiwiSystem;
using System.Diagnostics;


public class test57
{

  static void test57_phase0()
  {
    string ss1 = "Hello One";
    for (int p=0; p<4; p++)
      {
        string qq = (p>2) ? "Bonjor Number Two": ss1;
        Kiwi.Pause();
        Console.WriteLine("   stringers {0}  {1} len={2}", p, qq, qq.Length);
        Kiwi.Pause();
      }
  }

  static void test57_phase1()
  {
      int[] arr1 = new int [] {1, 222221, 5, 7, 8, 1121, 2021, 2048};           
      int[] arr2 = new int [] {101,102,103};
      for (int p=0; p<4; p++)
        {
          int [] qq = (p>2) ? arr2:arr1;
          Kiwi.Pause();
          Console.WriteLine("   wand {0}  {1}", p, qq, qq.Length);
          Kiwi.Pause();
        }
  }


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
	{
	   Console.WriteLine("Kiwi Demo - Test57 starting.");

	   Kiwi.Pause();	   test57_phase0();
           //	   Kiwi.Pause();	   test57_phase1();

	   
	   Console.WriteLine("Test57 done.");
	}
}

// eof


