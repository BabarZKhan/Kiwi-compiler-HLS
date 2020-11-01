// Kiwi Scientific Computing Regression Tests

// (C) 2014-15 DJ Greaves.
// University of Cambridge Computer Laboratory.
//
// Test 38r0 : dynamically-indexed array of objects containing scalars.
// Test 38r1 : statically-indexed array of objects containing scalars.
// Test 38r2 : array of an object that contains one array. 
// Test 38r3 : array of objects containing an array.
//
using System;
using System.Collections.Generic;
using System.Text;
using KiwiSystem;


class TestClass 
{
	public int bill, ben;
}

class test38r0
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	TestClass [] freddy = new TestClass[3];
        Console.WriteLine("Test38r0 start.");
	freddy[0] = new TestClass();
	freddy[1] = new TestClass();
	freddy[2] = new TestClass();
	Kiwi.Pause();
	freddy[2] = freddy[1];
	Kiwi.Pause();
	freddy[2].bill = 12345678;
        Console.WriteLine("Test38r0 finished.");
     }	
}
// eof
