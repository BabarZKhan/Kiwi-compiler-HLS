

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:45:16
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test17r2.exe -bondout-loadstore-port-count=0 -sim=1800 -vnl-rootmodname=DUT -vnl test17r2.v -kiwic-autodispose=enable -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg signed [31:0] KtestFpointers_zout,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiKTES17R2400PC10nz_pc_export);

function [31:0] rtl_unsigned_bitextract0;
   input [63:0] arg;
   rtl_unsigned_bitextract0 = $unsigned(arg[31:0]);
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg signed [31:0] KTES17R2400_SynthesisedInvokeDispatch_KtestFpointers_AddToHash_CZ_0_5_0_5_SPILL_256;
  reg signed [31:0] KTES17R2400_KtestFpointers_run_test17r2_V_1;
  reg [1:0] KTES17R2400_KtestFpointers_run_test17r2_V_0;
  reg [63:0] KtestFpointers_f_mg_cache1;
// abstractionName=repack-newnets
  reg [63:0] A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10;
  wire [63:0] A_KiIntPtr_Opaque_CC_MAPR10NoCE2_entrypoint10;
  wire [63:0] A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10;
  reg [63:0] A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10;
// abstractionName=res2-morenets
  reg [2:0] kiwiKTES17R2400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin500183x10;
  wire signed [31:0] hprpin500292x10;
 always   @(* )  begin 
       hpr_int_run_enable_DDX14 = 32'sd1;
       hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.KTES17R2400/1.0
      if (reset)  begin 
               A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10 <= 64'd0;
               kiwiKTES17R2400PC10nz <= 32'd0;
               KtestFpointers_f_mg_cache1 <= 64'd0;
               A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10 <= 64'd0;
               KTES17R2400_KtestFpointers_run_test17r2_V_0 <= 32'd0;
               KTES17R2400_KtestFpointers_run_test17r2_V_1 <= 32'd0;
               KTES17R2400_SynthesisedInvokeDispatch_KtestFpointers_AddToHash_CZ_0_5_0_5_SPILL_256 <= 32'd0;
               KtestFpointers_zout <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiKTES17R2400PC10nz)
                  32'h0/*0:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Kiwi Scientific Acceleration - Test17r2 start.");
                           kiwiKTES17R2400PC10nz <= 32'h4/*4:kiwiKTES17R2400PC10nz*/;
                           KtestFpointers_f_mg_cache1 <= 64'h0;
                           A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10 <= 32'sd0/*CX_reflection_idl_KtestFpointers.hof_foofunc_A*/;
                           KTES17R2400_KtestFpointers_run_test17r2_V_0 <= 32'h1;
                           KTES17R2400_KtestFpointers_run_test17r2_V_1 <= 32'sh7d0;
                           KTES17R2400_SynthesisedInvokeDispatch_KtestFpointers_AddToHash_CZ_0_5_0_5_SPILL_256 <= 32'sh2f4f;
                           KtestFpointers_zout <= 32'sh2f4f;
                           end 
                          
                  32'h2/*2:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiKTES17R2400PC10nz <= 32'h3/*3:kiwiKTES17R2400PC10nz*/;
                           KTES17R2400_KtestFpointers_run_test17r2_V_0 <= rtl_unsigned_bitextract0(KtestFpointers_f_mg_cache1);
                           end 
                          
                  32'h3/*3:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (($signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)>=32'sh1388))  begin 
                                  $display("Kiwi Scientific Acceleration - Test17r2 finished. katy=%1d", $signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1
                                  ));
                                  $finish(32'sd0);
                                   kiwiKTES17R2400PC10nz <= 32'h6/*6:kiwiKTES17R2400PC10nz*/;
                                   KTES17R2400_KtestFpointers_run_test17r2_V_1 <= $signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1
                                  );

                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                  if ((((32'h1/*1:USA12*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0
                          )? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: ((32'h2/*2:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE2_entrypoint10: ((32'h1/*1:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10: ((32'h0/*0:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10: 32'bx)))))? 1'd1
                          : (32'h0/*0:USA12*/!=((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0
                          )? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: ((32'h2/*2:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE2_entrypoint10: ((32'h1/*1:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10: ((32'h0/*0:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10: 32'bx)))))) || 
                          (32'h0/*0:USA12*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0
                          )? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: ((32'h2/*2:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE2_entrypoint10: ((32'h1/*1:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10: ((32'h0/*0:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10: 32'bx)))))) && 
                          ($signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)<32'sh1388))  begin 
                                  if ((((32'h1/*1:USA14*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0
                                  )? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: hprpin500183x10))? 1'd1: (32'h0/*0:USA14*/!=((32'h3
                                  /*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10
                                  : hprpin500183x10))) || (32'h0/*0:USA14*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                                  KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: hprpin500183x10
                                  ))) && ($signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)<32'sh1388))  begin 
                                           KTES17R2400_KtestFpointers_run_test17r2_V_1 <= $signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1
                                          );

                                           KTES17R2400_SynthesisedInvokeDispatch_KtestFpointers_AddToHash_CZ_0_5_0_5_SPILL_256 <= (($signed(32'sd500
                                          +KTES17R2400_KtestFpointers_run_test17r2_V_1)<32'sh1388) && (32'h1/*1:USA14*/!=((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10
                                          : hprpin500183x10)) && (32'h0/*0:USA14*/!=((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                                          KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10
                                          : hprpin500183x10))? 32'sh55: hprpin500292x10);

                                           end 
                                          if ((((32'h1/*1:USA14*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0
                                  )? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: hprpin500183x10))? 1'd1: (32'h0/*0:USA14*/!=((32'h3
                                  /*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10
                                  : hprpin500183x10))) || (32'h0/*0:USA14*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
                                  KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: hprpin500183x10
                                  ))) && ($signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)<32'sh1388))  kiwiKTES17R2400PC10nz
                                       <= 32'h5/*5:kiwiKTES17R2400PC10nz*/;

                                       end 
                                   end 
                          
                  32'h1/*1:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Set zout to %1d", KtestFpointers_zout);
                          if (!(!(!KtestFpointers_f_mg_cache1)))  begin 
                                   A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10 <= 32'sd1/*CX_reflection_idl_KtestFpointers.hof_foofunc_B*/;
                                   KtestFpointers_f_mg_cache1 <= 64'h0;
                                   end 
                                   kiwiKTES17R2400PC10nz <= 32'h2/*2:kiwiKTES17R2400PC10nz*/;
                           end 
                          
                  32'h4/*4:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiKTES17R2400PC10nz <= 32'h1/*1:kiwiKTES17R2400PC10nz*/;
                      
                  32'h5/*5:kiwiKTES17R2400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiKTES17R2400PC10nz <= 32'h4/*4:kiwiKTES17R2400PC10nz*/;
                           KtestFpointers_zout <= KTES17R2400_SynthesisedInvokeDispatch_KtestFpointers_AddToHash_CZ_0_5_0_5_SPILL_256
                          ;

                           end 
                          endcase
              if (reset)  kiwiKTES17R2400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX14)  kiwiKTES17R2400PC10nz_pc_export <= kiwiKTES17R2400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.KTES17R2400/1.0


       end 
      

assign hprpin500183x10 = ((32'h2/*2:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE2_entrypoint10
: ((32'h1/*1:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE1_entrypoint10
: ((32'h0/*0:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE0_entrypoint10
: 32'bx)));

assign hprpin500292x10 = (($signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)<32'sh1388) && (32'h0/*0:USA14*/==((32'h3/*3:KTES17R2400.KtestFpointers.run_test17r2.V_0*/==
KTES17R2400_KtestFpointers_run_test17r2_V_0)? A_KiIntPtr_Opaque_CC_MAPR10NoCE3_entrypoint10: hprpin500183x10))? $signed(32'sh277f+$signed(32'sd500
+KTES17R2400_KtestFpointers_run_test17r2_V_1)): $signed(32'sh2780+$signed(32'sd500+KTES17R2400_KtestFpointers_run_test17r2_V_1)));

// Structural Resource (FU) inventory for DUT:// 1 vectors of width 3
// 3 vectors of width 64
// 1 vectors of width 2
// 2 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 262 bits.
// 192 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:45:13
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test17r2.exe -bondout-loadstore-port-count=0 -sim=1800 -vnl-rootmodname=DUT -vnl test17r2.v -kiwic-autodispose=enable -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*
//| Class          | Style   | Dir Style                                                          | Timing Target | Method                      | UID            | Skip  |
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*
//| KtestFpointers | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | KtestFpointers.run_test17r2 | runtest17r2_10 | false |
//|                |         | dmode, enable/directorate-pc-export                                |               |                             |                |       |
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*

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
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class KtestFpointers  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=KtestFpointers.run_test17r2 uid=runtest17r2_10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=runtest17r2_10 full_idl=KtestFpointers.run_test17r2
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
//   srcfile=test17r2.exe
//
//
//   kiwic-autodispose=enable
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
//PC codings points for kiwiKTES17R2400PC10 
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiKTES17R2400PC10"   | 813 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 4    |
//| XU32'2:"2:kiwiKTES17R2400PC10"   | 811 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'4:"4:kiwiKTES17R2400PC10"   | 810 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'8:"8:kiwiKTES17R2400PC10"   | 808 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 5    |
//| XU32'8:"8:kiwiKTES17R2400PC10"   | 809 | 3       | hwm=0.0.0   | 3    |        | -     | -   | -    |
//| XU32'1:"1:kiwiKTES17R2400PC10"   | 812 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 1    |
//| XU32'16:"16:kiwiKTES17R2400PC10" | 807 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 4    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'0:"0:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'0:"0:kiwiKTES17R2400PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                              |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                   |
//| F0   | E813 | R0 DATA |                                                                                                                   |
//| F0+E | E813 | W0 DATA | KtestFpointers.zoutwrite(S32'12111) KTES17R2400.SynthesisedInvokeDispatch_KtestFpointers.AddToHash.CZ:0:5.0.5._S\ |
//|      |      |         | PILL.256write(S32'12111) KTES17R2400.KtestFpointers.run_test17r2.V_1write(S32'2000) KTES17R2400.KtestFpointers.r\ |
//|      |      |         | un_test17r2.V_0write(Cu({SC:c19,1})) @KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10write(E1) KtestFpointers.<>f__m\ |
//|      |      |         | g$cache1write(U64'0)  PLI:Kiwi Scientific Acce...                                                                 |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'2:"2:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'2:"2:kiwiKTES17R2400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                           |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                                                                                                |
//| F1   | E811 | R0 DATA | KtestFpointers.zoutargread(<NONE>)                                                                                             |
//| F1+E | E811 | W0 DATA | KtestFpointers.<>f__mg$cache1write(C64u({SC:c19,0})) @KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10write(E2)  PLI:Set zout to %d |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'4:"4:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'4:"4:kiwiKTES17R2400PC10"
//*------+------+---------+------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                 |
//*------+------+---------+------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                      |
//| F2   | E810 | R0 DATA |                                                      |
//| F2+E | E810 | W0 DATA | KTES17R2400.KtestFpointers.run_test17r2.V_0write(E3) |
//*------+------+---------+------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'8:"8:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'8:"8:kiwiKTES17R2400PC10"
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                               |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------*
//| F3   | -    | R0 CTRL |                                                                                                                                                    |
//| F3   | E809 | R0 DATA |                                                                                                                                                    |
//| F3+E | E809 | W0 DATA | KTES17R2400.KtestFpointers.run_test17r2.V_1write(E4)  PLI:GSAI:hpr_sysexit  PLI:Kiwi Scientific Acce...                                            |
//| F3   | E808 | R0 DATA |                                                                                                                                                    |
//| F3+E | E808 | W0 DATA | KTES17R2400.SynthesisedInvokeDispatch_KtestFpointers.AddToHash.CZ:0:5.0.5._SPILL.256write(E5) KTES17R2400.KtestFpointers.run_test17r2.V_1write(E4) |
//*------+------+---------+----------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'1:"1:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'1:"1:kiwiKTES17R2400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F4   | -    | R0 CTRL |      |
//| F4   | E812 | R0 DATA |      |
//| F4+E | E812 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'16:"16:kiwiKTES17R2400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R2400PC10 state=XU32'16:"16:kiwiKTES17R2400PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F5   | -    | R0 CTRL |                              |
//| F5   | E807 | R0 DATA |                              |
//| F5+E | E807 | W0 DATA | KtestFpointers.zoutwrite(E6) |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= X0:"CX_reflection_idl_KtestFpointers.hof_foofunc_A"
//
//
//  E2 =.= X1:"CX_reflection_idl_KtestFpointers.hof_foofunc_B"
//
//
//  E3 =.= Cu(KtestFpointers.<>f__mg$cache1)
//
//
//  E4 =.= C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1)
//
//
//  E5 =.= COND({[XU32'1:"1:USA12"!=(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), XU32'0:"0:USA12"!=(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))<S32'5000]}, S32'85, COND({[XU32'0:"0:USA12"==(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))<S32'5000]}, C(S'10111+(C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))), C(S'10112+(C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1)))))
//
//
//  E6 =.= C(KTES17R2400.SynthesisedInvokeDispatch_KtestFpointers.AddToHash.CZ:0:5.0.5._SPILL.256)
//
//
//  E7 =.= (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))>=S32'5000
//
//
//  E8 =.= {[XU32'1:"1:USA12"!=(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), XU32'0:"0:USA12"!=(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))<S32'5000]; [XU32'0:"0:USA12"==(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))<S32'5000]; [XU32'1:"1:USA12"==(COND(XU32'3:"3:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE3_entrypoint10), COND(XU32'2:"2:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE2_entrypoint10), COND(XU32'1:"1:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE1_entrypoint10), COND(XU32'0:"0:KTES17R2400.KtestFpointers.run_test17r2.V_0"==KTES17R2400.KtestFpointers.run_test17r2.V_0, (C)(@KiIntPtr_Opaque/CC/MAPR10NoCE0_entrypoint10), (C)(*UNDEF)))))), (C(500+KTES17R2400.KtestFpointers.run_test17r2.V_1))<S32'5000]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test17r2 to test17r2

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for DUT:
//1 vectors of width 3
//
//3 vectors of width 64
//
//1 vectors of width 2
//
//2 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 262 bits.
//
//192 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread KtestFpointers.run_test17r2 uid=runtest17r2_10 has 36 CIL instructions in 10 basic blocks
//Thread KtestFpointers.run_test17r2 uid=runtest17r2_10 has 47 CIL instructions in 15 basic blocks
//Thread mpc10 has 6 bevelab control states (pauses)
//Reindexed thread kiwiKTES17R2400PC10 with 6 minor control states
// eof (HPR L/S Verilog)
