

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 06:45:32
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test18.exe -root Tog1;Tog2;Tog3;Tog3.Main -sim=1800 -vnl test18.v -vnl-preserve-sequencer=true -vnl-ifshare on -give-backtrace -report-each-step
`timescale 1ns/1ns


module test18(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    output reg [7:0] hpr_abend_syndrome,
    output [7:0] hpr_unary_leds_DDX20,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output signed [11:0] Tog3_zout,
    input signed [10:0] Tog3_y,
    input signed [9:0] Tog3_x,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] kiwiTOG3MAIN400PC10nz_pc_export,
    
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
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_DDX20;
// abstractionName=kiwicmainnets10
  reg signed [31:0] Tog3_TOG3MAIN400_Tog3_Main_V_0;
  reg signed [31:0] Tog3_carly;
  reg signed [31:0] Tog2_boris;
  reg signed [31:0] Tog1_andy;
// abstractionName=res2-morenets
  reg [2:0] kiwiTOG3MAIN400PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.TOG3MAIN400/1.0
      if (reset)  begin 
               kiwiTOG3MAIN400PC10nz <= 32'd0;
               Tog1_andy <= 32'd0;
               Tog2_boris <= 32'd0;
               Tog3_carly <= 32'd0;
               Tog3_TOG3MAIN400_Tog3_Main_V_0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_DDX20) 
              case (kiwiTOG3MAIN400PC10nz)
                  32'h0/*0:kiwiTOG3MAIN400PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          $display("The answer is %1d at the start", 32'sd1111);
                          $display("k1-a: Shall I compare");
                          $display("k1-b: Thou art more lovely");
                           kiwiTOG3MAIN400PC10nz <= 32'h3/*3:kiwiTOG3MAIN400PC10nz*/;
                           Tog1_andy <= 32'sha;
                           Tog2_boris <= 32'sh64;
                           Tog3_carly <= 32'sh3e8;
                           Tog3_TOG3MAIN400_Tog3_Main_V_0 <= 32'sh3e8;
                           end 
                          
                  32'h2/*2:kiwiTOG3MAIN400PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          $display("Tog3-a: Sometimes too hot");
                          $display("Tog3-b: And oft is his gold complexion dimm'd");
                           kiwiTOG3MAIN400PC10nz <= 32'h4/*4:kiwiTOG3MAIN400PC10nz*/;
                           end 
                          
                  32'h1/*1:kiwiTOG3MAIN400PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          $display("k2-a: Rough winds may shake");
                          $display("k2-b: And Summer's lease hath all too short a fig");
                           kiwiTOG3MAIN400PC10nz <= 32'h2/*2:kiwiTOG3MAIN400PC10nz*/;
                           end 
                          
                  32'h3/*3:kiwiTOG3MAIN400PC10nz*/: if (hpr_int_run_enable_DDX20)  kiwiTOG3MAIN400PC10nz <= 32'h1/*1:kiwiTOG3MAIN400PC10nz*/;
                      
                  32'h4/*4:kiwiTOG3MAIN400PC10nz*/: if (hpr_int_run_enable_DDX20)  begin 
                          if (($signed(32'sd1000+Tog3_TOG3MAIN400_Tog3_Main_V_0)>=32'sd2000))  begin 
                                  $display("The answer is %1d at the end", 32'sd1+Tog1_andy+Tog2_boris+Tog3_carly);
                                  $finish(32'sd0);
                                   end 
                                   else  begin 
                                  $display("The answer is %1d at the end", 32'sd1+Tog1_andy+Tog2_boris+Tog3_carly);
                                  $display("The answer is %1d at the start", 32'sd1+Tog1_andy+Tog2_boris+Tog3_carly);
                                  $display("k1-a: Shall I compare");
                                  $display("k1-b: Thou art more lovely");
                                   kiwiTOG3MAIN400PC10nz <= 32'h3/*3:kiwiTOG3MAIN400PC10nz*/;
                                   Tog3_TOG3MAIN400_Tog3_Main_V_0 <= $signed(32'sd1000+Tog3_TOG3MAIN400_Tog3_Main_V_0);
                                   end 
                                  if (($signed(32'sd1000+Tog3_TOG3MAIN400_Tog3_Main_V_0)>=32'sd2000))  begin 
                                   kiwiTOG3MAIN400PC10nz <= 32'h5/*5:kiwiTOG3MAIN400PC10nz*/;
                                   Tog3_TOG3MAIN400_Tog3_Main_V_0 <= $signed(32'sd1000+Tog3_TOG3MAIN400_Tog3_Main_V_0);
                                   hpr_abend_syndrome <= 32'sd0;
                                   end 
                                   end 
                          endcase
              if (reset)  kiwiTOG3MAIN400PC10nz_pc_export <= 32'd0;
           else if (hpr_int_run_enable_DDX20)  kiwiTOG3MAIN400PC10nz_pc_export <= kiwiTOG3MAIN400PC10nz;
              if (reset)  hpr_abend_syndrome <= 32'sd255;
          //End structure cvtToVerilogkiwi.TOG3MAIN400/1.0


       end 
      

 always   @(* )  begin 
       hpr_int_run_enable_DDX20 = 32'sd1;
       hpr_int_run_enable_DDX20 = (32'sd255==hpr_abend_syndrome);
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
      

// Structural Resource (FU) inventory for test18:// 1 vectors of width 3
// 4 vectors of width 32
// 1 vectors of width 1
// Total state bits in module = 132 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 06:45:30
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable test18.exe -root Tog1;Tog2;Tog3;Tog3.Main -sim=1800 -vnl test18.v -vnl-preserve-sequencer=true -vnl-ifshare on -give-backtrace -report-each-step


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
//root_compiler: method compile: entry point. Method name=KiwiSystem.Kiwi..cctor uid=cctor18 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor18 full_idl=KiwiSystem.Kiwi..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
//
//
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor16 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor16 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class Tog1  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Tog1..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=Tog1..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Tog1  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'Tog1' tid=TOG1403 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'Tog1' tid=TOG1403
//
//
//root_compiler class done: Tog1
//
//
//KiwiC: front end input processing of class Tog2  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Tog2..cctor uid=cctor12 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor12 full_idl=Tog2..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Tog2  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'Tog2' tid=TOG2402 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'Tog2' tid=TOG2402
//
//
//root_compiler class done: Tog2
//
//
//KiwiC: front end input processing of class Tog3  wonky=NIL igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Tog3..cctor uid=cctor14 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor14 full_idl=Tog3..cctor
//
//
//Root method elaborated/compiled: specificf=S_root_method leftover=0/new + 0/prev
//
//
//KiwiC: front end input processing of class Tog3  wonky=NIL igrf=false
//
//
//kiwic root_compiler: start elaborating class 'Tog3' tid=TOG3401 with hls_style=Some MM_specific staticated=[]
//
//
//elaborating class 'Tog3' tid=TOG3401
//
//
//root_compiler class done: Tog3
//
//
//KiwiC: front end input processing of class Tog3.Main  wonky=Tog3 igrf=false
//
//
//root_compiler: method compile: entry point. Method name=Tog3.Main uid=Main10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Main10 full_idl=Tog3.Main
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
//   root=Tog1;Tog2;Tog3;Tog3.Main
//
//
//   srcfile=test18.exe
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
//PC codings points for kiwiTOG3MAIN400PC10 
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause                  | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:kiwiTOG3MAIN400PC10" | 811 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 3    |
//| XU32'2:"2:kiwiTOG3MAIN400PC10" | 809 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'4:"4:kiwiTOG3MAIN400PC10" | 808 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 4    |
//| XU32'1:"1:kiwiTOG3MAIN400PC10" | 810 | 3       | hwm=0.0.0   | 3    |        | -     | -   | 1    |
//| XU32'8:"8:kiwiTOG3MAIN400PC10" | 806 | 4       | hwm=0.0.0   | 4    |        | -     | -   | 3    |
//| XU32'8:"8:kiwiTOG3MAIN400PC10" | 807 | 4       | hwm=0.0.0   | 4    |        | -     | -   | -    |
//*--------------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'0:"0:kiwiTOG3MAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'0:"0:kiwiTOG3MAIN400PC10"
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                      |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                           |
//| F0   | E811 | R0 DATA |                                                                                                                                           |
//| F0+E | E811 | W0 DATA | Tog3.TOG3MAIN400.Tog3.Main.V_0write(S32'1000) Tog3.carlywrite(S32'1000) Tog2.boriswrite(S32'100) Tog1.andywrite(S32'10)  PLI:k1-b: Thou \ |
//|      |      |         | art more ...  PLI:k1-a: Shall I compar...  PLI:The answer is %d at ...                                                                    |
//*------+------+---------+-------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'2:"2:kiwiTOG3MAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'2:"2:kiwiTOG3MAIN400PC10"
//*------+------+---------+-----------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                      |
//*------+------+---------+-----------------------------------------------------------*
//| F1   | -    | R0 CTRL |                                                           |
//| F1   | E809 | R0 DATA |                                                           |
//| F1+E | E809 | W0 DATA |  PLI:k2-b: And Summer's l...  PLI:k2-a: Rough winds ma... |
//*------+------+---------+-----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'4:"4:kiwiTOG3MAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'4:"4:kiwiTOG3MAIN400PC10"
//*------+------+---------+-----------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                      |
//*------+------+---------+-----------------------------------------------------------*
//| F2   | -    | R0 CTRL |                                                           |
//| F2   | E808 | R0 DATA |                                                           |
//| F2+E | E808 | W0 DATA |  PLI:Tog3-b: And oft is h...  PLI:Tog3-a: Sometimes to... |
//*------+------+---------+-----------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'1:"1:kiwiTOG3MAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'1:"1:kiwiTOG3MAIN400PC10"
//*------+------+---------+------*
//| pc   | eno  | Phaser  | Work |
//*------+------+---------+------*
//| F3   | -    | R0 CTRL |      |
//| F3   | E810 | R0 DATA |      |
//| F3+E | E810 | W0 DATA |      |
//*------+------+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'8:"8:kiwiTOG3MAIN400PC10"
//res2: scon1: nopipeline: Thread=kiwiTOG3MAIN400PC10 state=XU32'8:"8:kiwiTOG3MAIN400PC10"
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                           |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*
//| F4   | -    | R0 CTRL |                                                                                                                                |
//| F4   | E807 | R0 DATA |                                                                                                                                |
//| F4+E | E807 | W0 DATA | Tog3.TOG3MAIN400.Tog3.Main.V_0write(E1)  PLI:GSAI:hpr_sysexit  PLI:The answer is %d at ...                                     |
//| F4   | E806 | R0 DATA |                                                                                                                                |
//| F4+E | E806 | W0 DATA | Tog3.TOG3MAIN400.Tog3.Main.V_0write(E1)  PLI:k1-b: Thou art more ...  PLI:k1-a: Shall I compar...  PLI:The answer is %d at ... |
//*------+------+---------+--------------------------------------------------------------------------------------------------------------------------------*

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
//  E1 =.= C(1000+Tog3.TOG3MAIN400.Tog3.Main.V_0)
//
//
//  E2 =.= (C(1000+Tog3.TOG3MAIN400.Tog3.Main.V_0))>=2000
//
//
//  E3 =.= (C(1000+Tog3.TOG3MAIN400.Tog3.Main.V_0))<2000
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test18 to test18

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for test18:
//1 vectors of width 3
//
//4 vectors of width 32
//
//1 vectors of width 1
//
//Total state bits in module = 132 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem.Kiwi..cctor uid=cctor18 has 2 CIL instructions in 1 basic blocks
//Thread System.BitConverter..cctor uid=cctor16 has 1 CIL instructions in 1 basic blocks
//Thread Tog1..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread Tog2..cctor uid=cctor12 has 1 CIL instructions in 1 basic blocks
//Thread Tog3..cctor uid=cctor14 has 1 CIL instructions in 1 basic blocks
//Thread Tog3.Main uid=Main10 has 19 CIL instructions in 7 basic blocks
//Thread mpc10 has 5 bevelab control states (pauses)
//Reindexed thread kiwiTOG3MAIN400PC10 with 5 minor control states
// eof (HPR L/S Verilog)
