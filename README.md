# Kiwi-compiler-HLS
Kiwi was developed at the University of Cambridge Computer Laboratory and Microsoft Research Limited, headed by David Greaves (UoCCL) and Satnam Singh (MRL). Kiwi continues at the Computer Laboratory as part of a logic synthesis project called HPR L/S and it uses the Orangepath core library.

# Overview
The Kiwi project aims to make reconfigurable computing technology like Field Programmable Gate Arrays (FPGAs) more accessible to mainstream programmers. FPGAs have a huge potential for quickly performing many interesting computations in parallel but their exploitation by computer programmers is limited by the need to think like a hardware engineer and the need to use hardware description languages rather than conventional programming languages.

# Asymptotic Background Motivation for FPGA Computing
The Von Neumann computer has hit a wall in terms of increasing clock frequency. It is widely accepted that Parallel Computing is the most energy-efficient way forward. The FPGA is intrinsically massively-parallel and can exploit the abundant transistor count of contemporary VLSI. Andre DeHon points out that the Von Neumann architecture no longer addresses the relevant problem: he writes "Stored-program processors are about compactness, fitting the computation into the minimum area possible".


# Why is computing on an FPGA becoming a good idea ?

Spatio-Parallel processing uses less energy than equivalent temporal processing (ie at higher clock rates) for various reasons. David Greaves gives nine:

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

 It is widely accepted that many problems in scientific computing can be vastly accelerated using either FPGA or GPU execution resources. Also, the FPGA approach in particular leads generally to a significant saving in execution energy. The product of the two gains can be typically one thousand fold.

Kiwi provides acceleration for multi-threaded (parallel) programs provided they can be converted to .NET bytecode. Originating from Microsoft, the .NET (also known as CIL) is a well-engineered, general purpose intermediate code that runs on many platforms, including mono/linux.

# Kiwi USPs - Unique Selling Points

There have been numerous high-level synthesis (HLS) projects in recent decades. Finally HLS has come of age, with all FPGA and EDA vendors offering HLS products. Nearly all of the prior work has used C, C++ or SystemC as the source HLL. (Historical note: Greaves CTOV compiler from 1995 is now owned by Synopsys Greaves-CTOV-1995 via the Tenison EDA sale.)

It is widely accepted that C# and mono/dotnet provide a significant leg up compared with C++ owing to crystal clear semantics, selectively checked overflows, neat higher-order functions and delegates, amenability to compiler optimisations and automated refactoring, garbage management, versioned assemblies and so on. Many of these benefits are most strongly felt with parallel programs. Also, the LINQ/Dryad extension is a clean route for manual invocations of accelerators.

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

