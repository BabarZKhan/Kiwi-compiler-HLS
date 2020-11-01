// Kiwi Scientific Acceleration
// Cordic implemented from Java. With testbench from CHstone dfsin.
// (C) 2014 DJ Greaves, University of Cambridge, Computer Laboratory.

// Note, cordic is normally used without any floating point (except at the start and end as needed), but our working angle here is floating instead of fixed in this unusual version.

using float64 = System.UInt64;
using System;
using KiwiSystem;

public class CordicWithSillyFloatingAngle 
{
  const int iterations = 32;
  static double [] ctab_new = new double [iterations];

  // This table generation is normally done offline, once and for all time.
  public static void Table_Generate() 
  {
    double theta = 1.0;
    for (int i=0; i<iterations; i++)
      {
	ctab_new[i] = Math.Atan(theta);
	Console.WriteLine("Cordic table {0} is arctan {1} = {2}", i, theta, ctab_new[i]);
	theta = theta / 2.0;
      }
  }


  static readonly double [] ctab = new [] { 
    0.785398163397448,
    0.463647609000806,
    0.244978663126864,
    0.124354994546761,
    0.0624188099959574,
    0.0312398334302683,
    0.0156237286204768,
    0.00781234106010111,
    0.00390623013196697,
    0.00195312251647882,
    0.000976562189559319,
    0.000488281211194898,
    0.000244140620149362,
    0.00012207031189367,
    6.10351561742088E-05,
    3.05175781155261E-05,
    1.52587890613158E-05,
    7.62939453110197E-06,
    3.8146972656065E-06,
    1.90734863281019E-06,
    9.53674316405961E-07,
    4.76837158203089E-07,
    2.38418579101558E-07,
    1.19209289550781E-07,
    5.96046447753906E-08,
    2.98023223876953E-08,
    1.49011611938477E-08,
    7.45058059692383E-09,
    3.72529029846191E-09,
    1.86264514923096E-09,
    9.31322574615479E-10,
    4.65661287307739E-10
  };

  // Here the angle is a floating-point number, but in reality we use fixed-point for some large, constant denominator, such as 10^9
  // or whatever suits the application to hand.
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


    int x=607252935 /*prescaled constant for iterations=32*/, y=0;
    // x = (int)(0.60725293500888 * 1e9);
    // Console.WriteLine("Starting vector value is x={0}", x);
    if (vd) Console.WriteLine("Range-reduced theta value is theta={0}", theta);
    for (int k=0; k<iterations; ++k)
      {
	int tx, ty;
	if (vd) Console.WriteLine("              {0} cf {1}   y={2}", theta, ctab[k], y);
	if (theta >= 0) // All nice simple integer arithmetic
	  {
            if (vd) Console.WriteLine(" k={0} ABOVE", k);
	    tx = x - (y>>k);
	    ty = y + (x>>k);
	    theta -= ctab[k];
	  }
	else
	  {
            if (vd) Console.WriteLine(" k={0} BELOW", k);
	    tx = x + (y>>k);
	    ty = y - (x>>k);
	    theta += ctab[k];
	  }
	x = tx; y = ty;
      }  

    double rv = (double)y;
    if (flip) rv = -rv;
    if (vd) Console.WriteLine(" ultimately {0}", rv);
    return rv; // Returns sine(arg) * 10^9.
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
    //  Table_Generate();
    bool verbosef = true;
    if (verbosef) Console.WriteLine("cordic: Testbench start");
    int main_result;
    int i;
    main_result = 0;
    for (i = 1; i < N; i++)
      {
	if (false && i != 10) continue;
	float64 result;
	Kiwi.Pause();
	result = from_double(1e-9 * CordicWithSillyFloatingAngle.sin(to_double(test_in[i])));
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
