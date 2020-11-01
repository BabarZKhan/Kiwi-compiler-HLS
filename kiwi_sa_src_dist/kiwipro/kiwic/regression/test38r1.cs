// Kiwi Scientific Computing Regression Tests


// (C) 2014-15 DJ Greaves.
// University of Cambridge Computer Laboratory.
//
// Test 38r0 : dynamically-indexed array of objects containing scalars.
// Test 38r1 : statically-indexed array of objects containing scalars.
// Test 38r2 : array of an object that contains one array. 
// Test 38r3 : array of objects containing an array.

using System;
using System.Collections.Generic;
using System.Text;
using KiwiSystem;


class TestClass 
{
	public int bill, ben;
}

class test38r1
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test38r1 start.");
	TestClass duran = new TestClass();

	duran.bill = 33000;
	duran.ben = 44000;
	Kiwi.Pause();
	Console.WriteLine(" pre bill={0}, ben={1}", duran.bill, duran.ben);
	for (int i=0; i<2; i++)
	{
		Kiwi.Pause();
		duran.bill = duran.bill + 1;
		duran.ben = duran.bill + 100;
		Console.WriteLine(" post bill={0}, ben={1}", duran.bill, duran.ben);
        }
        Console.WriteLine("Test38r1 finished.");
     }	
}
// eof
