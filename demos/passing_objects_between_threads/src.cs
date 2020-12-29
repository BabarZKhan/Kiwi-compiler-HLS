// (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met: redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer;
// redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution;
// neither the name of the copyright holders nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.



// The Capsule
// This is the item passed. A static number is generated in this example 
// but see the linked-list example for dynamic heap coding styles.



class Capsule // Capsules are passed over the hardware channel between threads.
{
  public bool newlinef;
  public int capint;

  public Capsule(int myval)
        { capint = myval;
          newlinef = false;
        }
}



// The Forked Process
// This independent process does some work and 
// returns the capsules to the main thread on a reverse FIFO channel.





class ConsumerClass
{   
    Kiwi.Channel<Capsule> workin, empties;

    public ConsumerClass(Kiwi.Channel<Capsule> fwd, Kiwi.Channel<Capsule> rev) // constructor
    {  
       workin = fwd;  empties = rev;
       Capsule cap2 = new Capsule(45);
       empties.Write(cap2);
    }

    public void process()
    { 
      Kiwi.Pause();
      for (int count=0; count < 10; count ++)
        {   
            Capsule rat = workin.Read();
            Console.Write("{0} ", rat.capint);
            if (rat.newlinef) Console.WriteLine("");
            Kiwi.Pause();
            empties.Write(rat);
        }
    }
}



//The FIFO Channel Definition
//The FIFO is defined in C# and separately compiled to a 
//dll file using the C# compiler.



public class Channel<T>
        {
            T datum;
            volatile bool emptyflag = true;

            public void Write(T v)
            {
                lock (this)
                {
                    while (!emptyflag) { Kiwi.NoUnroll(); Monitor.Wait(this); }
                    datum = v;
                    emptyflag = false;
                    Monitor.PulseAll(this);
                }
            }

            public T Read()
            {
                T r;
                lock (this)
                {
                    while (emptyflag) { Kiwi.NoUnroll(); Monitor.Wait(this); }
                    emptyflag = true;
                    r = datum;
                    Monitor.PulseAll(this);
                }
                return r;
            }

        }
        
 
// The main program kicks off the child processes using the same 
// primitives as on dot net. Kiwi requires that the number of threads 
// used is static (all created before EndOfStaticElaboration). 
// Here we kick off (thread1.Start) the thread from the outset, 
// but KiwiC can support the thread not being started until later in the run. 
        
        
        
        
        
       
// Print the times table by sending messages in capsules over a channel.
class test2r1
{
    static int limit = 3;
    static Capsule cap1 = new Capsule(23);
    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
        int i, j;
        Console.WriteLine("Test2r1: Times Table Capsule Channel Test: limit=" + limit);
        Kiwi.Channel<Capsule> fwd = new Kiwi.Channel<Capsule>();
        Kiwi.Channel<Capsule> rev = new Kiwi.Channel<Capsule>();

        ConsumerClass consumer = new ConsumerClass(fwd, rev);

        Thread thread1 = new Thread(new ThreadStart(consumer.process));
        thread1.Start();

        Capsule capx = cap1;

        for (i = 1; i <= limit; i++)
        {
            for (j = 1; j <= limit; j++)
                {
                   // Must not do allocs inside runtime loops pre Kiwi2
                   //Capsule cap = new Capsule(i*j);
                   capx.capint = i*j;
                   capx.newlinef = (j == limit);
                   fwd.Write(capx);
                   capx = rev.Read();
                }
           Kiwi.Pause();
        }
        Console.WriteLine("\nTest2r1: Capsule Times Table Test: Finished\n");
    }
}        
