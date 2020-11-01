using System;

/*



// CSharp form. - Converted manually to CSharp as a Kiwi experiment.


+--------------------------------------------------------------------------+
| CHStone : a suite of benchmark programs for C-based High-Level Synthesis |
| ======================================================================== |
|                                                                          |
| * Collected and Modified : Y. Hara, H. Tomiyama, S. Honda,               |
|                            H. Takada and K. Ishii                        |
|                            Nagoya University, Japan                      |
|                                                                          |
| * Remark :                                                               |
|    1. This source code is modified to unify the formats of the benchmark |
|       programs in CHStone.                                               |
|    2. Test vectors are added for CHStone.                                |
|    3. If "main_result" is 0 at the end of the program, the program is    |
|       correctly executed.                                                |
|    4. Please follow the copyright of each benchmark program.             |
+--------------------------------------------------------------------------+
*/
/*
 * Copyright (C) 2008
 * Y. Hara, H. Tomiyama, S. Honda, H. Takada and K. Ishii
 * Nagoya University, Japan
 * All rights reserved.
 *
 * Disclaimer of Warranty
 *
 * These software programs are available to the user without any license fee or
 * royalty on an "as is" basis. The authors disclaims any and all warranties, 
 * whether express, implied, or statuary, including any implied warranties or 
 * merchantability or of fitness for a particular purpose. In no event shall the
 * copyright-holder be liable for any incidental, punitive, or consequential damages
 * of any kind whatsoever arising from the use of these programs. This disclaimer
 * of warranty extends to the user of these programs and user's customers, employees,
 * agents, transferees, successors, and assigns.
 *
 */

using float64 = System.UInt64;

public class softfloat
{

  const bool verbositious = false;

  enum float_round_t  {
    float_round_nearest_even,
    float_round_to_zero,
    float_round_up,
    float_round_down,
    float_round_nearest_to_zero,
    float_round_nearest_to_up,
    float_round_nearest_to_down
  };

  const float_round_t float_rounding_mode = 0;
  static byte float_exception_flags = 0;
  const byte float_flag_invalid = 1;
  const byte float_flag_underflow = 2;
  const byte float_flag_divbyzero = 4;
  const byte float_flag_overflow = 8;
  const byte float_flag_inexact = 16;

  const int float_detect_tininess = 0; // Should be an enum.
  const int float_tininess_before_rounding = 0;

  static void raise(string arg)
  {
    Console.WriteLine("Raise {0} ", arg);
  }


/*----------------------------------------------------------------------------
| Shifts `a' right by the number of bits given in `count'.  If any nonzero
| bits are shifted off, they are ``jammed'' into the least significant bit of
| the result by setting the least significant bit to 1.  The value of `count'
| can be arbitrarily large; in particular, if `count' is greater than 64, the
| result will be either 0 or 1, depending on whether `a' is zero or nonzero.
| The result is stored in the location pointed to by `zPtr'.
*----------------------------------------------------------------------------*/

  public static  void shift64RightJamming (ulong a, short count, out ulong zPtr)
  {
    ulong z;
    
    if (count == 0)
      {
	z = a;
      }
    else if (count < 64)
      {
	z = (a >> count) | (((a << ((-count) & 63)) != 0)? 1uLL:0);
      }
    else
      {
	z = (a != 0uLL) ?1uLL:0;
      }
    zPtr = z;
  }
  
  /*----------------------------------------------------------------------------
    | Shifts the 128-bit value formed by concatenating `a0' and `a1' right by 64
    | _plus_ the number of bits given in `count'.  The shifted result is at most
    | 64 nonzero bits; this is stored at the location pointed to by `z0Ptr'.  The
    | bits shifted off form a second 64-bit result as follows:  The _last_ bit
    | shifted off is the most-significant bit of the extra result, and the other
    | 63 bits of the extra result are all zero if and only if _all_but_the_last_
    | bits shifted off were all zero.  This extra result is stored in the location
    | pointed to by `z1Ptr'.  The value of `count' can be arbitrarily large.
    |     (This routine makes more sense if `a0' and `a1' are considered to form
    | a fixed-point value with binary point between `a0' and `a1'.  This fixed-
    | point value is shifted right by the number of bits given in `count', and
    | the integer part of the result is returned at the location pointed to by
    | `z0Ptr'.  The fractional part of the result may be slightly corrupted as
    | described above, and is returned at the location pointed to by `z1Ptr'.)
    *----------------------------------------------------------------------------*/
  
