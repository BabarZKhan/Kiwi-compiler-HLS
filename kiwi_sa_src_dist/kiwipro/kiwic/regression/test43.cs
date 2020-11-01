// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test43 : Checked overflows.
//


/* Under construction: 

The value of the integer: 1
The value of the integer: 1001
The value of the integer: 1002001
The value of the integer: 1003003001
The value of the integer: -1016343263

Last line should not be printed ...
*/

using System;
using System.Text;
using KiwiSystem;

class test43
{

    [Kiwi.HardwareEntryPoint("BINARY-CONTROL")]
    static void Main() 
    {
        int v = 1;
        checked 
	{
	System.Console.WriteLine("test43 start");
	while (true)
	{
	  Kiwi.Pause();
          System.Console.WriteLine("The value of the integer: {0}", v);
          v = v * 1001;
        }

	System.Console.WriteLine("test43 finished");
    }

   }

}

// eof
