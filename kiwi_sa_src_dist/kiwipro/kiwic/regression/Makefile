#
# Kiwi Scientific Acceleration: KiwiC Regression Tests
#
# (C) 2018 DJ Greaves, University of Cambridge Computer Laboratory.
#  

MODEARGS=  \
	-vnl-roundtrip=disable \
	-vnl-resets=synchronous \
	-kiwife-dynpoly=disable \
	-kiwife-directorate-endmode=finish \
	-ip-incdir=$(HPRLS)/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 \
	-res2-share-array-reads=enable \
        -res2-regen-sequencer=enable \
	-res2-extend-schedules-to-keep-pli-order=full \
	-bevelab-revast-enable=disable \
	-compose=disable \

#	-res2-killpipelining=normal  \
# Spare command line flags

#
#-bevelab-cfg-dotreport=enable \

# -res2-loglevel=0 \

# 	-conerefine-loglevel=0
#	-kiwife-overloads-loglevel=0 
#	-kiwic-register-colours=disable 
# -vnl-loglevel=10
# -csharp-gen=disable 
# -vnl-loglevel=100
# -log-dir-name /tmp/kiwiobj
# -vnl-layout-delay-estimate=enable
# 

#  -kiwic-kcode-dump=enable 
# -dotplot-plot=combined
# -kiwic-kcode-dump=enable   
# -kiwic-cil-dump=separately - use this to get a disassembly of the input
# -verbose  
# -conerefine-loglevel=0
# -bevelab-loglevel=0
# -diosim-vcd=a.vcd
# -vnl-layout-delay-estimate=enable
# -vnl-keep-pli=disable
# -diosim-granularity=2
# -repack=disable
# -meox-enable-sorter1
#  -restructure2=disable
#  -techno=disable 
#  -csharp=dut.cs
#  -cpp=disable
#  -vnl-kandr=enable 
#  -bevelab-bisim-reduction=disable
#  -postgen-optimise=disable
#  -bevelab-redirect=disable

 


# export MONO_LOG_LEVEL=debug
# --gc=sgen

# The CSharp compiler, mono and the FSharp compiler is defined in the include file
include $(HPRLS)/hpr/Makefile.inc

# Where to copy output files for remote viewing
VIEWINGDEST ?=~/Dropbox


# --profile=logging 
# /opt/novell/mono/bin/mono 
RECIPE=
#-recipe kiwic-rebal 
# RECIPE=-recipe kiwic-rebal 

# -verboselevel=20 - makes a lot of console output and fflush calls.
# In this regression folder we sometimes use resources from HPRLS, but end user apps should always use the KDISTRO
KDISTRO    ?=$(HPRLS)/kiwipro/kiwic/kdistro

export KIWIC= $(MONO)  $(KDISTRO)/lib/kiwic.exe $(RECIPE)  $(MODEARGS)
#  -progress-monitor
#export KIWIC= $(MONO)  $(KDISTRO)/lib/kiwic.exe $(RECIPE)  $(MODEARGS) 

#Being done: these should be in KDISTRO and autoloaded too... 
KICKOFF_LIBRARIES=$(HPRLS)/kiwipro/kiwi/userlib/kickoff-libraries
KSTRINGIOLIB      ?=$(KDISTRO)/support/KiwiStringIO.dll

# 

CV_INT_ARITH_LIB  ?=$(KDISTRO)/lib/hpr_ipblocks/cvip0/cvgates.v
CV_FP_ARITH_LIB   ?=$(KDISTRO)/lib/hpr_ipblocks/cvip0/cv_fparith.v
CVLIBS             =$(CV_INT_ARITH_LIB) $(CV_FP_ARITH_LIB)
KLIB              ?=$(KDISTRO)/support/Kiwi.dll


real_all: tool current_problem regression
# for pipelining test4b3
# test17r2.isim <-- fails



current_problem:
# test56b test67d 
# 
# test58r1
# test71r2b 
# test58r1 fails on floating-point library change that was not completed properly.
# test24 is giving name alias error or thrasing in new ksc 
# test7 test62.goldspool test62 

 
# Recently Failing test list :

# Sept 2017: test58 on double-precision divider corruption.
# test7 - calling wrong method under dynamic dispatch
# test24 is giving name alias error
# test40  - unsafe code experiments

# Which is a test that makes remote virtual class on an object of which there is more than one instance?  And is dynamic dispatch supported?

#----------------------------------------------------------

# Main regression collection:
# Test 19 currently removed (for one day or so!)
regression: start_timestamp \
  test0 test1  test2 test3 test4 test4p test5 test6 test8 test9  \
  test10 test11 test12 test14  test15 test16 test17 test18  test19 \
  test20 test21 test23 test25 test26 test27 test28 test29 \
  test30 test31 test32 test34 test35 test38  \
  test41   test49 \
  test50 test51 test52 test53 test56  test57 test58 \
  test60 test61 test65 test66 test67 test68 test69 \
  test70 test71 test72 test74 \
  silentrunners \
  loopclassic_test \
  completion_message

# Currently Broken:
# THE FOLLOWING TESTS ARE KNOWN TO BE IN REGRESSION:
# test39
# need a test with top-level in and out args
# # test14z is new and give North test14z line3 : pongar[5]=X errors
# test24 - ksc ? big run on ksc
# test22 - never worked/finished -fails on >>0000:100:::create_array1:CT_arr(CTL_net(false, 32, Signed, native), 0): +++ Zero length array formed containing 0 locations of @/SINT.
# test36b stuck in bevelab
# test37
# test46 not tested yet
# test48 not tested recently - bounded recursion test - this has stopped working post gtrace implementation ?
# test45 - needs final fixes

examples:
	cd ../examples/fifo2; make fifo2bev


#                   test0: bit-level operations
# ok                test1: timestable with pauses:working again alpha 32
# new  not run  test101 simple threading and volatile test.
 
# ok                test2 channels and shared variables, pointers twiddle.
# ok            test3 enums not finished, so some commented out (more of the reflection API is needed).
# ok            test4 array/counters: working - updated golden for intermediate prints and one extra value on end : nb finish in k2sim deletes end of spool
# ok            test4p pauseless ditto
# 5b rtl unwind test5 timestable, with static subcall: working in 3 modes andalso csim
# ok            test6 a, b, c ok.
# ok - partially broken again            test7: overloading and objects: dynamic dispatch has failed so gldsim hacked for the moment to make it appear to pass. - WIP
# ok            test8 call by reference.
# no yes/no wrapper test9 parallel port: needs wrapper to generate an ack under diosim, but ok with csim? : need to check intest9.k2sim  - wrap error until attribute description fixed.
# ok              test10 return statements and multiplier structures 
# ok              test11 returns as well: hmmm vnlsim should print answer three times!
# ok              test12 dynamic objects (Kiwi-1 style - becoming statically allocated during KiwiC compilation)
# ok              test14 pointer twiddles 32/22/32 etc.. (was northtest) ok under gtrace
# ok                test14x pointer twiddles 32/22/32 etc. with arrays in the records.
# ok                test15: class constructors .cctor and so on.

# ok (no o/p)       test16: simple adder - should perhaps be a combinational logic demo
# ok                test17: higher-order programming, simple delegates for concurrency or perhaps local application - lambdas ? see test55
# ok                test18: inheritance overrides

# ok                test19: rpc - old style - HSIMPLE - to be replace with HFAST/AXI-S - same test should work again
# ok                test20: properties
# ok                test21: 2/3d arrays 
#                   test22: varargs - never finished
# ok                test23: d/p ram
# name alias repack test24
# ok                test25: non-strict operators and implicit state machine
# ok                test26: pass by reference
# ok                test27: run-time twiddle of object pointers including null, but without garbage collection.
# never tested      test27x: run-time null pointer abend
# ok manual 2d      test28: 2-d arrays with a little jagged.
# ok                        test29: array ptr manips 
# ok                test30: C# structs
# ok                test31: switch
# ok                test32: vari-latency divide 
# newly started     test33: complex tyvars
# ok                test34: very simple floating point
# ok                test35: C# structures and classes passed to methods as valuetypes
# ok                test36: structure mappings of fields to RAMs example (DUT.v works on isim but not on diosim - works now?).
# newly started     test37: polymorphic with arrays
# ok                test38r0: ultra simple array of objects
# ok                test38r1: simple field arrays
# ok                test38r2: field arrays
# r0 ok - r1/2 todo test39: linked lists - Including Kiwi-2 style with runtime dynamic allocation
#                      test39r1a works under diosim but repack needs to stop scalarising the dynamic store pointer for tst39r1b to work
# wrong answer      test40: unsafe code
# ok                test41: complex BRAM access patterns
# newly started     test42: substrate tests
# checked overflow  test43:
# ok, needs compose=disable  test44: HardPause RAM R/W
# BitConverter               test45: BitConverter  Some of it commented out since not all of it has passed tests yet.
# fails on 'First' sign      test46: IndexOf aka Salvator's numeric bugs
#                            test47: not in the repo?
#                   test48: bounded recursion
# ok                         test49: simple floating point arithmetic 
# ok                         test50: dual-port RAM shared between threads
# ok on b, a broken          test51: wide I/O busses (greater than 64 bits)
# ok                test52: integer to ASCII library
# ok                test53: mutable formals and register reuse at different stack depths
#  never tested     test54: Pipelined Accelerator Mode
# ok                test55: C# closures  - needs autodispose and string outputs print as integers under iverilog but rtl-ok-out kludged to pass.
#  ok                        test56: cordiclike  
#  ok                        test57: more-complex string handling (for Kiwi-lcd etc).
# ok                                    test58r0: sqrt - using fast bit convert <- recently broken
# ?                                     test58r1: sqrt - autoload from kickoff library - see test67 and rationalsise please
# list len abend                        test59: Setter/Getter struct with layout
# ok                                    test60: Passing an array ... trivial test
# ok                                    test61: Bit-Tally - tests the compile time of an unprintable function.
#  isinst fail                          test62: // Example from MSDN web page about C# 'as' and 'is' operators
# ok                                    test65: Constant scalarised ROMs
# ok                                    test66: RTL Parameters
# ok                                    test67: kickoff libraries sqrt/sine/cos/log etc.
# ok                                    test68: swapper method generics
# ok                                    test69: parent constructor calls
#                                       test70: very simple exception raising - but not handling.
# r1 ok, r2 missing                     test71: try/finally and catch testing
# unfinished ...                        test72: Virtex native FIFOs
#                                       test73: native atomic operations, such as Interlocked.Add 
# ok                                    test74: clock domains and so on
# ..                sw_test: Smith Waterman sequencer - singled threaded silly version


# Others: Designs that give output you must check manually:
# Silentrunners: those that should just compile but no test bench.  test16 included really.


#- ------------------------------------
.PHONY: tool
tool:
	cd $(HPRLS)/kiwipro/kiwic/src; make mk_kdistro

#-------------------------------------
.PHONY: test0

test0:test0b

test0.exe:test0.cs
	$(CSC) test0.cs  -r:$(KLIB)

test0aa:test0.exe
	./runtest "test0" " " " -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800  -diosim-loglevel=10 "  "gldsim"

test0ab:test0.exe
	./runtest "test0" " " " -restructure=disable -sim=1800  -diosim-loglevel=10 "  "gldsim"

test0b:test0.exe
# This test is now run without stage disables.
#	./runtest "test0" " " " -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800 "  "gldsim"
	./runtest "test0" " " " -vnl DUT.v -vnl-resets=synchronous -sim=1800  "  "gldsim"
	iverilog vsys.v DUT.v $(CVLIBS) 
	./a.out > test0.rtl-spool
	diff test0.rtl-spool expecteds/test0b.ok.out

# Note once had to compare the RTL against the following file for a little while but the signnedness problem has gone away.??
#	diff test0a.rtl-spool expecteds/test0a.ok.out
# This legacy can be deleted now


test0.goldspool:test0.exe
	MONO_PATH=$(KDISTRO)/support mono test0.exe > test0.goldspool

test0.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out


# test0 fails to simulate with the version 10.0 icarus verilog - loops on st3 forever.  Works with modelsim however.

test0.modelsim:
	rm -rf simple_lib
	vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib DUT.v
	vlog -work simple_lib $(HPRLS)/hpr/hpr_ipblocks/cvip0/cv_fparith.v
	vlog -work simple_lib $(HPRLS)/hpr/hpr_ipblocks/cvip0/cvgates.v 
	vlog -work simple_lib $(HPRLS)/kiwipro/kiwic/regression/vsys.v
	echo "run -all;exit" > run.do
	vsim -c simple_lib.SIMSYS -do "run.do"

#-------------------------------------
.PHONY: test1 test1a test1b test1c
ROOT1 = "-root test1;test1.Main"
test1:test1a test1b 
#test1a test1b  test1-sysc

test1.exe:test1.cs
	$(CSC) test1.cs -r:$(KLIB)

test1a:test1.exe
	./runtest "test1" "" "-sim 80   -diosim-plot a.plt -conerefine=disable -bevelab=disable -verilog-gen=disable"  "k1sim"


test1b:test1.exe
	./runtest "test1" "" "-diosim-plot b.plt -sim 2080 -vnl DUT.v -dotplot-plot=combined  -kiwic-kcode-dump=enable     -kiwife-hgen-loglevel=10  "  "k2vnlsim"

