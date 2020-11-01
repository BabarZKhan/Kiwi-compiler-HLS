// 
// Kiwi Scientfic Acceleration
// (C) 2010, DJ Greaves, University of Cambridge Computer Laboratory.
// 
// 
using System;

public class KiwiStringIO
{

  public static void WriteLine()
  {
    NewLine();
  }

  public static void NewLine()
  {
    StringIO_WrCh('\n'); // Unix newline char. No DOS compatibility here.
  }

  public static void WriteLine(string ss)
  {
    Write(ss);
    NewLine();
  }

  public static void Write(string ss)
  {
    for (int i=0; i< ss.Length; i++)
      {
	StringIO_WrCh(ss[i]);
      }

  }
  
  // -repack-to-roms=disable is prefereable for this (used to default to disable.)
  static uint [] tens_table = new uint []
    { 1,                    // 10^0
      10,                   // 10^1
      100,                  // 10^2 - max power for int8
      1000,                 // 10^3
      10000,                // 10^4 - max power for int16
      100000,               // 10^5
      1000000,              // 10^6
      10000000,             // 10^7
      100000000,            // 10^8
      1000000000,           // 10^9 : 0x3b9aca00  1000 000 000 - max power for int32
 };


  public static void Write(int i0) 
  {
    if (i0 < 0) 
      {
	StringIO_WrCh('-');
	WriteU((uint)(-i0));
      }
    else WriteU((uint)i0);
  }

  public static void Write(Int16 i0) 
  {
    if (i0 < 0) 
      {
	StringIO_WrCh('-');
	WriteU((uint)(-i0), 4);
      }
    else WriteU((uint)i0, 4);
  }

  public static void Write(System.SByte i0) 
  {
    if (i0 < 0) 
      {
	StringIO_WrCh('-');
	WriteU((uint)(-i0), 2);
      }
    else WriteU((uint)i0, 2);
  }

  public static void Write(System.Byte i0) 
  {
    WriteU((uint)i0, 2);
  }

  public static void Write(System.UInt16 i0) 
  {
    WriteU((uint)i0, 4);
  }

  public static void Write(System.UInt32 i0) 
  {
    WriteU((uint)i0, 9);
  }

  public static void WriteU(uint dd, int maxpower=9)  // UInt32.toString method (int2ascii).
  {
    uint d0 = dd;
    if (d0 == 0)
      {
	StringIO_WrCh('0');
      }
    else
      {
	int p0 = 0;
	while (tens_table[p0] <= d0 && p0 < maxpower) p0 ++;
	p0 --;
	// Console.WriteLine("  WriteU {0} Prescan p0={1}", dd, p0);
	while (p0>=0)
	  { uint d = d0 / tens_table[p0];     // We know these divides will result in a number 0-9 and so they can be done in a very small no of clock cycles.
	    d0 = d0 - d * tens_table[p0];
	    // Console.WriteLine("  WriteU {0} print p0={1} d={2}", d0, p0, d);
	    StringIO_WrCh((char)(d + '0'));
	    p0 --;
	  }
      }
  }


  // This will, soon, redirect at this point to enable UART, LCD, Framestore, filesystem and other output devices addressable
  // KiwiFiles.KiwiRemoteStreamServices.perform_op(Kiwi_fs_putchar, (UInt64) d);
  // Or to be : [Kiwi.Remote("parallel:four-phase")]  // Replaced with an external implementation since attributed with KiwiRemote.
  public static void StringIO_WrCh(char cc)
  {
    // KiwiC generates RTL that prints as a decimal from the former form, which is not what we want.
    // String.Write(c);  
    char c1 = (char)(cc & 0xFF);
    Console.Write("{0}", c1); 
  }

}


// eof

