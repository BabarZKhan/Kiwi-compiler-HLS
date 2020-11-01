// $Id: test101.cs,v 1.5 2011/09/28 10:49:33 djg11 Exp $

// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
//  Simple thread-shared data.  cf test2r2 shared variables.
//
// 
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


class Capsule // Capsules are passed over the channel.
{
  [Kiwi.Elaborate()] public bool newlinef;
  [Kiwi.Elaborate()] public int foo;

  public Capsule(int myval) 
	{ foo = myval;
	  newlinef = false; 
        }
}


class ConsumerClass
{

    int shared_var = 100;

    public void process_a()
    {
      for (int count=0; count < 10; count ++)
        {
            Console.WriteLine("proc_a {0} sv={1}", count, shared_var);
	    Kiwi.Pause(); 
        }
    }
}

//
//
//
class test101
{
    static int limit = 6;
    static Capsule cap = new Capsule(23);

    public static void Main()
    {
        int i, j;
        ConsumerClass consumer = new ConsumerClass(mych);

        Thread thread_a = new Thread(new ThreadStart(consumer.process_a));
        Thread thread_b = new Thread(new ThreadStart(consumer.process_b));
        thread_a.Start();
        thread_b.Start();

        Console.WriteLine("Times Table Capsule Channel Test: limit=" + limit);
        for (i = 1; i <= limit; i++)
        {
            for (j = 1; j <= limit; j++)
		{
		   // Must not do allocs inside loops (this year!)
		   //Capsule cap = new Capsule(i*j);
		   cap.foo = i*j;
		   cap.newlinef = (j == limit);
		   mych.Write(cap);
		}
	   Kiwi.Pause();
        }
        Console.WriteLine("\nTest101: Capsule Times Table Test: Finished\n");
    }
}

// eof