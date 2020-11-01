// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test69: parent constructor calls
// 

using System;
using System.Text;
using KiwiSystem;


class Upper
{
  public static int upvar = 0;

  public Upper () // Constructor
  {
    upvar ++;
    Console.WriteLine("Upper ctor called. Count={0}", upvar);

  }
}

class Middle:Upper
{
  public static int midvar = 0;

  public Middle () // Constructor
  {
    midvar ++;
    Console.WriteLine("Middle ctor called. Count={0}", midvar);

  }
}


class Lower: Middle
{
  public Lower(string hell)
  {
    Console.WriteLine("Lower ctor called. midcount={0}", Middle.midvar);
  }

  public void runner(int roger)
  {
    
  }
}

class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test69  starting.");
    Lower pog = new Lower("hello lower");
    for (int px=3; px>=0; px--)
      {
	Kiwi.Pause();
	pog.runner(px);
      }
  
    Console.WriteLine("Test69 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof