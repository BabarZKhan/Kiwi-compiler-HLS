// Kiwi Scientific Acceleration
// Test 5
// Misc small tests: subroutine call testing, HwWidth  and int64 as int32: please add further forms.
//
//
// $Id: test5.cs,v 1.5 2012/05/03 23:06:10 djg11 Exp $
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;

class test5
{
    static int limit = 8;
    public static string edict(bool nlf, int k, int mm)
    {
	Console.Write(k + " ");
	if (nlf) Console.WriteLine(" BAR");
	return "+";
    }

    [Kiwi.HwWidth(6)] static int q6;  // This does not really see if hardware width attributes work...

    public static void pretest()
    {
	  Console.WriteLine("Test Five: Pretest");
	  for (q6=0; q6<10; q6++)   
	    {
	      int r = q6*1000;
	      Kiwi.Pause();	    
	      Console.WriteLine("T {0} {1}", q6, r);
	      Kiwi.Pause();	    
	    }
	}


/* RTL simulation output:
Test Five: mpx ovf
T   0 pp=             1000000 masked=    1000000
T   1 pp=          1000000000 masked= 1000000000
T   2 pp=       1000000000000 masked= -727379968
T   3 pp=    1000000000000000 masked=-1530494976
T   4 pp= 1000000000000000000 masked=-1486618624
T   5 pp= 3875820019684212736 masked= -559939584
T   6 pp= 2003764205206896640 masked=-1593835520
T   7 pp=-6930898827444486144 masked= -402653184
T   8 pp= 5076944270305263616 masked= 1073741824
T   9 pp= 4089650035136921600 masked=          0
*/

    public static void mpx_ovf()
    {
       Console.WriteLine("Test Five: mpx ovf 32s");
       if (true)
       {
     	  int pps = 252;
	  for (int q=0; q<10; q++)   
	    {
	      Console.WriteLine("TP32s {0} pps={1}  {1:X}", q, pps);
	      pps = pps * 16;
	      Kiwi.Pause();	    
	    }
       }	    
       Console.WriteLine("Test Five: mpx ovf 32u");
       if (true)
       {
     	  uint ppu = 252u;
	  for (int q=0; q<10; q++)   
	    {
	      Console.WriteLine("TP32u {0} ppu={1}  {1:X}", q, ppu);
	      ppu = ppu * 16u;
	      Kiwi.Pause();	    
	    }
       }
       Console.WriteLine("Test Five: mpx ovf 64s");
       if (true)
       {
     	  Int64 pp64s = 252L;  // 64-bit signed overflow test.
	  for (int q=0; q<20; q++)   
	    {
	      Console.WriteLine("TP64s {0} pp64s={1}  {1:X}", q, pp64s);
	      pp64s = pp64s * 16L;
	      Kiwi.Pause();	    
	    }
       }

       Console.WriteLine("Test Five: mpx ovf 64u");
       if (true)
       {
     	  UInt64 pp64u = 252L;
	  for (int q=0; q<20; q++)   
	    {
	      Console.WriteLine("TP64u {0} pp64u={1}  {1:X}", q, pp64u);
	      pp64u = pp64u * 16L;
	      Kiwi.Pause();	    
	    }
       }
   }

    public static void trims()
    {
       Console.WriteLine("Test Five: trims");
       Int64 pp = 1000L;
       for (int q=0; q<10; q++)   
	    {
	      pp = pp * 1000L;
	      int masked = (int) pp;
	      Kiwi.Pause();	    
	      Console.WriteLine("T {0} pp={1} masked={2}", q, pp, masked);
	      Kiwi.Pause();	    
	    }
    }

    static ulong dstt_mac = ((ulong)0x00cafe0000);
    public static void ucasts()
    {
       Console.WriteLine("Test Five: ucasts");
       Kiwi.Pause();
       // oh dear - this was being initialised to  64'h_ffff_ffff_cafe_0000
       // bad output:               is FFFFFFFFCAFE0000 18446744072820228096
       // bad output:< dstt_mac pos is FFFFFFFFCAFE03E8 18446744072820229096

       Console.WriteLine("dstt_mac pre is {0:X} {0}", dstt_mac);
       Kiwi.Pause();
       dstt_mac += 1000;
       Console.WriteLine("dstt_mac pos is {0:X} {0}", dstt_mac);
    }
 
    public static void calls()
    {
      Console.WriteLine("Test Five: calls");
        int i, j;
 	Console.WriteLine("Test Five: Up To " + limit);
	for (i=1;i<=limit;i++)
	{
	  for (j=1;j<=limit;j++)
	   	  edict(j==limit-1, i*j, 32);
	  Kiwi.Pause();
	}
	Kiwi.Pause();
	Console.WriteLine();
    }


    public static void Main()
    {
      pretest();
      calls();
      ucasts();
      mpx_ovf();
      trims();
    }
}
// eof
