// Kiwi Scientific Computing Regression Tests
// Test 35 : structure assigns

using System;
using System.Text;
using KiwiSystem;

// C# passes structs by value to a method, meaning local modifications to contents do not commit to original instance.
// C# assigns structs by value, so all fields in the destination are updated by the assigment, rather than the handle just being redirected.

// Also setters and getters.

// Method updates to structs


struct SimpleStruct
{
  int filler_uno;
  private int struct_impl_vale;
  private long filler_duo;

  public int XX
    {
       set 
        {
            if (value < 100)
                struct_impl_vale = value;
        }
    }

    public void DisplayXX(string msg)
    {
      Console.WriteLine("{0} The stored struct value is: {1}", msg, struct_impl_vale);
    }
}


class SimpleClass
{
  int filler0;
  private int cls_impl_vale = 202;
  int filler1;

  public int YY
  {
    get 
        {
            return cls_impl_vale;
        }
    set 
      {
	if (value < 100) cls_impl_vale = value;
      }
  }
  
  public void DisplayYY(string msg)
  {
    Console.WriteLine("{0} the stored class value is: {1}", msg, cls_impl_vale);
  }
}


class TestClass
{
  
  void local_mutator_ss(SimpleStruct ss)
  {
    ss.XX = 89;    // Invoke the setter for the struct's field.
    ss.DisplayXX("L72");
  }

  void local_mutator_cc(SimpleClass cc)
  {
    cc.YY = 99;    // Invoke the setter of the field
    cc.DisplayYY("L78");
  }
  
  public void Main()
  {
        SimpleStruct ss = new SimpleStruct();
        SimpleClass cc = new SimpleClass();
	Kiwi.Pause();
        ss.XX = 5; // Invoke the setter
        ss.DisplayXX("L84");
        ss.XX = 50; 
        ss.DisplayXX("L86");
        ss.XX = 500; // Invoke the setter - should not update it to 500 owing to 100 limit.
        ss.DisplayXX("L88");

	local_mutator_ss(ss);
        ss.DisplayXX("L91");  // Struct should print unupdated since passed by value to the mutator.

	local_mutator_cc(cc);
        cc.DisplayYY("L94");  // Class, however, should print updated to 99 since passed by reference to the mutator.
  }
}



class Test35
{
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        Console.WriteLine("Test35 start.");
	TestClass tcc = new TestClass();
	tcc.Main();
        Console.WriteLine("Test35 end.");
     }	

}

// eof
