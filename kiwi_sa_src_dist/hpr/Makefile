#
#  $Id: Makefile,v 1.101 2013-07-21 21:33:33 djg11 Exp $
#
# CBG Orangepath.  Makefile for hpr library.
# HPR L/S Logic Synthesis/Formal/Compiler/Codesign System - FSharp Version.
#
# (C) 2003-17 DJ Greaves. All rights reserved.
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
#

HPRLS?=$(shell dirname $(shell dirname $(realpath $(lastword $(MAKEFILE_LIST)))))

include Makefile.inc


# -g - MDB files not created when compiling on mono.



HPRLIBREF=  -r moscow.dll -r yout.dll   \
     -r cpp_render.dll -r meox.dll -r abstract_hdr.dll -r abstracte.dll -r protocols.dll -r hprls_hdr.dll \
     -r Djg.dll -r linepoint_hdr.dll -r verilog_hdr.dll -r verilog_render.dll fitters.dll \
	-r hprxml.dll -r verilog_gen.dll -r gbuild.dll -r dotreport.dll -r plot.dll -r annealer.dll \
	-r reporter.dll -r cvipgen.dll -r revast.dll -r ksc.fs
#     -r conerefine.dll



# This is the default target for making the library, but normally one
# will call this makefile from the application's (e.g. jinjoin, h2, kiwic) makefile.
all:$(HPRDLL) opath.dll kpredict.exe
	echo "Library built ok. Now do a make in a customer directory (e.g. bsvc, jinjoin, h2, kiwic...)"

microrev:
#	$(FSC) test.fs -r microcode.dll -r m6805_hdr.dll
#	cp ../kiwic/distro/lib/FSharp.Core.dll  .
#	mono test.exe  

#
# cilnorm.dl unit_tests 
spare_all: yout.dll cpp_render.dll   verilog_gen.dll verilog_render.dll hprxml.dll  repack.dll conerefine.dll opath.exe  systolic.dll
	mono ./opath.exe -recipe kiwic/recipes/base0 -root frog $(HPRLS)/kiwic/regression/test1.exe 


vunit_tests:
	iverilog cvgates_units.v cvgates.v cv_fparith.v
	./a.out
	cp vcd.vcd ~/Dropbox
	sleep 1

unit_tests:tester.fs
	rm -f tester.exe
	$(FSC) tester.fs $(HPRLIBREF)
#	$(FSC) opath.fs $(HPRLIBREF) -r conerefine.dll -r microcode.dll
	mono tester.exe fishcan



# excludes opath_hdr.dll - say why please.
HPRDLL=linepoint_hdr.dll linprog.dll microcode_hdr.dll abstract_hdr.dll meox.dll  \
	abstracte.dll protocols.dll cpp_render.dll dpmap.dll hprxml.dll verilog_hdr.dll verilog_gen.dll \
	microcode.dll m6805_hdr.dll annealer.dll reporter.dll \
	verilog_render.dll  moscow.dll yout.dll opath_hdr.dll opath_interface.dll bevctrl.dll \
	planoptimiser.dll planhdr.dll fitters.dll cvipgen.dll revast.dll \
	hprls_hdr.dll Djg.dll  vsdg_s.dll compose.dll stimer.dll bevelab.dll repack.dll conerefine.dll systolic.dll smv_hdr.dll \
        gbuild.dll  restructure.dll   diosim.dll  dotreport.dll tableprinter.dll espresso.dll plot.dll  ksc.dll


 

distro: $(HPRDLL) opath.dll
	mkdir -p $(DISTDIR)/bin $(DISTDIR)/lib 	
#	cp $(HPRDLL) *.dll.so opath.dll $(DISTDIR)/lib
	cp $(HPRDLL)  opath.dll $(DISTDIR)/lib

opath.dll: $(HPRDLL) opath.fs opath_hdr.dll smv_hdr.dll reporter.dll protocols.dll cvipgen.dll
	$(FSC) opath.fs /target:library -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r microcode_hdr.dll -r abstract_hdr.dll -r meox.dll -r yout.dll -r abstracte.dll  -r cpp_render.dll -r dpmap.dll -r hprxml.dll -r verilog_gen.dll -r bevctrl.dll -r gbuild.dll -r verilog_render.dll -r opath_hdr.dll  -r opath_interface.dll -r smv_hdr.dll -r reporter.dll -r protocols.dll -r cvipgen.dll
	mkdir -p $(DISTDIR)/bin $(DISTDIR)/lib 


