// 
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// test14x
// Tests run-time twiddle of object pointers where the objects contain arrays, but without garbage collection.
//
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


class test14x
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
//	ha.next = hb;

	hb.left = 32;
	hb.right = 33;
//	hb.next = ha;

        ha.arrow[1] = 1001;
        ha.arrow[5] = 1002;
        hb.arrow[1] = 1003;
        hb.arrow[5] = 1004;

	// Do some pre-first-pause printing:
        Console.WriteLine("  Pre-test ha.left={0}, harrow={1}", ha.left, ha.arrow[1]);

	//  Console.WriteLine("  Stop now for debug\n"); 
	//  return;

        Console.WriteLine("  Pre-test hb.left={0}, harrow={1}", hb.left, hb.arrow[1]);

        // All pointer manipulations are decidable (no array subscript arithmetic).

        Console.WriteLine("First printed value should be 32 {0}", hb.left);
	for (int k=0; k<4; k++)
	  {
	    Kiwi.Pause();
	    //	    Kiwi.NoUnroll();
	    dc_cls ht = hb; hb = ha; ha = ht;
	    //	    Console.WriteLine("  North test14x : left={0} arrow[1]={1}  arrow[5]={2}", ha.left, ha.arrow[1], ha.arrow[5]);

	    Console.WriteLine("  North test14x line1 : left={0}", ha.left);
	    Console.WriteLine("  North test14x line2 : arrow[1]={0}", ha.arrow[1]);
	    Console.WriteLine("  North test14x line3 : arrow[5]={0}", ha.arrow[5]);

	  }
//        Console.WriteLine("End of test14x {0}", my_ds.structure_betty(10));
    }
}

// eof

