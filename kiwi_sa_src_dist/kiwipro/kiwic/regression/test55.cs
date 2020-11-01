// Kiwi Scientific Acceleration Example - C# closures
// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.


using System;
using System.Text;
using KiwiSystem;

public class test55
{

  static void t55_0()
  {
    Console.WriteLine("Kiwi Demo - Test55_0 starting - function delegate.");    
    Func<int, int, string> baz_topper = delegate(int var1, int var2)
      {	Console.WriteLine("  {1} {0} baz_topper", var1, var2);
	return "kandy";
      };

    Kiwi.Pause();
    for(int pp=0; pp<3; pp++)
      {
	Kiwi.Pause();
	string p = baz_topper(pp+1000, 50-pp);
	Console.WriteLine("  yielding {0}", p);
      }
    Console.WriteLine("t55_0 done.");
  }


  public static Func<int,int> GetAFunc()
  {
    var myVar = 1; // This is a dynamic free variable, or would be if this were not static ... enlarge test please.

    Func<int, int> inc = delegate(int var1)
      {
	Console.WriteLine("   ... GetAFun anonymous delegate body arg={0} fv={1}", var1, myVar);
	myVar = myVar + 1;
	return var1 + myVar;
      };
    return inc;
  }


  static void t55_1()
  {
    Console.WriteLine("Kiwi Demo - Test55_1 starting.");    
    var inc = GetAFunc();
    int a = inc(5);   // TODO reinsert these inline so int annotation does not cheat
    Console.WriteLine(a);
    int b = inc(6);   // without we have System.Exception: Dangling type var !!!1 encountered when supposedly grounded.
    Console.WriteLine(b);
    Console.WriteLine("Test55_1 done.");
  }


  static void t55_2()
  {
    Console.WriteLine("Kiwi Demo - Test55_2 starting - delegate pointer swapping.");    

    Action<int, string> boz_green = delegate(int var1, string var2)
      {	Console.WriteLine("  {1} {0} boz green", var1, var2);
      };

    Action<int, string> boz_red = delegate(int var1, string var2)
      {	Console.WriteLine("  {1} {0} boz red", var1, var2);
      };

    Kiwi.Pause();  // TODO - do all delegates currently have to be rezzed in lasso stem? If so, perhaps use always a coding style that puts them in the .ctor, but this may be relaxed now.

    for(int pp=0; pp<3; pp++)
      {
	Kiwi.Pause();
	boz_red(pp+100, "site1");
	boz_green(pp+200, "site2");
	var x = boz_red; boz_red = boz_green; boz_green = x;  // Swap delegates over
      }

    Console.WriteLine("Test55_2 done.");
  }


  [Kiwi.HardwareEntryPoint()]
  static void Main()
  {
    Console.WriteLine("Kiwi Demo - Test55 starting.");    
    t55_1(); // needs currently to run first since generates a closure heap item - in future KiwiC will handle that better we hope!
    t55_0();
    t55_2();
    Kiwi.Pause();
#if SPARE
    var voider = GetBFunc();
    voider(44);
#endif

  }

}





// eof