yout.dll:yout.fs
	$(FSC) --target:library yout.fs

linepoint_hdr.dll:linepoint_hdr.fs
	$(FSC) --target:library linepoint_hdr.fs

moscow.dll:moscow.fs linepoint_hdr.dll yout.dll
	$(FSC) --target:library moscow.fs -r linepoint_hdr.dll -r yout.dll

linprog.dll:linprog.fs
	$(FSC) --target:library linprog.fs 

hprls_hdr.dll:linepoint_hdr.dll linprog.dll moscow.dll hprls_hdr.fs
	$(FSC) --target:library hprls_hdr.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll

Djg.dll:Djg.fs
	$(FSC) --target:library Djg.fs

espresso.dll:linepoint_hdr.dll linprog.dll moscow.dll hprls_hdr.dll yout.dll Djg.dll espresso.fs 
	$(FSC) --target:library espresso.fs -r hprls_hdr.dll  \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll

meox.dll:linepoint_hdr.dll linprog.dll moscow.dll hprls_hdr.dll  yout.dll Djg.dll espresso.dll meox.fs
	$(FSC) --target:library meox.fs -r hprls_hdr.dll /r:espresso.dll \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll

cpp_render.dll:linepoint_hdr.dll linprog.dll moscow.dll microcode_hdr.dll abstract_hdr.dll meox.dll yout.dll abstracte.dll cpp_render.fs opath_hdr.dll bevctrl.dll protocols.dll gbuild.dll cvipgen.dll
	$(FSC) --target:library cpp_render.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r microcode_hdr.dll -r abstract_hdr.dll -r meox.dll -r yout.dll -r abstracte.dll -r opath_hdr.dll  -r opath_interface.dll /r:bevctrl.dll  /r:protocols.dll /r:gbuild.dll /r:cvipgen.dll

abstracte.dll:abstracte.fs meox.dll opath_hdr.dll hprxml.dll linepoint_hdr.dll protocols.dll fitters.dll
	$(FSC) --target:library abstracte.fs -r abstract_hdr.dll -r hprls_hdr.dll -r protocols.dll \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll -r meox.dll -r opath_hdr.dll -r hprxml.dll -r linepoint_hdr.dll  -r opath_interface.dll -r fitters.dll

ksc.dll:ksc.fs abstracte.dll meox.dll opath_hdr.dll hprxml.dll linepoint_hdr.dll protocols.dll fitters.dll
	$(FSC) --target:library ksc.fs -r abstracte.dll -r abstract_hdr.dll -r hprls_hdr.dll -r protocols.dll \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll -r meox.dll -r opath_hdr.dll -r hprxml.dll -r linepoint_hdr.dll  -r opath_interface.dll -r fitters.dll

protocols.dll:protocols.fs meox.dll opath_hdr.dll hprxml.dll linepoint_hdr.dll abstract_hdr.dll tableprinter.dll
	$(FSC) --target:library protocols.fs -r abstract_hdr.dll  -r hprls_hdr.dll \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll -r meox.dll -r opath_hdr.dll -r hprxml.dll -r linepoint_hdr.dll  -r opath_interface.dll -r tableprinter.dll

revast.dll:revast.fs meox.dll opath_hdr.dll hprxml.dll linepoint_hdr.dll abstract_hdr.dll abstracte.dll dotreport.dll reporter.dll protocols.dll
	$(FSC) --target:library revast.fs -r abstract_hdr.dll  -r hprls_hdr.dll \
	-r:linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll -r meox.dll -r opath_hdr.dll -r hprxml.dll -r linepoint_hdr.dll  -r opath_interface.dll -r:abstracte.dll -r:dotreport.dll -r:reporter.dll -r:protocols.dll

