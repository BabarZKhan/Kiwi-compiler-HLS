/*

A cuckoo hash is, like a set-associative cache, a means to mostly avoid chains of entries stored under a common hash (avoiding the Birthday Paradox) by using parallel lookup
in some number of ways. When all ways are full for a given entry, one of the items is chosen for relocation to a different
way, where it will be hashed to a different 'set' since the hash functions used in each way differ.  Hopefully that alternative location is not filled. If it is, either the
entry stored there can be similarly relocated, or else a different eviction candidate can be chosen.  Backtracking is potentially needed to explore alternative eviction candidates, so
it is easier to just push forward further items on a simple linear trajectory.    Lookup cost remains constant at unity, regardless of how much relocation is done.

 Highly relevant references used in the review, not mentioned in the paper: [1] Kekely Lukáš, Kučera Jan, Puš Viktor, Kořenek Jan, Vasilakos Athanasios: Software Defined Monitoring of Application Protocols. In: IEEE Transactions on Computers, vol. 65, no. 2, pp. 615-626, Feb. 2016. http://ieeexplore.ieee.org/xpl/articleDetails.jsp?tp=&arnumber=7087363 [2] Kekely Lukáš, Žádník Martin, Matoušek Jiří a Kořenek Jan: Fast Lookup for Dynamic Packet Filtering in FPGA. In: 17th IEEE Symposium on Design and Diagnostics of Electronic Circuits and Systems, pp. 219-222, May 2014. http://ieeexplore.ieee.org/xpls/abs_all.jsp?arnumber=6868793 


*/

//
//
// Kiwi Scientific Acceleration Example - Cuckoo Hash Demo
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
//
// 
//
//
using System;
using KiwiSystem;




public class DataGenerator
{
 	int seed;
	ulong dk;



  // PRGEN Park and Miller CACM October 1988 
  int random()
  {
    
    const int a = 2147001325;
    const int b = 715136305;
    seed = a * seed + b;
    return seed;
  }

  public void Reset()
  {
    seed = 123456;
    dk = 9999999900000000L;
  }
  
  public DataGenerator() // Constructor
  {
    Reset();
  }
  
  public int Generate(out int key, out ulong value)
  {
    key = random();
    value = dk ++;
    return 0;
  }
}



public class CuckooHasher
{
  const int n_ways = 4;
  ulong [] dataArray;
  int [] [] keyTables = new int [n_ways] [];
  int [] [] valuePointerTables = new int [n_ways] [];
  int waycap;
  int next_free = 0;
  int next_victim = 0;

  int stats_inserts = 0;
  int stats_insert_probes = 0;
  int stats_insert_evictions = 0;
  int stats_lookups = 0;
  int stats_lookup_probes = 0;


  public void printStats()
  {
    
    Console.WriteLine("cuckoo cache: this={0}, inserts={1}, lookups={2}", this, stats_inserts, stats_lookups);
    Console.WriteLine("cuckoo cache: insert_probes={0}, insert_evictions={1}", stats_insert_probes, stats_insert_evictions);
    Console.WriteLine("cuckoo cache: lookup_probes={0}", stats_lookup_probes);

  }

  public CuckooHasher(int capacity) // constructor
  {
    waycap = capacity / n_ways;
    for (int n=0; n<n_ways; n++) keyTables[n] = new int [waycap];
    for (int n=0; n<n_ways; n++) valuePointerTables[n] = new int [waycap];	
    dataArray = new ulong [capacity];
  }
  
  public void Clear()
   {
    for (int m=0; m<waycap; m++)
      for (int n=0; n<n_ways; n++) keyTables[n][m]= 0;
   }
  
