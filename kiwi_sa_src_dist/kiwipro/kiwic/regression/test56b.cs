// Kiwi Scientific Acceleration
// Cordic implemented from Java. With testbench from CHstone dfsin.
// (C) 2014 DJ Greaves, University of Cambridge, Computer Laboratory.

// This variant makes use of bits-to-real on input and output but is fixed-point for all internal working.

using float64 = System.UInt64;
using System;
using KiwiSystem;

public class CordicWithIntegerAngle 
{
  const int iterations = 56;
  static double [] ctab_new = new double [iterations];

  // This table generation is normally done offline, once and for all time.
  public static void Table_Generate() 
  {
    double theta = 1.0; // 1 radian ~= 57.3 degrees.
    for (int i=0; i<iterations; i++)
      {
        double at = Math.Atan(theta);
	ctab_new[i] = at;
        ulong ival = (ulong)(Math.Pow(2.0, 55.0) * at);
        Console.WriteLine(" {0},  // {1}", ival, at);
	Console.WriteLine("Cordic table {0} is arctan {1} = {2}", i, theta, ctab_new[i]);
	theta = theta / 2.0;
      }
  }

  class hutils
  {

  /* Adding the FastBitConvert attribute makes KiwiC
    ignore the bodies of functions such as these and replaces the body
    with its own fast-path identity code based only on the signatures of the functions. */

       [Kiwi.FastBitConvert()]
       public static double to_double(float64 farg)
       {
         byte [] asbytes = BitConverter.GetBytes(farg);
         double rr = BitConverter.ToDouble(asbytes, 0);
         return rr;
       }

       [Kiwi.FastBitConvert()]
       public static float64 from_double(double darg)
       {
         byte [] asbytes = BitConverter.GetBytes(darg);
         return BitConverter.ToUInt64(asbytes, 0);
       }

       [Kiwi.FastBitConvert()]
       public static float to_single(uint farg)
       {
         byte [] asbytes = BitConverter.GetBytes(farg);
         float rr = BitConverter.ToSingle(asbytes, 0);
         return rr;
       }

       [Kiwi.FastBitConvert()]
       public static uint from_single(float sarg)
       {
         byte [] asbytes = BitConverter.GetBytes(sarg);
         return BitConverter.ToUInt32(asbytes, 0);
       }

   }



  static readonly long [] ctab = new [] {  // w.r.t. a denominator of 2^55
      28296951008113760L,  // 0.785398163397448 // arctan 1.0 is 45 degrees, or 3/4 of a radian.
      16704665593018014L,  // 0.463647609000806
      8826286527774941L,  // 0.244978663126864
      4480360856819639L,  // 0.124354994546761
      2248874635509626L,  // 0.0624188099959574
      1125533617565380L,  // 0.0312398334302683
      562904147146537L,  // 0.0156237286204768
      281469250297300L,  // 0.00781234106010111
      140736772533998L,  // 0.00390623013196697
      70368654699383L,  // 0.00195312251647882
      35184360904027L,  // 0.000976562189559319
      17592184646314L,  // 0.000488281211194898
      8796092847445L,  // 0.000244140620149362
      4398046489258L,  // 0.00012207031189367
      2199023252821L,  // 6.10351561742088E-05
      1099511627434L,  // 3.05175781155261E-05
      549755813845L,  // 1.52587890613158E-05
      274877906938L,  // 7.62939453110197E-06
      137438953471L,  // 3.8146972656065E-06
      68719476735L,  // 1.90734863281019E-06
      34359738367L,  // 9.53674316405961E-07
      17179869183L,  // 4.76837158203089E-07
      8589934591L,  // 2.38418579101558E-07
      4294967295L,  // 1.19209289550781E-07
      2147483647L,  // 5.96046447753906E-08
      1073741823L,  // 2.98023223876953E-08
      536870911L,  // 1.49011611938477E-08
      268435456L,  // 7.45058059692383E-09
      134217728L,  // 3.72529029846191E-09
      67108864L,  // 1.86264514923096E-09
      33554432L,  // 9.31322574615479E-10
      16777216L,  // 4.65661287307739E-10
      8388608L,  // 2.3283064365387E-10
      4194304L,  // 1.16415321826935E-10
      2097152L,  // 5.82076609134674E-11
      1048576L,  // 2.91038304567337E-11
      524288L,  // 1.45519152283669E-11
      262144L,  // 7.27595761418343E-12
      131072L,  // 3.63797880709171E-12
      65536L,  // 1.81898940354586E-12
      32768L,  // 9.09494701772928E-13
      16384L,  // 4.54747350886464E-13
      8192L,  // 2.27373675443232E-13
      4096L,  // 1.13686837721616E-13
      2048L,  // 5.6843418860808E-14
      1024L,  // 2.8421709430404E-14
      512L,  // 1.4210854715202E-14
      256L,  // 7.105427357601E-15
      128L,  // 3.5527136788005E-15
      64L,  // 1.77635683940025E-15
      32L,  // 8.88178419700125E-16
      16L,  // 4.44089209850063E-16
      8L,  // 2.22044604925031E-16
      4L,  // 1.11022302462516E-16
      2L,  // 5.55111512312578E-17
      1L,  // 2.77555756156289E-17
     };


