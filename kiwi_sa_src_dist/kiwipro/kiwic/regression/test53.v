

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:48:45
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test53.exe /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test53.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=res2-directornets */
output reg [4:0] kiwiTESTMAIN400PC10nz_pc_export);
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_11_V_1;
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_11_V_0;
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_15_V_1;
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_15_V_0;
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_4_V_1;
  reg signed [31:0] TESTMAIN400_Test53_bottom_0_4_V_0;
  reg signed [31:0] Test53_modder_by;
  reg signed [31:0] Test53_modder_ax;
  reg signed [31:0] TESTMAIN400_Test53_t53a_0_4_V_0;
// abstractionName=res2-morenets
  reg [4:0] kiwiTESTMAIN400PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               kiwiTESTMAIN400PC10nz <= 32'd0;
               TESTMAIN400_Test53_bottom_0_11_V_1 <= 32'd0;
               TESTMAIN400_Test53_bottom_0_11_V_0 <= 32'd0;
               TESTMAIN400_Test53_bottom_0_15_V_1 <= 32'd0;
               TESTMAIN400_Test53_bottom_0_15_V_0 <= 32'd0;
               TESTMAIN400_Test53_bottom_0_4_V_1 <= 32'd0;
               TESTMAIN400_Test53_bottom_0_4_V_0 <= 32'd0;
               TESTMAIN400_Test53_t53a_0_4_V_0 <= 32'd0;
               Test53_modder_ax <= 32'd0;
               Test53_modder_by <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (kiwiTESTMAIN400PC10nz)
                  32'h0/*0:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test53 start.");
                          $display(" modder entry  %1d %1d", 32'sd10, 32'sd110);
                           kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_11_V_1 <= 32'sh0;
                           TESTMAIN400_Test53_bottom_0_11_V_0 <= 32'sh0;
                           TESTMAIN400_Test53_bottom_0_15_V_1 <= 32'sh0;
                           TESTMAIN400_Test53_bottom_0_15_V_0 <= 32'sh0;
                           TESTMAIN400_Test53_bottom_0_4_V_1 <= 32'sh0;
                           TESTMAIN400_Test53_bottom_0_4_V_0 <= 32'sh0;
                           TESTMAIN400_Test53_t53a_0_4_V_0 <= 32'sha;
                           Test53_modder_ax <= 32'sha;
                           Test53_modder_by <= 32'sh6e;
                           end 
                          
                  32'h1/*1:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display(" modder exit   %1d %1d", $signed(32'sd2+Test53_modder_ax), $signed(32'sd1+Test53_modder_by));
                           kiwiTESTMAIN400PC10nz <= 32'h3/*3:kiwiTESTMAIN400PC10nz*/;
                           Test53_modder_ax <= $signed(32'sd2+Test53_modder_ax);
                           Test53_modder_by <= $signed(32'sd1+Test53_modder_by);
                           end 
                          
                  32'h2/*2:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h1/*1:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h3/*3:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0)>=32'sd12)) $display("Test53a finished.");
                               else  begin 
                                  $display(" modder entry  %1d %1d", $signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0), $signed(32'sd100
                                  +$signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0)));
                                   kiwiTESTMAIN400PC10nz <= 32'h2/*2:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_t53a_0_4_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0);
                                   Test53_modder_ax <= $signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0);
                                   Test53_modder_by <= $signed(32'sd100+$signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0));
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0)>=32'sd12))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h4/*4:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_t53a_0_4_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_t53a_0_4_V_0);
                                   end 
                                   end 
                          
                  32'h4/*4:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'h5/*5:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h5/*5:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h6/*6:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_4_V_0 <= 32'sh2;
                           end 
                          
                  32'h6/*6:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_4_V_1 <= 32'sh0;
                           end 
                          
                  32'h7/*7:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TESTMAIN400_Test53_bottom_0_4_V_1>=32'sd2)) $display("middle subroutine %1d %1d", 32'sd2, 32'sd2);
                               else  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h8/*8:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_bottom_0_4_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_4_V_0);
                                   end 
                                  if ((TESTMAIN400_Test53_bottom_0_4_V_1>=32'sd2))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h9/*9:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_bottom_0_15_V_0 <= 32'sh15;
                                   end 
                                   end 
                          
                  32'h8/*8:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("bottom subroutine %1d %1d", TESTMAIN400_Test53_bottom_0_4_V_0, 32'sd2);
                           kiwiTESTMAIN400PC10nz <= 32'h7/*7:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_4_V_1 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_4_V_1);
                           end 
                          
                  32'h9/*9:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_15_V_1 <= 32'sh0;
                           end 
                          
                  32'ha/*10:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((TESTMAIN400_Test53_bottom_0_15_V_1<32'sd2))  begin 
                               kiwiTESTMAIN400PC10nz <= 32'hb/*11:kiwiTESTMAIN400PC10nz*/;
                               TESTMAIN400_Test53_bottom_0_15_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_15_V_0);
                               end 
                               else  kiwiTESTMAIN400PC10nz <= 32'hc/*12:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hb/*11:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("bottom subroutine %1d %1d", TESTMAIN400_Test53_bottom_0_15_V_0, 32'sd1);
                           kiwiTESTMAIN400PC10nz <= 32'ha/*10:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_15_V_1 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_15_V_1);
                           end 
                          
                  32'hc/*12:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz <= 32'hd/*13:kiwiTESTMAIN400PC10nz*/;
                      
                  32'hd/*13:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'he/*14:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_11_V_0 <= 32'sh14;
                           end 
                          
                  32'he/*14:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_11_V_1 <= 32'sh0;
                           end 
                          
                  32'hf/*15:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          if ((TESTMAIN400_Test53_bottom_0_11_V_1>=32'sd2)) $display("middle subroutine %1d %1d", 32'sd20, 32'sd2);
                               else  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h10/*16:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_bottom_0_11_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_11_V_0);
                                   end 
                                  if ((TESTMAIN400_Test53_bottom_0_11_V_1>=32'sd2))  begin 
                                   kiwiTESTMAIN400PC10nz <= 32'h11/*17:kiwiTESTMAIN400PC10nz*/;
                                   TESTMAIN400_Test53_bottom_0_15_V_0 <= 32'shc9;
                                   end 
                                   end 
                          
                  32'h10/*16:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("bottom subroutine %1d %1d", TESTMAIN400_Test53_bottom_0_11_V_0, 32'sd2);
                           kiwiTESTMAIN400PC10nz <= 32'hf/*15:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_11_V_1 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_11_V_1);
                           end 
                          
                  32'h11/*17:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                           kiwiTESTMAIN400PC10nz <= 32'h12/*18:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_15_V_1 <= 32'sh0;
                           end 
                          
                  32'h12/*18:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16) if ((TESTMAIN400_Test53_bottom_0_15_V_1<32'sd2)) 
                               begin 
                               kiwiTESTMAIN400PC10nz <= 32'h13/*19:kiwiTESTMAIN400PC10nz*/;
                               TESTMAIN400_Test53_bottom_0_15_V_0 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_15_V_0);
                               end 
                               else  kiwiTESTMAIN400PC10nz <= 32'h14/*20:kiwiTESTMAIN400PC10nz*/;
                      
                  32'h13/*19:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("bottom subroutine %1d %1d", TESTMAIN400_Test53_bottom_0_15_V_0, 32'sd1);
                           kiwiTESTMAIN400PC10nz <= 32'h12/*18:kiwiTESTMAIN400PC10nz*/;
                           TESTMAIN400_Test53_bottom_0_15_V_1 <= $signed(32'sd1+TESTMAIN400_Test53_bottom_0_15_V_1);
                           end 
                          
                  32'h14/*20:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test53b finished.");
                           kiwiTESTMAIN400PC10nz <= 32'h15/*21:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h15/*21:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $display("Test53 finished.");
                           kiwiTESTMAIN400PC10nz <= 32'h16/*22:kiwiTESTMAIN400PC10nz*/;
                           end 
                          
                  32'h16/*22:kiwiTESTMAIN400PC10nz*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           kiwiTESTMAIN400PC10nz <= 32'h17/*23:kiwiTESTMAIN400PC10nz*/;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          endcase
              if (reset)  kiwiTESTMAIN400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX16)  kiwiTESTMAIN400PC10nz_pc_export <= kiwiTESTMAIN400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 5
