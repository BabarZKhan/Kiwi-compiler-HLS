// Kiwi Scientific Acceleration
//
// tinytot.cs
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;

class tinytot
{
 
   class fish { int [] q; public int x; };

   //static fish bog1 = new fish();

    //static char [] bog0 = new char [32];

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
       fish bog1 = new fish();

       int [] q = new int [32];
       q[q[2]] = 3;

       bog1.x = 21;
    }
}

// eof



