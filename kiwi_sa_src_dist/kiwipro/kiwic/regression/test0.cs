// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// Test0 - J Shipton Emu Tests - integer casts and masks.

using System;
using KiwiSystem;


// st1: Wrong negate operator.
class test
{
  
  static int payloadOffset1;
  static ulong data;

  static void st0()
  {
    /* 
       A problem can arise with  "UInt32 value = ((UInt32)key << 24);" where key is a byte.
       Unlike C/C++, Verilog does not have integer promotion. The rhs operand to a shift, being 32 bits wide does not make the expression compute in 32 bits.  The left shifting a byte by 24 bits makes it zero in Verilog semantics.

       A related problem is demonstrated with operators that can cause 'carries' such as addition, multiplication, subtraction and negate.

       Bad output from test0 in the old days was:
        st0 byte left shift: 44 00000000
        st0 byte left shift: 45 00000000
        st0 byte arith 0: summ=250  prod=0
        st0 byte arith 1: summ=255  prod=236
        st0 byte arith 2: summ=4  prod=224
        st0 byte arith 3: summ=9  prod=220

   */
    byte pango = 0x44;
    for (int qq=0;qq<2;qq++)
      {
        UInt32 ban = (UInt32)(pango << 24);
        Kiwi.Pause();
        Console.WriteLine(" st0 byte left shift: {0:X} {1:X}", pango, ban);
        Kiwi.setGpioLeds8(qq);
        pango += 1;
      }

    byte pov = 250, barr = 0;
    for (int qq=0;qq<4;qq++)
      {
        UInt32 summ = (UInt32)(pov + barr);
        UInt32 prod = (UInt32)(pov * barr);
        Kiwi.Pause();
        Console.WriteLine(" st0 byte arith {0}: summ={1}  prod={2}", qq, summ, prod);
        barr += 4;
        pov += 1;  // st1: Neither byte overflows in this test, but their sum is larger than 255 in C# but a naive Verilog transliteration would only do a byte addition
      }
  }

  static void st1()
  {
    ulong mask = ~(0xFFuL << payloadOffset1);
    Console.WriteLine("<><><>> mask: {0:X}", mask);
  }

// Pre bug fix : $display("<><><>> mask: %H", 64'hffffffffffffffff & !(64'hff << (6'd63 & ETET4EntryPoint_V_6)));
// Post bug fix: $display("<><><>> mask: %H", 64'hffffffffffffffff&~(64'hff<<(6'd63&test_payloadOffset1)));

  public static void st2()
  {
    ulong mask = ~0uL << (payloadOffset1 + 8) | ~0uL >> (64 - payloadOffset1);
    data &= mask;
    Console.WriteLine("<><><>> data: {0:X}", data);
  }

  public static void st3()
  {
    ulong a = 0x12345678uL;
    for (int pp=0;pp<2;pp++)
      {
	a = a+0x100000;
	Kiwi.Pause();
	uint v2 = (ushort)a;
	uint v3 = (uint)(a & 0xFFFF);
	Kiwi.Pause();
	Console.WriteLine("v2 {0:X}", v2);
	Console.WriteLine("v3 {0:X}", v3);
      }
  }


  public static void st4()
  {
    unchecked
      {
        /*
          //Disparity between C# and Kiwi generated Verilog - right shifts were sometimes missing a sign extension.
          mono produces
          xx  shr.uns ffff
          xx  shr      ffffffff
          yy  shr.uns ffff
          yy  shr      ffffffff

Correct RTL simulation output
xx  shr.uns ffffff
xx  shr      3
yy  shr.uns 00ffffff
yy  shr      ffffffff
yy  shr.uns 0000ffff
yy  shr      ffffffff
yy  shr.uns 000000ff
yy  shr      ffffffff
*/

        uint unsig = (uint)0xFFFFFFFF;
        int sig =     (int)0xFFFFFFFF;

        Console.WriteLine("preloop   shr.uns {0:x}", unsig >> 8); // shr.un
        Console.WriteLine("preloop   shr     {0:x}", sig >> 8); // shr
        Console.WriteLine();
        for (int qq=0;qq<3; qq++)
          {
            unsig >>= 8;
            sig >>= 8;
            Kiwi.Pause();
            Console.WriteLine("inloop  shr.uns {0:x}", unsig); // shr.un
            Console.WriteLine("        shr     {0:x}", sig); // shr
          }
      }
  }

