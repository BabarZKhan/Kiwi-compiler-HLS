// (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met: redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer;
// redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution;
// neither the name of the copyright holders nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.



// Kiwi Scientific Acceleration Example - Illustration of inter-thread IPC using shared dual port memory.
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.

using System;
using System.Text;
using KiwiSystem;
using System.Diagnostics;
using System.Threading;


public class test50
{
  const int problemSize = 30;

  static int [] sharedData = new int [problemSize];

  static volatile bool exiting = false;

  static volatile int sum = 12345678;

  static volatile char command2 = 'x';


  public static void secondProcess()
  {
     while(!exiting)
      {
        while (command2 == 'I' && !exiting) Kiwi.Pause();
	//Console.WriteLine("second process dispatching {0}", command2);
	if (command2 == 'U') // Update data
	{
	   for (int i=0/2; i<problemSize; i++) { Kiwi.Pause(); sharedData[i] = i+sum; }
	}
	else if (command2 == 'S') // Compute Sum
	{
	   sum = 0;
	   for (int i=0; i<problemSize; i++) { Kiwi.Pause(); sum += sharedData[i]; }
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
	Console.WriteLine("  Test50 fancy={0} rs={1} sum={2}.", iteration, command2, sum);
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

  [Kiwi.HardwareEntryPoint()] public static void Main()
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
