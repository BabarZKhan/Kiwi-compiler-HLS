// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// Test41r0 : BRAM Complex Access Patterns
//

using System;
using System.Text;
using KiwiSystem;

//     $display("pc=%1d: wrd0=%1d, ad0=%1d wen=%1d ren=%1d", xpc10nz, A_SINT_CC_SCALbx10_ARA0_WRD0,  A_SINT_CC_SCALbx10_ARA0_AD0, A_SINT_CC_SCALbx10_ARA0_WEN0, A_SINT_CC_SCALbx10_ARA0_REN0);

class test41r0
{
    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
        int number = 2048;

        int [] testmem = new int [10000];

	System.Console.WriteLine("Running test41r0");
	Kiwi.Pause();
        System.Console.WriteLine("The value of the integer: {0}", number);

       for (int vv=100; vv<130; vv++)
       {
          testmem[vv] = 1000+vv;
       }

       for (int vv=114; vv<117; vv++)
       {
          testmem[vv] = 2000+vv;
       }

       int ss = 0; 
       for (int vv=110; vv<120; vv++)
       {
          int rb = testmem[vv];
          ss += rb;
	  Console.WriteLine("  readback vv={0} rb={1} ss={2}", vv, rb, ss);
       }
 

       // Keep the console window open in debug mode under Windoze.
       System.Console.WriteLine("Press any key to exit.");
       // System.Console.ReadKey();

    }
}

// eof
