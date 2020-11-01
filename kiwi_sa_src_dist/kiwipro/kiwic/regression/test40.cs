// 
// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// Test40 : Simple unsafe code demo.
//

using System;
using System.Text;
using KiwiSystem;


class test40
{
    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
        int number = 1024;

        unsafe 
        {
            // Convert to byte:
            byte* p = (byte*)&number;

            System.Console.Write("The 4 bytes of the integer:");

            // Display the 4 bytes of the int variable:
            for (int i = 0 ; i < sizeof(int) ; ++i)
            {
                System.Console.Write(" {0:X2}", *p);
                // Increment the pointer:
                p++;
            }
            System.Console.WriteLine();
            System.Console.WriteLine("The value of the integer: {0}", number);

            // Keep the console window open in debug mode.
            //System.Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();
        }
    }
}

    /* Output:
        The 4 bytes of the integer: 00 04 00 00
        The value of the integer: 1024
    *///
