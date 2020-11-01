// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
//
//
// test17:  higher-order programming: collection of function pointer management examples.  test55 is a simpler test without method pointer collections.
//
//  r2 = delegate dynamic mux

using System;
using System.Text;
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


      static int hof_foofunc_A(int v) { return v + 10010; } // These are the functions that are passed. No free vars and static in this simple case.
      static int hof_foofunc_B(int v) { return v + 10011; } 

      [Kiwi.HardwareEntryPoint()]
      static void run_test17r2()
        {
	   Console.WriteLine("Kiwi Scientific Acceleration - Test17r2 start.");
	   PointerToFunction wot = hof_foofunc_A;
	   int katy;
           for (katy=2000; katy < 5000; katy+=500)
            {
	      AddToHash(katy, wot);
	      wot = hof_foofunc_B;   
              Kiwi.Pause();
            }
	   Console.WriteLine("Kiwi Scientific Acceleration - Test17r2 finished. katy={0}", katy);
        }


      static void Main(string[] args)
      {
	run_test17r2();
      }
    }


//
//
// eof