# no sim
test1c:test1.exe
	./runtest "test1" "" "  -vnl DUT.v "  "k2vnlsim"
#-sim 2080 
	$(VERILOG) vsys.v DUT.v > spool &&  grep -c "1 3 5 7 9" spool 
	echo "test complete \n\n\n"

test1.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test1extra:		
	mcs test1.cs
	monodis test1.exe > test1.s
	../kiwic test1.s -root test1 -sim 2080  -diosim-plot a.plt -finish -vnl a.vnl 



# TODO: Get this from KDISTRO normally
HPRLS_CPP_SHIMS=$(HPRLS)/hpr/hprls_cpp_shims

test1-sysc:test1.exe
	$(KIWIC) test1.exe -cgen2=enable 
	$(CCP) -c test1.sysc.cpp -I$(SYSTEMC)/include -I$(HPRLS_CPP_SHIMS)

	exit 20202
#-------------------
.PHONY: test101

test101:test101a test101b
# test101c

test101a:test101.exe
	./runtest "test101" -root "test101;test101.Main -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test101b:test101.exe
	./runtest "test101" -root "test101;test101.Main -sim=1800  " "gldsim"

test101c:test101.exe
	./runtest "test101" -root "test101;test101.Main -sim=1800  -vnl-roundtrip=disable " "gldsim"

test101.exe:test101.cs
	$(CSC) test101.cs  -r:$(KLIB)

test101.goldspool:test101.exe
	MONO_PATH=$(KDISTRO)/support mono test101.exe
#-------------------------------------
# Basic concurrency
#  test2a  - Simple variable sharing between threads and locks (no fences)
#  test2r1 - Clean capsule passing along CSP-like channels
#  test2r2 - Shared variable spinning using test and set or lock constructs
#  test2r3 - needs further work?
.PHONY: test2
ROOT2="-root test2;test2.Main"


test2:test2a test2r1 test2r2
# test2r3


test2a: test2a.exe
	./runtest "test2a" "" " -sim 1178  -bondout-loadstore-port-count=0  -diosim-vcd=test2a.vcd "  "gldsim"

test2a.exe:test2a.cs
	$(CSC) test2a.cs  -r:$(KLIB)

test2a.goldspool:test2a.exe
	ulimit -t 10	; MONO_PATH=$(KDISTRO)/support mono test2a.exe

# -----------------
test2r1:test2r1a test2r1b 

test2r1.exe:test2r1.cs
	$(CSC) test2r1.cs  -r:$(KLIB)

test2r1a:test2r1.exe
	./runtest "test2r1" "" " -sim 1180   -restructure=disable  -conerefine=disable -repack=disable -bevelab=disable -verilog-gen=disable "  "gldsim"

test2r1b:test2r1.exe
	./runtest "test2r1" "" " -vnl-resets=synchronous -vnl DUT.v  -bondout-loadstore-port-count=0 -sim 1180 -diosim-vcd=test2r1.vcd  "  "gldsim"

test2r1.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)



test2r1.goldspool:test2r1.exe
	ulimit -t 10	; MONO_PATH=$(KDISTRO)/support mono test2r1.exe

test2z:	
	../kiwic test2.s $(ROOT2) -sim=1800  -vnl-resets=synchronous -vnl DUT.v
# -no-cone-refine 
#   always @(posedge clk) $display("test2 trace:  %d %d,  pc=%d %d ", WH_Capsule_fooCONBNET8, WH_Kiwi_Channel_1_0_emptyCONBNET34, testMain11finalpc, sumerClass12finalpc);

test2r2.exe:test2r2.cs
	$(CSC) test2r2.cs  -r:$(KLIB)

test2r2:test2r2a
# too many races: test2r2b 
#  -kiwic-cil-dump=combined  -kiwife-gtrace-loglevel=0

test2r2a:test2r2.exe
	./runtest "test2r2" "" " -sim 180   -conerefine=disable -bevelab=disable -verilog-gen=disable "  "gldsim"

test2r2b:test2r2.exe
	./runtest "test2r2" "" " -vnl-resets=synchronous -vnl DUT.v -sim 180"  "gldsimb"

test2r2.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out


test2r2.goldspool: test2r2.exe
	MONO_PATH=$(KDISTRO)/support mono test2r2.exe

test2r3.exe:test2r3.cs
	$(CSC) test2r3.cs  -r:$(KLIB)

test2r3:test2r3a test2r3b 

test2r3a:test2r3.exe
	./runtest "test2r3" "" " -sim 180 -conerefine=disable -bevelab=disable -verilog-gen=disable "  "gldsim"

test2r3b:test2r3.exe
	./runtest "test2r3" "" " -vnl-resets=synchronous -vnl DUT.v -sim 180"  "gldsimb"

test2r3.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out


test2r3.goldspool: test2r3.exe
	MONO_PATH=$(KDISTRO)/support mono test2r3.exe



#-------------------------------------
.PHONY: test3
test3:test3a test3b


test3.exe:test3.cs $(KLIB)
	$(CSC) test3.cs  -r:$(KLIB)


test3a:test3.exe
	./runtest "test3" "-root test3;test3.Main" " -conerefine=disable -bevelab=disable -verilog-gen=disable  -sim=1800"  "gldsim"

test3b:test3.exe
	./runtest "test3" "-root test3;test3.Main" " -vnl VNL.v -sim=1800"  "gldsim"

#-------------------------------------
.PHONY: test4p test4pa test4pb
test4p:test4pa test4pb 


test4p.exe:test4p.cs $(KLIB)
	$(CSC) test4p.cs  -r:$(KLIB)


test4pa:test4p.exe
	./runtest "test4p" "-root test4p;test4p.Main" "-conerefine disable -bevelab=disable  -verilog-gen=disable -sim 3000 "  "k2sim"


test4pb: 
	./runtest "test4p" "-root test4p;test4p.Main" " -vnl DUT4b.v -sim 3000 -restructure2=disable"  "k2vnlsim"

#-------------------------------------
.PHONY: test4 test4a test4b test4b1 test4b2 test4b3 test4bvanilla
test4:test4a test4b 

test4.exe:test4.cs $(KLIB)
	$(CSC) test4.cs  -r:$(KLIB)

# 
test4a:test4.exe
	./runtest "test4" "-root test4;test4.Main" "   -conerefine disable   -bevelab=disable  -verilog-gen=disable   -sim 3000 "  "k2sim"


test4b: test4b0 test4b1 test4b2 test4b3 test4bvanilla

# No restructure in 4b0.
test4b0:test4.exe
	./runtest "test4" "-root test4;test4.Main" " -vnl-rootmodname=DUT -vnl DUT4b0.v -vnl-resets=synchronous -sim 3000 -restructure2=disable -bevelab-recode-pc=disable"  "k2vnlsim"

# Force to unstructured regfile by making both large: the array is of size 9 so being larger or smaller than 9 is the significant aspect of these thresholds.
test4b1:test4.exe
	./runtest "test4" "-root test4;test4.Main" " -vnl DUT4b1.v -vnl-resets=synchronous -sim 3000 -res2-regfile-threshold=21 -res2-combram-threshold=22 -vnl-rootmodname=DUT"  "k2vnlsim"


test4b1.isim:
	$(VERILOG) vsys.v DUT4b1.v $(CV_INT_ARITH_LIB) && ./a.out
	cp vcd.vcd $(VIEWINGDEST)

# Force to structured regfile by making inbetween (same as vanilla)
#  
test4b2:test4.exe
	./runtest "test4" "-root test4;test4.Main" "  -vnl-rootmodname=DUT -vnl DUT4b2.v -vnl-resets=synchronous -res2-loglevel=0 -sim 3000 -diosim-vcd=diosim.vcd -bevelab-default-pause-mode=hard -res2-regfile-threshold=4 -res2-combram-threshold=20 "  "k2vnlsim"
	$(VERILOG) vsys.v DUT4b2.v $(CV_INT_ARITH_LIB) && ./a.out
	cp vcd.vcd $(VIEWINGDEST)

test4b2.isim:
	$(VERILOG) vsys.v DUT4b2.v $(CV_INT_ARITH_LIB) && ./a.out

# Force to synchronous SRAM by making both small
test4b3:test4.exe
	./runtest "test4" "-root test4;test4.Main" " -vnl-rootmodname=DUT -vnl DUT4b3.v -vnl-resets=synchronous -sim 3000 -res2-regfile-threshold=3 -res2-combram-threshold=5"  "k2vnlsim"

#Force to bondout DRAM is another route to test ...


test4bvanilla:test4.exe
	./runtest "test4" "-root test4;test4.Main" " -vnl-resets=synchronous -vnl DUT.v -sim 3000 "  "k2vnlsim"

test4bs:test4.exe
	./runtest "test4" "-root test4;test4.Main" " -vnl-resets=synchronous -vnl DUT.v -sim 3000 "  "k2vnlsim"


test4.goldspool:test4.exe
	MONO_PATH=$(KDISTRO)/support mono test4.exe
# -synthcontrol "array-scalarise=all"


test4.isim:
# $display("CLK  %h/%h,  ad=%d %d", mpc10, mpc10zz, DRSX32SS_AX_CC_SOL_QC_SOL_registered_AD0, DRSX32SS_AX_CC_SOL_QC_SOL_AD0);
# $display("Ram read %d  [%d] := %d", DRSX32SS_AX_CC_SOL_QC_SOL_REN0, DRSX32SS_AX_CC_SOL_QC_SOL_registered_AD0, DRSX32SS_AX_CC_SOL_QC_SOL_RDD0);
# $display("Ram write %d  [%d] := %d", DRSX32SS_AX_CC_SOL_QC_SOL_WEN0, DRSX32SS_AX_CC_SOL_QC_SOL_registered_AD0, DRSX32SS_AX_CC_SOL_QC_SOL_WRD0);
	$(VERILOG) vsys.v DUT4b3.v $(CV_INT_ARITH_LIB) && ./a.out
	cp vcd.vcd $(VIEWINGDEST)


# -et
#-------------------------------------
.PHONY: test5 test5a test5b test5bs test5c
# Test36 is similar
test5:test5a test5b 

# test5c - gives no clock error

test5.goldspool:test5.exe
	MONO_PATH=$(KDISTRO)/support mono test5.exe | tee test5.goldspool

test5a:test5.exe
	./runtest "test5" "-root test5;test5.Main" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable "  "gldsim"
test5b:test5.exe
	./runtest "test5" "-root test5;test5.Main" " -vnl DUT.v  -vnl-resets=synchronous -sim=1800  "  "gldsim"
test5bs:test5.exe
	./runtest "test5" "-root test5;test5.Main" "   -vnl-resets=synchronous -vnl=DUT.v -sim=1800"  "gldsim"

# 
test5c:test5.exe
	time ./runtest "test5" "-root test5;test5.Main" "-gatelib=cambridge -vnl=DUT.v -vnl-roundtrip=enable -sim=1800  "  "gldsim"
#    	     ./runtest "test5" "-root test5;test5.Main" "-gatelib cambridge -vnl DUT.v -sim=1800  "  "k2gatesim"



test5.exe:test5.cs
	$(CSC) test5.cs  -r:$(KLIB)

test5.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
#-------------------------------------
simple:
	./runtest "simple" "-root simple;simple.Main" "-vnl VNL.v -sim=1800 "  "k2gatesim"
#  "-gatelib cambridge -sim-gates 

#-------------------------------------
.PHONY: test6 test6a test6b1 test6b2 test6b3 test6b4 test6c test6gold
T6R="-root test6;test6.Main"

test6.exe:test6.cs
	$(CSC) test6.cs  -r:$(KLIB)

test6:test6a test6b test6c test6b1 test6b2 test6b3 test6b4

test6a:test6.exe
	./runtest "test6" $(T6R) " -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800 "  "k2rawsim"

test6b:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim=1800 -diosim-plot a.plt -vnl-ifshare=on "  "k2vnlsim"

test6b1:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim=1800 -diosim-plot a.plt -vnl-ifshare=simple "  "k2vnlsim"

test6b2:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim=1800 -diosim-plot a.plt -vnl-ifshare=none "  "k2vnlsim"

test6b3:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim 2800 -diosim-plot a.plt -bevelab-default-pause-mode=maximal "  "k2vnlsim"

test6b4:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim=1800 -diosim-plot a.plt -bevelab-default-pause-mode=bblock "  "k2vnlsim"

test6c:test6.exe
	./runtest "test6" $(T6R)  " -vnl DUT.v -sim=1800 -diosim-plot a.plt "  "k2vnlsim"

test6z:test6.exe
	../kiwic test6.exe -root test6 -sim 2080  -diosim-plot a.plt -finish -vnl a.v -vnl-preserve-sequencer=false

test6.goldspool:test6.exe
	MONO_PATH=$(KDISTRO)/support mono test6.exe > test6.goldspool

test6cs:
	$(VERILOG) vsys.v DUT.v

#-------------------------------------
# Test 7 - Overloading, overriding and object calling with polymorphic dynamic dispatch.
.PHONY: test7

test7:test7b

test7.exe:test7.cs
	$(CSC) test7.cs  -r:$(KLIB)