cvipgen.dll:cvipgen.fs meox.dll opath_hdr.dll hprxml.dll linepoint_hdr.dll abstract_hdr.dll protocols.dll abstracte.dll
	$(FSC) --target:library cvipgen.fs -r abstract_hdr.dll  -r hprls_hdr.dll \
	-r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r yout.dll -r Djg.dll -r meox.dll -r opath_hdr.dll -r hprxml.dll -r linepoint_hdr.dll  -r opath_interface.dll -r protocols.dll -r abstracte.dll


diosim.dll:diosim.fs abstracte.dll meox.dll moscow.dll yout.dll microcode_hdr.dll opath_hdr.dll linprog.dll plot.dll cvipgen.dll protocols.dll
	$(FSC) --target:library diosim.fs  -r opath_interface.dll -r hprls_hdr.dll -r abstract_hdr.dll -r abstracte.dll  -r meox.dll -r yout.dll -r moscow.dll -r microcode_hdr.dll -r opath_hdr.dll -r linprog.dll -r plot.dll -r cvipgen.dll -r protocols.dll

annealer.dll:annealer.fs moscow.dll yout.dll 
	$(FSC) --target:library annealer.fs  -r yout.dll -r moscow.dll 

restructure.dll:restructure.fs  opath_interface.dll hprls_hdr.dll abstract_hdr.dll abstracte.dll  meox.dll yout.dll moscow.dll microcode_hdr.dll tableprinter.dll  opath_hdr.dll dotreport.dll protocols.dll planhdr.dll planoptimiser.dll  cvipgen.dll
	$(FSC) --target:library restructure.fs  -r opath_interface.dll -r hprls_hdr.dll -r abstract_hdr.dll -r abstracte.dll -r meox.dll -r yout.dll -r moscow.dll -r microcode_hdr.dll -r tableprinter.dll -r opath_hdr.dll -r dotreport.dll -r protocols.dll -r planhdr.dll -r planoptimiser.dll -r cvipgen.dll

# socrender.dll:socrender.fs  opath_interface.dll hprls_hdr.dll abstract_hdr.dll abstracte.dll  meox.dll yout.dll moscow.dll microcode_hdr.dll tableprinter.dll  opath_hdr.dll dotreport.dll protocols.dll
#	$(FSC) --target:library socrender.fs  -r opath_interface.dll -r hprls_hdr.dll -r abstract_hdr.dll -r abstracte.dll -r meox.dll -r yout.dll -r moscow.dll -r microcode_hdr.dll -r tableprinter.dll -r opath_hdr.dll -r dotreport.dll -r protocols.dll




hprxml.dll:hprxml.fs moscow.dll yout.dll
	$(FSC) --target:library hprxml.fs -r yout.dll -r moscow.dll


plot.dll:moscow.dll yout.dll plot.fs
	$(FSC) --target:library plot.fs -r moscow.dll -r yout.dll




opath_hdr.dll:linepoint_hdr.dll linprog.dll moscow.dll yout.dll opath_hdr.fs abstract_hdr.dll opath_interface.dll 
	$(FSC) --target:library opath_hdr.fs -r abstract_hdr.dll \
	-r moscow.dll -r yout.dll

abstract_hdr.dll:linepoint_hdr.dll linprog.dll moscow.dll microcode_hdr.dll abstract_hdr.fs
	$(FSC) --target:library abstract_hdr.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r microcode_hdr.dll

planhdr.dll:planhdr.fs moscow.dll yout.dll planhdr.fs
	$(FSC) --target:library planhdr.fs -r yout.dll -r moscow.dll


verilog_hdr.dll:linepoint_hdr.dll linprog.dll moscow.dll verilog_hdr.fs hprls_hdr.dll meox.dll abstract_hdr.dll abstracte.dll
	$(FSC) --target:library verilog_hdr.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r meox.dll -r abstract_hdr.dll

verilog_render.dll:linepoint_hdr.dll linprog.dll moscow.dll verilog_render.fs hprls_hdr.dll verilog_hdr.dll meox.dll yout.dll opath_hdr.dll abstracte.dll
	$(FSC) --target:library verilog_render.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r opath_hdr.dll -r abstracte.dll

opath_interface.dll:opath_interface.fs
	$(FSC) --target:library opath_interface.fs


