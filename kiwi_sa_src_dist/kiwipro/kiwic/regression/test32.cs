// Test32: Variable-latency divide test.

// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// (C) 2012 DJ Greaves, University of Cambridge, Computer Laboratory.
// $Id: test32.cs,v 1.1 2012/11/09 09:51:07 djg11 Exp $




// Also, how many divider instances do we expect - can we sweep a command line arg for time/space tradeoff.
// -max_no_int_divs=2 


// What about divide-by-zero abend syndrome checkng ? TODO.

using System;
using System.Text;
using KiwiSystem;

class test32
{
  static int limit = 1000;
  static     volatile int nn = 32767;
  static     volatile int joj;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test32 Limit=" + limit);

    Kiwi.Pause(); // If you miss out this pause, the order of writing to the console is non-deterministic.  Why ? Varilength compile times?

    // We want the same behaviour as dotnet or C# for integer division with one argument negative.
    // Dotnet and C# round towards zero, like C, whereas Python and Rubywould give -6. 
    Console.WriteLine("Negative division results: 21/-4={0}  -21/4={1}", 21/-4, -21/4);
    //Negative division results: 21/-4=-5  -21/4=-5
    nn = 32768; // Must change this (or make volatile) to avoid divisions at compile time in second loop.
    Kiwi.Pause(); 
    for (int jojo=1;jojo<=limit;jojo *=3)  // try to use five dividers in one clock cycle in one thread.
      {
	joj = jojo; // Make volatile this way!
	Console.WriteLine(jojo + " +0 gives -> " + (nn+0)/joj);
	Console.WriteLine(jojo + " +1 gives -> " + (nn+1)/joj);
	Console.WriteLine(jojo + " +2 gives -> " + (nn+2)/joj);
	Console.WriteLine(jojo + " +3 gives -> " + (nn+3)/joj);
	Console.WriteLine(jojo + " +4 gives -> " + (nn+4)/joj);
	Kiwi.Pause();
      }  
    Console.WriteLine("Test32 middle.");
    Kiwi.Pause();
    for (int jojo=1;jojo<=limit;jojo *=3) 
      {
	joj = jojo;
	Console.WriteLine(jojo + " again -> " + nn/joj);
      }  
    Console.WriteLine("Test32 finished.");
    Kiwi.Pause();
  }
}

// eof



