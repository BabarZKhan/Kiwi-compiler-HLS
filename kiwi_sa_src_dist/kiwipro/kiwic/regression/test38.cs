// Kiwi Scientific Computing Regression Tests
// Test 38r0 : ultra-simple array of objects test.

using System;
using System.Collections.Generic;
using System.Text;
using KiwiSystem;


class TestClass <XT> 
{

    XT [] myarray;
    public void TestWorker()
    {
        XT [] polyarray = myarray;
	string ss = polyarray[1].ToString();
	Console.WriteLine("the toString result is {0}", ss);
    }

   public TestClass (XT [] arg) { myarray = arg; } 
}

class test38
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	int [] freddy = new int[4];
        Console.WriteLine("Test38 start. {0}", freddy.ToString());
	TestClass<int> tcc = new TestClass<int>(freddy);
	tcc.TestWorker();
        Console.WriteLine("Test38 finished.");
     }	
}
// eof
