// $Id: test20.cs,v 1.4 2010/11/29 11:03:38 djg11 Exp $
//
// test20: properties: setters and getters.
//
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;

class test20
{

  class dc
	{
	   public int left, right;
     
           public int seconds {
             get { return left+right; }
             set { left = 1000; right = value; }
           }

	};
  

    public static void Main()
    {
	dc ha; ha = new dc();

	ha.left = 200;
	ha.right = 400;

        Console.WriteLine("First printed value should be 200:  {0}", ha.left);

        Kiwi.Pause();
	Console.WriteLine("  Test20  l={0} r={1}", ha.left, ha.right);

        Kiwi.Pause();
	Console.WriteLine("  Test20  sumval={0}", ha.seconds);

        Kiwi.Pause();
	ha.seconds = 50;
        Kiwi.Pause();
	Console.WriteLine("  Test20  l={0} r={1}", ha.left, ha.right);
	Console.WriteLine("  Test20  sumval={0}", ha.seconds);


        Console.WriteLine("End of Test {0}", 1234);
    }
}

// eof


//

