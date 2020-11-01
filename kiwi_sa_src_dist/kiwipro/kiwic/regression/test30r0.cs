// $Id: test30.cs,v 1.2 2012/02/15 11:45:31 djg11 Exp $
//
// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
// 
// Test30r0 : Simple structure tests

// C# passes structs by value to a method, meaning local modifications to contents do not commit to original instance.
// C# assigns structs by value, so all fields in the destination are updated by the assigment, rather than the handle just being redirected.


// TODO copy guts of test35, method updates to structs, in here.

using System;
using System.Text;
using KiwiSystem;



public static class test30r0
{

 public struct memop 
  {
    public int address;
    public int data;
  };

 public class classop 
  {
    public int c_address;
    public int c_data;
  };


  [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      // Two local structs on the stack
      memop a1, a2; // As valuetypes, these need stack addresses that are wider apart than object handles.
      classop c3;
      a1.address = 42;
      a1.data = 32;
      Kiwi.Pause();	 
      a2 = a1; // A structure assign, not being a class pointer assign, will transfer all fields.
      Kiwi.Pause();	 
      a2.data = 100;
      Kiwi.Pause();	 
      // This tests the difference between a structure and a class since a1 and a2 remain distinct.

      Console.WriteLine("Test30r0. The following value should still be 32 owing to valuetype store. a1.data={0}.", a1.data);
    }
}


// eof



