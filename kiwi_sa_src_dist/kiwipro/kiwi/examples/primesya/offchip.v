

// CBG Orangepath HPR L/S System

// Verilog output file generated at 04/07/2018 12:00:43
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.7 : 15th June 2018 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe primesya.exe -vnl=offchip.v -vnl-rootmodname=DUT -kiwife-directorate-endmode=finish -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwic-register-colours=1 -diosim-vcd=vcd.vcd -res2-offchip-threshold=32
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=waypoints pi_name=kppIos10 */
    output reg [639:0] KppWaypoint0,
    output reg [639:0] KppWaypoint1,
    
/* portgroup= abstractionName=kiwicmiscio10 */
output signed [31:0] ksubsResultHi,
    output signed [31:0] ksubsResultLo,
    input signed [31:0] ksubsGpioSwitches,
    input signed [31:0] ksubsMiscReg0,
    output signed [31:0] ksubsMiscMon0,
    input [31:0] volume,
    output reg signed [31:0] ksubsDesignSerialNumber,
    output reg signed [31:0] edesign,
    output reg signed [31:0] evariant,
    output reg signed [31:0] elimit,
    output reg [31:0] count,
    output reg [7:0] ksubsAbendSyndrome,
    output reg [7:0] ksubsGpioLeds,
    output reg [7:0] ksubsManualWaypoint,
    
/* portgroup= abstractionName=res2-directornets */
output reg [2:0] primesya10PC10nz_pc_export,
    
/* portgroup= abstractionName=HFAST1 pi_name=bondout0_0 */
output reg bondout0_0_OPREQ,
    input bondout0_0_OPRDY,
    input bondout0_0_ACK,
    output bondout0_0_RWBAR,
    output [63:0] bondout0_0_WDATA,
    output [21:0] bondout0_0_ADDR,
    input [63:0] bondout0_0_RDATA,
    output [7:0] bondout0_0_LANES,
    
/* portgroup= abstractionName=HFAST1 pi_name=bondout0_1 */
output reg bondout0_1_OPREQ,
    input bondout0_1_OPRDY,
    input bondout0_1_ACK,
    output bondout0_1_RWBAR,
    output [63:0] bondout0_1_WDATA,
    output [21:0] bondout0_1_ADDR,
    input [63:0] bondout0_1_RDATA,
    output [7:0] bondout0_1_LANES,
    
/* portgroup= abstractionName=HFAST1 pi_name=bondout1_0 */
output reg bondout1_0_OPREQ,
    input bondout1_0_OPRDY,
    input bondout1_0_ACK,
    output bondout1_0_RWBAR,
    output [63:0] bondout1_0_WDATA,
    output [21:0] bondout1_0_ADDR,
    input [63:0] bondout1_0_RDATA,
    output [7:0] bondout1_0_LANES,
    
/* portgroup= abstractionName=HFAST1 pi_name=bondout1_1 */
output reg bondout1_1_OPREQ,
    input bondout1_1_OPRDY,
    input bondout1_1_ACK,
    output bondout1_1_RWBAR,
    output [63:0] bondout1_1_WDATA,
    output [21:0] bondout1_1_ADDR,
    input [63:0] bondout1_1_RDATA,
    output [7:0] bondout1_1_LANES,
    
