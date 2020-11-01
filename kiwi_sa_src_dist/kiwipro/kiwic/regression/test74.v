

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:49:53
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test74.exe -sim=1800 -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test74.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwife-hgen-loglevel=10 -kiwife-constvol-loglevel=10 -bevelab-loglevel=0 -res2-loglevel=10 -kiwife-gtrace-loglevel=10 -kiwic-cil-dump=combined -kiwic-kcode-dump=early -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input my_clock,
    input my_reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiTESTMAIN4001PC10nz_pc_export);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test74_Main_V_0;
// abstractionName=res2-morenets
  reg [2:0] kiwiTESTMAIN4001PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge my_clock )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400_1/1.0
      if (my_reset)  begin 
               kiwiTESTMAIN4001PC10nz <= 32'd0;
               TESTMAIN400_test74_Main_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN4001PC10nz)
                  32'h0/*0:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("%s%1d", "Test 1 Limit=", 32'sd10);
                           kiwiTESTMAIN4001PC10nz <= 32'h1/*1:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test74_Main_V_0 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $write("%1d%s", 32'sh1, " ");
                           kiwiTESTMAIN4001PC10nz <= 32'h2/*2:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test74_Main_V_0 <= 32'sh1;
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((32'sd10<$signed(32'sd2+TESTMAIN400_test74_Main_V_0))) $display(" Test 74 finished.");
                               else $write("%1d%s", $signed(32'sd2+TESTMAIN400_test74_Main_V_0), " ");
                          if ((32'sd10<$signed(32'sd2+TESTMAIN400_test74_Main_V_0)))  begin 
                                   kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                                   TESTMAIN400_test74_Main_V_0 <= $signed(32'sd2+TESTMAIN400_test74_Main_V_0);
                                   end 
                                   else  TESTMAIN400_test74_Main_V_0 <= $signed(32'sd2+TESTMAIN400_test74_Main_V_0);
                           end 
                          
                  32'h3/*3:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           kiwiTESTMAIN4001PC10nz <= 32'h4/*4:kiwiTESTMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (my_reset)  kiwiTESTMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN4001PC10nz_pc_export <= kiwiTESTMAIN4001PC10nz;
              if (my_reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400_1/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 3
// 1 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 36 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:49:51
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test74.exe -sim=1800 -vnl-rootmodname=DUT -vnl-resets=synchronous -kiwic-cil-dump=combined -vnl=test74.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=hard -kiwife-hgen-loglevel=10 -kiwife-constvol-loglevel=10 -bevelab-loglevel=0 -res2-loglevel=10 -kiwife-gtrace-loglevel=10 -kiwic-cil-dump=combined -kiwic-kcode-dump=early -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test74 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test74.Main | Main10 | false |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*

//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from kiwife:::
//Enumeration codepoints for KiwiSystem.Kiwi.PauseControl
//*----------------------+------------+---*
//| Token                | Code point | p |
//*----------------------+------------+---*
//| autoPauseEnable      | 0          | 6 |
//| hardPauseEnable      | 1          | 6 |
//| softPauseEnable      | 2          | 6 |
//| maximalPauseEnable   | 3          | 6 |
//| bblockPauseEnable    | 4          | 6 |
//| pipelinedAccelerator | 5          | 6 |
//*----------------------+------------+---*

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//Bondout Port Settings
//
//
//*----------------------------------+-------+-------------*
//
//
//| Key                              | Value | Description |
//
//
//*----------------------------------+-------+-------------*
//
//
//| bondout-loadstore-port-count     | 0     |             |
//
//
//| bondout-loadstore-lane_addr-size | 22    |             |
//
//
//*----------------------------------+-------+-------------*
//
//
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class test74  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test74..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test74..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test74  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test74.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test74.Main
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//Report of all settings used from the recipe or command line:
//
//
//   bondout-schema=bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8
//
//
//   bondout-protocol=HFAST1
//
//
//   bondout-loadstore-lane-width=8
//
//
//   bondout-loadstore-port-lanes=32
//
//
//   bondout-loadstore-port-count=0
//
//
//   bondout-loadstore-simplex-ports=disable
//
//
//   bondout-loadstore-lane-addr-size=22
//
//
//   kiwife-directorate-pc-export=enable
//
//
//   kiwife-directorate-endmode=finish
//
//
//   kiwife-directorate-startmode=self-start
//
//
//   kiwic-default-dynamic-heapalloc-bytes=1073741824
//
//
//   cil-uwind-budget=10000
//
//
//   kiwic-cil-dump=combined
//
//
//   kiwic-kcode-dump=early
//
//
//   kiwic-supress-zero-inits=disable
//
//
//   kiwife-dynpoly=disable
//
//
//   kiwic-library-redirects=enable
//
//
//   kiwic-register-colours=disable
//
//
//   array-4d-name=KIWIARRAY4D
//
//
//   array-3d-name=KIWIARRAY3D
//
//
//   array-2d-name=KIWIARRAY2D
//
//
//   kiwi-dll=Kiwi.dll
//
//
//   kiwic-dll=Kiwic.dll
//
//
//   kiwic-zerolength-arrays=disable
//
//
//   kiwifefpgaconsole-default=enable
//
//
//   kiwife-directorate-style=normal
//
//
//   kiwife-postgen-optimise=enable
//
//
//   kiwife-allow-hpr-alloc=enable
//
//
//   kiwife-filesearch-loglevel=3
//
//
//   kiwife-cil-loglevel=3
//
//
//   kiwife-ataken-loglevel=3
//
//
//   kiwife-gtrace-loglevel=10
//
//
//   kiwife-constvol-loglevel=10
//
//
//   kiwife-hgen-loglevel=10
//
//
//   kiwife-firstpass-loglevel=3
//
//
//   kiwife-overloads-loglevel=3
//
//
//   root=$attributeroot
//
//
//   srcfile=test74.exe
//
//
//   kiwic-autodispose=disable
//
//
//END OF KIWIC REPORT FILE
//
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| Key                    | Value   | Description                                                                                                |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*
//| int-flr-mul            | 1000    | Fixed-latency integer ALU integer latency scaling value for multiply.                                      |
//| max-no-fp-addsubs      | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread.                            |
//| max-no-fp-muls         | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.                                   |
//| max-no-int-muls        | 3       | Maximum number of int multipliers to instantiate per thread.                                               |
//| max-no-fp-divs         | 2       | Maximum number of f/p dividers to instantiate per thread.                                                  |
//| max-no-int-divs        | 2       | Maximum number of int dividers to instantiate per thread.                                                  |
//| max-no-rom-mirrors     | 8       | Maximum number of times to mirror a ROM per thread.                                                        |
//| max-ram-data_packing   | 8       | Maximum number of user words to pack into one RAM/loadstore word line.                                     |
//| fp-fl-dp-div           | 5       | Fixed-latency ALU floating-point, double-precision floating-point latency value for divide.                |
//| fp-fl-dp-add           | 4       | Fixed-latency ALU floating-point, double-precision floating-point latency value for add/sub.               |
//| fp-fl-dp-mul           | 3       | Fixed-latency ALU floating-point, double-precision floating-point latency value for multiply.              |
//| fp-fl-sp-div           | 15      | Fixed-latency ALU floating-point, single-precision floating-point floating-point latency value for divide. |
//| fp-fl-sp-add           | 4       | Fixed-latency ALU floating-point, single-precision floating-point latency value for add/sub.               |
//| fp-fl-sp-mul           | 5       | Fixed-latency ALU floating-point, single-precision floating-point latency value for multiply.              |
//| res2-offchip-threshold | 1000000 |                                                                                                            |
//| res2-combrom-threshold | 64      |                                                                                                            |
//| res2-combram-threshold | 32      |                                                                                                            |
//| res2-regfile-threshold | 8       |                                                                                                            |
//*------------------------+---------+------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN4001PC10 
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                   | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN4001PC10" | 816 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiTESTMAIN4001PC10" | 815 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN4001PC10" | 813 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN4001PC10" | 814 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiTESTMAIN4001PC10" | 812 | 3       | hwm=0.0.0   | 3    |        | -     | -   | -    |
//*---------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10" 816 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                       |
//*------+------+---------+------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                            |
//| F0   | E816 | R0 DATA |                                                            |
//| F0+E | E816 | W0 DATA | TESTMAIN400.test74.Main.V_0write(S32'0)  PLI:Test 1 Limit= |
//*------+------+---------+------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10" 815 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                           |
//*------+------+---------+------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                |
//| F1   | E815 | R0 DATA |                                                |
//| F1+E | E815 | W0 DATA | TESTMAIN400.test74.Main.V_0write(S32'1)  PLI:  |
//*------+------+---------+------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10" 813 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//  Absolute key numbers for scheduled edge res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10" 814 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//*------+------+---------+--------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                         |
//*------+------+---------+--------------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                              |
//| F2   | E814 | R0 DATA |                                                              |
//| F2+E | E814 | W0 DATA | TESTMAIN400.test74.Main.V_0write(E1)  PLI: Test 74 finished. |
//| F2   | E813 | R0 DATA |                                                              |
//| F2+E | E813 | W0 DATA | TESTMAIN400.test74.Main.V_0write(E1)  PLI:                   |
//*------+------+---------+--------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10" 812 :  major_start_pcl=3   edge_private_start/end=-1/-1 exec=3 (dend=0)
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//*------+------+---------+-----------------------*
//| pc   | eno  | Phaser  | Work                  |
//*------+------+---------+-----------------------*
//| F3   | -    | R0 CTRL |                       |
//| F3   | E812 | R0 DATA |                       |
//| F3+E | E812 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(2+TESTMAIN400.test74.Main.V_0)
//
//
//  E2 =.= 10<(C(2+TESTMAIN400.test74.Main.V_0))
//
//
//  E3 =.= 10>=(C(2+TESTMAIN400.test74.Main.V_0))
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test74 to test74

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 3
//
//1 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 36 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test74..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test74.Main uid=Main10 has 14 CIL instructions in 7 basic blocks
//Thread mpc10 has 4 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN4001PC10 with 4 minor control states
// eof (HPR L/S Verilog)
