// $Id: test30.cs,v 1.2 2012/02/15 11:45:31 djg11 Exp $
//
// 
// Kiwi Scientific Acceleration
// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test30r2 : Structure Array test.


using System;
using System.Text;
using KiwiSystem;


public static class test30r2
{

 public struct memop 
  {
    public int address;
    public int data;
  };


  static memop [] structarray = new memop [4];  // A simple static array of structs.


  public static void runTest30()
  {
      memop a1, a2;
      a1.data = 32;
      a1.address = 42;
      // stobj is used in test30r2 - for a structure assign :         30  : stobj (valuetype [test30r2]memop)
      structarray[1] = a1; // structure assign - uses CIL stobj instruction.

      a2.data = 132;
      a2.address = 142;

      structarray[2] = a2; // structure assign
      Kiwi.Pause();	      
      Console.WriteLine(" Test30r2 should still be 32 owing to valuetype store={0}.", structarray[1].data);
  }

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
      runTest30();
  }
}


// eof