  static int t1=32, t2=101;

  public static void st5()
  {

    int vv = (~t1) + (~t2);    
    Console.WriteLine("shipton test st5a: one's complement  {0} {1} --> {2}", t1, t2, vv);
    Kiwi.Pause();

    for (int k=0; k<2; k++)
      {
        vv = (~t1) + (~t2);    
    // A KiwiC regression made this display unsigned instead of : st5b: one's complement  32 101 --> -135, 64 50 --> -116, which is the correct signed output
    // vv is signed, so we should see -ve signs in the output.
    // correct:       $display("shipton test st5b: one's complement  %1d %1d --> %1d", test_t1, test_t2, $signed(~test_t1+~test_t2));
        Console.WriteLine("shipton test st5b: one's complement  {0} {1} --> {2}", t1, t2, vv);
        Kiwi.Pause();
        t1 = t1 << 1;
        t2 = t2 >> 1;
      }    
  }


  public static void st6()
  {
    Kiwi.Pause();
    /*

        64  : ldloc  V_1
        65  : ldc int32 .1
        66  : shl 
        67  : ldc int32 .0
        68  : conv.i8
        69  : bge LL84

This was producing         (Tsfl1_24_V_1<<32'sd1)<64'sh0)
instead of         ($signed(Tsfl1_24_V_1)<<32'sd1)<64'sh0);

The fact that it is a signed comparison is manifest in the bge instead of a bge_un cil instruction.

Correct output from st6 is:
dfsin ulong to long -ve kk=0 pp=FF000000 8556380160
dfsin ulong to long -ve kk=1 pp=FF0000000000 560750930165760
dfsin ulong to long -ve kk=2 pp=FF00000000000000 -144115188075855872
Went -ve
dfsin ulong to long -ve kk=3 pp=0 0
*/
    for (int kk=0; kk<4; kk++)
      {
        ulong pp = 0xFF000000LU << (kk*16);
        Console.WriteLine("dfsin ulong to long -ve kk={0} pp={1:X} {2}", kk, pp, (long)(pp<<1));
        Kiwi.Pause();
        if ((long)(pp<<1) < 0L) Console.WriteLine("Went -ve");
        Kiwi.Pause();        
      }    
  }


  public static void st7()
  {
    Kiwi.Pause();
    for (int kk=-2; kk<3; kk++)
      {
        Console.WriteLine("  st7 int abs test {0} {1}", kk, Math.Abs(kk));
        Console.WriteLine("               st7 int max test {0} {1}", kk, Math.Max(kk, 0));
        Kiwi.NoUnroll();
      }    


    double pp = -2.0;
    for (int kk=-2; kk<6; kk++)
      {
        pp += 1.1;
        Console.WriteLine("  st7 fp abs test {0} {1}", kk, Math.Abs(pp));
        Console.WriteLine("               st7 fp max test {0} {1}", kk, Math.Max(pp, 1.5));
        Kiwi.NoUnroll();
      }    

  }



  [Kiwi.OutputWordPort(63,0)] static long wout;
            
  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    data = 0xdeadbeef;
    payloadOffset1 = 8;

    wout = 32L << 32;


    Console.WriteLine("Test0 st0");    st0();
    Console.WriteLine("Test0 st1");    st1();
    Console.WriteLine("Test0 st2");    st2();
    Console.WriteLine("Test0 st3");    st3();
    Console.WriteLine("Test0 st4");    st4();
    Console.WriteLine("Test0 st5");    st5();
    Console.WriteLine("Test0 st6");    st6();
    Console.WriteLine("Test0 st7");    st7();
    Console.WriteLine("Test0 complete % %% %%% percent.");
  }
}

// eof
