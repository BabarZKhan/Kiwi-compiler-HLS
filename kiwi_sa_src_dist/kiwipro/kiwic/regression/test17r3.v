

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:45:20
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test17r3.exe -kiwic-kcode-dump=enable -sim=1800 -vnl test17r3.v -kiwic-autodispose=enable -kiwic-cil-dump=combined -give-backtrace -report-each-step
`timescale 1ns/1ns


module test17r3(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX14,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output reg signed [31:0] KtestFpointers_zout,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiKTES17R3400PC10nz_pc_export,
    
/* portgroup= abstractionName=bondout2 pi_name=bondout0 */
output reg bondout0_REQ0,
    input bondout0_ACK0,
    input bondout0_REQRDY0,
    output reg bondout0_ACKRDY0,
    output reg [21:0] bondout0_ADDR0,
    output reg bondout0_RWBAR0,
    output reg [63:0] bondout0_WDATA0,
    input [63:0] bondout0_RDATA0,
    output reg [7:0] bondout0_LANES0,
    output reg bondout0_REQ1,
    input bondout0_ACK1,
    input bondout0_REQRDY1,
    output reg bondout0_ACKRDY1,
    output reg [21:0] bondout0_ADDR1,
    output reg bondout0_RWBAR1,
    output reg [63:0] bondout0_WDATA1,
    input [63:0] bondout0_RDATA1,
    output reg [7:0] bondout0_LANES1,
    
/* portgroup= abstractionName=bondout0 pi_name=bondout1 */
output reg bondout1_REQ0,
    input bondout1_ACK0,
    input bondout1_REQRDY0,
    output reg bondout1_ACKRDY0,
    output reg [21:0] bondout1_ADDR0,
    output reg bondout1_RWBAR0,
    output reg [63:0] bondout1_WDATA0,
    input [63:0] bondout1_RDATA0,
    output reg [7:0] bondout1_LANES0,
    output reg bondout1_REQ1,
    input bondout1_ACK1,
    input bondout1_REQRDY1,
    output reg bondout1_ACKRDY1,
    output reg [21:0] bondout1_ADDR1,
    output reg bondout1_RWBAR1,
    output reg [63:0] bondout1_WDATA1,
    input [63:0] bondout1_RDATA1,
    output reg [7:0] bondout1_LANES1);

function [31:0] rtl_unsigned_bitextract0;
   input [63:0] arg;
   rtl_unsigned_bitextract0 = $unsigned(arg[31:0]);
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX14;
// abstractionName=kiwicmainnets10
  reg fastspilldup10;
  reg signed [31:0] KTES17R3400_KtestFpointers_run_test17r3_V_1;
// abstractionName=repack-newnets
  reg signed [31:0] A_SINT_CC_MAPR10NoCE1_myfoo10;
  reg signed [31:0] A_SINT_CC_MAPR10NoCE0_myfoo10;
  reg [63:0] A_System_Object_CC_MAPR12NoCE1_closure10;
// abstractionName=res2-morenets
  reg [3:0] kiwiKTES17R3400PC10nz;
// abstractionName=share-nets pi_name=shareAnets10
  wire signed [31:0] hprpin500240x10;
  wire signed [31:0] hprpin500249x10;
  wire signed [31:0] hprpin500267x10;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.KTES17R3400/1.0
      if (reset)  begin 
               A_System_Object_CC_MAPR12NoCE1_closure10 <= 64'd0;
               kiwiKTES17R3400PC10nz <= 32'd0;
               KtestFpointers_zout <= 32'd0;
               A_SINT_CC_MAPR10NoCE0_myfoo10 <= 32'd0;
               KTES17R3400_KtestFpointers_run_test17r3_V_1 <= 32'd0;
               fastspilldup10 <= 32'd0;
               A_SINT_CC_MAPR10NoCE1_myfoo10 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX14) 
              case (kiwiKTES17R3400PC10nz)
                  32'h0/*0:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Kiwi Scientific Acceleration - Test17r3 start.");
                          if ((rtl_unsigned_bitextract0(64'h0)==32'h1/*1:USA18*/))  A_SINT_CC_MAPR10NoCE1_myfoo10 <= hprpin500240x10;
                               kiwiKTES17R3400PC10nz <= 32'h1/*1:kiwiKTES17R3400PC10nz*/;
                           KtestFpointers_zout <= 32'sh0;
                           A_SINT_CC_MAPR10NoCE0_myfoo10 <= 32'sh2;
                           KTES17R3400_KtestFpointers_run_test17r3_V_1 <= 32'sh7d0;
                           fastspilldup10 <= rtl_unsigned_bitextract0(64'h0);
                           end 
                          
                  32'h1/*1:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((32'h0/*0:fastspilldup10*/==fastspilldup10))  A_SINT_CC_MAPR10NoCE0_myfoo10 <= $signed(32'sd4000+((32'h1
                              /*1:fastspilldup10*/==fastspilldup10)? A_SINT_CC_MAPR10NoCE1_myfoo10: ((32'h0/*0:fastspilldup10*/==fastspilldup10
                              )? A_SINT_CC_MAPR10NoCE0_myfoo10: 32'bx)));

                               kiwiKTES17R3400PC10nz <= 32'h2/*2:kiwiKTES17R3400PC10nz*/;
                           end 
                          
                  32'h2/*2:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiKTES17R3400PC10nz <= 32'h6/*6:kiwiKTES17R3400PC10nz*/;
                           KtestFpointers_zout <= hprpin500249x10;
                           end 
                          
                  32'h4/*4:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if (($signed(32'sd500+KTES17R3400_KtestFpointers_run_test17r3_V_1)>=32'sh1388))  begin 
                                  $display("Kiwi Scientific Acceleration - Test17r3 finished.");
                                  $finish(32'sd0);
                                   end 
                                   else  begin 
                                  if ((rtl_unsigned_bitextract0(64'h0)==32'h1/*1:USA18*/))  A_SINT_CC_MAPR10NoCE1_myfoo10 <= hprpin500267x10
                                      ;

                                       kiwiKTES17R3400PC10nz <= 32'h5/*5:kiwiKTES17R3400PC10nz*/;
                                   A_System_Object_CC_MAPR12NoCE1_closure10 <= 64'h0;
                                   KTES17R3400_KtestFpointers_run_test17r3_V_1 <= $signed(32'sd500+KTES17R3400_KtestFpointers_run_test17r3_V_1
                                  );

                                   fastspilldup10 <= rtl_unsigned_bitextract0(64'h0);
                                   end 
                                  if (($signed(32'sd500+KTES17R3400_KtestFpointers_run_test17r3_V_1)>=32'sh1388))  begin 
                                   kiwiKTES17R3400PC10nz <= 32'h8/*8:kiwiKTES17R3400PC10nz*/;
                                   KTES17R3400_KtestFpointers_run_test17r3_V_1 <= $signed(32'sd500+KTES17R3400_KtestFpointers_run_test17r3_V_1
                                  );

                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          
                  32'h5/*5:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          if ((32'h0/*0:fastspilldup10*/==fastspilldup10))  A_SINT_CC_MAPR10NoCE0_myfoo10 <= $signed(((32'h1/*1:fastspilldup10*/==
                              fastspilldup10)? A_SINT_CC_MAPR10NoCE1_myfoo10: ((32'h0/*0:fastspilldup10*/==fastspilldup10)? A_SINT_CC_MAPR10NoCE0_myfoo10
                              : 32'bx))+32'sd2*KTES17R3400_KtestFpointers_run_test17r3_V_1);

                               kiwiKTES17R3400PC10nz <= 32'h7/*7:kiwiKTES17R3400PC10nz*/;
                           end 
                          
                  32'h3/*3:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                          $display("Set zout to %1d", KtestFpointers_zout);
                           kiwiKTES17R3400PC10nz <= 32'h4/*4:kiwiKTES17R3400PC10nz*/;
                           end 
                          
                  32'h6/*6:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  kiwiKTES17R3400PC10nz <= 32'h3/*3:kiwiKTES17R3400PC10nz*/;
                      
                  32'h7/*7:kiwiKTES17R3400PC10nz*/: if (hpr_int_run_enable_DDX14)  begin 
                           kiwiKTES17R3400PC10nz <= 32'h6/*6:kiwiKTES17R3400PC10nz*/;
                           KtestFpointers_zout <= $signed(KTES17R3400_KtestFpointers_run_test17r3_V_1+((32'h1/*1:USA16*/==A_System_Object_CC_MAPR12NoCE1_closure10
                          )? A_SINT_CC_MAPR10NoCE1_myfoo10: ((32'h0/*0:USA16*/==A_System_Object_CC_MAPR12NoCE1_closure10)? A_SINT_CC_MAPR10NoCE0_myfoo10
                          : 32'bx))+-32'sd2*KTES17R3400_KtestFpointers_run_test17r3_V_1);

                           end 
                          endcase
              if (reset)  kiwiKTES17R3400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX14)  kiwiKTES17R3400PC10nz_pc_export <= kiwiKTES17R3400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.KTES17R3400/1.0


       end 
      

 always   @(* )  begin 
       hpr_int_run_enable_DDX14 = 32'sd1;
       hpr_int_run_enable_DDX14 = (32'sd255==hpr_abend_syndrome);
       bondout0_REQ0 = 32'd0;
       bondout0_ACKRDY0 = 32'd0;
       bondout0_ADDR0 = 32'd0;
       bondout0_RWBAR0 = 32'd0;
       bondout0_WDATA0 = 64'd0;
       bondout0_LANES0 = 32'd0;
       bondout0_REQ1 = 32'd0;
       bondout0_ACKRDY1 = 32'd0;
       bondout0_ADDR1 = 32'd0;
       bondout0_RWBAR1 = 32'd0;
       bondout0_WDATA1 = 64'd0;
       bondout0_LANES1 = 32'd0;
       bondout1_REQ0 = 32'd0;
       bondout1_ACKRDY0 = 32'd0;
       bondout1_ADDR0 = 32'd0;
       bondout1_RWBAR0 = 32'd0;
       bondout1_WDATA0 = 64'd0;
       bondout1_LANES0 = 32'd0;
       bondout1_REQ1 = 32'd0;
       bondout1_ACKRDY1 = 32'd0;
       bondout1_ADDR1 = 32'd0;
       bondout1_RWBAR1 = 32'd0;
       bondout1_WDATA1 = 64'd0;
       bondout1_LANES1 = 32'd0;
       end 
      

assign hprpin500240x10 = 32'sd4000+((rtl_unsigned_bitextract0(64'h0)==32'h1/*1:USA18*/)? A_SINT_CC_MAPR10NoCE1_myfoo10: ((rtl_unsigned_bitextract0(64'h0)==32'h0
/*0:USA18*/)? 32'sh2: 32'bx));

assign hprpin500249x10 = $signed(-32'sd2000+((64'h0==32'h1/*1:USA14*/)? A_SINT_CC_MAPR10NoCE1_myfoo10: ((64'h0==32'h0/*0:USA14*/)? A_SINT_CC_MAPR10NoCE0_myfoo10
: 32'bx)));

assign hprpin500267x10 = ((rtl_unsigned_bitextract0(64'h0)==32'h1/*1:USA18*/)? A_SINT_CC_MAPR10NoCE1_myfoo10: ((rtl_unsigned_bitextract0(64'h0)==32'h0/*0:USA18*/)? A_SINT_CC_MAPR10NoCE0_myfoo10
: 32'bx))+32'sd2*$signed(32'sd500+KTES17R3400_KtestFpointers_run_test17r3_V_1);

// Structural Resource (FU) inventory for test17r3:// 1 vectors of width 4
// 1 vectors of width 64
// 3 vectors of width 32
// 2 vectors of width 1
// Total state bits in module = 166 bits.
// 96 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:45:17
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test17r3.exe -kiwic-kcode-dump=enable -sim=1800 -vnl test17r3.v -kiwic-autodispose=enable -kiwic-cil-dump=combined -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*
//| Class          | Style   | Dir Style                                                          | Timing Target | Method                      | UID            | Skip  |
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*
//| KtestFpointers | MM_root | DS_normal self-start/directorate-startmode, finish/directorate-en\ |               | KtestFpointers.run_test17r3 | runtest17r3_10 | false |
//|                |         | dmode, enable/directorate-pc-export                                |               |                             |                |       |
//*----------------+---------+--------------------------------------------------------------------+---------------+-----------------------------+----------------+-------*

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
//root_compiler: method compile: entry point. Method name=KtestFpointers.run_test17r3 uid=runtest17r3_10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=runtest17r3_10 full_idl=KtestFpointers.run_test17r3
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
//   kiwic-cil-dump=combined
//
//
//   kiwic-kcode-dump=enable
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
//   srcfile=test17r3.exe
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
//Bondout Load/Store (and other) Ports 'Res2 Preliminary'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

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
//PC codings points for kiwiKTES17R3400PC10 
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiKTES17R3400PC10"   | 829 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:kiwiKTES17R3400PC10"   | 828 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiKTES17R3400PC10"   | 827 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 6    |
//| XU32'8:"8:kiwiKTES17R3400PC10"   | 825 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 4    |
//| XU32'16:"16:kiwiKTES17R3400PC10" | 823 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 5    |
//| XU32'16:"16:kiwiKTES17R3400PC10" | 824 | 4       | hwm=0.0.0   | 4    |        | -     | -   | -    |
//| XU32'32:"32:kiwiKTES17R3400PC10" | 822 | 5       | hwm=0.0.0   | 5    |        | -     | -   | 7    |
//| XU32'4:"4:kiwiKTES17R3400PC10"   | 826 | 6       | hwm=0.0.0   | 6    |        | -     | -   | 3    |
//| XU32'64:"64:kiwiKTES17R3400PC10" | 821 | 7       | hwm=0.0.0   | 7    |        | -     | -   | 6    |
//*----------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'0:"0:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'0:"0:kiwiKTES17R3400PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                  |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                                       |
//| F0   | E829 | R0 DATA |                                                                                                                                                       |
//| F0+E | E829 | W0 DATA | @_SINT/CC/MAPR10NoCE1_myfoo10write(E1) fastspilldup10write(E2) KTES17R3400.KtestFpointers.run_test17r3.V_1write(S32'2000) @_SINT/CC/MAPR10NoCE0_myfo\ |
//|      |      |         | o10write(S32'2) KtestFpointers.zoutwrite(S32'0)  PLI:Kiwi Scientific Acce...                                                                          |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'1:"1:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'1:"1:kiwiKTES17R3400PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F1   | -    | R0 CTRL |                                        |
//| F1   | E828 | R0 DATA |                                        |
//| F1+E | E828 | W0 DATA | @_SINT/CC/MAPR10NoCE0_myfoo10write(E3) |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'2:"2:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'2:"2:kiwiKTES17R3400PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F2   | -    | R0 CTRL |                              |
//| F2   | E827 | R0 DATA |                              |
//| F2+E | E827 | W0 DATA | KtestFpointers.zoutwrite(E4) |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'8:"8:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'8:"8:kiwiKTES17R3400PC10"
//*------+------+---------+------------------------------------*
//| pc   | eno  | Phaser  | Work                               |
//*------+------+---------+------------------------------------*
//| F3   | -    | R0 CTRL |                                    |
//| F3   | E825 | R0 DATA | KtestFpointers.zoutargread(<NONE>) |
//| F3+E | E825 | W0 DATA |  PLI:Set zout to %d                |
//*------+------+---------+------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'16:"16:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'16:"16:kiwiKTES17R3400PC10"
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                   |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                        |
//| F4   | E824 | R0 DATA |                                                                                                                        |
//| F4+E | E824 | W0 DATA | KTES17R3400.KtestFpointers.run_test17r3.V_1write(E5)  PLI:GSAI:hpr_sysexit  PLI:Kiwi Scientific Acce...                |
//| F4   | E823 | R0 DATA |                                                                                                                        |
//| F4+E | E823 | W0 DATA | @_SINT/CC/MAPR10NoCE1_myfoo10write(E6) fastspilldup10write(E2) KTES17R3400.KtestFpointers.run_test17r3.V_1write(E5) @\ |
//|      |      |         | System_Object/CC/MAPR12NoCE1_closure10write(C64u({SC:c15,0}))                                                          |
//*------+------+---------+------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'32:"32:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'32:"32:kiwiKTES17R3400PC10"
//*------+------+---------+----------------------------------------*
//| pc   | eno  | Phaser  | Work                                   |
//*------+------+---------+----------------------------------------*
//| F5   | -    | R0 CTRL |                                        |
//| F5   | E822 | R0 DATA |                                        |
//| F5+E | E822 | W0 DATA | @_SINT/CC/MAPR10NoCE0_myfoo10write(E7) |
//*------+------+---------+----------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'4:"4:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'4:"4:kiwiKTES17R3400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F6   | -    | R0 CTRL |      |
//| F6   | E826 | R0 DATA |      |
//| F6+E | E826 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'64:"64:kiwiKTES17R3400PC10"
//res2: scon1: nopipeline: Thread=kiwiKTES17R3400PC10 state=XU32'64:"64:kiwiKTES17R3400PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F7   | -    | R0 CTRL |                              |
//| F7   | E821 | R0 DATA |                              |
//| F7+E | E821 | W0 DATA | KtestFpointers.zoutwrite(E8) |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout1 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Final'
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| AddressSpace | Name       | Protocol                      | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | ClockDom |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*
//| bondout0     | bondout0_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout0     | bondout0_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_0 | HFAST1(PD_halfduplex)_RR1_AR1 | 33554432   | 4194304           | 22     | 64     | 8     | 8         | clk      |
//| bondout1     | bondout1_1 | HFAST1(PD_halfduplex)_RR1_AR1 | (33554432) | 4194304           | 22     | 64     | 8     | 8         | clk      |
//*--------------+------------+-------------------------------+------------+-------------------+--------+--------+-------+-----------+----------*

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from bondout memory map manager:::
//Memory Map Offchip
//
//Bondout/Offchip Memory Map - Lane addressed. = Nothing to Report
//

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= C(4000+(COND((Cu(CVT(C64u)({SC:c15,0})))==XU32'1:"1:USA18", @_SINT/CC/MAPR10NoCE1_myfoo10, COND((Cu(CVT(C64u)({SC:c15,0})))==XU32'0:"0:USA18", S32'2, *UNDEF))))
//
//
//  E2 =.= Cu(CVT(C64u)({SC:c15,0}))
//
//
//  E3 =.= C(4000+(COND(XU32'1:"1:fastspilldup10"==fastspilldup10, @_SINT/CC/MAPR10NoCE1_myfoo10, COND(XU32'0:"0:fastspilldup10"==fastspilldup10, @_SINT/CC/MAPR10NoCE0_myfoo10, *UNDEF))))
//
//
//  E4 =.= C(-2000+(COND((CVT(C64u)({SC:c15,0}))==XU32'1:"1:USA14", @_SINT/CC/MAPR10NoCE1_myfoo10, COND((CVT(C64u)({SC:c15,0}))==XU32'0:"0:USA14", @_SINT/CC/MAPR10NoCE0_myfoo10, *UNDEF))))
//
//
//  E5 =.= C(500+KTES17R3400.KtestFpointers.run_test17r3.V_1)
//
//
//  E6 =.= C((COND((Cu(CVT(C64u)({SC:c15,0})))==XU32'1:"1:USA18", @_SINT/CC/MAPR10NoCE1_myfoo10, COND((Cu(CVT(C64u)({SC:c15,0})))==XU32'0:"0:USA18", @_SINT/CC/MAPR10NoCE0_myfoo10, *UNDEF)))+2*(C(500+KTES17R3400.KtestFpointers.run_test17r3.V_1)))
//
//
//  E7 =.= C((COND(XU32'1:"1:fastspilldup10"==fastspilldup10, @_SINT/CC/MAPR10NoCE1_myfoo10, COND(XU32'0:"0:fastspilldup10"==fastspilldup10, @_SINT/CC/MAPR10NoCE0_myfoo10, *UNDEF)))+2*KTES17R3400.KtestFpointers.run_test17r3.V_1)
//
//
//  E8 =.= C(KTES17R3400.KtestFpointers.run_test17r3.V_1+(COND(XU32'1:"1:USA16"==(CVT(C64u)(@System_Object/CC/MAPR12NoCE1_closure10)), @_SINT/CC/MAPR10NoCE1_myfoo10, COND(XU32'0:"0:USA16"==(CVT(C64u)(@System_Object/CC/MAPR12NoCE1_closure10)), @_SINT/CC/MAPR10NoCE0_myfoo10, *UNDEF)))+-2*KTES17R3400.KtestFpointers.run_test17r3.V_1)
//
//
//  E9 =.= (C(500+KTES17R3400.KtestFpointers.run_test17r3.V_1))>=S32'5000
//
//
//  E10 =.= (C(500+KTES17R3400.KtestFpointers.run_test17r3.V_1))<S32'5000
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test17r3 to test17r3

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test17r3:
//1 vectors of width 4
//
//1 vectors of width 64
//
//3 vectors of width 32
//
//2 vectors of width 1
//
//Total state bits in module = 166 bits.
//
//96 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread KtestFpointers.run_test17r3 uid=runtest17r3_10 has 25 CIL instructions in 6 basic blocks
//Thread KtestFpointers.run_test17r3 uid=runtest17r3_10 has 28 CIL instructions in 6 basic blocks
//Thread mpc10 has 8 bevelab control states (pauses)
//Reindexed thread kiwiKTES17R3400PC10 with 8 minor control states
// eof (HPR L/S Verilog)
