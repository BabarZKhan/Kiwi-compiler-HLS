

// CBG Orangepath HPR L/S System

// Verilog output file generated at 03/10/2014 16:41:55
// KiwiC (.net/CIL/C# to Verilog/SystemC compiler): Version alpha 55b: 1st-Sept-2014 Unix 3.16.3.200
//  /home/djg11/d320/hprls/kiwipro/kiwic/distro/lib/kiwic.exe -csharp-gen=disable -bypass-verilog-roundtrip=enable tinytot.exe

module tinytot(input clk, input reset);
  wire xpc10;
 always   @(posedge clk )  begin 
      //Start HPR tinytot.exe
      if ((xpc10==0/*0:US*/)) $finish(0);
          //End HPR tinytot.exe


       end 
      

//Total area 0
// Total state bits in module = 0 bits.
// Total number of leaf cells = 0
endmodule

// LCP delay estimations included: turn off with -vnl-lcp-delay-estimate=disable
// eof (HPR L/S Verilog)
