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


class TestClass_38r2 
{
  public int [] lebon;

  public TestClass_38r2()
  {
    lebon = new int[2];  
  }
}



class test38r2
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test38r2 start.");
	TestClass_38r2 duran = new TestClass_38r2();
	duran.lebon[0] = 33000;
	duran.lebon[1] = 44000;
	Kiwi.Pause();
	Console.WriteLine(" pre lebon={0}, ben={1}", duran.lebon[0], duran.lebon[1]);
	for (int i=0; i<2; i++)
	{
		Kiwi.Pause();
		duran.lebon[0] = duran.lebon[0] + 1;
		duran.lebon[1] = duran.lebon[0] + 100;
		Console.WriteLine(" post lebon={0}, ben={1}", duran.lebon[0], duran.lebon[1]);
        }
        Console.WriteLine("Test38r2 finished.");
     }	
}
// eof
