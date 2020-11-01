#
# Kiwi Scientific Acceleration - dfsin Chstone test. 
#
include ../../Makefile.examples.inc



ANAME=dfsin-floatnat

KIWIC  ?=$(KIWI)/bin/kiwic \
     -res2-loadstore-port-count=0 \
     -vnl-resets=synchronous   \
     -vnl-roundtrip=disable \
     -kiwic-cil-dump=combined \
     -kiwic-kcode-dump=enable \
     -kiwic-register-colours=disable \
     -bevelab-default-pause-mode=soft  \
     -bevelab-soft-pause-threshold=20 \
     -max-no-int-muls=12 \
     -vnl-rootmodname=dfsin \
     -repack-to-roms=enable


#     -firstpass-loglevel=0 -gtrace-loglevel=0 \
#  -restructure2=disable \
#  -vnl-ifshare=none 
#  -verboselevel=10 
#      -diosim-tl=0 

all:ll_run


$(ANAME).exe:$(ANAME).cs ../softfloat.dll 
	$(CSC) $(ANAME).cs /r:../softfloat.dll /r:$(KIWIDLL)

monorun:$(ANAME).exe
	MONO_PATH=$(HPRLS)/kiwipro/kiwic/distro/support:.. time mono  $(ANAME).exe | tee goldspool

../softfloat.dll:../softfloat.cs
	$(CSC) -target:library ../softfloat.cs	
#	ikdasm ../softfloat.dll > ../softfloat.dis

softfloat.dll:../softfloat.dll


hl_run:$(ANAME).exe
	$(KIWIC)  -sim 2000  -repack=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable $(ANAME).exe ../softfloat.dll


$(ANAME).v:$(ANAME).exe ../softfloat.dll
	$(KIWIC) $(ANAME).exe -vnl=$(ANAME).v ../softfloat.dll


#------------------------------------------------
# RTLSIM Verilator Run
ll_run2:verilated/VVERI_SIMSYS.cpp veriltop-dfsin.cpp
	g++ -g -O2 -Iverilated -I$(VERILATOR_ROOT)/include veriltop-dfsin.cpp verilated/VVERI_SIMSYS.cpp $(VERILATOR_ROOT)/include/verilated.cpp verilated/VVERI_SIMSYS__Syms.cpp
	time ./a.out
	tail -8 KiwiC.rpt

verilated/VVERI_SIMSYS.cpp:$(ANAME).v veril_sys.v
	verilator --Mdir verilated -Wno-fatal -cc --top-module VERI_SIMSYS $(ANAME).v veril_sys.v $(CVLIBS) 

#------------------------------------------------
# RTLSIM Icarus Run
ll_run:$(ANAME).v
	iverilog $(ANAME).v vsys.v $(CVLIBS) 
	./a.out| tee icarus.spool
	cp vcd.vcd ~/Dropbox



# RTLSIM
icarus:$(ANAME).v
	iverilog $(ANAME).v vsys.v $(CVLIBS)
	time ./a.out| tee icarus.spool



# make these links once please - should be automated.
links:
	ln -s ../softfloat.dll
	ln -s $(KIWIDLL)

#---
dot:
	dot -Tpng < controlflow_combined.dot > ~/Dropbox/a.png

# eof
