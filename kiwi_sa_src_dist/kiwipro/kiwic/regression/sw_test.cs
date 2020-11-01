//
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
// All rights reserved. (C) 2007-15, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// SmithWaterman Genome Matcher - A simple canned version for KiwiC Regression Testing.
// Updated October 2015.
//
// Basic, single-threaded Smith Waterman code: hardwired input but with CRC on output.
// This copy kept for regression testing.
// 
//

using System;
using System.Text;
using KiwiSystem;



namespace SmithWaterman
{
  
    public class Program_t
    {
        static char [] alphabet = new char [] {'a','c','d','e','f','g','h','i','k','l','m','n','p','q','r','s','t','v','w','y'};

        [Kiwi.OutputWordPort("phase")] static uint g_phase;
        [Kiwi.OutputWordPort("result")] static uint g_result;
        [Kiwi.OutputWordPort("d_monitor")] static long d_monitor;


        public void waypoint(int phase, int idx) 
	{
	  Console.WriteLine("waypoint {0} {1}", phase, idx);
          g_phase = (uint)(idx * 256 + phase);
	}

       //* Query Sequence = d1xpa_1 a.6.1.2 (134-210) from ASTRAL 1.67 40% 
       static char[] test_query = new char []{'d','k','h','k','l','i','t','k','t','e','a','k','q','e','y','l','l','k','d','c','d','l','e','k','r','e','p','p','l','k','f','i','v','k','k','n','p','h','h','s','q','w','g','d','m','k','l','y','l','k','l','q','i','v','k','r','s','l','e','v','w','g','s','q','e','a','l','e','e','a','k','e','v','r','q','e','n'};


       //* Database Sequence =  d1jjcb2 a.6.1.1 (B:400-474) from ASTRAL 1.67 40% 
       static char [] test_d = new char[] {'p','p','e','a','i','p','f','r','p','e','y','a','n','r','l','l','g','t','s','y','p','e','a','e','q','i','a','i','l','k','r','l','g','c','r','v','e','g','e','g','p','t','y','r','v','t','p','p','s','h','r','l','d','l','r','l','e','e','d','l','v','e','e','v','a','r','i','q','g','y','e','t','i','p','l'};

       static int dlen = test_d.Length;
       static int qlen = test_query.Length;

