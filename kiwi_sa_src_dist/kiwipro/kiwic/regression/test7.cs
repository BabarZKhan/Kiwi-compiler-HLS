// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// $Id: test7.cs,v 1.3 2010/11/29 11:03:38 djg11 Exp $
//
// Test 7 - Overloading, overriding and object calling with polymorphic dynamic dispatch.
//

using System;
using System.Text;
using KiwiSystem;

interface boozy_IF
{
  int subber(int xx, int yy);
}


class test7upper : boozy_IF
{
  int upper7_field_a,  upper7_field_b;
  public virtual int adder(int vv)
  {
    Console.WriteLine("Test7 upper adder vv={0}", vv);
    return vv + 1002;
  }
  
  public int subber(int xx, int yy)
  {
    return xx - yy - 4;
  }
}


class test7lower : test7upper
{
  int lower7_field_a,  lower7_field_b; // TODO use these fields
  public override int adder(int vv)
  {
    Console.WriteLine("Test7 lower override adder vv={0}", vv);
    return vv + 102;
  }

  public new int subber(int xx, int yy)
  {
    return xx - yy - 204;
  }

  // Method overloading: Here we have two bodies, one with and one without nlf.
  public string edict(int k, int mm)
  {
    Console.Write(k + " ");
    return "+";
  }

  public string edict(bool nlf, int k, int mm)
  {
    Console.Write(k + " ");
    if (nlf) Console.WriteLine(" BAR");
    return "+";
  }
}



class test7fv_module
{
  public int fv = 2;

  public int incbyfv(int x) 
  {
    fv += 1;
    return x+fv+100;
  }
}


public class tester7
{

  public static void test7_nulltest() 
  {
    Console.WriteLine("test7_fv nulltest start");   // Null pointer test.
    var ax1 = new test7fv_module();

    for (int i=0; i<3; i++)
      {
        var ax2 = (i==1) ? null: ax1;
        Kiwi.Pause();
        if (ax2 != null) Console.WriteLine("Computed {0}", ax2.incbyfv(i));
        else Console.WriteLine("Was null for {0}", i);
      }

    Console.WriteLine("test7_fv nulltest done");
  }

  // TODO: the test does not call edict with three args where the first is false...
  public static void test7_fv()
  {
    Console.WriteLine("\nTest7 fv test start.");
      // TODO: the test does not call edict with three args where the first is false...
    var ax1 = new test7fv_module();
    var ax2 = new test7fv_module();

    ax2.fv = 400;
  
    for (int qq=0;qq<4;qq++)
      {
	Kiwi.Pause();
	test7fv_module aax = (qq==2) ? ax2:ax1;
	
	int a1 = aax.incbyfv(1);
	Console.WriteLine("   test7_fv {0} result {1}", qq, a1);
      }
    Console.WriteLine("test7_fv done");
  }

  static int limit = 8;
  
  // TODO: the test does not call edict with three args where the first is false...
  public static void test7_overloads()
  {
    Console.WriteLine("\nTest7 overloads test start.");
    test7lower t7l = new test7lower();
    test7upper t7m = (test7upper)(new test7lower());
    test7upper t7u = new test7upper();
    
    int i, j;
    Console.WriteLine("Test7_overrides Up To " + limit);
    for (i=1;i<=limit;i++)
      {
	for (j=1;j<=limit;j++)
	  if (j==limit-1) t7l.edict(true, i*j, 40);
	else t7l.edict(i*j, 32);
	//     	  Console.WriteLine(i + " deb");
	Kiwi.Pause();
      }
    Console.WriteLine("Test7 method overload test done");
  }

  public static void test7_static_overrides()
  {
    Console.WriteLine("\nTest7 static override test start.");
    test7lower t7l = new test7lower();
    test7upper t7m = (test7upper)(new test7lower());
    test7upper t7u = new test7upper();
    
    for (int i=0;i<2;i++)
      { 
	Kiwi.Pause();
	int pl = t7l.adder(i);
	int pm = t7m.adder(i);
	int pu = t7u.adder(i);
	Console.WriteLine("Test7 static override i={0} l={1}", i, pl);	  
	Console.WriteLine("      static override i={0} m={1}", i, pm);	  
	Console.WriteLine("      static override i={0} u={1}", i, pu);	  
      }
    Console.WriteLine("Test7 static override test done");
  }


  public static void test7_dyn_overrides()
  {
    Console.WriteLine("\nTest7 dynamic override test start.");
    test7lower t7l = new test7lower();
    test7upper t7m = (test7upper)(new test7lower());
    test7upper t7u = new test7upper();
  
    for (int i=0;i<3;i++)
      { 
	Kiwi.Pause();
	test7upper xx = (i==0)?t7l:(i==1)?t7m:t7u;
	Kiwi.Pause();
	int px = xx.adder(10);
	Console.WriteLine("Test7 dynamic i={0} px={1}", i, px);	  
      }
    Console.WriteLine("Test7 dynamic override test done");
    
  }


  // TODO: the test does not call edict with three args where the first is false...
  // TODO: demonstrate null-pointer runtime error in abend syndrome register.
  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    test7_nulltest();
    test7_overloads();
    test7_static_overrides();
    test7_dyn_overrides();
    test7_fv();
  }
}

// EOF
