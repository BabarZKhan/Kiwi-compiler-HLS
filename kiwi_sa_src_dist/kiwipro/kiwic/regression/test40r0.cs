//
// Wrong answer is : The 4 bytes of the integer:  0  0  0  0

// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// Test 40 :  Simple unsafe code demo.
//

using System;
using System.Text;
using KiwiSystem;


class test40r0
{
    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
        int number = 1024;

	// sizeof valuetypes is considered safe by mcs - sizeof is evaluated within mcs
	if (true)
	{
	  Console.WriteLine("The size of short is {0}.", sizeof(short));
	  Console.WriteLine("The size of int is {0}.", sizeof(int));
	  Console.WriteLine("The size of long is {0}.", sizeof(long));
	}

        unsafe 
        {
            // Convert to byte:
            byte* p = (byte*)&number;

            System.Console.Write("The 4 bytes of the integer:");

            // Display the 4 bytes of the int variable:
            for (int i = 0 ; i < sizeof(int) ; ++i)
            {
	        Kiwi.Pause();
                System.Console.Write(" {0:X2}", *p);
                // Increment the pointer:
                p++;
            }
            System.Console.WriteLine();
            System.Console.WriteLine("The value of the integer: {0}", number);
        }
    }
}

    /* Output:
        The 4 bytes of the integer: 00 04 00 00
        The value of the integer: 1024
    */