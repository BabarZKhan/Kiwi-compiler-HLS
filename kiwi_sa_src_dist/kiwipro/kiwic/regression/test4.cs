// $Id: test4.cs,v 1.12 2011/08/08 21:42:37 djg11 Exp $
// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// TEST4: Counter and array testing.


using System;
using System.Text;
using KiwiSystem;


public static class test4
{
    static int limit = 10;
    [Kiwi.OutputBitPort("dout")] static bool dout;
    [Kiwi.InputBitPort("din")] static bool din;

    static public void arraypart()
    {
      int odd = 0, even = 0;  // These are V_0 and V_1 in the ast.cil file.
      int[] arr = new int [] {100, 1, 222221, 5, 7, 8, 1121, 2021, 2048};       // arr=V_2

      arr[2] = 2;
      // V_3 is the current element of the array
//NB: The structure of this test is nasty for Kiwic code generation, since the
//array initialisation is not committed by a pause until after its 
//first read in the first loop
      int pr = 0; // V_3

// Old C# compiler:
      // V_4 is a copy of V_2
      // V_5 is current element index
      // V_6 is current element - the variable vale in the foreach loop.      

// New:
      // V_5 is a copy of V_2
      // V_6 is current element index
      // V_4 is current element - the variable vale in the foreach loop.      


//      Kiwi.KppMark("WorkStart");
//      Kiwi.KppMark("fredsLoop");
      foreach (int vale in arr) // vale=V_6
      {
         if (vale%2 == 0)  
            even++;      
         else 
            odd++;         
         Kiwi.Pause(); // In hard pause mode, this should take one clock cycle per loop. res2-pipeline should pre-address the array for the next iteration.
	 Console.Write("{1} vale={0}: ", vale, pr++);
	 Console.WriteLine("so far {0} Odd Numbers, and {1} Even Numbers.", odd, even);

      }
//      Kiwi.KppMark("WorkEnd");
      Console.WriteLine("Found {0} Odd Numbers, and {1} Even Numbers.", odd, even) ;
      Kiwi.Pause();
   }


    public static void Main()
    {
	int j = 0;  
	arraypart();

	if (false) while(true)
	{
	  j = j + 1;
	  if (j >= limit) j = 0;
	  dout = din & (j % 2 > 0);
     	  Console.WriteLine(" J={0}.", j);
	  Kiwi.Pause();
       }
    }
}


// eof