  public static  void shift64ExtraRightJamming (ulong a0, ulong a1, short count, out ulong z0Ptr, out ulong z1Ptr)
  {
    ulong z0, z1;
    byte negCount;
    negCount = (byte)((-count) & 63);
    
    if (count == 0)
      {
	z1 = a1;
	z0 = a0;
      }
    else if (count < 64)
      {
	z1 = (a0 << negCount) | ((a1 != 0) ? 1uLL:0);
	z0 = a0 >> count;
      }
    else
      {
	if (count == 64)
	  {
	    z1 = a0 | ((a1 != 0) ?1uLL:0);
	  }
	else
	  {
	    z1 = ((a0 | a1) != 0) ? 1:0uLL;
	  }
	z0 = 0;
      }
    z1Ptr = z1;
    z0Ptr = z0;
  }
  
  /*----------------------------------------------------------------------------
    | Adds the 128-bit value formed by concatenating `a0' and `a1' to the 128-bit
    | value formed by concatenating `b0' and `b1'.  Addition is modulo 2^128, so
    | any carry out is lost.  The result is broken into two 64-bit pieces which
    | are stored at the locations pointed to by `z0Ptr' and `z1Ptr'.
    *----------------------------------------------------------------------------*/
  public static void add128 (ulong a0, ulong a1, ulong b0, ulong b1, out ulong z0Ptr, out ulong z1Ptr)
  {
    ulong z1;
    z1 = a1 + b1;
    z1Ptr = z1;
    z0Ptr = a0 + b0 + ((z1 < a1)?1uLL:0);
  }

  /*----------------------------------------------------------------------------
  | Subtracts the 128-bit value formed by concatenating `b0' and `b1' from the
  | 128-bit value formed by concatenating `a0' and `a1'.  Subtraction is modulo
  | 2^128, so any borrow out (carry out) is lost.  The result is broken into two
  | 64-bit pieces which are stored at the locations pointed to by `z0Ptr' and
  | `z1Ptr'.
  *----------------------------------------------------------------------------*/

  public static  void sub128 (ulong a0, ulong a1, ulong b0, ulong b1, out ulong z0Ptr,
			      out ulong z1Ptr)
  {
    z1Ptr = a1 - b1;
    if (verbositious) Console.WriteLine("sub128 opstore start");
    z0Ptr = a0 - b0 - ((a1 < b1) ? 1uLL:0);
    if (verbositious) Console.WriteLine("sub128 opstore high res {0:X} - {1:X} -> {2:X}", a0, b0, z0Ptr);
  }

  /*----------------------------------------------------------------------------
    | Multiplies `a' by `b' to obtain a 128-bit product.  The product is broken
    | into two 64-bit pieces which are stored at the locations pointed to by
    | `z0Ptr' and `z1Ptr'.
    *----------------------------------------------------------------------------*/

  public static  void mul64To128 (ulong a, ulong b, out ulong z0Ptr, out ulong z1Ptr)
  {
    uint aHigh, aLow, bHigh, bLow;
    ulong z0, zMiddleA, zMiddleB, z1;
    
    aLow =  (uint)(a & 0xFFFFffff);
    aHigh = (uint)(a >> 32);
    bLow =  (uint)(b & 0xFFFFffff);
    bHigh = (uint)(b >> 32);
    z1 = ((ulong) aLow) * bLow;
    zMiddleA = ((ulong) aLow) * bHigh;
    zMiddleB = ((ulong) aHigh) * bLow;
    if (verbositious) Console.WriteLine("m64_128 z1       {0:X} <- {1:X} x {2:X}", z1, aLow, bLow);
    if (verbositious) Console.WriteLine("m64_128 zMiddleA {0:X} <- {1:X} x {2:X}", zMiddleA, aLow, bHigh);
    if (verbositious) Console.WriteLine("m64_128 zMiddleB {0:X} <- {1:X} x {2:X}", zMiddleB, aHigh, bLow);
    z0 = ((ulong) aHigh) * bHigh;
    if (verbositious) Console.WriteLine("aHigh    {0:X} <- cast {1:X} ", (ulong) aHigh, aHigh);
    if (verbositious) Console.WriteLine("z0       {0:X} <- {1:X} x {2:X}", z0, (ulong) aHigh, bHigh);
    if (verbositious) Console.WriteLine("z0 x      {0:X} ", z0);
    zMiddleA += zMiddleB;
    if (verbositious) Console.WriteLine("z0 y      {0:X}, zMiddleA2={0:X}", zMiddleA);
    ulong cbg_suspect = (zMiddleA < zMiddleB)?1uLL:0;
    if (verbositious) Console.WriteLine("cbg_suspect=0x{0:X}", cbg_suspect);
    z0 += (((zMiddleA < zMiddleB)?1uLL:0) << 32) + (zMiddleA >> 32);
    zMiddleA <<= 32;
    z1 += zMiddleA;
    z0 += (z1 < zMiddleA) ? 1uLL:0;
    if (verbositious) Console.WriteLine("z1,z0       0x{0:X} 0x{1:X}", z1, z0);
    z1Ptr = z1;
    z0Ptr = z0;
  }

