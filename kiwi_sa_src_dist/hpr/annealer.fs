(*
 * HPR L/S sml code for a simple optimiser framework
 *
 * $Id: annealer.fs,v 1.3 2013-07-22 16:10:27 djg11 Exp $
 *)

module annealer


open moscow
open yout


type 'stvec anneal_t =
    {
      mutate: WW_t -> 'stvec -> 'stvec;
      costfun: WW_t -> 'stvec-> float option;
      summaryfun: WW_t -> 'stvec -> string;
      bestkeep: WW_t -> float -> 'stvec -> unit;
      max_steps: int;
      vd:int;
    }
    
let annealer ww (gen:System.Random) (dd:anneal_t<'stvec>) (initial_setting:'stvec) =
    let ww = WF dd.vd "annealer" ww "start"
    let initial_temp = 100.0
    let initial_cost = valOf_or_fail "initial setting has invalid cost" (dd.costfun ww initial_setting)
    let rec stepper step temp setting cost stag bestcost =

        if step >= dd.max_steps then
            let ww = WF 3 "annealer" ww "steps limit reached"
            in setting
        elif temp < 1.0 then
            let ww = WF 3 "annealer" ww "temp zero reached"
            in setting
        else
        let setting' = dd.mutate ww setting
        let cost' = dd.costfun ww setting'
        if cost' = None then
            let _ = vprintln dd.vd "Invalid design cost - ignore this step"
            in stepper (step+1) temp setting cost stag bestcost
        else
        let better = valOf cost' < cost
        let worseness = if better || cost = 0.0 then 0.0 else abs(valOf cost' - cost)
        let best = valOf cost' < bestcost
        let _ = if best then dd.bestkeep ww (valOf cost') setting'
        let (stag', temp') =
            if stag > 10.0 then (stag * 0.3, temp * 0.8) 
            elif better then (0.5 * stag, temp)
            else (1.0 + 1.33 * stag, temp)
        // Quench at 10 degrees.
        let flip = if temp < 10.0 then false else gen.Next(1,150) < int(temp)
        let keep = if flip && worseness < 300.0 then not better else better
        let _ = vprintln dd.vd (sprintf "Annealer step=%i temp=%f stag=%f cost=%f better=%A keep=%A best=%A/%f %s" step temp stag (valOf cost') better keep best bestcost (dd.summaryfun ww setting'))        
        let bestcost' = if best then valOf cost' else bestcost
        let (setting', cost') = if keep then (setting', valOf cost') else (setting, cost)
        in stepper (step+1) temp' setting' cost' stag' bestcost'

    in stepper 0 initial_temp initial_setting initial_cost 0.0 initial_cost

let annealer_test ww (gen:System.Random) =
    let costfun ww lst = Some(float(List.fold (fun a c->a+c) 0 lst))
    let mutate ww lst =
        let item = gen.Next(0,length lst)
        let delta = gen.Next(0,2) * 2 - 1
        let rec q p = function
            | [] -> []
            | h::t when p=item -> (h+delta)::t
            | h::t -> h :: (q (p+1) t)
        in q 0 lst
    let bestkeep ww cost setting = ()
    let summaryfun ww = sfold (sprintf "%i ") 
    let (searchspace:int list anneal_t) = { costfun=costfun; mutate=mutate; max_steps=2000; summaryfun=summaryfun; vd= 0; bestkeep=bestkeep; }
    let id = [ for i in [1..10] -> gen.Next(2, 100) ]
    let ic = costfun ww id
    let ans = annealer ww gen searchspace id 

    in ans

// end
    