  static double sin_kernel(long itheta, bool cosinef=false, bool vd = false)
  {
    const long two_to_the_52 = 4503599627370496; // 2^52
    long x=2734824091825632L /*prescaled constant for iterations=56*/, y=0L;
    //x = (long)(0.60725293500888 * two_to_the_52); // That constant was for 32? We have more.
    //Console.WriteLine("Starting vector value is x={0}", x);
    if (vd) Console.WriteLine("Range-reduced theta value is theta={0}", itheta);
    for (int k=0; k<iterations; ++k)
      {
	long tx, ty;
	if (vd) Console.WriteLine("              {0} cf {1}   y={2}", itheta, ctab[k], y);
	if (itheta >= 0) // All nice simple integer arithmetic
	  {
            if (vd) Console.WriteLine(" k={0} ABOVE", k);
	    tx = x - (y>>k);
	    ty = y + (x>>k);
	    itheta -= ctab[k];
	  }
	else
	  {
            if (vd) Console.WriteLine(" k={0} BELOW", k);
	    tx = x + (y>>k);
	    ty = y - (x>>k);
	    itheta += ctab[k];
	  }
	x = tx; y = ty;
      }  
    long ianswer = (cosinef)? x:y;
    double rv;
    if (false)
      {
        rv = (double)(ianswer) / (double)(two_to_the_52);        
      }
    else
      {
        bool out_zero = (ianswer==0);
        bool out_Inf = false;
        bool out_NaN = false;
        bool out_sign = false;
        if (ianswer < 0L)
          {
            out_sign = true;
            ianswer = 0L - ianswer;
          }
        int out_exponent = 1023;
        ulong result = (ulong)(ianswer);
        while (result <= (1L<<52))
          {
            result *= 2;
            out_exponent -= 1;
          }
        while (result > (1L<<53))
          {
            result /= 2;
            out_exponent += 1;
          }
        float64 f64_out;
        // Double-Precision:  IEEE 1+11+52 = 64-bit format.
        if (out_NaN) f64_out = 0x7FFF0000FFFF0000;
        else if (out_zero) f64_out = 0x0;
        else if (out_Inf) f64_out = ((out_sign) ? (1uL << 63) : 0uL) | 0x7FF000000000000uL;
        else  f64_out = ((out_sign) ? (1uL << 63) : 0uL) | (((ulong)out_exponent) << 52) | (result & ((1L<<52)-1));
        // Console.WriteLine("sqrt out_zero={0}  out_NaN={1}  out_Inf={2}", out_zero, out_NaN, out_Inf);
        // Console.WriteLine("sqrt. f64_out=0x{0:x}", f64_out);
        rv = hutils.to_double(f64_out);
      }
    return rv; // Returns sine(arg)
  }



  
//  [Kiwi.Remote("protocol=HFAST1")]
  public static double sin(double theta) 
  {
    bool flip = false;
    bool vd = false;
    if (vd) Console.WriteLine("Starting theta value is theta={0}", theta);


    if (theta < 0.0)      // Range reduction - make +ve
      {
	theta -= Math.PI;
	flip = !flip;
	if (vd) Console.WriteLine("RR origin reflect");
      }

    while (theta >= Math.PI * 2.0)  // Range reduction - Take Mod 2.PI
      {
	theta -= Math.PI * 2.0;
	Kiwi.Pause();
	Kiwi.NoUnroll(); 
	if (vd) Console.WriteLine("RR Mod 2.Pi");
      }

    if (theta >= Math.PI)  // Range reduction - Take Mod PI
      {
	theta -= Math.PI;
	flip = !flip;	
	if (vd) Console.WriteLine("RR Mod Pi");
      }

    if (theta >= Math.PI / 2.0)  // Range reduction - Reflect about PI/2
      {
        // Note  0x400921fb54442d11 = 4614256656552045841 = 3.141593e+00 DOUBLE
	theta = Math.PI - theta;
	if (vd) Console.WriteLine("RR reflect {0}", theta);
      }

//Range reduce further to +/- Pi/4 needed.
    
    Console.WriteLine("  theta now {0}", theta);
    float64 ix = hutils.from_double(theta);
    bool in_sign = ((ix >> 63) & 1L) == 1L;
    ix &= (1uL<<63)-1L; // Mask off sign bit. // Already gone.
    
    if(ix < (0x3e400000<<32))			// |x| < 2**-27 
         {  return theta; }		// generate inexact

    
    int in_exponent = (int)((ix >> 52) & 0x7FFL);
    ulong in_mantissa = ix & ((1L<<52)-1L);
    ulong mantissa = in_mantissa | (1L<<52);
    bool in_zero = (in_exponent == 0) && (in_mantissa == 0);

     /*
      public static double KiwiSineDouble_(double x_in)
       {
           float64 ix = hutils.from_double(x_in);

           // Double-Precision:  IEEE 1+11+52 = 64-bit format.
           bool in_sign = ((ix >> 63) & 1L) == 1L;
           int in_exponent = (int)((ix >> 52) & 0x7FFL);
           ulong in_mantissa = ix & ((1L<<52)-1L);
           bool in_zero = (in_exponent == 0) && (in_mantissa == 0);
           bool in_denormal = (in_exponent == 0) && (in_mantissa != 0);
           in_zero |= in_denormal; // To save gates, discard denormals.
           //Console.WriteLine("DP in s={0} exp={1} m={2}", in_sign, in_exponent, in_mantissa); 
           int out_exponent = 0;
           bool out_sign = false;
           bool out_NaN = false;
           bool out_Inf = false;
           bool out_zero = false;
           ulong res = 0uL;

           if (in_exponent == 0x7ff)
             {
                if (in_mantissa != 0 || in_sign) out_NaN = true;  // Non-zero mantissa is NaN. Zero mantissa is +/- inf.
                else
                    { out_Inf = true; out_sign = in_sign;  }
             }
           else if (in_zero)
            {
              out_zero = true;
            }
          else
            {
            }        
       }
       */
// 1022 -> 4
    long theta_i = (long)(mantissa);
    while (in_exponent > 1020)
      {
        theta_i *= 2;
        in_exponent -= 1;
        //Console.WriteLine(" theta_i {0}   exp {1}", theta_i, in_exponent);
      }
    while (in_exponent < 1020)
      {
        theta_i /= 2;
        in_exponent += 1;
        //Console.WriteLine(" theta_i {0}   exp {1}", theta_i, in_exponent);
      }
    Console.WriteLine(" theta_i {0}   exp {1}", theta_i, in_exponent);

    double rv = sin_kernel(theta_i);
    
    if (flip) rv = -rv;
    if (vd) Console.WriteLine(" ultimately {0}", rv);
    return rv;
  }
}



