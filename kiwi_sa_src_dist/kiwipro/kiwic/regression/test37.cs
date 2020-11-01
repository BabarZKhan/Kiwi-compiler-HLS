// Kiwi Scientific Computing Regression Tests

// Test 37 : polymorphic arrays

using System;
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

class test37
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	int [] freddy = new int[4];
        Console.WriteLine("Test37 start. {0}", freddy.ToString());
	TestClass<int> tcc = new TestClass<int>(freddy);
	tcc.TestWorker();
        Console.WriteLine("Test37 finished.");
     }	
}
// eof
