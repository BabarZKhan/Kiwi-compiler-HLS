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

namespace KiwiSystem
{

   public class Interlocked // System.Threading.Interlocked is diverted here via wrappers/shims
   {

    // The read and write TLM method definitions ('properties' if you like) are inferred by KiwiC and do not need to be listed here.

    const string as1 = "SRI=1:FU=HPR_ATOMIC_ALU:MIRRORABLE=false:EIS=true";
    // The SRI=1 means that the location passed by ref to argument 1 becomes a shared resource FU rather than a simple RTL register.
    
    [KiwiSystem.Kiwi.HprPrimitiveFunction(as1)]
    public static Int64 hpr_atomic_add(ref Int64 arg, Int64 amount) // Atomic operation
    { // This function is implemented inside the KiwiC compiler.           
      arg += amount;
      return arg;
    }

     
    [KiwiSystem.Kiwi.HprPrimitiveFunction(as1)]
    public static int hpr_atomic_add(ref int arg, int amount) // Atomic operation
    { // This function is implemented inside the KiwiC compiler.           
      arg += amount;
      return arg;
    }

     
    [KiwiSystem.Kiwi.HprPrimitiveFunction("SRI=1:FU=HPR_ATOMIC_XCHG::EIS=true")]   // System.Threading.Interlocked is diverted here via wrappers/shims.
    public static Int64 hpr_exchange(ref Int64 location, Int64 value, Int64 comperand, bool conditional) // Atomic operation
    {
      // An implementation using lock  { } should compile fine under KiwiC. But for efficiency this function is implemented inside the KiwiC compiler.           

      Int64 result = location;
      if (!conditional || location == comperand) location = value;
      return result;
    }


    [KiwiSystem.Kiwi.HprPrimitiveFunction("SRI=1:FU=HPR_ATOMIC_XCHG:EIS=true")]   // System.Threading.Interlocked is diverted here via wrappers/shims.
    public static int hpr_exchange(ref int location, int value, int comperand, bool conditional) // Atomic operation
    {
      // An implementation using lock  { } should compile fine under KiwiC. But for efficiency this function is implemented inside the KiwiC compiler.           

      int result = location;
      if (!conditional || location == comperand) location = value;
      return result;
    }
   }
}



namespace System.Threading
{
  public class Interlocked
  {
   

    public static int Add(ref int arg, int amount) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_atomic_add(ref arg, amount);
    }

    public static int Increment(ref int arg) // Atomic operation wrapper
    {
      return Add(ref arg, 1);
    }

    public static int Decrement(ref int arg) // Atomic operation wrapper
    { 
      return Add(ref arg, -1);
    }


    public static Int64 Add(ref Int64 arg, Int64 amount) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_atomic_add(ref arg, amount);
    }

    public static Int64 Increment(ref Int64 arg) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_atomic_add(ref arg, 1L);
    }

    public static Int64 Decrement(ref Int64 arg) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_atomic_add(ref arg, -1L);
    }


    public static Int64 Exchange(ref Int64 location, Int64 value) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_exchange(ref location, value, 0L, false);
    }

    public static Int64 CompareExchange(ref Int64 location, Int64 value, Int64 comperand) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_exchange(ref location, value, comperand, true);
    }

    public static int Exchange(ref int location, int value) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_exchange(ref location, value, 0, false);
    }

    public static int CompareExchange(ref int location, int value, int comperand) // Atomic operation wrapper
    {
      return KiwiSystem.Interlocked.hpr_exchange(ref location, value, comperand, true);
    }
  }
}

// eof
