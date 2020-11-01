// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// $Id: test17.cs,v 1.4 2011/01/20 16:10:42 djg11 Exp $
//
//
// test17:  higher-order programming: collection of function pointer management examples.  test55 also tests delegates.
//
//  It would be good to extend this test for hof_function not static, but perhaps there's another test of that in the suite? I recall one.
//
using System;
using System.Text;
using System.Threading;
using KiwiSystem;

    class KtestFpointers
    {
        [Kiwi.OutputWordPort(31,0)]
        public static int zout;

  // This generates a System.MulticastDelegate    
        public delegate int PointerToFunction(int sox); // This line is like a 'typedef'

	public static void AddToHash(int key, PointerToFunction XYZ) 
        {
	  zout = XYZ(key + 101);  // Apply the higher-order function.
          Kiwi.Pause();
	  Console.WriteLine("Set zout to {0}", zout);
        }

      static int hof_foofunc(int v) { return v + 10; } // This is the function that gets passed. No free vars and static in this simple case.

      [Kiwi.HardwareEntryPoint()]
      static void run_test17()
        {
	   Console.WriteLine("Kiwi Scientific Acceleration - Test17 start.");
           for (int katy=2000; katy <5000; katy+=500)
            {
// TODO reinstate System.Int32.Equals
//	      if (katy.Equals(2000)) Console.WriteLine("Equals test");
	      AddToHash(katy, hof_foofunc);   
              Kiwi.Pause();
            }
	   Console.WriteLine("Kiwi Scientific Acceleration - Test17 finished.");
        }


      static void Main(string[] args)
      {
	run_test17();

      }
    }


//
//
// eof



