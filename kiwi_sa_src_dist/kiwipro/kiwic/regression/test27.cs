// $Id: test27.cs,v 1.1 2011/08/12 09:39:32 djg11 Exp $ 
//
// test27
// Tests run-time twiddle of object pointers including null, but without garbage collection.
//
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


class test27
{

  class dc
  {
    public int spare, left, right;
    public dc next;

    public dc()
    {
      next = this;
      spare = 987654321;
    }
  };
  

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      Console.WriteLine("Test 27 start.");
      Kiwi.Pause();
      dc ha = new dc();
      dc hb = new dc();
      dc hc = null;
      
	ha.left = 907;
	ha.right = 2223;

	hb.left = 32;
	hb.right = 4433;

	// Do some pre-first-pause printing:
        Console.WriteLine("  Pre-test ha.left {0}", ha.left);
        Console.WriteLine("  Pre-test hb.left {0}", hb.left);

        // All pointer manipulations are decidable (no array subscript arithmetic).

        Console.WriteLine("First printed value should be 32 {0}", hb.left);
	for (int kq=0; kq<8; kq++)
	  {
	    Kiwi.Pause();
	    //	    Kiwi.NoUnroll();
	    dc ht = hb; hb = hc; hc = ha; ha = ht;
	    Kiwi.Pause();
            if (ha != null)  Console.WriteLine("  Test27 (north variant) ha.left={0}   kq={1}", ha.left, kq);
            else  Console.WriteLine("  Test27 (north variant) ha currently null.");
	  }
//        Console.WriteLine("End of Test {0}", my_ds.structure_betty(10));
        Kiwi.Pause();
        Console.WriteLine("Test 27 finished.");
    }
}

// eof


//

