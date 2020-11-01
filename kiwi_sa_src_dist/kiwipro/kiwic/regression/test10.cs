// $Id: test10.cs,v 1.5 2011/06/24 14:55:58 djg11 Exp $
//
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
//
// Test 10 : method return values and structural arithmetic (with ALUs being used).
//

using System;
using System.Text;
using KiwiSystem;

/*
public class parallel_port
{
    [Kiwi.OutputWordPort(7, 0)] static byte dout;
    [Kiwi.OutputBitPort("")] static bool strobe;
    [Kiwi.InputBitPort("")] static bool ack;

    public static void putchar(byte c)
	{
	  while (ack == strobe) Kiwi.Pause();
	  dout = c;
	  Console.Write(" " + c);
	  Kiwi.Pause();
	  strobe = !strobe;
	}
}
*/

class test10
{
    static int tval = 789100;

    [Kiwi.OutputBitPort("mpxr")] static int mpxr;

    static Int64 returner(int k)
    {
       if (k < 10) return 100;
       if (k < 20) return 200;
       if (k < 30) return (43322123456789100 - tval)/10;
       return 100;
    }


    static void loptot(string s)
    { 
      for (int i = 0; i<s.Length; i++)
      {
        Console.WriteLine("{0} gives {1} and {2}", i, returner(i), returner(i*2));
        Kiwi.Pause();
      }
    }


    public static void Main()
    {
	int v = 65536;
        int qval = 789100;
	if (false)
 		Console.WriteLine("sp {0} {1} ", qval, (43322123456789100 - qval)/10);
	else
	{
	  Kiwi.KppMark(1, "Eden");
	  Console.WriteLine("m1 {0}", v);
	  Console.WriteLine("m2 {0}", -v);
	  Console.WriteLine("m3 {0}", v+1);
	  Console.WriteLine("m4 {0}", v-1);
	  Console.WriteLine("m5 {0}", -v+1);
	  Console.WriteLine("m6 {0}", v * 4);
	  Console.WriteLine("m7 {0}", v / 4);
	  Console.WriteLine("m8 {0}", v << 2);
	  Console.WriteLine("m9 {0}", v >> 2);
	  Console.WriteLine("m10 {0}", (1<<24)-v);
	  Kiwi.Pause();
	  loptot("Hello World\n");
	  Kiwi.KppMark(2, "MiddleTown");
	  for (int zz=1; zz<10;zz+=2)
	  {
	    v = 1 + zz;
	    Kiwi.Pause();
	    mpxr = v * v * v * v * v; // This should cause reuse of a structural multiplier instance several times using average recipe resource settings (multipliers per thread).      --max_no_int_muls=3
	    // Because mpxr is an output the multipliers will be instantiated.
	    // Restructure is happy to leave arithmetic that is only passed to a pli call as an RTL expression
	    Console.WriteLine("  {0} to power 5 is {1}", v, mpxr);
	 } 
	 Kiwi.Pause();
         Kiwi.KppMark(3, "Renaissance");
	 for (int zz=1; zz<10; zz+=2)
	  { //zz=1; v==2; sq=4; s=100000/(204)=490; mpxr=4*2+490=498.
	    v = 1 + zz;
	    Kiwi.Pause();
	    int sq = v*v;
	    int s = 100000/(sq+200);
	    mpxr = sq * v + s; // Here we test using the vsq twice, before and after a (vari-latency?) divider.
	    Console.WriteLine("  {0} processed is {1}", v, mpxr);
	 } 
	 Kiwi.Pause();

         Kiwi.KppMark(4, "Armageddon");
         Console.WriteLine("Frankly yes or no.");
     }	
    }
}

// eof
