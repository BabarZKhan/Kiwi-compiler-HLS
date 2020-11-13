// Test 21 - 2D and 3D arrays
//
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
// $Id: test21.cs,v 1.3 2012/06/11 15:57:37 djg11 Exp $
//

// Finishes at time 4.4us with -res2-pipeline=off
//                             -res2-pipelining=full

using System;
using System.Text;
using KiwiSystem;


// canned called Set and Get
// Generated by gmcs caller Set and Get
// Generated by gmcs on callee:      set_Item and get_Item
// Generated by gmcs on cache caller and callee  set_Item and get_Item
class test21
{
    static int limit = 5;

  static int [,] aa2 = new int [10, 11];
  static int [,,] a3 = new int [5, 7, 4];
  static int [,,] b3 = new int [6, 8, 9];

    // static int [,] aa2 = new int [limit, limit];      // 25 word RAM - written once so becomes a ROM if we had a pause at Point-A
    // static int [,,] a3 = new int [limit, limit, 2];  // 50 word RAM
    // static int [,,] b3 = new int [2, 2, 2];          // 8 word RAM

    public static string edict(bool nlf, int k, int mm)
    {
	Console.Write(k + " ");
	if (nlf) Console.WriteLine(" BAR");
	return "+";
    }

    public static void Main()
    {
        int i, j;
 	Console.WriteLine("Test 21: Up To " + limit);
        Kiwi.KppMark(0, "Start of test");
	for (i=1;i<limit;i++)
	{
	  for (j=1;j<limit;j++)
	    {
	      int fata = 50-i*j;
	      aa2[i, j]   = 100000+fata;
	      a3[i, j, 0] = 200+fata;
	      a3[i, j, 1] = 300+fata;
	    }
	  if (i == 4) Kiwi.Pause();
	}

        Kiwi.KppMark(1, "Point-A:Stage 1: aa2 readbacks");
        Console.WriteLine("stage 1"); // readbacks aa2

        // This will be the end of elaboration point

	for (i=1;i<limit;i++)
	{
          Console.Write("aa2[i,j]    ");
	  for (j=1;j<limit;j++)
	    {
	      int fata = aa2[i, j];
	      edict(j==limit-1, fata, 32);	      
	    }

          b3[1,1,1] = 10000;  // One-off poke a3

          Console.Write("a3[i,j, 0] "); // Read back a3 0
	  for (j=1;j<limit;j++)
	    {
	      int fata = a3[i, j, 0];  // First read back at address 1*5*2 + 1*2 + 0 = 12. Should get 249. 
	      //Console.Write("{0}:", j);
	      edict(j==limit-1, fata, 32);	      

	    }
	  Kiwi.Pause();
          Console.Write("a3[i,j, 1] ");    // Read back a3 1
	  for (j=1;j<limit;j++)
	    {
	      int fata = a3[i, j, 1];
	      edict(j==limit-1, fata, 32);	      
	    }
	  Kiwi.Pause();
	}
	Kiwi.Pause();
        Kiwi.KppMark(2, "END");
    }
}

// eof