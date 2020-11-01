//
// $Id: opath_interface.fs,v 1.1 2010-07-28 11:34:04 djg11 Exp $
//

namespace opath_interface

type IOpathPlugin =
    interface
       abstract MethodName : string
       abstract OpathInstaller: unit -> unit
    end


// eof
       
       
