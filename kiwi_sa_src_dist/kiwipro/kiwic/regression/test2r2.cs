// $Id: test2r2.cs,v 1.5 2011/09/28 10:49:33 djg11 Exp $
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
//
// Shared variable concurrency test.
// This version needs test and set or lock calls before it is stable enough to warrant inclusion in regression.
//
// NB: Single compilation unit for sender and receiver ... how to make separate?  Need to instantiate a shared component and make calls on it ...
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


//
//
//
class test_test2r2
{
    static int limit = 16;

    public static int shared_var;

    public static void process1()
    {
      for (int count=0; count < 100; count += 10)
        {
            Console.WriteLine("Process1 count={0}, shared_var={1}", count, shared_var);
	    shared_var = count + 1;
	    Kiwi.Pause(); 
        }
    }

    public static void process2()
    {
      for (int count=5; count < 100; count += 10)
        {
            Console.WriteLine("Process2 {0} ", count);
	    if (shared_var == 41) Console.WriteLine("Process2 spots Process1 {0} {1} ", count, shared_var);
	    shared_var = count + 1;
	    Kiwi.Pause(); 
        }
    }


    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test2r2: Shared-variable test: Start");
        Console.WriteLine("Note: this test, in its current form, is very fragile owing to races and has non-determinstic output - useless for regression!");
        Thread thread1 = new Thread(new ThreadStart(process1));
        thread1.Start();
	process2();
	Kiwi.Pause();
        Console.WriteLine("Test2r2: Shared-variable test: Finished");
    }
}

// eof