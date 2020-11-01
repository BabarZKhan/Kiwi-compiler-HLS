// Kiwi Scientific Acceleration
// (C) 2015-17 DJ Greaves - University of Cambridge Computer Laboratory.

// See demo on:
//    http://www.cl.cam.ac.uk/research/srg/han/hprls/orangepath/kiwic-demos/linkedlists.html

// Test 39r0 : Linked-list basic demo. Kiwi-1 style where freepool is per object type and statically allocated.  Unsafe code support in the future will make this basic approach applicable to polymorphic heap-like pools with a cast on the return type.


using System;
using System.Text;
using KiwiSystem;


class LinkedListOfInts
{
   public int car;
   public LinkedListOfInts cdr;   // This allows a potentially-infinite linked list.

   public LinkedListOfInts(int initial_cell_content) // Constructor used by C# compiler.
   {
     car = initial_cell_content;
   }


   // Static Members - the freelist
   static LinkedListOfInts freelist = null;

   public static void createFreepool(int no)
   {
      for (int i = 0; i < no; i++)
      {     
	LinkedListOfInts available_cell = new LinkedListOfInts(11*i);
	available_cell.cdr = freelist;
	freelist = available_cell;
	if (false) Console.WriteLine("  Created Item {0} ptr={1}", i, available_cell);
	Console.WriteLine("  Created Item {0}", i);
      }
      Console.WriteLine("Test 39r0: Freepool created.");
    }

   static Object locker = new Object();

   public static LinkedListOfInts k1_alloc(int no)
   {
     LinkedListOfInts result = null;
     if (freelist == null) KiwiSystem.KiwiBasicStatusMonitor.RunTimeAbend("no free cells", 1);
     else
     {
       lock (locker) 
       {
         result = freelist;  freelist = result.cdr; 
       }
       result.car = no;
     }
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


class test39r0
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
       const int poolSize = 4;
       Console.WriteLine("Test 39r0 Linked List Simple Demo.  Poolsize={0}", poolSize);
       LinkedListOfInts.createFreepool(poolSize);
       Console.WriteLine("Test 39r0 Linked List Simple Demo.");

       // For Kiwi-1 style allocate from a static pool, before the first Pause - create all the cells you will use and put them on the freelist.
       Kiwi.EndOfElaborate();
       Kiwi.Pause();

       // Now we are a customer for our cells - make a simple list
       LinkedListOfInts baser = null;

       for (int i = 0; i < 3; i++)
       {     
 	 LinkedListOfInts cell = LinkedListOfInts.k1_alloc(i*3+50);
	 cell.cdr = baser;
	 baser = cell;
	 Console.WriteLine("  Runtime Alloc Item {0}", i);
       }

       for (LinkedListOfInts p = baser; p != null; p = p.cdr)
	 {
	   Kiwi.Pause();
	   Console.WriteLine("  Readback Item {0}", p.car);
	 }
       Console.WriteLine("Test 39r0 Linked List Simple Demo Finished.");		
    }
}

// eof
