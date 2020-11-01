




module Djg

type WeakReference<'a>(v :'a list) =
   let contents =
       match v with
        | [] -> failwith "no arg"
        | _ -> v

   // new (v) = { contents=v }
   
   member this.IsAlive() = true

   member this.Target() = contents


// eof


