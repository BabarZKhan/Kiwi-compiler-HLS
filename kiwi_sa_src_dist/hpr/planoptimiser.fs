//
// CBG Orangepath. HPR L/S Formal Codesign System.
//
//
// The core constructs of hprls are split between abstracte.sml and meox.sml.
// This file contains the main imperative and functional programming constructs.
// It stores all expressions in a 'normalish form' in a memoising heap expression data structure and performs a multititude of peep-hole optimisations..
// Many other generic expression handling routines are in this file as well.
//
// (C) 2003-17, DJ Greaves, University of Cambridge, Computer Laboratory.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met: redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer;
// redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution;
// neither the name of the copyright holders nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


module planoptimiser

open moscow
open yout

open planhdr


// initialise (expected_no_training_runs, policy_)

// void start_run(trial/replay) // We support two design styles: we can save a cfig from the best run - which is potentially its full result - or we can re-run the best run to get the full result from its side effects etc..

// int decision_point (string tagstring, int range)
// void end_run(double merit)  // merit of less than zero means run should be ignored, except that an identical run should never been conducted (as per always).
// void report()

// A run can be a trial or replay
type planner_mode_t =
    | PM_trial
    | PM_replay


// This simple planner just makes a number of runs and takes the best, but
// advanced learning techniques can be put behind the same interface based
// on hill climbing etc..
type box_t = { m_decisions: (string * int) list ref; m_replayer: (string * int) list ref; }

type PlanOptimiserSimple (expected_no_training_runs:int, seed) = class

    let m_trials = ref 0
    let m_decisions = ref 0    
                   
    let m_mode = ref PM_trial        
    let m_box = ref None;
    let m_pig = ref(seed:int)    
    let start_run ww  replayo seed trial_no =
        let box = { m_decisions= ref []; m_replayer= ref [] }
        m_box := Some box
        mutinc m_trials 1
        let mode =
            match replayo with
                | None -> PM_trial
                | Some h ->
                    box.m_replayer := h
                    PM_replay

        m_mode := mode
        ()
        // When using a simple pseudo generator, we do not need to record the decisions since we can potentially
        // regenerate them deterministically, but we do as a safety check and to support better search techniques in the future.

    
            
    let oracle_decide tag arity =

        let pgen =
            mutinc m_decisions 1
            let pgen arity =
                let v = !m_pig
                m_pig := v + 1 // TODO use Park Miller etc..
                v % arity
            pgen
        let box = valOf !m_box

        let ans = 
            match !m_mode with
            | PM_trial ->
                let ans = pgen arity
                mutadd box.m_decisions (tag, ans)
                ans
            | PM_replay ->
                    match !box.m_replayer with
                        | [] ->
                            hpr_yikes ("planopt - ran out of replay history")
                            pgen arity
                        | (tag0, ans)::tt ->
                            box.m_replayer := tt
                            if tag0 <> tag then hpr_yikes(sprintf "planopt: replay tag trajectories differ %s cf %s" tag0 tag)
                            ans
        ans
                            


    member x.oraclef tag arity = oracle_decide tag arity

    member x.report vd =
        vprintln 2(sprintf "planoptimiser report: %i runs, %i decisions total"  !m_trials !m_decisions) // No report
        
end






// Non OO interface and wrapper to any of the OO plugins.
let planopt_run ww msg_ pprams_sys pprams_u seed no_of_trials =  

    let po = new PlanOptimiserSimple(no_of_trials, seed) // This is pluggable by changing type

    let do_xrun ww seed trial_no =
        let runname = sprintf "planopt RUN%i" trial_no 
        let ww = WF 2 "planopt trial run start" ww runname
        let cfig = pprams_u.operatex ww runname po.oraclef
        let merit:double = pprams_u.merit_fn ww cfig
        let ww = WF 2 "planopt trial run finish" ww runname
        (cfig, merit)

    let rec planopt_iterator besto trial_no = 
        let (cfig, merit) = do_xrun ww ((seed+1)*trial_no*11) trial_no
        vprintln 3 (sprintf "planopt: trial run complete and achieved merit %f" merit)
        if merit <= pprams_sys.acceptable_merit then
            vprintln 2 (sprintf "planopt: acceptable merit achieved on trial %i" trial_no)
            (trial_no, cfig, merit)
        else
            let newbest =
                    match besto with
                        | None -> (trial_no, cfig, merit)
                        // We seek maximum merit, so keep the new one if it has higher merit than current best. 
                        | Some(no0, cfig0, merit0) ->
                            if merit > merit0 then (trial_no, cfig, merit) else (no0, cfig0, merit0)
            if trial_no = pprams_sys.attempt_limit then
                dev_println (* 2  *)(sprintf "planopt: returning design with highest merit, which was %f, achieved from trial %i." (f3o3 newbest) (f1o3 newbest))
                newbest
            else planopt_iterator (Some newbest) (trial_no+1)

    let (trial_no, cfig, merit) = planopt_iterator None 1
    let ww = WF 3 "planopt_run" ww "End"    
    po.report 2
    (cfig, po, trial_no)

        
// eof

