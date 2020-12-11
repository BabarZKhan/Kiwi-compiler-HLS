# NOTE 
Please see below the **References** for more information from the original website of **David Greaves** (the main developer of Kiwi).

# Kiwi-compiler-HLS
Kiwi was developed at the University of Cambridge Computer Laboratory and Microsoft Research Limited, headed by **David Greaves (UoCCL)** and **Satnam Singh (MRL)**. Kiwi continues at the Computer Laboratory as part of a logic synthesis project called HPR L/S and it uses the Orangepath core library [1].

Kiwi is a High-Level Synthesis (HLS) system that primarily generates FPGA designs from C# source code.

Compared with existing high-level synthesis tools, KiwiC supports a wider subset of standard programming language features. In particular, it supports multi-dimensional arrays, threading, file-server I/O, object management and limited recursion. Release 1 of KiwiC supports static heap management, where all memory structures are allocated at compile-time and permanently allocated to on-FPGA RAM or external DRAM. Release 2 of KiwiC, which has had some successful tests already, supports arbitrary heap-allocation at run time but does not implement garbage collection.

The Kiwi performance predictor is another important design tool, enabling HPC users to explore the expected speed up of their application as the modify it, without having to wait for multi-hour FPGA compilations in each development iteration [2].


# Overview
The Kiwi project aims to make reconfigurable computing technology like Field Programmable Gate Arrays (FPGAs) more accessible to mainstream programmers. FPGAs have a huge potential for quickly performing many interesting computations in parallel but their exploitation by computer programmers is limited by the need to think like a hardware engineer and the need to use hardware description languages rather than conventional programming languages [1].

# Asymptotic Background Motivation for FPGA Computing
The Von Neumann computer has hit a wall in terms of increasing clock frequency. It is widely accepted that Parallel Computing is the most energy-efficient way forward. The FPGA is intrinsically massively-parallel and can exploit the abundant transistor count of contemporary VLSI. Andre DeHon points out that the Von Neumann architecture no longer addresses the relevant problem: he writes "Stored-program processors are about compactness, fitting the computation into the minimum area possible" [1].


# Why is computing on an FPGA becoming a good idea ?

Spatio-Parallel processing uses less energy than equivalent temporal processing (ie at higher clock rates) for various reasons. David Greaves gives nine [1]:

1. Pollack's rule states that energy use in a Von Neumann CPU grows with square of its IPC. But the FPGA with a static schedule moves the out-of-order overheads to  
   compile time.
1. To clock CMOS at a higher frequency needs a higher voltage, so energy use has quadratic growth with frequency.
1. Von Neumann SIMD extensions greatly amortise fetch and decode energy, but FPGA does better, supporting precise custom word widths, so no waste at all.
1. FPGA can implement massively-fused accumulate rather than re-normalising after each summation.
1. Memory bandwidth: FPGA has always had superb on-chip memory bandwidth but latest generation FPGA exceeds CPU on DRAM bandwidth too.
1. FPGA using combinational logic uses zero energy re-computing sub-expressions whose support has not changed. And it has no overhead determining whether it has changed.
1. FPGA has zero conventional instruction fetch and decode energy and its controlling micro-sequencer or predication energy can be close to zero.
1. Data locality can easily be exploited on FPGA --- operands are held closer to ALUs, giving near-data-processing (but the FPGA overall size is x10 times larger (x100 
   area) owing to overhead of making it reconfigurable).
1. The massively-parallel premise of the FPGA is the correct way forward, as indicated by asymptotic limit studies [DeHon].

# Scientific Users

 It is widely accepted that many problems in scientific computing can be vastly accelerated using either FPGA or GPU execution resources. Also, the FPGA approach in particular leads generally to a significant saving in execution energy. The product of the two gains can be typically one thousand fold [1].

Kiwi provides acceleration for multi-threaded (parallel) programs provided they can be converted to .NET bytecode. Originating from Microsoft, the .NET (also known as CIL) is a well-engineered, general purpose intermediate code that runs on many platforms, including mono/linux [1].

# Kiwi USPs - Unique Selling Points

There have been numerous high-level synthesis (HLS) projects in recent decades. Finally HLS has come of age, with all FPGA and EDA vendors offering HLS products. Nearly all of the prior work has used C, C++ or SystemC as the source HLL. (Historical note: Greaves CTOV compiler from 1995 is now owned by Synopsys Greaves-CTOV-1995 via the Tenison EDA sale.) [1]

It is widely accepted that C# and mono/dotnet provide a significant leg up compared with C++ owing to crystal clear semantics, selectively checked overflows, neat higher-order functions and delegates, amenability to compiler optimisations and automated refactoring, garbage management, versioned assemblies and so on. Many of these benefits are most strongly felt with parallel programs. Also, the LINQ/Dryad extension is a clean route for manual invocations of accelerators [1].

The Kiwi system has the following USPs compared with most/all other HLS tools:

