

open microcode;
open m6805_hdr;

[<EntryPoint>]
let main _ =
    let _ = System.Console.WriteLine "Hello from HPR test.fs"
    let _ = gen_gas_table m6805_set
    let _ = System.Console.WriteLine "Done HPR test.fs"    
    in 0




// EOF
