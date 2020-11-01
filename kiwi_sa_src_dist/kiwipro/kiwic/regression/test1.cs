// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// $Id: test1.cs,v 1.5 2011/06/13 12:58:42 djg11 Exp $


// Simple odd number printing with pauses.
// Correct output does NOT print 11
// Test 1 Limit=10
// 1 3 5 7 9  Test 1 finished.

using System;
using System.Text;
using KiwiSystem;

class test1
{
    static int toplimit = 10;

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        int jojo;
 	Console.WriteLine("Test 1 Limit=" + toplimit);
        Kiwi.Pause(); // If you miss out this pause, the order of writing to the console is or was non-deterministic (back in 2007). 
   	for (jojo=1;jojo<=toplimit;jojo+=2) 
	{
	  Console.Write(jojo  + " "); // String concat test.
	  Kiwi.Pause();
        }  
     	Console.WriteLine(" Test 1 finished.");
	Kiwi.Pause();
    }
}

// eof