test7a:test7.exe
	./runtest "test7" " " " -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800  "  "gldsim"

test7b:test7.exe
	./runtest "test7" " " " -vnl DUT.v -sim=1800  "  "gldsim"

#  

# This test should be run without stage disables.

test7.goldspool:test7.exe
	MONO_PATH=$(KDISTRO)/support mono test7.exe > test7.goldspool

test7.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB) 
	./a.out | tee test7.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)

#-------------------------------------
.PHONY: test8 test8a test8b
# call by ref
#You need to make sure the ldloca used in the call to edict does not eval the first arg but keeps it as a ref.
#Log:Inlining call: CIL method test8/edict
# 2 call formal=[out] int32&,  call actual=*(CE(CT_ctype(int32, [-1]), test8/T11/Main/T11/Main/V_1, <>))
# 1 call formal=[out] int32&,  call actual=*(yb(num 1))  --- WRONG
# 1 call formal=[out] int32&,  call actual=yb(stars=1, test8/Main/Main/V_0)[0] -- CORRECT

test8.exe:test8.cs
	$(CSC) test8.cs  -r:$(KLIB)



# rawsim fails
test8:test8a test8b 
# test8c c gives a different order of the output logging.

test8a:test8.exe
	./runtest "test8" "-conerefine disable -bevelab=disable  -verilog-gen=disable -root test8;test8.Main" "-vnl VNL.v -sim=1800  "  "k2rawsim"

test8b:test8.exe
	./runtest "test8" "-root test8;test8.Main" "-vnl VNL.v -sim=1800  "  "k2vnlsim"

test8c:test8.exe
	./runtest "test8" "-root test8;test8.Main" "-vnl VNL.v -sim=1800    -fecontrol PCMain:2:test8/Main/Main/V_0"  "k2vnlsim"

test8s:
	$(VERILOG) vsys.v VNL.v
#-------------------
# Test9 - string literals used as data.
.PHONY: test9 test9 test9a

test9: test9a test9b

test9.exe:test9.cs
	$(CSC) test9.cs  -r:$(KLIB)

# Test9 cannot be expected to work very well under diosim since it requires acknowledgements from the env.
test9a:test9.exe
	./runtest "test9" "" "   -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800 "  "gldsim"

# -kiwife-gtrace-loglevel=0  

test9b:test9.exe
	./runtest "test9" "" "  -xor-heuristic-value=2 -vnl DUT.v -sim=1800  -bevelab-default-pause-mode=hard -vnl-resets=synchronous -bondout-loadstore-port-count=0   " "gldsim"
	iverilog PARPSYS.v DUT.v
	./a.out > test9.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test9.rtl-spool expecteds/test9.ok.rtl-spool

test9.goldspool:test9.exe
	MONO_PATH=$(KDISTRO)/support mono test9.exe


test9.msim:
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib PARPSYS.v PARP.v
	vsim -c simple_lib.PARPSYS -do "run.do" 

test9.isim:
	iverilog PARPSYS.v DUT.v
	./a.out | tee test9.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)


test9cs:
	cs  -nox -clearnet vsys.v DUT.v
#	cs  -nox -clearnet -root PARPSYS PARPSYS.v test9.v


#-------------------
.PHONY: test10

# Structural arithmetic tests.

test10:test10a test10b

test10.exe:test10.cs
	$(CSC) test10.cs  -r:$(KLIB)

test10a:test10.exe
	./runtest "test10" -root "test10;test10.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim 2800" "k2sim"

test10b:test10.exe
	./runtest "test10" -root "test10;test10.Main -vnl DUT.v -sim 2800 -diosim-vcd=test10b.vcd  -vnl-resets=synchronous " "k2sim"
	cp test10b.vcd $(VIEWINGDEST)

test10.gldsim:test10.exe
	MONO_PATH=$(KDISTRO)/support mono test10.exe

test10.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
.PHONY: test11

test11:test11a test11b

test11.exe:test11.cs
	$(CSC) test11.cs  -r:$(KLIB)


test11a:test11.exe
	./runtest "test11" -root "test11;test11.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800" "k2sim"

test11b:test11.exe
	./runtest "test11" -root "test11;test11.Main -vnl VNL.v -sim=1800" "k2vnlsim"

test11.goldspool:test11.exe
	MONO_PATH=$(KDISTRO)/support mono test11.exe

#-------------------
.PHONY: test12

test12:test12a-noscal test12a test12b test12c
# test12-mos

test12-mos:
	./runtest.mos "test12"  -root "test12;test12.Main" "k2sim"

test12a-noscal:test12.exe
	./runtest "test12" -root "test12;test12.Main   -conerefine=disable -bevelab=disable  -repack-constidx-scalarise=0 -verilog-gen=disable -sim=1800" "k2sim"

test12a:test12.exe
	./runtest "test12" -root "test12;test12.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable -sim=1800" "k2sim"


test12b:test12.exe
	./runtest "test12" -root "test12;test12.Main -vnl DUT.v   -sim=1800 -kiwife-gtrace-loglevel=0 -res2-regfile-threshold=121 -res2-combram-threshold=122" "k2sim"

# -kiwife-postgen-optimise=disable 

#  - Default sized put in RAM not regfile, especially if RP array size is not scaled down.
test12c:test12.exe
	./runtest "test12" -root "test12;test12.Main -vnl DUT.v -sim=1800" "k2sim"

test12.goldspool:test12.exe
	MONO_PATH=$(KDISTRO)/support mono test12.exe > test12.goldspool

test12.exe:test12.cs
	$(CSC) test12.cs  -r:$(KLIB)


test12.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out


#-------------------
.PHONY: test14

test14:test14a1 test14a2 test14b test14c test14d test14w test14x 

test14a1:test14.exe
	./runtest "test14" -root "test14;test14.Main -sim=1800 -conerefine=disable -bevelab=disable  -repack=disable -restructure2=disable -verilog-gen=disable " "k2sim"

test14a2:test14.exe
	./runtest "test14" -root "test14;test14.Main -sim=1800 -conerefine=disable -bevelab=disable  -restructure2=disable -verilog-gen=disable " "k2sim"

test14b:test14.exe
	./runtest "test14" -root "test14;test14.Main -sim=1800  -vnl DUT.v  -res2-regfile-threshold=121 -res2-combram-threshold=122 " "k2sim"

test14c:test14.exe
	./runtest "test14" -root "test14;test14.Main -sim=1800  -vnl DUT.v  -res2-regfile-threshold=1 -res2-combram-threshold=122 " "k2sim"

test14d:test14.exe
	./runtest "test14" -root "test14;test14.Main -sim=1800  -vnl DUT.v  -res2-regfile-threshold=1 -res2-combram-threshold=1 " "k2sim"

test14.goldspool:test14.exe
	MONO_PATH=$(KDISTRO)/support mono test14.exe > test14.goldspool

test14.exe:test14.cs
	$(CSC) test14.cs  -r:$(KLIB)

test14.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

#-------------------
.PHONY: test14w
#
# test14w - as per test14x but mutable data so ROM support is not invoked.
# 
test14w:test14wa1 test14wb 

test14w.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test14wa1:test14w.exe
	./runtest "test14w"  "" " -sim=1800 -conerefine=disable -bevelab=disable  -repack=disable -restructure2=disable -verilog-gen=disable " "gldsim"


test14wa2:test14w.exe
	./runtest "test14w"  "" " -sim=1800  -vnl DUT.v -conerefine=disable  -vnl-resets=synchronous -restructure2=disable  " "gldsim"


test14wb:test14w.exe
	./runtest "test14w" "" " -sim=1800  -vnl DUT.v   -res2-regfile-threshold=121 -vnl-resets=synchronous -res2-combram-threshold=122 " "gldsim"

test14w.exe:test14w.cs
	$(CSC) test14w.cs  -r:$(KLIB)

test14w.goldspool:test14w.exe
	MONO_PATH=$(KDISTRO)/support mono test14w.exe 

#-------------------
.PHONY: test14x

test14x:test14xa1 test14xa2 test14xb 

test14xa1:test14x.exe
	./runtest "test14x"  "" " -sim=1800  -conerefine=disable -bevelab=disable  -repack=disable -restructure2=disable -verilog-gen=disable " "gldsim"

test14xa2:test14x.exe
	./runtest "test14x" "" " -sim=1800 -conerefine=disable -bevelab=disable  -restructure2=disable -verilog-gen=disable " "gldsim"

test14xb:test14x.exe
	./runtest "test14x" "" " -sim=1800  -vnl DUT.v  -res2-regfile-threshold=121 -res2-combram-threshold=122 " "gldsim"

test14x.exe:test14x.cs
	$(CSC) test14x.cs  -r:$(KLIB)

test14x.goldspool:test14x.exe
	MONO_PATH=$(KDISTRO)/support mono test14x.exe 


#-------------------
.PHONY: test14y

# Fails since test14y.cs file is lost?

test14y:test14ya1 test14ya2 test14yb 

test14ya1:test14y.exe
	./runtest "test14y"  "" " -sim=1800  -conerefine=disable -bevelab=disable  -repack=disable -restructure2=disable -verilog-gen=disable " "gldsim"

test14ya2:test14y.exe
	./runtest "test14y" "" " -sim=1800 -conerefine=disable -bevelab=disable  -restructure2=disable -verilog-gen=disable " "gldsim"

test14yb:test14y.exe
	./runtest "test14y" "" " -sim=1800  -vnl DUT.v  -res2-regfile-threshold=121 -res2-combram-threshold=122 " "gldsim"

test14y.exe:test14y.cs
	$(CSC) test14y.cs  -r:$(KLIB)

test14y.goldspool:test14y.exe
	MONO_PATH=$(KDISTRO)/support mono test14y.exe 


#-------------------
.PHONY: test14z

test14z:test14za1 test14za2 test14zb 

test14za1:test14z.exe
	./runtest "test14z"  "" " -sim=1800  -conerefine=disable -bevelab=disable  -repack=disable -restructure2=disable -verilog-gen=disable " "gldsim"

test14za2:test14z.exe
	./runtest "test14z" "" " -sim=1800 -conerefine=disable -bevelab=disable  -restructure2=disable -verilog-gen=disable " "gldsim"

test14zb:test14z.exe
	./runtest "test14z" "" " -sim=1800  -vnl DUT.v  -res2-regfile-threshold=121 -res2-combram-threshold=122 " "gldsim"

test14z.exe:test14z.cs
	$(CSC) test14z.cs  -r:$(KLIB)

test14z.goldspool:test14z.exe
	MONO_PATH=$(KDISTRO)/support mono test14z.exe 


#-------------------
.PHONY: test15

test15:test15a test15b

test15a:test15.exe
	./runtest "test15" -root "test15;test15.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800" "k2sim"

test15b:test15.exe
	./runtest "test15" -root "test15;test15.Main -sim=1800  -vnl DUT.v  " "k2vnlsim"

test15.goldspool:test15.exe
	MONO_PATH=$(KDISTRO)/support mono test15.exe > test15.goldspool

test15.exe:test15.cs
	$(CSC) test15.cs  -r:$(KLIB)



#-------------------
.PHONY: test16

test16:test16a test16b


test16.exe:test16.cs
	$(CSC) test16.cs  -r:$(KLIB)


test16a:test16.exe
	./runtest "test16" -root "Adder.Adder;Adder.Adder.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800" "k2sim"

test16b:test16.exe
	./runtest "test16"  -root "Adder.Adder;Adder.Adder.Main -sim=1800 -vnl test16.v " "k2vnlsim"



#-------------------
.PHONY: test17
# test17: HOF: higher-order programming, simple delegates for concurrency or perhaps local application - lambdas ? cf test55

test17: test17b test17r2b test17r2b.isim test17r3b

test17.exe:test17.cs
	$(CSC) test17.cs  -r:$(KLIB)



test17b:test17.exe
	./runtest "test17"  "" "  -sim=1800  -vnl test17.v -kiwic-autodispose=enable   -kiwic-cil-dump=combined " "gldsim"

test17r2.exe:test17.cs
	$(CSC) test17r2.cs  -r:$(KLIB)


test17.goldspool:test17.exe
	MONO_PATH=$(KDISTRO)/support mono test17.exe

#  

test17r2a:test17r2.exe
	./runtest "test17r2"  "" "  -restructure=disable -conerefine=disable -repack=disable -bevelab=disable -verilog-gen=disable  -sim=1800  -vnl-rootmodname=DUT -vnl test17r2.v -kiwic-autodispose=enable  " "gldsim"


test17r2b:test17r2b_compile test17r2b.isim

test17r2b_compile:test17r2.exe
	./runtest "test17r2"  "" " -bondout-loadstore-port-count=0 -sim=1800  -vnl-rootmodname=DUT -vnl test17r2.v -kiwic-autodispose=enable " "gldsim"


test17r2b.goldspool:test17r2.exe
	MONO_PATH=$(KDISTRO)/support mono test17r2.exe

test17r2b.isim:
	iverilog test17r2.v vsys.v $(CVLIBS)
	./a.out | tee test17r2.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test17r2.rtl-spool expecteds/test17r2.ok.rtl-spool

test17r3b:test17r3.exe
	./runtest "test17r3"  "" "  -kiwic-kcode-dump=enable  -sim=1800  -vnl test17r3.v -kiwic-autodispose=enable   -kiwic-cil-dump=combined " "gldsim"