  /*----------------------------------------------------------------------------
    | Returns an approximation to the 64-bit integer quotient obtained by dividing
    | `b' into the 128-bit value formed by concatenating `a0' and `a1'.  The
    | divisor `b' must be at least 2^63.  If q is the exact quotient truncated
    | toward zero, the approximation returned lies between q and q + 2 inclusive.
    | If the exact quotient q is larger than 64 bits, the maximum positive 64-bit
    | unsigned integer is returned.
    *----------------------------------------------------------------------------*/
  static ulong estimateDiv128To64 (ulong a0, ulong a1, ulong b)
  {
    ulong b0, b1;
    ulong rem0, rem1, term0, term1;
    ulong z;
    
    if (verbositious) Console.WriteLine("DIC DIV 238 a1={0} a0={1} b={2}", a1, a0, b);
    if (b <= a0)    return (0xFFFFFFFFFFFFFFFFuLL);
    b0 = b >> 32;
    z = (b0 << 32 <= a0) ? (0xFFFFFFFF00000000uLL) : (a0 / b0) << 32;
    if (verbositious) Console.WriteLine("DIC DIV 238 b0={0} z={1}", b0, z);
    mul64To128 (b, z, out term0, out term1);
    if (verbositious) Console.WriteLine("DIC DIV terms {0} {1}", term0, term1);

    sub128 (a0, a1, term0, term1, out rem0, out rem1);
    if (verbositious) Console.WriteLine("DIC DIV rems1 {0}-{1} = {2}", a1, term1, rem1);
    if (verbositious) Console.WriteLine("DIC DIV rems0 {0}-{1} = {2}", a0, term0, rem0);

    while (((long) rem0) < 0)
      {
        if (verbositious) Console.WriteLine("DIC DIV ITERATE {0}", rem0);
	z -= 0x100000000uLL;
	b1 = b << 32;
	add128 (rem0, rem1, b0, b1, out rem0, out rem1);
    }
    rem0 = (rem0 << 32) | (rem1 >> 32);
    z |= (b0 << 32 <= rem0) ? 0xFFFFFFFF : rem0 / b0;
    return z;
  }

/*----------------------------------------------------------------------------
| Returns the number of leading 0 bits before the most-significant 1 bit of
| `a'.  If `a' is zero, 32 is returned.
*----------------------------------------------------------------------------*/

