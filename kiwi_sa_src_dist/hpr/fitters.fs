(* 
 * CBG Orangepath HPR/LS.
 * HPR Logic Synthesis and  Formal Codesign System.
 * (C) 2003-15 DJ Greaves. All rights reserved.  See license file with this distro for use and disclaimers.
 * 
 * fitters.fs
 *  
 * Contains:
 *    Matrix functions and regression fitters.
 *
 *
 *
 *)
// kpredict

module fitters

open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Numerics
open moscow
open yout

let matrix_mpx1 (adata:double [,]) (bdata: double[]) =
    let rows = Array2D.length1 adata
    let cols = Array2D.length2 adata
    let _ = if cols <> bdata.Length then sf "col vector length incompatiple for multiply"    
    let r = Array.create cols  0.0
    let _ = for row in 0..rows-1 do
               let pf cc col  = cc + adata.[row,col] * bdata.[col]
               Array.set r row (List.fold pf 0.0 [0..cols-1])
               done 
    r

let matrix_mpx2 (adata:double [,]) (bdata: double[,]) =
    let rows = Array2D.length1 adata
    let nq = Array2D.length2 adata
    let _ = if nq <> Array2D.length1 bdata then sf "arrays incompatiple for multiply"
    let cols = Array2D.length2 bdata 
    let r = Array2D.create cols rows 0.0
    let _ = for col in 0..cols-1 do
               for row in 0..rows-1 do
                   let pfsq cc n  = cc + adata.[row, n] * bdata.[n, col]
                   Array2D.set r row col (List.fold pfsq 0.0 [0..nq-1])
                   done 
               done
    r
     
let lu_decompose (adata:double [,]) =
    let n = Array2D.length1 adata
    let ll = Array2D.create n n  0.0
    let uu = adata |> Array2D.map (fun x->x)
    let pivot_enable = true
//    for v in [0..n-1] do Array2D.set ll v v 1.0 done
    for k in [0..n-1] do
        if pivot_enable then
            let p = ref 0.0
            let k1 = ref 0
            for i in [k..n-1] do
                //vprintln 0 (sprintf "solvinG %i %i/%i" k i n)
                if abs(uu.[i,k]) > !p then // Pivoting : find largest 
                    let _ = p := abs(uu.[i,k])
                    let _ = k1 := k;
                    ()
                done
            for i in 0..n-1 do
                let t = uu.[k,i]
                let _ = Array2D.set uu k i uu.[!k1, i]
                Array2D.set uu !k1 i t
                done
        Array2D.set ll k k 1.0
        for i in k+1 .. n-1 do
            //Debug.Assert(Uu[k][k] != 0.0); // Singular matrix!
            let mu = uu.[i,k]/uu.[k,k]
            //let _ = vprintln 0 (sprintf "store l %i %i   := %f" i k mu)
            Array2D.set ll    i k mu // This simple store is all you need
            Array2D.set uu    i k 0.0
            for j in k+1 .. n-1 do
                Array2D.set uu i j (uu.[i,j] - mu * uu.[k,j])
                done
        done
    (ll, uu) // return L and U.


let fwdsSubst(ll:double [,]) (b: double []) =    // Forwards substitution to solve Ly=b
    let n = Array2D.length1 ll
    let y = Array.create n 0.0
    let _ = Array.set y 0 b.[0] 
    for i in 1 .. n-1 do
        let sum = List.fold (fun c j -> c + ll.[i,j] * y.[j]) 0.0 [0..i-1]
        let _ = Array.set y i (b.[i] - sum)
        ()
    done
    y

let backSubst (uu:double [,]) (y: double []) =    // Back substitution to solve Ux=y
    let n = Array2D.length1 uu
    let x = Array.create n 0.0
    let _ = Array.set x (n-1) (y.[n-1] / uu.[n-1, n-1]) 
    for ir in 1 .. n-1 do
        let i = n - 1 - ir
        let sum = List.fold (fun c j -> c + uu.[i,j] * x.[j]) 0.0 [i+1 .. n-1]
        let _ = Array.set x i ((y.[i] - sum)/uu.[i,i])
        ()
    done
    x

                    
