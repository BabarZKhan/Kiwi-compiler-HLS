// Kiwi Scientific Acceleration

// Random Number Generator
// Replaces WD System.Random for FPGA.

namespace KiwiSystem
{

   public class Random
   {

	   // 512 numbers in the ushort range
      static readonly ushort [] RandomPad512 = {
       (ushort)41381, (ushort)2842, (ushort)12847, (ushort)115,
       (ushort)51851, (ushort)62619, (ushort)5591, (ushort)15691,
       (ushort)56223, (ushort)47144, (ushort)32854, (ushort)6272,
       (ushort)45098, (ushort)61032, (ushort)5601, (ushort)13928,
       (ushort)16853, (ushort)49736, (ushort)2970, (ushort)4930,
       (ushort)55517, (ushort)60418, (ushort)34918, (ushort)23006,
       (ushort)59083, (ushort)41109, (ushort)52673, (ushort)65349,
       (ushort)34084, (ushort)57302, (ushort)63527, (ushort)41163,
       (ushort)32746, (ushort)50128, (ushort)47363, (ushort)7747,
       (ushort)18910, (ushort)29739, (ushort)1590, (ushort)11845,
       (ushort)14021, (ushort)10429, (ushort)23970, (ushort)46776,
       (ushort)56899, (ushort)13958, (ushort)58718, (ushort)2065,
       (ushort)38908, (ushort)31425, (ushort)25443, (ushort)46587,
       (ushort)38912, (ushort)38852, (ushort)49649, (ushort)44124,
       (ushort)20690, (ushort)24524, (ushort)38894, (ushort)19423,
       (ushort)26230, (ushort)31242, (ushort)63227, (ushort)52608,
       (ushort)44125, (ushort)50811, (ushort)37650, (ushort)59598,
       (ushort)24096, (ushort)17767, (ushort)12757, (ushort)48322,
       (ushort)346, (ushort)20362, (ushort)12780, (ushort)18294,
       (ushort)9496, (ushort)48203, (ushort)58652, (ushort)19739,
       (ushort)33191, (ushort)11138, (ushort)27119, (ushort)27372,
       (ushort)53701, (ushort)2224, (ushort)44052, (ushort)14087,
       (ushort)7270, (ushort)1451, (ushort)13528, (ushort)47713,
       (ushort)3243, (ushort)47897, (ushort)32610, (ushort)17999,
       (ushort)25526, (ushort)54603, (ushort)38526, (ushort)41802,
       (ushort)34401, (ushort)46153, (ushort)56375, (ushort)56034,
       (ushort)42789, (ushort)44575, (ushort)64905, (ushort)27422,
       (ushort)4773, (ushort)12993, (ushort)16087, (ushort)34217,
       (ushort)45997, (ushort)59672, (ushort)32781, (ushort)6541,
       (ushort)63897, (ushort)32003, (ushort)37251, (ushort)51812,
       (ushort)60014, (ushort)15975, (ushort)46348, (ushort)42041,
       (ushort)20518, (ushort)57754, (ushort)28879, (ushort)20359,
       (ushort)40694, (ushort)43455, (ushort)11220, (ushort)50432,
       (ushort)26751, (ushort)1507, (ushort)26196, (ushort)1315,
       (ushort)60993, (ushort)12775, (ushort)49739, (ushort)9147,
       (ushort)43973, (ushort)48283, (ushort)31435, (ushort)3017,
       (ushort)32584, (ushort)60549, (ushort)23319, (ushort)48264,
       (ushort)28122, (ushort)56628, (ushort)49287, (ushort)25717,
       (ushort)56905, (ushort)55094, (ushort)46254, (ushort)43931,
       (ushort)35914, (ushort)54507, (ushort)46937, (ushort)45700,
       (ushort)25507, (ushort)24295, (ushort)25161, (ushort)51087,
       (ushort)59748, (ushort)60062, (ushort)43732, (ushort)39069,
       (ushort)55135, (ushort)44657, (ushort)10173, (ushort)45879,
       (ushort)5234, (ushort)28275, (ushort)27639, (ushort)31155,
       (ushort)16985, (ushort)7543, (ushort)14640, (ushort)20478,
       (ushort)45073, (ushort)20018, (ushort)1848, (ushort)42748,
       (ushort)40460, (ushort)22669, (ushort)8128, (ushort)47550,
       (ushort)26549, (ushort)51058, (ushort)22519, (ushort)17389,
       (ushort)21513, (ushort)7075, (ushort)17408, (ushort)13608,
       (ushort)34123, (ushort)12415, (ushort)39506, (ushort)26949,
       (ushort)48649, (ushort)38827, (ushort)24420, (ushort)47525,
       (ushort)30723, (ushort)4363, (ushort)23645, (ushort)20496,
       (ushort)6056, (ushort)56463, (ushort)21895, (ushort)65475,
       (ushort)56633, (ushort)931, (ushort)28612, (ushort)36273,
       (ushort)37571, (ushort)57444, (ushort)13916, (ushort)15644,
       (ushort)56456, (ushort)6365, (ushort)48325, (ushort)43512,
       (ushort)32636, (ushort)64739, (ushort)27204, (ushort)63186,
       (ushort)59388, (ushort)46430, (ushort)6042, (ushort)38957,
       (ushort)47325, (ushort)64547, (ushort)18690, (ushort)37755,
       (ushort)12653, (ushort)9103, (ushort)31041, (ushort)56368,
       (ushort)50979, (ushort)3485, (ushort)40084, (ushort)38303,
       (ushort)26103, (ushort)21029, (ushort)15527, (ushort)25742,
       (ushort)26799, (ushort)15472, (ushort)53496, (ushort)42934,
       (ushort)32926, (ushort)30821, (ushort)16702, (ushort)25252,
       (ushort)29332, (ushort)16359, (ushort)53706, (ushort)58024,
       (ushort)50445, (ushort)39216, (ushort)5100, (ushort)43459,
       (ushort)14798, (ushort)48968, (ushort)54991, (ushort)63441,
       (ushort)13189, (ushort)7634, (ushort)32615, (ushort)24147,
       (ushort)63007, (ushort)25059, (ushort)41050, (ushort)10128,
       (ushort)1763, (ushort)401, (ushort)9460, (ushort)65030,
       (ushort)17512, (ushort)58530, (ushort)11599, (ushort)3538,
       (ushort)18184, (ushort)65155, (ushort)2560, (ushort)34672,
       (ushort)44161, (ushort)61171, (ushort)35352, (ushort)61474,
       (ushort)45120, (ushort)60218, (ushort)56336, (ushort)61032,
       (ushort)5032, (ushort)45504, (ushort)54950, (ushort)45372,
       (ushort)62094, (ushort)42842, (ushort)28644, (ushort)38805,
       (ushort)57936, (ushort)52390, (ushort)1543, (ushort)190,
       (ushort)57141, (ushort)53387, (ushort)10568, (ushort)31419,
       (ushort)29764, (ushort)18245, (ushort)49584, (ushort)10815,
       (ushort)14798, (ushort)55188, (ushort)57443, (ushort)30199,
       (ushort)16209, (ushort)519, (ushort)18469, (ushort)46383,
       (ushort)13682, (ushort)104, (ushort)48378, (ushort)57357,
       (ushort)13619, (ushort)7620, (ushort)30493, (ushort)1507,
       (ushort)53672, (ushort)53666, (ushort)43450, (ushort)40038,
       (ushort)47558, (ushort)56143, (ushort)59944, (ushort)37052,
       (ushort)36890, (ushort)37988, (ushort)26754, (ushort)25005,
       (ushort)50809, (ushort)46052, (ushort)22245, (ushort)28795,
       (ushort)15366, (ushort)50581, (ushort)37044, (ushort)9236,
       (ushort)5128, (ushort)43900, (ushort)36776, (ushort)28853,
       (ushort)48256, (ushort)39307, (ushort)133, (ushort)54323,
       (ushort)25464, (ushort)7683, (ushort)54714, (ushort)56681,
       (ushort)10860, (ushort)62220, (ushort)35417, (ushort)58265,
       (ushort)16215, (ushort)34934, (ushort)25614, (ushort)43196,
       (ushort)41790, (ushort)28355, (ushort)59164, (ushort)16377,
       (ushort)52801, (ushort)3804, (ushort)23421, (ushort)47068,
       (ushort)37257, (ushort)25612, (ushort)13987, (ushort)46584,
       (ushort)36777, (ushort)49199, (ushort)31115, (ushort)26626,
       (ushort)25664, (ushort)64556, (ushort)38670, (ushort)60583,
       (ushort)7143, (ushort)55586, (ushort)17067, (ushort)40090,
       (ushort)60876, (ushort)49384, (ushort)39984, (ushort)27292,
       (ushort)16515, (ushort)34503, (ushort)35078, (ushort)59277,
       (ushort)58331, (ushort)17284, (ushort)45051, (ushort)21935,
       (ushort)4562, (ushort)59943, (ushort)30927, (ushort)40119,
       (ushort)14582, (ushort)48417, (ushort)42224, (ushort)56925,
       (ushort)51909, (ushort)23979, (ushort)37647, (ushort)57717,
       (ushort)18653, (ushort)51590, (ushort)913, (ushort)34767,
       (ushort)37903, (ushort)15740, (ushort)16380, (ushort)2401,
       (ushort)4263, (ushort)40263, (ushort)56283, (ushort)48514,
       (ushort)35710, (ushort)46009, (ushort)57810, (ushort)53548,
       (ushort)60321, (ushort)22609, (ushort)9151, (ushort)26518,
       (ushort)42148, (ushort)18838, (ushort)37172, (ushort)18589,
       (ushort)36342, (ushort)21981, (ushort)60323, (ushort)28353,
       (ushort)15294, (ushort)15970, (ushort)60424, (ushort)31929,
       (ushort)13853, (ushort)39621, (ushort)61925, (ushort)24804,
       (ushort)29665, (ushort)58730, (ushort)40930, (ushort)13065,
       (ushort)19968, (ushort)49979, (ushort)24164, (ushort)42820,
       (ushort)47465, (ushort)35787, (ushort)39145, (ushort)18179,
       (ushort)62712, (ushort)4329, (ushort)44807, (ushort)65387,
       (ushort)432, (ushort)42775, (ushort)65096, (ushort)31391,
       (ushort)11131, (ushort)31342, (ushort)7764, (ushort)10152,
       (ushort)41222, (ushort)19952, (ushort)25886, (ushort)51977,
       (ushort)39323, (ushort)56649, (ushort)3326, (ushort)61493,
       (ushort)53228, (ushort)15230, (ushort)58099, (ushort)39544,
       (ushort)10565, (ushort)45852, (ushort)14205, (ushort)8706,
       (ushort)14486, (ushort)57031, (ushort)59572, (ushort)16428,
       (ushort)60738, (ushort)60252, (ushort)49323, (ushort)60450,
       (ushort)9647, (ushort)14064, (ushort)3885, (ushort)53132,
       (ushort)3580, (ushort)51811,  (ushort)13580,  (ushort)53128
};


