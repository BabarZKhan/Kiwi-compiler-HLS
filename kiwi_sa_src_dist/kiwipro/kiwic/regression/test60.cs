// Kiwi Scientific Acceleration
// (C) 2016 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test60 : Passing an array ... pretty trivial ... same array passed each time!
//

using System;
using System.Text;
using KiwiSystem;


public class Test60
{

  static void mutator(int [] myar)
  {
    myar[2] = 101;
  }

  static int [] bish = new int [] {  22, 43, 44, 55 };


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test60 start.");
    Kiwi.Pause();

    for (int xx=0; xx<2; xx++)
      {
	Console.WriteLine("   {0}  Bish [2]={1}   Bish[3]={2}", xx, bish[2], bish[3]);
	mutator(bish);
      }

    Console.WriteLine("Test60 finish.");
  }
  
}



// eof
