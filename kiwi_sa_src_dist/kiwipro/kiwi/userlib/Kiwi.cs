//
// $Id: attribute_definitions.cs,v 1.17 2010/08/02 12:53:06 djg11 Exp $
//
// Kiwi attribute definitions
// (C) 2014-16 DJ Greaves, University of Cambridge.
// (C) 2007-14 DJ Greaves + Satnam Singh (Univ of Cambridge + Microsoft Research).
//
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
//
//
using System;
using System.Text;
using System.Threading;

namespace KiwiSystem
{


  public class hpr_primitives
  {

    [Kiwi.HprPrimitiveFunction()]
    public static void hpr_sysexit(byte arg)
    {
      	// This function is implemented inside the KiwiC compiler.
    }

    [Kiwi.HprPrimitiveFunction()]
    public static void hpr_sysleds(byte arg)
    {
      	// This function is implemented inside the KiwiC compiler.
    }
  }


  public class Kiwi
    {
     
      // I/O
      public static void WriteLine(string arg)
      {
	// Note: Kiwi.WriteLine is different from Console.WriteLine in that it is only converted to a $display and does not write to the hardware console on the FPGA.
	// This function is implemented inside the KiwiC compiler.
      }

      public static void WriteLine(string arg, Object a0)
      {
	// Note: this is different from Console.WriteLine in that it is only converted to a $display and does not write to the hardware console on the FPGA.
	// This function is implemented inside the KiwiC compiler.
      }

      public static void WriteLine(string arg, Object a0, Object a1)
      {
	// Note: this is different from Console.WriteLine in that it is only converted to a $display and does not write to the hardware console on the FPGA.
	// This function is implemented inside the KiwiC compiler.
      }

      public static void WriteLine(string arg, Object a0, Object a1, Object a2)
      {
	// Note: this is different from Console.WriteLine in that it is only converted to a $display and does not write to the hardware console on the FPGA.
	// This function is implemented inside the KiwiC compiler.
      }

      public static void WriteLine(string arg, Object a0, Object a1, Object a2, Object a3)
      {
	// Note: this is different from Console.WriteLine in that it is only converted to a $display and does not write to the hardware console on the FPGA.
	// This function is implemented inside the KiwiC compiler.
      }
      
      // KPragma with first argument as Boolean true can be used to conditionally abend elaboration or to log user progress messages during elaboration.
      // KPragma calls present in run-time loops can be emitted at runtime using the Console.WriteLine mechanisms.
      // KPragma calls with magic string values will be used to instruct the compiler, but no magic words are currently implemented.

      public static int KPragma(bool fatalFlag, string cmd_or_message)
      {
        return 0;
        // This function is implemented inside the KiwiC compiler.
      }
      
      public static int KPragma(bool fatalFlag, string cmd_or_message, int arg0)
      {
        return 0;
        // This function is implemented inside the KiwiC compiler.
      }
      
      public static int KPragma(bool fatalFlag, string cmd_or_message, int arg0, int arg1)
      {
        return 0;
        // This function is implemented inside the KiwiC compiler.
      }

        public class HprPrimitiveFunction : System.Attribute
        {
            // No body needed here: this attribute causes functions in
            // the source code to NOT be inlined by the kiwic front end. Instead
            // kiwic will pass them on to next stage in the recipe.


            public HprPrimitiveFunction()
            {
            }
            public HprPrimitiveFunction(string arg)
            {
            }
        }

        // Do not alter the order of these (auto=0=default).
      public enum PauseControl { autoPauseEnable, hardPauseEnable, softPauseEnable, maximalPauseEnable, bblockPauseEnable, pipelinedAccelerator };
        //Many net-level hardware protocols are intolerant to clock dilation. In other words, their
        //semantics are defined in terms of the number of clock cycles for which a condition
        //holds.  A thread being compiled by KiwiC defaults to bblock or soft pause control, meaning that
        //KiwiC is free to stall the progress of a thread at any point, such as when it
        //needs to use extra clock cycles to overcome structural hazards.  These two approaches
        //are incompatible.  Therefore, for a region of code where clock cycle allocation
        //is important, KiwiC must be instructed to use hard pause control.

//The idea is that you can change it locally within various parts of a thread's control flow graph by calling 
// \verb+PauseControlSet(mode)+ where the mode is a member of the PauseControl enumeration. Also, this can be 
// passed as an argument to a Kiwi.Pause call to set the mode for just that pause.


