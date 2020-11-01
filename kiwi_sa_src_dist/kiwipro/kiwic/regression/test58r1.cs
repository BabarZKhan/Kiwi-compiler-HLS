// Kiwi Scientific Acceleration
// (C) 2016 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test58 : 58r0 sqrt - original version.
//          58r1 sqrt - automatically load and use the kickoff-libraries implementation.
//
// Q. What does autoload from kickoff mean?  
// A. An implementation of the component is in C# in the KickOff library.
//
// There are three ways an IP-block such as sqrt can be deployed:
//
// 1. The C# src code for the sqrt is inlined by KiwiC and the ALUs is uses are borrowed/shared with other parts of the application.
// 2. The square-root FU is compiled to an RTL module and becomes shared as per other stateless FUs and and ALUs.
// 3. A back-end substitution is made where the body of the C# implementation is ignored a foreign RTL implementation of the SQRT FU  is deployed and sharable.
//


using System;
using System.Text;
using KiwiSystem;

class Test58r1
{
  static void double_test()
  {
    Console.WriteLine("Test58r1: Double Test");
    for (long p=2; p< (long)(1e15); p *= 204)
        {
          Kiwi.Pause();
          double pf = (double)p;
          Console.WriteLine("    p={0}   pf={1}", p, pf);
          double d_sr = Math.Sqrt(pf);
          double d_error = (p - d_sr * d_sr)/p;
          Console.WriteLine("         root dp={0}            error={1}", d_sr, d_error);
        }
  }
  

    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
      Console.WriteLine("Test58r1 start - sqrt");
      // Standard dotnet library does not have a single precison one of course!
      Kiwi.KppMark(2, "Starting Double");
      double_test();
      Kiwi.KppMark(3, "Finished");
      //Console.WriteLine("Test58 finished at {0}.", Kiwi.tnow);
      Console.WriteLine("Test58r1 finished.", Kiwi.tnow);
    }

}

// eof