bevelab.dll:linepoint_hdr.dll linprog.dll moscow.dll bevelab.fs hprls_hdr.dll meox.dll yout.dll abstracte.dll dpmap.dll opath_hdr.dll reporter.dll protocols.dll revast.dll reporter.dll protocols.dll
	$(FSC) --target:library bevelab.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r dpmap.dll -r reporter.dll -r protocols.dll -r revast.dll -r:reporter.dll -r:protocols.dll


stimer.dll:linepoint_hdr.dll linprog.dll moscow.dll stimer.fs hprls_hdr.dll meox.dll yout.dll abstracte.dll dpmap.dll opath_hdr.dll
	$(FSC) --target:library stimer.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r dpmap.dll

compose.dll:linepoint_hdr.dll linprog.dll moscow.dll compose.fs hprls_hdr.dll meox.dll yout.dll abstracte.dll dpmap.dll opath_hdr.dll bevelab.dll
	$(FSC) --target:library compose.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r dpmap.dll -r bevelab.dll

vsdg_s.dll:linepoint_hdr.dll linprog.dll moscow.dll vsdg_s.fs hprls_hdr.dll meox.dll yout.dll abstracte.dll dpmap.dll opath_hdr.dll bevelab.dll
	$(FSC) --target:library vsdg_s.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r dpmap.dll -r bevelab.dll

smv_hdr.dll:linepoint_hdr.dll linprog.dll moscow.dll smv_hdr.fs hprls_hdr.dll meox.dll yout.dll abstracte.dll opath_hdr.dll
	$(FSC) --target:library smv_hdr.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll 

verilog_gen.dll:linepoint_hdr.dll linprog.dll moscow.dll verilog_gen.fs hprls_hdr.dll verilog_hdr.dll meox.dll yout.dll dpmap.dll gbuild.dll verilog_render.dll opath_hdr.dll bevctrl.dll abstracte.dll protocols.dll
	$(FSC) --target:library verilog_gen.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll -r verilog_render.dll -r dpmap.dll -r gbuild.dll -r opath_hdr.dll  -r bevctrl.dll -r opath_interface.dll -r protocols.dll

bevctrl.dll:linepoint_hdr.dll linprog.dll moscow.dll hprls_hdr.dll meox.dll yout.dll dpmap.dll gbuild.dll  opath_hdr.dll bevctrl.fs
	$(FSC) --target:library bevctrl.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll -r dpmap.dll -r opath_hdr.dll  -r opath_interface.dll

dotreport.dll:dotreport.fs moscow.dll yout.dll
	$(FSC) --target:library dotreport.fs -r yout.dll -r moscow.dll

planoptimiser.dll:planoptimiser.fs moscow.dll yout.dll planhdr.dll planoptimiser.fs planhdr.dll
	$(FSC) --target:library planoptimiser.fs -r yout.dll -r moscow.dll -r planhdr.dll


gbuild.dll:linepoint_hdr.dll linprog.dll moscow.dll gbuild.fs hprls_hdr.dll verilog_hdr.dll meox.dll yout.dll dpmap.dll plot.dll verilog_render.dll protocols.dll
	$(FSC) --target:library gbuild.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll -r verilog_render.dll -r dpmap.dll -r opath_interface.dll -r plot.dll -r verilog_render.dll -r protocols.dll

dpmap.dll:linepoint_hdr.dll linprog.dll moscow.dll  hprls_hdr.dll  meox.dll yout.dll opath_hdr.dll dpmap.fs
	$(FSC) --target:library dpmap.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll  -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll -r opath_hdr.dll

conerefine.dll:linepoint_hdr.dll linprog.dll moscow.dll conerefine.fs hprls_hdr.dll meox.dll yout.dll opath_hdr.dll abstracte.dll protocols.dll
	$(FSC) --target:library conerefine.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r protocols.dll

# We perhaps have just a temporary dependence on systolic before we use a new recipe
repack.dll:linepoint_hdr.dll linprog.dll moscow.dll repack.fs hprls_hdr.dll meox.dll yout.dll opath_hdr.dll tableprinter.dll protocols.dll  systolic.dll ksc.dll
	$(FSC) --target:library repack.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll -r tableprinter.dll -r protocols.dll -r systolic.dll -r ksc.dll

