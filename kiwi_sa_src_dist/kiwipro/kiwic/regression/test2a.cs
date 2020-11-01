// test2a.cs
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// (C) 2011-16 DJ Greaves
// Simple shared variable and mutex use but no fences.
//
// NB: Single compilation unit for sender and receiver ... TODO explain how to make separate?
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;



class ConsumerClass
{
  public volatile static int shvar = 1;

  public void process()
  {
    for (int count=0; count < 100; count ++)
      {
	if (shvar > 0) 
	  {
	    shvar -= 1;
	    Console.WriteLine("Decremented {0}", shvar);	      
	  }
	Kiwi.Pause();
      }
    }
}

//
// 
//

class test2a
{
    static int limit = 6;

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        int i, j;
        Console.WriteLine("Test2a: limit=" + limit);
        Console.WriteLine("Note: this test is very fragile owing to races and has completely non-determinstic output on mono - but in hard pause mode the RTL behaviour is stable enough to warrant regression testing.");
        ConsumerClass consumer = new ConsumerClass();

        Thread thread1 = new Thread(new ThreadStart(consumer.process));
        thread1.Start();

        for (i = 1; i <= limit; i++)
        {
            for (j = 1; j <= 10; j++)
	      {
		Kiwi.Pause();
	      }
	    Kiwi.Pause();
	    ConsumerClass.shvar += 5;
        }
        Kiwi.Pause();
        Console.WriteLine("\nTest2a: Finished\n");
    }
}

// eof