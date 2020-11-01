// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// test61.cs


// 
// BitTally - count the ones in a word
// 

using System;
using System.Text;
using KiwiSystem;

class BitTally
{
  public static uint tally00(uint ind)
  {
    uint tally = 0;
    for (int v =0; v<32; v++)
      {
        if (((ind >> v) & 1) != 0) tally ++;
// 	Console.WriteLine("  {0} bax {1}", v, tally);
      }
    return tally;
  }


  public static uint tally01(uint ind)
  {
    // Borrowed from the following, which explains why this works:
    // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel 
    uint output = ind - ((ind >> 1) & 0x55555555);
    output = ((output >> 2) & 0x33333333) + (output & 0x33333333);
    output = ((output + (output >> 4) & 0xF0F0F0F) * 0x1010101);
    return output  >> 24;
  }

  public static uint tally02(uint ind)
  {
    uint tally = 0;
    for (int v =0; v<32; v++) // should be 32
      {
        tally += ((ind >> v) & 1);
      }
    return tally;
  }

  // A 256-entry lookup table will comfortably fit in any L1 dcache.
  // But Kiwi requires (required?) a mirror mark up to make it produce 4 of these.
  static readonly byte [] tally8 = new byte [] {  
    0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
    1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
    1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
    1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
    2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
    3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
    3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
    4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
  };

  public static uint tally03(uint ind)
  {
    uint a0 = (ind >> 0) & 255;
    uint a1 = (ind >> 8) & 255;
    uint a2 = (ind >> 16) & 255;
    uint a3 = (ind >> 24) & 255;
    return (uint)(tally8[a0] + tally8[a1] + tally8[a2] + tally8[a3]);
  }

}


class bench
{
  const int limit = 100000000;

  [Kiwi.OutputBitPort("done")] static bool done = false;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    int jojo;
    Console.WriteLine("BitTally 1 Limit=" + limit);
    if(true)    for (jojo=1;jojo<=limit;jojo*=21) 
      {
        Kiwi.NoUnroll(); 
        uint testd = (uint)(31*jojo);
        uint tally = BitTally.tally00(testd);
	Kiwi.Pause();
        Console.WriteLine("   {0}  00 answers {1}", testd, tally); 
      }  

    if(true) for (jojo=1;jojo<=limit;jojo*=21) 
      {
        Kiwi.NoUnroll(); 
        uint testd = (uint)(31*jojo);
        uint tally = BitTally.tally01(testd);
	Kiwi.Pause();
        Console.WriteLine("   {0}  01 answers {1}", testd, tally); 
      }  

    if(true)for (jojo=1;jojo<=limit;jojo*=21) 
      {
        Kiwi.NoUnroll(); 
        uint testd = (uint)(31*jojo);
        uint tally = BitTally.tally02(testd);
	Kiwi.Pause();
        Console.WriteLine("   {0}  02 answers {1}", testd, tally); 
      }  

    if (true) for (jojo=1;jojo<=limit;jojo*=21) 
      {
        Kiwi.NoUnroll(); 
        uint testd = (uint)(31*jojo);
        uint tally = BitTally.tally03(testd);
	Kiwi.Pause();
        Console.WriteLine("   {0}  03 answers {1}", testd, tally); 
      }  
    Console.WriteLine("Test61 BitTally finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof



