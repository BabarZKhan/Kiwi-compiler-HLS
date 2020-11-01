
$Id: README.txt $

University of Cambridge Computer Laboratory

README for hprls/hpr/meox and so on	- The main HPR Logic Synthesis and Co-synthesis library.

This page is planned to go out of date and be replaced with : http://www.cl.cam.ac.uk/research/srg/han/hprls/orangepath/

The HPR L/S library is all based around the HPR VM defined in abstract.hdr.

The aim is to 'seamlessly' model both hardware and software in a form
that suits easy co-synthesis and co-simulation.  It also includes
some temporal logic for assertions.  Software can exist as both machine
code/assembler and a high-level, block-structured, AST form.

Message-passing, CSP-like channels are another thing that should perhaps be added as a primitive form.
 CSP communication primitives should really be added ... to add channels and complete the picture.

A VM contains variable declarations, executable code, temporal logic
assertions and child machines.

A system is a tree of VMs where each may be the root of a tree of VMs.

Variables are signed and unsiged integers of various precisions,
single and double precision floating point and 1-D arrays of such
variables.  A small amount of string handling is also provided.  All
variable are static (no dynamic storage) and must be unique in a
single namespace that spans the system.  The variables are declared
inside a given VM and may be global or local.  Global variables may be
accessed by code and assertions in any VM and local ones should (not
enforced) only be accessed in locally (or in son machines?).

Expressions commonly use the hexp_t form and commands use the hbev_t
form.  Single-bit variables have hbexp_t form. A library of 'ix_xxx'
primitives can be called as functions or procedures from hexp_t, hbexp_t and
hbev_t respectively.  Expressions are all stored in a memoising heap
using weak pointers.


