//
// Kiwi Scientific Acceleration Example - high-bandwidth I/O busses.
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//

// test51 widebus - fat net-level I/O ports.


// test51a widebus - fat net-level I/O ports using arrays.
// test51b widebus - fat net-level I/O ports using structs

using System;
using KiwiSystem;

// Wide busses can be modelled with structs or arrays.  Structs are pass by value and arrays are pass by reference, but the RTL only handles pass by value.

// NOTE - NOT TESTED WITH KIWIC YET 




public class WideWordDemoUsingStructs
{
  // Demo of wide input and output words.
  // You may want to overload your arithmetic operators to handle such constructs?

  // This is a C# struct, not a C# class.  Structs behave like other native valuetypes.
  public struct widenet<T1>
  {
    public ulong word1, word0;
    T1 bopper;
  }

  [Kiwi.OutputWordPort("normal")] public static ulong normal;

  [Kiwi.InputWordPort("uword128_in")] public static widenet<char> sword128_in;
  [Kiwi.OutputWordPort("uword128_out")] public static widenet<char> sword128_out;

  static void valuetype_test(widenet<char> bof) // Structs are passed by value, and call-by-value still gives a local copy
  {
    bof.word0 += 1; // Falls foul of operating on formals if passed by value?
  }


  [Kiwi.HardwareEntryPoint()]
  public static void Main() // A candidate for the RTL design style? See Kiwi.ClockDom.
  {
    Console.WriteLine("Starting widebus1 incrementer.");
    while (true)
    {
       normal = 4Lu;                                    
       sword128_out.word0 = sword128_in.word0 + 1;                                     // Increment: add one to input low word
       sword128_out.word1 = sword128_in.word1 + ((sword128_in.word0 ==0) ?1Lu:0Lu);    // with a carry into high word.
       Kiwi.Pause();
    }
  }
}

// eof