        // Maximal and bblock are 'debug' modes where pauses are inserted at every semicolon and every basic block boundary respectively.

	// pipelinedAccelerator is totally different, not being a classical/sequencer HLS mode, but more of a DSP/Cathedral real-time mode.
	
        static PauseControl old_pausemode_value = PauseControl.softPauseEnable;            

        [Kiwi.HprPrimitiveFunction()]
        // Update the pause inference mode of the current thread for the
        // next part of its progress.  Returns existing setting.
        public static PauseControl PauseControlSet(PauseControl newmode)
        {
	   PauseControl r = old_pausemode_value;
           old_pausemode_value = newmode;      
 	   // NB: The body of this function is re-implemented by KiwiC.
	   return r;
        }

	// For Example:  Kiwi.PauseControlSet(Kiwi.PauseControl.softPauseEnable);

      public static void Pause(int pauseFlags)
      {
  	//TODO: this increment of tnow clearly fails for multiple threads ... need to do per pid/tid.
	tnow += 10; // 100 MHz is 10ns period (default target area!) *) 

	/* First arg to hpr_pause is a bit vector with the following encodings
 enum hpr_PauseFlags =
    let NoUnroll = 1
    let SoftPause = 2
    let HardPause = 4
    let CurrentModePause = 8    
    let EndOfElaborate = 16
 */

	// NB: The body of this function is re-implemented by KiwiC.
      }
        // Pause for a clock cycle to happen. Without any args we pause using the current pause mode for the thread	
        // Various semi-conditional forms will be added.
        public static void Pause()
        {
	  Kiwi.Pause(/* CurrentPauseMode = */8);
	  //int tid = Thread.CurrentThread.ManagedThreadId;
	  // NB: The body of this function is re-implemented by KiwiC.
        }


        public static void Unroll(int a)
        {  // Use these unroll functions to instruct kiwic to subsume a variable (or variables)
            // during compilation.  It should typically be used with loop variables: 
            // 
            // for (int cpos = 0; cpos < height; cpos++)
            //   { Kiwi.Unroll(cpos);
            //     ...
            //   }
        }

        public static void Unroll(string command_string, int a)  
        { 
            // for (int cpos = 0; cpos < height; cpos++)
            // Suggest a suitable unroll factor (not working at the moment...)	     
            //   { Kiwi.Unroll("COUNT~=4", cpos);  
             
        }

        public static void Unroll(int a, int b)
        { // To subsume annotate two variables at once.
        }

        public static void Unroll(string comment_string, int a, int b)
        { // To subsume annotate two variables at once.
        }

        public static void Unroll(int a, int b, int c)
        { // To annotate three variables.
            // To request subsumation of more than three variables note that 
            // calling Unroll(v1, v2) is the same as Unroll(v1 + v2).  I.e. the
            // support of the expressions passed is flagged to be subsumed in total or
            // at least in the currently enclosing loop.
        }

        public static void NoUnroll()
        {
            // Put this in the body of a loop that is NOT to be unrolled by KiwiC.
            // If there is a Kiwic.Pause in the loop, that's the default anyway, so the addition
            // of a NoUnroll makes no difference.

	  Kiwi.Pause(/* NoUnroll = */1);
        }

