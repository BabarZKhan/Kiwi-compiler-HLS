

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:45:36
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.Test19_Server1 test19.exe -bondout-loadstore-port-count=0 -vnl Test19_Server1.v -vnl-kandr=disable -root Test19_Server1;Test19_Server1.setget_pixel;Test19_Server1.get_id;Test19_Server1.start
`timescale 1ns/1ns


module Test19_Server1(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets14 */
    output [7:0] hpr_unary_leds_start,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets12 */
output [7:0] hpr_unary_leds_get_id,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_setget_pixel,
    
/* portgroup= abstractionName=AUTOMETA_start pi_name=start */
input start_req,
    output reg start_ack,
    
/* portgroup= abstractionName=AUTOMETA_get_id pi_name=get_id */
input get_id_req,
    output reg get_id_ack,
    output reg signed [15:0] get_id_return,
    
/* portgroup= abstractionName=AUTOMETA_setget_pixel pi_name=setget_pixel */
input setget_pixel_req,
    output reg setget_pixel_ack,
    output reg [7:0] setget_pixel_return,
    input [7:0] setget_pixel_wdata,
    input setget_pixel_readf,
    input [31:0] setget_pixel_ayy,
    input [31:0] setget_pixel_axx,
    
/* portgroup= abstractionName=res2-directornets */
output reg [3:0] kiwiTESTIXEL402PC10nz_pc_export);

function [7:0] rtl_unsigned_bitextract4;
   input [31:0] arg;
   rtl_unsigned_bitextract4 = $unsigned(arg[7:0]);
   endfunction


function [31:0] rtl_unsigned_bitextract2;
   input [63:0] arg;
   rtl_unsigned_bitextract2 = $unsigned(arg[31:0]);
   endfunction


function  rtl_unsigned_bitextract1;
   input [31:0] arg;
   rtl_unsigned_bitextract1 = $unsigned(arg[0:0]);
   endfunction


function [31:0] rtl_unsigned_extend3;
   input [7:0] arg;
   rtl_unsigned_extend3 = { 24'b0, arg[7:0] };
   endfunction


function [31:0] rtl_unsigned_extend0;
   input argbit;
   rtl_unsigned_extend0 = { 31'b0, argbit };
   endfunction

// abstractionName=L2590-vg pi_name=net2batchdirectoratenets14
  reg hpr_int_run_enable_start;
  reg hpr_fsm_idle_start;
  reg hpr_fsm_ending_start;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets12
  reg hpr_int_run_enable_get_id;
  reg hpr_fsm_idle_get_id;
  reg hpr_fsm_ending_get_id;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_setget_pixel;
  reg hpr_fsm_idle_setget_pixel;
  reg hpr_fsm_ending_setget_pixel;
// abstractionName=kiwicmainnets10
  reg [31:0] Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_V_0;
  reg [7:0] Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256;
// abstractionName=res2-contacts pi_name=CV_SP_SSRAM_FL1
  wire [7:0] A_8_US_CC_SCALbx10_ARA0_rdata;
  reg [11:0] A_8_US_CC_SCALbx10_ARA0_addr;
  reg A_8_US_CC_SCALbx10_ARA0_wen;
  reg A_8_US_CC_SCALbx10_ARA0_ren;
  reg [7:0] A_8_US_CC_SCALbx10_ARA0_wdata;
// abstractionName=res2-morenets
  reg [7:0] Z8USCCSCALbx10ARA0rdatah10hold;
  reg Z8USCCSCALbx10ARA0rdatah10shot0;
  reg [2:0] kiwiTESTIXEL402PC10nz;
 always   @(* )  begin 
       hpr_int_run_enable_start = 32'sd1;
       hpr_fsm_ending_start = 1'd1;
       hpr_fsm_idle_start = 1'd1;
       hpr_int_run_enable_start = (start_req || !hpr_fsm_idle_start) && (32'sd255==hpr_abend_syndrome);
       hpr_int_run_enable_get_id = 32'sd1;
       hpr_fsm_ending_get_id = 1'd1;
       hpr_fsm_idle_get_id = 1'd1;
       hpr_int_run_enable_get_id = (get_id_req || !hpr_fsm_idle_get_id) && (32'sd255==hpr_abend_syndrome);
       A_8_US_CC_SCALbx10_ARA0_addr = 32'sd0;
       A_8_US_CC_SCALbx10_ARA0_wdata = 32'sd0;
       hpr_fsm_ending_setget_pixel = 32'sd0;
       hpr_fsm_idle_setget_pixel = 32'sd1;
       A_8_US_CC_SCALbx10_ARA0_wen = 32'sd0;
       A_8_US_CC_SCALbx10_ARA0_ren = 32'sd0;
       hpr_int_run_enable_setget_pixel = 32'sd1;

      case (kiwiTESTIXEL402PC10nz)
          32'h0/*0:kiwiTESTIXEL402PC10nz*/: if (!(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(setget_pixel_readf)))))  begin 
                   A_8_US_CC_SCALbx10_ARA0_addr = rtl_unsigned_bitextract2(setget_pixel_ayy+32'd128*setget_pixel_axx);
                   A_8_US_CC_SCALbx10_ARA0_wdata = rtl_unsigned_extend3(rtl_unsigned_bitextract4(rtl_unsigned_extend3(rtl_unsigned_bitextract4(setget_pixel_wdata
                  ))));

                   end 
                  
          32'h5/*5:kiwiTESTIXEL402PC10nz*/:  A_8_US_CC_SCALbx10_ARA0_addr = Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_V_0
          ;

      endcase
       hpr_fsm_ending_setget_pixel = (32'h4/*4:kiwiTESTIXEL402PC10nz*/==kiwiTESTIXEL402PC10nz);
       hpr_fsm_idle_setget_pixel = (32'h0/*0:kiwiTESTIXEL402PC10nz*/==kiwiTESTIXEL402PC10nz);
      if (hpr_int_run_enable_setget_pixel)  begin 
               A_8_US_CC_SCALbx10_ARA0_wen = !rtl_unsigned_extend0(rtl_unsigned_bitextract1(setget_pixel_readf)) && (32'h0/*0:kiwiTESTIXEL402PC10nz*/==
              kiwiTESTIXEL402PC10nz);

               A_8_US_CC_SCALbx10_ARA0_ren = ((32'h5/*5:kiwiTESTIXEL402PC10nz*/==kiwiTESTIXEL402PC10nz)? 32'd1: 32'd0);
               end 
               hpr_int_run_enable_setget_pixel = (setget_pixel_req || !hpr_fsm_idle_setget_pixel) && (32'sd255==hpr_abend_syndrome);
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TESTTART400/1.0
      if (!reset)  begin if (hpr_int_run_enable_start) $display("Server started.");
               end 
          if (reset)  hpr_abend_syndrome <= 32'sd255;
           else if (hpr_int_run_enable_start)  begin if (hpr_int_run_enable_start)  start_ack <= hpr_fsm_ending_start;
                   end 
              //End structure cvtToVerilogkiwi.TESTTART400/1.0


      //Start structure cvtToVerilogkiwi.TESTT_ID401/1.0
      if (reset)  get_id_return <= 32'd0;
           else if (hpr_int_run_enable_get_id)  get_id_return <= 32'sh4e8;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
           else if (hpr_int_run_enable_get_id)  begin if (hpr_int_run_enable_get_id)  get_id_ack <= hpr_fsm_ending_get_id;
                   end 
              //End structure cvtToVerilogkiwi.TESTT_ID401/1.0


      //Start structure cvtToVerilogkiwi.TESTIXEL402/1.0
      if (reset)  begin 
               setget_pixel_return <= 32'd0;
               Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256 <= 32'd0;
               Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_V_0 <= 32'd0;
               kiwiTESTIXEL402PC10nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_setget_pixel) 
              case (kiwiTESTIXEL402PC10nz)
                  32'h1/*1:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  kiwiTESTIXEL402PC10nz <= 32'h4/*4:kiwiTESTIXEL402PC10nz*/;
                      
                  32'h2/*2:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  kiwiTESTIXEL402PC10nz <= 32'h3/*3:kiwiTESTIXEL402PC10nz*/;
                      
                  32'h3/*3:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  kiwiTESTIXEL402PC10nz <= 32'h5/*5:kiwiTESTIXEL402PC10nz*/;
                      
                  32'h0/*0:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  begin 
                          if (!rtl_unsigned_extend0(rtl_unsigned_bitextract1(setget_pixel_readf))) $display("Set pixel (%1d,%1d) to %1d"
                              , setget_pixel_axx, setget_pixel_ayy, rtl_unsigned_extend3(rtl_unsigned_bitextract4(setget_pixel_wdata)));
                               else  begin 
                                   kiwiTESTIXEL402PC10nz <= 32'h2/*2:kiwiTESTIXEL402PC10nz*/;
                                   Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256 <= 32'h0;
                                   Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_V_0 <= rtl_unsigned_bitextract2(setget_pixel_ayy
                                  +32'd128*setget_pixel_axx);

                                   end 
                                  if (!(!(!rtl_unsigned_extend0(rtl_unsigned_bitextract1(setget_pixel_readf)))))  begin 
                                   kiwiTESTIXEL402PC10nz <= 32'h1/*1:kiwiTESTIXEL402PC10nz*/;
                                   Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256 <= rtl_unsigned_extend3(rtl_unsigned_bitextract4(rtl_unsigned_extend3(rtl_unsigned_bitextract4(setget_pixel_wdata
                                  ))));

                                   Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_V_0 <= rtl_unsigned_bitextract2(setget_pixel_ayy
                                  +32'd128*setget_pixel_axx);

                                   end 
                                   end 
                          
                  32'h5/*5:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  kiwiTESTIXEL402PC10nz <= 32'h6/*6:kiwiTESTIXEL402PC10nz*/;
                      
                  32'h4/*4:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  begin 
                           kiwiTESTIXEL402PC10nz <= 32'h0/*0:kiwiTESTIXEL402PC10nz*/;
                           setget_pixel_return <= Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256;
                           end 
                          
                  32'h6/*6:kiwiTESTIXEL402PC10nz*/: if (hpr_int_run_enable_setget_pixel)  begin 
                           kiwiTESTIXEL402PC10nz <= 32'h4/*4:kiwiTESTIXEL402PC10nz*/;
                           Test19_Server1_TESTIXEL402_Test19_Server1_setget_pixel_SPILL_256 <= ((32'h6/*6:kiwiTESTIXEL402PC10nz*/==kiwiTESTIXEL402PC10nz
                          )? rtl_unsigned_extend3(rtl_unsigned_bitextract4(A_8_US_CC_SCALbx10_ARA0_rdata)): rtl_unsigned_extend3(rtl_unsigned_bitextract4(Z8USCCSCALbx10ARA0rdatah10hold
                          )));

                           end 
                          endcase
              if (reset)  begin 
               kiwiTESTIXEL402PC10nz_pc_export <= 32'd0;
               Z8USCCSCALbx10ARA0rdatah10hold <= 32'd0;
               Z8USCCSCALbx10ARA0rdatah10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_setget_pixel)  begin 
                  if (Z8USCCSCALbx10ARA0rdatah10shot0)  Z8USCCSCALbx10ARA0rdatah10hold <= A_8_US_CC_SCALbx10_ARA0_rdata;
                       kiwiTESTIXEL402PC10nz_pc_export <= kiwiTESTIXEL402PC10nz;
                   Z8USCCSCALbx10ARA0rdatah10shot0 <= (32'h5/*5:kiwiTESTIXEL402PC10nz*/==kiwiTESTIXEL402PC10nz);
                   end 
                  if (reset)  hpr_abend_syndrome <= 32'sd255;
           else if (hpr_int_run_enable_setget_pixel)  begin if (hpr_int_run_enable_setget_pixel)  setget_pixel_ack <= hpr_fsm_ending_setget_pixel
                  ;

                   end 
              //End structure cvtToVerilogkiwi.TESTIXEL402/1.0


       end 
      

  CV_SP_SSRAM_FL1 #(
        .DATA_WIDTH(32'sd8),
        .ADDR_WIDTH(32'sd12),
        .WORDS(32'sd4096),
        .LANE_WIDTH(32'sd8
),
        .trace_me(32'sd0)) A_8_US_CC_SCALbx10_ARA0(
        .clk(clk),
        .reset(reset),
        .rdata(A_8_US_CC_SCALbx10_ARA0_rdata
),
        .addr(A_8_US_CC_SCALbx10_ARA0_addr),
        .wen(A_8_US_CC_SCALbx10_ARA0_wen),
        .ren(A_8_US_CC_SCALbx10_ARA0_ren),
        .wdata(A_8_US_CC_SCALbx10_ARA0_wdata
));

// Structural Resource (FU) inventory for Test19_Server1:// 1 vectors of width 3
// 12 vectors of width 1
// 3 vectors of width 8
// 1 vectors of width 12
// 1 vectors of width 32
// Total state bits in module = 83 bits.
// 8 continuously assigned (wire/non-state) bits 
//   cell CV_SP_SSRAM_FL1 count=1
// Total number of leaf cells = 1
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:45:33
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bondout-loadstore-port-count=0 -obj-dir-name=. -log-dir-name=obj_d.Test19_Server1 test19.exe -bondout-loadstore-port-count=0 -vnl Test19_Server1.v -vnl-kandr=disable -root Test19_Server1;Test19_Server1.setget_pixel;Test19_Server1.get_id;Test19_Server1.start


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

//Report from IP-XACT input/output:::
//Write IP-XACT abstractionDefinition for setget_pixel_rtl to AUTOMETA_setget_pixel_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT busDefinition for setget_pixel to AUTOMETA_setget_pixel

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT abstractionDefinition for get_id_rtl to AUTOMETA_get_id_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT busDefinition for get_id to AUTOMETA_get_id

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT abstractionDefinition for start_rtl to AUTOMETA_start_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT busDefinition for start to AUTOMETA_start

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
//KiwiC: front end input processing of class Test19_Server1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test19_Server1..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=Test19_Server1..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Test19_Server1  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'Test19_Server1' tid=TESTVER1403 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'Test19_Server1' tid=TESTVER1403
//
//
//root_compiler class done: Test19_Server1
//
//
//KiwiC: front end input processing of class Test19_Server1.setget_pixel  wonky=Test19_Server1 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test19_Server1.setget_pixel uid=setgetpixel10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=setgetpixel10 full_idl=Test19_Server1.setget_pixel: A remote-callable method
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Test19_Server1.get_id  wonky=Test19_Server1 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test19_Server1.get_id uid=getid10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=getid10 full_idl=Test19_Server1.get_id: A remote-callable method
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Test19_Server1.start  wonky=Test19_Server1 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Test19_Server1.start uid=start10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=start10 full_idl=Test19_Server1.start: A remote-callable method
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
//   root=Test19_Server1;Test19_Server1.setget_pixel;Test19_Server1.get_id;Test19_Server1.start
//
//
//   srcfile=test19.exe
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
//PC codings points for mpc16 
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:mpc16" | 808 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 0    |
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc16 state=XU32'0:"0:mpc16"
//res2: scon1: nopipeline: Thread=mpc16 state=XU32'0:"0:mpc16"
//*------+------+---------+----------------------*
//| pc   | eno  | Phaser  | Work                 |
//*------+------+---------+----------------------*
//| F0   | -    | R0 CTRL |                      |
//| F0   | E808 | R0 DATA |                      |
//| F0+E | E808 | W0 DATA |  PLI:Server started. |
//*------+------+---------+----------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for mpc14 
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause    | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:mpc14" | 809 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 0    |
//*------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc14 state=XU32'0:"0:mpc14"
//res2: scon1: nopipeline: Thread=mpc14 state=XU32'0:"0:mpc14"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F0   | -    | R0 CTRL |                              |
//| F0   | E809 | R0 DATA |                              |
//| F0+E | E809 | W0 DATA | get_id.returnwrite(S32'1256) |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for kiwiTESTIXEL402PC10 
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                  | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTESTIXEL402PC10" | 814 | 0       | hwm=0.0.1   | 0    |        | 1     | 1   | 4    |
//| XU32'0:"0:kiwiTESTIXEL402PC10" | 815 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 2    |
//| XU32'2:"2:kiwiTESTIXEL402PC10" | 812 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 3    |
//| XU32'4:"4:kiwiTESTIXEL402PC10" | 811 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 5    |
//| XU32'1:"1:kiwiTESTIXEL402PC10" | 813 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 0    |
//| XU32'8:"8:kiwiTESTIXEL402PC10" | 810 | 5       | hwm=0.1.0   | 6    |        | 6     | 6   | 4    |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'0:"0:kiwiTESTIXEL402PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'0:"0:kiwiTESTIXEL402PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                       |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL | setget_pixel.readfargread(<NONE>)                                                                                                          |
//| F0   | E815 | R0 DATA | setget_pixel.ayyargread(<NONE>) setget_pixel.axxargread(<NONE>)                                                                            |
//| F0+E | E815 | W0 DATA | Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel.V_0write(E1) Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel._SPILL.256writ\ |
//|      |      |         | e(U32'0)                                                                                                                                   |
//| F0   | E814 | R0 DATA | setget_pixel.ayyargread(<NONE>) setget_pixel.axxargread(<NONE>) setget_pixel.wdataargread(<NONE>)                                          |
//| F0+E | E814 | W0 DATA | Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel.V_0write(E1) Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel._SPILL.256writ\ |
//|      |      |         | e(E2) @8_US/CC/SCALbx10_ARA0_write(E3, E2)  PLI:Set pixel (%u,%u) to...                                                                    |
//| F1   | E814 | W1 DATA |                                                                                                                                            |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'2:"2:kiwiTESTIXEL402PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'2:"2:kiwiTESTIXEL402PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F2   | -    | R0 CTRL |      |
//| F2   | E812 | R0 DATA |      |
//| F2+E | E812 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'4:"4:kiwiTESTIXEL402PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'4:"4:kiwiTESTIXEL402PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F3   | -    | R0 CTRL |      |
//| F3   | E811 | R0 DATA |      |
//| F3+E | E811 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'1:"1:kiwiTESTIXEL402PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'1:"1:kiwiTESTIXEL402PC10"
//*------+------+---------+------------------------------*
//| pc   | eno  | Phaser  | Work                         |
//*------+------+---------+------------------------------*
//| F4   | -    | R0 CTRL |                              |
//| F4   | E813 | R0 DATA |                              |
//| F4+E | E813 | W0 DATA | setget_pixel.returnwrite(E4) |
//*------+------+---------+------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'8:"8:kiwiTESTIXEL402PC10"
//res2: scon1: nopipeline: Thread=kiwiTESTIXEL402PC10 state=XU32'8:"8:kiwiTESTIXEL402PC10"
//*------+------+---------+----------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                       |
//*------+------+---------+----------------------------------------------------------------------------*
//| F5   | -    | R0 CTRL |                                                                            |
//| F5   | E810 | R0 DATA | @8_US/CC/SCALbx10_ARA0_read(E5)                                            |
//| F6   | E810 | R1 DATA |                                                                            |
//| F6+E | E810 | W0 DATA | Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel._SPILL.256write(E6) |
//*------+------+---------+----------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= Cu((Cu(setget_pixel.ayy))+128*(Cu(setget_pixel.axx)))
//
//
//  E2 =.= Cu(C8u(Cu(C8u(setget_pixel.wdata))))
//
//
//  E3 =.= CVT(Cu)((Cu(setget_pixel.ayy))+128*(Cu(setget_pixel.axx)))
//
//
//  E4 =.= Cu(Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel._SPILL.256)
//
//
//  E5 =.= CVT(Cu)(Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel.V_0)
//
//
//  E6 =.= Cu(C8u(@8_US/CC/SCALbx10_ARA0[CVT(Cu)(Test19_Server1.TESTIXEL402.Test19_Server1.setget_pixel.V_0)]))
//
//
//  E7 =.= |-|(Cu(C1u(setget_pixel.readf)))
//
//
//  E8 =.= !(Cu(C1u(setget_pixel.readf)))
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for Test19_Server1 to Test19_Server1

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for Test19_Server1:
//1 vectors of width 3
//
//12 vectors of width 1
//
//3 vectors of width 8
//
//1 vectors of width 12
//
//1 vectors of width 32
//
//Total state bits in module = 83 bits.
//
//8 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor14 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread Test19_Server1..cctor uid=cctor10 has 4 CIL instructions in 1 basic blocks
//Thread Test19_Server1.setget_pixel uid=setgetpixel10 has 20 CIL instructions in 6 basic blocks
//Thread Test19_Server1.get_id uid=getid10 has 1 CIL instructions in 1 basic blocks
//Thread Test19_Server1.start uid=start10 has 2 CIL instructions in 1 basic blocks
//Thread mpc12 has 5 bevelab control states (pauses)
//Thread mpc14 has 1 bevelab control states (pauses)
//Thread mpc16 has 1 bevelab control states (pauses)
//Reindexed thread mpc16 with 1 minor control states
//Reindexed thread mpc14 with 1 minor control states
//Reindexed thread kiwiTESTIXEL402PC10 with 7 minor control states
// eof (HPR L/S Verilog)
