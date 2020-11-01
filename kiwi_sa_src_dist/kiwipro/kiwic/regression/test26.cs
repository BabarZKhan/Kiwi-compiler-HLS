// $Id: test26.cs,v 1.2 2011/10/18 22:33:42 djg11 Exp $
// Kiwi Scientific Acceleration
//
// TEST26: passing by reference a field of an object that has two or more fields of that type. With and without assignment
// between these fields.
//


using System;
using System.Text;
using System.Threading;
using KiwiSystem;


public static class test26
{

     // Net-level I/O.
    [Kiwi.OutputWordPort("dout")] static int dout;
    [Kiwi.InputWordPort("din")] static int din;

    class dc
	{
	   public int not_used, left, right;
	   public dc next;
	};

    public static void edict(ref int moscow, out int zulu)
    {
	Console.WriteLine(moscow + " ");
	moscow += 2;
	zulu = moscow + 2;
    }


    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      dc bog1 = new dc();
      int fish = 32;
      int chips = 101;
      bog1.left = 22;
      bog1.right = 42;
      bog1.next = bog1;
      Kiwi.Pause();
      edict(ref bog1.right, out fish);

      Console.WriteLine("Test 26 mid point 1: fish={0} chips={1}", fish, chips);

      Kiwi.Pause();
      int another = 1000;
      edict(ref bog1.right, out chips);
      Console.WriteLine("Test 26 end point: fish={0} chips={1}", fish, chips);
      Console.WriteLine("Test 26 finished.");

    }
}


// eof



