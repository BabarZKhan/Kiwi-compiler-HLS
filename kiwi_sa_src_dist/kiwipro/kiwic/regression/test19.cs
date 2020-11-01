// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// $Id: test19.cs,v 1.5 2011/06/13 12:58:42 djg11 Exp $
//
// test19: Old-style (pre IP-XACT) RPC tests with separately-compiled client and server.
//


// This example shows how to call a static method on a separately-compiled component.
// Actually, both client and server are cut from the same C# compilation, but in general they can come from different source files or DLLs.
// Using ths common file, there must be different KiwiC -root specifications when KiwiC is applied to it.

using System;
using System.Text;
using KiwiSystem;


// Old HSIMPLE interfacing, pior to IP-XACT, supported static calls only.

// Compiled typically with -root Test19_Server1;Test19_Server1.setget_pixel;Test19_Server1.get_id;Test19_Server1.start so that these all become mutually-exclusive entry points into a single RTL module.
// We should also add a Kiwi.RemoteClass test here please.

// We should check for free variables too, both constant and otherwise.

public class Test19_Server1
{
   static byte [] framestore = new byte [128*128];

   // This function will be remotely callable.  Protocol was previously unspecified. Did default to HSIMPLE.
   [Kiwi.Remote("protocol=HFAST:MIRRORABLE=false")]
   public static byte setget_pixel(uint axx, uint ayy, bool readf, byte wdata)
   {
     uint addr = axx * 128 + ayy;
     if (readf) 
       {
	 Kiwi.Pause();Kiwi.Pause();Kiwi.Pause();    // Give this some artificial latency
	 return framestore[addr];	 
       }

     else 
      { framestore[addr] = wdata;
	Console.WriteLine("Set pixel ({0},{1}) to {2}", axx, ayy, wdata);
	return wdata; 
      }
    }

   const string ps = "protocol=HFAST:MIRRORABLE=false";

  [Kiwi.Remote(ps)]
   public static short get_id()
   {
     return (short)1256;
   }


   [Kiwi.Remote(ps)]
   public static void start()
   {
     Console.WriteLine("Server started.");
   }
}


public class test19_client1
{

    [Kiwi.OutputWordPort("dout")] static short monout;

    static void framestore_draw_diagonal()
    {
      for (uint xx=44; xx<44+10; xx++)
      {
        uint yy = xx + 63;
	Test19_Server1.setget_pixel(xx, yy, false, (byte)((xx+yy))); // Do some data writes
        Kiwi.Pause();
      }

      for (uint xx=44; xx<44+10; xx++)
      {
        uint yy = xx + 63;
	monout = Test19_Server1.setget_pixel(xx, yy, true, (byte)0);  // Now some readbacks
	Console.WriteLine(" Readback {0}  {1}", xx, monout);
        Kiwi.Pause();
      }
    }

    // This is the h/w entry point, but we use the -root command line flag to invoke it, not the inline Kiwi.HardwareEntryPoint() attribute.
    public static void Main()
    {
       Console.WriteLine("Start of Test 19");
       Kiwi.Pause();
       monout = Test19_Server1.get_id();
       Console.WriteLine("Remote server id is {0}", monout);
       Kiwi.Pause();
       Test19_Server1.start();
       framestore_draw_diagonal();
       Console.WriteLine("End of Test 19");
    }

}

// eof


