# Kiwi Scientific Acceleration : KiwiC Compiler Makefile
#
# Built on HPR L/S Formal Codesign System.
#
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


ifndef HPRLS
  $(info Please set HPRLS env var to the parent of the parent directory: the one containing hpr and kiwpro.)
  $(error HPRLS env var not set)
endif


export KDISTRO=$(HPRLS)/kiwipro/kiwic/kdistro
export DISTDIR=$(KDISTRO)

KLIB0=${KDISTRO}/support/Kiwic.dll
KLIB1=${KDISTRO}/support/Kiwi.dll
MONO ?= mono
KIWIC= $(MONO)  $(KDISTRO)/lib/kiwic.exe  

# -gtrace-loglevel=0

HPRDIR=../../../hpr



KIWICDLL=cecilfe.dll cilgram.dll asmout.dll kiwi_canned.dll  $(CECIL) intcil.dll cilnorm.dll kcode.dll firstpass.dll



all:  mk_kdistro
# run_tinytests
	echo "Make target 'all' now complete"

# Synonym
tool: mk_kdistro


recomp:
	cd tiny; $(KDISTRO)/bin/kiwic  tiny.dll -resets asynchronous -root "tiny;tiny.Main" -vnl tiny.v


.phoney: mk_kdistro tool run_tinytests old_exportit

run_tinytests:
	cd tinytests ; make


allcil:	hprls.dll $(KIWICDLL) kiwic.exe


#------------------------------------------------------------------------------
# Make a K-Distro
# This is the distribution at the end of a compile from source, for local export of KiwiC and its libraries. It is what you might expect would be copied to /usr/lib after a 'make install' or 'rpm -i'.
#
mk_kdistro:allcil
	mkdir -p $(KDISTRO)/lib/recipes $(KDISTRO)/bin $(KDISTRO)/doc $(KDISTRO)/examples $(KDISTRO)/support $(KDISTRO)/tinytests
	cp release-notes.txt $(KDISTRO)
	cp DISTRO-README.txt $(KDISTRO)
