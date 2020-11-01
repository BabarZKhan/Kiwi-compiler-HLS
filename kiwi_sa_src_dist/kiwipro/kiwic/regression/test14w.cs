// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// (C) 2009 DJ Greaves, University of Cambridge, Computer Laboratory.
//
// test14w - as per test14x but mutable data so ROM support is not invoked.
//
// Tests run-time twiddle of object pointers where the objects contain arrays, but without garbage collection.
// No nulls used.

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


class test14w
{

  class dc_cls
	{
	  public int not_used, left, right;
	  public dc_cls next;
	  public int [] arrow = new int [191];
	};
  

  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	dc_cls ha; ha = new dc_cls();
	dc_cls hb; hb = new dc_cls();

	ha.left = 22;
	ha.right = 23;

	hb.left = 32;
	hb.right = 33;

        ha.arrow[1] = 1001;
        ha.arrow[15] = 1002;
        hb.arrow[1] = 1003;
        hb.arrow[15] = 1004;

	// Do some pre-first-pause printing:
        Console.WriteLine("  Pre-test ha.left={0}, harrow={1}", ha.left, ha.arrow[1]);

        Console.WriteLine("  Pre-test hb.left={0}, harrow={1}", hb.left, hb.arrow[1]);

        // All pointer manipulations are decidable (no array subscript arithmetic).

        Console.WriteLine("First printed value should be 32 {0}", hb.left);
	for (int kl=0; kl<4; kl++)
	  {
	    Kiwi.Pause();
	    dc_cls ht = hb; hb = ha; ha = ht;
	    //	    Console.WriteLine("  North test14w : left={0} arrow[1]={1}  arrow[15]={2}", ha.left, ha.arrow[1], ha.arrow[15]);

	    //Console.WriteLine("  North test14w line0 : pointer={0}", ha.ToString());
	    //Console.WriteLine("It {0} 14w", kl);
	    Console.WriteLine("  North test14w line1 : left={0}", ha.left);
	    Console.WriteLine("  North test14w line2 : arrow[1]={0}", ha.arrow[1]);
	    Console.WriteLine("  North test14w line3 : arrow[15]={0}", ha.arrow[15]);

            hb.arrow[15] = 100000 + hb.arrow[15]; // This write stops the data being a ROM.
	  }
        Console.WriteLine("End of test14w {0}", 0);
    }
}

// eof