test17r3.exe:test17r3.cs
	$(CSC) test17r3.cs  -r:$(KLIB)


test17r3.goldspool:test17r3.exe
	MONO_PATH=$(KDISTRO)/support mono test17r3.exe




#-------------------
.PHONY: test18

test18:test18a test18b test18c test18d
# test18d - failed until Jan 2019

test18a:test18.exe

#	./runtest "test18" -root "Tog1;Tog2;Tog3;Tog3.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800   -kiwic-kcode-dump=enable -kiwife-hgen-loglevel=10 " "k2sim"
	./runtest "test18" -root "Tog1;Tog2;Tog3;Tog3.Main -conerefine=disable -bevelab=disable  -verilog-gen=disable -sim=1800  -kiwic-kcode-dump=enable" "k2sim"


test18b:test18.exe
	./runtest "test18"  -root "Tog1;Tog2;Tog3;Tog3.Main -sim=1800 -vnl test18.v -vnl-preserve-sequencer=false " "k2vnlsim"


test18c:test18.exe
	./runtest "test18"  -root "Tog1;Tog2;Tog3;Tog3.Main -sim=1800 -vnl test18.v -vnl-preserve-sequencer=true  -vnl-ifshare simple " "k2vnlsim"


test18d:test18.exe
	./runtest "test18"  -root "Tog1;Tog2;Tog3;Tog3.Main -sim=1800 -vnl test18.v -vnl-preserve-sequencer=true  -vnl-ifshare on " "k2vnlsim"


test18.exe:test18.cs
	$(CSC) test18.cs  -r:$(KLIB)


test18.goldspool:test18.exe
	MONO_PATH=$(KDISTRO)/support mono test18.exe | tee test18.goldspool



#ifshare={none, simple, on}
# -verbose2 

#-------------------
.PHONY: test19  test19_internally_instantiated test19_externally_instantiated  test19_internal_and_external
# 
# RPC tests with separately-compiled client and server.

# This did use the old style HSIMPLE.
# Now it is here replaced with HFAST/AXI-S but with a manual testbench for external instantiation. 
#
# The System Instantiator automated testbench runs will be in a different folder within the $(HPRLS) top level.
#
test19: killer19 test19_internal_and_external


test19_internal_and_external: test19_internally_instantiated test19_externally_instantiated


killer19:
	rm -f client19.v Test19_Server1.v client19_internal.v client19_external.v

Test19_Server1.v:test19.exe
	$(KIWIC) -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.Test19_Server1 test19.exe   -bondout-loadstore-port-count=0  -vnl Test19_Server1.v -vnl-kandr=disable -root "Test19_Server1;Test19_Server1.setget_pixel;Test19_Server1.get_id;Test19_Server1.start"

# INTERNAL

test19_internally_instantiated: Test19_Server1.v  client19.v test19_internally_instantiated.isim


client19.v:test19.exe
	$(KIWIC) -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.client19_internal test19.exe  -kiwic-cil-dump=combined   -vnl client19_internal.v -vnl-kandr=disable -root "test19_client1;test19_client1.Main"



#	$(KIWIC) -log-dir-name=obj.server19 -obj-dir-name=. test19.exe   -vnl server19.v -vnl-kandr=disable -root "test19_server1;test19_server1.setget_pixel;test19_server1.get_id;test19_server1.start"


# Hmmm how do we say whether we want servers or clients from a KiwiC run? It is via the root flag.
# 
client19_internal.v:test19.exe
	$(KIWIC) -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.client19 test19.exe  -kiwic-cil-dump=combined   -vnl client19.v -vnl-kandr=disable -root "test19_client1;test19_client1.Main"

# -----------------------
# EXTERNAL: External instantiation test uses a different testbench
test19_externally_instantiated: test19_externally_instantiated.isim

test19_externally_instantiated.isim: client19_external.v Test19_Server1.v
# Now we are automating more, we need the server compile's RTL output file (or at least the IP-XACT file) to have the same name as the C# class being instantiated.


client19_external.v:test19.exe
	$(KIWIC) -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.client19_external test19.exe  -kiwic-cil-dump=combined   -vnl client19_external.v -vnl-kandr=disable -root "test19_client1;test19_client1.Main" -res2-external-afus=Test19_Server1


test19_internally_instantiated.isim:Test19_Server1.v client19_internal.v 
	iverilog $(CVLIBS) Test19_Server1.v client19_internal.v test19internalsys.v 
	./a.out | tee test19-internally.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	grep -c "Remote server id is 1256" test19-internally.rtl-spool


#-------------------------------------
test19_internally_instantiated: Test19_Server1.v  client19_external.v test19_externally_instantiated.isim

test19_externally_instantiated.isim:Test19_Server1.v client19_external.v test19sys.v
	iverilog $(CVLIBS) Test19_Server1.v client19_external.v test19externalsys.v
	./a.out | tee test19-externally.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	grep -c "Remote server id is 1256" test19-externally.rtl-spool

test19.exe:test19.cs
	$(CSC) test19.cs  -r:$(KLIB)

# ifdef definition
#test19.dll:test19.cs
#	$(CSC) test19.cs /target:library /d:SERVER -r:$(KLIB)


#-------------------
.PHONY: test20

test20:test20a 

test20a:test20.exe
	./runtest "test20" -root "test20;test20.Main -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "k2sim"

test20.exe:test20.cs
	$(CSC) test20.cs  -r:$(KLIB)


#-------------------
.PHONY: test21 test21a1 test21a2 test21b1 test21b2 
#
# NB: This has an autobarrier since the meet's alg was added ?
# Made autobarrier work again on 18-Oct-11

test21:test21a1 test21a2 test21b1 test21b2 
# test21c

test21a1:test21.exe
	./runtest "test21" -root "test21;test21.Main -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "k2sim"

test21a2:test21.exe
	./runtest "test21" -root "test21;test21.Main -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "k2sim"


test21b1:test21.exe
	./runtest "test21" -root "test21;test21.Main -sim=1800 -vnl-resets=synchronous   -diosim-vcd=test21b1.vcd " "k2sim"

# same?
test21b2:test21.exe
	./runtest "test21" -root "test21;test21.Main -sim=1800 -vnl-resets=synchronous   " "k2sim"


#  bypass not working - net decls lost
test21c:test21.exe
	./runtest "test21" -root "test21;test21.Main -sim=1800  -vnl-roundtrip=enable " "k2sim"

test21.exe:test21.cs
	$(CSC) test21.cs  -r:$(KLIB)

test21.goldspool:test21.exe
	MONO_PATH=$(KDISTRO)/support mono test21.exe

test21simple:test21simple.exe
#	./runtest "test21simple" -root "test21simple;test21simple.Main -restructure2=disable -sim=1800  " "gld"
	./runtest "test21simple" -root "test21simple;test21simple.Main -sim=1800  -vnl-resets=synchronous" "gld"
	iverilog a.v vsys.v
	./a.out

test21simple.exe:test21simple.cs
	$(CSC) test21simple.cs  -r:$(KLIB)

test21simple.goldspool:test21simple.exe
	MONO_PATH=$(KDISTRO)/support mono test21simple.exe

test21.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test21.modelsim:
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib DUT.v vsys.v
	vsim -c simple_lib.SIMSYS -do "run.do" 

#       $display("%t, %d,  Write [%d] := %d", $time, xpc10zz, DSINT_AX_CC_RPA0_AD0, DSINT_AX_CC_RPA0_WRD0);
#-------------------
.PHONY: test22 - varargs

test22:test22a test22b


test22.exe:test22.cs
	$(CSC) test22.cs  -r:$(KLIB)

test22a:test22.exe
	./runtest "test22" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test22b:test22.exe
	./runtest "test22" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test22.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test22.goldspool:test22.exe
	MONO_PATH=$(KDISTRO)/support mono test22.exe


#-------------------
.PHONY: test23

test23:test23a test23b
# test23c

test23a:test23.exe
	./runtest "test23" -root "test23;test23.Main -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test23b:test23.exe
	./runtest "test23" -root "test23;test23.Main -sim=1800  " "gldsim"

#  bypass not working - net decls lost
test23c:test23.exe
	./runtest "test23" -root "test23;test23.Main -sim=1800  -bypa
ss-verilog-roundtrip=disable " "gldsim"

test23.exe:test23.cs
	$(CSC) test23.cs  -r:$(KLIB)

test23.goldspool:test23.exe
	MONO_PATH=$(KDISTRO)/support mono test23.exe


#-------------------
.PHONY: test24

test24:test24a test24b
# test24c

test24a:test24.exe
	./runtest "test24" ""  " -sim=1800 -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test24b:test24.exe
	./runtest "test24" "" " -sim=1800  " "gldsim"

#  bypass not working - net decls lost
test24c:test24.exe
	./runtest "test24" ""  " -sim=1800  -vnl-roundtrip=enable " "k2vnlsim"

test24.exe:test24.cs
	$(CSC) test24.cs  -r:$(KLIB)

test24.goldspool:test24.exe
	MONO_PATH=$(KDISTRO)/support mono test24.exe

#-------------------
.PHONY: test25

test25:test25a test25b1 test25b2 test25b3
# test25c

test25.exe:test25.cs
	$(CSC) test25.cs  -r:$(KLIB)

test25a:test25.exe
	./runtest "test25" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test25b1:test25.exe
	./runtest "test25" "-diosim-vcd=test25.vcd" " -sim=1800  -bevelab-default-pause-mode=hard" "gldsim"

test25b2:test25.exe
	./runtest "test25" "-diosim-vcd=test25.vcd" " -sim=1800  -bevelab-default-pause-mode=bblock" "gldsim"

test25b3:test25.exe
	./runtest "test25" "-diosim-vcd=test25.vcd" " -sim=1800  -bevelab-default-pause-mode=auto" "gldsim"


test25.goldspool:test25.exe
	MONO_PATH=$(KDISTRO)/support mono test25.exe


#-------------------
.PHONY: test26 #  pass by reference

test26:test26a test26b
# test26c

test26.exe:test26.cs
	$(CSC) test26.cs  -r:$(KLIB)

test26a:test26.exe
	./runtest "test26" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -kiwic-cil-dump=separately " "gldsim"

test26b:test26.exe
	./runtest "test26" "-diosim-vcd=test26.vcd" " -sim=1800  " "gldsim"


test26.goldspool:test26.exe
	MONO_PATH=$(KDISTRO)/support mono test26.exe


#-------------------
.PHONY: test27 #  null pointers twiddle

# null pointer runtime abend is in test??

test27:test27a test27b
# test27c

test27.exe:test27.cs
	$(CSC) test27.cs  -r:$(KLIB)

test27a:test27.exe
	./runtest "test27" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -kiwic-cil-dump=separately " "gldsim"

test27b:test27.exe
	./runtest "test27" "-diosim-vcd=test27.vcd" " -sim=1800  " "gldsim"

test27.goldspool:test27.exe
	MONO_PATH=$(KDISTRO)/support mono test27.exe

#-------------------
.PHONY: test27x #  null pointers twiddle

# null pointer runtime abend is in test??

test27x:test27xa test27xb
# test27xc

test27x.exe:test27x.cs
	$(CSC) test27x.cs  -r:$(KLIB)

test27xa:test27x.exe
	./runtest "test27x" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -kiwic-cil-dump=separately " "gldsim"

test27xb:test27x.exe
	./runtest "test27x" "-diosim-vcd=test27x.vcd" " -sim=1800  " "gldsim"

test27x.goldspool:test27x.exe
	MONO_PATH=$(KDISTRO)/support mono test27x.exe

#-------------------
.PHONY: test28

test28:test28a test28b1 test28b2


test28.exe:test28.cs
	$(CSC) test28.cs  -r:$(KLIB)

test28a:test28.exe
	./runtest "test28" "" " -sim=1800 -repack=disable -conerefine=disable -bevelab=disable  -restructure2=disable -verilog-gen=disable " "gldsim"

test28b1:test28.exe
	./runtest "test28" "-diosim-vcd=test28.vcd" " -sim=1800  -restructure2=disable" "gldsim"

test28b2:test28.exe
	./runtest "test28" "-diosim-vcd=test28.vcd" " -sim=1800  " "gldsim"


test28.goldspool:test28.exe
	MONO_PATH=$(KDISTRO)/support mono test28.exe


#-------------------
.PHONY: test29

test29:test29a1 test29a2 test29b
# test29c

test29.exe:test29.cs
	$(CSC) test29.cs  -r:$(KLIB)

test29a1:test29.exe
	./runtest "test29" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test29a2:test29.exe
	./runtest "test29" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test29b:test29.exe
	./runtest "test29" "  -diosim-vcd=test29.vcd" " -vnl-resets=synchronous -vnl DUT.v -sim=1800  " "gldsim"


test29.goldspool:test29.exe
	MONO_PATH=$(KDISTRO)/support mono test29.exe

test29.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out > test29.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test29.rtl-spool expecteds/test29.ok.rtl-spool

#-------------------
.PHONY: test30 test30r0a1 test30r0a2 test30r0b test30r1 test30r1a1 test30r1a2 test30r1b

# note: monodis seems to just crash if applied to the test30 .exe files.

