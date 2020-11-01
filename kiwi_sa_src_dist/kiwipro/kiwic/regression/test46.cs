// Kiwi Scientific Acceleration


// Test46.cs - IndexOf and co
 

using System;
using System.Text;
using KiwiSystem;

class test460
{
   //test46.cs(16,26): error CS0677: `test460.shared': A volatile field cannot be of the type `ulong'
   //So use the Kiwi-provided attribute instead for now.

   //Ideally KiwiC works out which variables need to be volatile since all threads sharing a variable are compiled to FPGA at once.

   [Kiwi.Volatile()]
   static ulong shared_var;


   static void second_bug()
   {
     int p = 200;
     ulong constant = 0xaaaaaaaaaaaaaaaa;
     ulong src ;
     src = constant & (ulong)0x00ffffffffff;   //[correct] - 40'hf
     Kiwi.Pause();
     Console.WriteLine("Test p1 {0} {1:x}", p++, src);

     src = constant & (ulong)0x00ffffffff0000; //[correct] - 32'hf
     Kiwi.Pause();
     Console.WriteLine("Test p2 {0} {1:x}", p++, src);


     src = constant & (ulong)0x00ffffffff;     //[wrong] - 32'hf
     Kiwi.Pause();
     Console.WriteLine("Test p3 {0} {1:x}", p++, src);

     src = constant & (ulong)0x00ffffff00;     //[wrong] - 24'hf
     Kiwi.Pause();
     Console.WriteLine("Test p4 {0} {1:x}", p++, src);

     src = constant & (ulong)0x00ffff0000;     //[wrong] - 16'hf
     Kiwi.Pause();
     Console.WriteLine("Test p5 {0} {1:x}", p++, src);
   }

// This gives now:  Test p5 was ok already?
//Test test46 start.
//Test p1 200 aaaaaaaaaa
//Test p2 201 aaaaaaaa0000
//Test p3 202 aaaaaaaa
//Test p4 203 aaaaaa00
//Test p5 204 aaaa0000



  // oh dear - this was being initialised to  64'h_ffff_ffff_cafe_0000
  static ulong dstt_mac = ((ulong)0x00cafe0000);

  static bool pop = false;


  [Kiwi.HardwareEntryPoint()]
  public static void Main()
  {
    Console.WriteLine("Test test46 start.");

    second_bug();
    Console.WriteLine("dstt_mac is hex={0:X} decimal={0}", dstt_mac);
    dstt_mac += 1;
    Kiwi.Pause();
    Console.WriteLine("dstt_mac now is hex={0:X} decimal={0}", dstt_mac);
    shared_var = dstt_mac;

    int [] LUT = new int [] { 21,32,403, 404, 606};

    long before = 2399141888;
    ulong after = (ulong)before; // 2399141888 dec = 0x00_8f00_0000 hex
    Console.WriteLine("First    Checking Salvator's bug {0} hex={0:x}", before);
    Kiwi.Pause();
    Console.WriteLine("Second   Checking Salvator's bug {0} hex={0:x}", after);




    for (int jojo=1;jojo<=3;jojo+=1) 
      {
        LUT[jojo] += 1;
	pop = false;
	if ( (((ulong)dstt_mac)==(ulong)0x00cafe0000) )
	  {
	    pop = true;
	  }
        int tmp = 2;
//        bool exists = Array.IndexOf(LUT, 403) > -1 ? true : false;
//	if (jojo == 2 && jojo < 4) Console.WriteLine("exists {0} exists={1}", jojo, exists);
       }


    Console.WriteLine("Test test46 finished.");
    Kiwi.Pause();
  }  
}
// eof



