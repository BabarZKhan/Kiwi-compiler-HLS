// Kiwi Scientific Computing Regression Tests
// Test 36 : An array of structures - see how are they mapped to RAMs.

using System;
using System.Collections.Generic;
using System.Text;
using KiwiSystem;

class IntPair
{
    public bool c;
    public int x, y;
    public void DisplayX()
    {
        Console.WriteLine("  -- my stored value is: {0} {1}", x, y);
    }

    static int counter = 100;

    public IntPair() 
    { c = false; x = 1+counter; y = 2+counter; 
      Console.WriteLine(" .. bunny ctor call {0}", counter);
      counter ++;
    } // Constructor.

    public void toggle () { c = !c; }
}


class TestClass
{
    public void TC_Main()
    {
        IntPair [] ipairs = new IntPair [10];

        for (int pq = 0; pq < ipairs.Length; pq++) ipairs[pq] = new IntPair();

	// This example comes from DJG HLS lecture notes, where the best layout in RAM depends on the expected density of values in foo
	// but can the compiler tell that foo holds only 2 percent of the time ?
	
        for (int xcount = 0; xcount<10; xcount ++)
	{
	        Console.WriteLine("xcount start {0}", xcount);
		bool foo = (xcount < 2);
		int sum1 = 0, sum2 = 0;
		ipairs[xcount].x = 15;  ipairs[xcount].y = 16; ipairs[xcount].toggle();

		if (foo) 
			foreach (IntPair pp in ipairs)
			 {
			    sum1 += pp.x + pp.y;
	                    Console.WriteLine("  interim A : Xcount={0},  x={1} y={2}", xcount, pp.x, pp.y);
	                    Console.WriteLine("  interim A : Xcount={0},  sum1={1} sum2={2}", xcount, sum1, sum2);
			    Kiwi.Pause();
			 }   	   
		else
			foreach (IntPair pp in ipairs)
			 {	
			    sum2 += pp.c ? pp.y: pp.x;
	                    Console.WriteLine("  interim B : Xcount={0},  sum1={1} sum2={2}", xcount, sum1, sum2);
			    Kiwi.Pause();
			 }
                Console.WriteLine("  interim : Xcount={0},  sum1={1} sum2={2}", xcount, sum1, sum2);
        }
        ipairs[3].DisplayX();
        
    }
}

class test36
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test36 start.");
	TestClass tcc = new TestClass();
	tcc.TC_Main();
     }	

}

// eof