test30: test30r0 test30r2

test30r0:test30r0a1 test30r0a2 test30r0b


test30r0.exe:test30r0.cs
	$(CSC) test30r0.cs  -r:$(KLIB)

test30r0a1:test30r0.exe
	./runtest "test30r0" "" " -sim=1800  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test30r0a2:test30r0.exe
	./runtest "test30r0" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test30r0b:test30r0.exe
	./runtest "test30r0" "-diosim-vcd=test30r0.vcd" " -sim=1800  " "gldsim"


test30r0.goldspool:test30r0.exe
	MONO_PATH=$(KDISTRO)/support mono test30r0.exe
.PHONY: test30r0

# r1 src is missing from git ?
test30r1:test30r1a1 test30r1a2 test30r1b

test30r1.exe:test30r1.cs
	$(CSC) test30r1.cs  -r:$(KLIB)

test30r1a1:test30r1.exe
	./runtest "test30r1" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test30r1a2:test30r1.exe
	./runtest "test30r1" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test30r1b:test30r1.exe
	./runtest "test30r1" "-diosim-vcd=test30r1.vcd" " -sim=1800  " "gldsim"


test30r1.goldspool:test30r1.exe
	MONO_PATH=$(KDISTRO)/support mono test30r1.exe

test30r2:test30r2a1 test30r2a2 test30r2b

test30r2.exe:test30r2.cs
	$(CSC) test30r2.cs  -r:$(KLIB)

test30r2a1:test30r2.exe
	./runtest "test30r2" "" "  -kiwic-cil-dump=separately  -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test30r2a2:test30r2.exe
	./runtest "test30r2" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test30r2b:test30r2.exe
	./runtest "test30r2" "-kiwic-kcode-dump=enable -diosim-vcd=test30r2.vcd" " -sim=1800  " "gldsim"


test30r2.goldspool:test30r2.exe
	MONO_PATH=$(KDISTRO)/support mono test30r2.exe


#-------------------
.PHONY: test31

test31:test31a1 test31a2 test31b
# test31c

test31.exe:test31.cs
	$(CSC) test31.cs  -r:$(KLIB)

test31a1:test31.exe
	./runtest "test31" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test31a2:test31.exe
	./runtest "test31" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"

test31b:test31.exe
	./runtest "test31" "-diosim-vcd=test31.vcd" " -sim=1800  " "gldsim"


test31.goldspool:test31.exe
	MONO_PATH=$(KDISTRO)/support mono test31.exe


#-------------------
.PHONY: test32 - integer dividers

# What about divide-by-zero abend checkng ?

test32:test32a1 test32a2 test32b test32.isim
# test32c

test32.exe:test32.cs
	$(CSC) test32.cs  -r:$(KLIB)

test32a1:test32.exe
	./runtest "test32" "" " -sim 4800  -kiwic-cil-dump=combined -kiwic-kcode-dump=enable  -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test32a2:test32.exe
	./runtest "test32" "" " -sim 4800 -conerefine=disable -bevelab=disable  -verilog-gen=disable " "gldsim"


# Instantiates CV_INT_VL_DIVIDER_S from cvgates.v
test32b1:test32.exe
	./runtest "test32" "-max-no-int-divs=1 -diosim-vcd=test32b1.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"

test32b:test32.exe
	./runtest "test32" "-max-no-int-divs=1 -diosim-vcd=test32b1.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"
	./runtest "test32" "-max-no-int-divs=2 -diosim-vcd=test32b2.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"
	./runtest "test32" "-max-no-int-divs=3 -diosim-vcd=test32b3.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"
	./runtest "test32" "-max-no-int-divs=4 -diosim-vcd=test32b4.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"
	./runtest "test32" "-max-no-int-divs=5 -diosim-vcd=test32b5.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 4800  " "gldsim"

# when not reusing divider10 owing to allocation bug, finished at 110us.

test32b5:test32.exe
	./runtest "test32" "-max-no-int-divs=5 -diosim-vcd=test32b5.vcd -vnl-resets=synchronous -bondout-loadstore-port-count=0" " -vnl DUT.v -sim 3800  " "gldsim"

test32.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out > test32.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)


test32.goldspool:test32.exe
	MONO_PATH=$(KDISTRO)/support mono test32.exe

#-------------------
.PHONY: test33 - complex type variables

test33:test33a
# test32c

test33.exe:test33.cs
	$(CSC) test33.cs  -r:$(KLIB)

test33a:test33.exe
	./runtest "test33" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test33.goldspool:test33.exe
	MONO_PATH=$(KDISTRO)/support mono test33.exe


#-------------------
.PHONY: test34 - floating point

test34:test34a test34b


test34.exe:test34.cs
	$(CSC) test34.cs  -r:$(KLIB)

test34a:test34.exe
	./runtest "test34" "" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test34b:test34.exe
	./runtest "test34" "" " -sim=1800 -conerefine=disable  -repack=disable -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test34.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test34.goldspool:test34.exe
	MONO_PATH=$(KDISTRO)/support mono test34.exe


#-------------------
.PHONY: test35 - structure assigns

test35:test35a test35b


test35.exe:test35.cs
	$(CSC) test35.cs  -r:$(KLIB)



test35a:test35.exe
	./runtest "test35" "" " -sim=1800  -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable  " "gldsim"

test35b:test35.exe
	./runtest "test35" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test35.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test35.goldspool:test35.exe
	MONO_PATH=$(KDISTRO)/support mono test35.exe

#-------------------
.PHONY: test36 - structure mappings of fields to RAMs example

test36:test36a test36b


test36.exe:test36.cs
	$(CSC) test36.cs  -r:$(KLIB)

test36a:test36.exe
	./runtest "test36" "-diosim-clear-arrays=enable -diosim-clear-scalars=enable" " -sim=1800 -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable  -restructure2=disable " "gldsim"

test36b:test36.exe
	./runtest "test36" "-diosim-clear-arrays=enable -diosim-clear-scalars=enable" " -sim 19000  -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test36.isim:
	iverilog vsys.v DUT.v $(CVLIBS) $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test36.goldspool:test36.exe
	MONO_PATH=$(KDISTRO)/support mono test36.exe

#-------------------
.PHONY: test37 - polymorphic with arrays

test37:test37a test37b


test37.exe:test37.cs
	$(CSC) test37.cs  -r:$(KLIB)

test37a:test37.exe
	./runtest "test37" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test37b:test37.exe
	./runtest "test37" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test37.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test37.goldspool:test37.exe
	MONO_PATH=$(KDISTRO)/support mono test37.exe

#-------------------
.PHONY: test38 test38r0 test38r1 test38r2 test38r3 test38r4

test38: test38r0 test38r1 test38r2 test38r3 test38r4

test38r0:test38r0a test38r0b # - ultra simple array of objects

test38r0.exe:test38r0.cs
	$(CSC) test38r0.cs  -r:$(KLIB)

test38r0a:test38r0.exe
	./runtest "test38r0" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test38r0b:test38r0.exe
	./runtest "test38r0" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test38r0.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test38r0.goldspool:test38r0.exe
	MONO_PATH=$(KDISTRO)/support mono test38r0.exe



#-------------------
.PHONY: test38r1 - ultra simple array of objects

test38r1:test38r1a test38r1b


test38r1.exe:test38r1.cs
	$(CSC) test38r1.cs  -r:$(KLIB)

test38r1a:test38r1.exe
	./runtest "test38r1" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test38r1b:test38r1.exe
	./runtest "test38r1" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test38r1.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test38r1.goldspool:test38r1.exe
	MONO_PATH=$(KDISTRO)/support mono test38r1.exe

#-------------------
.PHONY: test38r2 - ultra simple array of objects

test38r2:test38r2a test38r2b


test38r2.exe:test38r2.cs
	$(CSC) test38r2.cs  -r:$(KLIB)

test38r2a:test38r2.exe
	./runtest "test38r2" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test38r2b:test38r2.exe
	./runtest "test38r2" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test38r2.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test38r2.goldspool:test38r2.exe
	MONO_PATH=$(KDISTRO)/support mono test38r2.exe

#-------------------
.PHONY: test38r3

test38r3:test38r3a1 test38r3a2 test38r3b


test38r3.exe:test38r3.cs
	$(CSC) test38r3.cs  -r:$(KLIB)

test38r3a1:test38r3.exe
	./runtest "test38r3" "" " -sim=1800  -restructure2=disable -restructure1=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test38r3a2:test38r3.exe
	./runtest "test38r3" "" "  -sim=1800    -vnl-resets=synchronous -restructure2=disable -vnl DUT.v" "gldsim"

# -conerefine-loglevel=0

test38r3b:test38r3.exe
	./runtest "test38r3" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test38r3.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test38r3.goldspool:test38r3.exe
	MONO_PATH=$(KDISTRO)/support mono test38r3.exe

#-------------------
.PHONY: test38r4

test38r4:test38r4a test38r4b


test38r4.exe:test38r4.cs
	$(CSC) test38r4.cs  -r:$(KLIB)

test38r4a:test38r4.exe
	./runtest "test38r4" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test38r4b:test38r4.exe
	./runtest "test38r4" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test38r4.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test38r4.goldspool:test38r4.exe
	MONO_PATH=$(KDISTRO)/support mono test38r4.exe

#-------------------
# test39: Linked List Demos 

# .PHONY: test39  test39r0 test39r1 test39r0a test39r0b test39r1a test39r1b test39r2

test39: test39r0 test39r1 test39r2

# -- r0

test39r0:test39r0a test39r0b

test39r0.exe:test39r0.cs
	$(CSC) test39r0.cs  -r:$(KLIB)

test39r0a:test39r0.exe
	./runtest "test39r0" "" " -sim=1800     -restructure2=disable -conerefine=disable -bevelab=disable  -kiwic-kcode-dump=enable -verilog-gen=disable -repack=disable " "gldsim"

test39r0b:test39r0.exe
	./runtest "test39r0" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v   " "gldsim"

test39r0.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test39r0.goldspool:test39r0.exe
	MONO_PATH=$(KDISTRO)/support mono test39r0.exe

# -- r1

test39r1: test39r1a test39r1b 

test39r1.exe:test39r1.cs
	$(CSC) test39r1.cs /unsafe  -r:$(KLIB)


test39r1a:test39r1.exe 
	./runtest "test39r1" "" " -sim=1800 -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test39r1b:test39r1.exe 
	./runtest "test39r1" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"



test39r1.isim:
#	./runtest "test39r1" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

# This has stopped working: "Specified cast is not valid."
test39r1.goldspool:test39r1.exe
	MONO_PATH=$(KDISTRO)/support mono test39r1.exe


llhelp.dll:llhelp.pe
	ilasm /dll llhelp.pe

# -- r2

test39r2: test39r2a test39r2b test39r2.isim

test39r2.exe:test39r2.cs  llhelp.dll
	$(CSC) test39r2.cs /unsafe  -r:$(KLIB) -r:llhelp.dll

test39r2a:test39r2.exe 
	./runtest "test39r2" "" " -sim=1800   -kiwic-cil-dump=combined -kiwic-autodispose=enable -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test39r2b:test39r2.exe 
	./runtest "test39r2" "" "   -kiwic-cil-dump=combined -bevelab-loglevel=0   -kiwic-kcode-dump=enable -sim=1800 -vnl-resets=synchronous -diosim-vcd=a.vcd  -vnl DUT.v" "gldsim"
# -kiwic-autodispose=enable 


test39r2.isim:
	iverilog vsys-dualduplex.v DUT.v $(CVLIBS) TEMP_HEAPMANAGER.v  $(KDISTRO)/support/rams/ddr2-models.v
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test39r2.goldspool:test39r2.exe
	MONO_PATH=$(KDISTRO)/support mono test39r2.exe


#-------------------
# Unsafe demos
.PHONY: test40 test40r0a test40r0b test40r1a test40r1b


test40:test40r0a test40r0b test40r1a test40r1b 

test40r0.exe:test40r0.cs
	$(CSC) /unsafe test40r0.cs  -r:$(KLIB)

test40r0a:test40r0.exe
	./runtest "test40r0" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test40r0b:test40r0.exe
	./runtest "test40r0" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"




test40r0.goldspool:test40r0.exe
	MONO_PATH=$(KDISTRO)/support mono test40r0.exe

test40r1.exe:test40r1.cs
	$(CSC) /unsafe  test40r1.cs  -r:$(KLIB)

test40r1a:test40r1.exe
	./runtest "test40r1" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test40r1b:test40r1.exe
	./runtest "test40r1" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test40.isim:
	iverilog vsys.v DUT.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test40r1.goldspool:test40r1.exe
	MONO_PATH=$(KDISTRO)/support mono test40r1.exe


#-------------------
# BRAM access patterns tests
.PHONY: test41 test41r0a test41r0b test41r1a test41r1b


test41:test41r0a test41r0b test41r1a test41r1b 

test41r0.exe:test41r0.cs
	$(CSC) /unsafe test41r0.cs  -r:$(KLIB)

test41r0a:test41r0.exe
	./runtest "test41r0" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test41r0b:test41r0.exe
	./runtest "test41r0" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"




test41r0.goldspool:test41r0.exe
	MONO_PATH=$(KDISTRO)/support mono test41r0.exe

