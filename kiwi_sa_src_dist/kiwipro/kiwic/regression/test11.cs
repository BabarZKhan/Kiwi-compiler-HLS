// $Id: test11.cs,v 1.5 2011/12/19 20:35:28 djg11 Exp $

// Test 11 : Conditional expressions in return statements. Incorporates a test of having more than one call site for a method will local or spill variables.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


class test11
{
  
       static int max2(int a, int b)
       {
           return (a > b) ? a : b;
       }

       static int max3(int a, int b, int c)
       {
           return (a >= max2(b, c)) ? a : max2(b, c);
       }

    public static void Main()
    {
	
        Console.WriteLine("  Ans aa {0}", max3(10, 1, 3));
        Console.WriteLine("  Ans bb {0}", max3(1, 10, 3));
        Console.WriteLine("  Ans cc {0}", max3(1, 2, 10));
    }
}

// eof
