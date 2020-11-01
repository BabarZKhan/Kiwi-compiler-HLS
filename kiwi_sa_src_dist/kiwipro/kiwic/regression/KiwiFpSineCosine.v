

// CBG Orangepath HPR L/S System

// Verilog output file generated at 18/03/2019 07:11:20
// Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019 Linux/X86_64:koo
//  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bevelab-default-pause-mode=soft -bevelab-soft-pause-threshold=220 -cfg-plot-each-step -max-no-fp-muls=2 -max-no-fp-addsubs=2 -bevelab-loglevel=8 -bevelab-recode-pc=disable -kiwife-directorate-pc-export=disable -bondout-loadstore-port-count=0 -kiwife-directorate-style=minimal -obj-dir-name=. -log-dir-name=d_obj_KiwiFpSineCosine /home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll
`timescale 1ns/1ns


module KiwiFpSineCosine(    
/* portgroup= abstractionName=L2590-vg pi_name=net2batchdirectoratenets10 */
    input clk,
    input reset,
    
/* portgroup= abstractionName=AUTOMETA_Sin_SQ1_F64 pi_name=Sin_SQ1_F64 */
input Sin_SQ1_F64_req,
    output reg Sin_SQ1_F64_ack,
    output reg /*fp*/ [63:0] Sin_SQ1_F64_return,
    input /*fp*/ [63:0] Sin_SQ1_F64_xd);

function [31:0] rtl_unsigned_bitextract0;
   input [63:0] arg;
   rtl_unsigned_bitextract0 = $unsigned(arg[31:0]);
   endfunction

// abstractionName=kiwic-decls pi_name=HPRL.SIN400
  reg/*fp*/  [63:0] Sin_SQ1_F64_xdinholder10;
// abstractionName=L2590-vg pi_name=net2batchdirectoratenets10
  reg hpr_int_run_enable_Sin_SQ1_F64;
  reg hpr_fsm_idle_Sin_SQ1_F64;
  reg hpr_fsm_ending_Sin_SQ1_F64;
// abstractionName=kiwicmainnets10
  reg/*fp*/  [63:0] HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6;
  reg [31:0] HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4;
  reg/*fp*/  [63:0] HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2;
  reg/*fp*/  [63:0] HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1;
  reg/*fp*/  [63:0] HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_10_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_10_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_10_YY;
  wire ifADDERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL4_ADDER_DP
  wire/*fp*/  [63:0] ifADDERALUF64_12_RR;
  reg/*fp*/  [63:0] ifADDERALUF64_12_XX;
  reg/*fp*/  [63:0] ifADDERALUF64_12_YY;
  wire ifADDERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_10_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_10_YY;
  wire ifMULTIPLIERALUF64_10_FAIL;
// abstractionName=res2-contacts pi_name=CV_FP_FL3_MULTIPLIER_DP
  wire/*fp*/  [63:0] ifMULTIPLIERALUF64_12_RR;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_XX;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF64_12_YY;
  wire ifMULTIPLIERALUF64_12_FAIL;
// abstractionName=res2-contacts pi_name=CVFPCVTFL2I32F64_10
  wire signed [31:0] CVFPCVTFL2I32F64_10_result;
  reg/*fp*/  [63:0] CVFPCVTFL2I32F64_10_arg;
  wire CVFPCVTFL2I32F64_10_FAIL;
// abstractionName=res2-morenets
  reg/*fp*/  [63:0] pipe70;
  reg/*fp*/  [63:0] pipe68;
  reg/*fp*/  [63:0] pipe66;
  reg/*fp*/  [63:0] pipe64;
  reg/*fp*/  [63:0] pipe62;
  reg/*fp*/  [63:0] pipe60;
  reg/*fp*/  [63:0] pipe58;
  reg/*fp*/  [63:0] pipe56;
  reg/*fp*/  [63:0] pipe54;
  reg/*fp*/  [63:0] pipe52;
  reg/*fp*/  [63:0] pipe50;
  reg/*fp*/  [63:0] pipe48;
  reg/*fp*/  [63:0] pipe46;
  reg/*fp*/  [63:0] pipe44;
  reg/*fp*/  [63:0] pipe42;
  reg/*fp*/  [63:0] pipe40;
  reg/*fp*/  [63:0] pipe38;
  reg/*fp*/  [63:0] pipe36;
  reg/*fp*/  [63:0] pipe34;
  reg/*fp*/  [63:0] pipe32;
  reg/*fp*/  [63:0] pipe30;
  reg/*fp*/  [63:0] pipe28;
  reg/*fp*/  [63:0] pipe26;
  reg/*fp*/  [63:0] pipe24;
  reg/*fp*/  [63:0] pipe22;
  reg/*fp*/  [63:0] pipe20;
  reg/*fp*/  [63:0] pipe18;
  reg/*fp*/  [63:0] pipe16;
  reg/*fp*/  [63:0] pipe14;
  reg/*fp*/  [63:0] pipe12;
  reg/*fp*/  [63:0] pipe10;
  reg/*fp*/  [63:0] ifADDERALUF6412RRh10hold;
  reg ifADDERALUF6412RRh10shot0;
  reg ifADDERALUF6412RRh10shot1;
  reg ifADDERALUF6412RRh10shot2;
  reg ifADDERALUF6412RRh10shot3;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6412RRh10hold;
  reg ifMULTIPLIERALUF6412RRh10shot0;
  reg ifMULTIPLIERALUF6412RRh10shot1;
  reg ifMULTIPLIERALUF6412RRh10shot2;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh16hold;
  reg ifADDERALUF6410RRh16shot0;
  reg ifADDERALUF6410RRh16shot1;
  reg ifADDERALUF6410RRh16shot2;
  reg ifADDERALUF6410RRh16shot3;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh16hold;
  reg ifMULTIPLIERALUF6410RRh16shot0;
  reg ifMULTIPLIERALUF6410RRh16shot1;
  reg ifMULTIPLIERALUF6410RRh16shot2;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh14hold;
  reg ifADDERALUF6410RRh14shot0;
  reg ifADDERALUF6410RRh14shot1;
  reg ifADDERALUF6410RRh14shot2;
  reg ifADDERALUF6410RRh14shot3;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh14hold;
  reg ifMULTIPLIERALUF6410RRh14shot0;
  reg ifMULTIPLIERALUF6410RRh14shot1;
  reg ifMULTIPLIERALUF6410RRh14shot2;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh12hold;
  reg ifADDERALUF6410RRh12shot0;
  reg ifADDERALUF6410RRh12shot1;
  reg ifADDERALUF6410RRh12shot2;
  reg ifADDERALUF6410RRh12shot3;
  reg/*fp*/  [63:0] ifADDERALUF6410RRh10hold;
  reg ifADDERALUF6410RRh10shot0;
  reg ifADDERALUF6410RRh10shot1;
  reg ifADDERALUF6410RRh10shot2;
  reg ifADDERALUF6410RRh10shot3;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh12hold;
  reg ifMULTIPLIERALUF6410RRh12shot0;
  reg ifMULTIPLIERALUF6410RRh12shot1;
  reg ifMULTIPLIERALUF6410RRh12shot2;
  reg/*fp*/  [63:0] ifMULTIPLIERALUF6410RRh10hold;
  reg ifMULTIPLIERALUF6410RRh10shot0;
  reg ifMULTIPLIERALUF6410RRh10shot1;
  reg ifMULTIPLIERALUF6410RRh10shot2;
  reg signed [31:0] CVFPCVTFL2I32F6410resulth10hold;
  reg CVFPCVTFL2I32F6410resulth10shot0;
  reg CVFPCVTFL2I32F6410resulth10shot1;
  reg [5:0] mpc12nz;
// abstractionName=share-nets pi_name=shareAnets10
  reg [63:0] hprpin500129x10;
 always   @(* )  begin 
       ifMULTIPLIERALUF64_10_XX = 32'sd0;
       ifMULTIPLIERALUF64_10_YY = 32'sd0;
       CVFPCVTFL2I32F64_10_arg = 32'sd0;
       ifADDERALUF64_10_XX = 32'sd0;
       ifADDERALUF64_10_YY = 32'sd0;
       ifMULTIPLIERALUF64_12_XX = 32'sd0;
       ifMULTIPLIERALUF64_12_YY = 32'sd0;
       ifADDERALUF64_12_XX = 32'sd0;
       ifADDERALUF64_12_YY = 32'sd0;
       hpr_fsm_ending_Sin_SQ1_F64 = 32'sd0;
       hpr_fsm_idle_Sin_SQ1_F64 = 32'sd1;
       hpr_int_run_enable_Sin_SQ1_F64 = 32'sd1;

      case (mpc12nz)
          32'h1/*1:mpc12nz*/:  begin 
              if ((($unsigned(32'sh7fff_ffff&HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4)<32'sh3e40_0000
              )? ((32'h3/*3:mpc12nz*/==mpc12nz)? !(!CVFPCVTFL2I32F64_10_result): !(!CVFPCVTFL2I32F6410resulth10hold)): 1'd1))  begin 
                       ifMULTIPLIERALUF64_10_XX = Sin_SQ1_F64_xdinholder10;
                       ifMULTIPLIERALUF64_10_YY = Sin_SQ1_F64_xdinholder10;
                       end 
                       CVFPCVTFL2I32F64_10_arg = Sin_SQ1_F64_xdinholder10;
               end 
              
          32'h4/*4:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = pipe70;
               ifMULTIPLIERALUF64_10_YY = ((32'h4/*4:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold);
               end 
              
          32'h8/*8:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = 64'h3de5_d93a_5acf_d57c;
               ifMULTIPLIERALUF64_10_YY = HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0;
               end 
              
          32'hb/*11:mpc12nz*/:  begin 
               ifADDERALUF64_10_XX = 64'hbe5a_e5e6_8a2b_9cf6;
               ifADDERALUF64_10_YY = ((32'hb/*11:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh12hold);
               end 
              
          32'hf/*15:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = pipe36;
               ifMULTIPLIERALUF64_10_YY = ((32'hf/*15:mpc12nz*/==mpc12nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh10hold);
               end 
              
          32'h12/*18:mpc12nz*/:  begin 
               ifADDERALUF64_10_XX = 64'h3ec7_1de3_57b1_fe85;
               ifADDERALUF64_10_YY = ((32'h12/*18:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold);
               end 
              
          32'h16/*22:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = pipe50;
               ifMULTIPLIERALUF64_10_YY = ((32'h16/*22:mpc12nz*/==mpc12nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh12hold);
               end 
              
          32'h19/*25:mpc12nz*/:  begin 
               ifADDERALUF64_10_XX = 64'hbf2a_01a0_19c1_61c3;
               ifADDERALUF64_10_YY = ((32'h19/*25:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh14hold);
               end 
              
          32'h1d/*29:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = pipe64;
               ifMULTIPLIERALUF64_10_YY = ((32'h1d/*29:mpc12nz*/==mpc12nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh14hold);
               end 
              
          32'h20/*32:mpc12nz*/:  begin 
               ifADDERALUF64_10_XX = 64'h3f81_1111_1110_f8a6;
               ifADDERALUF64_10_YY = ((32'h20/*32:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh16hold);
               end 
              
          32'h25/*37:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_12_XX = HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1;
               ifMULTIPLIERALUF64_12_YY = HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0;
               end 
              
          32'h28/*40:mpc12nz*/:  begin 
               ifADDERALUF64_12_XX = 64'hbfc5_5555_5555_553d;
               ifADDERALUF64_12_YY = ((32'h28/*40:mpc12nz*/==mpc12nz)? ifMULTIPLIERALUF64_12_RR: ifMULTIPLIERALUF6412RRh10hold);
               end 
              
          32'h2c/*44:mpc12nz*/:  begin 
               ifMULTIPLIERALUF64_10_XX = pipe22;
               ifMULTIPLIERALUF64_10_YY = ((32'h2c/*44:mpc12nz*/==mpc12nz)? ifADDERALUF64_12_RR: ifADDERALUF6412RRh10hold);
               end 
              
          32'h30/*48:mpc12nz*/:  begin 
               ifADDERALUF64_10_XX = Sin_SQ1_F64_xdinholder10;
               ifADDERALUF64_10_YY = HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6;
               end 
              endcase
       hpr_fsm_ending_Sin_SQ1_F64 = ($unsigned(32'sh7fff_ffff&HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4
      )<32'sh3e40_0000) && (32'h3/*3:mpc12nz*/==mpc12nz) && !CVFPCVTFL2I32F64_10_result || (32'h34/*52:mpc12nz*/==mpc12nz);

       hpr_fsm_idle_Sin_SQ1_F64 = (32'h0/*0:mpc12nz*/==mpc12nz);
       hpr_int_run_enable_Sin_SQ1_F64 = !hpr_fsm_idle_Sin_SQ1_F64 || Sin_SQ1_F64_req;
       end 
      

 always   @(posedge clk )  begin 
      //Start structure cvtToVerilogkiwi.HPRL.SIN400/1.0
      if (reset)  begin 
               Sin_SQ1_F64_xdinholder10 <= 64'd0;
               HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6 <= 64'd0;
               HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1 <= 64'd0;
               Sin_SQ1_F64_return <= 64'd0;
               HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2 <= 64'd0;
               HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0 <= 64'd0;
               HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4 <= 32'd0;
               mpc12nz <= 32'd0;
               end 
               else if (hpr_int_run_enable_Sin_SQ1_F64) 
              case (mpc12nz)
                  32'h1/*1:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2/*2:mpc12nz*/;
                      
                  32'h2/*2:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h3/*3:mpc12nz*/;
                      
                  32'h4/*4:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h5/*5:mpc12nz*/;
                      
                  32'h5/*5:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h6/*6:mpc12nz*/;
                      
                  32'h6/*6:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h7/*7:mpc12nz*/;
                      
                  32'h7/*7:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                          if ((($unsigned(32'sh7fff_ffff&HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4)<32'sh3e40_0000
                          )? ((32'h3/*3:mpc12nz*/==mpc12nz)? !(!CVFPCVTFL2I32F64_10_result): !(!CVFPCVTFL2I32F6410resulth10hold)): 1'd1
                          ))  begin 
                                   HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2 <= ((32'h7/*7:mpc12nz*/==
                                  mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh12hold);

                                   HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0 <= ((32'h4/*4:mpc12nz*/==
                                  mpc12nz)? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh10hold);

                                   HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4 <= $unsigned(32'sh7fff_ffff
                                  &HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4);

                                   end 
                                   mpc12nz <= 32'h8/*8:mpc12nz*/;
                           end 
                          
                  32'h3/*3:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                          if (((32'h3/*3:mpc12nz*/==mpc12nz)? !CVFPCVTFL2I32F64_10_result: !CVFPCVTFL2I32F6410resulth10hold) && ($unsigned(32'sh7fff_ffff
                          &HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4)<32'sh3e40_0000))  begin 
                                   mpc12nz <= 32'h0/*0:mpc12nz*/;
                                   Sin_SQ1_F64_return <= Sin_SQ1_F64_xdinholder10;
                                   HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4 <= $unsigned(32'sh7fff_ffff
                                  &HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4);

                                   end 
                                  if ((($unsigned(32'sh7fff_ffff&HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4
                          )<32'sh3e40_0000)? ((32'h3/*3:mpc12nz*/==mpc12nz)? !(!CVFPCVTFL2I32F64_10_result): !(!CVFPCVTFL2I32F6410resulth10hold
                          )): 1'd1))  mpc12nz <= 32'h4/*4:mpc12nz*/;
                               end 
                          
                  32'h8/*8:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h9/*9:mpc12nz*/;
                      
                  32'h9/*9:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'ha/*10:mpc12nz*/;
                      
                  32'ha/*10:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'hb/*11:mpc12nz*/;
                      
                  32'hb/*11:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'hc/*12:mpc12nz*/;
                      
                  32'hc/*12:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'hd/*13:mpc12nz*/;
                      
                  32'hd/*13:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'he/*14:mpc12nz*/;
                      
                  32'he/*14:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'hf/*15:mpc12nz*/;
                      
                  32'hf/*15:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h10/*16:mpc12nz*/;
                      
                  32'h10/*16:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h11/*17:mpc12nz*/;
                      
                  32'h11/*17:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h12/*18:mpc12nz*/;
                      
                  32'h12/*18:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h13/*19:mpc12nz*/;
                      
                  32'h13/*19:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h14/*20:mpc12nz*/;
                      
                  32'h14/*20:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h15/*21:mpc12nz*/;
                      
                  32'h15/*21:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h16/*22:mpc12nz*/;
                      
                  32'h16/*22:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h17/*23:mpc12nz*/;
                      
                  32'h17/*23:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h18/*24:mpc12nz*/;
                      
                  32'h18/*24:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h19/*25:mpc12nz*/;
                      
                  32'h19/*25:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1a/*26:mpc12nz*/;
                      
                  32'h1a/*26:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1b/*27:mpc12nz*/;
                      
                  32'h1b/*27:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1c/*28:mpc12nz*/;
                      
                  32'h1c/*28:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1d/*29:mpc12nz*/;
                      
                  32'h1d/*29:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1e/*30:mpc12nz*/;
                      
                  32'h1e/*30:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h1f/*31:mpc12nz*/;
                      
                  32'h1f/*31:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h20/*32:mpc12nz*/;
                      
                  32'h20/*32:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h21/*33:mpc12nz*/;
                      
                  32'h21/*33:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h22/*34:mpc12nz*/;
                      
                  32'h22/*34:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h23/*35:mpc12nz*/;
                      
                  32'h23/*35:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h24/*36:mpc12nz*/;
                      
                  32'h24/*36:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                           mpc12nz <= 32'h25/*37:mpc12nz*/;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1 <= ((32'h24/*36:mpc12nz*/==mpc12nz
                          )? ifADDERALUF64_10_RR: ifADDERALUF6410RRh16hold);

                           end 
                          
                  32'h25/*37:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h26/*38:mpc12nz*/;
                      
                  32'h26/*38:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h27/*39:mpc12nz*/;
                      
                  32'h27/*39:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h28/*40:mpc12nz*/;
                      
                  32'h28/*40:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h29/*41:mpc12nz*/;
                      
                  32'h29/*41:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2a/*42:mpc12nz*/;
                      
                  32'h2a/*42:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2b/*43:mpc12nz*/;
                      
                  32'h2b/*43:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2c/*44:mpc12nz*/;
                      
                  32'h2c/*44:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2d/*45:mpc12nz*/;
                      
                  32'h2d/*45:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2e/*46:mpc12nz*/;
                      
                  32'h2e/*46:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h2f/*47:mpc12nz*/;
                      
                  32'h2f/*47:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                          $display("Sine Double Quad=%1d, v=%f, r=%f", 32'sd0, $bitstoreal(HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2
                          ), $bitstoreal(HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1));
                          $display("  sub %f", $bitstoreal(((32'h2c/*44:mpc12nz*/==mpc12nz)? ifADDERALUF64_12_RR: ifADDERALUF6412RRh10hold
                          )));
                           mpc12nz <= 32'h30/*48:mpc12nz*/;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6 <= ((32'h2f/*47:mpc12nz*/==mpc12nz
                          )? ifMULTIPLIERALUF64_10_RR: ifMULTIPLIERALUF6410RRh16hold);

                           end 
                          
                  32'h30/*48:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h31/*49:mpc12nz*/;
                      
                  32'h31/*49:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h32/*50:mpc12nz*/;
                      
                  32'h32/*50:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h33/*51:mpc12nz*/;
                      
                  32'h33/*51:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  mpc12nz <= 32'h34/*52:mpc12nz*/;
                      
                  32'h0/*0:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                           mpc12nz <= 32'h1/*1:mpc12nz*/;
                           Sin_SQ1_F64_xdinholder10 <= Sin_SQ1_F64_xd;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6 <= 64'h0;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_1 <= 64'h0;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2 <= 64'h0;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0 <= 64'h0;
                           HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4 <= rtl_unsigned_bitextract0((hprpin500129x10
                          >>32'sd32));

                           end 
                          
                  32'h34/*52:mpc12nz*/: if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                          $display("  prod %f", $bitstoreal(HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_6));
                          $display("  ans %f", $bitstoreal(((32'h34/*52:mpc12nz*/==mpc12nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh16hold
                          )));
                           mpc12nz <= 32'h0/*0:mpc12nz*/;
                           Sin_SQ1_F64_return <= ((32'h34/*52:mpc12nz*/==mpc12nz)? ifADDERALUF64_10_RR: ifADDERALUF6410RRh16hold);
                           end 
                          endcase
              if (reset)  begin 
               pipe70 <= 64'd0;
               pipe68 <= 64'd0;
               pipe66 <= 64'd0;
               pipe64 <= 64'd0;
               pipe62 <= 64'd0;
               pipe60 <= 64'd0;
               pipe58 <= 64'd0;
               pipe56 <= 64'd0;
               pipe54 <= 64'd0;
               pipe52 <= 64'd0;
               pipe50 <= 64'd0;
               pipe48 <= 64'd0;
               pipe46 <= 64'd0;
               pipe44 <= 64'd0;
               pipe42 <= 64'd0;
               pipe40 <= 64'd0;
               pipe38 <= 64'd0;
               pipe36 <= 64'd0;
               pipe34 <= 64'd0;
               pipe32 <= 64'd0;
               pipe30 <= 64'd0;
               pipe28 <= 64'd0;
               pipe26 <= 64'd0;
               pipe24 <= 64'd0;
               pipe22 <= 64'd0;
               pipe20 <= 64'd0;
               pipe18 <= 64'd0;
               pipe16 <= 64'd0;
               pipe14 <= 64'd0;
               pipe12 <= 64'd0;
               pipe10 <= 64'd0;
               CVFPCVTFL2I32F6410resulth10hold <= 32'd0;
               CVFPCVTFL2I32F6410resulth10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot2 <= 32'd0;
               ifMULTIPLIERALUF6410RRh12hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh12shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh12shot2 <= 32'd0;
               ifADDERALUF6410RRh10hold <= 64'd0;
               ifADDERALUF6410RRh10shot1 <= 32'd0;
               ifADDERALUF6410RRh10shot2 <= 32'd0;
               ifADDERALUF6410RRh10shot3 <= 32'd0;
               ifADDERALUF6410RRh12hold <= 64'd0;
               ifADDERALUF6410RRh12shot1 <= 32'd0;
               ifADDERALUF6410RRh12shot2 <= 32'd0;
               ifADDERALUF6410RRh12shot3 <= 32'd0;
               ifMULTIPLIERALUF6410RRh14hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh14shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh14shot2 <= 32'd0;
               ifADDERALUF6410RRh14hold <= 64'd0;
               ifADDERALUF6410RRh14shot1 <= 32'd0;
               ifADDERALUF6410RRh14shot2 <= 32'd0;
               ifADDERALUF6410RRh14shot3 <= 32'd0;
               ifMULTIPLIERALUF6410RRh16hold <= 64'd0;
               ifMULTIPLIERALUF6410RRh16shot1 <= 32'd0;
               ifMULTIPLIERALUF6410RRh16shot2 <= 32'd0;
               ifADDERALUF6410RRh16hold <= 64'd0;
               ifADDERALUF6410RRh16shot1 <= 32'd0;
               ifADDERALUF6410RRh16shot2 <= 32'd0;
               ifADDERALUF6410RRh16shot3 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10hold <= 64'd0;
               ifMULTIPLIERALUF6412RRh10shot1 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot2 <= 32'd0;
               ifADDERALUF6412RRh10hold <= 64'd0;
               ifADDERALUF6412RRh10shot1 <= 32'd0;
               ifADDERALUF6412RRh10shot2 <= 32'd0;
               ifADDERALUF6412RRh10shot3 <= 32'd0;
               ifADDERALUF6410RRh16shot0 <= 32'd0;
               ifMULTIPLIERALUF6410RRh16shot0 <= 32'd0;
               ifADDERALUF6412RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF6412RRh10shot0 <= 32'd0;
               ifADDERALUF6410RRh14shot0 <= 32'd0;
               ifMULTIPLIERALUF6410RRh14shot0 <= 32'd0;
               ifADDERALUF6410RRh12shot0 <= 32'd0;
               ifMULTIPLIERALUF6410RRh10shot0 <= 32'd0;
               ifADDERALUF6410RRh10shot0 <= 32'd0;
               ifMULTIPLIERALUF6410RRh12shot0 <= 32'd0;
               CVFPCVTFL2I32F6410resulth10shot0 <= 32'd0;
               end 
               else if (hpr_int_run_enable_Sin_SQ1_F64)  begin 
                  if (ifADDERALUF6412RRh10shot3)  ifADDERALUF6412RRh10hold <= ifADDERALUF64_12_RR;
                      if (ifMULTIPLIERALUF6412RRh10shot2)  ifMULTIPLIERALUF6412RRh10hold <= ifMULTIPLIERALUF64_12_RR;
                      if (ifADDERALUF6410RRh16shot3)  ifADDERALUF6410RRh16hold <= ifADDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh16shot2)  ifMULTIPLIERALUF6410RRh16hold <= ifMULTIPLIERALUF64_10_RR;
                      if (ifADDERALUF6410RRh14shot3)  ifADDERALUF6410RRh14hold <= ifADDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh14shot2)  ifMULTIPLIERALUF6410RRh14hold <= ifMULTIPLIERALUF64_10_RR;
                      if (ifADDERALUF6410RRh12shot3)  ifADDERALUF6410RRh12hold <= ifADDERALUF64_10_RR;
                      if (ifADDERALUF6410RRh10shot3)  ifADDERALUF6410RRh10hold <= ifADDERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh12shot2)  ifMULTIPLIERALUF6410RRh12hold <= ifMULTIPLIERALUF64_10_RR;
                      if (ifMULTIPLIERALUF6410RRh10shot2)  ifMULTIPLIERALUF6410RRh10hold <= ifMULTIPLIERALUF64_10_RR;
                      if (CVFPCVTFL2I32F6410resulth10shot1)  CVFPCVTFL2I32F6410resulth10hold <= CVFPCVTFL2I32F64_10_result;
                       pipe70 <= pipe68;
                   pipe68 <= pipe66;
                   pipe66 <= Sin_SQ1_F64_xdinholder10;
                   pipe64 <= pipe62;
                   pipe62 <= pipe60;
                   pipe60 <= pipe58;
                   pipe58 <= pipe56;
                   pipe56 <= pipe54;
                   pipe54 <= pipe52;
                   pipe52 <= pipe50;
                   pipe50 <= pipe48;
                   pipe48 <= pipe46;
                   pipe46 <= pipe44;
                   pipe44 <= pipe42;
                   pipe42 <= pipe40;
                   pipe40 <= pipe38;
                   pipe38 <= pipe36;
                   pipe36 <= pipe34;
                   pipe34 <= pipe32;
                   pipe32 <= pipe30;
                   pipe30 <= pipe28;
                   pipe28 <= pipe26;
                   pipe26 <= pipe24;
                   pipe24 <= HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_0;
                   pipe22 <= pipe20;
                   pipe20 <= pipe18;
                   pipe18 <= pipe16;
                   pipe16 <= pipe14;
                   pipe14 <= pipe12;
                   pipe12 <= pipe10;
                   pipe10 <= HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_2;
                   CVFPCVTFL2I32F6410resulth10shot1 <= CVFPCVTFL2I32F6410resulth10shot0;
                   ifMULTIPLIERALUF6410RRh10shot1 <= ifMULTIPLIERALUF6410RRh10shot0;
                   ifMULTIPLIERALUF6410RRh10shot2 <= ifMULTIPLIERALUF6410RRh10shot1;
                   ifMULTIPLIERALUF6410RRh12shot1 <= ifMULTIPLIERALUF6410RRh12shot0;
                   ifMULTIPLIERALUF6410RRh12shot2 <= ifMULTIPLIERALUF6410RRh12shot1;
                   ifADDERALUF6410RRh10shot1 <= ifADDERALUF6410RRh10shot0;
                   ifADDERALUF6410RRh10shot2 <= ifADDERALUF6410RRh10shot1;
                   ifADDERALUF6410RRh10shot3 <= ifADDERALUF6410RRh10shot2;
                   ifADDERALUF6410RRh12shot1 <= ifADDERALUF6410RRh12shot0;
                   ifADDERALUF6410RRh12shot2 <= ifADDERALUF6410RRh12shot1;
                   ifADDERALUF6410RRh12shot3 <= ifADDERALUF6410RRh12shot2;
                   ifMULTIPLIERALUF6410RRh14shot1 <= ifMULTIPLIERALUF6410RRh14shot0;
                   ifMULTIPLIERALUF6410RRh14shot2 <= ifMULTIPLIERALUF6410RRh14shot1;
                   ifADDERALUF6410RRh14shot1 <= ifADDERALUF6410RRh14shot0;
                   ifADDERALUF6410RRh14shot2 <= ifADDERALUF6410RRh14shot1;
                   ifADDERALUF6410RRh14shot3 <= ifADDERALUF6410RRh14shot2;
                   ifMULTIPLIERALUF6410RRh16shot1 <= ifMULTIPLIERALUF6410RRh16shot0;
                   ifMULTIPLIERALUF6410RRh16shot2 <= ifMULTIPLIERALUF6410RRh16shot1;
                   ifADDERALUF6410RRh16shot1 <= ifADDERALUF6410RRh16shot0;
                   ifADDERALUF6410RRh16shot2 <= ifADDERALUF6410RRh16shot1;
                   ifADDERALUF6410RRh16shot3 <= ifADDERALUF6410RRh16shot2;
                   ifMULTIPLIERALUF6412RRh10shot1 <= ifMULTIPLIERALUF6412RRh10shot0;
                   ifMULTIPLIERALUF6412RRh10shot2 <= ifMULTIPLIERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot1 <= ifADDERALUF6412RRh10shot0;
                   ifADDERALUF6412RRh10shot2 <= ifADDERALUF6412RRh10shot1;
                   ifADDERALUF6412RRh10shot3 <= ifADDERALUF6412RRh10shot2;
                   ifADDERALUF6410RRh16shot0 <= (32'h20/*32:mpc12nz*/==mpc12nz) || (32'h30/*48:mpc12nz*/==mpc12nz);
                   ifMULTIPLIERALUF6410RRh16shot0 <= (32'h1d/*29:mpc12nz*/==mpc12nz) || (32'h2c/*44:mpc12nz*/==mpc12nz);
                   ifADDERALUF6412RRh10shot0 <= (32'h28/*40:mpc12nz*/==mpc12nz);
                   ifMULTIPLIERALUF6412RRh10shot0 <= (32'h25/*37:mpc12nz*/==mpc12nz);
                   ifADDERALUF6410RRh14shot0 <= (32'h19/*25:mpc12nz*/==mpc12nz);
                   ifMULTIPLIERALUF6410RRh14shot0 <= (32'h16/*22:mpc12nz*/==mpc12nz);
                   ifADDERALUF6410RRh12shot0 <= (32'h12/*18:mpc12nz*/==mpc12nz);
                   ifMULTIPLIERALUF6410RRh10shot0 <= (($unsigned(32'sh7fff_ffff&HPRL_SIN400_HprlsMathsPrimsCrude_KiwiFpSineCosine_KiwiSineDouble_0_5_V_4
                  )<32'sh3e40_0000)? !(!CVFPCVTFL2I32F6410resulth10hold) && (32'h1/*1:mpc12nz*/==mpc12nz): (32'h1/*1:mpc12nz*/==mpc12nz
                  )) || (32'hf/*15:mpc12nz*/==mpc12nz);

                   ifADDERALUF6410RRh10shot0 <= (32'hb/*11:mpc12nz*/==mpc12nz);
                   ifMULTIPLIERALUF6410RRh12shot0 <= (32'h4/*4:mpc12nz*/==mpc12nz) || (32'h8/*8:mpc12nz*/==mpc12nz);
                   CVFPCVTFL2I32F6410resulth10shot0 <= (32'h1/*1:mpc12nz*/==mpc12nz);
                   end 
                  if (!reset)  begin if (hpr_int_run_enable_Sin_SQ1_F64)  begin if (hpr_int_run_enable_Sin_SQ1_F64)  Sin_SQ1_F64_ack <= hpr_fsm_ending_Sin_SQ1_F64
                  ;

                   end 
               end 
          //End structure cvtToVerilogkiwi.HPRL.SIN400/1.0


       end 
      

  CV_FP_FL4_ADDER_DP ifADDERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_10_RR),
        .XX(ifADDERALUF64_10_XX
),
        .YY(ifADDERALUF64_10_YY),
        .FAIL(ifADDERALUF64_10_FAIL));
  CV_FP_FL4_ADDER_DP ifADDERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifADDERALUF64_12_RR),
        .XX(ifADDERALUF64_12_XX
),
        .YY(ifADDERALUF64_12_YY),
        .FAIL(ifADDERALUF64_12_FAIL));
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_10(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_10_RR),
        .XX(ifMULTIPLIERALUF64_10_XX
),
        .YY(ifMULTIPLIERALUF64_10_YY),
        .FAIL(ifMULTIPLIERALUF64_10_FAIL));
  CV_FP_FL3_MULTIPLIER_DP ifMULTIPLIERALUF64_12(
        .clk(clk),
        .reset(reset),
        .RR(ifMULTIPLIERALUF64_12_RR),
        .XX(ifMULTIPLIERALUF64_12_XX
),
        .YY(ifMULTIPLIERALUF64_12_YY),
        .FAIL(ifMULTIPLIERALUF64_12_FAIL));
  CV_FP_CVT_FL2_I32_F64 CVFPCVTFL2I32F64_10(
        .clk(clk),
        .reset(reset),
        .result(CVFPCVTFL2I32F64_10_result),
        .arg(CVFPCVTFL2I32F64_10_arg
),
        .FAIL(CVFPCVTFL2I32F64_10_FAIL));
always @(*) hprpin500129x10 = Sin_SQ1_F64_xd;

// Structural Resource (FU) inventory for KiwiFpSineCosine:// 56 vectors of width 64
// 1 vectors of width 6
// 40 vectors of width 1
// 2 vectors of width 32
// Total state bits in module = 3694 bits.
// 293 continuously assigned (wire/non-state) bits 
//   cell CV_FP_FL4_ADDER_DP count=2
//   cell CV_FP_FL3_MULTIPLIER_DP count=2
//   cell CV_FP_CVT_FL2_I32_F64 count=1
// Total number of leaf cells = 5
endmodule

//  
// Layout wiring length estimation mode is LAYOUT_lcp.
//HPR L/S (orangepath) auxiliary reports.
//KiwiC compilation report
//Kiwi Scientific Acceleration (KiwiC .net/CIL/C# to Verilog/SystemC compiler): Version Alpha 0.3.9b : 10th March 2019
//18/03/2019 07:11:17
//Cmd line args:  /home/djg11/d320/hprls/kiwipro/kiwic/kdistro/lib/kiwic.exe -vnl-roundtrip=disable -vnl-resets=synchronous -kiwife-dynpoly=disable -kiwife-directorate-endmode=finish -ip-incdir=/home/djg11/d320/hprls/kiwipro/kiwic/src/tinytests:.:/tmp/ip_block_folder1 -res2-share-array-reads=enable -res2-regen-sequencer=enable -res2-extend-schedules-to-keep-pli-order=full -bevelab-revast-enable=disable -compose=disable -bevelab-default-pause-mode=soft -bevelab-soft-pause-threshold=220 -cfg-plot-each-step -max-no-fp-muls=2 -max-no-fp-addsubs=2 -bevelab-loglevel=8 -bevelab-recode-pc=disable -kiwife-directorate-pc-export=disable -bondout-loadstore-port-count=0 -kiwife-directorate-style=minimal -obj-dir-name=. -log-dir-name=d_obj_KiwiFpSineCosine /home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll


//----------------------------------------------------------

//Report from kiwife:::
//Entry Points To Be Compiled:
//*---------------------------------------+-----------+----------------------------------------------------------------------+---------------+-------------------------------------------+-------+-------*
//| Class                                 | Style     | Dir Style                                                            | Timing Target | Method                                    | UID   | Skip  |
//*---------------------------------------+-----------+----------------------------------------------------------------------+---------------+-------------------------------------------+-------+-------*
//| HprlsMathsPrimsCrude.KiwiFpSineCosine | MM_remote | DS_minimal self-start/directorate-startmode, finish/directorate-end\ |               | HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin | Sin10 | false |
//|                                       |           | mode, disable/directorate-pc-export                                  |               |                                           |       |       |
//*---------------------------------------+-----------+----------------------------------------------------------------------+---------------+-------------------------------------------+-------+-------*

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
//Write IP-XACT abstractionDefinition for Sin_SQ1_F64_rtl to AUTOMETA_Sin_SQ1_F64_rtl

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT busDefinition for Sin_SQ1_F64 to AUTOMETA_Sin_SQ1_F64

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
//KiwiC: front end input processing of class System.BitConverter  wonky=System igrf=false
//
//
//root_compiler: method compile: entry point. Method name=System.BitConverter..cctor uid=cctor10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=cctor10 full_idl=System.BitConverter..cctor
//
//
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 0/prev
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
//Root method elaborated/compiled: specificf=S_kickoff_collate leftover=1/new + 1/prev
//
//
//KiwiC: front end input processing of class HprlsMathsPrimsCrude.KiwiFpSineCosine  wonky=HprlsMathsPrimsCrude igrf=false
//
//
//root_compiler: method compile: entry point. Method name=HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin uid=Sin10 staticf=true staticated=[]
//
//
//KiwiC start_toplevel_method: thread (or entry point) uid=Sin10 full_idl=HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin: A remote-callable method
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
//   kiwife-directorate-pc-export=disable
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
//   kiwife-directorate-style=minimal
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
//   srcfile=/home/djg11/d320/hprls/kiwipro/kiwi/userlib/kickoff-libraries/KiwiFpSineCosine/KiwiFpSineCosine.dll
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
//| max-no-fp-addsubs      | 2       | Maximum number of adders and subtractors (or combos) to instantiate per thread.                            |
//| max-no-fp-muls         | 2       | Maximum number of f/p multipliers or dividers to instantiate per thread.                                   |
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
//PC codings points for mpc12 
//*--------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| gb-flag/Pause      | eno | Root Pc | hwm          | Exec | Reverb | Start | End | Next |
//*--------------------+-----+---------+--------------+------+--------+-------+-----+------*
//| XU32'0:"0:mpc12"   | 827 | 0       | hwm=0.0.0    | 0    |        | -     | -   | 1    |
//| XU32'16:"16:mpc12" | 825 | 1       | hwm=2.2.0    | 3    |        | -     | -   | 0    |
//| XU32'16:"16:mpc12" | 826 | 1       | hwm=2.6.0    | 7    |        | 4     | 7   | 8    |
//| XU32'31:"31:mpc12" | 824 | 8       | hwm=0.28.0   | 36   |        | 9     | 36  | 37   |
//| XU32'32:"32:mpc12" | 823 | 37      | hwm=0.10.0   | 47   |        | 38    | 47  | 48   |
//| XU32'39:"39:mpc12" | 822 | 48      | hwm=0.4.0    | 52   |        | 49    | 52  | 0    |
//*--------------------+-----+---------+--------------+------+--------+-------+-----+------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc12 state=XU32'0:"0:mpc12"
//res2: scon1: nopipeline: Thread=mpc12 state=XU32'0:"0:mpc12"
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                          |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------*
//| F0   | -    | R0 CTRL |                                                                                                                                               |
//| F0   | E827 | R0 DATA | Sin._SQ1_F64.xdargread(<NONE>)                                                                                                                |
//| F0+E | E827 | W0 DATA | HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4write(E1) HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSine\ |
//|      |      |         | Double.0.5.V_0write(??64'0) HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_2write(??64'0) HPRL.SIN400.HprlsMathsPrim\ |
//|      |      |         | sCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1write(??64'0) HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_6write(??\ |
//|      |      |         | 64'0) Sin._SQ1_F64.xdinholder10write(C64f(Sin._SQ1_F64.xd))                                                                                   |
//*------+------+---------+-----------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc12 state=XU32'16:"16:mpc12"
//res2: scon1: nopipeline: Thread=mpc12 state=XU32'16:"16:mpc12"
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| pc   | eno  | Phaser  | Work                                                                                                                                                                |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*
//| F1   | -    | R0 CTRL | CVFPCVTFL2I32F64_10_hpr_dbl2int32(E2)                                                                                                                               |
//| F2   | -    | R1 CTRL |                                                                                                                                                                     |
//| F3   | -    | R2 CTRL |                                                                                                                                                                     |
//| F1   | E826 | R0 DATA | ifMULTIPLIERALUF64_10_compute(E2, E2)                                                                                                                               |
//| F2   | E826 | R1 DATA |                                                                                                                                                                     |
//| F3   | E826 | R2 DATA |                                                                                                                                                                     |
//| F4   | E826 | R3 DATA | ifMULTIPLIERALUF64_10_compute(E2, E3)                                                                                                                               |
//| F5   | E826 | R4 DATA |                                                                                                                                                                     |
//| F6   | E826 | R5 DATA |                                                                                                                                                                     |
//| F7   | E826 | R6 DATA |                                                                                                                                                                     |
//| F7+E | E826 | W0 DATA | HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4write(E4) HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0write(E3\ |
//|      |      |         | ) HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_2write(E5)                                                                                 |
//| F1   | E825 | R0 DATA |                                                                                                                                                                     |
//| F2   | E825 | R1 DATA |                                                                                                                                                                     |
//| F3   | E825 | R2 DATA |                                                                                                                                                                     |
//| F3+E | E825 | W0 DATA | HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4write(E4) Sin._SQ1_F64.returnwrite(E6)                                                      |
//*------+------+---------+---------------------------------------------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc12 state=XU32'31:"31:mpc12"
//res2: scon1: nopipeline: Thread=mpc12 state=XU32'31:"31:mpc12"
//*-------+------+----------+------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                               |
//*-------+------+----------+------------------------------------------------------------------------------------*
//| F8    | -    | R0 CTRL  |                                                                                    |
//| F8    | E824 | R0 DATA  | ifMULTIPLIERALUF64_10_compute(1.58969099521155E-10, E7)                            |
//| F9    | E824 | R1 DATA  |                                                                                    |
//| F10   | E824 | R2 DATA  |                                                                                    |
//| F11   | E824 | R3 DATA  | ifADDERALUF64_10_compute(-2.50507602534069E-08, E8)                                |
//| F12   | E824 | R4 DATA  |                                                                                    |
//| F13   | E824 | R5 DATA  |                                                                                    |
//| F14   | E824 | R6 DATA  |                                                                                    |
//| F15   | E824 | R7 DATA  | ifMULTIPLIERALUF64_10_compute(E7, E9)                                              |
//| F16   | E824 | R8 DATA  |                                                                                    |
//| F17   | E824 | R9 DATA  |                                                                                    |
//| F18   | E824 | R10 DATA | ifADDERALUF64_10_compute(2.75573137070701E-06, E10)                                |
//| F19   | E824 | R11 DATA |                                                                                    |
//| F20   | E824 | R12 DATA |                                                                                    |
//| F21   | E824 | R13 DATA |                                                                                    |
//| F22   | E824 | R14 DATA | ifMULTIPLIERALUF64_10_compute(E7, E11)                                             |
//| F23   | E824 | R15 DATA |                                                                                    |
//| F24   | E824 | R16 DATA |                                                                                    |
//| F25   | E824 | R17 DATA | ifADDERALUF64_10_compute(-0.000198412698298579, E12)                               |
//| F26   | E824 | R18 DATA |                                                                                    |
//| F27   | E824 | R19 DATA |                                                                                    |
//| F28   | E824 | R20 DATA |                                                                                    |
//| F29   | E824 | R21 DATA | ifMULTIPLIERALUF64_10_compute(E7, E13)                                             |
//| F30   | E824 | R22 DATA |                                                                                    |
//| F31   | E824 | R23 DATA |                                                                                    |
//| F32   | E824 | R24 DATA | ifADDERALUF64_10_compute(0.00833333333332249, E14)                                 |
//| F33   | E824 | R25 DATA |                                                                                    |
//| F34   | E824 | R26 DATA |                                                                                    |
//| F35   | E824 | R27 DATA |                                                                                    |
//| F36   | E824 | R28 DATA |                                                                                    |
//| F36+E | E824 | W0 DATA  | HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1write(E15) |
//*-------+------+----------+------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc12 state=XU32'32:"32:mpc12"
//res2: scon1: nopipeline: Thread=mpc12 state=XU32'32:"32:mpc12"
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------*
//| pc    | eno  | Phaser   | Work                                                                                                                          |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------*
//| F37   | -    | R0 CTRL  |                                                                                                                               |
//| F37   | E823 | R0 DATA  | ifMULTIPLIERALUF64_12_compute(E16, E7)                                                                                        |
//| F38   | E823 | R1 DATA  |                                                                                                                               |
//| F39   | E823 | R2 DATA  |                                                                                                                               |
//| F40   | E823 | R3 DATA  | ifADDERALUF64_12_compute(-0.166666666666666, E17)                                                                             |
//| F41   | E823 | R4 DATA  |                                                                                                                               |
//| F42   | E823 | R5 DATA  |                                                                                                                               |
//| F43   | E823 | R6 DATA  |                                                                                                                               |
//| F44   | E823 | R7 DATA  | ifMULTIPLIERALUF64_10_compute(E18, E19)                                                                                       |
//| F45   | E823 | R8 DATA  |                                                                                                                               |
//| F46   | E823 | R9 DATA  |                                                                                                                               |
//| F47   | E823 | R10 DATA |                                                                                                                               |
//| F47+E | E823 | W0 DATA  | HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_6write(E20)  PLI:  sub %f  PLI:Sine Double Quad=%d,... |
//*-------+------+----------+-------------------------------------------------------------------------------------------------------------------------------*

//----------------------------------------------------------

//Report from restructure2:::
//Schedule for res2: scon1: nopipeline: Thread=mpc12 state=XU32'39:"39:mpc12"
//res2: scon1: nopipeline: Thread=mpc12 state=XU32'39:"39:mpc12"
//*-------+------+---------+------------------------------------------------------------*
//| pc    | eno  | Phaser  | Work                                                       |
//*-------+------+---------+------------------------------------------------------------*
//| F48   | -    | R0 CTRL |                                                            |
//| F48   | E822 | R0 DATA | ifADDERALUF64_10_compute(E2, E21)                          |
//| F49   | E822 | R1 DATA |                                                            |
//| F50   | E822 | R2 DATA |                                                            |
//| F51   | E822 | R3 DATA |                                                            |
//| F52   | E822 | R4 DATA |                                                            |
//| F52+E | E822 | W0 DATA | Sin._SQ1_F64.returnwrite(E22)  PLI:  ans %f  PLI:  prod %f |
//*-------+------+---------+------------------------------------------------------------*

//----------------------------------------------------------

//Report from res2 enumbers:::
//Concise expression alias report.
//
//
//  E1 =.= CVT(Cu)((C64u(*APPLY:hpr_doubleToBits(C64f(Sin._SQ1_F64.xd))))>>>32)
//
//
//  E2 =.= Sin._SQ1_F64.xdinholder10
//
//
//  E3 =.= C64f(Sin._SQ1_F64.xdinholder10*Sin._SQ1_F64.xdinholder10)
//
//
//  E4 =.= Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4)
//
//
//  E5 =.= C64f(Sin._SQ1_F64.xdinholder10*(C64f(Sin._SQ1_F64.xdinholder10*Sin._SQ1_F64.xdinholder10)))
//
//
//  E6 =.= C64f(Sin._SQ1_F64.xdinholder10)
//
//
//  E7 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0
//
//
//  E8 =.= 1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0
//
//
//  E9 =.= -2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0
//
//
//  E10 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0)
//
//
//  E11 =.= 2.75573137070701E-06+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0)
//
//
//  E12 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(2.75573137070701E-06+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0))
//
//
//  E13 =.= -0.000198412698298579+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(2.75573137070701E-06+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0))
//
//
//  E14 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-0.000198412698298579+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(2.75573137070701E-06+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0)))
//
//
//  E15 =.= C64f(0.00833333333332249+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-0.000198412698298579+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(2.75573137070701E-06+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0*(-2.50507602534069E-08+1.58969099521155E-10*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0))))
//
//
//  E16 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1
//
//
//  E17 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0
//
//
//  E18 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_2
//
//
//  E19 =.= C64f(-0.166666666666666+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0)
//
//
//  E20 =.= C64f(HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_2*(C64f(-0.166666666666666+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_1*HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_0)))
//
//
//  E21 =.= HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_6
//
//
//  E22 =.= C64f(Sin._SQ1_F64.xdinholder10+HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_6)
//
//
//  E23 =.= {[(Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4))>=S32'1044381696]; [(Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4))<S32'1044381696, XU32'3:"3:mpc12nz"!=mpc12nz, |CVFPCVTFL2I32F6410resulth10hold|]; [(Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4))<S32'1044381696, XU32'3:"3:mpc12nz"==mpc12nz, |CVFPCVTFL2I32F64_10_result|]}
//
//
//  E24 =.= {[(Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4))<S32'1044381696, XU32'3:"3:mpc12nz"!=mpc12nz, |CVFPCVTFL2I32F6410resulth10hold|]; [(Cu(S32'2147483647&HPRL.SIN400.HprlsMathsPrimsCrude.KiwiFpSineCosine.KiwiSineDouble.0.5.V_4))<S32'1044381696, XU32'3:"3:mpc12nz"==mpc12nz, |CVFPCVTFL2I32F64_10_result|]}
//
//

//----------------------------------------------------------

//Report from IP-XACT input/output:::
//Write IP-XACT component file for KiwiFpSineCosine to KiwiFpSineCosine

//----------------------------------------------------------

//Report from verilog_render:::
//Structural Resource (FU) inventory for KiwiFpSineCosine:
//56 vectors of width 64
//
//1 vectors of width 6
//
//40 vectors of width 1
//
//2 vectors of width 32
//
//Total state bits in module = 3694 bits.
//
//293 continuously assigned (wire/non-state) bits 
//
//Total number of leaf cells = 0
//

//Major Statistics Report:
//Thread System.BitConverter..cctor uid=cctor10 has 1 CIL instructions in 1 basic blocks
//Thread KiwiSystem.Kiwi..cctor uid=cctor12 has 2 CIL instructions in 1 basic blocks
//Thread HprlsMathsPrimsCrude.KiwiFpSineCosine.Sin uid=Sin10 has 36 CIL instructions in 5 basic blocks
//Thread mpc12 has 5 bevelab control states (pauses)
//Reindexed thread mpc12 with 53 minor control states
// eof (HPR L/S Verilog)
