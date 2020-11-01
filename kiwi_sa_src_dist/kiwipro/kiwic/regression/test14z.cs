// Kiwic Regression Tests.
//
// test14z
//
// Tests run-time twiddle of object pointers where the objects contain two different classes of arrays and we have an array of such objects.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


class test14z
{

  class dc_cls
	{
	  public int not_used, left, right;
	  public dc_cls next;
	  public int [] pingar = new int [191];
	  public int [] pongar = new int [10];
	  public dc_cls(int arg)
          {

               left = arg+1;  right = arg+2;
	       pingar[1] = arg + 3;
               pingar[5] = arg + 4;
               pongar[7] = arg + 5;
          }
	};
  

  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	dc_cls [] dcars = new dc_cls[100];
        dcars[0] = new dc_cls(30);
	dcars[1] = new dc_cls(40);
        dcars[2] = new dc_cls(50);



	// Do some pre-first-pause printing:
        Console.WriteLine("  Pre-test ha.left={0}, ping-five={1}", dcars[1].left, dcars[1].pingar[5]);
        Console.WriteLine("  Pre-test ha.left={0}, pong-seven={1}", dcars[1].left, dcars[1].pongar[7]);

	//  Console.WriteLine("  Stop now for debug\n"); 
	//  return;

        Console.WriteLine("  Pre-test hb.left={0}, pingar-one={1}", dcars[2].left, dcars[2].pingar[1]);

        // All pointer manipulations are decidable (no array subscript arithmetic).

        Console.WriteLine("First printed value should be 32 {0}", dcars[0].left);
	for (int k=0; k<4; k++)
	  {
	    Kiwi.Pause();
	    dc_cls ht = dcars[0]; dcars[0] = dcars[1]; dcars[1] = ht;
	    //	    Console.WriteLine("  North test14z : left={0} arrow[1]={1}  arrow[5]={2}", ha.left, ha.arrow[1], ha.arrow[5]);

	    Console.WriteLine("  North test14z line1 : left={0}", dcars[0].left);
	    Console.WriteLine("  North test14z line2 : pingar[1]={0}", dcars[0].pingar[1]);
	    Console.WriteLine("  North test14z line3 : pongar[5]={0}", dcars[0].pongar[5]);
            Console.WriteLine("  North         line4 : pongar[7]={0}", dcars[0].pongar[7]);	
	    Kiwi.Pause();
            dcars[0].pongar[7] = 100001 + dcars[0].pongar[7]; // This write stops the data being a ROM.
            dcars[0].pingar[5] = 100000 + dcars[0].pingar[5]; // This write stops the data being a ROM.

	  }
       Console.WriteLine("End of test14z {0}", 1);
    }
}

// eof

