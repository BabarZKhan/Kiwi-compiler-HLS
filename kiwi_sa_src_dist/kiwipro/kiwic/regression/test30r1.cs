// $Id: test30.cs,v 1.2 2012/02/15 11:45:31 djg11 Exp $
//
// 
// Kiwi Scientific Acceleration.
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test30r1 : Structure net-level I/O test.


using System;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test30r1
{

 public struct memop 
  {
    public short address;
    public int data;
    public char qual;
  };

  [Kiwi.InputWordPort("memin")]    static memop memin;
  [Kiwi.OutputWordPort("memout")]  static memop memout;

  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      Console.WriteLine("Test30r1 start");
      for (int counter=0; counter != 4; counter ++)
        {
          Kiwi.Pause();	      
          memout.qual = memin.qual;
          memout.data = memin.data+1;
          //Console.WriteLine(" Test30r1 should still be 32 owing to valuetype store={0}.", structarray[1].dat);
        }

    }
}


// eof