class CordicTestbench
{
  
  [Kiwi.OutputBitPort("done")]  static bool done = false;


  /* Adding the FastBitConvert attribute makes KiwiC
    ignore the bodies of functions such as these and replaces the body
    with its own fast-path identity code based only on the signatures of the functions. */

  [Kiwi.FastBitConvert()]
  static double to_double(float64 farg)
  {
    byte [] asbytes = BitConverter.GetBytes(farg);
    double rr = BitConverter.ToDouble(asbytes, 0);
    return rr;
  }

  [Kiwi.FastBitConvert()]
  static float64 from_double(double darg)
  {
    byte [] asbytes = BitConverter.GetBytes(darg);
    return BitConverter.ToUInt64(asbytes, 0);
  }

/*
 * Copyright (C) 2008  Y. Hara, H. Tomiyama, S. Honda, H. Takada and K. Ishii
 * Test Vectors from CHStone
*/
  const int  N = 36;

  static readonly float64 [] test_in =
    {
      0x0000000000000000ULL,	/*      0  */
      0x3fc65717fced55c1ULL,	/*   PI/18 */
      0x3fd65717fced55c1ULL,	/*   PI/9  */
      0x3fe0c151fdb20051ULL,	/*   PI/6  */
      0x3fe65717fced55c1ULL,	/*  2PI/9  */
      0x3febecddfc28ab31ULL,	/*  5PI/18 */
      0x3ff0c151fdb20051ULL,	/*   PI/3  */
      0x3ff38c34fd4fab09ULL,	/*  7PI/18 */
      0x3ff65717fced55c1ULL,	/*  4PI/9  */
      0x3ff921fafc8b0079ULL,	/*   PI/2  */
      0x3ffbecddfc28ab31ULL,	/*  5PI/9  */
      0x3ffeb7c0fbc655e9ULL,	/* 11PI/18 */
      0x4000c151fdb20051ULL,	/*  2PI/3  */
      0x400226c37d80d5adULL,	/* 13PI/18 */
      0x40038c34fd4fab09ULL,	/*  7PI/9  */
      0x4004f1a67d1e8065ULL,	/*  5PI/6  */
      0x40065717fced55c1ULL,	/*  8PI/9  */
      0x4007bc897cbc2b1dULL,	/* 17PI/18 */
      0x400921fafc8b0079ULL,	/*   PI    */
      0x400a876c7c59d5d5ULL,	/* 19PI/18 */
      0x400becddfc28ab31ULL,	/* 10PI/9  */
      0x400d524f7bf7808dULL,	/*  7PI/6  */
      0x400eb7c0fbc655e9ULL,	/* 11PI/9  */
      0x40100e993dca95a3ULL,	/* 23PI/18 */
      0x4010c151fdb20051ULL,	/*  8PI/6  */
      0x4011740abd996affULL,	/* 25PI/18 */
      0x401226c37d80d5adULL,	/* 13PI/9  */
      0x4012d97c3d68405bULL,	/*  3PI/2  */
      0x40138c34fd4fab09ULL,	/* 14PI/9  */
      0x40143eedbd3715b7ULL,	/* 29PI/18 */
      0x4014f1a67d1e8065ULL,	/* 15PI/9  */
      0x4015a45f3d05eb13ULL,	/* 31PI/18 */
      0x40165717fced55c1ULL,	/* 16PI/9  */
      0x401709d0bcd4c06fULL,	/* 33PI/18 */
      0x4017bc897cbc2b1dULL,	/* 17PI/9  */
      0x40186f423ca395cbULL
    };				/* 35PI/18 */
  



