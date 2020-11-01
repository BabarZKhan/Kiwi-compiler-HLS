// $Id: test6.cs,v 1.6 2010/11/29 11:03:38 djg11 Exp $
// Test 6: the timestable without pauses: generates a combinational circuit
// and some (unrelated) PLI output.
//
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;

class test6
{
    static int limit = 10;
    [Kiwi.OutputBitPort("")] static bool dout;
    [Kiwi.InputBitPort("")] static bool din;

    public static void Main()
    {
        int i, j;
 	Console.WriteLine("Times Table Up To " + limit);
	for (i=1;i<=limit;i++)
	{
   	  for (j=1;j<=limit;j++)
		{
		   Console.Write(i*j + " ");
		   dout = !din;
		   Kiwi.Pause(); 
		}
     	  Console.WriteLine(" EOL {0}", i);
	}
	Kiwi.Pause(); // Pause before exit to make sure output is flushed
    }
}

// eof
