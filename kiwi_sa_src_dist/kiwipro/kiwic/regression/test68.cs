// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// 
// Test68: method generics
// 

using System;
using System.Text;
using KiwiSystem;



class Swapper
{
  public static void Swap<T>(ref T l, ref T r) // The method generic is part of the method name.
  {
    T temp;
    temp = l; l = r; r = temp;
  }

}

class bench
{
  [Kiwi.OutputBitPort("done")] static bool done = false;


  static int a1 = 3, a2 = 4;
  static char c1 = 'y', c2 = 'x';


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test68  starting.");
    for (int px=3; px>=0; px--)
      {
	Kiwi.Pause();
	Swapper.Swap<int>(ref a1, ref a2);
	if (px == 1) Swapper.Swap<char>(ref c1, ref c2);
	Console.WriteLine("   Swapsies {0}  {1}  {2}", px, a1, c1);
      }
  
    Console.WriteLine("Test68 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof