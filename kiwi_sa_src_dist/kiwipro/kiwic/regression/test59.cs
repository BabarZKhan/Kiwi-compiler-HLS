// Kiwi Scientific Acceleration
// (C) 2016 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test59 : Test of structures with layout control and setter/getters for fields.
//


// THIS TEST IS NOT FINISHED 

using System;
using System.Text;
using KiwiSystem;

using System.Runtime.InteropServices;

public class Test59
{

  [StructLayout(LayoutKind.Sequential, Size=2, Pack=2)]
  struct StructDut
  {
    
    private Int16 sdata0, sdata1; UInt64 sdata2;

    public int All
    {
      get { return (int) sdata1;  }
    }

    public int Boz
    {
      get     {  return (int)sdata1 & 0xf; }
      set     { sdata1 = (short)((sdata1 & 0xfff0) | value & 0xf); }
    }
    
    public int MiddleMan
    {
      get   {  return (int)sdata1 & 0x1f0; }
      set   {  sdata1 = (short)((sdata1 & 0xfe0f) | value & 0x1f0); }
    }
    
    public int Sandra
    {
      
      get { return (int)sdata1 & 0xfe00; }
      set { sdata1 = (short)((sdata1 & 0x1ff) | value & 0xfe00); }
    }
  }

  static StructDut aa, bb, cc;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test59 start.");
    aa.Boz = 2;
    aa.Sandra = 0x8000;
    bb = aa;
    aa.Sandra = 0x9000;
    Console.WriteLine("   All {0:X}", bb.All);
    Console.WriteLine("   1st Boz={0}    Sandra={1}", bb.Boz, bb.Sandra);

    for (int xx=0; xx<2; xx++)
      {
	StructDut pp = aa, qq = aa;
	// Can we somehow have an anonymous struct ?
	pp.Sandra += 0x200;
	qq.Boz += 7;
	cc = (xx == 1) ? pp:qq;
	Console.WriteLine("   2nd Boz={0}    Sandra={1}", cc.Boz, cc.Sandra);
	
      }




    Console.WriteLine("Test59 finish.");
  }
  
}



// eof
