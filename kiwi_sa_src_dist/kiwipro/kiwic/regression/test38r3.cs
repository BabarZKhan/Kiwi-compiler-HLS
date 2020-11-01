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

/*
A suitable, optimised hardware design to create from this program would spot that the lebon array handles are never
modified and dispense with these in the generated design.  There are three small arrays called lebon and these
are used only with constant indecies at certain locations.  For such a small and sparse array, implementation as discrete
registers is preferred. Nonetheless, the repack stage will maintain them as three tiny arrays with a multiplexor on their
read data ports and a demultiplexor on their write enable control signals.  The restructure recipe stage will make the
final conversion to a degenerate register file. The conerefine stage or else the FPGA tools will discard the unused registers 
in the file if there are any.

Test38r4 mutates the array handles. See description in that file.
*/
class test38r3
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	TestClass [] simon = new TestClass[3];
        Console.WriteLine("Test38r3 start.");

	simon[0] = new TestClass(0);
	simon[1] = new TestClass(1);
	simon[2] = new TestClass(2);
	Kiwi.Pause();
	Console.WriteLine(" half lebon0_0={0}", simon[0].lebon[0]);
	Console.WriteLine(" half lebon0_1={0}", simon[0].lebon[1]);
	Console.WriteLine(" half lebon1_1={0}", simon[1].lebon[1]);
	Console.WriteLine(" pre lebon0_0={0}, lebon0_1={1}", simon[0].lebon[0], simon[0].lebon[1]);
	for (int i=0; i<3; i++)
	{
		Kiwi.Pause();
		simon[0].lebon[0] = simon[0].lebon[0] + 10;
		simon[0].lebon[1] = simon[0].lebon[0] + 100;
		// 		foreach (int j in 0 .. simon.Length) 		
		for (int j = 0; j < simon.Length; j++)
		  {
		    Console.WriteLine(" post simon[{0}] : lebon[0]={1} and lebon[1]={2}", j, simon[j].lebon[0], simon[j].lebon[1]);		    
		    Console.WriteLine(" post duran={0}", simon[j].duranfield);
		  }
        }
        Console.WriteLine("Test38r3 finished.");
     }	
}
// eof
