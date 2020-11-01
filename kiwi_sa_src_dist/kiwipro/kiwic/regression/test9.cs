// Kiwi Scientific Acceleration: KiwiC compiler test/demo.
// $Id: test9.cs $
//
// Test 9 - constant strings - The string "Hello World" written to parallel port.
//

using System;
using System.Text;
using KiwiSystem;


public class parallelPort
{

  // Note we would no-longer encourage this style of programming using Kiwi.
  // Instead the I/O device should be a static resource in the blade manifest, described in several XML files using IP-XACT,  and we would make a server RPC call to it using Kiwi.Remote() markups.

  // Note these nets are static, as is needed for I/O bit bangers, but the methods are instance methods which makes little semantic sense!
  // Since the static variables require no constructing, there is no class constructor .cctor for parallelPort.
  
  // Here are the nets of the traditional 8-bit asynchronous printer port (Centronix or Centronics). 
  // But to make it a little more interesting, we have a parit bit too.
    [Kiwi.OutputWordPort(8, 0)] static ushort dout;
    [Kiwi.OutputBitPort("strobe")] static bool strobe;
    [Kiwi.InputBitPort("ack")] static bool ack;

    public void putchar_2p(byte c)
	{
	  while (ack == strobe) Kiwi.Pause();
          bool parity = true; // Odd parity - means value 000 is not allowed.
          Kiwi.Pause();
          for (int i=0; i<8; i++)
          {
             if (((c >> i) & 1) != 0) parity ^= true;
          }
	  dout = (ushort)((uint)c | (parity ? (1u<< 8):0u));
	  Console.Write(" " + c + "  ");
	  Kiwi.Pause();
	  strobe = !strobe;
	}

    public void putchar_4p(byte c)
	{
	  while (ack) Kiwi.Pause();
          bool parity = true; // Odd parity - means value 000 is not allowed.
          Kiwi.Pause();
          for (int i=0; i<8; i++)
          {
             if (((c >> i) & 1) != 0) parity ^= true;
          }
	  dout = (ushort)((uint)c | (parity ? (1u<< 8):0u));
	  strobe = true;
	  Console.Write(" 0x{0:X} ", (char)c);
	  Kiwi.Pause();
	  while (!ack) Kiwi.Pause();
	  strobe = false;
	}
}

//
// Test indexing a string directly.  System.String.ToCharArray is not invoked?


class test9
{
    static parallelPort the_pport = new parallelPort(); // Having parallelPort non-static is a bit odd actually. The I/O nets are static.

    public static void parallel_print(string ss)
    { 
      for(int i = 0; i<ss.Length; i++)
// Verilog and SystemC has 8-bit chars but C# and dotnet have 16 bit chars.
// We need to cast to byte to effect the conversion, post indexing of the string array.
           the_pport.putchar_4p((byte)ss[i]);
    }

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
	Console.WriteLine("\nTest9 start - note this does not exit currently under diosim.");
	parallel_print("Hello World\n");
	Console.WriteLine("\nDone");
    }
}

// eof