        public static int EndOfElaborate()
        {
            // Every thread compiled by KiwiC has its control flow partitioned between compile time and run time.  The division is the end of elaboration pont.
            // Although KiwiC will spot the end of elabroation point for itself, the user can make a manual call to this at the place where he thinks elaboration should end for confirmation.
            // 
	  Kiwi.Pause(/* EndOfElaborate = */16);
	  KPragma(false, "_EndOfElaborate");
	  return 0;
        }



        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class RomArrayAttribute : System.Attribute  // This is not used by Kiwi at the moment
        { 
            public string name;
            public RomArrayAttribute(string n)
            {
                name = n;
            }
        };

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class MemSpace : System.Attribute  // For objects placed in bondout memories, this attribute enables manual control of which memory space (and ultimately which storage bank) they are placed in.
        { 
            public string name;
            public MemSpace(string n)
            {
                name = n;
            }
        };


        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class InputBitPortAttribute : System.Attribute
        {
            public string name;

            public InputBitPortAttribute(string n)
            {
                name = n;
            }
            public InputBitPortAttribute()
            {
                name = "";
            }

        } ;

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class OutputBitPortAttribute : System.Attribute
        {
            public string name;

            public OutputBitPortAttribute(string n)
            {
                name = n;
            }
            public OutputBitPortAttribute()
            {
                name = "";
            }

        };

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class OutputNameAttribute : System.Attribute
        {
            public string name;

            public OutputNameAttribute(string n)
            {
                name = n;
            }

        };

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class InputNameAttribute : System.Attribute
        {
            public string name;

            public InputNameAttribute(string n)
            {
                name = n;
            }

        };
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class InputBytePortAttribute : System.Attribute
        {
            public string name;
            public InputBytePortAttribute(string n)
            {
                name = n;
            }
        };

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class OutputBytePortAttribute : System.Attribute
        {
            public string name;
            public OutputBytePortAttribute(string n)
            {
                name = n;
            }
        } ;

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class OutputWordPortAttribute : System.Attribute
        {
            public string name;
            public int loIndex, hiIndex;
            public OutputWordPortAttribute(int hi, int lo, string n)
            {
                // Use this to set the index range in the RTL (h::l)
                loIndex = lo;
                hiIndex = hi;
                name = n;
            }

            public OutputWordPortAttribute(int hi, int lo)
            {
                // Use this to set the index range in the RTL (h::l)
                loIndex = lo;
                hiIndex = hi;
            }

            // Use this to specify a friendly name to be used in the RTL.
            public OutputWordPortAttribute(string n)
            {
                name = n;
            }
        }


          // Use this attribute for a static constant variable whose value will be provided at logic synthesis time via an RTL paramer or a C++ constructor argument in SystemC.
          // For well-formed final RTL, those with default values should preceed those without.  See test65
        public class RtlParameterAttribute : System.Attribute
        {
          public string name;
          public int defaultv;
          public RtlParameterAttribute(string n)
            {
                name = n;
            }
          public RtlParameterAttribute(string n, int v)
            {
              name = n; defaultv = v;
            }
        } ;


        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class InputWordPortAttribute : System.Attribute
        {
            public string name;
            public int loIndex, hiIndex;
            public InputWordPortAttribute(int hi, int lo)
            {
                loIndex = lo;
                hiIndex = hi;
            }

            public InputWordPortAttribute(string n)
            {
                name = n;
            }

            public InputWordPortAttribute(int hi, int lo, string n)
            {
                // Use this to set the index range in the RTL (h::l)
                loIndex = lo;
                hiIndex = hi;
                name = n;
            }

        } ;


        //====================================
        // Current simulation time in ticks. Perhaps use a get method for it?
        // Reads of this are converted to $time or SystemC kernel timestamp in appropriate backends.
        public static UInt64 tnow = 0;


        //====================================
        public static void NeverReached(string msg)
        {
            // This does nothing as a s/w program but traps KiwiC if elaborated.
            // We could write to the Kiwi Abend register at run-time I suppose?
        }

        //====================================
	// Array mark-up attributes.
	
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class StripeAttribute : System.Attribute
        {
	  public StripeAttribute(int stride) //  Make stride physical RAMs or ROMs and stipe the data over them.
	  {
   	     //This attribute is detected by KiwiC and processed inside the compiler.   
	  }
	}

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class MirrorAttribute : System.Attribute
        {
	  public MirrorAttribute(int stride) //  Make multiple copies of the RAM or ROM.  This is most sensible for ROMs since all copies of a RAM must be updated with every write.
	  {
   	     //This attribute is detected by KiwiC and processed inside the compiler.   
	  }
	}

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class WidenAttribute : System.Attribute
        {
	  public WidenAttribute(int n) // Instruct KiwiC to pack $n$ words into a single location.
	  {
   	     //This attribute is detected by KiwiC and processed inside the compiler.   
	  }
	}


