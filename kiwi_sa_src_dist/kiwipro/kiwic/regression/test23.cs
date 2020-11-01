// $Id: test23.cs,v 1.2 2011/05/02 07:23:07 djg11 Exp $

// Test23: Dual-port RAM ??


// Why is this a dual-port RAM test ? In what way?

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test23
{
    static int limit = 10;
    [Kiwi.OutputWordPort("dout")] static int dout;
    [Kiwi.InputWordPort("din")] static int din;


    static int[] arrx = new int [2] {100, 110 }; 
    static string stringer = "Hello World";

    public static void Main()
    {
      for(int i = 0; i<stringer.Length; i++)
      {
	int v = din;
        Kiwi.Pause();
	//	int ch = ((int)s[i]);  
        dout = arrx[v & 1];
        Console.WriteLine("Hello {0}", dout);
       }
    }
}


// eof



