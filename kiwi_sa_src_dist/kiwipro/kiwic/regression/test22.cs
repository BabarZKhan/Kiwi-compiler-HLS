// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// (C) 2009-16 DJ Greaves, University of Cambridge, Computer Laboratory.
//
// KiwiC Test 22 - varargs
// $Id: test22.cs,v 1.1 2011/04/29 13:35:09 djg11 Exp $
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;



class Program22
{
  [Kiwi.HardwareEntryPoint()]
  static void runtest()
  {
     Console.WriteLine("// KiwiC Test 22 - varargs - start");
     Kiwi.Pause();
           VarArgs(12);
           VarArgs(22, 23, 24);
           VarArgs(33, 34, 35, 45, 55, 65, 75);

     Console.WriteLine("// KiwiC Test 22 - varargs - done");
      //   Console.ReadKey(true); // Avoid exit under much-loved windows.
  }


  static int Main(string[] args)
  {
     runtest();
     return 0;
  }
 
  static void VarArgs(int val1, params int[] vals)
  {
     Console.WriteLine("  val1=: {0}", val1);
     foreach (int i in vals)
	{
           Console.Write("  vals={0}", i);
        }
     Console.WriteLine();
  }


}

// eof