       //* Scoring Matrix (PAM 250) 
       static int  [,] SS = new int [20,20] {
       { 2, -2,  0,  0,  -4,   1,  -1,  -1,  -1,  -2,  -1,   0,   1,   0,  -2,   1,   1,   0,  -1,  -3},
       {-2, 12, -5, -5,  -4,  -3,  -3,  -2,  -5,  -6,  -5,  -4,  -3,  -5,  -4,   0,  -2,  -2,  -8,   0},
       { 0, -5,  4,  3,  -6,   1,   1,  -2,   0,  -4,  -3,   2,  -1,   2,  -1,   0,   0,  -2,  -7,  -4},
       { 0, -5,  3,  4,  -5,   0,   1,  -2,   0,  -3,  -2,   1,  -1,   2,  -1,   0,   0,  -2,  -7,  -4},
       {-4, -4, -6, -5,   9,  -5,  -2,   1,  -5,   2,   0,  -4,  -5,  -5,  -4,  -3,  -3,  -1,   0,   7},
       { 1, -3,  1,  0,  -5,   5,  -2,  -3,  -2,  -4,  -3,   0,  -1,  -1,  -3,   1,   0,  -1,  -7,  -5},
       {-1, -3,  1,  1,  -2,  -2,   6,  -2,   0,  -2,  -2,   2,   0,   3,   2,  -1,  -1,  -2,  -3,   0},
       {-1, -2, -2, -2,   1,  -3,  -2,   5,  -2,   2,   2,  -2,  -2,  -2,  -2,  -1,   0,   4,  -5,  -1},
       {-1, -5,  0,  0,  -5,  -2,   0,  -2,   5,  -3,   0,   1,  -1,   1,   3,   0,   0,  -2,  -3,  -4},
       {-2, -6, -4, -3,   2,  -4,  -2,   2,  -3,   6,   4,  -3,  -3,  -2,  -3,  -3,  -2,   2,  -2,  -1},
       {-1, -5, -3, -2,   0,  -3,  -2,   2,   0,   4,   6,  -2,  -2,  -1,   0,  -2,  -1,   2,  -4,  -2},
       { 0, -4,  2,  1,  -4,   0,   2,  -2,   1,  -3,  -2,   2,  -1,   1,   0,   1,   0,  -2,  -4,  -2},
       { 1, -3, -1, -1,  -5,  -1,   0,  -2,  -1,  -3,  -2,  -1,   6,   0,   0,   1,   0,  -1,  -6,  -5},
       { 0, -5,  2,  2,  -5,  -1,   3,  -2,   1,  -2,  -1,   1,   0,   4,   1,  -1,  -1,  -2,  -5,  -4},
       {-2, -4, -1, -1,  -4,  -3,   2,  -2,   3,  -3,   0,   0,   0,   1,   6,   0,  -1,  -2,   2,  -4},
       { 1,  0,  0,  0,  -3,   1,  -1,  -1,   0,  -3,  -2,   1,   1,  -1,   0,   2,   1,  -1,  -2,  -3},
       { 1, -2,  0,  0,  -3,   0,  -1,   0,   0,  -2,  -1,   0,   0,  -1,  -1,   1,   3,   0,  -5,  -3},
       { 0, -2, -2, -2,  -1,  -1,  -2,   4,  -2,   2,   2,  -2,  -1,  -2,  -2,  -1,   0,   4,  -6,  -2},
       {-6, -8, -7, -7,   0,  -7,  -3,  -5,  -3,  -2,  -4,  -4,  -6,  -5,   2,  -2,  -5,  -6,  17,   0},
       {-3,  0, -4, -4,   7,  -5,   0,  -1,  -4,  -1,  -2,  -2,  -5,  -4,  -4,  -3,  -3,  -2,   0,  10},
       };


       long[,] Ins = new long[2,  qlen+1]; long[,] Del = new long[2,  qlen+1]; long[,] H = new long[2,  qlen+1]; long[,] Hg = new long[2, qlen+1];

       int [] q_index = new int [qlen];

       public void sw_reset()	 //* Intialize Ins, Del, and H 
       {
          // Zeroth rows and cols forms sentinel zero margins. 
	  for (int dlv=0; dlv<2; dlv++)
	  {
	      Kiwi.Pause();
	      d_monitor = dlv;
	      Ins[dlv,0] = 0;
	      Del[dlv,0] = 0;
	      H[dlv,0] = 0;
	      Hg[dlv,0] = - 10;
	  }
	  for (int qlv=1; qlv<qlen+1; qlv++) {
	      Kiwi.Pause();
	      Ins[0,qlv] = 0;
	      Del[0,qlv] = 0;
	      H[0,qlv] = 0;
	      Hg[0,qlv] = - 10;
	    }
       }       


       CRC32checker maintest_crc = new CRC32checker();

       public int encode(char inch)
       {
          int j;
	  for (j=0; j<20; j++)
	     {
	       Kiwi.Pause();
 	       if (inch == alphabet[j]) return j;
	     }
  	  System.Diagnostics.Debug.Assert(j!=20);
          return 0;
	}



       public void install_query() // constructor time perhaps.
       {
	 // Encode from alphabetic to score table indecies.
	 for (int q_loc=0; q_loc<qlen; q_loc++)
	 {
	    q_index[q_loc] = encode(test_query[q_loc]);
	 }
       }


       public void runTest(int iterations)
       {
         waypoint(2, 0); 
         install_query();
         waypoint(3, 0);
	 for (int i=0; i<iterations; i++)
	 {
            sw_reset();
	    //            for (int col_no=0; col_no<dlen; col_no++)
	    // Limit the scope
            for (int col_no=0; col_no<3; col_no++)
            {
	      waypoint(6, col_no);
              next_data(col_no, test_d[col_no]);
              testReport(col_no);
              crcReport(col_no);
            }
         } 
         waypoint(12, 0);
       }
 
