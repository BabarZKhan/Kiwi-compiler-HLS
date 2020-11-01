// $Id: test2.cs,v 1.5 2011/09/28 10:49:33 djg11 Exp $
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
//
// A channel passing example: abstract data type passed along a channel
//
// NB: Single compilation unit for sender and receiver ... how to make separate?
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


class Capsule // Capsules are passed over the hardware channel between threads.
{
  public bool newlinef;
  public int capint;

  public Capsule(int myval) 
	{ capint = myval;
	  newlinef = false; 
        }
}


class ConsumerClass
{
    Kiwi.Channel<Capsule> workin, empties;
    public static volatile bool exiting = false;
    public ConsumerClass(Kiwi.Channel<Capsule> fwd, Kiwi.Channel<Capsule> rev) // constructor
    {
       workin = fwd;  empties = rev;
       Capsule cap2 = new Capsule(45);
       empties.Write(cap2);
    }

    public void process()
    {
      Kiwi.Pause();
      for (int count=0; count < 10; count ++)
        {
            Capsule rat = workin.Read();
            Console.Write("{0} ", rat.capint);
            if (rat.newlinef) Console.WriteLine("");
	    Kiwi.Pause();
	    empties.Write(rat);
            if (exiting) break;
        }
    }
}



//
// Print the times table by sending messages in capsules over a channel.
//
class test2r1
{
    static int limit = 3;
    static Capsule cap1 = new Capsule(23);
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        int i, j;
        Console.WriteLine("Test2r1: Times Table Capsule Channel Test: limit=" + limit);
        Kiwi.Channel<Capsule> fwd = new Kiwi.Channel<Capsule>();
        Kiwi.Channel<Capsule> rev = new Kiwi.Channel<Capsule>();

        ConsumerClass consumer = new ConsumerClass(fwd, rev);

        Thread thread1 = new Thread(new ThreadStart(consumer.process));
        thread1.Start();

	Capsule capx = cap1;

        for (i = 1; i <= limit; i++)
        {
            for (j = 1; j <= limit; j++)
		{
		   // Must not do allocs inside runtime loops pre Kiwi2, so pass a static number of capsule instances around the system.
		   //Capsule cap = new Capsule(i*j);
		   capx.capint = i*j;
		   capx.newlinef = (j == limit);
		   fwd.Write(capx);
		   capx = rev.Read();
		   ConsumerClass.exiting = (i==limit && j == limit); // There's a nasty race on setting exit flag before consumer goes into blocking read!
		}
	   Kiwi.Pause();
        }
 
        Console.WriteLine("\nTest2r1: Capsule Times Table Test: Finished\n");
    }
}

// eof