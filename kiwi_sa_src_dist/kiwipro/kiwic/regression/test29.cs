// Kiwi Scientific Computing Regression Tests
// $Id: test29.cs,v 1.2 2011/11/15 16:19:52 djg11 Exp $
// Some 2-D jagged array testing with base pointer changes.


using System;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test29
{
  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	int [] a1 = new int [] {  12, 13, 14 };
	int [] a2 = new int [] {  2, 3, 4 };
	int [] a3 = new int [] {  312, 313, 314 };
	int [] a4 = new int [] {  412, 413, 414 };

	int [] [] boz = new int [4] [] ;
	boz[0] = a1; // Manual create a jagged 2-D array instead of using multidimensional array (that also exists in C# and is supported by Kiwi).
	boz[1] = a2;
	boz[2] = a3;
	boz[3] = a4;
	Kiwi.Pause();
	int s = 1;
	Kiwi.Pause();
	for (int i=0; i<4; i++)
	  { s += boz[i][1];
	    Console.WriteLine(" Test29 i={0}  s={1}.", i, s);
	    Kiwi.Pause();	      
            int [] baser = boz[2];
            boz[2] = boz[1]; // Exchange pointers 1 and 2
            boz[1] = baser;
	    for(int ii=0; ii<3;ii++)Console.WriteLine(" Test29 ii={0}  baser={1}.", ii, baser[ii]);
	  }
    }
}


// eof



