// Kiwi Scientific Acceleration - Regression test: Virtex-like native FIFO demo.
//
// (C) 2017 DJ Greaves - University of Cambridge Computer Laboratory.
//

// The third one of these three illustrates the  envisioned typical end-user use case.

// test72r2 - Synthesise our own FIFO inline
// test72r2 - Generate our own FIFO as a separate block and manually replace with hardened one.  This cannot have a generic datatype parameter.
// test72r3 - Use hardened one from Kiwi distribution.


using System;
using System.Text;
using KiwiSystem;

//
// TODO explain syncnronous and asynchronous
//

// This FIFO high-level model should not be synthesised with KiwiC.
// It is much better to manually replace it with a hardend component from the IP Vendor's Library.
public class KiwiIntFifo
{

  int capacity;
  int low_threshold;
  int high_threshold;
  
  volatile int entries;
  volatile int in_ptr;
  volatile int out_ptr;

  /*DT*/int [] store;
  
  public KiwiIntFifo(int capacity) // Constructor
  {
    entries = 0;
    in_ptr = 0;
    out_ptr = 0;
    this.capacity = capacity;
    this.low_threshold = capacity / 8;
    this.high_threshold = 7*(capacity / 8);
    store = new /*DT*/int [capacity];
  }

  [ Kiwi.Remote() ]
  public void Queue(/*DT*/int arg)
  {
    if (entries < capacity)
      lock(this)
        {
          store[in_ptr++] = arg;
          entries += 1;
          if (in_ptr == capacity) in_ptr = 0;
        }
  }

  [ Kiwi.Remote() ]
  public /*DT*/int DeQueue()
  {
    /*DT*/int rr;
    if (entries > 0)
      lock(this)
        {
          rr = store[out_ptr++];
          entries -= 1;
          if (out_ptr == capacity) out_ptr = 0;
        }
    else rr = store[out_ptr];// Spurious under-run result.
    return rr;
  }

  public bool IsFull()
  {
    return (entries==capacity);
  }

  public bool IsNearlyFull()
  {
    return (entries>=high_threshold);    
  }

  public bool IsEmpty()
  {
    return (entries==0);
  }

  public bool IsNearlyEmpty()
  {
    return (entries < low_threshold);    
  }
  
}



class Test72r2
{
  static int NativeFifoTest()
  {
    KiwiIntFifo the_fifo = new KiwiIntFifo(100);
    Kiwi.Pause();

    for (int qq=0; qq<10; qq++)
      {
        the_fifo.Queue(qq+101);
        Console.WriteLine("Enqueue step {0}", qq);
        Kiwi.Pause();        
      }

    int ss = 0;
    for (int qq=0; qq<10; qq++)
      {
        int rr = the_fifo.DeQueue();
        ss += rr;
        Console.WriteLine("Dequeued {0}, sofar {1}", rr, ss);
        Kiwi.Pause();        
      }

    return ss;
  }

  
  [Kiwi.HardwareEntryPoint()]
  public static void Main()
     {
       Kiwi.Pause();
       Console.WriteLine("Start NativeFifo 72r2 Test");
       int rc = NativeFifoTest();
       Kiwi.Pause();
       Console.WriteLine("NativeFifo 72r2 Test: End of Demo. rc={0}", rc);      
       Kiwi.Pause();
       Kiwi.ReportNormalCompletion(); // KiwiC should perhaps pop this on the end automatically?
    }

}


// eof