	// Mark an array for implementation as a register file with as many ports as needed: ie there are as many
        // concurrent read and write operations supported as needed.  This is the default behaviour for arrays whose
	// length is less than res2-regfile-threshold.


	// Note: May2016 - this attribute should not need to be used now since restructure infers this.
        public class MultiPortRegfile : System.Attribute
        {
            public MultiPortRegfile() 
            {

            }
        }

	// By default, KiwiC will use one port on an SRAM for each thread that operates on it.
	// However, by settings the PortsPerThread to greater than one then
	// greater access bandwidth per clock cycle for each thread is possible.  Note that Xilinx Virtex BRAM
	// supports up to two ports per BRAM in total, so having ports per thread set to two is the maximum sensible
	// value and that may only sensible if there is only one thread making access to the RAM.  In the future, several threads
        // in the same clock domain might get to share the physical ports if the compiler can spot they are temporarily disjoint
	// (i.e. never concurrent).

        public class PortsPerThread : System.Attribute
        {
            public PortsPerThread(int n) 
            {

            }
        }

	// By default, KiwiC will use one port on an SRAM for each thread that operates on it.
	// However, by setting ThreadsPerPort to greater than one (e.g. to some arbitrary large number), the compiler
	// will instantiate arbiters to share a given port by up to that number of threads, before starting to use
	// another port.  Threads in the same clock domain are given affinity to each other when allocating the sharing.
        public class ThreadsPerPort : System.Attribute
        {
            public ThreadsPerPort(int n) 
            {

            }
        }

	// Mark an array for allocation as synchronous SRAM (in Xilinx terms BRAM).
        // This overrides the default mechanism that implements this behaviour for array lengths between res2-combram-threshold and  res2-offchip-threshold.
        public class SynchSRAM : System.Attribute
        {
            public SynchSRAM() // No arguments, defaults to one cycle latency.
            {
	       // This is spotted by KiwiC and processed accordingly internally.
            }

            public SynchSRAM(int latency) // With arguments: set the latency. 0 makes it a combinational RAM. 1 is the same as not having any argument, 2 or greater is not currently supported by the compiler, but might be in the future.
            {
	       // This is spotted by KiwiC and processed accordingly internally.
            }

        };

        public class CombSRAM : System.Attribute
        {

	  // An attributed added to an array to request that it is mapped to combinational RAM - e.g. LUT RAM.
            public CombSRAM() // No arguments
            {
	       // This is spotted by KiwiC and processed accordingly internally.
            }


        };

        // Mark an array for allocation in off-chip resources (such as DRAM or cached DRAM).
        // This overrides the default mechanism that implements this behaviour for array lengths greater than or equal to res2-offchip-threshold.
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class OutboardArray : System.Attribute
        {
            // Give the load/store port name or bank name and let KiwiC define the memory allocation.
            public OutboardArray(string bankname) // Automatic base form: compiler will select a base address.
            {

            }

	    // Using the special argument `-onchip-' the {\tt Kiwi.OutboardArray("-onchip-")} attribute forces that an array is not offboard regardless of size.  Clearly this may result in a design that is unsuitable for the target technology.


            public OutboardArray(string bankname, int Base) // Explicit base form if user wants memory map control
            {

            }

            public OutboardArray()  // Do not give a bank name: KiwiC will use dramport by default.
            {

            }

        } ;


        //====================================
	// Method markups


      /* From test56  - Adding the FastBitConvert attribute make KiwiC
         ignore the bodies of functions such as these and replaces the body
         with its own fast-path identity code based only on the signatures of 
         the functions. */
  [Kiwi.FastBitConvert()]
  static double to_double(ulong farg)
  {
    byte [] asbytes = BitConverter.GetBytes(farg);
    double rr = BitConverter.ToDouble(asbytes, 0);
    return rr;
  }

  [Kiwi.FastBitConvert()]
  static ulong from_double(double darg)
  {
    byte [] asbytes = BitConverter.GetBytes(darg);
    return BitConverter.ToUInt64(asbytes, 0);
  }