   static byte [] countLeadingZerosHigh = {
      8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4,
      3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
      2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
      2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

  static byte countLeadingZeros32 (uint a_in)
  {
    uint a = a_in;
    byte shiftCount;

    shiftCount = 0;
    if (a < 0x10000)
      {
	shiftCount += 16;
	a <<= 16;
      }
    if (a < 0x1000000)
      {
	shiftCount += 8;
	a <<= 8;
      }
    shiftCount += countLeadingZerosHigh[a >> 24];
    return shiftCount;
  }

  /*----------------------------------------------------------------------------
    | Returns the number of leading 0 bits before the most-significant 1 bit of
    | `a'.  If `a' is zero, 64 is returned.
    *----------------------------------------------------------------------------*/
  
  static byte countLeadingZeros64 (ulong a_in)
  {
    ulong a = a_in;
    byte shiftCount;
    
    shiftCount = 0;
    if (a < ((ulong) 1) << 32)
      {
	shiftCount += 32;
      }
    else
      {
	a >>= 32;
      }
    shiftCount += countLeadingZeros32 ((uint)(a&0xFFFFffff));
    return shiftCount;
    
  }

  /*----------------------------------------------------------------------------
    | Returns the result of converting the 32-bit two's complement integer `a'
    | to the double-precision floating-point format.  The conversion is performed
    | according to the IEC/IEEE Standard for Binary Floating-Point Arithmetic.
    *----------------------------------------------------------------------------*/
  
  public static float64 int32_to_float64 (int a)
  {
    bool zSign;
    uint absA;
    ulong zSig;
    
    if (a == 0) return 0;
    zSign = (a < 0);
    absA = (uint)(zSign ? -a : a);
    int shiftCount = countLeadingZeros32 (absA) + 21;
    zSig = absA;
    return packFloat64 (zSign, (short)(0x432 - shiftCount), zSig << shiftCount);
  }


  /*----------------------------------------------------------------------------
    | Normalizes the subnormal double-precision floating-point value represented
    | by the denormalized significand `aSig'.  The normalized exponent and
    | significand are stored at the locations pointed to by `zExpPtr' and
    | `zSigPtr', respectively.
    *----------------------------------------------------------------------------*/
  
  static void normalizeFloat64Subnormal(ulong aSig, out short zExpPtr, out ulong  zSigPtr)
  {
    int shiftCount = countLeadingZeros64(aSig) - 11;
    zSigPtr = aSig << shiftCount;
    zExpPtr = (short)(1 - shiftCount);
  }


  /*----------------------------------------------------------------------------
    | Takes an abstract floating-point value having sign `zSign', exponent `zExp',
    | and significand `zSig', and returns the proper double-precision floating-
    | point value corresponding to the abstract input.  Ordinarily, the abstract
    | value is simply rounded and packed into the double-precision format, with
    | the inexact exception raised if the abstract input cannot be represented
    | exactly.  However, if the abstract value is too large, the overflow and
    | inexact exceptions are raised and an infinity or maximal finite value is
    | returned.  If the abstract value is too small, the input value is rounded
    | to a subnormal number, and the underflow and inexact exceptions are raised
    | if the abstract input cannot be represented exactly as a subnormal double-
    | precision floating-point number.
    |     The input significand `zSig' has its binary point between bits 62
    | and 61, which is 10 bits to the left of the usual location.  This shifted
    | significand must be normalized or smaller.  If `zSig' is not normalized,
    | `zExp' must be 0; in that case, the result returned is a subnormal number,
    | and it must not require rounding.  In the usual case that `zSig' is
    | normalized, `zExp' must be 1 less than the ``true'' floating-point exponent.
    | The handling of underflow and overflow follows the IEC/IEEE Standard for
    | Binary Floating-Point Arithmetic.
    *----------------------------------------------------------------------------*/
  static float64 roundAndPackFloat64(bool zSign, short zExp_in, ulong zSig_in)
  {
    short zExp = zExp_in;
    ulong zSig = zSig_in;  // KiwiC does not support modifiy of formal parameters (currently).
    float_round_t roundingMode;
    bool roundNearestEven, isTiny;
    short roundIncrement, roundBits;
    
    roundingMode = float_rounding_mode;
    roundNearestEven = (roundingMode == float_round_t.float_round_nearest_even);
    roundIncrement = 0x200;
    if (!roundNearestEven)
      {
	if (roundingMode == float_round_t.float_round_to_zero)
	  {
	    roundIncrement = 0;
	  }
	else
	  {
	    roundIncrement = 0x3FF;
	    if (zSign)
	      {
		if (roundingMode == float_round_t.float_round_up)
		  roundIncrement = 0;
	      }
	    else
	      {
		if (roundingMode == float_round_t.float_round_down)
		  roundIncrement = 0;
	      }
	  }
      }
    roundBits = (short)(zSig & 0x3FF);
    if (verbositious) Console.WriteLine("  round bits {0:X}  & 3FF -> {1:X}", zSig, roundBits);
    if (0x7FD <= (short) zExp)
      {
        if (verbositious) Console.WriteLine("xr1 r={0}", roundBits);
	if ((0x7FD < zExp) || ((zExp == 0x7FD) && ((zSig + (ulong)roundIncrement) < 0)))

	  {
            if (verbositious) Console.WriteLine("xr2 r={0}", roundBits);
	    float_raise (float_flag_overflow | float_flag_inexact);
	    return packFloat64 (zSign, 0x7FF, 0) - (roundIncrement == 0?1uLL:0);
	  }
	if (zExp < 0)
	{
          if (verbositious) Console.WriteLine("xr3 r={0}", roundBits);
	  isTiny = (float_detect_tininess == float_tininess_before_rounding)
	    || (zExp < -1)
	    || (zSig + (ulong)roundIncrement < 0x8000000000000000uLL);
	  shift64RightJamming (zSig, (short)-zExp, out zSig);
	  zExp = 0;
	  roundBits = (short)(zSig & 0x3FF);
	  if (isTiny && (roundBits != 0))
	    float_raise (float_flag_underflow);
	}
      }
    if (roundBits != 0) float_exception_flags |= float_flag_inexact;
    zSig = (zSig + (ulong)roundIncrement) >> 10;
    if (((roundBits ^ 0x200) == 0) & roundNearestEven) zSig = (ulong)((long)zSig & -2LL);
    if (zSig == 0) zExp = 0;
    if (verbositious) Console.WriteLine("xr4 r={0}", roundBits);
    if (verbositious) Console.WriteLine("packFloat64 args zExp=0x{0:X} zSig={1:X}", zExp, zSig);
    return packFloat64 (zSign, zExp, zSig);
  }


/*----------------------------------------------------------------------------
| Takes an abstract floating-point value having sign `zSign', exponent `zExp',
| and significand `zSig', and returns the proper double-precision floating-
| point value corresponding to the abstract input.  Ordinarily, the abstract
| value is simply rounded and packed into the double-precision format, with
| the inexact exception raised if the abstract input cannot be represented
| exactly.  However, if the abstract value is too large, the overflow and
| inexact exceptions are raised and an infinity or maximal finite value is
| returned.  If the abstract value is too small, the input value is rounded
| to a subnormal number, and the underflow and inexact exceptions are raised
| if the abstract input cannot be represented exactly as a subnormal double-
| precision floating-point number.
|     The input significand `zSig' has its binary point between bits 62
| and 61, which is 10 bits to the left of the usual location.  This shifted
| significand must be normalized or smaller.  If `zSig' is not normalized,
| `zExp' must be 0; in that case, the result returned is a subnormal number,
| and it must not require rounding.  In the usual case that `zSig' is
| normalized, `zExp' must be 1 less than the ``true'' floating-point exponent.
| The handling of underflow and overflow follows the IEC/IEEE Standard for
| Binary Floating-Point Arithmetic.
*----------------------------------------------------------------------------*/

  static float64 normalizeRoundAndPackFloat64 (bool zSign, short zExp, ulong zSig)
  {
    int shiftCount;
    shiftCount = countLeadingZeros64 (zSig) - 1;
    return roundAndPackFloat64 (zSign, (short)(zExp - shiftCount), zSig << shiftCount);

  }
  
  static float64 packFloat64 (bool zSign, short zExp, ulong zSig)
  {
    return ((zSign ?1uLL:0) << 63) + (((ulong) zExp) << 52) + zSig;
  }
  
  public static ulong extractFloat64Frac (float64 a)
  {
    return a & (0x000FFFFFFFFFFFFFULL);
  }

  public static short extractFloat64Exp (float64 a)
  {
    return (short)((a >> 52) & 0x7FFULL);
  }

  public static bool extractFloat64Sign (float64 a)
  {
    return (a >> 63) > 0 ? true:false;
  }

  /*----------------------------------------------------------------------------
    | Raises the exceptions specified by `flags'.  Floating-point traps can be
    | defined here if desired.  It is currently not possible for such a trap
    | to substitute a result value.  If traps are not implemented, this routine
    | should be simply `float_exception_flags |= flags;'.
    *----------------------------------------------------------------------------*/


  
  static void float_raise (byte flags)
  {
    float_exception_flags |= flags;
  }

  /*----------------------------------------------------------------------------
    | The pattern for a default generated double-precision NaN.
    *----------------------------------------------------------------------------*/
  const float64 float64_default_nan = ( 0x7FFFFFFFFFFFFFFFULL );
    
    /*----------------------------------------------------------------------------
      | Returns 1 if the double-precision floating-point value `a' is a NaN;
      | otherwise returns 0.
      *----------------------------------------------------------------------------*/
    
  public static bool float64_is_nan (float64 a)
  {
    return ((0xFFE0000000000000ULL) < (a << 1));
  }
  
  /*----------------------------------------------------------------------------
    | Returns 1 if the double-precision floating-point value `a' is a signaling
    | NaN; otherwise returns 0.
    *----------------------------------------------------------------------------*/
  public static bool float64_is_signaling_nan (float64 a)
  {
    return (((a >> 51) & 0xFFF) == 0xFFE) && ((a & (0x0007FFFFFFFFFFFFULL)) != 0);
  }

/*----------------------------------------------------------------------------
| Takes two double-precision floating-point values `a' and `b', one of which
| is a NaN, and returns the appropriate NaN result.  If either `a' or `b' is a
| signaling NaN, the invalid exception is raised.
*----------------------------------------------------------------------------*/

static float64 propagateFloat64NaN (float64 a_in, float64 b_in)
{
  float64 a = a_in; float64 b = b_in;
  bool aIsSignalingNaN, bIsNaN, bIsSignalingNaN;
  //bool aIsNaN = float64_is_nan (a);
  aIsSignalingNaN = float64_is_signaling_nan (a);
  bIsNaN = float64_is_nan (b);
  bIsSignalingNaN = float64_is_signaling_nan (b);
  a |= (0x0008000000000000uLL);
  b |= (0x0008000000000000uLL);
  if (aIsSignalingNaN | bIsSignalingNaN)
    float_raise (float_flag_invalid);
  return bIsSignalingNaN ? b : aIsSignalingNaN ? a : bIsNaN ? b : a;

}

  
  public static bool float64_ge (float64 a, float64 b)
  {
    return float64_le (b, a);
  }
  
  // added by hiroyuki@acm.org
  public static float64 float64_neg (float64 x)
  {
    float64 ans = (((~x) & 0x8000000000000000ULL) | (x & 0x7fffffffffffffffULL));
    if (verbositious) Console.WriteLine("negation hex {0:X} gives {1:X}", x, ans);
    if (verbositious) Console.WriteLine("negation dec {0} gives {1}", x, ans);
    return ans;
  }
  

  public static float64 float64_abs (float64 x)
  {
    return (x & 0x7fffffffffffffffLL);
  }


  public static float64 float64_add (float64 a, float64 b)
  {
    bool aSign, bSign;
    
    aSign = extractFloat64Sign (a);
    bSign = extractFloat64Sign (b);
    if (aSign == bSign)
      return addFloat64Sigs (a, b, aSign);
    else
      return subFloat64Sigs (a, b, aSign);

  }


/*----------------------------------------------------------------------------
| Returns the result of adding the absolute values of the double-precision
| floating-point values `a' and `b'.  If `zSign' is 1, the sum is negated
| before being returned.  `zSign' is ignored if the result is a NaN.
| The addition is performed according to the IEC/IEEE Standard for Binary
| Floating-Point Arithmetic.
*----------------------------------------------------------------------------*/

static float64
addFloat64Sigs (float64 a, float64 b, bool zSign)
{
  short aExp, bExp, zExp;
  ulong aSig, bSig, zSig;
  int expDiff;

  aSig = extractFloat64Frac (a);
  aExp = extractFloat64Exp (a);
  bSig = extractFloat64Frac (b);
  bExp = extractFloat64Exp (b);
  expDiff = aExp - bExp;
  aSig <<= 9;
  bSig <<= 9;
  if (0 < expDiff)
    {
      if (aExp == 0x7FF)
	{
	  if (aSig != 0)
	    return propagateFloat64NaN (a, b);
	  return a;
	}
      if (bExp == 0)
	--expDiff;
      else
	bSig |= (0x2000000000000000uLL);
      shift64RightJamming (bSig, (short)expDiff, out bSig);
      zExp = aExp;
    }
  else if (expDiff < 0)
    {
      if (bExp == 0x7FF)
	{
	  if (bSig != 0)
	    return propagateFloat64NaN (a, b);
	  return packFloat64 (zSign, 0x7FF, 0);
	}
      if (aExp == 0)
	++expDiff;
      else
	{
	  aSig |= (0x2000000000000000uLL);
	}
      shift64RightJamming (aSig, (short)-expDiff, out aSig);
      zExp = bExp;
    }
  else
    {
      if (aExp == 0x7FF)
	{
	  if (aSig != 0 || bSig != 0)
	    return propagateFloat64NaN (a, b);
	  return a;
	}
      if (aExp == 0)
	return packFloat64 (zSign, 0, (aSig + bSig) >> 9);
      zSig = (0x4000000000000000uLL) + aSig + bSig;
      zExp = aExp;
      goto roundAndPack;
    }
  aSig |= (0x2000000000000000uLL);
  zSig = (aSig + bSig) << 1;
  --zExp;
  if ((long) zSig < 0)
    {
      zSig = aSig + bSig;
      ++zExp;
    }
roundAndPack:
  return roundAndPackFloat64 (zSign, zExp, zSig);

}

/*----------------------------------------------------------------------------
| Returns the result of subtracting the absolute values of the double-
| precision floating-point values `a' and `b'.  If `zSign' is 1, the
| difference is negated before being returned.  `zSign' is ignored if the
| result is a NaN.  The subtraction is performed according to the IEC/IEEE
| Standard for Binary Floating-Point Arithmetic.
*----------------------------------------------------------------------------*/

  public static float64 subFloat64Sigs (float64 a, float64 b, bool zSign_in)
  {
    bool zSign = zSign_in;
    short aExp, bExp, zExp;
    ulong aSig, bSig, zSig;
    int expDiff;
    
    aSig = extractFloat64Frac (a);
    aExp = extractFloat64Exp (a);
    bSig = extractFloat64Frac (b);
    bExp = extractFloat64Exp (b);
    expDiff = aExp - bExp;
    aSig <<= 10;
    bSig <<= 10;
    if (0 < expDiff)
      goto aExpBigger;
    if (expDiff < 0)
      goto bExpBigger;
    if (aExp == 0x7FF)
      {
	if (aSig != 0 || bSig != 0) return propagateFloat64NaN (a, b);
	float_raise (float_flag_invalid);
	return float64_default_nan;
      }
    if (aExp == 0)
      {
	aExp = 1;
	bExp = 1;
      }
    if (bSig < aSig)
      goto aBigger;
    if (aSig < bSig)
      goto bBigger;
    return packFloat64 (float_rounding_mode == float_round_t.float_round_down, 0, 0);

    bExpBigger:
    if (bExp == 0x7FF)
      {
	if (bSig != 0)
	  return propagateFloat64NaN (a, b);
	return packFloat64 (!zSign, 0x7FF, 0);
      }
    if (aExp == 0)
      ++expDiff;
    else
      aSig |= 0x4000000000000000ULL;
    shift64RightJamming (aSig, (short)-expDiff, out aSig);
    bSig |= 0x4000000000000000ULL;
    bBigger:
    zSig = bSig - aSig;
    zExp = bExp;
    zSign = !zSign;
    goto normalizeRoundAndPack;
    aExpBigger:
    if (aExp == 0x7FF)
      {
	if (aSig != 0)
	  return propagateFloat64NaN (a, b);
	return a;
      }
    if (bExp == 0)
      --expDiff;
    else
      bSig |= 0x4000000000000000ULL;
    shift64RightJamming (bSig, (short)expDiff, out bSig);
    aSig |= 0x4000000000000000ULL;
    aBigger:
    zSig = aSig - bSig;
    zExp = aExp;
    normalizeRoundAndPack:
    --zExp;
    return normalizeRoundAndPackFloat64 (zSign, zExp, zSig);
    
  }

  /*----------------------------------------------------------------------------
    | Returns the result of multiplying the double-precision floating-point values
    | `a' and `b'.  The operation is performed according to the IEC/IEEE Standard
    | for Binary Floating-Point Arithmetic.
    *----------------------------------------------------------------------------*/
  
  public static float64 float64_mul (float64 a, float64 b)
  {
    bool aSign, bSign, zSign;
    short aExp, bExp, zExp;
    ulong aSig, bSig, zSig0=0, zSig1=0;
    
    aSig = extractFloat64Frac (a);
    aExp = extractFloat64Exp (a);
    aSign = extractFloat64Sign (a);
    bSig = extractFloat64Frac (b);
    bExp = extractFloat64Exp (b);
    bSign = extractFloat64Sign (b);
    zSign = aSign ^ bSign;
    if (aExp == 0x7FF)
      {
	if ((aSig != 0) || ((bExp == 0x7FF) && (bSig != 0)))
	  return propagateFloat64NaN (a, b);
	if ((bExp == 0) && (bSig == 0))
	  {
	    float_raise (float_flag_invalid);
	    return float64_default_nan;
	  }
	return packFloat64 (zSign, 0x7FF, 0);
      }
    if (bExp == 0x7FF)
      {
	if (bSig != 0)
	  return propagateFloat64NaN (a, b);
	if ((aExp == 0) && (aSig == 0))
	  {
	    float_raise (float_flag_invalid);
	    return float64_default_nan;
	  }
	return packFloat64 (zSign, 0x7FF, 0);
      }
    if (aExp == 0)
      {
	if (aSig == 0)
	  return packFloat64 (zSign, 0, 0);
	normalizeFloat64Subnormal (aSig, out aExp, out aSig);
      }
    if (bExp == 0)
      {
        if (verbositious) Console.WriteLine("Point 799");
	if (bSig == 0)
	  return packFloat64 (zSign, 0, 0);
        if (verbositious) Console.WriteLine("Point 801");
	normalizeFloat64Subnormal (bSig, out bExp, out bSig);
      }
    zExp = (short)(aExp + bExp - 0x3FF);
    aSig = (aSig | (0x0010000000000000ULL)) << 10;
    bSig = (bSig | (0x0010000000000000ULL)) << 11;
    if (verbositious) Console.WriteLine("Point 806");
    mul64To128 (aSig, bSig, out zSig0, out zSig1);
    zSig0 |= (zSig1 != 0) ? 1uLL:0;
    if (verbositious) Console.WriteLine(" before round {0:X}, as cast {1:X} {1}", zSig0, (long) (zSig0 << 1));
    if (0 <= (long) (zSig0 << 1))
      {
        if (verbositious) Console.WriteLine("ShiftingLeft-sSig0 top={0:X}", (zSig0 >> (64-8)));
	zSig0 <<= 1;
	--zExp;
      }
    return roundAndPackFloat64 (zSign, zExp, zSig0);
 }

/*----------------------------------------------------------------------------
| Returns the result of dividing the double-precision floating-point value `a'
| by the corresponding value `b'.  The operation is performed according to
| the IEC/IEEE Standard for Binary Floating-Point Arithmetic.
*----------------------------------------------------------------------------*/

  public static float64 float64_div (float64 a, float64 b)
  {
    bool aSign, bSign, zSign;
    short aExp, bExp, zExp;
    ulong aSig, bSig, zSig;
    ulong rem0, rem1, term0, term1;
    
    aSig = extractFloat64Frac (a);
    aExp = extractFloat64Exp (a);
    aSign = extractFloat64Sign (a);
    bSig = extractFloat64Frac (b);
    bExp = extractFloat64Exp (b);
    bSign = extractFloat64Sign (b);
    zSign = aSign ^ bSign;
    if (aExp == 0x7FF)
      {
	if (aSig != 0)
	  return propagateFloat64NaN (a, b);
	if (bExp == 0x7FF)
	  {
	    if (bSig != 0)
	      return propagateFloat64NaN (a, b);
	    float_raise (float_flag_invalid);
	    return float64_default_nan;
	  }
	return packFloat64 (zSign, 0x7FF, 0);
      }
    if (bExp == 0x7FF)
      {
	if (bSig != 0)
	  return propagateFloat64NaN (a, b);
	return packFloat64 (zSign, 0, 0);
      }
    if (bExp == 0)
      {
	if (bSig == 0)
	  {
	    if ((aExp ==0 && (aSig) == 0))
	      {
		float_raise (float_flag_invalid);
		return float64_default_nan;
	      }
	    float_raise (float_flag_divbyzero);
	    return packFloat64 (zSign, 0x7FF, 0);
	  }
	normalizeFloat64Subnormal (bSig, out bExp, out bSig);
      }
    if (aExp == 0)
      {
	if (aSig == 0)
	  return packFloat64 (zSign, 0, 0);
	normalizeFloat64Subnormal (aSig, out aExp, out aSig);
      }
    zExp = (short)(aExp - bExp + 0x3FD);
    aSig = (aSig | (0x0010000000000000uLL)) << 10;
    bSig = (bSig | (0x0010000000000000uLL)) << 11;
    if (bSig <= (aSig + aSig))
      {
	aSig >>= 1;
	++zExp;
      }
    zSig = estimateDiv128To64 (aSig, 0, bSig);
    if (verbositious) Console.WriteLine("LX 894 {0}", zSig);
    if ((zSig & 0x1FF) <= 2)
      {
        if (verbositious) Console.WriteLine("LX 895");
	mul64To128 (bSig, zSig, out term0, out term1);
	sub128 (aSig, 0, term0, term1, out rem0, out rem1);
	while ((long) rem0 < 0)
	  {
	    --zSig;
	    add128 (rem0, rem1, 0, bSig, out rem0, out rem1);
	  }
	zSig |= (rem1 != 0) ? 1uLL:0;
      }
    return roundAndPackFloat64 (zSign, zExp, zSig);
  }

  /*----------------------------------------------------------------------------
    | Returns 1 if the double-precision floating-point value `a' is less than or
    | equal to the corresponding value `b', and 0 otherwise.  The comparison is
    | performed according to the IEC/IEEE Standard for Binary Floating-Point
    | Arithmetic.
    *----------------------------------------------------------------------------*/

  public static bool float64_le(float64 a, float64 b)
  {
    bool aSign, bSign;
    
    if (((extractFloat64Exp (a) == 0x7FF) && (extractFloat64Frac (a) != 0))
	|| ((extractFloat64Exp (b) == 0x7FF) && (extractFloat64Frac (b) != 0)))
      {
	float_raise (float_flag_invalid);
	return false;
      }
    aSign = extractFloat64Sign (a);
    bSign = extractFloat64Sign (b);
    if (aSign != bSign)
      return aSign || ((ulong) ((a | b) << 1) == 0);
    return (a == b) || (aSign ^ (a < b));
    
  }
}

// eof
