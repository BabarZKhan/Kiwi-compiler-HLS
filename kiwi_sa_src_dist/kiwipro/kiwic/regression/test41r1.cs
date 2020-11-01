// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// Test41r1 : BRAM Complex Access Patterns
//

using System;
using System.Text;
using KiwiSystem;

class test41r1
{
    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
        int number = 1024;

        int [] testmem = new int [10000];

	System.Console.WriteLine("Start Kiwi test41r1");
	Kiwi.Pause();
        System.Console.WriteLine("The value of the integer: {0}", number);

       for (int vv=100; vv<180; vv++)
       {
          testmem[vv] = vv-98;
       }

       int ss = 0; 
     
       for (int vv=110; vv<120; vv++)
       {
          int pp = testmem[vv];
          int qq = testmem[vv+40];
          int dd = qq/pp;
          ss += dd;
	  Console.Write("  readback vv={0} pp={1} qq={2}", vv, pp, qq);
	  Console.WriteLine("    dd={0} ss={1}", dd, ss);
       }
 



      // Keep the console window open in debug mode under Windoze.
      System.Console.WriteLine("Press any key to exit.");
      // System.Console.ReadKey();

    }
}

// eof
