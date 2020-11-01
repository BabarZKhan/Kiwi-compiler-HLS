// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test70: exception raising - but not handling.
// 

using System;
using System.Text;
using KiwiSystem;


class myDemoExn: System.Exception
{
  // Note KiwiC latches onto an integer field name containing the string 'code'.
  int ecode = 129;  

  public int error_code
  {
    set { ecode = value; }
  }
}


class UncaughtExceptionTest
{
  // Steer away from Kiwi-1 dynamic storage complexity by
  // making the thrown exception a static.
  static myDemoExn my_faulter = new myDemoExn();

  public void runner(int roger)
  {
    for (int pp=0; pp<10;pp++)
      {
        Kiwi.Pause();
        Console.WriteLine(" runner {0}", pp);
        my_faulter.error_code = 101 + pp;
        if (pp == 5) throw my_faulter;
      }
  }
}



class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test70  starting.");
    UncaughtExceptionTest pog = new UncaughtExceptionTest();
    for (int px=3; px>=0; px--)
      {
	Kiwi.Pause();
	pog.runner(px);
      }
  
    Console.WriteLine("Test70 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof