// Kiwi Scientific Acceleration:  KiwiC Regression Tests.
// $Id: test16.cs,v 1.5 2011/05/13 09:25:19 djg11 Exp $

// TEST 16: Simple adder - should perhaps be a combinational logic demo

using System;
using System.Text;
using System.Threading;
using KiwiSystem;

public static class test16
{

    [Kiwi.OutputWordPort("dout")] static int dout;
    [Kiwi.InputWordPort("din0")] static int din0;
    [Kiwi.InputWordPort("din1")] static int din1;


    public static void MainOld()
    {
	while(true)
	{
	  Kiwi.Pause();
	  dout = din0 + din1;
       }
    }
}


namespace Adder
{
    class Adder
    {

        [Kiwi.InputWordPort(7,0)]
        public static int x;

        [Kiwi.InputWordPort(7,0)]
        public static int y;

        [Kiwi.OutputWordPort(7,0)]
        public static int z;

        static void Main()
        {

            while (true)
            {
                z = x + y;
                Kiwi.Pause();
            }

        }
    }
}



// eof