       ushort x1, x2, x3;

       public void Reset(int seed)
       {
	  x3 = 0;
	  x2 = (ushort)(seed >> 16);
	  x1 = (ushort)(seed & 0xFFFF);
       }

       public Random()       // constructor
       {
         Reset(0);
       }

       public Random(int seed)       // constructor
       {
         Reset(seed);
       }


       public int Next()
       {
       //       System.Console.WriteLine("Random.Next {0} {1} {2}", x1, x2, x3);
	  x3 = (ushort)(x3 + 1);
	  x1 += (ushort) (x3 ^ x2);
	  int h = ((x1 >> 10) ^ x1) % RandomPad512.Length;
	  x2 = (ushort)(x2 ^ RandomPad512[h]);
	  return (x2 << 16) + x1;
       }

       public double NextDouble()
       {
         int n1 = System.Math.Abs(Next());
	 int n2 = System.Math.Abs(Next());
	 return (double)n1 / (double)n2;
       }

       public int Next(int upto)
       {
         int r0 = Next();
	 return r0 % upto;
       }

       public int Next(int minValue, int maxValue)
       {
         int r0 = Next();
	 return (r0 % (1 + maxValue - minValue)) + minValue;
       }


     /* Marsagilas Stack Overflow 1640258

     */

     /* Randu : v = (65539 * v) mod (2^31)
	          != v + (v<<1) + (v<<16)

	public int RanduNext()
	{
	seed = seed + (seed << 1) + (seed << 16);
	return seed;
	}
     */ 

     /* PRGEN Park and Miller - CACM October 1988
	public int Next()
	{
	const int a = 2147001325;
	const int b = 715136305;
	seed = a * seed + b;
	return seed;
	}
     */ 
   }
}


// eof
