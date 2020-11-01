# Kiwi Scientific Acceleration: primes example.
# (C) 2010-16 DJ Greaves, University of Cambridge Computer Laboratory.
#
#

include ../Makefile.examples.inc



VINCS=-I$(KIWI)/support/performance_predictor 


# -bevelab-default-pause-mode=maximal
# 
# -restructure2=disable -repack=disable 




APNAME=primesya

KIWICFLAGS=-vnl-rootmodname=DUT \
	       	-kiwife-directorate-endmode=finish \
		-vnl-resets=synchronous \
		-bevelab-default-pause-mode=hard \
		-vnl-roundtrip=disable \
		-kiwic-cil-dump=combined \
		-kiwic-kcode-dump=enable \
		-kiwic-register-colours=1

# 		-res2-loglevel=2 \

# Insert this
#     $display("pc=%d  %d %d %d", xpc10nz, pTMT4Main_V_0, pTMT4Main_V_1, pTMT4Main_V_2);



all:  tool offchip64.v offchip.isim 
# primes-gl
# all:  killer onchip.v onchip.isim
# Other targets: offchip.v crun

clean:
	rm -f offchip64.v offchip.v onchip.v vcd.vcd

# C Version
crun:yaprimes 
	time ./yaprimes 0
	time ./yaprimes 0
	time ./yaprimes 1
	time ./yaprimes 1
	time ./yaprimes 2
	time ./yaprimes 2

yaprimes:yaprimes.c
	gcc -o yaprimes -O3 yaprimes.c



dot:
	dot -Tpng dotout.dot > ~/Dropbox/dotout.png

normal:killer tool offchip64.v offchip.v onchip.v
#  select onchip.v offchip.v or $(APNAME)-hl

killer:
	rm -f onchip.v offchip.v offchip64.v

$(APNAME).exe:$(APNAME).cs
	$(CSC) $(APNAME).cs   /r:$(KIWIDLL)  

# Run under mono
monorun: $(APNAME).exe
	MONO_PATH=$(KDISTRO)/support:. mono $(APNAME).exe > monogold


# Whether on chip or off chip can be controlled via C# with the Kiwi.OutboardArray mark up.
# To compile the same source file such that the main array is onchip or offchip we can use a command line flag.
# One to adjust is +res2-offchip-threshold
# Setting protocol=none is a crude way to make offchip become onchip - ignores the offchip attributes. This is in flux at the moment 1Q17.
# 
onchip.v:$(APNAME).exe
	$(KIWIC)  $(KIWICFLAGS) $(APNAME).exe -sim=21999  -vnl=onchip.v -res2-offchip-threshold=10000000



offchip.v:$(APNAME).exe 
	$(KIWIC) $(APNAME).exe -vnl=offchip.v $(KIWICFLAGS)  -diosim-vcd=vcd.vcd -res2-offchip-threshold=32

# These are the old flags - prior to using bondout schema strings.
FLAGS_OFFCHIP64= \
	-bondout-loadstore-port-count=1 \
	-bondout-loadstore-port-lanes=8 \
	-bondout-loadstore-lane-width=8 \
	-bondout-loadstore-lane-addr-size=22


# Here we want an HFAST port that is 64 bits wide
offchip64.v:$(APNAME).exe 
	$(KIWIC) $(APNAME).exe -vnl=offchip64.v $(KIWICFLAGS)  $(FLAGS_OFFCHIP64) -res2-offchip-threshold=32 -kiwife-directorate-style=advanced

#	cp vcd.vcd ~/Dropbox
#
# -res2-offchip-threshold=90
#  -sim=10000
# Do not use -sim if there are offchip resources required - diosim fails - so use external RTL simulator profile generate


# High-level simulation
$(APNAME)-hl:$(APNAME).exe
	$(KIWIC) $(APNAME).exe -sim=1999 -conerefine=disable -bevelab=disable -protocol=none


# Gate-level build:
primes-gl:$(APNAME).exe
	$(KIWIC) $(APNAME).exe  -vnl=$(APNAME).vnl  -gatelib=CAM 


tool:
	cd ../../../kiwic/src ; make tool


#----------------------
# Simulate with modelsim or iverilog
.PHONY: modelsim0 modelsim1 i_onchip 

onchip.isim: vsys.v onchip.v
	iverilog $(VINCS) $(CVLIBS) onchip.v vsys.v 
	./a.out
#	cp vcd.vcd ~/Dropbox

offchip.isim:  offchip64.v primes-offchip-testbench.v 
	iverilog $(VINCS) $(CVLIBS) offchip64.v primes-offchip-testbench.v  ../../rams/ddr2-models.v 
	./a.out	
	echo "Done Icarus"
	sleep 1
#	ulimit -t200000;		./a.out
	cp vcd.vcd ~/Dropbox

m_onchip: onchip.v vsys.v
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib onchip.v vsys.v
	vsim -c simple_lib.SIMSYS -do "run.do" 

# This is suitable for the offchip version of primseya.cs  and other designs with one offchip RAM port.
m_offchip: DUT.v hsimple.v
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work offchip.v primes-offchip-testbench.v 
	vsim -c simple_lib.DUTH1 -do "run.do" 



# eof
