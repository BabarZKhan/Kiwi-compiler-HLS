// $Id: structuretest.cs,v 1.1 2009/03/11 21:52:48 djg11 Exp $ kiwic test (North Cottage)
//
//
// Tests run-time twiddle of object pointers, but without garbage collection.
//
// Also handles structs at some point...
//




class northtest
{
   struct ds
	{
	   public int left, right;
//	   public int structure_betty(int a) { return a+1; }
	};
  class dc
	{
	   public int left, right;
//	   public int structure_betty(int a) { return a+1; }
	};
  

  


    public static void Main()
    {
	dc ha; ha = new dc();
	dc hb; hb = new dc();

	ha.left = 22;
	ha.right = 23;
	ha.left = 32;
	hb.right = 33;

//	ds my_ds;
// Structs not implemented yet...
//        my_ds.a = 2;
//        my_ds.b = 2;
	  while (true)
	  {
	    Kiwi.Pause();
	    dc ht = hb; hb = ha; ha = ht;
    
//          Console.WriteLine("  Ans {0}", max3(1, 10, 3));
	    Console.WriteLine("  North test {0}", ha.left);
	  }
//        Console.WriteLine("End of Test {0}", my_ds.structure_betty(10));
    }
}

// eof


//

