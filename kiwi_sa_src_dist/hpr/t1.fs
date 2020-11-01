
module myt1

open moscow


let _ = System.Console.WriteLine "Hello World"

let _ = new System.Collections.Generic.Dictionary<int, int>()

let v = new ListStore<int, int>("lists");

let _ = v.add 1 2
let _ = v.add 1 3

let _ =

    let bog x = System.Console.WriteLine ("Bog " + x)
    for a in System.Environment.GetCommandLineArgs() do bog a


let _ = System.Console.WriteLine (sprintf "Hello t %A" (v.lookup 1))

