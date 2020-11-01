// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// $Id: test17.cs,v 1.4 2011/01/20 16:10:42 djg11 Exp $
//
//
// test17:  higher-order programming: collection of function pointer management examples.  test55 is a simpler test without method pointer collections.
//
// r3 - with object env (closure)

using System;
using System.Text;
using KiwiSystem;

    class SlaveClass
    {
      int myfoo = 2;

      public int Opand(int a, int b) // This function is used in a higer-order way and it requires its object closure.
      {
	myfoo += b;
	return a - b + myfoo;
      }
    }

    class KtestFpointers
    {
      [Kiwi.OutputWordPort(31,0)]
      public static int zout;

      // This generates a System.MulticastDelegate    
      public delegate int PointerToFunction(int vest, int sox); // This line is like a 'typedef'

      public static void AddToHash(int key, PointerToFunction XYZ) 
      {
	zout = XYZ(key, key * 2);  // Apply the higher-order function.
	Kiwi.Pause();
	Console.WriteLine("Set zout to {0}", zout);
      }
      
      static int hof_foofunc(int v) { return v + 10; } // This is the function that gets passed. No free vars and static in this simple case.

      [Kiwi.HardwareEntryPoint()]
      static void run_test17r3()
        {
	  SlaveClass sc = new SlaveClass();
	  
	  Console.WriteLine("Kiwi Scientific Acceleration - Test17r3 start.");
	  for (int katy=2000; katy <5000; katy+=500)
            {
	      AddToHash(katy, sc.Opand);   
              Kiwi.Pause();
            }
	   Console.WriteLine("Kiwi Scientific Acceleration - Test17r3 finished.");
        }


      static void Main(string[] args)
      {
	run_test17r3();

      }
    }


//
//
// eof



