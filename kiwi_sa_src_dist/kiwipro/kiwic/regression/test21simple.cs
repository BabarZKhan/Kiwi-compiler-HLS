// Test 21 - 2D and 3D arrays
// $Id: test21simple.cs,v 1.1 2012/06/11 15:57:37 djg11 Exp $
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;


// canned called Set and Get
// Generated by gmcs caller Set and Get
// Generated by gmcs on callee:      set_Item and get_Item
// Generated by gmcs on cache caller and callee  set_Item and get_Item
class test21simple
{
    static int limit = 5;

    static int [,] a2 = new int [limit, limit];
    static int [,,] a3 = new int [limit, limit, 2];
//    static int [,,] b3 = new int [2, 2, 2];

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
	for (i=1;i<limit;i++)
	{
	  for (j=1;j<limit;j++)
	    {
	      int fata = 50-i*j;
	      a2[i, j]    = 100+fata;
	      a3[i, j, 0] = 200+fata;
	      a3[i, j, 1] = 300+fata;
	    }
	  if (i == 4) Kiwi.Pause();
	}
        Console.WriteLine("stage 1");
	for (i=1;i<limit;i++)
	{
          Console.Write("a2[i,j]    ");
	  for (j=1;j<limit;j++)
	    {
	      int fata = a2[i, j];
	      edict(j==limit-1, fata, 32);	      
	    }

//          b3[1,1,1] = 10000;

          Console.Write("a3[i,j, 0] ");
	  for (j=1;j<limit;j++)
	    {
	      int fata = a3[i, j, 0];
	      //Console.Write("{0}:", j);
	      edict(j==limit-1, fata, 32);	      

	    }
	  Kiwi.Pause();
          Console.Write("a3[i,j, 1] ");
	  for (j=1;j<limit;j++)
	    {
	      int fata = a3[i, j, 1];
	      edict(j==limit-1, fata, 32);	      
	    }
	  Kiwi.Pause();
	}
	Kiwi.Pause();
    }
}

// eof
