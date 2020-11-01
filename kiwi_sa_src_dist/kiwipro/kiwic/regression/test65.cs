// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// test65.cs  - little ROMs mapped to Portless Register File 
//
// 

using System;
using System.Text;
using KiwiSystem;



class ConstRoms1
{


  // These are short little ROMs.
  static readonly byte[] Box1 = new byte[] { 22, 33, 44, 55, 66, 77 };

  static readonly short[] Box2 = new short[] { 22, 33, 44, 55, 66, 77 };
  
  public static void RunVarIdx()
  {
    for (int i=0;i<Box1.Length; i++)
      {
        Kiwi.NoUnroll();
        Console.WriteLine("  {0} vardix {1}", i, Box1[i]);
      }
  }

  public static void RunConstIdx()
  {
    for (int p=0; p<3;p++)
      {
        Kiwi.NoUnroll();
        Console.WriteLine("  {0} constidx {1}", p, Box2[3]+p);
      }
  }
}

class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false; // The old means of exit.

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test65 starting.");
    Kiwi.Pause();
    //ConstRoms1 app = new ConstRoms1();
    Kiwi.KppMark(2, "START-VARIDX");
    ConstRoms1.RunVarIdx();
    Kiwi.Pause();
    Kiwi.KppMark(2, "START-CONSTIDX");
    ConstRoms1.RunConstIdx();
    Console.WriteLine("Test65 finished.");
    done = true;
    Kiwi.KppMark(2, "FINISH-VARIDX");
    Kiwi.Pause();
  }
}

// eof