# Do not need this foreign FSharp.Core.dll when FSharp is properly installed. It breaks things on properly-installed systems.
#	cp $(FSHARP)/bin/FSharp.Core.dll $(KDISTRO)/lib
	cp $(KIWICDLL) kiwic.exe $(KDISTRO)/lib
	cp -ra $(HPRDIR)/hpr_ipblocks $(KDISTRO)/lib
	cp $(CECIL) $(KDISTRO)/lib/Mono.Cecil.dll # Note this copy renames it.
	cp recipes/*.rcp  $(KDISTRO)/lib/recipes
	cp kiwic $(KDISTRO)/bin
	chmod +x $(KDISTRO)/bin/kiwic
	cp ../../kiwi/userlib/Kiwi.dll ../../kiwi/userlib/Kiwi.cs  $(KDISTRO)/support
	cp ../../kiwi/userlib/Kiwic.dll ../../kiwi/userlib/Kiwic.cs  $(KDISTRO)/support
	cp ../../kiwi/userlib/KiwiStringIO.dll ../../kiwi/userlib/KiwiStringIO.cs  $(KDISTRO)/support
	cp ../../kiwi/userlib/KiwiSystem.Random.dll ../../kiwi/userlib/KiwiSystem.Random.cs  $(KDISTRO)/support
	cp ../../kiwi/userlib/KiwiSystem.Threading.Barrier.dll ../../kiwi/userlib/KiwiSystem.Threading.Barrier.cs  $(KDISTRO)/support
	cp ../../kiwi/userlib/KiwiSystem.Threading.Interlocked.dll ../../kiwi/userlib/KiwiSystem.Threading.Interlocked.cs  $(KDISTRO)/support
	mkdir -p $(KDISTRO)/support/filesystem ;	cp ../../kiwi/filesystem/kiwifs_bev.v  $(KDISTRO)/support/filesystem
	mkdir -p $(KDISTRO)/support/performance_predictor ;	cp ../../kiwi/performance_predictor/*.v  $(KDISTRO)/support/performance_predictor
	cp ../../kiwi/userlib/KiwiFilesystemStubs.*   $(KDISTRO)/support/filesystem
	mkdir -p $(KDISTRO)/support/rams ;	cp ../../kiwi/rams/ddr2-models.v  $(KDISTRO)/support/rams
#	mkdir -p $(KDISTRO)/support/rams ;	cp ../../kiwi/rams/generic_large_kiwi_rams.v  $(KDISTRO)/support/rams


	mkdir -p $(KDISTRO)/examples/primesya
	cd ../../kiwi/examples/primesya; cp Makefile primesya.cs primes-offchip-testbench.v  vsys.v  $(KDISTRO)/examples/primesya
# offchip.v onchip.v


	cd tinytests  ; cp tinytest0.cs Makefile $(KDISTRO)/tinytests

#------------------------------------------------------------------------------
# Please explain - This is an old distro - many of these demos are now missing from git
old_exportit:mk_kdistro
#	cp $(FSHARP2)/bin/FSharp.Core.dll $(KDISTRO)/lib
#
	mkdir -p $(KDISTRO)/SystemCsharp
	cd ../../systemcsharp; cp Makefile clkgen.cs SystemCsharp.cs clkgen.dll SystemCsharp.dll example1.cs README.txt  $(KDISTRO)/SystemCsharp
#
	mkdir -p $(KDISTRO)/examples/networksystem
	cd ../examples/networksystem; cp Makefile simpleTestModuleOne.cs simpleTestModuleOne.v  $(KDISTRO)/examples/networksystem
#
	mkdir -p $(KDISTRO)/examples/smithwaterman/serial
	mkdir -p $(KDISTRO)/examples/smithwaterman/parallel
	cd ../examples/smithwaterman/serial; cp Makefile sw_test.cs vsys.v sw_serial_offchip_testbench.v $(KDISTRO)/examples/smithwaterman/serial
#  DUT.v is missing
	cd ../examples/smithwaterman/parallel; cp Makefile vertsw.cs testbench.cs database.v VSYS.v $(KDISTRO)/examples/smithwaterman/parallel
#
	mkdir -p $(KDISTRO)/examples/bubblesort
	cd ../examples/bubblesort; cp Makefile bubblesort.cs SIMSYS.v bubblesort.v bubblesort.dll $(KDISTRO)/examples/bubblesort
#
	mkdir -p $(KDISTRO)/windows
	cd windows; cp runsmall.bat README.txt $(KDISTRO)/windows
#
	mkdir -p $(KDISTRO)/examples/tiny
	cd tiny; cp Makefile tiny.cs tiny.v vsys.v tiny.dll $(KDISTRO)/examples/tiny
#
	mkdir -p $(KDISTRO)/examples/fifo2
	cd ../examples/fifo2; cp makefile fifo2.cs diosim.out.gold fifo2.v FIFOSYS.v default.sig $(KDISTRO)/examples/fifo2
#
	mkdir -p $(KDISTRO)/examples/convolvers
	mkdir -p $(KDISTRO)/examples/convolvers/Sequential
	mkdir -p $(KDISTRO)/examples/convolvers/Parallel
	cd ../examples/convolvers; cp Makefile SequentialFIR.cs DUT.v $(KDISTRO)/examples/convolvers/Sequential
	cd ../examples/parfir1; cp Makefile ParallelConvolver-branded.cs  ParallelConvolver.cs simsys.v waveforms.gif $(KDISTRO)/examples/convolvers/Parallel
#  ParallelConvolver-branded.v is missing
#
	mkdir -p $(KDISTRO)/examples/sorters
	cd ../examples/sorters; cp Makefile bitonic.cs  $(KDISTRO)/examples/sorters
#  shellsort.cs missing
#
	mkdir -p $(KDISTRO)/examples/fftbench
	cd ../examples/fftbench; cp Makefile fftbench.cs  vsys.v  intcoefs_15_1024.cs $(KDISTRO)/examples/fftbench
# DUT.v spool


	mkdir -p $(KDISTRO)/examples/kthmax
	cd ../examples/kthmax; cp Makefile kthmax.cs vsys.v  $(KDISTRO)/examples/kthmax
#  DUT.v


	mkdir -p $(KDISTRO)/examples/sorters
	cd ../examples/sorters; cp Makefile systolic.cs main.cs bitonic.cs   $(KDISTRO)/examples/sorters
# DUT.v
	mkdir -p $(KDISTRO)/examples/primesya
	cd ../../kiwi/examples/primesya; cp Makefile primesya.cs primes-offchip-testbench.v vsys.v  $(KDISTRO)/examples/primesya
# offchip.v onchip.v 


	mkdir -p $(KDISTRO)/examples/framestore
	cd ../examples/framestore; cp makefile KiwiFramestore.cs KiwiFramestore.v SIMSYS.v default.sig $(KDISTRO)/examples/framestore
#
	rm -f kiwic.pdf
	wget http://www.cl.cam.ac.uk/research/srg/han/hprls/orangepath/manual/kiwic.pdf
	cp kiwic.pdf  $(KDISTRO)/doc
	cd $(KDISTRO)/.. ; zip -r kiwic-anon.zip distro
	cp $(KDISTRO)/../kiwic-anon.zip /home/httpd/html/kiwi-private/dists

.PHONEY: hprls.dll
hprls.dll:
	cd $(HPRDIR); make distro

#tool; 
#	cd ../regression; make test6b
#	cd ../examples/fifo2; make fifo2ps
#	cd ../examples/parfir1; make


# KiwiC uses Mono.Cecil.dll - set this to where your's is, e.g. in /usr/lib/mono/4.5-api
# You might find it using 	gacutil -l
CECILDIR=$(HPRLS)/kiwipro/kiwic/src/lib
CECIL=$(CECILDIR)/Mono.Cecil-kiwi.dll
# CECIL=/usr/lib/mono/gac/Mono.Cecil/0.6.9.0__0738eb9f132ed756/Mono.Cecil.dll
#CECIL=/usr/lib/mono/4.5-api/Mono.Cecil.dll


gaclist:
	gacutil -l

#
HPR ?=$(HPRLS)/hpr

# The libraries we are sensitive to is a subset of all the library dlls.
HPRLIBS= $(HPR)/moscow.dll  $(HPR)/yout.dll \
	 $(HPR)/opath_hdr.dll   $(HPR)/linepoint_hdr.dll \
	 $(HPR)/linprog.dll   $(HPR)/hprls_hdr.dll \
	 $(HPR)/abstract_hdr.dll  $(CECIL) \
	 $(HPR)/opath_interface.dll  \
	 $(HPR)/abstracte.dll   $(HPR)/meox.dll \
	 $(HPR)/ksc.dll \
	 $(HPR)/tableprinter.dll \

HPRLIBS_SANS_SENSITIVITY= \
	 $(HPR)/systolic.dll         $(HPR)/opath.dll \
	 $(HPR)/restructure.dll      $(HPR)/conerefine.dll \
	 $(HPR)/Djg.dll              $(HPR)/microcode_hdr.dll \
	 $(HPR)/repack.dll           $(HPR)/espresso.dll \
	 $(HPR)/gbuild.dll           $(HPR)/plot.dll \
	 $(HPR)/hprxml.dll           $(HPR)/bevelab.dll \
	 $(HPR)/diosim.dll           $(HPR)/compose.dll \
	 $(HPR)/verilog_render.dll   $(HPR)/verilog_hdr.dll  $(HPR)/verilog_gen.dll   \
	 $(HPR)/dpmap.dll            $(HPR)/revast.dll \
	 $(HPR)/cpp_render.dll       $(HPR)/fitters.dll \
	 $(HPR)/reporter.dll

HPRLIBSR=-r $(HPR)/moscow.dll      -r $(HPR)/yout.dll \
	-r $(HPR)/opath_hdr.dll    -r $(HPR)/linepoint_hdr.dll \
	-r $(HPR)/linprog.dll      -r $(HPR)/hprls_hdr.dll \
	-r $(HPR)/abstract_hdr.dll -r $(CECIL) \
	-r $(HPR)/Djg.dll          -r $(HPR)/microcode_hdr.dll -r $(HPR)/protocols.dll \
	-r $(HPR)/repack.dll       -r $(HPR)/espresso.dll \
	-r $(HPR)/abstracte.dll    -r $(HPR)/meox.dll \
	-r $(HPR)/restructure.dll  -r $(HPR)/conerefine.dll \
	-r $(HPR)/cpp_render.dll   -r $(HPR)/opath.dll \
	-r $(HPR)/dotreport.dll    -r $(HPR)/systolic.dll \
	-r $(HPR)/gbuild.dll       -r $(HPR)/plot.dll \
	-r $(HPR)/fitters.dll         -r $(HPR)/ksc.dll \
	-r $(HPR)/hprxml.dll          -r $(HPR)/bevelab.dll \
	-r $(HPR)/diosim.dll          -r $(HPR)/compose.dll \
	-r $(HPR)/verilog_render.dll  -r  $(HPR)/verilog_hdr.dll -r $(HPR)/verilog_gen.dll   \
	-r:$(HPR)/opath_interface.dll -r:$(HPR)/tableprinter.dll \
        -r:$(HPR)/dpmap.dll           -r:$(HPR)/revast.dll \
        -r:$(HPR)/reporter.dll   


# The CSharp compiler, mono and the FSharp compiler is defined in the include file
include $(HPRLS)/hpr/Makefile.inc

# CSharp compiler (mcs or gmcs)
CSC   ?=$(GMCS) -debug+  -r:$(KLIB1)

kiwic.exe:kiwic.fs cilnorm.dll cilgram.dll $(HPRLIBS)
	$(FSC) kiwic.fs $(HPRLIBSR) -r cilgram.dll -r cecilfe.dll  -r kiwi_canned.dll -r cilnorm.dll -r intcil.dll -r kcode.dll -r firstpass.dll  -r asmout.dll 

cilgram.dll:cilgram.fs $(HPRLIBS) 
	$(FSC) /target:library cilgram.fs $(HPRLIBSR)


asmout.dll:asmout.fs cilgram.dll $(HPRLIBS) 
	$(FSC) /target:library asmout.fs -r $(HPR)/meox.dll  $(HPRLIBSR) -r cilgram.dll

cecilfe.dll:cecilfe.fs asmout.dll cilgram.dll $(HPRLIBS)
	$(FSC) /target:library cecilfe.fs $(HPRLIBSR) -r asmout.dll -r cilgram.dll

kiwi_canned.dll:kiwi_canned.fs  $(HPRLIBS)  cilgram.dll asmout.dll
	$(FSC) --target:library kiwi_canned.fs $(HPRLIBSR) -r  cilgram.dll -r asmout.dll


kcode.dll:$(HPRLIBS) kcode.fs cilgram.dll asmout.dll kiwi_canned.dll cecilfe.dll
	$(FSC) --target:library kcode.fs $(HPRLIBSR) -r cilgram.dll -r asmout.dll -r kiwi_canned.dll -r asmout.dll -r cecilfe.dll

firstpass.dll:$(HPRLIBS) firstpass.fs asmout.dll  kcode.dll
	$(FSC) --target:library firstpass.fs $(HPRLIBSR) -r cilgram.dll -r asmout.dll -r kiwi_canned.dll -r asmout.dll -r kcode.dll

intcil.dll:$(HPRLIBS) intcil.fs cilgram.dll asmout.dll kiwi_canned.dll kcode.dll firstpass.dll
	$(FSC) --target:library intcil.fs $(HPRLIBSR) -r cilgram.dll -r asmout.dll -r $(HPR)/abstracte.dll -r $(HPR)/abstract_hdr.dll -r $(HPR)/protocols.dll -r kiwi_canned.dll -r asmout.dll  -r kcode.dll -r firstpass.dll

cilnorm.dll:$(HPRLIBS) cilnorm.fs cilgram.dll asmout.dll kiwi_canned.dll kcode.dll firstpass.dll intcil.dll cecilfe.dll 
	$(FSC) --target:library cilnorm.fs  $(HPRLIBSR) -r cilgram.dll -r asmout.dll -r kiwi_canned.dll -r asmout.dll  -r kcode.dll -r firstpass.dll -r intcil.dll -r cecilfe.dll 



# zip simple backup
zip:
	zip kiwicsrc.zip *.fs Makefile DISTRO-README.txt small0/tiny.cs small0/Makefile small0/vsys.v recipes lib/*dll ../../kiwi/examples/primesya/*.v ../../kiwi/rams/*.v ../../kiwi/userlib/*.cs ../regression/Makefile ../regression/expecteds/* ../regression/*.cs ../../doc/*.tex ../../doc/images/*.obj ../../doc/*.obj  ../../../docroot/*.obj  ../../../docroot/*.tex


SPIN=$(HOME)/packages/Spin/Src5.1.4/spin -n100
spinfull:
	echo $(SPIN) -a fromcil.spin
	$(SPIN) -a fromcil.spin
	gcc -DSAFETY -Wall pan.c -o /tmp/panmono
	rm -f test.spin.trail; time /tmp/panmono
	$(SPIN) -t -r fromcil.spin



# gchannel
#
# Other test targets: 
# transactors
# ok i2c2 = subdir
# i2cdemo = subdir
# ok but no i/o s : perhaps ok i2c 
# ok gchannel and/or channel
# ok udi (* fails now *)
# ok colourbars
# ok timestable
# ok blowfish (C++)
# ok arrays1



#--------------------------------------
# Use -GLOBALS to catch the outermost scope of the assembly
UDIS=tests/UDI_for_cil.s
udi:
	./kiwic $(UDIS) -root "-GLOBALS;UDI_Load_Internal_Single" -vnl udi_load_internal_single.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_Load_Internal_Double" -vnl udi_load_internal_double.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK1_1" -vnl udi_block1_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK1_2" -vnl udi_block1_2.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK2_1" -vnl udi_block2_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK1_2" -vnl udi_block1_2.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK7_1" -vnl udi_block7_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_10" -vnl udi_block9_10.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_5" -vnl udi_block9_5.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_9" -vnl udi_block9_9.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_2" -vnl udi_block9_2.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_8" -vnl udi_block9_8.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_3" -vnl udi_block9_3.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_7" -vnl udi_block9_7.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_1" -vnl udi_block9_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_6" -vnl udi_block9_6.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK9_4" -vnl udi_block9_4.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK11_1" -vnl udi_block11_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK11_2" -vnl udi_block11_2.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK12_1" -vnl udi_block12_1.v
	./kiwic $(UDIS) -root "-GLOBALS;UDI_BLOCK12_2" -vnl udi_block12_2.v

udi_diffs:
	diff  udi_load_internal_single.v /tmp/tidyup
	diff  udi_load_internal_double.v /tmp/tidyup
	diff  udi_block1_1.v /tmp/tidyup
	diff  udi_block1_2.v /tmp/tidyup
	diff  udi_block2_1.v /tmp/tidyup
	diff  udi_block1_2.v /tmp/tidyup
	diff  udi_block7_1.v /tmp/tidyup
	diff  udi_block9_10.v /tmp/tidyup
	diff  udi_block9_5.v /tmp/tidyup
	diff  udi_block9_9.v /tmp/tidyup
	diff  udi_block9_2.v /tmp/tidyup
	diff  udi_block9_8.v /tmp/tidyup
	diff  udi_block9_3.v /tmp/tidyup
	diff  udi_block9_7.v /tmp/tidyup
	diff  udi_block9_1.v /tmp/tidyup
	diff  udi_block9_6.v /tmp/tidyup
	diff  udi_block9_4.v /tmp/tidyup
	diff  udi_block11_1.v /tmp/tidyup
	diff  udi_block11_2.v /tmp/tidyup
	diff  udi_block12_1.v /tmp/tidyup
	diff  udi_block12_2.v /tmp/tidyup

udi.zip:
	zip udi.zip udi_*.v
	mv udi.zip ~

# Pushlogic code reflection example
Pioneer:
	./kiwic PioneerMinor.cil  -root "-GLOBALS;Works;Devices;Local;Parts;execs" -ubudget 10000000 -vnl p.vnl -smv smv.smv

channel:
	./kiwic channels/channels.S -root "TimesTable;TimesTable.Main" -vnl channels.v -smv smv.smv

gchannel:
	./kiwic channels/gchannel.S -root "TimesTable;TimesTable.Main" -vnl VNL.v -sim 280 -sim-rtl 1 
	cs -clearnet simsys.v -nox VNL.v


blowfish: blowfish1 blowfish2 blowfish3 blowfish4

blowfish1:
	./kiwic tests/custom_f.cil  -root UDI_BLOCK1_1  -vnl b1.v
blowfish2:
	./kiwic tests/custom_f.cil  -root UDI_BLOCK1_2  -vnl b2.v
blowfish3:
	./kiwic tests/custom_f.cil  -root UDI_BLOCK1_3  -vnl b3.v
blowfish4:
	./kiwic tests/custom_f.cil  -root UDI_BLOCK1_4  -vnl b4.v


# -sim 1 -plot "timetable1.plt"  -gatelib DEFAULT  -sim-rtl 5001 
# 
#	diogif -o a.gif -x timetable1.plt

clean:
	rm -f *.uo *.ui
#	cd $(HPRLS)/toolmisc; make clean
#	cd $(HPRLS)/h2tool; make clean
#	cd $(HPRLS)/kiwic/cilfe; make clean


#---------------------------------------------------------------------
# Source distro: Make a src distribution for those not yet invited to git.
SD=/tmp/kiwi_sa_src_dist
SD_FROM=$(HPRLS)
SD_NAME=kiwi_sa_src_dist.190309.zip # YYMMDD
# Manually adjust the date in SD_NAME.  Put year first for chrono folder sort.
srcdist:
	rm -rf $(SD) $(SD)/../$(SD_NAME)
	mkdir -p $(SD)
	mkdir -p $(SD)/hpr/prebuilt
	date > $(SD)/snap-date.txt

# Release Notes
	cp release-notes.txt $(SD)

	mkdir -p $(SD)/misc
# HPRLS
	cp -a $(SD_FROM)/hpr/*.fs  $(SD_FROM)/hpr/Makefile $(SD_FROM)/hpr/README.txt $(SD_FROM)/hpr/Makefile.inc $(SD)/hpr
	cp -a $(SD_FROM)/hpr/*.dll   $(SD)/hpr/prebuilt
# IP BLOCKS
	mkdir -p $(SD)/hpr/hpr_ipblocks/cvip0
	cp -ra $(SD_FROM)/hpr/hpr_ipblocks  $(SD)/hpr

# DOC
	mkdir -p $(SD)/kiwipro/doc
	cp -a $(SD_FROM)/kiwipro/doc/kiwic.pdf  $(SD)/kiwipro/doc

# KIWIC SRC
	mkdir -p $(SD)/kiwipro/kiwic/src
	cp -a $(SD_FROM)/kiwipro/kiwic/src/*.fs  $(SD_FROM)/kiwipro/kiwic/src/DISTRO-README.txt $(SD_FROM)/kiwipro/kiwic/src/kiwic $(SD_FROM)/kiwipro/kiwic/src/Makefile $(SD)/kiwipro/kiwic/src
	mkdir -p $(SD)/kiwipro/kiwic/src/prebuilt
	cp -a $(SD_FROM)/kiwipro//kiwic/src/*.dll $(SD_FROM)/kiwipro/kiwic/src/*.exe $(SD)/kiwipro/kiwic/src/prebuilt
	cp -ra $(SD_FROM)/kiwipro/kiwic/src/recipes $(SD)/kiwipro/kiwic/src

	cp $(SD_FROM)/kiwipro/kiwic/distro/lib/Mono.Cecil-kiwi.dll $(SD)/misc
# REGRESSION
	mkdir -p $(SD)/kiwipro/kiwic/regression/expecteds
	cp -a $(SD_FROM)/kiwipro/kiwic/regression/*.cs $(SD_FROM)/kiwipro/kiwic/regression/{mygrep,runtest} $(SD_FROM)/kiwipro/kiwic/regression/*.cpp $(SD_FROM)/kiwipro/kiwic/regression/*.h $(SD_FROM)/kiwipro/kiwic/regression/*.v $(SD_FROM)/kiwipro/kiwic/regression/Makefile  $(SD)/kiwipro/kiwic/regression
	cp -ra $(SD_FROM)/kiwipro/kiwic/regression/expecteds $(SD)/kiwipro/kiwic/regression


# USERLIB
	mkdir -p $(SD)/kiwipro/kiwi/userlib
	cp ../../kiwi/userlib/Kiwi.dll ../../kiwi/userlib/Kiwi.cs  $(SD)/kiwipro/kiwi/userlib
	cp ../../kiwi/userlib/Kiwic.dll ../../kiwi/userlib/Kiwic.cs  $(SD)/kiwipro/kiwi/userlib
	cp ../../kiwi/userlib/KiwiStringIO.dll ../../kiwi/userlib/KiwiStringIO.cs  $(SD)/kiwipro/kiwi/userlib
	cp ../../kiwi/userlib/KiwiSystem.*.dll ../../kiwi/userlib/KiwiSystem.*.cs  $(SD)/kiwipro/kiwi/userlib
#	cp ../../kiwi/userlib/KiwiSystem.Threading.Barrier.dll ../../kiwi/userlib/KiwiSystem.Threading.Barrier.cs  $(SD)/kiwipro/kiwi/userlib
#	cp ../../kiwi/userlib/KiwiSystem.Threading.Interlocked.dll ../../kiwi/userlib/KiwiSystem.Threading.Interlocked.cs  $(SD)/kiwipro/kiwi/userlib
#	cp ../../kiwi/userlib/KiwiSystem..dll ../../kiwi/userlib/KiwiSystem..cs  $(SD)/kiwipro/kiwi/userlib


# OLD FILE SYSTEM API - TO BE REPLACED WITH NoC and Zynq shims soon - actually this does work on the NoC now?
	mkdir -p $(SD)/kiwipro/kiwi/filesystem
	cp ../../kiwi/filesystem/kiwifs_bev.v   $(SD)/kiwipro/kiwi/filesystem

# EXAMPLES
	mkdir -p $(SD)/kiwipro/kiwi/examples

	mkdir -p $(SD)/kiwipro/kiwi/examples/primesya
	cd ../../kiwi/examples/primesya; cp Makefile *.v *.cs *.txt $(SD)/kiwipro/kiwi/examples/primesya

	mkdir -p $(SD)/kiwipro/kiwi/examples/chstone/dfsin
	cd ../../kiwi/examples/chstone; cp *.cs  $(SD)/kiwipro/kiwi/examples/chstone
	cd ../../kiwi/examples/chstone/dfsin; cp Makefile *.cpp *.v *.cs  $(SD)/kiwipro/kiwi/examples/chstone/dfsin

	mkdir -p $(SD)/kiwipro/kiwi/examples/cuckoo-cache
	cd ../../kiwi/examples/cuckoo-cache; cp Makefile *.cpp *.v *.cs  $(SD)/kiwipro/kiwi/examples/cuckoo-cache


# ZIP UP
	cd $(SD)/.. ; zip -r $(SD_NAME) $(SD)
	echo Done

srcdist-export:
	cp $(SD)/../$(SD_NAME) /home/httpd/html/kiwic-download/dists




#---------------------------------------------
# Make a binary distributon - no longer used
BD=/tmp/kiwic-tarball
binarydist:
	rm -rf $(BD) /tmp/kiwic.tar.gz
	mkdir -p $(BD)/kiwi

# Example 0
	mkdir -p $(BD)/examples/parfir1
	cp ../examples/parfir1/gold-output $(BD)/examples/parfir1
	cp ../examples/parfir1/Makefile $(BD)/examples/parfir1
	cp ../examples/parfir1/ParallelConvolver.cs $(BD)/examples/parfir1
	cp ../examples/parfir1/ParallelConvolver-branded.cs $(BD)/examples/parfir1
	cp ../examples/parfir1/ParallelConvolver-branded.v $(BD)/examples/parfir1
	cp ../examples/parfir1/SELFTEST.v $(BD)/examples/parfir1
	cp ../examples/parfir1/simsys.v $(BD)/examples/parfir1
	cp ../examples/parfir1/run.do $(BD)/examples/parfir1

# Example 1
	mkdir -p $(BD)/examples/fifo2
	cp ../examples/fifo2/makefile $(BD)/examples/fifo2
	cp ../examples/fifo2/fifo2.cs $(BD)/examples/fifo2
	cp ../examples/fifo2/FIFOSYS.v $(BD)/examples/fifo2
	cp ../examples/fifo2/run.do $(BD)/examples/fifo2

# Example 2 
	mkdir -p $(BD)/examples/primes
	cp ../examples/primesya/makefile $(BD)/examples/primes/makefile
	cp ../examples/primesya/primesya.cs $(BD)/examples/primes
	cp ../examples/primesya/run.do $(BD)/examples/primes
	cp ../examples/primesya/vsys.v $(BD)/examples/primes
	cp ../examples/primesya/DUT.v $(BD)/examples/primes
	cp ../examples/primesya/primes-offchip-testbench.v   $(BD)/examples/primes

# Example n 
	mkdir -p $(BD)/examples/framestore
	cp ../examples/framestore/makefile $(BD)/examples/framestore/makefile
	cp ../examples/framestore/framestore.cs $(BD)/examples/framestore
#	cp ../examples/framestore/run.do $(BD)/examples/framestore
	cp ../examples/framestore/SIMSYS.v $(BD)/examples/framestore


# Root items
	cp ../README $(BD)
	cp ../../kiwi/*.cs ../../kiwi/makefile ../../kiwi/README.txt $(BD)/kiwi

# Doc
	mkdir -p $(BD)/doc
	cp $(HOME)/Orangepath/manual/kiwic.pdf  $(BD)/doc

# Regressions
	mkdir -p $(BD)/kiwic/regression
	cp ../regression/*.cs ../regression/makefile ../regression/runtest ../regression/*.k2sim ../regression/*.k2rawsim ../regression/*.v ../regression/*.k2vnlsim ../regression/run.do $(BD)/kiwic/regression

# Finally:  Tar it up
	tar -czf /tmp/kiwic.tar.gz  $(BD)


# Tarball location koo.corpus.cam.ac.uk/kiwic.tar.gz
tarcopy:
	scp  /tmp/kiwic.tar.gz koo.corpus.cam.ac.uk:/home/httpd/html

# eof
