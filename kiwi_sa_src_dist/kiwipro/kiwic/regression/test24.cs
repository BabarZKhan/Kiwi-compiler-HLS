// $Id: test24.cs,v 1.7 2011/06/08 17:45:10 djg11 Exp $
// (C) 2009-16 DJ Greaves, University of Cambridge, Computer Laboratory.
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// TEST24: Horrendous array structural hazarding.  Multiple array operations on one hard pausemode clock cycle.
//
// 
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test24r1
{
    static int limit = 10;
    [Kiwi.OutputWordPort("dout")] static uint dout;
    [Kiwi.InputWordPort("din")] static uint din;

    static uint[] arrx = new uint [4] {3,2,1,0}; 
    static uint[] arry = new uint [4] {3,2,1,0}; 

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        string smsg = "Test24r1 run1";
        Kiwi.Pause(); // Predelay

	arrx[3] = 0; // These stores are omitted by the C# compiler during array init.
	arry[3] = 0;
      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();
        dout = arrx[i];
        Console.WriteLine("Test24a {2} {0} {0}", i, dout, smsg);
      }

      for(int i = 0; i<arry.Length; i++)
      {
        dout = arrx[i]; // Display without pauses
        Console.WriteLine("Test24b {2} {0} {1}", i, dout, smsg);
      }

      for(int i = 0; i<arry.Length; i++)
      {
        dout = arrx[arrx[i]]; // self indirection - display without pauses
	
        Console.WriteLine("Test24c {2} {0} {1}", i, dout, smsg);
      }

      for(int i = 0; i<arry.Length; i++)
      {
        dout = arrx[arry[i]]; //  indirection: isplay without pauses
        Console.WriteLine("Test24d {2} {0} {1}", i, dout, smsg);
      }

      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause(); 
        dout = arrx[arrx[i]]; // Self indirection with pauses
        Console.WriteLine("Test24e {2} {0} {1}", i, dout, smsg);
      }


      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();//
        dout = arrx[arry[i]];  // Double indirection with pauses
        Console.WriteLine("Test24f {2} {0} {1}", i, dout, smsg);
      }


      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();// Complex
        dout = arrx[i] + arry[i] + arrx[arrx.Length - 1 - i];
        Console.WriteLine("Test24g {2} {0} {1}", i, dout, smsg);
      }
      for(int i = 0; i<arry.Length; i++)
      {
        dout = arrx[arry[i]] ++;  // Double indirection with pauses
        Console.WriteLine("Test24h {2} {0} {1}", i, dout, smsg);
      }
      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();//
        dout = arrx[i]++ + 100;
        Console.WriteLine("Test24ia {2} {0} {1}", i, dout, smsg);
      }
      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();//
        dout = ++arrx[i] + 100;
        Console.WriteLine("Test24ib {2} {0} {1}", i, dout, smsg);
      }
      for(int i = 0; i<arry.Length; i++)
      {
        Kiwi.Pause();//
//This was non-deterministic code: ok to give wrong answer!
//        dout = arrx[i]++ + arry[i]++ + arrx[arrx.Length - 1 - i];
        dout = arrx[i] + arry[i]++ + arrx[arrx.Length - 1 - i];
        Console.WriteLine("Test24j nondet {2} {0} {1}", i, dout, smsg);
      }

      Kiwi.Pause(); 
      Console.WriteLine("Temp Stop");
      Kiwi.Pause(); 
      return;
    }

}


// eof



