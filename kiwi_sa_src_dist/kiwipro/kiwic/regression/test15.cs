// $Id: test15.cs,v 1.6 2010/11/29 11:03:38 djg11 Exp $ kiwic test (North Cottage)
//
// test15 - test constructors and class constructors

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KiwiSystem;

class test15
{

  class dcop
  {
	public int outer = 3;
	public int inner = 4;

	public dcop () { } // Nullary Constructor

	public dcop (int k, int dummy) // Constructor
	{
	  inner = k;
        }


	public int getter(int q)
	{
	  return q + inner + outer;
        }
  };
  
    static dcop ha = new dcop();
    static dcop hb = new dcop(100, 101);

    public static void Main()
    {
	dcop hc = new dcop(200, 101);


        Console.WriteLine("First   1007 {0}", ha.getter(1000));
        Console.WriteLine("Second  2007 {0}", ha.getter(2000));
        Console.WriteLine("Third   3103 {0}", hb.getter(3000));
        Console.WriteLine("Fourth  4103 {0}", hb.getter(4000));
        Console.WriteLine("Fifth   5203 {0}", hc.getter(5000));
        Console.WriteLine("Sixth   6203 {0}", hc.getter(6000));

        Console.WriteLine("DONE");

    }
}

// eof