      public void next_data(int col_no, char chin)
       {
          waypoint(4, 0);
         // Do scoring 
	 int current = (col_no) % 2;
	 int next    = (col_no+1) % 2;
         int stream_data = encode(chin);
	 for (int rowidx=0; rowidx<qlen; rowidx++) 
	   {
	     Kiwi.Pause();
	     int rowchar = q_index[rowidx];
	     long Ins_c = Ins[current,rowidx+1] - 2;
	     if (Hg[current,rowidx+1] > Ins_c) Ins[next,rowidx+1] = Hg[current,rowidx+1];
	     else Ins[next,rowidx+1] = Ins_c;

	     long Del_c = Del[next,rowidx] - 2;
	     if (Hg[next,rowidx] > Del_c) Del[next,rowidx+1] = Hg[next,rowidx];
	     else Ins[next,rowidx+1] = Del_c;

             long H_temp;
	     if (Ins[current,rowidx] > Del[current,rowidx])
	       H_temp = Ins[current,rowidx];
	     else
	       H_temp = Del[current,rowidx];
	     if (H_temp < H[current,rowidx]) H_temp = H[current,rowidx];
/*
	     Console.WriteLine("rowidx {0} rowchar={1} stream_data={2} ", rowidx, rowchar, stream_data);
	     Console.WriteLine("score ={0}", SS[rowchar, stream_data]);
	     Console.WriteLine("score 0 0 ={0}", SS[0, 0]);
	     Console.WriteLine("score 1 0 ={0}", SS[1, 0]);
	     Console.WriteLine("score 0 1 ={0}", SS[0, 1]);
	     Console.WriteLine("score 1 1 ={0}", SS[1, 1]);
*/
	     //	     Console.WriteLine("hsum {0} + {1} = {2}", H_temp, SS[rowchar, stream_data], H_temp + SS[rowchar, stream_data]);
	     H[next,rowidx+1] = H_temp + SS[rowchar, stream_data];

	     if (H[next,rowidx+1] < 0) H[next,rowidx+1] = 0;
	     Hg[next,rowidx+1] = H[next,rowidx+1] - 10;
          }
      }
      
      public void testReport(int col_no)
      {
        waypoint(8, col_no);
        Console.WriteLine("Scored h matrix {0}", col_no);
        for (int rr = 0; rr < 2; rr++)
         {
	   int i = (rr + col_no) % 2;
           for (int j = 0; j < qlen; j++)
            {
              Kiwi.Pause();
	      long dd = H[i,j];
              Console.Write("{0} {1} : {2}   ", i, j, dd);
              Console.WriteLine("");
              //g_result = dd;
	    }
         }
        waypoint(10, col_no);
      }

      public void crcReport(int col_no)
      {
        maintest_crc.reset(); 
        int i = (1 + col_no) % 2;
        for (int j = 0; j < qlen; j++)
	{
	  long dd = H[i, j];
   	  maintest_crc.process_word((uint)dd);
        }
        Console.WriteLine("step {0} crc is {1}    0x{1:X}", col_no,  maintest_crc.get());
        d_monitor = maintest_crc.get();
      }
   }
}




class CRC32checker
{
  const uint ethernet_polynomial_le = 0xedb88320U;