let SimuSolve vd coefs rhs =
    let (ll, uu) = lu_decompose coefs
    let y = fwdsSubst ll rhs
    let ans = backSubst uu y
    let checkit () =
        if vd >= 5 then vprintln 5 (sprintf "Simusolve check coefs\n=%A\nll=\n%A\n uu=\n%A\nrecomb=\n%A\n rhs=%A\ny=%A ans=%A" coefs ll uu (matrix_mpx2 ll uu) rhs y ans)
        let rhs2 = matrix_mpx1 coefs ans
        if vd >= 5 then vprintln 5 (sprintf "SimuSolve Check - target rhs=%A, answer gives=%A" rhs rhs2)
        ()
    //let _ = checkit()
    ans


let test_solver__ () = 
    // The subscript order is [row,col].
    let testd =  (array2D [ [| 1.0; 2.0; 4.0 |]; [| 3.0; 8.0; 14.0 |]; ]) // A list of rows.
    let _ = vprintln 0 (sprintf "testd dimes are %i %i and 0,1 is %f" (Array2D.length1 testd) (Array2D.length2 testd) testd.[0,1])
    // testd dimes are 2 3 and 0,1 is 2.000000

    let testd2 = array2D [ [| 1.0; 2.0; 4.0 |]; [| 3.0; 8.0; 14.0 |]; [| 2.0; 6.0; 13.0 |] ]
    let (ll, uu) = lu_decompose testd2
    let _ = vprintln 0 (sprintf "Example decompose gives ll=\n%A\nand uu=\n%A\nrecombined=\n%A" ll uu (matrix_mpx2 ll uu))
    let r = SimuSolve 0 testd2 [| 1.0; 2.0; 3.0 |]
    ()

(*    //*ttp://mathforum.org/library/drmath/view/72047.html
   So now we'll use the notation Sjk to mean the sum of x_i^j * y_i^k.  (Note that S00 = n, the number of data points you have.)
             [ S40  S30  S20 ] [ a ]   [ S21 ]
             [ S30  S20  S10 ] [ b ] = [ S11 ]
             [ S20  S10  S00 ] [ c ]   [ S01 ]
*)


