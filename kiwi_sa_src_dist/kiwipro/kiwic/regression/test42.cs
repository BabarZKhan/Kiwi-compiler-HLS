// Kiwi Scientific Acceleration.

// (C) 2015 DJ Greaves - University of Cambridge Computer Laboratory.
//
// Test42 : Subtrate Start/Stop/Error test and Programmed I/O multi-entry point slave.
//


// Under construction: this uses low performance HSIMPLE I/O protocol and does not report errors yet!


using System;
using System.Text;
using KiwiSystem;

class test42
{

	// Owing to a bug in KiwiC of Jan 2016 the keyword volatile needs adding here again - this was fixed before!

    static volatile int pio_slave_reg0 = 100;
    static volatile int pio_slave_reg1 = 444;
    static volatile int pio_slave_reg2 = 10000;

    [Kiwi.HardwareEntryPoint("BINARY-CONTROL")]
    static void Main() 
    {
        int [] testmem = new int [10000];

	System.Console.WriteLine("test42");
	Kiwi.Pause();
        System.Console.WriteLine("The value of the integer: {0}", 101);

	Kiwi.Pause();
	pio_slave_reg0 = 1024;
	pio_slave_reg1 = 1025;
	pio_slave_reg2 = 1026;
        while (true)
	{
          // Or else use   -kiwic-finish=disable
  	  Kiwi.Pause();
        }

    }


   [Kiwi.Remote("pioRegfileWrite")]
   public static void pioRegfileWrite(int addr, int data)
   {
     int wdata = data;
     Console.WriteLine("pioRegfileWrite addr={0} data=0x{1:X}.", addr, wdata);
     if (addr == 8) pio_slave_reg0 = wdata;
     if (addr == 16) pio_slave_reg1 = wdata;
     if (addr == 24) pio_slave_reg2 = wdata;
   }

   [Kiwi.Remote("pioRegfileRead")]
   public static int pioRegfileRead(int addr)
   {
     int rdata = 123456;
     if (addr == 8) rdata = pio_slave_reg0;
     if (addr == 16) rdata = pio_slave_reg1 + 256; // Some functionality !
     if (addr == 24) rdata = pio_slave_reg2;
     Console.WriteLine("pioRegfileRead addr={0} data=0x{1:X}.", addr, rdata);
     return rdata;
   }

}

// eof
