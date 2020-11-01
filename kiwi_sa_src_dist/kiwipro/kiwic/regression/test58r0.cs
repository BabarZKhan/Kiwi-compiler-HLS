// Kiwi Scientific Acceleration
// (C) 2016 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test58 : 58r0 sqrt - original version.
//          58r1 sqrt - automatically load and use the kickoff-libraries implementation.
//

using System;
using System.Text;
using KiwiSystem;
using System.Threading;

//   * ====================================================
//   * Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
//   *
//   * Developed at SunPro, a Sun Microsystems, Inc. business.
//   * Permission to use, copy, modify, and distribute this
//   * software is freely granted, provided that this notice
//   * is preserved.
//   * ====================================================
//  
//  	     ------------------------  	     ------------------------
//  	x0:  |s|   e    |    f1     |	 x1: |          f2           |
//  	     ------------------------  	     ------------------------
//  
//  	By performing shifts and subtracts on x0 and x1 (both regarded
//  	as integers), we obtain an 8-bit approximation of sqrt(x) as
//  	follows.
//  
//  		k  := (x0>>1) + 0x1ff80000;
//  		y0 := k - T1[31&(k>>15)].	... y ~ sqrt(x) to 8 bits
//  	Here k is a 32-bit integer and T1[] is an integer array containing
//  	correction terms. Now magically the floating value of y (y's
//  	leading 32-bit word is y0, the value of its trailing word is 0)
//  	approximates sqrt(x) to almost 8-bit.
//  
//  	Value of T1:
//  	static int T1[32]= {
//  	0,	1024,	3062,	5746,	9193,	13348,	18162,	23592,
//  	29598,	36145,	43202,	50740,	58733,	67158,	75992,	85215,
//  	83599,	71378,	60428,	50647,	41945,	34246,	27478,	21581,
//  	16499,	12183,	8588,	5674,	3403,	1742,	661,	130,};
//  
//      (2)	Iterative refinement
//  
//  	Apply Heron's rule three times to y, we have y approximates
//  	sqrt(x) to within 1 ulp (Unit in the Last Place):
//  
//  		y := (y+x/y)/2		... almost 17 sig. bits
//  		y := (y+x/y)/2		... almost 35 sig. bits
//  		y := y-(y-x/y)/2	... within 1 ulp
//

class test58_sqrt
{
  static uint [] T1 = new uint [] 
    {
      0,	1024,	3062,	5746,	9193,	13348,	18162,	23592,
      29598,	36145,	43202,	50740,	58733,	67158,	75992,	85215,
      83599,	71378,	60428,	50647,	41945,	34246,	27478,	21581,
      16499,	12183,	8588,	5674,	3403,	1742,	661,	130 
    };
   

  /* From test56  - Adding the FastBitConvert attribute makes KiwiC
    ignore the bodies of functions such as these and replaces the body
    with its own fast-path identity code based only on the signatures of the functions. */
  [Kiwi.FastBitConvert()]
  static ulong fast_from_double(double darg)
  {
    byte [] asbytes = BitConverter.GetBytes(darg);
    return BitConverter.ToUInt64(asbytes, 0);
  }

  [Kiwi.FastBitConvert()]
  static double fast_to_double(ulong farg)
  {
    byte [] asbytes = BitConverter.GetBytes(farg);
    double rr = BitConverter.ToDouble(asbytes, 0);
    return rr;
  }

  [Kiwi.FastBitConvert()]
  static uint fast_from_float(float darg)
  {
    byte [] asbytes = BitConverter.GetBytes(darg);
    return BitConverter.ToUInt32(asbytes, 0);
  }

  [Kiwi.FastBitConvert()]
  static float fast_to_float(uint farg)
  {
    byte [] asbytes = BitConverter.GetBytes(farg);
    float rr = BitConverter.ToSingle(asbytes, 0);
    return rr;
  }

  public static double Sqrt(double arg) // Double Precision
  {
    if (arg<=0.0) return 0.0;
    ulong u0 = fast_from_double(arg);
    ulong k = (u0>>1) +(0x1ff8L << 48);
    ulong y0 = k - ((ulong)T1[31 & (k>>(15+32))] << 32);
    double pp = fast_to_double(y0);
//  Console.WriteLine("DP: Start iteration with u0={0:X} y0={1:X} {2}", u0, y0, pp);
    for (int qit=0;qit<3; qit++)
    {
//    Console.WriteLine("  iterate {0}", pp);
//    double qq = arg/pp;
//    double sum = qq + pp;
//    pp = sum * 0.5;
//    Console.WriteLine("     qq={0}    sum={1}   pp={2}", qq, sum, pp);
      pp = (pp + arg/pp) * 0.5; // Need 3 iterations for DP
    }
    return pp;
  }

  public static float Sqrt_sp(float arg) // Single Precision
  {
    if (arg<=0.0f) return 0.0f;
    uint u0 = fast_from_float(arg);
    //Console.WriteLine("Start approx with {0:X} {0}", u0, u0);
    const uint adjuster = (1 << 29) - (1 << 22) - 0x4C000;
    uint u1 = (u0>>1) + adjuster;
    float pp = fast_to_float(u1);
//  Console.WriteLine("Start iteration with {0:X} {0} {1}", u1, pp);
  float ap = arg/pp;
//  Console.WriteLine("Additional Print {0:X} {0}", ap);
    pp = (pp + ap /* arg/pp */) * 0.5f; // Need 2 iterations for SP
//  Console.WriteLine("Middle {0}", pp);
    pp = (pp + arg/pp) * 0.5f; 
    return pp;
  }

}


class Test58r0
{
  static void double_test()
  {
    Console.WriteLine("Test58r1: Double Test");
    for (long p=2; p< (long)(1e15); p *= 204)
        {
          Kiwi.Pause();
          double pf = (double)p;
          Console.WriteLine("    p={0}   pf={1}", p, pf);
          double d_sr = test58_sqrt.Sqrt(pf);
          //double d_sr = Math.Sqrt(pf);
          double d_error = (p - d_sr * d_sr)/p;
          Console.WriteLine("         root dp={0}            error={1}", d_sr, d_error);
        }
  }
  
  static void single_test()
  {
    Console.WriteLine("Test58r0: Single Test");
    for (long p=2; p< (long)(1e15); p *= 204)
        {
          Kiwi.Pause();
          float pf = (float)p;
          Console.WriteLine("    p={0}   pf={1}", p, pf);
          float d_sr = test58_sqrt.Sqrt_sp(pf);
          //float d_sr = (float)Math.Sqrt((double)pf);
          float d_error = (p - d_sr * d_sr)/p;
          Console.WriteLine("         root sp={0}            error={1}", d_sr, d_error);
        }
  }
  

    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
      Console.WriteLine("Test58r0 start - sqrt");
      Kiwi.KppMark(1, "Starting Single");
      single_test();
      Kiwi.KppMark(2, "Starting Double");
      double_test();
      Kiwi.KppMark(3, "Finished");
      //Console.WriteLine("Test58 finished at {0}.", Kiwi.tnow);
      Console.WriteLine("Test58 finished.", Kiwi.tnow);
    }

}

// eof