  static uint [] crc32_table = new uint[]
   {  // Byte-oriented CRC32 
       0x00000000, 0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, //0
       0x17C56B6B, 0x1A864DB2, 0x1E475005, 0x2608EDB8, 0x22C9F00F, //5
       0x2F8AD6D6, 0x2B4BCB61, 0x350C9B64, 0x31CD86D3, 0x3C8EA00A, //10
       0x384FBDBD, 0x4C11DB70, 0x48D0C6C7, 0x4593E01E, 0x4152FDA9, //15
       0x5F15ADAC, 0x5BD4B01B, 0x569796C2, 0x52568B75, 0x6A1936C8, //20
       0x6ED82B7F, 0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3, //25
       0x709F7B7A, 0x745E66CD, 0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, //30
       0x95609039, 0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5, //35
       0xBE2B5B58, 0xBAEA46EF, 0xB7A96036, 0xB3687D81, 0xAD2F2D84, //40
       0xA9EE3033, 0xA4AD16EA, 0xA06C0B5D, 0xD4326D90, 0xD0F37027, //45
       0xDDB056FE, 0xD9714B49, 0xC7361B4C, 0xC3F706FB, 0xCEB42022, //50
       0xCA753D95, 0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1, //55
       0xE13EF6F4, 0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D, 0x34867077, //60
       0x30476DC0, 0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C,
       0x2E003DC5, 0x2AC12072, 0x128E9DCF, 0x164F8078, 0x1B0CA6A1,
       0x1FCDBB16, 0x018AEB13, 0x054BF6A4, 0x0808D07D, 0x0CC9CDCA,
       0x7897AB07, 0x7C56B6B0, 0x71159069, 0x75D48DDE, 0x6B93DDDB, 
       0x6F52C06C, 0x6211E6B5, 0x66D0FB02, 0x5E9F46BF, 0x5A5E5B08,
       0x571D7DD1, 0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D,
       0x40D816BA, 0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E,
       0xBFA1B04B, 0xBB60ADFC, 0xB6238B25, 0xB2E29692, 0x8AAD2B2F,
       0x8E6C3698, 0x832F1041, 0x87EE0DF6, 0x99A95DF3, 0x9D684044,
       0x902B669D, 0x94EA7B2A, 0xE0B41DE7, 0xE4750050, 0xE9362689,
       0xEDF73B3E, 0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2,
       0xC6BCF05F, 0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683,
       0xD1799B34, 0xDC3ABDED, 0xD8FBA05A, 0x690CE0EE, 0x6DCDFD59,
       0x608EDB80, 0x644FC637, 0x7A089632, 0x7EC98B85, 0x738AAD5C,
       0x774BB0EB, 0x4F040D56, 0x4BC510E1, 0x46863638, 0x42472B8F,
       0x5C007B8A, 0x58C1663D, 0x558240E4, 0x51435D53, 0x251D3B9E,
       0x21DC2629, 0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5,
       0x3F9B762C, 0x3B5A6B9B, 0x0315D626, 0x07D4CB91, 0x0A97ED48,
       0x0E56F0FF, 0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623,
       0xF12F560E, 0xF5EE4BB9, 0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2,
       0xE6EA3D65, 0xEBA91BBC, 0xEF68060B, 0xD727BBB6, 0xD3E6A601,
       0xDEA580D8, 0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD, 0xCDA1F604,
       0xC960EBB3, 0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7,
       0xAE3AFBA2, 0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B, 0x9B3660C6,
       0x9FF77D71, 0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD,
       0x81B02D74, 0x857130C3, 0x5D8A9099, 0x594B8D2E, 0x5408ABF7,
       0x50C9B640, 0x4E8EE645, 0x4A4FFBF2, 0x470CDD2B, 0x43CDC09C,
       0x7B827D21, 0x7F436096, 0x7200464F, 0x76C15BF8, 0x68860BFD,
       0x6C47164A, 0x61043093, 0x65C52D24, 0x119B4BE9, 0x155A565E,
       0x18197087, 0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B,
       0x0FDC1BEC, 0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088,
       0x2497D08D, 0x2056CD3A, 0x2D15EBE3, 0x29D4F654, 0xC5A92679,
       0xC1683BCE, 0xCC2B1D17, 0xC8EA00A0, 0xD6AD50A5, 0xD26C4D12,
       0xDF2F6BCB, 0xDBEE767C, 0xE3A1CBC1, 0xE760D676, 0xEA23F0AF,
       0xEEE2ED18, 0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4,
       0x89B8FD09, 0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5,
       0x9E7D9662, 0x933EB0BB, 0x97FFAD0C, 0xAFB010B1, 0xAB710D06,
       0xA6322BDF, 0xA2F33668, 0xBCB4666D, 0xB8757BDA, 0xB5365D03,
      0xB1F740B4 };

    uint crc_reg;
    uint byteno = 0;

