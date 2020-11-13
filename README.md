# Kiwi-compiler-HLS

# Overview
Kiwi is open source: you can download a source snapshot from HERE. (However there are still many bugs and a stable release is a few month's away!). Demo at FPL 2017, September, Ghent.

The Kiwi project aims to make reconfigurable computing technology like Field Programmable Gate Arrays (FPGAs) more accessible to mainstream programmers. FPGAs have a huge potential for quickly performing many interesting computations in parallel but their exploitation by computer programmers is limited by the need to think like a hardware engineer and the need to use hardware description languages rather than conventional programming languages.

# Asymptotic Background Motivation for FPGA Computing
The Von Neumann computer has hit a wall in terms of increasing clock frequency. It is widely accepted that Parallel Computing is the most energy-efficient way forward. The FPGA is intrinsically massively-parallel and can exploit the abundant transistor count of contemporary VLSI. Andre DeHon points out that the Von Neumann architecture no longer addresses the relevant problem: he writes "Stored-program processors are about compactness, fitting the computation into the minimum area possible".


# Why is computing on an FPGA becoming a good idea ?

Spatio-Parallel processing uses less energy than equivalent temporal processing (ie at higher clock rates) for various reasons. David Greaves gives nine:

1. Pollack's rule states that energy use in a Von Neumann CPU grows with square of its IPC. But the FPGA with a static schedule moves the out-of-order overheads to  
   compile time.
1. To clock CMOS at a higher frequency needs a higher voltage, so energy use has quadratic growth with frequency.
1. Von Neumann SIMD extensions greatly amortise fetch and decode energy, but FPGA does better, supporting precise custom word widths, so no waste at all.

