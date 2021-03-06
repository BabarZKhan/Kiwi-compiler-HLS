# Kiwi: Scientific Acceleration and Logic Synthesis from .net CIL Bytecode

High-Level Synthesis from Concurrent C#.

The KiwiC compiler takes CIL bytecode (dotnet portable assembly PE files) and generates RTL circuits for FPGA (or ASIC). This bytecode is generated by Microsoft .net tools and the mcs C# compiler from the mono project.


# Small, Hardware-oriented Demos
Here are some simple demos from early runs of the KiwiC compiler where illustrative C# programs are compiled to Verilog. All were compiled and run on linux and some have also been run on Windows. These demos explore hardware interfacing.


1. Times Table Test.

2. Seven Segment Display Driven by CSharp.

3. LCD DisplayTech 1602 Xilinx FPGA Eval Boards (ML605 or VC707).

4. Channel I/O Test.

5. Hello world, detailed step-by-step example.

6. Uninitialised Array Test.

7. On-chip and Off-chip primes counter.

8. Incremental Compilation and Remote Procedure Call (via IP-XACT).

9. Memory Hazards.

10. N-body demo.

11. Linked Lists.

10. Passing objects between threads.

11. Floating point, single and double precision.

12. Shared Memory using FPGA Dual-Port RAMs.

13. Higher-order programming with C# closures.

14. High-Level Synthesis (HLS) from Visual Basic.

15. Kiwi Zynq Substrate Programmed I/O and DMA Basic Demo.

16. Kiwi Bit Tally (Ones Counting) Comparison.

17. Common HLS Paradigms Demonstrated.

18. SystemC output for import of accelerators into virtual platforms, such as Prazor: Links missing.

19. HLS of Custom Arithmetic including Gustafson and Yonemoto's Posit Unum.

Although the above examples all use the C# front end and Kiwi library, other users of the KiwiC compiler have generated their .net code from C++ using gcc4cil.