The executable code of a VM has several basic forms (dic, asm, rtl, cmd,
fsm).  All code and assertions access the variables for read and
write (but assertions don't tend to write!) regardless of form.

DIC - Directly-indexed array:  Imperative program (assign/conditional branch/builtin call) stored in an array indexed by a PC.
ASM - Assembler for a local family of microprocessors
RTL - Register transfer-level code - a set of parallel assignments to be executed on an event.
CMD - Abstract syntax tree of a behavioural program (for/while/break/continue/assign/if etc) or single assigment statement.
FSM - Finite state machine form - like RTL but collated into disjoint sets


The executable code may be clocked or nonclocked.  Fragments may be
put in serial or parallel using the SP_par and SP_seq combinators.
There are two variants of SP_par, for lockstep and asynchronous
composition.

Further executable forms, just being added are executable dataflow graphs:

  VSDG - a dataflow graph for a single basic block with additional state edges representing memory order constraints.
  VSFG - an executable form of the VSDG where back edges in the control flow graph are represented using nested graphs.





The library is structured as a number of components that operate on a
VM to return another VM.  The opath (orangepath) mini-language enables
a 'recipe' to be run that invokes a sequence of library operations in
turn.  An opath recipe is held in an XML file.


Automatic recipes: The overall systems is a pluggable library. Where
certain components only accept certain input forms and such a
component is specified to be used by a recipe, it is envisioned that
automatic invokation of the other components to serve as input
adaptors will be triggered.  Otherwise it is necessary to manually
instantiate additional recipe stages.




Loops in the recipe can be used to repeat a step until a property holds.

In principle it is possible to load and save VMs to disk (serialised in XML) and so incremental compilation
at intermediate points in the opath recipe is a future option.

The opath core provides command line handling so that parameters from
the recipe and the command line are fed to the components. The opath
core also processes a few 'early args' that must be at the start
of the command line. These enable the recipe file to be specified and
the logging level to be set.


Main/core files describing the VM:
  hprls_hdr.fs       - main expressions and commands
  abstract_hdr.fs    - main VM structures

  abstracte.fs       - Contains a compiler to convert block-structured forms (such as 'if' and 'while') into dic basic/fortran style code.
  abstractspare.fs
  linepoint_hdr.fs
  linprog.fs
  meox.fs             - memoizing heap with simple algebraic tautologies eliminated.

  compose.fs          - Combine separate FSMs together and reorganise ready for RTL generation.

Behavioural elaborators: convert threaded programs to a more formal FSM form (used to generate a 'pure RTL' parallel form but now that step is achieved by calling compose on just one machine ?)
  bevctrl.fs
  bevelab.fs

Cone of influence plugin - trims code that has not effect on observable outputs
  conerefine.fs

Static timing analyser
  stimer.fs

SystemC output plugin
   cpp_render.fs

Simulator
  diosim.fs


Performance predictor output renderer (currently boiled inside restructure.fs but needs splitting out)

Area predictor output renderer (currently boiled inside verilog_gen.fs but needs splitting out)


Datapath mapper pluging (experimental)
  dpmap.fs

Plugin to overcome structural hazards (c.f. dpmap)
  restructure.fs -  * ALU and RAM instances are created and structural hazards resolved.
  systolic.fs ?

Gate-level synthesiser plugin (normally not used - RTL output is fine normally)
  gbuild.fs
  (genericgates.fs - still in mosml folder)

XML input and output:
  hprxml.fs

Plugin Generate assembler or machine code output for a microcontroller
  microcode_hdr.fs


Opath is main framework:
   opath.fs
   opath_hdr.fs
   opath_interface.fs


Verilog output plugin
   verilog_gen.fs - this also bit-blasts to a gate-level net list if needed. It can also compute area estimates for circuits.
   verilog_hdr.fs
   verilog_render.fs

// planhdr.fs - generic interface to a planner/optimiser controller.   

Pseudo-canned IP Block Generator
  cvipgen.fs - This module automatically generates simulation models and perhaps concrete implementations for various generic leaf IP Blocks such as adders and RAMs.

Protocols:
   protocols.fs - IP-XACT input and output and some canned port definitions

Planner and Optimiser
  planhdr.fs
  planoptimise.fs - simple reference implementation

HPR System Integrator (in a different folder now)
   socrender.fs - The new (2Q2017) standalone tool that stitches IP blocks together and might make dynamic partition for multi-FPGA designs in the fullness of time.

Libraries:
   yout.fs  - Logging code
   moscow.fs - misc library (compatibility shim for moscow ML)
   tableprinter.fs
   dotreport.fs
END

---------------------------------------------------------

Coding styles:

  Aim to write so that the automatic conversion to OCaml works following uJamJar emails: details to be added here?


  Global variables should be g_


  The xi_ form calls are the prefered interface to meox.  These take curried (rather than tupled) args.


---------------------------------------------------------
Fsharp compiler: use one or the other


FSC=/usr/bin/fsharpc
FSC=mono $(FSHARP)/bin/fsc.exe /nowarn:75 /consolecolors- /nologo  /lib:. --nowarn:25,64



Q. I get the error

\verb+Error: Could not load file or assembly 'FSharp.Core, Version=4.4.0.0+

A. This is not related to any missing files in the Kiwi distro.
Instead it is do with FSharp version incompatibilities.  The
FSharp.Core is part of the FSharp system.  If you are using pre-built
dll files then the version of mono or FSharp on your system may be
incompatible with the pre-built dll files and you would have to change
version or else regenerate the dll files by compiling the FSharp
source code with the 'fsharpc' compiler on your system.

You may wish to just compile a trivial 'Hello World' FSharp program on
your system to check that FSharp is all set up ok.


---------------------------------------------------------

Debugging mono under gdb:

put this in .gdbinit in your home folder:
  define mono_backtrace
   select-frame 0
   set $i = 0
   while ($i < $arg0)
     set $foo = (char*) mono_pmip ($pc)
     if ($foo)
       printf "#%d %p in %s\n", $i, $pc, $foo
     else
       frame
     end
     up-silently
     set $i = $i + 1
   end
  end

then in gdb
 handle SIGXCPU SIG33 SIG35 SIGPWR nostop noprint
 attach pid // where pid can be obtained from top and so on
 mono_backtrace 15
-----------------------
Also:

define mono_stack
 set $mono_thread = mono_thread_current ()
 if ($mono_thread == 0x00)
   printf "No mono thread associated with this thread\n"
 else
   set $ucp = malloc (sizeof (ucontext_t))
   call (void) getcontext ($ucp)
   call (void) mono_print_thread_dump ($ucp)
   call (void) free ($ucp)
 end
end

Using "mono_stack" from gdb will print a managed stack trace to the program's stdout.It will not print out in your gdb console! You can also use
  thread apply all mono_stack
to print stacks for all threads. 