1. Source language is C# (or other CIL PE-generating HLLs),
1. The same program runs on mono/dotnet for development as on the FPGA for high-performance execution,
1. Concurrent (parallel) programs are supported,
1. Dynamic allocation of objects and manipulation of object pointers is supported,
1. KiwiC performs automatic mapping of arrays and object heap to appropriate memory subsystems,
1. Channel communication between separately-compiled components, instead of manual bit banging of wires to implement a protocol,
1. Floating-Point is supported (custom precision F/P in the future),
1. Incremental compilation with IP-XACT wrapper generation and import for instantiated hierarchic blocks,
1. Mix hard and soft coding styles where clock cycle mapping is controlled from the C# file (for manual bit-banging of net-level protocols) or fully automatic (for 
   Scientific Acceleration),
1. Advanced register colouring taking into account wiring length under 2-D projection and multiplexor fan-in
1. Generates Verilog RTL output as well as a IP-XACT descriptions and SystemC behavioural and/or SystemC RTL-style models for whole-system modelling.
1. Some recursive programs are supported (unlimited recursion again in the future),
1. Compile on linux or Windows with existing substrate templates for Zynq, VC-707 and NetFPGA.

# Other Current Developments
Although Kiwi is a tool that mainly/essentially works, a lot of further development is envisioned. Apart from bug fixing, the main development/research areas for Kiwi at the moment are:

1. Even higher performance (mostly through enhanced, profile-directed tuning),
1. Spatially-aware register colouring,
1. Accurate performance prediction,
1. Custom-width floating point,
1. Better support for C# structs (as opposed to C# classes),
1. DRAM to cache prefetch,
1. Better debug support (including preserving vsibility name of C# local variable names),
1. Easier co-design when part of the application still runs under mono/Windows,
1. Dataflow (VSFG) execution engine,
1. Kiwi-2 full support for run-time dynamic storage allocation.

# Kiwi Use Cases

1. Kiwi can be used for generating custom accelerators to be embedded in other systems or it can be used as an execution platform for a complete scientific application [2].

1. The KiwiC compiler operates in several design styles. These vary in the amount of control the user has over the mapping of work to hardware clock cycles and the rate at which the resulting system can accept new input arguments [2].

1. Classical HLS generates a custom datapath and controlling sequencer for an application. The application may run once and exit or be organised as a server that goes busy when given new input data [2].

1. At the other extreme, we can generate a fully-pipelined, fixed-latency stream processor that tends not to have a controlling sequencer, but which instead relies on  predicated execution and a little backwards and forwards forwarding along its pipeline [2].

1. In all uses cases, the user's application is first coded in C# and can be developed and tested on the user's workstation using Visual Studio or Mono [2].

1. When high performance is required, the self-same binary file is further compiled using KiwiC for programmable hardware FPGAs [2].

1. FPGAs can use as little as 1/1000th of the energy and run 100 times faster than standard workstations. An everyday use of a hardware accelerator is the MPEG compression on a smart phone. This would instantly flatten the battery if done in software instead! [2]

# Kiwi Hardware Server and Real-Time Accelerator Synthesis

1. When generating a real-time accelerator, a C# function (method with arguments and return value) is designated by the user as the target root, either using a C# attribute or a command line flag to the KiwiC compiler. The user may also state the maximum processing latency. He will also typically state the reissue frequency, which could be once per clock cycle and whether stalls (flow control) is allowed [2].

1. For a real-time accelerator, multiple 'calls' to the designated function are being evaluated concurrently in the generated hardware. Operations on mutable state, including static RAMs and DRAM are allowed, but care must be taken over the way multiple executions appear to be interleaved, just as care is needed with re-entrant, multithreaded software operating on shared variables. Local variables are private to each invokation [2].

1. Note: real-time server mode is being implemented 3Q16. The prior 'hardware server' RPC-based implementation (LINK) is non-rentrant and does not enforce hard real time [2].

# Accelerating CPU-bound Applications

1. Applications that do not involve much I/O are always good candidates for FPGA execution since FPGA I/O performance and facilities have generally been inferior compared with standard processors, such an x86 motherboard [2].

1. Additionaly, applications that do not require a great deal of memory are also ideal for FPGA, since the FPGA DRAM controllers tend to be a little behind those on the latest x86 motherboards. However, many FPGAs now have considerable on-chip static RAM and fairly high bandwidth connection to larger co-located static RAMs, so memory footprint is not such an issue [2].

1. Kiwi works very well on intensive CPU-bound applications: particularly those that do a lot of bit-level operation and not much floating point. Typical examples are encryption, hashing and linear programming. The latest FPGAs now have much better quality floating-point support and this has recently extended the class of applications that benefit [2].

Kiwi is a form of acceleration for scientific applications and parallel programming. It uses the parallel constructs of the C# language and dotnet runtime. Specifically, Kiwi consists of a run-time library for native simulation of hardware descriptions within C# and the KiwiC compiler that generates RTL for FPGA from constrained/stylised .net bytecode [2].

Or in other words: Kiwi is developing a methodology for algorithm acceleration using parallel programming and the C# language. Specifically, Kiwi consists of a run-time library for hardware FPGA execution of algorithms expressed within C# and a compiler, KiwiC, that converts dotnet bytecode into Verilog RTL for further compilation for FPGA execution. In the future, custom domain-specific front ends that generate dotnet bytecode can be used [2].


# References
[1] https://www.cl.cam.ac.uk/~djg11/kiwi/
[2] https://www.cl.cam.ac.uk/research/srg/han/hprls/orangepath/kiwic.html