  static readonly float64 [] test_out = {
    0x0000000000000000ULL,	/*  0.000000 */
    0x3fc63a1a335aadcdULL,	/*  0.173648 */
    0x3fd5e3a82b09bf3eULL,	/*  0.342020 */
    0x3fdfffff91f9aa91ULL,	/*  0.500000 */
    0x3fe491b716c242e3ULL,	/*  0.642787 */
    0x3fe8836f672614a6ULL,	/*  0.766044 */
    0x3febb67ac40b2bedULL,	/*  0.866025 */
    0x3fee11f6127e28adULL,	/*  0.939693 */
    0x3fef838b6adffac0ULL,	/*  0.984808 */
    0x3fefffffe1cbd7aaULL,	/*  1.000000 */
    0x3fef838bb0147989ULL,	/*  0.984808 */
    0x3fee11f692d962b4ULL,	/*  0.939693 */
    0x3febb67b77c0142dULL,	/*  0.866026 */
    0x3fe883709d4ea869ULL,	/*  0.766045 */
    0x3fe491b81d72d8e8ULL,	/*  0.642788 */
    0x3fe00000ea5f43c8ULL,	/*  0.500000 */
    0x3fd5e3aa4e0590c5ULL,	/*  0.342021 */
    0x3fc63a1d2189552cULL,	/*  0.173648 */
    0x3ea6aedffc454b91ULL,	/*  0.000001 */
    0xbfc63a1444ddb37cULL,	/* -0.173647 */
    0xbfd5e3a4e68f8f3eULL,	/* -0.342019 */
    0xbfdffffd494cf96bULL,	/* -0.499999 */
    0xbfe491b61cb9a3d3ULL,	/* -0.642787 */
    0xbfe8836eb2dcf815ULL,	/* -0.766044 */
    0xbfebb67a740aae32ULL,	/* -0.866025 */
    0xbfee11f5912d2157ULL,	/* -0.939692 */
    0xbfef838b1ac64afcULL,	/* -0.984808 */
    0xbfefffffc2e5dc8fULL,	/* -1.000000 */
    0xbfef838b5ea2e7eaULL,	/* -0.984808 */
    0xbfee11f7112dae27ULL,	/* -0.939693 */
    0xbfebb67c2c31cb4aULL,	/* -0.866026 */
    0xbfe883716e6fd781ULL,	/* -0.766045 */
    0xbfe491b9cd1b5d56ULL,	/* -0.642789 */
    0xbfe000021d0ca30dULL,	/* -0.500001 */
    0xbfd5e3ad0a69caf7ULL,	/* -0.342021 */
    0xbfc63a23c48863ddULL
  };				/* -0.173649 */


