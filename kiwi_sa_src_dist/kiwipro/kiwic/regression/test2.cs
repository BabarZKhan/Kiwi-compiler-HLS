// $Id: test2.cs,v 1.5 2011/09/28 10:49:33 djg11 Exp $
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// (C) 2011-16 DJ Greaves
// A channel passing example: abstract data type passed along a channel between threads.
// Simple shared variable and mutex use but no fences.
// NB: Single compilation unit for sender and receiver ... how to make separate?
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


class Capsule // Capsules are passed over the channel.
{
//  [Kiwi.Elaborate()] 
  public bool newlinef;
//  [Kiwi.Elaborate()] 
  public int capint;

  public Capsule(int myval) 
	{ capint = myval;
	  newlinef = false; 
        }
}


class ConsumerClass
{
    Kiwi.Channel<Capsule> oxo;
    public static volatile bool exiting = false;
    public ConsumerClass(Kiwi.Channel<Capsule> c) // constructor
    {
        oxo = c;
    }

    public void process()
    {
      for (int count=0; count < 10; count ++)
        {
            Capsule rat = oxo.Read();
            Console.Write("{0} ", rat.capint);
            if (rat.newlinef) Console.WriteLine("");
	    Kiwi.Pause();
	    if (exiting) break;
        }
    }
}


//
// Print the times table by sending messages in capsules over a channel.
//

class test2
{
    static int limit = 6;
    static Capsule cap = new Capsule(23);

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        int i, j;
        Console.WriteLine("Times Table Capsule Channel Test: limit=" + limit);
        Kiwi.Channel<Capsule> mych = new Kiwi.Channel<Capsule>();
        ConsumerClass consumer = new ConsumerClass(mych);

        Thread thread1 = new Thread(new ThreadStart(consumer.process));
        thread1.Start();


        for (i = 1; i <= limit; i++)
        {
            for (j = 1; j <= limit; j++)
		{
		   // Must not do allocs inside loops (this year!)
		   //Capsule cap = new Capsule(i*j);
		   cap.capint = i*j;
		   cap.newlinef = (j == limit);
		   mych.Write(cap);
		}
	   Kiwi.Pause();
        }
        ConsumerClass.exiting = true;
        Kiwi.Pause();
        Console.WriteLine("\nTest2: Capsule Times Table Test: Finished\n");
    }
}

// eof