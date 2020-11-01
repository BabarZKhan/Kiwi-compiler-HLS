// Kiwi Scientific Acceleration
// Test53 - mutable formals and register reuse at different stack depths


using System;
using System.Text;
using KiwiSystem;


class Test53
{

  public static void modder(int ax, int by)
  {
    Console.WriteLine(" modder entry  {0} {1}", ax, by);
    Kiwi.Pause();
    ax += 2;
    by += 1;
    Console.WriteLine(" modder exit   {0} {1}", ax, by);
  }
  public static void t53a()
  {

    for (int qon=10; qon<12; qon++)
      {
        modder(qon, qon+100);
        Kiwi.Pause();
      }
    Console.WriteLine("Test53a finished.");
    Kiwi.Pause();
  }  

  static void bottom(int p0, int bound)
  {
    int p = p0;
    Kiwi.Pause();
    for (int kk = 0; kk <2; kk++)
      {
        p = p + 1;
        Kiwi.Pause();
        Console.WriteLine("bottom subroutine {0} {1}", p, bound);
      }
  }

  static void middle(int p, int bound)
  {
    Console.WriteLine("middle subroutine {0} {1}", p, bound);
    bottom(p*10+1, bound-1);
    Kiwi.Pause();
  }


  public static void t53b()
  {

    Kiwi.Pause();
    bottom(2,2);
    middle(2,2);
    Kiwi.Pause();
    bottom(20,2);
    middle(20,2);

    Console.WriteLine("Test53b finished.");
    Kiwi.Pause();
  }  


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test53 start.");
    t53a();
    t53b();
    Console.WriteLine("Test53 finished.");
    Kiwi.Pause();
  }  
}
// eof



