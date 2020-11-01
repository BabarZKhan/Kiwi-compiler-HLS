// $Id: test14.cs,v 1.7 2011/04/01 07:23:21 djg11 Exp $ kiwic test (North Cottage)
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// test14 - aka North Test - Tests run-time twiddle of object pointers, but without new or garbage collection inside the run-time loop.  Ultra simple really.

//
// Because the contents are scalars we expect Kiwi to use field arrays, but with only two they are likely to all be converted to flip-flops or a register file, depending on technology thresholds.
//
// A related test that uses the null pointer is ... ?
//

using System;
using System.Text;
using KiwiSystem;


class test14
{
  class dc
	{
	   public int not_used, left, right;
	   public dc next;
	};

    public static void Main()
    {
	dc ha; ha = new dc();
	dc hb; hb = new dc();

	ha.left = 22;
	ha.right = 23;
//	ha.next = hb;

	hb.left = 32;
	hb.right = 33;
//	hb.next = ha;

	// Do some pre-first-pause printing:
        Console.WriteLine("  Pre-test ha.left {0}", ha.left);
        Console.WriteLine("  Pre-test hb.left {0}", hb.left);

        // All pointer manipulations are decidable (no array subscript arithmetic).
        Console.WriteLine("First printed value should be 32 {0}", hb.left);
	for (int k=0; k<4; k++)
	  {
	    Kiwi.Pause();
	    dc ht = hb; hb = ha; ha = ht;
	    Console.WriteLine("  North test {0}", ha.left);
	  }
//        Console.WriteLine("End of Test {0}", my_ds.structure_betty(10));
    }
}

// eof


//

