// Kiwi Scientific Accleration
// Simple Verilator Testbench C++ wrapper

#include <stdio.h>
#include "VVERI_BENCHER.h"


int main_time = 0;

double sc_time_stamp()
{
  return (double)main_time;
}

int main(int argc, char **argv) 
{
  Verilated::commandArgs(argc, argv);
  VVERI_BENCHER *top = new VVERI_BENCHER("top");
  top->reset = 1;           // Set some inputs
  top->clk = 1;       
  top->din = 1;
  int stopcount = 0;
  while (!Verilated::gotFinish()) 
    {
      main_time += 5;
      top->clk = !top->clk;
      
      if (main_time > 50) 
	{
	  top->reset = 0;   
	}
      top->eval();         
      if (!top->reset && top->finish && !stopcount) stopcount = 5;
      if (stopcount > 0) { if (--stopcount <= 0) break;}
    }
  
  
  top->final();           
  delete top;
  printf("veriltop.cpp: Finished at %i\n", main_time);
  exit(0);
}

// eof
