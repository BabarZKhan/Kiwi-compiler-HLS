// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// (C) 2016 DJ Greaves
//
// BitConverter Tests - using explict Kiwi.Dispose at the moment but these simple cases are automated in theory by kiwic-autodispose...

// Also there is a bug in restructure2 that temporarily needs the additional manifest Pause calls
//
//


using System;
using KiwiSystem;

public class BitConverterTest
{
  static int RoundTripTest_Double(double x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    double nv = BitConverter.ToDouble(ddata, 0);
    Console.WriteLine("Sum after Double {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_Single(float x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    float nv = BitConverter.ToSingle(ddata, 0);
    Console.WriteLine("Sum after Single {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_Int64(Int64 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    long nv = BitConverter.ToInt64(ddata, 0);
    Console.WriteLine("Sum after Int64 {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_Int32(Int32 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    Int32 nv = BitConverter.ToInt32(ddata, 0);
    Console.WriteLine("Sum after Int32 {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_Int16(Int16 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    Int16 nv = BitConverter.ToInt16(ddata, 0);
    Console.WriteLine("Sum after Int16 {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_UInt64(int testno, UInt64 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("  {1} {0:x}", vv, testno);
    ulong nv = BitConverter.ToUInt64(ddata, 0);
    Console.WriteLine("Sum after UInt64 {0} sum={1} reconverted={2}", x, sum, nv);
    Console.WriteLine("Tnow {0}", Kiwi.tnow);
    //Kiwi.Dispose(ddata);
    return sum;
  }

  static int RoundTripTest_UInt32(UInt32 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    UInt32 nv = BitConverter.ToUInt32(ddata, 0);
    Console.WriteLine("Sum after UInt32 {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }


  static int RoundTripTest_UInt16(UInt16 x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
// Owing to a KiwiBug, calling this function twice when it contains a foreach loop does not compile. May 2016. Please fix.
//    foreach (byte vv in ddata) sum += vv;
//    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    UInt16 nv = BitConverter.ToUInt16(ddata, 0);
    Console.WriteLine("Sum after UInt16 {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

 // Others

  static int RoundTripTest_Char(Char x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    Char nv = BitConverter.ToChar(ddata, 0);
    Console.WriteLine("Sum after Char {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }


  static int RoundTripTest_Boolean(Boolean x)
  {
    int sum = 0;
    byte [] ddata = BitConverter.GetBytes(x);
    foreach (byte vv in ddata) sum += vv;
    foreach (byte vv in ddata)  Console.WriteLine("   {0:x}", vv);
    Boolean nv = BitConverter.ToBoolean(ddata, 0);
    Console.WriteLine("Sum after Boolean {0} sum={1} reconverted={2}", x, sum, nv);
    //Kiwi.Dispose(ddata);
    return sum;
  }

   static void perhaps_pause()
   {
     Kiwi.Pause();
   }

   static double [] ctests = new double [4];

   static void FloatConvTest0()
   {
     Kiwi.KppMark(1, "FloatConvert0");
      double pl = 12345.6789;
      for (int i=0; i<ctests.Length; i++) 
      {
         ctests[i] = (double)pl;
	 pl = pl * 12;
	 Kiwi.Pause();
      }
      Kiwi.Pause();
      for (int i=0; i<ctests.Length; i++) 
      {
         Console.WriteLine("FloatConvTest0: {0}  has {1:F}  ", i, ctests[i]);
      }


   }


   static void FloatConvTest1()
   {
     Kiwi.KppMark(2, "FloatConvert1");
      double bigv = 1.2345e30;
      for (int i=0; i<2; i++) 
      {
         Kiwi.Pause();
	 float floater = (float)(bigv);  // Test overflow error on convert.
         Console.WriteLine("FloatConvTest1:{0}  becomes {1}", bigv, floater);
	 bigv = bigv * 100.0;
     } 
   }
  


  static int BitConvTest()
  {
    Console.WriteLine("Bit Convertor Test");
    Console.WriteLine("IsLittleEndian={0}\n", System.BitConverter.IsLittleEndian);
    Kiwi.KppMark(3, "BitConvTest");

    const int tm = 0x40; // This should be all tests

    for (int qq=0;qq<4; qq++)
    {
       Kiwi.Pause(); // <<----------------- Restructure bug - this pause critically needed at the moment (Being fixed Dec 2016)
       int checkSum = 0;
           
       if ((tm & 0x8) > 0)
         {
           checkSum += RoundTripTest_Double(1.2345678);
           perhaps_pause();
         }

       if ((tm & 0x10) > 0)
         {
           checkSum += RoundTripTest_Single(1.2345678f);
           perhaps_pause();
         }
       
       if ((tm & 0x20) > 0)
         {
           checkSum += RoundTripTest_Int64(-1234567891+qq);
           perhaps_pause();
         }

           
       if ((tm & 0x40) > 0)
         {
           checkSum += RoundTripTest_Int32(-1234567891+qq);
           perhaps_pause();
         }


       if ((tm & 0x80) > 0)
         {
           
           checkSum += RoundTripTest_Int16((short)(-1234+qq));
           perhaps_pause();
         }

       if ((tm & 0x100) > 0)
         {
           checkSum += RoundTripTest_Char('*');
           perhaps_pause();
         }

       if ((tm & 0x200) > 0)
         {// 0x00000000499602d3 =  1234567891
           checkSum += RoundTripTest_UInt64(qq, 1234567891+(ulong)qq);
           perhaps_pause();
         }

/*

// May 2016 - enabling the other tests in here causes a heap/repack unpleasant KiwiC error (to do with overloaded functions) that must be fixed soon.
       checkSum += RoundTripTest_UInt32(12345678);
       perhaps_pause();

       checkSum += RoundTripTest_UInt16((UInt16)(10 + qq));
       perhaps_pause();
*/

         

       if ((tm & 0x400) > 0)
       {
         checkSum += RoundTripTest_UInt16((UInt16)(256 + 512 + qq));
         perhaps_pause();
       }
       
       if ((tm & 0x800) > 0)
         {
           checkSum += RoundTripTest_Boolean(true);
           perhaps_pause();
         }

       Console.WriteLine("Test45 iteration {0}: checksum = {1}", qq, checkSum);
     }
    return 0;
  }


  static void FloatLoopTest()
  {
    Kiwi.KppMark(4, "FloatLoop");
    for (int qw=0;qw<3; qw++)
      {
        Kiwi.Pause(); // <<----------------- Restructure bug - this pause critically needed at the moment (Being fixed Dec 2016)

        Kiwi.KppMark(5, "LoopTop");
        int [] testd = new int [8];
        Kiwi.Pause();
	Console.WriteLine("  {0}  {1}",  qw, qw);	
        Kiwi.KppMark(6, "LoopBot");
	//Kiwi.Dispose(testd);
      }
  }


  [Kiwi.HardwareEntryPoint()]
  static void Main()
  {
    Console.WriteLine("Hello from Test45");
    Kiwi.Pause();
    Kiwi.KppMark(7, "HeadMark");

    
    FloatConvTest0();

    FloatConvTest1();

    FloatLoopTest();
    
    BitConvTest();
    Console.WriteLine("Test45 finished.");
  }


  static int MainOld()
  {
    Console.WriteLine("Hello from Test45 - A\n");

    if (false) for (int qw=0;qw<4; qw++)
      {
        Kiwi.KppMark(8, "LoopTop");
        int [] testd = new int [8];
        Kiwi.Pause();
	Console.WriteLine("  {0}  {1}",  qw, qw);	
        Kiwi.KppMark(9, "LoopBot");
	//Kiwi.Dispose(testd);
      }
    
    ulong inn = 1025 +0xFFF000000;
    byte [] outt = BitConverter.GetBytes(inn);
    
    Console.WriteLine("{0} -> {1} ans",  inn, outt); // lsbyte in location 0 of the array
    for (int qq=0;qq<outt.Length; qq++)
      {
	Console.WriteLine("  {0}  {1}",  qq, outt[qq]);	
      }

    return 0;
  }
}


// eof
