// Kiwi Scientfic Acceleration
// (C) 2016, DJ Greaves, University of Cambridge Computer Laboratory.

// Test52.cs - String IO - bin2ascii etc..  - Please enhance this to show how to connect to UART, LCD panel, framestore or substrate gateway.


using KiwiSystem;
using System;


class Test52
{
  
  public void BasicIntegerTest()
  {
      KiwiStringIO.Write("Test 52 from Kiwi"); 
      KiwiStringIO.NewLine(); 

      KiwiStringIO.Write("Test A "); 
      KiwiStringIO.Write(0);
      KiwiStringIO.NewLine(); 

      KiwiStringIO.Write("Test B "); 
      KiwiStringIO.Write(-321);
      KiwiStringIO.NewLine(); 

      KiwiStringIO.Write("Test D "); 
      KiwiStringIO.Write(86420012);
      KiwiStringIO.WriteLine();

      KiwiStringIO.WriteLine("Test52 fin.\n"); 
      KiwiStringIO.WriteLine("Test52 Done.\n"); 
  }
}



public class main
{

  [Kiwi.HardwareEntryPoint()]   // For the RTLSIM and FPGA execution environments.

  public static void HwProcess()
  {
    Test52 tester = new Test52();
    Console.WriteLine("Tester52 Demo Start");
    Kiwi.KppMark(1, "START");
    tester.BasicIntegerTest();
    Console.WriteLine("Tester52 Demo Finished");
    Kiwi.Pause();
    Kiwi.KppMark(2, "END");
  }

  // For [WD] execution, Software entry point.
  public static int Main()
  {
    HwProcess();
    return 0;
  }

}


// eof
