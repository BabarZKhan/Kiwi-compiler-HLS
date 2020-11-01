// $Id: test25.cs $

// TEST25: Tricky non-strict operators and implicit state machine: how many
// program counter states do we get?  With two pause statements we should 
// need a total of three ?


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test25
{
    [Kiwi.OutputWordPort("dout")] static int dout;
    [Kiwi.InputWordPort("din")] static int din;


    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
       bool a = false, b=false;
       for (int dog=0;dog<20;dog++)
       {
          Kiwi.Pause(); 
          if (a && b) dout += 100;
          if (dout % 2 > 0) Kiwi.Pause(); 
	  dout += din + 1;
          a = !b; b = a;
          Console.WriteLine("dout={0}", dout);
       }
    }

}


// eof



