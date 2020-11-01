// Kiwi Scientific Acceleration
//
// Testbench for dfsin using verilator.
//

#include "VVERI_SIMSYS.h"

int main_time = 0;

double sc_time_stamp()
{
  return (double)main_time;
}



int main(int argc, char **argv) 
{
  Verilated::commandArgs(argc, argv);
  VVERI_SIMSYS*top = new VVERI_SIMSYS("top");   // SP_CELL (top, Vour);
  top->reset = 1;           // Set some inputs
      top->clk = 1;       
  while (!Verilated::gotFinish()) 
    {
      main_time += 5;
      top->clk = !top->clk;
      
      if (main_time > 50) 
	{
	top->reset = 0;   
	}
      top->eval();         
   }
  
  top->final();           
  delete top;
  exit(0);
}
 

// eof
   
