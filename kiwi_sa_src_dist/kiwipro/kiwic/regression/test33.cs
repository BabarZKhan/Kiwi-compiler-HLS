// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// (C) 2009 DJ Greaves, University of Cambridge, Computer Laboratory.

// Test33: Complex type variable scopes.  - not finished.

using System;
using System.Text;
using System.Threading;
using KiwiSystem;

class test33
{

  class upper_street<UF1, UF2>
  {

     class lower_nested<LQ1>
     {

       UF1 my1;
       UF2 my2;
       LQ1 lower_fred;
     }

    lower_nested<byte> the_lower = new lower_nested<byte>();

    public UF1 doit()
    {
      return 101;
    }

  }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test33 Start");
    upper_street <int, string>  topnotch = new upper_street<int, string>();

    int kl = topnotch.doit();
    Console.WriteLine("Test33 End :  kl={0}", kl);
  }
}

// eof



