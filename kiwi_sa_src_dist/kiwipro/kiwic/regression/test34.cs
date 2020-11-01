// 
// Kiwi Scientific Acceleration

// Test 34 : elementary floating point

using System;
using System.Text;
using KiwiSystem;


class test34
{
    public static void double_test()
    {
        Console.WriteLine("Test34 d start.");
	double fxx = 1.0;

	for (int x=0; x< 10; x++)
	{
  		Kiwi.Pause();
        	Console.WriteLine("Test34 double mpx {0}.", fxx);
		Kiwi.Pause();
		fxx = fxx * 1.1;
        }
        Console.WriteLine("Test34 d end.");
     }	

    public static void float_multiply_test() // Simple multiply test
    {
        Console.WriteLine("Test34 s/p multiply start.");
	float fxx = (float)1.0;

	for (int x=0; x< 10; x++)
	{
  		Kiwi.Pause();
        	Console.WriteLine("  Test34 s/p multiply {0}.", fxx);
		Kiwi.Pause();
		fxx = fxx * (float) 1.1;
        }
        Console.WriteLine("Test34 s/p multiply end.");
     }	

    public static void float_divide_test() 
    {
        Console.WriteLine("Test34 s/p divide start.");
	float fxx = (float)1.456;
	float fyy = 302.123f;
	for (int x=0; x< 10; x++)
	{
  		Kiwi.Pause();
		//  Should print
                //     Test34 divide 1.456.
                //     Test34 divide 0.004819229.
                //     Test34 divide 1.538103E-05.
         	Console.WriteLine("  Test34 s/p divide {0}.", fxx);
		Kiwi.Pause();
		fxx = fxx / fyy;
		fyy += 11.2f;
        }
        Console.WriteLine("Test34 s/p divide end.");
     }	


    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test34 start.");
	//double_test();
	float_multiply_test();
	float_divide_test();
     }	

}

// eof
