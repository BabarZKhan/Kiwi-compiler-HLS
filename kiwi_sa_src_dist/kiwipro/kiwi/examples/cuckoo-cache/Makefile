#//
#// Kiwi Scientific Acceleration Example - Cuckoo Hash Demo
#// (C) 2016 DJ Greaves, University of Cambridge, Computer Laboratory.
#//

ANAME=cuckoo_hash_demo


include ../Makefile.examples.inc

KIWIFLAGS= -progress-monitor -vnl-resets=synchronous \
           -kiwic-cil-dump=combined \
	   -obj-dir-name=. -log-dir-name=$(LOGDIR)/$(ANAME) \
	   -bondout-loadstore-port-count=0 \
           -vnl-roundtrip=disable \
           -bevelab-default-pause-mode=bblock \
           -bevelab-soft-pause-threshold=10 \
           -vnl-rootmodname=DUT \

# Replace  -res2-loadstore-port-count=0  with -bondout-loadstore-port-count=0

#	-kiwife-gtrace-loglevel=0	   -verboselevel=10 	-kiwife-ataken-loglevel=0 -kiwic-kcode-dump=early 

all:  killer verilsim

killer:
#	rm -f $(ANAME).v

$(ANAME).exe:$(ANAME).cs
	$(CSC) $(ANAME).cs  /r:$(KIWIDLL)  

$(ANAME).v:$(ANAME).exe
	mkdir -p $(LOGDIR)
	$(KIWIC)  -vnl=$(ANAME).v $(ANAME).exe $(KIWIFLAGS)
#	echo "KiwiC returned to Make"

#------------------------------------------------
monorun:$(ANAME).exe
	MONO_PATH=$(KDISTRO)/support mono $(ANAME).exe

#------------------------------------------------
# RTLSIM Verilator
verilsim:verilated/VVERI_SIMSYS.cpp
	g++ -g -O2 -Iverilated -I$(VERILATOR_ROOT)/include veriltop.cpp verilated/VVERI_SIMSYS.cpp $(VERILATOR_ROOT)/include/verilated.cpp verilated/VVERI_SIMSYS__Syms.cpp
	time ./a.out
	tail -8 KiwiC.rpt

verilated/VVERI_SIMSYS.cpp:$(ANAME).v veril_sys.v
	verilator --Mdir verilated -Wno-fatal -cc --top-module VERI_SIMSYS $(ANAME).v veril_sys.v $(CV_INT_ARITH_LIB)


#------------------------------------------------
# RTLSIM Icarus Simulation is too slow - use Verilator instead
isim:$(ANAME).v
	iverilog $(ANAME).v vsys.v $(CV_INT_ARITH_LIB)
	time ./a.out | tee isim.out
	head -100000 top.vcd > ~/Dropbox/vcd.vcd
	tail -8 KiwiC.rpt
#------------------------------------------------



tool: 
	cd $(HPRLS)/kiwipro/kiwic/src; make tool

# eof
