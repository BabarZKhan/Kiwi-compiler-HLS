// Kiwi Scientific Computing Regression Tests

// (C) 2014-15 DJ Greaves.
// University of Cambridge Computer Laboratory.
//
// Test 38r0 : dynamically-indexed array of objects containing scalars.
// Test 38r1 : statically-indexed array of objects containing scalars.
// Test 38r2 : array of an object that contains one array. 
// Test 38r3 : imutable array of objects containing an array.
// Test 38r4 : mutable array of objects containing an array.


using System;
using System.Collections.Generic;
using System.Text;
using KiwiSystem;


class TestClass 
{
  public int [] lebon;
  public int duranfield;

  public TestClass(int v)
  {
    lebon = new int[2];  
    lebon[0] = 33000 + v;
    lebon[1] = 44000 + v;
    duranfield = 900+v;
  }
}


class test38r4
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	TestClass [] simon = new TestClass[3];
        Console.WriteLine("Test38r4 start.");

	simon[0] = new TestClass(0);
	simon[1] = new TestClass(1);
	simon[2] = new TestClass(2);
	Kiwi.Pause();
	Console.WriteLine(" pre lebon={0}, ben={1}", simon[0].lebon[0], simon[0].lebon[1]);
	for (int i=0; i<4; i++)
	{
		Kiwi.Pause();
		if (i == 2) simon[0].lebon = simon[1].lebon;  // Mutate the array handle in this test at this point.
		if (i == 2) simon[1].lebon = simon[2].lebon;  // and another
		Kiwi.Pause();
		simon[0].lebon[0] = simon[0].lebon[0] + 10;
		simon[0].lebon[1] = simon[0].lebon[0] + 100;
		// 		foreach (int j in 0 .. simon.Length) 		
		for (int j = 0; j < simon.Length; j++)
		  {
		    Console.WriteLine(" post {0} : {1} and {2}", j, simon[j].lebon[0], simon[j].lebon[1]);		    
		    Console.WriteLine(" post duran={0}", simon[j].duranfield);
		  }
        }
        Console.WriteLine("Test38r4 finished.");
     }	
}
// eof
