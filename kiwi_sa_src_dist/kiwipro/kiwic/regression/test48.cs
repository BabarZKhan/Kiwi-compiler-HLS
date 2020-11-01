// Kiwi Scientific Acceleration


// Test48.cs - Bounded control flow recursion - not working recently.
 
using System;
using System.Text;
using KiwiSystem;

class OldTest3RecursionTest // This has stopped working with the new gtrace code.
{ 
  static void subroutine(int p, int bound)
  {
    Console.WriteLine("subroutine {0} {1}", p, bound);
    if (bound > 0) subroutine(p*10+1, bound-1);
  }


  public static void SpareMain()
  {
    for (int jojo=1; jojo<=5; jojo+=1) 
      {
        Kiwi.Pause();
        subroutine(3,3);
       }
  }
      
}


class test480
{

  static int triangle(int v)
  {
    if (v == 1) return v;
    else return v + triangle(v-1);
  }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test test48 start.");
    Kiwi.EndOfElaborate();
    Kiwi.Pause();

    for (int pp = 1; pp <= 3; pp++)
    {
      int rr = triangle(pp);
      Console.WriteLine("  test48 datum {0}  triangle={1}", pp, rr);
    }

    Console.WriteLine("Test test48 finished.");
    Kiwi.Pause();
  }  
}
// eof



