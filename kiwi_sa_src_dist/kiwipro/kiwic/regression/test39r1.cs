// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.

// Test 39r1 : Linked-list DRAM demo.

//
// r1 is Kiwi-1 style but uses ObjectHandler for pointer arithmetic.  Note the gcc4cil support also added pointer arithmetic without using ObjectHandler.
//

using System;
using System.Text;
using KiwiSystem;


unsafe class LinkedListOfInts
{
   public int car;                // Content Data
   public LinkedListOfInts cdr;   // This allows a potentially-infinite linked list.

   public LinkedListOfInts(int initial_cell_content) // Constructor used by C# compiler.
   {
     car = initial_cell_content;
   }

   static LinkedListOfInts freelist = null;
   static int sbrk = 0;

  // const int heapSize = 1000 * 1000 * 100; // 100 Megabyte -- will go in DRAM

   const int heapSize = 8000; // Smaller - will go in BRAM.
  
   static byte [] heapPool = new byte[heapSize];

   // The Kiwi.ObjectHandler provides a mechanism to read off the size of an object.
   static Kiwi.ObjectHandler<LinkedListOfInts> ohandler = new Kiwi.ObjectHandler<LinkedListOfInts>();
   static int itemSize()
   {
   #if IL
       // Inline CIL assembly language works under Microsoft's C# compiler.
       sizeof LinkedListOfInts
       ret
   #endif
 
     // Using mcs on linux we use a KiwiC backdoor that can measure the size of an object.
     return ohandler.size();
   }

  
   public static void createFreepool()  // This freepool is textually tied to our LinkedListIntCell so we will next want a further demo where it is polymorphic.
   {
      sbrk = 0;   
      Console.WriteLine("Test 39r1: Freepool created, {0} bytes.", heapSize);
    }

   static Object locker = new Object();

   public static LinkedListOfInts priv_alloc(int no)
   {
     LinkedListOfInts result = null;
     int bytes = itemSize();	
     if (sbrk + bytes >= heapSize && freelist == null) KiwiSystem.KiwiBasicStatusMonitor.RunTimeAbend("no free cells", 1);
     else if (freelist == null) 
     {
        lock (locker) 
     	  { 
  	    //This compiles but does not run under mono: it generates the "castclass LinkedListOfInts" CIL instruction.
       	    result = ohandler.handleArith((Object)heapPool, sbrk);
            // We should round up to a multiple of 8 bytes perhaps.
	    sbrk += bytes;
	  }
     }
     else
     {
       lock (locker) 
       { result = freelist;  freelist = result.cdr; }
     }
     result.car = no;
     return result;
   }

   public void Dispose()
   {
     lock (locker)
     {
       cdr = freelist;
       freelist = this;
     }
   }

}



class test39r1
{
//
// This is also a good illustration of field arrays exhibiting poor spatial locality, whether DRAM is cached or not.
//
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
       Console.WriteLine("Test 39r1 Linked Lists in DRAM Demo. Trees are a simple extension.");
       LinkedListOfInts.createFreepool();

       Kiwi.EndOfElaborate();
       Kiwi.Pause();

       // Now we are a customer for our cells - make a simple list
       LinkedListOfInts baser = null;

       for (int i = 0; i < 4; i++)
       {     
 	 LinkedListOfInts cell = LinkedListOfInts.priv_alloc(i*3+50);
	 cell.cdr = baser;
	 baser = cell;
	 Console.WriteLine("  Runtime Alloc Item {0}", i);
       }

       for (LinkedListOfInts p = baser; p != null; p = p.cdr)
	 {
	   Kiwi.Pause();
	   Console.WriteLine("  Readback Item {0}", p.car);
	 }
       Console.WriteLine("Test 39r1 Linked List DRAM Demo Finished.");		
    }
}

// eof
