
Generate Primes using the Sieve Method.


This uses one large array of bools.  You can get this in DRAM or BRAM
according to its size (the limit variable in the source code) and the
commandline flag -res2-offchip-threshold=90

You may also need to adjust timelimit in the toplevel simulation
wrapper vsys.v of primes-offchip-with-cache.v

There are three versions of the algorithm controlled by evariant.




