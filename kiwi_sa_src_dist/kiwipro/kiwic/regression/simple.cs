// Test 5
// Subroutine call testing: please add more forms.
// $Id: simple.cs,v 1.1 2009/03/11 21:52:48 djg11 Exp $
//

class simple
{
	static int dog = 0;

	static void bugger(int i, int j)
	{
//		if (i > 4) dog += 1;
		Kiwi.Pause();
	}


    public static void Main()
    {
    for(int i=0;i<10;i++) for(int j=0; j<5; j++)
       {
	  bugger(i, j);
	  Console.WriteLine("Hello {0},{1}", i, j);

 	}

    }
}