  int hash(int hashno, int arg)
  {
    int v = hashno * 51  + arg;
    if (v < 0) v = - v;
    return (v % waycap);
  }
  
  
  public int insert(int key_in, ulong value)
  {
    int key = key_in;
    int evict_stat = 0;
    int p = 0;
    if (key ==0)
      {
        System.Diagnostics.Debug.Assert(false, "Key of zero used");
        return -4;
      }
    else if (next_free == waycap * n_ways)
      {
        System.Diagnostics.Debug.Assert(false, "Table is full");
        return -2;
      }
    else
      {
        p = next_free ++;
        dataArray[p] = value;
        stats_inserts += 1;
        while (true)
          {
            int nn, hh=0;
            for (nn=0; nn<n_ways; nn++)
              {
                // For H/W HLS implementation we expect this loop to be unwound so all ways are done in parallel.
                stats_insert_probes += 1;
                hh = hash(nn, key);
                if (keyTables[nn][hh] == 0) { keyTables[nn][hh] = key; break; }
              } 
            if (nn==n_ways)
              {
//	        return -10;
                Console.WriteLine("Eviction {0} needed", evict_stat);
                evict_stat++;
                stats_insert_evictions += 1;
                int key1 = keyTables[next_victim][hh];
                int p1 = valuePointerTables[next_victim][hh];
                keyTables[next_victim][hh] = key;
                valuePointerTables[next_victim][hh] = p;
                key = key1;
                p = p1;
                next_victim = (++next_victim) % n_ways;
              }
            else
              {
                keyTables[nn][hh] = key;
                valuePointerTables[nn][hh] = p;
                return 0;
              }
          }
      }
  }


  public int lookup(int key, out ulong value)
  {
    stats_lookups += 1;
    value = 0;
    if (key ==0)
       {
         System.Diagnostics.Debug.Assert(false, "Key of zero used");
	 return -4;
       }
    else
      {
	 int nn, h=0;
         for (nn=0; nn<n_ways; nn++) 
	 {
           // For H/W HLS implementation we expect this loop to be unwound so all ways are done in parallel.
           stats_lookup_probes += 1;
           h = hash(nn, key);
           if (keyTables[nn][h] == key) break; 
         } 
         if (nn==n_ways)
	 {
  	   return -5; // not found
         }
         int p = valuePointerTables[nn][h];
	 value = dataArray[p];
	 return 0;
      }
   }


}


static class Testbench
{
  const int items = 32768;




  [Kiwi.HardwareEntryPoint()]
  static public void Main()
  {
    Kiwi.KppMark(0, "Start");
    Console.WriteLine("Cuckoo cache testbench start. Capacity={0}", items);
    CuckooHasher chasher = new CuckooHasher(items);
    DataGenerator dg = new DataGenerator();

    Kiwi.Pause(); // End of static elaboration.

    chasher.Clear();
    Console.WriteLine("Cuckoo cache cleared");		 
    Kiwi.KppMark(1, "Cache Cleared");
    if (true)
      {
        dg.Reset();
        int trials = 0, successes = 0;
        for (int iv = 0; iv < 2*items/3; iv++)
          { int key; ulong value;
            dg.Generate(out key, out value);
            int rc = chasher.insert(key, value);
            if (rc == 0) successes ++;
            trials ++;
          }
        Console.WriteLine("Cuckoo cache inserted items {0}/{1}", successes, trials); 
      }
    
    Kiwi.KppMark(2, "Data Entered");
    if (true)
      {
        dg.Reset();
        int trials = 0, successes = 0;
        for (int iv = 0; iv < 2*items/3; iv++)
          { int key; ulong wvalue, rvalue;
            dg.Generate(out key, out wvalue);
            int rc = chasher.lookup(key, out rvalue);
            if (rc == 0 && rvalue == wvalue) successes ++;
            trials ++;
          }
        Console.WriteLine("Cuckoo cache retrieved items {0}/{1}", successes, trials);
      }
    Kiwi.KppMark(3, "Readback Done");
    chasher.printStats();
    Console.WriteLine("Cuckoo cache demo finished.");
    Kiwi.Pause();
    Kiwi.ReportNormalCompletion();
  }
}


// eof