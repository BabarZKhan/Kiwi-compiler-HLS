// $Id: test27x.cs,v 1.1 2011/08/12 09:39:32 djg11 Exp $ 
//
// test27x
// Tests run-time twiddle of object pointers including null with null pointer abend.
//
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


class test27x
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
      Console.WriteLine("Test 27x start.");
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
        Console.WriteLine("First printed value should be 32 {0}", hb.left);
	for (int kq=0; kq<3; kq++)
	  {
	    Kiwi.Pause();
	    dc ht = hb; hb = hc; hc = ha; ha = ht;
	    Kiwi.Pause();
            Console.WriteLine("  Test27 (north variant) ha.left={0}   kq={1}", ha.left, kq);
	  }
// Mono should bomb out:
//    System.NullReferenceException: Object reference not set to an instance of an object
// 
        Kiwi.Pause();
        Console.WriteLine("Test 27x finished.");
    }
}
//
// eof
//