// 9 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 294 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:48:42
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test53.exe /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll -sim=1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test53.v -bondout-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Class  | Style   | Dir Style                                                                                            | Timing Target | Method      | UID    | Skip  |
//*--------+---------+------------------------------------------------------------------------------------------------------+---------------+-------------+--------+-------*
//| Test53 | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-endmode, enable/directorate-pc-export |               | Test53.Main | Main10 | false |
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
//KiwiC: front end input processing of class KiwiStringIO  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KiwiStringIO..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=KiwiStringIO..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 2/prev
//
//
//KiwiC: front end input processing of class Test53  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test53.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Test53.Main
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
//   ?>?=srcfile, test53.exe, /r:/home/djg11/d320/hprls/kiwipro/kiwic/kdistro/support/KiwiStringIO.dll
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
//PC codings points for kiwiTESTMAIN400PC10 
//*--------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                              | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTMAIN400PC10"             | 855 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTMAIN400PC10"             | 853 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 3    |
//| XU32'1:"1:kiwiTESTMAIN400PC10"             | 854 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"             | 851 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 2    |
//| XU32'4:"4:kiwiTESTMAIN400PC10"             | 852 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'8:"8:kiwiTESTMAIN400PC10"             | 850 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiTESTMAIN400PC10"           | 849 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 6    |
//| XU32'32:"32:kiwiTESTMAIN400PC10"           | 848 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 7    |
//| XU32'64:"64:kiwiTESTMAIN400PC10"           | 846 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 8    |
//| XU32'64:"64:kiwiTESTMAIN400PC10"           | 847 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 9    |
//| XU32'128:"128:kiwiTESTMAIN400PC10"         | 845 | 8       | hwm=0.0.0   | 8    |        | -     | -   | 7    |
//| XU32'256:"256:kiwiTESTMAIN400PC10"         | 844 | 9       | hwm=0.0.0   | 9    |        | -     | -   | 10   |
//| XU32'512:"512:kiwiTESTMAIN400PC10"         | 842 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 11   |
//| XU32'512:"512:kiwiTESTMAIN400PC10"         | 843 | 10      | hwm=0.0.0   | 10   |        | -     | -   | 12   |
//| XU32'1024:"1024:kiwiTESTMAIN400PC10"       | 841 | 11      | hwm=0.0.0   | 11   |        | -     | -   | 10   |
//| XU32'2048:"2048:kiwiTESTMAIN400PC10"       | 840 | 12      | hwm=0.0.0   | 12   |        | -     | -   | 13   |
//| XU32'4096:"4096:kiwiTESTMAIN400PC10"       | 839 | 13      | hwm=0.0.0   | 13   |        | -     | -   | 14   |
//| XU32'8192:"8192:kiwiTESTMAIN400PC10"       | 838 | 14      | hwm=0.0.0   | 14   |        | -     | -   | 15   |
//| XU32'16384:"16384:kiwiTESTMAIN400PC10"     | 836 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 16   |
//| XU32'16384:"16384:kiwiTESTMAIN400PC10"     | 837 | 15      | hwm=0.0.0   | 15   |        | -     | -   | 17   |
//| XU32'32768:"32768:kiwiTESTMAIN400PC10"     | 835 | 16      | hwm=0.0.0   | 16   |        | -     | -   | 15   |
//| XU32'65536:"65536:kiwiTESTMAIN400PC10"     | 834 | 17      | hwm=0.0.0   | 17   |        | -     | -   | 18   |
//| XU32'131072:"131072:kiwiTESTMAIN400PC10"   | 832 | 18      | hwm=0.0.0   | 18   |        | -     | -   | 19   |
//| XU32'131072:"131072:kiwiTESTMAIN400PC10"   | 833 | 18      | hwm=0.0.0   | 18   |        | -     | -   | 20   |
//| XU32'262144:"262144:kiwiTESTMAIN400PC10"   | 831 | 19      | hwm=0.0.0   | 19   |        | -     | -   | 18   |
//| XU32'524288:"524288:kiwiTESTMAIN400PC10"   | 830 | 20      | hwm=0.0.0   | 20   |        | -     | -   | 21   |
//| XU32'1048576:"1048576:kiwiTESTMAIN400PC10" | 829 | 21      | hwm=0.0.0   | 21   |        | -     | -   | 22   |
//| XU32'2097152:"2097152:kiwiTESTMAIN400PC10" | 828 | 22      | hwm=0.0.0   | 22   |        | -     | -   | -    |
//*--------------------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'0:"0:kiwiTESTMAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                           |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                |
//| F0   | E855 | R0 DATA |                                                                                                                                |
//| F0+E | E855 | W0 DATA | Test53.modder.bywrite(S32'110) Test53.modder.axwrite(S32'10) TESTMAIN400.Test53.t53a.0.4.V_0write(S32'10) TESTMAIN400.Test53.\ |
//|      |      |         | bottom.0.4.V_0write(S32'0) TESTMAIN400.Test53.bottom.0.4.V_1write(S32'0) TESTMAIN400.Test53.bottom.0.15.V_0write(S32'0) TESTM\ |
//|      |      |         | AIN400.Test53.bottom.0.15.V_1write(S32'0) TESTMAIN400.Test53.bottom.0.11.V_0write(S32'0) TESTMAIN400.Test53.bottom.0.11.V_1wr\ |
//|      |      |         | ite(S32'0)  PLI: modder entry  %d %d  PLI:Test53 start.                                                                        |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2:"2:kiwiTESTMAIN400PC10"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                     |
//| F1   | E853 | R0 DATA |                                                                                                                     |
//| F1+E | E853 | W0 DATA | Test53.modder.bywrite(C(1+Test53.modder.by)) Test53.modder.axwrite(C(2+Test53.modder.ax))  PLI: modder exit   %d %d |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1:"1:kiwiTESTMAIN400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F2   | -    | R0 CTRL |      |
//| F2   | E854 | R0 DATA |      |
//| F2+E | E854 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4:"4:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                   |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                        |
//| F3   | E852 | R0 DATA |                                                                                                                        |
//| F3+E | E852 | W0 DATA | TESTMAIN400.Test53.t53a.0.4.V_0write(E1)  PLI:Test53a finished.                                                        |
//| F3   | E851 | R0 DATA |                                                                                                                        |
//| F3+E | E851 | W0 DATA | Test53.modder.bywrite(E2) Test53.modder.axwrite(E1) TESTMAIN400.Test53.t53a.0.4.V_0write(E1)  PLI: modder entry  %d %d |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8:"8:kiwiTESTMAIN400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F4   | -    | R0 CTRL |      |
//| F4   | E850 | R0 DATA |      |
//| F4+E | E850 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16:"16:kiwiTESTMAIN400PC10"
//*------+------+---------+-----------------------------------------------*
//| pc   | eno  | Phaser  | Work                                          |
//*------+------+---------+-----------------------------------------------*
//| F5   | -    | R0 CTRL |                                               |
//| F5   | E849 | R0 DATA |                                               |
//| F5+E | E849 | W0 DATA | TESTMAIN400.Test53.bottom.0.4.V_0write(S32'2) |
//*------+------+---------+-----------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32:"32:kiwiTESTMAIN400PC10"
//*------+------+---------+-----------------------------------------------*
//| pc   | eno  | Phaser  | Work                                          |
//*------+------+---------+-----------------------------------------------*
//| F6   | -    | R0 CTRL |                                               |
//| F6   | E848 | R0 DATA |                                               |
//| F6+E | E848 | W0 DATA | TESTMAIN400.Test53.bottom.0.4.V_1write(S32'0) |
//*------+------+---------+-----------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'64:"64:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                         |
//*------+------+---------+------------------------------------------------------------------------------*
//| F7   | -    | R0 CTRL |                                                                              |
//| F7   | E847 | R0 DATA |                                                                              |
//| F7+E | E847 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_0write(S32'21)  PLI:middle subroutine %d... |
//| F7   | E846 | R0 DATA |                                                                              |
//| F7+E | E846 | W0 DATA | TESTMAIN400.Test53.bottom.0.4.V_0write(E3)                                   |
//*------+------+---------+------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'128:"128:kiwiTESTMAIN400PC10"
//*------+------+---------+-------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                    |
//*------+------+---------+-------------------------------------------------------------------------*
//| F8   | -    | R0 CTRL |                                                                         |
//| F8   | E845 | R0 DATA |                                                                         |
//| F8+E | E845 | W0 DATA | TESTMAIN400.Test53.bottom.0.4.V_1write(E4)  PLI:bottom subroutine %d... |
//*------+------+---------+-------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'256:"256:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'256:"256:kiwiTESTMAIN400PC10"
//*------+------+---------+------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                           |
//*------+------+---------+------------------------------------------------*
//| F9   | -    | R0 CTRL |                                                |
//| F9   | E844 | R0 DATA |                                                |
//| F9+E | E844 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_1write(S32'0) |
//*------+------+---------+------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'512:"512:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'512:"512:kiwiTESTMAIN400PC10"
//*-------+------+---------+---------------------------------------------*
//| pc    | eno  | Phaser  | Work                                        |
//*-------+------+---------+---------------------------------------------*
//| F10   | -    | R0 CTRL |                                             |
//| F10   | E843 | R0 DATA |                                             |
//| F10+E | E843 | W0 DATA |                                             |
//| F10   | E842 | R0 DATA |                                             |
//| F10+E | E842 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_0write(E5) |
//*-------+------+---------+---------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1024:"1024:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1024:"1024:kiwiTESTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                     |
//*-------+------+---------+--------------------------------------------------------------------------*
//| F11   | -    | R0 CTRL |                                                                          |
//| F11   | E841 | R0 DATA |                                                                          |
//| F11+E | E841 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_1write(E6)  PLI:bottom subroutine %d... |
//*-------+------+---------+--------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2048:"2048:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2048:"2048:kiwiTESTMAIN400PC10"
//*-------+------+---------+------*
//| pc    | eno  | Phaser  | Work |
//*-------+------+---------+------*
//| F12   | -    | R0 CTRL |      |
//| F12   | E840 | R0 DATA |      |
//| F12+E | E840 | W0 DATA |      |
//*-------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4096:"4096:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'4096:"4096:kiwiTESTMAIN400PC10"
//*-------+------+---------+-------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                            |
//*-------+------+---------+-------------------------------------------------*
//| F13   | -    | R0 CTRL |                                                 |
//| F13   | E839 | R0 DATA |                                                 |
//| F13+E | E839 | W0 DATA | TESTMAIN400.Test53.bottom.0.11.V_0write(S32'20) |
//*-------+------+---------+-------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8192:"8192:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'8192:"8192:kiwiTESTMAIN400PC10"
//*-------+------+---------+------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                           |
//*-------+------+---------+------------------------------------------------*
//| F14   | -    | R0 CTRL |                                                |
//| F14   | E838 | R0 DATA |                                                |
//| F14+E | E838 | W0 DATA | TESTMAIN400.Test53.bottom.0.11.V_1write(S32'0) |
//*-------+------+---------+------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16384:"16384:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'16384:"16384:kiwiTESTMAIN400PC10"
//*-------+------+---------+-------------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                          |
//*-------+------+---------+-------------------------------------------------------------------------------*
//| F15   | -    | R0 CTRL |                                                                               |
//| F15   | E837 | R0 DATA |                                                                               |
//| F15+E | E837 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_0write(S32'201)  PLI:middle subroutine %d... |
//| F15   | E836 | R0 DATA |                                                                               |
//| F15+E | E836 | W0 DATA | TESTMAIN400.Test53.bottom.0.11.V_0write(E7)                                   |
//*-------+------+---------+-------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32768:"32768:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'32768:"32768:kiwiTESTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                     |
//*-------+------+---------+--------------------------------------------------------------------------*
//| F16   | -    | R0 CTRL |                                                                          |
//| F16   | E835 | R0 DATA |                                                                          |
//| F16+E | E835 | W0 DATA | TESTMAIN400.Test53.bottom.0.11.V_1write(E8)  PLI:bottom subroutine %d... |
//*-------+------+---------+--------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'65536:"65536:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'65536:"65536:kiwiTESTMAIN400PC10"
//*-------+------+---------+------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                           |
//*-------+------+---------+------------------------------------------------*
//| F17   | -    | R0 CTRL |                                                |
//| F17   | E834 | R0 DATA |                                                |
//| F17+E | E834 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_1write(S32'0) |
//*-------+------+---------+------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'131072:"131072:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'131072:"131072:kiwiTESTMAIN400PC10"
//*-------+------+---------+---------------------------------------------*
//| pc    | eno  | Phaser  | Work                                        |
//*-------+------+---------+---------------------------------------------*
//| F18   | -    | R0 CTRL |                                             |
//| F18   | E833 | R0 DATA |                                             |
//| F18+E | E833 | W0 DATA |                                             |
//| F18   | E832 | R0 DATA |                                             |
//| F18+E | E832 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_0write(E5) |
//*-------+------+---------+---------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'262144:"262144:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'262144:"262144:kiwiTESTMAIN400PC10"
//*-------+------+---------+--------------------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                                     |
//*-------+------+---------+--------------------------------------------------------------------------*
//| F19   | -    | R0 CTRL |                                                                          |
//| F19   | E831 | R0 DATA |                                                                          |
//| F19+E | E831 | W0 DATA | TESTMAIN400.Test53.bottom.0.15.V_1write(E6)  PLI:bottom subroutine %d... |
//*-------+------+---------+--------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'524288:"524288:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'524288:"524288:kiwiTESTMAIN400PC10"
//*-------+------+---------+------------------------*
//| pc    | eno  | Phaser  | Work                   |
//*-------+------+---------+------------------------*
//| F20   | -    | R0 CTRL |                        |
//| F20   | E830 | R0 DATA |                        |
//| F20+E | E830 | W0 DATA |  PLI:Test53b finished. |
//*-------+------+---------+------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1048576:"1048576:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'1048576:"1048576:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F21   | -    | R0 CTRL |                       |
//| F21   | E829 | R0 DATA |                       |
//| F21+E | E829 | W0 DATA |  PLI:Test53 finished. |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2097152:"2097152:kiwiTESTMAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTMAIN400PC10 state=XU32'2097152:"2097152:kiwiTESTMAIN400PC10"
//*-------+------+---------+-----------------------*
//| pc    | eno  | Phaser  | Work                  |
//*-------+------+---------+-----------------------*
//| F22   | -    | R0 CTRL |                       |
//| F22   | E828 | R0 DATA |                       |
//| F22+E | E828 | W0 DATA |  PLI:GSAI:hpr_sysexit |
//*-------+------+---------+-----------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(1+TESTMAIN400.Test53.t53a.0.4.V_0)
//
//
//  E2 =.= C(100+(C(1+TESTMAIN400.Test53.t53a.0.4.V_0)))
//
//
//  E3 =.= C(1+TESTMAIN400.Test53.bottom.0.4.V_0)
//
//
//  E4 =.= C(1+TESTMAIN400.Test53.bottom.0.4.V_1)
//
//
//  E5 =.= C(1+TESTMAIN400.Test53.bottom.0.15.V_0)
//
//
//  E6 =.= C(1+TESTMAIN400.Test53.bottom.0.15.V_1)
//
//
//  E7 =.= C(1+TESTMAIN400.Test53.bottom.0.11.V_0)
//
//
//  E8 =.= C(1+TESTMAIN400.Test53.bottom.0.11.V_1)
//
//
//  E9 =.= (C(1+TESTMAIN400.Test53.t53a.0.4.V_0))>=12
//
//
//  E10 =.= (C(1+TESTMAIN400.Test53.t53a.0.4.V_0))<12
//
//
//  E11 =.= TESTMAIN400.Test53.bottom.0.4.V_1>=2
//
//
//  E12 =.= TESTMAIN400.Test53.bottom.0.4.V_1<2
//
//
//  E13 =.= TESTMAIN400.Test53.bottom.0.15.V_1>=2
//
//
//  E14 =.= TESTMAIN400.Test53.bottom.0.15.V_1<2
//
//
//  E15 =.= TESTMAIN400.Test53.bottom.0.11.V_1>=2
//
//
//  E16 =.= TESTMAIN400.Test53.bottom.0.11.V_1<2
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test53 to test53

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 5
//
//9 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 294 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread KiwiStringIO..cctor uid=cctor10 has 7 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread Test53.Main uid=Main10 has 81 CIL instructions in 31 basic blocks
//Thread mpc10 has 23 bevelab control states (pauses)
//Reindexed thread kiwiTESTMAIN400PC10 with 23 minor control states
// eof (HPR L/S Verilog)
