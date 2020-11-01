// $Id: test12.cs,v 1.7 2011/10/03 14:05:45 djg11 Exp $

// Test 12 : Simple dynamic object test with the essence of a linked-list.
//           Simple dynamic objects (Kiwi-1 style - becoming statically allocated during KiwiC compilation)

using System;
using System.Text;
using KiwiSystem;


class sonclass<T1, T2>
{
  T1 ama_t_one;
  T2 ama_t_two;
  public int bill;

  public sonclass<T1, T2> child;   // This allows a potentially-infinite linked list.

  public int ben(int weed)
  {
    return weed + bill;
  }
}


class test12
{
    public static void Main()
    {
       sonclass<byte, string>  andy = new sonclass<byte, string>();
       sonclass<byte, string> martin = new sonclass<byte, string>();
       andy.child   = new sonclass<byte, string>();
       martin.child = new sonclass<byte, string>();

       Console.WriteLine("Test 12 Michaelmas 2011.");

       andy.bill         = 100; // We have four instances and we put a value in all the bill fields.
       andy.child.bill   = 200;
       martin.bill       = 300;
       martin.child.bill = 400;

        Console.WriteLine("  Andy.bill  {0}", andy.bill );
        Console.WriteLine("  Andy.ben   {0}", andy.ben(11));
        Console.WriteLine("  Martin.ben {0}", martin.ben(12));
        Console.WriteLine("  Martin.child.ben {0}", martin.child.ben(14));

       // Now swap some pointers over, at compile time, and see if that works.
       sonclass<byte, string> pointer = martin;
       Console.WriteLine("  Martin.child.ben {0}", pointer.child.ben(16));
       pointer = andy;
       Console.WriteLine("  Andy.child.ben {0}", pointer.child.ben(16));
       andy = martin;
       martin = pointer;

       Console.WriteLine("  Swapped roof one   {0}", pointer.child.ben(18));
       Console.WriteLine("  Swapped roof other {0}", pointer.child.ben(20));
       for (int i = 0; i < 4; i++)
	 {     
	   Kiwi.Pause(); // better change to Kiwi.NoUnroll()
	   pointer = (i==0)?martin: (false && i==1) ? null: i==2? andy: andy.child;
	   Console.WriteLine("  Swapped one   i={0} {1}", i, /*pointer==null ? -1: */pointer.ben(18));
	   Console.WriteLine("  Swapped other i={0} {1}", i, /*pointer==null ? -2: */pointer.ben(20));
	 }
    }
}

// eof
