

>>> A new release-notes.txt will be started here at the end of 2017 as we approach a stable version of Kiwi - until then you are on your own with no hint of support.



Kiwi Scientific Acceleration Project:  DJ Greaves and S Singh.
Compilation of parallel programs written in C# to H/W
University of Cambridge and Microsoft Research

------------------------------

# All rights reserved. (C) 2007-17, DJ Greaves, University of Cambridge, Computer Laboratory.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are
# met: redistributions of source code must retain the above copyright
# notice, this list of conditions and the following disclaimer;
# redistributions in binary form must reproduce the above copyright
# notice, this list of conditions and the following disclaimer in the
# documentation and/or other materials provided with the distribution;
# neither the name of the copyright holders nor the names of its
# contributors may be used to endorse or promote products derived from
# this software without specific prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
# "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
# LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
# A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
# OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
# LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
# DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
# THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
# OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#

Kiwic : Kiwi Project Compiler (C) 1999-2017 DJ Greaves.

There is no such thing as a stable release to date.  This is
experimental software for academic partner investigation only. No
liability is assumed for any consequences from use or misuse or
implied warranties or patent infringements.


OLD NOTICE: All of the IP and software in the compiler directory and its
subdirectories belongs to David Greaves of the University of
Cambridge.  It cannot be copied, distributed or used commercially
without explicit permission.  This is experimental software: no
guarantees are implied and no liability is assumed for consequential
loss.

The above OLD NOTICE was revoked in January 2017 when the code base was made open source.

------------------------------



>>> A new set of update notes will be started here at some point in 2017 - until then you are on your own with no hint of support.

Most recent mods: 
  May 2017: a little better area metric to get the AES complexity demo plotted.
  May 2017: minor tidy up ready for IP-XACT and SoC Render code to be merged in.
  May 2017: recommenced this DISTRO-README.txt file after six year gap!




------------------------------


Linux users:

Basic operation (read doc/kiwic.pdf but its a bit out of date):
Assuming the distro is at PREFIX, use a makefile as follows: 


   ||KLIB0=$(PREFIX)/support/Kiwi.dll
   ||KIWIC=$(PREFIX)/bin/kiwic 
   ||
   ||framestore.exe:framestore.cs
   ||        mcs framestore.cs /r:$(KLIB0)
   ||
   ||framestore:framestore.exe
   ||        $(KIWIC) framestore.exe -vnl verilogoutput.v


Windows users:

In the windows folder there is an example .bat file that compiles one example.

'Kiwi.HardwareEntryPoint' attributes will be supported to remove the need
for command line options such as -root, but these are not appropriate for a 
multi-stage compilation where a Makefile wants to set a different root for
each stage.
---------------

8th August 2011 (version 54a-Interim)
   Quite a big rewrite just about completed: many designs that have been failing to compile since
the start of 2011 are working again, but not arrays of mutex's or some pointer twiddles.
   The new front end elaborator is present and this has eliminated replications of pause statements.


General Current Release Problems:

  bubblesorter.v - works without dram connected?
  There is a bug in postfix ++ applied to array location fixed (e.g. foo[subsc]++; )


  - bypass-verilog-roundtrip=false for RTL output testing is broken in a number of tests
  - test14 and related tests (parfir parallel convolver) that used to work with object handle manipulation are broken owing to a jan2011 regression.
  - Arrays of mutex's are broken: ("BROKEN IN THIS SPECIFIC RELEASE OF KIWIC mutex dot tag=" + htos tag_idl + "\n  ce=" + ceToStr (sbs()))
  - Bblock pause mode causes pli calls to be executed twice


   - There are some race conditions in values set up in constructors versus values read by the main threads!
   - Record (C# struct) types needed for fftbench test are not working
 
   - Bit-oriented bitwise I/O as used in framestore example.
   - Hard and auto Pause Control modes (the other two work)
   - Input from gcc4cil is likely not working (not tested recently)


----------------------
Very old, older notes:



1st July 2011 (version 53r)
   The SystemCsharp output mode is now working sufficiently for the ParticlePlotter application to work.

23rd June 2011 (Version 53p)
   Automatic load of Kiwic.dll Kiwi.dll from support folder (location overridable on command line (eg set to -kiwic-dll="" to disable autoload)).
   VCD output file from diosim can now be enabled and named on the command line (eg. add -sim=1000000 -diosim-vcd=myvcd.vcd to command line).
   Bug fix of width attribute on io ports
   Order of nets in RTL module now corresponds to definition order.
   Fixed bug in unwinder that was stopping Satnam's DVIcolourbars working, but now test21 has broken (unwinds forever in bevelab stage) can revert to old algorithm with -kiwic-oldunwind=enable

20th June 2011 (Version 53n)
   Bi-simulation reduction introduced to cut down on state space.


7th June 2011 (Version 53h)
   Turned on structural hazard recipe stage by default (turn it off again with -restructure2=disable if you want)
   Automatic allocation of offchip address space in dram banks
   The KiwiHardwareEntryPoint attribute is now used instead of Kiwi.Hardware
   Some bugs in postfix++ fixed (another remains? todo) 


13 May (Version 53e)
  Ansi rather than kandr Verilog module syntax.


10 May (Version 53d) fixes:
  Higher order multi-dimensional arrays (3d, 4d) now suported
  Custom port widths work again (e.g. in test18)
  Indexing a character string works again (test9)

Release 52s - Offchip attribute now working again.  Arrays of mutexes still need fixing.

Release 52m - the christmas rewrite is done more or less by arrays of mutexs are broken and the new name alias resoultion (e.g. for the BlueSpec example) is not completely finished.  Anyway-sufficient to run the Memocode11 examples.

Release 0.51-l does not ignore the attributes of certain fields (e.g. the io pads in fftbench)

Release 0.51i does not include hardware attributed marked components twice

Release 0.51a no longer contains a canned copy of System.Threading.Monitor and instead the KiwiSystem.Kiwic.dll needs to be added to the kiwic commandline (ie include both Kiwic.dll and Kiwi.dll)

Release 51f no longer generates an error on code such as
     array[x] += 1;

Release 0.51h automatically scans through class definitions for static fields on demand rather than needing them to be explicitly included using a -root or Kiwi.Hardware attribute.

END
