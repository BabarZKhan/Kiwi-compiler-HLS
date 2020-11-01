// primesya.cs - Sieve of Eratosthenes demo.
// Kiwi Scientific Acceleration
// $Id: primesya.cs,v 1.7 2011/06/08 17:45:10 djg11 Exp $
// (C) 2010 - DJ Greaves - University of Cambridge Computer Laboratory


using System;
using KiwiSystem;

/*
  Example runtimes on mono (no Kiwi) for 100K (intel i5-3337 @1.8GHz)
  A lot of prints here, so a bit slow, but shows the effect of evariation.

There are three versions of the algorithm controlled by evariant.
evariant=0  real	0m10.501s user	0m4.798s  sys	0m5.763s
evariant=1  real	0m4.870s  user	0m2.288s  sys	0m2.612s
evariant=2  real	0m3.502s  user	0m1.571s  sys	0m1.952s
*/


// Correct output is one of:
//   There are 27 primes below the natural number 100.
//   There are 48 primes below the natural number 200.
//   There are 80 primes below the natural number 400.
//   There are 111 primes below the natural number 600.
//   There are 170 primes below the natural number 1000.
//   There are 305 primes below the natural number 2000.
//   There are 671 primes below the natural number 5000.
//   There are 1231 primes below the natural number 10000.
//   There are 5135 primes below the natural number 50000.
//   There are 9159 primes below the natural number 95000.
//   There are 9594 primes below the natural number 100000. - icarus stopped tally at 98303

//  NB: generic prime density is l div  log (l+1) = 874 (cf 1231)


// There are three (at least) variations on this program that vary in efficiency but give the same result.
// They vary in the their control flow graphs.
// Performance predictor questions:
//   1. Can we extrapolate the performance within one variation to larger runs ?
//   2. Can we estimate the performance of the optimised code from the non-optimised code?
//   
//   The extrapolation is complicated by the DRAM banks, because the smaller runs operate within one row of each bank so there is now row writeback and precharge overhead. For the latter crossoff stages, this becomes a significant overhead owing to the wide strathe between accesses.
//   
//   Adding a DRAM cache makes little difference owing to excessive churn, but could we automatically predict that ?
//   

class primesya
{
//   
//   The following major parameter is edited by a sed script invoked by the Makefile before each run.
//   It needs to be a compile-time constant since KiwiC chooses what type of memory organisation to use based on its value.
  static int limit = 200;
  
  //Manual control via C#: Offchip use [Kiwi.OutboardArray()] or onchip use [Kiwi.OutboardArray("")]
  static bool [] PA = new bool[limit];

//  static uint [] PY = new uint[limit];
//  static int [] PZ1 = new int[limit];
//  static int [] PZ2 = new int[limit];

  // This input port, vol, was added to make some input data volatile.
  [Kiwi.InputWordPort(31, 0)][Kiwi.OutputName("volume")] static uint vol;
  
  [Kiwi.OutputWordPort(31, 0)][Kiwi.OutputName("count")] static uint count = 0;
  
  static int count1 = 0;
  
  [Kiwi.OutputWordPort(31, 0)][Kiwi.OutputName("elimit")] static int elimit = 0;      // The main scaling parameter (abscissa).

  // The evariant_master is also edited by a sed script that runs an individual experiment.
  // For fair comparison with mono native this needs to be a compile time constant.
    const int evariant_master = 0;

    [Kiwi.OutputWordPort(31, 0)][Kiwi.OutputName("evariant")] static int evariant_output = evariant_master;  // The alogorithmic variant - must hold when finish is asserted.
    [Kiwi.OutputWordPort(31, 0)][Kiwi.OutputName("edesign")] static int edesign = 4032; // A uid for this program


  [Kiwi.OutputWordPort("ksubsMiscMon0")]  static int misc_mon0;
  [Kiwi.InputWordPort("ksubsMiscReg0")]  static int misc_reg0;
  [Kiwi.InputWordPort("ksubsGpioSwitches")]  static int ksubsGpioSwitches;

  [Kiwi.OutputWordPort("ksubsResultLo")]  static int result_lo;
  [Kiwi.OutputWordPort("ksubsResultHi")]  static int result_hi;  
  [Kiwi.OutputBitPort("ksubsDesignSerialNumber")] static int serial_number = 0x001010;



  [Kiwi.HardwareEntryPoint()]
  public static void Main()
    {
      bool kpp = true;
      elimit = limit;
      Kiwi.KppMark(1, "START", "INITIALISE");  // Waypoint
      Console.WriteLine("Primes Up To " + limit);
      Kiwi.Pause(); 
      PA[0] = vol > 0; // Process some runtime input data on this thread - prevents Kiwic running the whole program at compile time.
      Kiwi.Pause(); 
      // Clear array
      
      count1 = 2; count = 0; // RESET VALUE FAILED AT ONE POINT: HENCE NEED THIS LINE
      for (int woz = 0; woz < limit; woz++) 
	{ Kiwi.Pause(); 
	  PA[woz] = true; 
	  Console.WriteLine("Setting initial array flag to hold : addr={0} readback={1}", woz, PA[woz]); // Read back and print.
	}
      
      Kiwi.KppMark(2, "wp2", "CROSSOFF"); // Waypoint
      int i, j;
      
      for (i=2;i<limit; i++)  // Can our predictor cope with the standard optimisations?
	{
	  Kiwi.Pause();
	  // Cross off the multiples - optimise by skipping where the base is already crossed off.
	  if (evariant_master > 0)	
	    {
	      bool pp = PA[i];
	      Console.WriteLine(" tnow={2}: scaning up for live factor {0} = {1} ", i, pp,  Kiwi.tnow);
	      if (!pp)		  continue;
	      count1 += 1;
	    }
	  // Can further optimise by commencing the cross-off at the factor squared.
	  j= (evariant_master > 1) ? i*i : i+i;
	  if (j >= limit)	
	    {
	      Console.WriteLine("Skip out on square");
	      break;
	    }
	  for (; j<limit; j+=i) 
            {
	      Console.WriteLine("Cross off {0} {1}   (count1={2})", i, j, count1);
	      Kiwi.Pause(); PA[j] = false; 
            }
        }
      Kiwi.KppMark(3, "wp3", "COUNTING");  // Waypoint
      Console.WriteLine("Now counting");
      // Count how many there were and store them consecutively in the output array.
      for (int w = 0; w < limit; w++) 
	{ Kiwi.Pause();  
	  if (PA[w]) 
	    {
              count += 1; 
	      if (false)
		{
//	             PY[count] = (uint)w;
//       	     PZ1[count] = w;
//		     PZ2[count] = w;
		}
	    }
          Console.WriteLine("Tally counting {0} {1}", w, count);
          //Console.WriteLine("Tally counting {0} {1} at {2}", w, count, Kiwi.tnow);
	}


      Console.WriteLine("There are {0} primes below the natural number {1}.", count, limit);
      Console.WriteLine("Optimisation variant={1} (count1 is {0}).", count1, evariant_master);
      result_lo = (int)count;
      result_hi = (int)limit;
      Kiwi.Pause();
      Kiwi.KppMark(5, "FINISH");  // Waypoint
      Kiwi.Pause();
    Kiwi.KppMark(0, "FINISHED");
    Kiwi.ReportNormalCompletion();
    }
}


// eof
