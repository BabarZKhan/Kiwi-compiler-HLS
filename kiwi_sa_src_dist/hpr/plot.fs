(*
 * $Id: plot.fs,v 1.3 2012-03-28 23:13:52 djg11 Exp $  
 *
 * 
 * (C) 2003-8 DJ Greaves. Cambridge. CBG. Fsharp version (C) 2010
 * HPR simulator: all internal forms can be simulated with this simulator.
 *
 * diosim.sml - H2 machine simulator and yet another waveform viewer.
 *
 *
 *)

module plot

open moscow
open yout


type plotter_t (edict:(string->unit), name) = class
  member d.preamble(xs, ys) =
    let _ = edict( "X " + i2s xs + "\n")       
    let _ = edict( "Y " + i2s ys + "\n")       
    let _ = edict( "m 4 4\n")
    let _ = edict( "B EEEEEE\n")     
    let _ = edict( "c FFFF00\n")     
    let _ = edict( name + "\n")
    in ()
    
    // Graphics plotting functions
  member d.move x y = edict("m " + (i2s x) + " " + (i2s y) + "\n") 

  member d.draw x y col = edict("c " + col + "\n" + "d " + (i2s x) + " " + (i2s y) + "\n")        

  member d.oblong x0 y0 x1 y1 col = //unfilled
    (d.move x0 y0; d.draw x0 y1 col; d.draw x1 y1 col; d.draw x1 y0 col; d.draw x0 y0 col)

  member d.text x y msg col = 
        edict("c " + col + "\n" + "m " + (i2s x) + " " + (i2s y) + "\nt " + msg + "\n") 

  member d.line x0 y0 x1 y1 col =
    let _ = d.move x0 y0
    in d.draw x1 y1 col


  member d.postamble() =
      ()
end
;



type cbg_ps_plotter_t (edict:(string->unit), name) = class
  let mutable clastx = 0
  let mutable clasty = 0
  let mutable lastcol = (1,2,3)
    
  member d.preamble(xs, ys)  =
    let _ = edict ("%! plot output for postscript printers, cbg.\n");
    let _ = edict (".1 setlinewidth\n");
    let _ = edict ("72 300 div dup scale\n");
    // let _ = edict ("100 100 translate\n");
            // now some simple definitions 
    let _ = edict ("/m {moveto} def\n");
    let _ = edict ("/rgb {setrgbcolor} def\n");    
    let _ = edict ("/c {closepath} def\n");    
    let _ = edict ("/r {rlineto} def\n");
    let _ = edict ("/s {stroke} def\n");
    let _ = edict ("/c {0 360 arc} def\n");  // circle
    let _ = edict ("/t1 {/Times-Roman findfont 6 scalefont setfont}def\n");
    let _ = edict ("/t10{/Times-Roman findfont 41 scalefont setfont}def\n");
    in ()
    

  member d.color (r,g,b) =
    let x2f r = "0." + i2s (r * 256 / 100) 
    let _ = if lastcol <> (r,g,b) then edict(x2f r + " " + x2f g + " " + x2f b + " rgb\n")
    in (lastcol <- (r,g,b); ())

  member d.move x0 y0 =
    let _ = edict(i2s x0 + " " + i2s y0 + " m \n")
    in (clastx <- x0;  clasty <- y0; ())

  member d.draw x y col = // colour ?
    let _ = edict (i2s (x-clastx) + " " + i2s (y-clasty) + " r\n")    
    in (clastx <- x;  clasty <- y; ())

  member d.line x0 y0 x1 y1 col =
    let _ = d.move x0 y0
    let _ = d.color col
    let _ =  d.draw x1 y1 col
    in edict("s\n")

  member d.text x y msg col = // set size ?
    let _ = d.move x y
    let _ = d.color col
    let _ = edict ("\nt1(" + msg + ")show\n")
    in ()

  member d.oblong x0 y0 x1 y1 col =
    (d.move x0 y0; d.draw x0 y1 col; d.draw x1 y1 col; d.draw x1 y0 col; d.draw x0 y0 col; edict "s\n") // can issue a closepath afterwards if not separate lines

  member d.postamble() =
    // let _ = edict ("\"%s\" show\n", b);
    //  edict ("s\nshowpage\n");
    ()
end


// eof
