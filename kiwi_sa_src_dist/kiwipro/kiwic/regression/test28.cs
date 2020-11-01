// $Id: test28.cs,v 1.3 2012/07/05 14:37:24 djg11 Exp $
// Kiwi Scientific Acceleration Regression Suite
// Some further 2-D array testing. Jaggeds.

// Needs a store1: 0 ~ -2 test28/T400/Main/T400/Main/V_5 lnc=h22:c23 rhs_aid=test28/T400/Main/T400/Main/V_4

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test28
{
  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	int [] a1 = new int [] {  12, 13, 14 };
	int [] a2 = new int [] {  212, 213, 214, 1000, 1001, 1002 };
	int [] a3 = new int [] {  312, 313, 314 };
	int [] a4 = new int [] {  412, 413, 414 };

	int [] [] boz = new int [5] [] ;
	boz[0] = a1; // Manually create a 2-D jagged array.
	boz[1] = a2;
	boz[2] = a3;
	boz[3] = a4;
	boz[4] = a2; // Repeat of boz[2].
	Kiwi.Pause();

	int ss = 0;
	for (int i=0; i<boz.Length; i++)
	  { ss = 200000 + boz[i][1];
	    Console.WriteLine(" Test28 p1 i={0}  s={1}.", i, ss);
	    Kiwi.Pause();	      
	  }

	ss = 0;
	for (int i=0; i<boz.Length; i++)
	  { ss += boz[i][1];
	    Console.WriteLine(" Test28 p2 i={0}  s={1}.", i, ss);
	    Kiwi.Pause();	      
	  }
    }
}
// eof



