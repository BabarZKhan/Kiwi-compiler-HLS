//
// Kiwi Scientific Acceleration Example - Illustration of Kiwi Pipelined Accelerator
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//


using System;
using System.Text;
using KiwiSystem;
using System.Diagnostics;
using System.Threading;


public class test54
{
  static readonly uint[] htab4 = { 0x51f4a750, 0x7e416553, 0x1a17a4c3, 0x3a275e96 };

  [Kiwi.PipelinedAccelerator("BiQuadxKernel", "II=1:MaxLat=16")]
  static uint Accel1(uint a0)
  {
    uint r0 = a0;
    for (int p=0; p<3; p++) { r0 += htab4[(r0 >> 6) % htab4.Length]; }
    return r0;
  }


//
//  [Kiwi.HardwareEntryPoint()]
// We do NOT designate Main as a hardware entry point currently, but could do so to get an RTL testbench for our accelerator.

  public static void Main()
	{
	   Console.WriteLine("Kiwi Demo - Test54 starting.");

	   for (uint p=0; p< 4; p++)
	     {
	       uint vv = p*1111;
	       uint rr = Accel1(vv);
	       Console.WriteLine("Test54 run {0}  in={1} out={2}", p, vv, rr);
	     }

	   Console.WriteLine("Test54 done.");
	}
}

// eof


