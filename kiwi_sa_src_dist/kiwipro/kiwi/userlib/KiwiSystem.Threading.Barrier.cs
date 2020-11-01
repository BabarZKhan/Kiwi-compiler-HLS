//
// Kiwi Scientific Acceleration.
// (C) 2007-17 DG Greaves, University of Cambridge, Computer Laboratory.
//

// A set of stubs and shims.
// 1. The Kiwi library substitutions mechanism will redirect System.Threading.Interlocked to KiwiSystem.Threading.Interlocked (this file)
// 2. The shims in here invoke the hpr stubs in here.
// 3. KiwiC traps the stubs and maps them to TLM calls on shared variable FUs.
// 4. Recipe stage restructure invokes the RPCs at the net level.

using System;
using System.Threading;
using System.Threading.Tasks;

using KiwiSystem;

namespace System.Threading
{
   
    public class Barrier
    {
      int participantCount;
      int counter = 0;
      bool polflag = false;

      int dispatched = 0;
      public Barrier(int participantCount) // Constructor 1
      {
        this.participantCount = participantCount; 
      }

      public Barrier(int participantCount, Action<System.Threading.Barrier> postPhaseAction) // Constructor 2
      {
        this.participantCount = participantCount;         
      }

      public void AddParticipants(int no)
      {
        participantCount += no;
      }

      public void RemoveParticipants(int no)
      {
        participantCount -= no;
      }


      public long CurrentPhaseNumber {
             get { return counter; }
       } // Property

      void kiwi_barrier(bool local_sense)
      {

        local_sense = !local_sense;
        Monitor.Enter(this);
        counter++;
        int arrived = counter;
        if (arrived == participantCount) // last arriver sets flag
        {
          Monitor.Exit(this);
          Monitor.PulseAll(this);

          // if (postPhaseACtion) invoke postPhaseAction();
          counter = 0;
          // Here: memory fence to ensure that the change to counter is seen before the change to flag
          polflag = local_sense;
        } 
        else
        {
          Monitor.Exit(this);
          while (polflag != local_sense); // spin waiting for flag
        }

        Interlocked.Add(ref dispatched, 1);
      }

      public void SignalAndWait()
      {
         // A better API is for the user to keep its own local_sense and to call it alternately each way up
        kiwi_barrier(true);
        kiwi_barrier(false);                
      }
    }
}


// eof
