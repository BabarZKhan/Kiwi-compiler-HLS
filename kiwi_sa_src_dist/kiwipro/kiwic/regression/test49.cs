//
// Kiwi Scientific Acceleration Example - Simple floating-point tests. (cf test34)
//
// (C) 2014 DJ Greaves, University of Cambridge, Computer Laboratory.
//


using System;
using System.Text;
using KiwiSystem;
using System.Diagnostics;


public class test49
{
  const int problemSize = 6;

  static double [] data = new double [problemSize];

  static volatile int volx = 100; // This defeats compile-time constant propagation.

  public static void test49_phase0()
	{
	   Console.WriteLine("Kiwi Demo - Test49 phase0 starting.");
	   for (int i=0; i<problemSize; i++) 
	   {
	     double qfp0 = (double)((volx+i)*3330.2);
  	     Kiwi.Pause();
             Console.WriteLine("data {0}  qfp0={1}", i, qfp0);	
	     float qfp1 = (float) qfp0;
  	     Kiwi.Pause();
	     float qfp2 = 7.12345f * (float) i;
	     int qfp3 = (int) qfp1;
  	     Kiwi.Pause();
             Console.WriteLine("                  qfp1={0}  qfp2={1}  qfp3={2}", qfp1, qfp2, qfp3);
  	     Kiwi.Pause();
	     }
	   } 

  public static void test49_phase1()
	{
	   Console.WriteLine("Kiwi Demo - Test49 phase1 starting.");
	   Kiwi.Pause();
	   for (int i=0; i<problemSize; i++) data[i] = 3.1415;
	   data[problemSize-1] = 2.71;
	   for (int it=0; it<3; it++)
	   {
	     Kiwi.Pause();
	     data[1] *= 100.0;
	     data[2] -= 100.0;
	     data[3] /= 100.0;
	     data[4] += 100.0;
	     for (int i=0; i<problemSize; i++)
	     {   
	       Console.WriteLine("phase1: data {0}  is {1}", i, data[i]);
	     }
	   } 
	   Console.WriteLine("Kiwi Demo - Test49 phase1 finished.");
        }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
	{
	   Console.WriteLine("Kiwi Demo - Test49 starting.");
	   Kiwi.Pause();
	   test49_phase0();
	   test49_phase1();
	   Console.WriteLine("Test49 done.");
	}
}

// eof

// Notes:     40e3f34d 7.123450 7.123450e+00

