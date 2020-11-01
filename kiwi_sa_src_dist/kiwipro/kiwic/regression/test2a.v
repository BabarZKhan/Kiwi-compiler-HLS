

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:42:42
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test2a.exe -sim 1178 -bondout-loadstore-port-count=0 -diosim-vcd=test2a.vcd -give-backtrace -report-each-step
`timescale 1ns/1ns


module test2a(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets12 */
    output [7:0] hpr_unary_leds_DDX20,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX18,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiTSX481PC10nz_pc_export,
    output reg [2:0] kiwiTESTMAIN4001PC10nz_pc_export);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets12
  reg hpr_int_run_enable_DDX20;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX18;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TSX481_ConsumerClass_process_V_0;
  reg signed [31:0] TESTMAIN400_test2a_Main_V_1;
  reg signed [31:0] TESTMAIN400_test2a_Main_V_0;
  reg signed [31:0] ConsumerClass_shvar;
// abstractionName=res2-morenets
  reg [2:0] kiwiTESTMAIN4001PC10nz;
  reg [2:0] kiwiTSX481PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX20 = 32'sd1;
       hpr_int_run_enable_DDX20 = (32'sd255==hpr_abend_syndrome);
       hpr_int_run_enable_DDX18 = 32'sd1;
       hpr_int_run_enable_DDX18 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TSX481/1.0
      if (reset)  begin 
               ConsumerClass_shvar <= 32'd0;
               kiwiTSX481PC10nz <= 32'd0;
               TSX481_ConsumerClass_process_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX20) 
              case (kiwiTSX481PC10nz)
                  32'h0/*0:kiwiTSX481PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                           kiwiTSX481PC10nz <= 32'h2/*2:kiwiTSX481PC10nz*/;
                           TSX481_ConsumerClass_process_V_0 <= 32'sh0;
                           end 
                          
                  32'h1/*1:kiwiTSX481PC10nz*/: if (hpr_int_run_enable_DDX20)  kiwiTSX481PC10nz <= 32'h3/*3:kiwiTSX481PC10nz*/;
                      
                  32'h2/*2:kiwiTSX481PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          if ((32'sd0<ConsumerClass_shvar))  begin 
                                  $display("Decremented %1d", $signed(-32'sd1+ConsumerClass_shvar));
                                   ConsumerClass_shvar <= $signed(-32'sd1+ConsumerClass_shvar);
                                   end 
                                   kiwiTSX481PC10nz <= 32'h1/*1:kiwiTSX481PC10nz*/;
                           end 
                          
                  32'h3/*3:kiwiTSX481PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          if (($signed(32'sd1+TSX481_ConsumerClass_process_V_0)>=32'sd100)) $finish(32'sd0);
                               else  begin 
                                   kiwiTSX481PC10nz <= 32'h2/*2:kiwiTSX481PC10nz*/;
                                   TSX481_ConsumerClass_process_V_0 <= $signed(32'sd1+TSX481_ConsumerClass_process_V_0);
                                   end 
                                  if (($signed(32'sd1+TSX481_ConsumerClass_process_V_0)>=32'sd100))  begin 
                                   kiwiTSX481PC10nz <= 32'h4/*4:kiwiTSX481PC10nz*/;
                                   TSX481_ConsumerClass_process_V_0 <= $signed(32'sd1+TSX481_ConsumerClass_process_V_0);
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          endcase
              if (reset)  kiwiTSX481PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX20)  kiwiTSX481PC10nz_pc_export <= kiwiTSX481PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TSX481/1.0


      //Start structure cvtToVerilogkiwi.TESTMAIN400_1/1.0
      if (reset)  begin 
               kiwiTESTMAIN4001PC10nz <= 32'd0;
               TESTMAIN400_test2a_Main_V_0 <= 32'd0;
               TESTMAIN400_test2a_Main_V_1 <= 32'd0;
               ConsumerClass_shvar <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX18) 
              case (kiwiTESTMAIN4001PC10nz)
                  32'h0/*0:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("%s%1d", "Test2a: limit=", 32'sd6);
                          $display("Note: this test is very fragile owing to races and has completely non-determinstic output on mono - but in hard pause mode the RTL behaviour is stable enough to warrant regression testing."
                          );
                           kiwiTESTMAIN4001PC10nz <= 32'h2/*2:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test2a_Main_V_0 <= 32'sh1;
                           TESTMAIN400_test2a_Main_V_1 <= 32'sh1;
                           ConsumerClass_shvar <= 32'sh1;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                           kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                           TESTMAIN400_test2a_Main_V_1 <= $signed(32'sd1+TESTMAIN400_test2a_Main_V_1);
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  kiwiTESTMAIN4001PC10nz <= 32'h1/*1:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((32'sd10<TESTMAIN400_test2a_Main_V_1))  kiwiTESTMAIN4001PC10nz
                           <= 32'h4/*4:kiwiTESTMAIN4001PC10nz*/;

                           else  kiwiTESTMAIN4001PC10nz <= 32'h2/*2:kiwiTESTMAIN4001PC10nz*/;
                      
                  32'h4/*4:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18) if ((32'sd6<$signed(32'sd1+TESTMAIN400_test2a_Main_V_0
                      )))  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h5/*5:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test2a_Main_V_0 <= $signed(32'sd1+TESTMAIN400_test2a_Main_V_0);
                               ConsumerClass_shvar <= $signed(32'sd5+ConsumerClass_shvar);
                               end 
                               else  begin 
                               kiwiTESTMAIN4001PC10nz <= 32'h3/*3:kiwiTESTMAIN4001PC10nz*/;
                               TESTMAIN400_test2a_Main_V_0 <= $signed(32'sd1+TESTMAIN400_test2a_Main_V_0);
                               TESTMAIN400_test2a_Main_V_1 <= 32'sh1;
                               ConsumerClass_shvar <= $signed(32'sd5+ConsumerClass_shvar);
                               end 
                              
                  32'h5/*5:kiwiTESTMAIN4001PC10nz*/: if (hpr_int_run_enable_DDX18)  begin 
                          $display("\nTest2a: Finished\n");
                          $finish(32'sd0);
                           kiwiTESTMAIN4001PC10nz <= 32'h6/*6:kiwiTESTMAIN4001PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  kiwiTESTMAIN4001PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX18)  kiwiTESTMAIN4001PC10nz_pc_export <= kiwiTESTMAIN4001PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400_1/1.0


       end 
      

// Structural Resource (FU) inventory for test2a:// 2 vectors of width 3
// 4 vectors of width 32
// 2 vectors of width 1
// Total state bits in module = 136 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:42:39
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test2a.exe -sim 1178 -bondout-loadstore-port-count=0 -diosim-vcd=test2a.vcd -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| test2a | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | test2a.Main | Main10 | false |
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
//KiwiC: front end input processing of class ConsumerClass  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=ConsumerClass..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=ConsumerClass..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class KiwiSystem.Kiwi  wonky=KiwiSystem igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor16 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor16 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class test2a  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test2a..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=test2a..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test2a  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test2a.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test2a.Main
//
//
//Logging start thread entry point=CE_region<&(CTL_record(System.Threading.ThreadStart, ...))>(System.Threading.ThreadStart.400016%System.Threading.ThreadStart%400016%24, nemtok=TESTMAIN400/test2a/Main/CZ:0:13/item12, ats={wondarray=true, constant=true}) user_arg_arity=0: USER_THREAD1(CE_conv(CT_cr(System.Object), CS_maskcast, CE_region<&(CTL_record(ConsumerClass, ...))>(ConsumerClass.400000%ConsumerClass%400000%12, nemtok=TESTMAIN400/test2a/Main/CZ:0:9/item10, ats={wondarray=true, constant=true})), CE_conv(CT_cr(System.Object), CS_maskcast, CE_reflection(idl=ConsumerClass.process)), ())
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=process10 full_idl=ConsumerClass.process
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
//   kiwic-cil-dump=disable
//
//
//   kiwic-kcode-dump=disable
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
//   kiwife-gtrace-loglevel=3
//
//
//   kiwife-constvol-loglevel=3
//
//
//   kiwife-hgen-loglevel=3
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
//   srcfile=test2a.exe
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
//PC codings points for kiwiTSX481PC10 
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause             | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTSX481PC10" | 817 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTSX481PC10" | 815 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 3    |
//| XU32'1:"1:kiwiTSX481PC10" | 816 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//| XU32'4:"4:kiwiTSX481PC10" | 813 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 2    |
//| XU32'4:"4:kiwiTSX481PC10" | 814 | 3       | hwm=0.0.0   | 3    |        | -     | -   | -    |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'0:"0:kiwiTSX481PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'0:"0:kiwiTSX481PC10"
//*------+------+---------+----------------------------------------------*
//| pc   | eno  | Phaser  | Work                                         |
//*------+------+---------+----------------------------------------------*
//| F0   | -    | R0 CTRL |                                              |
//| F0   | E817 | R0 DATA |                                              |
//| F0+E | E817 | W0 DATA | TSX481.ConsumerClass.process.V_0write(S32'0) |
//*------+------+---------+----------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'2:"2:kiwiTSX481PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'2:"2:kiwiTSX481PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F1   | -    | R0 CTRL |      |
//| F1   | E815 | R0 DATA |      |
//| F1+E | E815 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'1:"1:kiwiTSX481PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'1:"1:kiwiTSX481PC10"
//*------+------+---------+--------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                             |
//*------+------+---------+--------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                  |
//| F2   | E816 | R0 DATA |                                                  |
//| F2+E | E816 | W0 DATA | ConsumerClass.shvarwrite(E1)  PLI:Decremented %d |
//*------+------+---------+--------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'4:"4:kiwiTSX481PC10"
//res2: scon1: nopipeline: Thread=kiwiTSX481PC10 state=XU32'4:"4:kiwiTSX481PC10"
//*------+------+---------+-----------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                            |
//*------+------+---------+-----------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                 |
//| F3   | E814 | R0 DATA |                                                                 |
//| F3+E | E814 | W0 DATA | TSX481.ConsumerClass.process.V_0write(E2)  PLI:GSAI:hpr_sysexit |
//| F3   | E813 | R0 DATA |                                                                 |
//| F3+E | E813 | W0 DATA | TSX481.ConsumerClass.process.V_0write(E2)                       |
//*------+------+---------+-----------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTMAIN4001PC10 
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                     | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN4001PC10"   | 825 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN4001PC10"   | 823 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 3    |
//| XU32'1:"1:kiwiTESTMAIN4001PC10"   | 824 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//| XU32'4:"4:kiwiTESTMAIN4001PC10"   | 821 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 2    |
//| XU32'4:"4:kiwiTESTMAIN4001PC10"   | 822 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiTESTMAIN4001PC10"   | 819 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 3    |
//| XU32'8:"8:kiwiTESTMAIN4001PC10"   | 820 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiTESTMAIN4001PC10" | 818 | 5       | hwm=0.0.0   | 5    |        | -     | -   | -    |
//*-----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'0:"0:kiwiTESTMAIN4001PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                             |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                                  |
//| F0   | E825 | R0 DATA |                                                                                                                                                                  |
//| F0+E | E825 | W0 DATA | ConsumerClass.shvarwrite(S32'1) TESTMAIN400.test2a.Main.V_1write(S32'1) TESTMAIN400.test2a.Main.V_0write(S32'1)  PLI:Note: this test is v...  PLI:Test2a: limit= |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'2:"2:kiwiTESTMAIN4001PC10"
//*------+------+---------+--------------------------------------*
//| pc   | eno  | Phaser  | Work                                 |
//*------+------+---------+--------------------------------------*
//| F1   | -    | R0 CTRL |                                      |
//| F1   | E823 | R0 DATA |                                      |
//| F1+E | E823 | W0 DATA | TESTMAIN400.test2a.Main.V_1write(E3) |
//*------+------+---------+--------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'1:"1:kiwiTESTMAIN4001PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F2   | -    | R0 CTRL |      |
//| F2   | E824 | R0 DATA |      |
//| F2+E | E824 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'4:"4:kiwiTESTMAIN4001PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F3   | -    | R0 CTRL |      |
//| F3   | E822 | R0 DATA |      |
//| F3+E | E822 | W0 DATA |      |
//| F3   | E821 | R0 DATA |      |
//| F3+E | E821 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'8:"8:kiwiTESTMAIN4001PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                            |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                                 |
//| F4   | E820 | R0 DATA |                                                                                                                                 |
//| F4+E | E820 | W0 DATA | ConsumerClass.shvarwrite(C(5+ConsumerClass.shvar)) TESTMAIN400.test2a.Main.V_0write(E4)                                         |
//| F4   | E819 | R0 DATA |                                                                                                                                 |
//| F4+E | E819 | W0 DATA | ConsumerClass.shvarwrite(C(5+ConsumerClass.shvar)) TESTMAIN400.test2a.Main.V_1write(S32'1) TESTMAIN400.test2a.Main.V_0write(E4) |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN4001PC10 state=XU32'16:"16:kiwiTESTMAIN4001PC10"
//*------+------+---------+-----------------------------------------------*
//| pc   | eno  | Phaser  | Work                                          |
//*------+------+---------+-----------------------------------------------*
//| F5   | -    | R0 CTRL |                                               |
//| F5   | E818 | R0 DATA |                                               |
//| F5+E | E818 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:
//Test2a: Finished
// |
//*------+------+---------+-----------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(-1+ConsumerClass.shvar)
//
//
//  E2 =.= C(1+TSX481.ConsumerClass.process.V_0)
//
//
//  E3 =.= C(1+TESTMAIN400.test2a.Main.V_1)
//
//
//  E4 =.= C(1+TESTMAIN400.test2a.Main.V_0)
//
//
//  E5 =.= (C(1+TSX481.ConsumerClass.process.V_0))>=100
//
//
//  E6 =.= (C(1+TSX481.ConsumerClass.process.V_0))<100
//
//
//  E7 =.= 10<TESTMAIN400.test2a.Main.V_1
//
//
//  E8 =.= 10>=TESTMAIN400.test2a.Main.V_1
//
//
//  E9 =.= 6<(C(1+TESTMAIN400.test2a.Main.V_0))
//
//
//  E10 =.= 6>=(C(1+TESTMAIN400.test2a.Main.V_0))
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test2a to test2a

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test2a:
//2 vectors of width 3
//
//4 vectors of width 32
//
//2 vectors of width 1
//
//Total state bits in module = 136 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread ConsumerClass..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor16 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread test2a..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test2a.Main uid=Main10 has 36 CIL instructions in 7 basic blocks
//Thread ConsumerClass.process uid=process10 has 13 CIL instructions in 6 basic blocks
//Thread mpc10 has 6 bevelab control states (pauses)
//Thread mpc12 has 4 bevelab control states (pauses)
//Reindexed thread kiwiTSX481PC10 with 4 minor control states
//Reindexed thread kiwiTESTMAIN4001PC10 with 6 minor control states
// eof (HPR L/S Verilog)
