// Kiwi Scientific Acceleration
// (C) 2016 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test44 : Pipelined ALU and Synchronous RAM reads and writes in hard pause mode.
//          Hard and soft pause mode interworking.

// Notes:
// As of March 2016, dynamically changing the pause mode mid-thread is not tested and may
// not work.  As in        Kiwi.PauseControlSet(Kiwi.PauseControl.hardPauseEnable);
// Use the command line to set the default for now, as in -bevelab-default-pause-mode=hard


// Also -compose=disable is needed to avoid NameAlias

using System;
using System.Text;
using KiwiSystem;
using System.Threading;

// Note: finished at t=1.585us for 100 MHz clock without res2-pipelining enabled.

class test44
{
    [Kiwi.OutputBitPort("strobe")] static bool datastrobe = false;
    [Kiwi.OutputWordPort("databus")] static int databus;

    [Kiwi.OutputWordPort("finalbus")] static int finalbus;

    static volatile bool finished = false;

    class Writer
    {
      int [] SrcRam = new int[4096];
      
      public void generate()
      {
        Kiwi.PauseControlSet(Kiwi.PauseControl.softPauseEnable);
        databus = 1000;
        for (int iq=0; iq<10; iq++)  
	{
	  SrcRam[iq+100] = iq+10;
          Kiwi.Pause();
        }
      }

      public void transmit()
      {
        Kiwi.PauseControlSet(Kiwi.PauseControl.hardPauseEnable);
        for (int iq=0; iq<10; iq++)  
	{
          Kiwi.Pause();
	  datastrobe = true;
	  databus = SrcRam[iq+100] + 1000;
	  if (false && iq==5 || databus == 17) // PERHAPS Have a little stutter - tests control advance
          {
            Kiwi.Pause();
            datastrobe = false;
          }
        }
        Kiwi.Pause();
        datastrobe = false;
      }

    }

    class Reader
    {
      int [] DestRam = new int[4096];
      int dv = 0;

      public void ReceiveProcess()
      {
        int tid = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine("Receiver process started. Tid={0}", tid);
        Kiwi.PauseControlSet(Kiwi.PauseControl.hardPauseEnable);
        while(!finished)
	{
          Kiwi.Pause();
	  if (datastrobe)
          {
            int vv = databus;
	    int tt = (int)Kiwi.tnow; 
	    // Except for the stutters, these should have timestamps ONE clock cycle apart.
	    Console.WriteLine("Received data a={0} d={1} at {2}", dv, vv, tt);
            DestRam[dv++] = vv;
          }
        }
        Kiwi.Pause();
      }

     // If restructure is working more efficiently now (March 2016) we can stream hard pause mode data from a RAM one word per clock cycle.
     
      public void Increment()
      {
        // This was also giving : Unhandled Exception: meox+Name_alias: Exception of type 'meox+Name_alias' was thrown.
        Kiwi.PauseControlSet(Kiwi.PauseControl.softPauseEnable);
        //Kiwi.Pause(); // Needed to avoid 'NameAlias' spurious exception from KiwiC
	int no = dv; 
        Console.WriteLine("Increment {0} values", no);
	for (int qq=0;qq<no;qq++)
	{
            Kiwi.Pause();
	    // DestRam[qq] = DestRam[qq] + 100000;   // This CANNOT be done for synchronous RAMs in one clock cycle.
	    DestRam[qq+1] = DestRam[qq] + 100000; // This CAN be done for synchronous RAMs in one clock cycle.
        }
	//        Kiwi.Pause(); // Needed to avoid 'NameAlias' spurious exception from KiwiC
      }


      public void PrintOut()
      {
        Kiwi.PauseControlSet(Kiwi.PauseControl.softPauseEnable);
	int no = dv; 
        Console.WriteLine("PrintOut {0} values", no);
	for (int qq=0;qq<no;qq++)
	{
            Kiwi.Pause();
	    finalbus = DestRam[qq];
	    Console.WriteLine("Printout a={0} d={1}", qq, finalbus);
        }
        Kiwi.Pause();
      }
    }


    static void SetPhase(string pp)
    {  //Kiwi.KppMark(pp);
       System.Console.WriteLine(pp);
    }


    [Kiwi.HardwareEntryPoint()]
    static void Main() 
    {
        
	SetPhase("test44 start");
	Console.WriteLine("Note: this test will not run well under mono owing to lack of synchronisation between Writer and Reader when relying on hard pause mode.");
	Writer writer = new Writer();
        Reader reader = new Reader();
        Thread threadx = new Thread(new ThreadStart(reader.ReceiveProcess));
        threadx.Start();
        Kiwi.Pause();
        Kiwi.Pause();
        Kiwi.Pause();
	SetPhase("test44: Generating");
        writer.generate();
	SetPhase("test44: Sending");
        writer.transmit();

	SetPhase("test44: Printing");
	Kiwi.Pause();
	reader.PrintOut();
	SetPhase("test44: Increment in place");
	reader.Increment();
	reader.PrintOut();
	SetPhase("test44: Finished");

	//Ends after 158 clocks without repipeline in restructure2
	Console.WriteLine("test44 finished at {0}.", Kiwi.tnow);
	finished = true;
    }

}

// eof
