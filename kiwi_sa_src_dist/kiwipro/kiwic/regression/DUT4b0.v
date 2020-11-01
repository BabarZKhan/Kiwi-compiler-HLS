

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:43:02
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b0.v -vnl-resets=synchronous -sim 3000 -restructure2=disable -bevelab-recode-pc=disable -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX16,
    
/* portgroup= abstractionName=kiwicmiscio10 */
input din,
    output dout);
// abstractionName=bevelab-pc-nets
  reg signed [31:0] mpc10;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX16;
// abstractionName=kiwicmainnets10
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_6;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_4;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_3;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_1;
  reg signed [31:0] TESTMAIN400_test4_arraypart_0_3_V_0;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_SCALbx10_ARA0[8:0];
 always   @(* )  begin 
       hpr_int_run_enable_DDX16 = 32'sd1;
       hpr_int_run_enable_DDX16 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTMAIN400/1.0
      if (reset)  begin 
               A_SINT_CC_SCALbx10_ARA0[32'sd2] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd8] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd7] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd6] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd5] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd4] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd3] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd1] <= 32'd0;
               A_SINT_CC_SCALbx10_ARA0[32'sd0] <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_0 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_4 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_1 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_6 <= 32'd0;
               TESTMAIN400_test4_arraypart_0_3_V_3 <= 32'd0;
               mpc10 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX16) 
              case (mpc10)
                  32'h5e/*94:mpc10*/: if (hpr_int_run_enable_DDX16)  begin 
                          $finish(32'sd0);
                           mpc10 <= 32'bx;
                           hpr_abend_syndrome <= 32'sd0;
                           end 
                          
                  32'h49/*73:mpc10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (($signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6)<32'sh9))  begin 
                                  $write("%1d vale=%1d: ", TESTMAIN400_test4_arraypart_0_3_V_3, TESTMAIN400_test4_arraypart_0_3_V_4);
                                  $display("so far %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                   end 
                                   else  begin 
                                  $write("%1d vale=%1d: ", TESTMAIN400_test4_arraypart_0_3_V_3, TESTMAIN400_test4_arraypart_0_3_V_4);
                                  $display("so far %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                  $display("Found %1d Odd Numbers, and %1d Even Numbers.", TESTMAIN400_test4_arraypart_0_3_V_0, TESTMAIN400_test4_arraypart_0_3_V_1
                                  );
                                   end 
                                  if (($signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6)<32'sh9))  begin 
                                   mpc10 <= 32'h3a/*58:mpc10*/;
                                   TESTMAIN400_test4_arraypart_0_3_V_6 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6);
                                   TESTMAIN400_test4_arraypart_0_3_V_3 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_3);
                                   end 
                                   else  begin 
                                   mpc10 <= 32'h5e/*94:mpc10*/;
                                   TESTMAIN400_test4_arraypart_0_3_V_6 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_6);
                                   TESTMAIN400_test4_arraypart_0_3_V_3 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_3);
                                   end 
                                   end 
                          
                  32'h47/*71:mpc10*/: if (hpr_int_run_enable_DDX16)  mpc10 <= 32'h49/*73:mpc10*/;
                      
                  32'h3a/*58:mpc10*/: if (hpr_int_run_enable_DDX16)  begin 
                          if (!(!($signed(A_SINT_CC_SCALbx10_ARA0[TESTMAIN400_test4_arraypart_0_3_V_6])%32'sd2)))  TESTMAIN400_test4_arraypart_0_3_V_0
                               <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_0);

                               else  TESTMAIN400_test4_arraypart_0_3_V_1 <= $signed(32'sd1+TESTMAIN400_test4_arraypart_0_3_V_1);
                           mpc10 <= 32'h47/*71:mpc10*/;
                           TESTMAIN400_test4_arraypart_0_3_V_4 <= $signed(A_SINT_CC_SCALbx10_ARA0[TESTMAIN400_test4_arraypart_0_3_V_6
                          ]);

                           end 
                          
                  32'h0/*0:mpc10*/: if (hpr_int_run_enable_DDX16)  begin 
                           mpc10 <= 32'h3a/*58:mpc10*/;
                           TESTMAIN400_test4_arraypart_0_3_V_6 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_3 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_1 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_0 <= 32'sh0;
                           TESTMAIN400_test4_arraypart_0_3_V_4 <= 32'sh0;
                           A_SINT_CC_SCALbx10_ARA0[32'sd2] <= 32'sh2;
                           A_SINT_CC_SCALbx10_ARA0[32'sd8] <= 32'sh800;
                           A_SINT_CC_SCALbx10_ARA0[32'sd7] <= 32'sh7e5;
                           A_SINT_CC_SCALbx10_ARA0[32'sd6] <= 32'sh461;
                           A_SINT_CC_SCALbx10_ARA0[32'sd5] <= 32'sh8;
                           A_SINT_CC_SCALbx10_ARA0[32'sd4] <= 32'sh7;
                           A_SINT_CC_SCALbx10_ARA0[32'sd3] <= 32'sh5;
                           A_SINT_CC_SCALbx10_ARA0[32'sd1] <= 32'sh1;
                           A_SINT_CC_SCALbx10_ARA0[32'sd0] <= 32'sh64;
                           end 
                          endcase
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TESTMAIN400/1.0


       end 
      

// Structural Resource (FU) inventory for DUT:// 6 vectors of width 32
// 1 vectors of width 1
// 9 array locations of width 32
// Total state bits in module = 481 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:43:00
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test4.exe -root test4;test4.Main -vnl-rootmodname=DUT -vnl DUT4b0.v -vnl-resets=synchronous -sim 3000 -restructure2=disable -bevelab-recode-pc=disable -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| AddressSpace | Name       | Protocol                      | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | 4194304  | 22     | 64     | 8     | 8         |
//*--------------+------------+-------------------------------+----------+--------+--------+-------+-----------*

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
//| bondout-loadstore-port-count     | 1     |             |
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
//KiwiC: front end input processing of class test4  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test4..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=test4..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class test4  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'test4' tid=TEST4401 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'test4' tid=TEST4401
//
//
//root_compiler class done: test4
//
//
//KiwiC: front end input processing of class test4.Main  wonky=test4 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=test4.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=test4.Main
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
//   bondout-loadstore-port-count=1
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
//   root=test4;test4.Main
//
//
//   srcfile=test4.exe
//
//
//   kiwic-autodispose=disable
//
//
//END OF KIWIC REPORT FILE
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for DUT4b0 to DUT4b0

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//6 vectors of width 32
//
//1 vectors of width 1
//
//9 array locations of width 32
//
//Total state bits in module = 481 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread test4..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread test4.Main uid=Main10 has 42 CIL instructions in 8 basic blocks
//Thread mpc10 has 5 bevelab control states (pauses)
// eof (HPR L/S Verilog)