/* portgroup= abstractionName=L2590-vg pi_name=net2batch10 */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir pi_name=directorate10 */
input reset);
// abstractionName=res2-morenets
  reg [1:0] primesya10PC10nz;
 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogprimesya/1.0
      if (reset)  begin 
               primesya10PC10nz_pc_export <= 32'd0;
               count <= 32'd0;
               evariant <= 32'd0;
               edesign <= 32'd0;
               ksubsDesignSerialNumber <= 32'd0;
               ksubsGpioLeds <= 32'd0;
               ksubsAbendSyndrome <= 32'd0;
               elimit <= 32'd0;
               ksubsManualWaypoint <= 32'd0;
               KppWaypoint0 <= 640'd0;
               KppWaypoint1 <= 640'd0;
               bondout0_0_OPREQ <= 32'd0;
               bondout0_1_OPREQ <= 32'd0;
               bondout1_0_OPREQ <= 32'd0;
               bondout1_1_OPREQ <= 32'd0;
               primesya10PC10nz <= 32'd0;
               end 
               else  begin 
              
              case (primesya10PC10nz)
                  32'h0/*0:primesya10PC10nz*/:  begin 
                      $display("%s%1d", "Primes Up To ", 32'sh2710);
                       count <= 32'h0;
                       evariant <= 32'sd0;
                       edesign <= 32'sd4032;
                       ksubsDesignSerialNumber <= 32'sh1010;
                       ksubsGpioLeds <= 8'h80;
                       ksubsAbendSyndrome <= 8'h80;
                       elimit <= 32'sh2710;
                       ksubsManualWaypoint <= 8'h1;
                       KppWaypoint0 <= 32'sd1;
                       KppWaypoint1 <= "START";
                       primesya10PC10nz <= 32'h1/*1:primesya10PC10nz*/;
                       end 
                      
                  32'h1/*1:primesya10PC10nz*/:  primesya10PC10nz <= 32'h2/*2:primesya10PC10nz*/;

                  32'h2/*2:primesya10PC10nz*/:  begin 
                       count <= 32'h0;
                       primesya10PC10nz <= 32'h1/*1:primesya10PC10nz*/;
                       end 
                      endcase
               primesya10PC10nz_pc_export <= primesya10PC10nz;
               bondout0_0_OPREQ <= 32'd0;
               bondout0_1_OPREQ <= 32'd0;
               bondout1_0_OPREQ <= 32'd0;
               bondout1_1_OPREQ <= 32'd0;
               end 
              //End structure cvtToVerilogprimesya/1.0


       end 
      

// Structural Resource (FU) inventory:// 1 vectors of width 2
// Total state bits in module = 2 bits.
// Total number of leaf cells = 0
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.7 : 15th June 2018
//04/07/2018 12:00:40
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe primesya.exe -vnl=offchip.v -vnl-rootmodname=DUT -kiwife-directorate-endmode=finish -vnl-resets=synchronous -bevelab-default-pause-mode=hard -vnl-roundtrip=disable -kiwic-cil-dump=combined -kiwic-kcode-dump=enable -kiwic-register-colours=1 -diosim-vcd=vcd.vcd -res2-offchip-threshold=32


//----------------------------------------------------------

//Report from kiwife:::
//Bondout Load/Store (and other) Ports
//*--------------+------------+--------------------------+----------+--------+--------+-------+-----------*
//| AddressSpace | Name       | Protocol                 | No Words | Awidth | Dwidth | Lanes | LaneWidth |
//*--------------+------------+--------------------------+----------+--------+--------+-------+-----------*
//| bondout0     | bondout0_0 | IPB_HFAST1 PD_halfduplex | 4194304  | 22     | 64     | 8     | 8         |
//| bondout0     | bondout0_1 | IPB_HFAST1 PD_halfduplex | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_0 | IPB_HFAST1 PD_halfduplex | 4194304  | 22     | 64     | 8     | 8         |
//| bondout1     | bondout1_1 | IPB_HFAST1 PD_halfduplex | 4194304  | 22     | 64     | 8     | 8         |
//*--------------+------------+--------------------------+----------+--------+--------+-------+-----------*

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T400:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T401:::
//: Linear scan colouring done for 0 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T402:::
//: Linear scan colouring done for 1 vregs using 0 pregs
//

//----------------------------------------------------------

//Report from kiwife virtual to physical register colouring/mapping for thread T403:::
//: Linear scan colouring done for 1 vregs using 1 pregs
//
//Allocate phy reg purpose=cil_svar_def msg=allocation for thread T403 for V5006 dt=SINT usecount=1
//

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//Bondout Port Settings
//
//*----------------------------------+-------+-------------*
//
//| Key                              | Value | Description |
//
//*----------------------------------+-------+-------------*
//
//| bondout-loadstore-lane_addr-size | 22    |             |
//
//*----------------------------------+-------+-------------*
//
//KiwiC: front end input processing of class or method called KiwiSystem.Kiwi
//
//root_walk start thread at a static method (used as an entry point). Method name=KiwiSystem/Kiwi/.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) uid=cctor14 full_idl=KiwiSystem.Kiwi..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called System.BitConverter
//
//root_walk start thread at a static method (used as an entry point). Method name=System/BitConverter/.cctor uid=cctor12
//
//KiwiC start_thread (or entry point) uid=cctor12 full_idl=System.BitConverter..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+1
//
//KiwiC: front end input processing of class or method called primesya
//
//root_walk start thread at a static method (used as an entry point). Method name=primesya/.cctor uid=cctor10
//
//KiwiC start_thread (or entry point) uid=cctor10 full_idl=primesya..cctor
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+2
//
//KiwiC: front end input processing of class or method called primesya
//
//root_compiler: start elaborating class 'primesya'
//
//elaborating class 'primesya'
//
//compiling static method as entry point: style=Root idl=primesya/Main
//
//Performing root elaboration of method primesya.Main
//
//KiwiC start_thread (or entry point) uid=Main10 full_idl=primesya.Main
//
//root_compiler class done: primesya
//
//Report of all settings used from the recipe or command line:
//
//   bondout-schema=bondout0=H1H,H1H,4194304,8,8;bondout1=H1H,H1H,4194304,8,8
//
//   bondout-protocol=HFAST1
//
//   bondout-loadstore-lane-width=8
//
//   bondout-loadstore-port-lanes=32
//
//   bondout-loadstore-port-count=1
//
//   bondout-loadstore-simplex-ports=disable
//
//   bondout-loadstore-lane-addr-size=22
//
//   kiwife-directorate-ready-flag=absent
//
//   kiwife-directorate-endmode=finish
//
//   kiwife-directorate-startmode=self-start
//
//   kiwic-default-dynamic-heapalloc-bytes=1073741824
//
//   cil-uwind-budget=10000
//
//   kiwic-cil-dump=combined
//
//   kiwic-kcode-dump=enable
//
//   kiwife-dynpoly=enable
//
//   kiwic-register-colours=1
//
//   array-4d-name=KIWIARRAY4D
//
//   array-3d-name=KIWIARRAY3D
//
//   array-2d-name=KIWIARRAY2D
//
//   kiwi-dll=Kiwi.dll
//
//   kiwic-dll=Kiwic.dll
//
//   kiwic-zerolength-arrays=disable
//
//   kiwifefpgaconsole-default=enable
//
//   kiwife-directorate-style=basic
//
//   postgen-optimise=enable
//
//   kiwife-allow-hpr-alloc=enable
//
//   kiwife-cil-loglevel=20
//
//   kiwife-ataken-loglevel=20
//
//   kiwife-gtrace-loglevel=20
//
//   kiwife-firstpass-loglevel=20
//
//   kiwife-overloads-loglevel=20
//
//   root=$attributeroot
//
//   srcfile=primesya.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final State
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout0 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Res2 Final State
//Highest off-chip SRAM/DRAM location in use in logical memory space bondout1 is <null> (--not-used--) bytes=4194304

//----------------------------------------------------------

//Report from restructure2:::
//Bondout Load/Store (and other) Ports 'Res2 Final State'
//*--------------+------------+--------------------------+------------+-------------------+--------+--------+-------+-----------+------*
//| AddressSpace | Name       | Protocol                 | Bytes      | Addressable Words | Awidth | Dwidth | Lanes | LaneWidth | Used |
//*--------------+------------+--------------------------+------------+-------------------+--------+--------+-------+-----------+------*
//| bondout0     | bondout0_0 | IPB_HFAST1 PD_halfduplex | 33554432   | 4194304           | 22     | 64     | 8     | 8         | ---- |
//| bondout0     | bondout0_1 | IPB_HFAST1 PD_halfduplex | (33554432) | 4194304           | 22     | 64     | 8     | 8         | ---- |
//| bondout1     | bondout1_0 | IPB_HFAST1 PD_halfduplex | 33554432   | 4194304           | 22     | 64     | 8     | 8         | ---- |
//| bondout1     | bondout1_1 | IPB_HFAST1 PD_halfduplex | (33554432) | 4194304           | 22     | 64     | 8     | 8         | ---- |
//*--------------+------------+--------------------------+------------+-------------------+--------+--------+-------+-----------+------*

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*------------------------+-------+---------------------------------------------------------------------------------*
//| Key                    | Value | Description                                                                     |
//*------------------------+-------+---------------------------------------------------------------------------------*
//| int-flr-mul            | 1000  |                                                                                 |
//| max-no-fp-addsubs      | 6     | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls         | 6     | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls        | 3     | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs         | 2     | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs        | 2     | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors     | 8     | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing   | 8     | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div           | 5     |                                                                                 |
//| fp-fl-dp-add           | 4     |                                                                                 |
//| fp-fl-dp-mul           | 3     |                                                                                 |
//| fp-fl-sp-div           | 15    |                                                                                 |
//| fp-fl-sp-add           | 4     |                                                                                 |
//| fp-fl-sp-mul           | 5     |                                                                                 |
//| res2-offchip-threshold | 32    |                                                                                 |
//| res2-combrom-threshold | 64    |                                                                                 |
//| res2-combram-threshold | 32    |                                                                                 |
//| res2-regfile-threshold | 8     |                                                                                 |
//*------------------------+-------+---------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for primesya10PC10 
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| gb-flag/Pause             | eno | Root Pc | hwm         | Exec | Reverb | Start | End | Next |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*
//| XU32'0:"0:primesya10PC10" | 805 | 0       | hwm=0.0.0   | 0    |        | -     | -   | 1    |
//| XU32'1:"1:primesya10PC10" | 804 | 1       | hwm=0.0.0   | 1    |        | -     | -   | 2    |
//| XU32'2:"2:primesya10PC10" | 803 | 2       | hwm=0.0.0   | 2    |        | -     | -   | 1    |
//*---------------------------+-----+---------+-------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=primesya10PC10 state=XU32'0:"0:primesya10PC10" 805 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Schedule for res2: nopipeline: Thread=primesya10PC10 state=XU32'0:"0:primesya10PC10"
//res2: nopipeline: Thread=primesya10PC10 state=XU32'0:"0:primesya10PC10"
//*------+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                         |
//*------+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                                              |
//| F0   | 805 | R0 DATA |                                                                                                                                              |
//| F0+E | 805 | W0 DATA | ksubsManualWaypoint te=te:F0 write(U8'1) elimit te=te:F0 write(S32'10000) ksubsAbendSyndrome te=te:F0 write(U8'128) ksubsGpioLeds te=te:F0 \ |
//|      |     |         | write(U8'128) ksubsDesignSerialNumber te=te:F0 write(S32'4112) edesign te=te:F0 write(4032) evariant te=te:F0 write(0) count te=te:F0 write\ |
//|      |     |         | (U32'0)  PLI:Primes Up To   W/P:START                                                                                                        |
//*------+-----+---------+----------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=primesya10PC10 state=XU32'1:"1:primesya10PC10" 804 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Schedule for res2: nopipeline: Thread=primesya10PC10 state=XU32'1:"1:primesya10PC10"
//res2: nopipeline: Thread=primesya10PC10 state=XU32'1:"1:primesya10PC10"
//*------+-----+---------+------*
//| pc   | eno | Phaser  | Work |
//*------+-----+---------+------*
//| F1   | -   | R0 CTRL |      |
//| F1   | 804 | R0 DATA |      |
//| F1+E | 804 | W0 DATA |      |
//*------+-----+---------+------*

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=primesya10PC10 state=XU32'2:"2:primesya10PC10" 803 :  major_start_pcl=2   edge_private_start/end=-1/-1 exec=2 (dend=0)
//Schedule for res2: nopipeline: Thread=primesya10PC10 state=XU32'2:"2:primesya10PC10"
//res2: nopipeline: Thread=primesya10PC10 state=XU32'2:"2:primesya10PC10"
//*------+-----+---------+-----------------------------*
//| pc   | eno | Phaser  | Work                        |
//*------+-----+---------+-----------------------------*
//| F2   | -   | R0 CTRL |                             |
//| F2   | 803 | R0 DATA |                             |
//| F2+E | 803 | W0 DATA | count te=te:F2 write(U32'0) |
//*------+-----+---------+-----------------------------*

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

//Report from enumbers:::
//Concise expression alias report.
//
//  -- No expression aliases to report
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for offchip to offchip

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory:
//1 vectors of width 2
//
//Total state bits in module = 2 bits.
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread KiwiSystem/Kiwi/.cctor uid=cctor14 has 6 CIL instructions in 1 basic blocks
//Thread System/BitConverter/.cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread primesya/.cctor uid=cctor10 has 12 CIL instructions in 1 basic blocks
//Thread primesya/Main uid=Main10 has 73 CIL instructions in 2 basic blocks
//Thread mpc10 has 3 bevelab control states (pauses)
//Reindexed thread primesya10PC10 with 3 minor control states
// eof (HPR L/S Verilog)