test41r1.exe:test41r1.cs
	$(CSC) /unsafe  test41r1.cs  -r:$(KLIB)

test41r1a:test41r1.exe
	./runtest "test41r1" "" " -sim=1800  -restructure2=disable -conerefine=disable -bevelab=disable  -verilog-gen=disable -repack=disable " "gldsim"

test41r1b:test41r1.exe
	./runtest "test41r1" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v" "gldsim"


test41.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


test41r1.goldspool:test41r1.exe
	MONO_PATH=$(KDISTRO)/support mono test41r1.exe

#-------------------
# Substrate Tests
.PHONY: test42 test42r0


test42:test42r0

test42.exe:test42.cs
	$(CSC) test42.cs  -r:$(KLIB)


test42r0:test42.exe
	./runtest "test42" "" " -sim=1800 -vnl-resets=synchronous -vnl test42.v -bondout-loadstore-port-count=0 -vnl-rootmodname=TEST42SLAVE -kiwic-finish=disable -conerefine=disable " "gldsim"
	iverilog test42sys.v test42.v
	./a.rtl-spool > test42r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test42r0.rtl-spool expecteds/test42r0.ok.out

test42r0.goldspool:test42.exe
	MONO_PATH=$(KDISTRO)/support mono test42.exe


test42.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
# Checked overflow
.PHONY: test43 test43r0


test43:test43r0

test43.exe:test43.cs
	$(CSC) test43.cs  -r:$(KLIB)


test43r0:test43.exe
	./runtest "test43" "" " -sim=1800 -vnl-resets=synchronous -vnl test43.v -bondout-loadstore-port-count=0 -vnl-rootmodname=TEST43SLAVE -kiwic-finish=disable -conerefine=disable -kiwic-kcode-dump=enable" "gldsim"
	iverilog test43sys.v test43.v
	./a.out > test43r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test43r0.rtl-spool expecteds/test43r0.ok.out

test43r0.goldspool:test43.exe
	MONO_PATH=$(KDISTRO)/support mono test43.exe


test43.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

#-------------------
# Hard Pause Mode R/W to Synchronous RAMs
.PHONY: test44 test44r0


test44:test44r0

test44.exe:test44.cs
	$(CSC) test44.cs  -r:$(KLIB)


test44r0:test44.exe
	./runtest "test44" "" " -sim=1800 -vnl-resets=synchronous -vnl DUT.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -compose=disable " "gldsim"
	iverilog test44sys.v test44.v
	./a.out > test44r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test44r0.rtl-spool expecteds/test44r0.ok.out

test44r0.goldspool:test44.exe
	MONO_PATH=$(KDISTRO)/support mono test44.exe


test44.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)




#-------------------
# BitConverter Tests
.PHONY: test45 test45r0


test45:test45r0

test45.exe:test45.cs
	$(CSC) test45.cs  -r:$(KLIB)


test45r0a:test45.exe
	./runtest "test45" "" " -sim=1800 -vnl-resets=synchronous -vnl test45.v -conerefine=disable -bevelab=enable  -restructure2=disable  -verilog-gen=disable  -vnl-rootmodname=DUT " "gldsim"


test45r0:test45.exe
	./runtest "test45" "" " -cil-uwind-budget=400  -kiwic-kcode-dump=enable   -res2-regfile-threshold=8 -kiwic-autodispose=enable -sim=1800 -vnl-resets=synchronous -vnl test45.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft     -vnl-rootmodname=DUT " "gldsim"
	iverilog vsys.v test45.v
	./a.out > test45r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test45r0.rtl-spool expecteds/test45r0.ok.out

test45r0.goldspool:test45.exe
	MONO_PATH=$(KDISTRO)/support mono test45.exe


test45.isim:
	iverilog vsys.v test45.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
# IndexOf and so on. Best to split out?
.PHONY: test46 test46r0

test46:test46r0

test46.exe:test46.cs
	$(CSC) test46.cs  -r:$(KLIB)


test46r0:test46.exe
	./runtest "test46" "" " -sim=1800 -vnl-resets=synchronous -vnl test46.v -vnl-rootmodname=DUT -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard  " "gldsim"
# 
	iverilog vsys.v test46.v
	./a.out > test46r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test46r0.rtl-spool expecteds/test46r0.ok.out

test46.goldspool:test46.exe
	MONO_PATH=$(KDISTRO)/support mono test46.exe


test46.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)




#-------------------
# Bounded recursion.
.PHONY: test48 test48r0

test48:test48r0

test48.exe:test48.cs
	$(CSC) test48.cs  -r:$(KLIB)


test48r0:test48.exe
	./runtest "test48" "" " -sim=1800 -vnl-resets=synchronous -vnl=test48.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard  " "gldsim"
# 
	iverilog vsys.v test48.v
	./a.out > test48r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test48r0.rtl-spool expecteds/test48r0.ok.out

test48.goldspool:test48.exe
	MONO_PATH=$(KDISTRO)/support mono test48.exe


test48.isim:
	iverilog vsys.v DUT.v $(CV_INT_ARITH_LIB)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
# Simple floating-point arithmetic
.PHONY: test49 test49r0

test49:test49r0

test49.exe:test49.cs
	$(CSC) test49.cs  -r:$(KLIB)

#   -kiwife-gtrace-loglevel=0  -kiwic-kcode-dump=enable

test49r0:test49.exe
	./runtest "test49" "" "  -sim=1800  -diosim-vcd=test49.vcd -vnl-rootmodname=DUT  -vnl-resets=synchronous -vnl=test49.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard  " "gldsim"
# 
	iverilog vsys.v test49.v $(CVLIBS)
	./a.out > test49r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test49r0.rtl-spool expecteds/test49r0.ok.out

test49.goldspool:test49.exe
	MONO_PATH=$(KDISTRO)/support mono test49.exe


test49.isim:
	iverilog vsys.v test49.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
# Shared memory dual-port RAMs.
.PHONY: test50 test50r0

test50:test50r0

test50.exe:test50.cs
	$(CSC) test50.cs  -r:$(KLIB)

# -conerefine-loglevel=0
# -kiwife-hgen-loglevel=10  -kiwife-constvol-loglevel=10 -kiwife-gtrace-loglevel=10 -kiwife-ataken-loglevel=10
# OLD? : test50 needs soft pause mode to auto avoid a name alias or else -compose=disable
test50r0:test50.exe
	./runtest "test50" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft" "gldsim"
#	./runtest "test50" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test50.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft  " "gldsim"
# 
	iverilog vsys.v test50.v $(CVLIBS)
	./a.out > test50r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)

# Race conditions to fix before diff
	diff test50r0.rtl-spool expecteds/test50r0.ok.out

test50.goldspool:test50.exe
	MONO_PATH=$(KDISTRO)/support mono test50.exe


test50.isim:
	iverilog vsys.v test50.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)



#-------------------
# Wide I/O busses and structs (greater than 64 bits).
.PHONY: test51 test51r0

test51:test51br0

test51a.exe:test51a.cs
	$(CSC) test51a.cs  -r:$(KLIB)

# -conerefine-loglevel=0
# test51a needs soft pause mode to auto avoid a name alias or else -compose=disable
test51ar0:test51a.exe
	./runtest "test51a" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51a.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft  " "gldsim"
# 
	iverilog vsys.v test51a.v $(CVLIBS)
	./a.out > test51ar0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test51ar0.rtl-spool expecteds/test51ar0.ok.out

test51a.goldspool:test51a.exe
	MONO_PATH=$(KDISTRO)/support mono test51a.exe


test51a.isim:
	iverilog vsys.v test51a.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test51b.exe:test51b.cs
	$(CSC) test51b.cs  -r:$(KLIB)

# -conerefine-loglevel=0
# test51b needs soft pause mode to auto avoid a name alias or else -compose=disable
test51br0:test51b.exe
	./runtest "test51b" "" "  -sim 10 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test51b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=soft    " "gldsim"
# 
	iverilog vsys51.v test51b.v $(CVLIBS)
	./a.out > test51br0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test51br0.rtl-spool expecteds/test51br0.ok.out

test51b.goldspool:test51b.exe
	MONO_PATH=$(KDISTRO)/support mono test51b.exe


test51b.isim:
	iverilog vsys.v test51b.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#-------------------
# Integer to ASCII
.PHONY: test52 test52r0

test52:test52r0

test52.exe:test52.cs
	$(CSC) test52.cs  /r:$(KLIB) /r:$(KSTRINGIOLIB)


test52r0:test52.exe
	./runtest "test52" "/r:$(KSTRINGIOLIB)" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test52.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
# 
	iverilog vsys.v test52.v $(CVLIBS)
	./a.out > test52r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test52r0.rtl-spool expecteds/test52r0.ok.out

test52.goldspool:test52.exe
	MONO_PATH=$(KDISTRO)/support mono test52.exe

test52.isim:
	iverilog vsys.v test52.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

#----------------------
# // Test53 - mutable formals and register reuse at different stack depths.
.PHONY: test53 test53b

test53:test53b

test53.exe:test53.cs
	$(CSC) test53.cs  /r:$(KLIB) /r:$(KSTRINGIOLIB)


test53b:test53.exe
	./runtest "test53" "/r:$(KSTRINGIOLIB)" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test53.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock  " "gldsim"
# 
	iverilog vsys.v test53.v $(CVLIBS)
	./a.out > test53b.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test53b.rtl-spool expecteds/test53b.ok.out

test53.goldspool:test53.exe
	MONO_PATH=$(KDISTRO)/support mono test53.exe

test53.isim:
	iverilog vsys.v test53.v $(CVLIBS)

#----------------------
# Test54 Pipelined Accelerator
.PHONY: test54 test54r0

test54:test54r0

test54.exe:test54.cs
	$(CSC) test54.cs  /r:$(KLIB) /r:$(KSTRINGIOLIB)


test54r0:test54.exe
	./runtest "test54" "/r:$(KSTRINGIOLIB)" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test54.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock  " "gldsim"
# 
	iverilog vsys.v test54.v $(CVLIBS)
	./a.out > test54r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test54r0.rtl-spool expecteds/test54r0.ok.out

test54.goldspool:test54.exe
	MONO_PATH=$(KDISTRO)/support mono test54.exe

test54.isim:
	iverilog vsys.v test54.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)


#----------------------
# Test 55 Closures and Delegates cf test17

.PHONY: test55 test55r0

test55:test55r0

test55.exe:test55.cs
	$(CSC) /r:$(KLIB) test55.cs  

test55.isim:test55.v
	iverilog vsys.v test55.v $(CV_INT_ARITH_LIB)
	./a.out

test55r0:test55.exe
	./runtest "test55" "/r:$(KSTRINGIOLIB)" " -sim=1800  -kiwic-autodispose=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test55.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock  " "gldsim"
# 
	iverilog vsys.v test55.v $(CVLIBS)
	./a.out > test55r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test55r0.rtl-spool expecteds/test55r0.ok.out


test55.goldspool:test55.exe
	MONO_PATH=$(KDISTRO)/support mono test55.exe

#----------------------
# Test 56 Cordic (sortof) Test

.PHONY: test56 test56r0

test56:test56r0

test56.exe:test56.cs
	$(CSC) /r:$(KLIB) test56.cs  

test56.isim:test56.v
	iverilog vsys.v test56.v    $(CVLIBS)
	./a.out

test56r0:test56.v.kill test56.v test56r0sim

test56.v.kill:
	rm -f test56.v



test56.v:test56.exe
	./runtest "test56" "" " -sim=18000  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock  " "gldsim"
# 
test56r0sim:test56.v
	iverilog vsys.v test56.v $(CVLIBS)
	./a.out > test56r0.rtl-spool
#	iverilog vsys.v test56.v /home/djg11/Dropbox/o/cvgates.v  /home/djg11/Dropbox/o/cv_fparith.v
#	./a.out > test56r0.rtl-spool-other
	cp vcd.vcd $(VIEWINGDEST)
	diff test56r0.rtl-spool expecteds/test56r0.ok.out

test56.goldspool:test56.exe
	MONO_PATH=$(KDISTRO)/support mono test56.exe

#----------------------
# 56bb proper Cording

.PHONY: test56b test56br0

test56b:test56br0

test56b.exe:test56b.cs
	$(CSC) /r:$(KLIB) test56b.cs  

test56b.isim:test56b.v
	iverilog vsys.v test56b.v    $(CVLIBS)
	./a.out

test56br0:test56b.v.kill test56b.v test56br0sim

test56b.v.kill:
	rm -f test56b.v



test56b.v:test56b.exe
	./runtest "test56b" "" " -sim=18000  -kiwife-firstpass-loglevel=5 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test56b.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock  " "gldsim"
# 
test56br0sim:test56b.v
	iverilog vsys.v test56b.v $(CVLIBS)
	./a.out > test56br0.rtl-spool
#	iverilog vsys.v test56b.v /home/djg11/Dropbox/o/cvgates.v  /home/djg11/Dropbox/o/cv_fparith.v
#	./a.out > test56br0.rtl-spool-other
	cp vcd.vcd $(VIEWINGDEST)
	diff test56br0.rtl-spool expecteds/test56br0.ok.out

test56b.goldspool:test56b.exe
	MONO_PATH=$(KDISTRO)/support mono test56b.exe

