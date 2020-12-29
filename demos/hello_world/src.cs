
// Hello world written to parallel port.
using System; //NameSpace
public class parallel_port
{
    [Kiwi.OutputWordPort("")] static byte dout;
    [Kiwi.OutputBitPort("")] static bool strobe;
    [Kiwi.InputBitPort("")] static bool ack;

    public static void putchar(byte c)
	{
	  while (ack == strobe) Kiwi.Pause();
	  dout = c;
	  Kiwi.Pause();
	  strobe = !strobe;
	}

}

class test9
{
    public static void parallel_print(string s)
    { 
      for(int i = 0; i<s.Length; i++)
           parallel_port.putchar((byte)s[i]);
    }

    public static void Main()
    {
	parallel_print("Hello World\n");
    }
}
// eof