refrepack.dll:linepoint_hdr.dll linprog.dll moscow.dll refrepack.fs hprls_hdr.dll meox.dll yout.dll opath_hdr.dll
	$(FSC) --target:library refrepack.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll

systolic.dll:linepoint_hdr.dll linprog.dll moscow.dll systolic.fs hprls_hdr.dll meox.dll yout.dll opath_hdr.dll
	$(FSC) --target:library systolic.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r verilog_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll  -r opath_interface.dll -r opath_hdr.dll

reporter.dll:moscow.dll hprls_hdr.dll meox.dll yout.dll opath_hdr.dll abstract_hdr.dll abstracte.dll dotreport.dll reporter.fs
	$(FSC) --target:library reporter.fs -r linepoint_hdr.dll -r linprog.dll -r moscow.dll -r hprls_hdr.dll -r meox.dll -r yout.dll -r abstract_hdr.dll -r abstracte.dll  -r dotreport.dll

tableprinter.dll:tableprinter.fs moscow.dll yout.dll
	$(FSC) --target:library tableprinter.fs -r moscow.dll -r yout.dll


microcode_hdr.dll:microcode_hdr.fs hprls_hdr.dll
	$(FSC) --target:library microcode_hdr.fs -r hprls_hdr.dll

m6805_hdr.dll:m6805_hdr.fs hprls_hdr.dll microcode_hdr.dll
	$(FSC) --target:library m6805_hdr.fs -r hprls_hdr.dll -r microcode_hdr.dll

microcode.dll:microcode.fs hprls_hdr.dll yout.dll moscow.dll meox.dll abstracte.dll abstract_hdr.dll verilog_gen.dll linepoint_hdr.dll m6805_hdr.dll protocols.dll
	$(FSC) --target:library microcode.fs -r hprls_hdr.dll  -r moscow.dll  -r yout.dll -r meox.dll -r abstracte.dll -r abstract_hdr.dll -r verilog_gen.dll -r linepoint_hdr.dll -r m6805_hdr.dll  -r protocols.dll


fitters.dll:fitters.fs moscow.dll yout.dll hprxml.dll 
	$(FSC)  --target:library -r moscow.dll -r yout.dll -r hprxml.dll fitters.fs


kpredict.exe:kpredict.fs moscow.dll yout.dll hprxml.dll opath.dll fitters.dll
	$(FSC) kpredict.fs -r moscow.dll -r yout.dll -r hprxml.dll -r opath.dll -r fitters.dll

done:
	$(FSC) --target:library linepoint_hdr.fs
	$(FSC) --target:library linprog.fs

# ----------------------------------------


#PW=/ssd/paula1/djg11p/paula-d320/new-co/bitbucket-hprls2/kiwipro/kiwi/examples/primesya

PW=$(HPRLS)/kiwipro/kiwi/examples/primesya

FSHARP2?=/usr/local/FSharp-2.0.0.0
predict:kpredict.exe
	mkdir -p pred
	mv kpredict.exe pred
	cp *.dll pred
#	cp $(FSHARP2)/bin/FSharp.Core.sigdata  pred
#	cp $(FSHARP2)/bin/FSharp.Core.dll  pred
#	cd pred; 	mono kpredict.exe $(PW)/primes200.xml $(PW)/primes400.xml $(PW)/primes600.xml $(PW)/primes10000.xml 
# ~/tony2xpc10.xml

	cd pred; 	mono kpredict.exe $(PW)/primes-f200.xml $(PW)/primes-f600.xml $(PW)/primes-f3000.xml  \
	$(PW)/primes-f10000.xml \
	$(PW)/primes-f50000.xml \
	$(PW)/primes-f95000.xml 
# ~/tony2xpc10.xml
	cd pred; cp toViewer.xml ~/Dropbox/kpredict-displayer




# ----------------------------------------
clean:
	rm -rf *.exe *.dll


zip:
	zip hprsrc.zip *v *.fs Makefile README.txt
	zip -r hprsrc.zip hpr_ipblocks

# ----------------------------------------
#
# Notes
#
# (autoload 'ansi-color-for-comint-mode-on "ansi-color" nil t)
# (add-hook 'shell-mode-hook 'ansi-color-for-comint-mode-on)

# eof


