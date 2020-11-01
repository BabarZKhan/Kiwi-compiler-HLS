// Kiwi Scientific Acceleration: KiwiC regression tests
// $Id: test18.cs,v 1.5 2011/05/13 09:25:19 djg11 Exp $

// TEST 18:  A simple inheritance test


using System;
using System.Text;
using KiwiSystem;

    class Tog1
    {
      public static int andy = 10;

      public static void k1()  
      {
        Console.WriteLine("k1-a: Shall I compare");
        Console.WriteLine("k1-b: Thou art more lovely");
	Kiwi.Pause();
      }

      public static void k2()  
      {
        Console.WriteLine("Tog1-k2: Not visible");
	Kiwi.Pause();
      }

      public static void k3()  
      {
        Console.WriteLine("Tog1-k3: Not visible");
	Kiwi.Pause();
      }

    }


    class Tog2 : Tog1
    {
      public static int boris = 100;
      public new static void k2()  
      {
        Console.WriteLine("k2-a: Rough winds may shake");
        Console.WriteLine("k2-b: And Summer's lease hath all too short a fig");
	Kiwi.Pause();
      }

      new public static void k3()  
      {
        Console.WriteLine("Tog2-k3: Not visible");
	Kiwi.Pause();
      }

    }


    class Tog3 : Tog2
    {
      static int carly = 1000;

        [Kiwi.InputWordPort(9,0)]
        public static int x;

        [Kiwi.InputWordPort(10,0)]
        public static int y;

        [Kiwi.OutputWordPort(11,0)]
        public static int zout;


      public new static void k3()  
      {
        Console.WriteLine("Tog3-a: Sometimes too hot");
        Console.WriteLine("Tog3-b: And oft is his gold complexion dimm'd");
	Kiwi.Pause();
      }


        static void Main()
        {
           for (int katy = 1000; katy < 2000; katy += 1000)
            {
		Console.WriteLine("The answer is {0} at the start", andy+boris+carly+1);
		k1();
		k2();
		k3();
		Console.WriteLine("The answer is {0} at the end", andy+boris+carly+1);
            }

        }
    }


//
//
// eof