 /*using float64 = System.UInt64;
    From test56  - Adding the FastBitConvert attribute makes KiwiC
    ignore the bodies of functions such as these and replaces the body
    with its own fast-path identity code based only on the signatures of 
    the functions. 

  [Kiwi.FastBitConvert()]
  static double to_double(float64 farg)
  {
    byte [] asbytes = BitConverter.GetBytes(farg);
    double rr = BitConverter.ToDouble(asbytes, 0);
    return rr;
  }
  */

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class FastBitConvert : System.Attribute
        {
	  // FastBitConvert(): Direct KiwiC to replace the body of this function with a hardcoded alternative.  Compilation of the GetBytes version performs floating point conversion on each byte in turn.
	  public FastBitConvert()
            {
            }
        };

      // Remote : mark up a method as remote callable so that it can be invoked over a net-level interface.
      // Makes sense mainly for static methods.  
      [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class Remote : System.Attribute
        {
	  public Remote(string composite) // Composite attributes are parsed from inside this string
            {
            }
	  public Remote()
            {
            }
        } ;

      // Remote : mark up a class so all public methods are remote callable.  Constructor parameters may become RTL parameters.
      [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class RemoteClass : System.Attribute
        {
	  public RemoteClass(string composite) // Composite attributes are parsed from inside this string
            {
            }
	  public RemoteClass()
            {
            }
        } ;

      // A stub: do not process the body of this method: instead call out to a separate implementation using net-level handshake 
      // This is not supported/used/docmented?
      [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
      public class Stub : System.Attribute
        {
	  public Stub(string composite) // Composite attributes are parsed from inside this string
            {
            }
	  public Stub()
            {
            }

        } ;


        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        // Make a method implementation shared (rather than inlined at caller's site).
        // NB: May 2011: not so far implemented in KiwiC.
        public class Server : System.Attribute
        {
	  public Server(string composite) // Composite attributes are parsed from inside this string
            {
            }
	  public Server()
            {
            }

        } ;


        // Mark up a method as a root entry point for KiwiC conversion to hardware.
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class HardwareEntryPoint : System.Attribute
        {
            public string name;
            public HardwareEntryPoint()
            {
              // There is no body - this is an attribute/marker detected by KiwiC.
            }
	    
            public HardwareEntryPoint(PauseControl initialPauseMode)
            {
              // There is no body - this is an attribute/marker detected by KiwiC.
            }

            public HardwareEntryPoint(string subtrate_style)
            {
              // There is no body - this is an attribute/marker detected by KiwiC.
            }

        } ;

        // Mark up a method for conversion to hardware in accelerator mode.
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class PipelinedAccelerator : System.Attribute
        {
            public string name;
            public PipelinedAccelerator()
            {
//TODO check substrate_style      | [(_, X_num 5)] -> MM_pipeline_accelerator_hls
              // There is no body - this is an attribute/marker detected by KiwiC.
            }
	    
          public PipelinedAccelerator(string name, string attributeList="")
            {
              // There is no body - this is an attribute/marker detected by KiwiC.
            }

        } ;


      // Waypoints and phase marks
// The so-called manual waypoint was a temporary workaround. KppMark should be used going forward and this integrates with the directorate system.
// It you want the manual waypoint then do it manually in your own dll, not here in the main Kiwi.cs which affects everybody!
//      [Kiwi.OutputWordPort("ksubsManualWaypoint")] public static byte ksubsManualWaypoint = 0;

      [KiwiSystem.Kiwi.HprPrimitiveFunction()]    
      private static void KppMarkPrimitive(int manualWaypointNumber, string waypointName, string subsequentPhaseName = "")
      {
        /*  The body is implemented inside KiwiC Compiler  */
      }

      public static void KppMark(int manualWaypointNumber, string waypointName, string subsequentPhaseName)
      {
        KppMarkPrimitive(manualWaypointNumber, waypointName, subsequentPhaseName);
//        ksubsManualWaypoint = (byte) manualWaypointNumber; 
      }

      public static void KppMark(int manualWaypointNumber, string waypointName)
      {
        KppMarkPrimitive(manualWaypointNumber, waypointName);
//        ksubsManualWaypoint = (byte) manualWaypointNumber;
      }


        // Mark up a method or code point as waypoint or timing/profile point marker.
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class KppMarker : System.Attribute
        {
          public string name;
          public KppMarker(int manualWaypointNumber, string WaypointLabel)
            {
               //ksubsManualWaypoint = (byte) manualWaypointNumber; // This assign is implemented by KiwiC
            }

	  public KppMarker(int manualWaypointNumber, string WaypointLabel, string subsequentPhaseName)
            {
              //ksubsManualWaypoint = (byte) manualWaypointNumber; // This assign is implemented by KiwiC
            }
        } ;


        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class AssertCTL : System.Attribute
        {
            public AssertCTL(String Quantifier, string ViolationMsg)
            {
                //       name = n;
            }
        };

        [Kiwi.CTLInternal("hpr_next")]
        private static object hpr_next(object item)
        {
            return item; // kiwic implements this
        }

        // Next value of an expression
        public static Object next(Object item)
        {
            return hpr_next(item);  // This is a temporary body: kiwic implements it for real.
        }

        // Next value of a bool expression
        public static bool next(bool item)
        {
            return (bool)hpr_next(item);  // This is a temporary body: kiwic implements it for real.
        }

        // Next value of an integer expression
        public static int next(int item)
        {
            return (int)hpr_next(item);
        }

        // Until and weak until operators
        [Kiwi.CTLInternal("Kiwi.until")]
        public bool until(bool from, bool to)
        {
            return false;  // This is a temporary body: kiwic implements it for real.
        }

        // Until and weak until operators
        [Kiwi.CTLInternal("Kiwi.wuntil")]
        public bool wuntil(bool from, bool to)
        {
            return false;  // This is a temporary body: kiwic implements it for real.
        }


        // Hardware width: set the width of a local register.
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class HwWidth : System.Attribute
        {
            public HwWidth(int w)
            {
                //       name = n;
            }

        } ;



        //====================================

        // Temporal logic operator library stubs
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class CTLInternal : System.Attribute
        {
            public CTLInternal(String n)
            {
                //       name = n;
            }
        };


        //====================================

        // Flag a class as a root for hardware synthesis under KiwiC
        // TODO: implement, test and document.
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class HardwareModuleAttribute : System.Attribute
        {
            public string name;
            public HardwareModuleAttribute(string n)
            {
                name = n;
            }
        };

        // Flag a component as a root for hardware synthesis under kiwic
        // TODO: implement, test and document.
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class HardwareAttribute : System.Attribute
        {
            public HardwareAttribute()
            {
            }
        };

        // The complementary attribute to Hardware
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class SoftwareAttribute : System.Attribute
        {
            public SoftwareAttribute()
            {
            }
        };


        //====================================
        // Additional Volatile Attribute.
        //So use the Kiwi-provided attribute instead for now.
        //Ideally KiwiC works out which variables need to be volatile since all threads sharing a variable are compiled to FPGA at once.

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class Volatile : System.Attribute
        {
            public Volatile()
            {
            }
        };

        //====================================
        // Subsume control (introduced in KiwiC alpha 26) NO LONGER HAS ANY EFFECT  - DELETE ME.

        // Explicitly mark a variable for subsumption at KiwiC compile time.
        // The variable will not appear in the output RTL file.
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class SubsumeAttribute : System.Attribute
        {
            public SubsumeAttribute()
            {
            }
        };

        // DELETE ME.
        // Explicitly mark a variable for elaboration at KiwiC compile time.
        // The variable will appear in the output RTL file.
        //  Kiwi.Elaborate() is no longer used. To be deleted. 
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class ElaborateAttribute : System.Attribute
        {
            public ElaborateAttribute()
            {
            }
        };



        //====================================
        // Clock domain control (reset too).

        // An entry point method may be marked with this attribute and 
        // the method should not call directly or indirectly any other method
        // that is marked with a different ClockDom attribute.
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class ClockDom : System.Attribute
        {
          public ClockDom(string clockName, string resetName, string compositeAttributes = "", string clockEnableName = "")
            {
              // The real implementation is inside KiwiC - this is just a marker.
            }

        };

        //====================================
        // Original and old Channels library
        public class Channel<T>
        {
            [Kiwi.Elaborate()]
            T datum;
            [Kiwi.Elaborate()]
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

        //====================================
        // Manual Garbage Collection (as opposed to autodispose)
	[KiwiSystem.Kiwi.HprPrimitiveFunction()]    
	public static void Dispose(Object obj) //The real implementation is hardcoded inside KiwiC
	{
	  // Instruct the Kiwi-1 static allocator that an item will no longer be used again.
	  // It is then re-allocated at run-time inside the same non-unwound loop the existing static store will be reused.
	  // KiwiC should (ultimately) spot design patterns of this nature and automate the process, but manual invokation is
	  // needed mostly.
	  // Note: this does not interact (yet) with the Kiwi 2.0 HeapManager. Further study needed.
	}

	[KiwiSystem.Kiwi.HprPrimitiveFunction()]    
	public static bool inHardware()
	{
	  return false; // This is the returned value when running on the workstation.
	  // An alternative overriding implementation is hardcoded inside KiwiC and will return 'true' for FPGA and RTL simulation.

	}

      // Kiwi.ObjectHandler<T>
     // The object handler provides backdoors to certain unsafe code for pointer arithmetic
     // that is banned even in unsafe C# code.  Implementation in CIL assember would be possible
     // but having hardcoded support in the KiwiC compiler accessed via this object manager
     // is easier.
     public class ObjectHandler<T>
     {
       [Kiwi.HprPrimitiveFunction()]
       public int size()
       {
      #if IL
	  // Inline assembler works under Microsoft's C# compiler.
	  sizeof !T
	  ret
      #endif

      // The following ideal implementation is not allowed in C#, even unsafe C#.
      // return sizeof(T); //

      // The real implementation is hardcoded in Kiwi for mono use. It does not return numbers smaller than 4.
	 return -1;
       }


       // Perform pointer arithmetic for storage allocators and so on.
       // The internal implementation ensures the correct memory region information is propagated.
       [Kiwi.HprPrimitiveFunction()]
       public T handleArith(Object baser, int offset)
       {
	   long ifs = (long)baser;
	   Object p = (Object) (ifs + offset);
       	   T result = (T)(p);
          return result;
       }


    }

  //Old implementation of Kiwi abend syndrome and leds.
      
// This is via hpr primitive now. So we remove this static here.
//    [Kiwi.OutputWordPort("ksubsGpioLeds")] public static byte ksubsGpioLeds = 128;

      public static void setGpioLeds8(byte led8code)
      {
        KiwiSystem.hpr_primitives.hpr_sysleds(led8code);
      }
      public static void setGpioLeds8(int led8code)
      {
        KiwiSystem.hpr_primitives.hpr_sysleds((byte)led8code);
      }



  // hpr_abend_syndrome is now used instead, which is part of the HPR L/S director infrastructure.
  //Normally this should not be user code. Instead it is part of a Kiwi runtime library.
  //Note the code 128 denotes normal operation, 1 denotes normal exit and 2 denotes unspecified error.  Further codes are given in the table/listing in the Kiwi Manual.
  //[Kiwi.OutputWordPort("ksubsAbendSyndrome")] public static byte ksubsAbendSyndrome = 128;


    public static void ReportNormalCompletion()
    {
       KiwiSystem.hpr_primitives.hpr_sysexit(0);
    }

    public static void ReportCompletion(byte completionCode)
    {
      KiwiSystem.hpr_primitives.hpr_sysexit(completionCode);
    }
  }

  public static class KiwiBasicStatusMonitor
  {
       public static void RunTimeAbend(string errorMessage, int code)
       {
	  Console.WriteLine("  Kiwi RunTimeAbend  msg={0} code={1}", errorMessage, code);
          byte syndrome = (byte)(32 + (code & 31));
          KiwiSystem.hpr_primitives.hpr_sysexit(syndrome);
       }
  }
  
}

namespace System
{
  public class Environment
  {
   
     public static void Exit(byte completionCode = 0)
     {
       KiwiSystem.hpr_primitives.hpr_sysexit(completionCode);
     }
  }
  
}

// eof
