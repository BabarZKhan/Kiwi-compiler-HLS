// Kiwi Scientific Acceleration: KiwiC compiler test/demo.//
// $Id: test3.cs,v 1.2 2009/07/28 22:19:02 djg11 Exp $
//
//  Enumeration type tests

 
using System;
using System.Text;
using KiwiSystem;

class ShiptonEnums 
{

    enum EValues
    {
        XAA = 0x06, // Not in ascending order?
        XAB = 0x01,
        XAC = 0x11,
        XAD = 0x99
    }

    public static void RunE2(int p) 
    {
      Console.WriteLine("Run E2 {0}", p);
      EValues vv = (EValues)(p);
      if (p == 0) vv = EValues.XAC;
      Kiwi.Pause();
      switch (vv)
        {
        case EValues.XAA:
          Console.WriteLine("  XAA");
          break;
        case EValues.XAB:
          Console.WriteLine("  XAB");
          break;
        case EValues.XAC:
          Console.WriteLine("  XAC");
          break;
        case EValues.XAD:
          Console.WriteLine("  XAD");
          break;
        default:
          Console.WriteLine("  Unknown value {0:x}", (byte)vv);
          break;
        }
        Kiwi.Pause();
        Console.WriteLine("  Enum pt one {0} {1}", vv, (int)vv);
    }

}




class test3
{	
   enum Beatle { billy, john, paul, george, ringo };
// enum surname { "Star", "Harison", "Lennon", "McCartney", "Preston" }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Kiwi.Pause();
    Beatle f = Beatle.john;
    if (f == Beatle.john) Console.WriteLine(" yoko ");
    else if (f == Beatle.george) Console.WriteLine(" patie ");
    f = Beatle.george;


    for (int p=0; p<3; p++) ShiptonEnums.RunE2(p);

    return; // A top-level return gets mapped to an hpr_sysexit() call. This can lead to certain logic synthesisers complaining about $finish being unsupported, which is a fairly reasonable complaint.

    // Reflection API is needed for this final part
    // get a list of member names from Beatle enum,
    // figure out the numeric value, and display
    foreach (string volume in Enum.GetNames(typeof(Beatle)))
      {
        Console.WriteLine("Beatle Member: {0}\n Value: {1}", volume, (byte)Enum.Parse(typeof(Beatle), volume));
      }

    Console.WriteLine("Test3 finished ... let it be\n");
    }
}
// eof