#----------------------
# Test 57 Array Base Manips - Strangely no longer working?

.PHONY: test57a test57b 

test57:test57a 

# test57b

# note that half of test57 is commented out
# still broken test57b

test57.exe:test57.cs
	$(CSC) /r:$(KLIB) test57.cs  

test57a:test57.exe
	./runtest "test57" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable "  "gldsim"

test57b:test57.exe
	./runtest "test57" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test57.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
# 
	iverilog vsys.v test57.v $(CVLIBS)
	./a.out > test57r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test57r0.rtl-spool expecteds/test57r0.ok.out

test57.isim:test57.v
	iverilog vsys.v test57.v $(CV_INT_ARITH_LIB)   $(CVLIBS)
	./a.out


test57.goldspool:test57.exe
	MONO_PATH=$(KDISTRO)/support mono test57.exe

#----------------------
# Test 58 sqrt
.PHONY: test58 test58r0a test58r0b test58r1 test58r1b

test58:test58r0a  test58r0b # test58r1

test58r1: test58r1b


test58r0.exe:test58r0.cs
	$(CSC) /r:$(KLIB) test58r0.cs  

test58r0a:test58r0.exe
	./runtest "test58r0" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test58r0b:test58r0.exe
	./runtest "test58r0" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58r0.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
#  -kiwic-kcode-dump=enable
	iverilog vsys.v test58r0.v $(CVLIBS)
	./a.out > test58r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test58r0.rtl-spool expecteds/test58r0.ok.out

test58r0.isim:test58r0.v
	iverilog vsys.v test58r0.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test58r0.goldspool:test58r0.exe
	MONO_PATH=$(KDISTRO)/support mono test58r0.exe

#---
# r1 - using kickoff-libraries autoload.
test58r1.exe:test58r1.cs
	$(CSC) /r:$(KLIB) test58r1.cs  



test58r1b:test58r1.exe
	./runtest "test58r1" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
#  -kiwic-kcode-dump=enable
	iverilog vsys.v test58r1.v $(CVLIBS)
	./a.out > test58r1r1.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test58r1r1.rtl-spool expecteds/test58r1r1.ok.out

test58r1.isim:test58r1.v
	iverilog vsys.v test58r1.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test58r1.goldspool:test58r1.exe
	MONO_PATH=$(KDISTRO)/support mono test58r1.exe

#----------------------
# Test 59 Structures With Layout and Setters/Getters
.PHONY: test59a test59b 

test59:test59a  test59b

test59.exe:test59.cs
	$(CSC) /r:$(KLIB) test59.cs  

test59a:test59.exe
	./runtest "test59" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable "  "gldsim"

test59b:test59.exe
	./runtest "test59" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test59.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
#  
	iverilog vsys.v test59.v $(CVLIBS)
	./a.out > test59r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test59r0.rtl-spool expecteds/test59r0.ok.out

test59.isim:test59.v
	iverilog vsys.v test59.v $(CV_INT_ARITH_LIB)   $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test59.goldspool:test59.exe
	MONO_PATH=$(KDISTRO)/support mono test59.exe

#----------------------
#  Test60 : Passing an array ... 
.PHONY: test60a test60b 

test60:test60a  test60b

test60.exe:test60.cs
	$(CSC) /r:$(KLIB) test60.cs  

test60a:test60.exe
	./runtest "test60" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable "  "gldsim"

test60b:test60.exe
	./runtest "test60" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test60.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock " "gldsim"
# 
	iverilog vsys.v test60.v $(CVLIBS)
	./a.out > test60r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test60r0.rtl-spool expecteds/test60r0.ok.out

test60.isim:test60.v
	iverilog vsys.v test60.v $(CV_INT_ARITH_LIB)   $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test60.goldspool:test60.exe
	MONO_PATH=$(KDISTRO)/support mono test60.exe



#----------------------
#  Test61 : Bit Tally
.PHONY: test61a test61b 

test61:test61b

test61.exe:test61.cs
	$(CSC) /r:$(KLIB) test61.cs  

test61a:test61.exe
	./runtest "test61" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test61b:test61.exe
	./runtest "test61" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test61.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
# 
	iverilog vsys.v test61.v $(CVLIBS)
	./a.out > test61r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test61r0.rtl-spool expecteds/test61r0.ok.out

test61.isim:test61.v
	iverilog vsys.v test61.v   $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test61.goldspool:test61.exe
	MONO_PATH=$(KDISTRO)/support mono test61.exe


#----------------------
#  Test62: Example from MSDN web page about C# 'as' and 'is' operators
# Uses isinst and  castclass instructions

.PHONY: test62a test62b 

test62:test62b

test62.exe:test62.cs
	$(CSC) /r:$(KLIB) test62.cs  

test62a:test62.exe
	./runtest "test62" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test62b:test62.exe
	./runtest "test62" "" " -sim=1800  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -vnl=test62.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
# 
	iverilog vsys.v test62.v $(CVLIBS)
	./a.out > test62r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test62r0.rtl-spool expecteds/test62r0.ok.out

test62.isim:test62.v
	iverilog vsys.v test62.v $(CV_INT_ARITH_LIB)   $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test62.goldspool:test62.exe
	MONO_PATH=$(KDISTRO)/support mono test62.exe

#----------------------
#  Test65: Constant scalarised ROMs etc
#  test65.cs  - little ROMs mapped to Portless Register File 
.PHONY: test65a test65b 

test65:test65b

test65.exe:test65.cs
	$(CSC) /r:$(KLIB) test65.cs  

test65a:test65.exe
	./runtest "test65" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test65b:test65.exe
	./runtest "test65" "" "  -sim=1800   -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous   -vnl=test65.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"

#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable

#	./runtest "test65" "" " -sim=1800  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwife-overloads-loglevel=0  -vnl=test65.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
	iverilog vsys.v test65.v $(CVLIBS)
	./a.out > test65r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test65r0.rtl-spool expecteds/test65r0.ok.out

test65.isim:test65.v
	iverilog vsys.v test65.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test65.goldspool:test65.exe
	MONO_PATH=$(KDISTRO)/support mono test65.exe

#----------------------
#  Test66: RTL Parameters
.PHONY: test66a test66b 

test66:test66b

test66.exe:test66.cs
	$(CSC) /r:$(KLIB) test66.cs  

test66a:test66.exe
	./runtest "test66" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test66b:test66.exe
	./runtest "test66" "" " -sim=1800  -kiwic-cil-dump=combined  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -vnl=test66.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"


# 
	iverilog vsys.v test66.v $(CVLIBS)
	./a.out > test66r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test66r0.rtl-spool expecteds/test66r0.ok.out

test66.isim:test66.v
	iverilog vsys.v test66.v $(CVLIBS)
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test66.goldspool:test66.exe
	MONO_PATH=$(KDISTRO)/support mono test66.exe

#----------------------

# 65 Rtl Parameters - merge needed

#----------------------
#  Test67: sine/cos/tan/sqrt/log/exp
.PHONY: test67a test67b test67c

test67:test67killer test67c

test67killer:
	rm -f Test67c.v KiwiFpSqRoot.v KiwiFpSineCosine.v

test67.exe:test67a.cs
	$(CSC) /r:$(KLIB) test67a.cs  

# a and b are missing?
test67a:test67.exe
	./runtest "test67" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test67b:test67.exe
	./runtest "test67" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -vnl=test67.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable
	iverilog vsys.v test67.v $(CVLIBS)
	./a.out > test67r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test67r0.rtl-spool expecteds/test67r0.ok.out
#------
#-- c

