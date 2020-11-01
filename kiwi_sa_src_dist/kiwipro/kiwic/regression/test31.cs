// $Id: test31.cs,v 1.2 2012/07/05 14:37:24 djg11 Exp $
// Simple switch test


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test31
{

  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      for (int i=0; i<5;i++)
      {
        int q = 1234;
        Kiwi.Pause();
        switch (i)
        {
          case 4: q = 10000;
               break; //  Break was missing here on purpose for test. But not allowed in C#.
          case 5: q = 1000;
               break; 
          case 7: q = 100;
               break; 

          case 1: q = 10;
                goto case 4;

          case 6: case 44: case 102:
          case 2: q = 20;
            break;
          default: Console.WriteLine("{0} not set", q);
            break;
         }
         Console.WriteLine("{0} -> {1}", i, q);
      }
      Console.WriteLine(" Test31 complete");
    }
}


// eof