  [Kiwi.HardwareEntryPoint()]
  public static void Main() 
  {
    // CordicWithIntegerAngle.Table_Generate();
    bool verbosef = true;
    if (verbosef) Console.WriteLine("cordic: Testbench start");

    double t0 = 0.244978663126864;
    t0 = 0.785398163397448;
    t0 = 0.1;
    t0 *= 1.001;
    double resulta = CordicWithIntegerAngle.sin(t0);
    Console.WriteLine("t0 {0}  Sin={1}  cordic={2}", t0, Math.Sin(t0), resulta);
//    return;
    int main_result;
    main_result = 0;

    Console.WriteLine("Please return NaN outside supported range");
    for (int i = 1; i < N; i++)
      {
	if (false && i != 10) continue;
	float64 result;
	Kiwi.Pause();
	result = from_double(CordicWithIntegerAngle.sin(to_double(test_in[i])));
	main_result += (result == test_out[i])?1:0;
	if (verbosef)
	  {
	    Console.WriteLine("Test: input={0:X} expected={1:X} output={2:X} ", test_in[i], test_out[i], result);
	    if ((result ^ test_out[i]) != 0) Console.WriteLine("   test {0} hamming error={1:X}", i, result ^ test_out[i]);
	  }
      }
    Console.WriteLine ("Result: {0}/{1}", main_result, N);
    if (verbosef) Console.WriteLine("cordic: Testbench finished");
    done = true;
    Kiwi.Pause();
    Kiwi.Pause();
  }
}


// (* eof *)

