// $Id: test8.cs,v 1.4 2011/01/12 15:55:02 djg11 Exp $
// Test 8
// Kiwi Scientific Acceleration
//
// Call by reference test for some static variables

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;

class test8
{

    // yetti is passed by reference, hence india should skip vals in the outer for loop.

    public static void edict(ref int moscow, out int zulu)
    {
	Console.Write(moscow + " ");
	moscow += 2;
	zulu = moscow + 2;
    }


    // TODO: KiwiC's current implementation of boxing cannot cope with india being a static
    static int india = 4;
    static int france = 6;
    public static void Main()
    {

        india = 32;
 	Console.WriteLine("Test Call By Reference\n"); // Two newlines in total.
	for (india=1;india<=20;india++) 
	{
	   int yetti;
	   edict(ref india, out yetti);
	   Kiwi.Pause();
	   Kiwi.NoUnroll();
	}
    }
}

// eof