let do_regression style (xd:double list, yd:double list) =
    let vd = -1 //  -1 is debug printing off.
    let rec reg style (xd:double[], yd:double[]) =
        let n = double yd.Length
        let lst = [0 .. yd.Length - 1]
        let mom x y =
            let rec pow r d = if d=0 then 1.0 elif d=1 then r else r * pow r (d-1)
            let momf c n = (pow (double xd.[n]) x) * (pow (double yd.[n]) y) + c
            List.fold momf 0.0 lst

        let power_regression() = 
            let dolog (v:double []) = v |> Array.map Operators.log
            let (xl, yl) = (dolog xd, dolog yd)
            let mf:double->double = reg "linear" (xl, yl)
            (log >> mf >> exp)

        let constant_regression() =
            let mom_0_1 = mom 0 1
            let mf = fun _ -> mom_0_1 / n
            (mf)

        let quadratic_regression() =
            let mom_0_1 = mom 0 1
            let mom_1_0 = mom 1 0
            let mom_1_1 = mom 1 1
            let mom_2_0 = mom 2 0

            let coefs = Array2D.create 3 3 0.0
            Array2D.set coefs 2 0 (mom 4 0)
            Array2D.set coefs 2 1 (mom 3 0)
            Array2D.set coefs 2 2 (mom_2_0)    
            Array2D.set coefs 1 0 (mom 3 0)
            Array2D.set coefs 1 1 (mom_2_0)
            Array2D.set coefs 1 2 (mom_1_0)    
            Array2D.set coefs 0 0 (mom_2_0)
            Array2D.set coefs 0 1 (mom_1_0)
            Array2D.set coefs 0 2 (mom 0 0)    
            let rhs = Array.create 3 0.0
            Array.set rhs 2 (mom 2 1)
            Array.set rhs 1 (mom_1_1)
            Array.set rhs 0 (mom_0_1)
            let fit = SimuSolve vd coefs rhs
            vprintln 0 (sprintf "       quadratic   terms  %f +   %f x +  %f x^2" fit.[2] fit.[1] fit.[0])
            (fun x -> fit.[2] + fit.[1] * x + fit.[0] * x * x)

        let linear_regression() =
            let mom_0_1 = mom 0 1
            let mom_0_2 = mom 0 2
            let mom_1_0 = mom 1 0
            let mom_1_1 = mom 1 1
            let mom_2_0 = mom 2 0
            let slope = (n * mom_1_1 - (mom_1_0) * (mom_0_1)) / (n * mom_2_0 - (mom_1_0) * (mom_1_0)) 
            let intercept =  (mom_0_1 - slope * mom_1_0) / n
            let r = (n * mom_1_1 - mom_0_1 * mom_1_0) / (sqrt(n * mom_2_0 - (mom_1_0) * (mom_1_0)) * sqrt(n * mom_0_2 - (mom_0_1) * (mom_0_1)))
            vprintln 0 (sprintf "         linear   terms  a=%f  m=%f  r=%f" intercept slope r)
            (fun x -> intercept + slope * x)

        match style with
            | "constant" -> constant_regression()
            | "linear" -> linear_regression()            
            | "power-law" -> power_regression()
            | "quadratic" -> quadratic_regression()                        
            | other -> sf ("unsupported regression style: " + style)
    reg style (List.toArray xd, List.toArray yd)
    //reg style (xd |> Array.map double, yd |> Array.map double)
(*

Linear regression:
   Regression Equation(y) = a + bx
Slope(b) = (NΣXY - (ΣX)(ΣY)) / (NΣX2 - (ΣX)2)  
Intercept(a) = (ΣY - b(ΣX)) / N  =
   r = r_{xy} =\frac{n\sum x_iy_i-\sum x_i\sum y_i} {\sqrt{n\sum x_i^2-(\sum x_i)^2}~\sqrt{n\sum y_i^2-(\sum y_i)^2}}. 
*)


let run_regression pair_list =
    let n = length pair_list
    let nf = double n
    let (xd, yd) = List.unzip pair_list
    let ddata:(double list * double list) = (map double xd, map double yd)
    let styles = [ "quadratic"; "power-law"; "linear"; "constant" ] // list preferred among peers last

    let allz = conjunctionate (fun x->x=0.0) (snd ddata)
    let d0 = List.fold (fun c n -> c+n) 0.0 (fst ddata) // TODO - check maths behind this normaliser...
    let best = 
        if allz then
            let _ = vprintln 0 ("note all ordinates are zero")
            Some("allz", 0.0, fun _ -> 0.0)
        else

            let test style = 
                let fr = do_regression style ddata
                let goodness c (x, y) =
                    let d = (fr x)-y
                    c + d*d
                let ms_err = List.fold goodness 0.0 (List.zip (fst ddata) (snd ddata)) / nf
                let _ = vprintln 0 (sprintf "    Mean-sq-error for %s is %f" style ms_err)
                (style, ms_err / d0, fr)
            let best =
                let select_best sofar (style, ms_err, fr) =
                    if nonep sofar then Some(style, ms_err, fr)
                    elif f2o3(valOf sofar) < ms_err then sofar else Some(style, ms_err, fr)
                List.fold select_best None (map test styles)
            best
    let _ = vprintln 0 (sprintf "best fit was %A" best)
    best
    
// let _ = run_regression [ (1, 1); (2, 2); ]

// eof
