//
// Kiwi Scientific Acceleration Example - high-bandwidth I/O busses.
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//

// test51a widebus - fat net-level I/O ports using arrays.
// test51b widebus - fat net-level I/O ports using structs

using System;
using KiwiSystem;

// Wide busses can be modelled with structs or arrays.  Structs are pass by value and arrays are pass by reference, but the RTL only handles pass by value.

// NOTE - NOT TESTED WITH KIWIC YET 


public class WideWordDemoUsingArrays
{
  // Wide input and output, net-level I/O.
  [Kiwi.InputWordPort("widein")]
  static int [] widein = new int [8]; // 32 byte parallel input

  [Kiwi.OutputWordPort("wideout")]
  static int [] wideout = new int [8]; // 32 byte parallel output

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {

   while(true)
   {
//    for (int p=0; p<8 /*widein.Length*/; p++)
//    {
//      wideout[p] = widein[p];
//    }

//    Array.Copy(wideout, widein, 0);
        wideout[0] = widein[0];
        wideout[1] = widein[1];
        wideout[2] = widein[2];
        wideout[3] = widein[3];
        wideout[4] = widein[4];
        wideout[5] = widein[5];
        wideout[6] = widein[6];
        wideout[7] = widein[7];
        Kiwi.Pause();
    }
  }
}


// eof


