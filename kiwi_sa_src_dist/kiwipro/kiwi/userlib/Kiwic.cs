//
//
// Kiwi Scientific Acceleration stubs library.
// (C) 2007-17 DG Greaves, University of Cambridge, Computer Laboratory.
//
//
// This file defines KiwiSystem.Kiwic - a standard preamble. 
// The .dll form of the code in this file is read in by the KiwiC compiler to emulate the parts of the dotnet infrastructure supported by Kiwi. 
//
// This file should NOT be used when compiling application code (whether the output from such compilations is fed to KiwiC or executed on a VM such as mono).  The file Kiwi.dll, that contains Kiwi-specific extensions to the dotnet infrastructure, should be used at that point.
//
//
// KiwiC contains some further canned preamble: in kiwic-canned.dll. But that is being gradually moved in here instead.
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
using System;

namespace KiwiSystem
{
    class hprls_primitives
    {
      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void hpr_pause()
      {
	 // Wait for one clockcycle (only if necessary in soft pause mode)
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void hpr_pause(int pause_mode) // aka Kiwi.Pause() which does the same. dont need this one.
      {
	// The real body for this method is hardcoded and gets inserted by kiwic (hpr l/s library).  
	// Pause according to the pause mode in the argument (which may be not at all for some soft pauses).
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void hpr_sysexit(int return_code)
      {
	// The real body for this method is hardcoded and gets inserted by kiwic (hpr l/s library).
	// Generate an exit(rc), $finish statment or similar domain-specific simulation exit command.
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void begin_critical_region()
      {
	// The real body for this method is hardcoded and gets inserted by kiwic (hpr/ls library).     
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void end_critical_region()
      {
	 // The real body for this method is hardcoded and gets inserted by kiwic (hpr/ls library).
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public void hpr_pause_control(int new_default_mode)
      {
	// The real body for this method is hardcoded and gets inserted by kiwic (hpr/ls library).
	// The argument is encoded as follows:
	// 0-> "auto"
	// 1-> "hard"
	// 2-> "soft"
	// 3-> "maximal"
	// 4-> "bblock"
      }

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]
      static public bool hpr_testandset(Object it, bool nv) 
      {
	// The real body for this method is hardcoded and gets inserted by kiwic (hpr/ls library).
	// However, we plan to encode it here in future using the begin/end critical region primitives.
	return nv;
      }

    }
}


namespace System
{
  public class Exception
  {
       //Data: collection_t;   // Gets a collection of key/value pairs that provide additional user-defined information about the exception.
       string HelpLink;       //Gets or sets a link to the help file associated with this exception.
       string HResult;        // Gets or sets HRESULT, a coded numerical value that is assigned to a specific exception.
       string InnerException; // Gets the Exception instance that caused the current exception.
       string Message;        // Gets a message that describes the current exception.
       string Source;         // Gets or sets the name of the application or the object that causes the error.
       string StackTrace;     // Gets a string representation of the immediate frames on the call stack.
       string TargetSite;     // Gets the method that throws the current exception.
  }

  public class AggregateException : System.Exception
  {

  }
}


namespace System
{




  // System.BitConverter:  endian-specific xvertion between char arrays for various system types.
  public class BitConverter // Reimplementation of the standard library.
  {
    public static readonly bool IsLittleEndian = true;

    public static Int64 ToInt64(byte [] res, int off)
    {
      Int64 rr = 0;
      rr += (Int64)(res[0+off]) << 0;
      rr += (Int64)(res[1+off]) << 8;
      rr += (Int64)(res[2+off]) << 16;
      rr += (Int64)(res[3+off]) << 24;
      rr += (Int64)(res[4+off]) << 32;
      rr += (Int64)(res[5+off]) << 40;
      rr += (Int64)(res[6+off]) << 48;
      rr += (Int64)(res[7+off]) << 56;
      return rr;
    }

    public static byte [] GetBytes(Int64 x)
    {
      byte [] res = new byte [8];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      res[2] = (byte)((x >> 16) & 0xFF);
      res[3] = (byte)((x >> 24) & 0xFF);
      res[4] = (byte)((x >> 32) & 0xFF);
      res[5] = (byte)((x >> 40) & 0xFF);
      res[6] = (byte)((x >> 48) & 0xFF);
      res[7] = (byte)((x >> 56) & 0xFF);
      return res;
    }


    public static Int32 ToInt32(byte [] res, int off)
    {
      Int32 rr = 0;
      rr += (Int32)(res[0+off]) << 0;
      rr += (Int32)(res[1+off]) << 8;
      rr += (Int32)(res[2+off]) << 16;
      rr += (Int32)(res[3+off]) << 24;
      return rr;
    }

    public static byte [] GetBytes(Int32 x)
    {
      byte [] res = new byte [4];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      res[2] = (byte)((x >> 16) & 0xFF);
      res[3] = (byte)((x >> 24) & 0xFF);
      return res;
    }


    public static Int16 ToInt16(byte [] res, int off)
    {
      UInt32 rr = 0;
      rr += (UInt32)(res[0+off]) << 0;
      rr += (UInt32)(res[1+off]) <<  8;
      return (Int16) rr;
    }

    public static byte [] GetBytes(Int16 x)
    {
      byte [] res = new byte [2];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      return res;
    }


    // Unsigned now
    public static UInt64 ToUInt64(byte [] res, int off)
    {
      UInt64 rr = 0;
      rr += (UInt64)(res[0+off]) << 0;
      rr += (UInt64)(res[1+off]) << 8;
      rr += (UInt64)(res[2+off]) << 16;
      rr += (UInt64)(res[3+off]) << 24;
      rr += (UInt64)(res[4+off]) << 32;
      rr += (UInt64)(res[5+off]) << 40;
      rr += (UInt64)(res[6+off]) << 48;
      rr += (UInt64)(res[7+off]) << 56;
      return rr;
    }

    public static byte [] GetBytes(UInt64 x)
    {
      byte [] res = new byte [8];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      res[2] = (byte)((x >> 16) & 0xFF);
      res[3] = (byte)((x >> 24) & 0xFF);
      res[4] = (byte)((x >> 32) & 0xFF);
      res[5] = (byte)((x >> 40) & 0xFF);
      res[6] = (byte)((x >> 48) & 0xFF);
      res[7] = (byte)((x >> 56) & 0xFF);
      return res;
    }


    public static UInt32 ToUInt32(byte [] res, int off)
    {
      UInt32 rr = 0;
      rr += (UInt32)(res[0+off]) << 0;
      rr += (UInt32)(res[1+off]) << 8;
      rr += (UInt32)(res[2+off]) << 16;
      rr += (UInt32)(res[3+off]) << 24;
      return rr;
    }

    public static byte [] GetBytes(UInt32 x)
    {
      byte [] res = new byte [4];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      res[2] = (byte)((x >> 16) & 0xFF);
      res[3] = (byte)((x >> 24) & 0xFF);
      return res;
    }


    public static UInt16 ToUInt16(byte [] res, int off)
    {
      UInt32 rr = 0;
      rr += (UInt32)(res[0+off]) << 0;
      rr += (UInt32)(res[1+off]) <<  8;
      return (UInt16) rr;
    }

    public static byte [] GetBytes(UInt16 x)
    {
      byte [] res = new byte [2];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      return res;
    }

    public static char ToChar(byte [] res, int off)
    {
      UInt32 rr = 0;
      rr += (UInt32)(res[0+off]) << 0;
      rr += (UInt32)(res[1+off]) <<  8;
      return (char) rr;
    }

    public static byte [] GetBytes(char x)
    {
      byte [] res = new byte [2];
      res[0] = (byte)((x >> 0) & 0xFF);
      res[1] = (byte)((x >> 8) & 0xFF);
      return res;
    }

    public static Boolean ToBoolean(byte [] res, int off)
    {
      return (res[off] != 0);
    }

    public static byte [] GetBytes(Boolean x)
    {
      byte [] res = new byte [1];
      res[0] = (byte)((x) ? 1:0);
      return res;
    }


    // The floating-point versions can usefully take advantage of the Kiwi FastBitConvert attribute ...     

    public unsafe static byte [] GetBytes(float x)
    {
      byte *ptr = (byte *)(&x);    
      byte [] res = new byte [4];
      res[0] = ptr[0];
      res[1] = ptr[1];
      res[2] = ptr[2];
      res[3] = ptr[3];
      return res;
    }


    public unsafe static byte [] GetBytes(double x)
    {
      byte *ptr = (byte *)(&x);    
      byte [] res = new byte [8];
      res[0] = ptr[0];
      res[1] = ptr[1];
      res[2] = ptr[2];
      res[3] = ptr[3];
      res[4] = ptr[4];
      res[5] = ptr[5];
      res[6] = ptr[6];
      res[7] = ptr[7];
      return res;
    }

    public static unsafe float ToSingle(byte [] srcbytes, int off) 
    {
      float rr = 0.0f;
      byte *f =  (byte *)(&rr);
      f[0] = srcbytes[0+off];
      f[1] = srcbytes[1+off];
      f[2] = srcbytes[2+off];
      f[3] = srcbytes[3+off];
      return rr;
    }

    public static unsafe double ToDouble(byte [] srcbytes, int off)
    {
      double rr = 0.0f;
      byte *f =  (byte *)(&rr);
      f[0] = srcbytes[0+off];
      f[1] = srcbytes[1+off];
      f[2] = srcbytes[2+off];
      f[3] = srcbytes[3+off];
      f[4] = srcbytes[4+off];
      f[5] = srcbytes[5+off];
      f[6] = srcbytes[6+off];
      f[7] = srcbytes[7+off];
      return rr;
    }


  }


  // Most of the Array class is hardwired inside KiwiC, but some additional functionality is implemented here.
  public class Array<T> where T: System.IEquatable<T>
  {


    void Dispose() // It is intended that you call this manually when KiwiC escape analysis fails to spot an array does not escape a not-unwound loop.  However, mcs does not like explicit dispose of the builtin 1-D array type so a direct manual call to Kiwi.Dispose(array) should be made.
    {  
      // Implemented inside KiwiC as Kiwi.Dispose(this)
    }

    // Array.Index is converted to System.Array.IndexOf by the C# compiler.
    // It's a little odd that the static member inherits the tyvar of its instantiating class ... there might be a tidier way to define this.
    static int IndexOf(T [] haystack, T needle)
    {
      for (int v = 0; v<haystack.Length; v++) 
       {
         if (haystack[v].Equals(needle)) return v;
         // Kiwi.Pause(Soft) // Needed in case this is called with a hard-pause mode thread for a large array.
       }
      return -1;
    }
   
    // This is one of several copy functions in the standard library
    public static void Copy(T [] src, T [] dest, int length)
    {
      for (int i=0; i<length; i++) dest[i] = src[i];
    }

  } // End of class Array


  // System.Math Maths libraries - stubs for separately-implemented RTL versions.
  public class Math
  {
    [KiwiSystem.Kiwi.Stub("protocol=HFAST;searchbymethod=true")]
     public static double Sqrt(double arg) 
     {
       return 1.23455678;   // This is a place-holder.  The actual implementation is in a loaded RTL library.
     }

    [KiwiSystem.Kiwi.Stub("protocol=HFAST;searchbymethod=true")]
    public static double Sin(double arg) 
     {
       return 1.23455678;   // This is a place-holder.  The actual implementation is in a loaded RTL library.
     }

    [KiwiSystem.Kiwi.Stub("protocol=HFAST;searchbymethod=true")]
    public static double Cos(double arg) 
     {
       return 1.23455678;   // This is a place-holder.  The actual implementation is in a loaded RTL library.
     }

    [KiwiSystem.Kiwi.Stub("protocol=HFAST;searchbymethod=true")]
    public static double Log(double arg) 
     {
       return 1.23455678;   // This is a place-holder.  The actual implementation is in a loaded RTL library.
     }

    [KiwiSystem.Kiwi.Stub("protocol=HFAST;searchbymethod=true")]
    public static double Exp(double arg) 
     {
       return 1.23455678;   // This is a place-holder.  The actual implementation is in a loaded RTL library.
     }

  }

}


public class KIWIARRAY2D<CT>
{
  CT [] arraybase;   

  int d2dim0, d2dim1;
  
  public int GetLength(int n) { if (n==0) return d2dim0; return d2dim1; }

  public int get_Rank() { return 2; }


  public KIWIARRAY2D(int a1, int a0) // Constructor
  {
    d2dim0 = a0; d2dim1 = a1;
    arraybase = new CT [a0*a1];
  }



  // When we write A[p,q] += 1.0, mcs will get the address and duplicate it for the load and store ldind, stind.
  // This refuses to compile: gives error : Kiwic.cs(345,17): error CS0208: Cannot take the address of, get the size of, or declare a pointer to a managed type `CT'
  // So perhaps need to implement in CIL assembly code? Or assert that CT is not a managed type when used.
  /*
  public unsafe CT *Address(uint i1, uint i0)
   {
        return &(arraybase[i0 + i1*d2dim0]);
   }
   */

  // Because mcs refuses to compile the Address routine above, KiwiC intercepts calls to Address, calls KiwiSpecialAddress, multiplies by sizeof CT and and adds on the arraybase field instead.
  public long KiwiSpecialAddress(uint i1, uint i0)
   {
        return i0 + i1*d2dim0;
   }

  public CT this[uint i1, uint i0]
   {
      get  // read
      {
        return arraybase[i0 + i1*d2dim0];
      }

      set 
      {
	arraybase[i0 + i1*d2dim0] = value;
      }
   }
}



namespace System
{
  // Most of the Array class is hardwired inside KiwiC, but some additional functionality is implemented here.
  public partial class Array 
  {

    static void KiwiDotPause(int hardf)      // We cannot use Kiwi.Pause from Kiwic.dll so we use the Kiwi reserved name KiwiDotPause
    {
      // Implemenation is hardcoded in KiwiC.
      
       
    }

    // Array.Index is converted to System.Array.IndexOf by the C# compiler.
    static int IndexOf<ACT>(ACT [] haystack, ACT needle)  // where ACT: System.IEquatable<ACT>
    {
      Console.WriteLine("Array.IndexOf is half-way through being implemented and may not be working!");
      // return 2;
      for (int v = 0; v<haystack.Length; v++) 
       {
         KiwiDotPause(0); // Needed in case this is called with a hard-pause mode thread for a large array.
         if (haystack[v].Equals(needle)) return v;
       }
      return -1;
    }
   
  } // End of class Array

}


// Jagged (multi-vector) array
// For H/W synthesis, this will support concurrent access to each lane
// provided the first subscript is compile-time constant.
public class KIWIARRAY2D_J<CT>
{
   int d2dim0, d2dim1;
   public int GetLength(int n) { if (n==0) return d2dim0; return d2dim1; }

   CT [] [] bases;
   public KIWIARRAY2D_J(int a1, int a0) // Constructor
   {
     d2dim0 = a0; d2dim1 = a1;
     bases = new CT [a0] [];
     for (int i =0; i<a0; i++) bases[i] = new CT [a1];
   }

   public CT this[uint i1, uint i0]
    {
       get  // read
       {
	 return bases[i1][i0];
       }

       set //  
       {
	 bases[i1][i0] = value;
       }
    }
 }

// 3-d array.
// Note that this is a stupid implementation since the multiplies for the subscripts cannot be parallelised.  Better to store the premultiplied dim product.
public class KIWIARRAY3D<CT>
{
  CT [] arraybase;
  int d2dim0, d2dim1, d2dim2;

  public int GetLength(int n) { if (n==0) return d2dim0; if (n==1) return d2dim1; return d2dim2; }

  public int get_Rank() { return 3; }
  

  public KIWIARRAY3D(int a2, int a1, int a0) // Constructor
  {
    d2dim0 = a0; d2dim1 = a1; d2dim2 = a2;
    arraybase = new CT [a0*a1*a2];
  }

  
  public long KiwiAddress(uint i2, uint i1, uint i0)
  {
    return i0 + d2dim0*(i2*d2dim1 + i1);
  }

  public CT this[uint i2, uint i1, uint i0]
   {
      get  // read
      {
        return arraybase[i0 + d2dim0*(i2*d2dim1 + i1)]; // Stupid
      }

      set  // write:
      {
	arraybase[i0 + d2dim0*(i2*d2dim1 + i1)] = value;
      }
   }
}

public class KIWIARRAY4D<CT>
{
  CT [] arraybase;
  int d2dim0, d2dim1, d2dim2, d2dim3;
  public int GetLength(int n) { if (n==0) return d2dim0; if (n==1) return d2dim1; if (n==2) return d2dim2; return d2dim3; }

  public int get_Rank() { return 4; }

  public KIWIARRAY4D(int a3, int a2, int a1, int a0) // Constructor
  {
    d2dim0 = a0; d2dim1 = a1; d2dim2 = a2; d2dim3 = a3;
    arraybase = new CT [a0*a1*a2*a3];
  }

  public long KiwiAddress(uint i3, uint i2, uint i1, uint i0)
   {
       return i0 + d2dim0*(d2dim1*(d2dim2*i3 + i2) + i1);
   }

  public CT this[uint i3, uint i2, uint i1, uint i0]
   {
      get  // read
      {
        return arraybase[i0 + d2dim0*(d2dim1*(d2dim2*i3 + i2) + i1)];
      }

      set 
      {
	arraybase[i0 + d2dim0*(d2dim1*(d2dim2*i3 + i2) + i1)] = value;
      }
   }

}




namespace System
{

  public partial class IDisposable
  {
    [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
    public void Dispose()
    {
     //  This is invoked by mcs on the exit of a C# using block.
     //  Actual implementation is hardcoded inside KiwiC.
    }
    // TODO say why new() is not listed here...
  }

//  Kiwic.cs(254,20): warning CS0436: The type `System.Int32' conflicts with the imported type of same name'. Ignoring the imported type definition  
//gmcs /target:library  /unsafe /reference:Kiwi.dll Kiwic.cs
//warning CS1685: The predefined type `System.Int32' is redefined in the source code. Ignoring the local type definition

/*
  public partial class Int32
  {
     public static Int32 Parse(string arg)
     {
        char [] c = arg.ToCharArray();
        int r = 0;
        Int32 r = (Int32)0;
        for (int i=0; i<c.Length; i++) { r = r * 10 + c[i] - (Int32)'0'; }
        for (int i=0; i<c.Length; i++) { r = r * 10 + c[i] - (int)'0'; }
        return (Int32)(r);
      }
  }
*/

  // Contents of String need to appear both under strings for mono and mscorlib/System/String for MS .net 
  public partial class String
  {

    static Boolean op_Inequality(Object a, Object b)
    {
      return !op_Equality(a, b);
    }

    [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
    static Boolean op_Equality(Object a, Object b)
    {
      return true; // Actual implementation is inside KiwiC.
    }

    [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
#pragma warning disable 0436
    static System.String Concat(Object a, Object b)
#pragma warning restore 0436
    {
      return null; // Actual implementation is inside KiwiC.
    }

    // KiwiC supports diadic concat natively. Forms with greater arity are implemented here.
    static String Concat(Object a, Object b, Object c)
    {
      return Concat(Concat(a, b), c);
    }

    static String Concat(Object a, Object b, Object c, object d)
    {
      return Concat(Concat(Concat(a, b), c), d);
      
    }


    [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
    public char [] ToCharArray()
    {
      return null; // Actual implementation is hardcoded inside KiwiC.
    }

  }

}
 

namespace System
{
  public partial class Environment
  {
      [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
      public static void Exit(int exit_code)
      {
         Console.WriteLine("Kiwi runtime system - exit rc={0}", exit_code);
	   // Actual implementation is hardcoded inside KiwiC.
      }
  }

}



namespace System.Threading
{

  public partial class Thread
  {

    [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
    public static void MemoryBarrier()
    {
      	   // Actual implementation is hardcoded inside KiwiC.
    }

  }
      public class Monitor
      {

	// Enter aquires the lock on a critical region (c.f. Exit).
	public static void Enter(Object it)
	{
          // Acquire lock: we must spin until testandset returns false, meaning we did not have it before!
	  while (KiwiSystem.hprls_primitives.hpr_testandset(it, true)) continue;
	}


	// Enter, 2nd overload, aquires the lock and the taken flag holds unless exception occurred while waiting.
	public static void Enter(Object it, ref bool lockTaken)
	{
          lockTaken = false; 
          // Acquire lock: we must spin until testandset returns false, meaning we did not have it before!
	  do 
	    {
	      // Removing this pause here may speed up throughput! This is hardly a futex! TODO.
	      KiwiSystem.Kiwi.Pause();                                   
	    }
	  while (KiwiSystem.hprls_primitives.hpr_testandset(it, true));
          lockTaken = true; 
	}


	// Condition variable wait (spin lock) ?
        // condvar is currently ignored infact! TODO.
// This wait is executed inside a Lock statement so we first free the mutex, allowing others to
// do something. The actual spinning is done by the caller to Wait.  We just pause and then
// reacquire the lock.

	public static bool Wait(Object it)
	{
	  KiwiSystem.hprls_primitives.hpr_testandset(it, false);                     // Release lock
          KiwiSystem.Kiwi.Pause();                                   
	  //do // Spin lock (at least one pause version).
	  //  {
	  //    KiwiSystem.hprls_primitives.hpr_pause();                                   
	  //  } while (!condvar);
	  while (KiwiSystem.hprls_primitives.hpr_testandset(it, true)) // Reacquire lock
	    KiwiSystem.Kiwi.Pause();                                   
	  return false; // Return a condition - don't know if this is correct.
	}


	public static void Exit(Object it) // Exit from critical region.
	{
	   KiwiSystem.hprls_primitives.hpr_testandset(it, false);
	}

	public static void PulseAll(Object it) // Wake up all threads waiting on a semaphore.
	{
	  // This is currently a NoP for h/w synthesis since it is assumed a hardware circuit consumes no dynamic power while polling for a register to change state.
	}

	public static void Pulse(Object it) // Wake up one thread waiting on a semaphore.
	{
	  // This is currently a NoP for h/w synthesis since it is assumed a hardware circuit consumes no dynamic power while polling for a register to change state.
	}
      } 
}

namespace System
{
  namespace Diagnostics
  {

    // Here is a simple version of System.Diagnostics.Stopwatch that is built
    // trivially on top of the Kiwi.tnow mechanism.
    // The result will depend on the clock freq and so is not calibrated under Kiwi at the moment.
    class Stopwatch
    {

      ulong run_basis;
      ulong value;
      bool runf;
      
      public ulong ElapsedMilliseconds
      {
        get
          {
            ulong ans = value;
            if (runf) ans += (KiwiSystem.Kiwi.tnow - run_basis);
            return ans;
          }
      }
      
      public void Reset()
      {
        run_basis = KiwiSystem.Kiwi.tnow;
        value = 0;
        runf = false;
      }
      
      public void Start()
      {
        run_basis = KiwiSystem.Kiwi.tnow;
        runf = true;
      }
      
      public void Stop()
      {
        runf = false;
        value += (KiwiSystem.Kiwi.tnow - run_basis);
      }
      
      
    }
  }
}



// eof
