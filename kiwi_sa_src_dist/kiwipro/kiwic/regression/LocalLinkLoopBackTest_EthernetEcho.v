

// CBG Orangepath HPR L/S System

// Verilog output file generated at 27/10/2014 15:56:48
// KiwiC (.net/CIL/C# to Verilog/SystemC compiler): Version alpha 55b: 1st-Sept-2014 Unix 3.16.3.200
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -csharp-gen=disable -bypass-verilog-roundtrip=enable EthernetEcho.exe

module LocalLinkLoopBackTest_EthernetEcho(    input [7:0] rx_data,
    input rx_sof_n,
    input rx_eof_n,
    input rx_src_rdy_n,
    output reg [7:0] tx_data,
    output reg tx_sof_n,
    output reg tx_eof_n,
    output reg tx_src_rdy_n,
    input tx_dst_rdy_n,
    input clk,
    input reset);
  reg TLEec1_1_V_0;
  integer TLEec1_1_V_1;
  integer TLEec1_1_V_2;
  reg TLEec1_1_V_3;
  reg [7:0] D8US_AX_CC_SOL[1023:0];
  reg [5:0] xpc10;
  reg [7:0] D8US_AX_CC_SOL_RDD0;
  reg [31:0] D8US_AX_CC_SOL_registered_AD0;
  reg D8US_AX_CC_SOL_WEN0;
  reg D8US_AX_CC_SOL_REN0;
  reg [7:0] D8US_AX_CC_SOL_WRD0;
  reg [31:0] D8US_AX_CC_SOL_AD0;
  reg [7:0] D8US_AX_CC_SOLRRhold10j;
  reg [4:0] xpc10nz;
 always   @(* )  begin 
      //Start HPR EthernetEcho.exe
       D8US_AX_CC_SOL_REN0 = 0;
       D8US_AX_CC_SOL_WRD0 = 0;
       D8US_AX_CC_SOL_WEN0 = 0;
       D8US_AX_CC_SOL_AD0 = 0;
      if (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n) 
          case (xpc10nz)

          0/*0:US*/:  begin 
               D8US_AX_CC_SOL_AD0 = 0;
               D8US_AX_CC_SOL_AD0 = 0;
               end 
              
          1/*1:US*/:  begin 
               D8US_AX_CC_SOL_WEN0 = !rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               D8US_AX_CC_SOL_WEN0 = !rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               end 
              endcase
          
      case (xpc10nz)

      2/*2:US*/:  begin 
           D8US_AX_CC_SOL_AD0 = TLEec1_1_V_2;
           D8US_AX_CC_SOL_AD0 = TLEec1_1_V_2;
           end 
          
      3/*3:US*/:  begin 
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_REN0 = 1;
           end 
          endcase
      if (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n) 
          case (xpc10nz)

          4/*4:US*/:  begin 
               D8US_AX_CC_SOL_AD0 = 0;
               D8US_AX_CC_SOL_AD0 = 0;
               end 
              
          5/*5:US*/:  begin 
               D8US_AX_CC_SOL_WEN0 = !rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               D8US_AX_CC_SOL_WEN0 = !rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               end 
              endcase
          if (TLEec1_1_V_0 && !rx_src_rdy_n) 
          case (xpc10nz)

          6/*6:US*/:  begin 
               D8US_AX_CC_SOL_AD0 = 0;
               D8US_AX_CC_SOL_AD0 = 0;
               end 
              
          7/*7:US*/:  begin 
               D8US_AX_CC_SOL_WEN0 = TLEec1_1_V_0 && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               D8US_AX_CC_SOL_WEN0 = TLEec1_1_V_0 && !rx_src_rdy_n;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               end 
              endcase
          
      case (xpc10nz)

      8/*8:US*/:  begin 
           D8US_AX_CC_SOL_AD0 = 12;
           D8US_AX_CC_SOL_AD0 = 12;
           end 
          
      9/*9:US*/:  begin 
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_AD0 = 1+TLEec1_1_V_2;
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_AD0 = 1+TLEec1_1_V_2;
           end 
          
      10/*10:US*/:  begin 
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_REN0 = 1;
           end 
          
      11/*11:US*/:  begin 
           D8US_AX_CC_SOL_AD0 = 0;
           D8US_AX_CC_SOL_AD0 = 0;
           end 
          
      12/*12:US*/:  begin 
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_AD0 = 1+TLEec1_1_V_2;
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_AD0 = 1+TLEec1_1_V_2;
           end 
          
      13/*13:US*/:  begin 
           D8US_AX_CC_SOL_REN0 = 1;
           D8US_AX_CC_SOL_REN0 = 1;
           end 
          
      14/*14:US*/:  D8US_AX_CC_SOL_AD0 = 6;

      15/*15:US*/:  D8US_AX_CC_SOL_REN0 = 1;
      endcase
      if (!rx_src_rdy_n && !TLEec1_1_V_3) 
          case (xpc10nz)

          15/*15:US*/:  D8US_AX_CC_SOL_AD0 = TLEec1_1_V_1;

          16/*16:US*/:  begin 
               D8US_AX_CC_SOL_WEN0 = !rx_src_rdy_n && !TLEec1_1_V_3;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               end 
              endcase
          
      case (xpc10nz)

      14/*14:US*/:  D8US_AX_CC_SOL_AD0 = 6;

      15/*15:US*/:  D8US_AX_CC_SOL_REN0 = 1;
      endcase
      if (!rx_src_rdy_n && !TLEec1_1_V_3) 
          case (xpc10nz)

          15/*15:US*/:  D8US_AX_CC_SOL_AD0 = TLEec1_1_V_1;

          16/*16:US*/:  begin 
               D8US_AX_CC_SOL_WEN0 = !rx_src_rdy_n && !TLEec1_1_V_3;
               D8US_AX_CC_SOL_WRD0 = rx_data;
               end 
              endcase
          //End HPR EthernetEcho.exe


       end 
      

 always   @(posedge clk )  begin 
      //Start HPR HPR_SSRAM_1024_8
      if (D8US_AX_CC_SOL_WEN0)  D8US_AX_CC_SOL[D8US_AX_CC_SOL_registered_AD0] <= D8US_AX_CC_SOL_WRD0;
          //End HPR HPR_SSRAM_1024_8


      //Start HPR EthernetEcho.exe

      case (xpc10nz)

      0/*0:US*/:  begin 
          if (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/))  begin 
                   TLEec1_1_V_1 <= (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && rx_src_rdy_n? 0: 1);
                   TLEec1_1_V_3 <= ((rx_sof_n? 1'd1: rx_src_rdy_n && (rx_src_rdy_n==0/*0:US*/)) || (rx_src_rdy_n!=0/*0:US*/) || !rx_src_rdy_n
                  ) && (rx_eof_n==0/*0:US*/);

                   TLEec1_1_V_1 <= (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && rx_src_rdy_n? 0: 1);
                   TLEec1_1_V_3 <= ((rx_sof_n? 1'd1: rx_src_rdy_n && (rx_src_rdy_n==0/*0:US*/)) || (rx_src_rdy_n!=0/*0:US*/) || !rx_src_rdy_n
                  ) && (rx_eof_n==0/*0:US*/);

                   end 
                   tx_sof_n <= 1;
           tx_src_rdy_n <= 1;
           tx_eof_n <= 1;
           TLEec1_1_V_0 <= !rx_sof_n && (rx_src_rdy_n==0/*0:US*/);
           xpc10 <= ((rx_sof_n? 1'd1: (rx_src_rdy_n!=0/*0:US*/))? 16/*16:xpc10:16*/: 1/*1:xpc10:1*/);
           tx_sof_n <= 1;
           tx_src_rdy_n <= 1;
           tx_eof_n <= 1;
           TLEec1_1_V_0 <= !rx_sof_n && (rx_src_rdy_n==0/*0:US*/);
           xpc10 <= ((rx_sof_n? 1'd1: (rx_src_rdy_n!=0/*0:US*/))? 16/*16:xpc10:16*/: 1/*1:xpc10:1*/);
           xpc10nz <= 1/*1:xpc10nz*/;
           end 
          
      3/*3:US*/:  begin 
          if ((TLEec1_1_V_2<TLEec1_1_V_1))  begin 
                   tx_data <= D8US_AX_CC_SOL_RDD0;
                   TLEec1_1_V_2 <= 1+TLEec1_1_V_2;
                   tx_data <= D8US_AX_CC_SOL_RDD0;
                   TLEec1_1_V_2 <= 1+TLEec1_1_V_2;
                   xpc10nz <= 2/*2:xpc10nz*/;
                   end 
                   else  begin 
                   tx_src_rdy_n <= 1;
                   TLEec1_1_V_0 <= 0;
                   xpc10 <= 8/*8:xpc10:8*/;
                   tx_src_rdy_n <= 1;
                   TLEec1_1_V_0 <= 0;
                   xpc10 <= 8/*8:xpc10:8*/;
                   xpc10nz <= 6/*6:xpc10nz*/;
                   end 
                  if (((TLEec1_1_V_2<TLEec1_1_V_1)? (TLEec1_1_V_2==-1+TLEec1_1_V_1): 1'd1))  begin 
                   tx_eof_n <= ((TLEec1_1_V_2<TLEec1_1_V_1) && (TLEec1_1_V_2==-1+TLEec1_1_V_1)? 0: 1);
                   tx_eof_n <= ((TLEec1_1_V_2<TLEec1_1_V_1) && (TLEec1_1_V_2==-1+TLEec1_1_V_1)? 0: 1);
                   end 
                   end 
          
      10/*10:US*/:  begin 
          if ((TLEec1_1_V_2>=5) && (12>=TLEec1_1_V_1))  begin 
                   tx_src_rdy_n <= 1;
                   TLEec1_1_V_0 <= 0;
                   tx_src_rdy_n <= 1;
                   TLEec1_1_V_0 <= 0;
                   xpc10nz <= 6/*6:xpc10nz*/;
                   end 
                  if ((((12<TLEec1_1_V_1)? (TLEec1_1_V_1!=13/*13:US*/): 1'd1) || (TLEec1_1_V_1==13/*13:US*/)) && (TLEec1_1_V_2>=5))  begin 
                   xpc10 <= (((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1)) && (TLEec1_1_V_2>=5)? 32/*32:xpc10:32*/: 8/*8:xpc10:8*/);
                   xpc10 <= (((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1)) && (TLEec1_1_V_2>=5)? 32/*32:xpc10:32*/: 8/*8:xpc10:8*/);
                   end 
                  if (((TLEec1_1_V_1==13/*13:US*/) || (12>=TLEec1_1_V_1)) && (TLEec1_1_V_2>=5))  begin 
                   tx_eof_n <= ((TLEec1_1_V_2>=5) && (TLEec1_1_V_1==13/*13:US*/)? 0: 1);
                   tx_eof_n <= ((TLEec1_1_V_2>=5) && (TLEec1_1_V_1==13/*13:US*/)? 0: 1);
                   end 
                  if (((TLEec1_1_V_2<5)? 1'd1: ((12<TLEec1_1_V_1)? (TLEec1_1_V_1!=13/*13:US*/): 1'd1) || (TLEec1_1_V_1==13/*13:US*/))) 
                   begin 
                   TLEec1_1_V_2 <= ((TLEec1_1_V_2<5)? 1+TLEec1_1_V_2: (((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1)) && (TLEec1_1_V_2
                  >=5)? 13: 12));

                   TLEec1_1_V_2 <= ((TLEec1_1_V_2<5)? 1+TLEec1_1_V_2: (((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1)) && (TLEec1_1_V_2
                  >=5)? 13: 12));

                   end 
                  if (((TLEec1_1_V_2<5)? 1'd1: ((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1))))  begin 
                   tx_data <= ((TLEec1_1_V_2<5)? D8US_AX_CC_SOL_RDD0: D8US_AX_CC_SOLRRhold10j);
                   tx_data <= ((TLEec1_1_V_2<5)? D8US_AX_CC_SOL_RDD0: D8US_AX_CC_SOLRRhold10j);
                   end 
                  if (((TLEec1_1_V_1==13/*13:US*/)? 1'd1: (12<TLEec1_1_V_1)) && (TLEec1_1_V_2>=5))  xpc10nz <= 2/*2:xpc10nz*/;
              if ((12<TLEec1_1_V_1) && (TLEec1_1_V_1==13/*13:US*/) || (TLEec1_1_V_2<5))  xpc10nz <= 8/*8:xpc10nz*/;
               end 
          
      15/*15:US*/:  begin 
          if (TLEec1_1_V_3)  begin 
                   tx_data <= D8US_AX_CC_SOL_RDD0;
                   TLEec1_1_V_2 <= 6;
                   tx_sof_n <= 0;
                   tx_src_rdy_n <= 0;
                   xpc10 <= 2/*2:xpc10:2*/;
                   tx_data <= D8US_AX_CC_SOL_RDD0;
                   TLEec1_1_V_2 <= 6;
                   tx_sof_n <= 0;
                   tx_src_rdy_n <= 0;
                   xpc10 <= 2/*2:xpc10:2*/;
                   end 
                   else  begin 
                   TLEec1_1_V_3 <= ((rx_src_rdy_n? !TLEec1_1_V_3: 1'd1) || TLEec1_1_V_3) && (rx_eof_n==0/*0:US*/);
                   TLEec1_1_V_3 <= ((rx_src_rdy_n? !TLEec1_1_V_3: 1'd1) || TLEec1_1_V_3) && (rx_eof_n==0/*0:US*/);
                   end 
                  if (!rx_src_rdy_n && !TLEec1_1_V_3)  begin 
                   TLEec1_1_V_1 <= 1+TLEec1_1_V_1;
                   TLEec1_1_V_1 <= 1+TLEec1_1_V_1;
                   end 
                   xpc10nz <= 16/*16:xpc10nz*/;
           end 
          endcase
      if (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && (xpc10nz==4/*4:US*/))  begin 
               TLEec1_1_V_1 <= (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && rx_src_rdy_n? 0: 1);
               TLEec1_1_V_3 <= ((rx_sof_n? 1'd1: rx_src_rdy_n && (rx_src_rdy_n==0/*0:US*/)) || (rx_src_rdy_n!=0/*0:US*/) || !rx_src_rdy_n
              ) && (rx_eof_n==0/*0:US*/);

               xpc10 <= 1/*1:xpc10:1*/;
               TLEec1_1_V_1 <= (!rx_sof_n && (rx_src_rdy_n==0/*0:US*/) && rx_src_rdy_n? 0: 1);
               TLEec1_1_V_3 <= ((rx_sof_n? 1'd1: rx_src_rdy_n && (rx_src_rdy_n==0/*0:US*/)) || (rx_src_rdy_n!=0/*0:US*/) || !rx_src_rdy_n
              ) && (rx_eof_n==0/*0:US*/);

               xpc10 <= 1/*1:xpc10:1*/;
               end 
              
      case (xpc10nz)

      4/*4:US*/:  begin 
           TLEec1_1_V_0 <= !rx_sof_n && (rx_src_rdy_n==0/*0:US*/);
           TLEec1_1_V_0 <= !rx_sof_n && (rx_src_rdy_n==0/*0:US*/);
           xpc10nz <= 5/*5:xpc10nz*/;
           end 
          
      6/*6:US*/:  begin 
          if (TLEec1_1_V_0)  begin 
                   TLEec1_1_V_1 <= (TLEec1_1_V_0 && rx_src_rdy_n? 0: 1);
                   TLEec1_1_V_3 <= ((TLEec1_1_V_0? rx_src_rdy_n: 1'd1) || !rx_src_rdy_n) && (rx_eof_n==0/*0:US*/);
                   TLEec1_1_V_1 <= (TLEec1_1_V_0 && rx_src_rdy_n? 0: 1);
                   TLEec1_1_V_3 <= ((TLEec1_1_V_0? rx_src_rdy_n: 1'd1) || !rx_src_rdy_n) && (rx_eof_n==0/*0:US*/);
                   end 
                   xpc10 <= (TLEec1_1_V_0? 1/*1:xpc10:1*/: 16/*16:xpc10:16*/);
           xpc10 <= (TLEec1_1_V_0? 1/*1:xpc10:1*/: 16/*16:xpc10:16*/);
           xpc10nz <= 7/*7:xpc10nz*/;
           end 
          
      13/*13:US*/:  begin 
          if ((TLEec1_1_V_2<11))  begin 
                   tx_sof_n <= (0/*0:MS*/==(6/*6:MS*/==1+TLEec1_1_V_2));
                   tx_sof_n <= (0/*0:MS*/==(6/*6:MS*/==1+TLEec1_1_V_2));
                   xpc10nz <= 11/*11:xpc10nz*/;
                   end 
                   else  begin 
                   xpc10 <= 4/*4:xpc10:4*/;
                   xpc10 <= 4/*4:xpc10:4*/;
                   xpc10nz <= 8/*8:xpc10nz*/;
                   end 
                   tx_data <= ((TLEec1_1_V_2<11)? D8US_AX_CC_SOL_RDD0: D8US_AX_CC_SOLRRhold10j);
           TLEec1_1_V_2 <= ((TLEec1_1_V_2<11)? 1+TLEec1_1_V_2: 0);
           tx_data <= ((TLEec1_1_V_2<11)? D8US_AX_CC_SOL_RDD0: D8US_AX_CC_SOLRRhold10j);
           TLEec1_1_V_2 <= ((TLEec1_1_V_2<11)? 1+TLEec1_1_V_2: 0);
           end 
          endcase
      if ((xpc10==1/*1:US*/)) 
          case (xpc10nz)

          1/*1:US*/:  xpc10nz <= 14/*14:xpc10nz*/;

          5/*5:US*/:  xpc10nz <= 14/*14:xpc10nz*/;

          7/*7:US*/:  xpc10nz <= 14/*14:xpc10nz*/;

          16/*16:US*/:  xpc10nz <= 14/*14:xpc10nz*/;
          endcase
          if ((xpc10nz==7/*7:US*/))  begin 
              if ((xpc10==16/*16:US*/))  xpc10nz <= 4/*4:xpc10nz*/;
                  if ((xpc10==8/*8:US*/))  xpc10nz <= 6/*6:xpc10nz*/;
                   end 
              if ((xpc10==16/*16:US*/)) 
          case (xpc10nz)

          1/*1:US*/:  xpc10nz <= 4/*4:xpc10nz*/;

          5/*5:US*/:  xpc10nz <= 4/*4:xpc10nz*/;
          endcase
          
      case (xpc10nz)

      1/*1:US*/: if ((xpc10==0/*0:US*/))  xpc10nz <= 0/*0:xpc10nz*/;
          
      9/*9:US*/:  begin 
           D8US_AX_CC_SOLRRhold10j <= D8US_AX_CC_SOL_RDD0;
           xpc10nz <= 10/*10:xpc10nz*/;
           end 
          
      12/*12:US*/:  begin 
           D8US_AX_CC_SOLRRhold10j <= D8US_AX_CC_SOL_RDD0;
           xpc10nz <= 13/*13:xpc10nz*/;
           end 
          
      16/*16:US*/: if ((xpc10==2/*2:US*/))  xpc10nz <= 11/*11:xpc10nz*/;
          endcase
       D8US_AX_CC_SOL_registered_AD0 <= D8US_AX_CC_SOL_AD0;
      if ((xpc10nz==2/*2:US*/))  xpc10nz <= 3/*3:xpc10nz*/;
          if ((xpc10nz==8/*8:US*/))  xpc10nz <= 9/*9:xpc10nz*/;
          if ((xpc10nz==11/*11:US*/))  xpc10nz <= 12/*12:xpc10nz*/;
          if ((xpc10nz==14/*14:US*/))  xpc10nz <= 15/*15:xpc10nz*/;
          //End HPR EthernetEcho.exe


       end 
      

//Resource=SRAM i_D8US_AX/CC/SOL 1024x8 clk=posedge(clk) synchronous/pipeline=1 ports=1 <NONE>
always @(*)  #1 D8US_AX_CC_SOL_RDD0 = D8US_AX_CC_SOL[D8US_AX_CC_SOL_registered_AD0];

//Total area 0
// 4 vectors of width 1
// 1 vectors of width 5
// 1 vectors of width 6
// 3 vectors of width 8
// 2 vectors of width 32
// 64 bits in scalar variables
// Total state bits in module = 167 bits.
// 8192 continuously assigned (wire/non-state) bits 
// Total number of leaf cells = 0
endmodule

// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
