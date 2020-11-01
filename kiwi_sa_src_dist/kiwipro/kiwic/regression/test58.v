

// CBG Orangepath HPR L/S System

// Verilog output file generated at 17/09/2017 22:15:42
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.2b : 9th Sept 2017 Linux/X86_64:koo
//  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -repack-to-roms=enable -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test58.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step
`timescale 1ns/1ns


module DUT(    
/* portgroup= abstractionName=waypoints port_iname=noname */
    output reg [639:0] KppWaypoint0,
    output [639:0] KppWaypoint1,
    
/* portgroup= abstractionName=res2-directornets */
output reg [7:0] bevelab10nz_pc_export,
    
/* portgroup= abstractionName=L2590-vg port_iname=noname */
input clk,
    
/* portgroup= abstractionName=directorate-vg-dir port_iname=noname */
input reset);

function [63:0] hpr_flt2dbl0;//Floating-point convert single to double precision.
input [31:0] darg;
  hpr_flt2dbl0 = {darg[31], darg[30], {3{~darg[30]}}, darg[29:23], darg[22:0], {29{1'b0}}};
endfunction


function hpr_fp_dltd4; //Floating-point 'less-than' predicate.
   input [31:0] hpr_fp_dltd4_a, hpr_fp_dltd4_b;
  hpr_fp_dltd4 = ((hpr_fp_dltd4_a[31] && !hpr_fp_dltd4_b[31]) ? 1: (!hpr_fp_dltd4_a[31] && hpr_fp_dltd4_b[31])  ? 0 : (hpr_fp_dltd4_a[31]^(hpr_fp_dltd4_a[30:0]<hpr_fp_dltd4_b[30:0]))); 
   endfunction


function hpr_fp_dltd1; //Floating-point 'less-than' predicate.
   input [63:0] hpr_fp_dltd1_a, hpr_fp_dltd1_b;
  hpr_fp_dltd1 = ((hpr_fp_dltd1_a[63] && !hpr_fp_dltd1_b[63]) ? 1: (!hpr_fp_dltd1_a[63] && hpr_fp_dltd1_b[63])  ? 0 : (hpr_fp_dltd1_a[63]^(hpr_fp_dltd1_a[62:0]<hpr_fp_dltd1_b[62:0]))); 
   endfunction


function signed [31:0] rtl_signed_bitextract3;
   input [63:0] arg;
   rtl_signed_bitextract3 = $signed(arg[31:0]);
   endfunction


function [63:0] rtl_unsigned_extend2;
   input [31:0] arg;
   rtl_unsigned_extend2 = { 32'b0, arg[31:0] };
   endfunction

// abstractionName=res2-ctrl2
// abstractionName=kiwicmainnets10
  integer T401_test58_sqrt_Sqrt_1_12_V_4;
  reg/*fp*/  [63:0] T401_test58_sqrt_Sqrt_1_12_V_3;
  reg/*fp*/  [63:0] T401_test58_sqrt_Sqrt_1_12_SPILL_256;
  reg/*fp*/  [63:0] T401_test58_double_test_0_9_V_1;
  reg signed [63:0] T401_test58_double_test_0_9_V_0;
  reg/*fp*/  [31:0] T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
  reg signed [63:0] T401_test58_single_test_0_6_V_0;
// abstractionName=repack-newnets
  reg [31:0] A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[31:0];
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] fpcvt10_result;
  reg signed [63:0] fpcvt10_arg;
  wire fpcvt10_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPDIVIDER10_FPRR;
  reg/*fp*/  [31:0] CVFPDIVIDER10_NN;
  reg/*fp*/  [31:0] CVFPDIVIDER10_DD;
  wire CVFPDIVIDER10_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPADDER10_FPRR;
  reg/*fp*/  [31:0] CVFPADDER10_A0;
  reg/*fp*/  [31:0] CVFPADDER10_A1;
  wire CVFPADDER10_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPMULTIPLIER10_FPRR;
  reg/*fp*/  [31:0] CVFPMULTIPLIER10_A0;
  reg/*fp*/  [31:0] CVFPMULTIPLIER10_A1;
  wire CVFPMULTIPLIER10_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPDIVIDER12_FPRR;
  reg/*fp*/  [31:0] CVFPDIVIDER12_NN;
  reg/*fp*/  [31:0] CVFPDIVIDER12_DD;
  wire CVFPDIVIDER12_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPADDER12_FPRR;
  reg/*fp*/  [31:0] CVFPADDER12_A0;
  reg/*fp*/  [31:0] CVFPADDER12_A1;
  wire CVFPADDER12_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPMULTIPLIER12_FPRR;
  reg/*fp*/  [31:0] CVFPMULTIPLIER12_A0;
  reg/*fp*/  [31:0] CVFPMULTIPLIER12_A1;
  wire CVFPMULTIPLIER12_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] fpcvt12_result;
  reg signed [63:0] fpcvt12_arg;
  wire fpcvt12_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPMULTIPLIER14_FPRR;
  reg/*fp*/  [31:0] CVFPMULTIPLIER14_A0;
  reg/*fp*/  [31:0] CVFPMULTIPLIER14_A1;
  wire CVFPMULTIPLIER14_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPADDER14_FPRR;
  reg/*fp*/  [31:0] CVFPADDER14_A0;
  reg/*fp*/  [31:0] CVFPADDER14_A1;
  wire CVFPADDER14_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] fpcvt14_result;
  reg signed [63:0] fpcvt14_arg;
  wire fpcvt14_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPMULTIPLIER16_FPRR;
  reg/*fp*/  [31:0] CVFPMULTIPLIER16_A0;
  reg/*fp*/  [31:0] CVFPMULTIPLIER16_A1;
  wire CVFPMULTIPLIER16_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [31:0] CVFPADDER16_FPRR;
  reg/*fp*/  [31:0] CVFPADDER16_A0;
  reg/*fp*/  [31:0] CVFPADDER16_A1;
  wire CVFPADDER16_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] fpcvt16_result;
  reg signed [63:0] fpcvt16_arg;
  wire fpcvt16_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] CVFPDIVIDER14_FPRR;
  reg/*fp*/  [63:0] CVFPDIVIDER14_NN;
  reg/*fp*/  [63:0] CVFPDIVIDER14_DD;
  wire CVFPDIVIDER14_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] CVFPADDER18_FPRR;
  reg/*fp*/  [63:0] CVFPADDER18_A0;
  reg/*fp*/  [63:0] CVFPADDER18_A1;
  wire CVFPADDER18_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] CVFPMULTIPLIER18_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER18_A0;
  reg/*fp*/  [63:0] CVFPMULTIPLIER18_A1;
  wire CVFPMULTIPLIER18_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] fpcvt18_result;
  reg signed [63:0] fpcvt18_arg;
  wire fpcvt18_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] CVFPMULTIPLIER20_FPRR;
  reg/*fp*/  [63:0] CVFPMULTIPLIER20_A0;
  reg/*fp*/  [63:0] CVFPMULTIPLIER20_A1;
  wire CVFPMULTIPLIER20_fail;
// abstractionName=res2-contacts port_iname=noname
  wire/*fp*/  [63:0] CVFPADDER20_FPRR;
  reg/*fp*/  [63:0] CVFPADDER20_A0;
  reg/*fp*/  [63:0] CVFPADDER20_A1;
  wire CVFPADDER20_fail;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] CVFPADDER20RRh10hold;
  reg CVFPADDER20RRh10shot0;
  reg CVFPADDER20RRh10shot1;
  reg CVFPADDER20RRh10shot2;
  reg CVFPADDER20RRh10shot3;
  reg/*fp*/  [63:0] fpcvt18RRh10hold;
  reg fpcvt18RRh10shot0;
  reg fpcvt18RRh10shot1;
  reg/*fp*/  [63:0] CVFPMULTIPLIER20RRh10hold;
  reg CVFPMULTIPLIER20RRh10shot0;
  reg CVFPMULTIPLIER20RRh10shot1;
  reg CVFPMULTIPLIER20RRh10shot2;
  reg/*fp*/  [63:0] CVFPMULTIPLIER18RRh10hold;
  reg CVFPMULTIPLIER18RRh10shot0;
  reg CVFPMULTIPLIER18RRh10shot1;
  reg CVFPMULTIPLIER18RRh10shot2;
  reg/*fp*/  [63:0] CVFPADDER18RRh10hold;
  reg CVFPADDER18RRh10shot0;
  reg CVFPADDER18RRh10shot1;
  reg CVFPADDER18RRh10shot2;
  reg CVFPADDER18RRh10shot3;
  reg/*fp*/  [63:0] CVFPDIVIDER14RRh10hold;
  reg CVFPDIVIDER14RRh10shot0;
  reg CVFPDIVIDER14RRh10shot1;
  reg CVFPDIVIDER14RRh10shot2;
  reg CVFPDIVIDER14RRh10shot3;
  reg CVFPDIVIDER14RRh10shot4;
  reg/*fp*/  [63:0] fpcvt16RRh10hold;
  reg fpcvt16RRh10shot0;
  reg fpcvt16RRh10shot1;
  reg/*fp*/  [31:0] CVFPADDER14RRh10hold;
  reg CVFPADDER14RRh10shot0;
  reg CVFPADDER14RRh10shot1;
  reg CVFPADDER14RRh10shot2;
  reg CVFPADDER14RRh10shot3;
  reg/*fp*/  [31:0] fpcvt12RRh10hold;
  reg fpcvt12RRh10shot0;
  reg fpcvt12RRh10shot1;
  reg/*fp*/  [31:0] CVFPMULTIPLIER14RRh10hold;
  reg CVFPMULTIPLIER14RRh10shot0;
  reg CVFPMULTIPLIER14RRh10shot1;
  reg CVFPMULTIPLIER14RRh10shot2;
  reg CVFPMULTIPLIER14RRh10shot3;
  reg CVFPMULTIPLIER14RRh10shot4;
  reg/*fp*/  [31:0] CVFPADDER16RRh10hold;
  reg CVFPADDER16RRh10shot0;
  reg CVFPADDER16RRh10shot1;
  reg CVFPADDER16RRh10shot2;
  reg CVFPADDER16RRh10shot3;
  reg/*fp*/  [31:0] fpcvt14RRh10hold;
  reg fpcvt14RRh10shot0;
  reg fpcvt14RRh10shot1;
  reg/*fp*/  [31:0] CVFPMULTIPLIER16RRh10hold;
  reg CVFPMULTIPLIER16RRh10shot0;
  reg CVFPMULTIPLIER16RRh10shot1;
  reg CVFPMULTIPLIER16RRh10shot2;
  reg CVFPMULTIPLIER16RRh10shot3;
  reg CVFPMULTIPLIER16RRh10shot4;
  reg/*fp*/  [31:0] CVFPMULTIPLIER12RRh10hold;
  reg CVFPMULTIPLIER12RRh10shot0;
  reg CVFPMULTIPLIER12RRh10shot1;
  reg CVFPMULTIPLIER12RRh10shot2;
  reg CVFPMULTIPLIER12RRh10shot3;
  reg CVFPMULTIPLIER12RRh10shot4;
  reg/*fp*/  [31:0] CVFPADDER12RRh10hold;
  reg CVFPADDER12RRh10shot0;
  reg CVFPADDER12RRh10shot1;
  reg CVFPADDER12RRh10shot2;
  reg CVFPADDER12RRh10shot3;
  reg/*fp*/  [31:0] CVFPDIVIDER12RRh10hold;
  reg CVFPDIVIDER12RRh10shot0;
  reg CVFPDIVIDER12RRh10shot1;
  reg CVFPDIVIDER12RRh10shot2;
  reg CVFPDIVIDER12RRh10shot3;
  reg CVFPDIVIDER12RRh10shot4;
  reg CVFPDIVIDER12RRh10shot5;
  reg CVFPDIVIDER12RRh10shot6;
  reg CVFPDIVIDER12RRh10shot7;
  reg CVFPDIVIDER12RRh10shot8;
  reg CVFPDIVIDER12RRh10shot9;
  reg CVFPDIVIDER12RRh10shot10;
  reg CVFPDIVIDER12RRh10shot11;
  reg CVFPDIVIDER12RRh10shot12;
  reg CVFPDIVIDER12RRh10shot13;
  reg CVFPDIVIDER12RRh10shot14;
  reg/*fp*/  [31:0] CVFPMULTIPLIER10RRh10hold;
  reg CVFPMULTIPLIER10RRh10shot0;
  reg CVFPMULTIPLIER10RRh10shot1;
  reg CVFPMULTIPLIER10RRh10shot2;
  reg CVFPMULTIPLIER10RRh10shot3;
  reg CVFPMULTIPLIER10RRh10shot4;
  reg/*fp*/  [31:0] CVFPADDER10RRh10hold;
  reg CVFPADDER10RRh10shot0;
  reg CVFPADDER10RRh10shot1;
  reg CVFPADDER10RRh10shot2;
  reg CVFPADDER10RRh10shot3;
  reg/*fp*/  [31:0] CVFPDIVIDER10RRh10hold;
  reg CVFPDIVIDER10RRh10shot0;
  reg CVFPDIVIDER10RRh10shot1;
  reg CVFPDIVIDER10RRh10shot2;
  reg CVFPDIVIDER10RRh10shot3;
  reg CVFPDIVIDER10RRh10shot4;
  reg CVFPDIVIDER10RRh10shot5;
  reg CVFPDIVIDER10RRh10shot6;
  reg CVFPDIVIDER10RRh10shot7;
  reg CVFPDIVIDER10RRh10shot8;
  reg CVFPDIVIDER10RRh10shot9;
  reg CVFPDIVIDER10RRh10shot10;
  reg CVFPDIVIDER10RRh10shot11;
  reg CVFPDIVIDER10RRh10shot12;
  reg CVFPDIVIDER10RRh10shot13;
  reg CVFPDIVIDER10RRh10shot14;
  reg/*fp*/  [31:0] fpcvt10RRh10hold;
  reg fpcvt10RRh10shot0;
  reg fpcvt10RRh10shot1;
  reg [7:0] bevelab10nz;
 always   @(* )  begin 
       CVFPADDER20_A0 = 32'sd0;
       CVFPADDER20_A1 = 32'sd0;
       fpcvt18_arg = 32'sd0;
       CVFPMULTIPLIER20_A0 = 32'sd0;
       CVFPMULTIPLIER20_A1 = 32'sd0;
       CVFPMULTIPLIER18_A0 = 32'sd0;
       CVFPMULTIPLIER18_A1 = 32'sd0;
       CVFPADDER18_A0 = 32'sd0;
       CVFPADDER18_A1 = 32'sd0;
       CVFPDIVIDER14_NN = 32'sd0;
       CVFPDIVIDER14_DD = 32'sd0;
       fpcvt16_arg = 32'sd0;
       CVFPADDER16_A0 = 32'sd0;
       CVFPADDER16_A1 = 32'sd0;
       fpcvt14_arg = 32'sd0;
       CVFPMULTIPLIER16_A0 = 32'sd0;
       CVFPMULTIPLIER16_A1 = 32'sd0;
       CVFPADDER14_A0 = 32'sd0;
       CVFPADDER14_A1 = 32'sd0;
       fpcvt12_arg = 32'sd0;
       CVFPMULTIPLIER14_A0 = 32'sd0;
       CVFPMULTIPLIER14_A1 = 32'sd0;
       CVFPMULTIPLIER12_A0 = 32'sd0;
       CVFPMULTIPLIER12_A1 = 32'sd0;
       CVFPADDER12_A0 = 32'sd0;
       CVFPADDER12_A1 = 32'sd0;
       CVFPDIVIDER12_NN = 32'sd0;
       CVFPDIVIDER12_DD = 32'sd0;
       CVFPMULTIPLIER10_A0 = 32'sd0;
       CVFPMULTIPLIER10_A1 = 32'sd0;
       CVFPADDER10_A0 = 32'sd0;
       CVFPADDER10_A1 = 32'sd0;
       CVFPDIVIDER10_NN = 32'sd0;
       CVFPDIVIDER10_DD = 32'sd0;
       fpcvt10_arg = 32'sd0;

      case (bevelab10nz)
          32'h2/*2:bevelab10nz*/:  fpcvt10_arg = T401_test58_single_test_0_6_V_0;

          32'h4/*4:bevelab10nz*/:  begin 
               CVFPDIVIDER10_DD = $unsigned(32'sh1fbb_4000+($unsigned(((32'h4/*4:bevelab10nz*/==bevelab10nz)? fpcvt10_result: fpcvt10RRh10hold
              ))>>32'sd1));

               CVFPDIVIDER10_NN = ((32'h4/*4:bevelab10nz*/==bevelab10nz)? fpcvt10_result: fpcvt10RRh10hold);
               end 
              
          32'h13/*19:bevelab10nz*/:  begin 
               CVFPADDER10_A1 = ((32'h13/*19:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER10_FPRR: CVFPDIVIDER10RRh10hold);
               CVFPADDER10_A0 = $unsigned(32'sh1fbb_4000+($unsigned(((32'h4/*4:bevelab10nz*/==bevelab10nz)? fpcvt10_result: fpcvt10RRh10hold
              ))>>32'sd1));

               end 
              
          32'h17/*23:bevelab10nz*/:  begin 
               CVFPMULTIPLIER10_A1 = ((32'h17/*23:bevelab10nz*/==bevelab10nz)? CVFPADDER10_FPRR: CVFPADDER10RRh10hold);
               CVFPMULTIPLIER10_A0 = 32'h3f00_0000;
               end 
              
          32'h1c/*28:bevelab10nz*/:  begin 
               CVFPDIVIDER12_DD = ((32'h1c/*28:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER10_FPRR: CVFPMULTIPLIER10RRh10hold);
               CVFPDIVIDER12_NN = ((32'h4/*4:bevelab10nz*/==bevelab10nz)? fpcvt10_result: fpcvt10RRh10hold);
               end 
              
          32'h2b/*43:bevelab10nz*/:  begin 
               CVFPADDER12_A1 = ((32'h2b/*43:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER12_FPRR: CVFPDIVIDER12RRh10hold);
               CVFPADDER12_A0 = ((32'h1c/*28:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER10_FPRR: CVFPMULTIPLIER10RRh10hold);
               end 
              
          32'h2f/*47:bevelab10nz*/:  begin 
               CVFPMULTIPLIER12_A1 = ((32'h2f/*47:bevelab10nz*/==bevelab10nz)? CVFPADDER12_FPRR: CVFPADDER12RRh10hold);
               CVFPMULTIPLIER12_A0 = 32'h3f00_0000;
               end 
              endcase
      if ((64'shcc*T401_test58_single_test_0_6_V_0>=64'sh3_8d7e_a4c6_8000)) 
          case (bevelab10nz)
              32'h35/*53:bevelab10nz*/:  begin 
                   CVFPMULTIPLIER16_A1 = 32'sh8000_0000^T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
                   CVFPMULTIPLIER16_A0 = T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
                   fpcvt14_arg = T401_test58_single_test_0_6_V_0;
                   end 
                  
              32'h52/*82:bevelab10nz*/:  begin 
                   CVFPADDER16_A1 = ((32'h52/*82:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER16_FPRR: CVFPMULTIPLIER16RRh10hold);
                   CVFPADDER16_A0 = ((32'h4f/*79:bevelab10nz*/==bevelab10nz)? fpcvt14_result: fpcvt14RRh10hold);
                   end 
                  
              32'h56/*86:bevelab10nz*/:  begin 
                   CVFPDIVIDER10_DD = ((32'h4f/*79:bevelab10nz*/==bevelab10nz)? fpcvt14_result: fpcvt14RRh10hold);
                   CVFPDIVIDER10_NN = ((32'h56/*86:bevelab10nz*/==bevelab10nz)? CVFPADDER16_FPRR: CVFPADDER16RRh10hold);
                   end 
                  endcase
           else 
          case (bevelab10nz)
              32'h35/*53:bevelab10nz*/:  begin 
                   CVFPMULTIPLIER14_A1 = 32'sh8000_0000^T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
                   CVFPMULTIPLIER14_A0 = T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256;
                   fpcvt12_arg = T401_test58_single_test_0_6_V_0;
                   end 
                  
              32'h3a/*58:bevelab10nz*/:  begin 
                   CVFPADDER14_A1 = ((32'h3a/*58:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER14_FPRR: CVFPMULTIPLIER14RRh10hold);
                   CVFPADDER14_A0 = ((32'h37/*55:bevelab10nz*/==bevelab10nz)? fpcvt12_result: fpcvt12RRh10hold);
                   end 
                  
              32'h3e/*62:bevelab10nz*/:  begin 
                   CVFPDIVIDER12_DD = ((32'h37/*55:bevelab10nz*/==bevelab10nz)? fpcvt12_result: fpcvt12RRh10hold);
                   CVFPDIVIDER12_NN = ((32'h3e/*62:bevelab10nz*/==bevelab10nz)? CVFPADDER14_FPRR: CVFPADDER14RRh10hold);
                   end 
                  endcase
      if ((32'h68/*104:bevelab10nz*/==bevelab10nz))  fpcvt16_arg = T401_test58_double_test_0_9_V_0;
          if ((T401_test58_sqrt_Sqrt_1_12_V_4<32'sd3)) 
          case (bevelab10nz)
              32'h6b/*107:bevelab10nz*/:  begin 
                   CVFPDIVIDER14_DD = T401_test58_sqrt_Sqrt_1_12_V_3;
                   CVFPDIVIDER14_NN = T401_test58_double_test_0_9_V_1;
                   end 
                  
              32'h70/*112:bevelab10nz*/:  begin 
                   CVFPADDER18_A1 = ((32'h70/*112:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER14_FPRR: CVFPDIVIDER14RRh10hold);
                   CVFPADDER18_A0 = T401_test58_sqrt_Sqrt_1_12_V_3;
                   end 
                  
              32'h74/*116:bevelab10nz*/:  begin 
                   CVFPMULTIPLIER18_A1 = ((32'h74/*116:bevelab10nz*/==bevelab10nz)? CVFPADDER18_FPRR: CVFPADDER18RRh10hold);
                   CVFPMULTIPLIER18_A0 = 64'h3fe0_0000_0000_0000;
                   end 
                  endcase
          
      case (bevelab10nz)
          32'h78/*120:bevelab10nz*/:  begin 
               CVFPMULTIPLIER20_A1 = 64'sh8000_0000_0000_0000^T401_test58_sqrt_Sqrt_1_12_SPILL_256;
               CVFPMULTIPLIER20_A0 = T401_test58_sqrt_Sqrt_1_12_SPILL_256;
               fpcvt18_arg = T401_test58_double_test_0_9_V_0;
               end 
              
          32'h7b/*123:bevelab10nz*/:  begin 
               CVFPADDER20_A1 = ((32'h7b/*123:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER20_FPRR: CVFPMULTIPLIER20RRh10hold);
               CVFPADDER20_A0 = ((32'h7a/*122:bevelab10nz*/==bevelab10nz)? fpcvt18_result: fpcvt18RRh10hold);
               end 
              
          32'h7f/*127:bevelab10nz*/:  begin 
               CVFPDIVIDER14_DD = ((32'h7a/*122:bevelab10nz*/==bevelab10nz)? fpcvt18_result: fpcvt18RRh10hold);
               CVFPDIVIDER14_NN = ((32'h7f/*127:bevelab10nz*/==bevelab10nz)? CVFPADDER20_FPRR: CVFPADDER20RRh10hold);
               end 
              endcase
       end 
      

 always   @(posedge clk )  begin 
      //Start structure HPR test58/1.0
      if (reset)  begin 
               bevelab10nz_pc_export <= 32'd0;
               T401_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'd0;
               T401_test58_sqrt_Sqrt_1_12_V_4 <= 32'd0;
               T401_test58_sqrt_Sqrt_1_12_V_3 <= 64'd0;
               T401_test58_double_test_0_9_V_1 <= 64'd0;
               T401_test58_double_test_0_9_V_0 <= 64'd0;
               T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= 32'd0;
               T401_test58_single_test_0_6_V_0 <= 64'd0;
               fpcvt10RRh10hold <= 32'd0;
               fpcvt10RRh10shot1 <= 32'd0;
               CVFPDIVIDER10RRh10hold <= 32'd0;
               CVFPDIVIDER10RRh10shot1 <= 32'd0;
               CVFPDIVIDER10RRh10shot2 <= 32'd0;
               CVFPDIVIDER10RRh10shot3 <= 32'd0;
               CVFPDIVIDER10RRh10shot4 <= 32'd0;
               CVFPDIVIDER10RRh10shot5 <= 32'd0;
               CVFPDIVIDER10RRh10shot6 <= 32'd0;
               CVFPDIVIDER10RRh10shot7 <= 32'd0;
               CVFPDIVIDER10RRh10shot8 <= 32'd0;
               CVFPDIVIDER10RRh10shot9 <= 32'd0;
               CVFPDIVIDER10RRh10shot10 <= 32'd0;
               CVFPDIVIDER10RRh10shot11 <= 32'd0;
               CVFPDIVIDER10RRh10shot12 <= 32'd0;
               CVFPDIVIDER10RRh10shot13 <= 32'd0;
               CVFPDIVIDER10RRh10shot14 <= 32'd0;
               CVFPADDER10RRh10hold <= 32'd0;
               CVFPADDER10RRh10shot1 <= 32'd0;
               CVFPADDER10RRh10shot2 <= 32'd0;
               CVFPADDER10RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER10RRh10hold <= 32'd0;
               CVFPMULTIPLIER10RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot4 <= 32'd0;
               CVFPDIVIDER12RRh10hold <= 32'd0;
               CVFPDIVIDER12RRh10shot1 <= 32'd0;
               CVFPDIVIDER12RRh10shot2 <= 32'd0;
               CVFPDIVIDER12RRh10shot3 <= 32'd0;
               CVFPDIVIDER12RRh10shot4 <= 32'd0;
               CVFPDIVIDER12RRh10shot5 <= 32'd0;
               CVFPDIVIDER12RRh10shot6 <= 32'd0;
               CVFPDIVIDER12RRh10shot7 <= 32'd0;
               CVFPDIVIDER12RRh10shot8 <= 32'd0;
               CVFPDIVIDER12RRh10shot9 <= 32'd0;
               CVFPDIVIDER12RRh10shot10 <= 32'd0;
               CVFPDIVIDER12RRh10shot11 <= 32'd0;
               CVFPDIVIDER12RRh10shot12 <= 32'd0;
               CVFPDIVIDER12RRh10shot13 <= 32'd0;
               CVFPDIVIDER12RRh10shot14 <= 32'd0;
               CVFPADDER12RRh10hold <= 32'd0;
               CVFPADDER12RRh10shot1 <= 32'd0;
               CVFPADDER12RRh10shot2 <= 32'd0;
               CVFPADDER12RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER12RRh10hold <= 32'd0;
               CVFPMULTIPLIER12RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot4 <= 32'd0;
               CVFPMULTIPLIER16RRh10hold <= 32'd0;
               CVFPMULTIPLIER16RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER16RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER16RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER16RRh10shot4 <= 32'd0;
               fpcvt14RRh10hold <= 32'd0;
               fpcvt14RRh10shot1 <= 32'd0;
               CVFPADDER16RRh10hold <= 32'd0;
               CVFPADDER16RRh10shot1 <= 32'd0;
               CVFPADDER16RRh10shot2 <= 32'd0;
               CVFPADDER16RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER14RRh10hold <= 32'd0;
               CVFPMULTIPLIER14RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot4 <= 32'd0;
               fpcvt12RRh10hold <= 32'd0;
               fpcvt12RRh10shot1 <= 32'd0;
               CVFPADDER14RRh10hold <= 32'd0;
               CVFPADDER14RRh10shot1 <= 32'd0;
               CVFPADDER14RRh10shot2 <= 32'd0;
               CVFPADDER14RRh10shot3 <= 32'd0;
               fpcvt16RRh10hold <= 64'd0;
               fpcvt16RRh10shot1 <= 32'd0;
               CVFPDIVIDER14RRh10hold <= 64'd0;
               CVFPDIVIDER14RRh10shot1 <= 32'd0;
               CVFPDIVIDER14RRh10shot2 <= 32'd0;
               CVFPDIVIDER14RRh10shot3 <= 32'd0;
               CVFPDIVIDER14RRh10shot4 <= 32'd0;
               CVFPADDER18RRh10hold <= 64'd0;
               CVFPADDER18RRh10shot1 <= 32'd0;
               CVFPADDER18RRh10shot2 <= 32'd0;
               CVFPADDER18RRh10shot3 <= 32'd0;
               CVFPMULTIPLIER18RRh10hold <= 64'd0;
               CVFPMULTIPLIER18RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER18RRh10shot2 <= 32'd0;
               CVFPMULTIPLIER20RRh10hold <= 64'd0;
               CVFPMULTIPLIER20RRh10shot1 <= 32'd0;
               CVFPMULTIPLIER20RRh10shot2 <= 32'd0;
               fpcvt18RRh10hold <= 64'd0;
               fpcvt18RRh10shot1 <= 32'd0;
               CVFPADDER20RRh10hold <= 64'd0;
               CVFPADDER20RRh10shot1 <= 32'd0;
               CVFPADDER20RRh10shot2 <= 32'd0;
               CVFPADDER20RRh10shot3 <= 32'd0;
               CVFPDIVIDER14RRh10shot0 <= 32'd0;
               CVFPADDER20RRh10shot0 <= 32'd0;
               fpcvt18RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER20RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER18RRh10shot0 <= 32'd0;
               CVFPADDER18RRh10shot0 <= 32'd0;
               fpcvt16RRh10shot0 <= 32'd0;
               CVFPDIVIDER12RRh10shot0 <= 32'd0;
               CVFPADDER14RRh10shot0 <= 32'd0;
               fpcvt12RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER14RRh10shot0 <= 32'd0;
               CVFPDIVIDER10RRh10shot0 <= 32'd0;
               CVFPADDER16RRh10shot0 <= 32'd0;
               fpcvt14RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER16RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER12RRh10shot0 <= 32'd0;
               CVFPADDER12RRh10shot0 <= 32'd0;
               CVFPMULTIPLIER10RRh10shot0 <= 32'd0;
               CVFPADDER10RRh10shot0 <= 32'd0;
               fpcvt10RRh10shot0 <= 32'd0;
               KppWaypoint0 <= 640'd0;
               bevelab10nz <= 32'd0;
               end 
               else  begin 
              
              case (bevelab10nz)
                  32'h6a/*106:bevelab10nz*/: $display("    p=%1d   pf=%f", T401_test58_double_test_0_9_V_0, $bitstoreal(((32'h6a/*106:bevelab10nz*/==
                  bevelab10nz)? fpcvt16_result: fpcvt16RRh10hold)));

                  32'h84/*132:bevelab10nz*/: $display("         root dp=%f            error=%f", $bitstoreal(T401_test58_sqrt_Sqrt_1_12_SPILL_256
                  ), $bitstoreal(((32'h84/*132:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER14_FPRR: CVFPDIVIDER14RRh10hold)));
              endcase
              if ((T401_test58_double_test_0_9_V_0>=64'sh3_8d7e_a4c6_8000) && (32'h66/*102:bevelab10nz*/==bevelab10nz))  begin 
                      $display("Test58 finished.");
                      $finish(32'sd0);
                       end 
                      if ((64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000))  begin if ((32'h4d/*77:bevelab10nz*/==bevelab10nz
                  )) $display("         root sp=%f            error=%f", $bitstoreal(hpr_flt2dbl0(T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256
                      )), $bitstoreal(hpr_flt2dbl0(((32'h4d/*77:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER12_FPRR: CVFPDIVIDER12RRh10hold
                      ))));
                       end 
                   else if ((32'h65/*101:bevelab10nz*/==bevelab10nz))  begin 
                          $display("         root sp=%f            error=%f", $bitstoreal(hpr_flt2dbl0(T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256
                          )), $bitstoreal(hpr_flt2dbl0(((32'h65/*101:bevelab10nz*/==bevelab10nz)? CVFPDIVIDER10_FPRR: CVFPDIVIDER10RRh10hold
                          ))));
                          $display("Double Test");
                           end 
                          
              case (bevelab10nz)
                  32'h0/*0:bevelab10nz*/:  begin 
                      $display("Test58 start - sqrt");
                      $display("Single Test");
                       T401_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'h0;
                       T401_test58_sqrt_Sqrt_1_12_V_4 <= 32'sh0;
                       T401_test58_sqrt_Sqrt_1_12_V_3 <= 64'h0;
                       T401_test58_double_test_0_9_V_1 <= 64'h0;
                       T401_test58_double_test_0_9_V_0 <= 64'sh0;
                       T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= 32'h0;
                       T401_test58_single_test_0_6_V_0 <= 64'sh2;
                       KppWaypoint0 <= "Starting Single";
                       bevelab10nz <= 32'h1/*1:bevelab10nz*/;
                       end 
                      
                  32'h34/*52:bevelab10nz*/: $display("    p=%1d   pf=%f", T401_test58_single_test_0_6_V_0, $bitstoreal(hpr_flt2dbl0(((32'h4
                  /*4:bevelab10nz*/==bevelab10nz)? fpcvt10_result: fpcvt10RRh10hold))));
              endcase
              if ((64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000)) 
                  case (bevelab10nz)
                      32'h35/*53:bevelab10nz*/:  bevelab10nz <= 32'h36/*54:bevelab10nz*/;

                      32'h4d/*77:bevelab10nz*/:  T401_test58_single_test_0_6_V_0 <= 64'shcc*T401_test58_single_test_0_6_V_0;
                  endcase
                   else 
                  case (bevelab10nz)
                      32'h35/*53:bevelab10nz*/:  bevelab10nz <= 32'h4e/*78:bevelab10nz*/;

                      32'h65/*101:bevelab10nz*/:  begin 
                           T401_test58_double_test_0_9_V_0 <= 64'sh2;
                           T401_test58_single_test_0_6_V_0 <= 64'shcc*T401_test58_single_test_0_6_V_0;
                           KppWaypoint0 <= "Starting Double";
                           end 
                          endcase

              case (bevelab10nz)
                  32'h65/*101:bevelab10nz*/:  bevelab10nz <= 32'h66/*102:bevelab10nz*/;

                  32'h66/*102:bevelab10nz*/: if ((T401_test58_double_test_0_9_V_0<64'sh3_8d7e_a4c6_8000))  bevelab10nz <= 32'h67/*103:bevelab10nz*/;
                       else  begin 
                           KppWaypoint0 <= "Finished";
                           bevelab10nz <= 32'h85/*133:bevelab10nz*/;
                           end 
                          
                  32'h6a/*106:bevelab10nz*/:  begin 
                      if (((32'h6a/*106:bevelab10nz*/==bevelab10nz)? hpr_fp_dltd1(64'h0, fpcvt16_result): hpr_fp_dltd1(64'h0, fpcvt16RRh10hold
                      )))  begin 
                               T401_test58_sqrt_Sqrt_1_12_V_4 <= 32'sd0;
                               T401_test58_sqrt_Sqrt_1_12_V_3 <= 64'sh1ff8_0000_0000_0000+($unsigned(((32'h6a/*106:bevelab10nz*/==bevelab10nz
                              )? fpcvt16_result: fpcvt16RRh10hold))>>32'sd1)+(0-(rtl_unsigned_extend2(A_UINT_CC_T1_T1_SCALbx10_T1_ARA0
                              [rtl_signed_bitextract3(64'sh1f&(64'sh1ff8_0000_0000_0000+($unsigned(((32'h6a/*106:bevelab10nz*/==bevelab10nz
                              )? fpcvt16_result: fpcvt16RRh10hold))>>32'sd1)>>32'sd47))])<<32'sd32));

                               T401_test58_double_test_0_9_V_1 <= ((32'h6a/*106:bevelab10nz*/==bevelab10nz)? fpcvt16_result: fpcvt16RRh10hold
                              );

                               bevelab10nz <= 32'h6b/*107:bevelab10nz*/;
                               end 
                              if (((32'h6a/*106:bevelab10nz*/==bevelab10nz)? !hpr_fp_dltd1(64'h0, fpcvt16_result): !hpr_fp_dltd1(64'h0
                      , fpcvt16RRh10hold)))  begin 
                               T401_test58_sqrt_Sqrt_1_12_SPILL_256 <= 64'h0;
                               T401_test58_double_test_0_9_V_1 <= ((32'h6a/*106:bevelab10nz*/==bevelab10nz)? fpcvt16_result: fpcvt16RRh10hold
                              );

                               bevelab10nz <= 32'h78/*120:bevelab10nz*/;
                               end 
                               end 
                      endcase
              if ((T401_test58_sqrt_Sqrt_1_12_V_4<32'sd3)) 
                  case (bevelab10nz)
                      32'h6b/*107:bevelab10nz*/:  bevelab10nz <= 32'h6c/*108:bevelab10nz*/;

                      32'h77/*119:bevelab10nz*/:  begin 
                           T401_test58_sqrt_Sqrt_1_12_V_4 <= 32'sd1+T401_test58_sqrt_Sqrt_1_12_V_4;
                           T401_test58_sqrt_Sqrt_1_12_V_3 <= ((32'h77/*119:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER18_FPRR: CVFPMULTIPLIER18RRh10hold
                          );

                           end 
                          endcase
                   else if ((32'h6b/*107:bevelab10nz*/==bevelab10nz))  begin 
                           T401_test58_sqrt_Sqrt_1_12_SPILL_256 <= T401_test58_sqrt_Sqrt_1_12_V_3;
                           bevelab10nz <= 32'h78/*120:bevelab10nz*/;
                           end 
                          
              case (bevelab10nz)
                  32'h34/*52:bevelab10nz*/:  begin 
                       T401_test58_sqrt_Sqrt_sp_1_12_SPILL_256 <= (((32'h4/*4:bevelab10nz*/==bevelab10nz)? hpr_fp_dltd4(32'h0, fpcvt10_result
                      ): hpr_fp_dltd4(32'h0, fpcvt10RRh10hold))? ((32'h34/*52:bevelab10nz*/==bevelab10nz)? CVFPMULTIPLIER12_FPRR: CVFPMULTIPLIER12RRh10hold
                      ): 32'h0);

                       bevelab10nz <= 32'h35/*53:bevelab10nz*/;
                       end 
                      
                  32'h4d/*77:bevelab10nz*/:  bevelab10nz <= 32'h1/*1:bevelab10nz*/;

                  32'h77/*119:bevelab10nz*/:  bevelab10nz <= 32'h6b/*107:bevelab10nz*/;

                  32'h84/*132:bevelab10nz*/:  begin 
                       T401_test58_double_test_0_9_V_0 <= 64'shcc*T401_test58_double_test_0_9_V_0;
                       bevelab10nz <= 32'h66/*102:bevelab10nz*/;
                       end 
                      endcase
              if (CVFPADDER20RRh10shot3)  CVFPADDER20RRh10hold <= CVFPADDER20_FPRR;
                  if (fpcvt18RRh10shot1)  fpcvt18RRh10hold <= fpcvt18_result;
                  if (CVFPMULTIPLIER20RRh10shot2)  CVFPMULTIPLIER20RRh10hold <= CVFPMULTIPLIER20_FPRR;
                  if (CVFPMULTIPLIER18RRh10shot2)  CVFPMULTIPLIER18RRh10hold <= CVFPMULTIPLIER18_FPRR;
                  if (CVFPADDER18RRh10shot3)  CVFPADDER18RRh10hold <= CVFPADDER18_FPRR;
                  if (CVFPDIVIDER14RRh10shot4)  CVFPDIVIDER14RRh10hold <= CVFPDIVIDER14_FPRR;
                  if (fpcvt16RRh10shot1)  fpcvt16RRh10hold <= fpcvt16_result;
                  if (CVFPADDER14RRh10shot3)  CVFPADDER14RRh10hold <= CVFPADDER14_FPRR;
                  if (fpcvt12RRh10shot1)  fpcvt12RRh10hold <= fpcvt12_result;
                  if (CVFPMULTIPLIER14RRh10shot4)  CVFPMULTIPLIER14RRh10hold <= CVFPMULTIPLIER14_FPRR;
                  if (CVFPADDER16RRh10shot3)  CVFPADDER16RRh10hold <= CVFPADDER16_FPRR;
                  if (fpcvt14RRh10shot1)  fpcvt14RRh10hold <= fpcvt14_result;
                  if (CVFPMULTIPLIER16RRh10shot4)  CVFPMULTIPLIER16RRh10hold <= CVFPMULTIPLIER16_FPRR;
                  if (CVFPMULTIPLIER12RRh10shot4)  CVFPMULTIPLIER12RRh10hold <= CVFPMULTIPLIER12_FPRR;
                  if (CVFPADDER12RRh10shot3)  CVFPADDER12RRh10hold <= CVFPADDER12_FPRR;
                  if (CVFPDIVIDER12RRh10shot14)  CVFPDIVIDER12RRh10hold <= CVFPDIVIDER12_FPRR;
                  if (CVFPMULTIPLIER10RRh10shot4)  CVFPMULTIPLIER10RRh10hold <= CVFPMULTIPLIER10_FPRR;
                  if (CVFPADDER10RRh10shot3)  CVFPADDER10RRh10hold <= CVFPADDER10_FPRR;
                  if (CVFPDIVIDER10RRh10shot14)  CVFPDIVIDER10RRh10hold <= CVFPDIVIDER10_FPRR;
                  if (fpcvt10RRh10shot1)  fpcvt10RRh10hold <= fpcvt10_result;
                  
              case (bevelab10nz)
                  32'h1/*1:bevelab10nz*/:  bevelab10nz <= 32'h2/*2:bevelab10nz*/;

                  32'h2/*2:bevelab10nz*/:  bevelab10nz <= 32'h3/*3:bevelab10nz*/;

                  32'h3/*3:bevelab10nz*/:  bevelab10nz <= 32'h4/*4:bevelab10nz*/;

                  32'h4/*4:bevelab10nz*/:  bevelab10nz <= 32'h5/*5:bevelab10nz*/;

                  32'h5/*5:bevelab10nz*/:  bevelab10nz <= 32'h6/*6:bevelab10nz*/;

                  32'h6/*6:bevelab10nz*/:  bevelab10nz <= 32'h7/*7:bevelab10nz*/;

                  32'h7/*7:bevelab10nz*/:  bevelab10nz <= 32'h8/*8:bevelab10nz*/;

                  32'h8/*8:bevelab10nz*/:  bevelab10nz <= 32'h9/*9:bevelab10nz*/;

                  32'h9/*9:bevelab10nz*/:  bevelab10nz <= 32'ha/*10:bevelab10nz*/;

                  32'ha/*10:bevelab10nz*/:  bevelab10nz <= 32'hb/*11:bevelab10nz*/;

                  32'hb/*11:bevelab10nz*/:  bevelab10nz <= 32'hc/*12:bevelab10nz*/;

                  32'hc/*12:bevelab10nz*/:  bevelab10nz <= 32'hd/*13:bevelab10nz*/;

                  32'hd/*13:bevelab10nz*/:  bevelab10nz <= 32'he/*14:bevelab10nz*/;

                  32'he/*14:bevelab10nz*/:  bevelab10nz <= 32'hf/*15:bevelab10nz*/;

                  32'hf/*15:bevelab10nz*/:  bevelab10nz <= 32'h10/*16:bevelab10nz*/;

                  32'h10/*16:bevelab10nz*/:  bevelab10nz <= 32'h11/*17:bevelab10nz*/;

                  32'h11/*17:bevelab10nz*/:  bevelab10nz <= 32'h12/*18:bevelab10nz*/;

                  32'h12/*18:bevelab10nz*/:  bevelab10nz <= 32'h13/*19:bevelab10nz*/;

                  32'h13/*19:bevelab10nz*/:  bevelab10nz <= 32'h14/*20:bevelab10nz*/;

                  32'h14/*20:bevelab10nz*/:  bevelab10nz <= 32'h15/*21:bevelab10nz*/;

                  32'h15/*21:bevelab10nz*/:  bevelab10nz <= 32'h16/*22:bevelab10nz*/;

                  32'h16/*22:bevelab10nz*/:  bevelab10nz <= 32'h17/*23:bevelab10nz*/;

                  32'h17/*23:bevelab10nz*/:  bevelab10nz <= 32'h18/*24:bevelab10nz*/;

                  32'h18/*24:bevelab10nz*/:  bevelab10nz <= 32'h19/*25:bevelab10nz*/;

                  32'h19/*25:bevelab10nz*/:  bevelab10nz <= 32'h1a/*26:bevelab10nz*/;

                  32'h1a/*26:bevelab10nz*/:  bevelab10nz <= 32'h1b/*27:bevelab10nz*/;

                  32'h1b/*27:bevelab10nz*/:  bevelab10nz <= 32'h1c/*28:bevelab10nz*/;

                  32'h1c/*28:bevelab10nz*/:  bevelab10nz <= 32'h1d/*29:bevelab10nz*/;

                  32'h1d/*29:bevelab10nz*/:  bevelab10nz <= 32'h1e/*30:bevelab10nz*/;

                  32'h1e/*30:bevelab10nz*/:  bevelab10nz <= 32'h1f/*31:bevelab10nz*/;

                  32'h1f/*31:bevelab10nz*/:  bevelab10nz <= 32'h20/*32:bevelab10nz*/;

                  32'h20/*32:bevelab10nz*/:  bevelab10nz <= 32'h21/*33:bevelab10nz*/;

                  32'h21/*33:bevelab10nz*/:  bevelab10nz <= 32'h22/*34:bevelab10nz*/;

                  32'h22/*34:bevelab10nz*/:  bevelab10nz <= 32'h23/*35:bevelab10nz*/;

                  32'h23/*35:bevelab10nz*/:  bevelab10nz <= 32'h24/*36:bevelab10nz*/;

                  32'h24/*36:bevelab10nz*/:  bevelab10nz <= 32'h25/*37:bevelab10nz*/;

                  32'h25/*37:bevelab10nz*/:  bevelab10nz <= 32'h26/*38:bevelab10nz*/;

                  32'h26/*38:bevelab10nz*/:  bevelab10nz <= 32'h27/*39:bevelab10nz*/;

                  32'h27/*39:bevelab10nz*/:  bevelab10nz <= 32'h28/*40:bevelab10nz*/;

                  32'h28/*40:bevelab10nz*/:  bevelab10nz <= 32'h29/*41:bevelab10nz*/;

                  32'h29/*41:bevelab10nz*/:  bevelab10nz <= 32'h2a/*42:bevelab10nz*/;

                  32'h2a/*42:bevelab10nz*/:  bevelab10nz <= 32'h2b/*43:bevelab10nz*/;

                  32'h2b/*43:bevelab10nz*/:  bevelab10nz <= 32'h2c/*44:bevelab10nz*/;

                  32'h2c/*44:bevelab10nz*/:  bevelab10nz <= 32'h2d/*45:bevelab10nz*/;

                  32'h2d/*45:bevelab10nz*/:  bevelab10nz <= 32'h2e/*46:bevelab10nz*/;

                  32'h2e/*46:bevelab10nz*/:  bevelab10nz <= 32'h2f/*47:bevelab10nz*/;

                  32'h2f/*47:bevelab10nz*/:  bevelab10nz <= 32'h30/*48:bevelab10nz*/;

                  32'h30/*48:bevelab10nz*/:  bevelab10nz <= 32'h31/*49:bevelab10nz*/;

                  32'h31/*49:bevelab10nz*/:  bevelab10nz <= 32'h32/*50:bevelab10nz*/;

                  32'h32/*50:bevelab10nz*/:  bevelab10nz <= 32'h33/*51:bevelab10nz*/;

                  32'h33/*51:bevelab10nz*/:  bevelab10nz <= 32'h34/*52:bevelab10nz*/;

                  32'h36/*54:bevelab10nz*/:  bevelab10nz <= 32'h37/*55:bevelab10nz*/;

                  32'h37/*55:bevelab10nz*/:  bevelab10nz <= 32'h38/*56:bevelab10nz*/;

                  32'h38/*56:bevelab10nz*/:  bevelab10nz <= 32'h39/*57:bevelab10nz*/;

                  32'h39/*57:bevelab10nz*/:  bevelab10nz <= 32'h3a/*58:bevelab10nz*/;

                  32'h3a/*58:bevelab10nz*/:  bevelab10nz <= 32'h3b/*59:bevelab10nz*/;

                  32'h3b/*59:bevelab10nz*/:  bevelab10nz <= 32'h3c/*60:bevelab10nz*/;

                  32'h3c/*60:bevelab10nz*/:  bevelab10nz <= 32'h3d/*61:bevelab10nz*/;

                  32'h3d/*61:bevelab10nz*/:  bevelab10nz <= 32'h3e/*62:bevelab10nz*/;

                  32'h3e/*62:bevelab10nz*/:  bevelab10nz <= 32'h3f/*63:bevelab10nz*/;

                  32'h3f/*63:bevelab10nz*/:  bevelab10nz <= 32'h40/*64:bevelab10nz*/;

                  32'h40/*64:bevelab10nz*/:  bevelab10nz <= 32'h41/*65:bevelab10nz*/;

                  32'h41/*65:bevelab10nz*/:  bevelab10nz <= 32'h42/*66:bevelab10nz*/;

                  32'h42/*66:bevelab10nz*/:  bevelab10nz <= 32'h43/*67:bevelab10nz*/;

                  32'h43/*67:bevelab10nz*/:  bevelab10nz <= 32'h44/*68:bevelab10nz*/;

                  32'h44/*68:bevelab10nz*/:  bevelab10nz <= 32'h45/*69:bevelab10nz*/;

                  32'h45/*69:bevelab10nz*/:  bevelab10nz <= 32'h46/*70:bevelab10nz*/;

                  32'h46/*70:bevelab10nz*/:  bevelab10nz <= 32'h47/*71:bevelab10nz*/;

                  32'h47/*71:bevelab10nz*/:  bevelab10nz <= 32'h48/*72:bevelab10nz*/;

                  32'h48/*72:bevelab10nz*/:  bevelab10nz <= 32'h49/*73:bevelab10nz*/;

                  32'h49/*73:bevelab10nz*/:  bevelab10nz <= 32'h4a/*74:bevelab10nz*/;

                  32'h4a/*74:bevelab10nz*/:  bevelab10nz <= 32'h4b/*75:bevelab10nz*/;

                  32'h4b/*75:bevelab10nz*/:  bevelab10nz <= 32'h4c/*76:bevelab10nz*/;

                  32'h4c/*76:bevelab10nz*/:  bevelab10nz <= 32'h4d/*77:bevelab10nz*/;

                  32'h4e/*78:bevelab10nz*/:  bevelab10nz <= 32'h4f/*79:bevelab10nz*/;

                  32'h4f/*79:bevelab10nz*/:  bevelab10nz <= 32'h50/*80:bevelab10nz*/;

                  32'h50/*80:bevelab10nz*/:  bevelab10nz <= 32'h51/*81:bevelab10nz*/;

                  32'h51/*81:bevelab10nz*/:  bevelab10nz <= 32'h52/*82:bevelab10nz*/;

                  32'h52/*82:bevelab10nz*/:  bevelab10nz <= 32'h53/*83:bevelab10nz*/;

                  32'h53/*83:bevelab10nz*/:  bevelab10nz <= 32'h54/*84:bevelab10nz*/;

                  32'h54/*84:bevelab10nz*/:  bevelab10nz <= 32'h55/*85:bevelab10nz*/;

                  32'h55/*85:bevelab10nz*/:  bevelab10nz <= 32'h56/*86:bevelab10nz*/;

                  32'h56/*86:bevelab10nz*/:  bevelab10nz <= 32'h57/*87:bevelab10nz*/;

                  32'h57/*87:bevelab10nz*/:  bevelab10nz <= 32'h58/*88:bevelab10nz*/;

                  32'h58/*88:bevelab10nz*/:  bevelab10nz <= 32'h59/*89:bevelab10nz*/;

                  32'h59/*89:bevelab10nz*/:  bevelab10nz <= 32'h5a/*90:bevelab10nz*/;

                  32'h5a/*90:bevelab10nz*/:  bevelab10nz <= 32'h5b/*91:bevelab10nz*/;

                  32'h5b/*91:bevelab10nz*/:  bevelab10nz <= 32'h5c/*92:bevelab10nz*/;

                  32'h5c/*92:bevelab10nz*/:  bevelab10nz <= 32'h5d/*93:bevelab10nz*/;

                  32'h5d/*93:bevelab10nz*/:  bevelab10nz <= 32'h5e/*94:bevelab10nz*/;

                  32'h5e/*94:bevelab10nz*/:  bevelab10nz <= 32'h5f/*95:bevelab10nz*/;

                  32'h5f/*95:bevelab10nz*/:  bevelab10nz <= 32'h60/*96:bevelab10nz*/;

                  32'h60/*96:bevelab10nz*/:  bevelab10nz <= 32'h61/*97:bevelab10nz*/;

                  32'h61/*97:bevelab10nz*/:  bevelab10nz <= 32'h62/*98:bevelab10nz*/;

                  32'h62/*98:bevelab10nz*/:  bevelab10nz <= 32'h63/*99:bevelab10nz*/;

                  32'h63/*99:bevelab10nz*/:  bevelab10nz <= 32'h64/*100:bevelab10nz*/;

                  32'h64/*100:bevelab10nz*/:  bevelab10nz <= 32'h65/*101:bevelab10nz*/;

                  32'h67/*103:bevelab10nz*/:  bevelab10nz <= 32'h68/*104:bevelab10nz*/;

                  32'h68/*104:bevelab10nz*/:  bevelab10nz <= 32'h69/*105:bevelab10nz*/;

                  32'h69/*105:bevelab10nz*/:  bevelab10nz <= 32'h6a/*106:bevelab10nz*/;

                  32'h6c/*108:bevelab10nz*/:  bevelab10nz <= 32'h6d/*109:bevelab10nz*/;

                  32'h6d/*109:bevelab10nz*/:  bevelab10nz <= 32'h6e/*110:bevelab10nz*/;

                  32'h6e/*110:bevelab10nz*/:  bevelab10nz <= 32'h6f/*111:bevelab10nz*/;

                  32'h6f/*111:bevelab10nz*/:  bevelab10nz <= 32'h70/*112:bevelab10nz*/;

                  32'h70/*112:bevelab10nz*/:  bevelab10nz <= 32'h71/*113:bevelab10nz*/;

                  32'h71/*113:bevelab10nz*/:  bevelab10nz <= 32'h72/*114:bevelab10nz*/;

                  32'h72/*114:bevelab10nz*/:  bevelab10nz <= 32'h73/*115:bevelab10nz*/;

                  32'h73/*115:bevelab10nz*/:  bevelab10nz <= 32'h74/*116:bevelab10nz*/;

                  32'h74/*116:bevelab10nz*/:  bevelab10nz <= 32'h75/*117:bevelab10nz*/;

                  32'h75/*117:bevelab10nz*/:  bevelab10nz <= 32'h76/*118:bevelab10nz*/;

                  32'h76/*118:bevelab10nz*/:  bevelab10nz <= 32'h77/*119:bevelab10nz*/;

                  32'h78/*120:bevelab10nz*/:  bevelab10nz <= 32'h79/*121:bevelab10nz*/;

                  32'h79/*121:bevelab10nz*/:  bevelab10nz <= 32'h7a/*122:bevelab10nz*/;

                  32'h7a/*122:bevelab10nz*/:  bevelab10nz <= 32'h7b/*123:bevelab10nz*/;

                  32'h7b/*123:bevelab10nz*/:  bevelab10nz <= 32'h7c/*124:bevelab10nz*/;

                  32'h7c/*124:bevelab10nz*/:  bevelab10nz <= 32'h7d/*125:bevelab10nz*/;

                  32'h7d/*125:bevelab10nz*/:  bevelab10nz <= 32'h7e/*126:bevelab10nz*/;

                  32'h7e/*126:bevelab10nz*/:  bevelab10nz <= 32'h7f/*127:bevelab10nz*/;

                  32'h7f/*127:bevelab10nz*/:  bevelab10nz <= 32'h80/*128:bevelab10nz*/;

                  32'h80/*128:bevelab10nz*/:  bevelab10nz <= 32'h81/*129:bevelab10nz*/;

                  32'h81/*129:bevelab10nz*/:  bevelab10nz <= 32'h82/*130:bevelab10nz*/;

                  32'h82/*130:bevelab10nz*/:  bevelab10nz <= 32'h83/*131:bevelab10nz*/;

                  32'h83/*131:bevelab10nz*/:  bevelab10nz <= 32'h84/*132:bevelab10nz*/;
              endcase
               bevelab10nz_pc_export <= bevelab10nz;
               fpcvt10RRh10shot1 <= fpcvt10RRh10shot0;
               CVFPDIVIDER10RRh10shot1 <= CVFPDIVIDER10RRh10shot0;
               CVFPDIVIDER10RRh10shot2 <= CVFPDIVIDER10RRh10shot1;
               CVFPDIVIDER10RRh10shot3 <= CVFPDIVIDER10RRh10shot2;
               CVFPDIVIDER10RRh10shot4 <= CVFPDIVIDER10RRh10shot3;
               CVFPDIVIDER10RRh10shot5 <= CVFPDIVIDER10RRh10shot4;
               CVFPDIVIDER10RRh10shot6 <= CVFPDIVIDER10RRh10shot5;
               CVFPDIVIDER10RRh10shot7 <= CVFPDIVIDER10RRh10shot6;
               CVFPDIVIDER10RRh10shot8 <= CVFPDIVIDER10RRh10shot7;
               CVFPDIVIDER10RRh10shot9 <= CVFPDIVIDER10RRh10shot8;
               CVFPDIVIDER10RRh10shot10 <= CVFPDIVIDER10RRh10shot9;
               CVFPDIVIDER10RRh10shot11 <= CVFPDIVIDER10RRh10shot10;
               CVFPDIVIDER10RRh10shot12 <= CVFPDIVIDER10RRh10shot11;
               CVFPDIVIDER10RRh10shot13 <= CVFPDIVIDER10RRh10shot12;
               CVFPDIVIDER10RRh10shot14 <= CVFPDIVIDER10RRh10shot13;
               CVFPADDER10RRh10shot1 <= CVFPADDER10RRh10shot0;
               CVFPADDER10RRh10shot2 <= CVFPADDER10RRh10shot1;
               CVFPADDER10RRh10shot3 <= CVFPADDER10RRh10shot2;
               CVFPMULTIPLIER10RRh10shot1 <= CVFPMULTIPLIER10RRh10shot0;
               CVFPMULTIPLIER10RRh10shot2 <= CVFPMULTIPLIER10RRh10shot1;
               CVFPMULTIPLIER10RRh10shot3 <= CVFPMULTIPLIER10RRh10shot2;
               CVFPMULTIPLIER10RRh10shot4 <= CVFPMULTIPLIER10RRh10shot3;
               CVFPDIVIDER12RRh10shot1 <= CVFPDIVIDER12RRh10shot0;
               CVFPDIVIDER12RRh10shot2 <= CVFPDIVIDER12RRh10shot1;
               CVFPDIVIDER12RRh10shot3 <= CVFPDIVIDER12RRh10shot2;
               CVFPDIVIDER12RRh10shot4 <= CVFPDIVIDER12RRh10shot3;
               CVFPDIVIDER12RRh10shot5 <= CVFPDIVIDER12RRh10shot4;
               CVFPDIVIDER12RRh10shot6 <= CVFPDIVIDER12RRh10shot5;
               CVFPDIVIDER12RRh10shot7 <= CVFPDIVIDER12RRh10shot6;
               CVFPDIVIDER12RRh10shot8 <= CVFPDIVIDER12RRh10shot7;
               CVFPDIVIDER12RRh10shot9 <= CVFPDIVIDER12RRh10shot8;
               CVFPDIVIDER12RRh10shot10 <= CVFPDIVIDER12RRh10shot9;
               CVFPDIVIDER12RRh10shot11 <= CVFPDIVIDER12RRh10shot10;
               CVFPDIVIDER12RRh10shot12 <= CVFPDIVIDER12RRh10shot11;
               CVFPDIVIDER12RRh10shot13 <= CVFPDIVIDER12RRh10shot12;
               CVFPDIVIDER12RRh10shot14 <= CVFPDIVIDER12RRh10shot13;
               CVFPADDER12RRh10shot1 <= CVFPADDER12RRh10shot0;
               CVFPADDER12RRh10shot2 <= CVFPADDER12RRh10shot1;
               CVFPADDER12RRh10shot3 <= CVFPADDER12RRh10shot2;
               CVFPMULTIPLIER12RRh10shot1 <= CVFPMULTIPLIER12RRh10shot0;
               CVFPMULTIPLIER12RRh10shot2 <= CVFPMULTIPLIER12RRh10shot1;
               CVFPMULTIPLIER12RRh10shot3 <= CVFPMULTIPLIER12RRh10shot2;
               CVFPMULTIPLIER12RRh10shot4 <= CVFPMULTIPLIER12RRh10shot3;
               CVFPMULTIPLIER16RRh10shot1 <= CVFPMULTIPLIER16RRh10shot0;
               CVFPMULTIPLIER16RRh10shot2 <= CVFPMULTIPLIER16RRh10shot1;
               CVFPMULTIPLIER16RRh10shot3 <= CVFPMULTIPLIER16RRh10shot2;
               CVFPMULTIPLIER16RRh10shot4 <= CVFPMULTIPLIER16RRh10shot3;
               fpcvt14RRh10shot1 <= fpcvt14RRh10shot0;
               CVFPADDER16RRh10shot1 <= CVFPADDER16RRh10shot0;
               CVFPADDER16RRh10shot2 <= CVFPADDER16RRh10shot1;
               CVFPADDER16RRh10shot3 <= CVFPADDER16RRh10shot2;
               CVFPMULTIPLIER14RRh10shot1 <= CVFPMULTIPLIER14RRh10shot0;
               CVFPMULTIPLIER14RRh10shot2 <= CVFPMULTIPLIER14RRh10shot1;
               CVFPMULTIPLIER14RRh10shot3 <= CVFPMULTIPLIER14RRh10shot2;
               CVFPMULTIPLIER14RRh10shot4 <= CVFPMULTIPLIER14RRh10shot3;
               fpcvt12RRh10shot1 <= fpcvt12RRh10shot0;
               CVFPADDER14RRh10shot1 <= CVFPADDER14RRh10shot0;
               CVFPADDER14RRh10shot2 <= CVFPADDER14RRh10shot1;
               CVFPADDER14RRh10shot3 <= CVFPADDER14RRh10shot2;
               fpcvt16RRh10shot1 <= fpcvt16RRh10shot0;
               CVFPDIVIDER14RRh10shot1 <= CVFPDIVIDER14RRh10shot0;
               CVFPDIVIDER14RRh10shot2 <= CVFPDIVIDER14RRh10shot1;
               CVFPDIVIDER14RRh10shot3 <= CVFPDIVIDER14RRh10shot2;
               CVFPDIVIDER14RRh10shot4 <= CVFPDIVIDER14RRh10shot3;
               CVFPADDER18RRh10shot1 <= CVFPADDER18RRh10shot0;
               CVFPADDER18RRh10shot2 <= CVFPADDER18RRh10shot1;
               CVFPADDER18RRh10shot3 <= CVFPADDER18RRh10shot2;
               CVFPMULTIPLIER18RRh10shot1 <= CVFPMULTIPLIER18RRh10shot0;
               CVFPMULTIPLIER18RRh10shot2 <= CVFPMULTIPLIER18RRh10shot1;
               CVFPMULTIPLIER20RRh10shot1 <= CVFPMULTIPLIER20RRh10shot0;
               CVFPMULTIPLIER20RRh10shot2 <= CVFPMULTIPLIER20RRh10shot1;
               fpcvt18RRh10shot1 <= fpcvt18RRh10shot0;
               CVFPADDER20RRh10shot1 <= CVFPADDER20RRh10shot0;
               CVFPADDER20RRh10shot2 <= CVFPADDER20RRh10shot1;
               CVFPADDER20RRh10shot3 <= CVFPADDER20RRh10shot2;
               CVFPDIVIDER14RRh10shot0 <= (32'h7f/*127:bevelab10nz*/==bevelab10nz) || (T401_test58_sqrt_Sqrt_1_12_V_4<32'sd3) && (32'h6b
              /*107:bevelab10nz*/==bevelab10nz);

               CVFPADDER20RRh10shot0 <= (32'h7b/*123:bevelab10nz*/==bevelab10nz);
               fpcvt18RRh10shot0 <= (32'h78/*120:bevelab10nz*/==bevelab10nz);
               CVFPMULTIPLIER20RRh10shot0 <= (32'h78/*120:bevelab10nz*/==bevelab10nz);
               CVFPMULTIPLIER18RRh10shot0 <= (T401_test58_sqrt_Sqrt_1_12_V_4<32'sd3) && (32'h74/*116:bevelab10nz*/==bevelab10nz);
               CVFPADDER18RRh10shot0 <= (T401_test58_sqrt_Sqrt_1_12_V_4<32'sd3) && (32'h70/*112:bevelab10nz*/==bevelab10nz);
               fpcvt16RRh10shot0 <= (32'h68/*104:bevelab10nz*/==bevelab10nz);
               CVFPDIVIDER12RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000) && (32'h3e/*62:bevelab10nz*/==
              bevelab10nz) || (32'h1c/*28:bevelab10nz*/==bevelab10nz);

               CVFPADDER14RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000) && (32'h3a/*58:bevelab10nz*/==
              bevelab10nz);

               fpcvt12RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000) && (32'h35/*53:bevelab10nz*/==bevelab10nz
              );

               CVFPMULTIPLIER14RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0<64'sh3_8d7e_a4c6_8000) && (32'h35/*53:bevelab10nz*/==
              bevelab10nz);

               CVFPDIVIDER10RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0>=64'sh3_8d7e_a4c6_8000) && (32'h56/*86:bevelab10nz*/==
              bevelab10nz) || (32'h4/*4:bevelab10nz*/==bevelab10nz);

               CVFPADDER16RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0>=64'sh3_8d7e_a4c6_8000) && (32'h52/*82:bevelab10nz*/==
              bevelab10nz);

               fpcvt14RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0>=64'sh3_8d7e_a4c6_8000) && (32'h35/*53:bevelab10nz*/==bevelab10nz
              );

               CVFPMULTIPLIER16RRh10shot0 <= (64'shcc*T401_test58_single_test_0_6_V_0>=64'sh3_8d7e_a4c6_8000) && (32'h35/*53:bevelab10nz*/==
              bevelab10nz);

               CVFPMULTIPLIER12RRh10shot0 <= (32'h2f/*47:bevelab10nz*/==bevelab10nz);
               CVFPADDER12RRh10shot0 <= (32'h2b/*43:bevelab10nz*/==bevelab10nz);
               CVFPMULTIPLIER10RRh10shot0 <= (32'h17/*23:bevelab10nz*/==bevelab10nz);
               CVFPADDER10RRh10shot0 <= (32'h13/*19:bevelab10nz*/==bevelab10nz);
               fpcvt10RRh10shot0 <= (32'h2/*2:bevelab10nz*/==bevelab10nz);
               end 
              //End structure HPR test58/1.0


       end 
      

  CV_FP_CVT_FL2_F32_I64 fpcvt10(
        .clk(clk),
        .reset(reset),
        .result(fpcvt10_result),
        .arg(fpcvt10_arg),
        .fail(fpcvt10_fail
));
  CV_FP_FL15_SP_DIVIDER CVFPDIVIDER10(
        clk,
        reset,
        CVFPDIVIDER10_FPRR,
        CVFPDIVIDER10_NN,
        CVFPDIVIDER10_DD
,
        CVFPDIVIDER10_fail);
  CV_FP_FL4_SP_ADDER CVFPADDER10(
        clk,
        reset,
        CVFPADDER10_FPRR,
        CVFPADDER10_A0,
        CVFPADDER10_A1
,
        CVFPADDER10_fail);
  CV_FP_FL5_SP_MULTIPLIER CVFPMULTIPLIER10(
        clk,
        reset,
        CVFPMULTIPLIER10_FPRR,
        CVFPMULTIPLIER10_A0,
        CVFPMULTIPLIER10_A1
,
        CVFPMULTIPLIER10_fail);
  CV_FP_FL15_SP_DIVIDER CVFPDIVIDER12(
        clk,
        reset,
        CVFPDIVIDER12_FPRR,
        CVFPDIVIDER12_NN,
        CVFPDIVIDER12_DD
,
        CVFPDIVIDER12_fail);
  CV_FP_FL4_SP_ADDER CVFPADDER12(
        clk,
        reset,
        CVFPADDER12_FPRR,
        CVFPADDER12_A0,
        CVFPADDER12_A1
,
        CVFPADDER12_fail);
  CV_FP_FL5_SP_MULTIPLIER CVFPMULTIPLIER12(
        clk,
        reset,
        CVFPMULTIPLIER12_FPRR,
        CVFPMULTIPLIER12_A0,
        CVFPMULTIPLIER12_A1
,
        CVFPMULTIPLIER12_fail);
  CV_FP_CVT_FL2_F32_I64 fpcvt12(
        .clk(clk),
        .reset(reset),
        .result(fpcvt12_result),
        .arg(fpcvt12_arg),
        .fail(fpcvt12_fail
));
  CV_FP_FL5_SP_MULTIPLIER CVFPMULTIPLIER14(
        clk,
        reset,
        CVFPMULTIPLIER14_FPRR,
        CVFPMULTIPLIER14_A0,
        CVFPMULTIPLIER14_A1
,
        CVFPMULTIPLIER14_fail);
  CV_FP_FL4_SP_ADDER CVFPADDER14(
        clk,
        reset,
        CVFPADDER14_FPRR,
        CVFPADDER14_A0,
        CVFPADDER14_A1
,
        CVFPADDER14_fail);
  CV_FP_CVT_FL2_F32_I64 fpcvt14(
        .clk(clk),
        .reset(reset),
        .result(fpcvt14_result),
        .arg(fpcvt14_arg),
        .fail(fpcvt14_fail
));
  CV_FP_FL5_SP_MULTIPLIER CVFPMULTIPLIER16(
        clk,
        reset,
        CVFPMULTIPLIER16_FPRR,
        CVFPMULTIPLIER16_A0,
        CVFPMULTIPLIER16_A1
,
        CVFPMULTIPLIER16_fail);
  CV_FP_FL4_SP_ADDER CVFPADDER16(
        clk,
        reset,
        CVFPADDER16_FPRR,
        CVFPADDER16_A0,
        CVFPADDER16_A1
,
        CVFPADDER16_fail);
  CV_FP_CVT_FL2_F64_I64 fpcvt16(
        .clk(clk),
        .reset(reset),
        .result(fpcvt16_result),
        .arg(fpcvt16_arg),
        .fail(fpcvt16_fail
));
  CV_FP_FL5_DP_DIVIDER CVFPDIVIDER14(
        clk,
        reset,
        CVFPDIVIDER14_FPRR,
        CVFPDIVIDER14_NN,
        CVFPDIVIDER14_DD
,
        CVFPDIVIDER14_fail);
  CV_FP_FL4_DP_ADDER CVFPADDER18(
        clk,
        reset,
        CVFPADDER18_FPRR,
        CVFPADDER18_A0,
        CVFPADDER18_A1
,
        CVFPADDER18_fail);
  CV_FP_FL3_DP_MULTIPLIER CVFPMULTIPLIER18(
        clk,
        reset,
        CVFPMULTIPLIER18_FPRR,
        CVFPMULTIPLIER18_A0,
        CVFPMULTIPLIER18_A1
,
        CVFPMULTIPLIER18_fail);
  CV_FP_CVT_FL2_F64_I64 fpcvt18(
        .clk(clk),
        .reset(reset),
        .result(fpcvt18_result),
        .arg(fpcvt18_arg),
        .fail(fpcvt18_fail
));
  CV_FP_FL3_DP_MULTIPLIER CVFPMULTIPLIER20(
        clk,
        reset,
        CVFPMULTIPLIER20_FPRR,
        CVFPMULTIPLIER20_A0,
        CVFPMULTIPLIER20_A1
,
        CVFPMULTIPLIER20_fail);
  CV_FP_FL4_DP_ADDER CVFPADDER20(
        clk,
        reset,
        CVFPADDER20_FPRR,
        CVFPADDER20_A0,
        CVFPADDER20_A1
,
        CVFPADDER20_fail);
 initial        begin 
      //ROM data table: 32 words of 32 bits.
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[0] = 32'h0;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[1] = 32'h400;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[2] = 32'hbf6;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[3] = 32'h1672;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[4] = 32'h23e9;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[5] = 32'h3424;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[6] = 32'h46f2;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[7] = 32'h5c28;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[8] = 32'h739e;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[9] = 32'h8d31;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[10] = 32'ha8c2;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[11] = 32'hc634;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[12] = 32'he56d;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[13] = 32'h1_0656;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[14] = 32'h1_28d8;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[15] = 32'h1_4cdf;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[16] = 32'h1_468f;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[17] = 32'h1_16d2;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[18] = 32'hec0c;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[19] = 32'hc5d7;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[20] = 32'ha3d9;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[21] = 32'h85c6;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[22] = 32'h6b56;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[23] = 32'h544d;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[24] = 32'h4073;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[25] = 32'h2f97;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[26] = 32'h218c;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[27] = 32'h162a;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[28] = 32'hd4b;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[29] = 32'h6ce;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[30] = 32'h295;
       A_UINT_CC_T1_T1_SCALbx10_T1_ARA0[31] = 32'h82;
       end 
      

// 1 vectors of width 8
// 95 vectors of width 1
// 34 vectors of width 32
// 27 vectors of width 64
// 32 array locations of width 32
// 32 bits in scalar variables
// Total state bits in module = 3975 bits.
// 884 continuously assigned (wire/non-state) bits 
//   cell CV_FP_CVT_FL2_F32_I64 count=3
//   cell CV_FP_FL15_SP_DIVIDER count=2
//   cell CV_FP_FL4_SP_ADDER count=4
//   cell CV_FP_FL5_SP_MULTIPLIER count=4
//   cell CV_FP_CVT_FL2_F64_I64 count=2
//   cell CV_FP_FL5_DP_DIVIDER count=1
//   cell CV_FP_FL4_DP_ADDER count=2
//   cell CV_FP_FL3_DP_MULTIPLIER count=2
// Total number of leaf cells = 20
endmodule

//  
// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.2b : 9th Sept 2017
//17/09/2017 22:15:39
//Cmd line args:  /rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -repack-to-roms=enable -vnl-roundtrip=disable -report-each-step -vnl-resets=synchronous -kiwife-directorate-endmode=finish -ip-incdir=/rack-ham/paula1/homedir/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 test58.exe -sim 1800 -compose=disable -vnl-rootmodname=DUT -vnl-resets=synchronous -vnl=test58.v -res2-loadstore-port-count=0 -bevelab-default-pause-mode=bblock -give-backtrace -report-each-step


//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation TtS1._SPILL for prefix T401/test58_sqrt/Sqrt_sp/1.12/_SPILL
//

//----------------------------------------------------------

//Report from Abbreviation:::
//    setting up abbreviation TtS1SPILL10 for prefix T401/test58_sqrt/Sqrt/1.12/_SPILL
//

//----------------------------------------------------------

//Report from KiwiC-fe.rpt:::
//KiwiC: front end input processing of class or method called test58_sqrt
//
//root_walk start thread at a static method (used as an entry point). Method name=test58_sqrt/.cctor uid=cctor14
//
//KiwiC start_thread (or entry point) id=cctor14
//
//Root method elaborated: specificf=S_kickoff_collate leftover=1+0
//
//KiwiC: front end input processing of class or method called test58
//
//root_compiler: start elaborating class 'test58'
//
//elaborating class 'test58'
//
//compiling static method as entry point: style=Root idl=test58/test58
//
//Performing root elaboration of method test58
//
//KiwiC start_thread (or entry point) id=Main10
//
//root_compiler class done: test58
//
//Report of all settings used from the recipe or command line:
//
//   kiwife-directorate-ready-flag=absent
//
//   kiwife-directorate-endmode=finish
//
//   kiwife-directorate-startmode=self-start
//
//   cil-uwind-budget=10000
//
//   kiwic-cil-dump=disable
//
//   kiwic-kcode-dump=disable
//
//   kiwic-register-colours=disable
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
//   srcfile=test58.exe
//
//   kiwic-autodispose=disable
//
//END OF KIWIC REPORT FILE
//

//----------------------------------------------------------

//Report from restructure2:::
//Offchip Load/Store (and other) Ports = Nothing to Report
//

//----------------------------------------------------------

//Report from restructure2:::
//Restructure Technology Settings
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| Key                       | Value   | Description                                                                     |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//| int-flr-mul               | -3000   |                                                                                 |
//| max-no-fp-addsubs         | 6       | Maximum number of adders and subtractors (or combos) to instantiate per thread. |
//| max-no-fp-muls            | 6       | Maximum number of f/p multipliers or dividers to instantiate per thread.        |
//| max-no-int-muls           | 3       | Maximum number of int multipliers to instantiate per thread.                    |
//| max-no-fp-divs            | 2       | Maximum number of f/p dividers to instantiate per thread.                       |
//| max-no-int-divs           | 2       | Maximum number of int dividers to instantiate per thread.                       |
//| max-no-rom-mirrors        | 8       | Maximum number of times to mirror a ROM per thread.                             |
//| max-ram-data_packing      | 8       | Maximum number of user words to pack into one RAM/loadstore word line.          |
//| fp-fl-dp-div              | 5       |                                                                                 |
//| fp-fl-dp-add              | 4       |                                                                                 |
//| fp-fl-dp-mul              | 3       |                                                                                 |
//| fp-fl-sp-div              | 15      |                                                                                 |
//| fp-fl-sp-add              | 4       |                                                                                 |
//| fp-fl-sp-mul              | 5       |                                                                                 |
//| res2-offchip-threshold    | 1000000 |                                                                                 |
//| res2-combrom-threshold    | 64      |                                                                                 |
//| res2-combram-threshold    | 32      |                                                                                 |
//| res2-regfile-threshold    | 8       |                                                                                 |
//| res2-loadstore-port-count | 0       |                                                                                 |
//*---------------------------+---------+---------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//PC codings points for bevelab10 
//*--------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause            | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*--------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:bevelab10"     | 825 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 1    |
//| XU32'1:"1:bevelab10"     | 824 | 1       | hwm=0.0.0    | 1    |        | -     | -   | 2    |
//| XU32'2:"2:bevelab10"     | 823 | 2       | hwm=0.50.0   | 52   |        | 3     | 52  | 53   |
//| XU32'4:"4:bevelab10"     | 821 | 53      | hwm=0.24.0   | 101  |        | 78    | 101 | 102  |
//| XU32'4:"4:bevelab10"     | 822 | 53      | hwm=0.24.0   | 77   |        | 54    | 77  | 1    |
//| XU32'8:"8:bevelab10"     | 819 | 102     | hwm=0.0.0    | 102  |        | -     | -   | 103  |
//| XU32'8:"8:bevelab10"     | 820 | 102     | hwm=0.0.0    | 102  |        | -     | -   | -    |
//| XU32'16:"16:bevelab10"   | 818 | 103     | hwm=0.0.0    | 103  |        | -     | -   | 104  |
//| XU32'32:"32:bevelab10"   | 816 | 104     | hwm=2.2.0    | 106  |        | -     | -   | 107  |
//| XU32'32:"32:bevelab10"   | 817 | 104     | hwm=2.2.0    | 106  |        | -     | -   | 120  |
//| XU32'64:"64:bevelab10"   | 814 | 107     | hwm=0.12.0   | 119  |        | 108   | 119 | 107  |
//| XU32'64:"64:bevelab10"   | 815 | 107     | hwm=0.0.0    | 107  |        | -     | -   | 120  |
//| XU32'128:"128:bevelab10" | 813 | 120     | hwm=0.12.0   | 132  |        | 121   | 132 | 102  |
//*--------------------------+-----+---------+--------------+------+--------+-------+-----+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10" 825 :  major_start_pcl=0   edge_private_start/end=-1/-1 exec=0 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'0:"0:bevelab10"
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno | Phaser  | Work                                                                                                                                 |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -   | R0 CTRL |                                                                                                                                      |
//| F0   | 825 | R0 DATA |                                                                                                                                      |
//| F0+E | 825 | W0 DATA | T401.test58.single_test.0.6.V_0 te=te:F0 write(S64'2) T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256 te=te:F0 write(0.0f) T401.test58.dou\ |
//|      |     |         | ble_test.0.9.V_0 te=te:F0 write(S64'0) T401.test58.double_test.0.9.V_1 te=te:F0 write(0.0) T401.test58_sqrt.Sqrt.1.12.V_3 te=te:F0 \ |
//|      |     |         | write(0.0) T401.test58_sqrt.Sqrt.1.12.V_4 te=te:F0 write(S32'0) T401.test58_sqrt.Sqrt.1.12._SPILL.256 te=te:F0 write(0.0)  PLI:Sing\ |
//|      |     |         | le Test  W/P:Starting Single  PLI:Test58 start - sqrt                                                                                |
//*------+-----+---------+--------------------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10" 824 :  major_start_pcl=1   edge_private_start/end=-1/-1 exec=1 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'1:"1:bevelab10"
//*------+-----+---------+------*
//| pc   | eno | Phaser  | Work |
//*------+-----+---------+------*
//| F1   | -   | R0 CTRL |      |
//| F1   | 824 | R0 DATA |      |
//| F1+E | 824 | W0 DATA |      |
//*------+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10" 823 :  major_start_pcl=2   edge_private_start/end=3/52 exec=52 (dend=50)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'2:"2:bevelab10"
//*-------+-----+----------+------------------------------------------------------------------------------------*
//| pc    | eno | Phaser   | Work                                                                               |
//*-------+-----+----------+------------------------------------------------------------------------------------*
//| F2    | -   | R0 CTRL  |                                                                                    |
//| F2    | 823 | R0 DATA  | fpcvt10 te=te:F2 cvt(E1)                                                           |
//| F3    | 823 | R1 DATA  |                                                                                    |
//| F4    | 823 | R2 DATA  | CVFPDIVIDER10 te=te:F4 *fixed-func-ALU*(E2, E3)                                    |
//| F5    | 823 | R3 DATA  |                                                                                    |
//| F6    | 823 | R4 DATA  |                                                                                    |
//| F7    | 823 | R5 DATA  |                                                                                    |
//| F8    | 823 | R6 DATA  |                                                                                    |
//| F9    | 823 | R7 DATA  |                                                                                    |
//| F10   | 823 | R8 DATA  |                                                                                    |
//| F11   | 823 | R9 DATA  |                                                                                    |
//| F12   | 823 | R10 DATA |                                                                                    |
//| F13   | 823 | R11 DATA |                                                                                    |
//| F14   | 823 | R12 DATA |                                                                                    |
//| F15   | 823 | R13 DATA |                                                                                    |
//| F16   | 823 | R14 DATA |                                                                                    |
//| F17   | 823 | R15 DATA |                                                                                    |
//| F18   | 823 | R16 DATA |                                                                                    |
//| F19   | 823 | R17 DATA | CVFPADDER10 te=te:F19 *fixed-func-ALU*(E3, E4)                                     |
//| F20   | 823 | R18 DATA |                                                                                    |
//| F21   | 823 | R19 DATA |                                                                                    |
//| F22   | 823 | R20 DATA |                                                                                    |
//| F23   | 823 | R21 DATA | CVFPMULTIPLIER10 te=te:F23 *fixed-func-ALU*(0.5f, E5)                              |
//| F24   | 823 | R22 DATA |                                                                                    |
//| F25   | 823 | R23 DATA |                                                                                    |
//| F26   | 823 | R24 DATA |                                                                                    |
//| F27   | 823 | R25 DATA |                                                                                    |
//| F28   | 823 | R26 DATA | CVFPDIVIDER12 te=te:F28 *fixed-func-ALU*(E2, E6)                                   |
//| F29   | 823 | R27 DATA |                                                                                    |
//| F30   | 823 | R28 DATA |                                                                                    |
//| F31   | 823 | R29 DATA |                                                                                    |
//| F32   | 823 | R30 DATA |                                                                                    |
//| F33   | 823 | R31 DATA |                                                                                    |
//| F34   | 823 | R32 DATA |                                                                                    |
//| F35   | 823 | R33 DATA |                                                                                    |
//| F36   | 823 | R34 DATA |                                                                                    |
//| F37   | 823 | R35 DATA |                                                                                    |
//| F38   | 823 | R36 DATA |                                                                                    |
//| F39   | 823 | R37 DATA |                                                                                    |
//| F40   | 823 | R38 DATA |                                                                                    |
//| F41   | 823 | R39 DATA |                                                                                    |
//| F42   | 823 | R40 DATA |                                                                                    |
//| F43   | 823 | R41 DATA | CVFPADDER12 te=te:F43 *fixed-func-ALU*(E6, E7)                                     |
//| F44   | 823 | R42 DATA |                                                                                    |
//| F45   | 823 | R43 DATA |                                                                                    |
//| F46   | 823 | R44 DATA |                                                                                    |
//| F47   | 823 | R45 DATA | CVFPMULTIPLIER12 te=te:F47 *fixed-func-ALU*(0.5f, E8)                              |
//| F48   | 823 | R46 DATA |                                                                                    |
//| F49   | 823 | R47 DATA |                                                                                    |
//| F50   | 823 | R48 DATA |                                                                                    |
//| F51   | 823 | R49 DATA |                                                                                    |
//| F52   | 823 | R50 DATA |                                                                                    |
//| F52+E | 823 | W0 DATA  | T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256 te=te:F52 write(E9)  PLI:    p=%d   pf=%f |
//*-------+-----+----------+------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10" 821 :  major_start_pcl=53   edge_private_start/end=78/101 exec=101 (dend=24)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10" 822 :  major_start_pcl=53   edge_private_start/end=54/77 exec=77 (dend=24)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'4:"4:bevelab10"
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------------------*
//| pc     | eno | Phaser   | Work                                                                                                                  |
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------------------*
//| F53    | -   | R0 CTRL  |                                                                                                                       |
//| F53    | 822 | R0 DATA  | CVFPMULTIPLIER14 te=te:F53 *fixed-func-ALU*(E10, E11) fpcvt12 te=te:F53 cvt(E1)                                       |
//| F54    | 822 | R1 DATA  |                                                                                                                       |
//| F55    | 822 | R2 DATA  |                                                                                                                       |
//| F56    | 822 | R3 DATA  |                                                                                                                       |
//| F57    | 822 | R4 DATA  |                                                                                                                       |
//| F58    | 822 | R5 DATA  | CVFPADDER14 te=te:F58 *fixed-func-ALU*(E2, E12)                                                                       |
//| F59    | 822 | R6 DATA  |                                                                                                                       |
//| F60    | 822 | R7 DATA  |                                                                                                                       |
//| F61    | 822 | R8 DATA  |                                                                                                                       |
//| F62    | 822 | R9 DATA  | CVFPDIVIDER12 te=te:F62 *fixed-func-ALU*(E13, E2)                                                                     |
//| F63    | 822 | R10 DATA |                                                                                                                       |
//| F64    | 822 | R11 DATA |                                                                                                                       |
//| F65    | 822 | R12 DATA |                                                                                                                       |
//| F66    | 822 | R13 DATA |                                                                                                                       |
//| F67    | 822 | R14 DATA |                                                                                                                       |
//| F68    | 822 | R15 DATA |                                                                                                                       |
//| F69    | 822 | R16 DATA |                                                                                                                       |
//| F70    | 822 | R17 DATA |                                                                                                                       |
//| F71    | 822 | R18 DATA |                                                                                                                       |
//| F72    | 822 | R19 DATA |                                                                                                                       |
//| F73    | 822 | R20 DATA |                                                                                                                       |
//| F74    | 822 | R21 DATA |                                                                                                                       |
//| F75    | 822 | R22 DATA |                                                                                                                       |
//| F76    | 822 | R23 DATA |                                                                                                                       |
//| F77    | 822 | R24 DATA |                                                                                                                       |
//| F77+E  | 822 | W0 DATA  | T401.test58.single_test.0.6.V_0 te=te:F77 write(E14)  PLI:         root sp=%f ...                                     |
//| F53    | 821 | R0 DATA  | CVFPMULTIPLIER16 te=te:F53 *fixed-func-ALU*(E10, E11) fpcvt14 te=te:F53 cvt(E1)                                       |
//| F78    | 821 | R1 DATA  |                                                                                                                       |
//| F79    | 821 | R2 DATA  |                                                                                                                       |
//| F80    | 821 | R3 DATA  |                                                                                                                       |
//| F81    | 821 | R4 DATA  |                                                                                                                       |
//| F82    | 821 | R5 DATA  | CVFPADDER16 te=te:F82 *fixed-func-ALU*(E2, E12)                                                                       |
//| F83    | 821 | R6 DATA  |                                                                                                                       |
//| F84    | 821 | R7 DATA  |                                                                                                                       |
//| F85    | 821 | R8 DATA  |                                                                                                                       |
//| F86    | 821 | R9 DATA  | CVFPDIVIDER10 te=te:F86 *fixed-func-ALU*(E13, E2)                                                                     |
//| F87    | 821 | R10 DATA |                                                                                                                       |
//| F88    | 821 | R11 DATA |                                                                                                                       |
//| F89    | 821 | R12 DATA |                                                                                                                       |
//| F90    | 821 | R13 DATA |                                                                                                                       |
//| F91    | 821 | R14 DATA |                                                                                                                       |
//| F92    | 821 | R15 DATA |                                                                                                                       |
//| F93    | 821 | R16 DATA |                                                                                                                       |
//| F94    | 821 | R17 DATA |                                                                                                                       |
//| F95    | 821 | R18 DATA |                                                                                                                       |
//| F96    | 821 | R19 DATA |                                                                                                                       |
//| F97    | 821 | R20 DATA |                                                                                                                       |
//| F98    | 821 | R21 DATA |                                                                                                                       |
//| F99    | 821 | R22 DATA |                                                                                                                       |
//| F100   | 821 | R23 DATA |                                                                                                                       |
//| F101   | 821 | R24 DATA |                                                                                                                       |
//| F101+E | 821 | W0 DATA  | T401.test58.single_test.0.6.V_0 te=te:F101 write(E14) T401.test58.double_test.0.9.V_0 te=te:F101 write(S64'2)  PLI:D\ |
//|        |     |          | ouble Test  W/P:Starting Double  PLI:         root sp=%f ...                                                          |
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'8:"8:bevelab10" 819 :  major_start_pcl=102   edge_private_start/end=-1/-1 exec=102 (dend=0)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'8:"8:bevelab10" 820 :  major_start_pcl=102   edge_private_start/end=-1/-1 exec=102 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'8:"8:bevelab10"
//*--------+-----+---------+-----------------------------------------------------------*
//| pc     | eno | Phaser  | Work                                                      |
//*--------+-----+---------+-----------------------------------------------------------*
//| F102   | -   | R0 CTRL |                                                           |
//| F102   | 820 | R0 DATA |                                                           |
//| F102+E | 820 | W0 DATA |  PLI:GSAI:hpr_sysexit  PLI:Test58 finished.  W/P:Finished |
//| F102   | 819 | R0 DATA |                                                           |
//| F102+E | 819 | W0 DATA |                                                           |
//*--------+-----+---------+-----------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'16:"16:bevelab10" 818 :  major_start_pcl=103   edge_private_start/end=-1/-1 exec=103 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'16:"16:bevelab10"
//*--------+-----+---------+------*
//| pc     | eno | Phaser  | Work |
//*--------+-----+---------+------*
//| F103   | -   | R0 CTRL |      |
//| F103   | 818 | R0 DATA |      |
//| F103+E | 818 | W0 DATA |      |
//*--------+-----+---------+------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'32:"32:bevelab10" 816 :  major_start_pcl=104   edge_private_start/end=-1/-1 exec=106 (dend=2)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'32:"32:bevelab10" 817 :  major_start_pcl=104   edge_private_start/end=-1/-1 exec=106 (dend=2)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'32:"32:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'32:"32:bevelab10"
//*--------+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//| pc     | eno | Phaser  | Work                                                                                                                    |
//*--------+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//| F104   | -   | R0 CTRL | fpcvt16 te=te:F104 cvt(E15)                                                                                             |
//| F105   | -   | R1 CTRL |                                                                                                                         |
//| F106   | -   | R2 CTRL |                                                                                                                         |
//| F104   | 817 | R0 DATA |                                                                                                                         |
//| F105   | 817 | R1 DATA |                                                                                                                         |
//| F106   | 817 | R2 DATA |                                                                                                                         |
//| F106+E | 817 | W0 DATA | T401.test58.double_test.0.9.V_1 te=te:F106 write(E16) T401.test58_sqrt.Sqrt.1.12._SPILL.256 te=te:F106 write(0)  PLI: \ |
//|        |     |         |    p=%d   pf=%f                                                                                                         |
//| F104   | 816 | R0 DATA |                                                                                                                         |
//| F105   | 816 | R1 DATA |                                                                                                                         |
//| F106   | 816 | R2 DATA |                                                                                                                         |
//| F106+E | 816 | W0 DATA | T401.test58.double_test.0.9.V_1 te=te:F106 write(E16) T401.test58_sqrt.Sqrt.1.12.V_3 te=te:F106 write(E17) T401.test58\ |
//|        |     |         | _sqrt.Sqrt.1.12.V_4 te=te:F106 write(0)  PLI:    p=%d   pf=%f                                                           |
//*--------+-----+---------+-------------------------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'64:"64:bevelab10" 814 :  major_start_pcl=107   edge_private_start/end=108/119 exec=119 (dend=12)
//,   Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'64:"64:bevelab10" 815 :  major_start_pcl=107   edge_private_start/end=-1/-1 exec=107 (dend=0)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'64:"64:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'64:"64:bevelab10"
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------*
//| pc     | eno | Phaser   | Work                                                                                                      |
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------*
//| F107   | -   | R0 CTRL  |                                                                                                           |
//| F107   | 815 | R0 DATA  |                                                                                                           |
//| F107+E | 815 | W0 DATA  | T401.test58_sqrt.Sqrt.1.12._SPILL.256 te=te:F107 write(E18)                                               |
//| F107   | 814 | R0 DATA  | CVFPDIVIDER14 te=te:F107 *fixed-func-ALU*(E19, E20)                                                       |
//| F108   | 814 | R1 DATA  |                                                                                                           |
//| F109   | 814 | R2 DATA  |                                                                                                           |
//| F110   | 814 | R3 DATA  |                                                                                                           |
//| F111   | 814 | R4 DATA  |                                                                                                           |
//| F112   | 814 | R5 DATA  | CVFPADDER18 te=te:F112 *fixed-func-ALU*(E20, E21)                                                         |
//| F113   | 814 | R6 DATA  |                                                                                                           |
//| F114   | 814 | R7 DATA  |                                                                                                           |
//| F115   | 814 | R8 DATA  |                                                                                                           |
//| F116   | 814 | R9 DATA  | CVFPMULTIPLIER18 te=te:F116 *fixed-func-ALU*(0.5, E22)                                                    |
//| F117   | 814 | R10 DATA |                                                                                                           |
//| F118   | 814 | R11 DATA |                                                                                                           |
//| F119   | 814 | R12 DATA |                                                                                                           |
//| F119+E | 814 | W0 DATA  | T401.test58_sqrt.Sqrt.1.12.V_3 te=te:F119 write(E23) T401.test58_sqrt.Sqrt.1.12.V_4 te=te:F119 write(E24) |
//*--------+-----+----------+-----------------------------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from restructure2:::
//  Absolute key numbers for scheduled edge res2: nopipeline: Thread=bevelab10 state=XU32'128:"128:bevelab10" 813 :  major_start_pcl=120   edge_private_start/end=121/132 exec=132 (dend=12)
//Simple greedy schedule for res2: nopipeline: Thread=bevelab10 state=XU32'128:"128:bevelab10"
//res2: nopipeline: Thread=bevelab10 state=XU32'128:"128:bevelab10"
//*--------+-----+----------+------------------------------------------------------------------------------------*
//| pc     | eno | Phaser   | Work                                                                               |
//*--------+-----+----------+------------------------------------------------------------------------------------*
//| F120   | -   | R0 CTRL  |                                                                                    |
//| F120   | 813 | R0 DATA  | CVFPMULTIPLIER20 te=te:F120 *fixed-func-ALU*(E25, E26) fpcvt18 te=te:F120 cvt(E15) |
//| F121   | 813 | R1 DATA  |                                                                                    |
//| F122   | 813 | R2 DATA  |                                                                                    |
//| F123   | 813 | R3 DATA  | CVFPADDER20 te=te:F123 *fixed-func-ALU*(E16, E27)                                  |
//| F124   | 813 | R4 DATA  |                                                                                    |
//| F125   | 813 | R5 DATA  |                                                                                    |
//| F126   | 813 | R6 DATA  |                                                                                    |
//| F127   | 813 | R7 DATA  | CVFPDIVIDER14 te=te:F127 *fixed-func-ALU*(E28, E16)                                |
//| F128   | 813 | R8 DATA  |                                                                                    |
//| F129   | 813 | R9 DATA  |                                                                                    |
//| F130   | 813 | R10 DATA |                                                                                    |
//| F131   | 813 | R11 DATA |                                                                                    |
//| F132   | 813 | R12 DATA |                                                                                    |
//| F132+E | 813 | W0 DATA  | T401.test58.double_test.0.9.V_0 te=te:F132 write(E29)  PLI:         root dp=%f ... |
//*--------+-----+----------+------------------------------------------------------------------------------------*
//

//----------------------------------------------------------

//Report from enumbers:::
//Concise expression alias report.
//
//  E1 =.= T401.test58.single_test.0.6.V_0
//
//  E2 =.= CVT(Cf)(T401.test58.single_test.0.6.V_0)
//
//  E3 =.= Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1))))
//
//  E4 =.= (CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))
//
//  E5 =.= (Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))
//
//  E6 =.= 0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1))))))
//
//  E7 =.= (CVT(Cf)(T401.test58.single_test.0.6.V_0))/(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))))
//
//  E8 =.= 0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1))))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))))
//
//  E9 =.= COND(0f<(CVT(Cf)(T401.test58.single_test.0.6.V_0)), Cf(0.5f*(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1))))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(0.5f*((Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1)))))+(CVT(Cf)(T401.test58.single_test.0.6.V_0))/(Cf(*APPLY:hpr_bitsToSingle(Cu(S32'532365312+((Cu(*APPLY:hpr_singleToBits(CVT(Cf)(T401.test58.single_test.0.6.V_0))))>>>1))))))))), 0f)
//
//  E10 =.= Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256)
//
//  E11 =.= -(Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//  E12 =.= (Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256))*-(Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//  E13 =.= (CVT(Cf)(T401.test58.single_test.0.6.V_0))+(Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256))*-(Cf(T401.test58_sqrt.Sqrt_sp.1.12._SPILL.256))
//
//  E14 =.= S64'204*T401.test58.single_test.0.6.V_0
//
//  E15 =.= T401.test58.double_test.0.9.V_0
//
//  E16 =.= CVT(C64f)(T401.test58.double_test.0.9.V_0)
//
//  E17 =.= C64f(*APPLY:hpr_bitsToDouble(S64'2303591209400008704+((C64u(*APPLY:hpr_doubleToBits(CVT(C64f)(T401.test58.double_test.0.9.V_0))))>>>1)+-((CVT(C64u)(@_UINT/CC/T1__T1__SCALbx10_T1__ARA0[CVT(C)(S64'31&S64'2303591209400008704+((C64u(*APPLY:hpr_doubleToBits(CVT(C64f)(T401.test58.double_test.0.9.V_0))))>>>1)>>>47)]))<<32)))
//
//  E18 =.= C64f(T401.test58_sqrt.Sqrt.1.12.V_3)
//
//  E19 =.= T401.test58.double_test.0.9.V_1
//
//  E20 =.= T401.test58_sqrt.Sqrt.1.12.V_3
//
//  E21 =.= T401.test58.double_test.0.9.V_1/T401.test58_sqrt.Sqrt.1.12.V_3
//
//  E22 =.= T401.test58_sqrt.Sqrt.1.12.V_3+T401.test58.double_test.0.9.V_1/T401.test58_sqrt.Sqrt.1.12.V_3
//
//  E23 =.= 0.5*(T401.test58_sqrt.Sqrt.1.12.V_3+T401.test58.double_test.0.9.V_1/T401.test58_sqrt.Sqrt.1.12.V_3)
//
//  E24 =.= 1+T401.test58_sqrt.Sqrt.1.12.V_4
//
//  E25 =.= C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256)
//
//  E26 =.= -(C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//  E27 =.= (C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256))*-(C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//  E28 =.= (CVT(C64f)(T401.test58.double_test.0.9.V_0))+(C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256))*-(C64f(T401.test58_sqrt.Sqrt.1.12._SPILL.256))
//
//  E29 =.= S64'204*T401.test58.double_test.0.9.V_0
//
//  E30 =.= S64'204*T401.test58.single_test.0.6.V_0<S64'1000000000000000
//
//  E31 =.= S64'204*T401.test58.single_test.0.6.V_0>=S64'1000000000000000
//
//  E32 =.= T401.test58.double_test.0.9.V_0>=S64'1000000000000000
//
//  E33 =.= T401.test58.double_test.0.9.V_0<S64'1000000000000000
//
//  E34 =.= {[XU32'106:"106:bevelab10nz"==bevelab10nz, 0>=fpcvt16_result]; [XU32'106:"106:bevelab10nz"!=bevelab10nz, 0>=fpcvt16RRh10hold]}
//
//  E35 =.= {[XU32'106:"106:bevelab10nz"==bevelab10nz, 0<fpcvt16_result]; [XU32'106:"106:bevelab10nz"!=bevelab10nz, 0<fpcvt16RRh10hold]}
//
//  E36 =.= T401.test58_sqrt.Sqrt.1.12.V_4>=3
//
//  E37 =.= T401.test58_sqrt.Sqrt.1.12.V_4<3
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for test58 to test58

//----------------------------------------------------------

//Report from verilog_render:::
//1 vectors of width 8
//
//95 vectors of width 1
//
//34 vectors of width 32
//
//27 vectors of width 64
//
//32 array locations of width 32
//
//32 bits in scalar variables
//
//Total state bits in module = 3975 bits.
//
//884 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread test58_sqrt/.cctor uid=cctor14 has 8 CIL instructions in 1 basic blocks
//Thread test58 uid=Main10 has 83 CIL instructions in 16 basic blocks
//Thread mpc10 has 9 bevelab control states (pauses)
//Reindexed thread bevelab10 with 133 minor control states
// eof (HPR L/S Verilog)
