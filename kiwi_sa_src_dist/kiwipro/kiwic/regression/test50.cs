//
// Kiwi Scientific Acceleration Example
// Illustration of inter-thread IPC using shared dual-port memory.
//
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//
// TODO: Ideally add a clock domain attribute here to show two different clock domains.

using System;
using System.Text;
using KiwiSystem;
using System.Diagnostics;
using System.Threading;

	// By default, KiwiC will use one port on an SRAM for each thread that operates on it.
	// However, by settings the PortsPerThread to greater than one then
	// greater access bandwidth per clock cycle for each thread is possible.  Note that Xilinx Virtex BRAM
	// supports up to two ports per BRAM in total, so having ports per thread set to two is the maximum sensible
	// value and that may only sensible if there is only one thread making access to the RAM.  In the future, several threads
        // in the same clock domain might get to share the physical ports if the compiler can spot they are temporarily disjoint
	// (i.e. never concurrent).

// Curently we have no fence instructions here - so are assuming some memory ordering semantic ... like TWO (total write ordering).

public class test50
{
  const int problemSize = 30; // normally 30

  static int [] sharedData = new int [problemSize];

  static volatile bool exiting = false;

  static volatile int sum = 12345678;

  static volatile char command2 = 'x';


  public static void secondProcess()
  {
     while(!exiting)
      {
        while (command2 == 'I' && !exiting) Kiwi.Pause();
 	//Console.WriteLine("second process dispatching {0:X} at {1}", command2, Kiwi.tnow);
	if (command2 == 'U') // Update data
	{
	   for (int i=0; i<problemSize; i++) { Kiwi.Pause(); sharedData[i] = i+sum; }
	}
	else if (command2 == 'S') // Compute Sum
	{
	   sum = 0;
	   for (int i=0; i<problemSize; i++) { Kiwi.Pause(); /*Console.WriteLine("Do sum {0} {1}", i, sum); */sum += sharedData[i]; }
	}
 	else if (command2 == 'P') // Print Sum
 	{
             Console.WriteLine("sp: data sum {0}", sum);
  	     Kiwi.Pause();
        }
	else if (command2 == 'D') // Print Data
 	{
    	   //Console.WriteLine("second process Print Data {0}", command2);
	   for (int i=0; i<problemSize; i++) 
	   { 
             Console.WriteLine("sp: Print data: sharedData[{0}] = {1}",  i, sharedData[i]);
  	     Kiwi.Pause();
           }
        }
	//Console.WriteLine("second process dispatched {0}", command2);
	command2 = 'I'; // Set back to idle
      }
   } 


  public static int computeSum()
  {
     int mysum = 0;
     for (int i=0; i<problemSize; i++) { Kiwi.Pause(); mysum += sharedData[i]; }
     return mysum;	
  }

  public static void clearto(int v0)
  {
     int vv = v0;
     for (int i=0; i<problemSize; i++) { sharedData[i] = vv++; Kiwi.Pause(); }
     sharedData[problemSize-1] = 99;

  }

  public static void test50_phase0()
   {
     Console.WriteLine("Kiwi Demo - Test50 phase0 starting.");
     Console.WriteLine("  Test50 Remote Status={0}, sum= {1}", command2, sum);
     clearto(30);
     Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
     command2 = 'D';
     Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();

     for(int iteration = 0; iteration < 3; iteration++)
     {
	Console.WriteLine("  Test50 fancy iteration={0} rs={1} sum={2}.", iteration, command2, sum);
	command2 = 'P';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
	command2 = 'S';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
	command2 = 'P';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
	command2 = 'U';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
	command2 = 'S';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();
	command2 = 'P';
	Kiwi.Pause();     while (command2 != 'I') Kiwi.Pause();

	clearto(40 + iteration);
	Console.WriteLine("   point2 {0} {1}.", command2, sum);
        Kiwi.Pause();
     }
     Console.WriteLine("Finished main process.");
    }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
	{
	   Console.WriteLine("Kiwi Demo - Test50 starting.");
           Thread thread1 = new Thread(new ThreadStart(secondProcess));
           thread1.Start();

	   Kiwi.Pause();

	   test50_phase0();
	   exiting = true;
	   Console.WriteLine("Test50 starting join.");
           //thread1.Join();
	   Console.WriteLine("Test50 done.");
	}
}

// eof


