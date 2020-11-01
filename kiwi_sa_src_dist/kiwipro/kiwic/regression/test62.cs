// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// test62.cs
//
// Example from MSDN web page about C# 'as' and 'is' operators that uses Object.GetType() as well.
// 

using System;
using System.Text;
using KiwiSystem;



class SafeCasting
{
  
  class Animal
  {
    
    public void Eat()
    {
      Console.WriteLine("Eating."); 

    }

    public override string ToString()
    {
      return "I am an animal.";
    }
  }
  
  class Mammal : Animal
  {

  }

  class Giraffe : Mammal
  {

  }
  
  class SuperNova
  {
  }

  public static void Run()
  {
    
    SafeCasting app = new SafeCasting();
    
    // Use the is operator to verify the type.
    // before performing a cast.
    Giraffe g = new Giraffe();
    app.UseIsOperator(g);
    
    // Use the as operator and test for null before referencing the variable.
    app.UseAsOperator(g);
    
    // Use the as operator to test an incompatible type.
    SuperNova sn = new SuperNova();
    app.UseAsOperator(sn);
    
    if (false)
      {
        
        // Use the as operator with a value type.
        // Note the implicit conversion to int? in  the method body.
        int i = 5;
        app.UseAsWithNullable(i);
        
        double d = 9.78654;
        app.UseAsWithNullable(d);
         }
  }
  
  void UseIsOperator(Animal a)
  {
    
    if (a is Mammal)
      {
        
        Mammal m = (Mammal)a;
        Console.WriteLine("Mammal 'is' holds");
        m.Eat();
      }
    else Console.WriteLine("Mammal is returned false");
  }
  
  void UseAsOperator(object o)
  {
    
    Mammal m = o as Mammal;
    if (m != null)
      {
        Console.WriteLine("Use as Mammal: gives "  + m.ToString());
      }
    else
      {
        Console.WriteLine("{0} is not a Mammal", o.GetType().Name);
      }
  }
  
  void UseAsWithNullable(System.ValueType val)
  {
    
    int? j = val as int?;
    if (j != null)
      {
        Console.WriteLine("UseAsWithNullable worked with " + j);
      }
    else
      {
        Console.WriteLine("Could not convert " + val.ToString());
      }
  }
}

class bench
{
  const int limit = 100000000;

  [Kiwi.OutputBitPort("done")] static bool done = false;

  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test62 starting.");
    SafeCasting.Run();
    Console.WriteLine("Test62 finished.");
    done = true;
    Kiwi.Pause();
  }
}

// eof