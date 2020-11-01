// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
//
// Initialisation interval.
// 
// Loopclassic.cs


using System;
using System.Text;
using KiwiSystem;

/*

         

*/


class LoopClassic
{

  public static int associative_reduction_example(int starting)
  {
    int vr = 0;
    for (int i=0;i<5;i++)
      {
	int vx = (i+starting)*(i+3)*(i+5);
	vr ^= ((vx & 128)>0 ? 1:0);
      }
    return vr;
  }

  public static int loop_carried_example(int seed)
  {
    int vp = seed;
    for (int i=0;i<5;i++)
      {
	vp = vp * 31241/121;
      }
    return vp;
  }

  
  static int [] foos = new int [10];
  static int ipos = 0;
  public static int loop_forwarding_example(int newdata)
  {
    foos[ipos ++] = newdata;
    ipos %= foos.Length;
    int sum = 0;
    for (int i=0;i<foos.Length-1;i++)
      {
	sum += foos[i]^foos[i+1];
      }
    return sum;
  }


  public static int data_dependent_controlflow_example(int seed)
  {
    int vr = 0;
    int i;
    for (i=0;i<20;i++)
      {
	vr += i*i*seed;
	if (vr > 1111) break;
      }
    return i;
  }
}



public class ParkMillerGenerator
{
  int seed;
  public void Seed(int s)
  {    
    seed = s;
  }
  
  public ParkMillerGenerator(int s)
  {   
    seed = s;
  }


  public int Next()   // PRGEN Park and Miller CACM October 1988
  {    
    const int a = 2147001325;
    const int b = 715136305;
    seed = a * seed + b;
    //Console.WriteLine(" randval {0}", seed);
    Kiwi.NoUnroll(); 
    return seed;
  }
}


// Bubblesort is a good test for loop-forwarding and other redundant reads
class SortingHat
{
  int [] dara;
  ParkMillerGenerator rand = new ParkMillerGenerator(1);

  public SortingHat(int n)
  {
    dara = new int [n];
  }

  public void DataGen()
  {
    for (int i=0; i<dara.Length; i++) dara[i] = rand.Next();
  }


  public void PrintOut()
  {
    for (int i=0; i<dara.Length; i++)
      {
        Console.WriteLine("     Sorted {0} is {1}", i, dara[i]);
        if (i > 10 && i < dara.Length-10)
          {
            Console.WriteLine("   ... snip ...");
            i = dara.Length-10;
          }
      }
  }

  int swaps = 0;

  private void Swap(ref int a, ref int b)
  { swaps ++;
    int temp = a;
    a = b;
    b = temp;
  }

  public int BubbleSort()
  {
    int iterations = 0;
    swaps = 0;
    while(true)
      {
        bool changes = false;
        iterations += 1;
        //for (int i=0; i<dara.Length-1; i++) Console.WriteLine("   sort tracking {0} is {1}", i, dara[i]);
        for (int i=0; i<dara.Length-1; i++)
          { 
            if (dara[i] > dara[i+1]) 
              { Swap(ref dara[i], ref dara[i+1]);                
                changes = true;
              }
          }
        if (!changes) break;
      }
    Console.WriteLine("BubbleSort finished with {0} iterations and {1} swaps.", iterations, swaps);
    return 0;
  }


  // Iterative QuickSort from www.programmingalgorithms.com/algorithm/quick-sort-iterative
  public int QuickSort()
  {
    int startIndex = 0;
    int endIndex = dara.Length - 1;
    int top = -1;
    int[] stack = new int[dara.Length];
    swaps = 0;
    
    stack[++top] = startIndex;
    stack[++top] = endIndex;
    
    while (top >= 0)
      {
        endIndex = stack[top--];
        startIndex = stack[top--];
        int p = QsPartition(startIndex, endIndex);
        if (p - 1 > startIndex)
          {
            stack[++top] = startIndex;
            stack[++top] = p - 1;
          }
        if (p + 1 < endIndex)
          {
            stack[++top] = p + 1;
            stack[++top] = endIndex;
          }
      }
    Console.WriteLine("QuickSort finished after {0} swaps.", swaps);
    return 0;
  }
  
  private int QsPartition(int left, int right)
  {
    int x = dara[right];
    int i = (left - 1);
    for (int j = left; j <= right - 1; ++j)
      {
        if (dara[j] <= x)
          {
            ++i;
            Swap(ref dara[i], ref dara[j]);
          }
      }
    Swap(ref dara[i + 1], ref dara[right]);
    Console.WriteLine("QsPartition left={0} axant={1} right={2}", left, i, right);
    return (i + 1);
  }
  

  public int Check()
  {
    int viols = 0;
    for (int i=0; i<dara.Length-1; i++) 
      {
        if (dara[i] > dara[i+1]) viols ++;
      }
    Console.WriteLine("SortingHat Checker: violations={0}", viols);
    return viols;
  }

}



class bench
{
  const int zier = 1024;
  static SortingHat bub = new SortingHat(zier);

  static int SortingTest(int quickf)
  {
    bub.DataGen();
    ulong startTime = Kiwi.tnow;
    Console.WriteLine("Sort arity={1} start at {0}", startTime, zier);
    if (quickf>0) bub.QuickSort(); else bub.BubbleSort();
    Console.WriteLine("Sort arity={1} quick={2} finished after {0}", Kiwi.tnow-startTime, zier, quickf);
    bub.PrintOut();
    int vils = bub.Check();
    Console.WriteLine("Sort finished and checked. Violations={0}", vils);
    return 0;
  }

  const int limit = 100000000;  // Some stopping value


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("LoopClassic Demo Start");
    int jojo = 3;
    int result;

    //    for (int jojo=3;jojo<12;jojo += 3) 
//    {
//      Kiwi.NoUnroll(); 
// It is wrong to have waypoints within a loop - they need to be program articulation points.
// #if SPARE
        Kiwi.KppMark(1, "START");
        Kiwi.KppMark(2, "START-LOOP-CARRIED");
	result = LoopClassic.loop_carried_example(jojo);
        Console.WriteLine("   LCD jojo={0}  test result={1}", jojo, result); 

        Kiwi.KppMark(3, "START-DATADEP");
	result = LoopClassic.data_dependent_controlflow_example(jojo);
        Console.WriteLine("   DATADEP jojo={0}  test result={1}", jojo, result); 


        Kiwi.KppMark(4, "START-LOOPFWD");
	result = LoopClassic.loop_forwarding_example(jojo);
        Console.WriteLine("   LOOPFWD jojo={0}  test result={1}", jojo, result); 


        Kiwi.KppMark(5, "START-ASSOCRED");
	result = LoopClassic.associative_reduction_example(jojo);
        Console.WriteLine("   ASSOCRED jojo={0}  test result={1}", jojo, result); 


        Kiwi.KppMark(6, "START-SORT0");
        result = SortingTest(0);
//#endif
        Kiwi.KppMark(7, "START-SORT1");
        result = SortingTest(1);

        

//      }  
        Kiwi.KppMark(8, "FINISH");
    Console.WriteLine(" Test LoopClassic finished.");
    Kiwi.ReportNormalCompletion();
    Kiwi.Pause();
    Kiwi.Pause();
  }
}

// eof