    public void reset()
    {
       Console.WriteLine("CRC RESET");
       crc_reg = 0xFFffFFff; 
       Console.WriteLine("crc reg now:  reg={0:X}",  crc_reg);
       Kiwi.Pause();
     }

    public uint get() { return crc_reg; }

    public void process_word(uint dd)
    {
      if (true) Console.WriteLine("process unit word {0} word={1}", byteno, dd);
      process_byte((byte)((dd >> 0) & 0xFF));
      process_byte((byte)((dd >> 8) & 0xFF));
      process_byte((byte)((dd >> 16) & 0xFF));
      process_byte((byte)((dd >> 32) & 0xFF));
    }

    public void process_byte(byte dd)
    {
      byte cc = (byte)((crc_reg >> 24) & 0xFF); // 
      //Console.WriteLine("crc updating : dd={0:X}, reg={1:X},  reg<<8={2:X}",  dd, crc_reg , crc_reg << 8);
      byteno ++;
      Kiwi.Pause();
      //Console.WriteLine("crc byteno={0} dd={1} cc={2:X}", byteno, dd, cc);
      //   Console.WriteLine("crc update hex:  reg={0:X},  tab[dd]={1:X},  tab[cc]={2:X}",  crc_reg , crc32_table[dd], crc32_table[cc]);
      crc_reg = 0xFFFFffff & ((crc_reg << 8) ^ crc32_table[dd] ^ crc32_table[cc]);
      //Console.WriteLine("crc reg now (no={0}):  reg={1:X}",  byteno, crc_reg);
      //Console.WriteLine(" table 0x3a={0:X}  0x3b={1:X}", crc32_table[0x3a], crc32_table[0x3b]);

    }

    public void selftest()
    {
      reset();

      Kiwi.Pause();
      crc_reg = 0xFFffFFff; 
      Kiwi.Pause();
      Console.WriteLine("self test startup {0} a ", crc32_table[255]);
      Kiwi.Pause();
      byte cc = (byte)((crc_reg >> 24) & 0xFF);
      Console.WriteLine("self test startup {0} b ", cc);
      Kiwi.Pause();
      Console.WriteLine("self test startup {0} c ", crc32_table[cc]);

      process_byte(1);              Kiwi.Pause();
      process_byte(128);            Kiwi.Pause();
      process_byte(1);              Kiwi.Pause();
      Console.WriteLine("self test yields {0} (should be 821105832)", crc_reg);
/*
crc update hex: dd=01, reg=FFFFFFFF,  tab[dd]=4C11DB7,  tab[cc]=B1F740B4
crc update hex: dd=80, reg=4AC9A203,  tab[dd]=690CE0EE,  tab[cc]=1B0CA6A1
crc update hex: dd=01, reg=BBA2454F,  tab[dd]=4C11DB7,  tab[cc]=9675461F
*/
    }
}


class sw_test_single_threaded
{

    [Kiwi.OutputWordPort("result")] static uint g_resultx;

    [Kiwi.HardwareEntryPoint()]
    public static void Main()
    {
      int iterations = 1;
      Console.WriteLine("Smith Waterman Simple Test Start. Iterations={0}", iterations);
      
      SmithWaterman.Program_t sw = new SmithWaterman.Program_t();
      sw.runTest(iterations);

      Console.WriteLine("Smith Waterman CRC self check now.");
      CRC32checker selfcheck_crc = new CRC32checker();
      selfcheck_crc.selftest();
      Console.WriteLine("Smith Waterman Simple Test End.");
    }	
}

/*

1 60 : 16   
1 61 : 7   
1 62 : 6   
1 63 : 6   
1 64 : 3   
1 65 : 7   
1 66 : 13   
1 67 : 15   
1 68 : 9   
1 69 : 9   
1 70 : 16   
1 71 : 13   
1 72 : 19   
1 73 : 22   
1 74 : 17   
1 75 : 22   
1 76 : 21   
waypoint 10 74
step 74 crc is 459833601
waypoint 12 0
Smith Waterman Simple Test End.

*/

// eof
