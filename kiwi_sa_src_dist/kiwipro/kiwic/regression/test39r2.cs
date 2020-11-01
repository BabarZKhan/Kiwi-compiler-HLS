// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.

// Test 39r2 : Linked-list DRAM demo - with post-EndOfElaboration calls to new.


// This one works by generating calls to hpr_alloc that are then mapped (for RTL output) to a port on the heap manager FU at run time.  TODO check multi-threaded access to the heap manager FU.

// This would not compile under version 1 of KiwiC. It gave this error:
// Exception: Bad form heap pointer for obj_alloc of type IntegratedLinkedListOfInts post end of elaboration point (already allocated a variable sized object ?). sbrk=/tend:nota_const



using System;
using System.Text;
using KiwiSystem;

[Kiwi.MemSpace("space2")]
class IntegratedLinkedListOfInts
{
   public int car;                // Content Data
   public IntegratedLinkedListOfInts cdr = null;

   public IntegratedLinkedListOfInts(int initial_cell_content) // Constructor used by C# compiler.
   {
     car = initial_cell_content;
   }

   static IntegratedLinkedListOfInts freelist = null;

   static Object locker = new Object(); // Serves as a mutex.

   public static IntegratedLinkedListOfInts alloc(int no)
   {
     IntegratedLinkedListOfInts result = null;

     if (freelist == null) 
     {
       // In this demo we call the native 'new' function both before and after the end of static elaboration.  
       // The cells rezed before are same/different from those after?
       result = new IntegratedLinkedListOfInts(no);
     }
     else  // But we still need to manage our own disposals manually.
     {
       lock (locker) 
       { result = freelist;  freelist = result.cdr; }
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



class test39r2
{
//
// This is also a good illustration of field arrays exhibiting poor spatial locality, whether DRAM is cached or not.
//
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
       Console.WriteLine("Test 39r2 Linked Lists in DRAM Demo - with post-elabotate calls to new(). Trees are a simple extension.");

       IntegratedLinkedListOfInts baser = null;
       if (false)
         {
           // Add two cells to the list during static elaborate
           IntegratedLinkedListOfInts cell1 = IntegratedLinkedListOfInts.alloc(42);
           
           Kiwi.KppMark(1, "Done first elaborated alloc");
           
           IntegratedLinkedListOfInts cell2 = IntegratedLinkedListOfInts.alloc(44);
           
           Kiwi.KppMark(2, "Done second elaborated alloc");

           baser = cell1;
           cell1.cdr = cell2;
         }
       /*------------------ */ Kiwi.EndOfElaborate(); /*------------------ */

       Kiwi.Pause(); // Wait for a single clock edge - further work will be post elaborate.

       Kiwi.KppMark(3, "Post static elaboration now.");

       // Allocate cells at run time.
       for (int i = 0; i < 4; i++)
       {     
         Kiwi.Pause(); // Pause inside this loop before making further cells.
 	 IntegratedLinkedListOfInts cell = IntegratedLinkedListOfInts.alloc(i*3+200);  // TODO this is a strange number?
	 cell.cdr = baser;
	 baser = cell;
	 Console.WriteLine("  Runtime Alloc Item {0}", i);
       }


       // Printout phase.
       for (IntegratedLinkedListOfInts p = baser; p != null; p = p.cdr)
	 {
	   Kiwi.Pause();
	   Console.WriteLine("  Readback Item {0}", p.car);
	 }
       Console.WriteLine("Test 39r2 Linked List DRAM Demo Finished.");		
    }
}


/*  Expected output:
Test 39r2 Linked Lists in DRAM Demo - with post-elabotate calls to new(). Trees are a simple extension.
  Runtime Alloc Item 0
  Runtime Alloc Item 1
  Runtime Alloc Item 2
  Runtime Alloc Item 3
  Readback Item 209
  Readback Item 206
  Readback Item 203
  Readback Item 200
  Readback Item 42
  Readback Item 44
Test 39r2 Linked List DRAM Demo Finished.
*/
// eof
