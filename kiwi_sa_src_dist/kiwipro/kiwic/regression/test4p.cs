// $Id: test4p.cs,v 1.1 2012/02/21 14:26:45 djg11 Exp $
// TEST4P: Pauseless: Counter and array testing.


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test4p
{
    static int limit = 10;
    [Kiwi.OutputBitPort("dout")] static bool dout;
    [Kiwi.InputBitPort("din")] static bool din;

    static public void arraypart()
    {
      int odd = 0, even = 0;  // These are V_0 and V_1 in the ast.cil file.
      int[] arr = new int [] {0, 1, 222221, 5, 7, 8, 1121, 2021, 2048};       // arr=V_2

      arr[2] = 2;
      // V_3 is the current element of the array
//NB: The structure of this test is nasty for Kiwic code generation, since the
//array initialisation is not committed by a pause until after its 
//first read in the first loop
      int pr = 0; // V_3
      // V_4 is current element
      // V_5 is a copy of V_2
      foreach (int vale in arr) // vale=V_6
      {
         if (vale%2 == 0)  
            even++;      
         else 
            odd++;         
//         Kiwi.Pause();
	 Console.Write("{1} vale={0}: ", vale, pr++);
	 Console.WriteLine("so far {0} Odd Numbers, and {1} Even Numbers.", odd, even);
      }
      Console.WriteLine("Found {0} Odd Numbers, and {1} Even Numbers.", odd, even) ;
//      Kiwi.Pause();
   }


    public static void Main()
    {
        Kiwi.Pause(); // One pause to stop it running at compile time.
	int j = 0;  

	arraypart();
	Console.WriteLine("Test 4p done.");
    }
}


// eof