Test67c.exe: Test67c.cs $(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll
	$(CSC) /r:$(KLIB) /r:$(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll Test67c.cs


$(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll:$(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.cs
	$(CSC) /r:$(KLIB) /target:library $(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.cs



KiwiFpSqRoot.v:$(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll
	$(KIWIC)  -bevelab-default-pause-mode=soft -bevelab-soft-pause-threshold=220 \
	-cfg-plot-each-step \
	-bevelab-loglevel=8 \
	-bevelab-recode-pc=disable \
        -kiwife-directorate-pc-export=disable \
        -bondout-loadstore-port-count=0 \
	-kiwife-directorate-style=minimal \
	-obj-dir-name=. -log-dir-name=d_obj_KiwiFpSqRoot \
	$(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll



Test67c.v: Test67c.exe $(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll
	$(KIWIC) -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=110 \
		Test67c.exe $(KICKOFF_LIBRARIES)/KiwiFpSqRoot/KiwiFpSqRoot.dll

test67c:test67killer test67c.isim

test67c.isim:KiwiFpSqRoot.v Test67c.v
	iverilog Test67c.v KiwiFpSqRoot.v $(CVLIBS) vsys.v 
	./a.out > test67c.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test67c.rtl-spool expecteds/test67c.ok.out


test67c.goldspool:test67c.exe
	MONO_PATH=$(KDISTRO)/support mono test67c.exe

#------
#-- d

Test67d.exe: Test67d.cs $(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll
	$(CSC) /r:$(KLIB) /r:$(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll Test67d.cs


$(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll:$(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.cs
	$(CSC) /r:$(KLIB) /target:library $(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.cs



KiwiFpSineCosine.v:$(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll
	$(KIWIC)  -bevelab-default-pause-mode=soft -bevelab-soft-pause-threshold=220 \
	-cfg-plot-each-step \
	-max-no-fp-muls=2 \
	-max-no-fp-addsubs=2 \
	-bevelab-loglevel=8 \
	-bevelab-recode-pc=disable \
	-kiwife-directorate-pc-export=disable \
        -bondout-loadstore-port-count=0 \
	-kiwife-directorate-style=minimal \
	-obj-dir-name=. -log-dir-name=d_obj_KiwiFpSineCosine \
	$(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll



Test67d.v: Test67d.exe $(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll
	$(KIWIC) -vnl-rootmodname=DUT -vnl-resets=synchronous -bevelab-default-pause-mode=bblock -bevelab-soft-pause-threshold=110 \
		Test67d.exe $(KICKOFF_LIBRARIES)/KiwiFpSineCosine/KiwiFpSineCosine.dll

test67d:test67killer test67d.isim

test67d.isim:KiwiFpSineCosine.v Test67d.v
	iverilog Test67d.v KiwiFpSineCosine.v $(CVLIBS) vsys.v 
	./a.out | tee test67d.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test67d.rtl-spool expecteds/test67d.ok.out


test67d.goldspool:Test67d.exe
	MONO_PATH=$(KDISTRO)/support:$(KICKOFF_LIBRARIES)/KiwiFpSineCosine mono Test67d.exe | tee test67d.goldspool


#----------------------
#  Test68: swapper method generics
.PHONY: test68a test68b 

test68:test68b

test68.exe:test68.cs
	$(CSC) /r:$(KLIB) test68.cs  

test68a:test68.exe
	./runtest "test68" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test68b:test68.exe
	./runtest "test68" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test68.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable
	iverilog vsys.v test68.v $(CVLIBS)
	./a.out > test68r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test68r0.rtl-spool expecteds/test68r0.ok.out

test68.isim:test68.v
	iverilog vsys.v test68.v $(CVLIBS) $(HPRLS)/kiwipro/kiwic/src/tinytests/System_Math_Sin.v
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test68.goldspool:test68.exe
	MONO_PATH=$(KDISTRO)/support mono test68.exe

#----------------------
# Test69: parent constructor calls
.PHONY: test69a test69b 

test69:test69b

test69.exe:test69.cs
	$(CSC) /r:$(KLIB) test69.cs  

test69a:test69.exe
	./runtest "test69" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test69b:test69.exe
	./runtest "test69" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test69.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable
	iverilog vsys.v test69.v $(CVLIBS)
	./a.out > test69r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test69r0.rtl-spool expecteds/test69r0.ok.out

test69.isim:test69.v
	iverilog vsys.v test69.v $(CVLIBS) $(HPRLS)/kiwipro/kiwic/src/tinytests/System_Math_Sin.v
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test69.goldspool:test69.exe
	MONO_PATH=$(KDISTRO)/support mono test69.exe

#----------------------
# Test70: exceptions
.PHONY: test70a test70b 

test70:test70a

test70.exe:test70.cs
	$(CSC) /r:$(KLIB) test70.cs  

test70a:test70.exe
	./runtest "test70" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable"  "gldsim"

test70b:test70.exe
	./runtest "test70" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test70.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable
	iverilog vsys.v test70.v $(CVLIBS)
	./a.out > test70r0.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test70r0.rtl-spool expecteds/test70r0.ok.out

test70.isim:test70.v
	iverilog vsys.v test70.v $(CVLIBS) $(HPRLS)/kiwipro/kiwic/src/tinytests/System_Math_Sin.v
	./a.out
	cp vcd.vcd $(VIEWINGDEST)

test70.goldspool:test70.exe
	MONO_PATH=$(KDISTRO)/support mono test70.exe

#----------------------
# Test71: Further exceptions
# FinallySimple71r1 - complex finally blocks - no catching
# FinallySimple71r2 - complex finally blocks - with catching  - missing at the moment

.PHONY: test71r1a test71r1b test71r2b

test71:test71r0a test71r1b

test71r0.exe:test71r0.cs
	$(CSC) /r:$(KLIB) test71r0.cs  

test71r0a:test71r0.exe
	./runtest "test71r0" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable -kiwic-cil-dump=combined  -kiwic-kcode-dump=early "  "gldsim"


test71r1:test71r1a test71r1b

test71r0.goldspool:test71r0.exe
	MONO_PATH=$(KDISTRO)/support mono test71r0.exe

test71r1.exe:test71r1.cs
	$(CSC) /r:$(KLIB) test71r1.cs  

test71r1a:test71r1.exe
	./runtest "test71r1" "" " -sim=1800  -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous  -bevelab=disable -verilog-gen=disable -repack=disable -conerefine=disable -restructure2=disable  -kiwic-cil-dump=combined  -kiwic-kcode-dump=early "  "gldsim"

test71r1b:test71r1.exe
	./runtest "test71r1" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test71r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard   -kiwic-cil-dump=combined  -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 " "gldsim"
#  -bevelab-loglevel=0
	iverilog vsys.v test71r1.v $(CVLIBS)
	./a.out > test71r1.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test71r1.rtl-spool expecteds/test71r1.ok.out

test71r1.goldspool:test71r1.exe
	MONO_PATH=$(KDISTRO)/support mono test71r1.exe


test71r2.exe:test71r2.cs
	$(CSC) /r:$(KLIB) test71r2.cs  


test71r2b:test71r2.exe
	./runtest "test71r2" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test71r2.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard " "gldsim"
#  -kiwic-cil-dump=combined  -kiwic-kcode-dump=enable
	iverilog vsys.v test71r2.v $(CVLIBS)
	./a.out > test71r2.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test71r2.rtl-spool expecteds/test71r2.ok.out


test71r2.goldspool:test71r2.exe
	MONO_PATH=$(KDISTRO)/support mono test71r2.exe

#----------------------


modelsim0:
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib DUT.v vsys.v
	vsim -c simple_lib.SIMSYS -do "run.do" 

# This is suitable for the offchip version of primseya.cs  and other designs with one port.
modelsim1:
	rm -rf simple_lib; vlib simple_lib
	vmap simple_lib simple_lib
	vlog -work simple_lib DUTH1.v hsimple1.v
	vsim -c simple_lib.DUTH1 -do "run.do" 


#=================================================================
# Test72 Virtex Native FIFOs

test72: test72r1 
# test72r2
# missing ... just being added ... test72r2 test72r3

test72r1.exe:test72r1.cs
	$(CSC) /r:$(KLIB) test72r1.cs  


test72r1:test72r1.exe
	./runtest "test72r1" "" "  -sim=1800   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test72r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard   -kiwic-cil-dump=combined  -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 " "gldsim"
#  -bevelab-loglevel=0
	iverilog vsys.v test72r1.v $(CVLIBS)
	./a.out > test72r1.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test72r1.rtl-spool expecteds/test72r1.ok.out

test72r1.isim:
	iverilog vsys.v test72r1.v $(CVLIBS)
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test72r1.goldspool:test72r1.exe
	MONO_PATH=$(KDISTRO)/support mono test72r1.exe

test72r2.exe:test72r2.cs
	$(CSC) /r:$(KLIB) test72r2.cs  


test72r2:test72r2.exe
	./runtest "test72r2" "" "  -sim=1800 -kiwife-firstpass-loglevel=0   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test72r2.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard   -kiwic-cil-dump=combined  -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 " "gldsim"
#  -bevelab-loglevel=0
	iverilog vsys.v test72r2.v $(CVLIBS)
	./a.out > test72r2.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test72r2.rtl-spool expecteds/test72r2.ok.out

test72r2.isim:
	iverilog vsys.v test72r2.v $(CVLIBS)
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test72r2.goldspool:test72r2.exe
	MONO_PATH=$(KDISTRO)/support mono test72r2.exe

#=================================================================
# Test73 native atomics such as Interlocked.Add and Interlocked.Exchange

test73: test73r1 
# test73r1


test73r1.exe:test73r1.cs
	$(CSC) /r:$(KLIB) test73r1.cs  


test73r1:test73r1.exe
	./runtest "test73r1" "" "  -sim=1800   -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test73r1.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwife-hgen-loglevel=10  -kiwife-constvol-loglevel=10 -bevelab-loglevel=0 -res2-loglevel=10  -kiwife-gtrace-loglevel=10 -kiwic-cil-dump=combined  -kiwic-kcode-dump=early  " "gldsim"
#
	iverilog vsys.v test73r1.v $(CVLIBS)
	./a.out > test73r1.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test73r1.rtl-spool expecteds/test73r1.ok.out

test73r1.isim:
	iverilog vsys.v test73r1.v $(CVLIBS) ../src/tinytests/HPR_ATOMIC_ALU.v
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test73r1.goldspool:test73r1.exe
	MONO_PATH=$(KDISTRO)/support mono test73r1.exe

test73r2.exe:test73r2.cs
	$(CSC) /r:$(KLIB) test73r2.cs  


test73r2:test73r2.exe
	./runtest "test73r2" "" "  -sim=1800 -kiwife-firstpass-loglevel=0   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test73r2.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard   -kiwic-cil-dump=combined  -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 " "gldsim"
#  -bevelab-loglevel=0
	iverilog vsys.v test73r2.v $(CVLIBS)
	./a.out > test73r2.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test73r2.rtl-spool expecteds/test73r2.ok.out

test73r2.isim:
	iverilog vsys.v test73r2.v $(CVLIBS)
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test73r2.goldspool:test73r2.exe
	MONO_PATH=$(KDISTRO)/support mono test73r2.exe

#=================================================================
# Test74 clock domains

test74: test74 
# test74r2 not in use


test74.exe:test74.cs
	$(CSC) /r:$(KLIB) test74.cs  


test74:test74.exe
	./runtest "test74" "" "  -sim=1800   -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test74.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwife-hgen-loglevel=10  -kiwife-constvol-loglevel=10 -bevelab-loglevel=0 -res2-loglevel=10  -kiwife-gtrace-loglevel=10 -kiwic-cil-dump=combined  -kiwic-kcode-dump=early  " "gldsim"
#
	iverilog vsys74.v test74.v $(CVLIBS)
	./a.out > test74.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test74.rtl-spool expecteds/test74.ok.out

test74.isim:
	iverilog vsys.v test74.v $(CVLIBS) ../src/tinytests/HPR_ATOMIC_ALU.v
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test74.goldspool:test74.exe
	MONO_PATH=$(KDISTRO)/support mono test74.exe

test74r2.exe:test74r2.cs
	$(CSC) /r:$(KLIB) test74r2.cs  


test74r2:test74r2.exe
	./runtest "test74r2" "" "  -sim=1800 -kiwife-firstpass-loglevel=0   -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined   -vnl=test74r2.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard   -kiwic-cil-dump=combined  -kiwic-kcode-dump=early -kiwife-gtrace-loglevel=0 " "gldsim"
#  -bevelab-loglevel=0
	iverilog vsys.v test74r2.v $(CVLIBS)
	./a.out > test74r2.rtl-spool
	cp vcd.vcd $(VIEWINGDEST)
	diff test74r2.rtl-spool expecteds/test74r2.ok.out

test74r2.isim:
	iverilog vsys.v test74r2.v $(CVLIBS)
	./a.out 
	cp vcd.vcd $(VIEWINGDEST)

test74r2.goldspool:test74r2.exe
	MONO_PATH=$(KDISTRO)/support mono test74r2.exe

#-------------------
# These silent tests do not generate any output that is checked, but at least
# they should quietly compile without abend.

.PHONY: silentrunners silent1
silentrunners:  silent1 silent2

#-------------------------------
#
silent1:silent1compile

EthernetEcho.exe:EthernetEcho.cs  $(KLIB)
	$(CSC) EthernetEcho.cs  /r:$(KLIB) 

silent1compile: EthernetEcho.exe
	$(KIWIC) EthernetEcho.exe

#----------------------
loopclassic_test: loopclassic_killer loopclassic_veril_run

loopclassic.exe:loopclassic.cs $(KLIB)
	$(CSC) loopclassic.cs  /r:$(KLIB) 

loopclassic.v:loopclassic.exe
	time $(KIWIC) -obj-dir-name=. -cfg-plot-each-step   -kiwic-kcode-dump=enable -log-dir-name=d_obj_loopclassic_b -kiwic-autodispose=enable loopclassic.exe -vnl=loopclassic -vnl-rootmodname=DUT -vnl-resets=synchronous      -repack-loglevel=0 | tee loopclassic_b.spool

loopclassic_killer:
	rm -fr loopclassic.v loopclassic_verilated

# RTLSIM Verilator Run
loopclassic_veril_run:loopclassic_verilated/VVERI_SIMSYS.cpp veriltop.cpp loopclassic_verilated/Vloopclassic
	g++ -g -O2 -Iloopclassic_verilated -I$(VERILATOR_ROOT)/include veriltop.cpp loopclassic_verilated/VVERI_BENCHER.cpp $(VERILATOR_ROOT)/include/verilated.cpp loopclassic_verilated/VVERI_BENCHER__Syms.cpp
	time ./a.out | tee loopclassic_veril_out
	grep swaps loopclassic_veril_out > loopclassic_out1
	tail -8 loopclassic.rpt
	diff loopclassic_out1 expecteds/loopclassic_expected_swaps

loopclassic_verilated/VVERI_SIMSYS.cpp loopclassic_verilated/Vloopclassic: loopclassic.v veri_bencher.v
	verilator -cc -Mdir loopclassic_verilated  -Wno-fatal --top-module VERI_BENCHER loopclassic.v  veri_bencher.v $(CV_INT_ARITH_LIB)


loopclassic.monorun: loopclassic.exe
	MONO_PATH=$(KDISTRO)/support:. mono loopclassic.exe


#----------------------
# Smith-Waterman : Serial - simple version with canned data to run on.
#
#
#
sw_test.exe:sw_test.cs $(KLIB)
	$(CSC) sw_test.cs  /r:$(KLIB) 

silent2:sw_test

.PHONY: sw_test_a1 sw_test_a2 sw_test_b

sw_test: sw_test_a1 sw_test_b


sw_test_a1:sw_test.exe
	time $(KIWIC)   -kiwife-gtrace-loglevel=0  -log-dir-name=d_obj_sw_test_a1  -repack=disable -restructure2=disable -bevelab=disable -conerefine=disable sw_test.exe -verilog-gen=disable -sim 100000 -vnl-rootmodname=DUT -diosim-spool=diosim_sw_test_a1.out > sw_test_a1.spool
	mv sw_test.rpt d_obj_sw_test_a1
	diff diosim_sw_test_a1.out expecteds/sw_test_r1.gldsim
#	grep Linear d_obj_sw_test_a1/h02_kiwife/s00.log.txt

# gold gives +ve values here: self test startup 2985771188 c 
# we get -ve
sw_test_a2:sw_test.exe
	time $(KIWIC)  -restructure2=disable  -log-dir-name=d_obj_sw_test_a2 sw_test.exe   -vnl=sw_test_a2 -sim 100000 -vnl-rootmodname=DUT -vnl-resets=synchronous  -diosim-spool=diosim_sw_test_a2.out  > sw_test_a2.spool
	mv sw_test.rpt d_obj_sw_test_a2
# 
sw_test_b:sw_test.exe
	time $(KIWIC)    -obj-dir-name=. -res2-loglevel=0 -log-dir-name=d_obj_sw_test_b sw_test.exe -vnl=sw_test -vnl-rootmodname=DUT -vnl-resets=synchronous  > sw_test_b.spool
# -diosim-spool=diosim_sw_test_b.out
# 
sw_test.goldspool:sw_test.exe
	MONO_PATH=$(KDISTRO)/support mono ./sw_test.exe 

sw_test_a1.isim:sw_test_a1.v
	iverilog vsys.v sw_test_a1.v $(CV_INT_ARITH_LIB)
	./a.out > sw_test_a1.rtl-spool

sw_test.isim:sw_test.v
	iverilog vsys.v sw_test.v $(CV_INT_ARITH_LIB)
	./a.out > sw_test.out

#----------------------
# TinyTot
tinytot:	
	$(CSC) /r:$(KLIB)  tinytot.cs
	$(KIWIC) tinytot.exe

#-------------------
# 
#
SYSCSHARP ?= $(KDISTRO)/SystemCsharp/SystemCsharp.dll
dut-sysc:
	$(CSC) DUT.sysc.cs /r:$(SYSCSHARP)
	MONO_PATH=$(SYSCSHARP) $(MONO) DUT.csc.exe

#-------------------
minitest:
	cs -clearnet -nox vsys.v VNL.v
#-------------------------------------
Orangelib.exe:Orangelib.cs
	$(CSC) /target:library Orangelib.cs

#---------------------------


#---------------------------

# Dot output can be viewed with 'xdot' 'eog' or just 'display'

dot:
	dot -Tpng controlflow_combined.dot > $(VIEWINGDEST)/a.png



.PHONY: completion_message start_timestamp

start_timestamp:
	date
	echo "Regression test run commence"

completion_message:
	date
	echo "Regression test run complete"

zip:
	zip kiwic-regression.zip *.cs Makefile *.v expecteds/*


# eof

