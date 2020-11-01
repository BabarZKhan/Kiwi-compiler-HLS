//
// CBG Orangepath. HPR L/S Formal Codesign System.
//
// planhdr.fs - generic interface to a planner/optimiser controller.   
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


module planhdr

open yout


//
// Our simple planner provides a user function, called operatex, with a so-called oracle function which the user function
// calls for each decision it must make.
//
// The user function returns a configuration which can be converted to a cost with a another user provided function called expense_fn
//
// The planner records the decisions it made so that it can potentially do complex sensitivity analysis on them and so it can support
// a replay of a previous run.  Replaying a previous run might be cheaper in memory footprint than storing the result inside cfig_t: it
// is up to the user.

type planopt_decision_t = (string * int)

type planopt_decision_set_t = planopt_decision_t list

type planopt_oracle_function_t = string -> int -> int

type plantop_pprams_u_t<'cfig_t> =
    {
        operatex:     WW_t -> string -> planopt_oracle_function_t -> 'cfig_t
        merit_fn:     WW_t -> 'cfig_t -> double // Report the merit of a configuration - or -ve for an invalid design point.
    }

type planopt_pprams_sys_t =
    {
        attempt_limit: int
        acceptable_merit: double
    }




// eof

