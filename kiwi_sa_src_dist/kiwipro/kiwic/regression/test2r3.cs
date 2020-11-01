// $Id: test2r2.cs,v 1.5 2011/09/28 10:49:33 djg11 Exp $
// Kiwi Scientific Acceleration: Kiwic compiler test/demo.
//
// Shared variable concurrency test.
//
// NB: Single compilation unit for sender and receiver ... how to make separate?
//

using System;
using System.Text;
using System.Threading;
using KiwiSystem;


// MSDN web site: The following sample uses threads and lock. As long as the lock statement is present,
// the statement block is a critical section and balance will never become a negative number.

class Account
{
  private Object thisLock = new Object();
  int balance;

  Random r = new Random();

  public Account(int initial)
  {
    balance = initial;
  }

  int Withdraw(int amount)
  {

  // This condition never is true unless the lock statement is commented out.
  if (balance < 0)
  {
    throw new Exception("Negative Balance");
  }

// Comment out the next line to see the effect of leaving out
// the lock keyword.
   lock (thisLock)
   {
     if (balance >= amount)
     {
       Console.WriteLine("Balance before Withdrawal :  " + balance);
       Console.WriteLine("Amount to Withdraw        : -" + amount);
       balance = balance - amount;
       Console.WriteLine("Balance after Withdrawal  :  " + balance);
       return amount;
     }
   else
   {
     return 0; // transaction rejected
   }
  }
}

  public void DoTransactions()
  {
      for (int i = 0; i < 100; i++)
     {
       Withdraw(r.Next(1, 100));
     }
   }
}


class Test2r3
{

  [Kiwi.HardwareEntryPoint()]
  static void Main()
  {
    Console.WriteLine("Test2r3 start");
    Thread[] threads = new Thread[10];
    Account acc = new Account(1000);
    for (int i = 0; i < 10; i++)
    {
      Thread t = new Thread(new ThreadStart(acc.DoTransactions));
      threads[i] = t;
    }
    for (int i = 0; i < 10; i++)
    {
       threads[i].Start();
    }
   Console.WriteLine("Test2r3 finished.");
  }
}

// eof
